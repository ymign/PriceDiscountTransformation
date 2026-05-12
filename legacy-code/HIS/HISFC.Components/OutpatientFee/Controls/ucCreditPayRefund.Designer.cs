namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucCreditPayRefund
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
            this.txtIdCard = new System.Windows.Forms.TextBox();
            this.btnQuery = new System.Windows.Forms.Button();
            this.dtSt = new System.Windows.Forms.DateTimePicker();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.hospitalSerialNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hospitalOrderNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.orderNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patientCard = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.refundAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.refundStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transactionNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblorderNo = new System.Windows.Forms.Label();
            this.lbltotalAmount = new System.Windows.Forms.Label();
            this.lbltransactionNo = new System.Windows.Forms.Label();
            this.lblrefundStatus = new System.Windows.Forms.Label();
            this.lblhisNo = new System.Windows.Forms.Label();
            this.lblpayStatus = new System.Windows.Forms.Label();
            this.lblpayTime = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblIDcard = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtIdCard
            // 
            this.txtIdCard.Location = new System.Drawing.Point(297, 9);
            this.txtIdCard.Name = "txtIdCard";
            this.txtIdCard.Size = new System.Drawing.Size(203, 21);
            this.txtIdCard.TabIndex = 0;
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(506, 8);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 1;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // dtSt
            // 
            this.dtSt.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtSt.Location = new System.Drawing.Point(83, 6);
            this.dtSt.Name = "dtSt";
            this.dtSt.Size = new System.Drawing.Size(133, 21);
            this.dtSt.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.hospitalSerialNo,
            this.hospitalOrderNo,
            this.orderNo,
            this.patientCard,
            this.patientName,
            this.payStatus,
            this.totalAmount,
            this.payTime,
            this.refundAmount,
            this.refundStatus,
            this.transactionNo});
            this.dataGridView1.Location = new System.Drawing.Point(3, 36);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1093, 443);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(222, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "患者身份证:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "缴费时间:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.txtIdCard);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnQuery);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dtSt);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1101, 484);
            this.panel1.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lblIDcard);
            this.panel2.Controls.Add(this.lblName);
            this.panel2.Controls.Add(this.lblpayTime);
            this.panel2.Controls.Add(this.lblpayStatus);
            this.panel2.Controls.Add(this.lblhisNo);
            this.panel2.Controls.Add(this.lblrefundStatus);
            this.panel2.Controls.Add(this.lbltransactionNo);
            this.panel2.Controls.Add(this.lbltotalAmount);
            this.panel2.Controls.Add(this.lblorderNo);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Location = new System.Drawing.Point(3, 493);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1101, 82);
            this.panel2.TabIndex = 6;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(587, 58);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(101, 12);
            this.label12.TabIndex = 2;
            this.label12.Text = "HIS 对应发票号：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(46, 58);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 2;
            this.label11.Text = "身份证号：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(70, 34);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "姓名：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(587, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(101, 12);
            this.label9.TabIndex = 2;
            this.label9.Text = "品台支付流水号：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(623, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "退款状态：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(326, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "支付状态：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(326, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "支付时间：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(350, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "金额：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "平台订单号：";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(966, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 74);
            this.button1.TabIndex = 1;
            this.button1.Text = "退款";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // hospitalSerialNo
            // 
            this.hospitalSerialNo.HeaderText = "医院流水号";
            this.hospitalSerialNo.Name = "hospitalSerialNo";
            this.hospitalSerialNo.ReadOnly = true;
            // 
            // hospitalOrderNo
            // 
            this.hospitalOrderNo.HeaderText = "医院订单号";
            this.hospitalOrderNo.Name = "hospitalOrderNo";
            this.hospitalOrderNo.ReadOnly = true;
            // 
            // orderNo
            // 
            this.orderNo.HeaderText = "平台订单号";
            this.orderNo.Name = "orderNo";
            this.orderNo.ReadOnly = true;
            // 
            // patientCard
            // 
            this.patientCard.HeaderText = "患者身份证";
            this.patientCard.Name = "patientCard";
            this.patientCard.ReadOnly = true;
            // 
            // patientName
            // 
            this.patientName.HeaderText = "姓名";
            this.patientName.Name = "patientName";
            this.patientName.ReadOnly = true;
            // 
            // payStatus
            // 
            this.payStatus.HeaderText = "支付状态";
            this.payStatus.Name = "payStatus";
            this.payStatus.ReadOnly = true;
            // 
            // totalAmount
            // 
            this.totalAmount.HeaderText = "总金额(单位：元)";
            this.totalAmount.Name = "totalAmount";
            this.totalAmount.ReadOnly = true;
            // 
            // payTime
            // 
            this.payTime.HeaderText = "支付成功时间";
            this.payTime.Name = "payTime";
            this.payTime.ReadOnly = true;
            // 
            // refundAmount
            // 
            this.refundAmount.HeaderText = "退款金额";
            this.refundAmount.Name = "refundAmount";
            this.refundAmount.ReadOnly = true;
            // 
            // refundStatus
            // 
            this.refundStatus.HeaderText = "退款状态";
            this.refundStatus.Name = "refundStatus";
            this.refundStatus.ReadOnly = true;
            // 
            // transactionNo
            // 
            this.transactionNo.HeaderText = "平台支付流水号";
            this.transactionNo.Name = "transactionNo";
            this.transactionNo.ReadOnly = true;
            // 
            // lblorderNo
            // 
            this.lblorderNo.AutoSize = true;
            this.lblorderNo.Location = new System.Drawing.Point(117, 7);
            this.lblorderNo.Name = "lblorderNo";
            this.lblorderNo.Size = new System.Drawing.Size(11, 12);
            this.lblorderNo.TabIndex = 3;
            this.lblorderNo.Text = "-";
            // 
            // lbltotalAmount
            // 
            this.lbltotalAmount.AutoSize = true;
            this.lbltotalAmount.Location = new System.Drawing.Point(396, 7);
            this.lbltotalAmount.Name = "lbltotalAmount";
            this.lbltotalAmount.Size = new System.Drawing.Size(11, 12);
            this.lbltotalAmount.TabIndex = 3;
            this.lbltotalAmount.Text = "-";
            // 
            // lbltransactionNo
            // 
            this.lbltransactionNo.AutoSize = true;
            this.lbltransactionNo.Location = new System.Drawing.Point(694, 7);
            this.lbltransactionNo.Name = "lbltransactionNo";
            this.lbltransactionNo.Size = new System.Drawing.Size(11, 12);
            this.lbltransactionNo.TabIndex = 3;
            this.lbltransactionNo.Text = "-";
            // 
            // lblrefundStatus
            // 
            this.lblrefundStatus.AutoSize = true;
            this.lblrefundStatus.Location = new System.Drawing.Point(694, 34);
            this.lblrefundStatus.Name = "lblrefundStatus";
            this.lblrefundStatus.Size = new System.Drawing.Size(11, 12);
            this.lblrefundStatus.TabIndex = 3;
            this.lblrefundStatus.Text = "-";
            // 
            // lblhisNo
            // 
            this.lblhisNo.AutoSize = true;
            this.lblhisNo.Location = new System.Drawing.Point(694, 58);
            this.lblhisNo.Name = "lblhisNo";
            this.lblhisNo.Size = new System.Drawing.Size(11, 12);
            this.lblhisNo.TabIndex = 3;
            this.lblhisNo.Text = "-";
            // 
            // lblpayStatus
            // 
            this.lblpayStatus.AutoSize = true;
            this.lblpayStatus.Location = new System.Drawing.Point(396, 58);
            this.lblpayStatus.Name = "lblpayStatus";
            this.lblpayStatus.Size = new System.Drawing.Size(11, 12);
            this.lblpayStatus.TabIndex = 3;
            this.lblpayStatus.Text = "-";
            // 
            // lblpayTime
            // 
            this.lblpayTime.AutoSize = true;
            this.lblpayTime.Location = new System.Drawing.Point(397, 34);
            this.lblpayTime.Name = "lblpayTime";
            this.lblpayTime.Size = new System.Drawing.Size(11, 12);
            this.lblpayTime.TabIndex = 3;
            this.lblpayTime.Text = "-";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(117, 34);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(11, 12);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "-";
            // 
            // lblIDcard
            // 
            this.lblIDcard.AutoSize = true;
            this.lblIDcard.Location = new System.Drawing.Point(117, 58);
            this.lblIDcard.Name = "lblIDcard";
            this.lblIDcard.Size = new System.Drawing.Size(11, 12);
            this.lblIDcard.TabIndex = 3;
            this.lblIDcard.Text = "-";
            // 
            // ucCreditPayRefund
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ucCreditPayRefund";
            this.Size = new System.Drawing.Size(1107, 580);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtIdCard;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.DateTimePicker dtSt;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DataGridViewTextBoxColumn hospitalSerialNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn hospitalOrderNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn orderNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn patientCard;
        private System.Windows.Forms.DataGridViewTextBoxColumn patientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn payStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn payTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn refundAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn refundStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn transactionNo;
        private System.Windows.Forms.Label lblorderNo;
        private System.Windows.Forms.Label lblpayStatus;
        private System.Windows.Forms.Label lblhisNo;
        private System.Windows.Forms.Label lblrefundStatus;
        private System.Windows.Forms.Label lbltransactionNo;
        private System.Windows.Forms.Label lbltotalAmount;
        private System.Windows.Forms.Label lblpayTime;
        private System.Windows.Forms.Label lblIDcard;
        private System.Windows.Forms.Label lblName;
    }
}
