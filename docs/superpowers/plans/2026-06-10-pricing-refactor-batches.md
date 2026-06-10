# Pricing Refactor Batches Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 分三批把计价折价代码收敛到“workflow 只编排、engine 只计算、共享状态和限额逻辑可读可测”的结构，并保持现有行为不变。

**Architecture:** 第一批先拆薄 workflow 和持久化编排；第二批抽出限额执行器公共流程；第三批继续缩 `PricingContext` 并把请求共享状态访问改成强类型入口。每批都必须先绿测，再提交并推送。

**Tech Stack:** .NET 8, ASP.NET Core, MediatR, xUnit, SqlSugar, Oracle

---

## Batch Rules

- 每一批必须是可独立提交、可独立回滚的行为保持型重构。
- 每一批结束后都执行：`dotnet build src\Pricing.RuleCenter.slnx --no-restore`
- 每一批结束后都执行：`dotnet test src\Pricing.RuleCenter.slnx`
- 每一批结束后都执行：`git add ... && git commit -m "<中文提交信息>" && git push origin HEAD`
- 提交信息必须使用中文祈使句，且一批一个提交。

## Chunk 1: Workflow 瘦身

### Task 1: 拆分 simulate/confirm 的持久化职责

**Files:**
- Create: `src/Pricing.RuleCenter.Application/Application/Pricing/Persistence/PricingSimulationPersistenceService.cs`
- Create: `src/Pricing.RuleCenter.Application/Application/Pricing/Persistence/PricingConfirmationPersistenceService.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/Workflows/PricingSimulateWorkflow.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/Workflows/PricingConfirmWorkflow.cs`
- Modify: `src/Pricing.RuleCenter.Api/Startup/RuleCenterApiServiceCollectionExtensions.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingWorkflowSupportTests.cs`

- [ ] **Step 1: 先补一个 workflow 边界测试**

目标：验证 `workflow` 仍返回原有响应，但低层持久化逻辑被委托给独立服务。

- [ ] **Step 2: 创建试算持久化服务**

职责只包含：
- 写 `request log`
- 写 `trace steps`
- 写 `response json`

- [ ] **Step 3: 创建确认持久化服务**

职责只包含：
- 写 `request log`
- 写 `trace steps`
- 写 `discount details`
- 写 `limit occupies`
- 写 `response json`

- [ ] **Step 4: 修改 `PricingSimulateWorkflow`**

要求：
- 保留“校验 -> 权威单价诊断 -> 捕获运行包 -> 计算 -> 调持久化服务 -> 返回响应”的顺序
- 删除 workflow 内部对持久化细节的直接拼装

- [ ] **Step 5: 修改 `PricingConfirmWorkflow`**

要求：
- 保留“幂等 -> 事务 -> 捕获运行包 -> 计算 -> 调持久化服务 -> 返回响应”的顺序
- 删除 workflow 内部逐条写 `discount detail` / `limit occupy` 的循环

- [ ] **Step 6: 更新 DI 注册**

Run: `dotnet build src\Pricing.RuleCenter.slnx --no-restore`
Expected: BUILD SUCCESS

- [ ] **Step 7: 运行 workflow 相关测试**

Run: `dotnet test src\Pricing.RuleCenter.slnx --filter "PricingApiServiceTests|PricingWorkflowSupportTests"`
Expected: PASS

- [ ] **Step 8: 提交并推送第一批**

Run: `git add src tests docs/superpowers/plans/2026-06-10-pricing-refactor-batches.md`
Run: `git commit -m "拆分计价工作流与持久化职责"`
Run: `git push origin HEAD`

---

## Chunk 2: 限额执行器模板化

### Task 2: 提取限额执行器公共流程

**Files:**
- Create: `src/Pricing.RuleCenter.Application/Application/Engine/Executors/LimitExecution/LimitExecutionSupport.cs`
- Create: `src/Pricing.RuleCenter.Application/Application/Engine/Executors/LimitExecution/LimitOccupyDraftAppender.cs`
- Create: `src/Pricing.RuleCenter.Application/Application/Engine/Executors/LimitExecution/SharedLimitStateReader.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Engine/Executors/DailyQtyLimitExecutor.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Engine/Executors/TimeWindowLimitExecutor.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Engine/Executors/OnceQtyLimitExecutor.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Engine/Executors/SameGroupMutexExecutor.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Engine/Executors/SameOperationCeilingExecutor.cs`
- Test: `tests/Pricing.RuleCenter.Tests/DailyAndTimeWindowLimitExecutorTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/OnceQtyLimitExecutorTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/SameOperationCeilingExecutorTests.cs`
- Test: `tests/Pricing.RuleCenter.Core.Tests/SameGroupMutexBatchTests.cs`

- [ ] **Step 1: 给一个执行器先补保护测试**

目标：锁定“历史占额 + 请求内共享状态 + 截断 + 占额草稿”结果不变。

- [ ] **Step 2: 提取共享状态读取器**

统一负责：
- 读取请求内累计数量
- 读取请求内累计金额
- 按业务时间过滤请求内占额草稿

- [ ] **Step 3: 提取占额草稿追加器**

统一负责：
- 去重判断
- 草稿对象构建
- 常见 `LimitType/LimitDimensionCode` 规则

- [ ] **Step 4: 提取金额按数量比例缩放工具**

统一替换 `beforeQty == 0 ? 0 : FinalAmount * FinalQty / beforeQty` 这类重复逻辑。

- [ ] **Step 5: 重构 `DailyQtyLimitExecutor` 和 `TimeWindowLimitExecutor`**

要求：
- 公共读取和截断逻辑移到 support 类
- 保留各自的维度生成规则

- [ ] **Step 6: 重构 `OnceQtyLimitExecutor`、`SameGroupMutexExecutor`、`SameOperationCeilingExecutor`**

要求：
- 公共状态读取走统一 helper
- 各自只保留本规则特有的维度、窗口、锁键和结果修改逻辑

- [ ] **Step 7: 运行限额执行器测试**

Run: `dotnet test src\Pricing.RuleCenter.slnx --filter "DailyAndTimeWindowLimitExecutorTests|OnceQtyLimitExecutorTests|SameOperationCeilingExecutorTests|SameGroupMutexBatchTests"`
Expected: PASS

- [ ] **Step 8: 跑全量 build/test**

Run: `dotnet build src\Pricing.RuleCenter.slnx --no-restore`
Run: `dotnet test src\Pricing.RuleCenter.slnx`
Expected: PASS

- [ ] **Step 9: 提交并推送第二批**

Run: `git add src tests`
Run: `git commit -m "抽取限额执行器公共流程"`
Run: `git push origin HEAD`

---

## Chunk 3: PricingContext 缩身与强类型访问

### Task 3: 缩小可变上下文，减少字符串键散落

**Files:**
- Create: `src/Pricing.RuleCenter.Core/Models/PricingRequestFacts.cs`
- Create: `src/Pricing.RuleCenter.Core/Models/PricingComputationState.cs`
- Create: `src/Pricing.RuleCenter.Core/Models/RequestSharedStateKeys.cs`
- Modify: `src/Pricing.RuleCenter.Core/Models/PricingContext.cs`
- Modify: `src/Pricing.RuleCenter.Core/Models/RequestSharedPricingState.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/Builders/PricingContextFactory.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Engine/PricingEngine.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Engine/Executors/ChildItemPercentExecutor.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/PricingItemCalculationRunner.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/PricingSpecialFlagResolver.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RequestSharedPricingStateTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/ChildItemPercentExecutorTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs`

- [ ] **Step 1: 先补共享状态 key 的测试**

目标：锁定 `DAY_QTY:*`、`TIME_WINDOW:*`、`MUTEX:*`、`OP_CEILING:*`、`ITEM_AMT:*` 的生成口径。

- [ ] **Step 2: 把 `PricingContext` 拆成事实输入和运行结果两层**

要求：
- `PricingRequestFacts` 只放患者、项目、场景、时间、来源等输入事实
- `PricingComputationState` 只放 `FinalQty`、`FinalAmount`、`TraceSteps`、`PendingLimitOccupies` 等运行结果
- `PricingContext` 退化成组合对象，不再自己承载所有字段

- [ ] **Step 3: 为 `RequestSharedPricingState` 增加 typed accessor**

至少包含：
- `GetLimitQty`
- `GetLimitAmount`
- `GetParentItemAmount`
- `IncrementMutexCount`
- `IncrementOperationAmount`

- [ ] **Step 4: 改 `PricingContextFactory` 和 `PricingItemCalculationRunner`**

要求：
- 构造上下文时明确填充 facts/state/shared-state
- 不再让执行器直接依赖大量松散字段

- [ ] **Step 5: 改 `PricingEngine` 和 `ChildItemPercentExecutor`**

要求：
- `ChildItemPercentExecutor` 不再手拼 `"ITEM_AMT:{code}"`
- `PricingEngine` 只操作 `PricingComputationState`

- [ ] **Step 6: 收尾 `PricingSpecialFlagResolver`**

要求：
- special-flag 路径继续走统一规则快照入口
- 轻量上下文构造保持最小字段集

- [ ] **Step 7: 运行共享状态和上下文相关测试**

Run: `dotnet test src\Pricing.RuleCenter.slnx --filter "RequestSharedPricingStateTests|ChildItemPercentExecutorTests|PricingApiServiceTests"`
Expected: PASS

- [ ] **Step 8: 跑全量 build/test**

Run: `dotnet build src\Pricing.RuleCenter.slnx --no-restore`
Run: `dotnet test src\Pricing.RuleCenter.slnx`
Expected: PASS

- [ ] **Step 9: 提交并推送第三批**

Run: `git add src tests`
Run: `git commit -m "收敛计价上下文与共享状态访问"`
Run: `git push origin HEAD`

---

## Done Criteria

- `simulate`、`confirm`、`special-flag` 三条主链路都能用一句话说明入口职责。
- workflow 文件里不再混杂大量持久化细节。
- 限额执行器的重复读状态/截断/占额草稿代码明显下降。
- `PricingContext` 不再继续膨胀，字符串键访问集中到少数 helper。
- `dotnet build src\Pricing.RuleCenter.slnx --no-restore` 通过。
- `dotnet test src\Pricing.RuleCenter.slnx` 通过。
