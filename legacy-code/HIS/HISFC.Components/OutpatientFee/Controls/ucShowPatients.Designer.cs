namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    /// <summary>
    /// ucShowPatients<br></br>
    /// [功能描述: 输入的卡号多于一个患者选择患者UC]<br></br>
    /// [创 建 者: 王宇]<br></br>
    /// [创建时间: 2006-2-28]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    partial class ucShowPatients
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
            FarPoint.Win.Spread.CellType.TextCellType textCellType7 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType8 = new FarPoint.Win.Spread.CellType.TextCellType();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.btnCreateNew = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.pnlBottom = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.pnlBottom.SuspendLayout();
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
            this.neuSpread1.Location = new System.Drawing.Point(0, 0);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1});
            this.neuSpread1.Size = new System.Drawing.Size(574, 185);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 0;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.neuSpread1.TextTipAppearance = tipAppearance1;
            this.neuSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.neuSpread1_CellDoubleClick);
            this.neuSpread1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.neuSpread1_KeyDown);
            // 
            // neuSpread1_Sheet1
            // 
            this.neuSpread1_Sheet1.Reset();
            this.neuSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.neuSpread1_Sheet1.ColumnCount = 8;
            this.neuSpread1_Sheet1.RowCount = 1;
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "处方号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "卡号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "姓名";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "挂号科室";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "挂号级别";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "挂号时间";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "有效性";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "医生名";
            this.neuSpread1_Sheet1.Columns.Get(0).CellType = textCellType1;
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "处方号";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 67F;
            this.neuSpread1_Sheet1.Columns.Get(1).CellType = textCellType2;
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "卡号";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 81F;
            this.neuSpread1_Sheet1.Columns.Get(2).CellType = textCellType3;
            this.neuSpread1_Sheet1.Columns.Get(2).Label = "姓名";
            this.neuSpread1_Sheet1.Columns.Get(3).CellType = textCellType4;
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "挂号科室";
            this.neuSpread1_Sheet1.Columns.Get(4).CellType = textCellType5;
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "挂号级别";
            this.neuSpread1_Sheet1.Columns.Get(5).CellType = textCellType6;
            this.neuSpread1_Sheet1.Columns.Get(5).Label = "挂号时间";
            this.neuSpread1_Sheet1.Columns.Get(6).CellType = textCellType7;
            this.neuSpread1_Sheet1.Columns.Get(6).Label = "有效性";
            this.neuSpread1_Sheet1.Columns.Get(7).CellType = textCellType8;
            this.neuSpread1_Sheet1.Columns.Get(7).Label = "医生名";
            this.neuSpread1_Sheet1.Columns.Get(7).Width = 80F;
            this.neuSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.neuSpread1_Sheet1.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.neuSpread1_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // btnCreateNew
            // 
            this.btnCreateNew.Location = new System.Drawing.Point(204, 3);
            this.btnCreateNew.Name = "btnCreateNew";
            this.btnCreateNew.Size = new System.Drawing.Size(103, 23);
            this.btnCreateNew.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnCreateNew.TabIndex = 1;
            this.btnCreateNew.Text = "补挂号";
            this.btnCreateNew.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnCreateNew.UseVisualStyleBackColor = true;
            this.btnCreateNew.Click += new System.EventHandler(this.btnCreateNew_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnCreateNew);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 185);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(574, 30);
            this.pnlBottom.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.pnlBottom.TabIndex = 2;
            // 
            // ucShowPatients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.neuSpread1);
            this.Controls.Add(this.pnlBottom);
            this.Name = "ucShowPatients";
            this.Size = new System.Drawing.Size(574, 215);
            this.Enter += new System.EventHandler(this.ucShowPatients_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnCreateNew;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel pnlBottom;
    }
}
