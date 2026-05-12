namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    partial class frmBizStatusModify
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblReasonLabel = new System.Windows.Forms.Label();
            this.lblPayMethodLabel = new System.Windows.Forms.Label();
            this.lblBizStateLabel = new System.Windows.Forms.Label();
            this.lblRemarkLabel = new System.Windows.Forms.Label();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmbChangeReason = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbPayModes = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.cmbSeeFlag = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(540, 63);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClose.ForeColor = System.Drawing.Color.Gray;
            this.btnClose.Location = new System.Drawing.Point(500, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(30, 30);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "×";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblTitle.Location = new System.Drawing.Point(32, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(164, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "运维干预控制界面";
            // 
            // lblReasonLabel
            // 
            this.lblReasonLabel.AutoSize = true;
            this.lblReasonLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblReasonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblReasonLabel.Location = new System.Drawing.Point(34, 75);
            this.lblReasonLabel.Name = "lblReasonLabel";
            this.lblReasonLabel.Size = new System.Drawing.Size(80, 17);
            this.lblReasonLabel.TabIndex = 2;
            this.lblReasonLabel.Text = "变更原因分类";
            // 
            // lblPayMethodLabel
            // 
            this.lblPayMethodLabel.AutoSize = true;
            this.lblPayMethodLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPayMethodLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblPayMethodLabel.Location = new System.Drawing.Point(34, 135);
            this.lblPayMethodLabel.Name = "lblPayMethodLabel";
            this.lblPayMethodLabel.Size = new System.Drawing.Size(80, 17);
            this.lblPayMethodLabel.TabIndex = 4;
            this.lblPayMethodLabel.Text = "目标支付方式";
            // 
            // lblBizStateLabel
            // 
            this.lblBizStateLabel.AutoSize = true;
            this.lblBizStateLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblBizStateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblBizStateLabel.Location = new System.Drawing.Point(34, 195);
            this.lblBizStateLabel.Name = "lblBizStateLabel";
            this.lblBizStateLabel.Size = new System.Drawing.Size(80, 17);
            this.lblBizStateLabel.TabIndex = 6;
            this.lblBizStateLabel.Text = "调整看诊状态";
            // 
            // lblRemarkLabel
            // 
            this.lblRemarkLabel.AutoSize = true;
            this.lblRemarkLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblRemarkLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblRemarkLabel.Location = new System.Drawing.Point(34, 255);
            this.lblRemarkLabel.Name = "lblRemarkLabel";
            this.lblRemarkLabel.Size = new System.Drawing.Size(112, 17);
            this.lblRemarkLabel.TabIndex = 8;
            this.lblRemarkLabel.Text = "操作备注(前计必填)";
            // 
            // txtRemark
            // 
            this.txtRemark.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtRemark.Location = new System.Drawing.Point(34, 275);
            this.txtRemark.MaxLength = 200;
            this.txtRemark.Multiline = true;
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.Size = new System.Drawing.Size(468, 60);
            this.txtRemark.TabIndex = 9;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.btnCancel.Location = new System.Drawing.Point(37, 364);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(121, 36);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "放弃修改";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(77)))), ((int)(((byte)(255)))));
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(381, 364);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(121, 36);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "确认修改";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // cmbChangeReason
            // 
            this.cmbChangeReason.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbChangeReason.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbChangeReason.FormattingEnabled = true;
            this.cmbChangeReason.IsEnter2Tab = false;
            this.cmbChangeReason.IsFlat = false;
            this.cmbChangeReason.IsLike = true;
            this.cmbChangeReason.IsListOnly = false;
            this.cmbChangeReason.IsPopForm = true;
            this.cmbChangeReason.IsShowCustomerList = false;
            this.cmbChangeReason.IsShowID = false;
            this.cmbChangeReason.IsShowIDAndName = false;
            this.cmbChangeReason.Location = new System.Drawing.Point(34, 95);
            this.cmbChangeReason.Name = "cmbChangeReason";
            this.cmbChangeReason.ShowCustomerList = false;
            this.cmbChangeReason.ShowID = false;
            this.cmbChangeReason.Size = new System.Drawing.Size(465, 20);
            this.cmbChangeReason.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbChangeReason.TabIndex = 15;
            this.cmbChangeReason.Tag = "";
            this.cmbChangeReason.ToolBarUse = false;
            // 
            // cmbPayModes
            // 
            this.cmbPayModes.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbPayModes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbPayModes.FormattingEnabled = true;
            this.cmbPayModes.IsEnter2Tab = false;
            this.cmbPayModes.IsFlat = false;
            this.cmbPayModes.IsLike = true;
            this.cmbPayModes.IsListOnly = false;
            this.cmbPayModes.IsPopForm = true;
            this.cmbPayModes.IsShowCustomerList = false;
            this.cmbPayModes.IsShowID = false;
            this.cmbPayModes.IsShowIDAndName = false;
            this.cmbPayModes.Location = new System.Drawing.Point(37, 155);
            this.cmbPayModes.Name = "cmbPayModes";
            this.cmbPayModes.ShowCustomerList = false;
            this.cmbPayModes.ShowID = false;
            this.cmbPayModes.Size = new System.Drawing.Size(462, 20);
            this.cmbPayModes.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbPayModes.TabIndex = 16;
            this.cmbPayModes.Tag = "";
            this.cmbPayModes.ToolBarUse = false;
            // 
            // cmbSeeFlag
            // 
            this.cmbSeeFlag.ArrowBackColor = System.Drawing.SystemColors.Control;
            this.cmbSeeFlag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbSeeFlag.FormattingEnabled = true;
            this.cmbSeeFlag.IsEnter2Tab = false;
            this.cmbSeeFlag.IsFlat = false;
            this.cmbSeeFlag.IsLike = true;
            this.cmbSeeFlag.IsListOnly = false;
            this.cmbSeeFlag.IsPopForm = true;
            this.cmbSeeFlag.IsShowCustomerList = false;
            this.cmbSeeFlag.IsShowID = false;
            this.cmbSeeFlag.IsShowIDAndName = false;
            this.cmbSeeFlag.Location = new System.Drawing.Point(37, 215);
            this.cmbSeeFlag.Name = "cmbSeeFlag";
            this.cmbSeeFlag.ShowCustomerList = false;
            this.cmbSeeFlag.ShowID = false;
            this.cmbSeeFlag.Size = new System.Drawing.Size(462, 20);
            this.cmbSeeFlag.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbSeeFlag.TabIndex = 17;
            this.cmbSeeFlag.Tag = "";
            this.cmbSeeFlag.ToolBarUse = false;
            // 
            // frmBizStatusModify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(540, 433);
            this.Controls.Add(this.cmbSeeFlag);
            this.Controls.Add(this.cmbPayModes);
            this.Controls.Add(this.cmbChangeReason);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtRemark);
            this.Controls.Add(this.lblRemarkLabel);
            this.Controls.Add(this.lblBizStateLabel);
            this.Controls.Add(this.lblPayMethodLabel);
            this.Controls.Add(this.lblReasonLabel);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmBizStatusModify";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "运维干预控制台";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblReasonLabel;
        private System.Windows.Forms.Label lblPayMethodLabel;
        private System.Windows.Forms.Label lblBizStateLabel;
        private System.Windows.Forms.Label lblRemarkLabel;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbChangeReason;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbPayModes;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbSeeFlag;
    }
}
