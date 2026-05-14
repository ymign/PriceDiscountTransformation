using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.FrameWork.WinForms.Forms;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientGuide
{
    public partial class frmPopPubCostBill : frmBaseForm
    {
        public frmPopPubCostBill()
        {
            InitializeComponent();
        }

        public frmPopPubCostBill(Control c)
            : base(c)
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            base.OnLoad(e);
        }
    }
}
