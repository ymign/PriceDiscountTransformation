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
    public class QuitRegFeeService
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

        public string InPutMessage(FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY pay, ref string ero)
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
<orderId>{1}</orderId>
<hospTradeId>{2}</hospTradeId>
<payMode>{3}</payMode>
<payAmt>{4}</payAmt>
</data></Request>";
                xmldata = string.Format(xmldata, pay.PATIENTID, pay.ORDERID, pay.HospTradeId, pay.PAYMODE, pay.PAYAMT);
                xml = string.Format(xmlcontent, xmldata);

                return xml;
            }
            catch (Exception e)
            {
                ero = e.Message.ToString();
                return "";
            }
        }


        public int CallService(FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY pay, ref string ero)
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
}
