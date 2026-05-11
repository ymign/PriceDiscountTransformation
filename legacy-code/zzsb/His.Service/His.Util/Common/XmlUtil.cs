using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace His.Util.Common
{
    /// <summary>
    /// DESC:SOAP 消息序列化公共类
    /// Creater;杨明
    /// Version：1.0.0.1
    /// Date:2015-05-12
    /// Alter:2015-06-01
    /// </summary>
  public  class XmlUtil
    {
        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {

                return null;
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
        #endregion

        #region 序列化XML文件
        /// <summary>
        /// 序列化XML文件
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            //创建序列化对象
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();
            return str;
        }
        #endregion

        #region 将XML转换为DATATABLE
        /// <summary>
        /// 将XML转换为DATATABLE
        /// </summary>
        /// <param name="FileURL"></param>
        /// <returns></returns>
        public static DataTable XmlAnalysisArray( )
        {
            try
            {
                string FileURL = System.Configuration.ConfigurationManager.AppSettings["XmlFileUrl"].ToString();
                DataSet ds = new DataSet();
                ds.ReadXml(FileURL);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
               // System.Web.HttpContext.Current.Response.Write(ex.Message.ToString());
                return null;
            }
        }
        /// <summary>
        /// 将XML转换为DATATABLE
        /// </summary>
        /// <param name="FileURL"></param>
        /// <returns></returns>
        public static DataTable XmlAnalysisArray(string FileURL)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(FileURL);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
              //  System.Web.HttpContext.Current.Response.Write(ex.Message.ToString());
                return null;
            }
        }
        #endregion

        #region 获取对应XML节点的值
        /// <summary>
        /// 摘要:获取对应XML节点的值
        /// </summary>
        /// <param name="stringRoot">XML节点的标记</param>
        /// <returns>返回获取对应XML节点的值</returns>
        public static string XmlAnalysis(string stringRoot, string xml)
        {
            if (stringRoot.Equals("") == false)
            {
                try
                {
                    XmlDocument XmlLoad = new XmlDocument();
                    XmlLoad.LoadXml(xml);
                    return XmlLoad.DocumentElement.SelectSingleNode(stringRoot).InnerXml.Trim();
                }
                catch (Exception ex)
                {

                }
            }
            return "";
        }
        #endregion


        //add by allan  2016-07-26    XML的序列化以及反序列化

        #region XML序列化为字符串
        /// <summary>
        /// XML序列化为字符串
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="StrNamespace">命名空间</param>
        /// <param name="Strprefix">前缀</param>
        /// <returns></returns>
        public static string XmlSerialize<T>(T obj, string StrNamespace, string Strprefix)
        {
            string xmlString = string.Empty;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), StrNamespace);
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                xsn.Add(Strprefix, StrNamespace);
                xmlSerializer.Serialize(ms, obj, xsn);
                xmlString = Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
            }
            return xmlString;
        }
        #endregion

        #region 反序列化为对象
        /// <summary>
        /// XMLString反序列化成对象
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="strXml">XML字符串</param>
        /// <param name="StrNamespace">命名空间</param>
        /// <param name="Strprefix">前缀</param>
        /// <returns>对象</returns>
        public static T XmlDeserialize<T>(string strXml, string StrNamespace, string Strprefix)
        {
            T t = default(T);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), StrNamespace);
            using (Stream xmlStream = new MemoryStream(Encoding.GetEncoding("UTF-8").GetBytes(strXml)))
            {
                XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                xsn.Add(Strprefix, StrNamespace);
                XmlReader xmlReader = XmlReader.Create(xmlStream);
                Object obj = xmlSerializer.Deserialize(xmlReader);
                t = (T)obj;
            }
            return t;
        }
        #endregion

    }
}
