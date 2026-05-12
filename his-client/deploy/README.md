# PricingAgent 部署与回滚手册

## 交付包内容

- `HIS.Pricing.Client.dll`
- `Newtonsoft.Json.dll`
- `pricing-agent.config`
- `install-pricing-agent.ps1`
- `rollback-pricing-agent.ps1`

## 首次安装或升级

```powershell
.\install-pricing-agent.ps1 `
  -PackageDirectory D:\deploy\PricingAgent `
  -TargetDirectory D:\HIS\Client
```

脚本会先把目标目录中已有 DLL、PDB、`pricing-agent.config` 备份到
`D:\HIS\Client\pricing-agent-backup\yyyyMMddHHmmss`，再复制新文件。

## 回滚

回滚到最近一次备份：

```powershell
.\rollback-pricing-agent.ps1 -TargetDirectory D:\HIS\Client
```

回滚到指定备份：

```powershell
.\rollback-pricing-agent.ps1 `
  -TargetDirectory D:\HIS\Client `
  -BackupName 20260512103000
```

## 上线验证

- HIS 能加载 `HIS.Pricing.Client.dll`。
- `pricing-agent.config` 中 `BaseUrl` 指向当前院区计价服务。
- Agent 诊断窗口中服务健康检查通过，协议版本为 `1.0`。
- special-flag、simulate、confirm、commit 至少各完成一笔测试。
- `pricing-agent-logs` 目录产生当天日志。
- 人为断开网络后，commit/cancel/reverse 失败会在 `pricing-agent-pending` 目录生成 JSON 待补偿记录。
- 收费窗口在现场分辨率、DPI、双屏和键盘操作下完成走查。
