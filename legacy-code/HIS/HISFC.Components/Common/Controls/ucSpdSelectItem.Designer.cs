namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class ucSpdSelectItem
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
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // fpSpread1
            // 
            this.fpSpread1.About = "3.0.2004.2005";
            this.fpSpread1.AccessibleDescription = "";
            this.fpSpread1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.fpSpread1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.Location = new System.Drawing.Point(0, 0);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(695, 343);
            this.fpSpread1.TabIndex = 1;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpread1.TextTipAppearance = tipAppearance1;
            this.fpSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 8;
            this.fpSpread1_Sheet1.RowCount = 0;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "助记码";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "手术名称";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "价格";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "手术编码";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "手术分类";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "手术规模";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "item_code";
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "助记码";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 66F;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "手术名称";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 216F;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = numberCellType1;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "价格";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 57F;
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 0F;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "手术编码";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 88F;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "手术分类";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 59F;
            this.fpSpread1_Sheet1.Columns.Get(6).Label = "手术规模";
            this.fpSpread1_Sheet1.Columns.Get(6).Width = 57F;
            this.fpSpread1_Sheet1.Columns.Get(7).Label = "item_code";
            this.fpSpread1_Sheet1.Columns.Get(7).Visible = false;
            this.fpSpread1_Sheet1.GrayAreaBackColor = System.Drawing.SystemColors.Window;
            this.fpSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.RowHeader.Columns.Get(0).Width = 28F;
            this.fpSpread1_Sheet1.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.fpSpread1_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.fpSpread1.SetActiveViewport(0, 1, 0);
            // 
            // ucCustomSelectItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fpSpread1);
            this.Name = "ucCustomSelectItem";
            this.Size = new System.Drawing.Size(695, 343);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
    }
}
