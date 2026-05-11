namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmPatientView
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
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.ucQueryInpatientNo1 = new Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo();
            this.dtpFromDate = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.dtpToDate = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbInhosDept = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.neuGroupBox3 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.btQueryByName = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuGroupBox2 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.btQueryByDept = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.neuPanel1.SuspendLayout();
            this.neuGroupBox3.SuspendLayout();
            this.neuGroupBox2.SuspendLayout();
            this.neuGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // ucQueryInpatientNo1
            // 
            this.ucQueryInpatientNo1.InputType = 0;
            this.ucQueryInpatientNo1.Location = new System.Drawing.Point(13, 30);
            this.ucQueryInpatientNo1.Name = "ucQueryInpatientNo1";
            this.ucQueryInpatientNo1.ShowState = Neusoft.HISFC.Components.Common.Controls.enuShowState.All;
            this.ucQueryInpatientNo1.Size = new System.Drawing.Size(177, 27);
            this.ucQueryInpatientNo1.TabIndex = 0;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "yyyy-MM-dd";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.IsEnter2Tab = false;
            this.dtpFromDate.Location = new System.Drawing.Point(75, 40);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(117, 21);
            this.dtpFromDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtpFromDate.TabIndex = 1;
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Location = new System.Drawing.Point(8, 44);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(65, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 2;
            this.neuLabel1.Text = "入院时间：";
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "yyyy-MM-dd";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.IsEnter2Tab = false;
            this.dtpToDate.Location = new System.Drawing.Point(75, 67);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(117, 21);
            this.dtpToDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtpToDate.TabIndex = 3;
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Location = new System.Drawing.Point(5, 105);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(65, 12);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 5;
            this.neuLabel3.Text = "住院科室：";
            // 
            // cmbInhosDept
            // 
            this.cmbInhosDept.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbInhosDept.FormattingEnabled = true;
            this.cmbInhosDept.IsEnter2Tab = false;
            this.cmbInhosDept.IsFlat = false;
            this.cmbInhosDept.IsLike = true;
            this.cmbInhosDept.IsListOnly = false;
            this.cmbInhosDept.IsPopForm = true;
            this.cmbInhosDept.IsShowCustomerList = false;
            this.cmbInhosDept.IsShowID = false;
            this.cmbInhosDept.Location = new System.Drawing.Point(75, 102);
            this.cmbInhosDept.Name = "cmbInhosDept";
            this.cmbInhosDept.PopForm = null;
            this.cmbInhosDept.ShowCustomerList = false;
            this.cmbInhosDept.ShowID = false;
            this.cmbInhosDept.Size = new System.Drawing.Size(117, 20);
            this.cmbInhosDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbInhosDept.TabIndex = 6;
            this.cmbInhosDept.Tag = "";
            this.cmbInhosDept.ToolBarUse = false;
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Location = new System.Drawing.Point(13, 44);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(65, 12);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 7;
            this.neuLabel4.Text = "姓    名：";
            // 
            // txtName
            // 
            this.txtName.IsEnter2Tab = false;
            this.txtName.Location = new System.Drawing.Point(75, 40);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(117, 21);
            this.txtName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtName.TabIndex = 8;
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.neuGroupBox3);
            this.neuPanel1.Controls.Add(this.neuGroupBox2);
            this.neuPanel1.Controls.Add(this.neuGroupBox1);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.neuPanel1.Location = new System.Drawing.Point(0, 0);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(204, 416);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 9;
            // 
            // neuGroupBox3
            // 
            this.neuGroupBox3.Controls.Add(this.btQueryByName);
            this.neuGroupBox3.Controls.Add(this.txtName);
            this.neuGroupBox3.Controls.Add(this.neuLabel4);
            this.neuGroupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuGroupBox3.Location = new System.Drawing.Point(0, 278);
            this.neuGroupBox3.Name = "neuGroupBox3";
            this.neuGroupBox3.Size = new System.Drawing.Size(204, 138);
            this.neuGroupBox3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox3.TabIndex = 2;
            this.neuGroupBox3.TabStop = false;
            this.neuGroupBox3.Text = "姓名查询";
            // 
            // btQueryByName
            // 
            this.btQueryByName.Location = new System.Drawing.Point(83, 79);
            this.btQueryByName.Name = "btQueryByName";
            this.btQueryByName.Size = new System.Drawing.Size(75, 23);
            this.btQueryByName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btQueryByName.TabIndex = 10;
            this.btQueryByName.Text = "查询(&C)";
            this.btQueryByName.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btQueryByName.UseVisualStyleBackColor = true;
            this.btQueryByName.Click += new System.EventHandler(this.btQueryByName_Click);
            // 
            // neuGroupBox2
            // 
            this.neuGroupBox2.Controls.Add(this.cmbInhosDept);
            this.neuGroupBox2.Controls.Add(this.neuLabel3);
            this.neuGroupBox2.Controls.Add(this.btQueryByDept);
            this.neuGroupBox2.Controls.Add(this.neuLabel1);
            this.neuGroupBox2.Controls.Add(this.dtpFromDate);
            this.neuGroupBox2.Controls.Add(this.dtpToDate);
            this.neuGroupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuGroupBox2.Location = new System.Drawing.Point(0, 82);
            this.neuGroupBox2.Name = "neuGroupBox2";
            this.neuGroupBox2.Size = new System.Drawing.Size(204, 196);
            this.neuGroupBox2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox2.TabIndex = 1;
            this.neuGroupBox2.TabStop = false;
            this.neuGroupBox2.Text = "科室查询";
            // 
            // btQueryByDept
            // 
            this.btQueryByDept.Location = new System.Drawing.Point(83, 145);
            this.btQueryByDept.Name = "btQueryByDept";
            this.btQueryByDept.Size = new System.Drawing.Size(75, 23);
            this.btQueryByDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btQueryByDept.TabIndex = 9;
            this.btQueryByDept.Text = "查询(&Q)";
            this.btQueryByDept.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btQueryByDept.UseVisualStyleBackColor = true;
            this.btQueryByDept.Click += new System.EventHandler(this.btQueryByDept_Click);
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Controls.Add(this.ucQueryInpatientNo1);
            this.neuGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(204, 82);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 0;
            this.neuGroupBox1.TabStop = false;
            this.neuGroupBox1.Text = "住院号查询";
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
            this.neuSpread1.Location = new System.Drawing.Point(204, 0);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1});
            this.neuSpread1.Size = new System.Drawing.Size(598, 416);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 10;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.neuSpread1.TextTipAppearance = tipAppearance1;
            this.neuSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.neuSpread1_CellDoubleClick);
            // 
            // neuSpread1_Sheet1
            // 
            this.neuSpread1_Sheet1.Reset();
            this.neuSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.neuSpread1_Sheet1.ColumnCount = 6;
            this.neuSpread1_Sheet1.RowCount = 0;
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "住院号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "姓名";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "住院科室";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "病区";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "入院时间";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "在院状态";
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "住院号";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 87F;
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "姓名";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 76F;
            this.neuSpread1_Sheet1.Columns.Get(2).Label = "住院科室";
            this.neuSpread1_Sheet1.Columns.Get(2).Width = 94F;
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "病区";
            this.neuSpread1_Sheet1.Columns.Get(3).Width = 85F;
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "入院时间";
            this.neuSpread1_Sheet1.Columns.Get(4).Width = 142F;
            this.neuSpread1_Sheet1.Columns.Get(5).Label = "在院状态";
            this.neuSpread1_Sheet1.Columns.Get(5).Width = 94F;
            this.neuSpread1_Sheet1.DefaultStyle.CellType = textCellType1;
            this.neuSpread1_Sheet1.DefaultStyle.Parent = "DataAreaDefault";
            this.neuSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.neuSpread1.SetActiveViewport(0, 1, 0);
            // 
            // frmPatientView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 416);
            this.Controls.Add(this.neuSpread1);
            this.Controls.Add(this.neuPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPatientView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "住院患者查询";
            this.neuPanel1.ResumeLayout(false);
            this.neuGroupBox3.ResumeLayout(false);
            this.neuGroupBox3.PerformLayout();
            this.neuGroupBox2.ResumeLayout(false);
            this.neuGroupBox2.PerformLayout();
            this.neuGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpToDate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpFromDate;
        private Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo ucQueryInpatientNo1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbInhosDept;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtName;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox2;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btQueryByName;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btQueryByDept;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox3;

    }
}