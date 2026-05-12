# HIS 真实源码接入落地说明

## 1. 本次落点

已把 `legacy-code/HIS` 中真实收费链路接入 PricingAgent：

| 场景 | 文件 | 接入点 |
| --- | --- | --- |
| 门诊划价/收费明细生成 | `SOC.Local.OutpatientFee.ZhuHai/Zdwy/IOutpatientItemInputAndDisplay/ucDisplay.cs` | 存在 `pricing-agent.config` 时跳过旧 `SetRestrictingfee` / `SetDiscountfee` 折价，并按当前收费界面所有已选明细调用统一计价 `simulate`，把预览最终金额、折价、替换/加收摘要显示到收费界面 |
| 门诊收费落账 | `HISFC.BizProcess/Integrate/Fee/Fee.cs` | `ClinicFee`、`ClinicZYFee`、`ClinicFeeSaveFee` 在生成处方号后调用 PricingAgent confirm，落账成功后登记待 commit |
| 住院医技/住院收费界面预览 | `HISFC.Components/Common/Controls/ucInpatientCharge.cs`、`ucInpatientCharge_new.cs` | 存在 `pricing-agent.config` 时按当前新增住院明细调用统一计价 `simulate`，旧版按当前患者试算，新版按住院患者分组试算，并把预览最终金额写回“合计金额”列 |
| 住院正向收费落账 | `HISFC.BizProcess/Integrate/Fee/Fee.cs` | `FeeManager` 在正交易写库前调用 PricingAgent confirm，配置启用时不再执行旧折价 |
| 事务提交/回滚 | `HISFC.BizProcess/Integrate/Fee/Fee.cs` | `Commit()` 在 HIS 事务提交后调用 commit；`Rollback()` 和医保提交失败时调用 cancel |
| 字段适配 | `HISFC.BizProcess/Integrate/Fee/Pricing/PricingLegacyChargeBridge.cs` | 统一构造请求、界面试算预览、回填 confirm 结果、生成替换/加收子项、组装 HIS 真实 `actualItems` |

入口级流程详见：

- `18-HIS门诊收费主窗口与费用明细控件流程梳理.md`
- `19-HIS住院终端确认入口流程梳理.md`
- `20-HIS住院医技批费与住院收费界面流程梳理.md`
- `21-HIS住院退费确认入口流程梳理.md`
- `22-自助机门诊待缴费用入口流程梳理.md`
- `23-微信门诊缴费入口流程梳理.md`

## 2. 启用方式

以 `pricing-agent.config` 是否存在作为旧 HIS 的运行开关：

- 不存在：维持旧折价逻辑，方便未部署 DLL/配置的旧环境继续运行。
- 存在：由 PricingAgent 接管计价判定，旧折价逻辑跳过；即使 Agent 判定本次为普通计价，也不再回落执行旧折价。如果配置错误或计价服务不可用，收费会被阻断，不回退普通计价。

配置文件与 `HIS.Pricing.Client.dll` 放在 HIS 运行目录，模板见 `his-client/pricing-agent.config.sample`。

## 3. 字段映射

统一计价根请求不再放单个 `itemCode`。一次缴费通过 `items[]` 表达多条费用明细，每条明细各自携带 `itemCode`；`commit.actualItems[]` 也必须携带 `itemCode`，用于和 confirm 阶段保存的主项目、替换子项、加收子项逐项对账。

| 统一计价字段 | 门诊来源 | 住院来源 |
| --- | --- | --- |
| `patientId` | 优先 `Register.PID.CardNO`，为空用 `Register.ID` | `PatientInfo.ID` |
| `visitId` | `Register.ID` | `PatientInfo.ID` |
| `chargeNo` / `businessRequestNo` | `invoiceCombNO` | 已生成的首个 `RecipeNO`，为空时用住院号兜底 |
| `chargeDetailNo` | `RecipeNO-SequenceNO` | `RecipeNO-SequenceNO` |
| `itemCode` / `itemName` | `FeeItemList.Item.ID/Name` | `FeeItemList.Item.ID/Name` |
| `inputQty` / `unitPrice` | `Item.Qty` / `Item.Price` | `Item.Qty` / `Item.Price` |
| `actualItems` | HIS 写库成功后的 `feeDetails` | HIS 写库成功后的 `feeItemLists` |

## 4. 资金边界

- `simulate` 只在门诊/住院收费界面展示阶段使用，不占用额度、不写待提交状态、不新增 HIS 真实子项。
- `confirm` 必须发生在 HIS 写库前。
- `commit` 只在 HIS 事务提交成功后执行，且回传真实 `actualItems`。
- HIS 写库失败、医保提交失败或事务回滚时调用 `cancel`。
- HIS 已经提交成功后，`commit` 通知失败不再调用 `cancel`，由 SDK 补偿队列落 JSON 等待重试。
- 替换子项和加收子项会从 HIS 非药品主数据读取项目；读不到项目时阻断落账，避免伪造明细。
- 旧系统 2 小时历史占用 SQL 尚未在仓库中收录，本次未实现 `QueryLegacyOccupiedQty`，不会编造 SQL。

## 5. 界面预览与最终落账

门诊收费界面进入、加载患者费用、手工录入项目、勾选/取消费用或修改项目数量/价格时，`ucDisplay.cs` 会用当前收费界面所有已选费用明细整体调用 `simulate`，不能只按单行试算：

- 金额列显示统一计价返回的预览最终金额。
- 备注列追加“统一计价预览”摘要，包含原价、最终金额、折价、数量变化、替换子项或加收子项摘要。
- 特殊计价影响的项目名称用蓝色标记。
- 预览阶段不写旧 HIS `RebateCost` 减免字段，避免后续 `ComputCost` / `GetFeeItemList` 再次套用旧折扣逻辑。
- simulate 失败只在备注提示，不在界面阶段落账或占额；真正收费保存前的 `confirm` 会再次调用服务，失败时阻断收费。

住院医技/住院收费界面也遵循同一原则：界面预览只更新当前行展示金额和备注提示；最终落账仍以 `Fee.cs` 保存前 `confirm` 返回结果为准。界面预览不会生成 HIS 真实替换/加收子项，避免用户多次修改数量时产生重复子项；真实子项只在 confirm 成功后、HIS 写库前由 `PricingLegacyChargeBridge` 追加。

## 6. Review 重点

1. 现场确认 `invoiceCombNO` 是否就是门诊一次收费动作的稳定业务号。
2. 现场确认住院正向收费的 `RecipeNO` 是否在 confirm 前已稳定生成。
3. 用真实规则验证替换子项、加收子项是否都能在 HIS 项目主数据中查到。
4. 用 HIS 真实事务走查：confirm 成功后写库失败、医保提交失败、commit 通知失败三类路径。
5. 用门诊界面走查：加载已有费用、手工录入项目、勾选/取消费用、修改数量/价格时，金额列和备注列是否能按整次收费实时展示统一计价预览。
