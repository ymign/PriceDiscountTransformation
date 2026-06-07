# Config-First Pricing Rule Platform Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Rebuild the pricing rule center around `Template -> PolicyVersion -> RuntimePackage` so business users maintain strategies while the engine executes only compiled runtime packages.

**Architecture:** Keep the existing pricing execution core, but replace the current authoring model with separate template and policy models. Compile publishable policy versions into immutable runtime packages, then route all runtime rule loading through the active package reader instead of directly reading hand-maintained rule tables.

**Tech Stack:** .NET 6, ASP.NET Core Web API, xUnit, SqlSugar, Oracle 11g, WinForms HIS client.

---

## Chunk 1: Runtime package schema and core domain model

**Files:**
- Create: `sql/06-config-first-authoring-runtime-schema.sql`
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
- Create: `src/Domain/Interfaces/Runtime/IRuntimePackageRepository.cs`
- Modify: `src/Infrastructure/Database/EntityTypeConfigs.cs`
- Test: `tests/Pricing.RuleCenter.Tests/SqlSugarEntityTypeConfigTests.cs`

- [ ] Add the new Oracle DDL file for template, policy, review, runtime package, runtime rule, and active-package state tables.
- [ ] Add domain aggregates and constants for template, policy, and runtime package concepts without changing existing pricing execution behavior.
- [ ] Add repository interfaces for the new aggregates.
- [ ] Extend SqlSugar entity configuration so the new aggregates map cleanly to Oracle naming and field conventions.
- [ ] Update entity mapping tests to cover the new tables and key fields.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter SqlSugarEntityTypeConfigTests`

## Chunk 2: Runtime package compiler and active package reader

**Files:**
- Create: `src/Application/Application/RuntimePackages/RuntimePackageCompiler.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageBuildContext.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageBuildResult.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageActivationService.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageReader.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimeRuleSelector.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimeRuleProjectionFactory.cs`
- Create: `src/Application/Application/Policies/PolicyPriorityKeyFactory.cs`
- Create: `src/Application/Application/Policies/PolicyConflictService.cs`
- Create: `src/Application/Application/Policies/PolicyValidationService.cs`
- Create: `src/Infrastructure/Repositories/Runtime/RuntimePackageRepository.cs`
- Create: `src/Infrastructure/Repositories/Runtime/RuntimeRuleRepository.cs`
- Modify: `src/Application/Application/Engine/RuleMatchService.cs`
- Modify: `src/Application/Application/Engine/RuleRuntimeSnapshot/EffectiveRuleSnapshotLoader.cs`
- Modify: `src/API/Program.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RuleMatchServiceTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RulePublishConflictTests.cs`
- Test: `tests/Pricing.RuleCenter.Tests/RuleCacheOutboxAppServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackageCompilerTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimeRuleSelectorTests.cs`

- [ ] Write failing tests for policy conflict ranking, runtime package compilation, and active package selection.
- [ ] Implement `PolicyPriorityKeyFactory` and `PolicyConflictService` for `CapabilityFamily`, binding specificity, and scope-level ordering.
- [ ] Implement `RuntimePackageCompiler` so publishable `PolicyVersion` records compile to runtime rules, conditions, and actions.
- [ ] Implement `RuntimePackageReader` and `RuntimeRuleSelector` so runtime rule loading comes from the active package instead of hand-maintained rule rows.
- [ ] Adapt `RuleMatchService` and effective snapshot loading to use runtime package projections while preserving current action ordering semantics.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter "RuntimePackageCompilerTests|RuntimeRuleSelectorTests|RuleMatchServiceTests"`

## Chunk 3: Template and policy authoring application services

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
- Modify: `src/Infrastructure/DependencyInjection.cs`
- Modify: `src/API/Program.cs`
- Create: `tests/Pricing.RuleCenter.Tests/TemplateAppServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyAppServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyValidationServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyPreviewAppServiceTests.cs`

- [ ] Write failing tests for template creation, template versioning, policy draft save, weak-expression validation, and preview generation.
- [ ] Implement template CRUD and versioning services with support for parameter definitions, step definitions, and scope-definition constraints.
- [ ] Implement policy draft/version CRUD for binding, scope, parameter values, and publish-mode selection.
- [ ] Add `PolicyExpressionGuard` so weak expressions use the existing white-list parser and strong expressions remain role-restricted.
- [ ] Implement preview generation that translates a policy draft into business-readable capability and action-chain output.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter "TemplateAppServiceTests|PolicyAppServiceTests|PolicyValidationServiceTests|PolicyPreviewAppServiceTests"`

## Chunk 4: Publish workflow, optional approval, activation, and rollback

**Files:**
- Create: `src/Application/Application/RuntimePackages/RuntimePackagePublishService.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageRollbackService.cs`
- Create: `src/Application/Application/Policies/PolicyReviewAppService.cs`
- Create: `src/Application/Application/Policies/PolicyPublishProfileResolver.cs`
- Create: `src/Application/Application/Policies/PolicyPublishEligibilityService.cs`
- Create: `src/Application/Application/Policies/PolicyPackageDiffService.cs`
- Modify: `src/Application/Application/Rules/Guards/RuleApprovalGate.cs`
- Modify: `src/Application/Application/Rules/RuleApprovalAppService.cs`
- Modify: `src/Application/Application/Background/CacheVersionSynchronizer.cs`
- Modify: `src/Application/Application/Background/CacheVersionSyncService.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackagePublishServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackageRollbackServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyReviewAppServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyPackageDiffServiceTests.cs`

- [ ] Write failing tests for direct publish, approval-required publish, package activation, package rollback, and package diff generation.
- [ ] Implement publish-profile resolution so low-risk policies can go direct while higher-risk strategies require review when configured.
- [ ] Implement policy review services for submit, approve, reject, and approval-outdated checks against draft changes.
- [ ] Implement package publish, activation, and rollback services that switch only the active package pointer and invalidate runtime cache immediately.
- [ ] Implement diff generation for “current active package vs candidate package” to support pre-release review.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter "RuntimePackagePublishServiceTests|RuntimePackageRollbackServiceTests|PolicyReviewAppServiceTests|PolicyPackageDiffServiceTests"`

## Chunk 5: API surface and HIS workbench reshaping

**Files:**
- Create: `src/API/Controllers/TemplateController.cs`
- Create: `src/API/Controllers/PolicyController.cs`
- Create: `src/API/Controllers/RuntimePackageController.cs`
- Modify: `src/API/Controllers/PricingController.cs`
- Modify: `his-client/PricingRuleDtos.cs`
- Modify: `his-client/PricingApiClient.cs`
- Modify: `his-client/FrmPricingRuleWorkbench.cs`
- Create: `his-client/FrmPolicyPublishCenter.cs`
- Create: `tests/Pricing.RuleCenter.Tests/TemplateControllerRouteTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyControllerRouteTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackageControllerRouteTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/HisClientPolicyWorkbenchTests.cs`

- [ ] Write failing route tests for template, policy, preview, review, publish, package history, and package rollback endpoints.
- [ ] Add API controllers that expose template management, policy management, preview, review, publish, and package operations.
- [ ] Update HIS client DTOs and API client methods to use template/policy/package terminology rather than raw rule-action authoring.
- [ ] Replace the current rule workbench flow with template selection, business parameter entry, preview, and publish-center actions.
- [ ] Run: `dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --filter "TemplateControllerRouteTests|PolicyControllerRouteTests|RuntimePackageControllerRouteTests|HisClientPolicyWorkbenchTests"`

## Chunk 6: Historical rule migration, observability, and release hardening

**Files:**
- Create: `sql/07-seed-template-catalog.sql`
- Create: `sql/08-import-initial-policies.sql`
- Create: `src/Application/Application/Policies/PolicyImportService.cs`
- Create: `src/Application/Application/RuntimePackages/RuntimePackageTraceResolver.cs`
- Modify: `src/Application/Application/Pricing/Persistence/PricingRequestLogWriter.cs`
- Modify: `src/Application/Application/Pricing/Persistence/PricingTraceStepWriter.cs`
- Modify: `src/Domain/Aggregates/Charging/ChargeRequest.cs`
- Modify: `src/Domain/Aggregates/Charging/ChargeTraceStep.cs`
- Create: `tests/Pricing.RuleCenter.Tests/PolicyImportServiceTests.cs`
- Create: `tests/Pricing.RuleCenter.Tests/RuntimePackageTraceResolverTests.cs`
- Modify: `docs/物价折价改造方案文档/07-开发任务清单-物价折价改造.md`
- Modify: `docs/物价折价改造方案文档/14-测试计划.md`

- [ ] Write failing tests for importing initial policy data, tracing package-version lineage, and logging package version on each pricing call.
- [ ] Seed the initial template catalog for the six high-frequency historical rule families.
- [ ] Implement historical import from the curated rule list into draft `PolicyVersion` records with parameter hydration.
- [ ] Add `RuntimePackageTraceResolver` and persist `PackageVersion`, `SourcePolicyVersionId`, and `SourceTemplateVersionId` in request and trace records.
- [ ] Update rollout and test-plan docs to reflect package publish, package rollback, and migration checkpoints.
- [ ] Run: `dotnet build src\Pricing.RuleCenter.slnx --no-restore`
- [ ] Run: `dotnet test src\Pricing.RuleCenter.slnx --no-restore`
- [ ] Commit the completed chunk when tests are green.

## Recommended execution notes

- Start with Chunk 1 and Chunk 2 before touching any authoring UI or publish workflow.
- Do not implement policy workbench screens before the runtime package reader is already the only source of runtime rules.
- Keep the existing executor set intact until the runtime package compiler is stable; only then add new template families or capability metadata.
- Treat strong-expression enablement as a later guarded feature flag even if the database fields exist from day one.
