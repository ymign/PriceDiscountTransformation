using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Pricing.Client
{
    public sealed class FrmPricingRuleWorkbench : Form
    {
        private readonly PricingApiClient _client;
        private readonly string _operatorId;
        private TextBox _txtSearchItemCode;
        private ComboBox _cboSearchStatus;
        private ComboBox _cboSearchCategory;
        private DataGridView _gridRules;
        private TabControl _tabs;
        private TextBox _txtRuleCode;
        private TextBox _txtRuleName;
        private ComboBox _cboRuleCategory;
        private ComboBox _cboRuleScope;
        private TextBox _txtItemCode;
        private TextBox _txtItemName;
        private TextBox _txtGroupCode;
        private NumericUpDown _numPriority;
        private DateTimePicker _dtEffectiveFrom;
        private DateTimePicker _dtEffectiveTo;
        private CheckBox _chkHasEffectiveFrom;
        private CheckBox _chkHasEffectiveTo;
        private TextBox _txtRemark;
        private DataGridView _gridConditions;
        private DataGridView _gridActions;
        private DataGridView _gridVersions;
        private DataGridView _gridPublishHistory;
        private DataGridView _gridChangeLogs;
        private ComboBox _cboFormula;
        private TextBox _txtFormulaParams;
        private ComboBox _cboDictType;
        private DataGridView _gridDicts;
        private DataGridView _gridFormulas;
        private RuleHeaderResponse _selectedRule;
        private List<RuleVersionResponse> _versions;
        private List<DictResponse> _ruleCategories;
        private List<DictResponse> _ruleStatuses;
        private List<DictResponse> _ruleScopes;
        private List<FormulaDefResponse> _formulas;
        private bool _creatingNew;

        public FrmPricingRuleWorkbench(PricingApiClient client, string operatorId)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
            _operatorId = operatorId;
            InitializeComponent();
        }

        public FrmPricingRuleWorkbench(PricingApiClient client)
            : this(client, null)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadDictionaryData();
            SearchRules();
        }

        private void InitializeComponent()
        {
            Text = "特殊计价规则维护";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1180, 760);

            ToolStrip toolbar = new ToolStrip();
            toolbar.GripStyle = ToolStripGripStyle.Hidden;
            toolbar.Items.Add(CreateToolButton("查询", ToolbarSearchClick));
            toolbar.Items.Add(CreateToolButton("新建", ToolbarNewClick));
            toolbar.Items.Add(CreateToolButton("保存", ToolbarSaveClick));
            toolbar.Items.Add(CreateToolButton("新建版本", ToolbarCreateVersionClick));
            toolbar.Items.Add(CreateToolButton("保存条件", ToolbarSaveConditionsClick));
            toolbar.Items.Add(CreateToolButton("保存动作", ToolbarSaveActionsClick));
            toolbar.Items.Add(CreateToolButton("发布", ToolbarPublishClick));
            toolbar.Items.Add(CreateToolButton("停用", ToolbarDisableClick));
            toolbar.Items.Add(CreateToolButton("回滚", ToolbarRollbackClick));
            toolbar.Dock = DockStyle.Top;

            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 44;
            CreateSearchControls(searchPanel);

            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.SplitterDistance = 430;

            _gridRules = CreateReadOnlyGrid();
            _gridRules.Dock = DockStyle.Fill;
            _gridRules.SelectionChanged += GridRulesSelectionChanged;
            split.Panel1.Controls.Add(_gridRules);

            _tabs = new TabControl();
            _tabs.Dock = DockStyle.Fill;
            _tabs.TabPages.Add(CreateBasicTab());
            _tabs.TabPages.Add(CreateConditionTab());
            _tabs.TabPages.Add(CreateActionTab());
            _tabs.TabPages.Add(CreateVersionTab());
            _tabs.TabPages.Add(CreateDictTab());
            _tabs.TabPages.Add(CreateFormulaTab());
            split.Panel2.Controls.Add(_tabs);

            Controls.Add(split);
            Controls.Add(searchPanel);
            Controls.Add(toolbar);
        }

        private static ToolStripButton CreateToolButton(string text, EventHandler handler)
        {
            ToolStripButton button = new ToolStripButton(text);
            button.Click += handler;
            return button;
        }

        private void CreateSearchControls(Control parent)
        {
            Label itemLabel = new Label();
            itemLabel.Text = "项目编码";
            itemLabel.Location = new Point(12, 13);
            itemLabel.Size = new Size(60, 22);
            parent.Controls.Add(itemLabel);

            _txtSearchItemCode = new TextBox();
            _txtSearchItemCode.Location = new Point(75, 10);
            _txtSearchItemCode.Size = new Size(120, 22);
            parent.Controls.Add(_txtSearchItemCode);

            Label statusLabel = new Label();
            statusLabel.Text = "状态";
            statusLabel.Location = new Point(210, 13);
            statusLabel.Size = new Size(40, 22);
            parent.Controls.Add(statusLabel);

            _cboSearchStatus = new ComboBox();
            _cboSearchStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboSearchStatus.Location = new Point(255, 10);
            _cboSearchStatus.Size = new Size(120, 22);
            parent.Controls.Add(_cboSearchStatus);

            Label categoryLabel = new Label();
            categoryLabel.Text = "类别";
            categoryLabel.Location = new Point(390, 13);
            categoryLabel.Size = new Size(40, 22);
            parent.Controls.Add(categoryLabel);

            _cboSearchCategory = new ComboBox();
            _cboSearchCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboSearchCategory.Location = new Point(435, 10);
            _cboSearchCategory.Size = new Size(140, 22);
            parent.Controls.Add(_cboSearchCategory);
        }

        private TabPage CreateBasicTab()
        {
            TabPage page = new TabPage("基本信息");
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            page.Controls.Add(panel);

            _txtRuleCode = AddTextBox(panel, "规则编码", 20, 20, 210);
            _txtRuleName = AddTextBox(panel, "规则名称", 20, 58, 460);
            _cboRuleCategory = AddComboBox(panel, "规则类别", 20, 96, 180);
            _cboRuleScope = AddComboBox(panel, "规则范围", 310, 96, 180);
            _txtItemCode = AddTextBox(panel, "项目编码", 20, 134, 180);
            _txtItemName = AddTextBox(panel, "项目名称", 310, 134, 320);
            _txtGroupCode = AddTextBox(panel, "项目组", 20, 172, 180);

            Label priorityLabel = AddLabel(panel, "优先级", 310, 176);
            _numPriority = new NumericUpDown();
            _numPriority.Location = new Point(priorityLabel.Right + 6, 172);
            _numPriority.Size = new Size(90, 22);
            _numPriority.Maximum = 99999;
            _numPriority.Value = 100;
            panel.Controls.Add(_numPriority);

            _chkHasEffectiveFrom = new CheckBox();
            _chkHasEffectiveFrom.Text = "生效时间";
            _chkHasEffectiveFrom.Location = new Point(20, 212);
            _chkHasEffectiveFrom.Size = new Size(80, 24);
            panel.Controls.Add(_chkHasEffectiveFrom);

            _dtEffectiveFrom = new DateTimePicker();
            _dtEffectiveFrom.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            _dtEffectiveFrom.Format = DateTimePickerFormat.Custom;
            _dtEffectiveFrom.Location = new Point(105, 212);
            _dtEffectiveFrom.Size = new Size(170, 22);
            panel.Controls.Add(_dtEffectiveFrom);

            _chkHasEffectiveTo = new CheckBox();
            _chkHasEffectiveTo.Text = "失效时间";
            _chkHasEffectiveTo.Location = new Point(310, 212);
            _chkHasEffectiveTo.Size = new Size(80, 24);
            panel.Controls.Add(_chkHasEffectiveTo);

            _dtEffectiveTo = new DateTimePicker();
            _dtEffectiveTo.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            _dtEffectiveTo.Format = DateTimePickerFormat.Custom;
            _dtEffectiveTo.Location = new Point(395, 212);
            _dtEffectiveTo.Size = new Size(170, 22);
            panel.Controls.Add(_dtEffectiveTo);

            AddLabel(panel, "备注", 20, 254);
            _txtRemark = new TextBox();
            _txtRemark.Location = new Point(95, 250);
            _txtRemark.Size = new Size(570, 90);
            _txtRemark.Multiline = true;
            panel.Controls.Add(_txtRemark);

            return page;
        }

        private TabPage CreateConditionTab()
        {
            TabPage page = new TabPage("条件配置");
            _gridConditions = CreateEditableGrid();
            _gridConditions.Dock = DockStyle.Fill;
            page.Controls.Add(_gridConditions);
            return page;
        }

        private TabPage CreateActionTab()
        {
            TabPage page = new TabPage("动作配置");
            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Horizontal;
            split.SplitterDistance = 320;

            _gridActions = CreateEditableGrid();
            _gridActions.Dock = DockStyle.Fill;
            split.Panel1.Controls.Add(_gridActions);

            Panel formulaPanel = new Panel();
            formulaPanel.Dock = DockStyle.Fill;
            Label formulaLabel = new Label();
            formulaLabel.Text = "公式";
            formulaLabel.Location = new Point(12, 14);
            formulaLabel.Size = new Size(60, 22);
            formulaPanel.Controls.Add(formulaLabel);

            _cboFormula = new ComboBox();
            _cboFormula.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboFormula.Location = new Point(75, 10);
            _cboFormula.Size = new Size(260, 22);
            _cboFormula.SelectedIndexChanged += CboFormulaSelectedIndexChanged;
            formulaPanel.Controls.Add(_cboFormula);

            Label paramLabel = new Label();
            paramLabel.Text = "参数结构";
            paramLabel.Location = new Point(12, 48);
            paramLabel.Size = new Size(70, 22);
            formulaPanel.Controls.Add(paramLabel);

            _txtFormulaParams = new TextBox();
            _txtFormulaParams.Location = new Point(75, 46);
            _txtFormulaParams.Size = new Size(620, 92);
            _txtFormulaParams.Multiline = true;
            _txtFormulaParams.ReadOnly = true;
            formulaPanel.Controls.Add(_txtFormulaParams);
            split.Panel2.Controls.Add(formulaPanel);

            page.Controls.Add(split);
            return page;
        }

        private TabPage CreateVersionTab()
        {
            TabPage page = new TabPage("版本历史");
            SplitContainer splitOuter = new SplitContainer();
            splitOuter.Dock = DockStyle.Fill;
            splitOuter.Orientation = Orientation.Horizontal;
            splitOuter.SplitterDistance = 220;

            _gridVersions = CreateReadOnlyGrid();
            _gridVersions.Dock = DockStyle.Fill;
            splitOuter.Panel1.Controls.Add(_gridVersions);

            SplitContainer splitInner = new SplitContainer();
            splitInner.Dock = DockStyle.Fill;
            splitInner.SplitterDistance = 340;
            _gridPublishHistory = CreateReadOnlyGrid();
            _gridPublishHistory.Dock = DockStyle.Fill;
            _gridChangeLogs = CreateReadOnlyGrid();
            _gridChangeLogs.Dock = DockStyle.Fill;
            splitInner.Panel1.Controls.Add(_gridPublishHistory);
            splitInner.Panel2.Controls.Add(_gridChangeLogs);
            splitOuter.Panel2.Controls.Add(splitInner);

            page.Controls.Add(splitOuter);
            return page;
        }

        private TabPage CreateDictTab()
        {
            TabPage page = new TabPage("字典");
            Panel panel = new Panel();
            panel.Dock = DockStyle.Top;
            panel.Height = 42;

            Label label = new Label();
            label.Text = "字典类型";
            label.Location = new Point(12, 13);
            label.Size = new Size(70, 22);
            panel.Controls.Add(label);

            _cboDictType = new ComboBox();
            _cboDictType.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboDictType.Location = new Point(85, 10);
            _cboDictType.Size = new Size(200, 22);
            _cboDictType.SelectedIndexChanged += CboDictTypeSelectedIndexChanged;
            panel.Controls.Add(_cboDictType);

            Button refresh = new Button();
            refresh.Text = "刷新";
            refresh.Location = new Point(300, 8);
            refresh.Click += DictRefreshClick;
            panel.Controls.Add(refresh);

            _gridDicts = CreateReadOnlyGrid();
            _gridDicts.Dock = DockStyle.Fill;
            page.Controls.Add(_gridDicts);
            page.Controls.Add(panel);
            return page;
        }

        private TabPage CreateFormulaTab()
        {
            TabPage page = new TabPage("公式");
            _gridFormulas = CreateReadOnlyGrid();
            _gridFormulas.Dock = DockStyle.Fill;
            page.Controls.Add(_gridFormulas);
            return page;
        }

        private static Label AddLabel(Control parent, string text, int left, int top)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(left, top);
            label.Size = new Size(70, 22);
            parent.Controls.Add(label);
            return label;
        }

        private static TextBox AddTextBox(Control parent, string labelText, int left, int top, int width)
        {
            Label label = AddLabel(parent, labelText, left, top + 4);
            TextBox textBox = new TextBox();
            textBox.Location = new Point(label.Right + 6, top);
            textBox.Size = new Size(width, 22);
            parent.Controls.Add(textBox);
            return textBox;
        }

        private static ComboBox AddComboBox(Control parent, string labelText, int left, int top, int width)
        {
            Label label = AddLabel(parent, labelText, left, top + 4);
            ComboBox combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Location = new Point(label.Right + 6, top);
            combo.Size = new Size(width, 22);
            parent.Controls.Add(combo);
            return combo;
        }

        private static DataGridView CreateReadOnlyGrid()
        {
            DataGridView grid = new DataGridView();
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            return grid;
        }

        private static DataGridView CreateEditableGrid()
        {
            DataGridView grid = new DataGridView();
            grid.AllowUserToAddRows = true;
            grid.AllowUserToDeleteRows = true;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            return grid;
        }

        private void ToolbarSearchClick(object sender, EventArgs e)
        {
            SearchRules();
        }

        private void ToolbarNewClick(object sender, EventArgs e)
        {
            _creatingNew = true;
            _selectedRule = null;
            ClearBasicFields();
            _txtRuleCode.ReadOnly = false;
            _txtRuleCode.Text = "RULE_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            _numPriority.Value = 100;
            BindConditions(new List<RuleConditionItemRequest>());
            BindActions(new List<RuleActionItemRequest>());
            _gridVersions.DataSource = null;
            _gridPublishHistory.DataSource = null;
            _gridChangeLogs.DataSource = null;
        }

        private void ToolbarSaveClick(object sender, EventArgs e)
        {
            try
            {
                if (_creatingNew)
                {
                    ApiResponse<long> response = _client.CreateRule(BuildCreateRequest());
                    EnsureSuccess(response);
                    _creatingNew = false;
                    SearchRules();
                    MessageBox.Show(this, "规则已保存，请创建草稿版本后维护条件和动作。", "保存成功");
                }
                else
                {
                    EnsureRuleSelected();
                    ApiResponse response = _client.UpdateRule(_selectedRule.RuleId, BuildUpdateRequest());
                    EnsureSuccess(response);
                    SearchRules();
                    MessageBox.Show(this, "规则已保存。", "保存成功");
                }
            }
            catch (Exception ex)
            {
                ShowError("保存规则失败", ex);
            }
        }

        private void ToolbarCreateVersionClick(object sender, EventArgs e)
        {
            try
            {
                EnsureRuleSelected();
                ApiResponse<long> response = _client.CreateDraftVersion(_selectedRule.RuleId);
                EnsureSuccess(response);
                LoadRuleDetail(_selectedRule.RuleId);
                MessageBox.Show(this, "草稿版本已创建。", "新建版本");
            }
            catch (Exception ex)
            {
                ShowError("新建版本失败", ex);
            }
        }

        private void ToolbarSaveConditionsClick(object sender, EventArgs e)
        {
            try
            {
                EnsureRuleSelected();
                int versionNo = GetEditableVersionNo();
                ApiResponse response = _client.SaveConditions(_selectedRule.RuleId, versionNo, BuildConditionSaveRequest());
                EnsureSuccess(response);
                LoadConditions(_selectedRule.RuleId, versionNo);
                MessageBox.Show(this, "条件已保存。", "保存条件");
            }
            catch (Exception ex)
            {
                ShowError("保存条件失败", ex);
            }
        }

        private void ToolbarSaveActionsClick(object sender, EventArgs e)
        {
            try
            {
                EnsureRuleSelected();
                int versionNo = GetEditableVersionNo();
                ApiResponse response = _client.SaveActions(_selectedRule.RuleId, versionNo, BuildActionSaveRequest());
                EnsureSuccess(response);
                LoadActions(_selectedRule.RuleId, versionNo);
                MessageBox.Show(this, "动作已保存。", "保存动作");
            }
            catch (Exception ex)
            {
                ShowError("保存动作失败", ex);
            }
        }

        private void ToolbarPublishClick(object sender, EventArgs e)
        {
            try
            {
                EnsureRuleSelected();
                int versionNo = GetEditableVersionNo();
                ApiResponse response = _client.PublishRule(_selectedRule.RuleId, new RulePublishRequest
                {
                    VersionNo = versionNo,
                    PublishedBy = _operatorId,
                    Remark = "HIS 工作台发布"
                });
                EnsureSuccess(response);
                LoadRuleDetail(_selectedRule.RuleId);
                SearchRules();
                MessageBox.Show(this, "规则已发布。", "发布");
            }
            catch (Exception ex)
            {
                ShowError("发布失败", ex);
            }
        }

        private void ToolbarDisableClick(object sender, EventArgs e)
        {
            try
            {
                EnsureRuleSelected();
                ApiResponse response = _client.DisableRule(_selectedRule.RuleId, new RuleDisableRequest
                {
                    PublishedBy = _operatorId,
                    Remark = "HIS 工作台停用"
                });
                EnsureSuccess(response);
                LoadRuleDetail(_selectedRule.RuleId);
                SearchRules();
                MessageBox.Show(this, "规则已停用。", "停用");
            }
            catch (Exception ex)
            {
                ShowError("停用失败", ex);
            }
        }

        private void ToolbarRollbackClick(object sender, EventArgs e)
        {
            try
            {
                EnsureRuleSelected();
                ApiResponse response = _client.RollbackRule(_selectedRule.RuleId, new RuleRollbackRequest
                {
                    PublishedBy = _operatorId,
                    Remark = "HIS 工作台回滚"
                });
                EnsureSuccess(response);
                LoadRuleDetail(_selectedRule.RuleId);
                SearchRules();
                MessageBox.Show(this, "规则已回滚。", "回滚");
            }
            catch (Exception ex)
            {
                ShowError("回滚失败", ex);
            }
        }

        private void GridRulesSelectionChanged(object sender, EventArgs e)
        {
            RuleHeaderResponse row = GetCurrentRuleRow();
            if (row == null || _selectedRule != null && _selectedRule.RuleId == row.RuleId)
            {
                return;
            }

            try
            {
                _creatingNew = false;
                LoadRuleDetail(row.RuleId);
            }
            catch (Exception ex)
            {
                ShowError("加载规则详情失败", ex);
            }
        }

        private void CboFormulaSelectedIndexChanged(object sender, EventArgs e)
        {
            FormulaDefResponse formula = _cboFormula.SelectedItem as FormulaDefResponse;
            _txtFormulaParams.Text = formula == null ? string.Empty : formula.ParamSchemaJson;
        }

        private void CboDictTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedDict();
        }

        private void DictRefreshClick(object sender, EventArgs e)
        {
            LoadSelectedDict();
        }

        private void LoadDictionaryData()
        {
            _ruleCategories = SafeData(_client.GetDicts("RULE_CATEGORY"));
            _ruleStatuses = SafeData(_client.GetDicts("RULE_STATUS"));
            _ruleScopes = SafeData(_client.GetDicts("RULE_SCOPE"));
            _formulas = SafeData(_client.GetFormulas());

            BindDictCombo(_cboSearchCategory, _ruleCategories, true);
            BindDictCombo(_cboRuleCategory, _ruleCategories, false);
            BindDictCombo(_cboSearchStatus, _ruleStatuses, true);
            BindDictCombo(_cboRuleScope, _ruleScopes, false);
            BindFormulaCombo();
            LoadDictTypes();
            _gridFormulas.DataSource = _formulas;
        }

        private void LoadDictTypes()
        {
            try
            {
                ApiResponse<List<string>> response = _client.GetDictTypes();
                EnsureSuccess(response);
                List<string> types = response.Data ?? new List<string>();
                _cboDictType.Items.Clear();
                foreach (string type in types)
                {
                    _cboDictType.Items.Add(type);
                }

                if (_cboDictType.Items.Count > 0)
                {
                    _cboDictType.SelectedIndex = 0;
                }
            }
            catch
            {
                _cboDictType.Items.Clear();
            }
        }

        private void LoadSelectedDict()
        {
            if (_cboDictType.SelectedItem == null)
            {
                return;
            }

            try
            {
                _gridDicts.DataSource = SafeData(_client.GetDicts(_cboDictType.SelectedItem.ToString()));
            }
            catch (Exception ex)
            {
                ShowError("加载字典失败", ex);
            }
        }

        private static void BindDictCombo(ComboBox combo, List<DictResponse> data, bool includeAll)
        {
            combo.Items.Clear();
            if (includeAll)
            {
                combo.Items.Add(new ComboItem(string.Empty, "全部"));
            }

            if (data != null)
            {
                foreach (DictResponse item in data)
                {
                    combo.Items.Add(new ComboItem(item.DictCode, item.DictName));
                }
            }

            if (combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }

        private void BindFormulaCombo()
        {
            _cboFormula.Items.Clear();
            if (_formulas != null)
            {
                foreach (FormulaDefResponse formula in _formulas)
                {
                    _cboFormula.Items.Add(formula);
                }
            }

            _cboFormula.DisplayMember = "FormulaName";
            _cboFormula.ValueMember = "ExecutorCode";
            if (_cboFormula.Items.Count > 0)
            {
                _cboFormula.SelectedIndex = 0;
            }
        }

        private void SearchRules()
        {
            try
            {
                string status = GetComboValue(_cboSearchStatus);
                string category = GetComboValue(_cboSearchCategory);
                ApiResponse<PagedResponse<RuleHeaderResponse>> response = _client.GetRules(
                    _txtSearchItemCode.Text.Trim(),
                    status,
                    category,
                    1,
                    200);
                EnsureSuccess(response);
                _gridRules.DataSource = response.Data == null ? new List<RuleHeaderResponse>() : response.Data.Items;
            }
            catch (Exception ex)
            {
                ShowError("查询规则失败", ex);
            }
        }

        private void LoadRuleDetail(long ruleId)
        {
            ApiResponse<RuleHeaderResponse> ruleResponse = _client.GetRule(ruleId);
            EnsureSuccess(ruleResponse);
            _selectedRule = ruleResponse.Data;
            BindBasicFields(_selectedRule);
            LoadVersions(ruleId);
            int versionNo = GetDisplayVersionNo();
            if (versionNo > 0)
            {
                LoadConditions(ruleId, versionNo);
                LoadActions(ruleId, versionNo);
            }
            else
            {
                BindConditions(new List<RuleConditionItemRequest>());
                BindActions(new List<RuleActionItemRequest>());
            }

            _gridPublishHistory.DataSource = SafeData(_client.GetPublishHistory(ruleId));
            _gridChangeLogs.DataSource = SafeData(_client.GetChangeLogs(ruleId));
        }

        private void LoadVersions(long ruleId)
        {
            _versions = SafeData(_client.GetRuleVersions(ruleId));
            _gridVersions.DataSource = _versions;
        }

        private void LoadConditions(long ruleId, int versionNo)
        {
            List<RuleConditionResponse> responseItems = SafeData(_client.GetConditions(ruleId, versionNo));
            List<RuleConditionItemRequest> rows = new List<RuleConditionItemRequest>();
            foreach (RuleConditionResponse item in responseItems)
            {
                rows.Add(new RuleConditionItemRequest
                {
                    ConditionGroup = item.ConditionGroup,
                    ConditionType = item.ConditionType,
                    OperatorType = item.OperatorType,
                    LeftKey = item.LeftKey,
                    RightValue = item.RightValue,
                    ParamsJson = item.ParamsJson,
                    SortNo = item.SortNo,
                    IsEnabled = item.IsEnabled
                });
            }

            BindConditions(rows);
        }

        private void LoadActions(long ruleId, int versionNo)
        {
            List<RuleActionResponse> responseItems = SafeData(_client.GetActions(ruleId, versionNo));
            List<RuleActionItemRequest> rows = new List<RuleActionItemRequest>();
            foreach (RuleActionResponse item in responseItems)
            {
                rows.Add(new RuleActionItemRequest
                {
                    ActionType = item.ActionType,
                    ExecutorCode = item.ExecutorCode,
                    ParamsJson = item.ParamsJson,
                    ExclusiveGroup = item.ExclusiveGroup,
                    SortNo = item.SortNo,
                    OnError = item.OnError,
                    IsEnabled = item.IsEnabled
                });
            }

            BindActions(rows);
        }

        private void BindBasicFields(RuleHeaderResponse rule)
        {
            _txtRuleCode.ReadOnly = true;
            _txtRuleCode.Text = Safe(rule.RuleCode);
            _txtRuleName.Text = Safe(rule.RuleName);
            SetComboValue(_cboRuleCategory, rule.RuleCategory);
            SetComboValue(_cboRuleScope, rule.RuleScope);
            _txtItemCode.Text = Safe(rule.ItemCode);
            _txtItemName.Text = Safe(rule.ItemName);
            _txtGroupCode.Text = Safe(rule.GroupCode);
            _numPriority.Value = rule.Priority < _numPriority.Minimum ? _numPriority.Minimum : rule.Priority;
            _chkHasEffectiveFrom.Checked = rule.EffectiveFrom.HasValue;
            _chkHasEffectiveTo.Checked = rule.EffectiveTo.HasValue;
            if (rule.EffectiveFrom.HasValue)
            {
                _dtEffectiveFrom.Value = rule.EffectiveFrom.Value;
            }

            if (rule.EffectiveTo.HasValue)
            {
                _dtEffectiveTo.Value = rule.EffectiveTo.Value;
            }

            _txtRemark.Text = Safe(rule.Remark);
        }

        private void ClearBasicFields()
        {
            _txtRuleCode.Text = string.Empty;
            _txtRuleName.Text = string.Empty;
            _txtItemCode.Text = string.Empty;
            _txtItemName.Text = string.Empty;
            _txtGroupCode.Text = string.Empty;
            _txtRemark.Text = string.Empty;
            if (_cboRuleCategory.Items.Count > 0)
            {
                _cboRuleCategory.SelectedIndex = 0;
            }

            if (_cboRuleScope.Items.Count > 0)
            {
                _cboRuleScope.SelectedIndex = 0;
            }

            _chkHasEffectiveFrom.Checked = false;
            _chkHasEffectiveTo.Checked = false;
        }

        private void BindConditions(List<RuleConditionItemRequest> rows)
        {
            _gridConditions.DataSource = new BindingList<RuleConditionItemRequest>(rows);
        }

        private void BindActions(List<RuleActionItemRequest> rows)
        {
            _gridActions.DataSource = new BindingList<RuleActionItemRequest>(rows);
        }

        private RuleHeaderCreateRequest BuildCreateRequest()
        {
            return new RuleHeaderCreateRequest
            {
                RuleCode = _txtRuleCode.Text.Trim(),
                RuleName = _txtRuleName.Text.Trim(),
                RuleCategory = GetComboValue(_cboRuleCategory),
                RuleScope = GetComboValue(_cboRuleScope),
                ItemCode = EmptyToNull(_txtItemCode.Text),
                ItemName = EmptyToNull(_txtItemName.Text),
                GroupCode = EmptyToNull(_txtGroupCode.Text),
                Priority = Convert.ToInt32(_numPriority.Value),
                EffectiveFrom = _chkHasEffectiveFrom.Checked ? (DateTime?)_dtEffectiveFrom.Value : null,
                EffectiveTo = _chkHasEffectiveTo.Checked ? (DateTime?)_dtEffectiveTo.Value : null,
                Remark = EmptyToNull(_txtRemark.Text),
                CreatedBy = _operatorId
            };
        }

        private RuleHeaderUpdateRequest BuildUpdateRequest()
        {
            return new RuleHeaderUpdateRequest
            {
                RuleName = _txtRuleName.Text.Trim(),
                RuleCategory = GetComboValue(_cboRuleCategory),
                RuleScope = GetComboValue(_cboRuleScope),
                ItemCode = EmptyToNull(_txtItemCode.Text),
                ItemName = EmptyToNull(_txtItemName.Text),
                GroupCode = EmptyToNull(_txtGroupCode.Text),
                Priority = Convert.ToInt32(_numPriority.Value),
                EffectiveFrom = _chkHasEffectiveFrom.Checked ? (DateTime?)_dtEffectiveFrom.Value : null,
                EffectiveTo = _chkHasEffectiveTo.Checked ? (DateTime?)_dtEffectiveTo.Value : null,
                Remark = EmptyToNull(_txtRemark.Text),
                UpdatedBy = _operatorId
            };
        }

        private RuleConditionSaveRequest BuildConditionSaveRequest()
        {
            return new RuleConditionSaveRequest
            {
                Conditions = ReadRows<RuleConditionItemRequest>(_gridConditions)
            };
        }

        private RuleActionSaveRequest BuildActionSaveRequest()
        {
            return new RuleActionSaveRequest
            {
                Actions = ReadRows<RuleActionItemRequest>(_gridActions)
            };
        }

        private static List<T> ReadRows<T>(DataGridView grid)
        {
            List<T> rows = new List<T>();
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (!row.IsNewRow && row.DataBoundItem is T)
                {
                    rows.Add((T)row.DataBoundItem);
                }
            }

            return rows;
        }

        private RuleHeaderResponse GetCurrentRuleRow()
        {
            if (_gridRules.CurrentRow == null)
            {
                return null;
            }

            return _gridRules.CurrentRow.DataBoundItem as RuleHeaderResponse;
        }

        private int GetEditableVersionNo()
        {
            if (_versions == null || _versions.Count == 0)
            {
                throw new InvalidOperationException("当前规则没有版本，请先新建草稿版本。");
            }

            RuleVersionResponse draft = null;
            foreach (RuleVersionResponse version in _versions)
            {
                if (string.Equals(version.VersionStatus, "DRAFT", StringComparison.OrdinalIgnoreCase))
                {
                    draft = version;
                }
            }

            if (draft != null)
            {
                return draft.VersionNo;
            }

            return _versions[_versions.Count - 1].VersionNo;
        }

        private int GetDisplayVersionNo()
        {
            if (_versions == null || _versions.Count == 0)
            {
                return 0;
            }

            if (_selectedRule != null && _selectedRule.CurrentVersion > 0)
            {
                return _selectedRule.CurrentVersion;
            }

            return _versions[_versions.Count - 1].VersionNo;
        }

        private void EnsureRuleSelected()
        {
            if (_selectedRule == null)
            {
                throw new InvalidOperationException("请先选择规则。");
            }
        }

        private static void EnsureSuccess(ApiResponse response)
        {
            if (response == null)
            {
                throw new PricingApiException("计价服务返回空响应", null);
            }

            if (!response.IsSuccess)
            {
                throw new PricingApiException(response.Message, null);
            }
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
        }

        private static List<T> SafeData<T>(ApiResponse<List<T>> response)
        {
            EnsureSuccess(response);
            return response.Data ?? new List<T>();
        }

        private static string GetComboValue(ComboBox combo)
        {
            ComboItem item = combo.SelectedItem as ComboItem;
            if (item != null)
            {
                return item.Value;
            }

            return combo.SelectedItem == null ? string.Empty : combo.SelectedItem.ToString();
        }

        private static void SetComboValue(ComboBox combo, string value)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                ComboItem item = combo.Items[i] as ComboItem;
                if (item != null && string.Equals(item.Value, value, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        private static string EmptyToNull(string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim().Length == 0 ? null : value.Trim();
        }

        private static string Safe(string value)
        {
            return value == null ? string.Empty : value;
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private sealed class ComboItem
        {
            public readonly string Value;
            private readonly string _text;

            public ComboItem(string value, string text)
            {
                Value = value;
                _text = text;
            }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(Value))
                {
                    return _text;
                }

                return _text + " (" + Value + ")";
            }
        }
    }
}
