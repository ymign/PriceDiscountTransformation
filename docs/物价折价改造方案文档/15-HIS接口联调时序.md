# HIS 接口联调时序

## 1. 文档目的

本文描述 HIS 系统与统一计价规则中心的接口联调时序，覆盖门诊收费、退费、超时重试、服务不可用降级等核心场景，并提供请求/响应示例和错误码对照表。

技术前提：
- HIS：C# Windows Forms，通过 HTTP 调用计价服务
- 计价服务：ASP.NET Core Web API
- 数据库：Oracle 11g

---

## 2. 联调总体流程

HIS 收费流程与计价中心的交互遵循以下阶段：

```
HIS 收费录入
    |
    v
[1] GET /api/pricing/items/{itemCode}/special-flag
    |  检查是否为特殊计价项目
    |
    v
[2] 非特殊项目 -> HIS 普通计价流程（不走计价中心）
    特殊项目 -> 继续
    |
    v
[3] POST /api/pricing/calculate/simulate
    |  试算，返回折价金额供弹窗展示
    |
    v
[4] 用户在弹窗确认收费金额
    |
    v
[5] POST /api/pricing/calculate/confirm
    |  确认计价，占用额度
    |
    v
[6] HIS 执行本地落账（写 HIS 数据库）
    |
    v
[7] 落账成功 -> POST /api/pricing/calculate/commit
    落账失败 -> POST /api/pricing/calculate/cancel
```

---

## 3. 场景 1：门诊收费 - 普通项目

普通项目不命中特殊规则，HIS 不调用计价中心，走原有计价逻辑。

**时序：**

```
HIS 收费界面 -> 用户录入项目 -> HIS 本地计价 -> 落账
```

**说明：**

HIS 在录入项目时调用 special-flag 接口判断是否为特殊项目。返回 `isSpecial = false` 时，HIS 走原有计价流程，不调用 simulate/confirm/commit。

---

## 4. 场景 2：门诊收费 - 特殊项目（命中规则，弹窗确认）

**时序：**

```
HIS 收费界面
  |
  |-- [1] GET /api/pricing/items/{itemCode}/special-flag
  |        返回: { "isSpecial": true }
  |
  |-- [2] HIS 弹出特殊计价确认窗口
  |
  |-- [3] POST /api/pricing/calculate/simulate
  |        计价中心：匹配规则 -> 执行公式 -> 应用限制 -> 返回试算结果
  |        返回: { "finalAmount": 400.00, "traceSteps": [...] }
  |
  |-- [4] 弹窗展示：项目名称、原价、折后价、计算过程
  |        用户点击"确认收费"
  |
  |-- [5] POST /api/pricing/calculate/confirm
  |        计价中心：幂等校验 -> 写全量待确认折价明细 -> 特殊项目占用额度 -> 返回确认结果
  |        返回: { "requestId": 101, "finalAmount": 400.00 }
  |
  |-- [6] HIS 执行本地落账
  |        成功
  |
  |-- [7] POST /api/pricing/calculate/commit
  |        计价中心：按 HIS 真实落账明细对账 -> 更新状态为已结算
  |        返回: { "success": true }
```

**请求/响应示例：**

**[1] special-flag 请求**

```
GET /api/pricing/items/SKIN001/special-flag
```

**[1] special-flag 响应**

```json
{
  "code": 0,
  "message": "success",
  "data": {
    "itemCode": "SKIN001",
    "isSpecial": true,
    "ruleCount": 2
  }
}
```

**[3] simulate 请求**

```json
{
  "sourceSystem": "HIS",
  "patientId": "P00001",
  "visitId": "V00001",
  "encounterNo": "E00001",
  "chargeScene": "OUTPATIENT",
  "itemCode": "SKIN001",
  "itemName": "浅表肿物去除费",
  "inputQty": 3,
  "inputUnit": "个",
  "unitPrice": 200.00,
  "chargeTime": "2026-05-10T10:30:00",
  "bodyPartCode": null,
  "operationNo": null,
  "pregnancyNo": null,
  "mainChargeDetailNo": null,
  "pricingParts": null,
  "extraParams": {}
}
```

**[3] simulate 响应**

```json
{
  "code": 0,
  "message": "success",
  "data": {
    "itemCode": "SKIN001",
    "inputQty": 3,
    "convertedQty": 3,
    "unitPrice": 200.00,
    "formulaAmount": 400.00,
    "finalAmount": 400.00,
    "discountType": "FORMULA",
    "formulaType": "INCREMENT_PERCENT",
    "formulaDetail": "200 x (3 x 0.5 + 0.5) = 400.00",
    "limitCheckResult": null,
    "traceSteps": [
      {
        "stepNo": 1,
        "stepType": "RULE_MATCH",
        "description": "匹配规则: 浅表肿物去除费-递增比例",
        "input": "itemCode=SKIN001, chargeScene=OUTPATIENT",
        "output": "ruleVersionId=RV001"
      },
      {
        "stepNo": 2,
        "stepType": "FORMULA_EXEC",
        "description": "执行公式: INCREMENT_PERCENT(rate=0.5)",
        "input": "unitPrice=200, qty=3, rate=0.5",
        "output": "formulaAmount=400.00"
      }
    ]
  }
}
```

**[5] confirm 请求**

```json
{
  "sourceSystem": "HIS",
  "businessRequestNo": "CHG20260510001-SKIN001",
  "callType": "CONFIRM",
  "patientId": "P00001",
  "visitId": "V00001",
  "encounterNo": "E00001",
  "chargeScene": "OUTPATIENT",
  "chargeNo": "CHG20260510001",
  "chargeDetailNo": "D001",
  "itemCode": "SKIN001",
  "itemName": "浅表肿物去除费",
  "inputQty": 3,
  "inputUnit": "个",
  "unitPrice": 200.00,
  "chargeTime": "2026-05-10T10:30:00",
  "operatorId": "DOC001",
  "bodyPartCode": null,
  "operationNo": null,
  "pregnancyNo": null,
  "mainChargeDetailNo": null,
  "pricingParts": null,
  "extraParams": {}
}
```

**[5] confirm 响应**

```json
{
  "code": 0,
  "message": "success",
  "data": {
    "requestId": 101,
    "finalAmount": 400.00,
    "items": [
      {
        "chargeDetailNo": "D001",
        "itemCode": "SKIN001",
        "finalQty": 3,
        "finalAmount": 400.00
      }
    ]
  }
}
```

**[7] commit 请求**

```json
{
  "requestId": 101,
  "chargeNo": "CHG20260510001",
  "actualTotalAmount": 400.00,
  "actualItems": [
    {
      "chargeDetailNo": "D001",
      "itemCode": "SKIN001",
      "partSeq": null,
      "finalQty": 3,
      "finalAmount": 400.00
    }
  ]
}
```

**[7] commit 响应**

```json
{
  "code": 0,
  "message": "success"
}
```

**commit 对账约束：**

- `actualItems` 必须来自 HIS 已经写库成功的真实收费明细，不能用试算或 confirm 响应临时拼凑。
- 普通项目和主项目按 `chargeDetailNo + itemCode + partSeq` 严格匹配。
- 替换子项、加收子项可以由 HIS 生成新的 `chargeDetailNo`，但 `itemCode`、`partSeq`、`finalQty`、`finalAmount` 必须与 confirm 结果一致。
- 同一次 confirm 同时包含普通明细和特殊明细时，HIS commit 必须回传全部有效落账明细，不能只回传特殊项目。

---

## 5. 场景 3：门诊收费 - 特殊项目确认后 HIS 落账失败

**时序：**

```
HIS 收费界面
  |
  |-- [1] special-flag -> isSpecial = true
  |-- [2] simulate -> finalAmount = 400.00
  |-- [3] confirm -> 成功，额度已占用
  |-- [4] HIS 执行本地落账
  |        失败（数据库异常、余额不足等）
  |
  |-- [5] POST /api/pricing/calculate/cancel
  |        计价中心：释放额度 -> 更新状态
  |        返回: { "success": true }
  |
  |-- [6] HIS 提示用户落账失败，收费未完成
```

**[5] cancel 请求**

```json
{
  "requestId": 101
}
```

**[5] cancel 响应**

```json
{
  "code": 0,
  "message": "success"
}
```

---

## 6. 场景 4：退费

**时序：**

```
HIS 退费界面
  |
  |-- [1] 用户选择退费项目
  |-- [2] POST /api/pricing/calculate/reverse
  |        计价中心：校验退费合法性 -> 释放额度 -> 写退费日志
  |        返回: { "success": true, "releasedQty": 3 }
  |
  |-- [3] HIS 执行本地退费落账
```

**[2] reverse 请求**

```json
{
  "sourceSystem": "HIS",
  "businessRequestNo": "CHG20260510001-SKIN001-REV001",
  "callType": "REVERSE",
  "originalPricingResultNo": "PR20260510001",
  "reverseQty": 3,
  "reverseTime": "2026-05-10T14:00:00",
  "reverseReason": "患者要求退费",
  "operatorId": "CASH001"
}
```

**[2] reverse 响应**

```json
{
  "code": 0,
  "message": "success",
  "data": {
    "originalPricingResultNo": "PR20260510001",
    "reverseLogId": "RL001",
    "releasedQty": 3,
    "releasedAmount": 400.00,
    "dailyLimitRemaining": 10,
    "timeWindowLimitRemaining": 3
  }
}
```

---

## 7. 场景 5：超时重试

**时序：**

```
HIS
  |
  |-- [1] POST /api/pricing/calculate/confirm
  |        请求发出...等待...
  |        超时（30秒无响应）
  |
  |-- [2] HIS 使用相同的 businessRequestNo 重试
  |        POST /api/pricing/calculate/confirm
  |        （请求体完全一致）
  |
  |-- [3] 计价中心：检测到相同 businessRequestNo + 相同 fingerprint
  |        返回幂等结果（不重复占额）
  |        返回: { "requestId": 101, "finalAmount": 400.00 }
```

**关键约束：**

- HIS 重试时必须使用完全相同的 `businessRequestNo`
- 如果 HIS 尚未生成稳定收费单号或收费确认流水，不能用时间戳/GUID 临时生成 `businessRequestNo`；应先预生成稳定业务号，否则 confirm 必须阻断。
- HIS 重试时请求体参数不得变更（否则 fingerprint 不匹配，计价中心拒绝）
- 计价中心检测到幂等重复时，返回 HTTP 200 + 原始结果，不是 409

**[3] 幂等响应（与首次成功响应一致）**

```json
{
  "code": 0,
  "message": "success",
  "data": {
    "requestId": 101,
    "finalAmount": 400.00,
    "items": [
      {
        "chargeDetailNo": "D001",
        "itemCode": "SKIN001",
        "finalQty": 3,
        "finalAmount": 400.00
      }
    ]
  }
}
```

---

## 8. 场景 6：计价服务不可用

### 8.1 rollbackMode = STOP_CHARGE

**时序：**

```
HIS 收费界面
  |
  |-- [1] GET /api/pricing/items/{itemCode}/special-flag
  |        超时或连接失败
  |
  |-- [2] HIS 检查该项目的 rollbackMode 配置
  |        rollbackMode = STOP_CHARGE
  |
  |-- [3] HIS 禁止收费，提示："计价服务不可用，该项目暂时无法收费"
```

**说明：**

rollbackMode 配置在 HIS 本地缓存（启动时从计价中心同步或本地配置文件读取）。当计价服务不可用时，HIS 读取本地缓存的 rollbackMode 决定行为。

### 8.2 rollbackMode = LEGACY_EQUIVALENT

**时序：**

```
HIS 收费界面
  |
  |-- [1] GET /api/pricing/items/{itemCode}/special-flag
  |        超时或连接失败
  |
  |-- [2] HIS 检查该项目的 rollbackMode 配置
  |        rollbackMode = LEGACY_EQUIVALENT
  |
  |-- [3] HIS 回退为旧计价逻辑（代码硬编码的折价计算）
  |        标记本次收费为"降级计价"
  |
  |-- [4] 后续对账时，降级计价记录需人工复核
```

**说明：**

`LEGACY_EQUIVALENT` 仅允许在已确认的特殊项目上使用，且必须经过审批。降级计价的结果需在每日对账中标记，由收费处复核。

---

## 9. 错误码对照表

| 错误码 | HTTP 状态码 | 含义 | HIS 处理建议 |
|--------|------------|------|-------------|
| `SUCCESS` | 200 | 成功 | 正常处理 |
| `IDEMPOTENT_RETURN` | 200 | 幂等返回（重复请求） | 按正常成功处理 |
| `RULE_NOT_FOUND` | 200 | 未匹配到规则 | 提示"该项目无特殊计价规则"，可走普通计价 |
| `PRICE_MISMATCH` | 400 | 单价不一致 | 提示"单价与计价中心不一致，请刷新物价" |
| `INVALID_PARAMS` | 400 | 参数校验失败 | 检查请求参数后重试 |
| `FINGERPRINT_MISMATCH` | 409 | 指纹不匹配（参数变更） | 不得重试，检查请求参数是否变更 |
| `LIMIT_EXCEEDED` | 200 | 超出限额（部分归零） | 正常展示，超出部分金额为 0 |
| `RULE_CONFLICT` | 400 | 规则配置冲突 | 联系物价管理员检查规则配置 |
| `SYSTEM_ERROR` | 500 | 计价中心内部错误 | 重试或按 rollbackMode 降级 |
| `SERVICE_UNAVAILABLE` | 503 | 计价中心不可用 | 按 rollbackMode 降级 |
| `CONFIRM_TIMEOUT` | 408 | 确认超时 | 使用相同 businessRequestNo 重试 |
| `ALREADY_COMMITTED` | 200 | 已结算（commit 重复调用） | 按正常成功处理 |
| `ALREADY_CANCELLED` | 200 | 已取消（cancel 重复调用） | 按正常成功处理 |
| `REVERSE_EXCEED` | 400 | 退费数量超过有效收费 | 提示"退费数量超过有效收费，请检查" |
| `REVERSE_NOT_FOUND` | 404 | 原收费记录不存在 | 检查 originalRequestId 是否正确 |

---

## 10. 联调检查清单

### 10.1 接口连通性

- [ ] HIS 能正常调用 special-flag 接口
- [ ] HIS 能正常调用 simulate 接口
- [ ] HIS 能正常调用 confirm 接口
- [ ] HIS 能正常调用 commit 接口
- [ ] HIS 能正常调用 cancel 接口
- [ ] HIS 能正常调用 reverse 接口
- [ ] 超时配置正确（建议 30 秒）

### 10.2 数据一致性

- [ ] HIS 传入的 itemCode 与计价中心规则配置一致
- [ ] HIS 传入的 unitPrice 与权威物价表一致
- [ ] HIS 传入的 chargeTime 格式正确（ISO 8601）
- [ ] HIS 传入的 businessRequestNo 在收费动作内唯一
- [ ] confirm 返回的 finalAmount 与 HIS 落账金额一致
- [ ] commit 回传的 actualItems 覆盖本次 confirm 的全部有效落账明细
- [ ] 替换子项、加收子项即使使用 HIS 新明细号，也能按项目、部位序号、数量和金额完成对账

### 10.3 异常处理

- [ ] 计价服务超时时，HIS 按 rollbackMode 正确降级
- [ ] confirm 重试使用相同的 businessRequestNo
- [ ] cancel 在 HIS 落账失败时正确调用
- [ ] reverse 退费数量不超过有效收费
- [ ] 错误码映射到用户友好的提示信息

### 10.4 性能验证

- [ ] simulate 响应时间 < 200ms（P95）
- [ ] confirm 响应时间 < 500ms（P95）
- [ ] 高峰期并发场景下无超时
- [ ] 计价服务重启后 HIS 能自动恢复连接
