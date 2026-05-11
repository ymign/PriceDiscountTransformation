namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmMultipleSelection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.TipAppearance tipAppearance2 = new FarPoint.Win.Spread.TipAppearance();
            this.pnlShadow = new System.Windows.Forms.Panel();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlRightBorder = new System.Windows.Forms.Panel();
            this.pnlRightContent = new System.Windows.Forms.Panel();
            this.lblEmptyHint = new System.Windows.Forms.Label();
            this.fpSpreadRight = new FarPoint.Win.Spread.FpSpread();
            this.sheetRight = new FarPoint.Win.Spread.SheetView();
            this.pnlRightHeaderLine = new System.Windows.Forms.Panel();
            this.pnlRightHeader = new System.Windows.Forms.Panel();
            this.lblRightTitle = new System.Windows.Forms.Label();
            this.lblRightCount = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.pnlSplitter = new System.Windows.Forms.Panel();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlLeftBorder = new System.Windows.Forms.Panel();
            this.pnlLeftContent = new System.Windows.Forms.Panel();
            this.fpSpreadLeft = new FarPoint.Win.Spread.FpSpread();
            this.sheetLeft = new FarPoint.Win.Spread.SheetView();
            this.pnlLeftHeaderLine = new System.Windows.Forms.Panel();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.pnlSearchBox = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlLeftHeader = new System.Windows.Forms.Panel();
            this.lblLeftTitle = new System.Windows.Forms.Label();
            this.lblLeftCount = new System.Windows.Forms.Label();
            this.btnInvert = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.pnlFooterLine = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.lblLimitHint = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.pnlConfigBar = new System.Windows.Forms.Panel();
            this.lblConfigIcon = new System.Windows.Forms.Label();
            this.lblConfigInfo = new System.Windows.Forms.Label();
            this.pnlTitleBar = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblSearchIcon = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlShadow.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlRightBorder.SuspendLayout();
            this.pnlRightContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpreadRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetRight)).BeginInit();
            this.pnlRightHeader.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlLeftBorder.SuspendLayout();
            this.pnlLeftContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpreadLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetLeft)).BeginInit();
            this.pnlSearch.SuspendLayout();
            this.pnlSearchBox.SuspendLayout();
            this.pnlLeftHeader.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.pnlConfigBar.SuspendLayout();
            this.pnlTitleBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlShadow
            // 
            this.pnlShadow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.pnlShadow.Controls.Add(this.pnlContainer);
            this.pnlShadow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShadow.Location = new System.Drawing.Point(0, 0);
            this.pnlShadow.Name = "pnlShadow";
            this.pnlShadow.Padding = new System.Windows.Forms.Padding(1);
            this.pnlShadow.Size = new System.Drawing.Size(882, 562);
            this.pnlShadow.TabIndex = 0;
            // 
            // pnlContainer
            // 
            this.pnlContainer.BackColor = System.Drawing.Color.White;
            this.pnlContainer.Controls.Add(this.pnlMain);
            this.pnlContainer.Controls.Add(this.pnlFooter);
            this.pnlContainer.Controls.Add(this.pnlConfigBar);
            this.pnlContainer.Controls.Add(this.pnlTitleBar);
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(1, 1);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(880, 560);
            this.pnlContainer.TabIndex = 0;
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlMain.Controls.Add(this.pnlRight);
            this.pnlMain.Controls.Add(this.pnlSplitter);
            this.pnlMain.Controls.Add(this.pnlLeft);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 80);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.pnlMain.Size = new System.Drawing.Size(880, 420);
            this.pnlMain.TabIndex = 2;
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.White;
            this.pnlRight.Controls.Add(this.pnlRightBorder);
            this.pnlRight.Controls.Add(this.pnlRightHeader);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(528, 12);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(336, 396);
            this.pnlRight.TabIndex = 2;
            // 
            // pnlRightBorder
            // 
            this.pnlRightBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.pnlRightBorder.Controls.Add(this.pnlRightContent);
            this.pnlRightBorder.Controls.Add(this.pnlRightHeaderLine);
            this.pnlRightBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRightBorder.Location = new System.Drawing.Point(0, 40);
            this.pnlRightBorder.Name = "pnlRightBorder";
            this.pnlRightBorder.Padding = new System.Windows.Forms.Padding(1);
            this.pnlRightBorder.Size = new System.Drawing.Size(336, 356);
            this.pnlRightBorder.TabIndex = 1;
            // 
            // pnlRightContent
            // 
            this.pnlRightContent.BackColor = System.Drawing.Color.White;
            this.pnlRightContent.Controls.Add(this.lblEmptyHint);
            this.pnlRightContent.Controls.Add(this.fpSpreadRight);
            this.pnlRightContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRightContent.Location = new System.Drawing.Point(1, 3);
            this.pnlRightContent.Name = "pnlRightContent";
            this.pnlRightContent.Size = new System.Drawing.Size(334, 352);
            this.pnlRightContent.TabIndex = 1;
            // 
            // lblEmptyHint
            // 
            this.lblEmptyHint.BackColor = System.Drawing.Color.White;
            this.lblEmptyHint.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblEmptyHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.lblEmptyHint.Location = new System.Drawing.Point(50, 100);
            this.lblEmptyHint.Name = "lblEmptyHint";
            this.lblEmptyHint.Size = new System.Drawing.Size(234, 60);
            this.lblEmptyHint.TabIndex = 1;
            this.lblEmptyHint.Text = "暂无已选项目\r\n请从左侧列表中选择";
            this.lblEmptyHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fpSpreadRight
            // 
            this.fpSpreadRight.About = "3.0.2004.2005";
            this.fpSpreadRight.AccessibleDescription = "fpSpreadRight";
            this.fpSpreadRight.BackColor = System.Drawing.Color.White;
            this.fpSpreadRight.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fpSpreadRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpreadRight.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpreadRight.Location = new System.Drawing.Point(0, 0);
            this.fpSpreadRight.Name = "fpSpreadRight";
            this.fpSpreadRight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpSpreadRight.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.sheetRight});
            this.fpSpreadRight.Size = new System.Drawing.Size(334, 352);
            this.fpSpreadRight.TabIndex = 0;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("微软雅黑", 9F);
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpreadRight.TextTipAppearance = tipAppearance1;
            this.fpSpreadRight.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpreadRight.VerticalScrollBarWidth = 8;
            // 
            // sheetRight
            // 
            this.sheetRight.Reset();
            this.sheetRight.SheetName = "已选项目";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.sheetRight.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.sheetRight.ColumnCount = 3;
            this.sheetRight.RowCount = 0;
            this.sheetRight.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.sheetRight.ColumnHeader.DefaultStyle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.sheetRight.ColumnHeader.DefaultStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.sheetRight.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.sheetRight.ColumnHeader.Rows.Default.Height = 34F;
            this.sheetRight.DefaultStyle.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.sheetRight.DefaultStyle.Parent = "DataAreaDefault";
            this.sheetRight.GrayAreaBackColor = System.Drawing.Color.White;
            this.sheetRight.RowHeader.Visible = false;
            this.sheetRight.Rows.Default.Height = 36F;
            this.sheetRight.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.fpSpreadRight.SetActiveViewport(0, 1, 0);
            // 
            // pnlRightHeaderLine
            // 
            this.pnlRightHeaderLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(196)))), ((int)(((byte)(26)))));
            this.pnlRightHeaderLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRightHeaderLine.Location = new System.Drawing.Point(1, 1);
            this.pnlRightHeaderLine.Name = "pnlRightHeaderLine";
            this.pnlRightHeaderLine.Size = new System.Drawing.Size(334, 2);
            this.pnlRightHeaderLine.TabIndex = 0;
            // 
            // pnlRightHeader
            // 
            this.pnlRightHeader.BackColor = System.Drawing.Color.White;
            this.pnlRightHeader.Controls.Add(this.lblRightTitle);
            this.pnlRightHeader.Controls.Add(this.lblRightCount);
            this.pnlRightHeader.Controls.Add(this.btnClear);
            this.pnlRightHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRightHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlRightHeader.Name = "pnlRightHeader";
            this.pnlRightHeader.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlRightHeader.Size = new System.Drawing.Size(336, 40);
            this.pnlRightHeader.TabIndex = 0;
            // 
            // lblRightTitle
            // 
            this.lblRightTitle.AutoSize = true;
            this.lblRightTitle.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblRightTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblRightTitle.Location = new System.Drawing.Point(12, 10);
            this.lblRightTitle.Name = "lblRightTitle";
            this.lblRightTitle.Size = new System.Drawing.Size(65, 19);
            this.lblRightTitle.TabIndex = 0;
            this.lblRightTitle.Text = "已选项目";
            // 
            // lblRightCount
            // 
            this.lblRightCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.lblRightCount.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Bold);
            this.lblRightCount.ForeColor = System.Drawing.Color.White;
            this.lblRightCount.Location = new System.Drawing.Point(80, 9);
            this.lblRightCount.Name = "lblRightCount";
            this.lblRightCount.Size = new System.Drawing.Size(30, 20);
            this.lblRightCount.TabIndex = 1;
            this.lblRightCount.Text = "0";
            this.lblRightCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClear.Location = new System.Drawing.Point(274, 6);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(54, 26);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "清空";
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // pnlSplitter
            // 
            this.pnlSplitter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlSplitter.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSplitter.Location = new System.Drawing.Point(516, 12);
            this.pnlSplitter.Name = "pnlSplitter";
            this.pnlSplitter.Size = new System.Drawing.Size(12, 396);
            this.pnlSplitter.TabIndex = 1;
            // 
            // pnlLeft
            // 
            this.pnlLeft.BackColor = System.Drawing.Color.White;
            this.pnlLeft.Controls.Add(this.pnlLeftBorder);
            this.pnlLeft.Controls.Add(this.pnlSearch);
            this.pnlLeft.Controls.Add(this.pnlLeftHeader);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(16, 12);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(500, 396);
            this.pnlLeft.TabIndex = 0;
            // 
            // pnlLeftBorder
            // 
            this.pnlLeftBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.pnlLeftBorder.Controls.Add(this.pnlLeftContent);
            this.pnlLeftBorder.Controls.Add(this.pnlLeftHeaderLine);
            this.pnlLeftBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeftBorder.Location = new System.Drawing.Point(0, 84);
            this.pnlLeftBorder.Name = "pnlLeftBorder";
            this.pnlLeftBorder.Padding = new System.Windows.Forms.Padding(1);
            this.pnlLeftBorder.Size = new System.Drawing.Size(500, 312);
            this.pnlLeftBorder.TabIndex = 2;
            // 
            // pnlLeftContent
            // 
            this.pnlLeftContent.BackColor = System.Drawing.Color.White;
            this.pnlLeftContent.Controls.Add(this.fpSpreadLeft);
            this.pnlLeftContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeftContent.Location = new System.Drawing.Point(1, 3);
            this.pnlLeftContent.Name = "pnlLeftContent";
            this.pnlLeftContent.Size = new System.Drawing.Size(498, 308);
            this.pnlLeftContent.TabIndex = 1;
            // 
            // fpSpreadLeft
            // 
            this.fpSpreadLeft.About = "3.0.2004.2005";
            this.fpSpreadLeft.AccessibleDescription = "fpSpreadLeft";
            this.fpSpreadLeft.BackColor = System.Drawing.Color.White;
            this.fpSpreadLeft.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fpSpreadLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpreadLeft.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpreadLeft.Location = new System.Drawing.Point(0, 0);
            this.fpSpreadLeft.Name = "fpSpreadLeft";
            this.fpSpreadLeft.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpSpreadLeft.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.sheetLeft});
            this.fpSpreadLeft.Size = new System.Drawing.Size(498, 308);
            this.fpSpreadLeft.TabIndex = 0;
            tipAppearance2.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance2.Font = new System.Drawing.Font("微软雅黑", 9F);
            tipAppearance2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpreadLeft.TextTipAppearance = tipAppearance2;
            this.fpSpreadLeft.VerticalScrollBarWidth = 8;
            // 
            // sheetLeft
            // 
            this.sheetLeft.Reset();
            this.sheetLeft.SheetName = "待选项目";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.sheetLeft.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.sheetLeft.ColumnCount = 3;
            this.sheetLeft.RowCount = 0;
            this.sheetLeft.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.sheetLeft.ColumnHeader.DefaultStyle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.sheetLeft.ColumnHeader.DefaultStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.sheetLeft.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.sheetLeft.ColumnHeader.Rows.Default.Height = 34F;
            this.sheetLeft.DefaultStyle.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.sheetLeft.DefaultStyle.Parent = "DataAreaDefault";
            this.sheetLeft.GrayAreaBackColor = System.Drawing.Color.White;
            this.sheetLeft.RowHeader.Visible = false;
            this.sheetLeft.Rows.Default.Height = 36F;
            this.sheetLeft.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.fpSpreadLeft.SetActiveViewport(0, 1, 0);
            // 
            // pnlLeftHeaderLine
            // 
            this.pnlLeftHeaderLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(144)))), ((int)(((byte)(255)))));
            this.pnlLeftHeaderLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLeftHeaderLine.Location = new System.Drawing.Point(1, 1);
            this.pnlLeftHeaderLine.Name = "pnlLeftHeaderLine";
            this.pnlLeftHeaderLine.Size = new System.Drawing.Size(498, 2);
            this.pnlLeftHeaderLine.TabIndex = 0;
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.White;
            this.pnlSearch.Controls.Add(this.pnlSearchBox);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 40);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding = new System.Windows.Forms.Padding(12, 6, 12, 6);
            this.pnlSearch.Size = new System.Drawing.Size(500, 44);
            this.pnlSearch.TabIndex = 1;
            // 
            // pnlSearchBox
            // 
            this.pnlSearchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.pnlSearchBox.Controls.Add(this.txtSearch);
            this.pnlSearchBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearchBox.Location = new System.Drawing.Point(12, 6);
            this.pnlSearchBox.Name = "pnlSearchBox";
            this.pnlSearchBox.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.pnlSearchBox.Size = new System.Drawing.Size(476, 32);
            this.pnlSearchBox.TabIndex = 0;
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.txtSearch.Location = new System.Drawing.Point(10, 6);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(456, 18);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.Text = "搜索名称或编码...";
            // 
            // pnlLeftHeader
            // 
            this.pnlLeftHeader.BackColor = System.Drawing.Color.White;
            this.pnlLeftHeader.Controls.Add(this.lblLeftTitle);
            this.pnlLeftHeader.Controls.Add(this.lblLeftCount);
            this.pnlLeftHeader.Controls.Add(this.btnInvert);
            this.pnlLeftHeader.Controls.Add(this.btnSelectAll);
            this.pnlLeftHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLeftHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlLeftHeader.Name = "pnlLeftHeader";
            this.pnlLeftHeader.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlLeftHeader.Size = new System.Drawing.Size(500, 40);
            this.pnlLeftHeader.TabIndex = 0;
            // 
            // lblLeftTitle
            // 
            this.lblLeftTitle.AutoSize = true;
            this.lblLeftTitle.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblLeftTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblLeftTitle.Location = new System.Drawing.Point(12, 10);
            this.lblLeftTitle.Name = "lblLeftTitle";
            this.lblLeftTitle.Size = new System.Drawing.Size(65, 19);
            this.lblLeftTitle.TabIndex = 0;
            this.lblLeftTitle.Text = "待选项目";
            // 
            // lblLeftCount
            // 
            this.lblLeftCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(157)))), ((int)(((byte)(166)))));
            this.lblLeftCount.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Bold);
            this.lblLeftCount.ForeColor = System.Drawing.Color.White;
            this.lblLeftCount.Location = new System.Drawing.Point(80, 9);
            this.lblLeftCount.Name = "lblLeftCount";
            this.lblLeftCount.Size = new System.Drawing.Size(30, 20);
            this.lblLeftCount.TabIndex = 1;
            this.lblLeftCount.Text = "12";
            this.lblLeftCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnInvert
            // 
            this.btnInvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInvert.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.btnInvert.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInvert.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(144)))), ((int)(((byte)(255)))));
            this.btnInvert.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(144)))), ((int)(((byte)(255)))));
            this.btnInvert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInvert.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnInvert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(144)))), ((int)(((byte)(255)))));
            this.btnInvert.Location = new System.Drawing.Point(438, 6);
            this.btnInvert.Name = "btnInvert";
            this.btnInvert.Size = new System.Drawing.Size(54, 26);
            this.btnInvert.TabIndex = 3;
            this.btnInvert.Text = "反选";
            this.btnInvert.UseVisualStyleBackColor = false;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.btnSelectAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelectAll.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(144)))), ((int)(((byte)(255)))));
            this.btnSelectAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(144)))), ((int)(((byte)(255)))));
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectAll.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnSelectAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(144)))), ((int)(((byte)(255)))));
            this.btnSelectAll.Location = new System.Drawing.Point(378, 6);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(54, 26);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = "全选";
            this.btnSelectAll.UseVisualStyleBackColor = false;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(252)))));
            this.pnlFooter.Controls.Add(this.pnlFooterLine);
            this.pnlFooter.Controls.Add(this.lblSummary);
            this.pnlFooter.Controls.Add(this.lblLimitHint);
            this.pnlFooter.Controls.Add(this.btnReset);
            this.pnlFooter.Controls.Add(this.btnConfirm);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 500);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(880, 60);
            this.pnlFooter.TabIndex = 3;
            // 
            // pnlFooterLine
            // 
            this.pnlFooterLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.pnlFooterLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFooterLine.Location = new System.Drawing.Point(0, 0);
            this.pnlFooterLine.Name = "pnlFooterLine";
            this.pnlFooterLine.Size = new System.Drawing.Size(880, 1);
            this.pnlFooterLine.TabIndex = 0;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.lblSummary.Location = new System.Drawing.Point(20, 20);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(111, 20);
            this.lblSummary.TabIndex = 1;
            this.lblSummary.Text = "已选择 0 / 12 项";
            // 
            // lblLimitHint
            // 
            this.lblLimitHint.AutoSize = true;
            this.lblLimitHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblLimitHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.lblLimitHint.Location = new System.Drawing.Point(130, 21);
            this.lblLimitHint.Name = "lblLimitHint";
            this.lblLimitHint.Size = new System.Drawing.Size(0, 17);
            this.lblLimitHint.TabIndex = 2;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.BackColor = System.Drawing.Color.White;
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.btnReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.btnReset.Location = new System.Drawing.Point(640, 12);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 36);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "重置";
            this.btnReset.UseVisualStyleBackColor = false;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(157)))), ((int)(((byte)(166)))));
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FlatAppearance.BorderSize = 0;
            this.btnConfirm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(109)))), ((int)(((byte)(217)))));
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(756, 12);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(108, 36);
            this.btnConfirm.TabIndex = 4;
            this.btnConfirm.Text = "确认选择";
            this.btnConfirm.UseVisualStyleBackColor = false;
            // 
            // pnlConfigBar
            // 
            this.pnlConfigBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(235)))));
            this.pnlConfigBar.Controls.Add(this.lblConfigIcon);
            this.pnlConfigBar.Controls.Add(this.lblConfigInfo);
            this.pnlConfigBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlConfigBar.Location = new System.Drawing.Point(0, 48);
            this.pnlConfigBar.Name = "pnlConfigBar";
            this.pnlConfigBar.Size = new System.Drawing.Size(880, 32);
            this.pnlConfigBar.TabIndex = 1;
            // 
            // lblConfigIcon
            // 
            this.lblConfigIcon.AutoSize = true;
            this.lblConfigIcon.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.lblConfigIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(102)))), ((int)(((byte)(0)))));
            this.lblConfigIcon.Location = new System.Drawing.Point(16, 4);
            this.lblConfigIcon.Name = "lblConfigIcon";
            this.lblConfigIcon.Size = new System.Drawing.Size(20, 22);
            this.lblConfigIcon.TabIndex = 0;
            this.lblConfigIcon.Text = "●";
            // 
            // lblConfigInfo
            // 
            this.lblConfigInfo.AutoSize = true;
            this.lblConfigInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblConfigInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(102)))), ((int)(((byte)(0)))));
            this.lblConfigInfo.Location = new System.Drawing.Point(36, 7);
            this.lblConfigInfo.Name = "lblConfigInfo";
            this.lblConfigInfo.Size = new System.Drawing.Size(182, 17);
            this.lblConfigInfo.TabIndex = 1;
            this.lblConfigInfo.Text = "选择限制：最少 1 项，最多 5 项";
            // 
            // pnlTitleBar
            // 
            this.pnlTitleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(157)))), ((int)(((byte)(166)))));
            this.pnlTitleBar.Controls.Add(this.lblTitle);
            this.pnlTitleBar.Controls.Add(this.btnClose);
            this.pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTitleBar.Name = "pnlTitleBar";
            this.pnlTitleBar.Size = new System.Drawing.Size(880, 48);
            this.pnlTitleBar.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(106, 22);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "多项选择界面";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(840, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(32, 32);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // lblSearchIcon
            // 
            this.lblSearchIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblSearchIcon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblSearchIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.lblSearchIcon.Location = new System.Drawing.Point(0, 0);
            this.lblSearchIcon.Name = "lblSearchIcon";
            this.lblSearchIcon.Size = new System.Drawing.Size(0, 0);
            this.lblSearchIcon.TabIndex = 0;
            this.lblSearchIcon.Visible = false;
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 300;
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 100;
            // 
            // frmMultipleSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.ClientSize = new System.Drawing.Size(882, 562);
            this.Controls.Add(this.pnlShadow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMultipleSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "多选项目选择";
            this.pnlShadow.ResumeLayout(false);
            this.pnlContainer.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlRightBorder.ResumeLayout(false);
            this.pnlRightContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpreadRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetRight)).EndInit();
            this.pnlRightHeader.ResumeLayout(false);
            this.pnlRightHeader.PerformLayout();
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeftBorder.ResumeLayout(false);
            this.pnlLeftContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpreadLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetLeft)).EndInit();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearchBox.ResumeLayout(false);
            this.pnlSearchBox.PerformLayout();
            this.pnlLeftHeader.ResumeLayout(false);
            this.pnlLeftHeader.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.pnlConfigBar.ResumeLayout(false);
            this.pnlConfigBar.PerformLayout();
            this.pnlTitleBar.ResumeLayout(false);
            this.pnlTitleBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // 阴影边框
        private System.Windows.Forms.Panel pnlShadow;
        private System.Windows.Forms.Panel pnlContainer;

        // 标题栏
        private System.Windows.Forms.Panel pnlTitleBar;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;

        // 配置提示栏
        private System.Windows.Forms.Panel pnlConfigBar;
        private System.Windows.Forms.Label lblConfigIcon;
        private System.Windows.Forms.Label lblConfigInfo;

        // 主内容区
        private System.Windows.Forms.Panel pnlMain;

        // 左侧面板
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlLeftHeader;
        private System.Windows.Forms.Label lblLeftTitle;
        private System.Windows.Forms.Label lblLeftCount;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnInvert;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Panel pnlSearchBox;
        private System.Windows.Forms.Label lblSearchIcon;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel pnlLeftBorder;
        private System.Windows.Forms.Panel pnlLeftHeaderLine;
        private System.Windows.Forms.Panel pnlLeftContent;
        private FarPoint.Win.Spread.FpSpread fpSpreadLeft;
        private FarPoint.Win.Spread.SheetView sheetLeft;

        // 分隔栏
        private System.Windows.Forms.Panel pnlSplitter;

        // 右侧面板
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlRightHeader;
        private System.Windows.Forms.Label lblRightTitle;
        private System.Windows.Forms.Label lblRightCount;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Panel pnlRightBorder;
        private System.Windows.Forms.Panel pnlRightHeaderLine;
        private System.Windows.Forms.Panel pnlRightContent;
        private System.Windows.Forms.Label lblEmptyHint;
        private FarPoint.Win.Spread.FpSpread fpSpreadRight;
        private FarPoint.Win.Spread.SheetView sheetRight;

        // 底部栏
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Panel pnlFooterLine;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Label lblLimitHint;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnConfirm;

        // ToolTip
        private System.Windows.Forms.ToolTip toolTip1;
    }
}