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
    /// YYSS 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class YYSS : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "营养膳食信息获取")]
        public string GetInPatientInfoForYYSS(string xml)
        {
            His.Business.YYSS.InPatientYYSS obj = new His.Business.YYSS.InPatientYYSS();
            return obj.GetInPatientYYSS(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "病人医嘱信息获取")]
        public string GetInPatientOrderForYYSS(string xml)
        {
            His.Business.YYSS.InPatientOrder obj = new His.Business.YYSS.InPatientOrder();
            return obj.GetInPatientOrderYYSS(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "门诊患者信息获取")]
        public string GetOutPatientInfo(string xml)
        {
            His.Business.YYSS.OutPatientOrder obj = new His.Business.YYSS.OutPatientOrder();
            return obj.GetOutPatientYYSS(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "门诊病人医嘱信息获取")]
        public string GetOutPatientOrder(string xml)
        {
            His.Business.YYSS.OutPatientOrder obj = new His.Business.YYSS.OutPatientOrder();
            return obj.GetOutPatientOrderYYSS(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "住院患者信息获取")]
        public string GetInPatientInfo(string xml)
        {
            His.Business.YYSS.OutPatientOrder obj = new His.Business.YYSS.OutPatientOrder();
            return obj.GetInPatientYYSS(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "住院病人医嘱信息获取")]
        public string GetInPatientOrder(string xml)
        {
            His.Business.YYSS.OutPatientOrder obj = new His.Business.YYSS.OutPatientOrder();
            return obj.GetInPatientOrderYYSS(xml);
        }
    }
}
