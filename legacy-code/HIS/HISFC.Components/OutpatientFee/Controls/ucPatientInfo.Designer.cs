namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    /// <summary>
    /// ucPopSelected<br></br>
    /// [功能描述: 门诊患者基本信息UC]<br></br>
    /// [创 建 者: 王宇]<br></br>
    /// [创建时间: 2006-2-28]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    partial class ucPatientInfo
    {
        public void CustomMethod() { }
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
            Neusoft.FrameWork.WinForms.Controls.NeuLabel lbDoct;
            Neusoft.FrameWork.WinForms.Controls.NeuLabel lbPact;
            Neusoft.FrameWork.WinForms.Controls.NeuLabel lbMCardNO;
            Neusoft.FrameWork.WinForms.Controls.NeuLabel lbRebate;
            FarPoint.Win.Spread.TipAppearance tipAppearance1 = new FarPoint.Win.Spread.TipAppearance();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType2 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
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
            this.fpRecipeSeq = new Neusoft.FrameWork.WinForms.Controls.NeuSpread();
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
            this.fpRecipeSeq_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.plMain = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            lbDoct = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            lbPact = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            lbMCardNO = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            lbRebate = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            ((System.ComponentModel.ISupportInitialize)(this.fpRecipeSeq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpRecipeSeq_Sheet1)).BeginInit();
            this.plMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbDoct
            // 
            lbDoct.AutoSize = true;
            lbDoct.Font = new System.Drawing.Font("宋体", 10F);
            lbDoct.ForeColor = System.Drawing.Color.Blue;
            lbDoct.Location = new System.Drawing.Point(6, 66);
            lbDoct.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbDoct.Name = "lbDoct";
            lbDoct.Size = new System.Drawing.Size(99, 20);
            lbDoct.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            lbDoct.TabIndex = 10;
            lbDoct.Text = "开立医生:";
            // 
            // lbPact
            // 
            lbPact.AutoSize = true;
            lbPact.Font = new System.Drawing.Font("宋体", 10F);
            lbPact.ForeColor = System.Drawing.Color.Blue;
            lbPact.Location = new System.Drawing.Point(567, 66);
            lbPact.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbPact.Name = "lbPact";
            lbPact.Size = new System.Drawing.Size(99, 20);
            lbPact.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            lbPact.TabIndex = 12;
            lbPact.Text = "结算种类:";
            // 
            // lbMCardNO
            // 
            lbMCardNO.AutoSize = true;
            lbMCardNO.Font = new System.Drawing.Font("宋体", 10F);
            lbMCardNO.Location = new System.Drawing.Point(6, 117);
            lbMCardNO.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbMCardNO.Name = "lbMCardNO";
            lbMCardNO.Size = new System.Drawing.Size(99, 20);
            lbMCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            lbMCardNO.TabIndex = 16;
            lbMCardNO.Text = "医疗证号:";
            // 
            // lbRebate
            // 
            lbRebate.AutoSize = true;
            lbRebate.Font = new System.Drawing.Font("宋体", 10F);
            lbRebate.Location = new System.Drawing.Point(286, 117);
            lbRebate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbRebate.Name = "lbRebate";
            lbRebate.Size = new System.Drawing.Size(99, 20);
            lbRebate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            lbRebate.TabIndex = 18;
            lbRebate.Text = "优惠编码:";
            // 
            // lbCardNO
            // 
            this.lbCardNO.AutoSize = true;
            this.lbCardNO.Font = new System.Drawing.Font("宋体", 10F);
            this.lbCardNO.ForeColor = System.Drawing.Color.Blue;
            this.lbCardNO.Location = new System.Drawing.Point(8, 15);
            this.lbCardNO.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbCardNO.Name = "lbCardNO";
            this.lbCardNO.Size = new System.Drawing.Size(109, 20);
            this.lbCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbCardNO.TabIndex = 0;
            this.lbCardNO.Text = "就诊卡号：";
            // 
            // tbCardNO
            // 
            this.tbCardNO.Font = new System.Drawing.Font("宋体", 10F);
            this.tbCardNO.IsEnter2Tab = false;
            this.tbCardNO.Location = new System.Drawing.Point(114, 9);
            this.tbCardNO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbCardNO.Name = "tbCardNO";
            this.tbCardNO.Size = new System.Drawing.Size(164, 30);
            this.tbCardNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbCardNO.TabIndex = 1;
            this.tbCardNO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbCardNO_KeyDown);
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("宋体", 10F);
            this.tbName.IsEnter2Tab = false;
            this.tbName.Location = new System.Drawing.Point(392, 9);
            this.tbName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(164, 30);
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
            this.lbName.Location = new System.Drawing.Point(286, 15);
            this.lbName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(99, 20);
            this.lbName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbName.TabIndex = 2;
            this.lbName.Text = "患者姓名:";
            // 
            // lbSex
            // 
            this.lbSex.AutoSize = true;
            this.lbSex.Font = new System.Drawing.Font("宋体", 10F);
            this.lbSex.ForeColor = System.Drawing.Color.Blue;
            this.lbSex.Location = new System.Drawing.Point(572, 15);
            this.lbSex.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSex.Name = "lbSex";
            this.lbSex.Size = new System.Drawing.Size(59, 20);
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
            this.cmbSex.IsShowIDAndName = false;
            this.cmbSex.Location = new System.Drawing.Point(636, 9);
            this.cmbSex.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbSex.Name = "cmbSex";
            this.cmbSex.ShowCustomerList = false;
            this.cmbSex.ShowID = false;
            this.cmbSex.Size = new System.Drawing.Size(72, 28);
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
            this.lbAge.Location = new System.Drawing.Point(710, 15);
            this.lbAge.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbAge.Name = "lbAge";
            this.lbAge.Size = new System.Drawing.Size(59, 20);
            this.lbAge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbAge.TabIndex = 6;
            this.lbAge.Text = "年龄:";
            // 
            // tbAge
            // 
            this.tbAge.Font = new System.Drawing.Font("宋体", 10F);
            this.tbAge.IsEnter2Tab = false;
            this.tbAge.Location = new System.Drawing.Point(774, 9);
            this.tbAge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbAge.Name = "tbAge";
            this.tbAge.Size = new System.Drawing.Size(74, 30);
            this.tbAge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbAge.TabIndex = 7;
            this.tbAge.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbAge_KeyDown);
            // 
            // lbRegDept
            // 
            this.lbRegDept.AutoSize = true;
            this.lbRegDept.Font = new System.Drawing.Font("宋体", 10F);
            this.lbRegDept.ForeColor = System.Drawing.Color.Blue;
            this.lbRegDept.Location = new System.Drawing.Point(286, 66);
            this.lbRegDept.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbRegDept.Name = "lbRegDept";
            this.lbRegDept.Size = new System.Drawing.Size(99, 20);
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
            this.cmbRegDept.IsShowIDAndName = false;
            this.cmbRegDept.Location = new System.Drawing.Point(392, 62);
            this.cmbRegDept.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbRegDept.Name = "cmbRegDept";
            this.cmbRegDept.ShowCustomerList = false;
            this.cmbRegDept.ShowID = false;
            this.cmbRegDept.Size = new System.Drawing.Size(164, 28);
            this.cmbRegDept.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbRegDept.TabIndex = 11;
            this.cmbRegDept.Tag = "";
            this.cmbRegDept.ToolBarUse = false;
            this.cmbRegDept.SelectedIndexChanged += new System.EventHandler(this.cmbRegDept_SelectedIndexChanged);
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
            this.cmbDoct.IsShowIDAndName = false;
            this.cmbDoct.Location = new System.Drawing.Point(114, 62);
            this.cmbDoct.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbDoct.Name = "cmbDoct";
            this.cmbDoct.ShowCustomerList = false;
            this.cmbDoct.ShowID = false;
            this.cmbDoct.Size = new System.Drawing.Size(164, 28);
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
            this.cmbPact.IsShowIDAndName = false;
            this.cmbPact.Location = new System.Drawing.Point(674, 62);
            this.cmbPact.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbPact.Name = "cmbPact";
            this.cmbPact.ShowCustomerList = false;
            this.cmbPact.ShowID = false;
            this.cmbPact.Size = new System.Drawing.Size(178, 28);
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
            this.lbClass.Location = new System.Drawing.Point(567, 117);
            this.lbClass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbClass.Name = "lbClass";
            this.lbClass.Size = new System.Drawing.Size(99, 20);
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
            this.cmbClass.IsShowIDAndName = false;
            this.cmbClass.Location = new System.Drawing.Point(674, 112);
            this.cmbClass.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.ShowCustomerList = false;
            this.cmbClass.ShowID = false;
            this.cmbClass.Size = new System.Drawing.Size(178, 28);
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
            this.tbMCardNO.Location = new System.Drawing.Point(114, 112);
            this.tbMCardNO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbMCardNO.Name = "tbMCardNO";
            this.tbMCardNO.Size = new System.Drawing.Size(164, 30);
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
            this.cmbRebate.IsShowIDAndName = false;
            this.cmbRebate.Location = new System.Drawing.Point(392, 112);
            this.cmbRebate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbRebate.Name = "cmbRebate";
            this.cmbRebate.ShowCustomerList = false;
            this.cmbRebate.ShowID = false;
            this.cmbRebate.Size = new System.Drawing.Size(164, 28);
            this.cmbRebate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbRebate.TabIndex = 19;
            this.cmbRebate.Tag = "";
            this.cmbRebate.ToolBarUse = false;
            this.cmbRebate.SelectedIndexChanged += new System.EventHandler(this.cmbRebate_SelectedIndexChanged);
            this.cmbRebate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbRebate_KeyDown);
            // 
            // fpRecipeSeq
            // 
            this.fpRecipeSeq.About = "3.0.2004.2005";
            this.fpRecipeSeq.AccessibleDescription = "fpRecipeSeq, Sheet1, Row 0, Column 0, ";
            this.fpRecipeSeq.BackColor = System.Drawing.Color.White;
            this.fpRecipeSeq.ContextMenu = this.neuContexMenu1;
            this.fpRecipeSeq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpRecipeSeq.FileName = "";
            this.fpRecipeSeq.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpRecipeSeq.IsAutoSaveGridStatus = false;
            this.fpRecipeSeq.IsCanCustomConfigColumn = false;
            this.fpRecipeSeq.Location = new System.Drawing.Point(870, 3);
            this.fpRecipeSeq.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fpRecipeSeq.Name = "fpRecipeSeq";
            this.fpRecipeSeq.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpRecipeSeq.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpRecipeSeq_Sheet1});
            this.fpRecipeSeq.Size = new System.Drawing.Size(645, 162);
            this.fpRecipeSeq.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.fpRecipeSeq.TabIndex = 1;
            tipAppearance1.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpRecipeSeq.TextTipAppearance = tipAppearance1;
            this.fpRecipeSeq.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpRecipeSeq.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpRecipeSeq_ButtonClicked);
            this.fpRecipeSeq.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpRecipeSeq_CellClick);
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
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "删除(&D)";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
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
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
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
            this.menuItem7.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 1;
            this.menuItem9.Text = "其他";
            this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 5;
            this.menuItem6.Text = "全选(A+R)";
            this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click_1);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 6;
            this.menuItem8.Text = "上一张处方(P+R)";
            this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 7;
            this.menuItem10.Text = "下一张处方(N+R)";
            this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
            // 
            // fpRecipeSeq_Sheet1
            // 
            this.fpRecipeSeq_Sheet1.Reset();
            this.fpRecipeSeq_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpRecipeSeq_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpRecipeSeq_Sheet1.ColumnCount = 4;
            this.fpRecipeSeq_Sheet1.ColumnHeader.RowCount = 0;
            this.fpRecipeSeq_Sheet1.RowCount = 0;
            this.fpRecipeSeq_Sheet1.RowHeader.ColumnCount = 0;
            this.fpRecipeSeq_Sheet1.Columns.Get(0).CellType = checkBoxCellType1;
            this.fpRecipeSeq_Sheet1.Columns.Get(0).Width = 22F;
            this.fpRecipeSeq_Sheet1.Columns.Get(1).CellType = textCellType1;
            this.fpRecipeSeq_Sheet1.Columns.Get(1).Locked = true;
            this.fpRecipeSeq_Sheet1.Columns.Get(1).Width = 113F;
            this.fpRecipeSeq_Sheet1.Columns.Get(2).CellType = textCellType2;
            this.fpRecipeSeq_Sheet1.Columns.Get(2).Locked = true;
            this.fpRecipeSeq_Sheet1.Columns.Get(2).Width = 110F;
            this.fpRecipeSeq_Sheet1.Columns.Get(3).CellType = numberCellType1;
            this.fpRecipeSeq_Sheet1.Columns.Get(3).Locked = true;
            this.fpRecipeSeq_Sheet1.Columns.Get(3).Width = 75F;
            this.fpRecipeSeq_Sheet1.GrayAreaBackColor = System.Drawing.Color.White;
            this.fpRecipeSeq_Sheet1.HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, System.Drawing.Color.LightGray, System.Drawing.SystemColors.ControlLightLight, System.Drawing.SystemColors.ControlDark, 0);
            this.fpRecipeSeq_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpRecipeSeq_Sheet1.VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, System.Drawing.Color.LightGray, System.Drawing.SystemColors.ControlLightLight, System.Drawing.SystemColors.ControlDark, 0);
            this.fpRecipeSeq_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            this.fpRecipeSeq.SetActiveViewport(0, 1, 0);
            // 
            // plMain
            // 
            this.plMain.Controls.Add(this.tbCardNO);
            this.plMain.Controls.Add(this.lbCardNO);
            this.plMain.Controls.Add(this.tbAge);
            this.plMain.Controls.Add(this.cmbClass);
            this.plMain.Controls.Add(this.lbClass);
            this.plMain.Controls.Add(this.tbName);
            this.plMain.Controls.Add(this.cmbRebate);
            this.plMain.Controls.Add(lbRebate);
            this.plMain.Controls.Add(this.tbMCardNO);
            this.plMain.Controls.Add(this.lbName);
            this.plMain.Controls.Add(lbMCardNO);
            this.plMain.Controls.Add(this.lbSex);
            this.plMain.Controls.Add(this.cmbSex);
            this.plMain.Controls.Add(this.lbAge);
            this.plMain.Controls.Add(this.cmbPact);
            this.plMain.Controls.Add(lbPact);
            this.plMain.Controls.Add(this.lbRegDept);
            this.plMain.Controls.Add(this.cmbDoct);
            this.plMain.Controls.Add(this.cmbRegDept);
            this.plMain.Controls.Add(lbDoct);
            this.plMain.Dock = System.Windows.Forms.DockStyle.Left;
            this.plMain.Location = new System.Drawing.Point(3, 3);
            this.plMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.plMain.Name = "plMain";
            this.plMain.Size = new System.Drawing.Size(867, 162);
            this.plMain.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.plMain.TabIndex = 0;
            // 
            // ucPatientInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fpRecipeSeq);
            this.Controls.Add(this.plMain);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ucPatientInfo";
            this.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.Size = new System.Drawing.Size(1518, 168);
            this.Load += new System.EventHandler(this.ucPatientInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fpRecipeSeq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpRecipeSeq_Sheet1)).EndInit();
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
        protected Neusoft.FrameWork.WinForms.Controls.NeuSpread fpRecipeSeq;
        protected FarPoint.Win.Spread.SheetView fpRecipeSeq_Sheet1;
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
    }
}
