namespace Neusoft.HISFC.Components.OutpatientFee.InvoicePrint
{
    partial class ucInvoicePreviewGFAll
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

        #region 组件设计器生成的代码
        /// <summary> 
        /// 设计器支持所需的方法 - 不要使用代码编辑器 
        /// 修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ucPreviewTot = new Neusoft.HISFC.Components.OutpatientFee.InvoicePrint.ucInvoicePreviewGF();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ucPreviewPub = new ucInvoicePreviewGF();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ucPreviewSP = new ucInvoicePreviewGF();
            this.ucPreviewOwn = new ucInvoicePreviewGF();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ucPreviewTot
            // 
            this.ucPreviewTot.Dock = System.Windows.Forms.DockStyle.Top;
            this.ucPreviewTot.InvoiceType = "";
            this.ucPreviewTot.Location = new System.Drawing.Point(0, 0);
            this.ucPreviewTot.Name = "ucPreviewTot";
            this.ucPreviewTot.Size = new System.Drawing.Size(1007, 190);
            this.ucPreviewTot.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ucPreviewPub);
            this.panel1.Controls.Add(this.ucPreviewTot);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1007, 380);
            this.panel1.TabIndex = 1;
            // 
            // ucPreviewPub
            // 
            this.ucPreviewPub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucPreviewPub.InvoiceType = "";
            this.ucPreviewPub.Location = new System.Drawing.Point(0, 190);
            this.ucPreviewPub.Name = "ucPreviewPub";
            this.ucPreviewPub.Size = new System.Drawing.Size(1007, 190);
            this.ucPreviewPub.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ucPreviewSP);
            this.panel2.Controls.Add(this.ucPreviewOwn);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 380);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1007, 384);
            this.panel2.TabIndex = 2;
            // 
            // ucPreviewSP
            // 
            this.ucPreviewSP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucPreviewSP.InvoiceType = "";
            this.ucPreviewSP.Location = new System.Drawing.Point(0, 0);
            this.ucPreviewSP.Name = "ucPreviewSP";
            this.ucPreviewSP.Size = new System.Drawing.Size(1007, 192);
            this.ucPreviewSP.TabIndex = 2;
            // 
            // ucPreviewOwn
            // 
            this.ucPreviewOwn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucPreviewOwn.InvoiceType = "";
            this.ucPreviewOwn.Location = new System.Drawing.Point(0, 192);
            this.ucPreviewOwn.Name = "ucPreviewOwn";
            this.ucPreviewOwn.Size = new System.Drawing.Size(1007, 192);
            this.ucPreviewOwn.TabIndex = 1;
            // 
            // ucInvoicePreviewGFAll
            // 
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ucInvoicePreviewGFAll";
            this.Size = new System.Drawing.Size(1007, 764);
            this.Load += new System.EventHandler(this.ucInvoicePreviewGFAll_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private ucInvoicePreviewGF ucPreviewTot;
        private System.Windows.Forms.Panel panel1;
        private ucInvoicePreviewGF ucPreviewPub;
        private System.Windows.Forms.Panel panel2;
        private ucInvoicePreviewGF ucPreviewOwn;
        private ucInvoicePreviewGF ucPreviewSP;

    }
}