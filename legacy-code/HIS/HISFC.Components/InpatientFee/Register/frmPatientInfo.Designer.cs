namespace Neusoft.HISFC.Components.InpatientFee.Register
{
    partial class frmPatientInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancle = new System.Windows.Forms.Button();
            this.btQuit = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtLinkMan = new System.Windows.Forms.TextBox();
            this.txtWorkAdress = new System.Windows.Forms.TextBox();
            this.txtIdentNo = new System.Windows.Forms.TextBox();
            this.txtBirthday = new System.Windows.Forms.TextBox();
            this.txtInDate = new System.Windows.Forms.TextBox();
            this.txtSex = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(19, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "所取患者数据摘要";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "姓名";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(182, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "身份证号";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "联系人";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(182, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "入院日期";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "出生日期";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(206, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "性别";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 176);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "单位（户口）地址";
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(46, 223);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 8;
            this.btOK.Text = "确定";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancle
            // 
            this.btCancle.Location = new System.Drawing.Point(177, 223);
            this.btCancle.Name = "btCancle";
            this.btCancle.Size = new System.Drawing.Size(75, 23);
            this.btCancle.TabIndex = 9;
            this.btCancle.Text = "取消";
            this.btCancle.UseVisualStyleBackColor = true;
            this.btCancle.Click += new System.EventHandler(this.btCancle_Click);
            // 
            // btQuit
            // 
            this.btQuit.Location = new System.Drawing.Point(308, 223);
            this.btQuit.Name = "btQuit";
            this.btQuit.Size = new System.Drawing.Size(75, 23);
            this.btQuit.TabIndex = 10;
            this.btQuit.Text = "退出";
            this.btQuit.UseVisualStyleBackColor = true;
            this.btQuit.Click += new System.EventHandler(this.btQuit_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(84, 37);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(68, 21);
            this.txtName.TabIndex = 11;
            // 
            // txtLinkMan
            // 
            this.txtLinkMan.Location = new System.Drawing.Point(84, 126);
            this.txtLinkMan.Name = "txtLinkMan";
            this.txtLinkMan.ReadOnly = true;
            this.txtLinkMan.Size = new System.Drawing.Size(68, 21);
            this.txtLinkMan.TabIndex = 12;
            // 
            // txtWorkAdress
            // 
            this.txtWorkAdress.Location = new System.Drawing.Point(126, 171);
            this.txtWorkAdress.Name = "txtWorkAdress";
            this.txtWorkAdress.ReadOnly = true;
            this.txtWorkAdress.Size = new System.Drawing.Size(233, 21);
            this.txtWorkAdress.TabIndex = 13;
            // 
            // txtIdentNo
            // 
            this.txtIdentNo.Location = new System.Drawing.Point(241, 126);
            this.txtIdentNo.Name = "txtIdentNo";
            this.txtIdentNo.ReadOnly = true;
            this.txtIdentNo.Size = new System.Drawing.Size(118, 21);
            this.txtIdentNo.TabIndex = 14;
            // 
            // txtBirthday
            // 
            this.txtBirthday.Location = new System.Drawing.Point(84, 81);
            this.txtBirthday.Name = "txtBirthday";
            this.txtBirthday.ReadOnly = true;
            this.txtBirthday.Size = new System.Drawing.Size(68, 21);
            this.txtBirthday.TabIndex = 15;
            // 
            // txtInDate
            // 
            this.txtInDate.Location = new System.Drawing.Point(241, 81);
            this.txtInDate.Name = "txtInDate";
            this.txtInDate.ReadOnly = true;
            this.txtInDate.Size = new System.Drawing.Size(68, 21);
            this.txtInDate.TabIndex = 16;
            // 
            // txtSex
            // 
            this.txtSex.Location = new System.Drawing.Point(241, 37);
            this.txtSex.Name = "txtSex";
            this.txtSex.ReadOnly = true;
            this.txtSex.Size = new System.Drawing.Size(68, 21);
            this.txtSex.TabIndex = 17;
            // 
            // frmPatientInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 266);
            this.Controls.Add(this.txtSex);
            this.Controls.Add(this.txtInDate);
            this.Controls.Add(this.txtBirthday);
            this.Controls.Add(this.txtIdentNo);
            this.Controls.Add(this.txtWorkAdress);
            this.Controls.Add(this.txtLinkMan);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.btQuit);
            this.Controls.Add(this.btCancle);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmPatientInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "患者摘要";
            this.Load += new System.EventHandler(this.frmPatientInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancle;
        private System.Windows.Forms.Button btQuit;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtLinkMan;
        private System.Windows.Forms.TextBox txtWorkAdress;
        private System.Windows.Forms.TextBox txtIdentNo;
        private System.Windows.Forms.TextBox txtBirthday;
        private System.Windows.Forms.TextBox txtInDate;
        private System.Windows.Forms.TextBox txtSex;
    }
}