using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Forms
{
    /// <summary>
    /// 更新发票号
    /// {C075EA48-EFC2-4de0-A0F2-BFD3F119A85F}
    /// </summary>
    public partial class frmUpdateUsedInvoiceNo : Form
    {
        public frmUpdateUsedInvoiceNo()
        {
            InitializeComponent();
        }

        private void ctlUpdateUsedInvoiceNo_Load(object sender, EventArgs e)
        {
            tbRealInvoiceNO.Text = "";
            string invoiceNO = "";
            try
            {
                Neusoft.HISFC.Models.Base.Employee oper = outpatientManage.Operator as Neusoft.HISFC.Models.Base.Employee;
                invoiceNO = feeIntegrate.GetNextInvoiceNO(invoiceType, oper);
                if (string.IsNullOrEmpty(invoiceNO))
                {
                    MessageBox.Show(feeIntegrate.Err);
                    return;
                }
            }
            catch (Exception objEx)
            {
            }

            nextInvoiceNO = "";
            tbRealInvoiceNO.Text = invoiceNO;
            tbRealInvoiceNO.SelectAll();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            nextInvoiceNO = "";
            string invoiceNO = tbRealInvoiceNO.Text.Trim();
            if (string.IsNullOrEmpty(invoiceNO))
            {
                MessageBox.Show("请录入有效发票号！");
                return;
            }

            //invoiceNO = invoiceNO.PadLeft(12, '0');

            tbRealInvoiceNO.Text = invoiceNO;

            Neusoft.HISFC.Models.Base.Employee oper = outpatientManage.Operator as Neusoft.HISFC.Models.Base.Employee;

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            int iRes = feeIntegrate.UpdateNextInvoiceNO(oper.ID, invoiceType, invoiceNO);
            if (iRes <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(feeIntegrate.Err);
            }
            else
            {
                Neusoft.FrameWork.Management.PublicTrans.Commit();

                nextInvoiceNO = tbRealInvoiceNO.Text.Trim();

                MessageBox.Show("更新成功！");
            }

        }

        private void tbRealInvoiceNO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnUpdate_Click(null, null);
            }
        }

        #region 业务层
        /// <summary>
        /// 费用业务层
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        /// <summary>
        /// 门诊费用业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManage = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        #endregion


        #region 属性
        /// <summary>
        /// 下一发票号
        /// </summary>
        public string NextInvoiceNO
        {
            get { return nextInvoiceNO; }
        }

        string nextInvoiceNO = "";

        string invoiceType = "C";
        /// <summary>
        /// 发票类型R:挂号收据 C:门诊收据 P:预交收据 I:住院收据 A:门诊账户
        /// </summary>
        public string InvoiceType
        {
            get
            {
                return invoiceType;
            }
            set
            {
                invoiceType = value;
            }
        }
        #endregion 
    }
}
