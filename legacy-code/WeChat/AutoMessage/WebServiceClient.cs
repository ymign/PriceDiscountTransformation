using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Web.Services.Description;
using System.Xml;

namespace AutoMessage
{
    /// <summary>
    /// 调用WebService
    /// </summary>
    public class WebServiceClient
    {
        /// <summary>
        /// 调用webService
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="methodname">方法名</param>
        /// <param name="args">参数</param>
        /// <param name="strErrInfo">错误信息</param>
        /// <returns>返回值</returns>
        public static string InvokeWebService(string url, string methodname, object[] args, ref string strErrInfo)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            string classname = GetWsClassName(url);
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            try
            {
                //获取WSDL   
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?wsdl");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

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
                    throw new Exception(sb.ToString());
                }

                //生成代理实例，并调用方法   
                System.Reflection.Assembly assembly = cr.CompiledAssembly;

                Type[] types = assembly.GetTypes();
                Type t = null;
                if (types != null && types.Length > 0)
                {
                    t = types[0];
                    object obj = Activator.CreateInstance(t);
                    System.Reflection.MethodInfo mi = t.GetMethod(methodname);

                    return mi.Invoke(obj, args).ToString();
                }
                return "";


            }
            catch (Exception ex)
            {
                strErrInfo = ex.Message;
                return "";
            }
        }

       static  LogHelper.ServiceLog serviceLogManager;
        /// <summary>
        /// 服务日志管理
        /// </summary>
       static LogHelper.ServiceLog ServiceLogManager
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


        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');

            return pps[0];
        }


        public static int CallService(string Url, string xml, ref string returnStr,ref string ero)
        {
            #region

            try
            {
                HttpWebRequest httpRequest;
                httpRequest = (HttpWebRequest)WebRequest.Create(Url);
                httpRequest.Timeout = 30 * 1000;
                httpRequest.ReadWriteTimeout = 30 * 1000;
                httpRequest.Method = "POST";
                httpRequest.ContentType = "text/xml";

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

                returnStr = value;

                string res = "-1";
                if (value.ToUpper().Contains("TRUE"))
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
    }
}
