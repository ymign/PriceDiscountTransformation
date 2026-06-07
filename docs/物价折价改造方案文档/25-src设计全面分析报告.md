# src 设计全面分析报告

报告日期：2026-06-06

评估范围：`src/` 下的 `API`、`Application`、`Domain`、`Infrastructure` 四个项目，结合 `tests/`、`sql/01-create-tables.sql`、`CLAUDE.md`、`02/05/08/10/13/24` 号方案文档进行交叉核对。

## 1. 总体结论

当前 `src` 的总体设计方向是正确的，不是简单 CRUD，也不是把旧 HIS 硬编码搬到接口里。它已经形成了比较清晰的规则中心雏形：

- 用规则主档、版本、条件、动作、公式、字典表达业务配置。
- 用规则匹配服务和动作执行管线承接计价逻辑。
- 用 `confirm -> commit/cancel -> reverse/expire` 状态链处理资金安全。
- 用请求日志、步骤日志、折价明细、占额表和冲正日志支撑追溯。
- 用 Oracle `SELECT ... FOR UPDATE` 锁表、幂等指纹、版本 CAS、审批门禁和缓存版本同步处理一部分生产风险。

我的判断：

| 维度 | 评价 |
| --- | --- |
| 架构方向 | 好，分层和业务边界基本成立 |
| 业务建模 | 好，条件-动作-执行器模型适合后续迁移历史规则 |
| 资金安全 | 中上，核心链路已有幂等、事务、锁和状态推进，但仍需真实 Oracle 并发验证 |
| 可维护性 | 中上，局部服务过大，继续扩展会压低可读性 |
| 测试基础 | 好，自动化测试数量和重点覆盖明显优于一般阶段性项目 |
| 上线成熟度 | 还不到直接全量上线，适合继续做联调、压测、Oracle 集成验证和规则迁移验收 |

一句话结论：**这个设计值得继续沿用，不建议推倒重来；下一阶段重点应从“补功能”转为“收敛复杂度、验证并发资金链路、补上线治理”。**

## 2. 自动化验证结果

本次评估执行了：

```powershell
dotnet test src\Pricing.RuleCenter.slnx --no-restore
```

结果：

- `Pricing.RuleCenter.Core.Tests`：38 通过
- `Pricing.RuleCenter.Tests`：217 通过
- 合计：255 通过，0 失败

这说明当前主干至少在已有测试范围内是稳定的。需要注意，测试通过不能替代真实 Oracle 锁竞争、HIS 联调、历史规则迁移和性能压测。

## 3. 分层设计评价

### 3.1 当前分层

当前项目是四层结构：

- `API`：HTTP 控制器、Swagger、健康检查、异常中间件。
- `Application`：用例编排、事务边界、MediatR 命令、规则发布、计价确认。
- `Domain`：聚合、值对象、枚举、领域接口、计价上下文、金额取整。
- `Infrastructure`：SqlSugar、Oracle、仓储、UnitOfWork、实体映射。

项目引用方向基本正确：

- `Domain` 不依赖 ORM 和 ASP.NET Core。
- `Application` 依赖 `Domain`。
- `Infrastructure` 依赖 `Domain` 并实现仓储接口。
- `API` 组合 `Application` 和 `Infrastructure`。

这是一个比较健康的 Clean Architecture/DDD 分层落地方式，尤其是 `Domain` 没有直接打 SqlSugar 特性，实体映射集中在 `EntityTypeConfigs`，这个选择是对的。

### 3.2 主要优点

1. 业务逻辑没有直接写进 Controller。

   `PricingController` 通过 MediatR 分发命令，Controller 层主要负责路由和响应包装。计价、幂等、事务、状态推进都在应用服务中。

2. ORM 没有侵入领域模型。

   `SqlSugarSetup` 使用 Fluent 配置，避免在 Core 模型上堆 SqlSugar Attribute。这对后续替换数据访问方式、做单元测试都有价值。

3. 仓储接口按领域拆分。

   规则、字典、计价追溯、限额占用、审批、测试用例分别有接口，应用层不需要知道 Oracle 细节。

### 3.3 主要问题

1. 应用服务已经偏大。

   当前最大文件：

   - `PricingAppService.cs`：约 1953 行
   - `RulePublishAppService.cs`：约 1330 行
   - `RuleMatchService.cs`：约 598 行
   - `PricingDto.cs`：约 594 行

   `PricingAppService` 同时承担请求校验、幂等、权威单价校验、批量计算编排、请求日志保存、折价明细保存、占额保存、commit/cancel/reverse、响应构造、追溯写入等职责。

   这在当前阶段可以理解，因为资金链路需要集中收口；但继续加住院、门诊、多渠道、更多公式和对账能力时，这个类会越来越难审查。

2. 部分“依赖聚合类”降低了构造函数膨胀，但没有真正拆分职责。

   例如 `PricingAppCalculationDependencies`、`PricingAppPersistenceRepositories`、`RulePublishLifecycleRepositories` 是有帮助的，但它们只是把参数打包；业务复杂度仍集中在主服务内部。

3. API、MediatR、应用服务存在重复校验。

   FluentValidation 中 `InputQty >= 0`，而 `PricingAppService.GetRequiredItems` 又要求 `InputQty > 0`。这种重复防线本身没错，但口径不完全一致时，接口错误码和错误信息会变得不稳定。

## 4. 规则引擎设计评价

### 4.1 当前设计

规则引擎核心由三部分构成：

- `RuleMatchService`：按项目、状态、生效时间、条件组匹配规则，并整理动作链。
- `ActionExecutionPipeline`：按顺序派发动作执行器。
- 多个 `IRuleActionExecutor` 和 `IRuleConditionEvaluator` 实现具体业务。

动作执行顺序已经明确按旧 HIS 关键口径组织：

1. 双单位换算
2. 数量限制、时间窗、单次限制、互斥
3. 公式折价
4. 金额下限、金额上限、同手术封顶
5. 子项加收
6. 超出部分归零兜底

这个顺序是本项目成败关键之一。当前代码没有把规则按单条配置的 `SortNo` 直接全局执行，而是先按动作类型全局排序，再按规则优先级和 `SortNo` 细排，这是正确的。

### 4.2 优点

1. 执行器模式适合规则迁移。

   后续新增动作类型或公式族，理论上只需要新增执行器、注册 DI、维护字典，而不需要改 HIS 渠道代码。

2. 条件组支持“组内 AND、组间 OR”。

   这个模型可以表达部位、场景、年龄、就诊类型、孕次、收费科室排除等复杂条件。

3. 运行期对未知条件和动作采取保守策略。

   找不到条件评估器时不默认命中；资金动作执行器缺失时默认中断。这符合计价系统“宁可失败，不可错收”的原则。

4. 批量上下文已经考虑。

   `BatchPricingContext` 用来承接同批试算/确认内的同组互斥、同手术封顶和限额累计，避免把多项目完全独立计算。

### 4.3 风险与建议

1. 规则匹配存在潜在 N+1 查询。

   当前流程是先按 `ItemCode` 取候选规则，再逐条取条件，命中后再逐条取动作。候选规则少时没问题；如果后续历史规则迁移多、同项目规则多、接口并发高，会放大数据库压力。

   建议后续增加只读投影或批量读取：

   - 一次性读取候选规则的所有条件。
   - 一次性读取命中版本的所有动作。
   - 对已发布规则构建内存只读快照。

2. 动作顺序缓存使用静态字段，已有失效机制，但仍需更完整治理。

   当前通过 `PR_CACHE_VERSION` 同步多实例缓存，这是合理的轻量方案。风险在于缓存类型逐渐增多后，容易出现某类缓存漏失效。后续建议统一缓存键注册和失效入口，不要让每个服务各自维护。

3. 追溯步骤还没有完整上下文快照。

   `ActionExecutionPipeline` 目前主要记录金额输入输出和动作参数。对复杂规则，如多部位、多面积、主子项目、同组互斥，后续最好补 `InputSnapshot/OutputSnapshot` 或等价 CLOB 字段，方便财务复盘。

## 5. 资金状态链路评价

### 5.1 confirm

当前 confirm 设计较严谨：

- 要求稳定 `BusinessRequestNo`。
- 幂等键使用 `SourceSystem + BusinessRequestNo + CallType`。
- 请求指纹 `REQUEST_FINGERPRINT` 用于识别同业务号参数变化。
- 事务内再次查幂等记录，防止并发重复插入。
- 使用限额锁保护幂等键。
- 请求日志、步骤日志、折价明细、限额占用在同一事务内保存。
- confirm 只进入 `CONFIRM_PENDING`，不直接视为 HIS 已落账。

这是正确的资金链路设计。

### 5.2 commit

commit 当前做了几件重要的事：

- 使用请求维度锁。
- 只允许 `CONFIRM_PENDING -> CONFIRMED`。
- 已确认状态下支持幂等返回。
- 校验 confirm 保存的折价明细与 HIS 实际落账明细。
- 同步推进请求日志、折价明细、限额占用状态。

这个设计明显优于“confirm 算完就算收费成功”的简单方案。

### 5.3 cancel

cancel 只允许处理未落账的 `CONFIRM_PENDING`，同步释放请求、明细和占额。这与 reverse 分工清楚，避免“已落账退费用 cancel”这种资金口径混乱。

### 5.4 reverse

reverse 已覆盖不少关键风险：

- `ReverseNo` 幂等。
- 同号不同参数拒绝。
- 只允许已落账状态冲正。
- 校验本次退费加历史已退不超过原有效数量和金额。
- 全退和部分退费口径区分。
- 当日部分退费用负向占额释放。
- 支持 `ChargeDetailNo`、`ResultGroupNo`、`PartSeq` 定位。

这是当前源码里业务价值最高的部分之一。

### 5.5 仍需关注的问题

1. 状态推进依赖“所有写入口都遵守同一把锁”。

   commit/cancel/reverse/expire 都使用请求锁，这是好的。但数据库更新本身多数仍是读对象后更新。如果未来出现新入口、手工脚本、补偿任务绕过同一锁，仍可能有状态覆盖风险。

   建议后续把资金表状态推进也逐步改成 CAS：

   - 请求日志：`expectedBusinessStatus`
   - 折价明细：`expectedStatus`
   - 限额占用：`expectedStatus`

2. reverse 的复杂明细审计还可以更细。

   当前冲正日志偏“一次 reverse 一条主日志”。如果财务要求一次退费中每条子项、每个 part 都有独立冲正审计，后续需要补更细的 reverse detail 表或扩展现有日志结构。

3. 时间处理散落较多。

   业务计算已经强调使用 `BusinessChargeTime`，这是对的。但技术时间仍大量使用 `DateTime.Now`，例如请求时间、过期时间、响应时间、发布流水时间、健康接口时间等。

   建议引入 `IClock` 或 `ISystemClock`：

   - 业务时间继续来自请求字段。
   - 技术时间统一从时钟服务获取。
   - 测试可以稳定模拟过期、跨日、回滚、审批失效。
   - 后续多机部署时更容易处理时区和服务器时间漂移。

## 6. 规则发布与治理评价

### 6.1 当前能力

规则生命周期已经从普通 CRUD 提升到了较完整的治理链：

- 草稿版本。
- 发布、停用、回滚。
- 审批提交、通过、驳回。
- 审批动作维度区分。
- 审批状态 CAS。
- 发布前校验规则冲突。
- 发布前校验动作参数。
- 发布前校验测试用例和最新测试运行。
- 发布前校验关键动作 `OnError = STOP`。
- 发布前校验重复子项目。
- 事务内 `SELECT ... FOR UPDATE` 锁主档和版本。
- 版本状态和主档状态 CAS。
- 发布后清理规则缓存和动作顺序缓存。

这套设计方向很好，已经开始接近“规则上线前门禁”而不是“维护页面点保存”。

### 6.2 风险与建议

1. 发布校验很重要，但当前集中在 `RulePublishAppService` 中。

   该类已经超过 1300 行，包含发布、停用、回滚、审批校验、冲突校验、动作参数校验、测试门禁、缓存失效。建议下一阶段拆为：

   - `RulePublishService`：发布状态机。
   - `RulePublishGuard`：发布前门禁。
   - `RuleConflictDetector`：规则重叠和动作互斥。
   - `RuleApprovalGate`：审批有效性判断。
   - `RulePublishAuditWriter`：发布流水和变更日志。

2. 测试用例门禁还比较粗。

   当前要求启用测试用例存在、输入输出完整、最新运行通过。后续应区分：

   - 正向用例。
   - 边界用例。
   - 金额封顶用例。
   - 时间窗用例。
   - reverse 用例。
   - 主子项目用例。

   这需要表结构或 `CaseType/Tags` 字段支持。

3. 权限体系缺失。

   代码中目前没有看到认证和授权中间件。规则发布、停用、回滚、审批、字典维护都属于高风险管理接口，上线前必须至少具备：

   - 操作人身份可信。
   - 角色权限控制。
   - 审批人与提交人分离。
   - 审计日志不可伪造。

   如果部署在内网，也不应只依赖“没人知道接口”。

## 7. 数据库与基础设施评价

### 7.1 优点

1. Oracle 11g 约束考虑充分。

   表设计使用 `SEQUENCE`、`CLOB`、`NUMBER(18,4)`，符合 Oracle 11g 约束。

2. 关键唯一索引已经落地。

   包括：

   - `UK_PR_CRL_BIZ`：confirm 幂等业务键。
   - `UK_PR_CRV_NO`：reverse 幂等业务键。
   - `UK_PR_RV_RULE_PUBLISHED`：同规则最多一个发布版本。
   - `UK_PR_RAP_PENDING`：同规则版本同动作最多一个待审批。

3. 限额锁单独建 `PR_LIMIT_LOCK`。

   这比直接锁业务表更清晰，适合多维度锁键和时间窗小时桶。

4. 多实例缓存同步使用 `PR_CACHE_VERSION`。

   不引入 Redis 的情况下，这是一个务实方案。

### 7.2 风险与建议

1. 真实 Oracle 并发语义需要专项测试。

   单元测试不能完全证明：

   - `SELECT ... FOR UPDATE` 是否按预期阻塞。
   - 锁等待超时是否映射为正确错误码。
   - 唯一索引冲突是否稳定转业务异常。
   - 事务回滚是否覆盖全部仓储写入。
   - Oracle 连接池和 `IsAutoCloseConnection` 在事务中是否符合预期。

   建议建立一组 Oracle 集成测试或预发环境脚本，专测锁、并发、唯一约束和事务回滚。

2. 仓储层批量插入仍逐条取序列。

   当前做法简单可靠，但在大批量导入规则、批量追溯步骤、批量折价明细时可能有性能成本。可后续优化为批量取序列或数据库侧批量插入策略。

3. 普通字典缓存仍是单机 TTL 语义。

   `ACTION_TYPE_ORDER` 已有跨实例版本同步，但普通字典查询主要靠 TTL 和本机清理。若字典未来承载资金关键配置，建议纳入统一缓存版本体系。

## 8. API 与错误契约评价

### 8.1 优点

- API 路由清晰，核心端点齐全：simulate、batch-simulate、confirm、commit、cancel、reverse、special-flag、trace。
- 响应使用统一 `ApiResult`。
- 已引入结构化业务异常 `BizException` 和 `BizErrorCode`。
- 计价链路关键错误码已经覆盖价格不一致、幂等冲突、请求不存在、commit 明细不匹配、reverse 不允许等。

### 8.2 问题

1. 同时存在异常中间件和全局异常过滤器。

   `Program.cs` 实际注册的是 `ExceptionHandlerMiddleware`，但代码里还保留了 `GlobalExceptionFilter`，测试也覆盖它。两套映射长期并存会导致后续维护时出现分歧。

   建议保留一套正式入口，另一套删除或改成兼容薄包装。

2. 成功响应的 `TraceId` 没有形成统一链路。

   `LoggingBehavior` 生成了 trace id，异常中间件用 `HttpContext.TraceIdentifier`，计价业务又有自己的 `TraceId`。这些概念目前没有完全贯通。

   建议统一：

   - HTTP correlation id。
   - 业务计价 trace id。
   - 日志 trace id。
   - 响应 trace id。

3. 参数验证重复且口径略有差异。

   建议让 FluentValidation 作为入口参数校验，服务内部保留不可绕过的业务断言，但两者的规则和错误码要对齐。

## 9. 测试覆盖评价

### 9.1 已有覆盖很好

测试已经覆盖：

- 动作执行管线。
- 单次、日、时间窗、同组、同手术限额。
- 双单位换算、多 part 计算、面积阶梯、子项加收。
- 规则匹配和规则组作用域。
- confirm/commit/cancel/reverse 资金链。
- 幂等冲突。
- 规则发布冲突。
- 审批链路。
- 字典和公式维护。
- 缓存版本同步。
- API 路由和部分文档约束。
- HIS 客户端 SDK 基础行为。

255 个测试全部通过，是当前设计可信度的重要支撑。

### 9.2 还缺的测试类型

1. Oracle 真实集成测试。

   尤其是锁、事务、唯一索引、序列、CLOB、NUMBER 精度。

2. 并发压测。

   至少覆盖：

   - 同患者同项目 2 小时窗口并发 confirm。
   - 同一 `BusinessRequestNo` 并发 confirm。
   - commit 与 expire 并发。
   - reverse 与重复 reverse 并发。
   - 发布与计价读取并发。

3. 历史规则迁移验收。

   需要把 `docs/物价折价规则整理.xlsx` 中的代表性规则迁入后，用真实规则配置跑一批金标准用例。

4. HIS 端到端联调测试。

   单元测试能证明规则中心内部行为，但不能证明 HIS 明细号、收费单号、业务时间、重试号、退费号都能稳定传过来。

## 10. 可维护性与演进建议

### 10.1 P0：上线前必须处理

1. 补认证授权和管理接口权限。

   规则、字典、公式、审批、发布接口必须有身份和角色控制。

2. 做 Oracle 并发与事务专项验证。

   不能只靠内存替身和普通单元测试证明资金安全。

3. 统一异常处理入口。

   `ExceptionHandlerMiddleware` 和 `GlobalExceptionFilter` 二选一，避免未来错误码漂移。

4. 统一参数校验口径。

   FluentValidation 与应用服务校验必须一致，特别是数量是否允许 0、commit 明细是否必填、reverse 数量金额边界。

5. 完成 12 个资金安全验收用例的真实环境验证。

   重点是幂等、时间窗并发、过期释放、主子项目、reverse 超退、非等价回滚禁止普通计价。

### 10.2 P1：下一轮重构建议

1. 拆分 `PricingAppService`。

   建议拆为：

   - `PricingConfirmUseCase`
   - `PricingCommitUseCase`
   - `PricingCancelUseCase`
   - `PricingReverseUseCase`
   - `PricingRequestLogWriter`
   - `DiscountDetailWriter`
   - `LimitOccupyWriter`
   - `PricingResponseBuilder`

   拆分目标不是追求形式，而是让每条资金状态链可以单独审查。

2. 拆分 `RulePublishAppService`。

   发布状态机、审批门禁、冲突检测、测试门禁、动作参数校验、缓存失效应分开。

3. 引入统一时钟接口。

   用 `IClock` 收敛 `DateTime.Now`，提高测试稳定性和多机部署可控性。

4. 优化规则匹配读取模型。

   逐步从“按规则逐条读条件/动作”升级到“发布规则快照/只读投影/批量加载”。

5. 建立完整 correlation id。

   让 HTTP 请求、应用日志、计价 TraceId、数据库追溯链可以互相定位。

### 10.3 P2：中长期优化

1. 把发布门禁配置化。

   关键动作、必填参数、互斥动作、测试用例类型可以逐步从代码常量迁到字典或策略表。

2. 增加规则运行快照。

   发布时生成完整规则快照，计价时按快照运行，减少运行期多表读取和版本漂移风险。

3. 完善对账与运营报表。

   当前追溯链基础已经有了，下一步应向每日对账、异常报表、超时 pending 监控、规则变更影响分析推进。

4. 提升 SDK 和渠道降级治理。

   special-flag 已返回 `RollbackMode`，后续要在 HIS、自助机、微信侧严格执行，不允许特殊项目服务异常时普通计价。

## 11. 分项评分

| 模块 | 评分 | 说明 |
| --- | ---: | --- |
| 分层与依赖方向 | 8.5/10 | 方向正确，Domain 干净，Infrastructure 边界清楚 |
| 规则引擎 | 8/10 | 执行器模式成熟，后续需优化读取模型和快照 |
| 资金链路 | 8/10 | 幂等、锁、事务、状态推进都有，需真实并发验证 |
| 规则发布治理 | 8/10 | 审批、CAS、门禁都已补强，类过大需拆分 |
| 数据库设计 | 8/10 | Oracle 约束考虑充分，需集成测试证明语义 |
| API 契约 | 7/10 | 路由清晰，错误码在收敛，但异常入口和 trace id 需统一 |
| 测试 | 8/10 | 单元和应用测试较强，缺真实数据库和端到端验证 |
| 可维护性 | 7/10 | 核心思路好，但两个主服务继续膨胀会拖累后续 |
| 上线准备 | 6.5/10 | 适合预发联调，不建议未经并发压测和权限治理直接全量 |

## 12. 最终建议

当前 `src` 的设计不是“方向错了”，而是已经进入第二阶段问题：核心方向成立，资金安全链路也明显补强，接下来要防止复杂度继续集中在少数大类里，并用真实环境验证资金安全假设。

建议路线：

1. 保持现有四层结构和条件-动作-执行器模型。
2. 不要推翻 `confirm -> commit/cancel -> reverse/expire` 状态模型。
3. 下一轮优先做权限、Oracle 并发验证、异常入口统一、参数校验统一。
4. 再拆分 `PricingAppService` 和 `RulePublishAppService`，把资金状态机和发布门禁拆成可单独审查的组件。
5. 历史规则迁移前，先选 10 到 15 条代表性规则做金标准回归，再扩大到全部规则。

整体来看，这套源码已经具备规则中心的骨架和关键安全意识。只要下一步不继续把所有新逻辑堆进大服务，而是围绕资金链路和发布治理做结构化拆分，它可以支撑后续 HIS、自助机、微信多渠道统一计价改造。
