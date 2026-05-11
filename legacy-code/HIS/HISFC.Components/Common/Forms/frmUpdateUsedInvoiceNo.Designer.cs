namespace Neusoft.HISFC.Components.Common.Forms
{
    partial class frmUpdateUsedInvoiceNo
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
            this.tbRealInvoiceNO = new Neusoft.FrameWork.WinForms.Controls.NeuTextBox();
            this.btnUpdate = new Neusoft.FrameWork.WinForms.Controls.NeuButton();
            this.neuLabel1 = new Neusoft.FrameWork.WinForms.Controls.NeuLabel();
            this.SuspendLayout();
            // 
            // tbRealInvoiceNO
            // 
            this.tbRealInvoiceNO.BackColor = System.Drawing.Color.White;
            this.tbRealInvoiceNO.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRealInvoiceNO.IsEnter2Tab = false;
            this.tbRealInvoiceNO.Location = new System.Drawing.Point(102, 30);
            this.tbRealInvoiceNO.MaxLength = 12;
            this.tbRealInvoiceNO.Name = "tbRealInvoiceNO";
            this.tbRealInvoiceNO.Size = new System.Drawing.Size(123, 22);
            this.tbRealInvoiceNO.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.tbRealInvoiceNO.TabIndex = 8;
            this.tbRealInvoiceNO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbRealInvoiceNO_KeyDown);
            // 
            // btnUpdate
            // 
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(241, 31);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.btnUpdate.TabIndex = 7;
            this.btnUpdate.Text = "更新(&U)";
            this.btnUpdate.Type = Neusoft.FrameWork.WinForms.Controls.General.ButtonType.None;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // neuLabel1
            // 
            this.neuLabel1.AutoSize = true;
            this.neuLabel1.Font = new System.Drawing.Font("宋体", 10F);
            this.neuLabel1.Location = new System.Drawing.Point(12, 35);
            this.neuLabel1.Name = "neuLabel1";
            this.neuLabel1.Size = new System.Drawing.Size(84, 14);
            this.neuLabel1.Style = Neusoft.FrameWork.WinForms.Controls.StyleType.Fixed3D;
            this.neuLabel1.TabIndex = 6;
            this.neuLabel1.Text = "下一发票号:";
            // 
            // frmUpdateUsedInvoiceNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 81);
            this.Controls.Add(this.tbRealInvoiceNO);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.neuLabel1);
            this.Name = "frmUpdateUsedInvoiceNo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "更新发票号";
            this.Load += new System.EventHandler(this.ctlUpdateUsedInvoiceNo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Neusoft.FrameWork.WinForms.Controls.NeuTextBox tbRealInvoiceNO;
        private Neusoft.FrameWork.WinForms.Controls.NeuButton btnUpdate;
        private Neusoft.FrameWork.WinForms.Controls.NeuLabel neuLabel1;
    }
}
