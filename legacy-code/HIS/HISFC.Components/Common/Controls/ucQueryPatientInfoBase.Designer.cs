namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class ucQueryPatientInfoBase
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
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("zh-CN", false);
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
            this.cmbQueryType = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxAccurate = new System.Windows.Forms.CheckBox();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtQueryInfo = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbQueryType
            // 
            this.cmbQueryType.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbQueryType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbQueryType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQueryType.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbQueryType.FormattingEnabled = true;
            this.cmbQueryType.IsEnter2Tab = false;
            this.cmbQueryType.IsFlat = false;
            this.cmbQueryType.IsLike = true;
            this.cmbQueryType.IsListOnly = false;
            this.cmbQueryType.IsPopForm = true;
            this.cmbQueryType.IsShowCustomerList = false;
            this.cmbQueryType.IsShowID = false;
            this.cmbQueryType.IsShowIDAndName = false;
            this.cmbQueryType.Items.AddRange(new object[] {
            "门诊号",
            "住院号",
            "姓名"});
            this.cmbQueryType.Location = new System.Drawing.Point(83, 15);
            this.cmbQueryType.Name = "cmbQueryType";
            this.cmbQueryType.ShowCustomerList = false;
            this.cmbQueryType.ShowID = false;
            this.cmbQueryType.Size = new System.Drawing.Size(86, 24);
            this.cmbQueryType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbQueryType.TabIndex = 2;
            this.cmbQueryType.Tag = "";
            this.cmbQueryType.ToolBarUse = false;
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel2.Location = new System.Drawing.Point(6, 20);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(72, 16);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 3;
            this.neuLabel2.Text = "查询类别";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(238)))), ((int)(((byte)(230)))));
            this.groupBox1.Controls.Add(this.cbxAccurate);
            this.groupBox1.Controls.Add(this.neuLabel1);
            this.groupBox1.Controls.Add(this.txtQueryInfo);
            this.groupBox1.Controls.Add(this.cmbQueryType);
            this.groupBox1.Controls.Add(this.neuLabel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(757, 59);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // cbxAccurate
            // 
            this.cbxAccurate.AutoSize = true;
            this.cbxAccurate.Checked = true;
            this.cbxAccurate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxAccurate.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxAccurate.Location = new System.Drawing.Point(566, 17);
            this.cbxAccurate.Name = "cbxAccurate";
            this.cbxAccurate.Size = new System.Drawing.Size(91, 20);
            this.cbxAccurate.TabIndex = 6;
            this.cbxAccurate.Text = "精确查找";
            this.cbxAccurate.UseVisualStyleBackColor = true;
            this.cbxAccurate.CheckedChanged += new System.EventHandler(this.cbxAccurate_CheckedChanged);
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel1.Location = new System.Drawing.Point(187, 20);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(72, 16);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 5;
            this.neuLabel1.Text = "查询内容";
            // 
            // txtQueryInfo
            // 
            this.txtQueryInfo.Font = new System.Drawing.Font("宋体", 12F);
            this.txtQueryInfo.Location = new System.Drawing.Point(265, 13);
            this.txtQueryInfo.Name = "txtQueryInfo";
            this.txtQueryInfo.Size = new System.Drawing.Size(281, 26);
            this.txtQueryInfo.TabIndex = 4;
            this.txtQueryInfo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtQueryInfo_KeyDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.neuSpread1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 59);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(757, 305);
            this.panel1.TabIndex = 5;
            // 
            // neuSpread1
            // 
            this.neuSpread1.About = "3.0.2004.2005";
            this.neuSpread1.AccessibleDescription = "neuSpread1, Sheet1, Row 0, Column 0, ";
            this.neuSpread1.BackColor = System.Drawing.Color.White;
            this.neuSpread1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.neuSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuSpread1.FileName = "";
            this.neuSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.IsAutoSaveGridStatus = false;
            this.neuSpread1.IsCanCustomConfigColumn = false;
            this.neuSpread1.Location = new System.Drawing.Point(0, 0);
            this.neuSpread1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuSpread1.SelectionBlockOptions = FarPoint.Win.Spread.SelectionBlockOptions.Rows;
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1});
            this.neuSpread1.Size = new System.Drawing.Size(757, 305);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.VS2003;
            this.neuSpread1.TabIndex = 5;
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
            this.neuSpread1_Sheet1.ColumnCount = 11;
            this.neuSpread1_Sheet1.RowCount = 1;
            this.neuSpread1_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("MrsStyle", System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(254)))), ((int)(((byte)(250))))), System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(238)))), ((int)(((byte)(230))))), System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(230)))), ((int)(((byte)(191))))), System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238))))), false, false, false, true, false);
            this.neuSpread1_Sheet1.Cells.Get(0, 6).ParseFormatInfo = ((System.Globalization.NumberFormatInfo)(cultureInfo.NumberFormat.Clone()));
            ((System.Globalization.NumberFormatInfo)(this.neuSpread1_Sheet1.Cells.Get(0, 6).ParseFormatInfo)).CurrencySymbol = "¥";
            ((System.Globalization.NumberFormatInfo)(this.neuSpread1_Sheet1.Cells.Get(0, 6).ParseFormatInfo)).NumberDecimalDigits = 0;
            ((System.Globalization.NumberFormatInfo)(this.neuSpread1_Sheet1.Cells.Get(0, 6).ParseFormatInfo)).NumberGroupSizes = new int[] {
        0};
            this.neuSpread1_Sheet1.Cells.Get(0, 6).ParseFormatString = "E";
            this.neuSpread1_Sheet1.Cells.Get(0, 6).Value = "372922198501262877";
            this.neuSpread1_Sheet1.Cells.Get(0, 7).Value = "广东省深圳市福田区海园一路1号 (白石路与侨城东路交汇) 香港大学深圳医院科教管理楼1417房";
            this.neuSpread1_Sheet1.Cells.Get(0, 10).ParseFormatInfo = ((System.Globalization.NumberFormatInfo)(cultureInfo.NumberFormat.Clone()));
            ((System.Globalization.NumberFormatInfo)(this.neuSpread1_Sheet1.Cells.Get(0, 10).ParseFormatInfo)).CurrencySymbol = "¥";
            ((System.Globalization.NumberFormatInfo)(this.neuSpread1_Sheet1.Cells.Get(0, 10).ParseFormatInfo)).NumberDecimalDigits = 0;
            ((System.Globalization.NumberFormatInfo)(this.neuSpread1_Sheet1.Cells.Get(0, 10).ParseFormatInfo)).NumberGroupSizes = new int[] {
        0};
            this.neuSpread1_Sheet1.Cells.Get(0, 10).ParseFormatString = "n";
            this.neuSpread1_Sheet1.Cells.Get(0, 10).Value = ((long)(13822278023));
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "病人号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "门诊号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "住院号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "姓名";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "性别";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "年龄";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "证件号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "地址";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "电话";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 9).Value = "工作单位";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 10).Value = "单位电话";
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(238)))), ((int)(((byte)(230)))));
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.ColumnHeader.HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(189)))), ((int)(((byte)(166))))));
            this.neuSpread1_Sheet1.ColumnHeader.Rows.Get(0).Height = 30F;
            this.neuSpread1_Sheet1.ColumnHeader.VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(189)))), ((int)(((byte)(166))))));
            this.neuSpread1_Sheet1.Columns.Default.NoteIndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "病人号";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 80F;
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "门诊号";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 80F;
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "姓名";
            this.neuSpread1_Sheet1.Columns.Get(3).Width = 90F;
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "性别";
            this.neuSpread1_Sheet1.Columns.Get(4).Width = 35F;
            this.neuSpread1_Sheet1.Columns.Get(5).Label = "年龄";
            this.neuSpread1_Sheet1.Columns.Get(5).Width = 41F;
            this.neuSpread1_Sheet1.Columns.Get(6).CellType = textCellType1;
            this.neuSpread1_Sheet1.Columns.Get(6).Label = "证件号";
            this.neuSpread1_Sheet1.Columns.Get(6).Width = 122F;
            this.neuSpread1_Sheet1.Columns.Get(7).CellType = textCellType2;
            this.neuSpread1_Sheet1.Columns.Get(7).Label = "地址";
            this.neuSpread1_Sheet1.Columns.Get(7).Width = 250F;
            numberCellType1.DecimalPlaces = 0;
            this.neuSpread1_Sheet1.Columns.Get(8).CellType = numberCellType1;
            this.neuSpread1_Sheet1.Columns.Get(8).Label = "电话";
            this.neuSpread1_Sheet1.Columns.Get(8).Width = 100F;
            this.neuSpread1_Sheet1.Columns.Get(9).Label = "工作单位";
            this.neuSpread1_Sheet1.Columns.Get(9).Width = 250F;
            this.neuSpread1_Sheet1.Columns.Get(10).Label = "单位电话";
            this.neuSpread1_Sheet1.Columns.Get(10).Width = 100F;
            this.neuSpread1_Sheet1.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.neuSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(238)))), ((int)(((byte)(230)))));
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.RowHeader.Visible = false;
            this.neuSpread1_Sheet1.Rows.Default.NoteIndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.neuSpread1_Sheet1.SelectionStyle = FarPoint.Win.Spread.SelectionStyles.SelectionColors;
            this.neuSpread1_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(238)))), ((int)(((byte)(230)))));
            this.neuSpread1_Sheet1.SheetCornerStyle.NoteIndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(230)))), ((int)(((byte)(219)))));
            this.neuSpread1_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.neuSpread1_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // ucQueryPatientInfoBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "ucQueryPatientInfoBase";
            this.Size = new System.Drawing.Size(757, 364);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbQueryType;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private System.Windows.Forms.TextBox txtQueryInfo;
        private System.Windows.Forms.CheckBox cbxAccurate;
    }
}
