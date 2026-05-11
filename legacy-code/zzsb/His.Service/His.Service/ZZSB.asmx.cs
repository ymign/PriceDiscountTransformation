using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using His.Models.Common;
using His.Models.ZZSB;
using His.Util.Common;

namespace His.Service
{
    /// <summary>
    /// ZZSB 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class ZZSB : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "网络测试")]
        public string GetTestNetworkForSRM(string xml)
        {
            His.Business.ZZSB.TestNetworkSr tnf = new His.Business.ZZSB.TestNetworkSr();
            return tnf.GetOutPatientInfoForZZSB(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "门诊患者信息")]
        public string GetOutpatientInfoForSRM(string xml)
        {
            His.Util.Common.HisLog.WriteLog("门诊患者信息", xml);
            His.Business.ZZSB.Patientopb tnf = new His.Business.ZZSB.Patientopb();
            string rntXml = tnf.GetOutPatientInfoForZZSB(xml);

            His.Util.Common.HisLog.WriteLog("门诊患者信息", rntXml);
            return rntXml;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取本院职工门诊信息")]
        public string GetEmployeeInfoForSRM(string xml)
        {
            His.Business.ZZSB.Patientopb tnf = new His.Business.ZZSB.Patientopb();
            return tnf.GetEmployeeInfoForZZSB(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "根据电子凭证获取人员参保信息")]
        public string GetPersonMedicalInfo(string xml)
        {
            string guid = System.Guid.NewGuid().ToString();
            His.Util.Common.HisLog.WriteLog("根据电子凭证获取人员参保信息", guid + System.Environment.NewLine + xml);
            His.Business.ZZSB.Patientopb tnf = new His.Business.ZZSB.Patientopb();
            string rntXml = tnf.GetPersonMedicalInfoForDZPZ(xml);

            His.Util.Common.HisLog.WriteLog("根据电子凭证获取人员参保信息", guid + System.Environment.NewLine + rntXml);
            return rntXml;
        }
        [WebMethod(EnableSession = true, BufferResponse = true, Description = "门诊共济定点备案")]
        public string PerSonMZGJRecord(string xml)
        {
            string guid = System.Guid.NewGuid().ToString();
            His.Util.Common.HisLog.WriteLog("门诊共济定点备案", guid + System.Environment.NewLine + xml);
            His.Business.ZZSB.Patientopb tnf = new His.Business.ZZSB.Patientopb();
            string rntXml = tnf.PerSonMZGJRecord(xml);

            His.Util.Common.HisLog.WriteLog("门诊共济定点备案", guid + System.Environment.NewLine + rntXml);
            return rntXml;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "挂号科室")]
        public string GetRegisterDeptForSRM(string xml)
        {
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, xml);
            His.Business.ZZSB.RegisterDept tnf = new His.Business.ZZSB.RegisterDept();
            return tnf.GetOutPatientInfoForZZSB(xml);
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取医生排班")]
        public string GetDoctorScheduleForSRM(string xml)
        {
            His.Util.Common.HisLog.WriteLog("获取医生排班===========", xml);
            His.Business.ZZSB.DoctorSchedule tnf = new His.Business.ZZSB.DoctorSchedule();
            string rntXml = tnf.GetOutDoctorScheduleForZZSB(xml);
            His.Util.Common.HisLog.WriteLog("获取医生排班===========", rntXml);
            return rntXml;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取挂号费用")]
        public string GetRegisterFeeForSRM(string xml)
        {
            His.Business.ZZSB.TestNetwork tnf = new His.Business.ZZSB.TestNetwork();
            return tnf.GetOutPatientInfoForZZSB(xml);
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询候诊队列")]
        public string QueryWaitingQueenForSRM(string xml)
        {
            His.Util.Common.HisLog.WriteLog("队列序号", xml);
            His.Business.ZZSB.TestNetworktwo tnf = new His.Business.ZZSB.TestNetworktwo();

            string s = tnf.GetOutPatientInfoForZZSB(xml);
            His.Util.Common.HisLog.WriteLog("队列序号", s);
            return s;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询科室队列情况")]
        public string QueryDeptQueenForSRM(string xml)
        {
            His.Business.ZZSB.TestNetworkthr tnf = new His.Business.ZZSB.TestNetworkthr();
            return tnf.GetOutPatientInfoForZZSB(xml);
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取挂号记录")]
        public string GetRegisterRecordForSRM(string xml)
        {
            His.Business.ZZSB.TestNetworkfor tnf = new His.Business.ZZSB.TestNetworkfor();
            return tnf.GetOutPatientInfoForZZSB(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "挂号锁号")]
        public string LockRegisterForSRM(string xml)
        {
            His.Util.Common.HisLog.WriteLog("挂号锁号===========", xml);
            His.Business.ZZSB.OutPatientReg tnf = new His.Business.ZZSB.OutPatientReg();
            string rntXml = tnf.LockRegisterForSRM(xml);
            His.Util.Common.HisLog.WriteLog("挂号锁号===========", rntXml);
            return rntXml;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取医保挂号减免")]
        public string GetRegMedicalFee(string xml)
        {
            His.Util.Common.HisLog.WriteLog("获取医保挂号减免", xml);
            His.Business.ZZSB.OutPatientReg tnf = new His.Business.ZZSB.OutPatientReg();
            string rntXml = tnf.GetRegMedicalFee(xml);
            His.Util.Common.HisLog.WriteLog("获取医保挂号减免", rntXml);
            return rntXml;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "取消医保挂号减免")]
        public string CancelRegMedicalFee(string xml)
        {
            His.Util.Common.HisLog.WriteLog("取消医保挂号减免入参", xml);
            His.Business.ZZSB.OutPatientReg tnf = new His.Business.ZZSB.OutPatientReg();
            string rntXml = tnf.CancelRegMedicalFee(xml);
            His.Util.Common.HisLog.WriteLog("取消医保挂号减免出参", rntXml);
            return rntXml;
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "提交挂号")]
        public string SubmitTheRegisterForSRM(string xml)
        {
            His.Util.Common.HisLog.WriteLog("提交挂号===========", xml);
            His.Business.ZZSB.OutPatientReg tnf = new His.Business.ZZSB.OutPatientReg();
            string rntXml = tnf.SubmitTheRegisterForSRM(xml);
            His.Util.Common.HisLog.WriteLog("提交挂号===========", rntXml);
            return rntXml;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "挂号解锁")]
        public string UnlockRegisterForSRM(string xml)
        {
            His.Util.Common.HisLog.WriteLog("挂号解锁===========", xml);
            His.Business.ZZSB.OutPatientReg tnf = new His.Business.ZZSB.OutPatientReg();
            string rntXml = tnf.UnlockRegisterForSRM(xml);
            His.Util.Common.HisLog.WriteLog("挂号解锁===========", rntXml);
            return rntXml;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询就诊记录")]
        public string QueryVisitRecordForSRM(string xml)
        {
            His.Business.ZZSB.QueryVisitRecordForSRM tnf = new His.Business.ZZSB.QueryVisitRecordForSRM();
            return tnf.GetQueryVisitRecordForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取处方及收费项明细")]
        public string GetPrescriptionAndChargeDetailsForSRM(string xml)
        {
            His.Business.ZZSB.GetPrescriptionAndChargeDetailsForSRM tnf = new His.Business.ZZSB.GetPrescriptionAndChargeDetailsForSRM();
            return tnf.GetGetPrescriptionAndChargeDetailsForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取导诊信息")]
        public string GetGuideListInfoForSRM(string xml)
        {
            His.Business.ZZSB.GetGuideListInfoForSRM tnf = new His.Business.ZZSB.GetGuideListInfoForSRM();
            return tnf.GetGetGuideListInfoForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询物价费用明细")]
        public string QueryPriceDetailForSRM(string xml)
        {
            His.Business.ZZSB.QueryPriceDetailForSRM tnf = new His.Business.ZZSB.QueryPriceDetailForSRM();
            return tnf.GetQueryPriceDetailForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询费用记录")]
        public string QueryFeeRecordForSRM(string xml)
        {
            His.Business.ZZSB.QueryFeeRecordForSRM tnf = new His.Business.ZZSB.QueryFeeRecordForSRM();
            return tnf.GetQueryFeeRecordForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询取药队列")]
        public string QueryFetchMedicineQueueForSRM(string xml)
        {
            His.Business.ZZSB.QueryFetchMedicineQueueForSRM tnf = new His.Business.ZZSB.QueryFetchMedicineQueueForSRM();
            return tnf.GetQueryFetchMedicineQueueForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询检查队列")]
        public string QueryExaminationQueueForSRM(string xml)
        {
            His.Business.ZZSB.QueryExaminationQueueForSRM tnf = new His.Business.ZZSB.QueryExaminationQueueForSRM();
            return tnf.GetQueryExaminationQueueForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询已支付的就诊记录")]
        public string QueryPaidRecordForSRM(string xml)
        {
            His.Business.ZZSB.QueryPaidRecordForSRM tnf = new His.Business.ZZSB.QueryPaidRecordForSRM();
            return tnf.GetQueryPaidRecordForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询已缴费的处方及收费项明细")]
        public string QueryPaidDetailForSRM(string xml)
        {
            His.Business.ZZSB.QueryPaidDetailForSRM tnf = new His.Business.ZZSB.QueryPaidDetailForSRM();
            return tnf.GetQueryPaidDetailForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "住院查询日清单")]
        public string QueryInPatientQListForSRM(string xml)
        {
            His.Business.ZZSB.QueryInPatientQlistForSRM tnf = new His.Business.ZZSB.QueryInPatientQlistForSRM();
            return tnf.GetOutQueryInPatientQlistForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "门诊查询日清单")]
        public string QueryOutPatientListForSRM(string xml)
        {
            His.Business.ZZSB.QueryOutPatientListForSRM tnf = new His.Business.ZZSB.QueryOutPatientListForSRM();
            return tnf.GetOutQueryOutPatientListForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "判断身份证是否已经建档")]
        public string JudgeIDCardIfHasFileForSRM(string xml)
        {
            His.Business.ZZSB.JudgeIDCardHasFileForSRM tnf = new His.Business.ZZSB.JudgeIDCardHasFileForSRM();
            return tnf.GetOutQueryOutPatientListOneForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "门诊查询日清单1")]
        public string QueryOutPatientListOneForSRM(string xml)
        {
            His.Business.ZZSB.QueryOutPatientListOneForSRM tnf = new His.Business.ZZSB.QueryOutPatientListOneForSRM();
            return tnf.GetOutQueryOutPatientListOneForSRM(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "更新自助设备打印状态")]
        public string ZZSBBarcodePrintNotification(string xml)
        {
            His.Business.ZZSB.InPatientType tnf = new His.Business.ZZSB.InPatientType();
            //string err = string.Empty;
            return tnf.ZZSBBarcodePrintNotification(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "发卡办卡")]
        public string NewCardToPatientForSRM(string xml)
        {
            His.Business.ZZSB.NewCardToPatientForSRM tnf = new His.Business.ZZSB.NewCardToPatientForSRM();
            //string err = string.Empty;
            return tnf.NewCardToPatientFor(xml);
        }

        #region 预约相关

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查预约信息")]
        public string GetAppointmentRecord(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + xml);

            His.Models.ZZSB.BookReq reqInfo = new His.Models.ZZSB.BookReq();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            reqInfo.UserID = para.Element(space + "UserID").Value;
            reqInfo.PassWord = para.Element(space + "PassWord").Value;
            reqInfo.DeviceID = para.Element(space + "DeviceID").Value;
            reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
            reqInfo.BankCode = para.Element(space + "BankCode").Value;
            reqInfo.HospCode = para.Element(space + "HospCode").Value;
            reqInfo.CardNo = para.Element(space + "CardNo").Value;
            reqInfo.CardTypeCode = para.Element(space + "CardTypeCode").Value;
            reqInfo.PatientID = para.Element(space + "PatientID").Value;

            DataSource source = new His.Business.ZZSB.Booking().GetBookingInfo(reqInfo);
            string s = XmlUtil.Serializer(source.GetType(), source);
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);

            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "预约取号，含医保，减免")]
        public string SubmitAppointment(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + xml);

            His.Models.ZZSB.SubmitBookingReq reqInfo = new His.Models.ZZSB.SubmitBookingReq();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            reqInfo.UserID = para.Element(space + "UserID").Value;
            reqInfo.PassWord = para.Element(space + "PassWord").Value;
            reqInfo.ReqTraceNo = para.Element(space + "ReqTraceNo") == null ? "" : para.Element(space + "ReqTraceNo").Value;
            reqInfo.DeviceID = para.Element(space + "DeviceID").Value;
            reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
            reqInfo.BankCode = para.Element(space + "BankCode").Value;
            reqInfo.HospCode = para.Element(space + "HospCode").Value;
            reqInfo.CardNo = para.Element(space + "CardNo").Value;
            reqInfo.CardTypeCode = para.Element(space + "CardTypeCode").Value;
            reqInfo.PatientID = para.Element(space + "PatientID").Value;
            reqInfo.IsBook = true;
            reqInfo.ordercode = para.Element(space + "ordercode").Value;
            reqInfo.PayType = para.Element(space + "PayType").Value;
            reqInfo.TotalRegFee = para.Element(space + "TotalRegFee").Value;
            reqInfo.PayAmt = para.Element(space + "PayAmt") == null ? "" : para.Element(space + "PayAmt").Value;
            reqInfo.FeeType = para.Element(space + "FeeType") == null ? "" : para.Element(space + "FeeType").Value;
            reqInfo.Payinsufeestr = para.Element(space + "Payinsufeestr").Value;
            reqInfo.BankCardNo = para.Element(space + "BankCardNo").Value;
            // reqInfo.VouchNo = para.Element(space + "VouchNo").Value;

            DataSource source = new His.Business.ZZSB.Booking().SubmitBooking(reqInfo);
            string s = XmlUtil.Serializer(source.GetType(), source);
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);

            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "预约医生排班查询")]
        public string GetDoctorListAppointment(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog("预约医生排班查询", MethodName + "入参:" + xml);
            His.Models.ZZSB.BookDeptReq reqInfo = new His.Models.ZZSB.BookDeptReq();
            string returnStr = new His.Business.ZZSB.Booking().GetBookDeptReqModel(xml, ref reqInfo);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            #region 旧注释
            //XElement rootReq = XElement.Parse(xml);
            //XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            //XElement root = rootReq.Element(space + "DataSource");
            //XElement para = root.Element(space + "message");
            //reqInfo.UserID = para.Element(space + "UserID").Value;
            //reqInfo.PassWord = para.Element(space + "PassWord").Value;
            //reqInfo.DeviceID = para.Element(space + "DeviceID").Value;
            //reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
            //reqInfo.AppCode = para.Element(space + "AppCode").Value;
            //reqInfo.HospCode = para.Element(space + "HospCode").Value;
            //reqInfo.AppTypeCode = para.Element(space + "AppTypeCode").Value;
            //reqInfo.CardTypeCode = para.Element(space + "CardTypeCode").Value;
            //DateTime reqTime = DateTime.MinValue; ;
            //if (DateTime.TryParse(para.Element(space + "ReqTime").Value, out reqTime))
            //    reqInfo.ReqTime = reqTime;
            //reqInfo.ReqTraceNo = para.Element(space + "ReqTraceNo").Value;
            //reqInfo.RegDate = para.Element(space + "RegDate").Value;
            //reqInfo.DeptCode = para.Element(space + "DeptCode").Value; 
            #endregion
            string s = new His.Business.ZZSB.Booking().QueryBookDept(reqInfo);
            His.Util.Common.HisLog.WriteLog("预约医生排班查询", MethodName + "出参:" + s);
            // string s = string.Empty;
            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "预约医生出诊时段查询")]
        public string GetDoctorListTime(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + xml);

            His.Models.ZZSB.BookDoctReq reqInfo = new His.Models.ZZSB.BookDoctReq();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            reqInfo.UserID = para.Element(space + "UserID").Value;
            reqInfo.PassWord = para.Element(space + "PassWord").Value;
            reqInfo.DeviceID = para.Element(space + "DeviceID").Value;
            reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
            reqInfo.AppCode = para.Element(space + "AppCode").Value;
            reqInfo.FunCode = para.Element(space + "FunCode").Value;
            reqInfo.AppTypeCode = para.Element(space + "AppTypeCode").Value;
            // reqInfo.CardTypeCode = para.Element(space + "CardTypeCode").Value;
            DateTime reqTime = DateTime.MinValue; ;
            if (DateTime.TryParse(para.Element(space + "ReqTime").Value, out reqTime))
                reqInfo.ReqTime = reqTime;

            //reqInfo.IsBook = true;
            reqInfo.ReqTraceNo = para.Element(space + "ReqTraceNo").Value;
            reqInfo.RegDate = para.Element(space + "RegDate").Value;
            reqInfo.DeptCode = para.Element(space + "DeptCode").Value;
            reqInfo.DoctCode = para.Element(space + "DoctorCode").Value;

            //DataSource source = //new His.Business.ZZSB.Booking().SubmitBooking(reqInfo);
            //DataSource =
            string s = new His.Business.ZZSB.Booking().QueryBookDoct(reqInfo);
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);
            // string s = string.Empty;
            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "预约挂号锁定号源")]
        public string LockRegisterAppointment(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + xml);

            string s = new His.Business.ZZSB.OutPatientReg().BookLockRegisterForSRM(xml);
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);
            // string s = string.Empty;
            return s;
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "预约挂号号源解锁")]
        public string UnLockRegisterAppointment(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + xml);

            string s = new His.Business.ZZSB.OutPatientReg().BookUnlockRegisterForSRM(xml);
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);
            // string s = string.Empty;
            return s;
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "预约挂号提交挂号")]
        public string SubmitTheRegisterAppointment(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string logGuid = Guid.NewGuid().ToString();
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + "[" + logGuid + "]:" + xml);

            #region 赋值

            His.Models.ZZSB.OutPatientReg reqInfo = new OutPatientReg();
            string returnStr = new His.Business.ZZSB.OutPatientReg().GetOutPatientRegModelForXml(xml, ref reqInfo);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            // XElement rootReq = XElement.Parse(xml);
            // XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            // XElement root = rootReq.Element(space + "message");
            // XElement para = root.Element(space + "Request");
            // reqInfo.UserID = para.Element(space + "UserID").Value;
            // //reqInfo. = para.Element(space + "PassWord").Value;
            // reqInfo.DeviceID = para.Element(space + "DeviceID").Value;

            // reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
            // //reqInfo = para.Element(space + "AppCode").Value;
            // reqInfo.FunCode = para.Element(space + "FunCode").Value;
            // reqInfo.BankCode = para.Element(space + "BankCode").Value;
            // // reqInfo.CardTypeCode = para.Element(space + "CardTypeCode").Value;
            // DateTime reqTime = DateTime.Parse(para.Element(space + "ReqTime").Value);

            // //reqInfo.IsBook = true;
            // reqInfo.ReqTraceNo = para.Element(space + "ReqTraceNo").Value;
            // reqInfo.RegDate = para.Element(space + "RegDate").Value;
            // reqInfo.DeptCode = para.Element(space + "DeptCode").Value;
            // //reqInfo.RegistTime = para.Element(space + "RegistTime").Value;

            // reqInfo.DoctorCode = para.Element(space + "DoctorCode").Value;
            // reqInfo.RegSourceID = para.Element(space + "RegSourceID").Value;
            // reqInfo.TotalRegFee = int.Parse(para.Element(space + "TotalRegFee").Value);
            // reqInfo.CardNo = para.Element(space + "CardNo").Value;
            //// reqInfo.TreatFee = para.Element(space + "TreatFee").Value;
            // //reqInfo.ServicesFee = para.Element(space + "ServicesFee").Value;
            //// reqInfo.MetaFee = para.Element(space + "MetaFee").Value;

            //// reqInfo.OtherFee = para.Element(space + "OtherFee").Value;
            // //reqInfo.PatientBookFee = para.Element(space + "PatientBookFee").Value;
            // reqInfo.PayType = para.Element(space + "PayType").Value;
            // reqInfo.FeeType = para.Element(space + "FeeType").Value;
            // reqInfo.PosID = para.Element(space + "PosID").Value;
            // reqInfo.BankCardNo = para.Element(space + "BankCardNo").Value;
            // reqInfo.PayDate = para.Element(space + "PayDate").Value;
            // reqInfo.TranSerNo = para.Element(space + "TranSerNo").Value;

            // reqInfo.PayTime = para.Element(space + "PayTime").Value;
            // reqInfo.BatchNo = para.Element(space + "BatchNo").Value;
            // reqInfo.VouchNo = para.Element(space + "VouchNo").Value;
            // reqInfo.ReferNo = para.Element(space + "ReferNo").Value;
            // reqInfo.PayAmt = decimal.Parse(para.Element(space + "PayAmt").Value);
            // reqInfo.BankCode = para.Element(space + "BankCode").Value;
            // reqInfo.MedInsureTranNo = para.Element(space + "MedInsureTranNo").Value;


            // reqInfo.MedInsureStr = para.Element(space + "MedInsureStr").Value;
            // reqInfo.MedInsureFee = decimal.Parse(para.Element(space + "MedInsureFee").Value);
            // reqInfo.PersonalFee = decimal.Parse(para.Element(space + "PersonalFee").Value);
            // reqInfo.Payinsufeestr = para.Element(space + "Payinsufeestr").Value;

            #endregion

            string s = new His.Business.ZZSB.Booking().Appointment(reqInfo);
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + "[" + logGuid + "]:" + s);

            return s;
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询预约记录")]
        public string GetOrderList(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + xml);

            His.Models.ZZSB.BookReq reqInfo = new His.Models.ZZSB.BookReq();
            XElement rootReq = XElement.Parse(xml);
            reqInfo.UserID = rootReq.Element("UserID").Value;
            reqInfo.PassWord = rootReq.Element("PassWord").Value;
            reqInfo.DeviceID = rootReq.Element("DeviceID").Value;
            reqInfo.ServiceCode = rootReq.Element("ServiceCode").Value;
            reqInfo.BankCode = rootReq.Element("BankCode").Value;
            reqInfo.HospCode = rootReq.Element("HospCode").Value;
            reqInfo.CardNo = rootReq.Element("CardNo").Value;
            reqInfo.CardTypeCode = rootReq.Element("CardTypeCode").Value;
            reqInfo.RegDate = rootReq.Element("RegDate").Value;
            reqInfo.ReqTraceNo = rootReq.Element("ReqTraceNo").Value;
            reqInfo.FunCode = rootReq.Element("FunCode").Value;

            His.Models.ZZSB.BooKInFoList source = new His.Business.ZZSB.Booking().GetBookingInfoList(reqInfo);
            string s = His.Business.ZZSB.Function.GetXml(source);//XmlUtil.Serializer(source.GetType(), source);
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);

            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询停诊信息")]
        public string GetStoppedSchedules(string requestXml)
        {
            string logID = Guid.NewGuid().ToString();
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog("查询停诊信息", MethodName + "[" + logID + "]入参:" + requestXml);

            string returnStr = new His.Business.ZZSB.OutPatientReg().GetStoppedSchedulesXML(requestXml);

            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            string responseXml = new His.Business.ZZSB.Booking().GetStoppedSchedules();
            His.Util.Common.HisLog.WriteLog("查询停诊信息", MethodName + "[" + logID + "]出参:" + responseXml);
            return responseXml;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "停诊退号接口")]
        public string ReturnTheStoppedReg(string requestXml)
        {
            string logID = Guid.NewGuid().ToString();
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog("停诊退号", MethodName + "[" + logID + "]入参:" + requestXml);
            var requestModel = new ReturnTheStoppedRegRequestModel();
            string returnStr = new His.Business.ZZSB.Booking().GetReturnTheStoppedRegRequsetModel(requestXml, ref requestModel);

            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            string responseXml = new His.Business.ZZSB.Booking().ReturnTheStoppedReg(requestModel);
            His.Util.Common.HisLog.WriteLog("停诊退号", MethodName + "[" + logID + "]出参:" + responseXml);
            return responseXml;
        }

        #endregion

        #region 住院

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "住院患者信息查询")]
        public string GetInpatientInfoForSRM(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog("住院患者信息查询", "入参" + MethodName + ":" + xml);

            His.Models.ZZSB.InPatientReq reqInfo = new InPatientReq();
            string returnStr = new His.Business.ZZSB.OutPatientReg().GetReqInfoForXml(xml, ref reqInfo);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            //XElement rootReq = XElement.Parse(xml);
            //XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            //XElement root = rootReq.Element(space + "message");
            //XElement para = root.Element(space + "Request");
            //reqInfo.UserID = para.Element(space + "UserID").Value;
            //reqInfo.ReqTime = para.Element(space + "ReqTime").Value;
            //reqInfo.PassWord = para.Element(space + "PassWord").Value;
            //reqInfo.DeviceID = para.Element(space + "DeviceID").Value;
            //reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
            //reqInfo.BankCode = para.Element(space + "BankCode").Value;
            //reqInfo.HospCode = para.Element(space + "HospCode").Value;
            ////reqInfo.CardNo = para.Element(space + "CardNo").Value;
            //reqInfo.CardTypeCode = para.Element(space + "CardTypeCode").Value;
            //reqInfo.CardNo = para.Element(space + "CardNo").Value;
            //reqInfo.Name = para.Element(space + "Name").Value;
            //reqInfo.PatientID = para.Element(space + "PatientID").Value;

            string s = new His.Business.ZZSB.Inpatient().GetInfoByInpNo(reqInfo);

            //DataSource source = new His.Business.ZZSB.Booking().GetBookingInfo(reqInfo);
            //string s = XmlUtil.Serializer(source.GetType(), source);
            His.Util.Common.HisLog.WriteLog("住院患者信息查询", "出参" + MethodName + ":" + s);

            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "住院押金缴纳")]
        public string InPatientFeePrePay(string xml)
        {
            try
            {
                //日志
                string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                His.Util.Common.HisLog.WriteLog("住院押金缴纳", "入参" + MethodName + ":" + xml);

                His.Models.ZZSB.InpatientPrePayReq reqInfo = new InpatientPrePayReq();
                XElement rootReq = XElement.Parse(xml);
                XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
                XElement root = rootReq.Element(space + "message");
                XElement para = root.Element(space + "Request");
                reqInfo.UserID = para.Element(space + "UserID").Value;
                reqInfo.ReqTime = para.Element(space + "ReqTime").Value;
                reqInfo.PassWord = para.Element(space + "PassWord").Value;
                reqInfo.DeviceID = para.Element(space + "DeviceID").Value;
                reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
                reqInfo.BankCode = para.Element(space + "BankCode").Value;
                reqInfo.HospCode = para.Element(space + "HospCode").Value;
                reqInfo.InpatientNo = para.Element(space + "InpatientNo").Value;
                reqInfo.ApplicationOrderNo = para.Element(space + "ApplicationOrderNo").Value;
                reqInfo.PlatformOrderNo = para.Element(space + "PlatformOrderNo").Value;
                reqInfo.PaymentWay = Shadow.Util.Data.Func.NConvert.ToInt32(para.Element(space + "PaymentWay").Value);
                reqInfo.SettleDate = para.Element(space + "SettleDate").Value;
                reqInfo.TermialType = para.Element(space + "TermialType").Value;
                reqInfo.TotalFee = Shadow.Util.Data.Func.NConvert.ToDecimal(para.Element(space + "TotalFee").Value);
                reqInfo.ReqTraceNo = para.Element(space + "ReqTraceNo") == null ? "" : para.Element(space + "ReqTraceNo").Value;
                reqInfo.BankCardNo = para.Element(space + "BankCardNo") == null ? "" : para.Element(space + "BankCardNo").Value;
                string s = new His.Business.ZZSB.Inpatient().InpatientFeePrepay(reqInfo);

                //DataSource source = new His.Business.ZZSB.Booking().GetBookingInfo(reqInfo);
                //string s = XmlUtil.Serializer(source.GetType(), source);
                His.Util.Common.HisLog.WriteLog("住院押金缴纳", "出参" + MethodName + ":" + s);

                return s;
            }
            catch (Exception ex)
            {
                His.Util.Common.HisLog.WriteLog("住院押金缴纳", "异常" + ex.Message);
                return ex.StackTrace.ToString() + ex.Message;
            }

        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "一日清单汇总")]
        public string QueryInPatientListOnDayForSRM(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog("一日清单汇总", MethodName + ":" + xml);

            His.Models.ZZSB.InpatientTotDayFeeReq reqInfo = new InpatientTotDayFeeReq();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            reqInfo.UserID = para.Element(space + "UserID").Value;
            reqInfo.ReqTime = para.Element(space + "ReqTime").Value;
            reqInfo.PassWord = para.Element(space + "PassWord").Value;
            reqInfo.DeviceID = para.Element(space + "DeviceID").Value;
            reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
            reqInfo.BankCode = para.Element(space + "BankCode").Value;
            reqInfo.HospCode = para.Element(space + "HospCode").Value;
            reqInfo.InpatientNo = para.Element(space + "InpatientNo").Value;

            reqInfo.FeeDate = para.Element(space + "FeeDate").Value;

            string s = new His.Business.ZZSB.Inpatient().InPatientFeeInfoTot(reqInfo);

            //DataSource source = new His.Business.ZZSB.Booking().GetBookingInfo(reqInfo);
            //string s = XmlUtil.Serializer(source.GetType(), source);
            His.Util.Common.HisLog.WriteLog("一日清单汇总", MethodName + ":" + s);

            return s;

        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "出院患者费用明细")]
        public string QueryInPatientTotalListForSRM(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog("出院患者费用明细", "入参" + MethodName + ":" + xml);

            His.Models.ZZSB.InpatientFeeDetailReq reqInfo = new InpatientFeeDetailReq();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            reqInfo.UserID = para.Element(space + "UserID").Value;
            reqInfo.ReqTime = para.Element(space + "ReqTime").Value;
            reqInfo.PassWord = para.Element(space + "PassWord").Value;
            reqInfo.DeviceID = para.Element(space + "DeviceID").Value;
            reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
            reqInfo.BankCode = para.Element(space + "BankCode").Value;
            reqInfo.HospCode = para.Element(space + "HospCode").Value;
            reqInfo.InpatientNo = para.Element(space + "InpatientNo").Value;
            reqInfo.CardNo = para.Element(space + "CardNo").Value;
            reqInfo.InvoiceNo = para.Element(space + "InvoiceNo").Value;
            reqInfo.StartDate = para.Element(space + "StartDate").Value;
            reqInfo.EndDate = para.Element(space + "EndDate").Value;


            string s = new His.Business.ZZSB.Inpatient().InPatientFeeDetial(reqInfo);

            //DataSource source = new His.Business.ZZSB.Booking().GetBookingInfo(reqInfo);
            //string s = XmlUtil.Serializer(source.GetType(), source);
            His.Util.Common.HisLog.WriteLog("出院患者费用明细", "出参" + MethodName + ":" + s);

            return s;

        }

        #endregion

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取收费类别信息")]
        public string GetDictionaries(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + xml);

            His.Models.ZZSB.ItemDictionaries reqInfo = new His.Models.ZZSB.ItemDictionaries();

            His.Business.ZZSB.QueryData.GetItemDictionariesFromXml(xml, ref reqInfo);

            His.Models.ZZSB.ItemTypeList source = new His.Business.ZZSB.QueryData().GetDictionaries(reqInfo);
            string s = His.Business.ZZSB.Function.GetXml(source);//XmlUtil.Serializer(source.GetType(), source);
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);

            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取收费项目")]
        public string GetDictionary(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + xml);

            His.Models.ZZSB.ItemDictionary reqInfo = new His.Models.ZZSB.ItemDictionary();

            His.Business.ZZSB.QueryData.GetItemDictionaryFromXml(xml, ref reqInfo);

            His.Models.ZZSB.ItemInfoList source = new His.Business.ZZSB.QueryData().GetDictionary(reqInfo);
            string s = His.Business.ZZSB.Function.GetXml(source);//XmlUtil.Serializer(source.GetType(), source);
            His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);

            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询交易记录")]
        public string QueryTradeRecords(string xml)
        {
            try
            {
                //日志
                string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                His.Util.Common.HisLog.WriteLog("查询交易记录", MethodName + ":" + xml);

                His.Models.ZZSB.TradeRecords reqInfo = new TradeRecords();
                XElement rootReq = XElement.Parse(xml);
                XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
                XElement root = rootReq.Element(space + "message");
                XElement para = root.Element(space + "Request");
                reqInfo.TOT_COST = para.Element(space + "PayAmt") == null ? "" : para.Element(space + "PayAmt").Value;
                reqInfo.TranserNo = para.Element(space + "ReqTraceNo") == null ? "" : para.Element(space + "ReqTraceNo").Value;
                reqInfo.TYPE = para.Element(space + "Type") == null ? "" : para.Element(space + "Type").Value;

                string s = new His.Business.ZZSB.QueryData().QueryTradeRecords(reqInfo);
                His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);

                return s;
            }
            catch (Exception ex)
            {
                His.Util.Common.HisLog.WriteLog("查询交易记录", ex.Message);
                return ex.StackTrace.ToString() + ex.Message;
            }
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "根据住院流水号查询住院电子票")]
        public string GetElecInvoiceUrlListByInpatientNo(string xml)
        {
            try
            {
                //日志
                string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                His.Util.Common.HisLog.WriteLog("根据住院流水号查询住院电子票", MethodName + ":" + xml);

                XElement rootReq = XElement.Parse(xml);
                XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
                XElement root = rootReq.Element(space + "InpatientNo");
                string InpatientNo = root.Value;
                return new His.Business.ZZSB.Inpatient().GetElecInvoiceUrlListByInpatientNo(InpatientNo);
            }
            catch (Exception ex)
            {
                His.Util.Common.HisLog.WriteLog("根据住院流水号查询住院电子票", ex.Message);
                return ex.StackTrace.ToString() + ex.Message;
            }
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取挂号电子票")]
        public string GetRegisterElecInvoiceUrl(string xml)
        {
            try
            {
                //日志
                string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                His.Util.Common.HisLog.WriteLog("根据住院流水号查询住院电子票", MethodName + ":" + xml);

                XElement rootReq = XElement.Parse(xml);
                XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
                XElement xeCardNo = rootReq.Element(space + "CardNo");
                XElement xeLimitDays = rootReq.Element(space + "LimitDays");

                string cardNo = xeCardNo.Value;
                string LimitDays = xeLimitDays.Value;
                return new His.Business.ZZSB.OutPatientReg().GetRegisterElecInvoiceUrl(cardNo, LimitDays);
            }
            catch (Exception ex)
            {
                His.Util.Common.HisLog.WriteLog("根据住院流水号查询住院电子票", ex.Message);
                return ex.StackTrace.ToString() + ex.Message;
            }
        }
        #region 查询交易记录按订单号 zzf 20200921 注释
        //[WebMethod(EnableSession = true, BufferResponse = true, Description = "查询交易记录按订单号")]
        //public string QueryTradeRecords2(string xml)
        //{
        //    try
        //    {
        //        //日志
        //        string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        His.Util.Common.HisLog.WriteLog("查询交易记录", MethodName + ":" + xml);

        //        His.Models.ZZSB.TradeRecords reqInfo = new TradeRecords();
        //        XElement rootReq = XElement.Parse(xml);
        //        XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
        //        XElement root = rootReq.Element(space + "message");
        //        XElement para = root.Element(space + "Request");
        //        reqInfo.TOT_COST = para.Element(space + "PayAmt") == null ? "" : para.Element(space + "PayAmt").Value;
        //        reqInfo.ORDERID = para.Element(space + "OrderId") == null ? "" : para.Element(space + "OrderId").Value;
        //        reqInfo.TYPE = para.Element(space + "Type") == null ? "" : para.Element(space + "Type").Value;

        //        string s = new His.Business.ZZSB.QueryData().QueryTradeRecords2(reqInfo);
        //        His.Util.Common.HisLog.WriteLog(HisLogType.ZZSB, MethodName + ":" + s);

        //        return s;
        //    }
        //    catch (Exception ex)
        //    {
        //        His.Util.Common.HisLog.WriteLog("查询交易记录", ex.Message);
        //        return ex.StackTrace.ToString() + ex.Message;
        //    }
        //}
        #endregion
    }
}

