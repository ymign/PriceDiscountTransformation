using System;
using System.Collections.Generic;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 通用分页响应容器。
    /// 服务端返回列表数据时统一使用此结构包裹，HIS 客户端据此渲染分页网格。
    /// </summary>
    /// <typeparam name="T">列表元素的 DTO 类型</typeparam>
    public sealed class PagedResponse<T>
    {
        /// <summary>当前页数据列表，服务端无数据时返回空列表而非 null</summary>
        public List<T> Items { get; set; }

        /// <summary>满足筛选条件的记录总数（非当前页数量），用于前端计算总页数</summary>
        public int Total { get; set; }

        /// <summary>当前页码，从 1 开始，与服务端约定保持一致</summary>
        public int PageIndex { get; set; }

        /// <summary>每页条数，由请求方传入，服务端透传回来</summary>
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 规则头（PR_RULE_HEADER）的查询响应 DTO。
    /// 对应规则维护工作台左侧列表的一行数据，包含规则的基本信息、生命周期状态和审计字段。
    /// </summary>
    public sealed class RuleHeaderResponse
    {
        /// <summary>规则主键 ID（Oracle SEQUENCE 生成）</summary>
        public long RuleId { get; set; }

        /// <summary>规则编码，全局唯一，如 "RULE_DISCOUNT_CT_001"；新建时由工作台自动生成</summary>
        public string RuleCode { get; set; }

        /// <summary>规则名称，供人工识别，无唯一性约束</summary>
        public string RuleName { get; set; }

        /// <summary>规则类别，取自字典表 PR_DICT 的 RULE_CATEGORY 类型，如 "DISCOUNT"、"FORMULA"</summary>
        public string RuleCategory { get; set; }

        /// <summary>规则作用范围，取自字典表 PR_DICT 的 RULE_SCOPE 类型，如 "ITEM"、"GROUP"、"SCENE"</summary>
        public string RuleScope { get; set; }

        /// <summary>
        /// 适用项目编码。非空时表示此规则仅针对特定收费项目；
        /// 空时表示全局规则（适用于所有项目）。
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>适用项目名称（冗余字段，仅用于展示，不参与匹配逻辑）</summary>
        public string ItemName { get; set; }

        /// <summary>项目分组编码，关联 PR_ITEM_GROUP；用于"同组互斥"等批量约束场景</summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// 优先级，数值越小越优先匹配。
        /// 同一项目多条规则共存时，引擎按 Priority 升序尝试匹配，首个命中的规则生效。
        /// </summary>
        public int Priority { get; set; }

        /// <summary>当前已发布的版本号；若从未发布则为 0</summary>
        public int CurrentVersion { get; set; }

        /// <summary>规则生命周期状态，取自字典表 RULE_STATUS，如 "DRAFT"、"PUBLISHED"、"DISABLED"</summary>
        public string Status { get; set; }

        /// <summary>
        /// 是否启用标记，"Y"/"N" 字符串（Oracle 无原生布尔类型）。
        /// "N" 表示即使规则已发布，引擎也不会匹配此规则。
        /// </summary>
        public string IsEnabled { get; set; }

        /// <summary>生效起始时间，NULL 表示不限制开始时间</summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>生效截止时间，NULL 表示不限制结束时间（永久有效）</summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// 回滚模式。控制计价服务不可用时是否允许切回旧逻辑；
        /// 空值等价于 STOP_CHARGE，LEGACY_EQUIVALENT 必须经过等价性确认。
        /// </summary>
        public string RollbackMode { get; set; }

        /// <summary>备注信息，由维护人员填写，不影响计价逻辑</summary>
        public string Remark { get; set; }

        /// <summary>创建人工号，记录首次建立此规则的操作者</summary>
        public string CreatedBy { get; set; }

        /// <summary>创建时间（服务端写入，客户端只读展示）</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>最后修改人工号</summary>
        public string UpdatedBy { get; set; }

        /// <summary>最后修改时间（服务端写入，客户端只读展示）</summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// 新建规则头请求 DTO。
    /// 由规则维护工作台在"新建规则"时组装，发送至 POST /api/pricing/rules。
    /// </summary>
    public sealed class RuleHeaderCreateRequest
    {
        /// <summary>规则编码，建议格式 "RULE_类别_序号"，服务端校验唯一性</summary>
        public string RuleCode { get; set; }

        /// <summary>规则名称</summary>
        public string RuleName { get; set; }

        /// <summary>规则类别编码（字典值）</summary>
        public string RuleCategory { get; set; }

        /// <summary>规则作用范围编码（字典值）</summary>
        public string RuleScope { get; set; }

        /// <summary>适用项目编码，NULL 表示全局适用</summary>
        public string ItemCode { get; set; }

        /// <summary>适用项目名称（冗余展示用）</summary>
        public string ItemName { get; set; }

        /// <summary>项目分组编码</summary>
        public string GroupCode { get; set; }

        /// <summary>优先级，默认 100</summary>
        public int Priority { get; set; }

        /// <summary>生效起始时间，NULL 表示不限</summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>生效截止时间，NULL 表示不限</summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>回滚模式：STOP_CHARGE、LEGACY_EQUIVALENT、MANUAL_REVIEW</summary>
        public string RollbackMode { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }

        /// <summary>创建人工号</summary>
        public string CreatedBy { get; set; }
    }

    /// <summary>
    /// 更新规则头请求 DTO。
    /// 由规则维护工作台在"保存规则"（已有规则修改模式）时组装，发送至 PUT /api/pricing/rules/{id}。
    /// 不含 RuleCode，因为规则编码不允许修改。
    /// </summary>
    public sealed class RuleHeaderUpdateRequest
    {
        /// <summary>规则名称</summary>
        public string RuleName { get; set; }

        /// <summary>规则类别编码</summary>
        public string RuleCategory { get; set; }

        /// <summary>规则作用范围编码</summary>
        public string RuleScope { get; set; }

        /// <summary>适用项目编码</summary>
        public string ItemCode { get; set; }

        /// <summary>适用项目名称</summary>
        public string ItemName { get; set; }

        /// <summary>项目分组编码</summary>
        public string GroupCode { get; set; }

        /// <summary>优先级</summary>
        public int Priority { get; set; }

        /// <summary>生效起始时间</summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>生效截止时间</summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>回滚模式：STOP_CHARGE、LEGACY_EQUIVALENT、MANUAL_REVIEW</summary>
        public string RollbackMode { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }

        /// <summary>修改人工号</summary>
        public string UpdatedBy { get; set; }
    }

    /// <summary>
    /// 规则版本（PR_RULE_VERSION）响应 DTO。
    /// 每次规则变更创建新版本，版本状态流转：DRAFT -> PUBLISHED -> DISABLED。
    /// 发布时会生成 RuleSnapshot（规则快照 JSON），确保已发布版本不可变。
    /// </summary>
    public sealed class RuleVersionResponse
    {
        /// <summary>版本主键 ID</summary>
        public long VersionId { get; set; }

        /// <summary>所属规则 ID</summary>
        public long RuleId { get; set; }

        /// <summary>版本号，同一规则内从 1 递增</summary>
        public int VersionNo { get; set; }

        /// <summary>版本状态："DRAFT"（草稿，可编辑）、"PUBLISHED"（已发布，不可变）、"DISABLED"（已停用）</summary>
        public string VersionStatus { get; set; }

        /// <summary>该版本的生效起始时间（从规则头继承）</summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>该版本的生效截止时间</summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// 规则快照 JSON。发布时由服务端生成，包含该版本的完整规则定义（条件 + 动作），
        /// 用于计价引擎在不联表查询的情况下直接加载规则。
        /// </summary>
        public string RuleSnapshot { get; set; }

        /// <summary>发布人工号</summary>
        public string PublishedBy { get; set; }

        /// <summary>发布时间（仅已发布版本有值）</summary>
        public DateTime? PublishedAt { get; set; }

        /// <summary>发布备注（如"首次发布"、"调整折价比例"等）</summary>
        public string PublishRemark { get; set; }
    }

    /// <summary>
    /// 规则条件（PR_RULE_CONDITION）响应 DTO。
    /// 条件是规则匹配的前提，引擎按 ConditionGroup 分组（OR 关系），
    /// 组内条件按 AND 关系求值。全部条件满足时规则才命中。
    /// </summary>
    public sealed class RuleConditionResponse
    {
        /// <summary>条件主键 ID</summary>
        public long ConditionId { get; set; }

        /// <summary>所属规则 ID</summary>
        public long RuleId { get; set; }

        /// <summary>所属版本号</summary>
        public int VersionNo { get; set; }

        /// <summary>
        /// 条件分组标识。同一分组内的条件是 AND 关系，不同分组之间是 OR 关系。
        /// 例如 "GRP_1" 表示第一组条件。
        /// </summary>
        public string ConditionGroup { get; set; }

        /// <summary>
        /// 条件类型，如 "ITEM_CODE"（项目编码匹配）、"BODY_PART"（部位匹配）、
        /// "CHARGE_SCENE"（收费场景匹配）。引擎据此选择对应的条件求值器。
        /// </summary>
        public string ConditionType { get; set; }

        /// <summary>
        /// 比较操作符，如 "EQUALS"、"IN"、"NOT_IN"、"LIKE"、"BETWEEN"。
        /// 由条件求值器解析执行。
        /// </summary>
        public string OperatorType { get; set; }

        /// <summary>左侧比较键（通常是请求上下文中的字段名，如 "itemCode"、"bodyPartCode"）</summary>
        public string LeftKey { get; set; }

        /// <summary>
        /// 右侧比较值。当 OperatorType 为 "IN" 时，值为逗号分隔的多个编码。
        /// 该字段以字符串存储，条件求值器内部做类型转换。
        /// </summary>
        public string RightValue { get; set; }

        /// <summary>
        /// 扩展参数 JSON。部分复杂条件类型需要额外参数（如日期范围、正则表达式等），
        /// 统一以 JSON CLOB 存储，由具体求值器反序列化使用。
        /// </summary>
        public string ParamsJson { get; set; }

        /// <summary>排序号，用于条件展示顺序和求值顺序</summary>
        public int SortNo { get; set; }

        /// <summary>是否启用，"Y"/"N"；禁用的条件在引擎求值时被跳过</summary>
        public string IsEnabled { get; set; }
    }

    /// <summary>
    /// 规则条件编辑行请求 DTO（单条）。
    /// 工作台条件网格的每一行对应一个实例，保存时由 RuleConditionSaveRequest 批量提交。
    /// </summary>
    public sealed class RuleConditionItemRequest
    {
        /// <summary>条件分组标识</summary>
        public string ConditionGroup { get; set; }

        /// <summary>条件类型</summary>
        public string ConditionType { get; set; }

        /// <summary>比较操作符</summary>
        public string OperatorType { get; set; }

        /// <summary>左侧比较键</summary>
        public string LeftKey { get; set; }

        /// <summary>右侧比较值</summary>
        public string RightValue { get; set; }

        /// <summary>扩展参数 JSON</summary>
        public string ParamsJson { get; set; }

        /// <summary>排序号</summary>
        public int SortNo { get; set; }

        /// <summary>是否启用</summary>
        public string IsEnabled { get; set; }
    }

    /// <summary>
    /// 规则条件批量保存请求 DTO。
    /// 发送至 PUT /api/pricing/rules/{ruleId}/versions/{versionNo}/conditions，
    /// 服务端以全量覆盖方式替换该版本的所有条件。
    /// </summary>
    public sealed class RuleConditionSaveRequest
    {
        /// <summary>条件列表（全量，非增量）</summary>
        public List<RuleConditionItemRequest> Conditions { get; set; }
    }

    /// <summary>
    /// 规则动作（PR_RULE_ACTION）响应 DTO。
    /// 动作定义了规则命中后执行的计价操作，如折价、加收、限额校验等。
    /// 一个规则可有多个动作，按 SortNo 顺序执行。
    /// </summary>
    public sealed class RuleActionResponse
    {
        /// <summary>动作主键 ID</summary>
        public long ActionId { get; set; }

        /// <summary>所属规则 ID</summary>
        public long RuleId { get; set; }

        /// <summary>所属版本号</summary>
        public int VersionNo { get; set; }

        /// <summary>
        /// 动作类型，如 "DISCOUNT"（折价）、"SURCHARGE"（加收）、"LIMIT_QTY"（数量限制）、
        /// "LIMIT_AMOUNT"（金额限制）、"FORMULA"（公式计算）、"MUTEX"（互斥校验）。
        /// 引擎据此选择对应的动作执行器。
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// 执行器编码，标识具体执行器实现。
        /// 如 "EXEC_DISCOUNT_PERCENT"（百分比折价）、"EXEC_DISCOUNT_FIXED"（固定金额折价）。
        /// </summary>
        public string ExecutorCode { get; set; }

        /// <summary>
        /// 执行器参数 JSON。不同 ExecutorCode 需要不同的参数结构，
        /// 如百分比折价需要 {"percent": 80}，固定金额折价需要 {"fixedAmount": 50.00}。
        /// </summary>
        public string ParamsJson { get; set; }

        /// <summary>
        /// 互斥组标识。同一 ExclusiveGroup 下的动作互斥，只执行第一个命中的。
        /// 空表示不参与互斥。用于"同手术封顶"等业务约束。
        /// </summary>
        public string ExclusiveGroup { get; set; }

        /// <summary>排序号，决定同一规则内多个动作的执行顺序</summary>
        public int SortNo { get; set; }

        /// <summary>
        /// 出错策略，如 "STOP"（中断后续动作）、"SKIP"（跳过当前动作继续）、"FALLBACK"（回退旧逻辑）。
        /// 防止某个动作执行失败时影响整条规则链。
        /// </summary>
        public string OnError { get; set; }

        /// <summary>是否启用，"Y"/"N"</summary>
        public string IsEnabled { get; set; }
    }

    /// <summary>
    /// 规则动作编辑行请求 DTO（单条）。
    /// 工作台动作网格的每一行对应一个实例。
    /// </summary>
    public sealed class RuleActionItemRequest
    {
        /// <summary>动作类型</summary>
        public string ActionType { get; set; }

        /// <summary>执行器编码</summary>
        public string ExecutorCode { get; set; }

        /// <summary>执行器参数 JSON</summary>
        public string ParamsJson { get; set; }

        /// <summary>互斥组标识</summary>
        public string ExclusiveGroup { get; set; }

        /// <summary>排序号</summary>
        public int SortNo { get; set; }

        /// <summary>出错策略</summary>
        public string OnError { get; set; }

        /// <summary>是否启用</summary>
        public string IsEnabled { get; set; }
    }

    /// <summary>
    /// 规则动作批量保存请求 DTO。
    /// 发送至 PUT /api/pricing/rules/{ruleId}/versions/{versionNo}/actions，
    /// 服务端以全量覆盖方式替换该版本的所有动作。
    /// </summary>
    public sealed class RuleActionSaveRequest
    {
        /// <summary>动作列表（全量，非增量）</summary>
        public List<RuleActionItemRequest> Actions { get; set; }
    }

    /// <summary>
    /// 规则发布请求 DTO。
    /// 发布操作将指定草稿版本升级为 PUBLISHED 状态，服务端会：
    /// 1. 校验规则完整性（条件、动作不为空等）
    /// 2. 校验规则冲突（同项目同场景同生效期不允许多套冲突规则）
    /// 3. 生成规则快照 JSON
    /// 4. 立即失效相关缓存
    /// </summary>
    public sealed class RulePublishRequest
    {
        /// <summary>要发布的版本号</summary>
        public int VersionNo { get; set; }

        /// <summary>发布人工号</summary>
        public string PublishedBy { get; set; }

        /// <summary>发布备注（如"首次发布"、"调整 CT 折价比例至 80%"）</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 规则停用请求 DTO。
    /// 停用操作将规则状态置为 DISABLED，引擎不再匹配此规则。
    /// 服务端同时失效缓存并记录变更日志。
    /// </summary>
    public sealed class RuleDisableRequest
    {
        /// <summary>操作人工号</summary>
        public string PublishedBy { get; set; }

        /// <summary>停用原因</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 规则回滚请求 DTO。
    /// 回滚操作将规则回退到上一个已发布版本；若无历史版本则置为 DRAFT 状态。
    /// 用于紧急修复错误发布的规则。
    /// </summary>
    public sealed class RuleRollbackRequest
    {
        /// <summary>操作人工号</summary>
        public string PublishedBy { get; set; }

        /// <summary>回滚原因</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 规则发布响应 DTO。
    /// 发布成功后返回本次发布的详细信息，可用于审计追溯。
    /// </summary>
    public sealed class RulePublishResponse
    {
        /// <summary>发布记录主键 ID</summary>
        public long PublishId { get; set; }

        /// <summary>发布批次号，服务端生成的唯一编号，用于关联同一次发布操作</summary>
        public string PublishNo { get; set; }

        /// <summary>规则 ID</summary>
        public long RuleId { get; set; }

        /// <summary>发布前版本号，首次发布时为 NULL</summary>
        public int? FromVersion { get; set; }

        /// <summary>发布后版本号</summary>
        public int ToVersion { get; set; }

        /// <summary>
        /// 发布动作类型，如 "PUBLISH"（首次发布）、"REPUBLISH"（重新发布）、
        /// "DISABLE"（停用）、"ROLLBACK"（回滚）。
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>操作人工号</summary>
        public string PublishedBy { get; set; }

        /// <summary>操作时间</summary>
        public DateTime PublishedAt { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 规则变更日志（PR_RULE_CHANGE_LOG）响应 DTO。
    /// 记录规则的每一次变更，支持"规则变更链"追溯——
    /// 即审计人员可以按时间线查看规则从创建到当前状态的全部变更历史。
    /// </summary>
    public sealed class RuleChangeLogResponse
    {
        /// <summary>变更日志主键 ID</summary>
        public long ChangeId { get; set; }

        /// <summary>规则 ID</summary>
        public long RuleId { get; set; }

        /// <summary>涉及的版本号，部分变更（如停用）可能不涉及特定版本，此时为 NULL</summary>
        public int? VersionNo { get; set; }

        /// <summary>
        /// 变更类型，如 "CREATE"（新建）、"UPDATE"（修改）、"PUBLISH"（发布）、
        /// "DISABLE"（停用）、"ROLLBACK"（回滚）。
        /// </summary>
        public string ChangeType { get; set; }

        /// <summary>变更摘要，由服务端自动生成的人类可读描述</summary>
        public string ChangeSummary { get; set; }

        /// <summary>操作人工号</summary>
        public string ChangedBy { get; set; }

        /// <summary>变更时间</summary>
        public DateTime ChangedAt { get; set; }

        /// <summary>
        /// 变更来源系统，如 "HIS_WORKBENCH"（HIS 工作台）、"API"（外部接口调用）。
        /// 用于区分变更是人工操作还是系统自动触发。
        /// </summary>
        public string SourceSystem { get; set; }
    }

    /// <summary>
    /// 字典项（PR_DICT）响应 DTO。
    /// 字典表存储系统枚举值的可配置映射，如规则类别、规则状态、收费场景等。
    /// 使用字典而非硬编码枚举，是为了支持不重新部署的情况下扩展枚举值。
    /// </summary>
    public sealed class DictResponse
    {
        /// <summary>字典项主键 ID</summary>
        public long DictId { get; set; }

        /// <summary>字典类型，如 "RULE_CATEGORY"、"RULE_STATUS"、"CHARGE_SCENE"</summary>
        public string DictType { get; set; }

        /// <summary>字典编码（同类型内唯一），如 "DISCOUNT"、"PUBLISHED"</summary>
        public string DictCode { get; set; }

        /// <summary>字典名称（展示用），如 "折价规则"、"已发布"</summary>
        public string DictName { get; set; }

        /// <summary>父级编码，用于树形字典；非树形字典此字段为 NULL</summary>
        public string ParentCode { get; set; }

        /// <summary>排序号，用于展示顺序</summary>
        public int SortNo { get; set; }

        /// <summary>是否启用，"Y"/"N"</summary>
        public string IsEnabled { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 新建字典项请求 DTO。
    /// </summary>
    public sealed class DictCreateRequest
    {
        /// <summary>字典类型</summary>
        public string DictType { get; set; }

        /// <summary>字典编码</summary>
        public string DictCode { get; set; }

        /// <summary>字典名称</summary>
        public string DictName { get; set; }

        /// <summary>父级编码</summary>
        public string ParentCode { get; set; }

        /// <summary>排序号</summary>
        public int SortNo { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 更新字典项请求 DTO。DictType 和 DictCode 不可修改。
    /// </summary>
    public sealed class DictUpdateRequest
    {
        /// <summary>字典名称</summary>
        public string DictName { get; set; }

        /// <summary>父级编码</summary>
        public string ParentCode { get; set; }

        /// <summary>排序号</summary>
        public int SortNo { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 计价公式定义（PR_FORMULA_DEF）响应 DTO。
    /// 公式定义了可复用的计价计算逻辑模板，规则动作通过 ExecutorCode 引用具体公式执行器。
    /// ParamSchemaJson 定义了该公式需要的参数结构，前端据此动态渲染参数编辑表单。
    /// </summary>
    public sealed class FormulaDefResponse
    {
        /// <summary>公式主键 ID</summary>
        public long FormulaId { get; set; }

        /// <summary>公式编码，全局唯一，如 "FORMULA_AREA_PRICE"（面积计价公式）</summary>
        public string FormulaCode { get; set; }

        /// <summary>公式名称（展示用），如 "按面积计价"</summary>
        public string FormulaName { get; set; }

        /// <summary>公式说明，描述公式适用场景和计算规则</summary>
        public string FormulaDesc { get; set; }

        /// <summary>
        /// 执行器编码，与规则动作的 ExecutorCode 对应。
        /// 引擎在执行公式动作时，根据此编码找到具体的公式执行器实现。
        /// </summary>
        public string ExecutorCode { get; set; }

        /// <summary>
        /// 参数结构定义 JSON（JSON Schema 或简化结构）。
        /// 描述该公式需要哪些参数、参数类型、默认值等。
        /// 例如面积计价公式可能需要 {"area": "decimal", "unitPrice": "decimal", "discountRate": "decimal??"}。
        /// </summary>
        public string ParamSchemaJson { get; set; }

        /// <summary>是否启用，"Y"/"N"</summary>
        public string IsEnabled { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 新建公式定义请求 DTO。
    /// </summary>
    public sealed class FormulaDefCreateRequest
    {
        /// <summary>公式编码</summary>
        public string FormulaCode { get; set; }

        /// <summary>公式名称</summary>
        public string FormulaName { get; set; }

        /// <summary>公式说明</summary>
        public string FormulaDesc { get; set; }

        /// <summary>执行器编码</summary>
        public string ExecutorCode { get; set; }

        /// <summary>参数结构定义 JSON</summary>
        public string ParamSchemaJson { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 更新公式定义请求 DTO。FormulaCode 不可修改。
    /// </summary>
    public sealed class FormulaDefUpdateRequest
    {
        /// <summary>公式名称</summary>
        public string FormulaName { get; set; }

        /// <summary>公式说明</summary>
        public string FormulaDesc { get; set; }

        /// <summary>执行器编码</summary>
        public string ExecutorCode { get; set; }

        /// <summary>参数结构定义 JSON</summary>
        public string ParamSchemaJson { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }
    }

    // ================================================================
    // 新规则平台 DTO
    // ================================================================

    public class TemplateResponse
    {
        public long TemplateId { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string Category { get; set; }
        public string RiskLevel { get; set; }
        public string ExpressionMode { get; set; }
        public string Status { get; set; }
        public int CurrentVersionNo { get; set; }
    }

    public sealed class TemplateDetailResponse : TemplateResponse
    {
        public List<TemplateVersionResponse> Versions { get; set; }
    }

    public sealed class TemplateVersionResponse
    {
        public long TemplateVersionId { get; set; }
        public long TemplateId { get; set; }
        public int VersionNo { get; set; }
        public string VersionStatus { get; set; }
        public string CapabilityFamily { get; set; }
        public string MergeMode { get; set; }
        public string Checksum { get; set; }
        public string Description { get; set; }
        public List<TemplateParamDefDto> ParamDefs { get; set; }
        public List<TemplateStepDefDto> StepDefs { get; set; }
        public List<TemplateScopeDefDto> ScopeDefs { get; set; }
    }

    public sealed class TemplateParamDefDto
    {
        public string ParamCode { get; set; }
        public string ParamName { get; set; }
        public string ValueType { get; set; }
        public bool IsRequired { get; set; }
        public string DefaultText { get; set; }
        public decimal? DefaultNumber { get; set; }
        public bool? DefaultBool { get; set; }
        public string DictType { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string RegexRule { get; set; }
        public string UiControl { get; set; }
        public string HelpText { get; set; }
        public string RiskFlag { get; set; }
        public int SortNo { get; set; }
    }

    public sealed class TemplateStepDefDto
    {
        public int StepNo { get; set; }
        public string StepKind { get; set; }
        public string CapabilityCode { get; set; }
        public string ActionType { get; set; }
        public string ExecutorCode { get; set; }
        public string OnError { get; set; }
        public string StepConfigClob { get; set; }
    }

    public sealed class TemplateScopeDefDto
    {
        public string ScopeDimension { get; set; }
        public bool IsRequired { get; set; }
        public bool AllowMultiple { get; set; }
        public int SortNo { get; set; }
    }

    public sealed class TemplateCreateRequest
    {
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string Category { get; set; }
        public string RiskLevel { get; set; }
        public string ExpressionMode { get; set; }
        public string CreatedBy { get; set; }
    }

    public sealed class TemplateUpdateRequest
    {
        public string TemplateName { get; set; }
        public string Category { get; set; }
        public string RiskLevel { get; set; }
        public string ExpressionMode { get; set; }
        public string Status { get; set; }
        public string UpdatedBy { get; set; }
    }

    public sealed class TemplateVersionSaveRequest
    {
        public long? TemplateVersionId { get; set; }
        public int? VersionNo { get; set; }
        public string CapabilityFamily { get; set; }
        public string MergeMode { get; set; }
        public string Description { get; set; }
        public string Checksum { get; set; }
        public List<TemplateParamDefDto> ParamDefs { get; set; }
        public List<TemplateStepDefDto> StepDefs { get; set; }
        public List<TemplateScopeDefDto> ScopeDefs { get; set; }
    }

    public class PolicyResponse
    {
        public long PolicyId { get; set; }
        public string PolicyCode { get; set; }
        public string PolicyName { get; set; }
        public long TemplateId { get; set; }
        public string OwnerType { get; set; }
        public string PublishProfile { get; set; }
        public string Status { get; set; }
        public int CurrentVersionNo { get; set; }
    }

    public sealed class PolicyDetailResponse : PolicyResponse
    {
        public List<PolicyVersionResponse> Versions { get; set; }
    }

    public sealed class PolicyVersionResponse
    {
        public long PolicyVersionId { get; set; }
        public long PolicyId { get; set; }
        public long TemplateVersionId { get; set; }
        public int VersionNo { get; set; }
        public string PolicyStatus { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string BindingType { get; set; }
        public string ScopeLevel { get; set; }
        public int PriorityWeight { get; set; }
        public string Checksum { get; set; }
        public long? LastBuiltPackageId { get; set; }
        public List<PolicyBindingDto> Bindings { get; set; }
        public List<PolicyScopeDto> Scopes { get; set; }
        public List<PolicyParamDto> Params { get; set; }
    }

    public sealed class PolicyBindingDto
    {
        public string BindingType { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
    }

    public sealed class PolicyScopeDto
    {
        public string ScopeDimension { get; set; }
        public string ScopeOperator { get; set; }
        public string ScopeValueText { get; set; }
        public decimal? ScopeValueNumber { get; set; }
        public DateTime? ScopeValueDate { get; set; }
        public string ScopeJson { get; set; }
    }

    public sealed class PolicyParamDto
    {
        public string ParamCode { get; set; }
        public string ValueType { get; set; }
        public string ValueText { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBool { get; set; }
        public string ExprText { get; set; }
        public string ExprLevel { get; set; }
    }

    public sealed class PolicyCreateRequest
    {
        public string PolicyCode { get; set; }
        public string PolicyName { get; set; }
        public long TemplateId { get; set; }
        public string OwnerType { get; set; }
        public string PublishProfile { get; set; }
        public string CreatedBy { get; set; }
    }

    public sealed class PolicyUpdateRequest
    {
        public string PolicyName { get; set; }
        public string OwnerType { get; set; }
        public string PublishProfile { get; set; }
        public string Status { get; set; }
        public string UpdatedBy { get; set; }
    }

    public sealed class PolicyVersionSaveRequest
    {
        public long? PolicyVersionId { get; set; }
        public int? VersionNo { get; set; }
        public long TemplateVersionId { get; set; }
        public string BindingType { get; set; }
        public string ScopeLevel { get; set; }
        public int PriorityWeight { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string Checksum { get; set; }
        public List<PolicyBindingDto> Bindings { get; set; }
        public List<PolicyScopeDto> Scopes { get; set; }
        public List<PolicyParamDto> Params { get; set; }
    }

    public sealed class PolicyPreviewResponse
    {
        public long PolicyVersionId { get; set; }
        public string PolicyCode { get; set; }
        public long TemplateVersionId { get; set; }
        public string CapabilityFamily { get; set; }
        public string MergeMode { get; set; }
        public List<string> BindingSummary { get; set; }
        public List<string> ScopeSummary { get; set; }
        public List<string> ActionChain { get; set; }
    }

    public sealed class PolicyValidateResponse
    {
        public long PolicyVersionId { get; set; }
        public string PolicyStatus { get; set; }
    }

    public sealed class PolicyReviewSubmitRequest
    {
        public string SubmittedBy { get; set; }
        public string ReviewStage { get; set; }
    }

    public sealed class PolicyReviewDecisionRequest
    {
        public string ReviewedBy { get; set; }
        public string ReviewComment { get; set; }
    }

    public sealed class PolicyImportRequest
    {
        public List<long> RuleIds { get; set; }
        public string ImportedBy { get; set; }
    }

    public sealed class RuntimePackagePublishRequest
    {
        public List<long> PolicyVersionIds { get; set; }
        public string PublishedBy { get; set; }
    }

    public sealed class RuntimePackageOperationRequest
    {
        public string OperatedBy { get; set; }
    }

    public sealed class RuntimePackageHistoryResponse
    {
        public long PackageId { get; set; }
        public long PackageVersion { get; set; }
        public string PackageStatus { get; set; }
        public string BuiltBy { get; set; }
        public DateTime? BuiltAt { get; set; }
        public string ActivatedBy { get; set; }
        public DateTime? ActivatedAt { get; set; }
    }

    public sealed class PolicyPackageDiffResult
    {
        public long CandidatePackageId { get; set; }
        public long? ActivePackageId { get; set; }
        public List<long> AddedPolicyVersionIds { get; set; }
        public List<long> RemovedPolicyVersionIds { get; set; }
        public List<long> UnchangedPolicyVersionIds { get; set; }
    }
}
