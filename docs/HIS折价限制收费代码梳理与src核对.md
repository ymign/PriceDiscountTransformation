# HIS 折价与限制收费代码梳理及 src 核对

## 旧 HIS 入口链路

门诊界面入口在 `legacy-code/HIS-Use/SOC.Local.OutpatientFee.ZhuHai/Zdwy/IOutpatientItemInputAndDisplay/ucDisplay.cs`。

关键链路如下：

1. `SetItem`：用户录入项目时构造 `FeeItemList` 并写入界面行 `Tag`，单行会试算一次特殊规则，让界面金额尽量接近最终收费金额。
2. `SetChargeInfo`：加载患者已有划价明细时重算显示价。限制收费命中时调用 `ConvertRestrictingfeeCharge`，组套会先拆子项再汇总回主项显示。
3. `GetFeeItemList`：提交收费前生成最终费用明细。这里才是最终收费出口，会拆组套、处理 CT/DR 去重，再调用 `ConvertRestrictingfee` 和 `ConvertDiscountfee`。
4. `ZDWY.SpecialRule.Price.Restrictingfee`：承载旧 HIS 的限制收费、同组互斥、组套显示价重算、住院限制收费和比例折价公式。

## 旧 HIS 规则口径

### Restrictingfee

配置来源：

- `COM_DICTIONARY.TYPE = 'Restrictingfee'`
- `INPUT_CODE` 是限制数量上限
- 历史占用查询来自 `Fee.PactUnitItemRate.RestrictingfeePay2`

计算口径：

- 先查最近 2 小时历史已收费量。
- 再扣同一次收费动作中已经保留的数量。
- 剩余额度小于等于 0：当前行金额归零，`Memo = P{原数量}`。
- 剩余额度不足当前数量：数量截断为剩余额度，金额按截断数量计算，`Memo = N{原数量}`。
- 剩余额度足够：保留原金额或按单价乘数量补齐金额。

### RestrictingfeeCP / RestrictingfeeTX1 / RestrictingfeeZT

这些不是普通“同项目数量累计”：

- `RestrictingfeeCP`：床旁项目共享 2 小时额度。旧 SQL 命中当前床旁项目时，会汇总所有 `RestrictingfeeCP` 项目的历史收费量，不是只看同一个项目。
- `RestrictingfeeTX1`：胎心组套互斥，历史 SQL 命中时返回 99，效果是后续同组项目归零。
- `RestrictingfeeZT`：按 `COM_DICTIONARY.MARK` 分组互斥，历史 SQL 同样按最近 2 小时窗口判断。

### Astrictpackagefee

命中后不查询历史收费量，历史占用视为 0。

这不是“不限制”，而是“忽略历史记录”；本次收费内仍会继续扣减已经保留的项目。

### Discountfee

配置来源：

- `FIN_DISCOUNT_FEE`
- `DISCOUNT_RATE`：第二件起折扣率
- `TOPPRICE`：总金额封顶
- `DISCOUNT_TYPE = '2'` 时执行折价

公式：

```text
cost = price + price * DISCOUNT_RATE * (qty - 1)
if TOPPRICE > 0 and cost > TOPPRICE:
    cost = TOPPRICE
```

## src 新规则中心核对

已确认映射：

| 旧 HIS 规则 | src 规则/执行器 | 核对结论 |
| --- | --- | --- |
| `Restrictingfee` 2 小时数量限制 | `APPLY_TIME_WINDOW_LIMIT` / `TimeWindowLimitExecutor` | 已覆盖 2 小时窗口、历史占用、本次请求占用、超限截断 |
| 超限金额归零 | `DISCOUNT_EXCEED_TO_ZERO` / `ExceedToZeroExecutor` | 已覆盖全超限归零；部分超限金额由限额执行器按数量比例缩放 |
| `RestrictingfeeZT` 同组互斥 | `SAME_GROUP_MUTEX` / `SameGroupMutexExecutor` | 已修正为最近 2 小时窗口，不再按自然日误拦 |
| `RestrictingfeeTX1` 胎心互斥 | `SAME_GROUP_MUTEX` / `SameGroupMutexExecutor` | 与 ZT 共用互斥执行器，规则导入使用 `EXCLUSIVE_GROUP` |
| `RestrictingfeeCP` 床旁共享额度 | `APPLY_TIME_WINDOW_LIMIT` + `SAME_GROUP_MUTEX` | 普通时间窗限制同项目；额外 CP 互斥规则复刻旧 SQL “所有床旁项目共享额度”的口径 |
| `Discountfee` 比例折价 | `FORMULA_CALC` / `IncrementPercentExecutor` + `APPLY_MAX_AMOUNT` | 公式执行器已覆盖旧公式；`sql/04-import-rules.sql` 已支持运行时从 `FIN_DISCOUNT_FEE` 源表动态导入 |

本次发现并修复的 src 问题：

- 多个 `FORMULA_CALC` 执行器共享同一个 `ActionType` 时，原工厂一对一映射会启动失败，已改为一对多候选执行器。
- `ExecutorCode` 配错时，原管线会让所有公式执行器静默跳过并记录成功，已增加 `CanHandle` 校验，0 个匹配会按 `OnError=STOP` 中断。
- `OnError` 现在按资金安全默认处理：`null`、空字符串和大小写不同的 `stop` 都等价于 `STOP`，避免导入或人工维护时大小写不规范导致公式/限额漏执行后继续收费。
- 多个执行器同时声明能处理同一动作时，现在直接中断，避免重复计价。
- `SameGroupMutexExecutor` 原来按患者 + 互斥组 + 业务日累计，和旧 HIS 的最近 2 小时窗口不一致；已改为 `WindowMinutes`，默认 120。

当前验证状态：

- `src/Pricing.RuleCenter.slnx` 只包含主工程和 `tests/Pricing.RuleCenter.Core.Tests`，当前验证结果为 33 个测试通过。
- `tests/Pricing.RuleCenter.Tests` 是旧测试项目，已通过 `GlobalUsings.cs` 兼容命名空间迁移，并修正旧断言后恢复到 111 个测试通过。
- `FORMULA_CALC` 多执行器二级分派、`ExecutorCode` 未注册中断、重复执行器冲突、同组互斥 2 小时窗口、同手术封顶锁键等关键回归点均已有自动化测试覆盖。

仍需数据边界：

- `字典规则.sql` 未导出 `FIN_DISCOUNT_FEE` 实际行数据，因此本仓库不能写死比例折价项目清单。
- `sql/04-import-rules.sql` 已改为动态导入：运行时当前 Oracle 用户可访问 `FIN_DISCOUNT_FEE` 时，自动读取 `VALID_STATE='1'` 且 `DISCOUNT_TYPE='2'` 的记录，生成 `FORMULA_CALC/INCREMENT_PERCENT`；`TOPPRICE > 0` 时追加 `APPLY_MAX_AMOUNT`。
- 如果目标库没有 `FIN_DISCOUNT_FEE` 源表或表内无有效数据，脚本会输出跳过提示，不会伪造 `DF_` 规则，也不会生成 `APPLY_MIN_AMOUNT`。

## 重构注意点

- `ucDisplay.cs` 是 GBK/ANSI 文件，已按 GBK 安全写回注释，避免转码破坏旧 HIS 工程。
- `Restrictingfee.cs` 方法大量直接修改入参 `FeeItemList`，不是纯函数。
- `Memo` 中的 `P` / `N` 是旧逻辑保存原数量的兼容标记，不能随便删除。
- `hsREOnlyOneItem` 和 `hsREOnlylistItem` 是“先标记旧行、再删除旧行、最后追加重算行”的替换机制。
- `number` 是遍历序号，不是数量；重复项目依靠“项目编码 + number”区分。
