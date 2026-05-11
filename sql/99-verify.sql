-- ============================================================
-- 统一计价规则中心 - 建表验证
-- 执行完 01/02/03 后运行此脚本验证
-- ============================================================

-- 1. 检查表是否全部创建
SELECT table_name, num_rows
FROM user_tables
WHERE table_name LIKE 'PR_%'
ORDER BY table_name;

-- 预期 19 张表:
-- PR_CHARGE_DISCOUNT_DETAIL
-- PR_CHARGE_REQUEST_LOG
-- PR_CHARGE_REVERSE_LOG
-- PR_CHARGE_TRACE_STEP
-- PR_DICT
-- PR_FORMULA_DEF
-- PR_ITEM_GROUP
-- PR_ITEM_GROUP_DETAIL
-- PR_LIMIT_LOCK
-- PR_LIMIT_OCCUPY
-- PR_RULE_ACTION
-- PR_RULE_APPROVAL
-- PR_RULE_CHANGE_LOG
-- PR_RULE_CONDITION
-- PR_RULE_HEADER
-- PR_RULE_PUBLISH
-- PR_RULE_TEST_CASE
-- PR_RULE_TEST_RUN
-- PR_RULE_VERSION


-- 2. 检查序列是否全部创建
SELECT sequence_name, last_number
FROM user_sequences
WHERE sequence_name LIKE 'SEQ_PR_%'
ORDER BY sequence_name;

-- 预期 18 个序列


-- 3. 检查索引
SELECT index_name, table_name, uniqueness
FROM user_indexes
WHERE table_name LIKE 'PR_%'
ORDER BY table_name, index_name;


-- 4. 检查字典数据
SELECT dict_type, COUNT(*) AS cnt
FROM PR_DICT
GROUP BY dict_type
ORDER BY dict_type;

-- 预期:
-- ACTION_TYPE      12
-- ACTION_TYPE_ORDER 11
-- BODY_PART         4
-- CHARGE_SCENE      4
-- CONDITION_TYPE   10
-- FORMULA_TYPE      6
-- MUTUALLY_EXCLUSIVE_ACTION_TYPE 6
-- OPERATOR_TYPE     8
-- PRICING_UNIT      6
-- RULE_CATEGORY     4
-- RULE_SCOPE        3
-- SOURCE_SYSTEM     3


-- 5. 检查公式定义
SELECT formula_code, formula_name, executor_code
FROM PR_FORMULA_DEF
ORDER BY formula_id;

-- 预期 6 条公式


-- 6. 检查约束
SELECT constraint_name, table_name, constraint_type
FROM user_constraints
WHERE table_name LIKE 'PR_%'
  AND constraint_type IN ('P','U','C')
ORDER BY table_name, constraint_type, constraint_name;


-- ============================================================
-- 以下验证项在执行 04-import-rules.sql 后运行
-- ============================================================

-- 7. 验证 Restrictingfee 时间窗口限额规则（期望 26 条）
SELECT COUNT(*) AS restrictingfee_rule_count
FROM PR_RULE_HEADER
WHERE RULE_CODE LIKE 'RF_%'
  AND STATUS = 'PUBLISHED'
  AND IS_ENABLED = 'Y';
-- 期望: 26

-- 8. 验证每条 RF_ 规则都有 APPLY_TIME_WINDOW_LIMIT 动作
SELECT h.RULE_CODE,
       COUNT(CASE WHEN a.ACTION_TYPE = 'APPLY_TIME_WINDOW_LIMIT' THEN 1 END) AS has_window_limit,
       COUNT(CASE WHEN a.ACTION_TYPE = 'DISCOUNT_EXCEED_TO_ZERO'  THEN 1 END) AS has_exceed_zero,
       COUNT(CASE WHEN c.CONDITION_TYPE = 'CHARGE_DEPT_EXCLUDE'   THEN 1 END) AS has_dept_exclude
FROM PR_RULE_HEADER h
LEFT JOIN PR_RULE_ACTION    a ON a.RULE_ID = h.RULE_ID AND a.VERSION_NO = h.CURRENT_VERSION
LEFT JOIN PR_RULE_CONDITION c ON c.RULE_ID = h.RULE_ID AND c.VERSION_NO = h.CURRENT_VERSION
WHERE h.RULE_CODE LIKE 'RF_%'
GROUP BY h.RULE_CODE
ORDER BY h.RULE_CODE;
-- 期望每行: has_window_limit=1, has_exceed_zero=1, has_dept_exclude=1

-- 9. 验证 RestrictingfeeZT 互斥组（期望 4 组）
SELECT GROUP_CODE, GROUP_NAME, GROUP_TYPE,
       (SELECT COUNT(*) FROM PR_ITEM_GROUP_DETAIL d WHERE d.GROUP_ID = g.GROUP_ID) AS item_count
FROM PR_ITEM_GROUP g
WHERE GROUP_CODE IN ('ZT01','ZT02','ZT03','ZT04')
ORDER BY GROUP_CODE;
-- 期望: ZT01=3项, ZT02=2项, ZT03=2项, ZT04=3项

-- 10. 验证 RestrictingfeeTX1 互斥组（期望 1 组 15 项）
SELECT GROUP_CODE, GROUP_NAME,
       (SELECT COUNT(*) FROM PR_ITEM_GROUP_DETAIL d WHERE d.GROUP_ID = g.GROUP_ID) AS item_count
FROM PR_ITEM_GROUP g
WHERE GROUP_CODE = 'TX1';
-- 期望: item_count=15

-- 11. 验证 ZT/TX1 规则均有 SAME_GROUP_MUTEX 动作
SELECT COUNT(*) AS zt_tx_rule_count
FROM PR_RULE_HEADER h
WHERE (h.RULE_CODE LIKE 'ZT_%' OR h.RULE_CODE LIKE 'TX_%')
  AND STATUS = 'PUBLISHED'
  AND EXISTS (
    SELECT 1 FROM PR_RULE_ACTION a
    WHERE a.RULE_ID = h.RULE_ID
      AND a.ACTION_TYPE = 'SAME_GROUP_MUTEX'
  );
-- 期望: 25 (ZT01:3 + ZT02:2 + ZT03:2 + ZT04:3 + TX1:15)

-- 12. 验证所有导入规则的 ROLLBACK_MODE 均为 LEGACY_EQUIVALENT
SELECT COUNT(*) AS non_legacy_count
FROM PR_RULE_HEADER
WHERE (RULE_CODE LIKE 'RF_%' OR RULE_CODE LIKE 'ZT_%' OR RULE_CODE LIKE 'TX_%')
  AND (ROLLBACK_MODE IS NULL OR ROLLBACK_MODE != 'LEGACY_EQUIVALENT');
-- 期望: 0（全部设为 LEGACY_EQUIVALENT）

-- 13. 验证 ZT 每项的 EXCLUSIVE_GROUP 已设置
SELECT a.EXCLUSIVE_GROUP, COUNT(*) AS cnt
FROM PR_RULE_ACTION a
JOIN PR_RULE_HEADER h ON h.RULE_ID = a.RULE_ID
WHERE (h.RULE_CODE LIKE 'ZT_%' OR h.RULE_CODE LIKE 'TX_%')
  AND a.ACTION_TYPE = 'SAME_GROUP_MUTEX'
GROUP BY a.EXCLUSIVE_GROUP
ORDER BY a.EXCLUSIVE_GROUP;
-- 期望: ZT01=3, ZT02=2, ZT03=2, ZT04=3, TX1=15
