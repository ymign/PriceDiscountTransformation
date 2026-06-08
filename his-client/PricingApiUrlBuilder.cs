using System;
using System.Text;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 计价服务 API URL 构建器（静态工具类）。
    /// 集中管理所有 API 端点的路径拼接逻辑，避免 URL 分散在各调用处。
    ///
    /// 设计说明：
    /// - 所有方法返回相对路径（不含 baseUrl），由 PricingApiClient 拼接完整 URL
    /// - 路径参数使用 Uri.EscapeDataString 编码，防止特殊字符（如中文项目编码）导致 URL 解析错误
    /// - 查询参数通过 AppendQuery 方法自动处理 ?/& 分隔符和空值过滤
    /// </summary>
    public static class PricingApiUrlBuilder
    {
        // ================================================================
        // 规则管理端点
        // ================================================================

        /// <summary>
        /// 构建规则分页查询 URL。
        /// GET /api/pricing/rules?itemCode=xxx&status=xxx&category=xxx&pageIndex=1&pageSize=200
        /// </summary>
        /// <param name="itemCode">项目编码筛选（空值时省略此参数）</param>
        /// <param name="status">状态筛选（空值时省略此参数）</param>
        /// <param name="category">类别筛选（空值时省略此参数）</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>含查询参数的相对路径</returns>
        public static string BuildRulesQuery(string itemCode, string status, string category, int pageIndex, int pageSize)
        {
            StringBuilder builder = new StringBuilder("/api/pricing/rules");
            AppendQuery(builder, "itemCode", itemCode);
            AppendQuery(builder, "status", status);
            AppendQuery(builder, "category", category);
            AppendQuery(builder, "pageIndex", pageIndex.ToString());
            AppendQuery(builder, "pageSize", pageSize.ToString());
            return builder.ToString();
        }

        /// <summary>构建单条规则详情 URL。GET /api/pricing/rules/{ruleId}</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildRuleById(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId;
        }

        /// <summary>
        /// 构建按项目编码查询规则 URL。
        /// GET /api/pricing/rules/by-item/{itemCode}
        /// itemCode 会进行 URL 编码，支持中文项目名称。
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <returns>相对路径</returns>
        public static string BuildRulesByItemCode(string itemCode)
        {
            return "/api/pricing/rules/by-item/" + Encode(itemCode);
        }

        /// <summary>构建规则版本列表 URL。GET /api/pricing/rules/{ruleId}/versions</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildRuleVersions(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/versions";
        }

        /// <summary>构建单个版本详情 URL。GET /api/pricing/rules/{ruleId}/versions/{versionId}</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionId">版本 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildRuleVersionById(long ruleId, long versionId)
        {
            return "/api/pricing/rules/" + ruleId + "/versions/" + versionId;
        }

        /// <summary>
        /// 构建规则条件端点 URL。
        /// GET/PUT /api/pricing/rules/{ruleId}/versions/{versionNo}/conditions
        /// GET 用于查询条件列表，PUT 用于全量替换条件。
        /// </summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionNo">版本号</param>
        /// <returns>相对路径</returns>
        public static string BuildRuleConditions(long ruleId, int versionNo)
        {
            return "/api/pricing/rules/" + ruleId + "/versions/" + versionNo + "/conditions";
        }

        /// <summary>
        /// 构建规则动作端点 URL。
        /// GET/PUT /api/pricing/rules/{ruleId}/versions/{versionNo}/actions
        /// GET 用于查询动作列表，PUT 用于全量替换动作。
        /// </summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionNo">版本号</param>
        /// <returns>相对路径</returns>
        public static string BuildRuleActions(long ruleId, int versionNo)
        {
            return "/api/pricing/rules/" + ruleId + "/versions/" + versionNo + "/actions";
        }

        /// <summary>构建发布历史查询 URL。GET /api/pricing/rules/{ruleId}/publish-history</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildPublishHistory(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/publish-history";
        }

        /// <summary>构建变更日志查询 URL。GET /api/pricing/rules/{ruleId}/change-logs</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildChangeLogs(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/change-logs";
        }

        /// <summary>构建规则发布端点 URL。POST /api/pricing/rules/{ruleId}/publish</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildRulePublish(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/publish";
        }

        /// <summary>构建规则停用端点 URL。POST /api/pricing/rules/{ruleId}/disable</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildRuleDisable(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/disable";
        }

        /// <summary>构建规则回滚端点 URL。POST /api/pricing/rules/{ruleId}/rollback</summary>
        /// <param name="ruleId">规则 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildRuleRollback(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/rollback";
        }

        // ================================================================
        // 模板/策略/运行时包端点
        // ================================================================

        public static string BuildTemplates()
        {
            return "/api/pricing/templates";
        }

        public static string BuildTemplateById(long templateId)
        {
            return "/api/pricing/templates/" + templateId;
        }

        public static string BuildTemplateVersion(long templateId, long templateVersionId)
        {
            return "/api/pricing/templates/" + templateId + "/versions/" + templateVersionId;
        }

        public static string BuildTemplateVersions(long templateId)
        {
            return "/api/pricing/templates/" + templateId + "/versions";
        }

        public static string BuildPolicies()
        {
            return "/api/pricing/policies";
        }

        public static string BuildPolicyById(long policyId)
        {
            return "/api/pricing/policies/" + policyId;
        }

        public static string BuildPolicyVersions(long policyId)
        {
            return "/api/pricing/policies/" + policyId + "/versions";
        }

        public static string BuildPolicyVersionById(long policyVersionId)
        {
            return "/api/pricing/policies/versions/" + policyVersionId;
        }

        public static string BuildPolicyPreview(long policyVersionId)
        {
            return "/api/pricing/policies/versions/" + policyVersionId + "/preview";
        }

        public static string BuildPolicyValidate(long policyVersionId)
        {
            return "/api/pricing/policies/versions/" + policyVersionId + "/validate";
        }

        public static string BuildPolicyReviewSubmit(long policyVersionId)
        {
            return "/api/pricing/policies/versions/" + policyVersionId + "/review/submit";
        }

        public static string BuildPolicyReviewApprove(long policyVersionId)
        {
            return "/api/pricing/policies/versions/" + policyVersionId + "/review/approve";
        }

        public static string BuildPolicyReviewReject(long policyVersionId)
        {
            return "/api/pricing/policies/versions/" + policyVersionId + "/review/reject";
        }

        public static string BuildRuntimePackagesPublish()
        {
            return "/api/pricing/runtime-packages/publish";
        }

        public static string BuildRuntimePackageDiff(long packageId)
        {
            return "/api/pricing/runtime-packages/" + packageId + "/diff";
        }

        public static string BuildRuntimePackageActivate(long packageId)
        {
            return "/api/pricing/runtime-packages/" + packageId + "/activate";
        }

        public static string BuildRuntimePackageRollback(long packageId)
        {
            return "/api/pricing/runtime-packages/" + packageId + "/rollback";
        }

        public static string BuildRuntimePackageHistory(int take)
        {
            StringBuilder builder = new StringBuilder("/api/pricing/runtime-packages/history");
            AppendQuery(builder, "take", take.ToString());
            return builder.ToString();
        }

        // ================================================================
        // 字典管理端点
        // ================================================================

        /// <summary>
        /// 构建字典查询 URL。
        /// GET /api/pricing/dicts?dictType=xxx
        /// </summary>
        /// <param name="dictType">字典类型（空值时省略，查询所有字典）</param>
        /// <returns>含查询参数的相对路径</returns>
        public static string BuildDictsQuery(string dictType)
        {
            StringBuilder builder = new StringBuilder("/api/pricing/dicts");
            AppendQuery(builder, "dictType", dictType);
            return builder.ToString();
        }

        /// <summary>构建单个字典项 URL。GET/PUT/DELETE /api/pricing/dicts/{dictId}</summary>
        /// <param name="dictId">字典项 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildDictById(long dictId)
        {
            return "/api/pricing/dicts/" + dictId;
        }

        /// <summary>构建字典类型列表 URL。GET /api/pricing/dicts/types</summary>
        /// <returns>相对路径</returns>
        public static string BuildDictTypes()
        {
            return "/api/pricing/dicts/types";
        }

        // ================================================================
        // 公式管理端点
        // ================================================================

        /// <summary>构建单个公式定义 URL。GET/PUT /api/pricing/formulas/{formulaId}</summary>
        /// <param name="formulaId">公式 ID</param>
        /// <returns>相对路径</returns>
        public static string BuildFormulaById(long formulaId)
        {
            return "/api/pricing/formulas/" + formulaId;
        }

        // ================================================================
        // 特殊项目标识查询端点
        // ================================================================

        /// <summary>
        /// 构建特殊项目标识查询 URL。
        /// GET /api/pricing/items/{itemCode}/special-flag
        /// itemCode 会进行 URL 编码。
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <returns>相对路径</returns>
        public static string BuildSpecialFlag(string itemCode)
        {
            return "/api/pricing/items/" + Encode(itemCode) + "/special-flag";
        }

        // ================================================================
        // 内部工具方法
        // ================================================================

        /// <summary>
        /// 向 StringBuilder 追加查询参数。
        /// 自动判断使用 ? 还是 & 分隔（根据是否已有 ?）。
        /// 空值参数直接跳过（不追加），实现"空值不传参"的语义。
        /// </summary>
        /// <param name="builder">目标 StringBuilder</param>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值（空值时跳过）</param>
        private static void AppendQuery(StringBuilder builder, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            builder.Append(builder.ToString().IndexOf('?') >= 0 ? '&' : '?');
            builder.Append(Encode(name));
            builder.Append('=');
            builder.Append(Encode(value));
        }

        /// <summary>
        /// URL 编码。使用 Uri.EscapeDataString 对路径段或查询参数值进行编码。
        /// null 值视为空字符串处理。
        /// </summary>
        /// <param name="value">待编码字符串</param>
        /// <returns>编码后的字符串</returns>
        private static string Encode(string value)
        {
            return Uri.EscapeDataString(value == null ? string.Empty : value);
        }
    }
}
