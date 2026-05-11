namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class ucInputItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucInputItem));
            this.panel4 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.neuSplitter1 = new Neusoft.FrameWork.WinForms.Controls.NeuSplitter();
            this.panel3 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.chkoutflag = new Neusoft.FrameWork.WinForms.Controls.NeuCheckBox();
            this.txtItemCode = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtItemName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lblItemName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.panel2 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.cmbCategory = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.lblCategory = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.neuPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            this.panel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.VS2003;
            // 
            // neuSplitter1
            // 
            resources.ApplyResources(this.neuSplitter1, "neuSplitter1");
            this.neuSplitter1.Name = "neuSplitter1";
            this.neuSplitter1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.VS2003;
            this.neuSplitter1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Controls.Add(this.panel2);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            this.panel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.VS2003;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.neuPanel1);
            this.panel1.Controls.Add(this.txtItemCode);
            this.panel1.Controls.Add(this.txtItemName);
            this.panel1.Controls.Add(this.lblItemName);
            this.panel1.Controls.Add(this.neuLabel1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.chkoutflag);
            resources.ApplyResources(this.neuPanel1, "neuPanel1");
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.VS2003;
            // 
            // chkoutflag
            // 
            resources.ApplyResources(this.chkoutflag, "chkoutflag");
            this.chkoutflag.ForeColor = System.Drawing.Color.Red;
            this.chkoutflag.Name = "chkoutflag";
            this.chkoutflag.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.chkoutflag.TabStop = false;
            this.chkoutflag.UseVisualStyleBackColor = true;
            this.chkoutflag.CheckedChanged += new System.EventHandler(this.chkoutflag_CheckedChanged);
            // 
            // txtItemCode
            // 
            resources.ApplyResources(this.txtItemCode, "txtItemCode");
            this.txtItemCode.IsEnter2Tab = false;
            this.txtItemCode.Name = "txtItemCode";
            this.txtItemCode.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtItemCode.TextChanged += new System.EventHandler(this.txtItemCode_TextChanged);
            this.txtItemCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItemCode_KeyDown);
            this.txtItemCode.Leave += new System.EventHandler(this.txtItemCode_Leave);
            this.txtItemCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtItemCode_KeyUp);
            this.txtItemCode.Enter += new System.EventHandler(this.txtItemCode_Enter);
            // 
            // txtItemName
            // 
            resources.ApplyResources(this.txtItemName, "txtItemName");
            this.txtItemName.BackColor = System.Drawing.Color.White;
            this.txtItemName.IsEnter2Tab = false;
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtItemName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtItemName_KeyPress);
            // 
            // lblItemName
            // 
            resources.ApplyResources(this.lblItemName, "lblItemName");
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            // 
            // neuLabel1
            // 
            resources.ApplyResources(this.neuLabel1, "neuLabel1");
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cmbCategory);
            this.panel2.Controls.Add(this.lblCategory);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            this.panel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.VS2003;
            // 
            // cmbCategory
            // 
            resources.ApplyResources(this.cmbCategory, "cmbCategory");
            this.cmbCategory.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbCategory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.DropDownWidth = 81;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.IsEnter2Tab = false;
            this.cmbCategory.IsFlat = false;
            this.cmbCategory.IsLike = true;
            this.cmbCategory.IsListOnly = false;
            this.cmbCategory.IsPopForm = true;
            this.cmbCategory.IsShowCustomerList = false;
            this.cmbCategory.IsShowID = false;
            this.cmbCategory.IsShowIDAndName = false;
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.ShowCustomerList = false;
            this.cmbCategory.ShowID = false;
            this.cmbCategory.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbCategory.Tag = "";
            this.cmbCategory.ToolBarUse = false;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cmbCategory_SelectedIndexChanged);
            this.cmbCategory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbCategory_KeyDown);
            // 
            // lblCategory
            // 
            resources.ApplyResources(this.lblCategory, "lblCategory");
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.VS2003;
            // 
            // ucInputItem
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.neuSplitter1);
            this.Controls.Add(this.panel3);
            this.Name = "ucInputItem";
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.neuPanel1.ResumeLayout(false);
            this.neuPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lblCategory;
        protected Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbCategory;
        public Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtItemName;
        public Neusoft.FrameWork.WinForms.Controls.NeuLabel lblItemName;
        public Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtItemCode;
        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel panel3;
        protected Neusoft.FrameWork.WinForms.Controls.NeuSplitter neuSplitter1;
        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel panel4;
        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel panel2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        public Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuCheckBox chkoutflag;
        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;

    }
}
