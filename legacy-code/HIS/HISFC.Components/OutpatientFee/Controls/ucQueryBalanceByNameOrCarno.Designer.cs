namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    partial class ucQueryBalanceByNameOrCarno
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
            FarPoint.Win.Spread.TipAppearance tipAppearance3 = new FarPoint.Win.Spread.TipAppearance();
            this.neuPanel1 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.neuCheckBox1 = new Neusoft.FrameWork.WinForms.Controls.NeuCheckBox();
            this.neuPanel2 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.neubtnCancle = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neubtnOK = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neubtnPrint = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuPanel3 = new Neusoft.FrameWork.WinForms.Controls.NeuPanel();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.neuPanel1.SuspendLayout();
            this.neuPanel2.SuspendLayout();
            this.neuPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // neuPanel1
            // 
            this.neuPanel1.Controls.Add(this.textBox1);
            this.neuPanel1.Controls.Add(this.neuCheckBox1);
            this.neuPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.neuPanel1.Location = new System.Drawing.Point(0, 0);
            this.neuPanel1.Name = "neuPanel1";
            this.neuPanel1.Size = new System.Drawing.Size(813, 30);
            this.neuPanel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(343, 21);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // neuCheckBox1
            // 
            this.neuCheckBox1.AutoSize = true;
            this.neuCheckBox1.Location = new System.Drawing.Point(369, 7);
            this.neuCheckBox1.Name = "neuCheckBox1";
            this.neuCheckBox1.Size = new System.Drawing.Size(72, 16);
            this.neuCheckBox1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuCheckBox1.TabIndex = 1;
            this.neuCheckBox1.Text = "模糊查询";
            this.neuCheckBox1.UseVisualStyleBackColor = true;
            // 
            // neuPanel2
            // 
            this.neuPanel2.Controls.Add(this.neubtnCancle);
            this.neuPanel2.Controls.Add(this.neubtnOK);
            this.neuPanel2.Controls.Add(this.neubtnPrint);
            this.neuPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.neuPanel2.Location = new System.Drawing.Point(0, 310);
            this.neuPanel2.Name = "neuPanel2";
            this.neuPanel2.Size = new System.Drawing.Size(813, 30);
            this.neuPanel2.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel2.TabIndex = 1;
            // 
            // neubtnCancle
            // 
            this.neubtnCancle.Location = new System.Drawing.Point(441, 3);
            this.neubtnCancle.Name = "neubtnCancle";
            this.neubtnCancle.Size = new System.Drawing.Size(75, 23);
            this.neubtnCancle.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neubtnCancle.TabIndex = 0;
            this.neubtnCancle.Text = "取消";
            this.neubtnCancle.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.neubtnCancle.UseVisualStyleBackColor = true;
            this.neubtnCancle.Click += new System.EventHandler(this.neubtnCancle_Click);
            // 
            // neubtnOK
            // 
            this.neubtnOK.Location = new System.Drawing.Point(360, 4);
            this.neubtnOK.Name = "neubtnOK";
            this.neubtnOK.Size = new System.Drawing.Size(75, 23);
            this.neubtnOK.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neubtnOK.TabIndex = 0;
            this.neubtnOK.Text = "确定";
            this.neubtnOK.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.neubtnOK.UseVisualStyleBackColor = true;
            this.neubtnOK.Click += new System.EventHandler(this.neubtnOK_Click);
            // 
            // neubtnPrint
            // 
            this.neubtnPrint.Location = new System.Drawing.Point(276, 4);
            this.neubtnPrint.Name = "neubtnPrint";
            this.neubtnPrint.Size = new System.Drawing.Size(75, 23);
            this.neubtnPrint.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neubtnPrint.TabIndex = 0;
            this.neubtnPrint.Text = "打印副本";
            this.neubtnPrint.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.neubtnPrint.UseVisualStyleBackColor = true;
            this.neubtnPrint.Click += new System.EventHandler(this.neubtnPrint_Click);
            // 
            // neuPanel3
            // 
            this.neuPanel3.Controls.Add(this.fpSpread1);
            this.neuPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neuPanel3.Location = new System.Drawing.Point(0, 30);
            this.neuPanel3.Name = "neuPanel3";
            this.neuPanel3.Size = new System.Drawing.Size(813, 280);
            this.neuPanel3.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuPanel3.TabIndex = 2;
            // 
            // fpSpread1
            // 
            this.fpSpread1.About = "3.0.2004.2005";
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1, Row 0, Column 0, ";
            this.fpSpread1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(247)))), ((int)(((byte)(213)))));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(0, 0);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(813, 280);
            this.fpSpread1.TabIndex = 0;
            tipAppearance3.BackColor = System.Drawing.SystemColors.Info;
            tipAppearance3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tipAppearance3.ForeColor = System.Drawing.SystemColors.InfoText;
            this.fpSpread1.TextTipAppearance = tipAppearance3;
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // ucQueryBalanceByNameOrCarno
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.neuPanel3);
            this.Controls.Add(this.neuPanel2);
            this.Controls.Add(this.neuPanel1);
            this.Name = "ucQueryBalanceByNameOrCarno";
            this.Size = new System.Drawing.Size(813, 340);
            this.neuPanel1.ResumeLayout(false);
            this.neuPanel1.PerformLayout();
            this.neuPanel2.ResumeLayout(false);
            this.neuPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel1;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel2;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton neubtnPrint;
        private Neusoft.FrameWork.WinForms.Controls.NeuPanel neuPanel3;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private Neusoft.FrameWork.WinForms.Controls.NeuCheckBox neuCheckBox1;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton neubtnCancle;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton neubtnOK;
        private System.Windows.Forms.TextBox textBox1;
    }
}
