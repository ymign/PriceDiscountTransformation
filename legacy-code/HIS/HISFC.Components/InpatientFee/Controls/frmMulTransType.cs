using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.InpatientFee.Controls
{
    public partial class frmMulTransType : Neusoft.FrameWork.WinForms.Forms.BaseForm
    {
        public frmMulTransType()
        {
            InitializeComponent();
        }

        private ArrayList alPrePays = null;

        public ArrayList PrePays
        {
            set
            {
                this.alPrePays = value;
                this.SetPrePays();
            }
            get
            {
                return this.alPrePays;
            }
        }

        private void SetPrePays()
        {
            if (alPrePays.Count > 0 && alPrePays[0] is Neusoft.HISFC.Models.Fee.Inpatient.Prepay)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.Prepay tmpPrepay = alPrePays[0] as Neusoft.HISFC.Models.Fee.Inpatient.Prepay;
                this.txtPayType1.Text = tmpPrepay.PayType.Name;
                this.txtPreCost1.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(tmpPrepay.FT.PrepayCost, 2);
                this.cmbTransType1.Tag = tmpPrepay.PayType.ID;
                this.cmbTransType1.Text = tmpPrepay.PayType.Name;
            }

            if (alPrePays.Count > 1 && alPrePays[1] is Neusoft.HISFC.Models.Fee.Inpatient.Prepay)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.Prepay tmpPrepay = alPrePays[1] as Neusoft.HISFC.Models.Fee.Inpatient.Prepay;
                this.txtPayType2.Text = tmpPrepay.PayType.Name;
                this.txtPayType2.Visible = true;
                this.txtPreCost2.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(tmpPrepay.FT.PrepayCost, 2);
                this.txtPreCost2.Visible = true;
                this.cmbTransType2.Tag = tmpPrepay.PayType.ID;
                this.cmbTransType2.Text = tmpPrepay.PayType.Name;
                this.cmbTransType2.Visible = true;
            }

            if (alPrePays.Count > 2 && alPrePays[2] is Neusoft.HISFC.Models.Fee.Inpatient.Prepay)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.Prepay tmpPrepay = alPrePays[2] as Neusoft.HISFC.Models.Fee.Inpatient.Prepay;
                this.txtPayType3.Text = tmpPrepay.PayType.Name;
                this.txtPayType3.Visible = true;
                this.txtPreCost3.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(tmpPrepay.FT.PrepayCost, 2);
                this.txtPreCost3.Visible = true;
                this.cmbTransType3.Tag = tmpPrepay.PayType.ID;
                this.cmbTransType3.Text = tmpPrepay.PayType.Name;
                this.cmbTransType3.Visible = true;
            }
        }

        public string GetRetPrePaysInfo()
        {
            string retStr = string.Empty;
            foreach(Neusoft.HISFC.Models.Fee.Inpatient.Prepay tmpPrepay in this.alPrePays)
            {
                retStr += tmpPrepay.PayType.Name + "：" + Neusoft.FrameWork.Public.String.FormatNumberReturnString(tmpPrepay.FT.PrepayCost, 2) + "\n";
            }
            return retStr;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (alPrePays.Count > 0)
            {
                if (string.IsNullOrEmpty(this.cmbTransType1.Tag as string) == true)
                {
                    MessageBox.Show("请选择支付方式！");
                    this.cmbTransType1.Focus();
                    return;
                }
                (alPrePays[0] as Neusoft.HISFC.Models.Fee.Inpatient.Prepay).PayType.ID = this.cmbTransType1.Tag.ToString();
                (alPrePays[0] as Neusoft.HISFC.Models.Fee.Inpatient.Prepay).PayType.Name = this.cmbTransType1.Text;
            }

            if (alPrePays.Count > 1)
            {
                if (string.IsNullOrEmpty(this.cmbTransType2.Tag as string) == true)
                {
                    MessageBox.Show("请选择支付方式！");
                    this.cmbTransType2.Focus();
                    return;
                }
                (alPrePays[1] as Neusoft.HISFC.Models.Fee.Inpatient.Prepay).PayType.ID = this.cmbTransType2.Tag.ToString();
                (alPrePays[1] as Neusoft.HISFC.Models.Fee.Inpatient.Prepay).PayType.Name = this.cmbTransType2.Text;
            }

            if (alPrePays.Count > 2)
            {
                if (string.IsNullOrEmpty(this.cmbTransType3.Tag as string) == true)
                {
                    MessageBox.Show("请选择支付方式！");
                    this.cmbTransType3.Focus();
                    return;
                }
                (alPrePays[2] as Neusoft.HISFC.Models.Fee.Inpatient.Prepay).PayType.ID = this.cmbTransType3.Tag.ToString();
                (alPrePays[2] as Neusoft.HISFC.Models.Fee.Inpatient.Prepay).PayType.Name = this.cmbTransType3.Text;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cmbTransType1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.cmbTransType2.Visible == true)
                {
                    this.cmbTransType2.Focus();
                }
                else
                {
                    this.btnOK.Focus();
                }
            }
        }

        private void cmbTransType2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.cmbTransType3.Visible == true)
                {
                    this.cmbTransType3.Focus();
                }
                else
                {
                    this.btnOK.Focus();
                }
            }
        }

        private void cmbTransType3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnOK.Focus();
            }
        }

    }
}
