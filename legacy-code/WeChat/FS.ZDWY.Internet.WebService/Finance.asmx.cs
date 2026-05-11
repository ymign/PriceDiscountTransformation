using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Finance : System.Web.Services.WebService
    {

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

        [WebMethod(Description = "查询账单信息")]
        public string BillQuery(string req)
        {
            #region 入参

            // < Request >
            //< data >
            //< beginDate ></ beginDate >
            //< endDate ></ endDate >
            //< source ></ source >
            //< isPay ></ isPay >
            //< data >
            //</ Request >

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

                DateTime beginDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/beginDate"));//开始日期

                Function.ValidateParameter(beginDate.ToString(), "开始日期");

                DateTime endDate = Function.ToDateTime(Function.GetNoteValue(xmlDoc, "Request/data/endDate"));//结束日期

                Function.ValidateParameter(endDate.ToString(), "结束日期");

                string source = Function.GetNoteValue(xmlDoc, "Request/data/source");//来源

                Function.ValidateParameter(source, "来源标识");

                string isPay = Function.GetNoteValue(xmlDoc, "Request/data/isPay");

                Function.ValidateParameter(isPay, "是否支付");

                System.Data.DataTable dtRes = null;

                switch (source)
                {
                    case "1":

                        #region 门诊对账

                        dtRes = this.PatientManager.QueryFinanceBill(beginDate, endDate, isPay);

                        #endregion
                        break;
                    default:

                        #region 其他对账

                        dtRes = this.PatientManager.QueryFinanceBill(beginDate, endDate, isPay);

                        #endregion

                        break;
                }

                if (dtRes == null)
                {
                    string nullxml = Function.GetResponseXML(true, "操作成功", "");
                    ServiceLogManager.Write("传出报文：" + nullxml);
                    return nullxml;
                }
                if (dtRes.Rows.Count <= 0)
                {
                    string nullxml = Function.GetResponseXML(true, "操作成功", "");
                    ServiceLogManager.Write("传出报文：" + nullxml);
                    return nullxml;
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
                        else if (dtRes.Columns[j].ToString() == "departmentName")
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], Function.XmlString(dtRes.Rows[i][j].ToString().Replace("&",string.Empty)));
                        }
                        else
                        {
                            dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], Function.XmlString(dtRes.Rows[i][j].ToString()));
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
    }
}
