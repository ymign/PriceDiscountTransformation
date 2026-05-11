using System;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// Pricing SDK 运行配置。
    /// 该类不依赖 WinForms，可被 HIS、服务端适配器、自助机前置程序等复用。
    /// </summary>
    public sealed class PricingSdkOptions
    {
        /// <summary>计价服务基础地址，例如 http://pricing-rule-center-host。</summary>
        public string BaseUrl { get; set; }

        /// <summary>HTTP 超时时间，单位毫秒。</summary>
        public int TimeoutMs { get; set; }

        /// <summary>最大重试次数，含首次请求。</summary>
        public int MaxRetry { get; set; }

        /// <summary>重试间隔，单位毫秒。</summary>
        public int RetryDelayMs { get; set; }

        /// <summary>来源系统编码。多院区或多渠道部署时建议按医院/渠道区分。</summary>
        public string SourceSystem { get; set; }

        /// <summary>默认收费场景。调用方请求未传 ChargeScene 时使用。</summary>
        public string DefaultChargeScene { get; set; }

        /// <summary>
        /// 构造默认配置。
        /// 默认值与 PricingApiClient 保持一致，便于旧 HIS 最小改造接入。
        /// </summary>
        public PricingSdkOptions()
        {
            BaseUrl = string.Empty;
            TimeoutMs = 10000;
            MaxRetry = 3;
            RetryDelayMs = 2000;
            SourceSystem = "HIS";
            DefaultChargeScene = "OUTPATIENT";
        }

        /// <summary>
        /// 校验配置是否可用于创建 HTTP 客户端。
        /// </summary>
        public void ValidateForHttpClient()
        {
            if (string.IsNullOrEmpty(BaseUrl))
            {
                throw new InvalidOperationException("PricingSdkOptions.BaseUrl 不能为空。");
            }

            if (TimeoutMs <= 0)
            {
                throw new InvalidOperationException("PricingSdkOptions.TimeoutMs 必须大于 0。");
            }

            if (MaxRetry <= 0)
            {
                throw new InvalidOperationException("PricingSdkOptions.MaxRetry 必须大于 0。");
            }

            if (RetryDelayMs < 0)
            {
                throw new InvalidOperationException("PricingSdkOptions.RetryDelayMs 不能小于 0。");
            }
        }

        /// <summary>
        /// 返回去除首尾空白和尾部斜杠后的 BaseUrl。
        /// </summary>
        public string GetNormalizedBaseUrl()
        {
            return NormalizeBaseUrl(BaseUrl);
        }

        /// <summary>
        /// 规范化计价服务地址，避免调用层拼接出双斜杠。
        /// </summary>
        public static string NormalizeBaseUrl(string baseUrl)
        {
            if (baseUrl == null)
            {
                return string.Empty;
            }

            return baseUrl.Trim().TrimEnd('/');
        }
    }
}
