-- ============================================================
-- 统一计价规则中心 - 建表验证
-- 执行完 01/02/03 后运行此脚本验证
-- ============================================================

-- 1. 检查表是否全部创建
SELECT table_name, num_rows
FROM user_tables
WHERE table_name LIKE 'PR_%'
ORDER BY table_name;

-- 预期 21 张表:
-- PR_CACHE_INVALIDATION_OUTBOX
-- PR_CACHE_VERSION
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
DESC PR_LIMIT_OCCUPY;

SELECT COLUMN_NAME
FROM USER_TAB_COLUMNS
WHERE TABLE_NAME = 'PR_LIMIT_OCCUPY'
  AND COLUMN_NAME IN ('CHARGE_DETAIL_NO', 'RESULT_GROUP_NO')
ORDER BY COLUMN_NAME;
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

-- 预期 19 个序列


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

-- 4.1 检查动作执行顺序是否对齐旧 HIS
-- 关键顺序：
--   APPLY_TIME_WINDOW_LIMIT 必须早于 FORMULA_CALC；
--   FORMULA_CALC 必须早于 APPLY_MAX_AMOUNT/TOPPRICE；
--   DISCOUNT_EXCEED_TO_ZERO 作为超限兜底放在最后。
SELECT dict_code, sort_no, remark
FROM PR_DICT
WHERE dict_type = 'ACTION_TYPE_ORDER'
ORDER BY sort_no;
-- 期望顺序:
-- CONVERT_QTY=10
-- APPLY_DAY_LIMIT_QTY=20
-- APPLY_TIME_WINDOW_LIMIT=30
-- APPLY_ONCE_LIMIT_QTY=40
-- SAME_GROUP_MUTEX=50
-- FORMULA_CALC=60
-- APPLY_MIN_AMOUNT=70
-- APPLY_MAX_AMOUNT=80
-- SAME_OPERATION_CEILING=85
-- ADD_CHILD_ITEM=90
-- DISCOUNT_EXCEED_TO_ZERO=100

SELECT CASE
         WHEN MAX(CASE WHEN dict_code = 'APPLY_TIME_WINDOW_LIMIT' THEN sort_no END)
              < MAX(CASE WHEN dict_code = 'FORMULA_CALC' THEN sort_no END)
          AND MAX(CASE WHEN dict_code = 'FORMULA_CALC' THEN sort_no END)
              < MAX(CASE WHEN dict_code = 'APPLY_MAX_AMOUNT' THEN sort_no END)
          AND MAX(CASE WHEN dict_code = 'APPLY_MAX_AMOUNT' THEN sort_no END)
              < MAX(CASE WHEN dict_code = 'DISCOUNT_EXCEED_TO_ZERO' THEN sort_no END)
         THEN 'OK'
         ELSE 'ERROR'
       END AS action_order_check
FROM PR_DICT
WHERE dict_type = 'ACTION_TYPE_ORDER'
  AND dict_code IN (
    'APPLY_TIME_WINDOW_LIMIT',
    'FORMULA_CALC',
    'APPLY_MAX_AMOUNT',
    'DISCOUNT_EXCEED_TO_ZERO'
  );
-- 期望: OK


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

-- 6.1 检查关键唯一索引是否存在
SELECT index_name, table_name, uniqueness
FROM user_indexes
WHERE index_name IN ('UK_PR_RV_RULE_PUBLISHED', 'UK_PR_RAP_PENDING')
ORDER BY index_name;
-- 期望:
-- UK_PR_RV_RULE_PUBLISHED
-- UK_PR_RAP_PENDING


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

-- 9. 验证 RestrictingfeeCP 共享额度组（期望 1 组 8 项）
SELECT GROUP_CODE, GROUP_NAME, GROUP_TYPE,
       (SELECT COUNT(*) FROM PR_ITEM_GROUP_DETAIL d WHERE d.GROUP_ID = g.GROUP_ID) AS item_count
FROM PR_ITEM_GROUP g
WHERE GROUP_CODE = 'CP01';
-- 期望: CP01=8项

-- 10. 验证 CP_ 规则数量（期望 8 条）
SELECT COUNT(*) AS cp_rule_count
FROM PR_RULE_HEADER
WHERE RULE_CODE LIKE 'CP_%'
  AND STATUS = 'PUBLISHED'
  AND IS_ENABLED = 'Y';
-- 期望: 8

-- 11. 验证 CP_ 规则均有 SAME_GROUP_MUTEX、DISCOUNT_EXCEED_TO_ZERO 和 7021 科室排除条件
SELECT h.RULE_CODE,
       COUNT(CASE WHEN a.ACTION_TYPE = 'SAME_GROUP_MUTEX'        THEN 1 END) AS has_same_group_mutex,
       COUNT(CASE WHEN a.ACTION_TYPE = 'DISCOUNT_EXCEED_TO_ZERO' THEN 1 END) AS has_exceed_zero,
       COUNT(CASE WHEN c.CONDITION_TYPE = 'CHARGE_DEPT_EXCLUDE'  THEN 1 END) AS has_dept_exclude
FROM PR_RULE_HEADER h
LEFT JOIN PR_RULE_ACTION    a ON a.RULE_ID = h.RULE_ID AND a.VERSION_NO = h.CURRENT_VERSION
LEFT JOIN PR_RULE_CONDITION c ON c.RULE_ID = h.RULE_ID AND c.VERSION_NO = h.CURRENT_VERSION
WHERE h.RULE_CODE LIKE 'CP_%'
GROUP BY h.RULE_CODE
ORDER BY h.RULE_CODE;
-- 期望每行: has_same_group_mutex=1, has_exceed_zero=1, has_dept_exclude=1

-- 12. 验证 RestrictingfeeZT 互斥组（期望 4 组）
SELECT GROUP_CODE, GROUP_NAME, GROUP_TYPE,
       (SELECT COUNT(*) FROM PR_ITEM_GROUP_DETAIL d WHERE d.GROUP_ID = g.GROUP_ID) AS item_count
FROM PR_ITEM_GROUP g
WHERE GROUP_CODE IN ('ZT01','ZT02','ZT03','ZT04')
ORDER BY GROUP_CODE;
-- 期望: ZT01=3项, ZT02=2项, ZT03=2项, ZT04=3项

-- 13. 验证 RestrictingfeeTX1 互斥组（期望 1 组 15 项）
SELECT GROUP_CODE, GROUP_NAME,
       (SELECT COUNT(*) FROM PR_ITEM_GROUP_DETAIL d WHERE d.GROUP_ID = g.GROUP_ID) AS item_count
FROM PR_ITEM_GROUP g
WHERE GROUP_CODE = 'TX1';
-- 期望: item_count=15

-- 14. 验证 CP/ZT/TX1 规则均有 SAME_GROUP_MUTEX 动作
SELECT COUNT(*) AS group_mutex_rule_count
FROM PR_RULE_HEADER h
WHERE (h.RULE_CODE LIKE 'CP_%' OR h.RULE_CODE LIKE 'ZT_%' OR h.RULE_CODE LIKE 'TX_%')
  AND STATUS = 'PUBLISHED'
  AND EXISTS (
    SELECT 1 FROM PR_RULE_ACTION a
    WHERE a.RULE_ID = h.RULE_ID
      AND a.ACTION_TYPE = 'SAME_GROUP_MUTEX'
  );
-- 期望: 33 (CP01:8 + ZT01:3 + ZT02:2 + ZT03:2 + ZT04:3 + TX1:15)

-- 15. 验证所有导入规则的 ROLLBACK_MODE 均为 LEGACY_EQUIVALENT
SELECT COUNT(*) AS non_legacy_count
FROM PR_RULE_HEADER
WHERE (RULE_CODE LIKE 'RF_%' OR RULE_CODE LIKE 'CP_%' OR RULE_CODE LIKE 'ZT_%' OR RULE_CODE LIKE 'TX_%')
  AND (ROLLBACK_MODE IS NULL OR ROLLBACK_MODE != 'LEGACY_EQUIVALENT');
-- 期望: 0（全部设为 LEGACY_EQUIVALENT）

-- 16. 验证 CP/ZT/TX1 每项的 EXCLUSIVE_GROUP 已设置
SELECT a.EXCLUSIVE_GROUP, COUNT(*) AS cnt
FROM PR_RULE_ACTION a
JOIN PR_RULE_HEADER h ON h.RULE_ID = a.RULE_ID
WHERE (h.RULE_CODE LIKE 'CP_%' OR h.RULE_CODE LIKE 'ZT_%' OR h.RULE_CODE LIKE 'TX_%')
  AND a.ACTION_TYPE = 'SAME_GROUP_MUTEX'
GROUP BY a.EXCLUSIVE_GROUP
ORDER BY a.EXCLUSIVE_GROUP;
-- 期望: CP01=8, ZT01=3, ZT02=2, ZT03=2, ZT04=3, TX1=15

-- 17. 验证 FIN_DISCOUNT_FEE 动态导入结果
-- 如果 04-import-rules.sql 执行环境可访问 FIN_DISCOUNT_FEE，DF_ 数量应等于
-- FIN_DISCOUNT_FEE 中 VALID_STATE='1' 且 DISCOUNT_TYPE='2' 的可导入记录数（排除异常/重复）。
-- 如果执行环境没有源表或源表无有效数据，本查询结果为 0 属于预期。
-- 本仓库 sql/FIN_DISCOUNT_FEE.sql 当前提供 56 条有效源数据；独立规则中心库需要先存在
-- FIN_DISCOUNT_FEE 表结构，再执行该源数据脚本，最后执行 04-import-rules.sql 动态导入。
DECLARE
  v_source_count NUMBER := 0;
BEGIN
  BEGIN
    EXECUTE IMMEDIATE
      'SELECT COUNT(*) FROM FIN_DISCOUNT_FEE WHERE VALID_STATE = ''1'' AND DISCOUNT_TYPE = ''2'''
      INTO v_source_count;
    DBMS_OUTPUT.PUT_LINE('FIN_DISCOUNT_FEE 有效源数据: ' || v_source_count || ' 条；本仓库源脚本期望 56 条');
  EXCEPTION
    WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE('FIN_DISCOUNT_FEE 源表不存在或不可访问，无法校验 56 条源数据。错误: ' || SQLERRM);
  END;
END;
/

SELECT COUNT(*) AS discount_fee_rule_count
FROM PR_RULE_HEADER
WHERE RULE_CODE LIKE 'DF_%'
  AND STATUS = 'PUBLISHED'
  AND IS_ENABLED = 'Y';

-- 18. 验证 DF_ 规则均有 FORMULA_CALC 动作，且不会从 FIN_DISCOUNT_FEE 生成 APPLY_MIN_AMOUNT
SELECT h.RULE_CODE,
       COUNT(CASE WHEN a.ACTION_TYPE = 'FORMULA_CALC'      THEN 1 END) AS has_formula_calc,
       COUNT(CASE WHEN a.ACTION_TYPE = 'APPLY_MAX_AMOUNT'  THEN 1 END) AS has_max_amount,
       COUNT(CASE WHEN a.ACTION_TYPE = 'APPLY_MIN_AMOUNT'  THEN 1 END) AS unexpected_min_amount
FROM PR_RULE_HEADER h
LEFT JOIN PR_RULE_ACTION a ON a.RULE_ID = h.RULE_ID AND a.VERSION_NO = h.CURRENT_VERSION
WHERE h.RULE_CODE LIKE 'DF_%'
GROUP BY h.RULE_CODE
ORDER BY h.RULE_CODE;
-- 期望每行: has_formula_calc=1, unexpected_min_amount=0；has_max_amount 取决于 TOPPRICE 是否大于 0
