using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HisCallExternalServiceProject.FunctionModule.PriceMonitoringModule.Model;

namespace Neusoft.HISFC.Components.Common.Forms
{
    public partial class FrmPriceCourseAnalyze : Form
    {
        public FrmPriceCourseAnalyze()
        {
            InitializeComponent();
        }
        private DialogResult res = DialogResult.No;

        public void ZK() 
        {
            button1.Text = "确认转科";
            btnCancel.Text = "取消转科";
        }

        /// <summary>
        /// 返回值
        /// </summary>
        public DialogResult Res
        {
            get { return res; }
        }
        public HisCallExternalServiceProject.FunctionModule.PriceMonitoringModule.Model.ResponseModel responseModel = new HisCallExternalServiceProject.FunctionModule.PriceMonitoringModule.Model.ResponseModel();

        public void Init(HisCallExternalServiceProject.FunctionModule.PriceMonitoringModule.Model.ResponseModel r)
        {
            responseModel = r;
            int index = 0;
            //this.label1.Text = r.Message.ToString();
            this.fpSpread1_Sheet1.RowCount = 0;
            FarPoint.Win.Spread.CellType.RichTextCellType richTextCellType =
 new FarPoint.Win.Spread.CellType.RichTextCellType();
            r.Data = r.Data.OrderByDescending(o => o.IllegalLevel).ToArray();
            foreach (var item in r.Data)
            {

                this.fpSpread1_Sheet1.Rows.Add(index, 1);
                this.fpSpread1_Sheet1.Rows[index].Tag = item;
                this.fpSpread1_Sheet1.Rows[index].Height = 60;
                this.fpSpread1_Sheet1.Cells[index, 0].Text = item.RuleName.ToString();
                this.fpSpread1_Sheet1.Cells[index, 1].Text = item.Message.ToString();
                this.fpSpread1_Sheet1.Cells[index, 2].Text = item.IllegalCost.ToString();
                string IllegalLevel = item.IllegalLevel;
                if (IllegalLevel == "30")
                {
                    IllegalLevel = "【禁止】";
                    this.fpSpread1_Sheet1.Rows[index].BackColor = Color.Red;
                }
                else if (IllegalLevel == "21")
                {
                    IllegalLevel = "【警告】";
                    this.fpSpread1_Sheet1.Rows[index].BackColor = Color.DeepSkyBlue;
                   
                }
                else if (IllegalLevel == "20")
                {
                    this.fpSpread1_Sheet1.Rows[index].BackColor = Color.LightGreen;
                    IllegalLevel = "【提醒】";
                }
                else
                {
                    IllegalLevel = "【提示】";
                }
                this.fpSpread1_Sheet1.Cells[index, 3].Text = IllegalLevel;
                this.fpSpread1_Sheet1.Cells[index, 4].Text = item.IllegalClass.ToString() == "1" ? "行为类" : "项目类";
                //this.fpSpread1_Sheet1.Cells[index, 5].Text = item.IllegalClass.ToString();
                index++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //List<ResponseData> list = new List<ResponseData>();
            //for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            //{
            //    var responseData = fpSpread1_Sheet1.Rows[i].Tag as ResponseData;
            //    string mark = this.fpSpread1_Sheet1.Cells[i, 5].Text.ToString();
            //    if (responseData.IllegalLevel == "21")
            //    {
            //        if (string.IsNullOrEmpty(mark))
            //        {
            //            MessageBox.Show("请填写反馈理由！");
            //            return;
            //        }
            //        list.Add(responseData);
            //    }

            //}
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
