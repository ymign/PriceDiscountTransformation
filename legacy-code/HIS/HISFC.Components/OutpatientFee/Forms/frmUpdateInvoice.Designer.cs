namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    partial class frmUpdateInvoice
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
            this.ucUpdateInvoice = new Neusoft.HISFC.Components.OutpatientFee.Controls.ucUpdateUsedInvoiceNo();
            this.SuspendLayout();
            // 
            // ucUpdateInvoice
            // 
            this.ucUpdateInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucUpdateInvoice.Location = new System.Drawing.Point(0, 0);
            this.ucUpdateInvoice.Name = "ucUpdateInvoice";
            this.ucUpdateInvoice.Size = new System.Drawing.Size(490, 199);
            this.ucUpdateInvoice.TabIndex = 0;
            // 
            // frmUpdateInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 199);
            this.Controls.Add(this.ucUpdateInvoice);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUpdateInvoice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "更新发票号";
            this.ResumeLayout(false);

        }

        #endregion

        private Neusoft.HISFC.Components.OutpatientFee.Controls.ucUpdateUsedInvoiceNo ucUpdateInvoice;
    }
}