namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucVerifyPatientPact
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
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType3 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType4 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType5 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType6 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.gboTop = new System.Windows.Forms.GroupBox();
            this.dtpDealTime = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.chkTimer = new Neusoft.FrameWork.WinForms.Controls.NeuCheckBox();
            this.gboFull = new System.Windows.Forms.GroupBox();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.gboTop.SuspendLayout();
            this.gboFull.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // gboTop
            // 
            this.gboTop.Controls.Add(this.dtpDealTime);
            this.gboTop.Controls.Add(this.chkTimer);
            this.gboTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.gboTop.Location = new System.Drawing.Point(0, 0);
            this.gboTop.Name = "gboTop";
            this.gboTop.Size = new System.Drawing.Size(828, 70);
            this.gboTop.TabIndex = 0;
            this.gboTop.TabStop = false;
            this.gboTop.Text = "较验设置";
            // 
            // dtpDealTime
            // 
            this.dtpDealTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpDealTime.Enabled = false;
            this.dtpDealTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDealTime.IsEnter2Tab = false;
            this.dtpDealTime.Location = new System.Drawing.Point(190, 31);
            this.dtpDealTime.Name = "dtpDealTime";
            this.dtpDealTime.Size = new System.Drawing.Size(160, 21);
            this.dtpDealTime.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtpDealTime.TabIndex = 1;
            // 
            // chkTimer
            // 
            this.chkTimer.AutoSize = true;
            this.chkTimer.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkTimer.ForeColor = System.Drawing.Color.Red;
            this.chkTimer.Location = new System.Drawing.Point(82, 33);
            this.chkTimer.Name = "chkTimer";
            this.chkTimer.Size = new System.Drawing.Size(102, 16);
            this.chkTimer.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.chkTimer.TabIndex = 0;
            this.chkTimer.Text = "指定时间执行";
            this.chkTimer.UseVisualStyleBackColor = true;
            this.chkTimer.CheckedChanged += new System.EventHandler(this.chkTimer_CheckedChanged);
            // 
            // gboFull
            // 
            this.gboFull.Controls.Add(this.neuSpread1);
            this.gboFull.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboFull.Location = new System.Drawing.Point(0, 70);
            this.gboFull.Name = "gboFull";
            this.gboFull.Size = new System.Drawing.Size(828, 405);
            this.gboFull.TabIndex = 1;
            this.gboFull.TabStop = false;
            this.gboFull.Text = "较验失败数据";
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
            this.neuSpread1.Location = new System.Drawing.Point(3, 17);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1});
            this.neuSpread1.Size = new System.Drawing.Size(822, 385);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 0;
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
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "病历号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "姓名";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "性别";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "身份证号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "合同单位";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "备注";
            this.neuSpread1_Sheet1.Columns.Get(0).CellType = textCellType1;
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "病历号";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 80F;
            this.neuSpread1_Sheet1.Columns.Get(1).CellType = textCellType2;
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "姓名";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 80F;
            this.neuSpread1_Sheet1.Columns.Get(2).CellType = textCellType3;
            this.neuSpread1_Sheet1.Columns.Get(2).Label = "性别";
            this.neuSpread1_Sheet1.Columns.Get(2).Width = 45F;
            this.neuSpread1_Sheet1.Columns.Get(3).CellType = textCellType4;
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "身份证号";
            this.neuSpread1_Sheet1.Columns.Get(3).Width = 120F;
            this.neuSpread1_Sheet1.Columns.Get(4).CellType = textCellType5;
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "合同单位";
            this.neuSpread1_Sheet1.Columns.Get(5).CellType = textCellType6;
            this.neuSpread1_Sheet1.Columns.Get(5).Label = "备注";
            this.neuSpread1_Sheet1.Columns.Get(5).Width = 215F;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // ucVerifyPatientPact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gboFull);
            this.Controls.Add(this.gboTop);
            this.Name = "ucVerifyPatientPact";
            this.Size = new System.Drawing.Size(828, 475);
            this.gboTop.ResumeLayout(false);
            this.gboTop.PerformLayout();
            this.gboFull.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gboTop;
        private System.Windows.Forms.GroupBox gboFull;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuCheckBox chkTimer;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtpDealTime;
    }
}
