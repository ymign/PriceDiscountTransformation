using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HisCallExternalServiceProject.ZDWYPayPlatform.Models.YDZF;
using System.Runtime.InteropServices;

namespace Neusoft.HISFC.Components.Common.Forms
{
    public partial class frmYDZFRefund : Form
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
        GDSI.Process gdsi = new GDSI.Process();
        HisCallExternalServiceProject.ZDWYPayPlatform.ZFPTYDZFService _ydzfService = new HisCallExternalServiceProject.ZDWYPayPlatform.ZFPTYDZFService();
        HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        public string MedTransId { get; set; }
        private bool cancel = false;
        
        /// <summary>
        /// 是否取消 为true时收费人员点击了取消按钮
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
        }
        public ViewPayOrder _order;

        public frmYDZFRefund(ViewPayOrder order)
        {
            InitializeComponent();
            _order = order;
        }

        private void frmYDZFRefund_Load(object sender, EventArgs e)
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

        private void setInterface()
        {
            this.txtPayOrderId.Text = _order.OrderId;
            this.lblOrderType.Text = _order.ORDER_TYPE == "0"?"挂号":_order.ORDER_TYPE == "1"?"门诊收费":"";
            this.lblPAYMODE.Text = _order.PAY_TYPE == "WX" ? "微信" : _order.PAY_TYPE == "ZFB" ? "支付宝" : "";
            if (_order.YBSTATE == null)
            {
                if (_order.FS_SETLB_ORDER_STATE == "6")
                    this.lblYBRefundStatus.Text = "只退HIS";
            }
            else
            {
                switch (_order.YBSTATE)//医保退费状态
                {
                    case "SUCC":
                        this.lblYBRefundStatus.Text = "已退费";
                        break;
                    case "FAIL":
                        this.lblYBRefundStatus.Text = "退费失败";
                        break;
                    case "EXP":
                        this.lblYBRefundStatus.Text = "异常";
                        break;
                    case "":
                        if (_order.FS_SETLB_ORDER_STATE == "6")
                            this.lblYBRefundStatus.Text = "只退HIS";
                        else
                            this.lblYBRefundStatus.Text = "-";
                        break;
                }
            }
            switch (_order.REFUND_STATUS)//现金退款状态
            {
                case "0":
                case "":
                    this.lblXJRefundStatus.Text = "未退款";
                    break;
                case "1":
                    this.lblXJRefundStatus.Text = "退款中";
                    break;
                case "2":
                    this.lblXJRefundStatus.Text = "已退款";
                    break;
                case "3":
                    this.lblXJRefundStatus.Text = "退款异常";
                    break;
                case "4":
                    this.lblXJRefundStatus.Text = "退现金（非原路退款）";
                    break;
            }
        }

        private void btnYBRefund_Click(object sender, EventArgs e)
        {
            if(_order.YBSTATE == "SUCC")
            {
                MessageBox.Show("医保已退费。");
                return;
            }
            Res6203Model responseModel = null;
            int resStatus;
            if (rdbYB.Checked == true)
            {
                ReqFundRefund ReqfundRefund = new ReqFundRefund();
                string bussType = "";
                if (_order.ORDER_TYPE == "0")
                {
                    bussType = "01101";
                }
                else if (_order.ORDER_TYPE == "1")
                {
                    bussType = "01301";
                }
                string ecToken = gdsi.GetEcTokenBybussType(bussType);
                if (ecToken == "")
                    return;
                ReqfundRefund.FPSetlbID = _order.FPSetlbID;
                ReqfundRefund.EcToken = ecToken;
                ReqfundRefund.RefdType = "HI";
                ReqfundRefund.PayAuthNo = "";
                resStatus = _ydzfService.FundRefund(ReqfundRefund, ref responseModel);
            }
            else
            {
                ReqFundRefundByUserInfo ReqfundRefund  = new ReqFundRefundByUserInfo();
                ReqfundRefund.FPSetlbID = _order.FPSetlbID;
                ReqfundRefund.RefdType = "HI";
                ReqfundRefund.OperCode = outOp.Operator.ID;
                resStatus = _ydzfService.FundRefundByUserInfo(ReqfundRefund, ref responseModel);
                
            }
            if (resStatus == -1)
            {
                MessageBox.Show("退医保出错:" + _ydzfService.ErrMsg);
            }
            else
            {
                if (responseModel.RefStatus == "SUCC")
                    MessageBox.Show("医保退款成功");
                if (responseModel.RefStatus == "FAIL")
                    MessageBox.Show("医保退款失败");
                if (responseModel.RefStatus == "EXP")
                    MessageBox.Show("异常，医保退费成功，自费退费失败，可发起退纯自费重试");
            }
            _ydzfService.YDZFQuertOrderByMedTransId(MedTransId,ref _order);
            setInterface();
        }

        private void btnRefund_Click(object sender, EventArgs e)
        {
            if (_order.YBSTATE != "SUCC")
            {
                if (_order.FS_SETLB_ORDER_STATE != "6")
                {
                    MessageBox.Show("医保未退费，请先退医保。");
                    return;
                }
            }
            if (_order.REFUND_STATUS == "4" )
            {
                if (MessageBox.Show("此订单已经是 “退现金（非原路退款）” 状态，是否继续原路退款？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                } 
            }
            if(_order.REFUND_STATUS=="1"||_order.REFUND_STATUS=="2")
            {
                MessageBox.Show("已经发起退款或退款完成。");
                return;
            }
            ReqRefundModel rqModel = new ReqRefundModel();
            rqModel.OrderId = _order.OrderId;
            if (_order.PAY_TYPE =="WX")
                rqModel.PayChannel = "1";
            else if (_order.PAY_TYPE == "ZFB")
                rqModel.PayChannel = "2";
            rqModel.OperCode = outOp.Operator.ID;
            rqModel.OperName = outOp.Operator.Name;
            ResPayRefund responseModel = null;
            if (_ydzfService.YDZFPayRefund(rqModel, ref responseModel) == -1)
            {
                MessageBox.Show("退现金出错:" + _ydzfService.ErrMsg);
            }
            else
            {
                _ydzfService.YDZFQuertOrderByMedTransId(MedTransId,ref _order);
                setInterface();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _ydzfService.YDZFQuertOrderByMedTransId(MedTransId, ref _order);
            setInterface();
            if (this.lblYBRefundStatus.Text == "-" || this.lblXJRefundStatus.Text == "-")
            {
                MessageBox.Show("请先进行退费操作。");
                return;
            }
            if (this.lblYBRefundStatus.Text == "退费失败" || this.lblYBRefundStatus.Text == "异常")
            {
                MessageBox.Show("医保退费" + this.lblYBRefundStatus.Text);
                return;
            }
            if ( _order.REFUND_STATUS != "2" && _order.REFUND_STATUS != "4")
            {
                MessageBox.Show("现金退费" + this.lblXJRefundStatus.Text);
                return;
            }
            if ((this.lblYBRefundStatus.Text == "已退费" || this.lblYBRefundStatus.Text == "只退HIS") && (_order.REFUND_STATUS == "2" || _order.REFUND_STATUS == "4"))
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Close();
            return;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _ydzfService.YDZFQuertOrderByMedTransId(MedTransId, ref _order);
            setInterface();
        }

       

        public void MarkAsCashRefundOnlys()
        {
            _ydzfService.YDZFQuertOrderByMedTransId(MedTransId, ref _order);
            if (_order.YBSTATE == "SUCC")
            {
                MessageBox.Show("医保已退费。不能将订单设置为仅退HIS。");
                cancel = true;
                this.Close();
                return;
            }
            if (_order.FS_SETLB_ORDER_STATE == "6")
            {
                MessageBox.Show("已将订单设置为仅退HIS。");
            }
            else
            {
                ReqFundRefund ReqfundRefund = new ReqFundRefund();
                ReqfundRefund.FPSetlbID = _order.FPSetlbID;
                int resStatus = _ydzfService.MarkAsCashRefundOnlys(ReqfundRefund);
                if (resStatus == 1)
                {
                    MessageBox.Show("已将订单设置为仅退仅退HIS");
                }
                else
                {
                    MessageBox.Show("退医保出错:" + _ydzfService.ErrMsg);
                }
            }
            _ydzfService.YDZFQuertOrderByMedTransId(MedTransId, ref _order);
            setInterface();
            rdbUSER.Visible = false;
            rdbYB.Visible = false;
            btnYBRefund.Visible = false;
        }

        /// <summary>
        /// 退现金（非原路退款）
        /// </summary>
        public void SetNonOriginalRouteRefund()
        {
            _ydzfService.YDZFQuertOrderByMedTransId(MedTransId, ref _order);
            if (_order.REFUND_STATUS == "1" || _order.REFUND_STATUS == "2")
            {
                MessageBox.Show("已经发起退款或退款完成。不能将订单设置为 退现金（非原路退款）。");
                cancel = true;
                this.Close();
                return;
            }

            ReqRefundModel rqModel = new ReqRefundModel();
            rqModel.OrderId = _order.OrderId;
            if (_order.PAY_TYPE == "WX")
                rqModel.PayChannel = "1";
            else if (_order.PAY_TYPE == "ZFB")
                rqModel.PayChannel = "2";
            rqModel.OperCode = outOp.Operator.ID;
            rqModel.OperName = outOp.Operator.Name;
            int resStatus = _ydzfService.SetNonOriginalRouteRefund(rqModel);
            if (resStatus == 1)
            {
                MessageBox.Show("已将订单设置为退现金（非原路退款）。");
            }
            else
            {
                MessageBox.Show("设置订单状态出错:" + _ydzfService.ErrMsg);
            }
            _ydzfService.YDZFQuertOrderByMedTransId(MedTransId, ref _order);
            setInterface();
            btnRefund.Enabled = false;
            btnRefund.Visible = false;
        }
    }
}
