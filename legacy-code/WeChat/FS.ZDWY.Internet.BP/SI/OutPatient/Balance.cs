using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FS.ZDWY.Internet.BP.SI.OutPatient
{
    public class Balance : AbstractService<ArrayList, GDSI.Models.OutParam.OutParamBizh110104>
    {
        public override string FunctionID
        {
            get { return "bizh110104"; }
        }

        /// <summary>
        /// 挂号业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Registration.Register registerManager = new Neusoft.HISFC.BizLogic.Registration.Register();

        private Neusoft.HISFC.BizLogic.Manager.Person personMgr = new Neusoft.HISFC.BizLogic.Manager.Person();

        private FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new FS.ZDWY.Internet.BP.OutPatient.Register.Manager();

        protected override int ConvertModelToSendMessage(ArrayList fee, out string xml, params object[] appendParams)
        {
            xml = "";
            try
            {
                //appendParams[](){Aaa027,Aac001,ic_reg_permit,Aka130,Bka006,Save_flag,aaz267,bka042,clinicCode,Bka026}
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
                string clinicCode = "";
                string Bka026 = "";
                Aaa027 = appendParams[0] as string;
                Aac001 = appendParams[1] as string;
                icpermit = appendParams[2] as string;
                Aka130 = appendParams[3] as string;
                Bka006 = appendParams[4] as string;
                Save_flag = appendParams[5] as string;
                Aaz267 = appendParams[6] as string;
                Bka042 = appendParams[7] as string;
                clinicCode = appendParams[8] as string;
                Bka026 = appendParams[9] as string;

                Neusoft.HISFC.Models.Registration.Register reg = null; //患者基本信息
                reg = this.registerManager.GetByClinic(clinicCode);
                if (string.IsNullOrEmpty(reg.ID))
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    throw new Exception("获取挂号信息出错");
                }

                Hashtable htLimit = mgr.GetItemLimitList(reg.ID);

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
                AppendChildNode(doc, input, "aka130", Aka130);//门特挂号走普通门诊
                AppendChildNode(doc, input, "bka006", Bka006);//门特挂号走普通门诊

                #endregion

                #region 挂号

                #region 患者信息
                AppendChildNode(doc, input, "bka017", reg.DoctorInfo.SeeDate.ToString("yyyyMMdd"));
                AppendChildNode(doc, input, "bka014", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code);//工号
                AppendChildNode(doc, input, "bka015", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Name);
                AppendChildNode(doc, input, "bka021", reg.DoctorInfo.Templet.Dept.ID);
                AppendChildNode(doc, input, "bka022", reg.DoctorInfo.Templet.Dept.Name);
                AppendChildNode(doc, input, "bka019", reg.DoctorInfo.Templet.Dept.ID);
                AppendChildNode(doc, input, "bka020", reg.DoctorInfo.Templet.Dept.Name);
                AppendChildNode(doc, input, "bka026", string.IsNullOrEmpty(Bka026) ? "null01" : Bka026);
                if (Save_flag == "0")
                {
                    //试算挂号减免金额 未挂号，传CardNo
                    AppendChildNode(doc, input, "bka025", reg.ID);
                }
                else
                {
                    //计算挂号减免金额
                    AppendChildNode(doc, input, "bka025", reg.ID);
                }
                AppendChildNode(doc, input, "bka070", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code);
                AppendChildNode(doc, input, "akc172", FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Name);
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

                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItem in fee)
                {
                    XmlElement row = doc.CreateElement("row");
                    feeinfo.AppendChild(row);

                    if (feeItem == null)
                    {
                        continue;
                    }
                    decimal unitPrice = this.GetPrice(feeItem);
                    decimal Count = this.GetCount(feeItem);
                    Neusoft.HISFC.Models.Base.Employee doctor = this.personMgr.GetPersonByID(feeItem.RecipeOper.ID);//获取开立医生信息

                    AppendChildNode(doc, row, "aka063", feeItem.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug ? "1" : "2");
                    AppendChildNode(doc, row, "ake005", feeItem.Item.ID);
                    AppendChildNode(doc, row, "ake006", feeItem.Item.Name);
                    AppendChildNode(doc, row, "bka052", string.Empty);
                    AppendChildNode(doc, row, "bka053", string.Empty);
                    AppendChildNode(doc, row, "bka054", feeItem.Item.Specs);
                    AppendChildNode(doc, row, "bka051", this.personMgr.GetSysDate("yyyyMMdd"));
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
