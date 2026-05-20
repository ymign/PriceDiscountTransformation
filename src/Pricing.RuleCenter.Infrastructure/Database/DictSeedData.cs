using Pricing.RuleCenter.Core.Aggregates.Catalog;

namespace Pricing.RuleCenter.Infrastructure.Database;

/// <summary>
/// 字典种子数据，提供系统初始化时需要预置的字典记录。
///
/// 架构位置：
///   位于基础设施层（Infrastructure），在数据库初始化或迁移时调用。
///   将原本硬编码在引擎和服务中的常量列表，转为 PR_DICT 表中的可配置数据。
///
/// 使用场景：
///   - 系统首次部署时插入基础字典数据。
///   - 字典数据缺失时的兜底修复。
///   - 升级脚本中补充新增的字典类型。
///
/// 设计原则：
///   - 每个字典类型的种子数据集中在此类中管理，避免散落在各处。
///   - DICT_TYPE 作为分类键，DICT_CODE 作为编码键，SORT_NO 控制排序。
///   - 种子数据仅提供初始值，后续由工作台维护人员通过字典管理界面修改。
/// </summary>
public static class DictSeedData
{
    private sealed record DictSeedItem
    {
        public string DictType { get; init; } = string.Empty;

        public string DictCode { get; init; } = string.Empty;

        public string DictName { get; init; } = string.Empty;

        public int SortNo { get; init; }

        public string Remark { get; init; } = string.Empty;
    }

    /// <summary>
    /// 动作执行顺序字典类型编码。
    ///
    /// 业务背景：
    ///   计价引擎执行动作链时，必须对齐旧 HIS 正式环境顺序：
    ///   "换算 → 数量限制/互斥 → 比例折价 → TOPPRICE 封顶 → 子项加收 → 超限归零兜底"。
    ///   这个顺序不能按新设计文档中的"公式优先"理解，否则 Restrictingfee 与 FIN_DISCOUNT_FEE
    ///   同时命中时会用未截断数量计算折价金额，产生收费偏差。
    ///
    ///   此字典类型将执行顺序从代码硬编码改为数据库配置，新增动作类型只需插入字典记录，
    ///   无需修改引擎代码。
    ///
    /// 字典项含义：
    ///   - DICT_CODE：动作类型编码，对应 PR_RULE_ACTION.ACTION_TYPE
    ///   - SORT_NO：执行顺序号，数字越小越先执行（建议间隔 10，便于插入）
    ///   - DICT_NAME：动作类型中文名称，供工作台展示
    ///   - IS_ENABLED：是否参与排序计算，"N" 时该动作类型不参与执行链
    /// </summary>
    public const string ActionTypeOrderType = "ACTION_TYPE_ORDER";

    /// <summary>
    /// 互斥动作类型字典类型编码。
    ///
    /// 业务背景：
    ///   同一项目在相同场景和生效期内，不允许同时存在某些动作类型的规则（如不能同时有两条折价公式规则）。
    ///   此字典类型维护哪些动作类型之间互斥，发布校验时从字典读取互斥规则，而非硬编码在发布服务中。
    ///
    /// 字典项含义：
    ///   - DICT_CODE：互斥的动作类型编码，对应 PR_RULE_ACTION.ACTION_TYPE
    ///   - DICT_NAME：动作类型中文名称
    ///   - IS_ENABLED：是否参与互斥校验，"N" 时该动作类型不再作为互斥校验项
    /// </summary>
    public const string MutuallyExclusiveActionType = "MUTUALLY_EXCLUSIVE_ACTION_TYPE";

    /// <summary>
    /// 获取动作执行顺序的种子数据。
    ///
    /// 返回 PR_DICT 表中 DICT_TYPE = "ACTION_TYPE_ORDER" 的初始记录集合。
    /// SORT_NO 间隔为 10，便于后续在两个动作类型之间插入新的动作类型。
    ///
    /// 执行顺序设计依据：
    ///   旧 HIS 在 ucDisplay.GetFeeItemList 中先调用 ConvertRestrictingfee，再调用 ConvertDiscountfee；
    ///   ConvertDiscountfee 内部先按 DISCOUNT_RATE 计算比例折价，再按 TOPPRICE 做金额封顶。
    ///   因此这里必须把数量类限制排在 FORMULA_CALC 前，把 APPLY_MAX_AMOUNT 排在 FORMULA_CALC 后。
    ///
    ///   10  CONVERT_QTY             — 双单位换算，公式依赖换算后数量
    ///   20  APPLY_DAY_LIMIT_QTY     — 日数量限制，先截断可收费数量
    ///   30  APPLY_TIME_WINDOW_LIMIT — 时间窗数量限制（如2小时窗），先截断可收费数量
    ///   40  APPLY_ONCE_LIMIT_QTY    — 单次数量限制，先截断可收费数量
    ///   50  SAME_GROUP_MUTEX        — 同组互斥，先决定当前项目是否还能收费
    ///   60  FORMULA_CALC            — 比例折价，使用前面限制后的 FinalQty
    ///   70  APPLY_MIN_AMOUNT        — 金额下限，公式之后才能比较；FIN_DISCOUNT_FEE 当前不生成
    ///   80  APPLY_MAX_AMOUNT        — 金额上限/TOPPRICE，必须在比例折价后比较
    ///   85  SAME_OPERATION_CEILING  — 同手术封顶，金额类累计封顶必须在公式和单项封顶之后执行
    ///   90  ADD_CHILD_ITEM          — 子项加收
    ///   100 DISCOUNT_EXCEED_TO_ZERO — 超出部分归零兜底，必须最后执行
    /// </summary>
    /// <returns>动作执行顺序字典种子数据列表。</returns>
    public static IReadOnlyList<Dict> GetActionTypeOrderSeedData()
    {
        return new List<Dict>
        {
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "CONVERT_QTY",
                DictName = "双单位换算",
                SortNo = 10,
                Remark = "双单位换算，公式依赖换算后数量"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "APPLY_DAY_LIMIT_QTY",
                DictName = "日数量限制",
                SortNo = 20,
                Remark = "日数量限制，先截断可收费数量"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "APPLY_TIME_WINDOW_LIMIT",
                DictName = "时间窗数量限制",
                SortNo = 30,
                Remark = "时间窗数量限制（如旧 HIS 2 小时窗），先截断可收费数量"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "APPLY_ONCE_LIMIT_QTY",
                DictName = "单次数量限制",
                SortNo = 40,
                Remark = "单次数量限制，先截断可收费数量"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "SAME_GROUP_MUTEX",
                DictName = "同组互斥",
                SortNo = 50,
                Remark = "同组互斥，先决定当前项目是否还能收费"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "FORMULA_CALC",
                DictName = "公式计算",
                SortNo = 60,
                Remark = "比例折价，使用前面限制后的 FinalQty"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "APPLY_MIN_AMOUNT",
                DictName = "金额下限",
                SortNo = 70,
                Remark = "金额下限，公式之后才能比较；FIN_DISCOUNT_FEE 当前不生成该动作"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "APPLY_MAX_AMOUNT",
                DictName = "金额上限",
                SortNo = 80,
                Remark = "金额上限/TOPPRICE，必须在比例折价后比较"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "SAME_OPERATION_CEILING",
                DictName = "同手术封顶",
                SortNo = 85,
                Remark = "同手术封顶，金额类累计封顶必须在公式和单项封顶之后执行"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "ADD_CHILD_ITEM",
                DictName = "子项加收",
                SortNo = 90,
                Remark = "子项加收"
            }),
            CreateDict(new()
            {
                DictType = ActionTypeOrderType,
                DictCode = "DISCOUNT_EXCEED_TO_ZERO",
                DictName = "超出部分归零",
                SortNo = 100,
                Remark = "超出部分归零兜底，必须最后执行"
            })
        };
    }

    /// <summary>
    /// 获取互斥动作类型的种子数据。
    ///
    /// 返回 PR_DICT 表中 DICT_TYPE = "MUTUALLY_EXCLUSIVE_ACTION_TYPE" 的初始记录集合。
    /// 这些动作类型在同一项目、相同场景和生效期内不允许同时存在于不同规则中。
    ///
    /// 互斥设计依据：
    ///   FORMULA_CALC            — 折价公式：同一项目不能有多套折价公式
    ///   APPLY_MIN_AMOUNT        — 金额下限：同一项目不能有多套下限规则
    ///   APPLY_MAX_AMOUNT        — 金额上限：同一项目不能有多套上限规则
    ///   APPLY_DAY_LIMIT_QTY     — 日数量限制：同一项目不能有多套日限规则
    ///   APPLY_ONCE_LIMIT_QTY    — 单次数量限制：同一项目不能有多套单次限规则
    ///   APPLY_TIME_WINDOW_LIMIT — 时间窗限制：同一项目不能有多套时间窗规则
    ///
    /// 注意：CONVERT_QTY（换算规则）允许按不同部位维护不同规则，因此不在互斥列表中。
    /// </summary>
    /// <returns>互斥动作类型字典种子数据列表。</returns>
    public static IReadOnlyList<Dict> GetMutuallyExclusiveActionTypeSeedData()
    {
        return new List<Dict>
        {
            CreateDict(new()
            {
                DictType = MutuallyExclusiveActionType,
                DictCode = "FORMULA_CALC",
                DictName = "公式计算",
                SortNo = 10,
                Remark = "同一项目不能有多套折价公式"
            }),
            CreateDict(new()
            {
                DictType = MutuallyExclusiveActionType,
                DictCode = "APPLY_MIN_AMOUNT",
                DictName = "金额下限",
                SortNo = 20,
                Remark = "同一项目不能有多套下限规则"
            }),
            CreateDict(new()
            {
                DictType = MutuallyExclusiveActionType,
                DictCode = "APPLY_MAX_AMOUNT",
                DictName = "金额上限",
                SortNo = 30,
                Remark = "同一项目不能有多套上限规则"
            }),
            CreateDict(new()
            {
                DictType = MutuallyExclusiveActionType,
                DictCode = "APPLY_DAY_LIMIT_QTY",
                DictName = "日数量限制",
                SortNo = 40,
                Remark = "同一项目不能有多套日限规则"
            }),
            CreateDict(new()
            {
                DictType = MutuallyExclusiveActionType,
                DictCode = "APPLY_ONCE_LIMIT_QTY",
                DictName = "单次数量限制",
                SortNo = 50,
                Remark = "同一项目不能有多套单次限规则"
            }),
            CreateDict(new()
            {
                DictType = MutuallyExclusiveActionType,
                DictCode = "APPLY_TIME_WINDOW_LIMIT",
                DictName = "时间窗数量限制",
                SortNo = 60,
                Remark = "同一项目不能有多套时间窗规则"
            })
        };
    }

    /// <summary>
    /// 创建一条字典种子数据记录。
    /// </summary>
    /// <param name="dictType">字典类型编码。</param>
    /// <param name="dictCode">字典项编码。</param>
    /// <param name="dictName">字典项显示名称。</param>
    /// <param name="sortNo">排序号，数字越小越靠前。</param>
    /// <param name="remark">备注说明。</param>
    /// <returns>字典实体实例。</returns>
    private static Dict CreateDict(DictSeedItem seed)
    {
        return new Dict
        {
            DictType = seed.DictType,
            DictCode = seed.DictCode,
            DictName = seed.DictName,
            SortNo = seed.SortNo,
            IsEnabled = "Y",
            Remark = seed.Remark
        };
    }
}
