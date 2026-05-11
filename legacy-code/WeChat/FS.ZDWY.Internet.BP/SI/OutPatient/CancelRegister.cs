using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FS.ZDWY.Internet.BP.SI.OutPatient
{
    public class CancelRegister : AbstractService<string, GDSI.Models.OutParam.OutParamBizh110106>
    {
        public override string FunctionID
        {
            get { return "bizh110106"; }
        }

        protected override int ConvertModelToSendMessage(string t, out string xml, params object[] appendParams)
        {
            xml = "";
            try
            {
                //appendParams(){ ic_reg_permit,aaz218}
                //持卡就诊登记许可号
                string icpermit = "";
                icpermit = appendParams[0] as string;
                string RegNo = appendParams[1] as string;

                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement input = doc.CreateElement("program");
                doc.AppendChild(input);
                AppendChildNode(doc, input, "function_id", FunctionID);
                AppendChildNode(doc, input, "ic_reg_permit", icpermit);
                AppendChildNode(doc, input, "session_id", FS.ZDWY.Internet.BP.SI.ReadSIConfig.SessionId);
                AppendChildNode(doc, input, "akb020", FS.ZDWY.Internet.BP.SI.ReadSIConfig.HospitalCode);
                AppendChildNode(doc, input, "aaz218", RegNo);

                AppendChildNode(doc, input, "bka014", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code);
                AppendChildNode(doc, input, "bka015", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Name);
                xml = doc.InnerXml.ToString();
                return 1;
            }
            catch(Exception e)
            {
                base.ErrorMsg = e.Message.ToString();
                return -1;
            }
        }

        protected override int ConvertReciverMessageToModel(System.Xml.XmlDocument doc, ref GDSI.Models.OutParam.OutParamBizh110106 outParam)
        {
            XmlNode node = null;
            node = doc.SelectSingleNode("program/return_code");
            outParam.Return_code = node == null ? string.Empty : node.InnerText.ToString();
            node = doc.SelectSingleNode("program/return_code_message");
            outParam.Return_code_message = node == null ? string.Empty : node.InnerText.ToString();
            ReturnCode = outParam.Return_code;
            if (ReturnCode!="1")
            {
                base.ErrorMsg = outParam.Return_code_message;
                return -1;
            }
            return 1;
        }
    }
}
