using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// InpatientPrepay 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Deposit : System.Web.Services.WebService
    {
        FS.ZDWY.Internet.BP.InPatient.Deposit prepayManager;

        FS.ZDWY.Internet.BP.InPatient.Deposit PrepayManager
        {
            get
            {
                if (prepayManager == null)
                {
                    prepayManager = new BP.InPatient.Deposit();
                }
                return prepayManager;
            }
        }

        FS.ZDWY.Internet.BP.InPatient.InMainInfoManager inMainInfoManager;

        FS.ZDWY.Internet.BP.InPatient.InMainInfoManager InMainInfoManager
        {
            get
            {
                if (inMainInfoManager == null)
                {
                    inMainInfoManager = new BP.InPatient.InMainInfoManager();
                }
                return inMainInfoManager;
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

        /// <summary>
        /// 预交金记录查询
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [WebMethod(Description = "预交金记录查询")]
        public string Record(string req)
        {
            #region 入参模板

            //Request><data>
            //<patientId></patientId>
            //<admissionNo></admissionNo>
            //<certifcateType></certifcateType>
            //<certifcateNo></certifcateNo>
            //<cardType></cardType>
            //<cardNo></cardNo>
            //<startDate></startDate>
            //<endDate></endDate>
            //</data></Request>

            #endregion
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string patientId = Function.GetNoteValue(xmlDoc, "Request/data/patientId");  //患者ID
                Function.ValidateParameter(patientId, "患者ID");
                string admissionNo = Function.GetNoteValue(xmlDoc, "Request/data/inpatId");  //住院号ID
                string certifcateType = Function.GetNoteValue(xmlDoc, "Request/data/certifcateType");  //证件类型
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");  //证件号
                string cardType = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");  //卡类型
                string cardNo = Function.GetNoteValue(xmlDoc, "Request/data/cardNo");  //卡号
                DateTime startDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/startDate").ToString());  //开始日期
                Function.ValidateParameter(startDate.ToString(), "开始日期");
                DateTime endDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/endDate").ToString());  //结束日期
                Function.ValidateParameter(endDate.ToString(), "结束日期");
                System.Data.DataTable dtRes = this.PrepayManager.QueryInPrepay(patientId, admissionNo, startDate, endDate);
                if (dtRes == null)
                {
                    throw new Exception("查找住院预交金列表失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查找到院预交金列表");
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

        /// <summary>
        /// 预交金订单信息查询
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [WebMethod(Description = "预交金订单信息查询")]
        public string Query(string req)
        {
            #region 入参模板

            //<Request><data>
            //<chargeId></chargeId>
            //<hospChargeId></hospChargeId>
            //</data></Request>

            #endregion

            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string chargeId = Function.GetNoteValue(xmlDoc, "Request/data/chargeId");  //业务系统押金单号
                Function.ValidateParameter(chargeId, "业务系统押金单号");
                string hospChargeId = Function.GetNoteValue(xmlDoc, "Request/data/hospChargeId");  //院内押金单号
                //Function.ValidateParameter(chargeId, "院内押金单号");
                string returnDataErr = @"<hospChargeId></hospChargeId>
	<receiptId></receiptId>
	<invoiceId></invoiceId>
	<amount></amount>
	<chargeChannel></chargeChannel>
	<chargeType></chargeType>
	<chargeTime></chargeTime>
	<status>2</status>
	<remark></remark>";

                if (string.IsNullOrEmpty(hospChargeId))
                {
                    hospChargeId = this.PrepayManager.GetHosChargeID(chargeId);
                }

                if (!string.IsNullOrEmpty(hospChargeId))
                {
                    System.Data.DataTable dtRes = null;
                    if (!hospChargeId.Contains("-"))
                    {
                        hospChargeId = this.PrepayManager.GetHosChargeID(chargeId);
                    }

                    if (!hospChargeId.Contains("-"))
                    {
                        //throw new Exception("业务系统押金单号入参格式不正确");
                        string resXmlTmp = Function.GetResponseXML(true, "业务系统押金单号入参格式不正确", returnDataErr);
                        ServiceLogManager.Write("传出报文：" + resXmlTmp);
                        return resXmlTmp;
                    }

                    string inpatientNo = hospChargeId.Split('-')[0];

                    string happenNo = hospChargeId.Split('-')[1];

                    {
                        dtRes = this.PrepayManager.QueryInPrepay(inpatientNo, happenNo, hospChargeId);
                        if (dtRes == null)
                        {
                            string resXmlTmp = Function.GetResponseXML(true, "查找住院预交金列表失败", returnDataErr);
                            ServiceLogManager.Write("传出报文：" + resXmlTmp);
                            return resXmlTmp;
                            //throw new Exception("查找住院预交金列表失败");
                        }
                        if (dtRes.Rows.Count <= 0)
                        {
                            string resXmlTmp = Function.GetResponseXML(true, "没有查找到院预交金列表", returnDataErr);
                            ServiceLogManager.Write("传出报文：" + resXmlTmp);
                            return resXmlTmp;
                            //throw new Exception("没有查找到院预交金列表");
                        }

                    }


                    System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                    for (int i = 0; i < dtRes.Rows.Count; i++)
                    {
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
                    }
                    string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                    ServiceLogManager.Write("传出报文：" + resXml);
                    return resXml;
                }
                else
                {
                    string resXml = Function.GetResponseXML(true, "业务系统押金单号为空", returnDataErr);
                    ServiceLogManager.Write("传出报文：" + resXml);
                    return resXml;
                }
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "住院预交金")]
        public string Pay(string req)
        {
            #region 入参模板

            //<Request><data>
            //<chargeId></chargeId>
            //<transactionNo></transactionNo>
            //<chargeTime></chargeTime>
            //<chargeChannel></chargeChannel>
            //<chargeType></chargeType>
            //<amount></amount>
            //<patientId></patientId>
            //<admissionNo></admissionNo>
            //<certifcateType></certifcateType>
            //<certifcateNo></certifcateNo>
            //<cardType></cardType>
            //<cardNo></cardNo>
            //<name></name>
            //<source></source>
            //<payChannel></payChannel>
            //</data></Request>

            #endregion

            ServiceLogManager.Write("传入报文：" + req);

            string resXml = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string chargeId = Function.GetNoteValue(xmlDoc, "Request/data/chargeId");  //业务系统押金单号
                Function.ValidateParameter(chargeId, "业务系统押金单号");
                string transactionNo = Function.GetNoteValue(xmlDoc, "Request/data/transactionNo");  //支付平台支付流水
                Function.ValidateParameter(transactionNo, "支付平台支付流水");
                string chargeTime = Function.GetNoteValue(xmlDoc, "Request/data/chargeTime");//预交时间
                Function.ValidateParameter(chargeTime, "预交时间");
                string chargeChannel = Function.GetNoteValue(xmlDoc, "Request/data/chargeChannel");//预交渠道
                Function.ValidateParameter(chargeChannel, "预交渠道");
                string chargeType = Function.GetNoteValue(xmlDoc, "Request/data/chargeType");//充值类型
                Function.ValidateParameter(chargeType, "充值类型");
                decimal amount = Function.ToDecimal(Function.GetNoteValue(xmlDoc, "Request/data/amount"));//预交金额
                Function.ValidateParameter(amount.ToString(), "预交金额");
                string patientId = Function.GetNoteValue(xmlDoc, "Request/data/patientId");//院内用户id
                string admissionNo = Function.GetNoteValue(xmlDoc, "Request/data/inpatId");//用户住院号
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");//用户证件号码
                string name = Function.GetNoteValue(xmlDoc, "Request/data/name");//姓名
                Function.ValidateParameter(name, "姓名");

                string visitNo = Function.GetNoteValue(xmlDoc, "Request/data/inpatNumber");//住院流水号
                Function.ValidateParameter(visitNo, "住院流水号");

                string frontProviderId = Function.GetNoteValue(xmlDoc, "Request/data/frontProviderId");//第三方服务商
                Function.ValidateParameter(frontProviderId, "第三方服务商");

                string payChannel = Function.GetNoteValue(xmlDoc, "Request/data/payChannel");//支付渠道
                Function.ValidateParameter(frontProviderId, "支付渠道");
                if (string.IsNullOrEmpty(patientId) && string.IsNullOrEmpty(admissionNo) && string.IsNullOrEmpty(certifcateNo))
                {
                    Function.ValidateParameter(string.Empty, "院内用户id/住院号/身份证号必须填一项");
                }

                string platformOrderNo = "";
                string applicationOrderNo = "";
                try
                {
                    platformOrderNo = Function.GetNoteValue(xmlDoc, "Request/data/PlatformOrderNo");
                    applicationOrderNo = Function.GetNoteValue(xmlDoc, "Request/data/ApplicationOrderNo");
                }
                catch (Exception)
                {

                }



                List<FS.ZDWY.Internet.Models.FIN_IPR_INMAININFO> inMainInfoList = this.InMainInfoManager.QueryInMainInfoList(patientId, admissionNo, visitNo, certifcateNo, name);

                if (inMainInfoList == null || inMainInfoList.Count == 0)
                {
                    resXml = Function.GetResponseXML(false, "指定入参未找到有效的住院信息", string.Empty);
                    ServiceLogManager.Write("传出报文：" + resXml);
                    return resXml;
                }

                FS.ZDWY.Internet.Models.FIN_IPR_INMAININFO model = inMainInfoList[0];

                if (!(model.IN_STATE == "R" || model.IN_STATE == "I" || model.IN_STATE == "B"))
                {
                    resXml = Function.GetResponseXML(false, "住院号:" + model.PATIENT_NO + "的患者非在院状态，请确认", string.Empty);
                    ServiceLogManager.Write("传出报文：" + resXml);
                    return resXml;
                }

                FS.ZDWY.Internet.Models.FIN_IPB_INPREPAY prepayModel = new Models.FIN_IPB_INPREPAY();

                string errInfo = string.Empty;

                string OperCode = Function.DefaultOper.Code;
                if (payChannel == "2")
                    OperCode = Function.ZFBOper.Code;
                else if (payChannel == "3")
                    OperCode = Function.APPOper.Code;
                int rev = this.PrepayManager.PrePay(model, chargeId, transactionNo, chargeTime, chargeChannel, chargeType, amount, OperCode,applicationOrderNo,platformOrderNo, out prepayModel, out errInfo);
                if (rev <= 0)
                {
                    resXml = Function.GetResponseXML(false, errInfo, string.Empty);
                    ServiceLogManager.Write("传出报文：" + resXml);
                    return resXml;
                }

                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                dataXml.AppendFormat("<{0}>{1}</{0}>", "hospChargeId", prepayModel.INPATIENT_NO + "-" + prepayModel.HAPPEN_NO);
                dataXml.AppendFormat("<{0}>{1}</{0}>", "amount", model.FREE_COST + prepayModel.PREPAY_COST);
                dataXml.AppendFormat("<{0}>{1}</{0}>", "receiptId", prepayModel.RECEIPT_NO);
                dataXml.AppendFormat("<{0}>{1}</{0}>", "invoiceId", prepayModel.RECEIPT_NO);
                dataXml.AppendFormat("<{0}>{1}</{0}>", "remark", prepayModel.INVOICE_NO);

                resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + dataXml);
                return resXml;
            }
            catch (Exception ex)
            {
                resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }


        [WebMethod(Description = "测试")]
        public string Test(string req)
        {
            return string.Empty;
        }
    }
}
