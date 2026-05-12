# HIS 住院退费确认入口流程梳理

## 0. 快速结论

住院退费确认入口是 `ucConfirmQuitFee`，核心链路是：

```text
ucConfirmQuitFee
  -> GetConfirmItem()
  -> GetRetrunApplyItem()
  -> feeIntegrate.QuitItem(patientInfo, ref alQuitFeeItemList)
  -> Fee.cs FeeManager(..., TransTypes.Negative)
```

当前新折价架构已经接入住院正向收费的 confirm/commit/cancel，但住院退费 reverse 尚未安全接入。原因是 reverse 请求必须携带原 confirm 阶段返回的 `OriginalRequestId`，而当前旧 HIS 住院费用明细没有可靠字段持久化这个值。不能用处方号或明细号猜测 RequestId，否则会释放错误占用。

## 1. 分析范围

| 文件 | 作用 |
| --- | --- |
| `legacy-code/HIS/HISFC.Components/InpatientFee/Fee/ucConfirmQuitFee.cs` | 住院退费确认界面。查询退费申请，合并退费明细，调用 `QuitItem`。 |
| `legacy-code/HIS/HISFC.Components/InpatientFee/Fee/ucQuitFee.cs` | 住院退费申请/退费相关界面之一。 |
| `legacy-code/HIS/HISFC.BizProcess/Integrate/Fee/Fee.cs` | `QuitItem(...)` 入口，最终进入负交易 `FeeManager(...)`。 |
| `his-client/PricingApiClient.cs` | 已定义 `PricingReverseRequest`，其中 `OriginalRequestId` 为必填业务字段。 |

## 2. 退费确认主流程

`ucConfirmQuitFee` 的退费确认保存逻辑可压缩为：

```text
GetConfirmItem()
  -> 从界面选中可退费用明细
GetRetrunApplyItem()
  -> 从退费申请表读取/合并申请
PublicTrans.BeginTransaction()
  -> 校验终端确认数量、执行科室、申请状态
  -> returnApplyManager.ConfirmApply(...)
  -> 汇总 alQuitFeeItemList
  -> feeIntegrate.QuitItem(patientInfo, ref alQuitFeeItemList)
  -> terminalIntegrate.UpdateTerminalDetailRecipe(...)
  -> feeIntegrate.Commit()
  -> PublicTrans.Commit()
```

`feeIntegrate.QuitItem(patient, ref feeItemLists)` 进入：

```text
Fee.cs QuitItem(...)
  -> FeeManager(patient, ref feeItemLists, ChargeTypes.Fee, TransTypes.Negative)
```

## 3. 与正向收费的关系

正向收费路径已经在 `Fee.cs` 中接入统一计价：

```text
TransTypes.Positive + ChargeTypes.Fee
  -> ConfirmInpatientBeforeSave(...)
  -> HIS 写库
  -> MarkHisSaveSucceeded(...)
  -> CommitSavedCharges()
```

退费是负交易：

```text
TransTypes.Negative
  -> 当前不调用 ConfirmInpatientBeforeSave(...)
  -> 当前不调用 ReverseAfterHisRefund(...)
```

这不是遗漏一行调用的问题。reverse 需要准确知道原收费对应的统一计价 RequestId，才能按原占用记录释放额度。

## 4. reverse 接入阻塞点

`PricingReverseRequest` 要求：

| 字段 | 说明 | 当前旧 HIS 状态 |
| --- | --- | --- |
| `OriginalRequestId` | 原 confirm 返回的请求 ID | 未持久化到住院费用明细，无法可靠回查。 |
| `ReverseNo` | HIS 退费业务号 | 可由退费申请号/退费单号生成。 |
| `ChargeDetailNo` | 原收费明细号 | 可由 `RecipeNO-SequenceNO` 生成。 |
| `ItemCode` | 项目编码 | 可从退费明细获取。 |
| `ReverseTime` | 退费业务时间 | 可从操作时间获取。 |
| `ReverseQty` / `ReverseAmt` | 退费数量/金额 | 可从退费明细获取。 |
| `ReversedBy` | 操作员 | 可从 HIS 操作员获取。 |

缺失的关键字段是 `OriginalRequestId`。如果不补字段或不建映射表，无法安全调用 reverse。

## 5. 推荐落地方案

住院退费 reverse 建议拆成二期小改造：

1. 正向收费 confirm 成功后，把 `RequestId` 与 HIS 真实明细绑定持久化。
2. 优先建一张本地映射表，例如：

```text
PRICING_HIS_CHARGE_MAP
  SOURCE_SYSTEM
  VISIT_TYPE
  CHARGE_NO
  CHARGE_DETAIL_NO
  ITEM_CODE
  PRICING_REQUEST_ID
  HIS_RECIPE_NO
  HIS_SEQUENCE_NO
  CREATED_AT
```

3. `MarkHisSaveSucceeded(...)` 或 HIS 明细写库成功后写入映射。
4. `QuitItem(...)` 负交易成功后，以 `RecipeNO-SequenceNO` 查映射，构造 `PricingReverseRequest`。
5. HIS 退费事务提交成功后调用 reverse；如果 reverse 失败，写 SDK 补偿队列，不能回滚已提交的 HIS 退费。

## 6. 当前结论

| 项 | 结论 |
| --- | --- |
| 住院正向收费 | 已接入新折价架构。 |
| 住院退费 HIS 业务 | 旧流程完整，当前仍按 HIS 负交易处理。 |
| 统一计价 reverse | 暂未接入，缺少原 `RequestId` 持久化。 |
| 是否可以临时按处方号释放额度 | 不可以，存在释放错单和资金风险。 |

因此，退费入口当前文档先完成流程梳理和阻塞点标注，不建议在没有 `OriginalRequestId` 映射前强行接 reverse。
