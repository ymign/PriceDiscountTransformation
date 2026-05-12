using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    /// <summary>
    /// 更新发票号
    /// {C075EA48-EFC2-4de0-A0F2-BFD3F119A85F}
    /// </summary>
    public partial class frmUpdateInvoice : Form
    {
        public frmUpdateInvoice()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 下一发票号
        /// </summary>
        public string NextInvoiceNO
        {
            get { return this.ucUpdateInvoice.NextInvoiceNO; }
        }

        public string InvoiceType
        {
            set
            {
                this.ucUpdateInvoice.InvoiceType = value;
            }
        }
    }
}
