# API 启动配置精简与启动日志改造设计

## 背景

`src/Pricing.RuleCenter.Api/Program.cs` 当前同时承担宿主创建、Serilog 配置、基础设施注册、应用服务注册、认证授权、后台服务、规则引擎、健康检查、Swagger 和 HTTP 管道配置。入口文件职责过多，后续新增能力容易继续堆叠，启动日志也缺少清晰的生命周期事件。

## 目标

1. 保留现有 API 行为、认证授权策略、Swagger 开关、健康检查和集成测试入口。
2. 将 `Program.cs` 压缩为启动编排，只保留创建、注册、构建、管道、运行几个高层步骤。
3. 在 API 层新增组合根扩展，集中承接 Web 宿主相关注册，不把 Web 细节下沉到 Application/Core。
4. 输出结构化启动日志，明确区分启动中、已启动、启动失败、已停止。
5. 优化 HTTP 请求完成日志，补充 trace id、请求方法、路径、状态码和耗时字段。

## 设计

采用方案 A：API 层组合根拆分。

- `Program.cs` 只负责编排，不直接维护大量服务注册明细。
- 新增 `Startup` 目录，按职责拆分服务注册和管道配置。
- 应用服务、规则引擎、认证授权、Swagger、健康检查继续由 API 层组合，不改变现有项目依赖方向。
- 启动日志集中封装，避免在各注册扩展里散点记录。

## 日志事件

启动阶段输出以下结构化事件：

- `application_starting`
- `application_started`
- `application_startup_failed`
- `application_stopped`

事件字段包括：

- `service_name`
- `environment`
- `content_root`
- `urls`
- `swagger_enabled`
- `build_commit`
- `build_branch`
- `build_time_utc`

HTTP 请求日志由 `UseSerilogRequestLogging` 统一输出请求完成事件，并通过诊断上下文附加 `trace_id`、`request_method`、`request_path`、`status_code` 和 `elapsed_ms`。

## 边界

- 不调整 Controller、DTO、业务 Workflow、规则引擎执行顺序。
- 不新增业务应用服务层。
- 不改变 `/health`、`/health/version`、Swagger JSON、API Key 授权的外部行为。

## 验证

优先新增启动元信息相关测试，再执行：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore
git diff --check
```
