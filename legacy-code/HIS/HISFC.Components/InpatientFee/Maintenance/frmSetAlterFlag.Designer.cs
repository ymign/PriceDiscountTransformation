namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    partial class frmSetAlterFlag
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
            this.lblPaeintInfo = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.cmbPatientType = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmbType = new Neusoft.FrameWork.WinForms.Controls.NeuComboBox(this.components);
            this.btStartAlter = new System.Windows.Forms.Button();
            this.btCloseAlter = new System.Windows.Forms.Button();
            this.btCloseWindow = new System.Windows.Forms.Button();
            this.neuGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPaeintInfo
            // 
            this.lblPaeintInfo.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPaeintInfo.ForeColor = System.Drawing.Color.Blue;
            this.lblPaeintInfo.Location = new System.Drawing.Point(16, 58);
            this.lblPaeintInfo.Name = "lblPaeintInfo";
            this.lblPaeintInfo.Size = new System.Drawing.Size(434, 48);
            this.lblPaeintInfo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblPaeintInfo.TabIndex = 0;
            this.lblPaeintInfo.Text = "患者信息";
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Controls.Add(this.lblPaeintInfo);
            this.neuGroupBox1.Controls.Add(this.cmbPatientType);
            this.neuGroupBox1.Controls.Add(this.neuLabel1);
            this.neuGroupBox1.Controls.Add(this.cmbType);
            this.neuGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuGroupBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(478, 132);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 3;
            this.neuGroupBox1.TabStop = false;
            // 
            // cmbPatientType
            // 
            this.cmbPatientType.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbPatientType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbPatientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatientType.Enabled = false;
            this.cmbPatientType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbPatientType.FormattingEnabled = true;
            this.cmbPatientType.IsEnter2Tab = false;
            this.cmbPatientType.IsFlat = false;
            this.cmbPatientType.IsLike = true;
            this.cmbPatientType.IsListOnly = false;
            this.cmbPatientType.IsPopForm = true;
            this.cmbPatientType.IsShowCustomerList = false;
            this.cmbPatientType.IsShowID = false;
            this.cmbPatientType.IsShowIDAndName = false;
            this.cmbPatientType.Items.AddRange(new object[] {
            "合同单位类别",
            "合同单位",
            "科室",
            "患者"});
            this.cmbPatientType.Location = new System.Drawing.Point(240, 19);
            this.cmbPatientType.Name = "cmbPatientType";
            this.cmbPatientType.ShowCustomerList = false;
            this.cmbPatientType.ShowID = false;
            this.cmbPatientType.Size = new System.Drawing.Size(210, 22);
            this.cmbPatientType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbPatientType.TabIndex = 6;
            this.cmbPatientType.Tag = "";
            this.cmbPatientType.ToolBarUse = false;
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel1.ForeColor = System.Drawing.Color.Black;
            this.neuLabel1.Location = new System.Drawing.Point(16, 22);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(77, 14);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 5;
            this.neuLabel1.Text = "患者类别：";
            // 
            // cmbType
            // 
            this.cmbType.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbType.Enabled = false;
            this.cmbType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbType.FormattingEnabled = true;
            this.cmbType.IsEnter2Tab = false;
            this.cmbType.IsFlat = false;
            this.cmbType.IsLike = true;
            this.cmbType.IsListOnly = false;
            this.cmbType.IsPopForm = true;
            this.cmbType.IsShowCustomerList = false;
            this.cmbType.IsShowID = false;
            this.cmbType.IsShowIDAndName = false;
            this.cmbType.Items.AddRange(new object[] {
            "患者"});
            this.cmbType.Location = new System.Drawing.Point(97, 19);
            this.cmbType.Name = "cmbType";
            this.cmbType.ShowCustomerList = false;
            this.cmbType.ShowID = false;
            this.cmbType.Size = new System.Drawing.Size(126, 22);
            this.cmbType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbType.TabIndex = 5;
            this.cmbType.Tag = "";
            this.cmbType.Text = "患者";
            this.cmbType.ToolBarUse = false;
            // 
            // btStartAlter
            // 
            this.btStartAlter.Location = new System.Drawing.Point(38, 141);
            this.btStartAlter.Name = "btStartAlter";
            this.btStartAlter.Size = new System.Drawing.Size(99, 27);
            this.btStartAlter.TabIndex = 5;
            this.btStartAlter.Text = "启用警戒线";
            this.btStartAlter.UseVisualStyleBackColor = true;
            // 
            // btCloseAlter
            // 
            this.btCloseAlter.Location = new System.Drawing.Point(190, 141);
            this.btCloseAlter.Name = "btCloseAlter";
            this.btCloseAlter.Size = new System.Drawing.Size(99, 27);
            this.btCloseAlter.TabIndex = 6;
            this.btCloseAlter.Text = "关闭警戒线";
            this.btCloseAlter.UseVisualStyleBackColor = true;
            // 
            // btCloseWindow
            // 
            this.btCloseWindow.Location = new System.Drawing.Point(342, 141);
            this.btCloseWindow.Name = "btCloseWindow";
            this.btCloseWindow.Size = new System.Drawing.Size(99, 27);
            this.btCloseWindow.TabIndex = 7;
            this.btCloseWindow.Text = "退出界面";
            this.btCloseWindow.UseVisualStyleBackColor = true;
            // 
            // frmSetAlterFlag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 177);
            this.Controls.Add(this.btCloseWindow);
            this.Controls.Add(this.btCloseAlter);
            this.Controls.Add(this.btStartAlter);
            this.Controls.Add(this.neuGroupBox1);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "frmSetAlterFlag";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "开关警戒线";
            this.neuGroupBox1.ResumeLayout(false);
            this.neuGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblPaeintInfo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbType;
        private System.Windows.Forms.Button btStartAlter;
        private System.Windows.Forms.Button btCloseAlter;
        private System.Windows.Forms.Button btCloseWindow;
        private Neusoft.FrameWork.WinForms.Controls.NeuComboBox cmbPatientType;
    }
}