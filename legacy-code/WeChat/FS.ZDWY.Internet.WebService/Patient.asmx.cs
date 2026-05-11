using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Data;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// Patient 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Patient : System.Web.Services.WebService
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


        BP.OutPatient.PatientInfoManager outPatientManager;
        /// <summary>
        /// 返回的患者信息查询
        /// </summary>
        /// <returns></returns>
        BP.OutPatient.PatientInfoManager OutPatientManager
        {
            get
            {
                if (outPatientManager == null)
                {
                    outPatientManager = new BP.OutPatient.PatientInfoManager();
                }
                return outPatientManager;
            }
        }
        BP.OutPatient.QueryManager outPatientqueryManager;
        /// <summary>
        /// 门诊查询
        /// </summary>
        /// <returns></returns>
        BP.OutPatient.QueryManager OutPatientQueryManager
        {
            get
            {
                if (outPatientqueryManager == null)
                {
                    outPatientqueryManager = new BP.OutPatient.QueryManager();
                }
                return outPatientqueryManager;
            }
        }
        BP.InPatient.QueryManager queryManager;
        /// <summary>
        /// 患者住院就诊记录
        /// </summary>
        BP.InPatient.QueryManager QueryManager
        {
            get
            {
                if (queryManager == null)
                {
                    queryManager = new BP.InPatient.QueryManager();
                }
                return queryManager;
            }
        }

        [WebMethod(Description = "患者建档")]
        public string CreateFile(string req)
        {
            #region 入参模板
            /*
            <Request>
                <data>
                    <type></type>
                    <name></name>
                    <sex></sex>
                    <age></age>
                    <birth></birth>
                    <address></address>
                    <mobile></mobile>
                    <certifcateType></certifcateType>
                    <certifcateNo></certifcateNo>
                    <guardName></guardName>
                    <guardidType></guardidType>
                    <guardidNo></guardidNo>
                    <payChannel></payChannel>
                </data>
            </Request>
            */
            #endregion
            ServiceLogManager.Write("传入报文：" + req);
            #region 出参数据报文模板
            string dataXml = @"<cardType>{0}</cardType>
        <cardNo>{1}</cardNo>
        <patientId>{2}</patientId>
        <medicalNo>{3}</medicalNo>
        <createTime>{4}</createTime>
        <admissionNo>{5}</admissionNo>
        <visitNo>{6}</visitNo>";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                string type = Function.GetNoteValue(xmlDoc, "Request/data/type");  //患者类型	1成人 2儿童
                Function.ValidateParameter(type, "患者类型");
                string name = Function.GetNoteValue(xmlDoc, "Request/data/name"); //姓名
                Function.ValidateParameter(name, "姓名");
                string sex = Function.GetNoteValue(xmlDoc, "Request/data/sex"); //性别  1男 2女 9未知
                Function.ValidateParameter(sex, "性别");
                string age = Function.GetNoteValue(xmlDoc, "Request/data/age"); //年龄                
                string birth = Function.GetNoteValue(xmlDoc, "Request/data/birth"); //出生日期 YYYY-MM-DD
                Function.ValidateParameter(birth, "出生日期");
                string address = Function.GetNoteValue(xmlDoc, "Request/data/address"); //地址
                string mobile = Function.GetNoteValue(xmlDoc, "Request/data/mobile"); //电话
                Function.ValidateParameter(mobile, "电话");
                string certifcateType = Function.GetNoteValue(xmlDoc, "Request/data/certifcateType"); //证件类型
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo"); //证件号码  患者类型为2是可不填
                if (type == "1")
                {
                    Function.ValidateParameter(certifcateType, "证件类型");
                    Function.ValidateParameter(certifcateNo, "证件号码");
                }
                string guardName = Function.GetNoteValue(xmlDoc, "Request/data/guardName"); //监护人姓名  患者类型为2填写
                string guardidType = Function.GetNoteValue(xmlDoc, "Request/data/guardidType"); //监护人证件类型  患者类型为2填写
                string guardidNo = Function.GetNoteValue(xmlDoc, "Request/data/guardidNo"); //监护人证件号码  患者类型为2填写
                string country = Function.GetNoteValue(xmlDoc, "Request/data/country"); //国籍编号
                string gatxzNum = Function.GetNoteValue(xmlDoc, "Request/data/gatxzNum"); //港澳台身份证
                string payChannel = Function.GetNoteValue(xmlDoc, "Request/data/payChannel"); //支付渠道

                FS.ZDWY.Internet.Models.COM_PATIENTINFO patient = new Models.COM_PATIENTINFO();
                patient.NAME = name;
                patient.SEX_CODE = Function.ConvertHISSexCode(sex);
                patient.BIRTHDAY = Function.ToDateTime(birth);
                patient.HOME_NOW = address;
                patient.HOME_TEL = mobile;
                patient.IDCARDTYPE = certifcateType.PadLeft(2, '0');
                patient.IDCARDTYPE = Function.ConvertHisCardTypeCode(certifcateType);
                patient.IDENNO = certifcateNo;
                patient.LINKMAN_NAME = guardName;
                //新增字段
                patient.CARD_NO = PatientManager.GetCardNo();
                patient.PACT_CODE = "1";
                patient.PACT_NAME = "现金";
                patient.PAYKIND_CODE = "01";
                patient.OPER_CODE = Function.DefaultOper.Code;
                if (payChannel == "2")//支付渠道为支付宝 操作人更改操作人工号
                    patient.OPER_CODE = Function.ZFBOper.Code;
                else if (payChannel == "3")//支付渠道为支付宝 操作人更改操作人工号
                    patient.OPER_CODE = Function.APPOper.Code;
                patient.OPER_DATE = Function.ToDateTime(PatientManager.GetSysTime());
                patient.IS_VALID = "1";
                patient.PATIENT_TYPE = "1";
                patient.VIP_FLAG = "0";
                patient.GUARDIDNO = guardidNo;
                patient.COUN_CODE = country;
                //patient.LINKMAN_TEL=
                //港澳台身份证
                patient.INSURANCE_NAME = gatxzNum;
                string error = string.Empty;
                FS.ZDWY.Internet.Models.COM_PATIENTINFO patientReturn = new Models.COM_PATIENTINFO();
                int res = PatientManager.InsertPatientInfo(patient, ref error, ref patientReturn);
                //if (res <= 0)
                //{
                //    throw new Exception(error);
                //}
                string resXml = string.Empty;
                if (patientReturn != null)
                {
                    resXml = Function.GetResponseXML(true, "操作成功", string.Format(dataXml, 1, patientReturn.CARD_NO, patientReturn.CARD_NO, "", patientReturn.OPER_DATE, patientReturn.OLD_CARDNO, patientReturn.CARD_NO));  //默认是诊疗卡
                }
                else
                {
                    resXml = Function.GetResponseXML(true, "操作成功", string.Format(dataXml, 1, patient.CARD_NO, patient.CARD_NO, "", patient.OPER_DATE, "", patient.CARD_NO));  //默认是诊疗卡
                }

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


        [WebMethod(Description = "患者信息查询")]
        public string OutpatientQuery(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                string patientId = Function.GetNoteValue(xmlDoc, "Request/data/patientId");//院内用户Id
                //Function.ValidateParameter(patientId, "院内用户Id");
                string medicalNo = Function.GetNoteValue(xmlDoc, "Request/data/medicalNo");//病历号
                string certifcateType = Function.GetNoteValue(xmlDoc, "Request/data/certifcateType");//证件类型
                //Function.ValidateParameter(certifcateType, "证件类型");
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");//证件号码
                //Function.ValidateParameter(certifcateNo, "证件号码");
                string cardType = Function.GetNoteValue(xmlDoc, "Request/data/cardType");//用户卡类型
                string cardNo = Function.GetNoteValue(xmlDoc, "Request/data/cardNo");//用户卡号
                certifcateType = Function.ConvertHisCardTypeCode(certifcateType);
                if (string.IsNullOrEmpty(patientId))
                {
                    throw new Exception("patientId不允许为空！");
                }
                DataTable dpRes = OutPatientQueryManager.QueryPatientList(patientId, medicalNo, certifcateType, certifcateNo);

                if (dpRes == null)
                {
                    throw new Exception("患者信息查询失败");
                }
                if (dpRes.Rows.Count <= 0)
                {
                    throw new Exception("没有找到患者信息");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dpRes.Rows.Count; i++)
                {
                    //dataXml.Append("<item>");
                    for (int j = 0; j < dpRes.Columns.Count; j++)
                    {
                        if (dpRes.Rows[i][j] != null)
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dpRes.Columns[j], dpRes.Rows[i][j].ToString().Trim());
                        }
                        else
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dpRes.Columns[j], dpRes.Rows[i][j]);
                        }
                    }
                    //dataXml.Append("</item>");
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, e.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;

            }
        }

        [WebMethod(Description = "患者就诊卡是否有效")]
        public string CardEabled(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            string dataXml = "<eabled>{0}</eabled>";
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                string patientId = Function.GetNoteValue(xmlDoc, "Request/data/patientId");  //院内用户Id
                Function.ValidateParameter(patientId, "院内用户Id");
                string cardType = Function.GetNoteValue(xmlDoc, "Request/data/cardType");  //卡类型
                Function.ValidateParameter(cardType, "卡类型");
                string cardNo = Function.GetNoteValue(xmlDoc, "Request/data/cardNo");  //卡号
                Function.ValidateParameter(cardNo, "卡号");
                int res = OutPatientQueryManager.CardEabled(patientId, cardType, cardNo);
                string resXml = Function.GetResponseXML(true, "操作成功", string.Format(dataXml, res));  //默认是诊疗卡
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Format(dataXml, string.Empty));
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
        }

        [WebMethod(Description = "患者门诊就诊记录")]
        public string MZRecord(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                string patientId = Function.GetNoteValue(xmlDoc, "Request/data/patientId"); //院内用户id
                Function.ValidateParameter(patientId, "院内用户id");
                string medicalNo = Function.GetNoteValue(xmlDoc, "Request/data/medicalNo"); //病历号
                string certifcateType = Function.GetNoteValue(xmlDoc, "Request/data/certifcateType"); //用户证件类型
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo"); //用户证件号码
                string cardType = Function.GetNoteValue(xmlDoc, "Request/data/cardType"); //用户卡类型
                string cardNo = Function.GetNoteValue(xmlDoc, "Request/data/cardNo"); //用户卡号
                string startDate = Function.GetNoteValue(xmlDoc, "Request/data/startDate"); //就诊开始日期
                Function.ValidateParameter(startDate, "就诊开始日期");
                string endDate = Function.GetNoteValue(xmlDoc, "Request/data/endDate"); //就诊结束日期
                Function.ValidateParameter(endDate, "就诊结束日期");
                DateTime reStartDate = Function.ToDateTime(startDate);
                DateTime reEndDate = Function.ToDateTime(endDate);

                DataTable reRes = this.OutPatientQueryManager.QueryResgisterInfo(patientId, reStartDate, reEndDate);

                if (reRes == null)
                {
                    throw new Exception("用户就诊记录查询失败");
                }
                if (reRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查询到用户就诊记录");
                }

                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < reRes.Rows.Count; i++)
                {
                    dataXml.Append("<item>");
                    for (int j = 0; j < reRes.Columns.Count; j++)
                    {

                        dataXml.AppendFormat("<{0}>{1}</{0}>", reRes.Columns[j], reRes.Rows[i][j]);
                    }
                    dataXml.Append("</item>");
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;



            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, e.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;

            }
        }

        [WebMethod(Description = "患者住院就诊记录")]
        public string ZYRecord(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                string patientId = Function.GetNoteValue(xmlDoc, "Request/data/patientId"); //院内用户id
                Function.ValidateParameter(patientId, "院内用户id");
                string medicalNo = Function.GetNoteValue(xmlDoc, "Request/data/medicalNo"); //病历号
                string certifcateType = Function.GetNoteValue(xmlDoc, "Request/data/certifcateType"); //用户证件类型
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo"); //用户证件号码
                string cardType = Function.GetNoteValue(xmlDoc, "Request/data/cardType"); //用户卡类型
                string cardNo = Function.GetNoteValue(xmlDoc, "Request/data/cardNo"); //用户卡号
                string startDate = Function.GetNoteValue(xmlDoc, "Request/data/startDate"); //就诊开始日期
                Function.ValidateParameter(startDate, "就诊开始日期");
                string endDate = Function.GetNoteValue(xmlDoc, "Request/data/endDate");//就诊结束日期
                Function.ValidateParameter(endDate, "就诊结束日期");
                DateTime imStartDate = Function.ToDateTime(startDate);
                DateTime imEndDate = Function.ToDateTime(endDate);

                DataTable imRes = QueryManager.QueryInMainInfo(patientId, imStartDate, imEndDate);

                if (imRes == null)
                {
                    throw new Exception("用户住院就诊记录查询失败");
                }
                if (imRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查询到用户住院就诊记录");
                }

                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < imRes.Rows.Count; i++)
                {
                    for (int j = 0; j < imRes.Columns.Count; j++)
                    {

                        dataXml.AppendFormat("<{0}>{1}</{0}>", imRes.Columns[j], imRes.Rows[i][j]);
                    }
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;

            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, e.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;

            }
        }

        [WebMethod(Description = "患者检查报告列表")]
        public string CheckList(string req) { return "正在建设中……"; }

        [WebMethod(Description = "患者检查结果详情")]
        public string CheckDetail(string req) { return "正在建设中……"; }

        [WebMethod(Description = "患者检验结果列表")]
        public string InspectList(string req) { return "正在建设中……"; }

        [WebMethod(Description = "患者检验结果详情")]
        public string InspectDetail(string req) { return "正在建设中……"; }

        [WebMethod(Description = "患者排队查询")]
        public string QueueQuery(string req)
        {
            #region 入参

            //< Request >< data >
            //< patientId ></ patientId >
            //< medicalNo ></ medicalNo >
            //< certifcateType ></ certifcateType >
            //< certifcateNo ></ certifcateNo >
            //< cardType ></ cardType >
            //< cardNo ></ cardNo >
            //< visitNo ></ visitNo >
            //< type ></ type >
            //</ data ></ Request >

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

                string patientId = Function.GetNoteValue(xmlDoc, "Request/data/patientId");//院内用户id

                Function.ValidateParameter(patientId, "院内用户id");

                string medicalNo = Function.GetNoteValue(xmlDoc, "Request/data/medicalNo");//病历号

                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");//用户证件号码

                string type = Function.GetNoteValue(xmlDoc, "Request/data/type");//排队类别

                System.Data.DataTable dtRes = null;

                switch (type)
                {
                    case "1":

                        #region 挂号候诊排队

                        dtRes = this.PatientManager.QueryRegWaiting(patientId, medicalNo, certifcateNo);

                        #endregion
                        break;
                    case "2":
                        #region 取药排队
                        dtRes = this.PatientManager.QueryPhaWaiting(patientId, medicalNo, certifcateNo);
                        #endregion
                        break;
                }

                if (dtRes == null)
                {
                    throw new Exception("未找到有效的排队记录");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("未找到有效的排队记录");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    dataXml.AppendLine("<item>");
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

                        if (dtRes.Columns[j].ToString() == "doctorTitle")
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], Function.ConvertPlatformDoctLevelCode(dtRes.Rows[i][j].ToString()));
                        }
                    }
                    dataXml.AppendLine("</item>");
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

        [WebMethod(Description = "查询患者住院号")]
        public string QueryInpatId(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);

                string name = Function.GetNoteValue(xmlDoc, "Request/data/name");//姓名

                //string patientId = Function.GetNoteValue(xmlDoc, "Request/data/patientId");//院内用户id

                Function.ValidateParameter(name, "姓名");

                string inpatId = Function.GetNoteValue(xmlDoc, "Request/data/inpatId");

                Function.ValidateParameter(inpatId, "患者住院号");

                //身份证号
                string idno = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");

                System.Data.DataTable dtRes = null;

                dtRes = this.PatientManager.QueryInpatientNo(name, inpatId, idno);

                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();

                if (dtRes == null || dtRes.Rows.Count == 0)
                {
                    dataXml.Append("<cardType></cardType><cardNo></cardNo><patientId></patientId><inpatId></inpatId>");
                }
                else
                {
                    for (int i = 0; i < dtRes.Rows.Count; i++)
                    {
                        for (int j = 0; j < dtRes.Columns.Count; j++)
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j].ToString().Replace('/', '-'));
                        }
                    }
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

        [WebMethod(Description = "建档信息查询")]
        public string QueryByCondition(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);


                string certifcateType = Function.GetNoteValue(xmlDoc, "Request/data/certifcateType");//证件类型
 
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo");//证件号码

                string name = Function.GetNoteValue(xmlDoc, "Request/data/name");//用户卡类型
                string visitNo = Function.GetNoteValue(xmlDoc, "Request/data/visitNo");//用户卡号
                certifcateType = Function.ConvertHisCardTypeCode(certifcateType);

                DataTable dpRes = OutPatientQueryManager.QueryByCondition(certifcateType, certifcateNo, name, visitNo);

                if (dpRes == null)
                {
                    throw new Exception("患者信息查询失败");
                }
                if (dpRes.Rows.Count <= 0)
                {
                    throw new Exception("没有找到患者信息");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dpRes.Rows.Count; i++)
                {
                    //dataXml.Append("<item>");
                    for (int j = 0; j < dpRes.Columns.Count; j++)
                    {
                        if (dpRes.Rows[i][j] != null)
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dpRes.Columns[j], dpRes.Rows[i][j].ToString().Trim());
                        }
                        else
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dpRes.Columns[j], dpRes.Rows[i][j]);
                        }
                    }
                    //dataXml.Append("</item>");
                }
                string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, e.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;

            }
        }


    }
}
