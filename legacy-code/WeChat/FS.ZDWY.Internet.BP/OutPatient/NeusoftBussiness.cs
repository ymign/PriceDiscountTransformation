using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Fee.Outpatient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BP.OutPatient
{
    public class NeusoftBussiness
    {
        #region 业务层

        /// <summary>
        /// 管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// 医疗待遇接口
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareInterfaceProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();

        /// <summary>
        /// 挂号业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Registration.Register registerManager = new Neusoft.HISFC.BizLogic.Registration.Register();

        /// <summary>
        /// 预约管理类
        /// </summary>
        private Neusoft.HISFC.BizLogic.Registration.Booking bookingMgr = new Neusoft.HISFC.BizLogic.Registration.Booking();

        /// <summary>
        /// 排班管理类
        /// </summary>
        private Neusoft.HISFC.BizLogic.Registration.Schema schemaMgr = new Neusoft.HISFC.BizLogic.Registration.Schema();
        #endregion

        #region 挂号
        /// <summary>
        /// 获得患者应交金额、报销金额
        /// </summary>
        /// <param name="regFee"></param>
        /// <param name="chkFee"></param>
        /// <param name="digFee"></param>
        /// <param name="othFee"></param>
        /// <param name="digPub"></param>
        /// <param name="ownCost"></param>
        /// <param name="pubCost"></param>
        /// <param name="cardNo"></param>		
        private List<Neusoft.HISFC.Models.Account.AccountCardFee> BuildRegFee(Neusoft.HISFC.Models.Registration.Register register)
        {
            List<Neusoft.HISFC.Models.Account.AccountCardFee> lstAccFee = new List<Neusoft.HISFC.Models.Account.AccountCardFee>();
            Neusoft.HISFC.Models.Account.AccountCardFee cardFee = null;

            if (register.RegLvlFee.RegFee > 0)
            {
                cardFee = BuildAccountCardFee(Neusoft.HISFC.Models.Account.AccCardFeeType.RegFee, register.RegLvlFee.RegFee, 0);
                lstAccFee.Add(cardFee);
            }
            if (register.RegLvlFee.ChkFee > 0)
            {
                cardFee = BuildAccountCardFee(Neusoft.HISFC.Models.Account.AccCardFeeType.ChkFee, register.RegLvlFee.ChkFee, 0);
                lstAccFee.Add(cardFee);
            }
            if (register.RegLvlFee.OthFee > 0)
            {
                // 其他费当做病历本费处理
                cardFee = BuildAccountCardFee(Neusoft.HISFC.Models.Account.AccCardFeeType.CaseFee, register.RegLvlFee.OthFee, 0);
                lstAccFee.Add(cardFee);
            }
            if (register.RegLvlFee.OwnDigFee + register.RegLvlFee.PubDigFee > 0)
            {
                cardFee = BuildAccountCardFee(Neusoft.HISFC.Models.Account.AccCardFeeType.DiaFee, register.RegLvlFee.OwnDigFee, register.RegLvlFee.PubDigFee);
                lstAccFee.Add(cardFee);
            }

            return lstAccFee;
        }

        /// <summary>
        /// 创建挂号费实体
        /// </summary>
        /// <returns></returns>
        private Neusoft.HISFC.Models.Account.AccountCardFee BuildAccountCardFee(Neusoft.HISFC.Models.Account.AccCardFeeType feeType, decimal ownCost, decimal pubCost)
        {
            Neusoft.HISFC.Models.Account.AccountCardFee cardFee = new Neusoft.HISFC.Models.Account.AccountCardFee();
            cardFee.FeeType = feeType;
            cardFee.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
            cardFee.IStatus = 1;
            cardFee.Own_cost = ownCost;
            cardFee.Pub_cost = pubCost;
            cardFee.Tot_cost = ownCost + pubCost;

            return cardFee;
        }
        /// <summary>
        /// 计算医保费用
        /// </summary>
        /// <param name="register"></param>
        /// <param name="errInfo"></param>
        /// <returns></returns>
        public int ComputeRegCost(ref Neusoft.HISFC.Models.Registration.Register register, ref string errInfo)
        {
            //判断【合同单位】是否可以进行自助终端结算(常数维护)
            var selfFeePact = this.managerIntegrate.GetConstantList("SelfFeePact");   //可以进行自助终端结算的合同单位
            bool isCanSelfFee = false;
            foreach (Neusoft.FrameWork.Models.NeuObject o in selfFeePact)
            {
                if (o.ID.Equals(register.Pact.ID))
                {
                    isCanSelfFee = true;
                    break;
                }
            }
            if (!isCanSelfFee)
            {
                errInfo = "合同单位【" + register.Pact.Name + "】不允许进行微信或自助机挂号!请到人工窗口进行挂号!";
                return -1;
            }
            register.ID = "LOCK";
            register.InvoiceNO = "LOCK";

            // 挂号费用明细
            List<Neusoft.HISFC.Models.Account.AccountCardFee> lstAccFee = BuildRegFee(register);
            //有费用信息的时候才处理
            if (lstAccFee.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.Account.AccountCardFee accFee in lstAccFee)
                {
                    accFee.InvoiceNo = "LOCK";
                    accFee.Print_InvoiceNo = "LOCK";
                    accFee.ClinicNO = register.ID;

                    accFee.Patient.PID.CardNO = register.PID.CardNO;
                    accFee.Patient.Name = register.Name;

                    accFee.IStatus = 1;

                    //accFee.FeeOper.Oper.ID = "";
                    //accFee.FeeOper.Oper.Name = "";
                    accFee.FeeOper.ID = "";
                    accFee.FeeOper.Name = "";
                    accFee.FeeOper.OperTime = DateTime.Now;

                    //accFee.Oper.Oper.ID = "";
                    //accFee.Oper.Oper.Name = "";
                    accFee.Oper.ID = "";
                    accFee.Oper.Name = "";
                    accFee.Oper.OperTime = DateTime.Now;

                    accFee.IsBalance = false;
                    accFee.BalanceNo = "";

                    register.OwnCost += accFee.Own_cost;
                    register.PubCost += accFee.Pub_cost;
                    register.PayCost += accFee.Pay_cost;

                }
            }

            #region 待遇接口实现

            if ((register.OwnCost + register.PubCost + register.PayCost) > 0)
            {
                this.medcareInterfaceProxy.SetPactCode(register.Pact.ID);
                //连接待遇接口
                long returnValue = this.medcareInterfaceProxy.Connect();
                if (returnValue == -1)
                {
                    errInfo = "医疗待遇接口连接失败!" + this.medcareInterfaceProxy.ErrMsg;
                    return -1;
                }

                if (this.medcareInterfaceProxy.IsInBlackList(register))
                {
                    errInfo = this.medcareInterfaceProxy.ErrMsg;
                    return -1;
                }
                //todo：待确认是否需要
                //register.UsualObject = lstAccFee;  //{6E8955EE-09C2-40b5-89B7-B31326EDD753} 佛山居民医保二次改造

                register.SIMainInfo.InvoiceNo = register.InvoiceNO;
                returnValue = this.medcareInterfaceProxy.UploadRegInfoOutpatient(register);
                if (returnValue == -1)
                {
                    errInfo = "上传挂号信息失败!" + this.medcareInterfaceProxy.ErrMsg + "\r\n请到医院现场挂号!";
                    return -1;
                }

                register.OwnCost = register.SIMainInfo.OwnCost;  //自费金额
                register.PubCost = register.SIMainInfo.PubCost;  //统筹金额
                register.PayCost = register.SIMainInfo.PayCost;  //帐户金额

            }
            #endregion

            return 1;
        }

        ///// <summary>
        ///// 更新看诊序号
        ///// </summary>
        ///// <param name="Type">1医生 2科室 4全院</param>
        ///// <param name="seeDate">看诊日期</param>
        ///// <param name="Subject">Type=1时,医生代码;Type=2,科室代码;Type=4,ALL</param>
        ///// <param name="noonID">午别</param>
        ///// <returns></returns>
        //public int UpdateSeeNo(string Type, DateTime seeDate, string Subject, string noonID)
        //{            
        //    return registerManager.UpdateSeeNo(Type, seeDate, Subject, noonID);
        //}

        /// <summary>
        /// 预约锁号
        /// </summary>
        /// <param name="orderType">挂号类型	0：预约挂号，1:当日挂号</param>
        /// <param name="schema"></param>
        /// <param name="b"></param>
        /// <param name="oper"></param>
        /// <param name="bookingID">就诊序号</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public int LockPreRegInfo(string orderType, Models.FIN_OPR_SCHEMA schema, Models.FIN_OPR_BOOKING b, Models.OperInfo oper, ref string bookingID, ref string visitNo, ref string error)
        {
            try
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                this.registerManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                this.bookingMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                //this.schemaMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                #region 扣除排班对应的号源
                //需判断是当日号还是预约号,当天挂号扣减现场号源，预约挂号扣减预约号源
                /*
                if (orderType == "0")
                {
                    if (this.schemaMgr.Increase(schema.ID, false, true, false, false) == -1)
                    {
                        throw new Exception("更新医生排班信息时出错!" + this.schemaMgr.Err);
                    }
                }
                else if (orderType == "1")
                {
                    if (this.schemaMgr.Increase(schema.ID, true, false, false, false) == -1)
                    {
                        throw new Exception("更新医生排班信息时出错!" + this.schemaMgr.Err);
                    }
                }
                else
                {
                    throw new Exception("挂号类型不正确！");
                }
                */
                string strLockSchema = string.Empty;
                if (orderType == "0")
                {
                    strLockSchema = string.Format(@"update fin_opr_schema s --医师出诊表
   set s.tel_reged  = s.tel_reged + 1, --挂号已挂
       s.tel_reging = s.tel_reging + 1,
       s.order_no   = nvl(s.order_no, 0) + 1
 where s.id = '{0}'
   and s.tel_lmt > s.tel_reging
   and s.valid_flag='1'
   and s.stop<>'1'", schema.ID);
                }
                else if (orderType == "1")
                {
                    strLockSchema = string.Format(@"update fin_opr_schema s  --医师出诊表
                                set s.reged = s.reged + 1, --挂号已挂
                                    s.order_no = nvl(s.order_no,0) + 1
                                where s.id = '{0}' and  s.reg_lmt>s.reged", schema.ID);
                }
                else
                {
                    throw new Exception("挂号类型不正确！");
                }
                if (this.registerManager.ExecNoQuery(strLockSchema) <= 0)
                {
                    throw new Exception("号源已经用完，或停诊！请选择其他排班" + this.registerManager.Err);
                }

                #endregion

                #region 更新看诊序号
                string typeID = schema.SCHEMA_TYPE == "0" ? "2" : "1";
                string subject;
                if (typeID == "1")
                {
                    subject = schema.DOCT_CODE;
                }
                else
                {
                    subject = schema.DEPT_CODE;
                }
                //if (registerManager.UpdateSeeNo(typeID, schema.SEE_DATE, subject, schema.NOON_CODE) <= 0)
                //{
                //    throw new Exception("更新看诊序号失败！" + this.registerManager.Err);
                //}
                //更新看诊序号
                //全院是全天大排序，所以午别不生效，默认 1
                //if (registerManager.UpdateSeeNo("4", schema.SEE_DATE, "ALL", "1") == -1)
                //{
                //    throw new Exception("更新全院看诊序号失败！" + this.registerManager.Err);
                //}
                #endregion

                #region 写入到FIN_OPR_BOOKING预约表
                int seeNo = 0;
                if (this.registerManager.GetSeeNo(typeID, schema.SEE_DATE, subject, schema.NOON_CODE, ref seeNo) == -1)
                {
                    throw new Exception("获取看诊序号失败！" + this.registerManager.Err);
                }
                visitNo = seeNo.ToString();
                Neusoft.HISFC.Models.Registration.Booking booking = new Neusoft.HISFC.Models.Registration.Booking();
                booking.DoctorInfo.SeeNO = seeNo;
                //获取全院看诊序号
                if (registerManager.GetSeeNo("4", schema.SEE_DATE, "ALL", "1", ref seeNo) == -1)
                {
                    throw new Exception("获取全院看诊序号失败！" + this.registerManager.Err);
                }

                booking.OrderNO = seeNo;
                booking.BookingTypeId = "3";  //微信   //todo:可以考虑对照匹配
                booking.DoctorInfo.Templet.ID = schema.ID;

                string getBookingIDSql = "select seq_fin_booking.nextval from dual";
                bookingID = bookingMgr.ExecSqlReturnOne(getBookingIDSql);
                booking.ID = bookingID;
                booking.Name = b.NAME;
                booking.IDCard = b.IDENNO;
                booking.PhoneHome = b.RELA_PHONE;
                booking.AddressHome = b.ADDRESS;
                booking.PID.CardNO = b.CARD_NO;
                //booking.DoctorInfo = schema.Clone();

                booking.DoctorInfo.Templet.ID = schema.ID;
                booking.DoctorInfo.SeeDate = schema.SEE_DATE;
                booking.DoctorInfo.Templet.Begin = schema.BEGIN_TIME;
                booking.DoctorInfo.Templet.End = schema.END_TIME;
                booking.DoctorInfo.Templet.Dept.ID = schema.DEPT_CODE;
                booking.DoctorInfo.Templet.Dept.Name = schema.DEPT_NAME;
                booking.DoctorInfo.Templet.Doct.ID = schema.DOCT_CODE;
                booking.DoctorInfo.Templet.Doct.Name = schema.DOCT_NAME;
                booking.DoctorInfo.Templet.Noon.ID = schema.NOON_CODE;
                booking.DoctorInfo.Templet.IsAppend = schema.APPEND_FLAG == "1" ? true : false;
                booking.DoctorInfo.Templet.RegLevel.ID = schema.REGLEVL_CODE;

                booking.Oper.ID = oper.Code;
                booking.Oper.OperTime = oper.Time;

                booking.BookingTypeId = "3";  //3好像是微信
                booking.BookingTypeName = "微信预约";
                booking.Sex.ID = b.SEX_CODE;
                booking.Birthday = b.BIRTHDAY;

                if (this.bookingMgr.Insert(booking) == -1)
                {
                    throw new Exception("登记患者预约信息时出错!" + this.bookingMgr.Err);
                }

                #endregion

                //todo:可以考虑更新患者信息

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                error = ex.Message;
                return -1;
            }
        }

        public int LockRegInfo(string orderType, Models.FIN_OPR_SCHEMA schema, Models.FIN_OPR_BOOKING b, Models.OperInfo oper, ref string bookingID, ref string visitNo, ref string error)
        {
            try
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                this.registerManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                this.bookingMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                #region 写入到FIN_OPR_BOOKING预约表
                int seeNo = 0;

                Neusoft.HISFC.Models.Registration.Booking booking = new Neusoft.HISFC.Models.Registration.Booking();
                booking.DoctorInfo.SeeNO = seeNo;
                //获取全院看诊序号
                if (registerManager.GetSeeNo("4", schema.SEE_DATE, "ALL", "1", ref seeNo) == -1)
                {
                    throw new Exception("获取全院看诊序号失败！" + this.registerManager.Err);
                }

                booking.OrderNO = seeNo;
                booking.BookingTypeId = "3";  //微信   //todo:可以考虑对照匹配
                booking.DoctorInfo.Templet.ID = schema.ID;

                string getBookingIDSql = "select seq_fin_booking.nextval from dual";
                bookingID = bookingMgr.ExecSqlReturnOne(getBookingIDSql);
                booking.ID = bookingID;
                booking.Name = b.NAME;
                booking.IDCard = b.IDENNO;
                booking.PhoneHome = b.RELA_PHONE;
                booking.AddressHome = b.ADDRESS;
                booking.PID.CardNO = b.CARD_NO;
                //booking.DoctorInfo = schema.Clone();

                booking.DoctorInfo.Templet.ID = schema.ID;
                booking.DoctorInfo.SeeDate = schema.SEE_DATE;
                booking.DoctorInfo.Templet.Begin = schema.BEGIN_TIME;
                booking.DoctorInfo.Templet.End = schema.END_TIME;
                booking.DoctorInfo.Templet.Dept.ID = schema.DEPT_CODE;
                booking.DoctorInfo.Templet.Dept.Name = schema.DEPT_NAME;
                booking.DoctorInfo.Templet.Doct.ID = schema.DOCT_CODE;
                booking.DoctorInfo.Templet.Doct.Name = schema.DOCT_NAME;
                booking.DoctorInfo.Templet.Noon.ID = schema.NOON_CODE;
                booking.DoctorInfo.Templet.IsAppend = schema.APPEND_FLAG == "1" ? true : false;
                booking.DoctorInfo.Templet.RegLevel.ID = schema.REGLEVL_CODE;

                booking.Oper.ID = oper.Code;
                booking.Oper.OperTime = oper.Time;

                booking.BookingTypeId = "3";  //3好像是微信
                booking.BookingTypeName = "微信挂号";
                booking.Sex.ID = b.SEX_CODE;
                booking.Birthday = b.BIRTHDAY;

                if (this.bookingMgr.Insert(booking) == -1)
                {
                    throw new Exception("登记患者预约信息时出错!" + this.bookingMgr.Err);
                }

                #endregion

                //todo:可以考虑更新患者信息

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                error = ex.Message;
                return -1;
            }

        }
        #endregion

        #region 缴费

        /// <summary>
        /// 是否退费
        /// </summary>
        private static bool isQuitFee = false;

        /// <summary>
        /// 是否退费
        /// </summary>
        public static bool IsQuitFee
        {
            get
            {
                return isQuitFee;
            }
            set
            {
                isQuitFee = value;
            }
        }

        /// <summary>
        /// 控制参数类
        /// </summary>
        protected static Neusoft.FrameWork.Management.ControlParam controlManager = new Neusoft.FrameWork.Management.ControlParam();

        public static Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();

        public static ArrayList MakeInvoice(Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate, Neusoft.HISFC.Models.Registration.Register register, ArrayList feeItemLists,
string invoiceBeginNO, string realInvoiceBeginNO, ref string errText)
        {
            return MakeInvoice(feeIntegrate, register, feeItemLists, invoiceBeginNO, realInvoiceBeginNO, ref errText, null);
        }

        public static ArrayList MakeInvoice(Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate, Neusoft.HISFC.Models.Registration.Register register, ArrayList feeItemLists,
    string invoiceBeginNO, string realInvoiceBeginNO, ref string errText, System.Data.IDbTransaction t)
        {
            const string OWN_INVOICE = "1";//自费发票
            const string PUB_INVOICE = "2";//记帐发票
            const string SP_INVOICE = "3";//特殊发票
            const string YB_INVOICE = "4";//医保发票
            const string MAIN_INVOICE = "5";//所有费用信息形成的发票

            int returnValue = 0;//返回值
            DataSet dsInvoice = new DataSet();//发票大类
            ArrayList balancesAndBalanceListsAndFeeListsAll = new ArrayList();//所有发票和发票明细信息
            ArrayList balances = new ArrayList();  //发票主表集合
            ArrayList balanceLists = new ArrayList();//发票明细集合
            //发票费用明细
            ArrayList feeLists = new ArrayList();
            if (t != null)
            {
                feeIntegrate.SetTrans(t);
                controlManager.SetTrans(t);
            }

            #region 获得门诊发票大类-修改为从接口取发票大类

            Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
            string invoicePrintDll = controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, false, string.Empty);

            //Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint iInvoicePrint = null;
            Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IInvoicePrint.ucInvoicePrint iInvoicePrint
                = new Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IInvoicePrint.ucInvoicePrint();
            // 更改发票打印类获取方式；兼容原来方式【参数维护 和 接口维护】
            bool blnNewPrintStyle = false;
            if (string.IsNullOrEmpty(invoicePrintDll))
            {
                blnNewPrintStyle = true;
            }

            if (!blnNewPrintStyle)
            {
                #region 发票打印旧方式
                invoicePrintDll = Neusoft.FrameWork.Management.Connection.SystemPath + "\\Bin" + "\\" + invoicePrintDll;

                object obj = null;
                System.Reflection.Assembly a = System.Reflection.Assembly.LoadFrom(invoicePrintDll);
                try
                {
                    System.Type[] types = a.GetTypes();


                    foreach (System.Type type in types)
                    {
                        if (type.GetInterface("IInvoicePrint") != null)
                        {
                            try
                            {
                                obj = System.Activator.CreateInstance(type);
                                //iInvoicePrint = obj as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint;
                                break;
                            }
                            catch (Exception ex)
                            {
                                errText = ex.Message;
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errText = ex.Message;
                    return null;
                }
                #endregion
            }
            else
            {
                #region 新方式
                //iInvoicePrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(typeof(Neusoft.HISFC.BizProcess.Integrate.Fee), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint)) as IInvoicePrint;
                if (iInvoicePrint == null)
                {
                    errText = "请维护打印票据，查找打印票据失败！";
                    return null;
                }
                #endregion
            }
            iInvoicePrint.Register = register;

            #endregion

            returnValue = feeIntegrate.GetInvoiceClass(iInvoicePrint.InvoiceType, ref dsInvoice);
            if (dsInvoice.Tables[0].PrimaryKey.Length == 0)
            {
                dsInvoice.Tables[0].PrimaryKey = new DataColumn[] { dsInvoice.Tables[0].Columns["FEE_CODE"] };
            }
            foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in feeItemLists)
            {
                DataRow rowFind = dsInvoice.Tables[0].Rows.Find(new object[] { f.Item.MinFee.ID });
                //找到相应对应的发票项目
                f.Item.MinFee.User01 = rowFind["FEE_STAT_CATE"].ToString();
                f.Invoice.Type.ID = rowFind["FEE_STAT_CATE"].ToString();

            }

            string splitType = feeIntegrate.GetControlValue(Neusoft.HISFC.BizProcess.Integrate.Const.AUTO_INVOICE_TYPE, "0");

            #region 门诊分发票

            if (feeItemLists.Count > 0)
            {
                string tempInvoiceNO = invoiceBeginNO;//主体发票的发票号不需要累加,主要是为了

                ArrayList feeItemListSplit = feeIntegrate.SplitInvoice(register, ref feeItemLists);
                if (feeItemListSplit == null)
                {
                    errText = feeIntegrate.Err;
                    return null;
                }

                int i = 0;
                foreach (ArrayList list in feeItemListSplit)
                {
                    Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice = new Neusoft.HISFC.Models.Fee.Outpatient.Balance(); //发票实体
                    ArrayList tempBalanceLists = new ArrayList();//发票明细实体集合

                    if (t != null)
                    {
                        outpatientManager.SetTrans(t);
                    }
                    string invoiceCombNO = outpatientManager.GetInvoiceCombNO();//获得发票序列

                    returnValue = MakeInvoiceAndDetail(
                        feeIntegrate, list, register, ref tempInvoiceNO, ref realInvoiceBeginNO,
                        dsInvoice.Tables[0], OWN_INVOICE, ref invoice,
                        ref tempBalanceLists, splitType, ref errText, i);
                    if (returnValue == -1)
                    {
                        return null;
                    }
                    if (returnValue != -2)
                    {
                        #region 门诊发票的seq

                        //对返回的 invoice, list, tempBalanceLists 赋值也可以.
                        invoice.CombNO = invoiceCombNO;
                        foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in list)
                        {
                            f.InvoiceCombNO = invoiceCombNO;
                        }
                        foreach (ArrayList tempBalanceList in tempBalanceLists)
                        {
                            foreach (Neusoft.HISFC.Models.Fee.Outpatient.BalanceList detail in tempBalanceList)
                            {
                                ((Neusoft.HISFC.Models.Fee.Outpatient.Balance)detail.BalanceBase).CombNO = invoiceCombNO;
                            }
                        }

                        #endregion

                        //发票所对应的费用明细
                        feeLists.Add(list);
                        balances.Add(invoice);
                        balanceLists.Add(tempBalanceLists);
                    }
                    i++;
                }

                //当患者上传医保后公费字段有值时走一下方法，可使发票显示正确
                if (register.Pact.PayKind.ID != "01" && register.SIMainInfo.PubCost > 0)
                {
                    if (balances != null && balances.Count != 0)
                    {
                        decimal rate = 0;
                        decimal totPubCost = 0;
                        decimal totPayCost = 0;
                        Neusoft.HISFC.Models.Fee.Outpatient.Balance balance = new Neusoft.HISFC.Models.Fee.Outpatient.Balance();
                        int intCount = 0;

                        #region 算出每张发票的pub_cost之和，然后和总的register.SIMaininfo.PubCost相比

                        decimal balanceTotCost = 0;
                        for (int k = 0; k < balances.Count; k++)
                        {
                            Neusoft.HISFC.Models.Fee.Outpatient.Balance bTemp = balances[k] as Neusoft.HISFC.Models.Fee.Outpatient.Balance;
                            balanceTotCost += bTemp.FT.PubCost;
                        }

                        #endregion

                        //如果费用明细中返回pub_cost，则不需要加权平均分pub_cost.gmz
                        if (balanceTotCost != register.SIMainInfo.PubCost)
                        {
                            //费用明细中pub_cost为0，例如：佛山特定医保，需要加权平均
                            for (intCount = 0; intCount < balances.Count - 1; intCount++)
                            {
                                balance = balances[intCount] as Neusoft.HISFC.Models.Fee.Outpatient.Balance;
                                rate = balance.FT.TotCost / register.SIMainInfo.TotCost;
                                balance.FT.PubCost = register.SIMainInfo.PubCost * rate;
                                balance.FT.PayCost = register.SIMainInfo.PayCost * rate;
                                balance.FT.PayCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.PayCost, 2);
                                balance.FT.PubCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.PubCost, 2);
                                balance.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.TotCost, 2);
                                balance.FT.OwnCost = balance.FT.TotCost - balance.FT.PubCost - balance.FT.PayCost;
                                totPayCost += balance.FT.PayCost;
                                totPubCost += balance.FT.PubCost;
                            }

                        }
                        else
                        {
                            //如果费用明细中pub_cost不为0，例如：佛山居民医保，不需要加权平均
                            for (intCount = 0; intCount < balances.Count - 1; intCount++)
                            {
                                balance = balances[intCount] as Neusoft.HISFC.Models.Fee.Outpatient.Balance;
                                balance.FT.PayCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.PayCost, 2);
                                balance.FT.PubCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.PubCost, 2);
                                balance.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.TotCost, 2);
                                balance.FT.OwnCost = balance.FT.TotCost - balance.FT.PubCost - balance.FT.PayCost;
                                totPayCost += balance.FT.PayCost;
                                totPubCost += balance.FT.PubCost;
                            }
                        }

                        balance = balances[intCount] as Neusoft.HISFC.Models.Fee.Outpatient.Balance;
                        balance.FT.PubCost = register.SIMainInfo.PubCost - totPubCost;
                        balance.FT.PayCost = register.SIMainInfo.PayCost - totPayCost;
                        balance.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.TotCost, 2);
                        balance.FT.OwnCost = balance.FT.TotCost - balance.FT.PubCost - balance.FT.PayCost;
                    }
                }
            }

            #endregion

            balancesAndBalanceListsAndFeeListsAll.Add(balances);       //发票主表
            balancesAndBalanceListsAndFeeListsAll.Add(balanceLists);   //发票明细
            balancesAndBalanceListsAndFeeListsAll.Add(feeLists);       //发票费用明细

            return balancesAndBalanceListsAndFeeListsAll;
        }

        private static int MakeInvoiceAndDetail(Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate, ArrayList feeItemLists, Neusoft.HISFC.Models.Registration.Register register, ref string invoiceNO, ref string realInvoiceNO, DataTable dtInvoice,
    string invoiceFlag, ref Neusoft.HISFC.Models.Fee.Outpatient.Balance balance, ref ArrayList balanceLists, string splitType, ref string errText, int countI)
        {
            string invoiceType = feeIntegrate.GetControlValue(Neusoft.HISFC.BizProcess.Integrate.Const.GET_INVOICE_NO_TYPE, "0");
            Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();
            employee.ID = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code; //Neusoft.FrameWork.Management.Connection.Operator.ID;
            if (invoiceType == "2")
            {
                #region {3E09A62D-504B-4490-80A1-256F021B1ABD}

                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                for (int i = 0; i <= countI; i++)
                {
                    int iReturnValue = feeIntegrate.GetInvoiceNO(employee, "C", ref invoiceNO, ref realInvoiceNO, ref errText);
                    if (iReturnValue == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        return -1;
                    }
                }
                Neusoft.FrameWork.Management.PublicTrans.RollBack();

                #endregion
            }

            //形成门诊主发票
            balance = MakeInvoiceInfo(feeItemLists, register, invoiceNO, realInvoiceNO, invoiceFlag, splitType);
            //if (balance.FT.TotCost <= 0)
            //{
            //    return -2;
            //}
            //清空发票统计大类的费用合计.
            ResetInvoiceTable(dtInvoice);

            //形成门诊发票明细
            ArrayList tempBalanceLists = MakeInvoiceDetail(feeItemLists, register, invoiceNO, dtInvoice, invoiceFlag, splitType, ref errText);
            if (tempBalanceLists == null)
            {
                return -1;
            }
            //把自费发票加入发票明细表集合.
            balanceLists.Add(tempBalanceLists);

            //明细重新赋值发票【分发票时所有费用明细都为最后一张发票号】
            foreach (FeeItemList f in feeItemLists)
            {
                f.Invoice = balance.Invoice;
            }

            try
            {
                switch (invoiceType)
                {

                    case "1":
                    case "3"://中山
                        int len = invoiceNO.Length;
                        string orgInvoice = invoiceNO.Substring(0, len - 4);
                        string addInvoice = invoiceNO.Substring(len - 4, 4);
                        invoiceNO = orgInvoice + (NConvert.ToInt32(addInvoice) + 1).ToString().PadLeft(4, '0');
                        realInvoiceNO = Neusoft.FrameWork.Public.String.AddNumber(realInvoiceNO, 1);
                        break;
                    default:
                        invoiceNO = Neusoft.FrameWork.Public.String.AddNumber(invoiceNO, 1);
                        realInvoiceNO = invoiceNO;
                        break;
                }
            }
            catch (Exception e)
            {
                errText = e.Message;
                return -1;
            }

            return 1;
        }

        private static Neusoft.HISFC.Models.Fee.Outpatient.Balance MakeInvoiceInfo(ArrayList feeItemLists, Neusoft.HISFC.Models.Registration.Register register, string invoiceNO,
    string realInvoiceNO, string invoiceFlag, string splitFlag)
        {
            Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice = new Neusoft.HISFC.Models.Fee.Outpatient.Balance();
            decimal totCost = 0;//总金额
            decimal ownCost = 0;//自费金额
            decimal pubCost = 0;//记帐金额
            decimal payCost = 0;//自付金额
            decimal rebateCost = 0;//优惠价格

            if (invoiceFlag == "1")//自费发票
            {

                foreach (FeeItemList f in feeItemLists)
                {
                    ownCost += f.FT.OwnCost;
                    payCost += f.FT.PayCost;
                    pubCost += f.FT.PubCost;
                    rebateCost += f.FT.RebateCost;  //处理减免
                }
                totCost = ownCost + payCost + pubCost;
            }

            if (splitFlag == "0")//广医
            {
                if (invoiceFlag == "2")//记帐发票,临时这样写,以后要考虑公费的算法
                {
                    foreach (FeeItemList f in feeItemLists)
                    {
                        payCost += f.FT.PayCost;
                        pubCost += f.FT.PubCost;
                        rebateCost += f.FT.RebateCost; //处理减免
                    }
                    totCost = payCost + pubCost;
                }
                if (invoiceFlag == "3")//特殊发票,临时这样写,以后还要考虑.
                {
                    foreach (FeeItemList f in feeItemLists)
                    {
                        payCost += f.FT.PayCost;
                        pubCost += f.FT.PubCost;
                        rebateCost += f.FT.RebateCost; //处理减免
                    }
                    totCost = payCost + pubCost;
                }
            }

            if (splitFlag == "1")//中山
            {
                if (invoiceFlag == "2")//记帐发票,临时这样写,以后要考虑公费的算法
                {
                    foreach (FeeItemList f in feeItemLists)
                    {
                        ownCost += f.FT.OwnCost;
                        payCost += f.FT.PayCost;
                        pubCost += f.FT.PubCost;
                        rebateCost += f.FT.RebateCost;//处理减免 
                    }
                    totCost = ownCost + payCost + pubCost;
                }
                if (invoiceFlag == "3")//特殊发票,临时这样写,以后还要考虑.
                {
                    foreach (FeeItemList f in feeItemLists)
                    {
                        ownCost += f.FT.OwnCost;
                        payCost += f.FT.PayCost;
                        pubCost += f.FT.PubCost;
                        rebateCost += f.FT.RebateCost; //处理减免 
                    }
                    totCost = ownCost + payCost + pubCost;
                }
            }

            if (invoiceFlag == "4")//医保发票,暂时这么写,因为不知道怎么写.
            {
                foreach (FeeItemList f in feeItemLists)
                {
                    ownCost += f.FT.OwnCost;
                    payCost += f.FT.PayCost;
                    pubCost += f.FT.PubCost;
                    rebateCost += f.FT.RebateCost; //处理减免
                }
                totCost = ownCost + payCost + pubCost;
            }
            if (invoiceFlag == "5")//所有发票,如果是公费患者这里要写算法.
            {
                foreach (FeeItemList f in feeItemLists)
                {
                    payCost += f.FT.PayCost;
                    ownCost += f.FT.OwnCost;
                    pubCost += f.FT.PubCost;
                    rebateCost += f.FT.RebateCost; //处理减免
                }
                totCost = ownCost + payCost + pubCost;
            }

            #region 给明细发票号赋值

            foreach (FeeItemList f in feeItemLists)
            {
                f.Invoice.ID = invoiceNO;
            }

            #endregion

            invoice.Invoice.ID = invoiceNO;
            invoice.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
            invoice.Patient = register.Clone();
            invoice.FT.OwnCost = ownCost;
            invoice.FT.PayCost = payCost;
            invoice.FT.PubCost = pubCost;
            invoice.FT.TotCost = totCost;
            invoice.FT.RebateCost = rebateCost;
            invoice.User01 = rebateCost.ToString();
            string tempExamineFlag = null;
            if (register.ChkKind.Length > 0)
            {
                tempExamineFlag = register.ChkKind;
            }
            else
            {
                tempExamineFlag = "0";
            }
            invoice.ExamineFlag = tempExamineFlag;
            invoice.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
            invoice.CanceledInvoiceNO = "";
            invoice.IsDayBalanced = false;
            invoice.Memo = invoiceFlag;
            invoice.PrintTime = DateTime.Now;
            invoice.PrintedInvoiceNO = realInvoiceNO;
            invoice.IsAccount = true;     //终端发票

            return invoice;
        }


        private static void ResetInvoiceTable(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                row["TOT_COST"] = 0;
                row["OWN_COST"] = 0;
                row["PAY_COST"] = 0;
                row["PUB_COST"] = 0;
            }
        }


        /// <summary>
        /// 生成门诊发票明细
        /// </summary>
        /// <param name="feeItemLists">费用明细集合</param>
        /// <param name="register">挂号信息</param>
        /// <param name="invoiceNO">发票号</param>
        /// <param name="dtInvoice">发票大类</param>
        /// <param name="invoiceFlag">发票标志</param>
        /// <param name="splitType">分发票标志</param>
        /// <param name="errText">错误信息</param>
        /// <returns>成功: 发票明细集合 失败 null</returns>
        private static ArrayList MakeInvoiceDetail(ArrayList feeItemLists, Neusoft.HISFC.Models.Registration.Register register, string invoiceNO, DataTable dtInvoice, string invoiceFlag, string splitType, ref string errText)
        {
            ArrayList balanceLists = new ArrayList();

            foreach (FeeItemList f in feeItemLists)
            {
                DataRow rowFind = dtInvoice.Rows.Find(new object[] { f.Item.MinFee.ID });
                if (rowFind == null)
                {
                    errText = "最小费用为【" + f.Item.MinFee.ID + "】的最小费用没有在MZ01的发票大类中维护";
                    return null;
                }
                if (invoiceFlag == "1")//自费发票
                {
                    rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.OwnCost;
                    rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + f.FT.OwnCost;
                    rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                    rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;

                }
                if (splitType == "0")//广医
                {
                    if (invoiceFlag == "2" || invoiceFlag == "3")//记帐发票,特殊发票
                    {
                        rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.PayCost + f.FT.PubCost;
                        rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + 0;
                        rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                        rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                    }
                }
                if (splitType == "1")//中山
                {
                    if (invoiceFlag == "2" || invoiceFlag == "3")//记帐发票,特殊发票
                    {
                        rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.PayCost + f.FT.PubCost + f.FT.OwnCost;
                        rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + f.FT.OwnCost;
                        rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                        rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                    }
                }
                if (invoiceFlag == "4")//医保发票
                {
                    rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.PayCost + f.FT.PubCost + f.FT.OwnCost;
                    rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + f.FT.OwnCost;
                    rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                    rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                }

                if (invoiceFlag == "5")
                {
                    rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.PayCost + f.FT.PubCost + f.FT.OwnCost;
                    rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + f.FT.OwnCost;
                    rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                    rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                }

            }

            BalanceList detail = null;//发票明细实体

            for (int i = 1; i < 100; i++)
            {
                //找到相同打印序号,即同一统计类别的费用集合
                DataRow[] rowFind = dtInvoice.Select("SEQ = " + i.ToString(), "SEQ ASC");
                //如果没有找到说明已经找过了最大的打印序号,所有费用已经累加完毕.
                if (rowFind.Length == 0)
                {

                }
                else
                {
                    detail = new BalanceList();
                    detail.BalanceBase.Invoice.ID = invoiceNO;
                    detail.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                    detail.InvoiceSquence = i;
                    detail.FeeCodeStat.ID = rowFind[0]["FEE_STAT_CATE"].ToString();
                    detail.FeeCodeStat.Name = rowFind[0]["FEE_STAT_NAME"].ToString();

                    ///2007-8-20修改，保存打印序号到实体。
                    ///----------------------------------------------------
                    detail.FeeCodeStat.SortID = NConvert.ToInt32(rowFind[0]["SEQ"].ToString());
                    ///---------------------------------------------------

                    detail.BalanceBase.IsDayBalanced = false;
                    detail.BalanceBase.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                    detail.Memo = invoiceFlag;
                    foreach (DataRow row in rowFind)
                    {
                        detail.BalanceBase.FT.PubCost += NConvert.ToDecimal(row["PUB_COST"].ToString());
                        detail.BalanceBase.FT.OwnCost += NConvert.ToDecimal(row["OWN_COST"].ToString());
                        detail.BalanceBase.FT.PayCost += NConvert.ToDecimal(row["PAY_COST"].ToString());
                    }
                    detail.BalanceBase.FT.TotCost = detail.BalanceBase.FT.PubCost + detail.BalanceBase.FT.OwnCost + detail.BalanceBase.FT.PayCost;
                    //如果费用为0 说明次统计类别(打印序号)下没有费用
                    //处理四舍五入费用，暂时屏蔽，小于0也可以打印在发票上{DE54BEAE-EF40-4aa4-8DF5-8CCB2A3DDA1D}
                    if (detail.BalanceBase.FT.TotCost == 0)
                    {
                        continue;
                    }

                    balanceLists.Add(detail);
                }
            }

            return balanceLists;
        }

        public static bool GetSendPackage()
        {
            try
            {
                string sql = @"select nvl(a.code,'0') pk from com_dictionary a
 where a.type='ISSENDPACKAGE' ";
                string pk = outpatientManager.ExecSqlReturnOne(sql);
                return Neusoft.FrameWork.Function.NConvert.ToBoolean(pk);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 返回项目比例
        /// </summary>
        /// <param name="r">合同单位编码</param>
        /// <param name="f">费用明细</param>
        /// <param name="errMsg">返回错误信息</param>
        /// <returns></returns>
        public static Neusoft.HISFC.Models.Base.PactItemRate PactRate(Neusoft.HISFC.Models.Registration.Register r, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f, ref string errMsg)
        {
            Neusoft.HISFC.Models.Base.PactItemRate pRate = new Neusoft.HISFC.Models.Base.PactItemRate();
            pRate.Rate.RebateRate = 0;
            return pRate;
        }

        #endregion
    }
}
