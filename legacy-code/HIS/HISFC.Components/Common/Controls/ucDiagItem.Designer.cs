namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class ucDiagItem
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
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            this.neuFpEnter1 = new Neusoft.FrameWork.WinForms.Controls.NeuFpEnter(this.components);
            this.neuFpEnter1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            ((System.ComponentModel.ISupportInitialize)(this.neuFpEnter1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuFpEnter1_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // neuFpEnter1
            // 
            this.neuFpEnter1.About = "3.0.2004.2005";
            this.neuFpEnter1.AccessibleDescription = "neuFpEnter1, Sheet1, Row 0, Column 0, ";
            this.neuFpEnter1.BackColor = System.Drawing.SystemColors.Control;
            this.neuFpEnter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuFpEnter1.EditModePermanent = true;
            this.neuFpEnter1.EditModeReplace = true;
            this.neuFpEnter1.Location = new System.Drawing.Point(0, 0);
            this.neuFpEnter1.Name = "neuFpEnter1";
            this.neuFpEnter1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuFpEnter1.SelectNone = false;
            this.neuFpEnter1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuFpEnter1_Sheet1});
            this.neuFpEnter1.ShowListWhenOfFocus = false;
            this.neuFpEnter1.Size = new System.Drawing.Size(929, 434);
            this.neuFpEnter1.TabIndex = 0;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.neuFpEnter1.TextTipAppearance = tipAppearance1;
            this.neuFpEnter1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.neuFpEnter1_CellDoubleClick);
            // 
            // neuFpEnter1_Sheet1
            // 
            this.neuFpEnter1_Sheet1.Reset();
            this.neuFpEnter1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.neuFpEnter1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.neuFpEnter1_Sheet1.ColumnCount = 8;
            this.neuFpEnter1_Sheet1.RowCount = 0;
            this.neuFpEnter1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "院内诊断编码";
            this.neuFpEnter1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "院内诊断名称";
            this.neuFpEnter1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "ICD编码";
            this.neuFpEnter1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "ICD名称";
            this.neuFpEnter1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "诊断科室";
            this.neuFpEnter1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "诊断医师";
            this.neuFpEnter1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "诊断时间";
            this.neuFpEnter1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "有效";
            this.neuFpEnter1_Sheet1.Columns.Get(0).Label = "院内诊断编码";
            this.neuFpEnter1_Sheet1.Columns.Get(0).Width = 0F;
            this.neuFpEnter1_Sheet1.Columns.Get(1).Label = "院内诊断名称";
            this.neuFpEnter1_Sheet1.Columns.Get(1).Width = 265F;
            this.neuFpEnter1_Sheet1.Columns.Get(2).Label = "ICD编码";
            this.neuFpEnter1_Sheet1.Columns.Get(2).Width = 91F;
            this.neuFpEnter1_Sheet1.Columns.Get(3).Label = "ICD名称";
            this.neuFpEnter1_Sheet1.Columns.Get(3).Width = 175F;
            this.neuFpEnter1_Sheet1.Columns.Get(4).Label = "诊断科室";
            this.neuFpEnter1_Sheet1.Columns.Get(4).Width = 121F;
            this.neuFpEnter1_Sheet1.Columns.Get(5).Label = "诊断医师";
            this.neuFpEnter1_Sheet1.Columns.Get(5).Width = 94F;
            this.neuFpEnter1_Sheet1.Columns.Get(6).Label = "诊断时间";
            this.neuFpEnter1_Sheet1.Columns.Get(6).Width = 158F;
            this.neuFpEnter1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.neuFpEnter1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.neuFpEnter1_Sheet1.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.neuFpEnter1_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.neuFpEnter1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.neuFpEnter1.SetActiveViewport(0, 1, 0);
            // 
            // ucDiagItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.neuFpEnter1);
            this.Name = "ucDiagItem";
            this.Size = new System.Drawing.Size(929, 434);
            ((System.ComponentModel.ISupportInitialize)(this.neuFpEnter1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuFpEnter1_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuFpEnter neuFpEnter1;
        private FarPoint.Win.Spread.SheetView neuFpEnter1_Sheet1;
    }
}
