namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmYDZFRefund
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbUSER = new System.Windows.Forms.RadioButton();
            this.rdbYB = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnYBRefund = new System.Windows.Forms.Button();
            this.lblYBRefundStatus = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblXJRefundStatus = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRefund = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPayOrderId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblOrderType = new System.Windows.Forms.Label();
            this.lblPAYMODE = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(184)))), ((int)(((byte)(255)))));
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOk);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(527, 300);
            this.panel2.TabIndex = 5;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Window_MouseDown);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.label7.Location = new System.Drawing.Point(6, 262);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 28);
            this.label7.TabIndex = 4;
            this.label7.Text = "移动支付退款";
            this.label7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Window_MouseDown);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(184)))), ((int)(((byte)(255)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.lblXJRefundStatus);
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnRefund);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtPayOrderId);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lblOrderType);
            this.panel1.Controls.Add(this.lblPAYMODE);
            this.panel1.Location = new System.Drawing.Point(11, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(506, 242);
            this.panel1.TabIndex = 3;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Window_MouseDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbUSER);
            this.groupBox1.Controls.Add(this.rdbYB);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnYBRefund);
            this.groupBox1.Controls.Add(this.lblYBRefundStatus);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(5, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(494, 104);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "医保退款";
            // 
            // rdbUSER
            // 
            this.rdbUSER.AutoSize = true;
            this.rdbUSER.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rdbUSER.Location = new System.Drawing.Point(418, 20);
            this.rdbUSER.Name = "rdbUSER";
            this.rdbUSER.Size = new System.Drawing.Size(57, 29);
            this.rdbUSER.TabIndex = 6;
            this.rdbUSER.Text = "用户信息";
            this.rdbUSER.UseVisualStyleBackColor = true;
            // 
            // rdbYB
            // 
            this.rdbYB.AutoSize = true;
            this.rdbYB.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rdbYB.Checked = true;
            this.rdbYB.Location = new System.Drawing.Point(342, 20);
            this.rdbYB.Name = "rdbYB";
            this.rdbYB.Size = new System.Drawing.Size(57, 29);
            this.rdbYB.TabIndex = 6;
            this.rdbYB.TabStop = true;
            this.rdbYB.Text = "电子凭证";
            this.rdbYB.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(226, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "选择医保退款凭证:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(6, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "医保退款状态:";
            // 
            // btnYBRefund
            // 
            this.btnYBRefund.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(231)))), ((int)(((byte)(255)))));
            this.btnYBRefund.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnYBRefund.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnYBRefund.ForeColor = System.Drawing.Color.DimGray;
            this.btnYBRefund.Location = new System.Drawing.Point(358, 59);
            this.btnYBRefund.Name = "btnYBRefund";
            this.btnYBRefund.Size = new System.Drawing.Size(114, 32);
            this.btnYBRefund.TabIndex = 2;
            this.btnYBRefund.Text = "退医保";
            this.btnYBRefund.UseVisualStyleBackColor = false;
            this.btnYBRefund.Click += new System.EventHandler(this.btnYBRefund_Click);
            // 
            // lblYBRefundStatus
            // 
            this.lblYBRefundStatus.AutoSize = true;
            this.lblYBRefundStatus.Font = new System.Drawing.Font("宋体", 12F);
            this.lblYBRefundStatus.ForeColor = System.Drawing.Color.White;
            this.lblYBRefundStatus.Location = new System.Drawing.Point(122, 36);
            this.lblYBRefundStatus.Name = "lblYBRefundStatus";
            this.lblYBRefundStatus.Size = new System.Drawing.Size(16, 16);
            this.lblYBRefundStatus.TabIndex = 4;
            this.lblYBRefundStatus.Text = "-";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(14, 199);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "现金退款状态:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(100, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "-";
            // 
            // lblXJRefundStatus
            // 
            this.lblXJRefundStatus.AutoSize = true;
            this.lblXJRefundStatus.Font = new System.Drawing.Font("宋体", 12F);
            this.lblXJRefundStatus.ForeColor = System.Drawing.Color.White;
            this.lblXJRefundStatus.Location = new System.Drawing.Point(130, 199);
            this.lblXJRefundStatus.Name = "lblXJRefundStatus";
            this.lblXJRefundStatus.Size = new System.Drawing.Size(16, 16);
            this.lblXJRefundStatus.TabIndex = 4;
            this.lblXJRefundStatus.Text = "-";
            // 
            // linkLabel1
            // 
            this.linkLabel1.ActiveLinkColor = System.Drawing.Color.Blue;
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel1.ForeColor = System.Drawing.Color.White;
            this.linkLabel1.Location = new System.Drawing.Point(365, 42);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(77, 12);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "刷新订单状态";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(14, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "支付方式:";
            // 
            // btnRefund
            // 
            this.btnRefund.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(231)))), ((int)(((byte)(255)))));
            this.btnRefund.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRefund.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRefund.ForeColor = System.Drawing.Color.DimGray;
            this.btnRefund.Location = new System.Drawing.Point(363, 198);
            this.btnRefund.Name = "btnRefund";
            this.btnRefund.Size = new System.Drawing.Size(114, 32);
            this.btnRefund.TabIndex = 2;
            this.btnRefund.Text = "退现金";
            this.btnRefund.UseVisualStyleBackColor = false;
            this.btnRefund.Click += new System.EventHandler(this.btnRefund_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(14, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "支付订单号:";
            // 
            // txtPayOrderId
            // 
            this.txtPayOrderId.Font = new System.Drawing.Font("宋体", 12F);
            this.txtPayOrderId.Location = new System.Drawing.Point(116, 9);
            this.txtPayOrderId.Name = "txtPayOrderId";
            this.txtPayOrderId.ReadOnly = true;
            this.txtPayOrderId.Size = new System.Drawing.Size(222, 26);
            this.txtPayOrderId.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(344, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "订单类型:";
            // 
            // lblOrderType
            // 
            this.lblOrderType.AutoSize = true;
            this.lblOrderType.Font = new System.Drawing.Font("宋体", 12F);
            this.lblOrderType.ForeColor = System.Drawing.Color.White;
            this.lblOrderType.Location = new System.Drawing.Point(420, 12);
            this.lblOrderType.Name = "lblOrderType";
            this.lblOrderType.Size = new System.Drawing.Size(16, 16);
            this.lblOrderType.TabIndex = 0;
            this.lblOrderType.Text = "-";
            // 
            // lblPAYMODE
            // 
            this.lblPAYMODE.AutoSize = true;
            this.lblPAYMODE.Font = new System.Drawing.Font("宋体", 12F);
            this.lblPAYMODE.ForeColor = System.Drawing.Color.White;
            this.lblPAYMODE.Location = new System.Drawing.Point(100, 42);
            this.lblPAYMODE.Name = "lblPAYMODE";
            this.lblPAYMODE.Size = new System.Drawing.Size(16, 16);
            this.lblPAYMODE.TabIndex = 0;
            this.lblPAYMODE.Text = "-";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(231)))), ((int)(((byte)(255)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnCancel.Location = new System.Drawing.Point(419, 260);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 29);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(231)))), ((int)(((byte)(255)))));
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOk.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnOk.Location = new System.Drawing.Point(321, 261);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(92, 29);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "完成";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // frmYDZFRefund
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(184)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(529, 302);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmYDZFRefund";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmYDZFRefund";
            this.Load += new System.EventHandler(this.frmYDZFRefund_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRefund;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPayOrderId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblOrderType;
        private System.Windows.Forms.Label lblPAYMODE;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblYBRefundStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblXJRefundStatus;
        private System.Windows.Forms.Button btnYBRefund;
        private System.Windows.Forms.RadioButton rdbYB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbUSER;
        private System.Windows.Forms.Label label8;

    }
}