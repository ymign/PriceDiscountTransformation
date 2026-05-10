using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Pricing.Client
{
    public sealed class FrmPricingPopup : Form
    {
        private readonly PricingApiClient _client;
        private readonly PricingCalculateRequest _request;
        private Label _lblSummary;
        private Label _lblOriginalAmount;
        private Label _lblFinalAmount;
        private Label _lblDiscountAmount;
        private DataGridView _gridItems;
        private DataGridView _gridTrace;
        private Button _btnSimulate;
        private Button _btnConfirm;
        private Button _btnCancel;
        private TextBox _txtReason;
        private PricingCalculateResponse _simulateResponse;

        public PricingCalculateResponse ConfirmedResponse { get; private set; }
        public long ConfirmedRequestId { get; private set; }

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
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RunSimulate();
        }

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

        private static Label CreateAmountLabel(string text, int left)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(left, 34);
            label.Size = new Size(230, 26);
            label.Font = new Font("宋体", 11F, FontStyle.Bold);
            return label;
        }

        private string BuildSummary()
        {
            int count = _request.Items == null ? 0 : _request.Items.Count;
            return "患者：" + Safe(_request.PatientId)
                + "    收费场景：" + Safe(_request.ChargeScene)
                + "    业务号：" + Safe(_request.BusinessRequestNo)
                + Environment.NewLine
                + "本次收费动作包含 " + count + " 条费用明细，确认后占用额度；HIS 落账失败必须调用 cancel。";
        }

        private void BtnSimulateClick(object sender, EventArgs e)
        {
            RunSimulate();
        }

        private void BtnConfirmClick(object sender, EventArgs e)
        {
            try
            {
                EnsureBusinessRequestNo();
                ApiResponse<PricingCalculateResponse> response = _client.Confirm(_request);
                EnsureSuccess(response);
                ConfirmedResponse = response.Data;
                ConfirmedRequestId = response.Data.RequestId;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "计价服务暂时不可用，请稍后重试。" + Environment.NewLine + ex.Message,
                    "确认计价失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void RunSimulate()
        {
            try
            {
                _btnSimulate.Enabled = false;
                ApiResponse<PricingCalculateResponse> response = _client.Simulate(_request);
                EnsureSuccess(response);
                _simulateResponse = response.Data;
                BindResult(_simulateResponse);
            }
            catch (Exception ex)
            {
                _simulateResponse = null;
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

        private void BindResult(PricingCalculateResponse response)
        {
            if (response == null)
            {
                return;
            }

            _lblOriginalAmount.Text = "原价：" + FormatMoney(CalculateOriginalAmount(response));
            _lblFinalAmount.Text = "折后价：" + FormatMoney(response.FinalAmount);
            _lblDiscountAmount.Text = "折价金额：" + FormatMoney(response.DiscountAmount);
            _gridItems.DataSource = response.Items;
            _gridTrace.DataSource = BuildTraceRows(response);
            _txtReason.Text = BuildReason(response);
        }

        private static List<PricingTraceDisplayRow> BuildTraceRows(PricingCalculateResponse response)
        {
            List<PricingTraceDisplayRow> rows = new List<PricingTraceDisplayRow>();
            if (response.TraceSteps != null)
            {
                AddTraceRows(rows, string.Empty, response.TraceSteps);
            }

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

        private static string BuildReason(PricingCalculateResponse response)
        {
            List<string> reasons = new List<string>();
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

            if (response.Items != null)
            {
                foreach (PricingCalculateItemResponse item in response.Items)
                {
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

        private void EnsureBusinessRequestNo()
        {
            _request.BusinessRequestNo = PricingHisIntegrationHelper.EnsureBusinessRequestNo(
                _request.BusinessRequestNo,
                _request.ChargeNo);
        }

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

        private static string FormatMoney(decimal value)
        {
            return value.ToString("0.00");
        }

        private static string Safe(string value)
        {
            return value == null ? string.Empty : value;
        }

        private sealed class PricingTraceDisplayRow
        {
            public string Item { get; set; }
            public int StepNo { get; set; }
            public string StepType { get; set; }
            public string StepDesc { get; set; }
            public decimal? InputValue { get; set; }
            public decimal? OutputValue { get; set; }
        }
    }
}
