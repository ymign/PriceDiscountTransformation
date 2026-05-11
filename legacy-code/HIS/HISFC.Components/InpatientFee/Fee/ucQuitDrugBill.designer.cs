namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    partial class ucQuitDrugBill
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
            //退费单字体大小统一设置
            const float FontSize = 10.5F;

            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbRePrint = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.labPrintDate = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.labReason = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.labRemark = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.labArea = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbCardNo = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbDeptName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbSex = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuPanName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(815, 308);
            this.panel1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 77);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(815, 231);
            this.panel3.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.neuSpread1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(815, 231);
            this.panel5.TabIndex = 3;
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
            this.neuSpread1.Size = new System.Drawing.Size(815, 231);
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSpread1.TabIndex = 1;
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
            this.neuSpread1_Sheet1.ColumnCount = 10;
            this.neuSpread1_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin1", System.Drawing.Color.White, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.White, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, false, false, false, true, true);
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).BackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "项目名称";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "规格";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "数量";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "单位";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "金额";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "方号";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "来源";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "备注";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "应执行时间";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 9).Value = "申请日期";
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.Columns.Get(0).BackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.Columns.Get(0).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "项目名称";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 214F;
            this.neuSpread1_Sheet1.Columns.Get(1).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "规格";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 100F;
            this.neuSpread1_Sheet1.Columns.Get(2).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(2).Label = "数量";
            this.neuSpread1_Sheet1.Columns.Get(2).Width = 37F;
            this.neuSpread1_Sheet1.Columns.Get(3).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "单位";
            this.neuSpread1_Sheet1.Columns.Get(3).Width = 33F;
            this.neuSpread1_Sheet1.Columns.Get(4).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "金额";
            this.neuSpread1_Sheet1.Columns.Get(4).Width = 59F;
            this.neuSpread1_Sheet1.Columns.Get(5).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(5).Label = "方号";
            this.neuSpread1_Sheet1.Columns.Get(5).Width = 52F;
            this.neuSpread1_Sheet1.Columns.Get(6).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(6).Label = "来源";
            this.neuSpread1_Sheet1.Columns.Get(6).Visible = false;
            this.neuSpread1_Sheet1.Columns.Get(6).Width = 108F;
            this.neuSpread1_Sheet1.Columns.Get(7).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(7).Label = "备注";
            this.neuSpread1_Sheet1.Columns.Get(7).Width = 83F;
            this.neuSpread1_Sheet1.Columns.Get(8).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(8).Label = "应执行时间";
            this.neuSpread1_Sheet1.Columns.Get(8).Width = 130F;
            this.neuSpread1_Sheet1.Columns.Get(9).Font = new System.Drawing.Font("宋体", FontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(9).Label = "申请日期";
            this.neuSpread1_Sheet1.Columns.Get(9).Width = 83F;
            this.neuSpread1_Sheet1.GroupBarBackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.neuSpread1_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.lbRePrint);
            this.panel2.Controls.Add(this.labPrintDate);
            this.panel2.Controls.Add(this.labReason);
            this.panel2.Controls.Add(this.labRemark);
            this.panel2.Controls.Add(this.labArea);
            this.panel2.Controls.Add(this.lbCardNo);
            this.panel2.Controls.Add(this.lbDeptName);
            this.panel2.Controls.Add(this.lbSex);
            this.panel2.Controls.Add(this.lbName);
            this.panel2.Controls.Add(this.neuPanName);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(815, 77);
            this.panel2.TabIndex = 0;
            // 
            // lbRePrint
            // 
            this.lbRePrint.AutoSize = true;
            this.lbRePrint.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold);
            this.lbRePrint.Location = new System.Drawing.Point(231, 5);
            this.lbRePrint.Name = "lbRePrint";
            this.lbRePrint.Size = new System.Drawing.Size(29, 19);
            this.lbRePrint.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbRePrint.TabIndex = 22;
            this.lbRePrint.Text = "补";
            // 
            // labPrintDate
            // 
            this.labPrintDate.AutoSize = true;
            this.labPrintDate.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labPrintDate.Location = new System.Drawing.Point(615, 34);
            this.labPrintDate.Name = "labPrintDate";
            this.labPrintDate.Size = new System.Drawing.Size(77, 14);
            this.labPrintDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.labPrintDate.TabIndex = 21;
            this.labPrintDate.Text = "打印日期 :";
            // 
            // labReason
            // 
            this.labReason.AutoSize = true;
            this.labReason.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labReason.Location = new System.Drawing.Point(3, 56);
            this.labReason.Name = "labReason";
            this.labReason.Size = new System.Drawing.Size(77, 14);
            this.labReason.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.labReason.TabIndex = 20;
            this.labReason.Text = "退费原因 :";
            // 
            // labRemark
            // 
            this.labRemark.AutoSize = true;
            this.labRemark.Font = new System.Drawing.Font("宋体", 9F);
            this.labRemark.Location = new System.Drawing.Point(299, 58);
            this.labRemark.Name = "labRemark";
            this.labRemark.Size = new System.Drawing.Size(485, 12);
            this.labRemark.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.labRemark.TabIndex = 19;
            this.labRemark.Text = "注：护长签名并盖章，备注为送药房的项目须药房签名盖章，检验、检查项目须拿回原验单";
            // 
            // labArea
            // 
            this.labArea.AutoSize = true;
            this.labArea.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labArea.Location = new System.Drawing.Point(358, 34);
            this.labArea.Name = "labArea";
            this.labArea.Size = new System.Drawing.Size(49, 14);
            this.labArea.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.labArea.TabIndex = 18;
            this.labArea.Text = "病区 :";
            // 
            // lbCardNo
            // 
            this.lbCardNo.AutoSize = true;
            this.lbCardNo.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbCardNo.Location = new System.Drawing.Point(128, 34);
            this.lbCardNo.Name = "lbCardNo";
            this.lbCardNo.Size = new System.Drawing.Size(68, 14);
            this.lbCardNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbCardNo.TabIndex = 17;
            this.lbCardNo.Text = "住院号 :";
            // 
            // lbDeptName
            // 
            this.lbDeptName.AutoSize = true;
            this.lbDeptName.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbDeptName.Location = new System.Drawing.Point(472, 34);
            this.lbDeptName.Name = "lbDeptName";
            this.lbDeptName.Size = new System.Drawing.Size(77, 14);
            this.lbDeptName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbDeptName.TabIndex = 14;
            this.lbDeptName.Text = "科室名称 :";
            // 
            // lbSex
            // 
            this.lbSex.AutoSize = true;
            this.lbSex.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbSex.Location = new System.Drawing.Point(275, 34);
            this.lbSex.Name = "lbSex";
            this.lbSex.Size = new System.Drawing.Size(49, 14);
            this.lbSex.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbSex.TabIndex = 12;
            this.lbSex.Text = "性别 :";
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbName.Location = new System.Drawing.Point(3, 34);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(53, 14);
            this.lbName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbName.TabIndex = 11;
            this.lbName.Text = "姓名 :";
            // 
            // neuPanName
            // 
            this.neuPanName.AutoSize = true;
            this.neuPanName.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuPanName.Location = new System.Drawing.Point(329, 0);
            this.neuPanName.Name = "neuPanName";
            this.neuPanName.Size = new System.Drawing.Size(185, 24);
            this.neuPanName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanName.TabIndex = 1;
            this.neuPanName.Text = "住院退费申请单";
            // 
            // ucQuitDrugBill
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.Name = "ucQuitDrugBill";
            this.Size = new System.Drawing.Size(815, 308);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbCardNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbDeptName;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbSex;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbName;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuPanName;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private System.Windows.Forms.Panel panel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel labReason;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel labRemark;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel labArea;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel labPrintDate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbRePrint;
    }
}
