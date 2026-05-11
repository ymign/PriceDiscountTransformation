using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using His.Util.Common;
using His.Models.Common;
using His.Models.Endoscope;
using System.Collections.Generic;

namespace His.Service
{
    /// <summary>
    /// Endoscope 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Endoscope : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取门诊申请单")]
        public string  getOutEndoscopeApply(string xml)
        {
            HisLog.WriteLog(HisLogType.Endoscope, xml);
            PathologicApplyBillRequestInfo reqInfo = new PathologicApplyBillRequestInfo();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            //XElement para = rootReq.Element("Request");
            //XElement para=ro.Element("Request");
            reqInfo.APLY_FLOW_NUM = para.Element(space + "APLY_FLOW_NUM").Value;
            reqInfo.PATIENT_ID = para.Element(space + "PATIENT_ID").Value;
            reqInfo.CARDNO = para.Element(space + "CARDNO").Value;
            reqInfo.BILL_NO = para.Element(space + "BILL_NO").Value;
            reqInfo.EMPI = para.Element(space + "EMPI").Value;
            reqInfo.START_TIME = para.Element(space + "START_TIME").Value;
            reqInfo.END_TIME = para.Element(space + "END_TIME").Value;
            reqInfo.EXAM_TYPE = para.Element(space + "EXAM_TYPE").Value;
            reqInfo.PATIENT_TYPE = para.Element(space + "PATIENT_TYPE").Value;
            reqInfo.PATIENT_NAME = para.Element(space + "PATIENT_NAME").Value;
            // reqInfo.SAMPLE_BARNUM = para.Element("SAMPLE_BARNUM").Value;
            DataSource<His.Models.Endoscope.ExamApply> source = new His.Business.Endoscope.Endoscope().GetOutPatientApplyBill(reqInfo);
            string s = XmlUtil.Serializer(source.GetType(), source);
            s = s.Replace("OfExamApply", "");
            s = s.Replace("<APPLYINFO>", "").Replace("</APPLYINFO>", "");
            s = s.Replace("<ApplyBill>", "<APPLYINFO>").Replace("</ApplyBill>", "</APPLYINFO>");
            HisLog.WriteLog(HisLogType.Endoscope, s);
            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取住院申请单")]
        public string getInpEndoscopeApply(string xml)
        {
            HisLog.WriteLog(HisLogType.Phathologic, xml);
            PathologicApplyBillRequestInfo reqInfo = new PathologicApplyBillRequestInfo();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            //XElement para = rootReq.Element("Request");
            //XElement para=ro.Element("Request");
            reqInfo.APLY_FLOW_NUM = para.Element(space + "APLY_FLOW_NUM").Value;
            reqInfo.PATIENT_ID = para.Element(space + "PATIENT_ID").Value;
            reqInfo.CARDNO = para.Element(space + "CARDNO").Value;
            reqInfo.BILL_NO = para.Element(space + "BILL_NO").Value;
            reqInfo.EMPI = para.Element(space + "EMPI").Value;
            reqInfo.START_TIME = para.Element(space + "START_TIME").Value;
            reqInfo.END_TIME = para.Element(space + "END_TIME").Value;
            reqInfo.EXAM_TYPE = para.Element(space + "EXAM_TYPE").Value;
            reqInfo.PATIENT_TYPE = para.Element(space + "PATIENT_TYPE").Value;
            reqInfo.PATIENT_NAME = para.Element(space + "PATIENT_NAME").Value;
          // reqInfo.SAMPLE_BARNUM = para.Element("SAMPLE_BARNUM").Value;

            DataSource<His.Models.Endoscope.ExamApply> source = new His.Business.Endoscope.Endoscope().GetInpPatientApplyBill(reqInfo);
            string s = XmlUtil.Serializer(source.GetType(), source);
            s = s.Replace("OfExamApply", "");
            s = s.Replace("<APPLYINFO>", "").Replace("</APPLYINFO>", "");
            s = s.Replace("<ApplyBill>", "<APPLYINFO>").Replace("</ApplyBill>", "</APPLYINFO>");
            HisLog.WriteLog(HisLogType.Endoscope, XmlUtil.Serializer(source.GetType(), source));
           // string retObj = His.Util.Common.XmlUtil.Serializer(source.GetType(), source);
            return s;
        }

        //[WebMethod(EnableSession = true, BufferResponse = true, Description = "样本接收通知his，更改状态，不可退费")]
        //public string cancelEndoscopeCheckInNotification(string xml)
        //{

        //His.Business.Helper.HisLog.WriteLog(Common.HisLogType.Endoscope, xml);
        //SampleReceivedRequestInfo reqInfo = new SampleReceivedRequestInfo();
        //XElement rootReq = XElement.Parse(xml);
        //XElement para = rootReq.Element("Request");
        //reqInfo.APLY_FLOW_NUM = para.Element("APLY_FLOW_NUM").Value;
        //reqInfo.BODYPART_CODE = para.Element("BODYPART_CODE").Value;
        //reqInfo.BODYPART_NAME = para.Element("BODYPART_NAME").Value;
        //reqInfo.DEPT_CODE = para.Element("DEPT_CODE").Value;
        //reqInfo.DEPT_NAME = para.Element("DEPT_NAME").Value;
        //reqInfo.ORDER_ID = para.Element("ORDER_ID").Value;
        //reqInfo.RCV_TIME = para.Element("RCV_TIME").Value;
        //reqInfo.RCVR_NAME = para.Element("RCVR_NAME").Value;
        //reqInfo.SAMPLE_BARNUM = para.Element("SAMPLE_BARNUM").Value;
        //reqInfo.SAMPLE_CODE = para.Element("SAMPLE_CODE").Value;
        //reqInfo.SAMPLE_NAME = para.Element("SAMPLE_NAME").Value;

        //DataSource source = new DataSource();
        //source = new His.Business.Pathologic.Pathologic().PathologicReceivedConfirm(reqInfo);
        //string s= His.Service.Common.XmlUtil.Serializer(source.GetType(), source);
        //His.Business.Helper.HisLog.WriteLog(Common.HisLogType.Endoscope,s);
        //return s;
        //    return "";
        //}

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "医技预约消息通知，预约之后，内镜系统通知his")]
        public string EndoscopeAppointmentNotification(string xml)
        {
            string str=string.Empty;
            if (!string.IsNullOrEmpty(xml))
            {
                His.Util.Common.HisLog.WriteLog(HisLogType.Endoscope,xml);
                str = "<?xml version=\"1.0\" encoding=\"utf-8\"?> " 
                            +@"<DataSource>
                             <return>
	                             <Code>1</Code><!--成功：1 失败：0 -->
	                             <ErrorMsg></ErrorMsg><!-- 错误说明 -->
	                             <Result> <!--具体的返回值 -->		     
                               </Result>
                             </return>
                        </DataSource> ";
                return str;

            }
            return str;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取内镜系统排班信息")]
        public string getEndoscopeSchedule(string xml)
        {

            return "";
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "门诊患者在到检登记时，内镜系统请求his该患者是否缴费")]
        public string QueryOutEndoscopeFeeStatus(string xml)
        {
            HisLog.WriteLog(HisLogType.Endoscope, xml);
            FeeStatusRequestInfo reqInfo = new FeeStatusRequestInfo();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            
            //XElement para1 = para.Element(space + "CHECKININFO");
            //XElement para2 = para.Element(space + "PATIENTINFO");
            //reqInfo.EXAM_SYSTEM_CODE = para.Element(space + "EXAM_SYSTEM_CODE").Value;
            reqInfo.APLY_FLOW_NUM = para.Element(space + "APLY_FLOW_NUM").Value;
            //reqInfo.APLY_ITM_CODE = para.Element(space + "APLY_ITM_CODE").Value;
            //reqInfo.APLY_ITM_NAME = para.Element(space + "APLY_ITM_NAME").Value;
            //reqInfo.CHECK_REG_NUM = para.Element(space + "CHECK_REG_NUM").Value;
            //reqInfo.CHECK_REG_TIME = para.Element(space + "CHECK_REG_TIME").Value;
            //reqInfo.CARDNO = para.Element(space + "CARDNO").Value;
            //reqInfo.EMPI = para.Element(space + "EMPI").Value;
            //reqInfo.ORDER_ID = para.Element(space + "ORDER_ID").Value;
            reqInfo.PATIENT_ID = para.Element(space + "PATIENT_ID").Value;
            //reqInfo.PATIENT_NAME = para.Element(space + "PATIENT_NAME").Value;
            //reqInfo.PATIENT_TYPE = para.Element(space + "PATIENT_TYPE").Value;
            if(string.IsNullOrEmpty(reqInfo.PATIENT_TYPE))
                reqInfo.PATIENT_TYPE="0";


            DataSource<List<His.Models.Endoscope.ApplyChargeInfo>> source = new His.Business.Endoscope.Endoscope().EndoscopeFeeStatus(reqInfo);
            string str = XmlUtil.Serializer(source.GetType(), source);
            str = str.Replace("OfListOfApplyChargeInfo", "");
            str = str.Replace("<ExamApply>", "").Replace("</ExamApply>", "").Replace("ApplyChargeInfo", "FEEINFO");
            HisLog.WriteLog(HisLogType.Endoscope, "调用服务QuseryOutEndoscopeFeeStatus结束，输出参数：" + str);
            return str;
           
        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "内镜系统将患者取消到检通知his，his更改是否可退费")]
        public string CancelEndoscopeCheckInNotification(string xml)
        {

            HisLog.WriteLog(HisLogType.Endoscope, xml);
            FeeStatusRequestInfo reqInfo = new FeeStatusRequestInfo();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");

          //  space = rootReq.Name.Namespace;
            //  name.LocalName = "Request";
           // XElement para = rootReq.Element(space + "Request");
            // name.LocalName = "APLY_FLOW_NUM";

            reqInfo.EXAM_SYSTEM_CODE = para.Element(space + "EXAM_SYSTEM_CODE").Value;
            reqInfo.APLY_FLOW_NUM = para.Element(space + "APLY_FLOW_NUM").Value;
            reqInfo.APLY_ITM_CODE = para.Element(space + "APLY_ITM_CODE").Value;
            reqInfo.APLY_ITM_NAME = para.Element(space + "APLY_ITM_NAME").Value;
            reqInfo.CHECK_REG_NUM = para.Element(space + "CHECK_REG_NUM").Value;
            reqInfo.CANCEL_CHECK_TIME = para.Element(space + "CANCEL_CHECK_TIME").Value;
            reqInfo.CANCEL_CHECK_REASON = para.Element(space + "CANCEL_CHECK_REASON").Value;
            // reqInfo.CARDNO = para2.Element(space + "CARDNO").Value;
            // reqInfo.EMPI = para2.Element(space + "EMPI").Value;
            reqInfo.ORDER_ID = para.Element(space + "ORDER_ID").Value;
            reqInfo.PATIENT_ID = para.Element(space + "PATIENT_ID").Value;
            //  reqInfo.PATIENT_NAME = para.Element(space + "PATIENT_NAME").Value;
            reqInfo.PATIENT_TYPE = para.Element(space + "PATIENT_TYPE").Value;


            DataSource<object> source = new His.Business.Endoscope.Endoscope().CancelCheckIn(reqInfo);
            string str = XmlUtil.Serializer(source.GetType(), source);
            str = str.Replace("OfListOfApplyChargeInfo", "");
            str = str.Replace("<ExamApply>", "").Replace("</ExamApply>", "").Replace("ApplyChargeInfo", "FEEINFO");
            HisLog.WriteLog(HisLogType.Endoscope, "调用服务cancelEndoscopeCheckInNotification结束，输出参数：" + str);
            return str;

        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "检查报告通知")]
        public string endoscopeReportNotification(string xml)
        {
            HisLog.WriteLog(HisLogType.Endoscope, xml);

            DataSource<object> source = new DataSource<object>();
            source.Return.Code = "1";
            source.Return.Result = null;
            string str = XmlUtil.Serializer(source.GetType(), source);
      
            
            //str = str.Replace("OfListOfApplyChargeInfo", "");
            //str = str.Replace("<ExamApply>", "").Replace("</ExamApply>", "").Replace("ApplyChargeInfo", "FEEINFO");
            HisLog.WriteLog(HisLogType.Endoscope, "调用服务endoscopeReportNotification结束，输出参数：" + str);
            return str;

        }

    }
}
