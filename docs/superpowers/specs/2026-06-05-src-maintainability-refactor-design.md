# src 可维护性分批改造设计

## 背景

`src/` 当前可构建，但应用服务、状态机、审批保护和项目分层已经出现维护困难。用户已确认按前面归纳的 6 点分批完成改造。

## 目标

1. 修复当前 3 个红色测试，避免在红测基础上继续重构。
2. 抽出规则编辑冻结能力，避免 Header/Condition/Action 各自判断待审状态。
3. 拆分 `PricingAppService` 中最重的辅助职责，让主服务逐步退化为编排入口。
4. 新增 `Pricing.RuleCenter.Application` 项目，把应用服务和 DTO 从 Web API 项目中分离。
5. 统一常用状态/启用标识引用，减少魔术字符串。
6. 清理本轮触碰代码中的冗余注释，保留资金/状态关键说明。

## 设计原则

- 先红绿再重构：已有失败测试作为第一批红测；后续每批保持 build/test 可运行。
- 小步迁移：保留控制器 API 和对外 DTO 结构，不做接口破坏性变化。
- Facade 兼容：`PricingAppService` 可先保留为对控制器的门面，逐步抽离内部职责。
- 不扩大业务语义：本轮不新增审批流程、不改 SQL 表结构、不重写规则引擎。

## 批次划分

### 批次 1：规则编辑冻结与红测修复

创建 `RuleEditGuard`，统一基于规则变更日志判断指定规则/版本是否存在待处理的 PUBLISH 审批。Header 更新、条件保存、动作保存在写入前调用它。

### 批次 2：拆分 PricingAppService 辅助职责

先抽出纯逻辑/低依赖能力：commit 实落明细校验、请求指纹构建、锁键生成等。主服务保留 use-case 入口，减少单文件职责。

### 批次 3：Application 项目迁移

新增 `Pricing.RuleCenter.Application`，迁移 `Application/` 与 `Dto/`。API 项目只保留 Controllers、Filters、Program 和配置文件。

### 批次 4：状态常量与注释瘦身

本轮触碰代码统一使用 `StatusCodes` 常量；删除重复解释型注释，保留关键业务约束注释。

## 验证

每批至少运行相关测试；最终运行：

```powershell
dotnet build src\Pricing.RuleCenter.slnx --no-restore
dotnet test src\Pricing.RuleCenter.slnx --no-restore
```
