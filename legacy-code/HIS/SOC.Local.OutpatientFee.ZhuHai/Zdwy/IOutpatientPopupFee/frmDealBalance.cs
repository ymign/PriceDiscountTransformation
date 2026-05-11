using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.FrameWork.Models;
using Neusoft.FrameWork.Management;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.BizProcess.Interface.FeeInterface;
using FarPoint.Win.Spread;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.Net;
using Newtonsoft.Json;
using Neusoft.FrameWork.WinForms.Classes;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientPopupFee
{
    /// <summary>s
    /// 门诊收费结算
    /// </summary>
    public partial class frmDealBalance : Form, Neusoft.HISFC.BizProcess.Integrate.FeeInterface.IOutpatientPopupFee
    {
        public frmDealBalance()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 管理业务层
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// 费用业务层
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// 控制参数业务层
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParam = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        /// <summary>
        /// 工行支付接口
        /// </summary>
        public readonly GHpayService.Service GHService = new Neusoft.SOC.Local.OutpatientFee.ZhuHai.GHpayService.Service();

        /// <summary>
        /// 数据库事务
        /// </summary>
        private Neusoft.FrameWork.Management.Transaction trans;

        /// <summary>
        /// 数据库事务
        /// </summary>
        public Neusoft.FrameWork.Management.Transaction Trans
        {
            set
            {
                this.trans = value;
            }
        }

        /// <summary>
        /// 支付方式组合
        /// </summary>
        private ArrayList alPayModes = new ArrayList();

        /// <summary>
        /// 支付方式信息(用于调用收费)
        /// </summary>
        private ArrayList alPatientPayModeInfo = new ArrayList();

        /// <summary>
        /// 最小费用组合
        /// </summary>
        private ArrayList alMinFees = new ArrayList();

        /// <summary>
        /// 费用明细集合
        /// </summary>
        private ArrayList alFeeDetails = new ArrayList();

        /// <summary>
        /// 费用明细集合
        /// </summary>
        public ArrayList FeeDetails
        {
            set
            {
                alFeeDetails = value;
                this.SpliteMinFee();
            }
            get
            {
                return alFeeDetails;
            }
        }

        /// <summary>
        /// 发票集合
        /// </summary>
        private ArrayList alInvoices;

        private string posInvoice = "";
        /// <summary>
        /// 发票集合
        /// </summary>
        public ArrayList Invoices
        {
            set
            {
                this.alInvoices = value;
                if (this.alInvoices != null)
                {
                    this.fpSplit_Sheet1.RowCount = this.alInvoices.Count;
                    for (int i = 0; i < this.alInvoices.Count; i++)
                    {
                        Balance balance = this.alInvoices[i] as Balance;
                        this.fpSplit_Sheet1.Cells[i, 0].Text = balance.Invoice.ID;
                        posInvoice = balance.Invoice.ID;
                        this.fpSplit_Sheet1.Cells[i, 1].Text = balance.FT.TotCost.ToString();
                        string tmp = null;
                        switch (balance.Memo)
                        {
                            case "5":
                                tmp = "总发票";
                                break;
                            case "1":
                                tmp = "自费";
                                break;
                            case "2":
                                tmp = "记账";
                                break;
                            case "3":
                                tmp = "特殊";
                                break;
                            case "4":
                                tmp = "医保";
                                break;
                        }
                        this.fpSplit_Sheet1.Cells[i, 2].Text = tmp;
                        this.fpSplit_Sheet1.Cells[i, 2].Tag = balance.Memo;
                        this.fpSplit_Sheet1.Cells[i, 3].Text = balance.FT.OwnCost.ToString();
                        this.fpSplit_Sheet1.Cells[i, 4].Text = balance.FT.PayCost.ToString();
                        this.fpSplit_Sheet1.Cells[i, 5].Text = balance.FT.PubCost.ToString();
                        //发票主表
                        this.fpSplit_Sheet1.Rows[i].Tag = balance;
                        //发票明细
                        this.fpSplit_Sheet1.Cells[i, 0].Tag = ((ArrayList)alInvoiceDetails[i])[0] as ArrayList;
                        //费用明细
                        this.fpSplit_Sheet1.Cells[i, 3].Tag = ((ArrayList)InvoiceFeeDetails[i]) as ArrayList;
                    }
                }
            }
            get
            {
                return this.alInvoices;
            }
        }

        /// <summary>
        /// 发票明细结合
        /// </summary>
        private ArrayList alInvoiceDetails;

        /// <summary>
        /// 发票明细结合
        /// </summary>
        public ArrayList InvoiceDetails
        {
            set
            {
                this.alInvoiceDetails = value;
            }
            get
            {
                return this.alInvoiceDetails;
            }
        }

        /// <summary>
        /// 发票费用明细集合
        /// </summary>
        private ArrayList alInvoiceFeeDetails;

        public ArrayList InvoiceFeeDetails
        {
            set
            {
                this.alInvoiceFeeDetails = value;
            }
            get
            {
                return this.alInvoiceFeeDetails;
            }
        }

        /// <summary>
        /// 收费信息
        /// </summary>
        private Neusoft.HISFC.Models.Base.FT ftFeeInfo;

        /// <summary>
        /// 收费信息
        /// </summary>
        public Neusoft.HISFC.Models.Base.FT FTFeeInfo
        {
            get
            {
                return this.ftFeeInfo;
            }
        }

        /// <summary>
        /// 最小费用列表
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper helpMinFee = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 支付方式列表
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper helpPayMode = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 记账方式列表
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper helpPubType = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 合同单位对应支付方式帮助类
        /// </summary>
        private Neusoft.FrameWork.Public.ObjectHelper helpPactToPayModes = new Neusoft.FrameWork.Public.ObjectHelper();

        #region 金额

        /// <summary>
        /// 总金额
        /// </summary>
        private decimal totCost;

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotCost
        {
            set
            {
                this.totCost = value;
                this.tbTotCost.Text = totCost.ToString();
            }
            get
            {
                return totCost;
            }
        }

        /// <summary>
        /// 自费金额
        /// </summary>
        private decimal ownCost;

        /// <summary>
        /// 自费金额
        /// </summary>
        public decimal OwnCost
        {
            set
            {
                this.ownCost = value;
                this.tbOwnCost.Text = value.ToString();
            }
            get
            {
                return this.ownCost;
            }
        }

        /// <summary>
        /// 自付金额
        /// </summary>
        private decimal payCost;

        /// <summary>
        /// 自付金额
        /// </summary>
        public decimal PayCost
        {
            set
            {
                this.payCost = value;
                this.tbPayCost.Text = value.ToString();
            }
            get
            {
                return this.payCost;
            }
        }

        /// <summary>
        /// 就诊原因
        /// </summary>
        private string HKElderlyroll;

        /// <summary>
        /// 就诊原因
        /// </summary>
        public string HKELDERLYROLL
        {
            set
            {
                this.HKElderlyroll = value;
                this.tbJzyy.Text = value.ToString();
            }
            get
            {
                return this.HKElderlyroll;
            }
        }

        /// <summary>
        /// 中山民政统筹
        /// </summary>
        private decimal mzTCCost = 0;

        /// <summary>
        /// 中山民政统筹支付
        /// </summary>
        public decimal MzTCCost
        {
            get { return mzTCCost; }
            set
            {
                this.mzTCCost = value;
                if (mzTCCost != 0)
                {
                    this.fpPayMode_Sheet1.Cells["ZSMZ_Cost"].Text = value.ToString("F2");
                }
            }
        }

        /// <summary>
        /// 记账金额
        /// </summary>
        private decimal pubCost;

        /// <summary>
        /// 记账金额
        /// </summary>
        public decimal PubCost
        {
            set
            {
                this.pubCost = value;
                this.tbPubCost.Text = pubCost.ToString();
                //医保特殊处理(pub_cost)需要插入支付方式
                if (this.PatientInfo.Pact.PayKind.ID == "02")
                {
                    //if (this.alZhuHaiPactID.Contains(this.PatientInfo.Pact.ID) == true)
                    if (this.hsZhuHaiPactID.ContainsKey(this.PatientInfo.Pact.ID) == true)
                    {
                        this.fpPayMode_Sheet1.Cells["PBZH_Cost"].Text = value.ToString("F2");
                    }
                    //else if (this.alZhongShanPactID.Contains(this.PatientInfo.Pact.ID) == true)
                    else if (this.hsZhongShanPactID.ContainsKey(this.PatientInfo.Pact.ID) == true)
                    {
                        decimal tempPubCost = value - this.mzTCCost;
                        //this.fpPayMode_Sheet1.Cells["PBZS_Cost"].Text = value.ToString("F2");
                        this.fpPayMode_Sheet1.Cells["PBZS_Cost"].Text = tempPubCost.ToString("F2");
                    }
                    else if (this.PatientInfo.Pact.ID == "222" || this.PatientInfo.Pact.ID == "233" || this.PatientInfo.Pact.ID == "244" || this.PatientInfo.Pact.ID == "271" || this.PatientInfo.Pact.ID == "292")
                    {

                        this.fpPayMode_Sheet1.Cells["SNYDYBTC_Cost"].Text = value.ToString("F2");
                    }
                    else if (this.PatientInfo.Pact.ID == "223" || this.PatientInfo.Pact.ID == "224" || this.PatientInfo.Pact.ID == "225" || this.PatientInfo.Pact.ID == "302")
                    {

                        this.fpPayMode_Sheet1.Cells["SWYDYBTC_Cost"].Text = value.ToString("F2");
                    }
                    else
                    {
                        //为了保证该版本的pub_cost也插入到支付方式表中：凡是医保的报销金额，都需要赋值支付方式.
                        //没有维护的默认为【PBZH_Cost】珠海记账.
                        this.fpPayMode_Sheet1.Cells["PBZH_Cost"].Text = value.ToString("F2");
                    }
                }
            }
            get
            {
                return this.pubCost;
            }
        }

        /// <summary>
        /// 减免金额(优惠金额)
        /// </summary>
        private decimal rebateCost;

        /// <summary>
        /// 减免金额(优惠金额)
        /// </summary>
        public decimal RebateRate
        {
            set
            {
                this.rebateCost = value;
                this.tbRebateCost.Text = rebateCost.ToString();
                //由于已经调用Init()方法,可以直接将优惠赋值到支付方式界面
                if (this.fpPayMode_Sheet1.Cells["RC_Cost"] != null)
                {
                    this.fpPayMode_Sheet1.Cells["RC_Cost"].Value = value;
                }
            }
            get
            {
                return this.rebateCost;
            }
        }

        /// <summary>
        /// 应缴金额(=自费金额+自付金额)
        /// </summary>
        private decimal totOwnCost;

        /// <summary>
        /// 应缴金额(=自费金额+自付金额)
        /// </summary>
        public decimal TotOwnCost
        {
            set
            {
                this.totOwnCost = value;
                this.tbTotOwnCost.Text = value.ToString();
                this.tbRealCost.Text = value.ToString();
                this.tbRealCost.SelectAll();

                //根据合同单位默认相应的支付方式【常数：PACTTOPAYMODE】
                bool isCanFindPayMode = false;
                if (this.helpPactToPayModes != null && this.helpPactToPayModes.ArrayObject.Count > 0)
                {
                    Neusoft.FrameWork.Models.NeuObject obj = this.helpPactToPayModes.GetObjectFromID(this.PatientInfo.Pact.ID);
                    if (obj != null && !string.IsNullOrEmpty(obj.ID) && !string.IsNullOrEmpty(obj.Name))
                    {
                        for (int i = 0; i < this.fpPayMode_Sheet1.Rows.Count; i++)
                        {
                            string payID = this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Tag.ToString();
                            if (payID == obj.Name)
                            {
                                this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value = value;
                                isCanFindPayMode = true;
                                break;
                            }
                        }
                    }
                }

                if (!isCanFindPayMode)
                {
                    this.fpPayMode_Sheet1.Cells[0, (int)PayModeCols.Cost].Value = value;
                }

                //中山医保特殊处理(PAY_COST分离出来作为固定【社会保障卡(中山)】)
                if (this.hsZhongShanPactID.ContainsKey(this.PatientInfo.Pact.ID) == true)
                {
                    decimal MCZS_Cost = this.PatientInfo.SIMainInfo.PayCost;
                    this.fpPayMode_Sheet1.Cells["MCZS_Cost"].Text = MCZS_Cost.ToString("F2");
                }

            }
            get
            {
                return this.totOwnCost;
            }
        }

        /// <summary>
        /// 实付金额
        /// </summary>
        private decimal realCost;

        /// <summary>
        /// 实付金额
        /// </summary>
        public decimal RealCost
        {
            set
            {
                //中山医保特殊处理
                //if (this.alZhongShanPactID.Contains(this.PatientInfo.Pact.ID) == true)
                //if (this.hsZhongShanPactID.ContainsKey(this.PatientInfo.Pact.ID) == true)
                //{
                //    decimal MCZS_Cost = this.PatientInfo.SIMainInfo.PayCost;
                //    Neusoft.FrameWork.Models.NeuObject objGZZFUJE = this.PatientInfo.SIMainInfo.ExtendProperty["GZZFUJE"];
                //    Neusoft.FrameWork.Models.NeuObject objGZZFEJE = this.PatientInfo.SIMainInfo.ExtendProperty["GZZFEJE"];
                //    if (objGZZFUJE != null && Neusoft.FrameWork.Function.NConvert.ToDecimal(objGZZFUJE.Memo) > 0)
                //    {
                //        MCZS_Cost += Neusoft.FrameWork.Function.NConvert.ToDecimal(objGZZFUJE.Memo);
                //    }
                //    if (objGZZFEJE != null && Neusoft.FrameWork.Function.NConvert.ToDecimal(objGZZFEJE.Memo) > 9)
                //    {
                //        MCZS_Cost += Neusoft.FrameWork.Function.NConvert.ToDecimal(objGZZFEJE.Memo);
                //    }
                //    this.fpPayMode_Sheet1.Cells["MCZS_Cost"].Text = MCZS_Cost.ToString("F2");
                //}

                this.realCost = value;
            }
        }

        #endregion

        /// <summary>
        /// 患者合同单位信息
        /// </summary>
        private Neusoft.HISFC.Models.Base.PactInfo pactInfo = new Neusoft.HISFC.Models.Base.PactInfo();

        /// <summary>
        /// 患者合同单位信息
        /// </summary>
        public Neusoft.HISFC.Models.Base.PactInfo PactInfo
        {
            set
            {
                this.pactInfo = value;
                if ("01".Equals(value.PayKind.ID))//自费患者可以分票
                {
                    this.tpSplitInvoice.Show();
                }
                else
                {
                    this.tpSplitInvoice.Hide();
                }
            }
        }

        /// <summary>
        /// 患者挂号信息
        /// </summary>
        private Neusoft.HISFC.Models.Registration.Register rInfo = new Neusoft.HISFC.Models.Registration.Register();

        /// <summary>
        /// 患者挂号信息
        /// </summary>
        public Neusoft.HISFC.Models.Registration.Register PatientInfo
        {
            set
            {
                this.rInfo = value;
            }
            get
            {
                return this.rInfo;
            }
        }

        #region 新扫码墩开关 true 打开，false关闭
        /// <summary>
        /// 门诊收费新扫码墩开关 true 打开，false关闭
        /// </summary>
        private bool newScanCodeMZSF = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam().GetControlParam<bool>("newScanCodeMZSF", false, false);
        #endregion

        /// <summary>
        /// 扫码墩收款链接
        /// </summary>
        private string scanPaymentUrl = string.Empty;
        /// <summary>
        /// 扫码墩退款链接
        /// </summary>
        private string scanRefundUrl = string.Empty;


        #region 临时处理

        /// <summary>
        /// 珠海医保门诊合同单位
        /// </summary>
        //private ArrayList alZhuHaiPactID = ArrayList.Adapter(new string[]{ "4", "14", "19", "20", "21" });
        private Hashtable hsZhuHaiPactID = new Hashtable();

        /// <summary>
        /// 中山医保门诊合同单位
        /// </summary>
        //private ArrayList alZhongShanPactID = ArrayList.Adapter(new string[] { "18", "37", "38" });
        private Hashtable hsZhongShanPactID = new Hashtable();

        /// <summary>
        /// 允许手工输入优惠金额的合同单位
        /// </summary>
        private Hashtable hsCanInputRCMoneyPact = new Hashtable();

        #endregion

        #endregion

        #region 控件

        /// <summary>
        /// 记账方式选择列表
        /// </summary>
        private Neusoft.FrameWork.WinForms.Controls.PopUpListBox lbPubType = new Neusoft.FrameWork.WinForms.Controls.PopUpListBox();

        #endregion

        #region 属性

        /// <summary>
        /// 是否可以分发票
        /// </summary>
        private bool isCanSplit;

        /// <summary>
        /// 最多分发票张数
        /// </summary>
        private int splitCounts;

        /// <summary>
        /// 是否可以修改发票日期
        /// </summary>
        private bool isCanModifyInvoiceDate;

        /// <summary>
        /// 收费时是否可以修改发票打印日期
        /// </summary>
        private bool isCanModifyInvoicePrintDate;

        /// <summary>
        /// 是否系统处理银联交易
        /// </summary>
        private bool isAutoBankTrans = false;

        /// <summary>
        /// 是否系统处理银联交易
        /// </summary>
        public bool IsAutoBankTrans
        {
            set
            {
                isAutoBankTrans = value;
            }
        }

        /// <summary>
        /// 是否退费
        /// </summary>
        private bool isQuitFee = false;

        /// <summary>
        /// 是否退费
        /// </summary>
        public bool IsQuitFee
        {
            set
            {
                this.isQuitFee = value;
                if (this.isQuitFee == true)
                {
                    this.tbCharge.Enabled = false;
                }
            }
        }

        #endregion

        #region 状态

        /// <summary>
        /// 是否点击取消按钮
        /// </summary>
        private bool isPushCancelButton = false;

        /// <summary>
        /// 是否点击取消按钮
        /// </summary>
        public bool IsPushCancelButton
        {
            set
            {
                this.isPushCancelButton = value;
            }
            get
            {
                return this.isPushCancelButton;
            }
        }

        /// <summary>
        /// 是否结算成功（未退出）
        /// </summary>
        private bool isSuccessFee = false;

        /// <summary>
        /// 是否结算成功（未退出）
        /// </summary>
        public bool IsSuccessFee
        {
            set
            {
                this.isSuccessFee = value;
            }
            get
            {
                return this.isSuccessFee;
            }
        }

        /// <summary>
        /// 支付方式是否输入成功
        /// </summary>
        private bool isPaySuccess = false;

        #endregion

        #region 未使用

        /// <summary>
        /// 自费药金额[未使用]
        /// </summary>
        public decimal SelfDrugCost
        {
            set { }
        }

        /// <summary>
        /// 超标药金额[未使用]
        /// </summary>
        public decimal OverDrugCost
        {
            set { }
        }

        /// <summary>
        /// 找零金额[未使用]
        /// </summary>
        public decimal LeastCost
        {
            set { }
        }

        /// <summary>
        /// 银联接口[未使用]
        /// </summary>
        public Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBankTrans BankTrans
        {
            set { }
            get { return null; }
        }

        /// <summary>
        /// 发票和发票明细集合[未使用]
        /// </summary>
        public ArrayList InvoiceAndDetails
        {
            set { }
            get { return null; }
        }

        /// <summary>
        /// 是否现金冲账
        /// </summary>
        public bool IsCashPay
        {
            set { }
            get { return false; }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 实收金额改变触发
        /// </summary>
        public event Neusoft.HISFC.BizProcess.Integrate.FeeInterface.DelegateRealCost RealCostChange;

        /// <summary>
        /// 收费按钮触发
        /// </summary>
        public event Neusoft.HISFC.BizProcess.Integrate.FeeInterface.DelegateFee FeeButtonClicked;

        /// <summary>
        /// 划价按钮触发
        /// </summary>
        public event Neusoft.HISFC.BizProcess.Integrate.FeeInterface.DelegateChangeSomething ChargeButtonClicked;

        // Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        //private string icCard = "0";
        private void frmDealBalance_Load(object sender, EventArgs e)
        {
            //icCard = this.controlParamIntegrate.GetControlParam<string>("ICCard", true, "0");
            this.tbRealCost.Select();
            this.tbRealCost.Focus();
            this.tbRealCost.SelectAll();
            this.tbLeast.Text = "0";
            #region 扫码墩url初始化
            if (newScanCodeMZSF)
            {
                Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParams = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                this.scanPaymentUrl = controlParams.GetControlParam<string>("PT0003", false, string.Empty);
                this.scanRefundUrl = controlParams.GetControlParam<string>("PT0004", false, string.Empty);
            }
            #endregion
        }

        private int lbPubType_SelectItem(Keys key)
        {
            this.ProcessPubType();
            this.fpPubType.Focus();
            this.fpPubType_Sheet1.SetActiveCell(fpPubType_Sheet1.ActiveRowIndex, (int)PubTypes.Cost);
            return 1;
        }

        private void fpPubType_EditModeOn(object sender, EventArgs e)
        {
            if (this.fpPubType_Sheet1.ActiveColumnIndex == (int)PubTypes.PubType)
            {
                #region 下拉列表定位
                Control cell = this.fpPubType.EditingControl;
                this.lbPubType.Location = new Point(this.fpPubType.Location.X + cell.Location.X + 4,
                    this.panel1.Location.Y + this.tabControl1.Location.Y + this.fpPubType.Location.Y + cell.Location.Y + cell.Height * 2 + SystemInformation.Border3DSize.Height * 2);
                this.lbPubType.Size = new Size(cell.Width + 50 + SystemInformation.Border3DSize.Width * 2, 150);
                if (this.lbPubType.Location.Y + this.lbPubType.Height > this.fpPubType.Location.Y + this.fpPubType.Height)
                {
                    this.lbPubType.Location = new Point(this.fpPubType.Location.X + cell.Location.X + 4,
                        this.panel1.Location.Y + this.tabControl1.Location.Y + this.fpPubType.Location.Y + cell.Location.Y + cell.Height * 2 + SystemInformation.Border3DSize.Height * 2
                        - this.lbPubType.Size.Height - cell.Height);
                }
                #endregion
            }
            else
            {
                this.lbPubType.Visible = false;
            }
        }

        private void fpPubType_EditChange(object sender, EditorNotifyEventArgs e)
        {
            if (e.Column == (int)PubTypes.PubType)
            {
                string text = fpPubType_Sheet1.ActiveCell.Text.Trim();
                this.lbPubType.Filter(text);
                if (this.lbPubType.Visible == false)
                {
                    this.lbPubType.Visible = true;
                }
                this.fpPubType.Focus();
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {

            if (keyData == Keys.F12)
            {
                this.panel1.Focus();
                this.groupBox2.Focus();
                this.tbRealCost.Focus();
                this.tbRealCost.SelectAll();
            }
            else if (keyData == Keys.F4)
            {
                this.tabControl1.SelectedTab = this.tpPubType;
                this.tpPubType.Focus();
                this.fpPubType_Sheet1.ActiveRowIndex = 1;
                this.fpPubType_Sheet1.SetActiveCell(1, (int)PubTypes.PubType, false);
                this.fpPubType.EditMode = true;
            }
            else if (keyData == Keys.F5)
            {
                this.tabControl1.SelectedTab = this.tpSplitInvoice;
                this.tpSplitInvoice.Focus();
                this.tbCount.Focus();
            }
            else if (keyData == Keys.F6)
            {
                this.panel1.Focus();
                this.tabControl1.Focus();
                this.tabControl1.SelectedTab = this.tpPayMode;
                this.tpPayMode.Focus();
                this.fpPayMode.Focus();
                this.fpPayMode_Sheet1.ActiveRowIndex = 0;
                this.fpPayMode_Sheet1.SetActiveCell(0, (int)PayModeCols.Cost, false);
            }
            else if (keyData == Keys.Space) // 当敲击空格时所引
            {
                new Keystroke(Keys.None, Keys.None);
                this.panel1.Focus();
                this.tabControl1.Focus();
                this.tabControl1.SelectedTab = this.tpPayMode;
                this.tpPayMode.Focus();
                this.fpPayMode.Focus();
                this.fpPayMode_Sheet1.ActiveRowIndex = 0;
                int figure2 = int.Parse(tbCardNO.Text); // 将输入的数字进行转换
                this.fpPayMode_Sheet1.SetActiveCell(figure2 - 1, (int)PayModeCols.Cost, false);
            }
            else if (keyData == Keys.Escape)
            {
                if (this.lbPubType.Visible)
                {
                    this.lbPubType.Visible = false;
                    this.fpPubType.StopCellEditing();
                }
                else
                {
                    this.tbRealCost.Focus();
                    this.isPushCancelButton = true;
                    this.Close();
                }
            }
            else if (this.fpPubType.ContainsFocus)
            {
                if (keyData == Keys.Up)
                {
                    if (this.fpPubType.Visible == true)
                    {
                        this.lbPubType.PriorRow();
                    }
                }
                else if (keyData == Keys.Down)
                {
                    if (this.fpPubType.Visible == true)
                    {
                        this.lbPubType.NextRow();
                    }
                }
                else if (keyData == Keys.Enter)
                {
                    int curRow = this.fpPubType_Sheet1.ActiveRowIndex;
                    int curCol = this.fpPubType_Sheet1.ActiveColumnIndex;
                    this.fpPubType.StopCellEditing();
                    if (curCol == (int)PubTypes.PubType)
                    {
                        this.ProcessPubType();
                        this.fpPubType_Sheet1.SetActiveCell(curRow, (int)PubTypes.Cost, false);
                    }
                    else if (curCol == (int)PubTypes.Cost)
                    {
                        decimal cost = NConvert.ToDecimal(this.fpPubType_Sheet1.Cells[curRow, (int)PubTypes.Cost].Value);
                        if (cost < 0)
                        {
                            MessageBox.Show("金额不能小于零");
                            this.fpPubType.Focus();
                            this.fpPubType_Sheet1.SetActiveCell(curCol, (int)PubTypes.Cost, false);
                            return false;
                        }
                        this.fpPubType_Sheet1.SetActiveCell(curRow, (int)PubTypes.Mark, false);
                    }
                    else if (curCol == (int)PubTypes.Mark)
                    {
                        decimal cost = NConvert.ToDecimal(this.fpPubType_Sheet1.Cells[curRow, (int)PubTypes.Cost].Value);

                        //金额记录入记账支付方式 ?通过支付方式的名字来确定，如果修改名字，则有风险 gumzh?
                        int rowIndex = this.GetRowIndexByName(fpPayMode_Sheet1, "记账");
                        this.fpPayMode_Sheet1.Cells[rowIndex, (int)PayModeCols.Cost].Text = cost.ToString();
                        this.fpPayMode_Sheet1.Cells[rowIndex, (int)PayModeCols.Cost].Locked = true;

                        this.tabControl1.SelectedTab = this.tpPayMode;
                        this.fpPayMode_Sheet1.SetActiveCell(0, (int)PayModeCols.Cost);
                    }
                }
            }
            else if (this.fpPayMode.ContainsFocus)
            {
                if (keyData == Keys.Enter)
                {
                    int curRow = this.fpPayMode_Sheet1.ActiveRowIndex;
                    int curCol = this.fpPayMode_Sheet1.ActiveColumnIndex;
                    if (curCol == (int)PayModeCols.Cost)
                    {
                        decimal cost = NConvert.ToDecimal(this.fpPayMode_Sheet1.Cells[curRow, (int)PayModeCols.Cost].Value);
                        if (cost < 0)
                        {
                            MessageBox.Show("金额不能小于零");
                            this.fpPayMode.Focus();
                            this.fpPayMode_Sheet1.SetActiveCell(curRow, (int)PayModeCols.Cost, false);
                            return false;
                        }
                        else
                        {
                            if (curRow == 0)
                            {
                                this.fpPayMode_Sheet1.SetActiveCell(curRow + 2, (int)PayModeCols.Cost, false);
                            }
                            else
                            {
                                this.fpPayMode_Sheet1.SetActiveCell(curRow + 1, (int)PayModeCols.Cost, false);
                            }
                        }

                    }
                }
            }
            else if (this.tbRealCost.ContainsFocus)
            {
                #region 跳转到找零

                if (keyData == Keys.Enter)
                {
                    this.tbLeast.Focus(); //设置为焦点
                    this.tbLeast.SelectAll();
                }

                #endregion
            }
            else if (this.tbLeast.ContainsFocus)
            {
                #region 确认收费

                if (keyData == Keys.Enter)
                {
                    if (NConvert.ToDecimal(this.tbLeast.Text) < 0)
                    {
                        MessageBox.Show("找零金额小于0，请注意!");
                        this.tbRealCost.Focus();
                        this.tbRealCost.SelectAll();
                    }
                    else
                    {
                        this.tbFee.Focus(); //设置为焦点
                    }
                }

                #endregion
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 点击收费按钮触发
        /// 微信或支付宝支付直接走扫码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbFee_Click(object sender, EventArgs e)
        {
            this.Tag = "收费";
            this.tbFee.Enabled = false;
            alPatientPayModeInfo = QueryBalancePays();
            foreach (BalancePay p in alPatientPayModeInfo)
            {
                if (p.PayType.ID == "WX" || p.PayType.ID == "ZFB" || p.PayType.ID == "GHZFB" || p.PayType.ID == "GHWX" || p.PayType.ID == "GHRMB")
                {
                    tbQrCodeFee_Click(sender, e);
                    return;
                }
            }
            if (this.CheckPayMode() == false)
            {
                this.tbFee.Enabled = true;
                return;
            }

            if (this.isPaySuccess == false)
            {
                this.tbFee.Enabled = true;
                return;
            }

            this.isPaySuccess = false;

            if (this.SaveFee() == false)
            {
                this.tbFee.Enabled = true;
                return;
            }

            this.tbFee.Enabled = true;
            this.tbRealCost.Focus();

            this.isSuccessFee = true;

            this.Close();
        }

        /// <summary>
        /// 划价保存按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbCharge_Click(object sender, EventArgs e)
        {
            this.Tag = "划价保存";
            this.tbCharge.Enabled = false;
            this.SaveCharge();
            this.tbCharge.Enabled = true;
            this.tbRealCost.Focus();
            this.Close();
        }

        /// <summary>
        /// 取消按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbCancel_Click(object sender, EventArgs e)
        {
            this.Tag = "取消";
            this.isPushCancelButton = true;
            this.tbRealCost.Focus();
            this.Close();
        }

        /// <summary>
        /// 分发票默认按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbDefault_Click(object sender, EventArgs e)
        {
            this.Invoices = this.alInvoices;
        }

        private void fpPayMode_Sheet1_CellChanged(object sender, SheetViewEventArgs e)
        {
            this.CheckPayMode();
        }

        private void tbTotOwnCost_TextChanged(object sender, EventArgs e)
        {
            this.tbRealCost.Text = this.tbTotOwnCost.Text;
        }

        /// <summary>
        /// 找零金额-实付金额 - 现金
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbRealCost_TextChanged(object sender, EventArgs e)
        {
            decimal casCost = NConvert.ToDecimal(this.fpPayMode_Sheet1.Cells[0, (int)PayModeCols.Cost].Value);  //现金
            //this.tbLeast.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(NConvert.ToDecimal(this.tbRealCost.Text) - NConvert.ToDecimal(this.tbTotOwnCost.Text), 2);
            this.tbLeast.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(NConvert.ToDecimal(this.tbRealCost.Text) - casCost, 2);
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            //##未完全测试
            int row = this.fpSplit_Sheet1.ActiveRowIndex;
            if (this.fpSplit_Sheet1.RowCount <= 0)
            {
                return;
            }

            string tempType = this.fpSplit_Sheet1.Cells[row, 2].Tag.ToString();

            if (tempType != "1")//只有自费发票可以分票
            {
                return;
            }
            string beginInvoiceNo = this.fpSplit_Sheet1.Cells[row, 0].Text;
            string beginRealInvoiceNo = "";
            Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice = null;
            ArrayList invoiceDetails = null;
            try
            {
                invoice = this.fpSplit_Sheet1.Rows[row].Tag as Neusoft.HISFC.Models.Fee.Outpatient.Balance;
                beginRealInvoiceNo = invoice.PrintedInvoiceNO;
                invoiceDetails = this.fpSplit_Sheet1.Cells[row, 0].Tag as ArrayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            ucSplitInvoice split = new ucSplitInvoice();

            int count = 0;
            try
            {
                count = Convert.ToInt32(this.tbCount.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入分票得数量不合法" + ex.Message);
                this.tbCount.Focus();
                this.tbCount.SelectAll();
                return;
            }
            if (count > this.splitCounts)
            {
                MessageBox.Show("当前可分发票数不能大于: " + splitCounts.ToString());
                this.tbCount.Focus();
                this.tbCount.SelectAll();
                return;
            }
            if (count <= 0)
            {
                MessageBox.Show("当前可分发票数不能小于或等于0");
                this.tbCount.Focus();
                this.tbCount.SelectAll();
                return;
            }
            int days = 0;
            try
            {
                days = Convert.ToInt32(this.tbSplitDay.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入间隔天数不合法" + ex.Message);
                this.tbSplitDay.Focus();
                this.tbSplitDay.SelectAll();
                return;
            }
            if (days > 999)
            {
                MessageBox.Show("间隔天数不能大于999天!");
                this.tbSplitDay.Focus();
                this.tbSplitDay.SelectAll();
                return;
            }
            string invoiceNoType = this.controlParam.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.GET_INVOICE_NO_TYPE, true, "0");

            if (invoiceNoType == "2" && this.fpSplit_Sheet1.RowCount > 1)
            {
                MessageBox.Show("已经存在分票记录,如果要继续分票,请点击默认按钮,重新分配!");
                this.tbSplitDay.Focus();
                this.tbSplitDay.SelectAll();

                return;
            }

            this.btnSplit.Focus();
            split.Count = count;
            split.Days = days;
            split.InvoiceType = tempType;
            split.InvoiceNoType = invoiceNoType;
            split.BeginInvoiceNo = beginInvoiceNo;
            split.BeginRealInvoiceNo = beginRealInvoiceNo;
            split.Invoice = invoice;
            split.InvoiceDetails = invoiceDetails;
            split.AddInvoiceUnits(count, this.rbAuto.Checked ? "1" : "0");
            split.IsAuto = this.rbAuto.Checked;
            Form frmTemp = new Form();
            split.Dock = DockStyle.Fill;
            frmTemp.Controls.Add(split);
            frmTemp.Text = "分发票";
            frmTemp.WindowState = FormWindowState.Maximized;
            frmTemp.ShowDialog(this);

            if (!split.IsConfirm)
            {
                return;
            }

            this.dateTimePicker1.Enabled = false;//分过发票之后不允许再通过收费界面修改发票日期

            ArrayList splitInvoices = split.SplitInvoices;
            ArrayList splitInvoiceDetails = split.SplitInvoiceDetails;


            this.fpSplit_Sheet1.Rows.Add(row + 1, splitInvoices.Count);
            for (int i = 0; i < splitInvoices.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Outpatient.Balance invoiceTemp = splitInvoices[i] as Neusoft.HISFC.Models.Fee.Outpatient.Balance;
                this.fpSplit_Sheet1.Cells[row + 1 + i, 0].Text = invoiceTemp.Invoice.ID;
                this.fpSplit_Sheet1.Cells[row + 1 + i, 1].Text = invoiceTemp.FT.TotCost.ToString();
                string tmp = null;
                switch (invoiceTemp.Memo)
                {
                    case "5":
                        tmp = "总发票";
                        break;
                    case "1":
                        tmp = "自费";
                        break;
                    case "2":
                        tmp = "记账";
                        break;
                    case "3":
                        tmp = "特殊";
                        break;
                }
                this.fpSplit_Sheet1.Cells[row + 1 + i, 2].Text = tmp;
                this.fpSplit_Sheet1.Cells[row + 1 + i, 2].Tag = invoiceTemp.Memo;
                this.fpSplit_Sheet1.Cells[row + 1 + i, 3].Text = invoiceTemp.FT.OwnCost.ToString();
                this.fpSplit_Sheet1.Cells[row + 1 + i, 4].Text = invoiceTemp.FT.PayCost.ToString();
                this.fpSplit_Sheet1.Cells[row + 1 + i, 5].Text = invoiceTemp.FT.PubCost.ToString();
                this.fpSplit_Sheet1.Rows[row + 1 + i].Tag = invoiceTemp;
                this.fpSplit_Sheet1.Cells[row + 1 + i, 0].Tag = ((ArrayList)splitInvoiceDetails[i]) as ArrayList;
            }
            this.fpSplit_Sheet1.Rows.Remove(row, 1);
            for (int i = row + splitInvoices.Count; i < this.fpSplit_Sheet1.RowCount; i++)
            {
                Neusoft.HISFC.Models.Fee.Outpatient.Balance tempInvoice =
                    this.fpSplit_Sheet1.Rows[i].Tag as Neusoft.HISFC.Models.Fee.Outpatient.Balance;

                string nextInvoiceNo = ""; string nextRealInvoiceNo = ""; string errText = "";

                if (invoiceNoType == "2")//普通模式需要Trans支持
                {
                    Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                    this.feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                    int iReturn = this.feeIntegrate.GetNextInvoiceNO(invoiceNoType, tempInvoice.Invoice.ID, tempInvoice.PrintedInvoiceNO, ref nextInvoiceNo, ref nextRealInvoiceNo, splitInvoices.Count - 1, ref errText);
                    if (iReturn < 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(errText);

                        return;
                    }

                    Neusoft.FrameWork.Management.PublicTrans.RollBack();//因为此时不一定插入数据库,所以回滚,保持发票不跳号
                }
                else
                {

                    int iReturn = this.feeIntegrate.GetNextInvoiceNO(invoiceNoType, tempInvoice.Invoice.ID, tempInvoice.PrintedInvoiceNO, ref nextInvoiceNo, ref nextRealInvoiceNo, splitInvoices.Count - 1, ref errText);
                    if (iReturn < 0)
                    {
                        MessageBox.Show(errText);
                        return;
                    }
                }
                tempInvoice.Invoice.ID = nextInvoiceNo;
                tempInvoice.PrintedInvoiceNO = nextRealInvoiceNo;

                this.fpSplit_Sheet1.Cells[i, 0].Text = tempInvoice.Invoice.ID;
                this.fpSplit_Sheet1.Rows[i].Tag = tempInvoice;
                ArrayList alTemp = this.fpSplit_Sheet1.Cells[i, 0].Tag as ArrayList;
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.BalanceList detail in alTemp)
                {
                    detail.BalanceBase.Invoice.ID = tempInvoice.Invoice.ID;
                }
                this.fpSplit_Sheet1.Cells[i, 0].Tag = alTemp;
            }
        }

        private void tbCount_KeyDown(object sender, KeyEventArgs e)
        {
            //##未完全测试
            if (e.KeyCode == Keys.Enter)
            {
                int count = 0;
                try
                {
                    count = Convert.ToInt32(this.tbCount.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("输入分票得数量不合法" + ex.Message);
                    this.tbCount.Focus();
                    this.tbCount.SelectAll();

                    return;
                }
                if (count > this.splitCounts)
                {
                    MessageBox.Show("当前可分发票数不能大于: " + splitCounts.ToString());
                    this.tbCount.Focus();
                    this.tbCount.SelectAll();

                    return;
                }

                this.tbSplitDay.Focus();
                this.tbSplitDay.SelectAll();
            }
        }

        private void tbSplitDay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int count = 0;
                try
                {
                    count = Convert.ToInt32(this.tbSplitDay.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("输入间隔天数不合法" + ex.Message);
                    this.tbSplitDay.Focus();
                    this.tbSplitDay.SelectAll();
                    return;
                }
                if (count > 999)
                {
                    MessageBox.Show("间隔天数不能大于999天!");
                    this.tbSplitDay.Focus();
                    this.tbSplitDay.SelectAll();
                    return;
                }

                btnSplit.Focus();
            }
        }

        private void fpSplit_CellDoubleClick(object sender, CellClickEventArgs e)
        {
            this.PreViewInvoice();
        }

        #endregion

        #region 列枚举

        /// <summary>
        /// 支付方式列枚举
        /// </summary>
        enum PayModeCols
        {
            /// <summary>
            /// 支付方式
            /// </summary>
            PayMode = 0,
            /// <summary>
            /// 金额
            /// </summary>
            Cost = 1,
            /// <summary>
            /// 开户银行
            /// </summary>
            Bank = 2,
            /// <summary>
            /// 帐号
            /// </summary>
            Account = 3,
            /// <summary>
            /// 开据单位
            /// </summary>
            Company = 4,
            /// <summary>
            /// 支票，汇票，交易号
            /// </summary>
            PosNo = 5
        }

        /// <summary>
        /// 记账方式列枚举
        /// </summary>
        enum PubTypes
        {
            /// <summary>
            /// 记账方式
            /// </summary>
            PubType = 0,
            /// <summary>
            /// 金额
            /// </summary>
            Cost = 1,
            /// <summary>
            /// 备注
            /// </summary>
            Mark = 2
        }

        #endregion

        #region 方法

        private void PreViewInvoice()
        {
            //##未完全测试
            int row = this.fpSplit_Sheet1.ActiveRowIndex;
            if (this.fpSplit_Sheet1.RowCount <= 0)
            {
                return;
            }

            Balance invoicePreView = this.fpSplit_Sheet1.Rows[row].Tag as Balance;
            ArrayList invoiceDetailsPreview = this.fpSplit_Sheet1.Cells[row, 0].Tag as ArrayList;
            ArrayList InvoiceFeeDetailsPreview = this.fpSplit_Sheet1.Cells[row, 3].Tag as ArrayList;

            Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint iInvoicePrint = null;

            string returnValue = controlParam.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, false, string.Empty);
            if (string.IsNullOrEmpty(returnValue))
            {
                iInvoicePrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint;
            }
            else
            {
                returnValue = Application.StartupPath + returnValue;
                try
                {
                    Assembly a = Assembly.LoadFrom(returnValue);
                    Type[] types = a.GetTypes();
                    foreach (System.Type type in types)
                    {
                        if (type.GetInterface("IInvoicePrint") != null)
                        {
                            iInvoicePrint = System.Activator.CreateInstance(type) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint;

                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("初始化发票失败!" + e.Message);

                    return;
                }
            }

            if (iInvoicePrint == null)
            {
                MessageBox.Show("请维护打印票据，查找打印票据失败！");
                return;
            }

            try
            {
                if (this.trans != null)
                {
                    iInvoicePrint.Trans = Neusoft.FrameWork.Management.PublicTrans.Trans;
                }
                iInvoicePrint.SetPrintValue(this.rInfo, invoicePreView, invoiceDetailsPreview, alFeeDetails, alPatientPayModeInfo, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return;
            }

            Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl((Control)iInvoicePrint);
        }

        private bool CheckPayMode()
        {
            decimal sumCost = 0m;
            decimal shouldPay = 0m;//应缴金额
            for (int i = 0; i < this.fpPayMode_Sheet1.RowCount; i++)
            {
                if (this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text != string.Empty)
                {
                    if (this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text != "现金" &&
                         this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text != "珠海医保统筹" &&
                         this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text != "中山医保统筹" &&
                         this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text != "中山民政支付" && this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text != "省内异地医保统筹" && this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text != "省外异地医保统筹"
                        )
                    {
                        //统计除 【现金 和 统筹金额(pub_cost)】 之外的金额
                        sumCost += NConvert.ToDecimal(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value);
                    }
                }
            }
            if (realCost > sumCost)
            {
                //把剩余支付金额归入现金
                this.fpPayMode_Sheet1.Cells[0, (int)PayModeCols.Cost].Value = realCost - sumCost;
            }
            else if (realCost < sumCost)
            {
                //提示错误
                MessageBox.Show("请核准支付方式金额,总应付金额：" + realCost.ToString());
                return false;
            }
            else
            {
                this.fpPayMode_Sheet1.Cells[0, (int)PayModeCols.Cost].Value = 0;
            }

            for (int i = 0; i < this.fpPayMode_Sheet1.RowCount; i++)
            {
                if (this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked == false && this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value != null)
                {
                    shouldPay += Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value.ToString());
                }
            }
            this.tbTotOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(shouldPay, 2);
            this.isPaySuccess = true;
            return true;
        }

        private Neusoft.HISFC.Models.Base.FT GetFT()
        {
            Neusoft.HISFC.Models.Base.FT feeInfo = new Neusoft.HISFC.Models.Base.FT();
            feeInfo.TotCost = totCost;
            feeInfo.OwnCost = ownCost;
            feeInfo.PayCost = payCost;
            feeInfo.PubCost = pubCost;
            feeInfo.BalancedCost = NConvert.ToDecimal(this.tbTotOwnCost.Text);
            feeInfo.SupplyCost = NConvert.ToDecimal(this.tbTotOwnCost.Text);
            feeInfo.RealCost = NConvert.ToDecimal(this.tbRealCost.Text);
            feeInfo.ReturnCost = NConvert.ToDecimal(this.tbLeast.Text);
            this.ftFeeInfo = feeInfo;
            return feeInfo;
        }

        /// <summary>
        /// 处理记账方式列表回车
        /// </summary>
        /// <returns></returns>
        private int ProcessPubType()
        {
            int currRow = this.fpPubType_Sheet1.ActiveRowIndex;
            if (currRow < 0)
            {
                return 0;
            }
            NeuObject item = null;

            int returnValue = this.lbPubType.GetSelectedItem(out item);
            if (returnValue == -1 || item == null)
            {
                return -1;
            }

            fpPubType_Sheet1.SetValue(currRow, (int)PubTypes.PubType, item.Name);
            fpPubType.StopCellEditing();

            this.lbPubType.Visible = false;

            return 0;
        }

        /// <summary>
        /// 归类最小费用
        /// </summary>
        private void SpliteMinFee()
        {
            Hashtable htMinFee = new Hashtable();
            foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList in alFeeDetails)
            {
                string minFeeName = string.Empty;
                if (htMinFee.ContainsKey(feeItemList.Item.MinFee.ID) == false)
                {
                    Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = feeItemList.Item.MinFee.ID;
                    obj.Memo = Neusoft.FrameWork.Public.String.FormatNumber(feeItemList.FT.TotCost, 2).ToString();
                    if (htMinFee.ContainsKey(obj.ID) == false)
                    {
                        obj.Name = this.managerIntegrate.GetConstansObj("MINFEE", obj.ID).Name;
                    }
                    else
                    {
                        obj.Name = this.helpMinFee.GetObjectFromID(obj.ID).Name;
                    }
                    minFeeName = obj.Name;
                    htMinFee.Add(obj.ID, obj);
                }
                else
                {
                    Neusoft.FrameWork.Models.NeuObject obj = htMinFee[feeItemList.Item.MinFee.ID] as Neusoft.FrameWork.Models.NeuObject;
                    minFeeName = obj.Name;
                    obj.Memo = Neusoft.FrameWork.Public.String.FormatNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(obj.Memo) + feeItemList.FT.TotCost, 2).ToString();
                    htMinFee.Remove(obj.ID);
                    htMinFee.Add(obj.ID, obj);
                }
            }
            foreach (DictionaryEntry entry in htMinFee)
            {
                this.alMinFees.Add(entry.Value);
            }

            //设置界面显示
            if (this.fpSpread1_Sheet1.Rows.Count > 0)
            {
                this.fpSpread1_Sheet1.Rows.Remove(0, this.fpSpread1_Sheet1.Rows.Count);
            }

            if (this.alMinFees.Count > 0)
            {
                this.fpSpread1_Sheet1.Rows.Add(0, alMinFees.Count / 4 + 1);
            }
            for (int i = 0; i < alMinFees.Count; i++)
            {
                this.fpSpread1_Sheet1.Cells[i / 4, 2 * (i % 4)].Text = (alMinFees[i] as Neusoft.FrameWork.Models.NeuObject).Name;
                this.fpSpread1_Sheet1.Cells[i / 4, 2 * (i % 4) + 1].Text = (alMinFees[i] as Neusoft.FrameWork.Models.NeuObject).Memo;
            }
        }

        /// <summary>
        /// 初始化支付方式信息
        /// </summary>
        private int InitPayMode()
        {
            ArrayList tempPayModes = this.managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.PAYMODES);
            this.helpPayMode.ArrayObject = tempPayModes;
            if (tempPayModes == null || tempPayModes.Count == 0)
            {
                MessageBox.Show("获取支付方式列表错误");
                return -1;
            }
            //至少保留现金和记账支付方式
            Neusoft.FrameWork.Models.NeuObject objCA = new Neusoft.FrameWork.Models.NeuObject();
            objCA.ID = "CA";
            objCA.Name = "现金";

            Neusoft.FrameWork.Models.NeuObject objPB = new Neusoft.FrameWork.Models.NeuObject();
            objPB.ID = "PB";
            objPB.Name = "记账";

            if (helpPayMode.GetObjectFromID(objCA.ID) == null)
            {
                helpPayMode.ArrayObject.Add(objCA);
            }

            if (helpPayMode.GetObjectFromID(objPB.ID) == null)
            {
                helpPayMode.ArrayObject.Add(objPB);
            }

            //支付方式列表改为固定模式
            this.fpPayMode_Sheet1.RowCount = tempPayModes.Count;
            for (int i = 0; i < tempPayModes.Count; i++)
            {
                Neusoft.FrameWork.Models.NeuObject obj = tempPayModes[i] as Neusoft.FrameWork.Models.NeuObject;
                this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Tag = obj.ID;
                this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text = obj.Name;
                this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Locked = true;

                if ("SNYDYBTC".Equals(obj.ID))
                {
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked = true;
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Tag = "SNYDYBTC_Cost";
                }
                if ("SWYDYBTC".Equals(obj.ID))
                {
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked = true;
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Tag = "SWYDYBTC_Cost";
                }
                if ("PB".Equals(obj.ID))
                {
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked = true;
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Tag = "PB_Cost";
                }
                if ("RC".Equals(obj.ID))
                {
                    //if (!this.hsCanInputRCMoneyPact.Contains(this.PatientInfo.Pact.ID))
                    if (!(this.PatientInfo.Pact.PactDllName == "OwnFee.dll"))
                    {
                        this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked = true;
                        this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked = false;//2016-10-17 cxw认为不作控制 chengym
                        this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Tag = "RC_Cost";
                    }
                }
                if ("PBZH".Equals(obj.ID))
                {
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked = true;
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Tag = "PBZH_Cost";
                }
                if ("PBZS".Equals(obj.ID))
                {
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked = true;
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Tag = "PBZS_Cost";
                }
                if ("MCZS".Equals(obj.ID))
                {
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked = true;
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Tag = "MCZS_Cost";
                }
                if ("ZSMZ".Equals(obj.ID))
                {
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Locked = true;
                    this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Tag = "ZSMZ_Cost";
                }
            }

            return 1;
        }

        /// <summary>
        /// 初始化分发票信息
        /// </summary>
        /// <returns></returns>
        private int InitSplitInvoice()
        {
            string tmpCtrlValue = feeIntegrate.GetControlValue(Neusoft.HISFC.BizProcess.Integrate.Const.CANSPLIT, "0");
            if (string.IsNullOrEmpty(tmpCtrlValue) || "1".Equals(tmpCtrlValue) == false)
            {
                MessageBox.Show("是否分发票参数没有维护，现在采用默认值: 不可分发票!");
                tmpCtrlValue = "0";
            }

            this.isCanSplit = Neusoft.FrameWork.Function.NConvert.ToBoolean(tmpCtrlValue);

            this.rbAuto.Enabled = isCanSplit;
            this.rbMun.Enabled = isCanSplit;
            this.tbCount.Enabled = isCanSplit;
            this.btnSplit.Enabled = isCanSplit;
            this.tbDefault.Enabled = isCanSplit;

            this.splitCounts = this.controlParam.GetControlParam<int>(Neusoft.HISFC.BizProcess.Integrate.Const.SPLITCOUNTS, false, 9);
            this.isCanModifyInvoiceDate = this.controlParam.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.CAN_MODIFY_INVOICE_DATE, false, false);

            if (isCanModifyInvoiceDate == false)
            {
                this.tbSplitDay.Text = "0";
                this.tbSplitDay.Enabled = false;
            }
            else
            {
                this.tbSplitDay.Text = "1";
                this.tbSplitDay.Enabled = true;
            }

            return 1;
        }

        private int InitPubType()
        {
            ArrayList tempPubTypes = this.managerIntegrate.GetConstantList("PUBTYPE");
            this.helpPubType.ArrayObject = tempPubTypes;
            if (tempPubTypes == null || tempPubTypes.Count == 0)
            {
                MessageBox.Show("获取支付方式列表错误");
                return -1;
            }
            this.lbPubType.AddItems(tempPubTypes);
            this.Controls.Add(this.lbPubType);
            this.lbPubType.Hide();
            this.lbPubType.BorderStyle = BorderStyle.FixedSingle;
            this.lbPubType.BringToFront();
            this.lbPubType.SelectItem += new Neusoft.FrameWork.WinForms.Controls.PopUpListBox.MyDelegate(lbPubType_SelectItem);

            return 1;
        }

        /// <summary>
        /// 初始化farpoint,屏蔽一些热键
        /// </summary>
        private void InitFp()
        {
            InputMap im;

            #region 记账方式FP

            im = this.fpPubType.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.Down, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.Up, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.Escape, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.Back, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.F4, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.F5, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.F6, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            #endregion

            #region 支付方式FP

            im = this.fpPayMode.GetInputMap(InputMapMode.WhenAncestorOfFocused);
            im.Put(new Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            //im.Put(new Keystroke(Keys.Down, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            //im.Put(new Keystroke(Keys.Up, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            //im.Put(new Keystroke(Keys.Escape, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            //im.Put(new Keystroke(Keys.Back, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.F4, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.F5, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            im.Put(new Keystroke(Keys.F6, Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            #endregion
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public int Init()
        {

            #region 允许手工输入优惠金额的合同单位

            try
            {
                //临时处理
                this.hsCanInputRCMoneyPact = new Hashtable();
                hsCanInputRCMoneyPact.Add("5", null);
                hsCanInputRCMoneyPact.Add("9", null);
            }
            catch (Exception ex)
            { }

            #endregion

            //初始化FarPoint信息
            this.InitFp();

            //初始化最小费用列表
            this.helpMinFee.ArrayObject = this.managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.MINFEE);

            //初始化支付方式列表
            if (this.InitPayMode() < 0)
            {
                return -1;
            }

            //初始化记账方式列表
            if (this.InitPubType() < 0)
            {
                return -1;
            }

            //初始化分发票
            if (this.InitSplitInvoice() < 0)
            {
                return -1;
            }

            //是否可以修改发票打印日期
            this.isCanModifyInvoiceDate = this.controlParam.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.MODIFY_INVOICE_PRINTDATE, false, false);
            if (this.isCanModifyInvoicePrintDate == true)
            {
                this.dateTimePicker1.Enabled = true;
            }
            else
            {
                this.dateTimePicker1.Enabled = false;
            }

            //优化，放进内存中??
            #region 合同单位对应支付方式

            try
            {
                ArrayList al = this.managerIntegrate.GetConstantList("PACTTOPAYMODE");
                if (al != null && al.Count > 0)
                {
                    this.helpPactToPayModes.ArrayObject = al;
                }
            }
            catch (Exception ex)
            { }

            #endregion

            #region 查找【珠海医保】和【中山医保】的合同单位，根据待遇算法DLL来作为查询条件。

            try
            {
                //?如果修改接口名字的情况，这里要修改 gumzh?
                Neusoft.HISFC.BizLogic.Fee.PactUnitInfo pactMgr = new Neusoft.HISFC.BizLogic.Fee.PactUnitInfo();
                ArrayList alZH = pactMgr.QueryPactUnitByDLLName("ZhuHaiSI.dll");
                ArrayList alZS = pactMgr.QueryPactUnitByDLLName("ZhongShanSI.dll");

                //珠海社保
                if (alZH != null && alZH.Count > 0)
                {
                    this.hsZhuHaiPactID = new Hashtable();
                    foreach (Neusoft.FrameWork.Models.NeuObject obj in alZH)
                    {
                        if (!this.hsZhuHaiPactID.ContainsKey(obj.ID))
                        {
                            this.hsZhuHaiPactID.Add(obj.ID, obj);
                        }
                    }
                }
                //中山医保
                if (alZS != null && alZS.Count > 0)
                {
                    this.hsZhongShanPactID = new Hashtable();
                    foreach (Neusoft.FrameWork.Models.NeuObject obj in alZS)
                    {
                        if (!this.hsZhongShanPactID.ContainsKey(obj.ID))
                        {
                            this.hsZhongShanPactID.Add(obj.ID, obj);
                        }
                    }
                }

            }
            catch (Exception ex)
            { }

            #endregion

            return 1;
        }

        /// <summary>
        /// 保存划价信息
        /// </summary>
        /// <returns></returns>
        public bool SaveCharge()
        {
            this.ChargeButtonClicked();
            return true;
        }

        /// <summary>
        /// 保存收费信息##
        /// </summary>
        /// <returns></returns>
        public bool SaveFee()
        {
            string errText = string.Empty;
            int errRow = 0, errCol = 0;
            this.GetFT();
            //判断支付方式
            if (!this.IsPayModesValid(ref errText, ref errRow, ref errCol))
            {
                MessageBox.Show(errText, "提示");
                this.fpPayMode.Focus();
                this.fpPayMode_Sheet1.SetActiveCell(errRow, errCol, false);

                return false;
            }
            alPatientPayModeInfo = QueryBalancePays();
            if (alPatientPayModeInfo == null)
            {
                MessageBox.Show("获得支付方式信息出错!", "提示");
                return false;
            }
            //判断分发票方式
            if (this.IsSplitInvoicesValid() == false)
            {
                this.fpSplit.Focus();
                return false;
            }
            //长者券患者判断是否填写登记号
            //if (!string.IsNullOrEmpty(this.tbJzyy.Text) && string.IsNullOrEmpty(this.tbZzqdjh.Text))
            //{
            //    MessageBox.Show("请填写长者券登记号!", "提示");
            //    return false;
            //}


            ArrayList alTempInvoices = new ArrayList();
            ArrayList alTempInvoiceDetals = new ArrayList();
            ArrayList alTempInvoiceDetailsSec = new ArrayList();
            ArrayList alTempInvoiceFeeItemDetals = new ArrayList();
            ArrayList alTempInvoiceFeeItemDetalsSec = new ArrayList();
            Balance invoiceTemp = new Balance();

            for (int i = 0; i < this.fpSplit_Sheet1.RowCount; i++)
            {
                invoiceTemp = this.fpSplit_Sheet1.Rows[i].Tag as Balance;
                invoiceTemp.HKELDERLYROLL = this.tbZzqdjh.Text;
                alTempInvoices.Add(invoiceTemp);
                ArrayList tempArrayListTempInvoiceDetails = this.fpSplit_Sheet1.Cells[i, 0].Tag as ArrayList;
                alTempInvoiceDetailsSec.Add(tempArrayListTempInvoiceDetails);
                ArrayList tempArrayListTempInvoiceFeeItemDetals = this.fpSplit_Sheet1.Cells[i, 3].Tag as ArrayList;
                alTempInvoiceFeeItemDetalsSec.Add(tempArrayListTempInvoiceFeeItemDetals);
            }

            alTempInvoiceDetals.Add(alTempInvoiceDetailsSec);
            alTempInvoiceFeeItemDetals.Add(alTempInvoiceFeeItemDetalsSec);

            //0f4dd900-8a07-4dd6-a4ff-dc5567344fc4
            //add by allan 
            //if (this.icCard == "1")
            {
                #region 读取pos机xml端口
                int comPort = 1;
                string comPortFileName = Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + @"Profiles\ICCardBalanceXML.xml";
                XmlDocument doc = new XmlDocument();
                StreamReader sr = new StreamReader(comPortFileName, System.Text.Encoding.UTF8);
                string cleanDown = sr.ReadToEnd();
                doc.LoadXml(cleanDown);
                sr.Close();

                XmlNode protNode = doc.SelectSingleNode("XML/PORT");
                string protStr = protNode.InnerText;
                int protTemp = 1;
                if (int.TryParse(protStr, out protTemp))
                {
                    comPort = protTemp;
                }
                byte[] port = System.BitConverter.GetBytes(comPort);
                #endregion

                #region 获取建行POS机扣费开关
                Neusoft.FrameWork.Models.NeuObject conObj = managerIntegrate.GetConstant("IsCCBPosOpen", "1");
                bool IsCCBPosOpen = Neusoft.FrameWork.Function.NConvert.ToBoolean(conObj.Memo);

                Neusoft.FrameWork.Models.NeuObject conObj1 = managerIntegrate.GetConstant("IsCCBPosOpen", "2");
                bool IsNEWCCBPosOpen = Neusoft.FrameWork.Function.NConvert.ToBoolean(conObj1.Memo);
                #endregion
                foreach (BalancePay p in alPatientPayModeInfo)
                {

                    if (p.PayType.ID == "MCZH" || p.PayType.ID == "MCDZ" || p.PayType.ID == "WCMCZH" || p.PayType.ID == "MCSNYD")
                    {
                        #region 医保POS机
                        //最新pos机要初始化波特率
                        try
                        {
                            PerformanceTracerLogger.BeginStep("医保POS机初始化波特率");
                            bool istrue = Neusoft.SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_SetComm(57600, 0, 8, 1);
                        }
                        catch (Exception ex)
                        {

                        }
                        finally
                        {
                            PerformanceTracerLogger.EndStep();
                        }

                        bool bPort = Neusoft.SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_SetPort(port[0]);
                        if (bPort)
                        {


                            SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_SetTimeOut(80);

                            //Neusoft.SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_SetTimeOut(30);

                            if (p.PayType.ID == "MCZH" || p.PayType.ID == "WCMCZH" || p.PayType.ID == "MCSNYD")
                            {
                                this.WriteLog("儿保pos机", p.FT.TotCost.ToString("#0.00"));
                                #region pos机社保卡交易

                                PerformanceTracerLogger.BeginStep("医保POS机");

                                string strBack = SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_BalanceByMoney(p.FT.TotCost.ToString("#0.00"));

                                PerformanceTracerLogger.EndStep();

                                this.WriteLog("儿保pos机返回", strBack);
                                string[] backDetails = strBack.Split(',');
                                try
                                {
                                    #region 读取返回数据
                                    if (backDetails.Length > 4 && backDetails[1] == "00")//面居然读取16个
                                    {
                                        Neusoft.HISFC.Models.POS.MedPosRecordInfos infos = new Neusoft.HISFC.Models.POS.MedPosRecordInfos();  //交易成功继续信息
                                        infos.Card_No = this.PatientInfo.ID;
                                        infos.Invoice_No = ((Balance)alTempInvoices[alTempInvoices.Count - 1]).Invoice.ID;
                                        PatientInfo.PosInfo.GNBM = backDetails[0];
                                        infos.GNBM = backDetails[0];
                                        if (infos.GNBM != "31") //代表卡交易
                                        {
                                            PatientInfo.PosInfo.FKBZ = backDetails[1];
                                            infos.FKBZ = backDetails[1];
                                            PatientInfo.PosInfo.QQSJ = backDetails[2];
                                            infos.QQSJ = backDetails[2];
                                            PatientInfo.PosInfo.LJJYJE = backDetails[3];
                                            infos.LJJYJE = backDetails[3];
                                            PatientInfo.PosInfo.JYPZH = backDetails[4];
                                            infos.JYPZH = backDetails[4];
                                            PatientInfo.PosInfo.JYRZM = backDetails[5];
                                            infos.JYRZM = backDetails[5];
                                            PatientInfo.PosInfo.JYJE = backDetails[6];
                                            infos.JYJE = backDetails[6];
                                            PatientInfo.PosInfo.ZDJKH = backDetails[7];
                                            infos.ZDJKH = backDetails[7];
                                            PatientInfo.PosInfo.KJYXH = backDetails[8];
                                            infos.KJYXH = backDetails[8];
                                            PatientInfo.PosInfo.ZDJYXH = backDetails[9];
                                            infos.ZDJYXH = backDetails[9];
                                            PatientInfo.PosInfo.JYSJ = backDetails[10];
                                            infos.JYSJ = backDetails[10];
                                            PatientInfo.PosInfo.SBKSSCSDM = backDetails[11];
                                            infos.SBKSSCSDM = backDetails[11];
                                            PatientInfo.PosInfo.KPFWXX = backDetails[12];
                                            infos.KPFWXX = backDetails[12];
                                            PatientInfo.PosInfo.SBKKH = backDetails[13];
                                            infos.SBKKH = backDetails[13];
                                            PatientInfo.PosInfo.POSZDH = backDetails[14];
                                            infos.POSZDH = backDetails[14];
                                            PatientInfo.PosInfo.POSBB = backDetails[15];
                                            infos.POSBB = backDetails[15];
                                        }
                                        else//电子凭证交易
                                        {
                                            PatientInfo.PosInfo.FKBZ = backDetails[1];//反馈标志 
                                            infos.FKBZ = backDetails[1];//反馈标志 
                                            PatientInfo.PosInfo.QQSJ = backDetails[2];//请求时间  
                                            infos.QQSJ = backDetails[2];//请求时间  
                                            PatientInfo.PosInfo.LJJYJE = backDetails[3];//联机交易金额 
                                            infos.LJJYJE = backDetails[3];//联机交易金额 
                                            PatientInfo.PosInfo.JYPZH = backDetails[4];//交易凭证号 
                                            infos.JYPZH = backDetails[4];//交易凭证号 
                                            //PatientInfo.PosInfo.JYRZM = backDetails[5];
                                            //infos.JYRZM = backDetails[5];
                                            PatientInfo.PosInfo.JYJE = backDetails[6];//交易金额 
                                            infos.JYJE = backDetails[6];//交易金额 
                                            PatientInfo.PosInfo.ZDJKH = backDetails[7];//终端机psam卡号 
                                            infos.ZDJKH = backDetails[7];//终端机psam卡号 
                                            // PatientInfo.PosInfo.KJYXH = backDetails[8];
                                            //infos.KJYXH = backDetails[8];
                                            //PatientInfo.PosInfo.ZDJYXH = backDetails[9];
                                            //infos.ZDJYXH = backDetails[9];
                                            PatientInfo.PosInfo.JYSJ = backDetails[8];//交易时间 
                                            infos.JYSJ = backDetails[8];//交易时间 
                                            PatientInfo.PosInfo.SBKSSCSDM = backDetails[9];//社保卡所属城市代码 
                                            infos.SBKSSCSDM = backDetails[9];//社保卡所属城市代码 
                                            //PatientInfo.PosInfo.KPFWXX = backDetails[12];
                                            // infos.KPFWXX = backDetails[12];
                                            //PatientInfo.PosInfo.SBKKH = backDetails[13];
                                            // infos.SBKKH = backDetails[13];
                                            PatientInfo.PosInfo.POSZDH = backDetails[10];//Pos终端编号 
                                            infos.POSZDH = backDetails[10];//Pos终端编号 
                                            PatientInfo.PosInfo.POSBB = backDetails[11];//POS版本 
                                            infos.POSBB = backDetails[11];//POS版本 
                                        }


                                        //记录交易成功的记录
                                        Neusoft.HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                                        if (outOp.InsertMedPos(infos))  //保存成功 继续交易  失败 就退出
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            MessageBox.Show("POS机结算返回为：" + infos + "。处理失败：" + outOp.Err);
                                            //如果没有保存成功  也不能交易成功  需要退掉   这个过程基本上是用不到的
                                            SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_CancelByNo(infos.JYPZH);
                                            return false;
                                        }

                                    }
                                    else
                                    {
                                        PatientInfo.PosInfo.GNBM = backDetails[0];
                                        PatientInfo.PosInfo.FKBZ = backDetails[1];
                                        PatientInfo.PosInfo.ERRMSG = backDetails[2];
                                        MessageBox.Show("POS机交易失败" + strBack);
                                        return false;  //交易不成功
                                    }
                                    #endregion
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(strBack + "原因：" + e.Message);
                                    return false;
                                }
                                #endregion
                            }
                            else
                            {
                                string authCode = Interaction.InputBox("请将手机对准电子社保卡条形码！", "电子社保卡", "", -1, -1);

                                if (string.IsNullOrEmpty(authCode))
                                {
                                    MessageBox.Show("请将手机对准扫码器！", "异常");
                                    return false;
                                }

                                string para = "<A>91,{0},{1},{2}</A>";//<A>91,交易时间,交易金额,条形码</A>
                                DateTime now = managerIntegrate.GetDateTimeFromSysDateTime();
                                para = string.Format(para, now.ToString("yyyyMMddHHmmss"), p.FT.TotCost.ToString("#0.00"), authCode);
                                string strBack = SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_Balance(para);
                                string[] backDetails = strBack.Split(',');
                                try
                                {
                                    #region 读取返回数据
                                    if (backDetails.Length > 4 && backDetails[1] == "00")//面居然读取16个
                                    {
                                        Neusoft.HISFC.Models.POS.MedPosRecordInfos infos = new Neusoft.HISFC.Models.POS.MedPosRecordInfos();  //交易成功继续信息
                                        infos.Card_No = this.PatientInfo.ID;
                                        infos.Invoice_No = ((Balance)alTempInvoices[alTempInvoices.Count - 1]).Invoice.ID;
                                        PatientInfo.PosInfo.GNBM = backDetails[0];
                                        infos.GNBM = backDetails[0];
                                        PatientInfo.PosInfo.FKBZ = backDetails[1];
                                        infos.FKBZ = backDetails[1];
                                        PatientInfo.PosInfo.QQSJ = backDetails[2];
                                        infos.QQSJ = backDetails[2];
                                        PatientInfo.PosInfo.SBKKH = backDetails[3];
                                        infos.SBKKH = backDetails[3];
                                        PatientInfo.PosInfo.SFHM = backDetails[4];
                                        infos.SFHM = backDetails[4];
                                        PatientInfo.PosInfo.XM = backDetails[5];
                                        infos.XM = backDetails[5];
                                        PatientInfo.PosInfo.JYJE = backDetails[6];
                                        infos.JYJE = backDetails[6];
                                        PatientInfo.PosInfo.YBGZJE = backDetails[7];
                                        infos.YBGZJE = backDetails[7];
                                        PatientInfo.PosInfo.ZFBJE = backDetails[8];
                                        infos.ZFBJE = backDetails[8];
                                        PatientInfo.PosInfo.WXJE = backDetails[9];
                                        infos.WXJE = backDetails[9];
                                        PatientInfo.PosInfo.YLJE = backDetails[10];
                                        infos.YLJE = backDetails[10];
                                        PatientInfo.PosInfo.JYPZH = backDetails[11];
                                        infos.JYPZH = backDetails[11];
                                        PatientInfo.PosInfo.JYRZM = backDetails[12];
                                        infos.JYRZM = backDetails[12];
                                        PatientInfo.PosInfo.ZDJKH = backDetails[13];
                                        infos.ZDJKH = backDetails[13];
                                        PatientInfo.PosInfo.KJYXH = backDetails[14];
                                        infos.KJYXH = backDetails[14];
                                        PatientInfo.PosInfo.ZDJYXH = backDetails[15];
                                        infos.ZDJYXH = backDetails[15];
                                        PatientInfo.PosInfo.JYSJ = backDetails[16];
                                        infos.JYSJ = backDetails[16];
                                        PatientInfo.PosInfo.SBKSSCSDM = backDetails[17];
                                        infos.SBKSSCSDM = backDetails[17];
                                        PatientInfo.PosInfo.POSZDH = backDetails[18];
                                        infos.POSZDH = backDetails[18];
                                        PatientInfo.PosInfo.POSBB = backDetails[19];
                                        infos.POSBB = backDetails[19];

                                        //记录交易成功的记录
                                        Neusoft.HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                                        if (outOp.InsertYDMedPos(infos))  //保存成功 继续交易  失败 就退出
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            MessageBox.Show("移动支付POS机结算返回为：" + infos + "。处理失败：" + outOp.Err);
                                            //如果没有保存成功  也不能交易成功  需要退掉   这个过程基本上是用不到的
                                            SOC.Local.RADT.ZhuHai.ZDWY.POS.POSRead.RWCardD_CancelByNo(infos.JYPZH);
                                            return false;
                                        }

                                    }
                                    else
                                    {
                                        //等待超时后要查询订单是否支付成功，pos机的bug
                                        if (backDetails[0] == "11" && backDetails[1] == "-1" && backDetails[2] == "等待超时")
                                        {

                                        }
                                        PatientInfo.PosInfo.GNBM = backDetails[0];
                                        PatientInfo.PosInfo.FKBZ = backDetails[1];
                                        PatientInfo.PosInfo.ERRMSG = backDetails[2];
                                        MessageBox.Show("POS机交易失败" + strBack);
                                        return false;  //交易不成功
                                    }
                                    #endregion
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(strBack + "原因：" + e.Message);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                        #endregion
                    }
                    else if (p.PayType.ID == "MZZH")//门诊账户支付,扣取门诊预交金20190918
                    {
                        if (this.feeIntegrate.AccountPay(this.PatientInfo, p.FT.TotCost, ((Balance)alTempInvoices[alTempInvoices.Count - 1]).Invoice.ID, this.PatientInfo.DoctorInfo.Templet.Dept.ID, "C") < 0)
                        {
                            MessageBox.Show("门诊账户扣费失败！", "异常");
                            return false;
                        }
                    }
                    else if (p.PayType.ID == "CCB" && IsCCBPosOpen)//建行银行卡支付 20190920 改为连机支付
                    {
                        if (IsNEWCCBPosOpen)
                        {
                            Neusoft.HISFC.Models.POS.SDCCBPosInfo sdpayInfo = new Neusoft.HISFC.Models.POS.SDCCBPosInfo();
                            sdpayInfo.OperateType = "A0";
                            sdpayInfo.TransType = "30";
                            sdpayInfo.CardType = "01";
                            sdpayInfo.CashRegNo = "".PadRight(6, ' ');
                            sdpayInfo.CasherNo = "".PadRight(6, ' ');
                            sdpayInfo.Amount = (p.FT.TotCost * 100).ToString("0").PadLeft(12, '0');
                            sdpayInfo.CashTraceNo = "".PadRight(6, ' ');
                            sdpayInfo.OriginTraceNo = "".PadRight(6, ' ');
                            sdpayInfo.Reserved = "".PadRight(48, ' ');
                            string err = "";
                            Neusoft.HISFC.Models.POS.SDCCBPosOutInfo outInfo = new Neusoft.HISFC.Models.POS.SDCCBPosOutInfo();
                            Neusoft.HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                            outOp.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                            PerformanceTracerLogger.BeginStep("CCB_POS机");

                            int result = Neusoft.SOC.Local.RADT.ZhuHai.ZDWY.POS.SDCCBPos.CardTrans(0, sdpayInfo, ref outInfo, ref err);

                            PerformanceTracerLogger.EndStep();
                            if (result <= 0)
                            {
                                MessageBox.Show("杉德POS机扣费失败！" + err, "异常");
                                return false;
                            }
                            else
                            {
                                sdpayInfo.Card_NO = this.PatientInfo.PID.CardNO;//门诊号
                                sdpayInfo.InvoiceNo = ((Balance)alTempInvoices[alTempInvoices.Count - 1]).Invoice.ID;//发票号
                                sdpayInfo.SourceFlag = "2";
                                sdpayInfo.State = "1";
                                sdpayInfo.SerialNumber = this.PatientInfo.ID;

                                if (outOp.InsertSDPosInfo(sdpayInfo, outInfo) <= 0)
                                {
                                    MessageBox.Show("杉德POS机扣费成功，交易记录保存失败，请手工操作退费！" + err, "异常");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            #region 建行POS机扣费
                            Neusoft.HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                            outOp.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                            Neusoft.HISFC.Models.POS.CCBPosInfo payInfo = new Neusoft.HISFC.Models.POS.CCBPosInfo();
                            payInfo.OperType = "02";//银行卡支付
                            payInfo.Amount = (p.FT.TotCost * 100).ToString("0");//分为单位
                            payInfo.TotCost = p.FT.TotCost;
                            payInfo.TransCheck = outOp.GetSysDateTime("yyyyMMddHHmmss") + "123456";
                            payInfo.FeeDate = outOp.GetSysDateTime("yyyyMMdd");//消费日期
                            payInfo.VouchNo = "000001";//凭证号
                            payInfo.ReferenceNo = "000000000000";//参考号
                            payInfo.BatchNo = "000001";//批次号
                            payInfo.AuthNo = "000001";//授权码
                            payInfo.MerchantName = "2";//1挂号2门诊收费3住院押金4出院结算
                            payInfo.Card_No = this.PatientInfo.PID.CardNO;//门诊号
                            payInfo.Invoice_No = ((Balance)alTempInvoices[alTempInvoices.Count - 1]).Invoice.ID;//发票号
                            payInfo.State = "1";

                            string error = string.Empty;

                            PerformanceTracerLogger.BeginStep("建行POS机");

                            if (SOC.Local.RADT.ZhuHai.ZDWY.POS.CCBPosNew.CCBBankTrans(payInfo, ref error) != 1)
                            {
                                MessageBox.Show("建行POS机扣费失败！" + error, "异常");
                                return false;
                            }

                            PerformanceTracerLogger.EndStep();

                            if (payInfo.RspCode == "00")//扣费成功
                            {
                                //保存数据
                                if (outOp.InsertCCBPosNew(payInfo) < 0)  //保存成功 继续交易  失败 就退出
                                {
                                    MessageBox.Show("保存扣费数据失败：" + outOp.Err + "请手工操作退费");
                                    //如果没有保存成功  也不能交易成功  需要退掉   这个过程基本上是用不到的
                                    return false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("扣费失败,建行POS机扣费返回：" + payInfo.RspCode + "。处理失败");
                                return false;
                            }
                            #endregion
                        }


                    }
                }
            }
            //end 0f4dd900-8a07-4dd6-a4ff-dc5567344fc4

            this.FeeButtonClicked(alPatientPayModeInfo, alTempInvoices, alTempInvoiceDetals, alTempInvoiceFeeItemDetals);

            return true;
        }

        /// <summary>
        /// 设置控件默认焦点
        /// </summary>
        public void SetControlFocus()
        {
            this.panel1.Focus();
            this.groupBox2.Focus();
            this.tbRealCost.Focus();
        }

        /// <summary>
        /// 获取支付方式的行号
        /// </summary>
        /// <param name="sv"></param>
        /// <param name="name">支付方式</param>
        /// <returns></returns>
        private int GetRowIndexByName(SheetView sv, string name)
        {
            for (int i = 0; i <= sv.Rows.Count - 1; ++i)
            {
                if (name.Equals(sv.Cells[i, 0].Text))
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 设置列数据
        /// </summary>
        /// <param name="col"></param>
        private void SetCostValue(int col)
        {
            if (col == (int)PayModeCols.Cost)
            {
                if (NConvert.ToDecimal(this.fpPayMode_Sheet1.Cells[this.fpPayMode_Sheet1.ActiveRowIndex, (int)PayModeCols.Cost].Value) > 0)
                {
                    return;
                }

                decimal CACost = NConvert.ToDecimal(this.fpPayMode_Sheet1.Cells[0, (int)PayModeCols.Cost].Value);

                if (CACost > 0)
                {
                    this.fpPayMode_Sheet1.Cells[this.fpPayMode_Sheet1.ActiveRowIndex, (int)PayModeCols.Cost].Value = CACost;
                }
            }
        }

        /// <summary>
        /// 验证支付方式输入是否合法
        /// </summary>
        /// <param name="errText">错误信息</param>
        /// <param name="errRow">错误行</param>
        /// <param name="errCol">错误列</param>
        /// <returns>成功 true 错误false</returns>
        private bool IsPayModesValid(ref string errText, ref int errRow, ref int errCol)
        {
            string tempPayMode = string.Empty;
            decimal tempTotalCost = 0m;
            decimal tempCost = 0m;

            for (int i = 0; i < this.fpPayMode_Sheet1.RowCount; i++)
            {
                tempPayMode = this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text;

                try
                {
                    tempCost = NConvert.ToDecimal(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value);
                    tempTotalCost += tempCost;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("金额输入不合法" + ex.Message);
                    errRow = i;
                    errCol = (int)PayModeCols.Cost;
                    return false;
                }
                if (string.IsNullOrEmpty(tempPayMode) == true || tempCost == 0)
                {
                    continue;
                }

                string tempID = helpPayMode.GetID(tempPayMode);
                if (string.IsNullOrEmpty(tempID) == true)
                {
                    errText = "支付方式输入错误!";
                    errRow = i;
                    errCol = (int)PayModeCols.PayMode;
                    return false;
                }
            }

            if (tempTotalCost != this.totCost)
            {
                errText = "请核准支付方式金额";
            }

            return true;
        }

        /// <summary>
        /// 获得支付方式的集合
        /// </summary>
        /// <returns>成功 支付方式的集合 失败 null</returns>
        private ArrayList QueryBalancePays()
        {
            ArrayList balancePays = new ArrayList();
            BalancePay balancePay = null;

            decimal balancePayTotCost = 0;

            for (int i = 0; i < this.fpPayMode_Sheet1.RowCount; i++)
            {
                if (string.IsNullOrEmpty(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text) == true)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Text) == true)
                {
                    continue;
                }
                if (NConvert.ToDecimal(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value) == 0)
                {
                    continue;
                }
                balancePay = new BalancePay();
                balancePay.PayType.Name = this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Text;
                balancePay.PayType.ID = helpPayMode.GetID(balancePay.PayType.Name);
                if (string.IsNullOrEmpty(balancePay.PayType.ID) == true)
                {
                    return null;
                }
                if ("PB".Equals(balancePay.PayType.ID))
                {
                    //记账,需要同时获取记账方式表格中的信息
                    balancePay.Bank.Name = this.fpPubType_Sheet1.Cells[0, (int)PubTypes.PubType].Text;
                    balancePay.Bank.ID = helpPubType.GetID(balancePay.Bank.Name);
                    balancePay.Memo = this.fpPubType_Sheet1.Cells[0, (int)PubTypes.Mark].Text;
                    if (string.IsNullOrEmpty(balancePay.Bank.ID) || string.IsNullOrEmpty(balancePay.Bank.Name))
                    {
                        MessageBox.Show("记账方式错误!请选择记账方式!");
                        this.tabControl1.SelectedIndex = 2;
                        this.fpPubType.Focus();
                        return null;
                    }
                }
                balancePay.FT.TotCost = NConvert.ToDecimal(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value);
                balancePay.FT.RealCost = balancePay.FT.TotCost;
                balancePays.Add(balancePay);

                balancePayTotCost += balancePay.FT.TotCost;
            }

            if (balancePayTotCost != this.TotCost)
            {
                if ((balancePayTotCost + this.pubCost) != this.TotCost)
                {
                    MessageBox.Show("支付方式加总金额不等于总金额，请核对!");
                    return null;
                }
            }

            return balancePays;
        }

        /// <summary>
        /// 验证分发票数据是否合法
        /// </summary>
        /// <returns>成功 true 失败 false</returns>
        private bool IsSplitInvoicesValid()
        {
            decimal tempTotalCost = 0m;

            for (int i = 0; i < this.fpSplit_Sheet1.RowCount; i++)
            {
                if ("总发票".Equals(this.fpSplit_Sheet1.Cells[i, 2].Text))
                {
                    continue;
                }
                try
                {
                    tempTotalCost += NConvert.ToDecimal(this.fpSplit_Sheet1.Cells[i, 3].Text) +
                        NConvert.ToDecimal(this.fpSplit_Sheet1.Cells[i, 4].Text) +
                        NConvert.ToDecimal(this.fpSplit_Sheet1.Cells[i, 5].Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("金额输入不合法!\n" + ex.Message);
                    return false;
                }
            }

            if (Neusoft.FrameWork.Public.String.FormatNumber(tempTotalCost, 2) != this.totCost)
            {
                MessageBox.Show("分发票金额与总金额不符!请重新分配!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获得分发票信息
        /// </summary>
        /// <returns>成功 分发票信息 失败 null</returns>
        private ArrayList QuerySplitInvoices()
        {
            NeuObject obj = null;
            ArrayList alObj = new ArrayList();

            if ("01".Equals(this.pactInfo.ID))
            {
                for (int i = 0; i < this.fpSplit_Sheet1.RowCount; i++)
                {
                    obj = new NeuObject();
                    obj.ID = i.ToString();
                    obj.User01 = this.fpSplit_Sheet1.Cells[i, 1].Text;
                    alObj.Add(obj);
                }
            }
            else
            {
                obj = new NeuObject();
                obj.User01 = ownCost.ToString();
                obj.User02 = payCost.ToString();
                obj.User03 = pubCost.ToString();
            }

            return alObj;
        }

        #endregion

        /// <summary>
        /// 扫码支付
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbQrCodeFee_Click(object sender, EventArgs e)
        {
            this.Tag = "扫码收费";
            this.tbQrCodeFee.Enabled = false;
            this.tbFee.Enabled = false;

            this.fpPayMode.StopCellEditing();
            string authCode = Interaction.InputBox("请将手机对准条形码！", "字符串", "", -1, -1);
            string payMode = GetPayModeByPayBarCode(authCode);
            string payModeName = payMode == "ZFB" ? "支付宝" : payMode == "WX" ? "微信" : payMode == "SZRMB" ? "数字人民币" : "";
            string GHModeName = payMode == "ZFB" ? "GHZFB" : payMode == "WX" ? "GHWX" : payMode == "SZRMB" ? "GHRMB" : "";
            if (string.IsNullOrEmpty(payMode))
            {
                MessageBox.Show("条形码无效！", "异常");
                return;
            }

            #region 支付金额

            int row_barcode = 0, row_ca = 0, row_gh = 0;
            decimal cost_barcode = 0, cost_ca = 00, cost_gh = 0;
            string payment = "";

            for (int i = 0; i < this.fpPayMode_Sheet1.RowCount; i++)
            {
                string payID = this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.PayMode].Tag.ToString();

                if (payID.Equals(payMode))
                {
                    cost_barcode += Convert.ToDecimal(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value);
                    row_barcode = i;
                }

                if (payID.Equals("CA"))
                {
                    cost_ca += Convert.ToDecimal(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value);
                    row_ca = i;
                }

                if (payID.Equals(GHModeName))
                {
                    cost_gh += Convert.ToDecimal(this.fpPayMode_Sheet1.Cells[i, (int)PayModeCols.Cost].Value);
                    row_gh = i;
                    if (cost_gh > 0)
                    {
                        payment = payID;
                        cost_barcode = cost_gh;
                    }

                }
            }
            if (cost_barcode <= 0)
            {
                if (cost_ca > 0)
                {
                    cost_barcode = cost_ca;
                    this.fpPayMode_Sheet1.Cells[row_barcode, 1].Text = cost_barcode.ToString();
                }
                else
                {
                    MessageBox.Show("支付金额不正确，请重新选择支付方式！", "异常");
                    return;
                }
            }
            #endregion

            if (this.CheckPayMode() == false)
            {
                this.tbQrCodeFee.Enabled = true;
                return;
            }

            if (this.isPaySuccess == false)
            {
                this.tbQrCodeFee.Enabled = true;
                return;
            }

            this.isPaySuccess = false;

            #region 扫码收费
            string clinicCode = this.PatientInfo.ID;
            string tradeno = "";

            if (string.IsNullOrEmpty(authCode))
            {
                this.fpPayMode_Sheet1.Cells[row_barcode, 1].Text = "0";
                MessageBox.Show("请将手机对准扫码器！", "异常");
                return;
            }

            bool isSucc = false;
            bool isRefund = false;//是否退费
            try
            {
                if (newScanCodeMZSF)//扫码墩启用
                {
                    string pay_type = authCode.StartsWith("13") ? "3" : "4";//支付类型。微信：3；支付宝：4
                    Balance balance = this.alInvoices[0] as Balance;
                    Neusoft.HISFC.Models.ScanPay.PayMentInfo pMInfo = new Neusoft.HISFC.Models.ScanPay.PayMentInfo();
                    pMInfo.order_id = balance.Invoice.ID;//His订单号
                    pMInfo.type = "3";//订单类型 1当天挂号 2预约挂号 3门诊缴费 4住院按金 5门诊预交金充值 6住院预交金充值
                    pMInfo.fee = cost_barcode.ToString();//金额（元）
                    pMInfo.pay_code = authCode;//付款码，支持微信、支付宝，需与pay_type入参对应
                    pMInfo.patient_id = balance.Patient.PID.CardNO;//患者ID
                    pMInfo.patient_name = balance.Name;//患者姓名
                    pMInfo.pay_type = authCode.StartsWith("13") ? "3" : "4";//支付类型。微信：3；支付宝：4
                    pMInfo.data_order_id = System.DateTime.Now.ToString("yyyyMMddHHmmss") + "123456";
                    if (payment == "GHZFB" || payment == "GHWX" || payment == "GHRMB")
                    {
                        pMInfo.pay_type = "5";//支付类型。微信：3；支付宝：4;工行：5

                        PerformanceTracerLogger.BeginStep("工行支付");

                        string[] result = GHService.Getpay(pMInfo.pay_code, pMInfo.data_order_id, (cost_gh * 100).ToString("0")).Split(',');

                        PerformanceTracerLogger.EndStep();

                        if (result[0].ToString() == "支付成功")
                        {
                            pMInfo.code = "0";
                            pMInfo.msg = result[0].ToString();
                            isSucc = true;
                        }
                        else
                        {
                            pMInfo.code = "1";
                            pMInfo.msg = result[0].ToString();
                            balance.Invoice.ID = pMInfo.order_id + 1;
                            MessageBox.Show("扫码支付失败" + result[1].ToString(), "异常");
                            isSucc = false;
                        }
                    }
                    else
                    {
                        #region 扫码墩

                        string result = string.Empty;
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("order_id", pMInfo.order_id);//His订单号
                        dic.Add("type", pMInfo.type);//订单类型 1当天挂号 2预约挂号 3门诊缴费 4住院按金 5门诊预交金充值 6住院预交金充值
                        dic.Add("fee", pMInfo.fee);//金额（元）
                        dic.Add("pay_code", pMInfo.pay_code);//付款码，支持微信、支付宝，需与pay_type入参对应
                        dic.Add("patient_id", pMInfo.patient_id);//患者ID
                        dic.Add("patient_name", pMInfo.patient_name);//患者姓名
                        dic.Add("pay_type", pMInfo.pay_type);//支付类型。微信：3；支付宝：4

                        PerformanceTracerLogger.BeginStep("扫码墩微信/支付宝");

                        result = Post(scanPaymentUrl, dic);

                        PerformanceTracerLogger.EndStep();

                        JObject jResult = (JObject)JsonConvert.DeserializeObject(result);
                        pMInfo.code = jResult["code"].ToString();
                        pMInfo.msg = jResult["msg"].ToString();
                        pMInfo.data_order_id = "";
                        pMInfo.transaction_id = "";
                        if (jResult["code"].ToString() == "0")//成功
                        {
                            isSucc = true;
                            pMInfo.data_order_id = jResult["data"]["order_id"].ToString();
                            pMInfo.transaction_id = jResult["data"]["transaction_id"].ToString();
                            tradeno = pMInfo.data_order_id;
                        }
                        else
                        {
                            isSucc = false;
                        }

                        #endregion
                    }
                    Neusoft.HISFC.BizLogic.Fee.Outpatient outpMInfo = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                    outpMInfo.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                    if (!outpMInfo.InsertPayMentInfo(pMInfo))
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("插入挂号扫码支付记录表失败!");
                    }
                }
                else
                {
                    using (PushMsgService.PushMsginterfaceClient client = new Neusoft.SOC.Local.OutpatientFee.ZhuHai.PushMsgService.PushMsginterfaceClient(new System.ServiceModel.BasicHttpBinding(), new System.ServiceModel.EndpointAddress("http://172.16.106.100:8005/PushMsgService.svc")))
                    {
                        Balance balance = this.alInvoices[0] as Balance;
                        string invoiceNo = balance.Invoice.ID;

                        string[] recipes = new string[] { invoiceNo + "^" + ownCost.ToString() };

                        string res = client.QrCodeFee("2", clinicCode, recipes, Convert.ToDouble(cost_barcode), authCode, "");
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(res);
                        string resultCode = xml.SelectSingleNode("response/resultCode").InnerText;
                        string resultDesc = xml.SelectSingleNode("response/resultDesc").InnerText;

                        if (resultCode.Equals("0"))//成功
                        {
                            tradeno = resultDesc;
                            isSucc = true;
                        }
                        else if (resultCode.Equals("1"))//失败
                        {
                            this.fpPayMode_Sheet1.Cells[row_barcode, 1].Text = "0";
                            this.tbQrCodeFee.Enabled = true;
                            MessageBox.Show("扫码支付失败！请选择其他付款方式！原因：" + resultDesc, "异常");
                            return;
                        }
                        else if (resultCode.Equals("10003"))//等待
                        {
                            tradeno = resultDesc;
                            if (MessageBox.Show("患者是否支付成功？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                //查询订单状态
                                string state = client.QueryOrder("", tradeno, clinicCode, "2");
                                XmlDocument xml_state = new XmlDocument();
                                xml_state.LoadXml(state);
                                string orderState = xml_state.SelectSingleNode("response/orderState").InnerText;//0-未支付；1-已支付；2-已退费；
                                if (!orderState.Equals("1"))//不成功
                                {
                                    this.fpPayMode_Sheet1.Cells[row_barcode, 1].Text = "0";
                                    isRefund = true;
                                }
                                else
                                {
                                    isSucc = true;
                                }
                            }
                            else
                            {
                                this.fpPayMode_Sheet1.Cells[row_barcode, 1].Text = "0";
                                tradeno = resultDesc;
                                isRefund = true;
                            }
                        }
                        else
                        {
                            this.fpPayMode_Sheet1.Cells[row_barcode, 1].Text = "0";
                            isRefund = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                this.fpPayMode_Sheet1.Cells[row_barcode, 1].Text = "0";
            }

            if (isSucc == false)
            {
                this.fpPayMode_Sheet1.Cells[row_barcode, 1].Text = "0";
                if (isRefund == true)
                {
                    #region 退费
                    try
                    {
                        using (PushMsgService.PushMsginterfaceClient client = new Neusoft.SOC.Local.OutpatientFee.ZhuHai.PushMsgService.PushMsginterfaceClient(new System.ServiceModel.BasicHttpBinding(), new System.ServiceModel.EndpointAddress("http://172.16.106.100:8005/PushMsgService.svc")))
                        {
                            string res_Cancel = client.CancelQrCodeFee("", tradeno, Convert.ToDouble(cost_barcode), "2", clinicCode);
                            XmlDocument xml_Cancel = new XmlDocument();
                            xml_Cancel.LoadXml(res_Cancel);
                            string resultCode_Cancel = xml_Cancel.SelectSingleNode("response/resultCode").InnerText;
                            string resultDesc_Cancel = xml_Cancel.SelectSingleNode("response/resultDesc").InnerText;

                            //退费请求结果： 0-成功；1-失败
                            if (!resultCode_Cancel.Equals("0"))
                            {
                                this.tbQrCodeFee.Enabled = true;
                                MessageBox.Show("退回扫码所扣费用失败，原因：" + resultDesc_Cancel, "异常");
                                return;
                            }
                            else
                            {
                                this.tbQrCodeFee.Enabled = true;
                                MessageBox.Show("扫码支付失败，已将费用退还到原有账户：" + resultDesc_Cancel, "异常");
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.tbQrCodeFee.Enabled = true;
                        MessageBox.Show("请联系系统管理员介入！", "异常");
                        return;
                    }

                    #endregion
                }

                this.tbQrCodeFee.Enabled = true;
                MessageBox.Show("扫码支付失败！请选择其他付款方式！", "异常");
                return;
            }
            #endregion

            if (this.SaveFee() == false)
            {
                #region 退费
                try
                {
                    using (PushMsgService.PushMsginterfaceClient client = new Neusoft.SOC.Local.OutpatientFee.ZhuHai.PushMsgService.PushMsginterfaceClient(new System.ServiceModel.BasicHttpBinding(), new System.ServiceModel.EndpointAddress("http://172.16.106.100:8005/PushMsgService.svc")))
                    {
                        Balance balance = this.alInvoices[0] as Balance;
                        string invoiceNo = balance.Invoice.ID;

                        string[] recipes = new string[] { invoiceNo + "^" + cost_barcode.ToString() };

                        string res = client.CancelQrCodeFee("", tradeno, Convert.ToDouble(cost_barcode), "2", clinicCode);
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(res);
                        string resultCode = xml.SelectSingleNode("response/resultCode").InnerText;
                        string resultDesc = xml.SelectSingleNode("response/resultDesc").InnerText;

                        //支付请求
                        if (!resultCode.Equals("0"))
                        {
                            this.tbQrCodeFee.Enabled = true;
                            MessageBox.Show("退回扫码所扣费用失败，原因：" + resultDesc, "异常");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                #endregion

                this.tbQrCodeFee.Enabled = true;
                return;
            }
            this.tbQrCodeFee.Enabled = true;
            this.tbRealCost.Focus();

            this.isSuccessFee = true;

            this.Close();
        }
        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public string Post(string url, Dictionary<string, string> dic)
        {
            string LogAddress = "";
            if (!System.IO.Directory.Exists(Application.StartupPath + "\\扫码墩支付日志"))
            {
                System.IO.Directory.CreateDirectory(Application.StartupPath + "\\扫码墩支付日志");
            }
            if (LogAddress == "")
            {
                LogAddress = Application.StartupPath + "\\扫码墩支付日志\\" +
                    DateTime.Now.Year + '-' +
                    DateTime.Now.Month + '-' +
                    DateTime.Now.Day + '-' +
                    DateTime.Now.Hour + "_Log.log";
            }
            //把异常信息输出到文件
            System.IO.StreamWriter fs = new System.IO.StreamWriter(LogAddress, true);
            fs.WriteLine(@"HISFC\Src\HISFC.Components\Order\OutPatient\Controls\ucOutPatientOrder.cs");
            fs.WriteLine("当前时间：" + DateTime.Now.ToString());
            try
            {
                if (scanPaymentUrl == string.Empty)
                {
                    MessageBox.Show("接口地址未配置！", "提示");
                    return "";
                }
                fs.WriteLine("请求地址：");
                fs.WriteLine(scanPaymentUrl);
                string result = "";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                #region 添加Post 参数
                StringBuilder builder = new StringBuilder();
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
                fs.WriteLine("请求参数：");
                fs.WriteLine(builder.ToString());
                byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
                req.ContentLength = data.Length;
                using (System.IO.Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                fs.WriteLine("响应内容：");
                fs.WriteLine(result);
                fs.WriteLine("==================================================================================================================");
                fs.Close();
                return result;
            }
            catch (Exception ex)
            {
                fs.WriteLine("异常信息：" + ex.Message);
                fs.WriteLine("异常对象：" + ex.Source);
                fs.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
                fs.WriteLine("触发方法：" + ex.TargetSite);
                fs.Close();
                throw;
            }
        }

        private string GetPayModeByPayBarCode(string authCode)
        {
            string payMode = "";
            if (authCode.StartsWith("13"))
                payMode = "WX";
            else if (authCode.StartsWith("28"))
                payMode = "ZFB";
            else if (authCode.StartsWith("0100"))
                payMode = "SZRMB";
            return payMode;
        }

        public readonly static string baseUrl = AppDomain.CurrentDomain.BaseDirectory;
        public bool WriteLog(string type, string logMsg)
        {
            bool bo = true;
            string date = DateTime.Now.ToString("yyyyMMdd");
            string fileName = date + "_" + type + "_log.txt";
            string filePath = baseUrl + "Log\\" + fileName;
            string path = baseUrl + "Log\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            StreamWriter sw = null;
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        using (sw = new StreamWriter(fs))
                        {
                            sw.WriteLine("======================================================= "
                                + date + " ==============================================================");
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
                if (!string.IsNullOrEmpty(logMsg))
                {
                    using (sw = new StreamWriter(filePath, true))
                    {
                        sw.WriteLine("写入时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        sw.WriteLine(logMsg);
                        sw.WriteLine(@"==============================================================
                            =================================================================");
                        sw.WriteLine("\r");
                    }
                    if (sw != null)
                        sw.Close();
                }
            }
            catch (Exception ex)
            {
                bo = false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
            return bo;
        }
    }
}