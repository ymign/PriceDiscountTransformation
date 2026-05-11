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
using His.Util.Common;
using His.Models.BUltrasonic;

namespace His.Service
{
    /// <summary>
    /// BUltrasonic 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class BUltrasonic : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "获取门诊申请单")]
        public string getExamApply(string xml)
        {
            //日志
            His.Util.Common.HisLog.WriteLog(HisLogType.BUltrasonic, xml);

            His.Models.BUltrasonic.RequestApplyModel reqInfo = new His.Models.BUltrasonic.RequestApplyModel();
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            //XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            XElement root = rootReq.Element(space + "message");
            XElement para = root.Element(space + "Request");
            reqInfo.APLY_FLOW_NUM = para.Element(space + "APLY_FLOW_NUM").Value;
            reqInfo.PATIENT_ID = para.Element(space + "PATIENT_ID").Value;
            reqInfo.CARDNO = para.Element(space + "CARDNO").Value;
            reqInfo.BILL_NO = para.Element(space + "BILL_NO").Value;
           // reqInfo.EMPI = para.Element(space + "EMPI").Value;
            reqInfo.START_TIME = para.Element(space + "START_TIME").Value;
            reqInfo.END_TIME = para.Element(space + "END_TIME").Value;
            reqInfo.EXAM_TYPE = para.Element(space + "EXAM_TYPE").Value;
            reqInfo.PATIENT_TYPE = para.Element(space + "PATIENT_TYPE").Value;
            reqInfo.EXECUTIVE_DEPT = para.Element(space + "EXECUTIVE_DEPT").Value;
            // reqInfo.SAMPLE_BARNUM = para.Element(space + "SAMPLE_BARNUM").Value;

           DataSource source = new His.Business.BUltrasonic.BUltrasonic().GetApplyBill(reqInfo);
            string s = XmlUtil.Serializer(source.GetType(), source);
            s = s.Replace("</APPLYINFO>", "").Replace("<APPLYINFO>", "");
            s = s.Replace("</ApplyBill>", "</APPLYINFO>").Replace("<ApplyBill>", "<APPLYINFO>");
            His.Util.Common.HisLog.WriteLog(HisLogType.BUltrasonic, s);


            return s;

        }
    }
}
