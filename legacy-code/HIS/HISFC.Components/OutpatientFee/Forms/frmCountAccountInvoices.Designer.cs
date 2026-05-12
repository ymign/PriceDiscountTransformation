namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    partial class frmCountAccountInvoices
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
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.btExit = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btClear = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btQuery = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuLabel8 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.tbReturn = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel7 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbReal = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel6 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbCheck = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel5 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbPos = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbCash = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbTot = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbCount = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.neuGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.btExit);
            this.neuPanel1.Controls.Add(this.btClear);
            this.neuPanel1.Controls.Add(this.btQuery);
            this.neuPanel1.Controls.Add(this.neuLabel8);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.neuPanel1.Location = new System.Drawing.Point(0, 319);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(837, 50);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 2;
            // 
            // btExit
            // 
            this.btExit.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btExit.Location = new System.Drawing.Point(736, 13);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(75, 23);
            this.btExit.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btExit.TabIndex = 3;
            this.btExit.Text = "退出(&X)";
            this.btExit.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btExit.UseVisualStyleBackColor = true;
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            this.btExit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btExit_KeyDown);
            // 
            // btClear
            // 
            this.btClear.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btClear.Location = new System.Drawing.Point(629, 13);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(75, 23);
            this.btClear.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btClear.TabIndex = 2;
            this.btClear.Text = "清屏(&C)";
            this.btClear.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // btQuery
            // 
            this.btQuery.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btQuery.Location = new System.Drawing.Point(522, 13);
            this.btQuery.Name = "btQuery";
            this.btQuery.Size = new System.Drawing.Size(75, 23);
            this.btQuery.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btQuery.TabIndex = 1;
            this.btQuery.Text = "查询(&Q)";
            this.btQuery.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btQuery.UseVisualStyleBackColor = true;
            this.btQuery.Click += new System.EventHandler(this.btQuery_Click);
            // 
            // neuLabel8
            // 
            this.neuLabel8.AutoSize = true;
            this.neuLabel8.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel8.ForeColor = System.Drawing.Color.Red;
            this.neuLabel8.Location = new System.Drawing.Point(12, 16);
            this.neuLabel8.Name = "neuLabel8";
            this.neuLabel8.Size = new System.Drawing.Size(175, 14);
            this.neuLabel8.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel8.TabIndex = 0;
            this.neuLabel8.Text = "输入发票张数累计发票金额";
            // 
            // neuSpread1
            // 
            this.neuSpread1.About = "3.0.2004.2005";
            this.neuSpread1.AccessibleDescription = "neuSpread1, Sheet1, Row 0, Column 0, ";
            this.neuSpread1.BackColor = System.Drawing.SystemColors.Control;
            this.neuSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuSpread1.FileName = "";
            this.neuSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.IsAutoSaveGridStatus = false;
            this.neuSpread1.IsCanCustomConfigColumn = false;
            this.neuSpread1.Location = new System.Drawing.Point(0, 80);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1});
            this.neuSpread1.Size = new System.Drawing.Size(837, 289);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 1;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.neuSpread1.TextTipAppearance = tipAppearance1;
            this.neuSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // neuSpread1_Sheet1
            // 
            this.neuSpread1_Sheet1.Reset();
            this.neuSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.neuSpread1_Sheet1.ColumnCount = 6;
            this.neuSpread1_Sheet1.RowCount = 1;
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "发票号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "病历号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "患者姓名";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "收费时间";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "合计金额";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "支付方式";
            this.neuSpread1_Sheet1.Columns.Get(0).CellType = textCellType1;
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "发票号";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 105F;
            this.neuSpread1_Sheet1.Columns.Get(1).CellType = textCellType2;
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "病历号";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 105F;
            this.neuSpread1_Sheet1.Columns.Get(2).Label = "患者姓名";
            this.neuSpread1_Sheet1.Columns.Get(2).Width = 70F;
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "收费时间";
            this.neuSpread1_Sheet1.Columns.Get(3).Width = 147F;
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "合计金额";
            this.neuSpread1_Sheet1.Columns.Get(4).Width = 70F;
            this.neuSpread1_Sheet1.Columns.Get(5).Label = "支付方式";
            this.neuSpread1_Sheet1.Columns.Get(5).Width = 100F;
            this.neuSpread1_Sheet1.GrayAreaBackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Controls.Add(this.tbReturn);
            this.neuGroupBox1.Controls.Add(this.neuLabel7);
            this.neuGroupBox1.Controls.Add(this.tbReal);
            this.neuGroupBox1.Controls.Add(this.neuLabel6);
            this.neuGroupBox1.Controls.Add(this.tbCheck);
            this.neuGroupBox1.Controls.Add(this.neuLabel5);
            this.neuGroupBox1.Controls.Add(this.tbPos);
            this.neuGroupBox1.Controls.Add(this.neuLabel4);
            this.neuGroupBox1.Controls.Add(this.tbCash);
            this.neuGroupBox1.Controls.Add(this.neuLabel3);
            this.neuGroupBox1.Controls.Add(this.tbTot);
            this.neuGroupBox1.Controls.Add(this.neuLabel2);
            this.neuGroupBox1.Controls.Add(this.tbCount);
            this.neuGroupBox1.Controls.Add(this.neuLabel1);
            this.neuGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(837, 80);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 0;
            this.neuGroupBox1.TabStop = false;
            // 
            // tbReturn
            // 
            this.tbReturn.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbReturn.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbReturn.IsEnter2Tab = false;
            this.tbReturn.Location = new System.Drawing.Point(684, 16);
            this.tbReturn.Name = "tbReturn";
            this.tbReturn.ReadOnly = true;
            this.tbReturn.Size = new System.Drawing.Size(100, 25);
            this.tbReturn.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbReturn.TabIndex = 13;
            this.tbReturn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbReturn_KeyDown);
            // 
            // neuLabel7
            // 
            this.neuLabel7.AutoSize = true;
            this.neuLabel7.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel7.Location = new System.Drawing.Point(623, 20);
            this.neuLabel7.Name = "neuLabel7";
            this.neuLabel7.Size = new System.Drawing.Size(55, 15);
            this.neuLabel7.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel7.TabIndex = 12;
            this.neuLabel7.Text = "找零：";
            // 
            // tbReal
            // 
            this.tbReal.BackColor = System.Drawing.Color.White;
            this.tbReal.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbReal.IsEnter2Tab = false;
            this.tbReal.Location = new System.Drawing.Point(493, 16);
            this.tbReal.Name = "tbReal";
            this.tbReal.Size = new System.Drawing.Size(100, 25);
            this.tbReal.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbReal.TabIndex = 11;
            this.tbReal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbReal_KeyDown);
            // 
            // neuLabel6
            // 
            this.neuLabel6.AutoSize = true;
            this.neuLabel6.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel6.Location = new System.Drawing.Point(432, 20);
            this.neuLabel6.Name = "neuLabel6";
            this.neuLabel6.Size = new System.Drawing.Size(48, 15);
            this.neuLabel6.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel6.TabIndex = 10;
            this.neuLabel6.Text = "实收:";
            // 
            // tbCheck
            // 
            this.tbCheck.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbCheck.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCheck.IsEnter2Tab = false;
            this.tbCheck.Location = new System.Drawing.Point(493, 46);
            this.tbCheck.Name = "tbCheck";
            this.tbCheck.ReadOnly = true;
            this.tbCheck.Size = new System.Drawing.Size(100, 25);
            this.tbCheck.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCheck.TabIndex = 9;
            // 
            // neuLabel5
            // 
            this.neuLabel5.AutoSize = true;
            this.neuLabel5.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel5.Location = new System.Drawing.Point(432, 53);
            this.neuLabel5.Name = "neuLabel5";
            this.neuLabel5.Size = new System.Drawing.Size(55, 15);
            this.neuLabel5.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel5.TabIndex = 8;
            this.neuLabel5.Text = "支票：";
            // 
            // tbPos
            // 
            this.tbPos.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbPos.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPos.IsEnter2Tab = false;
            this.tbPos.Location = new System.Drawing.Point(307, 46);
            this.tbPos.Name = "tbPos";
            this.tbPos.ReadOnly = true;
            this.tbPos.Size = new System.Drawing.Size(100, 25);
            this.tbPos.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbPos.TabIndex = 7;
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel4.Location = new System.Drawing.Point(237, 53);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(48, 15);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 6;
            this.neuLabel4.Text = "银联:";
            // 
            // tbCash
            // 
            this.tbCash.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbCash.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCash.IsEnter2Tab = false;
            this.tbCash.Location = new System.Drawing.Point(96, 46);
            this.tbCash.Name = "tbCash";
            this.tbCash.ReadOnly = true;
            this.tbCash.Size = new System.Drawing.Size(100, 25);
            this.tbCash.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCash.TabIndex = 5;
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel3.Location = new System.Drawing.Point(16, 53);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(55, 15);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 4;
            this.neuLabel3.Text = "现金：";
            // 
            // tbTot
            // 
            this.tbTot.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbTot.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbTot.IsEnter2Tab = false;
            this.tbTot.Location = new System.Drawing.Point(307, 16);
            this.tbTot.Name = "tbTot";
            this.tbTot.ReadOnly = true;
            this.tbTot.Size = new System.Drawing.Size(100, 26);
            this.tbTot.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbTot.TabIndex = 3;
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel2.ForeColor = System.Drawing.Color.Red;
            this.neuLabel2.Location = new System.Drawing.Point(237, 20);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(59, 16);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 2;
            this.neuLabel2.Text = "合计：";
            // 
            // tbCount
            // 
            this.tbCount.BackColor = System.Drawing.Color.White;
            this.tbCount.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCount.IsEnter2Tab = false;
            this.tbCount.Location = new System.Drawing.Point(96, 16);
            this.tbCount.Name = "tbCount";
            this.tbCount.Size = new System.Drawing.Size(100, 24);
            this.tbCount.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCount.TabIndex = 1;
            this.tbCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbCount_KeyDown);
            // 
            // neuLabel1
            // 
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel1.Location = new System.Drawing.Point(16, 20);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(80, 16);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 0;
            this.neuLabel1.Text = "发票张数:";
            // 
            // frmCountAccountInvoices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 369);
            this.Controls.Add(this.neuPanel1);
            this.Controls.Add(this.neuSpread1);
            this.Controls.Add(this.neuGroupBox1);
            this.Name = "frmCountAccountInvoices";
            this.Text = "输入门诊账号发票张数累计发票金额";
            this.Load += new System.EventHandler(this.frmCountAccountInvoices_Load);
            this.neuPanel1.ResumeLayout(false);
            this.neuPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.neuGroupBox1.ResumeLayout(false);
            this.neuGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbCount;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbTot;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbCash;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbCheck;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbPos;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbReturn;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel7;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbReal;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel6;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel8;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btExit;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btClear;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btQuery;
    }
}