using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.InpatientFee.Controls
{
    public partial class ucTransType : Neusoft.FrameWork.WinForms.Forms.BaseForm
    {
        protected Neusoft.HISFC.Models.Fee.Inpatient.Prepay prePay = null;
        
        
        public ucTransType()
        {
            InitializeComponent();
        }


        public ucTransType(Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay):this()
        {
            prePay = prepay;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            return;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.cmbTransType1.Tag == null || this.cmbTransType1.Tag.ToString() == string.Empty)
            {
                MessageBox.Show("请选择支付方式！", "系统提示");
                this.cmbTransType1.Focus();
                return;
            }


            prePay.PayType.ID = this.cmbTransType1.Tag.ToString();
            prePay.PayType.Name = this.cmbTransType1.Name.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ucTransType_Load(object sender, EventArgs e)
        {
            this.cmbTransType1.Tag = "CA";
            this.cmbTransType1.Text = "现金";
        }

    }

}