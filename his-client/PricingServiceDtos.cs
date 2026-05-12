using System;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 计价服务健康检查响应。
    /// </summary>
    public sealed class PricingServiceHealthResponse
    {
        /// <summary>综合健康状态，如 healthy / unhealthy。</summary>
        public string Status { get; set; }

        /// <summary>数据库连接状态。</summary>
        public string Database { get; set; }

        /// <summary>PR_DICT 表是否可访问。</summary>
        public bool DictTableReady { get; set; }

        /// <summary>PR_RULE_HEADER 表是否可访问。</summary>
        public bool RuleHeaderReady { get; set; }

        /// <summary>服务已运行秒数。</summary>
        public double UptimeSeconds { get; set; }

        /// <summary>服务端当前时间。</summary>
        public DateTime ServerTime { get; set; }

        /// <summary>服务端程序版本。</summary>
        public string ServiceVersion { get; set; }

        /// <summary>服务端计价协议版本。</summary>
        public string ProtocolVersion { get; set; }
    }
}
