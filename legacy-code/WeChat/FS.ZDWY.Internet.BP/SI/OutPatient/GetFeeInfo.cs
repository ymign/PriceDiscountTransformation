using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace FS.ZDWY.Internet.BP.SI.OutPatient
{
    public class GetFeeInfo : AbstractService<string, string>
    {
        public override string FunctionID
        {
            get { return "bizh110102"; }
        }

        protected override int ConvertModelToSendMessage(string str, out string xml, params object[] appendParams)
        {
            xml = "";
            string bak006 = "";
            bak006 = appendParams[0] as string;
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement input = doc.CreateElement("program");
            doc.AppendChild(input);

            AppendChildNode(doc, input, "function_id", FunctionID);
            AppendChildNode(doc, input, "session_id", FS.ZDWY.Internet.BP.SI.ReadSIConfig.SessionId);
            AppendChildNode(doc, input, "bka895", "aaz218");
            AppendChildNode(doc, input, "bka896", str);
            AppendChildNode(doc, input, "akb020", FS.ZDWY.Internet.BP.SI.ReadSIConfig.HospitalCode);
            AppendChildNode(doc, input, "bka006", string.IsNullOrEmpty(bak006) ? "110" : bak006);
            xml = doc.InnerXml.ToString();
            return 1;
        }

        protected override int ConvertReciverMessageToModel(XmlDocument doc, ref string rrro)
        {
             try
            {
                XmlNode node = null;
                node = doc.SelectSingleNode("program/return_code");
                this.ReturnCode = node == null ? string.Empty : node.InnerText.ToString();
                node = doc.SelectSingleNode("program/return_code_message");
                this.ErrorMsg = node == null ? string.Empty : node.InnerText.ToString();
                this.ErrorMsg = node == null ? string.Empty : node.InnerText.ToString();

                if (ReturnCode != "1")
                {
                    return -1;
                }
                return 1;
            }
             catch (Exception e)
             {
                 this.ErrorMsg = e.Message.ToString();
                 return -1;
             }
        }
    }
}
