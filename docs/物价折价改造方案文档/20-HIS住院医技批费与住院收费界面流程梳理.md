# HIS 住院医技批费与住院收费界面流程梳理

## 0. 快速结论

住院医技/住院收费界面有两套控件：

```text
HISFC.Components.InpatientFee.Fee.ucCharge
  -> HISFC.Components.Common.Controls.ucInpatientCharge

HISFC.Components.InpatientFee.Fee.ucCharge_new
  -> HISFC.Components.Common.Controls.ucInpatientCharge_new
```

界面侧会在录入项目、修改数量/价格时计算展示金额；最终正向收费仍通过 `feeIntergrate.FeeItem(...)` 进入 `Fee.cs`。因此新折价架构的资金边界已经由 `Fee.cs` 统一覆盖，住院医技批费不需要在窗口层单独实现 confirm/commit/cancel。

## 1. 分析范围

| 文件 | 作用 |
| --- | --- |
| `legacy-code/HIS/HISFC.Components/InpatientFee/Fee/ucCharge.cs` | 旧版住院收费容器，承载 `ucInpatientCharge`。 |
| `legacy-code/HIS/HISFC.Components/InpatientFee/Fee/ucCharge_new.cs` | 新版住院收费容器，承载 `ucInpatientCharge_new`，支持更多多患者/科室场景。 |
| `legacy-code/HIS/HISFC.Components/Common/Controls/ucInpatientCharge.cs` | 旧版住院费用明细录入控件。 |
| `legacy-code/HIS/HISFC.Components/Common/Controls/ucInpatientCharge_new.cs` | 新版住院费用明细录入控件。 |
| `legacy-code/HIS/HISFC.BizProcess/Integrate/Fee/Fee.cs` | 住院最终收费落账和统一计价接入边界。 |

## 2. 容器到明细控件的调用关系

旧版 `ucCharge.Save()`：

```text
ucCharge.Save()
  -> ucInpatientCharge.PatientInfo = patient
  -> ucInpatientCharge.Save()
  -> ucInpatientCharge.FeeItemCollection
  -> Print(...)
```

新版 `ucCharge_new.Save()`：

```text
ucCharge_new.Save()
  -> ucInpatientCharge_new1.PatientInfo = patient
  -> ucInpatientCharge_new1.Save()
  -> ucInpatientCharge_new1.FeeItemCollection
  -> Print(...)
```

容器层只负责患者、医生、执行科室、打印和界面清理；费用明细生成、保存类型判断和最终落账在 `ucInpatientCharge` / `ucInpatientCharge_new` 内部。

## 3. 明细控件保存流程

`ucInpatientCharge.Save()` 和 `ucInpatientCharge_new.Save()` 都按 `feeType` 分流：

```text
Save()
  -> FeeTypes.划价/记账 -> Charge()
  -> FeeTypes.收费     -> Fee()
```

### 3.1 Charge 路径

`Charge()` 主要用于生成或更新待收费明细：

```text
PublicTrans.BeginTransaction()
  -> 遍历 FarPoint 行
  -> SetItem(..., PayTypes.Charged, ...)
  -> 新项目 InsertMedItemList / InsertFeeItemList
  -> 旧项目 UpdateChargeInfo
  -> PublicTrans.Commit()
```

这一路径不是最终资金边界，不调用统一计价 confirm。它仍只是 HIS 本地待收费明细维护。

### 3.2 Fee 路径

`Fee()` 是正向收费路径：

```text
校验患者和费用
  -> PublicTrans.BeginTransaction()
  -> 遍历 FarPoint 行
  -> SetItem(..., PayTypes.Balanced, ...)
  -> 收集 firstInputFeeItemlist
  -> 组套拆分 SplitUndrugCombItem
  -> 儿童加收/互斥提示/库存等校验
  -> feeIntergrate.FeeItem(patientInfo, ref firstInputFeeItemlist)
  -> 药品出库/适应症保存/暂存删除/HL7
  -> feeIntergrate.Commit()
```

新版 `ucInpatientCharge_new.Fee()` 会按 `ExtFlag2` 分组，可能一次界面操作内对多个住院患者分别调用 `feeIntergrate.FeeItem(...)`，但每组最终仍进入同一个 `Fee.cs` 住院正向收费边界。

## 4. 界面侧旧折价展示

两个明细控件在界面阶段都能看到比例折价调用：

```text
SetItemProperty()
  -> itemManager.SetDiscountfee(...)
  -> 按 discountRate/topprice 更新 TotCost

SetItem(...)
  -> itemManager.SetDiscountfee(...)
  -> 按 discountRate/topprice 写 FeeItemList.FT.TotCost
```

这部分属于界面显示和本地明细初始金额计算。数量限制折价不在这两个界面控件中完成，旧系统数量限制仍在 `Fee.cs` 最终落账层执行。

部署 PricingAgent 后，住院医技/住院收费界面已增加实时 `simulate` 预览：`ucInpatientCharge` / `ucInpatientCharge_new` 会在新增项目、修改数量/价格/付数、切换药品单位、删除行后，按当前新增收费明细调用统一计价试算，把预览最终金额写回“合计金额”列，并用蓝色文字和 FarPoint 单元格备注标识统一计价摘要。最终落账仍以 `Fee.cs` 写库前 `ConfirmInpatientBeforeSave(...)` 回填为准。

## 5. 新折价架构接入状态

住院医技正向收费已通过共享落账层接入：

| 阶段 | 当前代码落点 | 说明 |
| --- | --- | --- |
| simulate | `ucInpatientCharge.RefreshPricingPreviewForCurrentRows()` / `ucInpatientCharge_new.RefreshPricingPreviewForCurrentRows()` | 界面录入阶段按当前新增明细试算；新版控件按住院患者分组试算。 |
| 明细收集 | `ucInpatientCharge.Fee()` / `ucInpatientCharge_new.Fee()` | 生成 `firstInputFeeItemlist`。 |
| 资金边界 | `feeIntergrate.FeeItem(patientInfo, ref firstInputFeeItemlist)` | 进入 `Fee.cs`。 |
| confirm | `Fee.cs` 住院 `FeeManager(...)` | 写 HIS 前调用 `ConfirmInpatientBeforeSave(...)`。 |
| 旧折价跳过 | `Fee.cs` 住院旧折价循环条件 | PricingAgent 已接管后，由 Agent 判断特殊/普通计价；已配置时跳过旧数量/比例折价。 |
| commit/cancel | `feeIntergrate.Commit()` / `feeIntergrate.Rollback()` | 通过 `Fee.Commit()` / `Fee.Rollback()` 通知统一计价。 |

## 6. 与门诊入口的差异

| 点位 | 门诊收费 | 住院医技/住院收费 |
| --- | --- | --- |
| 界面控件 | 珠海本地 `ucDisplay` | `ucInpatientCharge` / `ucInpatientCharge_new` |
| 界面 simulate | 按当前已选明细整体试算 | 已接入；旧版按当前患者新增明细试算，新版按住院患者分组试算 |
| 资金边界 | `Fee.cs ClinicFee(...)` | `Fee.cs FeeItem(...)` |
| 场景编码 | `OUTPATIENT` | `INPATIENT` |
| 旧数量折价 | 门诊窗口和特殊规则 DLL 多处存在 | 主要在 `Fee.cs` 住院最终落账层 |

## 7. 测试建议

| 用例 | 预期 |
| --- | --- |
| `pricing-agent.config` 不存在 | 住院医技继续执行旧比例展示和旧最终折价。 |
| 配置存在，单患者正向收费 | `feeIntergrate.FeeItem` 前 confirm，HIS commit 后统一计价 commit。 |
| 配置存在，多患者批量收费 | 每个患者分组分别 confirm；任一失败时 HIS rollback 并 cancel 未提交请求。 |
| confirm 返回替换/加收子项 | `PricingLegacyChargeBridge` 从 HIS 项目主数据补全子项并追加到住院明细。 |
| confirm 后药品出库或 HL7 失败 | `feeIntergrate.Rollback()` 后统一计价 cancel。 |

## 8. 后续可选增强

住院医技界面实时预览已经接入，但该增强不是资金安全前置条件；资金边界仍以 `Fee.cs` confirm 为准。现场走查时需要同时验证旧版 `ucInpatientCharge` 单患者录入、新版 `ucInpatientCharge_new` 多患者分组录入，以及最终保存前 PricingAgent confirm 弹窗金额是否与界面预览一致。
