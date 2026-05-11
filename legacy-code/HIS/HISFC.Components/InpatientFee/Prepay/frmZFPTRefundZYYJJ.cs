using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Neusoft.HISFC.Models.Account;
using HisCallExternalServiceProject.ZDWYPayPlatform.Models;

namespace Neusoft.HISFC.Components.InpatientFee.Prepay
{
    public partial class frmZFPTRefundZYYJJ : Form
    {
        #region 窗体移动
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            //为当前应用程序释放鼠标捕获
            ReleaseCapture();
            //发送消息 让系统误以为在标题栏上按下鼠标
            SendMessage((IntPtr)this.Handle, VM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private const int VM_NCLBUTTONDOWN = 0XA1;//定义鼠标左键按下
        private const int HTCAPTION = 2;
        #endregion

        /// <summary>
        /// 住院费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.InPatient feeInpatient = new Neusoft.HISFC.BizLogic.Fee.InPatient();

        private bool cancel = false;
        /// <summary>
        /// 是否取消 为true时收费人员点击了取消按钮
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
        }
        private ZFPTOrder zfptOrd;
        
        public frmZFPTRefundZYYJJ(ZFPTOrder order)
        {
            InitializeComponent();
            zfptOrd = order;
        }

        private void frmZFPTRefundZYYJJ_Load(object sender, EventArgs e)
        {
            try
            {
                setInterface();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Close();
            return;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.zfptOrd.PAYAMT != this.zfptOrd.RETURNEDATM)
            {
                MessageBox.Show("退款金额不等于支付金额，请处理。");
                return;
            }
            this.Close();
        }

        private void btnRefund_Click(object sender, EventArgs e)
        {
            Neusoft.HISFC.Models.Base.Employee employee = this.feeInpatient.Operator as Neusoft.HISFC.Models.Base.Employee;
            HisCallExternalServiceProject.ZDWYPayPlatform.ZFPTService zfptService = new HisCallExternalServiceProject.ZDWYPayPlatform.ZFPTService();
            DepositRefund rqModel = new DepositRefund();
            #region 退费入参
            rqModel.payorderId = this.zfptOrd.payorderId;
            rqModel.orderType = this.zfptOrd.orderType;
            rqModel.payMode = this.zfptOrd.payMode;
            rqModel.refundAmout = this.zfptOrd.PAYAMT;
            rqModel.operCode = employee.ID;
            rqModel.operName = employee.Name;
            #endregion
            Response<ResRefund> responseModel = null;
            zfptService.CreditPayDepositRefund(rqModel, ref responseModel);
            if (responseModel != null)
            {
                if (responseModel.Code == "1")
                {
                    RefreshOrder();
                    MessageBox.Show("退款成功。可继续在此界面刷新查看订单状态，点击完成即可继续退号操作。");
                }
                else
                {
                    MessageBox.Show("退款失败：" + responseModel.Msg);
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RefreshOrder();
        }

        /// <summary>
        /// 刷新订单信息
        /// </summary>
        private void RefreshOrder()
        {
            this.zfptOrd = feeInpatient.GetZFPTOrder(this.zfptOrd.payorderId);
            setInterface();
        }

        private void setInterface()
        {
            this.txtPayOrderId.Text = this.zfptOrd.payorderId;
            #region 支付方式
            if (this.zfptOrd.payMode == "1")
            {
                this.lblPAYMODE.Text = "医保信用付";
            }
            else
            {
                this.lblPAYMODE.Text = "未知类型";
            }
            #endregion
            #region 订单类型
            if (this.zfptOrd.orderType == "1")
            {
                this.lblOrderType.Text = "挂号";
            }
            else if (this.zfptOrd.orderType == "2")
            {
                this.lblOrderType.Text = "门诊缴费";
            }
            else if (this.zfptOrd.orderType == "3")
            {
                this.lblOrderType.Text = "住院押金";
            }
            #endregion
            this.lblPAYAMT.Text = this.zfptOrd.PAYAMT.ToString();
            this.lblRETURNEDATM.Text = this.zfptOrd.RETURNEDATM.ToString();

            if (this.zfptOrd.PAYAMT == this.zfptOrd.RETURNEDATM)
            {
                this.btnRefund.Enabled = false;
            }
        }
    }
}
