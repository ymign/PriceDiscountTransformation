using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections;
using DataBaseHelp;

namespace His.Business.Helper
{
    public class Function
    {
        private const string HospitalId = "";

        private const string ApplicationId = "";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string GetRequest(DataSet ds)
        {
            string xmlRes = string.Empty;

            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ds.GetXml());
            XmlNode node = xmlDoc.FirstChild;
            string tranId = DateTime.Now.ToString("yyyyMMdd") + "_" + Guid.NewGuid().ToString().Substring(2, 6);
            writer.WriteStartElement("Request");
            writer.WriteElementString("ApplicationId", "8004");
            writer.WriteElementString("HospitalId", "Y3S0006");


            writer.WriteElementString("TransactionId", tranId);

            writer.WriteStartElement(node.Name);
            foreach (XmlNode item in node.ChildNodes)
            {
                writer.WriteStartElement(item.Name);
                foreach (XmlNode subItem in item.ChildNodes)
                {
                    writer.WriteElementString(subItem.Name, subItem.InnerText);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.Flush();

            StreamReader sr = new StreamReader(ms, System.Text.Encoding.UTF8);
            ms.Position = 0;
            xmlRes = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            writer.Close();
            return xmlRes;
        }

        public static string GetXmlFromDataSet(DataSet ds)
        {
            return ds.GetXml();
        }


    }
}
