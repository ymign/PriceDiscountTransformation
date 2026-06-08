-- ============================================================
-- 04-import-rules.sql
-- 从 com_dictionary 迁移规则到规则中心
-- 生成日期: 2026-05-11
-- 数据来源: 字典规则.sql
--
-- 包含规则：
--   1. Restrictingfee  (26条) — 2小时时间窗口限额，超限归零，排除7021科室
--   2. RestrictingfeeCP (1组8项) — 床旁加收项目共享2小时额度，排除7021科室
--   3. RestrictingfeeZT (4组) — 同次止血手术互斥组规则，排除7021科室
--   4. RestrictingfeeTX1(1组) — 胎心监护15项互斥组规则，排除7021科室
--   5. FIN_DISCOUNT_FEE        — B类比例折价规则，运行时从源表动态导入
--
-- 注意：
--   - RestrictingfeeCP 8项同时存在于 Restrictingfee 中；普通时间窗只限制同项目，
--     CP 还需要额外 SAME_GROUP_MUTEX，才能复刻旧 SQL “所有床旁项目共享额度”的口径
--   - Astrictpackagefee 4项不是独立折价规则；旧逻辑命中时忽略历史占用，本次收费内仍会继续扣减
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
    -- 旧 HIS SQL 使用 fee_date >= SYSDATE - (2/24)，迁移时直接写 120 分钟滑动窗口，
    -- 避免后续误读为自然日累计或固定整点两小时桶。
    v_params := '{"WindowMinutes":120,"LimitQty":' || v_items(i).limit_qty || '}';

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
-- 第二部分：RestrictingfeeCP 床旁加收共享额度（1组8项）
-- 旧 SQL 对当前床旁项目查询所有 RestrictingfeeCP 项目的 2 小时历史收费量。
-- 因此除了第一部分的同项目时间窗限制，还必须补一条同组互斥规则。
-- 规则结构：ITEM_MATCH + CHARGE_DEPT_EXCLUDE → SAME_GROUP_MUTEX + DISCOUNT_EXCEED_TO_ZERO
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
  -- 8 条床旁加收项目，均在 Restrictingfee 中已有同项目 2 小时时间窗规则。
  v_items(1) := 'F00000209343';
  v_items(2) := 'F00000209348';
  v_items(3) := 'F00000209351';
  v_items(4) := 'F00000209356';
  v_items(5) := 'F00000209361';
  v_items(6) := 'F00000209366';
  v_items(7) := 'F00000209369';
  v_items(8) := 'F00000209372';

  v_group_id := SEQ_PR_ITEM_GROUP.NEXTVAL;
  INSERT INTO PR_ITEM_GROUP (
    GROUP_ID, GROUP_CODE, GROUP_NAME, GROUP_TYPE, IS_ENABLED, CREATED_BY
  ) VALUES (
    v_group_id, 'CP01', '床旁加收共享额度组', 'MUTEX', 'Y', 'IMPORT'
  );

  FOR i IN 1..v_items.COUNT LOOP
    INSERT INTO PR_ITEM_GROUP_DETAIL (
      DETAIL_ID, GROUP_ID, ITEM_CODE, ROLE_TYPE, SORT_NO, IS_ENABLED
    ) VALUES (
      SEQ_PR_ITEM_GROUP_DETAIL.NEXTVAL, v_group_id,
      v_items(i), 'MAIN', i, 'Y'
    );

    v_rule_id := SEQ_PR_RULE_HEADER.NEXTVAL;
    INSERT INTO PR_RULE_HEADER (
      RULE_ID, RULE_CODE, RULE_NAME, RULE_CATEGORY, RULE_SCOPE,
      ITEM_CODE, GROUP_CODE, PRIORITY, CURRENT_VERSION, STATUS, IS_ENABLED,
      EFFECTIVE_FROM, ROLLBACK_MODE, CREATED_BY, REMARK
    ) VALUES (
      v_rule_id,
      'CP_' || v_items(i),
      '床旁共享额度-' || v_items(i),
      'LIMIT', 'ITEM',
      v_items(i), 'CP01',
      100, 1, 'PUBLISHED', 'Y',
      TRUNC(SYSDATE), 'LEGACY_EQUIVALENT',
      'IMPORT', '从 RestrictingfeeCP 字典迁移'
    );

    v_ver_id := SEQ_PR_RULE_VERSION.NEXTVAL;
    INSERT INTO PR_RULE_VERSION (
      VERSION_ID, RULE_ID, VERSION_NO, VERSION_STATUS,
      EFFECTIVE_FROM, PUBLISHED_BY, PUBLISHED_AT, PUBLISH_REMARK
    ) VALUES (
      v_ver_id, v_rule_id, 1, 'PUBLISHED',
      TRUNC(SYSDATE), 'IMPORT', SYSDATE, '初始迁移'
    );

    v_cond_id := SEQ_PR_RULE_CONDITION.NEXTVAL;
    INSERT INTO PR_RULE_CONDITION (
      CONDITION_ID, RULE_ID, VERSION_NO, CONDITION_GROUP,
      CONDITION_TYPE, OPERATOR_TYPE, LEFT_KEY, RIGHT_VALUE, SORT_NO, IS_ENABLED
    ) VALUES (
      v_cond_id, v_rule_id, 1, 'DEFAULT',
      'ITEM_MATCH', 'EQ', 'ITEM_CODE', v_items(i), 1, 'Y'
    );

    v_cond_id := SEQ_PR_RULE_CONDITION.NEXTVAL;
    INSERT INTO PR_RULE_CONDITION (
      CONDITION_ID, RULE_ID, VERSION_NO, CONDITION_GROUP,
      CONDITION_TYPE, OPERATOR_TYPE, LEFT_KEY, RIGHT_VALUE, SORT_NO, IS_ENABLED
    ) VALUES (
      v_cond_id, v_rule_id, 1, 'DEFAULT',
      'CHARGE_DEPT_EXCLUDE', 'EQ', 'CHARGE_DEPT_CODE', '7021', 2, 'Y'
    );

    v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
    INSERT INTO PR_RULE_ACTION (
      ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
      PARAMS_JSON, EXCLUSIVE_GROUP, SORT_NO, ON_ERROR, IS_ENABLED
    ) VALUES (
      v_act_id, v_rule_id, 1,
      'SAME_GROUP_MUTEX', 'SameGroupMutexExecutor',
      '{"GroupDimension":"EXCLUSIVE_GROUP","MaxCountPerGroup":1,"WindowMinutes":120}',
      'CP01',
      10, 'STOP', 'Y'
    );

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
  DBMS_OUTPUT.PUT_LINE('RestrictingfeeCP: ' || v_items.COUNT || ' 条规则已导入');
END;
/


-- ============================================================
-- 第三部分：RestrictingfeeZT 止血互斥组（4组10项）
-- MARK字段标记组号：1→ZT01，2→ZT02，3→ZT03，4→ZT04
-- 规则结构：ITEM_MATCH + CHARGE_DEPT_EXCLUDE → SAME_GROUP_MUTEX + DISCOUNT_EXCEED_TO_ZERO
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

    -- 条件：排除挂号部7021
    v_cond_id := SEQ_PR_RULE_CONDITION.NEXTVAL;
    INSERT INTO PR_RULE_CONDITION (
      CONDITION_ID, RULE_ID, VERSION_NO, CONDITION_GROUP,
      CONDITION_TYPE, OPERATOR_TYPE, LEFT_KEY, RIGHT_VALUE, SORT_NO, IS_ENABLED
    ) VALUES (
      v_cond_id, v_rule_id, 1, 'DEFAULT',
      'CHARGE_DEPT_EXCLUDE', 'EQ', 'CHARGE_DEPT_CODE', '7021', 2, 'Y'
    );

    -- 动作1：同组互斥（EXCLUSIVE_GROUP 模式，HIS无需传 ItemGroupCode）
    v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
    INSERT INTO PR_RULE_ACTION (
      ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
      PARAMS_JSON, EXCLUSIVE_GROUP, SORT_NO, ON_ERROR, IS_ENABLED
    ) VALUES (
      v_act_id, v_rule_id, 1,
      'SAME_GROUP_MUTEX', 'SameGroupMutexExecutor',
      '{"GroupDimension":"EXCLUSIVE_GROUP","MaxCountPerGroup":1,"WindowMinutes":120}',
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
-- 第四部分：RestrictingfeeTX1 胎心监护互斥组（1组15项）
-- 所有项目属于同一个互斥组 TX1，同次最多收1项
-- 规则结构：ITEM_MATCH + CHARGE_DEPT_EXCLUDE → SAME_GROUP_MUTEX + DISCOUNT_EXCEED_TO_ZERO
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

    -- 条件：排除挂号部7021
    v_cond_id := SEQ_PR_RULE_CONDITION.NEXTVAL;
    INSERT INTO PR_RULE_CONDITION (
      CONDITION_ID, RULE_ID, VERSION_NO, CONDITION_GROUP,
      CONDITION_TYPE, OPERATOR_TYPE, LEFT_KEY, RIGHT_VALUE, SORT_NO, IS_ENABLED
    ) VALUES (
      v_cond_id, v_rule_id, 1, 'DEFAULT',
      'CHARGE_DEPT_EXCLUDE', 'EQ', 'CHARGE_DEPT_CODE', '7021', 2, 'Y'
    );

    -- 动作1：同组互斥（EXCLUSIVE_GROUP 模式）
    v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
    INSERT INTO PR_RULE_ACTION (
      ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
      PARAMS_JSON, EXCLUSIVE_GROUP, SORT_NO, ON_ERROR, IS_ENABLED
    ) VALUES (
      v_act_id, v_rule_id, 1,
      'SAME_GROUP_MUTEX', 'SameGroupMutexExecutor',
      '{"GroupDimension":"EXCLUSIVE_GROUP","MaxCountPerGroup":1,"WindowMinutes":120}',
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
-- 第五部分：FIN_DISCOUNT_FEE B类比例折价规则（动态导入）
-- 说明：
--   1. FIN_DISCOUNT_FEE 源数据已放在 sql/FIN_DISCOUNT_FEE.sql。
--   2. 执行本段动态导入前，应先在同一 Oracle 用户下执行 sql/FIN_DISCOUNT_FEE.sql，
--      或确认目标库已经存在 HIS 正式环境同名源表。
--   3. 这里仍不把 56 条数据写死到 PR_ 规则表插入语句中，原因是 FIN_DISCOUNT_FEE 是旧 HIS 源表，
--      动态导入可以保留“源数据先落表、迁移脚本再转换”的可追溯链路。
--   4. 脚本运行时若当前 Oracle 用户可访问 FIN_DISCOUNT_FEE，则自动导入
--      VALID_STATE='1' 且 DISCOUNT_TYPE='2' 的记录。
--   5. 规则结构：
--        ITEM_MATCH → FORMULA_CALC / INCREMENT_PERCENT（第一件原价，第二件起按比例）
--        TOPPRICE > 0 时追加 APPLY_MAX_AMOUNT / AmountCeilingExecutor（最高限价）
--      注意：最终执行顺序不由这里的 SortNo 决定，而由 ACTION_TYPE_ORDER 决定；
--      旧 HIS 口径是 Restrictingfee 数量限制先执行，FIN_DISCOUNT_FEE 比例折价后执行，TOPPRICE 最后封顶。
--   6. FIN_DISCOUNT_FEE 表结构没有最低限价字段，严禁从该表生成 APPLY_MIN_AMOUNT。
--   7. 若源表不存在或无数据，本段只输出提示并跳过，不影响前面 Restrictingfee 规则导入。
-- ============================================================
DECLARE
  v_cursor       SYS_REFCURSOR;
  v_item_code    VARCHAR2(50);
  v_item_name    VARCHAR2(200);
  v_rate_text    VARCHAR2(50);
  v_top_text     VARCHAR2(50);
  v_rate         NUMBER;
  v_topprice     NUMBER;
  v_rate_json    VARCHAR2(50);
  v_top_json     VARCHAR2(50);
  v_rule_id      NUMBER;
  v_ver_id       NUMBER;
  v_cond_id      NUMBER;
  v_act_id       NUMBER;
  v_exists       NUMBER;
  v_source_count NUMBER := 0;
  v_import_count NUMBER := 0;
  v_skip_count   NUMBER := 0;
  v_has_source   NUMBER := 0;

  FUNCTION fmt_num(p_value NUMBER) RETURN VARCHAR2 IS
  BEGIN
    RETURN TO_CHAR(p_value, 'FM9999999990D9999999999', 'NLS_NUMERIC_CHARACTERS=.,');
  END;
BEGIN
  -- 使用动态 SQL 是为了允许规则中心独立库暂时没有 FIN_DISCOUNT_FEE 源表。
  -- 如果使用静态 SELECT，源表不存在时匿名块会直接编译失败，前面的规则导入结果也难以判断。
  BEGIN
    EXECUTE IMMEDIATE
      'SELECT COUNT(*) FROM FIN_DISCOUNT_FEE WHERE VALID_STATE = ''1'' AND DISCOUNT_TYPE = ''2'''
      INTO v_source_count;
    v_has_source := 1;
  EXCEPTION
    WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE('FIN_DISCOUNT_FEE: 源表不存在或不可访问，已跳过比例折价动态导入。错误: ' || SQLERRM);
      v_has_source := 0;
  END;

  IF v_has_source = 1 THEN
    IF v_source_count = 0 THEN
      DBMS_OUTPUT.PUT_LINE('FIN_DISCOUNT_FEE: 未找到 VALID_STATE=1 且 DISCOUNT_TYPE=2 的比例折价数据，未生成 DF_ 规则');
    ELSE
      OPEN v_cursor FOR
        'SELECT ITEM_CODE, ITEM_NAME, DISCOUNT_RATE, NVL(TOPPRICE, ''0'') ' ||
        'FROM FIN_DISCOUNT_FEE ' ||
        'WHERE VALID_STATE = ''1'' AND DISCOUNT_TYPE = ''2''';

      LOOP
        FETCH v_cursor INTO v_item_code, v_item_name, v_rate_text, v_top_text;
        EXIT WHEN v_cursor%NOTFOUND;

        BEGIN
          IF TRIM(v_item_code) IS NULL THEN
            v_skip_count := v_skip_count + 1;
            DBMS_OUTPUT.PUT_LINE('FIN_DISCOUNT_FEE: 跳过 ITEM_CODE 为空的记录');
          ELSIF TRIM(v_rate_text) IS NULL THEN
            v_skip_count := v_skip_count + 1;
            DBMS_OUTPUT.PUT_LINE('FIN_DISCOUNT_FEE: 跳过 DISCOUNT_RATE 为空的记录 ITEM_CODE=' || TRIM(v_item_code));
          ELSE
            v_rate := TO_NUMBER(TRIM(v_rate_text));
            IF TRIM(v_top_text) IS NULL THEN
              v_topprice := 0;
            ELSE
              v_topprice := TO_NUMBER(TRIM(v_top_text));
            END IF;
            v_rate_json := fmt_num(v_rate);
            v_top_json := fmt_num(v_topprice);

            SELECT COUNT(*) INTO v_exists
            FROM PR_RULE_HEADER
            WHERE RULE_CODE = 'DF_' || TRIM(v_item_code);

            IF v_exists > 0 THEN
              v_skip_count := v_skip_count + 1;
              DBMS_OUTPUT.PUT_LINE('FIN_DISCOUNT_FEE: 规则已存在，跳过 ' || TRIM(v_item_code));
            ELSE
              -- 规则头
              v_rule_id := SEQ_PR_RULE_HEADER.NEXTVAL;
              INSERT INTO PR_RULE_HEADER (
                RULE_ID, RULE_CODE, RULE_NAME, RULE_CATEGORY, RULE_SCOPE,
                ITEM_CODE, ITEM_NAME, PRIORITY, CURRENT_VERSION, STATUS, IS_ENABLED,
                EFFECTIVE_FROM, ROLLBACK_MODE, CREATED_BY, REMARK
              ) VALUES (
                v_rule_id,
                'DF_' || TRIM(v_item_code),
                SUBSTR('比例折价-' || NVL(v_item_name, TRIM(v_item_code)), 1, 200),
                'DISCOUNT', 'ITEM',
                TRIM(v_item_code), SUBSTR(v_item_name, 1, 200),
                100, 1, 'PUBLISHED', 'Y',
                TRUNC(SYSDATE), 'LEGACY_EQUIVALENT',
                'IMPORT', '从 FIN_DISCOUNT_FEE 动态迁移'
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
                'ITEM_MATCH', 'EQ', 'ITEM_CODE', TRIM(v_item_code), 1, 'Y'
              );

              -- 动作1：比例折价公式。旧公式为 price + price * rate * (qty - 1)，
              -- 对应 IncrementPercentExecutor 的 Rate 参数。
              v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
              INSERT INTO PR_RULE_ACTION (
                ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
                PARAMS_JSON, SORT_NO, ON_ERROR, IS_ENABLED
              ) VALUES (
                v_act_id, v_rule_id, 1,
                'FORMULA_CALC', 'INCREMENT_PERCENT',
                '{"Rate":' || v_rate_json || '}',
                10, 'STOP', 'Y'
              );

              -- 动作2：最高限价。TOPPRICE=0 或空表示不封顶。
              IF v_topprice > 0 THEN
                v_act_id := SEQ_PR_RULE_ACTION.NEXTVAL;
                INSERT INTO PR_RULE_ACTION (
                  ACTION_ID, RULE_ID, VERSION_NO, ACTION_TYPE, EXECUTOR_CODE,
                  PARAMS_JSON, SORT_NO, ON_ERROR, IS_ENABLED
                ) VALUES (
                  v_act_id, v_rule_id, 1,
                  'APPLY_MAX_AMOUNT', 'AmountCeilingExecutor',
                  '{"MaxAmount":' || v_top_json || '}',
                  20, 'STOP', 'Y'
                );
              END IF;

              v_import_count := v_import_count + 1;
            END IF;
          END IF;
        EXCEPTION
          WHEN OTHERS THEN
            v_skip_count := v_skip_count + 1;
            DBMS_OUTPUT.PUT_LINE(
              'FIN_DISCOUNT_FEE: 跳过异常记录 ITEM_CODE=' || NVL(v_item_code, '<NULL>') ||
              ', DISCOUNT_RATE=' || NVL(v_rate_text, '<NULL>') ||
              ', TOPPRICE=' || NVL(v_top_text, '<NULL>') ||
              ', 错误=' || SQLERRM
            );
        END;
      END LOOP;

      CLOSE v_cursor;
      COMMIT;
      DBMS_OUTPUT.PUT_LINE(
        'FIN_DISCOUNT_FEE: 源数据 ' || v_source_count ||
        ' 条，成功导入 ' || v_import_count ||
        ' 条，跳过 ' || v_skip_count || ' 条'
      );
    END IF;
  END IF;
END;
/
