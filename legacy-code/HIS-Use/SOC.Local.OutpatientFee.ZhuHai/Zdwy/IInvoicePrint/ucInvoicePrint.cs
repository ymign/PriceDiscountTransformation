using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IInvoicePrint
{
    public partial class ucInvoicePrint : System.Windows.Forms.UserControl, Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {
        public ucInvoicePrint()
        {
            InitializeComponent();
        }

        #region 变量

        private string description = "中山大学附属第六医院";

        private string invoiceType = "MZ01";

        private bool isPreView = false;

        private Neusoft.HISFC.Models.Registration.Register register;

        private string setPayModeType;

        private string splitInvoicePayMode;

        private Neusoft.FrameWork.Management.Transaction trans = new Neusoft.FrameWork.Management.Transaction();

        public string mzCost = string.Empty;

        Neusoft.FrameWork.Public.ObjectHelper payModesHelper = new Neusoft.FrameWork.Public.ObjectHelper();

        /// <summary>
        /// 控制参数
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

        /// <summary>
        /// 常数业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Constant constantMgr = new Neusoft.HISFC.BizLogic.Manager.Constant();
        /// <summary>
        /// 门诊费用业务类
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        #endregion

        #region 方法

        /// <summary>
        /// 获得费用名称输入框
        /// </summary>
        /// <param name="i">序号</param>
        /// <returns></returns>
        private Control GetFeeNameLable(int i)
        {
            Control c = this.Controls[string.Concat("lblPreFeeName", i.ToString())];
            if (c != null)
            {
                c.Visible = true;
            }

            return c;
        }

        /// <summary>
        /// 获得费用金额输入框
        /// </summary>
        /// <param name="i">序号</param>
        /// <returns></returns>
        private Control GetFeeCostLable(int i)
        {
            Control c = this.Controls[string.Concat("lblPriCost", i.ToString())];
            if (c != null)
            {
                c.Visible = true;
            }

            return c;
        }

        /// <summary>
        /// 获取发票打印大写数字数组(只打印到十万)
        /// </summary>
        /// <param name="Cash"></param>
        /// <returns></returns>
        private string[] GetUpperCashbyNumber(decimal Cash)
        {
            string[] sNumber = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[] sReturn = new string[9];
            string strCash = null;
            //填充位数
            int iLen = 0;
            strCash = Neusoft.FrameWork.Public.String.FormatNumber(Cash, 2).ToString("############.00");
            if (strCash.Length > 9)
            {
                strCash = strCash.Substring(strCash.Length - 9);
            }

            //填充位数
            iLen = 9 - strCash.Length;
            for (int j = 0; j < iLen; j++)
            {
                int k = 0;
                k = 8 - j;
                sReturn[k] = "零";
            }
            for (int i = 0; i < strCash.Length; i++)
            {
                string Temp = null;

                Temp = strCash.Substring(strCash.Length - 1 - i, 1);

                if (Temp == ".")
                {
                    continue;
                }
                sReturn[i] = sNumber[int.Parse(Temp)];
            }
            return sReturn;
        }

        #endregion

        #region IInvoicePrint 成员

        public string Description
        {
            get { return this.description; }
        }

        public string InvoiceType
        {
            get { return this.invoiceType; }
        }

        /// <summary>
        /// 设置是否打印副本 true发票副本 false发票套打
        /// </summary>
        public bool IsPreView
        {
            set { this.isPreView = value; }
        }

        public int Print()
        {
            try
            {
                Neusoft.FrameWork.WinForms.Classes.Print print = new Neusoft.FrameWork.WinForms.Classes.Print();
                Neusoft.HISFC.Models.Base.PageSize ps = new Neusoft.HISFC.Models.Base.PageSize("MZFP", 883, 480);
                print.SetPageSize(ps);
                if (isPreView)
                {
                    //发票副本
                    print.PrintDocument.PrinterSettings.PrinterName = this.controlIntegrate.GetControlParam<string>("MZFPFB", false, "MZFPFB");
                }
                else
                {
                    //发票套打
                    print.PrintDocument.PrinterSettings.PrinterName = this.controlIntegrate.GetControlParam<string>("MZFP", false, "MZFP");
                }

                print.ControlBorder = Neusoft.FrameWork.WinForms.Classes.enuControlBorder.None;
                print.IsCanCancel = false;
                print.PrintPage(0, 0, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
            return 1;
        }

        public int PrintOtherInfomation()
        {
            return 1;
        }

        public Neusoft.HISFC.Models.Registration.Register Register
        {
            set { this.register = value; }
        }

        /// <summary>
        /// 设置支付方式模式 1使用SplitInvoicePayMode 否则使用SetPrintValue中的发票参数
        /// </summary>
        public string SetPayModeType
        {
            set { this.setPayModeType = value; }
        }

        /// <summary>
        /// 发票支付方式
        /// </summary>
        public string SplitInvoicePayMode
        {
            set { this.splitInvoicePayMode = value; }
        }

        public void SetPreView(bool isPreView)
        {
            this.isPreView = isPreView;
        }

        public int SetPrintOtherInfomation(Neusoft.HISFC.Models.Registration.Register regInfo, ArrayList Invoices, ArrayList invoiceDetails, ArrayList feeDetails)
        {
            return 1;
        }

        public int SetPrintValue(Neusoft.HISFC.Models.Registration.Register regInfo, Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice, ArrayList invoiceDetails, ArrayList feeDetails, bool isPreview)
        {
            return 1;
        }

        public int SetPrintValue(Neusoft.HISFC.Models.Registration.Register regInfo, Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice, ArrayList invoiceDetails, ArrayList feeDetails, ArrayList alPayModes, bool isPreview)
        {
            this.isPreView = isPreview;

            if (feeDetails.Count <= 0)
            {
                return -1;
            }
            //设置控件显示
            foreach (Control c in this.Controls)
            {
                if (c.Name.Length > 6 && "lblPre".Equals(c.Name.Substring(0, 6)))
                {
                    c.Visible = isPreview;
                }

                if (isPreview == false)
                {
                    if (c.Name.Length > 3 && "lbl".Equals(c.Name.Substring(0, 3)))
                    {
                        System.Windows.Forms.Label lblControl = c as System.Windows.Forms.Label;
                        lblControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
                    }
                }
            }

            //设置基本信息            
            this.lblCardNo.Text = regInfo.PID.CardNO;
            this.lblPriSwYear.Text = invoice.PrintTime.Year.ToString();
            this.lblPriSwMonth.Text = invoice.PrintTime.Month.ToString();
            this.lblPriSwDay.Text = invoice.PrintTime.Day.ToString();
            string medicaltype = "";
            string partjlx = "0";
            string Taxid = ""; //税号
            GetPartjlx(regInfo.PID.CardNO, ref partjlx, ref Taxid);
            if (regInfo.IDCard != null)
            {
                if (regInfo.IDCard == "")
                {
                    this.lblIdenNo.Text = "";
                }
                else
                {
                    if (regInfo.IDCard.Length > 8)
                    {
                        this.lblIdenNo.Text = regInfo.IDCard.Substring(0, 4) + "********" + regInfo.IDCard.Substring(regInfo.IDCard.Length - 5, 4);
                    }
                    else
                    { 
                        this.lblIdenNo.Text = ""; 
                    }
                }
            }
            if (partjlx == "1")
            {
                this.lblIdenNo.Text = Taxid;
            }
            string strOperTemp = Neusoft.SOC.HISFC.BizProcess.CommonInterface.DefaultCommonController.CreateInstance().GetEmployee(invoice.BalanceOper.ID).Name.ToString();
            if (!string.IsNullOrEmpty(strOperTemp))
            {
                strOperTemp = "(" + strOperTemp + ")";
            }
            this.lblPriOper.Text = invoice.BalanceOper.ID + strOperTemp;
            this.lblAssessor.Text = "王永权";//医务科要求，门诊复核人为关则战，催得比较急，临时写死算了
            //this.lblInvoice.Text = invoice.Invoice.ID;
            this.lblInvoice.Text = "440601" + invoice.PrintTime.Year.ToString().Substring(2,2);
            this.lblFeeDate.Text = invoice.PrintTime.ToString("HH:mm:ss");
            this.lblPriBusinessID.Text = invoice.Patient.ID + invoice.Invoice.ID;
            this.lblPriBeginDate.Text = regInfo.DoctorInfo.SeeDate.ToString("yyyy年MM月dd日");
            //if (isPreview == true)
            //{
            //    this.lblPriSwYear.Text = regInfo.DoctorInfo.SeeDate.Year.ToString();
            //    this.lblPriSwMonth.Text = regInfo.DoctorInfo.SeeDate.Month.ToString();
            //    this.lblPriSwDay.Text = regInfo.DoctorInfo.SeeDate.Day.ToString();
            //    this.lblFeeDate.Text = "";
            //}
            this.lblPriSwBalanceType.Text = invoice.PrintedInvoiceNO;
            this.lblPriSwBalanceType2.Text = invoice.PrintedInvoiceNO;
            this.lblPriName.Text = regInfo.Name;
            this.lblPriPactUnit.Text = regInfo.Pact.Name;
            this.lblPriRegNo.Text = regInfo.SIMainInfo.RegNo;
            string RegNo = "";
            GetRegNo(invoice.Patient.ID, ref RegNo);
            this.lblPriRegNo.Text = RegNo;
            if (regInfo.Sex.Name.ToString() != "" && regInfo.Sex.Name.ToString() != null && regInfo.Sex.Name.ToString() == "男" && isPreview == false)
            {
                this.lblPriSex.Text = "性别：男";

            }
            else if (regInfo.Sex.Name.ToString() != "" && regInfo.Sex.Name.ToString() != null && regInfo.Sex.Name.ToString() == "女" && isPreview == false)
            {
                this.lblPriSex.Text = "性别：女";

            }


            //急诊√判断
            //if(regInfo.DoctorInfo.Templet.Dept.Name.Contains("急诊"))
            //{
            //    this.lblIsJZ.Visible = true;
            //    this.neuLabel4.Visible = false;
            //}

            this.lblPriPayKind.Text = regInfo.Pact.Name;
            if (regInfo.SSN != "")
            {
                this.lbPactName.Text += "," + "医疗证号：" + regInfo.SSN;
            }
            //this.neuLabel5.Visible = true;
            //this.neuLabel5.Text = "三级综合";
            ////费用大类名称
            this.lblPreFeeName1.Text = "西药费";
            this.lblPreFeeName1.Visible = true;
            this.lblPreFeeName2.Text = "中成药";
            this.lblPreFeeName2.Visible = true;
            this.lblPreFeeName3.Text = "中草药";
            this.lblPreFeeName3.Visible = true;
            this.lblPreFeeName4.Text = "诊查费";
            this.lblPreFeeName4.Visible = true;
            this.lblPreFeeName5.Text = "检查费";
            this.lblPreFeeName5.Visible = true;
            this.lblPreFeeName6.Text = "化验费";
            this.lblPreFeeName6.Visible = true;
            this.lblPreFeeName7.Text = "治疗费";
            this.lblPreFeeName7.Visible = true;
            this.lblPreFeeName8.Text = "手术费";
            this.lblPreFeeName8.Visible = true;
            //this.lblPreFeeName9.Text = "护理费";
            //this.lblPreFeeName9.Visible = true;
            this.lblPreFeeName9.Text = "材料费";
            this.lblPreFeeName9.Visible = true;
            this.lblPreFeeName10.Text = "其他费";
            this.lblPreFeeName10.Visible = true;
            //this.lblPreFeeName11.Text = "材料费";
            //this.lblPreFeeName11.Visible = true;
            this.lblPreFeeName11.Text = "护理费";
            this.lblPreFeeName11.Visible = true;
            this.lblPreFeeName12.Text = "床位费";
            this.lblPreFeeName12.Visible = true;
            //支付方式
            if (payModesHelper.ArrayObject == null || payModesHelper.ArrayObject.Count == 0)
            {
                payModesHelper.ArrayObject = constantMgr.GetList(Neusoft.HISFC.Models.Base.EnumConstant.PAYMODES);
            }
            string payKind = "";
            decimal MCZHFee = 0;
            if ("1".Equals(this.setPayModeType))
            {
                payKind = this.splitInvoicePayMode;
            }
            else
            {
                for (int i = 0; i < alPayModes.Count; i++)
                {
                    Neusoft.HISFC.Models.Fee.Outpatient.BalancePay payMode = alPayModes[i] as Neusoft.HISFC.Models.Fee.Outpatient.BalancePay;
                    payKind += " " + GetGetPayMode(payMode.PayType.ID)
                        + " " + Neusoft.FrameWork.Public.String.FormatNumber(payMode.FT.TotCost, 2) + "；  ";
                    //Neusoft.HISFC.Models.Fee.Outpatient.BalancePay payMode = alPayModes[i] as Neusoft.HISFC.Models.Fee.Outpatient.BalancePay;
                    //payKind += " " + payModesHelper.GetName(payMode.PayType.ID)
                    //    + " " + Neusoft.FrameWork.Public.String.FormatNumber(payMode.FT.TotCost, 2) + "；  ";
                    if (payMode.PayType.ID == "MCZH")
                    {
                        MCZHFee += payMode.FT.TotCost;
                    }
                }
            }

            if (regInfo.Pact.ID == "248" && regInfo.SIMainInfo.Bka841 > 0)//大爱无疆商保报销金额-只针对门特
            {
                this.lblbka841.Text = "大爱无疆报销金额:" + regInfo.SIMainInfo.Bka841 + "元";
            }
            else
            {
                this.lblbka841.Text = "";
            }

            lbPactName.Font = new Font("宋体", 8);
            lbPactName.Text = payKind;

            string strTemp = "";
            bool boolTemp = false;
            if (regInfo.Pact.PayKind.ID != "01")
            {
                if (regInfo.SIMainInfo.YearCost > 0)
                {
                    this.lbYearCost.Text = "剩余额度" + regInfo.SIMainInfo.YearCost.ToString();
                    boolTemp = true;
                }
                if (regInfo.SIMainInfo.AddTotCost > 0)
                {
                    this.lblAddTotCost.Text = "累计使用" + regInfo.SIMainInfo.AddTotCost.ToString();
                    boolTemp = true;
                }
            }
            this.lbYearCost.Visible = boolTemp;
            this.lblAddTotCost.Visible = boolTemp;
            //this.lbYearCost.Text = strTemp;


            //费用大类信息
            for (int i = 0; i < invoiceDetails.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Outpatient.BalanceList detail = invoiceDetails[i] as Neusoft.HISFC.Models.Fee.Outpatient.BalanceList;
                if (detail.InvoiceSquence < 1 || detail.InvoiceSquence > 24)
                {
                    continue;
                }

                ////费用大类名称
                //System.Windows.Forms.Label lblFeeName = this.GetFeeNameLable(detail.InvoiceSquence) as System.Windows.Forms.Label;
                //if (lblFeeName == null)
                //{
                //    MessageBox.Show("没有找到费用大类为" + detail.FeeCodeStat.Name + "的打印序号!");
                //    return -1;
                //}
                //lblFeeName.Text = detail.FeeCodeStat.Name;
                //lblFeeName.Visible = true;

                //费用大类金额
                System.Windows.Forms.Label lblFeeCost = this.GetFeeCostLable(detail.InvoiceSquence) as System.Windows.Forms.Label;
                if (lblFeeCost == null)
                {
                    MessageBox.Show("没有找到费用大类为" + detail.FeeCodeStat.Name + "的打印序号!");
                    return -1;
                }
                lblFeeCost.Text = detail.BalanceBase.FT.TotCost.ToString();

            }

            #region 2023新费用规则
            DataSet ds = new DataSet();
            if (outpatientManager.GetInvoicesSetlInfoFee(invoice.Patient.ID, invoice.Invoice.ID, regInfo.Pact.ID, ref ds) != -1)
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    decimal MedfeeSumamt = decimal.Parse(ds.Tables[0].Rows[0]["医疗费总额"].ToString());//医疗费总额
                    decimal HifpPay = decimal.Parse(ds.Tables[0].Rows[0]["基本医疗保险统筹基金支出"].ToString());//基本医疗保险统筹基金支出
                    decimal CvlservPay = decimal.Parse(ds.Tables[0].Rows[0]["公务员医疗补助资金支出"].ToString());//公务员医疗补助资金支出
                    decimal HifesPay = decimal.Parse(ds.Tables[0].Rows[0]["企业补充医疗保险基金支出"].ToString());//企业补充医疗保险基金支出
                    decimal HifmiPay = decimal.Parse(ds.Tables[0].Rows[0]["居民大病保险资金支出"].ToString());//居民大病保险资金支出
                    decimal HifobPay = decimal.Parse(ds.Tables[0].Rows[0]["职工大额医疗费用补助基金支出"].ToString());//职工大额医疗费用补助基金支出
                    decimal MafPay = decimal.Parse(ds.Tables[0].Rows[0]["医疗救助基金支出"].ToString());//医疗救助基金支出
                    decimal OthPay = decimal.Parse(ds.Tables[0].Rows[0]["其他支出"].ToString());//其他支出
                    decimal AcctPay = decimal.Parse(ds.Tables[0].Rows[0]["个人账户支出"].ToString());//个人账户支出
                    decimal PsnCashPay = decimal.Parse(ds.Tables[0].Rows[0]["个人现金支出"].ToString());//个人现金支出
                    decimal FulamtOwnpayAmt = decimal.Parse(ds.Tables[0].Rows[0]["全自费金额"].ToString());//全自费金额

                    decimal OtherPay = CvlservPay + HifesPay + HifmiPay + HifobPay + MafPay + OthPay;//其他支付
                    decimal OwnCost = MedfeeSumamt - FulamtOwnpayAmt - HifpPay;//个人自付

                    this.lblPriPub.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(HifpPay, 2);//医保统筹基金支付
                    this.lblOtherPay.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(OtherPay, 2);//其他支付
                    this.lblOwnPay.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(AcctPay + MCZHFee, 2);//个人账户支付
                    this.lblPriPay.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(PsnCashPay - MCZHFee, 2);//个人现金支付
                    this.lblPriPayCost.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(FulamtOwnpayAmt, 2);//个人自费
                    this.lblPriOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(OwnCost, 2);//个人自付
                }
                else
                {
                    this.lblPriPub.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.PubCost, 2);//医保统筹基金支付
                    this.lblOtherPay.Text = "";//其他支付
                    this.lblOwnPay.Text = "";//个人账户支付
                    this.lblPriPayCost.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.PayCost, 2);
                    this.lblPriOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.OwnCost, 2);
                    this.lblPriPay.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.PayCost + invoice.FT.OwnCost, 2);
                }
            }
            #endregion
            //费用信息
            if (regInfo.Pact.Name == "公费")
            {
                this.lblbka841.Text = "公费医疗:" + Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.PubCost, 2);
                invoice.FT.PubCost = 0;
                this.lblPriPub.Text = "0";

            }
            else
            {
                if (false)
                {
                    //费用信息
                    this.lblPriPub.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.PubCost, 2);//医保统筹基金支付
                    this.lblOtherPay.Text = "";//其他支付
                    this.lblOwnPay.Text = "";//个人账户支付
                    this.lblPriPayCost.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.PayCost, 2);
                    this.lblPriOwnCost.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.OwnCost, 2);
                    this.lblPriPay.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.PayCost + invoice.FT.OwnCost, 2);
                }
                GetInsutype(regInfo.ID, regInfo.IDCard, ref medicaltype);
                if (string.IsNullOrEmpty(medicaltype))
                {
                    this.lblPriPub.Text = regInfo.Memo;
                }
                else
                {
                    this.lblPriPub.Text = medicaltype;
                }

            }

            //重取发药窗口
            SetDrugWinNo(invoice, regInfo.ID);

            //显示领药药房信息(去除重复)
            if (string.IsNullOrEmpty(invoice.DrugWindowsNO) == false)
            {
                string[] drugWindows = invoice.DrugWindowsNO.Split('|');
                ArrayList alDrugWindow = new ArrayList();
                string disPlayWindow = "";
                for (int i = 0; i < drugWindows.Length; i++)
                {
                    if (alDrugWindow.Contains(drugWindows[i]) == false)
                    {
                        alDrugWindow.Add(drugWindows[i]);
                        disPlayWindow += drugWindows[i] + ",";
                    }
                }
                this.lblDrugWindow.Visible = true;
                this.lblDrugWindow.Text = disPlayWindow.TrimEnd(',');
            }
            else
            {
                this.lblDrugWindow.Visible = false;
            }

            //总金额大小写
            this.lblPriLower.Text = Neusoft.FrameWork.Public.String.FormatNumberReturnString(invoice.FT.TotCost, 2);

            string[] strMoney = this.GetUpperCashbyNumber(Neusoft.FrameWork.Public.String.FormatNumber(invoice.FT.TotCost, 2));
            this.lblPriF.Text = strMoney[0];
            this.lblPriJ.Text = strMoney[1];
            this.lblPriY.Text = strMoney[3];
            this.lblPriS.Text = strMoney[4];
            this.lblPriB.Text = strMoney[5];
            this.lblPriQ.Text = strMoney[6];
            this.lblPriW.Text = strMoney[7];
            this.lblPriSW.Text = strMoney[8];
            //if (regInfo.SIMainInfo.ExtendProperty.ContainsKey("MINZHEN"))
            //{
            //    mzCost = regInfo.SIMainInfo.ExtendProperty["MINZHEN"].ToString();
            //    if (!string.IsNullOrEmpty(mzCost))
            //    {
            //        this.lblMZ.Text = "民政支付：" + mzCost;
            //        mzCost = string.Empty;
            //    }
            //}



            return 1;
        }


        void SetDrugWinNo(Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice, string patientId)
        {
            string w = invoice.DrugWindowsNO;
            Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("MzPk", w);
            string sql = @"select replace(wm_concat(t_name), ',', '|') n
                          from (select distinct fun_get_dept_name(a.drug_dept_code) || b.t_name as t_name
                                  from pha_sto_recipe a
                                  join pha_sto_terminal b
                                    on a.send_terminal = b.t_code
                                 where a.invoice_no = '{0}'
                                   and a.clinic_code = '{1}')";

            try
            {
                sql = string.Format(sql, invoice.Invoice.ID, patientId);
               string winNo= new Neusoft.HISFC.BizLogic.Fee.Outpatient().ExecSqlReturnOne(sql);
               Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("MzPk", sql);
               Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("MzPk", winNo);
               if (!string.IsNullOrEmpty(winNo))
               {
                   invoice.DrugWindowsNO = winNo;
               }
              // invoice.DrugWindowsNO = "测试9号窗|中药房1号窗";
            }
            catch (Exception ex)
            {
                invoice.DrugWindowsNO = w;
            }

        }

        void GetRegNo(string PatientID, ref string RegNo)
        {
            string sql = @"   select Reg_no from  FIN_IPR_SIINMAININFO_GD g
   where g.inpatient_no ='{0}' and g.type_code = '1'";
            try
            {
                sql = string.Format(sql, PatientID);
                RegNo = new Neusoft.HISFC.BizLogic.Fee.Outpatient().ExecSqlReturnOne(sql);
                if (RegNo == "-1")
                    RegNo = "";
            }
            catch (Exception ex)
            {
                RegNo = "";
            }

        }
        void GetInsutype(string regInfoid, string IDCardid, ref string medicaltype)
        {
            string sql = @" SELECT (case when substr(INSUPLCADMDVS,1,4)='4404'  then '珠海'when  substr(INSUPLCADMDVS,1,2)='44' 
         and  substr(INSUPLCADMDVS,1,4)<>'4404' then '省内异地' else '跨省异地' end) || (select name from  com_dictionary dic where TYPE='Insutype' and dic.code= gd.insutype)
         from  fin_ipr_siinmaininfo_gd gd where (INPATIENT_NO='{0}' or IDENNO='{1}' )     and INSUPLCADMDVS is not null and VALID_FLAG='1'
         and rownum=1 ORDER BY oper_date desc ";
            try
            {
                sql = string.Format(sql, regInfoid, IDCardid);
                medicaltype = new Neusoft.HISFC.BizLogic.Fee.Outpatient().ExecSqlReturnOne(sql);
                if (medicaltype == "-1")
                    medicaltype = "";
            }
            catch (Exception ex)
            {
                medicaltype = "";
            }

        }
        public string GetPartjlx(string regInfoid, ref string partjlx, ref string Taxid)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient db = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            string sql = @"  select PAR_TJLX,idenno from  com_patientinfo where CARD_NO='{0}' ";
            try
            {
                sql = string.Format(sql, regInfoid);

                if (db.ExecQuery(sql) == -1)
                {
                    partjlx = "0";
                    return partjlx;
                }
                while (db.Reader.Read())
                {
                    partjlx = db.Reader[0].ToString();
                    Taxid = db.Reader[1].ToString();
                }
            }
            catch (Exception ex)
            {
                partjlx = "0";
                db.Reader.Close();
                return partjlx;
            }
            finally
            {
                db.Reader.Close();
            }
            return partjlx;
        }


        public void SetTrans(IDbTransaction trans)
        {
            this.trans.Trans = trans;
        }

        public IDbTransaction Trans
        {
            set { }
        }

        private string GetGetPayMode(string paymodeID)
        {
            string sql = @"select NAME from com_dictionary d where d.type = 'PAYMODES' and d.valid_state = '1' and d.code = '{0}'";
            try
            {
                sql = string.Format(sql, paymodeID);
                string PayModeName = new Neusoft.HISFC.BizLogic.Fee.Outpatient().ExecSqlReturnOne(sql);
                return PayModeName;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region 支付方式帮助方法

        private string GetPayModeName(string paymodeID)
        {
            switch (paymodeID)
            {
                case "CA":
                    return "现金";
                case "MER":
                    return "银联卡";
                case "ICBC":
                    return "银联卡";
                case "NXS":
                    return "银联卡";
                case "COMM":
                    return "银联卡";
                case "PB":
                    return "记账";
                case "UP":
                    return "银联卡";
                case "MCZH":
                    return "社会保障卡(珠海)";
                case "MCDZ":
                    return "社会保障卡(电子支付)";
                case "MCZS":
                    return "社会保障卡(中山)";
                case "PBZH":
                    return "珠海医保统筹";
                case "PBZS":
                    return "中山医保统筹";
                case "RC":
                    return "优惠";
                case "CH":
                    return "支票";
                case "PO":
                    return "汇款";
                case "ZZ":
                    return "宰账";
                case "GZ":
                    return "社会保障卡(广州)";
                case "ZSMZ":
                    return "中山门诊民政救助：";
                case "ZFB":
                    return "支付宝：";
                case "WX":
                    return "微信：";
                case "CCB":
                    return "建行：";
                case "MCSNYD":
                    return "社会保障卡（省内异地）：";
                case "SNYDYBTC":
                    return "省内异地医保统筹：";
                case "SWYDYBTC":
                    return "省外异地医保统筹：";
                default:
                    return "其他";

            }
        }

        #endregion

        #region IInterfaceContainer 成员

        public Type[] InterfaceTypes
        {
            get
            {
                Type[] type = new Type[1];
                type[0] = typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint);
                return type;
            }
        }

        #endregion



    }
}
