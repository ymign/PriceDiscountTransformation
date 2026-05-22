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

## 3. 自动化验证

本轮完成后已执行：

```powershell
dotnet test tests\Pricing.RuleCenter.Core.Tests\Pricing.RuleCenter.Core.Tests.csproj --no-restore
dotnet test tests\Pricing.RuleCenter.Tests\Pricing.RuleCenter.Tests.csproj --no-restore
dotnet test src\Pricing.RuleCenter.slnx --no-restore
git diff --check
```

结果：

- Core tests：38 通过
- API/Application tests：129 通过
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
- 规则发布并发一致性仍未做到数据库级硬约束；当前通过事务内重读降低了风险，但还没有落到 `SELECT FOR UPDATE` / CAS / 唯一约束
- 多实例部署下的规则缓存失效仍是进程内语义，未升级为分布式广播
- Trace 查询接口若要直接按 `TraceId` 聚合展示，可再补专门查询入口和测试

## 5. 结论

本轮修复后，`src/` 目录下最核心的几类问题已经收口：

- 事务边界更清晰
- 资金释放口径更安全
- 追溯链真正串起来了
- 已发布规则不再能被普通更新入口绕过
- 运行期缓存失效能跟上发布和字典维护

这些改动已经进入自动化测试保护范围，可以作为下一轮继续补发布阻断项和联调能力的基础版本。
