using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// 排班号源相关
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Description = "医生排班号源信息")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Scheduling : System.Web.Services.WebService
    {


        #region 属性

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

        [WebMethod(Description = "预约排班医生列表")]
        public string DoctorList(string req)
        {
            #region 入参模板
            /*
            <Request>
                <data>
                    <beginDate></beginDate>
                    <endDate></endDate
                    <deptCode></deptCode>
                </data>
            </Request> 
            */
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

                string beginDate = Function.GetNoteValue(xmlDoc, "Request/data/beginDate");  //班次开始日期
                Function.ValidateParameter(beginDate, "班次开始日期");
                string endDate = Function.GetNoteValue(xmlDoc, "Request/data/endDate"); //班次结束日期
                Function.ValidateParameter(endDate, "班次结束日期");
                string deptCode = Function.GetNoteValue(xmlDoc, "Request/data/deptCode"); //科室代码
                Function.ValidateParameter(deptCode, "科室代码");
                string elderlyVoucherDoctorFlag = Function.GetNoteValue(xmlDoc, "Request/data/elderlyVoucherDoctorFlag"); //科室代码
                Function.ValidateParameter(elderlyVoucherDoctorFlag, "长者券医生标识");
                DateTime dtBeginDate = Function.ToDateTime(beginDate);
                DateTime dtEndDate = Function.ToDateTime(endDate);
                System.Data.DataTable dtRes;
                if (elderlyVoucherDoctorFlag == "1")
                {
                    dtRes = OutPatientQueryManager.QueryZZQDoctorList(dtBeginDate, dtEndDate, deptCode);
                }
                else
                {
                    dtRes = OutPatientQueryManager.QueryDoctorList(dtBeginDate, dtEndDate, deptCode);
                }
                if (dtRes == null)
                {
                    throw new Exception("查找预约排班医生列表失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查找到预约排班医生列表");
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

        [WebMethod(Description = "预约排班查询")]
        public string Schedule(string req)
        {
            #region 入参模板
            /*
             <Request>
                <data>
                    <beginDate></beginDate>
                    <endDate></endDate
                    <deptCode></deptCode>
                    <doctorCode></doctorCode>
                </data>
            </Request> 
            */
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

                string beginDate = Function.GetNoteValue(xmlDoc, "Request/data/beginDate");  //班次开始日期
                Function.ValidateParameter(beginDate, "班次开始日期");
                string endDate = Function.GetNoteValue(xmlDoc, "Request/data/endDate"); //班次结束日期
                Function.ValidateParameter(endDate, "班次结束日期");
                string deptCode = Function.GetNoteValue(xmlDoc, "Request/data/deptCode"); //科室代码
                Function.ValidateParameter(deptCode, "科室代码");
                DateTime dtBeginDate = Function.ToDateTime(beginDate);
                DateTime dtEndDate = Function.ToDateTime(endDate);
                string doctorCode = Function.GetNoteValue(xmlDoc, "Request/data/doctorCode");  //医生代码 如若不传医生代码，就查询科室下全部医生班次。
                System.Data.DataTable dtRes = OutPatientQueryManager.QuerySchedule(dtBeginDate, dtEndDate, deptCode, doctorCode);
                if (dtRes == null)
                {
                    throw new Exception("查找预约排班信息失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查找到预约排班信息");
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

        [WebMethod(Description = "预约排班分时号源查询")]
        public string ScheduleTime(string req)
        {
            #region 入参模板
            /*
             <Request>
                <data>
                    <deptCode></deptCode>
                    <doctorCode></doctorCode>
                    <scheduleDate></scheduleDate>
                    <scheduleId></scheduleId>
                </data>
            </Request>
            */
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

                string scheduleDate = Function.GetNoteValue(xmlDoc, "Request/data/scheduleDate");  //号源日期
                Function.ValidateParameter(scheduleDate, "班次开始日期");
                string scheduleId = Function.GetNoteValue(xmlDoc, "Request/data/scheduleId"); //排班ID
                Function.ValidateParameter(scheduleId, "排班ID");
                string deptCode = Function.GetNoteValue(xmlDoc, "Request/data/deptCode"); //科室代码
                Function.ValidateParameter(deptCode, "科室代码");
                string doctorCode = Function.GetNoteValue(xmlDoc, "Request/data/doctorCode");  //医生代码 
                Function.ValidateParameter(doctorCode, "医生代码");
                DateTime dtscheduleDate = Function.ToDateTime(scheduleDate);
                
                System.Data.DataTable dtRes = OutPatientQueryManager.QueryScheduleTime(dtscheduleDate, scheduleId, deptCode, doctorCode);
                if (dtRes == null)
                {
                    throw new Exception("预约排班分时号源查询失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查找到预约排班分时号源信息");
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

        [WebMethod(Description = "查询预约科室列表")]
        public string Departments(string req)
        {
            ServiceLogManager.Write("传入报文：" + req);
            try
            {
                if (string.IsNullOrEmpty(req))
                {
                    throw new Exception("入参不正确");
                }
                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.LoadXml(req);

                //string beginDate = Function.GetNoteValue(xmlDoc, "Request/data/beginTime");  //调用时间
                //unction.ValidateParameter(beginDate, "调用时间");
                //DateTime dtBeginDate = Function.ToDateTime(beginDate);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(req);
                System.Data.DataTable dtRes;
                string elderlyVoucherRegDeptFlag = Function.GetNoteValue(xmlDoc, "Request/data/elderlyVoucherRegDeptFlag");
                if (elderlyVoucherRegDeptFlag == "1")
                {
                    dtRes = OutPatientQueryManager.QueryZZQBookDept();
                }
                else
                {
                    dtRes = OutPatientQueryManager.QueryBookDept();
                }
                if (dtRes == null)
                {
                    throw new Exception("查询预约科室列表失败");
                }
                if (dtRes.Rows.Count <= 0)
                {
                    throw new Exception("没有查询到预约科室列表信息");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                Dictionary<string, List<Models.DepartmentEntity>> listdepts = new Dictionary<string, List<Models.DepartmentEntity>>();
                for (int i = 0; i < dtRes.Rows.Count; i++)
                {
                    //dataXml.Append("<item>");
                    //for (int j = 0; j < dtRes.Columns.Count; j++)
                    //{
                        //if (dtRes.Columns[j].DataType.Name == "DateTime")
                        //{
                        //    dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j].ToString().Replace('/', '-'));
                        //}
                        //else
                        //{
                        //    dataXml.AppendFormat("<{0}>{1}</{0}>", dtRes.Columns[j], dtRes.Rows[i][j]);
                        //}
                    //}
                    //dataXml.Append("</item>");
                        Models.DepartmentEntity obj = new Models.DepartmentEntity();
                        obj.PARENTDEPTCODE = dtRes.Rows[i][0].ToString();
                        obj.DEPTDESCRIPTION = dtRes.Rows[i][1].ToString();
                        obj.DEPTLOCATION = dtRes.Rows[i][2].ToString();
                        obj.DEPT_CODE = dtRes.Rows[i][3].ToString();
                        obj.DEPT_NAME = dtRes.Rows[i][4].ToString();
                        if (listdepts.ContainsKey(obj.PARENTDEPTCODE))
                        {
                            listdepts[obj.PARENTDEPTCODE].Add(obj);
                        }
                        else
                        {
                            listdepts.Add(obj.PARENTDEPTCODE, new List<Models.DepartmentEntity> { obj });
                        }
                }
                string minxml = @"<itemData><sortId>{0}</sortId><deptCode>{1}</deptCode><deptName>{2}</deptName></itemData>";

                string mainxml = "";

                foreach(KeyValuePair<string,List<Models.DepartmentEntity>> obj in listdepts)
                {
                    string id = obj.Key;
                    string name = obj.Value[0].DEPTDESCRIPTION;
                    string xml = @"<item><broId>" + obj.Key + @"</broId><broName>" + name + @"</broName><items>";
                    string dxml = "";
                    foreach (Models.DepartmentEntity de in obj.Value)
                    {            
                        dxml += string.Format(minxml, de.DEPTLOCATION,de.DEPT_CODE,de.DEPT_NAME);
                        //string rep = de.DEPT_NAME.Replace("&", "&amp;");
                        //dxml += string.Format(minxml, de.DEPTLOCATION, de.DEPT_CODE, rep);
                    }
                    xml += dxml + @"</items></item>";

                    mainxml += xml;
                }

                string resXml = Function.GetResponseXML(true, "操作成功", mainxml.ToString());
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
    }
}
