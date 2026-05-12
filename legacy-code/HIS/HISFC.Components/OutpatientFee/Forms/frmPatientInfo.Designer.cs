namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    partial class frmPatientInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbDoct = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbPact = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbMCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbRebate = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.tbName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lbName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbSex = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbSex = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.lbAge = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.tbAge = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.lbRegDept = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbRegDept = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbDoct = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbPact = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.lbClass = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbClass = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.tbMCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.cmbRebate = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuContexMenu1 = new Neusoft.FrameWork.WinForms.Controls.NeuContexMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.plMain = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.tbCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tbJZDNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.plMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbDoct
            // 
            this.lbDoct.AutoSize = true;
            this.lbDoct.Font = new System.Drawing.Font("宋体", 10F);
            this.lbDoct.ForeColor = System.Drawing.Color.Blue;
            this.lbDoct.Location = new System.Drawing.Point(4, 44);
            this.lbDoct.Name = "lbDoct";
            this.lbDoct.Size = new System.Drawing.Size(70, 14);
            this.lbDoct.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbDoct.TabIndex = 10;
            this.lbDoct.Text = "开立医生:";
            // 
            // lbPact
            // 
            this.lbPact.AutoSize = true;
            this.lbPact.Font = new System.Drawing.Font("宋体", 10F);
            this.lbPact.ForeColor = System.Drawing.Color.Blue;
            this.lbPact.Location = new System.Drawing.Point(378, 44);
            this.lbPact.Name = "lbPact";
            this.lbPact.Size = new System.Drawing.Size(70, 14);
            this.lbPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbPact.TabIndex = 12;
            this.lbPact.Text = "结算种类:";
            // 
            // lbMCardNO
            // 
            this.lbMCardNO.AutoSize = true;
            this.lbMCardNO.Font = new System.Drawing.Font("宋体", 10F);
            this.lbMCardNO.Location = new System.Drawing.Point(4, 78);
            this.lbMCardNO.Name = "lbMCardNO";
            this.lbMCardNO.Size = new System.Drawing.Size(70, 14);
            this.lbMCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbMCardNO.TabIndex = 16;
            this.lbMCardNO.Text = "医疗证号:";
            // 
            // lbRebate
            // 
            this.lbRebate.AutoSize = true;
            this.lbRebate.Font = new System.Drawing.Font("宋体", 10F);
            this.lbRebate.Location = new System.Drawing.Point(191, 78);
            this.lbRebate.Name = "lbRebate";
            this.lbRebate.Size = new System.Drawing.Size(70, 14);
            this.lbRebate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbRebate.TabIndex = 18;
            this.lbRebate.Text = "优惠编码:";
            this.lbRebate.Visible = false;
            // 
            // lbCardNO
            // 
            this.lbCardNO.AutoSize = true;
            this.lbCardNO.Font = new System.Drawing.Font("宋体", 10F);
            this.lbCardNO.ForeColor = System.Drawing.Color.Blue;
            this.lbCardNO.Location = new System.Drawing.Point(5, 10);
            this.lbCardNO.Name = "lbCardNO";
            this.lbCardNO.Size = new System.Drawing.Size(77, 14);
            this.lbCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbCardNO.TabIndex = 0;
            this.lbCardNO.Text = "就诊卡号：";
            // 
            // tbCardNO
            // 
            this.tbCardNO.Font = new System.Drawing.Font("宋体", 10F);
            this.tbCardNO.IsEnter2Tab = false;
            this.tbCardNO.Location = new System.Drawing.Point(76, 6);
            this.tbCardNO.Name = "tbCardNO";
            this.tbCardNO.Size = new System.Drawing.Size(111, 23);
            this.tbCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCardNO.TabIndex = 1;
            this.tbCardNO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbCardNO_KeyDown);
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("宋体", 10F);
            this.tbName.IsEnter2Tab = false;
            this.tbName.Location = new System.Drawing.Point(261, 6);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(111, 23);
            this.tbName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbName.TabIndex = 3;
            this.tbName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbName_KeyDown);
            this.tbName.Leave += new System.EventHandler(this.tbName_Leave);
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("宋体", 10F);
            this.lbName.ForeColor = System.Drawing.Color.Blue;
            this.lbName.Location = new System.Drawing.Point(191, 10);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(70, 14);
            this.lbName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbName.TabIndex = 2;
            this.lbName.Text = "患者姓名:";
            // 
            // lbSex
            // 
            this.lbSex.AutoSize = true;
            this.lbSex.Font = new System.Drawing.Font("宋体", 10F);
            this.lbSex.ForeColor = System.Drawing.Color.Blue;
            this.lbSex.Location = new System.Drawing.Point(381, 10);
            this.lbSex.Name = "lbSex";
            this.lbSex.Size = new System.Drawing.Size(42, 14);
            this.lbSex.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbSex.TabIndex = 4;
            this.lbSex.Text = "性别:";
            // 
            // cmbSex
            // 
            this.cmbSex.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbSex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbSex.Font = new System.Drawing.Font("宋体", 10F);
            this.cmbSex.FormattingEnabled = true;
            this.cmbSex.IsEnter2Tab = false;
            this.cmbSex.IsFlat = false;
            this.cmbSex.IsLike = true;
            this.cmbSex.IsListOnly = false;
            this.cmbSex.IsPopForm = true;
            this.cmbSex.IsShowCustomerList = false;
            this.cmbSex.IsShowID = false;
            this.cmbSex.Location = new System.Drawing.Point(424, 6);
            this.cmbSex.Name = "cmbSex";
            this.cmbSex.PopForm = null;
            this.cmbSex.ShowCustomerList = false;
            this.cmbSex.ShowID = false;
            this.cmbSex.Size = new System.Drawing.Size(49, 21);
            this.cmbSex.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbSex.TabIndex = 5;
            this.cmbSex.Tag = "";
            this.cmbSex.ToolBarUse = false;
            this.cmbSex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbSex_KeyDown);
            // 
            // lbAge
            // 
            this.lbAge.AutoSize = true;
            this.lbAge.Font = new System.Drawing.Font("宋体", 10F);
            this.lbAge.ForeColor = System.Drawing.Color.Blue;
            this.lbAge.Location = new System.Drawing.Point(473, 10);
            this.lbAge.Name = "lbAge";
            this.lbAge.Size = new System.Drawing.Size(42, 14);
            this.lbAge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbAge.TabIndex = 6;
            this.lbAge.Text = "年龄:";
            // 
            // tbAge
            // 
            this.tbAge.Font = new System.Drawing.Font("宋体", 10F);
            this.tbAge.IsEnter2Tab = false;
            this.tbAge.Location = new System.Drawing.Point(516, 6);
            this.tbAge.Name = "tbAge";
            this.tbAge.Size = new System.Drawing.Size(51, 23);
            this.tbAge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbAge.TabIndex = 7;
            this.tbAge.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbAge_KeyDown);
            // 
            // lbRegDept
            // 
            this.lbRegDept.AutoSize = true;
            this.lbRegDept.Font = new System.Drawing.Font("宋体", 10F);
            this.lbRegDept.ForeColor = System.Drawing.Color.Blue;
            this.lbRegDept.Location = new System.Drawing.Point(191, 44);
            this.lbRegDept.Name = "lbRegDept";
            this.lbRegDept.Size = new System.Drawing.Size(70, 14);
            this.lbRegDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbRegDept.TabIndex = 8;
            this.lbRegDept.Text = "看诊科室:";
            // 
            // cmbRegDept
            // 
            this.cmbRegDept.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbRegDept.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbRegDept.Font = new System.Drawing.Font("宋体", 10F);
            this.cmbRegDept.FormattingEnabled = true;
            this.cmbRegDept.IsEnter2Tab = false;
            this.cmbRegDept.IsFlat = false;
            this.cmbRegDept.IsLike = true;
            this.cmbRegDept.IsListOnly = false;
            this.cmbRegDept.IsPopForm = true;
            this.cmbRegDept.IsShowCustomerList = false;
            this.cmbRegDept.IsShowID = false;
            this.cmbRegDept.Location = new System.Drawing.Point(261, 41);
            this.cmbRegDept.Name = "cmbRegDept";
            this.cmbRegDept.PopForm = null;
            this.cmbRegDept.ShowCustomerList = false;
            this.cmbRegDept.ShowID = false;
            this.cmbRegDept.Size = new System.Drawing.Size(111, 21);
            this.cmbRegDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbRegDept.TabIndex = 11;
            this.cmbRegDept.Tag = "";
            this.cmbRegDept.ToolBarUse = false;
            this.cmbRegDept.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbRegDept_KeyDown);
            // 
            // cmbDoct
            // 
            this.cmbDoct.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbDoct.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbDoct.Font = new System.Drawing.Font("宋体", 10F);
            this.cmbDoct.FormattingEnabled = true;
            this.cmbDoct.IsEnter2Tab = false;
            this.cmbDoct.IsFlat = false;
            this.cmbDoct.IsLike = true;
            this.cmbDoct.IsListOnly = false;
            this.cmbDoct.IsPopForm = true;
            this.cmbDoct.IsShowCustomerList = false;
            this.cmbDoct.IsShowID = false;
            this.cmbDoct.Location = new System.Drawing.Point(76, 41);
            this.cmbDoct.Name = "cmbDoct";
            this.cmbDoct.PopForm = null;
            this.cmbDoct.ShowCustomerList = false;
            this.cmbDoct.ShowID = false;
            this.cmbDoct.Size = new System.Drawing.Size(111, 21);
            this.cmbDoct.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbDoct.TabIndex = 9;
            this.cmbDoct.Tag = "";
            this.cmbDoct.ToolBarUse = false;
            this.cmbDoct.SelectedIndexChanged += new System.EventHandler(this.cmbDoct_SelectedIndexChanged);
            this.cmbDoct.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbDoct_KeyDown);
            // 
            // cmbPact
            // 
            this.cmbPact.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbPact.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbPact.Font = new System.Drawing.Font("宋体", 10F);
            this.cmbPact.FormattingEnabled = true;
            this.cmbPact.IsEnter2Tab = false;
            this.cmbPact.IsFlat = false;
            this.cmbPact.IsLike = true;
            this.cmbPact.IsListOnly = false;
            this.cmbPact.IsPopForm = true;
            this.cmbPact.IsShowCustomerList = false;
            this.cmbPact.IsShowID = false;
            this.cmbPact.Location = new System.Drawing.Point(449, 41);
            this.cmbPact.Name = "cmbPact";
            this.cmbPact.PopForm = null;
            this.cmbPact.ShowCustomerList = false;
            this.cmbPact.ShowID = false;
            this.cmbPact.Size = new System.Drawing.Size(120, 21);
            this.cmbPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbPact.TabIndex = 13;
            this.cmbPact.Tag = "";
            this.cmbPact.ToolBarUse = false;
            this.cmbPact.SelectedIndexChanged += new System.EventHandler(this.cmbPact_SelectedIndexChanged);
            this.cmbPact.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPact_KeyDown);
            // 
            // lbClass
            // 
            this.lbClass.AutoSize = true;
            this.lbClass.Font = new System.Drawing.Font("宋体", 10F);
            this.lbClass.Location = new System.Drawing.Point(378, 78);
            this.lbClass.Name = "lbClass";
            this.lbClass.Size = new System.Drawing.Size(70, 14);
            this.lbClass.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbClass.TabIndex = 14;
            this.lbClass.Text = "等级编码:";
            // 
            // cmbClass
            // 
            this.cmbClass.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbClass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClass.DropDownWidth = 350;
            this.cmbClass.Font = new System.Drawing.Font("宋体", 10F);
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.IsEnter2Tab = false;
            this.cmbClass.IsFlat = false;
            this.cmbClass.IsLike = true;
            this.cmbClass.IsListOnly = false;
            this.cmbClass.IsPopForm = true;
            this.cmbClass.IsShowCustomerList = false;
            this.cmbClass.IsShowID = false;
            this.cmbClass.Location = new System.Drawing.Point(449, 75);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.PopForm = null;
            this.cmbClass.ShowCustomerList = false;
            this.cmbClass.ShowID = false;
            this.cmbClass.Size = new System.Drawing.Size(120, 21);
            this.cmbClass.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbClass.TabIndex = 15;
            this.cmbClass.Tag = "";
            this.cmbClass.ToolBarUse = false;
            this.cmbClass.SelectedIndexChanged += new System.EventHandler(this.cmbClass_SelectedIndexChanged);
            this.cmbClass.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbClass_KeyDown);
            // 
            // tbMCardNO
            // 
            this.tbMCardNO.Font = new System.Drawing.Font("宋体", 10F);
            this.tbMCardNO.IsEnter2Tab = false;
            this.tbMCardNO.Location = new System.Drawing.Point(76, 75);
            this.tbMCardNO.Name = "tbMCardNO";
            this.tbMCardNO.Size = new System.Drawing.Size(111, 23);
            this.tbMCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbMCardNO.TabIndex = 17;
            this.tbMCardNO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbMCardNO_KeyDown);
            // 
            // cmbRebate
            // 
            this.cmbRebate.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbRebate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbRebate.Enabled = false;
            this.cmbRebate.Font = new System.Drawing.Font("宋体", 10F);
            this.cmbRebate.FormattingEnabled = true;
            this.cmbRebate.IsEnter2Tab = false;
            this.cmbRebate.IsFlat = false;
            this.cmbRebate.IsLike = true;
            this.cmbRebate.IsListOnly = false;
            this.cmbRebate.IsPopForm = true;
            this.cmbRebate.IsShowCustomerList = false;
            this.cmbRebate.IsShowID = false;
            this.cmbRebate.Location = new System.Drawing.Point(261, 75);
            this.cmbRebate.Name = "cmbRebate";
            this.cmbRebate.PopForm = null;
            this.cmbRebate.ShowCustomerList = false;
            this.cmbRebate.ShowID = false;
            this.cmbRebate.Size = new System.Drawing.Size(111, 21);
            this.cmbRebate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbRebate.TabIndex = 19;
            this.cmbRebate.Tag = "";
            this.cmbRebate.ToolBarUse = false;
            this.cmbRebate.Visible = false;
            this.cmbRebate.SelectedIndexChanged += new System.EventHandler(this.cmbRebate_SelectedIndexChanged);
            this.cmbRebate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbRebate_KeyDown);
            // 
            // neuContexMenu1
            // 
            this.neuContexMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem4,
            this.menuItem3,
            this.menuItem5,
            this.menuItem6,
            this.menuItem8,
            this.menuItem10});
            this.neuContexMenu1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuContexMenu1.Popup += new System.EventHandler(this.neuContexMenu1_Popup);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "添加(&A)";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "删除(&D)";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "-";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 3;
            this.menuItem3.Text = "复制(&C)";
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem7,
            this.menuItem9});
            this.menuItem5.Text = "复制(&C+N)";
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 0;
            this.menuItem7.Text = "3张";
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 1;
            this.menuItem9.Text = "其他";
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 5;
            this.menuItem6.Text = "全选(A+R)";
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 6;
            this.menuItem8.Text = "上一张处方(P+R)";
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 7;
            this.menuItem10.Text = "下一张处方(N+R)";
            // 
            // plMain
            // 
            this.plMain.Controls.Add(this.tbCancel);
            this.plMain.Controls.Add(this.btnOk);
            this.plMain.Controls.Add(this.tbCardNO);
            this.plMain.Controls.Add(this.lbCardNO);
            this.plMain.Controls.Add(this.tbAge);
            this.plMain.Controls.Add(this.cmbClass);
            this.plMain.Controls.Add(this.lbClass);
            this.plMain.Controls.Add(this.tbJZDNO);
            this.plMain.Controls.Add(this.tbName);
            this.plMain.Controls.Add(this.cmbRebate);
            this.plMain.Controls.Add(this.lbRebate);
            this.plMain.Controls.Add(this.tbMCardNO);
            this.plMain.Controls.Add(this.neuLabel1);
            this.plMain.Controls.Add(this.lbName);
            this.plMain.Controls.Add(this.lbMCardNO);
            this.plMain.Controls.Add(this.lbSex);
            this.plMain.Controls.Add(this.cmbSex);
            this.plMain.Controls.Add(this.lbAge);
            this.plMain.Controls.Add(this.cmbPact);
            this.plMain.Controls.Add(this.lbPact);
            this.plMain.Controls.Add(this.lbRegDept);
            this.plMain.Controls.Add(this.cmbDoct);
            this.plMain.Controls.Add(this.cmbRegDept);
            this.plMain.Controls.Add(this.lbDoct);
            this.plMain.Dock = System.Windows.Forms.DockStyle.Left;
            this.plMain.Location = new System.Drawing.Point(2, 2);
            this.plMain.Name = "plMain";
            this.plMain.Size = new System.Drawing.Size(578, 134);
            this.plMain.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.plMain.TabIndex = 0;
            // 
            // tbCancel
            // 
            this.tbCancel.Location = new System.Drawing.Point(492, 105);
            this.tbCancel.Name = "tbCancel";
            this.tbCancel.Size = new System.Drawing.Size(75, 23);
            this.tbCancel.TabIndex = 21;
            this.tbCancel.Text = "取消";
            this.tbCancel.UseVisualStyleBackColor = true;
            this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(398, 105);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 20;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // tbJZDNO
            // 
            this.tbJZDNO.Font = new System.Drawing.Font("宋体", 10F);
            this.tbJZDNO.IsEnter2Tab = false;
            this.tbJZDNO.Location = new System.Drawing.Point(261, 75);
            this.tbJZDNO.MaxLength = 8;
            this.tbJZDNO.Name = "tbJZDNO";
            this.tbJZDNO.Size = new System.Drawing.Size(111, 23);
            this.tbJZDNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbJZDNO.TabIndex = 3;
            this.tbJZDNO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbJZDNO_KeyDown);
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel1.ForeColor = System.Drawing.Color.Blue;
            this.neuLabel1.Location = new System.Drawing.Point(191, 79);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(70, 14);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 2;
            this.neuLabel1.Text = "记账单号:";
            // 
            // frmPatientInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 138);
            this.Controls.Add(this.plMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPatientInfo";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.plMain.ResumeLayout(false);
            this.plMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbCardNO;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbCardNO;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbName;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbName;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbSex;
        protected Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbSex;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbAge;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbAge;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbRegDept;
        protected Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbRegDept;
        protected Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbDoct;
        protected Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbPact;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel lbClass;
        protected Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbClass;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbMCardNO;
        protected Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbRebate;
        protected System.Windows.Forms.MenuItem menuItem1;
        protected System.Windows.Forms.MenuItem menuItem2;
        protected Neusoft.FrameWork.WinForms.Controls.NeuContexMenu neuContexMenu1;
        protected Neusoft.FrameWork.WinForms.Controls.NeuPanel plMain;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuItem10;
        protected Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbJZDNO;
        protected Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbDoct;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbPact;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbMCardNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbRebate;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button tbCancel;
    }
}