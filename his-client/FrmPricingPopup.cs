using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 特殊计价确认弹窗。
    /// 当 HIS 收费时检测到某项目命中特殊计价规则，弹出此窗口：
    /// 1. 自动执行试算（simulate），展示原价、折后价、折价金额
    /// 2. 展示计价引擎的计算步骤（TraceSteps），供收费人员了解折价依据
    /// 3. 收费人员确认后执行确认计价（confirm），占用额度
    ///
    /// 关键业务约束：
    /// - 试算不占用额度，仅确认才占用
    /// - 试算失败时禁止按普通价格继续收费（资金安全硬约束）
    /// - 确认后 HIS 落账失败必须调用 cancel 释放额度
    /// </summary>
    public sealed class FrmPricingPopup : Form
    {
        /// <summary>
        /// 统一计价服务 HTTP 客户端。所有 API 调用均通过此对象发出，
        /// 由调用方在构造时注入，弹窗本身不负责客户端的创建和配置。
        /// </summary>
        private readonly PricingApiClient _client;

        /// <summary>
        /// 计价请求对象。包含患者信息、收费场景、费用明细等完整上下文。
        /// 弹窗只读使用此对象，确认时原样传给 confirm 接口。
        /// </summary>
        private readonly PricingCalculateRequest _request;

        /// <summary>摘要信息标签，展示患者、收费场景、业务号等上下文</summary>
        private Label _lblSummary;

        /// <summary>原价标签，展示 InputQty * UnitPrice 的原始总金额</summary>
        private Label _lblOriginalAmount;

        /// <summary>折后价标签，展示引擎计算后的最终金额</summary>
        private Label _lblFinalAmount;

        /// <summary>折价金额标签，展示原价与折后价的差额</summary>
        private Label _lblDiscountAmount;

        /// <summary>费用明细网格，展示本次收费动作的所有费用明细行及其计价结果</summary>
        private DataGridView _gridItems;

        /// <summary>计算步骤网格，展示计价引擎的每一步计算过程（规则匹配、公式执行、限额校验等）</summary>
        private DataGridView _gridTrace;

        /// <summary>试算按钮，点击后重新执行 simulate 调用（幂等，可多次点击）</summary>
        private Button _btnSimulate;

        /// <summary>确认收费按钮，点击后执行 confirm 调用，成功则占用额度并关闭弹窗</summary>
        private Button _btnConfirm;

        /// <summary>取消按钮，点击后关闭弹窗，不执行任何计价操作</summary>
        private Button _btnCancel;

        /// <summary>折价原因文本框，只读，展示引擎返回的折价依据摘要</summary>
        private TextBox _txtReason;

        /// <summary>
        /// 最近一次试算响应。confirm 操作前必须确保此值非空（即至少成功试算过一次），
        /// 否则不允许确认收费。
        /// </summary>
        private PricingCalculateResponse _simulateResponse;

        /// <summary>
        /// 确认计价成功后的响应数据。调用方通过此属性获取确认结果，
        /// 包含最终金额、RequestId 等，用于后续 HIS 落账。
        /// </summary>
        public PricingCalculateResponse ConfirmedResponse { get; private set; }

        /// <summary>
        /// 确认计价成功后的请求 ID。HIS 落账失败时需使用此 ID 调用 cancel 释放额度。
        /// </summary>
        public long ConfirmedRequestId { get; private set; }

        /// <summary>
        /// 构造特殊计价确认弹窗。
        /// </summary>
        /// <param name="client">统一计价服务 HTTP 客户端，不可为 null</param>
        /// <param name="request">计价请求上下文，包含患者、项目、数量等完整信息，不可为 null</param>
        public FrmPricingPopup(PricingApiClient client, PricingCalculateRequest request)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            _client = client;
            _request = request;
            EnsureBusinessRequestNo();
            InitializeComponent();
        }

        /// <summary>
        /// 窗口加载时自动执行试算。
        /// OnLoad 在窗口显示之前触发，确保用户看到窗口时已经展示试算结果。
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RunSimulate();
        }

        /// <summary>
        /// 初始化界面控件。纯代码布局（无 .Designer.cs），按以下区域组织：
        /// - 顶部：摘要信息（患者、场景、业务号）
        /// - 中上：试算结果（原价、折后价、折价金额）
        /// - 中部：费用明细网格 + 计算步骤网格
        /// - 中下：折价原因文本框（只读）
        /// - 底部：试算、确认收费、取消三个按钮
        /// </summary>
        private void InitializeComponent()
        {
            Text = "特殊计价确认";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(860, 620);
            MinimizeBox = false;
            MaximizeBox = false;

            _lblSummary = new Label();
            _lblSummary.AutoSize = false;
            _lblSummary.Location = new Point(12, 12);
            _lblSummary.Size = new Size(820, 44);
            _lblSummary.Text = BuildSummary();

            GroupBox amountGroup = new GroupBox();
            amountGroup.Text = "试算结果";
            amountGroup.Location = new Point(12, 60);
            amountGroup.Size = new Size(820, 82);

            _lblOriginalAmount = CreateAmountLabel("原价：--", 20);
            _lblFinalAmount = CreateAmountLabel("折后价：--", 290);
            _lblDiscountAmount = CreateAmountLabel("折价金额：--", 560);
            amountGroup.Controls.Add(_lblOriginalAmount);
            amountGroup.Controls.Add(_lblFinalAmount);
            amountGroup.Controls.Add(_lblDiscountAmount);

            _gridItems = new DataGridView();
            _gridItems.Location = new Point(12, 150);
            _gridItems.Size = new Size(820, 150);
            _gridItems.ReadOnly = true;
            _gridItems.AllowUserToAddRows = false;
            _gridItems.AllowUserToDeleteRows = false;
            _gridItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _gridItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            _gridTrace = new DataGridView();
            _gridTrace.Location = new Point(12, 310);
            _gridTrace.Size = new Size(820, 150);
            _gridTrace.ReadOnly = true;
            _gridTrace.AllowUserToAddRows = false;
            _gridTrace.AllowUserToDeleteRows = false;
            _gridTrace.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _gridTrace.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Label reasonLabel = new Label();
            reasonLabel.Text = "折价原因";
            reasonLabel.Location = new Point(12, 470);
            reasonLabel.Size = new Size(80, 22);

            _txtReason = new TextBox();
            _txtReason.Location = new Point(12, 495);
            _txtReason.Size = new Size(820, 45);
            _txtReason.Multiline = true;
            _txtReason.ReadOnly = true;

            _btnSimulate = new Button();
            _btnSimulate.Text = "试算";
            _btnSimulate.Location = new Point(560, 548);
            _btnSimulate.Click += BtnSimulateClick;

            _btnConfirm = new Button();
            _btnConfirm.Text = "确认收费";
            _btnConfirm.Location = new Point(650, 548);
            _btnConfirm.Enabled = false;
            _btnConfirm.Click += BtnConfirmClick;

            _btnCancel = new Button();
            _btnCancel.Text = "取消";
            _btnCancel.Location = new Point(755, 548);
            _btnCancel.Click += BtnCancelClick;

            Controls.Add(_lblSummary);
            Controls.Add(amountGroup);
            Controls.Add(_gridItems);
            Controls.Add(_gridTrace);
            Controls.Add(reasonLabel);
            Controls.Add(_txtReason);
            Controls.Add(_btnSimulate);
            Controls.Add(_btnConfirm);
            Controls.Add(_btnCancel);
        }

        /// <summary>
        /// 创建金额展示标签的工厂方法。
        /// </summary>
        /// <param name="text">初始文本（如 "原价：--"）</param>
        /// <param name="left">左侧坐标</param>
        /// <returns>配置好的金额标签</returns>
        private static Label CreateAmountLabel(string text, int left)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(left, 34);
            label.Size = new Size(230, 26);
            label.Font = new Font("宋体", 11F, FontStyle.Bold);
            return label;
        }

        /// <summary>
        /// 构建摘要信息文本，展示患者、收费场景、业务号和费用条数，
        /// 并提醒收费人员确认后占用额度以及落账失败时的 cancel 要求。
        /// </summary>
        private string BuildSummary()
        {
            int count = _request.Items == null ? 0 : _request.Items.Count;
            return "患者：" + Safe(_request.PatientId)
                + "    收费场景：" + Safe(_request.ChargeScene)
                + "    业务号：" + Safe(_request.BusinessRequestNo)
                + Environment.NewLine
                + "本次收费动作包含 " + count + " 条费用明细，确认后占用额度；HIS 落账失败必须调用 cancel。";
        }

        /// <summary>试算按钮点击事件，重新执行试算（幂等操作）</summary>
        private void BtnSimulateClick(object sender, EventArgs e)
        {
            RunSimulate();
        }

        /// <summary>
        /// 确认收费按钮点击事件。
        /// 执行 confirm 接口，成功后设置 DialogResult.OK 并关闭弹窗。
        /// 调用方通过 DialogResult 判断是否需要继续 HIS 落账流程。
        /// </summary>
        private void BtnConfirmClick(object sender, EventArgs e)
        {
            try
            {
                if (_simulateResponse == null)
                {
                    MessageBox.Show(this,
                        "请先完成试算。试算失败或未返回结果时，不允许确认收费。",
                        "确认计价失败",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                _btnConfirm.Enabled = false;

                // ========== 第1阶段：确保业务请求号 ==========
                // BusinessRequestNo 是幂等性保证的关键字段，confirm 接口以
                // sourceSystem + businessRequestNo + callType 为幂等键。
                // 若调用方未传入，由辅助类自动生成。
                EnsureBusinessRequestNo();
                if (string.IsNullOrEmpty(_request.BusinessRequestNo))
                {
                    throw new InvalidOperationException(
                        "confirm 前必须传入稳定的 BusinessRequestNo；若 HIS 尚未生成收费单号，请先预生成一次收费确认流水。");
                }

                _lblSummary.Text = BuildSummary();

                // ========== 第2阶段：执行确认计价 ==========
                // confirm 会占用额度，接口是幂等的——相同 businessRequestNo 重复调用不会重复占用。
                ApiResponse<PricingCalculateResponse> response = _client.Confirm(_request);
                EnsureSuccess(response);
                ConfirmedResponse = response.Data;
                ConfirmedRequestId = response.Data.RequestId;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                _btnConfirm.Enabled = _simulateResponse != null;

                // confirm 失败时不关闭弹窗，提示收费人员重试。
                // 资金安全约束：不允许在 confirm 失败时回退为普通计价。
                MessageBox.Show(this,
                    "计价服务暂时不可用，请稍后重试。" + Environment.NewLine + ex.Message,
                    "确认计价失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>取消按钮点击事件，关闭弹窗但不执行任何计价操作</summary>
        private void BtnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// 执行试算。调用 simulate 接口（不占额度），成功后绑定结果到界面。
        /// 试算失败时清空结果并禁止确认收费——这是资金安全硬约束：
        /// 计价服务不可用时，渠道不得回退为普通计价。
        /// </summary>
        private void RunSimulate()
        {
            try
            {
                // ========== 第1阶段：禁用试算按钮防止重复点击 ==========
                _simulateResponse = null;
                _btnSimulate.Enabled = false;
                _btnConfirm.Enabled = false;

                // ========== 第2阶段：调用试算接口 ==========
                // simulate 是幂等的，不占用额度，仅返回计算结果。
                ApiResponse<PricingCalculateResponse> response = _client.Simulate(_request);
                EnsureSuccess(response);
                _simulateResponse = response.Data;

                // ========== 第3阶段：绑定结果到界面 ==========
                BindResult(_simulateResponse);
                _btnConfirm.Enabled = true;
            }
            catch (Exception ex)
            {
                // 试算失败：清空结果，提示不允许按普通价格继续。
                // 这是"特殊项目"的安全约束——计价服务不可用时必须阻断收费流程。
                _simulateResponse = null;
                _btnConfirm.Enabled = false;
                _gridItems.DataSource = null;
                _gridTrace.DataSource = null;
                _txtReason.Text = "试算失败，不允许按普通价格继续收费：" + ex.Message;
                MessageBox.Show(this,
                    "计价服务暂时不可用，请稍后重试。" + Environment.NewLine + ex.Message,
                    "试算失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                _btnSimulate.Enabled = true;
            }
        }

        /// <summary>
        /// 将试算响应数据绑定到界面控件。
        /// 更新金额标签、费用明细网格、计算步骤网格和折价原因文本框。
        /// </summary>
        /// <param name="response">试算响应数据</param>
        private void BindResult(PricingCalculateResponse response)
        {
            if (response == null)
            {
                return;
            }

            _lblOriginalAmount.Text = "原价：" + FormatMoney(CalculateOriginalAmount(response));
            _lblFinalAmount.Text = "折后价：" + FormatMoney(response.FinalAmount);
            _lblDiscountAmount.Text = "折价金额：" + FormatMoney(response.DiscountAmount);
            _gridItems.DataSource = BuildItemRows(response);
            _gridTrace.DataSource = BuildTraceRows(response);
            _txtReason.Text = BuildReason(response);
        }

        /// <summary>
        /// 构建费用明细展示行。避免直接绑定服务端 DTO 中的集合属性，
        /// 否则替换子项/加收子项会在 DataGridView 中显示为类型名，收费人员无法核对。
        /// </summary>
        /// <param name="response">试算响应</param>
        /// <returns>费用明细展示行列表</returns>
        private static List<PricingItemDisplayRow> BuildItemRows(PricingCalculateResponse response)
        {
            List<PricingItemDisplayRow> rows = new List<PricingItemDisplayRow>();
            if (response == null || response.Items == null)
            {
                return rows;
            }

            foreach (PricingCalculateItemResponse item in response.Items)
            {
                rows.Add(new PricingItemDisplayRow
                {
                    ChargeDetailNo = Safe(item.ChargeDetailNo),
                    ItemCode = Safe(item.ItemCode),
                    ItemName = Safe(item.ItemName),
                    InputQty = FormatQty(item.InputQty),
                    ConvertedQty = FormatQty(item.ConvertedQty),
                    FinalQty = FormatQty(item.FinalQty),
                    UnitPrice = FormatMoney(item.UnitPrice),
                    FinalAmount = FormatMoney(item.FinalAmount),
                    DiscountAmount = FormatMoney(item.DiscountAmount),
                    ExceedQty = FormatQty(item.ExceedQty),
                    ReplacementItem = BuildReplacementSummary(item.ReplacementItem),
                    ChildItems = BuildChildSummary(item.ChildItems)
                });
            }

            return rows;
        }

        /// <summary>
        /// 构建计算步骤展示行。将响应中的 TraceSteps（全局级 + 各明细级）合并为统一列表，
        /// 供计算步骤网格展示。每行包含所属项目、步骤序号、步骤类型、描述、输入值、输出值。
        /// </summary>
        /// <param name="response">试算响应</param>
        /// <returns>计算步骤展示行列表</returns>
        private static List<PricingTraceDisplayRow> BuildTraceRows(PricingCalculateResponse response)
        {
            List<PricingTraceDisplayRow> rows = new List<PricingTraceDisplayRow>();

            // 全局级计算步骤（如全局限额校验、同组互斥等）
            if (response.TraceSteps != null)
            {
                AddTraceRows(rows, string.Empty, response.TraceSteps);
            }

            // 各明细级计算步骤（如单项目公式计算、金额上下限等）
            if (response.Items != null)
            {
                foreach (PricingCalculateItemResponse item in response.Items)
                {
                    string prefix = Safe(item.ItemCode) + " " + Safe(item.ItemName);
                    AddTraceRows(rows, prefix, item.TraceSteps);
                }
            }

            return rows;
        }

        /// <summary>
        /// 将一组计算步骤转换为展示行并追加到列表。
        /// </summary>
        /// <param name="rows">目标列表</param>
        /// <param name="itemDesc">所属项目描述（全局级为空字符串）</param>
        /// <param name="steps">计算步骤列表</param>
        private static void AddTraceRows(
            List<PricingTraceDisplayRow> rows,
            string itemDesc,
            List<PricingTraceStepResponse> steps)
        {
            if (steps == null)
            {
                return;
            }

            foreach (PricingTraceStepResponse step in steps)
            {
                rows.Add(new PricingTraceDisplayRow
                {
                    Item = itemDesc,
                    StepNo = step.StepNo,
                    StepType = step.StepType,
                    StepDesc = step.StepDesc,
                    InputValue = step.InputValue,
                    OutputValue = step.OutputValue
                });
            }
        }

        /// <summary>
        /// 构建折价原因摘要。汇总全局和各明细的计算步骤描述，
        /// 生成人类可读的折价依据说明。无详细原因时返回默认提示。
        /// </summary>
        /// <param name="response">试算响应</param>
        /// <returns>折价原因文本</returns>
        private static string BuildReason(PricingCalculateResponse response)
        {
            List<string> reasons = new List<string>();

            // 收集全局级步骤描述
            if (response.TraceSteps != null)
            {
                foreach (PricingTraceStepResponse step in response.TraceSteps)
                {
                    if (!string.IsNullOrEmpty(step.StepDesc))
                    {
                        reasons.Add(step.StepDesc);
                    }
                }
            }

            // 收集各明细级步骤描述，前缀加上项目编码
            if (response.Items != null)
            {
                foreach (PricingCalculateItemResponse item in response.Items)
                {
                    if (item.ReplacementItem != null)
                    {
                        reasons.Add(Safe(item.ItemCode) + "：替换子项 "
                            + BuildReplacementSummary(item.ReplacementItem));
                    }

                    if (item.ChildItems != null && item.ChildItems.Count > 0)
                    {
                        reasons.Add(Safe(item.ItemCode) + "：加收子项 "
                            + BuildChildSummary(item.ChildItems));
                    }

                    if (item.TraceSteps == null)
                    {
                        continue;
                    }

                    foreach (PricingTraceStepResponse step in item.TraceSteps)
                    {
                        if (!string.IsNullOrEmpty(step.StepDesc))
                        {
                            reasons.Add(Safe(item.ItemCode) + "：" + step.StepDesc);
                        }
                    }
                }
            }

            if (reasons.Count == 0)
            {
                return response.IsSpecialItem ? "命中特殊计价，未返回详细原因。" : "未命中特殊计价规则。";
            }

            return string.Join(Environment.NewLine, reasons.ToArray());
        }

        /// <summary>
        /// 计算原价总额。原价 = 各明细的 InputQty * UnitPrice 之和。
        /// 若响应包含多条明细则逐条累加；否则使用顶层的 InputQty * UnitPrice。
        /// 注意：原价是折价前的参考金额，与 FinalAmount（折后金额）对比展示折价效果。
        /// </summary>
        /// <param name="response">试算响应</param>
        /// <returns>原价总额（保留全部精度，展示时再格式化）</returns>
        private decimal CalculateOriginalAmount(PricingCalculateResponse response)
        {
            decimal amount = 0m;
            if (response.Items != null && response.Items.Count > 0)
            {
                foreach (PricingCalculateItemResponse item in response.Items)
                {
                    amount += item.InputQty * item.UnitPrice;
                }

                return amount;
            }

            return response.InputQty * response.UnitPrice;
        }

        /// <summary>
        /// 确保 BusinessRequestNo 有值。
        /// 若调用方未传入，则只允许由稳定的 ChargeNo 推导；没有稳定业务号时 confirm 必须阻断。
        /// 此字段是 confirm 接口幂等性的关键组成部分。
        /// </summary>
        private void EnsureBusinessRequestNo()
        {
            _request.BusinessRequestNo = PricingHisIntegrationHelper.EnsureBusinessRequestNo(
                _request.BusinessRequestNo,
                _request.ChargeNo);
        }

        /// <summary>
        /// 校验 API 响应是否成功。失败时抛出 PricingApiException。
        /// 三级校验：响应非空 -> IsSuccess 标记 -> Data 非空。
        /// </summary>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <param name="response">API 响应</param>
        private static void EnsureSuccess<T>(ApiResponse<T> response)
        {
            if (response == null)
            {
                throw new PricingApiException("计价服务返回空响应", null);
            }

            if (!response.IsSuccess)
            {
                throw new PricingApiException(response.Message, null);
            }

            if (response.Data == null)
            {
                throw new PricingApiException("计价服务未返回业务数据", null);
            }
        }

        /// <summary>
        /// 格式化金额为两位小数字符串。统一使用 "0.00" 格式，
        /// 保证界面上的金额展示与服务端的取整策略（四舍五入保留 2 位小数）一致。
        /// </summary>
        /// <param name="value">金额值</param>
        /// <returns>两位小数格式字符串</returns>
        private static string FormatMoney(decimal value)
        {
            return value.ToString("0.00");
        }

        /// <summary>格式化数量，最多保留 4 位小数，避免界面出现多余尾零</summary>
        private static string FormatQty(decimal value)
        {
            return value.ToString("0.####");
        }

        /// <summary>构建替换子项摘要，用于费用明细网格和折价原因文本</summary>
        private static string BuildReplacementSummary(PricingReplacementItemResponse replacement)
        {
            if (replacement == null)
            {
                return string.Empty;
            }

            return Safe(replacement.ItemCode)
                + " " + Safe(replacement.ItemName)
                + " x" + FormatQty(replacement.Qty)
                + " 金额" + FormatMoney(replacement.Amount);
        }

        /// <summary>构建加收子项摘要，用于费用明细网格和折价原因文本</summary>
        private static string BuildChildSummary(List<PricingChildItemResponse> children)
        {
            if (children == null || children.Count == 0)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();
            foreach (PricingChildItemResponse child in children)
            {
                parts.Add(Safe(child.ItemCode)
                    + " " + Safe(child.ItemName)
                    + " x" + FormatQty(child.Qty)
                    + " 金额" + FormatMoney(child.Amount));
            }

            return string.Join("；", parts.ToArray());
        }

        /// <summary>安全字符串转换，null 转为空字符串，防止界面展示 "null" 字样</summary>
        private static string Safe(string value)
        {
            return value == null ? string.Empty : value;
        }

        /// <summary>
        /// 费用明细展示行（内部模型）。
        /// 将服务端 DTO 展平成 DataGridView 易读字段，便于收费人员核对主项目、超限、替换和加收子项。
        /// </summary>
        private sealed class PricingItemDisplayRow
        {
            /// <summary>HIS 收费明细单号</summary>
            public string ChargeDetailNo { get; set; }

            /// <summary>项目编码</summary>
            public string ItemCode { get; set; }

            /// <summary>项目名称</summary>
            public string ItemName { get; set; }

            /// <summary>录入数量</summary>
            public string InputQty { get; set; }

            /// <summary>换算后数量</summary>
            public string ConvertedQty { get; set; }

            /// <summary>最终计价数量</summary>
            public string FinalQty { get; set; }

            /// <summary>单价</summary>
            public string UnitPrice { get; set; }

            /// <summary>最终金额</summary>
            public string FinalAmount { get; set; }

            /// <summary>折价金额</summary>
            public string DiscountAmount { get; set; }

            /// <summary>超限数量</summary>
            public string ExceedQty { get; set; }

            /// <summary>替换子项摘要</summary>
            public string ReplacementItem { get; set; }

            /// <summary>加收子项摘要</summary>
            public string ChildItems { get; set; }
        }

        /// <summary>
        /// 计算步骤展示行（内部模型）。
        /// 用于将全局级和明细级的 TraceSteps 统一展现在一个网格中。
        /// </summary>
        private sealed class PricingTraceDisplayRow
        {
            /// <summary>所属项目描述（全局级步骤此项为空）</summary>
            public string Item { get; set; }

            /// <summary>步骤序号，标识在引擎计算链中的位置</summary>
            public int StepNo { get; set; }

            /// <summary>
            /// 步骤类型，如 "MATCH"（规则匹配）、"FORMULA"（公式计算）、
            /// "LIMIT_QTY"（数量限制）、"LIMIT_AMOUNT"（金额限制）等
            /// </summary>
            public string StepType { get; set; }

            /// <summary>步骤描述，人类可读的计算说明</summary>
            public string StepDesc { get; set; }

            /// <summary>输入值（如原数量、原金额），NULL 表示此步骤无输入值</summary>
            public decimal? InputValue { get; set; }

            /// <summary>输出值（如计算后数量、计算后金额），NULL 表示此步骤无输出值</summary>
            public decimal? OutputValue { get; set; }
        }
    }
}
