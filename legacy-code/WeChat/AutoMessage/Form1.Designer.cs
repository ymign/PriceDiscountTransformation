namespace AutoMessage
{
    partial class frmTest
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCharge = new System.Windows.Forms.Button();
            this.btnWaiting = new System.Windows.Forms.Button();
            this.btnStopSchema = new System.Windows.Forms.Button();
            this.btnAcceptReg = new System.Windows.Forms.Button();
            this.btnCancelReg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCharge
            // 
            this.btnCharge.Location = new System.Drawing.Point(25, 48);
            this.btnCharge.Name = "btnCharge";
            this.btnCharge.Size = new System.Drawing.Size(142, 36);
            this.btnCharge.TabIndex = 0;
            this.btnCharge.Text = "待缴费提醒";
            this.btnCharge.UseVisualStyleBackColor = true;
            this.btnCharge.Click += new System.EventHandler(this.btnCharge_Click);
            // 
            // btnWaiting
            // 
            this.btnWaiting.Location = new System.Drawing.Point(185, 48);
            this.btnWaiting.Name = "btnWaiting";
            this.btnWaiting.Size = new System.Drawing.Size(142, 36);
            this.btnWaiting.TabIndex = 1;
            this.btnWaiting.Text = "排队推送";
            this.btnWaiting.UseVisualStyleBackColor = true;
            this.btnWaiting.Click += new System.EventHandler(this.btnWaiting_Click);
            // 
            // btnStopSchema
            // 
            this.btnStopSchema.Location = new System.Drawing.Point(336, 48);
            this.btnStopSchema.Name = "btnStopSchema";
            this.btnStopSchema.Size = new System.Drawing.Size(142, 36);
            this.btnStopSchema.TabIndex = 2;
            this.btnStopSchema.Text = "医生停诊通知";
            this.btnStopSchema.UseVisualStyleBackColor = true;
            this.btnStopSchema.Click += new System.EventHandler(this.btnStopSchema_Click);
            // 
            // btnAcceptReg
            // 
            this.btnAcceptReg.Location = new System.Drawing.Point(497, 48);
            this.btnAcceptReg.Name = "btnAcceptReg";
            this.btnAcceptReg.Size = new System.Drawing.Size(142, 36);
            this.btnAcceptReg.TabIndex = 3;
            this.btnAcceptReg.Text = "挂号接诊";
            this.btnAcceptReg.UseVisualStyleBackColor = true;
            this.btnAcceptReg.Click += new System.EventHandler(this.btnAcceptReg_Click);
            // 
            // btnCancelReg
            // 
            this.btnCancelReg.Location = new System.Drawing.Point(25, 95);
            this.btnCancelReg.Name = "btnCancelReg";
            this.btnCancelReg.Size = new System.Drawing.Size(142, 36);
            this.btnCancelReg.TabIndex = 4;
            this.btnCancelReg.Text = "取消挂号";
            this.btnCancelReg.UseVisualStyleBackColor = true;
            this.btnCancelReg.Click += new System.EventHandler(this.btnCancelReg_Click);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 143);
            this.Controls.Add(this.btnCancelReg);
            this.Controls.Add(this.btnAcceptReg);
            this.Controls.Add(this.btnStopSchema);
            this.Controls.Add(this.btnWaiting);
            this.Controls.Add(this.btnCharge);
            this.Name = "frmTest";
            this.Text = "接口调用测试";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCharge;
        private System.Windows.Forms.Button btnWaiting;
        private System.Windows.Forms.Button btnStopSchema;
        private System.Windows.Forms.Button btnAcceptReg;
        private System.Windows.Forms.Button btnCancelReg;
    }
}

