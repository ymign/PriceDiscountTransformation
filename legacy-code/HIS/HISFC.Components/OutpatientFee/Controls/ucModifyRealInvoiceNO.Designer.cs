namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucModifyRealInvoiceNO
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
            this.components = new System.ComponentModel.Container();
            FarPoint.Win.Spread.TipAppearance tipAppearance5 = new FarPoint.Win.Spread.TipAppearance();
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.cmbInvoiceType = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuGroupBox2 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.neuLabel8 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtEndInvoice = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtBeginInvoice = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuGroupBox3 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.txtNewEndNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel6 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtNewBeginNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel7 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtOldEndNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtOldBeginNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel5 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.btConfirm = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btCancel = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.neuButton1 = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuGroupBox1.SuspendLayout();
            this.neuGroupBox2.SuspendLayout();
            this.neuGroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Controls.Add(this.cmbInvoiceType);
            this.neuGroupBox1.Controls.Add(this.neuLabel1);
            this.neuGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuGroupBox1.ForeColor = System.Drawing.Color.Blue;
            this.neuGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(706, 49);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 0;
            this.neuGroupBox1.TabStop = false;
            // 
            // cmbInvoiceType
            // 
            this.cmbInvoiceType.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbInvoiceType.FormattingEnabled = true;
            this.cmbInvoiceType.IsEnter2Tab = false;
            this.cmbInvoiceType.IsFlat = false;
            this.cmbInvoiceType.IsLike = true;
            this.cmbInvoiceType.IsListOnly = false;
            this.cmbInvoiceType.IsPopForm = true;
            this.cmbInvoiceType.IsShowCustomerList = false;
            this.cmbInvoiceType.IsShowID = false;
            this.cmbInvoiceType.Location = new System.Drawing.Point(106, 17);
            this.cmbInvoiceType.Name = "cmbInvoiceType";
            this.cmbInvoiceType.PopForm = null;
            this.cmbInvoiceType.ShowCustomerList = false;
            this.cmbInvoiceType.ShowID = false;
            this.cmbInvoiceType.Size = new System.Drawing.Size(181, 20);
            this.cmbInvoiceType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbInvoiceType.TabIndex = 1;
            this.cmbInvoiceType.Tag = "";
            this.cmbInvoiceType.ToolBarUse = false;
            this.cmbInvoiceType.SelectedIndexChanged += new System.EventHandler(this.cmbInvoiceType_SelectedIndexChanged);
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Location = new System.Drawing.Point(22, 22);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(65, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 0;
            this.neuLabel1.Text = "收据类别：";
            // 
            // neuGroupBox2
            // 
            this.neuGroupBox2.Controls.Add(this.neuLabel8);
            this.neuGroupBox2.Controls.Add(this.txtEndInvoice);
            this.neuGroupBox2.Controls.Add(this.neuLabel3);
            this.neuGroupBox2.Controls.Add(this.txtBeginInvoice);
            this.neuGroupBox2.Controls.Add(this.neuLabel2);
            this.neuGroupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.neuGroupBox2.ForeColor = System.Drawing.Color.Blue;
            this.neuGroupBox2.Location = new System.Drawing.Point(3, 17);
            this.neuGroupBox2.Name = "neuGroupBox2";
            this.neuGroupBox2.Size = new System.Drawing.Size(200, 292);
            this.neuGroupBox2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox2.TabIndex = 1;
            this.neuGroupBox2.TabStop = false;
            this.neuGroupBox2.Text = "电脑号";
            // 
            // neuLabel8
            // 
            this.neuLabel8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel8.ForeColor = System.Drawing.Color.Red;
            this.neuLabel8.Location = new System.Drawing.Point(6, 156);
            this.neuLabel8.Name = "neuLabel8";
            this.neuLabel8.Size = new System.Drawing.Size(163, 89);
            this.neuLabel8.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel8.TabIndex = 4;
            this.neuLabel8.Text = "注意:批量更新时, 请保证所有的发票号都存在且物理号不会重复;";
            // 
            // txtEndInvoice
            // 
            this.txtEndInvoice.IsEnter2Tab = false;
            this.txtEndInvoice.Location = new System.Drawing.Point(55, 97);
            this.txtEndInvoice.Name = "txtEndInvoice";
            this.txtEndInvoice.Size = new System.Drawing.Size(123, 21);
            this.txtEndInvoice.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtEndInvoice.TabIndex = 3;
            this.txtEndInvoice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEndInvoice_KeyDown);
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Location = new System.Drawing.Point(6, 101);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(41, 12);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 2;
            this.neuLabel3.Text = "终止号";
            // 
            // txtBeginInvoice
            // 
            this.txtBeginInvoice.IsEnter2Tab = false;
            this.txtBeginInvoice.Location = new System.Drawing.Point(55, 47);
            this.txtBeginInvoice.Name = "txtBeginInvoice";
            this.txtBeginInvoice.Size = new System.Drawing.Size(123, 21);
            this.txtBeginInvoice.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtBeginInvoice.TabIndex = 1;
            this.txtBeginInvoice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBeginInvoice_KeyDown);
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Location = new System.Drawing.Point(6, 52);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(41, 12);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 0;
            this.neuLabel2.Text = "起始号";
            // 
            // neuGroupBox3
            // 
            this.neuGroupBox3.Controls.Add(this.txtNewEndNO);
            this.neuGroupBox3.Controls.Add(this.neuLabel6);
            this.neuGroupBox3.Controls.Add(this.txtNewBeginNO);
            this.neuGroupBox3.Controls.Add(this.neuLabel7);
            this.neuGroupBox3.Controls.Add(this.txtOldEndNO);
            this.neuGroupBox3.Controls.Add(this.neuLabel4);
            this.neuGroupBox3.Controls.Add(this.txtOldBeginNO);
            this.neuGroupBox3.Controls.Add(this.neuLabel5);
            this.neuGroupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuGroupBox3.ForeColor = System.Drawing.Color.Blue;
            this.neuGroupBox3.Location = new System.Drawing.Point(203, 17);
            this.neuGroupBox3.Name = "neuGroupBox3";
            this.neuGroupBox3.Size = new System.Drawing.Size(250, 292);
            this.neuGroupBox3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox3.TabIndex = 2;
            this.neuGroupBox3.TabStop = false;
            this.neuGroupBox3.Text = "印刷号";
            // 
            // txtNewEndNO
            // 
            this.txtNewEndNO.IsEnter2Tab = false;
            this.txtNewEndNO.Location = new System.Drawing.Point(68, 206);
            this.txtNewEndNO.Name = "txtNewEndNO";
            this.txtNewEndNO.Size = new System.Drawing.Size(123, 21);
            this.txtNewEndNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtNewEndNO.TabIndex = 11;
            this.txtNewEndNO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewEndNO_KeyDown);
            // 
            // neuLabel6
            // 
            this.neuLabel6.AutoSize = true;
            this.neuLabel6.Location = new System.Drawing.Point(9, 210);
            this.neuLabel6.Name = "neuLabel6";
            this.neuLabel6.Size = new System.Drawing.Size(53, 12);
            this.neuLabel6.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel6.TabIndex = 10;
            this.neuLabel6.Text = "新终止号";
            // 
            // txtNewBeginNO
            // 
            this.txtNewBeginNO.IsEnter2Tab = false;
            this.txtNewBeginNO.Location = new System.Drawing.Point(68, 156);
            this.txtNewBeginNO.Name = "txtNewBeginNO";
            this.txtNewBeginNO.Size = new System.Drawing.Size(123, 21);
            this.txtNewBeginNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtNewBeginNO.TabIndex = 9;
            this.txtNewBeginNO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewBeginNO_KeyDown);
            // 
            // neuLabel7
            // 
            this.neuLabel7.AutoSize = true;
            this.neuLabel7.Location = new System.Drawing.Point(9, 161);
            this.neuLabel7.Name = "neuLabel7";
            this.neuLabel7.Size = new System.Drawing.Size(53, 12);
            this.neuLabel7.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel7.TabIndex = 8;
            this.neuLabel7.Text = "新起始号";
            // 
            // txtOldEndNO
            // 
            this.txtOldEndNO.IsEnter2Tab = false;
            this.txtOldEndNO.Location = new System.Drawing.Point(68, 97);
            this.txtOldEndNO.Name = "txtOldEndNO";
            this.txtOldEndNO.ReadOnly = true;
            this.txtOldEndNO.Size = new System.Drawing.Size(123, 21);
            this.txtOldEndNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtOldEndNO.TabIndex = 7;
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Location = new System.Drawing.Point(9, 101);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(53, 12);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 6;
            this.neuLabel4.Text = "原终止号";
            // 
            // txtOldBeginNO
            // 
            this.txtOldBeginNO.IsEnter2Tab = false;
            this.txtOldBeginNO.Location = new System.Drawing.Point(68, 47);
            this.txtOldBeginNO.Name = "txtOldBeginNO";
            this.txtOldBeginNO.ReadOnly = true;
            this.txtOldBeginNO.Size = new System.Drawing.Size(123, 21);
            this.txtOldBeginNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtOldBeginNO.TabIndex = 5;
            // 
            // neuLabel5
            // 
            this.neuLabel5.AutoSize = true;
            this.neuLabel5.Location = new System.Drawing.Point(9, 52);
            this.neuLabel5.Name = "neuLabel5";
            this.neuLabel5.Size = new System.Drawing.Size(53, 12);
            this.neuLabel5.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel5.TabIndex = 4;
            this.neuLabel5.Text = "原起始号";
            // 
            // neuSpread1
            // 
            this.neuSpread1.About = "3.0.2004.2005";
            this.neuSpread1.AccessibleDescription = "neuSpread1, Sheet1, Row 0, Column 0, ";
            this.neuSpread1.BackColor = System.Drawing.Color.White;
            this.neuSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuSpread1.FileName = "";
            this.neuSpread1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.IsAutoSaveGridStatus = false;
            this.neuSpread1.IsCanCustomConfigColumn = false;
            this.neuSpread1.Location = new System.Drawing.Point(459, 17);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1});
            this.neuSpread1.Size = new System.Drawing.Size(244, 312);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 3;
            tipAppearance5.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance5.ForeColor = System.Drawing.SystemColors.InfoText;
            this.neuSpread1.TextTipAppearance = tipAppearance5;
            this.neuSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // neuSpread1_Sheet1
            // 
            this.neuSpread1_Sheet1.Reset();
            this.neuSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.neuSpread1_Sheet1.ColumnCount = 2;
            this.neuSpread1_Sheet1.RowCount = 15;
            this.neuSpread1_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "电脑号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Font = new System.Drawing.Font("宋体", 12F);
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "印刷号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.ColumnHeader.Rows.Get(0).Height = 35F;
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "电脑号";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 150F;
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "印刷号";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 150F;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.Rows.Default.Height = 30F;
            this.neuSpread1_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.neuSpread1_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.neuSpread1_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // btConfirm
            // 
            this.btConfirm.Location = new System.Drawing.Point(381, 24);
            this.btConfirm.Name = "btConfirm";
            this.btConfirm.Size = new System.Drawing.Size(75, 23);
            this.btConfirm.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btConfirm.TabIndex = 4;
            this.btConfirm.Text = "↓顺改";
            this.btConfirm.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btConfirm.UseVisualStyleBackColor = true;
            this.btConfirm.Click += new System.EventHandler(this.btConfirm_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(515, 24);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btCancel.TabIndex = 5;
            this.btCancel.Text = "取消";
            this.btCancel.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.neuButton1);
            this.groupBox1.Controls.Add(this.btConfirm);
            this.groupBox1.Controls.Add(this.btCancel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 381);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(706, 58);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.neuSpread1);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 49);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(706, 332);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.neuGroupBox3);
            this.groupBox3.Controls.Add(this.neuGroupBox2);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox3.Location = new System.Drawing.Point(3, 17);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(456, 312);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            // 
            // neuButton1
            // 
            this.neuButton1.Location = new System.Drawing.Point(274, 24);
            this.neuButton1.Name = "neuButton1";
            this.neuButton1.Size = new System.Drawing.Size(75, 23);
            this.neuButton1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuButton1.TabIndex = 4;
            this.neuButton1.Text = "↑逆改";
            this.neuButton1.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.neuButton1.UseVisualStyleBackColor = true;
            this.neuButton1.Click += new System.EventHandler(this.neuButton1_Click);
            // 
            // ucModifyRealInvoiceNO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.neuGroupBox1);
            this.Name = "ucModifyRealInvoiceNO";
            this.Size = new System.Drawing.Size(706, 439);
            this.neuGroupBox1.ResumeLayout(false);
            this.neuGroupBox1.PerformLayout();
            this.neuGroupBox2.ResumeLayout(false);
            this.neuGroupBox2.PerformLayout();
            this.neuGroupBox3.ResumeLayout(false);
            this.neuGroupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbInvoiceType;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox2;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox3;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtEndInvoice;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtBeginInvoice;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtOldEndNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtOldBeginNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtNewEndNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel6;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtNewBeginNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel7;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btConfirm;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btCancel;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton neuButton1;
    }
}
