using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;

namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    public partial class frmScanReFind : Form
    {
        /// <summary>
        /// 退费接口地址
        /// </summary>
        private string scanRefundUrl = string.Empty;
        /// <summary>
        /// 可退金额查询地址
        /// </summary>
        private string RefundableUrl = string.Empty;
        /// <summary>
        /// 退款接口访问信息
        /// </summary>
        private Neusoft.HISFC.Models.ScanPay.ScanRefundInfo SRFInfo;
        /// <summary>
        /// 人员实体
        /// </summary>
        Neusoft.HISFC.Models.Base.Employee empl = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;
        /// <summary>
        /// 退款类型
        /// </summary>
        string ScanReFindType;
        string PatientID;
        string PatientName;
        /// <summary>
        /// 退款接口访问信息
        /// </summary>
        public Neusoft.HISFC.Models.ScanPay.ScanRefundInfo ScanRefundInfo
        {
            get { return SRFInfo; }
        }
        private bool result = false;
        /// <summary>
        /// 退款结果 true成功
        /// </summary>
        public bool Result
        {
            get { return result; }
        }
        private bool cancel = false;
        /// <summary>
        /// 是否取消 为true时收费人员点击了取消按钮
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
        }

        ArrayList ScanPayDirectRefundList;//有直接退款权限的用户
        /// <summary>
        /// 扫码墩退款
        /// </summary>
        /// <param name="InvoiceNO">发票号</param>
        /// <param name="ReFundFee">退款金额</param>
        /// <param name="Type">费用类型</param>
        public frmScanReFind(string PATIENT_ID,string fInvoiceNO, string fReFundFee, string fType, string fPatientID, string fPatientName)
        {
            InitializeComponent();
            txtInvoiceNO.Text = fInvoiceNO;
            txtReFundFee.Text = fReFundFee;
            ScanReFindType = fType;
            PatientName = fPatientName;
            PatientID = fPatientID;
            string ORDER_ID ="";
            string Type = "";
            decimal totalFee = -1;
            decimal totalRefundFee = -1;
            decimal refundableFee = -1;
            Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParams = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
            this.scanRefundUrl = controlParams.GetControlParam<string>("PT0004", false, string.Empty);//退款地址初始化
            this.RefundableUrl = controlParams.GetControlParam<string>("PT0006", false, string.Empty);//退款地址初始化
            Neusoft.HISFC.BizLogic.Manager.Constant con = new Neusoft.HISFC.BizLogic.Manager.Constant();
            this.ScanPayDirectRefundList = con.GetList("ScanPayDirectRefund");//获取有直接退款权限的用户
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpMInfo = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            if (outpMInfo.GetScanreFindByORDERID(RefundableUrl, PATIENT_ID,fInvoiceNO, fType, ref ORDER_ID, ref Type, ref totalFee, ref totalRefundFee, ref refundableFee))
            {
                txtOrderID.Text = ORDER_ID;
                txtPayType.Text = Type;
                txtFee.Text = totalFee.ToString();
                txtReFee.Text = totalRefundFee.ToString();
                txtBalance.Text = refundableFee.ToString();
            }
            else
            {
                panel1.Visible = true;
                btnReFundFee.Enabled = false;
            }
            if (decimal.Parse(txtBalance.Text) < Math.Abs(decimal.Parse(txtReFundFee.Text)))
            {
                for (int i = 0; i < ScanPayDirectRefundList.Count; i++)
                {
                    if (ScanPayDirectRefundList[i].ToString() == empl.ID)
                    {
                        this.btnRe.Visible = true;
                        break;
                    }
                }
            }
            else
            {
                this.btnRe.Visible = false;
            }
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <returns></returns>
        public string Post(Dictionary<string, string> dic)
        {
            string LogAddress = "";
            if (!System.IO.Directory.Exists(Application.StartupPath + "\\扫码墩退款日志"))
            {
                System.IO.Directory.CreateDirectory(Application.StartupPath + "\\扫码墩退款日志");
            }
            if (LogAddress == "")
            {
                LogAddress = Application.StartupPath + "\\扫码墩退款日志\\" +
                    DateTime.Now.Year + '-' +
                    DateTime.Now.Month + '-' +
                    DateTime.Now.Day + '-' +
                    DateTime.Now.Hour + "_Log.log";
            }
            //把异常信息输出到文件
            System.IO.StreamWriter fs = new System.IO.StreamWriter(LogAddress, true);
            fs.WriteLine("当前时间：" + DateTime.Now.ToString());
            try
            {
                if (scanRefundUrl == string.Empty)
                {
                    MessageBox.Show("接口地址未配置！", "提示");
                    return "";
                }
                fs.WriteLine("请求地址：");
                fs.WriteLine(scanRefundUrl);
                string result = "";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(scanRefundUrl);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                #region 添加Post 参数
                StringBuilder builder = new StringBuilder();
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
                fs.WriteLine("请求参数：");
                fs.WriteLine(builder.ToString());
                byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
                req.ContentLength = data.Length;
                using (System.IO.Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                fs.WriteLine("响应内容：");
                fs.WriteLine(result);
                fs.WriteLine("==================================================================================================================");
                fs.Close();
                return result;
            }
            catch (Exception ex)
            {
                fs.WriteLine("异常信息：" + ex.Message);
                fs.WriteLine("异常对象：" + ex.Source);
                fs.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
                fs.WriteLine("触发方法：" + ex.TargetSite);
                fs.Close();
                throw;
            }
        }

        private void btnReFundFee_Click(object sender, EventArgs e)
        {
            if (decimal.Parse(txtBalance.Text) < Math.Abs(decimal.Parse(txtReFundFee.Text)))
            {
                MessageBox.Show("可退金额不足。");
                result = false;
                return;
            }
            SRFInfo = new Neusoft.HISFC.Models.ScanPay.ScanRefundInfo();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            SRFInfo.refund_order_id = "YC" + DateTime.Now.ToString("yyyyMMddhhmmss");
            SRFInfo.refund_fee = Math.Abs(decimal.Parse(txtReFundFee.Text)).ToString();
            SRFInfo.ORDER_ID = txtOrderID.Text.Trim();
            SRFInfo.OPERUserID = empl.ID;
            SRFInfo.REFUNDTYPE = ScanReFindType;

            dic.Add("order_id", SRFInfo.ORDER_ID);//充值时的商户订单号
            dic.Add("refund_fee", SRFInfo.refund_fee);//退款金额（元）
            dic.Add("refund_order_id", SRFInfo.refund_order_id);//His退费订单号
            string res = string.Empty;
            res = Post(dic);
            JObject jResult = (JObject)JsonConvert.DeserializeObject(res);
            SRFInfo.CODE = jResult["code"].ToString();
            SRFInfo.MSG = jResult["msg"].ToString();
            SRFInfo.date_refund_order_id = "";
            SRFInfo.refund_transaction_id = "";
            SRFInfo.Patient_Id = PatientID;
            SRFInfo.Patient_Name = PatientName;
            if (jResult["code"].ToString() == "0")//成功
            {
                SRFInfo.date_refund_order_id = jResult["data"]["refund_order_id"].ToString();
                SRFInfo.refund_transaction_id = jResult["data"]["refund_transaction_id"].ToString();
                result = true;
            }
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpMInfo = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            if (!outpMInfo.InsertPaySCANREFUND(SRFInfo))
            {
                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("插入退款记录表失败!");
            }
            
            MessageBox.Show(SRFInfo.MSG);
            this.Close();
        }
        

        private void btnQuit_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Close();
            return;
        }


        /// <summary>
        /// 指定Get地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public string Get(string url, Dictionary<string, string> dic)
        {
            string LogAddress = "";
            if (!System.IO.Directory.Exists(Application.StartupPath + "\\扫码墩查询日志"))
            {
                System.IO.Directory.CreateDirectory(Application.StartupPath + "\\扫码墩查询日志");
            }
            if (LogAddress == "")
            {
                LogAddress = Application.StartupPath + "\\扫码墩查询日志\\" +
                    DateTime.Now.Year + '-' +
                    DateTime.Now.Month + '-' +
                    DateTime.Now.Day + '-' +
                    DateTime.Now.Hour + "_Log.log";
            }
            //把异常信息输出到文件
            System.IO.StreamWriter fs = new System.IO.StreamWriter(LogAddress, true);
            fs.WriteLine("当前时间：" + DateTime.Now.ToString());
            try
            {
                string result = "";
                #region 参数
                StringBuilder builder = new StringBuilder();
                builder.Append(url + "?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
                #endregion
                fs.WriteLine("请求地址：");
                fs.WriteLine(builder.ToString());

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder.ToString());
                req.Method = "GET";
                req.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();

                //获取响应内容
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                fs.WriteLine("响应内容：");
                fs.WriteLine(result);
                fs.WriteLine("==================================================================================================================");
                fs.Close();
                return result;
            }
            catch (Exception ex)
            {
                fs.WriteLine("异常信息：" + ex.Message);
                fs.WriteLine("异常对象：" + ex.Source);
                fs.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
                fs.WriteLine("触发方法：" + ex.TargetSite);
                fs.Close();
                throw;
            }
            
            
        }
        /// <summary>
        /// 设置直接退款按钮是否可见
        /// </summary>
        /// <param name="value"></param>
        public void SetbtnRe(bool value)
        {
            btnRe.Visible = value;
        }

        /// <summary>
        /// 不通过接口直接退款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRe_Click(object sender, EventArgs e)
        {
            if (decimal.Parse(txtBalance.Text) >= Math.Abs(decimal.Parse(txtReFundFee.Text)))
            {
                return;
            }
            if (MessageBox.Show("此操作将不会调用原路退款接口，是否继续退费?", "", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }
            result = true;
            this.Close();
        }
    }
}
