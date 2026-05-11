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
    /// LIS 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class LIS : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "LIS门诊申请单获取")]
        public string GetOutPatientApply(string xml)
        {
            His.Business.LIS.OutPatientApply opa = new His.Business.LIS.OutPatientApply();
            return opa.GetOutPatientApply(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "LIS住院申请单获取")]
        public string GetInPatientApply(string xml)
        {
            His.Business.LIS.InPatientApply ipa = new His.Business.LIS.InPatientApply();
            string s= ipa.GetInPatientApply(xml);
            His.Util.Common.HisLog.WriteLog("LisApply", s);
            return s;
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "LIS更新条码打印状态")]
        public string LisBarcodePrintNotification(string xml)
        {
            His.Business.LIS.InPatientApply ipa = new His.Business.LIS.InPatientApply();
            return ipa.LisBarcodePrintNotification(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "LIS样本接收确认通知")]
        public string LisSampleReceivedNotification(string xml)
        {
            His.Business.LIS.InPatientApply ipa = new His.Business.LIS.InPatientApply();
            return ipa.LisSampleReceivedNotification(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "取住院病人信息")]
        public string LisGetInPatientInfo(string xml)
        {
            His.Business.LIS.Patientzy ipa = new His.Business.LIS.Patientzy();
            return ipa.GetInPatientInfoForZY(xml);
        }
    }
}
