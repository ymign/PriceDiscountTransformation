using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Forms
{
    public partial class frmMedicalType : Form
    {
        private DialogResult res = DialogResult.Cancel;
        public string medicalType { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public DialogResult Res
        {
            get { return res; }
            //set { res = value; }
        }
        public frmMedicalType()
        {
            InitializeComponent();
        }

        private void neuButton1_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked) {
                medicalType = "01"; 
            }
            if (this.radioButton2.Checked)
            {
                medicalType = "02";
            }
            if (this.radioButton3.Checked)
            {
                medicalType = "03";
            }

            res = DialogResult.OK;
            this.Close();
        }
    }
}
