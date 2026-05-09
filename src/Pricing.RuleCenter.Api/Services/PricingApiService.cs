using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using SqlSugar;

namespace Pricing.RuleCenter.Api.Services;

/// <summary>
/// 计价 API 应用服务，统一编排试算、确认、提交、取消、冲正和特殊项目识别。
/// </summary>
/// <remarks>
/// 这个服务刻意不把资金安全逻辑下沉到控制器中。控制器只负责 HTTP 入口，
/// 这里集中处理权威单价校验、confirm 幂等、请求指纹、三段式状态机、
/// 限额占用、追溯日志和事务边界。这样做的目的，是让 HIS、自助机、公众号
/// 等不同渠道共享同一套资金口径，避免各端在失败重试、取消落账、过期释放
/// 上出现不一致。
/// </remarks>
public sealed class PricingApiService
{
    /// <summary>
    /// 计价引擎依赖，负责规则匹配和动作链执行；应用服务只关心它的输入输出，
    /// 不直接关心具体规则动作如何计算。
    /// </summary>
    private readonly IPricingEngine _engine;
    /// <summary>
    /// 规则头仓储，用于 special-flag 查询，判断某个项目当前是否存在已发布特殊规则。
    /// </summary>
    private readonly IRuleHeaderRepository _headerRepository;
    /// <summary>
    /// 计价请求日志仓储，负责保存幂等键、请求指纹、请求快照、响应快照和业务状态。
    /// </summary>
    private readonly IChargeRequestLogRepository _requestLogRepository;
    /// <summary>
    /// 折价结果明细仓储，负责保存最终折价、封顶、公式调整结果，并参与 commit/cancel/expire 状态同步。
    /// </summary>
    private readonly IChargeDiscountDetailRepository _discountRepository;
    /// <summary>
    /// 计价步骤仓储，负责把规则匹配、公式、限额、折价等过程写入追溯链路。
    /// </summary>
    private readonly IChargeTraceStepRepository _traceStepRepository;
    /// <summary>
    /// 限额占用仓储，负责时间窗、单日等额度的累计查询、锁行加锁和占用状态推进。
    /// </summary>
    private readonly ILimitOccupyRepository _limitRepository;
    /// <summary>
    /// 冲正日志仓储，负责记录退费、作废、撤销时为什么释放或作废原占用。
    /// </summary>
    private readonly IChargeReverseLogRepository _reverseLogRepository;
    /// <summary>
    /// 权威物价主数据仓储，confirm/simulate 计算前先校验单价，避免渠道传错单价导致错收费。
    /// </summary>
    private readonly IPriceMasterRepository _priceMasterRepository;
    /// <summary>
    /// _db 数据库客户端，用于开启事务和执行必须贴近 Oracle 的查询或锁操作。
    /// </summary>
    private readonly ISqlSugarClient _db;
    /// <summary>
    /// _options 配置对象，集中承载超时、清理间隔、单价校验等运行参数。
    /// </summary>
    private readonly PricingOptions _options;
    /// <summary>
    /// _logger 日志依赖，用于记录关键状态流转、幂等命中和异常上下文。
    /// </summary>
    private readonly ILogger<PricingApiService> _logger;

    /// <summary>
    /// 初始化计价接口应用服务。
    /// </summary>
    /// <param name="engine">计价引擎，负责规则匹配和动作链执行。</param>
    /// <param name="headerRepository">规则头仓储，用于 special-flag 查询。</param>
    /// <param name="requestLogRepository">请求日志仓储，用于幂等、状态机和快照保存。</param>
    /// <param name="discountRepository">折价明细仓储，用于保存最终结果并同步状态。</param>
    /// <param name="traceStepRepository">计价步骤仓储，用于写入可追溯的过程日志。</param>
    /// <param name="limitRepository">限额占用仓储，用于锁定和更新累计额度。</param>
    /// <param name="reverseLogRepository">冲正日志仓储，用于记录 reverse 操作审计。</param>
    /// <param name="priceMasterRepository">权威物价仓储，用于校验渠道传入单价。</param>
    /// <param name="db">SqlSugar 数据库客户端，用于创建事务边界。</param>
    /// <param name="options">计价配置项，包括过期时间和单价校验开关。</param>
    /// <param name="logger">日志组件，用于记录幂等命中、状态推进和异常上下文。</param>
    public PricingApiService(
        IPricingEngine engine,
        IRuleHeaderRepository headerRepository,
        IChargeRequestLogRepository requestLogRepository,
        IChargeDiscountDetailRepository discountRepository,
        IChargeTraceStepRepository traceStepRepository,
        ILimitOccupyRepository limitRepository,
        IChargeReverseLogRepository reverseLogRepository,
        IPriceMasterRepository priceMasterRepository,
        ISqlSugarClient db,
        IOptions<PricingOptions> options,
        ILogger<PricingApiService> logger)
    {
        _engine = engine;
        _headerRepository = headerRepository;
        _requestLogRepository = requestLogRepository;
        _discountRepository = discountRepository;
        _traceStepRepository = traceStepRepository;
        _limitRepository = limitRepository;
        _reverseLogRepository = reverseLogRepository;
        _priceMasterRepository = priceMasterRepository;
        _db = db;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// 执行试算计价。
    /// </summary>
    /// <param name="request">统一计价请求。试算允许重复调用，不要求稳定业务号。</param>
    /// <returns>返回本次试算的金额、数量、命中规则和追溯步骤。</returns>
    /// <remarks>
    /// 试算用于页面展示或影子验证，不写正式折价明细，也不占用限额额度。
    /// 但它仍然校验权威单价并保存请求日志和步骤日志，因为后续排查“页面为什么显示这个价格”
    /// 时需要看到当时的规则匹配过程。
    /// </remarks>
    public async Task<PricingCalculateResponse> SimulateAsync(PricingCalculateRequest request)
    {
        // ========== 第一阶段：价格入口校验 ==========
        // 即使是试算，也不允许在启用权威单价校验时用错误单价继续计算。
        // 这样可以提前暴露 HIS、自助机或公众号传参错误，避免试算展示和最终确认口径不一致。
        await ValidateAuthorityPriceAsync(request);

        // ========== 第二阶段：构造非占额上下文并执行引擎 ==========
        // shouldLockLimits=false 表示执行器只按历史 PENDING/CONFIRMED 数据试算，
        // 不创建 PR_LIMIT_LOCK 锁，也不写 PR_LIMIT_OCCUPY 占用。
        var context = BuildContext(request, "SIMULATE", shouldLockLimits: false);
        var result = await _engine.CalculateAsync(context);

        // ========== 第三阶段：保存试算追溯 ==========
        // 试算不会进入资金状态机，但保留请求和步骤日志可以支持后续页面解释、问题复盘和影子对账。
        var requestLog = await SaveRequestLog(
            request, result, "SIMULATE", "SIMULATED", fingerprint: null);
        await SaveTraceSteps(requestLog.RequestId, result);

        // ========== 第四阶段：保存响应快照 ==========
        // 响应快照不是幂等必需，但可以让追溯查询直接展示当时返回给渠道的结果。
        var response = BuildResponse(requestLog.RequestId, result);
        await SaveResponseJson(requestLog, response);
        return response;
    }

    /// <summary>
    /// 执行正式确认计价，并把结果推进到待落账保护状态。
    /// </summary>
    /// <param name="request">统一计价请求。confirm 必须传入稳定的 <c>BusinessRequestNo</c>。</param>
    /// <returns>返回可供 HIS 落账使用的最终计价结果。</returns>
    /// <exception cref="ArgumentException">业务号为空时抛出，防止 confirm 超时重试重复占额。</exception>
    /// <exception cref="InvalidOperationException">同一业务号参数变化时抛出 IDEMPOTENT_CONFLICT。</exception>
    /// <remarks>
    /// confirm 是资金安全链路的核心入口。它不直接表示 HIS 已经落账成功，而是表示计价中心
    /// 已经计算出结果并暂时占用额度。只有 HIS 写入收费明细成功并调用 commit 后，状态才会进入
    /// CONFIRMED；如果 HIS 失败或用户取消，则必须 cancel；如果长时间没有回调，则由后台 expire。
    /// </remarks>
    public async Task<PricingCalculateResponse> ConfirmAsync(PricingCalculateRequest request)
    {
        // ========== 第一阶段：强制稳定业务号 ==========
        // requestNo 是一次 HTTP 调用流水，超时重试时可能变化；BusinessRequestNo 才代表一次收费确认动作。
        // 如果这里允许空业务号，服务端无法区分“同一动作重试”和“新收费动作”，会导致重复占额。
        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            throw new ArgumentException("CONFIRM 必须传入稳定的 BusinessRequestNo");
        }

        // ========== 第二阶段：权威单价校验 ==========
        // 单价错误属于资金风险，不应该进入规则引擎后再修正。这里直接失败，让渠道重新取价或修正参数。
        await ValidateAuthorityPriceAsync(request);

        // ========== 第三阶段：幂等键和请求指纹校验 ==========
        // 幂等键只定位“是否同一次业务动作”；请求指纹负责证明“这次业务动作的参数没有悄悄变化”。
        // 两者不能互相替代，否则用户改了部位、数量或 extraParams 后可能继续复用旧结果。
        var fingerprint = BuildFingerprint(request, "CONFIRM");
        var existing = await _requestLogRepository.GetByBusinessKeyAsync(
            request.SourceSystem, request.BusinessRequestNo!, "CONFIRM");
        if (existing is not null)
        {
            // 同一业务号下参数不一致时必须阻断。这里不能覆盖旧记录，也不能重新计算，
            // 否则调用方会以为重试成功，实际额度和折价明细已经对应另一组参数。
            if (!string.Equals(existing.RequestFingerprint, fingerprint, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"IDEMPOTENT_CONFLICT: BusinessRequestNo={request.BusinessRequestNo} 已存在，但本次参数与首次请求不一致");
            }

            // 参数一致时直接返回首次响应快照，不重新执行规则，也不再次写占额。
            // 这是 confirm 超时重试时避免重复占额的关键路径。
            _logger.LogInformation(
                "幂等命中 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, RequestId={RequestId}",
                request.SourceSystem, request.BusinessRequestNo, existing.RequestId);
            return await BuildIdempotentResponse(existing);
        }

        // ========== 第四阶段：事务内执行引擎、写请求、写明细、写占额 ==========
        // confirm 产生的三类记录必须一起成功或一起回滚：
        // 1. PR_CHARGE_REQUEST_LOG：请求状态和响应快照；
        // 2. PR_CHARGE_DISCOUNT_DETAIL：待确认折价结果；
        // 3. PR_LIMIT_OCCUPY：待确认额度占用。
        // 如果其中任一张表失败，不能留下半条资金链路。
        return await ExecuteInTransactionAsync(async () =>
        {
            // shouldLockLimits=true 表示限额执行器会在计算窗口累计前锁定 PR_LIMIT_LOCK，
            // 以防两个渠道同时确认同一患者同一项目时一起看到“还剩额度”。
            var context = BuildContext(request, "CONFIRM", shouldLockLimits: true);
            var result = await _engine.CalculateAsync(context);

            // 请求日志先落库并拿到 RequestId，后续步骤、折价明细和占额都用它串联。
            var requestLog = await SaveRequestLog(
                request, result, "CONFIRM", "CONFIRM_PENDING", fingerprint);
            await SaveTraceSteps(requestLog.RequestId, result);

            // 普通项目没有命中特殊规则时仍有请求日志和响应快照，但不写折价明细和占额。
            // 特殊项目必须写 PENDING 明细与 PENDING 占额，等待 HIS commit/cancel。
            if (result.IsSpecialItem)
            {
                await SaveDiscountDetail(requestLog.RequestId, request, result, "PENDING");
                await SaveLimitOccupies(requestLog.RequestId, result);
            }

            // 最后保存响应快照。幂等重试优先读取这个快照，保证返回内容和首次 confirm 完全一致。
            var response = BuildResponse(requestLog.RequestId, result);
            await SaveResponseJson(requestLog, response);
            return response;
        });
    }

    /// <summary>
    /// HIS 落账成功后提交计价结果。
    /// </summary>
    /// <param name="request">commit 请求，必须携带 confirm 返回的请求 ID。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// commit 只允许从 CONFIRM_PENDING 进入 CONFIRMED。它不能重复提交，也不能提交已取消或已过期记录。
    /// 这样能保证报表口径只统计真正落账成功的折价明细和限额占用。
    /// </remarks>
    public async Task CommitAsync(PricingCommitRequest request)
    {
        // ========== 第一阶段：事务保护状态推进 ==========
        // 请求日志、折价明细、限额占用三张表必须在同一事务内推进到 CONFIRMED。
        // 如果任何一步失败，整体回滚，避免报表看到“请求已确认但占额仍待确认”的断裂状态。
        await ExecuteInTransactionAsync(async () =>
        {
            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new KeyNotFoundException($"请求不存在: {request.RequestId}");

            // 只允许待确认保护状态 commit。已经取消、过期或确认的记录再 commit，
            // 通常代表渠道回调乱序或重复回调，必须暴露给调用方处理。
            if (log.BusinessStatus != "CONFIRM_PENDING")
            {
                throw new InvalidOperationException(
                    $"只有CONFIRM_PENDING状态可以COMMIT, 当前: {log.BusinessStatus}");
            }

            // confirm 结果有有效期。过期后额度可能已经释放给其他收费动作，
            // 因此不能再接受迟到 commit，调用方必须重新 confirm。
            if (DateTime.Now > log.RequestAt.AddMinutes(_options.ConfirmExpireMinutes))
            {
                throw new InvalidOperationException("EXPIRED: 确认计价结果已过期，请重新 confirm");
            }

            // 请求日志进入 CONFIRMED，表示 HIS 已经完成本地落账。
            log.BusinessStatus = "CONFIRMED";
            log.ChargeNo = request.ChargeNo ?? log.ChargeNo;
            log.ResponseAt = DateTime.Now;
            await _requestLogRepository.UpdateAsync(log);

            // 折价明细和限额占用同步确认。后续窗口累计会把 CONFIRMED 记录计入净占用。
            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, "CONFIRMED");
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, "CONFIRMED");

            _logger.LogInformation("COMMIT RequestId={RequestId}", request.RequestId);
        });
    }

    /// <summary>
    /// HIS 落账失败、支付失败或用户取消时释放 confirm 保护状态。
    /// </summary>
    /// <param name="request">cancel 请求，必须携带 confirm 返回的请求 ID。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// cancel 只允许处理 CONFIRM_PENDING。它的业务含义不是“退费”，而是“确认结果没有被 HIS 使用”，
    /// 因此应该释放待确认占额，并把折价明细标记为 CANCELLED，避免进入正式收费报表。
    /// </remarks>
    public async Task CancelAsync(PricingCancelRequest request)
    {
        // ========== 第一阶段：事务保护取消 ==========
        // 与 commit 一样，cancel 也必须同步更新请求日志、折价明细和限额占用。
        // 否则会出现额度释放了但明细还在 PENDING，或明细取消了但额度仍占着的资金口径断裂。
        await ExecuteInTransactionAsync(async () =>
        {
            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new KeyNotFoundException($"请求不存在: {request.RequestId}");

            // 只有待落账的 confirm 可以取消。已 CONFIRMED 的记录代表 HIS 已经收费，
            // 应走 reverse，而不是 cancel。
            if (log.BusinessStatus != "CONFIRM_PENDING")
            {
                throw new InvalidOperationException(
                    $"只有CONFIRM_PENDING状态可以CANCEL, 当前: {log.BusinessStatus}");
            }

            // 请求日志标记为取消后，对账和追溯可以明确知道这次 confirm 没有形成正式收费。
            log.BusinessStatus = "CANCELLED";
            log.ResponseAt = DateTime.Now;
            await _requestLogRepository.UpdateAsync(log);

            // CANCELLED 占额不参与后续限额累计，相当于释放 confirm 阶段的待确认保护额度。
            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, "CANCELLED");
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, "CANCELLED");

            _logger.LogInformation("CANCEL RequestId={RequestId}", request.RequestId);
        });
    }

    /// <summary>
    /// 对已经落账确认的计价结果执行冲正。
    /// </summary>
    /// <param name="request">冲正请求，当前实现只支持整笔冲正。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// reverse 的语义不同于 cancel。cancel 处理“未落账的确认结果”，reverse 处理“已经落账的收费结果”。
    /// 当前阶段为了避免部分退费释放额度过量，仅支持整笔冲正；部分退费需要按 partSeq、历史已退数量和金额
    /// 做更细的校验后再开放。
    /// </remarks>
    public async Task ReverseAsync(PricingReverseRequest request)
    {
        // ========== 第一阶段：事务保护冲正 ==========
        // 冲正会同时影响请求状态、折价明细、限额占用和冲正日志。任何一环失败都不能部分提交。
        await ExecuteInTransactionAsync(async () =>
        {
            var log = await _requestLogRepository.GetByIdAsync(request.OriginalRequestId)
                ?? throw new KeyNotFoundException($"原请求不存在: {request.OriginalRequestId}");

            // 只有已落账确认的记录才能 reverse。CONFIRM_PENDING 应该 cancel；
            // CANCELLED/EXPIRED 没有形成正式收费，不应该再冲正。
            if (log.BusinessStatus != "CONFIRMED")
            {
                throw new InvalidOperationException(
                    $"只有CONFIRMED状态可以REVERSE, 当前: {log.BusinessStatus}");
            }

            // ========== 第二阶段：确认当前是否为整笔冲正 ==========
            // 部分退费需要校验“本次退费 + 历史已退 <= 原有效收费”，还要处理多部位 partSeq。
            // 当前接口没有足够信息表达这些口径，所以宁可明确拒绝，也不能自动释放错误额度。
            var details = await _discountRepository.GetByRequestIdAsync(request.OriginalRequestId);
            var originalQty = details.Sum(d => d.FinalQty ?? 0);
            var originalAmt = details.Sum(d => d.FinalAmt ?? 0);
            var reverseQty = request.ReverseQty ?? originalQty;
            var reverseAmt = request.ReverseAmt ?? originalAmt;
            if (reverseQty != originalQty || reverseAmt != originalAmt)
            {
                throw new InvalidOperationException("PARTIAL_REVERSE_NOT_SUPPORTED: 当前接口只支持整笔冲正");
            }

            // ========== 第三阶段：推进原请求与关联记录状态 ==========
            // 整笔冲正下，原占用和折价明细可直接标记 REVERSED，后续累计查询不再把它当作有效收费。
            log.BusinessStatus = "REVERSED";
            log.ResponseAt = DateTime.Now;
            await _requestLogRepository.UpdateAsync(log);

            await _discountRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, "REVERSED");
            await _limitRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, "REVERSED");

            // ========== 第四阶段：写冲正审计 ==========
            // 这张表回答“为什么原来的收费结果被冲掉”，也是后续财务追查和退费口径复盘的入口。
            await _reverseLogRepository.InsertAsync(new ChargeReverseLog
            {
                OriginalRequestId = request.OriginalRequestId,
                ChargeNo = log.ChargeNo,
                ReverseNo = request.ReverseNo,
                ItemCode = log.ItemCode,
                ReverseQty = reverseQty,
                ReverseAmt = reverseAmt,
                ReverseReason = request.Reason,
                ReversedBy = request.ReversedBy,
                ReversedAt = DateTime.Now
            });

            _logger.LogInformation("REVERSE OriginalRequestId={OriginalRequestId}", request.OriginalRequestId);
        });
    }

    /// <summary>
    /// 查询项目是否属于必须调用统一计价中心的特殊项目。
    /// </summary>
    /// <param name="itemCode">HIS 项目编码。</param>
    /// <returns>返回特殊项目标识和已发布规则数量。</returns>
    /// <remarks>
    /// 渠道侧可以用这个接口决定是否弹出特殊计价流程。若服务不可用，渠道侧应按文档要求保守处理，
    /// 不能自行按普通单价收费。
    /// </remarks>
    public async Task<SpecialFlagResponse> GetSpecialFlagAsync(string itemCode)
    {
        // special-flag 初期直接查数据库，不做本地缓存。规则发布频率低，实时查询可以避免缓存失效不及时。
        var rules = await _headerRepository.GetByItemCodeAsync(itemCode);
        var published = rules.Where(r => r.Status == "PUBLISHED" && r.IsEnabled == "Y").ToList();

        return new SpecialFlagResponse
        {
            ItemCode = itemCode,
            IsSpecial = published.Count > 0,
            RuleCount = published.Count
        };
    }

    private async Task ValidateAuthorityPriceAsync(PricingCalculateRequest request)
    {
        // ========== 第一阶段：兼容开关 ==========
        // 开发或联调环境可能暂时没有 HIS 权威物价表。配置关闭时跳过校验，
        // 但生产资金链路应保持开启，避免渠道传错单价后继续计算。
        if (!_options.EnableAuthorityPriceCheck)
        {
            return;
        }

        // ========== 第二阶段：读取权威单价 ==========
        // 单价来源以计价中心可访问的权威主数据为准，请求中的 UnitPrice 只作为对账校验输入。
        var authorityPrice = await _priceMasterRepository.GetUnitPriceAsync(request.ItemCode);
        if (!authorityPrice.HasValue)
        {
            // 找不到权威单价时不能“按渠道传入价格先算”，否则统一计价中心会变成错误价格的放大器。
            throw new InvalidOperationException(
                $"PRICE_MISMATCH: 未找到项目 {request.ItemCode} 的权威单价");
        }

        // ========== 第三阶段：按金额精度比较 ==========
        // Oracle 表和 C# 都以 4 位小数为当前金额精度，因此比较前先统一 round 到 4 位。
        if (Math.Round(authorityPrice.Value, 4) != Math.Round(request.UnitPrice, 4))
        {
            throw new InvalidOperationException(
                $"PRICE_MISMATCH: 项目 {request.ItemCode} 权威单价={authorityPrice.Value}, 请求单价={request.UnitPrice}");
        }
    }

    private static PricingContext BuildContext(
        PricingCalculateRequest request, string callType, bool shouldLockLimits)
    {
        // ========== 第一阶段：把接口 DTO 转换成引擎上下文 ==========
        // 引擎只关心标准化后的业务字段。这里统一 trim 字符串，并把空字符串折叠为 null，
        // 这样条件匹配和指纹计算不会因为多余空格或空串/null 差异产生不稳定行为。
        return new PricingContext
        {
            CallType = callType,
            ShouldLockLimits = shouldLockLimits,
            PatientId = request.PatientId.Trim(),
            VisitId = NormalizeString(request.VisitId),
            ItemCode = request.ItemCode.Trim(),
            ItemName = NormalizeString(request.ItemName),
            InputQty = request.InputQty,
            Unit = NormalizeString(request.Unit),
            UnitPrice = request.UnitPrice,
            BodyPartCode = NormalizeString(request.BodyPartCode),
            ChargeScene = NormalizeString(request.ChargeScene),
            BusinessChargeTime = request.BusinessChargeTime,
            SourceSystem = request.SourceSystem.Trim(),
            ChargeNo = NormalizeString(request.ChargeNo),
            BusinessRequestNo = NormalizeString(request.BusinessRequestNo),
            PricingParts = request.PricingParts?.Select(p => new PricingPartItem
            {
                PartSeq = p.PartSeq,
                PartCode = NormalizeString(p.PartCode),
                PartName = NormalizeString(p.PartName),
                BodyPartCode = NormalizeString(p.BodyPartCode),
                Qty = p.Qty,
                Area = p.Area,
                MeasureType = NormalizeString(p.MeasureType),
                MeasureValue = p.MeasureValue,
                MeasureUnit = NormalizeString(p.MeasureUnit),
                LesionCount = p.LesionCount
            }).ToList()
        };
    }

    private async Task<ChargeRequestLog> SaveRequestLog(
        PricingCalculateRequest request,
        PricingResult result,
        string callType,
        string businessStatus,
        string? fingerprint)
    {
        // ========== 第一阶段：构造请求日志 ==========
        // RequestNo 是技术流水，BusinessRequestNo 是业务幂等号。两者都保存：
        // 前者便于定位一次 HTTP 调用，后者用于判断同一次收费确认动作。
        var log = new ChargeRequestLog
        {
            RequestNo = NormalizeString(request.RequestNo) ?? $"REQ-{DateTime.Now:yyyyMMddHHmmssfff}",
            BusinessRequestNo = NormalizeString(request.BusinessRequestNo),
            RequestFingerprint = fingerprint,
            CallType = callType,
            BusinessStatus = businessStatus,
            SourceSystem = request.SourceSystem.Trim(),
            SourceTerminal = NormalizeString(request.SourceTerminal),
            PatientId = request.PatientId.Trim(),
            VisitId = NormalizeString(request.VisitId),
            ChargeScene = NormalizeString(request.ChargeScene),
            ChargeNo = NormalizeString(request.ChargeNo),
            ChargeDetailNo = NormalizeString(request.ChargeDetailNo),
            ItemCode = request.ItemCode.Trim(),
            ItemName = NormalizeString(request.ItemName),
            InputQty = request.InputQty,
            InputUnit = NormalizeString(request.Unit),
            BodyPartCode = NormalizeString(request.BodyPartCode),
            BusinessChargeTime = request.BusinessChargeTime,
            RequestJson = JsonConvert.SerializeObject(request),
            ResponseJson = JsonConvert.SerializeObject(result),
            RequestAt = DateTime.Now,
            ResponseAt = DateTime.Now,
            IsSuccess = "Y"
        };

        // ========== 第二阶段：插入日志并回填数据库主键 ==========
        // 后续折价明细、步骤日志、限额占用都依赖 RequestId 串联，所以这里先落库。
        await _requestLogRepository.InsertAsync(log);
        return log;
    }

    private async Task SaveResponseJson(ChargeRequestLog log, PricingCalculateResponse response)
    {
        // 响应快照用于 confirm 幂等重放。相同业务号和相同指纹再次进入时，
        // 直接返回首次响应，避免重复执行规则和重复占额。
        log.ResponseJson = JsonConvert.SerializeObject(response);
        log.ResponseAt = DateTime.Now;
        await _requestLogRepository.UpdateAsync(log);
    }

    private async Task SaveTraceSteps(long requestId, PricingResult result)
    {
        // 没有命中特殊规则时可能没有步骤。这里直接返回，避免写空集合导致不必要的数据库调用。
        if (result.TraceSteps.Count == 0)
        {
            return;
        }

        // 步骤日志只保存 DDL 允许的 StepType。ActionExecutionPipeline 已经把具体动作类型映射为
        // MATCH/FORMULA/LIMIT/DISCOUNT 等稳定类别，避免违反数据库 CHECK 约束。
        var entities = result.TraceSteps.Select(s => new ChargeTraceStep
        {
            RequestId = requestId,
            StepNo = s.StepNo,
            StepName = s.StepType,
            StepType = s.StepType,
            InputSnapshot = s.InputValue?.ToString(),
            OutputSnapshot = s.OutputValue?.ToString(),
            StepDesc = s.StepDesc,
            CreatedAt = DateTime.Now
        }).ToList();

        await _traceStepRepository.InsertBatchAsync(entities);
    }

    private async Task SaveDiscountDetail(
        long requestId,
        PricingCalculateRequest request,
        PricingResult result,
        string status)
    {
        // ========== 第一阶段：选择主命中规则 ==========
        // 当前折价明细表只有一个 RULE_ID 字段。多规则叠加时这里先记录第一条命中规则，
        // 完整动作链仍通过步骤日志和请求响应快照追溯。
        var firstRuleId = result.MatchedRuleIds.FirstOrDefault();

        // ========== 第二阶段：保存待确认或最终折价明细 ==========
        // confirm 阶段写 PENDING，commit 后再统一改 CONFIRMED。这样未落账的结果不会进入正式报表。
        var detail = new ChargeDiscountDetail
        {
            RequestId = requestId,
            ChargeNo = NormalizeString(request.ChargeNo),
            ChargeDetailNo = NormalizeString(request.ChargeDetailNo),
            PatientId = request.PatientId,
            VisitId = request.VisitId,
            ItemCode = request.ItemCode,
            ItemName = request.ItemName,
            RuleId = firstRuleId == 0 ? null : firstRuleId,
            OriginalQty = request.InputQty,
            FinalQty = result.FinalQty,
            UnitPrice = result.UnitPrice,
            OriginalAmt = request.UnitPrice * request.InputQty,
            CalculatedAmt = result.FinalAmount,
            FinalAmt = result.FinalAmount,
            DiscountAmt = result.DiscountAmount,
            Status = status,
            OccurredAt = DateTime.Now
        };

        await _discountRepository.InsertAsync(detail);
    }

    private async Task SaveLimitOccupies(long requestId, PricingResult result)
    {
        // confirm 结果不是永久有效。ExpireAt 用于后台清理长时间未 commit 的保护占用，
        // 防止 HIS 异常退出后额度一直被 PENDING 记录占住。
        var expireAt = DateTime.Now.AddMinutes(_options.ConfirmExpireMinutes);
        foreach (var occupy in result.LimitOccupies)
        {
            occupy.RequestId = requestId;
            occupy.Status = "PENDING";
            occupy.ExpireAt = expireAt;
            occupy.OccupiedAt = DateTime.Now;
            await _limitRepository.InsertAsync(occupy);
        }
    }

    private static PricingCalculateResponse BuildResponse(long requestId, PricingResult result)
    {
        // 响应 DTO 只返回渠道需要使用或展示的字段；更完整的内部计算状态留在追溯日志和请求快照里。
        return new PricingCalculateResponse
        {
            RequestId = requestId,
            IsSpecialItem = result.IsSpecialItem,
            InputQty = result.InputQty,
            FinalQty = result.FinalQty,
            UnitPrice = result.UnitPrice,
            FinalAmount = result.FinalAmount,
            DiscountAmount = result.DiscountAmount,
            TraceSteps = result.TraceSteps.Select(s => new PricingTraceStepResponse
            {
                StepNo = s.StepNo,
                StepType = s.StepType,
                StepDesc = s.StepDesc,
                InputValue = s.InputValue,
                OutputValue = s.OutputValue
            }).ToList(),
            MatchedRuleIds = result.MatchedRuleIds
        };
    }

    private async Task<PricingCalculateResponse> BuildIdempotentResponse(ChargeRequestLog log)
    {
        // ========== 第一优先级：返回首次响应快照 ==========
        // 这是最严格的幂等语义。即使后来规则配置发生变化，重试同一业务号也必须得到首次 confirm 的结果。
        if (!string.IsNullOrWhiteSpace(log.ResponseJson))
        {
            var response = JsonConvert.DeserializeObject<PricingCalculateResponse>(log.ResponseJson);
            if (response is not null)
            {
                return response;
            }
        }

        // ========== 兜底路径：由折价明细重建响应 ==========
        // 正常情况下不应该走到这里。保留兜底是为了兼容历史数据或响应快照缺失的异常记录。
        var details = await _discountRepository.GetByRequestIdAsync(log.RequestId);
        var detail = details.FirstOrDefault();
        return new PricingCalculateResponse
        {
            RequestId = log.RequestId,
            IsSpecialItem = detail is not null,
            InputQty = log.InputQty ?? 0,
            FinalQty = detail?.FinalQty ?? 0,
            UnitPrice = detail?.UnitPrice ?? 0,
            FinalAmount = detail?.FinalAmt ?? 0,
            DiscountAmount = detail?.DiscountAmt ?? 0
        };
    }

    private async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        // ========== 第一阶段：开启数据库事务 ==========
        // 本服务的大部分资金操作会同时更新多张表。使用同一个 SqlSugarClient 事务可以保证
        // 请求日志、折价明细、限额占用、冲正日志之间不会出现半提交。
        try
        {
            await _db.Ado.BeginTranAsync();
            var result = await action();
            // ========== 第二阶段：全部成功后提交 ==========
            // 只有所有仓储操作都成功，才允许对外暴露本次状态推进。
            await _db.Ado.CommitTranAsync();
            return result;
        }
        catch
        {
            // ========== 第三阶段：任何异常都回滚 ==========
            // 资金链路宁可失败返回给渠道重试，也不能留下部分写入的请求或占额。
            await _db.Ado.RollbackTranAsync();
            throw;
        }
    }

    private async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            await action();
            return true;
        });
    }

    private static string BuildFingerprint(PricingCalculateRequest request, string callType)
    {
        // ========== 第一阶段：构造规范化业务载荷 ==========
        // 指纹必须覆盖会影响规则匹配和金额的字段。只比较 patientId/itemCode/inputQty 是不够的：
        // 部位、业务时间、手术号、孕次号、extraParams、pricingParts 都可能改变最终结果。
        var payload = new
        {
            sourceSystem = NormalizeString(request.SourceSystem),
            businessRequestNo = NormalizeString(request.BusinessRequestNo),
            callType,
            patientId = NormalizeString(request.PatientId),
            visitId = NormalizeString(request.VisitId),
            encounterNo = NormalizeString(request.EncounterNo),
            chargeScene = NormalizeString(request.ChargeScene),
            chargeNo = NormalizeString(request.ChargeNo),
            chargeDetailNo = NormalizeString(request.ChargeDetailNo),
            itemCode = NormalizeString(request.ItemCode),
            itemName = NormalizeString(request.ItemName),
            inputQty = Math.Round(request.InputQty, 4),
            inputUnit = NormalizeString(request.Unit),
            unitPrice = Math.Round(request.UnitPrice, 4),
            chargeTime = request.BusinessChargeTime,
            bodyPartCode = NormalizeString(request.BodyPartCode),
            operationNo = GetExtraParam(request, "operationNo"),
            pregnancyNo = GetExtraParam(request, "pregnancyNo"),
            mainChargeDetailNo = GetExtraParam(request, "mainChargeDetailNo"),
            extraParams = NormalizeExtraParams(request.ExtraParams),
            pricingParts = request.PricingParts?
                .OrderBy(p => p.PartSeq ?? int.MaxValue)
                .ThenBy(p => p.PartCode)
                .Select(p => new
                {
                    partSeq = p.PartSeq,
                    partCode = NormalizeString(p.PartCode),
                    partName = NormalizeString(p.PartName),
                    bodyPartCode = NormalizeString(p.BodyPartCode),
                    qty = Math.Round(p.Qty, 4),
                    area = p.Area.HasValue ? Math.Round(p.Area.Value, 4) : (decimal?)null,
                    measureType = NormalizeString(p.MeasureType),
                    measureValue = p.MeasureValue.HasValue ? Math.Round(p.MeasureValue.Value, 4) : (decimal?)null,
                    measureUnit = NormalizeString(p.MeasureUnit),
                    lesionCount = p.LesionCount
                })
                .ToList()
        };

        // ========== 第二阶段：序列化后计算 SHA256 ==========
        // 存 hash 而不是完整 JSON，可以控制 REQUEST_FINGERPRINT 字段长度，同时避免数据库索引过大。
        var json = JsonConvert.SerializeObject(payload, Formatting.None);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash);
    }

    private static IReadOnlyDictionary<string, object?>? NormalizeExtraParams(
        IReadOnlyDictionary<string, object?>? extraParams)
    {
        return extraParams?
            .OrderBy(k => k.Key, StringComparer.Ordinal)
            .ToDictionary(k => k.Key.Trim(), k => NormalizeExtraValue(k.Value));
    }

    private static object? NormalizeExtraValue(object? value)
    {
        return value switch
        {
            null => null,
            string text => NormalizeString(text),
            decimal number => Math.Round(number, 4),
            double number => Math.Round((decimal)number, 4),
            float number => Math.Round((decimal)number, 4),
            JsonElement element => element.ValueKind == JsonValueKind.String
                ? NormalizeString(element.GetString())
                : element.GetRawText(),
            _ => value
        };
    }

    private static object? GetExtraParam(PricingCalculateRequest request, string key)
    {
        if (request.ExtraParams is null ||
            !request.ExtraParams.TryGetValue(key, out var value))
        {
            return null;
        }

        return NormalizeExtraValue(value);
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
