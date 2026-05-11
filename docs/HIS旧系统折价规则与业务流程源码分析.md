# HIS 旧系统折价规则与业务流程源码分析

> 基线资料：`docs/五院当前折价逻辑代码位置-校正版.md`。  
> 本文在该文档基础上，按 `legacy-code/` 当前源码重新串联 HIS、住院、 自助机、微信和新 `his-client` 接入样例的调用流程。  
> 分析日期：2026-05-11。

---

## 1. 总体结论

旧 HIS 的折价能力不是一个独立规则引擎，而是分散在窗口、业务层和 `ZDWY.SpecialRule.Price` 程序集中的硬编码流程。核心规则只有两大类：

1. 数量限制折价：项目命中 `Restrictingfee` 常数后，查询患者历史收费次数，剩余额度不足时将超出部分金额置 0。
2. 比例折价加最高限价：项目命中 `FIN_DISCOUNT_FEE` 且 `discount_type = "2"` 后，按“第 1 个全价，第 2 个及以后按比例”计算，再受 `topprice` 最高限价约束。

当前源码显示，人工 HIS 门诊和住院最终收费会执行折价；自助机调用了 `GetFeeItemListnew`，但该分支的数量折价代码已注释，实际不执行限制收费；微信端只做组套展开和普通发票生成，未看到调用 `ZDWY.SpecialRule`、`SetRestrictingfee` 或 `SetDiscountfee`。

---

## 2. 核心代码地图

| 模块 | 文件 | 关键方法 | 作用 |
|------|------|----------|------|
| 特殊规则 DLL | `legacy-code/HIS/ZDWY.SpecialRule/ZDWY.SpecialRule.Price/CTMRFeeRule.cs` | `GetFeeItemList`，`GetFeeItemListnew` | 门诊/自助机价格明细处理入口 |
| 特殊规则 DLL | `legacy-code/HIS/ZDWY.SpecialRule/ZDWY.SpecialRule.Price/Restrictingfee.cs` | `ConvertRestrictingfee` | 门诊明细数量限制折价 |
| 特殊规则 DLL | 同上 | `ConvertRestrictingfeeCharge` | 门诊组套主项拆明细后限制折价，再汇总回主项 |
| 特殊规则 DLL | 同上 | `ConvertRestrictingfeeZY` | 住院明细数量限制折价 |
| 特殊规则 DLL | 同上 | `ConvertDiscountfee`，`ConvertDiscountfeeZY` | 门诊/住院比例折价和最高限价 |
| 特殊规则 DLL DB | `legacy-code/HIS/ZDWY.SpecialRule/ZDWY.SpecialRule.Price/DB/CTMRFeeRuleDB.cs` | `SetRestrictingfee`，`SetDiscountfee`，`getRestrictingfee`，`getRestrictingfeeZT` | 读取规则配置和历史收费次数 |
| HIS 费用业务 | `legacy-code/HIS/HISFC.BizLogic/Fee/Item.cs` | `SetRestrictingfee`，`SetDiscountfee` | HIS 业务层同名规则查询包装 |
| 门诊窗口 | `legacy-code/HIS/SOC.Local.OutpatientFee.ZhuHai/Zdwy/IOutpatientItemInputAndDisplay/ucDisplay.cs` | `GetFeeItemList`，`GetFeeItemListForCharge`，`SetChargeInfo` 附近逻辑 | 人工门诊收费、划价、界面金额回填 |
| 住院收费业务 | `legacy-code/HIS/HISFC.BizProcess/Integrate/Fee/Fee.cs` | `FeeItem`，`FeeManager` | 住院最终收费落账前统一套用折价 |
| 住院终端 | `legacy-code/HIS/HISFC.Components/Terminal/Confirm/ucInpatientConfirm.cs` | `Save` | 终端确认保存，调用 `feeManager.FeeItem` 落账 |
| 住院医技界面 | `legacy-code/HIS/HISFC.Components/Common/Controls/ucInpatientCharge.cs`，`ucInpatientCharge_new.cs` | `SetDiscountfee` 调用点 | 界面侧计算比例折价展示金额 |
| 自助机 | `legacy-code/zzsb/His.Service/His.Business/ZZSB/Patientopb.cs` | `GetGetPrescriptionAndChargeDetailsForSRM` | 获取待缴费用后调用 `CTMRFeeRule.GetFeeItemListnew` |
| 微信 | `legacy-code/WeChat/FS.ZDWY.Internet.BP/OutPatient/RegisterInfoManager.cs`，`OrderManager.cs`，`NeusoftBussiness.cs` | `GetFeeItemList`，`MakeInvoice` | 组套展开和发票生成，未见折价调用 |

---

## 3. HIS 门诊调用流程

### 3.1 `ZDWY.SpecialRule.Price.CTMRFeeRule.GetFeeItemList`

源码证据：

- 入口方法：`CTMRFeeRule.cs:53`
- 数量规则查询：`CTMRFeeRule.cs:173`
- 比例规则查询：`CTMRFeeRule.cs:174`
- 数量折价执行：`CTMRFeeRule.cs:177`
- 比例折价执行：`CTMRFeeRule.cs:181`

流程：

```text
GetFeeItemList(clincCode, feeArryList)
  1. 根据 clincCode 查询挂号/患者信息
  2. 补全医生开立科室
  3. 对复合非药品组套做展开
     - DR/CT 组套按 ItemZT 常数做去重和特殊展开
     - 普通组套按组套明细展开
  4. 反向遍历明细，去掉重复 DR/CT 项目
  5. 对每条费用明细：
     - SetRestrictingfee(itemCode) 判断是否数量限制项目，读取 LimitNumber
     - SetDiscountfee(itemCode) 判断是否比例折价项目，读取 discountRate/topprice/discountType
     - 若命中数量限制且收费科室不是 7021，执行 ConvertRestrictingfee
     - 若 discountType == "2"，执行 ConvertDiscountfee
  6. 删除原列表中被替换的明细，追加折价后的明细
```

关键业务点：

- 数量限制在比例折价之前执行。
- 数量限制有科室排除：`rInfo.DoctorInfo.Templet.Dept.ID != "7021"`。源码只确认 7021 排除，未在当前仓库源码中确认“体检科室/多发伤”排除。
- 超限不是拒绝收费，而是把超出部分置为 0 元或截断数量后保留可收费部分。

### 3.2 门诊窗口 `ucDisplay`

源码证据：

- `GetFeeItemList` 入口：`ucDisplay.cs:7595`
- 明细数量规则查询：`ucDisplay.cs:7725`
- 明细比例规则查询：`ucDisplay.cs:7726`
- 明细数量折价：`ucDisplay.cs:7729`
- 明细比例折价：`ucDisplay.cs:7732`
- 组套显示/划价时 `ConvertRestrictingfeeCharge` 调用：`ucDisplay.cs:2139`，`ucDisplay.cs:4061`

人工门诊窗口并不只是调用特殊规则 DLL，也在本地窗口流程中直接调用 `undrugManager.SetRestrictingfee`、`undrugManager.SetDiscountfee` 和 `setRestrictingfee`。因此迁移时不能只看 `CTMRFeeRule.cs`，还要看 `ucDisplay.cs` 的收费保存、界面回填和组套汇总路径。

---

## 4. 住院调用流程

### 4.1 终端确认保存

源码证据：

- `ucInpatientConfirm.Save`：`ucInpatientConfirm.cs:1772`
- 组装费用后调用落账：`ucInpatientConfirm.cs:2008`，`ucInpatientConfirm.cs:2139`
- 事务提交：`ucInpatientConfirm.cs:2453`

流程：

```text
ucInpatientConfirm.Save()
  1. ValidState / 外部检查
  2. 开启 HIS 本地事务
  3. GetFeeOrder / GetNewFeeItemList / ChangeOrderToFeeItemList
  4. 对住院组套医嘱做展开、DR/CT 去重
  5. 调用 feeManager.FeeItem(myPatient, ref alOrder 或 alFeeItemList)
  6. 更新可退数量、扩展标记、发送药品信息
  7. feeManager.Commit()
```

终端确认的 `Save` 本身未直接调用 `ConvertRestrictingfeeZY`。折价发生在下一层 `HISFC.BizProcess.Integrate.Fee.FeeItem -> FeeManager`。

### 4.2 住院最终落账 `Fee.cs`

源码证据：

- `FeeItem(patient, ref ArrayList)`：`Fee.cs:2757`
- 正向收费折价循环：`Fee.cs:1426` 到 `Fee.cs:1434`

流程：

```text
FeeItem(patient, ref feeItemLists)
  -> FeeManager(patient, ref feeItemLists, ChargeTypes.Fee, TransTypes.Positive)
       对每条住院 FeeItemList：
         SetRestrictingfee(itemCode, ref LimitNumber)
         SetDiscountfee(itemCode, ref DISCOUNT_RATE, ref TOPPRICE)
         returnRows > 0       -> ConvertRestrictingfeeZY(patient.ID, ...)
         discount_type == "2" -> ConvertDiscountfeeZY(...)
       删除原明细并追加折价后明细
       继续后续落账、库存、费用明细写入
```

结论：

- 住院最终落账支持数量限制折价和比例折价。
- 住院医技界面 `ucInpatientCharge.cs`、`ucInpatientCharge_new.cs` 当前能确认的是界面金额展示侧调用 `SetDiscountfee` 计算比例折价，未在这两个界面文件中检索到 `SetRestrictingfee`。数量限制仍会在最终 `FeeManager` 正向收费时执行。

---

## 5. 自助机调用流程

源码证据：

- 自助机入口：`legacy-code/zzsb/His.Service/His.Business/ZZSB/Patientopb.cs:6646`
- 查询待缴明细：`Patientopb.cs:6662`
- 创建特殊规则对象：`Patientopb.cs:6681`
- 调用自助机分支：`Patientopb.cs:6684`
- 自助机分支入口：`CTMRFeeRule.cs:371`
- 数量限制代码被注释：`CTMRFeeRule.cs:478` 到 `CTMRFeeRule.cs:481`

流程：

```text
Patientopb.GetGetPrescriptionAndChargeDetailsForSRM(xml)
  -> GetPrescriptionAndChargeDetailsForSRMModel
  -> GetPrescriptionAndChargeDetailsForSRMData
  -> new ZDWY.SpecialRule.Price.CTMRFeeRule()
  -> CTMRFeeRule.GetFeeItemListnew(patientId, feeItems)
       1. 查询患者和合同单位
       2. 组套展开
       3. 自助机把组套明细金额汇总回主项
       4. 数量限制折价代码整段注释
       5. 未看到比例折价执行
  -> GetPrescriptionAndChargeDetailsForSRMXML(actualAll)
```

结论：

- 自助机虽然引用同一套 `ZDWY.SpecialRule.Price.CTMRFeeRule`，但走的是 `GetFeeItemListnew`。
- 该分支当前不执行数量限制折价，且当前源码中也未见 `SetDiscountfee/ConvertDiscountfee`。
- 迁移到统一规则中心后，如果自助机也调用新计价服务，会改变旧行为，需要业务确认。

---

## 6. 微信调用流程

源码证据：

- 微信挂号/缴费获取费用：`RegisterInfoManager.cs:4656`
- 微信订单获取费用：`OrderManager.cs:3282`
- 普通发票生成：`NeusoftBussiness.cs:484`，`NeusoftBussiness.cs:620`
- 当前检索未发现 `SetRestrictingfee`、`SetDiscountfee`、`ZDWY.SpecialRule` 调用。

流程：

```text
RegisterInfoManager / OrderManager
  -> GetFeeItemList(...)
       1. 非药品复合项目按组套展开
       2. DR/CT 项目做部分去重
       3. 返回普通 FeeItemList
  -> NeusoftBussiness.MakeInvoice(...)
       1. 发票拆分
       2. 生成发票主表和发票明细
       3. 返回普通收费结果
```

结论：

- 微信端当前源码没有执行特殊折价规则。
- 如果历史业务口径认为微信端有折价，需要补充 `OutpatientFeeSpecialRule` 的真实源码或部署 DLL 反查，否则以当前仓库源码为准：微信端不折价。

---

## 7. 数量限制折价规则

### 7.1 规则配置来源

源码证据：

- `CTMRFeeRuleDB.SetRestrictingfee`：`CTMRFeeRuleDB.cs:1527`
- SQL 索引：`Fee.PactUnitItemRate.Restrictingfee`，见 `CTMRFeeRuleDB.cs:1531`
- HIS 业务层同名包装：`HISFC.BizLogic/Fee/Item.cs:571`
- 常数查询清单：`legacy-code/database/ddl/相关字典.txt:1`

当前源码只能确认通过 SQL 索引读取，完整 SQL 配置没有收录到仓库。已有文档口径认为 `Restrictingfee.input_code` 是最大允许收费数量。

### 7.2 历史收费次数来源

源码证据：

- `getRestrictingfee`：`CTMRFeeRuleDB.cs:1599`
- SQL 索引：`Fee.PactUnitItemRate.RestrictingfeePay2`，见 `CTMRFeeRuleDB.cs:1603`
- `getRestrictingfeeZT`：`CTMRFeeRuleDB.cs:1621`
- SQL 索引：`Fee.PactUnitItemRate.RestrictingfeePayZT`，见 `CTMRFeeRuleDB.cs:1625`

注意：

- 当前仓库没有 `RestrictingfeePay2` 和 `RestrictingfeePayZT` 对应的完整 SQL。
- 因此“2 小时窗口”“是否排除已退费”“门诊住院累计表范围”等精确口径，仍需从 HIS SQL 配置补齐。
- 从代码结构看，旧系统不是查一张折价占额表，而是通过 SQL 动态查历史收费记录。

### 7.3 明细数量折价分支

源码证据：

- 门诊：`Restrictingfee.cs:158`
- 住院：`Restrictingfee.cs:608`
- 分组互斥和特殊常数读取：`Restrictingfee.cs:169` 到 `Restrictingfee.cs:209`，`Restrictingfee.cs:619` 到 `Restrictingfee.cs:659`

核心算法：

```text
feetype = 已收费次数
Limitsum = LimitNumber - feetype - 本次收费中已占用数量

if Limitsum <= 0:
  本条金额 = 0
  Memo = "P" + 原数量
  原列表删除，折价后明细追加回列表

else if Limitsum - 当前数量 <= 0:
  本条数量截断为 Limitsum
  本条金额 = 单价 * Limitsum
  Memo = "N" + 原数量
  原列表删除，折价后明细追加回列表

else:
  本条正常收费
```

关联常数：

| 常数类型 | 代码位置 | 业务含义 |
|----------|----------|----------|
| `Astrictpackagefee` | `Restrictingfee.cs:169`，`Restrictingfee.cs:619` | 组套内豁免，不参与历史次数扣减 |
| `RestrictingfeeZT` | `Restrictingfee.cs:170`，`Restrictingfee.cs:620` | 分组互斥，按组号查历史次数 |
| `RestrictingfeeCP` | `Restrictingfee.cs:208`，`Restrictingfee.cs:658` | 床旁项目互斥/限制 |
| `RestrictingfeeTX1` | `Restrictingfee.cs:209`，`Restrictingfee.cs:659` | 胎心类项目互斥/限制 |

### 7.4 组套数量折价

源码证据：

- `ConvertRestrictingfeeCharge`：`Restrictingfee.cs:296`
- 子项历史次数查询：`Restrictingfee.cs:357`
- 子项分组次数查询：`Restrictingfee.cs:360`
- 子项数量规则查询：`Restrictingfee.cs:362`

流程：

```text
ConvertRestrictingfeeCharge(CARD_NO, 组套主项 f, ...)
  1. 判断 f 是否复合项目
  2. 展开组套明细
  3. 对每个子项：
     - 取子项价格
     - 查子项历史收费次数
     - 查子项 Restrictingfee 上限
     - 按数量限制计算该子项可收费金额
  4. sumPricecot 累计所有子项折后金额
  5. f.FT.TotCost = sumPricecot
  6. f.FT.OwnCost = sumPricecot
```

迁移含义：

- 旧系统组套主项可在客户端内部拆细后汇总回主项。
- 新规则中心如果接收组套主项，必须有等价的“展开子项并逐项计价”逻辑；更推荐 HIS 调用前展开成明细传入规则中心。

---

## 8. 比例折价和最高限价

### 8.1 规则配置来源

源码证据：

- `CTMRFeeRuleDB.SetDiscountfee`：`CTMRFeeRuleDB.cs:1565`
- SQL 索引：`Fee.PactUnitItemRate.Discountfee`，见 `CTMRFeeRuleDB.cs:1569`
- HIS 业务层同名包装：`HISFC.BizLogic/Fee/Item.cs:625`
- 表结构：`legacy-code/database/ddl/FIN_DISCOUNT_FEE.sql:2`

`FIN_DISCOUNT_FEE` 字段：

| 字段 | 含义 |
|------|------|
| `item_code` | 项目编码 |
| `item_name` | 项目名称 |
| `discount_rate` | 折价比例 |
| `topprice` | 最高价 |
| `discount_type` | 折价类型，代码中 `"2"` 表示执行折价 |
| `valid_state` | 有效状态 |

当前表结构没有最低限价字段。旧文档中提到“最低限价”的地方，应按当前源码校正为最高限价或新系统扩展能力，不能从旧表迁出最低限价。

### 8.2 公式

源码证据：

- 门诊：`Restrictingfee.cs:744`
- 住院：`Restrictingfee.cs:767`

公式：

```text
amount = (unitPrice * discountRate) * (qty - 1) + unitPrice
       = unitPrice * (qty * discountRate + 1 - discountRate)
```

含义：

- 第 1 个按全价。
- 第 2 个及以后按 `discountRate` 比例。
- `TOPPRICE > 0` 且计算金额超过 `TOPPRICE` 时，最终金额截断为 `TOPPRICE`。
- 赋值字段为 `FT.TotCost` 和 `FT.OwnCost`。

---

## 9. 新 `his-client` 接入样例调用流程

当前仓库 `his-client/` 是规则中心改造后的 HIS 端接入示例，不是旧系统生产逻辑，但它定义了迁移后应替换旧逻辑的流程。

源码证据：

- 特殊标识查询：`his-client/PricingHisIntegrationHelper.cs:65`
- 弹窗试算：`his-client/FrmPricingPopup.cs:286`，`FrmPricingPopup.cs:295`
- 弹窗确认：`his-client/FrmPricingPopup.cs:255`
- HIS 落账成功后 commit：`PricingHisIntegrationHelper.cs:120`
- HIS 落账失败后 cancel：`PricingHisIntegrationHelper.cs:136`
- API 客户端方法：`PricingApiClient.cs:78` 到 `PricingApiClient.cs:145`

迁移后目标流程：

```text
HIS 收费录入
  -> GetSpecialFlag(itemCode)
       服务不可用：阻断收费，不允许普通计价
       非特殊项目：走旧普通收费
       特殊项目：打开 FrmPricingPopup
  -> Simulate(request)
       展示折后金额、折价原因、TraceSteps
  -> Confirm(request)
       占用额度，返回 RequestId
  -> HIS 本地落账
       成功：CommitAfterHisSuccess(RequestId, chargeNo)
       失败/取消：CancelAfterHisFailure(RequestId)
  -> 退费：Reverse(request)
```

与旧系统的关键差异：

- 旧系统在收费流程中直接改 `FeeItemList` 金额；新系统通过 confirm 返回最终明细，HIS 需要回填最终数量、金额、折价金额和追溯号。
- 旧系统历史次数来自 HIS 收费明细 SQL；新系统额度来自 `PR_LIMIT_OCCUPY`，上线当天需要历史占额迁移或兜底查询。
- 旧系统自助机/微信当前不折价；新系统统一入口默认会折价，需要按渠道做业务确认或规则条件排除。

---

## 10. 当前需要业务或源码补齐的点

1. 补齐 `Fee.PactUnitItemRate.RestrictingfeePay2` 和 `Fee.PactUnitItemRate.RestrictingfeePayZT` 的完整 SQL，确认 2 小时窗口、退费排除、门诊住院累计范围。
2. 确认自助机迁移后是否要执行特殊折价。旧源码当前不执行。
3. 确认微信迁移后是否要执行特殊折价。旧源码当前不执行。
4. 7021 科室排除需要迁移为规则条件或引擎条件，不能遗漏。
5. 历史 `FIN_DISCOUNT_FEE` 无最低限价字段，迁移历史规则时不要凭空创建金额下限动作。
6. 住院医技界面侧目前只确认比例折价展示，数量限制在最终 `FeeManager` 落账时执行。若业务要求界面预览也显示数量截断，需要额外改 UI 侧试算。
7. 组套主项是由 HIS 展开后传规则中心，还是规则中心内部展开，需要在联调前定下来。

---

## 11. 迁移映射建议

| 旧实现 | 新规则中心建议 |
|--------|----------------|
| `Restrictingfee` 常数 | `PR_RULE_CONDITION` 项目匹配 + 时间窗数量限制动作 |
| `getRestrictingfee` | `TimeWindowLimitExecutor` + `PR_LIMIT_OCCUPY`，上线过渡期补旧 HIS 明细 |
| `RestrictingfeeZT` | `PR_ITEM_GROUP` / `PR_ITEM_GROUP_DETAIL` + 同组互斥动作 |
| `RestrictingfeeCP` | 按床旁场景或项目组建立时间窗/互斥规则 |
| `RestrictingfeeTX1` | 按胎心项目组建立同组互斥或时间窗规则 |
| `Astrictpackagefee` | 迁移为规则排除条件，或不为豁免子项建立规则 |
| `FIN_DISCOUNT_FEE.discount_rate` | `IncrementPercentExecutor` 参数 |
| `FIN_DISCOUNT_FEE.topprice` | `AmountCeilingExecutor` 参数 |
| `ConvertRestrictingfeeCharge` | HIS 端展开明细后批量 confirm，或规则中心实现组套展开适配层 |
| `ConvertDiscountfee/ZY` | 公式动作 + 金额上限动作 |

---

## 12. 可直接用于联调的调用链清单

```text
人工门诊明细收费
ucDisplay.GetFeeItemList
  -> SetRestrictingfee / SetDiscountfee
  -> ConvertRestrictingfee
  -> ConvertDiscountfee

人工门诊组套/划价显示
ucDisplay.SetChargeInfo 附近逻辑
  -> SetRestrictingfee / SetDiscountfee
  -> ConvertRestrictingfeeCharge
  -> ConvertDiscountfee

住院终端确认
ucInpatientConfirm.Save
  -> feeManager.FeeItem
  -> Fee.FeeManager
  -> SetRestrictingfee / SetDiscountfee
  -> ConvertRestrictingfeeZY
  -> ConvertDiscountfeeZY

住院医技界面预览
ucInpatientCharge / ucInpatientCharge_new
  -> SetDiscountfee
  -> 按比例和 TOPPRICE 计算界面 TotCost
  -> 最终落账仍进入 FeeManager

自助机
Patientopb.GetGetPrescriptionAndChargeDetailsForSRM
  -> CTMRFeeRule.GetFeeItemListnew
  -> 组套汇总
  -> 折价代码注释，当前不折价

微信
RegisterInfoManager.GetFeeItemList / OrderManager.GetFeeItemList
  -> ConvertGroupToDetail
  -> NeusoftBussiness.MakeInvoice
  -> 未见特殊折价调用
```
