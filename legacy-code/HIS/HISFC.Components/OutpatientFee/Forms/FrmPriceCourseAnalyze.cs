using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    public partial class FrmPriceCourseAnalyze : Form
    {
        public FrmPriceCourseAnalyze()
        {
            InitializeComponent();
        }

        public void Init(HisCallExternalServiceProject.FunctionModule.PriceMonitoringModule.Model.ResponseModel r)
        {
            int index = 0;
            this.fpSpread1_Sheet1.RowCount = 0;
            foreach (var item in r.Data)
            {
                this.fpSpread1_Sheet1.Rows.Add(index, 1);
                this.fpSpread1_Sheet1.Rows[index].Tag = item;
                this.fpSpread1_Sheet1.Cells[index, 0].Text = item.RuleId.ToString();
                this.fpSpread1_Sheet1.Cells[index, 1].Text = item.RuleName.ToString();
                this.fpSpread1_Sheet1.Cells[index, 2].Text = item.Message.ToString();
                this.fpSpread1_Sheet1.Cells[index, 3].Text = item.IllegalLevel.ToString();
                this.fpSpread1_Sheet1.Cells[index, 4].Text = item.IllegalExplain.ToString();
                this.fpSpread1_Sheet1.Cells[index, 5].Text = item.IllegalClass.ToString();
                index++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
