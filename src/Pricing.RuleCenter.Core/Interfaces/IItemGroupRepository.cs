using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 项目组仓储接口 —— 规则维护中心的项目组数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_ITEM_GROUP。
///
/// 职责边界：
///   - 管理项目组的增删改查。
///   - 支持按主键查询、按类型查询。
///   - 项目组是规则作用范围（RULE_SCOPE = "GROUP"）的匹配目标。
///
/// 项目组类型说明：
///   - MAIN_CHILD — 主子项目组，主项目收费时自动加收子项目
///   - MUTUAL_EXCLUSIVE — 同类互斥项目组，同组项目只执行优先级最高的一条
///   - SAME_OPERATION — 同手术项目组，同一手术下的多个项目共享限额
///
/// 与项目组明细（PR_ITEM_GROUP_DETAIL）的关系：
///   - PR_ITEM_GROUP 保存组的元数据（编码、名称、类型）。
///   - PR_ITEM_GROUP_DETAIL 保存组内包含的项目列表。
///   - 两者是一对多关系：一个组包含多个项目明细。
/// </summary>
public interface IItemGroupRepository
{
    /// <summary>
    /// 按主键查询单个项目组。
    ///
    /// 使用场景：项目组详情页回显数据、编辑时校验是否存在。
    /// </summary>
    /// <param name="groupId">项目组主键（PR_ITEM_GROUP.GROUP_ID）。</param>
    /// <returns>项目组实体，不存在时返回 null。</returns>
    Task<ItemGroup?> GetByIdAsync(long groupId);

    /// <summary>
    /// 按项目组类型查询项目组列表。
    ///
    /// 使用场景：工作台按类型筛选项目组，如下拉列表展示所有互斥组。
    /// </summary>
    /// <param name="groupType">
    /// 项目组类型编码（MAIN_CHILD / MUTUAL_EXCLUSIVE / SAME_OPERATION）。
    /// </param>
    /// <returns>指定类型的项目组列表。</returns>
    Task<IReadOnlyList<ItemGroup>> GetByTypeAsync(string groupType);

    /// <summary>
    /// 插入一条项目组记录，返回自增主键。
    ///
    /// 调用时机：配置人员创建项目组时写入。
    /// </summary>
    /// <param name="entity">
    /// 项目组实体，包含：
    /// - GroupCode：项目组编码（全局唯一业务键）
    /// - GroupName：项目组名称
    /// - GroupType：项目组类型
    /// - CreatedBy：创建人
    /// </param>
    /// <returns>新插入记录的主键ID。</returns>
    Task<long> InsertAsync(ItemGroup entity);

    /// <summary>
    /// 按项目组编码查询单个项目组。
    ///
    /// 使用场景：计价引擎按条件配置的项目组编码查找项目组，再进一步判断项目是否属于该组。
    /// GroupCode 是全局唯一的业务键，查询效率等同于按主键查询。
    /// </summary>
    /// <param name="groupCode">项目组编码（PR_ITEM_GROUP.GROUP_CODE）。</param>
    /// <returns>项目组实体，不存在时返回 null。</returns>
    Task<ItemGroup?> GetByCodeAsync(string groupCode);

    /// <summary>
    /// 更新一条项目组记录。
    ///
    /// 调用时机：配置人员编辑项目组信息时调用。
    /// 注意：更新时应同步更新 UpdatedAt 和 UpdatedBy 字段。
    /// </summary>
    /// <param name="entity">
    /// 项目组实体，包含待更新的字段。
    /// 必须包含 GroupId 作为更新条件。
    /// </param>
    Task UpdateAsync(ItemGroup entity);
}
