using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 新规则平台策略发布中心。
    /// 供物价管理员在 HIS 内直接操作模板策略和运行时包。
    /// </summary>
    public sealed class FrmPolicyPublishCenter : Form
    {
        private readonly PricingApiClient _client;
        private readonly string _operatorId;

        private DataGridView _gridPolicies;
        private DataGridView _gridPackages;
        private TextBox _txtPreview;

        private List<PolicyResponse> _policies;
        private List<RuntimePackageHistoryResponse> _packages;
        private PolicyResponse _selectedPolicy;
        private RuntimePackageHistoryResponse _selectedPackage;

        public FrmPolicyPublishCenter(PricingApiClient client, string operatorId)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
            _operatorId = operatorId;
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RefreshAll();
        }

        private void InitializeComponent()
        {
            Text = "策略发布中心";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1180, 760);

            ToolStrip toolbar = new ToolStrip();
            toolbar.GripStyle = ToolStripGripStyle.Hidden;
            toolbar.Items.Add(CreateToolButton("刷新", ToolbarRefreshClick));
            toolbar.Items.Add(CreateToolButton("预览", ToolbarPreviewClick));
            toolbar.Items.Add(CreateToolButton("校验", ToolbarValidateClick));
            toolbar.Items.Add(CreateToolButton("提审", ToolbarSubmitReviewClick));
            toolbar.Items.Add(CreateToolButton("发布", ToolbarPublishClick));
            toolbar.Items.Add(CreateToolButton("激活包", ToolbarActivateClick));
            toolbar.Items.Add(CreateToolButton("回滚包", ToolbarRollbackClick));
            toolbar.Dock = DockStyle.Top;

            SplitContainer mainSplit = new SplitContainer();
            mainSplit.Dock = DockStyle.Fill;
            mainSplit.Orientation = Orientation.Horizontal;
            mainSplit.SplitterDistance = 360;

            SplitContainer upperSplit = new SplitContainer();
            upperSplit.Dock = DockStyle.Fill;
            upperSplit.SplitterDistance = 560;

            _gridPolicies = CreateReadOnlyGrid();
            _gridPolicies.Dock = DockStyle.Fill;
            _gridPolicies.SelectionChanged += GridPoliciesSelectionChanged;
            upperSplit.Panel1.Controls.Add(_gridPolicies);

            _txtPreview = new TextBox();
            _txtPreview.Dock = DockStyle.Fill;
            _txtPreview.Multiline = true;
            _txtPreview.ScrollBars = ScrollBars.Vertical;
            upperSplit.Panel2.Controls.Add(_txtPreview);

            _gridPackages = CreateReadOnlyGrid();
            _gridPackages.Dock = DockStyle.Fill;
            _gridPackages.SelectionChanged += GridPackagesSelectionChanged;

            mainSplit.Panel1.Controls.Add(upperSplit);
            mainSplit.Panel2.Controls.Add(_gridPackages);

            Controls.Add(mainSplit);
            Controls.Add(toolbar);
        }

        private static ToolStripButton CreateToolButton(string text, EventHandler handler)
        {
            ToolStripButton button = new ToolStripButton(text);
            button.Click += handler;
            return button;
        }

        private static DataGridView CreateReadOnlyGrid()
        {
            DataGridView grid = new DataGridView();
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoGenerateColumns = true;
            return grid;
        }

        private void ToolbarRefreshClick(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void ToolbarPreviewClick(object sender, EventArgs e)
        {
            PolicyVersionResponse version = GetSelectedPolicyCurrentVersion();
            if (version == null)
            {
                return;
            }

            try
            {
                ApiResponse<PolicyPreviewResponse> response = _client.PreviewPolicy(version.PolicyVersionId);
                EnsureSuccess(response);
                _txtPreview.Text = BuildPreviewText(response.Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "预览失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToolbarValidateClick(object sender, EventArgs e)
        {
            PolicyVersionResponse version = GetSelectedPolicyCurrentVersion();
            if (version == null)
            {
                return;
            }

            try
            {
                ApiResponse<PolicyValidateResponse> response = _client.ValidatePolicy(version.PolicyVersionId);
                EnsureSuccess(response);
                MessageBox.Show(this, "校验完成，状态：" + (response.Data == null ? string.Empty : response.Data.PolicyStatus), "校验成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "校验失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToolbarSubmitReviewClick(object sender, EventArgs e)
        {
            PolicyVersionResponse version = GetSelectedPolicyCurrentVersion();
            if (version == null)
            {
                return;
            }

            try
            {
                ApiResponse<long> response = _client.SubmitPolicyReview(version.PolicyVersionId, new PolicyReviewSubmitRequest
                {
                    SubmittedBy = _operatorId,
                    ReviewStage = "NORMAL"
                });
                EnsureSuccess(response);
                MessageBox.Show(this, "已提交审核。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "提审失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToolbarPublishClick(object sender, EventArgs e)
        {
            PolicyVersionResponse version = GetSelectedPolicyCurrentVersion();
            if (version == null)
            {
                return;
            }

            try
            {
                ApiResponse<long> response = _client.PublishRuntimePackage(new RuntimePackagePublishRequest
                {
                    PolicyVersionIds = new List<long> { version.PolicyVersionId },
                    PublishedBy = _operatorId
                });
                EnsureSuccess(response);
                MessageBox.Show(this, "已生成并激活运行时包，包ID：" + response.Data, "发布成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "发布失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToolbarActivateClick(object sender, EventArgs e)
        {
            if (_selectedPackage == null)
            {
                MessageBox.Show(this, "请先选择运行时包。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ApiResponse<long> response = _client.ActivateRuntimePackage(_selectedPackage.PackageId, new RuntimePackageOperationRequest
                {
                    OperatedBy = _operatorId
                });
                EnsureSuccess(response);
                MessageBox.Show(this, "已激活运行时包：" + response.Data, "激活成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "激活失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToolbarRollbackClick(object sender, EventArgs e)
        {
            if (_selectedPackage == null)
            {
                MessageBox.Show(this, "请先选择运行时包。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ApiResponse<long> response = _client.RollbackRuntimePackage(_selectedPackage.PackageId, new RuntimePackageOperationRequest
                {
                    OperatedBy = _operatorId
                });
                EnsureSuccess(response);
                MessageBox.Show(this, "已回滚运行时包：" + response.Data, "回滚成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "回滚失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridPoliciesSelectionChanged(object sender, EventArgs e)
        {
            if (_gridPolicies.CurrentRow == null || _gridPolicies.CurrentRow.Index < 0 || _policies == null)
            {
                _selectedPolicy = null;
                return;
            }

            if (_gridPolicies.CurrentRow.Index < _policies.Count)
            {
                _selectedPolicy = _policies[_gridPolicies.CurrentRow.Index];
            }
        }

        private void GridPackagesSelectionChanged(object sender, EventArgs e)
        {
            if (_gridPackages.CurrentRow == null || _gridPackages.CurrentRow.Index < 0 || _packages == null)
            {
                _selectedPackage = null;
                return;
            }

            if (_gridPackages.CurrentRow.Index < _packages.Count)
            {
                _selectedPackage = _packages[_gridPackages.CurrentRow.Index];
            }
        }

        private void RefreshAll()
        {
            try
            {
                ApiResponse<List<PolicyResponse>> policyResponse = _client.GetPolicies();
                EnsureSuccess(policyResponse);
                _policies = policyResponse.Data ?? new List<PolicyResponse>();
                _gridPolicies.DataSource = null;
                _gridPolicies.DataSource = _policies;

                ApiResponse<List<RuntimePackageHistoryResponse>> packageResponse = _client.GetRuntimePackageHistory(30);
                EnsureSuccess(packageResponse);
                _packages = packageResponse.Data ?? new List<RuntimePackageHistoryResponse>();
                _gridPackages.DataSource = null;
                _gridPackages.DataSource = _packages;

                _txtPreview.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "刷新失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private PolicyVersionResponse GetSelectedPolicyCurrentVersion()
        {
            if (_selectedPolicy == null)
            {
                MessageBox.Show(this, "请先选择策略。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            ApiResponse<PolicyDetailResponse> response = _client.GetPolicy(_selectedPolicy.PolicyId);
            EnsureSuccess(response);
            if (response.Data == null || response.Data.Versions == null || response.Data.Versions.Count == 0)
            {
                MessageBox.Show(this, "当前策略没有版本。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            return response.Data.Versions.OrderByDescending(item => item.VersionNo).First();
        }

        private static void EnsureSuccess(ApiResponse response)
        {
            if (response == null)
            {
                throw new InvalidOperationException("服务未返回响应。");
            }

            if (!response.Success)
            {
                throw new InvalidOperationException(response.Message);
            }
        }

        private static string BuildPreviewText(PolicyPreviewResponse preview)
        {
            if (preview == null)
            {
                return string.Empty;
            }

            return "策略编码：" + preview.PolicyCode + Environment.NewLine +
                   "模板版本：" + preview.TemplateVersionId + Environment.NewLine +
                   "能力族：" + preview.CapabilityFamily + Environment.NewLine +
                   "合并模式：" + preview.MergeMode + Environment.NewLine +
                   "绑定：" + string.Join(", ", preview.BindingSummary == null ? new string[0] : preview.BindingSummary.ToArray()) + Environment.NewLine +
                   "作用域：" + string.Join(", ", preview.ScopeSummary == null ? new string[0] : preview.ScopeSummary.ToArray()) + Environment.NewLine +
                   "动作链：" + Environment.NewLine +
                   string.Join(Environment.NewLine, preview.ActionChain == null ? new string[0] : preview.ActionChain.ToArray());
        }
    }
}
