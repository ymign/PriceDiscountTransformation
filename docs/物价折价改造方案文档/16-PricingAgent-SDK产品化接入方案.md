# PricingAgent + SDK 产品化接入方案

## 1. 结论

可以按“合理用药”类似模式产品化。

HIS 不需要把特殊计价界面和规则判断全部写进自己系统，只需要在收费保存前调用统一计价 Agent。Agent 自己判断是否命中特殊项目，自己弹出特殊计价确认窗口，确认后把最终数量、金额、替换子项、加收子项返回给 HIS。HIS 落账成功后再调用 commit，落账失败才调用 cancel。

这种形态后续换医院、换 HIS、接自助机或微信时，复用的是同一套规则中心和同一套 SDK 协议，只需要做少量字段映射。

## 2. 产品边界

### 2.1 统一计价服务

服务端仍是统一规则中心，负责：

- 特殊项目标识查询。
- 试算、确认计价、commit、cancel、reverse。
- 规则匹配、公式、双单位换算、数量/金额限制、同组互斥、同手术封顶。
- 占额、冲正、追溯、审计。

### 2.2 PricingSdk

`PricingSdk` 是无界面的标准接入层，负责：

- `CheckSpecialPricingRequired`：查询项目是否需要特殊计价。
- `Simulate`：试算，不占用额度。
- `ConfirmBeforeCharge`：收费保存前确认，占用额度。
- `CommitAfterHisSuccess`：HIS 落账成功后提交。
- `CancelAfterHisFailure`：HIS 落账失败后取消。
- `ReverseAfterHisRefund`：HIS 退费或冲正后释放额度。
- `BuildChargeRequest`：把 HIS 轻量上下文转换成计价请求。

SDK 不依赖弹窗，适合自助机、微信、服务端网关、非 WinForms HIS。

### 2.3 PricingAgent

`PricingAgent` 是带 UI 的产品入口，负责：

- HIS 调用 `ConfirmBeforeCharge`。
- Agent 内部调用 special-flag。
- 非特殊项目直接返回 `OrdinaryPricing`。
- 特殊项目打开 `FrmPricingPopup`。
- 弹窗内试算和确认计价。
- 返回 `PricingAgentChargeResult` 给 HIS。
- 提供 `OpenRuleWorkbench`，让 HIS 菜单直接打开规则维护工作台。

Agent 适合传统 WinForms/WPF HIS，形态接近“合理用药弹窗”。

## 3. HIS 收费入口时序

```text
HIS 收费保存按钮
  |
  |-- 调用 PricingAgent.ConfirmBeforeCharge(...)
        |
        |-- special-flag
        |
        |-- 非特殊项目 -> 返回 OrdinaryPricing，HIS 走原普通收费
        |
        |-- 特殊项目 -> Agent 弹出自己的特殊计价窗口
              |
              |-- simulate 试算
              |-- 操作员确认
              |-- confirm 占用额度
              |-- 返回最终计价结果和 RequestId
  |
  |-- HIS 按返回结果写本地收费明细
        |
        |-- 成功 -> CommitAfterHisSuccess(RequestId, chargeNo, actualItems, totalAmount)
        |
        |-- 失败 -> CancelAfterHisFailure(RequestId)
```

关键边界：

- 操作员取消弹窗时，HIS 不落账。
- confirm 成功但 HIS 落账失败，必须 cancel。
- HIS 已经真实落账成功但 commit 通知失败，不能 cancel，必须重试 commit 或交给补偿任务。
- special-flag 或 confirm 服务不可用时，特殊项目不能回退普通计价。

## 4. 交付包建议

```text
PricingAgent/
  HIS.Pricing.Client.dll
  Newtonsoft.Json.dll
  PricingAgent.config
  README.md
  samples/
    WinFormsChargeSample.cs
    ReverseSample.cs
```

`PricingAgent.config` 建议包含：

| 配置 | 说明 |
| --- | --- |
| `BaseUrl` | 计价服务地址 |
| `SourceSystem` | 医院和渠道编码，例如 `HIS_WY` |
| `TimeoutMs` | HTTP 超时时间 |
| `MaxRetry` | confirm/commit 重试次数 |
| `RetryDelayMs` | 重试间隔 |
| `DefaultChargeScene` | 默认收费场景 |

当前仓库已提供代码级 `PricingSdkOptions`，后续如果要做安装包，可再加配置文件读取器和版本检查器。

## 5. 多系统复用方式

| 接入方 | 推荐方式 | 说明 |
| --- | --- | --- |
| WinForms HIS | `PricingAgent` | 自带弹窗，HIS 改动最少 |
| WPF HIS | `PricingAgent` 或 `PricingSdk` | 可先用 Agent，后续再换原生 WPF UI |
| 自助机 | `PricingSdk` | 自助机自己展示确认页，不需要 WinForms 弹窗 |
| 微信/互联网医院 | 后端直接调 HTTP 或 SDK 适配层 | 由小程序/H5 展示确认结果 |
| 其他医院 HIS | `PricingAgent` + 字段适配 | 只改患者、就诊、收费明细字段映射 |

如果某家医院 HIS 不能直接加载 .NET DLL，可以在 `PricingSdk` 外层再包装成本地 EXE、COM 或 HTTP 本地代理，但核心协议和资金安全流程不变。

## 6. HIS 最小改造点

每个医院只需要确认以下字段来源：

- 患者：`patientId`、`visitId`、`encounterNo`、`visitType`、`patientAge`。
- 收费动作：`chargeNo`、`businessRequestNo`、`businessChargeTime`、`chargeScene`。
- 操作员：`operatorId`、`operatorName`、`chargeDeptCode`。
- 明细：`itemCode`、`itemName`、`inputQty`、`unit`、`unitPrice`、`bodyPartCode`。
- 复杂项目：`pricingParts`、`operationNo`、`pregnancyNo` 等扩展参数。
- 落账回传：`chargeDetailNo`、`itemCode`、`partSeq`、`finalQty`、`finalAmount`。
- 退费回传：`originalRequestId`、`reverseNo`、`chargeDetailNo`、`reverseQty`、`reverseAmt`。

## 7. 产品化风险点

1. `businessRequestNo` 必须稳定。confirm 超时重试不能重新生成业务号。
2. `actualItems` 必须来自 HIS 真实落账明细，不能用试算结果拼凑。
3. 替换子项和加收子项必须在 HIS 中真实落账，并在 commit 回传。
4. 规则服务不可用时，特殊项目不能静默走原价。
5. 上线过渡期如要计入旧 HIS 历史收费次数，必须实现旧系统窗口数量查询。
6. 不同医院的项目编码、科室编码、部位编码、收费场景需要做字典映射。
7. 多肿物、多部位、多面积项目必须传明细，不能压成一个总数量。

## 8. 当前代码落点

- `his-client/PricingSdkOptions.cs`
- `his-client/PricingSdk.cs`
- `his-client/PricingChargeContext.cs`
- `his-client/PricingAgent.cs`
- `his-client/FrmPricingPopup.cs`
- `his-client/FrmPricingRuleWorkbench.cs`

这套边界已经能支持“一个 DLL 接入、自己弹窗、结果回传”的产品形态。后续真正做商品化交付时，再补安装包、配置读取、日志落盘、自动更新和授权控制。
