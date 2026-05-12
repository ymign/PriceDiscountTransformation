using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.OutpatientFee
{
    /// <summary>
    /// 
    /// </summary>
    public partial class frmScanInput : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public frmScanInput()
        {
            InitializeComponent();
        }

        public string QRCode=string.Empty;


        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                QRCode = this.txtInput.Text.Trim();
                if (string.IsNullOrEmpty(QRCode))
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
                this.Close();

            }
            else if (e.KeyCode== Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
