# 纯直接规则读链路改造 Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 把 `PricingController` 相关计价与特殊项目判断链路，从“运行时包优先”收敛为“纯直接规则读模型”。

**Architecture:** 先切计价与 special-flag 的读链路，再清理响应/持久化里的运行时包字段，最后恢复旧规则维护入口并退役运行时包平台侧接口。改造按批次执行，每批都要求可以独立编译、跑通测试并具备回退点。

**Tech Stack:** .NET 8, ASP.NET Core, SqlSugar, Oracle 11g, xUnit

---

## Chunk 1: 批次一 - 计价读链路切回直接规则

### Task 1: 删除规则引擎中的运行时包优先读模型

**Files:**
- Modify: `src/Pricing.RuleCenter.Application/Application/Engine/RuleMatchRepositories.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Engine/RuleRuntimeSnapshot/EffectiveRuleSnapshotLoader.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/ActiveRuntimePackageReader.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimePackageTraceResolver.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimePackageTraceResolution.cs`
- Test: `tests/Pricing.RuleCenter.Tests/ActiveRuntimePackageReaderTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RuntimePackageTraceResolverTests.cs`

- [ ] **Step 1: 写失败测试，锁定“规则匹配只读 PR_RULE_*”**

在 `tests/Pricing.RuleCenter.Tests/ActiveRuntimePackageReaderTests.cs` 和 `tests/Pricing.RuleCenter.Tests/RuntimePackageTraceResolverTests.cs` 中，把运行时包读取断言改为“不再参与计价主链路”；新增或调整测试，验证 `EffectiveRuleSnapshotLoader` 在计价读链路中只走 `Header/Condition/Action` 仓储。

- [ ] **Step 2: 跑测试确认当前实现失败**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~ActiveRuntimePackageReaderTests|FullyQualifiedName~RuntimePackageTraceResolverTests"
```

Expected:
- 至少一条测试因运行时包读取仍存在而失败

- [ ] **Step 3: 最小实现删除运行时包读链路**

实现要点：

- `RuleMatchRepositories` 删除 `IRuntimePackageStateRepository` / `IRuntimeRuleReadRepository`
- `EffectiveRuleSnapshotLoader.LoadCurrentAsync` 删除 `_runtimePackageReader` 分支，只保留 `PR_RULE_*`
- 删除 `ActiveRuntimePackageReader`、`RuntimePackageTraceResolver`、`RuntimePackageTraceResolution`

- [ ] **Step 4: 跑测试确认通过**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~ActiveRuntimePackageReaderTests|FullyQualifiedName~RuntimePackageTraceResolverTests"
```

Expected:
- PASS

### Task 2: 删除 simulate/confirm 中的运行时包解析

**Files:**
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/Workflows/PricingSimulateWorkflow.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/Workflows/PricingConfirmWorkflow.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs`

- [ ] **Step 1: 写失败测试**

在 `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs` 增加/调整测试，验证：

- `simulate`
- `confirm`

在直接规则模式下不再解析 `RuntimePackageId/RuntimePackageVersion`。

- [ ] **Step 2: 跑测试确认失败**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~PricingApiServiceTests"
```

Expected:
- 与 `runtime package` 相关断言失败

- [ ] **Step 3: 最小实现**

实现要点：

- 删除两个 workflow 中的 `RuntimePackageTraceResolver` 依赖
- 删除 `ResolveAsync(calculations)` 调用
- `PersistAsync` 输入不再传 `RuntimeTrace`

- [ ] **Step 4: 跑测试确认通过**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~PricingApiServiceTests"
```

Expected:
- PASS

## Chunk 2: 批次二 - 特殊项目判断与 DTO/追溯字段清理

### Task 3: 让 special-flag 只读旧规则表

**Files:**
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/PricingSpecialFlagResolver.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs`

- [ ] **Step 1: 写失败测试**

新增/调整测试，验证 `special-flag` 不再依赖激活包，直接按 `IRuleHeaderRepository` 返回结果。

- [ ] **Step 2: 跑测试确认失败**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~PricingApiServiceTests.GetSpecialFlag"
```

Expected:
- 当前逻辑仍优先运行时包，测试失败

- [ ] **Step 3: 最小实现**

实现要点：

- 删除 `RuntimePackageTraceResolver` 与 `EffectiveRuleSnapshotLoader` 的运行时包路径
- 删除 `ResolveFromRuntimePackageAsync` / `ResolveFromRuntimeSnapshotSetAsync`
- `ResolveAsync` 统一使用 `IRuleHeaderRepository.GetByItemCodeAsync`

- [ ] **Step 4: 跑测试确认通过**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~PricingApiServiceTests.GetSpecialFlag"
```

Expected:
- PASS

### Task 4: 清理响应 DTO 与持久化写入中的运行时包字段

**Files:**
- Modify: `src/Pricing.RuleCenter.Application/Dto/PricingResponseDto.cs`
- Modify: `src/Pricing.RuleCenter.Application/Dto/PricingSpecialFlagDto.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/Builders/PricingResponseBuilder.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/Persistence/PricingRequestLogWriter.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/Persistence/PricingTraceStepWriter.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Pricing/Persistence/PricingDiscountDetailWriter.cs`
- Test: `tests/Pricing.RuleCenter.Tests/PricingApiServiceTests.cs`

- [ ] **Step 1: 写失败测试**

把当前针对 `RuntimePackageId/RuntimePackageVersion/MatchedRuntimeRuleIds/MatchedPolicyVersionIds/MatchedTemplateVersionIds` 的断言改成“字段已移除或始终为空”的目标行为。

- [ ] **Step 2: 跑测试确认失败**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~PricingApiServiceTests"
```

Expected:
- 与响应/日志中运行时包字段相关断言失败

- [ ] **Step 3: 最小实现**

实现要点：

- DTO 删除运行时包字段
- ResponseBuilder 删除运行时包相关投影
- RequestLog/TraceStep/DiscountDetail 写入器删除运行时包字段赋值

- [ ] **Step 4: 跑测试确认通过**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~PricingApiServiceTests"
```

Expected:
- PASS

## Chunk 3: 批次三 - 启动注册与旧规则维护入口恢复

### Task 5: 收缩启动注册中的运行时包读依赖

**Files:**
- Modify: `src/Pricing.RuleCenter.Api/Startup/RuleCenterApiServiceCollectionExtensions.cs`
- Modify: `src/Pricing.RuleCenter.Infrastructure/DependencyInjection.cs`
- Test: `tests/Pricing.RuleCenter.Tests/ProjectReleaseGateTests.cs`

- [ ] **Step 1: 写失败测试**

新增/调整启动相关测试，验证 DI 不再注册：

- `IRuntimePackageStateRepository`
- `IRuntimeRuleReadRepository`
- `RuntimePackageTraceResolver`

用于计价主链路。

- [ ] **Step 2: 跑测试确认失败**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~ProjectReleaseGateTests"
```

Expected:
- 因仍存在注册而失败

- [ ] **Step 3: 最小实现**

实现要点：

- 删除计价主链路中的运行时包注册
- 保持应用可启动

- [ ] **Step 4: 跑测试确认通过**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~ProjectReleaseGateTests"
```

Expected:
- PASS

### Task 6: 恢复旧规则写维护入口

**Files:**
- Modify: `src/Pricing.RuleCenter.Api/Security/LegacyRuleAuthoringGuardFilter.cs`
- Modify: `src/Pricing.RuleCenter.Api/Controllers/RuleHeaderController.cs`
- Modify: `src/Pricing.RuleCenter.Api/Controllers/RuleVersionController.cs`
- Modify: `src/Pricing.RuleCenter.Api/Controllers/RuleConditionController.cs`
- Modify: `src/Pricing.RuleCenter.Api/Controllers/RuleActionController.cs`
- Modify: `src/Pricing.RuleCenter.Api/Controllers/RuleApprovalController.cs`
- Modify: `src/Pricing.RuleCenter.Api/Controllers/RulePublishController.cs`
- Test: `tests/Pricing.RuleCenter.Tests/ApiSecurityIntegrationTests.cs`

- [ ] **Step 1: 写失败测试**

调整测试，验证旧规则写接口不再默认返回 `410 Gone`。

- [ ] **Step 2: 跑测试确认失败**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~ApiSecurityIntegrationTests"
```

Expected:
- 因 `LegacyRuleAuthoringGuardFilter` 仍拦截而失败

- [ ] **Step 3: 最小实现**

实现要点：

- 删除或放开 `LegacyRuleAuthoringGuardFilter`
- 移除旧规则写控制器上的 `ServiceFilter`

- [ ] **Step 4: 跑测试确认通过**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~ApiSecurityIntegrationTests"
```

Expected:
- PASS

## Chunk 4: 批次四 - 平台侧运行时包退役

### Task 7: 删除 RuntimePackageController 及控制器级入口

**Files:**
- Delete: `src/Pricing.RuleCenter.Api/Controllers/RuntimePackageController.cs`
- Modify: `src/Pricing.RuleCenter.Api/Startup/RuleCenterApiServiceCollectionExtensions.cs`
- Test: `tests/Pricing.RuleCenter.Tests\ControllerNotFoundTests.cs`

- [ ] **Step 1: 写失败测试**

新增/调整测试，验证 `/api/pricing/runtime-packages/*` 不再暴露。

- [ ] **Step 2: 跑测试确认失败**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~ControllerNotFoundTests"
```

Expected:
- 因控制器仍存在而失败

- [ ] **Step 3: 最小实现**

实现要点：

- 删除控制器
- 删除应用层注册与 Swagger 分组引用

- [ ] **Step 4: 跑测试确认通过**

Run:
```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter "FullyQualifiedName~ControllerNotFoundTests"
```

Expected:
- PASS

### Task 8: 清理运行时包平台侧服务与死代码

**Files:**
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimePackageActivationService.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimePackageBuildContext.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimePackageBuildResult.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimePackageCompiler.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimePackagePublishService.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimePackageQueryAppService.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimePackageRollbackService.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimeRuleProjectionAdapter.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimeRuleProjectionFactory.cs`
- Delete: `src/Pricing.RuleCenter.Application/Application/RuntimePackages/RuntimeRuleSnapshot.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Policies/PolicyConflictService.cs`
- Modify: `src/Pricing.RuleCenter.Application/Application/Policies/IPolicyConflictService.cs`
- Modify: `src/Pricing.RuleCenter.Infrastructure/DependencyInjection.cs`
- Test: `tests/Pricing.RuleCenter.Tests\ProjectReleaseGateTests.cs`

- [ ] **Step 1: 写失败测试**

增加回归测试，验证构建时不存在运行时包平台类的强依赖。

- [ ] **Step 2: 跑测试确认失败**

Run:
```powershell
dotnet build src\Pricing.RuleCenter.slnx --no-restore
```

Expected:
- 因仍有平台侧服务引用而失败

- [ ] **Step 3: 最小实现**

实现要点：

- 删除整套运行时包平台侧服务
- 清理策略冲突接口里对 `RuntimeRuleSnapshot` 的依赖，必要时改为直接规则或更通用结构
- 删除 Infrastructure 中对应仓储注册

- [ ] **Step 4: 跑构建与关键测试确认通过**

Run:
```powershell
dotnet build src\Pricing.RuleCenter.slnx --no-restore
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore
dotnet test tests\Pricing.RuleCenter.Core.Tests\Pricing.RuleCenter.Core.Tests.csproj --no-restore
```

Expected:
- 全部 PASS

## 执行顺序建议

1. 先执行 Chunk 1，保证收费主链路完全切到直接规则。
2. 再执行 Chunk 2，清掉 special-flag 和响应/落库里的运行时包痕迹。
3. 再执行 Chunk 3，恢复旧规则写维护入口。
4. 最后执行 Chunk 4，退役整套运行时包平台侧代码。

## 回滚策略

- Chunk 1~3 之间允许逐批回滚，每批都保持可编译和可运行。
- Chunk 4 必须在确认旧规则维护入口恢复、计价主链路已稳定后再执行。

Plan complete and saved to `docs/superpowers/plans/2026-06-12-direct-rule-read-migration.md`. Ready to execute?
