using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HisCallExternalServiceProject.CreditPayRefundService;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucCreditPayRefund : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        HisCallExternalServiceProject.CreditPayRefund creditPayRefund = new HisCallExternalServiceProject.CreditPayRefund();

        HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        string shospitalOrderNo;
        string shospitalSerialNo;
        string stotcost;
        public ucCreditPayRefund()
        {
            InitializeComponent();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            clear();//清空界面
            ResPayQuery ResPayQuery = null;//回参
            string PatientCard = txtIdCard.Text.Trim();//身份证
            string StartTime = dtSt.Value.ToString("yyyy-MM-dd 00:00:00");//起始时间
            string EndTime = dtSt.Value.ToString("yyyy-MM-dd 23:59:59");//结束时间
            if (PatientCard == "")
            {
                MessageBox.Show("请输入身份证。");
                return;
            }
            try
            {
                ResPayQuery = creditPayRefund.CreditPayPayQuery(PatientCard, StartTime, EndTime);
                if (ResPayQuery != null)
                {
                    if (ResPayQuery.PayOrderList.Count() > 0)
                    {
                        dataGridView1.Rows.Clear();
                        foreach(PaymentOrder pay in  ResPayQuery.PayOrderList)
                        {
                            int index = dataGridView1.Rows.Add();
                            dataGridView1.Rows[index].Cells[0].Value = pay.HospitalSerialNo;
                            dataGridView1.Rows[index].Cells[1].Value = pay.HospitalOrderNo;
                            dataGridView1.Rows[index].Cells[2].Value = pay.OrderNo;
                            dataGridView1.Rows[index].Cells[3].Value = pay.PatientCard;
                            dataGridView1.Rows[index].Cells[4].Value = pay.PatientName;
                            dataGridView1.Rows[index].Cells[5].Value = pay.PayStatus == "1" ? "支付成功" : "支付失败";
                            dataGridView1.Rows[index].Cells[6].Value = pay.TotalAmount;
                            dataGridView1.Rows[index].Cells[7].Value = pay.PayTime;
                            dataGridView1.Rows[index].Cells[8].Value = pay.RefundAmount;
                            dataGridView1.Rows[index].Cells[9].Value = pay.RefundStatus == "0" ? "未退款" : pay.RefundStatus == "1" ? "待退款" : pay.RefundStatus == "2" ? "退款完成" : pay.RefundStatus == "3" ? "退款失败" : pay.RefundStatus;
                            dataGridView1.Rows[index].Cells[10].Value = pay.TransactionNo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("查询缴费记录出错：" + ex.Message); 
            }
            
        }

        //退款按钮
        private void button1_Click(object sender, EventArgs e)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            try
            {
               ResRefund res = creditPayRefund.Refund(this.shospitalOrderNo, this.shospitalSerialNo, outpatientManager.Operator.ID, this.stotcost, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
               if (res != null)
               {
                   string RefundStatus = res.RefundStatus == "0" ? "未退款" : res.RefundStatus == "1" ? "待退款" : res.RefundStatus == "2" ? "退款完成" : res.RefundStatus == "3" ? "退款失败" : res.RefundStatus;
                   MessageBox.Show("退款调用成功，当前退款状态：" + RefundStatus);
               }
               else
               {
                   MessageBox.Show("退款调用失败返回值为空。请重新查询。");
               }
            }
            catch (Exception ex)
            {
                MessageBox.Show("退款调用失败：" + ex.Message);
            }
            clear();//清空界面
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                clear();//清空界面
                setData(e.RowIndex);//界面赋值
            }
        }

        private void clear()
        {
            lblhisNo.Text = "-";
            lblIDcard.Text = "-";
            lblName.Text = "-";
            lblorderNo.Text = "-";
            lblpayStatus.Text = "-";
            lblrefundStatus.Text = "-";
            lbltransactionNo.Text = "-";
            lbltotalAmount.Text = "-";
            lblpayTime.Text = "-";
            shospitalOrderNo = "";
            shospitalSerialNo = "";
            stotcost = "";
            button1.Enabled = false;
        }

        public void setData(int index)
        {
            lblIDcard.Text = dataGridView1.Rows[index].Cells[3].Value!=null?dataGridView1.Rows[index].Cells[3].Value.ToString():"-";
            lblName.Text = dataGridView1.Rows[index].Cells[4].Value != null ? dataGridView1.Rows[index].Cells[4].Value.ToString() : "-";
            lblorderNo.Text = dataGridView1.Rows[index].Cells[2].Value != null ? dataGridView1.Rows[index].Cells[2].Value.ToString() : "-";
            lblpayStatus.Text = dataGridView1.Rows[index].Cells[5].Value != null ? dataGridView1.Rows[index].Cells[5].Value.ToString() : "-";
            lblrefundStatus.Text = dataGridView1.Rows[index].Cells[9].Value != null ? dataGridView1.Rows[index].Cells[9].Value.ToString() : "-";
            lbltotalAmount.Text = dataGridView1.Rows[index].Cells[6].Value != null ? dataGridView1.Rows[index].Cells[6].Value.ToString() : "-";
            lblpayTime.Text = dataGridView1.Rows[index].Cells[7].Value != null ? dataGridView1.Rows[index].Cells[7].Value.ToString() : "-";
            lbltransactionNo.Text = dataGridView1.Rows[index].Cells[10].Value != null ? dataGridView1.Rows[index].Cells[10].Value.ToString() : "-";
            string InvoiceNo = outOp.getInvoiceNobyHosNo(dataGridView1.Rows[index].Cells[1].Value.ToString());
            if (InvoiceNo != "")
            {
                lblhisNo.Text = InvoiceNo;
                button1.Enabled = false;
            }
            else
            {
                lblhisNo.Text = "-";
                shospitalOrderNo = dataGridView1.Rows[index].Cells[1].Value.ToString();
                shospitalSerialNo = dataGridView1.Rows[index].Cells[0].Value.ToString();
                stotcost = lbltotalAmount.Text;
                button1.Enabled = true;
            }
        }
    }
}
