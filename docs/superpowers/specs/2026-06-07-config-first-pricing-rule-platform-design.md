# 配置优先计价规则平台设计

## 前提

当前 `src/` 中的新架构代码仍处于开发阶段，尚未正式投产。

这意味着本次平台重构的默认前提不是“兼容现有作者模型”，而是：

- 保留已经验证有价值的计价执行内核和资金安全约束
- 推翻不合理的作者模型、发布模型和运行时装载入口
- 不为旧 `Rule + Condition + Action` 人工维护模式继续背书

本次设计只保留两类兼容性：

1. 计价业务语义兼容
2. 已有执行器、计价流程、限额/追溯/冲销核心链路尽量复用

除此之外，表结构、服务边界、控制器和工作台都可以调整。

## 目标

1. 让物价科维护“业务策略”，而不是技术规则明细。
2. 让运行时只读取已激活规则包，不读取草稿和作者表。
3. 把发布对象从“单条规则”升级为“候选包构建 + 激活指针切换”。
4. 在不重写执行器体系的前提下，重建作者模型、编译模型和运行时读模型。
5. 为后续扩展场景覆盖、科室覆盖、强表达式留结构，不在第一期一次性放开。

## 非目标

- 第一阶段不保留旧规则维护界面的原样交互。
- 第一阶段不向物价科开放强表达式。
- 第一阶段不做作者模型与旧 `PR_RULE_*` 表的双写。
- 第一阶段不追求万能 DSL，只覆盖高频规则族。
- 不再允许业务人员直接维护运行时 `Rule/Condition/Action` 明细。

## 设计原则

### 原则 1：作者模型与运行时模型彻底分离

- `Template` 负责描述规则骨架
- `Policy` 负责描述院内策略
- `Runtime Package` 负责描述可执行编译产物

运行时禁止直接读取 `Template` 和 `Policy`。

### 原则 2：规则包是唯一运行时入口

发布不再让单条策略直接生效。

统一流程改为：

1. 保存/校验策略版本
2. 可选审批
3. 构建候选运行时包
4. 差异预览
5. 原子激活包指针

运行时永远只识别“当前激活包”。

### 原则 3：不做旧作者模型双写

旧 `PR_RULE_HEADER / PR_RULE_VERSION / PR_RULE_CONDITION / PR_RULE_ACTION` 不再作为新平台作者模型。

第一期策略是：

- 旧表只作为迁移输入和历史参考
- 新平台只写新三模型表和运行时包表
- 运行时读取新包，不再从旧规则表取候选规则

### 原则 4：保留执行内核，重建配置内核

保留并复用：

- `PricingEngine`
- `ActionExecutionPipeline`
- 现有 `IRuleActionExecutor`
- 现有受控弱表达式能力
- `simulate / confirm / commit / cancel / reverse`
- 限额占用、请求日志、追溯、冲销主链路

重建：

- 模板/策略作者模型
- 冲突检测
- 发布治理
- 运行时读模型
- 工作台交互模型
- 追溯中的来源标识

### 原则 5：运行时契约优先于存储细节

第一期不要求执行内核立即彻底改名，但必须先引入一个新的运行时契约层。

运行时最小契约应是：

- `IRuntimeRuleReadRepository`
- `ActiveRuntimePackageReader`
- `RuntimeRuleSelector`
- `RuntimeRuleProjectionAdapter`

也就是说，即使底层存的是 `PR_RUNTIME_RULE / CONDITION / ACTION`，引擎也不应该继续假设自己面对的是作者模型。

## 总体架构

### 1. 模板层

模板层面向程序员和高权限管理员，负责定义：

- 模板编码、名称、类别、风险等级
- 参数定义
- 步骤骨架
- 允许开放的作用域维度
- 能力族 `CapabilityFamily`
- 合并模式 `MergeMode`
- 表达式权限

模板只描述“这类规则怎么长”，不绑定院内项目。

### 2. 策略层

策略层面向物价科，是业务真正维护的对象。

策略维护内容只有：

- 选择模板
- 绑定项目或项目组
- 填参数
- 设生效时间
- 设发布模式
- 设适用范围

策略层不暴露：

- `ActionType`
- `ExecutorCode`
- 原始 `ParamsJson`
- 运行时动作排序细节

### 3. 编译层

编译层把 `Template + PolicyVersion` 转成运行时包，负责：

- 参数校验
- 表达式静态校验
- 绑定与作用域归一化
- 能力族优先级计算
- 冲突检测
- 运行时投影生成
- 候选包构建
- 包差异比较
- 包激活与回滚

### 4. 运行时层

运行时只做三件事：

1. 读取当前激活包
2. 根据计价上下文选出命中能力
3. 将命中的动作链送入现有执行内核

运行时不能读取草稿策略，不能直接查询作者表。

## 新的单一事实来源

### 第一阶段事实来源

- 模板定义：`PR_TEMPLATE*`
- 策略定义：`PR_POLICY*`
- 运行时事实：`PR_RUNTIME_PACKAGE*`

### 旧规则表定位

下列表在新平台完成切换后不再是作者入口：

- `PR_RULE_HEADER`
- `PR_RULE_VERSION`
- `PR_RULE_CONDITION`
- `PR_RULE_ACTION`
- `PR_RULE_APPROVAL`
- `PR_RULE_PUBLISH`

它们的定位改为：

- 迁移输入
- 历史参考
- 过渡期调试样本

### 切换策略

- 不做模板/策略到旧规则表的双写
- 不做运行时包到旧规则表的镜像回写
- 编译产物直接进入新运行时表
- 现有计价入口改造为读取新运行时读模型

## 数据模型

## 一、模板模型

### `PR_TEMPLATE`

模板主表，核心字段：

- `TEMPLATE_ID`
- `TEMPLATE_CODE`
- `TEMPLATE_NAME`
- `CATEGORY`
- `RISK_LEVEL`
- `EXPRESSION_MODE`
- `STATUS`
- `CURRENT_VERSION_NO`
- `CREATED_BY`
- `CREATED_AT`
- `UPDATED_BY`
- `UPDATED_AT`

### `PR_TEMPLATE_VERSION`

模板版本表，核心字段：

- `TEMPLATE_VERSION_ID`
- `TEMPLATE_ID`
- `VERSION_NO`
- `VERSION_STATUS`
- `CAPABILITY_FAMILY`
- `MERGE_MODE`
- `CHECKSUM`
- `DESCRIPTION`
- `PUBLISHED_BY`
- `PUBLISHED_AT`

### `PR_TEMPLATE_PARAM_DEF`

模板参数定义表，核心字段：

- `PARAM_DEF_ID`
- `TEMPLATE_VERSION_ID`
- `PARAM_CODE`
- `PARAM_NAME`
- `VALUE_TYPE`
- `IS_REQUIRED`
- `DEFAULT_TEXT`
- `DEFAULT_NUMBER`
- `DEFAULT_BOOL`
- `DICT_TYPE`
- `MIN_VALUE`
- `MAX_VALUE`
- `REGEX_RULE`
- `UI_CONTROL`
- `HELP_TEXT`
- `RISK_FLAG`
- `SORT_NO`

### `PR_TEMPLATE_STEP_DEF`

模板步骤骨架表，核心字段：

- `STEP_DEF_ID`
- `TEMPLATE_VERSION_ID`
- `STEP_NO`
- `STEP_KIND`
- `CAPABILITY_CODE`
- `ACTION_TYPE`
- `EXECUTOR_CODE`
- `ON_ERROR`
- `STEP_CONFIG_CLOB`

说明：

- `STEP_KIND` 用于区分 `CONDITION / ACTION / META`
- `STEP_CONFIG_CLOB` 只允许模板侧维护
- 运行时不直接消费该表

### `PR_TEMPLATE_SCOPE_DEF`

模板允许开放的作用域定义：

- `SCOPE_DEF_ID`
- `TEMPLATE_VERSION_ID`
- `SCOPE_DIMENSION`
- `IS_REQUIRED`
- `ALLOW_MULTIPLE`
- `SORT_NO`

第一期支持的标准作用域维度：

- `ITEM`
- `GROUP`
- `SCENE`
- `DEPT`
- `BODY_PART`
- `VISIT_TYPE`
- `TIME_RANGE`

## 二、策略模型

### `PR_POLICY`

策略主表，核心字段：

- `POLICY_ID`
- `POLICY_CODE`
- `POLICY_NAME`
- `TEMPLATE_ID`
- `OWNER_TYPE`
- `PUBLISH_PROFILE`
- `STATUS`
- `CURRENT_VERSION_NO`

### `PR_POLICY_VERSION`

策略版本表，核心字段：

- `POLICY_VERSION_ID`
- `POLICY_ID`
- `TEMPLATE_VERSION_ID`
- `VERSION_NO`
- `POLICY_STATUS`
- `EFFECTIVE_FROM`
- `EFFECTIVE_TO`
- `BINDING_TYPE`
- `SCOPE_LEVEL`
- `PRIORITY_WEIGHT`
- `CHECKSUM`
- `LAST_BUILT_PACKAGE_ID`

### `PR_POLICY_BINDING`

绑定对象表，描述策略作用到哪个项目或项目组：

- `POLICY_BINDING_ID`
- `POLICY_VERSION_ID`
- `BINDING_TYPE`
- `ITEM_CODE`
- `ITEM_NAME`
- `GROUP_CODE`
- `GROUP_NAME`

### `PR_POLICY_SCOPE`

业务维护视角的作用域表，核心字段：

- `POLICY_SCOPE_ID`
- `POLICY_VERSION_ID`
- `SCOPE_DIMENSION`
- `SCOPE_OPERATOR`
- `SCOPE_VALUE_TEXT`
- `SCOPE_VALUE_NUMBER`
- `SCOPE_VALUE_DATE`
- `SCOPE_JSON`

说明：

- `PR_POLICY_SCOPE` 不等于运行时 `Condition`
- 它只是业务维护输入

### `PR_POLICY_PARAM`

参数值表，核心字段：

- `POLICY_PARAM_ID`
- `POLICY_VERSION_ID`
- `PARAM_CODE`
- `VALUE_TYPE`
- `VALUE_TEXT`
- `VALUE_NUMBER`
- `VALUE_DATE`
- `VALUE_BOOL`
- `EXPR_TEXT`
- `EXPR_LEVEL`

### `PR_POLICY_REVIEW`

策略审批记录表，审批对象是 `PolicyVersion`：

- `REVIEW_ID`
- `POLICY_VERSION_ID`
- `REVIEW_STATUS`
- `REVIEW_STAGE`
- `SUBMITTED_BY`
- `SUBMITTED_AT`
- `REVIEWED_BY`
- `REVIEWED_AT`
- `REVIEW_COMMENT`
- `SOURCE_CHECKSUM`

## 三、运行时模型

### `PR_RUNTIME_PACKAGE`

规则包主表，核心字段：

- `PACKAGE_ID`
- `PACKAGE_VERSION`
- `PACKAGE_STATUS`
- `BUILD_SCOPE`
- `SOURCE_CHECKSUM`
- `BUILT_BY`
- `BUILT_AT`
- `ACTIVATED_BY`
- `ACTIVATED_AT`
- `ROLLED_BACK_FROM_PACKAGE_ID`

### `PR_RUNTIME_PACKAGE_POLICY`

记录某个包由哪些策略版本构建而来：

- `PACKAGE_POLICY_ID`
- `PACKAGE_ID`
- `POLICY_VERSION_ID`
- `POLICY_CODE`
- `TEMPLATE_VERSION_ID`
- `CAPABILITY_FAMILY`
- `PRIORITY_KEY`

### `PR_RUNTIME_RULE`

运行时规则头，核心字段：

- `RUNTIME_RULE_ID`
- `PACKAGE_ID`
- `SOURCE_TEMPLATE_VERSION_ID`
- `SOURCE_POLICY_VERSION_ID`
- `CAPABILITY_FAMILY`
- `MERGE_MODE`
- `TARGET_ITEM_CODE`
- `TARGET_GROUP_CODE`
- `SCOPE_LEVEL`
- `PRIORITY_KEY`
- `EFFECTIVE_FROM`
- `EFFECTIVE_TO`
- `MATCH_KEY`

### `PR_RUNTIME_CONDITION`

运行时条件明细表：

- `RUNTIME_CONDITION_ID`
- `RUNTIME_RULE_ID`
- `CONDITION_GROUP`
- `CONDITION_TYPE`
- `OPERATOR_TYPE`
- `LEFT_KEY`
- `RIGHT_VALUE`
- `PARAMS_JSON`
- `SORT_NO`

### `PR_RUNTIME_ACTION`

运行时动作明细表：

- `RUNTIME_ACTION_ID`
- `RUNTIME_RULE_ID`
- `STEP_NO`
- `ACTION_TYPE`
- `EXECUTOR_CODE`
- `PARAMS_JSON`
- `EXCLUSIVE_GROUP`
- `SORT_NO`
- `ON_ERROR`

### `PR_RUNTIME_PACKAGE_STATE`

当前激活包状态表，只保留一行：

- `STATE_CODE`
- `ACTIVE_PACKAGE_ID`
- `ACTIVE_PACKAGE_VERSION`
- `UPDATED_AT`
- `UPDATED_BY`

约束：

- 第一阶段只允许一个全院激活包
- 后续若支持多租户或分环境，再扩展 `STATE_CODE`

## 四、现有追溯表补充字段

为保证按包追溯，现有表需要补充字段。

### `PR_CHARGE_REQUEST_LOG`

新增：

- `RUNTIME_PACKAGE_ID`
- `RUNTIME_PACKAGE_VERSION`

说明：

- 请求级只记录本次计价使用的包版本
- 不在请求表存单一 `SOURCE_POLICY_VERSION_ID`，因为一次计价可能命中多条策略

### `PR_CHARGE_TRACE_STEP`

新增：

- `RUNTIME_PACKAGE_ID`
- `RUNTIME_RULE_ID`
- `SOURCE_POLICY_VERSION_ID`
- `SOURCE_TEMPLATE_VERSION_ID`

### `PR_CHARGE_DISCOUNT_DETAIL`

新增：

- `RUNTIME_PACKAGE_ID`
- `RUNTIME_RULE_ID`
- `SOURCE_POLICY_VERSION_ID`
- `SOURCE_TEMPLATE_VERSION_ID`

## 生命周期设计

### 1. 模板状态机

模板版本状态：

`DRAFT -> PUBLISHED -> DISABLED`

说明：

- 第一阶段模板只允许程序员或高权限管理员维护
- 已被策略引用的模板版本不得直接物理删除

### 2. 策略版本状态机

策略版本状态：

`DRAFT -> VALIDATED -> REVIEW_PENDING -> APPROVED -> PUBLISH_READY -> SUPERSEDED`

补充说明：

- 审批关闭时，`VALIDATED` 可直接进入 `PUBLISH_READY`
- `COMPILED` 不是策略状态
- 被拒绝时回到 `DRAFT`
- 新版本生效后，旧已激活来源策略版本标记为 `SUPERSEDED`

### 3. 审批记录状态机

审批记录状态：

`PENDING -> APPROVED -> REJECTED -> OUTDATED`

`OUTDATED` 含义：

- 提交审批后，策略版本内容发生变化
- 原审批结果失效，必须重新提审

### 4. 运行时包状态机

运行时包状态：

`BUILDING -> BUILT -> ACTIVE -> SUPERSEDED`

失败分支：

`BUILDING -> BUILD_FAILED`

回滚语义：

- 回滚不是“修改旧包内容”
- 回滚是“重新激活历史 BUILT 包”
- 被重新激活的包状态重新标记为 `ACTIVE`
- 原活动包转 `SUPERSEDED`

## 运行时契约

第一期运行时必须引入新的读模型契约。

### 核心接口

- `IRuntimePackageRepository`
- `IRuntimePackageStateRepository`
- `IRuntimeRuleReadRepository`
- `IRuntimeRuleBuildRepository`

### 运行时读取边界

- `IRuntimeRuleReadRepository` 负责按激活包和项目编码读取候选规则快照
- 读模型可直接返回 `RuntimeRuleSnapshot`
- 若现有执行内核暂时仍依赖 `RuleAggregate/RuleAction` 形状，则通过 `RuntimeRuleProjectionAdapter` 转换

关键约束：

- 引擎不得再直接依赖 `IRuleHeaderRepository / IRuleConditionRepository / IRuleActionRepository`
- `EffectiveRuleSnapshotLoader` 的数据源必须切到运行时包读模型

## 覆盖优先级与冲突规则

### 1. 覆盖按能力族判定

每个模板版本必须声明：

- `CapabilityFamily`
- `MergeMode`

第一阶段标准能力族：

- `UNIT_CONVERSION`
- `QTY_LIMIT_ONCE`
- `QTY_LIMIT_DAILY`
- `QTY_LIMIT_TIME_WINDOW`
- `FORMULA_PRICING`
- `AMOUNT_FLOOR`
- `AMOUNT_CEILING`
- `MUTEX`
- `SAME_OPERATION_CEILING`
- `CHILD_SURCHARGE`
- `ZERO_FALLBACK`

### 2. 合并模式

第一阶段只允许两种：

- `SINGLE_WINNER`
- `MULTI_ALLOWED`

默认建议：

- 换算、公式、金额上下限、同手术封顶使用 `SINGLE_WINNER`
- 明确允许叠加的子项加收才允许 `MULTI_ALLOWED`

### 3. 优先级键

同一能力族内，优先级按以下顺序比较：

1. `BindingRank`：`ITEM > GROUP > GLOBAL`
2. `ScopeOwnerRank`：`DEPT > SCENE > HOSPITAL > PLATFORM`
3. `SpecificityScore`：作用域维度非空越多越高
4. `SpecificityDimensionOrder`：`BODY_PART > VISIT_TYPE > TIME_RANGE > DEPT > SCENE`
5. `PRIORITY_WEIGHT`：值越小优先级越高
6. `PolicyVersionNo`：较新版本优先
7. `PolicyVersionId`：最终稳定兜底

说明：

- “同项目不同部位不同换算规则”依赖第 3、4 点实现
- 优先级键必须在编译期算出并持久化到 `PR_RUNTIME_RULE.PRIORITY_KEY`

### 4. 冲突判定

以下条件同时满足时视为冲突：

- 同一 `CapabilityFamily`
- `MergeMode = SINGLE_WINNER`
- 绑定对象相同或归一化后重叠
- 生效时间重叠
- 归一化作用域重叠
- 优先级键前 5 段完全相同

处理原则：

- 构建候选包时直接阻断
- 不允许把冲突留给运行时临时决定

## 编译算法

`RuntimePackageCompiler` 第一阶段固定走 10 步：

1. 读取 `PUBLISH_READY` 的策略版本
2. 加载模板版本及参数定义
3. 绑定参数默认值并做强校验
4. 校验弱表达式白名单
5. 归一化绑定与业务作用域
6. 计算能力族、合并模式和优先级键
7. 做冲突检测
8. 生成运行时规则头、条件、动作
9. 批量写入候选包及包来源关系
10. 将候选包标记为 `BUILT`

第一阶段规则：

- 只做“全量候选包构建”
- 不做增量编译
- 不做局部包激活

## 激活与回滚

### 候选包构建事务

候选包构建事务负责：

- 创建 `PR_RUNTIME_PACKAGE`
- 写 `PR_RUNTIME_PACKAGE_POLICY`
- 写 `PR_RUNTIME_RULE / CONDITION / ACTION`
- 将包状态从 `BUILDING` 更新为 `BUILT`

此事务完成后，对运行时仍不可见。

### 激活事务

激活事务只做指针切换，不重编译。

激活事务必须在同一 UOW 内完成：

1. 锁定 `PR_RUNTIME_PACKAGE_STATE`
2. 校验候选包状态为 `BUILT`
3. 更新 `ACTIVE_PACKAGE_ID / ACTIVE_PACKAGE_VERSION`
4. 更新包状态：新包 `ACTIVE`，旧包 `SUPERSEDED`
5. 写发布审计
6. 递增 `PR_CACHE_VERSION`
7. 写 `PR_CACHE_INVALIDATION_OUTBOX`

原子性要求：

- 激活成功前，旧包继续服务
- 激活事务失败时，活动指针不得改变

### 回滚事务

回滚事务本质上也是“激活历史已构建包”：

1. 锁定 `PR_RUNTIME_PACKAGE_STATE`
2. 校验目标历史包状态可激活
3. 更新活动指针
4. 写回滚审计
5. 递增缓存版本并写 outbox

## 运行时选择算法

运行时选择器固定走以下步骤：

1. 读取当前激活包状态
2. 按 `ItemCode / GroupCode` 加载候选运行时规则
3. 过滤生效时间和作用域
4. 按 `CapabilityFamily` 分组
5. 对 `SINGLE_WINNER` 选胜者，对 `MULTI_ALLOWED` 收集列表
6. 将获胜动作链按全局执行顺序送入现有引擎

运行时仍必须遵守已确认业务顺序：

1. 规则匹配
2. 双单位换算
3. 数量限制 / 时间窗 / 同组互斥
4. 公式折价
5. 金额下限
6. 金额上限 / TOPPRICE
7. 同手术封顶
8. 子项加收
9. 超出部分归零

## 表达式分权

### 弱表达式

开放给物价科，限制为：

- 四则运算
- `min/max/round/ceil/floor`
- 白名单变量

弱表达式必须在保存和构建阶段双重校验。

### 强表达式

第一阶段只预留字段与状态，不默认开放。

强表达式能力要求：

- 仅高权限角色可维护
- 高风险审批
- 专项测试用例门禁
- 独立特性开关

## 初始模板目录

第一批只落地高频规则族，不追求一次覆盖全部历史规则。

建议首批模板：

1. `TPL_UNIT_CONVERT_BY_BODY_PART`
2. `TPL_ONCE_QTY_LIMIT`
3. `TPL_DAILY_QTY_LIMIT`
4. `TPL_TIME_WINDOW_QTY_LIMIT`
5. `TPL_INCREMENT_PERCENT`
6. `TPL_AREA_STEP_INCREMENT`

第二批再补：

- `TPL_SAME_OPERATION_CEILING`
- `TPL_CHILD_SURCHARGE`
- `TPL_ZERO_FALLBACK`

## 工作台设计

### 模板中心

面向程序员/高权限管理员，维护：

- 模板
- 模板版本
- 参数定义
- 步骤骨架
- 风险等级
- 表达式权限

### 策略中心

面向物价科，维护：

- 模板选择
- 项目/项目组绑定
- 参数填写
- 生效时间
- 预览
- 提交发布

策略中心不展示技术执行细节。

### 发布中心

面向负责人、审批人、信息科，负责：

- 候选包校验结果
- 冲突结果
- 候选包与当前激活包差异
- 审批
- 激活
- 回滚

## 权限模型

- 物价科：维护弱表达式策略草稿
- 物价科负责人：提交发布、激活低风险候选包
- 审批人：审批高风险策略
- 程序员/信息科：维护模板、启用强表达式、处理迁移与导入
- 管理员：回滚包、查看审计与追溯

## 实施顺序

1. 新三模型表结构和运行时读模型契约
2. 编译器、候选包构建、活动包读取
3. 激活/回滚事务和缓存失效
4. 模板/策略应用服务与 API
5. 历史规则导入与追溯字段补齐
6. HIS 工作台与旧作者入口下线

## 风险与控制

### 风险 1：模板抽象过度

控制方式：

- 第一批模板只覆盖高频规则族
- 模板必须有明确执行器落点
- 不引入通用 DSL

### 风险 2：运行时包过大

控制方式：

- 候选包构建时预计算索引字段
- 运行时按 `ItemCode / GroupCode` 取候选
- 活动包状态单独缓存

### 风险 3：运行时仍被旧作者模型耦合

控制方式：

- 强制引入 `IRuntimeRuleReadRepository`
- `EffectiveRuleSnapshotLoader` 改造成读取运行时包
- 不再新增基于 `IRuleHeaderRepository` 的运行时逻辑

### 风险 4：审批和包激活边界混乱

控制方式：

- 审批对象固定为 `PolicyVersion`
- 激活对象固定为 `RuntimePackage`
- `COMPILED` 不再作为策略状态

### 风险 5：追溯信息不完整

控制方式：

- 请求级记录包版本
- 步骤级和折价明细级记录来源模板/策略/运行时规则

## 结论

本项目的新平台方向应明确为：

`Template -> PolicyVersion -> RuntimePackage -> PricingEngine`

第一阶段不再继续强化旧 `Rule + Condition + Action` 人工维护模式，而是：

- 保留执行内核
- 推翻旧作者模型
- 建立新的模板驱动策略平台
- 以规则包作为唯一运行时入口

这样才能在项目尚未正式投产前，把架构一次性调整到长期可维护的方向。
