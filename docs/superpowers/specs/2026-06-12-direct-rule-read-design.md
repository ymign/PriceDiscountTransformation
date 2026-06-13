# 纯直接规则读链路替代运行时包设计

## 背景

当前 `src` 同时存在两套规则运行路径：

- 旧规则表直读：`PR_RULE_HEADER / PR_RULE_CONDITION / PR_RULE_ACTION`
- 运行时包直读：`PR_RUNTIME_* + ACTIVE_PACKAGE`

结合当前业务约束，规则启用只会发生在无人收费时段，收费链路不需要“白天热切换”“请求内冻结活动包”“整包灰度切换”这些能力。现有运行时包在计价接口里的主要作用，已经退化成：

- 决定计价和 `special-flag` 优先读哪套规则源
- 在响应和追溯表里写入 `RuntimePackageId/RuntimePackageVersion`

这套能力增加了读链路复杂度、定位成本和维护成本，但当前业务场景无法充分兑现其收益。

## 目标

把 `PricingController` 相关的计价和特殊项目判断链路，统一切回“直接规则读模型”：

- 计价匹配统一从 `PR_RULE_*` 读取
- `special-flag` 统一从 `PR_RULE_*` 判断
- 响应、请求日志、追溯步骤、折价明细不再携带运行时包信息
- 恢复旧规则维护入口，不再强制要求模板/策略/运行时包发布流程

## 非目标

- 本轮不重构计价公式、执行器、限额占用等核心计价逻辑
- 本轮不改 Oracle 表结构和历史数据迁移脚本
- 本轮不重做模板/策略平台建模，只做退役或隔离

## 设计原则

1. 先切读链路，再退平台侧代码，避免一步删穿。
2. 每个批次都要保证系统可编译、接口可运行、测试可验证。
3. 优先删除“运行时包作为读模型”的依赖，不先碰无关规则能力。
4. 能直接复用现有 `PR_RULE_*` 仓储与缓存的，不新增中间抽象。

## 分批方案

### 批次一：计价读链路切回直接规则

目标：让 `simulate / batch-simulate / confirm` 的规则匹配不再依赖运行时包。

范围：

- 删除 `EffectiveRuleSnapshotLoader` 中的运行时包优先分支
- 删除 `ActiveRuntimePackageReader`
- 删除 `RuleMatchRepositories` 中运行时包读依赖
- 删除 `RuntimePackageTraceResolver / RuntimePackageTraceResolution`
- 删除 `PricingSimulateWorkflow / PricingConfirmWorkflow` 中运行时包解析

结果：

- `PricingController` 的收费主链路只读 `PR_RULE_*`
- 运行时包不再参与计价结果生成

### 批次二：特殊项目判断与响应追溯字段清理

目标：让 `special-flag` 和对外/落库 DTO 全部摆脱运行时包字段。

范围：

- 删除 `PricingSpecialFlagResolver` 中运行时包优先逻辑
- 删除 `PricingResponseDto / PricingSpecialFlagDto` 中运行时包相关字段
- 删除 `PricingResponseBuilder / PricingRequestLogWriter / PricingTraceStepWriter / PricingDiscountDetailWriter` 中运行时包追溯写入

结果：

- `GET /api/pricing/items/{itemCode}/special-flag` 只按旧规则表判断
- 计价响应和追溯表只保留“规则事实”，不再混入“运行时包事实”

### 批次三：启动注册与旧规则维护入口恢复

目标：把启动注册和维护接口切到“直接规则模式”。

范围：

- 删除 `RuleCenterApiServiceCollectionExtensions`、`Infrastructure.DependencyInjection` 中运行时包读依赖注册
- 放开 `LegacyRuleAuthoringGuardFilter`
- 取消旧规则写控制器上的退役保护

结果：

- API 启动不再注入运行时包读链路
- 旧规则写维护入口恢复为正式入口

### 批次四：平台侧运行时包退役

目标：把不再需要的运行时包接口和服务从主系统中退役。

范围：

- 删除 `RuntimePackageController`
- 删除 `RuntimePackageActivationService / RuntimePackagePublishService / RuntimePackageRollbackService / RuntimePackageQueryAppService / RuntimePackageCompiler`
- 清理对应控制器注册、Swagger 分组与死代码引用

结果：

- 系统正式从“双轨模式”收敛到“纯直接规则模式”

## 风险与应对

### 风险一：响应契约变更影响调用方

运行时包字段从响应 DTO 删除后，若上游已消费这些字段，需要同步接口契约。

应对：

- 第二批前先确认是否已有渠道依赖 `runtime_package_id/runtime_package_version`
- 如需兼容，可先保留字段但固定返回空值，下一版再删

### 风险二：平台侧代码删得过快，导致管理端短时失效

应对：

- 保持“批次三先恢复旧规则维护入口，批次四再删运行时包平台”
- 确保管理端先有稳定可用的新入口再退役旧平台

### 风险三：历史追溯数据存在运行时包字段

应对：

- 本轮不清历史数据，不动表结构
- 只停止新数据写入运行时包信息

## 建议结论

在“规则启用只发生在无人收费时段”的业务前提下，运行时包在计价主链路中的收益不足以覆盖其复杂度成本。建议按上述四个批次，逐步把系统收敛为“纯直接规则读链路”。
