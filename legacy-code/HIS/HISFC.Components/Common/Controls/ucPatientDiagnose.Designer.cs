namespace Neusoft.HISFC.Components.Common.Controls
{
    partial class ucPatientDiagnose
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.plData = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tbcDiagList = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.plDiagInput = new System.Windows.Forms.Panel();
            this.cmbDiagType = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbDiagList = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel7 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbSuffix = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbPrefix = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel5 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel6 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbDiagnose = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.plToolBar = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuToolBar1 = new Neusoft.FrameWork.WinForms.Controls.NeuToolBar();
            this.tbSave = new System.Windows.Forms.ToolBarButton();
            this.tbAbandon = new System.Windows.Forms.ToolBarButton();
            this.tbDelete = new System.Windows.Forms.ToolBarButton();
            this.tbUp = new System.Windows.Forms.ToolBarButton();
            this.tbDown = new System.Windows.Forms.ToolBarButton();
            this.tbExit = new System.Windows.Forms.ToolBarButton();
            this.panel1.SuspendLayout();
            this.plData.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tbcDiagList.SuspendLayout();
            this.panel2.SuspendLayout();
            this.plDiagInput.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.plToolBar.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.plData);
            this.panel1.Controls.Add(this.plToolBar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1316, 777);
            this.panel1.TabIndex = 0;
            // 
            // plData
            // 
            this.plData.Controls.Add(this.splitContainer1);
            this.plData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plData.Location = new System.Drawing.Point(0, 88);
            this.plData.Margin = new System.Windows.Forms.Padding(4);
            this.plData.Name = "plData";
            this.plData.Size = new System.Drawing.Size(1316, 689);
            this.plData.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1316, 689);
            this.splitContainer1.SplitterDistance = 1257;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tbcDiagList);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 52);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1257, 637);
            this.panel3.TabIndex = 2;
            // 
            // tbcDiagList
            // 
            this.tbcDiagList.Controls.Add(this.tabPage1);
            this.tbcDiagList.Controls.Add(this.tabPage2);
            this.tbcDiagList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcDiagList.Location = new System.Drawing.Point(0, 0);
            this.tbcDiagList.Margin = new System.Windows.Forms.Padding(4);
            this.tbcDiagList.Name = "tbcDiagList";
            this.tbcDiagList.SelectedIndex = 0;
            this.tbcDiagList.Size = new System.Drawing.Size(1257, 637);
            this.tbcDiagList.TabIndex = 0;
            this.tbcDiagList.SelectedIndexChanged += new System.EventHandler(this.tbcDiagList_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(1249, 607);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1249, 611);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.plDiagInput);
            this.panel2.Controls.Add(this.cmbDiagnose);
            this.panel2.Controls.Add(this.neuLabel3);
            this.panel2.Controls.Add(this.neuLabel2);
            this.panel2.Controls.Add(this.neuLabel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1257, 52);
            this.panel2.TabIndex = 1;
            // 
            // plDiagInput
            // 
            this.plDiagInput.Controls.Add(this.cmbDiagType);
            this.plDiagInput.Controls.Add(this.cmbDiagList);
            this.plDiagInput.Controls.Add(this.neuLabel7);
            this.plDiagInput.Controls.Add(this.cmbSuffix);
            this.plDiagInput.Controls.Add(this.cmbPrefix);
            this.plDiagInput.Controls.Add(this.neuLabel4);
            this.plDiagInput.Controls.Add(this.neuLabel5);
            this.plDiagInput.Controls.Add(this.neuLabel6);
            this.plDiagInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.plDiagInput.Location = new System.Drawing.Point(0, 0);
            this.plDiagInput.Margin = new System.Windows.Forms.Padding(4);
            this.plDiagInput.Name = "plDiagInput";
            this.plDiagInput.Size = new System.Drawing.Size(1257, 49);
            this.plDiagInput.TabIndex = 9;
            // 
            // cmbDiagType
            // 
            this.cmbDiagType.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbDiagType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbDiagType.Enabled = false;
            this.cmbDiagType.FormattingEnabled = true;
            this.cmbDiagType.IsEnter2Tab = false;
            this.cmbDiagType.IsFlat = false;
            this.cmbDiagType.IsLike = true;
            this.cmbDiagType.IsListOnly = false;
            this.cmbDiagType.IsPopForm = true;
            this.cmbDiagType.IsShowCustomerList = false;
            this.cmbDiagType.IsShowID = false;
            this.cmbDiagType.IsShowIDAndName = false;
            this.cmbDiagType.Location = new System.Drawing.Point(109, 17);
            this.cmbDiagType.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDiagType.Name = "cmbDiagType";
            this.cmbDiagType.ShowCustomerList = false;
            this.cmbDiagType.ShowID = false;
            this.cmbDiagType.Size = new System.Drawing.Size(160, 24);
            this.cmbDiagType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbDiagType.TabIndex = 10;
            this.cmbDiagType.Tag = "";
            this.cmbDiagType.ToolBarUse = false;
            // 
            // cmbDiagList
            // 
            this.cmbDiagList.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbDiagList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbDiagList.FormattingEnabled = true;
            this.cmbDiagList.IsEnter2Tab = false;
            this.cmbDiagList.IsFlat = false;
            this.cmbDiagList.IsLike = true;
            this.cmbDiagList.IsListOnly = false;
            this.cmbDiagList.IsPopForm = true;
            this.cmbDiagList.IsShowCustomerList = false;
            this.cmbDiagList.IsShowID = false;
            this.cmbDiagList.IsShowIDAndName = false;
            this.cmbDiagList.Location = new System.Drawing.Point(573, 17);
            this.cmbDiagList.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDiagList.Name = "cmbDiagList";
            this.cmbDiagList.ShowCustomerList = false;
            this.cmbDiagList.ShowID = false;
            this.cmbDiagList.Size = new System.Drawing.Size(160, 24);
            this.cmbDiagList.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbDiagList.TabIndex = 11;
            this.cmbDiagList.Tag = "";
            this.cmbDiagList.ToolBarUse = false;
            // 
            // neuLabel7
            // 
            this.neuLabel7.AutoSize = true;
            this.neuLabel7.Location = new System.Drawing.Point(21, 21);
            this.neuLabel7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.neuLabel7.Name = "neuLabel7";
            this.neuLabel7.Size = new System.Drawing.Size(72, 16);
            this.neuLabel7.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel7.TabIndex = 9;
            this.neuLabel7.Text = "诊断类型";
            // 
            // cmbSuffix
            // 
            this.cmbSuffix.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbSuffix.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbSuffix.FormattingEnabled = true;
            this.cmbSuffix.IsEnter2Tab = false;
            this.cmbSuffix.IsFlat = false;
            this.cmbSuffix.IsLike = true;
            this.cmbSuffix.IsListOnly = false;
            this.cmbSuffix.IsPopForm = true;
            this.cmbSuffix.IsShowCustomerList = false;
            this.cmbSuffix.IsShowID = false;
            this.cmbSuffix.IsShowIDAndName = false;
            this.cmbSuffix.Location = new System.Drawing.Point(797, 17);
            this.cmbSuffix.Margin = new System.Windows.Forms.Padding(4);
            this.cmbSuffix.Name = "cmbSuffix";
            this.cmbSuffix.ShowCustomerList = false;
            this.cmbSuffix.ShowID = false;
            this.cmbSuffix.Size = new System.Drawing.Size(162, 24);
            this.cmbSuffix.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbSuffix.TabIndex = 7;
            this.cmbSuffix.Tag = "";
            this.cmbSuffix.ToolBarUse = false;
            // 
            // cmbPrefix
            // 
            this.cmbPrefix.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbPrefix.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbPrefix.FormattingEnabled = true;
            this.cmbPrefix.IsEnter2Tab = false;
            this.cmbPrefix.IsFlat = false;
            this.cmbPrefix.IsLike = true;
            this.cmbPrefix.IsListOnly = false;
            this.cmbPrefix.IsPopForm = true;
            this.cmbPrefix.IsShowCustomerList = false;
            this.cmbPrefix.IsShowID = false;
            this.cmbPrefix.IsShowIDAndName = false;
            this.cmbPrefix.Location = new System.Drawing.Point(321, 13);
            this.cmbPrefix.Margin = new System.Windows.Forms.Padding(4);
            this.cmbPrefix.Name = "cmbPrefix";
            this.cmbPrefix.ShowCustomerList = false;
            this.cmbPrefix.ShowID = false;
            this.cmbPrefix.Size = new System.Drawing.Size(160, 24);
            this.cmbPrefix.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbPrefix.TabIndex = 8;
            this.cmbPrefix.Tag = "";
            this.cmbPrefix.ToolBarUse = false;
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Location = new System.Drawing.Point(749, 21);
            this.neuLabel4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(40, 16);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 4;
            this.neuLabel4.Text = "后缀";
            // 
            // neuLabel5
            // 
            this.neuLabel5.AutoSize = true;
            this.neuLabel5.Location = new System.Drawing.Point(517, 21);
            this.neuLabel5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.neuLabel5.Name = "neuLabel5";
            this.neuLabel5.Size = new System.Drawing.Size(40, 16);
            this.neuLabel5.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel5.TabIndex = 2;
            this.neuLabel5.Text = "诊断";
            // 
            // neuLabel6
            // 
            this.neuLabel6.AutoSize = true;
            this.neuLabel6.Location = new System.Drawing.Point(285, 21);
            this.neuLabel6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.neuLabel6.Name = "neuLabel6";
            this.neuLabel6.Size = new System.Drawing.Size(40, 16);
            this.neuLabel6.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel6.TabIndex = 0;
            this.neuLabel6.Text = "前缀";
            // 
            // cmbDiagnose
            // 
            this.cmbDiagnose.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbDiagnose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbDiagnose.FormattingEnabled = true;
            this.cmbDiagnose.IsEnter2Tab = false;
            this.cmbDiagnose.IsFlat = false;
            this.cmbDiagnose.IsLike = true;
            this.cmbDiagnose.IsListOnly = false;
            this.cmbDiagnose.IsPopForm = true;
            this.cmbDiagnose.IsShowCustomerList = false;
            this.cmbDiagnose.IsShowID = false;
            this.cmbDiagnose.IsShowIDAndName = false;
            this.cmbDiagnose.Location = new System.Drawing.Point(525, 20);
            this.cmbDiagnose.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDiagnose.Name = "cmbDiagnose";
            this.cmbDiagnose.ShowCustomerList = false;
            this.cmbDiagnose.ShowID = false;
            this.cmbDiagnose.Size = new System.Drawing.Size(160, 24);
            this.cmbDiagnose.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbDiagnose.TabIndex = 6;
            this.cmbDiagnose.Tag = "";
            this.cmbDiagnose.ToolBarUse = false;
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Location = new System.Drawing.Point(727, 33);
            this.neuLabel3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(40, 16);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 4;
            this.neuLabel3.Text = "后缀";
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Location = new System.Drawing.Point(479, 31);
            this.neuLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(40, 16);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 2;
            this.neuLabel2.Text = "诊断";
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Location = new System.Drawing.Point(213, 32);
            this.neuLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(40, 16);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 0;
            this.neuLabel1.Text = "前缀";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer2.Size = new System.Drawing.Size(54, 689);
            this.splitContainer2.SplitterDistance = 353;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // plToolBar
            // 
            this.plToolBar.Controls.Add(this.panel4);
            this.plToolBar.Controls.Add(this.neuToolBar1);
            this.plToolBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.plToolBar.Location = new System.Drawing.Point(0, 0);
            this.plToolBar.Margin = new System.Windows.Forms.Padding(4);
            this.plToolBar.Name = "plToolBar";
            this.plToolBar.Size = new System.Drawing.Size(1316, 88);
            this.plToolBar.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblName);
            this.panel4.Controls.Add(this.lblCardNO);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 65);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1316, 23);
            this.panel4.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(213, 14);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(40, 16);
            this.lblName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblName.TabIndex = 11;
            this.lblName.Text = "姓名";
            // 
            // lblCardNO
            // 
            this.lblCardNO.AutoSize = true;
            this.lblCardNO.Location = new System.Drawing.Point(21, 14);
            this.lblCardNO.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCardNO.Name = "lblCardNO";
            this.lblCardNO.Size = new System.Drawing.Size(56, 16);
            this.lblCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblCardNO.TabIndex = 10;
            this.lblCardNO.Text = "住院号";
            // 
            // neuToolBar1
            // 
            this.neuToolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbSave,
            this.tbAbandon,
            this.tbDelete,
            this.tbUp,
            this.tbDown,
            this.tbExit});
            this.neuToolBar1.ButtonSize = new System.Drawing.Size(59, 59);
            this.neuToolBar1.DropDownArrows = true;
            this.neuToolBar1.Location = new System.Drawing.Point(0, 0);
            this.neuToolBar1.Margin = new System.Windows.Forms.Padding(4);
            this.neuToolBar1.Name = "neuToolBar1";
            this.neuToolBar1.ShowToolTips = true;
            this.neuToolBar1.Size = new System.Drawing.Size(1316, 65);
            this.neuToolBar1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuToolBar1.TabIndex = 0;
            this.neuToolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.neuToolBar1_ButtonClick);
            // 
            // tbSave
            // 
            this.tbSave.Name = "tbSave";
            this.tbSave.Text = "保存";
            // 
            // tbAbandon
            // 
            this.tbAbandon.Name = "tbAbandon";
            this.tbAbandon.Text = "作废";
            // 
            // tbDelete
            // 
            this.tbDelete.Name = "tbDelete";
            this.tbDelete.Text = "删除";
            // 
            // tbUp
            // 
            this.tbUp.Name = "tbUp";
            this.tbUp.Text = "上移";
            // 
            // tbDown
            // 
            this.tbDown.Name = "tbDown";
            this.tbDown.Text = "下移";
            // 
            // tbExit
            // 
            this.tbExit.Name = "tbExit";
            this.tbExit.Text = "退出";
            // 
            // ucPatientDiagnose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ucPatientDiagnose";
            this.Size = new System.Drawing.Size(1316, 777);
            this.Load += new System.EventHandler(this.ucPatientDiagnose_Load);
            this.panel1.ResumeLayout(false);
            this.plData.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tbcDiagList.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.plDiagInput.ResumeLayout(false);
            this.plDiagInput.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.plToolBar.ResumeLayout(false);
            this.plToolBar.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel plToolBar;
        private System.Windows.Forms.Panel plData;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tbcDiagList;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuToolBar neuToolBar1;
        private System.Windows.Forms.ToolBarButton tbSave;
        private System.Windows.Forms.ToolBarButton tbAbandon;
        private System.Windows.Forms.ToolBarButton tbDelete;
        private System.Windows.Forms.ToolBarButton tbUp;
        private System.Windows.Forms.ToolBarButton tbDown;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbPrefix;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbSuffix;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbDiagnose;
        private System.Windows.Forms.Panel plDiagInput;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbDiagType;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel7;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel6;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbDiagList;
        private System.Windows.Forms.Panel panel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblName;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblCardNO;
        private System.Windows.Forms.ToolBarButton tbExit;
    }
}
