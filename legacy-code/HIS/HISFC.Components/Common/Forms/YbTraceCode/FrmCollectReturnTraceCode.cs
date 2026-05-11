using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Linq;
using System.Windows.Forms;
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using Neusoft.HISFC.Models.MedicalTraceCode;
using Neusoft.HISFC.BizLogic.Pharmacy;

namespace Neusoft.HISFC.Components.Common.Forms.YbTraceCode
{
    /// <summary>
    /// 追溯码采集窗体 - 门诊退费场景
    /// </summary>
    public partial class FrmCollectReturnTraceCode : Form
    {

        #region 字段

        /// <summary>
        /// 输入数据 - 用于初始化界面
        /// </summary>
        private List<PhaComApplyout> _applyItems;

        /// <summary>
        /// 扫码数据 - 用于存储追溯码采集结果
        /// </summary>
        private List<YbTraceCollectMain> _drugs;

        private int _selectedIndex = 0;

        // UI 增强控件
        private Panel _progressPanel;
        private Label _progressLabel;
        private Timer _scanFlashTimer;
        private int _scanFlashCount = 0;
        private Color _originalScanBoxColor;

        private TraceCodeDAL QueryService = new TraceCodeDAL();

        protected Dictionary<string, List<string>> ApplyTraceMap =
               new Dictionary<string, List<string>>();

        private PatientAndApplyInfo PatientAndApplyInfo = new PatientAndApplyInfo();

        private DateTime collectStartTime = DateTime.Now;

        public List<YbTraceCollectMain> YbTraceCollectMainList = new List<YbTraceCollectMain>();


        #endregion

        #region 构造函数

        public FrmCollectReturnTraceCode()
        {
            InitializeComponent();
            InitSpread();
            InitProgressBar();
            InitScanFlashAnimation();
            InitEnhancedStyles();
            //LoadTestData();

            Neusoft.FrameWork.WinForms.Classes.Function.EnableDrag(this.lblTitle, this);

            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this, 12);

            Neusoft.FrameWork.WinForms.Classes.Function.ApplyCustomBorder(this, Color.FromArgb(77, 182, 172), 2, 8);

            SetFpStyle();
        }

        private void FpSpread1_EditModeOn(object sender, EventArgs e)
        {
            var sheet = fpSpread1.ActiveSheet;
            int row = sheet.ActiveRowIndex;
            int column = sheet.ActiveColumnIndex;

            if (row < 0 || column < 0)
            {
                return;
            }

            if (column == 4 || column == 7)
            {
                fpSpread1.StopCellEditing();
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化 Spread 控件
        /// </summary>
        private void InitSpread()
        {
            var sheet = fpSpread1.ActiveSheet;
            fpSpread1.BackColor = Color.White;
            sheet.GrayAreaBackColor = Color.White;
            sheet.DefaultStyle.BackColor = Color.White;

            // 基础设置 - 12列布局
            // 0:序号 1:药品信息 
            // 2:"原发X盒" 3:"退药" 4:[数量] 5:"盒" 6:"重收X盒"
            // 7:"原发X粒" 8:[开关] 
            // 9:状态 10:原码 11:已采
            sheet.ColumnCount = 12;
            sheet.RowCount = 0;
            sheet.OperationMode = OperationMode.Normal;
            sheet.Protect = true;

            // 关键设置：允许按钮点击但仅在双击时进入编辑
            fpSpread1.EditModePermanent = false;

            // 列宽设置 - 增大宽度便于操作
            sheet.Columns[0].Width = 25;   // 序号
            sheet.Columns[1].Width = 360;  // 药品信息(加宽)
            sheet.Columns[2].Width = 75;   // "原发X盒"
            sheet.Columns[3].Width = 45;   // "退药"
            sheet.Columns[4].Width = 55;   // [数量输入框]
            sheet.Columns[5].Width = 100;   // "盒 重收X盒"
            sheet.Columns[6].Width = 80;   // "原发X粒"
            sheet.Columns[7].Width = 80;   // [开关]
            sheet.Columns[8].Width = 100;   // 状态
            sheet.Columns[9].Width = 100;   // 原码按钮
            sheet.Columns[10].Width = 100;  // 已采按钮
            sheet.Columns[11].Width = 1;   // 隐藏列

            // 所有列垂直居中
            for (int c = 0; c < 12; c++)
            {
                sheet.Columns[c].VerticalAlignment = CellVerticalAlignment.Center;
                sheet.Columns[c].HorizontalAlignment = CellHorizontalAlignment.Center;
            }

            // 药品信息列特殊设置：左对齐 + 允许换行
            sheet.Columns[1].HorizontalAlignment = CellHorizontalAlignment.Left;
            var drugInfoCellType = new TextCellType();
            drugInfoCellType.ReadOnly = true;
            drugInfoCellType.Multiline = true;
            sheet.Columns[1].CellType = drugInfoCellType;

            // 表头文字
            sheet.ColumnHeader.Cells[0, 0].Text = "序号";
            sheet.ColumnHeader.Cells[0, 1].Text = "药品信息";
            sheet.ColumnHeader.Cells[0, 2].Text = "";
            sheet.ColumnHeader.Cells[0, 3].Text = "";
            sheet.ColumnHeader.Cells[0, 4].Text = "";
            sheet.ColumnHeader.Cells[0, 5].Text = "";
            sheet.ColumnHeader.Cells[0, 6].Text = "";
            sheet.ColumnHeader.Cells[0, 7].Text = "";
            sheet.ColumnHeader.Cells[0, 8].Text = "状态";
            sheet.ColumnHeader.Cells[0, 9].Text = "发药原码";
            sheet.ColumnHeader.Cells[0, 10].Text = "当前采集";
            sheet.ColumnHeader.Cells[0, 11].Text = "";

            // 合并表头单元格
            sheet.ColumnHeader.Cells[0, 2].ColumnSpan = 4; // 包装退药区域
            sheet.ColumnHeader.Cells[0, 2].Text = "包装退药(需扫码)";
            sheet.ColumnHeader.Cells[0, 6].ColumnSpan = 2; // 拆零退药区域
            sheet.ColumnHeader.Cells[0, 6].Text = "拆零退药(无需扫码)";

            // 显示表头
            sheet.ColumnHeader.Visible = true;
            sheet.RowHeader.Visible = false;

            // 设置网格线
            sheet.HorizontalGridLine = new GridLine(GridLineType.Flat, Color.FromArgb(238, 238, 238));
            sheet.VerticalGridLine = new GridLine(GridLineType.None);

            // 隐藏第11列
            sheet.Columns[11].Visible = false;

            // 表头样式 - 增强视觉层次
            sheet.ColumnHeader.DefaultStyle.BackColor = Color.FromArgb(243, 244, 246); // 稍深的灰色
            sheet.ColumnHeader.DefaultStyle.ForeColor = Color.FromArgb(75, 85, 99);
            sheet.ColumnHeader.DefaultStyle.Font = new Font("微软雅黑", 11F, FontStyle.Bold);
            sheet.ColumnHeader.DefaultStyle.Border = new LineBorder(Color.FromArgb(229, 231, 235), 0, false, false, false, false);
            sheet.ColumnHeader.Rows[0].Height = 44; // 稍高一点
            for (int c = 0; c < sheet.ColumnCount; c++)
            {
                var top = new ComplexBorderSide(Color.FromArgb(209, 213, 219), 1);
                var bottom = new ComplexBorderSide(Color.FromArgb(209, 213, 219), 2); // 底部加粗
                var none = new ComplexBorderSide(Color.Transparent, 0);
                sheet.ColumnHeader.Cells[0, c].Border = new ComplexBorder(top, bottom, none, none);
            }

            // 默认行样式 - 增大字体
            sheet.DefaultStyle.Font = new Font("微软雅黑", 15F);
            sheet.DefaultStyle.ForeColor = Color.FromArgb(51, 51, 51);
            fpSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.Never;

            // === 使用自定义单元格类型，实现类似 HTML 的样式 ===

            // 只读文本单元格类型
            var textCellType = new TextCellType();
            textCellType.ReadOnly = true;

            // 序号列 - 使用自定义 IndexCellType
            sheet.Columns[0].CellType = new IndexCellType();
            sheet.Columns[0].Locked = true;

            // 药品信息列 - 使用自定义 DrugInfoCellType
            sheet.Columns[1].CellType = new DrugInfoCellType();
            sheet.Columns[1].Locked = true;

            // 原发X盒 - 只读
            sheet.Columns[2].CellType = textCellType;
            sheet.Columns[2].Locked = true;

            // "退药" - 只读
            sheet.Columns[3].CellType = textCellType;
            sheet.Columns[3].Locked = true;

            // 退药数量列 - 使用自定义 StyledNumberCellType
            var numCellType = new StyledNumberCellType();
            numCellType.DecimalPlaces = 0;
            numCellType.MinimumValue = 0;
            numCellType.MaximumValue = 999;
            numCellType.ShowSeparator = false;
            sheet.Columns[4].CellType = numCellType;
            sheet.Columns[4].Locked = true; // 禁止直接编辑，只能通过加减按钮调整

            // "盒 重收X盒" - 只读
            sheet.Columns[5].CellType = textCellType;
            sheet.Columns[5].Locked = true;

            // "原发X粒" - 只读
            sheet.Columns[6].CellType = textCellType;
            sheet.Columns[6].Locked = true;

            // 拆零开关列 - 使用自定义 ToggleSwitchCellType
            sheet.Columns[7].CellType = new ToggleSwitchCellType();
            sheet.Columns[7].Locked = true; // 通过 CellClick 事件手动切换，避免进入编辑状态导致样式丢失

            // 状态列 - 使用自定义 StatusTagCellType
            sheet.Columns[8].CellType = new StatusTagCellType();
            sheet.Columns[8].Locked = true;

            // 原码按钮 - 使用自定义 StyledButtonCellType
            var btnOrigCode = new StyledButtonCellType();
            btnOrigCode.Text = "原码";
            btnOrigCode.TwoState = false;
            sheet.Columns[9].CellType = btnOrigCode;
            sheet.Columns[9].Locked = false;

            // 已采按钮 - 使用自定义 StyledButtonCellType
            var btnScanned = new StyledButtonCellType();
            btnScanned.Text = "已采(0)";
            btnScanned.IsPrimary = true;
            btnScanned.TwoState = false;
            sheet.Columns[10].CellType = btnScanned;
            sheet.Columns[10].Locked = false;

            // 隐藏列 - 只读
            sheet.Columns[11].Locked = true;

            // 事件绑定 - ButtonClicked 是按钮点击的主要事件
            fpSpread1.ButtonClicked += FpSpread1_ButtonClicked;
            fpSpread1.SelectionChanged += FpSpread1_SelectionChanged;
            fpSpread1.Change += FpSpread1_Change;
            fpSpread1.CellClick += FpSpread1_CellClick;
            fpSpread1.EditModeOn += FpSpread1_EditModeOn;
        }

        /// <summary>
        /// 初始化进度条
        /// </summary>
        private void InitProgressBar()
        {
            // 创建进度条容器面板
            _progressPanel = new Panel();
            _progressPanel.Height = 6;
            _progressPanel.Dock = DockStyle.Top;
            _progressPanel.BackColor = Color.FromArgb(229, 231, 235);
            pnlFooter.Controls.Add(_progressPanel);
            _progressPanel.BringToFront();

            // 创建进度标签（覆盖在进度条上）
            _progressLabel = new Label();
            _progressLabel.Height = 6;
            _progressLabel.Left = 0;
            _progressLabel.Top = 0;
            _progressLabel.Width = 0;
            _progressLabel.BackColor = Color.FromArgb(16, 185, 129); // 绿色进度
            _progressPanel.Controls.Add(_progressLabel);
        }

        /// <summary>
        /// 初始化扫码闪烁动画
        /// </summary>
        private void InitScanFlashAnimation()
        {
            _originalScanBoxColor = txtScanCode.BackColor;

            _scanFlashTimer = new Timer();
            _scanFlashTimer.Interval = 80; // 闪烁间隔
            _scanFlashTimer.Tick += (s, e) =>
            {
                if (_scanFlashCount >= 4) // 闪烁2次（绿-白-绿-白）
                {
                    _scanFlashTimer.Stop();
                    txtScanCode.BackColor = _originalScanBoxColor;
                    _scanFlashCount = 0;
                    return;
                }

                if (_scanFlashCount % 2 == 0)
                {
                    txtScanCode.BackColor = Color.FromArgb(220, 252, 231); // 浅绿色
                }
                else
                {
                    txtScanCode.BackColor = _originalScanBoxColor;
                }
                _scanFlashCount++;
            };
        }

        /// <summary>
        /// 触发扫码成功闪烁动画
        /// </summary>
        private void TriggerScanFlash()
        {
            _scanFlashCount = 0;
            _scanFlashTimer.Start();
        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        private void UpdateProgressBar()
        {
            if (_drugs == null || _drugs.Count == 0 || _progressPanel == null) return;

            int total = 0;
            int completed = 0;

            foreach (var d in _drugs)
            {
                int required = (int)d.PactActualCollectQty;
                if (required > 0)
                {
                    total++;
                    if (d.PactTracCodgsList.Count >= required)
                    {
                        completed++;
                    }
                }
            }

            if (total > 0)
            {
                double percent = (double)completed / total;
                _progressLabel.Width = (int)(_progressPanel.Width * percent);

                // 全部完成时变成蓝色
                if (completed >= total)
                {
                    _progressLabel.BackColor = Color.FromArgb(59, 130, 246); // 蓝色
                }
                else
                {
                    _progressLabel.BackColor = Color.FromArgb(16, 185, 129); // 绿色
                }
            }
        }

        /// <summary>
        /// 初始化增强样式 - 最终UI美化
        /// </summary>
        private void InitEnhancedStyles()
        {
            // 扫码输入框样式增强
            txtScanCode.BorderStyle = BorderStyle.FixedSingle;

            // 所有标签样式已在 Designer.cs 中设置
            // lblStats: 左侧提示文字
            // lblPending: 橙色待采数
            // lblScanned: 绿色已采数
        }

        /// <summary>
        /// 右侧统计标签
        /// </summary>
        private Label _statsRightLabel;

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void LoadData(List<PhaComApplyout> phaComApplyoutList)
        {
            PatientAndApplyInfo = QueryService.GetMZPatientAndApplyInfo(phaComApplyoutList[0].ApplyNumber.ToString());

            this.lblPatientInfo.Text = PatientAndApplyInfo.Name + " | " + "门诊号: " + PatientAndApplyInfo.CardNo;

            InitializeWithApplyItems(phaComApplyoutList);
        }

        /// <summary>
        /// 加载测试数据
        /// </summary>
        private void LoadTestData()
        {
            // 测试数据 - 使用 PhaComApplyout 初始化
            var testItems = new List<PhaComApplyout>
            {
                new PhaComApplyout
                {
                    ApplyNumber = 1,
                    DrugCode = "D001",
                    TradeName = "艾托格列净片（捷诺妥）",
                    Specs = "5mg*14片/盒",
                    PackUnit = "盒",
                    MinUnit = "片",
                    Needcollectqty = 2,
                    NeedCollectSpiltQty = 0,
                    NeedCollectTraceCodeFlag = "1"
                },
                new PhaComApplyout
                {
                    ApplyNumber = 2,
                    DrugCode = "D002",
                    TradeName = "德谷胰岛素利拉鲁肆注",
                    Specs = "3ml:300单位/支",
                    PackUnit = "支",
                    MinUnit = "ml",
                    Needcollectqty = 1,
                    NeedCollectSpiltQty = 2,
                    NeedCollectTraceCodeFlag = "1"
                },
                new PhaComApplyout
                {
                    ApplyNumber = 3,
                    DrugCode = "D003",
                    TradeName = "阿莫西林胶囊",
                    Specs = "0.25g*24粒/盒",
                    PackUnit = "盒",
                    MinUnit = "粒",
                    Needcollectqty = 1,
                    NeedCollectSpiltQty = 5,
                    NeedCollectTraceCodeFlag = "1"
                },
                new PhaComApplyout
                {
                    ApplyNumber = 4,
                    DrugCode = "D004",
                    TradeName = "布洛芬缓释胶囊",
                    Specs = "0.3g*20粒/包",
                    PackUnit = "包",
                    MinUnit = "粒",
                    Needcollectqty = 0,
                    NeedCollectSpiltQty = 8,
                    NeedCollectTraceCodeFlag = "1"
                }
            };

            // 使用测试数据初始化
            InitializeWithApplyItems(testItems);
        }

        /// <summary>
        /// 使用 PhaComApplyout 列表初始化界面
        /// </summary>
        /// <param name="applyItems">申请数据列表</param>
        public void InitializeWithApplyItems(List<PhaComApplyout> applyItems)
        {
            _applyItems = applyItems ?? new List<PhaComApplyout>();
            _drugs = new List<YbTraceCollectMain>();

            // 将 PhaComApplyout 转换为 YbTraceCollectMain
            for (int i = 0; i < _applyItems.Count; i++)
            {
                var item = _applyItems[i];
                var drug = ConvertToYbTraceCollectMain(item, i);
                _drugs.Add(drug);
            }

            RenderData();
            UpdateStats();
            UpdateProgressBar();

            // 默认选中第一行
            if (_drugs.Count > 0)
            {
                _selectedIndex = 0;
                fpSpread1.ActiveSheet.SetActiveCell(0, 0);
                HighlightSelectedRow();
                ScrollToRow(0);
            }

            ApplyTraceMap = this.QueryService.GetApplyNumberTraceMap(_drugs.Select(a => a.ApplyNumber).ToList());

            txtScanCode.Focus();
        }

        /// <summary>
        /// 将 PhaComApplyout 转换为 YbTraceCollectMain
        /// </summary>
        private YbTraceCollectMain ConvertToYbTraceCollectMain(PhaComApplyout applyOut, int index)
        {
            var traceCollectMainInfo = new YbTraceCollectMain();
            traceCollectMainInfo.Id = Guid.NewGuid().ToString();
            traceCollectMainInfo.ApplyNumber = applyOut.ApplyNumber.ToString();

            traceCollectMainInfo.BusinessScenario = BusinessScenarioEnum.OutpatientReturnAudit;
            traceCollectMainInfo.CollectType = CollectTypeEnum.ReturnOfGoods;
            traceCollectMainInfo.SourceSystem = SourceSystemEnum.HIS;
            traceCollectMainInfo.BusinessType = BusinessTypeEnum.MZ;

            traceCollectMainInfo.SerialNo = applyOut.PatientId;
            traceCollectMainInfo.PatientName = "";
            traceCollectMainInfo.DrugCode = applyOut.DrugCode;
            traceCollectMainInfo.DrugName = applyOut.TradeName;
            var drugInfo = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(applyOut.DrugCode);
            traceCollectMainInfo.DrugSpecs = drugInfo.Specs;
            traceCollectMainInfo.DrugCustomCode = drugInfo.UserCode;
            traceCollectMainInfo.DrugPactQty = applyOut.PackQty.ToString();
            traceCollectMainInfo.DrugPactUnit = drugInfo.PackUnit;
            traceCollectMainInfo.DrugMinUnit = drugInfo.MinUnit;
            traceCollectMainInfo.DrugSplitUnit = drugInfo.MinUnit;
            traceCollectMainInfo.PharmacyCode = applyOut.DrugDeptCode;
            traceCollectMainInfo.PharmacyName = "";
            traceCollectMainInfo.DeptCode = applyOut.DeptCode;
            traceCollectMainInfo.DeptName = "";
            traceCollectMainInfo.MoOrderNo = applyOut.MoOrder;
            traceCollectMainInfo.ExecOrderNo = applyOut.ExecSqn;

            traceCollectMainInfo.CollectIp = Neusoft.FrameWork.WinForms.Classes.Function.GetLocalIP();

            if (this.QueryService.Operator != null)
            {
                traceCollectMainInfo.CreatedCode = this.QueryService.Operator.ID;
                traceCollectMainInfo.CreatedName = this.QueryService.Operator.Name;
                traceCollectMainInfo.CollectOperCode = traceCollectMainInfo.CreatedCode;
                traceCollectMainInfo.CollectOperName = traceCollectMainInfo.CreatedName;
            }

            if (Neusoft.FrameWork.Management.Connection.Hospital.ID == "CORE_HIS502")
            {
                traceCollectMainInfo.HospitalCode = "H44040200357";
                traceCollectMainInfo.HospitalName = "中山大学珠海校区卫生服务中心";
            }
            else
            {
                traceCollectMainInfo.HospitalCode = "H44040200001";
                traceCollectMainInfo.HospitalName = "中山大学附属第五医院";
            }

            //需要采集 且采集完成的数据才进行赋值采集退费数据
            if (applyOut.NeedCollectTraceCodeFlag == "1" && TraceCodeCollectionStatusEnum.IsCollectCompleted(applyOut.Tracecodecollectionstatus))
            {

                if (applyOut.Alreadycollectqty > 0)
                {
                    traceCollectMainInfo.IsHavePact = "1";
                    traceCollectMainInfo.PactNeedCollectQty = applyOut.Alreadycollectqty;
                    traceCollectMainInfo.PactActualCollectQty = applyOut.Alreadycollectqty;
                    traceCollectMainInfo.PactAppealCollectQty = applyOut.Appealcollectqty;
                    traceCollectMainInfo.PactUnCollectQty = 0;
                    traceCollectMainInfo.PactCollectStatus = "0";
                    traceCollectMainInfo.PactCollectMethod = "1";
                }
                else
                {
                    traceCollectMainInfo.IsHavePact = "0";
                    traceCollectMainInfo.PactNeedCollectQty = 0;
                    traceCollectMainInfo.PactActualCollectQty = 0;
                    traceCollectMainInfo.PactAppealCollectQty = 0;
                    traceCollectMainInfo.PactUnCollectQty = 0;
                    traceCollectMainInfo.PactCollectStatus = "2";
                    traceCollectMainInfo.PactCollectMethod = "1";
                }

                if (applyOut.AlreadyCollectSpiltQty > 0)
                {
                    traceCollectMainInfo.IsHaveSplit = "1";
                    traceCollectMainInfo.SplitNeedCollectQty = applyOut.AlreadyCollectSpiltQty;
                    traceCollectMainInfo.SplitActualCollectQty = applyOut.AlreadyCollectSpiltQty;
                    traceCollectMainInfo.SplitAppealCollectQty = applyOut.AppealCollectSpiltQty;
                    traceCollectMainInfo.SplitUnCollectQty = 0;
                    traceCollectMainInfo.SplitCollectStatus = "0";
                    traceCollectMainInfo.SplitCollectMethod = "1";
                }
                else
                {
                    traceCollectMainInfo.IsHaveSplit = "0";
                    traceCollectMainInfo.SplitNeedCollectQty = 0;
                    traceCollectMainInfo.SplitActualCollectQty = 0;
                    traceCollectMainInfo.SplitAppealCollectQty = 0;
                    traceCollectMainInfo.SplitUnCollectQty = 0;
                    traceCollectMainInfo.SplitCollectStatus = "2";
                    traceCollectMainInfo.SplitCollectMethod = "1";
                }


            }

            //非数据库属性
            traceCollectMainInfo.SortIndex = index;
            return traceCollectMainInfo;
        }

        /// <summary>
        /// 获取采集结果
        /// </summary>
        /// <returns>采集完成的追溯码数据</returns>
        public List<YbTraceCollectMain> GetCollectionResults()
        {
            return _drugs;
        }

        #endregion

        #region 数据渲染

        /// <summary>
        /// 渲染数据到 Spread
        /// </summary>
        private void RenderData()
        {
            var sheet = fpSpread1.ActiveSheet;
            sheet.RowCount = _drugs.Count;

            for (int i = 0; i < _drugs.Count; i++)
            {
                var d = _drugs[i];
                RenderRow(i, d);
            }
        }

        /// <summary>
        /// 单元格点击事件（用于处理自定义开关）
        /// </summary>
        private void FpSpread1_CellClick(object sender, CellClickEventArgs e)
        {
            if (e.Row >= 0 && e.Row < _drugs.Count)
            {
                var sheet = fpSpread1.ActiveSheet;
                var drug = _drugs[e.Row];
                int pactQty = (int)drug.PactNeedCollectQty;
                int splitQty = (int)drug.SplitNeedCollectQty;

                if (e.Column == 4 && pactQty > 0)
                {
                    Rectangle cellRect = fpSpread1.GetCellRectangle(0, 0, e.Row, 4);
                    if (!cellRect.IsEmpty)
                    {
                        var spinnerRect = GetSpinnerRect(cellRect);
                        if (spinnerRect.Contains(e.X, e.Y))
                        {
                            fpSpread1.StopCellEditing();
                            e.Cancel = true;

                            int current = 0;
                            object val = sheet.Cells[e.Row, 4].Value;
                            if (val != null)
                            {
                                int.TryParse(val.ToString(), out current);
                            }

                            bool isIncrement = e.Y < spinnerRect.Top + spinnerRect.Height / 2;
                            int next = isIncrement ? current + 1 : current - 1;
                            if (next < 0) next = 0;
                            if (next > pactQty) next = pactQty;

                            if (next != current)
                            {
                                sheet.Cells[e.Row, 4].Value = next;
                                drug.PactActualCollectQty = next;
                                drug.PactTracCodgsList.Clear();
                                RenderRow(e.Row, drug);
                                UpdateStats();
                                HighlightSelectedRow();
                            }
                            return;
                        }
                    }
                }
                else if (e.Column == 7 && splitQty > 0)
                {
                    fpSpread1.StopCellEditing();
                    bool current = false;
                    object val = sheet.Cells[e.Row, 7].Value;
                    if (val is bool)
                    {
                        current = (bool)val;
                    }
                    else if (val != null)
                    {
                        bool.TryParse(val.ToString(), out current);
                    }

                    bool next = !current;
                    sheet.Cells[e.Row, 7].Value = next;
                    drug.SplitActualCollectQty = next ? splitQty : 0;

                    RenderRow(e.Row, drug);
                    UpdateStats();
                    HighlightSelectedRow();
                }
            }
        }

        private Rectangle GetSpinnerRect(Rectangle cellRect)
        {
            int boxX = cellRect.X + (cellRect.Width - StyledNumberCellType.BoxWidth) / 2;
            int boxY = cellRect.Y + (cellRect.Height - StyledNumberCellType.BoxHeight) / 2;
            var boxRect = new Rectangle(boxX, boxY, StyledNumberCellType.BoxWidth, StyledNumberCellType.BoxHeight);
            return new Rectangle(
                boxRect.Right - StyledNumberCellType.SpinnerWidth,
                boxRect.Y,
                StyledNumberCellType.SpinnerWidth,
                boxRect.Height);
        }

        /// <summary>
        /// 渲染单行
        /// </summary>
        private void RenderRow(int rowIndex, YbTraceCollectMain drug)
        {

            var sheet = fpSpread1.ActiveSheet;

            sheet.Rows[rowIndex].Tag = drug;

            int pactQty = (int)drug.PactNeedCollectQty;
            int splitQty = (int)drug.SplitNeedCollectQty;
            int pactActual = (int)drug.PactActualCollectQty;  // 实际退药包装数
            int splitActual = (int)drug.SplitActualCollectQty; // 实际退药拆零数

            // 行高 - 增大便于操作
            sheet.Rows[rowIndex].Height = 65;

            // 0: 序号 - 使用自定义单元格类型
            var indexCell = new IndexCellType();
            sheet.Cells[rowIndex, 0].CellType = indexCell;
            sheet.Cells[rowIndex, 0].Value = (rowIndex + 1).ToString();

            // 1: 药品信息（名称 + 规格）- 换行显示
            sheet.Cells[rowIndex, 1].Value = drug.DrugName + "\n" + drug.DrugSpecs;
            sheet.Cells[rowIndex, 1].Font = new Font("微软雅黑", 15F);

            // === 包装退药区域 (列2-5) ===
            var fontNormal = new Font("微软雅黑", 10F);
            if (pactQty > 0)
            {
                // 2: "原发 X 盒"
                sheet.Cells[rowIndex, 2].Value = "原采 " + pactQty + drug.DrugPactUnit;
                sheet.Cells[rowIndex, 2].ForeColor = Color.FromArgb(102, 102, 102);
                sheet.Cells[rowIndex, 2].Font = fontNormal;

                // 3: "退药"
                sheet.Cells[rowIndex, 3].Value = "退药";
                sheet.Cells[rowIndex, 3].ForeColor = Color.FromArgb(102, 102, 102);
                sheet.Cells[rowIndex, 3].Font = fontNormal;

                // 4: [数量输入框] - 显示实际退药数量
                sheet.Cells[rowIndex, 4].Value = pactActual;
                sheet.Cells[rowIndex, 4].Locked = true;
                sheet.Cells[rowIndex, 4].Font = new Font("微软雅黑", 11F, FontStyle.Bold);

                // 5: "盒  重收 X 盒"
                int retake = Math.Max(0, pactQty - pactActual);
                sheet.Cells[rowIndex, 5].Value = drug.DrugPactUnit + "  重收 " + retake + drug.DrugPactUnit;
                sheet.Cells[rowIndex, 5].ForeColor = Color.FromArgb(102, 102, 102);
                sheet.Cells[rowIndex, 5].Font = fontNormal;
            }
            else
            {
                // 无包装 - 合并列2-5，居中显示
                sheet.AddSpanCell(rowIndex, 2, 1, 4); // 合并4列
                sheet.Cells[rowIndex, 2].Value = "无包装";
                sheet.Cells[rowIndex, 2].ForeColor = Color.FromArgb(180, 180, 180);
                sheet.Cells[rowIndex, 2].Font = new Font("微软雅黑", 11F);
                sheet.Cells[rowIndex, 2].HorizontalAlignment = CellHorizontalAlignment.Center;
                sheet.Cells[rowIndex, 2].Locked = true;
            }

            // === 拆零退药区域 (列6-7) ===
            if (splitQty > 0)
            {
                // 6: "原发 X 粒"
                sheet.Cells[rowIndex, 6].Value = "原采 " + splitQty + drug.DrugSplitUnit;
                sheet.Cells[rowIndex, 6].ForeColor = Color.FromArgb(102, 102, 102);
                sheet.Cells[rowIndex, 6].Font = fontNormal;

                // 7: 开关 - 根据实际退药拆零数判断是否开启
                sheet.Cells[rowIndex, 7].Value = splitActual > 0;
                sheet.Cells[rowIndex, 7].HorizontalAlignment = CellHorizontalAlignment.Center;
                sheet.Cells[rowIndex, 7].Locked = true;
            }
            else
            {
                // 无拆零 - 合并列6-7，居中显示
                sheet.AddSpanCell(rowIndex, 6, 1, 2); // 合并2列
                sheet.Cells[rowIndex, 6].Value = "无拆零";
                sheet.Cells[rowIndex, 6].ForeColor = Color.FromArgb(180, 180, 180);
                sheet.Cells[rowIndex, 6].Font = new Font("微软雅黑", 11F);
                sheet.Cells[rowIndex, 6].HorizontalAlignment = CellHorizontalAlignment.Center;
                sheet.Cells[rowIndex, 6].Locked = true;
            }

            // 8: 状态
            UpdateRowStatus(rowIndex, drug);

            // 10: 已采按钮 - 显示已采数量（使用自定义样式）
            var btnScanned = new StyledButtonCellType();
            btnScanned.Text = "已采(" + drug.PactTracCodgsList.Count + ")";
            btnScanned.IsPrimary = true;
            btnScanned.TwoState = false;
            sheet.Cells[rowIndex, 10].CellType = btnScanned;
        }

        /// <summary>
        /// 更新行状态显示
        /// </summary>
        private void UpdateRowStatus(int rowIndex, YbTraceCollectMain drug)
        {
            var sheet = fpSpread1.ActiveSheet;
            // 使用PactActualCollectQty作为需要采集的数量（用户可调整的退药数量）
            int required = (int)drug.PactActualCollectQty;
            int scanned = drug.PactTracCodgsList.Count;
            int splitActual = (int)drug.SplitActualCollectQty;

            string statusText;
            Color bgColor, textColor, rowBgColor;

            // 拆零自动完成（无需扫码）
            if (required == 0 && splitActual > 0)
            {
                statusText = "拆零自动";
                bgColor = Color.FromArgb(240, 255, 244);
                textColor = Color.FromArgb(22, 163, 74);
                rowBgColor = Color.White;
            }
            else if (required == 0)
            {
                // 无需采集
                statusText = "无需采集";
                bgColor = Color.FromArgb(245, 245, 245);
                textColor = Color.FromArgb(153, 153, 153);
                rowBgColor = Color.White;
            }
            else if (scanned >= required)
            {
                // 已完成
                statusText = "✓ 已完成";
                bgColor = Color.FromArgb(240, 255, 244);
                textColor = Color.FromArgb(22, 163, 74);
                rowBgColor = Color.White;
            }
            else
            {
                // 待采集
                statusText = string.Format("待采 {0}/{1}{2}", scanned, required, drug.DrugPactUnit);
                bgColor = Color.FromArgb(255, 242, 232);
                textColor = Color.FromArgb(234, 88, 12);
                rowBgColor = Color.White;
            }

            // 状态单元格 - 现在在第8列
            sheet.Cells[rowIndex, 8].Value = statusText;
            sheet.Cells[rowIndex, 8].BackColor = bgColor;
            sheet.Cells[rowIndex, 8].ForeColor = textColor;
            sheet.Cells[rowIndex, 8].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheet.Cells[rowIndex, 8].Font = new Font("微软雅黑", 10F, FontStyle.Bold);

            // 序号列(第0列) - 设置 IndexCellType 的 StatusColor
            Color seqColor;
            if (required == 0 && splitActual > 0)
            {
                // 拆零自动 - 绿色
                seqColor = Color.FromArgb(22, 163, 74);
            }
            else if (required == 0)
            {
                // 无需采集 - 灰色
                seqColor = Color.FromArgb(180, 180, 180);
            }
            else if (scanned >= required)
            {
                // 已完成 - 绿色
                seqColor = Color.FromArgb(22, 163, 74);
            }
            else
            {
                // 待采集 - 橙色
                seqColor = Color.FromArgb(234, 88, 12);
            }
            // 更新 IndexCellType 的 StatusColor
            var indexCellType = sheet.Cells[rowIndex, 0].CellType as IndexCellType;
            if (indexCellType != null)
            {
                indexCellType.StatusColor = seqColor;
            }

            // 整行背景色(除了序号列和状态列)
            for (int c = 1; c < 12; c++)
            {
                if (c != 8) // 状态列保持自己的背景色
                {
                    sheet.Cells[rowIndex, c].BackColor = rowBgColor;
                }
            }
        }

        /// <summary>
        /// 高亮选中行
        /// </summary>
        private void HighlightSelectedRow()
        {
            var sheet = fpSpread1.ActiveSheet;

            for (int i = 0; i < sheet.RowCount; i++)
            {
                bool isSelected = (i == _selectedIndex);

                // 选中行药品名称高亮（蓝色字体 + 加粗）
                if (isSelected)
                {
                    sheet.Cells[i, 1].ForeColor = Color.FromArgb(37, 99, 235);
                    sheet.Cells[i, 1].Font = new Font("微软雅黑", 10F, FontStyle.Bold);
                }
                else
                {
                    sheet.Cells[i, 1].ForeColor = Color.FromArgb(51, 51, 51);
                    sheet.Cells[i, 1].Font = new Font("微软雅黑", 10F);
                }
                // 注意：序号列(第0列)的颜色由 UpdateRowStatus 根据采集状态设置，不在这里修改
            }
        }

        /// <summary>
        /// 更新统计信息
        /// </summary>
        private void UpdateStats()
        {
            int total = _drugs.Count;
            int completed = 0;
            int pending = 0;
            int scannedCount = 0;

            foreach (var d in _drugs)
            {
                int required = (int)d.PactActualCollectQty; // 使用实际退药数量
                scannedCount += d.PactTracCodgsList.Count;
                if (required == 0)
                    continue;
                if (d.PactTracCodgsList.Count >= required)
                    completed++;
                else
                    pending++;
            }

            // 更新 Designer.cs 中定义的标签
            lblPending.Text = string.Format("待采 {0}", pending);
            lblScanned.Text = string.Format("| 已采 {0}", scannedCount);
        }

        /// <summary>
        /// 滚动到指定行
        /// </summary>
        private void ScrollToRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < fpSpread1.ActiveSheet.RowCount)
            {
                // 设置顶部行使目标行可见
                int visibleRows = (int)(fpSpread1.Height / fpSpread1.ActiveSheet.Rows[0].Height);
                int topRow = Math.Max(0, rowIndex - visibleRows / 2);
                fpSpread1.ActiveSheet.SetActiveCell(rowIndex, 0);
            }
        }

        #endregion

        #region 扫码处理

        /// <summary>
        /// 扫码输入框回车事件
        /// </summary>
        private void TxtScanCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcessScanCode();
            }
        }

        /// <summary>
        /// 处理扫码 - 退药场景：按当前界面所有行添加追溯码
        /// </summary>
        private void ProcessScanCode()
        {
            var scanCode = this.txtScanCode.Text.Trim();
            this.txtScanCode.Clear();
            this.txtScanCode.Focus();

            var applyNumber = string.Empty;

            foreach (KeyValuePair<string, List<string>> kv in ApplyTraceMap)
            {
                List<string> traceCodes = kv.Value;
                if (traceCodes == null)
                {
                    continue;
                }

                foreach (string tracecode in traceCodes)
                {
                    if (string.Equals(tracecode, scanCode, StringComparison.Ordinal))
                    {
                        applyNumber = kv.Key;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(applyNumber))
                {
                    break;
                }
            }


            if (string.IsNullOrEmpty(applyNumber))
            {
                ShowToast("该追溯码不在收费时采集的码值集合中！", false);
                ShakeInput();
                return;
            }

            var collectMainInfo = new YbTraceCollectMain();

            var rowIndex = -1;
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                var info = this.fpSpread1_Sheet1.Rows[i].Tag as YbTraceCollectMain;
                if (info.ApplyNumber == applyNumber)
                {
                    collectMainInfo = info;
                    rowIndex = i;
                    break;
                }
            }

            if (collectMainInfo == null || collectMainInfo.ApplyNumber != applyNumber)
            {
                ShowToast("该追溯码未找到对应Tag信息实体！", false);
                ShakeInput();
                return;
            }

            ChangeCollectMainInfo(scanCode, collectStartTime, collectMainInfo);

            if (collectMainInfo.IsHavePact == YesNoEnum.Yes)
            {
                // 扫码成功 - 触发闪烁动画
                TriggerScanFlash();

                UpdateRowStatus(rowIndex, collectMainInfo);
                RenderRow(rowIndex, collectMainInfo);
                UpdateStats();
                UpdateProgressBar();

                // 选中该行并滚动到可见
                _selectedIndex = rowIndex;
                fpSpread1.ActiveSheet.SetActiveCell(rowIndex, 0);
                HighlightSelectedRow();
                ScrollToRow(rowIndex);



            }


            return;
            var code = txtScanCode.Text.Trim();
            if (string.IsNullOrEmpty(code))
                return;

            // 退药场景：没有原码匹配，直接添加到当前选中的药品
            // 如果当前选中的药品已采集完成，则查找下一个待采集的药品
            int matchIndex = -1;

            // 优先检查当前选中行
            if (_selectedIndex >= 0 && _selectedIndex < _drugs.Count)
            {
                var currentDrug = _drugs[_selectedIndex];
                int required = (int)currentDrug.PactActualCollectQty; // 使用实际退药数量
                if (required > 0 && currentDrug.PactTracCodgsList.Count < required)
                {
                    matchIndex = _selectedIndex;
                }
            }

            // 如果当前行已完成，查找第一个未完成的药品
            if (matchIndex < 0)
            {
                for (int i = 0; i < _drugs.Count; i++)
                {
                    var drug = _drugs[i];
                    int required = (int)drug.PactActualCollectQty; // 使用实际退药数量
                    if (required == 0)
                        continue;
                    if (drug.PactTracCodgsList.Count >= required)
                        continue;
                    matchIndex = i;
                    break;
                }
            }

            if (matchIndex >= 0)
            {
                // 扫码成功
                var drug = _drugs[matchIndex];
                int required = (int)drug.PactActualCollectQty; // 使用实际退药数量

                // 检查是否重复扫码
                if (drug.PactTracCodgsList.Contains(code))
                {
                    // 重复扫码 - 显示错误提示，报动输入框
                    ShowToast("⚠ 该追溯码已扫描过！", false);
                    ShakeInput();
                }
                else
                {
                    // 扫码成功 - 触发闪烁动画
                    TriggerScanFlash();

                    drug.PactTracCodgsList.Add(code);
                    UpdateRowStatus(matchIndex, drug);
                    RenderRow(matchIndex, drug);
                    UpdateStats();
                    UpdateProgressBar();

                    // 选中该行并滚动到可见
                    _selectedIndex = matchIndex;
                    fpSpread1.ActiveSheet.SetActiveCell(matchIndex, 0);
                    HighlightSelectedRow();
                    ScrollToRow(matchIndex);

                    // 检查是否刚好完成当前药品的采集
                    if (drug.PactTracCodgsList.Count >= required)
                    {
                        // 当前药品采集完成，显示简短提示
                        ShowToast(string.Format("✓ {0} 采集完成", drug.DrugName), true);
                    }

                }
            }
            else
            {
                // 所有药品都已采集完成
                ShowToast("所有药品追溯码已采集完成！", true);
            }

            txtScanCode.Clear();
            txtScanCode.Focus();
        }

        #endregion


        /// <summary>
        /// 每次扫码后修改采集信息实体
        /// </summary>
        /// <param name="scanCode"></param>
        /// <param name="collectStartTime"></param>
        /// <param name="info"></param>
        private void ChangeCollectMainInfo(
            string scanCode,
            DateTime collectStartTime,
            YbTraceCollectMain info)
        {
            if (string.IsNullOrEmpty(scanCode))
            {
                return;
            }

            if (info.PactTracCodgsList == null)
            {
                info.PactTracCodgsList = new List<string>();
            }

            if (!TraceCodeCollectionStatusEnum.IsValid(info.PactCollectStatus))
            {
                ShowToast("药品[" + info.DrugName + "]当前采集状态非法!", false);
                return;
            }

            if (TraceCodeCollectionStatusEnum.IsCollectCompleted(info.PactCollectStatus))
            {
                ShowToast("药品[" + info.DrugName + "]" + TraceCodeCollectionStatusEnum.GetDescription(info.PactCollectStatus) + ",请勿重复采集!", false);
                return;
            }

            if (info.PactTracCodgsList.Contains(scanCode))
            {
                ShowToast("该码已采集,请勿重复扫码!", false);
                return;
            }

            //TODO 提前拷贝一下对象,防止出现异常情况 导致值没回滚

            info.PactTracCodgsList.Add(scanCode);
            if (string.IsNullOrEmpty(info.PactTracCodgs))
            {
                info.PactTracCodgs = scanCode;
            }
            else
            {
                info.PactTracCodgs = info.PactTracCodgs + ";" + scanCode;
            }

            info.PactUnCollectQty = info.PactActualCollectQty - info.PactAppealCollectQty;

            info.PactCollectCompleteRate =
info.PactNeedCollectQty == 0
? "0%"
: Math.Round((info.PactActualCollectQty / info.PactNeedCollectQty) * 100, 2).ToString("0.##") + "%";

            info.PactCollectStatus = TraceCodeCollectionStatusEnum.GetStatusForQty(info.PactActualCollectQty, info.PactTracCodgsList.Count, info.PactAppealCollectQty);

            info.IdentifiyCode = scanCode.Substring(0, 7);

            info.CardNo = PatientAndApplyInfo.CardNo;
            info.PatientNo = PatientAndApplyInfo.PatientNo;
            info.PharmacyCode = PatientAndApplyInfo.DrugDeptCode;
            info.PharmacyName = PatientAndApplyInfo.DrugDeptName;
            info.DeptCode = PatientAndApplyInfo.DeptCode;
            info.DeptName = PatientAndApplyInfo.DeptName;

            if (TraceCodeCollectionStatusEnum.IsCollectCompleted(info.PactCollectStatus))
            {
                info.CollectStartTime = collectStartTime;
                info.CollectEndTime = DateTime.Now;
                info.CollectDurationMs =
    Convert.ToDecimal((info.CollectEndTime - info.CollectStartTime).TotalMilliseconds);
            }


        }

        #region 事件处理

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        private void FpSpread1_ButtonClicked(object sender, EditorNotifyEventArgs e)
        {
            if (e.Row >= 0 && e.Row < _drugs.Count)
            {
                var drug = _drugs[e.Row];

                if (e.Column == 9) // 原码按钮
                {
                    ShowOrigCodesDialog(drug);
                }
                else if (e.Column == 10) // 已采按钮
                {
                    ShowScannedCodesDialog(drug);
                }
            }
        }

        /// <summary>
        /// 选择变化事件
        /// </summary>
        private void FpSpread1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int row = fpSpread1.ActiveSheet.ActiveRowIndex;
            if (row >= 0 && row < _drugs.Count)
            {
                _selectedIndex = row;
                HighlightSelectedRow();
            }
        }

        /// <summary>
        /// 单元格值变化事件（退药数量修改）
        /// </summary>
        private void FpSpread1_Change(object sender, ChangeEventArgs e)
        {
            if (e.Row >= 0 && e.Row < _drugs.Count)
            {

                var sheet = fpSpread1.ActiveSheet;
                var drug = sheet.Rows[e.Row].Tag as YbTraceCollectMain;
                int pactQty = (int)drug.PactNeedCollectQty;
                int splitQty = (int)drug.SplitNeedCollectQty;

                if (e.Column == 4 && pactQty > 0) // 退药数量
                {
                    object val = sheet.Cells[e.Row, 4].Value;
                    int newVal = 0;
                    if (val != null)
                    {
                        int.TryParse(val.ToString(), out newVal);
                    }
                    if (newVal < 0) newVal = 0;
                    if (newVal > pactQty) newVal = pactQty;

                    drug.PactActualCollectQty = newVal;
                    drug.PactTracCodgsList.Clear(); // 清空已扫码

                    // 重新渲染该行
                    RenderRow(e.Row, drug);
                    UpdateStats();
                    HighlightSelectedRow();
                }
                else if (e.Column == 7 && splitQty > 0) // 拆零开关
                {
                    object val = sheet.Cells[e.Row, 7].Value;
                    bool isChecked = val != null && (bool)val;
                    drug.SplitActualCollectQty = isChecked ? splitQty : 0;

                    // 重新渲染该行
                    RenderRow(e.Row, drug);
                    UpdateStats();
                    HighlightSelectedRow();
                }
            }
        }

        /// <summary>
        /// 键盘事件（上下键导航、F8审核）
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up)
            {
                if (_selectedIndex > 0)
                {
                    _selectedIndex--;
                    fpSpread1.ActiveSheet.SetActiveCell(_selectedIndex, 0);
                    HighlightSelectedRow();
                }
                return true;
            }
            else if (keyData == Keys.Down)
            {
                if (_selectedIndex < _drugs.Count - 1)
                {
                    _selectedIndex++;
                    fpSpread1.ActiveSheet.SetActiveCell(_selectedIndex, 0);
                    HighlightSelectedRow();
                }
                return true;
            }
            else if (keyData == Keys.F8)
            {
                BtnSubmit_Click(null, null);
                return true;
            }
            else if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// 审核通过按钮
        /// </summary>
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            var list = GetYbTraceCollectMainListForFPData();
            if (!IsValid(list))
            {
                return;
            }
            // 更新包装相关字段
            foreach (var item in list)
            {
                item.PactAppealCollectQty = 0;
                item.PactNeedCollectQty = item.PactActualCollectQty;
                item.PactUnCollectQty = 0;
            }
            // 遍历表格行，更新拆零相关字段
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                // 从行的 Tag 获取数据对象
                var info = this.fpSpread1_Sheet1.Rows[i].Tag as YbTraceCollectMain;
                if (info == null)
                {
                    continue;
                }
                // 获取原发拆零数量，判断是否有拆零
                int splitQty = (int)info.SplitNeedCollectQty;
                if (splitQty <= 0)
                {
                    // 无拆零数据，跳过
                    continue;
                }
                // 拆零是否全退（安全获取开关值）
                bool splitAllReturn = false;
                var cellValue = this.fpSpread1_Sheet1.Cells[i, 7].Value;
                if (cellValue is bool)
                {
                    splitAllReturn = (bool)cellValue;
                }
                else if (cellValue != null)
                {
                    bool.TryParse(cellValue.ToString(), out splitAllReturn);
                }
                // 在 list 中找到对应的项进行同步更新
                var listItem = list.FirstOrDefault(x => x.ApplyNumber == info.ApplyNumber);
                if (listItem == null)
                {
                    continue;
                }
                // 根据是否全退更新拆零相关字段
                if (splitAllReturn)
                {
                    // 全退：保持原有拆零数量
                    listItem.SplitActualCollectQty = info.SplitNeedCollectQty;
                    listItem.SplitAppealCollectQty = 0;
                    listItem.SplitNeedCollectQty = info.SplitNeedCollectQty;
                    listItem.SplitUnCollectQty = 0;
                    listItem.SplitCollectStatus = "3"; //采集成功
                }
                else
                {
                    // 不退：清空拆零相关字段
                    listItem.SplitActualCollectQty = 0;
                    listItem.SplitAppealCollectQty = 0;
                    listItem.SplitNeedCollectQty = 0;
                    listItem.SplitUnCollectQty = 0;
                    listItem.SplitTracCodgs = "";
                    listItem.SplitTracCodgsList = new List<string>();
                    listItem.SplitCollectStatus = "2"; // 无需采集
                }
            }
            this.YbTraceCollectMainList = list;
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private bool IsValid(List<YbTraceCollectMain> list)
        {
            if (!list.Any())
            {
                MessageBox.Show("数据验证失败:采集集合为空!");
                return false;
            }

            foreach (var item in list)
            {
                if (item.IsHavePact == YesNoEnum.Yes)
                {
                    if (!TraceCodeCollectionStatusEnum.IsCollectCompleted(item.PactCollectStatus))
                    {
                        MessageBox.Show(item.DrugName + "包装采集未完成!");
                        return false;
                    }

                    if (item.PactTracCodgsList == null || item.PactTracCodgsList.Count != item.PactActualCollectQty)
                    {
                        MessageBox.Show(item.DrugName + "包装实采数量与追溯码数量不一致!");
                        return false;
                    }

                }

                if (item.IsHaveSplit == YesNoEnum.Yes && 1 == 2)//暂时弃用 发药保存时才进行分配
                {
                    if (!TraceCodeCollectionStatusEnum.IsCollectCompleted(item.SplitCollectStatus))
                    {
                        MessageBox.Show(item.DrugName + "拆零采集未完成!");
                        return false;

                    }

                    if (item.SplitActualCollectQty + item.SplitAppealCollectQty != item.SplitNeedCollectQty)
                    {
                        MessageBox.Show(item.DrugName + "数量采集不正确!");
                        return false;
                    }

                    if (item.SplitTracCodgsList == null || item.SplitTracCodgsList.Count != item.SplitActualCollectQty)
                    {
                        MessageBox.Show(item.DrugName + "拆零实采数量与追溯码数量不一致!");
                        return false;
                    }

                }



            }

            return true;
        }

        /// <summary>
        /// 获取FP绑定的数据源集合信息
        /// </summary>
        /// <returns></returns>
        private List<YbTraceCollectMain> GetYbTraceCollectMainListForFPData()
        {
            if (this.fpSpread1_Sheet1.RowCount <= 0)
            {
                return null;
            }

            var list = new List<YbTraceCollectMain>();

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                var info = this.fpSpread1_Sheet1.Rows[i].Tag as YbTraceCollectMain;
                list.Add(info);
            }

            return list;

        }

        /// <summary>
        /// 取消按钮
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (HasUnfinishedWork())
            {
                var result = MessageBox.Show("还有未完成的采集，确定要取消吗？", "提示",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;
            }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 重置按钮
        /// </summary>
        private void BtnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("确定要重置所有已扫描的追溯码吗？", "确认重置",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                foreach (var d in _drugs)
                {
                    d.PactTracCodgsList.Clear();
                }
                RenderData();
                UpdateStats();
                HighlightSelectedRow();
                ShowToast("已重置所有扫描记录", true);
                txtScanCode.Focus();
            }
        }

        /// <summary>
        /// 窗体点击事件（保持扫码框焦点）
        /// </summary>
        private void FrmCollectReturnTraceCode_Click(object sender, EventArgs e)
        {
            txtScanCode.Focus();
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 显示原码对话框 - 美化版本（支持复制）
        /// </summary>
        private void ShowOrigCodesDialog(YbTraceCollectMain drug)
        {
            if (!ApplyTraceMap.ContainsKey(drug.ApplyNumber))
            {
                MessageBox.Show("暂未找到发药时采集情况!");
                return;
            }
            var pactTraceList = ApplyTraceMap[drug.ApplyNumber];

            // 创建自定义对话框
            var dialog = new Form
            {
                Text = "追溯码详情",
                Size = new Size(520, 420),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White,
                Font = new Font("微软雅黑", 9F)
            };

            // 标题面板
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(16, 12, 16, 12)
            };

            var lblDrugName = new Label
            {
                Text = drug.DrugName,
                Font = new Font("微软雅黑", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(48, 49, 51),
                AutoSize = true,
                Location = new Point(16, 12)
            };

            var lblSpecs = new Label
            {
                Text = "规格：" + drug.DrugSpecs,
                Font = new Font("微软雅黑", 9F),
                ForeColor = Color.FromArgb(144, 147, 153),
                AutoSize = true,
                Location = new Point(16, 40)
            };

            headerPanel.Controls.Add(lblDrugName);
            headerPanel.Controls.Add(lblSpecs);

            // 追溯码列表面板（可滚动）
            var listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(16, 8, 16, 8)
            };

            // 创建追溯码列表
            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0)
            };

            for (int i = 0; i < pactTraceList.Count; i++)
            {
                string traceCode = pactTraceList[i].ToString();

                // 单个追溯码卡片
                var codePanel = new Panel
                {
                    Width = 460,
                    Height = 50,
                    Margin = new Padding(0, 0, 0, 8),
                    BackColor = Color.FromArgb(250, 250, 250),
                    Padding = new Padding(12, 8, 12, 8)
                };

                // 序号标签
                var lblIndex = new Label
                {
                    Text = (i + 1).ToString(),
                    Font = new Font("微软雅黑", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(64, 158, 255),
                    Size = new Size(24, 24),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(12, 13)
                };

                // 追溯码标签
                var lblCode = new Label
                {
                    Text = traceCode,
                    Font = new Font("Consolas", 10F),
                    ForeColor = Color.FromArgb(48, 49, 51),
                    AutoSize = true,
                    Location = new Point(48, 15)
                };

                // 单个复制按钮
                var btnCopy = new Button
                {
                    Text = "复制",
                    Size = new Size(50, 26),
                    Location = new Point(398, 12),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(64, 158, 255),
                    Font = new Font("微软雅黑", 8F),
                    Cursor = Cursors.Hand,
                    Tag = traceCode  // 存储追溯码值
                };
                btnCopy.FlatAppearance.BorderColor = Color.FromArgb(64, 158, 255);
                btnCopy.FlatAppearance.BorderSize = 1;
                btnCopy.Click += (s, ev) =>
                {
                    var code = ((Button)s).Tag.ToString();
                    Clipboard.SetText(code);
                    ((Button)s).Text = "已复制";
                    ((Button)s).BackColor = Color.FromArgb(103, 194, 58);
                    ((Button)s).ForeColor = Color.White;
                    ((Button)s).FlatAppearance.BorderColor = Color.FromArgb(103, 194, 58);

                    // 1秒后恢复
                    var timer = new System.Windows.Forms.Timer { Interval = 1000 };
                    timer.Tick += (ts, te) =>
                    {
                        ((Button)s).Text = "复制";
                        ((Button)s).BackColor = Color.White;
                        ((Button)s).ForeColor = Color.FromArgb(64, 158, 255);
                        ((Button)s).FlatAppearance.BorderColor = Color.FromArgb(64, 158, 255);
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                };

                codePanel.Controls.Add(lblIndex);
                codePanel.Controls.Add(lblCode);
                codePanel.Controls.Add(btnCopy);
                flowPanel.Controls.Add(codePanel);
            }

            listPanel.Controls.Add(flowPanel);

            // 底部按钮面板
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(16, 12, 16, 12)
            };

            // 统计信息
            var lblCount = new Label
            {
                Text = string.Format("共 {0} 个追溯码", pactTraceList.Count),
                Font = new Font("微软雅黑", 9F),
                ForeColor = Color.FromArgb(144, 147, 153),
                AutoSize = true,
                Location = new Point(16, 20)
            };

            // 全部复制按钮
            var btnCopyAll = new Button
            {
                Text = "全部复制",
                Size = new Size(80, 32),
                Location = new Point(dialog.ClientSize.Width - 186, 14),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 158, 255),
                Font = new Font("微软雅黑", 9F),
                Cursor = Cursors.Hand
            };
            btnCopyAll.FlatAppearance.BorderColor = Color.FromArgb(64, 158, 255);
            btnCopyAll.FlatAppearance.BorderSize = 1;
            btnCopyAll.Click += (s, ev) =>
            {
                // 将所有追溯码用换行符连接
                string allCodes = string.Join("\r\n", pactTraceList.ToArray());

                Clipboard.SetText(allCodes);
                btnCopyAll.Text = "已复制";
                btnCopyAll.BackColor = Color.FromArgb(103, 194, 58);
                btnCopyAll.ForeColor = Color.White;
                btnCopyAll.FlatAppearance.BorderColor = Color.FromArgb(103, 194, 58);

                // 1秒后恢复
                var timer = new System.Windows.Forms.Timer { Interval = 1000 };
                timer.Tick += (ts, te) =>
                {
                    btnCopyAll.Text = "全部复制";
                    btnCopyAll.BackColor = Color.White;
                    btnCopyAll.ForeColor = Color.FromArgb(64, 158, 255);
                    btnCopyAll.FlatAppearance.BorderColor = Color.FromArgb(64, 158, 255);
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            };

            // 关闭按钮
            var btnClose = new Button
            {
                Text = "关闭",
                Size = new Size(80, 32),
                Location = new Point(dialog.ClientSize.Width - 96, 14),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(64, 158, 255),
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 9F),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, ev) => dialog.Close();

            footerPanel.Controls.Add(lblCount);
            footerPanel.Controls.Add(btnCopyAll);
            footerPanel.Controls.Add(btnClose);

            // 组装对话框
            dialog.Controls.Add(listPanel);
            dialog.Controls.Add(headerPanel);
            dialog.Controls.Add(footerPanel);

            // 显示对话框
            dialog.ShowDialog(this);
        }

        /// <summary>
        /// 显示已采集追溯码对话框 - 美化版本（支持移除）
        /// </summary>
        private void ShowScannedCodesDialog(YbTraceCollectMain drug)
        {
            if (drug.PactTracCodgsList.Count == 0)
            {
                MessageBox.Show(string.Format("【{0}】\n\n暂无已采集的追溯码", drug.DrugName),
                    "已采集追溯码", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 创建自定义对话框
            var dialog = new Form
            {
                Text = "已采集追溯码",
                Size = new Size(520, 420),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White,
                Font = new Font("微软雅黑", 9F)
            };

            // 标题面板
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(16, 12, 16, 12)
            };

            var lblDrugName = new Label
            {
                Text = drug.DrugName,
                Font = new Font("微软雅黑", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(48, 49, 51),
                AutoSize = true,
                Location = new Point(16, 12)
            };

            var lblSpecs = new Label
            {
                Text = "规格：" + drug.DrugSpecs,
                Font = new Font("微软雅黑", 9F),
                ForeColor = Color.FromArgb(144, 147, 153),
                AutoSize = true,
                Location = new Point(16, 40)
            };

            headerPanel.Controls.Add(lblDrugName);
            headerPanel.Controls.Add(lblSpecs);

            // 追溯码列表面板（可滚动）
            var listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(16, 8, 16, 8)
            };

            // 创建追溯码列表容器
            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0),
                Tag = drug // 存储药品对象引用
            };

            // 底部按钮面板
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(16, 12, 16, 12)
            };

            // 统计信息标签
            var lblCount = new Label
            {
                Text = string.Format("共 {0} 个追溯码", drug.PactTracCodgsList.Count),
                Font = new Font("微软雅黑", 9F),
                ForeColor = Color.FromArgb(144, 147, 153),
                AutoSize = true,
                Location = new Point(16, 20),
                Tag = drug // 存储药品对象引用
            };

            // 刷新列表的方法
            System.Action refreshList = null;
            refreshList = () =>
            {
                flowPanel.Controls.Clear();

                for (int i = 0; i < drug.PactTracCodgsList.Count; i++)
                {
                    string traceCode = drug.PactTracCodgsList[i].ToString();
                    int currentIndex = i; // 捕获当前索引

                    // 单个追溯码卡片
                    var codePanel = new Panel
                    {
                        Width = 460,
                        Height = 50,
                        Margin = new Padding(0, 0, 0, 8),
                        BackColor = Color.FromArgb(250, 250, 250),
                        Padding = new Padding(12, 8, 12, 8)
                    };

                    // 序号标签
                    var lblIndex = new Label
                    {
                        Text = (i + 1).ToString(),
                        Font = new Font("微软雅黑", 9F, FontStyle.Bold),
                        ForeColor = Color.White,
                        BackColor = Color.FromArgb(103, 194, 58), // 绿色表示已采集
                        Size = new Size(24, 24),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Location = new Point(12, 13)
                    };

                    // 追溯码标签
                    var lblCode = new Label
                    {
                        Text = traceCode,
                        Font = new Font("Consolas", 10F),
                        ForeColor = Color.FromArgb(48, 49, 51),
                        AutoSize = true,
                        Location = new Point(48, 15)
                    };

                    // 单个移除按钮
                    var btnRemove = new Button
                    {
                        Text = "移除",
                        Size = new Size(50, 26),
                        Location = new Point(398, 12),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(245, 108, 108),
                        Font = new Font("微软雅黑", 8F),
                        Cursor = Cursors.Hand,
                        Tag = traceCode // 存储追溯码值
                    };
                    btnRemove.FlatAppearance.BorderColor = Color.FromArgb(245, 108, 108);
                    btnRemove.FlatAppearance.BorderSize = 1;
                    btnRemove.Click += (s, ev) =>
                    {
                        var codeToRemove = ((Button)s).Tag.ToString();
                        if (MessageBox.Show(string.Format("确定要移除追溯码：\n{0}？", codeToRemove),
                            "确认移除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            drug.PactTracCodgsList.Remove(codeToRemove);
                            drug.PactTracCodgs = string.Join(",", drug.PactTracCodgsList.ToArray());

                            // 更新采集状态：如果还有未采集的，状态改为待采集
                            int required = (int)drug.PactActualCollectQty;
                            if (drug.PactTracCodgsList.Count < required)
                            {
                                drug.PactCollectStatus = "0"; // 待采集
                            }

                            // 更新统计
                            lblCount.Text = string.Format("共 {0} 个追溯码", drug.PactTracCodgsList.Count);

                            // 刷新列表
                            refreshList();

                            // 如果没有追溯码了，关闭对话框
                            if (drug.PactTracCodgsList.Count == 0)
                            {
                                dialog.Close();
                            }
                        }
                    };

                    codePanel.Controls.Add(lblIndex);
                    codePanel.Controls.Add(lblCode);
                    codePanel.Controls.Add(btnRemove);
                    flowPanel.Controls.Add(codePanel);
                }
            };

            // 初始化列表
            refreshList();

            listPanel.Controls.Add(flowPanel);

            // 全部移除按钮
            var btnRemoveAll = new Button
            {
                Text = "全部移除",
                Size = new Size(80, 32),
                Location = new Point(dialog.ClientSize.Width - 186, 14),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(245, 108, 108),
                Font = new Font("微软雅黑", 9F),
                Cursor = Cursors.Hand
            };
            btnRemoveAll.FlatAppearance.BorderColor = Color.FromArgb(245, 108, 108);
            btnRemoveAll.FlatAppearance.BorderSize = 1;
            btnRemoveAll.Click += (s, ev) =>
            {
                if (MessageBox.Show(string.Format("确定要移除全部 {0} 个追溯码？\n此操作不可撤销！", drug.PactTracCodgsList.Count),
                    "确认全部移除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    drug.PactTracCodgsList.Clear();
                    drug.PactTracCodgs = "";

                    // 更新采集状态为待采集
                    int required = (int)drug.PactActualCollectQty;
                    if (required > 0)
                    {
                        drug.PactCollectStatus = "0"; // 待采集
                    }

                    dialog.Close();
                }
            };

            // 关闭按钮
            var btnClose = new Button
            {
                Text = "关闭",
                Size = new Size(80, 32),
                Location = new Point(dialog.ClientSize.Width - 96, 14),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(64, 158, 255),
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 9F),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, ev) => dialog.Close();

            footerPanel.Controls.Add(lblCount);
            footerPanel.Controls.Add(btnRemoveAll);
            footerPanel.Controls.Add(btnClose);

            // 组装对话框
            dialog.Controls.Add(listPanel);
            dialog.Controls.Add(headerPanel);
            dialog.Controls.Add(footerPanel);

            // 对话框关闭时更新界面
            dialog.FormClosed += (s, ev) =>
            {
                // 找到对应行并更新
                for (int i = 0; i < fpSpread1.ActiveSheet.RowCount; i++)
                {
                    var rowDrug = fpSpread1.ActiveSheet.Rows[i].Tag as YbTraceCollectMain;
                    if (rowDrug != null && rowDrug.ApplyNumber == drug.ApplyNumber)
                    {
                        RenderRow(i, drug);
                        break;
                    }
                }
                UpdateStats();
                UpdateProgressBar();
            };

            // 显示对话框
            dialog.ShowDialog(this);
        }

        /// <summary>
        /// 显示 Toast 提示
        /// </summary>
        private void ShowToast(string message, bool isSuccess)
        {
            lblToast.Text = message;
            lblToast.BackColor = isSuccess ? Color.FromArgb(240, 255, 244) : Color.FromArgb(255, 242, 242);
            lblToast.ForeColor = isSuccess ? Color.FromArgb(22, 163, 74) : Color.FromArgb(220, 38, 38);
            pnlToast.Visible = true;

            // 3秒后自动隐藏
            timerToast.Stop();
            timerToast.Start();
        }

        /// <summary>
        /// Toast 定时器
        /// </summary>
        private void TimerToast_Tick(object sender, EventArgs e)
        {
            pnlToast.Visible = false;
            timerToast.Stop();
        }

        /// <summary>
        /// 播放提示音
        /// </summary>
        private void PlayBeep(bool isSuccess)
        {
            try
            {
                if (isSuccess)
                    SystemSounds.Asterisk.Play();
                else
                    SystemSounds.Hand.Play();
            }
            catch
            {
                // 忽略声音播放错误
            }
        }

        /// <summary>
        /// 输入框抖动效果
        /// </summary>
        private void ShakeInput()
        {
            Point originalLocation = txtScanCode.Location;
            int shakeAmount = 5;

            for (int i = 0; i < 3; i++)
            {
                txtScanCode.Location = new Point(originalLocation.X + shakeAmount, originalLocation.Y);
                txtScanCode.Refresh();
                System.Threading.Thread.Sleep(50);

                txtScanCode.Location = new Point(originalLocation.X - shakeAmount, originalLocation.Y);
                txtScanCode.Refresh();
                System.Threading.Thread.Sleep(50);
            }

            txtScanCode.Location = originalLocation;
        }

        /// <summary>
        /// 是否有未完成的工作
        /// </summary>
        private bool HasUnfinishedWork()
        {
            foreach (var d in _drugs)
            {
                if (d.PactTracCodgsList.Count > 0)
                    return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 设置FP表格样式
        /// </summary>
        private void SetFpStyle()
        {
            // 基本设置
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.BorderStyle = BorderStyle.None;

            // Sheet设置
            this.fpSpread1_Sheet1.GrayAreaBackColor = Color.White;
            this.fpSpread1_Sheet1.RowHeader.Visible = false;

            // 表头样式
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = ColorTranslator.FromHtml("#F7F8FA");
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.ForeColor = ColorTranslator.FromHtml("#606266");
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Font = new Font("Microsoft YaHei", 9f, FontStyle.Regular);
            this.fpSpread1_Sheet1.ColumnHeader.Rows[0].Height = 40;

            // 去掉列头网格线
            this.fpSpread1_Sheet1.ColumnHeader.HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);
            this.fpSpread1_Sheet1.ColumnHeader.VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);

            // 默认单元格样式
            this.fpSpread1_Sheet1.DefaultStyle.Font = new Font("Microsoft YaHei", 9f);
            this.fpSpread1_Sheet1.DefaultStyle.ForeColor = ColorTranslator.FromHtml("#303133");
            this.fpSpread1_Sheet1.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

            // 行高
            this.fpSpread1_Sheet1.Rows.Default.Height = 36;


            // 列对齐方式
            this.fpSpread1_Sheet1.Columns[0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns[1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns[3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns[4].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns[5].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;

            // 隐藏数据区网格线
            this.fpSpread1_Sheet1.HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);
            this.fpSpread1_Sheet1.VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btn_AutoAssign_Click(object sender, EventArgs e)
        {
            MessageBox.Show("当前界面不允许使用该功能!");
            return;
        }

        /// <summary>
        /// 全部不退 - 将所有药品的退药数量设为0
        /// </summary>
        private void btn_AllNoReturn_Click(object sender, EventArgs e)
        {
            if (this.fpSpread1_Sheet1.RowCount == 0)
            {
                ShowToast("无数据！", false);
                return;
            }

            if (MessageBox.Show("确定要将所有药品设为不退吗？\n此操作会清空所有已采集的追溯码！",
                "确认全部不退", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                var drug = this.fpSpread1_Sheet1.Rows[i].Tag as YbTraceCollectMain;
                if (drug == null)
                {
                    continue;
                }

                // 1. 清空包装退药数量和追溯码
                drug.PactActualCollectQty = 0;
                drug.PactTracCodgsList = new List<string>();
                drug.PactTracCodgs = "";
                drug.PactCollectStatus = "2"; // 无需采集

                // 2. 清空拆零退药数量和追溯码
                drug.SplitActualCollectQty = 0;
                drug.SplitTracCodgsList = new List<string>();
                drug.SplitTracCodgs = "";
                drug.SplitCollectStatus = "2"; // 无需采集

                // 3. 更新拆零开关状态为关闭
                int splitQty = (int)drug.SplitNeedCollectQty;
                if (splitQty > 0)
                {
                    this.fpSpread1_Sheet1.Cells[i, 7].Value = false; // 开关设为关闭
                }

                // 4. 刷新行显示
                RenderRow(i, drug);
            }

            // 5. 更新统计和进度
            UpdateStats();
            UpdateProgressBar();
            HighlightSelectedRow();

            ShowToast("已将所有药品设为不退", true);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


    }

    // RefundDrugItem 类已移除，现在使用 YbTraceCollectMain
}
