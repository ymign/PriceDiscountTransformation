# HIS 集成与部署方案

## 1. 文档目的

本文补充统一计价规则中心与 HIS 现有系统的集成细节、灰度发布方案、性能容量评估和上线检查清单。

技术前提：
- HIS：C# Windows Forms/WPF
- 数据库：Oracle 11g
- 计价服务：ASP.NET Core Web API

---

## 2. HIS 集成架构

### 2.1 部署拓扑

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   HIS 客户端  │     │  自助机服务   │     │  微信公众号   │
│  (WinForms)  │     │             │     │             │
└──────┬───────┘     └──────┬──────┘     └──────┬──────┘
       │                    │                    │
       └────────────────────┼────────────────────┘
                            │
                    ┌───────▼────────┐
                    │  统一计价服务    │
                    │ ASP.NET Core   │
                    │ Web API        │
                    └───────┬────────┘
                            │
                    ┌───────▼────────┐
                    │   Oracle 11g   │
                    │  (PR_* 表)     │
                    └────────────────┘
```

### 2.2 计价服务与 HIS 数据库的关系

两种方案：

**方案 A：独立库（推荐）**

计价服务使用独立的 Oracle Schema（如 PRICING），与 HIS 主库分离。

优点：
- 计价服务故障不影响 HIS 核心业务
- 独立备份和恢复
- 权限隔离

缺点：
- 跨库查询对账时需要 DB Link
- 部署稍复杂

**方案 B：同库不同 Schema**

计价服务的 PR_* 表与 HIS 表在同一个 Oracle 实例中，但使用不同 Schema。

优点：
- 部署简单
- 对账查询方便

缺点：
- 计价服务的锁可能影响 HIS 性能
- 权限管理复杂

**建议：** 采用方案 A，对账通过定时同步或 DB Link 实现。

---

## 3. HIS 前端改造

### 3.1 折价页签改造

在"非药品维护-物价详细信息维护"窗体中：

**新增 Tab 页：**
- Tab 名称：折价
- 位置：常规 与 其他 之间
- 控件：TabControl 的 TabPages.Insert(1, tabDiscount)

**页签内字段布局：**

```
┌─────────────────────────────────────────────────┐
│ [折价]                                           │
│                                                  │
│ ☑ 折价项目                                       │
│                                                  │
│ ── 数量限制 ──────────────────────────────────    │
│ 单日收费上限数量: [____]                           │
│ 单次收费上限数量: [____]                           │
│ 2小时内收费上限数量: [____]                        │
│                                                  │
│ ── 金额限制 ──────────────────────────────────    │
│ 单日收费上限金额: [____]                           │
│ 单日收费下限金额: [____]                           │
│                                                  │
│ ── 公式计价 ──────────────────────────────────    │
│ 公式类型: [下拉框▼]                               │
│                                                  │
│ ── 双单位换算 ─────────────────────────────────   │
│ 计价类型: [下拉框▼]    计价单位: [下拉框▼]         │
│ 计价数量: [____]      换算单位: [下拉框▼]         │
│ 换算数量: [1] (固定不可改)                        │
│                                                  │
│ ── 规则明细 ──────────────────────────────────    │
│ [DataGridView: 部位 | 计价类型 | 计价单位 | ...]  │
│ [新增] [编辑] [删除]                              │
└─────────────────────────────────────────────────┘
```

**字段校验规则：**

| 字段 | 校验 | 说明 |
|---|---|---|
| 数量字段 | ≥0 的整数，可为空 | NULL 表示不校验；0 表示限制为零 |
| 金额字段 | ≥0，保留2位小数，可为空 | NULL 表示不校验；0 表示限制为零 |
| 上限 vs 下限 | 上限 ≥ 下限 | 同时填写时校验 |
| 换算数量 | 固定为1，禁用 | 不可编辑 |
| 公式类型 | 下拉选择 | 空值 = 不启用公式计价 |

**下拉框数据源：**

| 下拉框 | 字典类型 | 来源 |
|---|---|---|
| 公式类型 | FORMULA_TYPE | PR_DICT |
| 计价类型 | PRICING_TYPE | PR_DICT |
| 计价单位 | PRICING_UNIT | PR_DICT |
| 换算单位 | CONVERT_UNIT | PR_DICT |

### 3.2 收费弹窗

当收费员录入项目后，如果项目为特殊项目（调用 special-flag 接口判断），弹出计价输入窗体。

**弹窗触发条件：**

```
1. 收费员在收费录入界面输入项目编码
2. 调用 GET /api/pricing/items/{itemCode}/special-flag
3. 如果 special = true，弹出计价输入窗体
4. 如果 special = false，按原流程处理
5. 如果接口异常或超时，按 special = true 处理，禁止绕回普通计价
```

说明：特殊项目判断接口异常时必须走安全策略。宁可多进入一次计价弹窗或提示服务不可用，也不能漏掉特殊项目后按普通价格收费。

**弹窗布局：**

```
┌─────────────────────────────────────────────────┐
│ 计价参数输入                                      │
│                                                  │
│ 项目: CT平扫 (CT001)                             │
│ 单价: 300.00 元                                  │
│                                                  │
│ 部位: [下拉框▼]     数量: [____]                 │
│ 面积: [____] 平方厘米                            │
│ 长度: [____] 厘米                                │
│                                                  │
│ ── 试算结果 ──────────────────────────────────   │
│ 原始金额: 1200.00                                │
│ 折价金额: 300.00                                 │
│ 最终金额: 900.00                                 │
│ 折价原因: 2小时内收费上限3，超出1个折价为0元        │
│                                                  │
│        [试算]  [确认收费]  [取消]                 │
└─────────────────────────────────────────────────┘
```

**操作流程：**

```
1. 弹窗打开后，收费员填写部位、数量等参数
2. 点击 [试算]：调用 simulate 接口，展示预估结果
3. 收费员可修改参数后再次试算
4. 点击 [确认收费]：调用 confirm 接口
5. confirm 成功后，将结果回填到 HIS 收费录入界面
6. HIS 继续执行原有的落账流程
7. 落账成功后，HIS 调用 commit 接口
8. 落账失败或取消，HIS 调用 cancel 接口
```

**结果回填字段：**

| HIS 字段 | 来源 | 说明 |
|---|---|---|
| 数量 | response.finalQty | 最终收费数量 |
| 金额 | response.finalAmount | 最终金额 |
| 折价金额 | response.discountAmount | 折价金额（展示用） |
| 折价原因 | response.reasonDesc | 折价原因（展示用） |
| 追溯号 | response.traceId | 关联追溯记录 |

### 3.3 HIS 调用示例代码

```csharp
// 特殊项目判断
public async Task<bool> IsSpecialItemAsync(string itemCode)
{
    var response = await _httpClient.GetAsync(
        $"{_pricingBaseUrl}/api/pricing/items/{itemCode}/special-flag");
    
    if (!response.IsSuccessStatusCode)
    {
        // 接口异常时按特殊项目处理（安全策略）
        return true;
    }
    
    var result = await response.Content.ReadFromJsonAsync<SpecialFlagResponse>();
    return result?.Special ?? true;
}

// 试算
public async Task<PricingResponse> SimulateAsync(PricingRequest request)
{
    request.CallType = "SIMULATE";
    request.RequestNo = $"SIM_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
    
    var response = await _httpClient.PostAsJsonAsync(
        $"{_pricingBaseUrl}/api/pricing/calculate/simulate", request);
    
    return await response.Content.ReadFromJsonAsync<PricingResponse>();
}

// 确认计价
public async Task<PricingResponse> ConfirmAsync(PricingRequest request)
{
    request.CallType = "CONFIRM";
    request.RequestNo = $"CFM_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
    
    var response = await _httpClient.PostAsJsonAsync(
        $"{_pricingBaseUrl}/api/pricing/calculate/confirm", request);
    
    return await response.Content.ReadFromJsonAsync<PricingResponse>();
}

// HIS 落账成功后提交
public async Task<bool> CommitAsync(string traceId)
{
    var response = await _httpClient.PostAsJsonAsync(
        $"{_pricingBaseUrl}/api/pricing/calculate/commit",
        new { TraceId = traceId });
    
    return response.IsSuccessStatusCode;
}

// HIS 落账失败或取消
public async Task<bool> CancelAsync(string traceId)
{
    var response = await _httpClient.PostAsJsonAsync(
        $"{_pricingBaseUrl}/api/pricing/calculate/cancel",
        new { TraceId = traceId });
    
    return response.IsSuccessStatusCode;
}
```

---

## 4. 收费入口改造清单

| 收费入口 | 改造内容 | 优先级 |
|---|---|---|
| 门诊收费 | 录入项目后判断特殊项目 → 弹窗 → 调用计价服务 | P0 |
| 住院收费 | 同门诊收费 | P0 |
| 手术批费 | 同门诊收费，额外传入 operationNo | P0 |
| 补缴费 | 同门诊收费 | P1 |
| 急诊收费 | 同门诊收费 | P1 |
| 自助机 | 调用 simulate 展示价格 → 支付前调用 confirm | P1 |
| 微信公众号 | 调用 simulate 展示价格 → 确认支付前调用 confirm | P1 |

---

## 5. 灰度发布方案

### 5.1 灰度开关设计

在 PR_RULE_HEADER 表中，IS_ENABLED 字段控制单条规则是否启用。

额外增加全局灰度配置表：

```sql
CREATE TABLE PR_GRAYSCALE_CONFIG (
    CONFIG_ID     NUMBER(18,0) PRIMARY KEY,
    CONFIG_KEY    VARCHAR2(100) NOT NULL,  -- 配置键
    CONFIG_VALUE  VARCHAR2(500),           -- 配置值
    DESCRIPTION   VARCHAR2(500),
    IS_ENABLED    CHAR(1) DEFAULT 'Y',
    UPDATED_AT    DATE,
    UPDATED_BY    VARCHAR2(50)
);
```

预定义配置键：

| CONFIG_KEY | 说明 | 示例值 |
|---|---|---|
| GLOBAL_SWITCH | 全局开关，关闭后所有渠道走旧逻辑 | Y/N |
| ENABLED_SYSTEMS | 启用的渠道列表 | HIS,SELF_MACHINE |
| ENABLED_SCENES | 启用的收费场景 | OUTPATIENT_CHARGE |
| ENABLED_ITEMS | 启用的项目列表（为空则全部启用） | CT001,CT002 |
| ENABLED_BRANCHES | 启用的院区（多院区医院） | MAIN,BRANCH_01 |

### 5.2 灰度切换流程

**第一阶段：影子模式（1-2周）**

```
1. 计价服务上线，但不接入 HIS 收费流程
2. HIS 收费时仍走旧逻辑
3. 同时异步调用计价服务 simulate 接口（不影响收费）
4. 比对旧逻辑和新服务的结果差异
5. 差异记录到对账表，人工核查
6. 确认无差异后进入下一阶段
```

**第二阶段：按项目灰度（2-4周）**

```
1. 选择 3-5 个简单项目（如只有数量上限的项目）
2. 在 PR_GRAYSCALE_CONFIG 中配置 ENABLED_ITEMS
3. 这些项目走新计价服务，其他项目走旧逻辑
4. 监控计价结果、性能、异常
5. 逐步扩大项目范围
```

**第三阶段：按渠道灰度（2-4周）**

```
1. HIS 先接入（最核心，先验证）
2. 自助机接入
3. 微信公众号接入
```

**第四阶段：全量切换**

```
1. 所有特殊项目都走新计价服务
2. 旧逻辑代码保留但不再调用
3. 运行稳定 1 个月后，下线旧逻辑
```

### 5.3 回滚方案

| 回滚级别 | 操作 | 影响范围 |
|---|---|---|
| 单规则回滚 | 将 PR_RULE_HEADER.IS_ENABLED 设为 N | 仅该规则 |
| 单项目回滚 | 从 ENABLED_ITEMS 中移除该项目编码 | 仅该项目 |
| 单渠道回滚 | 从 ENABLED_SYSTEMS 中移除该渠道 | 仅该渠道 |
| 全局回滚 | GLOBAL_SWITCH 设为 N | 所有渠道 |

回滚后，所有渠道自动走旧逻辑，无需重新发版。

---

## 6. 性能容量评估

### 6.1 容量假设

基于典型三甲医院数据：

| 指标 | 假设值 | 说明 |
|---|---|---|
| 日均收费笔数 | 5,000-10,000 | 非药品项目 |
| 日均特殊项目笔数 | 500-2,000 | 需调用计价服务 |
| 峰值 QPS | 20-50 | 早上门诊开单高峰 |
| 试算次数/确认次数 | 3:1 | 平均试算3次后确认1次 |
| 患者日均特殊项目数 | 2-5 | 平均每个患者 |
| 规则总数 | 100-500 | 初期74条，逐步扩展 |

### 6.2 性能目标

| 接口 | 目标 P99 | 目标 QPS | 说明 |
|---|---|---|---|
| simulate | ≤500ms | ≥100 | 试算 |
| confirm | ≤1s | ≥50 | 确认 |
| special-flag | ≤100ms | ≥200 | 轻量查询 |
| batch-simulate | ≤2s | ≥20 | 批量 |

### 6.3 数据库性能设计

**关键索引：**

```sql
-- 规则查询（最高频）
CREATE INDEX IDX_PR_RULE_HEADER_ITEM
ON PR_RULE_HEADER (ITEM_CODE, IS_ENABLED, STATUS, EFFECTIVE_FROM, EFFECTIVE_TO);

-- 限额占用查询
CREATE INDEX IDX_PR_LIMIT_OCCUPY_LOOKUP
ON PR_LIMIT_OCCUPY (LIMIT_KEY, STATUS, OCCUPIED_AT);

-- 请求日志查询
CREATE INDEX IDX_PR_REQ_PATIENT_DATE
ON PR_CHARGE_REQUEST_LOG (PATIENT_ID, REQUEST_AT);

CREATE INDEX IDX_PR_REQ_CHARGE
ON PR_CHARGE_REQUEST_LOG (CHARGE_NO, ITEM_CODE);

-- 折价结果查询
CREATE INDEX IDX_PR_DISC_PATIENT_DATE
ON PR_CHARGE_DISCOUNT_DETAIL (PATIENT_ID, OCCURRED_AT);

CREATE INDEX IDX_PR_DISC_ITEM_DATE
ON PR_CHARGE_DISCOUNT_DETAIL (ITEM_CODE, OCCURRED_AT);
```

**PR_LIMIT_LOCK 锁行创建：**

PR_LIMIT_LOCK 用于 `SELECT FOR UPDATE` 锁定。由于 `LIMIT_KEY` 包含患者、项目、日期、小时、手术号、孕次号等动态维度，不要求提前预初始化所有锁键。

推荐策略：

1. 计价引擎根据规则生成本次需要锁定的 `LOCK_KEY` 列表。
2. 对不存在的锁键先尝试插入 `PR_LIMIT_LOCK`。
3. 如果并发插入同一锁键触发唯一键冲突，忽略该错误，继续查询已有锁行。
4. 按 `LOCK_KEY` 字典序依次 `SELECT ... FOR UPDATE`，避免多锁键场景死锁。
5. 锁定完成后再查询 `PR_LIMIT_OCCUPY` 并计算剩余额度。

预初始化只能作为性能优化，用于提前创建少量确定性强的锁键；不能作为正确性依赖。

### 6.4 缓存策略

| 缓存对象 | 缓存位置 | TTL | 失效时机 |
|---|---|---|---|
| 规则配置 | 计价服务内存 | 5 分钟 | 规则发布/停用/回滚时主动失效 |
| 字典数据 | 计价服务内存 | 30 分钟 | 字典变更时主动失效 |
| special-flag | 不缓存 | - | 每次实时查询 |

规则缓存实现建议：

```csharp
// 使用 MemoryCache + 主动失效
public class RuleCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IRuleRepository _repo;

    public async Task<List<RuleHeader>> GetRulesAsync(string itemCode)
    {
        var cacheKey = $"rules:{itemCode}";
        if (!_cache.TryGetValue(cacheKey, out List<RuleHeader> rules))
        {
            rules = await _repo.GetPublishedRulesAsync(itemCode);
            _cache.Set(cacheKey, rules, TimeSpan.FromMinutes(5));
        }
        return rules;
    }

    // 规则变更时调用
    public void Invalidate(string itemCode)
    {
        _cache.Remove($"rules:{itemCode}");
    }

    // 全局失效（发布/停用/回滚时）
    public void InvalidateAll()
    {
        // 清除所有规则缓存
        // 实际实现可用 CacheEntryRemovedCallback 或标记版本号
    }
}
```

---

## 7. 对账集成

### 7.1 HIS 数据抽取

每日凌晨对账需要从 HIS 收费明细表抽取数据。需要确认的 HIS 表：

| HIS 表名 | 用途 | 关键字段 |
|---|---|---|
| （待确认）收费明细表 | 核对金额一致性 | 收费单号、项目编码、数量、金额、收费时间 |
| （待确认）退费表 | 核对退费冲正 | 退费单号、原收费单号、退费金额 |
| （待确认）物价项目表 | 核对项目编码映射 | 项目编码、项目名称 |

**注意：** 以上表名需要与 HIS 开发人员确认。对账任务通过 DB Link 或定时同步获取 HIS 数据。

### 7.2 对账异常处理

发现异常后的处理路径：

```
L1 异常（金额不一致）：
  1. 对账报表标记为 L1
  2. 通知财务和信息科
  3. 人工核查原因
  4. 如果是计价服务错误 → 修复规则 → 补费/退费
  5. 如果是 HIS 错误 → 通知 HIS 修复

L2 异常（状态不一致）：
  1. 对账报表标记为 L2
  2. 通知信息科
  3. 检查是否需要补调接口（如漏调 commit）
  4. 检查是否需要释放占用（如 CONFIRM_PENDING 超时）

L3 异常（追溯缺失）：
  1. 对账报表标记为 L3
  2. 定期批量修复
```

---

## 8. 上线检查清单

### 8.1 上线前

| 检查项 | 状态 | 说明 |
|---|---|---|
| PR_* 表全部创建完成 | ☐ | 包括索引和序列 |
| 字典数据初始化完成 | ☐ | 计价类型、计价单位、公式类型、收费场景 |
| 历史规则导入完成 | ☐ | 74条规则全部映射并导入 |
| 计价服务部署完成 | ☐ | 含健康检查接口 |
| HIS 前端改造完成 | ☐ | 折价页签、收费弹窗、结果回填 |
| HIS 接口调用代码完成 | ☐ | simulate、confirm、commit、cancel |
| 灰度配置初始化 | ☐ | GLOBAL_SWITCH = N（初始关闭） |
| 对账任务部署完成 | ☐ | 定时任务配置 |
| 监控告警配置完成 | ☐ | SLA 报警、熔断报警 |
| 测试用例全部通过 | ☐ | 含规则测试用例 |
| 回滚方案验证通过 | ☐ | 确认可以随时切回旧逻辑 |

### 8.2 上线后第一周

| 检查项 | 频率 | 说明 |
|---|---|---|
| 计价接口成功率 | 每小时 | 低于 99.9% 立即排查 |
| 计价接口延迟 | 每小时 | P99 超过 2s 立即排查 |
| 对账一级异常 | 每天 | 有异常当天处理 |
| CONFIRM_PENDING 超时 | 每天 | 超过 10 条排查原因 |
| 灰度项目计价结果 | 每天 | 人工抽查 10 笔 |

### 8.3 上线后第一月

| 检查项 | 频率 | 说明 |
|---|---|---|
| 规则命中率统计 | 每周 | 分析规则配置是否合理 |
| 折价金额汇总 | 每周 | 财务确认折价金额合理 |
| 旧逻辑与新逻辑对比 | 每天（影子模式期间） | 差异为 0 后停止 |
| 性能基线建立 | 一次 | 记录正常状态下的性能指标 |

---

## 9. 待确认清单

以下问题需要在实现前与 HIS 团队确认：

| 编号 | 问题 | 影响 |
|---|---|---|
| 1 | HIS 的物价项目主表叫什么？主键字段是什么？ | ITEM_CODE 映射 |
| 2 | HIS 的收费入口代码在哪个模块/方法？ | 改造位置 |
| 3 | HIS 的收费明细表叫什么？关键字段？ | 对账数据源 |
| 4 | HIS 前端是 WinForms 还是 WPF？ | 弹窗实现方式 |
| 5 | HIS 是否已有 HTTP 客户端封装？ | 接口调用方式 |
| 6 | HIS 的数据库与计价服务是否同一个 Oracle 实例？ | 部署架构 |
| 7 | 是否有现有的任务调度平台？ | 对账任务实现 |
| 8 | 是否有现有的监控平台？ | 告警接入 |
| 9 | 日均非药品收费笔数大约多少？ | 容量评估 |
| 10 | 是否有多院区？ | 灰度粒度 |

---

## 10. 结论

HIS 集成的关键原则：

1. **渐进式切换** — 影子模式 → 按项目灰度 → 按渠道灰度 → 全量
2. **随时可回滚** — 灰度开关可以秒级切回旧逻辑
3. **特殊项目不降级** — 计价服务异常时不允许按普通价格收费
4. **每笔可追溯** — confirm/commit/cancel 三段式 + 完整日志
5. **每日可对账** — 凌晨自动核查，第二天财务能看到异常
