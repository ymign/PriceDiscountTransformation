# 仓库指南

## 项目结构与模块组织

本仓库目前包含医院物价折价改造项目的设计文档、.NET 6 规则中心源码、HIS 客户端示例、Oracle SQL 初始化脚本和自动化测试。

- `CLAUDE.md`：项目背景、架构说明、技术约束和核心业务规则。
- 根目录 `.txt` 文件：原始需求，包括折价标识、上下限、计价公式、双单位换算等。
- `docs/物价折价规则整理.xlsx`：待迁移的历史物价折价规则。
- `docs/医保立项指南服务改造方案.pptx`：业务背景资料。
- `docs/物价折价改造方案文档/`：编号设计文档，建议按 `01` 到 `13` 顺序阅读。
- `src/`：规则中心 API、Core、Infrastructure 源码。
- `tests/`：xUnit 自动化测试。
- `his-client/`：HIS 端集成示例与 WinForms 工作台/弹窗代码。
- `sql/`：Oracle 11g 建表、字典初始化、公式初始化和验证脚本。

## 构建、测试与开发命令

常用本地检查命令如下：

```powershell
git status --short --branch
rg --files
dotnet build src\Pricing.RuleCenter.slnx --no-restore
dotnet test src\Pricing.RuleCenter.slnx --no-restore
git diff -- docs
```

编辑前用 `git status` 确认工作区状态；用 `rg --files` 查看文件结构；提交前用 `git diff` 检查变更。

## 编码风格与命名规范

Markdown 文档应使用清晰标题、短段落和一致术语。新增方案文档继续使用编号命名，例如 `10-新主题.md`。业务术语保持一致：`confirm`、`commit`、`cancel`、`reverse`、`PR_` 表名前缀、金额使用 `decimal`。

新增或修改 C# 代码时，遵循 .NET 常规风格：公共类型和成员使用 PascalCase，局部变量和参数使用 camelCase，异步方法以 `Async` 结尾，缩进使用四个空格。金额计算禁止使用 `double` 或 `float`，C# 使用 `decimal`，Oracle 使用 `NUMBER(18,4)`。

## 测试要求

已有 xUnit 自动化测试。代码变更后优先运行 `dotnet test src\Pricing.RuleCenter.slnx --no-restore`。重点保持规则匹配、公式计算、双单位换算、金额/数量限制、并发占额、`confirm` 幂等、退费与冲正相关测试覆盖。

## 提交与合并请求规范

现有提交信息使用简洁的祈使句，例如 `Add price discount transformation docs`。后续提交应聚焦单一逻辑变更，避免把无关格式调整混入同一提交。

合并请求应说明变更摘要、影响的文档或模块、关键业务规则变化以及未确认问题。涉及 HIS 界面或流程时，补充截图或流程说明。相关任务可引用 `07-开发任务清单-物价折价改造.md`。

## Agent 专用说明

修改前先阅读 `CLAUDE.md` 和相关编号文档。保留用户已有内容，除非明确要求替换。避免对 Word、Excel、PowerPoint 等来源文件做无关格式变更。
