using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucInvoiceDeal : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucInvoiceDeal()
        {
            InitializeComponent();
        }

        private void neuButton1_Click(object sender, EventArgs e)
        {
            if (this.neuRadioButton1.Checked && this.neuRadioButton2.Checked)
            {
                MessageBox.Show("请选择单个发票类型");
            }
            if (!this.neuRadioButton1.Checked && !this.neuRadioButton2.Checked)
            {
                MessageBox.Show("请选择发票类型");
            }
            string type="";
            if (this.neuRadioButton1.Checked)
            {
                type = this.neuRadioButton1.Tag.ToString();
            }
            if (this.neuRadioButton2.Checked)
            {
                type = this.neuRadioButton2.Tag.ToString();
            }
            string invoiceno = this.neuTextBox1.Text.Trim();
            if (string.IsNullOrEmpty(invoiceno))
            {
                MessageBox.Show("填写发票号!");
                return;
            }
            int result = 0;
            switch (type)
            {
                case "C":
                    result = DealFeeInvoice(invoiceno);
                    break;
                case "R":
                    result = DealRegInvoice(invoiceno);
                    break;
                default:
                    break;
            }
            if (result == 1)
            {
                MessageBox.Show("更新成功!");
            }
            else
            {
                MessageBox.Show("更新失败!");
            }

        }

        /// <summary>
        /// 处理挂号发票
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        private int DealRegInvoice(string no)
        {
            string sql = @"
                            update fin_opb_accountcardfee p
                               set p.zzsb_print_date = null, p.zzsb_oper_code = ''
                             where p.invoice_no = '{0}'
                               and p.trans_type = '1'
                               and p.cancel_flag='1'";

            sql = string.Format(sql,no);

            Neusoft.HISFC.BizLogic.Fee.Account account = new Neusoft.HISFC.BizLogic.Fee.Account();
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            account.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            int result = account.ExecNoQuery(sql);
            if (result <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();
            return 1;
        }

        /// <summary>
        /// 处理收费发票
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        private int DealFeeInvoice(string no)
        {
            string sql = @"
                            update fin_opb_invoiceinfo p
                            set p.account_flag='1',
                                p.zzsb_print_date=null
                            where p.invoice_no='{0}'
                            and p.trans_type='1'
                            and p.cancel_flag='1'";

            sql = string.Format(sql, no);

            Neusoft.HISFC.BizLogic.Fee.Account account = new Neusoft.HISFC.BizLogic.Fee.Account();
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            account.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            int result = account.ExecNoQuery(sql);
            if (result <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();
            return 1;
        }
    }
}
