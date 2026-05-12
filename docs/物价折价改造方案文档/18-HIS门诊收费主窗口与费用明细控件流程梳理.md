# HIS 门诊收费主窗口与费用明细控件流程梳理

> 配套可视化版本：[`18-HIS门诊收费主窗口与费用明细控件流程梳理.html`](18-HIS门诊收费主窗口与费用明细控件流程梳理.html)。
>
> 如果只是想快速梳理当前门诊收费主逻辑，建议先打开 HTML 看板，从“控件组成 -> 门诊号输入 -> 加载费用明细 -> 明细控件闭环 -> 确认收费 -> 统一计价边界”顺序看。Markdown 保留更完整的文字说明和源码锚点。

## 0. 快速阅读方法

这份文档按四条线理解即可：

| 线索 | 看什么 | 核心结论 |
| --- | --- | --- |
| 界面控件线 | `registerControl`、`itemInputControl`、`leftControl`、`rightControl`、`popFeeControl` | `ucCharge` 是容器，患者信息、费用明细、支付弹窗大多靠插件加载。 |
| 患者选择线 | `tbCardNO_KeyDown()`、`InputedCardAndEnter`、`ucShowPatients`、`SelectedPatient` | 输入门诊号/卡号后，患者控件发事件，主窗口弹挂号选择，选中后返回 `Register`。 |
| 费用明细线 | `QueryChargedFeeItemListsByClinicNO(register.ID)`、`FeeDetails`、`FeeDetailsSelected`、`ChargeInfoList` | 主窗口查全部未结算明细，患者控件按处方序号分组，明细控件只展示当前选中组。 |
| 收费落账线 | `SaveFee()`、`frmDealBalance`、`popFeeControl_FeeButtonClicked()`、`Fee.cs ClinicFee` | `SaveFee()` 只是准备收费和弹支付窗，最终资金边界在 `Fee.cs` 的 confirm/commit/cancel。 |

三个最关键的数据对象：

| 对象 | 从哪里来 | 用到哪里 |
| --- | --- | --- |
| `Register` | `ucShowPatients` 选择挂号后返回 | 患者控件、明细控件、医保接口、收费落账全程使用。 |
| `FeeItemList` | 医生已开立未结算明细，或 `ucDisplay.SetItem()` 手工生成 | 费用表格展示、金额汇总、发票拆分、HIS 收费写库。 |
| `Balance` / `BalancePay` | `SaveFee()` 发票拆分和支付弹窗生成 | `popFeeControl_FeeButtonClicked()` 最终收费落账、打印和医保结算。 |

## 1. 分析范围

本文件梳理旧 HIS 门诊收费主窗口 `ucCharge` 与珠海本地费用明细控件 `ucDisplay` 的关联方式、患者门诊号输入后的挂号选择链路、未结算费用加载链路，以及划价/收费保存时的主业务流程。

核心源码：

| 文件 | 作用 |
| --- | --- |
| `legacy-code/HIS/HISFC.Components/OutpatientFee/Controls/ucCharge.cs` | 门诊收费主窗口。负责装载插件控件、患者选择、费用选择、划价保存、收费结算、发票、医保、支付弹窗和打印。 |
| `legacy-code/HIS/SOC.Local.OutpatientFee.ZhuHai/Zdwy/IOutpatientItemInputAndDisplay/ucDisplay.cs` | 珠海本地费用明细录入/展示控件。负责显示未结算明细、手工录入项目、旧折价/统一计价预览、汇总金额、返回收费明细。 |
| `legacy-code/HIS/HISFC.BizProcess/Integrate/Fee/IInterface.cs` | `IOutpatientItemInputAndDisplay`、`IOutpatientInfomation` 等主窗口插件接口定义。 |
| `legacy-code/HIS/HISFC.BizProcess/Integrate/Fee/Fee.cs` | `GetPlugIns<T>` 插件加载、门诊落账 `ClinicFee`、统一计价 confirm/commit/cancel 落点。 |
| `legacy-code/HIS/HISFC.Components/OutpatientFee/Controls/ucShowPatients.cs` | 输入卡号/门诊号后，多挂号记录选择弹窗内容控件。 |
| `legacy-code/HIS/SOC.Local.OutpatientFee.ZhuHai/Zdwy/IOutpatientInfomation/ucPatientInfo.cs` | 珠海本地患者/挂号信息录入控件。负责卡号回车、处方序号列表、已选未结算费用分组。 |

## 2. 总结论

`ucCharge.cs` 并不直接 `new Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientItemInputAndDisplay.ucDisplay()`。

实际关联链路是：

```text
ucCharge.Init()
  -> LoadPulgIns()
  -> feeIntegrate.GetPlugIns<IOutpatientItemInputAndDisplay>(Const.INTERFACE_ITEM_INPUT, null)
  -> 读取控制参数 MZ0084
  -> 从 HIS 运行目录加载 DLL 并 Activator.CreateInstance()
  -> 得到实现 IOutpatientItemInputAndDisplay 的控件实例
  -> InitItemInputControl()
  -> plMain.Controls.Add((Control)itemInputControl), Dock = Fill
```

关键证据：

| 结论 | 代码位置 |
| --- | --- |
| 主窗口字段只持有接口 `IOutpatientItemInputAndDisplay itemInputControl` | `ucCharge.cs:64-69` |
| 主窗口用 `INTERFACE_ITEM_INPUT` 加载费用明细插件 | `ucCharge.cs:2215-2219` |
| 如果插件参数为空或加载失败，才回退到主工程内置 `new ucDisplay()` | `ucCharge.cs:2217-2220` |
| `INTERFACE_ITEM_INPUT` 常量值是 `MZ0084` | `Const.cs:386` |
| `MZ0084` 映射接口名 `IOutpatientItemInputAndDisplay` | `Const.cs:432-433` |
| `GetPlugIns<T>` 读取控制参数，按 `dll|namespace.class` 格式反射创建实例 | `Fee.cs:3783-3817` |
| 珠海本地 `ucDisplay` 实现了 `IOutpatientItemInputAndDisplay` | `SOC.Local.../ucDisplay.cs:19-21` |

因此，`ucCharge` 与珠海本地 `ucDisplay` 的运行时关系取决于现场控制参数 `MZ0084`。典型值应类似：

```text
\SOC.Local.OutpatientFee.ZhuHai.dll|Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientItemInputAndDisplay.ucDisplay
```

如果 `MZ0084` 没有配置，主窗口会回退到 `legacy-code/HIS/HISFC.Components/OutpatientFee/Controls/ucDisplay.cs`，而不是珠海本地 `SOC.Local...ucDisplay.cs`。

## 3. 主窗口控件组成

`ucCharge` 是一个容器型主窗口，实际业务区域由多个插件控件拼装：

| 主窗口字段 | 接口 | 运行位置 | 职责 |
| --- | --- | --- | --- |
| `registerControl` | `IOutpatientInfomation` | `plTop` | 患者卡号/门诊号输入、挂号信息、处方序号、已选费用明细。 |
| `itemInputControl` | `IOutpatientItemInputAndDisplay` | `plMain` | 费用明细列表、手工录入项目、金额计算、返回收费明细。 |
| `leftControl` | `IOutpatientOtherInfomationLeft` | 左侧区域 | 发票预览、票据相关信息。 |
| `rightControl` | `IOutpatientOtherInfomationRight` | 右侧区域 | 患者费用汇总、收费金额展示。 |
| `popFeeControl` | `IOutpatientPopupFee` | 弹窗 | 实收金额、支付方式、收费确认/划价保存。 |
| `ucShow` | `ucShowPatients` | `fPopWin` 弹窗 | 多挂号记录选择。 |

你当前调试到的运行时实例关系如下，这组对象就是门诊收费主界面实际拼起来的控件树和外围窗口：

| 主窗口字段 | 当前运行时实例 | 业务含义 |
| --- | --- | --- |
| `registerControl` | `Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientInfomation.ucPatientInfo` | 珠海本地患者信息控件。门诊号/卡号输入、挂号选择、处方序号和未结算明细分组都从这里发起。 |
| `itemInputControl` | `Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientItemInputAndDisplay.ucDisplay` | 珠海本地费用明细控件。挂号选择后，主窗口把未结算明细通过 `ChargeInfoList` 塞给它，它再显示到中间费用表格。 |
| `leftControl` | `Neusoft.HISFC.Components.OutpatientFee.Controls.ucInvoicePreview` | 主工程内置发票预览控件。收费前校验发票号，收费后刷新票据预览。 |
| `rightControl` | `Neusoft.HISFC.Components.OutpatientFee.Controls.ucCostDisplay` | 主工程内置金额展示控件。显示患者费用汇总、支付金额、医保/自费等信息。 |
| `popFeeControl` | `Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientPopupFee.frmDealBalance` | 珠海本地支付弹窗。`SaveFee()` 组好费用、发票、医保预结算结果后弹出它，真正点击收费后回调主窗口落账。 |
| `iMultiScreen` | `Neusoft.SOC.Local.OutpatientFee.GuangZhou.Gyzl.IMultiScreen.frmMiltScreen`，标题为 `门诊外屏` | 外屏窗口。主窗口创建后交给 `rightControl.MultiScreen`，金额变化时由右侧金额控件同步外屏显示。 |
| `iBankTrans` | `Neusoft.HISFC.Components.OutpatientFee.Forms.frmBankTrans` | 银行卡交易窗口。主窗口赋给 `popFeeControl.BankTrans`，支付弹窗内需要银行卡交易时调用。 |

这也说明当前现场不是纯主工程默认界面：患者信息、费用明细、支付弹窗来自珠海本地实现，左侧发票预览、右侧金额展示、银行卡交易仍走主工程默认实现，外屏则走现场可配置的本地实现。

初始化顺序在 `ucCharge.Init()` 中固定：

```text
InitControlParams()
LoadPulgIns()
InitRegisterControl()
InitItemInputControl()
InitRightControl()
InitLeftControl()
InitPopFeeControl()
InitPopShowPatient()
Refresh()
```

对应代码：`ucCharge.cs:706-727`。

## 4. 插件装载细节

### 4.1 主窗口加载费用明细控件

`ucCharge.LoadPulgIns()` 中先加载患者控件，再加载费用明细控件：

```text
registerControl = GetPlugIns<IOutpatientInfomation>(INTERFACE_REGINFO, null)
itemInputControl = GetPlugIns<IOutpatientItemInputAndDisplay>(INTERFACE_ITEM_INPUT, null)
```

费用明细控件创建后，主窗口立即写入基础上下文：

| 赋值 | 含义 | 代码位置 |
| --- | --- | --- |
| `itemInputControl.ItemKind = itemKind` | 控制药品/非药品/全部项目范围 | `ucCharge.cs:2221` |
| `itemInputControl.CustomEvent += Pact_Foucs` | 本地控件触发合同单位焦点事件 | `ucCharge.cs:2222` |
| `itemInputControl.LeftControl = leftControl` | 明细控件可以刷新左侧发票预览 | `ucCharge.cs:2234` |
| `itemInputControl.RightControl = rightControl` | 明细控件可以刷新右侧金额汇总 | `ucCharge.cs:2250` |
| `itemInputControl.IsCanSelectItemAndFee = isCanSelectItemAndFee` | 是否允许勾选部分明细收费 | `ucCharge.cs:2252` |
| `itemInputControl.YBPactCode = ybPactCode` | 医保合同单位显示相关参数 | `ucCharge.cs:2253` |

### 4.2 明细控件嵌入主界面

`ucCharge.InitItemInputControl()` 把 `itemInputControl` 加入 `plMain`：

```text
plMain.Controls.Add((Control)itemInputControl)
((Control)itemInputControl).Dock = DockStyle.Fill
itemInputControl.Init()
itemInputControl.FeeItemListChanged += itemInputControl_FeeItemListChanged
itemInputControl.IsUseNewUndrugZT = IsUseNewUndrugZT
```

对应代码：`ucCharge.cs:3601-3617`。

这就是“费用明细界面嵌入主界面”的实际位置。`ucCharge` 本身不画费用表格，表格全部在 `itemInputControl` 实现内。

## 5. 患者门诊号输入到挂号选择

### 5.1 输入入口在患者信息控件

珠海本地患者控件 `IOutpatientInfomation/ucPatientInfo.cs` 负责卡号/门诊号输入。用户在卡号输入框回车后进入 `tbCardNO_KeyDown()`：

1. 读取 `tbCardNO.Text.Trim()`。
2. 校验输入是否合法。
3. 清空当前患者、费用、处方序号等旧状态。
4. 如果以 `/` 或 `+` 开头，走“无挂号直接收费/临时患者”逻辑，生成临时卡号和就诊流水号，并 `AddNewRecipe()`。
5. 普通卡号/门诊号则做账户卡识别、卡号补齐、创建空 `Register` 对象。
6. 触发接口事件 `InputedCardAndEnter(cardNO, tmpOrgCardNo, tbCardNO.Location, tbCardNO.Height)`。

对应代码：`SOC.Local.../IOutpatientInfomation/ucPatientInfo.cs:2304-2418`。

### 5.2 主窗口接收输入事件

`ucCharge.InitRegisterControl()` 在初始化患者控件时绑定：

```text
registerControl.InputedCardAndEnter += registerControl_InputedCardAndEnter
```

对应代码：`ucCharge.cs:3581-3590`。

事件处理 `registerControl_InputedCardAndEnter()` 做三件事：

1. 把输入卡号写给 `ucShow`：

```text
ucShow.OrgCardNO = orgNO
ucShow.CardNO = cardNO
ucShow.operType = "1"
```

2. `ucShow.CardNO` 的 setter 会立即查询挂号记录。
3. 如果查询出多条记录，或者允许重新挂号且查出一条记录，则在输入框下方弹出 `fPopWin.ShowDialog()`。

对应代码：`ucCharge.cs:3646-3664`。

### 5.3 挂号选择弹窗如何查询和展示

`ucShowPatients.CardNO` setter 调用 `FillPatientInfoByCardNO()`，后者执行：

```text
QueryPatientInfosByCardNO(cardNO)
DisplayPatients(patients)
```

查询逻辑：

| 步骤 | 说明 | 代码位置 |
| --- | --- | --- |
| 读取挂号有效天数参数 `VALID_REG_DAYS` | 决定从哪个日期开始查有效挂号 | `ucShowPatients.cs:298-316` |
| `registerManager.QueryValidPatientsByCardNO(cardNO, dtQueryBeginTime)` | 按卡号查有效挂号记录 | `ucShowPatients.cs:319-320` |
| `QueryCheckPatients(cardNO)` | 合并体检登记记录 | `ucShowPatients.cs:331-340` |
| 可选按看诊序号查挂号 | 参数 `REG_RECIPE_NO_RELPACE_CARD_NO` 开启后按 `orgCardNO` 查 | `ucShowPatients.cs:350-388` |

展示逻辑：

| 情况 | 处理 |
| --- | --- |
| 无挂号记录，且允许重新挂号 | 用患者基本信息构造一个“补挂号/临时挂号”的 `Register`，再触发 `SelectedPatient`。 |
| 只有一条挂号，且不允许重新挂号 | 不弹窗，直接触发 `SelectedPatient`。 |
| 多条挂号，或允许重新挂号 | 填充 `neuSpread1` 列表，由收费员双击或回车选择。 |

对应代码：`ucShowPatients.cs:466-549`。

弹窗列表还会调用 `outpatientManager.QueryChargedFeeItemListsByClinicNO(patient.ID)` 判断每条挂号是否已有未收费费用，有费用的行会标绿色并打标识，方便收费员选择有明细的挂号记录。对应代码：`ucShowPatients.cs:538-545`。

### 5.4 选择挂号后回到主窗口

收费员双击或回车后，`ucShowPatients.SelectPatient()` 触发 `SelectedPatient(register)` 并关闭弹窗：

```text
SelectedPatient((Register)neuSpread1_Sheet1.Rows[row].Tag)
FindForm().Close()
```

对应代码：`ucShowPatients.cs:585-588`、`ucShowPatients.cs:650-662`。

主窗口在 `InitPopShowPatient()` 中绑定：

```text
ucShow.SelectedPatient += ucShow_SelectedPatient
```

对应代码：`ucCharge.cs:765-778`。

## 6. 选择挂号后加载未结算费用

`ucCharge.ucShow_SelectedPatient(register)` 是患者选择后的主流程。

### 6.1 主窗口先设置患者上下文

主要动作：

1. 未来预约号提示确认。
2. 特殊合同单位 `258` 提示确认。
3. 医保/待遇接口可选获取患者待遇信息。
4. `registerControl.PatientInfo = register`。
5. `rightControl.SetInfomation(..., "4")` 刷新右侧患者信息。
6. `itemInputControl.PatientInfo = register`，把同一个 `Register` 传给费用明细控件。

对应代码：`ucCharge.cs:785-849`。

### 6.2 查询 HIS 未结算费用明细

主窗口用挂号流水号 `register.ID` 查询未结算明细：

```text
ArrayList feeItemLists = outpatientManager.QueryChargedFeeItemListsByClinicNO(register.ID)
```

对应代码：`ucCharge.cs:854-856`。

如果参数要求只显示当前科室费用，主窗口会按当前收费员科室过滤；否则把完整未结算明细写入患者控件：

```text
registerControl.FeeDetails = feeItemLists.Clone()
```

对应代码：`ucCharge.cs:865-885`。

### 6.3 患者控件把明细按处方序号分组

珠海本地患者控件 `FeeDetails` setter 会调用自身 `SetChargeInfo()`：

```text
this.feeDetails = value
this.SetChargeInfo()
```

对应代码：`SOC.Local.../IOutpatientInfomation/ucPatientInfo.cs:1590-1602`。

`SetChargeInfo()` 会：

1. 清空处方序号列表。
2. 按 `FeeItemList.RecipeSequence` 把未结算费用分组。
3. 每组计算金额合计。
4. 写入 `fpRecipeSeq` 处方列表。
5. 默认选择第一组，或按参数全选。
6. 设置 `feeDetailsSelected`。
7. 设置当前 `recipeSequence`。

对应代码：`SOC.Local.../IOutpatientInfomation/ucPatientInfo.cs:1938-2074`。

这一步解释了为什么主窗口里有“处方序号/费用组选中”的概念：`ucCharge` 查询到的是全部未结算明细，`registerControl` 负责按处方序号分组并产生当前选中明细。

### 6.4 主窗口把当前选中明细推给费用明细控件

患者控件处理完处方序号后，主窗口继续把关键上下文推给明细控件：

```text
itemInputControl.IsCanAddItem = registerControl.IsCanAddItem
itemInputControl.RecipeSequence = registerControl.RecipeSequence
itemInputControl.ChargeInfoList = registerControl.FeeDetailsSelected
registerControl_SeeDoctChanged(...)
```

对应代码：`ucCharge.cs:888-893`。

这里的 `ChargeInfoList` 是主窗口和 `ucDisplay` 之间最关键的数据通道：它就是“当前挂号记录下，当前选中的未结算费用明细”。

## 7. `ucDisplay` 如何显示费用明细

### 7.1 接收 `ChargeInfoList`

珠海本地 `ucDisplay.ChargeInfoList` setter：

```text
this.alChargeInfo = value
isDealCellChange = false
SetChargeInfo()
isDealCellChange = true
```

对应代码：`SOC.Local.../IOutpatientItemInputAndDisplay/ucDisplay.cs:1207-1228`。

`isDealCellChange = false` 的目的，是批量加载明细时先禁止单元格变化事件重复触发金额重算。

### 7.2 明细控件展示流程

`ucDisplay.SetChargeInfo()` 是“未结算费用明细显示到表格”的核心方法。

主要流程：

1. `Clear()` 清空原表格。
2. 准备旧折价相关集合，如 `hsREOnlyOneItem`、`hsREOnlylistItem`。
3. 如果未启用 PricingAgent，则对加载进来的 `alChargeInfo` 先跑旧逻辑：
   - `undrugManager.SetRestrictingfee()`
   - `undrugManager.SetDiscountfee()`
   - `setRestrictingfee.ConvertRestrictingfeeCharge()`
   - `setRestrictingfee.ConvertDiscountfee()`
4. 如果启用 PricingAgent，则不跑旧折价，改为整体调用统一计价预览。
5. 遍历每一条 `FeeItemList`：
   - 识别药品/非药品/材料标志。
   - 从 `dsItem` 中按项目编码找主数据。
   - 如果项目主数据缺失，再按处方号和序号判断是否允许重新查询有效项目。
   - 带出价格、儿童价、包装数量、单位、执行科室、频次、用法。
   - 处理公费、公医比例、超声第二项减价等本地规则。
   - 写入 FarPoint 表格列。
   - `Rows[currRow].Tag = f`，把真实 `FeeItemList` 绑定在表格行上。
   - 写入是否预约、是否确认、医保等级等扩展信息。
6. 调用 `SumCost()` 汇总金额并通知主窗口。

对应代码：`SOC.Local.../IOutpatientItemInputAndDisplay/ucDisplay.cs:2383-2900`。

### 7.3 明细控件与主窗口的反向同步

`ucDisplay.SumCost()` 不只是算总金额，还负责把变化反推给主窗口：

```text
alFee = GetFeeItemList()
rightControl.SetInfomation(rInfo, null, alFee, null, "0")
leftControl.RefreshDisplayInfomation(alFee)
alCharge = GetFeeItemListForCharge()
FeeItemListChanged(alCharge)
```

对应代码：`SOC.Local.../IOutpatientItemInputAndDisplay/ucDisplay.cs:4764-4788`。

主窗口订阅了 `FeeItemListChanged`：

```text
itemInputControl.FeeItemListChanged += itemInputControl_FeeItemListChanged
```

对应代码：`ucCharge.cs:3612-3615`。

主窗口事件处理：

```text
registerControl.ModifyFeeDetails = al.Clone()
registerControl.DealModifyDetails()
```

对应代码：`ucCharge.cs:3625-3634`。

珠海本地患者控件 `DealModifyDetails()` 会按 `RecipeSequence` 重新汇总处方序号列表上的金额。对应代码：`SOC.Local.../IOutpatientInfomation/ucPatientInfo.cs:955-1040`。

这形成一个闭环：

```text
registerControl.FeeDetailsSelected
  -> ucCharge.itemInputControl.ChargeInfoList
  -> ucDisplay.SetChargeInfo()
  -> ucDisplay.SumCost()
  -> FeeItemListChanged(alCharge)
  -> ucCharge.itemInputControl_FeeItemListChanged()
  -> registerControl.ModifyFeeDetails
  -> registerControl.DealModifyDetails()
```

## 8. 手工录入项目流程

`ucDisplay.Init()` 会初始化可收费项目列表、项目选择弹窗、执行科室、频次、用法、合同单位、右侧金额控件等。关键点：

| 动作 | 代码位置 |
| --- | --- |
| 初始化参数和操作员 | `ucDisplay.cs:6636-6657` |
| 加载项目列表 `LoadItem(myOperator.Dept.ID)` | `ucDisplay.cs:6818-6821` |
| 初始化 FarPoint 表格 | `ucDisplay.cs:6823-6824` |
| 加载项目选择插件 `IChooseItemForOutpatient`，默认 `ucPopSelected` | `ucDisplay.cs:6826-6832` |
| 绑定项目选择事件 `SelectedItem += chooseItemControl_SelectedItem` | `ucDisplay.cs:6860-6861` |
| 初始化执行科室、频次、用法、合同单位 | `ucDisplay.cs:6864-6877` |
| 初始化右侧金额控件数据源 | `ucDisplay.cs:6881-6886` |

收费员在项目输入列录入拼音/五笔/编码时，`fpSpread1_EditChange()` 会把输入文本传给项目选择控件过滤；选择项目后进入 `SetItem()`。对应代码：`ucDisplay.cs:10344-10370`、`ucDisplay.cs:7016-7140`。

`SetItem()` 是手工录入项目的核心方法，主要负责：

1. 校验是否已选择患者和科室。
2. 从项目列表中定位项目主数据。
3. 如果当前价格和最新价格不同，刷新项目缓存。
4. 识别非药品、药品、组套、协定处方、材料等项目类型。
5. 组套项目展开为明细再次递归 `SetItem()`。
6. 生成新的 `FeeItemList`，填入项目、价格、单位、数量、执行科室、处方序号、患者信息。
7. 调用适应症接口。
8. 未启用 PricingAgent 时执行旧折价/限价逻辑。
9. 把 `FeeItemList` 放入当前行 `Rows[row].Tag`。
10. 调用 `RefreshPricingPreviewForSingleRow()` 和 `RefreshItemInfo()`。

对应代码：`SOC.Local.../IOutpatientItemInputAndDisplay/ucDisplay.cs:3392-4514`。

## 9. 统一计价预览和旧折价的关系

当前 `ucDisplay.cs` 已接入 PricingAgent 预览逻辑，但仅发生在界面阶段：

| 逻辑 | 说明 | 代码位置 |
| --- | --- | --- |
| `IsPricingAgentEnabled()` | 以 HIS 运行目录是否存在 `pricing-agent.config` 判断是否启用 | `ucDisplay.cs:31-40` |
| `GetPricingPreview()` | 组织当前明细调用 `PricingOutpatientPreviewService.Simulate()` | `ucDisplay.cs:46-75` |
| `RefreshPricingPreviewForCurrentRows()` | 按当前所有选中明细整体试算，不只试算单行 | `ucDisplay.cs:255-297` |
| `ApplyPricingPreviewAmount()` | 把预览最终金额写入 `FT.TotCost/OwnCost`，但不写旧 `RebateCost` | `ucDisplay.cs:97-114` |
| `ApplyPricingPreviewMemo()` | 把预览摘要写入备注列，并将特殊项目标蓝 | `ucDisplay.cs:168-195` |
| `SetChargeInfo()` | 启用 PricingAgent 时跳过旧 `SetRestrictingfee/SetDiscountfee` | `ucDisplay.cs:2431-2491` |
| `SetItem()` | 手工录入项目时，启用 PricingAgent 则跳过旧折价，再刷新统一计价预览 | `ucDisplay.cs:4391-4510` |
| `GetFeeItemList()` | 最终取当前收费明细时，启用 PricingAgent 则跳过旧折价 | `ucDisplay.cs:8088-8157` |

需要注意：`ucCharge.SaveFee()` 本身不直接调用 PricingAgent。主窗口只负责从 `itemInputControl.GetFeeItemList()` 取到当前费用明细，再调用 `feeIntegrate.ClinicFee(...)`。真正的统一计价 confirm 发生在 `HISFC.BizProcess.Integrate.Fee.Fee.cs`：

1. `ClinicFee()` 先生成稳定的 `invoiceCombNO` 和处方号/明细号。
2. 生成处方号后调用 `PricingChargeBridge.ConfirmOutpatientBeforeSave(...)`。
3. confirm 不通过则返回 `false`，主窗口收费失败。
4. HIS 明细写库成功后 `MarkHisSaveSucceeded()`。
5. `Commit()` 成功提交 HIS 事务后调用 `CommitSavedCharges()`。
6. `Rollback()` 或医保提交失败时调用 `CancelUncommittedCharges()`。

对应代码：`Fee.cs:5584-5705`、`Fee.cs:6544-6548`、`Fee.cs:2744-2780`、`Fee.cs:2813-2818`。

## 10. 收费保存主流程

主窗口收费入口有两个：

| 入口 | 调用 |
| --- | --- |
| 工具栏“确认收费” | `ToolStrip_ItemClicked()` -> `SaveFee()`，`ucCharge.cs:4354-4358` |
| 框架保存事件 | `OnSave()` -> `SaveFee()`，`ucCharge.cs:4588-4592` |

`SaveFee()` 流程如下。

### 10.1 前置校验

1. 校验发票号更新时间。
2. 校验患者信息存在。
3. `registerControl.IsPatientInfoValid()` 校验患者/挂号信息。
4. `registerControl.GetRegInfo()` 从界面刷新 `Register` 对象。
5. `itemInputControl.IsValid` 校验明细表格。
6. `itemInputControl.StopEdit()` 停止 FarPoint 编辑。
7. `leftControl.IsValid()` 校验发票/左侧信息。
8. `itemInputControl.GetFeeItemList()` 获取最终待收费明细。

对应代码：`ucCharge.cs:1126-1194`。

### 10.2 明细业务校验

主窗口对 `comFeeItemLists` 执行大量业务校验：

| 校验 | 代码位置 |
| --- | --- |
| 没有费用明细则阻断 | `ucCharge.cs:1195-1201` |
| 长者券合同单位 `258` 未处理记录提示 | `ucCharge.cs:1204-1214` |
| 处方类型和合同单位不匹配提示/阻断 | `ucCharge.cs:1229-1378` |
| 7 岁以下儿童附加项目校验 | `ucCharge.cs:1283-1295` |
| `ForbidmeanwhileFee` 同时收费提示 | `ucCharge.cs:1239-1315` |
| 无痛胃肠镜相关提示 | `ucCharge.cs:1317-1330` |
| LIS 试管自动加收 | `ucCharge.cs:1412-1449` |
| 停用项目校验 | `ucCharge.cs:1452-1457` |
| 药品库存/执行科室校验 | `ucCharge.cs:1459-1496` |

### 10.3 发票、医保预结算和金额计算

1. 从左侧控件取当前显示发票号。
2. 调用 `feeIntegrate.GetInvoiceNOWithHosCode()` 取正式发票号。
3. 开启 HIS 事务。
4. 连接医保/待遇接口。
5. 清空 `SIMainInfo` 费用字段。
6. 删除医保已上传明细。
7. 上传当前费用明细。
8. 非公费患者执行医保预结算。
9. 汇总 `OwnCost/PayCost/PubCost/TotCost/RebateCost`。
10. 医保总额与 HIS 总额不一致时按参数决定是否阻断。
11. 金额保留两位。
12. 账户余额不足时提示充值。
13. 可选执行收费金额取整接口。

对应代码：`ucCharge.cs:1498-1845`。

### 10.4 支付弹窗和发票预生成

`SaveFee()` 会重新创建 `popFeeControl`，写入患者、费用、发票、金额等上下文：

```text
popFeeControl.PatientInfo = registerControl.PatientInfo
popFeeControl.FeeDetails = comFeeItemLists
popFeeControl.InvoiceFeeDetails = balancesAndBalanceLists[2]
popFeeControl.InvoiceDetails = balancesAndBalanceLists[1]
popFeeControl.Invoices = balancesAndBalanceLists[0]
```

发票数据由：

```text
Class.Function.MakeInvoice(feeIntegrate, patient, comFeeItemLists, invoiceNO, realInvoiceNO, ref errText)
```

生成。对应代码：`ucCharge.cs:1848-2008`。

弹窗打开后：

| 用户动作 | 事件 | 主窗口处理 |
| --- | --- | --- |
| 点击收费 | `FeeButtonClicked` | 进入 `popFeeControl_FeeButtonClicked(...)` 真实落账 |
| 点击划价保存 | `ChargeButtonClicked` | 调用 `SaveCharge()` |
| 取消/关闭 | `IsSuccessFee = false` | 回滚 HIS 事务和医保接口 |

取消路径对应代码：`ucCharge.cs:2009-2027`。

### 10.5 最终落账

`popFeeControl_FeeButtonClicked()` 是最终收费确认后的落账入口。

主要流程：

1. 判断是否临时发票。
2. 开启事务。
3. 连接医保接口。
4. 根据医保接口能力分两条路径：
   - `IsUploadAllFeeDetailsOutpatient == true`：整单写库、上传医保、医保结算、提交事务。
   - 否则：先写支付方式，再按发票逐张上传医保、结算、写库、提交。
5. 调用 `feeIntegrate.ClinicFee(... ChargeTypes.Fee ...)` 或 `ClinicFeeSaveFee(...)` 保存 HIS 收费数据。
6. 医保 commit 成功后提交 HIS 事务。
7. 电子票据/纸质票据处理。
8. 发药窗口/打包发药接口处理。
9. PACS 申请单收费标志更新。
10. 打印发票和导诊单。
11. 执行 `afterFee.AfterFee(comFeeItemLists, "0")`。
12. 提示收费成功并 `Clear()`。

关键代码：`ucCharge.cs:2382-3355`。

## 11. 划价保存流程

工具栏“划价保存”或弹窗“划价保存”都会调用 `SaveCharge()`：

| 入口 | 代码位置 |
| --- | --- |
| 工具栏 | `ucCharge.cs:4360-4362` |
| 支付弹窗 | `ucCharge.cs:2370-2373` |
| 快捷键 | `ucCharge.cs:3942-3949` |

`SaveCharge()` 与 `SaveFee()` 的差异：

| 项目 | `SaveCharge()` | `SaveFee()` |
| --- | --- | --- |
| 目的 | 划价保存，不收款 | 确认收费并落账 |
| 明细来源 | `registerControl.FeeSameDetails` / `FeeDetailsSelected` | `itemInputControl.GetFeeItemList()` |
| 是否弹支付窗 | 否 | 是 |
| 是否处理医保预结算/结算 | 否 | 是 |
| 保存调用 | `feeIntegrate.ClinicFee(ChargeTypes.Save, ...)` | `feeIntegrate.ClinicFee(ChargeTypes.Fee, ...)` |
| 成功后 | 提示划价成功、清屏、刷新 | 提示收费成功、清屏、打印/后处理 |

核心代码：`ucCharge.cs:961-1120`。

## 12. 处方序号切换、删除和合同单位变化

主窗口通过患者控件事件驱动明细控件刷新：

| 患者控件事件 | 主窗口方法 | 对明细控件的影响 |
| --- | --- | --- |
| `RecipeSeqChanged` | `registerControl_RecipeSeqChanged()` | 清空明细控件，重新设置 `PatientInfo`、`ChargeInfoList`、`RecipeSequence`、`IsCanAddItem`。 |
| `RecipeSeqDeleted` | `registerControl_RecipeSeqDeleted(al)` | 对每条被删费用调用 `itemInputControl.DeleteRow(f)`。 |
| `SeeDoctChanged` | `registerControl_SeeDoctChanged()` | 调用 `itemInputControl.RefreshSeeDoc()`，刷新右侧患者信息。 |
| `SeeDeptChanaged` | `registerControl_SeeDeptChanaged()` | 调用 `itemInputControl.RefreshSeeDept()`。 |
| `PriceRuleChanaged` | `registerControl_PriceRuleChanaged()` | 调用 `itemInputControl.ModifyPrice()` 或体检价 `PhyExamModifyPrice()`。 |
| `PactChanged` | `registerControl_PactChanged()` | 更新 `PatientInfo`，调用 `RefreshItemForPact()` 和右侧信息刷新。 |
| `ChangeFocus` | `registerControl_ChangeFocus()` | 焦点切到费用明细控件。 |

对应代码：`ucCharge.cs:3704-3810`。

处方序号切换最关键：

```text
itemInputControl.Clear()
itemInputControl.PatientInfo = registerControl.PatientInfo
rightControl.SetInfomation(..., "4")
itemInputControl.ChargeInfoList = registerControl.FeeDetailsSelected
itemInputControl.RecipeSequence = registerControl.RecipeSequence
itemInputControl.IsCanAddItem = registerControl.IsCanAddItem
```

对应代码：`ucCharge.cs:3744-3755`。

## 13. 需要特别注意的老代码点

1. `ucDisplay.GetFeeItemListForCharge(bool isGroupDetail)` 在珠海本地实现里主体被注释，当前直接返回空集合。`ucCharge.ChangeRecipe()` 调用的是这个 bool 重载，因此“暂存/换处方”相关逻辑可能拿不到当前明细。代码位置：`ucCharge.cs:755-759`、`ucDisplay.cs:7404-7422`。

2. `ucCharge.registerControl_InputedCardAndEnter()` 末尾有一行：

```text
this.itemInputControl.PatientInfo = this.itemInputControl.PatientInfo;
```

这行没有实际赋新值，真正把患者传给费用明细控件的是 `ucShow_SelectedPatient()` 中的 `this.itemInputControl.PatientInfo = register`。代码位置：`ucCharge.cs:3693`、`ucCharge.cs:849`。

3. `ucCharge.SaveFee()` 的界面预览金额不是最终统一计价资金边界。最终落账前仍会在 `Fee.cs` 中 confirm，HIS 写库成功后才 commit。界面 `simulate` 只是预览。

4. 主窗口对费用明细控件只认接口，不认具体类。现场排查“为什么没有加载珠海本地 `ucDisplay`”时，第一优先级应查控制参数 `MZ0084` 是否指向正确 DLL 和命名空间。

5. `ucShowPatients` 在多挂号列表中会按未收费明细给行标绿，但这只是选择辅助；真正加载明细仍发生在 `ucCharge.ucShow_SelectedPatient()` 的 `QueryChargedFeeItemListsByClinicNO(register.ID)`。

## 14. 主流程总览

完整流程可以压缩为下面这条链：

```text
收费员输入门诊号/卡号并回车
  -> 珠海患者控件 ucPatientInfo.tbCardNO_KeyDown()
  -> 触发 IOutpatientInfomation.InputedCardAndEnter
  -> ucCharge.registerControl_InputedCardAndEnter()
  -> ucShowPatients.CardNO 查询有效挂号/体检/可选看诊序号
  -> 0/1/多条挂号分别处理，必要时弹出 fPopWin
  -> 收费员选择 Register
  -> ucCharge.ucShow_SelectedPatient(register)
  -> registerControl.PatientInfo = register
  -> itemInputControl.PatientInfo = register
  -> outpatientManager.QueryChargedFeeItemListsByClinicNO(register.ID)
  -> registerControl.FeeDetails = feeItemLists
  -> registerControl 按 RecipeSequence 分组并产生 FeeDetailsSelected
  -> itemInputControl.RecipeSequence = registerControl.RecipeSequence
  -> itemInputControl.ChargeInfoList = registerControl.FeeDetailsSelected
  -> 珠海 ucDisplay.SetChargeInfo() 将明细显示到 FarPoint 表格
  -> ucDisplay.SumCost() 刷新左右金额控件并触发 FeeItemListChanged
  -> ucCharge 把修改后的明细回写 registerControl.ModifyFeeDetails
  -> 收费员确认收费
  -> ucCharge.SaveFee()
  -> itemInputControl.GetFeeItemList()
  -> 发票、医保预结算、金额汇总、支付弹窗
  -> popFeeControl_FeeButtonClicked()
  -> feeIntegrate.ClinicFee(... ChargeTypes.Fee ...)
  -> Fee.cs 生成处方号后 PricingAgent confirm
  -> HIS 写库成功
  -> HIS 事务提交后 PricingAgent commit
  -> 打印/导诊/清屏
```

## 15. 对物价折价改造的落点判断

从这条链路看，门诊收费界面有两个关键接入层：

1. **界面预览层：`SOC.Local...IOutpatientItemInputAndDisplay\ucDisplay.cs`**

   适合做“当前收费界面所有已选明细”的实时试算展示。用户加载患者费用、手工录入项目、改数量/价格/单位、勾选/取消明细时，都能在这里刷新金额和备注。

2. **最终资金层：`HISFC.BizProcess.Integrate.Fee.Fee.cs`**

   适合做 confirm/commit/cancel。因为只有这里能拿到稳定的 `invoiceCombNO`、`RecipeNO-SequenceNO`、HIS 写库成功/失败、事务提交/回滚结果。

`ucCharge.cs` 本身是编排层，不建议把复杂折价规则直接塞进主窗口。主窗口最稳定的职责是继续保持：

```text
收集患者与明细上下文
  -> 调用明细控件拿 FeeItemList
  -> 调用 Fee 集成层落账
  -> 根据结果提交/回滚/清屏
```
