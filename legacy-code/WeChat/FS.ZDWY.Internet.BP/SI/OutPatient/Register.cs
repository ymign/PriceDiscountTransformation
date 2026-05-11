using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FS.ZDWY.Internet.BP.SI.OutPatient
{
    public class Register : AbstractService<FS.ZDWY.Internet.Models.Views.OutPatient.ComPatient, GDSI.Models.OutParam.OutParamBizh110104>
    {
        public override string FunctionID
        {
            get { return "bizh110104"; }
        }

        protected override int ConvertModelToSendMessage(FS.ZDWY.Internet.Models.Views.OutPatient.ComPatient r, out string xml, params object[] appendParams)
        {
            xml = "";
            try
            {
                //appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042}
                //医保分中心编码
                string Aaa027 = "";
                //个人电脑号
                string Aac001 = "";
                //持卡就诊登记许可号
                string icpermit = "";
                //业务类型
                string Aka130 = "";
                //医疗待遇类型
                string Bka006 = "";
                //预计算
                string Save_flag = "";
                //门慢申请序号、门诊选点号
                string Aaz267 = "";
                //工伤生育凭证号
                string Bka042 = "";
                Aaa027 = appendParams[0] as string;
                Aac001 = appendParams[1] as string;
                icpermit = appendParams[2] as string;
                Aka130 = appendParams[3] as string;
                Bka006 = appendParams[4] as string;
                Save_flag = appendParams[5] as string;
                Aaz267 = appendParams[6] as string;
                Bka042 = appendParams[7] as string;

                #region XML
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlElement input = doc.CreateElement("program");
                doc.AppendChild(input);

                AppendChildNode(doc, input, "session_id", FS.ZDWY.Internet.BP.SI.ReadSIConfig.SessionId);
                AppendChildNode(doc, input, "function_id", FunctionID);
                AppendChildNode(doc, input, "ic_reg_permit", icpermit);
                AppendChildNode(doc, input, "akb020", FS.ZDWY.Internet.BP.SI.ReadSIConfig.HospitalCode);
                AppendChildNode(doc, input, "aac001", Aac001);
                AppendChildNode(doc, input, "aka130", Aka130 == "16" ? "11" : Aka130);//门特挂号走普通门诊
                AppendChildNode(doc, input, "bka006", Bka006.Substring(0, 2) == "16" ? "110" : Bka006);//门特挂号走普通门诊

                #endregion

                #region 挂号

                #region 患者信息
                AppendChildNode(doc, input, "bka017", r.OperTime.Date.ToString("yyyyMMdd"));
                AppendChildNode(doc, input, "bka014", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code);//工号
                AppendChildNode(doc, input, "bka015", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Name);
                AppendChildNode(doc, input, "bka021", r.Dept.ID);
                AppendChildNode(doc, input, "bka022", r.Dept.Name.Replace("＆", "&amp;"));
                AppendChildNode(doc, input, "bka019", r.Dept.ID);
                AppendChildNode(doc, input, "bka020", r.Dept.Name);
                AppendChildNode(doc, input, "bka026", "ZJJM");
                if (Save_flag == "0")
                {
                    //试算挂号减免金额 未挂号，传CardNo
                    AppendChildNode(doc, input, "bka025", r.CardNo);
                }
                else
                {
                    //计算挂号减免金额
                    AppendChildNode(doc, input, "bka025", r.ClinicCode);
                }
                AppendChildNode(doc, input, "bka070", "99999");
                AppendChildNode(doc, input, "akc172", "ZDWY");
                AppendChildNode(doc, input, "save_flag", Save_flag);
                AppendChildNode(doc, input, "bka893", Save_flag);
                if (!string.IsNullOrEmpty(Aaz267))
                {
                    AppendChildNode(doc, input, "aaz267", Aaz267);
                }
                else
                {
                    AppendChildNode(doc, input, "aaz267", "");
                }
                if (!string.IsNullOrEmpty(Bka042))
                {
                    AppendChildNode(doc, input, "bka042", Bka042);
                }
                else
                {
                    AppendChildNode(doc, input, "bka042", "");
                }

                #endregion

                #region 费用信息

                XmlElement feeinfo = doc.CreateElement("feeinfo");
                input.AppendChild(feeinfo);
                XmlElement row = doc.CreateElement("row");
                feeinfo.AppendChild(row);


                FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new BP.OutPatient.Register.Manager();
                string regfee = "";
                int ret = mgr.getRegItemCode(r.RegLevel.ID, ref regfee);
                if (ret < 0)
                {
                    this.ErrorMsg = "调用门诊挂号接口(bizh110104)异常。获取挂号费失败!";
                    return -1;
                }
                Neusoft.HISFC.BizProcess.Integrate.Fee feeMgr = new Neusoft.HISFC.BizProcess.Integrate.Fee();
                Neusoft.HISFC.Models.Fee.Item.Undrug undrg = feeMgr.GetItem(regfee);
                Neusoft.HISFC.BizLogic.Manager.Person personMgr = new Neusoft.HISFC.BizLogic.Manager.Person();
                if (undrg != null && !string.IsNullOrEmpty(undrg.ID))
                {
                    Neusoft.HISFC.Models.Base.Employee doctor = personMgr.GetPersonByID(r.Doct.ID);//获取开立医生信息
                    AppendChildNode(doc, row, "aka063", "21");
                    AppendChildNode(doc, row, "ake005", undrg.ID);
                    AppendChildNode(doc, row, "ake006", undrg.Name);
                    AppendChildNode(doc, row, "bka052", "");
                    AppendChildNode(doc, row, "bka053", "");
                    AppendChildNode(doc, row, "bka054", "");
                    AppendChildNode(doc, row, "bka051", r.OperTime.Date.ToString("yyyyMMdd"));
                    AppendChildNode(doc, row, "bka055", "次");
                    AppendChildNode(doc, row, "bka056", (r.OwnDigFee + r.RegFee).ToString());
                    AppendChildNode(doc, row, "bka057", "1");
                    AppendChildNode(doc, row, "bka058", (r.OwnDigFee + r.RegFee).ToString());
                    AppendChildNode(doc, row, "bka070", "");
                    AppendChildNode(doc, row, "bka074", doctor.IDCard);
                    AppendChildNode(doc, row, "bka075", doctor.Name);
                    AppendChildNode(doc, row, "bka071", "");
                    AppendChildNode(doc, row, "aka036", "0");
                }
                else
                {
                    this.ErrorMsg = "调用门诊挂号接口(bizh110104)异常。获取挂号费失败!";
                    return -1;
                }
                #endregion

                #endregion
                xml = doc.InnerXml.ToString();
                return 1;
            }
            catch (Exception e)
            {
                base.ErrorMsg = e.Message.ToString();
                return -1;
            }
        }


        protected override int ConvertReciverMessageToModel(System.Xml.XmlDocument doc, ref GDSI.Models.OutParam.OutParamBizh110104 outParam)
        {
            try
            {
                XmlNode node = null;
                node = doc.SelectSingleNode("program/return_code");
                ReturnCode = node == null ? string.Empty : node.InnerText.ToString();
                node = doc.SelectSingleNode("program/return_code_message");
                this.ErrorMsg = node == null ? string.Empty : node.InnerText.ToString();
                if (ReturnCode != "1")
                {
                    return -1;
                }
                outParam.Return_code = ReturnCode;
                outParam.Return_code_message = base.ErrorMsg;
                GDSI.Models.Payinfo payinfo = new GDSI.Models.Payinfo();
                #region 挂号结果
                node = doc.SelectSingleNode("program/payinfo/row/aaz218");
                payinfo.Aaz218 = node == null ? "" : node.InnerText;

                node = doc.SelectSingleNode("program/payinfo/row/akb020");
                payinfo.Akb020 = node == null ? string.Empty : node.InnerText.ToString();
                node = doc.SelectSingleNode("program/payinfo/row/akc264");
                if (node != null)
                {
                    payinfo.Akc264 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka831");
                if (node != null)
                {
                    payinfo.Bka831 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka832");
                if (node != null)
                {
                    payinfo.Bka832 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka825");
                if (node != null)
                {
                    payinfo.Bka825 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka826");
                if (node != null)
                {
                    payinfo.Bka826 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/aka151");
                if (node != null)
                {
                    payinfo.Aka151 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka838");
                if (node != null)
                {
                    payinfo.Bka838 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/akb067");
                if (node != null)
                {
                    payinfo.Akb067 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/akb066");
                if (node != null)
                {
                    payinfo.Akb066 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka821");
                if (node != null)
                {
                    payinfo.Bka821 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka839");
                if (node != null)
                {
                    payinfo.Bka839 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/ake039");
                if (node != null)
                {
                    payinfo.Ake039 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/ake035");
                if (node != null)
                {
                    payinfo.Ake035 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/ake026");
                if (node != null)
                {
                    payinfo.Ake026 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/ake029");
                if (node != null)
                {
                    payinfo.Ake029 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka841");
                if (node != null)
                {
                    payinfo.Bka841 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka842");
                if (node != null)
                {
                    payinfo.Bka842 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }

                node = doc.SelectSingleNode("program/payinfo/row/bka840");
                if (node != null)
                {
                    payinfo.Bka840 = Neusoft.FrameWork.Function.NConvert.ToDecimal(node.InnerText.ToString());
                }
                #endregion
                outParam.Payinfo = payinfo;
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
