namespace Pricing.RuleCenter.Core.Constants;

/// <summary>
/// 规则状态常量类，提供与数据库存储格式一致的字符串常量。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// PR_RULE_HEADER 表的 Status 字段使用字符串存储（如 "DRAFT"、"PUBLISHED"），
/// 无法直接使用 C# 枚举。本类提供常量字符串，统一代码中的状态引用，避免魔术字符串散落各处。
/// </para>
/// <para>
/// 【与枚举的关系】
/// 常量值与 Core/Enums/RuleStatus.cs 枚举名称一一对应。
/// 如需转换：RuleStatus.Draft.ToString().ToUpperInvariant() == RuleStatusCodes.Draft。
/// </para>
/// <para>
/// 【状态机路径】
/// Draft（草稿）→ Published（已发布）→ Disabled（已停用）
/// </para>
/// </remarks>
public static class RuleStatusCodes
{
    /// <summary>
    /// 草稿状态：规则正在编辑中，不参与计价引擎匹配。
    /// </summary>
    public const string Draft = "DRAFT";

    /// <summary>
    /// 已发布状态：规则正式生效，参与计价引擎的规则匹配。
    /// </summary>
    public const string Published = "PUBLISHED";

    /// <summary>
    /// 已停用状态：规则不再参与计价引擎匹配。
    /// </summary>
    public const string Disabled = "DISABLED";
}

/// <summary>
/// 版本状态常量类，提供与数据库存储格式一致的字符串常量。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// PR_RULE_VERSION 表的 VersionStatus 字段使用字符串存储。
/// 本类提供常量字符串，统一代码中的状态引用。
/// </para>
/// <para>
/// 【状态机路径】
/// Draft（草稿）→ Published（已发布）→ Disabled（已停用）
///      ↘ RolledBack（已回滚）
/// </para>
/// </remarks>
public static class VersionStatusCodes
{
    /// <summary>
    /// 草稿状态：版本正在编辑中，不参与计价引擎匹配。
    /// </summary>
    public const string Draft = "DRAFT";

    /// <summary>
    /// 已发布状态：版本正式生效，参与计价引擎匹配。
    /// </summary>
    public const string Published = "PUBLISHED";

    /// <summary>
    /// 已停用状态：版本被停用，不再参与计价引擎匹配。
    /// </summary>
    public const string Disabled = "DISABLED";

    /// <summary>
    /// 已回滚状态：版本被回滚，不再参与计价引擎匹配。
    /// </summary>
    public const string RolledBack = "ROLLED_BACK";
}

/// <summary>
/// 启用标识常量类，提供与数据库存储格式一致的字符串常量。
/// </summary>
/// <remarks>
/// 【设计目的】
/// 多张表（PR_RULE_HEADER、PR_DICT、PR_FORMULA_DEF 等）的 IsEnabled 字段
/// 使用 "Y"/"N" 字符串存储。本类提供常量，统一代码中的引用。
/// </remarks>
public static class EnableFlag
{
    /// <summary>
    /// 启用标识：记录处于启用状态，参与业务逻辑。
    /// </summary>
    public const string Yes = "Y";

    /// <summary>
    /// 禁用标识：记录处于禁用状态，不参与业务逻辑。
    /// </summary>
    public const string No = "N";
}

/// <summary>
/// 业务状态常量类，用于计价请求的生命周期状态。
/// </summary>
/// <remarks>
/// <para>
/// 【设计目的】
/// PR_CHARGE_REQUEST_LOG 表的 BusinessStatus 字段使用字符串存储。
/// 本类提供常量字符串，统一代码中的状态引用。
/// </para>
/// <para>
/// 【状态机路径】
/// SIMULATED（试算）→ CONFIRM_PENDING（待确认）→ CONFIRMED（已确认）→ COMMITTED（已落账）
///      ↘ CANCELLED（已取消）
///      ↘ EXPIRED（已过期）
///      ↘ REVERSED（已冲正）
/// </para>
/// </remarks>
public static class BusinessStatusCodes
{
    /// <summary>
    /// 试算状态：仅模拟计算，不占用额度。
    /// </summary>
    public const string Simulated = "SIMULATED";

    /// <summary>
    /// 待确认状态：confirm 调用后，额度已锁定等待 HIS 落账。
    /// </summary>
    public const string ConfirmPending = "CONFIRM_PENDING";

    /// <summary>
    /// 已确认状态：HIS commit 后确认，额度正式扣减。
    /// </summary>
    public const string Confirmed = "CONFIRMED";

    /// <summary>
    /// 已落账状态：HIS 结算成功，请求完成。
    /// </summary>
    public const string Committed = "COMMITTED";

    /// <summary>
    /// 已取消状态：cancel 调用后释放额度。
    /// </summary>
    public const string Cancelled = "CANCELLED";

    /// <summary>
    /// 已过期状态：后台清理任务将超时的 PENDING 记录自动过期。
    /// </summary>
    public const string Expired = "EXPIRED";

    /// <summary>
    /// 已冲正状态：reverse（退费）后释放额度。
    /// </summary>
    public const string Reversed = "REVERSED";
}

/// <summary>
/// 占用状态常量类，用于额度占用记录的生命周期状态。
/// </summary>
/// <remarks>
/// 【设计目的】
/// PR_LIMIT_OCCUPY 表的 Status 字段使用字符串存储。
/// 常量值与 Core/Enums/OccupyStatus.cs 枚举名称对应。
/// </remarks>
public static class OccupyStatusCodes
{
    /// <summary>
    /// 待确认状态：confirm 调用后创建的占用记录，额度已锁定。
    /// </summary>
    public const string Pending = "PENDING";

    /// <summary>
    /// 已确认状态：HIS commit 后确认，额度正式扣减。
    /// </summary>
    public const string Confirmed = "CONFIRMED";

    /// <summary>
    /// 已取消状态：cancel 调用后释放的占用记录。
    /// </summary>
    public const string Cancelled = "CANCELLED";

    /// <summary>
    /// 已冲正状态：reverse 后释放的占用记录。
    /// </summary>
    public const string Reversed = "REVERSED";

    /// <summary>
    /// 已过期状态：后台清理任务自动过期的占用记录。
    /// </summary>
    public const string Expired = "EXPIRED";
}