namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmLimitation
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
            this.cmbLimit = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.lblMemo = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnNO = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 181);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "是否满足报销条件?";
            // 
            // cmbLimit
            // 
            this.cmbLimit.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbLimit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbLimit.FormattingEnabled = true;
            this.cmbLimit.IsEnter2Tab = false;
            this.cmbLimit.IsFlat = false;
            this.cmbLimit.IsLike = true;
            this.cmbLimit.IsListOnly = false;
            this.cmbLimit.IsPopForm = true;
            this.cmbLimit.IsShowCustomerList = false;
            this.cmbLimit.IsShowID = false;
            this.cmbLimit.IsShowIDAndName = false;
            this.cmbLimit.Location = new System.Drawing.Point(195, 80);
            this.cmbLimit.Name = "cmbLimit";
            this.cmbLimit.ShowCustomerList = false;
            this.cmbLimit.ShowID = false;
            this.cmbLimit.Size = new System.Drawing.Size(57, 20);
            this.cmbLimit.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbLimit.TabIndex = 1;
            this.cmbLimit.Tag = "";
            this.cmbLimit.ToolBarUse = false;
            // 
            // lblMemo
            // 
            this.lblMemo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMemo.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMemo.ForeColor = System.Drawing.Color.Red;
            this.lblMemo.Location = new System.Drawing.Point(12, 5);
            this.lblMemo.Name = "lblMemo";
            this.lblMemo.Size = new System.Drawing.Size(417, 162);
            this.lblMemo.TabIndex = 2;
            this.lblMemo.Text = "格列卫支付范围限定为费城染色体阳性的慢性髓性白血病的慢性期、加速期或急变期，胃肠道间质瘤；昕维和格尼可的支付范围限定为费城染色体阳性的慢性髓性白血病的慢性期、加速" +
                "期或急变期";
            this.lblMemo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(251, 218);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(86, 35);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "满足";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnNO
            // 
            this.btnNO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNO.Location = new System.Drawing.Point(343, 216);
            this.btnNO.Name = "btnNO";
            this.btnNO.Size = new System.Drawing.Size(86, 37);
            this.btnNO.TabIndex = 4;
            this.btnNO.Text = "不满足";
            this.btnNO.UseVisualStyleBackColor = true;
            this.btnNO.Click += new System.EventHandler(this.btnNO_Click);
            // 
            // frmLimitation
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 265);
            this.ControlBox = false;
            this.Controls.Add(this.btnNO);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblMemo);
            this.Controls.Add(this.cmbLimit);
            this.Controls.Add(this.label1);
            this.Name = "frmLimitation";
            this.Text = "限制条件药品信息提示";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbLimit;
        private System.Windows.Forms.Label lblMemo;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnNO;
    }
}