using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SqlSugar;
using System.Collections;
using Neusoft.FrameWork.Function;
using FS.ZDWY.Internet.Models.Views.OutPatient;
using FS.ZDWY.Internet.Models;
using FS.ZDWY.Internet.BL;
using FS.ZDWY.Internet.BL.OutPatient;

namespace FS.ZDWY.Internet.BP.OutPatient
{

    /// <summary>
    /// 门诊开单接口管理类
    /// </summary>
    public class OrderManager
    {
        NeusoftBussiness neusoftBussiness = new NeusoftBussiness();

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
                            and empl.valid_state='1'";
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
                if (schema.DEPT_CODE != order.DEPTCODE)
                {
                    throw new Exception("排班信息不正确【科室信息】，请核对");
                }
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
                if (patient.SEX_CODE != ConvertHISSexCode(order.SEX))
                {
                    throw new Exception("患者信息不正确【性别】，请核对");
                }
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
                if (patient.BIRTHDAY.Date.AddDays(14 * 365) > mgr.GetDateTimeFromSysDateTime())
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
                string visitAddressSql = @"select (select n.remark from MET_NUO_CONSOLE n where n.console_code=m.console_code and rownum=1) 
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
        public int CancelLock(string orderId, string hospitalNum, string patientId, string cancelReason, string frontProviderId, DateTime cancelTime, ref string error)
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
        public Models.Views.ComResult<Models.Views.OutPatient.PayResult> RegisterPay(Models.PLATFORM_REGISTER_PAY regpay)
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
            //医保取消
            string SiTransid = "";
            Neusoft.HISFC.Models.Registration.Register cancelreg = null;

            Models.Views.OutPatient.ComPatient cancelpatient = null;
            string cancelBka006 = "";
            string cancelAka130 = "";
            if (regpay.HCAREAMOUNT == 0 && regpay.EXPENSEAMOUNT == 0)
            {
                regpay.TransNo = "";
            }
            if ((regpay.HCAREAMOUNT > 0 || regpay.EXPENSEAMOUNT > 0) && string.IsNullOrEmpty(regpay.TransNo))
            {
                throw new Exception("存在报销金额，请传入【TransNo】");
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
                patient.Pact.ID = "1";
                patient.Pact.PayKind.ID = "01";
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
                            throw new Exception("获取合同单位信息出错！");
                        }
                    }
                    else
                    {
                        throw new Exception("获取合同单位信息出错！");
                    }
                }
                else
                {
                    throw new Exception("获取合同单位信息出错！");
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

                #region 挂号限制

                //14岁
                if (string.IsNullOrEmpty(patient.Birthday))
                {
                    DateTime dd = DateTime.MinValue;
                    if (DateTime.TryParse(patient.Birthday, out dd))
                        if (dd.AddDays(14 * 365) > mgr.GetDateTimeFromSysDateTime())
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
                        if (string.IsNullOrEmpty(patient.Queue.ID) || string.IsNullOrEmpty(patient.NurseCell.ID))
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

                #region 医保减免接口
                if (!string.IsNullOrWhiteSpace(regpay.TransNo.Trim()))
                {
                    #region 医保接口
                    string Aaz267 = "1";
                    //string Bka006 = "110";
                    string DiagCode = "";

                    FS.ZDWY.Internet.BP.SI.OutPatient.GetPatientInfo GetPtient = new SI.OutPatient.GetPatientInfo();
                    GDSI.Models.OutParam.OutParamBizh110001 res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                    GDSI.Models.Personinfo patientinfo = null;

                    #region menman
                    bool isspecial = false;
                    //FS.ZDWY.Internet.BP.SI.OutPatient.GetPatientInfo GetPtient = new SI.OutPatient.GetPatientInfo();
                    //GDSI.Models.OutParam.OutParamBizh110001 res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                    string[] GetinfoParams = new string[] { regpay.TransNo, "13", "131" };
                    if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                    {
                        isspecial = false;
                    }
                    else
                    {
                        if (res110001.Personinfos.Count == 1)
                        {
                            if (res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                            {
                                isspecial = true;
                            }
                            else
                            {
                                isspecial = false;
                            }
                        }
                        else if (res110001.Personinfos.Count > 1)
                        {
                            GDSI.Models.Personinfo temp = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                            patient.SSN = temp.Aac001;
                            if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                            {
                                isspecial = false;
                            }
                            else
                            {
                                if (res110001.Personinfos != null && res110001.Personinfos.Count > 0 && res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                                {
                                    isspecial = true;
                                }
                                else
                                {
                                    isspecial = false;
                                }
                            }
                        }
                        else
                        {
                            //if (res110001.Personinfos != null && res110001.Personinfos.Count > 0 && res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                            //{
                            //    isspecial = true;
                            //}
                            isspecial = false;
                        }
                    }
                    if (isspecial)
                    {
                        patient.Pact.PayKind.ID = "02";
                        patient.Pact.ID = "252";
                        patient.Pact.Name = "广东医保门诊慢性病（门诊）";

                        //if (res110001.Personinfos.Count != 1)
                        //{
                        //    throw new Exception("【医保错误】【医保挂号】获取患者医保信息过多或无！请在收费窗口挂号！");
                        //}

                        //if (res110001.Spinfos.Count != 1)
                        //{
                        //    throw new Exception("【医保错误】【医保挂号】获取医保患者信息失败！原因：病种信息多或无！请在收费窗口挂号！");
                        //}

                        //GDSI.Models.Personinfo 
                        patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                        GDSI.Models.Spinfo cli = null;
                        if (res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                        {
                            cli = res110001.Spinfos[0] as GDSI.Models.Spinfo;
                            Aaz267 = cli.Aaz267;
                            //Bka006 = cli.Bka006;
                            DiagCode = cli.Bka026;
                            cancelBka006 = cli.Bka006;
                            cancelAka130 = "13";
                        }
                    }
                    #endregion
                    else
                    {
                        patient.Pact.PayKind.ID = "02";
                        patient.Pact.ID = "246";
                        patient.Pact.Name = "广东医保（门诊）";
                        //FS.ZDWY.Internet.BP.SI.OutPatient.GetPatientInfo GetPtient = new SI.OutPatient.GetPatientInfo();
                        res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                        GetinfoParams = new string[] { regpay.TransNo, "11", "110" };
                        if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                        {
                            throw new Exception("【医保错误】【医保挂号】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                        }

                        if (res110001.Personinfos.Count > 1)
                        {
                            GDSI.Models.Personinfo temp = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                            patient.SSN = temp.Aac001;
                            if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                            {
                                throw new Exception("【医保错误】【医保减免接口-2】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                            }
                        }
                        //if (res110001.Personinfos.Count != 1)
                        //{
                        //    throw new Exception("【医保错误】【医保挂号】获取患者医保信息过多或无！请在收费窗口挂号！");
                        //}

                        //GDSI.Models.Personinfo 
                        patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                        Aaz267 = "1";
                        //Bka006 = "110";
                        DiagCode = "";
                        cancelBka006 = "110";
                        cancelAka130 = "11";
                    }
                    patient.OperTime = mgr.GetDateTimeFromSysDateTime();

                    cancelpatient = patient;//取消挂号
                    //Aka130 = "11";
                    //Bka006 = "110";
                    //appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042}
                    FS.ZDWY.Internet.BP.SI.OutPatient.Register Hcareser = new SI.OutPatient.Register();
                    //GDSI.Models.Personinfo patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                    //GDSI.Models.Clinicapplyinfo cli = res110001.Clinicapplyinfos[0] as GDSI.Models.Clinicapplyinfo;
                    GDSI.Models.OutParam.OutParamBizh110104 res110004 = new GDSI.Models.OutParam.OutParamBizh110104();

                    //string[] appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, regpay.TransNo, "11", "110", "1", cli.Aaz267, "" };

                    string[] appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, regpay.TransNo, cancelAka130, cancelBka006, "1", Aaz267, "" };

                    //if (Hcareser.CallService(patient, ref res110004, appendParams) <= 0)
                    //{
                    //    throw new Exception("【医保错误】【医保挂号】减免医保信息失败！原因：" + Hcareser.ErrorMsg);
                    //}

                    int resint = Hcareser.CallService(patient, ref res110004, appendParams);
                    if (resint == -2 || Hcareser.ErrorMsg.Contains("重新调用功能号bizh110001"))
                    {
                        res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                        GetinfoParams = new string[] { regpay.TransNo, cancelAka130, cancelBka006 };
                        if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                        {
                            throw new Exception("【医保错误】【医保2次减免接口】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                        }

                        if (res110001.Personinfos.Count > 1)
                        {
                            GDSI.Models.Personinfo temp = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                            patient.SSN = temp.Aac001;
                            if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                            {
                                throw new Exception("【医保错误】【医保2次减免接口】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                            }
                        }

                        patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                        appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, regpay.TransNo, cancelAka130, cancelBka006, "0", Aaz267, "" };
                        if (Hcareser.CallService(patient, ref res110004, appendParams) <= 0)
                        {
                            throw new Exception("【医保错误】【医保2次减免接口】获取医保患者医保减免信息失败！原因：" + Hcareser.ErrorMsg);
                        }
                    }
                    else if (resint < 0)
                    {
                        throw new Exception("【医保错误】【医保减免接口-1】获取医保患者医保减免信息失败！原因：" + Hcareser.ErrorMsg);
                    }

                    SiTransid = res110004.Payinfo.Aaz218;
                    Models.Views.OutPatient.HcareResult HcareRes = new Models.Views.OutPatient.HcareResult();
                    HcareRes.HcareAmount = (res110004.Payinfo.Bka832 + res110004.Payinfo.Akb066);
                    HcareRes.ExpenseAmount = 0;
                    HcareRes.SelfAmount = res110004.Payinfo.Bka831;
                    HcareRes.TotalAmount = (patient.OwnDigFee + patient.RegFee);

                    #endregion
                    #region 插入数据库
                    Neusoft.HISFC.BizLogic.Registration.Register regMgr = new Neusoft.HISFC.BizLogic.Registration.Register();
                    Neusoft.HISFC.Models.Registration.Register reg = regMgr.GetByClinic(booking.REG_ID);
                    reg.SIMainInfo.RegNo = res110004.Payinfo.Aaz218;

                    reg.ID = patient.ClinicCode;
                    reg.SIMainInfo.RegNo = res110004.Payinfo.Aaz218;
                    reg.InvoiceNO = patient.InvoiceStr;
                    reg.PID.CardNO = patient.CardNo;
                    reg.SSN = patient.SSN;
                    reg.Name = patient.Name;
                    reg.IDCard = patient.IDCard;
                    reg.ClinicDiagnose = "";
                    reg.Pact.PayKind.ID = "M";
                    reg.Pact.ID = patient.Pact.ID;
                    reg.Pact.Name = "01";
                    reg.SIMainInfo.TotCost = res110004.Payinfo.Akc264;
                    reg.SIMainInfo.PubCost = res110004.Payinfo.Bka832;
                    reg.SIMainInfo.OwnCost = res110004.Payinfo.Bka831;
                    reg.Sex.ID = patient.SexCode;
                    reg.DoctorInfo.Templet.Dept.ID = patient.Dept.ID;
                    reg.PVisit.InTime = patient.RegDate;
                    reg.SIMainInfo.BalanceDate = patient.RegDate;

                    reg.SIMainInfo.Bka825 = res110004.Payinfo.Bka825;
                    reg.SIMainInfo.Bka826 = res110004.Payinfo.Bka826;
                    reg.SIMainInfo.Aka151 = res110004.Payinfo.Aka151;
                    reg.SIMainInfo.Bka838 = res110004.Payinfo.Bka838;
                    reg.SIMainInfo.Akb067 = res110004.Payinfo.Akb067;
                    reg.SIMainInfo.Akb066 = res110004.Payinfo.Akb066;
                    reg.SIMainInfo.Bka821 = res110004.Payinfo.Bka821;
                    reg.SIMainInfo.Bka839 = res110004.Payinfo.Bka839;
                    reg.SIMainInfo.Ake039 = res110004.Payinfo.Ake039;
                    reg.SIMainInfo.Ake035 = res110004.Payinfo.Ake035;
                    reg.SIMainInfo.Ake026 = res110004.Payinfo.Ake026;
                    reg.SIMainInfo.Ake029 = res110004.Payinfo.Ake029;
                    reg.SIMainInfo.Bka841 = res110004.Payinfo.Bka841;
                    reg.SIMainInfo.Bka842 = res110004.Payinfo.Bka842;
                    reg.SIMainInfo.Bka840 = res110004.Payinfo.Bka840;
                    reg.SIMainInfo.Bka020 = "";
                    reg.ClinicDiagnose = "";
                    reg.SIMainInfo.Aaa027 = "";
                    reg.SSN = patientinfo.Aac001;
                    reg.SIMainInfo.Aab301 = "4404";
                    reg.SIMainInfo.Aae140 = patientinfo.Aae140;
                    reg.SIMainInfo.Aka130 = cancelAka130;
                    reg.SIMainInfo.Bka006 = cancelBka006;
                    patient.RegNo = reg.SIMainInfo.RegNo;

                    if (mgr.InsertOutPatientReg(reg) < 0)
                    {
                        throw new Exception("保存医保费用信息出错！" + mgr.Err);
                    }

                    cancelreg = reg;

                    #endregion

                    patient.PubDigFee = HcareRes.HcareAmount;
                    patient.OwnDigFee = HcareRes.SelfAmount;
                    patient.RegNo = res110004.Payinfo.Aaz218;
                    patient.Ecost = 0;
                    if (order.ISECOST == "1")
                    {
                        patient.Ecost = HcareRes.SelfAmount;
                        patient.OwnDigFee = 0;
                    }
                }
                //减免信息。
                patient.RegDiagCode = "";
                #endregion

                #region 减免费用处理

                decimal hccost = regpay.HCAREAMOUNT;//
                decimal excost = regpay.EXPENSEAMOUNT;//
                decimal pucost = regpay.HCAREAMOUNT + regpay.EXPENSEAMOUNT;//
                decimal secost = regpay.SELFAMOUNT;//
                decimal ecost = regpay.ECOSTAMOUNT;
                if (regpay.TOTALAMOUNT != hccost + excost + secost + ecost)
                {
                    throw new Exception("入参费用不符！");
                }

                if (regpay.TOTALAMOUNT != (patient.RegFee + patient.OwnDigFee + patient.PubDigFee + patient.Ecost) * 100)
                {
                    throw new Exception("费用不符！系统挂号金额：" + ((patient.RegFee + patient.OwnDigFee + patient.PubDigFee + patient.Ecost) * 100) + "，入参费用金额为：" + regpay.TOTALAMOUNT);
                }

                if (pucost != patient.PubDigFee * 100)
                {
                    throw new Exception("费用不符！医保费用不符！系统挂号金额：" + (patient.PubDigFee * 100) + "，入参费用金额为：" + pucost);
                }

                patient.PubDigFee = (hccost + excost) / 100;
                patient.OwnDigFee = regpay.SELFAMOUNT / 100;
                //减免信息。
                //patient.RegNo = "";
                patient.RegDiagCode = "";
                patient.Ecost = regpay.ECOSTAMOUNT / 100;
                #endregion

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

                Models.Views.OutPatient.PayResult res = new Models.Views.OutPatient.PayResult();
                res.HospTradeId = patient.InvoiceStr;
                res.InvoiceId = patient.InvoiceStr;
                res.ReceiptId = patient.InvoiceStr;
                res.VisitAddress = patient.Dept.Name + patient.Room.Name;
                res.VisitNo = patient.ClinicCode;
                res.Proof = patient.SeeNO.ToString();
                res.Remark = "";



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
                platformPayLogic.RollbackTran();
                platformOrderLogic.RollbackTran();
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                result.IsSuccessful = false;
                result.Message = ex.Message;

                if (!string.IsNullOrEmpty(SiTransid))
                {
                    FS.ZDWY.Internet.BP.SI.OutPatient.GetCancelFeeInfo cancelfeeinfo = new SI.OutPatient.GetCancelFeeInfo();
                    string erro = "";
                    if (cancelfeeinfo.CallService(SiTransid, ref erro, cancelBka006) < 0)
                    {
                        result.Message += "收费时提取门诊业务信息！原因：" + cancelfeeinfo.ErrorMsg;
                    }


                    object[] objfee = new object[] { regpay.TransNo, cancelpatient.RegLevel.ID, cancelpatient.Doct.ID };
                    FS.ZDWY.Internet.BP.SI.OutPatient.CancelFee cancelfee = new SI.OutPatient.CancelFee();
                    if (cancelfee.CallService(cancelreg, ref erro, objfee) < 0)
                    {
                        result.Message += "取消费用信息！原因：" + cancelfee.ErrorMsg;
                    }

                    FS.ZDWY.Internet.BP.SI.OutPatient.GetFeeInfo feeinfo = new SI.OutPatient.GetFeeInfo();
                    if (feeinfo.CallService(SiTransid, ref erro, cancelBka006) < 0)
                    {
                        result.Message += "收费时提取门诊业务信息！原因：" + feeinfo.ErrorMsg;
                    }

                    FS.ZDWY.Internet.BP.SI.OutPatient.CancelRegister cancel = new SI.OutPatient.CancelRegister();
                    GDSI.Models.OutParam.OutParamBizh110106 outParam = new GDSI.Models.OutParam.OutParamBizh110106();
                    object[] obj = new object[] { regpay.TransNo, SiTransid };
                    if (cancel.CallService("", ref outParam, obj) <= 0)
                    {
                        result.Message = result.Message + "取消医保挂号失败！原因：" + cancel.ErrorMsg;
                    }
                }

                return result;
            }

        }


        /// <summary>
        /// 订单退费
        /// </summary>
        /// <returns></returns>
        public Models.Views.ComResult<Models.PLATFORM_REGISTER_PAY> RegisterBackPay(string orderId, string psRefOrdNum, string hospitalNum, string hospTradeId, string refundReason, string transno)
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
                objReturn.InputOper.ID = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                objReturn.CancelOper.ID = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
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

                //    if (reg.RegType == Neusoft.HISFC.Models.Base.EnumRegType.Pre)
                //    {
                //        IsTeled = true; //预约号
                //    }
                //    else if (reg.RegType == Neusoft.HISFC.Models.Base.EnumRegType.Reg)
                //    {
                //        if (reg.DoctorInfo.SeeDate > current)
                //        {
                //            IsTeled = true;//预约号
                //        }
                //        else
                //        {
                //            IsReged = true;//现场号
                //        }
                //    }
                //    else
                //    {
                //        IsSped = true;//特诊号
                //    }

                //    rtn = schMgr.Reduce(reg.DoctorInfo.Templet.ID, IsReged, false, IsTeled, IsSped);
                //    if (rtn == -1)
                //    {
                //        throw new Exception(schMgr.Err);
                //    }

                //    if (rtn == 0)
                //    {
                //        throw new Exception("已无排班信息, 无法恢复限额");
                //    }
                //}

                if (order.REGTYPE == "0")
                {
                    string updateSchema = @"update fin_opr_schema s --医师出诊表
                                           set s.tel_reging = s.tel_reging + {1}, --预约已约
                                               s.tel_reged  = s.tel_reged + {1}-- 预约已挂
                                         where s.id = '{0}'";
                    updateSchema = string.Format(updateSchema, order.SCHEDULEID, "-1");
                    int res = mgr.ExecNoQuery(updateSchema);
                    if (res <= 0)
                    {
                        throw new Exception("解锁号源失败！");
                    }
                }
                else
                {
                    throw new Exception("当日挂号无法线上退费！");
                }
                #endregion

                Neusoft.HISFC.Models.Registration.Register cancelreg = reg.Clone();
                cancelreg.SIMainInfo.RegNo = "";
                if (mgr.GetRegSIPersonInfo(booking.REG_ID, ref cancelreg) > 0)
                {
                    if (!string.IsNullOrEmpty(cancelreg.SIMainInfo.RegNo))
                    {
                        if (mgr.UpdateSIPersonInfo(cancelreg.ID) <= 0)
                        {
                            throw new Exception("更新医保信息失败！" + mgr.Err + cancelreg.ID);
                        }

                        #region 医保退号

                        FS.ZDWY.Internet.BP.SI.OutPatient.GetCancelFeeInfo cancelfeeinfo = new SI.OutPatient.GetCancelFeeInfo();
                        string erro = "";
                        if (cancelfeeinfo.CallService(cancelreg.SIMainInfo.RegNo, ref erro, cancelreg.SIMainInfo.Bka006) < 0)
                        {
                            result.Message += "收费时提取门诊业务信息！原因：" + cancelfeeinfo.ErrorMsg;
                        }


                        object[] objfee = new object[] { "", booking.REGLEVL_CODE, booking.DOCT_CODE };
                        FS.ZDWY.Internet.BP.SI.OutPatient.CancelFee cancelfee = new SI.OutPatient.CancelFee();
                        if (cancelfee.CallService(cancelreg, ref erro, objfee) < 0)
                        {
                            result.Message += "取消费用信息！原因：" + cancelfee.ErrorMsg;
                        }

                        FS.ZDWY.Internet.BP.SI.OutPatient.GetFeeInfo feeinfo = new SI.OutPatient.GetFeeInfo();

                        if (feeinfo.CallService(cancelreg.SIMainInfo.RegNo, ref erro, cancelreg.SIMainInfo.Bka006) < 0)
                        {
                            throw new Exception("收费时提取门诊业务信息！原因：" + feeinfo.ErrorMsg);
                        }

                        //if (!string.IsNullOrWhiteSpace(transno))
                        //{
                        FS.ZDWY.Internet.BP.SI.OutPatient.CancelRegister cancel = new SI.OutPatient.CancelRegister();
                        GDSI.Models.OutParam.OutParamBizh110106 outParam = new GDSI.Models.OutParam.OutParamBizh110106();
                        object[] obj = new object[] { "", cancelreg.SIMainInfo.RegNo };
                        if (cancel.CallService("", ref outParam, obj) <= 0)
                        {
                            throw new Exception("取消医保挂号失败！原因：" + cancel.ErrorMsg);
                        }
                        //}
                        //else
                        //{
                        //throw new Exception("医保挂号患者，请传入tranno！");
                        //}


                        #endregion
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
        public Models.Views.ComResult<Models.Views.OutPatient.HcareResult> Hcare(string orderId, string hospitalNum, string transno, string patientId)
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

                string str = ((int)((patient.RegFee + patient.OwnDigFee) * 100)).ToString();
                if (str != order.REGFEE)
                {
                    throw new Exception("获取费用不符合！");
                }

                #region 在职人员判断
                #endregion

                if (!string.IsNullOrWhiteSpace(transno))
                {
                    #region 医保处理

                    string cancelBka006 = "110";
                    string cancelAka130 = "11";

                    string Aaz267 = "1";
                    //string Bka006 = "110";
                    string DiagCode = "";


                    FS.ZDWY.Internet.BP.SI.OutPatient.GetPatientInfo GetPtient = new SI.OutPatient.GetPatientInfo();
                    GDSI.Models.OutParam.OutParamBizh110001 res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                    GDSI.Models.Personinfo patientinfo = null;

                    #region menman
                    bool isspecial = false;
                    string[] GetinfoParams = new string[] { transno, "13", "131" };
                    if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                    {
                        isspecial = false;
                    }
                    else
                    {
                        if (res110001.Personinfos.Count == 1)
                        {
                            if (res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                            {
                                isspecial = true;
                            }
                            else
                            {
                                isspecial = false;
                            }
                        }
                        else if (res110001.Personinfos.Count > 1)
                        {
                            GDSI.Models.Personinfo temp = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                            patient.SSN = temp.Aac001;
                            if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                            {
                                isspecial = false;
                            }
                            else
                            {
                                if (res110001.Personinfos != null && res110001.Personinfos.Count > 0 && res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                                {
                                    isspecial = true;
                                }
                                else
                                {
                                    isspecial = false;
                                }
                            }
                        }
                        else
                        {
                            //if (res110001.Personinfos != null && res110001.Personinfos.Count > 0 && res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                            //{
                            //    isspecial = true;
                            //}
                            isspecial = false;
                        }
                    }
                    if (isspecial)
                    {
                        //if (res110001.Personinfos.Count != 1)
                        //{
                        //    throw new Exception("【医保错误】【医保减免接口】获取医保患者信息失败！原因：存在多个医保信息！");
                        //}
                        //if (res110001.Spinfos.Count != 1)
                        //{
                        //    throw new Exception("【医保错误】【医保减免接口】获取医保患者信息失败！原因：存在多个病种信息！");
                        //}
                        //GDSI.Models.Personinfo 
                        patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                        GDSI.Models.Spinfo cli = null;
                        if (res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                        {
                            cli = res110001.Spinfos[0] as GDSI.Models.Spinfo;
                            Aaz267 = cli.Aaz267;
                            //Bka006 = cli.Bka006;
                            DiagCode = cli.Bka026;
                            cancelBka006 = cli.Bka006;
                            cancelAka130 = "13";
                        }
                    }
                    #endregion
                    else
                    {
                        //FS.ZDWY.Internet.BP.SI.OutPatient.GetPatientInfo GetPtient = new SI.OutPatient.GetPatientInfo();
                        res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                        GetinfoParams = new string[] { transno, "11", "110" };
                        if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                        {
                            throw new Exception("【医保错误】【医保减免接口-2】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                        }

                        if (res110001.Personinfos.Count > 1)
                        {
                            GDSI.Models.Personinfo temp = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                            patient.SSN = temp.Aac001;
                            if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                            {
                                throw new Exception("【医保错误】【医保减免接口-2】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                            }
                        }
                        //if (res110001.Personinfos.Count != 1)
                        //{
                        //    throw new Exception("【医保错误】【医保减免接口】获取患者医保信息过多或无！请在收费窗口挂号！");
                        //}

                        //GDSI.Models.Personinfo 
                        patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                        Aaz267 = "1";
                        //Bka006 = "110";
                        DiagCode = "";
                        cancelBka006 = "110";
                        cancelAka130 = "11";
                    }
                    //Aka130 = "11";
                    //Bka006 = "110";
                    //appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042}
                    FS.ZDWY.Internet.BP.SI.OutPatient.Register Hcareser = new SI.OutPatient.Register();
                    //GDSI.Models.Personinfo patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                    //GDSI.Models.Clinicapplyinfo cli = res110001.Clinicapplyinfos[0] as GDSI.Models.Clinicapplyinfo;
                    GDSI.Models.OutParam.OutParamBizh110104 res110004 = new GDSI.Models.OutParam.OutParamBizh110104();
                    patient.OperTime = mgr.GetDateTimeFromSysDateTime();

                    //string[] appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, regpay.TransNo, "11", "110", "1", cli.Aaz267, "" };



                    string[] appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, transno, cancelAka130, cancelBka006, "0", Aaz267, "" };
                    //if (Hcareser.CallService(patient, ref res110004, appendParams) <= 0)
                    //{
                    //    throw new Exception("【医保错误】【医保减免接口】获取医保患者医保减免信息失败！原因：" + Hcareser.ErrorMsg);
                    //}

                    int resint = Hcareser.CallService(patient, ref res110004, appendParams);
                    if (resint == -2 || Hcareser.ErrorMsg.Contains("重新调用功能号bizh110001"))
                    {
                        res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                        GetinfoParams = new string[] { transno, cancelAka130, cancelBka006 };
                        if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                        {
                            throw new Exception("【医保错误】【医保减免接口-3】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                        }

                        if (res110001.Personinfos.Count > 1)
                        {
                            GDSI.Models.Personinfo temp = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                            patient.SSN = temp.Aac001;
                            if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                            {
                                throw new Exception("【医保错误】【医保2次减免接口】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                            }
                        }

                        patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                        appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, transno, cancelAka130, cancelBka006, "0", Aaz267, "" };
                        if (Hcareser.CallService(patient, ref res110004, appendParams) <= 0)
                        {
                            throw new Exception("【医保错误】【医保减免接口-4】获取医保患者医保减免信息失败！原因：" + Hcareser.ErrorMsg);
                        }
                    }
                    else if (resint < 0)
                    {
                        throw new Exception("【医保错误】【医保减免接口-5】获取医保患者医保减免信息失败！原因：" + Hcareser.ErrorMsg);
                    }

                    #endregion

                    Models.Views.OutPatient.HcareResult res = new Models.Views.OutPatient.HcareResult();
                    res.HcareAmount = (res110004.Payinfo.Bka832 + res110004.Payinfo.Akb066) * 100;
                    res.ExpenseAmount = 0;
                    res.SelfAmount = res110004.Payinfo.Bka831 * 100;
                    res.TotalAmount = (patient.OwnDigFee + patient.RegFee) * 100;
                    res.EcostAmount = 0;
                    if (order.ISECOST == "1")
                    {
                        res.SelfAmount = 0;
                        res.EcostAmount = res110004.Payinfo.Bka831 * 100;
                    }

                    if (order.REGFEE != ((int)res.TotalAmount).ToString())
                    {
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
                    if (order.ISECOST == "1")
                    {
                        res.SelfAmount = 0;
                        res.EcostAmount = (patient.OwnDigFee + patient.RegFee) * 100;
                    }

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

        private packagService.ZDWY.MzPackage mzPk = new packagService.ZDWY.MzPackage();

        /// <summary>
        /// 处方缴费
        /// </summary>
        /// <param name="billpay"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        public Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY> BillPay(Models.PLATFORM_BALANCE_PAY billpay, Models.OperInfo oper)
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

            //医保取消
            string SiTransid = "";
            Neusoft.HISFC.Models.Registration.Register cancelreg = null;
            string cancelBka006 = "";

            ArrayList RollBackFee = new ArrayList();
            try
            {

                oper = FS.ZDWY.Internet.BP.Common.Function.DefaultOper;
                string dt22 = outpatientManager.GetSysDateTime();
                if (string.IsNullOrEmpty(billpay.VISITNO))
                {
                    throw new Exception("VISITNO为空！");
                }
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.PAYAMT) <= 0)
                {
                    throw new Exception("PayAmt 支付金额不能小于等于0！");
                }


                if (string.IsNullOrEmpty(billpay.PAYMODE))
                {
                    throw new Exception("PayType 支付方式不能为空！");
                }

                string clinicCode = billpay.VISITNO;  //就诊号

                ArrayList comFeeItemLists = new ArrayList();  //费用集合
                Neusoft.HISFC.Models.Registration.Register reg = null; //患者基本信息

                decimal totFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.TOTALAMOUNT) / 100;       //总费用
                decimal ownFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.SELFAMOUNT) / 100;   //个人支付费用
                decimal pubFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.EXPENSEAMOUNT) / 100;   //报销费用
                decimal hcareFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(billpay.HCAREAMOUNT) / 100;   //报销费用

                if (totFee != ownFee + pubFee + hcareFee)
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
                reg.Pact.Name = "自费";


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
                ArrayList al = outpatientManager.QueryChargedFeeItemListsByClinicNO(clinicCode);

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
                    throw new Exception(errMsg);
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
                    throw new Exception(errMsg);
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

                #region 医保处理
                if (!string.IsNullOrWhiteSpace(billpay.TRANNO))
                {
                    reg.Pact.PayKind.ID = "02";
                    reg.Pact.ID = "252";
                    reg.Pact.Name = "广东医保门诊慢性病（门诊）";

                    #region 医保处理
                    FS.ZDWY.Internet.Models.Views.OutPatient.ComPatient patient = new Models.Views.OutPatient.ComPatient();
                    patient.OperTime = outpatientManager.GetDateTimeFromSysDateTime();
                    patient.SSN = reg.IDCard;
                    patient.Name = reg.Name;
                    patient.IDCard = reg.IDCard;
                    FS.ZDWY.Internet.BP.SI.OutPatient.GetPatientInfo GetPtient = new SI.OutPatient.GetPatientInfo();
                    GDSI.Models.OutParam.OutParamBizh110001 res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                    string[] GetinfoParams = new string[] { billpay.TRANNO, "13", "131" };
                    if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                    {
                        throw new Exception("【医保错误】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                    }
                    if (res110001.Personinfos.Count != 1)
                    {
                        throw new Exception("【医保错误】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                    }

                    //Aka130 = "11";
                    //Bka006 = "110";
                    //appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042}
                    FS.ZDWY.Internet.BP.SI.OutPatient.Balance Hcareser = new SI.OutPatient.Balance();
                    GDSI.Models.Personinfo patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                    GDSI.Models.Spinfo cli = null;
                    string Aaz267 = "1";
                    string Bka006 = "131";
                    string DiagCode = "";
                    if (res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                    {
                        cli = res110001.Spinfos[0] as GDSI.Models.Spinfo;
                        Aaz267 = cli.Aaz267;
                        Bka006 = cli.Bka006;
                        DiagCode = cli.Bka026;
                        cancelBka006 = cli.Bka006;
                    }
                    GDSI.Models.OutParam.OutParamBizh110104 res110004 = new GDSI.Models.OutParam.OutParamBizh110104();
                    //appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042,clinicCode,Bka026}
                    string[] appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, billpay.TRANNO, "13", Bka006, "1", Aaz267, "", reg.ID, DiagCode };
                    if (Hcareser.CallService(comFeeItemLists, ref res110004, appendParams) <= 0)
                    {
                        throw new Exception("【医保错误】获取医保患者医保减免信息失败！原因：" + Hcareser.ErrorMsg);
                    }

                    #endregion

                    SiTransid = res110004.Payinfo.Aaz218;
                    cancelreg = reg;
                    RollBackFee = (ArrayList)comFeeItemLists.Clone();

                    #region 插入数据库
                    Neusoft.HISFC.BizLogic.Registration.Register regMgr = new Neusoft.HISFC.BizLogic.Registration.Register();
                    //Neusoft.HISFC.Models.Registration.Register sireg = regMgr.GetByClinic
                    reg.SIMainInfo.RegNo = res110004.Payinfo.Aaz218;
                    reg.InvoiceNO = invoiceNO;
                    //reg.PID.CardNO = patient.CardNo;
                    reg.SSN = patientinfo.Aac001;
                    //reg.Name = patient.Name;
                    reg.IDCard = patient.IDCard;
                    reg.ClinicDiagnose = "";
                    //reg.Pact.PayKind.ID = "M";
                    //reg.Pact.ID = "251";
                    //reg.Pact.Name = "01";
                    reg.SIMainInfo.TotCost = res110004.Payinfo.Akc264;
                    reg.SIMainInfo.PubCost = res110004.Payinfo.Bka832;
                    reg.SIMainInfo.OwnCost = res110004.Payinfo.Bka831;
                    reg.SIMainInfo.PayCost = 0;
                    //reg.Sex.ID = patient.SexCode;
                    //reg.DoctorInfo.Templet.Dept.ID = patient.Dept.ID;
                    //reg.PVisit.InTime = patient.RegDate;
                    //reg.SIMainInfo.BalanceDate = patient.RegDate;

                    reg.SIMainInfo.Bka825 = res110004.Payinfo.Bka825;
                    reg.SIMainInfo.Bka826 = res110004.Payinfo.Bka826;
                    reg.SIMainInfo.Aka151 = res110004.Payinfo.Aka151;
                    reg.SIMainInfo.Bka838 = res110004.Payinfo.Bka838;
                    reg.SIMainInfo.Akb067 = res110004.Payinfo.Akb067;
                    reg.SIMainInfo.Akb066 = res110004.Payinfo.Akb066;
                    reg.SIMainInfo.Bka821 = res110004.Payinfo.Bka821;
                    reg.SIMainInfo.Bka839 = res110004.Payinfo.Bka839;
                    reg.SIMainInfo.Ake039 = res110004.Payinfo.Ake039;
                    reg.SIMainInfo.Ake035 = res110004.Payinfo.Ake035;
                    reg.SIMainInfo.Ake026 = res110004.Payinfo.Ake026;
                    reg.SIMainInfo.Ake029 = res110004.Payinfo.Ake029;
                    reg.SIMainInfo.Bka841 = res110004.Payinfo.Bka841;
                    reg.SIMainInfo.Bka842 = res110004.Payinfo.Bka842;
                    reg.SIMainInfo.Bka840 = res110004.Payinfo.Bka840;
                    reg.SIMainInfo.Bka020 = "";
                    reg.ClinicDiagnose = "";
                    reg.SIMainInfo.Aaa027 = "";
                    reg.SSN = patientinfo.Aac001;
                    reg.SIMainInfo.Aab301 = "11";
                    reg.SIMainInfo.Aae140 = patientinfo.Aae140;
                    reg.SIMainInfo.Aka130 = "13";
                    reg.SIMainInfo.Bka006 = Bka006;

                    if (mgr.InsertOutPatientBalance(reg) < 0)
                    {
                        throw new Exception("保存医保费用信息出错！" + mgr.Err);
                    }
                    #endregion
                }
                else
                {
                    reg.SIMainInfo.PubCost = 0;
                    reg.SIMainInfo.PayCost = 0;
                }

                #endregion

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

                if (ownCost != ownFee)
                {
                    throw new Exception("医保预计算金额与实际结算金额不符！");
                }

                ////判断一下传递进来的费用 与 待遇算法的费用是否相等
                if (totFee != totCost || totFee != (pubFee + ownFee))// || pubCost != tcfee || rebateRate != yhfee)
                {
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
                    throw new Exception(errMsg);
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

                //获取支付方式
                ArrayList balancePays = this.QueryBalancePays((ownCost - rebateRate), billpay, rebateRate, pubCost);
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
                    throw new Exception("缴费失败!" + this.feeIntegrate.Err);
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
set invo.oper_code='00A105'
where invo.clinic_code='{0}'
and invo.invoice_no='{1}'
and invo.trans_type='1'";
                invoicesql = string.Format(invoicesql, reg.ID, invoiceNO);
                if (mgr.ExecNoQuery(invoicesql) <= 0)
                {
                    throw new Exception("更新发票信息失败！");
                }

                string paysql = @"update fin_opb_paymode pa
set pa.oper_code='00A105'
where  pa.invoice_no='{0}'
and pa.trans_type='1'";
                paysql = string.Format(paysql, invoiceNO);
                if (mgr.ExecNoQuery(paysql) <= 0)
                {
                    throw new Exception("更新发票信息失败！");
                }

                string desql = @"update fin_opb_invoicedetail de
set de.oper_code='00A105'
where de.invoice_no='{0}'
and de.trans_type='1'";
                desql = string.Format(desql, invoiceNO);
                if (mgr.ExecNoQuery(desql) <= 0)
                {
                    throw new Exception("更新发票信息失败！");
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

                #region 医保退费

                if (!string.IsNullOrEmpty(SiTransid))
                {
                    FS.ZDWY.Internet.BP.SI.OutPatient.GetCancelFeeInfo cancelfeeinfo = new SI.OutPatient.GetCancelFeeInfo();
                    string erro = "";
                    if (cancelfeeinfo.CallService(SiTransid, ref erro, cancelBka006) < 0)
                    {
                        result.Message += "收费时提取门诊业务信息！原因：" + cancelfeeinfo.ErrorMsg;
                    }

                    object[] objfee = new object[] { billpay.TRANNO, "", "", RollBackFee };
                    FS.ZDWY.Internet.BP.SI.OutPatient.CancelFee cancelfee = new SI.OutPatient.CancelFee();
                    if (cancelfee.CallService(cancelreg, ref erro, objfee) < 0)
                    {
                        result.Message += "取消费用信息！原因：" + cancelfee.ErrorMsg;
                    }

                    FS.ZDWY.Internet.BP.SI.OutPatient.GetFeeInfo feeinfo = new SI.OutPatient.GetFeeInfo();
                    if (feeinfo.CallService(SiTransid, ref erro, cancelBka006) < 0)
                    {
                        result.Message += "收费时提取门诊业务信息！原因：" + feeinfo.ErrorMsg;
                    }

                    FS.ZDWY.Internet.BP.SI.OutPatient.CancelRegister cancel = new SI.OutPatient.CancelRegister();
                    GDSI.Models.OutParam.OutParamBizh110106 outParam = new GDSI.Models.OutParam.OutParamBizh110106();
                    object[] obj = new object[] { billpay.TRANNO, SiTransid };
                    if (cancel.CallService(SiTransid, ref outParam, obj) <= 0)
                    {
                        result.Message = result.Message + "取消医保挂号失败！原因：" + cancel.ErrorMsg;
                    }
                }

                #endregion

                return result;
            }
        }

        /// <summary>
        /// 获得收费信息
        /// </summary>
        /// <param name="al"></param>
        /// <param name="reg"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public ArrayList GetFeeItemList(ArrayList al, Neusoft.HISFC.Models.Registration.Register reg, ref string errMsg)
        {
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

                        if (undrugItem != null && undrugItem.UnitFlag == "1")
                        {
                            ArrayList alDetail = this.ConvertGroupToDetail(f, reg, ref errMsg);
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
            return feeItemLists;

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
            if (this.outpatientManager.QueryItemList(deptCode, Neusoft.HISFC.Models.Base.ItemKind.Undrug, ref dsItem) == -1)
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
            decimal priceGroup = this.feeIntegrate.GetPrice(priceForm, age, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup);


            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
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
                        if ((new packagService.ZDWY.MzPackage().QuitFee(feeList, ref  msg) == -1))
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
                string FRONTPROVIDERID)
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
                ArrayList al = outpatientManager.QueryChargedFeeItemListsByClinicNO(clinicCode);

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

                if (!string.IsNullOrWhiteSpace(TRANNO))
                {
                    #region 医保处理
                    FS.ZDWY.Internet.Models.Views.OutPatient.ComPatient patient = new Models.Views.OutPatient.ComPatient();
                    patient.OperTime = outpatientManager.GetDateTimeFromSysDateTime();
                    patient.SSN = reg.IDCard;
                    patient.Name = reg.Name;
                    patient.IDCard = reg.IDCard;
                    FS.ZDWY.Internet.BP.SI.OutPatient.GetPatientInfo GetPtient = new SI.OutPatient.GetPatientInfo();
                    GDSI.Models.OutParam.OutParamBizh110001 res110001 = new GDSI.Models.OutParam.OutParamBizh110001();
                    string[] GetinfoParams = new string[] { TRANNO, "13", "131" };
                    if (GetPtient.CallService(patient, ref res110001, GetinfoParams) <= 0)
                    {
                        throw new Exception("【医保错误】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                    }
                    if (res110001.Personinfos.Count != 1)
                    {
                        throw new Exception("【医保错误】获取医保患者信息失败！原因：" + GetPtient.ErrorMsg);
                    }

                    //Aka130 = "11";
                    //Bka006 = "110";
                    FS.ZDWY.Internet.BP.SI.OutPatient.Balance Hcareser = new SI.OutPatient.Balance();
                    GDSI.Models.Personinfo patientinfo = res110001.Personinfos[0] as GDSI.Models.Personinfo;
                    GDSI.Models.Spinfo cli = null;
                    string Aaz267 = "1";
                    string Bka006 = "131";
                    string DiagCode = "";
                    if (res110001.Spinfos != null && res110001.Spinfos.Count > 0)
                    {
                        cli = res110001.Spinfos[0] as GDSI.Models.Spinfo;
                        Aaz267 = cli.Aaz267;
                        Bka006 = cli.Bka006;
                        DiagCode = cli.Bka026;
                    }
                    GDSI.Models.OutParam.OutParamBizh110104 res110004 = new GDSI.Models.OutParam.OutParamBizh110104();
                    //appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042,clinicCode,Bka026}
                    string[] appendParams = new string[] { patientinfo.Aaa027, patientinfo.Aac001, TRANNO, "13", Bka006, "0", Aaz267, "", reg.ID, DiagCode };
                    if (Hcareser.CallService(comFeeItemLists, ref res110004, appendParams) <= 0)
                    {
                        throw new Exception("【医保错误】获取医保患者医保减免信息失败！原因：" + Hcareser.ErrorMsg);
                    }

                    #endregion

                    Models.Views.OutPatient.HcareResult res = new Models.Views.OutPatient.HcareResult();
                    res.HcareAmount = 0;
                    res.ExpenseAmount = (res110004.Payinfo.Bka832 + res110004.Payinfo.Akb066) * 100;
                    res.SelfAmount = res110004.Payinfo.Bka831 * 100;
                    res.TotalAmount = (patient.OwnDigFee + patient.RegFee) * 100;

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
                result.IsSuccessful = false;
                result.Message = e.Message.ToString();
                return result;
                //}
            }

        }
        #endregion

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
                decimal fee = mgr.QuitRegFee(obj.HospTradeId);
                if (!obj.SELFAMOUNT.Equals(fee))
                {
                    obj.RefundReason = "患者退款非微信方式！";
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
                    regpaylog.Update(obj);
                    continue;
                }
                if (order == null || string.IsNullOrEmpty(order.ORDERID))
                {
                    obj.RefundReason = "orderid不存在！";
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

                continue;
            }
            return 1;
        }

        #endregion

        #region 门诊开单
        public Models.Views.ComResult<Models.Views.OutPatient.AddOrderResult> AddNewOrder(string parSequenceNo, string parCardNo, string parDoctcode, string parDeptcode,
           string parItemcode, string parUnitPrice, string parQty, string parOwnCost,
           string parExecdeptcode, string parExecdeptname, ref string parClinicCode, ref string parAppCode, ref string parErrMsg)
        {
            parClinicCode = "01";
            parAppCode = "9";
            parErrMsg = "ok";
            Models.Views.ComResult<Models.Views.OutPatient.AddOrderResult> result = new Models.Views.ComResult<Models.Views.OutPatient.AddOrderResult>();
            result.IsSuccessful = true;
            result.Message = string.Empty;

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            //设置事务
            outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            try
            {
                DateTime dtNow = outpatientManager.GetDateTimeFromSysDateTime();
                string sql = @"PRC_WX_INSERTFEEDETAIL,PAR_SEQUENCE_NO,22,1,{0},PAR_CARD_NO,22,1,{1},PAR_DOCTCODE,22,1,{2},PAR_DEPTCODE,22,1,{3},PAR_ITEMCODE,22,1,{4},PAR_UNIT_PRICE,13,1,{5},PAR_QTY,13,1,{6},PAR_OWN_COST,13,1,{7},PAR_EXECDEPTCODE,22,1,{8},PAR_EXECDEPTNAME,22,1,{9},PAR_CLINIC_CODE,22,2,{10},PAR_APPCODE,28,2,{11},PAR_ERRMSG,22,2,{12}";
                sql = string.Format(sql, parSequenceNo, parCardNo, parDoctcode, parDeptcode,
             parItemcode, parUnitPrice, parQty, parOwnCost,
             parExecdeptcode, parExecdeptname, parClinicCode, parAppCode, parErrMsg);
                string returnString = string.Empty;
                if (outpatientManager.ExecEvent(sql, ref returnString) == -1)
                {
                    result.IsSuccessful = false;
                    result.Message = "执行存储过程PRC_WX_INSERTFEEDETAIL出错，错误信息：" + outpatientManager.Err;
                }
                else
                {
                    string[] temp;//临时存储过程返回字符串
                    temp = returnString.Split(',');
                    result.ReturnData = new Models.Views.OutPatient.AddOrderResult();
                    result.ReturnData.clinicCode = temp[0];
                    result.IsSuccessful = temp[1] == "0";
                    result.Message = temp[2];
                }
            }
            catch (Exception ex)
            {

                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                result.IsSuccessful = false;
                result.Message = ex.Message;
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 自助开单接口
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public Models.Views.ComResult<SelfServiceAddOrderResponseModel> SelfServiceAddOrder(SelfServiceAddOrderRequsetModel reqModel)
        {
            Models.Views.ComResult<SelfServiceAddOrderResponseModel> result = new Models.Views.ComResult<SelfServiceAddOrderResponseModel>();
            string errMsg = string.Empty;
            string clincCode = string.Empty;
            if (!this.CheckSelfServiceAddOrderRequseModelValid(reqModel, ref errMsg))
            {
                return this.ErrSelfServiceAddOrder(errMsg);
            }
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();
            var patientInfo = patientInfoLogic.Get(reqModel.patientCardNo);
            if (patientInfo == null)
            {
                return this.ErrSelfServiceAddOrder("根据就诊卡号【" + reqModel.patientCardNo + "】未找到相关建档信息！");
            }
            if (patientInfo.NAME != reqModel.patientName)
            {
                return this.ErrSelfServiceAddOrder("根据就诊卡号【" + reqModel.patientCardNo + "】查询的建档信息名称与传入名称不一致！");
            }
            RegisterInfoLogic rigInfoBll = new RegisterInfoLogic();
            MetCasDiagnoseLogic diagBll = new MetCasDiagnoseLogic();
            FinOpbFeeDetailLogic feeDetailBll = new FinOpbFeeDetailLogic();
            FinComUndrugInfoLogic undrugBll = new FinComUndrugInfoLogic();
            DepartmentEntityLogic departMentBll = new DepartmentEntityLogic();
            diagBll.BeginTran();
            rigInfoBll.BeginTran();
            feeDetailBll.BeginTran();
            var sysdate = rigInfoBll.GetDateTime();
            //设置事务
            outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            try
            {
                //1.先查询如果今天患者已经挂过核酸科室的号，就直接选取那条挂号，否则新增一条挂号记录
                //0员挂号 就没必要去给费用表添加数据了
                clincCode = rigInfoBll.GetHSTodayClincCodeForCardNo(reqModel.patientCardNo);
                if (string.IsNullOrWhiteSpace(clincCode))
                {
                    clincCode = feeDetailBll.GetClincCode();
                    string sqlInsertReg = string.Empty;
                    FIN_OPR_REGISTER registerInfoModel = new FIN_OPR_REGISTER();
                    #region 挂号实体赋值
                    registerInfoModel.CLINIC_CODE = clincCode;
                    registerInfoModel.CARD_NO = patientInfo.CARD_NO;
                    registerInfoModel.REG_DATE = sysdate;
                    #region 午别
                    string noonCode = "";
                    if (sysdate.Hour < 12 && sysdate.Hour > 6)
                    {
                        noonCode = "1";//上午
                    }
                    else if (sysdate.Hour > 12 && sysdate.Hour < 18)
                    {
                        noonCode = "2";//下午
                    }
                    else
                    {
                        noonCode = "3";//晚上
                    }
                    #endregion
                    registerInfoModel.NOON_CODE = noonCode;
                    registerInfoModel.NAME = patientInfo.NAME;
                    registerInfoModel.IDENNO = patientInfo.IDENNO;
                    registerInfoModel.CARD_TYPE = patientInfo.IDCARDTYPE;
                    registerInfoModel.SEX_CODE = patientInfo.SEX_CODE;
                    registerInfoModel.BIRTHDAY = patientInfo.BIRTHDAY;
                    registerInfoModel.PAYKIND_CODE = "01";
                    registerInfoModel.PAYKIND_NAME = "自费";
                    registerInfoModel.PACT_CODE = "1";
                    registerInfoModel.PACT_NAME = "现金";
                    registerInfoModel.MCARD_NO = "";
                    registerInfoModel.REGLEVL_CODE = "16";
                    registerInfoModel.REGLEVL_NAME = "珠澳特定人员";
                    registerInfoModel.DEPT_CODE = reqModel.itemList[0].deptCode;
                    registerInfoModel.DEPT_NAME = departMentBll.GetSingle(w => w.DEPT_CODE == reqModel.itemList[0].deptCode).DEPT_NAME;
                    registerInfoModel.SEENO = -1;//暂未确定
                    registerInfoModel.DOCT_CODE = reqModel.itemList[0].doctCode;
                    registerInfoModel.DOCT_NAME = reqModel.itemList[0].doctName;
                    registerInfoModel.SEE_DATE = sysdate;
                    registerInfoModel.YNREGCHRG = "1";//挂号收费标志 暂时为0 未收费
                    registerInfoModel.YNBOOK = "0";
                    registerInfoModel.YNFR = "1";
                    registerInfoModel.REG_FEE = 0;
                    registerInfoModel.CHCK_FEE = 0;
                    registerInfoModel.DIAG_FEE = 0;
                    registerInfoModel.OTH_FEE = 0;
                    registerInfoModel.OWN_COST = 0;
                    registerInfoModel.PUB_COST = 0;
                    registerInfoModel.PAY_COST = 0;
                    registerInfoModel.VALID_FLAG = "1";
                    registerInfoModel.OPER_CODE = "00A105";
                    registerInfoModel.YNSEE = "1";
                    registerInfoModel.CHECK_FLAG = "0";
                    registerInfoModel.RELA_PHONE = patientInfo.HOME_TEL;
                    registerInfoModel.ADDRESS = patientInfo.HOME_NOW;
                    registerInfoModel.TRANS_TYPE = "1";
                    registerInfoModel.BEGIN_TIME = sysdate;
                    registerInfoModel.END_TIME = sysdate;
                    registerInfoModel.APPEND_FLAG = "0";
                    registerInfoModel.OPER_DATE = sysdate;
                    registerInfoModel.IS_SENDINHOSCASE = "0";
                    registerInfoModel.IS_ENCRYPTNAME = "0";
                    registerInfoModel.ECO_COST = 0;
                    registerInfoModel.IS_EMERGENCY = "1";
                    registerInfoModel.SOURCE_FLAG = reqModel.sourceFlag;
                    #endregion
                    if (!rigInfoBll.Insert(registerInfoModel))
                    {
                        throw new Exception("新增挂号记录失败!");
                    }

                }
                if (!diagBll.IsAny(w => w.INPATIENT_NO == clincCode && w.PERSSON_TYPE == "0" && w.VALID_FLAG == "1"))
                {
                    //2.加入自动诊断  体检（医保结算需要）
                    MET_CAS_DIAGNOSE diagInfo = new MET_CAS_DIAGNOSE();
                    diagInfo.INPATIENT_NO = clincCode;
                    diagInfo.HAPPEN_NO = 1;
                    diagInfo.DIAG_KIND = "10";
                    diagInfo.ICD_CODE = "Z00.001";
                    diagInfo.DIAG_NAME = "体检";
                    diagInfo.DIAG_DATE = sysdate;
                    diagInfo.DOCT_CODE = reqModel.itemList[0].doctCode;
                    diagInfo.DOCT_NAME = reqModel.itemList[0].doctName;
                    diagInfo.DUBDIAG_FLAG = "0";
                    diagInfo.MAIN_FLAG = "1";
                    diagInfo.OPER_CODE = reqModel.itemList[0].doctCode;
                    diagInfo.OPER_DATE = sysdate;
                    diagInfo.OPER_TYPE = "1";
                    diagInfo.VALID_FLAG = "1";
                    diagInfo.PERSSON_TYPE = "0";
                    diagInfo.SOURCE_FLAG = "1";
                    if (!diagBll.Insert(diagInfo))
                    {
                        throw new Exception("诊断添加失败!");
                    }
                }
                //3.插入相关费用项目 医嘱不生成
                foreach (var feeItem in reqModel.itemList)
                {
                    var undrugInfo = undrugBll.GetUnDrugEntityForItemCode(feeItem.itemCode);
                    if (undrugInfo.ITEM_CODE != feeItem.itemCode)
                    {
                        throw new Exception("【" + feeItem.itemName + "】相关费用项目不存在!");
                    }
                    if (Convert.ToDecimal(undrugInfo.UNIT_PRICE) != feeItem.unitPrice)
                    {
                        throw new Exception("【" + feeItem.itemName + "】费用项目价格已发生变动，暂时无法进行操作!");
                    }

                    FIN_OPB_FEEDETAIL feeDetail = new FIN_OPB_FEEDETAIL();
                    if (undrugInfo.UNITFLAG == "1")
                    {
                        feeDetail.PACKAGE_CODE = feeItem.itemCode;
                        feeDetail.PACKAGE_NAME = feeItem.itemName;
                    }
                    feeDetail.RECIPE_NO = feeDetailBll.GetRecipeNo();
                    feeDetail.SEQUENCE_NO = 1;
                    feeDetail.TRANS_TYPE = "1";
                    feeDetail.CLINIC_CODE = clincCode;
                    feeDetail.CARD_NO = patientInfo.CARD_NO;
                    feeDetail.REG_DATE = sysdate;
                    feeDetail.REG_DPCD = feeItem.deptCode;
                    feeDetail.DOCT_CODE = feeItem.doctCode;
                    feeDetail.DOCT_DEPT = feeItem.deptCode;
                    feeDetail.ITEM_CODE = feeItem.itemCode;
                    feeDetail.ITEM_NAME = feeItem.itemName;
                    feeDetail.DRUG_FLAG = "0";
                    feeDetail.SPECS = undrugInfo.SPECS;
                    feeDetail.FEE_CODE = undrugInfo.FEE_CODE;
                    feeDetail.CLASS_CODE = undrugInfo.SYS_CLASS;
                    feeDetail.UNIT_PRICE = Convert.ToDouble(feeItem.unitPrice);
                    feeDetail.QTY = Convert.ToDouble(feeItem.qty);
                    feeDetail.DAYS = 1;
                    feeDetail.FREQUENCY_CODE = "QD";
                    feeDetail.INJECT_NUMBER = 0;
                    feeDetail.EMC_FLAG = "0";
                    feeDetail.DOSE_ONCE = 0;
                    feeDetail.PACK_QTY = 1;
                    feeDetail.PRICE_UNIT = undrugInfo.STOCK_UNIT;
                    feeDetail.PUB_COST = 0;
                    feeDetail.PAY_COST = 0;
                    feeDetail.OWN_COST = Convert.ToDouble(feeItem.ownCost);
                    feeDetail.EXEC_DPCD = feeItem.execDeptCode;
                    feeDetail.EXEC_DPNM = feeItem.execDeptName;
                    feeDetail.CENTER_CODE = "0";
                    feeDetail.MAIN_DRUG = "0";
                    feeDetail.COMB_NO = feeDetailBll.GetCombono();
                    feeDetail.OPER_CODE = feeItem.doctCode;
                    feeDetail.OPER_DATE = sysdate;
                    feeDetail.PAY_FLAG = "0";
                    feeDetail.CANCEL_FLAG = "1";
                    feeDetail.FEE_DATE = Convert.ToDateTime("0001-01-01 00:00:00");
                    feeDetail.CONFIRM_FLAG = "1";
                    feeDetail.CONFIRM_DATE = Convert.ToDateTime("0001-01-01 00:00:00");
                    feeDetail.INVOICE_SEQ = "NULL";
                    feeDetail.NEW_ITEMRATE = 0;
                    feeDetail.OLD_ITEMRATE = 0;
                    feeDetail.EXT_FLAG2 = "1";
                    feeDetail.EXT_FLAG3 = "1";
                    feeDetail.NOBACK_NUM = Convert.ToDouble(feeItem.qty);
                    feeDetail.CONFIRM_NUM = 0;
                    feeDetail.CONFIRM_INJECT = 0;
                    feeDetail.MO_ORDER = feeDetailBll.GetMoOrder();
                    feeDetail.ECO_COST = 0;
                    feeDetail.OVER_COST = 0;
                    feeDetail.EXCESS_COST = 0;
                    feeDetail.DRUG_OWNCOST = 0;
                    feeDetail.COST_SOURCE = "4";
                    feeDetail.SUBJOB_FLAG = "0";
                    feeDetail.ACCOUNT_FLAG = "0";
                    feeDetail.UPDATE_SEQUENCENO = 0;
                    feeDetail.PAYKIND_CODE = "01";
                    feeDetail.PACT_CODE = "1";
                    feeDetail.BELONG_DEPT = "6126";
                    feeDetail.HOS_CODE = "CORE_HIS50";
                    if (!feeDetailBll.Insert(feeDetail))
                    {
                        throw new Exception("插入费用失败!");
                    }
                }
                //事务提交
                rigInfoBll.CommitTran();
                diagBll.CommitTran();
                feeDetailBll.CommitTran();
            }
            catch (Exception ex)
            {
                rigInfoBll.RollbackTran();
                diagBll.RollbackTran();
                feeDetailBll.RollbackTran();
                return this.ErrSelfServiceAddOrder(ex.Message);
            }
            return SuccessSelfServiceAddOrder(clincCode);
        }

        private bool CheckSelfServiceAddOrderRequseModelValid(SelfServiceAddOrderRequsetModel reqModel, ref string errMsg)
        {
            errMsg = "入参校验失败：";
            if (reqModel == null)
            {
                errMsg = errMsg + "入参数据不能为空！";
                return false;
            }
            if (string.IsNullOrWhiteSpace(reqModel.patientCardNo))
            {
                errMsg = errMsg + "就诊卡号不能为空!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(reqModel.patientName))
            {
                errMsg = errMsg + "就诊人名称不能为空!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(reqModel.sourceFlag) || !"1,2".Contains(reqModel.sourceFlag))
            {
                errMsg = errMsg + "来源渠道非法值!";
                return false;
            }
            if (reqModel.itemList == null)
            {
                errMsg = errMsg + "开立项目集合不正确!";
                return false;
            }
            if (reqModel.itemList.Count <= 0)
            {
                errMsg = errMsg + "开立项目集合数量不能小于0!";
                return false;
            }
            foreach (var item in reqModel.itemList)
            {
                if (item.qty <= 0)
                {
                    errMsg = errMsg + "项目【" + item.itemName + "】开立数量不能小于等于0！";
                    return false;
                }
                if (item.unitPrice < 0)
                {
                    errMsg = errMsg + "项目【" + item.itemName + "】开立单价不能小于0！";
                    return false;
                }
                if (item.ownCost < 0)
                {
                    errMsg = errMsg + "项目【" + item.itemName + "】开立金额不能小于0！";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(item.doctCode))
                {
                    errMsg = errMsg + "开单医生编码不能为空！";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(item.doctName))
                {
                    errMsg = errMsg + "开单医生名称不能为空！";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(item.itemCode))
                {
                    errMsg = errMsg + "开立项目编码不能为空！";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(item.itemName))
                {
                    errMsg = errMsg + "开立项目名称不能为空！";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(item.deptCode))
                {
                    errMsg = errMsg + "开方医师所在科室不能为空！";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(item.execDeptCode))
                {
                    errMsg = errMsg + "执行科室编码不能为空！";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(item.execDeptName))
                {
                    errMsg = errMsg + "执行科室名称不能为空！";
                    return false;
                }
            }
            return true;

        }

        private Models.Views.ComResult<SelfServiceAddOrderResponseModel> ErrSelfServiceAddOrder(string errMsg)
        {
            Models.Views.ComResult<SelfServiceAddOrderResponseModel> m = new Models.Views.ComResult<SelfServiceAddOrderResponseModel>();
            m.IsSuccessful = false;
            m.Message = errMsg;
            m.ReturnData = new SelfServiceAddOrderResponseModel();
            return m;
        }
        private Models.Views.ComResult<SelfServiceAddOrderResponseModel> SuccessSelfServiceAddOrder(string clincCode)
        {
            Models.Views.ComResult<SelfServiceAddOrderResponseModel> m = new Models.Views.ComResult<SelfServiceAddOrderResponseModel>();
            m.IsSuccessful = true;
            m.Message = "";
            m.ReturnData = new SelfServiceAddOrderResponseModel();
            m.ReturnData.clinicCode = clincCode;
            return m;
        }

    }
}
