using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// HIS 集成辅助类。
    /// 封装 HIS 客户端与统一计价服务交互的高层操作，供 HIS 收费模块直接调用。
    /// 主要职责：
    /// 1. 特殊项目标识查询（收费前快速判断是否需要弹窗）
    /// 2. 计价弹窗展示与结果获取
    /// 3. HIS 落账成功/失败后的 commit/cancel 通知
    /// 4. 计价请求对象的便捷构建
    ///
    /// 设计说明：
    /// - 此类是 HIS 收费流程与计价服务之间的适配层，HIS 开发者无需了解 API 细节
    /// - 所有资金安全约束（幂等、额度、回滚）由此类编排，调用方只需关心业务流程
    /// </summary>
    public sealed class PricingHisIntegrationHelper
    {
        /// <summary>
        /// 统一计价服务 HTTP 客户端。所有与计价服务的通信均通过此对象，
        /// 由调用方在构造时注入。注入方式确保 HIS 全局共享同一客户端实例（含连接配置、超时、重试策略）。
        /// </summary>
        private readonly PricingApiClient _client;

        /// <summary>
        /// 构造 HIS 集成辅助类实例。
        /// </summary>
        /// <param name="client">统一计价服务 HTTP 客户端，不可为 null</param>
        public PricingHisIntegrationHelper(PricingApiClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
        }

        /// <summary>
        /// 检查指定项目是否为特殊计价项目。
        /// HIS 收费时在录入每个项目后调用此方法，根据返回的决策结果：
        /// - AllowOrdinary：非特殊项目，按普通流程计价
        /// - RequirePopup：特殊项目，必须打开计价弹窗
        /// - BlockAsServiceUnavailable：服务不可用，必须阻断收费（禁止回退为普通计价）
        ///
        /// 资金安全约束：计价服务不可用时，渠道不得回退为普通计价。
        /// 这是为了防止特殊项目在服务抖动时被按原价收费，造成医院资金损失。
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <returns>特殊计价决策结果，包含是否特殊、是否允许普通计价、是否需要弹窗等信息</returns>
        public SpecialPricingDecision CheckSpecialPricingRequired(string itemCode)
        {
            // 项目编码为空时按普通流程处理（防御性逻辑，不应发生在正常业务中）
            if (string.IsNullOrEmpty(itemCode))
            {
                return SpecialPricingDecision.AllowOrdinary("项目编码为空，按原流程处理。");
            }

            try
            {
                // ========== 第1阶段：调用特殊标识查询接口 ==========
                ApiResponse<SpecialFlagResponse> response = _client.GetSpecialFlag(itemCode);

                // 查询失败（非 200 或业务码非 0）时阻断收费，不回退普通计价
                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    return SpecialPricingDecision.BlockAsServiceUnavailable(
                        "特殊计价标识查询失败，禁止按普通计价继续收费。");
                }

                // ========== 第2阶段：根据标识返回决策 ==========
                if (response.Data.IsSpecial)
                {
                    return SpecialPricingDecision.RequirePopup("命中特殊计价项目。");
                }

                return SpecialPricingDecision.AllowOrdinary("非特殊计价项目。");
            }
            catch (Exception ex)
            {
                // 网络异常、超时等情况同样阻断收费
                return SpecialPricingDecision.BlockAsServiceUnavailable(
                    "计价服务暂时不可用，禁止按普通计价继续收费：" + ex.Message);
            }
        }

        /// <summary>
        /// 展示特殊计价确认弹窗并返回结果。
        /// 弹窗内部会自动执行试算，用户确认后执行确认计价（占用额度）。
        /// 调用方通过 PricingPopupResult.Confirmed 判断用户是否确认。
        /// </summary>
        /// <param name="owner">弹窗所属父窗口，用于模态展示</param>
        /// <param name="request">计价请求上下文</param>
        /// <returns>弹窗结果，包含确认状态、响应数据和 RequestId</returns>
        public PricingPopupResult ShowPricingPopup(
            IWin32Window owner,
            PricingCalculateRequest request)
        {
            FrmPricingPopup popup = new FrmPricingPopup(_client, request);
            DialogResult result = popup.ShowDialog(owner);
            if (result != DialogResult.OK)
            {
                return PricingPopupResult.Cancelled();
            }

            return PricingPopupResult.FromConfirmed(popup.ConfirmedResponse, popup.ConfirmedRequestId);
        }

        /// <summary>
        /// HIS 落账成功后通知计价服务提交（commit）。
        /// commit 将 confirm 阶段占用的额度正式消费，状态从 CONFIRM_PENDING 流转为 CONFIRMED。
        /// 此操作是三阶段确认（confirm -> commit -> cancel）的第二步。
        /// 当前服务端生产链路必须回传 HIS 实际落账明细，此兼容重载不适用于生产收费入口。
        /// </summary>
        /// <param name="requestId">confirm 阶段返回的请求 ID</param>
        /// <param name="chargeNo">HIS 落账后的收费单号，用于关联 HIS 侧的收费记录</param>
        /// <returns>API 响应</returns>
        [Obsolete("生产 commit 必须回传 HIS 实际落账明细，请使用 CommitAfterHisSuccess(long, string, List<PricingCommitActualItemRequest>, decimal?)。")]
        public ApiResponse CommitAfterHisSuccess(long requestId, string chargeNo)
        {
            throw new InvalidOperationException(
                "当前计价中心要求 commit 回传 HIS 实际落账明细，请使用带 actualItems 参数的 CommitAfterHisSuccess 重载。");
        }

        /// <summary>
        /// HIS 落账成功后通知计价服务提交（commit），并回传 HIS 实际落账明细。
        /// 生产收费链路应优先使用此重载；计价中心会校验实际落账数量/金额与 confirm 结果完全一致。
        /// </summary>
        /// <param name="requestId">confirm 阶段返回的请求 ID</param>
        /// <param name="chargeNo">HIS 落账后的收费单号</param>
        /// <param name="actualItems">HIS 实际落账明细列表</param>
        /// <param name="actualTotalAmount">HIS 实际落账总金额</param>
        /// <returns>API 响应</returns>
        public ApiResponse CommitAfterHisSuccess(
            long requestId,
            string chargeNo,
            List<PricingCommitActualItemRequest> actualItems,
            decimal? actualTotalAmount)
        {
            if (actualItems == null || actualItems.Count == 0)
            {
                throw new ArgumentException("commit 必须回传 HIS 实际落账明细 actualItems。", "actualItems");
            }

            return _client.Commit(new PricingCommitRequest
            {
                RequestId = requestId,
                ChargeNo = chargeNo,
                ActualItems = actualItems,
                ActualTotalAmount = actualTotalAmount
            });
        }

        /// <summary>
        /// HIS 落账失败后通知计价服务取消（cancel）。
        /// cancel 释放 confirm 阶段占用的额度，状态从 CONFIRM_PENDING 流转为 CANCELLED。
        /// 资金安全约束：HIS 落账失败时必须调用此方法，否则额度会被永久占用。
        /// </summary>
        /// <param name="requestId">confirm 阶段返回的请求 ID</param>
        /// <returns>API 响应</returns>
        public ApiResponse CancelAfterHisFailure(long requestId)
        {
            return _client.Cancel(new PricingCancelRequest
            {
                RequestId = requestId
            });
        }

        /// <summary>
        /// 确保业务请求号有值。
        /// BusinessRequestNo 是 confirm 接口幂等性的关键字段，
        /// 幂等键 = sourceSystem + businessRequestNo + callType。
        /// 生成策略（按优先级）：
        /// 1. 调用方已传入 -> 直接使用
        /// 2. 有 HIS 收费单号 -> 以 "HIS_CHARGE_" + chargeNo 构造
        /// 3. 均无 -> 以 "HIS_PENDING_" + 时间戳 + GUID 构造（兜底方案）
        /// </summary>
        /// <param name="existingBusinessRequestNo">调用方已有的业务请求号</param>
        /// <param name="chargeNo">HIS 收费单号</param>
        /// <returns>保证非空的业务请求号</returns>
        public static string EnsureBusinessRequestNo(string existingBusinessRequestNo, string chargeNo)
        {
            if (!string.IsNullOrEmpty(existingBusinessRequestNo))
            {
                return existingBusinessRequestNo;
            }

            if (!string.IsNullOrEmpty(chargeNo))
            {
                return "HIS_CHARGE_" + chargeNo;
            }

            return "HIS_PENDING_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 便捷方法：构建单项目计价请求。
        /// 适用于 HIS 收费录入时一个收费动作只包含一个项目的场景。
        /// 对于多项目、多肿物、多部位的复杂场景，调用方应自行构建 PricingCalculateRequest，
        /// 使用 PricingParts 明细表达多部位信息。
        /// </summary>
        /// <param name="patientId">患者 ID</param>
        /// <param name="visitId">就诊 ID</param>
        /// <param name="chargeScene">收费场景，如 "OUTPATIENT"、"SURGERY"</param>
        /// <param name="chargeNo">HIS 收费单号（可为 null，但影响幂等键生成）</param>
        /// <param name="businessRequestNo">业务请求号（可为 null，自动生成）</param>
        /// <param name="operatorId">操作员工号</param>
        /// <param name="operatorName">操作员姓名</param>
        /// <param name="itemCode">项目编码</param>
        /// <param name="itemName">项目名称</param>
        /// <param name="qty">数量</param>
        /// <param name="unit">计量单位</param>
        /// <param name="unitPrice">单价（注意：confirm 时计价中心会读取权威单价校验，不一致返回 PRICE_MISMATCH）</param>
        /// <param name="bodyPartCode">部位编码（可为 null）</param>
        /// <param name="chargeDeptCode">收费科室编码（可为 null），用于排除7021等科室的折价规则</param>
        /// <returns>构建好的计价请求对象</returns>
        public static PricingCalculateRequest BuildSingleItemRequest(
            string patientId,
            string visitId,
            string chargeScene,
            string chargeNo,
            string businessRequestNo,
            string operatorId,
            string operatorName,
            string itemCode,
            string itemName,
            decimal qty,
            string unit,
            decimal unitPrice,
            string bodyPartCode,
            string chargeDeptCode = null)
        {
            PricingCalculateRequest request = new PricingCalculateRequest();
            request.RequestNo = "HIS_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
            request.PatientId = patientId;
            request.VisitId = visitId;
            request.ChargeScene = chargeScene;
            request.BusinessChargeTime = DateTime.Now;
            request.SourceSystem = "HIS";
            request.ChargeNo = chargeNo;
            request.BusinessRequestNo = EnsureBusinessRequestNo(businessRequestNo, chargeNo);
            request.OperatorId = operatorId;
            request.OperatorName = operatorName;
            request.ChargeDeptCode = chargeDeptCode;
            request.Items = new List<PricingCalculateItemRequest>();
            request.Items.Add(new PricingCalculateItemRequest
            {
                ItemRequestNo = "ITEM_1",
                ItemCode = itemCode,
                ItemName = itemName,
                InputQty = qty,
                Unit = unit,
                UnitPrice = unitPrice,
                BodyPartCode = bodyPartCode
            });
            return request;
        }

        /// <summary>
        /// 查询 HIS 旧系统在指定时间窗口内已收费的数量（方案B兜底查询）。
        /// 上线过渡期调用，确保新系统上线当天不会因历史数据断层而放行超限收费。
        ///
        /// 实现说明：
        /// 此方法需要访问 HIS 旧收费明细表，具体表名和字段请对照旧系统 getRestrictingfee
        /// 方法的 SQL（配置键：Fee.PactUnitItemRate.RestrictingfeePay2）填写。
        ///
        /// 典型实现参考：
        /// SELECT COUNT(*) FROM FIN_CHARGE_DETAIL
        /// WHERE PATIENT_ID = :patientId
        ///   AND ITEM_CODE = :itemCode
        ///   AND CHARGE_TIME BETWEEN :windowStart AND :windowEnd
        ///   AND STATUS != 'REFUNDED'  -- 排除已退费记录
        ///
        /// 当前仓库未收录完整 SQL，默认实现会直接抛出异常，避免上线过渡期误以为已经计入旧收费数据。
        /// 上线稳定且确认不再需要旧历史兜底后，调用方可不再调用此方法，直接不传 LegacyOccupiedQty。
        /// </summary>
        /// <param name="patientId">患者 ID</param>
        /// <param name="itemCode">项目编码</param>
        /// <param name="windowStart">时间窗口开始（业务收费时间 - 2小时）</param>
        /// <param name="windowEnd">时间窗口结束（业务收费时间）</param>
        /// <returns>旧系统窗口内已收费数量</returns>
        public static decimal QueryLegacyOccupiedQty(
            string patientId,
            string itemCode,
            DateTime windowStart,
            DateTime windowEnd)
        {
            // TODO: 从 HIS 内网 SQL 配置中提取 Fee.PactUnitItemRate.RestrictingfeePay2 后实现。
            // try
            // {
            //     // 示例：使用 HIS 现有数据库访问方式查询
            //     // string sql = "SELECT NVL(SUM(CHARGE_QTY),0) FROM FIN_CHARGE_DETAIL " +
            //     //              "WHERE PATIENT_ID=:pid AND ITEM_CODE=:icode " +
            //     //              "AND CHARGE_TIME BETWEEN :wstart AND :wend " +
            //     //              "AND STATUS != 'REFUNDED'";
            //     // return HisDbHelper.QueryScalar<decimal>(sql, patientId, itemCode, windowStart, windowEnd);
            // }
            // catch
            // {
            //     throw;
            // }
            throw new NotSupportedException(
                "QueryLegacyOccupiedQty 尚未接入旧 HIS getRestrictingfee SQL。上线过渡期必须实现旧收费明细查询，或不要调用此方法。");
        }
    }

    /// <summary>
    /// 特殊计价决策结果。
    /// 由 CheckSpecialPricingRequired 方法返回，HIS 收费流程据此决定后续行为。
    ///
    /// 三种决策：
    /// - AllowOrdinary：允许普通计价（非特殊项目）
    /// - RequirePopup：必须打开计价弹窗（特殊项目）
    /// - BlockAsServiceUnavailable：阻断收费（服务不可用，禁止回退）
    /// </summary>
    public sealed class SpecialPricingDecision
    {
        /// <summary>是否为特殊计价项目</summary>
        public bool IsSpecial { get; private set; }

        /// <summary>是否允许按普通计价流程继续收费</summary>
        public bool AllowOrdinaryPricing { get; private set; }

        /// <summary>是否需要打开特殊计价确认弹窗</summary>
        public bool ShouldOpenPopup { get; private set; }

        /// <summary>
        /// 计价服务是否不可用。为 true 时必须阻断收费流程，
        /// HIS 不得回退为普通计价——这是资金安全硬约束。
        /// </summary>
        public bool ServiceUnavailable { get; private set; }

        /// <summary>决策说明信息，可展示给收费人员或记录到日志</summary>
        public string Message { get; private set; }

        /// <summary>
        /// 创建"允许普通计价"决策。用于非特殊项目的正常放行。
        /// </summary>
        /// <param name="message">决策说明</param>
        /// <returns>允许普通计价的决策实例</returns>
        public static SpecialPricingDecision AllowOrdinary(string message)
        {
            return new SpecialPricingDecision
            {
                IsSpecial = false,
                AllowOrdinaryPricing = true,
                ShouldOpenPopup = false,
                ServiceUnavailable = false,
                Message = message
            };
        }

        /// <summary>
        /// 创建"需要弹窗"决策。用于特殊项目的收费流程，HIS 必须打开计价确认弹窗。
        /// </summary>
        /// <param name="message">决策说明</param>
        /// <returns>需要弹窗的决策实例</returns>
        public static SpecialPricingDecision RequirePopup(string message)
        {
            return new SpecialPricingDecision
            {
                IsSpecial = true,
                AllowOrdinaryPricing = false,
                ShouldOpenPopup = true,
                ServiceUnavailable = false,
                Message = message
            };
        }

        /// <summary>
        /// 创建"服务不可用阻断"决策。计价服务异常时使用，
        /// HIS 必须阻断收费流程，不得回退为普通计价。
        /// </summary>
        /// <param name="message">决策说明（含错误信息）</param>
        /// <returns>服务不可用阻断决策实例</returns>
        public static SpecialPricingDecision BlockAsServiceUnavailable(string message)
        {
            return new SpecialPricingDecision
            {
                IsSpecial = true,
                AllowOrdinaryPricing = false,
                ShouldOpenPopup = false,
                ServiceUnavailable = true,
                Message = message
            };
        }
    }

    /// <summary>
    /// 特殊计价弹窗结果。
    /// 封装弹窗操作的结果，供 HIS 收费流程判断后续行为。
    /// </summary>
    public sealed class PricingPopupResult
    {
        /// <summary>用户是否点击了"确认收费"。为 false 时表示用户取消了操作。</summary>
        public bool Confirmed { get; private set; }

        /// <summary>
        /// 确认计价后的请求 ID。仅当 Confirmed = true 时有值。
        /// HIS 落账失败时需使用此 ID 调用 cancel 释放额度。
        /// </summary>
        public long RequestId { get; private set; }

        /// <summary>
        /// 确认计价后的完整响应数据。仅当 Confirmed = true 时有值。
        /// 包含最终金额、匹配的规则 ID 等信息，可用于 HIS 侧的审计记录。
        /// </summary>
        public PricingCalculateResponse Response { get; private set; }

        /// <summary>
        /// 从确认成功的结果创建弹窗结果。
        /// </summary>
        /// <param name="response">确认计价响应</param>
        /// <param name="requestId">请求 ID</param>
        /// <returns>已确认的弹窗结果</returns>
        public static PricingPopupResult FromConfirmed(PricingCalculateResponse response, long requestId)
        {
            return new PricingPopupResult
            {
                Confirmed = true,
                Response = response,
                RequestId = requestId
            };
        }

        /// <summary>
        /// 创建用户取消的弹窗结果。
        /// </summary>
        /// <returns>已取消的弹窗结果</returns>
        public static PricingPopupResult Cancelled()
        {
            return new PricingPopupResult
            {
                Confirmed = false
            };
        }
    }
}
