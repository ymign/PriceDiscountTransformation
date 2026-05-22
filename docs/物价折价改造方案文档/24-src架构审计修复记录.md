# 24-src架构审计修复记录

## 1. 目的

本文记录 2026-05 对 `src/` 规则中心源码的一轮架构审计和修复结果，重点覆盖：

- 资金链路 bug 修复
- 应用服务事务边界修正
- 发布与缓存失效补强
- 追溯链完整性补齐
- 特殊项目接口补充降级策略
- 已发布规则更新边界收紧

本文只记录已经落地并经过自动化测试验证的修复项；未纳入本轮代码的事项放到“后续跟踪”。

## 2. 已修复项

### 2.1 应用服务事务边界改为 `IUnitOfWork`

- `PricingAppService`
- `RulePublishAppService`
- `ExpireCleanupAppService`

原实现直接依赖 `ISqlSugarClient` 控制事务，导致单元测试难以替身化，也让应用层显式绑定 ORM。

本轮调整为依赖 `IUnitOfWork`：

- 应用层只声明事务边界，不依赖具体 ORM
- 测试可用 `NoopUnitOfWork` 替代
- 生产仍由 `SqlSugarUnitOfWork` 统一承接事务

### 2.2 补上遗漏测试项目

`src/Pricing.RuleCenter.slnx` 已纳入 `tests/Pricing.RuleCenter.Tests`。

此前只跑 `slnx` 会漏掉 API/应用层测试，形成假绿。

### 2.3 核心计价与批量上下文 bug 修复

- `UnitConvertExecutor`：换算后同步重算 `FinalAmount`
- `OnceQtyLimitExecutor`：`0` 视为“限制为零”，不再被当成“未配置”
- `BatchPricingContext`：`FinalQty = 0` 的结果不再占用同组互斥名额
- `SameOperationCeilingExecutorTests`：锁键和维度断言与现实现行口径对齐

### 2.4 reverse 幂等与资金释放修复

- 同一 `ReverseNo` 现在同时校验：
  - `ReverseQty`
  - `ReverseAmt`
  - `ReverseTime`
  - `ReversedBy`
  - `Reason`
- reverse 负向占额改为挂到本次 reverse 请求，而不是原请求
- reverse 请求日志、负向占额、冲正日志统一复用原请求 `TraceId`

修复后避免了两类资金风险：

- 同一 `ReverseNo` 改原因/操作人/退费时间被误判为幂等成功
- 负向占额错误挂原请求，导致全退或多次退费后净占用失真

### 2.5 TraceId 全链路补齐

新增并贯通了请求级 `TraceId`：

- `ChargeRequest`
- `ChargeTraceStep`
- `ChargeDiscountDetail`
- `LimitOccupy`
- `ChargeReverseLog`

当前 `simulate`、`confirm`、`reverse` 已能把同一次计价/冲正链路串起来，满足追溯查询和审计落点。

### 2.6 special-flag 接口补充有效期和降级模式

`GetSpecialFlagAsync` 现在：

- 只统计当前时间点有效的已发布规则
- 返回 `RollbackMode`
- 多条有效规则并存时返回更保守的降级策略

当前保守优先级：

- `STOP_CHARGE`
- `NEW_SERVICE_ONLY`
- `MANUAL_REVIEW`
- `LEGACY_EQUIVALENT`

这保证渠道在统一计价服务不可用时，不会因为缺少降级口径而回退普通计价。

### 2.7 已发布规则主档更新边界收紧

`RuleHeaderAppService.UpdateAsync` 现在禁止通过普通更新入口修改已发布规则的匹配关键字段：

- `RuleCategory`
- `RuleScope`
- `ItemCode`
- `GroupCode`
- `Priority`
- `EffectiveFrom`
- `EffectiveTo`
- `RollbackMode`

允许继续修改：

- `RuleName`
- `ItemName`
- `Remark`
- `UpdatedBy`

目的：防止绕过版本与发布状态机，直接篡改线上匹配口径。

### 2.8 规则运行期缓存失效补齐

新增 `IRuleRuntimeCacheInvalidator`，由 `RuleMatchService` 实现。

已接入两条失效路径：

- `RulePublishAppService` 发布/停用/回滚后清理动作顺序缓存
- `DictAppService` 修改 `ACTION_TYPE_ORDER` 字典后同步清理动作顺序缓存

这样动作顺序调整不再依赖服务重启。

### 2.9 发布前动作参数阻断补强

`RulePublishAppService` 现在会阻断一部分高风险缺参动作发布：

- `APPLY_TIME_WINDOW_LIMIT`
- `APPLY_DAY_LIMIT_QTY`
- `APPLY_ONCE_LIMIT_QTY`
- `APPLY_MAX_AMOUNT`
- `APPLY_MIN_AMOUNT`

阻断原则：

- 只拦“缺参会静默跳过或直接失真”的动作
- 暂不对 `CONVERT_QTY` 做发布阻断，避免误伤现有仅做冲突验证的历史测试数据

### 2.10 请求状态机口径统一

Core 聚合 `ChargeRequest` 的 `MarkCommitted` / `MarkReversed` 已统一到现行应用层口径：

- commit 后正式状态为 `CONFIRMED`
- reverse 允许从 `CONFIRMED` 进入
- `COMMITTED` 常量保留为兼容旧数据识别，不再作为新代码正式状态

### 2.11 reverse 全退释放口径修复

`PricingAppService.ReverseAsync` 本轮补了 3 个高风险边界：

- 同日全额退费时，不再同时：
  - 把原占额整体改成 `REVERSED`
  - 再额外插入负向 `REVERSE` 占额
- 原请求是否进入 `REVERSED`，现在同时要求：
  - 数量结清
  - 金额也结清
- 兼容旧数据时，`COMMITTED` 状态的原请求现在也允许进入 reverse

这次修复直接消除了两个资金风险：

- 全退后净占额被冲成负数，导致后续收费可多占额度
- 只退满数量、没退满金额，却把整单错误标记为 `REVERSED`

对应新增回归测试：

- `ReverseAsync_DoesNotInsertNegativeLimitOccupyForFullRefund`
- `ReverseAsync_AllowsCommittedLegacyStatus`
- `ReverseAsync_DoesNotMarkWholeRequestReversedWhenQuantityFullButAmountNotFull`

### 2.12 发布恢复与回滚目标修复

`RulePublishAppService` 本轮补了两类发布状态机缺陷：

- 规则从 `DISABLED` 重新发布时，显式恢复 `IsEnabled = Y`
- 回滚目标优先按 `PR_RULE_PUBLISH` 发布流水解析，而不是简单取“小于当前版本号的最大 DISABLED 版本”

同时在 `PublishAsync` / `DisableAsync` / `RollbackAsync` 的事务内重新读取当前主档和目标版本，减少事务外陈旧读导致的状态机穿透风险。

对应新增回归测试：

- `PublishAsync_ReEnablesRuleWhenPublishingFromDisabledHeader`
- `RollbackAsync_UsesPublishHistoryInsteadOfHighestDisabledVersion`

### 2.13 限额占用补齐明细级身份

`PR_LIMIT_OCCUPY` 本轮补了两列业务身份字段：

- `CHARGE_DETAIL_NO`
- `RESULT_GROUP_NO`

同时落地了三处配套改造：

- confirm 持久化限额占用时，透传费用明细的 `ChargeDetailNo`
- 主子项目/替换子项场景下，限额占用与折价明细共享同一 `ResultGroupNo`
- reverse 释放原占额时，匹配顺序改成：
  - 先按 `ResultGroupNo`
  - 再按 `ChargeDetailNo`
  - 老数据缺字段时再兼容回退到项目维度

这次修复解决的是一个结构性资金问题：

- 同一请求内如果存在“相同 `ItemCode`、不同 `ChargeDetailNo`”的多条收费明细，部分退费不再被迫按项目维度比例释放，而是优先释放命中的原占额

对应新增回归测试：

- `ReverseAsync_ReleasesOnlyMatchedChargeDetailOccupyWhenSameItemHasMultipleDetails`
- `ConfirmAsync_PersistsChargeDetailNoIntoLimitOccupies`

### 2.14 发布前门禁补齐首批硬阻断

`RulePublishAppService` 本轮补了三类真正执行的发布前硬阻断：

- 启用测试用例缺失阻断
- 启用测试用例最新运行未通过阻断
- `ADD_CHILD_ITEM` 重复子项目阻断
- 资金关键动作 `OnError != STOP` 阻断

当前门禁口径是“最小可验证闭环”，基于现有模型直接落地：

- 至少存在 1 条启用测试用例
- 每条启用测试用例必须具备 `InputJson` 和 `ExpectedJson`
- 每条启用测试用例必须至少有 1 次运行
- 每条启用测试用例的最新运行必须 `IsPass = Y`
- `ADD_CHILD_ITEM` 的 `childItems[].itemCode` 不能为空，且同一规则版本内不得重复
- 以下资金关键动作必须 `OnError = STOP`（空值和大小写 `stop` 视为兼容等价）：
  - `CONVERT_QTY`
  - `FORMULA_CALC`
  - `APPLY_DAY_LIMIT_QTY`
  - `APPLY_TIME_WINDOW_LIMIT`
  - `APPLY_ONCE_LIMIT_QTY`
  - `SAME_GROUP_MUTEX`
  - `APPLY_MIN_AMOUNT`
  - `APPLY_MAX_AMOUNT`
  - `SAME_OPERATION_CEILING`
  - `ADD_CHILD_ITEM`
  - `DISCOUNT_EXCEED_TO_ZERO`

对应新增回归测试：

- `PublishAsync_RejectsWhenEnabledTestCasesAreMissing`
- `PublishAsync_RejectsWhenLatestEnabledTestRunDidNotPass`
- `PublishAsync_RejectsDuplicateChildItemsInAddChildAction`
- `PublishAsync_RejectsCriticalActionWhenOnErrorIsNotStop`

### 2.15 发布状态机显式加锁

`RulePublishAppService` 本轮继续把发布一致性往数据库语义上收了一层：

- `IRuleHeaderRepository` 新增 `GetByIdForUpdateAsync`
- `IRuleVersionRepository` 新增 `GetByRuleAndVersionForUpdateAsync`
- 发布、停用、回滚在事务内不再只做“重新读取”，而是显式对主档和目标版本执行 `SELECT ... FOR UPDATE`

这次改动的目标不是彻底解决所有并发问题，而是先把最危险的窗口再缩小一层：

- 两个事务同时读取同一条规则主档并分别推进状态
- 两个事务同时读取同一目标版本并各自把它从 `DRAFT` 推进为 `PUBLISHED`

对应新增回归测试：

- `PublishAsync_LocksHeaderAndTargetVersionInsideTransaction`

### 2.16 版本状态推进补齐 CAS 语义

在显式加锁之外，本轮继续把 `PR_RULE_VERSION` 的状态推进补成了带期望旧状态的条件更新：

- `IRuleVersionRepository.UpdateStatusAsync` 新增 `expectedCurrentStatus`
- 发布时要求目标版本必须从 `DRAFT -> PUBLISHED`
- 停用时要求当前版本必须从 `PUBLISHED -> DISABLED`
- 回滚时要求：
  - 当前版本必须从 `PUBLISHED -> ROLLED_BACK`
  - 回滚目标版本必须从 `DISABLED -> PUBLISHED`

这一步的意义是：

- 就算事务已经拿到了锁，也不会在状态已经被外部推进后的情况下“盲写覆盖”
- 应用服务可以据此区分“正常推进失败”和“并发状态已变化”

同时，SQL 脚本已补上版本表的唯一索引约束草案：

- `UK_PR_RV_RULE_PUBLISHED`

约束语义：同一规则下最多只允许一个 `VERSION_STATUS = 'PUBLISHED'` 的版本。

对应新增回归测试：

- `PublishAsync_UsesCompareAndSetWhenPromotingDraftVersion`

### 2.17 主档状态推进补齐 CAS 语义

在版本表之外，本轮也把 `PR_RULE_HEADER` 的状态推进补成了带期望旧状态的条件更新：

- `IRuleHeaderRepository.UpdateAsync` 新增 `expectedCurrentStatus`
- 发布时主档只允许：
  - `DRAFT -> PUBLISHED`
  - `DISABLED -> PUBLISHED`
  - 已发布规则升级到新版本时保持 `PUBLISHED -> PUBLISHED`
- 停用时主档只允许：
  - `PUBLISHED -> DISABLED`
- 回滚时主档仍要求当前保持：
  - `PUBLISHED`

这一步的作用是让主档和版本表的并发保护口径保持一致：

- 事务内先 `FOR UPDATE` 锁住主档
- 真正落库时再用期望旧状态做一次 CAS

这样即使对象在事务内被多处改写，也不会在状态已变化后继续盲写覆盖。

对应新增/补强回归测试：

- `PublishAsync_UsesCompareAndSetWhenPromotingDraftVersion`

### 2.18 多实例缓存版本同步基础设施

本轮补了基于 Oracle 的最小跨实例缓存失效基础设施，不引入 Redis 或消息总线：

- 新增 `PR_CACHE_VERSION`
- 新增 `CacheVersionRepository`
- 新增 `ICacheVersionSynchronizer` / `CacheVersionSynchronizer`
- 新增后台轮询服务 `CacheVersionSyncService`

当前共享版本号作用域先收两类：

- `EFFECTIVE_RULES`
- `ACTION_TYPE_ORDER`

写侧行为：

- 规则发布/停用/回滚后递增：
  - `EFFECTIVE_RULES`
  - `ACTION_TYPE_ORDER`
- `ACTION_TYPE_ORDER` 字典变更后递增：
  - `ACTION_TYPE_ORDER`

读侧行为：

- `RuleHeaderAppService.GetEffectiveAsync` 读取生效规则前先做一次版本同步
- 后台轮询服务持续检查数据库版本，发现变化后清理本机：
  - 生效规则 `IMemoryCache`
  - `RuleMatchService` 的动作顺序静态缓存

这一步的目标不是完整分布式缓存系统，而是先把“其他实例长时间继续使用旧缓存”的风险收敛到版本号同步窗口内。

对应新增回归测试：

- `CacheVersionSynchronizerTests.IncreaseVersionAsync_ShouldPersistAndRememberLatestVersion`
- `CacheVersionSynchronizerTests.SyncAsync_ShouldClearRuleAndRuntimeCacheWhenDatabaseVersionChanges`
- `DictAppServiceTests.UpdateAsync_ClearsRuntimeCacheWhenActionTypeOrderChanges`

### 2.19 发布链路开始切换到结构化业务异常

本轮引入了轻量 `BizException`，并让 `GlobalExceptionFilter` 优先识别它：

- `BizException(code, httpStatusCode, message)`
- `GlobalExceptionFilter` 直接返回业务码和独立的 HTTP 状态码

当前先把规则发布链路里最密集的一批字符串异常切成结构化错误：

- 规则不存在 → `RuleNotFound`
- 规则版本不存在 → `RuleVersionNotFound`
- 规则重叠冲突 → `RuleOverlapConflict`
- 版本并发冲突 → `RuleVersionConcurrencyConflict`
- 主档并发冲突 → `RuleHeaderConcurrencyConflict`
- 测试用例缺失/不完整/未运行/失败
- `ADD_CHILD_ITEM` 子项目非法/重复
- 关键动作 `OnError` 非 `STOP`
- 动作参数 JSON 非法/关键参数缺失

这一步的收益有两个：

- 前端/HIS/SDK 不再需要依赖异常消息前缀猜测业务语义
- 业务码和 HTTP 状态码开始解耦，后续可以继续细化接口契约

对应新增/补强回归测试：

- `GlobalExceptionFilterTests.OnException_ShouldMapBizExceptionToConfiguredBusinessCodeAndHttpStatus`
- `RulePublishConflictTests` 中多条发布门禁测试已升级为直接断言 `BizErrorCode`
- `RulePublishConflictTests.PublishAsync_ReturnsRuleNotFoundBizCodeWhenHeaderIsMissing`
- `RulePublishConflictTests.PublishAsync_ReturnsRuleVersionNotFoundBizCodeWhenVersionIsMissing`

### 2.20 计价链路开始切换到结构化业务异常

本轮开始把 `PricingAppService` 中最核心的一批资金链路异常从
`InvalidOperationException + 消息前缀`
切到
`BizException + BizErrorCode`：

- `PRICE_MISMATCH`
- `IDEMPOTENT_CONFLICT`
- `REVERSE_*` 中的关键失败分支
- `COMMIT/CANCEL` 的核心状态校验分支

本轮优先收口的是“最影响接口契约和联调体验”的错误：

- 同一业务号/冲正号参数不一致
- 权威单价缺失或不一致
- 退费目标不存在、数量/金额越界、状态不允许
- commit 明细缺失、数量不匹配、金额不匹配、实际落账明细缺失

这样做以后，调用方不再需要依赖字符串前缀去区分：

- 幂等冲突
- 价格校验失败
- 退费不允许

对应新增/补强回归测试：

- `PricingReverseTests` 中冲正金额越界与幂等冲突已升级为直接断言 `BizErrorCode`
- `PricingApiServiceTests` 中 commit 数量/金额不匹配已升级为直接断言 `BizErrorCode`

### 2.21 规则条件/动作整体保存补上事务边界

`RuleConditionAppService.SaveAsync` 和 `RuleActionAppService.SaveAsync`
此前都采用：

- 先 `DeleteByRuleAndVersionAsync`
- 再 `InsertBatchAsync`
- 无事务保护

这会留下一个高风险窗口：

- 草稿版本先被整批清空
- 随后批量插入如果失败
- 该版本就会停留在“条件/动作全空”或部分重建后的不一致状态

本轮改为显式依赖 `IUnitOfWork`，把“删除旧定义 + 重建新定义”纳入同一事务：

- `RuleConditionAppService`
- `RuleActionAppService`

当前落地口径：

- 版本存在且为 `DRAFT` 的校验仍在事务外先做
- 真正有破坏性的“删旧 + 插新”在事务内执行
- 插入抛错时显式 `RollbackAsync`
- 审计变更日志继续保持 best-effort 旁路语义，不反向影响已提交成功的主配置保存

对应新增回归测试：

- `RuleDefinitionTransactionTests.SaveConditionsAsync_RollsBackDeleteWhenInsertFails`
- `RuleDefinitionTransactionTests.SaveConditionsAsync_CommitsRebuiltCollectionWhenInsertSucceeds`
- `RuleDefinitionTransactionTests.SaveActionsAsync_RollsBackDeleteWhenInsertFails`
- `RuleDefinitionTransactionTests.SaveActionsAsync_CommitsRebuiltCollectionWhenInsertSucceeds`

## 3. 自动化验证

本轮完成后已执行：

```powershell
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore --filter RuleDefinitionTransactionTests
dotnet test tests\Pricing.RuleCenter.Core.Tests\Pricing.RuleCenter.Core.Tests.csproj --no-restore
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore
dotnet test src\Pricing.RuleCenter.slnx --no-restore
git diff --check
```

结果：

- Core tests：38 通过
- API/Application tests：147 通过
- 解决方案测试：全部通过
- `git diff --check` 通过

说明：

- 运行中出现过一次 `MSB3026` 文件复制重试警告，随后测试成功完成，不影响结果判定
- .NET 10 preview SDK 提示为环境提示，不属于代码失败

## 4. 后续跟踪

以下问题仍建议继续推进，但本轮没有贸然扩大改动范围：

- 发布前校验“缺少测试用例、重复子项目、动作 `OnError` 非 `STOP`”等更完整阻断项
- `CONVERT_QTY` 的发布参数完整性校验，需要结合历史配置口径再收紧
- authority price 的时间版本追溯
- reverse 审计如果需要“一次退费多条逐明细冲正日志”，需补表设计或仓储接口
- `PR_LIMIT_OCCUPY` 目前已经补上 `ChargeDetailNo` / `ResultGroupNo`，但还没有补 `OriginalDiscountId`；如果后续要做到“完全按折价明细主键释放”，还需要继续补表和仓储
- 测试用例门禁目前还不能自动区分“正向用例/边界用例”，因为现有表结构没有 `CaseType` 或标签字段
- 规则发布并发一致性已补事务内 `FOR UPDATE` 锁、版本状态 CAS 和主档状态 CAS，但数据库侧仍缺对主档流程的更细粒度错误码和更强约束
- 多实例部署下的缓存已经具备基于 Oracle 版本号的同步基础设施，但读侧目前只先接了生效规则查询和动作顺序缓存，字典普通查询仍是单机内存 TTL 语义
- 计价链路（`PricingAppService`）已经开始切换 `BizException`，但仍有部分 `COMMIT_*`、`REVERSE_*` 和其余状态校验分支尚未全部完成结构化
- 规则条件/动作保存已经补了应用层事务边界，但版本状态校验仍是“事务外先读、事务内写”；如果后续要继续收紧到“草稿编辑串行化”，可再评估是否补 `GetByRuleAndVersionForUpdateAsync` 级别的锁定保存
- Trace 查询接口若要直接按 `TraceId` 聚合展示，可再补专门查询入口和测试

## 5. 结论

本轮修复后，`src/` 目录下最核心的几类问题已经收口：

- 事务边界更清晰
- 资金释放口径更安全
- 追溯链真正串起来了
- 已发布规则不再能被普通更新入口绕过
- 运行期缓存失效能跟上发布和字典维护

这些改动已经进入自动化测试保护范围，可以作为下一轮继续补发布阻断项和联调能力的基础版本。
