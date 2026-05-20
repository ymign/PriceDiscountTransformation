-- ============================================================
-- 统一计价规则中心 - 修正动作执行顺序（旧 HIS 兼容）
-- ============================================================
--
-- 使用场景：
--   1. 新库首次初始化时，02-init-dict-data.sql 已经写入正确顺序，本脚本可不执行；
--   2. 已经执行过旧版 02-init-dict-data.sql 的库，必须执行本脚本修正 PR_DICT；
--   3. 每次上线前可重复执行，本脚本按 DICT_TYPE + DICT_CODE 做 UPSERT，不会重复插入。
--
-- 关键业务口径：
--   旧 HIS 正式环境在 ucDisplay.GetFeeItemList 中先调用 ConvertRestrictingfee，
--   再调用 ConvertDiscountfee；ConvertDiscountfee 内部先按 DISCOUNT_RATE 比例折价，
--   再按 TOPPRICE 做最高限价封顶。
--
--   因此规则中心必须是：
--     数量限制/互斥  <  FORMULA_CALC  <  APPLY_MAX_AMOUNT/TOPPRICE
--
--   如果数据库里 ACTION_TYPE_ORDER 仍是旧顺序（FORMULA_CALC 在数量限制前），
--   即使 C# 默认顺序已经修正，运行时仍会被数据库旧字典覆盖，导致金额偏差。
-- ============================================================

DECLARE
  PROCEDURE upsert_action_order(
    p_code   IN VARCHAR2,
    p_name   IN VARCHAR2,
    p_sort   IN NUMBER,
    p_remark IN VARCHAR2
  ) IS
    v_count NUMBER;
  BEGIN
    SELECT COUNT(*)
      INTO v_count
      FROM PR_DICT
     WHERE DICT_TYPE = 'ACTION_TYPE_ORDER'
       AND DICT_CODE = p_code;

    IF v_count = 0 THEN
      INSERT INTO PR_DICT (
        DICT_ID, DICT_TYPE, DICT_CODE, DICT_NAME, SORT_NO, IS_ENABLED, REMARK
      ) VALUES (
        SEQ_PR_DICT.NEXTVAL, 'ACTION_TYPE_ORDER', p_code, p_name, p_sort, 'Y', p_remark
      );
    ELSE
      UPDATE PR_DICT
         SET DICT_NAME  = p_name,
             SORT_NO    = p_sort,
             IS_ENABLED = 'Y',
             REMARK     = p_remark
       WHERE DICT_TYPE = 'ACTION_TYPE_ORDER'
         AND DICT_CODE = p_code;
    END IF;
  END;
BEGIN
  upsert_action_order('CONVERT_QTY',              '双单位换算',       10,  '双单位换算，公式依赖换算后数量');
  upsert_action_order('APPLY_DAY_LIMIT_QTY',      '日数量限制',       20,  '日数量限制，先截断可收费数量');
  upsert_action_order('APPLY_TIME_WINDOW_LIMIT',  '时间窗口数量限制', 30,  '时间窗口数量限制（如旧 HIS 2 小时窗），先截断可收费数量');
  upsert_action_order('APPLY_ONCE_LIMIT_QTY',     '单次数量限制',     40,  '单次数量限制，先截断可收费数量');
  upsert_action_order('SAME_GROUP_MUTEX',         '同组互斥',         50,  '同组互斥，先决定当前项目是否还能收费');
  upsert_action_order('FORMULA_CALC',             '公式计算',         60,  '比例折价，使用前面限制后的 FinalQty');
  upsert_action_order('APPLY_MIN_AMOUNT',         '金额下限',         70,  '金额下限，公式之后才能比较；FIN_DISCOUNT_FEE 当前不生成该动作');
  upsert_action_order('APPLY_MAX_AMOUNT',         '金额上限',         80,  '金额上限/TOPPRICE，必须在比例折价后比较');
  upsert_action_order('SAME_OPERATION_CEILING',   '同手术封顶',       85,  '同手术封顶，金额类累计封顶必须在公式和单项封顶之后执行');
  upsert_action_order('ADD_CHILD_ITEM',           '子项加收',         90,  '子项加收');
  upsert_action_order('DISCOUNT_EXCEED_TO_ZERO',  '超出部分归零',     100, '超出部分归零兜底，必须最后执行');

  COMMIT;
  DBMS_OUTPUT.PUT_LINE('ACTION_TYPE_ORDER 已修正为旧 HIS 兼容顺序。');
END;
/

-- 执行后检查：action_order_check 应为 OK。
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
