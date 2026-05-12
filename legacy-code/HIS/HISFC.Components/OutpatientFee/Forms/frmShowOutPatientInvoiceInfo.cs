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
    /// 显示患者的发票信息
    /// </summary>
    public partial class frmShowOutPatientInvoiceInfo : Form
    {
        /// <summary>
        /// 显示患者的发票信息
        /// </summary>
        public frmShowOutPatientInvoiceInfo()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 费用层业务类
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.FeeReport feeMgr = new Neusoft.HISFC.BizLogic.Fee.FeeReport();

        /// <summary>
        /// 门诊账户业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.Account accMgr = new Neusoft.HISFC.BizLogic.Fee.Account();
        
        /// <summary>
        /// 患者基本信息
        /// </summary>
        private Neusoft.HISFC.Models.Registration.Register regInfo;

        /// <summary>
        /// 患者基本信息
        /// </summary>
        public Neusoft.HISFC.Models.Registration.Register RegInfo
        {
            get
            {
                return this.regInfo;
            }
            set
            {
                this.regInfo = value;
            }
        }

        /// <summary>
        /// 是否启用门诊账户
        /// </summary>
        private bool isAccount = false;

        /// <summary>
        /// 是否启用门诊账户
        /// </summary>
        public bool IsAccount
        {
            get
            {
                return this.isAccount;
            }
            set
            {
                this.isAccount = value;
            }
        }



        #endregion

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmShowOutPatientInvoiceInfo_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            DateTime dtNow = this.feeMgr.GetDateTimeFromSysDateTime();
            this.neuBeginDate.Value = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);
            this.neuEndDate.Value = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 23, 59, 59);

            if (this.RegInfo == null || string.IsNullOrEmpty(this.RegInfo.PID.CardNO))
            {
                MessageBox.Show("没有患者的基本信息!");
                return;
            }

            if (this.IsAccount)
            {
                this.neuLabel8.Visible = true;
                this.tbVacancy.Visible = true;
                decimal vacancy = 0m;
                if (this.accMgr.GetVacancy(this.RegInfo.PID.CardNO, ref vacancy) == -1)
                {
                    MessageBox.Show(this.accMgr.Err);
                    return;
                }

                this.tbVacancy.Text = vacancy.ToString("F2");
            }

            this.QueryPatientInvoiceInfo();

        }

        /// <summary>
        /// 查询患者基本信息
        /// </summary>
        private void QueryPatientInvoiceInfo()
        {
            this.neuSpread1_Sheet1.Rows.Count = 0; //先清空
            this.neuSpread1.ActiveSheet = this.neuSpread1_Sheet1; //查询之后活动sheet

            //赋值
            this.tbName.Text = this.RegInfo.Name;
            this.tbCardNo.Text = this.RegInfo.PID.CardNO;
            this.tbBirth.Text = this.RegInfo.Birthday.ToString("yyyy-MM-dd HH:mm:ss");
            this.tbGender.Text = this.RegInfo.Sex.Name;
            this.tbIDNo.Text = this.RegInfo.IDCard;

            //查询发票信息
            DataSet dsResult = new DataSet();
            int returnValue = this.feeMgr.QueryOutPatientInvoiceInfoByDate(this.RegInfo.PID.CardNO, this.neuBeginDate.Value.ToString(), this.neuEndDate.Value.ToString(), ref dsResult);

            if (returnValue == -1)
            {
                MessageBox.Show(this.feeMgr.Err);
                return;
            }

            DataTable dt = dsResult.Tables[0];
            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("该患者今天之内没有产生任何发票信息!");
                return;
            }

            decimal totCost = 0m;
            decimal totPub = 0m;
            decimal totOwn = 0m;

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
                this.neuSpread1_Sheet1.Cells[rowIndex, 6].Text = dr[6].ToString();
                this.neuSpread1_Sheet1.Cells[rowIndex, 7].Text = dr[7].ToString();
                this.neuSpread1_Sheet1.Cells[rowIndex, 8].Text = dr[8].ToString();
                this.neuSpread1_Sheet1.Rows[rowIndex].Tag = dr[9].ToString();

                totCost += Neusoft.FrameWork.Function.NConvert.ToDecimal(dr[4].ToString());
                totPub += Neusoft.FrameWork.Function.NConvert.ToDecimal(dr[5].ToString());
                totOwn += Neusoft.FrameWork.Function.NConvert.ToDecimal(dr[6].ToString());

                rowIndex++;
            }

            rowIndex = this.neuSpread1_Sheet1.Rows.Count;
            this.neuSpread1_Sheet1.Rows.Add(rowIndex, 1);

            this.neuSpread1_Sheet1.Cells[rowIndex, 0].Text = "合计:";
            this.neuSpread1_Sheet1.Cells[rowIndex, 4].Text = totCost.ToString("F2");
            this.neuSpread1_Sheet1.Cells[rowIndex, 5].Text = totPub.ToString("F2");
            this.neuSpread1_Sheet1.Cells[rowIndex, 6].Text = totOwn.ToString("F2");
        }

        /// <summary>
        /// 双击查询发票明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neuSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (this.neuSpread1.ActiveSheet == this.neuSpread1_Sheet1)
            {
                this.neuSpread1_Sheet2.Rows.Count = 0;//先清空

                if (e.Row > (this.neuSpread1_Sheet1.Rows.Count - 1))
                {
                    return;
                }

                if (this.neuSpread1_Sheet1.ActiveRow.Tag == null)
                {
                    return;
                }

                string invoiceNO = this.neuSpread1_Sheet1.ActiveRow.Tag.ToString();
                DataSet dsResult = new DataSet();
                int returnValue = this.feeMgr.QueryOutPatientInvoiceDetailByInvoiceNo(invoiceNO, ref dsResult);
                if (returnValue == -1)
                {
                    MessageBox.Show(this.feeMgr.Err);
                    return;
                }

                DataTable dt = dsResult.Tables[0];
                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("该发票没有发票费用信息!");
                    return;
                }

                decimal totCost = 0m;
                int rowIndex = this.neuSpread1_Sheet2.Rows.Count;

                foreach (DataRow dr in dt.Rows)
                {
                    this.neuSpread1_Sheet2.Rows.Add(rowIndex, 1);

                    this.neuSpread1_Sheet2.Cells[rowIndex, 0].Text = dr[0].ToString();
                    this.neuSpread1_Sheet2.Cells[rowIndex, 1].Text = dr[1].ToString();

                    totCost += Neusoft.FrameWork.Function.NConvert.ToDecimal(dr[1].ToString());
                    
                    rowIndex++;
                }

                rowIndex = this.neuSpread1_Sheet2.Rows.Count;
                this.neuSpread1_Sheet2.Rows.Add(rowIndex, 1);

                this.neuSpread1_Sheet2.Cells[rowIndex, 0].Text = "合计:";
                this.neuSpread1_Sheet2.Cells[rowIndex, 1].Text = totCost.ToString("F2");

                this.neuSpread1.ActiveSheet = this.neuSpread1_Sheet2;

            }
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbQuery_Click(object sender, EventArgs e)
        {
            this.neuSpread1_Sheet1.Rows.Count = 0;
            this.neuSpread1_Sheet2.Rows.Count = 0;

            this.QueryPatientInvoiceInfo();

        }

        /// <summary>
        /// 退出窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neuButton1_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        /// <summary>
        /// 键盘事件
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
