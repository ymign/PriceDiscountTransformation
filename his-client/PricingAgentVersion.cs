using System;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// PricingAgent 产品版本信息。
    /// </summary>
    public static class PricingAgentVersion
    {
        /// <summary>产品名称。</summary>
        public const string ProductName = "PricingAgent";

        /// <summary>SDK DLL 版本。修改公开协议或交付包时必须递增。</summary>
        public const string SdkVersion = "1.0.0";

        /// <summary>客户端协议版本。服务端 /health 返回的 ProtocolVersion 必须与该值一致。</summary>
        public const string ProtocolVersion = "1.0";

        /// <summary>配置文件格式版本。</summary>
        public const string ConfigFormatVersion = "1.0";

        /// <summary>当前产品化交付日期。</summary>
        public const string BuildDate = "2026-05-12";

        /// <summary>
        /// 返回可展示给现场实施和 HIS 开发人员的版本摘要。
        /// </summary>
        public static string GetDisplayText()
        {
            return ProductName
                + " SDK " + SdkVersion
                + " / Protocol " + ProtocolVersion
                + " / Config " + ConfigFormatVersion
                + " / Build " + BuildDate;
        }

        /// <summary>
        /// 校验服务端协议是否与当前 SDK 兼容。
        /// </summary>
        public static void EnsureCompatibleService(PricingServiceHealthResponse health)
        {
            EnsureCompatibleService(health, ProtocolVersion);
        }

        /// <summary>
        /// 按指定协议版本校验服务端兼容性。
        /// </summary>
        public static void EnsureCompatibleService(PricingServiceHealthResponse health, string expectedProtocolVersion)
        {
            if (health == null)
            {
                throw new InvalidOperationException("无法获取计价服务健康检查结果。");
            }

            if (string.IsNullOrEmpty(health.ProtocolVersion))
            {
                throw new InvalidOperationException("计价服务未返回 ProtocolVersion，无法确认 SDK 协议兼容性。");
            }

            if (string.IsNullOrEmpty(expectedProtocolVersion))
            {
                throw new InvalidOperationException("期望协议版本不能为空。");
            }

            if (!string.Equals(expectedProtocolVersion, health.ProtocolVersion, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "计价服务协议版本不兼容。客户端 ProtocolVersion="
                    + expectedProtocolVersion
                    + "，服务端 ProtocolVersion="
                    + health.ProtocolVersion
                    + "。");
            }
        }
    }
}
