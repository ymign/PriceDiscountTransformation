using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using Neusoft.HISFC.Models.Account;

namespace Neusoft.HISFC.Components.Registration
{
    /// <summary>
    /// 换开纸质票
    /// </summary>
    public partial class frmFeeChangeReturnPaperTicketReprint : Form
    {
        public frmFeeChangeReturnPaperTicketReprint()
        {
            InitializeComponent();

            this.Init();
        }

        public frmFeeChangeReturnPaperTicketReprint(Neusoft.HISFC.Models.Registration.Register reg)
        {
            InitializeComponent();

            obj = reg;
            Init();
        }

        #region 变量

        /// <summary>
        /// 控制管理类
        /// </summary>
        private Neusoft.FrameWork.Management.ControlParam ctlMgr = new Neusoft.FrameWork.Management.ControlParam();

        /// <summary>
        /// 帐户管理
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.Account accMgr = new Neusoft.HISFC.BizLogic.Fee.Account();

        Neusoft.HISFC.BizProcess.Integrate.RADT radt = new Neusoft.HISFC.BizProcess.Integrate.RADT();

        /// <summary>
        /// 当前挂号实体
        /// </summary>
        private Neusoft.HISFC.Models.Registration.Register obj;

        /// <summary>
        /// 挂号实体
        /// </summary>
        public Neusoft.HISFC.Models.Registration.Register Register
        {
            get
            {
                return this.obj;
            }
        }

        /// <summary>
        /// 挂号管理类
        /// </summary>
        private Neusoft.HISFC.BizLogic.Registration.Register regMgr = new Neusoft.HISFC.BizLogic.Registration.Register();



        UseElecBillService.UseElecBillOutPatient useElecBill = new UseElecBillService.UseElecBillOutPatient();

        public Neusoft.HISFC.Models.Base.Const elecobj = null;
        public string strRealInvoiceNO = "";
        /// <summary>
        /// 可退号天数
        /// </summary>
        private int PermitDays = 1;
        /// <summary>
        /// 允许补打挂号票天数
        /// </summary>
        private int printDays = 0;
        #endregion
        #region 业务层
        /// <summary>
        /// 门诊费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        /// <summary>
        /// 控制参数业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        /// <summary>
        /// 管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        /// <summary>
        /// 费用综合业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        /// <summary>
        /// 药品业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Pharmacy pharmacyIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Pharmacy();

        /// <summary>
        /// 挂号管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Registration.Registration registerManager = new Neusoft.HISFC.BizProcess.Integrate.Registration.Registration();

        protected Neusoft.HISFC.BizLogic.Fee.Item itemMgr = new Neusoft.HISFC.BizLogic.Fee.Item();
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this.txtCardNo.KeyDown += new KeyEventHandler(txtCardNo_KeyDown);
            this.button1.Click += new EventHandler(button1_Click);
            this.button2.Click += new EventHandler(button2_Click);
            //this.neuButton1.Click += new EventHandler(btnPrintBarCode_Click);
            this.label7.Visible = false;//一开始将提示隐藏
            this.txtCardNo.Select();
            this.txtCardNo.Focus();
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
            this.txtmedicaltype.Text = obj.medicaltype.ToString();
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
            //赋值电子票信息
            if (this.regMgr.QueryElecDataForId(obj.ID + obj.UserCode, "2", ref model) == 1)
            {
                this.txtbillBatchCode.Text = model.billBatchCode;
                this.txtbillNo.Text = model.billNo;
                this.txtrandom.Text = model.random;
                this.txtcreateTime.Text = model.createTime;

                //查询上次换开的纸质票人员信息
                Neusoft.HISFC.BizLogic.Registration.Register regMgrpaper = new Neusoft.HISFC.BizLogic.Registration.Register();
                Neusoft.HISFC.Models.ElecBill.Elec_OutPatientPaperBill paperModel = new
                     Neusoft.HISFC.Models.ElecBill.Elec_OutPatientPaperBill();
                if (regMgrpaper.QueryElecPaperDataForId(obj.ID + obj.UserCode, "2", ref paperModel) < 0)
                {
                    MessageBox.Show(regMgrpaper.Err);
                }
                this.txtReturnCount.Text = paperModel.lastmodifycode.ToString();
                this.txtLastRetunName.Text = paperModel.createName.ToString();
                this.txtLastRetunTime.Text = paperModel.createTime.ToString();
                #region 1.再去获取系统当前操作员的纸质票信息
                string strInvioceNO = "";

                string strErrText = "";
                int iRes = 0;
                string strInvoiceType = "C";   //挂号收据
                Neusoft.HISFC.Models.Base.Employee employee = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;
                Neusoft.HISFC.BizProcess.Integrate.Fee feeMgr = new Neusoft.HISFC.BizProcess.Integrate.Fee();
                iRes = feeMgr.GetInvoiceNOWithHosCode(employee, strInvoiceType, ref strInvioceNO, ref strRealInvoiceNO, Neusoft.FrameWork.Management.Connection.Hospital.ID, ref strErrText);
                string strRealNo = string.Empty;
                if (iRes == -1)
                {
                    MessageBox.Show("获取当前操作员实际发票号出错!");

                }
                else
                {
                    strRealNo = System.Text.RegularExpressions.Regex.Replace(strRealInvoiceNO, "[a-zA-Z]+", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    this.txtHisPaperNo.Text = strRealNo;
                }
                #endregion

                //2.获取电子票据平台纸质票号码  
                string pBillNo = string.Empty;
                string pBillBatchCode = string.Empty;
                if (useElecBill.GetPaperBillNo("1", employee.ID,this.txtHisPaperNo.Text , ref pBillNo, ref pBillBatchCode) < 0)
                {
                    MessageBox.Show(this.useElecBill.message);
                }
                //this.txtpBillNo.Text = pBillNo;
                this.txtpBillBatchCode.Text = pBillBatchCode;

            }
            else
            {
                MessageBox.Show(this.regMgr.Err);
            }
            //    //2.获取电子票据平台纸质票号码  
            //    string pBillNo = string.Empty;
            //    string pBillBatchCode = string.Empty;
            //    if (useElecBill.GetPaperBillNo("0", employee.ID, ref pBillNo, ref pBillBatchCode) < 0)
            //    {
            //        MessageBox.Show(this.useElecBill.message);
            //    }
            //    this.txtpBillNo.Text = pBillNo;
            //    this.txtpBillBatchCode.Text = pBillBatchCode;
            //    //若是his系统纸质票实际号码和平台不一致 则提示收费员
            //    if (!string.IsNullOrEmpty(strRealNo) && !string.IsNullOrEmpty(pBillNo))
            //    {

            //        if (strRealNo != pBillNo)
            //        {
            //            this.label7.Visible = true;

            //        }
            //    }

            //}
            //
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



        private void txtCardNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            ArrayList tal = new ArrayList();

            string cardNo;
            Neusoft.HISFC.Models.Account.AccountCard accountCard = new Neusoft.HISFC.Models.Account.AccountCard();
            int ret = feeIntegrate.ValidMarkNO(txtCardNo.Text, ref accountCard);
            if (ret > 0)
            {
                cardNo = accountCard.Patient.PID.CardNO;
                txtCardNo.Text = cardNo;
            }
            else
            {
                cardNo = txtCardNo.Text;
            }

            string sql = @"select i.invoice_no,fun_get_employee_name(i.oper_code) as name, i.tot_cost, i.oper_date, i.name,i.clinic_code,decode(pact_name,'现金',( SELECT (case when substr(INSUPLCADMDVS,1,4)='4404'  then '珠海'when  substr(INSUPLCADMDVS,1,2)='44' 
  and  substr(INSUPLCADMDVS,1,4)<>'4404' then '省内异地' else '跨省异地' end) || (select name from  com_dictionary dic where TYPE='Insutype' and dic.code= gd.insutype)
  from  fin_ipr_siinmaininfo_gd gd where idenno in (select idenno from  fin_opr_register reg  where reg.card_no=i.card_no) and VALID_FLAG='1'   and INSUPLCADMDVS is not null  and rownum=1)
         ,( SELECT (case when substr(INSUPLCADMDVS,1,4)='4404'  then '珠海'when  substr(INSUPLCADMDVS,1,2)='44' 
        and  substr(INSUPLCADMDVS,1,4)<>'4404' then '省内异地' else '跨省异地' end) || (select name from  com_dictionary dic where TYPE='Insutype' and dic.code= gd.insutype)
         from  fin_ipr_siinmaininfo_gd gd where (INPATIENT_NO=i.CLINIC_CODE  )  and VALID_FLAG='1'     and INSUPLCADMDVS is not null and rownum=1)  )medicaltype
                            from fin_opb_invoiceinfo i
                           where (i.card_no = '{0}' or i.name = '{0}')
                            --and i.hos_code = '{1}'
                             --and i.oper_date >= sysdate - 30
                             and i.TRANS_TYPE = '1'
                             and i.cancel_flag = '1'
                           order by oper_date desc";
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

            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                Neusoft.HISFC.Models.Base.Const c = new Neusoft.HISFC.Models.Base.Const();
                c.ID = c.ID = dr[0].ToString();
                c.Name = dr[1].ToString();
                c.Memo = dr[2].ToString();
                c.SpellCode = dr[3].ToString();
                c.WBCode = dr[4].ToString();
                c.UserCode = dr[5].ToString();
                c.medicaltype = dr[6].ToString();
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
                string[] cols = { "发票电脑号", "收费人", "金额", "日期", "姓名", "流水号", "医保类型" };
                bool[] visibles = { true, true, true, true, true, false };
                int[] widths = { 120, 80, 80, 80, 60, 60, 120 };
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

        private void Clear()
        {
            this.obj = null;

            this.txtCardNo.Text = "";


        }



        private void button1_Click(object sender, EventArgs e)
        {

            this.DialogResult = DialogResult.OK;
            this.Close();
        }



        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <returns></returns>
        private int valid()
        {

            return 0;

        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.No ;
            this.Close();
        }
    }
}