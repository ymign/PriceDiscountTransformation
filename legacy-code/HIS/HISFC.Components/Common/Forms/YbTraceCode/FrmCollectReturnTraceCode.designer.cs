namespace Neusoft.HISFC.Components.Common.Forms.YbTraceCode
{
    partial class FrmCollectReturnTraceCode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCollectReturnTraceCode));
            FarPoint.Win.Spread.TipAppearance tipAppearance3 = new FarPoint.Win.Spread.TipAppearance();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblPatientInfo = new System.Windows.Forms.Label();
            this.pictureBoxClose = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlScan = new System.Windows.Forms.Panel();
            this.lblScanTip = new System.Windows.Forms.Label();
            this.txtScanCode = new System.Windows.Forms.TextBox();
            this.lblScanIcon = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlToast = new System.Windows.Forms.Panel();
            this.lblToast = new System.Windows.Forms.Label();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btn_AllNoReturn = new System.Windows.Forms.Button();
            this.btn_AutoAssign = new System.Windows.Forms.Button();
            this.lblScanned = new System.Windows.Forms.Label();
            this.lblPending = new System.Windows.Forms.Label();
            this.lblStats = new System.Windows.Forms.Label();
            this.lblShortcuts = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.timerToast = new System.Windows.Forms.Timer(this.components);
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).BeginInit();
            this.pnlScan.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlToast.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(167)))), ((int)(((byte)(155)))));
            this.pnlHeader.Controls.Add(this.lblPatientInfo);
            this.pnlHeader.Controls.Add(this.pictureBoxClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1154, 50);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblPatientInfo
            // 
            this.lblPatientInfo.AutoSize = true;
            this.lblPatientInfo.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPatientInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(255)))), ((int)(((byte)(250)))));
            this.lblPatientInfo.Location = new System.Drawing.Point(239, 18);
            this.lblPatientInfo.Name = "lblPatientInfo";
            this.lblPatientInfo.Size = new System.Drawing.Size(191, 16);
            this.lblPatientInfo.TabIndex = 18;
            this.lblPatientInfo.Text = "曾翠玲 | 门诊号: 4665";
            this.lblPatientInfo.Click += new System.EventHandler(this.label1_Click);
            // 
            // pictureBoxClose
            // 
            this.pictureBoxClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxClose.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxClose.Image")));
            this.pictureBoxClose.Location = new System.Drawing.Point(1116, 11);
            this.pictureBoxClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBoxClose.Name = "pictureBoxClose";
            this.pictureBoxClose.Size = new System.Drawing.Size(24, 18);
            this.pictureBoxClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxClose.TabIndex = 17;
            this.pictureBoxClose.TabStop = false;
            this.pictureBoxClose.Click += new System.EventHandler(this.pictureBoxClose_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1154, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "     门诊退费 - 追溯码采集";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlScan
            // 
            this.pnlScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(251)))), ((int)(((byte)(250)))));
            this.pnlScan.Controls.Add(this.lblScanTip);
            this.pnlScan.Controls.Add(this.txtScanCode);
            this.pnlScan.Controls.Add(this.lblScanIcon);
            this.pnlScan.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlScan.Location = new System.Drawing.Point(0, 50);
            this.pnlScan.Name = "pnlScan";
            this.pnlScan.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.pnlScan.Size = new System.Drawing.Size(1154, 70);
            this.pnlScan.TabIndex = 1;
            // 
            // lblScanTip
            // 
            this.lblScanTip.AutoSize = true;
            this.lblScanTip.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblScanTip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(144)))), ((int)(((byte)(156)))));
            this.lblScanTip.Location = new System.Drawing.Point(620, 25);
            this.lblScanTip.Name = "lblScanTip";
            this.lblScanTip.Size = new System.Drawing.Size(301, 17);
            this.lblScanTip.TabIndex = 2;
            this.lblScanTip.Text = "使用扫码枪/高拍仪扫描药品追溯码，系统自动匹配药品";
            // 
            // txtScanCode
            // 
            this.txtScanCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScanCode.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.txtScanCode.Location = new System.Drawing.Point(100, 18);
            this.txtScanCode.Name = "txtScanCode";
            this.txtScanCode.Size = new System.Drawing.Size(500, 29);
            this.txtScanCode.TabIndex = 1;
            this.txtScanCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtScanCode_KeyPress);
            // 
            // lblScanIcon
            // 
            this.lblScanIcon.AutoSize = true;
            this.lblScanIcon.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblScanIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(167)))), ((int)(((byte)(155)))));
            this.lblScanIcon.Location = new System.Drawing.Point(20, 22);
            this.lblScanIcon.Name = "lblScanIcon";
            this.lblScanIcon.Size = new System.Drawing.Size(79, 19);
            this.lblScanIcon.TabIndex = 0;
            this.lblScanIcon.Text = "扫码输入：";
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlContent.Controls.Add(this.pnlToast);
            this.pnlContent.Controls.Add(this.fpSpread1);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 120);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.pnlContent.Size = new System.Drawing.Size(1154, 504);
            this.pnlContent.TabIndex = 2;
            // 
            // pnlToast
            // 
            this.pnlToast.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pnlToast.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(251)))), ((int)(((byte)(246)))));
            this.pnlToast.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlToast.Controls.Add(this.lblToast);
            this.pnlToast.Location = new System.Drawing.Point(346, 187);
            this.pnlToast.Name = "pnlToast";
            this.pnlToast.Size = new System.Drawing.Size(520, 60);
            this.pnlToast.TabIndex = 4;
            this.pnlToast.Visible = false;
            // 
            // lblToast
            // 
            this.lblToast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblToast.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Bold);
            this.lblToast.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblToast.Location = new System.Drawing.Point(0, 0);
            this.lblToast.Name = "lblToast";
            this.lblToast.Size = new System.Drawing.Size(518, 58);
            this.lblToast.TabIndex = 0;
            this.lblToast.Text = "操作成功";
            this.lblToast.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fpSpread1
            // 
            this.fpSpread1.About = "3.0.2004.2005";
            this.fpSpread1.AccessibleDescription = "fpSpread1";
            this.fpSpread1.BackColor = System.Drawing.Color.White;
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.Never;
            this.fpSpread1.Location = new System.Drawing.Point(10, 0);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(1134, 504);
            this.fpSpread1.TabIndex = 0;
            tipAppearance3.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance3.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpread1.TextTipAppearance = tipAppearance3;
            this.fpSpread1.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 12;
            this.fpSpread1_Sheet1.RowCount = 5;
            this.fpSpread1_Sheet1.GrayAreaBackColor = System.Drawing.Color.White;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlFooter.Controls.Add(this.btn_AllNoReturn);
            this.pnlFooter.Controls.Add(this.btn_AutoAssign);
            this.pnlFooter.Controls.Add(this.lblScanned);
            this.pnlFooter.Controls.Add(this.lblPending);
            this.pnlFooter.Controls.Add(this.lblStats);
            this.pnlFooter.Controls.Add(this.lblShortcuts);
            this.pnlFooter.Controls.Add(this.btnReset);
            this.pnlFooter.Controls.Add(this.btnCancel);
            this.pnlFooter.Controls.Add(this.btnSubmit);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 624);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1154, 60);
            this.pnlFooter.TabIndex = 3;
            // 
            // btn_AllNoReturn
            // 
            this.btn_AllNoReturn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btn_AllNoReturn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(232)))), ((int)(((byte)(226)))));
            this.btn_AllNoReturn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_AllNoReturn.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_AllNoReturn.ForeColor = System.Drawing.Color.White;
            this.btn_AllNoReturn.Location = new System.Drawing.Point(860, 12);
            this.btn_AllNoReturn.Name = "btn_AllNoReturn";
            this.btn_AllNoReturn.Size = new System.Drawing.Size(90, 32);
            this.btn_AllNoReturn.TabIndex = 7;
            this.btn_AllNoReturn.Text = "全部不退";
            this.btn_AllNoReturn.UseVisualStyleBackColor = false;
            this.btn_AllNoReturn.Click += new System.EventHandler(this.btn_AllNoReturn_Click);
            // 
            // btn_AutoAssign
            // 
            this.btn_AutoAssign.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btn_AutoAssign.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(232)))), ((int)(((byte)(226)))));
            this.btn_AutoAssign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_AutoAssign.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_AutoAssign.ForeColor = System.Drawing.Color.White;
            this.btn_AutoAssign.Location = new System.Drawing.Point(488, 12);
            this.btn_AutoAssign.Name = "btn_AutoAssign";
            this.btn_AutoAssign.Size = new System.Drawing.Size(90, 32);
            this.btn_AutoAssign.TabIndex = 5;
            this.btn_AutoAssign.Text = "一键赋码";
            this.btn_AutoAssign.UseVisualStyleBackColor = false;
            this.btn_AutoAssign.Visible = false;
            this.btn_AutoAssign.Click += new System.EventHandler(this.btn_AutoAssign_Click);
            // 
            // lblScanned
            // 
            this.lblScanned.AutoSize = true;
            this.lblScanned.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.lblScanned.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblScanned.Location = new System.Drawing.Point(665, 18);
            this.lblScanned.Name = "lblScanned";
            this.lblScanned.Size = new System.Drawing.Size(67, 22);
            this.lblScanned.TabIndex = 6;
            this.lblScanned.Text = "| 已采 0";
            // 
            // lblPending
            // 
            this.lblPending.AutoSize = true;
            this.lblPending.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.lblPending.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(88)))), ((int)(((byte)(12)))));
            this.lblPending.Location = new System.Drawing.Point(602, 18);
            this.lblPending.Name = "lblPending";
            this.lblPending.Size = new System.Drawing.Size(57, 22);
            this.lblPending.TabIndex = 5;
            this.lblPending.Text = "待采 0";
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblStats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            this.lblStats.Location = new System.Drawing.Point(15, 22);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(383, 17);
            this.lblStats.TabIndex = 0;
            this.lblStats.Text = "💡 扫码自动匹配 | 拆零无需扫码    快捷键: ↑↓切换 | Enter扫码 | F8审核";
            // 
            // lblShortcuts
            // 
            this.lblShortcuts.AutoSize = true;
            this.lblShortcuts.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblShortcuts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.lblShortcuts.Location = new System.Drawing.Point(20, 35);
            this.lblShortcuts.Name = "lblShortcuts";
            this.lblShortcuts.Size = new System.Drawing.Size(0, 17);
            this.lblShortcuts.TabIndex = 1;
            this.lblShortcuts.Visible = false;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(232)))), ((int)(((byte)(226)))));
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(766, 12);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(90, 32);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "重置界面";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(214)))), ((int)(((byte)(188)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(954, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 32);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "关闭窗口";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.btnSubmit.FlatAppearance.BorderSize = 0;
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(1048, 12);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(90, 32);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Text = "审核通过";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.BtnSubmit_Click);
            // 
            // timerToast
            // 
            this.timerToast.Interval = 3000;
            this.timerToast.Tick += new System.EventHandler(this.TimerToast_Tick);
            // 
            // FrmCollectReturnTraceCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1154, 684);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlScan);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCollectReturnTraceCode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "追溯码采集";
            this.Click += new System.EventHandler(this.FrmCollectReturnTraceCode_Click);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).EndInit();
            this.pnlScan.ResumeLayout(false);
            this.pnlScan.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlToast.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlScan;
        private System.Windows.Forms.Label lblScanTip;
        private System.Windows.Forms.TextBox txtScanCode;
        private System.Windows.Forms.Label lblScanIcon;
        private System.Windows.Forms.Panel pnlContent;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.Label lblShortcuts;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Panel pnlToast;
        private System.Windows.Forms.Label lblToast;
        private System.Windows.Forms.Timer timerToast;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.PictureBox pictureBoxClose;
        private System.Windows.Forms.Button btn_AutoAssign;
        private System.Windows.Forms.Label lblPending;
        private System.Windows.Forms.Label lblScanned;
        private System.Windows.Forms.Button btn_AllNoReturn;
        private System.Windows.Forms.Label lblPatientInfo;
    }
}
