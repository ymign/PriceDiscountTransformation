using FS.ZDWY.Internet.BL.InPatient;
using FS.ZDWY.Internet.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BP.InPatient
{
    public class InRegister
    {

        #region 属性

        /// <summary>
        /// 间隔小时
        /// </summary>
        private int intervalHour = 3;
        public int IntervalHour
        {
            get
            {
                return intervalHour;
            }
            set
            {
                this.intervalHour = value;
            }
        }

        private int showDays = 1;
        public int ShowDays
        {
            get
            {
                return showDays;
            }
            set
            {
                this.showDays = value;
            }
        }

        private bool isArriveProcess = false;
        public bool IsArriveProcess
        {
            get
            {

                return isArriveProcess;
            }
            set
            {
                isArriveProcess = value;
            }
        }

        /// <summary>
        /// 是否生成默认警戒线
        /// </summary>
        private bool isCreateMoneyAlert = false;
        public bool IsCreateMoneyAlert
        {
            get
            {
                return this.isCreateMoneyAlert;
            }
            set
            {
                this.isCreateMoneyAlert = value;
            }
        }

        /// <summary>
        /// 是否默认临时号
        /// </summary>
        private int istemp = 0;
        public int IsTempNo
        {
            get
            {
                return istemp;
            }
            set
            {
                this.istemp = value;
            }
        }

        /// <summary>
        /// 是否上传平台，获取EMPI号
        /// </summary>
        private bool isUploadEMPI = false;

        /// <summary>
        /// 是否上传平台，获取EMPI号
        /// </summary>
        public bool IsUploadEMPI
        {
            get
            {
                return isUploadEMPI;
            }
            set
            {
                this.isUploadEMPI = value;
            }
        }

        /// <summary>
        /// 是否提醒打印腕带
        /// </summary>
        private bool isPrintBracelet = false;

        /// <summary>
        /// 是否提醒打印腕带
        /// </summary>
        public bool IsPrintBracelet
        {
            get
            {
                return isPrintBracelet;
            }
            set
            {
                this.isPrintBracelet = value;
            }
        }

        /// <summary>
        /// 是否启用自动生成住院号
        /// </summary>
        private bool isAutoPatientNO = true;

        public bool IsAutoPatientNO
        {
            get
            {
                return isAutoPatientNO;
            }
            set
            {
                isAutoPatientNO = value;
            }
        }
        /// <summary>
        /// 是否默认生殖住院号
        /// </summary>
        private bool isFertilityPatientNO = false;
        public bool IsFertilityPatientNO
        {
            get
            {
                return isFertilityPatientNO;
            }
            set
            {
                isFertilityPatientNO = value;
            }
        }

        #endregion
        RegisterInfo IRegister = new RegisterInfo();
        Neusoft.SOC.Local.RADT.ZhuHai.ZDWY.Interface.SavePatient ISave = new Neusoft.SOC.Local.RADT.ZhuHai.ZDWY.Interface.SavePatient();
        /// <summary>
        /// 入出转
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();
        /// <summary>
        /// 患者入出转
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.InPatient inpatientManager = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        /// <summary>
        /// 费用公用接口业务层
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        /// <summary>
        /// 入出转
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        Neusoft.HISFC.BizLogic.RADT.InPatient radtInpatient = new Neusoft.HISFC.BizLogic.RADT.InPatient();

        Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareInterfaceProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();
        private bool isModify = false;
        private string tempUpdatePatientID;
        private string defaultPactCode = "2";
        private bool isAllowModifyInDate = false;
        private bool isNewPatient = true;
        private string tempPatientNo = "";

        /// <summary>
        /// 查询入院通知单信息
        /// </summary>
        /// <param name="cardNO">门诊卡号</param>
        /// <returns></returns>
        public IprPrepayinInfo GetIprPrepayinByCardNo(string cardNO)
        {
            try
            {
                DateTime date = DateTime.Parse(inpatientManager.GetSysDateTime()).AddDays(-15);//获取15天前日期
                IprPrepayinInfoLogic logic = new IprPrepayinInfoLogic();//数据库访问
                List<IprPrepayinInfo> iprs = logic.GetIprPrepayinInfo(cardNO, date);//获取15天内入院通知单
                if (iprs.Count > 1)
                {
                    throw new Exception("存在多条入院通知单信息，请到窗口办理入院");
                }
                else if (iprs.Count == 0)
                {
                    throw new Exception("无入院通知单信息，请到窗口办理入院");
                }
                else
                {
                    return iprs[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public InPatientRegistInfo GetLastRegistInfo(string cardNo)
        {
            InMainInfoLogic infoLogic = new InMainInfoLogic();
            FIN_IPR_INMAININFO inInfo = infoLogic.GetINMAININFOByCardID(cardNo);
            if (inInfo != null)
            {
                InPatientRegistInfo inPatient = new InPatientRegistInfo();
                inPatient.CardNO = inInfo.CARD_NO;
                inPatient.PatientNO = inInfo.PATIENT_NO;
                inPatient.Name = inInfo.NAME;
                inPatient.IDCard = inInfo.IDENNO;
                inPatient.Sex = inInfo.SEX_CODE;
                inPatient.Nationality = inInfo.NATION_CODE;
                inPatient.Birthday = ((DateTime)inInfo.BIRTHDAY).ToString("yyyy-MM-dd hh:mm:ss");
                inPatient.CompanyName = inInfo.WORK_NAME;
                inPatient.MaritalStatus = inInfo.MARI;
                inPatient.DIST = inInfo.DIST;
                inPatient.AreaCode = inInfo.BIRTH_AREA;
                inPatient.Country = inInfo.COUN_CODE;
                inPatient.Profession = inInfo.PROF_CODE;
                inPatient.HomeZip = inInfo.HOME_ZIP;
                inPatient.PhoneHome = inInfo.HOME_TEL;
                inPatient.PhoneBusiness = inInfo.WORK_TEL;
                return inPatient;
            }
            return null;
        }

        /// <summary>
        /// 插入患者登记信息
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        public int InPatientSave(InPatientRegistInfo Info, ref string Msg)
        {
            InPatientRegistLogic logic = new InPatientRegistLogic();
            return logic.InPatientSave(Info);
        }

        /// <summary>
        /// 预填写入院申请信息查询
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        public InPatientRegistInfo GetInPatientRegistInfo(string CardNO)
        {
            InPatientRegistLogic logic = new InPatientRegistLogic();
            return logic.GetInPatientRegistInfo(CardNO);
        }
        /// <summary>
        /// 住院登记
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        public int insertPatientInfo(InPatientRegistInfo Info, string cardType, string QRCode, ref string Msg)
        {
            Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
            Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
            Neusoft.HISFC.Models.Base.Employee employee = this.managerIntegrate.GetEmployeeInfo("00A105");
            Neusoft.FrameWork.Management.Connection.Operator = employee as Neusoft.FrameWork.Models.NeuObject;
            string tempPatientNo = "";
            //验证有效性,如果有不符合录入,那么中止方法
            //if (!this.IRegister.IsInputValid())
            //{
            //    PatientInfo patientTemp = this.IRegister.GetPatientInfo(patientInfo);
            //    if (patientTemp == null)
            //    {
            //        this.tempPatientNo = patientInfo.PID.PatientNO;
            //    }
            //    else
            //    {
            //        this.tempPatientNo = patientTemp.PID.PatientNO;
            //    }
            //    return -1;
            //}
            patientInfo = this.IRegister.GetPatientInfo(patientInfo, Info);
            if (patientInfo == null)
            {
                return -1;
            }
            tempPatientNo = patientInfo.PID.PatientNO;

            //如果还没有输入住院号,那么自动生成住院号
            if (string.IsNullOrEmpty(patientInfo.PID.PatientNO))
            {
                if (this.isAutoPatientNO)
                {
                    //如果自动生成住院号失败,那么中止方法
                    if (this.getAutoPatientNO(ref patientInfo, ref Msg) == -1)
                    {
                        return -1;
                    }
                }
                else
                {
                    Msg = "没有输入住院号!";
                    return -1;
                }
            }

            if (this.ISave.Saving(Neusoft.SOC.HISFC.BizProcess.CommonInterface.Common.EnumSaveType.Insert, patientInfo) == -1)
            {
                Msg = this.ISave.Err;
                return -1;
            }

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            this.radtIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.inpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.managerIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            string errorInfo = string.Empty;
            try
            {
                Neusoft.HISFC.Models.RADT.PatientInfo patient = new Neusoft.HISFC.Models.RADT.PatientInfo();

                if (this.radtIntegrate.GetInputPatientNO(patientInfo.PID.PatientNO, ref patient) == -1)
                {
                    //如果是自动获取住院号，则再重新获取，否则，报错！
                    if (patient != null && patient.PatientNOType == Neusoft.HISFC.Models.RADT.EnumPatientNOType.Second)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Msg = "此住院号正在使用或此患者正在治疗！";
                        return -1;
                    }
                    else if (this.isAutoPatientNO)
                    {
                        string patientNO = string.Empty;
                        bool isRecycle = false;
                        if (Neusoft.SOC.Local.RADT.ZhuHai.Function.GetAutoPatientNO(ref patientNO, ref isRecycle) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            Msg = "获得自动生成住院号出错!" + this.radtIntegrate.Err;
                            return -1;
                        }
                        patientInfo.PID.PatientNO = patientNO;
                        patientInfo.PID.CardNO = Neusoft.SOC.Local.RADT.ZhuHai.Function.GetCardNOByPatientNO(patientInfo.PID.CardNO, patientNO);
                    }
                }


                //获取新的住院流水号：
                patientInfo.ID = this.radtIntegrate.GetNewInpatientNO();

                #region 更新预约入院主表fin_ipr_prepayin {B63ACC72-86CD-4c0d-81A4-218DBA1A7361}20191030
                this.managerIntegrate.UpdatePreInPatientNo(patientInfo);
                #endregion

                patientInfo.InTimes = Neusoft.SOC.Local.RADT.ZhuHai.Function.GetMaxIntimes(patientInfo.PID.PatientNO);

                if (patientInfo.InTimes == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Msg = "获取最大住院次数出错，请检查！";
                    return -1;
                }

                //插入住院主表
                if (this.radtIntegrate.RegisterPatient(patientInfo) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Msg = this.radtIntegrate.Err;
                    return -1;
                }

                //插入住院主表扩展表
                if (patientInfo.User01.Trim() != "")
                {
                    if (this.radtIntegrate.InsertInmaininfoExtend(patientInfo) == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Msg = this.radtIntegrate.Err;
                        return -1;
                    }
                }

                //如果取的是废号更新住院号标志
                if (this.radtIntegrate.UpdatePatientNOState(patientInfo.PID.PatientNO) < 0)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Msg = "更新住院号状态出错！";

                    return -1;
                }

                // 更新登记时候的血滞纳金和公费日限额和日限额累计and生育保险电脑号and日限额超标金额
                if (this.radtIntegrate.UpdateFeePatientInfoForRegister(patientInfo) < -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Msg = "更新公费信息出错！";
                    return -1;
                }


                //插入基本表
                Neusoft.SOC.HISFC.RADT.BizLogic.ComPatient patientMgr = new Neusoft.SOC.HISFC.RADT.BizLogic.ComPatient();
                patientMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                if (patientMgr.InsertPatient(patientInfo) < 0)
                {
                    this.isNewPatient = false;
                    //先查询
                    if (patientMgr.UpdatePatientForInpatient(patientInfo) <= 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Msg = "插入患者基本信息出错!" + patientMgr.Err;
                        return -1;
                    }
                }
                else
                {
                    this.isNewPatient = true;
                }

                //插入变更信息
                if (this.radtIntegrate.InsertShiftData(patientInfo) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Msg = "插入变更信息出错!";
                    return -1;
                }



                #region 生成默认警戒线

                if (isCreateMoneyAlert)
                {

                    Neusoft.HISFC.BizLogic.Manager.Constant conStant = new Neusoft.HISFC.BizLogic.Manager.Constant();
                    Neusoft.FrameWork.Models.NeuObject conStantObj = null;

                    conStantObj = conStant.GetConstant("MONEYALERT", patientInfo.Pact.ID);

                    if (string.IsNullOrEmpty(conStantObj.ID))
                    {
                        conStantObj = conStant.GetConstant("MONEYALERT", patientInfo.Pact.PayKind.ID);
                    }

                    if (conStantObj == null)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Msg = "默认警戒线没有维护，请先维护！";
                        return -1;
                    }
                    if (Neusoft.FrameWork.Public.String.IsNumeric(conStantObj.Memo))
                    {
                        patientInfo.PVisit.MoneyAlert = Neusoft.FrameWork.Function.NConvert.ToDecimal(conStantObj.Memo);
                    }
                    else
                    {

                        patientInfo.PVisit.MoneyAlert = 0m;

                    }
                    if (this.radtIntegrate.UpdatePatientAlert(patientInfo.ID, patientInfo.PVisit.MoneyAlert, Neusoft.HISFC.Models.Base.EnumAlertType.M.ToString(), DateTime.MinValue, DateTime.MinValue) <= 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Msg = "更新警戒线失败！";
                        return -1;
                    }
                }
                #endregion

                # region 如果包含接诊流程，更新床的使用状态

                if (this.isArriveProcess)
                {
                    Neusoft.HISFC.Models.Base.Bed bedObjTemp = patientInfo.PVisit.PatientLocation.Bed;
                    Neusoft.HISFC.Models.Base.Bed bedObj = bedObjTemp.Clone();
                    bedObj.Status.User03 = bedObjTemp.Status.ID.ToString();
                    bedObj.Status.ID = Neusoft.HISFC.Models.Base.EnumBedStatus.O;
                    bedObj.InpatientNO = patientInfo.ID;

                    if (managerIntegrate.SetBed(bedObj) == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Msg = "更新床位状态失败！";
                        return -1;
                    }

                    if (this.radtIntegrate.InsertRecievePatientShiftData(patientInfo) == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        Msg = "插入接诊变更信息出错!";

                        return -1;
                    }
                }
                #endregion

                #region 担保信息

                //插入担保信息
                if (this.radtIntegrate.InsertSurty(patientInfo) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Msg = "插入担保信息出错!" + this.radtIntegrate.Err;

                    return -1;
                }

                #endregion




                #region 医保接口


                long returnValue = 0;

                medcareInterfaceProxy.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                returnValue = medcareInterfaceProxy.SetPactCode(patientInfo.Pact.ID);
                medcareInterfaceProxy.Trans = Neusoft.FrameWork.Management.PublicTrans.Trans;
                if (returnValue != 1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    medcareInterfaceProxy.Rollback();
                    Msg = "待遇接口获得合同单位失败!" + medcareInterfaceProxy.ErrMsg;
                    return -1;
                }
                returnValue = medcareInterfaceProxy.Connect();
                if (returnValue != 1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    medcareInterfaceProxy.Rollback();
                    Msg = "待遇接口初始化失败" + medcareInterfaceProxy.ErrMsg;
                    return -1;
                }

                patientInfo.SIQueryType = cardType;
                patientInfo.EcVoucher = QRCode;
                returnValue = medcareInterfaceProxy.UploadRegInfoInpatient(patientInfo);
                if (returnValue != 1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    medcareInterfaceProxy.Rollback();
                    Msg = "待遇接口住院登记失败" + medcareInterfaceProxy.ErrMsg;
                    return -1;
                }

                #region 增加判断住院号是否重复功能

                int iCount = this.radtInpatient.GetPatientCountByPatientNo(patientInfo.PID.PatientNO);
                if (iCount < 0)
                {
                    medcareInterfaceProxy.Rollback();
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();

                    Msg = "判断住院号是否重复失败" + this.radtInpatient.Err;
                    return -1;
                }
                if (iCount > 1)
                {
                    medcareInterfaceProxy.Rollback();
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Msg = "[" + patientInfo.PID.PatientNO + "]住院号有重复，请删除住院号内容重新保存！";
                    return -1;
                }

                #endregion

                medcareInterfaceProxy.Commit();
                returnValue = medcareInterfaceProxy.Disconnect();

                #endregion

                if (this.ISave.SaveCommitting(Neusoft.SOC.HISFC.BizProcess.CommonInterface.Common.EnumSaveType.Insert, patientInfo) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    Msg = this.ISave.Err;
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                Msg = "住院登记失败！" + ex.Message;
                return -1;
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            if (this.ISave.Saved(Neusoft.SOC.HISFC.BizProcess.CommonInterface.Common.EnumSaveType.Insert, patientInfo) == -1)
            {
                Msg = this.ISave.Err;
            }

            Msg = "登记成功!住院号是：" + patientInfo.PID.PatientNO;


            //添加广州医保匹配
            if (Neusoft.SOC.Local.RADT.ZhuHai.Function.GetRegInfoInpatient(patientInfo, ref errorInfo) <= 0)
            {
                Msg = "匹配医保信息失败，" + errorInfo;
            }


            if (isUploadEMPI && this.isNewPatient)
            {
                PatEMPIOperate(patientInfo);  //EMPI操作
            }

            return 1;
        }

        private int getAutoPatientNO(ref Neusoft.HISFC.Models.RADT.PatientInfo patient, ref string err)
        {
            patient.PatientNOType = Neusoft.HISFC.Models.RADT.EnumPatientNOType.First;
            string patientNO = string.Empty;
            bool isRecycle = false;
            if (GetAutoPatientNO(ref patientNO, ref isRecycle, ref err) == -1)
            {
                err = "获得自动生成住院号出错!" + this.radtIntegrate.Err;
                return -1;
            }
            //默认第一次入院
            patient.PID.PatientNO = patientNO;
            patient.ID = "T001";
            patient.InTimes = 1;

            patient.PID.CardNO = Neusoft.SOC.Local.RADT.ZhuHai.Function.GetCardNOByPatientNO(patient.PID.CardNO, patientNO);
            patient.InTimes = 1;//如果第一次输入则赋值为1
            return 1;
        }

        /// <summary>
        /// 自动获取住院号方法
        /// </summary>
        /// <param name="patientNO"></param>
        /// <param name="isRecycle"></param>
        /// <returns></returns>
        public int GetAutoPatientNO(ref string patientNO, ref bool isRecycle, ref string err)
        {
            Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();
            try
            {
                return radtIntegrate.GetAutoPatientNO(ref patientNO, ref isRecycle);
            }
            finally
            {
                err = radtIntegrate.Err;
            }
        }

        #region =======EMPI 数据操作=========
        //add by allan EMPI 数据操作 2016-07-27
        private void PatEMPIOperate(Neusoft.HISFC.Models.RADT.PatientInfo regObj)
        {
            if (regObj.IDCard.Length == 18 || regObj.IDCard.Length == 15)
            {
                Nesoft.EMPI.EMPI.PATIENTINFO pInfo = new Nesoft.EMPI.EMPI.PATIENTINFO(); //病人信息
                Nesoft.EMPI.EMPI.PATIENT pat = new Nesoft.EMPI.EMPI.PATIENT();
                pat.NAME = regObj.Name;                                 //姓名
                pat.IDNO = regObj.IDCard;                                 //身份证号
                pat.BIRTHDAY = regObj.Birthday.ToString("yyyy-MM-dd");//regObj.Birthday.ToString("yyyyMMdd");    //出生日期
                pat.SEX = regObj.Sex.Name;                              //性别
                pat.CNY = regObj.Country.ID;                            //国家代码
                pat.CNYNAME = regObj.Country.Name;                      //国家名称
                pat.ACT = "";                                           //户籍代码
                pat.ADDR = regObj.AddressHome;                          //家庭住址
                pat.ZPCODE = regObj.HomeZip;                            //邮政编码
                pat.ABOBLD = regObj.BloodType.Name;                     //血型
                pat.RHBLD = regObj.BloodType.RH == true ? "是" : "否";  //RH   
                pat.NTN = regObj.Nationality.ID;        //民族
                pat.BCP = regObj.AreaCode;
                pat.CTOR = regObj.Kin.Name;
                pat.CTORTEL = regObj.Kin.RelationPhone;
                pat.CTORLTN = regObj.Kin.RelationLink;
                pat.HMTEL = regObj.PhoneHome;
                pat.MOBILE = regObj.PhoneHome;                              //手机号码
                pat.EML = regObj.Email;
                pat.CPY = regObj.CompanyName;
                pat.CPYTEL = regObj.PhoneBusiness;
                pat.MRG = regObj.MaritalStatus.Name;
                pat.PFSN = regObj.Profession.ID;     //职业代码
                pat.MEMO = regObj.Memo;
                Nesoft.EMPI.EMPI.CARD card = new Nesoft.EMPI.EMPI.CARD();
                card.CARDNO = regObj.PID.PatientNO;
                card.CARDTYPE = "I"; //门诊都是O 住院是I
                card.OPERCODE = "";
                card.OPERNAME = "";
                pInfo.PATIENT = pat;
                pInfo.CARDINFOS = new List<Nesoft.EMPI.EMPI.CARD> { card };
                pInfo.DOMAIN = "001";
                Nesoft.EMPI.EMPIOperate op = new Nesoft.EMPI.EMPIOperate();
                string iReturn = op.EmpiReg(pInfo);
            }
        }
        #endregion

        public List<DicObject> GetDictionary(string DType)
        {
            switch (DType)
            {
                case "民族":
                    return IRegister.QueryConstant(Neusoft.HISFC.Models.Base.EnumConstant.NATION);
                case "科室":
                    return IRegister.GetDept();
                case "婚姻状况":
                    return IRegister.GetMaritalStatusList();
                case "籍贯":
                    return IRegister.QueryConstant(Neusoft.HISFC.Models.Base.EnumConstant.DIST);
                case "国籍":
                    return IRegister.QueryConstant(Neusoft.HISFC.Models.Base.EnumConstant.COUNTRY);
                case "职位":
                    return IRegister.QueryConstant(Neusoft.HISFC.Models.Base.EnumConstant.PROFESSION);
                case "与患者关系":
                    return IRegister.QueryConstant(Neusoft.HISFC.Models.Base.EnumConstant.RELATIVE);
                case "入院途径":
                    return IRegister.QueryConstant(Neusoft.HISFC.Models.Base.EnumConstant.INAVENUE);
                case "入院来源":
                    return IRegister.QueryConstant(Neusoft.HISFC.Models.Base.EnumConstant.INSOURCE);
                case "入院情况":
                    return IRegister.QueryConstant(Neusoft.HISFC.Models.Base.EnumConstant.INCIRCS);
                case "收住医师":
                    return IRegister.GetDoct();
                case "性别":
                    return IRegister.GetSex();
                case "合同单位":
                    return IRegister.GetPact();
                case "日间手术标记":
                    return IRegister.GetDayOperFlagList();
                case "诊断编码":
                    return IRegister.GetICD();
                case "地址":
                    return IRegister.GetAddr();
                default:
                    return null;
            }

        }
    }
}
