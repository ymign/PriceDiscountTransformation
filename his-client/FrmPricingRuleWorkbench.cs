using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// 特殊计价规则维护工作台。
    /// 供物价管理员在 HIS 客户端内完成规则的全生命周期管理：
    /// 查询 -> 新建 -> 编辑基本信息 -> 创建草稿版本 -> 配置条件 -> 配置动作 -> 发布 -> 停用/回滚。
    ///
    /// 界面布局：
    /// - 顶部工具栏：查询、新建、保存、新建版本、保存条件、保存动作、发布、停用、回滚
    /// - 搜索栏：项目编码、状态、类别筛选
    /// - 左侧：规则列表网格
    /// - 右侧：选项卡（基本信息、条件配置、动作配置、版本历史、字典、公式）
    ///
    /// 业务约束：
    /// - 草稿版本可编辑，已发布版本不可变
    /// - 同一规则同时只允许一个 DRAFT 版本
    /// - 发布前服务端校验规则冲突、完整性等阻断项
    /// - NULL ≠ 0：前端空值必须存为 NULL（EmptyToNull 方法保证）
    /// </summary>
    public sealed class FrmPricingRuleWorkbench : Form
    {
        /// <summary>
        /// 统一计价服务 HTTP 客户端。所有与计价服务的通信均通过此对象。
        /// </summary>
        private readonly PricingApiClient _client;

        /// <summary>
        /// 当前操作员工号。记录到规则的 CreatedBy/UpdatedBy/PublishedBy 等审计字段。
        /// 可为 null（构造时未传入），此时审计字段为空。
        /// </summary>
        private readonly string _operatorId;

        // ================================================================
        // 搜索栏控件
        // ================================================================

        /// <summary>搜索条件：项目编码文本框</summary>
        private TextBox _txtSearchItemCode;

        /// <summary>搜索条件：状态下拉框（字典绑定）</summary>
        private ComboBox _cboSearchStatus;

        /// <summary>搜索条件：类别下拉框（字典绑定）</summary>
        private ComboBox _cboSearchCategory;

        // ================================================================
        // 规则列表网格
        // ================================================================

        /// <summary>规则列表网格，左侧展示查询结果，选中行触发详情加载</summary>
        private DataGridView _gridRules;

        // ================================================================
        // 右侧选项卡
        // ================================================================

        /// <summary>右侧选项卡控件，包含基本信息、条件、动作、版本历史、字典、公式六个页签</summary>
        private TabControl _tabs;

        // ================================================================
        // 基本信息页签控件
        // ================================================================

        /// <summary>规则编码文本框（新建时可编辑，编辑时只读）</summary>
        private TextBox _txtRuleCode;

        /// <summary>规则名称文本框</summary>
        private TextBox _txtRuleName;

        /// <summary>规则类别下拉框（字典绑定）</summary>
        private ComboBox _cboRuleCategory;

        /// <summary>规则范围下拉框（字典绑定）</summary>
        private ComboBox _cboRuleScope;

        /// <summary>项目编码文本框</summary>
        private TextBox _txtItemCode;

        /// <summary>项目名称文本框</summary>
        private TextBox _txtItemName;

        /// <summary>项目分组编码文本框</summary>
        private TextBox _txtGroupCode;

        /// <summary>优先级数值框（0-99999，默认 100）</summary>
        private NumericUpDown _numPriority;

        /// <summary>生效起始时间选择器</summary>
        private DateTimePicker _dtEffectiveFrom;

        /// <summary>生效截止时间选择器</summary>
        private DateTimePicker _dtEffectiveTo;

        /// <summary>是否设置生效起始时间的复选框（未勾选时传 NULL）</summary>
        private CheckBox _chkHasEffectiveFrom;

        /// <summary>是否设置生效截止时间的复选框（未勾选时传 NULL）</summary>
        private CheckBox _chkHasEffectiveTo;

        /// <summary>回滚模式下拉框（STOP_CHARGE / LEGACY_EQUIVALENT / MANUAL_REVIEW）</summary>
        private ComboBox _cboRollbackMode;

        /// <summary>备注文本框</summary>
        private TextBox _txtRemark;

        // ================================================================
        // 条件/动作/版本/字典/公式页签控件
        // ================================================================

        /// <summary>条件配置网格（可编辑，支持新增/删除行）</summary>
        private DataGridView _gridConditions;

        /// <summary>动作配置网格（可编辑，支持新增/删除行）</summary>
        private DataGridView _gridActions;

        /// <summary>版本历史网格（只读）</summary>
        private DataGridView _gridVersions;

        /// <summary>发布历史网格（只读）</summary>
        private DataGridView _gridPublishHistory;

        /// <summary>变更日志网格（只读）</summary>
        private DataGridView _gridChangeLogs;

        /// <summary>公式选择下拉框（动作配置页签中使用）</summary>
        private ComboBox _cboFormula;

        /// <summary>公式参数结构展示文本框（只读，展示选中公式的 ParamSchemaJson）</summary>
        private TextBox _txtFormulaParams;

        /// <summary>字典类型选择下拉框</summary>
        private ComboBox _cboDictType;

        /// <summary>字典项列表网格</summary>
        private DataGridView _gridDicts;

        /// <summary>公式定义列表网格</summary>
        private DataGridView _gridFormulas;

        // ================================================================
        // 状态字段
        // ================================================================

        /// <summary>
        /// 当前选中的规则头信息。为 null 表示未选中任何规则（或处于新建模式）。
        /// 选中行变化时更新此字段，所有编辑操作以此为基准。
        /// </summary>
        private RuleHeaderResponse _selectedRule;

        /// <summary>当前规则的版本列表缓存，用于判断可编辑版本和展示版本号</summary>
        private List<RuleVersionResponse> _versions;

        /// <summary>规则类别字典缓存（用于搜索栏和基本信息页签的下拉框绑定）</summary>
        private List<DictResponse> _ruleCategories;

        /// <summary>规则状态字典缓存（用于搜索栏下拉框绑定）</summary>
        private List<DictResponse> _ruleStatuses;

        /// <summary>规则范围字典缓存（用于基本信息页签下拉框绑定）</summary>
        private List<DictResponse> _ruleScopes;

        /// <summary>公式定义列表缓存（用于动作配置页签的公式下拉框绑定）</summary>
        private List<FormulaDefResponse> _formulas;

        /// <summary>
        /// 是否处于新建模式。true 时表示用户点击了"新建"按钮，
        /// 保存操作走 CreateRule 路径；false 时走 UpdateRule 路径。
        /// </summary>
        private bool _creatingNew;

        /// <summary>
        /// 构造规则维护工作台（带操作员信息）。
        /// </summary>
        /// <param name="client">统一计价服务 HTTP 客户端，不可为 null</param>
        /// <param name="operatorId">操作员工号，用于审计字段</param>
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

        /// <summary>
        /// 构造规则维护工作台（不带操作员信息，审计字段为空）。
        /// </summary>
        /// <param name="client">统一计价服务 HTTP 客户端</param>
        public FrmPricingRuleWorkbench(PricingApiClient client)
            : this(client, null)
        {
        }

        /// <summary>
        /// 窗口加载时初始化字典数据和查询规则列表。
        /// OnLoad 在窗口显示之前触发，确保用户看到窗口时数据已就绪。
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadDictionaryData();
            SearchRules();
        }

        /// <summary>
        /// 初始化界面控件。纯代码布局（无 .Designer.cs）。
        /// 按以下结构组织：
        /// - 工具栏（顶部 Dock）
        /// - 搜索栏（顶部 Dock，工具栏下方）
        /// - 分割容器（Fill）：左侧规则列表 + 右侧选项卡
        /// </summary>
        private void InitializeComponent()
        {
            Text = "特殊计价规则维护";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1180, 760);

            // ========== 工具栏 ==========
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

            // ========== 搜索栏 ==========
            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 44;
            CreateSearchControls(searchPanel);

            // ========== 主体分割容器 ==========
            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.SplitterDistance = 430;

            // 左侧：规则列表网格
            _gridRules = CreateReadOnlyGrid();
            _gridRules.Dock = DockStyle.Fill;
            _gridRules.SelectionChanged += GridRulesSelectionChanged;
            split.Panel1.Controls.Add(_gridRules);

            // 右侧：选项卡（6 个页签）
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

        /// <summary>创建工具栏按钮的工厂方法</summary>
        /// <param name="text">按钮文本</param>
        /// <param name="handler">点击事件处理器</param>
        /// <returns>配置好的 ToolStripButton</returns>
        private static ToolStripButton CreateToolButton(string text, EventHandler handler)
        {
            ToolStripButton button = new ToolStripButton(text);
            button.Click += handler;
            return button;
        }

        /// <summary>
        /// 创建搜索栏控件。包含项目编码文本框、状态下拉框、类别下拉框。
        /// 下拉框数据在 LoadDictionaryData 中绑定。
        /// </summary>
        /// <param name="parent">搜索栏面板</param>
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

        /// <summary>
        /// 创建"基本信息"页签。包含规则头的所有可编辑字段：
        /// 规则编码、名称、类别、范围、项目编码/名称、分组、优先级、生效时间、备注。
        /// </summary>
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

            // 生效时间：通过复选框控制是否传值（未勾选时传 NULL 表示不限制）
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

            _cboRollbackMode = AddComboBox(panel, "回滚模式", 20, 250, 220);

            AddLabel(panel, "备注", 20, 292);
            _txtRemark = new TextBox();
            _txtRemark.Location = new Point(95, 288);
            _txtRemark.Size = new Size(570, 70);
            _txtRemark.Multiline = true;
            panel.Controls.Add(_txtRemark);

            return page;
        }

        /// <summary>
        /// 创建"条件配置"页签。使用可编辑网格，支持新增/删除行。
        /// 条件定义规则匹配的前提，引擎按 ConditionGroup 分组求值。
        /// </summary>
        private TabPage CreateConditionTab()
        {
            TabPage page = new TabPage("条件配置");
            _gridConditions = CreateEditableGrid();
            _gridConditions.Dock = DockStyle.Fill;
            page.Controls.Add(_gridConditions);
            return page;
        }

        /// <summary>
        /// 创建"动作配置"页签。分为上下两部分：
        /// - 上部：动作网格（可编辑）
        /// - 下部：公式选择区（下拉框 + 参数结构展示）
        /// 动作定义规则命中后执行的计价操作。
        /// </summary>
        private TabPage CreateActionTab()
        {
            TabPage page = new TabPage("动作配置");
            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Horizontal;
            split.SplitterDistance = 320;

            // 上部：动作网格
            _gridActions = CreateEditableGrid();
            _gridActions.Dock = DockStyle.Fill;
            split.Panel1.Controls.Add(_gridActions);

            // 下部：公式选择区
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

        /// <summary>
        /// 创建"版本历史"页签。分为三部分：
        /// - 上部：版本列表
        /// - 下部左：发布历史
        /// - 下部右：变更日志
        /// </summary>
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

        /// <summary>
        /// 创建"字典"页签。顶部选择字典类型，下方展示该类型的字典项列表。
        /// 字典表存储系统枚举值的可配置映射，支持不重新部署的情况下扩展枚举值。
        /// </summary>
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

        /// <summary>
        /// 创建"公式"页签。展示所有公式定义列表（只读）。
        /// 供物价管理员查看可用的计价公式及其参数结构。
        /// </summary>
        private TabPage CreateFormulaTab()
        {
            TabPage page = new TabPage("公式");
            _gridFormulas = CreateReadOnlyGrid();
            _gridFormulas.Dock = DockStyle.Fill;
            page.Controls.Add(_gridFormulas);
            return page;
        }

        // ================================================================
        // 控件工厂方法
        // ================================================================

        /// <summary>在指定父容器中创建标签控件</summary>
        /// <param name="parent">父容器</param>
        /// <param name="text">标签文本</param>
        /// <param name="left">左侧坐标</param>
        /// <param name="top">顶部坐标</param>
        /// <returns>创建的标签</returns>
        private static Label AddLabel(Control parent, string text, int left, int top)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(left, top);
            label.Size = new Size(70, 22);
            parent.Controls.Add(label);
            return label;
        }

        /// <summary>在指定父容器中创建"标签 + 文本框"组合</summary>
        /// <param name="parent">父容器</param>
        /// <param name="labelText">标签文本</param>
        /// <param name="left">标签左侧坐标</param>
        /// <param name="top">顶部坐标</param>
        /// <param name="width">文本框宽度</param>
        /// <returns>创建的文本框</returns>
        private static TextBox AddTextBox(Control parent, string labelText, int left, int top, int width)
        {
            Label label = AddLabel(parent, labelText, left, top + 4);
            TextBox textBox = new TextBox();
            textBox.Location = new Point(label.Right + 6, top);
            textBox.Size = new Size(width, 22);
            parent.Controls.Add(textBox);
            return textBox;
        }

        /// <summary>在指定父容器中创建"标签 + 下拉框"组合</summary>
        /// <param name="parent">父容器</param>
        /// <param name="labelText">标签文本</param>
        /// <param name="left">标签左侧坐标</param>
        /// <param name="top">顶部坐标</param>
        /// <param name="width">下拉框宽度</param>
        /// <returns>创建的下拉框</returns>
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

        /// <summary>
        /// 创建只读网格的标准配置。
        /// 禁止增删行、整行选择、自动调整列宽。
        /// </summary>
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

        /// <summary>
        /// 创建可编辑网格的标准配置。
        /// 允许增删行、单元格选择。
        /// 用于条件配置和动作配置页签。
        /// </summary>
        private static DataGridView CreateEditableGrid()
        {
            DataGridView grid = new DataGridView();
            grid.AllowUserToAddRows = true;
            grid.AllowUserToDeleteRows = true;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            return grid;
        }

        // ================================================================
        // 工具栏事件处理
        // ================================================================

        /// <summary>"查询"按钮：执行规则搜索</summary>
        private void ToolbarSearchClick(object sender, EventArgs e)
        {
            SearchRules();
        }

        /// <summary>
        /// "新建"按钮：进入新建模式。
        /// 清空基本信息字段，生成临时规则编码（RULE_ + 时间戳），
        /// 清空条件、动作、版本历史。
        /// </summary>
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

        /// <summary>
        /// "保存"按钮：保存规则头信息。
        /// 新建模式走 CreateRule（返回新 ID），编辑模式走 UpdateRule。
        /// 保存成功后刷新规则列表。
        /// </summary>
        private void ToolbarSaveClick(object sender, EventArgs e)
        {
            try
            {
                if (_creatingNew)
                {
                    // 新建模式：创建规则头，成功后退出新建模式
                    ApiResponse<long> response = _client.CreateRule(BuildCreateRequest());
                    EnsureSuccess(response);
                    _creatingNew = false;
                    SearchRules();
                    MessageBox.Show(this, "规则已保存，请创建草稿版本后维护条件和动作。", "保存成功");
                }
                else
                {
                    // 编辑模式：更新已有规则头
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

        /// <summary>
        /// "新建版本"按钮：为当前选中规则创建草稿版本。
        /// 同一规则同时只允许一个 DRAFT 版本。
        /// 创建成功后重新加载规则详情（含版本列表）。
        /// </summary>
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

        /// <summary>
        /// "保存条件"按钮：全量保存当前草稿版本的条件。
        /// 从可编辑网格读取所有行，替换服务端的条件列表。
        /// 仅 DRAFT 版本可保存。
        /// </summary>
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

        /// <summary>
        /// "保存动作"按钮：全量保存当前草稿版本的动作。
        /// 从可编辑网格读取所有行，替换服务端的动作列表。
        /// 仅 DRAFT 版本可保存。
        /// </summary>
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

        /// <summary>
        /// "发布"按钮：将当前草稿版本发布。
        /// 发布后版本状态变为 PUBLISHED，规则快照生成，缓存失效。
        /// 发布操作不可逆（需通过回滚撤回）。
        /// </summary>
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

        /// <summary>
        /// "停用"按钮：停用当前规则。
        /// 停用后引擎不再匹配此规则，同时失效缓存。
        /// </summary>
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

        /// <summary>
        /// "回滚"按钮：回滚当前规则到上一个已发布版本。
        /// 用于紧急修复错误发布的规则。
        /// </summary>
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

        // ================================================================
        // 控件事件处理
        // ================================================================

        /// <summary>
        /// 规则列表选中行变化事件。加载选中规则的完整详情（基本信息、条件、动作、版本历史等）。
        /// 避免重复加载：若选中的规则与当前已选中规则相同则跳过。
        /// </summary>
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

        /// <summary>公式下拉框选中变化事件，更新公式参数结构展示</summary>
        private void CboFormulaSelectedIndexChanged(object sender, EventArgs e)
        {
            FormulaDefResponse formula = _cboFormula.SelectedItem as FormulaDefResponse;
            _txtFormulaParams.Text = formula == null ? string.Empty : formula.ParamSchemaJson;
        }

        /// <summary>字典类型下拉框选中变化事件，加载对应类型的字典项</summary>
        private void CboDictTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedDict();
        }

        /// <summary>字典刷新按钮点击事件，重新加载当前选中类型的字典项</summary>
        private void DictRefreshClick(object sender, EventArgs e)
        {
            LoadSelectedDict();
        }

        // ================================================================
        // 数据加载方法
        // ================================================================

        /// <summary>
        /// 加载字典数据。从计价服务获取规则类别、状态、范围、公式等字典数据，
        /// 绑定到对应的下拉框。调用时机：窗口加载时。
        ///
        /// 使用 SafeData 包装：服务端返回 null 时降级为空列表，防止后续代码空引用。
        /// </summary>
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
            BindRollbackModeCombo(_cboRollbackMode);
            BindFormulaCombo();
            LoadDictTypes();
            _gridFormulas.DataSource = _formulas;
        }

        /// <summary>
        /// 加载字典类型列表。用于"字典"页签的类型选择下拉框。
        /// 失败时静默处理（清空下拉框），不影响工作台主功能。
        /// </summary>
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
                // 字典类型加载失败不影响主功能，静默处理
                _cboDictType.Items.Clear();
            }
        }

        /// <summary>加载当前选中字典类型的字典项列表</summary>
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

        /// <summary>
        /// 绑定字典数据到下拉框。
        /// includeAll 为 true 时在首行添加"全部"选项（搜索栏使用）。
        /// </summary>
        /// <param name="combo">目标下拉框</param>
        /// <param name="data">字典数据列表</param>
        /// <param name="includeAll">是否包含"全部"选项</param>
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

        /// <summary>
        /// 绑定回滚模式。该字段是资金安全策略，不依赖远端字典，避免字典加载失败时无法保存安全值。
        /// </summary>
        /// <param name="combo">目标下拉框</param>
        private static void BindRollbackModeCombo(ComboBox combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ComboItem("STOP_CHARGE", "暂停收费"));
            combo.Items.Add(new ComboItem("LEGACY_EQUIVALENT", "旧逻辑等价"));
            combo.Items.Add(new ComboItem("MANUAL_REVIEW", "人工复核"));
            combo.SelectedIndex = 0;
        }

        /// <summary>绑定公式数据到公式选择下拉框</summary>
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

        /// <summary>
        /// 执行规则搜索。根据搜索栏的筛选条件调用查询接口，
        /// 结果绑定到左侧规则列表网格。默认查询前 200 条。
        /// </summary>
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

        /// <summary>
        /// 加载规则完整详情。包括：
        /// 1. 规则头信息 -> 绑定到基本信息页签
        /// 2. 版本列表 -> 绑定到版本历史网格
        /// 3. 当前版本的条件和动作 -> 绑定到条件/动作网格
        /// 4. 发布历史和变更日志 -> 绑定到对应网格
        /// </summary>
        /// <param name="ruleId">规则 ID</param>
        private void LoadRuleDetail(long ruleId)
        {
            // ========== 第1阶段：加载规则头 ==========
            ApiResponse<RuleHeaderResponse> ruleResponse = _client.GetRule(ruleId);
            EnsureSuccess(ruleResponse);
            _selectedRule = ruleResponse.Data;
            BindBasicFields(_selectedRule);

            // ========== 第2阶段：加载版本列表 ==========
            LoadVersions(ruleId);

            // ========== 第3阶段：加载当前版本的条件和动作 ==========
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

            // ========== 第4阶段：加载发布历史和变更日志 ==========
            _gridPublishHistory.DataSource = SafeData(_client.GetPublishHistory(ruleId));
            _gridChangeLogs.DataSource = SafeData(_client.GetChangeLogs(ruleId));
        }

        /// <summary>加载指定规则的版本列表并绑定到版本网格</summary>
        /// <param name="ruleId">规则 ID</param>
        private void LoadVersions(long ruleId)
        {
            _versions = SafeData(_client.GetRuleVersions(ruleId));
            _gridVersions.DataSource = _versions;
        }

        /// <summary>
        /// 加载指定版本的条件列表。
        /// 将服务端返回的 RuleConditionResponse 转换为可编辑的 RuleConditionItemRequest，
        /// 绑定到条件配置网格（BindingList 支持 DataGridView 的增删改）。
        /// </summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionNo">版本号</param>
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

        /// <summary>
        /// 加载指定版本的动作列表。
        /// 将服务端返回的 RuleActionResponse 转换为可编辑的 RuleActionItemRequest。
        /// </summary>
        /// <param name="ruleId">规则 ID</param>
        /// <param name="versionNo">版本号</param>
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

        // ================================================================
        // 数据绑定方法
        // ================================================================

        /// <summary>
        /// 将规则头信息绑定到基本信息页签的各控件。
        /// 规则编码设为只读（不允许修改已有规则的编码）。
        /// </summary>
        /// <param name="rule">规则头信息</param>
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
            if (rule.Priority < _numPriority.Minimum)
            {
                _numPriority.Value = _numPriority.Minimum;
            }
            else if (rule.Priority > _numPriority.Maximum)
            {
                _numPriority.Value = _numPriority.Maximum;
            }
            else
            {
                _numPriority.Value = rule.Priority;
            }
            _chkHasEffectiveFrom.Checked = rule.EffectiveFrom.HasValue;
            _chkHasEffectiveTo.Checked = rule.EffectiveTo.HasValue;
            SetComboValue(
                _cboRollbackMode,
                string.IsNullOrEmpty(rule.RollbackMode) ? "STOP_CHARGE" : rule.RollbackMode);
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

        /// <summary>清空基本信息页签的所有字段（新建模式时调用）</summary>
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

            if (_cboRollbackMode.Items.Count > 0)
            {
                _cboRollbackMode.SelectedIndex = 0;
            }

            _chkHasEffectiveFrom.Checked = false;
            _chkHasEffectiveTo.Checked = false;
        }

        /// <summary>绑定条件列表到条件配置网格（使用 BindingList 支持增删改）</summary>
        private void BindConditions(List<RuleConditionItemRequest> rows)
        {
            _gridConditions.DataSource = new BindingList<RuleConditionItemRequest>(rows);
        }

        /// <summary>绑定动作列表到动作配置网格（使用 BindingList 支持增删改）</summary>
        private void BindActions(List<RuleActionItemRequest> rows)
        {
            _gridActions.DataSource = new BindingList<RuleActionItemRequest>(rows);
        }

        // ================================================================
        // 请求构建方法
        // ================================================================

        /// <summary>
        /// 从基本信息页签构建新建规则请求。
        /// 注意 EmptyToNull 调用：业务约束要求空值必须存为 NULL（NULL ≠ 0）。
        /// </summary>
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
                RollbackMode = GetComboValue(_cboRollbackMode),
                Remark = EmptyToNull(_txtRemark.Text),
                CreatedBy = _operatorId
            };
        }

        /// <summary>从基本信息页签构建更新规则请求（不含 RuleCode）</summary>
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
                RollbackMode = GetComboValue(_cboRollbackMode),
                Remark = EmptyToNull(_txtRemark.Text),
                UpdatedBy = _operatorId
            };
        }

        /// <summary>从条件配置网格构建条件保存请求（全量）</summary>
        private RuleConditionSaveRequest BuildConditionSaveRequest()
        {
            return new RuleConditionSaveRequest
            {
                Conditions = ReadRows<RuleConditionItemRequest>(_gridConditions)
            };
        }

        /// <summary>从动作配置网格构建动作保存请求（全量）</summary>
        private RuleActionSaveRequest BuildActionSaveRequest()
        {
            return new RuleActionSaveRequest
            {
                Actions = ReadRows<RuleActionItemRequest>(_gridActions)
            };
        }

        /// <summary>
        /// 从 DataGridView 读取所有数据行（排除新行占位符）。
        /// 通过 DataBoundItem 获取绑定的数据对象。
        /// </summary>
        /// <typeparam name="T">行数据类型</typeparam>
        /// <param name="grid">源网格</param>
        /// <returns>数据行列表</returns>
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

        // ================================================================
        // 辅助方法
        // ================================================================

        /// <summary>获取规则列表网格当前选中行的规则头数据</summary>
        /// <returns>选中的规则头，无选中时返回 null</returns>
        private RuleHeaderResponse GetCurrentRuleRow()
        {
            if (_gridRules.CurrentRow == null)
            {
                return null;
            }

            return _gridRules.CurrentRow.DataBoundItem as RuleHeaderResponse;
        }

        /// <summary>
        /// 获取可编辑版本号。用于保存条件/动作时确定目标版本。
        /// 只允许返回 DRAFT 版本的版本号；若无 DRAFT 则抛出异常。
        /// 已发布版本不可直接编辑或重复发布，必须先创建新的草稿版本。
        /// </summary>
        /// <returns>版本号</returns>
        private int GetEditableVersionNo()
        {
            if (_versions == null || _versions.Count == 0)
            {
                throw new InvalidOperationException("当前规则没有版本，请先新建草稿版本。");
            }

            // 遍历查找 DRAFT 版本（取最后一个 DRAFT，理论上同一规则只有一个）
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

            throw new InvalidOperationException("当前规则没有可编辑的 DRAFT 版本，请先新建草稿版本。");
        }

        /// <summary>
        /// 获取展示版本号。用于加载条件/动作时确定要展示的版本。
        /// 优先使用规则头的 CurrentVersion（已发布版本）；
        /// 若未发布则使用最新版本号。
        /// </summary>
        /// <returns>版本号，无版本时返回 0</returns>
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

        /// <summary>确保当前已选中规则，否则抛出异常</summary>
        private void EnsureRuleSelected()
        {
            if (_selectedRule == null)
            {
                throw new InvalidOperationException("请先选择规则。");
            }
        }

        /// <summary>校验无数据 API 响应是否成功，失败时抛出异常</summary>
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

        /// <summary>校验带数据 API 响应是否成功，失败时抛出异常</summary>
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

        /// <summary>
        /// 安全获取列表数据。校验响应成功后返回 Data，若 Data 为 null 则返回空列表。
        /// 防止后续 foreach 等操作因 null 引用而崩溃。
        /// </summary>
        private static List<T> SafeData<T>(ApiResponse<List<T>> response)
        {
            EnsureSuccess(response);
            return response.Data ?? new List<T>();
        }

        /// <summary>
        /// 获取下拉框当前选中项的 Value 值。
        /// 支持 ComboItem（带 Value/Text 的自定义项）和普通字符串两种情况。
        /// </summary>
        private static string GetComboValue(ComboBox combo)
        {
            ComboItem item = combo.SelectedItem as ComboItem;
            if (item != null)
            {
                return item.Value;
            }

            return combo.SelectedItem == null ? string.Empty : combo.SelectedItem.ToString();
        }

        /// <summary>
        /// 设置下拉框选中项。按 Value 值匹配（忽略大小写）。
        /// 未找到匹配项时不修改选中状态。
        /// </summary>
        /// <param name="combo">目标下拉框</param>
        /// <param name="value">要选中的 Value 值</param>
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

        /// <summary>
        /// 空值转 NULL。业务约束要求 NULL ≠ 0：NULL 表示"不校验"，0 表示"限制为零"。
        /// 前端空值必须存为 NULL，不能存为空字符串。
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>空值返回 null，非空返回 Trim 后的值</returns>
        private static string EmptyToNull(string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim().Length == 0 ? null : value.Trim();
        }

        /// <summary>安全字符串转换，null 转为空字符串</summary>
        private static string Safe(string value)
        {
            return value == null ? string.Empty : value;
        }

        /// <summary>显示错误消息框</summary>
        /// <param name="title">消息框标题</param>
        /// <param name="ex">异常对象</param>
        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 下拉框选项封装。将字典编码（Value）和字典名称（Text）绑定在一起，
        /// 下拉框展示 Text，提交时取 Value。
        /// ToString 返回 "名称 (编码)" 格式，仅用于非 ComboItem 场景的回退展示。
        /// </summary>
        private sealed class ComboItem
        {
            /// <summary>字典编码（提交到服务端的值）</summary>
            public readonly string Value;

            /// <summary>字典名称（展示给用户的文本）</summary>
            private readonly string _text;

            /// <summary>
            /// 构造下拉框选项。
            /// </summary>
            /// <param name="value">字典编码</param>
            /// <param name="text">字典名称</param>
            public ComboItem(string value, string text)
            {
                Value = value;
                _text = text;
            }

            /// <summary>
            /// 返回展示文本。空编码时只显示名称，非空时显示 "名称 (编码)" 格式。
            /// </summary>
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
