using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
namespace FS.ZDWY.Internet.SIWebService.Interface.Inpatient
{
    public class fyqdxx : AbstractService
    {
        public override string FunctionID
        {
            get
            {
                return "mcce_bizh120113";
            }
        }

        protected override int ConvertModelToSendMessage(out string xml, params object[] appendParams)
        {
            xml = "";
            try
            {
                string function_id = "";
                string akb020 = "";
                string aaz218 = "";
                string Operate = "";
                string Secfalg = "";
                string aac002 = "";
                string fromdate = "";
                if (appendParams.Length < 5)
                {
                    this.ErrorMsg = "入参不符合接口规范！";
                    return -1;
                }

                function_id = appendParams[0] as string;
                akb020 = appendParams[1] as string;
                aaz218 = appendParams[2] as string;
                Operate = appendParams[3] as string;
                Secfalg = appendParams[4] as string;
                aac002 = appendParams[5] as string;
                fromdate = appendParams[6] as string;

                if (!function_id.Equals(FunctionID))
                {
                    this.ErrorMsg = "[FunctionID]值不符合接口规范！";
                    return -1;
                }


                #region XML
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement input = doc.CreateElement("program");
                doc.AppendChild(input);

                AppendChildNode(doc, input, "session_id", FS.ZDWY.Internet.SIWebService.Interface.Comm.ReadSIConfig.SessionId);
                AppendChildNode(doc, input, "function_id", FunctionID);
                AppendChildNode(doc, input, "akb020", akb020);
                AppendChildNode(doc, input, "aaz218", aaz218);
                AppendChildNode(doc, input, "Operate", Operate);
                AppendChildNode(doc, input, "Secfalg", Secfalg);
                AppendChildNode(doc, input, "aac002", aac002);
                AppendChildNode(doc, input, "fromdate", fromdate);
                xml = doc.InnerXml.ToString();
                return 1;
                #endregion
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message.ToString();
                return -1;
            }
        }
    }
}