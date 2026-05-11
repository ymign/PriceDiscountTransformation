namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmAccountPerPay
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
            this.gbPrePay = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.cmbPayType = new Neusoft.HISFC.Components.Common.Controls.cmbPayType(this.components);
            this.neuLabel13 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.txtpay = new Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox();
            this.neuLabel6 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.gboInfo = new Neusoft.FrameWork.WinForms.Controls.NeuGroupBox();
            this.lblRecharge = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblCharge = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.lblVacancy = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel4 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.neuLabel2 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.gbPrePay.SuspendLayout();
            this.gboInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPrePay
            // 
            this.gbPrePay.Controls.Add(this.cmbPayType);
            this.gbPrePay.Controls.Add(this.neuLabel13);
            this.gbPrePay.Controls.Add(this.txtpay);
            this.gbPrePay.Controls.Add(this.neuLabel6);
            this.gbPrePay.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbPrePay.Location = new System.Drawing.Point(0, 71);
            this.gbPrePay.Name = "gbPrePay";
            this.gbPrePay.Size = new System.Drawing.Size(465, 64);
            this.gbPrePay.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.gbPrePay.TabIndex = 1;
            this.gbPrePay.TabStop = false;
            // 
            // cmbPayType
            // 
            this.cmbPayType.ArrowBackColor = System.Drawing.Color.Silver;
            this.cmbPayType.FormattingEnabled = true;
            this.cmbPayType.IsEnter2Tab = false;
            this.cmbPayType.IsFlat = false;
            this.cmbPayType.IsLike = true;
            this.cmbPayType.IsListOnly = false;
            this.cmbPayType.IsPopForm = true;
            this.cmbPayType.IsShowCustomerList = false;
            this.cmbPayType.IsShowID = false;
            this.cmbPayType.Location = new System.Drawing.Point(68, 27);
            this.cmbPayType.Name = "cmbPayType";
            this.cmbPayType.Pop = true;
            this.cmbPayType.PopForm = null;
            this.cmbPayType.ShowCustomerList = false;
            this.cmbPayType.ShowID = false;
            this.cmbPayType.Size = new System.Drawing.Size(148, 20);
            this.cmbPayType.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Flat;
            this.cmbPayType.TabIndex = 0;
            this.cmbPayType.Tag = "";
            this.cmbPayType.ToolBarUse = false;
            this.cmbPayType.WorkUnit = "";
            this.cmbPayType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPayType_KeyDown);
            // 
            // neuLabel13
            // 
            this.neuLabel13.AutoSize = true;
            this.neuLabel13.Location = new System.Drawing.Point(8, 32);
            this.neuLabel13.Name = "neuLabel13";
            this.neuLabel13.Size = new System.Drawing.Size(65, 12);
            this.neuLabel13.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel13.TabIndex = 2;
            this.neuLabel13.Text = "支付方式：";
            // 
            // txtpay
            // 
            this.txtpay.AllowNegative = false;
            this.txtpay.BackColor = System.Drawing.Color.White;
            this.txtpay.IsAutoRemoveDecimalZero = false;
            this.txtpay.IsEnter2Tab = false;
            this.txtpay.Location = new System.Drawing.Point(311, 27);
            this.txtpay.Name = "txtpay";
            this.txtpay.NumericPrecision = 11;
            this.txtpay.NumericScaleOnFocus = 2;
            this.txtpay.NumericScaleOnLostFocus = 2;
            this.txtpay.NumericValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtpay.SetRange = new System.Drawing.Size(-1, -1);
            this.txtpay.Size = new System.Drawing.Size(124, 21);
            this.txtpay.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.txtpay.TabIndex = 1;
            this.txtpay.Text = "0.00";
            this.txtpay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtpay.UseGroupSeperator = true;
            this.txtpay.ZeroIsValid = true;
            this.txtpay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtpay_KeyDown);
            // 
            // neuLabel6
            // 
            this.neuLabel6.AutoSize = true;
            this.neuLabel6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel6.Location = new System.Drawing.Point(249, 32);
            this.neuLabel6.Name = "neuLabel6";
            this.neuLabel6.Size = new System.Drawing.Size(65, 12);
            this.neuLabel6.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel6.TabIndex = 3;
            this.neuLabel6.Text = "金    额：";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(250, 156);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "取消";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(136, 156);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "确定";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // gboInfo
            // 
            this.gboInfo.Controls.Add(this.lblRecharge);
            this.gboInfo.Controls.Add(this.lblCharge);
            this.gboInfo.Controls.Add(this.lblVacancy);
            this.gboInfo.Controls.Add(this.neuLabel1);
            this.gboInfo.Controls.Add(this.neuLabel4);
            this.gboInfo.Controls.Add(this.neuLabel2);
            this.gboInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.gboInfo.Location = new System.Drawing.Point(0, 0);
            this.gboInfo.Name = "gboInfo";
            this.gboInfo.Size = new System.Drawing.Size(465, 71);
            this.gboInfo.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.gboInfo.TabIndex = 0;
            this.gboInfo.TabStop = false;
            // 
            // lblRecharge
            // 
            this.lblRecharge.AutoSize = true;
            this.lblRecharge.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblRecharge.ForeColor = System.Drawing.Color.Red;
            this.lblRecharge.Location = new System.Drawing.Point(374, 33);
            this.lblRecharge.Name = "lblRecharge";
            this.lblRecharge.Size = new System.Drawing.Size(89, 19);
            this.lblRecharge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblRecharge.TabIndex = 2;
            this.lblRecharge.Text = "帐户余额";
            // 
            // lblCharge
            // 
            this.lblCharge.AutoSize = true;
            this.lblCharge.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCharge.ForeColor = System.Drawing.Color.Red;
            this.lblCharge.Location = new System.Drawing.Point(218, 33);
            this.lblCharge.Name = "lblCharge";
            this.lblCharge.Size = new System.Drawing.Size(89, 19);
            this.lblCharge.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblCharge.TabIndex = 2;
            this.lblCharge.Text = "帐户余额";
            // 
            // lblVacancy
            // 
            this.lblVacancy.AutoSize = true;
            this.lblVacancy.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblVacancy.ForeColor = System.Drawing.Color.Red;
            this.lblVacancy.Location = new System.Drawing.Point(62, 33);
            this.lblVacancy.Name = "lblVacancy";
            this.lblVacancy.Size = new System.Drawing.Size(89, 19);
            this.lblVacancy.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.lblVacancy.TabIndex = 2;
            this.lblVacancy.Text = "帐户余额";
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Location = new System.Drawing.Point(4, 37);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(65, 12);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 2;
            this.neuLabel1.Text = "帐户余额：";
            // 
            // neuLabel4
            // 
            this.neuLabel4.AutoSize = true;
            this.neuLabel4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel4.Location = new System.Drawing.Point(161, 37);
            this.neuLabel4.Name = "neuLabel4";
            this.neuLabel4.Size = new System.Drawing.Size(65, 12);
            this.neuLabel4.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel4.TabIndex = 3;
            this.neuLabel4.Text = "本次扣费：";
            // 
            // neuLabel2
            // 
            this.neuLabel2.AutoSize = true;
            this.neuLabel2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.neuLabel2.Location = new System.Drawing.Point(316, 37);
            this.neuLabel2.Name = "neuLabel2";
            this.neuLabel2.Size = new System.Drawing.Size(65, 12);
            this.neuLabel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel2.TabIndex = 3;
            this.neuLabel2.Text = "最少充值：";
            // 
            // frmAccountPerPay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 191);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.gbPrePay);
            this.Controls.Add(this.gboInfo);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "frmAccountPerPay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "预交金冲值";
            this.Load += new System.EventHandler(this.frmAccountPerPay_Load);
            this.gbPrePay.ResumeLayout(false);
            this.gbPrePay.PerformLayout();
            this.gboInfo.ResumeLayout(false);
            this.gboInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox gbPrePay;
        private Neusoft.HISFC.Components.Common.Controls.cmbPayType cmbPayType;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel13;
        private Neusoft.FrameWork.WinForms.Controls.NeuNumericTextBox txtpay;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel6;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private Neusoft.FrameWork.WinForms.Controls.NeuGroupBox gboInfo;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblVacancy;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblCharge;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel4;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel lblRecharge;
    }
}