using FS.ZDWY.Internet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Xml.Serialization;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// Inpatient 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Inpatient : System.Web.Services.WebService
    {
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
        [WebMethod(Description = "查询入院通知单信息")]
        public IprPrepayinInfo GetIprPrepayinByCardNo(string cardNO)
        {
            string GUID = Guid.NewGuid().ToString();
            try
            {
                ServiceLogManager.Write("查询入院通知单信息" + GUID + "传入参数：" + cardNO);
                BP.InPatient.InRegister register = new BP.InPatient.InRegister();
                return register.GetIprPrepayinByCardNo(cardNO);
            }
            catch (Exception ex)
            {
                ServiceLogManager.Write("查询入院通知单信息" + GUID + "操作失败！" + ex.Message);
                throw ex;
            }
        }
        [WebMethod(Description = "预填写入院申请信息保存")]
        public string InPatientSave(Models.InPatientRegistInfo Info)
        {
            string resXml = "";
            string Msg = "";
            string GUID = Guid.NewGuid().ToString();
            string xml = XmlSerialize(Info);
            try
            {
                ServiceLogManager.Write("预填写入院申请信息保存" + GUID + "传入参数：" + xml);
                BP.InPatient.InRegister register = new BP.InPatient.InRegister();
                if (register.InPatientSave(Info, ref Msg) == 1)
                {
                    resXml = Function.GetResponseXML(true, "操作成功！" + Msg, "");
                    ServiceLogManager.Write("预填写入院申请信息保存" + GUID + "传出参数：" + resXml);
                    return resXml;
                }
                else
                {
                    resXml = Function.GetResponseXML(false, "操作失败！" + Msg, "");
                    ServiceLogManager.Write("预填写入院申请信息保存" + GUID + "传出参数：" + resXml);
                    return resXml;
                }
            }
            catch (Exception ex)
            {
                resXml = Function.GetResponseXML(false, "操作失败！" + ex.Message, "");
                ServiceLogManager.Write("预填写入院申请信息保存" + GUID + "传出参数：" + resXml);
                return resXml;
            }

        }
        [WebMethod(Description = "预填写入院申请信息查询")]
        public Models.InPatientRegistInfo GetInPatientRegistInfo(string cardNO)
        {
            string GUID = Guid.NewGuid().ToString();
            try
            {
                ServiceLogManager.Write("预填写入院申请信息查询" + GUID + "传入参数：" + cardNO);
                BP.InPatient.InRegister register = new BP.InPatient.InRegister();
                return register.GetInPatientRegistInfo(cardNO);
            }
            catch (Exception ex)
            {
                ServiceLogManager.Write("预填写入院申请信息查询" + GUID + "操作失败！" + ex.Message);
                throw ex;
            }
        }

        [WebMethod(Description = "查询上一次住院信息")]
        public Models.InPatientRegistInfo GetLastRegistInfo(string cardNO)
        {
            string GUID = Guid.NewGuid().ToString();
            try
            {
                ServiceLogManager.Write("查询上一次住院信息" + GUID + "传入参数：" + cardNO);
                BP.InPatient.InRegister register = new BP.InPatient.InRegister();
                return register.GetLastRegistInfo(cardNO);
            }
            catch (Exception ex)
            {
                ServiceLogManager.Write("查询上一次住院信息" + GUID + "操作失败！" + ex.Message);
                throw ex;
            }
        }

        [WebMethod(Description = "住院登记")]
        public string InPatientRegist(Models.InPatientRegistInfo Info)
        {
            string resXml = "";
            string Msg = "";
            string GUID = Guid.NewGuid().ToString();
            string xml = XmlSerialize(Info);
            try
            {
                ServiceLogManager.Write("住院登记"+ GUID + "传入参数：" + xml);
                BP.InPatient.InRegister register = new BP.InPatient.InRegister();
                if (register.insertPatientInfo(Info, "readCard", "", ref Msg) == 1)
                {
                    resXml = Function.GetResponseXML(true, "操作成功！" + Msg, "");
                    ServiceLogManager.Write("住院登记" + GUID + "传出参数：" + resXml);
                    return resXml;
                }
                else
                {
                    resXml = Function.GetResponseXML(false, "操作失败！" + Msg, "");
                    ServiceLogManager.Write("住院登记" + GUID + "传出参数：" + resXml);
                    return resXml;
                }
            }
            catch (Exception ex)
            {
                resXml = Function.GetResponseXML(false, "操作失败！" + ex.Message, "");
                ServiceLogManager.Write("住院登记" + GUID + "传出参数：" + resXml);
                return resXml;
            }
            
        }
        [WebMethod(Description = "字典获取")]
        public List<DicObject> GetDictionary(string TYPE)
        {
            BP.InPatient.InRegister register = new BP.InPatient.InRegister();
            return register.GetDictionary(TYPE);
        }

        /// <summary>
        /// 将实体类转换成XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string XmlSerialize<T>(T obj)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.IndentChars = "";
            settings.NewLineChars = "";
            //settings.NewLineChars = "\r\n";
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = true; // 不生成声明头
            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(sw, settings))
                {
                    // 强制指定命名空间，覆盖默认的命名空间
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    var serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(xmlWriter, obj, namespaces);
                    xmlWriter.Close();
                    string serialized = sw.ToString();
                    return serialized;
                }
            }

        }


    }
}
