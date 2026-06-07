# 配置优先计价规则平台设计

## 背景

当前 `src/` 已经具备规则匹配、动作管线、公式执行器、审批、追溯、占额等核心能力，但现有模型仍然以技术执行单元为中心：

- 业务维护对象接近 `Rule + Condition + Action + ExecutorCode`
- 运行时模型和维护模型耦合
- 物价科若要自助维护，仍需理解技术字段和参数 JSON
- 未来若直接叠加国家规则、本院差异、场景覆盖、科室覆盖，冲突检测和解释成本会快速失控

项目目前尚未正式落地，适合在此阶段一次性完成主模型重构，把系统从“可配置规则引擎”升级为“模板驱动的策略编译平台”。

## 目标

1. 把 90% 以上的新物价规则收敛为模板 + 参数配置，无需改代码。
2. 让物价科维护的是业务策略，而不是技术动作链。
3. 运行时只读取已发布的规则包，保证原子发布、快速回滚和稳定追溯。
4. 保留现有执行内核中有价值的部分，避免重写全部计价逻辑。
5. 为未来的场景覆盖、科室覆盖、高权限强表达式预留结构，但第一期不开放全部能力。

## 非目标

- 第一阶段不把所有历史界面原样兼容到新工作台。
- 第一阶段不向物价科开放强表达式。
- 第一阶段不开放场景级/科室级常规维护能力，只保留结构预留。
- 不再让人工直接维护运行时 `Rule/Condition/Action` 明细。

## 核心决策

### 决策 1：采用三模型架构

平台严格拆成三套模型：

- `Template`：模板模型，描述标准规则骨架和参数定义
- `Policy`：策略模型，描述本院具体项目如何套用模板
- `Runtime Package`：运行时规则包，描述当前可执行的编译产物

这三套模型分别服务于不同角色：

- 模板给程序员/高权限管理员维护
- 策略给物价科维护
- 规则包给引擎执行

### 决策 2：发布对象从单条规则改为规则包

发布不再是“单条规则立即生效”，而是：

1. 策略版本通过校验
2. 如开启审批，则完成审批
3. 编译器把当前应生效的策略版本统一编译为一个 `Runtime Package`
4. 通过原子切换把该包设为当前激活版本

运行时永远只认“当前激活包”。

### 决策 3：运行时复用执行内核，重建配置内核

现有以下能力继续保留并复用：

- `PricingEngine`
- `ActionExecutionPipeline`
- 现有各类 `IRuleActionExecutor`
- 现有受控弱表达式能力
- confirm / commit / cancel / reverse 流程
- 限额占用、追溯、冲销链路

需要重建的是：

- 业务维护模型
- 发布模型
- 冲突检测模型
- 工作台交互模型
- 运行时数据装载入口

### 决策 4：表达式分权

- `弱表达式`：开放给物价科，只允许四则运算、`min/max/round/ceil/floor` 和白名单变量
- `强表达式`：只开放给程序员或高权限角色，默认高风险，进入更严格审批与测试

## 总体架构

### 1. 模板层

模板层定义“规则族”而不是院内项目规则。典型模板包括：

- 2 小时窗口数量上限
- 固定比例递增
- 分部位换算 + 单肿物封顶
- 面积分段递增 + 同手术封顶
- 子项比例加收

模板层负责：

- 定义参数清单和数据类型
- 定义固定步骤骨架
- 定义可用作用域维度
- 定义风险级别与表达式权限
- 定义能力族与合并模式

### 2. 策略层

策略层是物价科真正维护的业务对象。每条策略要完成：

- 选择模板
- 绑定本院项目或项目组
- 填参数
- 设生效时间
- 设发布模式
- 选择适用范围

策略层不暴露：

- `ActionType`
- `ExecutorCode`
- 原始 `ParamsJson`
- 运行时规则顺序

### 3. 编译层

编译层负责把模板 + 策略转成运行时规则包，主要职责：

- 参数校验
- 表达式静态校验
- 作用域标准化
- 冲突检测
- 运行时规则生成
- 索引生成
- 包激活与回滚

### 4. 运行时层

运行时只做两件事：

- 读取当前激活包
- 用包内规则完成计价

运行时不能看到草稿策略，不能直接读取作者模型。

## 数据模型

### 一、模板模型

#### `PR_TEMPLATE`

模板主表，核心字段：

- `TEMPLATE_ID`
- `TEMPLATE_CODE`
- `TEMPLATE_NAME`
- `CATEGORY`
- `RISK_LEVEL`
- `EXPRESSION_MODE`
- `STATUS`
- `CURRENT_VERSION`

#### `PR_TEMPLATE_VERSION`

模板版本表，核心字段：

- `TEMPLATE_VERSION_ID`
- `TEMPLATE_ID`
- `VERSION_NO`
- `STATUS`
- `CHECKSUM`
- `DESCRIPTION`

#### `PR_TEMPLATE_PARAM_DEF`

模板参数定义表，核心字段：

- `PARAM_DEF_ID`
- `TEMPLATE_VERSION_ID`
- `PARAM_CODE`
- `PARAM_NAME`
- `VALUE_TYPE`
- `IS_REQUIRED`
- `DEFAULT_*`
- `DICT_TYPE`
- `MIN_VALUE`
- `MAX_VALUE`
- `REGEX_RULE`
- `UI_CONTROL`
- `HELP_TEXT`
- `RISK_FLAG`

#### `PR_TEMPLATE_STEP_DEF`

模板步骤骨架表，核心字段：

- `STEP_DEF_ID`
- `TEMPLATE_VERSION_ID`
- `STEP_NO`
- `STEP_KIND`
- `CAPABILITY_CODE`
- `EXECUTOR_CODE`
- `ON_ERROR`
- `BINDING_MODE`
- `STEP_CONFIG_CLOB`

#### `PR_TEMPLATE_SCOPE_DEF`

模板允许作用域定义，控制模板能开放哪些维度：

- `ITEM`
- `GROUP`
- `SCENE`
- `DEPT`
- `BODY_PART`
- `VISIT_TYPE`
- `TIME_RANGE`

### 二、策略模型

#### `PR_POLICY`

策略主表，核心字段：

- `POLICY_ID`
- `POLICY_CODE`
- `POLICY_NAME`
- `TEMPLATE_ID`
- `OWNER_TYPE`
- `PUBLISH_MODE`
- `STATUS`
- `CURRENT_VERSION`

#### `PR_POLICY_VERSION`

策略版本表，核心字段：

- `POLICY_VERSION_ID`
- `POLICY_ID`
- `TEMPLATE_VERSION_ID`
- `VERSION_NO`
- `LIFECYCLE_STATUS`
- `EFFECTIVE_FROM`
- `EFFECTIVE_TO`
- `SCOPE_LEVEL`
- `PRIORITY_WEIGHT`
- `OVERRIDE_MODE`
- `CHECKSUM`
- `LAST_COMPILED_PACKAGE_ID`

#### `PR_POLICY_BINDING`

绑定对象表，描述策略作用到哪个项目或项目组：

- `BINDING_TYPE`
- `ITEM_CODE`
- `ITEM_NAME`
- `GROUP_CODE`

#### `PR_POLICY_SCOPE`

作用域表，描述场景、部位、时间、就诊类型等适用条件。

它是业务维护视角的作用域，不等于运行时 `RuleCondition`。

#### `PR_POLICY_PARAM`

参数值表，核心字段：

- `PARAM_CODE`
- `VALUE_TYPE`
- `VALUE_TEXT`
- `VALUE_NUMBER`
- `VALUE_DATE`
- `VALUE_BOOL`
- `EXPR_TEXT`
- `EXPR_LEVEL`

#### `PR_POLICY_REVIEW`

策略审批记录表，审批对象是 `PolicyVersion`，不是运行时规则。

### 三、运行时模型

#### `PR_RUNTIME_PACKAGE`

规则包主表，核心字段：

- `PACKAGE_ID`
- `PACKAGE_VERSION`
- `BUILD_STATUS`
- `SOURCE_CHECKSUM`
- `BUILT_BY`
- `BUILT_AT`
- `ACTIVATED_BY`
- `ACTIVATED_AT`
- `BASE_PACKAGE_ID`

#### `PR_RUNTIME_PACKAGE_POLICY`

记录某个包由哪些策略版本编译而来，供追溯与差异比较。

#### `PR_RUNTIME_RULE`

运行时规则头，只给引擎使用，核心字段：

- `SOURCE_TEMPLATE_VERSION_ID`
- `SOURCE_POLICY_VERSION_ID`
- `TARGET_ITEM_CODE`
- `TARGET_GROUP_CODE`
- `SCOPE_LEVEL`
- `PRIORITY`
- `MATCH_KEY`

#### `PR_RUNTIME_CONDITION`

运行时条件明细表，是编译产物，不允许业务人员直接维护。

#### `PR_RUNTIME_ACTION`

运行时动作明细表，是编译产物，不允许业务人员直接维护。

#### `PR_RUNTIME_PACKAGE_STATE`

只保存当前激活包状态，运行时启动或缓存失效后从这里读取当前版本。

## 生命周期与发布链

统一使用以下状态链：

`DRAFT -> VALIDATED -> APPROVAL_PENDING -> APPROVED -> COMPILED -> ACTIVATED`

说明：

- 审批开关关闭时，`VALIDATED` 可直接进入 `COMPILED`
- 审批开关开启时，必须先进入 `APPROVAL_PENDING` 和 `APPROVED`
- `ACTIVATED` 是包级状态，不是单策略状态

## 服务边界

### `TemplateService`

负责模板、模板版本、模板参数定义、模板步骤骨架维护。

### `PolicyService`

负责策略、策略版本、绑定对象、参数值、适用范围维护。

### `PolicyValidationService`

负责参数合法性、白名单表达式、模板约束、生效期等静态校验。

### `PolicyConflictService`

负责基于能力族、作用域、绑定对象、生效期做冲突检测。

### `RuntimePackageCompiler`

负责把策略版本编译成运行时规则包。

### `RuntimePackagePublishService`

负责包构建、激活、回滚、缓存失效和发布审计。

### `RuntimePricingService`

负责读取当前包并驱动现有计价引擎完成计价。

## 覆盖优先级与冲突规则

### 决策：覆盖按能力族判定，而不是整条规则判定

每个模板必须声明：

- `CapabilityFamily`
- `MergeMode`

示例能力族：

- `UNIT_CONVERSION`
- `QTY_LIMIT`
- `FORMULA_PRICING`
- `AMOUNT_FLOOR`
- `AMOUNT_CEILING`
- `MUTEX`
- `SAME_OPERATION_CEILING`
- `CHILD_SURCHARGE`
- `ZERO_FALLBACK`

### 优先级顺序

同一 `CapabilityFamily` 内，按以下顺序比较：

1. 绑定粒度：`ITEM > GROUP`
2. 作用域层级：`DEPT > SCENE > HOSPITAL > PLATFORM`
3. 作用域具体度：条件越具体优先级越高
4. 人工优先级：`PRIORITY_WEIGHT` 越小越高
5. 版本新旧：同级时较新版本覆盖旧版本

### 冲突规则

- 同时间段、同绑定对象、同能力族、同覆盖层级，不能存在两条 `SINGLE_WINNER` 策略同时生效
- `MULTI_ALLOWED` 只开放给明确允许叠加的能力族
- 强表达式策略默认高风险

## 编译算法

`RuntimePackageCompiler` 固定走 8 步：

1. 读取待入包的可发布策略版本
2. 补齐模板参数定义并校验参数
3. 归一化绑定与作用域
4. 计算能力族与优先级键
5. 做冲突检测
6. 生成运行时规则头、条件、动作
7. 建立运行时索引
8. 写入新包并原子激活

## 运行时选择算法

运行时选择器固定走以下步骤：

1. 读取当前激活包
2. 按 `ItemCode / GroupCode` 找候选规则
3. 过滤生效时间和作用域
4. 按 `CapabilityFamily` 分组
5. 对 `SINGLE_WINNER` 选胜者，对 `MULTI_ALLOWED` 收集列表
6. 把获胜动作链按固定顺序送入现有计价引擎

## 工作台设计

### 一、模板中心

供程序员和高权限管理员使用，维护：

- 模板
- 模板版本
- 参数定义
- 步骤骨架
- 风险级别
- 表达式权限

### 二、策略中心

供物价科使用，维护：

- 选择模板
- 绑定项目/项目组
- 填参数
- 选生效时间
- 选发布模式
- 看效果预览

不暴露技术执行细节。

### 三、发布中心

供负责人、审批人、信息科使用，负责：

- 校验结果查看
- 冲突结果查看
- 包差异预览
- 审批/发布
- 包回滚

## 权限模型

- 物价科：弱表达式策略维护
- 物价科负责人：提交发布、低风险直发
- 审批人：审批高风险策略
- 程序员/信息科：模板、强表达式、特殊能力维护
- 管理员：激活包、回滚包、审计查看

## 分期实施建议

### 第 1 期：运行时包化

- 建三模型表结构
- 建编译器和包读取器
- 改造现有引擎入口为读取运行时包

### 第 2 期：模板与策略维护

- 建模板中心 API
- 建策略中心 API
- 建参数与作用域维护能力
- 建试算预览

### 第 3 期：发布治理

- 建审批开关
- 建冲突检测
- 建包级差异预览
- 建回滚能力

### 第 4 期：扩展能力

- 场景覆盖
- 科室覆盖
- 强表达式
- 更多模板族

## 预期收益

- 新规则以模板参数方式落地，减少代码改动频率
- 物价科维护对象从技术规则切换为业务策略
- 发布切换原子化，回滚更快
- 追溯链可同时解释模板来源、策略来源和运行时包版本
- 为未来扩展预留能力，同时不在第一期把复杂度一次放开

## 风险与控制

### 风险 1：模板抽象过度

控制方式：第一期只落地高频规则族，不追求万能 DSL。

### 风险 2：运行时包过大

控制方式：编译阶段预建索引，运行时只做候选过滤和能力族选择。

### 风险 3：工作台过于技术化

控制方式：业务人员只接触模板、参数、绑定、作用域、试算和发布。

### 风险 4：强表达式失控

控制方式：默认关闭，仅对高权限角色开放，并纳入高风险审批。

## 结论

本项目最合理的升级方向，不是继续增强“人工维护规则表”的现有模式，而是把规则中心升级为：

`模板驱动的策略编译平台 + 规则包运行时`

这样既能复用当前执行内核，又能从主模型层面解决配置优先、自助维护、原子发布、快速回滚和长期扩展的问题。
