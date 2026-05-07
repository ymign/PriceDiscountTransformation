# Oracle 表设计草案

## 1. 设计目标

本文给出“统一计价规则中心”的 Oracle 11g 表设计草案，服务于以下目标：

- 支持国家、省市物价规则频繁调整。
- 支持折价、封顶、公式、双单位、时间窗口、同组互斥、主子项目加收等规则。
- 支持一项目多规则、一规则多条件、一规则多动作。
- 支持规则版本化、发布、回滚。
- 支持每一次折价计算完整回查。
- 适配 `.NET 6 + SqlSugarCore + Oracle 11g`。

## 2. 核心设计原则

### 2.1 不把规则全部做成固定字段

国家物价规则变化非常不稳定，如果只设计以下字段：

- 单日上限数量。
- 单次上限数量。
- 单日上限金额。
- 公式类型。

短期能用，但遇到“同组互斥”“同一手术封顶”“部位差异”“主子项目比例加收”时很快不够。

因此建议采用：

- 规则头表。
- 规则条件表。
- 规则动作表。
- 规则参数表。
- 规则版本表。
- 规则日志表。

这种设计比单纯扩字段更能承载乱规则。

### 2.2 条件和动作拆开

一条规则可以理解为：

> 当满足某些条件时，执行某些动作。

例如：

- 条件：项目为 CT 平扫，2 小时内，同患者，同部位组。
- 动作：超过 3 个部位后，超出部分折价为 0。

又例如：

- 条件：项目为浅表肿物去除费，数量大于 1。
- 动作：按 50% 递增公式计算。

因此条件和动作应拆表，不要混在一个字段里。

### 2.3 固定列 + 参数扩展并存

高频检索字段必须单独成列：

- 项目编码。
- 患者。
- 收费时间。
- 规则版本。
- 来源系统。
- 收费单号。

低频变化字段可以放扩展参数：

- 公式参数。
- 条件参数。
- 动作参数。
- 输入输出快照。

Oracle 11g 没有原生 JSON 类型，扩展参数建议用 `CLOB` 存 JSON 字符串。

## 3. 命名约定

建议统一前缀：

- `PR_`：Pricing Rule。

主键建议：

- 所有主键用 `NUMBER(18,0)`。
- 每张表配一个 `SEQUENCE`。
- 业务编码使用 `VARCHAR2`。

时间字段建议：

- Oracle 使用 `DATE`。
- 如需要毫秒级，可使用 `TIMESTAMP`，但 HIS 老系统兼容性要确认。

是否字段建议：

- 使用 `CHAR(1)`。
- `Y` 表示是。
- `N` 表示否。

## 4. 规则配置表

## 4.1 规则头表 `PR_RULE_HEADER`

用途：

- 保存一条规则的主信息。
- 一条规则可以对应一个项目，也可以对应一个项目组。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `RULE_ID` | `NUMBER(18,0)` | 主键 |
| `RULE_CODE` | `VARCHAR2(50)` | 规则编码 |
| `RULE_NAME` | `VARCHAR2(200)` | 规则名称 |
| `RULE_CATEGORY` | `VARCHAR2(30)` | 规则类别，如 DISCOUNT、FORMULA、LIMIT、MIXED |
| `RULE_SCOPE` | `VARCHAR2(30)` | 规则范围，如 ITEM、GROUP、SCENE |
| `ITEM_CODE` | `VARCHAR2(50)` | 项目编码，可为空 |
| `ITEM_NAME` | `VARCHAR2(200)` | 项目名称 |
| `GROUP_CODE` | `VARCHAR2(50)` | 项目组编码，可为空 |
| `PRIORITY` | `NUMBER(10,0)` | 优先级，数字越小优先级越高 |
| `CURRENT_VERSION` | `NUMBER(10,0)` | 当前版本号 |
| `STATUS` | `VARCHAR2(20)` | DRAFT、PUBLISHED、DISABLED |
| `IS_ENABLED` | `CHAR(1)` | 是否启用 |
| `EFFECTIVE_FROM` | `DATE` | 生效时间 |
| `EFFECTIVE_TO` | `DATE` | 失效时间 |
| `REMARK` | `VARCHAR2(1000)` | 备注 |
| `CREATED_BY` | `VARCHAR2(50)` | 创建人 |
| `CREATED_AT` | `DATE` | 创建时间 |
| `UPDATED_BY` | `VARCHAR2(50)` | 更新人 |
| `UPDATED_AT` | `DATE` | 更新时间 |

建议索引：

- `IDX_PR_RULE_HEADER_ITEM`：`ITEM_CODE, IS_ENABLED, STATUS`
- `IDX_PR_RULE_HEADER_GROUP`：`GROUP_CODE, IS_ENABLED, STATUS`
- `IDX_PR_RULE_HEADER_EFFECT`：`EFFECTIVE_FROM, EFFECTIVE_TO`

## 4.2 规则版本表 `PR_RULE_VERSION`

用途：

- 保存每次发布后的规则版本。
- 历史收费必须关联当时命中的版本。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `VERSION_ID` | `NUMBER(18,0)` | 主键 |
| `RULE_ID` | `NUMBER(18,0)` | 规则 ID |
| `VERSION_NO` | `NUMBER(10,0)` | 版本号 |
| `VERSION_STATUS` | `VARCHAR2(20)` | DRAFT、PUBLISHED、ROLLED_BACK、DISABLED |
| `EFFECTIVE_FROM` | `DATE` | 生效时间 |
| `EFFECTIVE_TO` | `DATE` | 失效时间 |
| `RULE_SNAPSHOT` | `CLOB` | 规则完整快照 |
| `PUBLISHED_BY` | `VARCHAR2(50)` | 发布人 |
| `PUBLISHED_AT` | `DATE` | 发布时间 |
| `PUBLISH_REMARK` | `VARCHAR2(1000)` | 发布说明 |

建议索引：

- `IDX_PR_RULE_VERSION_RULE`：`RULE_ID, VERSION_NO`
- `IDX_PR_RULE_VERSION_EFFECT`：`RULE_ID, EFFECTIVE_FROM, EFFECTIVE_TO`

## 4.3 规则条件表 `PR_RULE_CONDITION`

用途：

- 保存规则触发条件。
- 一条规则可以有多条条件。

示例条件：

- 患者类型为儿童。
- 收费场景为门诊。
- 项目属于某个组。
- 2 小时内累计。
- 同一手术。
- 同一胎次。
- 部位为头面部。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `CONDITION_ID` | `NUMBER(18,0)` | 主键 |
| `RULE_ID` | `NUMBER(18,0)` | 规则 ID |
| `VERSION_NO` | `NUMBER(10,0)` | 规则版本 |
| `CONDITION_GROUP` | `VARCHAR2(50)` | 条件组 |
| `CONDITION_TYPE` | `VARCHAR2(50)` | 条件类型 |
| `OPERATOR_TYPE` | `VARCHAR2(20)` | EQ、NE、GT、GE、LT、LE、IN、BETWEEN |
| `LEFT_KEY` | `VARCHAR2(100)` | 左值，如 ITEM_CODE、BODY_PART、AGE |
| `RIGHT_VALUE` | `VARCHAR2(1000)` | 右值 |
| `PARAMS_JSON` | `CLOB` | 扩展参数 |
| `SORT_NO` | `NUMBER(10,0)` | 排序 |
| `IS_ENABLED` | `CHAR(1)` | 是否启用 |

常见 `CONDITION_TYPE` 建议：

- `ITEM_MATCH`
- `GROUP_MATCH`
- `VISIT_TYPE_MATCH`
- `CHARGE_SCENE_MATCH`
- `BODY_PART_MATCH`
- `TIME_WINDOW`
- `SAME_DAY`
- `SAME_OPERATION`
- `SAME_PREGNANCY`
- `AGE_MATCH`

## 4.4 规则动作表 `PR_RULE_ACTION`

用途：

- 保存命中规则后要执行的动作。
- 一条规则可以有多个动作，按顺序执行。

示例动作：

- 单位换算。
- 执行公式。
- 应用金额上限。
- 应用金额下限。
- 超出数量折价为 0。
- 生成加收子项目。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `ACTION_ID` | `NUMBER(18,0)` | 主键 |
| `RULE_ID` | `NUMBER(18,0)` | 规则 ID |
| `VERSION_NO` | `NUMBER(10,0)` | 规则版本 |
| `ACTION_TYPE` | `VARCHAR2(50)` | 动作类型 |
| `EXECUTOR_CODE` | `VARCHAR2(50)` | 执行器编码 |
| `PARAMS_JSON` | `CLOB` | 动作参数 |
| `SORT_NO` | `NUMBER(10,0)` | 执行顺序 |
| `ON_ERROR` | `VARCHAR2(20)` | STOP、SKIP、WARN |
| `IS_ENABLED` | `CHAR(1)` | 是否启用 |

常见 `ACTION_TYPE` 建议：

- `CONVERT_QTY`
- `FORMULA_CALC`
- `APPLY_MAX_AMOUNT`
- `APPLY_MIN_AMOUNT`
- `APPLY_DAY_LIMIT_QTY`
- `APPLY_ONCE_LIMIT_QTY`
- `APPLY_TIME_WINDOW_LIMIT`
- `DISCOUNT_EXCEED_TO_ZERO`
- `ADD_CHILD_ITEM`
- `ROUND_QTY`

## 4.5 公式定义表 `PR_FORMULA_DEF`

用途：

- 定义系统支持的受控公式。
- 不建议让业务直接写任意脚本。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `FORMULA_ID` | `NUMBER(18,0)` | 主键 |
| `FORMULA_CODE` | `VARCHAR2(50)` | 公式编码 |
| `FORMULA_NAME` | `VARCHAR2(200)` | 公式名称 |
| `FORMULA_DESC` | `VARCHAR2(1000)` | 业务描述 |
| `EXECUTOR_CODE` | `VARCHAR2(50)` | 对应 C# 执行器 |
| `PARAM_SCHEMA_JSON` | `CLOB` | 参数结构说明 |
| `IS_ENABLED` | `CHAR(1)` | 是否启用 |
| `REMARK` | `VARCHAR2(1000)` | 备注 |

建议先支持公式：

- `UNIT_PRICE_TIMES_QTY`
- `INCREMENT_50_PERCENT`
- `INCREMENT_70_PERCENT`
- `UNIT_PRICE_QTY_PLUS_BASE`
- `AREA_STEP_PERCENT`
- `MAIN_ITEM_PERCENT`
- `CEILING_AMOUNT`

## 4.6 字典表 `PR_DICT`

用途：

- 维护计价类型、计价单位、计价部位、规则类别等字典。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `DICT_ID` | `NUMBER(18,0)` | 主键 |
| `DICT_TYPE` | `VARCHAR2(50)` | 字典类型 |
| `DICT_CODE` | `VARCHAR2(50)` | 字典编码 |
| `DICT_NAME` | `VARCHAR2(200)` | 字典名称 |
| `SORT_NO` | `NUMBER(10,0)` | 排序 |
| `IS_ENABLED` | `CHAR(1)` | 是否启用 |
| `REMARK` | `VARCHAR2(500)` | 备注 |

## 5. 项目组与主子项目表

## 5.1 项目组表 `PR_ITEM_GROUP`

用途：

- 支持同组互斥、同胎次项目封顶、复合项目替换加收。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `GROUP_ID` | `NUMBER(18,0)` | 主键 |
| `GROUP_CODE` | `VARCHAR2(50)` | 项目组编码 |
| `GROUP_NAME` | `VARCHAR2(200)` | 项目组名称 |
| `GROUP_TYPE` | `VARCHAR2(50)` | MUTEX、ADDON、REPLACE、LIMIT |
| `IS_ENABLED` | `CHAR(1)` | 是否启用 |
| `REMARK` | `VARCHAR2(1000)` | 备注 |

## 5.2 项目组明细表 `PR_ITEM_GROUP_DETAIL`

用途：

- 保存项目组内项目。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `DETAIL_ID` | `NUMBER(18,0)` | 主键 |
| `GROUP_ID` | `NUMBER(18,0)` | 项目组 ID |
| `ITEM_CODE` | `VARCHAR2(50)` | 项目编码 |
| `ITEM_NAME` | `VARCHAR2(200)` | 项目名称 |
| `ROLE_TYPE` | `VARCHAR2(50)` | MAIN、ADDON、REPLACE_TARGET |
| `SORT_NO` | `NUMBER(10,0)` | 排序 |
| `IS_ENABLED` | `CHAR(1)` | 是否启用 |

## 6. 规则发布与审计表

## 6.1 规则发布表 `PR_RULE_PUBLISH`

用途：

- 记录每次发布、停用、回滚行为。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `PUBLISH_ID` | `NUMBER(18,0)` | 主键 |
| `PUBLISH_NO` | `VARCHAR2(50)` | 发布流水号 |
| `RULE_ID` | `NUMBER(18,0)` | 规则 ID |
| `FROM_VERSION` | `NUMBER(10,0)` | 原版本 |
| `TO_VERSION` | `NUMBER(10,0)` | 新版本 |
| `ACTION_TYPE` | `VARCHAR2(30)` | PUBLISH、DISABLE、ROLLBACK |
| `PUBLISHED_BY` | `VARCHAR2(50)` | 操作人 |
| `PUBLISHED_AT` | `DATE` | 操作时间 |
| `REMARK` | `VARCHAR2(1000)` | 说明 |

## 6.2 规则变更日志表 `PR_RULE_CHANGE_LOG`

用途：

- 回答“规则为什么变了、谁改的、改了什么”。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `CHANGE_ID` | `NUMBER(18,0)` | 主键 |
| `RULE_ID` | `NUMBER(18,0)` | 规则 ID |
| `VERSION_NO` | `NUMBER(10,0)` | 版本号 |
| `CHANGE_TYPE` | `VARCHAR2(30)` | CREATE、UPDATE、PUBLISH、DISABLE、ROLLBACK |
| `CHANGE_SUMMARY` | `VARCHAR2(1000)` | 变更摘要 |
| `BEFORE_SNAPSHOT` | `CLOB` | 变更前快照 |
| `AFTER_SNAPSHOT` | `CLOB` | 变更后快照 |
| `CHANGED_BY` | `VARCHAR2(50)` | 操作人 |
| `CHANGED_AT` | `DATE` | 操作时间 |
| `SOURCE_SYSTEM` | `VARCHAR2(50)` | 来源系统 |

## 7. 计价追溯表

## 7.1 计价请求日志表 `PR_CHARGE_REQUEST_LOG`

用途：

- 记录每一次试算或确认计价请求。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `REQUEST_ID` | `NUMBER(18,0)` | 主键 |
| `REQUEST_NO` | `VARCHAR2(50)` | 请求流水号 |
| `TRACE_ID` | `VARCHAR2(50)` | 追踪号 |
| `CALL_TYPE` | `VARCHAR2(20)` | SIMULATE、CONFIRM、REVERSE |
| `SOURCE_SYSTEM` | `VARCHAR2(50)` | HIS、SELF_MACHINE、WECHAT |
| `SOURCE_TERMINAL` | `VARCHAR2(100)` | 终端或服务标识 |
| `PATIENT_ID` | `VARCHAR2(50)` | 患者 ID |
| `VISIT_ID` | `VARCHAR2(50)` | 就诊 ID |
| `CHARGE_SCENE` | `VARCHAR2(50)` | 收费场景 |
| `CHARGE_NO` | `VARCHAR2(50)` | 收费单号 |
| `ITEM_CODE` | `VARCHAR2(50)` | 项目编码 |
| `ITEM_NAME` | `VARCHAR2(200)` | 项目名称 |
| `INPUT_QTY` | `NUMBER(18,4)` | 原始数量 |
| `INPUT_UNIT` | `VARCHAR2(50)` | 原始单位 |
| `BODY_PART_CODE` | `VARCHAR2(50)` | 部位 |
| `REQUEST_JSON` | `CLOB` | 请求快照 |
| `RESPONSE_JSON` | `CLOB` | 响应快照 |
| `REQUEST_AT` | `DATE` | 请求时间 |
| `RESPONSE_AT` | `DATE` | 响应时间 |
| `IS_SUCCESS` | `CHAR(1)` | 是否成功 |
| `ERROR_MESSAGE` | `VARCHAR2(2000)` | 错误信息 |

建议索引：

- `IDX_PR_REQ_TRACE`：`TRACE_ID`
- `IDX_PR_REQ_PATIENT`：`PATIENT_ID, REQUEST_AT`
- `IDX_PR_REQ_CHARGE`：`CHARGE_NO, ITEM_CODE`

## 7.2 计价步骤日志表 `PR_CHARGE_TRACE_STEP`

用途：

- 按步骤记录计算过程。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `STEP_ID` | `NUMBER(18,0)` | 主键 |
| `REQUEST_ID` | `NUMBER(18,0)` | 请求 ID |
| `TRACE_ID` | `VARCHAR2(50)` | 追踪号 |
| `STEP_NO` | `NUMBER(10,0)` | 步骤号 |
| `STEP_NAME` | `VARCHAR2(100)` | 步骤名称 |
| `STEP_TYPE` | `VARCHAR2(50)` | MATCH、CONVERT、FORMULA、LIMIT、DISCOUNT |
| `RULE_ID` | `NUMBER(18,0)` | 命中规则 |
| `RULE_VERSION_NO` | `NUMBER(10,0)` | 命中版本 |
| `INPUT_SNAPSHOT` | `CLOB` | 步骤输入 |
| `OUTPUT_SNAPSHOT` | `CLOB` | 步骤输出 |
| `STEP_DESC` | `VARCHAR2(2000)` | 步骤说明 |
| `CREATED_AT` | `DATE` | 创建时间 |

## 7.3 折价结果明细表 `PR_CHARGE_DISCOUNT_DETAIL`

用途：

- 记录最终发生折价、封顶、公式调整、金额调整的结果。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `DISCOUNT_ID` | `NUMBER(18,0)` | 主键 |
| `REQUEST_ID` | `NUMBER(18,0)` | 请求 ID |
| `TRACE_ID` | `VARCHAR2(50)` | 追踪号 |
| `CHARGE_NO` | `VARCHAR2(50)` | 收费单号 |
| `CHARGE_DETAIL_NO` | `VARCHAR2(50)` | 收费明细号 |
| `PATIENT_ID` | `VARCHAR2(50)` | 患者 ID |
| `VISIT_ID` | `VARCHAR2(50)` | 就诊 ID |
| `ITEM_CODE` | `VARCHAR2(50)` | 项目编码 |
| `ITEM_NAME` | `VARCHAR2(200)` | 项目名称 |
| `RULE_ID` | `NUMBER(18,0)` | 命中规则 |
| `RULE_VERSION_NO` | `NUMBER(10,0)` | 命中版本 |
| `DISCOUNT_TYPE` | `VARCHAR2(50)` | 折价类型 |
| `ORIGINAL_QTY` | `NUMBER(18,4)` | 原始数量 |
| `CONVERTED_QTY` | `NUMBER(18,4)` | 换算后数量 |
| `FINAL_QTY` | `NUMBER(18,4)` | 最终收费数量 |
| `ORIGINAL_AMT` | `NUMBER(18,4)` | 原始金额 |
| `CALCULATED_AMT` | `NUMBER(18,4)` | 计算金额 |
| `FINAL_AMT` | `NUMBER(18,4)` | 最终金额 |
| `DISCOUNT_AMT` | `NUMBER(18,4)` | 折价金额 |
| `REASON_CODE` | `VARCHAR2(50)` | 原因编码 |
| `REASON_DESC` | `VARCHAR2(2000)` | 原因描述 |
| `LIMIT_BASE_INFO` | `CLOB` | 累计依据 |
| `OCCURRED_AT` | `DATE` | 发生时间 |
| `CONFIRMED_BY` | `VARCHAR2(50)` | 确认人 |

建议索引：

- `IDX_PR_DISC_PATIENT`：`PATIENT_ID, OCCURRED_AT`
- `IDX_PR_DISC_CHARGE`：`CHARGE_NO, CHARGE_DETAIL_NO`
- `IDX_PR_DISC_ITEM`：`ITEM_CODE, OCCURRED_AT`

## 8. 冲正与退费表

## 8.1 限额占用表 `PR_LIMIT_OCCUPY`

用途：

- 防止 HIS、自助机、微信公众号等多渠道并发收费时突破全院上限。
- 记录一次确认计价对单日、单次、2 小时窗口等额度的占用。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `OCCUPY_ID` | `NUMBER(18,0)` | 主键 |
| `REQUEST_ID` | `NUMBER(18,0)` | 请求 ID |
| `TRACE_ID` | `VARCHAR2(50)` | 追踪号 |
| `PATIENT_ID` | `VARCHAR2(50)` | 患者 ID |
| `ITEM_CODE` | `VARCHAR2(50)` | 项目编码 |
| `RULE_ID` | `NUMBER(18,0)` | 规则 ID |
| `RULE_VERSION_NO` | `NUMBER(10,0)` | 规则版本 |
| `LIMIT_TYPE` | `VARCHAR2(50)` | DAY_QTY、ONCE_QTY、TIME_WINDOW、DAY_AMOUNT |
| `LIMIT_KEY` | `VARCHAR2(200)` | 限额维度键 |
| `OCCUPY_QTY` | `NUMBER(18,4)` | 占用数量 |
| `OCCUPY_AMT` | `NUMBER(18,4)` | 占用金额 |
| `STATUS` | `VARCHAR2(20)` | PENDING、CONFIRMED、CANCELLED、REVERSED、EXPIRED |
| `OCCUPIED_AT` | `DATE` | 占用时间 |
| `CONFIRMED_AT` | `DATE` | 确认时间 |
| `EXPIRE_AT` | `DATE` | 过期时间 |

## 8.2 限额锁表 `PR_LIMIT_LOCK`

用途：

- 为全院累计限制提供数据库级锁维度。
- 确认计价时按患者、项目、日期、时间窗口构造锁键，并在事务中锁定。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `LOCK_KEY` | `VARCHAR2(200)` | 主键 |
| `LOCK_DESC` | `VARCHAR2(500)` | 锁说明 |
| `UPDATED_AT` | `DATE` | 更新时间 |

说明：

- 例如单日限制锁键可以是 `DAY:P001:CT001:20260507`。
- 事务中使用 `SELECT ... FOR UPDATE` 锁住该行，再计算剩余额度。
- 试算不锁定、不占额。

## 8.3 计价冲正表 `PR_CHARGE_REVERSE_LOG`

用途：

- 记录退费、作废、撤销时对折价累计口径的影响。

建议字段：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `REVERSE_ID` | `NUMBER(18,0)` | 主键 |
| `ORIGINAL_REQUEST_ID` | `NUMBER(18,0)` | 原请求 ID |
| `REVERSE_REQUEST_ID` | `NUMBER(18,0)` | 冲正请求 ID |
| `CHARGE_NO` | `VARCHAR2(50)` | 原收费单号 |
| `REVERSE_NO` | `VARCHAR2(50)` | 冲正单号 |
| `ITEM_CODE` | `VARCHAR2(50)` | 项目编码 |
| `REVERSE_QTY` | `NUMBER(18,4)` | 冲正数量 |
| `REVERSE_AMT` | `NUMBER(18,4)` | 冲正金额 |
| `REVERSE_REASON` | `VARCHAR2(1000)` | 冲正原因 |
| `REVERSED_BY` | `VARCHAR2(50)` | 操作人 |
| `REVERSED_AT` | `DATE` | 操作时间 |

说明：

- 全院累计规则必须明确退费是否释放额度。
- 这张表用于后续追查“为什么退费后又能收费”。

## 9. SqlSugarCore 实体映射建议

### 9.1 主键与序列

Oracle 11g 没有自增主键，建议使用序列。

SqlSugarCore 中可以通过插入前取序列值，或封装统一 `IIdGenerator`。

建议序列命名：

- `SEQ_PR_RULE_HEADER`
- `SEQ_PR_RULE_VERSION`
- `SEQ_PR_RULE_CONDITION`
- `SEQ_PR_RULE_ACTION`
- `SEQ_PR_CHARGE_REQUEST_LOG`
- `SEQ_PR_CHARGE_TRACE_STEP`
- `SEQ_PR_CHARGE_DISCOUNT_DETAIL`

### 9.2 CLOB 字段

以下字段建议映射为 `string`：

- `RULE_SNAPSHOT`
- `PARAMS_JSON`
- `REQUEST_JSON`
- `RESPONSE_JSON`
- `INPUT_SNAPSHOT`
- `OUTPUT_SNAPSHOT`
- `LIMIT_BASE_INFO`

注意：

- 不要把需要高频检索的字段只存到 CLOB。
- CLOB 只用于快照、参数、解释性信息。

### 9.3 金额精度

建议金额统一使用：

- `NUMBER(18,4)`

C# 中使用：

- `decimal`

不要使用：

- `double`
- `float`

## 10. 扩展性设计说明

## 10.1 新增一种条件

如果未来新增规则：

> 节假日夜间收费规则不同。

可以新增：

- `CONDITION_TYPE = HOLIDAY_NIGHT`
- `PARAMS_JSON` 存节假日、时间段参数。
- C# 新增一个 `IRuleConditionEvaluator` 实现。

不需要改主表字段。

## 10.2 新增一种动作

如果未来新增规则：

> 超过部分按 30% 收费，而不是 0 元。

可以新增：

- `ACTION_TYPE = DISCOUNT_EXCEED_BY_RATE`
- `PARAMS_JSON = {"rate":0.3}`
- C# 新增一个动作执行器。

不需要改已有规则表结构。

## 10.3 新增一种公式

如果未来新增规则：

> 前 3 个按原价，之后每个按 20%。

可以新增：

- `FORMULA_CODE = FIRST_N_FULL_THEN_RATE`
- `PARAM_SCHEMA_JSON = {"fullQty":3,"rate":0.2}`
- C# 新增公式执行器。

不需要让业务写脚本，也不需要让 HIS、自助机、公众号改自己的代码。

## 11. 最小落地版本建议

第一阶段可以先落以下表：

- `PR_RULE_HEADER`
- `PR_RULE_VERSION`
- `PR_RULE_CONDITION`
- `PR_RULE_ACTION`
- `PR_FORMULA_DEF`
- `PR_DICT`
- `PR_RULE_CHANGE_LOG`
- `PR_CHARGE_REQUEST_LOG`
- `PR_CHARGE_TRACE_STEP`
- `PR_CHARGE_DISCOUNT_DETAIL`
- `PR_LIMIT_OCCUPY`
- `PR_LIMIT_LOCK`

后续再补：

- `PR_ITEM_GROUP`
- `PR_ITEM_GROUP_DETAIL`
- `PR_CHARGE_REVERSE_LOG`

## 12. 结论

如果希望长期适应“国家规则乱调整、规则乱七八糟”的现实，表结构不能只围绕当前几个字段设计。

推荐用：

- 规则头。
- 规则版本。
- 规则条件。
- 规则动作。
- 公式定义。
- 参数快照。
- 计算追溯。

这种模型能让大部分新规则通过“新增配置 + 新增执行器”解决，而不是继续在 HIS、自助机、公众号里到处改代码。
