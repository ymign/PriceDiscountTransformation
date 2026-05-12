using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Neusoft.HISFC.Models.Fee;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    /// <summary>
    /// 更新发票号
    /// {C075EA48-EFC2-4de0-A0F2-BFD3F119A85F}
    /// </summary>
    public partial class ucUpdateUsedInvoiceNo : UserControl
    {
        public ucUpdateUsedInvoiceNo()
        {
            InitializeComponent();

            this.cmbUserNumbers.SelectedValueChanged += new EventHandler(cmbUserNumbers_SelectedValueChanged);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnClear.Click += new EventHandler(btnClear_Click);
        }

        #region 业务层
        /// <summary>
        /// 费用业务层
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        Neusoft.HISFC.BizLogic.Fee.InvoiceServiceNoEnum manager = new Neusoft.HISFC.BizLogic.Fee.InvoiceServiceNoEnum();
        #endregion

        private string invoiceType = "C";
        /// <summary>
        /// 发票类型
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

        string nextInvoiceNO = "";
        /// <summary>
        /// 下一发票号
        /// </summary>
        public string NextInvoiceNO
        {
            get { return nextInvoiceNO; }
        }

        #region  方法

        /// <summary>
        /// 验证起始号、终止号、数量的有效性。
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateNumValid()
        {
            long startcode = 0;
            long endcode = 0;
            long num = 0;
            if (this.txtBeginNO.Text.Trim() == "")
            {
                MessageBox.Show("请输入发票起始号！", "提示", MessageBoxButtons.OK);
                return false;
            }

            if (this.txtEndNO.Text.Trim() == "")
            {
                MessageBox.Show("请输入发票终止号！", "提示", MessageBoxButtons.OK);
                return false;
            }

            #region 检察

            try
            {
                startcode = Neusoft.FrameWork.Public.String.GetNumber(this.txtBeginNO.Text.Trim());
            }
            catch (FormatException formatException)
            {
                MessageBox.Show("起始号必须是大于0的数字!" + formatException.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch (OverflowException overflowException)
            {
                MessageBox.Show("起始号最大为12位!" + overflowException.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("请重新输入起始号!" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            try
            {
                endcode = Neusoft.FrameWork.Public.String.GetNumber(this.txtEndNO.Text.Trim());
            }
            catch (FormatException formatException)
            {
                MessageBox.Show("终止号必须是大于0的数字!" + formatException.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch (OverflowException overflowException)
            {
                MessageBox.Show("终止号最大为12位!" + overflowException.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("请重新输入终止号!" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

         
            #endregion

            if (endcode < startcode)
            {
                MessageBox.Show("终止号应大于或等于起始号！", "提示", MessageBoxButtons.OK);

                return false;
            }

            if (this.txtEndNO.Text.Trim().Length > 12)
            {
                MessageBox.Show("发票数量过大，请重新输入！", "提示");
                return false;
            }

            if (manager.InvoicesIsValid(startcode, endcode, invoiceType) == false)
            {
                MessageBox.Show("输入的发票号有误，可能已被领取，请重新输入！", "提示", MessageBoxButtons.OK);
                return false;
            }


            return true;
        }

        /// <summary>
        /// 查找所有发票信息
        /// </summary>
        /// <param name="operCode"></param>
        /// <param name="invoiceType"></param>
        public void QueryInvoices(string operCode, string invoiceType)
        {
            ArrayList invoiceList = manager.QueryInvoices(operCode, invoiceType);
            if (invoiceList == null)
            {
                //提示错误
                MessageBox.Show(this, "查询错误，原因：" + manager.Err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ArrayList al = new ArrayList();
            string userNO = string.Empty;
            foreach (Invoice invoice in invoiceList)
            {
                if (invoice.ValidState != "-1")
                {
                    invoice.ID = invoice.AcceptTime.ToString("yyyyMMddHHmmss") + invoice.AcceptOper.ID;
                    invoice.Name = invoice.BeginNO + "~" + invoice.EndNO+ (invoice.ValidState == "1" ? "（正在使用）" : "（未用）" );
                    if (invoice.ValidState == "1")
                    {
                        userNO = invoice.ID;
                    }
                    al.Add(invoice);
                }
            }
            this.cmbUserNumbers.AddItems(al);
            this.cmbUserNumbers.Tag = userNO;//选中当前使用号
        }

        /// <summary>
        /// 保存发票领用信息
        /// </summary>
        /// <returns></returns>
        protected virtual void SaveInvoiceNoGet()
        {
            Invoice invoice = new Invoice();
            //类型
            invoice.Type.ID = this.cmbInvoiceType.Tag.ToString();
            invoice.Type.Name = this.cmbInvoiceType.Text;

            invoice.BeginNO = this.txtBeginNO.Text.Trim().PadLeft(12, '0');
            invoice.EndNO = this.txtEndNO.Text.Trim().PadLeft(12, '0');
            invoice.UsedNO = Neusoft.FrameWork.Public.String.AddNumber(invoice.BeginNO, -1);

            //领用人
            invoice.AcceptOper.ID = Neusoft.FrameWork.Management.Connection.Operator.ID;
            invoice.AcceptOper.Name = Neusoft.FrameWork.Management.Connection.Operator.Name;
            invoice.AcceptTime = manager.GetDateTimeFromSysDateTime();
            //参数
            invoice.IsPublic = false;
            invoice.ValidState = "1";

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            this.manager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            if (this.manager.InsertInvoice(invoice) == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("发票设置失败！" + this.manager.Err);
                return;
            }

            //将其他的更新为不使用
            foreach (Invoice invoiceTemp in this.cmbUserNumbers.alItems)
            {
                invoiceTemp.ValidState = "0";
                if (manager.UpdateInvoiceDefaltState(invoiceTemp) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新失败：" + manager.Err);
                    return;
                }
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();
            MessageBox.Show("发票设置成功！", "提示", MessageBoxButtons.OK);
        }
        /// <summary>
        /// 保存发票领用信息
        /// </summary>
        /// <returns></returns>
        private void SaveInvoiceGet()
        {
            //更新一下发票号
            nextInvoiceNO = "";
            string invoiceNO = tbRealInvoiceNO.Text.Trim();
            if (string.IsNullOrEmpty(invoiceNO))
            {
                MessageBox.Show("请录入有效的下一发票号！");
                return;
            }

            invoiceNO = invoiceNO.PadLeft(12, '0');
            tbRealInvoiceNO.Text = invoiceNO;

            if (this.cmbUserNumbers.SelectedItem is Invoice)
            {
                //说明已经领用了，可以跳号
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

                //将其他的更新为不使用
                foreach (Invoice invoice in this.cmbUserNumbers.alItems)
                {
                    if (invoice.ID == this.cmbUserNumbers.SelectedItem.ID)
                    {
                        invoice.ValidState = "1";
                    }
                    else
                    {
                        invoice.ValidState = "0";
                    }
                    if (manager.UpdateInvoiceDefaltState(invoice) == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("更新失败：" + manager.Err);
                        return;
                    }
                }
                Neusoft.FrameWork.Management.PublicTrans.Commit();

                Neusoft.HISFC.Models.Base.Employee oper = manager.Operator as Neusoft.HISFC.Models.Base.Employee;

                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                int iRes = feeIntegrate.UpdateNextInvoiceNO(oper.ID, this.invoiceType, invoiceNO);
                if (iRes <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(feeIntegrate.Err);
                }

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                nextInvoiceNO = tbRealInvoiceNO.Text.Trim();
                MessageBox.Show("更新成功！");
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            this.cmbUserNumbers.Tag = null;
            this.txtBeginNO.Text = string.Empty;
            this.txtEndNO.Text = string.Empty;
            this.txtUserNO.Text = string.Empty;
            this.tbRealInvoiceNO.Text = string.Empty;
            this.txtBeginNO.Enabled = true;
            this.txtEndNO.Enabled = true;
            this.txtUserNO.Enabled = false;
            this.tbRealInvoiceNO.Enabled = false;

            this.txtBeginNO.Focus();
            this.txtBeginNO.SelectAll();
        }

        #endregion

        #region 事件
        void btnCancel_Click(object sender, EventArgs e)
        {

            if (this.ParentForm != null)
            {
                this.ParentForm.Close();
            }
        }

        private void ctlUpdateUsedInvoiceNo_Load(object sender, EventArgs e)
        {
            this.Clear();

            Neusoft.HISFC.Models.Base.Employee oper = manager.Operator as Neusoft.HISFC.Models.Base.Employee;
            //加载发票类型
            this.cmbInvoiceType.AddItems((new Neusoft.HISFC.BizLogic.Manager.Constant().GetList("GetInvoiceType")));
            this.cmbInvoiceType.Tag = this.invoiceType;
            //查找发票信息
            this.QueryInvoices(oper.ID, this.invoiceType);
        }

        void cmbUserNumbers_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.cmbUserNumbers.SelectedItem is Invoice)
            {
                Invoice invoice = this.cmbUserNumbers.SelectedItem as Invoice;

                this.cmbInvoiceType.Tag = invoice.Type.ID;
                this.txtBeginNO.Text = invoice.BeginNO;
                this.txtEndNO.Text = invoice.EndNO;
                this.txtUserNO.Text = invoice.UsedNO;
                this.txtUserNO.Enabled = false;
                this.txtBeginNO.Enabled = false;
                this.txtEndNO.Enabled = false;
                this.tbRealInvoiceNO.Enabled = true;

                this.tbRealInvoiceNO.Text = Neusoft.FrameWork.Public.String.AddNumber(invoice.UsedNO, 1);
            }
            else
            {
                this.Clear();
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //如果选择了当前发票号段，则把当前的更新为使用状态，然后更新下一发票号
            if (this.cmbUserNumbers.Tag == null || string.IsNullOrEmpty(this.cmbUserNumbers.Tag.ToString()))
            {
                if (this.ValidateNumValid() == false)
                {
                    return;
                }
                //相当于领用发票
                this.SaveInvoiceNoGet();
            }
            else
            {
                this.SaveInvoiceGet();
            }

            if (this.ParentForm != null)
            {
                this.ParentForm.Close();
            }

        }

        void btnClear_Click(object sender, EventArgs e)
        {
            this.Clear();
        }
        #endregion
    }

}
