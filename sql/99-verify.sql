-- ============================================================
-- 统一计价规则中心 - 建表验证
-- 执行完 01/02/03 后运行此脚本验证
-- ============================================================

-- 1. 检查表是否全部创建
SELECT table_name, num_rows
FROM user_tables
WHERE table_name LIKE 'PR_%'
ORDER BY table_name;

-- 预期 16 张表:
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
-- PR_RULE_CHANGE_LOG
-- PR_RULE_CONDITION
-- PR_RULE_HEADER
-- PR_RULE_PUBLISH
-- PR_RULE_VERSION


-- 2. 检查序列是否全部创建
SELECT sequence_name, last_number
FROM user_sequences
WHERE sequence_name LIKE 'SEQ_PR_%'
ORDER BY sequence_name;

-- 预期 15 个序列


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
-- ACTION_TYPE      11
-- BODY_PART         4
-- CHARGE_SCENE      5
-- CONDITION_TYPE   10
-- FORMULA_TYPE      6
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
