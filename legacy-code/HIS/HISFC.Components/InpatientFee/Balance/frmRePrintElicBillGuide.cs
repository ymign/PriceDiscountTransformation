using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;
using System.Collections;

namespace Neusoft.HISFC.Components.InpatientFee.Balance
{
    public partial class frmRePrintElicBillGuide : Form
    {
        public frmRePrintElicBillGuide()
        {
            InitializeComponent();
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK ;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.No ;
            this.Close();
        }

        private void ucQueryInpatientNo_myEvent()
        {
            if (this.ucQueryInpatientNo.InpatientNo == null || this.ucQueryInpatientNo.InpatientNo == "")
            {
                CommonController.CreateInstance().MessageBox("此住院号不存在请重新输入！", MessageBoxIcon.Warning);
                this.ucQueryInpatientNo.Focus();
                return;
            }

            this.QueryByInPatientNO(this.ucQueryInpatientNo.InpatientNo, null);
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            Neusoft.HISFC.Models.ElecBill.Elec_OutPatientRecord elecRecordModel = new Neusoft.HISFC.Models.ElecBill.Elec_OutPatientRecord();
            if (inpatientFeeManager.QueryElecDataForId(elecobj.ID + elecobj.UserCode, "3", ref elecRecordModel) < 0)
            {
                MessageBox.Show(inpatientFeeManager.Err);
                return;
            }
            else
            {
                string Url = elecRecordModel.pictureUrl;
                try
                {
                    System.Diagnostics.Process.Start("chrome.exe", Url);
                    // System.Diagnostics.Process.Start("FireFox.exe", string.Format(this.ViewUrl, this.ucOutPatientOrder1.Patient.PID.CardNO, "1"));
                }
                catch
                {
                    try
                    {
                        System.Diagnostics.Process.Start(Url);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("打开浏览器出错！"+ex.Message);
                        return;
                    }

                }
            }
        }

    }
}
