namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucItemZT
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
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucItemZT));
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtFileter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.neuTabControl1 = new Neusoft.FrameWork.WinForms.Controls.NeuTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ucInputItem1 = new Neusoft.HISFC.Components.Common.Controls.ucInputItem();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.panel1.SuspendLayout();
            this.neuTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Controls.Add(this.neuTabControl1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1020, 628);
            this.panel3.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 198);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1020, 430);
            this.panel2.TabIndex = 7;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.neuSpread1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 75);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1020, 355);
            this.panel4.TabIndex = 3;
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
            this.neuSpread1.Size = new System.Drawing.Size(1020, 355);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 2;
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
            this.neuSpread1_Sheet1.ColumnCount = 12;
            this.neuSpread1_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "序号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "类型";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "系统编码";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "组合编码";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "组合名称";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "明细";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "数量";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "取整规则";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "公式";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 9).Value = "有效";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 10).Value = "操作员";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 11).Value = "操作时间";
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.ColumnHeader.Rows.Get(0).Height = 35F;
            this.neuSpread1_Sheet1.Columns.Get(0).CellType = textCellType1;
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "序号";
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "类型";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 48F;
            this.neuSpread1_Sheet1.Columns.Get(2).Label = "系统编码";
            this.neuSpread1_Sheet1.Columns.Get(2).Width = 82F;
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "组合编码";
            this.neuSpread1_Sheet1.Columns.Get(3).Width = 96F;
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "组合名称";
            this.neuSpread1_Sheet1.Columns.Get(4).Width = 110F;
            this.neuSpread1_Sheet1.Columns.Get(5).Label = "明细";
            this.neuSpread1_Sheet1.Columns.Get(5).Width = 110F;
            this.neuSpread1_Sheet1.Columns.Get(9).CellType = checkBoxCellType1;
            this.neuSpread1_Sheet1.Columns.Get(9).Label = "有效";
            this.neuSpread1_Sheet1.Columns.Get(9).Width = 34F;
            this.neuSpread1_Sheet1.Columns.Get(10).CellType = textCellType2;
            this.neuSpread1_Sheet1.Columns.Get(10).Label = "操作员";
            this.neuSpread1_Sheet1.Columns.Get(10).Width = 66F;
            this.neuSpread1_Sheet1.Columns.Get(11).Label = "操作时间";
            this.neuSpread1_Sheet1.Columns.Get(11).Width = 123F;
            this.neuSpread1_Sheet1.DataAutoSizeColumns = false;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.Rows.Default.Height = 30F;
            this.neuSpread1_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.neuSpread1_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.neuSpread1_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtFileter);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1020, 75);
            this.panel1.TabIndex = 2;
            // 
            // txtFileter
            // 
            this.txtFileter.Location = new System.Drawing.Point(74, 31);
            this.txtFileter.Name = "txtFileter";
            this.txtFileter.Size = new System.Drawing.Size(270, 21);
            this.txtFileter.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(24, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "过滤：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(70, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(626, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "注意：只支持一人操作维护，两人同时维护会导致数据相互覆盖";
            // 
            // neuTabControl1
            // 
            this.neuTabControl1.Controls.Add(this.tabPage1);
            this.neuTabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuTabControl1.Location = new System.Drawing.Point(0, 0);
            this.neuTabControl1.Name = "neuTabControl1";
            this.neuTabControl1.SelectedIndex = 0;
            this.neuTabControl1.Size = new System.Drawing.Size(1020, 198);
            this.neuTabControl1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuTabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ucInputItem1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1012, 172);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "用法";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ucInputItem1
            // 
            this.ucInputItem1.AlCatagory = ((System.Collections.ArrayList)(resources.GetObject("ucInputItem1.AlCatagory")));
            this.ucInputItem1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucInputItem1.DrugSendType = "A";
            this.ucInputItem1.FeeItem = ((Neusoft.FrameWork.Models.NeuObject)(resources.GetObject("ucInputItem1.FeeItem")));
            this.ucInputItem1.FontSize = null;
            this.ucInputItem1.InputType = 0;
            this.ucInputItem1.IsDeptUsedFlag = false;
            this.ucInputItem1.IsIncludeMat = false;
            this.ucInputItem1.IsListShowAlways = true;
            this.ucInputItem1.IsShowCategory = false;
            this.ucInputItem1.IsShowInput = true;
            this.ucInputItem1.IsShowSelfMark = true;
            this.ucInputItem1.Location = new System.Drawing.Point(3, 3);
            this.ucInputItem1.Name = "ucInputItem1";
            //this.ucInputItem1.PactInfo = null;
            this.ucInputItem1.Patient = null;
            this.ucInputItem1.ShowCategory = Neusoft.HISFC.Components.Common.Controls.EnumCategoryType.SysClass;
            this.ucInputItem1.ShowItemType = Neusoft.HISFC.Components.Common.Controls.EnumShowItemType.Undrug;
            this.ucInputItem1.Size = new System.Drawing.Size(1006, 166);
            this.ucInputItem1.TabIndex = 0;
            this.ucInputItem1.UndrugApplicabilityarea = Neusoft.HISFC.Components.Common.Controls.EnumUndrugApplicabilityarea.All;
            this.ucInputItem1.SelectedItem += new Neusoft.FrameWork.WinForms.Forms.SelectedItemHandler(this.ucInputItem1_SelectedItem);
            // 
            // ucItemZT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Name = "ucItemZT";
            this.Size = new System.Drawing.Size(1020, 628);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.neuTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtFileter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTabControl neuTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Neusoft.HISFC.Components.Common.Controls.ucInputItem ucInputItem1;
        private System.Windows.Forms.Panel panel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;


    }
}
