# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

这是一个**物价折价改造项目**的设计与规划仓库。目标是将医院信息系统（HIS）中硬编码的物价/折价规则，改造为可配置、可维护的统一计价规则中心，服务于所有渠道（HIS、自助机、微信）。

项目当前处于**设计阶段**，暂无源代码。仓库包含需求文档、架构设计、数据库表设计草案、接口规范和任务拆解。

## 技术栈

- **运行时：** .NET 6
- **数据库：** Oracle 11g（无原生JSON支持，用CLOB；无自增，用SEQUENCE）
- **ORM：** SqlSugarCore
- **接口：** ASP.NET Core Web API
- **驱动：** Oracle.ManagedDataAccess.Core
- **前端：** HIS系统（C# Windows Forms/WPF）
- **金额类型：** C# 始终使用 `decimal`，Oracle 使用 `NUMBER(18,4)` —— 禁止使用 `double` 或 `float`

## 架构

三大核心组件：

1. **规则维护中心** — 物价规则的增删改查、字典维护、公式定义、规则版本管理与生命周期
2. **统一计价引擎** — 规则匹配、双单位换算、公式执行、金额/数量上下限、折价计算
3. **折价追溯中心** — 规则变更审计、计价请求日志、计算步骤日志、折价结果明细

### 计划项目结构

```
Pricing.RuleCenter.Api           — HTTP接口层
Pricing.RuleCenter.Application   — 用例、事务、编排
Pricing.RuleCenter.Domain        — 规则匹配、公式计算、折价决策
Pricing.RuleCenter.Infrastructure— SqlSugarCore、Oracle、日志、缓存
Pricing.RuleCenter.Contracts     — 请求/响应DTO、枚举、接口契约
```

### 核心设计模式

- **条件-动作分离：** 规则建模为"满足条件时执行动作"（PR_RULE_CONDITION + PR_RULE_ACTION）
- **执行器模式：** `IRuleConditionEvaluator`、`IRuleActionExecutor`、`IPricingFormulaExecutor` —— 新增规则类型只需新增执行器，无需修改各渠道代码
- **三阶段确认：** `confirm`（占用额度）→ `commit`（HIS结算）→ `cancel`（释放额度）
- **可追溯性作为架构原则：** 每笔折价必须可沿三条链路追溯——规则变更链、计算过程链、最终结果链

## 核心业务规则

### 计价计算顺序（强制）

1. 按项目、场景、部位、时间匹配规则
2. 双单位换算（如已配置）
3. 公式计算（如已配置）
4. 金额下限
5. 金额上限
6. 日数量限制
7. 时间窗数量限制（如2小时窗）
8. 同组互斥 / 同手术封顶
9. 子项加收 / 附加项目
10. 超出部分 → 0元（不是整单，不是拒单）

### 关键业务约束

- **全院累计：** 门诊 + 住院合计计数用于限制判断
- **超出 = 0元：** 不是"拒单"，不是"整单归零"，仅超出部分为0元
- **NULL ≠ 0：** NULL 表示"不校验"，0 表示"限制为零"。前端空值必须存为 NULL
- **公式优先于限制：** 公式 + 金额限制共存时，先算公式，再与限制比较
- **公式项目无数量限制** —— 仅受金额限制
- **换算数量固定为1：** `换算数量`是固定值；公式使用换算后数量，非输入数量
- **模拟 vs 确认：** 试算不占用额度，仅确认计价才占用

## 接口端点

| 方法 | 路径 | 用途 |
|------|------|------|
| POST | `/api/pricing/calculate/simulate` | 试算（不占额度） |
| POST | `/api/pricing/calculate/confirm` | 确认计价（占用额度） |
| POST | `/api/pricing/calculate/commit` | HIS结算成功通知 |
| POST | `/api/pricing/calculate/cancel` | 取消确认（释放额度） |
| POST | `/api/pricing/calculate/reverse` | 退费/冲销 |
| GET | `/api/pricing/items/{itemCode}/special-flag` | 查询是否特殊项目 |
| POST | `/api/pricing/trace/query` | 追溯查询 |
| GET | `/api/pricing/rules/effective` | 查询生效规则 |

## 数据库表（PR_前缀）

**规则配置：** PR_RULE_HEADER, PR_RULE_VERSION, PR_RULE_CONDITION, PR_RULE_ACTION, PR_FORMULA_DEF, PR_DICT

**项目分组：** PR_ITEM_GROUP, PR_ITEM_GROUP_DETAIL

**审核与发布：** PR_RULE_PUBLISH, PR_RULE_CHANGE_LOG, PR_RULE_APPROVAL

**计价追溯：** PR_CHARGE_REQUEST_LOG, PR_CHARGE_TRACE_STEP, PR_CHARGE_DISCOUNT_DETAIL

**并发控制：** PR_LIMIT_OCCUPY, PR_LIMIT_LOCK（SELECT ... FOR UPDATE 模式）

**冲销：** PR_CHARGE_REVERSE_LOG

**测试：** PR_RULE_TEST_CASE, PR_RULE_TEST_RUN

## 文档索引

需求与参考资料位于仓库根目录和 `docs/`：

| 文件 | 用途 |
|------|------|
| `需求描述.txt` | 原始需求简述 |
| `物价界面改造1——标识及上下限.txt` | 原始需求提出折价页签、标识、数量/金额限制 |
| `物价界面改造2——计价公式.txt` | 公式计价、公式与限制的交互 |
| `物价界面改造3——双单位.txt` | 双单位换算 |
| `docs/物价折价规则整理.xlsx` | 74条历史规则，需迁移 |
| `docs/医保立项指南服务改造方案.pptx` | 更广泛的业务背景 |

设计文档在 `docs/物价折价改造方案文档/`，按 01-09 顺序阅读。最低必读：01、02、03、04、05、08、09。

特别注意：
- `08-设计缺陷与补充方案.md` — 25 个设计缺陷与补充方案，5 个 P0 级问题必须在编码前解决
- `04-Oracle表设计草案.md` — 已合并 08 的补充方案（LIMIT_KEY 规则、互斥组、版本一致性、滑动窗口等）
- `05-统一计价接口设计.md` — 已合并 08 的补充方案（批量试算、分页、超时降级、SLA 等）
- `09-HIS集成与部署方案.md` — HIS 特殊计价规则维护工作台、收费弹窗、灰度发布、性能容量、上线检查清单

## 稳定性要求（金融级）

- **幂等性：** confirm 接口必须幂等（键：`sourceSystem + requestNo`）
- **并发额度：** PR_LIMIT_LOCK + SELECT FOR UPDATE 防止多渠道限额突破
- **取整策略：** 必须统一且可配置（roundMode、roundScale、minChargeQty、ceilBaseQty）
- **特殊项目：** 计价服务不可用时，渠道不得回退为普通计价
- **缓存失效：** 规则发布/停用/回滚时必须立即失效缓存
- **挂起清理：** 后台任务必须扫描长时间处于 CONFIRM_PENDING 状态的记录

## 实施优先级

1. 数据模型设计（规则表 + 追溯表）
2. 工作台内置字典维护（计价类型、单位、公式类型）
3. HIS 特殊计价规则维护工作台
4. 物价计算服务（核心引擎）
5. 追溯与审计
6. 收费录入集成（特殊项目弹窗）
7. 历史规则迁移（xlsx中74条规则）
8. 每日对账与异常报表
9. 测试与部署

## 待确认问题（实现前必须确认）

- "单次"的定义 —— 单行项目、单次动作、单次手术、还是单次执行批次？
- "折价项目"和"特殊项目"是否为同一标识
- 一个项目是否可以有多条规则（数据模型必须支持，无论业务答案如何）
- 退费/作废是否释放额度，以及释放哪个时间窗的额度
- 中间金额与最终金额的取整精度
- "同手术""同孕次"标识在现有系统中的来源
