namespace Neusoft.HISFC.Components.InpatientFee.Balance
{
    partial class frmBalancePaperTicketReprint
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
            this.panel3 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.ucQueryInpatientNo1 = new Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo();
            this.label7 = new System.Windows.Forms.Label();
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.txtpBillBatchCode = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel5 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtHisPaperNo = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel7 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtrandom = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.txtcreateTime = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel3 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtbillNo = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtbillBatchCode = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.groupBox3 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.txtFeeName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel9 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtFeedate = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.neuLabel8 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.label9 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtTo_Cost = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.label8 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtInvoiceNo = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.label1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtName = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.button1 = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.button2 = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.panel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.panel3.SuspendLayout();
            this.neuGroupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ucQueryInpatientNo1);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.neuGroupBox1);
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.button2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(860, 506);
            this.panel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.panel3.TabIndex = 1;
            // 
            // ucQueryInpatientNo1
            // 
            this.ucQueryInpatientNo1.DefaultInputType = 0;
            this.ucQueryInpatientNo1.InputType = 0;
            this.ucQueryInpatientNo1.IsDeptOnly = true;
            this.ucQueryInpatientNo1.Location = new System.Drawing.Point(15, 12);
            this.ucQueryInpatientNo1.Name = "ucQueryInpatientNo1";
            this.ucQueryInpatientNo1.PatientInState = "ALL";
            this.ucQueryInpatientNo1.ShowState = Neusoft.HISFC.Components.Common.Controls.enuShowState.All;
            this.ucQueryInpatientNo1.Size = new System.Drawing.Size(179, 27);
            this.ucQueryInpatientNo1.TabIndex = 25;
            this.ucQueryInpatientNo1.myEvent += new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(this.ucQueryInpatientNo1_myEvent);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(230, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(450, 23);
            this.label7.TabIndex = 24;
            this.label7.Text = "请注意:系统纸质票号码与电子平台不一致！";
            this.label7.Visible = false;
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.neuGroupBox1.Controls.Add(this.txtpBillBatchCode);
            this.neuGroupBox1.Controls.Add(this.neuLabel5);
            this.neuGroupBox1.Controls.Add(this.txtHisPaperNo);
            this.neuGroupBox1.Controls.Add(this.neuLabel7);
            this.neuGroupBox1.Controls.Add(this.txtrandom);
            this.neuGroupBox1.Controls.Add(this.txtcreateTime);
            this.neuGroupBox1.Controls.Add(this.neuLabel4);
            this.neuGroupBox1.Controls.Add(this.neuLabel3);
            this.neuGroupBox1.Controls.Add(this.txtbillNo);
            this.neuGroupBox1.Controls.Add(this.neuLabel2);
            this.neuGroupBox1.Controls.Add(this.txtbillBatchCode);
            this.neuGroupBox1.Controls.Add(this.neuLabel1);
            this.neuGroupBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuGroupBox1.Location = new System.Drawing.Point(13, 232);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(836, 207);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 24;
            this.neuGroupBox1.TabStop = false;
            this.neuGroupBox1.Text = "电子票据信息";
            // 
            // txtpBillBatchCode
            // 
            this.txtpBillBatchCode.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtpBillBatchCode.Font = new System.Drawing.Font("宋体", 12F);
            this.txtpBillBatchCode.IsEnter2Tab = false;
            this.txtpBillBatchCode.Location = new System.Drawing.Point(401, 108);
            this.txtpBillBatchCode.Name = "txtpBillBatchCode";
            this.txtpBillBatchCode.ReadOnly = true;
            this.txtpBillBatchCode.Size = new System.Drawing.Size(112, 26);
            this.txtpBillBatchCode.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtpBillBatchCode.TabIndex = 36;
            // 
            // neuLabel5
            // 
            this.neuLabel5.AutoSize = true;
            this.neuLabel5.Font = new System.Drawing.Font("宋体", 12F);
            this.neuLabel5.Location = new System.Drawing.Point(287, 115);
            this.neuLabel5.Name = "neuLabel5";
            this.neuLabel5.Size = new System.Drawing.Size(112, 16);
            this.neuLabel5.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel5.TabIndex = 35;
            this.neuLabel5.Text = "纸质票据代码:";
            // 
            // txtHisPaperNo
            // 
            this.txtHisPaperNo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtHisPaperNo.Font = new System.Drawing.Font("宋体", 12F);
            this.txtHisPaperNo.IsEnter2Tab = false;
            this.txtHisPaperNo.Location = new System.Drawing.Point(128, 115);
            this.txtHisPaperNo.Name = "txtHisPaperNo";
            this.txtHisPaperNo.ReadOnly = true;
            this.txtHisPaperNo.Size = new System.Drawing.Size(111, 26);
            this.txtHisPaperNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtHisPaperNo.TabIndex = 34;
            // 
            // neuLabel7
            // 
            this.neuLabel7.AutoSize = true;
            this.neuLabel7.Font = new System.Drawing.Font("宋体", 12F);
            this.neuLabel7.ForeColor = System.Drawing.Color.Red;
            this.neuLabel7.Location = new System.Drawing.Point(14, 117);
            this.neuLabel7.Name = "neuLabel7";
            this.neuLabel7.Size = new System.Drawing.Size(112, 16);
            this.neuLabel7.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel7.TabIndex = 32;
            this.neuLabel7.Text = "系统纸质号码:";
            // 
            // txtrandom
            // 
            this.txtrandom.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtrandom.Font = new System.Drawing.Font("宋体", 12F);
            this.txtrandom.IsEnter2Tab = false;
            this.txtrandom.Location = new System.Drawing.Point(128, 80);
            this.txtrandom.Name = "txtrandom";
            this.txtrandom.ReadOnly = true;
            this.txtrandom.Size = new System.Drawing.Size(111, 26);
            this.txtrandom.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtrandom.TabIndex = 27;
            // 
            // txtcreateTime
            // 
            this.txtcreateTime.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtcreateTime.Font = new System.Drawing.Font("宋体", 12F);
            this.txtcreateTime.IsEnter2Tab = false;
            this.txtcreateTime.Location = new System.Drawing.Point(401, 80);
            this.txtcreateTime.Name = "txtcreateTime";
            this.txtcreateTime.ReadOnly = true;
            this.txtcreateTime.Size = new System.Drawing.Size(112, 26);
            this.txtcreateTime.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtcreateTime.TabIndex = 26;
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Font = new System.Drawing.Font("宋体", 12F);
            this.neuLabel4.Location = new System.Drawing.Point(14, 82);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(112, 16);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 25;
            this.neuLabel4.Text = "检   验   码:";
            // 
            // neuLabel3
            // 
            this.neuLabel3.AutoSize = true;
            this.neuLabel3.Font = new System.Drawing.Font("宋体", 12F);
            this.neuLabel3.Location = new System.Drawing.Point(287, 82);
            this.neuLabel3.Name = "neuLabel3";
            this.neuLabel3.Size = new System.Drawing.Size(112, 16);
            this.neuLabel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel3.TabIndex = 24;
            this.neuLabel3.Text = "开 票 时 间 :";
            // 
            // txtbillNo
            // 
            this.txtbillNo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtbillNo.Font = new System.Drawing.Font("宋体", 12F);
            this.txtbillNo.IsEnter2Tab = false;
            this.txtbillNo.Location = new System.Drawing.Point(401, 37);
            this.txtbillNo.Name = "txtbillNo";
            this.txtbillNo.ReadOnly = true;
            this.txtbillNo.Size = new System.Drawing.Size(112, 26);
            this.txtbillNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtbillNo.TabIndex = 23;
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 12F);
            this.neuLabel2.Location = new System.Drawing.Point(287, 39);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(112, 16);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 22;
            this.neuLabel2.Text = "电子票据号码:";
            // 
            // txtbillBatchCode
            // 
            this.txtbillBatchCode.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtbillBatchCode.Font = new System.Drawing.Font("宋体", 12F);
            this.txtbillBatchCode.IsEnter2Tab = false;
            this.txtbillBatchCode.Location = new System.Drawing.Point(128, 39);
            this.txtbillBatchCode.Name = "txtbillBatchCode";
            this.txtbillBatchCode.ReadOnly = true;
            this.txtbillBatchCode.Size = new System.Drawing.Size(111, 26);
            this.txtbillBatchCode.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtbillBatchCode.TabIndex = 21;
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 12F);
            this.neuLabel1.Location = new System.Drawing.Point(14, 41);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(112, 16);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 7;
            this.neuLabel1.Text = "电子票据代码:";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.txtFeeName);
            this.groupBox3.Controls.Add(this.neuLabel9);
            this.groupBox3.Controls.Add(this.txtFeedate);
            this.groupBox3.Controls.Add(this.neuLabel8);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.txtTo_Cost);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtInvoiceNo);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.txtName);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(15, 48);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(836, 171);
            this.groupBox3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "发票信息";
            // 
            // txtFeeName
            // 
            this.txtFeeName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtFeeName.Font = new System.Drawing.Font("宋体", 12F);
            this.txtFeeName.IsEnter2Tab = false;
            this.txtFeeName.Location = new System.Drawing.Point(98, 99);
            this.txtFeeName.Name = "txtFeeName";
            this.txtFeeName.ReadOnly = true;
            this.txtFeeName.Size = new System.Drawing.Size(112, 26);
            this.txtFeeName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtFeeName.TabIndex = 27;
            // 
            // neuLabel9
            // 
            this.neuLabel9.Font = new System.Drawing.Font("宋体", 12F);
            this.neuLabel9.Location = new System.Drawing.Point(12, 101);
            this.neuLabel9.Name = "neuLabel9";
            this.neuLabel9.Size = new System.Drawing.Size(82, 23);
            this.neuLabel9.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel9.TabIndex = 26;
            this.neuLabel9.Text = "收费人:";
            // 
            // txtFeedate
            // 
            this.txtFeedate.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtFeedate.Font = new System.Drawing.Font("宋体", 12F);
            this.txtFeedate.IsEnter2Tab = false;
            this.txtFeedate.Location = new System.Drawing.Point(350, 58);
            this.txtFeedate.Name = "txtFeedate";
            this.txtFeedate.ReadOnly = true;
            this.txtFeedate.Size = new System.Drawing.Size(192, 26);
            this.txtFeedate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtFeedate.TabIndex = 25;
            // 
            // neuLabel8
            // 
            this.neuLabel8.Font = new System.Drawing.Font("宋体", 12F);
            this.neuLabel8.Location = new System.Drawing.Point(245, 63);
            this.neuLabel8.Name = "neuLabel8";
            this.neuLabel8.Size = new System.Drawing.Size(82, 23);
            this.neuLabel8.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel8.TabIndex = 24;
            this.neuLabel8.Text = "收费时间:";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("宋体", 12F);
            this.label9.Location = new System.Drawing.Point(12, 63);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 23);
            this.label9.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.label9.TabIndex = 23;
            this.label9.Text = "总金额:";
            // 
            // txtTo_Cost
            // 
            this.txtTo_Cost.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtTo_Cost.Font = new System.Drawing.Font("宋体", 12F);
            this.txtTo_Cost.IsEnter2Tab = false;
            this.txtTo_Cost.Location = new System.Drawing.Point(98, 61);
            this.txtTo_Cost.Name = "txtTo_Cost";
            this.txtTo_Cost.ReadOnly = true;
            this.txtTo_Cost.Size = new System.Drawing.Size(112, 26);
            this.txtTo_Cost.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtTo_Cost.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("宋体", 12F);
            this.label8.Location = new System.Drawing.Point(11, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 22);
            this.label8.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.label8.TabIndex = 21;
            this.label8.Text = "发 票 号:";
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtInvoiceNo.Font = new System.Drawing.Font("宋体", 12F);
            this.txtInvoiceNo.IsEnter2Tab = false;
            this.txtInvoiceNo.Location = new System.Drawing.Point(98, 23);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.ReadOnly = true;
            this.txtInvoiceNo.Size = new System.Drawing.Size(112, 26);
            this.txtInvoiceNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtInvoiceNo.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(245, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 23);
            this.label1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.label1.TabIndex = 6;
            this.label1.Text = "姓    名:";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtName.Font = new System.Drawing.Font("宋体", 12F);
            this.txtName.IsEnter2Tab = false;
            this.txtName.Location = new System.Drawing.Point(350, 23);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(112, 26);
            this.txtName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtName.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(336, 467);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.button1.TabIndex = 6;
            this.button1.Text = "打印";
            this.button1.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(553, 467);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.button2.TabIndex = 7;
            this.button2.Text = "退出";
            this.button2.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(860, 506);
            this.panel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.panel1.TabIndex = 2;
            // 
            // frmBalancePaperTicketReprint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 506);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBalancePaperTicketReprint";
            this.Text = "住院结算换开纸质票";
            this.panel3.ResumeLayout(false);
            this.neuGroupBox1.ResumeLayout(false);
            this.neuGroupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuPanel panel3;
        protected Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo ucQueryInpatientNo1;
        private System.Windows.Forms.Label label7;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtpBillBatchCode;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtHisPaperNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel7;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtrandom;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtcreateTime;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtbillNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtbillBatchCode;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox groupBox3;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtFeeName;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel9;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtFeedate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel8;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel label9;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtTo_Cost;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel label8;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtInvoiceNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel label1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtName;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton button1;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton button2;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel panel1;
    }
}