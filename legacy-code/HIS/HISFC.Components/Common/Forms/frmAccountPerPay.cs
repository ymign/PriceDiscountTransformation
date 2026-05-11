using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.BizProcess.Interface.Account;

namespace Neusoft.HISFC.Components.Common.Forms
{
    /// <summary>
    /// 预交金补充值
    /// </summary>
    public partial class frmAccountPerPay : Neusoft.FrameWork.WinForms.Forms.BaseForm
    {
        /// <summary>
        /// 帐户余额
        /// </summary>
        protected decimal decVacancy = 0;
        /// <summary>
        /// 本次扣费
        /// </summary>
        protected decimal decCharge = 0;
        /// <summary>
        /// 患者信息
        /// </summary>
        protected Neusoft.HISFC.Models.RADT.Patient accountPatient = null;
        

        public frmAccountPerPay()
        {
            InitializeComponent();
        }

        public frmAccountPerPay(Neusoft.HISFC.Models.RADT.Patient patient, decimal vacancy, decimal charge)
            : this()
        {
            accountPatient = patient;
            decVacancy = vacancy;
            decCharge = charge;
        }

        private void frmAccountPerPay_Load(object sender, EventArgs e)
        {
            if (accountPatient == null)
            {
                MessageBox.Show("补充值患者信息为空!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

            lblVacancy.Text = decVacancy.ToString();
            lblCharge.Text = decCharge.ToString();

            decimal decTemp = decCharge - decVacancy;
            lblRecharge.Text = decTemp > 0 ? decTemp.ToString() : "";
        }


        #region 业务层变量

        /// <summary>
        /// 账户业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.Account accountManager = new Neusoft.HISFC.BizLogic.Fee.Account();
        /// <summary>
        /// 费用综合业务层 
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        #endregion


        private void Save()
        {
            //为了避免预交金重复收费的问题
            string accountMoney = this.txtpay.Text;
            this.txtpay.Text = "0.00";

            if (this.cmbPayType.Tag == null || this.cmbPayType.Tag.ToString() == string.Empty)
            {
                MessageBox.Show("请选择支付方式！", "系统提示");
                this.cmbPayType.Focus();
                return;
            }
            decimal money = Neusoft.FrameWork.Function.NConvert.ToDecimal(accountMoney);
            if (money == 0)
            {
                MessageBox.Show("请输入交费金额！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtpay.Focus();
                txtpay.SelectAll();
                return;
            }

            if (MessageBox.Show("是否确认充值！", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                return;
            }

            this.cmdOK.Enabled = false;

            Neusoft.HISFC.Models.Account.Account account = accountManager.GetAccountByCardNo(accountPatient.PID.CardNO);
            if (account == null)
            {
                MessageBox.Show("该卡未建立账户或账户已注销，请建立账户！", "系统提示");
                return;
            }

            #region 获取发票号
            string invoiceNO = this.feeIntegrate.GetNewInvoiceNO("A");
            if (invoiceNO == null || invoiceNO == string.Empty)
            {
                MessageBox.Show("获得发票号出错!" + this.feeIntegrate.Err);
                return;
            }
            #endregion

            #region 预交金实体

            HISFC.Models.Account.PrePay prePay = new Neusoft.HISFC.Models.Account.PrePay();
            prePay.Patient = accountPatient;//患者基本信息
            prePay.PayType.ID = this.cmbPayType.Tag.ToString();//支付方式
            prePay.PayType.Name = this.cmbPayType.Text;
            prePay.Bank = this.cmbPayType.bank.Clone();//开户银行
            prePay.FT.PrepayCost = FrameWork.Function.NConvert.ToDecimal(accountMoney);//预交金
            prePay.InvoiceNO = invoiceNO; //发票号
            prePay.ValidState = Neusoft.HISFC.Models.Base.EnumValidState.Valid;//预交金状态
            prePay.PrePayOper.ID = accountManager.Operator.ID;//操作员编号
            prePay.PrePayOper.Name = accountManager.Operator.Name;//操作员姓名
            prePay.PrePayOper.OperTime = accountManager.GetDateTimeFromSysDateTime();//系统时间
            prePay.AccountNO = account.ID; //帐号
            prePay.IsHostory = false; //是否历史数据           

            #endregion

            #region 更新数据
            //设置事物

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            accountManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            if (!accountManager.AccountPrePayManager(prePay, 1))
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(accountManager.Err, "错误");
                return;
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();
            MessageBox.Show("交费 （" + accountMoney + "） 成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            #endregion

            #region 打印
            IPrintPrePayRecipe Iprint = Neusoft.FrameWork.WinForms.Classes.
               UtilInterface.CreateObject(this.GetType(), typeof(IPrintPrePayRecipe)) as IPrintPrePayRecipe;
            if (Iprint != null)
            {
                Iprint.SetValue(prePay);
                Iprint.Print();
            }
            else
            {
                MessageBox.Show("请维护打印票据，查找打印票据失败！");
            }

            #endregion


            this.cmdOK.Enabled = true;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmbPayType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                this.txtpay.Focus();
            }
        }

        private void txtpay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                this.cmdOK.Focus();
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (cmdOK.Focused)
                {
                    this.Save();
                }
                else
                {
                    SendKeys.Send("{Tab}");
                }

                Application.DoEvents();

                return true;
            }
            return base.ProcessDialogKey(keyData);
        }


      
    
    }
}
