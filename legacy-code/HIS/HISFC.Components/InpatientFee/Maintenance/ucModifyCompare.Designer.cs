namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    partial class ucModifyCompare
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbCompareQuery = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel9 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fpCompareItem = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.fpCompareItem_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpCompareItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpCompareItem_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbCompareQuery);
            this.groupBox1.Controls.Add(this.neuLabel9);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(688, 66);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(274, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 12);
            this.label1.TabIndex = 20;
            this.label1.Text = "医保目录属性：  1、甲   2、乙   3、丙";
            // 
            // tbCompareQuery
            // 
            this.tbCompareQuery.IsEnter2Tab = false;
            this.tbCompareQuery.Location = new System.Drawing.Point(110, 21);
            this.tbCompareQuery.Name = "tbCompareQuery";
            this.tbCompareQuery.Size = new System.Drawing.Size(102, 21);
            this.tbCompareQuery.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCompareQuery.TabIndex = 19;
            this.tbCompareQuery.TextChanged += new System.EventHandler(this.tbCompareQuery_TextChanged);
            // 
            // neuLabel9
            // 
            this.neuLabel9.AutoSize = true;
            this.neuLabel9.Location = new System.Drawing.Point(6, 26);
            this.neuLabel9.Name = "neuLabel9";
            this.neuLabel9.Size = new System.Drawing.Size(95, 12);
            this.neuLabel9.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel9.TabIndex = 18;
            this.neuLabel9.Text = "已对照项目查询:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fpCompareItem);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 66);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(688, 532);
            this.panel1.TabIndex = 1;
            // 
            // fpCompareItem
            // 
            this.fpCompareItem.About = "3.0.2004.2005";
            this.fpCompareItem.AccessibleDescription = "fpCompareItem, Sheet1, Row 0, Column 0, ";
            this.fpCompareItem.BackColor = System.Drawing.Color.White;
            this.fpCompareItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpCompareItem.FileName = "";
            this.fpCompareItem.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpCompareItem.IsAutoSaveGridStatus = false;
            this.fpCompareItem.IsCanCustomConfigColumn = false;
            this.fpCompareItem.Location = new System.Drawing.Point(0, 0);
            this.fpCompareItem.Name = "fpCompareItem";
            this.fpCompareItem.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpCompareItem.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpCompareItem_Sheet1});
            this.fpCompareItem.Size = new System.Drawing.Size(688, 532);
            this.fpCompareItem.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.fpCompareItem.TabIndex = 1;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpCompareItem.TextTipAppearance = tipAppearance1;
            this.fpCompareItem.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpCompareItem.EditChange += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpCompareItem_EditChange);
            // 
            // fpCompareItem_Sheet1
            // 
            this.fpCompareItem_Sheet1.Reset();
            this.fpCompareItem_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpCompareItem_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpCompareItem_Sheet1.Columns.Get(0).AllowAutoSort = true;
            this.fpCompareItem_Sheet1.Columns.Get(0).SortIndicator = FarPoint.Win.Spread.Model.SortIndicator.Ascending;
            this.fpCompareItem_Sheet1.Columns.Get(17).AllowAutoSort = true;
            this.fpCompareItem_Sheet1.Columns.Get(17).SortIndicator = FarPoint.Win.Spread.Model.SortIndicator.Descending;
            this.fpCompareItem_Sheet1.Columns.Get(23).AllowAutoSort = true;
            this.fpCompareItem_Sheet1.Columns.Get(23).SortIndicator = FarPoint.Win.Spread.Model.SortIndicator.Descending;
            this.fpCompareItem_Sheet1.GrayAreaBackColor = System.Drawing.Color.White;
            this.fpCompareItem_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpCompareItem_Sheet1.RowHeader.Columns.Get(0).AllowAutoSort = true;
            this.fpCompareItem_Sheet1.RowHeader.Columns.Get(0).Width = 37F;
            this.fpCompareItem_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.fpCompareItem.SetViewportLeftColumn(0, 0, 8);
            // 
            // ucModifyCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "ucModifyCompare";
            this.Size = new System.Drawing.Size(688, 598);
            this.Load += new System.EventHandler(this.ucModifyCompare_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpCompareItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpCompareItem_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread fpCompareItem;
        private FarPoint.Win.Spread.SheetView fpCompareItem_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbCompareQuery;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel9;
        private System.Windows.Forms.Label label1;

    }
}
