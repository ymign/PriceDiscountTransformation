# src 代码结构与规则引擎整改开发计划

> **For agentic workers:** REQUIRED: Use `superpowers:subagent-driven-development` if subagents are available, or `superpowers:executing-plans` if working in a single agent session. Steps use checkbox (`- [ ]`) syntax for tracking. This plan is designed for goal mode execution: proceed chunk by chunk, verify after every chunk, and do not skip tests.

**Goal:** 在不推翻现有规则中心架构的前提下，降低 `src` 阅读和维护难度，并把规则引擎从“能覆盖当前历史规则”提升为“多数未来价格政策通过配置解决，少数新政策只新增规则原语/执行器，不改渠道和主流程”。

**Architecture:** 保留当前 `API/Application/Domain/Infrastructure` 四层结构、条件-动作-执行器模型、`confirm -> commit/cancel -> reverse/expire` 资金状态链。整改采用“先加测试保护，再拆大类，再抽能力原语，再补规则能力矩阵和上线验证”的路线。每个阶段必须保持行为等价，先重构边界，再增强引擎表达能力。

**Tech Stack:** .NET 6、ASP.NET Core Web API、MediatR、FluentValidation、SqlSugarCore、Oracle 11g、xUnit、IMemoryCache、Oracle `SELECT ... FOR UPDATE`。

---

## 0. Goal 模式启动提示词

后续进入 goal 模式时，建议直接使用下面这段作为目标提示词：

```text
请按 docs/物价折价改造方案文档/26-src代码结构与规则引擎整改开发计划.md 一次性完成 src 代码结构与规则引擎整改。

执行要求：
1. 先阅读 CLAUDE.md、AGENTS.md、docs/物价折价改造方案文档/25-src设计全面分析报告.md、docs/物价折价改造方案文档/26-src代码结构与规则引擎整改开发计划.md。
2. 严格按计划的 Chunk 顺序执行，不要跳过测试保护阶段。
3. 每个 Chunk 完成后运行该 Chunk 指定测试；失败必须修复后再继续。
4. 资金链路行为必须保持兼容：confirm 幂等、commit 对账、cancel 释放、reverse 超退校验、expire 清理均不能退化。
5. 规则引擎现有行为必须保持兼容：动作顺序、NULL 与 0 语义、业务时间优先、最终金额 2 位四舍五入、超出部分 0 元兜底不能改变。
6. 除计划明确要求外，不做无关格式化和无关重构。
7. 每个阶段优先添加或补强测试，再改实现。
8. 完成后运行 dotnet test src\Pricing.RuleCenter.slnx --no-restore 和 git diff --check，并更新整改记录文档。
```

预期效果：

- `PricingAppService` 不再接近 2000 行，资金主流程按用例拆分。
- `RulePublishAppService` 不再承载所有发布、审批、门禁、冲突检测逻辑。
- 规则引擎增加“能力矩阵”和“规则原语缺口”文档。
- 新政策优先走配置；配置无法表达时，只新增 evaluator/executor/公式原语，不改 HIS 侧和计价主流程。
- 测试数量增加，现有 255 个测试保持通过。

## 1. 总体整改路线

### 1.1 两条主线

主线 A：降低代码阅读难度。

- 把过大的应用服务拆成单一职责用例类。
- 把日志、响应构造、持久化写入、幂等判断、权威单价校验从主流程中抽出。
- 保持 Controller、MediatR 命令和对外 API 不变。
- 保持数据库表结构不做破坏性调整。

主线 B：提升规则引擎可配置覆盖能力。

- 建立“价格政策能力矩阵”，明确哪些政策纯配置可表达，哪些需要新增规则原语。
- 将现有规则能力整理为稳定原语：条件原语、动作原语、公式原语、组合原语。
- 补充表达式公式引擎设计和最小落地能力。
- 补充发布前门禁，避免配置出“看似可配、实际会错算”的规则。

### 1.2 不做的事

本计划不做以下事项，除非后续单独立项：

- 不推翻现有四层架构。
- 不切换 ORM。
- 不把规则引擎改成外部商业规则引擎。
- 不一次性引入 Redis、消息总线或微服务拆分。
- 不修改 HIS 客户端业务流程，除非测试发现接口契约必须补充。
- 不追求“所有未来政策永远无需代码”，只追求“主流程稳定、扩展点明确、绝大多数规则配置化”。

## 2. 目标架构

### 2.1 计价应用层目标结构

目标目录：

```text
src/Application/Application/Pricing/
  PricingAppService.cs                  // 兼容门面，薄封装
  UseCases/
    SimulatePricingUseCase.cs
    ConfirmPricingUseCase.cs
    CommitPricingUseCase.cs
    CancelPricingUseCase.cs
    ReversePricingUseCase.cs
    GetSpecialFlagUseCase.cs
  Idempotency/
    PricingIdempotencyService.cs
    PricingIdempotencyResult.cs
  AuthorityPrice/
    AuthorityPriceChecker.cs
  Persistence/
    PricingRequestLogWriter.cs
    PricingTraceStepWriter.cs
    PricingDiscountDetailWriter.cs
    PricingLimitOccupyWriter.cs
    PricingReverseLogWriter.cs
  Builders/
    PricingContextFactory.cs
    PricingResponseBuilder.cs
    PricingTraceIdGenerator.cs
    PricingResultGroupNoGenerator.cs
  Validation/
    PricingRequestGuard.cs
```

设计目标：

- `PricingAppService` 只保留对外方法，委托给 use case，避免一次性改 Controller 和测试。
- 每个 use case 只处理一条业务链路。
- 持久化 writer 只负责把领域结果落库，不做业务判断。
- builder 只负责构造上下文、响应、trace id、result group no。
- guard/checker/service 只负责校验，不夹杂落库。

### 2.2 规则发布目标结构

目标目录：

```text
src/Application/Application/Rules/
  RulePublishAppService.cs              // 兼容门面，薄封装
  Publishing/
    PublishRuleUseCase.cs
    DisableRuleUseCase.cs
    RollbackRuleUseCase.cs
    RulePublishTransactionWriter.cs
    RulePublishCacheInvalidator.cs
  Guards/
    RulePublishGuard.cs
    RuleApprovalGate.cs
    RuleConflictDetector.cs
    RuleActionParameterValidator.cs
    RuleTestCaseGate.cs
    RuleCriticalActionGuard.cs
    RuleChildItemGuard.cs
  Profiles/
    RuleConflictProfile.cs
    RuleConditionScope.cs
```

设计目标：

- 发布、停用、回滚三个状态机分离。
- 门禁聚合类只编排各项检查，不写具体检查细节。
- 冲突检测和参数校验可以独立测试。
- 缓存失效集中处理，避免漏清理。

### 2.3 规则引擎增强目标结构

目标目录：

```text
src/Application/Application/Engine/
  RuleMatchService.cs
  RuleRuntimeSnapshot/
    EffectiveRuleSnapshot.cs
    EffectiveRuleSnapshotLoader.cs
    EffectiveRuleSnapshotCache.cs
  Formula/
    FormulaEvaluationContext.cs
    FormulaExpressionEvaluator.cs
    FormulaFunctionRegistry.cs
    FormulaExpressionValidator.cs
  Capability/
    RuleCapabilityCatalog.cs
    RuleCapabilityMatrixExporter.cs
```

设计目标：

- 当前规则匹配继续可用。
- 逐步引入生效规则快照，减少运行期多表 N+1 读取。
- 公式从“每种公式一个 executor”逐步升级为“常见公式可表达式配置”。
- 新政策先映射能力矩阵，再判断配置、表达式、还是新增原语。

## 3. 全局执行规则

- [ ] 每个 Chunk 开始前运行 `git status --short --branch`，确认工作区状态。
- [ ] 每个 Chunk 只解决一个目标，不混入无关格式化。
- [ ] 每个重构任务先补测试或确认现有测试覆盖。
- [ ] 每个 Chunk 完成后运行指定测试。
- [ ] 每个 Chunk 完成后运行 `git diff --check`。
- [ ] 涉及资金链路的 Chunk 必须运行 `dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "PricingApiServiceTests|PricingReverseTests|CoreBusinessCoverageTests"`。
- [ ] 涉及规则发布的 Chunk 必须运行 `dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "RulePublishConflictTests|RuleApprovalAppServiceTests|RuleDefinitionTransactionTests"`。
- [ ] 涉及规则引擎的 Chunk 必须运行 `dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "RuleMatchServiceTests|ActionExecutionPipelineTests|CoreBusinessCoverageTests"`。
- [ ] 全部完成后运行 `dotnet test src\Pricing.RuleCenter.slnx --no-restore`。

## Chunk 1: 基线保护与阅读地图

**目标：** 在动代码前建立基线，明确当前大类职责和必须保持的行为。

**Files:**

- Read: `CLAUDE.md`
- Read: `AGENTS.md`
- Read: `docs/物价折价改造方案文档/25-src设计全面分析报告.md`
- Create: `docs/物价折价改造方案文档/27-src整改执行记录.md`
- Modify: none

### Task 1.1: 建立执行记录

- [ ] 创建 `docs/物价折价改造方案文档/27-src整改执行记录.md`。

建议内容：

```markdown
# src 整改执行记录

## 基线

- 开始日期：
- 当前分支：
- 初始测试命令：`dotnet test src\Pricing.RuleCenter.slnx --no-restore`
- 初始测试结果：

## Chunk 进度

| Chunk | 状态 | 测试结果 | 备注 |
| --- | --- | --- | --- |
| Chunk 1 | 未开始 |  |  |
```

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore
```

Expected：

- 255 个测试左右全部通过。
- 如果测试数量有变化，以实际结果记录。

- [ ] 更新 `27-src整改执行记录.md` 的基线测试结果。

### Task 1.2: 生成当前职责地图

- [ ] 在 `27-src整改执行记录.md` 增加“当前大类职责地图”。

必须记录：

- `PricingAppService` 当前 public 方法和职责。
- `RulePublishAppService` 当前 public 方法和职责。
- `RuleMatchService` 当前职责。
- `ActionExecutionPipeline` 当前职责。

执行命令：

```powershell
rg -n "public async Task|public Task|private async Task|private static|class PricingAppService|class RulePublishAppService" src\Application\Application\Pricing\PricingAppService.cs src\Application\Application\Rules\RulePublishAppService.cs
```

验收：

- 文档中能看出后续拆分目标。
- 没有修改源码。

## Chunk 2: 统一计价校验入口

**目标：** 先降低重复校验造成的阅读噪音，统一 FluentValidation 与应用服务手工校验口径。

**Files:**

- Modify: `src/Application/Application/Pricing/Validation/PricingCommandValidators.cs`
- Create: `src/Application/Application/Pricing/Validation/PricingRequestGuard.cs`
- Modify: `src/Application/Application/Pricing/PricingAppService.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingCalculateRequestTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/ValidationBehaviorTests.cs`

### Task 2.1: 补齐现有校验行为测试

- [ ] 阅读 `PricingAppService.GetRequiredItems` 当前校验口径。
- [ ] 在 `PricingCalculateRequestTests` 增加或确认以下测试：

必须覆盖：

- `Items` 为空失败。
- `InputQty = 0` 失败或通过，以当前业务约定为准。建议统一为失败，因为服务层当前要求大于 0。
- `UnitPrice < 0` 失败。
- `BusinessChargeTime` 为空失败。
- `PricingParts[].Qty <= 0` 失败。

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "PricingCalculateRequestTests|ValidationBehaviorTests"
```

Expected：

- 新增测试先能暴露口径不一致，或确认当前行为。

### Task 2.2: 新增 PricingRequestGuard

- [ ] 创建 `src/Application/Application/Pricing/Validation/PricingRequestGuard.cs`。

职责：

- 统一 `PricingCalculateRequest`、`PricingCommitRequest`、`PricingCancelRequest`、`PricingReverseRequest` 的不可绕过业务校验。
- 返回规范化后的 items，替代 `GetRequiredItems` 内部散落校验。
- 不访问数据库。
- 不写日志。

建议 API：

```csharp
internal static class PricingRequestGuard
{
    public static IReadOnlyList<PricingCalculateItemRequest> GetRequiredItems(PricingCalculateRequest request);
    public static void EnsureConfirmRequest(PricingCalculateRequest request);
    public static void EnsureCommitRequest(PricingCommitRequest request);
    public static void EnsureCancelRequest(PricingCancelRequest request);
    public static void EnsureReverseRequest(PricingReverseRequest request);
}
```

- [ ] 将 `PricingAppService` 中 `GetRequiredItems`、`ValidateCommitRequest`、`ValidateCancelRequest`、`ValidateReverseRequest` 的实现迁移到 `PricingRequestGuard`。
- [ ] 在 `PricingAppService` 中保留私有方法薄包装或直接替换调用。
- [ ] 保证异常类型和错误消息尽量不变，避免测试和客户端契约大面积变化。

### Task 2.3: 对齐 FluentValidation

- [ ] 修改 `PricingCommandValidators.cs`，使入口校验与 `PricingRequestGuard` 口径一致。

重点：

- `InputQty` 使用 `GreaterThan(0)`，不要入口允许 0 而服务拒绝 0。
- `BusinessChargeTime` 必填。
- `PricingParts[].Qty` 大于 0。
- confirm 必须有 `BusinessRequestNo`。
- commit 如果当前业务要求 `ActualItems` 必填，应在 validator 表达；如果为了兼容旧调用允许部分空，则必须在测试说明。

### Task 2.4: 验证

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "PricingCalculateRequestTests|ValidationBehaviorTests|PricingApiServiceTests"
```

- [ ] 更新 `27-src整改执行记录.md`。

## Chunk 3: 抽出计价上下文和响应构造

**目标：** 从 `PricingAppService` 里先抽出纯构造逻辑，降低主流程阅读负担，不改变资金行为。

**Files:**

- Create: `src/Application/Application/Pricing/Builders/PricingContextFactory.cs`
- Create: `src/Application/Application/Pricing/Builders/PricingResponseBuilder.cs`
- Create: `src/Application/Application/Pricing/Builders/PricingTraceIdGenerator.cs`
- Create: `src/Application/Application/Pricing/Builders/PricingResultGroupNoGenerator.cs`
- Modify: `src/Application/Application/Pricing/PricingAppService.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/BatchPricingContextTests.cs`

### Task 3.1: 抽 PricingTraceIdGenerator

- [ ] 创建 `PricingTraceIdGenerator`。

职责：

- 从 `BuildTraceId` 迁移逻辑。
- 只生成业务 trace id。
- 不访问数据库。

建议 API：

```csharp
internal static class PricingTraceIdGenerator
{
    public static string Build(string callType, string? requestNo, string? businessRequestNo);
}
```

- [ ] 替换 `PricingAppService.BuildTraceId` 调用。
- [ ] 保持生成格式兼容，除非测试明确允许变化。

### Task 3.2: 抽 PricingResultGroupNoGenerator

- [ ] 创建 `PricingResultGroupNoGenerator`。

职责：

- 从 `BuildResultGroupNo`、`ResolveResultGroupNo` 迁移逻辑。
- 处理主子项目同组编号。

建议 API：

```csharp
internal static class PricingResultGroupNoGenerator
{
    public static string Build(long requestId, PricingCalculateItemRequest item, string groupType);
    public static string? Resolve(PricingCalculateItemRequest item, PricingResult result, long requestId);
}
```

### Task 3.3: 抽 PricingContextFactory

- [ ] 创建 `PricingContextFactory`。

职责：

- 从 `BuildContext` 迁移逻辑。
- 负责把 request + item 标准化为 `PricingContext`。
- 合并 extra params。

建议 API：

```csharp
internal sealed class PricingContextFactory
{
    public PricingContext Create(PricingContextBuildInput input);
}
```

注意：

- 如果 `PricingContextBuildInput` 当前是 `PricingAppService` 内部 record，需要迁移到独立文件或保持 internal。
- 不改变 `BusinessChargeTime` 优先规则。
- 不改变 `PricingParts` 明细表达。

### Task 3.4: 抽 PricingResponseBuilder

- [ ] 创建 `PricingResponseBuilder`。

职责：

- 从 `BuildResponse`、`BuildItemResponse`、`BuildReasonDesc`、`BuildReplacementReasonDesc`、`BuildChildReasonDesc` 迁移逻辑。
- 只负责响应 DTO 构造，不写数据库。

建议 API：

```csharp
internal sealed class PricingResponseBuilder
{
    public PricingCalculateResponse Build(
        long requestId,
        IReadOnlyList<ItemPricingCalculation> calculations,
        DateTime? expireAt = null);
}
```

### Task 3.5: 验证

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "PricingApiServiceTests|BatchPricingContextTests|CoreBusinessCoverageTests"
```

验收：

- `PricingAppService` 行数明显下降。
- 所有响应字段与原测试兼容。

## Chunk 4: 抽出权威单价与幂等服务

**目标：** 把 confirm/simulate 主流程中的横切业务能力抽成独立服务。

**Files:**

- Create: `src/Application/Application/Pricing/AuthorityPrice/AuthorityPriceChecker.cs`
- Create: `src/Application/Application/Pricing/Idempotency/PricingIdempotencyService.cs`
- Create: `src/Application/Application/Pricing/Idempotency/PricingIdempotencyResult.cs`
- Modify: `src/Application/Application/Pricing/PricingAppServiceDependencies.cs`
- Modify: `src/API/Program.cs`
- Modify: `src/Application/Application/Pricing/PricingAppService.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs`

### Task 4.1: 抽 AuthorityPriceChecker

- [ ] 创建 `AuthorityPriceChecker`。

职责：

- 从 `ValidateAuthorityPriceAsync` 迁移逻辑。
- 只依赖 `IPriceMasterRepository`、`IOptions<PricingOptions>`、`ILogger`。
- 不知道 confirm/simulate。

建议 API：

```csharp
public sealed class AuthorityPriceChecker
{
    public Task CheckAsync(IReadOnlyList<PricingCalculateItemRequest> items);
}
```

- [ ] 注册 DI。
- [ ] 替换 `PricingAppService` 调用。

验收：

- `PRICE_MISMATCH` 行为不变。
- 权威单价校验开关行为不变。

### Task 4.2: 抽 PricingIdempotencyService

- [ ] 创建 `PricingIdempotencyResult`。

建议模型：

```csharp
internal sealed record PricingIdempotencyResult(
    bool HasExisting,
    ChargeRequest? ExistingRequest,
    string Fingerprint);
```

- [ ] 创建 `PricingIdempotencyService`。

职责：

- 生成 confirm 指纹。
- 查询已有业务键。
- 判断同号参数是否一致。
- 事务外和事务内都可复用。

建议 API：

```csharp
public sealed class PricingIdempotencyService
{
    public Task<PricingIdempotencyResult> CheckConfirmAsync(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items);

    public void EnsureSameFingerprint(
        ChargeRequest existing,
        string fingerprint,
        string businessRequestNo);
}
```

注意：

- 不要在该服务中写入请求日志。
- 不要在该服务中构造 response。
- `BuildIdempotentResponse` 暂时仍可留在原服务或后续抽出。

### Task 4.3: 验证

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "PricingApiServiceTests"
```

重点确认：

- 重复 confirm 返回同一结果。
- 同一业务号参数变化返回 `IdempotencyConflict`。
- 单价不一致返回 `PriceMismatch`。

## Chunk 5: 抽出计价持久化写入器

**目标：** 把请求日志、步骤日志、折价明细、占额和冲正日志写入逻辑从主服务剥离。

**Files:**

- Create: `src/Application/Application/Pricing/Persistence/PricingRequestLogWriter.cs`
- Create: `src/Application/Application/Pricing/Persistence/PricingTraceStepWriter.cs`
- Create: `src/Application/Application/Pricing/Persistence/PricingDiscountDetailWriter.cs`
- Create: `src/Application/Application/Pricing/Persistence/PricingLimitOccupyWriter.cs`
- Create: `src/Application/Application/Pricing/Persistence/PricingReverseLogWriter.cs`
- Modify: `src/API/Program.cs`
- Modify: `src/Application/Application/Pricing/PricingAppService.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingReverseTests.cs`

### Task 5.1: 抽 PricingRequestLogWriter

- [ ] 迁移 `SaveRequestLog`、`SaveResponseJson`。
- [ ] 保持 `TraceId`、`RequestFingerprint`、`BusinessStatus`、`RequestJson`、`ResponseJson` 字段行为不变。
- [ ] 不在 writer 中开启事务，事务由 use case 管。

建议 API：

```csharp
public sealed class PricingRequestLogWriter
{
    public Task<ChargeRequest> SaveAsync(RequestLogSaveInput input);
    public Task SaveResponseJsonAsync(ChargeRequest log, PricingCalculateResponse response);
}
```

### Task 5.2: 抽 PricingTraceStepWriter

- [ ] 迁移 `SaveTraceSteps`。
- [ ] 保持 step no、trace id、request id 行为不变。

### Task 5.3: 抽 PricingDiscountDetailWriter

- [ ] 迁移 `SaveDiscountDetail`、`SaveChildDiscountDetails`。
- [ ] 保持普通项目也保存 commit 对账基准的行为不变。
- [ ] 保持 `ResultGroupNo`、`ChargeDetailNo`、`PartSeq` 行为不变。

### Task 5.4: 抽 PricingLimitOccupyWriter

- [ ] 迁移 `SaveLimitOccupies`、`InsertNegativeLimitOccupiesAsync`。
- [ ] 保持占额状态、过期时间、业务时间、明细身份字段不变。

### Task 5.5: 抽 PricingReverseLogWriter

- [ ] 迁移 reverse 请求日志保存和冲正日志写入。
- [ ] 保持 `ReverseNo` 幂等唯一键语义不变。

### Task 5.6: 验证

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "PricingApiServiceTests|PricingReverseTests|CoreBusinessCoverageTests"
```

验收：

- `PricingAppService` 只剩流程编排，持久化细节明显减少。
- 所有资金链路测试通过。

## Chunk 6: 拆分计价 UseCase

**目标：** 将 `PricingAppService` 降级为兼容门面，真正业务流程进入独立 use case。

**Files:**

- Create: `src/Application/Application/Pricing/UseCases/SimulatePricingUseCase.cs`
- Create: `src/Application/Application/Pricing/UseCases/ConfirmPricingUseCase.cs`
- Create: `src/Application/Application/Pricing/UseCases/CommitPricingUseCase.cs`
- Create: `src/Application/Application/Pricing/UseCases/CancelPricingUseCase.cs`
- Create: `src/Application/Application/Pricing/UseCases/ReversePricingUseCase.cs`
- Create: `src/Application/Application/Pricing/UseCases/GetSpecialFlagUseCase.cs`
- Modify: `src/Application/Application/Pricing/PricingAppService.cs`
- Modify: `src/API/Program.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingReverseTests.cs`

### Task 6.1: 抽 SimulatePricingUseCase

- [ ] 迁移 `SimulateAsync` 主流程。
- [ ] 保持试算不占额、但保存请求日志和步骤日志。
- [ ] 保持批量上下文行为。

建议 API：

```csharp
public sealed class SimulatePricingUseCase
{
    public Task<PricingCalculateResponse> ExecuteAsync(PricingCalculateRequest request);
}
```

### Task 6.2: 抽 ConfirmPricingUseCase

- [ ] 迁移 `ConfirmAsync` 主流程。
- [ ] 保持幂等事务外和事务内二次检查。
- [ ] 保持幂等锁键。
- [ ] 保持请求日志、步骤、明细、占额同事务。

### Task 6.3: 抽 CommitPricingUseCase

- [ ] 迁移 `CommitAsync` 主流程。
- [ ] 保持 commit 实际落账明细校验。
- [ ] 保持已 confirmed 幂等行为。
- [ ] 保持过期 confirm 不允许 commit。

### Task 6.4: 抽 CancelPricingUseCase

- [ ] 迁移 `CancelAsync` 主流程。
- [ ] 保持只允许 `CONFIRM_PENDING` cancel。
- [ ] 保持 cancelled/expired 幂等。

### Task 6.5: 抽 ReversePricingUseCase

- [ ] 迁移 `ReverseAsync` 主流程。
- [ ] 保持 `ReverseNo` 幂等。
- [ ] 保持超退校验。
- [ ] 保持全退和部分退费行为。
- [ ] 保持同组主子项目校验。

### Task 6.6: 抽 GetSpecialFlagUseCase

- [ ] 迁移 `GetSpecialFlagAsync`。
- [ ] 保持只统计当前有效已发布规则。
- [ ] 保持 `RollbackMode` 保守优先级。

### Task 6.7: 保留 PricingAppService 兼容门面

- [ ] `PricingAppService` 构造函数注入 6 个 use case。
- [ ] 原 public 方法仅委托 use case。
- [ ] 不改 MediatR handler。
- [ ] 不改 Controller。

验收：

- `PricingAppService` 目标行数小于 250 行。
- 所有 public 方法签名不变。

### Task 6.8: 验证

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "PricingApiServiceTests|PricingReverseTests|BatchPricingContextTests|CoreBusinessCoverageTests"
```

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore
```

## Chunk 7: 拆分规则发布门禁与状态机

**目标：** 将 `RulePublishAppService` 拆为门面、use case 和 guard，降低发布逻辑阅读难度。

**Files:**

- Create: `src/Application/Application/Rules/Publishing/PublishRuleUseCase.cs`
- Create: `src/Application/Application/Rules/Publishing/DisableRuleUseCase.cs`
- Create: `src/Application/Application/Rules/Publishing/RollbackRuleUseCase.cs`
- Create: `src/Application/Application/Rules/Publishing/RulePublishTransactionWriter.cs`
- Create: `src/Application/Application/Rules/Publishing/RulePublishCacheInvalidator.cs`
- Create: `src/Application/Application/Rules/Guards/RulePublishGuard.cs`
- Create: `src/Application/Application/Rules/Guards/RuleApprovalGate.cs`
- Create: `src/Application/Application/Rules/Guards/RuleConflictDetector.cs`
- Create: `src/Application/Application/Rules/Guards/RuleActionParameterValidator.cs`
- Create: `src/Application/Application/Rules/Guards/RuleTestCaseGate.cs`
- Create: `src/Application/Application/Rules/Guards/RuleCriticalActionGuard.cs`
- Create: `src/Application/Application/Rules/Guards/RuleChildItemGuard.cs`
- Modify: `src/Application/Application/Rules/RulePublishAppService.cs`
- Modify: `src/API/Program.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RulePublishConflictTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RuleApprovalAppServiceTests.cs`

### Task 7.1: 抽 RulePublishCacheInvalidator

- [ ] 从 `RulePublishAppService.ClearEffectiveCache` 抽出。
- [ ] 集中清理：
  - `EffectiveRuleCacheKeys`
  - `SpecialFlagCacheKeys`
  - `IRuleRuntimeCacheInvalidator`
  - `ICacheVersionSynchronizer`

建议 API：

```csharp
public sealed class RulePublishCacheInvalidator
{
    public Task InvalidateAfterRuleLifecycleChangedAsync();
}
```

### Task 7.2: 抽 RuleApprovalGate

- [ ] 迁移 `EnsureApprovalPassedAsync` 和 `GetLatestDraftChangeTimeAsync`。
- [ ] 保持审批缺失、审批过期、审批驳回错误码不变。

### Task 7.3: 抽 RuleTestCaseGate

- [ ] 迁移 `ValidateEnabledTestCasesAsync`。
- [ ] 保持缺测试用例、用例不完整、未运行、运行失败错误码不变。

### Task 7.4: 抽动作相关 Guard

- [ ] `RuleActionParameterValidator` 迁移 `ValidateActionParametersAsync`、`ValidateActionParameters`、JSON 参数解析。
- [ ] `RuleCriticalActionGuard` 迁移关键动作 `OnError = STOP` 校验。
- [ ] `RuleChildItemGuard` 迁移 `ADD_CHILD_ITEM` 子项校验。

### Task 7.5: 抽 RuleConflictDetector

- [ ] 迁移：
  - `ValidatePublishConflictsAsync`
  - `BuildRuleProfileAsync`
  - `BuildConditionScopes`
  - `HasForbiddenActionConflictAsync`
  - `GetMutuallyExclusiveActionsAsync`
- [ ] 将 `RuleConflictProfile`、`RuleConditionScope` 移到 `Profiles` 目录。

### Task 7.6: 抽 RulePublishGuard

- [ ] 新增聚合门禁类。
- [ ] 编排：
  - 审批门禁
  - 规则冲突
  - 动作参数
  - 测试用例
  - 关键动作失败策略
  - 子项重复

建议 API：

```csharp
public sealed class RulePublishGuard
{
    public Task EnsureCanPublishAsync(RuleAggregate header, int versionNo);
    public Task EnsureCanDisableAsync(long ruleId, int currentVersionNo);
    public Task EnsureCanRollbackAsync(long ruleId, int currentVersionNo);
}
```

### Task 7.7: 抽三个生命周期 UseCase

- [ ] `PublishRuleUseCase` 迁移 `PublishAsync` 状态机。
- [ ] `DisableRuleUseCase` 迁移 `DisableAsync` 状态机。
- [ ] `RollbackRuleUseCase` 迁移 `RollbackAsync` 状态机。
- [ ] 保持事务内 `FOR UPDATE` 和 CAS 行为不变。
- [ ] 保持发布流水和变更日志行为不变。

### Task 7.8: RulePublishAppService 变成兼容门面

- [ ] 原 public 方法签名不变。
- [ ] `GetPublishHistoryAsync`、`GetChangeLogsAsync` 可暂时保留。
- [ ] `PublishAsync`、`DisableAsync`、`RollbackAsync` 委托 use case。

验收：

- `RulePublishAppService` 目标行数小于 250 行。
- 规则发布相关测试全部通过。

### Task 7.9: 验证

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "RulePublishConflictTests|RuleApprovalAppServiceTests|RuleDefinitionTransactionTests|CacheVersionSynchronizerTests"
```

- [ ] 运行完整测试：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore
```

## Chunk 8: 统一异常处理入口

**目标：** 消除 `ExceptionHandlerMiddleware` 与 `GlobalExceptionFilter` 双入口长期漂移风险。

**Files:**

- Modify: `src/API/Middleware/ExceptionHandlerMiddleware.cs`
- Modify: `src/API/Filters/GlobalExceptionFilter.cs`
- Modify: `tests/Pricing.RuleCenter.Tests/GlobalExceptionFilterTests.cs`
- Optional Create: `src/API/Errors/ApiExceptionMapper.cs`
- Test: `tests/Pricing.RuleCenter.Tests/GlobalExceptionFilterTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/ControllerNotFoundTests.cs`

### Task 8.1: 抽 ApiExceptionMapper

- [ ] 创建 `src/API/Errors/ApiExceptionMapper.cs`。

职责：

- 把异常映射为：
  - HTTP 状态码
  - 业务 code
  - message
  - errors

建议 API：

```csharp
internal static class ApiExceptionMapper
{
    public static ApiErrorMapping Map(Exception exception);
}

internal sealed record ApiErrorMapping(
    int StatusCode,
    int Code,
    string Message,
    IReadOnlyDictionary<string, string[]>? Errors);
```

- [ ] `ExceptionHandlerMiddleware` 和 `GlobalExceptionFilter` 都调用该 mapper。

### Task 8.2: 决定正式入口

- [ ] 保留 `ExceptionHandlerMiddleware` 为正式入口。
- [ ] `GlobalExceptionFilter` 仅保留为兼容测试对象，内部调用同一 mapper。
- [ ] 在代码注释说明 `Program.cs` 使用 middleware。

### Task 8.3: 验证

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "GlobalExceptionFilterTests|ControllerNotFoundTests|PricingApiServiceTests"
```

验收：

- 两套入口映射结果一致。
- 未来新增错误码只改一个 mapper。

## Chunk 9: 引入统一时钟

**目标：** 收敛 `DateTime.Now`，提高过期、跨日、审批失效和发布流水测试稳定性。

**Files:**

- Create: `src/Domain/Interfaces/IClock.cs`
- Create: `src/Infrastructure/SystemClock.cs`
- Modify: `src/API/Program.cs` or `src/Infrastructure/DependencyInjection.cs`
- Modify selected files using technical time:
  - `src/Application/Application/Pricing/*`
  - `src/Application/Application/Rules/*`
  - `src/Application/Application/Background/ExpireCleanupService.cs`
  - `src/Application/Application/Background/CacheVersionSyncService.cs`
  - repositories only where they currently stamp technical time
- Test: existing tests plus targeted new tests

### Task 9.1: 新增 IClock

- [ ] 创建：

```csharp
namespace Pricing.RuleCenter.Core.Interfaces;

public interface IClock
{
    DateTime Now { get; }
}
```

- [ ] 创建 `SystemClock`：

```csharp
public sealed class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}
```

- [ ] 注册 DI。

### Task 9.2: 只替换技术时间

- [ ] 替换以下场景：
  - RequestAt
  - ResponseAt
  - CreatedAt
  - UpdatedAt
  - PublishedAt
  - ReviewedAt
  - ExpireAt
  - TraceId 时间戳

- [ ] 不替换业务时间：
  - `BusinessChargeTime`
  - 规则匹配生效时间入参
  - reverse 业务退费时间入参

注意：

- 业务时间必须继续以请求传入为准。
- 不能把 `BusinessChargeTime` 改成 `clock.Now`。

### Task 9.3: 测试

- [ ] 为过期判断增加固定时钟测试。
- [ ] 为审批过期判断增加固定时钟测试。

运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "Expire|Approval|PricingApiServiceTests|RulePublishConflictTests"
```

## Chunk 10: 规则能力矩阵

**目标：** 回答“现有规则引擎能否应对国家物价局未来价格规则”，把口号变成可验证矩阵。

**Files:**

- Create: `docs/物价折价改造方案文档/28-规则引擎能力矩阵.md`
- Optional Create: `src/Application/Application/Engine/Capability/RuleCapabilityCatalog.cs`
- Optional Test: `tests/Pricing.RuleCenter.Tests/RuleCapabilityCatalogTests.cs`

### Task 10.1: 编写能力矩阵文档

- [ ] 创建 `28-规则引擎能力矩阵.md`。

必须包含以下表格：

| 政策类型 | 当前是否可纯配置 | 使用条件原语 | 使用动作原语 | 是否需新增代码 | 风险 |
| --- | --- | --- | --- | --- | --- |
| 固定折扣 | 是 | ITEM/SCENE | FORMULA_CALC | 否 | 低 |
| 数量上限 | 是 | ITEM/SCENE | APPLY_*_LIMIT_QTY | 否 | 低 |
| 时间窗限制 | 是 | ITEM/SCENE/TIME | APPLY_TIME_WINDOW_LIMIT | 否 | 中 |
| 金额封顶 | 是 | ITEM/SCENE | APPLY_MAX_AMOUNT | 否 | 低 |
| 多部位换算 | 部分 | BODY_PART | CONVERT_QTY/CONVERT_QTY_BY_PART | 视规则 | 中 |
| 多肿物面积阶梯 | 部分 | PRICING_PARTS | AREA_STEP_INCREMENT | 视规则 | 中 |
| 主子项目加收 | 是 | ITEM/GROUP | ADD_CHILD_ITEM | 否 | 中 |
| 组合包计价 | 部分 | GROUP | 需组合原语 | 可能 | 高 |
| 病种/诊断计价 | 否或部分 | 需 DIAGNOSIS | 视规则 | 可能 | 高 |
| 医保身份差异 | 否或部分 | 需 INSURANCE_TYPE | 视规则 | 可能 | 高 |
| 设备型号差异 | 否或部分 | 需 DEVICE_TYPE | 视规则 | 可能 | 中 |
| 跨周期住院计价 | 否或部分 | 需 INPATIENT_PERIOD | 需周期原语 | 可能 | 高 |
| 政策追溯重算 | 否 | 需快照 | 需重算流程 | 是 | 高 |

- [ ] 对每一类写明：
  - 现有配置方式。
  - 缺失字段。
  - 缺失 evaluator。
  - 缺失 executor。
  - 是否影响上线。

### Task 10.2: 定义“一劳永逸”的工程标准

- [ ] 在文档中明确：

```text
本项目不承诺所有未来政策零代码。
本项目承诺：
1. 已有原语覆盖的政策通过配置解决。
2. 新政策如果只是新增条件维度，则新增 evaluator，不改主流程。
3. 新政策如果只是新增计算动作，则新增 executor，不改主流程。
4. 新政策如果只是新增公式表达，则优先通过表达式公式解决。
5. 渠道侧不再写价格规则。
```

验收：

- 文档能直接指导后续政策评估。
- 项目负责人能看懂“哪些能配置、哪些不能”。

## Chunk 11: 表达式公式引擎最小落地

**目标：** 将简单新公式从“新增 executor”降低为“配置表达式”。

**Files:**

- Create: `src/Application/Application/Engine/Formula/FormulaEvaluationContext.cs`
- Create: `src/Application/Application/Engine/Formula/FormulaExpressionEvaluator.cs`
- Create: `src/Application/Application/Engine/Formula/FormulaFunctionRegistry.cs`
- Create: `src/Application/Application/Engine/Formula/FormulaExpressionValidator.cs`
- Modify: `src/Application/Application/Engine/Executors/IncrementPercentExecutor.cs` or create new `ExpressionFormulaExecutor.cs`
- Modify: `src/API/Program.cs`
- Test: `tests/Pricing.RuleCenter.Tests/FormulaExpressionEvaluatorTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/ActionExecutionPipelineTests.cs`

### Task 11.1: 选择表达式策略

- [ ] 优先实现受控表达式，不直接执行 C# 脚本。
- [ ] 禁止任意方法调用。
- [ ] 仅允许白名单变量：
  - `inputQty`
  - `convertedQty`
  - `finalQty`
  - `unitPrice`
  - `originalAmount`
  - `finalAmount`
  - `partCount`
  - `area`
- [ ] 仅允许白名单函数：
  - `min(a,b)`
  - `max(a,b)`
  - `round(a,scale)`
  - `ceil(a)`
  - `floor(a)`

### Task 11.2: 编写表达式求值测试

- [ ] 新增 `FormulaExpressionEvaluatorTests`。

测试用例：

- `unitPrice * finalQty * 0.5`
- `min(unitPrice * finalQty, 440)`
- `max(unitPrice * finalQty, 10)`
- `round(unitPrice * finalQty / 3, 2)`
- 未知变量失败。
- 未知函数失败。
- 非法表达式失败。

### Task 11.3: 实现最小 evaluator

- [ ] 可以选择简单递归下降 parser，或使用项目已允许的安全表达式库。
- [ ] 如果引入新 NuGet 包，必须说明理由，并确认 .NET 6 兼容。
- [ ] 不允许 `DataTable.Compute` 承载生产规则，除非加严格白名单和测试，因为错误语义不够可控。

建议：

- 第一版只支持十进制、变量、括号、四则运算、函数调用。
- 所有数字转 `decimal`。
- 不支持字符串表达式。
- 不支持反射。

### Task 11.4: 新增 ExpressionFormulaExecutor

- [ ] 创建或扩展公式执行器，支持 `ExecutorCode = EXPRESSION_FORMULA`。
- [ ] 参数 JSON 示例：

```json
{
  "expression": "min(unitPrice * finalQty * 0.5, 440)",
  "amountField": "FinalAmount"
}
```

- [ ] 执行后写入 `FormulaAmount` 和 `FinalAmount`。
- [ ] 中间计算不提前取整，最终仍由 `PricingAmountRounder.RoundFinal` 统一处理。

### Task 11.5: 发布前校验表达式

- [ ] 在 `RuleActionParameterValidator` 中增加：
  - `EXPRESSION_FORMULA` 必须有 `expression`。
  - 表达式必须通过 `FormulaExpressionValidator`。
  - 禁止未知变量和未知函数。

### Task 11.6: 验证

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "FormulaExpressionEvaluatorTests|ActionExecutionPipelineTests|FormulaDefAppServiceTests|RulePublishConflictTests"
```

验收：

- 简单公式可以配置表达式解决。
- 旧公式 executor 行为不变。

## Chunk 12: 生效规则快照读取模型

**目标：** 降低运行期按规则逐条读取条件/动作的复杂度，为未来高并发和政策版本追溯打基础。

**Files:**

- Create: `src/Application/Application/Engine/RuleRuntimeSnapshot/EffectiveRuleSnapshot.cs`
- Create: `src/Application/Application/Engine/RuleRuntimeSnapshot/EffectiveRuleSnapshotLoader.cs`
- Create: `src/Application/Application/Engine/RuleRuntimeSnapshot/EffectiveRuleSnapshotCache.cs`
- Modify: `src/Application/Application/Engine/RuleMatchService.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RuleMatchServiceTests.cs`
- Test: `tests/Pricing.RuleCenter.Core.Tests/RuleMatchServiceGroupScopeTests.cs`

### Task 12.1: 定义 EffectiveRuleSnapshot

- [ ] 创建 snapshot 模型。

建议字段：

```csharp
internal sealed class EffectiveRuleSnapshot
{
    public RuleAggregate Header { get; init; }
    public IReadOnlyList<RuleCondition> Conditions { get; init; }
    public IReadOnlyList<RuleAction> Actions { get; init; }
}
```

### Task 12.2: 实现 Loader

- [ ] `EffectiveRuleSnapshotLoader` 负责按 itemCode 加载候选规则、条件、动作。
- [ ] 第一版可以仍调用现有仓储，但集中在 loader。
- [ ] 后续可优化为批量仓储查询。

### Task 12.3: 实现 Cache

- [ ] `EffectiveRuleSnapshotCache` 使用 `IMemoryCache`。
- [ ] 缓存 key 至少包含：
  - itemCode
  - 业务日期或时间粒度如果需要
  - cache version scope
- [ ] 发布、停用、回滚后通过现有缓存失效机制清理。

### Task 12.4: 改 RuleMatchService 使用 snapshot

- [ ] `RuleMatchService.MatchAsync` 从 snapshot 获取规则、条件和动作。
- [ ] 保持匹配逻辑和动作排序不变。

### Task 12.5: 验证

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "RuleMatchServiceTests|RuleMatchServiceGroupScopeTests|ActionExecutionPipelineTests"
```

验收：

- 测试行为不变。
- `RuleMatchService` 不再直接散落多次仓储读取条件和动作。

## Chunk 13: 规则原语缺口补齐

**目标：** 根据能力矩阵补最关键的通用条件原语，减少未来政策新增代码概率。

**Files:**

- Modify/Create evaluators under `src/Application/Application/Engine/Evaluators/`
- Modify DTO/context if needed:
  - `src/Domain/Models/PricingContext.cs`
  - `src/Application/Dto/PricingDto.cs`
- Modify: `src/API/Program.cs`
- Test: new evaluator tests

### Task 13.1: 评估是否补充医保身份条件

- [ ] 如果 HIS 能传医保身份，新增：
  - `InsuranceTypeMatchEvaluator`
  - `PricingCalculateRequest.InsuranceType` 或 `ExtraParams["insuranceType"]`
- [ ] 如果 HIS 暂不能稳定提供，先只在能力矩阵记录，不落代码。

### Task 13.2: 评估是否补充诊断/病种条件

- [ ] 如果 HIS 能传诊断编码，新增：
  - `DiagnosisMatchEvaluator`
  - 支持 `diagnosisCodes` 集合匹配。
- [ ] 如果涉及病种组合复杂规则，先作为高风险缺口记录。

### Task 13.3: 评估是否补充设备型号条件

- [ ] 如果价格政策按设备型号区分，新增：
  - `DeviceTypeMatchEvaluator`
  - 从 item extra params 或 request extra params 获取。

### Task 13.4: 验证

- [ ] 每新增一个 evaluator，必须补：
  - 命中测试。
  - 不命中测试。
  - 空配置表示不校验测试。
  - 未传入上下文时保守不命中或按业务约定处理测试。

运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "RuleMatchServiceTests|CoreBusinessCoverageTests"
```

## Chunk 14: 发布门禁增强为“能力可表达性校验”

**目标：** 不允许配置人员发布当前引擎无法可靠表达的规则。

**Files:**

- Create: `src/Application/Application/Rules/Guards/RuleCapabilityGuard.cs`
- Modify: `src/Application/Application/Rules/Guards/RulePublishGuard.cs`
- Modify: `src/Application/Dto/BizErrorCode.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RulePublishConflictTests.cs`

### Task 14.1: 新增 RuleCapabilityGuard

- [ ] 校验内容：
  - 未知 `ConditionType` 不允许发布。
  - 未知 `ActionType` 不允许发布。
  - 未知 `ExecutorCode` 不允许发布。
  - 表达式公式变量不在白名单不允许发布。
  - 配置需要 `PricingParts` 的动作，必须标记前端采集要求。

### Task 14.2: 新增错误码

- [ ] 在 `BizErrorCode` 增加：
  - `RuleCapabilityUnsupported`
  - `RuleConditionUnsupported`
  - `RuleActionUnsupported`
  - `RuleFormulaUnsupported`

### Task 14.3: 接入 RulePublishGuard

- [ ] 发布前调用 `RuleCapabilityGuard`。
- [ ] 停用/回滚不需要校验能力表达性，除非回滚目标规则缺失必要执行器。

### Task 14.4: 测试

- [ ] 新增发布失败用例：
  - 未知条件类型。
  - 未知动作类型。
  - 未知 executor code。
  - 表达式公式非法变量。

运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "RulePublishConflictTests"
```

## Chunk 15: Oracle 并发与事务验证脚本

**目标：** 为上线前证明锁和事务语义准备可执行验证，不只依赖单元测试。

**Files:**

- Create: `docs/物价折价改造方案文档/29-Oracle并发与事务验证方案.md`
- Optional Create: `sql/90-concurrency-verify.sql`
- Optional Create: `tests/Pricing.RuleCenter.OracleIntegration.Tests/` if environment allows

### Task 15.1: 编写验证方案

- [ ] 创建 `29-Oracle并发与事务验证方案.md`。

必须包含：

- 验证环境要求。
- 连接串配置方式。
- 并发 confirm 同业务号。
- 并发 confirm 同患者同项目时间窗。
- commit 与 expire 竞态。
- reverse 并发同 ReverseNo。
- 发布与保存条件/动作竞态。
- 审批并发通过/驳回。
- 唯一索引冲突转换。
- 事务中途失败回滚。

### Task 15.2: 可选 SQL 验证脚本

- [ ] 如果方便，创建 `sql/90-concurrency-verify.sql`，至少包含：
  - 检查唯一索引存在。
  - 检查锁表记录。
  - 检查 pending 超时记录。
  - 检查同业务键重复记录。
  - 检查同 reverse no 重复记录。

验收：

- 运维或开发能按文档在预发 Oracle 执行。

## Chunk 16: 文档和最终验证

**目标：** 完成整改记录、架构说明和最终测试。

**Files:**

- Modify: `docs/物价折价改造方案文档/27-src整改执行记录.md`
- Modify: `docs/物价折价改造方案文档/25-src设计全面分析报告.md` if final conclusion changed
- Optional Modify: `CLAUDE.md` if architecture instructions changed

### Task 16.1: 更新执行记录

- [ ] 在 `27-src整改执行记录.md` 填写：
  - 每个 Chunk 完成状态。
  - 每个 Chunk 测试命令和结果。
  - 未完成项。
  - 行为兼容性说明。
  - 新增能力说明。

### Task 16.2: 最终测试

- [ ] 运行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore
```

Expected：

- 全部测试通过。

- [ ] 运行：

```powershell
git diff --check
```

Expected：

- 无输出。

- [ ] 运行：

```powershell
git status --short --branch
```

Expected：

- 只出现本计划涉及的代码、测试、文档变更。

## 4. 验收总清单

### 4.1 可读性验收

- [ ] `PricingAppService` 变为薄门面，目标小于 250 行。
- [ ] `RulePublishAppService` 变为薄门面，目标小于 250 行。
- [ ] confirm、commit、cancel、reverse 各有独立 use case。
- [ ] 发布、停用、回滚各有独立 use case。
- [ ] 发布门禁拆成独立 guard，可单独测试。
- [ ] 权威单价、幂等、响应构造、上下文构造、持久化写入均从主流程抽离。

### 4.2 行为兼容验收

- [ ] confirm 幂等行为不变。
- [ ] confirm 同业务号参数变化仍拒绝。
- [ ] simulate 不占额。
- [ ] commit 必须校验实际落账明细。
- [ ] cancel 不允许处理已落账记录。
- [ ] reverse 不允许超退。
- [ ] reverse 同 `ReverseNo` 幂等。
- [ ] expire 只处理仍为 `CONFIRM_PENDING` 的记录。
- [ ] 规则动作顺序不变。
- [ ] 公式仍使用限制后的 `FinalQty`。
- [ ] 最终金额仍统一 2 位四舍五入。

### 4.3 规则引擎能力验收

- [ ] 完成 `28-规则引擎能力矩阵.md`。
- [ ] 明确哪些政策纯配置可表达。
- [ ] 明确哪些政策需要新增 evaluator。
- [ ] 明确哪些政策需要新增 executor。
- [ ] 表达式公式支持基础四则运算、变量、白名单函数。
- [ ] 发布前能阻断未知条件、未知动作、未知 executor。
- [ ] 新增规则原语不需要修改 Controller 和 HIS 客户端。

### 4.4 测试验收

- [ ] 完整测试通过：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore
```

- [ ] 格式检查通过：

```powershell
git diff --check
```

- [ ] 新增测试覆盖：
  - 计价 use case 拆分后行为。
  - 规则发布 guard。
  - 表达式公式。
  - 能力门禁。
  - 新增 evaluator。

## 5. 风险控制

### 5.1 最大风险

最大风险不是拆类本身，而是在拆分过程中不小心改变资金语义。尤其注意：

- confirm 事务内二次幂等检查不能丢。
- 限额锁不能移到事务外无效位置。
- commit/cancel/reverse/expire 必须继续同步更新三类资金表。
- reverse 全退和部分退费不能混淆。
- 规则动作全局顺序不能被类拆分改变。

### 5.2 回滚策略

每个 Chunk 必须是可独立回滚的：

- Chunk 2-9 属于结构整改，失败时回滚该 Chunk，不应影响规则引擎增强。
- Chunk 10 是文档能力矩阵，低风险。
- Chunk 11 表达式公式引擎可通过不注册 executor 或不发布对应动作来禁用。
- Chunk 12 快照读取如果出现问题，应保留开关回退到原 `RuleMatchService` 读取模式。
- Chunk 13 新 evaluator 可不注册或不配置条件类型来回退。
- Chunk 14 能力门禁如果误伤历史规则，应先以 warn/report 模式运行，再切 hard block。

## 6. 推荐提交顺序

如果后续执行时需要拆提交，建议：

1. `Add src remediation execution record`
2. `Unify pricing request validation`
3. `Extract pricing builders`
4. `Extract pricing idempotency and authority price checks`
5. `Extract pricing persistence writers`
6. `Split pricing application use cases`
7. `Split rule publish guards and use cases`
8. `Unify API exception mapping`
9. `Introduce application clock`
10. `Add rule engine capability matrix`
11. `Add expression formula evaluation`
12. `Add effective rule snapshot loading`
13. `Add missing rule condition primitives`
14. `Add rule capability publish gate`
15. `Add Oracle concurrency verification plan`
16. `Update remediation execution record`

## 7. 完成定义

本计划完成的定义：

- 代码结构明显可读，核心大类拆成用例和组件。
- 现有资金安全行为全部有测试保护且全部通过。
- 规则引擎能力边界被文档化，不再用“一劳永逸”这类不可验证目标描述。
- 新增常见公式可以通过表达式配置。
- 无法表达的政策能在发布前或能力矩阵中明确暴露，而不是运行期错算。
- 后续国家、省、市物价政策变更时，团队可以按“配置 -> 表达式 -> 新 evaluator -> 新 executor”的顺序评估和落地。
