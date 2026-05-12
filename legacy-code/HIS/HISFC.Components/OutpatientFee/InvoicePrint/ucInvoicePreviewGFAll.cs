using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.OutpatientFee.InvoicePrint
{
    public partial class ucInvoicePreviewGFAll : UserControl
    {
        public ucInvoicePreviewGFAll()
        {
            InitializeComponent();
        }

        #region 变量

        ArrayList invoiceAndInvoiceDetails = new ArrayList();

        #endregion

        #region 属性

        public ArrayList InvoiceAndInvoiceDetails
        {
            set
            {
                invoiceAndInvoiceDetails = value;
            }
        }

        #endregion

        #region 函数

        public void Init()
        {
            this.ucPreviewOwn.InvoiceType = "1";
            this.ucPreviewOwn.InvoiceAndDetails = invoiceAndInvoiceDetails;

            this.ucPreviewTot.InvoiceType = "5";
            this.ucPreviewTot.InvoiceAndDetails = invoiceAndInvoiceDetails;

            this.ucPreviewPub.InvoiceType = "2";
            this.ucPreviewPub.InvoiceAndDetails = invoiceAndInvoiceDetails;

            this.ucPreviewSP.InvoiceType = "3";
            this.ucPreviewSP.InvoiceAndDetails = invoiceAndInvoiceDetails;

        }

        #endregion

        #region 事件
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {

                this.FindForm().Close();
            }
            return true;
        }
        #endregion

        private void ucInvoicePreviewGFAll_Load(object sender, System.EventArgs e)
        {
            this.FindForm().Focus();
        }
    }
}
