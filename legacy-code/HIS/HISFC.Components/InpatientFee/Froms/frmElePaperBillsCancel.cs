using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.InpatientFee.Forms
{
    public partial class frmElePaperBillsCancel : Form
    {
        public frmElePaperBillsCancel()
        {
            InitializeComponent();
            dtShow = (DataTable)dataGridView1.DataSource;
            dataGridView1.DataSource = dtShow;
        }

        Neusoft.HISFC.Models.Base.Employee employee = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;
        UseElecBillService.UseElecBillInPatient elecBillInPatient = new UseElecBillService.UseElecBillInPatient();
        DataTable dt;
        DataTable dtShow;

        private void btnQuery_Click(object sender, EventArgs e)
        {
            queryDate();
        }

        private void queryDate()
        {
            Neusoft.HISFC.BizLogic.Fee.InPatient inPatient = new Neusoft.HISFC.BizLogic.Fee.InPatient();
            DataSet ds = new DataSet();
            int returnCode = inPatient.GetElePaperBillsCancel(ref ds);//获取纸质票已作废电子票任有效的换开记录
            if (returnCode == 1)
            {
                try
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        dt = ds.Tables[0];
                    }
                    else
                    {
                        dt.Rows.Clear();
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    dt.Rows.Clear();
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                dt.Rows.Clear();
            }
            txtInvoiceNo.Text = "";
            showDate();
        }

        /// <summary>
        /// 显示数据
        /// </summary>
        private void showDate()
        {
            if (txtInvoiceNo.Text.Trim() == "")
            {
                dtShow = dt.Copy();
            }
            else
            {
                string str = txtInvoiceNo.Text.Trim();  // 要查询的字符串  

                // 使用LINQ进行模糊查询  
                var query = dt.AsEnumerable().Where(row => row.Field<string>("发票号").Contains(str));
                if (!query.Any())//如果为空
                {
                    dtShow.Rows.Clear();
                }
                else
                {
                    dtShow = query.CopyToDataTable();
                }
            }
            dataGridView1.DataSource = dtShow;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> checkedRows = dataGridView1.Rows.Cast<DataGridViewRow>()
            .Where(row =>
            {
                DataGridViewCheckBoxCell checkBoxCell = row.Cells[0] as DataGridViewCheckBoxCell;
                return checkBoxCell != null && checkBoxCell.Value != null && (bool)checkBoxCell.Value;
            }).ToList();
            if (checkedRows.Count < 1)
                return;
            UseElecBillService.UseElecBillInPatient elecInPatient = new UseElecBillService.UseElecBillInPatient();
            foreach (DataGridViewRow row in checkedRows)
            {
                string msg = string.Empty;
                string invoiceNo = row.Cells["发票号"].Value.ToString();
                string inpatientNo = row.Cells["住院流水号"].Value.ToString();
                string operatorCode = employee.ID;
                if (elecBillInPatient.InPatientFeeInvalidPaper(invoiceNo, inpatientNo, operatorCode, ref msg) == -1)
                {
                    MessageBox.Show(invoiceNo+"作废失败，错误信息:"+msg);
                }
            }
            txtInvoiceNo.Text = "";
            showDate();
        }

        private void txtInvoiceNo_TextChanged(object sender, EventArgs e)
        {
            showDate();
        }
    }
}
