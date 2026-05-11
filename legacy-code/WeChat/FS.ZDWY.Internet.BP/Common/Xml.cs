using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FS.ZDWY.Internet.BP.Common
{
    public static class Xml
    {
        /// <summary>
        /// 往项目里包含文件
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="aspxCsFileList"></param>
        public static void WriteCsproj(string csprojPath, string filePath, bool isContent)
        {
            //1。初始化一个xml实例
            XmlDocument xmlDoc = new XmlDocument();
            //2。导入指定xml文件
            xmlDoc.Load(csprojPath);
            //3。查找节点Project
            XmlNodeList root_childlist = xmlDoc.ChildNodes;
            XmlNode root_Project = null; ;
            foreach (XmlNode xn in root_childlist)
            {
                if (xn.Name == "Project")
                {
                    root_Project = xn;
                    break;
                }
            }
            //4。查找Project节点下的ItemGroup节点，确定内容节点Content和编译节点Compile所在的ItemGroup
            XmlNodeList childlist_Project = root_Project.ChildNodes;//根节点的字节点
            foreach (XmlNode xn in childlist_Project)
            {
                if (xn.Name == "ItemGroup")
                {
                    if (isContent)
                    {
                        if (xn.FirstChild.Name == "Content")
                        {
                            XmlElement xe_Content = xmlDoc.CreateElement("Content", xmlDoc.DocumentElement.NamespaceURI);//创建一个节点
                            xe_Content.SetAttribute("Include", filePath);//设置该节点genre属性
                            xn.AppendChild(xe_Content);
                            break;
                        }
                    }
                    else
                    {
                        if (xn.FirstChild.Name == "Compile")
                        {
                            XmlElement xe_Compile = xmlDoc.CreateElement("Compile", xmlDoc.DocumentElement.NamespaceURI);//创建一个节点
                            xe_Compile.SetAttribute("Include", filePath);//设置该节点genre属性
                            xn.AppendChild(xe_Compile);
                            break;
                        }
                    }
                }
            }

            //5。保存修改后的文件
            xmlDoc.Save(csprojPath);
        }

        /// <summary>
        /// 格式化XML
        /// </summary>
        /// <param name="XMLstring"></param>
        /// <returns></returns>
        public static string FormatXML(string XMLstring)
        {
            //校验是否是XML报文
            //if (!XMLstring.Contains("<?xml version")) return XMLstring;
            XmlDocument xmlDocument = GetXmlDocument(XMLstring);
            return ConvertXmlDocumentTostring(xmlDocument);
        }
        public static string ConvertXmlDocumentTostring(XmlDocument xmlDocument)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(memoryStream, null)
            {
                Formatting = Formatting.Indented//缩进
            };
            xmlDocument.Save(writer);
            StreamReader streamReader = new StreamReader(memoryStream);
            memoryStream.Position = 0;
            string xmlString = streamReader.ReadToEnd();
            streamReader.Close();
            memoryStream.Close();
            return xmlString;
        }
        public static XmlDocument GetXmlDocument(string xmlString)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            return document;
        }

        public static string ObjToXml(object obj)
        {
            using (MemoryStream Stream = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(obj.GetType());
                xml.Serialize(Stream, obj);
                Stream.Position = 0;
                StreamReader sr = new StreamReader(Stream);
                return sr.ReadToEnd();
            }

        }

        public static string Serialize<T>(T obj)
        {
            return Serialize<T>(obj, Encoding.UTF8);
        }
        /// <summary> 
        /// 实体对象序列化成xml字符串 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="obj"></param> 
        /// <returns></returns> 
        public static string Serialize<T>(T obj, Encoding encoding)
        {
            try
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }
                var ser = new XmlSerializer(obj.GetType());
                using (var ms = new MemoryStream())
                {
                    using (var writer = new XmlTextWriter(ms, encoding))
                    {
                        writer.Formatting = Formatting.Indented;
                        ser.Serialize(writer, obj);
                    }
                    var xml = encoding.GetString(ms.ToArray());
                    xml = xml.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
                    xml = xml.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
                    xml = Regex.Replace(xml, @"\s{2}", "");
                    xml = Regex.Replace(xml, @"\s{1}/>", "/>");
                    return xml;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary> 
        /// 反序列化xml字符为对象，默认为Utf-8编码 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="xml"></param> 
        /// <returns></returns> 
        public static T DeSerialize<T>(string xml) where T : new()
        {
            return DeSerialize<T>(xml, Encoding.UTF8);
        }
        /// <summary> 
        /// 反序列化xml字符为对象 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="xml"></param> 
        /// <param name="encoding"></param> 
        /// <returns></returns> 
        public static T DeSerialize<T>(string xml, Encoding encoding) where T : new()
        {
            try
            {
                var mySerializer = new XmlSerializer(typeof(T));
                using (var ms = new MemoryStream(encoding.GetBytes(xml)))
                {
                    using (var sr = new StreamReader(ms, encoding))
                    {
                        return (T)mySerializer.Deserialize(sr);
                    }
                }
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
        static System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("[&<>'\"]");

        public static string GetEscapingContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return "";
            }
            if (re.IsMatch(content))
            {
                content = content.Replace("&", "&amp;");
                content = content.Replace("<", "&lt;");
                content = content.Replace(">", "&gt;");
                content = content.Replace("'", "&apos;");
                content = content.Replace("\"", "&quot;");
            }
            return content;
        }

        public static T XmlDeSerializeToModel<T>(string xml, string nodeName, ref string errMsg) where T : class, new()
        {
            try
            {
                T t = new T();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlNodeList xmlNodeList = xmlDoc.SelectNodes(nodeName);
                foreach (XmlNode xnls in xmlNodeList)
                {

                    foreach (XmlNode xnl in xnls.ChildNodes)
                    {
                        PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var p in propertyInfos)
                        {
                            if (xnl.Name.ToUpper() == p.Name.ToUpper())
                            {
                                var Name = p.PropertyType.Name;
                                switch (Name.ToLower())
                                {
                                    case "string":
                                        p.SetValue(t, xnl.InnerText.Trim(), null);
                                        break;
                                    case "char":
                                        p.SetValue(t, xnl.InnerText.Trim(), null);
                                        break;
                                    case "int":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "int32":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "unint":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "byte":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "sbyte":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "short":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "ushort":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "long":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "ulong":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "float":
                                        p.SetValue(t, NConvert.ToDecimal(xnl.InnerText.Trim()), null);
                                        break;
                                    case "decimal":
                                        p.SetValue(t, NConvert.ToDecimal(xnl.InnerText.Trim()), null);
                                        break;
                                    case "double":
                                        p.SetValue(t, NConvert.ToDouble(xnl.InnerText.Trim()), null);
                                        break;
                                    case "bool":
                                        p.SetValue(t, NConvert.ToBoolean(xnl.InnerText.Trim()), null);
                                        break;
                                    case "datetime":
                                        p.SetValue(t, NConvert.ToDateTime(xnl.InnerText.Trim()), null);
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            }
                        }
                    }
                    break;
                }
                return t;
            }
            catch (Exception ex)
            {

                return default(T);

            }

        }

        public static List<T> XmlDeSerializeToList<T>(string xml, string nodeName, ref string errMsg) where T : class, new()
        {
            try
            {
                List<T> tList = new List<T>();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlNodeList xmlNodeList = xmlDoc.SelectNodes(nodeName);
                foreach (XmlNode xnls in xmlNodeList)
                {
                    T t = new T();
                    foreach (XmlNode xnl in xnls.ChildNodes)
                    {
                        PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var p in propertyInfos)
                        {
                            if (xnl.Name.ToUpper() == p.Name.ToUpper())
                            {
                                var Name = p.PropertyType.Name;
                                switch (Name.ToLower())
                                {
                                    case "string":
                                        p.SetValue(t, xnl.InnerText.Trim(), null);
                                        break;
                                    case "char":
                                        p.SetValue(t, xnl.InnerText.Trim(), null);
                                        break;
                                    case "int":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "int32":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "unint":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "byte":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "sbyte":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "short":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "ushort":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "long":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "ulong":
                                        p.SetValue(t, NConvert.ToInt32(xnl.InnerText.Trim()), null);
                                        break;
                                    case "float":
                                        p.SetValue(t, NConvert.ToDecimal(xnl.InnerText.Trim()), null);
                                        break;
                                    case "decimal":
                                        p.SetValue(t, NConvert.ToDecimal(xnl.InnerText.Trim()), null);
                                        break;
                                    case "double":
                                        p.SetValue(t, NConvert.ToDouble(xnl.InnerText.Trim()), null);
                                        break;
                                    case "bool":
                                        p.SetValue(t, NConvert.ToBoolean(xnl.InnerText.Trim()), null);
                                        break;
                                    case "datetime":
                                        p.SetValue(t, NConvert.ToDateTime(xnl.InnerText.Trim()), null);
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            }
                        }
                    }
                    tList.Add(t);
                }
                return tList;
            }
            catch (Exception ex)
            {

                return default(List<T>);
            }

        }
    }
}
