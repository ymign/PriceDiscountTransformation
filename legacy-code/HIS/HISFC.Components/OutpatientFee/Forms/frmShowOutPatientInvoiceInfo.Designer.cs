namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    partial class frmShowOutPatientInvoiceInfo
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
            FarPoint.Win.Spread.TipAppearance tipAppearance3 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.TextCellType textCellType5 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType6 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.neuSpread1_Sheet2 = new FarPoint.Win.Spread.SheetView();
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.tbQuery = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuEndDate = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.neuLabel7 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuBeginDate = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.neuLabel6 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbIDNo = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel5 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbGender = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbBirth = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbCardNo = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuButton1 = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuLabel8 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbVacancy = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet2)).BeginInit();
            this.neuGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuSpread1
            // 
            this.neuSpread1.About = "3.0.2004.2005";
            this.neuSpread1.AccessibleDescription = "neuSpread1, 发票信息, Row 0, Column 0, ";
            this.neuSpread1.BackColor = System.Drawing.Color.White;
            this.neuSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuSpread1.FileName = "";
            this.neuSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.IsAutoSaveGridStatus = false;
            this.neuSpread1.IsCanCustomConfigColumn = false;
            this.neuSpread1.Location = new System.Drawing.Point(0, 121);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1,
            this.neuSpread1_Sheet2});
            this.neuSpread1.Size = new System.Drawing.Size(842, 255);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 1;
            tipAppearance3.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance3.ForeColor = System.Drawing.SystemColors.InfoText;
            this.neuSpread1.TextTipAppearance = tipAppearance3;
            this.neuSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.neuSpread1_CellDoubleClick);
            // 
            // neuSpread1_Sheet1
            // 
            this.neuSpread1_Sheet1.Reset();
            this.neuSpread1_Sheet1.SheetName = "发票信息";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.neuSpread1_Sheet1.ColumnCount = 9;
            this.neuSpread1_Sheet1.RowCount = 1;
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "发票号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "病历号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "患者姓名";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "合同单位";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "总金额";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "公费金额";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "自费金额";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "操作员";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "操作时间";
            this.neuSpread1_Sheet1.Columns.Get(0).CellType = textCellType5;
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "发票号";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 105F;
            this.neuSpread1_Sheet1.Columns.Get(1).CellType = textCellType6;
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "病历号";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 105F;
            this.neuSpread1_Sheet1.Columns.Get(2).Label = "患者姓名";
            this.neuSpread1_Sheet1.Columns.Get(2).Width = 70F;
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "合同单位";
            this.neuSpread1_Sheet1.Columns.Get(3).Width = 80F;
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "总金额";
            this.neuSpread1_Sheet1.Columns.Get(4).Width = 70F;
            this.neuSpread1_Sheet1.Columns.Get(5).Label = "公费金额";
            this.neuSpread1_Sheet1.Columns.Get(5).Width = 70F;
            this.neuSpread1_Sheet1.Columns.Get(6).Label = "自费金额";
            this.neuSpread1_Sheet1.Columns.Get(6).Width = 70F;
            this.neuSpread1_Sheet1.Columns.Get(7).Label = "操作员";
            this.neuSpread1_Sheet1.Columns.Get(7).Width = 70F;
            this.neuSpread1_Sheet1.Columns.Get(8).Label = "操作时间";
            this.neuSpread1_Sheet1.Columns.Get(8).Width = 147F;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // neuSpread1_Sheet2
            // 
            this.neuSpread1_Sheet2.Reset();
            this.neuSpread1_Sheet2.SheetName = "费用信息";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.neuSpread1_Sheet2.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.neuSpread1_Sheet2.ColumnCount = 2;
            this.neuSpread1_Sheet2.RowCount = 1;
            this.neuSpread1_Sheet2.ColumnHeader.Cells.Get(0, 0).Value = "费用分类";
            this.neuSpread1_Sheet2.ColumnHeader.Cells.Get(0, 1).Value = "金额";
            this.neuSpread1_Sheet2.Columns.Get(0).Label = "费用分类";
            this.neuSpread1_Sheet2.Columns.Get(0).Width = 100F;
            this.neuSpread1_Sheet2.Columns.Get(1).Label = "金额";
            this.neuSpread1_Sheet2.Columns.Get(1).Width = 100F;
            this.neuSpread1_Sheet2.RowHeader.Columns.Default.Resizable = false;
            this.neuSpread1_Sheet2.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Controls.Add(this.tbVacancy);
            this.neuGroupBox1.Controls.Add(this.neuLabel8);
            this.neuGroupBox1.Controls.Add(this.neuButton1);
            this.neuGroupBox1.Controls.Add(this.tbQuery);
            this.neuGroupBox1.Controls.Add(this.neuEndDate);
            this.neuGroupBox1.Controls.Add(this.neuLabel7);
            this.neuGroupBox1.Controls.Add(this.neuBeginDate);
            this.neuGroupBox1.Controls.Add(this.neuLabel6);
            this.neuGroupBox1.Controls.Add(this.tbIDNo);
            this.neuGroupBox1.Controls.Add(this.neuLabel5);
            this.neuGroupBox1.Controls.Add(this.tbGender);
            this.neuGroupBox1.Controls.Add(this.neuLabel4);
            this.neuGroupBox1.Controls.Add(this.tbBirth);
            this.neuGroupBox1.Controls.Add(this.neuLabel3);
            this.neuGroupBox1.Controls.Add(this.tbCardNo);
            this.neuGroupBox1.Controls.Add(this.neuLabel2);
            this.neuGroupBox1.Controls.Add(this.tbName);
            this.neuGroupBox1.Controls.Add(this.neuLabel1);
            this.neuGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(842, 121);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 0;
            this.neuGroupBox1.TabStop = false;
            this.neuGroupBox1.Text = "患者基本信息";
            // 
            // tbQuery
            // 
            this.tbQuery.Location = new System.Drawing.Point(529, 83);
            this.tbQuery.Name = "tbQuery";
            this.tbQuery.Size = new System.Drawing.Size(75, 23);
            this.tbQuery.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbQuery.TabIndex = 14;
            this.tbQuery.Text = "查询";
            this.tbQuery.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.tbQuery.UseVisualStyleBackColor = true;
            this.tbQuery.Click += new System.EventHandler(this.tbQuery_Click);
            // 
            // neuEndDate
            // 
            this.neuEndDate.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.neuEndDate.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.neuEndDate.IsEnter2Tab = false;
            this.neuEndDate.Location = new System.Drawing.Point(313, 82);
            this.neuEndDate.Name = "neuEndDate";
            this.neuEndDate.Size = new System.Drawing.Size(183, 24);
            this.neuEndDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuEndDate.TabIndex = 13;
            // 
            // neuLabel7
            // 
            this.neuLabel7.AutoSize = true;
            this.neuLabel7.Location = new System.Drawing.Point(281, 89);
            this.neuLabel7.Name = "neuLabel7";
            this.neuLabel7.Size = new System.Drawing.Size(23, 12);
            this.neuLabel7.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel7.TabIndex = 12;
            this.neuLabel7.Text = "---";
            // 
            // neuBeginDate
            // 
            this.neuBeginDate.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.neuBeginDate.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuBeginDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.neuBeginDate.IsEnter2Tab = false;
            this.neuBeginDate.Location = new System.Drawing.Point(92, 82);
            this.neuBeginDate.Name = "neuBeginDate";
            this.neuBeginDate.Size = new System.Drawing.Size(183, 24);
            this.neuBeginDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuBeginDate.TabIndex = 11;
            // 
            // neuLabel6
            // 
            this.neuLabel6.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel6.Location = new System.Drawing.Point(12, 87);
            this.neuLabel6.Name = "neuLabel6";
            this.neuLabel6.Size = new System.Drawing.Size(65, 16);
            this.neuLabel6.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel6.TabIndex = 10;
            this.neuLabel6.Text = "时间段:";
            // 
            // tbIDNo
            // 
            this.tbIDNo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbIDNo.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbIDNo.IsEnter2Tab = false;
            this.tbIDNo.Location = new System.Drawing.Point(92, 54);
            this.tbIDNo.Name = "tbIDNo";
            this.tbIDNo.ReadOnly = true;
            this.tbIDNo.Size = new System.Drawing.Size(183, 24);
            this.tbIDNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbIDNo.TabIndex = 9;
            // 
            // neuLabel5
            // 
            this.neuLabel5.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel5.Location = new System.Drawing.Point(12, 58);
            this.neuLabel5.Name = "neuLabel5";
            this.neuLabel5.Size = new System.Drawing.Size(90, 16);
            this.neuLabel5.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel5.TabIndex = 8;
            this.neuLabel5.Text = "证件号:";
            // 
            // tbGender
            // 
            this.tbGender.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbGender.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbGender.IsEnter2Tab = false;
            this.tbGender.Location = new System.Drawing.Point(671, 24);
            this.tbGender.Name = "tbGender";
            this.tbGender.ReadOnly = true;
            this.tbGender.Size = new System.Drawing.Size(100, 24);
            this.tbGender.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbGender.TabIndex = 7;
            // 
            // neuLabel4
            // 
            this.neuLabel4.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel4.Location = new System.Drawing.Point(625, 28);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(62, 16);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 6;
            this.neuLabel4.Text = "性别:";
            // 
            // tbBirth
            // 
            this.tbBirth.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbBirth.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbBirth.IsEnter2Tab = false;
            this.tbBirth.Location = new System.Drawing.Point(489, 24);
            this.tbBirth.Name = "tbBirth";
            this.tbBirth.ReadOnly = true;
            this.tbBirth.Size = new System.Drawing.Size(100, 24);
            this.tbBirth.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbBirth.TabIndex = 5;
            // 
            // neuLabel3
            // 
            this.neuLabel3.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel3.Location = new System.Drawing.Point(407, 28);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(76, 16);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 4;
            this.neuLabel3.Text = "出生年月:";
            // 
            // tbCardNo
            // 
            this.tbCardNo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbCardNo.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCardNo.IsEnter2Tab = false;
            this.tbCardNo.Location = new System.Drawing.Point(274, 24);
            this.tbCardNo.Name = "tbCardNo";
            this.tbCardNo.ReadOnly = true;
            this.tbCardNo.Size = new System.Drawing.Size(100, 24);
            this.tbCardNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCardNo.TabIndex = 3;
            // 
            // neuLabel2
            // 
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel2.Location = new System.Drawing.Point(211, 28);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(90, 16);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 2;
            this.neuLabel2.Text = "病历号:";
            // 
            // tbName
            // 
            this.tbName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbName.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbName.IsEnter2Tab = false;
            this.tbName.Location = new System.Drawing.Point(92, 24);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(100, 24);
            this.tbName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbName.TabIndex = 1;
            // 
            // neuLabel1
            // 
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel1.Location = new System.Drawing.Point(12, 28);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(90, 16);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 0;
            this.neuLabel1.Text = "患者姓名:";
            // 
            // neuButton1
            // 
            this.neuButton1.Location = new System.Drawing.Point(721, 83);
            this.neuButton1.Name = "neuButton1";
            this.neuButton1.Size = new System.Drawing.Size(75, 23);
            this.neuButton1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuButton1.TabIndex = 15;
            this.neuButton1.Text = "退出";
            this.neuButton1.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.neuButton1.UseVisualStyleBackColor = true;
            this.neuButton1.Click += new System.EventHandler(this.neuButton1_Click);
            // 
            // neuLabel8
            // 
            this.neuLabel8.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel8.Location = new System.Drawing.Point(407, 58);
            this.neuLabel8.Name = "neuLabel8";
            this.neuLabel8.Size = new System.Drawing.Size(76, 16);
            this.neuLabel8.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel8.TabIndex = 16;
            this.neuLabel8.Text = "账户余额:";
            this.neuLabel8.Visible = false;
            // 
            // tbVacancy
            // 
            this.tbVacancy.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbVacancy.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbVacancy.IsEnter2Tab = false;
            this.tbVacancy.Location = new System.Drawing.Point(489, 54);
            this.tbVacancy.Name = "tbVacancy";
            this.tbVacancy.ReadOnly = true;
            this.tbVacancy.Size = new System.Drawing.Size(100, 24);
            this.tbVacancy.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbVacancy.TabIndex = 17;
            this.tbVacancy.Visible = false;
            // 
            // frmShowOutPatientInvoiceInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 376);
            this.Controls.Add(this.neuSpread1);
            this.Controls.Add(this.neuGroupBox1);
            this.Name = "frmShowOutPatientInvoiceInfo";
            this.Text = "门诊患者缴费信息";
            this.Load += new System.EventHandler(this.frmShowOutPatientInvoiceInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet2)).EndInit();
            this.neuGroupBox1.ResumeLayout(false);
            this.neuGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbName;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbCardNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbGender;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbBirth;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbIDNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel6;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker neuBeginDate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel7;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker neuEndDate;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton tbQuery;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet2;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton neuButton1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbVacancy;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel8;
    }
}