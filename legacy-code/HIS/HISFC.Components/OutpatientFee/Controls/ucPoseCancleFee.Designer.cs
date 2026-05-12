namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucPoseCancleFee
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
            this.txtMoney = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPZH = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtMoney
            // 
            this.txtMoney.Location = new System.Drawing.Point(174, 148);
            this.txtMoney.Name = "txtMoney";
            this.txtMoney.Size = new System.Drawing.Size(100, 21);
            this.txtMoney.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 153);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "金额(元为单位)：";
            // 
            // txtPZH
            // 
            this.txtPZH.Location = new System.Drawing.Point(129, 120);
            this.txtPZH.Name = "txtPZH";
            this.txtPZH.Size = new System.Drawing.Size(100, 21);
            this.txtPZH.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(70, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "凭证号：";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(334, 153);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "退费";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "实体社保卡",
            "电子社保卡"});
            this.comboBox1.Location = new System.Drawing.Point(334, 122);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 10;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(257, 122);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(53, 12);
            this.lblType.TabIndex = 11;
            this.lblType.Text = "退款方式";
            // 
            // ucPoseCancleFee
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.txtMoney);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPZH);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "ucPoseCancleFee";
            this.Size = new System.Drawing.Size(530, 328);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMoney;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPZH;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lblType;
    }
}
