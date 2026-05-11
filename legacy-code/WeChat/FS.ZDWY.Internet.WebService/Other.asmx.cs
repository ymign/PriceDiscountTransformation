using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// Other 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Other : System.Web.Services.WebService
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

        #endregion

        [WebMethod(Description = "查询医生排班")]
        public string doctor(string req)
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

                string startTime = Function.GetNoteValue(xmlDoc, "Request/data/startTime");  //号源日期
                Function.ValidateParameter(startTime, "开始时间");
                string endTime = Function.GetNoteValue(xmlDoc, "Request/data/endTime"); //排班ID
                Function.ValidateParameter(endTime, "结束时间");
                string doctorCode = Function.GetNoteValue(xmlDoc, "Request/data/doctorCode");  //医生代码 
                Function.ValidateParameter(doctorCode, "医生代码");
                DateTime dtstartTime = Function.ToDateTime(startTime);
                DateTime dtendTime = Function.ToDateTime(endTime);

                System.Data.DataTable dtRes = OutPatientQueryManager.QueryScheduleByDoctor(dtstartTime, dtendTime, doctorCode);
                if (dtRes == null)
                {
                    throw new Exception("查询医生排班失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查找到查询医生排班信息");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    if (dtRes.Rows[i]["deptCode"].ToString().IndexOf(',') == -1)
                    {
                        if (dataXml.ToString().IndexOf(dtRes.Rows[i]["deptCode"].ToString()) != -1)
                        {
                            continue;
                        }
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
                    else
                    {
                        string[] deptCodeList = dtRes.Rows[i]["deptCode"].ToString().Split(',');
                        string[] deptNameList = dtRes.Rows[i]["deptName"].ToString().Split(',');

                        for (int k = 0; k < deptCodeList.Length; k++)
                        {
                            if (dataXml.ToString().IndexOf(deptCodeList[k]) != -1)
                            {
                                continue;
                            }
                            dataXml.Append("<item>");
                            dataXml.AppendFormat("<{0}>{1}</{0}>", "deptCode", deptCodeList[k]);
                            dataXml.AppendFormat("<{0}>{1}</{0}>", "deptName", deptNameList[k]);
                            for (int j = 0; j < dtRes.Columns.Count; j++)
                            {
                                if (dtRes.Columns[j].ColumnName != "deptCode" && dtRes.Columns[j].ColumnName != "deptName")
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
                            dataXml.Append("</item>");
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
    }
}
