using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FS.ZDWY.Internet.BP.SI.OutPatient
{
    public class CancelFee : AbstractService<Neusoft.HISFC.Models.Registration.Register, string>
    {
        public override string FunctionID
        {
            get { return "bizh110105"; }
        }

        protected override int ConvertModelToSendMessage(Neusoft.HISFC.Models.Registration.Register reg, out string xml, params object[] appendParams)
        {
            xml = "";
            try
            {
                string icpermit = appendParams[0] as string;
                string RegLevel = appendParams[1] as string;
                string Doctid = appendParams[2] as string;
                ArrayList al = new ArrayList();
                if (appendParams.Length > 3)
                {
                    al = appendParams[3] as ArrayList;
                }

                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement input = doc.CreateElement("program");
                doc.AppendChild(input);

                AppendChildNode(doc, input, "function_id", FunctionID);
                AppendChildNode(doc, input, "session_id", FS.ZDWY.Internet.BP.SI.ReadSIConfig.SessionId);
                AppendChildNode(doc, input, "akb020", FS.ZDWY.Internet.BP.SI.ReadSIConfig.HospitalCode);

                AppendChildNode(doc, input, "aaz218", reg.SIMainInfo.RegNo);
                AppendChildNode(doc, input, "bka014", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code);
                AppendChildNode(doc, input, "bka015", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Name);
                AppendChildNode(doc, input, "save_flag", "1");
                AppendChildNode(doc, input, "bka893", "1");
                AppendChildNode(doc, input, "ic_reg_permit", icpermit);

                FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new BP.OutPatient.Register.Manager();
                Hashtable htLimit = mgr.GetItemLimitList(reg.ID);
                string regfee = "";
                int ret = mgr.getRegItemCode(RegLevel, ref regfee);
                if (ret < 0)
                {
                    this.ErrorMsg = "调用门诊挂号接口(bizh110104)异常。获取挂号费失败!";
                    return -1;
                }
                Neusoft.HISFC.BizProcess.Integrate.Fee feeMgr = new Neusoft.HISFC.BizProcess.Integrate.Fee();
                Neusoft.HISFC.Models.Fee.Item.Undrug undrg = feeMgr.GetItem(regfee);
                Neusoft.HISFC.BizLogic.Manager.Person personMgr = new Neusoft.HISFC.BizLogic.Manager.Person();
                Neusoft.HISFC.Models.Base.Employee doctor = personMgr.GetPersonByID(Doctid);//获取开立医生信息

                if (al != null && al.Count > 0)
                {
                    XmlElement feeinfo = doc.CreateElement("feeinfo");
                    input.AppendChild(feeinfo);
                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItem in al)
                    {
                        XmlElement row = doc.CreateElement("row");
                        feeinfo.AppendChild(row);

                        if (feeItem == null)
                        {
                            continue;
                        }
                        decimal unitPrice = this.GetPrice(feeItem);
                        decimal Count = this.GetCount(feeItem);
                        doctor = personMgr.GetPersonByID(feeItem.RecipeOper.ID);//获取开立医生信息

                        AppendChildNode(doc, row, "aka063", feeItem.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug ? "1" : "2");
                        AppendChildNode(doc, row, "ake005", feeItem.Item.ID);
                        AppendChildNode(doc, row, "ake006", feeItem.Item.Name);
                        AppendChildNode(doc, row, "bka052", string.Empty);
                        AppendChildNode(doc, row, "bka053", string.Empty);
                        AppendChildNode(doc, row, "bka054", feeItem.Item.Specs);
                        AppendChildNode(doc, row, "bka051", personMgr.GetSysDate("yyyyMMdd"));
                        AppendChildNode(doc, row, "bka055", string.Empty);
                        AppendChildNode(doc, row, "bka056", unitPrice.ToString("F4"));
                        AppendChildNode(doc, row, "bka057", Count.ToString("F4"));
                        AppendChildNode(doc, row, "bka058", feeItem.FT.OwnCost.ToString("F2"));
                        AppendChildNode(doc, row, "bka070", feeItem.Order.ID);
                        AppendChildNode(doc, row, "bka074", doctor.IDCard);
                        AppendChildNode(doc, row, "bka075", doctor.Name);
                        AppendChildNode(doc, row, "bka071", feeItem.Order.ID);
                        AppendChildNode(doc, row, "aka036", GetItemLimit(feeItem.Order.ID, htLimit));
                    }
                }
                else
                {
                    XmlElement feeinfo = doc.CreateElement("feeinfo");
                    input.AppendChild(feeinfo);

                    XmlElement row = doc.CreateElement("row");
                    feeinfo.AppendChild(row);

                    AppendChildNode(doc, row, "aka063", "21");
                    AppendChildNode(doc, row, "ake005", undrg.ID);
                    AppendChildNode(doc, row, "ake006", undrg.Name);
                    AppendChildNode(doc, row, "bka052", "");
                    AppendChildNode(doc, row, "bka053", "");
                    AppendChildNode(doc, row, "bka054", "");
                    AppendChildNode(doc, row, "bka051", feeMgr.GetDateTimeFromSysDateTime().Date.ToString("yyyyMMdd"));
                    AppendChildNode(doc, row, "bka055", "次");
                    AppendChildNode(doc, row, "bka056", reg.SIMainInfo.TotCost.ToString());
                    AppendChildNode(doc, row, "bka057", "-1");
                    AppendChildNode(doc, row, "bka058", "-" + reg.SIMainInfo.TotCost.ToString());
                    AppendChildNode(doc, row, "bka070", "");
                    AppendChildNode(doc, row, "bka074", doctor.IDCard);
                    AppendChildNode(doc, row, "bka075", doctor.Name);
                    AppendChildNode(doc, row, "bka071", "");
                    AppendChildNode(doc, row, "aaz213", "1");
                    AppendChildNode(doc, row, "aka036", "0");
                }

                xml = doc.InnerXml.ToString();
                return 1;
            }
            catch (Exception e)
            {
                this.ErrorMsg = e.Message.ToString();
                return -1;
            }
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
