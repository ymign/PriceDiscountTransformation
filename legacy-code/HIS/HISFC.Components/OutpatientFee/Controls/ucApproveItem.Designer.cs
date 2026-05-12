namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucApproveItem
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
            FarPoint.Win.Spread.TipAppearance tipAppearance2 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType2 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType3 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType4 = new FarPoint.Win.Spread.CellType.NumberCellType();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.fpFeeDetail = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.fpFeeDetail_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnModifys = new System.Windows.Forms.Button();
            this.ckChecked = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.fpFeeDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpFeeDetail_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOk.Location = new System.Drawing.Point(753, 331);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "确定(&O)";
            // 
            // btnExport
            // 
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExport.Location = new System.Drawing.Point(672, 331);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 14;
            this.btnExport.Text = "导出(&S)";
            // 
            // fpFeeDetail
            // 
            this.fpFeeDetail.About = "3.0.2004.2005";
            this.fpFeeDetail.AccessibleDescription = "fpFeeDetail, 项目, Row 0, Column 0, ";
            this.fpFeeDetail.BackColor = System.Drawing.Color.White;
            this.fpFeeDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.fpFeeDetail.FileName = "";
            this.fpFeeDetail.Font = new System.Drawing.Font("宋体", 9F);
            this.fpFeeDetail.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpFeeDetail.IsAutoSaveGridStatus = false;
            this.fpFeeDetail.IsCanCustomConfigColumn = false;
            this.fpFeeDetail.Location = new System.Drawing.Point(0, 0);
            this.fpFeeDetail.Name = "fpFeeDetail";
            this.fpFeeDetail.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpFeeDetail.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpFeeDetail_Sheet1});
            this.fpFeeDetail.Size = new System.Drawing.Size(831, 325);
            this.fpFeeDetail.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.fpFeeDetail.TabIndex = 101;
            this.fpFeeDetail.TabStrip.ButtonPolicy = FarPoint.Win.Spread.TabStripButtonPolicy.AsNeeded;
            this.fpFeeDetail.TabStrip.DefaultSheetTab.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fpFeeDetail.TabStrip.DefaultSheetTab.Size = -1;
            this.fpFeeDetail.TabStripPlacement = FarPoint.Win.Spread.TabStripPlacement.Top;
            tipAppearance2.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpFeeDetail.TextTipAppearance = tipAppearance2;
            this.fpFeeDetail.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // fpFeeDetail_Sheet1
            // 
            this.fpFeeDetail_Sheet1.Reset();
            this.fpFeeDetail_Sheet1.SheetName = "项目";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpFeeDetail_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpFeeDetail_Sheet1.ColumnCount = 11;
            this.fpFeeDetail_Sheet1.RowCount = 0;
            this.fpFeeDetail_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin2", System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.Gainsboro, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240))))), System.Drawing.Color.Empty, false, false, false, true, true);
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "多选";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "自定义码";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "项目名称";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "组套名称";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "审批信息";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "价格";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "数量";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "单位";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "总金额";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 9).Value = "拼音码";
            this.fpFeeDetail_Sheet1.ColumnHeader.Cells.Get(0, 10).Value = "五笔码";
            this.fpFeeDetail_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpFeeDetail_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpFeeDetail_Sheet1.ColumnHeader.Rows.Get(0).Height = 30F;
            this.fpFeeDetail_Sheet1.Columns.Get(0).CellType = checkBoxCellType2;
            this.fpFeeDetail_Sheet1.Columns.Get(0).Label = "多选";
            this.fpFeeDetail_Sheet1.Columns.Get(0).Width = 22F;
            this.fpFeeDetail_Sheet1.Columns.Get(2).AllowAutoSort = true;
            this.fpFeeDetail_Sheet1.Columns.Get(2).Label = "项目名称";
            this.fpFeeDetail_Sheet1.Columns.Get(2).Locked = true;
            this.fpFeeDetail_Sheet1.Columns.Get(2).Width = 256F;
            this.fpFeeDetail_Sheet1.Columns.Get(3).AllowAutoSort = true;
            this.fpFeeDetail_Sheet1.Columns.Get(3).Label = "组套名称";
            this.fpFeeDetail_Sheet1.Columns.Get(3).Locked = true;
            this.fpFeeDetail_Sheet1.Columns.Get(3).Width = 94F;
            this.fpFeeDetail_Sheet1.Columns.Get(4).AllowAutoSort = true;
            this.fpFeeDetail_Sheet1.Columns.Get(4).CellType = textCellType2;
            this.fpFeeDetail_Sheet1.Columns.Get(4).Label = "审批信息";
            this.fpFeeDetail_Sheet1.Columns.Get(4).Locked = true;
            this.fpFeeDetail_Sheet1.Columns.Get(4).Width = 173F;
            this.fpFeeDetail_Sheet1.Columns.Get(5).AllowAutoSort = true;
            this.fpFeeDetail_Sheet1.Columns.Get(5).CellType = numberCellType3;
            this.fpFeeDetail_Sheet1.Columns.Get(5).Label = "价格";
            this.fpFeeDetail_Sheet1.Columns.Get(5).Locked = true;
            this.fpFeeDetail_Sheet1.Columns.Get(5).Width = 55F;
            this.fpFeeDetail_Sheet1.Columns.Get(6).CellType = numberCellType4;
            this.fpFeeDetail_Sheet1.Columns.Get(6).Label = "数量";
            this.fpFeeDetail_Sheet1.Columns.Get(6).Locked = true;
            this.fpFeeDetail_Sheet1.Columns.Get(6).Width = 39F;
            this.fpFeeDetail_Sheet1.Columns.Get(7).Label = "单位";
            this.fpFeeDetail_Sheet1.Columns.Get(7).Locked = true;
            this.fpFeeDetail_Sheet1.Columns.Get(7).Width = 39F;
            this.fpFeeDetail_Sheet1.Columns.Get(8).AllowAutoSort = true;
            this.fpFeeDetail_Sheet1.Columns.Get(8).Label = "总金额";
            this.fpFeeDetail_Sheet1.Columns.Get(8).Locked = true;
            this.fpFeeDetail_Sheet1.Columns.Get(8).Width = 69F;
            this.fpFeeDetail_Sheet1.Columns.Get(9).Label = "拼音码";
            this.fpFeeDetail_Sheet1.Columns.Get(9).Visible = false;
            this.fpFeeDetail_Sheet1.Columns.Get(10).Label = "五笔码";
            this.fpFeeDetail_Sheet1.Columns.Get(10).Visible = false;
            this.fpFeeDetail_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
            this.fpFeeDetail_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.fpFeeDetail_Sheet1.RowHeader.Columns.Get(0).Width = 36F;
            this.fpFeeDetail_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpFeeDetail_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.fpFeeDetail_Sheet1.Rows.Default.Height = 25F;
            this.fpFeeDetail_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.Gainsboro;
            this.fpFeeDetail_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.fpFeeDetail_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.fpFeeDetail_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.fpFeeDetail.SetActiveViewport(0, 1, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(16, 336);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(197, 24);
            this.label1.TabIndex = 102;
            this.label1.Text = "请注意：1.红色字体代表已经审批\r\n2.未划价保存的项目不允许批量修改";
            // 
            // btnModifys
            // 
            this.btnModifys.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnModifys.Location = new System.Drawing.Point(591, 331);
            this.btnModifys.Name = "btnModifys";
            this.btnModifys.Size = new System.Drawing.Size(75, 23);
            this.btnModifys.TabIndex = 103;
            this.btnModifys.Text = "批量修改";
            // 
            // ckChecked
            // 
            this.ckChecked.AutoSize = true;
            this.ckChecked.Location = new System.Drawing.Point(329, 335);
            this.ckChecked.Name = "ckChecked";
            this.ckChecked.Size = new System.Drawing.Size(48, 16);
            this.ckChecked.TabIndex = 147;
            this.ckChecked.Text = "全选";
            this.ckChecked.UseVisualStyleBackColor = true;
            // 
            // ucApproveItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ckChecked);
            this.Controls.Add(this.btnModifys);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fpFeeDetail);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnOk);
            this.Name = "ucApproveItem";
            this.Size = new System.Drawing.Size(831, 366);
            ((System.ComponentModel.ISupportInitialize)(this.fpFeeDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpFeeDetail_Sheet1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnExport;
        protected Neusoft.FrameWork.WinForms.Controls.NeuSpread fpFeeDetail;
        private FarPoint.Win.Spread.SheetView fpFeeDetail_Sheet1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnModifys;
        private System.Windows.Forms.CheckBox ckChecked;
    }
}
