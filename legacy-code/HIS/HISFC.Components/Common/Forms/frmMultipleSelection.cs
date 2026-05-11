using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using FarPoint.Win.Spread.Model;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Components.Common.Forms
{
    /// <summary>
    /// 多选弹出框窗体
    /// 功能：左侧待选项目列表，右侧已选项目列表，支持搜索、全选、反选、清空等操作
    /// </summary>
    public partial class frmMultipleSelection : Form
    {
        #region 配置项

        private int _minSelectCount = 0;
        private int _maxSelectCount = int.MaxValue;

        /// <summary>
        /// 最少选择数量，默认0（不限制）
        /// </summary>
        public int MinSelectCount
        {
            get { return _minSelectCount; }
            set { _minSelectCount = value; }
        }

        /// <summary>
        /// 最多选择数量，默认不限制（int.MaxValue）
        /// </summary>
        public int MaxSelectCount
        {
            get { return _maxSelectCount; }
            set { _maxSelectCount = value; }
        }

        #endregion

        #region 颜色常量

        private readonly Color ColorPrimary = Color.FromArgb(24, 144, 255);
        private readonly Color ColorPrimaryHover = Color.FromArgb(9, 109, 217);
        private readonly Color ColorPrimaryLight = Color.FromArgb(230, 244, 255);
        private readonly Color ColorSuccess = Color.FromArgb(82, 196, 26);
        private readonly Color ColorSuccessLight = Color.FromArgb(237, 248, 237);
        private readonly Color ColorWarning = Color.FromArgb(255, 251, 235);
        private readonly Color ColorWarningText = Color.FromArgb(153, 102, 0);
        private readonly Color ColorDanger = Color.FromArgb(220, 53, 69);
        private readonly Color ColorDangerLight = Color.FromArgb(255, 240, 240);
        private readonly Color ColorBorder = Color.FromArgb(217, 217, 217);
        private readonly Color ColorBorderFocus = Color.FromArgb(24, 144, 255);
        private readonly Color ColorTextPrimary = Color.FromArgb(51, 51, 51);
        private readonly Color ColorTextSecondary = Color.FromArgb(102, 102, 102);
        private readonly Color ColorTextHint = Color.FromArgb(153, 153, 153);
        private readonly Color ColorRowAlt = Color.FromArgb(250, 251, 252);
        private readonly Color ColorRowHover = Color.FromArgb(240, 248, 255);
        private readonly Color ColorShadow = Color.FromArgb(200, 200, 200);

        #endregion

        #region 数据相关

        private List<NeuObject> allItems;
        private List<NeuObject> selectedItems;
        private List<NeuObject> filteredItems;

        private List<NeuObject> _selectedResult;
        /// <summary>
        /// 已选择的结果列表
        /// </summary>
        public List<NeuObject> SelectedResult
        {
            get { return _selectedResult; }
            private set { _selectedResult = value; }
        }

        private bool _isConfirmed = false;
        /// <summary>
        /// 是否确认选择
        /// </summary>
        public bool IsConfirmed
        {
            get { return _isConfirmed; }
            private set { _isConfirmed = value; }
        }

        private string searchPlaceholder = "搜索名称或编码...";
        private int lastHoveredRow = -1;

        #endregion

        #region 窗体拖动

        private bool isDragging = false;
        private Point dragStartPoint;

        #endregion

        #region 构造函数

        public frmMultipleSelection()
        {
            allItems = new List<NeuObject>();
            selectedItems = new List<NeuObject>();
            filteredItems = new List<NeuObject>();
            _selectedResult = new List<NeuObject>();

            InitializeComponent();
            InitializeForm();
            InitializeEvents();
            InitializeSpread();
            InitializeToolTips();
        }

        #endregion

        #region 初始化

        private void InitializeForm()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            txtSearch.ForeColor = ColorTextHint;

            lblLeftCount.Paint += new PaintEventHandler(Badge_Paint);
            lblRightCount.Paint += new PaintEventHandler(Badge_Paint);
        }

        private void InitializeEvents()
        {
            pnlTitleBar.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            pnlTitleBar.MouseMove += new MouseEventHandler(TitleBar_MouseMove);
            pnlTitleBar.MouseUp += new MouseEventHandler(TitleBar_MouseUp);
            lblTitle.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            lblTitle.MouseMove += new MouseEventHandler(TitleBar_MouseMove);
            lblTitle.MouseUp += new MouseEventHandler(TitleBar_MouseUp);

            btnClose.Click += new EventHandler(BtnClose_Click);
            btnSelectAll.Click += new EventHandler(BtnSelectAll_Click);
            btnInvert.Click += new EventHandler(BtnInvert_Click);
            btnClear.Click += new EventHandler(BtnClear_Click);
            btnReset.Click += new EventHandler(BtnReset_Click);
            btnConfirm.Click += new EventHandler(BtnConfirm_Click);

            btnSelectAll.MouseEnter += new EventHandler(LinkButton_MouseEnter);
            btnSelectAll.MouseLeave += new EventHandler(LinkButton_MouseLeave);
            btnInvert.MouseEnter += new EventHandler(LinkButton_MouseEnter);
            btnInvert.MouseLeave += new EventHandler(LinkButton_MouseLeave);
            btnClear.MouseEnter += new EventHandler(ClearButton_MouseEnter);
            btnClear.MouseLeave += new EventHandler(ClearButton_MouseLeave);

            txtSearch.TextChanged += new EventHandler(TxtSearch_TextChanged);
            txtSearch.GotFocus += new EventHandler(TxtSearch_GotFocus);
            txtSearch.LostFocus += new EventHandler(TxtSearch_LostFocus);

            fpSpreadLeft.CellClick += new CellClickEventHandler(FpSpreadLeft_CellClick);
            fpSpreadRight.CellClick += new CellClickEventHandler(FpSpreadRight_CellClick);

            fpSpreadLeft.LeaveCell += new LeaveCellEventHandler(FpSpreadLeft_LeaveCell);
            fpSpreadLeft.EnterCell += new EnterCellEventHandler(FpSpreadLeft_EnterCell);
        }

        private void InitializeSpread()
        {
            // 左侧表格
            sheetLeft.Columns[0].Label = " ";
            sheetLeft.Columns[0].Width = 40;
            sheetLeft.Columns[0].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheetLeft.Columns[0].VerticalAlignment = CellVerticalAlignment.Center;

            sheetLeft.Columns[1].Label = "项目名称";
            sheetLeft.Columns[1].Width = 280;
            sheetLeft.Columns[1].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheetLeft.Columns[1].VerticalAlignment = CellVerticalAlignment.Center;

            sheetLeft.Columns[2].Label = "项目编码";
            sheetLeft.Columns[2].Width = 130;
            sheetLeft.Columns[2].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheetLeft.Columns[2].VerticalAlignment = CellVerticalAlignment.Center;

            CheckBoxCellType checkBox = new CheckBoxCellType();
            checkBox.ThreeState = false;
            sheetLeft.Columns[0].CellType = checkBox;

            // 右侧表格
            sheetRight.Columns[0].Label = "序号";
            sheetRight.Columns[0].Width = 50;
            sheetRight.Columns[0].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheetRight.Columns[0].VerticalAlignment = CellVerticalAlignment.Center;

            sheetRight.Columns[1].Label = "项目名称";
            sheetRight.Columns[1].Width = 180;
            sheetRight.Columns[1].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheetRight.Columns[1].VerticalAlignment = CellVerticalAlignment.Center;

            sheetRight.Columns[2].Label = "操作";
            sheetRight.Columns[2].Width = 70;
            sheetRight.Columns[2].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheetRight.Columns[2].VerticalAlignment = CellVerticalAlignment.Center;

            ApplySpreadStyle(fpSpreadLeft, sheetLeft);
            ApplySpreadStyle(fpSpreadRight, sheetRight);
        }

        private void ApplySpreadStyle(FpSpread spread, SheetView sheet)
        {
            spread.BorderStyle = System.Windows.Forms.BorderStyle.None;
            sheet.GrayAreaBackColor = Color.White;

            sheet.SelectionBackColor = ColorPrimaryLight;
            sheet.SelectionPolicy = SelectionPolicy.Single;

            sheet.AlternatingRows[0].BackColor = Color.White;
            sheet.AlternatingRows[1].BackColor = ColorRowAlt;

            sheet.HorizontalGridLine = new GridLine(GridLineType.Flat, Color.FromArgb(240, 240, 240));
            sheet.VerticalGridLine = new GridLine(GridLineType.None);

            sheet.ColumnHeader.DefaultStyle.HorizontalAlignment = CellHorizontalAlignment.Center;
            sheet.ColumnHeader.DefaultStyle.VerticalAlignment = CellVerticalAlignment.Center;
        }

        private void InitializeToolTips()
        {
            toolTip1.SetToolTip(btnSelectAll, "选择当前列表中的所有项目");
            toolTip1.SetToolTip(btnInvert, "反转当前列表的选择状态");
            toolTip1.SetToolTip(btnClear, "清空所有已选项目");
            toolTip1.SetToolTip(btnReset, "重置选择和搜索条件");
            toolTip1.SetToolTip(btnConfirm, "确认当前选择");
            toolTip1.SetToolTip(btnClose, "关闭窗口");
            toolTip1.SetToolTip(txtSearch, "输入关键字搜索项目名称或编码");
        }

        #endregion

        #region 徽章绘制

        private void Badge_Paint(object sender, PaintEventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl == null) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, lbl.Width, lbl.Height);
            int radius = Math.Min(lbl.Width, lbl.Height) / 2;

            using (GraphicsPath path = CreateRoundedRectangle(rect, radius))
            {
                using (SolidBrush brush = new SolidBrush(lbl.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                using (SolidBrush brush = new SolidBrush(lbl.ForeColor))
                {
                    e.Graphics.DrawString(lbl.Text, lbl.Font, brush, rect, sf);
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置数据源
        /// </summary>
        /// <param name="items">NeuObject列表</param>
        public void SetDataSource(List<NeuObject> items)
        {
            if (items == null)
            {
                allItems = new List<NeuObject>();
            }
            else
            {
                allItems = items;
            }
            filteredItems = new List<NeuObject>(allItems);
            RefreshLeftSpread();
            UpdateStatus();
        }

        /// <summary>
        /// 设置已选择的项目
        /// </summary>
        /// <param name="selectedIds">已选择的ID列表</param>
        public void SetSelectedItems(List<string> selectedIds)
        {
            selectedItems.Clear();
            if (selectedIds != null)
            {
                foreach (string id in selectedIds)
                {
                    NeuObject item = allItems.Find(delegate(NeuObject x) { return x.ID == id; });
                    if (item != null)
                    {
                        selectedItems.Add(item);
                    }
                }
            }
            RefreshBothSpreads();
            UpdateStatus();
        }

        /// <summary>
        /// 设置选择数量限制
        /// </summary>
        /// <param name="min">最少选择数量</param>
        /// <param name="max">最多选择数量，传0或负数表示不限制</param>
        public void SetSelectionLimit(int min, int max)
        {
            _minSelectCount = min;
            _maxSelectCount = max <= 0 ? int.MaxValue : max;
            UpdateConfigBar();
            UpdateStatus();
        }

        #endregion

        #region 刷新列表

        private void RefreshLeftSpread()
        {
            // 保存当前滚动位置
            int topRow = fpSpreadLeft.GetViewportTopRow(0);

            sheetLeft.RowCount = 0;
            sheetLeft.RowCount = filteredItems.Count;

            bool hasMaxLimit = _maxSelectCount < int.MaxValue;

            for (int i = 0; i < filteredItems.Count; i++)
            {
                NeuObject item = filteredItems[i];
                bool isSelected = selectedItems.Exists(delegate(NeuObject x) { return x.ID == item.ID; });
                bool isDisabled = hasMaxLimit && !isSelected && selectedItems.Count >= _maxSelectCount;

                sheetLeft.Cells[i, 0].Value = isSelected;
                sheetLeft.Cells[i, 1].Value = item.Name;
                sheetLeft.Cells[i, 2].Value = item.ID;

                if (isDisabled)
                {
                    sheetLeft.Rows[i].BackColor = Color.FromArgb(252, 252, 252);
                    sheetLeft.Rows[i].ForeColor = Color.FromArgb(180, 180, 180);
                }
                else if (isSelected)
                {
                    sheetLeft.Rows[i].BackColor = ColorPrimaryLight;
                    sheetLeft.Rows[i].ForeColor = ColorPrimary;
                }
                else
                {
                    sheetLeft.Rows[i].BackColor = (i % 2 == 0) ? Color.White : ColorRowAlt;
                    sheetLeft.Rows[i].ForeColor = ColorTextPrimary;
                }

                sheetLeft.Rows[i].Tag = item.ID;
            }

            lblLeftCount.Text = filteredItems.Count.ToString();

            // 恢复滚动位置
            if (topRow >= 0 && topRow < filteredItems.Count)
            {
                fpSpreadLeft.SetViewportTopRow(0, topRow);
            }
        }

        private void RefreshRightSpread()
        {
            sheetRight.RowCount = 0;
            sheetRight.RowCount = selectedItems.Count;

            for (int i = 0; i < selectedItems.Count; i++)
            {
                NeuObject item = selectedItems[i];

                sheetRight.Cells[i, 0].Value = (i + 1).ToString();
                sheetRight.Cells[i, 0].ForeColor = ColorPrimary;
                sheetRight.Cells[i, 0].Font = new Font("微软雅黑", 9F, FontStyle.Bold);

                sheetRight.Cells[i, 1].Value = item.Name;
                sheetRight.Cells[i, 1].ForeColor = ColorTextPrimary;

                sheetRight.Cells[i, 2].Value = "移除";
                sheetRight.Cells[i, 2].ForeColor = ColorDanger;
                sheetRight.Cells[i, 2].Font = new Font("微软雅黑", 9F);

                sheetRight.Rows[i].BackColor = (i % 2 == 0) ? Color.White : ColorRowAlt;
                sheetRight.Rows[i].Tag = item.ID;
            }

            lblRightCount.Text = selectedItems.Count.ToString();

            if (selectedItems.Count > 0)
            {
                lblRightCount.BackColor = ColorSuccess;
                lblEmptyHint.Visible = false;
                fpSpreadRight.Visible = true;
            }
            else
            {
                lblRightCount.BackColor = Color.FromArgb(180, 180, 180);
                lblEmptyHint.Visible = true;
                fpSpreadRight.Visible = true;
            }

            lblRightCount.Invalidate();
        }

        private void RefreshBothSpreads()
        {
            RefreshLeftSpread();
            RefreshRightSpread();
        }

        #endregion

        #region 状态更新

        private void UpdateConfigBar()
        {
            bool hasMaxLimit = _maxSelectCount < int.MaxValue;
            if (_minSelectCount > 0 && hasMaxLimit)
            {
                lblConfigInfo.Text = string.Format("选择限制：最少 {0} 项，最多 {1} 项", _minSelectCount, _maxSelectCount);
                pnlConfigBar.Visible = true;
            }
            else if (_minSelectCount > 0)
            {
                lblConfigInfo.Text = string.Format("选择限制：最少 {0} 项", _minSelectCount);
                pnlConfigBar.Visible = true;
            }
            else if (hasMaxLimit)
            {
                lblConfigInfo.Text = string.Format("选择限制：最多 {0} 项", _maxSelectCount);
                pnlConfigBar.Visible = true;
            }
            else
            {
                pnlConfigBar.Visible = false;
            }
        }

        private void UpdateStatus()
        {
            int count = selectedItems.Count;
            int total = allItems.Count;
            bool hasMaxLimit = _maxSelectCount < int.MaxValue;

            lblSummary.Text = string.Format("已选择 {0} / {1} 项", count, total);

            if (count < _minSelectCount)
            {
                pnlConfigBar.BackColor = ColorDangerLight;
                lblConfigIcon.ForeColor = ColorDanger;
                lblConfigInfo.ForeColor = ColorDanger;
                lblLimitHint.Text = string.Format("（还需选择 {0} 项）", _minSelectCount - count);
                lblLimitHint.ForeColor = ColorDanger;
                btnConfirm.Enabled = false;
                btnConfirm.BackColor = ColorBorder;
            }
            else if (hasMaxLimit && count >= _maxSelectCount)
            {
                pnlConfigBar.BackColor = ColorWarning;
                lblConfigIcon.ForeColor = ColorWarningText;
                lblConfigInfo.ForeColor = ColorWarningText;
                lblLimitHint.Text = "（已达上限）";
                lblLimitHint.ForeColor = ColorWarningText;
                btnConfirm.Enabled = true;
                btnConfirm.BackColor = Color.FromArgb(79, 157, 166);
            }
            else
            {
                pnlConfigBar.BackColor = ColorSuccessLight;
                lblConfigIcon.ForeColor = ColorSuccess;
                lblConfigInfo.ForeColor = ColorSuccess;
                lblLimitHint.Text = "";
                btnConfirm.Enabled = true;
                btnConfirm.BackColor = Color.FromArgb(79, 157, 166);
            }

            // 如果没有限制配置，隐藏配置栏
            if (_minSelectCount <= 0 && !hasMaxLimit)
            {
                pnlConfigBar.Visible = false;
            }

            lblLeftCount.Invalidate();
        }

        #endregion

        #region 事件处理

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = e.Location;
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = PointToScreen(e.Location);
                this.Location = new Point(currentPos.X - dragStartPoint.X, currentPos.Y - dragStartPoint.Y);
            }
        }

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void LinkButton_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.ForeColor = Color.White;
            }
        }

        private void LinkButton_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.ForeColor = ColorPrimary;
            }
        }

        private void ClearButton_MouseEnter(object sender, EventArgs e)
        {
            btnClear.ForeColor = Color.White;
        }

        private void ClearButton_MouseLeave(object sender, EventArgs e)
        {
            btnClear.ForeColor = ColorDanger;
        }

        private void FpSpreadLeft_EnterCell(object sender, EnterCellEventArgs e)
        {
            if (e.Row >= 0 && e.Row < filteredItems.Count)
            {
                NeuObject item = filteredItems[e.Row];
                bool isSelected = selectedItems.Exists(delegate(NeuObject x) { return x.ID == item.ID; });
                bool hasMaxLimit = _maxSelectCount < int.MaxValue;
                bool isDisabled = hasMaxLimit && !isSelected && selectedItems.Count >= _maxSelectCount;

                if (!isDisabled && !isSelected)
                {
                    sheetLeft.Rows[e.Row].BackColor = ColorRowHover;
                }
                lastHoveredRow = e.Row;
            }
        }

        private void FpSpreadLeft_LeaveCell(object sender, LeaveCellEventArgs e)
        {
            if (e.Row >= 0 && e.Row < filteredItems.Count)
            {
                NeuObject item = filteredItems[e.Row];
                bool isSelected = selectedItems.Exists(delegate(NeuObject x) { return x.ID == item.ID; });
                bool hasMaxLimit = _maxSelectCount < int.MaxValue;
                bool isDisabled = hasMaxLimit && !isSelected && selectedItems.Count >= _maxSelectCount;

                if (isDisabled)
                {
                    sheetLeft.Rows[e.Row].BackColor = Color.FromArgb(252, 252, 252);
                }
                else if (isSelected)
                {
                    sheetLeft.Rows[e.Row].BackColor = ColorPrimaryLight;
                }
                else
                {
                    sheetLeft.Rows[e.Row].BackColor = (e.Row % 2 == 0) ? Color.White : ColorRowAlt;
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            if (selectedItems.Count > 0)
            {
                if (MessageBox.Show("当前有已选择的项目，确定要关闭吗？", "确认关闭",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            int addedCount = 0;
            bool hasMaxLimit = _maxSelectCount < int.MaxValue;

            foreach (NeuObject item in filteredItems)
            {
                if (hasMaxLimit && selectedItems.Count >= _maxSelectCount)
                {
                    MessageBox.Show(string.Format("已达到最大选择数量 {0} 项", _maxSelectCount), "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }

                if (!selectedItems.Exists(delegate(NeuObject x) { return x.ID == item.ID; }))
                {
                    selectedItems.Add(item);
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                RefreshBothSpreads();
                UpdateStatus();
            }
        }

        private void BtnInvert_Click(object sender, EventArgs e)
        {
            List<NeuObject> newSelected = new List<NeuObject>();
            int skipped = 0;
            bool hasMaxLimit = _maxSelectCount < int.MaxValue;

            foreach (NeuObject item in selectedItems)
            {
                if (!filteredItems.Exists(delegate(NeuObject x) { return x.ID == item.ID; }))
                {
                    newSelected.Add(item);
                }
            }

            foreach (NeuObject item in filteredItems)
            {
                if (!selectedItems.Exists(delegate(NeuObject x) { return x.ID == item.ID; }))
                {
                    if (!hasMaxLimit || newSelected.Count < _maxSelectCount)
                    {
                        newSelected.Add(item);
                    }
                    else
                    {
                        skipped++;
                    }
                }
            }

            selectedItems = newSelected;
            RefreshBothSpreads();
            UpdateStatus();

            if (skipped > 0)
            {
                MessageBox.Show(string.Format("反选完成，{0} 项因超出上限未添加", skipped), "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("暂无已选项目", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(string.Format("确定要清空已选的 {0} 个项目吗？", selectedItems.Count), "确认清空",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                selectedItems.Clear();
                RefreshBothSpreads();
                UpdateStatus();
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            bool hasSearch = txtSearch.Text != searchPlaceholder && !string.IsNullOrEmpty(txtSearch.Text);
            if (selectedItems.Count == 0 && !hasSearch)
            {
                MessageBox.Show("暂无需要重置的内容", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("确定要重置所有选择和搜索条件吗？", "确认重置",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                txtSearch.Text = searchPlaceholder;
                txtSearch.ForeColor = ColorTextHint;
                selectedItems.Clear();
                filteredItems = new List<NeuObject>(allItems);
                RefreshBothSpreads();
                UpdateStatus();
            }
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (selectedItems.Count < _minSelectCount)
            {
                MessageBox.Show(string.Format("请至少选择 {0} 个项目", _minSelectCount), "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _selectedResult = new List<NeuObject>(selectedItems);
            _isConfirmed = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void TxtSearch_GotFocus(object sender, EventArgs e)
        {
            if (txtSearch.Text == searchPlaceholder)
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = ColorTextPrimary;
            }
            // 聚焦时边框变蓝
            pnlSearchBox.BackColor = ColorPrimaryLight;
            txtSearch.BackColor = ColorPrimaryLight;
        }

        private void TxtSearch_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text) || string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                txtSearch.Text = searchPlaceholder;
                txtSearch.ForeColor = ColorTextHint;
            }
            // 失焦恢复灰色背景
            Color bgColor = Color.FromArgb(245, 246, 248);
            pnlSearchBox.BackColor = bgColor;
            txtSearch.BackColor = bgColor;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (keyword == searchPlaceholder.ToLower() || string.IsNullOrEmpty(keyword))
            {
                filteredItems = new List<NeuObject>(allItems);
            }
            else
            {
                filteredItems = allItems.FindAll(delegate(NeuObject x)
                {
                    return x.Name.ToLower().Contains(keyword) || x.ID.ToLower().Contains(keyword);
                });
            }

            RefreshLeftSpread();
        }

        private void FpSpreadLeft_CellClick(object sender, CellClickEventArgs e)
        {
            if (e.Row < 0 || e.Row >= filteredItems.Count) return;

            NeuObject item = filteredItems[e.Row];
            bool isSelected = selectedItems.Exists(delegate(NeuObject x) { return x.ID == item.ID; });
            bool hasMaxLimit = _maxSelectCount < int.MaxValue;

            if (isSelected)
            {
                selectedItems.RemoveAll(delegate(NeuObject x) { return x.ID == item.ID; });
            }
            else
            {
                if (hasMaxLimit && selectedItems.Count >= _maxSelectCount)
                {
                    MessageBox.Show(string.Format("最多只能选择 {0} 项", _maxSelectCount), "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                selectedItems.Add(item);
            }

            RefreshBothSpreads();
            UpdateStatus();
        }

        private void FpSpreadRight_CellClick(object sender, CellClickEventArgs e)
        {
            if (e.Row < 0 || e.Row >= selectedItems.Count) return;

            if (e.Column == 2)
            {
                selectedItems.RemoveAt(e.Row);
                RefreshBothSpreads();
                UpdateStatus();
            }
        }

        #endregion
    }
}
