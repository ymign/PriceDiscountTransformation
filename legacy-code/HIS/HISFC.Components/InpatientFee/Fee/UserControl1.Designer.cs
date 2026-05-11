namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    partial class ucQuitFeeReceipt
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
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.lbTitle = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.neuPanel1.SuspendLayout();
            this.SuspendLayout();
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
            this.neuSpread1.Location = new System.Drawing.Point(0, 63);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1});
            this.neuSpread1.Size = new System.Drawing.Size(832, 515);
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
            this.neuSpread1_Sheet1.ColumnCount = 9;
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "项目名称";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "规格";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "价格";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "退费数量";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "单位";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "金额";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "记帐日期";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "处方号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "处方内流水号";
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "项目名称";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 182F;
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "规格";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 56F;
            this.neuSpread1_Sheet1.Columns.Get(2).Label = "价格";
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "退费数量";
            this.neuSpread1_Sheet1.Columns.Get(3).Width = 37F;
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "单位";
            this.neuSpread1_Sheet1.Columns.Get(4).Width = 35F;
            this.neuSpread1_Sheet1.Columns.Get(5).Label = "金额";
            this.neuSpread1_Sheet1.Columns.Get(5).Width = 37F;
            this.neuSpread1_Sheet1.Columns.Get(6).Label = "记帐日期";
            this.neuSpread1_Sheet1.Columns.Get(6).Width = 152F;
            this.neuSpread1_Sheet1.Columns.Get(7).Label = "处方号";
            this.neuSpread1_Sheet1.Columns.Get(7).Width = 95F;
            this.neuSpread1_Sheet1.Columns.Get(8).Label = "处方内流水号";
            this.neuSpread1_Sheet1.Columns.Get(8).Width = 95F;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.lbTitle);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuPanel1.Location = new System.Drawing.Point(0, 0);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(832, 63);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 1;
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbTitle.Location = new System.Drawing.Point(285, 21);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(118, 21);
            this.lbTitle.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbTitle.TabIndex = 0;
            this.lbTitle.Text = "neuLabel1";
            // 
            // ucQuitFeeReceipt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.neuSpread1);
            this.Controls.Add(this.neuPanel1);
            this.Name = "ucQuitFeeReceipt";
            this.Size = new System.Drawing.Size(832, 578);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.neuPanel1.ResumeLayout(false);
            this.neuPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        public Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        public Neusoft.FrameWork.WinForms.Controls.NeuLabel lbTitle;
    }
}
