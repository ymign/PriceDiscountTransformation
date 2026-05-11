namespace Neusoft.HISFC.Components.InpatientFee.Controls
{
    partial class ucTransType
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmbTransType1 = new Neusoft.HISFC.Components.InpatientFee.Controls.cmbTransType();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "请选择支付方式：";
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(51, 46);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(58, 19);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "确定";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(164, 46);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(58, 19);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "取消";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmbTransType1
            // 
            this.cmbTransType1.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbTransType1.FormattingEnabled = true;
            this.cmbTransType1.IsEnter2Tab = false;
            this.cmbTransType1.IsFlat = false;
            this.cmbTransType1.IsLike = true;
            this.cmbTransType1.IsListOnly = false;
            this.cmbTransType1.IsPopForm = true;
            this.cmbTransType1.IsShowCustomerList = false;
            this.cmbTransType1.IsShowID = false;
            this.cmbTransType1.Location = new System.Drawing.Point(133, 15);
            this.cmbTransType1.Name = "cmbTransType1";
            this.cmbTransType1.Pop = true;
            this.cmbTransType1.PopForm = null;
            this.cmbTransType1.ShowCustomerList = false;
            this.cmbTransType1.ShowID = false;
            this.cmbTransType1.Size = new System.Drawing.Size(124, 20);
            this.cmbTransType1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbTransType1.TabIndex = 0;
            this.cmbTransType1.Tag = "";
            this.cmbTransType1.ToolBarUse = false;
            this.cmbTransType1.WorkUnit = "";
            // 
            // ucTransType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(286, 77);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbTransType1);
            this.KeyPreview = true;
            this.Name = "ucTransType";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "支付方式选择";
            this.Load += new System.EventHandler(this.ucTransType_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private cmbTransType cmbTransType1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
    }
}