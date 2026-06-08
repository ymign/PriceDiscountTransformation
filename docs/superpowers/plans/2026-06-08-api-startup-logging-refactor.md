# API 启动配置精简与启动日志 Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Simplify the API `Program.cs` and make startup/request logging explicit and structured without changing external API behavior.

**Architecture:** Keep all Web host composition inside `Pricing.RuleCenter.Api`. Move registration and pipeline details into focused `Startup` extension classes, and keep `Program.cs` as orchestration only. Add a small testable startup metadata resolver for logging and Swagger enablement.

**Tech Stack:** .NET 8, ASP.NET Core Web API, Serilog, xUnit, MediatR, FluentValidation.

---

## Chunk 1: Startup Metadata Test

**Files:**
- Modify: `tests/Pricing.RuleCenter.Tests/ApiDocumentationIntegrationTests.cs`
- Create: `src/Pricing.RuleCenter.Api/Startup/RuleCenterStartupInfo.cs`

- [ ] Add a failing unit test proving startup metadata resolves `service_name`, environment, Swagger enablement, and build fields from configuration.
- [ ] Run the targeted test and confirm it fails because `RuleCenterStartupInfo` does not exist yet.
- [ ] Implement the smallest startup metadata resolver needed by the test.
- [ ] Re-run the targeted test and confirm it passes.

## Chunk 2: Composition Root Split

**Files:**
- Create: `src/Pricing.RuleCenter.Api/Startup/RuleCenterApiServiceCollectionExtensions.cs`
- Create: `src/Pricing.RuleCenter.Api/Startup/RuleCenterApiApplicationBuilderExtensions.cs`
- Modify: `src/Pricing.RuleCenter.Api/Program.cs`

- [ ] Move infrastructure, application service, authentication, hosted service, rule engine, health check, controller, and Swagger registrations into grouped extension methods.
- [ ] Move middleware, Swagger, authentication, authorization, health check, and controller mapping into a pipeline extension.
- [ ] Keep `public partial class Program` for `WebApplicationFactory<Program>` integration tests.
- [ ] Run API documentation/security tests after the split.

## Chunk 3: Structured Startup and Request Logging

**Files:**
- Create: `src/Pricing.RuleCenter.Api/Startup/RuleCenterLoggingExtensions.cs`
- Modify: `src/Pricing.RuleCenter.Api/Program.cs`
- Modify: `src/Pricing.RuleCenter.Api/Startup/RuleCenterApiApplicationBuilderExtensions.cs`

- [ ] Centralize Serilog host configuration in `AddRuleCenterLogging`.
- [ ] Add startup lifecycle logs: `application_starting`, `application_started`, `application_startup_failed`, `application_stopped`.
- [ ] Configure `UseSerilogRequestLogging` message and diagnostic context fields.
- [ ] Avoid changing existing business workflow logs in this refactor.

## Chunk 4: Verification and Commit

**Files:**
- Review all touched files.

- [ ] Run `dotnet test src\Pricing.RuleCenter.slnx --no-restore`.
- [ ] Run `git diff --check`.
- [ ] Inspect `git diff --stat` and `git status --short`.
- [ ] Commit with a Chinese message.
- [ ] Push to GitHub.
