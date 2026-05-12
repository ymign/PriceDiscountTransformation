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
    /// <summary>
    /// 门诊账号发票累计
    /// </summary>
    public partial class frmCountAccountInvoices : Form
    {
        /// <summary>
        /// 门诊账号发票累计
        /// </summary>
        public frmCountAccountInvoices()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 门诊账户操作业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.Account accMgr = new Neusoft.HISFC.BizLogic.Fee.Account();

        #endregion 

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmCountAccountInvoices_Load(object sender, EventArgs e)
        {
            this.tbCount.Select();
            this.tbCount.Focus();
        }

        /// <summary>
        /// 输入发票张数回车事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Query();
            }
        }

        /// <summary>
        /// 发票查询
        /// </summary>
        private void Query()
        {
            string countTemp = this.tbCount.Text;
            this.Clear();
            this.tbCount.Text = countTemp;

            if (string.IsNullOrEmpty(this.tbCount.Text.Trim()))
            {
                MessageBox.Show("请输入发票张数!");
                this.tbCount.Focus();
                return;
            }

            //张数
            int count = 0;

            try
            {
                count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.tbCount.Text.ToString());

                if (count <= 0)
                {
                    MessageBox.Show("请输入大于0的整数!");
                    this.tbCount.Focus();
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("输入数字非法，请输入阿拉伯数字! " + ex.Message);
                this.tbCount.Focus();
                return;
            }

            DataSet dsResult = new DataSet();

            int returnValue = this.accMgr.GetAccountInvoiceByCount(this.accMgr.Operator.ID, count, ref dsResult);
            if (returnValue == -1)
            {
                MessageBox.Show(this.accMgr.Err);
                this.tbCount.Focus();
                return;
            }

            DataTable dt = dsResult.Tables[0];
            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("该收费员在最近24小时内没有进行门诊充值!");
                this.tbCount.Focus();
                return;
            }

            decimal totCost = 0m;
            decimal totCash = 0m;
            decimal totCheck = 0m;
            decimal totPos = 0m;

            this.neuSpread1_Sheet1.Rows.Count = 0;
            int rowIndex = this.neuSpread1_Sheet1.Rows.Count;
            foreach (DataRow dr in dt.Rows)
            {
                this.neuSpread1_Sheet1.Rows.Add(rowIndex, 1);

                this.neuSpread1_Sheet1.Cells[rowIndex, 0].Text = dr[0].ToString();
                this.neuSpread1_Sheet1.Cells[rowIndex, 1].Text = dr[1].ToString();
                this.neuSpread1_Sheet1.Cells[rowIndex, 2].Text = dr[2].ToString();
                this.neuSpread1_Sheet1.Cells[rowIndex, 3].Text = dr[3].ToString();
                this.neuSpread1_Sheet1.Cells[rowIndex, 4].Text = dr[4].ToString();
                this.neuSpread1_Sheet1.Cells[rowIndex, 5].Text = dr[5].ToString();

                totCost += Neusoft.FrameWork.Function.NConvert.ToDecimal(dr[4].ToString());
                if (dr[6].ToString() == "CA")
                {
                    totCash += Neusoft.FrameWork.Function.NConvert.ToDecimal(dr[4].ToString());
                }
                else if (dr[6].ToString() == "CH")
                {
                    totCheck += Neusoft.FrameWork.Function.NConvert.ToDecimal(dr[4].ToString());
                }
                else
                {
                    totPos += Neusoft.FrameWork.Function.NConvert.ToDecimal(dr[4].ToString());
                }

                rowIndex++;

            }

            this.tbTot.Text = totCost.ToString("F2");
            this.tbCash.Text = totCash.ToString("F2");
            this.tbCheck.Text = totCheck.ToString("F2");
            this.tbPos.Text = totPos.ToString("F2");

            this.tbReal.Select();
            this.tbReal.Focus();

        }

        /// <summary>
        /// 清屏操作
        /// </summary>
        private void Clear()
        {
            this.neuSpread1_Sheet1.Rows.Count = 0;
            this.neuSpread1_Sheet1.Rows.Add(this.neuSpread1_Sheet1.Rows.Count, 1);

            this.tbCount.Text = "";
            this.tbTot.Text = "";
            this.tbCash.Text = "";
            this.tbPos.Text = "";
            this.tbCheck.Text = "";
            this.tbReal.Text = "";
            this.tbReturn.Text = "";
            this.tbCount.Select();
            this.tbCount.Focus();

        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btQuery_Click(object sender, EventArgs e)
        {
            this.Query();
        }

        /// <summary>
        /// 清屏按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClear_Click(object sender, EventArgs e)
        {
            this.Clear();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btExit_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        /// <summary>
        /// 按回车退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btExit_KeyDown(object sender, KeyEventArgs e)
        {
            this.FindForm().Close();
        }

        /// <summary>
        /// 实收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbReal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    decimal realCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.tbReal.Text.Trim());
                    decimal cashCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.tbCash.Text.Trim());

                    if (realCost < cashCost)
                    {
                        MessageBox.Show("您输入实收金额小于应收现金!请重新输入!");
                        this.tbReal.Select();
                        this.tbReal.Focus();
                        return;
                    }

                    decimal returnCost = realCost - cashCost;
                    this.tbReturn.Text = returnCost.ToString("F2");
                    this.tbReturn.Focus();
                    this.tbReturn.SelectAll();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("输入非法数字! " + ex.Message);
                    this.tbReal.Select();
                    this.tbReal.Focus();
                    return;
                }
            }
        }

        /// <summary>
        /// 控制回车跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbReturn_KeyDown(object sender, KeyEventArgs e)
        {
            this.btExit.Select();
            this.btExit.Focus();
        }
        
        /// <summary>
        /// 按ECS退出界面
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.FindForm().Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }




    }
}
