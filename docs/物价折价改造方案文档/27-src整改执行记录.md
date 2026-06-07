# src 整改执行记录

## 基线

- 开始日期：2026-06-06
- 当前分支：`main...origin/main [ahead 2]`
- 初始测试命令：`dotnet test src\Pricing.RuleCenter.slnx --no-restore`
- 初始测试结果：通过。`Pricing.RuleCenter.Core.Tests` 38 通过，`Pricing.RuleCenter.Tests` 217 通过，合计 255 通过，0 失败。
- 初始格式检查：通过。`git diff --check` 无格式错误输出。

## Chunk 进度

| Chunk | 状态 | 测试结果 | 备注 |
| --- | --- | --- | --- |
| Chunk 1 | 已完成 | 基线测试通过；`git diff --check` 通过 | 已生成当前大类职责地图 |
| Chunk 2 | 已完成 | `PricingCalculateRequestTests|ValidationBehaviorTests|PricingApiServiceTests` 29 通过；`CoreBusinessCoverageTests` 24 通过；`git diff --check` 通过 | 新增 `PricingRequestGuard`，对齐入口 validator 与服务层强校验 |
| Chunk 3 | 已完成 | `PricingApiServiceTests|BatchPricingContextTests|CoreBusinessCoverageTests` 50 通过；`git diff --check` 通过 | 抽出 `Builders/*`；`PricingAppService` 当前约 1543 行 |
| Chunk 4 | 已完成 | `PricingApiServiceTests` 24 通过；`PricingApiServiceTests|PricingReverseTests|CoreBusinessCoverageTests` 61 通过；`git diff --check` 通过 | 抽出 `AuthorityPriceChecker`、`PricingIdempotencyService`；`PricingAppService` 当前约 1499 行 |
| Chunk 5 | 已完成 | `PricingApiServiceTests|PricingReverseTests|CoreBusinessCoverageTests` 61 通过；`git diff --check` 通过 | 抽出 `Persistence/*Writer`；`PricingAppService` 当前约 1005 行 |
| Chunk 6 | 已完成 | `PricingApiServiceTests|PricingReverseTests|BatchPricingContextTests|CoreBusinessCoverageTests` 63 通过；全量 `dotnet test` 258 通过；`git diff --check` 通过 | 新增 6 个计价 use case；`PricingAppService` 当前约 105 行兼容门面 |
| Chunk 7 | 已完成 | `RulePublishConflictTests|RuleApprovalAppServiceTests|RuleDefinitionTransactionTests|CacheVersionSynchronizerTests` 43 通过；`git diff --check` 通过 | 新增发布/停用/回滚 use case；抽出 `RulePublishGuard`、审批门禁、冲突检测、动作参数/关键动作/子项目/测试用例门禁、缓存失效器和事务执行器；`RulePublishAppService` 当前为兼容门面 |
| Chunk 8 | 已完成 | `ApiExceptionMapperTests|GlobalExceptionFilterTests|ControllerNotFoundTests|PricingApiServiceTests` 41 通过；`git diff --check` 通过 | 新增 `ApiExceptionMapper`，中间件和 MVC 过滤器共用同一异常映射；保留 middleware 作为 `Program.cs` 的统一入口 |
| Chunk 9 | 已完成 | `RuleDefinitionTransactionTests` 12 通过；`Expire|Approval|PricingApiServiceTests|RulePublishConflictTests|RuleDefinitionTransactionTests|RuleHeaderServiceTests|DictAppServiceTests|FormulaDefAppServiceTests` 80 通过；`git diff --check` 无格式错误 | 新增 `IClock/SystemClock/FixedClock`；计价、规则发布、审批、配置维护、过期清理等应用编排层技术时间改用统一时钟；领域聚合自打点、仓储默认补时间、健康检查时间和测试夹具中的 `DateTime.Now` 暂保留 |
| Chunk 10 | 已完成 | 文档检查；第 11-14 阶段组合测试通过 | 新增 `28-规则引擎能力矩阵.md`，明确“可配置/部分配置/需新增原语”的判定标准和一劳永逸边界 |
| Chunk 11 | 已完成 | `FormulaExpressionEvaluatorTests|ActionExecutionPipelineTests|FormulaDefAppServiceTests|RulePublishConflictTests` 通过；第 11-14 阶段组合测试 89 通过 | 新增受控表达式公式 parser、函数白名单、表达式校验器和 `EXPRESSION_FORMULA` executor；发布参数校验接入表达式校验 |
| Chunk 12 | 已完成 | `RuleMatchServiceTests|RuleMatchServiceGroupScopeTests|ActionExecutionPipelineTests` 17 通过；第 11-14 阶段组合测试通过 | 新增 `EffectiveRuleSnapshot`、loader、cache；`RuleMatchService` 改为消费运行期快照，保留旧构造兼容测试 |
| Chunk 13 | 已完成 | `ExtraParamConditionEvaluatorTests|RuleMatchServiceTests|CoreBusinessCoverageTests` 42 通过；第 11-14 阶段组合测试通过 | 新增医保身份、诊断/病种、设备型号三个基于 `ExtraParams` 的条件 evaluator，不扩展主 DTO 字段 |
| Chunk 14 | 已完成 | 新增 4 个发布失败用例通过；`RulePublishConflictTests` 在组合测试中通过 | 新增 `RuleCapabilityGuard` 和错误码，发布前阻断未知条件、未知动作、未知公式 executor 和非法表达式公式 |
| Chunk 15 | 已完成 | 文档和 SQL 只读脚本检查 | 新增 `29-Oracle并发与事务验证方案.md` 与 `sql/90-concurrency-verify.sql`，覆盖 confirm/reverse 幂等、限额锁、commit-expire 竞态、发布审批竞态和事务回滚验证 |
| Chunk 16 | 已完成 | 全量 `dotnet test src\Pricing.RuleCenter.slnx --no-restore`：`Pricing.RuleCenter.Core.Tests` 38 通过，`Pricing.RuleCenter.Tests` 256 通过，合计 294 通过，0 失败；`git diff --check` 无格式错误 | 最终验证完成；`PricingAppService` 105 行、`RulePublishAppService` 107 行；工作区仅包含本计划相关代码、测试、文档、SQL 变更 |

## 最终验证

- 全量测试命令：`dotnet test src\Pricing.RuleCenter.slnx --no-restore`
- 全量测试结果：通过。`Pricing.RuleCenter.Core.Tests` 38 通过，`Pricing.RuleCenter.Tests` 256 通过，合计 294 通过，0 失败。
- 格式检查命令：`git diff --check`
- 格式检查结果：无 trailing whitespace 或 patch 格式错误；仅 Git 在 Windows 下提示 LF/CRLF 换行转换。
- 工作区检查命令：`git status --short --branch`
- 工作区状态：`main...origin/main [ahead 2]`，变更集中在整改计划涉及的源码、测试、文档和 SQL。

## 最终结构结果

- `PricingAppService`：105 行，保留对外兼容门面，委托 `Simulate/Confirm/Commit/Cancel/Reverse/GetSpecialFlag` use case。
- `RulePublishAppService`：107 行，保留查询和生命周期兼容门面，发布/停用/回滚由独立 use case 处理。
- 计价应用层新增边界：
  - `AuthorityPrice/*`
  - `Builders/*`
  - `Idempotency/*`
  - `Persistence/*Writer`
  - `UseCases/*`
  - `Validation/PricingRequestGuard.cs`
- 规则发布新增边界：
  - `Publishing/*UseCase`
  - `Publishing/*Writer`
  - `Publishing/*CacheInvalidator`
  - `Guards/*`
  - `Profiles/*`
- 规则引擎新增边界：
  - `Formula/*`
  - `RuleRuntimeSnapshot/*`
  - `Evaluators/*ExtraParam*`

## 行为兼容性说明

- confirm 幂等、请求指纹冲突、commit 实际落账校验、cancel 状态限制、reverse 超退校验和 reverse 幂等均保留原有测试覆盖。
- 规则动作顺序仍由 `ACTION_TYPE_ORDER` 字典和默认顺序共同保证，旧 HIS 口径仍为换算、数量限制/互斥、公式、金额上下限、同手术封顶、子项、超限归零。
- 业务时间仍以 `BusinessChargeTime` 为准；统一时钟只替换应用编排层技术时间。
- 表达式公式中间计算使用 `decimal`，最终金额仍由 `PricingAmountRounder.RoundFinal` 统一保留 2 位四舍五入。
- 运行期规则快照只集中读取规则、条件、动作，不改变条件组匹配、动作互斥和动作排序语义。

## 新增规则能力

- 新增受控表达式公式：
  - 变量白名单：`inputQty`、`convertedQty`、`finalQty`、`unitPrice`、`originalAmount`、`finalAmount`、`partCount`、`area`。
  - 函数白名单：`min`、`max`、`round`、`ceil`、`floor`。
  - 新 executor：`FORMULA_CALC + EXPRESSION_FORMULA`。
- 新增通用条件原语：
  - `INSURANCE_TYPE_MATCH`：读取 `ExtraParams["insuranceType"]`。
  - `DIAGNOSIS_MATCH`：读取 `ExtraParams["diagnosisCodes"]`。
  - `DEVICE_TYPE_MATCH`：读取 `ExtraParams["deviceType"]`。
- 新增发布能力门禁：
  - 未知条件类型阻断发布。
  - 未知动作类型阻断发布。
  - 未知公式 executor 阻断发布。
  - 表达式公式非法变量/函数/语法阻断发布。

## 未完成边界

- 本轮未承诺“所有未来政策零代码”。跨周期住院计价、组合包复杂分摊、政策追溯重算、DRG/DIP 总额类政策仍需新增规则原语或独立流程。
- Oracle 并发验证脚本为只读结构检查和人工并发验证指南，尚未在真实预发 Oracle 会话中执行。
- 领域聚合内部自打点、部分仓储默认补时间和健康检查时间未纳入统一时钟，后续如需完全可控时间可单独做领域层时钟改造。

## 当前大类职责地图

### PricingAppService

当前文件：`src/Application/Application/Pricing/PricingAppService.cs`

当前行数：1953 行。

公开入口：

- `SimulateAsync`：试算计价，校验权威单价，执行规则引擎，保存请求日志、步骤日志和响应快照，不占用额度。
- `ConfirmAsync`：正式确认计价，校验业务号、权威单价和幂等指纹，事务内再次幂等检查，执行规则引擎，保存请求日志、步骤日志、折价明细、限额占用和响应快照。
- `CommitAsync`：HIS 落账成功后推进状态，校验实际落账明细，同步确认请求日志、折价明细和限额占用。
- `CancelAsync`：HIS 未落账或取消时释放 confirm 保护状态，同步取消请求日志、折价明细和限额占用。
- `ReverseAsync`：对已落账记录执行退费/冲正，处理 reverse 幂等、超退校验、主子项目分组校验、全退/部分退和负向占额。
- `GetSpecialFlagAsync`：查询当前有效已发布规则，返回特殊项目标识和保守 `RollbackMode`。

当前内部职责：

- reverse 历史退费数量/金额汇总。
- reverse 请求日志和冲正日志保存。
- 部分退费负向占额插入。
- 权威单价校验。
- `PricingContext` 构造。
- extra params 合并。
- 请求日志、响应快照、步骤日志、折价明细、子项明细、限额占用写入。
- 同请求限额累计。
- 幂等锁键、请求锁键、reverse 锁键、trace id、result group no 构造。
- 响应 DTO 和 reason 文本构造。
- 事务包装。
- 请求参数校验。

整改目标：

- 先抽校验、builder、幂等、权威单价和持久化 writer。
- 再拆成 `Simulate/Confirm/Commit/Cancel/Reverse/GetSpecialFlag` 六个 use case。
- 最终 `PricingAppService` 只作为兼容门面。

### RulePublishAppService

当前文件：`src/Application/Application/Rules/RulePublishAppService.cs`

当前行数：1330 行。

公开入口：

- `GetPublishHistoryAsync`：查询规则发布流水。
- `GetChangeLogsAsync`：查询规则变更日志。
- `PublishAsync`：发布草稿版本，校验审批、冲突、动作参数、测试用例和状态机，事务内推进主档/版本/流水。
- `DisableAsync`：停用已发布规则，校验审批并事务内推进状态。
- `RollbackAsync`：回滚到历史发布版本，校验审批并事务内推进当前版本和目标版本状态。

当前内部职责：

- 回滚目标版本解析。
- 审批有效性校验。
- 草稿变更时间判断。
- 发布冲突校验。
- 动作参数 JSON 校验。
- 启用测试用例门禁。
- 关键动作 `OnError = STOP` 门禁。
- `ADD_CHILD_ITEM` 子项目门禁。
- 规则冲突 profile 构造。
- 条件范围重叠判断。
- 互斥动作字典加载和缓存。
- 发布流水和变更日志映射。
- 生效规则、special-flag、运行期动作顺序缓存清理。
- 事务包装。

整改目标：

- 拆为发布、停用、回滚三个 use case。
- 拆出 `RulePublishGuard`、`RuleApprovalGate`、`RuleConflictDetector`、`RuleActionParameterValidator`、`RuleTestCaseGate`、`RuleCriticalActionGuard`、`RuleChildItemGuard`。
- 集中缓存失效逻辑到 `RulePublishCacheInvalidator`。
- 最终 `RulePublishAppService` 只作为兼容门面。

### RuleMatchService

当前文件：`src/Application/Application/Engine/RuleMatchService.cs`

当前行数：598 行。

当前职责：

- 按项目编码读取候选规则。
- 过滤已发布、启用、业务时间有效规则。
- 按条件组执行“组内 AND、组间 OR”匹配。
- 收集命中规则动作。
- 应用动作互斥组。
- 从字典加载并缓存全局动作执行顺序。
- 按动作类型、规则优先级和 `SortNo` 排序动作链。
- 提供运行期缓存清理。

整改目标：

- 后续引入 `EffectiveRuleSnapshot`、loader 和 cache。
- 降低条件/动作逐条读取造成的 N+1 风险。
- 保持动作顺序和保守匹配语义不变。

### ActionExecutionPipeline

当前文件：`src/Application/Application/Engine/ActionExecutionPipeline.cs`

当前行数：265 行。

当前职责：

- 根据 `ActionType` 和 `ExecutorCode` 选择唯一执行器。
- 按规则动作链顺序执行 executor。
- 按 `OnError` 处理 STOP/WARN/SKIP。
- 写入动作执行追溯步骤。
- 将具体动作类型映射为数据库允许的步骤类型。

整改目标：

- 保持管线结构稳定。
- 后续接入表达式公式 executor。
- 后续增强能力门禁，避免未知 action/executor 运行期才暴露。

## 执行注意事项

- 资金链路行为必须保持兼容。
- 规则动作顺序必须保持兼容。
- 所有金额仍使用 `decimal`。
- 业务时间仍优先使用请求传入的 `BusinessChargeTime`。
- 每个 Chunk 完成后更新本记录。

## Review 修复记录：规则发布能力码兼容

日期：2026-06-06

修复内容：

- 新增集中能力码定义 `RuleConditionTypeCodes`、`RuleActionTypeCodes`、`ActionOnErrorCodes`、`FormulaExecutorCodes`，避免 `RuleCapabilityGuard` 和运行时执行器各自维护魔法字符串。
- `RuleCapabilityGuard` 支持 `sql/03-init-formula-def.sql` 已种子的公式执行器别名：
  - `ConvertQtyByPartExecutor`
  - `AreaStepIncrementExecutor`
  - `ChildItemPercentExecutor`
- `ConditionEvaluatorFactory` 支持把条件别名分派到现有运行时 evaluator：
  - `ITEM_CODE` -> `ITEM_MATCH`
  - `CHARGE_SCENE_MATCH` -> `CHARGE_SCENE`
  - `BODY_PART_MATCH` -> `BODY_PART`
- `ITEM_CODE` 明确决策：作为 `ITEM_MATCH` 的历史兼容别名保留，发布可通过，运行时也由 `ItemMatchEvaluator` 处理；后续配置推荐统一使用 `ITEM_MATCH`。
- 公式执行器的 `CanHandle` 统一引用 `FormulaExecutorCodes`，发布校验和运行期执行使用同一套别名判断。
- 相邻发布门禁 `RuleActionParameterValidator`、`RuleCriticalActionGuard`、`RuleChildItemGuard`、`RuleConflictDetector` 改为复用集中动作/条件编码。

回归测试：

- `PublishAsync_AllowsSeededFormulaExecutorAliases`：覆盖 3 个种子公式执行器别名发布成功。
- `PublishAsync_AllowsConditionAliasesHandledByRuntime`：覆盖 3 个条件别名发布成功。
- `Factories_ReturnRegisteredStrategiesAndFallbacks`：覆盖 `ITEM_CODE`、`CHARGE_SCENE_MATCH`、`BODY_PART_MATCH` 能分派到运行时 evaluator，并验证 `ITEM_CODE` 实际按项目编码匹配。

验证结果：

- `dotnet test src\Pricing.RuleCenter.slnx --no-restore --filter "RulePublishConflictTests|CoreBusinessCoverageTests|RuleMatchServiceTests|ActionExecutionPipelineTests"`：通过，Core.Tests 7 个、Tests 62 个。
- `dotnet test src\Pricing.RuleCenter.slnx --no-restore`：通过，Core.Tests 38 个、Tests 262 个，共 300 个。
- `git diff --check`：无格式错误；仅有 Git 提示 LF 将在下次触碰时替换为 CRLF。
