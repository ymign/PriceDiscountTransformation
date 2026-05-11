using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Description;
using System.Xml;

namespace FS.ZDWY.Internet.BP.QuitFee
{
    public class QuitFeeService
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        private string url = "";

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        #region 日志
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

        public string InPutMessage(FS.ZDWY.Internet.Models.PLATFORM_BALANCE_PAY pay, ref string ero)
        {
            string xml = "";
            ero = "";
            try
            {
                string xmlcontent = @"<?xml version='1.0' encoding='utf-8'?>
<soap:Envelope xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
   <soap:Header>
      <AuthorizationSoapHeader xmlns='http://datareceive.service.esb.medata.com/'>
         <MDIP_ACCESSTOKEN>7EAC38D1C96B236EAD9A7A148FE1C513</MDIP_ACCESSTOKEN>
      </AuthorizationSoapHeader>
   </soap:Header>
   <soap:Body>
      <ns2:dataReceive xmlns:ns2='http://datareceive.service.esb.medata.com/'>
         <param><![CDATA[
	    {0}
         ]]></param>
      </ns2:dataReceive>
   </soap:Body>
</soap:Envelope>";
                //内容
                string xmldata = @"<Request><data>
<patientId>{0}</patientId>
<hospTradeId>{1}</hospTradeId>
<payMode>{2}</payMode>
<payAmt>{3}</payAmt>
<orderId>{4}</orderId>
</data></Request>";
                xmldata = string.Format(xmldata, pay.PATIENTID, pay.HOSPITALNUM, pay.PAYMODE, pay.PAYAMT,pay.ORDERID);
                xml = string.Format(xmlcontent, xmldata);

                return xml;
            }
            catch (Exception e)
            {
                ero = e.Message.ToString();
                return "";
            }
        }


        public int CallService(FS.ZDWY.Internet.Models.PLATFORM_BALANCE_PAY pay,ref string ero)
        {
            #region webservice 要使用post
            /*try
            {
                string xml = InPutMessage(pay, ref ero);
                if(string.IsNullOrEmpty(xml))
                {
                    return -1;
                }
                ServiceLogManager.Write("入参报文：" + xml);
                var result = WebService.InvokeWebService(Url, "dataReceive", new string[] { xml }, ref ero);
                if (result == null)
                {
                    ero = "服务异常！";
                    return -9;
                }
                ServiceLogManager.Write("出参报文：" + result.ToString());
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result.ToString());
                string res = GetValue(xmlDoc, "/Response/ok");
                if (res != "1")
                {
                    ero = GetValue(xmlDoc, "/Response/errorMsg");
                    return -1;
                }
                return 1;
            }
            catch(Exception e)
            {
                ero = e.Message.ToString();
                return -9;
            }*/
            #endregion

            #region
            
            try
            {
                HttpWebRequest httpRequest;
                httpRequest = (HttpWebRequest)WebRequest.Create(Url);
                httpRequest.Timeout = 30 * 1000;
                httpRequest.ReadWriteTimeout = 30 * 1000;
                httpRequest.Method = "POST";
                httpRequest.ContentType = "text/xml";

                string xml = InPutMessage(pay, ref ero);
                if (string.IsNullOrEmpty(xml))
                {
                    return -1;
                }
                ServiceLogManager.Write("入参报文：" + xml);

                byte[] requestData = Encoding.UTF8.GetBytes(xml);
                httpRequest.ContentLength = requestData.Length;
                var reqStream = httpRequest.GetRequestStream();
                reqStream.Write(requestData, 0, requestData.Length);
                reqStream.Flush();
                reqStream.Close();

                var responseResult = (HttpWebResponse)httpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(responseResult.GetResponseStream(), Encoding.UTF8);
                string result = streamReader.ReadToEnd();
                result = result.Replace("&gt;", ">");
                result = result.Replace("&lt;", "<");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                ServiceLogManager.Write("出参报文：" + xmlDoc.InnerXml.ToString());

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");

                string value = xmlDoc.SelectSingleNode("//soap:Envelope", nsmgr).InnerText;

                string res = "-1";
                if(value.ToUpper().Contains("TRUE"))
                {
                    res = "1";
                }

                if (res != "1")
                {
                    ero = value;
                    return -1;
                }
                return 1;
            }
            catch (Exception ex)
            {
                ero = ex.Message.ToString();
                return -9;
            }
            #endregion
        }

        protected virtual string GetValue(XmlDocument xmlDoc, string xpath)
        {
            var node = xmlDoc.SelectSingleNode(xpath);
            if (node == null)
            {
                return "不存在节点" + xpath;
            }
            return node.InnerText;
        }
    }

    public class WebService
    {
        public static object InvokeWebService(string url, string methodname, object[] args, ref string error)
        {

            //这里的namespace是需引用的webservices的命名空间，在这里是写死的，大家可以加一个参数从外面传进来。
            try
            {
                //获取WSDL
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url);
                ServiceDescription sd = ServiceDescription.Read(stream);
                string classname = sd.Services[0].Name;

                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.ProtocolName = "Soap"; // 指定访问协议。
                sdi.Style = ServiceDescriptionImportStyle.Client; // 生成客户端代理。
                sdi.AddServiceDescription(sd, "", "");

                CodeNamespace cn = new CodeNamespace();
                //cn.Name = "AAAA";

                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                ICodeCompiler icc = csc.CreateCompiler();

                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new System.Exception(sb.ToString());
                }

                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);

                return mi.Invoke(obj, args);
            }
            catch (System.Exception e)
            {
                error = e.Message;
                return null;
            }
        }

        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');

            return pps[0];
        }
    }
}
