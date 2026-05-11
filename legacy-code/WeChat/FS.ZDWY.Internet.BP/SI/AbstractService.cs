using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FS.ZDWY.Internet.BP.SI
{
    public abstract class AbstractService<T, E>
    {
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

        /// <summary>
        /// 业务编号
        /// </summary>
        public abstract string FunctionID
        {
            get;
        }

        /// <summary>
        /// 执行结果返回编码
        /// </summary>
        protected string ReturnCode = string.Empty;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>组织入参</summary>
        /// <param name="t">入参对象</param>      
        /// <param name="xml">附加参数</param>
        /// <returns>-1 失败 1 成功</returns>
        protected abstract int ConvertModelToSendMessage(T t, out string xml, params object[] appendParams);


        /// <summary>
        /// 处理出参
        /// </summary>
        /// <param name="xmlDoc">中心返回的消息</param>
        /// <returns>-1 失败 1 成功</returns>
        protected virtual int ConvertReciverMessageToModel(XmlDocument xmlDoc, ref E reciverObject)
        {
            return 1;
        }
        /// <summary>调用接口服务</summary>
        /// <param name="sendObject">入参实体</param>
        /// <param name="reciverObject">出参实体</param>
        /// <param name="appendParams">备用参数</param>
        /// <returns>-1 失败 1 成功</returns>
        public int CallService(T sendObject, ref E reciverObject, params object[] appendParams)
        {
           try
           {
               if (string.IsNullOrWhiteSpace(FS.ZDWY.Internet.BP.SI.ReadSIConfig.SessionId) || FS.ZDWY.Internet.BP.SI.ReadSIConfig.SessionId == "-9")
               {
                   InitializeSiSession();
               }
               HttpWebRequest httpRequest;
               httpRequest = (HttpWebRequest)WebRequest.Create(FS.ZDWY.Internet.BP.SI.ReadSIConfig.ServiceUrl);
               httpRequest.Timeout = FS.ZDWY.Internet.BP.SI.ReadSIConfig.TimeOut * 1000;
               httpRequest.ReadWriteTimeout = FS.ZDWY.Internet.BP.SI.ReadSIConfig.TimeOut * 1000;
               httpRequest.Method = "POST";
               httpRequest.ContentType = "text/xml";
               string xml;
               if (ConvertModelToSendMessage(sendObject, out xml, appendParams) < 0)  //入参
               {
                   ServiceLogManager.Write("入参报文：" + xml);
                   return -1;
               }

               ServiceLogManager.Write("医保入参报文：" + xml);

               byte[] requestData = Encoding.UTF8.GetBytes(xml);
               httpRequest.ContentLength = requestData.Length;
               var reqStream = httpRequest.GetRequestStream();
               reqStream.Write(requestData, 0, requestData.Length);
               reqStream.Flush();
               reqStream.Close();
 
               var responseResult = (HttpWebResponse)httpRequest.GetResponse();
               StreamReader streamReader = new StreamReader(responseResult.GetResponseStream(), Encoding.UTF8);
               string result = streamReader.ReadToEnd();
               ServiceLogManager.Write("医保出参报文：" + result);
               XmlDocument xmlDoc = new XmlDocument();
               xmlDoc.LoadXml(result);
               if (this.ConvertReciverMessageToModel(xmlDoc, ref reciverObject) < 0)  //出参
               {
                   if (this.ReturnCode == "-9")
                   {
                       this.ErrorMsg = "医保登录凭证过期，请重试。";
                       InitializeSiSession();
                   }
                   return -1;
               }
               return 1;
           }
           catch (Exception ex)
           {
               this.ErrorMsg = ex.ToString();
               ServiceLogManager.Write("医保系统错误：" + ex.Message.ToString());
               return -1;
           }
        }

        private int InitializeSiSession()
        {
            try
            {
                GDSI.Models.InParam.InParamSys0001 inParam = new GDSI.Models.InParam.InParamSys0001();
                inParam.Userid = FS.ZDWY.Internet.BP.SI.ReadSIConfig.UserID;
                inParam.Password = FS.ZDWY.Internet.BP.SI.ReadSIConfig.Password;

                #region 生成XML
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement input = doc.CreateElement("program");
                doc.AppendChild(input);

                AppendChildNode(doc, input, "function_id", inParam.Function_id);
                AppendChildNode(doc, input, "userid", inParam.Userid);
                AppendChildNode(doc, input, "password", inParam.Password);

                string xml = doc.InnerXml.ToString();

                ServiceLogManager.Write("医保入参报文：" + xml);
                #endregion

                HttpWebRequest httpRequest;
                httpRequest = (HttpWebRequest)WebRequest.Create(FS.ZDWY.Internet.BP.SI.ReadSIConfig.ServiceUrl);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "text/xml";
                httpRequest.Timeout = FS.ZDWY.Internet.BP.SI.ReadSIConfig.TimeOut * 1000;
                httpRequest.ReadWriteTimeout = FS.ZDWY.Internet.BP.SI.ReadSIConfig.TimeOut * 1000;

                byte[] requestData = Encoding.UTF8.GetBytes(xml);
                httpRequest.ContentLength = requestData.Length;
                var reqStream = httpRequest.GetRequestStream();
                reqStream.Write(requestData, 0, requestData.Length);
                reqStream.Flush();
                reqStream.Close();

                var responseResult = (HttpWebResponse)httpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(responseResult.GetResponseStream(), Encoding.UTF8);
                string result = streamReader.ReadToEnd();
                ServiceLogManager.Write("医保出参报文：" + result);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                string returnCode = GetValue(xmlDoc, "program/return_code");
                if (string.IsNullOrEmpty(returnCode))
                {
                    return -1;
                }
                if (returnCode == "1")
                {
                    FS.ZDWY.Internet.BP.SI.ReadSIConfig.SessionId = GetValue(xmlDoc, "program/session_id");
                    return 1;
                }
                else
                {
                    this.ErrorMsg = "医保错误提示：" + GetValue(xmlDoc, "program/return_code_message");
                    return -1;
                }
            }
            catch(Exception e)
            {
                this.ErrorMsg = e.Message.ToString();
                return -1;
            }

        }

        public static void AppendChildNode(XmlDocument doc, XmlElement parentNode, string nodeName, string nodeValue)
        {
            if (doc == null || parentNode == null || string.IsNullOrEmpty(nodeName))
            {
                return;
            }
            XmlElement exist = doc.GetElementById(parentNode.InnerText);
            if (exist == null)
            {
                XmlElement node = doc.CreateElement(nodeName);
                node.InnerText = nodeValue;
                parentNode.AppendChild(node);
            }
            else
            {
                XmlElement node = doc.CreateElement(nodeName);
                node.InnerText = nodeValue;
                exist.AppendChild(node);
            }
        }

        protected virtual string GetValue(XmlDocument xmlDoc, string xpath)
        {
            var node = xmlDoc.SelectSingleNode(xpath);
            if (node == null)
            {
                this.ErrorMsg = string.Format("取节点{0}的值失败", xpath);
                return null;
            }
            return node.InnerText;
        }

        protected decimal GetPrice(Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f)
        {
            if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug && f.Item.Qty > 400)//中草药
            {
                return Neusoft.FrameWork.Public.String.FormatNumber(System.Math.Abs(f.FT.TotCost * 10 / f.Item.Qty), 4);
            }
            else
            {
                return Neusoft.FrameWork.Public.String.FormatNumber(System.Math.Abs(f.FT.TotCost / f.Item.Qty), 4);
            }
        }

        protected decimal GetCount(Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f)
        {
            if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug && f.Item.Qty > 400)//中草药
            {
                return Neusoft.FrameWork.Public.String.FormatNumber(System.Math.Abs(f.Item.Qty / 10), 4);
            }
            else
            {
                return f.Item.Qty;
            }

        }

        /// <summary>
        /// 获取限制用药标识
        /// </summary>
        /// <param name="OrderID">医嘱流水号</param>
        /// <param name="OrderID">医嘱限制药集合</param>
        public string GetItemLimit(string OrderID, Hashtable ht)
        {
            if (ht.Contains(OrderID))
            {
                Neusoft.HISFC.Models.Order.Inpatient.OrderExtend obj = ht[OrderID] as Neusoft.HISFC.Models.Order.Inpatient.OrderExtend;
                if (obj.Extend3 == "1")
                {
                    return "1"; // 在限制药列表里
                }
                else
                {
                    return "0"; // 不在限制药列表里
                }
            }
            else
            {
                return "0"; // 不在限制药列表里
            }
        }
    }
}
