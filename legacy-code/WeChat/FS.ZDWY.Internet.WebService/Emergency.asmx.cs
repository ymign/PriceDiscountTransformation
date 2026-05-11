using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// Emergency 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Emergency : System.Web.Services.WebService
    {
        #region 属性

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
        #endregion

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
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
            string gudi = Guid.NewGuid().ToString();
            ServiceLogManager.Write(gudi + "急诊系统患者建档入参：" + req);
            #region 出参数据报文模板
            string dataXml = @"<cardType>{0}</cardType>
        <cardNo>{1}</cardNo>
        <patientId>{2}</patientId>
        <medicalNo>{3}</medicalNo>
        <createTime>{4}</createTime>
       ";
            #endregion

            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                string cardNo = Function.GetNoteValue(xmlDoc, "Request/data/cardNo");//为空代表建档，反之代表修改建档信息
                string name = Function.GetNoteValue(xmlDoc, "Request/data/name"); //姓名
                Function.ValidateParameter(name, "姓名");
                string sex = Function.GetNoteValue(xmlDoc, "Request/data/sex");
                Function.ValidateParameter(sex, "性别");
                string age = Function.GetNoteValue(xmlDoc, "Request/data/age"); //年龄                
                string birth = Function.GetNoteValue(xmlDoc, "Request/data/birth"); //出生日期 YYYY-MM-DD
                Function.ValidateParameter(birth, "出生日期");
                string address = Function.GetNoteValue(xmlDoc, "Request/data/address"); //地址
                string mobile = Function.GetNoteValue(xmlDoc, "Request/data/mobile"); //电话
                Function.ValidateParameter(mobile, "电话");
                string certifcateType = Function.GetNoteValue(xmlDoc, "Request/data/certifcateType"); //证件类型
                string certifcateNo = Function.GetNoteValue(xmlDoc, "Request/data/certifcateNo"); //证件号码  患者类型为2是可不填
                string country = Function.GetNoteValue(xmlDoc, "Request/data/country"); //国籍编号
                string gatxzNum = Function.GetNoteValue(xmlDoc, "Request/data/gatxzNum"); //港澳台身份证

                FS.ZDWY.Internet.Models.COM_PATIENTINFO patient = new Models.COM_PATIENTINFO();
                patient.NAME = name;
                patient.SEX_CODE = sex; //Function.ConvertHISSexCode(sex);
                patient.BIRTHDAY = Function.ToDateTime(birth);
                patient.HOME_NOW = address;
                patient.HOME_TEL = mobile;
                patient.IDCARDTYPE = certifcateType.PadLeft(2, '0');
                patient.IDCARDTYPE = Function.ConvertHisCardTypeCode(certifcateType);
                patient.IDENNO = certifcateNo;
                patient.OPER_CODE = Function.EmergencyOper.Code;
                patient.OPER_DATE = Function.ToDateTime(PatientManager.GetSysTime());
                patient.INSURANCE_NAME = gatxzNum;
                patient.COUN_CODE = country;
                //新增字段
                if (string.IsNullOrEmpty(cardNo))
                {
                    patient.CARD_NO = PatientManager.GetCardNo();
                    patient.PACT_CODE = "1";
                    patient.PACT_NAME = "现金";
                    patient.PAYKIND_CODE = "01";
                    patient.IS_VALID = "1";
                    patient.PATIENT_TYPE = "1";
                    patient.VIP_FLAG = "0";
                }
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
                    resXml = Function.GetResponseXML(true, "操作成功", string.Format(dataXml, 1, patientReturn.CARD_NO, patientReturn.CARD_NO, "", patientReturn.OPER_DATE));  //默认是诊疗卡
                }
                else
                {
                    resXml = Function.GetResponseXML(true, "操作成功", string.Format(dataXml, 1, patient.CARD_NO, patient.CARD_NO, "", patient.OPER_DATE));  //默认是诊疗卡
                }

                ServiceLogManager.Write(gudi + "急诊系统患者建档出参：" + resXml);
                return resXml;
            }
            catch (Exception ex)
            {
                string resXml = Function.GetResponseXML(false, ex.Message, string.Empty);
                ServiceLogManager.Write(gudi + "急诊系统患者建档出参：" + resXml);
                return resXml;
            }
        }

    }
}
