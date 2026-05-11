# HIS 特殊计价前端接入说明

本目录提供批次十一的 HIS WinForms 前端代码，目标运行环境为 .NET Framework 3.5。

接入形态按“合理用药”这类产品化模式设计：HIS 只引用 `HIS.Pricing.Client.dll` 并调用 Agent/SDK 接口，特殊计价弹窗和规则工作台由本 DLL 自己负责，HIS 不需要重写计价 UI。

## 文件说明

- `PricingApiClient.cs`：基于 `HttpWebRequest` 的同步 HTTP 客户端，不使用 `HttpClient` 和 `async/await`。
- `PricingApiUrlBuilder.cs`：统一构造后端管理接口 URL，避免查询参数拼接错误。
- `PricingRuleDtos.cs`：规则工作台需要的请求/响应 DTO。
- `PricingSdkOptions.cs`：SDK/Agent 运行配置，封装服务地址、超时、重试和渠道编码。
- `PricingSdk.cs`：无界面的产品 SDK，封装 special-flag、simulate、confirm、commit、cancel、reverse。
- `PricingChargeContext.cs`：单项目收费动作的轻量上下文，用于减少 HIS 侧 DTO 组装代码。
- `PricingAgent.cs`：产品化 Agent 入口，HIS 调一个接口，Agent 自己判断特殊项目并弹出自己的收费确认窗口。
- `PricingHisIntegrationHelper.cs`：收费入口接入辅助，封装特殊项目判断、弹窗、commit/cancel。
- `FrmPricingRuleWorkbench.cs`：特殊计价规则维护工作台。
- `FrmPricingPopup.cs`：收费前特殊计价弹窗。

## 产品化推荐接入

推荐新医院或新 HIS 按 `PricingAgent` 接入。HIS 只负责三件事：

1. 收费保存前调用 `ConfirmBeforeCharge`。
2. 如果返回 `Confirmed = true`，按 `Response.Items`、`ReplacementItem`、`ChildItems` 回填 HIS 真实收费明细。
3. HIS 本地落账成功后调用 `CommitAfterHisSuccess`，落账失败才调用 `CancelAfterHisFailure`。

```csharp
PricingSdkOptions options = new PricingSdkOptions();
options.BaseUrl = "http://pricing-rule-center-host";
options.SourceSystem = "HIS_WY";
options.DefaultChargeScene = "OUTPATIENT";

PricingAgent agent = new PricingAgent(options);

PricingChargeContext context = new PricingChargeContext();
context.PatientId = patientId;
context.VisitId = visitId;
context.ChargeNo = chargeNo;
context.BusinessRequestNo = businessRequestNo;
context.OperatorId = currentUserId;
context.OperatorName = currentUserName;
context.ItemCode = itemCode;
context.ItemName = itemName;
context.InputQty = qty;
context.Unit = unit;
context.UnitPrice = unitPrice;
context.BodyPartCode = bodyPartCode;

PricingAgentChargeResult result = agent.ConfirmBeforeCharge(this, context);
if (!result.AllowCharge)
{
    MessageBox.Show(result.Message);
    return;
}

if (result.OrdinaryPricing)
{
    // 非特殊项目，继续 HIS 原收费流程。
    SaveOrdinaryCharge();
    return;
}

// 特殊项目：HIS 必须按 result.Response 的最终数量、金额、替换子项、加收子项落账。
bool hisChargeSaved = SaveChargeByPricingResult(result.Response);
if (!hisChargeSaved)
{
    agent.CancelAfterHisFailure(result.RequestId);
    return;
}

List<PricingCommitActualItemRequest> actualItems = BuildActualItemsFromHisSavedDetails(savedDetails);
agent.CommitAfterHisSuccess(result.RequestId, hisChargeNo, actualItems, hisActualTotalAmount);
```

旧工程如暂时不想引入 Agent，也可以继续使用 `PricingHisIntegrationHelper`；该类保留兼容，后续新接入优先使用 `PricingAgent` 或 `PricingSdk`。

## HIS 菜单接入

在 HIS 菜单注册或模块加载点中创建工作台：

```csharp
PricingSdkOptions options = new PricingSdkOptions();
options.BaseUrl = "http://pricing-rule-center-host";
PricingAgent agent = new PricingAgent(options);
agent.OpenRuleWorkbench(this, currentUserId);
```

医院现有菜单注册方式尚未在本仓库确认，因此本目录只提供可加载窗体，不硬编码 HIS 菜单框架。

## 本地构建前置

- 安装 .NET Framework 3.5 reference assemblies / targeting pack。
- 还原 `packages.config` 中的 Newtonsoft.Json 到仓库根目录 `packages`，或按本院 HIS 工程的 NuGet 包路径调整 `HIS.Pricing.Client.csproj` 的 `HintPath`。

## 收费入口接入

以下是兼容旧接入层的写法。新接入推荐优先看上面的 `PricingAgent` 示例。

```csharp
PricingApiClient client = new PricingApiClient("http://pricing-rule-center-host");
PricingHisIntegrationHelper helper = new PricingHisIntegrationHelper(client);

SpecialPricingDecision decision = helper.CheckSpecialPricingRequired(itemCode);
if (decision.ServiceUnavailable)
{
    MessageBox.Show(decision.Message);
    return;
}

if (decision.ShouldOpenPopup)
{
    PricingCalculateRequest request = PricingHisIntegrationHelper.BuildSingleItemRequest(
        patientId, visitId, chargeScene, chargeNo, businessRequestNo,
        currentUserId, currentUserName, itemCode, itemName, qty, unit, unitPrice, bodyPartCode);

    PricingPopupResult result = helper.ShowPricingPopup(this, request);
    if (!result.Confirmed)
    {
        return;
    }

    // HIS 按 result.Response.Items 回填最终数量、金额、折价金额和追溯号。
    // 如返回 ReplacementItem / ChildItems，HIS 必须把替换子项、加收子项一并落账。
    // result.Response.ExpireAt / ExpireSeconds 表示 confirm 占额有效期。

    // HIS 本地落账或支付失败时，才能 cancel 释放 confirm 占用。
    // 一旦 HIS 已经真实落账成功，commit 通知失败不能 cancel，应记录待补偿并重试 commit。
    if (!hisChargeSaved)
    {
        helper.CancelAfterHisFailure(result.RequestId);
        return;
    }

    // 这里的 savedDetails 必须来自 HIS 本地真实落账成功后的收费明细，
    // 不能直接用试算结果拼凑。ChargeDetailNo、ItemCode、PartSeq、FinalQty、FinalAmount
    // 会被计价中心与 confirm 阶段保存的结果逐项比对。
    List<PricingCommitActualItemRequest> actualItems = new List<PricingCommitActualItemRequest>();
    foreach (HisChargeDetail detail in savedDetails)
    {
        actualItems.Add(new PricingCommitActualItemRequest
        {
            ChargeDetailNo = detail.ChargeDetailNo,
            ItemCode = detail.ItemCode,
            PartSeq = detail.PartSeq,
            FinalQty = detail.Qty,
            FinalAmount = detail.Amount
        });
    }

    helper.CommitAfterHisSuccess(result.RequestId, hisChargeNo, actualItems, hisActualTotalAmount);
}
```

## 资金安全约束

- `confirm` 超时重试必须复用同一个 `BusinessRequestNo`。
- `confirm` 成功后，HIS 落账成功调用带 `actualItems` 参数的 `CommitAfterHisSuccess`。
- HIS 落账失败、支付失败或操作员取消调用 `CancelAfterHisFailure`。
- HIS 已经落账成功但 `commit` 通知失败时，不允许再调用 `cancel`；应重试 `commit` 或交给对账补偿。
- `special-flag` 或计价服务不可用时，不允许回退为普通计价。
- 一次收费动作可传多条 `PricingCalculateRequest.Items`，每条费用明细独立携带 `ItemCode`。
- `ReplacementItem` 和 `ChildItems` 是需要 HIS 一并落账和回传 commit 的真实收费结果，不是只用于展示的说明文本。
- 上线过渡期如需计入旧 HIS 历史收费次数，必须先实现 `QueryLegacyOccupiedQty` 中的 `RestrictingfeePay2` SQL；当前默认实现会抛错，避免误按 0 次历史收费放行。
