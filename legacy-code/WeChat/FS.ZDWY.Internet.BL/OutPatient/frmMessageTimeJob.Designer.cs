namespace AutoMessage
{
    partial class frmMessageTimeJob
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtUrlFee = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFeeInterval = new System.Windows.Forms.TextBox();
            this.btnFee = new System.Windows.Forms.Button();
            this.btnWaiting = new System.Windows.Forms.Button();
            this.txtWaitingInteval = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUrlWaiting = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSchema = new System.Windows.Forms.Button();
            this.txtSchemaInterval = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUrlSchema = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnRegAccept = new System.Windows.Forms.Button();
            this.txtRegInteval = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtRegAccept = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnRegCancel = new System.Windows.Forms.Button();
            this.txtRegCencelInteval = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtRegCancel = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnMinSize = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnBegin = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "待缴费提醒";
            // 
            // txtUrlFee
            // 
            this.txtUrlFee.Location = new System.Drawing.Point(81, 30);
            this.txtUrlFee.Margin = new System.Windows.Forms.Padding(2);
            this.txtUrlFee.Name = "txtUrlFee";
            this.txtUrlFee.Size = new System.Drawing.Size(253, 21);
            this.txtUrlFee.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(343, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "刷新间隔(S)：";
            // 
            // txtFeeInterval
            // 
            this.txtFeeInterval.Location = new System.Drawing.Point(431, 30);
            this.txtFeeInterval.Margin = new System.Windows.Forms.Padding(2);
            this.txtFeeInterval.Name = "txtFeeInterval";
            this.txtFeeInterval.Size = new System.Drawing.Size(58, 21);
            this.txtFeeInterval.TabIndex = 3;
            this.txtFeeInterval.Text = "1";
            // 
            // btnFee
            // 
            this.btnFee.Location = new System.Drawing.Point(491, 31);
            this.btnFee.Margin = new System.Windows.Forms.Padding(2);
            this.btnFee.Name = "btnFee";
            this.btnFee.Size = new System.Drawing.Size(50, 20);
            this.btnFee.TabIndex = 4;
            this.btnFee.Text = "设置";
            this.btnFee.UseVisualStyleBackColor = true;
            // 
            // btnWaiting
            // 
            this.btnWaiting.Location = new System.Drawing.Point(491, 62);
            this.btnWaiting.Margin = new System.Windows.Forms.Padding(2);
            this.btnWaiting.Name = "btnWaiting";
            this.btnWaiting.Size = new System.Drawing.Size(50, 20);
            this.btnWaiting.TabIndex = 9;
            this.btnWaiting.Text = "设置";
            this.btnWaiting.UseVisualStyleBackColor = true;
            this.btnWaiting.Visible = false;
            // 
            // txtWaitingInteval
            // 
            this.txtWaitingInteval.Location = new System.Drawing.Point(431, 61);
            this.txtWaitingInteval.Margin = new System.Windows.Forms.Padding(2);
            this.txtWaitingInteval.Name = "txtWaitingInteval";
            this.txtWaitingInteval.Size = new System.Drawing.Size(58, 21);
            this.txtWaitingInteval.TabIndex = 8;
            this.txtWaitingInteval.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(343, 63);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "刷新间隔(S)：";
            this.label3.Visible = false;
            // 
            // txtUrlWaiting
            // 
            this.txtUrlWaiting.Location = new System.Drawing.Point(81, 61);
            this.txtUrlWaiting.Margin = new System.Windows.Forms.Padding(2);
            this.txtUrlWaiting.Name = "txtUrlWaiting";
            this.txtUrlWaiting.Size = new System.Drawing.Size(253, 21);
            this.txtUrlWaiting.TabIndex = 6;
            this.txtUrlWaiting.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 63);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "排队推送";
            this.label4.Visible = false;
            // 
            // btnSchema
            // 
            this.btnSchema.Location = new System.Drawing.Point(491, 91);
            this.btnSchema.Margin = new System.Windows.Forms.Padding(2);
            this.btnSchema.Name = "btnSchema";
            this.btnSchema.Size = new System.Drawing.Size(50, 20);
            this.btnSchema.TabIndex = 14;
            this.btnSchema.Text = "设置";
            this.btnSchema.UseVisualStyleBackColor = true;
            this.btnSchema.Visible = false;
            // 
            // txtSchemaInterval
            // 
            this.txtSchemaInterval.Location = new System.Drawing.Point(431, 91);
            this.txtSchemaInterval.Margin = new System.Windows.Forms.Padding(2);
            this.txtSchemaInterval.Name = "txtSchemaInterval";
            this.txtSchemaInterval.Size = new System.Drawing.Size(58, 21);
            this.txtSchemaInterval.TabIndex = 13;
            this.txtSchemaInterval.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(343, 93);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "刷新间隔(S)：";
            this.label5.Visible = false;
            // 
            // txtUrlSchema
            // 
            this.txtUrlSchema.Location = new System.Drawing.Point(81, 91);
            this.txtUrlSchema.Margin = new System.Windows.Forms.Padding(2);
            this.txtUrlSchema.Name = "txtUrlSchema";
            this.txtUrlSchema.Size = new System.Drawing.Size(253, 21);
            this.txtUrlSchema.TabIndex = 11;
            this.txtUrlSchema.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1, 93);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "医生停诊通知";
            this.label6.Visible = false;
            // 
            // btnRegAccept
            // 
            this.btnRegAccept.Location = new System.Drawing.Point(491, 122);
            this.btnRegAccept.Margin = new System.Windows.Forms.Padding(2);
            this.btnRegAccept.Name = "btnRegAccept";
            this.btnRegAccept.Size = new System.Drawing.Size(50, 20);
            this.btnRegAccept.TabIndex = 19;
            this.btnRegAccept.Text = "设置";
            this.btnRegAccept.UseVisualStyleBackColor = true;
            this.btnRegAccept.Visible = false;
            // 
            // txtRegInteval
            // 
            this.txtRegInteval.Location = new System.Drawing.Point(431, 121);
            this.txtRegInteval.Margin = new System.Windows.Forms.Padding(2);
            this.txtRegInteval.Name = "txtRegInteval";
            this.txtRegInteval.Size = new System.Drawing.Size(58, 21);
            this.txtRegInteval.TabIndex = 18;
            this.txtRegInteval.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(343, 123);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 12);
            this.label7.TabIndex = 17;
            this.label7.Text = "刷新间隔(S)：";
            this.label7.Visible = false;
            // 
            // txtRegAccept
            // 
            this.txtRegAccept.Location = new System.Drawing.Point(81, 121);
            this.txtRegAccept.Margin = new System.Windows.Forms.Padding(2);
            this.txtRegAccept.Name = "txtRegAccept";
            this.txtRegAccept.Size = new System.Drawing.Size(253, 21);
            this.txtRegAccept.TabIndex = 16;
            this.txtRegAccept.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 123);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 15;
            this.label8.Text = "挂号接诊";
            this.label8.Visible = false;
            // 
            // btnRegCancel
            // 
            this.btnRegCancel.Location = new System.Drawing.Point(491, 151);
            this.btnRegCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnRegCancel.Name = "btnRegCancel";
            this.btnRegCancel.Size = new System.Drawing.Size(50, 20);
            this.btnRegCancel.TabIndex = 24;
            this.btnRegCancel.Text = "设置";
            this.btnRegCancel.UseVisualStyleBackColor = true;
            this.btnRegCancel.Visible = false;
            // 
            // txtRegCencelInteval
            // 
            this.txtRegCencelInteval.Location = new System.Drawing.Point(431, 150);
            this.txtRegCencelInteval.Margin = new System.Windows.Forms.Padding(2);
            this.txtRegCencelInteval.Name = "txtRegCencelInteval";
            this.txtRegCencelInteval.Size = new System.Drawing.Size(58, 21);
            this.txtRegCencelInteval.TabIndex = 23;
            this.txtRegCencelInteval.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(343, 152);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "刷新间隔(S)：";
            this.label9.Visible = false;
            // 
            // txtRegCancel
            // 
            this.txtRegCancel.Location = new System.Drawing.Point(81, 150);
            this.txtRegCancel.Margin = new System.Windows.Forms.Padding(2);
            this.txtRegCancel.Name = "txtRegCancel";
            this.txtRegCancel.Size = new System.Drawing.Size(253, 21);
            this.txtRegCancel.TabIndex = 21;
            this.txtRegCancel.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 152);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 20;
            this.label10.Text = "取消挂号";
            this.label10.Visible = false;
            // 
            // btnMinSize
            // 
            this.btnMinSize.Location = new System.Drawing.Point(354, 185);
            this.btnMinSize.Margin = new System.Windows.Forms.Padding(2);
            this.btnMinSize.Name = "btnMinSize";
            this.btnMinSize.Size = new System.Drawing.Size(57, 20);
            this.btnMinSize.TabIndex = 25;
            this.btnMinSize.Text = "最小化";
            this.btnMinSize.UseVisualStyleBackColor = true;
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(485, 185);
            this.btnQuit.Margin = new System.Windows.Forms.Padding(2);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(50, 20);
            this.btnQuit.TabIndex = 26;
            this.btnQuit.Text = "退出";
            this.btnQuit.UseVisualStyleBackColor = true;
            // 
            // btnBegin
            // 
            this.btnBegin.Location = new System.Drawing.Point(415, 185);
            this.btnBegin.Margin = new System.Windows.Forms.Padding(2);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(66, 20);
            this.btnBegin.TabIndex = 27;
            this.btnBegin.Text = "开始运行";
            this.btnBegin.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            // 
            // timer3
            // 
            this.timer3.Enabled = true;
            this.timer3.Interval = 1000;
            // 
            // timer4
            // 
            this.timer4.Enabled = true;
            this.timer4.Interval = 1000;
            // 
            // timer5
            // 
            this.timer5.Enabled = true;
            this.timer5.Interval = 1000;
            // 
            // frmMessageTimeJob
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 223);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnMinSize);
            this.Controls.Add(this.btnRegCancel);
            this.Controls.Add(this.txtRegCencelInteval);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtRegCancel);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnRegAccept);
            this.Controls.Add(this.txtRegInteval);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtRegAccept);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnSchema);
            this.Controls.Add(this.txtSchemaInterval);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtUrlSchema);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnWaiting);
            this.Controls.Add(this.txtWaitingInteval);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtUrlWaiting);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnFee);
            this.Controls.Add(this.txtFeeInterval);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUrlFee);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmMessageTimeJob";
            this.Text = "frmMessageTimeJob";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUrlFee;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFeeInterval;
        private System.Windows.Forms.Button btnFee;
        private System.Windows.Forms.Button btnWaiting;
        private System.Windows.Forms.TextBox txtWaitingInteval;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUrlWaiting;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSchema;
        private System.Windows.Forms.TextBox txtSchemaInterval;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUrlSchema;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnRegAccept;
        private System.Windows.Forms.TextBox txtRegInteval;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtRegAccept;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnRegCancel;
        private System.Windows.Forms.TextBox txtRegCencelInteval;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtRegCancel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnMinSize;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnBegin;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Timer timer4;
        private System.Windows.Forms.Timer timer5;
    }
}