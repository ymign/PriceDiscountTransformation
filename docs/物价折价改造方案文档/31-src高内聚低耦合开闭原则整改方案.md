# 31-src高内聚低耦合开闭原则整改方案

## 1. 目的

基于当前 `src/` 代码审计结果，本轮整改聚焦三个仍然影响可维护性的结构问题：

- `simulate` / `confirm` 工作流存在重复的“逐条明细计价 + 批量上下文累计”编排。
- `RuleMatchService`、`PricingSpecialFlagResolver` 仍存在内部 `new` 组装协作者的耦合点。
- 动作追溯分类、限额占额草稿结算仍由核心编排类中的硬编码分支控制，新增能力时需要改既有主流程。

本轮目标不是推翻现有架构，而是在保持业务口径不变的前提下，把这些高频维护点改造成：

- 更高内聚：同一职责只放在一个类中。
- 更低耦合：编排类只依赖抽象或显式注入的协作者。
- 更接近开闭原则：新增规则能力优先通过新增协作者，而不是修改既有主流程。

## 2. 现状判断

当前代码只能算“部分符合”，还不能认定已经达到严格意义上的高内聚、低耦合和开闭原则，主要原因如下：

- 计价工作流已拆分，但 `PricingSimulateWorkflow` 与 `PricingConfirmWorkflow` 仍重复维护单条明细循环、`BatchPricingContext` 和请求内占额累计。
- `RuleMatchService` 内部直接构造 `RuleConditionGroupMatcher`、`RuleActionPlanBuilder`、`EffectiveRuleSnapshotCache`，导致替换实现需要改服务本体。
- `PricingSpecialFlagResolver` 通过可空依赖加内部 `new` 拼装条件匹配器，查询路径的扩展点不够干净。
- `ActionExecutionPipeline` 的追溯步骤类型分类、`PricingEngine` 的占额草稿结算仍然依赖主流程中的 `switch` / 条件分支。

## 3. 本轮范围

本轮只做 3 个批次，且每个批次都要求行为等价：

1. 抽共享计价明细运行器，消除 `simulate` / `confirm` 的重复编排。
2. 去掉规则匹配和 special-flag 解析中的内部组装耦合点。
3. 把动作追溯分类和限额占额草稿结算改成可扩展策略。

## 4. 本轮不做

以下事项明确不纳入本轮：

- 不做 `Core/Application` 物理项目重新拆分。
- 不做 API 契约调整。
- 不修改计价顺序、金额口径、幂等口径、退费口径。
- 不改 `ACTION_TYPE_ORDER` 字典机制。
- 不做数据库结构调整。

## 5. 分批方案

### 批次 1：共享计价明细运行器

目标：

- 抽出“逐条明细创建 `PricingContext`、调用引擎、累计请求内占额、维护 `BatchPricingContext`”的公共流程。
- `PricingSimulateWorkflow` 和 `PricingConfirmWorkflow` 只保留各自特有的事务、幂等和持久化逻辑。

计划变更：

- 新增 `PricingItemCalculationRunner`。
- 新增请求内占额累计协作者，收口重复的 `AccumulateInRequestLimits`。
- 调整 `PricingSimulateWorkflow` / `PricingConfirmWorkflow` 构造与调用路径。

验收标准：

- `simulate` 和 `confirm` 的批量口径、限额口径、运行包快照口径不变。
- 两个 workflow 中不再保留重复的逐条计价循环。

### 批次 2：规则匹配协作者显式注入

目标：

- 让 `RuleMatchService` 和 `PricingSpecialFlagResolver` 只依赖显式注入的协作者，不在内部 `new` 出匹配器、排序器或缓存。

计划变更：

- 为规则条件组匹配、动作执行计划构建、运行期规则快照缓存引入显式接口。
- `RuleMatchService` 改为依赖注入这些协作者。
- `PricingSpecialFlagResolver` 改为直接接收条件组匹配协作者，而不是接收工厂后自行拼装。
- 同步调整 DI 和相关测试夹具。

验收标准：

- 生产代码中 `RuleMatchService`、`PricingSpecialFlagResolver` 不再持有内部 `new` 的协作者组装逻辑。
- 现有匹配行为、运行包优先级和 special-flag 粗判/细判口径不变。

### 批次 3：把主流程硬编码改为策略

目标：

- 让新增动作类型时，不必再修改动作追溯分类逻辑。
- 让新增限额占额类型时，不必再修改 `PricingEngine` 的结算 `switch`。

计划变更：

- 在 `IRuleActionExecutor` 上补充追溯步骤类型元数据。
- `ActionExecutionPipeline` 改为从执行器读取追溯步骤类型。
- 新增 `ILimitOccupyValueFinalizer` 及若干实现，把不同 `LimitType` 的占额草稿结算口径下沉为策略。
- `PricingEngine` 改为依赖限额占额结算策略。

验收标准：

- `ActionExecutionPipeline` 不再维护动作类型到追溯步骤类型的硬编码映射方法。
- `PricingEngine` 不再通过 `switch (LimitType)` 结算占额草稿。
- 同组互斥、同手术封顶、数量类限额的结算结果保持兼容。

## 6. 执行记录

### 批次状态

| 批次 | 状态 | 备注 |
| --- | --- | --- |
| 批次 1 | 已完成 | 已抽出共享计价明细运行器 |
| 批次 2 | 已完成 | 已去掉规则匹配和 special-flag 的内部组装 |
| 批次 3 | 已完成 | 已把动作追溯分类和占额结算改为策略 |

### 涉及文件

- `src/Pricing.RuleCenter.Application/Application/Pricing/Workflows/*`
- `src/Pricing.RuleCenter.Application/Application/Pricing/PricingItemCalculationRunner.cs`
- `src/Pricing.RuleCenter.Application/Application/Pricing/PricingInRequestLimitAccumulator.cs`
- `src/Pricing.RuleCenter.Application/Application/Engine/*`
- `src/Pricing.RuleCenter.Core/Interfaces/*`

### 验证命令

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore
git diff --check
```

### 验证结果

- 已执行全量测试，结果通过。
- `git diff --check` 通过。

## 7. 本轮结果

本轮完成后，`src` 在以下三个点上比整改前更接近高内聚、低耦合和开闭原则：

- 明细计价编排职责集中到共享运行器，workflow 只保留各自业务差异。
- 规则匹配主链路不再依赖内部临时组装的协作者，扩展和替换更直接。
- 核心主流程去掉两处关键行为硬编码，未来新增动作/限额类型时更容易通过新增策略扩展。

但本轮仍然没有解决“命名空间与物理项目边界混杂”的结构问题，这一项需要后续独立改造，不和本轮混做。
