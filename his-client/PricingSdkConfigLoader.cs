using System;
using System.Collections.Generic;
using System.IO;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// Pricing SDK 配置文件读取器。
    /// </summary>
    public static class PricingSdkConfigLoader
    {
        /// <summary>
        /// 从 key=value 配置文件读取 SDK 配置。
        /// </summary>
        public static PricingSdkOptions LoadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("配置文件路径不能为空。", "path");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("PricingAgent 配置文件不存在。", path);
            }

            Dictionary<string, string> values = ReadKeyValueFile(path);
            PricingSdkOptions options = new PricingSdkOptions();

            options.BaseUrl = GetString(values, "BaseUrl", options.BaseUrl);
            options.TimeoutMs = GetInt(values, "TimeoutMs", options.TimeoutMs);
            options.MaxRetry = GetInt(values, "MaxRetry", options.MaxRetry);
            options.RetryDelayMs = GetInt(values, "RetryDelayMs", options.RetryDelayMs);
            options.SourceSystem = GetString(values, "SourceSystem", options.SourceSystem);
            options.DefaultChargeScene = GetString(values, "DefaultChargeScene", options.DefaultChargeScene);
            options.EnableLocalLog = GetBool(values, "EnableLocalLog", options.EnableLocalLog);
            options.LogDirectory = GetString(values, "LogDirectory", options.LogDirectory);
            options.EnableCompensationQueue = GetBool(values, "EnableCompensationQueue", options.EnableCompensationQueue);
            options.CompensationDirectory = GetString(values, "CompensationDirectory", options.CompensationDirectory);
            options.ExpectedProtocolVersion = GetString(values, "ExpectedProtocolVersion", options.ExpectedProtocolVersion);

            options.ValidateForHttpClient();
            options.ValidateForProductRuntime();
            return options;
        }

        /// <summary>
        /// 从程序目录下的 pricing-agent.config 读取 SDK 配置。
        /// </summary>
        public static PricingSdkOptions LoadDefault()
        {
            return LoadFromFile(GetDefaultConfigPath());
        }

        /// <summary>
        /// 返回默认配置文件路径。
        /// </summary>
        public static string GetDefaultConfigPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pricing-agent.config");
        }

        private static Dictionary<string, string> ReadKeyValueFile(string path)
        {
            Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line == null)
                {
                    continue;
                }

                line = line.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
                {
                    continue;
                }

                int index = line.IndexOf('=');
                if (index <= 0)
                {
                    throw new InvalidOperationException("配置文件第 " + (i + 1) + " 行格式错误，必须使用 key=value。");
                }

                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1).Trim();
                if (key.Length == 0)
                {
                    throw new InvalidOperationException("配置文件第 " + (i + 1) + " 行 key 不能为空。");
                }

                values[key] = value;
            }

            return values;
        }

        private static string GetString(Dictionary<string, string> values, string key, string defaultValue)
        {
            string value;
            return values.TryGetValue(key, out value) ? value : defaultValue;
        }

        private static int GetInt(Dictionary<string, string> values, string key, int defaultValue)
        {
            string value;
            if (!values.TryGetValue(key, out value) || value.Length == 0)
            {
                return defaultValue;
            }

            int result;
            if (!int.TryParse(value, out result))
            {
                throw new InvalidOperationException("配置项 " + key + " 必须是整数。");
            }

            return result;
        }

        private static bool GetBool(Dictionary<string, string> values, string key, bool defaultValue)
        {
            string value;
            if (!values.TryGetValue(key, out value) || value.Length == 0)
            {
                return defaultValue;
            }

            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "y", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "0", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "no", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "n", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            throw new InvalidOperationException("配置项 " + key + " 必须是布尔值。");
        }
    }
}
