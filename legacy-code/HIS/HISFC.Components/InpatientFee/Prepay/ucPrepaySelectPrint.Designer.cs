namespace Neusoft.HISFC.Components.InpatientFee.Prepay
{
    partial class ucPrepaySelectPrint
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            FarPoint.Win.Spread.TipAppearance tipAppearance3 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.TipAppearance tipAppearance4 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType2 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType3 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType4 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType2 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.DateTimeCellType dateTimeCellType2 = new FarPoint.Win.Spread.CellType.DateTimeCellType();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucPrepaySelectPrint));
            this.ucQueryInpatientNo1 = new Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.neuPanel2 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.neuPanel3 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.fpPrepay = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.fpPrepay_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.gbPatientInfo = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.btUnchek = new System.Windows.Forms.Button();
            this.btCheck = new System.Windows.Forms.Button();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtIntimes = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtClinicDiagnose = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtBirthday = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblBirthday = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblBedNo = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtPact = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtBedNo = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblPact = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblDoctor = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtDept = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtDoctor = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblDept = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblDateIn = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtNurseStation = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtDateIn = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblNurceCell = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            this.neuPanel1.SuspendLayout();
            this.neuPanel2.SuspendLayout();
            this.neuPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpPrepay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpPrepay_Sheet1)).BeginInit();
            this.gbPatientInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // ucQueryInpatientNo1
            // 
            this.ucQueryInpatientNo1.DefaultInputType = 0;
            this.ucQueryInpatientNo1.InputType = 0;
            this.ucQueryInpatientNo1.IsDeptOnly = true;
            this.ucQueryInpatientNo1.Location = new System.Drawing.Point(10, 5);
            this.ucQueryInpatientNo1.Name = "ucQueryInpatientNo1";
            this.ucQueryInpatientNo1.PatientInState = "ALL";
            this.ucQueryInpatientNo1.ShowState = Neusoft.HISFC.Components.Common.Controls.enuShowState.All;
            this.ucQueryInpatientNo1.Size = new System.Drawing.Size(190, 28);
            this.ucQueryInpatientNo1.TabIndex = 2;
            this.ucQueryInpatientNo1.myEvent += new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(this.ucQueryInpatientNo1_myEvent);
            // 
            // neuSpread1
            // 
            this.neuSpread1.About = "3.0.2004.2005";
            this.neuSpread1.AccessibleDescription = "neuSpread1, Sheet1, Row 0, Column 0, ";
            this.neuSpread1.BackColor = System.Drawing.Color.White;
            this.neuSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuSpread1.FileName = "";
            this.neuSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.IsAutoSaveGridStatus = false;
            this.neuSpread1.IsCanCustomConfigColumn = false;
            this.neuSpread1.Location = new System.Drawing.Point(0, 0);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.Size = new System.Drawing.Size(759, 333);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 3;
            tipAppearance3.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance3.ForeColor = System.Drawing.SystemColors.InfoText;
            this.neuSpread1.TextTipAppearance = tipAppearance3;
            this.neuSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.ActiveSheetIndex = -1;
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.ucQueryInpatientNo1);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuPanel1.Location = new System.Drawing.Point(0, 0);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(759, 41);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 4;
            // 
            // neuPanel2
            // 
            this.neuPanel2.Controls.Add(this.neuPanel3);
            this.neuPanel2.Controls.Add(this.gbPatientInfo);
            this.neuPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel2.Location = new System.Drawing.Point(0, 41);
            this.neuPanel2.Name = "neuPanel2";
            this.neuPanel2.Size = new System.Drawing.Size(759, 456);
            this.neuPanel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel2.TabIndex = 5;
            // 
            // neuPanel3
            // 
            this.neuPanel3.Controls.Add(this.fpPrepay);
            this.neuPanel3.Controls.Add(this.neuSpread1);
            this.neuPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel3.Location = new System.Drawing.Point(0, 123);
            this.neuPanel3.Name = "neuPanel3";
            this.neuPanel3.Size = new System.Drawing.Size(759, 333);
            this.neuPanel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel3.TabIndex = 18;
            // 
            // fpPrepay
            // 
            this.fpPrepay.About = "3.0.2004.2005";
            this.fpPrepay.AccessibleDescription = "fpPrepay, Sheet1, Row 0, Column 0, ";
            this.fpPrepay.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.fpPrepay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpPrepay.FileName = "";
            this.fpPrepay.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpPrepay.IsAutoSaveGridStatus = false;
            this.fpPrepay.IsCanCustomConfigColumn = false;
            this.fpPrepay.Location = new System.Drawing.Point(0, 0);
            this.fpPrepay.Name = "fpPrepay";
            this.fpPrepay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpPrepay.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpPrepay_Sheet1});
            this.fpPrepay.Size = new System.Drawing.Size(759, 333);
            this.fpPrepay.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.fpPrepay.TabIndex = 4;
            tipAppearance4.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance4.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpPrepay.TextTipAppearance = tipAppearance4;
            this.fpPrepay.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // fpPrepay_Sheet1
            // 
            this.fpPrepay_Sheet1.Reset();
            this.fpPrepay_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpPrepay_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpPrepay_Sheet1.ColumnCount = 10;
            this.fpPrepay_Sheet1.ActiveSkin = FarPoint.Win.Spread.DefaultSkins.Classic2;
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "选择";
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "收据号码";
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "状态";
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "金额";
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "支付方式";
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "来源";
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "是否结算";
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "操作员";
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "原收据号码";
            this.fpPrepay_Sheet1.ColumnHeader.Cells.Get(0, 9).Value = "操作时间";
            this.fpPrepay_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(105)))), ((int)(((byte)(107)))));
            this.fpPrepay_Sheet1.ColumnHeader.DefaultStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.fpPrepay_Sheet1.ColumnHeader.DefaultStyle.ForeColor = System.Drawing.Color.White;
            this.fpPrepay_Sheet1.ColumnHeader.DefaultStyle.Locked = false;
            this.fpPrepay_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpPrepay_Sheet1.Columns.Get(0).CellType = checkBoxCellType2;
            this.fpPrepay_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpPrepay_Sheet1.Columns.Get(0).Label = "选择";
            this.fpPrepay_Sheet1.Columns.Get(0).Locked = false;
            this.fpPrepay_Sheet1.Columns.Get(0).VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.fpPrepay_Sheet1.Columns.Get(0).Width = 40F;
            this.fpPrepay_Sheet1.Columns.Get(1).CellType = textCellType3;
            this.fpPrepay_Sheet1.Columns.Get(1).Label = "收据号码";
            this.fpPrepay_Sheet1.Columns.Get(1).Locked = true;
            this.fpPrepay_Sheet1.Columns.Get(1).Width = 95F;
            this.fpPrepay_Sheet1.Columns.Get(2).CellType = textCellType4;
            this.fpPrepay_Sheet1.Columns.Get(2).Label = "状态";
            this.fpPrepay_Sheet1.Columns.Get(2).Locked = true;
            currencyCellType2.DecimalPlaces = 2;
            this.fpPrepay_Sheet1.Columns.Get(3).CellType = currencyCellType2;
            this.fpPrepay_Sheet1.Columns.Get(3).Label = "金额";
            this.fpPrepay_Sheet1.Columns.Get(3).Locked = true;
            this.fpPrepay_Sheet1.Columns.Get(3).Width = 82F;
            this.fpPrepay_Sheet1.Columns.Get(4).Label = "支付方式";
            this.fpPrepay_Sheet1.Columns.Get(4).Locked = true;
            this.fpPrepay_Sheet1.Columns.Get(4).Width = 77F;
            this.fpPrepay_Sheet1.Columns.Get(5).Label = "来源";
            this.fpPrepay_Sheet1.Columns.Get(5).Locked = true;
            this.fpPrepay_Sheet1.Columns.Get(5).Width = 77F;
            this.fpPrepay_Sheet1.Columns.Get(6).Label = "是否结算";
            this.fpPrepay_Sheet1.Columns.Get(6).Locked = true;
            this.fpPrepay_Sheet1.Columns.Get(6).Width = 75F;
            this.fpPrepay_Sheet1.Columns.Get(7).Label = "操作员";
            this.fpPrepay_Sheet1.Columns.Get(7).Locked = true;
            this.fpPrepay_Sheet1.Columns.Get(7).Width = 69F;
            this.fpPrepay_Sheet1.Columns.Get(8).Label = "原收据号码";
            this.fpPrepay_Sheet1.Columns.Get(8).Width = 112F;
            dateTimeCellType2.Calendar = ((System.Globalization.Calendar)(resources.GetObject("dateTimeCellType2.Calendar")));
            dateTimeCellType2.CalendarDayFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dateTimeCellType2.CalendarSurroundingDaysColor = System.Drawing.SystemColors.GrayText;
            dateTimeCellType2.DateDefault = new System.DateTime(2006, 11, 14, 17, 6, 27, 0);
            dateTimeCellType2.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.ShortDateWithTime;
            dateTimeCellType2.TimeDefault = new System.DateTime(2006, 11, 14, 17, 6, 27, 0);
            this.fpPrepay_Sheet1.Columns.Get(9).CellType = dateTimeCellType2;
            this.fpPrepay_Sheet1.Columns.Get(9).Label = "操作时间";
            this.fpPrepay_Sheet1.Columns.Get(9).Locked = true;
            this.fpPrepay_Sheet1.Columns.Get(9).Width = 164F;
            this.fpPrepay_Sheet1.DefaultStyle.BackColor = System.Drawing.Color.White;
            this.fpPrepay_Sheet1.DefaultStyle.ForeColor = System.Drawing.Color.Black;
            this.fpPrepay_Sheet1.DefaultStyle.Locked = false;
            this.fpPrepay_Sheet1.DefaultStyle.Parent = "DataAreaDefault";
            this.fpPrepay_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.fpPrepay_Sheet1.RowHeader.Columns.Get(0).Width = 40F;
            this.fpPrepay_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(105)))), ((int)(((byte)(107)))));
            this.fpPrepay_Sheet1.RowHeader.DefaultStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.fpPrepay_Sheet1.RowHeader.DefaultStyle.ForeColor = System.Drawing.Color.White;
            this.fpPrepay_Sheet1.RowHeader.DefaultStyle.Locked = false;
            this.fpPrepay_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpPrepay_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(105)))), ((int)(((byte)(107)))));
            this.fpPrepay_Sheet1.SheetCornerStyle.ForeColor = System.Drawing.Color.White;
            this.fpPrepay_Sheet1.SheetCornerStyle.Locked = false;
            this.fpPrepay_Sheet1.SheetCornerStyle.Parent = "HeaderDefault";
            this.fpPrepay_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.fpPrepay_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // gbPatientInfo
            // 
            this.gbPatientInfo.Controls.Add(this.btUnchek);
            this.gbPatientInfo.Controls.Add(this.btCheck);
            this.gbPatientInfo.Controls.Add(this.neuLabel1);
            this.gbPatientInfo.Controls.Add(this.txtIntimes);
            this.gbPatientInfo.Controls.Add(this.neuLabel2);
            this.gbPatientInfo.Controls.Add(this.txtClinicDiagnose);
            this.gbPatientInfo.Controls.Add(this.txtBirthday);
            this.gbPatientInfo.Controls.Add(this.lblBirthday);
            this.gbPatientInfo.Controls.Add(this.txtName);
            this.gbPatientInfo.Controls.Add(this.lblName);
            this.gbPatientInfo.Controls.Add(this.lblBedNo);
            this.gbPatientInfo.Controls.Add(this.txtPact);
            this.gbPatientInfo.Controls.Add(this.txtBedNo);
            this.gbPatientInfo.Controls.Add(this.lblPact);
            this.gbPatientInfo.Controls.Add(this.lblDoctor);
            this.gbPatientInfo.Controls.Add(this.txtDept);
            this.gbPatientInfo.Controls.Add(this.txtDoctor);
            this.gbPatientInfo.Controls.Add(this.lblDept);
            this.gbPatientInfo.Controls.Add(this.lblDateIn);
            this.gbPatientInfo.Controls.Add(this.txtNurseStation);
            this.gbPatientInfo.Controls.Add(this.txtDateIn);
            this.gbPatientInfo.Controls.Add(this.lblNurceCell);
            this.gbPatientInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbPatientInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbPatientInfo.Location = new System.Drawing.Point(0, 0);
            this.gbPatientInfo.Name = "gbPatientInfo";
            this.gbPatientInfo.Size = new System.Drawing.Size(759, 123);
            this.gbPatientInfo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.gbPatientInfo.TabIndex = 17;
            this.gbPatientInfo.TabStop = false;
            this.gbPatientInfo.Text = "患者信息";
            // 
            // btUnchek
            // 
            this.btUnchek.Location = new System.Drawing.Point(636, 90);
            this.btUnchek.Name = "btUnchek";
            this.btUnchek.Size = new System.Drawing.Size(45, 23);
            this.btUnchek.TabIndex = 21;
            this.btUnchek.Text = "反选";
            this.btUnchek.UseVisualStyleBackColor = true;
            this.btUnchek.Click += new System.EventHandler(this.btUnchek_Click);
            // 
            // btCheck
            // 
            this.btCheck.Location = new System.Drawing.Point(562, 90);
            this.btCheck.Name = "btCheck";
            this.btCheck.Size = new System.Drawing.Size(49, 23);
            this.btCheck.TabIndex = 20;
            this.btCheck.Text = "全选";
            this.btCheck.UseVisualStyleBackColor = true;
            this.btCheck.Click += new System.EventHandler(this.btCheck_Click);
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Location = new System.Drawing.Point(373, 90);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(53, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 19;
            this.neuLabel1.Text = "住院次数";
            // 
            // txtIntimes
            // 
            this.txtIntimes.BackColor = System.Drawing.Color.White;
            this.txtIntimes.ForeColor = System.Drawing.Color.Black;
            this.txtIntimes.IsEnter2Tab = false;
            this.txtIntimes.Location = new System.Drawing.Point(437, 86);
            this.txtIntimes.Name = "txtIntimes";
            this.txtIntimes.ReadOnly = true;
            this.txtIntimes.Size = new System.Drawing.Size(100, 21);
            this.txtIntimes.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtIntimes.TabIndex = 18;
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Location = new System.Drawing.Point(20, 89);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(53, 12);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 17;
            this.neuLabel2.Text = "门诊诊断";
            // 
            // txtClinicDiagnose
            // 
            this.txtClinicDiagnose.BackColor = System.Drawing.Color.White;
            this.txtClinicDiagnose.ForeColor = System.Drawing.Color.Black;
            this.txtClinicDiagnose.IsEnter2Tab = false;
            this.txtClinicDiagnose.Location = new System.Drawing.Point(77, 87);
            this.txtClinicDiagnose.Name = "txtClinicDiagnose";
            this.txtClinicDiagnose.ReadOnly = true;
            this.txtClinicDiagnose.Size = new System.Drawing.Size(286, 21);
            this.txtClinicDiagnose.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtClinicDiagnose.TabIndex = 16;
            // 
            // txtBirthday
            // 
            this.txtBirthday.BackColor = System.Drawing.Color.White;
            this.txtBirthday.ForeColor = System.Drawing.Color.Black;
            this.txtBirthday.IsEnter2Tab = false;
            this.txtBirthday.Location = new System.Drawing.Point(602, 54);
            this.txtBirthday.Name = "txtBirthday";
            this.txtBirthday.ReadOnly = true;
            this.txtBirthday.Size = new System.Drawing.Size(100, 21);
            this.txtBirthday.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtBirthday.TabIndex = 14;
            // 
            // lblBirthday
            // 
            this.lblBirthday.AutoSize = true;
            this.lblBirthday.Location = new System.Drawing.Point(547, 56);
            this.lblBirthday.Name = "lblBirthday";
            this.lblBirthday.Size = new System.Drawing.Size(53, 12);
            this.lblBirthday.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblBirthday.TabIndex = 15;
            this.lblBirthday.Text = "出生日期";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.White;
            this.txtName.ForeColor = System.Drawing.Color.Black;
            this.txtName.IsEnter2Tab = false;
            this.txtName.Location = new System.Drawing.Point(77, 19);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(100, 21);
            this.txtName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtName.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(53, 12);
            this.lblName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblName.TabIndex = 1;
            this.lblName.Text = "患者姓名";
            // 
            // lblBedNo
            // 
            this.lblBedNo.AutoSize = true;
            this.lblBedNo.Location = new System.Drawing.Point(396, 56);
            this.lblBedNo.Name = "lblBedNo";
            this.lblBedNo.Size = new System.Drawing.Size(29, 12);
            this.lblBedNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblBedNo.TabIndex = 13;
            this.lblBedNo.Text = "床号";
            // 
            // txtPact
            // 
            this.txtPact.BackColor = System.Drawing.Color.White;
            this.txtPact.ForeColor = System.Drawing.Color.Black;
            this.txtPact.IsEnter2Tab = false;
            this.txtPact.Location = new System.Drawing.Point(263, 19);
            this.txtPact.Name = "txtPact";
            this.txtPact.ReadOnly = true;
            this.txtPact.Size = new System.Drawing.Size(100, 21);
            this.txtPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtPact.TabIndex = 2;
            // 
            // txtBedNo
            // 
            this.txtBedNo.BackColor = System.Drawing.Color.White;
            this.txtBedNo.ForeColor = System.Drawing.Color.Black;
            this.txtBedNo.IsEnter2Tab = false;
            this.txtBedNo.Location = new System.Drawing.Point(437, 54);
            this.txtBedNo.Name = "txtBedNo";
            this.txtBedNo.ReadOnly = true;
            this.txtBedNo.Size = new System.Drawing.Size(100, 21);
            this.txtBedNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtBedNo.TabIndex = 12;
            // 
            // lblPact
            // 
            this.lblPact.AutoSize = true;
            this.lblPact.Location = new System.Drawing.Point(195, 22);
            this.lblPact.Name = "lblPact";
            this.lblPact.Size = new System.Drawing.Size(53, 12);
            this.lblPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblPact.TabIndex = 3;
            this.lblPact.Text = "合同单位";
            // 
            // lblDoctor
            // 
            this.lblDoctor.AutoSize = true;
            this.lblDoctor.Location = new System.Drawing.Point(195, 56);
            this.lblDoctor.Name = "lblDoctor";
            this.lblDoctor.Size = new System.Drawing.Size(53, 12);
            this.lblDoctor.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblDoctor.TabIndex = 11;
            this.lblDoctor.Text = "住院医生";
            // 
            // txtDept
            // 
            this.txtDept.BackColor = System.Drawing.Color.White;
            this.txtDept.ForeColor = System.Drawing.Color.Black;
            this.txtDept.IsEnter2Tab = false;
            this.txtDept.Location = new System.Drawing.Point(437, 19);
            this.txtDept.Name = "txtDept";
            this.txtDept.ReadOnly = true;
            this.txtDept.Size = new System.Drawing.Size(100, 21);
            this.txtDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtDept.TabIndex = 4;
            // 
            // txtDoctor
            // 
            this.txtDoctor.BackColor = System.Drawing.Color.White;
            this.txtDoctor.ForeColor = System.Drawing.Color.Black;
            this.txtDoctor.IsEnter2Tab = false;
            this.txtDoctor.Location = new System.Drawing.Point(263, 54);
            this.txtDoctor.Name = "txtDoctor";
            this.txtDoctor.ReadOnly = true;
            this.txtDoctor.Size = new System.Drawing.Size(100, 21);
            this.txtDoctor.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtDoctor.TabIndex = 10;
            // 
            // lblDept
            // 
            this.lblDept.AutoSize = true;
            this.lblDept.Location = new System.Drawing.Point(372, 22);
            this.lblDept.Name = "lblDept";
            this.lblDept.Size = new System.Drawing.Size(53, 12);
            this.lblDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblDept.TabIndex = 5;
            this.lblDept.Text = "住院科室";
            // 
            // lblDateIn
            // 
            this.lblDateIn.AutoSize = true;
            this.lblDateIn.Location = new System.Drawing.Point(20, 56);
            this.lblDateIn.Name = "lblDateIn";
            this.lblDateIn.Size = new System.Drawing.Size(53, 12);
            this.lblDateIn.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblDateIn.TabIndex = 9;
            this.lblDateIn.Text = "入院日期";
            // 
            // txtNurseStation
            // 
            this.txtNurseStation.BackColor = System.Drawing.Color.White;
            this.txtNurseStation.ForeColor = System.Drawing.Color.Black;
            this.txtNurseStation.IsEnter2Tab = false;
            this.txtNurseStation.Location = new System.Drawing.Point(602, 19);
            this.txtNurseStation.Name = "txtNurseStation";
            this.txtNurseStation.ReadOnly = true;
            this.txtNurseStation.Size = new System.Drawing.Size(100, 21);
            this.txtNurseStation.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtNurseStation.TabIndex = 6;
            // 
            // txtDateIn
            // 
            this.txtDateIn.BackColor = System.Drawing.Color.White;
            this.txtDateIn.ForeColor = System.Drawing.Color.Black;
            this.txtDateIn.IsEnter2Tab = false;
            this.txtDateIn.Location = new System.Drawing.Point(77, 54);
            this.txtDateIn.Name = "txtDateIn";
            this.txtDateIn.ReadOnly = true;
            this.txtDateIn.Size = new System.Drawing.Size(100, 21);
            this.txtDateIn.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtDateIn.TabIndex = 8;
            // 
            // lblNurceCell
            // 
            this.lblNurceCell.AutoSize = true;
            this.lblNurceCell.Location = new System.Drawing.Point(547, 22);
            this.lblNurceCell.Name = "lblNurceCell";
            this.lblNurceCell.Size = new System.Drawing.Size(53, 12);
            this.lblNurceCell.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblNurceCell.TabIndex = 7;
            this.lblNurceCell.Text = "所属病区";
            // 
            // ucPrepaySelectPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.neuPanel2);
            this.Controls.Add(this.neuPanel1);
            this.Name = "ucPrepaySelectPrint";
            this.Size = new System.Drawing.Size(759, 497);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            this.neuPanel1.ResumeLayout(false);
            this.neuPanel2.ResumeLayout(false);
            this.neuPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpPrepay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpPrepay_Sheet1)).EndInit();
            this.gbPatientInfo.ResumeLayout(false);
            this.gbPatientInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo ucQueryInpatientNo1;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel3;
        protected Neusoft.FrameWork.WinForms.Controls.NeuGroupBox gbPatientInfo;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtIntimes;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtClinicDiagnose;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtBirthday;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblBirthday;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtName;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblName;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblBedNo;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtPact;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtBedNo;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblPact;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblDoctor;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtDept;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtDoctor;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblDept;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblDateIn;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtNurseStation;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtDateIn;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblNurceCell;
        protected Neusoft.FrameWork.WinForms.Controls.NeuSpread fpPrepay;
        protected FarPoint.Win.Spread.SheetView fpPrepay_Sheet1;
        private System.Windows.Forms.Button btUnchek;
        private System.Windows.Forms.Button btCheck;
    }
}
