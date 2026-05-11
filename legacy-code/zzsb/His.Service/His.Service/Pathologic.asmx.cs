using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using His.Models.Pathologic;
using His.Business.Pathologic;
using His.Util.Common;
using His.Models.Common;

namespace His.Service
{
    /// <summary>
    /// Service1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Pathologic : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取门诊申请单")]
        public string getOutPathologicApply(string xml)
        {
         
          
            His.Util.Common.HisLog.WriteLog(HisLogType.Phathologic, xml);
            PathologicApplyBillRequestInfo reqInfo = new PathologicApplyBillRequestInfo();
            XElement rootReq = XElement.Parse(xml);           
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            //XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space+"message");           
            XElement para = root.Element(space + "Request");
            reqInfo.APLY_FLOW_NUM = para.Element(space+"APLY_FLOW_NUM").Value;
            reqInfo.PATIENT_ID = para.Element(space + "PATIENT_ID").Value;
            reqInfo.CARDNO = para.Element(space + "CARDNO").Value;
            reqInfo.BILL_NO = para.Element(space + "BILL_NO").Value;
            reqInfo.EMPI = para.Element(space + "EMPI").Value;
            reqInfo.START_TIME = para.Element(space + "START_TIME").Value;
            reqInfo.END_TIME = para.Element(space + "END_TIME").Value;
            reqInfo.EXAM_TYPE = para.Element(space + "EXAM_TYPE").Value;
            reqInfo.PATIENT_TYPE = para.Element(space + "PATIENT_TYPE").Value;
            reqInfo.PATIENT_NAME = para.Element(space + "PATIENT_NAME").Value;
           // reqInfo.SAMPLE_BARNUM = para.Element(space + "SAMPLE_BARNUM").Value;

            DataSource source = new His.Business.Pathologic.Pathologic().GetOutPatientApplyBill(reqInfo);
            string s = XmlUtil.Serializer(source.GetType(), source);
            s = s.Replace("</APPLYINFO>", "").Replace("<APPLYINFO>", "");
            s = s.Replace("</ApplyBill>", "</APPLYINFO>").Replace("<ApplyBill>", "<APPLYINFO>");
            His.Util.Common.HisLog.WriteLog(HisLogType.Phathologic, s);
            
            
            return s;

        }


        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取住院申请单")]
        public string getInpPathologicApply(string xml)
        {
            His.Util.Common.HisLog.WriteLog(HisLogType.Phathologic, xml);
            PathologicApplyBillRequestInfo reqInfo = new PathologicApplyBillRequestInfo();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
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
            //reqInfo.SAMPLE_BARNUM = para.Element(space + "SAMPLE_BARNUM").Value;

            DataSource source = new His.Business.Pathologic.Pathologic().GetInpPatientApplyBill(reqInfo);
            string s = XmlUtil.Serializer(source.GetType(), source);
            s = s.Replace("</APPLYINFO>", "").Replace("<APPLYINFO>", "");
            s = s.Replace("</ApplyBill>", "</APPLYINFO>").Replace("<ApplyBill>", "<APPLYINFO>");
            His.Util.Common.HisLog.WriteLog(HisLogType.Phathologic, s);

            return s;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "样本接收通知his，更改状态，不可退费")]
        public string  pathologicReceivedNotification(string xml)
        {

            His.Util.Common.HisLog.WriteLog(HisLogType.Phathologic, xml);
            SampleReceivedRequestInfo reqInfo = new SampleReceivedRequestInfo();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            reqInfo.APLY_FLOW_NUM = para.Element(space + "APLY_FLOW_NUM").Value;
            reqInfo.BODYPART_CODE = para.Element(space + "BODYPART_CODE").Value;
            reqInfo.BODYPART_NAME = para.Element(space + "BODYPART_NAME").Value;
            reqInfo.DEPT_CODE = para.Element(space + "DEPT_CODE").Value;
            reqInfo.DEPT_NAME = para.Element(space + "DEPT_NAME").Value;
            reqInfo.ORDER_ID = para.Element(space + "ORDER_ID").Value;
            reqInfo.RCV_TIME = para.Element(space + "RCV_TIME").Value;
            reqInfo.RCVR_NAME = para.Element(space + "RCVR_NAME").Value;
            reqInfo.SAMPLE_BARNUM = para.Element(space + "SAMPLE_BARNUM").Value;
            reqInfo.SAMPLE_CODE = para.Element(space + "SAMPLE_CODE").Value;
            reqInfo.SAMPLE_NAME = para.Element(space + "SAMPLE_NAME").Value;
            reqInfo.PATIENT_TYPE = para.Element(space + "PATIENT_TYPE").Value;
           // reqInfo.PATIENT_NAME = para.Element(space + "PATIENT_NAME").Value;

            DataSource source = new DataSource();
            source = new His.Business.Pathologic.Pathologic().PathologicReceivedConfirm(reqInfo);
            string s= XmlUtil.Serializer(source.GetType(), source);
            His.Util.Common.HisLog.WriteLog(HisLogType.Phathologic,s);
            return s;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "检查报告通知")]
        public string phathologicReportNotification(string xml)
        {
            HisLog.WriteLog(HisLogType.Endoscope, xml);

            DataSource source = new DataSource();
            source.Return.Code = "1";
            source.Return.Result = null;
            string str = XmlUtil.Serializer(source.GetType(), source);

            HisLog.WriteLog(HisLogType.Endoscope, "调用服务phathologicReportNotification结束，输出参数：" + str);
            return str;

        }
    }
}
