using System;
using System.IO;
#if NET35
using Newtonsoft.Json;
#else
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 本地补偿队列存储。
    /// </summary>
    public sealed class PricingCompensationStore
    {
#if !NET35
        private static readonly JsonSerializerOptions s_indentedJsonOptions = CreateIndentedJsonOptions();
#endif

        private readonly string _directory;

        /// <summary>
        /// 创建补偿队列存储。
        /// </summary>
        public PricingCompensationStore(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentException("补偿队列目录不能为空。", "directory");
            }

            _directory = directory;
        }

        /// <summary>补偿队列目录。</summary>
        public string DirectoryPath
        {
            get { return _directory; }
        }

        /// <summary>
        /// 按配置创建补偿队列。未启用补偿队列时返回 null。
        /// </summary>
        public static PricingCompensationStore Create(PricingSdkOptions options)
        {
            if (options == null || !options.EnableCompensationQueue)
            {
                return null;
            }

            return new PricingCompensationStore(options.GetNormalizedCompensationDirectory());
        }

        /// <summary>
        /// 保存一条待人工处理或后台重试的补偿记录。
        /// </summary>
        public string SavePending(
            string operation,
            string businessKey,
            object request,
            int? apiCode,
            string apiMessage,
            string traceId,
            Exception exception)
        {
            try
            {
                Directory.CreateDirectory(_directory);
                PricingPendingOperationRecord record = new PricingPendingOperationRecord();
                record.Operation = operation;
                record.BusinessKey = businessKey;
                record.CreatedAt = DateTime.Now;
                record.Request = request;
                record.ApiCode = apiCode;
                record.ApiMessage = apiMessage;
                record.TraceId = traceId;
                record.ExceptionMessage = exception == null ? string.Empty : exception.Message;

                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff")
                    + "-"
                    + SafeFileName(operation)
                    + "-"
                    + SafeFileName(businessKey)
                    + ".json";
                string path = Path.Combine(_directory, fileName);
                File.WriteAllText(path, SerializeJson(record));
                return path;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 查询当前待补偿记录文件。
        /// </summary>
        public static string[] GetPendingFiles(string directory)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                return new string[0];
            }

            return Directory.GetFiles(directory, "*.json");
        }

        private static string SafeFileName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "unknown";
            }

            char[] invalid = Path.GetInvalidFileNameChars();
            char[] chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                for (int j = 0; j < invalid.Length; j++)
                {
                    if (chars[i] == invalid[j])
                    {
                        chars[i] = '_';
                        break;
                    }
                }
            }

            return new string(chars);
        }

        private static string SerializeJson(object value)
        {
#if NET35
            return JsonConvert.SerializeObject(value, Formatting.Indented);
#else
            return JsonSerializer.Serialize(value, s_indentedJsonOptions);
#endif
        }

#if !NET35
        private static JsonSerializerOptions CreateIndentedJsonOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.PropertyNameCaseInsensitive = true;
            options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            options.WriteIndented = true;
            return options;
        }
#endif
    }

    /// <summary>
    /// 本地待补偿操作记录。
    /// </summary>
    public sealed class PricingPendingOperationRecord
    {
        public string Operation { get; set; }

        public string BusinessKey { get; set; }

        public DateTime CreatedAt { get; set; }

        public object Request { get; set; }

        public int? ApiCode { get; set; }

        public string ApiMessage { get; set; }

        public string TraceId { get; set; }

        public string ExceptionMessage { get; set; }
    }
}
