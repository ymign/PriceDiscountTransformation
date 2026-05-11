using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    public partial class frmInvoiceReturn : Form
    {
        #region 变量

        private Neusoft.HISFC.Models.Fee.Invoice CurrInvoice;
        private long invoiceUsedNo;
        private string invoiceUsedNoStr;
        private long invoiceEndNo;
        private string invoiceEndNoStr;
        private long invoiceQty;

        #endregion

        public frmInvoiceReturn(Neusoft.HISFC.Models.Fee.Invoice invoiceInfo)
        {
            InitializeComponent();
            this.CurrInvoice = invoiceInfo;
            Init();
        }

        #region 属性

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this.txtUsedno.Text = CurrInvoice.UsedNO;

            if (CurrInvoice.ValidState == "0")
            {
                invoiceUsedNo = Neusoft.FrameWork.Public.String.GetNumber(CurrInvoice.BeginNO);
                invoiceUsedNoStr = CurrInvoice.BeginNO;
            }
            else
            {
                invoiceUsedNo = Neusoft.FrameWork.Public.String.GetNumber(CurrInvoice.UsedNO) + 1;
                invoiceUsedNoStr = Neusoft.FrameWork.Public.String.AddNumber(CurrInvoice.UsedNO, 1);
            }
            invoiceEndNo = Neusoft.FrameWork.Public.String.GetNumber(CurrInvoice.EndNO);
            invoiceEndNoStr = CurrInvoice.EndNO;

            invoiceQty =invoiceEndNo-invoiceUsedNo + 1;



            this.txtStartNo.Text = invoiceUsedNoStr;
            this.txtEndNo.Text = CurrInvoice.EndNO;
            this.txtQty.Text = invoiceQty.ToString();

        }

        /// <summary>
        /// 检查数据有效性
        /// </summary>
        /// <returns></returns>
        private bool ValidateValue()
        {
            //2007-5-29新增加的代码 路志鹏
            //for (int i = 0, j = this.txtStartNo.Text.Length; i < j; i++)
            //{
            //    if (!char.IsDigit(this.txtStartNo.Text, i))
            //    {
            //        //可以说明是第几个字符错误了
            //        MessageBox.Show("起始发票号格式不正确，请重新输入！", "提示", MessageBoxButtons.OK);
            //        txtStartNo.Focus();
            //        txtStartNo.SelectAll();
            //        return false;
            //    }
            //}
            

            long startNo =Neusoft.FrameWork.Public.String.GetNumber(this.txtStartNo.Text.Trim());
            long endNo = Neusoft.FrameWork.Public.String.GetNumber(this.txtEndNo.Text.Trim());
            long useNo = Neusoft.FrameWork.Public.String.GetNumber(this.txtUsedno.Text.Trim());

            if (startNo > endNo)
            {
                MessageBox.Show("起始号不能大于终止号！", "提示", MessageBoxButtons.OK);

                return false;
            }
            if (startNo < invoiceUsedNo || startNo > invoiceEndNo || endNo > invoiceEndNo)
            {
                MessageBox.Show("回收的发票超出范围！", "提示", MessageBoxButtons.OK);
                return false;

            }

            CurrInvoice.BeginNO = this.txtStartNo.Text.Trim();
            CurrInvoice.EndNO = this.txtEndNo.Text.Trim();
            CurrInvoice.Qty = (int)(endNo - startNo);

            long Count1 = 0; //要回收的数量
            long Count2 = endNo - useNo + 1; //实际能回收的数量

            // [2007/02/06] 新增加的代码
            for (int i = 0, j = this.txtQty.Text.Length; i < j; i++)
            {
                if (!char.IsDigit(this.txtQty.Text, i))
                {
                    //可以说明是第几个字符错误了
                    MessageBox.Show("回收的数量必须是数字", "提示", MessageBoxButtons.OK);
                    return false;
                }
            }
            //新增加的代码结束

            Count1 = Convert.ToInt32(this.txtQty.Text); //填写的要回收的数量
            if (Count1 > Count2)
            {
                MessageBox.Show("数量过大", "提示", MessageBoxButtons.OK);
                this.txtQty.Focus();
                return false;
            }
            if (endNo - startNo< 0)
            {
                MessageBox.Show("起始发票号过大", "提示", MessageBoxButtons.OK);
                this.txtStartNo.Focus();
                return false;
            }
            this.txtStartNo.Text = Neusoft.FrameWork.Public.String.AddNumber(this.txtEndNo.Text.Trim(), -Count1 + 1);// Convert.ToString(Convert.ToInt64(this.txtEndNo.Text) - Count1 + 1);


            return true;

        }

        #endregion

        #region 共有方法

        #endregion

        #region 事件

        private void txtQty_Leave(object sender, EventArgs e)
        {

            // [2007/05/29] 新增加的代码
            for (int i = 0, j = this.txtQty.Text.Length; i < j; i++)
            {
                if (!char.IsDigit(this.txtQty.Text, i))
                {
                    //可以说明是第几个字符错误了
                    MessageBox.Show("回收的数量必须是数字", "提示", MessageBoxButtons.OK);
                    txtQty.Focus();
                    txtQty.SelectAll();
                    return;
                }
            }

            long Count1 = 0; //要回收的数量
            long Count2 = Convert.ToInt64(this.txtEndNo.Text) - Convert.ToInt64(this.txtUsedno.Text) + 1; //实际能回收的数量
            Count1 = Convert.ToInt32(this.txtQty.Text); //填写的要回收的数量
            if (Count1 > Count2)
            {
                MessageBox.Show("要回收的数量过大");
                return;
            }
            this.txtStartNo.Text = Convert.ToString(Convert.ToInt64(this.txtEndNo.Text) - Count1 + 1).PadLeft(12, '0');
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateValue())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion
                                
    }
}