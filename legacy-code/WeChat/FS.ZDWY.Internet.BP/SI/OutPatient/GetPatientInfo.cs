using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace FS.ZDWY.Internet.BP.SI.OutPatient
{
    public class GetPatientInfo : AbstractService<FS.ZDWY.Internet.Models.Views.OutPatient.ComPatient, GDSI.Models.OutParam.OutParamBizh110001>
    {
        private Neusoft.HISFC.BizLogic.Manager.Constant consMgr = new Neusoft.HISFC.BizLogic.Manager.Constant();
        public override string FunctionID
        {
            get { return "bizh110001"; }
        }

        protected int ConvertModelToSendMessage(Neusoft.HISFC.Models.Registration.Register r, out string xml, params object[] appendParams)
        {
            xml = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement input = doc.CreateElement("program");
                doc.AppendChild(input);

                AppendChildNode(doc, input, "function_id", FunctionID);
                AppendChildNode(doc, input, "session_id", FS.ZDWY.Internet.BP.SI.ReadSIConfig.SessionId);

                string icpermit = appendParams[0] as string;
                string Bka006 = "";//待遇类型
                string Aka130 = "";//业务类型
                string aae140 = "";//参保类型
                //r.SSN = r.IDCard;
                //if (string.IsNullOrEmpty(r.SSN))
                //{
                //    base.ErrorMsg = "患者社会保障号或社保卡号不能为空!";
                //    return -1;
                //}
                //else 
                if (string.IsNullOrEmpty(r.SSN))//医疗保险号
                {
                    r.SSN = r.IDCard;
                }

                if (Regex.IsMatch(r.SSN, @"^[a-zA-Z]{3}[0].*$"))//匹配外籍身份证号
                {
                    AppendChildNode(doc, input, "bka895", "aac002");
                }
                else if (Regex.IsMatch(r.SSN, @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$") && r.SSN.Length != 16)//国内身份证
                {
                    AppendChildNode(doc, input, "bka895", "aac002");
                }
                else
                {
                    if (r.SSN.Length == 16)
                    {
                        AppendChildNode(doc, input, "bka895", "aac001");
                    }
                    else
                    {
                        AppendChildNode(doc, input, "bka100", "aac001");
                    }
                }
                //Aka130 = consMgr.GetConstant("PactToMedType", r.Pact.ID).Memo;//业务类型

                if (Aka130 == "13")//门慢
                {
                    Bka006 = "131";//默认传131，取人员信息接口会返回患者的病种信息
                }
                else//除了门慢，其他的业务类型挂号都走普通门诊
                {
                    Aka130 = "11";
                    Bka006 = "110";
                }

                AppendChildNode(doc, input, "bka896", r.SSN);
                AppendChildNode(doc, input, "akb020", FS.ZDWY.Internet.BP.SI.ReadSIConfig.HospitalCode);
                AppendChildNode(doc, input, "bka006", Bka006);
                AppendChildNode(doc, input, "ic_reg_permit", icpermit);
                xml = doc.InnerXml.ToString();
                return 1;
            }
            catch(Exception e)
            {
                base.ErrorMsg = e.Message.ToString();
                return -1;
            }
        }

        protected override int ConvertModelToSendMessage(FS.ZDWY.Internet.Models.Views.OutPatient.ComPatient r, out string xml, params object[] appendParams)
        {
            xml = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement input = doc.CreateElement("program");
                doc.AppendChild(input);

                AppendChildNode(doc, input, "function_id", FunctionID);
                AppendChildNode(doc, input, "session_id", FS.ZDWY.Internet.BP.SI.ReadSIConfig.SessionId);

                string icpermit = appendParams[0] as string;
                string Bka006 = "";//待遇类型
                string Aka130 = "";//业务类型
                string aae140 = "";//参保类型
                if (appendParams.Length >= 3)
                {
                    Aka130 = appendParams[1] as string;
                    Bka006 = appendParams[2] as string;
                }
                else
                {
                    //固定未普通挂号减免
                    Aka130 = "11";
                    Bka006 = "110";
                }

                //r.SSN = r.IDCard;
                //if (string.IsNullOrEmpty(r.SSN))
                //{
                //    base.ErrorMsg = "患者社会保障号或社保卡号不能为空!";
                //    return -1;
                //}
                //else 
                if (string.IsNullOrEmpty(r.SSN))//医疗保险号
                {
                    r.SSN = r.IDCard;
                }

                if (Regex.IsMatch(r.SSN, @"^[a-zA-Z]{3}[0].*$"))//匹配外籍身份证号
                {
                    AppendChildNode(doc, input, "bka895", "aac002");
                }
                else if (Regex.IsMatch(r.SSN, @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$") && r.SSN.Length != 16)//国内身份证
                {
                    AppendChildNode(doc, input, "bka895", "aac002");
                }
                else
                {
                    if (r.SSN.Length == 16)
                    {
                        AppendChildNode(doc, input, "bka895", "aac001");
                    }
                    else
                    {
                        AppendChildNode(doc, input, "bka100", "aac001");
                    }
                }
                //Aka130 = consMgr.GetConstant("PactToMedType", r.Pact.ID).Memo;//业务类型

                if (Aka130 == "13")//门慢
                {
                    Bka006 = "131";//默认传131，取人员信息接口会返回患者的病种信息
                }
                else//除了门慢，其他的业务类型挂号都走普通门诊
                {
                    Aka130 = "11";
                    Bka006 = "110";
                }

                AppendChildNode(doc, input, "bka896", r.SSN);
                AppendChildNode(doc, input, "akb020", FS.ZDWY.Internet.BP.SI.ReadSIConfig.HospitalCode);
                AppendChildNode(doc, input, "bka006", Bka006);
                AppendChildNode(doc, input, "ic_reg_permit", icpermit);
                xml = doc.InnerXml.ToString();
                return 1;
            }
            catch (Exception e)
            {
                base.ErrorMsg = e.Message.ToString();
                return -1;
            }
        }

        protected override int ConvertReciverMessageToModel(XmlDocument doc, ref GDSI.Models.OutParam.OutParamBizh110001 reciverObject)
        {
            reciverObject = new GDSI.Models.OutParam.OutParamBizh110001();
            try
            {
                XmlNode node = null;
                node = doc.SelectSingleNode("program/return_code");
                reciverObject.Return_code = node == null ? string.Empty : node.InnerText.ToString();
                this.ReturnCode = node == null ? string.Empty : node.InnerText.ToString();
                node = doc.SelectSingleNode("program/return_code_message");
                reciverObject.Return_code_message = node == null ? string.Empty : node.InnerText.ToString();
                this.ErrorMsg = node == null ? string.Empty : node.InnerText.ToString();

                if (ReturnCode != "1")
                {
                    return -1;
                }

                XmlNodeList nodeList = null;

                nodeList = doc.SelectNodes("program/personinfo/row");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode itemNode in nodeList)
                    {
                        GDSI.Models.Personinfo personinfo = new GDSI.Models.Personinfo();

                        node = itemNode.SelectSingleNode("aac001");
                        personinfo.Aac001 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aac003");
                        personinfo.Aac003 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aac004");
                        personinfo.Aac004 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("bka004");
                        personinfo.Bka004 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aac013");
                        personinfo.Aac013 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("bka005");
                        personinfo.Bka005 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aac002");
                        personinfo.Aac002 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aae005");
                        personinfo.Aae005 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aac006");
                        personinfo.Aac006 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("baa027");
                        personinfo.Baa027 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aab001");
                        personinfo.Aab001 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("bka008");
                        personinfo.Bka008 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aae140");
                        personinfo.Aae140 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("bka042");
                        personinfo.Bka042 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aaz267");
                        personinfo.Aaz267 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aae030");
                        personinfo.Aae030 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aae031");
                        personinfo.Aae031 = node == null ? string.Empty : node.InnerText.ToString();

                        reciverObject.Personinfos.Add(personinfo);
                    }
                }

                nodeList = doc.SelectNodes("program/freezeinfo/row");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode itemNode in nodeList)
                    {
                        GDSI.Models.Freezeinfo freezeinfo = new GDSI.Models.Freezeinfo();

                        node = itemNode.SelectSingleNode("aaa157");
                        freezeinfo.Aaa157 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aad006");
                        freezeinfo.Aad006 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aac031");
                        freezeinfo.Aac031 = node == null ? string.Empty : node.InnerText.ToString();

                        reciverObject.Freezeinfos.Add(freezeinfo);
                    }
                }

                nodeList = doc.SelectNodes("program/clinicapplyinfo/row");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode itemNode in nodeList)
                    {
                        GDSI.Models.Clinicapplyinfo clinicapplyinfo = new GDSI.Models.Clinicapplyinfo();

                        node = itemNode.SelectSingleNode("aaz267");
                        clinicapplyinfo.Aaz267 = node == null ? string.Empty : node.InnerText.ToString();
                        //以下2018-04-12 文档变动没用了 
                        node = itemNode.SelectSingleNode("bka006");
                        clinicapplyinfo.Bka006 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("bka345");
                        clinicapplyinfo.Bka345 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aka130");
                        clinicapplyinfo.Aka130 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("bka026");
                        clinicapplyinfo.Bka026 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aka121");
                        clinicapplyinfo.Aka121 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aae030");
                        clinicapplyinfo.Aae030 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aae031");
                        clinicapplyinfo.Aae031 = node == null ? string.Empty : node.InnerText.ToString();

                        reciverObject.Clinicapplyinfos.Add(clinicapplyinfo);
                    }
                }

                #region 门慢
                nodeList = doc.SelectNodes("program/spinfo/row");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode itemNode in nodeList)
                    {
                        GDSI.Models.Spinfo spinfo = new GDSI.Models.Spinfo();

                        node = itemNode.SelectSingleNode("aaz267");
                        spinfo.Aaz267 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("bka006");
                        spinfo.Bka006 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("bka345");
                        spinfo.Bka345 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aka130");
                        spinfo.Aka130 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("bka026");
                        spinfo.Bka026 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aka121");
                        spinfo.Aka121 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aae030");
                        spinfo.Aae030 = node == null ? string.Empty : node.InnerText.ToString();
                        node = itemNode.SelectSingleNode("aae031");
                        spinfo.Aae031 = node == null ? string.Empty : node.InnerText.ToString();


                        reciverObject.Spinfos.Add(spinfo);
                    }
                }
                #endregion

                #region 工伤、生育
                //nodeList = doc.SelectNodes("program/injuryorbirthinfo/row");
                //if (nodeList != null && nodeList.Count > 0)
                //{
                //    foreach (XmlNode itemNode in nodeList)
                //    {
                //        GDSI.Models.Injuryorbirthinfo injuryorbirthinfo = new GDSI.Models.Injuryorbirthinfo();

                //        node = itemNode.SelectSingleNode("bka042");
                //        injuryorbirthinfo.Bka042 = node == null ? string.Empty : node.InnerText.ToString();
                //        node = itemNode.SelectSingleNode("aae030");
                //        injuryorbirthinfo.Aae030 = node == null ? string.Empty : node.InnerText.ToString();
                //        node = itemNode.SelectSingleNode("aae031");
                //        injuryorbirthinfo.Aae031 = node == null ? string.Empty : node.InnerText.ToString();

                //        outParam.Injuryorbirthinfos.Add(injuryorbirthinfo);
                //    }
                //}
                #endregion
                return 1;
            }
            catch(Exception e)
            {
                this.ErrorMsg = e.Message.ToString();
                return -1;
            }
        } 
    }
}
