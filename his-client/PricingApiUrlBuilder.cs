using System;
using System.Text;

namespace HIS.Pricing.Client
{
    public static class PricingApiUrlBuilder
    {
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

        public static string BuildRuleById(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId;
        }

        public static string BuildRulesByItemCode(string itemCode)
        {
            return "/api/pricing/rules/by-item/" + Encode(itemCode);
        }

        public static string BuildRuleVersions(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/versions";
        }

        public static string BuildRuleVersionById(long ruleId, long versionId)
        {
            return "/api/pricing/rules/" + ruleId + "/versions/" + versionId;
        }

        public static string BuildRuleConditions(long ruleId, int versionNo)
        {
            return "/api/pricing/rules/" + ruleId + "/versions/" + versionNo + "/conditions";
        }

        public static string BuildRuleActions(long ruleId, int versionNo)
        {
            return "/api/pricing/rules/" + ruleId + "/versions/" + versionNo + "/actions";
        }

        public static string BuildPublishHistory(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/publish-history";
        }

        public static string BuildChangeLogs(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/change-logs";
        }

        public static string BuildRulePublish(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/publish";
        }

        public static string BuildRuleDisable(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/disable";
        }

        public static string BuildRuleRollback(long ruleId)
        {
            return "/api/pricing/rules/" + ruleId + "/rollback";
        }

        public static string BuildDictsQuery(string dictType)
        {
            StringBuilder builder = new StringBuilder("/api/pricing/dicts");
            AppendQuery(builder, "dictType", dictType);
            return builder.ToString();
        }

        public static string BuildDictById(long dictId)
        {
            return "/api/pricing/dicts/" + dictId;
        }

        public static string BuildDictTypes()
        {
            return "/api/pricing/dicts/types";
        }

        public static string BuildFormulaById(long formulaId)
        {
            return "/api/pricing/formulas/" + formulaId;
        }

        public static string BuildSpecialFlag(string itemCode)
        {
            return "/api/pricing/items/" + Encode(itemCode) + "/special-flag";
        }

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

        private static string Encode(string value)
        {
            return Uri.EscapeDataString(value == null ? string.Empty : value);
        }
    }
}
