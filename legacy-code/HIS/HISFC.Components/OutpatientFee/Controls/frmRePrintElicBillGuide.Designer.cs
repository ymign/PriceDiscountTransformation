namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class frmRePrintElicBillGuide
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
            this.panel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.panel3 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
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
            this.label6 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtCardNo = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.button1 = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.button2 = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuButton1 = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(574, 220);
            this.panel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.panel1.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.txtCardNo);
            this.panel3.Controls.Add(this.neuButton1);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.button2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(574, 220);
            this.panel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.panel3.TabIndex = 1;
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
            this.groupBox3.Location = new System.Drawing.Point(11, 42);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(550, 136);
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
            this.txtFeedate.Size = new System.Drawing.Size(159, 26);
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
            this.txtName.Size = new System.Drawing.Size(159, 26);
            this.txtName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtName.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("宋体", 12F);
            this.label6.Location = new System.Drawing.Point(26, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 23);
            this.label6.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.label6.TabIndex = 18;
            this.label6.Text = "病 历 号:";
            // 
            // txtCardNo
            // 
            this.txtCardNo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCardNo.Font = new System.Drawing.Font("宋体", 12F);
            this.txtCardNo.IsEnter2Tab = false;
            this.txtCardNo.Location = new System.Drawing.Point(113, 9);
            this.txtCardNo.Name = "txtCardNo";
            this.txtCardNo.Size = new System.Drawing.Size(112, 26);
            this.txtCardNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtCardNo.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(340, 190);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.button1.TabIndex = 6;
            this.button1.Text = "打印";
            this.button1.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(454, 190);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 23);
            this.button2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.button2.TabIndex = 7;
            this.button2.Text = "退出";
            this.button2.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            // 
            // neuButton1
            // 
            this.neuButton1.ForeColor = System.Drawing.Color.Blue;
            this.neuButton1.Location = new System.Drawing.Point(207, 190);
            this.neuButton1.Name = "neuButton1";
            this.neuButton1.Size = new System.Drawing.Size(127, 23);
            this.neuButton1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuButton1.TabIndex = 6;
            this.neuButton1.Text = "查看(打印)电子票";
            this.neuButton1.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.neuButton1.Click += new System.EventHandler(this.neuButton1_Click);
            // 
            // frmRePrintElicBillGuide
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(574, 220);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRePrintElicBillGuide";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "补打指引单（打印查看电子票据）";
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuPanel panel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel panel3;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel label1;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton button2;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton button1;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtName;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel label6;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox groupBox3;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel label8;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel label9;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtInvoiceNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtTo_Cost;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtCardNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtFeedate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel8;
        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox txtFeeName;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel9;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton neuButton1;
    }
}