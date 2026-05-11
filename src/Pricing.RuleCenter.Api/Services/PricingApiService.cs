using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pricing.RuleCenter.Api.Dto;
using Pricing.RuleCenter.Core.Interfaces;
using Pricing.RuleCenter.Core.Models;
using Pricing.RuleCenter.Core.Options;
using Pricing.RuleCenter.Core.Services;
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

    private sealed record ItemPricingCalculation(
        PricingCalculateItemRequest Item,
        PricingResult Result);

    private sealed record CommitDetailKey(
        string ChargeDetailNo,
        string ItemCode,
        int? PartSeq);

    private sealed record CommitDetailTotals(
        decimal Qty,
        decimal Amount);

    /// <summary>
    /// 执行试算计价。
    /// </summary>
    /// <param name="request">统一计价请求。试算允许重复调用，不要求稳定业务号。</param>
    /// <returns>返回本次试算的金额、数量、命中规则和追溯步骤。</returns>
    /// <remarks>
    /// 试算用于页面展示或影子验证，不写正式折价明细，也不占用限额额度。
    /// 但它仍然校验权威单价并保存请求日志和步骤日志，因为后续排查"页面为什么显示这个价格"
    /// 时需要看到当时的规则匹配过程。
    /// </remarks>
    public async Task<PricingCalculateResponse> SimulateAsync(PricingCalculateRequest request)
    {
        var items = GetRequiredItems(request);

        // ========== 第零阶段：记录试算请求入口日志 ==========
        var firstItem = items[0];
        _logger.LogInformation(
            "SIMULATE 开始 SourceSystem={SourceSystem}, PatientId={PatientId}, ItemCode={ItemCode}, InputQty={InputQty}",
            request.SourceSystem, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        // ========== 第一阶段：价格入口校验 ==========
        // 即使是试算，也不允许在启用权威单价校验时用错误单价继续计算。
        // 这样可以提前暴露 HIS、自助机或公众号传参错误，避免试算展示和最终确认口径不一致。
        await ValidateAuthorityPriceAsync(items);

        // ========== 第二阶段：构造非占额上下文并执行引擎 ==========
        // shouldLockLimits=false 表示执行器只按历史 PENDING/CONFIRMED 数据试算，
        // 不创建 PR_LIMIT_LOCK 锁，也不写 PR_LIMIT_OCCUPY 占用。
        // 批量场景下创建 BatchPricingContext，确保同批内多个项目的限额累计和互斥判断正确。
        var inRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        var inRequestLimitOccupies = new List<LimitOccupy>();
        var batchContext = items.Count > 1 ? new BatchPricingContext() : null;
        var calculations = new List<ItemPricingCalculation>(items.Count);
        foreach (var item in items)
        {
            var context = BuildContext(
                request,
                item,
                "SIMULATE",
                shouldLockLimits: false,
                inRequestOccupiedQtyByLimitDimension,
                inRequestLimitOccupies);
            var result = await _engine.CalculateAsync(context, batchContext);
            AccumulateInRequestLimits(inRequestOccupiedQtyByLimitDimension, inRequestLimitOccupies, result);
            calculations.Add(new ItemPricingCalculation(item, result));
        }

        // ========== 第三阶段：保存试算追溯 ==========
        // 试算不会进入资金状态机，但保留请求和步骤日志可以支持后续页面解释、问题复盘和影子对账。
        var requestLog = await SaveRequestLog(
            request, items, calculations, "SIMULATE", "SIMULATED", fingerprint: null);
        await SaveTraceSteps(requestLog.RequestId, calculations);

        // ========== 第四阶段：保存响应快照 ==========
        // 响应快照不是幂等必需，但可以让追溯查询直接展示当时返回给渠道的结果。
        var response = BuildResponse(requestLog.RequestId, calculations);
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
        var items = GetRequiredItems(request);

        // ========== 第零阶段：记录 confirm 请求入口日志 ==========
        // 无论后续成功或失败，先记录请求到达信息，便于排查"渠道说调了但计价中心没收到"的问题。
        // 只记录首项的 itemCode，多项目场景通过 RequestId 在追溯链路中查看完整列表。
        var firstItem = items[0];
        _logger.LogInformation(
            "CONFIRM 开始 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, PatientId={PatientId}, ItemCode={ItemCode}, InputQty={InputQty}",
            request.SourceSystem, request.BusinessRequestNo, request.PatientId, firstItem.ItemCode, firstItem.InputQty);

        // ========== 第一阶段：强制稳定业务号 ==========
        // requestNo 是一次 HTTP 调用流水，超时重试时可能变化；BusinessRequestNo 才代表一次收费确认动作。
        // 如果这里允许空业务号，服务端无法区分"同一动作重试"和"新收费动作"，会导致重复占额。
        if (string.IsNullOrWhiteSpace(request.BusinessRequestNo))
        {
            _logger.LogWarning(
                "CONFIRM 校验失败: BusinessRequestNo 为空, SourceSystem={SourceSystem}",
                request.SourceSystem);
            throw new ArgumentException("CONFIRM 必须传入稳定的 BusinessRequestNo");
        }

        // ========== 第二阶段：权威单价校验 ==========
        // 单价错误属于资金风险，不应该进入规则引擎后再修正。这里直接失败，让渠道重新取价或修正参数。
        await ValidateAuthorityPriceAsync(items);

        // ========== 第三阶段：幂等键和请求指纹校验 ==========
        // 幂等键只定位"是否同一次业务动作"；请求指纹负责证明"这次业务动作的参数没有悄悄变化"。
        // 两者不能互相替代，否则用户改了部位、数量或 extraParams 后可能继续复用旧结果。
        var fingerprint = BuildFingerprint(request, items, "CONFIRM");
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
                "幂等命中 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, RequestId={RequestId}, ItemCode={ItemCode}, OriginalStatus={Status}",
                request.SourceSystem, request.BusinessRequestNo, existing.RequestId, existing.ItemCode, existing.BusinessStatus);
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
            await _limitRepository.EnsureAndLockAsync(new[]
            {
                BuildIdempotencyLockKey(request.SourceSystem, request.BusinessRequestNo!, "CONFIRM")
            });

            var existingInTransaction = await _requestLogRepository.GetByBusinessKeyAsync(
                request.SourceSystem, request.BusinessRequestNo!, "CONFIRM");
            if (existingInTransaction is not null)
            {
                if (!string.Equals(existingInTransaction.RequestFingerprint, fingerprint, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"IDEMPOTENT_CONFLICT: BusinessRequestNo={request.BusinessRequestNo} 已存在，但本次参数与首次请求不一致");
                }

                _logger.LogInformation(
                    "CONFIRM 事务内幂等命中 SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, RequestId={RequestId}",
                    request.SourceSystem, request.BusinessRequestNo, existingInTransaction.RequestId);
                return await BuildIdempotentResponse(existingInTransaction);
            }

            // shouldLockLimits=true 表示限额执行器会在计算窗口累计前锁定 PR_LIMIT_LOCK，
            // 以防两个渠道同时确认同一患者同一项目时一起看到"还剩额度"。
            // 批量场景下创建 BatchPricingContext，确保同批内限额累计和互斥判断正确。
            var inRequestOccupiedQtyByLimitDimension = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var inRequestLimitOccupies = new List<LimitOccupy>();
            var batchContext = items.Count > 1 ? new BatchPricingContext() : null;
            var calculations = new List<ItemPricingCalculation>(items.Count);
            foreach (var item in items)
            {
                var context = BuildContext(
                    request,
                    item,
                    "CONFIRM",
                    shouldLockLimits: true,
                    inRequestOccupiedQtyByLimitDimension,
                    inRequestLimitOccupies);
                var result = await _engine.CalculateAsync(context, batchContext);
                AccumulateInRequestLimits(inRequestOccupiedQtyByLimitDimension, inRequestLimitOccupies, result);
                calculations.Add(new ItemPricingCalculation(item, result));
            }

            // 请求日志先落库并拿到 RequestId，后续步骤、折价明细和占额都用它串联。
            var requestLog = await SaveRequestLog(
                request, items, calculations, "CONFIRM", "CONFIRM_PENDING", fingerprint);
            await SaveTraceSteps(requestLog.RequestId, calculations);

            // 普通项目没有命中特殊规则时仍有请求日志和响应快照，但不写折价明细和占额。
            // 特殊项目必须写 PENDING 明细与 PENDING 占额，等待 HIS commit/cancel。
            foreach (var calculation in calculations.Where(c => c.Result.IsSpecialItem))
            {
                await SaveDiscountDetail(
                    requestLog.RequestId, request, calculation.Item, calculation.Result, "PENDING");
                await SaveLimitOccupies(requestLog.RequestId, calculation.Result);
            }

            // 最后保存响应快照。幂等重试优先读取这个快照，保证返回内容和首次 confirm 完全一致。
            var response = BuildResponse(
                requestLog.RequestId,
                calculations,
                requestLog.RequestAt.AddMinutes(_options.ConfirmExpireMinutes));
            await SaveResponseJson(requestLog, response);

            // ========== 第五阶段：记录 confirm 成功日志 ==========
            // 记录最终结果摘要，便于对账和异常排查。
            // 包含 RequestId 以便后续通过追溯接口查看完整计算过程。
            _logger.LogInformation(
                "CONFIRM 成功 RequestId={RequestId}, SourceSystem={SourceSystem}, BusinessRequestNo={BusinessRequestNo}, ItemCode={ItemCode}, FinalQty={FinalQty}, FinalAmount={FinalAmount}, IsSpecialItem={IsSpecialItem}",
                requestLog.RequestId, request.SourceSystem, request.BusinessRequestNo,
                firstItem.ItemCode, response.FinalQty, response.FinalAmount, response.IsSpecialItem);

            return response;
        });
    }

    /// <summary>
    /// HIS 落账成功后提交计价结果，主子项目原子处理。
    /// </summary>
    /// <param name="request">commit 请求，必须携带 confirm 返回的请求 ID。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// <para>
    /// commit 只允许从 CONFIRM_PENDING 进入 CONFIRMED。它不能重复提交，也不能提交已取消或已过期记录。
    /// 这样能保证报表口径只统计真正落账成功的折价明细和限额占用。
    /// </para>
    /// <para>
    /// 主子项目原子性保证：
    /// 当请求涉及主子项目（通过 ResultGroupNo 关联）时，按 ResultGroupNo 分组处理。
    /// 同组内的主项目和子项在同一事务中一起 commit，如果同组内任何一项 commit 失败，整组回滚。
    /// 不同组之间独立处理，某组失败不影响其他组。
    /// </para>
    /// </remarks>
    public async Task CommitAsync(PricingCommitRequest request)
    {
        // ========== 第零阶段：记录 commit 请求入口日志 ==========
        _logger.LogInformation(
            "COMMIT 开始 RequestId={RequestId}, ChargeNo={ChargeNo}",
            request.RequestId, request.ChargeNo);

        // ========== 第一阶段：事务保护状态推进 ==========
        // 请求日志、折价明细、限额占用三张表必须在同一事务内推进到 CONFIRMED。
        // 如果任何一步失败，整体回滚，避免报表看到"请求已确认但占额仍待确认"的断裂状态。
        await ExecuteInTransactionAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[] { BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new KeyNotFoundException($"请求不存在: {request.RequestId}");

            if (log.BusinessStatus == "CONFIRMED" || log.BusinessStatus == "COMMITTED")
            {
                if ((request.ActualItems?.Count ?? 0) > 0 || request.ActualTotalAmount.HasValue)
                {
                    var confirmedDetails = await _discountRepository.GetByRequestIdAsync(request.RequestId);
                    ValidateCommitActuals(request, confirmedDetails, requireActualItems: false);
                }

                _logger.LogInformation(
                    "COMMIT 幂等命中 RequestId={RequestId}, 当前状态={Status}",
                    request.RequestId, log.BusinessStatus);
                return;
            }

            // 只允许待确认保护状态 commit。已经取消、过期或确认的记录再 commit，
            // 通常代表渠道回调乱序或重复回调，必须暴露给调用方处理。
            if (log.BusinessStatus != "CONFIRM_PENDING")
            {
                _logger.LogWarning(
                    "COMMIT 状态校验失败 RequestId={RequestId}, 当前状态={Status}, 期望=CONFIRM_PENDING",
                    request.RequestId, log.BusinessStatus);
                throw new InvalidOperationException(
                    $"只有CONFIRM_PENDING状态可以COMMIT, 当前: {log.BusinessStatus}");
            }

            // confirm 结果有有效期。过期后额度可能已经释放给其他收费动作，
            // 因此不能再接受迟到 commit，调用方必须重新 confirm。
            if (DateTime.Now > log.RequestAt.AddMinutes(_options.ConfirmExpireMinutes))
            {
                _logger.LogWarning(
                    "COMMIT 已过期 RequestId={RequestId}, RequestAt={RequestAt}, 过期分钟数={ExpireMinutes}",
                    request.RequestId, log.RequestAt, _options.ConfirmExpireMinutes);
                throw new InvalidOperationException("EXPIRED: 确认计价结果已过期，请重新 confirm");
            }

            // commit 是 HIS 真正落账后的资金状态推进。推进前必须把 HIS 实际落账明细
            // 与 confirm 保存的折价明细逐项比对，避免 HIS 少收、多收或数量不一致后仍进入 CONFIRMED。
            var details = await _discountRepository.GetByRequestIdAsync(request.RequestId);
            ValidateCommitActuals(request, details, requireActualItems: true);

            // 请求日志进入 CONFIRMED，表示 HIS 已经完成本地落账。
            log.BusinessStatus = "CONFIRMED";
            log.ChargeNo = request.ChargeNo ?? log.ChargeNo;
            log.ResponseAt = DateTime.Now;
            await _requestLogRepository.UpdateAsync(log);

            // 折价明细和限额占用同步确认。后续窗口累计会把 CONFIRMED 记录计入净占用。
            await _discountRepository.UpdateStatusByRequestIdAsync(request.RequestId, "CONFIRMED");
            await _limitRepository.UpdateStatusByRequestIdAsync(request.RequestId, "CONFIRMED");

            _logger.LogInformation(
                "COMMIT 成功 RequestId={RequestId}, SourceSystem={SourceSystem}, ItemCode={ItemCode}, ChargeNo={ChargeNo}",
                request.RequestId, log.SourceSystem, log.ItemCode, log.ChargeNo);
        });
    }

    /// <summary>
    /// HIS 落账失败、支付失败或用户取消时释放 confirm 保护状态，主子项目原子释放。
    /// </summary>
    /// <param name="request">cancel 请求，必须携带 confirm 返回的请求 ID。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// <para>
    /// cancel 只允许处理 CONFIRM_PENDING。它的业务含义不是"退费"，而是"确认结果没有被 HIS 使用"，
    /// 因此应该释放待确认占额，并把折价明细标记为 CANCELLED，避免进入正式收费报表。
    /// </para>
    /// <para>
    /// 主子项目原子性保证：
    /// 同组内的限额占用和折价明细在同一事务中一起取消。
    /// 原子性保证：不会出现主项目取消但子项目残留的情况。
    /// </para>
    /// </remarks>
    public async Task CancelAsync(PricingCancelRequest request)
    {
        // ========== 第零阶段：记录 cancel 请求入口日志 ==========
        _logger.LogInformation(
            "CANCEL 开始 RequestId={RequestId}",
            request.RequestId);

        // ========== 第一阶段：事务保护取消 ==========
        // 与 commit 一样，cancel 也必须同步更新请求日志、折价明细和限额占用。
        // 否则会出现额度释放了但明细还在 PENDING，或明细取消了但额度仍占着的资金口径断裂。
        await ExecuteInTransactionAsync(async () =>
        {
            await _limitRepository.EnsureAndLockAsync(new[] { BuildRequestLockKey(request.RequestId) });

            var log = await _requestLogRepository.GetByIdAsync(request.RequestId)
                ?? throw new KeyNotFoundException($"请求不存在: {request.RequestId}");

            if (log.BusinessStatus == "CANCELLED" || log.BusinessStatus == "EXPIRED")
            {
                _logger.LogInformation(
                    "CANCEL 幂等命中 RequestId={RequestId}, 当前状态={Status}",
                    request.RequestId, log.BusinessStatus);
                return;
            }

            // 只有待落账的 confirm 可以取消。已 CONFIRMED 的记录代表 HIS 已经收费，
            // 应走 reverse，而不是 cancel。
            if (log.BusinessStatus != "CONFIRM_PENDING")
            {
                _logger.LogWarning(
                    "CANCEL 状态校验失败 RequestId={RequestId}, 当前状态={Status}, 期望=CONFIRM_PENDING",
                    request.RequestId, log.BusinessStatus);
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

            _logger.LogInformation(
                "CANCEL 成功 RequestId={RequestId}, SourceSystem={SourceSystem}, ItemCode={ItemCode}, 限额已释放",
                request.RequestId, log.SourceSystem, log.ItemCode);
        });
    }

    /// <summary>
    /// 对已经落账确认的计价结果执行冲正，主子项目原子处理。
    /// </summary>
    /// <param name="request">冲正请求，支持按收费明细、项目和片段定位部分退费。</param>
    /// <returns>异步任务。</returns>
    /// <remarks>
    /// <para>
    /// reverse 的语义不同于 cancel。cancel 处理"未落账的确认结果"，reverse 处理"已经落账的收费结果"。
    /// 部分退费必须校验"本次退费 + 历史已退 <= 原有效收费数量"。当日退费通过负向占额释放额度；
    /// 隔日退费只记录冲正事实，不回写历史窗口，重收时按重收当天业务时间重新校验。
    /// </para>
    /// <para>
    /// 主子项目原子性保证：
    /// 退费时按 ResultGroupNo 分组原子处理。校验：本次退费 + 历史已退不超过原有效收费（按组校验）。
    /// 同组内的主项目和子项一起冲正，不会出现主项目冲正成功但子项目残留的情况。
    /// </para>
    /// </remarks>
    public async Task ReverseAsync(PricingReverseRequest request)
    {
        // ========== 第零阶段：记录 reverse 请求入口日志 ==========
        _logger.LogInformation(
            "REVERSE 开始 OriginalRequestId={OriginalRequestId}, ItemCode={ItemCode}, ReverseQty={ReverseQty}",
            request.OriginalRequestId, request.ItemCode, request.ReverseQty);

        // ========== 第一阶段：事务保护冲正 ==========
        // 冲正会同时影响请求状态、折价明细、限额占用和冲正日志。任何一环失败都不能部分提交。
        await ExecuteInTransactionAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(request.ReverseNo))
            {
                throw new ArgumentException("REVERSE 必须传入稳定的 ReverseNo");
            }

            await _limitRepository.EnsureAndLockAsync(new[]
            {
                BuildRequestLockKey(request.OriginalRequestId),
                BuildReverseLockKey(request.OriginalRequestId, request.ReverseNo)
            });

            var log = await _requestLogRepository.GetByIdAsync(request.OriginalRequestId)
                ?? throw new KeyNotFoundException($"原请求不存在: {request.OriginalRequestId}");

            var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
            var sameReverseNo = reverseLogs.FirstOrDefault(r =>
                string.Equals(r.ReverseNo, request.ReverseNo, StringComparison.OrdinalIgnoreCase));
            if (sameReverseNo is not null)
            {
                if (!IsSameReverseRequest(sameReverseNo, request))
                {
                    throw new InvalidOperationException(
                        $"IDEMPOTENT_CONFLICT: ReverseNo={request.ReverseNo} 已存在，但本次冲正参数与首次请求不一致");
                }

                _logger.LogInformation(
                    "REVERSE 幂等命中 OriginalRequestId={OriginalRequestId}, ReverseNo={ReverseNo}",
                    request.OriginalRequestId, request.ReverseNo);
                return;
            }

            // 只有已落账确认的记录才能 reverse。CONFIRM_PENDING 应该 cancel；
            // CANCELLED/EXPIRED 没有形成正式收费，不应该再冲正。
            if (log.BusinessStatus != "CONFIRMED")
            {
                _logger.LogWarning(
                    "REVERSE 状态校验失败 OriginalRequestId={OriginalRequestId}, 当前状态={Status}, 期望=CONFIRMED",
                    request.OriginalRequestId, log.BusinessStatus);
                throw new InvalidOperationException(
                    $"只有CONFIRMED状态可以REVERSE, 当前: {log.BusinessStatus}");
            }

            // ========== 第二阶段：定位原折价明细并校验可退数量 ==========
            var details = await _discountRepository.GetByRequestIdAsync(request.OriginalRequestId);
            var matchedDetails = FilterReverseDetails(details, request);
            if (matchedDetails.Count == 0)
            {
                throw new InvalidOperationException("REVERSE_DETAIL_NOT_FOUND: 未找到可退费的原收费明细");
            }

            var allOriginalQty = details
                .Where(d => d.Status == "CONFIRMED" || d.Status == "COMMITTED")
                .Sum(d => d.FinalQty ?? 0);
            var originalQty = matchedDetails.Sum(d => d.FinalQty ?? 0);
            var originalAmt = matchedDetails.Sum(d => d.FinalAmt ?? 0);
            var reverseQty = request.ReverseQty ?? originalQty;
            if (reverseQty <= 0)
            {
                throw new InvalidOperationException("REVERSE_QTY_INVALID: 退费数量必须大于0");
            }

            var historicalReversedQty = await GetHistoricalReversedQtyAsync(request);
            var allHistoricalReversedQty = await GetHistoricalReversedQtyAsync(request.OriginalRequestId);
            if (historicalReversedQty + reverseQty > originalQty)
            {
                throw new InvalidOperationException(
                    $"REVERSE_QTY_EXCEEDED: 原有效数量={originalQty}, 历史已退={historicalReversedQty}, 本次退费={reverseQty}");
            }

            var reverseAmt = request.ReverseAmt ??
                (originalQty == 0 ? 0 : originalAmt * reverseQty / originalQty);
            reverseAmt = PricingAmountRounder.RoundFinalAmount(reverseAmt);
            var historicalReversedAmt = await GetHistoricalReversedAmtAsync(request);
            if (historicalReversedAmt + reverseAmt > originalAmt)
            {
                throw new InvalidOperationException(
                    $"REVERSE_AMT_EXCEEDED: 原有效金额={originalAmt}, 历史已退={historicalReversedAmt}, 本次退费={reverseAmt}");
            }

            // ========== 第二阶段半：按 ResultGroupNo 分组校验退费数量 ==========
            // 主子项目原子性要求：当原请求包含主子项目（通过 ResultGroupNo 关联）时，
            // 退费校验必须按组进行，确保同组内的退费数量不超过该组的原有效收费数量。
            // 这样可以防止"主项目退了但子项目没退"或"子项目退超了"的不一致情况。
            var groupedDetails = matchedDetails
                .Where(d => !string.IsNullOrWhiteSpace(d.ResultGroupNo))
                .GroupBy(d => d.ResultGroupNo)
                .ToList();
            foreach (var group in groupedDetails)
            {
                var groupOriginalQty = group.Sum(d => d.FinalQty ?? 0);
                var groupOriginalAmt = group.Sum(d => d.FinalAmt ?? 0);

                var groupHistoricalQty = reverseLogs
                    .Where(r => group.Any(d =>
                        string.Equals(d.ItemCode, r.ItemCode, StringComparison.OrdinalIgnoreCase) &&
                        d.ChargeDetailNo == r.ChargeDetailNo))
                    .Sum(r => r.ReverseQty ?? 0);
                var groupHistoricalAmt = reverseLogs
                    .Where(r => group.Any(d =>
                        string.Equals(d.ItemCode, r.ItemCode, StringComparison.OrdinalIgnoreCase) &&
                        d.ChargeDetailNo == r.ChargeDetailNo))
                    .Sum(r => r.ReverseAmt ?? 0);

                // 按组分配本次退费数量（按原始数量比例分摊）
                var groupRatio = originalQty == 0 ? 0 : groupOriginalQty / originalQty;
                var groupReverseQty = reverseQty * groupRatio;
                var groupReverseAmt = reverseAmt * groupRatio;

                if (groupHistoricalQty + groupReverseQty > groupOriginalQty)
                {
                    throw new InvalidOperationException(
                        $"REVERSE_GROUP_QTY_EXCEEDED: ResultGroupNo={group.Key}, 组原有效数量={groupOriginalQty}, 组历史已退={groupHistoricalQty}, 组本次退费={groupReverseQty}");
                }

                if (groupHistoricalAmt + groupReverseAmt > groupOriginalAmt)
                {
                    throw new InvalidOperationException(
                        $"REVERSE_GROUP_AMT_EXCEEDED: ResultGroupNo={group.Key}, 组原有效金额={groupOriginalAmt}, 组历史已退={groupHistoricalAmt}, 组本次退费={groupReverseAmt}");
                }
            }

            // ========== 第三阶段：根据全退或部分退费推进状态 ==========
            // 全退时原请求整体进入 REVERSED，旧占额不再参与累计；部分退费则保留原 CONFIRMED，
            // 并用负向占额扣减当前累计，避免把未退数量也释放掉。
            if (allHistoricalReversedQty + reverseQty == allOriginalQty)
            {
                log.BusinessStatus = "REVERSED";
                log.ResponseAt = DateTime.Now;
                await _requestLogRepository.UpdateAsync(log);

                await _discountRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, "REVERSED");
                await _limitRepository.UpdateStatusByRequestIdAsync(request.OriginalRequestId, "REVERSED");
            }

            // ========== 第四阶段：当日退费写负向占额 ==========
            var reverseTime = request.ReverseTime ?? DateTime.Now;
            var reverseRequestId = await SaveReverseRequestLogAsync(
                request,
                log,
                matchedDetails,
                reverseQty,
                reverseAmt,
                reverseTime);
            await InsertNegativeLimitOccupiesAsync(request, reverseQty, reverseAmt, reverseTime);

            // ========== 第五阶段：写冲正审计 ==========
            // 这张表回答"为什么原来的收费结果被冲掉"，也是后续财务追查和退费口径复盘的入口。
            await _reverseLogRepository.InsertAsync(new ChargeReverseLog
            {
                OriginalRequestId = request.OriginalRequestId,
                ReverseRequestId = reverseRequestId,
                ChargeNo = log.ChargeNo,
                ReverseNo = request.ReverseNo,
                ChargeDetailNo = NormalizeString(request.ChargeDetailNo),
                ItemCode = NormalizeString(request.ItemCode) ?? matchedDetails.FirstOrDefault()?.ItemCode ?? log.ItemCode,
                PartSeq = request.PartSeq,
                ReverseQty = reverseQty,
                ReverseAmt = reverseAmt,
                ReverseReason = request.Reason,
                ReversedBy = request.ReversedBy,
                ReversedAt = reverseTime
            });

            _logger.LogInformation(
                "REVERSE 成功 OriginalRequestId={OriginalRequestId}, ItemCode={ItemCode}, ReverseQty={ReverseQty}, ReverseAmt={ReverseAmt}, 全退={IsFullReverse}",
                request.OriginalRequestId, matchedDetails.FirstOrDefault()?.ItemCode,
                reverseQty, reverseAmt, allHistoricalReversedQty + reverseQty == allOriginalQty);
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

    private static void ValidateCommitActuals(
        PricingCommitRequest request,
        IReadOnlyList<ChargeDiscountDetail> details,
        bool requireActualItems)
    {
        if (details.Count == 0)
        {
            throw new InvalidOperationException(
                $"COMMIT_DETAIL_NOT_FOUND: RequestId={request.RequestId} 未找到 confirm 折价明细");
        }

        var expected = BuildExpectedCommitTotals(details);
        var expectedTotal = PricingAmountRounder.RoundFinalAmount(expected.Values.Sum(v => v.Amount));
        var actualItems = request.ActualItems ?? Array.Empty<PricingCommitActualItemRequest>();
        if (actualItems.Count == 0)
        {
            if (request.ActualTotalAmount.HasValue &&
                PricingAmountRounder.RoundFinalAmount(request.ActualTotalAmount.Value) != expectedTotal)
            {
                throw new InvalidOperationException(
                    $"COMMIT_AMOUNT_MISMATCH: confirm总金额={expectedTotal}, HIS实际总金额={PricingAmountRounder.RoundFinalAmount(request.ActualTotalAmount.Value)}");
            }

            if (requireActualItems)
            {
                throw new InvalidOperationException(
                    "COMMIT_ACTUAL_ITEMS_REQUIRED: commit 必须传入 HIS 实际落账明细");
            }

            return;
        }

        foreach (var item in actualItems)
        {
            if (string.IsNullOrWhiteSpace(item.ItemCode))
            {
                throw new InvalidOperationException("COMMIT_ACTUAL_ITEM_INVALID: 实际落账明细 ItemCode 不能为空");
            }
        }

        var actual = BuildActualCommitTotals(actualItems);
        var missingKeys = expected.Keys.Where(key => !actual.ContainsKey(key)).ToList();
        if (missingKeys.Count > 0)
        {
            throw new InvalidOperationException(
                $"COMMIT_DETAIL_MISMATCH: HIS 未回传实际落账明细 {string.Join(", ", missingKeys.Select(FormatCommitKey))}");
        }

        var extraKeys = actual.Keys.Where(key => !expected.ContainsKey(key)).ToList();
        if (extraKeys.Count > 0)
        {
            throw new InvalidOperationException(
                $"COMMIT_DETAIL_MISMATCH: HIS 回传了 confirm 未产生的落账明细 {string.Join(", ", extraKeys.Select(FormatCommitKey))}");
        }

        foreach (var pair in expected)
        {
            var actualValue = actual[pair.Key];
            if (Math.Round(actualValue.Qty, 4) != Math.Round(pair.Value.Qty, 4))
            {
                throw new InvalidOperationException(
                    $"COMMIT_QTY_MISMATCH: {FormatCommitKey(pair.Key)} confirm数量={Math.Round(pair.Value.Qty, 4)}, HIS实际数量={Math.Round(actualValue.Qty, 4)}");
            }

            var expectedAmount = PricingAmountRounder.RoundFinalAmount(pair.Value.Amount);
            var actualAmount = PricingAmountRounder.RoundFinalAmount(actualValue.Amount);
            if (actualAmount != expectedAmount)
            {
                throw new InvalidOperationException(
                    $"COMMIT_AMOUNT_MISMATCH: {FormatCommitKey(pair.Key)} confirm金额={expectedAmount}, HIS实际金额={actualAmount}");
            }
        }

        var actualTotal = PricingAmountRounder.RoundFinalAmount(actual.Values.Sum(v => v.Amount));
        if (actualTotal != expectedTotal)
        {
            throw new InvalidOperationException(
                $"COMMIT_AMOUNT_MISMATCH: confirm总金额={expectedTotal}, HIS实际明细合计={actualTotal}");
        }

        if (request.ActualTotalAmount.HasValue &&
            PricingAmountRounder.RoundFinalAmount(request.ActualTotalAmount.Value) != expectedTotal)
        {
            throw new InvalidOperationException(
                $"COMMIT_AMOUNT_MISMATCH: confirm总金额={expectedTotal}, HIS实际总金额={PricingAmountRounder.RoundFinalAmount(request.ActualTotalAmount.Value)}");
        }
    }

    private static Dictionary<CommitDetailKey, CommitDetailTotals> BuildExpectedCommitTotals(
        IReadOnlyList<ChargeDiscountDetail> details)
    {
        return details
            .GroupBy(d => BuildCommitDetailKey(d.ChargeDetailNo, d.ItemCode, d.PartSeq))
            .ToDictionary(
                group => group.Key,
                group => new CommitDetailTotals(
                    group.Sum(d => d.FinalQty ?? 0m),
                    group.Sum(d => d.FinalAmt ?? 0m)));
    }

    private static Dictionary<CommitDetailKey, CommitDetailTotals> BuildActualCommitTotals(
        IReadOnlyList<PricingCommitActualItemRequest> actualItems)
    {
        return actualItems
            .GroupBy(i => BuildCommitDetailKey(i.ChargeDetailNo, i.ItemCode, i.PartSeq))
            .ToDictionary(
                group => group.Key,
                group => new CommitDetailTotals(
                    group.Sum(i => i.FinalQty),
                    group.Sum(i => i.FinalAmount)));
    }

    private static CommitDetailKey BuildCommitDetailKey(
        string? chargeDetailNo,
        string? itemCode,
        int? partSeq)
    {
        return new CommitDetailKey(
            NormalizeString(chargeDetailNo)?.ToUpperInvariant() ?? string.Empty,
            NormalizeString(itemCode)?.ToUpperInvariant() ?? string.Empty,
            partSeq);
    }

    private static string FormatCommitKey(CommitDetailKey key)
    {
        var chargeDetailNo = string.IsNullOrWhiteSpace(key.ChargeDetailNo) ? "-" : key.ChargeDetailNo;
        var itemCode = string.IsNullOrWhiteSpace(key.ItemCode) ? "-" : key.ItemCode;
        var partSeq = key.PartSeq.HasValue ? key.PartSeq.Value.ToString() : "-";
        return $"ChargeDetailNo={chargeDetailNo}, ItemCode={itemCode}, PartSeq={partSeq}";
    }

    private static IReadOnlyList<ChargeDiscountDetail> FilterReverseDetails(
        IReadOnlyList<ChargeDiscountDetail> details,
        PricingReverseRequest request)
    {
        var query = details.Where(d => d.Status == "CONFIRMED" || d.Status == "COMMITTED");

        var chargeDetailNo = NormalizeString(request.ChargeDetailNo);
        if (!string.IsNullOrWhiteSpace(chargeDetailNo))
        {
            query = query.Where(d => string.Equals(
                d.ChargeDetailNo, chargeDetailNo, StringComparison.OrdinalIgnoreCase));
        }

        var itemCode = NormalizeString(request.ItemCode);
        if (!string.IsNullOrWhiteSpace(itemCode))
        {
            query = query.Where(d => string.Equals(
                d.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase));
        }

        if (request.PartSeq.HasValue)
        {
            query = query.Where(d => d.PartSeq == request.PartSeq);
        }

        return query.ToList();
    }

    private static bool IsSameReverseRequest(
        ChargeReverseLog existing,
        PricingReverseRequest request)
    {
        var requestChargeDetailNo = NormalizeString(request.ChargeDetailNo);
        if (requestChargeDetailNo is not null &&
            !string.Equals(
                NormalizeString(existing.ChargeDetailNo),
                requestChargeDetailNo,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var requestItemCode = NormalizeString(request.ItemCode);
        if (requestItemCode is not null &&
            !string.Equals(
                NormalizeString(existing.ItemCode),
                requestItemCode,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (existing.PartSeq != request.PartSeq)
        {
            return false;
        }

        if (request.ReverseQty.HasValue &&
            Math.Round(existing.ReverseQty ?? 0m, 4) != Math.Round(request.ReverseQty.Value, 4))
        {
            return false;
        }

        if (request.ReverseAmt.HasValue &&
            PricingAmountRounder.RoundFinalAmount(existing.ReverseAmt ?? 0m) !=
            PricingAmountRounder.RoundFinalAmount(request.ReverseAmt.Value))
        {
            return false;
        }

        return true;
    }

    private async Task<decimal> GetHistoricalReversedQtyAsync(PricingReverseRequest request)
    {
        var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
        var chargeDetailNo = NormalizeString(request.ChargeDetailNo);
        var itemCode = NormalizeString(request.ItemCode);

        return reverseLogs
            .Where(r => string.IsNullOrWhiteSpace(chargeDetailNo) ||
                        string.Equals(r.ChargeDetailNo, chargeDetailNo, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.IsNullOrWhiteSpace(itemCode) ||
                        string.Equals(r.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase))
            .Where(r => !request.PartSeq.HasValue || r.PartSeq == request.PartSeq)
            .Sum(r => r.ReverseQty ?? 0);
    }

    private async Task<decimal> GetHistoricalReversedQtyAsync(long originalRequestId)
    {
        var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(originalRequestId);
        return reverseLogs.Sum(r => r.ReverseQty ?? 0);
    }

    private async Task<decimal> GetHistoricalReversedAmtAsync(PricingReverseRequest request)
    {
        var reverseLogs = await _reverseLogRepository.GetByOriginalRequestIdAsync(request.OriginalRequestId);
        var chargeDetailNo = NormalizeString(request.ChargeDetailNo);
        var itemCode = NormalizeString(request.ItemCode);

        return reverseLogs
            .Where(r => string.IsNullOrWhiteSpace(chargeDetailNo) ||
                        string.Equals(r.ChargeDetailNo, chargeDetailNo, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.IsNullOrWhiteSpace(itemCode) ||
                        string.Equals(r.ItemCode, itemCode, StringComparison.OrdinalIgnoreCase))
            .Where(r => !request.PartSeq.HasValue || r.PartSeq == request.PartSeq)
            .Sum(r => r.ReverseAmt ?? 0);
    }

    private async Task<long> SaveReverseRequestLogAsync(
        PricingReverseRequest request,
        ChargeRequestLog originalLog,
        IReadOnlyList<ChargeDiscountDetail> matchedDetails,
        decimal reverseQty,
        decimal reverseAmt,
        DateTime reverseTime)
    {
        var firstDetail = matchedDetails.FirstOrDefault();
        var reverseRequestLog = new ChargeRequestLog
        {
            RequestNo = BuildReverseRequestNo(request.OriginalRequestId, request.ReverseNo!),
            BusinessRequestNo = NormalizeString(request.ReverseNo),
            RequestFingerprint = BuildReverseFingerprint(request, originalLog, reverseTime),
            TraceId = originalLog.TraceId,
            CallType = "REVERSE",
            BusinessStatus = "REVERSED",
            SourceSystem = NormalizeString(originalLog.SourceSystem) ?? "UNKNOWN",
            SourceTerminal = originalLog.SourceTerminal,
            PatientId = originalLog.PatientId,
            VisitId = originalLog.VisitId,
            ChargeScene = originalLog.ChargeScene,
            ChargeNo = originalLog.ChargeNo,
            ChargeDetailNo = NormalizeString(request.ChargeDetailNo) ?? firstDetail?.ChargeDetailNo ?? originalLog.ChargeDetailNo,
            ResultGroupNo = firstDetail?.ResultGroupNo ?? originalLog.ResultGroupNo,
            ItemCode = NormalizeString(request.ItemCode) ?? firstDetail?.ItemCode ?? originalLog.ItemCode,
            ItemName = firstDetail?.ItemName ?? originalLog.ItemName,
            InputQty = reverseQty,
            InputUnit = originalLog.InputUnit,
            BodyPartCode = originalLog.BodyPartCode,
            BusinessChargeTime = reverseTime,
            PriceVersion = originalLog.PriceVersion,
            RequestJson = JsonConvert.SerializeObject(request),
            ResponseJson = JsonConvert.SerializeObject(new
            {
                request.OriginalRequestId,
                request.ReverseNo,
                ReverseQty = reverseQty,
                ReverseAmt = reverseAmt
            }),
            RequestAt = DateTime.Now,
            ResponseAt = DateTime.Now,
            IsSuccess = "Y"
        };

        return await _requestLogRepository.InsertAsync(reverseRequestLog);
    }

    private async Task InsertNegativeLimitOccupiesAsync(
        PricingReverseRequest request,
        decimal reverseQty,
        decimal reverseAmt,
        DateTime reverseTime)
    {
        var originalOccupies = await _limitRepository.GetByRequestIdAsync(request.OriginalRequestId);
        var candidates = originalOccupies
            .Where(o => o.Status == "CONFIRMED")
            .Where(o => o.OccupyType == "CHARGE")
            .Where(o => string.IsNullOrWhiteSpace(request.ItemCode) ||
                        string.Equals(o.ItemCode, request.ItemCode, StringComparison.OrdinalIgnoreCase))
            .Where(o => !request.PartSeq.HasValue || o.PartSeq == request.PartSeq)
            .Where(o => IsSameBusinessDay(o.BusinessChargeTime, reverseTime))
            .ToList();

        var totalOriginalQty = candidates.Sum(o => Math.Abs(o.OccupyQty));
        if (totalOriginalQty <= 0)
        {
            return;
        }

        foreach (var occupy in candidates)
        {
            var ratio = Math.Abs(occupy.OccupyQty) / totalOriginalQty;
            var releaseQty = reverseQty * ratio;
            var releaseAmt = reverseAmt * ratio;
            await _limitRepository.InsertAsync(new LimitOccupy
            {
                RequestId = request.OriginalRequestId,
                TraceId = occupy.TraceId,
                PatientId = occupy.PatientId,
                ItemCode = occupy.ItemCode,
                RuleId = occupy.RuleId,
                RuleVersionNo = occupy.RuleVersionNo,
                LimitType = occupy.LimitType,
                LimitKey = occupy.LimitKey,
                LimitDimensionCode = occupy.LimitDimensionCode,
                OccupyQty = -releaseQty,
                OccupyAmt = -PricingAmountRounder.RoundFinalAmount(releaseAmt),
                OccupyType = "REVERSE",
                OriginalOccupyId = occupy.OccupyId,
                BusinessChargeTime = occupy.BusinessChargeTime,
                PartSeq = occupy.PartSeq,
                Status = "CONFIRMED",
                OccupiedAt = DateTime.Now,
                ConfirmedAt = DateTime.Now
            });

            // 记录限额释放日志，负向占用表示释放额度。
            _logger.LogInformation(
                "限额释放 OriginalRequestId={OriginalRequestId}, LimitType={LimitType}, LimitDimensionCode={LimitDimensionCode}, ReleaseQty={ReleaseQty}, ReleaseAmt={ReleaseAmt}",
                request.OriginalRequestId, occupy.LimitType, occupy.LimitDimensionCode, releaseQty, releaseAmt);
        }
    }

    private static bool IsSameBusinessDay(DateTime? originalBusinessTime, DateTime reverseTime)
    {
        return originalBusinessTime.HasValue &&
               originalBusinessTime.Value.Date == reverseTime.Date;
    }

    private async Task ValidateAuthorityPriceAsync(IReadOnlyList<PricingCalculateItemRequest> items)
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
        foreach (var item in items)
        {
            var authorityPrice = await _priceMasterRepository.GetUnitPriceAsync(item.ItemCode);
            if (!authorityPrice.HasValue)
            {
                // 找不到权威单价时不能"按渠道传入价格先算"，否则统一计价中心会变成错误价格的放大器。
                _logger.LogWarning(
                    "权威单价校验失败: 未找到项目权威单价 ItemCode={ItemCode}",
                    item.ItemCode);
                throw new InvalidOperationException(
                    $"PRICE_MISMATCH: 未找到项目 {item.ItemCode} 的权威单价");
            }

            // ========== 第三阶段：按金额精度比较 ==========
            // Oracle 表和 C# 都以 4 位小数为当前金额精度，因此比较前先统一 round 到 4 位。
            if (Math.Round(authorityPrice.Value, 4) != Math.Round(item.UnitPrice, 4))
            {
                _logger.LogWarning(
                    "权威单价校验失败: 单价不一致 ItemCode={ItemCode}, AuthorityPrice={AuthorityPrice}, RequestPrice={RequestPrice}",
                    item.ItemCode, authorityPrice.Value, item.UnitPrice);
                throw new InvalidOperationException(
                    $"PRICE_MISMATCH: 项目 {item.ItemCode} 权威单价={authorityPrice.Value}, 请求单价={item.UnitPrice}");
            }
        }
    }

    private static PricingContext BuildContext(
        PricingCalculateRequest request,
        PricingCalculateItemRequest item,
        string callType,
        bool shouldLockLimits,
        IReadOnlyDictionary<string, decimal>? inRequestOccupiedQtyByLimitDimension = null,
        IReadOnlyList<LimitOccupy>? inRequestLimitOccupies = null)
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
            ItemCode = item.ItemCode.Trim(),
            ItemName = NormalizeString(item.ItemName),
            InputQty = item.InputQty,
            Unit = NormalizeString(item.Unit),
            UnitPrice = item.UnitPrice,
            BodyPartCode = NormalizeString(item.BodyPartCode),
            ChargeScene = NormalizeString(request.ChargeScene),
            ItemGroupCode = NormalizeString(item.ItemGroupCode),
            VisitType = NormalizeString(request.VisitType),
            PatientAge = request.PatientAge,
            BusinessChargeTime = item.BusinessChargeTime ?? request.BusinessChargeTime,
            SourceSystem = request.SourceSystem.Trim(),
            ChargeNo = NormalizeString(request.ChargeNo),
            BusinessRequestNo = NormalizeString(request.BusinessRequestNo),
            ChargeDeptCode = NormalizeString(request.ChargeDeptCode),
            LegacyOccupiedQty = item.LegacyOccupiedQty ?? 0m,
            ExtraParams = MergeExtraParams(request.ExtraParams, item.ExtraParams),
            InRequestOccupiedQtyByLimitDimension =
                inRequestOccupiedQtyByLimitDimension?.ToDictionary(
                    item => item.Key,
                    item => item.Value,
                    StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, decimal>(),
            InRequestLimitOccupies = inRequestLimitOccupies is null
                ? Array.Empty<LimitOccupy>()
                : inRequestLimitOccupies.ToList(),
            PricingParts = item.PricingParts?.Select(p => new PricingPartItem
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

    private static IReadOnlyDictionary<string, string>? MergeExtraParams(
        IReadOnlyDictionary<string, object?>? requestParams,
        IReadOnlyDictionary<string, object?>? itemParams)
    {
        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        AddExtraParams(merged, requestParams);
        AddExtraParams(merged, itemParams);
        return merged.Count == 0 ? null : merged;
    }

    private static void AddExtraParams(
        Dictionary<string, string> target,
        IReadOnlyDictionary<string, object?>? source)
    {
        if (source is null)
        {
            return;
        }

        foreach (var pair in source)
        {
            var key = NormalizeString(pair.Key);
            if (key is null)
            {
                continue;
            }

            var normalizedValue = NormalizeExtraValue(pair.Value);
            var textValue = NormalizeString(normalizedValue?.ToString());
            if (textValue is not null)
            {
                target[key] = textValue;
            }
        }
    }

    private async Task<ChargeRequestLog> SaveRequestLog(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items,
        IReadOnlyList<ItemPricingCalculation> calculations,
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
            ChargeDetailNo = items.Count == 1 ? NormalizeString(items[0].ChargeDetailNo) : null,
            ItemCode = items.Count == 1 ? items[0].ItemCode.Trim() : null,
            ItemName = items.Count == 1 ? NormalizeString(items[0].ItemName) : null,
            InputQty = items.Count == 1 ? items[0].InputQty : null,
            InputUnit = items.Count == 1 ? NormalizeString(items[0].Unit) : null,
            BodyPartCode = items.Count == 1 ? NormalizeString(items[0].BodyPartCode) : null,
            BusinessChargeTime = items.Count == 1
                ? items[0].BusinessChargeTime ?? request.BusinessChargeTime
                : request.BusinessChargeTime,
            RequestJson = JsonConvert.SerializeObject(request),
            ResponseJson = JsonConvert.SerializeObject(calculations.Select(c => c.Result).ToList()),
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

    private static void AccumulateInRequestLimits(
        Dictionary<string, decimal> inRequestOccupiedQtyByLimitDimension,
        List<LimitOccupy> inRequestLimitOccupies,
        PricingResult result)
    {
        foreach (var occupy in result.LimitOccupies.Where(o =>
                     !string.IsNullOrWhiteSpace(o.LimitType) &&
                     !string.IsNullOrWhiteSpace(o.LimitDimensionCode)))
        {
            var key = BuildInRequestLimitKey(occupy.LimitType, occupy.LimitDimensionCode);
            inRequestOccupiedQtyByLimitDimension.TryGetValue(key, out var existingQty);
            inRequestOccupiedQtyByLimitDimension[key] = existingQty + occupy.OccupyQty;
            inRequestLimitOccupies.Add(occupy);
        }
    }

    private static string BuildInRequestLimitKey(string limitType, string? limitDimensionCode)
    {
        return $"{limitType.Trim().ToUpperInvariant()}:{limitDimensionCode?.Trim().ToUpperInvariant()}";
    }

    private static string BuildIdempotencyLockKey(
        string sourceSystem,
        string businessRequestNo,
        string callType)
    {
        return $"IDEMP:{sourceSystem.Trim()}:{businessRequestNo.Trim()}:{callType.Trim()}"
            .ToUpperInvariant();
    }

    internal static string BuildRequestLockKey(long requestId)
    {
        return $"REQ:{requestId}".ToUpperInvariant();
    }

    private static string BuildReverseLockKey(long originalRequestId, string reverseNo)
    {
        return $"REV:{originalRequestId}:{reverseNo.Trim()}".ToUpperInvariant();
    }

    private static string BuildReverseRequestNo(long originalRequestId, string reverseNo)
    {
        var raw = $"{originalRequestId}:{reverseNo.Trim()}";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)))[..16];
        return $"REV-{originalRequestId}-{hash}";
    }

    private async Task SaveTraceSteps(long requestId, IReadOnlyList<ItemPricingCalculation> calculations)
    {
        // 没有命中特殊规则时可能没有步骤。这里直接返回，避免写空集合导致不必要的数据库调用。
        var steps = calculations
            .SelectMany(c => c.Result.TraceSteps.Select(s => (c.Item, Step: s)))
            .ToList();
        if (steps.Count == 0)
        {
            return;
        }

        // 步骤日志只保存 DDL 允许的 StepType。ActionExecutionPipeline 已经把具体动作类型映射为
        // MATCH/FORMULA/LIMIT/DISCOUNT 等稳定类别，避免违反数据库 CHECK 约束。
        var stepNo = 1;
        var entities = steps.Select(s => new ChargeTraceStep
        {
            RequestId = requestId,
            StepNo = stepNo++,
            StepName = s.Step.StepType,
            StepType = s.Step.StepType,
            InputSnapshot = s.Step.InputValue?.ToString(),
            OutputSnapshot = s.Step.OutputValue?.ToString(),
            StepDesc = $"{s.Item.ItemCode}: {s.Step.StepDesc}",
            CreatedAt = DateTime.Now
        }).ToList();

        await _traceStepRepository.InsertBatchAsync(entities);
    }

    private async Task SaveDiscountDetail(
        long requestId,
        PricingCalculateRequest request,
        PricingCalculateItemRequest item,
        PricingResult result,
        string status)
    {
        // ========== 第一阶段：选择主命中规则 ==========
        // 当前折价明细表只有一个 RULE_ID 字段。多规则叠加时这里先记录第一条命中规则，
        // 完整动作链仍通过步骤日志和请求响应快照追溯。
        var firstRuleId = result.MatchedRuleIds.FirstOrDefault();
        var now = DateTime.Now;
        var hasChildItems = result.ChildPricingResults.Count > 0;
        var resultGroupNo = result.ReplaceChildResult is null && !hasChildItems
            ? null
            : BuildResultGroupNo(requestId, item, result.ReplaceChildResult is not null ? "REPLACE" : "CHILD");

        // ========== 第二阶段：保存待确认或最终折价明细 ==========
        // confirm 阶段写 PENDING，commit 后再统一改 CONFIRMED。这样未落账的结果不会进入正式报表。
        var replacementAmt = result.ReplaceChildResult is null
            ? 0m
            : PricingAmountRounder.RoundFinalAmount(result.ReplaceChildResult.Amount);
        var mainFinalAmt = result.ReplaceChildResult is null
            ? result.FinalAmount
            : Math.Max(result.FinalAmount - replacementAmt, 0m);
        var mainDiscountAmt = PricingAmountRounder.RoundFinalAmount(
            item.UnitPrice * item.InputQty - mainFinalAmt);

        var detail = new ChargeDiscountDetail
        {
            RequestId = requestId,
            ChargeNo = NormalizeString(request.ChargeNo),
            ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
            PatientId = request.PatientId,
            VisitId = request.VisitId,
            ItemCode = item.ItemCode,
            ItemName = item.ItemName,
            RuleId = firstRuleId == 0 ? null : firstRuleId,
            ResultGroupNo = resultGroupNo,
            OriginalQty = item.InputQty,
            ConvertedQty = result.ConvertedQty,
            FinalQty = result.FinalQty,
            UnitPrice = result.UnitPrice,
            OriginalAmt = PricingAmountRounder.RoundFinalAmount(item.UnitPrice * item.InputQty),
            CalculatedAmt = PricingAmountRounder.RoundFinalAmount(mainFinalAmt),
            FinalAmt = PricingAmountRounder.RoundFinalAmount(mainFinalAmt),
            DiscountAmt = mainDiscountAmt,
            DiscountType = result.ReplaceChildResult is null ? null : "EXCESS_REPLACE",
            ReasonCode = result.ReplaceChildResult is null ? null : "EXCESS_REPLACE",
            ReasonDesc = result.ReplaceChildResult is null ? null : BuildReasonDesc(result),
            Status = status,
            OccurredAt = now
        };

        var mainDiscountId = await _discountRepository.InsertAsync(detail);

        if (result.ReplaceChildResult is null)
        {
            await SaveChildDiscountDetails(
                requestId,
                request,
                item,
                result,
                resultGroupNo,
                mainDiscountId,
                firstRuleId,
                status,
                now);
            return;
        }

        var replacement = result.ReplaceChildResult;
        var replacementDetail = new ChargeDiscountDetail
        {
            RequestId = requestId,
            ChargeNo = NormalizeString(request.ChargeNo),
            ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
            PatientId = request.PatientId,
            VisitId = request.VisitId,
            ItemCode = replacement.ItemCode,
            ItemName = replacement.ItemName,
            RuleId = firstRuleId == 0 ? null : firstRuleId,
            ResultGroupNo = resultGroupNo,
            ParentDiscountId = mainDiscountId,
            ConvertedQty = replacement.Qty,
            FinalQty = replacement.Qty,
            UnitPrice = replacement.UnitPrice,
            OriginalAmt = 0m,
            CalculatedAmt = replacementAmt,
            FinalAmt = replacementAmt,
            DiscountAmt = -replacementAmt,
            DiscountType = "EXCESS_REPLACE",
            ReasonCode = "EXCESS_REPLACE",
            ReasonDesc = BuildReplacementReasonDesc(item, replacement),
            Status = status,
            OccurredAt = now
        };

        await _discountRepository.InsertAsync(replacementDetail);

        await SaveChildDiscountDetails(
            requestId,
            request,
            item,
            result,
            resultGroupNo,
            mainDiscountId,
            firstRuleId,
            status,
            now);
    }

    private async Task SaveChildDiscountDetails(
        long requestId,
        PricingCalculateRequest request,
        PricingCalculateItemRequest item,
        PricingResult result,
        string? resultGroupNo,
        long mainDiscountId,
        long firstRuleId,
        string status,
        DateTime now)
    {
        foreach (var child in result.ChildPricingResults)
        {
            var childAmount = PricingAmountRounder.RoundFinalAmount(child.Amount);
            var childDetail = new ChargeDiscountDetail
            {
                RequestId = requestId,
                ChargeNo = NormalizeString(request.ChargeNo),
                ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
                PatientId = request.PatientId,
                VisitId = request.VisitId,
                ItemCode = child.ItemCode,
                ItemName = child.ItemName,
                RuleId = firstRuleId == 0 ? null : firstRuleId,
                ResultGroupNo = resultGroupNo,
                ParentDiscountId = mainDiscountId,
                ConvertedQty = child.Qty,
                FinalQty = child.Qty,
                UnitPrice = child.UnitPrice,
                OriginalAmt = 0m,
                CalculatedAmt = childAmount,
                FinalAmt = childAmount,
                DiscountAmt = -childAmount,
                DiscountType = "ADD_CHILD_ITEM",
                ReasonCode = "ADD_CHILD_ITEM",
                ReasonDesc = BuildChildReasonDesc(item, child),
                Status = status,
                OccurredAt = now
            };

            await _discountRepository.InsertAsync(childDetail);
        }
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

            // 记录限额占用日志，包含占用维度和数量，便于排查额度超限问题。
            _logger.LogDebug(
                "限额占用 RequestId={RequestId}, LimitType={LimitType}, LimitDimensionCode={LimitDimensionCode}, OccupyQty={OccupyQty}, ItemCode={ItemCode}",
                requestId, occupy.LimitType, occupy.LimitDimensionCode, occupy.OccupyQty, occupy.ItemCode);
        }
    }

    private static PricingCalculateResponse BuildResponse(
        long requestId,
        IReadOnlyList<ItemPricingCalculation> calculations,
        DateTime? expireAt = null)
    {
        // 响应 DTO 只返回渠道需要使用或展示的字段；更完整的内部计算状态留在追溯日志和请求快照里。
        var itemResponses = calculations
            .Select(c => BuildItemResponse(requestId, c.Item, c.Result))
            .ToList();
        var first = itemResponses.FirstOrDefault();
        var expireSeconds = expireAt.HasValue
            ? Math.Max(0, (int)Math.Ceiling((expireAt.Value - DateTime.Now).TotalSeconds))
            : (int?)null;

        return new PricingCalculateResponse
        {
            RequestId = requestId,
            Items = itemResponses,
            IsSpecialItem = itemResponses.Any(i => i.IsSpecialItem),
            InputQty = itemResponses.Sum(i => i.InputQty),
            FinalQty = itemResponses.Sum(i => i.FinalQty),
            UnitPrice = itemResponses.Count == 1 ? first?.UnitPrice ?? 0 : 0,
            FinalAmount = itemResponses.Sum(i => i.FinalAmount),
            DiscountAmount = itemResponses.Sum(i => i.DiscountAmount),
            ExpireAt = expireAt,
            ExpireSeconds = expireSeconds,
            TraceSteps = itemResponses.Count == 1 ? first?.TraceSteps ?? Array.Empty<PricingTraceStepResponse>() : Array.Empty<PricingTraceStepResponse>(),
            MatchedRuleIds = itemResponses.SelectMany(i => i.MatchedRuleIds).Distinct().ToList()
        };
    }

    private static string BuildResultGroupNo(long requestId, PricingCalculateItemRequest item, string groupType)
    {
        var chargeDetailNo = NormalizeString(item.ChargeDetailNo) ?? "NO_DETAIL";
        var itemRequestNo = NormalizeString(item.ItemRequestNo) ?? "NO_ITEM_REQUEST";
        var rawKey = $"{item.ItemCode.Trim()}:{chargeDetailNo}:{itemRequestNo}".ToUpperInvariant();
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)))[..12];
        return $"{groupType}:{requestId}:{hash}";
    }

    private static string BuildReasonDesc(PricingResult result)
    {
        if (result.ReplaceChildResult is null)
        {
            return $"超出限额数量 {result.ExceedQty}，超出部分归零";
        }

        return $"超出限额数量 {result.ExceedQty}，替换为 {result.ReplaceChildResult.ItemCode} " +
               $"{result.ReplaceChildResult.ItemName}，数量 {result.ReplaceChildResult.Qty}，金额 {result.ReplaceChildResult.Amount}";
    }

    private static string BuildReplacementReasonDesc(
        PricingCalculateItemRequest item,
        ReplaceChildResult replacement)
    {
        return $"主项目 {item.ItemCode} 超限后替换为 {replacement.ItemCode} " +
               $"{replacement.ItemName}，数量 {replacement.Qty}，金额 {replacement.Amount}";
    }

    private static string BuildChildReasonDesc(
        PricingCalculateItemRequest item,
        ChildPricingResult child)
    {
        return $"主项目 {item.ItemCode} 自动加收子项目 {child.ItemCode} " +
               $"{child.ItemName}，数量 {child.Qty}，金额 {child.Amount}";
    }

    private static PricingCalculateItemResponse BuildItemResponse(
        long requestId,
        PricingCalculateItemRequest item,
        PricingResult result)
    {
        return new PricingCalculateItemResponse
        {
            ItemRequestNo = NormalizeString(item.ItemRequestNo),
            ChargeDetailNo = NormalizeString(item.ChargeDetailNo),
            RequestId = requestId,
            ItemCode = item.ItemCode.Trim(),
            ItemName = NormalizeString(item.ItemName),
            IsSpecialItem = result.IsSpecialItem,
            InputQty = result.InputQty,
            FinalQty = result.FinalQty,
            ConvertedQty = result.ConvertedQty,
            UnitPrice = result.UnitPrice,
            FinalAmount = PricingAmountRounder.RoundFinalAmount(
                result.FinalAmount + result.ChildPricingResults.Sum(c => c.Amount)),
            DiscountAmount = PricingAmountRounder.RoundFinalAmount(
                result.DiscountAmount - result.ChildPricingResults.Sum(c => c.Amount)),
            ExceedQty = result.ExceedQty,
            ReplacementItem = result.ReplaceChildResult is null
                ? null
                : new PricingReplacementItemResponse
                {
                    ItemCode = result.ReplaceChildResult.ItemCode,
                    ItemName = NormalizeString(result.ReplaceChildResult.ItemName),
                    Qty = result.ReplaceChildResult.Qty,
                    UnitPrice = result.ReplaceChildResult.UnitPrice,
                    Amount = PricingAmountRounder.RoundFinalAmount(result.ReplaceChildResult.Amount)
                },
            ChildItems = result.ChildPricingResults.Select(c => new PricingChildItemResponse
            {
                ItemCode = c.ItemCode,
                ItemName = NormalizeString(c.ItemName),
                Qty = c.Qty,
                UnitPrice = c.UnitPrice,
                Amount = PricingAmountRounder.RoundFinalAmount(c.Amount),
                ShareParentLimit = c.ShareParentLimit
            }).ToList(),
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
        var detail = details.FirstOrDefault(d => d.ParentDiscountId is null) ?? details.FirstOrDefault();
        var replacement = detail is null
            ? null
            : details.FirstOrDefault(d => d.ParentDiscountId == detail.DiscountId);
        var expireAt = log.BusinessStatus == "CONFIRM_PENDING"
            ? log.RequestAt.AddMinutes(_options.ConfirmExpireMinutes)
            : (DateTime?)null;
        var expireSeconds = expireAt.HasValue
            ? Math.Max(0, (int)Math.Ceiling((expireAt.Value - DateTime.Now).TotalSeconds))
            : (int?)null;

        return new PricingCalculateResponse
        {
            RequestId = log.RequestId,
            IsSpecialItem = detail is not null,
            InputQty = log.InputQty ?? 0,
            FinalQty = detail?.FinalQty ?? 0,
            UnitPrice = detail?.UnitPrice ?? 0,
            FinalAmount = (detail?.FinalAmt ?? 0) + (replacement?.FinalAmt ?? 0),
            DiscountAmount = (detail?.DiscountAmt ?? 0) + (replacement?.DiscountAmt ?? 0),
            ExpireAt = expireAt,
            ExpireSeconds = expireSeconds,
            Items = detail is null
                ? Array.Empty<PricingCalculateItemResponse>()
                : new[]
                {
                    new PricingCalculateItemResponse
                    {
                        RequestId = log.RequestId,
                        ItemCode = detail.ItemCode ?? string.Empty,
                        ItemName = detail.ItemName,
                        IsSpecialItem = true,
                        InputQty = detail.OriginalQty ?? 0,
                        ConvertedQty = detail.ConvertedQty ?? detail.FinalQty ?? 0,
                        FinalQty = detail.FinalQty ?? 0,
                        UnitPrice = detail.UnitPrice ?? 0,
                        FinalAmount = (detail.FinalAmt ?? 0) + (replacement?.FinalAmt ?? 0),
                        DiscountAmount = (detail.DiscountAmt ?? 0) + (replacement?.DiscountAmt ?? 0),
                        ExceedQty = replacement?.FinalQty ?? Math.Max((detail.ConvertedQty ?? 0) - (detail.FinalQty ?? 0), 0m),
                        ReplacementItem = replacement is null
                            ? null
                            : new PricingReplacementItemResponse
                            {
                                ItemCode = replacement.ItemCode ?? string.Empty,
                                ItemName = replacement.ItemName,
                                Qty = replacement.FinalQty ?? 0,
                                UnitPrice = replacement.UnitPrice ?? 0,
                                Amount = replacement.FinalAmt ?? 0
                            }
                    }
                },
            TraceSteps = Array.Empty<PricingTraceStepResponse>(),
            MatchedRuleIds = detail?.RuleId is long ruleId ? new[] { ruleId } : Array.Empty<long>()
        };
    }

    private async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        // ========== 第一阶段：开启数据库事务 ==========
        // 本服务的大部分资金操作会同时更新多张表。使用同一个 SqlSugarClient 事务可以保证
        // 请求日志、折价明细、限额占用、冲正日志之间不会出现半提交。
        try
        {
            if (_db is not null)
            {
                await _db.Ado.BeginTranAsync();
            }
            var result = await action();
            // ========== 第二阶段：全部成功后提交 ==========
            // 只有所有仓储操作都成功，才允许对外暴露本次状态推进。
            if (_db is not null)
            {
                await _db.Ado.CommitTranAsync();
            }
            return result;
        }
        catch (Exception ex)
        {
            // ========== 第三阶段：任何异常都回滚 ==========
            // 资金链路宁可失败返回给渠道重试，也不能留下部分写入的请求或占额。
            _logger.LogError(ex, "事务执行异常，已回滚");
            if (_db is not null)
            {
                await _db.Ado.RollbackTranAsync();
            }
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

    private static string BuildFingerprint(
        PricingCalculateRequest request,
        IReadOnlyList<PricingCalculateItemRequest> items,
        string callType)
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
            visitType = NormalizeString(request.VisitType),
            patientAge = request.PatientAge,
            encounterNo = NormalizeString(request.EncounterNo),
            chargeScene = NormalizeString(request.ChargeScene),
            chargeNo = NormalizeString(request.ChargeNo),
            chargeTime = request.BusinessChargeTime,
            operationNo = GetExtraParam(request.ExtraParams, "operationNo"),
            pregnancyNo = GetExtraParam(request.ExtraParams, "pregnancyNo"),
            mainChargeDetailNo = GetExtraParam(request.ExtraParams, "mainChargeDetailNo"),
            extraParams = NormalizeExtraParams(request.ExtraParams),
            items = items
                .OrderBy(i => i.ChargeDetailNo)
                .ThenBy(i => i.ItemRequestNo)
                .ThenBy(i => i.ItemCode)
                .Select(i => new
                {
                    itemRequestNo = NormalizeString(i.ItemRequestNo),
                    chargeDetailNo = NormalizeString(i.ChargeDetailNo),
                    itemCode = NormalizeString(i.ItemCode),
                    itemName = NormalizeString(i.ItemName),
                    itemGroupCode = NormalizeString(i.ItemGroupCode),
                    inputQty = Math.Round(i.InputQty, 4),
                    inputUnit = NormalizeString(i.Unit),
                    unitPrice = Math.Round(i.UnitPrice, 4),
                    chargeTime = i.BusinessChargeTime ?? request.BusinessChargeTime,
                    bodyPartCode = NormalizeString(i.BodyPartCode),
                    operationNo = GetExtraParam(i.ExtraParams, "operationNo"),
                    pregnancyNo = GetExtraParam(i.ExtraParams, "pregnancyNo"),
                    mainChargeDetailNo = GetExtraParam(i.ExtraParams, "mainChargeDetailNo"),
                    extraParams = NormalizeExtraParams(i.ExtraParams),
                    pricingParts = i.PricingParts?
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
                })
                .ToList()
        };

        // ========== 第二阶段：序列化后计算 SHA256 ==========
        // 存 hash 而不是完整 JSON，可以控制 REQUEST_FINGERPRINT 字段长度，同时避免数据库索引过大。
        var json = JsonConvert.SerializeObject(payload, Formatting.None);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash);
    }

    private static string BuildReverseFingerprint(
        PricingReverseRequest request,
        ChargeRequestLog originalLog,
        DateTime reverseTime)
    {
        var payload = new
        {
            sourceSystem = NormalizeString(originalLog.SourceSystem),
            callType = "REVERSE",
            originalRequestId = request.OriginalRequestId,
            reverseNo = NormalizeString(request.ReverseNo),
            chargeNo = NormalizeString(originalLog.ChargeNo),
            chargeDetailNo = NormalizeString(request.ChargeDetailNo),
            itemCode = NormalizeString(request.ItemCode),
            partSeq = request.PartSeq,
            reverseTime,
            reverseQty = request.ReverseQty.HasValue ? Math.Round(request.ReverseQty.Value, 4) : (decimal?)null,
            reverseAmt = request.ReverseAmt.HasValue ? PricingAmountRounder.RoundFinalAmount(request.ReverseAmt.Value) : (decimal?)null,
            reversedBy = NormalizeString(request.ReversedBy),
            reason = NormalizeString(request.Reason)
        };

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

    private static object? GetExtraParam(IReadOnlyDictionary<string, object?>? extraParams, string key)
    {
        if (extraParams is null ||
            !extraParams.TryGetValue(key, out var value))
        {
            return null;
        }

        return NormalizeExtraValue(value);
    }

    private static IReadOnlyList<PricingCalculateItemRequest> GetRequiredItems(
        PricingCalculateRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException("费用明细不能为空");
        }

        foreach (var item in request.Items)
        {
            if (string.IsNullOrWhiteSpace(item.ItemCode))
            {
                throw new ArgumentException("费用明细项目编码不能为空");
            }
        }

        return request.Items;
    }

    private static string? NormalizeString(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
