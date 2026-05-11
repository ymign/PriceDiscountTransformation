using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HisCallExternalServiceProject.ZDWYPayPlatform.Models.YDZF;

namespace Neusoft.HISFC.Components.Common.Forms
{
    public partial class frmOfflineRefund : Neusoft.FrameWork.WinForms.Forms.BaseForm
    {
        Neusoft.HISFC.BizLogic.Fee.Outpatient outBiz = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        //权限管理类
        Neusoft.HISFC.BizLogic.Manager.UserPowerDetailManager privManager = new Neusoft.HISFC.BizLogic.Manager.UserPowerDetailManager();
        public frmOfflineRefund()
        {
            InitializeComponent();
            setDateView(null);
        }
        DataSet ds;
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                
                try
                {
                     ds = outBiz.GetOffLineRefundYDZFOrderInfo(txtOrderID.Text.Trim());
                }
                catch
                {
                    ds = null;
                    throw;
                }
                finally
                {
                    setDateView(ds);
                }
            }
        }

        private void setDateView(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                Label[] labels = panel2.Controls.OfType<Label>().ToArray();

                for (int i = 0; i < labels.Length; i++)
                {
                    labels[i].Text = string.Empty;
                }

                ckbAcctUsedFlag.Checked = false;
            }
            else
            {
                Label[] labels = panel2.Controls.OfType<Label>().ToArray();
                for (int i = 0; i < labels.Length; i++)
                {
                    labels[i].Text =ds.Tables[0].Columns[i].ColumnName +":"+ ds.Tables[0].Rows[0][i].ToString();
                }
                if (ds.Tables[0].Rows[0]["个帐使用标识"].ToString() == "0")
                {
                    ckbAcctUsedFlag.Checked = false;
                }
                else
                {
                    ckbAcctUsedFlag.Checked = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("请先查询订单");
                return;
            }
            HisCallExternalServiceProject.ZDWYPayPlatform.ZFPTYDZFService zfptService = new HisCallExternalServiceProject.ZDWYPayPlatform.ZFPTYDZFService();
            ResOfflineRefund responseModel = new ResOfflineRefund();
            string OrderId =ds.Tables[0].Rows[0]["订单号"].ToString();
            string FPSetlbID=ds.Tables[0].Rows[0]["医保结算业务id"].ToString();
            string OrderType = ds.Tables[0].Rows[0]["订单类型"].ToString();
            string PayChannel = "";
            string AcctUsedFlag="0";
            string OperCode = privManager.Operator.ID;

            if (ds.Tables[0].Rows[0]["支付方式"].ToString() == "WX")
            {
                PayChannel = "1";
            }
            if (ckbAcctUsedFlag.Checked)
            {
                AcctUsedFlag = "1";
            }
            if (zfptService.YDZFOfflineRefund(OrderId, FPSetlbID, OrderType, PayChannel, AcctUsedFlag, OperCode, ref responseModel) == 1)
            {
                string msg = "";
                if (!string.IsNullOrEmpty(responseModel.InsuranceCancelStatus))
                {
                    msg = "医保退款\r\n";
                    if (responseModel.InsuranceCancelStatus == "1")
                    {
                        msg += "成功\r\n";
                    }
                    else
                    {
                        msg += "失败，" + responseModel.InsuranceCancelMsg + "\r\n";
                    }
                }
                if (!string.IsNullOrEmpty(responseModel.AcctCancelStatus))
                {
                    msg += "个账退款\r\n";
                    if (responseModel.AcctCancelStatus == "1")
                    {
                        msg += "成功";
                    }
                    else
                    {
                        msg += "失败，" + responseModel.AcctCancelMsg;
                    }
                }
                MessageBox.Show(msg, "退款结果");
            }
            else
            {
                MessageBox.Show("退款出错:" + zfptService.ErrMsg, "退款出错", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
