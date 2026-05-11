using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
//using His.Business.wirelessAnalgesic;

namespace His.Service
{
    /// <summary>
    /// WirelessAnalgesic 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class WirelessAnalgesic : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "无线镇痛信息获取")]
        public string GetInOperationInfo(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return string.Empty;
            His.Util.Common.HisLog.WriteLog("无线镇痛", xml);
            XElement rootReq = XElement.Parse(xml);
            XNamespace space = (rootReq.FirstNode as XElement).Name.Namespace;
            string patient_id = rootReq.Element(space + "PATIENT_ID").Value;
            if (string.IsNullOrEmpty(patient_id))
            {
                return null;
            }
            His.Business.wirelessAnalgesic.wirelessAnalgesic obj = new His.Business.wirelessAnalgesic.wirelessAnalgesic();
            string str = obj.GetInOperationInfo(patient_id);

            His.Util.Common.HisLog.WriteLog("无线镇痛", str);
            return str;
        }
     
    }
}
