using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace FS.ZDWY.Internet.SIWebService.Interface.Inpatient
{
    public class rydjxx : AbstractService
    {
        public override string FunctionID
        {
            get
            {
                return "bizh120102";
            }
        }

        protected override int ConvertModelToSendMessage(out string xml, params object[] appendParams)
        {
            xml = "";
            try
            {
                string function_id = "";
                string bka895 = "";
                string bka896 = "";
                string akb020 = "";
                string aka130 = "";
                string bka891 = "";
                string aae030 = "";
                string aae031 = "";

                if (appendParams.Length < 8)
                {
                    this.ErrorMsg = "入参不符合接口规范！";
                    return -1;
                }

                function_id = appendParams[0] as string;
                bka895 = appendParams[1] as string;
                bka896 = appendParams[2] as string;
                akb020 = appendParams[3] as string;
                aka130 = appendParams[4] as string;
                bka891 = appendParams[5] as string;
                aae030 = appendParams[6] as string;
                aae031 = appendParams[7] as string;

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
                AppendChildNode(doc, input, "bka895", bka895);
                AppendChildNode(doc, input, "bka896", bka896);
                AppendChildNode(doc, input, "akb020", akb020);
                AppendChildNode(doc, input, "aka130", aka130);
                AppendChildNode(doc, input, "bka891", bka891);
                AppendChildNode(doc, input, "aae030", aae030);
                AppendChildNode(doc, input, "aae031", aae031);

                xml = doc.InnerXml.ToString();
                return 1;
                #endregion
            }
            catch(Exception e)
            {
                ErrorMsg = e.Message.ToString();
                return -1;
            }
        }
    }
}