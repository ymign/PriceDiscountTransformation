using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

namespace His.Service
{
    /// <summary>
    /// WeiXin 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class WeiXin : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "提交挂号")]
        public string Register(string xml)
        {
            His.Util.Common.HisLog.WriteLog("WinXin", xml);
            His.Business.WeiXin.Register reg = new His.Business.WeiXin.Register();
            His.Models.ZZSB.OutPatientReg opr = new His.Models.ZZSB.OutPatientReg();

            #region 入参实体

            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement para = rootReq.Element(space + "message");
            //XElement para = root.Element(space + "Request");

            opr.UserID = para.Element(space + "UserID").Value;
            opr.DeviceID = para.Element(space + "DeviceID").Value;
            opr.ServiceCode = para.Element(space + "ServiceCode").Value;
            opr.FunCode = para.Element(space + "FunCode").Value;
            opr.ReqTime = para.Element(space + "ReqTime").Value;
            opr.ReqTraceNo = para.Element(space + "ReqTraceNo").Value;
            opr.CardNo = para.Element(space + "CardNo").Value;
            opr.DeptCode = para.Element(space + "DeptCode").Value;
            opr.SessionCode = para.Element(space + "SessionCode").Value;
            opr.DoctorCode = para.Element(space + "DoctorCode").Value;
            opr.RegSourceID = para.Element(space + "RegSourceID").Value;
            opr.TranSerNo = para.Element(space + "TranSerNo").Value;
            opr.TotalRegFee = decimal.Parse(para.Element(space + "TotalRegFee").Value);
            opr.PayType = para.Element(space + "PayType").Value;
            opr.PosID = para.Element(space + "PosID").Value;
            opr.BankCardNo = para.Element(space + "BankCardNo").Value;
            opr.PayDate = para.Element(space + "PayDate").Value;
            opr.PayTime = para.Element(space + "PayTime").Value;
            opr.BatchNo = para.Element(space + "BatchNo").Value;
            opr.VouchNo = para.Element(space + "VouchNo").Value;
            opr.ReferNo = para.Element(space + "ReferNo").Value;
            opr.PayAmt = decimal.Parse(para.Element(space + "PayAmt").Value);
            opr.BankCode = para.Element(space + "BankCode").Value;
            opr.MedInsureTranNo = para.Element(space + "MedInsureTranNo").Value;
            opr.MedInsureStr = para.Element(space + "MedInsureStr").Value;
            opr.MedInsureFee = decimal.Parse(para.Element(space + "MedInsureFee").Value);
            opr.PersonalFee = decimal.Parse(para.Element(space + "PersonalFee").Value);
            opr.Payinsufeestr = para.Element(space + "Payinsufeestr").Value;
            opr.FeeType = para.Element(space + "FeeType").Value;

            #endregion

            string result = reg.SubmitRegister(opr);
            His.Util.Common.HisLog.WriteLog("WinXin", result);
            return result;
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "预约挂号")]
        public string Appointment(string xml)
        {
            //日志
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            His.Util.Common.HisLog.WriteLog("WeiXin", MethodName + ":" + xml);

            #region 赋值

            His.Models.ZZSB.OutPatientReg reqInfo = new His.Models.ZZSB.OutPatientReg();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            
            reqInfo.UserID = para.Element(space + "UserID").Value;
            reqInfo.DeviceID = para.Element(space + "DeviceID").Value;
            reqInfo.ServiceCode = para.Element(space + "ServiceCode").Value;
            reqInfo.FunCode = para.Element(space + "FunCode").Value;
            reqInfo.BankCode = para.Element(space + "BankCode").Value;
            DateTime reqTime = DateTime.Parse(para.Element(space + "ReqTime").Value);
            reqInfo.ReqTraceNo = para.Element(space + "ReqTraceNo").Value;
            reqInfo.RegDate = para.Element(space + "RegDate").Value;
            reqInfo.DeptCode = para.Element(space + "DeptCode").Value;
            reqInfo.DoctorCode = para.Element(space + "DoctorCode").Value;
            reqInfo.RegSourceID = para.Element(space + "RegSourceID").Value;
            reqInfo.TotalRegFee = int.Parse(para.Element(space + "TotalRegFee").Value);
            reqInfo.CardNo = para.Element(space + "CardNo").Value;
            reqInfo.PayType = para.Element(space + "PayType").Value;
            reqInfo.FeeType = para.Element(space + "FeeType").Value;
            reqInfo.PosID = para.Element(space + "PosID").Value;
            reqInfo.BankCardNo = para.Element(space + "BankCardNo").Value;
            reqInfo.PayDate = para.Element(space + "PayDate").Value;
            reqInfo.TranSerNo = para.Element(space + "TranSerNo").Value;
            reqInfo.PayTime = para.Element(space + "PayTime").Value;
            reqInfo.BatchNo = para.Element(space + "BatchNo").Value;
            reqInfo.VouchNo = para.Element(space + "VouchNo").Value;
            reqInfo.ReferNo = para.Element(space + "ReferNo").Value;
            reqInfo.PayAmt = decimal.Parse(para.Element(space + "PayAmt").Value);
            reqInfo.BankCode = para.Element(space + "BankCode").Value;
            reqInfo.MedInsureTranNo = para.Element(space + "MedInsureTranNo").Value;
            reqInfo.MedInsureStr = para.Element(space + "MedInsureStr").Value;
            reqInfo.MedInsureFee = decimal.Parse(para.Element(space + "MedInsureFee").Value);
            reqInfo.PersonalFee = decimal.Parse(para.Element(space + "PersonalFee").Value);
            reqInfo.Payinsufeestr = para.Element(space + "Payinsufeestr").Value;

            #endregion

            string s = new His.Business.WeiXin.Register().SubmitAppointment(reqInfo);
            His.Util.Common.HisLog.WriteLog("WeiXin", MethodName + ":" + s);
            // string s = string.Empty;
            return s;
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "住院押金缴纳")]
        public string InPatientFeePrePay(string xml)
        {
            try
            {
                //日志
                string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                His.Util.Common.HisLog.WriteLog("预交金", MethodName + ":" + xml);

                His.Models.ZZSB.InpatientPrePayReq reqInfo = new His.Models.ZZSB.InpatientPrePayReq();
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
                reqInfo.PaymentWay = Shadow.Util.Data.Func.NConvert.ToInt32(para.Element(space + "PaymentWay").Value);
                reqInfo.SettleDate = para.Element(space + "SettleDate").Value;
                reqInfo.TermialType = para.Element(space + "TermialType").Value;
                reqInfo.TotalFee = Shadow.Util.Data.Func.NConvert.ToDecimal(para.Element(space + "TotalFee").Value);

                string s = new His.Business.ZZSB.Inpatient().InpatientFeePrepay(reqInfo);
                His.Util.Common.HisLog.WriteLog("预交金", MethodName + ":" + s);

                return s;
            }
            catch (Exception ex)
            {
                His.Util.Common.HisLog.WriteLog("预交金", ex.Message);
                return ex.StackTrace.ToString() + ex.Message;
            }

        }

    }
}
