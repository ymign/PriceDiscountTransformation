using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Xml.Linq;

namespace Nesoft.EMPI.PushUndrug
{

    public class unDrugLogic
    {
        public string PushUndrugInfo(Neusoft.HISFC.Models.Fee.Item.Undrug info, string DOEVENT)
        {
            Nesoft.EMPI.undrug.WEBESPort Port = new Nesoft.EMPI.undrug.WEBESPort();

            XmlDocument xmlMessage = new XmlDocument();
            //XmlNode Node = xmlMessage.CreateXmlDeclaration("1.0", "utf-8", "");
            //xmlMessage.AppendChild(Node);
            XmlNode Root = xmlMessage.CreateElement("MSG");
            xmlMessage.AppendChild(Root);
            XmlNode Node1 = xmlMessage.CreateNode(XmlNodeType.Element, "ROW", null);
            CreateNode(xmlMessage, Node1, "MD_UNDRUG_CODE", info.ID);
            CreateNode(xmlMessage, Node1, "MD_UNDRUG_NAME", info.Name);
            CreateNode(xmlMessage, Node1, "MD_FEE_CODE", info.MinFee.ID);
            CreateNode(xmlMessage, Node1, "MD_SPELL_CODE", info.SpellCode);
            CreateNode(xmlMessage, Node1, "MD_EXEDEPT_CODE", info.ExecDept);
            CreateNode(xmlMessage, Node1, "MD_STOCK_UNIT", info.PriceUnit);
            CreateNode(xmlMessage, Node1, "MD_UNIT_PRICE", info.Price.ToString());
            string grade = string.Empty;
            if (info.Grade != null && info.Grade == "3")
            {
                grade = "1";
            }
            else
            {
                grade = "0";
            }
            CreateNode(xmlMessage, Node1, "MD_SPECIAL_FLAG2", grade);
            CreateNode(xmlMessage, Node1, "MD_SPECIAL_FLAG3", info.FTRate.EMCRate.ToString());
            CreateNode(xmlMessage, Node1, "MD_APPLICABILITYAREA", "0");
            CreateNode(xmlMessage, Node1, "MD_VALID_STATE", info.ValidState);
            CreateNode(xmlMessage, Node1, "MD_UNDRUG_FEE_CODE", info.UserCode);
            CreateNode(xmlMessage, Node1, "MD_UNDRUG_FEE_NAME", info.Name);
            CreateNode(xmlMessage, Node1, "MD_UNDRUG_EXT1", info.UnitFlag);//0明细 1组套
            CreateNode(xmlMessage, Node1, "MD_UNDRUG_EXT2", "");
            CreateNode(xmlMessage, Node1, "MD_UNDRUG_EXT3", "");
            CreateNode(xmlMessage, Node1, "MD_OPER_NAME", info.Oper.Name);
            CreateNode(xmlMessage, Node1, "MD_OPER_CODE", info.Oper.ID);
            CreateNode(xmlMessage, Node1, "MD_OPER_TIME", System.DateTime.Now.ToString());
            CreateNode(xmlMessage, Node1, "DOEVENT", DOEVENT);
            xmlMessage.DocumentElement.AppendChild(Node1);

            XmlDocument xmlDoc = new XmlDocument();
            //XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            //xmlDoc.AppendChild(node);
            XmlNode root = xmlDoc.CreateElement("MSG");
            xmlDoc.AppendChild(root);

            XmlNode node1 = xmlDoc.CreateNode(XmlNodeType.Element, "HEAD", null);
            CreateNode(xmlDoc, node1, "ID", "");
            CreateNode(xmlDoc, node1, "KEY", "1004");
            CreateNode(xmlDoc, node1, "SRC", "HIS");
            CreateNode(xmlDoc, node1, "FUN", "undrug_sync");
            CreateNode(xmlDoc, node1, "TIME", DateTime.Now.ToString());
            xmlDoc.DocumentElement.AppendChild(node1);
            XmlNode node2 = xmlDoc.CreateNode(XmlNodeType.Element, "BODY", null);

            CreateNode(xmlDoc, node2, "CONTENT", xmlMessage.InnerXml.ToString());
            xmlDoc.DocumentElement.AppendChild(node2);
            xmlDoc.InnerXml.ToString().Replace("&lt;", "<");
            xmlDoc.InnerXml.ToString().Replace("&gt;", ">");
            //xmlDoc.InnerXml.ToString().Replace("<CONTENT>", "<CONTENT><![CDATA[");
            //xmlDoc.InnerXml.ToString().Replace("</CONTENT>", "]></CONTENT>");
            Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("undrug", xmlDoc.InnerXml.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<CONTENT>", "<CONTENT><![CDATA[").Replace("</CONTENT>", "]></CONTENT>"));
            string ss = Port.invokeLogic(xmlDoc.InnerXml.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<CONTENT>", "<CONTENT><![CDATA[").Replace("</CONTENT>", "]]></CONTENT>"));
            Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("undrug", ss);
            return ss;
        }
        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

    }
}
