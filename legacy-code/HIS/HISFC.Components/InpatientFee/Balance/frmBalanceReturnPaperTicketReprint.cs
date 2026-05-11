using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;

namespace Neusoft.HISFC.Components.InpatientFee.Balance
{
    public partial class frmBalanceReturnPaperTicketReprint : Form
    {
        public frmBalanceReturnPaperTicketReprint()
        {
            InitializeComponent();
        }

        public enum EnumCol
        {
            选择,
            住院号,
            姓名,
            收据号,
            收据编号,
            收费时间, 住院起止日期, 结账方式, 应收金额, 预收金额, 实收金额, 欠费金额, 药费限额, 付款方式, 结账员, 作废员工, 作废时间, 日结时间
        }

        #region 变量
        Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();
        Neusoft.HISFC.BizLogic.Fee.InPatient inpatientFeeManager = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        Neusoft.SOC.HISFC.InpatientFee.BizProcess.RADT radtManager = new Neusoft.SOC.HISFC.InpatientFee.BizProcess.RADT();
        Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy printer = null;
        Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = null;
        UseElecBillService.UseElecBillInPatient useElecBill = new UseElecBillService.UseElecBillInPatient();

        public Neusoft.HISFC.Models.Base.Const elecobj = null;
        public string strRealInvoiceNO = "";
        #endregion

        private void ucQueryInpatientNo1_myEvent()
        {
            if (this.ucQueryInpatientNo1.InpatientNo == null || this.ucQueryInpatientNo1.InpatientNo == "")
            {
                CommonController.CreateInstance().MessageBox("此住院号不存在请重新输入！", MessageBoxIcon.Warning);
                this.ucQueryInpatientNo1.Focus();
                return;
            }

            this.QueryByInPatientNO(this.ucQueryInpatientNo1.InpatientNo, null);
        }

        /// <summary>
        /// 住院号回车处理
        /// </summary>
        private void QueryByInPatientNO(string inpatientNO, string invoiceNO)
        {
            //通过住院号获取住院基本信息
            this.patientInfo = this.radtManager.GetPatientInfo(inpatientNO);
            if (this.patientInfo == null)
            {
                CommonController.CreateInstance().MessageBox("查找患者信息失败，" + radtManager.Err, MessageBoxIcon.Warning);
                return;
            }

            ArrayList alAllBill = this.inpatientFeeManager.QueryBalancesByInpatientNO(this.patientInfo.ID, "ALL");//出院结算发票。
            if (alAllBill == null)
            {
                CommonController.CreateInstance().MessageBox("获取发票号出错，" + this.inpatientFeeManager.Err, MessageBoxIcon.Warning);
                return;
            }
            if (alAllBill.Count < 1)
            {
                CommonController.CreateInstance().MessageBox("该患者没有已结算的发票,请通过发票号查询!", MessageBoxIcon.Warning);
                return;
            }
            ArrayList tal = new ArrayList();
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.Balance balance in alAllBill)
            {

                Neusoft.HISFC.Models.Base.Const c = new Neusoft.HISFC.Models.Base.Const();
                c.ID = balance.Invoice.ID;
                c.Name = CommonController.CreateInstance().GetEmployeeName(balance.BalanceOper.ID);
                c.Memo = balance.FT.TotCost.ToString();
                c.SpellCode = balance.BalanceOper.OperTime.ToString();
                c.WBCode = balance.Patient.Name;
                c.UserCode = balance.Patient.ID;
                tal.Add(c);
            }


            if (tal.Count == 0)
            {
                MessageBox.Show("没有需要换开的数据!");
                return;
            }
            else
            {
                FrameWork.WinForms.Forms.frmEasyChoose fc = new Neusoft.FrameWork.WinForms.Forms.frmEasyChoose(tal);
                string[] cols = { "发票电脑号", "收费人", "金额", "日期", "姓名", "流水号" };
                bool[] visibles = { true, true, true, true, true, false };
                int[] widths = { 120, 80, 80, 80, 60, 60 };
                fc.SetFormat(cols, visibles, widths);
                fc.ShowDialog();

                elecobj = fc.Object as Neusoft.HISFC.Models.Base.Const;
                if (elecobj == null)
                {
                    MessageBox.Show("请选择一条记录!");
                    return;
                }
                //界面赋值
                SetValue(elecobj);


            }
        }

        /// <summary>
        /// 界面赋值
        /// </summary>
        /// <param name="reg"></param>
        private void SetValue(Neusoft.HISFC.Models.Base.Const obj)
        {
            this.txtInvoiceNo.Text = obj.ID.ToString();
            this.txtName.Text = obj.WBCode.ToString();
            this.txtFeeName.Text = obj.Name.ToString();
            this.txtTo_Cost.Text = obj.Memo.ToString();
            this.txtFeedate.Text = obj.SpellCode.ToString();
            SetElecValue(obj);
            //if (reg == null)
            //{
            //    reg = new Neusoft.HISFC.Models.Registration.Register();
            //}
            //if (reg.LstCardFee.Count > 0)
            //{
            //    this.txtRecipeNo.Text = (reg.LstCardFee[0] as Neusoft.HISFC.Models.Account.AccountCardFee).InvoiceNo;
            //}
            //else
            //{
            //    this.txtRecipeNo.Text = reg.RecipeNO;
            //}
            //this.txtSeeNo.Text = reg.OrderNO.ToString();
            //this.txtName.Text = reg.Name;
            //this.cmbSex.Tag = reg.Sex.ID;
            //this.dtBirthday.Value = reg.Birthday;
            //this.txtPhone.Text = reg.PhoneHome;
            //this.txtAdress.Text = reg.AddressHome;
            //SetElecValue(reg);
        }

        /// <summary>
        /// 电子票信息赋值
        /// </summary>
        private void SetElecValue(Neusoft.HISFC.Models.Base.Const obj)
        {
            Neusoft.HISFC.Models.ElecBill.Elec_OutPatientRecord model = new Neusoft.HISFC.Models.ElecBill.Elec_OutPatientRecord();
            string operId = Neusoft.FrameWork.Management.Connection.Operator.ID.ToString();
            //查询上次换开的纸质票人员信息
            Neusoft.HISFC.Models.ElecBill.Elec_OutPatientPaperBill paperModel = new
            Neusoft.HISFC.Models.ElecBill.Elec_OutPatientPaperBill();
            if (inpatientFeeManager.QueryElecPaperDataForId(obj.ID + obj.UserCode, "3", ref paperModel) < 0)
            {
                MessageBox.Show(inpatientFeeManager.Err);
            }
            this.txtReturnCount.Text = paperModel.lastmodifycode.ToString();
            this.txtLastRetunName.Text = paperModel.createName.ToString();
            this.txtLastRetunTime.Text = paperModel.createTime.ToString();
            //赋值电子票信息
            if (this.inpatientFeeManager.QueryElecDataForId(obj.ID + obj.UserCode, "3", ref model) == 1)
            {
                this.txtbillBatchCode.Text = model.billBatchCode;
                this.txtbillNo.Text = model.billNo;
                this.txtrandom.Text = model.random;
                this.txtcreateTime.Text = model.createTime;
                
                #region 1.再去获取系统当前操作员的纸质票信息
                string strInvioceNO = "";


                Neusoft.HISFC.Models.Base.Employee employee = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;
                strInvioceNO = feeIntegrate.GetNextInvoiceNO("I", employee);
                if (string.IsNullOrEmpty(strInvioceNO))
                {
                    MessageBox.Show("获取当前操作员实际发票号出错!");
                    return;
                }
                else
                {
                    this.txtHisPaperNo.Text = strInvioceNO;
                }
                #endregion

                //2.获取电子票据平台纸质票号码  
                string pBillNo = string.Empty;
                string pBillBatchCode = string.Empty;
                if (useElecBill.GetPaperBillNo("1", employee.ID, this.txtHisPaperNo.Text, ref pBillNo, ref pBillBatchCode) < 0)
                {
                    MessageBox.Show(this.useElecBill.message);
                }
                //this.txtpBillNo.Text = pBillNo;
                this.txtpBillBatchCode.Text = pBillBatchCode;

            }
            else
            {
                MessageBox.Show(this.inpatientFeeManager.Err);
            }
        }

        /// <summary>
        /// 截取发票数字部分
        /// </summary>
        /// <param name="strBgnNo"></param>
        /// <returns></returns>
        static private string bgnNoSubNumber(string strBgnNo)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(strBgnNo.Substring(0, 1), @"[^1-9]"))
            {
                strBgnNo = strBgnNo.Remove(0, 1);
                strBgnNo = bgnNoSubNumber(strBgnNo);
            }
            return strBgnNo;
        }

        /// <summary>
        /// 根据界面赋值的信息获取换开纸质票所需的数据
        /// </summary>
        /// <returns></returns>
        public Neusoft.HISFC.Models.ElecBill.Elec_OutPatientPaperBill GetElecPaperData()
        {

            Neusoft.HISFC.Models.ElecBill.Elec_OutPatientPaperBill model = new Neusoft.HISFC.Models.ElecBill.Elec_OutPatientPaperBill();
            model.billBatchCode = this.txtbillBatchCode.Text;
            model.billNo = this.txtbillNo.Text;
            model.pBillNo = this.txtHisPaperNo.Text;
            model.pBillBatchCode = this.txtpBillBatchCode.Text;
            return model;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
