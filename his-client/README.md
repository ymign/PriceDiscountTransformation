# HIS 特殊计价前端接入说明

本目录提供批次十一的 HIS WinForms 前端代码，目标运行环境为 .NET Framework 3.5。

## 文件说明

- `PricingApiClient.cs`：基于 `HttpWebRequest` 的同步 HTTP 客户端，不使用 `HttpClient` 和 `async/await`。
- `PricingApiUrlBuilder.cs`：统一构造后端管理接口 URL，避免查询参数拼接错误。
- `PricingRuleDtos.cs`：规则工作台需要的请求/响应 DTO。
- `PricingHisIntegrationHelper.cs`：收费入口接入辅助，封装特殊项目判断、弹窗、commit/cancel。
- `FrmPricingRuleWorkbench.cs`：特殊计价规则维护工作台。
- `FrmPricingPopup.cs`：收费前特殊计价弹窗。

## HIS 菜单接入

在 HIS 菜单注册或模块加载点中创建工作台：

```csharp
PricingApiClient client = new PricingApiClient("http://pricing-rule-center-host");
FrmPricingRuleWorkbench frm = new FrmPricingRuleWorkbench(client, currentUserId);
frm.Show();
```

医院现有菜单注册方式尚未在本仓库确认，因此本目录只提供可加载窗体，不硬编码 HIS 菜单框架。

## 收费入口接入

收费录入界面在保存收费动作前接入：

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
    // HIS 落账成功后必须调用 CommitAfterHisSuccess。
    // HIS 落账失败或操作员取消必须调用 CancelAfterHisFailure。
}
```

## 资金安全约束

- `confirm` 超时重试必须复用同一个 `BusinessRequestNo`。
- `confirm` 成功后，HIS 落账成功调用 `CommitAfterHisSuccess`。
- HIS 落账失败、支付失败或操作员取消调用 `CancelAfterHisFailure`。
- `special-flag` 或计价服务不可用时，不允许回退为普通计价。
- 一次收费动作可传多条 `PricingCalculateRequest.Items`，每条费用明细独立携带 `ItemCode`。
