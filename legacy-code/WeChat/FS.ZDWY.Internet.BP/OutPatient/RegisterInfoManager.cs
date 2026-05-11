using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SqlSugar;
using System.Collections;
using Neusoft.FrameWork.Function;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.HISFC.Models.Base;
using FS.ZDWY.Internet.Models;
using GDSI.CountryMedical.Common;

namespace FS.ZDWY.Internet.BP.OutPatient
{
    public class RegisterInfoManager
    {
        NeusoftBussiness neusoftBussiness = new NeusoftBussiness();

        LogHelper.ServiceLog serviceLogManager;
        /// <summary>
        /// 服务日志管理
        /// </summary>
        LogHelper.ServiceLog ServiceLogManager
        {
            get
            {
                if (serviceLogManager == null)
                {
                    serviceLogManager = new LogHelper.ServiceLog();
                }
                return serviceLogManager;
            }
        }
        public Models.Views.ComResult<Models.Views.OrderResult> Order(Models.PLATFORM_REGISTER_ORDER order, Models.OperInfo oper)
        {
            /*
             * 1.判断排班信息是否一致，是否有效
             * 2.判断诊金是否正确
             * 3.判断患者信息是否存在
            */
            Models.Views.ComResult<Models.Views.OrderResult> result = new Models.Views.ComResult<Models.Views.OrderResult>();
            BL.OutPatient.PlatformOrderLogic plaLogic = new BL.OutPatient.PlatformOrderLogic();
            try
            {
                order.STATUS = "1";//待支付

                #region 记录数据

                plaLogic.BeginTran();
                FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();

                #region 判断是否优惠
                try
                {
                    //是否职工
                    string ecosql = @"
                            select 
                            count(1)
                            from com_employee  empl
                            where empl.idenno='{0}'
                            and empl.empl_name='{1}'
                            and (empl.valid_state='1'
                            or empl.empstate in ('10','05','19'))
                            ";
                    ecosql = string.Format(ecosql, order.CERTIFCATENO, order.NAME);
                    int ecores = Neusoft.FrameWork.Function.NConvert.ToInt32(mgr.ExecSqlReturnOne(ecosql));
                    if (ecores > 0)
                    {
                        order.ISECOST = "1";
                    }
                    if (order.ISECOST != "1")
                    {
                        //是否节假日
                        ecosql = @"
                            select  count(1)
                            from com_dictionary dic
                            where dic.type='EcoDate'
                            and to_date(dic.code,'yyyy-mm-dd')=trunc(to_date('{0}','yyyy-mm-dd hh24:mi:ss'))";
                        ecosql = string.Format(ecosql, order.SCHEDULEDATE.ToString());
                        ecores = Neusoft.FrameWork.Function.NConvert.ToInt32(mgr.ExecSqlReturnOne(ecosql));
                        if (ecores > 0)
                        {
                            order.ISECOST = "1";
                        }
                    }
                }
                catch
                {
                    order.ISECOST = "0";
                }

                #endregion

                FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic platformOrderLogic = new BL.OutPatient.PlatformOrderLogic();
                FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER oldorder = platformOrderLogic.Get(order.ORDERID);

                List<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER> orderlist = platformOrderLogic.GetList(obj => obj.SCHEDULEID == order.SCHEDULEID
                    && obj.PATIENTID == order.PATIENTID
                    && (obj.STATUS == "1" || obj.STATUS == "2"));
                if (orderlist != null && orderlist.Count > 0)
                {
                    throw new Exception("该时段已挂号！");
                }
                orderlist = platformOrderLogic.GetList(obj => (obj.DOCTORCODE == order.DOCTORCODE && obj.DOCTORCODE != "None" ||
                (obj.DOCTORCODE == "None" && obj.DEPTCODE == order.DEPTCODE))
                    && obj.PATIENTID == order.PATIENTID
                    && obj.SCHEDULEDATE.Value.Date == order.SCHEDULEDATE.Value.Date
                    && (obj.STATUS == "1" || obj.STATUS == "2"));
                if (orderlist != null && orderlist.Count > 0)
                {
                    throw new Exception("您已预约当天同一医生，无需重复挂号。");
                }
                if (oldorder != null)
                {
                    if (!string.IsNullOrEmpty(oldorder.STATUS) && oldorder.STATUS == "1")
                    {
                        throw new Exception("患者已经预约挂号成功！预约流水号为：" + oldorder.CLINIC_CODE);
                    }
                    else if (!string.IsNullOrEmpty(oldorder.STATUS) && !string.IsNullOrEmpty(oldorder.REGISTERID))
                    {
                        throw new Exception("患者已经预约挂号成功！预约流水号为：" + oldorder.CLINIC_CODE + ",并且已取号");
                    }
                    else if (!string.IsNullOrEmpty(oldorder.STATUS) && oldorder.STATUS == "3")
                    {
                        throw new Exception("患者已经预约挂号已作废");
                    }
                    else if (!string.IsNullOrEmpty(oldorder.STATUS) && oldorder.STATUS == "4")
                    {
                        throw new Exception("患者已经预约挂号已退费");
                    }
                }


                if (!plaLogic.Update(order))
                {
                    if (!plaLogic.Insert(order))
                    {
                        throw new Exception("记录数据失败！");
                    }
                }
                #endregion

                #region 1.核对排班信息
                BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
                oper.Time = schedulingLogic.GetDateTime();
                Models.FIN_OPR_SCHEMA schema = schedulingLogic.Get(order.SCHEDULEID);
                if (schema == null || string.IsNullOrEmpty(schema.ID))
                {
                    throw new Exception("排班信息不存在，请核对");
                }
                if (schema.VALID_FLAG == "0")
                {
                    throw new Exception("该排班信息已经停诊，请核对");
                }
                //if (schema.DEPT_CODE != order.DEPTCODE)
                //{
                //    throw new Exception("排班信息不正确【科室信息】，请核对");
                //}
                if (schema.DOCT_CODE != order.DOCTORCODE)
                {
                    throw new Exception("排班信息不正确【医生信息】，请核对");
                }
                if (schema.SEE_DATE.Date != SqlFunc.ToDate(order.SCHEDULEDATE).Date)
                {
                    throw new Exception("排班信息不正确【看诊日期】，请核对");
                }
                if (order.REGTYPE == "1")  //当日挂号
                {
                    if (schema.SEE_DATE.Date != oper.Time.Date)
                    {
                        throw new Exception("该号源不是当日号源，请核对。【REGTYPE】入参是否正确？");
                    }
                    if (schema.REG_LMT - schema.REGED <= 0)
                    {
                        throw new Exception("该号源已经用完，请选择其他号源");
                    }
                }
                else if (order.REGTYPE == "0")  //预约挂号
                {
                    if (schema.SEE_DATE.Date <= oper.Time.Date)
                    {
                        throw new Exception("该号源是当日号源，请核对。【REGTYPE】入参是否正确?");
                    }
                    if (schema.TEL_LMT - schema.TEL_REGED <= 0)
                    {
                        throw new Exception("该号源已经用完，请选择其他号源");
                    }
                }
                #endregion

                #region 2.核对患者信息
                //patient.CARD_NO = order.PATIENTID;
                //patient.NAME = order.NAME;
                //patient.SEX_CODE = ConvertHISSexCode(order.SEX);
                //patient.BIRTHDAY = SqlFunc.ToDate(order.BIRTH);
                //patient.HOME_NOW = order.ADDRESS;
                //patient.HOME_TEL = order.MOBILE;
                //patient.IDCARDTYPE = order.CERTIFCATETYPE;
                //patient.IDENNO = order.CERTIFCATENO;
                BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();
                Models.COM_PATIENTINFO patient = patientInfoLogic.Get(order.CARDNO);
                if (patient == null || string.IsNullOrEmpty(patient.CARD_NO))
                {
                    throw new Exception("患者信息不存在，请核对");
                }
                if (patient.NAME != order.NAME)
                {
                    throw new Exception("患者信息不正确【姓名】，请核对");
                }
                //if (patient.SEX_CODE != ConvertHISSexCode(order.SEX))
                //{
                //    throw new Exception("患者信息不正确【性别】，请核对");
                //}
                if (patient.BIRTHDAY.Date != SqlFunc.ToDate(order.BIRTH.Date))
                {
                    throw new Exception("患者信息不正确【出生年月日】，请核对");
                }

                patient.PACT_CODE = "1";
                patient.PAYKIND_CODE = "01";
                #endregion

                #region 3.核对诊金信息
                //根据排班和患者的结算类别获取对应的诊金
                BL.OutPatient.RegFeeOnPactLogic regFeeOnPactLogic = new BL.OutPatient.RegFeeOnPactLogic();
                Models.FIN_OPR_REGFEEONPACT regfeeonpact = regFeeOnPactLogic.GetSingle(w => w.REGLEVL_CODE == schema.REGLEVL_CODE && w.PACT_CODE == patient.PACT_CODE);  //Dept_Code这里估计会有问题
                if (regfeeonpact == null || string.IsNullOrEmpty(regfeeonpact.ID))
                {
                    throw new Exception("获取排版对应的诊查费失败");
                }
                decimal consultationFee = SqlFunc.ToDecimal(order.REGFEE) / 100m;  //接口所有关于钱的单位都是分，所以要转换
                if (regfeeonpact.DIAG_FEE != consultationFee)
                {
                    throw new Exception(string.Format("诊金校对失败，传入的诊金是{0}，而HIS获取出诊信息对应的诊金是{1}", consultationFee, regfeeonpact.DIAG_FEE));
                }
                #endregion

                #region 4.整合预约信息
                Models.FIN_OPR_BOOKING booking = new Models.FIN_OPR_BOOKING();
                booking.ADDRESS = order.ADDRESS;
                booking.APP_FLAG = "0";
                //booking.APP_SENDFLAG = "1";  //这个暂时不知道用来是干什么的
                booking.BEGIN_TIME = SqlFunc.ToDate(order.BEGINTIME);
                booking.BIRTHDAY = order.BIRTH;
                booking.BOOKING_DATE = SqlFunc.ToDate(order.ORDERTIME);
                booking.CARD_NO = order.CARDNO;
                //booking.CLINIC_CODE = order.CLINIC_CODE;  //这个时候还没有流水号
                //booking.CONFIRM_DATE  //此时也还没有确认时间
                //booking.CONFIRM_OPCD  //此时也还没有确认操作人
                booking.DEPT_CODE = order.DEPTCODE;
                booking.DEPT_NAME = schema.DEPT_NAME;
                booking.DOCT_CODE = schema.DOCT_CODE;
                booking.DOCT_NAME = schema.DOCT_NAME;
                booking.END_TIME = SqlFunc.ToDate(order.ENDTIME);
                booking.IDENNO = patient.IDENNO;
                booking.NAME = patient.NAME;
                booking.NOON_CODE = schema.NOON_CODE;
                booking.OPER_CODE = oper.Code;
                booking.OPER_DATE = oper.Time;
                booking.REGLEVL_CODE = schema.REGLEVL_CODE;
                //booking.REG_ID  //此时还没有挂号流水号
                booking.RELA_PHONE = order.MOBILE;
                booking.SCHEMA_NO = schema.ID;
                booking.SEE_FLAG = "0";
                booking.SEX_CODE = patient.SEX_CODE;
                booking.SOURCE = order.FRONTPROVIDERID;
                booking.VALID_FLAG = "1";
                #endregion

                #region 5.整合挂号信息
                /*
                Models.FIN_OPR_REGISTER register = new Models.FIN_OPR_REGISTER();
                register.CARD_NO = patient.CARD_NO;
                register.NAME = patient.NAME;
                register.SEX_CODE = patient.SEX_CODE;
                register.BIRTHDAY = patient.BIRTHDAY;
                register.ADDRESS = patient.HOME_NOW;
                register.RELA_PHONE = patient.HOME_TEL;
                register.CARD_TYPE = patient.IDCARDTYPE;
                register.IDENNO = patient.IDENNO;
                register.REG_DATE = schema.SEE_DATE;
                register.VALID_FLAG = "1";
                register.TRANS_TYPE = "1";
                register.PACT_CODE = patient.PACT_CODE;
                register.PAYKIND_CODE = patient.PAYKIND_CODE;
                register.REGLEVL_CODE = schema.REGLEVL_CODE;
                register.REGLEVL_NAME = schema.REGLEVL_NAME;
                register.DEPT_CODE = schema.DEPT_CODE;
                register.DEPT_NAME = schema.DEPT_NAME;
                register.SCHEMA_NO = schema.ID;
                //register.ORDER_NO    //每日顺序号 //todo:
                //register.SEENO  看诊序号
                register.BEGIN_TIME = schema.BEGIN_TIME;
                register.END_TIME = schema.END_TIME;
                register.DOCT_CODE = schema.DOCT_CODE;
                register.DOCT_NAME = schema.DOCT_NAME;
                */
                Neusoft.HISFC.Models.Registration.Register register = new Neusoft.HISFC.Models.Registration.Register();
                register.PID.CardNO = patient.CARD_NO;
                register.Name = patient.NAME;
                register.IDCard = patient.IDENNO;
                register.IDCardType.ID = patient.IDCARDTYPE;
                register.Sex.ID = patient.SEX_CODE;
                register.Pact.ID = patient.PACT_CODE;
                register.PhoneHome = patient.HOME_TEL;
                register.AddressHome = patient.HOME_NOW;
                register.Birthday = patient.BIRTHDAY;
                register.DoctorInfo.SeeDate = schema.BEGIN_TIME;    //预约挂号时间为预约开始时间
                register.RegType = Neusoft.HISFC.Models.Base.EnumRegType.Pre;//为预约挂号
                register.RegLvlFee.RegLevel.ID = schema.REGLEVL_CODE;
                register.RegLvlFee.RegLevel.Name = schema.REGLEVL_NAME;
                register.DoctorInfo.Templet.RegLevel.ID = schema.REGLEVL_CODE;
                register.DoctorInfo.Templet.RegLevel.Name = schema.REGLEVL_NAME;
                register.DoctorInfo.Templet.ID = schema.ID;
                register.DoctorInfo.Templet.Noon.ID = schema.NOON_CODE;
                register.DoctorInfo.Templet.Begin = schema.BEGIN_TIME;
                register.DoctorInfo.Templet.End = schema.END_TIME;
                register.DoctorInfo.Templet.Dept.ID = schema.DEPT_CODE;
                register.DoctorInfo.Templet.Dept.Name = schema.DEPT_NAME;
                register.DoctorInfo.Templet.Doct.ID = schema.DOCT_CODE;
                register.DoctorInfo.Templet.Doct.Name = schema.DOCT_NAME;
                //设置排班对应的挂号费               
                register.RegLvlFee.RegFee = regfeeonpact.REG_FEE;
                register.RegLvlFee.ChkFee = regfeeonpact.CHCK_FEE;
                register.RegLvlFee.OwnDigFee = regfeeonpact.DIAG_FEE;
                //if (isBuyBL)
                //{
                //    register.RegLvlFee.OthFee = regLvFee.OthFee;
                //}
                //else
                {  //这里不购买病历本
                    register.RegLvlFee.OthFee = 0;
                }
                string errMsg = string.Empty;
                ////计算费用  //医保费用计算
                //var returnValue = neusoftBussiness.ComputeRegCost(ref register, ref errMsg);
                //if (returnValue <= 0)
                //{
                //    errMsg = "计算费用失败！" + errMsg;
                //    throw new Exception(errMsg);
                //}
                /*
                totCost = (register.OwnCost + register.PubCost + register.PayCost) * 100;  //单位：分
                pubCost = register.PubCost * 100;//单位：分
                ownCost = (register.OwnCost - register.EcoCost) * 100;  //单位：分
                ecoCost = register.EcoCost * 100;  //单位：分
                */
                #endregion

                #region 挂号限制
                //1.14岁以下不能挂急诊内科
                if (patient.BIRTHDAY.Date.AddYears(14) > mgr.GetDateTimeFromSysDateTime())
                {
                    if (mgr.GetAge14LimitDept(order.DEPTCODE) != "-1")
                    {
                        throw new Exception("14周岁以下不能挂内科！");
                    }
                }
                else
                {
                    if (mgr.GetPediatricsDept(order.DEPTCODE) != "-1")
                    {
                        throw new Exception("14周岁以上不能挂儿科！");
                    }
                    //if (order.DEPTCODE == "6002")
                    //{
                    //    throw new Exception("14周岁以上不能挂儿科！");
                    //}
                    //if (order.DEPTCODE == "6012")
                    //{
                    //    throw new Exception("14周岁以上不能挂急诊儿科！");
                    //}
                    //if (order.DEPTCODE == "6049")
                    //{
                    //    throw new Exception("14周岁以上不能挂儿童保健科！");
                    //}
                    //if (order.DEPTCODE == "6181")
                    //{
                    //    throw new Exception("14周岁以上不能挂儿童发热门诊！");
                    //}
                    if (order.DEPTCODE == "6181")
                    {
                        throw new Exception("14周岁以上不能挂儿童发热门诊！");
                    }
                }

                //男性
                if (patient.SEX_CODE == "M")
                {
                    if (order.DEPTCODE == "6070")
                    {
                        throw new Exception("男性不能挂该科室！");
                    }
                }
                //女性
                //else if (patient.SEX_CODE == "F")
                //{
                //    if (order.DEPTCODE == "9254")
                //    {
                //        throw new Exception("女性不能挂该科室！");
                //    }
                //}

                #endregion

                #region 6.占用号源
                string bookingID = string.Empty;
                string visitNo = string.Empty;
                if (order.REGTYPE == "0")
                {
                    //判断支付方式，1在线支付 0到院支付， 到院支付直接锁号就行
                    if (order.PAYMETHOD == "0")
                    {
                        if (neusoftBussiness.LockPreRegInfo(order.REGTYPE, schema, booking, oper, ref bookingID, ref visitNo, ref errMsg) <= 0)
                        {
                            throw new Exception(errMsg);
                        }
                    }
                    else if (order.PAYMETHOD == "1")
                    {
                        if (neusoftBussiness.LockPreRegInfo(order.REGTYPE, schema, booking, oper, ref bookingID, ref visitNo, ref errMsg) <= 0)
                        {
                            throw new Exception(errMsg);
                        }
                    }
                    else
                    {
                        throw new Exception("支付方式错误");
                    }
                }
                else
                {
                    if (neusoftBussiness.LockPreRegInfo(order.REGTYPE, schema, booking, oper, ref bookingID, ref visitNo, ref errMsg) <= 0)
                    {
                        throw new Exception(errMsg);
                    }
                }
                #endregion

                order.CLINIC_CODE = bookingID;
                if (!plaLogic.Update(order))
                {
                    throw new Exception("记录数据失败！");
                }

                #region 组织返回结果

                //是否职工
                string visitAddressSql = @"select (select n.remark||n.room_name from MET_NUO_CONSOLE n where n.console_code=m.console_code and rownum=1) 
from fin_opr_schema m
where m.id='{0}'";
                visitAddressSql = string.Format(visitAddressSql, schema.ID);
                string visitAddress = mgr.ExecSqlReturnOne(visitAddressSql, "");

                Models.Views.OrderResult res = new Models.Views.OrderResult();
                res.OrderId = order.ORDERID;
                res.HospitalNum = bookingID;
                res.Proof = bookingID;
                res.VisitNo = visitNo;  //就诊序号
                res.VisitAddress = visitAddress;// string.Empty;  //就诊位置

                result.IsSuccessful = true;
                result.Message = "";
                result.ReturnData = res;

                plaLogic.CommitTran();

                return result;
                #endregion

            }
            catch (Exception ex)
            {
                plaLogic.RollbackTran();
                result.IsSuccessful = false;
                result.Message = ex.Message;
                return result;
            }
            //result.IsSuccessful = true;
            //return result;
        }


        public Models.Views.ComResult<Models.Views.OrderResult> AddRegister(Models.PLATFORM_REGISTER_ORDER order, Models.OperInfo oper, string noonCode, string regLevel)
        {
            /*
             * 1.查询排班
             * 2.查询患者信息
             * 3.加号源
             * 4.锁号
            */
            Models.Views.ComResult<Models.Views.OrderResult> result = new Models.Views.ComResult<Models.Views.OrderResult>();
            BL.OutPatient.PlatformOrderLogic plaLogic = new BL.OutPatient.PlatformOrderLogic();
            BL.OutPatient.BookingLogic bookingLgc = new BL.OutPatient.BookingLogic();
            BL.RegisterInfoLogic regLogic = new BL.RegisterInfoLogic();
            int addRes = 0;
            string schemaId = "";
            try
            {
                order.STATUS = "1";//待支付

                #region 记录数据

                //plaLogic.BeginTran();
                FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();

                #region 判断是否优惠
                try
                {
                    //是否职工
                    string ecosql = @"
                            select 
                            count(1)
                            from com_employee  empl
                            where empl.idenno='{0}'
                            and empl.empl_name='{1}'
                            and (empl.valid_state='1'
                            or empl.empstate in ('10','05','19'))";
                    ecosql = string.Format(ecosql, order.CERTIFCATENO, order.NAME);
                    int ecores = Neusoft.FrameWork.Function.NConvert.ToInt32(mgr.ExecSqlReturnOne(ecosql));
                    if (ecores > 0)
                    {
                        order.ISECOST = "1";
                    }
                    if (order.ISECOST != "1")
                    {
                        //是否节假日
                        ecosql = @"
                            select  count(1)
                            from com_dictionary dic
                            where dic.type='EcoDate'
                            and to_date(dic.code,'yyyy-mm-dd')=trunc(to_date('{0}','yyyy-mm-dd hh24:mi:ss'))";
                        ecosql = string.Format(ecosql, order.SCHEDULEDATE.ToString());
                        ecores = Neusoft.FrameWork.Function.NConvert.ToInt32(mgr.ExecSqlReturnOne(ecosql));
                        if (ecores > 0)
                        {
                            order.ISECOST = "1";
                        }
                    }
                }
                catch
                {
                    order.ISECOST = "0";
                }

                #endregion

                FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic platformOrderLogic = new BL.OutPatient.PlatformOrderLogic();
                FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER oldorder = platformOrderLogic.Get(order.ORDERID);

                List<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER> orderlist = platformOrderLogic.GetList(obj => obj.SCHEDULEID == order.SCHEDULEID
                    && obj.PATIENTID == order.PATIENTID
                    && (obj.STATUS == "1" || obj.STATUS == "2"));
                if (orderlist != null && orderlist.Count > 0)
                {
                    throw new Exception("该时段已挂号！");
                }
                orderlist = platformOrderLogic.GetList(obj => obj.DOCTORCODE == order.DOCTORCODE
                    && obj.PATIENTID == order.PATIENTID
                    && obj.SCHEDULEDATE.Value.Date == order.SCHEDULEDATE.Value.Date
                    && (obj.STATUS == "1" || obj.STATUS == "2"));
                if (orderlist != null && orderlist.Count > 0)
                {
                    throw new Exception("您已预约当天同一医生，无需重复挂号。");
                }
                if (oldorder != null)
                {
                    if (!string.IsNullOrEmpty(oldorder.STATUS) && oldorder.STATUS == "1")
                    {
                        throw new Exception("患者已经加号成功！取号流水号为：" + oldorder.CLINIC_CODE);
                    }
                    else if (!string.IsNullOrEmpty(oldorder.STATUS) && !string.IsNullOrEmpty(oldorder.REGISTERID))
                    {
                        throw new Exception("患者已经加号成功！取号流水号为：" + oldorder.CLINIC_CODE + ",并且已取号");
                    }
                    else if (!string.IsNullOrEmpty(oldorder.STATUS) && oldorder.STATUS == "3")
                    {
                        throw new Exception("患者已经加过号并已作废");
                    }
                    else if (!string.IsNullOrEmpty(oldorder.STATUS) && oldorder.STATUS == "4")
                    {
                        throw new Exception("患者已经取号并已退费");
                    }
                }


                #endregion

                #region 1.核对排班信息
                //TODO:查询排班
                BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
                oper.Time = schedulingLogic.GetDateTime();
                DateTime dtBegin = order.SCHEDULEDATE.Value.Date;
                DateTime dtEnd = order.SCHEDULEDATE.Value.Date;
                if (noonCode == "1")
                {
                    dtBegin = order.SCHEDULEDATE.Value.Date;
                    dtEnd = order.SCHEDULEDATE.Value.Date.AddHours(12);
                }
                else
                {
                    dtBegin = order.SCHEDULEDATE.Value.Date.AddHours(12);
                    dtEnd = order.SCHEDULEDATE.Value.AddDays(1);
                }
                //查询医生当天的排班记录
                DataTable dtSchedul = schedulingLogic.QuerySchedule("1", dtBegin, noonCode, order.DEPTCODE, order.DOCTORCODE);
                if (dtSchedul == null || dtSchedul.Rows.Count == 0)
                {
                    //查询科室排班
                    dtSchedul = schedulingLogic.QuerySchedule("0", dtBegin, noonCode, order.DEPTCODE, order.DOCTORCODE);
                    if (dtSchedul == null || dtSchedul.Rows.Count == 0)
                    {
                        throw new Exception("指定的时段没有有效排班，不允许加号");
                    }
                }
                DataRow rowSchema = dtSchedul.Rows[dtSchedul.Rows.Count - 1];
                schemaId = rowSchema["scheduleId"].ToString();//排班ID

                Models.FIN_OPR_SCHEMA schema = schedulingLogic.Get(schemaId);
                if (schema == null || string.IsNullOrEmpty(schema.ID))
                {
                    throw new Exception("获取排班信息出错，请核对");
                }
                schema.APPEND_FLAG = "1";//加号标志
                schema.REGLEVL_CODE = regLevel;
                #endregion

                #region 2.核对患者信息
                //TODO：查询患者信息
                //patient.CARD_NO = order.PATIENTID;
                //patient.NAME = order.NAME;
                //patient.SEX_CODE = ConvertHISSexCode(order.SEX);
                //patient.BIRTHDAY = SqlFunc.ToDate(order.BIRTH);
                //patient.HOME_NOW = order.ADDRESS;
                //patient.HOME_TEL = order.MOBILE;
                //patient.IDCARDTYPE = order.CERTIFCATETYPE;
                //patient.IDENNO = order.CERTIFCATENO;
                BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();
                Models.COM_PATIENTINFO patient = patientInfoLogic.Get(order.CARDNO);
                if (patient == null || string.IsNullOrEmpty(patient.CARD_NO))
                {
                    throw new Exception("患者信息不存在，请核对");
                }
                if (patient.NAME != order.NAME)
                {
                    throw new Exception("患者信息不正确【姓名】，请核对");
                }


                patient.PACT_CODE = "1";
                patient.PAYKIND_CODE = "01";
                #endregion
                //FS.ZDWY.Internet.Models.FIN_OPR_BOOKING book = bookingLgc.Get(order.CLINIC_CODE);
                order.SCHEDULEID = schemaId;
                order.NUMBERINFOID = noonCode;
                order.BEGINTIME = Convert.ToDateTime(schema.BEGIN_TIME).ToString("HH:mm");
                order.ENDTIME = Convert.ToDateTime(schema.END_TIME).ToString("HH:mm");

                //order.CARDTYPE = nodesVales["cardType"];
                //order.TYPE = nodesVales["type"];
                order.SEX = patient.SEX_CODE;
                //order.AGE = patient.age;
                order.BIRTH = patient.BIRTHDAY;
                //order.ADDRESS = patient.LINKMAN_ADD;
                //order.MOBILE = nodesVales["mobile"];
                //order.FRONTPROVIDERID = nodesVales["frontProviderId"];
                //order.CERTIFCATETYPE = nodesVales["certifcateType"];
                //order.CERTIFCATENO = nodesVales["certifcateNo"];
                //order.PATIENTID = nodesVales["patientId"];
                //order.GUARDNAME = nodesVales["guardName"];
                //order.GUARDIDTYPE = nodesVales["guardidType"];
                //order.GUARDIDNO = nodesVales["guardidNo"];


                if (!plaLogic.Update(order))
                {
                    if (!plaLogic.Insert(order))
                    {
                        throw new Exception("记录数据失败！");
                    }
                }
                #region 3.核对诊金信息
                //TODO:诊金赋值
                //根据排班和患者的结算类别获取对应的诊金
                BL.OutPatient.RegFeeOnPactLogic regFeeOnPactLogic = new BL.OutPatient.RegFeeOnPactLogic();
                //Models.FIN_OPR_REGFEEONPACT regfeeonpact = regFeeOnPactLogic.GetSingle(w => w.REGLEVL_CODE == rowSchema["level"].ToString() && w.PACT_CODE == patient.PACT_CODE);  //Dept_Code这里估计会有问题
                Models.FIN_OPR_REGFEEONPACT regfeeonpact = regFeeOnPactLogic.GetSingle(w => w.REGLEVL_CODE == regLevel && w.PACT_CODE == patient.PACT_CODE);
                if ((regfeeonpact == null || string.IsNullOrEmpty(regfeeonpact.ID)) && regLevel != "16")//珠澳特定人员为0元加号
                {
                    throw new Exception("获取排班对应的诊查费失败");
                }

                #endregion

                #region 4.整合预约信息
                Models.FIN_OPR_BOOKING booking = new Models.FIN_OPR_BOOKING();
                booking.ADDRESS = order.ADDRESS;
                booking.APP_FLAG = "1";
                //booking.APP_SENDFLAG = "1";  //这个暂时不知道用来是干什么的
                booking.BEGIN_TIME = SqlFunc.ToDate(order.BEGINTIME);
                booking.BIRTHDAY = patient.BIRTHDAY;
                booking.BOOKING_DATE = SqlFunc.ToDate(order.ORDERTIME);
                booking.CARD_NO = order.CARDNO;
                //booking.CLINIC_CODE = order.CLINIC_CODE;  //这个时候还没有流水号
                //booking.CONFIRM_DATE  //此时也还没有确认时间
                //booking.CONFIRM_OPCD  //此时也还没有确认操作人
                booking.DEPT_CODE = order.DEPTCODE;
                booking.DEPT_NAME = rowSchema["deptName"].ToString();
                booking.DOCT_CODE = order.DOCTORCODE;
                booking.DOCT_NAME = rowSchema["doctorName"].ToString();
                booking.END_TIME = SqlFunc.ToDate(order.ENDTIME);
                booking.IDENNO = patient.IDENNO;
                booking.NAME = patient.NAME;
                booking.NOON_CODE = noonCode;
                booking.OPER_CODE = oper.Code;
                booking.OPER_DATE = oper.Time;
                booking.REGLEVL_CODE = regLevel;// rowSchema["level"].ToString();
                //booking.REG_ID  //此时还没有挂号流水号
                booking.RELA_PHONE = order.MOBILE;
                booking.SCHEMA_NO = rowSchema["scheduleId"].ToString();
                booking.SEE_FLAG = "0";
                booking.SEX_CODE = patient.SEX_CODE;
                booking.SOURCE = order.FRONTPROVIDERID;
                booking.VALID_FLAG = "1";
                #endregion

                #region 5.整合挂号信息
                /*
                Models.FIN_OPR_REGISTER register = new Models.FIN_OPR_REGISTER();
                register.CARD_NO = patient.CARD_NO;
                register.NAME = patient.NAME;
                register.SEX_CODE = patient.SEX_CODE;
                register.BIRTHDAY = patient.BIRTHDAY;
                register.ADDRESS = patient.HOME_NOW;
                register.RELA_PHONE = patient.HOME_TEL;
                register.CARD_TYPE = patient.IDCARDTYPE;
                register.IDENNO = patient.IDENNO;
                register.REG_DATE = schema.SEE_DATE;
                register.VALID_FLAG = "1";
                register.TRANS_TYPE = "1";
                register.PACT_CODE = patient.PACT_CODE;
                register.PAYKIND_CODE = patient.PAYKIND_CODE;
                register.REGLEVL_CODE = schema.REGLEVL_CODE;
                register.REGLEVL_NAME = schema.REGLEVL_NAME;
                register.DEPT_CODE = schema.DEPT_CODE;
                register.DEPT_NAME = schema.DEPT_NAME;
                register.SCHEMA_NO = schema.ID;
                //register.ORDER_NO    //每日顺序号 //todo:
                //register.SEENO  看诊序号
                register.BEGIN_TIME = schema.BEGIN_TIME;
                register.END_TIME = schema.END_TIME;
                register.DOCT_CODE = schema.DOCT_CODE;
                register.DOCT_NAME = schema.DOCT_NAME;
                */
                //Neusoft.HISFC.Models.Registration.Register register = new Neusoft.HISFC.Models.Registration.Register();
                //register.PID.CardNO = patient.CARD_NO;
                //register.Name = patient.NAME;
                //register.IDCard = patient.IDENNO;
                //register.IDCardType.ID = patient.IDCARDTYPE;
                //register.Sex.ID = patient.SEX_CODE;
                //register.Pact.ID = patient.PACT_CODE;
                //register.PhoneHome = patient.HOME_TEL;
                //register.AddressHome = patient.HOME_NOW;
                //register.Birthday = patient.BIRTHDAY;
                //register.DoctorInfo.SeeDate = schema.BEGIN_TIME;    //预约挂号时间为预约开始时间
                //register.RegType = Neusoft.HISFC.Models.Base.EnumRegType.Pre;//为预约挂号
                //register.RegLvlFee.RegLevel.ID = schema.REGLEVL_CODE;
                //register.RegLvlFee.RegLevel.Name = schema.REGLEVL_NAME;
                //register.DoctorInfo.Templet.RegLevel.ID = schema.REGLEVL_CODE;
                //register.DoctorInfo.Templet.RegLevel.Name = schema.REGLEVL_NAME;
                //register.DoctorInfo.Templet.ID = schema.ID;
                //register.DoctorInfo.Templet.Noon.ID = schema.NOON_CODE;
                //register.DoctorInfo.Templet.Begin = schema.BEGIN_TIME;
                //register.DoctorInfo.Templet.End = schema.END_TIME;
                //register.DoctorInfo.Templet.Dept.ID = schema.DEPT_CODE;
                //register.DoctorInfo.Templet.Dept.Name = schema.DEPT_NAME;
                //register.DoctorInfo.Templet.Doct.ID = schema.DOCT_CODE;
                //register.DoctorInfo.Templet.Doct.Name = schema.DOCT_NAME;
                ////设置排班对应的挂号费               
                //register.RegLvlFee.RegFee = regfeeonpact.REG_FEE;
                //register.RegLvlFee.ChkFee = regfeeonpact.CHCK_FEE;
                //register.RegLvlFee.OwnDigFee = regfeeonpact.DIAG_FEE;
                //if (isBuyBL)
                //{
                //    register.RegLvlFee.OthFee = regLvFee.OthFee;
                //}
                //else
                //{  //这里不购买病历本
                //    register.RegLvlFee.OthFee = 0;
                //}
                string errMsg = string.Empty;
                ////计算费用  //医保费用计算
                //var returnValue = neusoftBussiness.ComputeRegCost(ref register, ref errMsg);
                //if (returnValue <= 0)
                //{
                //    errMsg = "计算费用失败！" + errMsg;
                //    throw new Exception(errMsg);
                //}
                /*
                totCost = (register.OwnCost + register.PubCost + register.PayCost) * 100;  //单位：分
                pubCost = register.PubCost * 100;//单位：分
                ownCost = (register.OwnCost - register.EcoCost) * 100;  //单位：分
                ecoCost = register.EcoCost * 100;  //单位：分
                */
                #endregion

                #region 挂号限制
                //1.14岁以下不能挂急诊内科
                if (patient.BIRTHDAY.Date.AddYears(14) > mgr.GetDateTimeFromSysDateTime())
                {
                    if (mgr.GetAge14LimitDept(order.DEPTCODE) != "-1")
                    {
                        throw new Exception("14周岁以下不能挂内科！");
                    }
                }
                else
                {
                    if (order.DEPTCODE == "6002")
                    {
                        throw new Exception("14周岁以上不能挂儿科！");
                    }
                    if (order.DEPTCODE == "6012")
                    {
                        throw new Exception("14周岁以上不能挂急诊儿科！");
                    }
                    if (order.DEPTCODE == "6049")
                    {
                        throw new Exception("14周岁以上不能挂儿童保健科！");
                    }
                }

                //男性
                if (patient.SEX_CODE == "M")
                {
                    if (order.DEPTCODE == "6070")
                    {
                        throw new Exception("男性不能挂该科室！");
                    }
                }
                //女性
                //else if (patient.SEX_CODE == "F")
                //{
                //    if (order.DEPTCODE == "9254")
                //    {
                //        throw new Exception("女性不能挂该科室！");
                //    }
                //}

                #endregion

                #region 珠澳特定人员门诊 直接插入挂号表不需要再支付
                if (regLevel == "16")//
                {
                    DateTime now = mgr.GetDateTimeFromSysDateTime();
                    string schemaSql = OutPatient.Register.Sql.GetSchema;
                    string compatientSql = OutPatient.Register.Sql.GetPatientInfo;
                    string regfeeSql = OutPatient.Register.Sql.GetRegFee;
                    string nurQueueSql1 = OutPatient.Register.Sql.GetNurQueueByDept;
                    string nurQueueSql2 = OutPatient.Register.Sql.GetNurQueueByDoct;
                    string invoicenoSql1 = OutPatient.Register.Sql.GetInvoiceInfoUsed;
                    string invoicenoSql2 = OutPatient.Register.Sql.GetInvoiceR;
                    string invoicenoSql3 = OutPatient.Register.Sql.GetInvoiceUserCode;
                    string seenoSql = OutPatient.Register.Sql.GetSeeNo;
                    string clinicCodeSql = OutPatient.Register.Sql.GetClinicCode;
                    string noonSql = OutPatient.Register.Sql.GetNoonName;
                    string intimesSql = OutPatient.Register.Sql.GetOutPatientInTimes;
                    string pactSql = OutPatient.Register.Sql.GetPactInfo;
                    #region 获取患者基本信息

                    compatientSql = string.Format(compatientSql, order.CARDNO);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt = mgr.GetDataTable(compatientSql);
                    Models.Views.OutPatient.ComPatient patient1 = null;
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                patient1 = new Models.Views.OutPatient.ComPatient();
                                patient1.CardNo = dt.Rows[i][0].ToString();
                                patient1.Name = dt.Rows[i][1].ToString();
                                patient1.Birthday = dt.Rows[i][2].ToString();
                                patient1.SexCode = dt.Rows[i][3].ToString();
                                patient1.IDCard = dt.Rows[i][4].ToString();
                                patient1.McardNo = dt.Rows[i][5].ToString();
                                patient1.HomePhone = dt.Rows[i][6].ToString();
                                patient1.Address = dt.Rows[i][7].ToString();
                                patient1.RegDate = now;
                                break;
                            }
                            if (patient == null || string.IsNullOrEmpty(patient1.CardNo))
                            {
                                throw new Exception("获取患者信息出错！");
                            }
                        }
                        else
                        {
                            throw new Exception("没有找到患者信息！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有找到患者信息！");
                    }
                    patient1.Oper.ID = order.OPERCODE;
                    patient1.Oper.Name = order.OPERNAME;
                    patient1.OperTime = mgr.GetDateTimeFromSysDateTime();
                    #endregion
                    string result2 = string.Empty;

                    patient1.Pact.ID = "1";
                    patient1.Pact.PayKind.ID = "01";



                    #region 获取合同单位
                    pactSql = string.Format(pactSql, patient1.Pact.ID);
                    dt = new System.Data.DataTable();
                    dt = mgr.GetDataTable(pactSql);
                    //His.Models.ZZSB.PactInfo pactUnit = null;
                    Neusoft.HISFC.Models.Base.PactInfo pactUnit = null;
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                pactUnit = new Neusoft.HISFC.Models.Base.PactInfo();

                                pactUnit.ID = dt.Rows[i][0].ToString();//合同代码          
                                pactUnit.Name = dt.Rows[i][1].ToString();//合同单位名称                    
                                pactUnit.PayKind.ID = dt.Rows[i][2].ToString();//结算类别                    
                                pactUnit.Rate.PubRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][3].ToString().Trim());//公费比例                    
                                pactUnit.Rate.PayRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][4].ToString().Trim());//自付比例                   
                                pactUnit.Rate.OwnRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][5].ToString().Trim()); //自费比例                   
                                pactUnit.Rate.RebateRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][6].ToString().Trim()); //优惠比例                    
                                pactUnit.Rate.ArrearageRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][7].ToString().Trim());//欠费比例                    
                                pactUnit.Rate.IsBabyShared = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][8].ToString());//婴儿标志 0 无关 1 有关                                
                                pactUnit.IsNeedMCard = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][9].ToString().Trim()); //是否要求必须有医疗证号 0 否 1 是                      
                                pactUnit.IsInControl = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][10].ToString().Trim());//是否受监控 1受监控0不受监控                   
                                pactUnit.ItemType = dt.Rows[i][11].ToString().Trim(); //标志  0 全部 1 药品 2 非药品   
                                pactUnit.DayQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][12].ToString().Trim());//日限额                     
                                pactUnit.MonthQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][13].ToString().Trim()); //月限额                    
                                pactUnit.YearQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][14].ToString().Trim());//年限额
                                pactUnit.OnceQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][15].ToString().Trim());//一次限
                                string PriceForm = dt.Rows[i][16].ToString();
                                if (PriceForm == "0")
                                {
                                    pactUnit.PriceForm = "默认价";
                                }
                                else if (PriceForm == "1")
                                {
                                    pactUnit.PriceForm = "特诊价";
                                }
                                else if (PriceForm == "2")
                                {
                                    pactUnit.PriceForm = "儿童价";
                                }
                                //{B9303CFE-755D-4585-B5EE-8C1901F79450}maokb增加购入价
                                else if (PriceForm == "3")
                                {
                                    pactUnit.PriceForm = "购入价";
                                }
                                else
                                {
                                    pactUnit.PriceForm = "默认价";
                                }

                                pactUnit.BedQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][17].ToString());//床位限额
                                pactUnit.AirConditionQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][18].ToString());//空调限额
                                pactUnit.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[i][19]);//序号             
                                pactUnit.ShortName = dt.Rows[i][20].ToString();//合同单位简称
                                pactUnit.PactDllName = dt.Rows[i][21].ToString(); //待遇dll名称
                                pactUnit.PactDllDescription = dt.Rows[i][22].ToString();//待遇dll说明
                                pactUnit.PactSystemType = dt.Rows[i][23].ToString().Trim();

                                switch (pactUnit.PactSystemType)
                                {
                                    case "1":
                                        pactUnit.PactSystemType = "门诊";
                                        break;
                                    case "2":
                                        pactUnit.PactSystemType = "住院";
                                        break;
                                    case "3":
                                        pactUnit.PactSystemType = "系统";
                                        break;
                                    default:
                                        pactUnit.PactSystemType = "全院";
                                        break;
                                }
                                pactUnit.SpellCode = dt.Rows[i][24].ToString();//拼音码
                                pactUnit.WBCode = dt.Rows[i][25].ToString();//五笔码
                                pactUnit.PatientType.ID = dt.Rows[i][26].ToString();//人员类型编码
                                pactUnit.PatientType.Name = dt.Rows[i][27].ToString();//人员类型名称
                                pactUnit.IsUseInOutPatientFee = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][28].ToString().Trim());

                                break;
                            }
                            if (pactUnit == null || string.IsNullOrEmpty(pactUnit.ID))
                            {
                                ServiceLogManager.Write(result2 + "获取合同单位信息出错！  patient1.Pact.ID：" + patient1.Pact.ID);
                                ServiceLogManager.Write(pactUnit == null ? "pactUnit为空" : "ID为空");
                                throw new Exception(result2 + "获取合同单位信息出错！");
                            }
                        }
                        else
                        {
                            ServiceLogManager.Write(result2 + "获取合同单位信息出错！返回行数为0  patient1.Pact.ID：" + patient1.Pact.ID);
                            throw new Exception(result2 + "获取合同单位信息出错！返回行数为0");
                        }
                    }
                    else
                    {
                        ServiceLogManager.Write(result2 + "获取合同单位信息出错！dt为空  patient1.Pact.ID：" + patient1.Pact.ID);
                        throw new Exception(result2 + "获取合同单位信息出错！dt为空");
                    }
                    patient1.Pact = pactUnit;
                    #endregion

                    #region 支付方式

                    patient1.PayType = Common.Function.SetPayType("1");

                    #endregion

                    #region 获取排班信息

                    schemaSql = string.Format(schemaSql, order.SCHEDULEID);
                    dt = new System.Data.DataTable();
                    dt = mgr.GetDataTable(schemaSql);
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                patient1.SchemaID = dt.Rows[i][0].ToString();
                                patient1.SchemaType = dt.Rows[i][1].ToString();//排班类型，0科室/1医生
                                patient1.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][2].ToString());
                                patient1.Noon.ID = dt.Rows[i][4].ToString();
                                patient1.Dept.ID = dt.Rows[i][5].ToString();
                                patient1.Dept.Name = dt.Rows[i][6].ToString();
                                patient1.Doct.ID = order.DOCTORCODE;
                                patient1.Doct.Name = schedulingLogic.GetEMPLNAMEbyEMPLCODE(order.DOCTORCODE);
                                patient1.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][20].ToString());
                                patient1.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][21].ToString());
                                // patient1.RegLevel.ID = dt.Rows[i][29].ToString();
                                //patient.RegLevel.Name = dt.Rows[i][30].ToString();
                                patient1.RegLevel.ID = booking.REGLEVL_CODE;
                                patient1.Room.ID = dt.Rows[i][31].ToString();
                                patient1.Room.Name = dt.Rows[i][32].ToString();
                                patient1.Console.ID = dt.Rows[i][33].ToString();
                                patient1.Console.Name = dt.Rows[i][34].ToString();
                                break;
                            }
                            if (string.IsNullOrEmpty(patient1.SchemaID))
                            {
                                throw new Exception("获取排班信息出错！");
                            }
                        }
                        else
                        {
                            throw new Exception("没有找到排班信息！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有找到排班信息！");
                    }

                    #endregion

                    #region 获取挂号等级费用

                    //regfeeSql = string.Format(regfeeSql, "1", patient1.RegLevel.ID);
                    //dt = new System.Data.DataTable();
                    //dt = mgr.GetDataTable(regfeeSql);
                    //if (dt != null)
                    //{
                    //    if (dt.Rows.Count > 0)
                    //    {
                    //        for (int i = 0; i < dt.Rows.Count; i++)
                    //        {
                    //            patient1.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][4]);//挂号费
                    //            patient1.OwnDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][6]);//诊查费
                    //            break;
                    //        }
                    //        if (patient1.OwnDigFee == null || string.IsNullOrEmpty(patient1.OwnDigFee.ToString()))
                    //        {
                    //            throw new Exception("获取费用信息出错！");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        throw new Exception("获取费用信息出错！");
                    //    }
                    //}
                    //else
                    //{
                    //    throw new Exception("获取费用信息出错！");
                    //}
                    #endregion
                    patient1.OwnDigFee = 0;
                    patient1.RegFee = 0;


                    #region 获取护士分诊队列信息
                    dt = new System.Data.DataTable();
                    if (patient1.SchemaType == "0")
                    {
                        //为科室排班
                        // nurQueueSql1 = string.Format(nurQueueSql1, now.ToString("yyyy-MM-dd HH:mm:ss"), patient1.Dept.ID, patient1.Noon.ID, patient1.Room.ID);
                        nurQueueSql1 = string.Format(nurQueueSql1, patient1.SchemaID);
                        dt = mgr.GetDataTable(nurQueueSql1);
                    }
                    else if (patient1.SchemaType == "1")
                    {
                        //为医生排班
                        nurQueueSql2 = string.Format(nurQueueSql2, patient1.Begin.ToShortDateString(), patient1.Doct.ID, patient1.Noon.ID, patient1.Dept.ID);
                        dt = mgr.GetDataTable(nurQueueSql2);
                    }
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                patient1.NurseCell.ID = dt.Rows[i][0].ToString();
                                patient1.Queue.ID = dt.Rows[i][1].ToString();
                                patient1.Queue.Name = dt.Rows[i][2].ToString();
                                break;
                            }
                            if (string.IsNullOrEmpty(patient1.Queue.ID))//|| string.IsNullOrEmpty(patient.NurseCell.ID)
                            {
                                throw new Exception("获取分诊队列信息出错！");
                            }
                        }
                        else
                        {
                            throw new Exception("没有找到分诊队列信息！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有找到分诊队列信息！");
                    }
                    #endregion


                    #region 获取门诊流水号

                    dt = new System.Data.DataTable();
                    dt = mgr.GetDataTable(clinicCodeSql);


                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            if (!Convert.IsDBNull(dt.Rows[0][0]))
                            {
                                patient1.ClinicCode = dt.Rows[0][0].ToString();
                            }
                            else
                            {
                                throw new Exception("获取门诊流水号出错！");
                            }
                        }
                        else
                        {
                            throw new Exception("获取门诊流水号出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("获取门诊流水号出错！");
                    }


                    #endregion

                    #region 获取门诊看诊次数

                    dt = new System.Data.DataTable();
                    intimesSql = string.Format(intimesSql, patient1.CardNo);
                    dt = mgr.GetDataTable(intimesSql);

                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            if (!Convert.IsDBNull(dt.Rows[0][0]))
                            {
                                patient1.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[0][0]);
                            }
                            else
                            {
                                throw new Exception("获取门诊看诊次数出错！");
                            }
                        }
                        else
                        {
                            throw new Exception("获取门诊看诊次数出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("获取门诊看诊次数出错！");
                    }

                    #endregion


                    //减免信息。
                    patient1.RegDiagCode = "";

                    #region 获取seeNo
                    //当日挂号
                    if (order.REGTYPE == "1")
                    {
                        dt = new System.Data.DataTable();
                        DataTable dt2 = new DataTable();
                        {
                            //为医生排班

                            //min最小看诊序号，seeNO当前看诊序号，cnt当前排班限额
                            int minNo = -1, seeNo = 0, cnt = 0, Residue = 0;

                            if (mgr.GetMinSeeNo(patient1.SchemaID, ref minNo) == -1)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                result.IsSuccessful = false;
                                result.Message = mgr.Err;
                                return result;
                            }
                            if (mgr.GetCurrentSeeNo(patient1.SchemaID, ref seeNo) == -1)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                result.IsSuccessful = false;
                                result.Message = mgr.Err;
                                return result;
                            }


                            if (minNo < 1)
                            {
                                throw new Exception("取出最小看诊序号不正确，排班ID：" + patient1.SchemaID.ToString());
                            }
                            if (mgr.GetSourceCount(patient1.SchemaID, ref cnt) != -1)
                            {
                                mgr.GetResidue(patient1.SchemaID, ref Residue);
                                if (Residue >= cnt)
                                {
                                    throw new Exception("已经没有足够号源可以，请选择其他时段排班");
                                }
                            }

                            if ((patient1.RegLevel.ID != "4") && (seeNo == 0 || seeNo < minNo))//RegLevel.ID==4是急诊，seeNo==0为排班当天第一个挂号，seeNo<minNo 为上一时段未挂完的号，时段过了，则从下一个时段最小序号开始
                            {
                                seeNo = minNo;
                            }
                            else
                            {
                                seeNo = seeNo + 1;
                            }

                            patient1.SeeNO = seeNo;
                        }
                    }
                    else
                    {
                        patient1.SeeNO = 0;
                    }
                    #endregion
                    if (order.REGTYPE == "0")
                    {
                        patient1.Isbooking = "1";
                    }
                    #region 更新排班表，插入号源表
                    //插入挂号主表
                    string insertReg = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.insertReg;
                    string[] argm = mgr.GetRegInfo(patient1);
                    insertReg = string.Format(insertReg, argm);
                    string msg = "";
                    if (mgr.ExecuteSql(insertReg, ref msg) == -1)
                    {
                        throw new Exception("挂号登记失败！" + msg);
                    }
                    order.STATUS = "2";
                    if (!plaLogic.Update(order))
                    {
                        throw new Exception("记录数据失败！");
                    }
                    #endregion

                    #region 组织返回结果

                    //是否职工
                    string visitAddressSql1 = @"select (select n.remark||n.room_name from MET_NUO_CONSOLE n where n.console_code=m.console_code and rownum=1) 
from fin_opr_schema m
where m.id='{0}'";
                    visitAddressSql1 = string.Format(visitAddressSql1, schema.ID);
                    string visitAddress1 = mgr.ExecSqlReturnOne(visitAddressSql1, "");
                    Models.Views.OrderResult res1 = new Models.Views.OrderResult();
                    res1.OrderId = order.ORDERID;
                    res1.HospitalNum = patient1.ClinicCode;
                    res1.Proof = "";
                    res1.VisitNo = "";  //就诊序号
                    res1.VisitAddress = visitAddress1;// string.Empty;  //就诊位置
                    res1.RegFee = (0 * 100).ToString();
                    result.IsSuccessful = true;
                    result.Message = "";
                    result.ReturnData = res1;
                    #endregion

                    return result;
                }
                #endregion

                #region 增加号源,医院要求加号不占用现场号源
                addRes = regLogic.UpdateSchemaRegLmt(schemaId);
                if (addRes <= 0)
                {
                    throw new Exception("增加号源失败！索引：UpdateSchemaRegLmt");
                }
                #endregion

                #region 6.占用号源
                string bookingID = string.Empty;
                string visitNo = string.Empty;
                if (order.REGTYPE == "0")
                {
                    //判断支付方式，1在线支付 0到院支付， 到院支付直接锁号就行
                    if (order.PAYMETHOD == "0")
                    {
                        if (neusoftBussiness.LockPreRegInfo(order.REGTYPE, schema, booking, oper, ref bookingID, ref visitNo, ref errMsg) <= 0)
                        {
                            throw new Exception(errMsg);
                        }
                    }
                    else if (order.PAYMETHOD == "1")
                    {
                        if (neusoftBussiness.LockPreRegInfo(order.REGTYPE, schema, booking, oper, ref bookingID, ref visitNo, ref errMsg) <= 0)
                        {
                            throw new Exception(errMsg);
                        }
                    }
                    else
                    {
                        throw new Exception("支付方式错误");
                    }
                }
                else
                {
                    if (neusoftBussiness.LockPreRegInfo(order.REGTYPE, schema, booking, oper, ref bookingID, ref visitNo, ref errMsg) <= 0)
                    {
                        throw new Exception(errMsg);
                    }
                }
                #endregion

                order.REGFEE = (regfeeonpact.DIAG_FEE * 100).ToString();
                order.CLINIC_CODE = bookingID;
                if (!plaLogic.Update(order))
                {
                    throw new Exception("记录数据失败！");
                }


                #region 组织返回结果

                //是否职工
                string visitAddressSql = @"select (select n.remark||n.room_name from MET_NUO_CONSOLE n where n.console_code=m.console_code and rownum=1) 
from fin_opr_schema m
where m.id='{0}'";
                visitAddressSql = string.Format(visitAddressSql, schema.ID);
                string visitAddress = mgr.ExecSqlReturnOne(visitAddressSql, "");
                Models.Views.OrderResult res = new Models.Views.OrderResult();
                res.OrderId = order.ORDERID;
                res.HospitalNum = bookingID;
                res.Proof = bookingID;
                res.VisitNo = visitNo;  //就诊序号
                res.VisitAddress = visitAddress;// string.Empty;  //就诊位置
                res.RegFee = (regfeeonpact.DIAG_FEE * 100).ToString();
                result.IsSuccessful = true;
                result.Message = "";
                result.ReturnData = res;
                #endregion
                //plaLogic.CommitTran();

                return result;


            }
            catch (Exception ex)
            {
                //plaLogic.RollbackTran();
                result.IsSuccessful = false;
                result.Message = ex.Message;
                return result;
            }
            //result.IsSuccessful = true;
            //return result;
        }

        /// <summary>
        /// 释放号源
        /// </summary>
        /// <param name="orderId">平台定单号</param>
        /// <param name="hospitalNum">医院订单号</param>
        /// <param name="patientId">院内用户ID</param>
        /// <param name="cancelReason">取消原因</param>
        /// <param name="frontProviderId">第三方服务商 ID</param>
        /// <param name="cancelTime">取消时间</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public int CancelLock(string orderId, string hospitalNum, string patientId, string cancelReason, string frontProviderId, DateTime cancelTime, string clincCode, ref string error)
        {
            BL.OutPatient.BookingLogic bookingLogic = new BL.OutPatient.BookingLogic();
            OutPatient.Register.Manager mgr = new OutPatient.Register.Manager();
            FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic platformOrderLogic = new BL.OutPatient.PlatformOrderLogic();

            //
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            platformOrderLogic.BeginTran();
            bookingLogic.BeginTran();
            mgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            try
            {



                #region 1.获取订单号，并校验
                string orderType = string.Empty;
                string schemaID = string.Empty;


                FS.ZDWY.Internet.Models.FIN_OPR_BOOKING booking = bookingLogic.Get(hospitalNum);
                if (booking == null)
                {
                    throw new Exception("获取对应预约订单信息失败");
                }
                if (string.IsNullOrEmpty(booking.CLINIC_CODE))
                {
                    throw new Exception("获取对应预约订单信息失败");
                }
                if (booking.CARD_NO != patientId)
                {
                    throw new Exception("预约订单信息不正确（patientId）");
                }
                //需要再查下保存的订单信息
                FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER order = platformOrderLogic.Get(orderId);
                if (order == null || string.IsNullOrEmpty(order.ORDERID))
                {
                    throw new Exception("获取对应预约订单信息失败");
                }
                if (order.STATUS != "1")
                {
                    throw new Exception("订单已作废或已取号");
                }
                orderType = order.REGTYPE;
                schemaID = booking.SCHEMA_NO;
                #endregion



                #region 2.作废预约订单，并释放号源
                //作废订单信息
                bool updateOrderRes = platformOrderLogic.Update((o) => new Models.PLATFORM_REGISTER_ORDER() { STATUS = "3" }, w => w.ORDERID == orderId);
                if (!updateOrderRes)
                {
                    throw new Exception("作废预约订单信息失败");
                }
                bool updateBookingRes = bookingLogic.Update((o) => new FS.ZDWY.Internet.Models.FIN_OPR_BOOKING() { VALID_FLAG = "0" }, w => w.CLINIC_CODE == hospitalNum);
                if (!updateBookingRes)
                {
                    throw new Exception("作废预约订单信息失败");
                }
                //更新号源信息                
                //BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
                //if (schedulingLogic.UpdateNum(orderType, schemaID) <= 0)
                //{
                //    throw new Exception("释放号源失败");
                //}

                if (order.REGTYPE == "1")
                {
                    string updateSchema = @"update fin_opr_schema s --医师出诊表
                                           set s.reged = s.reged + {1}
                                         where s.id = '{0}'";
                    updateSchema = string.Format(updateSchema, order.SCHEDULEID, "-1");
                    int result = mgr.ExecNoQuery(updateSchema);
                    if (result <= 0)
                    {
                        throw new Exception("解锁号源失败！");
                    }
                }
                else if (order.REGTYPE == "0")
                {
                    string updateSchema = @"update fin_opr_schema s --医师出诊表
                                           set s.tel_reging = s.tel_reging + {1}, --预约已约
                                               s.tel_reged  = s.tel_reged + {1}-- 预约已挂
                                         where s.id = '{0}'";
                    updateSchema = string.Format(updateSchema, order.SCHEDULEID, "-1");
                    int result = mgr.ExecNoQuery(updateSchema);
                    if (result <= 0)
                    {
                        throw new Exception("解锁号源失败！");
                    }
                }

                if (!string.IsNullOrEmpty(clincCode))
                {
                    GDSI.ZhuHaiSI.Business.Comom.MedicalService ms = new GDSI.ZhuHaiSI.Business.Comom.MedicalService();
                    Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
                    if (ms.CancelRegSettlement(clincCode, "00A105", "微信", "3") < 0)
                    {

                        throw new Exception(ms.ErrorMessage);
                    }
                }


                Neusoft.FrameWork.Management.PublicTrans.Commit();
                platformOrderLogic.CommitTran();
                bookingLogic.CommitTran();
                return 1;
                #endregion
            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                platformOrderLogic.RollbackTran();
                bookingLogic.RollbackTran();
                error = e.Message.ToString();
                return -1;
            }




            return 1;
        }

        /// <summary>
        /// 平台的性别代码转成HIS的
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string ConvertHISSexCode(string code)
        {
            switch (code)
            {
                case "1": return "M";
                case "2": return "F";
                case "9": return "U";
                default: return "U";
            }
        }

        #region 新增

        public string Test()
        {
            string sqlsugarservice = "";
            string hisservice = "";
            try
            {
                OutPatient.Register.Manager mgr = new OutPatient.Register.Manager();
                DateTime now = mgr.GetDateTimeFromSysDateTime();
                hisservice = "his数据库连接正常！";
                string text = AppDomain.CurrentDomain.BaseDirectory + "\\App.config";
                FS.ZDWY.Internet.BL.OutPatient.BookingLogic log = new BL.OutPatient.BookingLogic();
                now = log.GetDateTime();
                sqlsugarservice = "sqlsugar数据库连接正常！";
                return sqlsugarservice + ";" + hisservice + ";" + now.ToString();
            }
            catch (Exception e)
            {
                return sqlsugarservice + ";" + hisservice + ";" + e.Message.ToString();
            }

        }




        /// <summary>
        /// 当日挂号支付
        /// </summary>
        /// <returns></returns>
        public Models.Views.ComResult<Models.Views.OutPatient.PayResult> RegisterPay(Models.PLATFORM_REGISTER_PAY regpay, string clincCode, string isMedical, string informedConsentResult)
        {
            Models.Views.ComResult<Models.Views.OutPatient.PayResult> result = new Models.Views.ComResult<Models.Views.OutPatient.PayResult>();

            #region 获取订单号，并校验  booking,order
            string orderType = string.Empty;
            string schemaID = string.Empty;

            BL.OutPatient.BookingLogic bookingLogic = new BL.OutPatient.BookingLogic();
            FS.ZDWY.Internet.Models.FIN_OPR_BOOKING booking = bookingLogic.Get(regpay.HOSPITALNUM);
            if (booking == null)
            {
                result.IsSuccessful = false;
                result.Message = "获取对应预约订单信息失败";
                return result;
            }
            if (string.IsNullOrEmpty(booking.CLINIC_CODE))
            {
                result.IsSuccessful = false;
                result.Message = "获取对应预约订单信息失败";
                return result;
            }
            if (booking.CARD_NO != regpay.PATIENTID)
            {
                result.IsSuccessful = false;
                result.Message = "预约订单信息不正确（patientId）";
                return result;
            }


            FS.ZDWY.Internet.BL.OutPatient.RegisterPayInfoLogic platformPayLogic = new BL.OutPatient.RegisterPayInfoLogic();

            FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic platformOrderLogic = new BL.OutPatient.PlatformOrderLogic();
            FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER order = platformOrderLogic.Get(regpay.ORDERID);
            if (order == null || string.IsNullOrEmpty(order.ORDERID))
            {
                result.IsSuccessful = false;
                result.Message = "预约订单信息不正确（patientId）";
                return result;
            }
            #endregion

            if (order.STATUS == "2")
            {
                Models.PLATFORM_REGISTER_PAY preres = platformPayLogic.Get(regpay.ORDERID);

                Models.Views.OutPatient.PayResult res = new Models.Views.OutPatient.PayResult();
                res.HospTradeId = preres.HospTradeId;
                res.InvoiceId = preres.HospTradeId;
                res.ReceiptId = preres.HospTradeId;
                res.VisitAddress = "";
                res.VisitNo = order.REGISTERID;
                res.Proof = "";
                res.Remark = "";
                result.IsSuccessful = true;
                result.ReturnData = res;
                return result;
            }
            else if (order.STATUS == "3")
            {
                result.IsSuccessful = false;
                result.Message = "订单已作废！";
                return result;
            }
            else if (order.STATUS != "1")
            {
                result.IsSuccessful = false;
                result.Message = "订单已缴费！";
                return result;
            }

            OutPatient.Register.Manager mgr = new OutPatient.Register.Manager();
            DateTime now = mgr.GetDateTimeFromSysDateTime();

            if (regpay.HCAREAMOUNT == 0 && regpay.EXPENSEAMOUNT == 0)
            {
                regpay.TransNo = "";
            }
            if ((regpay.HCAREAMOUNT > 0 || regpay.EXPENSEAMOUNT > 0) && isMedical == "0")
            {
                throw new Exception("存在报销金额，【isMedical】不能为0");
            }
            try
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                mgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                platformOrderLogic.BeginTran();
                platformPayLogic.BeginTran();

                #region 验证挂号患者信息和挂号级别

                #endregion

                #region 获取患者信息，排班信息，挂号登记费用等

                string schemaSql = OutPatient.Register.Sql.GetSchema;
                string compatientSql = OutPatient.Register.Sql.GetPatientInfo;
                string regfeeSql = OutPatient.Register.Sql.GetRegFee;
                string nurQueueSql1 = OutPatient.Register.Sql.GetNurQueueByDept;
                string nurQueueSql2 = OutPatient.Register.Sql.GetNurQueueByDoct;
                string invoicenoSql1 = OutPatient.Register.Sql.GetInvoiceInfoUsed;
                string invoicenoSql2 = OutPatient.Register.Sql.GetInvoiceR;
                string invoicenoSql3 = OutPatient.Register.Sql.GetInvoiceUserCode;
                string seenoSql = OutPatient.Register.Sql.GetSeeNo;
                string clinicCodeSql = OutPatient.Register.Sql.GetClinicCode;
                string noonSql = OutPatient.Register.Sql.GetNoonName;
                string intimesSql = OutPatient.Register.Sql.GetOutPatientInTimes;
                string pactSql = OutPatient.Register.Sql.GetPactInfo;

                string getnewseeno = OutPatient.Register.Sql.GetNewSeeNo;
                string updateseeno = OutPatient.Register.Sql.UpdateSeeNo;

                #region 判断是否有足够号源

                int regRemainCount = 0;
                string sql = @"select (t.reg_lmt - t.reged) regRemain
                                                          from fin_opr_schema t
                                                         where t.id = '{0}'";
                sql = string.Format(sql, order.SCHEDULEID);
                //排班表
                regRemainCount = Neusoft.FrameWork.Function.NConvert.ToInt32(mgr.ExecSqlReturnOne(sql));

                //if (regRemainCount <= 0)
                //{
                //    throw new Exception("没有足够号源，请选择其他排班！");
                //}
                mgr.ExecQuery("select '" + order.SCHEDULEID + "-" + regRemainCount.ToString() + "' from dual ");
                #endregion

                #region 获取患者基本信息

                compatientSql = string.Format(compatientSql, order.CARDNO);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = mgr.GetDataTable(compatientSql);
                Models.Views.OutPatient.ComPatient patient = null;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient = new Models.Views.OutPatient.ComPatient();
                            patient.CardNo = dt.Rows[i][0].ToString();
                            patient.Name = dt.Rows[i][1].ToString();
                            patient.Birthday = dt.Rows[i][2].ToString();
                            patient.SexCode = dt.Rows[i][3].ToString();
                            patient.IDCard = dt.Rows[i][4].ToString();
                            patient.McardNo = dt.Rows[i][5].ToString();
                            patient.HomePhone = dt.Rows[i][6].ToString();
                            patient.Address = dt.Rows[i][7].ToString();
                            patient.RegDate = now;
                            break;
                        }
                        if (patient == null || string.IsNullOrEmpty(patient.CardNo))
                        {
                            throw new Exception("获取患者信息出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有找到患者信息！");
                    }
                }
                else
                {
                    throw new Exception("没有找到患者信息！");
                }
                patient.Oper.ID = regpay.OPERCODE;
                patient.Oper.Name = regpay.OPERNAME;
                patient.OperTime = mgr.GetDateTimeFromSysDateTime();
                #endregion
                string result2 = string.Empty;
                if (isMedical == "1")
                {
                    string sql12 = string.Format(@" select p.pact_code from fin_ipr_siinmaininfo_gd p where p.inpatient_no='{0}' and p.type_code='0' and ydzf = '1'", clincCode);

                    try
                    {
                        result2 = mgr.ExecSqlReturnOne(sql12);
                        if (result2 == "-1" || string.IsNullOrEmpty(result2))
                        {
                            result2 = string.Empty;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("获取医保合同单位信息出错！" + e.Message);
                    }
                    patient.Pact.ID = result2;
                    patient.Pact.PayKind.ID = "02";
                }
                else if (regpay.PAYMODE == "7")//长者券
                {
                    patient.Pact.ID = "258";
                    patient.Pact.PayKind.ID = "01";
                }
                else
                {
                    patient.Pact.ID = "1";
                    patient.Pact.PayKind.ID = "01";
                }

                string insuplcAdmdvs = "";
                if (isMedical == "1")
                {
                    string sql12 = string.Format(@" select p.insuplcAdmdvs from fin_ipr_siinmaininfo_gd p where p.inpatient_no='{0}' and p.type_code='0' ", clincCode);

                    try
                    {
                        insuplcAdmdvs = mgr.ExecSqlReturnOne(sql12);
                        if (insuplcAdmdvs == "-1" || string.IsNullOrEmpty(insuplcAdmdvs))
                        {
                            insuplcAdmdvs = string.Empty;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("获取医保参保信息出错！" + e.Message);
                    }
                }

                #region 获取合同单位
                pactSql = string.Format(pactSql, patient.Pact.ID);
                dt = new System.Data.DataTable();
                dt = mgr.GetDataTable(pactSql);
                //His.Models.ZZSB.PactInfo pactUnit = null;
                Neusoft.HISFC.Models.Base.PactInfo pactUnit = null;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            pactUnit = new Neusoft.HISFC.Models.Base.PactInfo();

                            pactUnit.ID = dt.Rows[i][0].ToString();//合同代码          
                            pactUnit.Name = dt.Rows[i][1].ToString();//合同单位名称                    
                            pactUnit.PayKind.ID = dt.Rows[i][2].ToString();//结算类别                    
                            pactUnit.Rate.PubRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][3].ToString().Trim());//公费比例                    
                            pactUnit.Rate.PayRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][4].ToString().Trim());//自付比例                   
                            pactUnit.Rate.OwnRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][5].ToString().Trim()); //自费比例                   
                            pactUnit.Rate.RebateRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][6].ToString().Trim()); //优惠比例                    
                            pactUnit.Rate.ArrearageRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][7].ToString().Trim());//欠费比例                    
                            pactUnit.Rate.IsBabyShared = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][8].ToString());//婴儿标志 0 无关 1 有关                                
                            pactUnit.IsNeedMCard = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][9].ToString().Trim()); //是否要求必须有医疗证号 0 否 1 是                      
                            pactUnit.IsInControl = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][10].ToString().Trim());//是否受监控 1受监控0不受监控                   
                            pactUnit.ItemType = dt.Rows[i][11].ToString().Trim(); //标志  0 全部 1 药品 2 非药品   
                            pactUnit.DayQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][12].ToString().Trim());//日限额                     
                            pactUnit.MonthQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][13].ToString().Trim()); //月限额                    
                            pactUnit.YearQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][14].ToString().Trim());//年限额
                            pactUnit.OnceQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][15].ToString().Trim());//一次限
                            string PriceForm = dt.Rows[i][16].ToString();
                            if (PriceForm == "0")
                            {
                                pactUnit.PriceForm = "默认价";
                            }
                            else if (PriceForm == "1")
                            {
                                pactUnit.PriceForm = "特诊价";
                            }
                            else if (PriceForm == "2")
                            {
                                pactUnit.PriceForm = "儿童价";
                            }
                            //{B9303CFE-755D-4585-B5EE-8C1901F79450}maokb增加购入价
                            else if (PriceForm == "3")
                            {
                                pactUnit.PriceForm = "购入价";
                            }
                            else
                            {
                                pactUnit.PriceForm = "默认价";
                            }

                            pactUnit.BedQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][17].ToString());//床位限额
                            pactUnit.AirConditionQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][18].ToString());//空调限额
                            pactUnit.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[i][19]);//序号             
                            pactUnit.ShortName = dt.Rows[i][20].ToString();//合同单位简称
                            pactUnit.PactDllName = dt.Rows[i][21].ToString(); //待遇dll名称
                            pactUnit.PactDllDescription = dt.Rows[i][22].ToString();//待遇dll说明
                            pactUnit.PactSystemType = dt.Rows[i][23].ToString().Trim();

                            switch (pactUnit.PactSystemType)
                            {
                                case "1":
                                    pactUnit.PactSystemType = "门诊";
                                    break;
                                case "2":
                                    pactUnit.PactSystemType = "住院";
                                    break;
                                case "3":
                                    pactUnit.PactSystemType = "系统";
                                    break;
                                default:
                                    pactUnit.PactSystemType = "全院";
                                    break;
                            }
                            pactUnit.SpellCode = dt.Rows[i][24].ToString();//拼音码
                            pactUnit.WBCode = dt.Rows[i][25].ToString();//五笔码
                            pactUnit.PatientType.ID = dt.Rows[i][26].ToString();//人员类型编码
                            pactUnit.PatientType.Name = dt.Rows[i][27].ToString();//人员类型名称
                            pactUnit.IsUseInOutPatientFee = Neusoft.FrameWork.Function.NConvert.ToBoolean(dt.Rows[i][28].ToString().Trim());

                            break;
                        }
                        if (pactUnit == null || string.IsNullOrEmpty(pactUnit.ID))
                        {
                            ServiceLogManager.Write(result2 + "获取合同单位信息出错！ clincCode：" + clincCode + " patient.Pact.ID：" + patient.Pact.ID);
                            ServiceLogManager.Write(pactUnit == null ? "pactUnit为空" : "ID为空");
                            throw new Exception(result2 + "获取合同单位信息出错！");
                        }
                    }
                    else
                    {
                        ServiceLogManager.Write(result2 + "获取合同单位信息出错！返回行数为0 clincCode：" + clincCode + " patient.Pact.ID：" + patient.Pact.ID);
                        throw new Exception(result2 + "获取合同单位信息出错！返回行数为0");
                    }
                }
                else
                {
                    ServiceLogManager.Write(result2 + "获取合同单位信息出错！dt为空 clincCode：" + clincCode + " patient.Pact.ID：" + patient.Pact.ID);
                    throw new Exception(result2 + "获取合同单位信息出错！dt为空");
                }
                patient.Pact = pactUnit;
                #endregion

                #region 支付方式

                patient.PayType = Common.Function.SetPayType(regpay.PAYMODE);

                #endregion

                #region 获取排班信息

                schemaSql = string.Format(schemaSql, order.SCHEDULEID);
                dt = new System.Data.DataTable();
                dt = mgr.GetDataTable(schemaSql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.SchemaID = dt.Rows[i][0].ToString();
                            patient.SchemaType = dt.Rows[i][1].ToString();//排班类型，0科室/1医生
                            patient.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][2].ToString());
                            patient.Noon.ID = dt.Rows[i][4].ToString();
                            patient.Dept.ID = dt.Rows[i][5].ToString();
                            patient.Dept.Name = dt.Rows[i][6].ToString();
                            patient.Doct.ID = dt.Rows[i][7].ToString();
                            patient.Doct.Name = dt.Rows[i][8].ToString();
                            patient.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][20].ToString());
                            patient.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][21].ToString());
                            // patient.RegLevel.ID = dt.Rows[i][29].ToString();
                            //patient.RegLevel.Name = dt.Rows[i][30].ToString();
                            patient.RegLevel.ID = booking.REGLEVL_CODE;
                            patient.Room.ID = dt.Rows[i][31].ToString();
                            patient.Room.Name = dt.Rows[i][32].ToString();
                            patient.Console.ID = dt.Rows[i][33].ToString();
                            patient.Console.Name = dt.Rows[i][34].ToString();
                            break;
                        }
                        if (string.IsNullOrEmpty(patient.SchemaID))
                        {
                            throw new Exception("获取排班信息出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有找到排班信息！");
                    }
                }
                else
                {
                    throw new Exception("没有找到排班信息！");
                }

                #endregion

                #region 挂号限制

                //14岁
                if (!string.IsNullOrEmpty(patient.Birthday))
                {
                    DateTime dd = DateTime.MinValue;
                    if (DateTime.TryParse(patient.Birthday, out dd))
                        if (dd.AddYears(14) > mgr.GetDateTimeFromSysDateTime())
                        {
                            if (mgr.GetAge14LimitDept(order.DEPTCODE) != "-1")
                            {
                                throw new Exception("14周岁以下不能挂内科！");
                            }
                        }
                        else
                        {
                            if (order.DEPTCODE == "6002")
                            {
                                throw new Exception("14周岁以上不能挂儿科！");
                            }
                        }
                }

                //男性
                if (patient.SexCode == "M")
                {
                    if (order.DEPTCODE == "6070")
                    {
                        throw new Exception("男性不能挂该科室！");
                    }
                }
                //女性
                //else if (patient.SexCode == "F")
                //{
                //    if (order.DEPTCODE == "9254")
                //    {
                //        throw new Exception("女性不能挂该科室！");
                //    }
                //}

                #endregion


                #region 获取挂号等级费用

                regfeeSql = string.Format(regfeeSql, "1", patient.RegLevel.ID);
                dt = new System.Data.DataTable();
                dt = mgr.GetDataTable(regfeeSql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][4]);//挂号费
                            patient.OwnDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][6]);//诊查费
                            break;
                        }
                        if (patient.OwnDigFee == null || string.IsNullOrEmpty(patient.OwnDigFee.ToString()))
                        {
                            throw new Exception("获取费用信息出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("获取费用信息出错！");
                    }
                }
                else
                {
                    throw new Exception("获取费用信息出错！");
                }

                #endregion

                #region 获取护士分诊队列信息
                dt = new System.Data.DataTable();
                if (patient.SchemaType == "0")
                {
                    //为科室排班
                    // nurQueueSql1 = string.Format(nurQueueSql1, now.ToString("yyyy-MM-dd HH:mm:ss"), patient.Dept.ID, patient.Noon.ID, patient.Room.ID);
                    nurQueueSql1 = string.Format(nurQueueSql1, patient.SchemaID);
                    dt = mgr.GetDataTable(nurQueueSql1);
                }
                else if (patient.SchemaType == "1")
                {
                    //为医生排班
                    nurQueueSql2 = string.Format(nurQueueSql2, patient.Begin.ToShortDateString(), patient.Doct.ID, patient.Noon.ID, patient.Dept.ID);
                    dt = mgr.GetDataTable(nurQueueSql2);
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.NurseCell.ID = dt.Rows[i][0].ToString();
                            patient.Queue.ID = dt.Rows[i][1].ToString();
                            patient.Queue.Name = dt.Rows[i][2].ToString();
                            break;
                        }
                        if (string.IsNullOrEmpty(patient.Queue.ID))//|| string.IsNullOrEmpty(patient.NurseCell.ID)
                        {
                            throw new Exception("获取分诊队列信息出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有找到分诊队列信息！");
                    }
                }
                else
                {
                    throw new Exception("没有找到分诊队列信息！");
                }
                #endregion

                //发票锁号
                string lockinvoice = OutPatient.Register.Sql.LockInvoiceNo;
                lockinvoice = string.Format(lockinvoice, regpay.OPERCODE, "1");
                int lockres = mgr.ExecNoQuery(lockinvoice);
                if (lockres <= 0)
                {
                    throw new Exception("发票号锁号失败！");
                }

                #region 获取发票信息
                string erro = "";
                string realInvoice = string.Empty;
                string invoiceStr = string.Empty;
                dt = new System.Data.DataTable();
                invoicenoSql1 = string.Format(invoicenoSql1, regpay.OPERCODE, "1");
                dt = mgr.GetDataTable(invoicenoSql1);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.BeginInvoice = dt.Rows[i][0].ToString();
                            patient.EndInvoice = dt.Rows[i][1].ToString();
                            break;
                        }
                        if (mgr.GetInvoiceR(invoicenoSql2, regpay.OPERCODE, now, ref realInvoice, ref invoiceStr, ref erro) < 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            result.IsSuccessful = false;
                            result.Message = erro;
                            return result;
                        }
                        if (!string.IsNullOrEmpty(erro))
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            result.IsSuccessful = false;
                            result.Message = erro;
                            return result;
                        }

                        patient.RealInvoice = realInvoice;
                        patient.InvoiceStr = invoiceStr;
                        patient.IsUseingInvoice = true;
                    }
                    else
                    {
                        invoicenoSql1 = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.GetInvoiceInfoUsed;
                        invoicenoSql1 = string.Format(invoicenoSql1, regpay.OPERCODE, "0");
                        dt = mgr.GetDataTable(invoicenoSql1);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (!Convert.IsDBNull(dt.Rows[0][0]))
                                {
                                    patient.RealInvoice = dt.Rows[0][0].ToString();
                                    patient.BeginInvoice = dt.Rows[0][0].ToString();
                                    patient.EndInvoice = dt.Rows[0][1].ToString();
                                }
                                if (mgr.GetInvoiceR(invoicenoSql2, regpay.OPERCODE, now, ref realInvoice, ref invoiceStr, ref erro) < 0)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    result.IsSuccessful = false;
                                    result.Message = erro;
                                    return result;
                                }
                                if (!string.IsNullOrEmpty(erro))
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    result.IsSuccessful = false;
                                    result.Message = erro;
                                    return result;
                                }
                            }
                            else
                            {
                                throw new Exception("没有找到发票信息！");
                            }
                        }
                        else
                        {
                            throw new Exception("没有找到发票信息！");
                        }
                    }
                    patient.NextRealInvoice = mgr.AddNumber(patient.RealInvoice);
                    patient.NextInvoiceStr = mgr.AddNumber(patient.InvoiceStr);
                }
                else
                {
                    throw new Exception("没有找到发票信息！");
                }

                #endregion

                #region 获取门诊流水号

                dt = new System.Data.DataTable();
                dt = mgr.GetDataTable(clinicCodeSql);

                if (isMedical == "1")
                {
                    patient.ClinicCode = clincCode;
                }
                else
                {
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            if (!Convert.IsDBNull(dt.Rows[0][0]))
                            {
                                patient.ClinicCode = dt.Rows[0][0].ToString();
                            }
                            else
                            {
                                throw new Exception("获取门诊流水号出错！");
                            }
                        }
                        else
                        {
                            throw new Exception("获取门诊流水号出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("获取门诊流水号出错！");
                    }
                }

                #endregion

                #region 获取门诊看诊次数

                dt = new System.Data.DataTable();
                intimesSql = string.Format(intimesSql, patient.CardNo);
                dt = mgr.GetDataTable(intimesSql);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (!Convert.IsDBNull(dt.Rows[0][0]))
                        {
                            patient.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[0][0]);
                        }
                        else
                        {
                            throw new Exception("获取门诊看诊次数出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("获取门诊看诊次数出错！");
                    }
                }
                else
                {
                    throw new Exception("获取门诊看诊次数出错！");
                }

                #endregion


                //减免信息。
                patient.RegDiagCode = "";
                #endregion

                #region 减免费用处理

                decimal hccost = regpay.HCAREAMOUNT;//
                decimal excost = regpay.EXPENSEAMOUNT;//
                decimal pucost = regpay.HCAREAMOUNT + regpay.EXPENSEAMOUNT;//
                decimal secost = regpay.SELFAMOUNT;//
                decimal ecost = regpay.ECOSTAMOUNT;

                patient.PubDigFee = (hccost + excost) / 100;

                patient.OwnDigFee = regpay.SELFAMOUNT / 100;
                //减免信息。
                //patient.RegNo = "";
                patient.RegDiagCode = "";
                patient.Ecost = regpay.ECOSTAMOUNT / 100;


                if (regpay.PAYMODE == "7")
                {
                    patient.OwnDigFee = 0;
                }
                if (regpay.PAYMODE == "4" && (insuplcAdmdvs.StartsWith("4403") || insuplcAdmdvs.StartsWith("4415") || !insuplcAdmdvs.StartsWith("44")))//虚账个账处理
                {
                    patient.PayType = Common.Function.SetPayType("1");
                    patient.PubDigFee = patient.OwnDigFee + patient.PubDigFee;
                    patient.OwnDigFee = 0;
                }
                #endregion

                #region 更新号源  预约挂号全都预扣号源
                string msg = "";

                //if (order.REGTYPE == "1")
                //{
                //    string updateLmtSql = string.Format(FS.ZDWY.Internet.BP.OutPatient.Register.Sql.UpdateSchemaReged, order.SCHEDULEID, "1");

                //    int rt = mgr.ExecuteSql(updateLmtSql, ref msg);
                //    if (rt <= 0)
                //    {
                //        throw new Exception("挂号失败，当前时段号源已被抢完，请选后一时段排班挂号！");
                //    }
                //}

                #endregion

                #region 获取seeNo
                //当日挂号
                if (order.REGTYPE == "1")
                {
                    dt = new System.Data.DataTable();
                    DataTable dt2 = new DataTable();
                    {
                        //为医生排班

                        //min最小看诊序号，seeNO当前看诊序号，cnt当前排班限额
                        int minNo = -1, seeNo = 0, cnt = 0, Residue = 0;

                        if (mgr.GetMinSeeNo(patient.SchemaID, ref minNo) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            result.IsSuccessful = false;
                            result.Message = mgr.Err;
                            return result;
                        }
                        if (mgr.GetCurrentSeeNo(patient.SchemaID, ref seeNo) == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            result.IsSuccessful = false;
                            result.Message = mgr.Err;
                            return result;
                        }


                        if (minNo < 1)
                        {
                            throw new Exception("取出最小看诊序号不正确，排班ID：" + patient.SchemaID.ToString());
                        }
                        if (mgr.GetSourceCount(patient.SchemaID, ref cnt) != -1)
                        {
                            mgr.GetResidue(patient.SchemaID, ref Residue);
                            if (Residue >= cnt)
                            {
                                throw new Exception("已经没有足够号源可以，请选择其他时段排班");
                            }
                        }

                        if ((patient.RegLevel.ID != "4") && (seeNo == 0 || seeNo < minNo))//RegLevel.ID==4是急诊，seeNo==0为排班当天第一个挂号，seeNo<minNo 为上一时段未挂完的号，时段过了，则从下一个时段最小序号开始
                        {
                            seeNo = minNo;
                        }
                        else
                        {
                            seeNo = seeNo + 1;
                        }

                        patient.SeeNO = seeNo;
                    }
                }
                else
                {
                    patient.SeeNO = 0;
                }
                #endregion
                if (order.REGTYPE == "0")
                {
                    patient.Isbooking = "1";
                }
                #region 更新排班表，插入号源表
                //插入挂号主表
                string insertReg = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.insertReg;
                //插入挂号费用表 挂号费
                string insertRegFee = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.insertRegFee;
                //插入挂号费用表 诊查费
                string insertDiagFee = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.insertRegFee;
                //插入挂号费用表 优惠金额
                string insertEcost = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.insertRegFee;
                //插入护士分诊记录表
                string insertAssignRecord = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.insertAssignRecord;
                //插入交易记录表
                //string InsertTradeRecords = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.InsertTradeRecords;
                //更新护士分诊队列表
                string updateNurQueue = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.updateNurQueues;
                //更新com_Dictionary发票信息
                string updatecomDictionarySql = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.updatecomDictionary;
                //更新占用状态
                string updateShemaLockState = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.UpdateRegLockState;
                //跟新看诊序号
                string setseeno = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.SetSeeNo;
                //更新预约
                string updatebook = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.updatebooking;



                ArrayList sqlList = new ArrayList();
                if (patient.Doct.ID == "None")
                {
                    patient.Doct.ID = string.Empty;
                }


                #region 获取交易记录信息
                #endregion
                if (patient.RegLevel.Name == null || patient.RegLevel.Name.Trim() == "")
                {
                    patient.RegLevel.Name = mgr.GetReglevlName(patient.RegLevel.ID);
                }
                patient.InformedConsentResult = informedConsentResult;
                string[] argm = mgr.GetRegInfo(patient);
                string[] regFeeArgm = mgr.GetRegFeeInfo(patient);
                string[] diagFeeArgm = mgr.GetDiagFeeInfo(patient);// this.GetDiagFeeInfo(patient);
                //string[] ecostArgm = mgr.GetEcostFeeInfo(patient);
                string[] assignRecordArgm = mgr.GetAssignRecordInfo(patient);
                //string[] tradeRecordsArgm = Function.GetTradeRecordsInfo(recordsInfo);



                insertReg = string.Format(insertReg, argm);
                insertRegFee = string.Format(insertRegFee, regFeeArgm);
                insertDiagFee = string.Format(insertDiagFee, diagFeeArgm);
                //insertEcost = string.Format(insertEcost, ecostArgm);
                //InsertTradeRecords = string.Format(InsertTradeRecords, tradeRecordsArgm);
                insertAssignRecord = string.Format(insertAssignRecord, assignRecordArgm);
                updateNurQueue = string.Format(updateNurQueue, patient.Queue.ID);
                updatebook = string.Format(updatebook, regpay.HOSPITALNUM, "1", "1", patient.ClinicCode);
                //updateShemaLockState = string.Format(updateShemaLockState, opr.TranSerNo, regpay.CurrentOpert.Code, "3");

                string InsertSISql = string.Empty;//处理诊金减免的sql
                string InsertGDSIinfo = string.Empty;//省集中平台的sql

                sqlList.Add(insertReg);//挂号主表
                sqlList.Add(insertRegFee);//挂号费插入fin_opb_accountcardfee
                sqlList.Add(insertDiagFee);//诊查费插入fin_opb_accountcardfee
                //sqlList.Add(insertEcost);//
                //sqlList.Add(InsertTradeRecords);//交易记录表插入数据
                if (order.REGTYPE == "1")
                {
                    sqlList.Add(insertAssignRecord);//护士分诊记录表met_nuo_assignrecord
                }
                //sqlList.Add(updateNurQueue);
                //sqlList.Add(updateShemaLockState);//更新自助锁号表
                sqlList.Add(updatebook);//更新预约挂号

                //长者券合同单位，需要插入一条诊查费到门诊费用表
                if (regpay.PAYMODE == "7")
                {
                    //根据挂号级别获取需要插入的项目编码
                    string itemCode = string.Empty;
                    int ret = mgr.getRegItemCode(patient.RegLevel.ID, ref itemCode);
                    if (ret < 0)
                    {
                        throw new Exception("获取诊疗项目出错！");
                    }
                    string itemName = mgr.GetItemNameForItemCode(itemCode);
                    if (string.IsNullOrEmpty(itemName))
                        throw new Exception("获取诊疗项目名称出错！");
                    string itemPrice = mgr.GetPriceForItemCode(itemCode);
                    if (string.IsNullOrEmpty(itemPrice))
                        throw new Exception("获取诊疗项目价格出错！");
                    string insertRegFeeDetail = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.insertRegFeeDetail;
                    string[] regFeeDetail = mgr.GetRegFeeDetailInfo(patient, itemCode, itemName, itemPrice);
                    insertRegFeeDetail = string.Format(insertRegFeeDetail, regFeeDetail);
                    sqlList.Add(insertRegFeeDetail);
                }
                #region 处理发票

                if (patient.IsUseingInvoice)
                {
                    //使用在用的发票组
                    if (patient.EndInvoice == patient.NextRealInvoice)
                    {
                        //如果结束发票号=下一张发票号，说明该发票组已经用完了，更新使用标识为-1，并找到下一组发票更新使用标识为1，更新COM_DICTIONARY
                        string updateComInvoiceSql1 = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.updateComInvoice;
                        string updateComInvoiceSql2 = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.updateComInvoiceNew;
                        string starInvoice = string.Empty;
                        string invoiceGetTime = string.Empty;
                        mgr.GetUnUseInvoice(regpay.OPERCODE, ref starInvoice, ref invoiceGetTime);
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, regpay.OPERCODE, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "-1");
                        //更新新发票组
                        updateComInvoiceSql2 = string.Format(updateComInvoiceSql2, regpay.OPERCODE, patient.RealInvoice, "1", invoiceGetTime);

                        updatecomDictionarySql = string.Format(updatecomDictionarySql, regpay.OPERCODE, starInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updateComInvoiceSql2);
                        sqlList.Add(updatecomDictionarySql);
                    }
                    else
                    {
                        string updateComInvoiceSql1 = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.updateComInvoice;
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, regpay.OPERCODE, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                        updatecomDictionarySql = string.Format(updatecomDictionarySql, regpay.OPERCODE, patient.NextRealInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updatecomDictionarySql);

                    }
                }
                else
                {
                    string updateComInvoiceSql1 = FS.ZDWY.Internet.BP.OutPatient.Register.Sql.updateComInvoice;
                    //更新旧发票组
                    updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, regpay.OPERCODE, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                    updatecomDictionarySql = string.Format(updatecomDictionarySql, regpay.OPERCODE, patient.NextRealInvoice, patient.NextInvoiceStr);

                    sqlList.Add(updateComInvoiceSql1);
                    sqlList.Add(updatecomDictionarySql);
                }

                #endregion

                for (int i = 0; i < sqlList.Count; i++)
                {
                    if (mgr.ExecuteSql(sqlList[i].ToString(), ref msg) == -1)
                    {
                        throw new Exception("挂号登记失败！" + msg);
                    }
                }

                order.REGISTERID = patient.ClinicCode;
                order.STATUS = "2";
                if (!platformOrderLogic.Update(order))
                {
                    throw new Exception("挂号登记失败！更新订单状态失败！");
                }
                string visitAddressSql = @"select (select n.remark||n.room_name from MET_NUO_CONSOLE n where n.console_code=m.console_code and rownum=1) 
from fin_opr_schema m
where m.id='{0}'";
                visitAddressSql = string.Format(visitAddressSql, patient.SchemaID);
                string visitAddress = mgr.ExecSqlReturnOne(visitAddressSql, "");
                Models.Views.OutPatient.PayResult res = new Models.Views.OutPatient.PayResult();
                res.HospTradeId = patient.InvoiceStr;
                res.InvoiceId = patient.InvoiceStr;
                res.ReceiptId = patient.InvoiceStr;
                res.VisitAddress = visitAddress;
                res.VisitNo = patient.SeeNO.ToString();
                res.Proof = patient.SeeNO.ToString();
                res.Remark = "";
                res.ClinicCode = patient.ClinicCode;


                result.IsSuccessful = true;
                result.ReturnData = res;

                Models.PLATFORM_REGISTER_PAY orderpay = new Models.PLATFORM_REGISTER_PAY();
                regpay.RegisterID = patient.ClinicCode;
                regpay.PsRefOrdNum = "";//平台退款订单号
                regpay.HospTradeId = patient.InvoiceStr;
                regpay.PayRefTime = mgr.GetDateTimeFromSysDateTime();
                regpay.RefundReason = "";
                regpay.RefundOpercode = "";
                regpay.RefundOpername = "";

                if (!platformPayLogic.Insert(regpay))
                {
                    throw new Exception("插入平台结算数据失败！");
                }

                platformPayLogic.CommitTran();
                platformOrderLogic.CommitTran();
                Neusoft.FrameWork.Management.PublicTrans.Commit();
                return result;
                #endregion
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                result.IsSuccessful = false;
                result.Message = ex.Message;
                return result;

            }

        }


        /// <summary>
        /// 订单退费
        /// </summary>
        /// <returns></returns>
        public Models.Views.ComResult<Models.PLATFORM_REGISTER_PAY> RegisterBackPay(string orderId, string psRefOrdNum, string hospitalNum, string hospTradeId, string refundReason, string transno, string payChannel)
        {
            Models.Views.ComResult<Models.PLATFORM_REGISTER_PAY> result = new Models.Views.ComResult<Models.PLATFORM_REGISTER_PAY>();
            #region 事务开始
            //挂号管理类
            Neusoft.HISFC.BizLogic.Registration.Register regMgr = new Neusoft.HISFC.BizLogic.Registration.Register();
            //排班管理类
            Neusoft.HISFC.BizLogic.Registration.Schema schMgr = new Neusoft.HISFC.BizLogic.Registration.Schema();
            //费用
            //Neusoft.HISFC.BizLogic.Fee.Account accMgr = new Neusoft.HISFC.BizLogic.Fee.Account();

            FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic platformOrderLogic = new BL.OutPatient.PlatformOrderLogic();
            FS.ZDWY.Internet.BL.OutPatient.RegisterPayInfoLogic platformPayLogic = new BL.OutPatient.RegisterPayInfoLogic();
            BL.OutPatient.BookingLogic bookingLogic = new BL.OutPatient.BookingLogic();
            OutPatient.Register.Manager mgr = new OutPatient.Register.Manager();

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            regMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            schMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            mgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            platformOrderLogic.BeginTran();
            platformPayLogic.BeginTran();
            bookingLogic.BeginTran();
            #endregion
            try
            {
                #region 获取订单号，并校验  booking,order,orderpay

                FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER order = platformOrderLogic.Get(orderId);
                if (order == null || string.IsNullOrEmpty(order.ORDERID))
                {
                    throw new Exception("预约订单信息不正确");
                }

                if (order.CLINIC_CODE != hospitalNum)
                {
                    throw new Exception("系统订单号与入参医院订单号不符！");
                }

                FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY orderpay = new Models.PLATFORM_REGISTER_PAY();
                orderpay = platformPayLogic.Get(orderId);

                if (order.STATUS == "4")
                {
                    if (orderpay != null)
                    {
                        result.IsSuccessful = true;
                        result.Message = "患者已退费！退费订单号：" + orderpay.PsRefOrdNum;
                        return result;
                    }
                    else
                    {
                        result.IsSuccessful = true;
                        return result;
                    }
                }


                FS.ZDWY.Internet.Models.FIN_OPR_BOOKING booking = bookingLogic.Get(hospitalNum);
                if (booking == null)
                {
                    throw new Exception("获取对应预约订单信息失败");
                }
                if (string.IsNullOrEmpty(booking.CLINIC_CODE))
                {
                    throw new Exception("获取对应预约订单信息失败");
                }

                List<Models.FIN_OPR_REGISTER> reglist = null;
                Models.FIN_OPR_REGISTER regobj = null;
                if (!string.IsNullOrEmpty(booking.REG_ID))
                {
                    FS.ZDWY.Internet.BL.RegisterInfoLogic reglog = new FS.ZDWY.Internet.BL.RegisterInfoLogic();
                    reglist = reglog.GetList(o => o.CLINIC_CODE == booking.REG_ID);
                    if (reglist == null || reglist.Count == 0)
                    {
                        throw new Exception("查询患者信息信息出错！");
                    }
                    if (reglist.Exists(o => o.VALID_FLAG == "0"))
                    {
                        throw new Exception("患者已退费！");
                    }
                    regobj = reglist.Find(o => o.VALID_FLAG == "1");
                }
                else
                {
                    throw new Exception("该订单未取号！");
                }
                #endregion

                int rtn;
                Neusoft.HISFC.BizLogic.Registration.EnumUpdateStatus flag = Neusoft.HISFC.BizLogic.Registration.EnumUpdateStatus.Cancel;

                #region his业务

                DateTime current = regMgr.GetDateTimeFromSysDateTime();

                Neusoft.HISFC.Models.Registration.Register reg = regMgr.GetByClinic(booking.REG_ID);

                Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParams = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

                string cardRule = controlParams.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.NO_REG_CARD_RULES, false, "9");
                if (reg.PID.CardNO != "" && reg.PID.CardNO != string.Empty)
                {
                    if (reg.PID.CardNO.Substring(0, 1) == cardRule)
                    {
                        throw new Exception("此号段为直接收费使用，不可以退号");
                    }
                }

                //出错
                if (reg == null || reg.ID == null || reg.ID == "")
                {
                    throw new Exception(regMgr.Err);
                }

                if (reg.IsSee)
                {
                    throw new Exception("该号已经看诊,不能作废!");
                }

                //是否已经退号
                if (reg.Status == Neusoft.HISFC.Models.Base.EnumRegisterStatus.Back)
                {
                    throw new Exception("该挂号记录已经退号，不能再次退号!");
                }

                //是否已经作废
                if (reg.Status == Neusoft.HISFC.Models.Base.EnumRegisterStatus.Cancel)
                {
                    throw new Exception("该挂号记录已经作废，不能进行退号!");
                }

                #region 退费
                //查询数据
                List<Neusoft.HISFC.Models.Account.AccountCardFee> lstCardFee = null;
                int iRes = mgr.QueryAccountCardFeeByInvoiceNO(orderpay.HospTradeId, out lstCardFee);
                if (lstCardFee == null || lstCardFee.Count == 0)
                {
                    throw new Exception("未检索到患者相关发票信息");
                }

                #region 作废信息

                iRes = mgr.CancelAccountCardFeeByInvoice(orderpay.HospTradeId, 0);
                if (iRes <= 0)
                {
                    throw new Exception("作废收费项目失败！" + mgr.Err);
                }
                #endregion
                #region 插入负记录

                Neusoft.HISFC.Models.Account.AccountCardFee cardFee = null;
                if (lstCardFee.Count > 0)
                {
                    //for (int idx = 0; idx < lstCardFee.Count; idx++)
                    foreach (Neusoft.HISFC.Models.Account.AccountCardFee obj in lstCardFee)
                    {
                        cardFee = obj.Clone();
                        cardFee.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                        cardFee.ID = orderpay.OPERCODE;
                        cardFee.Name = orderpay.OPERNAME;
                        cardFee.Oper.OperTime = current;
                        cardFee.Tot_cost = -cardFee.Tot_cost;
                        cardFee.Own_cost = -cardFee.Own_cost;
                        cardFee.Pub_cost = -cardFee.Pub_cost;
                        cardFee.Pay_cost = -cardFee.Pay_cost;
                        cardFee.IStatus = (int)Neusoft.HISFC.Models.Base.EnumRegisterStatus.Back;

                        iRes = mgr.InsertAccountCardFee(cardFee);
                        if (iRes <= 0)
                        {
                            throw new Exception("退费失败，插入退费记录失败！" + mgr.Err);
                        }
                    }
                }

                #endregion

                #endregion

                #region 退号
                Neusoft.HISFC.Models.Registration.Register objReturn = reg.Clone();
                objReturn.RegLvlFee.ChkFee = -reg.RegLvlFee.ChkFee;//检查费
                objReturn.RegLvlFee.OwnDigFee = -reg.RegLvlFee.OwnDigFee;//侦察费


                objReturn.RegLvlFee.OthFee = -reg.RegLvlFee.OthFee;//其他费
                objReturn.RegLvlFee.RegFee = -reg.RegLvlFee.RegFee;//挂号费
                objReturn.PayCost = -reg.PayCost;
                objReturn.OwnCost = -reg.OwnCost;
                objReturn.PubCost = -reg.PubCost;
                objReturn.BalanceOperStat.IsCheck = false;//是否结算
                objReturn.BalanceOperStat.ID = "";
                objReturn.BalanceOperStat.Oper.ID = "";
                //objReturn.BeginTime = DateTime.MinValue; 
                objReturn.CheckOperStat.IsCheck = false;//是否核查
                objReturn.Status = Neusoft.HISFC.Models.Base.EnumRegisterStatus.Back;//退号
                objReturn.InputOper.OperTime = current;//操作时间
                if (payChannel == "")
                    objReturn.InputOper.ID = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                objReturn.CancelOper.ID = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                if (payChannel == "2") //支付渠道为支付宝
                {
                    objReturn.InputOper.ID = FS.ZDWY.Internet.BP.Common.Function.ZFBOper.Code;
                    objReturn.InputOper.ID = FS.ZDWY.Internet.BP.Common.Function.ZFBOper.Code;
                }
                else if (payChannel == "3") //支付渠道为APP
                {
                    objReturn.InputOper.ID = FS.ZDWY.Internet.BP.Common.Function.APPOper.Code;
                    objReturn.InputOper.ID = FS.ZDWY.Internet.BP.Common.Function.APPOper.Code;
                }
                objReturn.CancelOper.OperTime = current;//退号时间
                //{F3258E87-7BCC-411a-865E-A9843AD2C6DD}
                //objReturn.OwnCost = -reg.OwnCost;//自费
                //objReturn.PayCost = -reg.PayCost;
                objReturn.PubCost = -reg.PubCost;

                //if (this.otherFeeType == "1" && !this.chbQuitFeeBookFee.Checked)
                //{
                //    objReturn.OwnCost = objReturn.OwnCost - objReturn.RegLvlFee.OthFee;
                //    objReturn.RegLvlFee.OthFee = 0;
                //}

                objReturn.TranType = Neusoft.HISFC.Models.Base.TransTypes.Negative;

                if (regMgr.Insert(objReturn) <= 0)
                {
                    throw new Exception(regMgr.Err);
                }

                flag = Neusoft.HISFC.BizLogic.Registration.EnumUpdateStatus.Return;
                #endregion

                reg.CancelOper.ID = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                if (payChannel == "2") //支付渠道为支付宝
                {
                    reg.CancelOper.ID = FS.ZDWY.Internet.BP.Common.Function.ZFBOper.Code;
                }
                else if (payChannel == "3") //支付渠道为APP
                {
                    reg.CancelOper.ID = FS.ZDWY.Internet.BP.Common.Function.APPOper.Code;
                }
                reg.CancelOper.OperTime = current;

                //更新原来项目为作废
                rtn = regMgr.Update(flag, reg);
                if (rtn == -1)
                {
                    throw new Exception(regMgr.Err);
                }
                if (rtn == 0)
                {
                    throw new Exception("该挂号信息状态已经变更, 请重新检索数据");
                }

                #region 恢复限额
                //恢复原来排班限额
                //如果原来更新限额,那么恢复限额
                //if (reg.DoctorInfo.Templet.ID != null && reg.DoctorInfo.Templet.ID != "")
                //{
                //    //现场号、预约号、特诊号

                //    bool IsReged = false, IsTeled = false, IsSped = false;

                //if (reg.RegType == Neusoft.HISFC.Models.Base.EnumRegType.Pre)
                //{
                //    IsTeled = true; //预约号
                //}
                //else if (reg.RegType == Neusoft.HISFC.Models.Base.EnumRegType.Reg)
                //{
                //    if (reg.DoctorInfo.SeeDate > current)
                //    {
                //        IsTeled = true;//预约号
                //    }
                //    else
                //    {
                //        IsReged = true;//现场号
                //    }
                //}
                //else
                //{
                //    IsSped = true;//特诊号
                //}

                //rtn = schMgr.Reduce(reg.DoctorInfo.Templet.ID, IsReged, false, IsTeled, IsSped);
                //    if (rtn == -1)
                //    {
                //        throw new Exception(schMgr.Err);
                //    }

                //    if (rtn == 0)
                //    {
                //        throw new Exception("已无排班信息, 无法恢复限额");
                //    }
                //}


                string MaxSchemaSql = @"select  ( case WHEN dept_code in （select code from com_dictionary  where type ='SendbackSource' and mark='2'  ）
                                      THEN id ELSE    NVL(LEAD(id) OVER (ORDER BY sort_id), id ) END)
                                      from ( select  row_number() over (order by b.end_time asc) as sort_id,b.* from fin_opr_schema a
                                      join fin_opr_schema b  on a.see_date=b.see_date
                                      and a.schema_type=b.schema_type 
                                      and a.noon_code=b.noon_code
                                      and a.week=b.week and a.dept_code=b.dept_code
                                      and a.doct_code=b.doct_code
                                      and a.console_code=b.console_code
                                      AND B.BEGIN_TIME >= a.BEGIN_TIME
                                      where a.id='{0}'
                                      order by a.end_time asc
                                      ) x    where  x.end_time >=sysdate ";
                MaxSchemaSql = string.Format(MaxSchemaSql, order.SCHEDULEID);
                string maxId = mgr.ExecSqlReturnOne(MaxSchemaSql);
                string IESchemaSql = @"select  (CASE WHEN  end_time >sysdate 
                                       and dept_code not in(select code from com_dictionary p where type = 'SendbackSource'and  mark='1') then '1' else '0'end )
                                       from  fin_opr_schema   where  id='{0}'   ";
                IESchemaSql = string.Format(IESchemaSql, maxId);
                string IEId = mgr.ExecSqlReturnOne(IESchemaSql);
                string IESchemaLogSql = @"   INSERT INTO FIN_OPR_SCHEMALOG
                                            (ID, MAXID, CLINIC_CODE, REGTYPE, OPER_DATE)
                                             VALUES   ('{0}', '{1}', '{2}','{3}', sysdate)";
                if (order.REGTYPE == "1")
                {
                    if (!string.IsNullOrEmpty(maxId) && (IEId == "1"))
                    {
                        string updateSql = @"update fin_opr_schema a
                                            set a.reg_lmt=a.reg_lmt-1
                                            where a.id='{0}' ";
                        string updateSql2 = @"update fin_opr_schema a
                                            set a.reg_lmt=a.reg_lmt+1 
                                            where a.id='{0}' ";
                        updateSql = string.Format(updateSql, order.SCHEDULEID);
                        int x = mgr.ExecNoQuery(updateSql);
                        updateSql2 = string.Format(updateSql2, maxId);
                        int y = mgr.ExecNoQuery(updateSql2);

                        string updateSchema = @"update fin_opr_schema s --医师出诊表
                                                set s.reged = s.reged + {1}--已挂号
                                                where s.id = '{0}'";
                        updateSchema = string.Format(updateSchema, order.SCHEDULEID, "-1");
                        int res = mgr.ExecNoQuery(updateSchema);
                        if (res <= 0)
                        {
                            throw new Exception("解锁号源失败！");
                        }
                        IESchemaLogSql = string.Format(IESchemaLogSql, order.SCHEDULEID, maxId, booking.REG_ID, order.REGTYPE);
                        mgr.ExecNoQuery(IESchemaLogSql);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(maxId) && (IEId == "1"))
                    {
                        string updateSql2 = @"update fin_opr_schema a
                                            set a.reg_lmt=a.reg_lmt+1 
                                            where a.id='{0}' and   to_char(END_TIME,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD') ";
                        updateSql2 = string.Format(updateSql2, maxId);
                        int y = mgr.ExecNoQuery(updateSql2);
                        string updateSchema = @"update fin_opr_schema s --医师出诊表
                                           set s.tel_reging = s.tel_reging + {1}, --预约已约
                                               s.tel_reged  = s.tel_reged + {1},-- 预约已挂
                                               s.TEL_LMT =DECODE(to_char(s.END_TIME,'YYYY-MM-DD'), TO_CHAR(SYSDATE,'YYYY-MM-DD'), s.TEL_LMT+ {1}, s.TEL_LMT)
                                         where s.id = '{0}' ";
                        updateSchema = string.Format(updateSchema, order.SCHEDULEID, "-1");
                        int res = mgr.ExecNoQuery(updateSchema);
                        if (res <= 0)
                        {
                            throw new Exception("解锁号源失败！");
                        }
                        IESchemaLogSql = string.Format(IESchemaLogSql, order.SCHEDULEID, maxId, booking.REG_ID, order.REGTYPE);
                        mgr.ExecNoQuery(IESchemaLogSql);

                    }
                    else
                    {
                        string updateSchema = @"update fin_opr_schema s --医师出诊表
                                           set s.tel_reging = s.tel_reging + {1}, --预约已约
                                               s.tel_reged  = s.tel_reged + {1}-- 预约已挂
                                         where s.id = '{0}' ";
                        updateSchema = string.Format(updateSchema, order.SCHEDULEID, "-1");
                        int res = mgr.ExecNoQuery(updateSchema);
                        if (res <= 0)
                        {
                            throw new Exception("解锁号源失败！");
                        }
                    }
                }
                //else //by yhm 信息科张群群 提出微信可以当日退号，开关由微信平台控制
                //{
                //    throw new Exception("当日挂号无法线上退费！");
                //}
                #endregion

                Neusoft.HISFC.Models.Registration.Register cancelreg = reg.Clone();
                cancelreg.SIMainInfo.RegNo = "";
                string ydzfOrderID = string.Empty;//移动支付订单号 如果是移动支付不走原医保流程
                mgr.GetGHYDZFOrderID(orderId, ref ydzfOrderID);
                if (mgr.GetRegSIPersonInfo(booking.REG_ID, ref cancelreg) > 0)
                {
                    if (!string.IsNullOrEmpty(cancelreg.SIMainInfo.RegNo) && string.IsNullOrEmpty(ydzfOrderID))
                    {
                        //新医保退号
                        Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
                        GDSI.ZhuHaiSI.Business.Comom.MedicalService ms = new GDSI.ZhuHaiSI.Business.Comom.MedicalService();
                        if (ms.CancelRegSettlement(cancelreg.ID, "00A105", "微信", "3") < 0)
                        {

                            throw new Exception(ms.ErrorMessage);
                        }

                        //if (mgr.UpdateSIPersonInfo(cancelreg.ID) <= 0)
                        //{
                        //    throw new Exception("更新医保信息失败！" + mgr.Err + cancelreg.ID);
                        //}



                        //#region 医保退号

                        //FS.ZDWY.Internet.BP.SI.OutPatient.GetCancelFeeInfo cancelfeeinfo = new SI.OutPatient.GetCancelFeeInfo();
                        //string erro = "";
                        //if (cancelfeeinfo.CallService(cancelreg.SIMainInfo.RegNo, ref erro, cancelreg.SIMainInfo.Bka006) < 0)
                        //{
                        //    result.Message += "收费时提取门诊业务信息！原因：" + cancelfeeinfo.ErrorMsg;
                        //}


                        //object[] objfee = new object[] { "", booking.REGLEVL_CODE, booking.DOCT_CODE };
                        //FS.ZDWY.Internet.BP.SI.OutPatient.CancelFee cancelfee = new SI.OutPatient.CancelFee();
                        //if (cancelfee.CallService(cancelreg, ref erro, objfee) < 0)
                        //{
                        //    result.Message += "取消费用信息！原因：" + cancelfee.ErrorMsg;
                        //}

                        //FS.ZDWY.Internet.BP.SI.OutPatient.GetFeeInfo feeinfo = new SI.OutPatient.GetFeeInfo();

                        //if (feeinfo.CallService(cancelreg.SIMainInfo.RegNo, ref erro, cancelreg.SIMainInfo.Bka006) < 0)
                        //{
                        //    throw new Exception("收费时提取门诊业务信息！原因：" + feeinfo.ErrorMsg);
                        //}

                        ////if (!string.IsNullOrWhiteSpace(transno))
                        ////{
                        //FS.ZDWY.Internet.BP.SI.OutPatient.CancelRegister cancel = new SI.OutPatient.CancelRegister();
                        //GDSI.Models.OutParam.OutParamBizh110106 outParam = new GDSI.Models.OutParam.OutParamBizh110106();
                        //object[] obj = new object[] { "", cancelreg.SIMainInfo.RegNo };
                        //if (cancel.CallService("", ref outParam, obj) <= 0)
                        //{
                        //    throw new Exception("取消医保挂号失败！原因：" + cancel.ErrorMsg);
                        //}
                        ////}
                        ////else
                        ////{
                        ////throw new Exception("医保挂号患者，请传入tranno！");
                        ////}


                        //#endregion
                    }
                }


                #endregion

                //Models.PLATFORM_REGISTER_PAY res = new Models.PLATFORM_REGISTER_PAY();
                orderpay.PsRefOrdNum = psRefOrdNum;
                orderpay.PayRefTime = regMgr.GetDateTimeFromSysDateTime();
                orderpay.RefundReason = refundReason;
                orderpay.RefundOpercode = order.OPERCODE;
                orderpay.RefundOpername = order.OPERNAME;
                if (!platformPayLogic.Update(orderpay))
                {
                    throw new Exception("记录退费信息失败！");
                }

                order.STATUS = "4";
                if (!platformOrderLogic.Update(order))
                {
                    throw new Exception("更新订单状态失败！");
                }

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                platformOrderLogic.CommitTran();
                platformPayLogic.CommitTran();
                bookingLogic.CommitTran();

                result.IsSuccessful = true;
                return result;
            }
            catch (Exception e)
            {
                //事务回滚
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                platformOrderLogic.RollbackTran();
                platformPayLogic.RollbackTran();
                bookingLogic.RollbackTran();
                result.IsSuccessful = false;
                result.Message = e.Message.ToString();
                return result;
            }
        }

        /// <summary>
        /// 挂号减免
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="hospitalNum"></param>
        /// <param name="transno"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public Models.Views.ComResult<Models.Views.OutPatient.HcareResult> Hcare(string orderId, string hospitalNum, string transno, string patientId, FS.ZDWY.Internet.Models.HcareInModel hcareModel, string settlementType)
        {
            Models.Views.ComResult<Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();
            try
            {
                #region 获取订单号，并校验  booking,order
                string orderType = string.Empty;
                string schemaID = string.Empty;

                BL.OutPatient.BookingLogic bookingLogic = new BL.OutPatient.BookingLogic();
                FS.ZDWY.Internet.Models.FIN_OPR_BOOKING booking = bookingLogic.Get(hospitalNum);
                if (booking == null)
                {
                    result.IsSuccessful = false;
                    result.Message = "获取对应预约订单信息失败";
                    return result;
                }
                if (string.IsNullOrEmpty(booking.CLINIC_CODE))
                {
                    result.IsSuccessful = false;
                    result.Message = "获取对应预约订单信息失败";
                    return result;
                }

                FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic platformOrderLogic = new BL.OutPatient.PlatformOrderLogic();
                FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER order = platformOrderLogic.Get(orderId);
                if (order == null || string.IsNullOrEmpty(order.ORDERID))
                {
                    result.IsSuccessful = false;
                    result.Message = "预约订单信息不正确（patientId）";
                    return result;
                }
                #endregion

                if (order.STATUS == "3")
                {
                    result.IsSuccessful = false;
                    result.Message = "订单已作废！";
                    return result;
                }

                if (order.STATUS != "1")
                {
                    result.IsSuccessful = false;
                    result.Message = "订单已缴费！";
                    return result;
                }

                OutPatient.Register.Manager mgr = new OutPatient.Register.Manager();
                //Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                //mgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                DateTime now = mgr.GetDateTimeFromSysDateTime();

                #region 获取患者基本信息
                string compatientSql = OutPatient.Register.Sql.GetPatientInfo;
                compatientSql = string.Format(compatientSql, order.CARDNO);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = mgr.GetDataTable(compatientSql);
                Models.Views.OutPatient.ComPatient patient = null;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient = new Models.Views.OutPatient.ComPatient();
                            patient.CardNo = dt.Rows[i][0].ToString();
                            patient.Name = dt.Rows[i][1].ToString();
                            patient.Birthday = dt.Rows[i][2].ToString();
                            patient.SexCode = dt.Rows[i][3].ToString();
                            patient.IDCard = dt.Rows[i][4].ToString();
                            patient.McardNo = dt.Rows[i][5].ToString();
                            patient.HomePhone = dt.Rows[i][6].ToString();
                            patient.Address = dt.Rows[i][7].ToString();
                            patient.RegDate = now;
                            break;
                        }
                        if (patient == null || string.IsNullOrEmpty(patient.CardNo))
                        {
                            throw new Exception("获取患者信息出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有找到患者信息！");
                    }
                }
                else
                {
                    throw new Exception("没有找到患者信息！");
                }
                patient.Oper.ID = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                patient.Oper.Name = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Name;
                #endregion
                #region 获取排班信息

                string schemaSql = OutPatient.Register.Sql.GetSchema;

                schemaSql = string.Format(schemaSql, order.SCHEDULEID);
                dt = new System.Data.DataTable();
                dt = mgr.GetDataTable(schemaSql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.SchemaID = dt.Rows[i][0].ToString();
                            patient.SchemaType = dt.Rows[i][1].ToString();//排班类型，0科室/1医生
                            patient.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][2].ToString());
                            patient.Noon.ID = dt.Rows[i][4].ToString();
                            patient.Dept.ID = dt.Rows[i][5].ToString();
                            patient.Dept.Name = dt.Rows[i][6].ToString();
                            patient.Doct.ID = dt.Rows[i][7].ToString();
                            patient.Doct.Name = dt.Rows[i][8].ToString();
                            patient.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][20].ToString());
                            patient.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][21].ToString());
                            patient.RegLevel.ID = dt.Rows[i][29].ToString();
                            patient.RegLevel.Name = dt.Rows[i][30].ToString();
                            patient.Room.ID = dt.Rows[i][31].ToString();
                            patient.Room.Name = dt.Rows[i][32].ToString();
                            patient.Console.ID = dt.Rows[i][33].ToString();
                            patient.Console.Name = dt.Rows[i][34].ToString();
                            break;
                        }
                        if (string.IsNullOrEmpty(patient.SchemaID))
                        {
                            throw new Exception("获取排班信息出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有找到排班信息！");
                    }
                }
                else
                {
                    throw new Exception("没有找到排班信息！");
                }

                #endregion
                #region 获取挂号等级费用
                string regfeeSql = OutPatient.Register.Sql.GetRegFee;
                if (booking.APP_FLAG == "1")
                {
                    regfeeSql = string.Format(regfeeSql, "1", booking.REGLEVL_CODE);
                }
                else
                {
                    regfeeSql = string.Format(regfeeSql, "1", patient.RegLevel.ID);
                }
                dt = new System.Data.DataTable();
                dt = mgr.GetDataTable(regfeeSql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][4]);//挂号费
                            patient.OwnDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][6]);//诊查费
                            break;
                        }
                        if (patient.OwnDigFee == null || string.IsNullOrEmpty(patient.OwnDigFee.ToString()))
                        {
                            throw new Exception("获取费用信息出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("获取费用信息出错！");
                    }
                }
                else
                {
                    throw new Exception("获取费用信息出错！");
                }

                #endregion

                string str = ((int)((patient.RegFee + patient.OwnDigFee) * 100)).ToString();
                if (str != order.REGFEE)
                {
                    throw new Exception("获取费用不符合！");
                }

                #region 在职人员判断
                #endregion

                GDSI.OutpatientWebService.ClinicBalanceResponseModel outModel2207 = new GDSI.OutpatientWebService.ClinicBalanceResponseModel();
                GDSI.ZhuHaiSI.Model.RegSettlementInModel inModel = new GDSI.ZhuHaiSI.Model.RegSettlementInModel();
                Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";

                if (!string.IsNullOrWhiteSpace(hcareModel.IsMedical))
                {
                    #region 医保处理
                    string errorMessage = string.Empty;
                    string clinicCodeSql = OutPatient.Register.Sql.GetClinicCode;
                    #region 获取门诊流水号

                    dt = new System.Data.DataTable();
                    dt = mgr.GetDataTable(clinicCodeSql);

                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            if (!Convert.IsDBNull(dt.Rows[0][0]))
                            {
                                patient.ClinicCode = dt.Rows[0][0].ToString();
                            }
                            else
                            {
                                throw new Exception("获取门诊流水号出错！");
                            }
                        }
                        else
                        {
                            throw new Exception("获取门诊流水号出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("获取门诊流水号出错！");
                    }

                    #endregion
                    inModel.ClincCode = patient.ClinicCode;
                    inModel.CardNo = patient.CardNo;
                    inModel.PactCode = hcareModel.FeeType == null ? "" : hcareModel.FeeType;
                    inModel.SchemaID = patient.SchemaID;
                    inModel.MdtrtCertType = hcareModel.MdtrtCertType;
                    inModel.MdtrtCertNo = hcareModel.MdtrtCertNo;
                    inModel.CardSn = hcareModel.CardSN;
                    inModel.PsnCertType = hcareModel.PsnCertType;
                    inModel.Certno = patient.IDCard;
                    inModel.PsnName = patient.Name;
                    inModel.RegFee = "";
                    inModel.DeptCode = patient.Dept.ID;
                    inModel.BirctrlType = "";
                    inModel.BirctrlMatnDate = "";
                    inModel.OpterType = "3";
                    inModel.OpterCode = "00A105";
                    inModel.OpterName = "微信";
                    GDSI.ZhuHaiSI.Business.Comom.MedicalService ms = new GDSI.ZhuHaiSI.Business.Comom.MedicalService();
                    if (ms.RegSettlementforSettlementType(inModel, settlementType, ref outModel2207) < 0)
                    {
                        ms.RollBack();
                        throw new Exception(ms.ErrorMessage);
                    }
                    #endregion

                    var medicalFeeCalculatorInput = new MedicalFeeCalculatorInput();
                    medicalFeeCalculatorInput.MedfeeSumamt = outModel2207.SetlInfo.MedfeeSumamt;
                    medicalFeeCalculatorInput.FundPaySumamt = outModel2207.SetlInfo.FundPaySumamt;
                    medicalFeeCalculatorInput.PsnCashPay = outModel2207.SetlInfo.PsnCashPay;
                    medicalFeeCalculatorInput.AcctPay = outModel2207.SetlInfo.AcctPay;
                    medicalFeeCalculatorInput.AcctMulaidPay = outModel2207.SetlInfo.AcctMulaidPay;
                    var medicalFeeCalculatorOutput = MedicalFeeCalculator.Calculate(medicalFeeCalculatorInput);
                    if (!medicalFeeCalculatorOutput.IsSuccess)
                    {
                        ms.RollBack();
                        throw new Exception(medicalFeeCalculatorOutput.ErrorMessage);
                    }


                    Models.Views.OutPatient.HcareResult res = new Models.Views.OutPatient.HcareResult();
                    res.HcareAmount = medicalFeeCalculatorOutput.PubCost * 100;
                    res.ExpenseAmount = 0;
                    res.SelfAmount = medicalFeeCalculatorOutput.OwnCost * 100;
                    res.TotalAmount = (medicalFeeCalculatorOutput.TotCost) * 100;
                    res.EcostAmount = 0;
                    res.ClincCode = patient.ClinicCode;
                    res.Remark = ms.ErrorMessage;
                    if (order.ISECOST == "1" && (patient.OwnDigFee + patient.RegFee) <= 30)
                    {
                        res.SelfAmount = 0;
                        res.EcostAmount = medicalFeeCalculatorOutput.OwnCost * 100;
                    }

                    if (order.REGFEE != ((int)res.TotalAmount).ToString())
                    {
                        ms.RollBack();
                        throw new Exception("【医保减免接口】订单费用与挂号费用不等！");
                    }

                    result.IsSuccessful = true;
                    result.Message = "";
                    result.ReturnData = res;

                    return result;
                }
                else
                {
                    Models.Views.OutPatient.HcareResult res = new Models.Views.OutPatient.HcareResult();
                    res.HcareAmount = 0;
                    res.ExpenseAmount = 0;
                    res.SelfAmount = (patient.OwnDigFee + patient.RegFee) * 100;
                    res.TotalAmount = (patient.OwnDigFee + patient.RegFee) * 100;
                    res.EcostAmount = 0;
                    res.ClincCode = "";
                    //if (order.ISECOST == "1")
                    //{
                    //    res.SelfAmount = 0;
                    //    res.EcostAmount = (patient.OwnDigFee + patient.RegFee) * 100;
                    //}

                    result.IsSuccessful = true;
                    result.Message = "";
                    result.ReturnData = res;

                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccessful = false;
                result.Message = e.Message.ToString();
                return result;
            }
        }

        public Models.Views.ComResult<Models.Views.OutPatient.HcareResult> CancelHcare(string clincCode)
        {
            Models.Views.ComResult<Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();
            Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
            GDSI.ZhuHaiSI.Business.Comom.MedicalService ms = new GDSI.ZhuHaiSI.Business.Comom.MedicalService();
            if (ms.CancelRegSettlement(clincCode, "00A105", "微信", "3") < 0)
            {

                throw new Exception(ms.ErrorMessage);
            }
            //Models.Views.OutPatient.HcareResult res = new Models.Views.OutPatient.HcareResult();
            result.IsSuccessful = true;
            result.Message = "取消成功！";
            //result.ReturnData = res;
            return result;
        }

        #endregion

        #region 缴费
        Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        Neusoft.HISFC.BizLogic.Registration.Register registerManager = new Neusoft.HISFC.BizLogic.Registration.Register();
        Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        Neusoft.HISFC.BizLogic.Fee.UndrugPackAge undrugPackAgeManager = new Neusoft.HISFC.BizLogic.Fee.UndrugPackAge();
        Neusoft.HISFC.BizProcess.Integrate.Order orderIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Order();
        Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        Neusoft.HISFC.BizLogic.Fee.PactUnitItemRate pactUnitItemRateManager = new Neusoft.HISFC.BizLogic.Fee.PactUnitItemRate();
        Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        Neusoft.HISFC.BizProcess.Integrate.Pharmacy pharmacyIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Pharmacy();
        Neusoft.HISFC.BizLogic.Fee.Item undrugManager = new Neusoft.HISFC.BizLogic.Fee.Item();
        Neusoft.HISFC.BizProcess.Integrate.Manager conMgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        Neusoft.HISFC.BizLogic.Registration.Schema schMgr = new Neusoft.HISFC.BizLogic.Registration.Schema();
        Neusoft.HISFC.BizLogic.Fee.PactUnitInfo pactManager = new Neusoft.HISFC.BizLogic.Fee.PactUnitInfo();
        Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm confirmIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm();
        Neusoft.HISFC.BizProcess.Integrate.Material.Material mateIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Material.Material();
        Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareInterfaceProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();
        Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IGetItemPrice.ItemPrice ItemPrice = new Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IGetItemPrice.ItemPrice();
        private packagService.ZDWY.MzPackage mzPk = new packagService.ZDWY.MzPackage();
        /// <summary>
        /// 处方缴费
        /// </summary>
        /// <param name="billpay"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        public Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY> BillPay(Models.PLATFORM_BALANCE_PAY billpay, Models.OperInfo oper, string regNo, string balanceNo, string RECIPTTYPE, string RECIPTNO, string ydzf)
        {
            /**
            * 重新定义输入参数意义
            * pm.PayAmt 结算总金额 tot_cost
            * pm.PersonalFee 自费金额 own_cost
            * pm.MedInsureFee 医保统筹金额 pub_cost
            * pm.InvoiceNo 发票号 为医保结算必须
            * 通过传入金额和再次明细合计金额合计比较验证
            * 
            * 
            */
            FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
            #region
            //事务开启
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            //设置事务
            FS.ZDWY.Internet.BL.OutPatient.PlatformBillLogic plabill = new BL.OutPatient.PlatformBillLogic();
            plabill.BeginTran();

            outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            registerManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            managerIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            undrugPackAgeManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            orderIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            pactUnitItemRateManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            controlParamIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            pharmacyIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            undrugManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            conMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            mgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            //medcareInterfaceProxy.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            #endregion
            Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY> result = new Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY>();

            pactUnitItemRateManager.Operator.ID = oper.Code;
            pactUnitItemRateManager.Operator.Name = oper.Name;
            //医保取消
            string SiTransid = "";
            Neusoft.HISFC.Models.Registration.Register cancelreg = null;
            string cancelBka006 = "";
            string debugFee = "";
            ArrayList RollBackFee = new ArrayList();
            try
            {
                //oper = FS.ZDWY.Internet.BP.Common.Function.DefaultOper;
                string dt22 = outpatientManager.GetSysDateTime();
                if (string.IsNullOrEmpty(billpay.VISITNO))
                {
                    throw new Exception("VISITNO为空！");
                }
                //if (Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.PAYAMT) <= 0)
                //{
                //    throw new Exception("PayAmt 支付金额不能小于等于0！");
                //}


                if (string.IsNullOrEmpty(billpay.PAYMODE))
                {
                    throw new Exception("PayType 支付方式不能为空！");
                }

                string clinicCode = billpay.VISITNO;  //就诊号

                ArrayList comFeeItemLists = new ArrayList();  //费用集合
                Neusoft.HISFC.Models.Registration.Register reg = null; //患者基本信息

                decimal totFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.TOTALAMOUNT) / 100;       //总费用

                decimal ownFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.SELFAMOUNT) / 100;   //个人支付费用(移动支付时为现金支付)
                decimal pubFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.EXPENSEAMOUNT) / 100;   //报销费用
                decimal hcareFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.HCAREAMOUNT) / 100;   //报销费用
                decimal psnAcctPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.PSNACCTPAY) / 100;   //个人账户支出
                if (totFee != ownFee + pubFee + hcareFee + psnAcctPay)
                {
                    throw new Exception("费用不等！");
                }
                    //已缴费返回历史纪录
                    Models.PLATFORM_BALANCE_PAY prepayres = plabill.Get(billpay.ORDERID);
                if (prepayres != null)
                {
                    result.IsSuccessful = true;
                    result.ReturnData = prepayres;
                    return result;
                }

                string errMsg = "";

                decimal selfDrugCost = 0;//自费药金额
                decimal overDrugCost = 0;//超标药金额
                decimal ownCost = 0;//自费金额
                decimal pubCost = 0;//社保支付金额
                decimal payCost = 0;//自付金额
                decimal totCost = 0;//总金额
                decimal formerTotCost = 0;//对比的总金额

                reg = registerManager.GetByClinic(clinicCode);
                if (string.IsNullOrEmpty(reg.ID))
                {
                    throw new Exception("获取挂号信息出错");
                }
                reg.Pact.PayKind.ID = "02";
                reg.Pact.ID = "1";
                reg.Pact.Name = "现金";


                //判断【合同单位】是否可以进行自助终端结算(常数维护)
                ArrayList selfFeePact = managerIntegrate.GetConstantList("SelfFeePact");   //可以进行自助终端结算的合同单位
                bool isCanSelfFee = false;
                foreach (Neusoft.FrameWork.Models.NeuObject o in selfFeePact)
                {
                    if (o.ID.Equals(reg.Pact.ID))
                    {
                        isCanSelfFee = true;
                        break;
                    }
                }
                if (!isCanSelfFee)
                {
                    throw new Exception("合同单位【" + reg.Pact.Name + "】,请到人工窗口收费处进行收费!");
                }
                //reg.SIMainInfo.OwnCost = ownFee;
                //reg.SIMainInfo.PayCost = decimal.Zero;
                //reg.SIMainInfo.PubCost = pubFee;

                //获取挂号的未收费项目信息
                List<string> reciptTypeList = RECIPTTYPE.Split('|').ToList();
                List<string> reciptNoList = RECIPTNO.Split('|').ToList();
                if (reciptTypeList.Count != reciptNoList.Count)
                {
                    throw new Exception("处方单号个数与处方类型个数不符");
                }
                ArrayList alist = new ArrayList();
                for (int i = 0; i < reciptTypeList.Count; i++)
                {
                    ArrayList altemp = outpatientManager.QueryChargedFeeItemListsByRecipeNoAndRecipeFlag(clinicCode, reciptNoList[i], reciptTypeList[i]);
                    alist.AddRange(altemp);
                }
                //ArrayList al = new ArrayList(alist);
                ArrayList al = new ArrayList();
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in alist)
                {
                    if (f.Item.ItemType == EnumItemType.Drug)
                    {
                        if ((f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost) > 0)
                        {
                            al.Add(f);
                        }
                    }
                    else
                        al.Add(f);
                }
                string doctid = "";     //开方医生工号
                string deptid = "";    //开方科室
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in al)
                {
                    if (f.Item.IsMaterial)
                    {
                        continue;
                    }
                    doctid = f.RecipeOper.ID;
                    deptid = f.RecipeOper.Dept.ID;
                    if (string.IsNullOrEmpty(doctid))
                    {
                        throw new Exception("开方医生为空!");
                    }
                    if (string.IsNullOrEmpty(deptid))
                    {
                        throw new Exception("开方科室为空!");
                    }
                }

                //清空费用信息
                comFeeItemLists.Clear();
                comFeeItemLists = this.GetFeeItemList(al, reg, ref errMsg);
                if (comFeeItemLists == null || comFeeItemLists.Count <= 0)
                {
                    throw new Exception("您暂时无缴费信息!" + errMsg);
                }
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    if (string.IsNullOrEmpty(f.RecipeOper.ID))
                    {
                        f.RecipeOper.ID = doctid;
                    }
                }
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    if (f.Patient.PID.CardNO != reg.PID.CardNO || f.Patient.ID != reg.ID)
                    {
                        throw new Exception("收费前患者费用明细与实际不符，请告知电脑中心!");
                    }
                }

                //判断是否有项目停用
                if (!this.IsItemValid(comFeeItemLists, ref errMsg))
                {
                    throw new Exception("判断是否有项目停用出错!" + errMsg);
                }

                //用药品超标金额保存原来的总费用
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        f.FT.ExcessCost = f.Item.Qty * f.Item.ChildPrice / f.Item.PackQty;
                        f.FT.ExcessCost = Neusoft.FrameWork.Public.String.FormatNumber(f.FT.ExcessCost, 2);
                    }

                    //获得费用明细的总金额
                    f.FT.TotCost = f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                }

                #region 获取发票号

                string invoiceNO = "";                //当前发票电脑号
                string realInvoiceNO = string.Empty; //当前发票应刷号        

                Neusoft.HISFC.Models.Base.Employee employee = this.managerIntegrate.GetEmployeeInfo(FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code);

                //获得本次收费起始发票号
                long returnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", true, ref invoiceNO, ref realInvoiceNO, ref errMsg);
                if (returnValue == -1)
                {
                    throw new Exception("获得本次收费起始发票号出错！" + errMsg);
                }
                if (string.IsNullOrEmpty(invoiceNO))
                {
                    throw new Exception("获取发票号出错！");
                }
                if (reg.Pact.PayKind.ID == "02")
                {
                    reg.SIMainInfo.InvoiceNo = invoiceNO;
                }

                #endregion

                reg.SIMainInfo.OwnCost = ownFee;
                reg.SIMainInfo.PayCost = decimal.Zero;
                reg.SIMainInfo.PubCost = pubFee;
                reg.SIMainInfo.TotCost = totFee;
                reg.SIMainInfo.BalNo = balanceNo;
                GDSI.CountryMedical.Model.FIN_IPR_SIINMAININFO_GD gdInfo = null;
                if (ydzf == "1")//移动支付流程
                {
                    GDSI.CountryMedical.DAL.QueryDAL queryDB = new GDSI.CountryMedical.DAL.QueryDAL();
                    gdInfo = queryDB.GetGDModelForIDAndBalanceNo(clinicCode, balanceNo);
                    if (gdInfo == null)
                    {
                        throw new Exception("医保处理出错：未查询到相关参保信息！");
                    }
                    if (string.IsNullOrEmpty(gdInfo.MDTRTID))
                    {
                        throw new Exception("医保处理出错：未查询到相关参保信息！");
                    }
                    if (gdInfo.VALID_FLAG != "1" || gdInfo.BALANCE_STATE != "1")
                    {
                        throw new Exception("医保处理出错：当前登记信息状态不正确！");
                    }
                    Neusoft.HISFC.Models.Base.PactInfo pact1 = conMgr.GetPactUnitInfoByPactCode(gdInfo.PACT_CODE);
                    reg.Pact = pact1;
                    reg.SIMainInfo.InsuplcAdmdvs = gdInfo.INSUPLCADMDVS;
                    if (reg.SIMainInfo.InsuplcAdmdvs.StartsWith("4403") || reg.SIMainInfo.InsuplcAdmdvs.StartsWith("4415") || !reg.SIMainInfo.InsuplcAdmdvs.StartsWith("44"))//虚账地区
                    {
                        pubFee += psnAcctPay;
                        reg.SIMainInfo.PubCost = pubFee;
                        psnAcctPay = 0;
                    }
                }
                else//原医保流程
                {
                    #region 医保处理
                    if (!string.IsNullOrWhiteSpace(regNo) || reg.SIMainInfo.PubCost > 0)
                    {
                        GDSI.CountryMedical.DAL.QueryDAL queryDB = new GDSI.CountryMedical.DAL.QueryDAL();
                        gdInfo = queryDB.GetGDModelForIDAndBalanceNo(clinicCode, balanceNo);
                        if (gdInfo == null)
                        {
                            throw new Exception("医保处理出错：未查询到相关参保信息！");
                        }
                        if (gdInfo.MDTRTID != regNo)
                        {
                            throw new Exception("医保处理出错：根据就医登记号未查询到相关参保信息！");
                        }
                        if (gdInfo.VALID_FLAG != "1" || gdInfo.BALANCE_STATE != "0")
                        {
                            throw new Exception("医保处理出错：当前登记信息状态不正确！");
                        }
                        reg.Pact.PayKind.ID = "02";
                        reg.Pact.ID = gdInfo.PACT_CODE;
                        reg.Pact.Name = gdInfo.PACT_NAME;
                        reg.SIMainInfo.enumCallAPIChannel = Neusoft.HISFC.Models.SIInterface.EnumCallAPIChannel.ZDWY_WX_MZJF;
                        Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
                        Neusoft.FrameWork.Management.Connection.Operator = employee as Neusoft.FrameWork.Models.NeuObject;
                        Neusoft.HISFC.Models.Base.PactInfo pact1 = conMgr.GetPactUnitInfoByPactCode(reg.Pact.ID);
                        reg.SIMainInfo.OpterCode = oper.Code;
                        reg.SIMainInfo.OpterName = oper.Name;
                        reg.SIMainInfo.OpterType = "3";
                        reg.SIMainInfo.RegNo = regNo;
                        reg.SIMainInfo.BalNo = balanceNo;
                        reg.Pact = pact1;


                        //设置待遇的合同单位参数
                        this.medcareInterfaceProxy.SetPactCode(reg.Pact.ID);
                        this.medcareInterfaceProxy.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                        this.medcareInterfaceProxy.BeginTranscation();//开始待遇算法事务
                        this.medcareInterfaceProxy.IsLocalProcess = false;  //实时计算
                        returnValue = this.medcareInterfaceProxy.Connect();//连接待遇接口
                        if (returnValue == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            //医保回滚可能出错，此处提示
                            if (this.medcareInterfaceProxy.Rollback() == -1)
                            {
                                throw new Exception("医保处理出错！" + this.medcareInterfaceProxy.ErrMsg);

                            }
                            this.medcareInterfaceProxy.Disconnect();
                            throw new Exception("医疗待遇接口连接失败!" + this.medcareInterfaceProxy.ErrMsg);

                        }
                        //待遇接口门诊结算
                        returnValue = this.medcareInterfaceProxy.BalanceOutpatient(reg, ref comFeeItemLists);
                        if (returnValue == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            //医保回滚可能出错，此处提示
                            if (this.medcareInterfaceProxy.Rollback() == -1)
                            {
                                throw new Exception("医保回滚提示：" + this.medcareInterfaceProxy.ErrMsg);

                            }
                            this.medcareInterfaceProxy.Disconnect();
                            throw new Exception("待遇接口门诊结算失败!" + this.medcareInterfaceProxy.ErrMsg);
                        }



                        #region 医保处理
                        //FS.ZDWY.Internet.Models.Views.OutPatient.ComPatient patient = new Models.Views.OutPatient.ComPatient();
                        //patient.OperTime = outpatientManager.GetDateTimeFromSysDateTime();
                        //patient.SSN = reg.IDCard;
                        //patient.Name = reg.Name;
                        //patient.IDCard = reg.IDCard;
                        //FS.ZDWY.Internet.BP.SI.OutPatient.GetPatientInfo GetPtient = new SI.OutPatient.GetPatientInfo();
                        //GDSI.Models.OutParam.OutParamBizh110001 res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                        //string[] GetinfoParams = new string[] { billpay.TRANNO, "13", "131" };
                        //if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                        //{
                        //    throw new Exception("【医保错误】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                        //}
                        //if (res110001.Personinfos.Count != 1)
                        //{
                        //    throw new Exception("【医保错误】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                        //}

                        ////Aka130 = "11";
                        ////Bka006 = "110";
                        ////appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042}
                        //FS.ZDWY.Internet.BP.SI.OutPatient.Balance Hcareser = new SI.OutPatient.Balance();
                        //GDSI.Models.Personinfo patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                        //GDSI.Models.Spinfo cli = null;
                        //string Aaz267 = "1";
                        //string Bka006 = "131";
                        //string DiagCode = "";
                        //if (res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                        //{
                        //    cli = res110001.Spinfos[0] as GDSI.Models.Spinfo;
                        //    Aaz267 = cli.Aaz267;
                        //    Bka006 = cli.Bka006;
                        //    DiagCode = cli.Bka026;
                        //    cancelBka006 = cli.Bka006;
                        //}
                        //GDSI.Models.OutParam.OutParamBizh110104 res110004 = new GDSI.Models.OutParam.OutParamBizh110104();
                        ////appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042,clinicCode,Bka026}
                        //string[] appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, billpay.TRANNO, "13", Bka006, "1", Aaz267, "", reg.ID, DiagCode };
                        //if (Hcareser.CallService(comFeeItemLists, ref res110004, appendParams) <= 0)
                        //{
                        //    throw new Exception("【医保错误】获取医保患者医保减免信息失败！原因：" + Hcareser.ErrorMsg);
                        //}

                        //#endregion

                        //SiTransid = res110004.Payinfo.Aaz218;
                        //cancelreg = reg;
                        //RollBackFee = (ArrayList)comFeeItemLists.Clone();

                        //#region 插入数据库
                        //Neusoft.HISFC.BizLogic.Registration.Register regMgr = new Neusoft.HISFC.BizLogic.Registration.Register();
                        ////Neusoft.HISFC.Models.Registration.Register sireg = regMgr.GetByClinic
                        //reg.SIMainInfo.RegNo = res110004.Payinfo.Aaz218;
                        //reg.InvoiceNO = invoiceNO;
                        ////reg.PID.CardNO = patient.CardNo;
                        //reg.SSN = patientinfo.Aac001;
                        ////reg.Name = patient.Name;
                        //reg.IDCard = patient.IDCard;
                        //reg.ClinicDiagnose = "";
                        ////reg.Pact.PayKind.ID = "M";
                        ////reg.Pact.ID = "251";
                        ////reg.Pact.Name = "01";
                        //reg.SIMainInfo.TotCost = res110004.Payinfo.Akc264;
                        //reg.SIMainInfo.PubCost = res110004.Payinfo.Bka832;
                        //reg.SIMainInfo.OwnCost = res110004.Payinfo.Bka831;
                        //reg.SIMainInfo.PayCost = 0;
                        ////reg.Sex.ID = patient.SexCode;
                        ////reg.DoctorInfo.Templet.Dept.ID = patient.Dept.ID;
                        ////reg.PVisit.InTime = patient.RegDate;
                        ////reg.SIMainInfo.BalanceDate = patient.RegDate;

                        //reg.SIMainInfo.Bka825 = res110004.Payinfo.Bka825;
                        //reg.SIMainInfo.Bka826 = res110004.Payinfo.Bka826;
                        //reg.SIMainInfo.Aka151 = res110004.Payinfo.Aka151;
                        //reg.SIMainInfo.Bka838 = res110004.Payinfo.Bka838;
                        //reg.SIMainInfo.Akb067 = res110004.Payinfo.Akb067;
                        //reg.SIMainInfo.Akb066 = res110004.Payinfo.Akb066;
                        //reg.SIMainInfo.Bka821 = res110004.Payinfo.Bka821;
                        //reg.SIMainInfo.Bka839 = res110004.Payinfo.Bka839;
                        //reg.SIMainInfo.Ake039 = res110004.Payinfo.Ake039;
                        //reg.SIMainInfo.Ake035 = res110004.Payinfo.Ake035;
                        //reg.SIMainInfo.Ake026 = res110004.Payinfo.Ake026;
                        //reg.SIMainInfo.Ake029 = res110004.Payinfo.Ake029;
                        //reg.SIMainInfo.Bka841 = res110004.Payinfo.Bka841;
                        //reg.SIMainInfo.Bka842 = res110004.Payinfo.Bka842;
                        //reg.SIMainInfo.Bka840 = res110004.Payinfo.Bka840;
                        //reg.SIMainInfo.Bka020 = "";
                        //reg.ClinicDiagnose = "";
                        //reg.SIMainInfo.Aaa027 = "";
                        //reg.SSN = patientinfo.Aac001;
                        //reg.SIMainInfo.Aab301 = "11";
                        //reg.SIMainInfo.Aae140 = patientinfo.Aae140;
                        //reg.SIMainInfo.Aka130 = "13";
                        //reg.SIMainInfo.Bka006 = Bka006;

                        //if (mgr.InsertOutPatientBalance(reg) < 0)
                        //{
                        //    throw new Exception("保存医保费用信息出错！" + mgr.Err);
                        //}
                        #endregion
                    }
                    else
                    {
                        reg.SIMainInfo.PubCost = 0;
                        reg.SIMainInfo.PayCost = 0;
                    }

                    #endregion
                }
                //获得当前系统时间
                DateTime nowTime = this.undrugManager.GetDateTimeFromSysDateTime();
                //汇总没有进行待遇计算时的费用总金额
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    //如果有已经有明细账户支付了,首先考虑只是自费患者,那么将自费调整为0, 账户支付调整为自费金额.
                    if (reg.Pact.ID == "1" && f.IsAccounted)
                    {
                        if (f.FT.OwnCost > 0)
                        {
                            f.FT.PayCost += f.FT.OwnCost;
                            f.FT.OwnCost = 0;
                        }
                    }

                    f.FeeOper.OperTime = nowTime;

                    //通过待遇算法处理，可能产生减免费用
                    formerTotCost += f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                }

                //重新计算待遇计算后的费用金额
                decimal rebateRate = 0;

                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    // 通过待遇算法处理，可能产生减免费用
                    totCost += f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                    if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        overDrugCost += Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.ExcessCost);
                        selfDrugCost += Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.DrugOwnCost);
                    }
                    f.NoBackQty = f.Item.Qty;
                    rebateRate += f.FT.RebateCost;   //优惠金额
                }

                //ownCost = totCost - pubFee;
                //payCost += 0;
                //pubCost += pubFee;
                ownCost = totCost - reg.SIMainInfo.PubCost - reg.SIMainInfo.PayCost;
                payCost += reg.SIMainInfo.PayCost;
                pubCost += reg.SIMainInfo.PubCost;

                //所有金额保留2位小数
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(totCost, 2);
                ownCost = Neusoft.FrameWork.Public.String.FormatNumber(ownCost, 2);
                pubCost = Neusoft.FrameWork.Public.String.FormatNumber(pubCost, 2);
                payCost = Neusoft.FrameWork.Public.String.FormatNumber(payCost, 2);

                //if (ownCost != ownFee)
                //{
                //    throw new Exception("医保预计算金额与实际结算金额不符！");
                //}

                ////判断一下传递进来的费用 与 待遇算法的费用是否相等

                if (totFee != totCost || totFee != (pubFee + ownFee + psnAcctPay))// || pubCost != tcfee || rebateRate != yhfee)
                {
                    debugFee = "totFee:" + totFee + " totCost:" + totCost + " pubFee:" + pubFee + " ownFee:" + ownFee + " clinicCode" + clinicCode;
                    throw new Exception("本院收费系统的总费用与终端机的费用不符合,医生可能修改医嘱，请认真核对!");
                }

                #region 收费金额取整-因为是电子货币，所以不适用
                #endregion

                //当减免金额大于 ownCost 时，rebateRate = ownCost
                if (rebateRate > ownCost)
                {
                    rebateRate = ownCost;
                }

                #region 生成发票和发票明细

                FS.ZDWY.Internet.BP.OutPatient.NeusoftBussiness.IsQuitFee = false;
                ArrayList balancesAndBalanceLists = FS.ZDWY.Internet.BP.OutPatient.NeusoftBussiness.MakeInvoice(this.feeIntegrate, reg, comFeeItemLists, invoiceNO, realInvoiceNO, ref errMsg);
                if (balancesAndBalanceLists == null || balancesAndBalanceLists.Count <= 0)
                {
                    throw new Exception("生成发票和发票明细出错：" + errMsg);
                }

                //发票
                ArrayList alInvoice = (ArrayList)balancesAndBalanceLists[0];
                if (alInvoice == null || alInvoice.Count <= 0)
                {
                    throw new Exception("发票数量为0!");
                }

                //发票明细
                ArrayList alInvoiceDetails = (ArrayList)balancesAndBalanceLists[1];
                if (alInvoiceDetails == null | alInvoiceDetails.Count <= 0)
                {
                    throw new Exception("发票明细数量为0!");
                }

                //发票费用明细
                ArrayList alInvoiceFeeDetails = (ArrayList)balancesAndBalanceLists[2];
                if (alInvoiceFeeDetails == null || alInvoiceFeeDetails.Count <= 0)
                {
                    throw new Exception("发票费用明细为空!");
                }

                #endregion
                ArrayList balancePays;
                //获取支付方式
                if (ydzf == "1")
                {
                    balancePays = this.QueryBalancePaysYDZF((ownFee - rebateRate), billpay, psnAcctPay, pubFee, reg.SIMainInfo.InsuplcAdmdvs);
                }
                else
                {
                    balancePays = this.QueryBalancePays((ownCost - rebateRate), billpay, rebateRate, pubCost);
                }
                if (balancePays == null || balancePays.Count <= 0)
                {
                    throw new Exception("支付方式的记录为0条!");
                }

                this.feeIntegrate.IsNeedUpdateInvoiceNO = true;

                #region 物资收费-暂时不使用

                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList temfItem in comFeeItemLists)
                {
                    if (temfItem.Item.ItemType != Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        temfItem.StockOper.Dept.ID = temfItem.ExecOper.Dept.ID;
                    }
                }

                #endregion
                Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
                bool boolReturnValue = this.feeIntegrate.ClinicFee(Neusoft.HISFC.Models.Base.ChargeTypes.Fee, "C", true, reg,
                   alInvoice, alInvoiceDetails, comFeeItemLists, alInvoiceFeeDetails, balancePays, ref errMsg, employee);
                if (!boolReturnValue)
                {
                    throw new Exception("缴费失败!" + errMsg + this.feeIntegrate.Err);
                }
                reg.SIMainInfo.InvoiceNo = ((Neusoft.HISFC.Models.Fee.Outpatient.Balance)alInvoice[0]).Invoice.ID;
                reg.SIMainInfo.User01 = ((Neusoft.HISFC.Models.Fee.Outpatient.Balance)alInvoice[0]).PrintedInvoiceNO;
                reg.SIMainInfo.OperInfo.User02 = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;


                Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
                Neusoft.FrameWork.Management.Connection.Operator = employee as Neusoft.FrameWork.Models.NeuObject;
                Neusoft.HISFC.Models.Base.PactInfo pact = conMgr.GetPactUnitInfoByPactCode(reg.Pact.ID);
                reg.Pact = pact;

                string SqlQueryMZDrugWindow = @"SELECT DRUG_WINDOW FROM FIN_OPB_INVOICEINFO WHERE INVOICE_NO ='{0}'";

                string drugWindow = this.outpatientManager.ExecSqlReturnOne(string.Format(SqlQueryMZDrugWindow, invoiceNO));

                billpay.INVOICEID = realInvoiceNO;
                billpay.RECEIPTID = invoiceNO;
                billpay.VISITADDRESS = drugWindow;
                billpay.HOSPITALNUM = invoiceNO;
                billpay.OPERTIME = conMgr.GetDateTimeFromSysDateTime();
                billpay.STATUS = "0";

                result.IsSuccessful = true;
                result.ReturnData = billpay;

                if (!plabill.Insert(billpay))
                {
                    throw new Exception("保存收费记录失败！");
                }

                string invoicesql = @"update fin_opb_invoiceinfo invo
set invo.oper_code='{2}'
where invo.clinic_code='{0}'
and invo.invoice_no='{1}'
and invo.trans_type='1'";
                invoicesql = string.Format(invoicesql, reg.ID, invoiceNO, oper.Code);
                if (mgr.ExecNoQuery(invoicesql) <= 0)
                {
                    throw new Exception("更新发票信息失败！");
                }

                string paysql = @"update fin_opb_paymode pa
set pa.oper_code='{1}'
where  pa.invoice_no='{0}'
and pa.trans_type='1'";
                paysql = string.Format(paysql, invoiceNO, oper.Code);
                if (mgr.ExecNoQuery(paysql) <= 0)
                {
                    throw new Exception("更新发票信息失败！");
                }

                string desql = @"update fin_opb_invoicedetail de
set de.oper_code='{1}'
where de.invoice_no='{0}'
and de.trans_type='1'";
                desql = string.Format(desql, invoiceNO, oper.Code);
                if (mgr.ExecNoQuery(desql) <= 0)
                {
                    throw new Exception("更新发票信息失败！");
                }
                if (ydzf == "1" && gdInfo != null)
                {
                    string gdSql = @"Update fin_ipr_siinmaininfo_gd gd 
set gd.invoice_no = '{0}'
where gd.mdtrtid = '{1}'";
                    gdSql = string.Format(gdSql, invoiceNO, gdInfo.MDTRTID);
                    if (mgr.ExecNoQuery(gdSql) <= 0)
                    {
                        throw new Exception("更新发票信息失败！");
                    }
                }
                #region
                #endregion

                #region -------------发药机--------------------
                try
                {
                    //门诊西药房发药机发送处方
                    if (FS.ZDWY.Internet.BP.OutPatient.NeusoftBussiness.GetSendPackage())
                    {
                        string err = string.Empty;

                        Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("MzPk", "inside");
                        ArrayList feeList = new ArrayList();
                        foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList item in comFeeItemLists)
                        {
                            if (item.ExecOper.Dept.ID == "9004")
                            {
                                feeList.Add(item);
                            }
                        }

                        if (feeList.Count > 0)
                        {
                            int i = mzPk.RecipeFee(reg, feeList, ref err);
                            Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("MzPk", "result:" + i.ToString());
                            if (i == -1)
                            {
                                Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("MzPk", "end err：" + err);
                            }
                            else if (i == 0) { /* 不处理 */}
                            else
                            {
                                string MzPkTerSql = @" select nvl(min(fun_get_dept_name(c.dept_code) || c.t_name),0) winNo
    from pha_sto_recipe b  join pha_sto_terminal c on c.t_code = b.send_terminal
     where b.invoice_no='{0}' and  b.drug_dept_code = '9004' and b.sendpackge='1' ";

                                string sql = string.Format(MzPkTerSql, invoiceNO);
                                drugWindow = outpatientManager.ExecSqlReturnOne(sql);
                                UpdateTerminalByInvoice(invoiceNO, drugWindow);
                                billpay.VISITADDRESS = drugWindow;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("MzPk", ex.Message);
                }

                #endregion

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                plabill.CommitTran();

                return result;
            }
            catch (Exception e)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                plabill.RollbackTran();
                result.IsSuccessful = false;
                result.Message = e.Message.ToString();
                result.ReturnData = null;
                if (this.medcareInterfaceProxy.Rollback() == -1)
                {
                    throw new Exception("medcareInterfaceProxy:" + this.medcareInterfaceProxy.ErrMsg + "   " + "Exception:" + e.Message + "    " + debugFee);

                }
                #region 医保退费

                //if (!string.IsNullOrEmpty(SiTransid))
                //{
                //    FS.ZDWY.Internet.BP.SI.OutPatient.GetCancelFeeInfo cancelfeeinfo = new SI.OutPatient.GetCancelFeeInfo();
                //    string erro = "";
                //    if (cancelfeeinfo.CallService(SiTransid, ref erro, cancelBka006) < 0)
                //    {
                //        result.Message += "收费时提取门诊业务信息！原因：" + cancelfeeinfo.ErrorMsg;
                //    }

                //    object[] objfee = new object[] { billpay.TRANNO, "", "", RollBackFee };
                //    FS.ZDWY.Internet.BP.SI.OutPatient.CancelFee cancelfee = new SI.OutPatient.CancelFee();
                //    if (cancelfee.CallService(cancelreg, ref erro, objfee) < 0)
                //    {
                //        result.Message += "取消费用信息！原因：" + cancelfee.ErrorMsg;
                //    }

                //    FS.ZDWY.Internet.BP.SI.OutPatient.GetFeeInfo feeinfo = new SI.OutPatient.GetFeeInfo();
                //    if (feeinfo.CallService(SiTransid, ref erro, cancelBka006) < 0)
                //    {
                //        result.Message += "收费时提取门诊业务信息！原因：" + feeinfo.ErrorMsg;
                //    }

                //    FS.ZDWY.Internet.BP.SI.OutPatient.CancelRegister cancel = new SI.OutPatient.CancelRegister();
                //    GDSI.Models.OutParam.OutParamBizh110106 outParam = new GDSI.Models.OutParam.OutParamBizh110106();
                //    object[] obj = new object[] { billpay.TRANNO, SiTransid };
                //    if (cancel.CallService(SiTransid, ref outParam, obj) <= 0)
                //    {
                //        result.Message = result.Message + "取消医保挂号失败！原因：" + cancel.ErrorMsg;
                //    }
                //}

                #endregion

                return result;
            }
        }
        /// <summary>
        /// 处方缴费
        /// </summary>
        /// <param name="billpay"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        public decimal BillFeeCalculation(Models.PLATFORM_BALANCE_PAY billpay, Models.OperInfo oper, string regNo, string balanceNo, string RECIPTTYPE, string RECIPTNO, string ydzf)
        {
            FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
            FS.ZDWY.Internet.BL.OutPatient.PlatformBillLogic plabill = new BL.OutPatient.PlatformBillLogic();
            Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY> result = new Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY>();
            pactUnitItemRateManager.Operator.ID = oper.Code;
            pactUnitItemRateManager.Operator.Name = oper.Name;
            //医保取消
            string SiTransid = "";
            Neusoft.HISFC.Models.Registration.Register cancelreg = null;
            string cancelBka006 = "";
            string debugFee = "";
            ArrayList RollBackFee = new ArrayList();
            try
            {
                //oper = FS.ZDWY.Internet.BP.Common.Function.DefaultOper;
                string dt22 = outpatientManager.GetSysDateTime();
                if (string.IsNullOrEmpty(billpay.VISITNO))
                {
                    throw new Exception("VISITNO为空！");
                }
                //if (Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.PAYAMT) <= 0)
                //{
                //    throw new Exception("PayAmt 支付金额不能小于等于0！");
                //}



                string clinicCode = billpay.VISITNO;  //就诊号

                ArrayList comFeeItemLists = new ArrayList();  //费用集合
                Neusoft.HISFC.Models.Registration.Register reg = null; //患者基本信息

                decimal totFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.TOTALAMOUNT) / 100;       //总费用

                decimal ownFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.SELFAMOUNT) / 100;   //个人支付费用(移动支付时为现金支付)
                decimal pubFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.EXPENSEAMOUNT) / 100;   //报销费用
                decimal hcareFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.HCAREAMOUNT) / 100;   //报销费用
                decimal psnAcctPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.PSNACCTPAY) / 100;   //个人账户支出
                if (totFee != ownFee + pubFee + hcareFee + psnAcctPay)
                {
                    throw new Exception("费用不等！");
                }

                string errMsg = "";

                decimal selfDrugCost = 0;//自费药金额
                decimal overDrugCost = 0;//超标药金额
                decimal ownCost = 0;//自费金额
                decimal pubCost = 0;//社保支付金额
                decimal payCost = 0;//自付金额
                decimal totCost = 0;//总金额
                decimal formerTotCost = 0;//对比的总金额

                reg = registerManager.GetByClinic(clinicCode);
                if (string.IsNullOrEmpty(reg.ID))
                {
                    throw new Exception("获取挂号信息出错");
                }
                reg.Pact.PayKind.ID = "02";
                reg.Pact.ID = "1";
                reg.Pact.Name = "现金";


                //判断【合同单位】是否可以进行自助终端结算(常数维护)
                ArrayList selfFeePact = managerIntegrate.GetConstantList("SelfFeePact");   //可以进行自助终端结算的合同单位
                bool isCanSelfFee = false;
                foreach (Neusoft.FrameWork.Models.NeuObject o in selfFeePact)
                {
                    if (o.ID.Equals(reg.Pact.ID))
                    {
                        isCanSelfFee = true;
                        break;
                    }
                }
                if (!isCanSelfFee)
                {
                    throw new Exception("合同单位【" + reg.Pact.Name + "】,请到人工窗口收费处进行收费!");
                }

                //获取挂号的未收费项目信息
                List<string> reciptTypeList = RECIPTTYPE.Split('|').ToList();
                List<string> reciptNoList = RECIPTNO.Split('|').ToList();
                if (reciptTypeList.Count != reciptNoList.Count)
                {
                    throw new Exception("处方单号个数与处方类型个数不符");
                }
                ArrayList alist = new ArrayList();
                for (int i = 0; i < reciptTypeList.Count; i++)
                {
                    ArrayList altemp = outpatientManager.QueryChargedFeeItemListsByRecipeNoAndRecipeFlag(clinicCode, reciptNoList[i], reciptTypeList[i]);
                    alist.AddRange(altemp);
                }
                ArrayList al = new ArrayList();
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in alist)
                {
                    if ((f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost) > 0)
                    {
                        al.Add(f);
                    }
                }
                string doctid = "";     //开方医生工号
                string deptid = "";    //开方科室
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in al)
                {
                    if (f.Item.IsMaterial)
                    {
                        continue;
                    }
                    doctid = f.RecipeOper.ID;
                    deptid = f.RecipeOper.Dept.ID;
                    if (string.IsNullOrEmpty(doctid))
                    {
                        throw new Exception("开方医生为空!");
                    }
                    if (string.IsNullOrEmpty(deptid))
                    {
                        throw new Exception("开方科室为空!");
                    }
                }


                //清空费用信息
                comFeeItemLists.Clear();
                comFeeItemLists = this.GetFeeItemList(al, reg, ref errMsg);
                if (comFeeItemLists == null || comFeeItemLists.Count <= 0)
                {
                    throw new Exception("您暂时无缴费信息!" + errMsg);
                }

                //给组套明细付值
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    if (string.IsNullOrEmpty(f.RecipeOper.ID))
                    {
                        f.RecipeOper.ID = doctid;
                    }
                }
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    if (f.Patient.PID.CardNO != reg.PID.CardNO || f.Patient.ID != reg.ID)
                    {
                        throw new Exception("收费前患者费用明细与实际不符，请告知电脑中心!");
                    }
                }

                //判断是否有项目停用
                if (!this.IsItemValid(comFeeItemLists, ref errMsg))
                {
                    throw new Exception("判断是否有项目停用出错!" + errMsg);
                }

                //用药品超标金额保存原来的总费用
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        f.FT.ExcessCost = f.Item.Qty * f.Item.ChildPrice / f.Item.PackQty;
                        f.FT.ExcessCost = Neusoft.FrameWork.Public.String.FormatNumber(f.FT.ExcessCost, 2);
                    }

                    //获得费用明细的总金额
                    f.FT.TotCost = f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                }

                #region 获取发票号

                string invoiceNO = "";                //当前发票电脑号
                string realInvoiceNO = string.Empty; //当前发票应刷号

                Neusoft.HISFC.Models.Base.Employee employee = this.managerIntegrate.GetEmployeeInfo(FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code);

                //获得本次收费起始发票号
                long returnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", true, ref invoiceNO, ref realInvoiceNO, ref errMsg);
                if (returnValue == -1)
                {
                    throw new Exception("获得本次收费起始发票号出错！" + errMsg);
                }
                if (string.IsNullOrEmpty(invoiceNO))
                {
                    throw new Exception("获取发票号出错！");
                }
                if (reg.Pact.PayKind.ID == "02")
                {
                    reg.SIMainInfo.InvoiceNo = invoiceNO;
                }

                #endregion

                reg.SIMainInfo.OwnCost = ownFee;
                reg.SIMainInfo.PayCost = decimal.Zero;
                reg.SIMainInfo.PubCost = pubFee;
                reg.SIMainInfo.TotCost = totFee;
                reg.SIMainInfo.BalNo = balanceNo;
                GDSI.CountryMedical.Model.FIN_IPR_SIINMAININFO_GD gdInfo = null;
                if (ydzf == "1")//移动支付流程
                {
                    GDSI.CountryMedical.DAL.QueryDAL queryDB = new GDSI.CountryMedical.DAL.QueryDAL();
                    gdInfo = queryDB.GetGDModelForIDAndBalanceNo(clinicCode, balanceNo);
                    if (gdInfo == null)
                    {
                        throw new Exception("医保处理出错：未查询到相关参保信息！");
                    }
                    if (string.IsNullOrEmpty(gdInfo.MDTRTID))
                    {
                        throw new Exception("医保处理出错：未查询到相关参保信息！");
                    }

                    Neusoft.HISFC.Models.Base.PactInfo pact1 = conMgr.GetPactUnitInfoByPactCode(gdInfo.PACT_CODE);
                    reg.Pact = pact1;
                    reg.SIMainInfo.InsuplcAdmdvs = gdInfo.INSUPLCADMDVS;
                }

                //获得当前系统时间
                DateTime nowTime = this.undrugManager.GetDateTimeFromSysDateTime();
                //汇总没有进行待遇计算时的费用总金额
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    //如果有已经有明细账户支付了,首先考虑只是自费患者,那么将自费调整为0, 账户支付调整为自费金额.
                    if (reg.Pact.ID == "1" && f.IsAccounted)
                    {
                        if (f.FT.OwnCost > 0)
                        {
                            f.FT.PayCost += f.FT.OwnCost;
                            f.FT.OwnCost = 0;
                        }
                    }

                    f.FeeOper.OperTime = nowTime;

                    //通过待遇算法处理，可能产生减免费用
                    formerTotCost += f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                }

                //重新计算待遇计算后的费用金额
                decimal rebateRate = 0;
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    // 通过待遇算法处理，可能产生减免费用
                    totCost += f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                    if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        overDrugCost += Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.ExcessCost);
                        selfDrugCost += Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.DrugOwnCost);
                    }
                    f.NoBackQty = f.Item.Qty;
                    rebateRate += f.FT.RebateCost;   //优惠金额
                }

                //ownCost = totCost - pubFee;
                //payCost += 0;
                //pubCost += pubFee;
                ownCost = totCost - reg.SIMainInfo.PubCost - reg.SIMainInfo.PayCost;
                payCost += reg.SIMainInfo.PayCost;
                pubCost += reg.SIMainInfo.PubCost;

                //所有金额保留2位小数
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(totCost, 2);
                ownCost = Neusoft.FrameWork.Public.String.FormatNumber(ownCost, 2);
                pubCost = Neusoft.FrameWork.Public.String.FormatNumber(pubCost, 2);
                payCost = Neusoft.FrameWork.Public.String.FormatNumber(payCost, 2);

                ////判断一下传递进来的费用 与 待遇算法的费用是否相等

                if (totFee != totCost || totFee != (pubFee + ownFee + psnAcctPay))// || pubCost != tcfee || rebateRate != yhfee)
                {
                    debugFee = "totFee:" + totFee + " totCost:" + totCost + " pubFee:" + pubFee + " ownFee:" + ownFee + " clinicCode" + clinicCode;
                    throw new Exception("本院收费系统的总费用与终端机的费用不符合,医生可能修改医嘱，请认真核对!");
                }


                return totFee;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取ct/mr维护常数规则
        /// </summary>
        /// <returns></returns>
        public Hashtable GetHsItemZt()
        {
            Hashtable hsItemZT = null;
            Neusoft.HISFC.BizLogic.Manager.Constant consManager = new Neusoft.HISFC.BizLogic.Manager.Constant();
            ArrayList alItemZT = consManager.GetAllList("ItemZT");
            if (alItemZT != null)
            {
                hsItemZT = new Hashtable();
                foreach (Neusoft.HISFC.Models.Base.Const conObj in alItemZT)
                {
                    Neusoft.FrameWork.Models.NeuObject obj = null;
                    if (!conObj.IsValid)
                    {
                        continue;
                    }
                    if (hsItemZT.ContainsKey(conObj.Name))
                    {
                        if (string.IsNullOrEmpty(conObj.Memo.Trim()))
                        {
                            continue;
                        }
                        string[] itemIDs = null;
                        //string[] temps = conObj.Memo.Split('&');

                        itemIDs = conObj.Memo.Split('|');
                        foreach (string itemID in itemIDs)
                        {
                            obj = new NeuObject();
                            obj.ID = itemID;
                            obj.Name = conObj.WBCode;//数量
                            switch (conObj.SortID.ToString())
                            {
                                case "0":
                                    obj.Memo = "每个项目收取";
                                    break;
                                case "1":
                                    obj.Memo = "第一个项目收取";
                                    break;
                                case "2":
                                    obj.Memo = "第二个项目起加收";
                                    break;
                                case "3":
                                    obj.Memo = "只收取一次";
                                    break;

                            }

                            //obj.Memo = temps[2];//公式 0 每个项目收取、1 第一个项目收取、2 第二个项目起加收
                            switch (conObj.SpellCode)
                            {
                                case "0":
                                    obj.User01 = "总量取整";
                                    break;
                                case "1":
                                    obj.User01 = "单个取整";
                                    break;
                                case "2":
                                    obj.User01 = "固定数量";
                                    break;
                            }
                            //obj.User01 = conObj.SpellCode;//0 总量取整、1 单个取整 2固定数量
                            switch (conObj.UserCode)
                            {
                                case "0":
                                    obj.User02 = "DR";
                                    break;
                                case "1":
                                    obj.User02 = "CT";
                                    break;
                            }
                            //obj.User02 = conObj.UserCode;//0 DR 1 CT

                            ((ArrayList)hsItemZT[conObj.Name]).Add(obj);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(conObj.Memo.Trim()))
                        {
                            continue;
                        }
                        ArrayList al = new ArrayList();
                        string[] itemIDs = null;
                        //string[] temps = conObj.Memo.Split('&');
                        itemIDs = conObj.Memo.Split('|');
                        foreach (string itemID in itemIDs)
                        {
                            obj = new NeuObject();
                            obj.ID = itemID;
                            obj.Name = conObj.WBCode;//数量
                            switch (conObj.SortID.ToString())
                            {
                                case "0":
                                    obj.Memo = "每个项目收取";
                                    break;
                                case "1":
                                    obj.Memo = "第一个项目收取";
                                    break;
                                case "2":
                                    obj.Memo = "第二个项目起加收";
                                    break;
                            }

                            //obj.Memo = temps[2];//公式 0 每个项目收取、1 第一个项目收取、2 第二个项目起加收
                            switch (conObj.SpellCode)
                            {
                                case "0":
                                    obj.User01 = "总量取整";
                                    break;
                                case "1":
                                    obj.User01 = "单个取整";
                                    break;
                                case "2":
                                    obj.User01 = "固定数量";
                                    break;
                            }
                            //obj.User01 = conObj.SpellCode;//0 总量取整、1 单个取整 2固定数量
                            switch (conObj.UserCode)
                            {
                                case "0":
                                    obj.User02 = "DR";
                                    break;
                                case "1":
                                    obj.User02 = "CT";
                                    break;
                            }
                            //obj.User02 = conObj.UserCode;//0 DR 1 CT

                            al.Add(obj);
                            hsItemZT.Add(conObj.Name, al);
                        }
                    }
                }
            }
            return hsItemZT;
        }

        /// <summary>
        /// 获得收费信息
        /// </summary>
        /// <param name="al"></param>
        /// <param name="reg"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public ArrayList GetFeeItemList(ArrayList al, Neusoft.HISFC.Models.Registration.Register reg, ref string errMsg, bool needRecipeItem = false)
        {
            Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
            string tempPayKindid = reg.Pact.PayKind.ID;
            reg.Pact = pactManager.GetPactUnitInfoByPactCode(reg.Pact.ID);
            reg.Pact.PayKind.ID = tempPayKindid;
            bool isFindDRFirst = false;
            bool isFindCTFirst = false;
            Hashtable hsDROnlyOneItem = new Hashtable();
            Hashtable hsCTOnlyOneItem = new Hashtable();
            decimal drCount = 0;
            ArrayList feeItemLists = new ArrayList();
            try
            {
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in al)
                {
                    if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
                    {
                        //非药品
                        Neusoft.HISFC.Models.Fee.Item.Undrug undrugItem = undrugManager.GetUndrugByCode(f.Item.ID);

                        f.Item.NeedConfirm = undrugItem.NeedConfirm;
                        Hashtable hsItemZT = this.GetHsItemZt();
                        f.Item.NeedConfirm = undrugItem.NeedConfirm;
                        if (undrugItem != null && undrugItem.UnitFlag == "1")
                        {
                            ArrayList alDetail = null;
                            if (hsItemZT.ContainsKey(f.Item.ID))
                            {
                                ArrayList alItem = (ArrayList)hsItemZT[f.Item.ID];
                                string type = (alItem[0] as NeuObject).User02;
                                if (type == "DR")
                                {
                                    alDetail = ConvertDRGroupToDetail(f, !isFindDRFirst, reg, ref hsDROnlyOneItem, ref drCount, ref errMsg);
                                    isFindDRFirst = true;
                                }
                                else if (type == "CT")
                                {
                                    alDetail = ConvertCTGroupToDetail(f, !isFindCTFirst, reg, ref hsCTOnlyOneItem, ref errMsg);
                                    isFindCTFirst = true;
                                }
                            }
                            else
                            {
                                alDetail = this.ConvertGroupToDetail(f, reg, ref errMsg);
                            }
                            if (alDetail == null)
                            {
                                errMsg = "【" + f.Item.Name + "】获得组套明细出错!" + errMsg;
                                //return null;
                                continue;
                            }

                            if (alDetail.Count == 0)
                            {
                                errMsg = "【" + f.Item.Name + "】是组合项目,但是没有维护明细或者明细已经停用!请与信息科联系!" + errMsg;
                                return null;
                            }
                            if (needRecipeItem)
                            {

                                foreach (object item in alDetail)
                                {
                                    FeeItemList feeItem = item as FeeItemList;
                                    if (feeItem != null)
                                    {
                                        feeItem.RecipeNO = f.RecipeNO;
                                        feeItem.RecipeFlag = f.RecipeFlag;
                                    }
                                }
                            }

                            feeItemLists.AddRange(alDetail);
                        }
                        else
                        {
                            feeItemLists.Add(f);
                        }
                    }
                    else
                    {
                        //药品
                        feeItemLists.Add(f);
                    }
                }
            }
            catch (Exception ex)
            { }
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                FeeItemList f1 = feeItemLists[i] as FeeItemList;
                if (hsDROnlyOneItem.ContainsKey(f1.Item.ID))
                {
                    feeItemLists.RemoveAt(i);
                }
                if (hsCTOnlyOneItem.ContainsKey(f1.Item.ID))
                {
                    if (hsCTOnlyOneItem[f1.Item.ID].ToString() != "true")
                    {
                        hsCTOnlyOneItem.Remove(f1.Item.ID);
                        hsCTOnlyOneItem.Add(f1.Item.ID, "true");
                    }
                    else
                    {
                        feeItemLists.RemoveAt(i);
                    }
                }
            }
            foreach (DictionaryEntry de in hsDROnlyOneItem)
            {
                FeeItemList f2 = de.Value as FeeItemList;
                feeItemLists.Add(f2);
            }
            return feeItemLists;

        }


        /// <summary>
        /// 返回项目比例
        /// </summary>
        /// <param name="pactId">合同单位编码</param>
        /// <param name="f">费用明细</param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Base.PactItemRate PactRate(Neusoft.HISFC.Models.Registration.Register r, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f, ref string errMsg)
        {
            Neusoft.HISFC.Models.Base.PactItemRate pRate = new Neusoft.HISFC.Models.Base.PactItemRate();
            pRate.Rate.RebateRate = 0;
            return pRate;
        }

        /// <summary>
        /// 把组套拆分成明细
        /// </summary>
        /// <param name="f"></param>
        /// <param name="isFirst"></param>
        /// <param name="hsOnlyOneItem"></param>
        /// <param name="drCount"></param>
        /// <returns></returns>
        private ArrayList ConvertDRGroupToDetail(FeeItemList f, bool isFirst, Neusoft.HISFC.Models.Registration.Register rInfo, ref Hashtable hsOnlyOneItem, ref decimal drCount, ref string errText)
        {

            ArrayList undrugCombList = this.undrugPackAgeManager.QueryUndrugZTBypackageCode(f.Item.ID);
            ArrayList alTemp = new ArrayList();
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + undrugPackAgeManager.Err;

                return null;
            }
            decimal price = 0;
            decimal priceSecond = 0; // {C41CAC71-0186-43cf-9167-2D33E4626D74}
            decimal count = 0;
            string feeCode = string.Empty;
            string itemType = string.Empty;
            decimal totCost = 0;
            FeeItemList feeDetail = null;
            if (f.Order.ID == null || f.Order.ID == string.Empty)
            {
                f.Order.ID = this.orderIntegrate.GetNewOrderID();
                if (f.Order.ID == null || f.Order.ID == string.Empty)
                {
                    errText = "获得医嘱流水号出错!";

                    return null;
                }
            }
            //操作科室
            string deptCode = f.ExecOper.Dept.ID;//(Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee).Dept.ID;
            if (string.IsNullOrEmpty(deptCode))
            {
                errText = "操作科室为空!";
                return null;
            }

            //加载项目
            DataSet dsItem = new DataSet();
            if (this.outpatientManager.QueryItemList(deptCode, f.Item.ID, ref dsItem) == -1)
            {
                errText = "获得项目列表出错!" + this.outpatientManager.Err;
                return null;
            }

            //有价格打折的
            DataRow rowFind;
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                errText = "查找组套明细出错!";
                return null;
            }
            rowFind = rowFinds[0];

            DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();
            int age = 0;
            int month = 0;
            int day = 0;
            this.outpatientManager.GetAge(rInfo.Birthday, nowTime, ref age, ref month, ref day);

            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
            string priceForm = rInfo.Pact.PriceForm;

            decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
            decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
            decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
            decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);

            decimal orgGroupPrice = 0;
            decimal priceGroup = this.ItemPrice.GetPrice(f.Item.ID, rInfo, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup, ref orgGroupPrice);

            decimal rate = f.Item.Price / orgGroupPrice;
            if (rate == 1)
            {
                rate = priceGroup / orgGroupPrice;
            }

            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                if (isFirst && undrugCombo.SortID == 2)
                {
                    //如果是第一个DR项目，并且细项是第二组起收的继续循环
                    continue;
                }
                else if (!isFirst && undrugCombo.SortID == 1)
                {
                    //如果不是第一个DR项目，并且细项是第一组收的继续循环
                    continue;
                }
                if (undrugCombo.SpellCode != "0")
                {
                    dsItem = new DataSet();
                    if (this.outpatientManager.QueryItemList(deptCode, undrugCombo.ID, ref dsItem) == -1)
                    {
                        errText = "获得项目列表出错!" + this.outpatientManager.Err;
                        return null;
                    }
                    DataRow rowFindZT;
                    DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                    rowFindZT = rowFindZTs[0];
                    string itemName = rowFindZT["ITEM_NAME"].ToString();
                    if (itemName.ToUpper().Contains("DR"))
                    {
                        drCount += NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;
                    }
                }
            }

            //符合项目明细的加成（减免）比例
            decimal itemRate = 1;
            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                if (isFirst && undrugCombo.SortID == 2)
                {
                    //如果是第一个DR项目，并且细项是第二组起收的继续循环
                    continue;
                }
                else if (!isFirst && undrugCombo.SortID == 1)
                {
                    //如果不是第一个DR项目，并且细项是第一组收的继续循环
                    continue;
                }
                dsItem = new DataSet();
                if (this.outpatientManager.QueryItemList(deptCode, undrugCombo.ID, ref dsItem) == -1)
                {
                    errText = "获得项目列表出错!" + this.outpatientManager.Err;
                    return null;
                }
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                feeDetail = new FeeItemList();

                feeCode = rowFindZT["FEE_CODE"].ToString();
                try
                {
                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;

                    decimal orgPrice = price;
                    itemRate = feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                    price = this.ItemPrice.GetPrice(undrugCombo.ID, rInfo, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice) * itemRate;
                    feeDetail.OrgPrice = orgPrice;
                }
                catch (Exception e)
                {
                    errText = e.Message;

                    return null;
                }

                //组合项目原本就有打折的
                //if (rate > 0)
                //{
                //    price *= rate;
                //}

                //根据优惠比例重新计算单价------------------------- 
                string errMsg = string.Empty;
                PactItemRate myRate = this.PactRate(rInfo, feeDetail, ref errMsg);
                if (myRate == null)
                {
                    errText = errMsg;
                    return null;
                }

                price *= 1 - myRate.Rate.RebateRate;
                //--------------------------------------------------
                count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

                //组套拆分成明细的时候，也保存两位小数
                //totCost = price * count;
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

                feeDetail.Patient = f.Patient.Clone();
                feeDetail.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
                feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
                feeDetail.Name = feeDetail.Item.Name;
                feeDetail.ID = feeDetail.Item.ID;
                itemType = rowFindZT["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "1")
                {
                    //feeDetail.Item.IsPharmacy = true;
                    feeDetail.Item.ItemType = EnumItemType.Drug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "2")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = true;
                }
                feeDetail.RecipeOper = f.RecipeOper.Clone();
                feeDetail.Item.Price = price;
                feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
                feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
                feeDetail.Item.MinFee.ID = feeCode;
                feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
                feeDetail.Item.Qty = count;
                feeDetail.Days = NConvert.ToDecimal(f.Days);
                feeDetail.FT.TotCost = totCost;
                //自费如此，如果加上公费需要重新计算!!!
                feeDetail.FT.OwnCost = totCost;
                feeDetail.ExecOper = f.ExecOper.Clone();
                feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
                //if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
                //{
                //    feeDetail.Item.IsNeedConfirm = true;
                //}
                //else
                //{
                //    feeDetail.Item.IsNeedConfirm = false;
                //}

                //feeDetail.Item.NeedConfirm = f.Item.NeedConfirm;

                if (string.IsNullOrEmpty(rowFindZT["CONFIRM_FLAG"].ToString()))
                {
                    feeDetail.Item.NeedConfirm = Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm.None;
                }
                else
                {
                    if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(rowFindZT["CONFIRM_FLAG"].ToString())))
                    {
                        feeDetail.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFindZT["CONFIRM_FLAG"].ToString());
                    }
                }

                feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

                feeDetail.Order.ID = f.Order.ID;

                feeDetail.UndrugComb.ID = f.Item.ID;
                feeDetail.UndrugComb.Name = f.Item.Name;
                feeDetail.UndrugComb.Qty = f.Item.Qty;

                feeDetail.Order.Combo.ID = f.Order.Combo.ID;
                feeDetail.Item.IsMaterial = f.Item.IsMaterial;
                feeDetail.RecipeSequence = f.RecipeSequence;
                feeDetail.FTSource = f.FTSource;
                feeDetail.FeePack = f.FeePack;
                if (rInfo.Pact.PayKind.ID == "03")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    if (pactRate == null)
                    {
                        pactRate = this.pactUnitItemRateManager.GetOnepPactUnitItemRateByItem(rInfo.Pact.ID, feeDetail.Item.ID);
                    }
                    if (pactRate != null)
                    {
                        if (pactRate.Rate.PayRate != rInfo.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeDetail.ItemRateFlag = "1";
                            }
                            else
                            {
                                //feeDetail.ItemRateFlag = "3";
                                feeDetail.ItemRateFlag = "2";
                            }
                        }
                        else
                        {
                            feeDetail.ItemRateFlag = "2";

                        }
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            //feeDetail.ItemRateFlag = "2";//DEL 30
                            feeDetail.ItemRateFlag = "3";
                        }
                    }
                    else
                    {
                        if (f.ItemRateFlag == "3")
                        {
                            //DEL 30
                            ////if (rowFindZT["ZF"].ToString() != "1")
                            ////{
                            ////    feeDetail.OrgItemRate = f.OrgItemRate;
                            ////    feeDetail.NewItemRate = f.NewItemRate;
                            ////    feeDetail.ItemRateFlag = "2";
                            ////}
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = f.ItemRateFlag;
                        }
                    }
                }

                //复合项目的用法赋给明细项目
                feeDetail.Order.Usage = f.Order.Usage;
                //使用原来的处方号
                //feeDetail.RecipeNO = f.RecipeNO;
                feeDetail.Order.ApplyNo = f.Order.ApplyNo;
                feeDetail.Order.Sample.ID = f.Order.Sample.ID;
                feeDetail.Order.Sample.Name = f.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

                if (undrugCombo.SpellCode == "0")
                {
                    //总量取整的，做标识
                    if (hsOnlyOneItem.ContainsKey(feeDetail.Item.ID))
                    {
                        FeeItemList temp = hsOnlyOneItem[feeDetail.Item.ID] as FeeItemList;
                        //temp.UndrugComb.User02 = (Neusoft.FrameWork.Function.NConvert.ToInt32(temp.UndrugComb.User02) + 1).ToString();
                        //if (Neusoft.FrameWork.Function.NConvert.ToInt32(temp.UndrugComb.User02) % 2 != 0)
                        //{
                        //    temp.Item.Qty += feeDetail.Item.Qty;
                        //    temp.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(temp.Item.Qty));
                        //    temp.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(temp.Item.Price * temp.Item.Qty, 2);
                        //    temp.FT.OwnCost = temp.FT.TotCost;
                        //}
                        //temp.Item.Qty += feeDetail.Item.Qty;
                        //temp.FT.TotCost += feeDetail.FT.TotCost;
                        //temp.FT.OwnCost += feeDetail.FT.OwnCost;

                        temp.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(drCount / 2));
                        temp.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(temp.Item.Price * temp.Item.Qty, 2);
                        temp.FT.OwnCost = temp.FT.TotCost;
                    }
                    else
                    {
                        //feeDetail.UndrugComb.User02 = "1";

                        feeDetail.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(drCount / 2));
                        feeDetail.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(feeDetail.Item.Price * feeDetail.Item.Qty, 2);
                        feeDetail.FT.OwnCost = feeDetail.FT.TotCost;

                        hsOnlyOneItem.Add(feeDetail.Item.ID, feeDetail);
                    }
                }

                alTemp.Add(feeDetail);
            }
            if (alTemp.Count > 0)
            {
                if (f.FT.RebateCost > 0)//有减免
                {
                    if (rInfo.Pact.PayKind.ID != "01")
                    {
                        errText = "暂时不允许非自费患者减免!";

                        return null;
                    }
                    //decimal rebateRate =
                    //    Neusoft.FrameWork.Public.String.FormatNumber(
                    //    f.FT.RebateCost / (f.FT.OwnCost + f.FT.RebateCost), 2);
                    //decimal tempFix = 0;
                    //decimal tempRebateCost = 0;
                    //foreach (FeeItemList feeTemp in alTemp)
                    //{
                    //    feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost + feeTemp.FT.RebateCost) * rebateRate;
                    //    tempRebateCost += feeTemp.FT.RebateCost;
                    //    feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                    //    feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    //}
                    //tempFix = f.FT.RebateCost - tempRebateCost;
                    //FeeItemList fFix = alTemp[0] as FeeItemList;
                    //fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                    //减免单独算
                    decimal rebateRate =
                        Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
                    decimal tempFix = 0;
                    decimal tempRebateCost = 0;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
                        tempRebateCost += feeTemp.FT.RebateCost;
                        //feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                        //feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    }
                    tempFix = f.FT.RebateCost - tempRebateCost;
                    FeeItemList fFix = alTemp[0] as FeeItemList;
                    fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                }
            }
            if (alTemp.Count > 0)
            {
                if (f.SpecialPrice > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.SpecialPrice = f.SpecialPrice;

                            break;
                        }
                    }
                }
            }
            if (alTemp.Count > 0)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.FT.User03 = f.FT.User03;

                            break;
                        }
                    }
                }
            }
            return alTemp;
        }

        /// <summary>
        /// 把组套拆分成明细
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private ArrayList ConvertCTGroupToDetail(FeeItemList f, bool isFirst, Neusoft.HISFC.Models.Registration.Register rInfo, ref Hashtable hsOnlyOneItem, ref string errText)
        {

            ArrayList undrugCombList = this.undrugPackAgeManager.QueryUndrugZTBypackageCode(f.Item.ID);
            ArrayList alTemp = new ArrayList();
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + undrugPackAgeManager.Err;

                return null;
            }
            decimal price = 0;
            decimal priceSecond = 0; // {C41CAC71-0186-43cf-9167-2D33E4626D74}
            decimal count = 0;
            string feeCode = string.Empty;
            string itemType = string.Empty;
            decimal totCost = 0;
            FeeItemList feeDetail = null;
            if (f.Order.ID == null || f.Order.ID == string.Empty)
            {
                f.Order.ID = this.orderIntegrate.GetNewOrderID();
                if (f.Order.ID == null || f.Order.ID == string.Empty)
                {
                    errText = "获得医嘱流水号出错!";

                    return null;
                }
            }
            //操作科室
            string deptCode = f.ExecOper.Dept.ID;//(Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee).Dept.ID;
            if (string.IsNullOrEmpty(deptCode))
            {
                errText = "操作科室为空!";
                return null;
            }

            //加载项目
            DataSet dsItem = new DataSet();
            dsItem = new DataSet();
            if (this.outpatientManager.QueryItemList(deptCode, f.Item.ID, ref dsItem) == -1)
            {
                errText = "获得项目列表出错!" + this.outpatientManager.Err;
                return null;
            }
            //有价格打折的
            DataRow rowFind;
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                errText = "查找组套明细出错!";
                return null;
            }
            rowFind = rowFinds[0];

            DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();
            int age = 0;
            int month = 0;
            int day = 0;
            this.outpatientManager.GetAge(rInfo.Birthday, nowTime, ref age, ref month, ref day);

            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
            string priceForm = rInfo.Pact.PriceForm;

            decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
            decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
            decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
            decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);

            decimal orgGroupPrice = 0;
            decimal priceGroup = this.ItemPrice.GetPrice(f.Item.ID, rInfo, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup, ref orgGroupPrice);

            decimal rate = f.Item.Price / orgGroupPrice;
            if (rate == 1)
            {
                rate = priceGroup / orgGroupPrice;
            }

            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                dsItem = new DataSet();
                if (this.outpatientManager.QueryItemList(deptCode, undrugCombo.ID, ref dsItem) == -1)
                {
                    errText = "获得项目列表出错!" + this.outpatientManager.Err;
                    return null;
                }
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                #region pacs项目收费新模式

                if (undrugCombo.SortID == 3)
                {
                    if (hsOnlyOneItem.ContainsKey(undrugCombo.ID))
                    {
                        continue;
                    }
                    else
                    {
                        string itemName = rowFindZT["ITEM_NAME"].ToString();
                        if (itemName.Contains("三维重建"))
                        {
                            if (!hsOnlyOneItem.ContainsValue("四维"))
                            {
                                hsOnlyOneItem.Add(undrugCombo.ID, "三维");
                            }
                            else
                            {
                                hsOnlyOneItem.Add(undrugCombo.ID, "true");
                            }
                        }
                        else if (itemName.Contains("四维重建"))
                        {
                            Hashtable hsTemp = hsOnlyOneItem.Clone() as Hashtable;
                            foreach (DictionaryEntry de in hsTemp)
                            {
                                if (de.Value.ToString() == "三维")
                                {
                                    hsOnlyOneItem.Remove(de.Key);
                                    hsOnlyOneItem.Add(de.Key.ToString(), "true");
                                }
                            }
                            hsOnlyOneItem.Add(undrugCombo.ID, "四维");
                        }
                        else
                        {
                            hsOnlyOneItem.Add(undrugCombo.ID, "其他");
                        }
                    }
                }

                #endregion
            }

            //符合项目明细的加成（减免）比例
            decimal itemRate = 1;
            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                //if (undrugCombo.SortID == 3)
                //{
                //    if (hsOnlyOneItem.ContainsKey(undrugCombo.ID))
                //    {
                //        if (hsOnlyOneItem[undrugCombo.ID].ToString() != "true")
                //        {
                //            hsOnlyOneItem.Remove(undrugCombo.ID);
                //            hsOnlyOneItem.Add(undrugCombo.ID, "true");
                //        }
                //        else
                //        {
                //            continue;
                //        }
                //    }
                //    else
                //    {
                //        continue;
                //    }
                //}
                dsItem = new DataSet();
                if (this.outpatientManager.QueryItemList(deptCode, undrugCombo.ID, ref dsItem) == -1)
                {
                    errText = "获得项目列表出错!" + this.outpatientManager.Err;
                    return null;
                }
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                feeDetail = new FeeItemList();

                feeCode = rowFindZT["FEE_CODE"].ToString();
                try
                {
                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;

                    decimal orgPrice = price;
                    itemRate = feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                    price = this.ItemPrice.GetPrice(undrugCombo.ID, rInfo, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice) * itemRate;
                    feeDetail.OrgPrice = orgPrice;
                }
                catch (Exception e)
                {
                    errText = e.Message;

                    return null;
                }

                //组合项目原本就有打折的
                //if (rate > 0)
                //{
                //    price *= rate;
                //}

                //根据优惠比例重新计算单价------------------------- 
                string errMsg = string.Empty;
                PactItemRate myRate = this.PactRate(rInfo, feeDetail, ref errMsg);
                if (myRate == null)
                {
                    errText = errMsg;
                    return null;
                }

                price *= 1 - myRate.Rate.RebateRate;
                //--------------------------------------------------
                count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

                //组套拆分成明细的时候，也保存两位小数
                //totCost = price * count;
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

                feeDetail.Patient = f.Patient.Clone();
                feeDetail.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
                feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
                feeDetail.Name = feeDetail.Item.Name;
                feeDetail.ID = feeDetail.Item.ID;
                itemType = rowFindZT["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "1")
                {
                    //feeDetail.Item.IsPharmacy = true;
                    feeDetail.Item.ItemType = EnumItemType.Drug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "2")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = true;
                }
                feeDetail.RecipeOper = f.RecipeOper.Clone();
                feeDetail.Item.Price = price;
                feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
                feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
                feeDetail.Item.MinFee.ID = feeCode;
                feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
                feeDetail.Item.Qty = count;
                feeDetail.Days = NConvert.ToDecimal(f.Days);
                feeDetail.FT.TotCost = totCost;
                //自费如此，如果加上公费需要重新计算!!!
                feeDetail.FT.OwnCost = totCost;
                feeDetail.ExecOper = f.ExecOper.Clone();
                feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
                //if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
                //{
                //    feeDetail.Item.IsNeedConfirm = true;
                //}
                //else
                //{
                //    feeDetail.Item.IsNeedConfirm = false;
                //}

                //feeDetail.Item.NeedConfirm = f.Item.NeedConfirm;

                if (string.IsNullOrEmpty(rowFindZT["CONFIRM_FLAG"].ToString()))
                {
                    feeDetail.Item.NeedConfirm = Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm.None;
                }
                else
                {
                    if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(rowFindZT["CONFIRM_FLAG"].ToString())))
                    {
                        feeDetail.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFindZT["CONFIRM_FLAG"].ToString());
                    }
                }

                feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

                feeDetail.Order.ID = f.Order.ID;

                feeDetail.UndrugComb.ID = f.Item.ID;
                feeDetail.UndrugComb.Name = f.Item.Name;
                feeDetail.UndrugComb.Qty = f.Item.Qty;

                feeDetail.Order.Combo.ID = f.Order.Combo.ID;
                feeDetail.Item.IsMaterial = f.Item.IsMaterial;
                feeDetail.RecipeSequence = f.RecipeSequence;
                feeDetail.FTSource = f.FTSource;
                feeDetail.FeePack = f.FeePack;
                if (rInfo.Pact.PayKind.ID == "03")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    if (pactRate == null)
                    {
                        pactRate = this.pactUnitItemRateManager.GetOnepPactUnitItemRateByItem(rInfo.Pact.ID, feeDetail.Item.ID);
                    }
                    if (pactRate != null)
                    {
                        if (pactRate.Rate.PayRate != rInfo.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeDetail.ItemRateFlag = "1";
                            }
                            else
                            {
                                //feeDetail.ItemRateFlag = "3";
                                feeDetail.ItemRateFlag = "2";
                            }
                        }
                        else
                        {
                            feeDetail.ItemRateFlag = "2";

                        }
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            //feeDetail.ItemRateFlag = "2";//DEL 30
                            feeDetail.ItemRateFlag = "3";
                        }
                    }
                    else
                    {
                        if (f.ItemRateFlag == "3")
                        {
                            //DEL 30
                            ////if (rowFindZT["ZF"].ToString() != "1")
                            ////{
                            ////    feeDetail.OrgItemRate = f.OrgItemRate;
                            ////    feeDetail.NewItemRate = f.NewItemRate;
                            ////    feeDetail.ItemRateFlag = "2";
                            ////}
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = f.ItemRateFlag;
                        }
                    }
                }

                //复合项目的用法赋给明细项目
                feeDetail.Order.Usage = f.Order.Usage;
                //使用原来的处方号
                //feeDetail.RecipeNO = f.RecipeNO;
                feeDetail.Order.ApplyNo = f.Order.ApplyNo;
                feeDetail.Order.Sample.ID = f.Order.Sample.ID;
                feeDetail.Order.Sample.Name = f.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

                alTemp.Add(feeDetail);
            }
            if (alTemp.Count > 0)
            {
                if (f.FT.RebateCost > 0)//有减免
                {
                    if (rInfo.Pact.PayKind.ID != "01")
                    {
                        errText = "暂时不允许非自费患者减免!";

                        return null;
                    }
                    //decimal rebateRate =
                    //    Neusoft.FrameWork.Public.String.FormatNumber(
                    //    f.FT.RebateCost / (f.FT.OwnCost + f.FT.RebateCost), 2);
                    //decimal tempFix = 0;
                    //decimal tempRebateCost = 0;
                    //foreach (FeeItemList feeTemp in alTemp)
                    //{
                    //    feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost + feeTemp.FT.RebateCost) * rebateRate;
                    //    tempRebateCost += feeTemp.FT.RebateCost;
                    //    feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                    //    feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    //}
                    //tempFix = f.FT.RebateCost - tempRebateCost;
                    //FeeItemList fFix = alTemp[0] as FeeItemList;
                    //fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                    //减免单独算
                    decimal rebateRate =
                        Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
                    decimal tempFix = 0;
                    decimal tempRebateCost = 0;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
                        tempRebateCost += feeTemp.FT.RebateCost;
                        //feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                        //feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    }
                    tempFix = f.FT.RebateCost - tempRebateCost;
                    FeeItemList fFix = alTemp[0] as FeeItemList;
                    fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                }
            }
            if (alTemp.Count > 0)
            {
                if (f.SpecialPrice > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.SpecialPrice = f.SpecialPrice;

                            break;
                        }
                    }
                }
            }
            if (alTemp.Count > 0)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.FT.User03 = f.FT.User03;

                            break;
                        }
                    }
                }
            }
            return alTemp;
        }

        /// <summary>
        /// 把组套拆分成明细
        /// </summary>
        /// <param name="f"></param>
        /// <param name="reg"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private ArrayList ConvertGroupToDetail(Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f, Neusoft.HISFC.Models.Registration.Register reg, ref string errMsg)
        {
            string tmpRecipeNo = f.RecipeNO;
            ArrayList undrugCombList = this.undrugPackAgeManager.QueryUndrugPackagesBypackageCode(f.Item.ID);
            ArrayList alTemp = new ArrayList();
            if (undrugCombList == null)
            {
                errMsg = "获得组套明细出错!" + this.undrugPackAgeManager.Err;
                return null;
            }
            if (undrugCombList.Count == 0)
            {
                return undrugCombList;
            }

            decimal price = 0;
            decimal priceSecond = 0;
            decimal count = 0;
            string feeCode = string.Empty;
            string itemType = string.Empty;
            decimal totCost = 0;
            Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeDetail = null;
            if (f.Order.ID == null || f.Order.ID == string.Empty)
            {
                f.Order.ID = this.orderIntegrate.GetNewOrderID();
                if (f.Order.ID == null || f.Order.ID == string.Empty)
                {
                    errMsg = "获得医嘱流水号出错!";
                    return null;
                }
            }

            //操作科室
            string deptCode = f.ExecOper.Dept.ID;//(Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee).Dept.ID;
            if (string.IsNullOrEmpty(deptCode))
            {
                errMsg = "操作科室为空!";
                return null;
            }

            //加载项目
            DataSet dsItem = new DataSet();
            dsItem = new DataSet();
            if (this.outpatientManager.QueryItemList(deptCode, f.Item.ID, ref dsItem) == -1)
            {
                errMsg = "获得项目列表出错!" + this.outpatientManager.Err;
                return null;
            }
            //有价格打折的
            DataRow rowFind;
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                errMsg = "查找组套明细出错!";
                return null;
            }

            rowFind = rowFinds[0];
            DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();
            int age = 0;
            int month = 0;
            int day = 0;
            this.outpatientManager.GetAge(reg.Birthday, nowTime, ref age, ref month, ref day);

            //价格形式
            string priceForm = reg.Pact.PriceForm;
            decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
            decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
            decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
            decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]); //NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]); 非药品没有购入价格
            //decimal priceGroup = this.feeIntegrate.GetPrice(priceForm, age, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup);


            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                dsItem = new DataSet();
                if (this.outpatientManager.QueryItemList(deptCode, undrugCombo.ID, ref dsItem) == -1)
                {
                    errMsg = "获得项目列表出错!" + this.outpatientManager.Err;
                    return null;
                }
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    errMsg = "查找组套明细出错!";
                    continue;
                }
                rowFindZT = rowFindZTs[0];

                feeDetail = new Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList();

                feeCode = rowFindZT["FEE_CODE"].ToString();
                try
                {

                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);  // NConvert.ToDecimal(rowFind["UNIT_PRICE"]);非药品没有购入价格
                    decimal orgPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;
                    decimal itemRate = 1;
                    itemRate = feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                    //price = this.feeIntegrate.GetPrice(priceForm, age, unitPrice, childPrice, SPPrice, purchasePrice);

                    price = this.feeIntegrate.GetPrice(undrugCombo.ID, reg, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                    feeDetail.OrgPrice = orgPrice;
                }
                catch (Exception e)
                {
                    errMsg = e.Message;
                    return null;
                }

                //组合项目原本就有打折的
                //中五打折不需要用计算的rate
                //if (priceGroup > 0)
                //{
                //    price *= f.Item.Price / priceGroup;
                //}
                //根据优惠比例重新计算单价
                errMsg = string.Empty;
                Neusoft.HISFC.Models.Base.PactItemRate myRate = FS.ZDWY.Internet.BP.OutPatient.NeusoftBussiness.PactRate(reg, feeDetail, ref errMsg);
                if (myRate == null)
                {
                    return null;
                }

                price *= 1 - myRate.Rate.RebateRate;
                count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

                //组套拆分成明细的时候，也保存两位小数
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

                feeDetail.Patient = f.Patient.Clone();
                feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
                feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
                feeDetail.Name = feeDetail.Item.Name;
                feeDetail.ID = feeDetail.Item.ID;
                itemType = rowFindZT["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    feeDetail.Item.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.UnDrug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "1")
                {
                    feeDetail.Item.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.Drug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "2")
                {
                    feeDetail.Item.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.UnDrug;
                    feeDetail.IsGroup = true;
                }
                feeDetail.RecipeOper = f.RecipeOper.Clone();
                feeDetail.Item.Price = price;
                feeDetail.OrgPrice = price;
                feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
                feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
                feeDetail.Item.MinFee.ID = feeCode;
                feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
                feeDetail.Item.Qty = count;
                feeDetail.Days = NConvert.ToDecimal(f.Days);
                feeDetail.FT.TotCost = totCost;
                //自费如此，如果加上公费需要重新计算!!!
                feeDetail.FT.OwnCost = totCost;
                feeDetail.ExecOper = f.ExecOper.Clone();
                feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
                if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
                {
                    feeDetail.Item.IsNeedConfirm = true;
                }
                else
                {
                    feeDetail.Item.IsNeedConfirm = false;
                }

                feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

                feeDetail.Order.ID = f.Order.ID;
                feeDetail.UndrugComb.ID = f.Item.ID;
                feeDetail.UndrugComb.Name = f.Item.Name;
                feeDetail.Order.Combo.ID = f.Order.Combo.ID;
                feeDetail.Item.IsMaterial = f.Item.IsMaterial;
                feeDetail.RecipeSequence = f.RecipeSequence;
                feeDetail.FTSource = f.FTSource;
                feeDetail.FeePack = f.FeePack;
                if (reg.Pact.PayKind.ID == "03")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    if (pactRate == null)
                    {
                        pactRate = this.pactUnitItemRateManager.GetOnepPactUnitItemRateByItem(reg.Pact.ID, feeDetail.Item.ID);
                    }
                    if (pactRate != null)
                    {
                        if (pactRate.Rate.PayRate != reg.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeDetail.ItemRateFlag = "1";
                            }
                            else
                            {
                                feeDetail.ItemRateFlag = "2";
                            }
                        }
                        else
                        {
                            feeDetail.ItemRateFlag = "2";

                        }
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                    }
                    else
                    {
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = f.ItemRateFlag;
                        }
                    }
                }

                //复合项目的用法赋给明细项目
                feeDetail.Order.Usage = f.Order.Usage;
                feeDetail.Order.ApplyNo = f.Order.ApplyNo;
                feeDetail.Order.Sample.ID = f.Order.Sample.ID;
                feeDetail.Order.Sample.Name = f.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

                //使用原来的处方号
                //feeDetail.RecipeNO = f.RecipeNO;
                feeDetail.Item.Memo = tmpRecipeNo;

                alTemp.Add(feeDetail);
            }

            if (alTemp.Count > 0)
            {
                if (f.FT.RebateCost > 0)//有减免
                {
                    if (reg.Pact.PayKind.ID != "01")
                    {
                        errMsg = "暂时不允许非自费患者减免!";
                        return null;
                    }
                    //减免单独算
                    decimal rebateRate = Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
                    decimal tempFix = 0;
                    decimal tempRebateCost = 0;
                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeTemp in alTemp)
                    {
                        feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
                        tempRebateCost += feeTemp.FT.RebateCost;
                    }
                    tempFix = f.FT.RebateCost - tempRebateCost;
                    Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList fFix = alTemp[0] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                    fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                }
            }
            if (alTemp.Count > 0)
            {
                if (f.SpecialPrice > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.SpecialPrice = f.SpecialPrice;

                            break;
                        }
                    }
                }
            }
            if (alTemp.Count > 0)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.FT.User03 = f.FT.User03;

                            break;
                        }
                    }
                }
            }
            return alTemp;
        }
        //明细单独项目限制收费计算规则
        protected virtual bool IsItemValid(ArrayList feeItemLists, ref string errMsg)
        {
            string tmpValue = "0";

            bool isJudgeValid = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.STOP_ITEM_WARNNING, false, false);
            if (!isJudgeValid) //如果不需要判断，默认都没有停用
            {
                return true;
            }

            foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in feeItemLists)
            {
                if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                {
                    Neusoft.HISFC.Models.Pharmacy.Item drugItem = this.pharmacyIntegrate.GetItem(f.Item.ID);
                    if (drugItem == null)
                    {
                        errMsg = "查询药品项目出错!" + this.pharmacyIntegrate.Err;
                        return false;
                    }
                    if (drugItem.IsStop)
                    {
                        errMsg = "【" + drugItem.Name + "】已经停用!请验证再收费!";
                        return false;
                    }
                }
                else
                {
                    Neusoft.HISFC.Models.Fee.Item.Undrug undrugItem = this.undrugManager.GetUndrugByCode(f.Item.ID);
                    if (undrugItem == null)
                    {
                        errMsg = "查询非药品项目出错!" + this.undrugManager.Err;
                        return false;
                    }
                    if (undrugItem.ValidState != "1")//停用
                    {
                        errMsg = "【" + undrugItem.Name + "】已经停用或废弃，请验证再收费!";
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amtCost">现金金额</param>
        /// <param name="pm">银行支付信息</param>
        /// <param name="psnAcctPay">个人账户支出</param>
        /// <param name="pubCost">医保基金支付</param>
        /// <param name="insuplcAdmdvs">参保区划</param>
        /// <returns></returns>
        private ArrayList QueryBalancePaysYDZF(decimal amtCost, Models.PLATFORM_BALANCE_PAY pm, decimal psnAcctPay, decimal pubCost, string insuplcAdmdvs)
        {
            //初始化支付方式信息
            Neusoft.FrameWork.Public.ObjectHelper helpPayMode = new Neusoft.FrameWork.Public.ObjectHelper();
            ArrayList alPayModes = this.managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.PAYMODES);
            if (alPayModes == null || alPayModes.Count <= 0)
            {
                return null;
            }
            helpPayMode.ArrayObject = alPayModes;

            //支付方式
            ArrayList balancePays = new ArrayList();
            Neusoft.HISFC.Models.Fee.Outpatient.BalancePay balancePay = null;
            string payType = string.Empty;
            switch (pm.PAYMODE)
            {
                case "2":
                    payType = "PTZFB";
                    break;
                case "1":
                    payType = "PTWX";
                    break;
                default:
                    throw new Exception("未对照的支付方式，请核实。");
            }
            //生成银行支付方式
            if (amtCost > 0)
            {
                balancePay = new Neusoft.HISFC.Models.Fee.Outpatient.BalancePay();

                //佛四特殊的支付方式 ?应该优化，设置成参数gumzh?
                balancePay.PayType.ID = payType;
                if (string.IsNullOrEmpty(helpPayMode.GetName(payType)))
                {
                    return null;
                }
                balancePay.PayType.Name = helpPayMode.GetName(payType);

                balancePay.FT.TotCost = amtCost;
                balancePay.FT.RealCost = balancePay.FT.TotCost;

                //银行基本信息
                balancePay.Bank.ID = "";
                balancePay.Bank.Name = pm.PAYTIME.ToString();  //交易日期时间
                balancePay.Bank.Account = pm.PAYAMT;  //金额
                balancePay.POSNO = "";     //银行交易流水号
                balancePay.Bank.InvoiceNO = "";    //银行交易批次号

                balancePays.Add(balancePay);
            }

            //个帐
            if (psnAcctPay > 0)
            {
                balancePay = new Neusoft.HISFC.Models.Fee.Outpatient.BalancePay();
                if (insuplcAdmdvs.StartsWith("4403") || insuplcAdmdvs.StartsWith("4415")|| !insuplcAdmdvs.StartsWith("44"))//虚账地区
                    balancePay.PayType.ID = "PBZH";
                else
                    balancePay.PayType.ID = "PTYBK";
                if (string.IsNullOrEmpty(helpPayMode.GetName(balancePay.PayType.ID)))
                {
                    return null;
                }
                balancePay.PayType.Name = helpPayMode.GetName(balancePay.PayType.ID);
                balancePay.FT.TotCost = psnAcctPay;
                balancePay.FT.RealCost = balancePay.FT.TotCost;
                //银行基本信息
                balancePay.Bank.ID = "";  //银行交易银行卡号
                balancePay.Bank.Name = pm.PAYTIME.ToString();  //交易日期时间
                balancePay.Bank.Account = pm.PAYAMT;  //金额
                balancePay.POSNO = "";       //银行交易流水号
                balancePay.Bank.InvoiceNO = "";    //银行交易批次号
                balancePays.Add(balancePay);
            }

            if (pubCost > 0)
            {
                balancePay = new Neusoft.HISFC.Models.Fee.Outpatient.BalancePay();

                //佛四特殊的支付方式 ?应该优化，设置成参数gumzh?
                balancePay.PayType.ID = "PBZH";
                if (string.IsNullOrEmpty(helpPayMode.GetName(balancePay.PayType.ID)))
                {
                    return null;
                }
                balancePay.PayType.Name = helpPayMode.GetName(balancePay.PayType.ID);

                balancePay.FT.TotCost = pubCost;
                balancePay.FT.RealCost = balancePay.FT.TotCost;

                //银行基本信息
                balancePay.Bank.ID = "";  //银行交易银行卡号
                balancePay.Bank.Name = "";  //交易日期时间
                balancePay.Bank.Account = pm.PAYAMT;  //金额
                balancePay.POSNO = "";       //银行交易流水号
                balancePay.Bank.InvoiceNO = "";    //银行交易批次号

                balancePays.Add(balancePay);
            }

            if (balancePays == null || balancePays.Count <= 0)
            {
                return null;
            }

            return balancePays;
        }

        /// <summary>
        /// 获得支付方式的集合
        /// </summary>
        /// <param name="amtCost">银行支付金额</param>
        /// <param name="hsBankInfo">银行支付信息</param>
        /// <param name="ecoCost">优惠金额</param>
        /// <param name="pubCost">公费金额，暂时不用生成支付方式</param>
        /// <returns></returns>
        private ArrayList QueryBalancePays(decimal amtCost, Models.PLATFORM_BALANCE_PAY pm, decimal ecoCost, decimal pubCost)
        {
            //初始化支付方式信息
            Neusoft.FrameWork.Public.ObjectHelper helpPayMode = new Neusoft.FrameWork.Public.ObjectHelper();
            ArrayList alPayModes = this.managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.PAYMODES);
            if (alPayModes == null || alPayModes.Count <= 0)
            {
                return null;
            }
            helpPayMode.ArrayObject = alPayModes;

            //支付方式
            ArrayList balancePays = new ArrayList();
            Neusoft.HISFC.Models.Fee.Outpatient.BalancePay balancePay = null;
            string payType = string.Empty;
            switch (pm.PAYMODE)
            {
                case "3":
                    payType = "PTYL";
                    break;
                case "2":
                    payType = "PTZFB";
                    break;
                case "1":
                    payType = "PTWX";
                    break;
                case "4":
                    payType = "PTYBK";
                    break;
                case "6":
                    payType = "YBXYF";
                    break;
                default:
                    payType = "PTWX";
                    break;
            }
            //生成银行支付方式
            if (amtCost > 0)
            {
                balancePay = new Neusoft.HISFC.Models.Fee.Outpatient.BalancePay();

                //佛四特殊的支付方式 ?应该优化，设置成参数gumzh?
                balancePay.PayType.ID = payType;
                if (string.IsNullOrEmpty(helpPayMode.GetName(payType)))
                {
                    return null;
                }
                balancePay.PayType.Name = helpPayMode.GetName(payType);

                balancePay.FT.TotCost = amtCost;
                balancePay.FT.RealCost = balancePay.FT.TotCost;

                //银行基本信息
                balancePay.Bank.ID = "";
                balancePay.Bank.Name = pm.PAYTIME.ToString();  //交易日期时间
                balancePay.Bank.Account = pm.PAYAMT;  //金额
                balancePay.POSNO = "";     //银行交易流水号
                balancePay.Bank.InvoiceNO = "";    //银行交易批次号

                balancePays.Add(balancePay);
            }

            //生成优惠支付方式
            //if (ecoCost > 0)
            //{
            //    balancePay = new BalancePay();
            //    balancePay.PayType.ID = "RC";
            //    if (string.IsNullOrEmpty(helpPayMode.GetName("RC")))
            //    {
            //        return null;
            //    }
            //    balancePay.PayType.Name = helpPayMode.GetName("RC");
            //    balancePay.FT.TotCost = ecoCost;
            //    balancePay.FT.RealCost = balancePay.FT.TotCost;
            //    balancePays.Add(balancePay);
            //}

            if (pubCost > 0)
            {
                balancePay = new Neusoft.HISFC.Models.Fee.Outpatient.BalancePay();

                //佛四特殊的支付方式 ?应该优化，设置成参数gumzh?
                balancePay.PayType.ID = "PBZH";
                if (string.IsNullOrEmpty(helpPayMode.GetName(balancePay.PayType.ID)))
                {
                    return null;
                }
                balancePay.PayType.Name = helpPayMode.GetName(balancePay.PayType.ID);

                balancePay.FT.TotCost = pubCost;
                balancePay.FT.RealCost = balancePay.FT.TotCost;

                //银行基本信息
                balancePay.Bank.ID = "";  //银行交易银行卡号
                balancePay.Bank.Name = "";  //交易日期时间
                balancePay.Bank.Account = pm.PAYAMT;  //金额
                balancePay.POSNO = "";       //银行交易流水号
                balancePay.Bank.InvoiceNO = "";    //银行交易批次号

                balancePays.Add(balancePay);
            }

            if (balancePays == null || balancePays.Count <= 0)
            {
                return null;
            }

            return balancePays;
        }

        void UpdateTerminalByInvoice(string invNo, string winNo)
        {
            string sql = @"update fin_opb_invoiceinfo a
                            set a.drug_window='{0}'
                            where a.invoice_no='{1}' ";
            try
            {
                sql = string.Format(sql, winNo, invNo);
                int i = outpatientManager.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("MzPk", ex.Message + "\n" + sql);
            }

        }

        #endregion

        #region 退费

        public Models.Views.ComResult<string> refundNotice(Models.PLATFORM_BALANCE_PAY billpay, Models.OperInfo oper)
        {
            Models.Views.ComResult<string> result = new Models.Views.ComResult<string>();
            FS.ZDWY.Internet.BL.OutPatient.PlatformBillLogic billlogic = new BL.OutPatient.PlatformBillLogic();

            //启动事务
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            registerManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            schMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            confirmIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            mateIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            billlogic.BeginTran();

            Neusoft.HISFC.Models.Registration.Register patient = new Neusoft.HISFC.Models.Registration.Register();
            try
            {
                Models.PLATFORM_BALANCE_PAY payobj = billlogic.Get(billpay.ORDERID);
                #region 获取发票 balances
                ArrayList balances = outpatientManager.QueryBalancesSameInvoiceCombNOByInvoiceNO(payobj.INVOICEID);

                //查询业务层出错
                if (balances == null)
                {
                    throw new Exception("查询发票出错!");
                }
                //没有找到纪录
                if (balances.Count == 0)
                {
                    throw new Exception("发票号不存在,请重新录入!");
                }
                #endregion

                #region 限制
                //获得是否可以退日结过费用控制参数
                bool isCanQuitDayBlanced = this.controlParamIntegrate.GetControlParam<bool>(Neusoft.HISFC.BizProcess.Integrate.Const.CAN_QUIT_DAYBALANCED_INVOICE, true, false);

                if (!isCanQuitDayBlanced)//不允许退日结过费用
                {
                    Neusoft.HISFC.Models.Fee.Outpatient.Balance tmpInvoice = balances[0] as Neusoft.HISFC.Models.Fee.Outpatient.Balance;

                    if (tmpInvoice == null)
                    {
                        throw new Exception("发票格式转换出错!");
                    }
                    if (tmpInvoice.IsDayBalanced)
                    {
                        throw new Exception("该发票已经日结,您没有权限进行退费!");
                    }
                }

                int canQuitDays = this.controlParamIntegrate.GetControlParam<int>(Neusoft.HISFC.BizProcess.Integrate.Const.VALID_QUIT_DAYS, true, 10000);

                DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();

                Neusoft.HISFC.Models.Fee.Outpatient.Balance tmpInvoiceValid = balances[0] as Neusoft.HISFC.Models.Fee.Outpatient.Balance;

                if (tmpInvoiceValid == null)
                {
                    throw new Exception("发票格式转换出错!");
                }

                int tempDays = (nowTime - tmpInvoiceValid.BalanceOper.OperTime).Days;

                if (tempDays >= canQuitDays)
                {
                    throw new Exception("该发票已经超出可退费天数,不允许退费!");
                }
                #endregion

                #region 退费

                int iReturn = 0;

                //获得负发票流水号
                string invoiceSeqNegative = outpatientManager.GetInvoiceCombNO();
                if (invoiceSeqNegative == null || invoiceSeqNegative == string.Empty)
                {
                    throw new Exception("获得发票流水号失败!" + outpatientManager.Err);
                }
                #region 记录作废发票的金额
                decimal CancelTotCost = 0; //作废发票的总金额
                decimal CancelOwnCost = 0;//作废发票的自费金额
                decimal CancelPayCost = 0;//作废发票的自付金额
                decimal CancelPubCost = 0;//作废发票的公费金额
                decimal CancelRebateCost = 0; // 作废优惠减免金额
                string InvoiceNO = "";
                #endregion

                // {69245A77-FB7A-42ed-844B-855E7ABC612F}
                bool blnIsAccountInvoice = false;

                //退费标记
                Neusoft.HISFC.Models.Base.CancelTypes cancelType = Neusoft.HISFC.Models.Base.CancelTypes.Canceled;

                //为了打退票，将发票明细存起来 {BB77678F-A3E1-4f62-9D8D-8D52C1C17F8B}
                ArrayList alInvoiceDetails = new ArrayList();


                foreach (Neusoft.HISFC.Models.Fee.Outpatient.Balance invoice in balances)
                {
                    // {69245A77-FB7A-42ed-844B-855E7ABC612F}
                    blnIsAccountInvoice = invoice.IsAccount;

                    #region 发票主表处理

                    InvoiceNO = invoice.Invoice.ID;
                    //如果是当前操作员并且没有日结 则为作废
                    if (invoice.IsDayBalanced == false && invoice.BalanceOper.ID.Equals(outpatientManager.Operator.ID))
                    {
                        cancelType = Neusoft.HISFC.Models.Base.CancelTypes.LogOut;
                    }

                    iReturn = outpatientManager.UpdateBalanceCancelType(invoice.Invoice.ID, invoice.CombNO, nowTime, cancelType);
                    if (iReturn == -1)
                    {
                        throw new Exception("作废原始发票信息出错!" + outpatientManager.Err);
                    }
                    if (iReturn == 0)
                    {
                        throw new Exception("该发票已经作废!");
                    }

                    //插入负纪录冲账
                    Neusoft.HISFC.Models.Fee.Outpatient.Balance invoClone = invoice.Clone();

                    CancelTotCost += invoClone.FT.TotCost;
                    CancelOwnCost += invoClone.FT.OwnCost;
                    CancelPayCost += invoClone.FT.PayCost;
                    CancelPubCost += invoClone.FT.PubCost;

                    invoClone.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                    invoClone.FT.TotCost = -invoClone.FT.TotCost;
                    invoClone.FT.OwnCost = -invoClone.FT.OwnCost;
                    invoClone.FT.PayCost = -invoClone.FT.PayCost;
                    invoClone.FT.PubCost = -invoClone.FT.PubCost;
                    invoClone.CancelType = cancelType;

                    invoClone.CanceledInvoiceNO = invoice.ID;
                    invoClone.CancelOper.ID = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                    invoClone.BalanceOper.ID = outpatientManager.Operator.ID;//日结需要 改为当前退费人
                    invoClone.BalanceOper.OperTime = nowTime;
                    invoClone.CancelOper.OperTime = nowTime;
                    invoClone.IsAuditing = false;
                    invoClone.AuditingOper.ID = string.Empty;
                    invoClone.AuditingOper.OperTime = DateTime.MinValue;
                    invoClone.IsDayBalanced = false;
                    invoClone.BalanceID = string.Empty;
                    invoClone.DayBalanceOper.OperTime = DateTime.MinValue;

                    invoClone.CombNO = invoiceSeqNegative;

                    iReturn = outpatientManager.InsertBalance(invoClone);
                    if (iReturn <= 0)
                    {
                        throw new Exception("插入发票冲账信息出错!!" + outpatientManager.Err);
                    }
                    #endregion

                    #region 发票明细信息处理
                    //处理发票明细表信息
                    ArrayList alInvoiceDetail = outpatientManager.QueryBalanceListsByInvoiceNOAndInvoiceSequence(invoice.Invoice.ID, invoice.CombNO);
                    if (alInvoiceDetail == null)
                    {
                        throw new Exception("获得发票明细出错!" + outpatientManager.Err);
                    }


                    //作废发票明细表信息
                    iReturn = outpatientManager.UpdateBalanceListCancelType(invoice.Invoice.ID, invoice.CombNO, nowTime, cancelType);
                    if (iReturn <= 0)
                    {
                        throw new Exception("作废发票明细出错!" + outpatientManager.Err);
                    }

                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.BalanceList d in alInvoiceDetail)
                    {
                        d.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                        d.BalanceBase.FT.OwnCost = -d.BalanceBase.FT.OwnCost;
                        d.BalanceBase.FT.PubCost = -d.BalanceBase.FT.PubCost;
                        d.BalanceBase.FT.PayCost = -d.BalanceBase.FT.PayCost;
                        d.BalanceBase.BalanceOper.OperTime = nowTime;
                        d.BalanceBase.BalanceOper.ID = outpatientManager.Operator.ID;
                        d.BalanceBase.CancelType = cancelType;
                        d.BalanceBase.IsDayBalanced = false;
                        d.BalanceBase.DayBalanceOper.ID = string.Empty;
                        d.BalanceBase.DayBalanceOper.OperTime = DateTime.MinValue;
                        //d.CombNO = invoiceSeqNegative;
                        ((Neusoft.HISFC.Models.Fee.Outpatient.Balance)d.BalanceBase).CombNO = invoiceSeqNegative;

                        iReturn = outpatientManager.InsertBalanceList(d);
                        if (iReturn <= 0)
                        {
                            throw new Exception("插入发票明细冲账信息出错!" + outpatientManager.Err);
                        }
                    }
                    #endregion

                    //为了打退票，将发票明细存起来 {D5FA97FA-8DBB-48e7-BF5B-8DF4049EEE2B}
                    alInvoiceDetails.Add(alInvoiceDetail);
                }

                Neusoft.HISFC.Models.Fee.Outpatient.Balance invoiceInfo = ((Neusoft.HISFC.Models.Fee.Outpatient.Balance)balances[0]);

                #region 处理支付信息
                ArrayList payList = new ArrayList();
                string choosePayMode = this.feeIntegrate.GetControlValue(Neusoft.HISFC.BizProcess.Integrate.Const.QUIT_PAY_MODE_SELECT, "1");
                ArrayList feePayMods = this.outpatientManager.QueryBalancePaysByInvoiceSequence(invoiceInfo.CombNO);

                if (feePayMods.Count >= 0)
                {
                    #region 新加的

                    int returnJValue = this.outpatientManager.UpdateBalancePayModeCancelType(invoiceInfo.Invoice.ID, invoiceInfo.CombNO, nowTime, cancelType);
                    if (returnJValue <= 0)
                    {
                        throw new Exception("作废发票支付方式出错!" + outpatientManager.Err);
                    }

                    int bpIdx = 0;
                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.BalancePay bp in feePayMods)
                    {
                        if (bp != null)
                        {
                            Neusoft.HISFC.Models.Fee.Outpatient.BalancePay objPay = bp.Clone();
                            if (bp.PayType.ID == "CD" || bp.PayType.ID == "DB")
                            {
                                //银联
                            }
                            #region
                            objPay.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                            objPay.FT.TotCost = -objPay.FT.TotCost;
                            objPay.FT.RealCost = -objPay.FT.RealCost;
                            objPay.FT.OwnCost = -objPay.FT.OwnCost;
                            objPay.InputOper.OperTime = nowTime;
                            objPay.Invoice.ID = InvoiceNO;
                            objPay.Squence = (99 - bpIdx).ToString();
                            objPay.InputOper.ID = outpatientManager.Operator.ID;
                            objPay.InvoiceCombNO = invoiceSeqNegative;
                            objPay.CancelType = cancelType;
                            objPay.IsChecked = false;
                            objPay.CheckOper.ID = string.Empty;
                            objPay.CheckOper.OperTime = DateTime.MinValue;
                            objPay.BalanceOper.ID = string.Empty;
                            //p.BalanceNo = 0;
                            objPay.IsDayBalanced = false;
                            objPay.IsAuditing = false;
                            objPay.AuditingOper.OperTime = DateTime.MinValue;
                            objPay.AuditingOper.ID = string.Empty;
                            #endregion
                            iReturn = outpatientManager.InsertBalancePay(objPay);
                            if (iReturn <= 0)
                            {
                                throw new Exception("插入支付负信息出错!" + outpatientManager.Err);
                            }
                            #region 账户新增(账户冲掉扣费金额)
                            if (objPay.PayType.ID == "YS")
                            {
                                //微信处理
                            }
                            //8abe2f72-4f10-4eea-934a-7f7d338ffc1d
                            else if (objPay.PayType.ID == "MCZH") //allan  卡退费
                            {
                                //微信处理
                            }
                            else if (objPay.PayType.ID == "MCDZ")//电子社保卡退费
                            {
                                //微信处理
                            }
                            //end 8abe2f72-4f10-4eea-934a-7f7d338ffc1d
                            #endregion


                            bpIdx++;

                            #region 对于减免、记账患者，处理减免、记账数据

                            if (objPay.PayType.ID != "RC" || objPay.PayType.ID != "JZ")
                            {
                                payList.Add(objPay);
                            }

                            #endregion
                        }
                    }
                    #endregion
                }
                #endregion

                bool isCashPay = false;//是否现金冲账

                #region 记录退费信息
                ArrayList alQuitFeeItemList = new ArrayList();
                if (false)
                {
                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemTemp in alQuitFeeItemList)
                    {
                        Neusoft.HISFC.Models.Order.OutPatient.Order orderTemp = orderIntegrate.GetOneOrder(feeItemTemp.Patient.ID, feeItemTemp.Order.ID.ToString());
                        if (orderTemp != null && orderTemp.Status == 1)
                        {
                            this.orderIntegrate.UpdateOrderBeCaceled(orderTemp);
                        }
                    }
                }

                #endregion

                //处理费用明细
                ArrayList alFeeDetail = outpatientManager.QueryFeeItemListsByInvoiceSequence(invoiceInfo.CombNO);
                if (alFeeDetail == null)
                {
                    throw new Exception("获得患者费用明细出错!" + outpatientManager.Err);
                }
                iReturn = outpatientManager.UpdateFeeItemListCancelType(invoiceInfo.CombNO, nowTime, cancelType);
                if (iReturn <= 0)
                {
                    throw new Exception("作废患者明细出错!" + outpatientManager.Err);
                }

                ArrayList oldFeeItemLists = new ArrayList();
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in alFeeDetail)
                {
                    iReturn = pharmacyIntegrate.CancelApplyOutClinic(f.RecipeNO, f.SequenceNO);
                    if (iReturn < 0)
                    {
                        throw new Exception("作废发药申请出错!药品可能已经发药，请刷新窗口重试");
                    }

                    if (f.IsConfirmed == false)
                    {
                        iReturn = confirmIntegrate.CancelConfirmTerminal(f.Order.ID, f.Item.ID);
                        if (iReturn < 0)
                        {
                            throw new Exception("作废终端申请出错!" + confirmIntegrate.Err);
                        }
                    }
                    else
                    {
                        throw new Exception("项目已经终端确认！");
                    }

                    oldFeeItemLists.Add(f.Clone());
                    f.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                    f.FT.OwnCost = -f.FT.OwnCost;
                    f.FT.PayCost = -f.FT.PayCost;
                    f.FT.PubCost = -f.FT.PubCost;
                    f.FT.TotCost = f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                    f.Item.Qty = -f.Item.Qty;
                    f.CancelType = cancelType;
                    f.FeeOper.ID = outpatientManager.Operator.ID;
                    f.FeeOper.OperTime = nowTime;
                    f.ChargeOper.OperTime = nowTime;
                    f.InvoiceCombNO = invoiceSeqNegative;
                    f.ConfirmedInjectCount = 0;
                    f.HosCode = Neusoft.FrameWork.Management.Connection.Hospital.ID;
                    //iReturn = outpatientManager.InsertFeeItemList(f);
                    iReturn = outpatientManager.InsertFeeItemListWithHosCode(f);
                    if (iReturn <= 0)
                    {
                        throw new Exception("插入费用明细冲帐信息出错!" + outpatientManager.Err);
                    }
                }

                //if (this.patient.Pact.PayKind.ID == "02" && DialogResult.Yes == MessageBox.Show("是否选择医保登记患者？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                Neusoft.HISFC.Models.Base.PactInfo pact = this.GetPactInfoByPactCode(patient.Pact.ID);
                if (patient.Pact.PayKind.ID == "02" && pact.IsUseInOutPatientFee == true)
                {

                }


                //{6FC43DF1-86E1-4720-BA3F-356C25C74F16}
                #region 更新退费申请退费标记


                #endregion

                ArrayList alMate = new ArrayList();
                List<string> MTCancleList = new List<string>();
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in alFeeDetail)
                {
                    iReturn = pharmacyIntegrate.CancelApplyOutClinic(f.RecipeNO, f.SequenceNO);
                    if (iReturn < 0)
                    {
                        throw new Exception("作废发药申请出错!药品可能已经发药，请刷新窗口重试");
                    }

                    if (f.IsConfirmed == false)
                    {
                        iReturn = confirmIntegrate.CancelConfirmTerminal(f.Order.ID, f.Item.ID);
                        if (iReturn < 0)
                        {
                            throw new Exception("作废终端申请出错!" + confirmIntegrate.Err);
                        }
                    }
                    else
                    {
                        throw new Exception("项目已经终端确认！");
                    }

                    if (f.Item.SysClass.ID.ToString() == "UC")
                    {
                        MTCancleList.Add(f.Order.ID);
                    }
                    //非对照的物资 {40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}
                    if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.MatItem)
                    {
                        alMate.Add(f);
                    }
                    else
                    {
                        if (f.MateList.Count > 0)
                        {
                            alMate.Add(f);
                        }
                    }

                }

                #region 对物资退费部分进行退库

                Neusoft.HISFC.BizLogic.MedicalTechnology.Appointment appMgr = new Neusoft.HISFC.BizLogic.MedicalTechnology.Appointment();
                MTCancleList.ForEach(t => appMgr.Cancle(t));
                if (alMate.Count > 0)
                {
                    //退库
                    if (mateIntegrate.MaterialFeeOutputBack(alMate) < 0)
                    {
                        throw new Exception("物资退库失败,\n" + mateIntegrate.Err);
                    }
                }
                #endregion


                if (true)
                {
                    #region 全退

                    //if (InterfaceManager.GetIOrder() != null)
                    //{
                    //    if (InterfaceManager.GetIOrder().SendFeeInfo(this.patient, alQuitFeeItemList, false) < 0)
                    //    {
                    //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    //        this.medcareInterfaceProxy.Rollback();
                    //        MessageBox.Show(this, "退费失败，请向系统管理员报告错误信息：" + InterfaceManager.GetIOrder().Err, "提示>>", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //        return -1;
                    //    }
                    //}

                    Neusoft.FrameWork.Management.PublicTrans.Commit();
                    billlogic.CommitTran();

                    #endregion
                }

                #region 通知发药机处方退费

                if (false)
                {
                    ArrayList feeList = new ArrayList();
                    string msg = string.Empty;
                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList item in alQuitFeeItemList)
                    {
                        if (item.ExecOper.Dept.ID == "9004")
                        {
                            feeList.Add(item);
                        }
                    }

                    if (feeList.Count > 0)
                    {
                        if ((new packagService.ZDWY.MzPackage().QuitFee(feeList, ref msg) == -1))
                        {

                        }
                    }
                }

                #endregion

                #endregion

                Models.Views.ComResult<string> res = new Models.Views.ComResult<string>();
                result.IsSuccessful = true;
                result.Message = "操作成功！";
                result.ReturnData = "1";
                return result;

            }
            catch (Exception e)
            {
                billlogic.RollbackTran();
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                result.IsSuccessful = false;
                result.Message = e.Message.ToString();
                result.ReturnData = "0";
                return result;
            }
        }


        /// <summary>
        /// 获得结算类别信息
        /// </summary>
        /// <param name="pactCode">合同单位代码</param>
        /// <returns>结算类别信息, null失败</returns>
        private Neusoft.HISFC.Models.Base.PactInfo GetPactInfoByPactCode(string pactCode)
        {
            Neusoft.HISFC.Models.Base.PactInfo p = null;

            p = pactManager.GetPactUnitInfoByPactCode(pactCode);
            if (p == null)
            {
                return null;
            }

            return p;
        }

        #endregion

        #region 医保预计算
        /// <summary>
        /// 医保预计算
        /// </summary>
        /// <returns></returns>
        public FS.ZDWY.Internet.Models.Views.ComResult<FS.ZDWY.Internet.Models.Views.OutPatient.HcareResult> BillHcare(string HOSPITALNUM,
                string VISITNO,
                string PATIENTID,
                string TRANNO,
                string PATIENTNAME,
                string PATIENTCARD,

                string FRONTPROVIDERID, string RECIPTTYPE, string RECIPTNO,
 FS.ZDWY.Internet.Models.Views.QueryPersonRequestModel reqModel,
                string settlementType
                )
        {
            FS.ZDWY.Internet.Models.Views.ComResult<FS.ZDWY.Internet.Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();
            decimal totCost = 0;
            try
            {
                if (string.IsNullOrEmpty(VISITNO))
                {
                    throw new Exception("VISITNO为空！");
                }
                string clinicCode = VISITNO;

                ArrayList comFeeItemLists = new ArrayList();  //费用集合
                Neusoft.HISFC.Models.Registration.Register reg = null; //患者基本信息

                reg = registerManager.GetByClinic(clinicCode);
                if (string.IsNullOrEmpty(reg.ID))
                {
                    throw new Exception("获取挂号信息出错");
                }
                if (!PATIENTNAME.Equals(reg.Name))
                {
                    throw new Exception("患者姓名不符！");
                }
                if (string.IsNullOrEmpty(reg.IDCard))
                {
                    reg.IDCard = PATIENTCARD;
                }

                //获取挂号的未收费项目信息
                List<string> reciptTypeList = RECIPTTYPE.Split('|').ToList();
                List<string> reciptNoList = RECIPTNO.Split('|').ToList();
                if (reciptNoList.Distinct().Count() > 1)
                {
                    throw new Exception("不同处方类型只能单独结算，不可组合结算");
                }
                if (reciptTypeList.Count != reciptNoList.Count)
                {
                    throw new Exception("处方单号个数与处方类型个数不符");
                }
                ArrayList al = new ArrayList();
                for (int i = 0; i < reciptTypeList.Count; i++)
                {
                    ArrayList altemp = outpatientManager.QueryChargedFeeItemListsByRecipeNoAndRecipeFlag(clinicCode, reciptTypeList[i], reciptNoList[i]);
                    al.AddRange(altemp);
                }

                string doctid = "";     //开方医生工号
                string deptid = "";    //开方科室
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in al)
                {
                    if (f.Item.IsMaterial)
                    {
                        continue;
                    }
                    doctid = f.RecipeOper.ID;
                    deptid = f.RecipeOper.Dept.ID;
                    if (string.IsNullOrEmpty(doctid))
                    {
                        throw new Exception("开方医生为空!");
                    }
                    if (string.IsNullOrEmpty(deptid))
                    {
                        throw new Exception("开方科室为空!");
                    }
                }

                //清空费用信息
                comFeeItemLists.Clear();
                string errMsg = "";
                comFeeItemLists = this.GetFeeItemList(al, reg, ref errMsg);
                if (comFeeItemLists == null || comFeeItemLists.Count <= 0)
                {
                    throw new Exception("您暂时无缴费信息!" + errMsg);
                }


                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in comFeeItemLists)
                {
                    // 通过待遇算法处理，可能产生减免费用
                    totCost += f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                    f.FT.TotCost = f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                }

                reg.SIMainInfo.TotCost = totCost;

                if (string.IsNullOrWhiteSpace(TRANNO))
                {
                    string invoiceNO = "";                //当前发票电脑号
                    string realInvoiceNO = string.Empty; //当前发票应刷号
                    if (settlementType == "1")//门慢
                    {
                        reg.Pact.ID = "252";
                    }
                    else if (settlementType == "0")//普通医保
                    {
                        reg.Pact.ID = "246";
                    }
                    else
                    {
                        throw new Exception("传入减免方式无效!");
                    }
                    Neusoft.HISFC.Models.Base.PactInfo pact = conMgr.GetPactUnitInfoByPactCode(reg.Pact.ID);
                    reg.Pact = pact;
                    Neusoft.HISFC.Models.Base.Employee employee = this.managerIntegrate.GetEmployeeInfo("00A105");
                    long returnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", true, ref invoiceNO, ref realInvoiceNO, ref errMsg);
                    if (returnValue == -1)
                    {
                        //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        throw new Exception("获取发票号失败！");
                    }
                    if (reg.Pact.PayKind.ID == "02")
                    {
                        reg.SIMainInfo.InvoiceNo = invoiceNO;

                    }
                    Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                    this.medcareInterfaceProxy.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                    //开始待遇事务
                    this.medcareInterfaceProxy.BeginTranscation();
                    //设置待遇的合同单位参数
                    this.medcareInterfaceProxy.SetPactCode(reg.Pact.ID);

                    this.medcareInterfaceProxy.IsLocalProcess = false;
                    reg.SIMainInfo.OpterType = "3";
                    reg.SIMainInfo.OpterCode = "00A105";
                    reg.SIMainInfo.OpterName = "微信";
                    Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";


                    //连接待遇接口
                    returnValue = this.medcareInterfaceProxy.Connect();
                    if (returnValue == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //医保回滚可能出错，此处提示
                        if (this.medcareInterfaceProxy.Rollback() == -1)
                        {
                            throw new Exception(this.medcareInterfaceProxy.ErrMsg);

                        }
                        this.medcareInterfaceProxy.Disconnect();
                        throw new Exception("医疗待遇接口连接失败!" + this.medcareInterfaceProxy.ErrMsg);

                    }
                    //调用医保预结算前,清空保存预结算金额字段.
                    reg.SIMainInfo.OwnCost = 0;
                    reg.SIMainInfo.PayCost = 0;
                    reg.SIMainInfo.PubCost = 0;
                    reg.SIMainInfo.TotCost = 0;
                    reg.SIMainInfo.SiPubCost = 0;//此处用于中山医保民政统筹
                    //删除本次因为错误或者其他原因上传的明细
                    returnValue = this.medcareInterfaceProxy.DeleteUploadedFeeDetailsAllOutpatient(reg);

                    //重新上传所有明细
                    returnValue = this.medcareInterfaceProxy.UploadFeeDetailsOutpatient(reg, ref comFeeItemLists);
                    if (returnValue == -1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        //医保回滚可能出错，此处提示
                        if (this.medcareInterfaceProxy.Rollback() == -1)
                        {
                            throw new Exception(this.medcareInterfaceProxy.ErrMsg);

                        }
                        this.medcareInterfaceProxy.Disconnect();
                        throw new Exception("上传费用明细失败!" + this.medcareInterfaceProxy.ErrMsg);

                    }
                    reg.SIMainInfo.InsuplcAdmdvs = reqModel.InsuplcAdmdvs;
                    reg.SIMainInfo.MdtrtCertType = reqModel.MdtrCertTyp;
                    reg.SIMainInfo.MdtrtCertNo = reqModel.MdtrtCertNo;
                    reg.SIMainInfo.CardSn = reqModel.CardSN;
                    reg.SIMainInfo.PsnCertType = reqModel.PsnCertType;
                    reg.SIMainInfo.Certno = reqModel.CertNo;
                    reg.SIMainInfo.enumCallAPIChannel = Neusoft.HISFC.Models.SIInterface.EnumCallAPIChannel.ZDWY_WX_MZJF;
                    returnValue = this.medcareInterfaceProxy.PreBalanceOutpatient(reg, ref comFeeItemLists);
                    if (returnValue == -1 || returnValue == 3)
                    {

                        string errmsg = this.medcareInterfaceProxy.ErrMsg;
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        this.medcareInterfaceProxy.Rollback();
                        this.medcareInterfaceProxy.Disconnect();
                        throw new Exception("获得医保结算信息失败!" + errmsg);

                    }

                    this.medcareInterfaceProxy.Commit();
                    this.medcareInterfaceProxy.Disconnect();
                    Neusoft.FrameWork.Management.PublicTrans.Commit();


                    #region 医保处理
                    //FS.ZDWY.Internet.Models.Views.OutPatient.ComPatient patient = new Models.Views.OutPatient.ComPatient();
                    //patient.OperTime = outpatientManager.GetDateTimeFromSysDateTime();
                    //patient.SSN = reg.IDCard;
                    //patient.Name = reg.Name;
                    //patient.IDCard = reg.IDCard;
                    //FS.ZDWY.Internet.BP.SI.OutPatient.GetPatientInfo GetPtient = new SI.OutPatient.GetPatientInfo();
                    //GDSI.Models.OutParam.OutParamBizh110001 res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                    //string[] GetinfoParams = new string[] { TRANNO, "13", "131" };
                    //if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                    //{
                    //    throw new Exception("【医保错误】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                    //}
                    //if (res110001.Personinfos.Count != 1)
                    //{
                    //    throw new Exception("【医保错误】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                    //}

                    ////Aka130 = "11";
                    ////Bka006 = "110";
                    //FS.ZDWY.Internet.BP.SI.OutPatient.Balance Hcareser = new SI.OutPatient.Balance();
                    //GDSI.Models.Personinfo patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                    //GDSI.Models.Spinfo cli = null;
                    //string Aaz267 = "1";
                    //string Bka006 = "131";
                    //string DiagCode = "";
                    //if (res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                    //{
                    //    cli = res110001.Spinfos[0] as GDSI.Models.Spinfo;
                    //    Aaz267 = cli.Aaz267;
                    //    Bka006 = cli.Bka006;
                    //    DiagCode = cli.Bka026;
                    //}
                    //GDSI.Models.OutParam.OutParamBizh110104 res110004 = new GDSI.Models.OutParam.OutParamBizh110104();
                    ////appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042,clinicCode,Bka026}
                    //string[] appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, TRANNO, "13", Bka006, "0", Aaz267, "", reg.ID, DiagCode };
                    //if (Hcareser.CallService(comFeeItemLists, ref res110004, appendParams) <= 0)
                    //{
                    //    throw new Exception("【医保错误】获取医保患者医保减免信息失败！原因：" + Hcareser.ErrorMsg);
                    //}

                    #endregion

                    Models.Views.OutPatient.HcareResult res = new Models.Views.OutPatient.HcareResult();
                    res.HcareAmount = 0;
                    res.ExpenseAmount = reg.SIMainInfo.PubCost * 100;
                    res.SelfAmount = reg.SIMainInfo.OwnCost * 100;
                    res.TotalAmount = (reg.SIMainInfo.TotCost) * 100;

                    //res.HcareAmount = 0;
                    //res.ExpenseAmount = (res110004.Payinfo.Bka832 + res110004.Payinfo.Akb066) * 100;
                    //res.SelfAmount = res110004.Payinfo.Bka831 * 100;
                    //res.TotalAmount = (patient.OwnDigFee + patient.RegFee) * 100;
                    res.BalanceNo = reg.SIMainInfo.BalNo;
                    res.regNO = reg.SIMainInfo.MdtrtID;
                    result.IsSuccessful = true;
                    result.Message = "";
                    result.ReturnData = res;

                    return result;
                }

                Models.Views.OutPatient.HcareResult own = new Models.Views.OutPatient.HcareResult();
                own.HcareAmount = 0;
                own.ExpenseAmount = 0;
                own.SelfAmount = totCost * 100;
                own.TotalAmount = totCost * 100;

                result.IsSuccessful = true;
                result.Message = "";
                result.ReturnData = own;
                return result;

            }
            catch (Exception e)
            {
                //if (totCost > 0)
                //{
                //    Models.Views.OutPatient.HcareResult own = new Models.Views.OutPatient.HcareResult();
                //    own.HcareAmount = 0;
                //    own.ExpenseAmount = 0;
                //    own.SelfAmount = totCost * 100;
                //    own.TotalAmount = totCost * 100;

                //    result.IsSuccessful = true;
                //    result.Message = e.Message.ToString();
                //    result.ReturnData = own;
                //    return result;
                //}
                //else
                //{
                //医保回滚可能出错，此处提示
                if (this.medcareInterfaceProxy.Rollback() == -1)
                {
                    throw new Exception(this.medcareInterfaceProxy.ErrMsg);

                }
                result.IsSuccessful = false;
                result.Message = e.Message.ToString();
                return result;
                //}
            }

        }
        #endregion

        public FS.ZDWY.Internet.Models.Views.ComResult<FS.ZDWY.Internet.Models.Views.OutPatient.HcareResult> Cancelbillhcare(string clincCode, string regNO, string balanceNo)
        {
            try
            {
                ArrayList alFeeDetail = new ArrayList();
                Neusoft.HISFC.Models.Registration.Register reg = null; //患者基本信息
                Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
                //自助终端统一维护成固定的一个员工工号：T00001
                Neusoft.HISFC.Models.Base.Employee employee = this.managerIntegrate.GetEmployeeInfo("00A105");
                Neusoft.FrameWork.Management.Connection.Operator = employee as Neusoft.FrameWork.Models.NeuObject;
                reg = this.registerManager.GetByClinic(clincCode);
                if (string.IsNullOrEmpty(reg.ID))
                {
                    throw new Exception("获取挂号信息出错");
                }
                GDSI.CountryMedical.DAL.QueryDAL queryDB = new GDSI.CountryMedical.DAL.QueryDAL();
                var gdInfo = queryDB.GetGDModelForIDAndBalanceNo(clincCode, balanceNo);
                if (gdInfo == null)
                {
                    throw new Exception("未查询到相关医保登记记录，无法撤销！");
                }
                if (gdInfo.MDTRTID != regNO)
                {
                    throw new Exception("根据顺序号查询出的就诊ID与传入就诊ID不一致，无法撤销！");
                }
                if ((gdInfo.VALID_FLAG == "1" && gdInfo.BALANCE_STATE == "1") || !string.IsNullOrEmpty(gdInfo.SETLID))
                {
                    throw new Exception("信息已结算，无法撤销！");
                }
                //Neusoft.HISFC.Models.Base.PactInfo pact = conMgr.GetPactUnitInfoByPactCode(gdInfo.PACT_CODE);
                //reg.Pact = pact;
                //reg.SIMainInfo.BalNo = balanceNo;
                //reg.SIMainInfo.RegNo = regNO;
                //reg.SIMainInfo.OpterType = "3";
                //reg.SIMainInfo.OpterCode = "00A105";
                //reg.SIMainInfo.OpterName = "微信";
                GDSI.Process pro = new GDSI.Process();
                GDSI.ZhuHaiSI.Model.RollBackModel rollBackModel = new GDSI.ZhuHaiSI.Model.RollBackModel();
                rollBackModel.IsZYApi = false;
                rollBackModel.rollBackChrgBchno = balanceNo;
                rollBackModel.rollBackClincCode = clincCode;
                rollBackModel.rollBackInsuplcAdmdvs = gdInfo.INSUPLCADMDVS;
                rollBackModel.rollBackIptOtpNo = clincCode + "_" + balanceNo;
                rollBackModel.rollBackMdtrtId = gdInfo.MDTRTID;
                rollBackModel.rollBackPsnNo = gdInfo.PSNNO;
                rollBackModel.rollBackSetlId = "";

                rollBackModel.EnumCallAPIChannel = Neusoft.HISFC.Models.SIInterface.EnumCallAPIChannel.ZDWY_WX_MZJF;
                if (pro.CancelBalance(rollBackModel) != 1)
                {
                    throw new Exception("撤销失败：" + pro.ErrMsg);
                }
                pro.Commit();
                pro.Disconnect();
                #region 弃用
                //Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                //this.medcareInterfaceProxy.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                //this.medcareInterfaceProxy.SetPactCode(reg.Pact.ID);
                //this.medcareInterfaceProxy.BeginTranscation();
                //this.medcareInterfaceProxy.IsLocalProcess = false;
                //long returnValue = medcareInterfaceProxy.Connect();
                //if (returnValue != 1)
                //{
                //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //    medcareInterfaceProxy.Rollback();
                //    medcareInterfaceProxy.Disconnect();
                //    throw new Exception("待遇接口初始化失败" + medcareInterfaceProxy.ErrMsg);

                //}
                //returnValue = medcareInterfaceProxy.DeleteUploadedFeeDetailsOutpatient(reg, ref alFeeDetail);
                //if (returnValue != 1)
                //{
                //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //    medcareInterfaceProxy.Rollback();
                //    medcareInterfaceProxy.Disconnect();
                //    throw new Exception("待遇接口上传退费明细失败" + medcareInterfaceProxy.ErrMsg);

                //}
                //returnValue = medcareInterfaceProxy.CancelBalanceOutpatient(reg, ref alFeeDetail);
                //if (returnValue != 1)
                //{
                //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //    medcareInterfaceProxy.Rollback();
                //    medcareInterfaceProxy.Disconnect();
                //    throw new Exception("待遇接口结算失败" + medcareInterfaceProxy.ErrMsg);

                //} 
                #endregion
                Models.Views.OutPatient.HcareResult own = new Models.Views.OutPatient.HcareResult();
                FS.ZDWY.Internet.Models.Views.ComResult<FS.ZDWY.Internet.Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();
                result.IsSuccessful = true;
                result.Message = "";
                result.ReturnData = own;
                return result;
            }
            catch (Exception e)
            {

                Models.Views.OutPatient.HcareResult own = new Models.Views.OutPatient.HcareResult();
                FS.ZDWY.Internet.Models.Views.ComResult<FS.ZDWY.Internet.Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();
                result.IsSuccessful = false;
                result.Message = e.Message.ToString();
                result.ReturnData = null;
                return result;
            }

        }

        #region 退费

        public void QuitFee(string url)
        {
            BL.OutPatient.PlatformBillRefundLogic refundlogic = new BL.OutPatient.PlatformBillRefundLogic();
            BL.OutPatient.PlatformBillLogic billlogic = new BL.OutPatient.PlatformBillLogic();
            FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
            //refundlogic.BeginTran();
            //billlogic.BeginTran();
            try
            {
                //查询要退费的数据
                List<FS.ZDWY.Internet.Models.PLATFORM_BALANCE_REFUNDPAY> refundlist = refundlogic.GetList(o => o.STATUS == "0");
                if (refundlist != null && refundlist.Count > 0)
                {
                    foreach (Models.PLATFORM_BALANCE_REFUNDPAY repay in refundlist)
                    {
                        List<Neusoft.HISFC.Models.Fee.Outpatient.BalancePay> paylist = mgr.QueryPayTypeByInvoiceNO(repay.INVOICEID);

                        if (paylist == null || paylist.Count == 0)
                        {
                            continue;
                        }

                        decimal tot = 0;
                        paylist.ForEach(o =>
                        {
                            if (o.PayType.ID == "PTWX" || o.PayType.ID == "PTYBK" || o.PayType.ID == "PTYL" || o.PayType.ID == "PTZFB")
                            { tot += o.FT.TotCost; }
                        });

                        Models.PLATFORM_BALANCE_PAY billpay = billlogic.Get(repay.ORDERID);

                        Neusoft.HISFC.Models.Fee.Outpatient.BalancePay oper = paylist[0];

                        //判断
                        //微信退费总金额 与 微信缴费总金额  不等就涉及退费其他费用
                        if (repay.PAYAMT != ((int)(Math.Abs(tot) * 100)).ToString())
                        {
                            repay.OPERCODE = oper.InputOper.ID;
                            repay.OPERTIME = oper.InputOper.OperTime;
                            repay.REMARK = "发票存在退现金情况！";
                            repay.STATUS = "-1";
                            refundlogic.Update(repay);

                            billpay.OPERCODE = oper.InputOper.ID;
                            billpay.OPERTIME = oper.InputOper.OperTime;
                            billpay.REMARK = "发票存在退现金情况！";
                            billlogic.Update(billpay);
                            return;
                        }

                        //费用一致，则调用 退费接口
                        BP.QuitFee.QuitFeeService service = new QuitFee.QuitFeeService();
                        service.Url = url;
                        string ero = "";
                        if (service.CallService(billpay, ref ero) != 1)
                        {
                            repay.OPERCODE = oper.InputOper.ID;
                            repay.OPERTIME = oper.InputOper.OperTime;
                            repay.REMARK = ero;
                            repay.STATUS = "-1";
                            refundlogic.Update(repay);

                            billpay.OPERCODE = oper.InputOper.ID;
                            billpay.OPERTIME = oper.InputOper.OperTime;
                            billpay.REMARK = ero;
                            billlogic.Update(billpay);
                            return;
                        }


                        repay.OPERCODE = oper.InputOper.ID;
                        repay.OPERTIME = oper.InputOper.OperTime;
                        repay.REFUNDTIME = oper.InputOper.OperTime;
                        repay.REMARK = ero;
                        repay.STATUS = "1";
                        refundlogic.Update(repay);

                        billpay.OPERCODE = oper.InputOper.ID;
                        billpay.OPERTIME = oper.InputOper.OperTime;
                        billpay.REFUNDTIME = oper.InputOper.OperTime;
                        billpay.REMARK = ero;
                        billpay.STATUS = "1";
                        billlogic.Update(billpay);
                        return;
                    }
                }
                //refundlogic.CommitTran();
                //billlogic.CommitTran();
            }
            catch (Exception e)
            {
                //refundlogic.RollbackTran();
                //billlogic.RollbackTran();
            }
        }

        public int QuitFeeTest(string url)
        {
            Models.PLATFORM_BALANCE_PAY billpay = new Models.PLATFORM_BALANCE_PAY();
            billpay.PATIENTID = "";
            billpay.HOSPITALNUM = "";
            billpay.PAYMODE = "";
            billpay.PAYAMT = "";
            //费用一致，则调用 退费接口
            BP.QuitFee.QuitFeeService service = new QuitFee.QuitFeeService();
            service.Url = url;
            string ero = "";
            if (service.CallService(billpay, ref ero) == -9)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public int QuitRegFee(string url)
        {
            BL.OutPatient.RegisterPayInfoLogic paylogic = new BL.OutPatient.RegisterPayInfoLogic();
            List<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY> PayList = paylogic.GetRefundList();
            BP.OutPatient.Register.Manager mgr = new Register.Manager();

            //更新平台数据
            mgr.UpdatePlaStatus();

            if (PayList == null || PayList.Count <= 0)
            {
                return -1;
            }
            BL.OutPatient.RegisterPayInfoLogic regpaylog = new BL.OutPatient.RegisterPayInfoLogic();
            BL.OutPatient.PlatformOrderLogic orderlogic = new BL.OutPatient.PlatformOrderLogic();

            foreach (FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY obj in PayList)
            {
                try
                {


                    ServiceLogManager.Write("挂号退费:" + obj.HospTradeId);
                    decimal fee = mgr.QuitRegFee(obj.HospTradeId);
                    if (!obj.SELFAMOUNT.Equals(fee))
                    {
                        obj.RefundReason = "患者退款非微信方式！";
                        ServiceLogManager.Write(obj.RefundReason);
                        regpaylog.Update(obj);
                        continue;
                    }

                    BP.QuitFee.QuitRegFeeService service = new QuitFee.QuitRegFeeService();
                    service.Url = url;
                    string ero = "";

                    BL.RegisterInfoLogic reg = new BL.RegisterInfoLogic();
                    FS.ZDWY.Internet.Models.FIN_OPR_REGISTER regobj = reg.GetList(o => o.TRANS_TYPE == "2" && o.INVOICE_NO == obj.HospTradeId).First();

                    FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER order = orderlogic.Get(obj.ORDERID);
                    if (regobj == null || string.IsNullOrEmpty(regobj.CLINIC_CODE))
                    {
                        obj.RefundReason = "未知错误！";
                        ServiceLogManager.Write(obj.RefundReason);
                        regpaylog.Update(obj);
                        continue;
                    }
                    if (order == null || string.IsNullOrEmpty(order.ORDERID))
                    {
                        obj.RefundReason = "orderid不存在！";
                        ServiceLogManager.Write(obj.RefundReason);
                        regpaylog.Update(obj);
                        continue;
                    }

                    //退费服务
                    if (service.CallService(obj, ref ero) != 1)
                    {
                        obj.RefundReason = ero;
                        regpaylog.Update(obj);
                        continue;
                    }

                    obj.PsRefOrdNum = obj.ORDERID;
                    obj.RefundReason = "退费";
                    obj.RefundOpercode = regobj.CANCEL_OPCD;
                    obj.RefundOpername = regobj.CANCEL_OPCD;
                    obj.PayRefTime = reg.GetDateTime();
                    regpaylog.Update(obj);

                    order.STATUS = "4";
                    orderlogic.Update(order);
                    ServiceLogManager.Write(obj.RefundReason + obj.HospTradeId);
                    continue;
                }
                catch (Exception ex)
                {
                    ServiceLogManager.Write(ex.Message);
                }
            }
            return 1;
        }

        public ServiceResult QuitRegFeeByClinicCode(string url, string clinicCode)
        {
            try
            {


                //获取已退费挂号信息
                BL.RegisterInfoLogic reg = new BL.RegisterInfoLogic();
                FS.ZDWY.Internet.Models.FIN_OPR_REGISTER regobj = reg.GetList(o => o.TRANS_TYPE == "2" && o.CLINIC_CODE == clinicCode).First();
                if (regobj == null || string.IsNullOrEmpty(regobj.CLINIC_CODE))
                {
                    return new ServiceResult(false, "未找到退号记录", null);
                }


                BL.OutPatient.RegisterPayInfoLogic paylogic = new BL.OutPatient.RegisterPayInfoLogic();
                FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY obj = paylogic.GetRefundPayInfoByClinicCode(clinicCode);
                FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();

                if (obj == null)
                {
                    return new ServiceResult(false, "未找到支付记录", null);
                }
                BL.OutPatient.RegisterPayInfoLogic regpaylog = new BL.OutPatient.RegisterPayInfoLogic();
                BL.OutPatient.PlatformOrderLogic orderlogic = new BL.OutPatient.PlatformOrderLogic();





                ServiceLogManager.Write("挂号退费:" + obj.HospTradeId);
                decimal fee = mgr.QuitRegFee(obj.HospTradeId);
                if (!obj.SELFAMOUNT.Equals(fee))
                {
                    obj.RefundReason = "患者退款非微信方式！非原路退款。";
                    ServiceLogManager.Write(obj.RefundReason);
                    regpaylog.Update(obj);
                    return new ServiceResult(false, "患者退款非微信方式", null);
                }

                BP.QuitFee.QuitRegFeeService service = new QuitFee.QuitRegFeeService();
                service.Url = url;
                string ero = "";


                FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER order = orderlogic.Get(obj.ORDERID);

                if (order == null || string.IsNullOrEmpty(order.ORDERID))
                {
                    obj.RefundReason = "orderid不存在！";
                    ServiceLogManager.Write(obj.RefundReason);
                    regpaylog.Update(obj);
                    return new ServiceResult(false, "orderid不存在", null);
                }

                //退费服务
                if (service.CallService(obj, ref ero) != 1)
                {
                    obj.RefundReason = ero;
                    regpaylog.Update(obj);
                    return new ServiceResult(false, ero, null);
                }

                obj.PsRefOrdNum = obj.ORDERID;
                obj.RefundReason = "退费";
                obj.RefundOpercode = regobj.CANCEL_OPCD;
                obj.RefundOpername = regobj.CANCEL_OPCD;
                obj.PayRefTime = reg.GetDateTime();
                regpaylog.Update(obj);

                order.STATUS = "4";
                orderlogic.Update(order);
                ServiceLogManager.Write(obj.RefundReason + obj.HospTradeId);
                return new ServiceResult();
            }
            catch (Exception ex)
            {
                ServiceLogManager.Write(ex.Message);
                return new ServiceResult(ex);
            }


        }
        #endregion
    }
}
