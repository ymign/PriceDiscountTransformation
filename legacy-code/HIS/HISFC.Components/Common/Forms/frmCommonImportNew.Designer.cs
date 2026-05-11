namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmCommonImportNew
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
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlUpload = new System.Windows.Forms.Panel();
            this.picUpload = new System.Windows.Forms.PictureBox();
            this.lblUploadTitle = new System.Windows.Forms.Label();
            this.lblUploadHint = new System.Windows.Forms.Label();
            this.pnlFileCard = new System.Windows.Forms.Panel();
            this.picFile = new System.Windows.Forms.PictureBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblFileSize = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.pnlMapping = new System.Windows.Forms.Panel();
            this.lblMappingTitle = new System.Windows.Forms.Label();
            this.lblMappingHint = new System.Windows.Forms.Label();
            this.pnlMappingFields = new System.Windows.Forms.Panel();
            this.pnlDataHeader = new System.Windows.Forms.Panel();
            this.lblDataTitle = new System.Windows.Forms.Label();
            this.lblRowCount = new System.Windows.Forms.Label();
            this.lblColCount = new System.Windows.Forms.Label();
            this.pnlTable = new System.Windows.Forms.Panel();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlUpload.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpload)).BeginInit();
            this.pnlFileCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFile)).BeginInit();
            this.pnlMapping.SuspendLayout();
            this.pnlDataHeader.SuspendLayout();
            this.pnlTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(157)))), ((int)(((byte)(166)))));
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1050, 56);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1002, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(36, 36);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(101, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "数据导入";
            // 
            // pnlUpload
            // 
            this.pnlUpload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlUpload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.pnlUpload.Controls.Add(this.picUpload);
            this.pnlUpload.Controls.Add(this.lblUploadTitle);
            this.pnlUpload.Controls.Add(this.lblUploadHint);
            this.pnlUpload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlUpload.Location = new System.Drawing.Point(28, 68);
            this.pnlUpload.Name = "pnlUpload";
            this.pnlUpload.Size = new System.Drawing.Size(994, 70);
            this.pnlUpload.TabIndex = 1;
            // 
            // picUpload
            // 
            this.picUpload.Location = new System.Drawing.Point(372, 14);
            this.picUpload.Name = "picUpload";
            this.picUpload.Size = new System.Drawing.Size(50, 46);
            this.picUpload.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUpload.TabIndex = 0;
            this.picUpload.TabStop = false;
            // 
            // lblUploadTitle
            // 
            this.lblUploadTitle.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Bold);
            this.lblUploadTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(144)))), ((int)(((byte)(255)))));
            this.lblUploadTitle.Location = new System.Drawing.Point(340, 20);
            this.lblUploadTitle.Name = "lblUploadTitle";
            this.lblUploadTitle.Size = new System.Drawing.Size(400, 24);
            this.lblUploadTitle.TabIndex = 1;
            this.lblUploadTitle.Text = "点击选择文件，或拖拽文件至此区域";
            this.lblUploadTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUploadHint
            // 
            this.lblUploadHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblUploadHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.lblUploadHint.Location = new System.Drawing.Point(340, 44);
            this.lblUploadHint.Name = "lblUploadHint";
            this.lblUploadHint.Size = new System.Drawing.Size(400, 18);
            this.lblUploadHint.TabIndex = 2;
            this.lblUploadHint.Text = "支持格式：Excel (.xlsx, .xls) 、CSV";
            this.lblUploadHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlFileCard
            // 
            this.pnlFileCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFileCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(248)))), ((int)(((byte)(237)))));
            this.pnlFileCard.Controls.Add(this.picFile);
            this.pnlFileCard.Controls.Add(this.lblFileName);
            this.pnlFileCard.Controls.Add(this.lblFileSize);
            this.pnlFileCard.Controls.Add(this.btnRemove);
            this.pnlFileCard.Location = new System.Drawing.Point(28, 146);
            this.pnlFileCard.Name = "pnlFileCard";
            this.pnlFileCard.Size = new System.Drawing.Size(994, 50);
            this.pnlFileCard.TabIndex = 2;
            this.pnlFileCard.Visible = false;
            // 
            // picFile
            // 
            this.picFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(196)))), ((int)(((byte)(26)))));
            this.picFile.Location = new System.Drawing.Point(16, 10);
            this.picFile.Name = "picFile";
            this.picFile.Size = new System.Drawing.Size(30, 30);
            this.picFile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFile.TabIndex = 0;
            this.picFile.TabStop = false;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.lblFileName.Location = new System.Drawing.Point(56, 8);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(96, 19);
            this.lblFileName.TabIndex = 1;
            this.lblFileName.Text = "文件名称.xlsx";
            // 
            // lblFileSize
            // 
            this.lblFileSize.AutoSize = true;
            this.lblFileSize.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblFileSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.lblFileSize.Location = new System.Drawing.Point(56, 29);
            this.lblFileSize.Name = "lblFileSize";
            this.lblFileSize.Size = new System.Drawing.Size(66, 17);
            this.lblFileSize.TabIndex = 2;
            this.lblFileSize.Text = "512.00 KB";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.BackColor = System.Drawing.Color.Transparent;
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnRemove.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnRemove.Location = new System.Drawing.Point(910, 12);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(70, 26);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "移除文件";
            this.btnRemove.UseVisualStyleBackColor = false;
            // 
            // pnlMapping
            // 
            this.pnlMapping.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMapping.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(235)))));
            this.pnlMapping.Controls.Add(this.lblMappingTitle);
            this.pnlMapping.Controls.Add(this.lblMappingHint);
            this.pnlMapping.Controls.Add(this.pnlMappingFields);
            this.pnlMapping.Location = new System.Drawing.Point(28, 202);
            this.pnlMapping.Name = "pnlMapping";
            this.pnlMapping.Size = new System.Drawing.Size(994, 80);
            this.pnlMapping.TabIndex = 3;
            this.pnlMapping.Visible = false;
            // 
            // lblMappingTitle
            // 
            this.lblMappingTitle.AutoSize = true;
            this.lblMappingTitle.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblMappingTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(83)))), ((int)(((byte)(9)))));
            this.lblMappingTitle.Location = new System.Drawing.Point(14, 10);
            this.lblMappingTitle.Name = "lblMappingTitle";
            this.lblMappingTitle.Size = new System.Drawing.Size(93, 19);
            this.lblMappingTitle.TabIndex = 0;
            this.lblMappingTitle.Text = "字段映射配置";
            // 
            // lblMappingHint
            // 
            this.lblMappingHint.AutoSize = true;
            this.lblMappingHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblMappingHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(98)))), ((int)(((byte)(7)))));
            this.lblMappingHint.Location = new System.Drawing.Point(110, 12);
            this.lblMappingHint.Name = "lblMappingHint";
            this.lblMappingHint.Size = new System.Drawing.Size(224, 17);
            this.lblMappingHint.TabIndex = 1;
            this.lblMappingHint.Text = "请选择文件中的列与系统字段的对应关系";
            // 
            // pnlMappingFields
            // 
            this.pnlMappingFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMappingFields.BackColor = System.Drawing.Color.Transparent;
            this.pnlMappingFields.Location = new System.Drawing.Point(14, 36);
            this.pnlMappingFields.Name = "pnlMappingFields";
            this.pnlMappingFields.Size = new System.Drawing.Size(966, 40);
            this.pnlMappingFields.TabIndex = 2;
            // 
            // pnlDataHeader
            // 
            this.pnlDataHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDataHeader.BackColor = System.Drawing.Color.White;
            this.pnlDataHeader.Controls.Add(this.lblDataTitle);
            this.pnlDataHeader.Controls.Add(this.lblRowCount);
            this.pnlDataHeader.Controls.Add(this.lblColCount);
            this.pnlDataHeader.Location = new System.Drawing.Point(28, 150);
            this.pnlDataHeader.Name = "pnlDataHeader";
            this.pnlDataHeader.Size = new System.Drawing.Size(994, 36);
            this.pnlDataHeader.TabIndex = 4;
            // 
            // lblDataTitle
            // 
            this.lblDataTitle.AutoSize = true;
            this.lblDataTitle.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Bold);
            this.lblDataTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblDataTitle.Location = new System.Drawing.Point(0, 8);
            this.lblDataTitle.Name = "lblDataTitle";
            this.lblDataTitle.Size = new System.Drawing.Size(69, 19);
            this.lblDataTitle.TabIndex = 0;
            this.lblDataTitle.Text = "数据预览";
            // 
            // lblRowCount
            // 
            this.lblRowCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRowCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.lblRowCount.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblRowCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(144)))), ((int)(((byte)(255)))));
            this.lblRowCount.Location = new System.Drawing.Point(810, 6);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblRowCount.Size = new System.Drawing.Size(85, 24);
            this.lblRowCount.TabIndex = 1;
            this.lblRowCount.Text = "行数: 0";
            this.lblRowCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblColCount
            // 
            this.lblColCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblColCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(248)))), ((int)(((byte)(237)))));
            this.lblColCount.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblColCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(196)))), ((int)(((byte)(26)))));
            this.lblColCount.Location = new System.Drawing.Point(905, 6);
            this.lblColCount.Name = "lblColCount";
            this.lblColCount.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblColCount.Size = new System.Drawing.Size(85, 24);
            this.lblColCount.TabIndex = 2;
            this.lblColCount.Text = "列数: 0";
            this.lblColCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlTable
            // 
            this.pnlTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.pnlTable.Controls.Add(this.fpSpread1);
            this.pnlTable.Location = new System.Drawing.Point(28, 190);
            this.pnlTable.Name = "pnlTable";
            this.pnlTable.Padding = new System.Windows.Forms.Padding(1);
            this.pnlTable.Size = new System.Drawing.Size(994, 382);
            this.pnlTable.TabIndex = 5;
            // 
            // fpSpread1
            // 
            this.fpSpread1.About = "3.0.2004.2005";
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1";
            this.fpSpread1.BackColor = System.Drawing.Color.White;
            this.fpSpread1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.Location = new System.Drawing.Point(1, 1);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(992, 380);
            this.fpSpread1.TabIndex = 0;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("微软雅黑", 9F);
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpread1.TextTipAppearance = tipAppearance1;
            this.fpSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 10;
            this.fpSpread1_Sheet1.RowCount = 0;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(157)))), ((int)(((byte)(166)))));
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.ForeColor = System.Drawing.Color.White;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread1_Sheet1.ColumnHeader.Rows.Default.Height = 38F;
            this.fpSpread1_Sheet1.Columns.Default.Width = 130F;
            this.fpSpread1_Sheet1.DefaultStyle.BackColor = System.Drawing.Color.White;
            this.fpSpread1_Sheet1.DefaultStyle.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.fpSpread1_Sheet1.DefaultStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.fpSpread1_Sheet1.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.DefaultStyle.Parent = "DataAreaDefault";
            this.fpSpread1_Sheet1.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.fpSpread1_Sheet1.GrayAreaBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.fpSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;
            this.fpSpread1_Sheet1.RowHeader.Visible = false;
            this.fpSpread1_Sheet1.Rows.Default.Height = 34F;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.fpSpread1.SetActiveViewport(0, 1, 0);
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlFooter.Controls.Add(this.btnCancel);
            this.pnlFooter.Controls.Add(this.btnConfirm);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 586);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1050, 64);
            this.pnlFooter.TabIndex = 6;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.btnCancel.Location = new System.Drawing.Point(780, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 40);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "取 消";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(157)))), ((int)(((byte)(166)))));
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FlatAppearance.BorderSize = 0;
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(904, 14);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(120, 40);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "确认";
            this.btnConfirm.UseVisualStyleBackColor = false;
            // 
            // frmCommonImportNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1050, 650);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlTable);
            this.Controls.Add(this.pnlDataHeader);
            this.Controls.Add(this.pnlMapping);
            this.Controls.Add(this.pnlFileCard);
            this.Controls.Add(this.pnlUpload);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCommonImportNew";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "数据导入";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlUpload.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picUpload)).EndInit();
            this.pnlFileCard.ResumeLayout(false);
            this.pnlFileCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFile)).EndInit();
            this.pnlMapping.ResumeLayout(false);
            this.pnlMapping.PerformLayout();
            this.pnlDataHeader.ResumeLayout(false);
            this.pnlDataHeader.PerformLayout();
            this.pnlTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // 头部
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;

        // 上传区
        private System.Windows.Forms.Panel pnlUpload;
        private System.Windows.Forms.PictureBox picUpload;
        private System.Windows.Forms.Label lblUploadTitle;
        private System.Windows.Forms.Label lblUploadHint;

        // 文件卡片
        private System.Windows.Forms.Panel pnlFileCard;
        private System.Windows.Forms.PictureBox picFile;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblFileSize;
        private System.Windows.Forms.Button btnRemove;

        // 列映射区域
        private System.Windows.Forms.Panel pnlMapping;
        private System.Windows.Forms.Label lblMappingTitle;
        private System.Windows.Forms.Label lblMappingHint;
        private System.Windows.Forms.Panel pnlMappingFields;

        // 数据预览区
        private System.Windows.Forms.Panel pnlDataHeader;
        private System.Windows.Forms.Label lblDataTitle;
        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.Label lblColCount;

        // 表格
        private System.Windows.Forms.Panel pnlTable;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;

        // 底部
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
    }
}