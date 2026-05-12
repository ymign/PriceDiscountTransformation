using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// PricingAgent 现场诊断窗口。
    /// </summary>
    public sealed class FrmPricingAgentDiagnostics : Form
    {
        private readonly PricingSdk _sdk;
        private TextBox _txtInfo;
        private ListBox _lstPending;
        private Button _btnHealth;
        private Button _btnRefresh;

        /// <summary>
        /// 创建诊断窗口。
        /// </summary>
        public FrmPricingAgentDiagnostics(PricingSdk sdk)
        {
            if (sdk == null)
            {
                throw new ArgumentNullException("sdk");
            }

            _sdk = sdk;
            InitializeComponents();
            RefreshInfo();
            RefreshPendingFiles();
        }

        private void InitializeComponents()
        {
            Text = "PricingAgent 诊断";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(760, 520);
            MinimumSize = new Size(720, 460);
            BackColor = Color.White;
            Font = new Font("Microsoft YaHei UI", 9F);

            _txtInfo = new TextBox();
            _txtInfo.Multiline = true;
            _txtInfo.ReadOnly = true;
            _txtInfo.ScrollBars = ScrollBars.Vertical;
            _txtInfo.Location = new Point(12, 12);
            _txtInfo.Size = new Size(720, 170);
            _txtInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label pendingLabel = new Label();
            pendingLabel.Text = "待补偿记录";
            pendingLabel.Location = new Point(12, 198);
            pendingLabel.Size = new Size(120, 22);

            _lstPending = new ListBox();
            _lstPending.Location = new Point(12, 224);
            _lstPending.Size = new Size(720, 190);
            _lstPending.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _btnHealth = new Button();
            _btnHealth.Text = "检测服务";
            _btnHealth.Location = new Point(532, 430);
            _btnHealth.Size = new Size(95, 30);
            _btnHealth.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnHealth.Click += BtnHealthClick;

            _btnRefresh = new Button();
            _btnRefresh.Text = "刷新";
            _btnRefresh.Location = new Point(637, 430);
            _btnRefresh.Size = new Size(95, 30);
            _btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnRefresh.Click += BtnRefreshClick;

            Controls.Add(_txtInfo);
            Controls.Add(pendingLabel);
            Controls.Add(_lstPending);
            Controls.Add(_btnHealth);
            Controls.Add(_btnRefresh);
        }

        private void RefreshInfo()
        {
            string logDirectory = _sdk.LogDirectory;
            string compensationDirectory = _sdk.CompensationDirectory;
            string baseUrl = _sdk.Options == null ? "(调用方直接注入 HTTP 客户端)" : _sdk.Options.GetNormalizedBaseUrl();

            _txtInfo.Text =
                "版本：" + PricingAgentVersion.GetDisplayText() + Environment.NewLine
                + "服务地址：" + baseUrl + Environment.NewLine
                + "日志目录：" + (string.IsNullOrEmpty(logDirectory) ? "(未启用)" : logDirectory) + Environment.NewLine
                + "补偿目录：" + (string.IsNullOrEmpty(compensationDirectory) ? "(未启用)" : compensationDirectory) + Environment.NewLine
                + "说明：commit/cancel/reverse 调用失败或返回非成功业务码时，会在补偿目录写入 JSON 记录。";
        }

        private void RefreshPendingFiles()
        {
            _lstPending.Items.Clear();
            string directory = _sdk.CompensationDirectory;
            string[] files = PricingCompensationStore.GetPendingFiles(directory);
            for (int i = 0; i < files.Length; i++)
            {
                _lstPending.Items.Add(files[i]);
            }
        }

        private void BtnHealthClick(object sender, EventArgs e)
        {
            try
            {
                ApiResponse<PricingServiceHealthResponse> response = _sdk.CheckServiceCompatibility();
                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    MessageBox.Show(this, "服务健康检查失败。", "PricingAgent", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(
                    this,
                    "服务状态：" + response.Data.Status
                    + Environment.NewLine
                    + "服务版本：" + response.Data.ServiceVersion
                    + Environment.NewLine
                    + "协议版本：" + response.Data.ProtocolVersion,
                    "PricingAgent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "PricingAgent", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefreshClick(object sender, EventArgs e)
        {
            RefreshInfo();
            RefreshPendingFiles();
        }
    }
}
