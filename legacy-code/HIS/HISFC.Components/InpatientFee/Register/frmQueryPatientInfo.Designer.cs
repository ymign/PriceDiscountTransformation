namespace Neusoft.HISFC.Components.InpatientFee.Register
{
    partial class frmQueryPatientInfo
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.TipAppearance tipAppearance2 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.tcMain = new Neusoft.FrameWork.WinForms.Controls.NeuTabControl();
            this.tbpageName = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbSexF = new Neusoft.FrameWork.WinForms.Controls.NeuRadioButton();
            this.rbSexM = new Neusoft.FrameWork.WinForms.Controls.NeuRadioButton();
            this.rbDimness = new Neusoft.FrameWork.WinForms.Controls.NeuRadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbpagePatientNo = new System.Windows.Forms.TabPage();
            this.txtPatientNo = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.btSearch = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btNewSearch = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.btNoSelect = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btSelect = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btStop = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtSame = new System.Windows.Forms.RadioButton();
            this.rbtLike = new System.Windows.Forms.RadioButton();
            this.neuCheckBox2 = new Neusoft.FrameWork.WinForms.Controls.NeuCheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxOldSys = new Neusoft.FrameWork.WinForms.Controls.NeuCheckBox();
            this.cbxNewSys = new Neusoft.FrameWork.WinForms.Controls.NeuCheckBox();
            this.neuPanel2 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.spOldPatientInfo = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.sheetOldPatient = new FarPoint.Win.Spread.SheetView();
            this.spNewPatientInfo = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.sheetNewPatient = new FarPoint.Win.Spread.SheetView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblOldPatient = new System.Windows.Forms.Label();
            this.lblNewPatient = new System.Windows.Forms.Label();
            this.pnButton = new System.Windows.Forms.Panel();
            this.tcMain.SuspendLayout();
            this.tbpageName.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tbpagePatientNo.SuspendLayout();
            this.neuPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.neuPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spOldPatientInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetOldPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spNewPatientInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetNewPatient)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tbpageName);
            this.tcMain.Controls.Add(this.tbpagePatientNo);
            this.tcMain.Location = new System.Drawing.Point(25, 51);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(358, 146);
            this.tcMain.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tcMain.TabIndex = 0;
            // 
            // tbpageName
            // 
            this.tbpageName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(226)))), ((int)(((byte)(224)))));
            this.tbpageName.Controls.Add(this.groupBox3);
            this.tbpageName.Controls.Add(this.label2);
            this.tbpageName.Controls.Add(this.label1);
            this.tbpageName.Controls.Add(this.txtName);
            this.tbpageName.Controls.Add(this.neuLabel2);
            this.tbpageName.Controls.Add(this.neuLabel1);
            this.tbpageName.Location = new System.Drawing.Point(4, 21);
            this.tbpageName.Name = "tbpageName";
            this.tbpageName.Padding = new System.Windows.Forms.Padding(3);
            this.tbpageName.Size = new System.Drawing.Size(350, 121);
            this.tbpageName.TabIndex = 0;
            this.tbpageName.Text = "姓名和性别";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbSexF);
            this.groupBox3.Controls.Add(this.rbSexM);
            this.groupBox3.Controls.Add(this.rbDimness);
            this.groupBox3.Location = new System.Drawing.Point(109, 59);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(146, 29);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            // 
            // rbSexF
            // 
            this.rbSexF.AutoSize = true;
            this.rbSexF.Location = new System.Drawing.Point(50, 10);
            this.rbSexF.Name = "rbSexF";
            this.rbSexF.Size = new System.Drawing.Size(35, 16);
            this.rbSexF.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.rbSexF.TabIndex = 3;
            this.rbSexF.Text = "女";
            this.rbSexF.UseVisualStyleBackColor = true;
            // 
            // rbSexM
            // 
            this.rbSexM.AutoSize = true;
            this.rbSexM.Location = new System.Drawing.Point(9, 10);
            this.rbSexM.Name = "rbSexM";
            this.rbSexM.Size = new System.Drawing.Size(35, 16);
            this.rbSexM.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.rbSexM.TabIndex = 2;
            this.rbSexM.Text = "男";
            this.rbSexM.UseVisualStyleBackColor = true;
            // 
            // rbDimness
            // 
            this.rbDimness.AutoSize = true;
            this.rbDimness.Checked = true;
            this.rbDimness.Location = new System.Drawing.Point(91, 10);
            this.rbDimness.Name = "rbDimness";
            this.rbDimness.Size = new System.Drawing.Size(47, 16);
            this.rbDimness.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.rbDimness.TabIndex = 4;
            this.rbDimness.TabStop = true;
            this.rbDimness.Text = "不详";
            this.rbDimness.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(74, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "性别";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(74, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "姓名";
            // 
            // txtName
            // 
            this.txtName.IsEnter2Tab = false;
            this.txtName.Location = new System.Drawing.Point(109, 32);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(129, 21);
            this.txtName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtName.TabIndex = 5;
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Location = new System.Drawing.Point(90, 69);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(0, 12);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 1;
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Location = new System.Drawing.Point(90, 36);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(0, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 0;
            // 
            // tbpagePatientNo
            // 
            this.tbpagePatientNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(226)))), ((int)(((byte)(224)))));
            this.tbpagePatientNo.Controls.Add(this.txtPatientNo);
            this.tbpagePatientNo.Controls.Add(this.neuLabel3);
            this.tbpagePatientNo.Location = new System.Drawing.Point(4, 21);
            this.tbpagePatientNo.Name = "tbpagePatientNo";
            this.tbpagePatientNo.Padding = new System.Windows.Forms.Padding(3);
            this.tbpagePatientNo.Size = new System.Drawing.Size(350, 121);
            this.tbpagePatientNo.TabIndex = 1;
            this.tbpagePatientNo.Text = "住院号";
            // 
            // txtPatientNo
            // 
            this.txtPatientNo.BackColor = System.Drawing.Color.White;
            this.txtPatientNo.IsEnter2Tab = false;
            this.txtPatientNo.Location = new System.Drawing.Point(123, 45);
            this.txtPatientNo.Name = "txtPatientNo";
            this.txtPatientNo.Size = new System.Drawing.Size(100, 21);
            this.txtPatientNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtPatientNo.TabIndex = 1;
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Location = new System.Drawing.Point(76, 48);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(41, 12);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 0;
            this.neuLabel3.Text = "住院号";
            // 
            // btSearch
            // 
            this.btSearch.Location = new System.Drawing.Point(519, 17);
            this.btSearch.Name = "btSearch";
            this.btSearch.Size = new System.Drawing.Size(75, 23);
            this.btSearch.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btSearch.TabIndex = 8;
            this.btSearch.Text = "开始查找";
            this.btSearch.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btSearch.UseVisualStyleBackColor = true;
            this.btSearch.Click += new System.EventHandler(this.nbtSearch_Click);
            // 
            // btNewSearch
            // 
            this.btNewSearch.Location = new System.Drawing.Point(519, 103);
            this.btNewSearch.Name = "btNewSearch";
            this.btNewSearch.Size = new System.Drawing.Size(75, 23);
            this.btNewSearch.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btNewSearch.TabIndex = 10;
            this.btNewSearch.Text = "新查找";
            this.btNewSearch.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btNewSearch.UseVisualStyleBackColor = true;
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.btNoSelect);
            this.neuPanel1.Controls.Add(this.btSelect);
            this.neuPanel1.Controls.Add(this.btStop);
            this.neuPanel1.Controls.Add(this.groupBox2);
            this.neuPanel1.Controls.Add(this.groupBox1);
            this.neuPanel1.Controls.Add(this.tcMain);
            this.neuPanel1.Controls.Add(this.btNewSearch);
            this.neuPanel1.Controls.Add(this.btSearch);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuPanel1.Location = new System.Drawing.Point(0, 0);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(708, 201);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 12;
            // 
            // btNoSelect
            // 
            this.btNoSelect.AutoSize = true;
            this.btNoSelect.Location = new System.Drawing.Point(557, 160);
            this.btNoSelect.Name = "btNoSelect";
            this.btNoSelect.Size = new System.Drawing.Size(81, 23);
            this.btNoSelect.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btNoSelect.TabIndex = 18;
            this.btNoSelect.Text = "新病人,不选";
            this.btNoSelect.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btNoSelect.UseVisualStyleBackColor = true;
            this.btNoSelect.Click += new System.EventHandler(this.btNoSelect_Click);
            // 
            // btSelect
            // 
            this.btSelect.AutoSize = true;
            this.btSelect.Location = new System.Drawing.Point(447, 160);
            this.btSelect.Name = "btSelect";
            this.btSelect.Size = new System.Drawing.Size(75, 23);
            this.btSelect.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btSelect.TabIndex = 17;
            this.btSelect.Text = "选中";
            this.btSelect.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btSelect.UseVisualStyleBackColor = true;
            this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
            // 
            // btStop
            // 
            this.btStop.Location = new System.Drawing.Point(519, 60);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(75, 23);
            this.btStop.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btStop.TabIndex = 16;
            this.btStop.Text = "停止";
            this.btStop.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btStop.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbtSame);
            this.groupBox2.Controls.Add(this.rbtLike);
            this.groupBox2.Controls.Add(this.neuCheckBox2);
            this.groupBox2.Location = new System.Drawing.Point(286, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(97, 60);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            // 
            // rbtSame
            // 
            this.rbtSame.AutoSize = true;
            this.rbtSame.Checked = true;
            this.rbtSame.Location = new System.Drawing.Point(23, 38);
            this.rbtSame.Name = "rbtSame";
            this.rbtSame.Size = new System.Drawing.Size(47, 16);
            this.rbtSame.TabIndex = 15;
            this.rbtSame.TabStop = true;
            this.rbtSame.Text = "相等";
            this.rbtSame.UseVisualStyleBackColor = true;
            // 
            // rbtLike
            // 
            this.rbtLike.AutoSize = true;
            this.rbtLike.Location = new System.Drawing.Point(23, 13);
            this.rbtLike.Name = "rbtLike";
            this.rbtLike.Size = new System.Drawing.Size(47, 16);
            this.rbtLike.TabIndex = 14;
            this.rbtLike.Text = "相似";
            this.rbtLike.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.rbtLike.UseVisualStyleBackColor = true;
            // 
            // neuCheckBox2
            // 
            this.neuCheckBox2.AutoSize = true;
            this.neuCheckBox2.BackColor = System.Drawing.Color.Transparent;
            this.neuCheckBox2.Location = new System.Drawing.Point(123, 17);
            this.neuCheckBox2.Name = "neuCheckBox2";
            this.neuCheckBox2.Size = new System.Drawing.Size(60, 16);
            this.neuCheckBox2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuCheckBox2.TabIndex = 13;
            this.neuCheckBox2.Text = "旧系统";
            this.neuCheckBox2.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxOldSys);
            this.groupBox1.Controls.Add(this.cbxNewSys);
            this.groupBox1.Location = new System.Drawing.Point(25, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 43);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // cbxOldSys
            // 
            this.cbxOldSys.AutoSize = true;
            this.cbxOldSys.BackColor = System.Drawing.Color.Transparent;
            this.cbxOldSys.Checked = true;
            this.cbxOldSys.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxOldSys.Location = new System.Drawing.Point(123, 17);
            this.cbxOldSys.Name = "cbxOldSys";
            this.cbxOldSys.Size = new System.Drawing.Size(60, 16);
            this.cbxOldSys.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.cbxOldSys.TabIndex = 13;
            this.cbxOldSys.Text = "旧系统";
            this.cbxOldSys.UseVisualStyleBackColor = false;
            // 
            // cbxNewSys
            // 
            this.cbxNewSys.AutoSize = true;
            this.cbxNewSys.BackColor = System.Drawing.Color.Transparent;
            this.cbxNewSys.Checked = true;
            this.cbxNewSys.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxNewSys.Location = new System.Drawing.Point(23, 17);
            this.cbxNewSys.Name = "cbxNewSys";
            this.cbxNewSys.Size = new System.Drawing.Size(60, 16);
            this.cbxNewSys.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.cbxNewSys.TabIndex = 14;
            this.cbxNewSys.Text = "新系统";
            this.cbxNewSys.UseVisualStyleBackColor = false;
            // 
            // neuPanel2
            // 
            this.neuPanel2.Controls.Add(this.spOldPatientInfo);
            this.neuPanel2.Controls.Add(this.spNewPatientInfo);
            this.neuPanel2.Controls.Add(this.panel1);
            this.neuPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel2.Location = new System.Drawing.Point(0, 201);
            this.neuPanel2.Name = "neuPanel2";
            this.neuPanel2.Size = new System.Drawing.Size(708, 165);
            this.neuPanel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel2.TabIndex = 13;
            // 
            // spOldPatientInfo
            // 
            this.spOldPatientInfo.About = "3.0.2004.2005";
            this.spOldPatientInfo.AccessibleDescription = "spOldPatientInfo, Sheet1";
            this.spOldPatientInfo.BackColor = System.Drawing.Color.White;
            this.spOldPatientInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spOldPatientInfo.FileName = "";
            this.spOldPatientInfo.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.spOldPatientInfo.IsAutoSaveGridStatus = false;
            this.spOldPatientInfo.IsCanCustomConfigColumn = false;
            this.spOldPatientInfo.Location = new System.Drawing.Point(379, 19);
            this.spOldPatientInfo.Name = "spOldPatientInfo";
            this.spOldPatientInfo.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.sheetOldPatient});
            this.spOldPatientInfo.Size = new System.Drawing.Size(329, 146);
            this.spOldPatientInfo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.spOldPatientInfo.TabIndex = 1;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.spOldPatientInfo.TextTipAppearance = tipAppearance1;
            this.spOldPatientInfo.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.spOldPatientInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.spOldPatientInfo_MouseDown);
            this.spOldPatientInfo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spOldPatientInfo_KeyDown);
            // 
            // sheetOldPatient
            // 
            this.sheetOldPatient.Reset();
            this.sheetOldPatient.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.sheetOldPatient.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.sheetOldPatient.ColumnCount = 16;
            this.sheetOldPatient.RowCount = 0;
            this.sheetOldPatient.RowHeader.ColumnCount = 0;
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 0).Value = "床号";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 1).Value = "姓名";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 2).Value = "性别";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 3).Value = "住院号";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 4).Value = "出生日期";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 5).Value = "当前病区";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 6).Value = "标志";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 7).Value = "单位及地址";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 8).Value = "出生地址";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 9).Value = "工作单位";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 10).Value = "入院日期";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 11).Value = "出院日期";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 12).Value = "身份证号";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 13).Value = "联系人";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 14).Value = "住院次数";
            this.sheetOldPatient.ColumnHeader.Cells.Get(0, 15).Value = "记账号";
            this.sheetOldPatient.Columns.Get(0).AllowAutoSort = true;
            this.sheetOldPatient.Columns.Get(0).Label = "床号";
            this.sheetOldPatient.Columns.Get(0).Locked = true;
            this.sheetOldPatient.Columns.Get(0).Visible = false;
            this.sheetOldPatient.Columns.Get(0).Width = 54F;
            this.sheetOldPatient.Columns.Get(1).AllowAutoSort = true;
            this.sheetOldPatient.Columns.Get(1).Label = "姓名";
            this.sheetOldPatient.Columns.Get(1).Locked = true;
            this.sheetOldPatient.Columns.Get(1).Width = 55F;
            this.sheetOldPatient.Columns.Get(2).AllowAutoSort = true;
            this.sheetOldPatient.Columns.Get(2).Label = "性别";
            this.sheetOldPatient.Columns.Get(2).Locked = true;
            this.sheetOldPatient.Columns.Get(2).Width = 37F;
            this.sheetOldPatient.Columns.Get(3).AllowAutoSort = true;
            this.sheetOldPatient.Columns.Get(3).Label = "住院号";
            this.sheetOldPatient.Columns.Get(3).Locked = true;
            this.sheetOldPatient.Columns.Get(3).Width = 67F;
            this.sheetOldPatient.Columns.Get(4).Label = "出生日期";
            this.sheetOldPatient.Columns.Get(4).Locked = true;
            this.sheetOldPatient.Columns.Get(4).Width = 63F;
            this.sheetOldPatient.Columns.Get(5).AllowAutoSort = true;
            this.sheetOldPatient.Columns.Get(5).Label = "当前病区";
            this.sheetOldPatient.Columns.Get(5).Locked = true;
            this.sheetOldPatient.Columns.Get(5).Width = 80F;
            this.sheetOldPatient.Columns.Get(6).Label = "标志";
            this.sheetOldPatient.Columns.Get(6).Locked = true;
            this.sheetOldPatient.Columns.Get(6).Width = 39F;
            this.sheetOldPatient.Columns.Get(7).Label = "单位及地址";
            this.sheetOldPatient.Columns.Get(7).Locked = true;
            this.sheetOldPatient.Columns.Get(7).Width = 150F;
            this.sheetOldPatient.Columns.Get(8).Label = "出生地址";
            this.sheetOldPatient.Columns.Get(8).Locked = true;
            this.sheetOldPatient.Columns.Get(8).Width = 120F;
            this.sheetOldPatient.Columns.Get(9).Label = "工作单位";
            this.sheetOldPatient.Columns.Get(9).Locked = true;
            this.sheetOldPatient.Columns.Get(9).Width = 150F;
            this.sheetOldPatient.Columns.Get(10).AllowAutoSort = true;
            this.sheetOldPatient.Columns.Get(10).Label = "入院日期";
            this.sheetOldPatient.Columns.Get(10).Locked = true;
            this.sheetOldPatient.Columns.Get(10).Width = 81F;
            this.sheetOldPatient.Columns.Get(11).AllowAutoSort = true;
            this.sheetOldPatient.Columns.Get(11).Label = "出院日期";
            this.sheetOldPatient.Columns.Get(11).Locked = true;
            this.sheetOldPatient.Columns.Get(11).Width = 89F;
            this.sheetOldPatient.Columns.Get(12).CellType = textCellType1;
            this.sheetOldPatient.Columns.Get(12).Label = "身份证号";
            this.sheetOldPatient.Columns.Get(12).Locked = true;
            this.sheetOldPatient.Columns.Get(12).Width = 130F;
            this.sheetOldPatient.Columns.Get(13).Label = "联系人";
            this.sheetOldPatient.Columns.Get(13).Locked = true;
            this.sheetOldPatient.Columns.Get(14).AllowAutoSort = true;
            this.sheetOldPatient.Columns.Get(14).Label = "住院次数";
            this.sheetOldPatient.Columns.Get(14).Locked = true;
            this.sheetOldPatient.Columns.Get(14).Width = 79F;
            this.sheetOldPatient.Columns.Get(15).Label = "记账号";
            this.sheetOldPatient.Columns.Get(15).Visible = false;
            this.sheetOldPatient.GrayAreaBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(226)))), ((int)(((byte)(224)))));
            this.sheetOldPatient.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.sheetOldPatient.RowHeader.Columns.Default.Resizable = false;
            this.sheetOldPatient.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.sheetOldPatient.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.sheetOldPatient.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.spOldPatientInfo.SetActiveViewport(0, 1, 0);
            // 
            // spNewPatientInfo
            // 
            this.spNewPatientInfo.About = "3.0.2004.2005";
            this.spNewPatientInfo.AccessibleDescription = "spNewPatientInfo, Sheet1";
            this.spNewPatientInfo.BackColor = System.Drawing.Color.White;
            this.spNewPatientInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.spNewPatientInfo.FileName = "";
            this.spNewPatientInfo.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.spNewPatientInfo.IsAutoSaveGridStatus = false;
            this.spNewPatientInfo.IsCanCustomConfigColumn = false;
            this.spNewPatientInfo.Location = new System.Drawing.Point(0, 19);
            this.spNewPatientInfo.Name = "spNewPatientInfo";
            this.spNewPatientInfo.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.sheetNewPatient});
            this.spNewPatientInfo.Size = new System.Drawing.Size(379, 146);
            this.spNewPatientInfo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.spNewPatientInfo.TabIndex = 0;
            tipAppearance2.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.spNewPatientInfo.TextTipAppearance = tipAppearance2;
            this.spNewPatientInfo.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.spNewPatientInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.spNewPatientInfo_MouseDown);
            this.spNewPatientInfo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spNewPatientInfo_KeyDown);
            // 
            // sheetNewPatient
            // 
            this.sheetNewPatient.Reset();
            this.sheetNewPatient.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.sheetNewPatient.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.sheetNewPatient.ColumnCount = 16;
            this.sheetNewPatient.RowCount = 0;
            this.sheetNewPatient.RowHeader.ColumnCount = 0;
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 0).Value = "床号";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 1).Value = "姓名";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 2).Value = "性别";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 3).Value = "住院号";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 4).Value = "出生日期";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 5).Value = "当前病区";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 6).Value = "标志";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 7).Value = "单位及地址";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 8).Value = "出生地址";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 9).Value = "工作单位";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 10).Value = "入院日期";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 11).Value = "出院日期";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 12).Value = "身份证号";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 13).Value = "联系人";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 14).Value = "住院次数";
            this.sheetNewPatient.ColumnHeader.Cells.Get(0, 15).Value = "记账号";
            this.sheetNewPatient.Columns.Get(0).AllowAutoSort = true;
            this.sheetNewPatient.Columns.Get(0).Label = "床号";
            this.sheetNewPatient.Columns.Get(0).Visible = false;
            this.sheetNewPatient.Columns.Get(0).Width = 54F;
            this.sheetNewPatient.Columns.Get(1).AllowAutoSort = true;
            this.sheetNewPatient.Columns.Get(1).Label = "姓名";
            this.sheetNewPatient.Columns.Get(1).Locked = true;
            this.sheetNewPatient.Columns.Get(1).Width = 51F;
            this.sheetNewPatient.Columns.Get(2).AllowAutoSort = true;
            this.sheetNewPatient.Columns.Get(2).Label = "性别";
            this.sheetNewPatient.Columns.Get(2).Locked = true;
            this.sheetNewPatient.Columns.Get(2).Width = 38F;
            this.sheetNewPatient.Columns.Get(3).AllowAutoSort = true;
            this.sheetNewPatient.Columns.Get(3).Label = "住院号";
            this.sheetNewPatient.Columns.Get(3).Locked = true;
            this.sheetNewPatient.Columns.Get(3).Width = 61F;
            this.sheetNewPatient.Columns.Get(4).Label = "出生日期";
            this.sheetNewPatient.Columns.Get(4).Locked = true;
            this.sheetNewPatient.Columns.Get(4).Width = 63F;
            this.sheetNewPatient.Columns.Get(5).AllowAutoSort = true;
            this.sheetNewPatient.Columns.Get(5).Label = "当前病区";
            this.sheetNewPatient.Columns.Get(5).Locked = true;
            this.sheetNewPatient.Columns.Get(5).Width = 79F;
            this.sheetNewPatient.Columns.Get(6).Label = "标志";
            this.sheetNewPatient.Columns.Get(6).Locked = true;
            this.sheetNewPatient.Columns.Get(6).Width = 39F;
            this.sheetNewPatient.Columns.Get(7).Label = "单位及地址";
            this.sheetNewPatient.Columns.Get(7).Locked = true;
            this.sheetNewPatient.Columns.Get(7).Width = 150F;
            this.sheetNewPatient.Columns.Get(8).Label = "出生地址";
            this.sheetNewPatient.Columns.Get(8).Locked = true;
            this.sheetNewPatient.Columns.Get(8).Width = 120F;
            this.sheetNewPatient.Columns.Get(9).Label = "工作单位";
            this.sheetNewPatient.Columns.Get(9).Locked = true;
            this.sheetNewPatient.Columns.Get(9).Width = 150F;
            this.sheetNewPatient.Columns.Get(10).AllowAutoSort = true;
            this.sheetNewPatient.Columns.Get(10).Label = "入院日期";
            this.sheetNewPatient.Columns.Get(10).Locked = true;
            this.sheetNewPatient.Columns.Get(10).Width = 89F;
            this.sheetNewPatient.Columns.Get(11).AllowAutoSort = true;
            this.sheetNewPatient.Columns.Get(11).Label = "出院日期";
            this.sheetNewPatient.Columns.Get(11).Locked = true;
            this.sheetNewPatient.Columns.Get(11).Width = 81F;
            this.sheetNewPatient.Columns.Get(12).CellType = textCellType2;
            this.sheetNewPatient.Columns.Get(12).Label = "身份证号";
            this.sheetNewPatient.Columns.Get(12).Locked = true;
            this.sheetNewPatient.Columns.Get(12).Width = 127F;
            this.sheetNewPatient.Columns.Get(13).Label = "联系人";
            this.sheetNewPatient.Columns.Get(13).Locked = true;
            this.sheetNewPatient.Columns.Get(14).AllowAutoSort = true;
            this.sheetNewPatient.Columns.Get(14).Label = "住院次数";
            this.sheetNewPatient.Columns.Get(14).Locked = true;
            this.sheetNewPatient.Columns.Get(14).Width = 79F;
            this.sheetNewPatient.Columns.Get(15).Label = "记账号";
            this.sheetNewPatient.Columns.Get(15).Locked = true;
            this.sheetNewPatient.Columns.Get(15).Visible = false;
            this.sheetNewPatient.GrayAreaBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(226)))), ((int)(((byte)(224)))));
            this.sheetNewPatient.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.sheetNewPatient.RowHeader.Columns.Default.Resizable = false;
            this.sheetNewPatient.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.sheetNewPatient.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.sheetNewPatient.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.spNewPatientInfo.SetActiveViewport(0, 1, 0);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblOldPatient);
            this.panel1.Controls.Add(this.lblNewPatient);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(708, 19);
            this.panel1.TabIndex = 2;
            // 
            // lblOldPatient
            // 
            this.lblOldPatient.AutoSize = true;
            this.lblOldPatient.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOldPatient.ForeColor = System.Drawing.Color.Red;
            this.lblOldPatient.Location = new System.Drawing.Point(392, 3);
            this.lblOldPatient.Name = "lblOldPatient";
            this.lblOldPatient.Size = new System.Drawing.Size(84, 12);
            this.lblOldPatient.TabIndex = 1;
            this.lblOldPatient.Text = "旧系统读取:0";
            // 
            // lblNewPatient
            // 
            this.lblNewPatient.AutoSize = true;
            this.lblNewPatient.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblNewPatient.ForeColor = System.Drawing.Color.Red;
            this.lblNewPatient.Location = new System.Drawing.Point(15, 3);
            this.lblNewPatient.Name = "lblNewPatient";
            this.lblNewPatient.Size = new System.Drawing.Size(84, 12);
            this.lblNewPatient.TabIndex = 0;
            this.lblNewPatient.Text = "新系统读取:0";
            // 
            // pnButton
            // 
            this.pnButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnButton.Location = new System.Drawing.Point(0, 366);
            this.pnButton.Name = "pnButton";
            this.pnButton.Size = new System.Drawing.Size(708, 28);
            this.pnButton.TabIndex = 14;
            // 
            // frmQueryPatientInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 394);
            this.Controls.Add(this.neuPanel2);
            this.Controls.Add(this.pnButton);
            this.Controls.Add(this.neuPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmQueryPatientInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "病案查找";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmQueryPatientInfo_KeyDown);
            this.tcMain.ResumeLayout(false);
            this.tbpageName.ResumeLayout(false);
            this.tbpageName.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tbpagePatientNo.ResumeLayout(false);
            this.tbpagePatientNo.PerformLayout();
            this.neuPanel1.ResumeLayout(false);
            this.neuPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.neuPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spOldPatientInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetOldPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spNewPatientInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetNewPatient)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuTabControl tcMain;
        private System.Windows.Forms.TabPage tbpagePatientNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtPatientNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btSearch;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btNewSearch;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread spNewPatientInfo;
        private FarPoint.Win.Spread.SheetView sheetNewPatient;
        private System.Windows.Forms.TabPage tbpageName;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtName;
        private Neusoft.FrameWork.WinForms.Controls.NeuRadioButton rbDimness;
        private Neusoft.FrameWork.WinForms.Controls.NeuRadioButton rbSexF;
        private Neusoft.FrameWork.WinForms.Controls.NeuRadioButton rbSexM;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuCheckBox cbxOldSys;
        private Neusoft.FrameWork.WinForms.Controls.NeuCheckBox cbxNewSys;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btNoSelect;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btSelect;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btStop;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbtSame;
        private System.Windows.Forms.RadioButton rbtLike;
        private Neusoft.FrameWork.WinForms.Controls.NeuCheckBox neuCheckBox2;
        private System.Windows.Forms.Panel pnButton;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread spOldPatientInfo;
        private FarPoint.Win.Spread.SheetView sheetOldPatient;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblOldPatient;
        private System.Windows.Forms.Label lblNewPatient;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}