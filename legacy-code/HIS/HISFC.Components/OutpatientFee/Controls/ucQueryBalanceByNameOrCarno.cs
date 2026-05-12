using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Neusoft.HISFC.Models.Fee.Outpatient;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucQueryBalanceByNameOrCarno : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucQueryBalanceByNameOrCarno()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 门诊费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        /// <summary>
        /// 费用综合业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();



        
        public event SelectRow GetSelectRow;

        DataView BalancesDV = null;

        #region 方法

        /// <summary>
        /// farpoint 初始化
        /// </summary>
        public void FPInit()
        {
            FarPoint.Win.Spread.CellType.CheckBoxCellType ckt=new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            FarPoint.Win.Spread.CellType.TextCellType tct = new FarPoint.Win.Spread.CellType.TextCellType();
      
            #region 设置fp
            this.fpSpread1_Sheet1.ColumnCount = (int)Col.MAX;
            this.fpSpread1_Sheet1.Rows.Count = 0;
            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Sel].Text = ".";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Sel].Width = 40;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.BalanceNo].Text = "电脑发票号";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.BalanceNo].Width = 120;
            this.fpSpread1_Sheet1.Columns[(int)Col.BalanceNo].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.PrintNo].Text = "发票印刷号";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.PrintNo].Width = 120;
            this.fpSpread1_Sheet1.Columns[(int)Col.PrintNo].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Cost].Text = "金额";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Cost].Width = 70;
            this.fpSpread1_Sheet1.Columns[(int)Col.Cost].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Time].Text = "日期";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Time].Width = 120;
            this.fpSpread1_Sheet1.Columns[(int)Col.Time].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Name].Text = "姓名";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Name].Width = 60;
            this.fpSpread1_Sheet1.Columns[(int)Col.Name].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Sex].Text = "性别";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Sex].Width = 50;
            this.fpSpread1_Sheet1.Columns[(int)Col.Sex].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Age].Text = "年龄";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Age].Width = 50;
            this.fpSpread1_Sheet1.Columns[(int)Col.Age].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Linkman].Text = "联系人";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Linkman].Width = 70;
            this.fpSpread1_Sheet1.Columns[(int)Col.Linkman].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Phone].Text = "联系电话";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Phone].Width = 100;
            this.fpSpread1_Sheet1.Columns[(int)Col.Phone].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Carno].Text = "就诊卡号";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Carno].Width = 100;
            this.fpSpread1_Sheet1.Columns[(int)Col.Carno].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.PactName].Text = "合同单位";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.PactName].Width = 100;
            this.fpSpread1_Sheet1.Columns[(int)Col.PactName].Locked = true;

            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, (int)Col.Seq].Text = "发票序号";
            this.fpSpread1_Sheet1.ColumnHeader.Columns[(int)Col.Seq].Width = 100;
            this.fpSpread1_Sheet1.Columns[(int)Col.Seq].Locked = true;
            //this.fpSpread1_Sheet1.Columns[(int)Col.Seq].Visible = false;
            #endregion

            this.fpSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
            this.fpSpread1_Sheet1.Columns[(int)Col.Sel].CellType = ckt;
            this.fpSpread1_Sheet1.Columns[(int)Col.BalanceNo].CellType = tct;
            this.fpSpread1_Sheet1.Columns[(int)Col.PrintNo].CellType = tct;
            this.fpSpread1_Sheet1.Columns[(int)Col.Carno].CellType = tct;
            this.fpSpread1_Sheet1.Columns[(int)Col.Seq].CellType = tct;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="nameorcarno"></param>
        public void Query(string nameorcarno)
        {
            string cardNo;
            Neusoft.HISFC.Models.Account.AccountCard accountCard = new Neusoft.HISFC.Models.Account.AccountCard();
            int ret = feeIntegrate.ValidMarkNO(nameorcarno, ref accountCard);
            if (ret > 0)
            {
                cardNo = accountCard.Patient.PID.CardNO;
            }
            else
            {
                cardNo = nameorcarno;
            }
            string sql = @"select 'false' as Sel,
                                   r.card_no as Carno,
                                   i.name as Name,
                                   fun_get_sex(r.sex_code) as Sex,
                                   fun_get_age(r.birthday) as Age,
                                   i.invoice_no as BalanceNo,
                                   i.print_invoiceno as PrintNo,
                                   i.tot_cost as Cost,
                                   i.pact_name as PactName,
                                   i.oper_date as Time,
                                   '' as Linkman,
                                   r.rela_phone as Phone
                              from fin_opb_invoiceinfo i, fin_opr_register r
                             where (i.card_no = '{0}' or i.name = '{0}')
                                  --and i.hos_code = 'CORE_HIS50'
                                  --i.oper_date >= sysdate - 1
                               and i.TRANS_TYPE = '1'
                               and i.cancel_flag = '1'
                               and r.clinic_code = i.clinic_code
                             order by Time desc
                            ";
            sql = string.Format(sql, cardNo, Neusoft.FrameWork.Management.Connection.Hospital.ID);
            DataSet ds = new DataSet();

            int result = outpatientManager.ExecQuery(sql, ref ds);
            if (result == -1)
            {
                MessageBox.Show("查询数据失败!");
                return;
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("没有需要打印发票的数据!");
                return;
            }

            BalancesDV = new DataView(ds.Tables[0]);

            AddToFP(BalancesDV);
            this.SetTopLevel(true);
            this.Show();

        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="nameorcarno"></param>
        public void QueryByDt(string nameorcarno,DateTime dt)
        {
            string cardNo;
            Neusoft.HISFC.Models.Account.AccountCard accountCard = new Neusoft.HISFC.Models.Account.AccountCard();
            int ret = feeIntegrate.ValidMarkNO(nameorcarno, ref accountCard);
            if (ret > 0)
            {
                cardNo = accountCard.Patient.PID.CardNO;
            }
            else
            {
                cardNo = nameorcarno;
            }
            string sql = @"select 'false' as Sel,
                                   r.card_no as Carno,
                                   i.name as Name,
                                   fun_get_sex(r.sex_code) as Sex,
                                   fun_get_age(r.birthday) as Age,
                                   i.invoice_no as BalanceNo,
                                   i.print_invoiceno as PrintNo,
                                   i.tot_cost as Cost,
                                   i.pact_name as PactName,
                                   i.oper_date as Time,
                                   '' as Linkman,
                                   r.rela_phone as Phone,
                                   i.INVOICE_SEQ as SEQ
                              from fin_opb_invoiceinfo i, fin_opr_register r
                             where (i.card_no = '{0}' or i.name = '{0}')
                                  --and i.hos_code = 'CORE_HIS50'
                               and i.reg_date >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                               and i.TRANS_TYPE = '1'
                               and i.cancel_flag = '1'
                               and r.clinic_code = i.clinic_code
                             order by Time desc
                            ";
            sql = string.Format(sql, cardNo,dt.ToString());
            DataSet ds = new DataSet();

            int result = outpatientManager.ExecQuery(sql, ref ds);
            if (result == -1)
            {
                MessageBox.Show("查询数据失败!");
                return;
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("没有需要打印发票的数据!");
                return;
            }

            BalancesDV = new DataView(ds.Tables[0]);

            AddToFP(BalancesDV);
            this.SetTopLevel(true);
            this.Show();

        }

        /// <summary>
        /// 添加到FP
        /// </summary>
        /// <param name="ds"></param>
        public void AddToFP(DataView ds)
        {
            this.fpSpread1_Sheet1.Reset();
            this.FPInit();
            int index = 0;
            foreach (DataRow dr in ds.ToTable().Rows)
            {
                this.fpSpread1_Sheet1.AddRows(index, 1);
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Sel].Value = Convert.ToBoolean(dr[(int)Col.Sel].ToString());
                this.fpSpread1_Sheet1.Cells[index, (int)Col.BalanceNo].Text = dr[(int)Col.BalanceNo].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.PrintNo].Text = dr[(int)Col.PrintNo].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Cost].Text = dr[(int)Col.Cost].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Time].Text = dr[(int)Col.Time].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Name].Text = dr[(int)Col.Name].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Sex].Text = dr[(int)Col.Sex].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Age].Text = dr[(int)Col.Age].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Linkman].Text = dr[(int)Col.Linkman].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Phone].Text = dr[(int)Col.Phone].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Carno].Text = dr[(int)Col.Carno].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.PactName].Text = dr[(int)Col.PactName].ToString();
                this.fpSpread1_Sheet1.Cells[index, (int)Col.Seq].Text = dr[(int)Col.Seq].ToString();
                index++;
            }
            

        }


        /// <summary>
        /// 数据过滤
        /// </summary>
        /// <param name="str"></param>
        public void RowFilter(string str)
        {
            string fliter = "";
            if (str.Length > 0)
            {
                if (neuCheckBox1.Checked)
                {
                    fliter = "(BalanceNo like '%" + str + "%') OR" + " (PrintNo like '%" + str + "%') OR" + " (Name like '%" + str + "%') OR" + " (Phone like '%" + str + "%')";

                }
                else
                {
                    fliter = "(BalanceNo like '%" + str + "%') ";
                }
                this.BalancesDV.RowFilter = fliter;
                this.AddToFP(BalancesDV);

            }
        }

        #endregion

        #region 事件
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neubtnOK_Click(object sender, EventArgs e)
        {
            int num = 0;
            int row = 0;
            for (int i = 0; i < this.fpSpread1_Sheet1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(this.fpSpread1_Sheet1.Cells[i, (int)Col.Sel].Value))
                {
                    num++;
                    row = i;
                }
            }
            if (num >= 2 || num == 0)
            {
                MessageBox.Show("请选择一条数据!");
            }
            if (num == 1)
            {
                string balances = this.fpSpread1_Sheet1.Cells[row, (int)Col.BalanceNo].Text + "-" + this.fpSpread1_Sheet1.Cells[row, (int)Col.Seq].Text;
                this.GetSelectRow(balances);
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neubtnCancle_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// 双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            for (int i = 0; i < this.fpSpread1_Sheet1.Rows.Count; i++)
            {
                if (e.Row == i)
                {
                    this.fpSpread1_Sheet1.Cells[i, (int)Col.Sel].Value = true;
                }
                else
                {
                    this.fpSpread1_Sheet1.Cells[i, (int)Col.Sel].Value = false;
                }
            }

            string balances = this.fpSpread1_Sheet1.Cells[e.Row, (int)Col.BalanceNo].Text + "-" + this.fpSpread1_Sheet1.Cells[e.Row,(int)Col.Seq].Text;
            this.GetSelectRow(balances);
            this.Hide();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.RowFilter(textBox1.Text);
        }
        #endregion

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neubtnPrint_Click(object sender, EventArgs e)
        {
            Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint iInvoicePrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(
                typeof(ucRePrintInvoiceforatm), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint;
            if (iInvoicePrint == null)
            {
                MessageBox.Show("未配置发票打印接口");
                return;
            }
            Balance currentBalance = null;
            string erro = "";
            for (int i = 0; i < this.fpSpread1_Sheet1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(this.fpSpread1_Sheet1.Cells[i, (int)Col.Sel].Value))
                {
                    string invoiceNo = this.fpSpread1_Sheet1.Cells[i, (int)Col.BalanceNo].Text;

                    #region
                    ArrayList comBalances = outpatientManager.QueryBalancesSameInvoiceCombNOByInvoiceNO(invoiceNo);
                    if (comBalances == null)
                    {

                        currentBalance = null;

                        continue;
                    }
                    if (comBalances.Count == 0)
                    {
                        currentBalance = null;
                        continue;
                    }

                    decimal totCost = 0, ownCost = 0, payCost = 0, pubCost = 0;
                    if (comBalances.Count > 1)
                    {
                        bool isSelect = false;
                        string SeqNo = "";
                        foreach (Balance balance in comBalances)
                        {
                            if (SeqNo == "")
                            {
                                SeqNo = balance.CombNO;

                                continue;
                            }
                            else
                            {
                                if (SeqNo != balance.CombNO)
                                {
                                    isSelect = true;
                                }
                            }
                        }

                        if (isSelect)
                        {
                            Neusoft.HISFC.Components.OutpatientFee.Controls.ucInvoiceSelect ucSelect = new Neusoft.HISFC.Components.OutpatientFee.Controls.ucInvoiceSelect();

                            ucSelect.Add(comBalances);

                            Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(ucSelect);

                            Neusoft.HISFC.Models.Fee.Outpatient.Balance selectInvoice = ucSelect.SelectedBalance;
                            if (selectInvoice == null || selectInvoice.Invoice.ID == null || selectInvoice.Invoice.ID == "")
                            {
                                currentBalance = null;
                                continue;
                            }

                            comBalances = outpatientManager.QueryBalancesByInvoiceSequence(selectInvoice.CombNO);
                            if (comBalances == null)
                            {
                                currentBalance = null;
                                continue;
                            }
                        }

                    }
                    #endregion

                    currentBalance = (comBalances[0] as Balance).Clone();

                    #region
                    if (currentBalance == null)
                    {
                        continue;
                    }

                    Neusoft.HISFC.Models.Registration.Register regInfo = new Neusoft.HISFC.Models.Registration.Register();
                    regInfo.PID.CardNO = currentBalance.Patient.PID.CardNO;
                    regInfo.Pact = currentBalance.Patient.Pact.Clone();
                    regInfo.Name = currentBalance.Patient.Name;
                    regInfo.SSN = currentBalance.Patient.SSN;
                    regInfo.DoctorInfo.SeeDate = ((Neusoft.HISFC.Models.Registration.Register)currentBalance.Patient).DoctorInfo.SeeDate;
                    regInfo.PID.ID = currentBalance.Patient.ID;
                    regInfo.ID = currentBalance.Patient.ID;

                    string invoiceSeq = currentBalance.CombNO;

                    ArrayList invoiceDetails = outpatientManager.QueryBalanceListsByInvoiceSequence(invoiceSeq);
                    if (invoiceDetails == null || invoiceDetails.Count <= 0)
                    {
                        erro += invoiceSeq + "获取发票明细信息失败,";
                        continue;
                    }

                    ArrayList payModes = outpatientManager.QueryBalancePaysByInvoiceSequence(invoiceSeq);
                    if (payModes == null || payModes.Count <= 0)
                    {
                        erro += invoiceSeq + "获取支付方式信息失败,";
                        continue;
                    }

                    ArrayList feeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(invoiceSeq);
                    if (feeItemLists == null || feeItemLists.Count <= 0)
                    {
                        erro += invoiceSeq + "获取费用明细信息失败,";
                        continue;
                    }

                    //currentBalance.PrintTime = outpatientManager.GetDateTimeFromSysDateTime();
                    
                    iInvoicePrint.SetPrintValue(regInfo, currentBalance, invoiceDetails, feeItemLists, payModes, true);
                    iInvoicePrint.Print();
                    #endregion
                }
            }
            this.Hide();
        }
    }

    public enum Col
    {
        /// <summary>
        /// 选中
        /// </summary>
        Sel,
        /// <summary>
        /// 就诊卡号
        /// </summary>
        Carno,
        /// <summary>
        /// 姓名
        /// </summary>
        Name,
        /// <summary>
        /// 性别
        /// </summary>
        Sex,
        /// <summary>
        /// 年龄
        /// </summary>
        Age,
        /// <summary>
        /// 发票电脑号
        /// </summary>
        BalanceNo,
        /// <summary>
        /// 发票印刷号
        /// </summary>
        PrintNo,
        /// <summary>
        /// 金额
        /// </summary>
        Cost,
        /// <summary>
        /// 合同单位
        /// </summary>
        PactName,
        /// <summary>
        /// 日期
        /// </summary>
        Time,
        /// <summary>
        /// 联系人
        /// </summary>
        Linkman,
        /// <summary>
        /// 联系电话
        /// </summary>
        Phone,
        /// <summary>
        /// 发票序号
        /// </summary>
        Seq,
        MAX
        
    }

    public delegate void SelectRow(string BalacesNo);
}
