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
using Neusoft.FrameWork.Management;
using Neusoft.HISFC.BizProcess.Integrate;
using Neusoft.HISFC.BizProcess.Interface.Fee;
using Neusoft.HISFC.Models.RADT;
using Neusoft.HISFC.Models.Registration;
using Neusoft.HISFC.Components.Common.Forms;
using Neusoft.HISFC.BizProcess.Interface.Account;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    /// <summary>
    /// 发票重打
    /// </summary>
    public partial class ucRePrintInvoiceforatmgh : Neusoft.FrameWork.WinForms.Controls.ucBaseControl, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer, IReprintInvoice
    {
        #region 成员
        /// <summary>
        /// 重打发票是否走号
        /// </summary>
        bool isRollCode;
        /// <summary>
        /// 工具条
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();
        /// <summary>
        /// 当前收费发票
        /// </summary>
        protected Neusoft.HISFC.Models.Fee.Outpatient.Balance currentBalance = new Neusoft.HISFC.Models.Fee.Outpatient.Balance();
        /// <summary>
        /// 费用明细信息
        /// </summary>
        protected ArrayList comFeeItemLists = new ArrayList();
        /// <summary>
        /// 发票明细信息
        /// </summary>
        protected ArrayList comBalanceLists = new ArrayList();
        /// <summary>
        /// 结算信息
        /// </summary>
        ArrayList comBalances = new ArrayList();
        /// <summary>
        /// 支付信息
        /// </summary>
        ArrayList comBalancePays = new ArrayList();

        /// <summary>
        /// 收费确认单打印实体
        /// </summary>
        private IAccountFeeDetailPrint Iprint = null;
        #endregion

        #region IInterfaceContainer 成员

        public Type[] InterfaceTypes
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ucRePrintInvoiceforatmgh()
        {
            InitializeComponent();
        }

        private void ucRePrintInvoice_Load(object sender, EventArgs e)
        {
            if (!blnShowQeruyInfo)
            {
                this.ucTrvInvoice.UcClose();
            }

            //获取重打发票是否走号
            this.isRollCode = this.controlParamIntegrate.GetControlParam<bool>("MZ2010", true, true);

            this.ucTrvInvoice.evnCardNoFind += new CardNoFind(ucTrvInvoice_evnCardNoFind);
            this.ucTrvInvoice.evnInvoiceNoFind += new InvoiceNoFind(ucTrvInvoice_evnInvoiceNoFind);
            this.ucTrvInvoice.evnInvoiceSelectChange += new InvoiceNodeSelectChange(ucTrvInvoice_evnInvoiceSelectChange);

            // {C075EA48-EFC2-4de0-A0F2-BFD3F119A85F}
            GetNextInvoiceNO();
        }

        void ucTrvInvoice_evnInvoiceSelectChange(object sender, Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice)
        {
            this.tbInvoiceNo.Text = invoice.Invoice.ID;

            tbInvoiceNo_KeyDown(null, new KeyEventArgs(Keys.Enter));
        }

        void ucTrvInvoice_evnInvoiceNoFind()
        {
            this.Clear();
        }

        void ucTrvInvoice_evnCardNoFind()
        {
            this.Clear();
        }
        #endregion

        #region 属性

        bool blnShowQeruyInfo = true;

        [Category("控件设置"), Description("是否显示左边查询树")]
        public bool BlnShowQueryInfo
        {
            get { return blnShowQeruyInfo; }
            set { blnShowQeruyInfo = value; }
        }

        private bool isAutoPrintGuide = false;
        [Category("控件设置"), Description("是否自动打印费用清单")]
        public bool IsAutoPrintGuide
        {
            get
            {
                return isAutoPrintGuide;
            }
            set
            {
                isAutoPrintGuide = value;
            }
        }


        private bool isCopyRecipes = true;
        [Category("控件设置"), Description("作废发票时，是否自动复制处方")]
        public bool IsCopyRecipes
        {
            get
            {
                return isCopyRecipes;
            }
            set
            {
                isCopyRecipes = value;
            }
        }

        private bool isShowInvoiceErr = true;
        [Category("控件设置"), Description("发票错误是否显示")]
        public bool IsShowInvoiceErr
        {
            get { return isShowInvoiceErr; }
            set { isShowInvoiceErr = value; }
        }

        #endregion


        /// <summary>
        /// 设置toolBar按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override Neusoft.FrameWork.WinForms.Forms.ToolBarService Init(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("汇总打印", "对预交金发票进行汇总，然后打印！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.H合并, true, false, null);
            toolBarService.AddToolButton("补打", "补打发票，不走号！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.C重打, true, false, null);
            toolBarService.AddToolButton("发票补打", "补打发票，不走号！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.C重打, true, false, null);
            toolBarService.AddToolButton("重打", "重打发票，作废原发票，产生新的发票号！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印, true, false, null);
            toolBarService.AddToolButton("发票重打", "重打发票，作废原发票，产生新的发票号！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印, true, false, null);
            toolBarService.AddToolButton("自助设备补打", "补打自助设备产生的发票，走号，仅补打一次！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印, true, false, null);
            toolBarService.AddToolButton("更新发票号", "更新下一发票号", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.F分票, true, false, null);
            toolBarService.AddToolButton("补打指引单", "补打指引单", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印清单, true, false, null);
            toolBarService.AddToolButton("清单补打", "清单补打", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印清单, true, false, null);
            toolBarService.AddToolButton("查看记账单", "查看公费记账单", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印清单, true, false, null);

            toolBarService.AddToolButton("发票作废", "发票作废", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.C重打, true, false, null);
            toolBarService.AddToolButton("刷卡", "刷卡", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印清单, true, false, null);

            toolBarService.AddToolButton("打印发票副本", "打印发票副本", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.C重打, true, false, null);

            toolBarService.AddToolButton("打印收费确认单", "打印收费确认单", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印清单, true, false, null);
            toolBarService.AddToolButton("换开纸质票", "换开纸质票", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印, true, false, null);
            toolBarService.AddToolButton("打印医保结算清单", "打印医保结算清单", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印清单, true, false, null);
            return toolBarService;
        }
        /// <summary>
        /// 按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "换开纸质票":
                    break;
                case "汇总打印":
                    SummaryAndPrintInvoice();
                    this.ucTrvInvoice.RefurbishInvoice();
                    GetNextInvoiceNOWithHosCode();
                    //GetNextInvoiceNO();
                    break;
                case "重打":
                case "发票重打":
                    if (MessageBox.Show("是否要重打发票?\r\n作废原发票，产生新的发票号！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        this.Print(2);
                        this.ucTrvInvoice.RefurbishInvoice();
                        //GetNextInvoiceNO();
                        GetNextInvoiceNOWithHosCode();
                    }
                    break;

                case "更新发票号":
                    // {C075EA48-EFC2-4de0-A0F2-BFD3F119A85F}
                    frmUpdateInvoice frmUpdate = new frmUpdateInvoice();
                    frmUpdate.ShowDialog();
                    GetNextInvoiceNOWithHosCode();
                    //GetNextInvoiceNO();
                    break;

                case "补打":
                case "发票补打":
                    // {565A0ED8-A071-443f-AFEA-7835D78A52BF}
                    if (MessageBox.Show("是否要补打发票?\r\n补打发票，不走号！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        this.Print(1);
                        //GetNextInvoiceNO();
                        GetNextInvoiceNOWithHosCode();
                    }
                    break;

                case "补打指引单":
                case "清单补打":

                    PrintOutPatientGuide();
                    break;

                case "查看记账单":
                    this.PrintOutPatientMZJZD();
                    break;
                case "发票作废":
                    this.CancelInvoicePrint();
                    break;
                case "打印发票副本":
                    this.PrintInvoiceCopy();
                    break;
                case "自助设备补打":
                    if (MessageBox.Show("是否要补打自助设备发票?\r\n走当前号，每发票仅能补打一次！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        this.PrintZZSB();
                        GetNextInvoiceNO();
                    }
                    break;
                case "打印收费确认单":
                    this.printAcountFeeDetail();
                    break;
                case "打印医保结算清单":
                    this.PrintMedicalBanlance();
                    break;
                default:
                    break;
            }
            base.ToolStrip_ItemClicked(sender, e);
        }

        private void Research()
        {
            string cardNo = "";
            string error = "";
            if (Function.OperCard(ref cardNo, ref error) == -1)
            {
                MessageBox.Show(error);
                return;
            }
            this.ucTrvInvoice.MCardNo = cardNo;
            //txtcard_no.Text = cardNo;
            //txtcard_no_KeyDown(null,new KeyEventArgs(Keys.Enter));


        }

        /// <summary>
        /// 获取下一打印发票号
        /// {C075EA48-EFC2-4de0-A0F2-BFD3F119A85F}
        /// </summary>
        private void GetNextInvoiceNO()
        {
            lblNextInvoiceNO.Text = "";
            Neusoft.HISFC.Models.Base.Employee oper = outpatientManager.Operator as Neusoft.HISFC.Models.Base.Employee;
            string invoiceNO = feeIntegrate.GetNextInvoiceNO("C", oper);
            if (string.IsNullOrEmpty(invoiceNO))
            {
                if (this.isShowInvoiceErr)
                {
                    MessageBox.Show(feeIntegrate.Err);
                }
                return;
            }

            lblNextInvoiceNO.Text = "打印号： " + invoiceNO;
        }

        /// <summary>
        /// 获取下一打印发票号
        /// {C075EA48-EFC2-4de0-A0F2-BFD3F119A85F}
        /// </summary>
        private void GetNextInvoiceNOWithHosCode()
        {
            lblNextInvoiceNO.Text = "";
            Neusoft.HISFC.Models.Base.Employee oper = outpatientManager.Operator as Neusoft.HISFC.Models.Base.Employee;
            string invoiceNO = feeIntegrate.GetNextInvoiceNOWithHosCode("C", oper, Neusoft.FrameWork.Management.Connection.Hospital.ID);
            if (string.IsNullOrEmpty(invoiceNO))
            {
                if (this.isShowInvoiceErr)
                {
                    MessageBox.Show(feeIntegrate.Err);
                }
                return;
            }

            lblNextInvoiceNO.Text = "打印号： " + invoiceNO;
        }

        private void tbInvoiceNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string strInvoice = this.tbInvoiceNo.Text.Trim();
                if (string.IsNullOrEmpty(strInvoice))
                    return;

                string errMsg = "";
                QueryBalances(strInvoice, out errMsg);
                if (!string.IsNullOrEmpty(errMsg))
                {
                    MessageBox.Show(errMsg);
                }
            }
        }

        /// <summary>
        /// 获得发票信息
        /// </summary>
        protected void QueryBalances(string invoiceNo, out string errMsg)
        {
            errMsg = null;
            this.Clear();
            //invoiceNo = invoiceNo.PadLeft(12, '0');
            comBalances = outpatientManager.QueryBalancesSameInvoiceCombNOByInvoiceNOGH(invoiceNo);
            if (comBalances == null)
            {
                errMsg = "获得发票信息出错!" + outpatientManager.Err;
                currentBalance = null;

                return;
            }
            if (comBalances.Count == 0)
            {
                errMsg = "您输入的发票号码不存在,请查证再输入";
                currentBalance = null;
                this.tbInvoiceNo.SelectAll();
                this.tbInvoiceNo.Focus();

                return;
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
                        errMsg = "您没有选择发票，请重新输入选择!";
                        currentBalance = null;
                        this.tbInvoiceNo.SelectAll();
                        this.tbInvoiceNo.Focus();

                        return;
                    }

                    comBalances = outpatientManager.QueryBalancesByInvoiceSequence(selectInvoice.CombNO);
                    if (comBalances == null)
                    {
                        errMsg = "获得发票信息出错!" + outpatientManager.Err;
                        currentBalance = null;
                        this.tbInvoiceNo.SelectAll();
                        this.tbInvoiceNo.Focus();

                        return;
                    }
                }
                string tempInvoiceNO = "";
                foreach (Balance balance in comBalances)
                {
                    tempInvoiceNO += balance.Invoice.ID + "\n";
                    totCost += balance.FT.TotCost;
                    ownCost += balance.FT.OwnCost;
                    payCost += balance.FT.PayCost;
                    pubCost += balance.FT.PubCost;
                }

                errMsg = "该发票共应" + comBalances.Count + "张!分别为: \n" + tempInvoiceNO + "\n请把以上发票都收回!";
            }
            else
            {
                string tempInvoiceNO = "";
                foreach (Balance balance in comBalances)
                {
                    tempInvoiceNO += balance.Invoice.ID + "\n";
                    totCost += balance.FT.TotCost;
                    ownCost += balance.FT.OwnCost;
                    payCost += balance.FT.PayCost;
                    pubCost += balance.FT.PubCost;
                }
            }

            currentBalance = (comBalances[0] as Balance).Clone();
            if (currentBalance.CancelType != Neusoft.HISFC.Models.Base.CancelTypes.Valid)
            {
                errMsg = "您输入的发票号码已经作废，请查证再输入";
                //MessageBox.Show("您输入的发票号码已经作废，请查证再输入");
                currentBalance = null;
                this.tbInvoiceNo.SelectAll();
                this.tbInvoiceNo.Focus();

                return;
            }

            this.tbInvoiceNo.Text = currentBalance.Invoice.ID;
            this.tbPName.Text = currentBalance.Patient.Name;

            Neusoft.HISFC.Models.Base.Employee employee = this.managerIntegrate.GetEmployeeInfo(currentBalance.BalanceOper.ID);
            if (employee == null)
            {
                errMsg = "获得当前发票操作员信息失败!" + this.managerIntegrate.Err;
                //MessageBox.Show("获得当前发票操作员信息失败!" + this.managerIntegrate.Err);
            }
            Neusoft.HISFC.Models.Registration.Register regobj = this.registerManager.GetByClinic(currentBalance.Patient.ID);

            string phone = "";
            string sql = @"select tt.work_tel from com_patientinfo tt where tt.card_no='{0}'";
            sql = string.Format(sql, regobj.PID.CardNO);
            phone = outpatientManager.ExecSqlReturnOne(sql, " ");
            //add by tangyi 20220317 添加身份证
            string sql_1 = "select p.idenno from com_patientinfo p where p.card_no = '{0}'";
            sql_1 = string.Format(sql_1, regobj.PID.CardNO);
            string IDCard = outpatientManager.ExecSqlReturnOne(sql_1);
            if (!string.IsNullOrEmpty(IDCard))
            {
                this.tbID.Text = IDCard;
            }
            this.tbOperName.Text = employee.Name;
            this.tbPactInfo.Text = currentBalance.Patient.Pact.Name;

            this.otherphone.Text = phone;
            this.tbphone.Text = regobj.PhoneHome;//regobj.PhoneBusiness;
            this.tbsex.Text = regobj.Sex.Name;
            this.tbcarno.Text = regobj.PID.CardNO;
            this.tbage.Text = outpatientManager.GetAge(regobj.Birthday, outpatientManager.GetDateTimeFromSysDateTime(), true);

            this.tbTotCost.Text = totCost.ToString();
            this.tbOwnCost.Text = ownCost.ToString();
            this.tbPayCost.Text = payCost.ToString();
            this.tbPubCost.Text = pubCost.ToString();

            this.tbInvoiceDate.Text = currentBalance.BalanceOper.OperTime.ToString();

            if (!FillBalanceLists(currentBalance.Invoice.ID))
            {
                errMsg = "获得发票明细信息出错!" + outpatientManager.Err;
                //MessageBox.Show("获得发票明细信息出错!" + outpatientManager.Err);

                return;
            }

            comFeeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(currentBalance.CombNO);
            if (comFeeItemLists == null)
            {
                errMsg = "获得患者费用明细出错!";
                //MessageBox.Show("获得患者费用明细出错!");

                return;
            }
            this.comBalances = comBalances;
        }

        /// <summary>
        /// 换开纸质票并打印
        /// </summary>
        /// <param name="printType"></param>
        /// <returns></returns>
        protected bool PrintChangePaperBill(int printType)
        {
            if (currentBalance == null)
            {
                MessageBox.Show("该发票已经作废!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();

                return false;
            }
            if (currentBalance.Invoice.ID == "")
            {
                MessageBox.Show("请输入发票信息!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();

                return false;
            }
            //if (printType == 1 && currentBalance.Invoice.ID.StartsWith("T"))
            //{
            //    MessageBox.Show("该发票为自助机发票,不需要补打发票", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            //if (printType == 2 && !currentBalance.Invoice.ID.StartsWith("T"))
            //{
            //    MessageBox.Show("发票已重打,不允许再重打!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            //
            // {48E41D0A-7397-402a-9E51-CD7B6CCAEBBE}
            // 终端产生临时发票，需要汇总打印
            //if (currentBalance.IsAccount)
            //{
            //    MessageBox.Show("终端扣费产生临时发票，请汇总打印！");
            //    return false;
            //}

            bool isCanQuitOtherOper = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.CAN_REPRINT_OTHER_OPER_INVOICE, true, false);

            if (!isCanQuitOtherOper)//不予许交叉重打
            {
                Balance tmpInvoice = comBalances[0] as Balance;

                if (tmpInvoice == null)
                {
                    MessageBox.Show("发票格式转换出错!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }

                if (tmpInvoice.BalanceOper.ID != this.outpatientManager.Operator.ID)
                {
                    MessageBox.Show("该发票为操作员" + tmpInvoice.BalanceOper.ID + "收取,您没有权限进重打!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }
            }

            bool isCanReprintDayBalance = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.CAN_REPRINT_DAYBALANCED_INVOICE, true, false);

            if (!isCanReprintDayBalance)//不予许交叉重打
            {
                Balance tmpInvoice = comBalances[0] as Balance;

                if (tmpInvoice == null)
                {
                    MessageBox.Show("发票格式转换出错!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }

                if (tmpInvoice.IsDayBalanced)
                {
                    MessageBox.Show("该发票已经日结,您没有权限进重打!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }
            }

            if (printType == 1)
            {
                // {565A0ED8-A071-443f-AFEA-7835D78A52BF}
                // 补打不走号
                string strErrText = "";
                if (this.PrintInvoiceOnly(currentBalance, out strErrText) == -1)
                {
                    MessageBox.Show(Language.Msg("补打出错! " + strErrText));
                    return false;
                }
                return true;
            }
            else if (!this.isRollCode)
            {
                // 重打不走号
                if (this.PrintNotRollCode() == -1)
                {
                    MessageBox.Show(Language.Msg("重打出错!"));
                    return false;
                }
                return true;
            }

            // 否则 走号 
            // 
            //Neusoft.FrameWork.Management.Transaction t = new Neusoft.FrameWork.Management.Transaction(Neusoft.FrameWork.Management.Connection.Instance);
            #region 增加重打分发票限制，分过的发票不得重打
            if (comBalances.Count > 1)
            {
                MessageBox.Show(Language.Msg("分发票的不允许重打，如需重打请退费后重收！"));
                Clear();
                return false;
            }

            #endregion
            //t.BeginTransaction();
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            pharmacyIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            controlParamIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            int returnValue = 0;
            string currentInvoiceNO = "";
            string currentRealInvoiceNO = "";
            string errText = "";
            DateTime nowTime = new DateTime();

            nowTime = outpatientManager.GetDateTimeFromSysDateTime();
            string invoiceType = controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.GET_INVOICE_NO_TYPE, false, "0");

            //发票
            ArrayList invoicesPrint = new ArrayList();
            //发票明细
            ArrayList invoiceDetailsPrintTemp = new ArrayList();
            //发票明细
            ArrayList invoiceDetailsPrint = new ArrayList();
            //发票费用明细
            ArrayList invoicefeeDetailsPrintTemp = new ArrayList();
            //发票费用明细
            ArrayList invoicefeeDetailsPrint = new ArrayList();
            //全部费用明细
            ArrayList feeDetailsPrint = new ArrayList();


            //获得负发票流水号
            string invoiceSeqNegative = outpatientManager.GetInvoiceCombNO();
            if (invoiceSeqNegative == null || invoiceSeqNegative == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得发票流水号失败!" + outpatientManager.Err);

                return false;
            }

            //获得正发票流水号
            string invoiceSeqPositive = outpatientManager.GetInvoiceCombNO();
            if (invoiceSeqPositive == null || invoiceSeqPositive == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得发票流水号失败!" + outpatientManager.Err);

                return false;
            }

            Hashtable hsInvoice = new Hashtable();

            Neusoft.HISFC.Models.Base.Employee employee = this.outpatientManager.Operator as Neusoft.HISFC.Models.Base.Employee;

            #region 处理发票信息
            foreach (Balance invoice in comBalances)
            {
                //returnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", ref currentInvoiceNO, ref currentRealInvoiceNO, ref errText);
                returnValue = this.feeIntegrate.GetInvoiceNOWithHosCode(employee, "C", ref currentInvoiceNO, ref currentRealInvoiceNO, Neusoft.FrameWork.Management.Connection.Hospital.ID, ref errText);
                if (returnValue == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(errText);

                    return false;
                }

                hsInvoice.Add(invoice.Invoice.ID, currentInvoiceNO);

                returnValue = outpatientManager.UpdateBalanceCancelType(invoice.Invoice.ID, invoice.CombNO, nowTime, Neusoft.HISFC.Models.Base.CancelTypes.Reprint);
                if (returnValue == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("作废原始发票信息出错!" + outpatientManager.Err);

                    return false;
                }
                if (returnValue == 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("该发票已经作废!");

                    return false;
                }
                //处理冲账信息(负记录)
                invoice.PrintTime = invoice.BalanceOper.OperTime;
                Balance invoClone = invoice.Clone();
                invoClone.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                invoClone.FT.TotCost = -invoClone.FT.TotCost;
                invoClone.FT.OwnCost = -invoClone.FT.OwnCost;
                invoClone.FT.PayCost = -invoClone.FT.PayCost;
                invoClone.FT.PubCost = -invoClone.FT.PubCost;
                invoClone.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                //invoClone.CanceledInvoiceNO = invoice.Invoice.ID;
                //invoClone.CancelOper.ID = outpatientManager.Operator.ID;//只能自己作废自己收的发票号，所以不存
                invoClone.BalanceOper.ID = outpatientManager.Operator.ID;
                invoClone.BalanceOper.OperTime = nowTime;
                invoClone.CancelOper.OperTime = nowTime;
                invoClone.IsAuditing = false;
                invoClone.AuditingOper.ID = "";
                invoClone.AuditingOper.OperTime = DateTime.MinValue;
                invoClone.IsDayBalanced = false;
                invoClone.DayBalanceOper.OperTime = DateTime.MinValue;

                invoClone.CombNO = invoiceSeqNegative;

                returnValue = outpatientManager.InsertBalance(invoClone);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入发票冲账信息出错!!" + outpatientManager.Err);
                    return false;
                }
                invoClone.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                invoClone.FT.TotCost = -invoClone.FT.TotCost;
                invoClone.FT.OwnCost = -invoClone.FT.OwnCost;
                invoClone.FT.PayCost = -invoClone.FT.PayCost;
                invoClone.FT.PubCost = -invoClone.FT.PubCost;
                invoClone.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                invoClone.CanceledInvoiceNO = invoice.Invoice.ID;
                invoClone.CancelOper.OperTime = DateTime.MinValue;
                invoClone.Invoice.ID = currentInvoiceNO;
                invoClone.PrintedInvoiceNO = currentRealInvoiceNO;
                invoClone.BalanceOper.ID = outpatientManager.Operator.ID;
                invoClone.BalanceOper.OperTime = nowTime;
                invoClone.CombNO = invoiceSeqPositive;

                invoicesPrint.Add(invoClone);

                returnValue = outpatientManager.InsertBalance(invoClone);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入新发票信息出错!" + outpatientManager.Err);

                    return false;
                }

                //处理发票明细表信息
                ArrayList alInvoceDetail = outpatientManager.QueryBalanceListsByInvoiceNOAndInvoiceSequence(invoice.Invoice.ID, currentBalance.CombNO);
                if (comBalanceLists == null)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("获得发票明细出错!" + outpatientManager.Err);

                    return false;
                }
                //作废发票明细表信息
                returnValue = outpatientManager.UpdateBalanceListCancelType(invoice.Invoice.ID, currentBalance.CombNO, nowTime, Neusoft.HISFC.Models.Base.CancelTypes.Reprint);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("作废发票明细出错!" + outpatientManager.Err);

                    return false;
                }
                invoiceDetailsPrintTemp = new ArrayList();
                foreach (BalanceList d in alInvoceDetail)
                {
                    d.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                    d.BalanceBase.FT.OwnCost = -d.BalanceBase.FT.OwnCost;
                    d.BalanceBase.FT.PubCost = -d.BalanceBase.FT.PubCost;
                    d.BalanceBase.FT.PayCost = -d.BalanceBase.FT.PayCost;
                    d.BalanceBase.BalanceOper.OperTime = nowTime;
                    d.BalanceBase.BalanceOper.ID = outpatientManager.Operator.ID;
                    d.BalanceBase.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                    d.BalanceBase.IsDayBalanced = false;
                    d.BalanceBase.DayBalanceOper.ID = "";
                    d.BalanceBase.DayBalanceOper.OperTime = DateTime.MinValue;
                    ((Balance)d.BalanceBase).CombNO = invoiceSeqNegative;
                    //{9D9D4A6E-84D2-4c07-B6F0-5F2C8DB1DFD7}
                    d.FeeCodeStat.SortID = d.InvoiceSquence;

                    returnValue = outpatientManager.InsertBalanceList(d);

                    if (returnValue <= 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("插入发票明细冲账信息出错!" + outpatientManager.Err);

                        return false;
                    }
                    d.BalanceBase.Invoice.ID = currentInvoiceNO;
                    d.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                    d.BalanceBase.FT.OwnCost = -d.BalanceBase.FT.OwnCost;
                    d.BalanceBase.FT.PubCost = -d.BalanceBase.FT.PubCost;
                    d.BalanceBase.FT.PayCost = -d.BalanceBase.FT.PayCost;
                    d.BalanceBase.BalanceOper.OperTime = nowTime;
                    d.BalanceBase.BalanceOper.ID = outpatientManager.Operator.ID;
                    d.BalanceBase.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                    ((Balance)d.BalanceBase).CombNO = invoiceSeqPositive;
                    d.BalanceBase.FT.TotCost = d.BalanceBase.FT.OwnCost + d.BalanceBase.FT.PayCost + d.BalanceBase.FT.PubCost;

                    returnValue = outpatientManager.InsertBalanceList(d);
                    if (returnValue <= 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("插入新发票明细信息出错!" + outpatientManager.Err);

                        return false;
                    }

                    invoiceDetailsPrintTemp.Add(d);
                }

                invoiceDetailsPrint.Add(invoiceDetailsPrintTemp);

                // {565A0ED8-A071-443f-AFEA-7835D78A52BF}
                // 发票走号
                string strErr = "";
                string strInvoiceTemp = currentInvoiceNO;
                string strRealInvoiceTemp = currentRealInvoiceNO;
                //returnValue = this.feeIntegrate.UseInvoiceNO(invoiceType, "C", 1, ref strInvoiceTemp, ref strRealInvoiceTemp, ref strErr);
                returnValue = this.feeIntegrate.UseInvoiceNOWithHosCode(invoiceType, "C", 1, ref strInvoiceTemp, ref strRealInvoiceTemp, ref strErr, Neusoft.FrameWork.Management.Connection.Hospital.ID);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新操作员发票失败!" + feeIntegrate.Err);

                    return false;
                }

                //插入发票对照表，注意发票头暂时保持的是"00"
                if (feeIntegrate.InsertInvoiceExtend(currentInvoiceNO, "C", currentRealInvoiceNO, "00") < 1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入发票对照表信息出错!" + feeIntegrate.Err);
                    return false;
                }



                //if (invoiceType == "2")
                //{
                //    returnValue = this.feeIntegrate.UpdateOnlyRealInvoiceNO(currentInvoiceNO, currentRealInvoiceNO, ref errText);
                //    if (returnValue <= 0)
                //    {
                //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //        MessageBox.Show("更新操作员发票失败!" + feeIntegrate.Err);

                //        return false;
                //    }
                //}
                //else
                //{
                //    returnValue = this.feeIntegrate.UpdateInvoiceNO(currentInvoiceNO, currentRealInvoiceNO, ref errText);
                //    if (returnValue <= 0)
                //    {
                //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //        MessageBox.Show("更新操作员发票失败!" + feeIntegrate.Err);

                //        return false;
                //    }
                //}
            }
            #endregion

            #region 处理支付信息

            comBalancePays = outpatientManager.QueryBalancePaysByInvoiceSequence(currentBalance.CombNO);
            if (comBalancePays == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得患者支付信息出错！" + outpatientManager.Err);

                return false;
            }
            returnValue = outpatientManager.UpdateCancelTyeByInvoiceSequence("4", currentBalance.CombNO, Neusoft.HISFC.Models.Base.CancelTypes.Reprint);
            if (returnValue < 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("更新支付方式出错!" + outpatientManager.Err);

                return false;
            }
            foreach (BalancePay p in comBalancePays)
            {
                p.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                p.FT.TotCost = -p.FT.TotCost;
                p.FT.RealCost = -p.FT.RealCost;
                p.InputOper.OperTime = nowTime;
                p.InputOper.ID = outpatientManager.Operator.ID;
                p.InvoiceCombNO = invoiceSeqNegative;
                p.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                p.IsChecked = false;
                p.CheckOper.ID = "";
                p.CheckOper.OperTime = DateTime.MinValue;
                p.BalanceOper.ID = "";
                p.BalanceOper.OperTime = DateTime.MinValue;
                p.IsDayBalanced = false;
                p.IsAuditing = false;
                p.AuditingOper.OperTime = DateTime.MinValue;
                p.AuditingOper.ID = "";

                returnValue = outpatientManager.InsertBalancePay(p);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入支付负信息出错!" + outpatientManager.Err);

                    return false;
                }
                p.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                p.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                p.FT.TotCost = -p.FT.TotCost;
                p.FT.RealCost = -p.FT.RealCost;
                p.InvoiceCombNO = invoiceSeqPositive;
                p.Invoice.ID = currentInvoiceNO;
                returnValue = outpatientManager.InsertBalancePay(p);

                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入支付信息出错!" + outpatientManager.Err);

                    return false;
                }
            }
            #endregion

            #region 处理费用明细信息
            ArrayList feeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(currentBalance.CombNO);
            if (feeItemLists == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得患者费用明细出错!" + outpatientManager.Err);

                return false;
            }
            returnValue = outpatientManager.UpdateFeeItemListCancelType(currentBalance.CombNO, nowTime, Neusoft.HISFC.Models.Base.CancelTypes.Reprint);
            if (returnValue <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("作废患者明细出错!" + outpatientManager.Err);

                return false;
            }

            foreach (FeeItemList f in feeItemLists)
            {
                f.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                f.FT.OwnCost = -f.FT.OwnCost;
                f.FT.PayCost = -f.FT.PayCost;
                f.FT.PubCost = -f.FT.PubCost;
                f.Item.Qty = -f.Item.Qty;
                f.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                //f.FeeOper.ID = outpatientManager.Operator.ID;
                //f.FeeOper.OperTime = nowTime;
                f.ChargeOper.ID = employee.ID;
                f.ChargeOper.OperTime = nowTime;
                f.CancelOper.OperTime = nowTime;
                f.InvoiceCombNO = invoiceSeqNegative;

                returnValue = outpatientManager.InsertFeeItemList(f);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入费用明细冲帐信息出错!" + outpatientManager.Err);

                    return false;
                }
            }

            foreach (FeeItemList f in feeItemLists)
            {

                f.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                f.FT.OwnCost = -f.FT.OwnCost;
                f.FT.PayCost = -f.FT.PayCost;
                f.FT.PubCost = -f.FT.PubCost;
                f.Item.Qty = -f.Item.Qty;
                f.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                //f.FeeOper.ID = outpatientManager.Operator.ID;
                //f.FeeOper.OperTime = nowTime;
                f.ChargeOper.ID = employee.ID;
                f.ChargeOper.OperTime = nowTime;
                f.CancelOper.OperTime = nowTime;
                f.Invoice.ID = currentInvoiceNO;
                f.InvoiceCombNO = invoiceSeqPositive;

                returnValue = outpatientManager.InsertFeeItemList(f);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入费用明细信息出错!" + outpatientManager.Err);

                    return false;
                }
                feeDetailsPrint.Add(f);
            }
            #endregion

            #region 处理药品处方信息
            foreach (Balance invo in comBalances)
            {
                if (this.pharmacyIntegrate.UpdateDrugRecipeInvoiceN0(invo.Invoice.ID, hsInvoice[invo.Invoice.ID].ToString()) < 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("根据旧发票号更新新发票号出错！");

                    return false;
                }
            }

            #endregion

            // {565A0ED8-A071-443f-AFEA-7835D78A52BF}
            // 事务提交
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            // 以下打印操作不需要事务，如打印失败，请通过刷卡查找到发票号，使用补打功能。

            #region 生成赋值后的发票费用明细
            foreach (Balance b in invoicesPrint)
            {
                #region 克隆一个费用明细信息列表，因为后面操作需要对列表元素有删除操作．
                ArrayList feeItemListsClone = new ArrayList();
                foreach (FeeItemList f in feeItemLists)
                {
                    feeItemListsClone.Add(f.Clone());
                }
                #endregion

                while (feeItemListsClone.Count > 0)
                {
                    invoicefeeDetailsPrintTemp = new ArrayList();
                    string compareItem = b.Invoice.ID;
                    foreach (FeeItemList f in feeItemListsClone)
                    {
                        if (f.Invoice.ID == compareItem)
                        {
                            invoicefeeDetailsPrintTemp.Add(f);
                        }
                        else
                        {
                            break;
                        }
                    }
                    invoicefeeDetailsPrint.Add(invoicefeeDetailsPrintTemp);
                    foreach (FeeItemList f in invoicefeeDetailsPrintTemp)
                    {
                        feeItemListsClone.Remove(f);
                    }
                }
            }
            #endregion

            string strPrintErrMsg = "作废发票已成功，新发票流水号为：【" + currentInvoiceNO + "】，请使用补打功能！";

            //string invoicePrintDll = this.feeIntegrate.GetControlValue(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, string.Empty);
            string invoicePrintDll = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, false, "");

            // 更改发票打印类获取方式；兼容原来方式
            // 2011-08-04
            // 此处不提示
            //if (invoicePrintDll == null || invoicePrintDll == "")
            //{
            //    //Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //    MessageBox.Show("没有设置发票打印参数，不能打印! \r\n " + strPrintErrMsg);

            //    return false;
            //}

            Neusoft.HISFC.Models.Registration.Register rInfo = new Neusoft.HISFC.Models.Registration.Register();
            Balance invoiceTemp = ((Balance)comBalances[0]);
            rInfo.PID.CardNO = invoiceTemp.Patient.PID.CardNO;
            rInfo.Pact = invoiceTemp.Patient.Pact.Clone();
            rInfo.Name = invoiceTemp.Patient.Name;
            rInfo.SSN = invoiceTemp.Patient.SSN;
            rInfo.PID.ID = invoiceTemp.Patient.ID;
            rInfo.ID = invoiceTemp.Patient.ID;
            rInfo.PVisit.RegistTime = invoiceTemp.RegTime;

            #region
            ArrayList alPrintInvoicefeeDetails = new ArrayList();

            alPrintInvoicefeeDetails.Add(invoicefeeDetailsPrint);
            ArrayList alPrintInvoices = new ArrayList();

            alPrintInvoices.Add(invoiceDetailsPrint);
            #endregion

            returnValue = this.feeIntegrate.PrintInvoice(invoicePrintDll, rInfo, invoicesPrint, alPrintInvoices, feeDetailsPrint, alPrintInvoicefeeDetails, comBalancePays, false, ref errText);

            if (returnValue == -1)
            {
                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(errText + "\r\n" + strPrintErrMsg);

                return false;
            }

            //Neusoft.FrameWork.Management.PublicTrans.Commit();
            if (isAutoPrintGuide)
            {
                this.PrintGuide(rInfo, invoicesPrint, feeDetailsPrint, 2);
            }
            currentBalance = null;
            MessageBox.Show("操作成功!");

            Clear();

            return true;
        }

        protected void PrintMedicalBanlance()
        {
            if (currentBalance == null)
            {
                MessageBox.Show("该发票已经作废!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();
                return;
            }
            if (currentBalance.Invoice.ID == "")
            {
                MessageBox.Show("请输入发票信息!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();
                return;
            }
            string invoiceNo = currentBalance.Invoice.ID;
            string Patientid = currentBalance.Patient.ID;
            //MessageBox.Show(invoiceNo);
            GDSI.ZhuHaiSI.Print.ucNewXQMZBalanceInfogh uc = new GDSI.ZhuHaiSI.Print.ucNewXQMZBalanceInfogh();
            if (uc.Init(invoiceNo, Patientid) <= 0)
            {
                MessageBox.Show(uc.ErrMsg);
                return;
            }
            uc.PrintInBalanceInfo(20, 20);

        }

        /// <summary>
        /// 自助设备发票补打（走号，不冲正金额，仅更新原有印刷发票号）
        /// </summary>
        /// <returns></returns>
        protected bool PrintZZSB()
        {
            if (currentBalance == null)
            {
                MessageBox.Show("该发票已经作废!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();

                return false;
            }
            if (currentBalance.Invoice.ID == "")
            {
                MessageBox.Show("请输入发票信息!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();

                return false;
            }
            #region 判断是否开立过电子票和换开过纸质票  yhm
            //判断是否开立过电子票 registerControl.PatientInfo.ID
            string invoiceNOBill = currentBalance.Invoice.ID;
            Neusoft.HISFC.BizLogic.Registration.Register regMgr = new Neusoft.HISFC.BizLogic.Registration.Register();
            if (regMgr.QueryElecDataForClincCode(invoiceNOBill + currentBalance.Patient.ID, "2") > 0)//有电子票信息
            {
                MessageBox.Show("该票据为电子票据，请使用换开纸质票进行打印");
                return false;
            }
            #endregion
            string strErrText = "";
            string currentInvoiceNO = "";
            string currentRealInvoiceNO = "";
            int returnValue = 1;
            string invoiceType = controlParamIntegrate.GetControlParam<string>(Const.GET_INVOICE_NO_TYPE, false, "0");
            Neusoft.HISFC.Models.Base.Employee employee = this.outpatientManager.Operator as Neusoft.HISFC.Models.Base.Employee;
            returnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", ref currentInvoiceNO, ref currentRealInvoiceNO, ref strErrText);
            if (returnValue < 0)
            {
                MessageBox.Show("自助设备补打出错!" + strErrText);
                return false;
            }
            string strInvoiceTemp = currentInvoiceNO;
            string strRealInvoiceTemp = currentRealInvoiceNO;

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            //更新常数表
            returnValue = this.feeIntegrate.UseInvoiceNO(invoiceType, "C", 1, ref strInvoiceTemp, ref strRealInvoiceTemp, ref strErrText);
            if (returnValue <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("更新操作员发票失败!" + strErrText);

                return false;
            }

            ////插入发票对照表，注意发票头暂时保持的是"00"
            if (feeIntegrate.InsertInvoiceExtend(currentInvoiceNO, "C", currentRealInvoiceNO, "00") < 1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("插入发票对照表信息出错!" + feeIntegrate.Err);
                return false;
            }
            if (outpatientManager.UpdateZZSBPrintInvoice(employee.ID, currentBalance.Invoice.ID, currentRealInvoiceNO) < 1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("更新发票汇总表失败!" + outpatientManager.Err);
                return false;
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();
            if (this.PrintInvoiceOnly(currentBalance, out strErrText) == -1)
            {
                MessageBox.Show("自助设备补打出错，请直接不走号补打！" + strErrText);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 重打/补打发票
        /// 补打发票不走号，重打发票按系统参数MZ2010控制
        /// {565A0ED8-A071-443f-AFEA-7835D78A52BF}
        /// </summary>
        /// <param name="printType">1 = 补打；2 = 重打</param>
        /// <returns>成功 true 失败 false</returns>
        protected bool Print(int printType)
        {
            if (currentBalance == null)
            {
                MessageBox.Show("该发票已经作废!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();

                return false;
            }
            if (currentBalance.Invoice.ID == "")
            {
                MessageBox.Show("请输入发票信息!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();

                return false;
            }
            //if (printType == 1 && currentBalance.Invoice.ID.StartsWith("T"))
            //{
            //    MessageBox.Show("该发票为自助机发票,不需要补打发票", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            //if (printType == 2 && !currentBalance.Invoice.ID.StartsWith("T"))
            //{
            //    MessageBox.Show("发票已重打,不允许再重打!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            //
            // {48E41D0A-7397-402a-9E51-CD7B6CCAEBBE}
            // 终端产生临时发票，需要汇总打印
            //if (currentBalance.IsAccount)
            //{
            //    MessageBox.Show("终端扣费产生临时发票，请汇总打印！");
            //    return false;
            //}

            bool isCanQuitOtherOper = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.CAN_REPRINT_OTHER_OPER_INVOICE, true, false);

            if (!isCanQuitOtherOper)//不予许交叉重打
            {
                Balance tmpInvoice = comBalances[0] as Balance;

                if (tmpInvoice == null)
                {
                    MessageBox.Show("发票格式转换出错!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }

                if (tmpInvoice.BalanceOper.ID != this.outpatientManager.Operator.ID)
                {
                    MessageBox.Show("该发票为操作员" + tmpInvoice.BalanceOper.ID + "收取,您没有权限进重打!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }
            }

            bool isCanReprintDayBalance = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.CAN_REPRINT_DAYBALANCED_INVOICE, true, false);

            if (!isCanReprintDayBalance)//不予许交叉重打
            {
                Balance tmpInvoice = comBalances[0] as Balance;

                if (tmpInvoice == null)
                {
                    MessageBox.Show("发票格式转换出错!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }

                if (tmpInvoice.IsDayBalanced)
                {
                    MessageBox.Show("该发票已经日结,您没有权限进重打!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }
            }

            if (printType == 1)
            {
                // {565A0ED8-A071-443f-AFEA-7835D78A52BF}
                // 补打不走号
                string strErrText = "";
                if (this.PrintInvoiceOnly(currentBalance, out strErrText) == -1)
                {
                    MessageBox.Show(Language.Msg("补打出错! " + strErrText));
                    return false;
                }
                return true;
            }
            else if (!this.isRollCode)
            {
                // 重打不走号
                if (this.PrintNotRollCode() == -1)
                {
                    MessageBox.Show(Language.Msg("重打出错!"));
                    return false;
                }
                return true;
            }

            // 否则 走号 
            // 
            //Neusoft.FrameWork.Management.Transaction t = new Neusoft.FrameWork.Management.Transaction(Neusoft.FrameWork.Management.Connection.Instance);
            #region 增加重打分发票限制，分过的发票不得重打
            if (comBalances.Count > 1)
            {
                MessageBox.Show(Language.Msg("分发票的不允许重打，如需重打请退费后重收！"));
                Clear();
                return false;
            }

            #endregion
            //t.BeginTransaction();
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            pharmacyIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            controlParamIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            int returnValue = 0;
            string currentInvoiceNO = "";
            string currentRealInvoiceNO = "";
            string errText = "";
            DateTime nowTime = new DateTime();

            nowTime = outpatientManager.GetDateTimeFromSysDateTime();
            string invoiceType = controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.GET_INVOICE_NO_TYPE, false, "0");

            //发票
            ArrayList invoicesPrint = new ArrayList();
            //发票明细
            ArrayList invoiceDetailsPrintTemp = new ArrayList();
            //发票明细
            ArrayList invoiceDetailsPrint = new ArrayList();
            //发票费用明细
            ArrayList invoicefeeDetailsPrintTemp = new ArrayList();
            //发票费用明细
            ArrayList invoicefeeDetailsPrint = new ArrayList();
            //全部费用明细
            ArrayList feeDetailsPrint = new ArrayList();


            //获得负发票流水号
            string invoiceSeqNegative = outpatientManager.GetInvoiceCombNO();
            if (invoiceSeqNegative == null || invoiceSeqNegative == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得发票流水号失败!" + outpatientManager.Err);

                return false;
            }

            //获得正发票流水号
            string invoiceSeqPositive = outpatientManager.GetInvoiceCombNO();
            if (invoiceSeqPositive == null || invoiceSeqPositive == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得发票流水号失败!" + outpatientManager.Err);

                return false;
            }

            Hashtable hsInvoice = new Hashtable();

            Neusoft.HISFC.Models.Base.Employee employee = this.outpatientManager.Operator as Neusoft.HISFC.Models.Base.Employee;

            #region 处理发票信息
            foreach (Balance invoice in comBalances)
            {
                //returnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", ref currentInvoiceNO, ref currentRealInvoiceNO, ref errText);
                returnValue = this.feeIntegrate.GetInvoiceNOWithHosCode(employee, "C", ref currentInvoiceNO, ref currentRealInvoiceNO, Neusoft.FrameWork.Management.Connection.Hospital.ID, ref errText);
                if (returnValue == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(errText);

                    return false;
                }

                hsInvoice.Add(invoice.Invoice.ID, currentInvoiceNO);

                returnValue = outpatientManager.UpdateBalanceCancelType(invoice.Invoice.ID, invoice.CombNO, nowTime, Neusoft.HISFC.Models.Base.CancelTypes.Reprint);
                if (returnValue == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("作废原始发票信息出错!" + outpatientManager.Err);

                    return false;
                }
                if (returnValue == 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("该发票已经作废!");

                    return false;
                }
                //处理冲账信息(负记录)
                invoice.PrintTime = invoice.BalanceOper.OperTime;
                Balance invoClone = invoice.Clone();
                invoClone.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                invoClone.FT.TotCost = -invoClone.FT.TotCost;
                invoClone.FT.OwnCost = -invoClone.FT.OwnCost;
                invoClone.FT.PayCost = -invoClone.FT.PayCost;
                invoClone.FT.PubCost = -invoClone.FT.PubCost;
                invoClone.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                //invoClone.CanceledInvoiceNO = invoice.Invoice.ID;
                //invoClone.CancelOper.ID = outpatientManager.Operator.ID;//只能自己作废自己收的发票号，所以不存
                invoClone.BalanceOper.ID = outpatientManager.Operator.ID;
                invoClone.BalanceOper.OperTime = nowTime;
                invoClone.CancelOper.OperTime = nowTime;
                invoClone.IsAuditing = false;
                invoClone.AuditingOper.ID = "";
                invoClone.AuditingOper.OperTime = DateTime.MinValue;
                invoClone.IsDayBalanced = false;
                invoClone.DayBalanceOper.OperTime = DateTime.MinValue;

                invoClone.CombNO = invoiceSeqNegative;

                returnValue = outpatientManager.InsertBalance(invoClone);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入发票冲账信息出错!!" + outpatientManager.Err);
                    return false;
                }
                invoClone.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                invoClone.FT.TotCost = -invoClone.FT.TotCost;
                invoClone.FT.OwnCost = -invoClone.FT.OwnCost;
                invoClone.FT.PayCost = -invoClone.FT.PayCost;
                invoClone.FT.PubCost = -invoClone.FT.PubCost;
                invoClone.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                invoClone.CanceledInvoiceNO = invoice.Invoice.ID;
                invoClone.CancelOper.OperTime = DateTime.MinValue;
                invoClone.Invoice.ID = currentInvoiceNO;
                invoClone.PrintedInvoiceNO = currentRealInvoiceNO;
                invoClone.BalanceOper.ID = outpatientManager.Operator.ID;
                invoClone.BalanceOper.OperTime = nowTime;
                invoClone.CombNO = invoiceSeqPositive;

                invoicesPrint.Add(invoClone);

                returnValue = outpatientManager.InsertBalance(invoClone);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入新发票信息出错!" + outpatientManager.Err);

                    return false;
                }

                //处理发票明细表信息
                ArrayList alInvoceDetail = outpatientManager.QueryBalanceListsByInvoiceNOAndInvoiceSequence(invoice.Invoice.ID, currentBalance.CombNO);
                if (comBalanceLists == null)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("获得发票明细出错!" + outpatientManager.Err);

                    return false;
                }
                //作废发票明细表信息
                returnValue = outpatientManager.UpdateBalanceListCancelType(invoice.Invoice.ID, currentBalance.CombNO, nowTime, Neusoft.HISFC.Models.Base.CancelTypes.Reprint);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("作废发票明细出错!" + outpatientManager.Err);

                    return false;
                }
                invoiceDetailsPrintTemp = new ArrayList();
                foreach (BalanceList d in alInvoceDetail)
                {
                    d.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                    d.BalanceBase.FT.OwnCost = -d.BalanceBase.FT.OwnCost;
                    d.BalanceBase.FT.PubCost = -d.BalanceBase.FT.PubCost;
                    d.BalanceBase.FT.PayCost = -d.BalanceBase.FT.PayCost;
                    d.BalanceBase.BalanceOper.OperTime = nowTime;
                    d.BalanceBase.BalanceOper.ID = outpatientManager.Operator.ID;
                    d.BalanceBase.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                    d.BalanceBase.IsDayBalanced = false;
                    d.BalanceBase.DayBalanceOper.ID = "";
                    d.BalanceBase.DayBalanceOper.OperTime = DateTime.MinValue;
                    ((Balance)d.BalanceBase).CombNO = invoiceSeqNegative;
                    //{9D9D4A6E-84D2-4c07-B6F0-5F2C8DB1DFD7}
                    d.FeeCodeStat.SortID = d.InvoiceSquence;

                    returnValue = outpatientManager.InsertBalanceList(d);

                    if (returnValue <= 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("插入发票明细冲账信息出错!" + outpatientManager.Err);

                        return false;
                    }
                    d.BalanceBase.Invoice.ID = currentInvoiceNO;
                    d.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                    d.BalanceBase.FT.OwnCost = -d.BalanceBase.FT.OwnCost;
                    d.BalanceBase.FT.PubCost = -d.BalanceBase.FT.PubCost;
                    d.BalanceBase.FT.PayCost = -d.BalanceBase.FT.PayCost;
                    d.BalanceBase.BalanceOper.OperTime = nowTime;
                    d.BalanceBase.BalanceOper.ID = outpatientManager.Operator.ID;
                    d.BalanceBase.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                    ((Balance)d.BalanceBase).CombNO = invoiceSeqPositive;
                    d.BalanceBase.FT.TotCost = d.BalanceBase.FT.OwnCost + d.BalanceBase.FT.PayCost + d.BalanceBase.FT.PubCost;

                    returnValue = outpatientManager.InsertBalanceList(d);
                    if (returnValue <= 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("插入新发票明细信息出错!" + outpatientManager.Err);

                        return false;
                    }

                    invoiceDetailsPrintTemp.Add(d);
                }

                invoiceDetailsPrint.Add(invoiceDetailsPrintTemp);

                // {565A0ED8-A071-443f-AFEA-7835D78A52BF}
                // 发票走号
                string strErr = "";
                string strInvoiceTemp = currentInvoiceNO;
                string strRealInvoiceTemp = currentRealInvoiceNO;
                //returnValue = this.feeIntegrate.UseInvoiceNO(invoiceType, "C", 1, ref strInvoiceTemp, ref strRealInvoiceTemp, ref strErr);
                returnValue = this.feeIntegrate.UseInvoiceNOWithHosCode(invoiceType, "C", 1, ref strInvoiceTemp, ref strRealInvoiceTemp, ref strErr, Neusoft.FrameWork.Management.Connection.Hospital.ID);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新操作员发票失败!" + feeIntegrate.Err);

                    return false;
                }

                //插入发票对照表，注意发票头暂时保持的是"00"
                if (feeIntegrate.InsertInvoiceExtend(currentInvoiceNO, "C", currentRealInvoiceNO, "00") < 1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入发票对照表信息出错!" + feeIntegrate.Err);
                    return false;
                }



                //if (invoiceType == "2")
                //{
                //    returnValue = this.feeIntegrate.UpdateOnlyRealInvoiceNO(currentInvoiceNO, currentRealInvoiceNO, ref errText);
                //    if (returnValue <= 0)
                //    {
                //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //        MessageBox.Show("更新操作员发票失败!" + feeIntegrate.Err);

                //        return false;
                //    }
                //}
                //else
                //{
                //    returnValue = this.feeIntegrate.UpdateInvoiceNO(currentInvoiceNO, currentRealInvoiceNO, ref errText);
                //    if (returnValue <= 0)
                //    {
                //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //        MessageBox.Show("更新操作员发票失败!" + feeIntegrate.Err);

                //        return false;
                //    }
                //}
            }
            #endregion

            #region 处理支付信息

            comBalancePays = outpatientManager.QueryBalancePaysByInvoiceSequence(currentBalance.CombNO);
            if (comBalancePays == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得患者支付信息出错！" + outpatientManager.Err);

                return false;
            }
            returnValue = outpatientManager.UpdateCancelTyeByInvoiceSequence("4", currentBalance.CombNO, Neusoft.HISFC.Models.Base.CancelTypes.Reprint);
            if (returnValue < 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("更新支付方式出错!" + outpatientManager.Err);

                return false;
            }
            foreach (BalancePay p in comBalancePays)
            {
                p.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                p.FT.TotCost = -p.FT.TotCost;
                p.FT.RealCost = -p.FT.RealCost;
                p.InputOper.OperTime = nowTime;
                p.InputOper.ID = outpatientManager.Operator.ID;
                p.InvoiceCombNO = invoiceSeqNegative;
                p.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                p.IsChecked = false;
                p.CheckOper.ID = "";
                p.CheckOper.OperTime = DateTime.MinValue;
                p.BalanceOper.ID = "";
                p.BalanceOper.OperTime = DateTime.MinValue;
                p.IsDayBalanced = false;
                p.IsAuditing = false;
                p.AuditingOper.OperTime = DateTime.MinValue;
                p.AuditingOper.ID = "";

                returnValue = outpatientManager.InsertBalancePay(p);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入支付负信息出错!" + outpatientManager.Err);

                    return false;
                }
                p.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                p.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                p.FT.TotCost = -p.FT.TotCost;
                p.FT.RealCost = -p.FT.RealCost;
                p.InvoiceCombNO = invoiceSeqPositive;
                p.Invoice.ID = currentInvoiceNO;
                returnValue = outpatientManager.InsertBalancePay(p);

                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入支付信息出错!" + outpatientManager.Err);

                    return false;
                }
            }
            #endregion

            #region 处理费用明细信息
            ArrayList feeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(currentBalance.CombNO);
            if (feeItemLists == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得患者费用明细出错!" + outpatientManager.Err);

                return false;
            }
            returnValue = outpatientManager.UpdateFeeItemListCancelType(currentBalance.CombNO, nowTime, Neusoft.HISFC.Models.Base.CancelTypes.Reprint);
            if (returnValue <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("作废患者明细出错!" + outpatientManager.Err);

                return false;
            }

            foreach (FeeItemList f in feeItemLists)
            {
                f.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                f.FT.OwnCost = -f.FT.OwnCost;
                f.FT.PayCost = -f.FT.PayCost;
                f.FT.PubCost = -f.FT.PubCost;
                f.Item.Qty = -f.Item.Qty;
                f.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                //f.FeeOper.ID = outpatientManager.Operator.ID;
                //f.FeeOper.OperTime = nowTime;
                f.ChargeOper.ID = employee.ID;
                f.ChargeOper.OperTime = nowTime;
                f.CancelOper.OperTime = nowTime;
                f.InvoiceCombNO = invoiceSeqNegative;

                returnValue = outpatientManager.InsertFeeItemList(f);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入费用明细冲帐信息出错!" + outpatientManager.Err);

                    return false;
                }
            }

            foreach (FeeItemList f in feeItemLists)
            {

                f.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                f.FT.OwnCost = -f.FT.OwnCost;
                f.FT.PayCost = -f.FT.PayCost;
                f.FT.PubCost = -f.FT.PubCost;
                f.Item.Qty = -f.Item.Qty;
                f.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                //f.FeeOper.ID = outpatientManager.Operator.ID;
                //f.FeeOper.OperTime = nowTime;
                f.ChargeOper.ID = employee.ID;
                f.ChargeOper.OperTime = nowTime;
                f.CancelOper.OperTime = nowTime;
                f.Invoice.ID = currentInvoiceNO;
                f.InvoiceCombNO = invoiceSeqPositive;

                returnValue = outpatientManager.InsertFeeItemList(f);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入费用明细信息出错!" + outpatientManager.Err);

                    return false;
                }
                feeDetailsPrint.Add(f);
            }
            #endregion

            #region 处理药品处方信息
            foreach (Balance invo in comBalances)
            {
                if (this.pharmacyIntegrate.UpdateDrugRecipeInvoiceN0(invo.Invoice.ID, hsInvoice[invo.Invoice.ID].ToString()) < 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("根据旧发票号更新新发票号出错！");

                    return false;
                }
            }

            #endregion

            // {565A0ED8-A071-443f-AFEA-7835D78A52BF}
            // 事务提交
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            // 以下打印操作不需要事务，如打印失败，请通过刷卡查找到发票号，使用补打功能。

            #region 生成赋值后的发票费用明细
            foreach (Balance b in invoicesPrint)
            {
                #region 克隆一个费用明细信息列表，因为后面操作需要对列表元素有删除操作．
                ArrayList feeItemListsClone = new ArrayList();
                foreach (FeeItemList f in feeItemLists)
                {
                    feeItemListsClone.Add(f.Clone());
                }
                #endregion

                while (feeItemListsClone.Count > 0)
                {
                    invoicefeeDetailsPrintTemp = new ArrayList();
                    string compareItem = b.Invoice.ID;
                    foreach (FeeItemList f in feeItemListsClone)
                    {
                        if (f.Invoice.ID == compareItem)
                        {
                            invoicefeeDetailsPrintTemp.Add(f);
                        }
                        else
                        {
                            break;
                        }
                    }
                    invoicefeeDetailsPrint.Add(invoicefeeDetailsPrintTemp);
                    foreach (FeeItemList f in invoicefeeDetailsPrintTemp)
                    {
                        feeItemListsClone.Remove(f);
                    }
                }
            }
            #endregion

            string strPrintErrMsg = "作废发票已成功，新发票流水号为：【" + currentInvoiceNO + "】，请使用补打功能！";

            //string invoicePrintDll = this.feeIntegrate.GetControlValue(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, string.Empty);
            string invoicePrintDll = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, false, "");

            // 更改发票打印类获取方式；兼容原来方式
            // 2011-08-04
            // 此处不提示
            //if (invoicePrintDll == null || invoicePrintDll == "")
            //{
            //    //Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //    MessageBox.Show("没有设置发票打印参数，不能打印! \r\n " + strPrintErrMsg);

            //    return false;
            //}

            Neusoft.HISFC.Models.Registration.Register rInfo = new Neusoft.HISFC.Models.Registration.Register();
            Balance invoiceTemp = ((Balance)comBalances[0]);
            rInfo.PID.CardNO = invoiceTemp.Patient.PID.CardNO;
            rInfo.Pact = invoiceTemp.Patient.Pact.Clone();
            rInfo.Name = invoiceTemp.Patient.Name;
            rInfo.SSN = invoiceTemp.Patient.SSN;
            rInfo.PID.ID = invoiceTemp.Patient.ID;
            rInfo.ID = invoiceTemp.Patient.ID;
            rInfo.PVisit.RegistTime = invoiceTemp.RegTime;

            #region
            ArrayList alPrintInvoicefeeDetails = new ArrayList();

            alPrintInvoicefeeDetails.Add(invoicefeeDetailsPrint);
            ArrayList alPrintInvoices = new ArrayList();

            alPrintInvoices.Add(invoiceDetailsPrint);
            #endregion

            returnValue = this.feeIntegrate.PrintInvoice(invoicePrintDll, rInfo, invoicesPrint, alPrintInvoices, feeDetailsPrint, alPrintInvoicefeeDetails, comBalancePays, false, ref errText);

            if (returnValue == -1)
            {
                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(errText + "\r\n" + strPrintErrMsg);

                return false;
            }

            //Neusoft.FrameWork.Management.PublicTrans.Commit();
            if (isAutoPrintGuide)
            {
                this.PrintGuide(rInfo, invoicesPrint, feeDetailsPrint, 2);
            }
            currentBalance = null;
            MessageBox.Show("操作成功!");

            Clear();

            return true;
        }
        /// <summary>
        /// 重打发票（不走号，原样打印）
        /// </summary>
        /// <returns></returns>
        private int PrintNotRollCode()
        {
            int returnValue = 0;
            string currentInvoiceNO = "";
            string currentRealInvoiceNO = "";
            string errText = "";
            DateTime nowTime = new DateTime();
            nowTime = outpatientManager.GetDateTimeFromSysDateTime();
            string invoiceType = controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.GET_INVOICE_NO_TYPE, false, "0");


            //发票
            ArrayList invoicesPrint = new ArrayList();
            //发票明细
            ArrayList invoiceDetailsPrintTemp = new ArrayList();
            //发票明细
            ArrayList invoiceDetailsPrint = new ArrayList();
            //发票费用明细
            ArrayList invoicefeeDetailsPrintTemp = new ArrayList();
            //发票费用明细
            ArrayList invoicefeeDetailsPrint = new ArrayList();
            //全部费用明细
            ArrayList feeDetailsPrint = new ArrayList();


            //获得负发票流水号
            string invoiceSeqNegative = outpatientManager.GetInvoiceCombNO();
            if (invoiceSeqNegative == null || invoiceSeqNegative == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得发票流水号失败!" + outpatientManager.Err);

                return -1;
            }

            //获得正发票流水号
            string invoiceSeqPositive = outpatientManager.GetInvoiceCombNO();
            if (invoiceSeqPositive == null || invoiceSeqPositive == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得发票流水号失败!" + outpatientManager.Err);

                return -1;
            }

            Hashtable hsInvoice = new Hashtable();

            Neusoft.HISFC.Models.Base.Employee employee = this.outpatientManager.Operator as Neusoft.HISFC.Models.Base.Employee;

            foreach (Balance invoice in comBalances)
            {
                //returnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", ref currentInvoiceNO, ref currentRealInvoiceNO, ref errText);
                returnValue = this.feeIntegrate.GetInvoiceNOWithHosCode(employee, "C", ref currentInvoiceNO, ref currentRealInvoiceNO, Neusoft.FrameWork.Management.Connection.Hospital.ID, ref errText);
                currentInvoiceNO = invoice.Invoice.ID;
                if (returnValue == -1)
                {
                    MessageBox.Show(errText);
                    return -1;
                }
                hsInvoice.Add(invoice.Invoice.ID, currentInvoiceNO);
                invoice.PrintTime = invoice.BalanceOper.OperTime;
                Balance invoClone = invoice.Clone();

                #region 发票信息赋值
                invoClone.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                invoClone.FT.TotCost = invoClone.FT.TotCost;
                invoClone.FT.OwnCost = invoClone.FT.OwnCost;
                invoClone.FT.PayCost = invoClone.FT.PayCost;
                invoClone.FT.PubCost = invoClone.FT.PubCost;
                invoClone.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                invoClone.CanceledInvoiceNO = invoice.Invoice.ID;
                invoClone.CancelOper.ID = outpatientManager.Operator.ID;
                invoClone.BalanceOper.ID = outpatientManager.Operator.ID;
                invoClone.BalanceOper.OperTime = nowTime;
                invoClone.CancelOper.OperTime = nowTime;
                invoClone.IsAuditing = false;
                invoClone.AuditingOper.ID = "";
                invoClone.AuditingOper.OperTime = DateTime.MinValue;
                invoClone.IsDayBalanced = false;
                invoClone.DayBalanceOper.OperTime = DateTime.MinValue;
                invoClone.CombNO = invoiceSeqNegative;
                invoClone.Invoice.ID = currentInvoiceNO;
                invoClone.PrintedInvoiceNO = currentRealInvoiceNO;
                invoicesPrint.Add(invoClone);
                #endregion

                #region 处理发票明细表信息
                ArrayList alInvoceDetail = outpatientManager.QueryBalanceListsByInvoiceNOAndInvoiceSequence(invoice.Invoice.ID, currentBalance.CombNO);
                if (comBalanceLists == null)
                {
                    MessageBox.Show("获得发票明细出错!" + outpatientManager.Err);
                    return -1;
                }

                invoiceDetailsPrintTemp = new ArrayList();
                foreach (BalanceList d in alInvoceDetail)
                {
                    d.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                    d.BalanceBase.FT.OwnCost = d.BalanceBase.FT.OwnCost;
                    d.BalanceBase.FT.PubCost = d.BalanceBase.FT.PubCost;
                    d.BalanceBase.FT.PayCost = d.BalanceBase.FT.PayCost;
                    d.BalanceBase.BalanceOper.OperTime = nowTime;
                    d.BalanceBase.BalanceOper.ID = outpatientManager.Operator.ID;
                    d.BalanceBase.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                    d.BalanceBase.IsDayBalanced = false;
                    d.BalanceBase.DayBalanceOper.ID = "";
                    d.BalanceBase.DayBalanceOper.OperTime = DateTime.MinValue;
                    ((Balance)d.BalanceBase).CombNO = invoiceSeqNegative;
                    d.FeeCodeStat.SortID = d.InvoiceSquence;
                    d.BalanceBase.Invoice.ID = currentInvoiceNO;
                    d.BalanceBase.FT.TotCost = d.BalanceBase.FT.OwnCost + d.BalanceBase.FT.PayCost + d.BalanceBase.FT.PubCost;
                    invoiceDetailsPrintTemp.Add(d);
                }

                invoiceDetailsPrint.Add(invoiceDetailsPrintTemp);
                #endregion


            }

            #region 处理支付信息
            comBalancePays = outpatientManager.QueryBalancePaysByInvoiceSequence(currentBalance.CombNO);
            if (comBalancePays == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得患者支付信息出错！" + outpatientManager.Err);

                return -1;
            }
            foreach (BalancePay p in comBalancePays)
            {
                p.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                p.FT.TotCost = p.FT.TotCost;
                p.FT.RealCost = p.FT.RealCost;
                p.InputOper.OperTime = nowTime;
                p.InputOper.ID = outpatientManager.Operator.ID;
                p.InvoiceCombNO = invoiceSeqNegative;
                p.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                p.IsChecked = false;
                p.CheckOper.ID = "";
                p.CheckOper.OperTime = DateTime.MinValue;
                p.BalanceOper.ID = "";
                p.BalanceOper.OperTime = DateTime.MinValue;
                p.IsDayBalanced = false;
                p.IsAuditing = false;
                p.AuditingOper.OperTime = DateTime.MinValue;
                p.AuditingOper.ID = "";
            }
            #endregion


            #region 处理费用明细信息
            ArrayList feeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(currentBalance.CombNO);
            if (feeItemLists == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得患者费用明细出错!" + outpatientManager.Err);

                return -1;
            }

            foreach (FeeItemList f in feeItemLists)
            {
                f.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                f.FT.OwnCost = f.FT.OwnCost;
                f.FT.PayCost = f.FT.PayCost;
                f.FT.PubCost = f.FT.PubCost;
                f.Item.Qty = f.Item.Qty;
                f.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
                f.FeeOper.ID = outpatientManager.Operator.ID;
                f.FeeOper.OperTime = nowTime;
                f.CancelOper.OperTime = nowTime;
                f.Invoice.ID = currentInvoiceNO;
                f.InvoiceCombNO = invoiceSeqNegative;
            }
            #endregion

            #region 生成赋值后的发票费用明细
            foreach (Balance b in invoicesPrint)
            {
                ArrayList feeItemListsClone = new ArrayList();
                foreach (FeeItemList f in feeItemLists)
                {
                    feeItemListsClone.Add(f.Clone());
                }
                while (feeItemListsClone.Count > 0)
                {
                    invoicefeeDetailsPrintTemp = new ArrayList();
                    string compareItem = b.Invoice.ID;
                    foreach (FeeItemList f in feeItemListsClone)
                    {
                        if (f.Invoice.ID == compareItem)
                        {
                            invoicefeeDetailsPrintTemp.Add(f);
                        }
                        else
                        {
                            break;
                        }
                    }
                    invoicefeeDetailsPrint.Add(invoicefeeDetailsPrintTemp);
                    foreach (FeeItemList f in invoicefeeDetailsPrintTemp)
                    {
                        feeItemListsClone.Remove(f);
                    }
                }
            }
            #endregion

            //string invoicePrintDll = this.feeIntegrate.GetControlValue(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, string.Empty);
            string invoicePrintDll = this.controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, false, "");

            // 更改发票打印类获取方式；兼容原来方式
            // 2011-08-04
            // 此处不提示
            //if (invoicePrintDll == null || invoicePrintDll == "")
            //{
            //    MessageBox.Show("没有设置发票打印参数，不能打印!");

            //    return -1; ;
            //}

            #region 患者挂号信息
            Neusoft.HISFC.Models.Registration.Register rInfo = new Neusoft.HISFC.Models.Registration.Register();
            Balance invoiceTemp = ((Balance)comBalances[0]);
            rInfo.PID.CardNO = invoiceTemp.Patient.PID.CardNO;
            rInfo.Pact = invoiceTemp.Patient.Pact.Clone();
            rInfo.Name = invoiceTemp.Patient.Name;
            rInfo.SSN = invoiceTemp.Patient.SSN;
            #endregion


            ArrayList alPrintInvoicefeeDetails = new ArrayList();

            alPrintInvoicefeeDetails.Add(invoicefeeDetailsPrint);
            ArrayList alPrintInvoices = new ArrayList();

            alPrintInvoices.Add(invoiceDetailsPrint);

            //打印
            //if (this.isRollCode)
            //{
            returnValue = this.feeIntegrate.PrintInvoice(invoicePrintDll, rInfo, invoicesPrint, alPrintInvoices, feeDetailsPrint, alPrintInvoicefeeDetails, comBalancePays, false, ref errText);

            //}
            //else
            //{
            //    returnValue = this.feeIntegrate.PrintInvoice(invoicePrintDll, rInfo, invoicesPrint, alPrintInvoices, feeDetailsPrint, alPrintInvoicefeeDetails, comBalancePays, true, ref errText);
            //}
            if (returnValue == -1)
            {
                MessageBox.Show(errText);
                return -1;
            }
            if (isAutoPrintGuide)
            {
                this.PrintGuide(rInfo, invoicesPrint, feeDetailsPrint, 2);
            }
            currentBalance = null;
            MessageBox.Show("操作成功!");

            Clear();

            return 1;
        }
        /// <summary>
        /// 先汇总，然后打印
        /// </summary>
        /// <returns></returns>
        private void SummaryAndPrintInvoice(List<Balance> lstInvoice)
        {
            if (lstInvoice == null)
                return;
            Neusoft.HISFC.Models.Registration.Register regInfo = null;
            Balance invoiceTemp = lstInvoice[0] as Balance;
            if (invoiceTemp == null)
            {
                return;
            }

            regInfo = registerManager.GetByClinic(invoiceTemp.Patient.ID);
            if (regInfo == null)
            {
                regInfo = new Neusoft.HISFC.Models.Registration.Register();

                regInfo.PID.CardNO = invoiceTemp.Patient.PID.CardNO;
                regInfo.Pact = invoiceTemp.Patient.Pact.Clone();
                regInfo.Name = invoiceTemp.Patient.Name;
                regInfo.SSN = invoiceTemp.Patient.SSN;

            }

            Neusoft.HISFC.Models.Base.Employee employee = this.outpatientManager.Operator as Neusoft.HISFC.Models.Base.Employee;

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            pharmacyIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            controlParamIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            //获得负发票流水号
            string invoiceSeqNegative = outpatientManager.GetInvoiceCombNO();
            if (invoiceSeqNegative == null || invoiceSeqNegative == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得发票流水号失败!" + outpatientManager.Err);

                return;
            }

            //获得正发票流水号
            string invoiceSeqPositive = outpatientManager.GetInvoiceCombNO();
            if (invoiceSeqPositive == null || invoiceSeqPositive == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得发票流水号失败!" + outpatientManager.Err);

                return;
            }

            string newInvoiceNo = "";
            string newRealInvoiceNo = "";
            string errText = "";
            //int returnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", ref newInvoiceNo, ref newRealInvoiceNo, ref errText);
            int returnValue = this.feeIntegrate.GetInvoiceNOWithHosCode(employee, "C", ref newInvoiceNo, ref newRealInvoiceNo, Neusoft.FrameWork.Management.Connection.Hospital.ID, ref errText);
            if (returnValue <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(errText);
                return;
            }

            returnValue = this.outpatientManager.SummaryAccountInvoice(regInfo, employee, lstInvoice, newInvoiceNo, newRealInvoiceNo, invoiceSeqNegative, invoiceSeqPositive);
            if (returnValue <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(this.outpatientManager.Err);
                return;
            }

            foreach (Balance invo in lstInvoice)
            {
                if (this.pharmacyIntegrate.UpdateDrugRecipeInvoiceN0(invo.Invoice.ID, newInvoiceNo) < 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("根据旧发票号更新新发票号出错！");

                    return;
                }
            }

            //插入发票对照表，注意发票头暂时保持的是"00"
            if (feeIntegrate.InsertInvoiceExtend(newInvoiceNo, "C", newRealInvoiceNo, "00") < 1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("插入发票对照表信息出错!" + feeIntegrate.Err);
                return;
            }

            string invoiceStytle = controlParamIntegrate.GetControlParam<string>(Const.GET_INVOICE_NO_TYPE, false, "0");
            //if (feeIntegrate.UseInvoiceNO(invoiceStytle, "C", 1, ref newInvoiceNo, ref newRealInvoiceNo, ref errText) < 0)
            if (feeIntegrate.UseInvoiceNOWithHosCode(invoiceStytle, "C", 1, ref newInvoiceNo, ref newRealInvoiceNo, ref errText, Neusoft.FrameWork.Management.Connection.Hospital.ID) < 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("更新发票号出错！");

                return;
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();
            MessageBox.Show("汇总发票成功！");



            #region 发票打印

            string invoicePrintDll = null;

            invoicePrintDll = controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, false, string.Empty);

            // 更改发票打印类获取方式；兼容原来方式
            // 2011-08-04
            // 此处不作提示
            //if (invoicePrintDll == null || invoicePrintDll == string.Empty)
            //{
            //    MessageBox.Show("没有设置发票打印参数，收费请维护!");
            //}

            ArrayList invoices = outpatientManager.QueryBalancesByInvoiceSequence(invoiceSeqPositive);
            if (invoices == null || invoices.Count <= 0)
            {
                MessageBox.Show("获取发票信息失败！", "打印失败");
                return;
            }
            ArrayList invoiceDetails = outpatientManager.QueryBalanceListsByInvoiceSequence(invoiceSeqPositive);
            if (invoiceDetails == null || invoiceDetails.Count <= 0)
            {
                MessageBox.Show("获取发票明细信息失败！", "打印失败");
                return;
            }
            ArrayList arTemp = new ArrayList();
            arTemp.Add(invoiceDetails);
            ArrayList arlTemp2 = new ArrayList();
            arlTemp2.Add(arTemp);


            ArrayList payModes = outpatientManager.QueryBalancePaysByInvoiceSequence(invoiceSeqPositive);
            if (payModes == null || payModes.Count <= 0)
            {
                MessageBox.Show("获取支付方式信息失败！", "打印失败");
                return;
            }
            ArrayList feeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(invoiceSeqPositive);
            if (feeItemLists == null || feeItemLists.Count <= 0)
            {
                MessageBox.Show("获取费用明细信息失败！", "打印失败");
                return;
            }

            #region 生成赋值后的发票费用明细

            ArrayList invoicefeeDetailsPrint = new ArrayList();

            foreach (Balance b in invoices)
            {

                #region 克隆一个费用明细信息列表，因为后面操作需要对列表元素有删除操作．
                ArrayList feeItemListsClone = new ArrayList();
                foreach (FeeItemList f in feeItemLists)
                {
                    feeItemListsClone.Add(f.Clone());
                }
                #endregion

                while (feeItemListsClone.Count > 0)
                {
                    ArrayList invoicefeeDetailsPrintTemp = new ArrayList();
                    string compareItem = b.Invoice.ID;
                    foreach (FeeItemList f in feeItemListsClone)
                    {
                        if (f.Invoice.ID == compareItem)
                        {
                            invoicefeeDetailsPrintTemp.Add(f);
                        }
                        else
                        {
                            break;
                        }
                    }
                    invoicefeeDetailsPrint.Add(invoicefeeDetailsPrintTemp);
                    foreach (FeeItemList f in invoicefeeDetailsPrintTemp)
                    {
                        feeItemListsClone.Remove(f);
                    }
                }
            }
            #endregion

            ArrayList arlTemp3 = new ArrayList();
            arlTemp3.Add(invoicefeeDetailsPrint);

            foreach (Balance invoice in invoices)
            {
                invoice.PrintTime = invoice.BalanceOper.OperTime;
            }
            Clear();
            this.feeIntegrate.PrintInvoice(invoicePrintDll, regInfo, invoices, arlTemp2, feeItemLists, arlTemp3, payModes, false, ref errText);

            #endregion


        }
        /// <summary>
        /// 先汇总，然后打印
        /// </summary>
        private void SummaryAndPrintInvoice()
        {
            if (ucTrvInvoice.SelectNode == null)
            {
                MessageBox.Show("请选择预交金发票！");
                return;
            }

            TreeNode tnSelect = ucTrvInvoice.SelectNode;
            List<Balance> lstBalance = new List<Balance>();
            Balance invoice = null;

            if (tnSelect.Tag != null)
            {
                tnSelect = tnSelect.Parent;
            }
            if (tnSelect != null && tnSelect.Nodes != null)
            {
                foreach (TreeNode tn in tnSelect.Nodes)
                {
                    if (tn != null && tn.Tag != null)
                    {
                        invoice = tn.Tag as Balance;
                        if (invoice != null && invoice.IsAccount)
                        {
                            lstBalance.Add(invoice);
                        }
                    }
                }
            }

            if (lstBalance != null && lstBalance.Count > 0)
            {
                DialogResult result = MessageBox.Show("是否汇总并打印发票?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    SummaryAndPrintInvoice(lstBalance);
                }
            }
            else
            {
                MessageBox.Show("请选择预交金发票！");
            }
        }
        /// <summary>
        /// 打印发票
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="strErrText"></param>
        /// <returns></returns>
        private int PrintInvoiceOnly(Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice, out string strErrText)
        {
            strErrText = "";
            if (invoice == null)
            {
                return -1;
            }

            Neusoft.HISFC.Models.Registration.Register regInfo = new Neusoft.HISFC.Models.Registration.Register();
            Balance invoiceTemp = invoice;
            regInfo = this.registerManager.GetByClinic(invoiceTemp.Patient.ID);
            regInfo.PID.CardNO = invoiceTemp.Patient.PID.CardNO;
            regInfo.Pact = invoiceTemp.Patient.Pact.Clone();
            regInfo.Name = invoiceTemp.Patient.Name;
            regInfo.SSN = invoiceTemp.Patient.SSN;
            regInfo.DoctorInfo.SeeDate = ((Neusoft.HISFC.Models.Registration.Register)invoiceTemp.Patient).DoctorInfo.SeeDate;
            regInfo.PID.ID = invoiceTemp.Patient.ID;
            regInfo.ID = invoiceTemp.Patient.ID;
            string invoiceSeq = invoiceTemp.CombNO;

            #region 发票打印

            string invoicePrintDll = null;

            invoicePrintDll = controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, false, string.Empty);

            // 更改发票打印类获取方式；兼容原来方式
            // 2011-08-04
            // 此处不作提示
            //if (invoicePrintDll == null || invoicePrintDll == string.Empty)
            //{
            //    strErrText = "没有设置发票打印参数，收费请维护!";
            //    return -1;
            //}

            ArrayList invoices = outpatientManager.QueryBalancesByInvoiceSequence(invoiceSeq);
            if (invoices == null || invoices.Count <= 0)
            {
                strErrText = "获取发票信息失败！";
                return -1;
            }
            ArrayList invoiceDetails = outpatientManager.QueryBalanceListsByInvoiceSequence(invoiceSeq);
            if (invoiceDetails == null || invoiceDetails.Count <= 0)
            {
                strErrText = "获取发票明细信息失败！";
                return -1;
            }
            ArrayList arTemp = new ArrayList();
            arTemp.Add(invoiceDetails);
            ArrayList arlTemp2 = new ArrayList();
            arlTemp2.Add(arTemp);


            ArrayList payModes = outpatientManager.QueryBalancePaysByInvoiceSequence(invoiceSeq);
            if (payModes == null || payModes.Count <= 0)
            {
                strErrText = "获取支付方式信息失败！";
                return -1;
            }
            ArrayList feeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(invoiceSeq);
            if (feeItemLists == null || feeItemLists.Count <= 0)
            {
                strErrText = "获取费用明细信息失败！";
                return -1;
            }

            #region 生成赋值后的发票费用明细

            ArrayList invoicefeeDetailsPrint = new ArrayList();

            foreach (Balance b in invoices)
            {

                #region 克隆一个费用明细信息列表，因为后面操作需要对列表元素有删除操作．
                ArrayList feeItemListsClone = new ArrayList();
                foreach (FeeItemList f in feeItemLists)
                {
                    feeItemListsClone.Add(f.Clone());
                }
                #endregion

                while (feeItemListsClone.Count > 0)
                {
                    ArrayList invoicefeeDetailsPrintTemp = new ArrayList();
                    //string compareItem = b.Invoice.ID; 判断发票号的话对于发票重打不能保证发票表中所以发票号在费用明细表中对应
                    string compareItem = b.CombNO;
                    foreach (FeeItemList f in feeItemListsClone)
                    {
                        //if (f.Invoice.ID == compareItem) 
                        if (f.InvoiceCombNO == compareItem)
                        {
                            invoicefeeDetailsPrintTemp.Add(f);
                        }
                        else
                        {
                            break;
                        }
                    }
                    invoicefeeDetailsPrint.Add(invoicefeeDetailsPrintTemp);
                    foreach (FeeItemList f in invoicefeeDetailsPrintTemp)
                    {
                        feeItemListsClone.Remove(f);
                    }
                }
            }
            #endregion

            ArrayList arlTemp3 = new ArrayList();
            arlTemp3.Add(invoicefeeDetailsPrint);


            foreach (Balance inv in invoices)
            {
                inv.PrintTime = invoice.BalanceOper.OperTime;
            }

            this.feeIntegrate.PrintInvoice(invoicePrintDll, regInfo, invoices, arlTemp2, feeItemLists, arlTemp3, payModes, false, ref strErrText);

            if (isAutoPrintGuide)
            {
                this.PrintGuide(regInfo, invoices, feeItemLists, 2);
            }

            #endregion

            return 1;
        }

        private void PrintOutPatientGuide()
        {
            if (comBalances == null || comBalances.Count <= 0)
            {
                MessageBox.Show("请输入发票信息！");
                return;
            }

            if (comFeeItemLists == null || comBalances.Count <= 0)
            {
                MessageBox.Show("无费用信息！");
                return;
            }

            Balance balance = comBalances[0] as Balance;
            Register patient = balance.Patient as Register;


            #region 门诊指引单打印

            this.PrintGuide(patient, comBalances, comFeeItemLists, 2);

            #endregion
        }

        /// <summary>
        /// 查看门诊记账单
        /// </summary>
        private void PrintOutPatientMZJZD()
        {
            if (comBalances == null || comBalances.Count <= 0)
            {
                MessageBox.Show("请输入发票信息！");
                return;
            }

            if (comFeeItemLists == null || comBalances.Count <= 0)
            {
                MessageBox.Show("无费用信息！");
                return;
            }

            Balance balance = comBalances[0] as Balance;
            Register patient = balance.Patient as Register;


            #region 门诊记账单查看

            this.PrintGuide(patient, comBalances, comFeeItemLists, 3);

            #endregion
        }

        /// <summary>
        /// 打印发票副本
        /// </summary>
        private void PrintInvoiceCopy()
        {
            if (currentBalance == null)
            {
                return;
            }
            Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint iInvoicePrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(
                this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint;
            if (iInvoicePrint == null)
            {
                MessageBox.Show("未配置发票打印接口");
                return;
            }

            Neusoft.HISFC.Models.Registration.Register regInfo = new Neusoft.HISFC.Models.Registration.Register();
            regInfo = this.registerManager.GetByClinic(currentBalance.Patient.ID);
            regInfo.PID.CardNO = currentBalance.Patient.PID.CardNO;
            regInfo.Pact = currentBalance.Patient.Pact.Clone();
            regInfo.Name = currentBalance.Patient.Name;
            regInfo.SSN = currentBalance.Patient.SSN;
            regInfo.DoctorInfo.SeeDate = ((Neusoft.HISFC.Models.Registration.Register)currentBalance.Patient).DoctorInfo.SeeDate;
            regInfo.PID.ID = currentBalance.Patient.ID;
            regInfo.ID = currentBalance.Patient.ID;
            regInfo.SIMainInfo.InvoiceNo = currentBalance.Invoice.ID;
            #region 获取医保结算信息
            if (regInfo.Pact.PayKind.ID == "02")
            {
                Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareInterfaceProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();

                int returnValue = medcareInterfaceProxy.SetPactCode(currentBalance.Patient.Pact.ID);


                medcareInterfaceProxy.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                medcareInterfaceProxy.Connect();

                ArrayList FeeDetails = new ArrayList();

                returnValue = medcareInterfaceProxy.QueryFeeDetailsOutpatient(regInfo, ref FeeDetails);

                medcareInterfaceProxy.Disconnect();

            }
            #endregion


            string invoiceSeq = currentBalance.CombNO;

            ArrayList invoiceDetails = outpatientManager.QueryBalanceListsByInvoiceSequence(invoiceSeq);
            if (invoiceDetails == null || invoiceDetails.Count <= 0)
            {
                MessageBox.Show("获取发票明细信息失败！");
                return;
            }

            ArrayList payModes = outpatientManager.QueryBalancePaysByInvoiceSequence(invoiceSeq);
            if (payModes == null || payModes.Count <= 0)
            {
                MessageBox.Show("获取支付方式信息失败！");
                return;
            }

            ArrayList feeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(invoiceSeq);
            if (feeItemLists == null || feeItemLists.Count <= 0)
            {
                MessageBox.Show("获取费用明细信息失败！");
                return;
            }

            this.currentBalance.PrintTime = DateTime.Parse(tbInvoiceDate.Text);
            iInvoicePrint.SetPrintValue(regInfo, currentBalance, invoiceDetails, feeItemLists, payModes, true);
            iInvoicePrint.Print();
        }

        #region 内部方法
        /// <summary>
        /// 清空
        /// </summary>
        protected virtual void Clear()
        {
            comBalanceLists = new ArrayList();
            comFeeItemLists = new ArrayList();
            comBalances = new ArrayList();
            comBalancePays = new ArrayList();
            currentBalance = new Neusoft.HISFC.Models.Fee.Outpatient.Balance();
            this.tbInvoiceDate.Text = "";
            this.tbInvoiceNo.Text = "";
            this.tbPName.Text = "";
            this.tbOperName.Text = "";
            this.tbOwnCost.Text = "";
            this.tbPayCost.Text = "";
            this.tbPubCost.Text = "";
            this.tbTotCost.Text = "";
            this.fpSpread1_Sheet1.RowCount = 0;
            this.fpSpread1_Sheet1.RowCount = 5;
            this.tbInvoiceNo.Focus();
        }
        /// <summary>
        /// 显示发票费用信息
        /// </summary>
        /// <param name="invoiceCombNO"></param>
        /// <returns>成功 true 失败 false</returns>
        private bool FillBalanceLists(string invoiceCombNO)
        {
            comBalanceLists = outpatientManager.QueryBalanceListsByInvoiceSequencegh(invoiceCombNO);

            if (comBalanceLists == null)
            {
                return false;
            }

            BalanceList balanceList = new BalanceList();
            for (int i = 0; i < comBalanceLists.Count; i++)
            {
                balanceList = comBalanceLists[i] as BalanceList;
                if (i > 4)
                {
                    this.fpSpread1_Sheet1.Rows.Add(i, 1);
                }
                this.fpSpread1_Sheet1.Cells[i, 0].Text = balanceList.FeeCodeStat.Name;
                this.fpSpread1_Sheet1.Cells[i, 1].Text = balanceList.BalanceBase.FT.OwnCost.ToString();
                this.fpSpread1_Sheet1.Cells[i, 2].Text = balanceList.BalanceBase.FT.PayCost.ToString();
                this.fpSpread1_Sheet1.Cells[i, 3].Text = balanceList.BalanceBase.FT.PubCost.ToString();
                this.fpSpread1_Sheet1.Cells[i, 4].Text = (balanceList.BalanceBase.FT.OwnCost + balanceList.BalanceBase.FT.PayCost + balanceList.BalanceBase.FT.PubCost).ToString();
            }

            return true;
        }

        #endregion

        #region 打印门诊指引单
        /// <summary>
        /// 打印门诊指引单
        /// </summary>
        /// <param name="rInfo"></param>
        /// <param name="invoices"></param>
        /// <param name="feeDetails"></param>
        /// <param name="printType">打印类型 清单补打=2 记账单查看=3</param>
        private void PrintGuide(Register rInfo, ArrayList invoices, ArrayList feeDetails, int printType)
        {
            IOutpatientGuide print = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(IOutpatientGuide)) as IOutpatientGuide;
            if (print != null)
            {
                rInfo.PrintInvoiceCnt = printType;
                print.SetValue(rInfo, invoices, feeDetails);
                print.Print();
            }
        }

        #endregion

        /// <summary>
        /// 作废发票,只能作废自己收到发票
        /// {565A0ED8-A071-443f-AFEA-7835D78A52BF}
        /// </summary>
        /// <returns>成功 true 失败 false</returns>
        protected bool CancelInvoicePrint()
        {
            if (currentBalance == null)
            {
                MessageBox.Show("该发票已经作废!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();

                return false;
            }
            if (currentBalance.Invoice.ID == "")
            {
                MessageBox.Show("请输入发票信息!");
                this.tbInvoiceNo.Focus();
                this.tbInvoiceNo.SelectAll();

                return false;
            }

            //
            // {48E41D0A-7397-402a-9E51-CD7B6CCAEBBE}
            // 终端产生临时发票，需要汇总打印
            if (currentBalance.IsAccount)
            {
                MessageBox.Show("终端扣费产生临时发票，请汇总打印！");
                return false;
            }

            //bool isCanQuitOtherOper = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.CAN_REPRINT_OTHER_OPER_INVOICE, true, false);
            bool isCanQuitOtherOper = false;
            if (!isCanQuitOtherOper)//不予许交叉重打
            {
                Balance tmpInvoice = comBalances[0] as Balance;

                if (tmpInvoice == null)
                {
                    MessageBox.Show("发票格式转换出错!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }

                if (tmpInvoice.BalanceOper.ID != this.outpatientManager.Operator.ID)
                {
                    MessageBox.Show("该发票为操作员" + tmpInvoice.BalanceOper.ID + "收取,您没有权限进作废!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }
            }

            //bool isCanReprintDayBalance = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.CAN_REPRINT_DAYBALANCED_INVOICE, true, false);
            bool isCanReprintDayBalance = false;
            if (!isCanReprintDayBalance)//不予许交叉重打
            {
                Balance tmpInvoice = comBalances[0] as Balance;

                if (tmpInvoice == null)
                {
                    MessageBox.Show("发票格式转换出错!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }

                if (tmpInvoice.IsDayBalanced)
                {
                    MessageBox.Show("该发票已经日结,您没有权限进作废!");
                    tbInvoiceNo.SelectAll();
                    tbInvoiceNo.Focus();

                    return false;
                }
            }

            ArrayList feeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(currentBalance.CombNO);
            if (feeItemLists == null)
            {
                MessageBox.Show("获得患者费用明细出错!" + outpatientManager.Err);
                return false;
            }

            Hashtable table = new Hashtable();
            foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in feeItemLists)
            {
                if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
                {
                    if (table.ContainsKey(string.IsNullOrEmpty(f.UndrugComb.ID) ? f.Item.ID : f.UndrugComb.ID) == false && f.IsConfirmed && f.ConfirmedQty != 0)
                    {
                        if (MessageBox.Show(this, (string.IsNullOrEmpty(f.UndrugComb.Name) ? f.Item.Name : f.UndrugComb.Name) + "已经在医技终端科室确认，是否继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            return false;
                        }
                        table.Add(string.IsNullOrEmpty(f.UndrugComb.ID) ? f.Item.ID : f.UndrugComb.ID, f);
                    }
                }
                else if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                {
                    if (f.IsConfirmed == true && f.ConfirmedQty != 0)
                    {
                        MessageBox.Show(f.Item.Name + "该药品未在药房全退，不能进行作废发票! 药品：" + f.Item.Name);
                        return false;
                    }
                }
            }


            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            pharmacyIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            controlParamIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            int returnValue = 0;
            string currentInvoiceNO = "";
            string currentRealInvoiceNO = "";
            string errText = "";
            DateTime nowTime = new DateTime();

            nowTime = outpatientManager.GetDateTimeFromSysDateTime();

            //发票
            ArrayList invoicesPrint = new ArrayList();
            //发票明细
            ArrayList invoiceDetailsPrintTemp = new ArrayList();
            //发票明细
            ArrayList invoiceDetailsPrint = new ArrayList();
            //发票费用明细
            ArrayList invoicefeeDetailsPrintTemp = new ArrayList();
            //发票费用明细
            ArrayList invoicefeeDetailsPrint = new ArrayList();
            //全部费用明细
            ArrayList feeDetailsPrint = new ArrayList();


            //获得负发票流水号
            string invoiceSeqNegative = outpatientManager.GetInvoiceCombNO();
            if (invoiceSeqNegative == null || invoiceSeqNegative == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得发票流水号失败!" + outpatientManager.Err);

                return false;
            }

            Neusoft.HISFC.Models.Base.Employee employee = this.outpatientManager.Operator as Neusoft.HISFC.Models.Base.Employee;

            #region 处理发票信息
            foreach (Balance invoice in comBalances)
            {
                returnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", ref currentInvoiceNO, ref currentRealInvoiceNO, ref errText);

                if (returnValue == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(errText);

                    return false;
                }

                returnValue = outpatientManager.UpdateBalanceCancelType(invoice.Invoice.ID, invoice.CombNO, nowTime, Neusoft.HISFC.Models.Base.CancelTypes.LogOut);
                if (returnValue == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("作废原始发票信息出错!" + outpatientManager.Err);

                    return false;
                }
                if (returnValue == 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("该发票已经作废!");

                    return false;
                }
                //处理冲账信息(负记录)
                invoice.PrintTime = invoice.BalanceOper.OperTime;
                Balance invoClone = invoice.Clone();
                invoClone.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                invoClone.FT.TotCost = -invoClone.FT.TotCost;
                invoClone.FT.OwnCost = -invoClone.FT.OwnCost;
                invoClone.FT.PayCost = -invoClone.FT.PayCost;
                invoClone.FT.PubCost = -invoClone.FT.PubCost;
                invoClone.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.LogOut;
                invoClone.CanceledInvoiceNO = invoice.Invoice.ID;
                invoClone.CancelOper.ID = outpatientManager.Operator.ID;
                invoClone.BalanceOper.ID = outpatientManager.Operator.ID;
                invoClone.BalanceOper.OperTime = nowTime;
                invoClone.CancelOper.OperTime = nowTime;
                invoClone.IsAuditing = false;
                invoClone.AuditingOper.ID = "";
                invoClone.AuditingOper.OperTime = DateTime.MinValue;
                invoClone.IsDayBalanced = false;
                invoClone.DayBalanceOper.OperTime = DateTime.MinValue;

                invoClone.CombNO = invoiceSeqNegative;

                returnValue = outpatientManager.InsertBalance(invoClone);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入发票冲账信息出错!!" + outpatientManager.Err);
                    return false;
                }

                //处理发票明细表信息
                ArrayList alInvoceDetail = outpatientManager.QueryBalanceListsByInvoiceNOAndInvoiceSequence(invoice.Invoice.ID, currentBalance.CombNO);
                if (comBalanceLists == null)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("获得发票明细出错!" + outpatientManager.Err);

                    return false;
                }
                //作废发票明细表信息
                returnValue = outpatientManager.UpdateBalanceListCancelType(invoice.Invoice.ID, currentBalance.CombNO, nowTime, Neusoft.HISFC.Models.Base.CancelTypes.LogOut);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("作废发票明细出错!" + outpatientManager.Err);

                    return false;
                }
                invoiceDetailsPrintTemp = new ArrayList();
                foreach (BalanceList d in alInvoceDetail)
                {
                    d.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                    d.BalanceBase.FT.OwnCost = -d.BalanceBase.FT.OwnCost;
                    d.BalanceBase.FT.PubCost = -d.BalanceBase.FT.PubCost;
                    d.BalanceBase.FT.PayCost = -d.BalanceBase.FT.PayCost;
                    d.BalanceBase.BalanceOper.OperTime = nowTime;
                    d.BalanceBase.BalanceOper.ID = outpatientManager.Operator.ID;
                    d.BalanceBase.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.LogOut;
                    d.BalanceBase.IsDayBalanced = false;
                    d.BalanceBase.DayBalanceOper.ID = "";
                    d.BalanceBase.DayBalanceOper.OperTime = DateTime.MinValue;
                    ((Balance)d.BalanceBase).CombNO = invoiceSeqNegative;
                    //{9D9D4A6E-84D2-4c07-B6F0-5F2C8DB1DFD7}
                    d.FeeCodeStat.SortID = d.InvoiceSquence;

                    returnValue = outpatientManager.InsertBalanceList(d);

                    if (returnValue <= 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("插入发票明细冲账信息出错!" + outpatientManager.Err);

                        return false;
                    }
                }
            }
            #endregion

            #region 处理支付信息

            comBalancePays = outpatientManager.QueryBalancePaysByInvoiceSequence(currentBalance.CombNO);
            if (comBalancePays == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得患者支付信息出错！" + outpatientManager.Err);

                return false;
            }
            returnValue = outpatientManager.UpdateCancelTyeByInvoiceSequence("4", currentBalance.CombNO, Neusoft.HISFC.Models.Base.CancelTypes.LogOut);
            if (returnValue < 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("更新支付方式出错!" + outpatientManager.Err);

                return false;
            }
            foreach (BalancePay p in comBalancePays)
            {
                p.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                p.FT.TotCost = -p.FT.TotCost;
                p.FT.RealCost = -p.FT.RealCost;
                p.InputOper.OperTime = nowTime;
                p.InputOper.ID = outpatientManager.Operator.ID;
                p.InvoiceCombNO = invoiceSeqNegative;
                p.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.LogOut;
                p.IsChecked = false;
                p.CheckOper.ID = "";
                p.CheckOper.OperTime = DateTime.MinValue;
                p.BalanceOper.ID = "";
                p.BalanceOper.OperTime = DateTime.MinValue;
                p.IsDayBalanced = false;
                p.IsAuditing = false;
                p.AuditingOper.OperTime = DateTime.MinValue;
                p.AuditingOper.ID = "";

                returnValue = outpatientManager.InsertBalancePay(p);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入支付负信息出错!" + outpatientManager.Err);

                    return false;
                }
            }
            #endregion

            #region 处理费用明细信息

            feeItemLists = outpatientManager.QueryFeeItemListsByInvoiceSequence(currentBalance.CombNO);
            if (feeItemLists == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("获得患者费用明细出错!" + outpatientManager.Err);

                return false;
            }

            returnValue = outpatientManager.UpdateFeeItemListCancelType(currentBalance.CombNO, nowTime, Neusoft.HISFC.Models.Base.CancelTypes.LogOut);
            if (returnValue <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("作废患者明细出错!" + outpatientManager.Err);

                return false;
            }

            ArrayList alOldFeeItemLists = new ArrayList();
            foreach (FeeItemList f in feeItemLists)
            {
                alOldFeeItemLists.Add(f.Clone());
                f.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                f.FT.OwnCost = -f.FT.OwnCost;
                f.FT.PayCost = -f.FT.PayCost;
                f.FT.PubCost = -f.FT.PubCost;
                f.Item.Qty = -f.Item.Qty;
                f.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.LogOut;
                //f.FeeOper.ID = outpatientManager.Operator.ID;
                //f.FeeOper.OperTime = nowTime;
                f.ChargeOper.ID = employee.ID;
                f.ChargeOper.OperTime = nowTime;
                f.CancelOper.OperTime = nowTime;
                f.InvoiceCombNO = invoiceSeqNegative;
                returnValue = outpatientManager.InsertFeeItemList(f);
                if (returnValue <= 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("插入费用明细冲帐信息出错!" + outpatientManager.Err);

                    return false;
                }

                //处理发药申请记录
                if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                {
                    if (f.IsConfirmed == false)
                    {
                        int iReturn = pharmacyIntegrate.CancelApplyOutClinic(f.RecipeNO, f.SequenceNO);
                        if (iReturn < 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("作废发药申请出错!" + pharmacyIntegrate.Err);
                            return false;
                        }
                    }
                }
            }

            if (InterfaceManager.GetIOrder() != null)
            {
                if (InterfaceManager.GetIOrder().SendFeeInfo((comBalances[0] as Balance).Patient, alOldFeeItemLists, false) < 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(this, "退费失败，请向系统管理员报告错误信息：" + InterfaceManager.GetIOrder().Err, "提示>>", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }


            if (this.isCopyRecipes)
            {
                Balance curBalance = comBalances[0] as Balance;
                Register patient = curBalance.Patient as Register;
                string curText = string.Empty;
                foreach (FeeItemList f in alOldFeeItemLists)
                {
                    f.FTSource = "0";//划价保存费用来源
                    if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
                    {
                        if (table.ContainsKey(string.IsNullOrEmpty(f.UndrugComb.ID) ? f.Item.ID : f.UndrugComb.ID) && f.IsConfirmed && f.ConfirmedQty != 0)
                        {
                            f.Item.IsNeedConfirm = false;
                            f.Item.NeedConfirm = Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm.None;
                        }
                        else
                        {
                            bool flag = outpatientManager.IsHaveTechApplyNo(f.RecipeNO, f.SequenceNO);
                            if (flag)
                            {
                                f.Item.IsNeedConfirm = true;
                                f.Item.NeedConfirm = Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm.Outpatient;
                            }
                        }
                    }
                    else
                    {
                        f.IsConfirmed = false;
                    }

                    f.Item.SpecialFlag2 = Neusoft.FrameWork.Function.NConvert.ToInt32(f.IsConfirmed).ToString();
                    f.RecipeSequence = null;
                    f.RecipeNO = null;
                    f.Invoice = null;
                    f.InvoiceCombNO = "NULL";
                    f.FeeOper.OperTime = DateTime.MinValue;
                    f.FeeOper.ID = string.Empty;
                    f.FeeOper.Name = string.Empty;
                }
                DateTime dtCharge = outpatientManager.GetDateTimeFromSysDateTime();
                if (!feeIntegrate.SetChargeInfo(patient, alOldFeeItemLists, dtCharge, ref curText))
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("处方复制失败!" + feeIntegrate.Err);
                    return false;
                }
            }


            #endregion

            // 事务提交
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show("作废发票成功!");

            Clear();

            return true;
        }

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

        #region IReprintInvoice 成员
        /// <summary>
        /// 重打发票
        /// </summary>
        /// <returns></returns>
        public bool PrintInvoice()
        {
            if (MessageBox.Show("是否要重打发票?\r\n作废原发票，产生新的发票号！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                return this.Print(2);
            }
            return false;
        }
        /// <summary>
        /// 补打发票
        /// </summary>
        /// <returns></returns>
        public bool PrintInvoiceNotRollCode()
        {
            if (MessageBox.Show("是否要补打发票?\r\n补打发票，不走号！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                return this.Print(1);
            }
            return false;
        }

        public bool SetValue(Balance invoice)
        {

            if (invoice == null)
            {
                return false;
            }

            this.ucRePrintInvoice_Load(null, null);

            this.tbInvoiceNo.Text = invoice.Invoice.ID;
            string errMsg = "";
            this.QueryBalances(invoice.Invoice.ID, out errMsg);

            if (!string.IsNullOrEmpty(errMsg))
            {
                MessageBox.Show(errMsg);
                return false;
            }

            return true;
        }

        #endregion
        private void tbID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            ArrayList tal = new ArrayList();

            string cardNo;
            //add by tangyi 20220317 根据身份证查询Name
            string sql_1 = "select p.name from com_patientinfo p where p.idenno ='{0}'";
            sql_1 = string.Format(sql_1, tbID.Text);
            cardNo = outpatientManager.ExecSqlReturnOne(sql_1);
            if (string.IsNullOrEmpty(cardNo))
            {
                MessageBox.Show("查询数据失败!");
                return;
            }
            string sql = @"select i.invoice_no, i.print_invoiceno, i.tot_cost, i.oper_date, i.name
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

                tal.Add(c);
            }
            Neusoft.HISFC.Models.Base.Const tobj = null;

            if (tal.Count == 0)
            {
                MessageBox.Show("没有需要打印的数据!");
                return;
            }
            else
            {
                FrameWork.WinForms.Forms.frmEasyChoose fc = new Neusoft.FrameWork.WinForms.Forms.frmEasyChoose(tal);
                string[] cols = { "发票电脑号", "发票印刷号", "金额", "日期", "姓名" };
                bool[] visibles = { true, true, true, true, true, false };
                int[] widths = { 80, 80, 80, 120, 60 };
                fc.SetFormat(cols, visibles, widths);
                fc.ShowDialog();

                tobj = fc.Object as Neusoft.HISFC.Models.Base.Const;
                if (tobj == null)
                {
                    MessageBox.Show("请选择一条记录!");
                    return;
                }

            }

            if (tobj.SpellCode == "已打印")
            {
                MessageBox.Show("该发票已经打印，不能再打印!");
                return;
            }

            // MessageBox.Show(tobj.ID);

            tbInvoiceNo.Text = tobj.ID;
            tbInvoiceNo_KeyDown(null, new KeyEventArgs(Keys.Enter));
        }
        private void txtcard_no_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            ArrayList tal = new ArrayList();

            string cardNo;
            Neusoft.HISFC.Models.Account.AccountCard accountCard = new Neusoft.HISFC.Models.Account.AccountCard();
            int ret = feeIntegrate.ValidMarkNO(txtcard_no.Text, ref accountCard);
            if (ret > 0)
            {
                cardNo = accountCard.Patient.PID.CardNO;
                txtcard_no.Text = cardNo;
            }
            else
            {
                cardNo = txtcard_no.Text;
            }

            //            string sql = @"select i.invoice_no, i.print_invoiceno, i.tot_cost, i.oper_date, i.name
            //                            from fin_opb_invoiceinfo i
            //                           where (i.card_no = '{0}' or i.name = '{0}')
            //                             --and i.oper_date >= sysdate - 30
            //                             and i.TRANS_TYPE = '1'
            //                             and i.cancel_flag = '1'
            //                           order by oper_date desc";


            //            sql = string.Format(sql, cardNo);
            string sql = @"select i.invoice_no, i.print_invoiceno, i.tot_cost, i.oper_date, i.name
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

                tal.Add(c);
            }
            Neusoft.HISFC.Models.Base.Const tobj = null;

            if (tal.Count == 0)
            {
                MessageBox.Show("没有需要打印的数据!");
                return;
            }
            else
            {
                FrameWork.WinForms.Forms.frmEasyChoose fc = new Neusoft.FrameWork.WinForms.Forms.frmEasyChoose(tal);
                string[] cols = { "发票电脑号", "发票印刷号", "金额", "日期", "姓名" };
                bool[] visibles = { true, true, true, true, true, false };
                int[] widths = { 80, 80, 80, 120, 60 };
                fc.SetFormat(cols, visibles, widths);
                fc.ShowDialog();

                tobj = fc.Object as Neusoft.HISFC.Models.Base.Const;
                if (tobj == null)
                {
                    MessageBox.Show("请选择一条记录!");
                    return;
                }

            }

            if (tobj.SpellCode == "已打印")
            {
                MessageBox.Show("该发票已经打印，不能再打印!");
                return;
            }

            // MessageBox.Show(tobj.ID);

            tbInvoiceNo.Text = tobj.ID;
            tbInvoiceNo_KeyDown(null, new KeyEventArgs(Keys.Enter));
        }

        #region 打印门诊收费确认单 20190923
        private void printAcountFeeDetail()
        {
            if (currentBalance == null)
            {
                return;
            }

            Iprint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject<IAccountFeeDetailPrint>(this.GetType());
            if (Iprint == null)
            {
                MessageBox.Show("请维护打印票据，查找打印票据失败！");
                return;
            }

            //currentBalance.Patient.PID.CardNO;
            //currentBalance.Invoice.ID;

            Iprint.SetValue(currentBalance.Patient.PID.CardNO, currentBalance.Invoice.ID);
            Iprint.Print();
        }
        #endregion
    }
}
