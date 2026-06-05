# src Maintainability Refactor Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Reduce `src/` maintenance complexity while preserving existing API behavior and test coverage.

**Architecture:** Introduce a real Application project, keep API as HTTP adapter, extract focused rule edit and pricing helper services from oversized application services. Refactor in small reversible batches guarded by xUnit tests.

**Tech Stack:** .NET 6, ASP.NET Core Web API, xUnit, SqlSugar/Oracle infrastructure.

---

## Chunk 1: Rule edit guard and current red tests

**Files:**
- Create: `src/Pricing.RuleCenter.Api/Application/Rules/RuleEditGuard.cs`
- Modify: `src/Pricing.RuleCenter.Api/Application/Rules/RuleHeaderAppService.cs`
- Modify: `src/Pricing.RuleCenter.Api/Application/Rules/RuleConditionAppService.cs`
- Modify: `src/Pricing.RuleCenter.Api/Application/Rules/RuleActionAppService.cs`
- Modify: `src/Pricing.RuleCenter.Api/Program.cs`

- [ ] Re-run the three failing tests and confirm they fail for missing pending-approval guard behavior.
- [ ] Add `RuleEditGuard` that detects latest PUBLISH approval state from change logs.
- [ ] Replace per-service or missing guard logic with `RuleEditGuard`.
- [ ] Run targeted tests for rule header/condition/action pending approval.

## Chunk 2: PricingAppService helper extraction

**Files:**
- Create: `src/Pricing.RuleCenter.Api/Application/Pricing/PricingCommitActualValidator.cs`
- Create: `src/Pricing.RuleCenter.Api/Application/Pricing/PricingRequestFingerprintBuilder.cs`
- Create: `src/Pricing.RuleCenter.Api/Application/Pricing/PricingLockKeyBuilder.cs`
- Modify: `src/Pricing.RuleCenter.Api/Application/Pricing/PricingAppService.cs`
- Test: existing `PricingApiServiceTests.cs`, `PricingReverseTests.cs`

- [ ] Extract commit actual detail validation first; run commit-related tests.
- [ ] Extract fingerprint normalization/building; run idempotency tests.
- [ ] Extract lock key helpers; run pricing/reverse tests.

## Chunk 3: Application project split

**Files:**
- Create: `src/Pricing.RuleCenter.Application/Pricing.RuleCenter.Application.csproj`
- Move: `src/Pricing.RuleCenter.Api/Application/**` to `src/Pricing.RuleCenter.Application/**`
- Move: `src/Pricing.RuleCenter.Api/Dto/**` to `src/Pricing.RuleCenter.Application/Dto/**`
- Modify: `src/Pricing.RuleCenter.Api/Pricing.RuleCenter.Api.csproj`
- Modify: `src/Pricing.RuleCenter.slnx`
- Modify imports in controllers, filters, tests.

- [ ] Add Application project referencing Core.
- [ ] Move application services and DTOs; update namespaces.
- [ ] Update API project references and using directives.
- [ ] Update tests to reference Application where needed.
- [ ] Build solution.

## Chunk 4: Constants and comment slimming

**Files:** touched pricing/rule service files.

- [ ] Replace touched magic status strings with `StatusCodes` constants.
- [ ] Remove redundant stage-banner comments from touched methods when method names/tests already explain behavior.
- [ ] Keep comments for money safety, status-machine transitions, and Oracle/HIS compatibility.
- [ ] Run full build and tests.
