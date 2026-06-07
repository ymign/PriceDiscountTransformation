# Config-First Pricing Rule Platform Implementation Plan

> 说明：当前仓库中的新 `src/` 仍处于开发阶段，本计划默认允许推翻旧 `Rule + Condition + Action` 作者模型；只保留已验证有价值的计价内核、执行器体系和资金安全约束。

**Goal:** 用 `Template -> PolicyVersion -> RuntimePackage` 重建规则平台，让业务维护策略，运行时只执行已激活规则包。

**Implementation stance:** 不做旧作者模型双写，不再新增基于 `PR_RULE_*` 的作者功能；旧规则表只作为迁移输入和历史参考。

**Tech Stack:** .NET 6, ASP.NET Core Web API, xUnit, SqlSugar, Oracle 11g, WinForms HIS client.

---

## Chunk 1: Schema, domain contracts, and traceability baseline

**Files:**
- Create: `sql/06-config-first-authoring-runtime-schema.sql`
- Modify: `sql/99-verify.sql`
- Create: `src/Domain/Aggregates/Templates/TemplateAggregate.cs`
- Create: `src/Domain/Aggregates/Templates/TemplateVersion.cs`
- Create: `src/Domain/Aggregates/Templates/TemplateParamDef.cs`
- Create: `src/Domain/Aggregates/Templates/TemplateStepDef.cs`
- Create: `src/Domain/Aggregates/Templates/TemplateScopeDef.cs`
- Create: `src/Domain/Aggregates/Policies/PolicyAggregate.cs`
- Create: `src/Domain/Aggregates/Policies/PolicyVersion.cs`
- Create: `src/Domain/Aggregates/Policies/PolicyBinding.cs`
- Create: `src/Domain/Aggregates/Policies/PolicyScope.cs`
- Create: `src/Domain/Aggregates/Policies/PolicyParam.cs`
- Create: `src/Domain/Aggregates/Policies/PolicyReview.cs`
- Create: `src/Domain/Aggregates/Runtime/RuntimePackage.cs`
- Create: `src/Domain/Aggregates/Runtime/RuntimePackagePolicy.cs`
- Create: `src/Domain/Aggregates/Runtime/RuntimeRule.cs`
- Create: `src/Domain/Aggregates/Runtime/RuntimeCondition.cs`
- Create: `src/Domain/Aggregates/Runtime/RuntimeAction.cs`
- Create: `src/Domain/Aggregates/Runtime/RuntimePackageState.cs`
- Create: `src/Domain/Constants/TemplateCapabilityCodes.cs`
- Create: `src/Domain/Constants/PolicyLifecycleCodes.cs`
- Create: `src/Domain/Constants/RuntimePackageStatusCodes.cs`
- Create: `src/Domain/Interfaces/Templates/ITemplateRepository.cs`
- Create: `src/Domain/Interfaces/Policies/IPolicyRepository.cs`
- Create: `src/Domain/Interfaces/Policies/IPolicyReviewRepository.cs`
- Create: `src/Domain/Interfaces/Runtime/IRuntimePackageRepository.cs`
- Create: `src/Domain/Interfaces/Runtime/IRuntimePackageStateRepository.cs`
- Create: `src/Domain/Interfaces/Runtime/IRuntimeRuleReadRepository.cs`
- Create: `src/Domain/Interfaces/Runtime/IRuntimeRuleBuildRepository.cs`
- Modify: `src/Domain/Aggregates/Charging/ChargeRequest.cs`
- Modify: `src/Domain/Aggregates/Charging/ChargeTraceStep.cs`
- Modify: `src/Domain/Aggregates/Charging/ChargeDiscountDetail.cs`
- Modify: `src/Infrastructure/Database/EntityTypeConfigs.cs`
- Test: `tests/Pricing.RuleCenter.Tests/SqlSugarEntityTypeConfigTests.cs`

- [ ] Add the new Oracle DDL for `PR_TEMPLATE*`, `PR_POLICY*`, `PR_RUNTIME_PACKAGE*`, plus new trace columns on charge request, trace step, and discount detail tables.
- [ ] Define the new domain aggregates and status constants without depending on the old rule authoring model.
- [ ] Add repository contracts for template, policy review, runtime package state, runtime read model, and runtime bulk-build writes.
- [ ] Extend SqlSugar mapping so all new entities and new trace columns map cleanly to Oracle naming conventions.
- [ ] Update entity mapping tests to assert the new tables and the added runtime trace fields.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter SqlSugarEntityTypeConfigTests`

## Chunk 2: Active package runtime reader and engine cutover

**Files:**
- Create: `src/Application/Application/RuntimePackages/ActiveRuntimePackageReader.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimeRuleSnapshot.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimeRuleProjectionAdapter.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimeRuleSelector.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageLocalCache.cs`
- Create: `src/Infrastructure/Repositories/Runtime/RuntimePackageStateRepository.cs`
- Create: `src/Infrastructure/Repositories/Runtime/RuntimeRuleReadRepository.cs`
- Modify: `src/Application/Application/Engine/RuleMatchRepositories.cs`
- Modify: `src/Application/Application/Engine/RuleMatchService.cs`
- Modify: `src/Application/Application/Engine/RuleRuntimeSnapshot/EffectiveRuleSnapshotLoader.cs`
- Modify: `src/API/Program.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RuleMatchServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/ActiveRuntimePackageReaderTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimeRuleSelectorTests.cs`

- [ ] Write failing tests for active package loading, candidate runtime rule reading, and package-based rule selection.
- [ ] Introduce `IRuntimeRuleReadRepository` as the only runtime source for candidate rules; do not allow the engine to keep reading `IRuleHeaderRepository` directly.
- [ ] Adapt `EffectiveRuleSnapshotLoader` so it loads from runtime package projections rather than old authoring rows.
- [ ] Keep the existing execution pipeline and executors, but feed them through a runtime projection adapter.
- [ ] Update dependency injection so the pricing engine runtime path resolves through the active package reader.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter "ActiveRuntimePackageReaderTests|RuntimeRuleSelectorTests|RuleMatchServiceTests"`

## Chunk 3: Compiler, conflict detection, candidate package build

**Files:**
- Create: `src/Application/Application/RuntimePackages/RuntimePackageCompiler.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageBuildContext.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageBuildResult.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimeRuleProjectionFactory.cs`
- Create: `src/Application/Application/Policies/PolicyPriorityKeyFactory.cs`
- Create: `src/Application/Application/Policies/PolicyConflictService.cs`
- Create: `src/Application/Application/Policies/PolicyValidationService.cs`
- Create: `src/Infrastructure/Repositories/Runtime/RuntimePackageRepository.cs`
- Create: `src/Infrastructure/Repositories/Runtime/RuntimeRuleBuildRepository.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RulePublishConflictTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackageCompilerTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyPriorityKeyFactoryTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyValidationServiceTests.cs`

- [ ] Write failing tests for priority key generation, single-winner conflict blocking, weak-expression validation, and full candidate package compilation.
- [ ] Implement `PolicyPriorityKeyFactory` with explicit binding rank, scope owner rank, specificity score, dimension tie-breaker, manual priority, and version fallback.
- [ ] Implement `PolicyConflictService` so conflicts are blocked during candidate package build, not deferred to runtime.
- [ ] Implement `RuntimePackageCompiler` as a full-package builder that writes `BUILT` candidate packages without activating them.
- [ ] Persist package-to-policy lineage in `PR_RUNTIME_PACKAGE_POLICY` for diff, rollback, and tracing.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter "RuntimePackageCompilerTests|PolicyPriorityKeyFactoryTests|PolicyValidationServiceTests|RulePublishConflictTests"`

## Chunk 4: Publish, review, activation, rollback, and cache invalidation

**Files:**
- Create: `src/Application/Application/RuntimePackages/RuntimePackagePublishService.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageActivationService.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageRollbackService.cs`
- Create: `src/Application/Application/Policies/PolicyReviewAppService.cs`
- Create: `src/Application/Application/Policies/PolicyPublishProfileResolver.cs`
- Create: `src/Application/Application/Policies/PolicyPublishEligibilityService.cs`
- Create: `src/Application/Application/Policies/PolicyPackageDiffService.cs`
- Modify: `src/Application/Application/Background/CacheVersionSynchronizer.cs`
- Modify: `src/Application/Application/Background/CacheVersionSyncService.cs`
- Modify: `src/Infrastructure/DependencyInjection.cs`
- Modify: `src/API/Program.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackagePublishServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackageActivationServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackageRollbackServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyReviewAppServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyPackageDiffServiceTests.cs`

- [ ] Write failing tests for direct publish, approval-required publish, activation pointer switching, rollback to historical built package, and cache invalidation outbox emission.
- [ ] Make the approval object `PolicyVersion`, not runtime rules and not template versions.
- [ ] Make the activation object `RuntimePackage`; activation must only switch the active package pointer and related cache state.
- [ ] Ensure activation and rollback update `PR_RUNTIME_PACKAGE_STATE`, package statuses, cache version, and invalidation outbox inside one UOW.
- [ ] Provide package diff output between the current active package and a built candidate package before activation.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter "RuntimePackagePublishServiceTests|RuntimePackageActivationServiceTests|RuntimePackageRollbackServiceTests|PolicyReviewAppServiceTests|PolicyPackageDiffServiceTests"`

## Chunk 5: Template and policy authoring services and API

**Files:**
- Create: `src/Application/Dto/TemplateDto.cs`
- Create: `src/Application/Dto/PolicyDto.cs`
- Create: `src/Application/Application/Templates/TemplateAppService.cs`
- Create: `src/Application/Application/Templates/TemplateVersionAppService.cs`
- Create: `src/Application/Application/Policies/PolicyAppService.cs`
- Create: `src/Application/Application/Policies/PolicyVersionAppService.cs`
- Create: `src/Application/Application/Policies/PolicyPreviewAppService.cs`
- Create: `src/Application/Application/Policies/PolicyExpressionGuard.cs`
- Create: `src/Infrastructure/Repositories/Templates/TemplateRepository.cs`
- Create: `src/Infrastructure/Repositories/Policies/PolicyRepository.cs`
- Create: `src/Infrastructure/Repositories/Policies/PolicyReviewRepository.cs`
- Create: `src/API/Controllers/TemplateController.cs`
- Create: `src/API/Controllers/PolicyController.cs`
- Create: `src/API/Controllers/RuntimePackageController.cs`
- Modify: `src/API/Program.cs`
- Create: `tests/Pricing.RuleCenter.Tests/TemplateAppServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyAppServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyPreviewAppServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/TemplateControllerRouteTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyControllerRouteTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackageControllerRouteTests.cs`

- [ ] Write failing tests for template creation/versioning, policy draft save, preview generation, review submit, build candidate package, activate package, and rollback endpoints.
- [ ] Expose template management, policy management, preview, review, candidate package build, activation, history, and rollback APIs.
- [ ] Keep weak-expression checks in the application layer; do not expose raw executor code or params JSON in policy APIs.
- [ ] Do not add new capabilities to the old `RuleHeader/RuleVersion/RuleCondition/RuleAction` authoring controllers.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter "TemplateAppServiceTests|PolicyAppServiceTests|PolicyPreviewAppServiceTests|TemplateControllerRouteTests|PolicyControllerRouteTests|RuntimePackageControllerRouteTests"`

## Chunk 6: Historical import, runtime tracing, and HIS workbench cutover

**Files:**
- Create: `sql/07-seed-template-catalog.sql`
- Create: `sql/08-import-initial-policies.sql`
- Create: `src/Application/Application/Policies/PolicyImportService.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageTraceResolver.cs`
- Modify: `src/Application/Application/Pricing/Persistence/PricingRequestLogWriter.cs`
- Modify: `src/Application/Application/Pricing/Persistence/PricingTraceStepWriter.cs`
- Modify: `src/Application/Application/Pricing/Persistence/PricingDiscountDetailWriter.cs`
- Modify: `his-client/PricingRuleDtos.cs`
- Modify: `his-client/PricingApiClient.cs`
- Modify: `his-client/FrmPricingRuleWorkbench.cs`
- Create: `his-client/FrmPolicyPublishCenter.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyImportServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackageTraceResolverTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/HisClientPolicyWorkbenchTests.cs`
- Modify: `docs/物价折价改造方案文档/07-开发任务清单-物价折价改造.md`
- Modify: `docs/物价折价改造方案文档/14-测试计划.md`

- [ ] Write failing tests for importing old rules into policy drafts, persisting runtime package identifiers on pricing requests, and tracing source template/policy/rule lineage in steps and discount details.
- [ ] Seed the initial template catalog for the first-batch high-frequency rule families.
- [ ] Implement one-way import from curated historical rules into `PolicyVersion` drafts; do not maintain dual-write back to the old `PR_RULE_*` tables.
- [ ] Update the HIS workbench flow to use template selection, business parameter entry, preview, candidate build, activation, and rollback views.
- [ ] Update rollout and test-plan docs to reflect package build, activation, rollback, import, and legacy authoring cutover checkpoints.
- [ ] Run: `dotnet build src\Pricing.RuleCenter.slnx --no-restore`
- [ ] Run: `dotnet test src\Pricing.RuleCenter.slnx --no-restore`

## Chunk 7: Legacy authoring retirement and cleanup

**Files:**
- Modify: `src/API/Program.cs`
- Modify: `src/API/Controllers/PricingController.cs`
- Modify: `src/Application/Application/Rules/RuleHeaderAppService.cs`
- Modify: `src/Application/Application/Rules/RuleVersionAppService.cs`
- Modify: `src/Application/Application/Rules/RuleConditionAppService.cs`
- Modify: `src/Application/Application/Rules/RuleActionAppService.cs`
- Modify: `src/Application/Application/Rules/RuleApprovalAppService.cs`
- Modify: `src/Application/Application/Rules/RulePublishAppService.cs`
- Modify: `tests/Pricing.RuleCenter.Tests/ApiDocumentationIntegrationTests.cs`
- Modify: `tests/Pricing.RuleCenter.Tests/ProjectReleaseGateTests.cs`

- [ ] Remove or feature-flag old rule authoring endpoints from the primary API surface once the new template/policy/package APIs are green.
- [ ] Keep pricing runtime endpoints stable, but cut their runtime data source fully to active runtime packages.
- [ ] Update Swagger grouping and release-gate tests so the new platform is the default authoring path.
- [ ] Commit the chunk only after the full solution test suite is green.

## Recommended execution notes

- Start with Chunk 1 and Chunk 2 before touching any new authoring UI.
- Do not build the HIS publish center before candidate package build and activation are already working.
- Treat the existing `Rule*` authoring services as migration-era code; do not deepen that model.
- Prefer full-package rebuilds in v1. Incremental compile can wait until production usage proves it necessary.
- Keep the current executor set intact until the new runtime package reader is stable.
