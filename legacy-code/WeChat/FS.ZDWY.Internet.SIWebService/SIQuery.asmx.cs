using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.SIWebService
{
    /// <summary>
    /// SIQuery 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class SIQuery : System.Web.Services.WebService
    {
        #region 属性

        LogHelper.ServiceLog serviceLogManager;
        /// <summary>
        /// 服务日志管理
        /// </summary>
        LogHelper.ServiceLog ServiceLogManager
        {
            get
            {
                if (serviceLogManager == null)
                {
                    serviceLogManager = new LogHelper.ServiceLog();
                }
                return serviceLogManager;
            }
        }

        #endregion

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }


        [WebMethod(Description = "hlht_省医保_入院登记后取业务信息")]
        public string rydjxx(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string function_id = "";
                string bka895 = "";
                string bka896 = "";
                string akb020 = "";
                string aka130 = "";
                string bka891 = "";
                string aae030 = "";
                string aae031 = "";

                function_id = Function.GetNoteValue(xmlDoc, "program/function_id");
                bka895 = Function.GetNoteValue(xmlDoc, "program/bka895");
                bka896 = Function.GetNoteValue(xmlDoc, "program/bka896");
                akb020 = Function.GetNoteValue(xmlDoc, "program/akb020");
                aka130 = Function.GetNoteValue(xmlDoc, "program/aka130");
                bka891 = Function.GetNoteValue(xmlDoc, "program/bka891");
                aae030 = Function.GetNoteValue(xmlDoc, "program/aae030");
                aae031 = Function.GetNoteValue(xmlDoc, "program/aae031");

                Manager manger = new Manager();
                string resxml = manger.RYDJXX(function_id, bka895, bka896, akb020, aka130, bka891, aae030, aae031);
                ServiceLogManager.Write("传出报文：" + resxml);
                return resxml;

            }
            catch (Exception e)
            {
                string xml = @"
<program>
    <return_code>0</return_code>
    <return_code_message>{0}</return_code_message>
</program>";
                xml = string.Format(xml, e.Message.ToString());
                ServiceLogManager.Write("传出报文：" + xml);
                return xml;
            }
        }


        [WebMethod(Description = "hlht_省医保_2.3hlht_省医保_费用清单信息提取")]
        public string fyqdxx(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string function_id = "";
                string akb020 = "";
                string aaz218 = "";
                string Operate = "";
                string Secfalg = "";
                string aac002 = "";
                string fromdate = "";

                 function_id = Function.GetNoteValue(xmlDoc, "program/function_id");
                 akb020 = Function.GetNoteValue(xmlDoc, "program/akb020");
                 aaz218 = Function.GetNoteValue(xmlDoc, "program/aaz218");
                 Operate = Function.GetNoteValue(xmlDoc, "program/Operate");
                 Secfalg = Function.GetNoteValue(xmlDoc, "program/Secfalg");
                 aac002 = Function.GetNoteValue(xmlDoc, "program/aac002");
                 fromdate = Function.GetNoteValue(xmlDoc, "program/fromdate");
                 Manager manger = new Manager();
                 string resxml = manger.FYQDXX(function_id, akb020, aaz218, Operate, Secfalg, aac002, fromdate);
                 ServiceLogManager.Write("传出报文：" + resxml);
                 return resxml;

            }
            catch(Exception e)
            {
                string xml = @"
<program>
    <return_code>0</return_code>
    <return_code_message>{0}</return_code_message>
</program>";
                xml = string.Format(xml,e.Message.ToString());
                ServiceLogManager.Write("传出报文：" + xml);
                return xml;
            }
        }
    }
}
