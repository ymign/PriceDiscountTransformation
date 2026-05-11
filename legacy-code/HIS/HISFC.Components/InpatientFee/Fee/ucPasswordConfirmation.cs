using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    public partial class ucPasswordConfirmation : Neusoft.FrameWork.WinForms.Forms.BaseForm
    {
        public ucPasswordConfirmation()
        {
            InitializeComponent();
        }
        private string uid = string.Empty;

        private string pwd = string.Empty;
        public string Uid
        {
            get { return uid; }
            set { uid = value; }
        }

        public string Pwd
        {
            get { return pwd; }
            set { pwd = value; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.pwd = this.neuTextBox1.Text;
            this.Close();
        }
    }
}
