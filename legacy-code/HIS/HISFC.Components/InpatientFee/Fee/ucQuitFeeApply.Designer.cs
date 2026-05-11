namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    /// <summary>
    /// 
    /// </summary>
    partial class ucQuitFeeApply
    {

        /// <summary> 
        /// 清理所有正在使用的资源。

        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                this.dtDrug.Rows.Clear();
                this.dtDrug.Dispose();
                this.dtUndrug.Rows.Clear();
                this.dtUndrug.Dispose();
                this.dtMate.Rows.Clear();
                this.dtMate.Dispose();
                components.Dispose();

            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。

        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.TipAppearance tipAppearance2 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType3 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType4 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType5 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType6 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType7 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType8 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType2 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType9 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType10 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType11 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType12 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType13 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType14 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType15 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType16 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType3 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType4 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            this.gbTop = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.neuTxtInDate = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtBed = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblBed = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtDept = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblDept = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtPact = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblPact = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.ucQueryPatientInfo = new Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.neuPanel3 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.fpUnQuit = new FarPoint.Win.Spread.FpSpread();
            this.fpUnQuit_SheetDrug = new FarPoint.Win.Spread.SheetView();
            this.fpUnQuit_SheetUndrug = new FarPoint.Win.Spread.SheetView();
            this.neuSplitter1 = new Neusoft.FrameWork.WinForms.Controls.NeuSplitter();
            this.neuPanel4 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.fpQuit = new FarPoint.Win.Spread.FpSpread();
            this.fpQuit_SheetDrug = new FarPoint.Win.Spread.SheetView();
            this.fpQuit_SheetUndrug = new FarPoint.Win.Spread.SheetView();
            this.gbQuitItem = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.ckbAllQuit = new Neusoft.FrameWork.WinForms.Controls.NeuCheckBox();
            this.txtUnit = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.mtbQty = new Neusoft.FrameWork.WinForms.Controls.NeuMaskedTextBox();
            this.lblQty = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtPrice = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblPrice = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtItemName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblItemName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuPanel2 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.cmbQuitFeeReason = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtFilter = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.dtpEndTime = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.lblFeeTime = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.dtpBeginTime = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.gbTop.SuspendLayout();
            this.neuPanel1.SuspendLayout();
            this.neuPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpUnQuit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpUnQuit_SheetDrug)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpUnQuit_SheetUndrug)).BeginInit();
            this.neuPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpQuit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpQuit_SheetDrug)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpQuit_SheetUndrug)).BeginInit();
            this.gbQuitItem.SuspendLayout();
            this.neuPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTop
            // 
            this.gbTop.Controls.Add(this.neuTxtInDate);
            this.gbTop.Controls.Add(this.neuLabel3);
            this.gbTop.Controls.Add(this.txtBed);
            this.gbTop.Controls.Add(this.lblBed);
            this.gbTop.Controls.Add(this.txtDept);
            this.gbTop.Controls.Add(this.lblDept);
            this.gbTop.Controls.Add(this.txtPact);
            this.gbTop.Controls.Add(this.lblPact);
            this.gbTop.Controls.Add(this.txtName);
            this.gbTop.Controls.Add(this.lblName);
            this.gbTop.Controls.Add(this.ucQueryPatientInfo);
            this.gbTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbTop.ForeColor = System.Drawing.Color.Black;
            this.gbTop.Location = new System.Drawing.Point(0, 0);
            this.gbTop.Name = "gbTop";
            this.gbTop.Size = new System.Drawing.Size(917, 40);
            this.gbTop.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.gbTop.TabIndex = 0;
            this.gbTop.TabStop = false;
            // 
            // neuTxtInDate
            // 
            this.neuTxtInDate.BackColor = System.Drawing.Color.White;
            this.neuTxtInDate.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuTxtInDate.IsEnter2Tab = false;
            this.neuTxtInDate.Location = new System.Drawing.Point(655, 12);
            this.neuTxtInDate.Name = "neuTxtInDate";
            this.neuTxtInDate.ReadOnly = true;
            this.neuTxtInDate.Size = new System.Drawing.Size(130, 22);
            this.neuTxtInDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuTxtInDate.TabIndex = 10;
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel3.Location = new System.Drawing.Point(592, 16);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(66, 13);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 9;
            this.neuLabel3.Text = "入院日期:";
            // 
            // txtBed
            // 
            this.txtBed.BackColor = System.Drawing.Color.White;
            this.txtBed.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtBed.IsEnter2Tab = false;
            this.txtBed.Location = new System.Drawing.Point(831, 13);
            this.txtBed.Name = "txtBed";
            this.txtBed.ReadOnly = true;
            this.txtBed.Size = new System.Drawing.Size(83, 22);
            this.txtBed.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtBed.TabIndex = 8;
            // 
            // lblBed
            // 
            this.lblBed.AutoSize = true;
            this.lblBed.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblBed.Location = new System.Drawing.Point(791, 17);
            this.lblBed.Name = "lblBed";
            this.lblBed.Size = new System.Drawing.Size(40, 13);
            this.lblBed.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblBed.TabIndex = 7;
            this.lblBed.Text = "床号:";
            // 
            // txtDept
            // 
            this.txtDept.BackColor = System.Drawing.Color.White;
            this.txtDept.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtDept.IsEnter2Tab = false;
            this.txtDept.Location = new System.Drawing.Point(498, 12);
            this.txtDept.Name = "txtDept";
            this.txtDept.ReadOnly = true;
            this.txtDept.Size = new System.Drawing.Size(88, 22);
            this.txtDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtDept.TabIndex = 6;
            // 
            // lblDept
            // 
            this.lblDept.AutoSize = true;
            this.lblDept.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDept.Location = new System.Drawing.Point(436, 17);
            this.lblDept.Name = "lblDept";
            this.lblDept.Size = new System.Drawing.Size(66, 13);
            this.lblDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblDept.TabIndex = 5;
            this.lblDept.Text = "住院科室:";
            // 
            // txtPact
            // 
            this.txtPact.BackColor = System.Drawing.Color.White;
            this.txtPact.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPact.IsEnter2Tab = false;
            this.txtPact.Location = new System.Drawing.Point(339, 13);
            this.txtPact.Name = "txtPact";
            this.txtPact.ReadOnly = true;
            this.txtPact.Size = new System.Drawing.Size(91, 22);
            this.txtPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtPact.TabIndex = 4;
            // 
            // lblPact
            // 
            this.lblPact.AutoSize = true;
            this.lblPact.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPact.Location = new System.Drawing.Point(278, 17);
            this.lblPact.Name = "lblPact";
            this.lblPact.Size = new System.Drawing.Size(66, 13);
            this.lblPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblPact.TabIndex = 3;
            this.lblPact.Text = "合同单位:";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.White;
            this.txtName.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtName.IsEnter2Tab = false;
            this.txtName.Location = new System.Drawing.Point(201, 12);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(71, 22);
            this.txtName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtName.TabIndex = 2;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblName.Location = new System.Drawing.Point(166, 16);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(40, 13);
            this.lblName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblName.TabIndex = 1;
            this.lblName.Text = "姓名:";
            // 
            // ucQueryPatientInfo
            // 
            this.ucQueryPatientInfo.DefaultInputType = 0;
            this.ucQueryPatientInfo.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ucQueryPatientInfo.InputType = 0;
            this.ucQueryPatientInfo.IsDeptOnly = true;
            this.ucQueryPatientInfo.Location = new System.Drawing.Point(6, 8);
            this.ucQueryPatientInfo.Name = "ucQueryPatientInfo";
            this.ucQueryPatientInfo.PatientInState = "ALL";
            this.ucQueryPatientInfo.ShowState = Neusoft.HISFC.Components.Common.Controls.enuShowState.All;
            this.ucQueryPatientInfo.Size = new System.Drawing.Size(154, 27);
            this.ucQueryPatientInfo.TabIndex = 0;
            this.ucQueryPatientInfo.myEvent += new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(this.ucQueryInpatientNO_myEvent);
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.neuPanel3);
            this.neuPanel1.Controls.Add(this.neuPanel2);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel1.ForeColor = System.Drawing.Color.Black;
            this.neuPanel1.Location = new System.Drawing.Point(0, 40);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(917, 567);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 1;
            // 
            // neuPanel3
            // 
            this.neuPanel3.Controls.Add(this.fpUnQuit);
            this.neuPanel3.Controls.Add(this.neuSplitter1);
            this.neuPanel3.Controls.Add(this.neuPanel4);
            this.neuPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel3.Location = new System.Drawing.Point(0, 40);
            this.neuPanel3.Name = "neuPanel3";
            this.neuPanel3.Size = new System.Drawing.Size(917, 527);
            this.neuPanel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel3.TabIndex = 1;
            // 
            // fpUnQuit
            // 
            this.fpUnQuit.About = "3.0.2004.2005";
            this.fpUnQuit.AccessibleDescription = "fpUnQuit, 非药品(F4), Row 0, Column 0, ";
            this.fpUnQuit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(247)))), ((int)(((byte)(213)))));
            this.fpUnQuit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpUnQuit.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpUnQuit.Location = new System.Drawing.Point(0, 0);
            this.fpUnQuit.Name = "fpUnQuit";
            this.fpUnQuit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpUnQuit.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpUnQuit_SheetDrug,
            this.fpUnQuit_SheetUndrug});
            this.fpUnQuit.Size = new System.Drawing.Size(917, 231);
            this.fpUnQuit.TabIndex = 0;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpUnQuit.TextTipAppearance = tipAppearance1;
            this.fpUnQuit.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpUnQuit.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpUnQuit_ButtonClicked);
            this.fpUnQuit.ActiveSheetChanged += new System.EventHandler(this.fpUnQuit_ActiveSheetChanged);
            this.fpUnQuit.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpUnQuit_CellDoubleClick);
            this.fpUnQuit.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpUnQuit_CellClick);
            this.fpUnQuit.ColumnWidthChanged += new FarPoint.Win.Spread.ColumnWidthChangedEventHandler(this.fpUnQuit_ColumnWidthChanged);
            this.fpUnQuit.ActiveSheetIndex = 1;
            // 
            // fpUnQuit_SheetDrug
            // 
            this.fpUnQuit_SheetDrug.Reset();
            this.fpUnQuit_SheetDrug.SheetName = "药品(F3)";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpUnQuit_SheetDrug.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpUnQuit_SheetDrug.ColumnCount = 10;
            this.fpUnQuit_SheetDrug.RowCount = 10;
            this.fpUnQuit_SheetDrug.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpUnQuit_SheetDrug.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpUnQuit_SheetDrug.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpUnQuit_SheetDrug.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpUnQuit_SheetDrug.RowHeader.Columns.Default.Resizable = true;
            this.fpUnQuit_SheetDrug.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpUnQuit_SheetDrug.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpUnQuit_SheetDrug.Rows.Default.Height = 25F;
            this.fpUnQuit_SheetDrug.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpUnQuit_SheetDrug.SheetCornerStyle.Parent = "CornerDefault";
            this.fpUnQuit_SheetDrug.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.fpUnQuit_SheetDrug.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // fpUnQuit_SheetUndrug
            // 
            this.fpUnQuit_SheetUndrug.Reset();
            this.fpUnQuit_SheetUndrug.SheetName = "非药品(F4)";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpUnQuit_SheetUndrug.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpUnQuit_SheetUndrug.ColumnCount = 10;
            this.fpUnQuit_SheetUndrug.RowCount = 10;
            this.fpUnQuit_SheetUndrug.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpUnQuit_SheetUndrug.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpUnQuit_SheetUndrug.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpUnQuit_SheetUndrug.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpUnQuit_SheetUndrug.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.fpUnQuit_SheetUndrug.RowHeader.Columns.Default.Resizable = true;
            this.fpUnQuit_SheetUndrug.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpUnQuit_SheetUndrug.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpUnQuit_SheetUndrug.Rows.Default.Height = 25F;
            this.fpUnQuit_SheetUndrug.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.fpUnQuit_SheetUndrug.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.fpUnQuit_SheetUndrug.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpUnQuit_SheetUndrug.SheetCornerStyle.Locked = false;
            this.fpUnQuit_SheetUndrug.SheetCornerStyle.Parent = "HeaderDefault";
            this.fpUnQuit_SheetUndrug.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.fpUnQuit_SheetUndrug.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // neuSplitter1
            // 
            this.neuSplitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.neuSplitter1.Location = new System.Drawing.Point(0, 231);
            this.neuSplitter1.Name = "neuSplitter1";
            this.neuSplitter1.Size = new System.Drawing.Size(917, 5);
            this.neuSplitter1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSplitter1.TabIndex = 2;
            this.neuSplitter1.TabStop = false;
            // 
            // neuPanel4
            // 
            this.neuPanel4.Controls.Add(this.fpQuit);
            this.neuPanel4.Controls.Add(this.gbQuitItem);
            this.neuPanel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.neuPanel4.Location = new System.Drawing.Point(0, 236);
            this.neuPanel4.Name = "neuPanel4";
            this.neuPanel4.Padding = new System.Windows.Forms.Padding(1);
            this.neuPanel4.Size = new System.Drawing.Size(917, 291);
            this.neuPanel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel4.TabIndex = 1;
            // 
            // fpQuit
            // 
            this.fpQuit.About = "3.0.2004.2005";
            this.fpQuit.AccessibleDescription = "fpQuit, 已退非药品";
            this.fpQuit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(247)))), ((int)(((byte)(213)))));
            this.fpQuit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpQuit.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpQuit.Location = new System.Drawing.Point(1, 43);
            this.fpQuit.Name = "fpQuit";
            this.fpQuit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpQuit.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpQuit_SheetDrug,
            this.fpQuit_SheetUndrug});
            this.fpQuit.Size = new System.Drawing.Size(915, 247);
            this.fpQuit.TabIndex = 1;
            tipAppearance2.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpQuit.TextTipAppearance = tipAppearance2;
            this.fpQuit.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpQuit.ActiveSheetChanged += new System.EventHandler(this.fpQuit_ActiveSheetChanged);
            this.fpQuit.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpQuit_CellDoubleClick);
            this.fpQuit.ActiveSheetIndex = 1;
            // 
            // fpQuit_SheetDrug
            // 
            this.fpQuit_SheetDrug.Reset();
            this.fpQuit_SheetDrug.SheetName = "已退药品";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpQuit_SheetDrug.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpQuit_SheetDrug.ColumnCount = 13;
            this.fpQuit_SheetDrug.RowCount = 0;
            this.fpQuit_SheetDrug.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 0).Value = "退药原因";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 1).Value = "药品名称";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 2).Value = "规格";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 3).Value = "价格";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 4).Value = "退费数量";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 5).Value = "单位";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 6).Value = "金额";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 7).Value = "记帐日期";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 8).Value = "是否发药";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 9).Value = "是否退费申请";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 10).Value = "处方号";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 11).Value = "处方内部流水号";
            this.fpQuit_SheetDrug.ColumnHeader.Cells.Get(0, 12).Value = "应执行时间";
            this.fpQuit_SheetDrug.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpQuit_SheetDrug.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpQuit_SheetDrug.ColumnHeader.Rows.Get(0).Height = 30F;
            textCellType1.WordWrap = true;
            this.fpQuit_SheetDrug.Columns.Get(0).CellType = textCellType1;
            this.fpQuit_SheetDrug.Columns.Get(0).Label = "退药原因";
            this.fpQuit_SheetDrug.Columns.Get(0).Width = 113F;
            this.fpQuit_SheetDrug.Columns.Get(1).CellType = textCellType2;
            this.fpQuit_SheetDrug.Columns.Get(1).Label = "药品名称";
            this.fpQuit_SheetDrug.Columns.Get(1).Width = 212F;
            this.fpQuit_SheetDrug.Columns.Get(2).CellType = textCellType3;
            this.fpQuit_SheetDrug.Columns.Get(2).Label = "规格";
            this.fpQuit_SheetDrug.Columns.Get(2).Width = 127F;
            this.fpQuit_SheetDrug.Columns.Get(3).CellType = textCellType4;
            this.fpQuit_SheetDrug.Columns.Get(3).Label = "价格";
            this.fpQuit_SheetDrug.Columns.Get(4).CellType = textCellType5;
            this.fpQuit_SheetDrug.Columns.Get(4).Label = "退费数量";
            this.fpQuit_SheetDrug.Columns.Get(5).CellType = textCellType6;
            this.fpQuit_SheetDrug.Columns.Get(5).Label = "单位";
            this.fpQuit_SheetDrug.Columns.Get(6).CellType = textCellType7;
            this.fpQuit_SheetDrug.Columns.Get(6).Label = "金额";
            this.fpQuit_SheetDrug.Columns.Get(7).CellType = textCellType8;
            this.fpQuit_SheetDrug.Columns.Get(7).Label = "记帐日期";
            this.fpQuit_SheetDrug.Columns.Get(7).Width = 152F;
            this.fpQuit_SheetDrug.Columns.Get(8).CellType = checkBoxCellType1;
            this.fpQuit_SheetDrug.Columns.Get(8).Label = "是否发药";
            this.fpQuit_SheetDrug.Columns.Get(9).CellType = checkBoxCellType2;
            this.fpQuit_SheetDrug.Columns.Get(9).Label = "是否退费申请";
            this.fpQuit_SheetDrug.Columns.Get(9).Width = 85F;
            this.fpQuit_SheetDrug.Columns.Get(10).CellType = textCellType9;
            this.fpQuit_SheetDrug.Columns.Get(10).Label = "处方号";
            this.fpQuit_SheetDrug.Columns.Get(10).Width = 95F;
            this.fpQuit_SheetDrug.Columns.Get(11).Label = "处方内部流水号";
            this.fpQuit_SheetDrug.Columns.Get(11).Width = 92F;
            this.fpQuit_SheetDrug.Columns.Get(12).Label = "应执行时间";
            this.fpQuit_SheetDrug.Columns.Get(12).Width = 120F;
            this.fpQuit_SheetDrug.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.fpQuit_SheetDrug.RowHeader.Columns.Default.Resizable = true;
            this.fpQuit_SheetDrug.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpQuit_SheetDrug.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpQuit_SheetDrug.Rows.Default.Height = 25F;
            this.fpQuit_SheetDrug.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.fpQuit_SheetDrug.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.fpQuit_SheetDrug.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpQuit_SheetDrug.SheetCornerStyle.Locked = false;
            this.fpQuit_SheetDrug.SheetCornerStyle.Parent = "HeaderDefault";
            this.fpQuit_SheetDrug.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.fpQuit_SheetDrug.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.fpQuit.SetActiveViewport(0, 1, 0);
            // 
            // fpQuit_SheetUndrug
            // 
            this.fpQuit_SheetUndrug.Reset();
            this.fpQuit_SheetUndrug.SheetName = "已退非药品";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpQuit_SheetUndrug.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpQuit_SheetUndrug.ColumnCount = 13;
            this.fpQuit_SheetUndrug.RowCount = 0;
            this.fpQuit_SheetUndrug.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 0).Value = "项目名称";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 1).Value = "费用名称";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 2).Value = "价格";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 3).Value = "退费数量";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 4).Value = "单位";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 5).Value = "金额";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 6).Value = "执行科室";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 7).Value = "是否执行";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 8).Value = "是否退费申请";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 9).Value = "处方号";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 10).Value = "处方内部流水号";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 11).Value = "应执行时间";
            this.fpQuit_SheetUndrug.ColumnHeader.Cells.Get(0, 12).Value = "条码";
            this.fpQuit_SheetUndrug.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpQuit_SheetUndrug.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpQuit_SheetUndrug.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpQuit_SheetUndrug.Columns.Get(0).CellType = textCellType10;
            this.fpQuit_SheetUndrug.Columns.Get(0).Label = "项目名称";
            this.fpQuit_SheetUndrug.Columns.Get(0).Width = 214F;
            this.fpQuit_SheetUndrug.Columns.Get(1).CellType = textCellType11;
            this.fpQuit_SheetUndrug.Columns.Get(1).Label = "费用名称";
            this.fpQuit_SheetUndrug.Columns.Get(2).CellType = textCellType12;
            this.fpQuit_SheetUndrug.Columns.Get(2).Label = "价格";
            this.fpQuit_SheetUndrug.Columns.Get(3).CellType = textCellType13;
            this.fpQuit_SheetUndrug.Columns.Get(3).Label = "退费数量";
            this.fpQuit_SheetUndrug.Columns.Get(4).CellType = textCellType14;
            this.fpQuit_SheetUndrug.Columns.Get(4).Label = "单位";
            this.fpQuit_SheetUndrug.Columns.Get(5).CellType = textCellType15;
            this.fpQuit_SheetUndrug.Columns.Get(5).Label = "金额";
            this.fpQuit_SheetUndrug.Columns.Get(6).CellType = textCellType16;
            this.fpQuit_SheetUndrug.Columns.Get(6).Label = "执行科室";
            this.fpQuit_SheetUndrug.Columns.Get(7).CellType = checkBoxCellType3;
            this.fpQuit_SheetUndrug.Columns.Get(7).Label = "是否执行";
            this.fpQuit_SheetUndrug.Columns.Get(8).CellType = checkBoxCellType4;
            this.fpQuit_SheetUndrug.Columns.Get(8).Label = "是否退费申请";
            this.fpQuit_SheetUndrug.Columns.Get(8).Width = 85F;
            this.fpQuit_SheetUndrug.Columns.Get(9).Label = "处方号";
            this.fpQuit_SheetUndrug.Columns.Get(9).Width = 113F;
            this.fpQuit_SheetUndrug.Columns.Get(11).Label = "应执行时间";
            this.fpQuit_SheetUndrug.Columns.Get(11).Width = 120F;
            this.fpQuit_SheetUndrug.Columns.Get(12).Label = "条码";
            this.fpQuit_SheetUndrug.Columns.Get(12).Width = 120F;
            this.fpQuit_SheetUndrug.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.fpQuit_SheetUndrug.RowHeader.Columns.Default.Resizable = true;
            this.fpQuit_SheetUndrug.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpQuit_SheetUndrug.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpQuit_SheetUndrug.Rows.Default.Height = 25F;
            this.fpQuit_SheetUndrug.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.fpQuit_SheetUndrug.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.fpQuit_SheetUndrug.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpQuit_SheetUndrug.SheetCornerStyle.Locked = false;
            this.fpQuit_SheetUndrug.SheetCornerStyle.Parent = "HeaderDefault";
            this.fpQuit_SheetUndrug.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.fpQuit_SheetUndrug.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.fpQuit.SetActiveViewport(1, 1, 0);
            // 
            // gbQuitItem
            // 
            this.gbQuitItem.Controls.Add(this.ckbAllQuit);
            this.gbQuitItem.Controls.Add(this.txtUnit);
            this.gbQuitItem.Controls.Add(this.mtbQty);
            this.gbQuitItem.Controls.Add(this.lblQty);
            this.gbQuitItem.Controls.Add(this.txtPrice);
            this.gbQuitItem.Controls.Add(this.lblPrice);
            this.gbQuitItem.Controls.Add(this.txtItemName);
            this.gbQuitItem.Controls.Add(this.lblItemName);
            this.gbQuitItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbQuitItem.Location = new System.Drawing.Point(1, 1);
            this.gbQuitItem.Name = "gbQuitItem";
            this.gbQuitItem.Size = new System.Drawing.Size(915, 42);
            this.gbQuitItem.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.gbQuitItem.TabIndex = 0;
            this.gbQuitItem.TabStop = false;
            // 
            // ckbAllQuit
            // 
            this.ckbAllQuit.AutoSize = true;
            this.ckbAllQuit.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckbAllQuit.Location = new System.Drawing.Point(711, 16);
            this.ckbAllQuit.Name = "ckbAllQuit";
            this.ckbAllQuit.Size = new System.Drawing.Size(73, 17);
            this.ckbAllQuit.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.ckbAllQuit.TabIndex = 7;
            this.ckbAllQuit.Text = "全退(&A)";
            this.ckbAllQuit.UseVisualStyleBackColor = true;
            // 
            // txtUnit
            // 
            this.txtUnit.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtUnit.IsEnter2Tab = false;
            this.txtUnit.Location = new System.Drawing.Point(633, 13);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(45, 22);
            this.txtUnit.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtUnit.TabIndex = 6;
            // 
            // mtbQty
            // 
            this.mtbQty.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.mtbQty.Location = new System.Drawing.Point(561, 13);
            this.mtbQty.Name = "mtbQty";
            this.mtbQty.Size = new System.Drawing.Size(72, 22);
            this.mtbQty.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.mtbQty.TabIndex = 5;
            this.mtbQty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mtbQty_KeyDown);
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblQty.Location = new System.Drawing.Point(520, 16);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(40, 13);
            this.lblQty.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblQty.TabIndex = 4;
            this.lblQty.Text = "数量:";
            // 
            // txtPrice
            // 
            this.txtPrice.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPrice.IsEnter2Tab = false;
            this.txtPrice.Location = new System.Drawing.Point(419, 13);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(95, 22);
            this.txtPrice.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtPrice.TabIndex = 3;
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPrice.Location = new System.Drawing.Point(378, 16);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(40, 13);
            this.lblPrice.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblPrice.TabIndex = 2;
            this.lblPrice.Text = "价格:";
            // 
            // txtItemName
            // 
            this.txtItemName.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtItemName.IsEnter2Tab = false;
            this.txtItemName.Location = new System.Drawing.Point(71, 13);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(291, 22);
            this.txtItemName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtItemName.TabIndex = 1;
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblItemName.Location = new System.Drawing.Point(3, 16);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(66, 13);
            this.lblItemName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblItemName.TabIndex = 0;
            this.lblItemName.Text = "项目名称:";
            // 
            // neuPanel2
            // 
            this.neuPanel2.Controls.Add(this.cmbQuitFeeReason);
            this.neuPanel2.Controls.Add(this.neuLabel4);
            this.neuPanel2.Controls.Add(this.txtFilter);
            this.neuPanel2.Controls.Add(this.neuLabel2);
            this.neuPanel2.Controls.Add(this.neuLabel1);
            this.neuPanel2.Controls.Add(this.dtpEndTime);
            this.neuPanel2.Controls.Add(this.lblFeeTime);
            this.neuPanel2.Controls.Add(this.dtpBeginTime);
            this.neuPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuPanel2.Location = new System.Drawing.Point(0, 0);
            this.neuPanel2.Name = "neuPanel2";
            this.neuPanel2.Size = new System.Drawing.Size(917, 40);
            this.neuPanel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel2.TabIndex = 0;
            // 
            // cmbQuitFeeReason
            // 
            this.cmbQuitFeeReason.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbQuitFeeReason.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbQuitFeeReason.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.cmbQuitFeeReason.FormattingEnabled = true;
            this.cmbQuitFeeReason.IsEnter2Tab = false;
            this.cmbQuitFeeReason.IsFlat = false;
            this.cmbQuitFeeReason.IsLike = true;
            this.cmbQuitFeeReason.IsListOnly = false;
            this.cmbQuitFeeReason.IsPopForm = true;
            this.cmbQuitFeeReason.IsShowCustomerList = false;
            this.cmbQuitFeeReason.IsShowID = false;
            this.cmbQuitFeeReason.IsShowIDAndName = false;
            this.cmbQuitFeeReason.Location = new System.Drawing.Point(720, 9);
            this.cmbQuitFeeReason.Name = "cmbQuitFeeReason";
            this.cmbQuitFeeReason.ShowCustomerList = false;
            this.cmbQuitFeeReason.ShowID = false;
            this.cmbQuitFeeReason.Size = new System.Drawing.Size(133, 20);
            this.cmbQuitFeeReason.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbQuitFeeReason.TabIndex = 21;
            this.cmbQuitFeeReason.Tag = "";
            this.cmbQuitFeeReason.ToolBarUse = false;
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel4.Location = new System.Drawing.Point(648, 13);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(66, 13);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 20;
            this.neuLabel4.Text = "退费原因:";
            // 
            // txtFilter
            // 
            this.txtFilter.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFilter.IsEnter2Tab = false;
            this.txtFilter.Location = new System.Drawing.Point(467, 8);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(167, 22);
            this.txtFilter.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtFilter.TabIndex = 14;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            this.txtFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilter_KeyDown);
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel2.Location = new System.Drawing.Point(396, 13);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(66, 13);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 19;
            this.neuLabel2.Text = "项目检索:";
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel1.Location = new System.Drawing.Point(213, 13);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(27, 13);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 11;
            this.neuLabel1.Text = "到:";
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndTime.IsEnter2Tab = false;
            this.dtpEndTime.Location = new System.Drawing.Point(243, 9);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.Size = new System.Drawing.Size(138, 21);
            this.dtpEndTime.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtpEndTime.TabIndex = 12;
            // 
            // lblFeeTime
            // 
            this.lblFeeTime.AutoSize = true;
            this.lblFeeTime.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFeeTime.Location = new System.Drawing.Point(3, 12);
            this.lblFeeTime.Name = "lblFeeTime";
            this.lblFeeTime.Size = new System.Drawing.Size(66, 13);
            this.lblFeeTime.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblFeeTime.TabIndex = 9;
            this.lblFeeTime.Text = "记帐日期:";
            // 
            // dtpBeginTime
            // 
            this.dtpBeginTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpBeginTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBeginTime.IsEnter2Tab = false;
            this.dtpBeginTime.Location = new System.Drawing.Point(71, 9);
            this.dtpBeginTime.Name = "dtpBeginTime";
            this.dtpBeginTime.Size = new System.Drawing.Size(137, 21);
            this.dtpBeginTime.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtpBeginTime.TabIndex = 10;
            this.dtpBeginTime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpBeginTime_KeyDown);
            // 
            // ucQuitFeeApply
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.neuPanel1);
            this.Controls.Add(this.gbTop);
            this.Name = "ucQuitFeeApply";
            this.Size = new System.Drawing.Size(917, 607);
            this.Load += new System.EventHandler(this.ucQuitFee_Load);
            this.gbTop.ResumeLayout(false);
            this.gbTop.PerformLayout();
            this.neuPanel1.ResumeLayout(false);
            this.neuPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpUnQuit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpUnQuit_SheetDrug)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpUnQuit_SheetUndrug)).EndInit();
            this.neuPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpQuit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpQuit_SheetDrug)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpQuit_SheetUndrug)).EndInit();
            this.gbQuitItem.ResumeLayout(false);
            this.gbQuitItem.PerformLayout();
            this.neuPanel2.ResumeLayout(false);
            this.neuPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected  Neusoft.FrameWork.WinForms.Controls.NeuGroupBox gbTop;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel3;
        /// <summary>
        /// 未退FP
        /// </summary>
        protected FarPoint.Win.Spread.FpSpread fpUnQuit;
        /// <summary>
        /// 未退药品页

        /// </summary>
        protected FarPoint.Win.Spread.SheetView fpUnQuit_SheetDrug;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel2;
        /// <summary>
        /// 未退非药品页
        /// </summary>
        protected FarPoint.Win.Spread.SheetView fpUnQuit_SheetUndrug;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel4;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuGroupBox gbQuitItem;
        /// <summary>
        /// 是否全退选项
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuCheckBox ckbAllQuit;
        /// <summary>
        /// 单位
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtUnit;
        /// <summary>
        /// 退费数量

        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuMaskedTextBox mtbQty;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblQty;
        /// <summary>
        /// 单价
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtPrice;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblPrice;
        /// <summary>
        /// 项目名称 Tag保存当前费用实体
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtItemName;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblItemName;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuSplitter neuSplitter1;
        /// <summary>
        /// 已退药品页

        /// </summary>
        protected FarPoint.Win.Spread.SheetView fpQuit_SheetDrug;
        /// <summary>
        /// 已退非药品页
        /// </summary>
        protected FarPoint.Win.Spread.SheetView fpQuit_SheetUndrug;
        /// <summary>
        /// 患者查询控件

        /// </summary>
        protected Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo ucQueryPatientInfo;
        /// <summary>
        /// 床号
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtBed;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblBed;
        /// <summary>
        /// 住院科室
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtDept;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblDept;
        /// <summary>
        /// 合同单位
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtPact;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblPact;
        /// <summary>
        /// 姓名
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtName;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblName;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblFeeTime;
        /// <summary>
        /// 查询开始时间

        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpBeginTime;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        /// <summary>
        /// 查询结束时间
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpEndTime;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtFilter;
        /// <summary>
        /// 
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        public FarPoint.Win.Spread.FpSpread fpQuit;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox neuTxtInDate;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbQuitFeeReason;
        private System.ComponentModel.IContainer components;
    }
}
