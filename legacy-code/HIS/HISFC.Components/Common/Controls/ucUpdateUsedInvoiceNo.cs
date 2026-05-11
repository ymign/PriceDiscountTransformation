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

namespace Neusoft.HISFC.Components.Common.Controls
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
            string startcode = string.Empty;
            string endcode = string.Empty;

            if (this.txtBeginNO.Text.Trim() == "")
            {
                MessageBox.Show("请输入发票起始号！", "提示", MessageBoxButtons.OK);
                this.txtBeginNO.Focus();
                this.txtBeginNO.SelectAll();
                return false;
            }

            if (this.txtEndNO.Text.Trim() == "")
            {
                MessageBox.Show("请输入发票终止号！", "提示", MessageBoxButtons.OK);
                this.txtEndNO.Focus();
                this.txtEndNO.SelectAll();
                return false;
            }
            startcode = this.txtBeginNO.Text.Trim().PadLeft(10,'0');
            endcode = this.txtEndNO.Text.Trim().PadLeft(10, '0');

            if (endcode.CompareTo(startcode)<0)
            {
                MessageBox.Show("终止号应大于或等于起始号！", "提示", MessageBoxButtons.OK);
                this.txtEndNO.Focus();
                this.txtEndNO.SelectAll();
                return false;
            }

            if (this.txtEndNO.Text.Trim().Length > 10)
            {
                MessageBox.Show("发票数量过大，请重新输入！", "提示");
                this.txtEndNO.Focus();
                this.txtEndNO.SelectAll();
                return false;
            }

            if (manager.InvoicesIsValid(startcode, endcode, invoiceType) == false)
            {
                MessageBox.Show("输入的发票号有误，可能已被领取，请重新输入！", "提示", MessageBoxButtons.OK);
                this.txtEndNO.Focus();
                this.txtEndNO.SelectAll();
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
            //ArrayList invoiceList = manager.QueryInvoices(operCode, invoiceType);
            ArrayList invoiceList = manager.QueryInvoicesWithHosCode(operCode, invoiceType, Neusoft.FrameWork.Management.Connection.Hospital.ID);
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
                    invoice.Name = invoice.BeginNO + "~" + invoice.EndNO + (invoice.ValidState == "1" ? (invoice.EndNO == invoice.UsedNO ? "（已用完）" : "（正在使用）") : "（未用）");
                    if (invoice.ValidState == "1" && invoice.EndNO != invoice.UsedNO)
                    {
                        userNO = invoice.ID;
                    }
                    al.Add(invoice);
                }
            }
            //Invoice invoiceNull = new Invoice();
            //invoiceNull.Type.ID = invoiceType;

            //al.Insert(0, invoiceNull);
            this.cmbUserNumbers.AddItems(al);

            if (string.IsNullOrEmpty(userNO))
            {
                this.Clear();
            }
            else
            {
                this.cmbUserNumbers.Tag = userNO;//选中当前使用号
                this.tbRealInvoiceNO.Focus();
                this.tbRealInvoiceNO.SelectAll();
            }
        }

        /// <summary>
        /// 保存发票领用信息
        /// </summary>
        /// <returns></returns>
        protected virtual int SaveInvoiceNoGet()
        {
            Invoice invoice = new Invoice();
            //类型
            invoice.Type.ID = this.cmbInvoiceType.Tag.ToString();
            invoice.Type.Name = this.cmbInvoiceType.Text;

            invoice.BeginNO = this.txtBeginNO.Text.Trim().PadLeft(10, '0');
            invoice.EndNO = this.txtEndNO.Text.Trim().PadLeft(10, '0');
            invoice.UsedNO = Neusoft.FrameWork.Public.String.AddNumber(invoice.BeginNO, -1);

            //领用人
            invoice.AcceptOper.ID = Neusoft.FrameWork.Management.Connection.Operator.ID;
            invoice.AcceptOper.Name = Neusoft.FrameWork.Management.Connection.Operator.Name;
            invoice.AcceptTime = manager.GetDateTimeFromSysDateTime();
            //参数
            invoice.IsPublic = false;
            invoice.ValidState = "1";
            invoice.HosCode = Neusoft.FrameWork.Management.Connection.Hospital.ID;
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            this.manager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            if (this.manager.InsertInvoiceWithHosCode(invoice) == -1)
            //if (this.manager.InsertInvoice(invoice) == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("发票设置失败！" + this.manager.Err);
                return -1;
            }

            //将其他的更新为不使用
            foreach (Invoice invoiceTemp in this.cmbUserNumbers.alItems)
            {
                invoiceTemp.HosCode = Neusoft.FrameWork.Management.Connection.Hospital.ID;
                //如果是已经用完，则修改为-1
                if (invoiceTemp.ValidState != "-1" && invoiceTemp.EndNO == invoiceTemp.UsedNO)
                {
                    invoiceTemp.ValidState = "-1";
                }
                else
                {
                    invoiceTemp.ValidState = "0";
                }
                //if (manager.UpdateInvoiceDefaltState(invoiceTemp) == -1)
                if (manager.UpdateInvoiceDefaltStateWithHosCode(invoiceTemp) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新失败：" + manager.Err);
                    return -1;
                }
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();
            //MessageBox.Show("发票设置成功！", "提示", MessageBoxButtons.OK);
            return 1;
        }
        /// <summary>
        /// 保存发票领用信息
        /// </summary>
        /// <returns></returns>
        private int  SaveInvoiceGet()
        {
            //更新一下发票号
            nextInvoiceNO = "";
            string invoiceNO = tbRealInvoiceNO.Text.Trim();
            if (string.IsNullOrEmpty(invoiceNO))
            {
                MessageBox.Show("请录入有效的下一发票号！");
                return -1;
            }

            invoiceNO = invoiceNO.PadLeft(10, '0');
            tbRealInvoiceNO.Text = invoiceNO;

            if (this.cmbUserNumbers.SelectedItem is Invoice)
            {
                //判断此发票是否已用完
                if (((Invoice)this.cmbUserNumbers.SelectedItem).EndNO == ((Invoice)this.cmbUserNumbers.SelectedItem).UsedNO)
                {
                    MessageBox.Show("此发票段已用完，请重新录入！");
                    this.Clear();
                    return -1;
                }
                //说明已经领用了，可以跳号
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

                //将其他的更新为不使用
                foreach (Invoice invoice in this.cmbUserNumbers.alItems)
                {
                        //如果是已经用完，则修改为-1
                    if (invoice.ValidState != "-1" && invoice.EndNO == invoice.UsedNO)
                    {
                        invoice.ValidState = "-1";
                    }
                    else
                    {
                        if (invoice.ID == this.cmbUserNumbers.SelectedItem.ID)
                        {
                            invoice.ValidState = "1";
                        }
                        else
                        {
                            invoice.ValidState = "0";
                        }
                    }

                    //if (manager.UpdateInvoiceDefaltState(invoice) == -1)
                    if (manager.UpdateInvoiceDefaltStateWithHosCode(invoice) == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("更新失败：" + manager.Err);
                        return -1;
                    }
                }
                Neusoft.FrameWork.Management.PublicTrans.Commit();

                Neusoft.HISFC.Models.Base.Employee oper = manager.Operator as Neusoft.HISFC.Models.Base.Employee;

                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                //int iRes = feeIntegrate.UpdateNextInvoiceNO(oper.ID, this.invoiceType, invoiceNO);
                int iRes = feeIntegrate.UpdateNextInvoiceNOWithHosCode(oper.ID, this.invoiceType, invoiceNO, Neusoft.FrameWork.Management.Connection.Hospital.ID);
                if (iRes <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(feeIntegrate.Err);
                    return -1;
                }

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                nextInvoiceNO = tbRealInvoiceNO.Text.Trim();
                //MessageBox.Show("更新成功！");
            }
            return 1;
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
            int i = -1;
            //如果选择了当前发票号段，则把当前的更新为使用状态，然后更新下一发票号
            if (this.cmbUserNumbers.Tag == null || string.IsNullOrEmpty(this.cmbUserNumbers.Tag.ToString()))
            {
                if (this.ValidateNumValid())
                {
                    //相当于领用发票
                    i = this.SaveInvoiceNoGet();
                }
            }
            else
            {
                i=this.SaveInvoiceGet();
            }

            if (i > 0)
            {
                if (this.ParentForm != null)
                {
                    this.ParentForm.Close();
                }
            }

        }

        void btnClear_Click(object sender, EventArgs e)
        {
            this.Clear();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                SendKeys.Send("{tab}");
            }
            return base.ProcessDialogKey(keyData);
        }
        #endregion
    }

}
