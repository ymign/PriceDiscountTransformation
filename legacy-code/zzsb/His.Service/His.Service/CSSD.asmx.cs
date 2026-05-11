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
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class CSSD : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询门诊患者信息")]
        public string GetDisinfectionOutpInfo(string xml)
        {
            His.Business.CSSD.GetDisinfectionOutpInfo tnf = new His.Business.CSSD.GetDisinfectionOutpInfo();
            return tnf.GetDisinfectionOutpInfoForCSSD(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "查询住院患者信息")]
        public string GetDisinfectionInpInfo(string xml)
        {
            His.Business.CSSD.GetDisinfectionInpInfo tnf = new His.Business.CSSD.GetDisinfectionInpInfo();
            return tnf.GetDisinfectionInpInfoForCSSD(xml);
        }
    }
}
