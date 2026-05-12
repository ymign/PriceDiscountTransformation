using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.BizProcess.Interface.FeeInterface;

namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    public partial class frmBankTrans : Form,IBankTrans
    {
        public frmBankTrans()
        {
            InitializeComponent();
        }
        private decimal totCost = 0m;

        public decimal TotCost
        {
            get { return totCost; }
            set { totCost = value;
            this.neuLabel1.Text = totCost.ToString();
            }
        }
        private List<Neusoft.FrameWork.Models.NeuObject> listTransInfo = new List<Neusoft.FrameWork.Models.NeuObject>();

        public List<Neusoft.FrameWork.Models.NeuObject> ListTransInfo
        {
            get { return listTransInfo; }
            set { listTransInfo = value; }
        }

        private void frmBankTrans_Load(object sender, EventArgs e)
        {
            this.neuSpread1_Sheet1.Rows.Count  = 0;
            if (inputListInfo[0].ToString() == "0")
            {
                this.Text = "收费";
            }
            else
            {
                this.Text="退费";
            }
            if (listTransInfo.Count  >0)
            {
                int i = 0;
                foreach (Neusoft.FrameWork.Models.NeuObject  no in listTransInfo)
                {
                    this.neuSpread1_Sheet1.Rows.Count++;
                    neuSpread1_Sheet1.Cells[i, 0].Text = no.Name;
                    neuSpread1_Sheet1.Cells[i, 1].Text = no.ID;
                    neuSpread1_Sheet1.Cells[i, 2].Text = no.Memo;
                    neuSpread1_Sheet1.Cells[i, 3].Text =no.User01;
                    i++;
                }
            }
            this.listTransInfo.Clear();
        }

        private void neuButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.neuSpread1_Sheet1.RowCount; i++)
            {
                if (string.IsNullOrEmpty(neuSpread1_Sheet1.Cells[i, 0].Text) == true)
                {
                    break;
                }
                else
                {
                    listTransInfo.Add(new Neusoft.FrameWork.Models.NeuObject());
                    listTransInfo[listTransInfo.Count - 1].ID = i + "id";
                    listTransInfo[listTransInfo.Count - 1].Name = neuSpread1_Sheet1.Cells[i,0].Text;
                    listTransInfo[listTransInfo.Count - 1].Memo = i + "Memo";
                    listTransInfo[listTransInfo.Count - 1].User01 = i + "User01";
                    ///  0:银行 1：账号 2：pos号 3：金额
                    this.outputListInfo.Add(neuSpread1_Sheet1.Cells[i, 0].Text);
                    this.outputListInfo.Add(neuSpread1_Sheet1.Cells[i, 1].Text);
                    this.outputListInfo.Add(neuSpread1_Sheet1.Cells[i, 2].Text);
                    this.outputListInfo.Add(neuSpread1_Sheet1.Cells[i, 3].Text);
                    //listTransInfo[listTransInfo.Count - 1].ID = i + "id";
                    //listTransInfo[listTransInfo.Count - 1].Name = neuSpread1_Sheet1.Cells[i, 0].Text;
                    //listTransInfo[listTransInfo.Count - 1].Memo = i + "Memo";
                    //listTransInfo[listTransInfo.Count - 1].User01 = i + "User01";
                }
            }
            this.Close();
            return;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.neuSpread1_Sheet1.Rows.Count++;
        }

        #region IBankTrans 成员

        public bool Do()
        {
            this.TotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(inputListInfo[1]);
            frmBankTrans_Load(null, null);
            this.ShowDialog();
            if (this.outputListInfo.Count < 4)
            {
                this.outputListInfo.Clear();
                this.outputListInfo.Add("失败");
                return false;
            }
            else
            {
                //this.outputListInfo.Add("成功");
                return true;
            }
        }
        private List<object> inputListInfo = new List<object>();

        public List<object> InputListInfo
        {
            get { return inputListInfo; }
            set { inputListInfo = value; }
        }
        private List<object> outputListInfo = new List<object>();

        public List<object> OutputListInfo
        {
            get { return outputListInfo; }
            set { outputListInfo = value; }
        }
       
     

        #endregion
    }
}
