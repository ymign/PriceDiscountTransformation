using Pricing.RuleCenter.Core.Models;

namespace Pricing.RuleCenter.Core.Interfaces;

/// <summary>
/// 项目组明细仓储接口 —— 规则维护中心的项目组明细数据访问层契约。
///
/// 架构位置：
///   位于核心层（Core），由基础设施层（Infrastructure）实现，应用层通过依赖注入使用。
///   对应 Oracle 表 PR_ITEM_GROUP_DETAIL。
///
/// 职责边界：
///   - 管理项目组内包含的收费项目（增删查）。
///   - 支持按项目组查询成员列表、按项目编码查询所属组。
///   - 支持批量插入和按组删除，便于项目组成员的整体维护。
///
/// 主子项目关系：
///   - MAIN — 主项目，收费时触发子项目加收
///   - CHILD — 子项目，随主项目一起收费，必须与主项目同 resultGroupNo 原子 commit/cancel
///
/// 与项目组（PR_ITEM_GROUP）的关系：
///   - PR_ITEM_GROUP 保存组的元数据（编码、名称、类型）。
///   - PR_ITEM_GROUP_DETAIL 保存组内包含的项目列表。
///   - 两者是一对多关系：一个组包含多个项目明细。
/// </summary>
public interface IItemGroupDetailRepository
{
    /// <summary>
    /// 查询指定项目组的所有明细记录。
    ///
    /// 使用场景：项目组详情页展示成员列表、计价引擎按组匹配项目。
    /// </summary>
    /// <param name="groupId">项目组主键（PR_ITEM_GROUP.GROUP_ID）。</param>
    /// <returns>项目组明细列表。</returns>
    Task<IReadOnlyList<ItemGroupDetail>> GetByGroupIdAsync(long groupId);

    /// <summary>
    /// 查询指定项目编码所属的所有项目组明细。
    ///
    /// 使用场景：判断某项目是否属于某个项目组类型，或查找项目所属的所有组。
    /// 一个项目可能同时属于多个不同类型的项目组。
    /// </summary>
    /// <param name="itemCode">项目编码（HIS 物价主数据 ITEM_CODE）。</param>
    /// <returns>包含该项目的所有项目组明细列表。</returns>
    Task<IReadOnlyList<ItemGroupDetail>> GetByItemCodeAsync(string itemCode);

    /// <summary>
    /// 批量插入项目组明细记录。
    ///
    /// 调用时机：配置人员批量添加项目组成员时调用。
    /// 批量插入使用事务保证原子性，全部成功或全部回滚。
    /// </summary>
    /// <param name="entities">
    /// 项目组明细实体列表，每个实体包含：
    /// - GroupId：所属项目组ID
    /// - ItemCode：项目编码
    /// - ItemName：项目名称（可选）
    /// - RoleType：角色类型（MAIN/CHILD）
    /// - SortNo：排序号
    /// </param>
    Task InsertBatchAsync(IReadOnlyList<ItemGroupDetail> entities);

    /// <summary>
    /// 删除指定项目组下的所有明细记录。
    ///
    /// 调用时机：配置人员清空项目组成员或删除项目组时调用。
    /// 注意：删除前应确认是否有规则引用了该项目组，避免规则匹配失效。
    /// </summary>
    /// <param name="groupId">项目组主键（PR_ITEM_GROUP.GROUP_ID）。</param>
    Task DeleteByGroupIdAsync(long groupId);
}
