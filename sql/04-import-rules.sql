-- ============================================================
-- 04-import-rules.sql
-- 从 com_dictionary 迁移规则到规则中心
-- 生成日期: 2026-05-11
-- 数据来源: 字典规则.sql
--
-- 包含规则：
--   1. Restrictingfee  (26条) — 2小时时间窗口限额，超限归零，排除7021科室
--   2. RestrictingfeeZT (4组) — 同次止血手术互斥组规则
--   3. RestrictingfeeTX1(1组) — 胎心监护15项互斥组规则
--   4. FIN_DISCOUNT_FEE        — B类比例折价规则【待数据】
--
-- 注意：
--   - RestrictingfeeCP 8项与 Restrictingfee 完全重叠，规则已涵盖，不重复生成
--   - Astrictpackagefee 4项为免除项，无需规则
--   - 方案B兜底字段 LegacyOccupiedQty 已在代码层注入，本 SQL 无需处理
-- ============================================================

-- ============================================================
-- 第一部分：Restrictingfee 时间窗口限额规则（26 条）
-- 规则结构：ITEM_MATCH + CHARGE_DEPT_EXCLUDE → APPLY_TIME_WINDOW_LIMIT + DISCOUNT_EXCEED_TO_ZERO
-- ============================================================
DECLARE
  TYPE t_rec IS RECORD (item_code VARCHAR2(50), limit_qty NUMBER);
  TYPE t_tab IS TABLE OF t_rec INDEX BY PLS_INTEGER;
  v_items   t_tab;
  v_rule_id NUMBER;
  v_ver_id  NUMBER;
  v_cond_id NUMBER;
  v_act_id  NUMBER;
  v_params  VARCHAR2(200);
BEGIN
  -- 26 条活跃 Restrictingfee 记录（已排除 VALID_STATE='0' 的 F00000010813）
  v_items( 1).item_code := 'F00000078543'; v_items( 1).limit_qty :=  1;
  v_items( 2).item_code := 'F00000205647'; v_items( 2).limit_qty :=  1;
  v_items( 3).item_code := 'F00000205661'; v_items( 3).limit_qty :=  3;
  v_items( 4).item_code := 'F00000205662'; v_items( 4).limit_qty :=  1;
  v_items( 5).item_code := 'F00000205663'; v_items( 5).limit_qty :=  1;
  v_items( 6).item_code := 'F00000205667'; v_items( 6).limit_qty :=  3;
  v_items( 7).item_code := 'F00000205668'; v_items( 7).limit_qty :=  1;
  v_items( 8).item_code := 'F00000205669'; v_items( 8).limit_qty :=  1;
  v_items( 9).item_code := 'F00000205672'; v_items( 9).limit_qty :=  2;
  v_items(10).item_code := 'F00000205673'; v_items(10).limit_qty :=  1;
  v_items(11).item_code := 'F00000205678'; v_items(11).limit_qty :=  3;
  v_items(12).item_code := 'F00000205683'; v_items(12).limit_qty :=  3;
  v_items(13).item_code := 'F00000205688'; v_items(13).limit_qty :=  2;
  v_items(14).item_code := 'F00000205692'; v_items(14).limit_qty :=  2;
  v_items(15).item_code := 'F00000209343'; v_items(15).limit_qty :=  1;
  v_items(16).item_code := 'F00000209348'; v_items(16).limit_qty :=  1;
  v_items(17).item_code := 'F00000209351'; v_items(17).limit_qty :=  1;
  v_items(18).item_code := 'F00000209355'; v_items(18).limit_qty := 99;
  v_items(19).item_code := 'F00000209356'; v_items(19).limit_qty :=  1;
  v_items(20).item_code := 'F00000209361'; v_items(20).limit_qty :=  1;
  v_items(21).item_code := 'F00000209365'; v_items(21).limit_qty :=  5;
  v_items(22).item_code := 'F00000209366'; v_items(22).limit_qty :=  1;
  v_items(23).item_code := 'F00000209369'; v_items(23).limit_qty :=  1;
  v_items(24).item_code := 'F00000209371'; v_items(24).limit_qty := 99;
  v_items(25).item_code := 'F00000209372'; v_items(25).limit_qty :=  1;
  v_items(26).item_code := 'F00000209376'; v_items(26).limit_qty := 99;

  FOR i IN 1..v_items.COUNT LOOP
    v_params := '{"WindowHours":2,"LimitQty":' || v_items(i).limit_qty || '}';

    -- 规则头
    v_rule_id := SEQ_PR_RULE_HEADER.NEXTVAL;
    INSERT INTO PR_RULE_HEADER (
      RULE_ID, RULE_CODE, RULE_NAME, RULE_CATEGORY, RULE_SCOPE,
      ITEM_CODE, PRIORITY, CURRENT_VERSION, STATUS, IS_ENABLED,
      EFFECTIVE_FROM, ROLLBACK_MODE, CREATED_BY, REMARK
    ) VALUES (
      v_rule_id,
      'RF_' || v_items(i).item_code,
      '2小时限额-' || v_items(i).item_code,
      'LIMIT', 'ITEM',
      v_items(i).item_code,
      100, 1, 'PUBLISHED', 'Y',
      TRUNC(SYSDATE), 'LEGACY_EQUIVALENT',
      'IMPORT', '从 Restrictingfee 字典迁移'
    );

    -- 版本
    v_ver_id := SEQ_PR_RULE_VERSION.NEXTVAL;
    INSERT INTO PR_RULE_VERSION (
      VERSION_ID, RULE_ID, VERSION_NO, VERSION_STATUS,
      EFFECTIVE_FROM, PUBLISHED_BY, PUBLISHED_AT, PUBLISH_REMARK
    ) VALUES (
      v_ver_id, v_rule_id, 1, 'PUBLISHED',
      TRUNC(SYSDATE), 'IMPORT', SYSDATE, '初始迁移'
    );

    -- 条件1：项目匹配
    v_cond_id := SEQ_PR_RULE_CONDITION.NEXTVAL;
    INSERT INTO PR_RULE_CONDITION (
      CONDITION_ID, RULE_ID, VERSION_NO, CONDITION_GROUP,
      CONDITION_TYPE, OPERATOR_TYPE, LEFT_KEY, RIGHT_VALUE, SORT_NO, IS_ENABLED
    ) VALUES (
      v_cond_id, v_rule_id, 1, 'DEFAULT',
      'ITEM_MATCH', 'EQ', 'ITEM_CODE', v_items(i).item_code, 1, 'Y'
    );

    -- 条件2：排除挂号部7021
    v_cond_id := SEQ_PR_RULE_CONDITION.NEXTVAL;
    INSERT INTO PR_RULE_CONDITION (
      CONDITION_ID, RULE_ID, VERSION_NO, CONDITION_GROUP,
      CONDITION_TYPE, OPERATOR_TYPE, LEFT_KEY, RIGHT_VALUE, SORT_NO, IS_ENABLED
    ) VALUES (
      v_cond_id, v_rule_id, 1, 'DEFAULT',
      'CHARGE_DEPT_EXCLUDE', 'EQ', 'CHARGE_DEPT_CODE', '7021', 2, 'Y'
    );

    -- 动作1：时间窗口限额
    v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
    INSERT INTO PR_RULE_ACTION (
      ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
      PARAMS_JSON, SORT_NO, ON_ERROR, IS_ENABLED
    ) VALUES (
      v_act_id, v_rule_id, 1,
      'APPLY_TIME_WINDOW_LIMIT', 'TimeWindowLimitExecutor',
      v_params, 10, 'STOP', 'Y'
    );

    -- 动作2：超限归零
    v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
    INSERT INTO PR_RULE_ACTION (
      ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
      PARAMS_JSON, SORT_NO, ON_ERROR, IS_ENABLED
    ) VALUES (
      v_act_id, v_rule_id, 1,
      'DISCOUNT_EXCEED_TO_ZERO', 'ExceedToZeroExecutor',
      '{"ExceedAction":"ZERO"}', 20, 'STOP', 'Y'
    );
  END LOOP;

  COMMIT;
  DBMS_OUTPUT.PUT_LINE('Restrictingfee: ' || v_items.COUNT || ' 条规则已导入');
END;
/


-- ============================================================
-- 第二部分：RestrictingfeeZT 止血互斥组（4组10项）
-- MARK字段标记组号：1→ZT01，2→ZT02，3→ZT03，4→ZT04
-- 规则结构：ITEM_MATCH → SAME_GROUP_MUTEX + DISCOUNT_EXCEED_TO_ZERO
-- ============================================================
DECLARE
  TYPE t_item_rec IS RECORD (item_code VARCHAR2(50), group_code VARCHAR2(20));
  TYPE t_item_tab IS TABLE OF t_item_rec INDEX BY PLS_INTEGER;
  TYPE t_grp_rec IS RECORD (
    group_code VARCHAR2(20), group_name VARCHAR2(200)
  );
  TYPE t_grp_tab IS TABLE OF t_grp_rec INDEX BY PLS_INTEGER;

  v_items    t_item_tab;
  v_groups   t_grp_tab;
  v_group_id NUMBER;
  v_rule_id  NUMBER;
  v_ver_id   NUMBER;
  v_cond_id  NUMBER;
  v_act_id   NUMBER;
BEGIN
  -- 组定义
  v_groups(1).group_code := 'ZT01'; v_groups(1).group_name := '止血互斥组01';
  v_groups(2).group_code := 'ZT02'; v_groups(2).group_name := '止血互斥组02';
  v_groups(3).group_code := 'ZT03'; v_groups(3).group_name := '止血互斥组03';
  v_groups(4).group_code := 'ZT04'; v_groups(4).group_name := '止血互斥组04';

  -- 项目清单（item_code, 所属组编码）
  -- MARK=1 → ZT01
  v_items( 1).item_code := 'F00000209723'; v_items( 1).group_code := 'ZT01';
  v_items( 2).item_code := 'F00000209729'; v_items( 2).group_code := 'ZT01';
  v_items( 3).item_code := 'F00000209730'; v_items( 3).group_code := 'ZT01';
  -- MARK=2 → ZT02
  v_items( 4).item_code := 'F00000209708'; v_items( 4).group_code := 'ZT02';
  v_items( 5).item_code := 'F00000209720'; v_items( 5).group_code := 'ZT02';
  -- MARK=3 → ZT03
  v_items( 6).item_code := 'F00000209705'; v_items( 6).group_code := 'ZT03';
  v_items( 7).item_code := 'F00000209706'; v_items( 7).group_code := 'ZT03';
  -- MARK=4 → ZT04
  v_items( 8).item_code := 'F00000209714'; v_items( 8).group_code := 'ZT04';
  v_items( 9).item_code := 'F00000209724'; v_items( 9).group_code := 'ZT04';
  v_items(10).item_code := 'F00000209731'; v_items(10).group_code := 'ZT04';

  -- 建立4个项目组
  FOR g IN 1..v_groups.COUNT LOOP
    v_group_id := SEQ_PR_ITEM_GROUP.NEXTVAL;
    INSERT INTO PR_ITEM_GROUP (
      GROUP_ID, GROUP_CODE, GROUP_NAME, GROUP_TYPE, IS_ENABLED, CREATED_BY
    ) VALUES (
      v_group_id, v_groups(g).group_code, v_groups(g).group_name,
      'MUTEX', 'Y', 'IMPORT'
    );
  END LOOP;

  -- 为每个项目建立组明细和规则
  FOR i IN 1..v_items.COUNT LOOP
    -- 查找所属组的 GROUP_ID
    SELECT GROUP_ID INTO v_group_id
    FROM PR_ITEM_GROUP WHERE GROUP_CODE = v_items(i).group_code;

    -- 项目组明细
    INSERT INTO PR_ITEM_GROUP_DETAIL (
      DETAIL_ID, GROUP_ID, ITEM_CODE, ROLE_TYPE, SORT_NO, IS_ENABLED
    ) VALUES (
      SEQ_PR_ITEM_GROUP_DETAIL.NEXTVAL, v_group_id,
      v_items(i).item_code, 'MAIN', i, 'Y'
    );

    -- 规则头
    v_rule_id := SEQ_PR_RULE_HEADER.NEXTVAL;
    INSERT INTO PR_RULE_HEADER (
      RULE_ID, RULE_CODE, RULE_NAME, RULE_CATEGORY, RULE_SCOPE,
      ITEM_CODE, GROUP_CODE, PRIORITY, CURRENT_VERSION, STATUS, IS_ENABLED,
      EFFECTIVE_FROM, ROLLBACK_MODE, CREATED_BY, REMARK
    ) VALUES (
      v_rule_id,
      'ZT_' || v_items(i).group_code || '_' || v_items(i).item_code,
      '止血互斥-' || v_items(i).group_code || '-' || v_items(i).item_code,
      'LIMIT', 'ITEM',
      v_items(i).item_code, v_items(i).group_code,
      100, 1, 'PUBLISHED', 'Y',
      TRUNC(SYSDATE), 'LEGACY_EQUIVALENT',
      'IMPORT', '从 RestrictingfeeZT 字典迁移'
    );

    -- 版本
    v_ver_id := SEQ_PR_RULE_VERSION.NEXTVAL;
    INSERT INTO PR_RULE_VERSION (
      VERSION_ID, RULE_ID, VERSION_NO, VERSION_STATUS,
      EFFECTIVE_FROM, PUBLISHED_BY, PUBLISHED_AT, PUBLISH_REMARK
    ) VALUES (
      v_ver_id, v_rule_id, 1, 'PUBLISHED',
      TRUNC(SYSDATE), 'IMPORT', SYSDATE, '初始迁移'
    );

    -- 条件：项目匹配
    v_cond_id := SEQ_PR_RULE_CONDITION.NEXTVAL;
    INSERT INTO PR_RULE_CONDITION (
      CONDITION_ID, RULE_ID, VERSION_NO, CONDITION_GROUP,
      CONDITION_TYPE, OPERATOR_TYPE, LEFT_KEY, RIGHT_VALUE, SORT_NO, IS_ENABLED
    ) VALUES (
      v_cond_id, v_rule_id, 1, 'DEFAULT',
      'ITEM_MATCH', 'EQ', 'ITEM_CODE', v_items(i).item_code, 1, 'Y'
    );

    -- 动作1：同组互斥（EXCLUSIVE_GROUP 模式，HIS无需传 ItemGroupCode）
    v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
    INSERT INTO PR_RULE_ACTION (
      ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
      PARAMS_JSON, EXCLUSIVE_GROUP, SORT_NO, ON_ERROR, IS_ENABLED
    ) VALUES (
      v_act_id, v_rule_id, 1,
      'SAME_GROUP_MUTEX', 'SameGroupMutexExecutor',
      '{"GroupDimension":"EXCLUSIVE_GROUP","MaxCountPerGroup":1}',
      v_items(i).group_code,
      10, 'STOP', 'Y'
    );

    -- 动作2：超限归零
    v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
    INSERT INTO PR_RULE_ACTION (
      ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
      PARAMS_JSON, SORT_NO, ON_ERROR, IS_ENABLED
    ) VALUES (
      v_act_id, v_rule_id, 1,
      'DISCOUNT_EXCEED_TO_ZERO', 'ExceedToZeroExecutor',
      '{"ExceedAction":"ZERO"}', 20, 'STOP', 'Y'
    );
  END LOOP;

  COMMIT;
  DBMS_OUTPUT.PUT_LINE('RestrictingfeeZT: ' || v_items.COUNT || ' 条规则已导入');
END;
/


-- ============================================================
-- 第三部分：RestrictingfeeTX1 胎心监护互斥组（1组15项）
-- 所有项目属于同一个互斥组 TX1，同次最多收1项
-- 规则结构：ITEM_MATCH → SAME_GROUP_MUTEX + DISCOUNT_EXCEED_TO_ZERO
-- ============================================================
DECLARE
  TYPE t_code_tab IS TABLE OF VARCHAR2(50) INDEX BY PLS_INTEGER;
  v_items    t_code_tab;
  v_group_id NUMBER;
  v_rule_id  NUMBER;
  v_ver_id   NUMBER;
  v_cond_id  NUMBER;
  v_act_id   NUMBER;
BEGIN
  -- 15 条胎心监护项目
  v_items( 1) := 'F00000209650';
  v_items( 2) := 'F00000209651';
  v_items( 3) := 'F00000209652';
  v_items( 4) := 'F00000209662';
  v_items( 5) := 'F00000209663';
  v_items( 6) := 'F00000209664';
  v_items( 7) := 'F00000209665';
  v_items( 8) := 'F00000209666';
  v_items( 9) := 'F00000209667';
  v_items(10) := 'F00000209668';
  v_items(11) := 'F00000209669';
  v_items(12) := 'F00000209670';
  v_items(13) := 'F00000209671';
  v_items(14) := 'F00000209672';
  v_items(15) := 'F00000209673';

  -- 建立项目组
  v_group_id := SEQ_PR_ITEM_GROUP.NEXTVAL;
  INSERT INTO PR_ITEM_GROUP (
    GROUP_ID, GROUP_CODE, GROUP_NAME, GROUP_TYPE, IS_ENABLED, CREATED_BY
  ) VALUES (
    v_group_id, 'TX1', '胎心监护互斥组', 'MUTEX', 'Y', 'IMPORT'
  );

  -- 为每个项目建立组明细和规则
  FOR i IN 1..v_items.COUNT LOOP
    -- 项目组明细
    INSERT INTO PR_ITEM_GROUP_DETAIL (
      DETAIL_ID, GROUP_ID, ITEM_CODE, ROLE_TYPE, SORT_NO, IS_ENABLED
    ) VALUES (
      SEQ_PR_ITEM_GROUP_DETAIL.NEXTVAL, v_group_id,
      v_items(i), 'MAIN', i, 'Y'
    );

    -- 规则头
    v_rule_id := SEQ_PR_RULE_HEADER.NEXTVAL;
    INSERT INTO PR_RULE_HEADER (
      RULE_ID, RULE_CODE, RULE_NAME, RULE_CATEGORY, RULE_SCOPE,
      ITEM_CODE, GROUP_CODE, PRIORITY, CURRENT_VERSION, STATUS, IS_ENABLED,
      EFFECTIVE_FROM, ROLLBACK_MODE, CREATED_BY, REMARK
    ) VALUES (
      v_rule_id,
      'TX_' || v_items(i),
      '胎心互斥-' || v_items(i),
      'LIMIT', 'ITEM',
      v_items(i), 'TX1',
      100, 1, 'PUBLISHED', 'Y',
      TRUNC(SYSDATE), 'LEGACY_EQUIVALENT',
      'IMPORT', '从 RestrictingfeeTX1 字典迁移'
    );

    -- 版本
    v_ver_id := SEQ_PR_RULE_VERSION.NEXTVAL;
    INSERT INTO PR_RULE_VERSION (
      VERSION_ID, RULE_ID, VERSION_NO, VERSION_STATUS,
      EFFECTIVE_FROM, PUBLISHED_BY, PUBLISHED_AT, PUBLISH_REMARK
    ) VALUES (
      v_ver_id, v_rule_id, 1, 'PUBLISHED',
      TRUNC(SYSDATE), 'IMPORT', SYSDATE, '初始迁移'
    );

    -- 条件：项目匹配
    v_cond_id := SEQ_PR_RULE_CONDITION.NEXTVAL;
    INSERT INTO PR_RULE_CONDITION (
      CONDITION_ID, RULE_ID, VERSION_NO, CONDITION_GROUP,
      CONDITION_TYPE, OPERATOR_TYPE, LEFT_KEY, RIGHT_VALUE, SORT_NO, IS_ENABLED
    ) VALUES (
      v_cond_id, v_rule_id, 1, 'DEFAULT',
      'ITEM_MATCH', 'EQ', 'ITEM_CODE', v_items(i), 1, 'Y'
    );

    -- 动作1：同组互斥（EXCLUSIVE_GROUP 模式）
    v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
    INSERT INTO PR_RULE_ACTION (
      ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
      PARAMS_JSON, EXCLUSIVE_GROUP, SORT_NO, ON_ERROR, IS_ENABLED
    ) VALUES (
      v_act_id, v_rule_id, 1,
      'SAME_GROUP_MUTEX', 'SameGroupMutexExecutor',
      '{"GroupDimension":"EXCLUSIVE_GROUP","MaxCountPerGroup":1}',
      'TX1',
      10, 'STOP', 'Y'
    );

    -- 动作2：超限归零
    v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
    INSERT INTO PR_RULE_ACTION (
      ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
      PARAMS_JSON, SORT_NO, ON_ERROR, IS_ENABLED
    ) VALUES (
      v_act_id, v_rule_id, 1,
      'DISCOUNT_EXCEED_TO_ZERO', 'ExceedToZeroExecutor',
      '{"ExceedAction":"ZERO"}', 20, 'STOP', 'Y'
    );
  END LOOP;

  COMMIT;
  DBMS_OUTPUT.PUT_LINE('RestrictingfeeTX1: ' || v_items.COUNT || ' 条规则已导入');
END;
/


-- ============================================================
-- 第四部分：FIN_DISCOUNT_FEE B类比例折价规则【待数据】
-- 说明：FIN_DISCOUNT_FEE 表数据未在字典规则.sql 中提供。
--       收到数据后补充，规则结构参考：
--         ITEM_MATCH → INCREMENT_PERCENT（比例折扣）
--         或         → AMOUNT_FLOOR（金额下限）
-- ============================================================
-- TODO: 等待 FIN_DISCOUNT_FEE 数据后补充 B 类折价规则
