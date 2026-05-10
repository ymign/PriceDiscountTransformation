using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 统一计价服务 HTTP 客户端（.NET Framework 3.5 兼容）
    /// 基于 HttpWebRequest，同步调用，支持超时和重试
    /// </summary>
    public sealed class PricingApiClient
    {
        private readonly string _baseUrl;
        private readonly int _timeoutMs;
        private readonly int _maxRetry;
        private readonly int _retryDelayMs;

        public PricingApiClient(string baseUrl, int timeoutMs, int maxRetry, int retryDelayMs)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _timeoutMs = timeoutMs;
            _maxRetry = maxRetry;
            _retryDelayMs = retryDelayMs;
        }

        public PricingApiClient(string baseUrl)
            : this(baseUrl, 10000, 3, 2000)
        {
        }

        public ApiResponse<PricingCalculateResponse> Simulate(PricingCalculateRequest request)
        {
            return Post<PricingCalculateResponse>("/api/pricing/calculate/simulate", request);
        }

        public ApiResponse<PricingCalculateResponse> Confirm(PricingCalculateRequest request)
        {
            return PostWithRetry<PricingCalculateResponse>(
                "/api/pricing/calculate/confirm", request);
        }

        public ApiResponse Commit(PricingCommitRequest request)
        {
            return PostNoData("/api/pricing/calculate/commit", request);
        }

        public ApiResponse Cancel(PricingCancelRequest request)
        {
            return PostNoData("/api/pricing/calculate/cancel", request);
        }

        public ApiResponse Reverse(PricingReverseRequest request)
        {
            return PostNoData("/api/pricing/calculate/reverse", request);
        }

        public ApiResponse<SpecialFlagResponse> GetSpecialFlag(string itemCode)
        {
            return Get<SpecialFlagResponse>(PricingApiUrlBuilder.BuildSpecialFlag(itemCode));
        }

        public ApiResponse<PagedResponse<RuleHeaderResponse>> GetRules(
            string itemCode, string status, string category, int pageIndex, int pageSize)
        {
            return Get<PagedResponse<RuleHeaderResponse>>(
                PricingApiUrlBuilder.BuildRulesQuery(itemCode, status, category, pageIndex, pageSize));
        }

        public ApiResponse<RuleHeaderResponse> GetRule(long ruleId)
        {
            return Get<RuleHeaderResponse>(PricingApiUrlBuilder.BuildRuleById(ruleId));
        }

        public ApiResponse<List<RuleHeaderResponse>> GetRulesByItemCode(string itemCode)
        {
            return Get<List<RuleHeaderResponse>>(PricingApiUrlBuilder.BuildRulesByItemCode(itemCode));
        }

        public ApiResponse<long> CreateRule(RuleHeaderCreateRequest request)
        {
            return Post<long>("/api/pricing/rules", request);
        }

        public ApiResponse UpdateRule(long ruleId, RuleHeaderUpdateRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildRuleById(ruleId), request);
        }

        public ApiResponse<List<RuleVersionResponse>> GetRuleVersions(long ruleId)
        {
            return Get<List<RuleVersionResponse>>(PricingApiUrlBuilder.BuildRuleVersions(ruleId));
        }

        public ApiResponse<RuleVersionResponse> GetRuleVersion(long ruleId, long versionId)
        {
            return Get<RuleVersionResponse>(PricingApiUrlBuilder.BuildRuleVersionById(ruleId, versionId));
        }

        public ApiResponse<long> CreateDraftVersion(long ruleId)
        {
            return Post<long>(PricingApiUrlBuilder.BuildRuleVersions(ruleId), null);
        }

        public ApiResponse<List<RuleConditionResponse>> GetConditions(long ruleId, int versionNo)
        {
            return Get<List<RuleConditionResponse>>(PricingApiUrlBuilder.BuildRuleConditions(ruleId, versionNo));
        }

        public ApiResponse SaveConditions(long ruleId, int versionNo, RuleConditionSaveRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildRuleConditions(ruleId, versionNo), request);
        }

        public ApiResponse<List<RuleActionResponse>> GetActions(long ruleId, int versionNo)
        {
            return Get<List<RuleActionResponse>>(PricingApiUrlBuilder.BuildRuleActions(ruleId, versionNo));
        }

        public ApiResponse SaveActions(long ruleId, int versionNo, RuleActionSaveRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildRuleActions(ruleId, versionNo), request);
        }

        public ApiResponse<List<RulePublishResponse>> GetPublishHistory(long ruleId)
        {
            return Get<List<RulePublishResponse>>(PricingApiUrlBuilder.BuildPublishHistory(ruleId));
        }

        public ApiResponse<List<RuleChangeLogResponse>> GetChangeLogs(long ruleId)
        {
            return Get<List<RuleChangeLogResponse>>(PricingApiUrlBuilder.BuildChangeLogs(ruleId));
        }

        public ApiResponse PublishRule(long ruleId, RulePublishRequest request)
        {
            return PostNoData(PricingApiUrlBuilder.BuildRulePublish(ruleId), request);
        }

        public ApiResponse DisableRule(long ruleId, RuleDisableRequest request)
        {
            return PostNoData(PricingApiUrlBuilder.BuildRuleDisable(ruleId), request);
        }

        public ApiResponse RollbackRule(long ruleId, RuleRollbackRequest request)
        {
            return PostNoData(PricingApiUrlBuilder.BuildRuleRollback(ruleId), request);
        }

        public ApiResponse<List<DictResponse>> GetDicts(string dictType)
        {
            return Get<List<DictResponse>>(PricingApiUrlBuilder.BuildDictsQuery(dictType));
        }

        public ApiResponse<List<string>> GetDictTypes()
        {
            return Get<List<string>>(PricingApiUrlBuilder.BuildDictTypes());
        }

        public ApiResponse<long> CreateDict(DictCreateRequest request)
        {
            return Post<long>("/api/pricing/dicts", request);
        }

        public ApiResponse UpdateDict(long dictId, DictUpdateRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildDictById(dictId), request);
        }

        public ApiResponse DeleteDict(long dictId)
        {
            string responseText = SendRequest("DELETE", PricingApiUrlBuilder.BuildDictById(dictId), null);
            return JsonConvert.DeserializeObject<ApiResponse>(responseText);
        }

        public ApiResponse<List<FormulaDefResponse>> GetFormulas()
        {
            return Get<List<FormulaDefResponse>>("/api/pricing/formulas");
        }

        public ApiResponse<FormulaDefResponse> GetFormula(long formulaId)
        {
            return Get<FormulaDefResponse>(PricingApiUrlBuilder.BuildFormulaById(formulaId));
        }

        public ApiResponse<long> CreateFormula(FormulaDefCreateRequest request)
        {
            return Post<long>("/api/pricing/formulas", request);
        }

        public ApiResponse UpdateFormula(long formulaId, FormulaDefUpdateRequest request)
        {
            return PutNoData(PricingApiUrlBuilder.BuildFormulaById(formulaId), request);
        }

        private ApiResponse<T> PostWithRetry<T>(string path, object body)
        {
            Exception lastEx = null;
            for (int i = 0; i < _maxRetry; i++)
            {
                try
                {
                    return Post<T>(path, body);
                }
                catch (WebException ex)
                {
                    lastEx = ex;
                    if (i < _maxRetry - 1)
                    {
                        Thread.Sleep(_retryDelayMs);
                    }
                }
            }
            throw new PricingApiException("计价服务请求失败，已重试 " + _maxRetry + " 次", lastEx);
        }

        private ApiResponse<T> Post<T>(string path, object body)
        {
            string json = JsonConvert.SerializeObject(body);
            string responseText = SendRequest("POST", path, json);
            return JsonConvert.DeserializeObject<ApiResponse<T>>(responseText);
        }

        private ApiResponse PostNoData(string path, object body)
        {
            string json = JsonConvert.SerializeObject(body);
            string responseText = SendRequest("POST", path, json);
            return JsonConvert.DeserializeObject<ApiResponse>(responseText);
        }

        private ApiResponse PutNoData(string path, object body)
        {
            string json = JsonConvert.SerializeObject(body);
            string responseText = SendRequest("PUT", path, json);
            return JsonConvert.DeserializeObject<ApiResponse>(responseText);
        }

        private ApiResponse<T> Get<T>(string path)
        {
            string responseText = SendRequest("GET", path, null);
            return JsonConvert.DeserializeObject<ApiResponse<T>>(responseText);
        }

        private string SendRequest(string method, string path, string jsonBody)
        {
            string url = _baseUrl + path;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.ContentType = "application/json; charset=utf-8";
            req.Accept = "application/json";
            req.Timeout = _timeoutMs;
            req.ReadWriteTimeout = _timeoutMs;

            if (jsonBody != null && method != "GET")
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonBody);
                req.ContentLength = data.Length;
                using (Stream stream = req.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    return ReadResponse(resp);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (HttpWebResponse errResp = (HttpWebResponse)ex.Response)
                    {
                        string errBody = ReadResponse(errResp);
                        throw new PricingApiException(
                            "HTTP " + (int)errResp.StatusCode + ": " + errBody, ex);
                    }
                }
                throw;
            }
        }

        private static string ReadResponse(HttpWebResponse response)
        {
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public sealed class PricingCalculateRequest
    {
        public string RequestNo { get; set; }
        public string PatientId { get; set; }
        public string VisitId { get; set; }
        public string EncounterNo { get; set; }
        public string ChargeScene { get; set; }
        public DateTime BusinessChargeTime { get; set; }
        public string SourceSystem { get; set; }
        public string SourceTerminal { get; set; }
        public string ChargeNo { get; set; }
        public string BusinessRequestNo { get; set; }
        public string OperatorId { get; set; }
        public string OperatorName { get; set; }
        public Dictionary<string, object> ExtraParams { get; set; }
        public List<PricingCalculateItemRequest> Items { get; set; }
    }

    public sealed class PricingCalculateItemRequest
    {
        public string ItemRequestNo { get; set; }
        public string ChargeDetailNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal InputQty { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public string BodyPartCode { get; set; }
        public DateTime? BusinessChargeTime { get; set; }
        public Dictionary<string, object> ExtraParams { get; set; }
        public List<PricingPartItemRequest> PricingParts { get; set; }
    }

    public sealed class PricingPartItemRequest
    {
        public int? PartSeq { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string BodyPartCode { get; set; }
        public decimal Qty { get; set; }
        public decimal? Area { get; set; }
        public string MeasureType { get; set; }
        public decimal? MeasureValue { get; set; }
        public string MeasureUnit { get; set; }
        public int? LesionCount { get; set; }
    }

    public sealed class PricingCalculateResponse
    {
        public List<PricingCalculateItemResponse> Items { get; set; }
        public long RequestId { get; set; }
        public bool IsSpecialItem { get; set; }
        public decimal InputQty { get; set; }
        public decimal FinalQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public List<PricingTraceStepResponse> TraceSteps { get; set; }
        public List<long> MatchedRuleIds { get; set; }
    }

    public sealed class PricingCalculateItemResponse
    {
        public string ItemRequestNo { get; set; }
        public string ChargeDetailNo { get; set; }
        public long RequestId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public bool IsSpecialItem { get; set; }
        public decimal InputQty { get; set; }
        public decimal FinalQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public List<PricingTraceStepResponse> TraceSteps { get; set; }
        public List<long> MatchedRuleIds { get; set; }
    }

    public sealed class PricingTraceStepResponse
    {
        public int StepNo { get; set; }
        public string StepType { get; set; }
        public string StepDesc { get; set; }
        public decimal? InputValue { get; set; }
        public decimal? OutputValue { get; set; }
    }

    public sealed class PricingCommitRequest
    {
        public long RequestId { get; set; }
        public string ChargeNo { get; set; }
    }

    public sealed class PricingCancelRequest
    {
        public long RequestId { get; set; }
    }

    public sealed class PricingReverseRequest
    {
        public long OriginalRequestId { get; set; }
        public string ReverseNo { get; set; }
        public string ChargeDetailNo { get; set; }
        public string ItemCode { get; set; }
        public int? PartSeq { get; set; }
        public DateTime? ReverseTime { get; set; }
        public decimal? ReverseQty { get; set; }
        public decimal? ReverseAmt { get; set; }
        public string ReversedBy { get; set; }
        public string Reason { get; set; }
    }

    public sealed class SpecialFlagResponse
    {
        public string ItemCode { get; set; }
        public bool IsSpecial { get; set; }
        public int RuleCount { get; set; }
    }

    public sealed class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string TraceId { get; set; }
        public bool IsSuccess
        {
            get { return Code == 0; }
        }
    }

    public sealed class ApiResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string TraceId { get; set; }
        public bool IsSuccess
        {
            get { return Code == 0; }
        }
    }

    public sealed class PricingApiException : Exception
    {
        public PricingApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
