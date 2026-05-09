-- ============================================================
-- 统一计价规则中心 - 公式定义初始化
-- 基于 12-历史规则分类与公式定义.md 的 4 种公式
-- ============================================================

-- 1. 标准计价：单价×数量（默认，无需公式执行器）
INSERT INTO PR_FORMULA_DEF (FORMULA_ID, FORMULA_CODE, FORMULA_NAME, FORMULA_DESC, EXECUTOR_CODE, PARAM_SCHEMA_JSON, IS_ENABLED)
VALUES (SEQ_PR_FORMULA_DEF.NEXTVAL, 'UNIT_PRICE_TIMES_QTY', '单价×数量',
  '标准计价公式，totalAmount = unitPrice × qty',
  'UnitPriceTimesQtyExecutor',
  '{"params":[]}',
  'Y');

-- 2. 固定比例递增：第1个100%，后续按rate加收
INSERT INTO PR_FORMULA_DEF (FORMULA_ID, FORMULA_CODE, FORMULA_NAME, FORMULA_DESC, EXECUTOR_CODE, PARAM_SCHEMA_JSON, IS_ENABLED)
VALUES (SEQ_PR_FORMULA_DEF.NEXTVAL, 'INCREMENT_PERCENT', '固定比例递增',
  '总价 = 单价 × (数量 × rate + (1 - rate))。第1个按100%收费，第2个及以上按rate收费。浅表肿物rate=0.5, 自体皮移植rate=0.7',
  'IncrementPercentExecutor',
  '{"params":[{"name":"rate","type":"decimal","required":true,"desc":"递增比例，如0.5表示50%"}]}',
  'Y');

-- 3. 分部位单位换算：面积→数量换算
INSERT INTO PR_FORMULA_DEF (FORMULA_ID, FORMULA_CODE, FORMULA_NAME, FORMULA_DESC, EXECUTOR_CODE, PARAM_SCHEMA_JSON, IS_ENABLED)
VALUES (SEQ_PR_FORMULA_DEF.NEXTVAL, 'CONVERT_QTY_BY_PART', '分部位单位换算',
  '按部位将面积换算为计费数量。convertedQty = ceil(inputArea / convertBaseArea)，totalAmount = unitPrice × convertedQty。多肿物分别计算后求和，每个肿物可独立封顶',
  'ConvertQtyByPartExecutor',
  '{"params":[{"name":"convertBaseArea","type":"decimal","required":true,"desc":"换算基础面积，如头面部4cm²"},{"name":"maxAmountPerLesion","type":"decimal","required":false,"desc":"单肿物封顶金额"}]}',
  'Y');

-- 4. 面积分段递增：以baseArea为基础，超出按stepRate递增
INSERT INTO PR_FORMULA_DEF (FORMULA_ID, FORMULA_CODE, FORMULA_NAME, FORMULA_DESC, EXECUTOR_CODE, PARAM_SCHEMA_JSON, IS_ENABLED)
VALUES (SEQ_PR_FORMULA_DEF.NEXTVAL, 'AREA_STEP_INCREMENT', '面积分段递增',
  '面积≤baseArea时原价，超出后 extraSteps = ceil((area-baseArea)/baseArea), amount = unitPrice × (1 + extraSteps × stepRate)。默认baseArea=15cm², stepRate=0.15',
  'AreaStepIncrementExecutor',
  '{"params":[{"name":"baseArea","type":"decimal","required":true,"desc":"基础面积(cm²)，默认15"},{"name":"stepRate","type":"decimal","required":true,"desc":"每步加收比例，默认0.15"}]}',
  'Y');

-- 5. 子项比例加收：基于主项目最终金额按比例计算
INSERT INTO PR_FORMULA_DEF (FORMULA_ID, FORMULA_CODE, FORMULA_NAME, FORMULA_DESC, EXECUTOR_CODE, PARAM_SCHEMA_JSON, IS_ENABLED)
VALUES (SEQ_PR_FORMULA_DEF.NEXTVAL, 'CHILD_ITEM_PERCENT', '子项比例加收',
  '子项金额 = 主项目最终金额 × childRate。注意基准是主项目经过公式计算和封顶之后的最终金额，不是原始单价',
  'ChildItemPercentExecutor',
  '{"params":[{"name":"childRate","type":"decimal","required":true,"desc":"加收比例，如0.30表示30%"},{"name":"mainItemCode","type":"string","required":true,"desc":"主项目编码"}]}',
  'Y');

-- 6. 金额封顶：不超过固定金额
INSERT INTO PR_FORMULA_DEF (FORMULA_ID, FORMULA_CODE, FORMULA_NAME, FORMULA_DESC, EXECUTOR_CODE, PARAM_SCHEMA_JSON, IS_ENABLED)
VALUES (SEQ_PR_FORMULA_DEF.NEXTVAL, 'CEILING_AMOUNT', '金额封顶',
  'finalAmount = min(calculatedAmount, maxAmount)',
  'CeilingAmountExecutor',
  '{"params":[{"name":"maxAmount","type":"decimal","required":true,"desc":"封顶金额"}]}',
  'Y');


COMMIT;
