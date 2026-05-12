using System;
using System.IO;
using System.Text;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// PricingAgent 本地日志。
    /// </summary>
    public sealed class PricingAgentLogger
    {
        private readonly object _syncRoot = new object();
        private readonly string _logDirectory;

        /// <summary>
        /// 创建日志实例。
        /// </summary>
        public PricingAgentLogger(string logDirectory)
        {
            if (string.IsNullOrEmpty(logDirectory))
            {
                throw new ArgumentException("日志目录不能为空。", "logDirectory");
            }

            _logDirectory = logDirectory;
        }

        /// <summary>日志目录。</summary>
        public string LogDirectory
        {
            get { return _logDirectory; }
        }

        /// <summary>
        /// 按配置创建日志实例。未启用本地日志时返回 null。
        /// </summary>
        public static PricingAgentLogger Create(PricingSdkOptions options)
        {
            if (options == null || !options.EnableLocalLog)
            {
                return null;
            }

            return new PricingAgentLogger(options.GetNormalizedLogDirectory());
        }

        /// <summary>
        /// 写入普通日志。
        /// </summary>
        public void Info(
            string operation,
            string requestId,
            string businessNo,
            string requestNo,
            string traceId,
            int? code,
            long elapsedMs,
            string message)
        {
            Write("INFO", operation, requestId, businessNo, requestNo, traceId, code, elapsedMs, message);
        }

        /// <summary>
        /// 写入错误日志。
        /// </summary>
        public void Error(
            string operation,
            string requestId,
            string businessNo,
            string requestNo,
            string traceId,
            int? code,
            long elapsedMs,
            string message)
        {
            Write("ERROR", operation, requestId, businessNo, requestNo, traceId, code, elapsedMs, message);
        }

        private void Write(
            string level,
            string operation,
            string requestId,
            string businessNo,
            string requestNo,
            string traceId,
            int? code,
            long elapsedMs,
            string message)
        {
            try
            {
                Directory.CreateDirectory(_logDirectory);
                string file = Path.Combine(_logDirectory, "pricing-agent-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    + "\t" + Safe(level)
                    + "\t" + Safe(operation)
                    + "\trequestId=" + Safe(requestId)
                    + "\tbusinessNo=" + Safe(businessNo)
                    + "\trequestNo=" + Safe(requestNo)
                    + "\ttraceId=" + Safe(traceId)
                    + "\tcode=" + (code.HasValue ? code.Value.ToString() : string.Empty)
                    + "\telapsedMs=" + elapsedMs
                    + "\t" + Safe(message)
                    + Environment.NewLine;

                lock (_syncRoot)
                {
                    File.AppendAllText(file, line, Encoding.UTF8);
                }
            }
            catch
            {
                // 日志失败不能影响收费主流程。
            }
        }

        private static string Safe(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
        }
    }
}
