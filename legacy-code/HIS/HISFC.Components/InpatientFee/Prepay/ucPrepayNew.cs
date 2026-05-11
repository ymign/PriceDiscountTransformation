using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Neusoft.FrameWork.Function;
using Neusoft.FrameWork.Management;
using System.Collections;
using Neusoft.FrameWork.WinForms.Classes;
using System.Xml;
using Microsoft.VisualBasic;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Neusoft.HISFC.Models.Account;
using HisCallExternalServiceProject.UnityPay.WXFKM;
using HisCallExternalServiceProject.UnityPay.Model.CreateOrder;
using HisCallExternalServiceProject.UnityPay.Model.PayOrder;
using HisCallExternalServiceProject.UnityPay.Model.QueryOrderPayResult;
using System.Threading;
using HisCallExternalServiceProject.UnityPay.Enums;
using HisCallExternalServiceProject.UnityPay.DB;
using HisCallExternalServiceProject.UnityPay.Model.His;
using HisCallExternalServiceProject.UnityPay.Forms;
using HisCallExternalServiceProject.UnityPay.Model.RefundOrder;
using HisCallExternalServiceProject.UnityPay;

namespace Neusoft.HISFC.Components.InpatientFee.Prepay
{
    /// <summary>
    /// ucPrepayNew<br></br>
    /// [功能描述: 结算控件]<br></br>
    /// [创 建 者: lingk]<br></br>
    /// [创建时间: 2013-08-19]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>

    public partial class ucPrepayNew : Neusoft.FrameWork.WinForms.Controls.ucBaseControl, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ucPrepayNew()
        {
            this.InitializeComponent();
        }

        #region "变量"
        /// <summary>
        /// 患者基本信息综合实体

        /// </summary>
        protected Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

        /// <summary>
        /// 入出转integrate层

        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();

        /// <summary>
        /// 住院费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.InPatient feeInpatient = new Neusoft.HISFC.BizLogic.Fee.InPatient();

        /// <summary>
        /// 住院费用组合业务层
        /// <summary>
        /// 工行支付接口
        /// </summary>
        public readonly GHpayService.Service GHService = new Neusoft.HISFC.Components.InpatientFee.GHpayService.Service();

        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// 管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        GDSI.LocalManager lm = new GDSI.LocalManager();
        GDSI.CountryMedical.DAL.QueryDAL queryDB = new GDSI.CountryMedical.DAL.QueryDAL();
        private static GDSI.Log log = new GDSI.Log();
        /// <summary>
        /// toolBarService
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();

        //控制参数判断
        /// <summary>
        /// 是否打印冲红发票
        /// </summary>
        bool IsPrintReturn = false;

        /// <summary>
        /// 负发票是否走新票号
        /// </summary>
        bool IsReturnNewInvoice = false;

        /// <summary>
        /// 是否可以作废，重打隔天票据
        /// </summary>
        private bool isCanDealBefore = true;

        /// <summary>
        /// 是否可以交叉退预交金
        /// </summary>
        private bool isCanQuitOtherOper = true;

        /// <summary>
        /// 是否打印预交金发票
        /// </summary>
        private bool isPrintInvoice = true;

        /// <summary>
        /// 是否打印签名卡
        /// </summary>
        private bool isPrintPatientSign = false;

        /// <summary>
        /// 发票打印接口
        /// </summary>
        private Neusoft.HISFC.BizProcess.Interface.FeeInterface.IPrepayPrint prepayPrint = null;

        #region 新扫码墩开关 true 打开，false关闭
        /// <summary>
        /// 预交金管理新扫码墩开关 true 打开，false关闭
        /// </summary>
        private bool newScanCodeYJJGL = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam().GetControlParam<bool>("newScanCodeYJJGL", false, false);

        /// <summary>
        /// 预交金管理新扫码墩开关 true 打开，false关闭
        /// </summary>
        private bool newScanReFundZYYJJTF = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam().GetControlParam<bool>("newScanReFundZYYJJTF", false, false);
        /// <summary>
        /// 支付平台退款开关
        /// </summary>
        public bool ZFPTReFundZYYJ = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam().GetControlParam<bool>("ZFPTReFundZYYJ", false, false);
        /// <summary>
        /// 扫码墩可退金额查询地址
        /// </summary>
        private string RefundableUrl = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam().GetControlParam<string>("PT0006", false, string.Empty);
        #endregion

        /// <summary>
        /// 扫码墩收款链接
        /// </summary>
        private string scanPaymentUrl = string.Empty;
        /// <summary>
        /// 扫码墩退款链接
        /// </summary>
        private string scanRefundUrl = string.Empty;

        #endregion "属性"
        #region IInterfaceContainer 成员

        Type[] Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer.InterfaceTypes
        {
            get
            {
                Type[] type = new Type[1];
                type[0] = typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IPrepayPrint);

                return type;
            }
        }

        #endregion

        #region "属性"

        public Neusoft.HISFC.BizProcess.Interface.FeeInterface.IPrepayPrint PrepayPrint
        {
            set
            {
                this.prepayPrint = value;
            }
        }

        /// <summary>
        /// 是否允许补打以前的发票

        /// </summary>
        [Category("控件设置"), Description("是否可以作废，重打隔天票据")]
        public bool IsCanDealBefore
        {
            get
            {
                return isCanDealBefore;
            }
            set
            {
                isCanDealBefore = value;
            }
        }


        [Category("控件设置"), Description("是否可以交叉退预交金")]
        public bool IsCanQuitOtherOper
        {
            get { return isCanQuitOtherOper; }
            set { isCanQuitOtherOper = value; }
        }

        [Category("控件设置"), Description("是否打印预交金收据")]
        public bool IsPrintInvoice
        {
            get
            {
                return isPrintInvoice;
            }
            set
            {
                isPrintInvoice = value;
            }
        }

        [Category("控件设置"), Description("查询显示患者的状态设置")]
        public Neusoft.HISFC.Components.Common.Controls.enuShowState ShowState
        {
            get
            {
                return this.ucQueryInpatientNo.ShowState;
            }
            set
            {
                this.ucQueryInpatientNo.ShowState = value;
            }
        }

        [Category("控件设置"), Description("是否打印签名卡 是=true，否=false")]
        public bool IsPrintPatientSign
        {
            get
            {
                return isPrintPatientSign;
            }
            set
            {
                isPrintPatientSign = value;
            }
        }

        string strBankPayMode = string.Empty;
        [Category("控件设置"), Description("显示银行与开户账号的支付方式，用'|'分开")]
        public string ShowBankPayMode
        {
            get
            {
                return strBankPayMode;
            }
            set
            {
                strBankPayMode = value;
            }
        }

        #endregion

        #region "方法"
        /// <summary>
        /// 初始化控件信息

        /// </summary>
        public virtual void initControl()
        {
            //初始化默认现金方式

            this.cmbPayType.Tag = "CA";
            this.cmbPayType.Text = "现金";

            //确定选择方式
            this.cmbPayType.IsListOnly = true;
            this.cmbTransType1.IsListOnly = true;
            this.cmbTransType2.IsListOnly = true;
            //初始化farpoint属性

            this.fpPrepay_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.fpPrepay_Sheet1.GrayAreaBackColor = System.Drawing.Color.White;
            //初始化住院号控件
            this.ucQueryInpatientNo.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ucQueryInpatientNo.TextBox.Size = new System.Drawing.Size(116, 21);
            this.ucQueryInpatientNo.TextBox.Location = new System.Drawing.Point(52, 5);
            this.ucQueryInpatientNo.TextBox.BringToFront();
            //添加支付方式控件事件
            this.cmbPayType.KeyDown += new KeyEventHandler(cmbPayType_KeyDown);
            this.cmbPayType.KeyPress += new KeyPressEventHandler(cmbPayType_KeyPress);
            this.cmbPayType.SelectedIndexChanged += new EventHandler(cmbPayType_SelectedIndexChanged);

            this.cmbTransType1.KeyDown += new KeyEventHandler(cmbTransType1_KeyDown);
            this.cmbTransType1.KeyPress += new KeyPressEventHandler(cmbTransType1_KeyPress);
            this.cmbTransType1.SelectedIndexChanged += new EventHandler(cmbTransType1_SelectedIndexChanged);

            this.cmbTransType2.KeyDown += new KeyEventHandler(cmbTransType2_KeyDown);
            this.cmbTransType2.KeyPress += new KeyPressEventHandler(cmbTransType2_KeyPress);
            this.cmbTransType2.SelectedIndexChanged += new EventHandler(cmbTransType2_SelectedIndexChanged);
            //显示下一张收据号 //不需要显示了
            //this.GetNextInvoiceNO();


            ArrayList alBanks = this.managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.BANK);
            if (alBanks == null || alBanks.Count <= 0)
            {
                MessageBox.Show("获取银行列表失败!");
                return;
            }
            this.cmbBank.AddItems(alBanks);

            this.cmbBank.KeyDown += new KeyEventHandler(cmbBank_KeyDown);
            //  this.cmbBank.SelectedIndexChanged +=new EventHandler(cmbBank_SelectedIndexChanged);

            this.txtPreCost.KeyDown += new KeyEventHandler(txtPreCost_KeyDown);
            this.txtPreCost1.KeyDown += new KeyEventHandler(txtPreCost1_KeyDown);
            this.txtPreCost2.KeyDown += new KeyEventHandler(txtPreCost2_KeyDown);

            this.txtMark.KeyDown += new KeyEventHandler(txtMark_KeyDown);

            this.pnlBankInfo.Visible = false;



        }

        /// <summary>
        /// 读取控制类信息
        /// </summary>
        private int ReadControlInfo()
        {
            Neusoft.FrameWork.Management.ControlParam controlParm = new Neusoft.FrameWork.Management.ControlParam();
            try
            {
                this.IsPrintReturn = Neusoft.FrameWork.Function.NConvert.ToBoolean(controlParm.QueryControlerInfo("100015"));
                this.IsReturnNewInvoice = Neusoft.FrameWork.Function.NConvert.ToBoolean(controlParm.QueryControlerInfo("100016"));
            }
            catch
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("读取控制类信息出错!", 211);
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 查询患者预交金信息
        /// </summary>
        /// <param name="patientInfo">住院患者基本信息实体</param>
        /// <returns>1 成功 －1失败</returns>
        protected virtual int QueryPatientPrepay(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo)
        {
            //添加行


            ArrayList al = new ArrayList();

            try
            {
                //根据住院号提取患者预交金信息到ArrayList
                al = this.feeInpatient.QueryPrepays(patientInfo.ID);
                if (al == null) return 0;
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(ex.Message, 211);
                return -1;
            }
            this.fpPrepay_Sheet1.RowCount = 0;
            this.fpPrepay_Sheet1.RowCount = al.Count;
            //交款次数
            int PayCount = 0;
            //返款次数
            int WasteCount = 0;
            Hashtable hsCount = new Hashtable();
            Hashtable hsOrgInfo = new Hashtable();

            for (int i = 0; i < al.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
                prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)al[i];

                string PrepayState = "";
                if (prepay.FT.PrepayCost > 0)
                {
                    if (!hsCount.ContainsKey(prepay.RecipeNO))
                    {
                        hsCount.Add(prepay.RecipeNO, prepay.RecipeNO);
                        PayCount++;
                    }
                    PrepayState = "收取";
                }
                else
                {
                    WasteCount++;
                    switch (prepay.PrepayState)
                    {
                        case "1":
                            PrepayState = "作废";

                            break;
                        case "2":
                            PrepayState = "补打";
                            break;
                        default:
                            PrepayState = "收取";
                            break;
                    }
                }
                //更新一些没有的字段()
                string PrepaySource = "";
                if (prepay.TransferPrepayState == "0")
                {
                    PrepaySource = "预交金";
                }
                else
                {
                    PrepaySource = "转押金";
                }
                //结算标记
                string BalanceFlag = "";
                if (prepay.BalanceState == "0")
                {
                    BalanceFlag = "未结算";
                }
                else
                {
                    BalanceFlag = "已结算";
                }
                //收款员姓名


                Neusoft.HISFC.BizProcess.Integrate.Manager managerIntergrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
                Neusoft.HISFC.Models.Base.Employee empl = new Neusoft.HISFC.Models.Base.Employee();
                empl = managerIntergrate.GetEmployeeInfo(prepay.PrepayOper.ID);

                if (empl == null)
                { prepay.PrepayOper.Name = ""; }
                else
                {
                    prepay.PrepayOper.Name = empl.Name;
                }
                //取到原有的预交金收取人
                if (!hsOrgInfo.ContainsKey(prepay.RecipeNO))
                {
                    if (prepay.BalanceState == "1") //记录已结算的，可能存在结算召回显示问题
                    {
                        hsOrgInfo.Add(prepay.RecipeNO, prepay.PrepayOper.Name);
                    }
                }
                else
                {
                    if (prepay.BalanceState == "0") //结算召回
                    {
                        prepay.PrepayOper.Name = hsOrgInfo[prepay.RecipeNO].ToString();
                    }
                }
                //支付方式

                Neusoft.FrameWork.Models.NeuObject payObj = this.managerIntegrate.GetConstant("PAYMODES", prepay.PayType.ID);
                if (payObj == null)
                {
                    MessageBox.Show("获得支付方式定义信息出错!" + this.managerIntegrate.Err);

                    return -1;
                }

                //添加farpoint显示内容
                //{4E569A30-8655-4461-86B8-450BD5D245D4}
                //Object[] o = new Object[] { prepay.RecipeNO, PrepayState, prepay.FT.PrepayCost, payObj.Name, PrepaySource, BalanceFlag, prepay.PrepayOper.Name, prepay.PrepayOper.OperTime.ToString() };
                Object[] o = new Object[] { prepay.RecipeNO, PrepayState, prepay.FT.PrepayCost, payObj.Name, PrepaySource, BalanceFlag, prepay.PrepayOper.Name, prepay.OrgInvoice.ID, prepay.PrepayOper.OperTime.ToString(), prepay.User02 };

                for (int j = 0; j <= o.GetUpperBound(0); j++)
                {
                    try
                    {
                        fpPrepay_Sheet1.Cells[i, j].Value = o[j];
                    }
                    catch (Exception ex)
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg(ex.Message, 211);
                        return -1;
                    }
                }
                if (prepay.PrepayState != "0") this.fpPrepay_Sheet1.Cells[i, 1].ForeColor = System.Drawing.Color.Red;
                fpPrepay_Sheet1.Rows[i].Tag = prepay;
            }
            //返还交款次数
            this.txtPayNum.Text = PayCount.ToString();
            this.txtBackNum.Text = WasteCount.ToString();
            //余额
            if (Neusoft.FrameWork.Public.String.FormatNumber(decimal.Parse(this.txtFreeCost.Text), 2) < 0)
            {
                this.txtFreeCost.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                this.txtFreeCost.ForeColor = System.Drawing.Color.Black;
            }
            return 1;
        }

        /// <summary>
        /// 清空
        /// </summary>
        protected virtual void Clear()
        {

            this.patientInfo = null;
            txtSumPreCost.Text = "";
            this.txtTotCost.Text = "";
            this.txtName.Text = "";
            this.txtDept.Text = "";
            this.txtPact.Text = "";
            this.txtBedNo.Text = "";
            this.txtOwnCost.Text = "";
            txtFreeCost.Text = "";
            txtBirthday.Text = "";
            txtNurseStation.Text = "";
            txtDateIn.Text = "";
            txtDoctor.Text = "";
            this.cmbPayType.Tag = "CA";
            this.cmbPayType.Text = "现金";
            this.cmbPayType.bank = new Neusoft.HISFC.Models.Base.Bank();
            this.fpPrepay_Sheet1.RowCount = 0;
            this.txtPayNum.Text = "";
            this.txtBackNum.Text = "";
            this.txtPreCost.Text = "";//预交金额清空
            this.txtPreCost2.Text = "";//预交金额清空
            this.txtPreCost1.Text = "";//预交金额清空
            this.cmbTransType1.Text = "";
            this.cmbTransType2.Text = "";
            this.txtIntimes.Text = "";
            this.txtClinicDiagnose.Text = "";
            this.txtDIST.Text = "";
            this.txtBirthArea.Text = "";
            //显示下一张收据号
            //this.GetNextInvoiceNO();
        }

        /// <summary>
        /// 利用患者信息实体进行控件赋值
        /// </summary>
        /// <param name="patientInfo">患者基本信息实体</param>
        protected virtual void EvaluteByPatientInfo(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo)
        {
            if (patientInfo == null)
            {
                patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
            }
            //预交金总额
            this.txtSumPreCost.Text = patientInfo.FT.PrepayCost.ToString();
            //费用金额
            this.txtTotCost.Text = patientInfo.FT.TotCost.ToString();
            // 姓名
            this.txtName.Text = patientInfo.Name;
            // 科室
            this.txtDept.Text = patientInfo.PVisit.PatientLocation.Dept.Name;
            // 合同单位
            this.txtPact.Text = patientInfo.Pact.Name;
            //床号
            this.txtBedNo.Text = patientInfo.PVisit.PatientLocation.Bed.ID;
            //自费金额
            this.txtOwnCost.Text = patientInfo.FT.OwnCost.ToString();
            //余额
            txtFreeCost.Text = patientInfo.FT.LeftCost.ToString();
            //生日
            txtBirthday.Text = patientInfo.Birthday.ToString("yyyy-MM-dd");
            //所属病区



            txtNurseStation.Text = patientInfo.PVisit.PatientLocation.NurseCell.Name;
            //入院日期
            txtDateIn.Text = patientInfo.PVisit.InTime.ToString("yyyy-MM-dd");
            // 医生
            txtDoctor.Text = patientInfo.PVisit.AdmittingDoctor.Name;
            //住院号

            //备注
            this.txtMark.Text = patientInfo.Memo.ToString();

            //籍贯
            try
            {
                this.txtDIST.Text = patientInfo.DIST.ToString();
                //出生地
                this.txtBirthArea.Text = patientInfo.AreaCode.ToString();
            }
            catch { }

            this.ucQueryInpatientNo.Text = patientInfo.PID.PatientNO;

            //门诊诊断
            this.txtClinicDiagnose.Text = patientInfo.ClinicDiagnose;
            //住院次数
            this.txtIntimes.Text = patientInfo.InTimes.ToString();

        }

        /// <summary>
        /// 增加ToolBar控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("收取", "收取患者的预交金", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.J借入, true, false, null);
            toolBarService.AddToolButton("返还", "返还患者预交金", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.J借出, true, false, null);
            //toolBarService.AddToolButton("作废", "返还患者预交金", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.J借出, true, false, null);
            toolBarService.AddToolButton("清屏", "清空信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q清空, true, false, null);
            toolBarService.AddToolButton("重打", "预交金发票重打(走号)", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.C重打, true, false, null);
            toolBarService.AddToolButton("补打", "预交金发票补打(不走号)", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印, true, false, null);
            toolBarService.AddToolButton("换开", "", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.H换单, true, false, null);
            toolBarService.AddToolButton("帮助", "打开帮助文件", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.B帮助, true, false, null);

            toolBarService.AddToolButton("补打退费发票", "补打退费签名单", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印, true, false, null);

            toolBarService.AddToolButton("更新发票号", "更新下一发票号", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.F分票, true, false, null);
            toolBarService.AddToolButton("医保电子凭证", "", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.C查找, true, false, null);

            return this.toolBarService;
        }

        /// <summary>
        /// 定义toolbar按钮click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //string tempText = string.Empty;

            //try
            //{
            //    tempText = this.hsToolBar[e.ClickedItem.Text].ToString();
            //}
            //catch (Exception ex)
            //{
            //    return;
            //}

            ButtonClicked(e.ClickedItem.Text);

            base.ToolStrip_ItemClicked(sender, e);
        }

        /// <summary>
        /// 响应键盘、鼠标事件
        /// </summary>
        /// <param name="tempText">工具栏按钮名称</param>
        private void ButtonClicked(string tempText)
        {
            switch (tempText)
            {
                case "收取":

                    this.ReceivePrepay();

                    break;
                case "返还":
                case "作废":

                    this.ReturnPrepay();
                    break;
                case "清屏":

                    this.Clear();
                    this.ucQueryInpatientNo.Text = "";
                    this.ucQueryInpatientNo.Focus();
                    break;
                case "重打":
                    this.ReprintPrepay();
                    break;
                case "补打":
                    this.QueryAndPrintPrepay();
                    break;
                case "帮助":
                    break;
                case "退出":
                    {
                        this.FindForm().Close();
                        break;
                    }
                case "补打退费发票":
                    this.RePrintPrepayPatientSign();
                    break;
                case "更新发票号":
                    //Neusoft.HISFC.Components.Common.Forms.frmUpdateUsedInvoiceNo frmUpdate = new Neusoft.HISFC.Components.Common.Forms.frmUpdateUsedInvoiceNo();
                    //frmUpdate.InvoiceType = "P";
                    //frmUpdate.ShowDialog();

                    //GetNextInvoiceNO();
                    break;
                case "医保电子凭证":
                    GetPatientNoByDZPZ();
                    break;
                case "换开":
                    this.ReplaceQueryAndPrintPrepay();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 打印预交金
        /// 王宇修改， 控制冲红票打印负数，并且注明作废字样
        /// 增加了[bool]isReturn参数，如果是冲红票为True,正常收取票为False
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="prepay"></param>
        /// <param name="isReturn"></param>
        protected virtual void PrintPrepayInvoice(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, ArrayList alPrepay, bool isReturn)
        {
            if (patientInfo.IsEncrypt)
            {
                patientInfo.Name = Neusoft.FrameWork.WinForms.Classes.Function.Decrypt3DES(patientInfo.NormalName);
            }
            this.prepayPrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IPrepayPrint)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IPrepayPrint;
            //regprint.SetPrintValue(regObj,regmr);
            //this.prepayPrint = new ucPrepayPrint();
            if (this.prepayPrint == null)
            {
                //this.prepayPrint = new ucPrepayPrint();
                return;
            }


            this.prepayPrint.SetValue(patientInfo, alPrepay);
            this.prepayPrint.Print();


        }

        /// <summary>
        /// 换开预交金
        /// 增加了[bool]isReturn参数，如果是冲红票为True,正常收取票为False
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="prepay"></param>
        /// <param name="isReturn"></param>
        protected virtual void ReplacePrintPrepayInvoice(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, ArrayList alPrepay, bool isReturn)
        {
            if (patientInfo.IsEncrypt)
            {
                patientInfo.Name = Neusoft.FrameWork.WinForms.Classes.Function.Decrypt3DES(patientInfo.NormalName);
            }
            this.prepayPrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IPrepayPrint), 1) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IPrepayPrint;
            //regprint.SetPrintValue(regObj,regmr);
            //this.prepayPrint = new ucPrepayPrint();
            if (this.prepayPrint == null)
            {
                //this.prepayPrint = new ucPrepayPrint();
                return;
            }


            this.prepayPrint.SetValue(patientInfo, alPrepay);
            this.prepayPrint.Print();


        }

        /// <summary>
        /// 打印预交金
        /// 王宇修改， 控制冲红票打印负数，并且注明作废字样
        /// 增加了[bool]isReturn参数，如果是冲红票为True,正常收取票为False
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="prepay"></param>
        /// <param name="isReturn"></param>
        protected virtual void PrintPrepayInvoice(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay, bool isReturn)
        {
            if (patientInfo.IsEncrypt)
            {
                patientInfo.Name = Neusoft.FrameWork.WinForms.Classes.Function.Decrypt3DES(patientInfo.NormalName);
            }
            this.prepayPrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IPrepayPrint)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IPrepayPrint;
            //regprint.SetPrintValue(regObj,regmr);
            //this.prepayPrint = new ucPrepayPrint();
            if (this.prepayPrint == null)
            {
                //this.prepayPrint = new ucPrepayPrint();
                return;
            }


            this.prepayPrint.SetValue(patientInfo, prepay);
            this.prepayPrint.Print();


        }

        protected virtual void PrintPrepayPatientSign(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay)
        {
            if (patientInfo.IsEncrypt)
            {
                patientInfo.Name = Neusoft.FrameWork.WinForms.Classes.Function.Decrypt3DES(patientInfo.NormalName);
            }
            Neusoft.SOC.HISFC.InpatientFee.Interface.IBillPrint IBillPrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.SOC.HISFC.InpatientFee.Interface.IBillPrint)) as Neusoft.SOC.HISFC.InpatientFee.Interface.IBillPrint;
            if (IBillPrint == null)
            {
                return;
            }
            string errInfo = string.Empty;
            if (IBillPrint.SetData(patientInfo, prepay, ref errInfo) < 0)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("设置病人签名卡打印数据失败，原因：" + errInfo, 211);
                return;
            }
            IBillPrint.Print();
        }

        /// <summary>
        /// 预交金收取
        /// </summary>
        protected virtual void ReceivePrepay()
        {
            //{645F3DDE-4206-4f26-9BC5-307E33BD882C}
            string errText = string.Empty;
            if (!feeIntegrate.AfterDayBalanceCanFee(this.feeInpatient.Operator.ID, true, ref errText))
            {
                MessageBox.Show(errText);
                return;
            }

            //判断患者
            if (this.patientInfo == null)
            {
                return;
            }
            else
            {
                if (this.patientInfo.ID == null || this.patientInfo.ID.Trim() == "") return;
            }
            //金额判断
            decimal prepayCost = 0m;
            decimal prepayCost1 = 0m;
            decimal prepayCost2 = 0m;
            try
            {
                prepayCost = decimal.Parse(this.txtPreCost.Text);
            }
            catch
            {
                prepayCost = 0;
                this.txtPreCost.Text = "0.00";
            }
            try
            {
                prepayCost1 = decimal.Parse(this.txtPreCost1.Text);
            }
            catch
            {
                prepayCost1 = 0;
                this.txtPreCost1.Text = "0.00";
            }
            try
            {
                prepayCost2 = decimal.Parse(this.txtPreCost2.Text);
            }
            catch
            {
                prepayCost2 = 0;
                this.txtPreCost2.Text = "0.00";
            }
            if (prepayCost == 0 && prepayCost2 == 0 && prepayCost1 == 0)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("请输入预交金金额!", 111);
                this.txtPreCost.Focus();
                this.txtPreCost.SelectAll();
                return;
            }
            if ((prepayCost + prepayCost2 + prepayCost1) < 0)
            {

                Neusoft.FrameWork.WinForms.Classes.Function.Msg("预交金额应大于零!", 111);
                this.txtPreCost.Focus();
                this.txtPreCost.SelectAll();
                return;

            }

            string strTemp = Neusoft.FrameWork.Public.String.LowerMoneyToUpper(prepayCost + prepayCost2 + prepayCost1);

            //判断支付方式
            if ((prepayCost != 0 && (this.cmbPayType.Tag == null || this.cmbPayType.Tag.ToString() == string.Empty)) ||
                (prepayCost1 != 0 && (this.cmbTransType1.Tag == null || this.cmbTransType1.Tag.ToString() == string.Empty)) ||
                (prepayCost2 != 0 && (this.cmbTransType2.Tag == null || this.cmbTransType2.Tag.ToString() == string.Empty)))
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("请选择支付方式！", 111);
                this.cmbTransType1.Focus();
                return;
            }

            if (MessageBox.Show(strTemp, "预交金金额：", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                return;
            }

            //判断支付方式
            //if (this.cmbPayType.Tag == null || this.cmbPayType.Tag.ToString() == string.Empty)
            //{
            //    Neusoft.FrameWork.WinForms.Classes.Function.Msg("请选择支付方式！", 111);
            //    this.cmbPayType.Focus();
            //    return;
            //}



            //判断回车确认住院号

            if (this.patientInfo.PID.PatientNO != this.ucQueryInpatientNo.Text)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("请回车确认住院号", 111);
                return;
            }

            //判断封帐
            if ((this.feeInpatient.GetStopAccount(this.patientInfo.ID)) == "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者处于封帐状态,可能正在结算,请稍后再做此操作!", 111);
                return;
            }

            //事务连接1
            //Neusoft.FrameWork.Management.Transaction t = new Transaction(this.feeInpatient.Connection);
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            this.feeInpatient.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            //this.Person.SetTrans(t.Trans);
            //建立新插入预交金实体
            Neusoft.HISFC.Models.Fee.Inpatient.Prepay newPrepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
            Neusoft.HISFC.Models.Fee.Inpatient.Prepay newPrepay1 = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
            Neusoft.HISFC.Models.Fee.Inpatient.Prepay newPrepay2 = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();

            //提取发票号码
            //发票类型-预交金
            ArrayList alPrepay = new ArrayList();

            string InvoiceNo = "";
            //InvoiceNo = this.feeIntegrate.GetNewInvoiceNO(Neusoft.HISFC.Models.Fee.EnumInvoiceType.P);
            //InvoiceNo = this.feeIntegrate.GetNewInvoiceNO("P");
            InvoiceNo = this.feeIntegrate.GetReceipt_NO(); ;
            InvoiceNo = "YJ" + InvoiceNo.PadLeft(8, '0');
            if (InvoiceNo == null || InvoiceNo.Trim() == "")
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("请领取发票!", 111);
                return;
            }

            #region 扫码支付[支付平台]

            if (this.cmbPayType.Tag.ToString().Equals("WX") || this.cmbPayType.Tag.ToString().Equals("ZFB")) //走自己的支付平台
            {
                string authCode = Interaction.InputBox("请核实：扫码支付金额：" + prepayCost.ToString() + "元", "字符串", "", -1, -1);
                if (string.IsNullOrEmpty(authCode))
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("请将手机对准扫码器！", 111);
                    this.cmbPayType.Focus();
                    return;
                }
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("创建充值订单中...");
                Application.DoEvents();
                UnityPayService unityPayService = new UnityPayService();
                CreateOrderRequestDto requestDto = new CreateOrderRequestDto();
                requestDto.ApplicationOrderNo = System.DateTime.Now.ToString("yyyyMMddHHmmss") + "123456";
                requestDto.ClientCode = "ZDWY_GLACIER";
                requestDto.OpterCode = this.feeInpatient.Operator.ID;
                requestDto.OpterName = this.feeInpatient.Operator.Name;
                requestDto.OrderAmount = prepayCost;
                requestDto.PatientName = this.patientInfo.Name;
                requestDto.PatientNo = this.patientInfo.PID.CardNO;
                requestDto.OrderDate = DateTime.Now;
                requestDto.OrderBigType = OrderBigTypeEnum.CZOrder;
                requestDto.OrderSmallType = OrderSmallTypeEnum.IAP;
                requestDto.CZOrderInfo = new CZOrderInfo();
                requestDto.CZOrderInfo.OrderSmallType = OrderSmallTypeEnum.IAP;
                requestDto.CZOrderInfo.PatientNo = this.patientInfo.PID.PatientNO;
                requestDto.CZOrderInfo.PatientPhone = this.patientInfo.Kin.RelationPhone;
                requestDto.CZOrderInfo.PatientIdCard = this.patientInfo.IDCard;
                CreateOrderResponseDto responseDto = new CreateOrderResponseDto();
                var createOrderResult = unityPayService.CreateOrder(requestDto, ref responseDto);
                if (createOrderResult < 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg(unityPayService.errMsg, 111);
                    return;
                }
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("订单支付中...");
                PayOrderRequestDto payOrderRequestDto = new PayOrderRequestDto();
                payOrderRequestDto.ClientCode = requestDto.ClientCode;
                payOrderRequestDto.OpterCode = this.feeInpatient.Operator.ID;
                payOrderRequestDto.OpterName = this.feeInpatient.Operator.Name;
                payOrderRequestDto.ApplicationOrderNo = requestDto.ApplicationOrderNo;
                payOrderRequestDto.PatientNo = requestDto.PatientNo;
                payOrderRequestDto.OrderAmount = requestDto.OrderAmount;
                if (this.cmbPayType.Tag.ToString() == "WX")
                {
                    payOrderRequestDto.PayType = PayTypeEnum.WeChat_FKM;
                    payOrderRequestDto.WechatFKMInfo = new WechatFKMInfo();
                    payOrderRequestDto.WechatFKMInfo.AuthCode = authCode;
                }
                else if (this.cmbPayType.Tag.ToString() == "ZFB")
                {
                    payOrderRequestDto.PayType = PayTypeEnum.ZFB_FKM;
                    payOrderRequestDto.ZFBFKMInfo = new ZFBFKMInfo();
                    payOrderRequestDto.ZFBFKMInfo.AuthCode = authCode;
                }

                PayOrderResponseDto payOrderResponseDto = new PayOrderResponseDto();
                var payOrderResult = unityPayService.PayOrder(payOrderRequestDto, ref payOrderResponseDto);
                if (payOrderResult < 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg(unityPayService.errMsg, 111);
                    return;
                }
                if (payOrderResponseDto.PaymentStatus == PaymentStatusEnum.Fail)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付失败:" + payOrderResponseDto.PaymentDes, 111);
                    return;
                }

                UnityPayDB db = new UnityPayDB();
                FinTransRecord payRecordInfo = new FinTransRecord();
                payRecordInfo.Id = Guid.NewGuid().ToString();
                payRecordInfo.TransactionNo = InvoiceNo;
                payRecordInfo.TransType = "1";
                payRecordInfo.ClientCode = requestDto.ClientCode;
                payRecordInfo.PlatformOrderNo = responseDto.PlatformOrderNo;
                payRecordInfo.ClientCode = requestDto.ClientCode;
                payRecordInfo.ApplicationOrderNo = requestDto.ApplicationOrderNo;
                payRecordInfo.PayChannelCode = payOrderRequestDto.PayType.ToString();
                payRecordInfo.TransAmount = requestDto.OrderAmount;
                payRecordInfo.OrderBigType = requestDto.OrderBigType.ToString();
                payRecordInfo.OrderSmallType = requestDto.CZOrderInfo.OrderSmallType.ToString();
                payRecordInfo.PatientNo = requestDto.PatientNo;
                payRecordInfo.PatientName = requestDto.PatientName;
                payRecordInfo.CreatedCode = db.Operator.ID;
                payRecordInfo.CreatedName = db.Operator.Name;
                payRecordInfo.HospitalCode = "H44040200001";
                payRecordInfo.BusinessNo = this.patientInfo.ID;
                if (payOrderResponseDto.PaymentStatus == PaymentStatusEnum.Success)
                {
                    payRecordInfo.PayTransFinishTime = payOrderResponseDto.TransFinishTime;
                    if (!db.InsertPaySuccessInfo(payRecordInfo))
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付失败:" + db.Err, 111);
                        return;
                    }

                }
                else //支付中的话 需要循环调用查询支付结果接口
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("支付结果查询中...");
                    for (int i = 0; i < 20; i++)
                    {
                        //等待10s再去调用查询接口
                        if (i != 0)
                        {
                            Thread.Sleep(5000);
                        }

                        QueryOrderPayResultRequestDto queryOrderPayResultRequestDto = new QueryOrderPayResultRequestDto();
                        QueryOrderPayResultResponseDto queryOrderPayResultResponseDto = new QueryOrderPayResultResponseDto();
                        queryOrderPayResultRequestDto.ApplicationOrderNo = requestDto.ApplicationOrderNo;
                        queryOrderPayResultRequestDto.PlatformOrderNo = "";
                        var queryPayResult = unityPayService.QueryOrderPayResult(queryOrderPayResultRequestDto, ref queryOrderPayResultResponseDto);
                        if (queryPayResult < 0)
                        {
                            continue;
                        }
                        if (queryOrderPayResultResponseDto.PaymentStatus == PaymentStatusEnum.Fail)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付失败", 111);

                            return;
                        }
                        if (queryOrderPayResultResponseDto.PaymentStatus == PaymentStatusEnum.Processing)
                        {
                            if (i == 19)//最后一次查询如果都是 支付中的话 那么就当是支付失败处理吧
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                                Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付失败", 111);
                                return;
                            }
                            continue;
                        }
                        if (queryOrderPayResultResponseDto.PaymentStatus == PaymentStatusEnum.Success)
                        {
                            payRecordInfo.PayTransFinishTime = queryOrderPayResultResponseDto.TransFinishTime;
                            if (!db.InsertPaySuccessInfo(payRecordInfo))
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                                Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付失败:" + db.Err, 111);
                                return;
                            }
                            break;
                        }

                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付失败:" + unityPayService.errMsg, 111);
                        return;
                    }


                }

                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
                var info = db.GetPayRecordInfoForNoAndBusinessno(payRecordInfo.TransactionNo, payRecordInfo.BusinessNo, payRecordInfo.OrderBigType);
                if (info == null || info.PlatformOrderNo != payRecordInfo.PlatformOrderNo)
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付数据验证失败:" + unityPayService.errMsg, 111);
                    return;
                }

            }

            //this.cmbPayType.Tag.ToString().Equals("ZFB") || 
            if (this.cmbPayType.Tag.ToString().Equals("GHZFB") || this.cmbPayType.Tag.ToString().Equals("GHWX") || this.cmbPayType.Tag.ToString().Equals("GHRMB"))
            {
                bool isSucc = false;
                bool isRefund = false;
                string tradeNo = "";//扫码返回的交易流水号
                string authCode = Interaction.InputBox("请核实：扫码支付金额：" + prepayCost.ToString() + "元", "字符串", "", -1, -1);

                if (string.IsNullOrEmpty(authCode))
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("请将手机对准扫码器！", 111);
                    this.cmbPayType.Focus();
                    return;
                }
                string payMode = GetPayModeByPayBarCode(authCode);
                if (string.IsNullOrEmpty(payMode))
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("条形码无效！", 111);
                    this.cmbPayType.Focus();
                    return;
                }
                if (this.cmbPayType.Tag.ToString() != "GHZFB" && this.cmbPayType.Tag.ToString() != "GHWX" && this.cmbPayType.Tag.ToString() != "GHRMB")
                {
                    this.cmbPayType.Tag = payMode;
                }
                try
                {
                    if (newScanCodeYJJGL)//扫码墩启用
                    {
                        #region 扫码墩
                        Neusoft.HISFC.Models.ScanPay.PayMentInfo pMInfo = new Neusoft.HISFC.Models.ScanPay.PayMentInfo();
                        pMInfo.order_id = InvoiceNo;//His订单号
                        pMInfo.type = "4";//订单类型 1当天挂号 2预约挂号 3门诊缴费 4住院按金 5门诊预交金充值 6住院预交金充值
                        pMInfo.fee = prepayCost.ToString();//金额（元）
                        pMInfo.pay_code = authCode;//付款码，支持微信、支付宝，需与pay_type入参对应
                        pMInfo.patient_id = this.patientInfo.PID.PatientNO;//患者ID
                        pMInfo.patient_name = this.patientInfo.Name;//患者姓名
                        pMInfo.pay_type = authCode.StartsWith("13") ? "3" : "4";//支付类型。微信：3；支付宝：4
                        pMInfo.data_order_id = System.DateTime.Now.ToString("yyyyMMddHHmmss") + "123456";
                        if (this.cmbPayType.Tag.ToString().Equals("GHZFB") || this.cmbPayType.Tag.ToString().Equals("GHWX") || this.cmbPayType.Tag.ToString().Equals("GHRMB"))
                        {
                            pMInfo.pay_type = "5";//支付类型。微信：3；支付宝：4;工行：5
                            string[] result = GHService.Getpay(pMInfo.pay_code, pMInfo.data_order_id, (prepayCost * 100).ToString("0")).Split(',');
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
                                MessageBox.Show("扫码支付失败" + result[1].ToString(), "异常");
                                isSucc = false;
                            }
                        }
                        else
                        {
                            string result = string.Empty;
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("order_id", pMInfo.order_id);//His订单号
                            dic.Add("type", pMInfo.type);//订单类型 1当天挂号 2预约挂号 3门诊缴费 4住院按金 5门诊预交金充值 6住院预交金充值
                            dic.Add("fee", pMInfo.fee);//金额（元）
                            dic.Add("pay_code", pMInfo.pay_code);//付款码，支持微信、支付宝，需与pay_type入参对应
                            dic.Add("patient_id", pMInfo.patient_id);//患者ID
                            dic.Add("patient_name", pMInfo.patient_name);//患者姓名
                            dic.Add("pay_type", pMInfo.pay_type);//支付类型。微信：3；支付宝：4
                            result = Post(scanPaymentUrl, dic);
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
                                tradeNo = pMInfo.data_order_id;
                            }
                            else
                            {
                                isSucc = false;
                            }
                        }
                        Neusoft.HISFC.BizLogic.Fee.Outpatient outpMInfo = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                        //outpMInfo.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                        if (!outpMInfo.InsertPayMentInfo(pMInfo))
                        {
                            //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("插入挂号扫码支付记录表失败!");
                        }
                        #endregion
                    }
                    else
                    {
                        using (PushMsgService.PushMsginterfaceClient client = new Neusoft.HISFC.Components.InpatientFee.PushMsgService.PushMsginterfaceClient(new System.ServiceModel.BasicHttpBinding(), new System.ServiceModel.EndpointAddress("http://172.16.106.100:8005/PushMsgService.svc")))
                        {

                            string[] recipe = new string[] { InvoiceNo + "^" + prepayCost.ToString() };
                            string res = client.QrCodeFee("3", this.patientInfo.ID, recipe, Convert.ToDouble(prepayCost), authCode, "");
                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(res);
                            string resultCode = xml.SelectSingleNode("response/resultCode").InnerText;
                            string resultDesc = xml.SelectSingleNode("response/resultDesc").InnerText;

                            if (resultCode.Equals("0"))//成功
                            {
                                isSucc = true;
                                tradeNo = resultDesc;
                            }
                            else if (resultCode.Equals("1"))//失败
                            {
                                isSucc = false;
                            }
                            else if (resultCode.Equals("10003"))//等待患者支付
                            {
                                tradeNo = resultDesc;
                                if (MessageBox.Show("患者是否支付成功？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                {
                                    //查询订单状态
                                    string state = client.QueryOrder("", tradeNo, this.patientInfo.ID, "3");
                                    XmlDocument xml_state = new XmlDocument();
                                    xml_state.LoadXml(state);
                                    string orderState = xml_state.SelectSingleNode("response/orderState").InnerText;//0-未支付；1-已支付；2-已退费；
                                    if (!orderState.Equals("1"))//不成功
                                    {
                                        isRefund = true;
                                    }
                                    else//成功
                                    {
                                        isSucc = true;
                                    }
                                }
                                else //收费员取消收费
                                {
                                    isRefund = true;
                                }
                            }
                            else //其他
                            {
                                isRefund = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                if (isSucc == false)
                {
                    if (isRefund == true)
                    {
                        #region 退费
                        try
                        {
                            using (PushMsgService.PushMsginterfaceClient client = new Neusoft.HISFC.Components.InpatientFee.PushMsgService.PushMsginterfaceClient(new System.ServiceModel.BasicHttpBinding(), new System.ServiceModel.EndpointAddress("http://172.16.106.100:8005/PushMsgService.svc")))
                            {
                                string res_Cancel = client.CancelQrCodeFee("", tradeNo, Convert.ToDouble(prepayCost), "3", this.patientInfo.ID);
                                XmlDocument xml_Cancel = new XmlDocument();
                                xml_Cancel.LoadXml(res_Cancel);
                                string resultCode_Cancel = xml_Cancel.SelectSingleNode("response/resultCode").InnerText;
                                string resultDesc_Cancel = xml_Cancel.SelectSingleNode("response/resultDesc").InnerText;

                                //退费请求结果： 0-成功；1-失败
                                if (!resultCode_Cancel.Equals("0"))
                                {
                                    MessageBox.Show("退回扫码所扣费用失败，原因：" + resultDesc_Cancel, "异常");
                                }
                                else
                                {
                                    MessageBox.Show("扫码支付失败，已将费用退还到原有账户：" + resultDesc_Cancel, "异常");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("请联系系统管理员介入！", "异常");
                        }

                        #endregion
                    }

                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("扫码支付失败！请选择其他支付方式！：", 111);
                    this.cmbPayType.Focus();
                    return;
                }

            }
            #endregion

            #region 获取建行POS机扣费开关
            Neusoft.FrameWork.Models.NeuObject conObj = managerIntegrate.GetConstant("IsCCBPosOpen", "1");
            bool IsCCBPosOpen = Neusoft.FrameWork.Function.NConvert.ToBoolean(conObj.Memo);
            Neusoft.FrameWork.Models.NeuObject conObj1 = managerIntegrate.GetConstant("IsCCBPosOpen", "2");
            bool IsSDCCBPosOpen = Neusoft.FrameWork.Function.NConvert.ToBoolean(conObj1.Memo);
            #endregion

            #region 建行POS机支付 20191022
            if (this.cmbPayType.Tag.ToString().Equals("CCB") && IsCCBPosOpen)
            {
                if (IsSDCCBPosOpen)
                {
                    Neusoft.HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                    outOp.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                    Neusoft.HISFC.Models.POS.SDCCBPosInfo sdpayInfo = new Neusoft.HISFC.Models.POS.SDCCBPosInfo();
                    sdpayInfo.OperateType = "A0";
                    sdpayInfo.TransType = "30";
                    sdpayInfo.CardType = "01";
                    sdpayInfo.CashRegNo = "".PadRight(6, ' ');
                    sdpayInfo.CasherNo = "".PadRight(6, ' ');
                    sdpayInfo.Amount = (prepayCost * 100).ToString("0").PadLeft(12, '0');
                    sdpayInfo.CashTraceNo = "".PadRight(6, ' ');
                    sdpayInfo.OriginTraceNo = "".PadRight(6, ' ');
                    sdpayInfo.Reserved = "".PadRight(48, ' ');
                    string err = "";
                    Neusoft.HISFC.Models.POS.SDCCBPosOutInfo outInfo = new Neusoft.HISFC.Models.POS.SDCCBPosOutInfo();
                    int result = Neusoft.SOC.Local.RADT.ZhuHai.ZDWY.POS.SDCCBPos.CardTrans(0, sdpayInfo, ref outInfo, ref err);
                    if (result <= 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("杉德POS机扣费失败！" + err, "异常");
                        return;
                    }
                    else
                    {
                        sdpayInfo.Card_NO = this.patientInfo.PID.CardNO;//门诊号
                        sdpayInfo.InvoiceNo = InvoiceNo;//发票号
                        sdpayInfo.SourceFlag = "3";
                        sdpayInfo.State = "1";
                        sdpayInfo.SerialNumber = this.patientInfo.ID;

                        if (outOp.InsertSDPosInfo(sdpayInfo, outInfo) <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("杉德POS机扣费成功，交易记录保存失败，请手工操作退费！" + err, "异常");
                            return;
                        }
                    }
                }
                else
                {
                    #region 建行POS机扣费
                    Neusoft.HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                    outOp.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                    Neusoft.HISFC.Models.POS.CCBPosInfo payInfo = new Neusoft.HISFC.Models.POS.CCBPosInfo();
                    //payInfo.TransType = "2";//银行卡支付
                    //payInfo.Amount = (prepayCost * 100).ToString("0");//分为单位
                    //payInfo.Card_No = this.patientInfo.ID;
                    //payInfo.Invoice_No = InvoiceNo;
                    //payInfo.TransCheck = outOp.GetSysDateTime("yyyyMMddHHmmss");
                    //payInfo.OriTraceNo = "000000";
                    //payInfo.State = "1";

                    payInfo.OperType = "02";//银行卡支付
                    payInfo.Amount = (prepayCost * 100).ToString("0");//分为单位
                    payInfo.TotCost = prepayCost;
                    payInfo.TransCheck = outOp.GetSysDateTime("yyyyMMddHHmmss") + "123456";
                    payInfo.FeeDate = outOp.GetSysDateTime("yyyyMMdd");//消费日期
                    payInfo.VouchNo = "000001";//凭证号
                    payInfo.ReferenceNo = "000000000000";//参考号
                    payInfo.BatchNo = "000001";//批次号
                    payInfo.AuthNo = "000001";//授权码
                    payInfo.MerchantName = "3";//1挂号2门诊收费3住院押金4出院结算
                    payInfo.Card_No = this.patientInfo.ID;//门诊号
                    payInfo.Invoice_No = InvoiceNo;//发票号
                    payInfo.State = "1";

                    string error = string.Empty;
                    if (SOC.Local.RADT.ZhuHai.ZDWY.POS.CCBPosNew.CCBBankTrans(payInfo, ref error) != 1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("建行POS机初扣费失败!" + error, 111);
                        return;
                    }

                    if (payInfo.RspCode == "00")//扣费成功
                    {
                        //保存数据
                        if (outOp.InsertCCBPosNew(payInfo) < 0)  //保存成功 继续交易  失败 就退出
                        {
                            MessageBox.Show("保存扣费数据失败：" + outOp.Err + "请手工操作退费");
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            //如果没有保存成功  也不能交易成功  需要退掉   这个过程基本上是用不到的
                            return;
                        }
                    }
                    else
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("建行POS机结算返回为：" + payInfo.RspCode + "。处理失败", 111);
                        return;
                    }
                    #endregion
                }

            }
            #endregion

            //财务组


            Neusoft.FrameWork.Models.NeuObject finGroup = new Neusoft.FrameWork.Models.NeuObject();
            finGroup = this.feeInpatient.GetFinGroupInfoByOperCode(this.feeInpatient.Operator.ID);

            newPrepay.RecipeNO = InvoiceNo;

            //实体赋值
            if (this.pnlBankInfo.Visible)
            {
                Neusoft.HISFC.Models.Base.Bank bank = new Neusoft.HISFC.Models.Base.Bank();
                bank.ID = this.cmbBank.Tag.ToString();
                if (!string.IsNullOrEmpty(bank.ID))
                {
                    bank.Name = this.cmbBank.SelectedItem.Name;
                    bank.Account = this.txtBankAccount.Text.Trim();
                    cmbPayType.bank = bank;
                }

            }

            if (this.cmbPayType.Tag != null && decimal.Parse(this.txtPreCost.Text) > 0)
            {
                if (prepayCost != decimal.Parse(this.txtPreCost.Text))
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付金额" + prepayCost + "与记录金额" + this.txtPreCost.Text + "不一致！", 211);
                    return;
                }
                newPrepay.Name = this.patientInfo.Name;
                newPrepay.PrepayOper.ID = this.feeInpatient.Operator.ID;
                newPrepay.PrepayOper.Name = this.feeInpatient.Operator.Name;
                newPrepay.FT.PrepayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtPreCost.Text);
                newPrepay.Bank = this.cmbPayType.bank.Clone();
                newPrepay.PayType.ID = this.cmbPayType.Tag.ToString();
                newPrepay.Dept = this.patientInfo.PVisit.PatientLocation.Dept.Clone();
                newPrepay.BalanceState = "0";
                newPrepay.BalanceNO = 0;
                newPrepay.PrepayState = "0";
                newPrepay.IsTurnIn = false;
                newPrepay.FinGroup.ID = finGroup.ID;
                newPrepay.PrepayOper.OperTime = DateTime.Parse(this.feeInpatient.GetSysDateTime());
                newPrepay.TransferPrepayState = "0";

                //正常收或退预交金 ext_falg = "1";与结算召回区分，用字段 User01  By Maokb 060804
                newPrepay.User01 = "1";

                //增加备注字段
                newPrepay.User02 = this.txtMark.Text;

                //调用业务层组合业务


                if (this.feeInpatient.PrepayManager(this.patientInfo, newPrepay) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("收取失败!" + feeInpatient.Err, 211);
                    return;
                }
                else
                {
                    alPrepay.Add(newPrepay);
                }
            }

            if (this.cmbTransType1.Tag != null && decimal.Parse(this.txtPreCost1.Text) > 0)
            {
                if (prepayCost1 != decimal.Parse(this.txtPreCost1.Text))
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付金额" + prepayCost1 + "与记录金额" + this.txtPreCost1.Text + "不一致！", 211);
                    return;
                }
                if (this.cmbTransType1.Tag.ToString() == this.cmbPayType.Tag.ToString())
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("前两种支付方式相同!", 211);
                    return;
                }

                //实体赋值，银行实体暂时不理，貌似这边没有要求
                newPrepay1.RecipeNO = InvoiceNo;

                newPrepay1.Name = this.patientInfo.Name;
                newPrepay1.PrepayOper.ID = this.feeInpatient.Operator.ID;
                newPrepay1.PrepayOper.Name = this.feeInpatient.Operator.Name;
                newPrepay1.FT.PrepayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtPreCost1.Text);
                newPrepay1.Bank = this.cmbPayType.bank.Clone();
                newPrepay1.PayType.ID = this.cmbTransType1.Tag.ToString();
                newPrepay1.Dept = this.patientInfo.PVisit.PatientLocation.Dept.Clone();
                newPrepay1.BalanceState = "0";
                newPrepay1.BalanceNO = 0;
                newPrepay1.PrepayState = "0";
                newPrepay1.IsTurnIn = false;
                newPrepay1.FinGroup.ID = finGroup.ID;
                newPrepay1.PrepayOper.OperTime = DateTime.Parse(this.feeInpatient.GetSysDateTime());
                newPrepay1.TransferPrepayState = "0";

                //正常收或退预交金 ext_falg = "1";与结算召回区分，用字段 User01  By Maokb 060804
                newPrepay1.User01 = "1";

                //增加备注字段
                newPrepay1.User02 = this.txtMark.Text;

                if (this.feeInpatient.PrepayManager(this.patientInfo, newPrepay1) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("收取失败!" + feeInpatient.Err, 211);
                    return;
                }
                else
                {
                    alPrepay.Add(newPrepay1);
                }
            }

            if (this.cmbTransType2.Tag != null && decimal.Parse(this.txtPreCost2.Text) > 0)
            {
                if (prepayCost2 != decimal.Parse(this.txtPreCost2.Text))
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("支付金额" + prepayCost2 + "与记录金额" + this.txtPreCost2.Text + "不一致！", 211);
                    return;
                }
                if (this.cmbTransType2.Tag.ToString() == this.cmbPayType.Tag.ToString())
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("最后支付方式与第一支付方式相同!", 211);
                    return;
                }
                if (this.cmbTransType2.Tag.ToString() == this.cmbTransType1.Tag.ToString())
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("后两种支付方式相同!", 211);
                    return;
                }

                //实体赋值，银行实体暂时不理，貌似这边没有要求
                newPrepay2.RecipeNO = InvoiceNo;

                newPrepay2.Name = this.patientInfo.Name;
                newPrepay2.PrepayOper.ID = this.feeInpatient.Operator.ID;
                newPrepay2.PrepayOper.Name = this.feeInpatient.Operator.Name;
                newPrepay2.FT.PrepayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtPreCost2.Text);
                newPrepay2.Bank = this.cmbPayType.bank.Clone();
                newPrepay2.PayType.ID = this.cmbTransType2.Tag.ToString();
                newPrepay2.Dept = this.patientInfo.PVisit.PatientLocation.Dept.Clone();
                newPrepay2.BalanceState = "0";
                newPrepay2.BalanceNO = 0;
                newPrepay2.PrepayState = "0";
                newPrepay2.IsTurnIn = false;
                newPrepay2.FinGroup.ID = finGroup.ID;
                newPrepay2.PrepayOper.OperTime = DateTime.Parse(this.feeInpatient.GetSysDateTime());
                newPrepay2.TransferPrepayState = "0";

                //正常收或退预交金 ext_falg = "1";与结算召回区分，用字段 User01  By Maokb 060804
                newPrepay2.User01 = "1";

                //增加备注字段
                newPrepay2.User02 = this.txtMark.Text;

                if (this.feeInpatient.PrepayManager(this.patientInfo, newPrepay2) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("收取失败!" + feeInpatient.Err, 211);
                    return;
                }
                else
                {
                    alPrepay.Add(newPrepay2);
                }
            }

            //刷新余额标记
            this.txtFreeCost.Text = (Neusoft.FrameWork.Public.String.FormatNumber(decimal.Parse(this.txtFreeCost.Text), 2) + newPrepay.FT.PrepayCost + newPrepay1.FT.PrepayCost + newPrepay2.FT.PrepayCost).ToString();
            this.txtSumPreCost.Text = (Neusoft.FrameWork.Public.String.FormatNumber(decimal.Parse(this.txtSumPreCost.Text), 2) + newPrepay.FT.PrepayCost + newPrepay1.FT.PrepayCost + newPrepay2.FT.PrepayCost).ToString();

            #region HL7发送消息到平台
            if (InterfaceManager.GetIADT() != null)
            {
                ArrayList alprepay = new ArrayList();
                alprepay.Add(newPrepay);
                if (InterfaceManager.GetIADT().Prepay(this.patientInfo, alprepay, "2") < 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show(this, "个人体检取消登记失败，请向系统管理员报告错误信息：" + InterfaceManager.GetIADT().Err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;

                }
            }

            #endregion

            Neusoft.FrameWork.Management.PublicTrans.Commit();
            Neusoft.FrameWork.WinForms.Classes.Function.Msg("预交金收取成功!", 111);
            //打印预交金发票


            //重新检索预交金记录
            this.QueryPatientPrepay(this.patientInfo);

            if (isPrintInvoice)
            {
                this.PrintPrepayInvoice(this.patientInfo, alPrepay, false);
            }

            //DialogResult dia;
            //frmNotice frmNotice = new frmNotice();
            //frmNotice.label1.Text = "是否收取预交金?";

            //frmNotice.ShowDialog();

            //dia = frmNotice.dr;

            //if (dia == DialogResult.No)
            //{
            //    //DialogResult diaWarning = MessageBox.Show("确定预览时没有打印预交金发票吗？误操作会造成浪费一张发票！", "警告！",
            //    //                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            //    //if (diaWarning == DialogResult.Yes)
            //    //{
            //    //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //    //    return;
            //    //}
            //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //    return;
            //}

            //Neusoft.FrameWork.Management.PublicTrans.Commit();
            //Neusoft.FrameWork.WinForms.Classes.Function.Msg("预交金收取成功!", 111);

            //
            this.txtPreCost.Text = "";
            this.txtPreCost1.Text = "";
            this.txtPreCost2.Text = "";
            this.txtMark.Text = "";
            this.Clear();
            this.ucQueryInpatientNo.Text = "";
            this.ucQueryInpatientNo.Focus();
            //显示下一张收据号
            //this.GetNextInvoiceNO();
            this.ucQueryInpatientNo.Focus();

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

        /// <summary>
        /// 预交金返还判断
        /// </summary>
        /// <param name="prepay"></param>
        /// <returns></returns>
        private bool ValidReturnPrepay(Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay)
        {
            //{645F3DDE-4206-4f26-9BC5-307E33BD882C}
            string errText = string.Empty;
            if (!feeIntegrate.AfterDayBalanceCanFee(this.feeInpatient.Operator.ID, true, ref errText))
            {
                MessageBox.Show(errText);
                return false;
            }

            if (prepay.PrepayState == "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经作废!不能进行再次作废操作!", 111);
                return false;
            }
            if (prepay.PrepayState == "2")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经进行过补打操作,已经成为作废发票,不能再作废!", 111);
                return false;
            }
            if (prepay.BalanceState == "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该票据已经结算过不能作废!!", 111);
                return false;
            }
            #region 作废,日结后也可以做返还操作
            //if (prepay.Memo == "1")
            //{
            //    Neusoft.FrameWork.WinForms.Classes.Function.Msg("该票据已经日结不能作废!!", 111);
            //    return false;
            //}
            #endregion
            if (prepay.TransferPrepayState == "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金为结算的转押金还没有进行正常打印操作,不能作废!", 111);
                return false;
            }
            if (!isCanDealBefore)
            {
                if (prepay.PrepayOper.OperTime < feeInpatient.GetDateTimeFromSysDateTime().Date)
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("不能作废当天前的预交金!", 111);
                    return false;
                }
            }

            if (!isCanQuitOtherOper)
            {
                if ((prepay.PrepayOper.ID != feeInpatient.Operator.ID) && (prepay.PrepayOper.OperTime.Date == feeInpatient.GetDateTimeFromSysDateTime().Date))
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("该发票为操作员" + prepay.PrepayOper.ID + "收取,您没有权限作废！", 111);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 预交金返还
        /// </summary>
        protected virtual void ReturnPrepay()
        {


            //有效操作判断
            if (this.fpPrepay_Sheet1.ActiveRowIndex < 0) return;
            if (this.fpPrepay_Sheet1.Rows.Count <= 0) return;
            if (this.patientInfo == null)
            {
                return;
            }
            else
            {
                if (this.patientInfo.ID == null || this.patientInfo.ID.Trim() == "") return;
            }

            Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayOne = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
            prepayOne = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)this.fpPrepay_Sheet1.ActiveRow.Tag;

            if (prepayOne == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("请选择一条预交金记录", 111);
                return;
            }

            prepayOne = feeInpatient.QueryPrePay(this.patientInfo.ID, prepayOne.ID);
            if (prepayOne == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("查询预交金信息失败！" + feeInpatient.Err, 111);
                return;
            }

            //存储需要退的预交金实体数组
            ArrayList alPrepay = new ArrayList();

            if (!ValidReturnPrepay(prepayOne)) return;

            if (!string.IsNullOrEmpty(prepayOne.RecipeNO))
            {
                for (int i = 0; i < this.fpPrepay_Sheet1.RowCount; i++)
                {
                    Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayTmp = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)this.fpPrepay_Sheet1.Rows[i].Tag;
                    if (prepayTmp != null)
                    {
                        if (!string.IsNullOrEmpty(prepayTmp.RecipeNO) && prepayTmp.RecipeNO.ToString() == prepayOne.RecipeNO.ToString()
                            && prepayTmp.BalanceState == prepayOne.BalanceState)
                        {
                            prepayTmp = feeInpatient.QueryPrePay(this.patientInfo.ID, prepayTmp.ID);
                            if (prepayTmp == null)
                            {
                                Neusoft.FrameWork.WinForms.Classes.Function.Msg("查询预交金信息失败！" + feeInpatient.Err, 111);
                                return;
                            }
                            if (!ValidReturnPrepay(prepayTmp))
                                return;
                            alPrepay.Add(prepayTmp);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }

            DialogResult r = Neusoft.FrameWork.WinForms.Classes.Function.Msg("是否作废发票号为" + prepayOne.RecipeNO + "的预交金?", 422);
            if (r == DialogResult.No) return;
            //判断封帐
            if ((this.feeInpatient.GetStopAccount(this.patientInfo.ID)) == "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者处于封帐状态,可能正在结算,请稍后再做此操作!", 111);
                return;
            }
            if (alPrepay == null || alPrepay.Count == 0)
            {
                return;
            }
            #region 获取扫码支付可退金额
            string SPayType = string.Empty;//扫码支付退款支付方式
            using (Neusoft.HISFC.BizLogic.Fee.Outpatient outPat = new Neusoft.HISFC.BizLogic.Fee.Outpatient())
            {
                decimal totalFee = -1;
                decimal totalRefundFee = -1;
                decimal refundableFee = -1;

                string ORDER_ID = string.Empty;
                if (outPat.GetScanreFindByORDERID(RefundableUrl, this.patientInfo.PID.PatientNO, prepayOne.RecipeNO, "4", ref ORDER_ID, ref  SPayType, ref totalFee, ref totalRefundFee, ref refundableFee))
                {
                    if (totalFee != refundableFee)
                    {
                        Neusoft.HISFC.BizLogic.Manager.Constant con = new Neusoft.HISFC.BizLogic.Manager.Constant();
                        ArrayList ScanPayDirectRefundList = con.GetList("ScanPayDirectRefund");//获取有直接退款权限的用户
                        Neusoft.HISFC.Models.Base.Employee empl = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;//当前用户
                        bool Refund = false;
                        for (int i = 0; i < ScanPayDirectRefundList.Count; i++)//是否有退款权限
                        {
                            if (ScanPayDirectRefundList[i].ToString() == empl.ID)
                            {
                                Refund = true;
                                break;
                            }
                        }
                        if (Refund)
                        {
                            MessageBox.Show("此订单总支付金额不等于可退金额。请核对后再进行操作！" + System.Environment.NewLine + "订单:" + ORDER_ID + System.Environment.NewLine + "支付方式:" + SPayType + System.Environment.NewLine + "总支付金额:" + totalFee + System.Environment.NewLine + "已退款金额:" + totalRefundFee + System.Environment.NewLine + "可退金额:" + refundableFee);
                        }
                        else
                        {
                            MessageBox.Show("此订单总支付金额不等于可退金额。请联系收费处主管！");
                            return;
                        }
                    }
                }
            }
            #endregion
            ZFPTOrder zfptOrd = null;//支付平台订单
            #region 获取支付平台订单信息
            if (ZFPTReFundZYYJ)
            {
                if (prepayOne.PrepayOper.ID == "00W999")
                {
                    string regType = "5";//订单类型
                    string zzsborderID = this.feeInpatient.GetZZSBORDERID(prepayOne.RecipeNO, this.patientInfo.ID, regType);
                    if (zzsborderID != string.Empty)
                    {
                        zfptOrd = feeInpatient.GetZFPTOrder(zzsborderID);
                    }
                }
            }
            #endregion

            HISFC.Components.InpatientFee.Controls.frmMulTransType frmMul = new Neusoft.HISFC.Components.InpatientFee.Controls.frmMulTransType();
            frmMul.PrePays = alPrepay;
            DialogResult dr = frmMul.ShowDialog();
            if (dr == DialogResult.OK)
            {
                alPrepay = frmMul.PrePays;
            }
            else
            {
                return;
            }

            //退费支付方式提示
            string strRetPrepayInfo = frmMul.GetRetPrePaysInfo();

            //事务连接
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            this.feeInpatient.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay in alPrepay)
            {
                //原有发票号码
                prepay.OrgInvoice.ID = prepay.RecipeNO;
                //判断负记录走新发票号码
                if (this.IsReturnNewInvoice)
                {
                    //提取发票号码
                    //发票类型-预交金
                    string InvoiceNo = "";
                    //InvoiceNo = this.feeIntegrate.GetNewInvoiceNO(Neusoft.HISFC.Models.Fee.EnumInvoiceType.P);
                    InvoiceNo = this.feeIntegrate.GetNewInvoiceNO("P");
                    if (InvoiceNo == null || InvoiceNo == "")
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("提取发票出错!", 211);
                        return;
                    }
                    prepay.RecipeNO = InvoiceNo;
                }
                //{EC04199C-9F39-48f1-BCFE-98430D9962E6}
                prepay.IsPrint = IsPrintReturn;

                #region 作废
                //HISFC.Components.InpatientFee.Controls.ucTransType trasType = new Neusoft.HISFC.Components.InpatientFee.Controls.ucTransType(prepay);
                //if (trasType.ShowDialog() != DialogResult.OK)
                //{
                //    return;
                //}
                #endregion

                #region 获取建行POS机扣费开关
                Neusoft.FrameWork.Models.NeuObject conObj = managerIntegrate.GetConstant("IsCCBPosOpen", "1");
                bool IsCCBPosOpen = Neusoft.FrameWork.Function.NConvert.ToBoolean(conObj.Memo);
                Neusoft.FrameWork.Models.NeuObject conObj1 = managerIntegrate.GetConstant("IsCCBPosOpen", "2");
                bool IsSDCCBPosOpen = Neusoft.FrameWork.Function.NConvert.ToBoolean(conObj1.Memo);
                #endregion

                #region 建行POS机退费 20191022
                if (prepay.PayType.ID == "CCB" && IsCCBPosOpen)
                {
                    if (IsSDCCBPosOpen)
                    {
                        Neusoft.HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                        outOp.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                        var sdPosRecordInfo = outOp.GetSDPosRecordInfo(this.patientInfo.ID, prepay.OrgInvoice.ID, "3");
                        if (sdPosRecordInfo == null)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("POS机退费失败,未找到对应收费记录！" + outOp.Err, "异常");
                            return;
                        }
                        //if (sdPosRecordInfo.OutAmount != (objPay.FT.TotCost * 100).ToString("0").PadLeft(12, '0'))
                        //{
                        //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //    MessageBox.Show("POS机退费失败,退费金额与收费金额不一致！收费金额为" + sdPosRecordInfo.OutAmount + "分，退费金额为：" + (objPay.FT.TotCost * 100).ToString("0").PadLeft(12, '0') + outOp.Err, "异常");
                        //    return;
                        //}
                        Neusoft.HISFC.Models.POS.SDCCBPosInfo sdpayInfo = new Neusoft.HISFC.Models.POS.SDCCBPosInfo();
                        sdpayInfo.OperateType = "A0";
                        sdpayInfo.TransType = "40";
                        sdpayInfo.CardType = "01";
                        sdpayInfo.CashRegNo = "".PadRight(6, ' ');
                        sdpayInfo.CasherNo = "".PadRight(6, ' ');
                        sdpayInfo.Amount = (sdPosRecordInfo.Amount).PadLeft(12, '0');
                        sdpayInfo.CashTraceNo = "".PadRight(6, ' ');
                        sdpayInfo.OriginTraceNo = sdPosRecordInfo.Systracdno.PadRight(6, ' ');
                        sdpayInfo.Reserved = "".PadRight(48, ' ');
                        string err = "";
                        Neusoft.HISFC.Models.POS.SDCCBPosOutInfo outInfo = new Neusoft.HISFC.Models.POS.SDCCBPosOutInfo();
                        outOp.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                        int res = Neusoft.SOC.Local.RADT.ZhuHai.ZDWY.POS.SDCCBPos.CardTrans(0, sdpayInfo, ref outInfo, ref err);
                        if (res <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("杉德POS机扣费失败！" + err, "异常");
                            return;
                        }
                        else
                        {
                            if (!outOp.UpdateSDPosRecordInfoState(sdPosRecordInfo.RecordId))
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show("更新POS机交易记录表失败,请联系信息科！" + outOp.Err, "异常");
                                return;
                            }
                        }
                    }
                    else
                    {
                        #region 建行POS机退费
                        Neusoft.HISFC.BizLogic.Fee.Outpatient outOp = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                        Neusoft.HISFC.Models.POS.CCBPosInfo infos = outOp.GetCCBPosInfosNew(prepay.OrgInvoice.ID);
                        if (infos != null)
                        {
                            infos.OperType = "04";//银行卡退货
                            infos.TransCheck = outOp.GetSysDateTime("yyyyMMddHHmmss") + "123456";
                            infos.State = "1";
                            infos.MerchantName = "3";//1挂号2门诊收费3住院押金4出院结算
                            string error = string.Empty;

                            if (SOC.Local.RADT.ZhuHai.ZDWY.POS.CCBPosNew.CCBBankTrans(infos, ref error) != 1)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                //MessageBox.Show("建行POS机退费失败！" + error, "异常");
                                Neusoft.FrameWork.WinForms.Classes.Function.Msg("建行POS机退费失败！" + error, 211);
                                return;
                            }

                            if (infos.RspCode == "00")//撤销成功
                            {
                                //保存数据
                                if (outOp.InsertCCBPosNew(infos) < 0)  //保存成功 继续交易  失败 就退出
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show("保存撤销数据失败：" + outOp.Err + "请联系信息科");
                                    //如果没有保存成功  也不能交易成功  需要退掉   这个过程基本上是用不到的
                                    return;
                                }
                            }
                            else
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                Neusoft.FrameWork.WinForms.Classes.Function.Msg("建行POS机结算返回为：" + infos.RspCode + "。处理失败：" + outOp.Err, 211);
                                //MessageBox.Show("建行POS机结算返回为：" + infos.RspCode + "。处理失败：" + outOp.Err);
                                return;
                            }
                        }
                        else
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            // MessageBox.Show("建行POS机退费失败！" + "没有找到扣费数据", "异常");
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg("建行POS机退费失败！没有找到扣费数据", 211);
                            return;
                        }
                        #endregion
                    }

                }

                #endregion

                //支付平台微信退款
                if (prepay.PayType.ID == "WX" || prepay.PayType.ID == "ZFB")
                {
                    UnityPayDB _db = new UnityPayDB();
                    UnityPayService _service = new UnityPayService();
                    try
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("退款中...");
                        Application.DoEvents();
                        var payRecordInfo = _db.GetPayRecordInfoForNo(prepay.RecipeNO);
                        if (payRecordInfo == null)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("微信退款失败:未找到" + prepay.RecipeNO + "对应的支付信息.");
                            return;
                        }

                        RefundOrderRequestDto refundOrderReqDto = new RefundOrderRequestDto();
                        refundOrderReqDto.ClientCode = payRecordInfo.ClientCode;//"ZDWY_GLACIER";
                        refundOrderReqDto.OpterCode = _db.Operator.ID;
                        refundOrderReqDto.OpterCode = _db.Operator.ID;
                        refundOrderReqDto.OpterName = _db.Operator.Name;
                        refundOrderReqDto.RefundAmount = prepay.FT.PrepayCost;
                        refundOrderReqDto.ApplicationOrderNo = payRecordInfo.ApplicationOrderNo;
                        refundOrderReqDto.ApplicationRefundOrderNo = "WY_CK" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                        RefundOrderResponseDto RefundOrderRepDto = new RefundOrderResponseDto();
                        if (_service.RefundOrder(refundOrderReqDto, ref RefundOrderRepDto) < 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("退款失败:" + _service.errMsg);
                            return;
                        }
                        if (RefundOrderRepDto.RefundStatus != RefundStatusEnum.SUCCESS && RefundOrderRepDto.RefundStatus != RefundStatusEnum.PROCESSING)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("退款失败:" + RefundOrderRepDto.RefundDes);
                            return;
                        }
                        //插入退款交易记录表
                        FinTransRecord recordInfo = new FinTransRecord();
                        recordInfo.Id = Guid.NewGuid().ToString();
                        recordInfo.TransType = "2";
                        recordInfo.TransactionNo = prepay.RecipeNO;
                        recordInfo.PlatformOrderNo = payRecordInfo.PlatformOrderNo;
                        recordInfo.ClientCode = refundOrderReqDto.ClientCode;
                        recordInfo.ApplicationOrderNo = payRecordInfo.ApplicationOrderNo;
                        recordInfo.ApplicationRefundOrderNo = RefundOrderRepDto.ApplicationRefundOrderNo;
                        recordInfo.PlatformRefundOrderNo = RefundOrderRepDto.PlatformRefundOrderNo;
                        recordInfo.PayChannelCode = payRecordInfo.PayChannelCode;
                        recordInfo.RefundTransFinishTime = RefundOrderRepDto.TransFinishTime;
                        recordInfo.TransAmount = refundOrderReqDto.RefundAmount;
                        recordInfo.HospitalCode = payRecordInfo.HospitalCode;
                        recordInfo.OrderBigType = payRecordInfo.OrderBigType;
                        recordInfo.OrderSmallType = payRecordInfo.OrderSmallType;
                        recordInfo.PatientNo = payRecordInfo.PatientNo;
                        recordInfo.PatientName = payRecordInfo.PatientName;
                        recordInfo.CreatedCode = refundOrderReqDto.OpterCode;
                        recordInfo.CreatedName = refundOrderReqDto.OpterName;
                        recordInfo.BusinessNo = this.patientInfo.ID;
                        if (!_db.InsertRefundSuccessInfo(recordInfo))
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("退款失败:" + _db.Err);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("退款失败:" + ex.Message);
                        return;
                    }
                    finally
                    {

                        Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();

                    }



                }

                #region 扫码墩退款
                //ZFB
                if ((prepay.PayType.ID == "111111") && newScanReFundZYYJJTF)
                {
                    Neusoft.HISFC.Components.OutpatientFee.Forms.frmScanReFind frmSRF = new Neusoft.HISFC.Components.OutpatientFee.Forms.frmScanReFind(this.patientInfo.PID.PatientNO, prepay.RecipeNO, prepay.FT.PrepayCost.ToString(), "4", prepay.ID, prepay.Name);
                    frmSRF.ShowDialog();
                    if (frmSRF.Cancel)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        return;
                    }
                    Neusoft.HISFC.Models.ScanPay.ScanRefundInfo ScanRefundInfo = null;//微信/支付宝（扫码墩）退款信息类
                    ScanRefundInfo = frmSRF.ScanRefundInfo;
                    if (ScanRefundInfo != null)
                    {
                        Neusoft.HISFC.BizLogic.Fee.Outpatient outpMInfo = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                        if (!outpMInfo.InsertPaySCANREFUND(ScanRefundInfo))
                        {
                            MessageBox.Show("插入退款记录表失败!");
                        }

                    }
                    if (!frmSRF.Result)//退款失败
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("微信、支付宝退款失败");


                    }
                    if (!frmSRF.Result)//退款失败
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("微信、支付宝退款失败");

                        return;
                    }
                }
                if ((prepay.PayType.ID == "GHZFB") || (prepay.PayType.ID == "GHWX") || (prepay.PayType.ID == "GHRMB"))
                {
                    Neusoft.HISFC.Models.ScanPay.ScanRefundInfo ScanRefundInfo = new Neusoft.HISFC.Models.ScanPay.ScanRefundInfo();//微信/支付宝（扫码墩）退款信息类
                    Neusoft.HISFC.BizLogic.Fee.Outpatient outpMInfo = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
                    ScanRefundInfo.refund_order_id = prepay.RecipeNO;//HIS订单号
                    ScanRefundInfo.date_refund_order_id = System.DateTime.Now.ToString("yyyyMMddHHmmss") + "123";
                    ScanRefundInfo.REFUNDTYPE = "4";
                    ScanRefundInfo.refund_fee = prepay.FT.PrepayCost.ToString();
                    ScanRefundInfo.ORDER_ID = outpMInfo.GetDataorderid(ScanRefundInfo.refund_order_id);
                    string[] resultl = GHService.GetRefund(ScanRefundInfo.ORDER_ID, ScanRefundInfo.date_refund_order_id.ToString(), (prepay.FT.PrepayCost * 100).ToString("0"), feeInpatient.Operator.ID).Split(',');

                    if (resultl[0].ToString() == "退款失败")
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("工行退款失败");
                        return;
                    }
                    ScanRefundInfo.CODE = "0";
                    ScanRefundInfo.MSG = resultl[0].ToString();
                    if (ScanRefundInfo != null)
                    {

                        if (!outpMInfo.InsertPaySCANREFUND(ScanRefundInfo))
                        {
                            MessageBox.Show("插入退款记录表失败!");

                        }
                    }
                }

                #endregion
                #region 支付平台退款

                if (ZFPTReFundZYYJ)
                {
                    if (zfptOrd != null && prepay.PayType.ID == "YBXYF")
                    {
                        frmZFPTRefundZYYJJ frmZFPTTK = new frmZFPTRefundZYYJJ(zfptOrd);
                        frmZFPTTK.ShowDialog();
                        if (frmZFPTTK.Cancel)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();

                            return;
                        }

                    }


                }


                #endregion
                //调用业务层组合业务返还预交金
                if (this.feeInpatient.PrepayManagerReturn(this.patientInfo, prepay) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err + "作废失败!", 211);
                    return;
                }
                //刷新余额标记
                this.txtFreeCost.Text = (Neusoft.FrameWork.Public.String.FormatNumber(decimal.Parse(this.txtFreeCost.Text), 2) + prepay.FT.PrepayCost).ToString();
                this.txtSumPreCost.Text = (Neusoft.FrameWork.Public.String.FormatNumber(decimal.Parse(this.txtSumPreCost.Text), 2) + prepay.FT.PrepayCost).ToString();

                #region HL7发送消息到平台
                if (InterfaceManager.GetIADT() != null)
                {
                    ArrayList alprepay = new ArrayList();
                    alprepay.Add(prepay);
                    if (InterfaceManager.GetIADT().Prepay(this.patientInfo, alprepay, "1") < 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(this, "个人体检取消登记失败，请向系统管理员报告错误信息：" + InterfaceManager.GetIADT().Err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;

                    }
                }

                #endregion
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();
            Neusoft.FrameWork.WinForms.Classes.Function.Msg("预交金退费成功!\n" + strRetPrepayInfo, 111);
            //重新检索预交金记录
            this.QueryPatientPrepay(this.patientInfo);
            //打印冲红发票;
            if (this.IsPrintReturn)
            {
                this.PrintPrepayInvoice(this.patientInfo, alPrepay, true);
            }

            if (this.isPrintPatientSign)
            {
                this.PrintPrepayPatientSign(this.patientInfo, prepayOne);
            }
        }

        /// <summary>
        /// 预交金重打，作废原来号码，产生新的一个号
        /// </summary>
        protected virtual void ReprintPrepay()
        {
            if (this.fpPrepay_Sheet1.ActiveRowIndex < 0) return;
            if (this.fpPrepay_Sheet1.Rows.Count <= 0) return;
            if (this.patientInfo == null)
            {
                return;
            }
            else
            {
                if (this.patientInfo.ID == null || this.patientInfo.ID.Trim() == "") return;
            }

            ArrayList alPrepay = new ArrayList();

            Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
            prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)this.fpPrepay_Sheet1.ActiveRow.Tag;

            if (prepay == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("请选择一条预交金记录", 111);
                return;
            }
            if (!string.IsNullOrEmpty(prepay.RecipeNO))
            {
                for (int i = 0; i < this.fpPrepay_Sheet1.RowCount; i++)
                {
                    Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayTmp = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)this.fpPrepay_Sheet1.Rows[i].Tag;
                    if (prepayTmp != null)
                    {
                        if (!string.IsNullOrEmpty(prepayTmp.RecipeNO) && prepayTmp.RecipeNO.ToString() == prepay.RecipeNO.ToString())
                        {
                            alPrepay.Add(prepayTmp);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            if (alPrepay == null || alPrepay.Count == 0)
            {
                return;
            }
            else
            {
                Neusoft.FrameWork.Management.ControlParam controlParm = new Neusoft.FrameWork.Management.ControlParam();
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prePayT in alPrepay)
                {
                    if (prePayT.PrepayState == "1")
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经作废!不能进行重打操作!", 111);
                        return;
                    }
                    if (prePayT.PrepayState == "2")
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经进行过重打操作,已经成为作废发票,不能再重打!", 111);
                        return;
                    }
                    if (prePayT.BalanceState == "1")
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该票据已经结算过不能重打!!", 111);
                        return;
                    }
                    if (prePayT.TransferPrepayState == "1")
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金为结算的转押金还没有进行正常打印操作,不能重打!", 111);
                        return;
                    }

                    string limitDays = "";

                    limitDays = controlParm.QueryControlerInfo("100022");
                    if (limitDays == null || limitDays == "")
                        limitDays = "";
                    if (limitDays.Trim() != "")
                    {
                        if ((this.feeInpatient.GetDateTimeFromSysDateTime().Date - prePayT.PrepayOper.OperTime.Date).Days > Neusoft.FrameWork.Function.NConvert.ToInt32(limitDays))
                        {
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg("预交金发生间隔超过" + limitDays + "天,不能进行重打操作!", 111);
                            return;
                        }
                    }
                }
            }
            DialogResult r = Neusoft.FrameWork.WinForms.Classes.Function.Msg("是否重打发票号为" + prepay.RecipeNO + "的预交金?", 422);
            if (r == DialogResult.No) return;
            //判断封帐
            if ((this.feeInpatient.GetStopAccount(this.patientInfo.ID)) == "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者处于封帐状态,可能正在结算,请稍后再做此操作!", 111);
                return;
            }
            //事务连接
            //Transaction t = new Transaction(this.feeInpatient.Connection);
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            this.feeInpatient.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            //提取发票号码
            //发票类型-预交金
            ArrayList alprepay = new ArrayList();
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayRp in alPrepay)
            {
                string returnInvoice = "";
                if (this.IsReturnNewInvoice)
                {
                    returnInvoice = this.feeIntegrate.GetNewInvoiceNO("P");
                    if (returnInvoice == null || returnInvoice == "")
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("提取发票出错!", 211);
                        return;
                    }
                }

                string invoiceNo = "";
                invoiceNo = this.feeIntegrate.GetNewInvoiceNO("P");
                if (invoiceNo == null || invoiceNo == "")
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("提取发票出错!", 211);
                    return;
                }
                //调用组合业务处理正负记录
                Neusoft.HISFC.Models.Fee.Inpatient.Prepay returnPrepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
                //{EC04199C-9F39-48f1-BCFE-98430D9962E6}
                prepayRp.IsPrint = IsPrintReturn;
                if (this.feeInpatient.PrepaySignOperation(prepayRp, this.patientInfo, invoiceNo, returnInvoice, ref returnPrepay) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err + "重打失败!", 211);
                    return;
                }

                alprepay.Add(returnPrepay);
                #region HL7发送消息到平台
                if (InterfaceManager.GetIADT() != null)
                {
                    alprepay.Add(prepayRp);

                    if (InterfaceManager.GetIADT().Prepay(this.patientInfo, alprepay, "2") < 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(this, "个人体检取消登记失败，请向系统管理员报告错误信息：" + InterfaceManager.GetIADT().Err, "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;

                    }
                }

                #endregion
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();

            if (this.IsPrintReturn)
            {
                this.PrintPrepayInvoice(this.patientInfo, alprepay, true);
            }
            //打印预交金发票
            this.PrintPrepayInvoice(this.patientInfo, alPrepay, false);
            //重新检索预交金记录
            this.QueryPatientPrepay(this.patientInfo);
            //显示下一张收据号
            //this.GetNextInvoiceNO();
            Neusoft.FrameWork.WinForms.Classes.Function.Msg("重打完毕！", 111);
        }

        /// <summary>
        /// 预交金补打，直接查询预交金信息打印，不走号
        /// </summary>
        protected virtual void QueryAndPrintPrepay()
        {
            if (this.fpPrepay_Sheet1.ActiveRowIndex < 0) return;
            if (this.fpPrepay_Sheet1.Rows.Count <= 0) return;
            if (this.patientInfo == null)
            {
                return;
            }
            else
            {
                if (this.patientInfo.ID == null || this.patientInfo.ID.Trim() == "") return;
            }

            ArrayList alPrepay = new ArrayList();
            Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
            prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)this.fpPrepay_Sheet1.ActiveRow.Tag;

            if (prepay == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("请选择一条预交金记录", 111);
                return;
            }
            if (!string.IsNullOrEmpty(prepay.RecipeNO))
            {
                for (int i = 0; i < this.fpPrepay_Sheet1.RowCount; i++)
                {
                    Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayTmp = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)this.fpPrepay_Sheet1.Rows[i].Tag;
                    if (prepayTmp != null)
                    {
                        if (!string.IsNullOrEmpty(prepayTmp.RecipeNO) && prepayTmp.RecipeNO.ToString() == prepay.RecipeNO.ToString())
                        {
                            alPrepay.Add(prepayTmp);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            if (alPrepay == null || alPrepay.Count == 0)
            {
                return;
            }

            bool isCallBack = false;
            bool isReprint = false;
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayRp in alPrepay)
            {
                if (prepayRp.PrepayState == "1" && prepayRp.BalanceState == "1")
                {
                    isCallBack = true;
                }
                if (prepayRp.PrepayState == "0" && prepayRp.BalanceState == "0")
                {
                    isReprint = true;
                }
            }

            foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayRp in alPrepay)
            {
                if (prepayRp.PrepayOper.ID == "00W999")
                {
                    if (Neusoft.FrameWork.Function.NConvert.ToInt32(prepayRp.User03) >= 1)
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经进行过补打操作,不能再补打!", 111);
                        return;
                    }
                }
                else
                {
                    if (Neusoft.FrameWork.Function.NConvert.ToInt32(prepayRp.User03) >= 2)
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经进行过补打操作,不能再补打!", 111);
                        return;
                    }
                }

                if (isCallBack && isReprint)
                {
                    patientInfo.User01 = "RePrint";
                }
                else
                {
                    if (prepayRp.PrepayState == "1")
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经作废!不能进行补打操作!", 111);
                        return;
                    }

                    if (prepayRp.BalanceState == "1")
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该票据已经结算过不能补打!!", 111);
                        return;
                    }
                }

                if (prepayRp.PrepayState == "2")
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经进行过补打操作,已经成为作废发票,不能再补打!", 111);
                    return;
                }

                if (prepayRp.TransferPrepayState == "1")
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金为结算的转押金还没有进行正常打印操作,不能补打!", 111);
                    return;
                }
            }

            DialogResult r = Neusoft.FrameWork.WinForms.Classes.Function.Msg("是否补打发票号为" + prepay.RecipeNO + "的预交金?", 422);
            if (r == DialogResult.No) return;
            //判断封帐
            if ((this.feeInpatient.GetStopAccount(this.patientInfo.ID)) == "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者处于封帐状态,可能正在结算,请稍后再做此操作!", 111);
                return;
            }

            //更新住院押金明细表备注[2016-03-30 肖锡燕]
            try
            {
                this.feeInpatient.UpdateInPrepayRemark(prepay.RecipeNO);
                this.feeInpatient.UpdateInPrepayRePrint(prepay.RecipeNO, this.patientInfo.ID, feeInpatient.Operator.ID);
            }
            catch (Exception ex)
            { }

            //打印预交金发票
            this.PrintPrepayInvoice(this.patientInfo, alPrepay, false);

            //完

            Neusoft.FrameWork.WinForms.Classes.Function.Msg("补打完毕！", 111);

            //重新检索预交金记录
            this.QueryPatientPrepay(this.patientInfo);
        }

        /// <summary>
        /// 补打退费签名单
        /// </summary>
        protected void RePrintPrepayPatientSign()
        {
            if (this.fpPrepay_Sheet1.ActiveRowIndex < 0) return;
            if (this.fpPrepay_Sheet1.Rows.Count <= 0) return;
            if (this.patientInfo == null)
            {
                return;
            }
            else
            {
                if (this.patientInfo.ID == null || this.patientInfo.ID.Trim() == "") return;
            }

            Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
            prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)this.fpPrepay_Sheet1.ActiveRow.Tag;

            if (prepay == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("请选择一条预交金记录", 111);
                return;
            }

            if (prepay.PrepayState != "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金还没有返回!不能进行退费补打签名单操作!", 111);
                return;
            }

            if (prepay.FT.PrepayCost > 0)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("退费补打签名操作的金额必须为负数!", 111);
                return;
            }

            DialogResult r = Neusoft.FrameWork.WinForms.Classes.Function.Msg("是否补打发票号为" + prepay.RecipeNO + "的预交金退费签名单?", 422);
            if (r == DialogResult.No) return;
            //判断封帐
            if ((this.feeInpatient.GetStopAccount(this.patientInfo.ID)) == "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者处于封帐状态,可能正在结算,请稍后再做此操作!", 111);
                return;
            }

            if (this.isPrintPatientSign)
            {
                this.PrintPrepayPatientSign(this.patientInfo, prepay);
            }

            Neusoft.FrameWork.WinForms.Classes.Function.Msg("补打完毕！", 111);

            //重新检索预交金记录
            this.QueryPatientPrepay(this.patientInfo);
        }

        /// <summary>
        /// 获取下一打印发票号
        /// {4914954F-6464-41e9-AFCB-4F0ABFD626AE}
        /// </summary>
        protected void GetNextInvoiceNO()
        {
            lblNextInvoiceNO.Text = "";
            Neusoft.HISFC.Models.Base.Employee oper = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;
            string invoiceNO = "";
            string realInvoiceNO = "";
            string errText = "";

            this.feeIntegrate.GetInvoiceNO(oper, "P", ref invoiceNO, ref realInvoiceNO, ref errText);

            if (string.IsNullOrEmpty(invoiceNO))
            {
                //未领取发票则弹出窗口输入
                Neusoft.HISFC.Components.Common.Forms.frmUpdateInvoice frm = new Neusoft.HISFC.Components.Common.Forms.frmUpdateInvoice();
                frm.InvoiceType = "P";
                frm.ShowDialog(this);

                int iReturn = this.feeIntegrate.GetInvoiceNO(oper, "P", ref invoiceNO, ref realInvoiceNO, ref errText);
                if (iReturn == -1)
                {
                    MessageBox.Show(errText);
                    return;
                }
            }

            lblNextInvoiceNO.Text = "电脑号： " + invoiceNO + ", 印刷号：" + realInvoiceNO;
        }

        /// <summary>
        /// 获取支付方式信息(用于界面提示)
        /// </summary>
        /// <returns></returns>
        private string GetPayModesMsg()
        {
            string ret = string.Empty;
            if (this.cmbPayType.SelectedItem != null && Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtPreCost.Text) > 0)
            {
                ret += this.cmbPayType.SelectedItem.Name + "：" + this.txtPreCost.Text + "\n";
            }
            if (this.cmbTransType1.SelectedItem != null && Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtPreCost1.Text) > 0)
            {
                ret += this.cmbTransType1.SelectedItem.Name + "：" + this.txtPreCost1.Text + "\n";
            }
            if (this.cmbTransType2.SelectedItem != null && Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtPreCost2.Text) > 0)
            {
                ret += this.cmbTransType2.SelectedItem.Name + "：" + this.txtPreCost2.Text + "\n";
            }
            return ret;
        }

        #endregion

        #region "事件"

        /// <summary>
        /// 控件加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucPrepay_Load(object sender, EventArgs e)
        {
            //初始化控件

            this.initControl();
            //重新初始化工具栏
            //try
            //{
            //    Function.RefreshToolBar(this.hsToolBar, ((Neusoft.FrameWork.WinForms.Forms.frmBaseForm)this.ParentForm).toolBar1, "预交金管理");
            //}
            //catch { }

            //设置窗体控件的输入法状态为半角
            Neusoft.HISFC.Components.Common.Classes.Function.SetIme(this);

            this.ucQueryInpatientNo.Focus();
            this.ucQueryInpatientNo.Select();


            #region 扫码墩url初始化
            if (newScanCodeYJJGL)
            {
                Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParams = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                this.scanPaymentUrl = controlParams.GetControlParam<string>("PT0003", false, string.Empty);
                this.scanRefundUrl = controlParams.GetControlParam<string>("PT0004", false, string.Empty);
            }
            #endregion
        }

        void cmbPayType_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.txtPreCost.Focus();
        }

        void cmbTransType1_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.txtPreCost1.Focus();
        }

        void cmbTransType2_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.txtPreCost2.Focus();
        }

        void cmbPayType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                string[] strBanks = this.strBankPayMode.Split('|');
                foreach (string str in strBanks)
                {
                    if (string.IsNullOrEmpty(str)) continue;
                    if (str == this.cmbPayType.Tag.ToString())
                    {
                        this.pnlBankInfo.Visible = true;
                        this.txtPreCost.SelectAll();
                        txtPreCost.Focus();
                        break;
                    }
                }

                this.pnlBankInfo.Visible = false;
                this.txtPreCost.SelectAll();
                txtPreCost.Focus();
            }
        }

        void cmbTransType1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                string[] strBanks = this.strBankPayMode.Split('|');
                foreach (string str in strBanks)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    if (str == this.cmbTransType1.Tag.ToString())
                    {
                        this.pnlBankInfo.Visible = true;
                        this.txtPreCost1.SelectAll();
                        txtPreCost1.Focus();
                        break;
                    }
                }

                this.pnlBankInfo.Visible = false;
                this.txtPreCost1.SelectAll();
                txtPreCost1.Focus();
            }
        }

        void cmbTransType2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                string[] strBanks = this.strBankPayMode.Split('|');
                foreach (string str in strBanks)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    if (str == this.cmbTransType2.Tag.ToString())
                    {
                        this.pnlBankInfo.Visible = true;
                        this.txtPreCost2.SelectAll();
                        txtPreCost2.Focus();
                        break;
                    }
                }

                this.pnlBankInfo.Visible = false;
                this.txtPreCost2.SelectAll();
                txtPreCost2.Focus();
            }
        }

        void cmbPayType_SelectedIndexChanged(object sender, EventArgs e)
        {

            string[] strBanks = this.strBankPayMode.Split('|');
            foreach (string str in strBanks)
            {
                if (string.IsNullOrEmpty(str)) continue;
                if (str == this.cmbPayType.Tag.ToString())
                {
                    this.pnlBankInfo.Visible = true;
                    this.txtPreCost.SelectAll();
                    txtPreCost.Focus();
                    return;
                }
            }
            this.pnlBankInfo.Visible = false;
            this.txtPreCost.SelectAll();
            txtPreCost.Focus();
        }

        void cmbTransType1_SelectedIndexChanged(object sender, EventArgs e)
        {

            string[] strBanks = this.strBankPayMode.Split('|');
            foreach (string str in strBanks)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                if (str == this.cmbTransType1.Tag.ToString())
                {
                    this.pnlBankInfo.Visible = true;
                    this.txtPreCost1.SelectAll();
                    txtPreCost1.Focus();
                    return;
                }
            }
            this.pnlBankInfo.Visible = false;
            this.txtPreCost1.SelectAll();
            txtPreCost1.Focus();
        }

        void cmbTransType2_SelectedIndexChanged(object sender, EventArgs e)
        {

            string[] strBanks = this.strBankPayMode.Split('|');
            foreach (string str in strBanks)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                if (str == this.cmbTransType2.Tag.ToString())
                {
                    this.pnlBankInfo.Visible = true;
                    this.txtPreCost2.SelectAll();
                    txtPreCost2.Focus();
                    return;
                }
            }
            this.pnlBankInfo.Visible = false;
            this.txtPreCost2.SelectAll();
            txtPreCost2.Focus();
        }

        void cmbBank_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                this.txtBankAccount.SelectAll();
                txtBankAccount.Focus();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                if (this.cmbBank.SelectedIndex < this.cmbBank.Items.Count - 1)
                    this.cmbBank.SelectedIndex++;
                else
                    this.cmbBank.SelectedIndex = 0;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                if (this.cmbBank.SelectedIndex > 0)
                    this.cmbBank.SelectedIndex--;
                else
                {
                    this.cmbBank.SelectedIndex = this.cmbBank.Items.Count - 1;
                }
            }
        }
        void cmbBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtBankAccount.SelectAll();
            txtBankAccount.Focus();
        }

        void txtPreCost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                string[] strBanks = this.strBankPayMode.Split('|');
                foreach (string str in strBanks)
                {
                    if (string.IsNullOrEmpty(str)) continue;
                    if (str == this.cmbPayType.Tag.ToString())
                    {
                        this.cmbBank.SelectAll();
                        cmbBank.Focus();
                        return;
                    }
                }
                this.cmbTransType1.SelectAll();
                this.cmbTransType1.Focus();
            }
        }

        void txtPreCost1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                string[] strBanks = this.strBankPayMode.Split('|');
                foreach (string str in strBanks)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    if (str == this.cmbTransType1.Tag.ToString())
                    {
                        this.cmbBank.SelectAll();
                        cmbBank.Focus();
                        return;
                    }
                }
                this.cmbTransType2.SelectAll();
                this.cmbTransType2.Focus();
            }
        }

        void txtPreCost2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                string[] strBanks = this.strBankPayMode.Split('|');
                foreach (string str in strBanks)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    if (str == this.cmbTransType2.Tag.ToString())
                    {
                        this.cmbBank.SelectAll();
                        cmbBank.Focus();
                        return;
                    }
                }

                this.txtMark.SelectAll();
                this.txtMark.Focus();

            }

        }

        void txtMark_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                string msg = this.GetPayModesMsg();
                if (string.IsNullOrEmpty(msg) == false)
                {
                    DialogResult dr = MessageBox.Show(msg + "确认收取?", "提示", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.OK)
                    {
                        this.ReceivePrepay();
                    }
                    else
                    {
                        this.cmbPayType.Focus();
                    }
                }
                else
                {
                    this.ReceivePrepay();
                }
            }
        }

        private void ucQueryInpatientNo_myEvent()
        {
            //清空
            this.Clear();
            this.fpPrepay_Sheet1.RowCount = 0;

            //判断是否有该患者
            if (this.ucQueryInpatientNo.InpatientNo == null || this.ucQueryInpatientNo.InpatientNo.Trim() == "")
            {
                if (string.IsNullOrEmpty(ucQueryInpatientNo.Err))
                {
                    ucQueryInpatientNo.Err = "此患者不在院!";
                }
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.ucQueryInpatientNo.Err, 211);

                this.ucQueryInpatientNo.Focus();
                return;
            }
            //获取住院号赋值给实体
            this.patientInfo = this.radtIntegrate.GetPatientInfomation(this.ucQueryInpatientNo.InpatientNo);

            if (this.patientInfo == null) MessageBox.Show(this.radtIntegrate.Err);



            if ((Neusoft.HISFC.Models.Base.EnumInState)Enum.Parse(typeof(Neusoft.HISFC.Models.Base.EnumInState), this.patientInfo.PVisit.InState.ID.ToString()) == Neusoft.HISFC.Models.Base.EnumInState.N
                || (Neusoft.HISFC.Models.Base.EnumInState)Enum.Parse(typeof(Neusoft.HISFC.Models.Base.EnumInState), this.patientInfo.PVisit.InState.ID.ToString()) == Neusoft.HISFC.Models.Base.EnumInState.O)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者已经出院!", 111);

                this.patientInfo.ID = null;

                return;
            }

            //控件赋值患者信息



            this.EvaluteByPatientInfo(this.patientInfo);



            //读取控制类参数

            if (this.ReadControlInfo() == -1)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("提取控制信息出错!", 211);
                this.Clear();
                return;
            }

            //判断未打印的转押金



            ArrayList alForegift = new ArrayList();
            //判断是否存在未打印转押金
            alForegift = this.feeInpatient.QueryForegif(this.patientInfo.ID);
            if (alForegift == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err, 211);
                this.Clear();
                return;
            }
            //{64BD57CE-9361-41f6-AE91-2618CBA5047A}
            ArrayList alCallBacePrepay = feeInpatient.QueryCallBackPrePay(this.patientInfo.ID);
            if (alCallBacePrepay == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err, 211);
                this.Clear();
                return;
            }

            if (!IsPrintReturn)
            {
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay p in alCallBacePrepay)
                {
                    if (p.PrepayState != "0")
                    {
                        alCallBacePrepay.Remove(p);
                    }
                }
            }

            int count = alCallBacePrepay.Count + alForegift.Count;

            if (count > 0)
            {
                //{64BD57CE-9361-41f6-AE91-2618CBA5047A}
                DialogResult r = MessageBox.Show("患者有" + count.ToString() + "笔预交金没有打印,是否打印?", "提示", MessageBoxButtons.YesNo);
                if (r == DialogResult.Yes)
                {

                    string errText = string.Empty;
                    Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                    this.feeInpatient.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                    foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay in alForegift)
                    {
                        //提取发票号码
                        //发票类型-预交金
                        //string InvoiceNo = "";
                        //InvoiceNo = this.feeIntegrate.GetNewInvoiceNO("P");

                        //if (InvoiceNo == null || InvoiceNo == "")
                        //{
                        //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //    Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err,211);
                        //    return;
                        //}
                        ////					
                        //prepay.RecipeNO = InvoiceNo;
                        //prepay.PrepayOper.ID = this.feeInpatient.Operator.ID;
                        //prepay.PrepayOper.Name = this.feeInpatient.Operator.Name;

                        ////打印转押金发票
                        //this.PrintPrepayInvoice(this.patientInfo, prepay, false);

                        if (PrintForgift(prepay, ref errText) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg(errText, 211);
                            return;
                        }
                        //更新转押金发票号码和状态
                        if (feeInpatient.UpdateForgift(this.patientInfo, prepay) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err, 211);
                            return;
                        }

                    }

                    foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay in alCallBacePrepay)
                    {
                        if (PrintForgift(prepay, ref errText) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg(errText, 211);
                            return;
                        }

                        if (feeInpatient.UpdateCallBackPrePay(patientInfo, prepay) <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err, 211);
                            return;
                        }
                    }

                    Neusoft.FrameWork.Management.PublicTrans.Commit();


                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("发票打印完毕!", 111);
                }

            }

            if (this.QueryPatientPrepay(this.patientInfo) == -1)
            {
                this.Clear();
                this.fpPrepay_Sheet1.Rows.Count = 0;
                return;
            }
            this.cmbPayType.Focus();
        }

        protected override int OnQuery(object sender, object neuObject)
        {
            this.ucQueryInpatientNo_myEvent();
            return base.OnQuery(sender, neuObject);
        }

        /// <summary>
        /// 打印转押金和召回发票
        /// </summary>
        /// <param name="prepay"></param>
        /// <param name="errText"></param>
        private int PrintForgift(Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay, ref string errText)
        {
            string InvoiceNo = "";
            InvoiceNo = this.feeIntegrate.GetNewInvoiceNO("P");

            if (InvoiceNo == null || InvoiceNo == "")
            {
                errText = this.feeInpatient.Err;
                return -1;
            }
            //					
            prepay.RecipeNO = InvoiceNo;
            prepay.PrepayOper.ID = this.feeInpatient.Operator.ID;
            prepay.PrepayOper.Name = this.feeInpatient.Operator.Name;

            //打印转押金发票
            this.PrintPrepayInvoice(this.patientInfo, prepay, false);
            return 1;
        }
        #endregion

        #region 快捷键


        /// <summary>
        /// toolBar映射
        /// </summary>
        protected Hashtable hsToolBar = new Hashtable();

        /// <summary>
        /// 按键设置
        /// </summary>
        /// <param name="keyData">当前按键</param>
        /// <returns>继续执行True False 当前处理结束</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 执行快捷键

        /// </summary>
        /// <param name="key">当前按键</param>
        private bool ExecuteShotCut(Keys key)
        {
            string opName = Function.GetOperationName("预交金管理", key.GetHashCode().ToString());

            if (opName == "") return false;

            ButtonClicked(opName);

            return true;

        }

        #endregion

        /// <summary>
        /// 单击时全选

        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPreCost_Click(object sender, EventArgs e)
        {
            this.txtPreCost.SelectAll();
            txtPreCost.Focus();
        }


        #region 树操作
        /// <summary>
        /// 接收树选择的患者基本信息
        /// </summary>
        /// <param name="neuObject">患者基本信息实体</param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override int OnSetValue(object neuObject, TreeNode e)
        {
            this.patientInfo = neuObject as Neusoft.HISFC.Models.RADT.PatientInfo;

            if (patientInfo == null || patientInfo.ID == null || patientInfo.ID == "")
            {
                return -1;
            }

            QueryInpatientNo(this.patientInfo.ID);
            return 0;
        }

        private void QueryInpatientNo(string inpatientno)
        {
            //清空
            this.Clear();
            this.fpPrepay_Sheet1.RowCount = 0;

            //判断是否有该患者
            if (inpatientno == null || inpatientno == "")
            {
                if (this.ucQueryInpatientNo.Err == "")
                {
                    ucQueryInpatientNo.Err = "此患者不在院!";
                }
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.ucQueryInpatientNo.Err, 211);

                this.ucQueryInpatientNo.Focus();
                return;
            }
            //获取住院号赋值给实体
            this.patientInfo = this.radtIntegrate.GetPatientInfomation(inpatientno);

            if (this.patientInfo == null) MessageBox.Show(this.radtIntegrate.Err);



            //if ((Neusoft.HISFC.Models.Base.EnumInState)this.patientInfo.PVisit.InState.ID == Neusoft.HISFC.Models.Base.EnumInState.N
            //    || (Neusoft.HISFC.Models.Base.EnumInState)this.patientInfo.PVisit.InState.ID == Neusoft.HISFC.Models.Base.EnumInState.O)
            if (this.patientInfo.PVisit.InState.ID.ToString() == Neusoft.HISFC.Models.Base.EnumInState.N.ToString() || this.patientInfo.PVisit.InState.ID.ToString() == Neusoft.HISFC.Models.Base.EnumInState.O.ToString())
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者已经出院!", 111);

                this.patientInfo.ID = null;

                return;
            }

            //控件赋值患者信息



            this.EvaluteByPatientInfo(this.patientInfo);



            //读取控制类参数

            if (this.ReadControlInfo() == -1)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("提取控制信息出错!", 211);
                this.Clear();
                return;
            }

            //判断未打印的转押金



            ArrayList alForegift = new ArrayList();
            //判断是否存在未打印转押金
            alForegift = this.feeInpatient.QueryForegif(this.patientInfo.ID);
            if (alForegift == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err, 211);
                this.Clear();
                return;
            }
            //{64BD57CE-9361-41f6-AE91-2618CBA5047A}
            ArrayList alCallBacePrepay = feeInpatient.QueryCallBackPrePay(this.patientInfo.ID);
            if (alCallBacePrepay == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err, 211);
                this.Clear();
                return;
            }

            if (!IsPrintReturn)
            {
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay p in alCallBacePrepay)
                {
                    if (p.PrepayState != "0")
                    {
                        alCallBacePrepay.Remove(p);
                    }
                }
            }

            int count = alCallBacePrepay.Count + alForegift.Count;

            if (count > 0)
            {
                //{64BD57CE-9361-41f6-AE91-2618CBA5047A}
                DialogResult r = MessageBox.Show("患者有" + count.ToString() + "笔预交金没有打印,是否打印?", "提示", MessageBoxButtons.YesNo);
                if (r == DialogResult.Yes)
                {

                    string errText = string.Empty;
                    Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                    this.feeInpatient.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                    foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay in alForegift)
                    {
                        //提取发票号码
                        //发票类型-预交金
                        //string InvoiceNo = "";
                        //InvoiceNo = this.feeIntegrate.GetNewInvoiceNO("P");

                        //if (InvoiceNo == null || InvoiceNo == "")
                        //{
                        //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //    Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err,211);
                        //    return;
                        //}
                        ////					
                        //prepay.RecipeNO = InvoiceNo;
                        //prepay.PrepayOper.ID = this.feeInpatient.Operator.ID;
                        //prepay.PrepayOper.Name = this.feeInpatient.Operator.Name;

                        ////打印转押金发票
                        //this.PrintPrepayInvoice(this.patientInfo, prepay, false);

                        if (PrintForgift(prepay, ref errText) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg(errText, 211);
                            return;
                        }
                        //更新转押金发票号码和状态
                        if (feeInpatient.UpdateForgift(this.patientInfo, prepay) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err, 211);
                            return;
                        }

                    }

                    foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay in alCallBacePrepay)
                    {
                        if (PrintForgift(prepay, ref errText) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg(errText, 211);
                            return;
                        }

                        if (feeInpatient.UpdateCallBackPrePay(patientInfo, prepay) <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Neusoft.FrameWork.WinForms.Classes.Function.Msg(this.feeInpatient.Err, 211);
                            return;
                        }
                    }

                    Neusoft.FrameWork.Management.PublicTrans.Commit();


                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("发票打印完毕!", 111);
                }

            }

            if (this.QueryPatientPrepay(this.patientInfo) == -1)
            {
                this.Clear();
                this.fpPrepay_Sheet1.Rows.Count = 0;
                return;
            }
            this.cmbPayType.Focus();
        }

        #endregion

        private void gbPrepayInfo_Enter(object sender, EventArgs e)
        {
            //GetNextInvoiceNO();
        }

        private void pnlUp_Paint(object sender, PaintEventArgs e)
        {

        }

        public void GetPatientNoByDZPZ()
        {
            string PatientNo = string.Empty;
            string IdNo = string.Empty;
            GDSI.Models.DecodeModel dm = new GDSI.Models.DecodeModel();
            GDSI.Models.DecodeModel.DecodeDetailModel ddm = new GDSI.Models.DecodeModel.DecodeDetailModel();
            string Dept_Code = lm.GetMedicalDeptCode(lm.Operator.ID);//根据当前操作人工号获取所在科室
            string DeptCode_GD = lm.GetCatyForDeptCode(Dept_Code);//获取医保科室编码
            string DeptName_GD = lm.GetCatyNameForDeptCode(DeptCode_GD);//获取医保科室名称
            if (string.IsNullOrEmpty(DeptCode_GD))
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("未找到科室编码[" + Dept_Code + "]对应的医保科室代码！", 111);
                return;
            }
            else if (string.IsNullOrEmpty(DeptName_GD))
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("未找到科室编码[" + DeptCode_GD + "]对应的医保科室名称！", 111);
                return;
            }
            ddm.operatorId = lm.Operator.ID;
            ddm.operatorName = lm.Operator.Name;
            ddm.officeId = DeptCode_GD;
            ddm.officeName = DeptName_GD;
            ddm.businessType = "01104";
            ddm.deviceType = "";
            ddm.orgId = "H44040200001";
            dm.orgId = "H44040200001";
            dm.transType = "ec.query";
            dm.data = ddm;
            string InData = JsonConvert.SerializeObject(dm);
            byte[] OutData = new byte[1024];
            string strUrl = queryDB.GetControlParam("DecodeServiceURL", GDSI.CountryMedical.Model.EnumHospitalType.ZDWY);
            string returnCode = string.Empty;
            JObject jsonObject = new JObject();
            string guid = Guid.NewGuid().ToString();
            log.WriteLog("医保电子凭证解码接口入参：[" + guid + "]" + System.Environment.NewLine + "URL:" + strUrl + System.Environment.NewLine + InData);
            if (string.IsNullOrEmpty(strUrl))
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("解码接口地址未配置！", 111);
                return;
            }

            try
            {
                returnCode = GDSI.Function.NationEcTrans(strUrl, InData, ref OutData[0]);
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(ex.Message, 111);
                return;
            }
            if (returnCode == "0000")
            {
                string outObj = System.Text.Encoding.Default.GetString(OutData, 0, OutData.Length); //将字节数组转换为字符串;
                log.WriteLog("医保电子凭证解码接口出参：[" + guid + "]" + System.Environment.NewLine + "URL:" + strUrl + System.Environment.NewLine + outObj);
                jsonObject = JObject.Parse(outObj);
                string code = (string)jsonObject["code"];
                if (code == "0")
                {
                    IdNo = (string)jsonObject["data"]["idNo"];
                    if (string.IsNullOrEmpty(IdNo))
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("医保电子凭证解码接口返回身份证号为空！", 111);
                        return;
                    }
                    //根据身份证查询在院的住院号返回
                    PatientNo = this.lm.GetInPatientByIdNo(IdNo);
                    if (string.IsNullOrEmpty(PatientNo))
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("根据身份证号【" + IdNo + "】查询不到该患者的在院信息！", 111);
                        return;
                    }
                    QueryInpatientNo(PatientNo);

                }
                else
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("医保电子凭证解码接口返回错误编码[" + (string)jsonObject["code"] + "]，错误信息：" + (string)jsonObject["message"] + "！", 111);
                    return;
                }
            }
            else
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("医保电子凭证解码接口返回错误编码[" + (string)jsonObject["code"] + "]，错误信息：" + (string)jsonObject["message"] + "！", 111);
                return;
            }
        }

        /// <summary>
        /// 换开发票，直接查询预交金信息打印，不走号
        /// </summary>
        protected virtual void ReplaceQueryAndPrintPrepay()
        {
            if (this.fpPrepay_Sheet1.ActiveRowIndex < 0) return;
            if (this.fpPrepay_Sheet1.Rows.Count <= 0) return;
            if (this.patientInfo == null)
            {
                return;
            }
            else
            {
                if (this.patientInfo.ID == null || this.patientInfo.ID.Trim() == "") return;
            }

            ArrayList alPrepay = new ArrayList();
            Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
            prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)this.fpPrepay_Sheet1.ActiveRow.Tag;

            if (prepay == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("请选择一条预交金记录", 111);
                return;
            }
            if (!string.IsNullOrEmpty(prepay.RecipeNO))
            {
                for (int i = 0; i < this.fpPrepay_Sheet1.RowCount; i++)
                {
                    Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayTmp = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)this.fpPrepay_Sheet1.Rows[i].Tag;
                    if (prepayTmp != null)
                    {
                        if (!string.IsNullOrEmpty(prepayTmp.RecipeNO) && prepayTmp.RecipeNO.ToString() == prepay.RecipeNO.ToString())
                        {
                            alPrepay.Add(prepayTmp);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            if (alPrepay == null || alPrepay.Count == 0)
            {
                return;
            }

            bool isCallBack = false;
            bool isReprint = false;
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayRp in alPrepay)
            {
                if (prepayRp.PrepayState == "1" && prepayRp.BalanceState == "1")
                {
                    isCallBack = true;
                }
                if (prepayRp.PrepayState == "0" && prepayRp.BalanceState == "0")
                {
                    isReprint = true;
                }
            }

            foreach (Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepayRp in alPrepay)
            {
                //根据住院流水号和预交金票据号查询出换开的次数
                string hkNum = this.feeInpatient.GetHKNum(this.patientInfo.ID, prepay.RecipeNO);
                if (string.IsNullOrEmpty(hkNum))
                {
                    hkNum = "0";
                }
                if (prepayRp.PrepayOper.ID == "00W999")
                {
                    if (Neusoft.FrameWork.Function.NConvert.ToInt32(hkNum) >= 1)
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经进行过换开操作,不能再换开!", 111);
                        return;
                    }
                }
                else
                {
                    if (Neusoft.FrameWork.Function.NConvert.ToInt32(hkNum) >= 2)
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经进行过换开操作,不能再换开!", 111);
                        return;
                    }
                }

                if (isCallBack && isReprint)
                {
                    patientInfo.User01 = "RePrint";
                }
                else
                {
                    if (prepayRp.PrepayState == "1")
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经作废!不能进行换开操作!", 111);
                        return;
                    }

                    if (prepayRp.BalanceState == "1")
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该票据已经结算过不能换开!!", 111);
                        return;
                    }
                }

                if (prepayRp.PrepayState == "4")
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金已经进行过换开操作,已经成为作废发票,不能再换开!", 111);
                    return;
                }

                if (prepayRp.TransferPrepayState == "1")
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.Msg("该预交金为结算的转押金还没有进行正常打印操作,不能换开!", 111);
                    return;
                }
            }

            DialogResult r = Neusoft.FrameWork.WinForms.Classes.Function.Msg("是否换开发票号为" + prepay.RecipeNO + "的预交金?", 422);
            if (r == DialogResult.No) return;
            //判断封帐
            if ((this.feeInpatient.GetStopAccount(this.patientInfo.ID)) == "1")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者处于封帐状态,可能正在结算,请稍后再做此操作!", 111);
                return;
            }

            //更新住院押金明细表备注[2016-03-30 肖锡燕]
            try
            {
                this.feeInpatient.UpdateInPrepayRemarkNew(prepay.RecipeNO);
                this.feeInpatient.UpdateInPrepayRePrintNew(prepay.RecipeNO, this.patientInfo.ID, feeInpatient.Operator.ID);
            }
            catch (Exception ex)
            { }

            //换开预交金发票
            this.ReplacePrintPrepayInvoice(this.patientInfo, alPrepay, false);

            //完

            Neusoft.FrameWork.WinForms.Classes.Function.Msg("换开完毕！", 111);

            //重新检索预交金记录
            this.QueryPatientPrepay(this.patientInfo);
        }


    }
}
