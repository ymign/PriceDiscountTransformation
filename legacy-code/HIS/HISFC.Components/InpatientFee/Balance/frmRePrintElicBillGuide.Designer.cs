namespace Neusoft.HISFC.Components.InpatientFee.Balance
{
    partial class frmRePrintElicBillGuide
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
            this.ucQueryInpatientNo = new Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo();
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
            this.btnLink = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.panel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ucQueryInpatientNo);
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Controls.Add(this.btnLink);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.button2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(586, 232);
            this.panel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.panel3.TabIndex = 1;
            // 
            // ucQueryInpatientNo
            // 
            this.ucQueryInpatientNo.DefaultInputType = 0;
            this.ucQueryInpatientNo.InputType = 0;
            this.ucQueryInpatientNo.IsDeptOnly = true;
            this.ucQueryInpatientNo.Location = new System.Drawing.Point(15, 15);
            this.ucQueryInpatientNo.Name = "ucQueryInpatientNo";
            this.ucQueryInpatientNo.PatientInState = "ALL";
            this.ucQueryInpatientNo.ShowState = Neusoft.HISFC.Components.Common.Controls.enuShowState.All;
            this.ucQueryInpatientNo.Size = new System.Drawing.Size(179, 27);
            this.ucQueryInpatientNo.TabIndex = 20;
            this.ucQueryInpatientNo.myEvent += new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(this.ucQueryInpatientNo_myEvent);
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
            this.groupBox3.Size = new System.Drawing.Size(559, 137);
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
            this.txtName.Size = new System.Drawing.Size(192, 26);
            this.txtName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtName.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(336, 197);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.button1.TabIndex = 6;
            this.button1.Text = "打印";
            this.button1.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(467, 197);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(107, 23);
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
            this.panel1.Size = new System.Drawing.Size(586, 232);
            this.panel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.panel1.TabIndex = 2;
            // 
            // btnLink
            // 
            this.btnLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLink.ForeColor = System.Drawing.Color.MediumBlue;
            this.btnLink.Location = new System.Drawing.Point(205, 197);
            this.btnLink.Name = "btnLink";
            this.btnLink.Size = new System.Drawing.Size(115, 23);
            this.btnLink.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnLink.TabIndex = 6;
            this.btnLink.Text = "查看(打印)电子票据";
            this.btnLink.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // frmRePrintElicBillGuide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 232);
            this.Controls.Add(this.panel1);
            this.Name = "frmRePrintElicBillGuide";
            this.Text = "指引单补打";
            this.panel3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuPanel panel3;
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
        protected Neusoft.HISFC.Components.Common.Controls.ucQueryInpatientNo ucQueryInpatientNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnLink;
    }
}