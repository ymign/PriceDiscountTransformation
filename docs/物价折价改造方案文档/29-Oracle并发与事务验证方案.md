# Oracle 并发与事务验证方案

## 1. 验证目标

本方案用于上线前在预发 Oracle 11g 环境验证规则中心的资金安全边界，不替代单元测试。

重点证明：

- confirm 同业务号并发只能成功产生一份有效结果。
- 同患者同项目时间窗并发不会突破数量限制。
- commit 与 expire 竞态不会造成请求日志、折价明细、限额占用状态不一致。
- reverse 同 ReverseNo 并发具备幂等保护。
- 发布与保存条件/动作竞态不会把审批后又修改的草稿发布上线。
- 审批并发通过/驳回不会留下多个待处理审批。
- 唯一索引冲突可以被服务层转换为业务冲突。
- 事务中途失败必须整体回滚。

## 2. 验证环境要求

- 使用预发 Oracle，不允许直接在生产执行压测或破坏性验证。
- 已执行 `sql/01-create-tables.sql`、字典初始化、公式初始化和必要测试规则初始化。
- 规则中心 API 指向同一个预发库。
- 至少准备两个并发客户端：
  - API 压测工具：Postman runner、JMeter、k6 或自写 PowerShell 并发脚本。
  - SQL 会话：SQLPlus、PL/SQL Developer、DataGrip 或 DBeaver，至少两个独立连接。
- 验证前记录当前时间窗口和测试患者、测试项目编码，避免污染真实业务数据。

## 3. 连接串配置

推荐使用预发专用账号，权限包含：

- 查询 `PR_%` 表和索引。
- 对测试环境执行 `SELECT ... FOR UPDATE`。
- 插入或清理测试数据时必须走已审批脚本，不在本方案脚本中直接提供生产写操作。

.NET 配置示例：

```json
{
  "ConnectionStrings": {
    "PricingRuleCenter": "User Id=PRICING_UAT;Password=***;Data Source=10.0.0.1:1521/HISUAT"
  }
}
```

SQLPlus 示例：

```sql
CONNECT PRICING_UAT/***@HISUAT
```

## 4. 基础结构验证

执行 `sql/90-concurrency-verify.sql`，确认：

- `UK_PR_CRL_BIZ` 存在，保护 `SOURCE_SYSTEM + BUSINESS_REQUEST_NO + CALL_TYPE`。
- `UK_PR_CRV_NO` 存在，保护 `ORIGINAL_REQUEST_ID + REVERSE_NO`。
- `UK_PR_RAP_PENDING` 存在，保护同规则版本同动作只有一个 pending 审批。
- `PR_LIMIT_LOCK` 主键存在，锁维度键唯一。
- `PR_LIMIT_OCCUPY` 有 `LIMIT_KEY`、`LIMIT_DIMENSION_CODE`、`BUSINESS_CHARGE_TIME`、`STATUS`、`EXPIRE_AT` 索引。

任何缺失必须先修表或补索引，再做并发验证。

## 5. 并发 confirm 同业务号

### 场景

两个线程同时请求：

- `sourceSystem = HIS`
- `businessRequestNo = UAT-CONFIRM-001`
- `callType = CONFIRM`
- 请求体完全相同

### 预期

- 两个请求最多只有一个创建新 `PR_CHARGE_REQUEST_LOG`。
- 另一个请求返回同一业务号的幂等结果，或在唯一索引冲突后重新读取已有结果。
- `PR_CHARGE_REQUEST_LOG` 中同业务键只有一条记录。
- `PR_LIMIT_OCCUPY` 中同 `REQUEST_ID` 的有效占用不重复。

### 验证 SQL

```sql
SELECT SOURCE_SYSTEM, BUSINESS_REQUEST_NO, CALL_TYPE, COUNT(*) AS CNT
FROM PR_CHARGE_REQUEST_LOG
WHERE BUSINESS_REQUEST_NO = 'UAT-CONFIRM-001'
GROUP BY SOURCE_SYSTEM, BUSINESS_REQUEST_NO, CALL_TYPE;

SELECT REQUEST_ID, COUNT(*) AS OCCUPY_CNT
FROM PR_LIMIT_OCCUPY
WHERE REQUEST_ID IN (
  SELECT REQUEST_ID
  FROM PR_CHARGE_REQUEST_LOG
  WHERE BUSINESS_REQUEST_NO = 'UAT-CONFIRM-001'
)
GROUP BY REQUEST_ID;
```

## 6. 并发 confirm 同患者同项目时间窗

### 场景

准备一条时间窗限额规则：

- 同患者、同项目、2 小时窗口。
- 窗口限额为 1。

并发提交两个不同 `businessRequestNo`，同患者、同项目、同 `businessChargeTime`。

### 预期

- `PR_LIMIT_LOCK` 中对应时间窗锁键被串行锁定。
- 两个 confirm 不能同时占用超过限额。
- 第二笔应按规则截断数量或超出部分 0 元，而不是突破限额。

### 双 SQL 会话锁验证

会话 A：

```sql
SELECT *
FROM PR_LIMIT_LOCK
WHERE LOCK_KEY = 'TW:P001:ITEM001:2026060610'
FOR UPDATE;
```

会话 B 在会话 A 未提交前执行同样 SQL，应阻塞或等待超时。会话 A 提交后，会话 B 才能继续。

## 7. commit 与 expire 竞态

### 场景

制造一条即将过期的 `CONFIRM_PENDING` 请求，同时触发：

- HIS 调用 commit。
- 后台 expire cleanup 扫描。

### 预期

- 最终状态只能是 `CONFIRMED/COMMITTED` 或 `EXPIRED` 之一。
- 请求日志、折价明细、限额占用状态必须一致。
- 若 commit 先锁定并成功，expire 不得再释放占用。
- 若 expire 先成功，commit 应返回状态不允许。

### 验证 SQL

```sql
SELECT REQUEST_ID, BUSINESS_STATUS, RESPONSE_AT
FROM PR_CHARGE_REQUEST_LOG
WHERE BUSINESS_REQUEST_NO = 'UAT-COMMIT-EXPIRE-001';

SELECT STATUS, COUNT(*) AS CNT
FROM PR_CHARGE_DISCOUNT_DETAIL
WHERE REQUEST_ID = (
  SELECT REQUEST_ID
  FROM PR_CHARGE_REQUEST_LOG
  WHERE BUSINESS_REQUEST_NO = 'UAT-COMMIT-EXPIRE-001'
)
GROUP BY STATUS;

SELECT STATUS, COUNT(*) AS CNT
FROM PR_LIMIT_OCCUPY
WHERE REQUEST_ID = (
  SELECT REQUEST_ID
  FROM PR_CHARGE_REQUEST_LOG
  WHERE BUSINESS_REQUEST_NO = 'UAT-COMMIT-EXPIRE-001'
)
GROUP BY STATUS;
```

## 8. reverse 并发同 ReverseNo

### 场景

对同一已确认收费请求并发提交两次 reverse：

- `reverseNo = UAT-REV-001`
- 原请求相同。
- 退费数量/金额相同。

### 预期

- `PR_CHARGE_REVERSE_LOG` 只有一条同 `ORIGINAL_REQUEST_ID + REVERSE_NO` 记录。
- 第二次返回幂等结果或唯一索引冲突被转换为业务幂等。
- 负数 `PR_LIMIT_OCCUPY` 不重复插入。

### 验证 SQL

```sql
SELECT ORIGINAL_REQUEST_ID, REVERSE_NO, COUNT(*) AS CNT
FROM PR_CHARGE_REVERSE_LOG
WHERE REVERSE_NO = 'UAT-REV-001'
GROUP BY ORIGINAL_REQUEST_ID, REVERSE_NO;

SELECT ORIGINAL_OCCUPY_ID, COUNT(*) AS CNT
FROM PR_LIMIT_OCCUPY
WHERE REQUEST_ID IN (
  SELECT REQUEST_ID
  FROM PR_CHARGE_REQUEST_LOG
  WHERE BUSINESS_REQUEST_NO = 'UAT-REVERSE-001'
)
AND OCCUPY_TYPE = 'REVERSE'
GROUP BY ORIGINAL_OCCUPY_ID;
```

## 9. 发布与保存条件/动作竞态

### 场景

对同一规则版本：

- 审批通过。
- 发布线程准备发布。
- 另一个线程保存条件或动作。

### 预期

- 如果保存发生在审批之后，发布必须失败，错误码为审批过期或待审核编辑不允许。
- `PR_RULE_CHANGE_LOG` 中保存动作的 `CHANGED_AT` 晚于审批时间时，不允许发布。
- 发布事务内必须锁定规则主档和目标版本。

### 验证 SQL

```sql
SELECT RULE_ID, VERSION_NO, CHANGE_TYPE, CHANGED_AT
FROM PR_RULE_CHANGE_LOG
WHERE RULE_ID = :rule_id
ORDER BY CHANGED_AT DESC;

SELECT RULE_ID, VERSION_NO, ACTION_TYPE, APPROVAL_STATUS, REVIEWED_AT
FROM PR_RULE_APPROVAL
WHERE RULE_ID = :rule_id
ORDER BY APPROVAL_ID DESC;
```

## 10. 审批并发通过/驳回

### 场景

两个审批人同时对同一 pending 审批提交：

- A 提交通过。
- B 提交驳回。

### 预期

- 最终只能有一个状态更新成功。
- `UK_PR_RAP_PENDING` 保证同规则版本同动作只有一个 pending。
- 服务层应基于当前状态 compare-and-set，避免后写覆盖先写。

### 验证 SQL

```sql
SELECT RULE_ID, VERSION_NO, ACTION_TYPE, APPROVAL_STATUS, COUNT(*) AS CNT
FROM PR_RULE_APPROVAL
WHERE RULE_ID = :rule_id
GROUP BY RULE_ID, VERSION_NO, ACTION_TYPE, APPROVAL_STATUS;
```

## 11. 唯一索引冲突转换

必须至少覆盖：

- `UK_PR_CRL_BIZ`：confirm 幂等业务键重复。
- `UK_PR_CRV_NO`：reverse 幂等业务键重复。
- `UK_PR_RAP_PENDING`：重复提交审批。
- `UK_PR_RV_RULE_VER`：重复创建规则版本。

预期：

- 不向调用方暴露 Oracle 原始异常文本。
- 转换为 `BizException` 或统一 API 错误结构。
- 日志保留 Oracle 错误号和业务键，便于排查。

## 12. 事务中途失败回滚

### 场景

通过测试桩或预发调试开关制造事务中途失败：

- confirm 写入请求日志后，写限额占用前失败。
- publish 更新版本状态后，写发布流水前失败。
- reverse 写冲正日志后，写负数占额前失败。

### 预期

- 事务回滚后看不到半成品状态。
- 请求日志、折价明细、限额占用、发布流水、变更日志状态一致。

### 验证 SQL

```sql
SELECT *
FROM PR_CHARGE_REQUEST_LOG
WHERE BUSINESS_REQUEST_NO = 'UAT-TX-ROLLBACK-001';

SELECT *
FROM PR_LIMIT_OCCUPY
WHERE REQUEST_ID IN (
  SELECT REQUEST_ID
  FROM PR_CHARGE_REQUEST_LOG
  WHERE BUSINESS_REQUEST_NO = 'UAT-TX-ROLLBACK-001'
);
```

## 13. 验收标准

- `sql/90-concurrency-verify.sql` 基础结构检查全部通过。
- 每个并发场景都有 API 请求记录、SQL 查询结果和日志截图。
- 没有重复业务键、重复 reverseNo、重复 pending 审批。
- 没有请求日志与限额占用状态不一致记录。
- 并发锁验证能证明 `SELECT ... FOR UPDATE` 生效。
- 所有异常均返回业务错误码，不暴露 Oracle 栈给渠道。
