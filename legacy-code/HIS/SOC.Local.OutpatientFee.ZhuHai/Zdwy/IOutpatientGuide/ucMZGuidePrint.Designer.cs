namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientGuide
{
    partial class ucMZGuidePrint
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        //private System.ComponentModel.IContainer components = null;

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
            FarPoint.Win.Spread.CellType.TextCellType textCellType7 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // neuSpread1
            // 
            this.neuSpread1.About = "3.0.2004.2005";
            this.neuSpread1.AccessibleDescription = "neuSpread1, Sheet1, Row 0, Column 0, ";
            this.neuSpread1.BackColor = System.Drawing.Color.White;
            this.neuSpread1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.neuSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuSpread1.FileName = "";
            this.neuSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.Never;
            this.neuSpread1.IsAutoSaveGridStatus = false;
            this.neuSpread1.IsCanCustomConfigColumn = false;
            this.neuSpread1.Location = new System.Drawing.Point(0, 0);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1});
            this.neuSpread1.Size = new System.Drawing.Size(240, 70);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 1;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.neuSpread1.TextTipAppearance = tipAppearance1;
            this.neuSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.Never;
            // 
            // neuSpread1_Sheet1
            // 
            this.neuSpread1_Sheet1.Reset();
            this.neuSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.neuSpread1_Sheet1.ColumnCount = 6;
            this.neuSpread1_Sheet1.ColumnHeader.RowCount = 0;
            this.neuSpread1_Sheet1.RowCount = 1;
            this.neuSpread1_Sheet1.RowHeader.ColumnCount = 0;
            textCellType1.Multiline = true;
            this.neuSpread1_Sheet1.Cells.Get(0, 0).CellType = textCellType1;
            textCellType2.Multiline = true;
            textCellType2.WordWrap = true;
            this.neuSpread1_Sheet1.Columns.Get(0).CellType = textCellType2;
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 40F;
            textCellType3.Multiline = true;
            textCellType3.WordWrap = true;
            this.neuSpread1_Sheet1.Columns.Get(1).CellType = textCellType3;
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 40F;
            textCellType4.Multiline = true;
            textCellType4.WordWrap = true;
            this.neuSpread1_Sheet1.Columns.Get(2).CellType = textCellType4;
            this.neuSpread1_Sheet1.Columns.Get(2).Width = 40F;
            textCellType5.Multiline = true;
            textCellType5.WordWrap = true;
            this.neuSpread1_Sheet1.Columns.Get(3).CellType = textCellType5;
            this.neuSpread1_Sheet1.Columns.Get(3).Width = 50F;
            textCellType6.Multiline = true;
            textCellType6.WordWrap = true;
            this.neuSpread1_Sheet1.Columns.Get(4).CellType = textCellType6;
            this.neuSpread1_Sheet1.Columns.Get(4).Width = 30F;
            textCellType7.Multiline = true;
            textCellType7.WordWrap = true;
            this.neuSpread1_Sheet1.Columns.Get(5).CellType = textCellType7;
            this.neuSpread1_Sheet1.Columns.Get(5).Width = 40F;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            // 
            // ucMZGuidePrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.neuSpread1);
            this.Name = "ucMZGuidePrint";
            this.Size = new System.Drawing.Size(240, 70);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

       // private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        //private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
    }
}
