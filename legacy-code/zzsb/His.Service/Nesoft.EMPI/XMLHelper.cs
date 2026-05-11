using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Nesoft.EMPI
{
    public class XMLHelper
    {
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
