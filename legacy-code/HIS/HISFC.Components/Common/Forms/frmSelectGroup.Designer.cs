namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmSelectGroup
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectGroup));
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.TipAppearance tipAppearance2 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            this.spread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.spread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.btnOK = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btnCancel = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.chkAll = new Neusoft.FrameWork.WinForms.Controls.NeuCheckBox();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.neuSpread1 = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
            this.neuSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSave = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btnDown = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btnUp = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxIsSelectAll = new Neusoft.FrameWork.WinForms.Controls.NeuCheckBox();
            this.btDelete = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            ((System.ComponentModel.ISupportInitialize)(this.spread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spread1_Sheet1)).BeginInit();
            this.neuPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // spread1
            // 
            this.spread1.About = "3.0.2004.2005";
            resources.ApplyResources(this.spread1, "spread1");
            this.spread1.BackColor = System.Drawing.Color.White;
            this.spread1.FileName = "";
            this.spread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.spread1.IsAutoSaveGridStatus = false;
            this.spread1.IsCanCustomConfigColumn = false;
            this.spread1.Name = "spread1";
            this.spread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.spread1_Sheet1});
            this.spread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.spread1.TextTipAppearance = tipAppearance1;
            this.spread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.spread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.spread1_ButtonClicked);
            this.spread1.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.spread1_CellClick);
            // 
            // spread1_Sheet1
            // 
            this.spread1_Sheet1.Reset();
            this.spread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.spread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.spread1_Sheet1.ColumnCount = 20;
            this.spread1_Sheet1.RowCount = 1;
            this.spread1_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin1", System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247))))), System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247))))), System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(217)))), ((int)(((byte)(217))))), System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, false, false, false, true, true);
            this.spread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.spread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.spread1_Sheet1.ColumnHeader.Rows.Get(0).Height = 30F;
            this.spread1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.spread1_Sheet1.RowHeader.Columns.Get(0).Width = 37F;
            this.spread1_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.spread1_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.spread1_Sheet1.Rows.Default.Height = 22F;
            this.spread1_Sheet1.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.MultiRange;
            this.spread1_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.spread1_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.spread1_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.spread1_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.spread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnOK.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnCancel.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkAll
            // 
            resources.ApplyResources(this.chkAll, "chkAll");
            this.chkAll.Checked = true;
            this.chkAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAll.Name = "chkAll";
            this.chkAll.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.label1);
            this.neuPanel1.Controls.Add(this.groupBox1);
            this.neuPanel1.Controls.Add(this.btnCancel);
            this.neuPanel1.Controls.Add(this.chkAll);
            this.neuPanel1.Controls.Add(this.btnOK);
            resources.ApplyResources(this.neuPanel1, "neuPanel1");
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitter1);
            this.groupBox1.Controls.Add(this.neuSpread1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // neuSpread1
            // 
            this.neuSpread1.About = "3.0.2004.2005";
            resources.ApplyResources(this.neuSpread1, "neuSpread1");
            this.neuSpread1.BackColor = System.Drawing.Color.White;
            this.neuSpread1.FileName = "";
            this.neuSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.neuSpread1.IsAutoSaveGridStatus = false;
            this.neuSpread1.IsCanCustomConfigColumn = false;
            this.neuSpread1.Name = "neuSpread1";
            this.neuSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.neuSpread1_Sheet1});
            this.neuSpread1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            tipAppearance2.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.neuSpread1.TextTipAppearance = tipAppearance2;
            this.neuSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // neuSpread1_Sheet1
            // 
            this.neuSpread1_Sheet1.Reset();
            this.neuSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.neuSpread1_Sheet1.ColumnCount = 5;
            this.neuSpread1_Sheet1.RowCount = 0;
            this.neuSpread1_Sheet1.ActiveSkin = new FarPoint.Win.Spread.SheetSkin("CustomSkin1", System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247))))), System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.LightGray, FarPoint.Win.Spread.GridLines.Both, System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247))))), System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(217)))), ((int)(((byte)(217))))), System.Drawing.Color.Empty, System.Drawing.Color.Empty, System.Drawing.Color.Empty, false, false, false, true, true);
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "项目编码";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "项目名称";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "数量";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "有效";
            this.neuSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "医保等级";
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.ColumnHeader.Rows.Get(0).Height = 30F;
            this.neuSpread1_Sheet1.Columns.Get(0).Label = "项目编码";
            this.neuSpread1_Sheet1.Columns.Get(0).Width = 95F;
            this.neuSpread1_Sheet1.Columns.Get(1).Label = "项目名称";
            this.neuSpread1_Sheet1.Columns.Get(1).Width = 226F;
            this.neuSpread1_Sheet1.Columns.Get(3).CellType = checkBoxCellType1;
            this.neuSpread1_Sheet1.Columns.Get(3).Label = "有效";
            this.neuSpread1_Sheet1.Columns.Get(4).Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuSpread1_Sheet1.Columns.Get(4).Label = "医保等级";
            this.neuSpread1_Sheet1.Columns.Get(4).Width = 35F;
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = true;
            this.neuSpread1_Sheet1.RowHeader.Columns.Get(0).Width = 25F;
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.neuSpread1_Sheet1.RowHeader.DefaultStyle.Parent = "HeaderDefault";
            this.neuSpread1_Sheet1.Rows.Default.Height = 22F;
            this.neuSpread1_Sheet1.SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.neuSpread1_Sheet1.SheetCornerStyle.Parent = "CornerDefault";
            this.neuSpread1_Sheet1.VisualStyles = FarPoint.Win.VisualStyles.Off;
            this.neuSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.neuSpread1.SetActiveViewport(0, 1, 0);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.spread1);
            this.panel1.Controls.Add(this.panel2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnDown);
            this.panel2.Controls.Add(this.btnUp);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cbxIsSelectAll);
            this.panel2.Controls.Add(this.btDelete);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnSave.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDown
            // 
            resources.ApplyResources(this.btnDown, "btnDown");
            this.btnDown.Name = "btnDown";
            this.btnDown.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnDown.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            resources.ApplyResources(this.btnUp, "btnUp");
            this.btnUp.Name = "btnUp";
            this.btnUp.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnUp.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Name = "label2";
            // 
            // cbxIsSelectAll
            // 
            resources.ApplyResources(this.cbxIsSelectAll, "cbxIsSelectAll");
            this.cbxIsSelectAll.Checked = true;
            this.cbxIsSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxIsSelectAll.ForeColor = System.Drawing.Color.Red;
            this.cbxIsSelectAll.Name = "cbxIsSelectAll";
            this.cbxIsSelectAll.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.cbxIsSelectAll.UseVisualStyleBackColor = true;
            this.cbxIsSelectAll.CheckedChanged += new System.EventHandler(this.cbxIsSelectAll_CheckedChanged);
            // 
            // btDelete
            // 
            resources.ApplyResources(this.btDelete, "btDelete");
            this.btDelete.Name = "btDelete";
            this.btDelete.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btDelete.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // frmSelectGroup
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.neuPanel1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.Name = "frmSelectGroup";
            ((System.ComponentModel.ISupportInitialize)(this.spread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spread1_Sheet1)).EndInit();
            this.neuPanel1.ResumeLayout(false);
            this.neuPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neuSpread1_Sheet1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuSpread spread1;
        private FarPoint.Win.Spread.SheetView spread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnOK;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnCancel;
        private Neusoft.FrameWork.WinForms.Controls.NeuCheckBox chkAll;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuSpread neuSpread1;
        private FarPoint.Win.Spread.SheetView neuSpread1_Sheet1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btDelete;
        private System.Windows.Forms.Label label2;
        private Neusoft.FrameWork.WinForms.Controls.NeuCheckBox cbxIsSelectAll;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnUp;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnDown;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnSave;
    }
}