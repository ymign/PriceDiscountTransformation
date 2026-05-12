# HIS 住院终端确认入口流程梳理

## 0. 快速结论

住院终端确认的界面入口是 `ucInpatientConfirm.Save()`，但旧折价和新统一计价都不应直接塞在这个窗口方法里。它真正的费用落账边界在：

```text
ucInpatientConfirm.Save()
  -> feeManager.FeeItem(myPatient, ref alOrder/alFeeItemList)
  -> HISFC.BizProcess.Integrate.Fee.FeeManager(...)
  -> PricingLegacyChargeBridge.ConfirmInpatientBeforeSave(...)
```

当前源码已经在 `Fee.cs` 的住院正向收费路径接入 PricingAgent。部署 `pricing-agent.config` 后，住院终端正向收费会在 HIS 写库前执行统一计价 `confirm`；HIS 写库成功后登记待提交；`feeManager.Commit()` 成功提交 HIS 事务后执行统一计价 `commit`；异常或回滚时执行 `cancel`。

## 1. 分析范围

| 文件 | 作用 |
| --- | --- |
| `legacy-code/HIS/HISFC.Components/Terminal/Confirm/ucInpatientConfirm.cs` | 住院终端确认界面。负责选择待确认医嘱、组装住院费用、更新终端确认明细。 |
| `legacy-code/HIS/HISFC.BizProcess/Integrate/Fee/Fee.cs` | 住院最终收费落账入口。负责旧折价、统一计价 confirm/commit/cancel、HIS 费用写库、库存和医保事务。 |
| `legacy-code/HIS/HISFC.BizProcess/Integrate/Fee/Pricing/PricingLegacyChargeBridge.cs` | 旧 HIS 到 PricingAgent 的适配层。构造住院计价请求、回填统一计价结果、登记待 commit 状态。 |

## 2. 终端确认主流程

`ucInpatientConfirm.Save()` 的核心链路：

```text
ValidState()
  -> CheckWJJK(myPatient.ID)
  -> PublicTrans.BeginTransaction()
  -> GetFeeOrder()
  -> GetNewFeeItemList()
  -> ChangeOrderToFeeItemList()
  -> 住院组套展开、DR/CT 去重
  -> 弹出本次确认收费金额汇总
  -> feeManager.FeeItem(myPatient, ref alOrder)
  -> feeManager.FeeItem(myPatient, ref alFeeItemList)
  -> 写终端确认明细
  -> 更新医嘱执行/收费状态
  -> 发送 HL7 消息
  -> feeManager.Commit()
```

关键点：

- `Save()` 自己不直接调用 `SetRestrictingfee`、`SetDiscountfee`、`ConvertRestrictingfeeZY`。
- 终端确认会把医嘱或新增费用转换为 `Inpatient.FeeItemList` 后交给 `feeManager.FeeItem()`。
- `feeManager.FeeItem()` 进入 `Fee.cs` 的共享住院收费边界，因此终端确认和其他住院正向收费共用同一套新计价接入。

## 3. 旧折价位置

旧逻辑不在终端界面，而在 `Fee.cs` 的住院正向 `FeeManager(...)` 中：

```text
FeeItem(patient, ref feeItemLists)
  -> FeeManager(patient, ref feeItemLists, ChargeTypes.Fee, TransTypes.Positive)
       SetRestrictingfee(itemCode, ref LimitNumber)
       SetDiscountfee(itemCode, ref DISCOUNT_RATE, ref TOPPRICE)
       ConvertRestrictingfeeZY(...)
       ConvertDiscountfeeZY(...)
```

部署 PricingAgent 后，`FeeManager(...)` 会先调用 `ConfirmInpatientBeforeSave(...)`。如果统一计价已配置并返回可落账结果，则跳过旧折价循环，避免旧折价和新规则重复作用。

## 4. 新折价架构接入状态

当前住院终端正向收费已由共享落账层覆盖：

| 阶段 | 当前代码落点 | 说明 |
| --- | --- | --- |
| 启用开关 | `PricingLegacyChargeBridge.IsConfigured()` | HIS 运行目录存在 `pricing-agent.config` 才启用；未部署时保持旧逻辑。 |
| confirm | `Fee.cs` 住院 `FeeManager(...)` | 仅 `TransTypes.Positive` + `ChargeTypes.Fee` 触发，写 HIS 前占额并回填金额/子项。 |
| 旧折价跳过 | `Fee.cs` 旧折价循环外层条件 | 已配置 PricingAgent 后，由 Agent 判断特殊/普通计价；即使返回普通计价，也不再执行旧 `ConvertRestrictingfeeZY/ConvertDiscountfeeZY`。 |
| HIS 写库成功 | `PricingChargeBridge.MarkHisSaveSucceeded(...)` | 从真实 HIS 明细重建 `actualItems`，等待事务提交。 |
| commit | `Fee.Commit()` | HIS 事务提交成功后调用统一计价 commit。 |
| cancel | `Fee.Rollback()`、医保提交失败分支 | HIS 失败或回滚时释放 confirm 占用。 |

## 5. 需要注意的边界

1. 终端界面本身没有做 `simulate` 预览，最终金额以 `Fee.cs` 写库前 confirm 回填为准。
2. 终端确认中有较多 `feeManager.Rollback()` 分支；这些分支会进入 `Fee.Rollback()` 并调用统一计价 `cancel`。
3. 当前住院正向收费使用 `GetPricingChargeNo(patient.ID, feeItemLists)` 生成业务号，优先取明细 `RecipeNO`，取不到时回落住院流水号。上线前需确认住院处方号在 confirm 前是否稳定。
4. 退费不是本入口的正向收费流程。住院退费需要 reverse，见 `21-HIS住院退费确认入口流程梳理.md`。

## 6. 测试建议

| 用例 | 预期 |
| --- | --- |
| 未部署 `pricing-agent.config` | 终端确认保持旧 `ConvertRestrictingfeeZY/ConvertDiscountfeeZY` 行为。 |
| 部署配置且普通项目 | Agent 返回普通计价时不阻断，且不再回落执行旧折价。 |
| 部署配置且特殊项目 | confirm 成功后 HIS 写库，HIS commit 后统一计价 commit。 |
| confirm 成功但后续终端明细写库失败 | `feeManager.Rollback()` 后统一计价 cancel。 |
| HIS commit 成功但统一计价 commit 失败 | SDK 写补偿队列，不能再 cancel。 |
