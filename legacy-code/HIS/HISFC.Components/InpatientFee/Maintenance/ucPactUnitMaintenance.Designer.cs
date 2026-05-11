namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    partial class ucPactUnitMaintenance
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
            FarPoint.Win.Spread.TipAppearance tipAppearance2 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.TipAppearance tipAppearance3 = new FarPoint.Win.Spread.TipAppearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucPactUnitMaintenance));
            this.neuTabControl1 = new Neusoft.FrameWork.WinForms.Controls.NeuTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.fpMain = new FarPoint.Win.Spread.FpSpread();
            this.fpMain_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.neuSplitter2 = new Neusoft.FrameWork.WinForms.Controls.NeuSplitter();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbQueryValue = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.fpDetail = new FarPoint.Win.Spread.FpSpread();
            this.fpDetail_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.neuSplitter1 = new Neusoft.FrameWork.WinForms.Controls.NeuSplitter();
            this.neuTabControl2 = new Neusoft.FrameWork.WinForms.Controls.NeuTabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.neuPanel2 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.fpFeeCode = new FarPoint.Win.Spread.FpSpread();
            this.fpFeeCode_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.neuPanel3 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.ucInputItem1 = new Neusoft.HISFC.Components.Common.Controls.ucInputItem();
            this.neuTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpMain_Sheet1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.neuPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpDetail_Sheet1)).BeginInit();
            this.neuTabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.neuPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpFeeCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpFeeCode_Sheet1)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.neuPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuTabControl1
            // 
            this.neuTabControl1.Controls.Add(this.tabPage1);
            this.neuTabControl1.Controls.Add(this.tabPage2);
            this.neuTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuTabControl1.Location = new System.Drawing.Point(0, 0);
            this.neuTabControl1.Name = "neuTabControl1";
            this.neuTabControl1.SelectedIndex = 0;
            this.neuTabControl1.Size = new System.Drawing.Size(893, 662);
            this.neuTabControl1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuTabControl1.TabIndex = 0;
            this.neuTabControl1.SelectedIndexChanged += new System.EventHandler(this.neuTabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.fpMain);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(885, 636);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "合同单位基本信息维护";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // fpMain
            // 
            this.fpMain.About = "3.0.2004.2005";
            this.fpMain.AccessibleDescription = "";
            this.fpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpMain.Location = new System.Drawing.Point(3, 3);
            this.fpMain.Name = "fpMain";
            this.fpMain.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpMain_Sheet1});
            this.fpMain.Size = new System.Drawing.Size(879, 630);
            this.fpMain.TabIndex = 0;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpMain.TextTipAppearance = tipAppearance1;
            this.fpMain.EditModeOff += new System.EventHandler(this.fpMain_EditModeOff);
            // 
            // fpMain_Sheet1
            // 
            this.fpMain_Sheet1.Reset();
            this.fpMain_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpMain_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpMain_Sheet1.RowHeader.Columns.Get(0).Width = 37F;
            this.fpMain_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.neuSplitter2);
            this.tabPage2.Controls.Add(this.neuPanel1);
            this.tabPage2.Controls.Add(this.neuTabControl2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(885, 636);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "明细维护";
            // 
            // neuSplitter2
            // 
            this.neuSplitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuSplitter2.Location = new System.Drawing.Point(3, 304);
            this.neuSplitter2.Name = "neuSplitter2";
            this.neuSplitter2.Size = new System.Drawing.Size(879, 3);
            this.neuSplitter2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSplitter2.TabIndex = 2;
            this.neuSplitter2.TabStop = false;
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.panel1);
            this.neuPanel1.Controls.Add(this.fpDetail);
            this.neuPanel1.Controls.Add(this.neuSplitter1);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel1.Location = new System.Drawing.Point(3, 304);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(879, 329);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.tbQueryValue);
            this.panel1.Controls.Add(this.neuLabel1);
            this.panel1.Location = new System.Drawing.Point(0, 3);
            this.panel1.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(879, 30);
            this.panel1.TabIndex = 6;
            // 
            // tbQueryValue
            // 
            this.tbQueryValue.IsEnter2Tab = false;
            this.tbQueryValue.Location = new System.Drawing.Point(58, 2);
            this.tbQueryValue.Name = "tbQueryValue";
            this.tbQueryValue.Size = new System.Drawing.Size(221, 21);
            this.tbQueryValue.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbQueryValue.TabIndex = 5;
            this.tbQueryValue.TextChanged += new System.EventHandler(this.tbQueryValue_TextChanged);
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.neuLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.neuLabel1.Location = new System.Drawing.Point(5, 7);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(47, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 4;
            this.neuLabel1.Text = "输入码:";
            // 
            // fpDetail
            // 
            this.fpDetail.About = "3.0.2004.2005";
            this.fpDetail.AccessibleDescription = "";
            this.fpDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fpDetail.Location = new System.Drawing.Point(0, 39);
            this.fpDetail.Name = "fpDetail";
            this.fpDetail.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpDetail_Sheet1});
            this.fpDetail.Size = new System.Drawing.Size(872, 287);
            this.fpDetail.TabIndex = 1;
            tipAppearance2.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpDetail.TextTipAppearance = tipAppearance2;
            // 
            // fpDetail_Sheet1
            // 
            this.fpDetail_Sheet1.Reset();
            this.fpDetail_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpDetail_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpDetail_Sheet1.RowHeader.Columns.Get(0).Width = 37F;
            this.fpDetail_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // neuSplitter1
            // 
            this.neuSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuSplitter1.Location = new System.Drawing.Point(0, 0);
            this.neuSplitter1.Name = "neuSplitter1";
            this.neuSplitter1.Size = new System.Drawing.Size(879, 3);
            this.neuSplitter1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuSplitter1.TabIndex = 0;
            this.neuSplitter1.TabStop = false;
            // 
            // neuTabControl2
            // 
            this.neuTabControl2.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.neuTabControl2.Controls.Add(this.tabPage3);
            this.neuTabControl2.Controls.Add(this.tabPage4);
            this.neuTabControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuTabControl2.ItemSize = new System.Drawing.Size(65, 17);
            this.neuTabControl2.Location = new System.Drawing.Point(3, 3);
            this.neuTabControl2.Multiline = true;
            this.neuTabControl2.Name = "neuTabControl2";
            this.neuTabControl2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.neuTabControl2.SelectedIndex = 0;
            this.neuTabControl2.Size = new System.Drawing.Size(879, 301);
            this.neuTabControl2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuTabControl2.TabIndex = 0;
            this.neuTabControl2.SelectedIndexChanged += new System.EventHandler(this.neuTabControl2_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Transparent;
            this.tabPage3.Controls.Add(this.neuPanel2);
            this.tabPage3.Location = new System.Drawing.Point(4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(871, 276);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "最小费用";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // neuPanel2
            // 
            this.neuPanel2.BackColor = System.Drawing.SystemColors.Control;
            this.neuPanel2.Controls.Add(this.fpFeeCode);
            this.neuPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel2.Location = new System.Drawing.Point(3, 3);
            this.neuPanel2.Name = "neuPanel2";
            this.neuPanel2.Size = new System.Drawing.Size(865, 270);
            this.neuPanel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel2.TabIndex = 0;
            // 
            // fpFeeCode
            // 
            this.fpFeeCode.About = "3.0.2004.2005";
            this.fpFeeCode.AccessibleDescription = "";
            this.fpFeeCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpFeeCode.Location = new System.Drawing.Point(0, 0);
            this.fpFeeCode.Name = "fpFeeCode";
            this.fpFeeCode.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpFeeCode_Sheet1});
            this.fpFeeCode.Size = new System.Drawing.Size(865, 270);
            this.fpFeeCode.TabIndex = 0;
            tipAppearance3.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance3.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpFeeCode.TextTipAppearance = tipAppearance3;
            this.fpFeeCode.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpFeeCode.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpFeeCode_CellDoubleClick);
            // 
            // fpFeeCode_Sheet1
            // 
            this.fpFeeCode_Sheet1.Reset();
            this.fpFeeCode_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpFeeCode_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpFeeCode_Sheet1.RowHeader.Columns.Get(0).Width = 37F;
            this.fpFeeCode_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Transparent;
            this.tabPage4.Controls.Add(this.neuPanel3);
            this.tabPage4.Location = new System.Drawing.Point(4, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(871, 276);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "收费项目";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // neuPanel3
            // 
            this.neuPanel3.Controls.Add(this.ucInputItem1);
            this.neuPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel3.Location = new System.Drawing.Point(3, 3);
            this.neuPanel3.Name = "neuPanel3";
            this.neuPanel3.Size = new System.Drawing.Size(865, 270);
            this.neuPanel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel3.TabIndex = 0;
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
            this.ucInputItem1.IsShowCategory = true;
            this.ucInputItem1.IsShowInput = true;
            this.ucInputItem1.IsShowSelfMark = true;
            this.ucInputItem1.Location = new System.Drawing.Point(0, 0);
            this.ucInputItem1.Name = "ucInputItem1";
            this.ucInputItem1.Patient = null;
            this.ucInputItem1.ShowCategory = Neusoft.HISFC.Components.Common.Controls.EnumCategoryType.SysClass;
            this.ucInputItem1.ShowItemType = Neusoft.HISFC.Components.Common.Controls.EnumShowItemType.All;
            this.ucInputItem1.Size = new System.Drawing.Size(865, 270);
            this.ucInputItem1.TabIndex = 0;
            this.ucInputItem1.UndrugApplicabilityarea = Neusoft.HISFC.Components.Common.Controls.EnumUndrugApplicabilityarea.All;
            // 
            // ucPactUnitMaintenance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.neuTabControl1);
            this.Name = "ucPactUnitMaintenance";
            this.Size = new System.Drawing.Size(893, 662);
            this.Load += new System.EventHandler(this.ucPactUnitMaintenance_Load);
            this.neuTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpMain_Sheet1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.neuPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpDetail_Sheet1)).EndInit();
            this.neuTabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.neuPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpFeeCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpFeeCode_Sheet1)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.neuPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuTabControl neuTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private FarPoint.Win.Spread.FpSpread fpMain;
        private FarPoint.Win.Spread.SheetView fpMain_Sheet1;
        private System.Windows.Forms.TabPage tabPage2;
        private Neusoft.FrameWork.WinForms.Controls.NeuTabControl neuTabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        private FarPoint.Win.Spread.FpSpread fpDetail;
        private FarPoint.Win.Spread.SheetView fpDetail_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuSplitter neuSplitter1;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel3;
        
        private FarPoint.Win.Spread.FpSpread fpFeeCode;
        private FarPoint.Win.Spread.SheetView fpFeeCode_Sheet1;
        private Neusoft.HISFC.Components.Common.Controls.ucInputItem ucInputItem1;
        private Neusoft.FrameWork.WinForms.Controls.NeuSplitter neuSplitter2;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbQueryValue;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private System.Windows.Forms.Panel panel1;
        
    }
}
