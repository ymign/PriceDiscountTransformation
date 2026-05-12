using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucScanPayGHlFee : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        /// <summary>
        /// 尝试退款计数，超过3弹出按钮提供退款
        /// </summary>
        int ConfirmRefund = 0;
        DataTable dt;
        /// <summary>
        /// 退款地址
        /// </summary>
        private string scanRefundUrl = string.Empty;
        /// <summary>
        /// 方鼎扫码墩数据库链接字符串
        /// </summary>
        string constring = string.Empty;
        Neusoft.HISFC.Models.Base.Employee empl = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;
        public ucScanPayGHlFee()
        {
            InitializeComponent();
            dt = new DataTable();
            dt.Columns.Add("order_id", typeof(System.String));
            dt.Columns.Add("his_order_id", typeof(System.String));
            dt.Columns.Add("transaction_id", typeof(System.String));
            dt.Columns.Add("fee", typeof(System.Decimal));
            dt.Columns.Add("pay_type", typeof(System.String));
            dt.Columns.Add("type", typeof(System.String));
            dt.Columns.Add("patient_id", typeof(System.String));
            dt.Columns.Add("patient_name", typeof(System.String));
            dt.Columns.Add("payment_at", typeof(System.DateTime));
            dataGridView1.DataSource = dt;
            Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParams = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
            this.scanRefundUrl = controlParams.GetControlParam<string>("PT0004", false, string.Empty);//退款地址初始化
            this.constring = controlParams.GetControlParam<string>("PT0005", false, string.Empty);//方鼎扫码墩数据库链接字符串初始化
            if (scanRefundUrl == string.Empty)
            {
                panel1.Dock = System.Windows.Forms.DockStyle.Fill;
                panel1.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Select();
        }

        private void Select()
        {
            if (txtQuery.Text.Trim() == "")
                return;
            ReSet();
            DataSet dts = new DataSet();
             Neusoft.HISFC.BizLogic.Manager.DataBase dbMgr = new Neusoft.HISFC.BizLogic.Manager.DataBase();
            using (MySqlConnection msc = new MySqlConnection(constring))
            {
                //写入sql语句
                string sql = @"select order_id,
                                      his_order_id,
                                      transaction_id,
                                      fee,
                                      pay_type,
                                      type,
                                      patient_id,
                                      patient_name,
                                      payment_at from v_scan_payment_orders where (order_id = '{0}' or his_order_id ='{0}' or transaction_id ='{0}' or patient_id = '{0}' or patient_name = '{0}') and pay_mode = 1 order by created_at desc";
                //创建命令对象
                sql = string.Format(sql, txtQuery.Text.Trim());
                //dts = dbMgr.ExecQuery(sql);
                MySqlCommand cmd = new MySqlCommand(sql, msc);
                //打开数据库连接
                msc.Open();
                //执行命令,ExcuteReader返回的是DataReader对象
                MySqlDataReader reader = cmd.ExecuteReader();
                //循环单行读取数据，当读取为null时，就退出循环

                while (reader.Read())
                {
                    DataRow row = dt.NewRow();
                    row["order_id"] = reader.GetValue(0).ToString();
                    row["his_order_id"] = reader.GetValue(1).ToString();
                    row["transaction_id"] = reader.GetValue(2).ToString();
                    row["fee"] = (reader.GetDecimal(3) * decimal.Parse("0.01"));
                    row["pay_type"] = reader.GetValue(4).ToString() == "4" ? "支付宝" : "微信";
                    row["type"] = reader.GetValue(5).ToString() == "1" ? "挂号" : reader.GetValue(5).ToString() == "3" ? "门诊缴费" : reader.GetValue(5).ToString() == "4" ? "住院预交金" : reader.GetValue(5).ToString();
                    row["patient_id"] = reader.GetValue(6).ToString();
                    row["patient_name"] = reader.GetValue(7).ToString();
                    if (reader.GetValue(8).ToString()!="")
                        row["payment_at"] = reader.GetDateTime(8);
                    dt.Rows.Add(row);
                }
            }
            dataGridView1.DataSource = dt;
        }

        /// <summary>
        /// 点击退款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReFund_Click(object sender, EventArgs e)
        {
            if (txtOrderId.Text == "")
                return;
            if (dataGridView2.RowCount > 0)
            {
                MessageBox.Show("该订单已进行过退款或为正常订单，无法通过此方式进行退款。");
                ConfirmRefund++;
                if (ConfirmRefund > 3)
                {
                    if (MessageBox.Show("确认这笔订单是需要退款的？", "询问", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        button2.Visible = true;
                        MessageBox.Show("请再点击确认退款按钮进行退款。");
                    }
                }
                return;
            }
            Neusoft.HISFC.Models.ScanPay.ScanRefundInfo SRFInfo = new Neusoft.HISFC.Models.ScanPay.ScanRefundInfo();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            SRFInfo.refund_order_id ="YC"+DateTime.Now.ToString("yyyyMMddhhmmss");
            SRFInfo.refund_fee = txtReFundFee.Text.Trim();
            SRFInfo.ORDER_ID = txtOrderId.Text.Trim();
            SRFInfo.OPERUserID = empl.ID;
            SRFInfo.REFUNDTYPE = "0";

            dic.Add("order_id", SRFInfo.ORDER_ID);//充值时的商户订单号
            dic.Add("refund_fee", SRFInfo.refund_fee);//退款金额（元）
            dic.Add("refund_order_id", SRFInfo.refund_order_id);//His退费订单号
            string result = string.Empty;
            result = Post(dic);
            JObject jResult = (JObject)JsonConvert.DeserializeObject(result);
            SRFInfo.CODE = jResult["code"].ToString();
            SRFInfo.MSG = jResult["msg"].ToString();
            SRFInfo.date_refund_order_id = "";
            SRFInfo.refund_transaction_id = "";
            SRFInfo.Patient_Id = txtPatientId.Text;
            SRFInfo.Patient_Name = txtPatientName.Text;
            if (jResult["code"].ToString() == "0")//成功
            {
                SRFInfo.date_refund_order_id = jResult["data"]["refund_order_id"].ToString();
                SRFInfo.refund_transaction_id = jResult["data"]["refund_transaction_id"].ToString();
            }
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpMInfo = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            //outpMInfo.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            if (!outpMInfo.InsertPaySCANREFUND(SRFInfo))
            {
                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("插入退款记录表失败!");
            }
            MessageBox.Show(SRFInfo.MSG);
            ReSet();
        }

        
        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <returns></returns>
        public string Post( Dictionary<string, string> dic)
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex <= -1)
                return;
            txtPatientId.Text = dataGridView1.Rows[e.RowIndex].Cells["patient_id"].Value.ToString();
            txtPatientName.Text = dataGridView1.Rows[e.RowIndex].Cells["patient_name"].Value.ToString();
            txtOrderId.Text = dataGridView1.Rows[e.RowIndex].Cells["order_id"].Value.ToString();
            txtReFundFee.Text = dataGridView1.Rows[e.RowIndex].Cells["fee"].Value.ToString();

            #region 按退款码查询付款记录和退款记录
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpMInfo = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            DataSet ds = outpMInfo.GetScanreFindByORDERID(txtOrderId.Text);
            #endregion
            if (ds != null)
                dataGridView2.DataSource = ds.Tables[0];
            else
            {
                DataTable dt2 = this.dataGridView2.DataSource as DataTable;
                if (dt2 != null)
                {
                    dt2.Rows.Clear();
                    this.dataGridView2.DataSource = dt2;
                }
            }
        }
        /// <summary>
        /// 重置控件状态
        /// </summary>
        private void ReSet()
        {
            DataTable dt1 = this.dataGridView1.DataSource as DataTable;
            if (dt1 != null)
            {
                dt1.Rows.Clear();
                this.dataGridView1.DataSource = dt1;
            }
            DataTable dt2 = this.dataGridView2.DataSource as DataTable;
            if (dt2 != null)
            {
                dt2.Rows.Clear();
                this.dataGridView2.DataSource = dt2;
            }
            txtPatientId.Text = "";
            txtPatientName.Text = "";
            txtOrderId.Text = "";
            txtReFundFee.Text = "";
            button2.Visible = false;
            ConfirmRefund = 0;
        }

        private void txtQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Select();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtOrderId.Text == "")
                return;
            
            Neusoft.HISFC.Models.ScanPay.ScanRefundInfo SRFInfo = new Neusoft.HISFC.Models.ScanPay.ScanRefundInfo();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            SRFInfo.refund_order_id = "YC" + DateTime.Now.ToString("yyyyMMddhhmmss");
            SRFInfo.refund_fee = txtReFundFee.Text.Trim();
            SRFInfo.ORDER_ID = txtOrderId.Text.Trim();
            SRFInfo.OPERUserID = empl.ID;
            SRFInfo.REFUNDTYPE = "0";

            dic.Add("order_id", SRFInfo.ORDER_ID);//充值时的商户订单号
            dic.Add("refund_fee", SRFInfo.refund_fee);//退款金额（元）
            dic.Add("refund_order_id", SRFInfo.refund_order_id);//His退费订单号
            string result = string.Empty;
            result = Post(dic);
            JObject jResult = (JObject)JsonConvert.DeserializeObject(result);
            SRFInfo.CODE = jResult["code"].ToString();
            SRFInfo.MSG = jResult["msg"].ToString();
            SRFInfo.date_refund_order_id = "";
            SRFInfo.refund_transaction_id = "";
            SRFInfo.Patient_Id = txtPatientId.Text;
            SRFInfo.Patient_Name = txtPatientName.Text;
            if (jResult["code"].ToString() == "0")//成功
            {
                SRFInfo.date_refund_order_id = jResult["data"]["refund_order_id"].ToString();
                SRFInfo.refund_transaction_id = jResult["data"]["refund_transaction_id"].ToString();
            }
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpMInfo = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            //outpMInfo.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            if (!outpMInfo.InsertPaySCANREFUND(SRFInfo))
            {
                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("插入退款记录表失败!");
            }
            MessageBox.Show(SRFInfo.MSG);
            ReSet();
        }
    }
}
