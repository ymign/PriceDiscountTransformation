using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Events;

namespace Pricing.RuleCenter.Core.Aggregates.Rules;

/// <summary>
/// 规则聚合根，封装规则生命周期状态转换和领域事件发布。
/// </summary>
/// <remarks>
/// <para>
/// 【聚合边界】
/// 本聚合根是规则聚合的唯一入口。聚合边界包含：
///   - RuleAggregate（本类）— 规则身份、适用范围、优先级、当前版本号
///   - RuleVersion — 规则版本，同一规则可有多个草稿/发布版本
///   - RuleCondition — 规则条件，定义"何时触发"
///   - RuleAction — 规则动作，定义"触发后做什么"
///   - RulePublish — 发布流水，记录每次发布/停用/回滚操作
///   - RuleChangeLog — 变更日志，记录面向配置人员的变更摘要
///   - RuleApproval — 审批记录，记录发布审批流程
///   - RuleTestCase / RuleTestRun — 测试用例和测试执行结果
/// </para>
/// <para>
/// 【对应 Oracle 表】
/// PR_RULE_HEADER。主键 RULE_ID 由 SEQUENCE 生成。
/// </para>
/// <para>
/// 【在规则体系中的角色】
/// 规则主档保存规则身份、适用范围、优先级和当前发布版本。
/// 条件（PR_RULE_CONDITION）和动作（PR_RULE_ACTION）挂在具体版本上，
/// 主档的 CurrentVersion 决定计价时读取哪套版本明细。
/// </para>
/// <para>
/// 【规则生命周期状态机】
/// <code>
/// DRAFT（草稿）──发布──→ PUBLISHED（已发布）──停用──→ DISABLED（已停用）
///       ↑                      │                        │
///       │                      └──回滚──→ PUBLISHED       │
///       │                                               │
///       └────────────── 重新编辑/新版本 ──────────────────┘
/// </code>
/// 版本管理通过 PR_RULE_VERSION 实现，主档只记录当前生效版本号。
/// </para>
/// <para>
/// 【规则冲突校验（发布前必须执行）】
/// <list type="bullet">
/// <item>同一项目、同一场景、同一生效期内不允许多套折价公式或不同折价额度规则同时生效</item>
/// <item>同一项目允许按不同部位维护不同换算规则（条件中部位不同即可）</item>
/// <item>多肿物、多部位、多面积项目必须使用 pricingParts 明细表达，不能压成单个数量粗算</item>
/// </list>
/// </para>
/// <para>
/// 【领域事件机制】
/// 聚合根在执行状态转换时收集领域事件到 DomainEvents 列表。
/// 应用服务在事务提交后遍历 DomainEvents 分发给事件处理器。
/// 当前实现状态：事件收集已完成，分发机制待后续迭代集成。
/// 缓存失效和审计日志仍由应用服务直接执行，待事件分发就绪后逐步迁移。
/// </para>
/// </remarks>
public sealed class RuleAggregate
{
    /// <summary>
    /// 领域事件收集列表，聚合根在状态转换时将事件追加到此列表。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 【设计目的】
    /// DDD 中聚合根不应直接调用基础设施（如缓存清除、日志写入），
    /// 而是通过领域事件声明"发生了什么"，由应用层/基础设施层的事件处理器决定"做什么"。
    /// </para>
    /// <para>
    /// 【使用方式】
    /// 1. 调用 Publish()/Disable()/Rollback() → 自动追加事件到 DomainEvents
    /// 2. 应用服务在事务提交后遍历 DomainEvents → 分发给处理器
    /// 3. 处理完成后清空 DomainEvents，避免重复处理
    /// </para>
    /// </remarks>
    public List<IDomainEvent> DomainEvents { get; } = new();

    /// <summary>
    /// 规则主键，对应 Oracle 列 RULE_ID，NUMBER 类型，由 SEQUENCE 生成。
    /// </summary>
    /// <remarks>
    /// 全局唯一标识一条规则，是关联规则头、版本、条件、动作和追溯结果的核心外键。
    /// 不可修改，由数据库在 Insert 时自动填充。
    /// </remarks>
    public long RuleId { get; set; }

    /// <summary>
    /// 规则编码，全局唯一的业务键。
    /// </summary>
    /// <remarks>
    /// 用于业务配置和运维识别，格式建议：RULE_ + 分类缩写 + 序号，如 RULE_DISCOUNT_001。
    /// 修改编码需评估所有引用该规则的追溯记录，不建议在发布后修改。
    /// </remarks>
    public string RuleCode { get; set; } = string.Empty;

    /// <summary>
    /// 规则名称，面向配置人员展示的中文名称。
    /// </summary>
    /// <remarks>
    /// 用于工作台展示和审计，例如："皮肤科治疗项目折价规则"。
    /// 不参与计价计算，仅用于可读性。
    /// </remarks>
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// 规则类别编码，标识该规则的业务类别。
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>DISCOUNT — 纯折价规则</item>
    /// <item>FORMULA — 纯公式规则</item>
    /// <item>LIMIT — 纯限额规则</item>
    /// <item>MIXED — 混合规则（同时包含折价、公式、限额）</item>
    /// </list>
    /// 默认值为 "MIXED"。该值应与 PR_DICT 中的 RULE_CATEGORY 字典对应。
    /// </remarks>
    public string RuleCategory { get; set; } = "MIXED";

    /// <summary>
    /// 规则作用范围，标识该规则作用于什么粒度。
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>ITEM — 单个项目，通过 ITEM_CODE 匹配</item>
    /// <item>GROUP — 项目组，通过 GROUP_CODE 匹配</item>
    /// <item>SCENE — 场景级，通过条件中的收费场景匹配</item>
    /// </list>
    /// 默认值为 "ITEM"。计价引擎按此字段决定规则匹配的入口维度。
    /// </remarks>
    public string RuleScope { get; set; } = "ITEM";

    /// <summary>
    /// 项目编码，对应 HIS 物价主数据表 FIN_COM_UNDRUGINFO.ITEM_CODE。
    /// </summary>
    /// <remarks>
    /// 当 RULE_SCOPE = "ITEM" 时，该字段指定规则针对哪个项目。
    /// 空值表示该规则不针对特定项目（如按项目组或场景匹配）。
    /// "折价项目"和"特殊项目"是同一标识，通过该编码区分是否为特殊计价项目。
    /// </remarks>
    public string? ItemCode { get; set; }

    /// <summary>
    /// 项目名称，来源为 HIS 物价主数据。
    /// </summary>
    /// <remarks>
    /// 非计算字段，仅用于工作台展示和审计可读性。
    /// </remarks>
    public string? ItemName { get; set; }

    /// <summary>
    /// 项目组编码，对应 PR_ITEM_GROUP.GROUP_CODE。
    /// </summary>
    /// <remarks>
    /// 当 RULE_SCOPE = "GROUP" 时，该字段指定规则针对哪个项目组。
    /// 空值表示该规则不针对特定项目组。
    /// </remarks>
    public string? GroupCode { get; set; }

    /// <summary>
    /// 规则优先级，数字越小优先级越高。
    /// </summary>
    /// <remarks>
    /// 当同一项目命中多条规则时，按优先级排序执行。
    /// 默认值为 100，建议间隔 10，便于后续插入中间优先级规则。
    /// </remarks>
    public int Priority { get; set; } = 100;

    /// <summary>
    /// 当前生效版本号，指向 PR_RULE_VERSION.VERSION_NO。
    /// </summary>
    /// <remarks>
    /// 由规则生命周期服务在发布或回滚时维护。
    /// 0 表示该规则尚未发布任何版本。
    /// 计价引擎读取此字段决定使用哪套版本的条件和动作。
    /// </remarks>
    public int CurrentVersion { get; set; }

    /// <summary>
    /// 规则状态，描述规则在生命周期状态机中的当前位置。
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>DRAFT — 草稿，正在编辑中</item>
    /// <item>PUBLISHED — 已发布，规则生效中，参与计价匹配</item>
    /// <item>DISABLED — 已停用，不再参与计价匹配</item>
    /// <item>ROLLED_BACK — 已回滚，恢复到之前版本</item>
    /// </list>
    /// 默认值为 "DRAFT"。状态转换必须通过 Publish()/Disable()/Rollback() 方法执行，
    /// 不允许外部直接赋值，否则状态机约束会被绕过。
    /// </remarks>
    public string Status { get; set; } = "DRAFT";

    /// <summary>
    /// 启用标识，"Y" 表示参与计价匹配，"N" 表示禁用。
    /// </summary>
    /// <remarks>
    /// 与 STATUS 的区别：STATUS 描述生命周期阶段，IS_ENABLED 描述是否参与匹配。
    /// 停用时 Disable() 方法会同时将 IS_ENABLED 设为 "N"，
    /// 确保生效规则查询（WHERE IS_ENABLED = 'Y'）自动排除已停用规则。
    /// 默认值为 "Y"。
    /// </remarks>
    public string IsEnabled { get; set; } = "Y";

    /// <summary>
    /// 规则生效开始时间，业务收费时间早于该时间的请求不匹配该规则。
    /// </summary>
    /// <remarks>
    /// 空值表示无生效开始时间限制（从创建时起生效）。
    /// 规则生效期判断必须使用业务收费发生时间（BusinessChargeTime），
    /// 不得使用技术占用时间替代。
    /// </remarks>
    public DateTime? EffectiveFrom { get; set; }

    /// <summary>
    /// 规则生效结束时间，业务收费时间晚于该时间的请求不匹配该规则。
    /// </summary>
    /// <remarks>
    /// 空值表示未设失效时间（长期有效）。
    /// </remarks>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// 回滚模式，决定计价服务不可用时的降级策略。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 当计价服务出现故障或超时时，各渠道根据此字段决定降级行为：
    /// <list type="bullet">
    /// <item><c>STOP_CHARGE</c> — 暂停收费转人工（默认，最安全）。适用于资金风险最高的项目。</item>
    /// <item><c>LEGACY_EQUIVALENT</c> — 自动切回旧计价逻辑（需审批）。适用于已验证等价性的项目。</item>
    /// <item><c>MANUAL_REVIEW</c> — 继续使用新服务但标记需人工复核。适用于金额较小或影响面低的项目。</item>
    /// </list>
    /// </para>
    /// <para>
    /// 资金安全约束：<c>LEGACY_EQUIVALENT</c> 必须经过审批流程，
    /// 且需要确认新旧逻辑在所有边界场景下的计算结果一致。
    /// 其他模式不允许自动回退到旧逻辑，必须转人工、暂停或继续使用新服务。
    /// </para>
    /// <para>
    /// 空值行为：NULL 等价于 <c>STOP_CHARGE</c>（最安全策略）。
    /// </para>
    /// </remarks>
    public string? RollbackMode { get; set; }

    /// <summary>
    /// 规则备注，配置人员或开发人员填写的补充说明。
    /// </summary>
    /// <remarks>
    /// 例如："该规则根据收费处 2026 年 5 月通知新增"。
    /// 不参与计价计算，仅用于可读性和审计追溯。
    /// </remarks>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建人，来源为工作台登录用户，用于审计。
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 记录创建时间，由计价中心自动填充，用于审计和排序。
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最后修改人，来源为工作台登录用户，用于审计。
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 记录最后更新时间，由计价中心在每次更新时自动填充，用于乐观锁和审计。
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 将规则状态从草稿推进为已发布，并发布领域事件。
    /// </summary>
    /// <param name="versionNo">发布的目标版本号，由应用服务传入。</param>
    /// <remarks>
    /// <para>
    /// 【前置条件】
    /// 只有 DRAFT 或 PUBLISHED 状态的规则允许发布：
    ///   - DRAFT → PUBLISHED：首次发布
    ///   - PUBLISHED → PUBLISHED：版本升级（新版本覆盖旧版本）
    /// DISABLED 状态的规则不允许直接发布，必须先创建新草稿版本。
    /// </para>
    /// <para>
    /// 【领域事件】
    /// 发布成功后追加 RulePublishedEvent 到 DomainEvents。
    /// 应用服务在事务提交后应遍历 DomainEvents 并分发给事件处理器，
    /// 处理器负责清除缓存和写入审计日志。
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">规则当前状态不允许发布时抛出。</exception>
    public void Publish(int versionNo)
    {
        // ========== 第一阶段：校验前置状态 ==========
        // 只有 DRAFT 和 PUBLISHED 允许发布。DISABLED 规则必须先创建新草稿版本，
        // 不能直接"重新发布"，否则发布历史会出现状态跳跃，审计追溯链断裂。
        if (Status != RuleStatusCodes.Draft && Status != RuleStatusCodes.Published)
            throw new InvalidOperationException($"规则 {RuleCode} 当前状态为 {Status}，不允许发布。");

        // ========== 第二阶段：推进状态和版本号 ==========
        // CurrentVersion 是计价引擎读取规则版本的入口。发布后必须立即更新，
        // 否则引擎会继续使用旧版本的条件和动作，导致计价结果与配置不一致。
        Status = RuleStatusCodes.Published;
        CurrentVersion = versionNo;
        UpdatedAt = DateTime.Now;

        // ========== 第三阶段：收集领域事件 ==========
        // 事件在事务提交后由应用服务分发。这里只收集，不直接执行副作用。
        // 如果直接调用缓存清除，聚合根就依赖了基础设施，违反 DDD 分层原则。
        DomainEvents.Add(new RulePublishedEvent(RuleId, versionNo, DateTime.Now));
    }

    /// <summary>
    /// 将规则状态推进为已停用，并发布领域事件。
    /// </summary>
    /// <param name="reason">停用原因，写入领域事件供审计日志使用。</param>
    /// <remarks>
    /// <para>
    /// 【前置条件】
    /// 已停用的规则不允许重复停用（幂等保护）。
    /// 草稿规则没有"停用"的业务含义，应通过删除草稿版本处理。
    /// </para>
    /// <para>
    /// 【副作用】
    /// 停用后 IsEnabled 设为 "N"，确保生效规则查询自动排除。
    /// 同时追加 RuleDisabledEvent，应用服务在事务提交后分发。
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">规则已是停用状态时抛出。</exception>
    public void Disable(string? reason = null)
    {
        // ========== 第一阶段：幂等校验 ==========
        // 已停用规则再次调用 Disable 不应抛异常（幂等），但当前实现选择抛异常
        // 以明确告知调用方"这条规则已经停用了，不需要再停"。
        if (Status == RuleStatusCodes.Disabled)
            throw new InvalidOperationException($"规则 {RuleCode} 已是停用状态。");

        // ========== 第二阶段：推进状态和启用标志 ==========
        // IS_ENABLED 必须同步设为 "N"。如果只改 STATUS 不改 IS_ENABLED，
        // 生效规则查询（WHERE IS_ENABLED = 'Y' AND STATUS = 'PUBLISHED'）
        // 仍然会匹配到这条规则，导致已停用规则继续参与计价。
        Status = RuleStatusCodes.Disabled;
        IsEnabled = EnableFlag.No;
        UpdatedAt = DateTime.Now;

        // ========== 第三阶段：收集领域事件 ==========
        // 停用事件携带停用原因，供审计日志处理器写入 PR_RULE_CHANGE_LOG。
        DomainEvents.Add(new RuleDisabledEvent(RuleId, reason ?? "管理员手动停用", DateTime.Now));
    }

    /// <summary>
    /// 回滚到指定版本号，将状态恢复为已发布。
    /// </summary>
    /// <param name="targetVersionNo">回滚目标版本号，由应用服务从历史版本中选取。</param>
    /// <remarks>
    /// <para>
    /// 【前置条件】
    /// 回滚只对已发布规则有效。应用服务在调用前必须校验：
    ///   - 规则状态为 PUBLISHED
    ///   - targetVersionNo 对应的版本存在且状态为 DISABLED（历史版本）
    /// </para>
    /// <para>
    /// 【为什么 Rollback 不校验前置状态】
    /// 与 Publish/Disable 不同，Rollback 的前置校验由应用服务执行，
    /// 因为"是否存在可回滚的历史版本"需要查询版本表，聚合根不应依赖仓储。
    /// 聚合根只负责"把状态和版本号推进到目标值"。
    /// </para>
    /// <para>
    /// 【领域事件】
    /// 回滚等同于"重新发布历史版本"，因此追加 RulePublishedEvent。
    /// 不需要单独的 RuleRolledBackEvent，因为回滚的缓存失效和审计逻辑
    /// 与发布相同——都是"让计价引擎读到新版本"。
    /// </para>
    /// </remarks>
    public void Rollback(int targetVersionNo)
    {
        // ========== 第一阶段：推进状态和版本号 ==========
        // 回滚后状态恢复为 PUBLISHED，CurrentVersion 切回目标版本号。
        // 计价引擎下次请求会读到回滚后的版本条件和动作。
        Status = RuleStatusCodes.Published;
        CurrentVersion = targetVersionNo;
        UpdatedAt = DateTime.Now;

        // ========== 第二阶段：收集领域事件 ==========
        // 回滚等同于重新发布历史版本，使用 RulePublishedEvent。
        // 审计日志的 ChangeType 字段由应用服务设为 "ROLLBACK"，
        // 不需要单独的事件类型来区分"首次发布"和"回滚发布"。
        DomainEvents.Add(new RulePublishedEvent(RuleId, targetVersionNo, DateTime.Now));
    }

    /// <summary>
    /// 判断规则在指定业务时间点是否生效。
    /// </summary>
    /// <param name="businessTime">业务收费发生时间，不得使用技术时间替代。</param>
    /// <returns>生效期内返回 true，否则返回 false。</returns>
    /// <remarks>
    /// <para>
    /// 【空值语义】
    /// EffectiveFrom 为空 = 从创建时起生效（无开始限制）。
    /// EffectiveTo 为空 = 长期有效（无结束限制）。
    /// 两个都为空 = 任何时间点都生效。
    /// </para>
    /// <para>
    /// 【业务时间优先】
    /// 参数必须是 BusinessChargeTime（调用方传入的业务收费时间），
    /// 不能用 DateTime.Now 或 RequestAt 替代。
    /// 否则规则生效判断会受服务器时钟漂移影响，导致计价结果不稳定。
    /// </para>
    /// </remarks>
    public bool IsEffectiveAt(DateTime businessTime)
    {
        if (EffectiveFrom.HasValue && businessTime < EffectiveFrom.Value)
            return false;
        if (EffectiveTo.HasValue && businessTime > EffectiveTo.Value)
            return false;
        return true;
    }

    /// <summary>
    /// 清空领域事件列表，通常在应用服务分发完事件后调用。
    /// </summary>
    /// <remarks>
    /// 防止同一批事件被重复分发。应用服务在事务提交后应：
    /// 1. 遍历 DomainEvents 分发给处理器
    /// 2. 调用 ClearDomainEvents() 清空列表
    /// </remarks>
    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}