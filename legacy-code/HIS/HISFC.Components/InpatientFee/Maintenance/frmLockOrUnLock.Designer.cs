namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    partial class frmLockOrUnLock
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
            this.neuGroupBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.lbPatientNo = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lbName = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.gbDate = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.dtEnd = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.dtBegin = new Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker();
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel5 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.btnLock = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btnUnLock = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.btnExit = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.lbStatus = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuGroupBox1.SuspendLayout();
            this.gbDate.SuspendLayout();
            this.SuspendLayout();
            // 
            // neuGroupBox1
            // 
            this.neuGroupBox1.Controls.Add(this.lbStatus);
            this.neuGroupBox1.Controls.Add(this.lbPatientNo);
            this.neuGroupBox1.Controls.Add(this.lbName);
            this.neuGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.neuGroupBox1.Name = "neuGroupBox1";
            this.neuGroupBox1.Size = new System.Drawing.Size(317, 45);
            this.neuGroupBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuGroupBox1.TabIndex = 1;
            this.neuGroupBox1.TabStop = false;
            // 
            // lbPatientNo
            // 
            this.lbPatientNo.AutoSize = true;
            this.lbPatientNo.ForeColor = System.Drawing.Color.Blue;
            this.lbPatientNo.Location = new System.Drawing.Point(129, 22);
            this.lbPatientNo.Name = "lbPatientNo";
            this.lbPatientNo.Size = new System.Drawing.Size(47, 12);
            this.lbPatientNo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbPatientNo.TabIndex = 3;
            this.lbPatientNo.Text = "住院号:";
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.ForeColor = System.Drawing.Color.Blue;
            this.lbName.Location = new System.Drawing.Point(28, 22);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(35, 12);
            this.lbName.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbName.TabIndex = 0;
            this.lbName.Text = "姓名:";
            // 
            // gbDate
            // 
            this.gbDate.Controls.Add(this.dtEnd);
            this.gbDate.Controls.Add(this.dtBegin);
            this.gbDate.Controls.Add(this.neuLabel4);
            this.gbDate.Controls.Add(this.neuLabel5);
            this.gbDate.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbDate.Location = new System.Drawing.Point(0, 45);
            this.gbDate.Name = "gbDate";
            this.gbDate.Size = new System.Drawing.Size(317, 73);
            this.gbDate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.gbDate.TabIndex = 2;
            this.gbDate.TabStop = false;
            // 
            // dtEnd
            // 
            this.dtEnd.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEnd.IsEnter2Tab = false;
            this.dtEnd.Location = new System.Drawing.Point(108, 45);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Size = new System.Drawing.Size(159, 21);
            this.dtEnd.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtEnd.TabIndex = 9;
            // 
            // dtBegin
            // 
            this.dtBegin.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtBegin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtBegin.IsEnter2Tab = false;
            this.dtBegin.Location = new System.Drawing.Point(109, 12);
            this.dtBegin.Name = "dtBegin";
            this.dtBegin.Size = new System.Drawing.Size(158, 21);
            this.dtBegin.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.dtBegin.TabIndex = 8;
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Location = new System.Drawing.Point(28, 17);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(53, 12);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 0;
            this.neuLabel4.Text = "开始时间";
            // 
            // neuLabel5
            // 
            this.neuLabel5.AutoSize = true;
            this.neuLabel5.Location = new System.Drawing.Point(28, 49);
            this.neuLabel5.Name = "neuLabel5";
            this.neuLabel5.Size = new System.Drawing.Size(53, 12);
            this.neuLabel5.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel5.TabIndex = 0;
            this.neuLabel5.Text = "结束时间";
            // 
            // btnLock
            // 
            this.btnLock.AutoSize = true;
            this.btnLock.Location = new System.Drawing.Point(111, 124);
            this.btnLock.Name = "btnLock";
            this.btnLock.Size = new System.Drawing.Size(63, 23);
            this.btnLock.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnLock.TabIndex = 13;
            this.btnLock.Text = "关锁";
            this.btnLock.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnLock.UseVisualStyleBackColor = true;
            this.btnLock.Click += new System.EventHandler(this.btnLock_Click);
            // 
            // btnUnLock
            // 
            this.btnUnLock.AutoSize = true;
            this.btnUnLock.Location = new System.Drawing.Point(30, 124);
            this.btnUnLock.Name = "btnUnLock";
            this.btnUnLock.Size = new System.Drawing.Size(63, 23);
            this.btnUnLock.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnUnLock.TabIndex = 12;
            this.btnUnLock.Text = "开锁";
            this.btnUnLock.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnUnLock.UseVisualStyleBackColor = true;
            this.btnUnLock.Click += new System.EventHandler(this.btnUnLock_Click);
            // 
            // btnExit
            // 
            this.btnExit.AutoSize = true;
            this.btnExit.Location = new System.Drawing.Point(224, 124);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(63, 23);
            this.btnExit.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnExit.TabIndex = 14;
            this.btnExit.Text = "退出";
            this.btnExit.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbStatus.ForeColor = System.Drawing.Color.Red;
            this.lbStatus.Location = new System.Drawing.Point(258, 22);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(44, 12);
            this.lbStatus.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lbStatus.TabIndex = 4;
            this.lbStatus.Text = "已开锁";
            // 
            // frmLockOrUnLock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 170);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnLock);
            this.Controls.Add(this.btnUnLock);
            this.Controls.Add(this.gbDate);
            this.Controls.Add(this.neuGroupBox1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLockOrUnLock";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "开锁/关锁";
            this.Load += new System.EventHandler(this.frmLockOrUnLock_Load);
            this.neuGroupBox1.ResumeLayout(false);
            this.neuGroupBox1.PerformLayout();
            this.gbDate.ResumeLayout(false);
            this.gbDate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox neuGroupBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox gbDate;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtEnd;
        private Neusoft.FrameWork.WinForms.Controls.NeuDateTimePicker dtBegin;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel5;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnLock;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnUnLock;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbPatientNo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbName;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnExit;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lbStatus;
    }
}