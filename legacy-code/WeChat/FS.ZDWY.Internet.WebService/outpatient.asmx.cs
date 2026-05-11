using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// outpatient 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class outpatient : System.Web.Services.WebService
    {
        #region 属性

        BP.OutPatient.PatientInfoManager patientManager;
        /// <summary>
        /// 患者基本信息管理
        /// </summary>
        BP.OutPatient.PatientInfoManager PatientManager
        {
            get
            {
                if (patientManager == null)
                {
                    patientManager = new BP.OutPatient.PatientInfoManager();
                }
                return patientManager;
            }
        }

        LogHelper.ErrorLog errorLogManager;

        LogHelper.ErrorLog ErrorLogManager
        {
            get
            {
                if (errorLogManager == null)
                {
                    errorLogManager = new LogHelper.ErrorLog();
                }
                return errorLogManager;
            }
        }

        LogHelper.ServiceLog serviceLogManager;
        /// <summary>
        /// 服务日志管理
        /// </summary>
        LogHelper.ServiceLog ServiceLogManager
        {
            get
            {
                if (serviceLogManager == null)
                {
                    serviceLogManager = new LogHelper.ServiceLog();
                }
                return serviceLogManager;
            }
        }

        #endregion

        [WebMethod]
        public string HelloWorld()
        {
            FS.ZDWY.Internet.BP.OutPatient.RegisterInfoManager mgr = new BP.OutPatient.RegisterInfoManager();

            return mgr.Test();
        }

        [WebMethod(Description = "医保减免")]
        public string hcare(string req)
        {
            #region 入参模板
            /*
<Request><data>
<patientId></patientId>
<orderId></orderId>
<hospitalNum></hospitalNum>
<patientName></patientName>
<patientCard></patientCard>
<frontProviderId></frontProviderId>
<tranno></tranno>
</data></Request>
            */
            #endregion
            string GUID = Guid.NewGuid().ToString();
            ServiceLogManager.Write(GUID+"挂号医保减免传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<hcareAmount>{0}</hcareAmount>
<selfAmount>{1}</selfAmount>
<expenseAmount>{2}</expenseAmount>
<totalAmount>{3}</totalAmount>
<ecostAmount>{5}</ecostAmount>
<clincCode>{6}</clincCode>
<remark>{4}</remark>
";
            #endregion

            #region 获取入参值并验证

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(req);

            List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "平台订单号" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "patientId", NodeInstruction = "院内用户ID" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "patientName", NodeInstruction = "就诊人名称" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "patientCard", NodeInstruction = "就诊人卡号" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "frontProviderId", NodeInstruction = "第三方服务商 ID" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "tranno", NodeInstruction = "就诊持卡号码" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "feeType", NodeInstruction = "合同单位" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "mdtrtCertType", NodeInstruction = "就诊凭证类型" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "mdtrtCertNo", NodeInstruction = "就诊凭证编号" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "cardSN", NodeInstruction = "卡识别码" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "psnCertType", NodeInstruction = "人员证件类型" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "certNo", NodeInstruction = "证件号码" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "isMedical", NodeInstruction = "是否医保减免" });
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "settlementType", NodeInstruction = "减免方式" });
            Dictionary<string, string> nodesVales = new Dictionary<string, string>();
            foreach (var item in reqNodes)
            {
                item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                if (item.IsRequired)
                {
                    Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                }
                nodesVales.Add(item.NodeName, item.NodeValue);
            }
            #endregion

            #region 抽取信息
            string orderid = nodesVales["orderId"];
            string hospitalNum = nodesVales["hospitalNum"];
            string transno = nodesVales["tranno"] == null ? "" : nodesVales["tranno"];
            string patientId = nodesVales["patientId"];
            string settlementType = nodesVales["settlementType"];
            FS.ZDWY.Internet.Models.HcareInModel hcareModel = new Models.HcareInModel();
            hcareModel.MdtrtCertType = nodesVales["mdtrtCertType"];
            hcareModel.MdtrtCertNo = nodesVales["mdtrtCertNo"];
            hcareModel.CardSN = nodesVales["cardSN"] == null ? "" : nodesVales["cardSN"];
            hcareModel.PsnCertType = nodesVales["psnCertType"] == null ? "" : nodesVales["psnCertType"];
            hcareModel.CertNo = nodesVales["certNo"] == null ? "" : nodesVales["certNo"];
            hcareModel.CardNo = patientId;
            hcareModel.IsMedical = nodesVales["isMedical"] == null ? "0" : nodesVales["isMedical"];
            #endregion

            #region 挂号减免

            string resXml = string.Empty;
            BP.OutPatient.RegisterInfoManager registerManager = new BP.OutPatient.RegisterInfoManager();
            Models.Views.ComResult<Models.Views.OutPatient.HcareResult> result = registerManager.Hcare(orderid, hospitalNum, transno, patientId, hcareModel, settlementType);
            if (result.Message.Contains("参保人只能去转诊机构就医"))
            {
                result.Message = "根据珠海医保政策调整，自2022年12月1日起，取消门诊挂号减免诊金10元政策，统一纳入门诊共济和门特待遇报销。";
            }
            if (!result.IsSuccessful)
            {
                resXml = Function.GetResponseXML(false, result.Message, "");
            }
            else
            {
                resXml = Function.GetResponseXML(true, "操作成功",
                    string.Format(dataXml, ((int)result.ReturnData.HcareAmount).ToString(), ((int)result.ReturnData.SelfAmount).ToString(), ((int)result.ReturnData.ExpenseAmount).ToString(), ((int)result.ReturnData.TotalAmount).ToString(), result.ReturnData.Remark, ((int)result.ReturnData.EcostAmount).ToString(), result.ReturnData.ClincCode.ToString())
                    );
            }
            ServiceLogManager.Write(GUID + "挂号医保减免传出报文：" + resXml);
            return resXml;

            #endregion
        }

        [WebMethod(Description = "取消医保减免")]
        public string cancelHcare(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            #region 获取入参值并验证

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(req);

            List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
            reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "clincCode", NodeInstruction = "门诊流水号" });

            Dictionary<string, string> nodesVales = new Dictionary<string, string>();
            foreach (var item in reqNodes)
            {
                item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                if (item.IsRequired)
                {
                    Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                }
                nodesVales.Add(item.NodeName, item.NodeValue);
            }
            #endregion
            #region 出参数据报文模板
            string dataXml = @"<hcareAmount>{0}</hcareAmount>
<selfAmount>{1}</selfAmount>
<expenseAmount>{2}</expenseAmount>
<totalAmount>{3}</totalAmount>
<ecostAmount>{5}</ecostAmount>
<remark>{4}</remark>
";
            #endregion

            #region 抽取信息
            string clincCode = nodesVales["orderId"];

            #endregion

            #region 挂号减免
            string resXml = string.Empty;
            BP.OutPatient.RegisterInfoManager registerManager = new BP.OutPatient.RegisterInfoManager();
            Models.Views.ComResult<Models.Views.OutPatient.HcareResult> result = registerManager.CancelHcare(clincCode);
            if (!result.IsSuccessful)
            {
                resXml = Function.GetResponseXML(false, "操作失败！" + result.Message, "");
            }
            else
            {
                resXml = Function.GetResponseXML(true, "操作成功", "");
            }
            ServiceLogManager.Write("传出报文：" + resXml);
            return resXml;

            #endregion
        }

        [WebMethod(Description = "支付订单")]
        public string pay(string req)
        {
            #region 入参模板
            /*
<Request><data>
<orderId></orderId>
<hospitalNum></hospitalNum>
<payMode></payMode>
<payAmt></payAmt>
<transactionNo></transactionNo>
<payTime></payTime>
<patientId></patientId>
<certifcateType></certifcateType>
<certifcateNo></certifcateNo>
<medicalNo></medicalNo>
<cardType></cardType>
<cardNo></cardNo>
<medicalInsuranceId></medicalInsuranceId>
<frontProviderId></frontProviderId>
<patientId></patientId>
<hcareAmount></hcareAmount>
<selfAmount></selfAmount>
<expenseAmount></expenseAmount>
<totalAmount></totalAmount>
<payChannel></payChannel>            
</data></Request>
            */
            #endregion
            ServiceLogManager.Write("支付订单pay传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<hospTradeId>{0}</hospTradeId>
                        <invoiceId>{1}</invoiceId>
                        <receiptId>{2}</receiptId>
                        <visitAddress>{3}</visitAddress>
                        <visitNo>{4}</visitNo>
                        <proof>{5}</proof>
                        <remark>{6}</remark>
                        <clincCode>{7}</clincCode>";
            #endregion
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }

                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "平台订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payMode", NodeInstruction = "支付方式" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payAmt", NodeInstruction = "支付金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "transactionNo", NodeInstruction = "收单机构流水号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payTime", NodeInstruction = "支付时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "patientId", NodeInstruction = "院内用户ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "certifcateType", NodeInstruction = "用户证件类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "certifcateNo", NodeInstruction = "用户证件号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "medicalNo", NodeInstruction = "病历号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "cardType", NodeInstruction = "诊疗卡类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "cardNo", NodeInstruction = "诊疗卡号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "medicalInsuranceId", NodeInstruction = "医保账户" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "frontProviderId", NodeInstruction = "第三方服务商 ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hcareAmount", NodeInstruction = "医保减免" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "selfAmount", NodeInstruction = "自费金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "expenseAmount", NodeInstruction = "医保报销" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "ecostAmount", NodeInstruction = "优惠金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "totalAmount", NodeInstruction = "费用总金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "tranno", NodeInstruction = "就诊持卡号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "clincCode", NodeInstruction = "门诊流水号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "isMedical", NodeInstruction = "是否医保" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "payChannel", NodeInstruction = "支付渠道" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "InformedConsentResult", NodeInstruction = "知情同意书结果" });
                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }
                #endregion

                #region 抽取信息

                //订单信息
                //就按这个入参信息存到新的订单表里
                Models.PLATFORM_REGISTER_PAY payinfo = new Models.PLATFORM_REGISTER_PAY();
                payinfo.ORDERID = nodesVales["orderId"];
                payinfo.HOSPITALNUM = nodesVales["hospitalNum"];
                payinfo.PAYMODE = nodesVales["payMode"];
                payinfo.PAYAMT = nodesVales["payAmt"];
                payinfo.TRANSACTIONNO = nodesVales["transactionNo"];
                payinfo.PAYTIME = Function.ToDateTime(nodesVales["payTime"]);
                if (payinfo.PAYTIME <= DateTime.MinValue)
                {
                    throw new Exception("【payTime】时间入参格式不符！");
                }
                payinfo.PATIENTID = nodesVales["patientId"];
                payinfo.CERTIFCATETYPE = nodesVales["certifcateType"];
                payinfo.CERTIFCATENO = nodesVales["certifcateNo"];
                payinfo.MEDICALNO = nodesVales["medicalNo"];
                payinfo.CARDTYPE = nodesVales["cardType"];
                payinfo.CARDNO = nodesVales["cardNo"];
                payinfo.MEDICALINSURANCEID = nodesVales["medicalInsuranceId"];
                payinfo.FRONTPROVIDERID = nodesVales["frontProviderId"];
                payinfo.HCAREAMOUNT = Function.ToDecimal(nodesVales["hcareAmount"]);
                payinfo.SELFAMOUNT = Function.ToDecimal(nodesVales["selfAmount"]);
                payinfo.EXPENSEAMOUNT = Function.ToDecimal(nodesVales["expenseAmount"]);
                payinfo.TOTALAMOUNT = Function.ToDecimal(nodesVales["totalAmount"]);
                payinfo.ECOSTAMOUNT = Function.ToDecimal(nodesVales["ecostAmount"]);
                payinfo.OPERCODE = Function.DefaultOper.Code;
                if (nodesVales["payChannel"] == "2")
                    payinfo.OPERCODE = Function.ZFBOper.Code;
                else if (nodesVales["payChannel"] == "3")
                    payinfo.OPERCODE = Function.APPOper.Code;
                payinfo.OPERNAME = Function.DefaultOper.Name;
                payinfo.TransNo = nodesVales["tranno"];
                string isMedical = nodesVales["isMedical"];
                string clincCode = nodesVales["clincCode"];
                string informedConsentResult = nodesVales["InformedConsentResult"];
                #endregion

                #region 挂号
                string resXml = string.Empty;
                BP.OutPatient.RegisterInfoManager registerManager = new BP.OutPatient.RegisterInfoManager();
                Models.Views.ComResult<Models.Views.OutPatient.PayResult> result = registerManager.RegisterPay(payinfo, clincCode, isMedical,informedConsentResult);
                if (!result.IsSuccessful)
                {
                    resXml = Function.GetResponseXML(false, "操作失败！" + result.Message, "");
                }
                else
                {
                    resXml = Function.GetResponseXML(true, "操作成功",
                        string.Format(dataXml, result.ReturnData.HospTradeId, result.ReturnData.InvoiceId, result.ReturnData.ReceiptId, Function.XmlString(result.ReturnData.VisitAddress), result.ReturnData.VisitNo, result.ReturnData.Proof, result.ReturnData.Remark, result.ReturnData.ClinicCode)
                        );
                }
                ServiceLogManager.Write("支付订单pay传出报文：" + resXml);
                return resXml;

                #endregion

            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("支付订单pay传出报文：" + resXml);
                return resXml;
            }
            return "";

        }

        [WebMethod(Description = "订单退费")]
        public string refund(string req)
        {
            #region 入参模板
            /*
<orderId>           </orderId>
<psRefOrdNum>       </psRefOrdNum>
<hospitalNum>       </hospitalNum>
<hospTradeId>       </hospTradeId>
<payRefTime>        </payRefTime>
<refundAmt>         </refundAmt>
<refundReason>      </refundReason>
<frontProviderId>   </frontProviderId>
<patientId>         </patientId>
            */
            #endregion

            ServiceLogManager.Write("订单退费refund传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<refundFlag>{0}</refundFlag>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "平台订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "psRefOrdNum", NodeInstruction = "平台退款订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospTradeId", NodeInstruction = "医院支付单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payRefTime", NodeInstruction = "退费时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "refundAmt", NodeInstruction = "退款金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "refundReason", NodeInstruction = "退费原因" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "frontProviderId", NodeInstruction = "第三方服务商 ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "patientId", NodeInstruction = "院内用户ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "tranno", NodeInstruction = "就诊持卡号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "payChannel", NodeInstruction = "支付渠道" });
                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                #endregion

                string orderId = nodesVales["orderId"];
                string psRefOrdNum = nodesVales["psRefOrdNum"];
                string hospitalNum = nodesVales["hospitalNum"];
                string hospTradeId = nodesVales["hospTradeId"];
                string refundReason = nodesVales["refundReason"];
                string tranno = nodesVales["tranno"] == null ? "" : nodesVales["tranno"];
                string payChannel = nodesVales["payChannel"];
                #region 退费
                string resXml = string.Empty;
                BP.OutPatient.RegisterInfoManager registerManager = new BP.OutPatient.RegisterInfoManager();
                Models.Views.ComResult<Models.PLATFORM_REGISTER_PAY> result = null;
                result = registerManager.RegisterBackPay(orderId, psRefOrdNum, hospitalNum, hospTradeId, refundReason, tranno, payChannel);
                if (!result.IsSuccessful)
                {
                    resXml = Function.GetResponseXML(false, "操作失败！" + result.Message, "<refundFlag>1</refundFlag>");
                }
                else
                {
                    resXml = Function.GetResponseXML(true, "操作成功", "<refundFlag>1</refundFlag>");
                }
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
                #endregion


            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                ServiceLogManager.Write("订单退费refund传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "门诊共济定点备案")]
        public string PerSonMZGJRecord(string req)
        {
            string guid = Guid.NewGuid().ToString();
            ServiceLogManager.Write("门诊共济定点备案【" + guid + "】：" + req);
            if (string.IsNullOrEmpty(req))
            {
                throw new Exception("入参不正确");
            }
            try
            {
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "MdtrCertTyp", NodeInstruction = "就诊凭证类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "MdtrtCertNo", NodeInstruction = "就诊凭证编号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "PsnCertType", NodeInstruction = "人员证件类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "InsuplcAdmdvs", NodeInstruction = "参保区划" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "CardSN", NodeInstruction = "卡识别码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "CertNo", NodeInstruction = "证件号码" });
                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                #endregion
                FS.ZDWY.Internet.BP.OutPatient.PerSonMZGJRecordModel reqModel = new BP.OutPatient.PerSonMZGJRecordModel();
                reqModel.MdtrCertTyp = nodesVales["MdtrCertTyp"];
                reqModel.MdtrtCertNo = nodesVales["MdtrtCertNo"];
                reqModel.PsnCertType = nodesVales["PsnCertType"];
                reqModel.InsuplcAdmdvs = nodesVales["InsuplcAdmdvs"];
                reqModel.CardSN = nodesVales["CardSN"];
                reqModel.CertNo = nodesVales["CertNo"];
                FS.ZDWY.Internet.BP.OutPatient.PerSonRecordManager perSonRecordManager = new BP.OutPatient.PerSonRecordManager();
                var result = perSonRecordManager.PerSonMZGJRecord(reqModel);
                string resXml = string.Empty;
                if (!result.IsSuccessful)
                {
                    resXml = Function.GetResponseXML(false, "操作失败！" + result.Message, "");
                }
                else
                {
                    resXml = Function.GetResponseXML(true, "操作成功", "");
                }
                ServiceLogManager.Write("门诊共济定点备案【" + guid + "】：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message.ToString(), "");
                ServiceLogManager.Write("门诊共济定点备案【" + guid + "】：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "门诊共济选点改点")]
        public string PerSonMZGJSelectPoint(string req)
        {
            string guid = Guid.NewGuid().ToString();
            ServiceLogManager.Write("门诊共济选点改点【" + guid + "】：" + req);
            if (string.IsNullOrEmpty(req))
            {
                throw new Exception("入参不正确");
            }
            try
            {
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "MdtrCertTyp", NodeInstruction = "就诊凭证类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "MdtrtCertNo", NodeInstruction = "就诊凭证编号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "PsnCertType", NodeInstruction = "人员证件类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "InsuplcAdmdvs", NodeInstruction = "参保区划" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "CardSN", NodeInstruction = "卡识别码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "CertNo", NodeInstruction = "证件号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "ChgRea", NodeInstruction = "变更原因" });
                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                #endregion
                FS.ZDWY.Internet.BP.OutPatient.PerSonMZGJSelectPointModel reqModel = new BP.OutPatient.PerSonMZGJSelectPointModel();
                reqModel.MdtrCertTyp = nodesVales["MdtrCertTyp"];
                reqModel.MdtrtCertNo = nodesVales["MdtrtCertNo"];
                reqModel.PsnCertType = nodesVales["PsnCertType"];
                reqModel.InsuplcAdmdvs = nodesVales["InsuplcAdmdvs"];
                reqModel.CardSN = nodesVales["CardSN"];
                reqModel.CertNo = nodesVales["CertNo"];
                reqModel.ChgRea = nodesVales["ChgRea"];
                FS.ZDWY.Internet.BP.OutPatient.PerSonRecordManager perSonRecordManager = new BP.OutPatient.PerSonRecordManager();
                var result = perSonRecordManager.PerSonMZGJSelectPoint(reqModel);
                string resXml = string.Empty;
                if (!result.IsSuccessful)
                {
                    resXml = Function.GetResponseXML(false, "操作失败！" + result.Message, "");
                }
                else
                {
                    resXml = Function.GetResponseXML(true, "操作成功", "");
                }
                ServiceLogManager.Write("门诊共济选点改点【" + guid + "】：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + ex.Message.ToString(), "");
                ServiceLogManager.Write("门诊共济选点改点【" + guid + "】：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "预约挂号订单查询")]
        public string query(string req)
        {
            #region 入参模板
            /*
            <Request><data>
<orderId></orderId>
<hospitalNum></hospitalNum>
<queryFlag></queryFlag>
<frontProviderId></frontProviderId>
</data></Request>
            */
            #endregion

            ServiceLogManager.Write("传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<hospitalNum>{0}</hospitalNum> 
<visitAddress>{1}</visitAddress>
<visitNo>{2}</visitNo>
<orderTime>{3}</orderTime>
<payTime>{4}</payTime>
<takeTime>{5}</takeTime>
<cancelTime>{6}</cancelTime>
<refundTime>{7}</refundTime>
<payAmt>{8}</payAmt>
<RefundFee>{9}</RefundFee>
<orderStatus>{10}</orderStatus>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "平台订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "queryFlag", NodeInstruction = "查询标志" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "frontProviderId", NodeInstruction = "第三方服务商 ID" });

                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                string resXml = string.Empty;
                BP.OutPatient.QueryManager QueManager = new BP.OutPatient.QueryManager();
                Models.Views.ComResult<Models.Views.QueryResult> result = QueManager.QueryOrder(nodesVales["orderId"], nodesVales["hospitalNum"], nodesVales["queryFlag"], nodesVales["frontProviderId"]);
                if (result.IsSuccessful)
                {
                    resXml = Function.GetResponseXML(true, "操作成功",
                            string.Format(dataXml, result.ReturnData.HospitalNum,
    Function.XmlString(result.ReturnData.VisitAddress),
    result.ReturnData.VisitNo,
    result.ReturnData.OrderTime,
    result.ReturnData.PayTime,
    result.ReturnData.TakeTime,
    result.ReturnData.CancelTime,
    result.ReturnData.RefundTime,
    result.ReturnData.PayAmt,
    result.ReturnData.RefundFee,
    result.ReturnData.OrderStatus)
                            );
                }
                else
                {
                    throw new Exception(result.Message);
                }
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;

                #endregion
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }

        #region 缴费
        [WebMethod(Description = "待缴费记录")]
        public string billList(string req)
        {
            string GUID = Guid.NewGuid().ToString();
            ServiceLogManager.Write(GUID + "传入报文：" + req);
            #region 出参数据报文模板
            //就诊信息

            string RegdataXml = @"<item><hospitalNum>{0}</hospitalNum>
<deptCode>{1}</deptCode>
<deptName>{2}</deptName>
<doctorCode>{3}</doctorCode>
<doctorName>{4}</doctorName>
<totalAmt>{5}</totalAmt>
<visitDate>{6}</visitDate>
<visitNo>{7}</visitNo>
<clincCode>{8}</clincCode>
<reciptNo>{9}</reciptNo>
<reciptType>{10}</reciptType>
<bills>
";

            //项目明细
            string ItemdataXml = @"<bill><prescriptionId>{0}</prescriptionId>
<prescriptionType>{1}</prescriptionType>
<selfAmt>{2}</selfAmt>
<itemName>{3}</itemName>
<RECIPENO>{4}</RECIPENO>
<isStore>{5}</isStore>
<reciptType>{6}</reciptType>
</bill>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "patientId", NodeInstruction = "院内用户id" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "medicalNo", NodeInstruction = "病历号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "visitNo", NodeInstruction = "就诊号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "startTime", NodeInstruction = "开始时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "endTime", NodeInstruction = "结束时间" });

                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                string patientId = nodesVales["patientId"];
                string medicalNo = nodesVales["medicalNo"];
                string visitNo = nodesVales["visitNo"];
                string startTime = nodesVales["startTime"];
                string endTime = nodesVales["endTime"];

                string resXml = string.Empty;
                FS.ZDWY.Internet.BP.OutPatient.QueryManager mgr = new BP.OutPatient.QueryManager();
                string erro = "";
                List<FS.ZDWY.Internet.Models.FIN_OPR_REGISTER> registerlist = mgr.GetRegisterList(patientId, medicalNo, visitNo, startTime, endTime, ref erro);
                if (registerlist == null || registerlist.Count == 0)
                {
                    throw new Exception(erro + "没有挂号记录！");
                }
                resXml = "";
                System.Collections.Hashtable hsItemList = mgr.getItemListWipeOffZZSB();//获取不可在自助设备缴费的项目
                foreach (FS.ZDWY.Internet.Models.FIN_OPR_REGISTER reg in registerlist)
                {
                    if (!mgr.RecipeFlagIsNull(reg.CLINIC_CODE))
                    {
                        throw new Exception("存在处方类型为空的医嘱，请到人工窗口处理");
                    }
                    DataTable dtOld = mgr.billListOld(reg.CLINIC_CODE, ref erro);
                    DataTable dt = mgr.billList(reg.CLINIC_CODE, ref erro);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        continue;
                    }
                    int totcost = 0;
                    if (dtOld.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtOld.Rows.Count; i++)
                        {
                            if (hsItemList.Contains(dtOld.Rows[i]["prescriptionId"].ToString()))//存在不可在自助设备缴费的项目
                            {
                                throw new Exception(erro + "费用项目包含了:" + dtOld.Rows[i]["itemName"].ToString() + ",请到人工窗口进行缴费，谢谢合作！");
                            }
                        }
                    }
                    if (dt.Rows.Count > 0)
                    {
                        var groupedData = dt.AsEnumerable().GroupBy(row => row.Field<string>("RECIPE_NO"));
                        foreach (var group in groupedData)
                        {
                            string detialXml = "";
                            totcost = 0;
                            for (int i = 0; i < group.Count(); i++)
                            {
                                if (hsItemList.Contains(group.ElementAt(i)["prescriptionId"].ToString()))//存在不可在自助设备缴费的项目
                                {
                                    throw new Exception(erro + "费用项目包含了:" + group.ElementAt(i)["itemName"].ToString() + ",请到人工窗口进行缴费，谢谢合作！");
                                }
                                detialXml += string.Format(ItemdataXml, group.ElementAt(i)["prescriptionId"].ToString(),
                                                        group.ElementAt(i)["prescriptionType"].ToString(),
                                                        group.ElementAt(i)["selfAmt"].ToString(),
                                                        group.ElementAt(i)["itemName"].ToString(),
                                                        group.ElementAt(i)["RECIPE_NO"].ToString(),
                                                        group.ElementAt(i)["isStore"].ToString(),
                                                        group.ElementAt(i)["reciptType"].ToString()
                                                        );
                                totcost += Function.ToInt32(group.ElementAt(i)["selfAmt"].ToString());
                            }
                            if (string.IsNullOrEmpty(reg.SEE_DPCD) || string.IsNullOrEmpty(mgr.GetDeptNameForCode(reg.SEE_DPCD)))
                            {
                                reg.SEE_DPCD = reg.DEPT_CODE;
                            }
                            string doctorCode = reg.SEE_DOCD;
                            if (mgr.GetEmployeeType(doctorCode) != "D")
                            {
                                doctorCode = reg.DOCT_CODE;
                            }
                            string mainxml = string.Format(RegdataXml,
                                                        reg.CLINIC_CODE,
                                                        reg.SEE_DPCD,
                                                        Function.XmlString(mgr.GetDeptNameForCode(reg.SEE_DPCD)).Replace("＆", "、"),
                                                        doctorCode,
                                                        mgr.GetEmplName(doctorCode),
                                                        totcost.ToString(),
                                                        reg.SEE_DATE,
                                                        reg.CLINIC_CODE,
                                                        reg.CLINIC_CODE,
                                                        group.Key,
                                                        group.ElementAt(0)["reciptType"].ToString()
                                                        );
                            resXml += mainxml + detialXml + "</bills></item>";
                        }
                    }
                    #region 原本
                    //if (dt.Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dt.Rows.Count; i++)
                    //    {
                    //        if (hsItemList.Contains(dt.Rows[i]["prescriptionId"].ToString()))//存在不可在自助设备缴费的项目
                    //        {
                    //            throw new Exception(erro + "费用项目包含了:" + dt.Rows[i]["itemName"].ToString() + ",请到人工窗口进行缴费，谢谢合作！");
                    //        }

                    //        detialXml += string.Format(ItemdataXml, dt.Rows[i]["prescriptionId"].ToString(),
                    //                                dt.Rows[i]["prescriptionType"].ToString(),
                    //                                dt.Rows[i]["selfAmt"].ToString(),
                    //                                dt.Rows[i]["itemName"].ToString(),
                    //                                dt.Rows[i]["RECIPE_NO"].ToString(),
                    //                                dt.Rows[i]["isStore"].ToString(),
                    //                                dt.Rows[i]["reciptType"].ToString()
                    //                                );
                    //        totcost += Function.ToInt32(dt.Rows[i]["selfAmt"].ToString());
                    //    }
                    //}
                    //// reg.DEPT_CODE,
                    //// Function.XmlString(reg.DEPT_NAME).Replace("＆","、"),           
                    //if (string.IsNullOrEmpty(reg.SEE_DPCD) || string.IsNullOrEmpty(mgr.GetDeptNameForCode(reg.SEE_DPCD)))
                    //{
                    //    reg.SEE_DPCD = reg.DEPT_CODE;
                    //}
                    //string mainxml = string.Format(RegdataXml,
                    //                            reg.CLINIC_CODE,
                    //                            reg.SEE_DPCD,
                    //                            Function.XmlString(mgr.GetDeptNameForCode(reg.SEE_DPCD)).Replace("＆", "、"),
                    //                            reg.SEE_DOCD,
                    //                            mgr.GetEmplName(reg.SEE_DOCD),
                    //                            totcost.ToString(),
                    //                            reg.SEE_DATE,
                    //                            reg.CLINIC_CODE,
                    //                            reg.CLINIC_CODE);
                    //resXml += mainxml + detialXml + "</bills></item>";
                    #endregion
                }
                string xml = Function.GetResponseXML(true, "操作成功", resXml);
                ServiceLogManager.Write(GUID + "传出报文：" + xml);
                return xml;

                #endregion
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                ServiceLogManager.Write(GUID + "传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "待缴费明细")]
        public string billDetail(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<item>
<feeType>{0}</feeType>
<itemId>{1}</itemId>
<feeName>{2}</feeName>
<itemName>{3}</itemName>
<unit>{4}</unit>
<count>{5}</count>
<price>{6}</price>
<spece>{7}</spece>
<amount>{8}</amount>
</item>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "visitNo", NodeInstruction = "就诊号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });

                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                string visitNo = nodesVales["visitNo"];
                string hospitalNum = nodesVales["hospitalNum"];

                string resXml = string.Empty;
                FS.ZDWY.Internet.BP.OutPatient.QueryManager mgr = new BP.OutPatient.QueryManager();
                string erro = "";
                DataTable dt = mgr.billDetail(visitNo, ref erro);
                if (dt == null)
                {
                    throw new Exception(erro + "没有缴费明细");
                }
                resXml = "";
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        resXml += string.Format(dataXml, dt.Rows[i]["feeType"].ToString(),
                                                dt.Rows[i]["itemId"].ToString(),
                                                dt.Rows[i]["feeName"].ToString(),
                                                dt.Rows[i]["itemName"].ToString(),
                                                dt.Rows[i]["unit"].ToString(),
                                                dt.Rows[i]["count"].ToString(),
                                                dt.Rows[i]["price"].ToString(),
                                                dt.Rows[i]["spece"].ToString(),
                                                dt.Rows[i]["amount"].ToString()
                                                );
                    }
                }
                //resXml = "<item><bills>" + resXml + "</bills></item>";
                string xml = Function.GetResponseXML(true, "操作成功", resXml);
                ServiceLogManager.Write("传出报文：" + xml);
                return xml;


                #endregion
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }


        [WebMethod(Description = "待缴费支付费用计算")]
        public string BillFeeCalculation(string req)
        {
            ServiceLogManager.Write("待缴费支付费用计算BillFeeCalculation传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<totFee>{0}</totFee>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }

                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                //reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "平台定单号" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "visitNo", NodeInstruction = "就诊号" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payMode", NodeInstruction = "支付方式" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payAmt", NodeInstruction = "支付金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payTime", NodeInstruction = "支付时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "patientId", NodeInstruction = "院内用户ID" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "certifcateType", NodeInstruction = "用户证件类型" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "certifcateNo", NodeInstruction = "用户证件号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "medicalNo", NodeInstruction = "病历号" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "cardType", NodeInstruction = "用户卡类型" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "cardNo", NodeInstruction = "用户卡号" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "frontProviderId", NodeInstruction = "第三方服务商 ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "hcareAmount", NodeInstruction = "医保减免" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "selfAmount", NodeInstruction = "自费金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "expenseAmount", NodeInstruction = "医保报销" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "totalAmount", NodeInstruction = "费用总金额" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "tranno", NodeInstruction = "就诊持卡号码" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "transactionNo", NodeInstruction = "收单机构流水号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "balanceNo", NodeInstruction = "医保结算序号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "regNo", NodeInstruction = "医保登记号" });
                //reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "payChannel", NodeInstruction = "支付渠道" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "reciptNo", NodeInstruction = "处方号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "reciptType", NodeInstruction = "处方类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "ydzf", NodeInstruction = "是否移动支付" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "psnAcctPay", NodeInstruction = "个人账户支出" });
                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                FS.ZDWY.Internet.Models.PLATFORM_BALANCE_PAY balance = new Models.PLATFORM_BALANCE_PAY();
                //balance.ORDERID = nodesVales["orderId"];
                //balance.HOSPITALNUM = nodesVales["hospitalNum"];
                balance.VISITNO = nodesVales["visitNo"];
                //balance.PAYMODE = nodesVales["payMode"];
                balance.PAYAMT = nodesVales["payAmt"];
                balance.PAYTIME = Function.ToDateTime(nodesVales["payTime"]);
                balance.PATIENTID = nodesVales["patientId"];
                //balance.CERTIFCATETYPE = nodesVales["certifcateType"];
                //balance.CERTIFCATENO = nodesVales["certifcateNo"];
                balance.MEDICALNO = nodesVales["medicalNo"];
                //balance.CARDTYPE = nodesVales["cardType"];
                //balance.CARDNO = nodesVales["cardNo"];
                //balance.FRONTPROVIDERID = nodesVales["frontProviderId"];
                balance.HCAREAMOUNT = Function.ToDecimal(nodesVales["hcareAmount"]);
                balance.SELFAMOUNT = Function.ToDecimal(nodesVales["selfAmount"]);
                balance.EXPENSEAMOUNT = Function.ToDecimal(nodesVales["expenseAmount"]);
                balance.TOTALAMOUNT = Function.ToDecimal(nodesVales["totalAmount"]);
                balance.PSNACCTPAY = Function.ToDecimal(nodesVales["psnAcctPay"]);
                //balance.TRANNO = nodesVales["tranno"];
                //balance.TRANSACTIONNO = nodesVales["transactionNo"];

                balance.OPERCODE = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                balance.OPERNAME = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                #endregion
                string regNo = nodesVales["regNo"];
                string balanceNo = nodesVales["balanceNo"];
                string reciptNo = nodesVales["reciptNo"];
                string reciptType = nodesVales["reciptType"];
                string ydzf = nodesVales["ydzf"];
                FS.ZDWY.Internet.BP.OutPatient.RegisterInfoManager regpay = new BP.OutPatient.RegisterInfoManager();
                decimal totFee = regpay.BillFeeCalculation(balance, FS.ZDWY.Internet.BP.Common.Function.ZFBOper, regNo, balanceNo, reciptType, reciptNo, ydzf);
                if (totFee > 0)
                {
                    string resXml = Function.GetResponseXML(true, "成功",
                        string.Format(dataXml, totFee * 100));
                    ServiceLogManager.Write("待缴费支付费用计算BillFeeCalculation传入报文：" + resXml);
                    return resXml;

                }
                else
                {
                    string resXml = Function.GetResponseXML(false, "订单金额小于0", "");
                    ServiceLogManager.Write("待缴费支付费用计算BillFeeCalculation传入报文：" + resXml);
                    return resXml;
                }
            }
            catch (Exception e)
            {

                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                ServiceLogManager.Write("待缴费支付费用计算BillFeeCalculation传入报文：" + resXml);
                ErrorLogManager.Write("待缴费支付费用计算BillFeeCalculation传入报文 :" + e.Message + "\r\nSource:" + e.Source + "\r\nStackTrace:" + e.StackTrace);
                return resXml;
            }
        }

        [WebMethod(Description = "待缴费支付")]
        public string billpay(string req)
        {
            ServiceLogManager.Write("待缴费支付billpay传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<hospTradeId>{0}</hospTradeId>
<visitNo>{1}</visitNo>
<invoiceId>{2}</invoiceId>
<receiptId>{3}</receiptId>
<visitAddress>{4}</visitAddress>
<sequenceNo>{5}</sequenceNo>
<remark>{6}</remark>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "平台定单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "visitNo", NodeInstruction = "就诊号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payMode", NodeInstruction = "支付方式" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payAmt", NodeInstruction = "支付金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "payTime", NodeInstruction = "支付时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "patientId", NodeInstruction = "院内用户ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "certifcateType", NodeInstruction = "用户证件类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "certifcateNo", NodeInstruction = "用户证件号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "medicalNo", NodeInstruction = "病历号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "cardType", NodeInstruction = "用户卡类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "cardNo", NodeInstruction = "用户卡号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "frontProviderId", NodeInstruction = "第三方服务商 ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "hcareAmount", NodeInstruction = "医保减免" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "selfAmount", NodeInstruction = "自费金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "expenseAmount", NodeInstruction = "医保报销" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "totalAmount", NodeInstruction = "费用总金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "tranno", NodeInstruction = "就诊持卡号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "transactionNo", NodeInstruction = "收单机构流水号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "balanceNo", NodeInstruction = "医保结算序号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "regNo", NodeInstruction = "医保登记号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "payChannel", NodeInstruction = "支付渠道" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "reciptNo", NodeInstruction = "处方号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "reciptType", NodeInstruction = "处方类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "ydzf", NodeInstruction = "是否移动支付" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "psnAcctPay", NodeInstruction = "个人账户支出" });                
                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                FS.ZDWY.Internet.Models.PLATFORM_BALANCE_PAY balance = new Models.PLATFORM_BALANCE_PAY();
                balance.ORDERID = nodesVales["orderId"];
                balance.HOSPITALNUM = nodesVales["hospitalNum"];
                balance.VISITNO = nodesVales["visitNo"];
                balance.PAYMODE = nodesVales["payMode"];
                balance.PAYAMT = nodesVales["payAmt"];
                balance.PAYTIME = Function.ToDateTime(nodesVales["payTime"]);
                balance.PATIENTID = nodesVales["patientId"];
                balance.CERTIFCATETYPE = nodesVales["certifcateType"];
                balance.CERTIFCATENO = nodesVales["certifcateNo"];
                balance.MEDICALNO = nodesVales["medicalNo"];
                balance.CARDTYPE = nodesVales["cardType"];
                balance.CARDNO = nodesVales["cardNo"];
                balance.FRONTPROVIDERID = nodesVales["frontProviderId"];
                balance.HCAREAMOUNT = Function.ToDecimal(nodesVales["hcareAmount"]);
                balance.SELFAMOUNT = Function.ToDecimal(nodesVales["selfAmount"]);
                balance.EXPENSEAMOUNT = Function.ToDecimal(nodesVales["expenseAmount"]);
                balance.TOTALAMOUNT = Function.ToDecimal(nodesVales["totalAmount"]);
                balance.PSNACCTPAY = Function.ToDecimal(nodesVales["psnAcctPay"]);
                balance.TRANNO = nodesVales["tranno"];
                balance.TRANSACTIONNO = nodesVales["transactionNo"];

                balance.OPERCODE = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                balance.OPERNAME = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;

                if (nodesVales["payChannel"] == "2") //支付渠道为支付宝
                {
                    balance.OPERCODE = FS.ZDWY.Internet.BP.Common.Function.ZFBOper.Code;
                    balance.OPERNAME = FS.ZDWY.Internet.BP.Common.Function.ZFBOper.Code;
                }
                else if (nodesVales["payChannel"] == "3") //支付渠道为APP
                {
                    balance.OPERCODE = FS.ZDWY.Internet.BP.Common.Function.APPOper.Code;
                    balance.OPERNAME = FS.ZDWY.Internet.BP.Common.Function.APPOper.Code;
                }
                #endregion
                string regNo = nodesVales["regNo"];
                string balanceNo = nodesVales["balanceNo"];
                string reciptNo = nodesVales["reciptNo"];
                string reciptType = nodesVales["reciptType"];
                string ydzf = nodesVales["ydzf"];

                FS.ZDWY.Internet.BP.OutPatient.RegisterInfoManager regpay = new BP.OutPatient.RegisterInfoManager();
                Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY> res = new Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY>();
                if (nodesVales["payChannel"] == "2") //支付渠道为支付宝
                {
                    res = regpay.BillPay(balance, FS.ZDWY.Internet.BP.Common.Function.ZFBOper, regNo, balanceNo, reciptType, reciptNo, ydzf);                
                }
                else if (nodesVales["payChannel"] == "3") //支付渠道为APP
                {
                    res = regpay.BillPay(balance, FS.ZDWY.Internet.BP.Common.Function.APPOper, regNo, balanceNo, reciptType, reciptNo, ydzf);                
                }
                else
                    res = regpay.BillPay(balance, FS.ZDWY.Internet.BP.Common.Function.DefaultOper, regNo, balanceNo, reciptType, reciptNo, ydzf);
                if (res.ReturnData != null)
                {
                    string resXml = Function.GetResponseXML(res.IsSuccessful, res.Message,
                        string.Format(dataXml, res.ReturnData.HOSPITALNUM,
                        res.ReturnData.VISITNO,
                         res.ReturnData.INVOICEID,
                          res.ReturnData.RECEIPTID,
                         res.ReturnData.VISITADDRESS,
                           res.ReturnData.SEQUENCENO,
                            res.ReturnData.REMARK));
                    ServiceLogManager.Write("待缴费支付billpay传出报文：" + resXml);
                    return resXml;

                }
                else
                {
                    string resXml = Function.GetResponseXML(false, res.Message, "");
                    ServiceLogManager.Write("待缴费支付billpay传出报文：" + resXml);
                    return resXml;
                }
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                ServiceLogManager.Write("待缴费支付billpay传出报文：" + resXml);
                ErrorLogManager.Write("待缴费支付billpay :" + e.Message + "\r\nSource:" + e.Source + "\r\nStackTrace:" + e.StackTrace);
                return resXml;
            }
        }



        [WebMethod(Description = "缴费订单退款")]
        public string refundNotice(string req)
        {
            return "";
            /*ServiceLogManager.Write("传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<refundFlag>{0}</refundFlag>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "平台定单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "patientId", NodeInstruction = "院内用户ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "refundId ", NodeInstruction = "业务系统退款单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "totalFee", NodeInstruction = "总 金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "refundFee", NodeInstruction = "退款金额" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "refundTime", NodeInstruction = "退款时间" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "prescriptionIds", NodeInstruction = "处方号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "prescriptionType", NodeInstruction = "处方类型" });

                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                FS.ZDWY.Internet.Models.PLATFORM_BALANCE_PAY balance = new Models.PLATFORM_BALANCE_PAY();
                balance.ORDERID = nodesVales["orderId"];
                balance.HOSPITALNUM = nodesVales["hospitalNum"];
                balance.PAYAMT = nodesVales["totalFee"];
                balance.PATIENTID = nodesVales["patientid"];
                balance.PRESCRIPTIONIDS = nodesVales["prescriptionids"];
                balance.PRESCRIPTIONTYPE = nodesVales["prescriptiontype"];
                balance.OPERID = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                balance.OPERNAME = FS.ZDWY.Internet.BP.Common.Function.DefaultOper.Code;
                #endregion

                FS.ZDWY.Internet.BP.OutPatient.RegisterInfoManager regpay = new BP.OutPatient.RegisterInfoManager();
                Models.Views.ComResult<string> res = new Models.Views.ComResult<string>();
                res = regpay.refundNotice(balance, FS.ZDWY.Internet.BP.Common.Function.DefaultOper);

                if (res.ReturnData != null)
                {
                    string resXml = Function.GetResponseXML(res.IsSuccessful, res.Message,
                        string.Format(dataXml, res.ReturnData));
                    return resXml;

                }
                else
                {
                    string resXml = Function.GetResponseXML(false, res.Message, "0");
                    return resXml;
                }
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                return resXml;
            }
             */
        }

        [WebMethod(Description = "缴费订单查询")]
        public string billstatus(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<billId>{0}</billId>
<receiptId>{1}</receiptId>
<invoiceId>{2}</invoiceId>
<visitNo>{3}</visitNo>
<totalFee>{4}</totalFee>
<realFee>{5}</realFee>
<isPay>{6}</isPay>
<payTime>{7}</payTime>
<isRefund>{8}</isRefund>
<isRebate>{9}</isRebate>
<refundTime>{10}</refundTime>
<refundFee>{11}</refundFee>
<isComplete>{12}</isComplete>
<completeTime>{13}</completeTime>
<hospTradeId>{14}</hospTradeId>
<visitAddress>{15}</visitAddress>
<sequenceNo>{15}</sequenceNo>
";
            //<billqrcode>{16}</billqrcode>
            //<pictureurl>{17}</pictureurl>
            //<pictureneturl>{18}</pictureneturl>
            //<random>{19}</random>
            #endregion

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "orderId", NodeInstruction = "平台定单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "visitNo", NodeInstruction = "就诊号" });

                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                string ORDERID = nodesVales["orderId"];
                string HOSPITALNUM = nodesVales["hospitalNum"];
                string VISITNO = nodesVales["visitNo"];

                string billqrcode = string.Empty;//二维码图片数据
                string pictureurl = string.Empty;//内网地址
                string pictureneturl = string.Empty;//外网地址
                string random = string.Empty;//校验码

                Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY> res = new Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY>();
                BP.OutPatient.QueryManager que = new BP.OutPatient.QueryManager();
                res = que.GetBillPayInfo(ORDERID);
                if (res.ReturnData != null)
                {
                    string erro = string.Empty;
                    DataTable dt = que.GetElecBillInfo(res.ReturnData.INVOICEID, "2", ref erro);
                    if (dt != null)
                    {
                        billqrcode = dt.Rows[0]["billqrcode"].ToString();
                        pictureurl = dt.Rows[0]["pictureurl"].ToString();
                        pictureneturl = dt.Rows[0]["pictureneturl"].ToString();
                        random = dt.Rows[0]["random"].ToString();
                    }
                    string resXml = Function.GetResponseXML(res.IsSuccessful, res.Message,
                           string.Format(dataXml, res.ReturnData.ORDERID,
                           res.ReturnData.RECEIPTID,
                           res.ReturnData.INVOICEID,
                           res.ReturnData.VISITNO,
                           res.ReturnData.PAYAMT,
                           res.ReturnData.TOTALAMOUNT,
                           "1",
                           res.ReturnData.PAYTIME.ToLongTimeString(),
                           res.ReturnData.STATUS,
                           "0",
                           res.ReturnData.REFUNDTIME.ToLongTimeString(),
                           res.ReturnData.TOTALAMOUNT,
                           "",
                           res.ReturnData.PAYTIME.ToLongTimeString(),
                           string.IsNullOrEmpty(res.ReturnData.INVOICEID) ? "" : res.ReturnData.INVOICEID,
                           string.IsNullOrEmpty(res.ReturnData.VISITADDRESS) ? "无" : res.ReturnData.VISITADDRESS,
                           string.IsNullOrEmpty(res.ReturnData.SEQUENCENO) ? "无" : res.ReturnData.SEQUENCENO
                        //,billqrcode,
                        //pictureurl,
                        //pictureneturl,
                        //random
                           )
                           );
                    ServiceLogManager.Write("传出报文：" + resXml);
                    return resXml;
                }
                else
                {
                    res.ReturnData = new Models.PLATFORM_BALANCE_PAY();
                    res.IsSuccessful = true;
                    res.Message = "订单号：" + ORDERID + "没有查到缴费记录";
                    string resXml = Function.GetResponseXML(res.IsSuccessful, res.Message,
                      string.Format(dataXml, res.ReturnData.ORDERID,
                      res.ReturnData.RECEIPTID,
                      res.ReturnData.INVOICEID,
                      res.ReturnData.VISITNO,
                      res.ReturnData.PAYAMT,
                      res.ReturnData.TOTALAMOUNT,
                      "0",
                      res.ReturnData.PAYTIME.ToLongTimeString(),
                      res.ReturnData.STATUS,
                      "0",
                      res.ReturnData.REFUNDTIME.ToLongTimeString(),
                      res.ReturnData.TOTALAMOUNT,
                      "",
                      res.ReturnData.PAYTIME.ToLongTimeString(),
                      string.IsNullOrEmpty(res.ReturnData.INVOICEID) ? "" : res.ReturnData.INVOICEID,
                      string.IsNullOrEmpty(res.ReturnData.VISITADDRESS) ? "无" : res.ReturnData.VISITADDRESS,
                      string.IsNullOrEmpty(res.ReturnData.SEQUENCENO) ? "无" : res.ReturnData.SEQUENCENO)
                      );
                    ServiceLogManager.Write("传出报文：" + resXml);
                    return resXml;
                    //else
                    //{
                    //    string resXml = Function.GetResponseXML(res.IsSuccessful, res.Message,
                    //       "");
                    //    ServiceLogManager.Write("传出报文：" + resXml);
                    //    return resXml;
                    //}

                }
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, e.Message.ToString(),
                       "");
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "缴费医保减免")]
        public string billhcare(string req)
        {
            ServiceLogManager.Write("缴费医保减免billhcare传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<hcareAmount>{0}</hcareAmount>
<selfAmount>{1}</selfAmount>
<expenseAmount>{2}</expenseAmount>
<totalAmount>{3}</totalAmount>
<balanceNo>{5}</balanceNo>
<regNo>{6}</regNo>
<remark>{4}</remark>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "hospitalNum", NodeInstruction = "医院订单号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "visitNo", NodeInstruction = "就诊号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "patientId", NodeInstruction = "院内用户ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "tranno", NodeInstruction = "就诊持卡号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "patientName", NodeInstruction = "就诊人名称" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "patientCard", NodeInstruction = "就诊人卡号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "frontProviderId", NodeInstruction = "第三方服务商 ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "MdtrCertType", NodeInstruction = "就诊凭证类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "MdtrtCertNo", NodeInstruction = "就诊凭证编号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "PsnCertType", NodeInstruction = "人员证件类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "InsuplcAdmdvs", NodeInstruction = "参保区划" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "CardSN", NodeInstruction = "卡识别码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "CertNo", NodeInstruction = "证件号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "settlementType", NodeInstruction = "减免方式" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "reciptType", NodeInstruction = "处方类型" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "reciptNo", NodeInstruction = "处方号" });           
                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                string HOSPITALNUM = nodesVales["hospitalNum"];
                string VISITNO = nodesVales["visitNo"];
                string PATIENTID = nodesVales["patientId"];
                string TRANNO = nodesVales["tranno"];
                string PATIENTNAME = nodesVales["patientName"];
                string PATIENTCARD = nodesVales["patientCard"];
                string FRONTPROVIDERID = nodesVales["frontProviderId"];
                string settlementType = nodesVales["settlementType"];
                string reciptType = nodesVales["reciptType"];
                string reciptNo = nodesVales["reciptNo"];               
                FS.ZDWY.Internet.Models.Views.QueryPersonRequestModel reqModel = new FS.ZDWY.Internet.Models.Views.QueryPersonRequestModel();
                reqModel.MdtrCertTyp = nodesVales["MdtrCertType"];
                reqModel.MdtrtCertNo = nodesVales["MdtrtCertNo"];
                reqModel.PsnCertType = nodesVales["PsnCertType"];
                reqModel.InsuplcAdmdvs = nodesVales["InsuplcAdmdvs"];
                reqModel.CardSN = nodesVales["CardSN"];
                reqModel.CertNo = nodesVales["CertNo"];

                #endregion

                FS.ZDWY.Internet.BP.OutPatient.RegisterInfoManager regpay = new BP.OutPatient.RegisterInfoManager();
                Models.Views.ComResult<Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();
                result = regpay.BillHcare(HOSPITALNUM, VISITNO, PATIENTID, TRANNO, PATIENTNAME, PATIENTCARD, FRONTPROVIDERID, reciptType, reciptNo, reqModel, settlementType);

                if (result.ReturnData != null)
                {
                    string resXml = Function.GetResponseXML(true, "操作成功",
                                       string.Format(dataXml, ((int)result.ReturnData.HcareAmount).ToString(), ((int)result.ReturnData.SelfAmount).ToString(), ((int)result.ReturnData.ExpenseAmount).ToString(), ((int)result.ReturnData.TotalAmount).ToString(), result.ReturnData.Remark, result.ReturnData.BalanceNo, result.ReturnData.regNO)
                                       );
                    ServiceLogManager.Write("缴费医保减免billhcare传出报文：" + resXml);
                    return resXml;
                }
                else
                {
                    string resXml = Function.GetResponseXML(false, result.Message, "");
                    ServiceLogManager.Write("缴费医保减免billhcare传出报文：" + resXml);
                    return resXml;
                }
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                ServiceLogManager.Write("缴费医保减免billhcare传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "取消缴费医保减免")]
        public string cancelbillhcare(string req)
        {
            ServiceLogManager.Write("取消缴费医保减免cancelbillhcare传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "visitNo", NodeInstruction = "就诊号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "patientId", NodeInstruction = "院内用户ID" });
                // reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "tranno", NodeInstruction = "就诊持卡号码" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "patientName", NodeInstruction = "就诊人名称" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = false, NodeName = "patientCard", NodeInstruction = "就诊人卡号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "frontProviderId", NodeInstruction = "第三方服务商 ID" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "balanceNo", NodeInstruction = "医保结算序号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "regNo", NodeInstruction = "医保就医登记号" });

                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                //string HOSPITALNUM = nodesVales["hospitalNum"];
                string VISITNO = nodesVales["visitNo"];
                string PATIENTID = nodesVales["patientId"];
                //string TRANNO = nodesVales["tranno"];
                string PATIENTNAME = nodesVales["patientName"];
                string PATIENTCARD = nodesVales["patientCard"];
                string FRONTPROVIDERID = nodesVales["frontProviderId"];
                string balanceNo = nodesVales["balanceNo"];
                string regNO = nodesVales["regNo"];
                #endregion

                FS.ZDWY.Internet.BP.OutPatient.RegisterInfoManager regpay = new BP.OutPatient.RegisterInfoManager();
                Models.Views.ComResult<Models.Views.OutPatient.HcareResult> result = new Models.Views.ComResult<Models.Views.OutPatient.HcareResult>();
                result = regpay.Cancelbillhcare(VISITNO, regNO, balanceNo);

                if (result.ReturnData != null)
                {
                    string resXml = Function.GetResponseXML(true, "操作成功", "");
                    ServiceLogManager.Write("取消缴费医保减免cancelbillhcare传出报文：" + resXml);
                    return resXml;
                }
                else
                {
                    string resXml = Function.GetResponseXML(false, result.Message, "");
                    ServiceLogManager.Write("取消缴费医保减免cancelbillhcare传出报文" + resXml);
                    return resXml;
                }
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                ServiceLogManager.Write("取消缴费医保减免cancelbillhcare传出报文" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "已缴费明细")]
        public string sucDetail(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<item>
<feeType>{0}</feeType>
<itemId>{1}</itemId>
<feeName>{2}</feeName>
<itemName>{3}</itemName>
<unit>{4}</unit>
<count>{5}</count>
<price>{6}</price>
<spece>{7}</spece>
<amount>{8}</amount>
</item>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "visitNo", NodeInstruction = "就诊号" });
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "hospTradeId", NodeInstruction = "医院支付单号" });

                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }

                string visitNo = nodesVales["visitNo"];
                string hospTradeId = nodesVales["hospTradeId"];

                string resXml = string.Empty;
                FS.ZDWY.Internet.BP.OutPatient.QueryManager mgr = new BP.OutPatient.QueryManager();
                string erro = "";
                DataTable dt = mgr.billPayDetail(visitNo, hospTradeId, ref erro);
                if (dt == null)
                {
                    throw new Exception(erro + "没有缴费明细");
                }
                resXml = "";
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        resXml += string.Format(dataXml, dt.Rows[i]["feeType"].ToString(),
                                                dt.Rows[i]["itemId"].ToString(),
                                                dt.Rows[i]["feeName"].ToString(),
                                                dt.Rows[i]["itemName"].ToString(),
                                                dt.Rows[i]["unit"].ToString(),
                                                dt.Rows[i]["count"].ToString(),
                                                dt.Rows[i]["price"].ToString(),
                                                dt.Rows[i]["spece"].ToString(),
                                                dt.Rows[i]["amount"].ToString()
                                                );
                    }
                }
                //resXml = "<item><bills>" + resXml + "</bills></item>";
                string xml = Function.GetResponseXML(true, "操作成功", resXml);
                ServiceLogManager.Write("传出报文：" + xml);
                return xml;


                #endregion
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, "操作失败！" + e.Message.ToString(), "");
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }
        #endregion

        #region 信息查询
        [WebMethod(Description = "预约就诊提醒")]
        public string remind(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }

                FS.ZDWY.Internet.BP.OutPatient.QueryManager quer = new BP.OutPatient.QueryManager();
                System.Data.DataTable dtRes = quer.BookRemind();
                if (dtRes == null)
                {
                    throw new Exception("查询预约就诊提醒失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有预约就诊提醒信息");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    dataXml.Append("<item>");
                    for (int j = 0; j < dtRes.Columns.Count; j++)
                    {
                        if (dtRes.Columns[j].DataType.Name == "DateTime")
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j].ToString().Replace('/', '-'));
                        }
                        else
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j]);
                        }
                    }
                    dataXml.Append("</item>");
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "停诊通知")]
        public string stop(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }

                FS.ZDWY.Internet.BP.OutPatient.QueryManager quer = new BP.OutPatient.QueryManager();
                System.Data.DataTable dtRes = quer.StopSchedulRemind();
                if (dtRes == null)
                {
                    //throw new Exception("查询停诊信息失败");
                    string res = Function.GetResponseXML(true, "操作成功", "");
                    ServiceLogManager.Write("传出报文：" + res);
                    return res;
                }
                if (dtRes.Rows.Count <= 0)
                {
                    //throw new Exception("没有停诊信息");
                    string res = Function.GetResponseXML(true, "操作成功", "");
                    ServiceLogManager.Write("传出报文：" + res);
                    return res;
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    dataXml.Append("<item>");
                    for (int j = 0; j < dtRes.Columns.Count; j++)
                    {
                        if (dtRes.Columns[j].DataType.Name == "DateTime")
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j].ToString().Replace('/', '-'));
                        }
                        else
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j]);
                        }
                    }
                    dataXml.Append("</item>");
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "查询锁号")]
        public string appoOrder(string req)
        {
            ServiceLogManager.Write("查询锁号传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                #region 获取入参值并验证
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                List<Models.ReqNode> reqNodes = new List<Models.ReqNode>();
                reqNodes.Add(new Models.ReqNode() { IsRequired = true, NodeName = "patientId", NodeInstruction = "门诊号" });

                Dictionary<string, string> nodesVales = new Dictionary<string, string>();
                foreach (var item in reqNodes)
                {
                    item.NodeValue = Function.GetNoteValue(xmlDoc, "Request/data/" + item.NodeName);
                    if (item.IsRequired)
                    {
                        Function.ValidateParameter(item.NodeValue, item.NodeInstruction);
                    }
                    nodesVales.Add(item.NodeName, item.NodeValue);
                }
                string patientId = nodesVales["patientId"];
                #endregion

                FS.ZDWY.Internet.BP.OutPatient.QueryManager query = new BP.OutPatient.QueryManager();
                System.Data.DataTable dtRes = query.QueryBookingRegInfo(patientId);
                if (dtRes == null)
                {
                    string res = Function.GetResponseXML(true, "操作成功", "");
                    ServiceLogManager.Write("查询锁号传出报文：" + res);
                    return res;
                }
                if (dtRes.Rows.Count <= 0)
                {
                    string res = Function.GetResponseXML(true, "操作成功", "");
                    ServiceLogManager.Write("查询锁号传出报文：" + res);
                    return res;
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    for (int j = 0; j < dtRes.Columns.Count; j++)
                    {
                        dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j]);
                    }
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("查询锁号传出报文：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("查询锁号传出报文：" + resXml);
                return resXml;
            }
        }
        #endregion

    }
}
