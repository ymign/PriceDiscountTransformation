namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucQuitItemApply
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
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet2)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet2)).BeginInit();
            this.neuTabControl1.SuspendLayout();
            this.tpQuit.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuLabel2
            // 
            this.neuLabel2.Location = new System.Drawing.Point(166, 19);
            // 
            // fpSpread1
            // 
            this.fpSpread1.AccessibleDescription = "fpSpread1, 非药品(F8), Row 0, Column 8, ";
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "药品(F7)";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 10;
            this.fpSpread1_Sheet1.RowCount = 5;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "药品名称";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "组";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "组合号";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "规格";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "数量";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "单位";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "可退数量";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "金额";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "每次量和付数";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread1_Sheet1.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpSpread1_Sheet1.DefaultStyle.Parent = "DataAreaDefault";
            this.fpSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread1_Sheet1.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Range;
            this.fpSpread1_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Cell;
            this.fpSpread1_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread1_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.fpSpread1_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Auto;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // fpSpread1_Sheet2
            // 
            this.fpSpread1_Sheet2.Reset();
            this.fpSpread1_Sheet2.SheetName = "非药品(F8)";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet2.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet2.ColumnCount = 9;
            this.fpSpread1_Sheet2.RowCount = 5;
            this.fpSpread1_Sheet2.ActiveColumnIndex = 8;
            this.fpSpread1_Sheet2.ColumnHeader.Cells.Get(0, 0).Value = "非药品名称";
            this.fpSpread1_Sheet2.ColumnHeader.Cells.Get(0, 1).Value = "组";
            this.fpSpread1_Sheet2.ColumnHeader.Cells.Get(0, 2).Value = "组合号";
            this.fpSpread1_Sheet2.ColumnHeader.Cells.Get(0, 3).Value = "数量";
            this.fpSpread1_Sheet2.ColumnHeader.Cells.Get(0, 4).Value = "单位";
            this.fpSpread1_Sheet2.ColumnHeader.Cells.Get(0, 5).Value = "可退数量";
            this.fpSpread1_Sheet2.ColumnHeader.Cells.Get(0, 6).Value = "金额";
            this.fpSpread1_Sheet2.ColumnHeader.Cells.Get(0, 7).Value = "组合信息或自定义码";
            this.fpSpread1_Sheet2.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread1_Sheet2.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread1_Sheet2.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpSpread1_Sheet2.DefaultStyle.Parent = "DataAreaDefault";
            this.fpSpread1_Sheet2.OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
            this.fpSpread1_Sheet2.RowHeader.Columns.Default.Resizable = true;
            this.fpSpread1_Sheet2.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread1_Sheet2.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread1_Sheet2.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Range;
            this.fpSpread1_Sheet2.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Cell;
            this.fpSpread1_Sheet2.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread1_Sheet2.SheetCornerStyle.Parent = "CornerDefault";
            this.fpSpread1_Sheet2.VisualStyles = FarPoint.Win.VisualStyles.Auto;
            this.fpSpread1_Sheet2.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // fpSpread2
            // 
            this.fpSpread2.AccessibleDescription = "fpSpread2, 已退非药品(F8), Row 0, Column 2, ";
            // 
            // fpSpread2_Sheet1
            // 
            this.fpSpread2_Sheet1.Reset();
            this.fpSpread2_Sheet1.SheetName = "已退药品(F7)";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread2_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread2_Sheet1.ColumnCount = 7;
            this.fpSpread2_Sheet1.RowCount = 5;
            this.fpSpread2_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "已退药品名称";
            this.fpSpread2_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "规格";
            this.fpSpread2_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "已退数量";
            this.fpSpread2_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "单位";
            this.fpSpread2_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "标志";
            this.fpSpread2_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "单价";
            this.fpSpread2_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "金额";
            this.fpSpread2_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread2_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread2_Sheet1.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpSpread2_Sheet1.DefaultStyle.Parent = "DataAreaDefault";
            this.fpSpread2_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;
            this.fpSpread2_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.fpSpread2_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread2_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread2_Sheet1.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Range;
            this.fpSpread2_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Cell;
            this.fpSpread2_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread2_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.fpSpread2_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Auto;
            this.fpSpread2_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // fpSpread2_Sheet2
            // 
            this.fpSpread2_Sheet2.Reset();
            this.fpSpread2_Sheet2.SheetName = "已退非药品(F8)";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread2_Sheet2.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread2_Sheet2.ColumnCount = 4;
            this.fpSpread2_Sheet2.RowCount = 5;
            this.fpSpread2_Sheet2.ActiveColumnIndex = 2;
            this.fpSpread2_Sheet2.ColumnHeader.Cells.Get(0, 0).Value = "已退非药品名称";
            this.fpSpread2_Sheet2.ColumnHeader.Cells.Get(0, 1).Value = "数量";
            this.fpSpread2_Sheet2.ColumnHeader.Cells.Get(0, 2).Value = "单位";
            this.fpSpread2_Sheet2.ColumnHeader.Cells.Get(0, 3).Value = "标志";
            this.fpSpread2_Sheet2.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread2_Sheet2.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread2_Sheet2.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpSpread2_Sheet2.DefaultStyle.Parent = "DataAreaDefault";
            this.fpSpread2_Sheet2.OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
            this.fpSpread2_Sheet2.RowHeader.Columns.Default.Resizable = true;
            this.fpSpread2_Sheet2.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread2_Sheet2.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpSpread2_Sheet2.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Range;
            this.fpSpread2_Sheet2.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Cell;
            this.fpSpread2_Sheet2.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpSpread2_Sheet2.SheetCornerStyle.Parent = "CornerDefault";
            this.fpSpread2_Sheet2.VisualStyles = FarPoint.Win.VisualStyles.Auto;
            this.fpSpread2_Sheet2.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // tpFee
            // 
            this.tpFee.Size = new System.Drawing.Size(970, 551);
            // 
            // ucQuitItemApply
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ucQuitItemApply";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet2)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet2)).EndInit();
            this.neuTabControl1.ResumeLayout(false);
            this.tpQuit.ResumeLayout(false);
            this.ResumeLayout(false);

            this.fpSpread1_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpSpread1_Sheet2.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpSpread2_Sheet2.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpSpread2_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
        }

        #endregion
    }
}
