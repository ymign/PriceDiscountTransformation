using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using FS.ZDWY.Internet.Models;
using System.Data;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// BaseData 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class BaseData : System.Web.Services.WebService
    {

        BP.BP_DepartmentEntity bp_DepartmentEntity;
        /// <summary>
        /// 返回的科室查询信息
        /// </summary>
        BP.BP_DepartmentEntity Bp_DepartmentEntity
        {
            get
            {
                if (bp_DepartmentEntity == null)
                {
                    bp_DepartmentEntity = new BP.BP_DepartmentEntity();
                }
                return bp_DepartmentEntity;
            }

        }
        BP.Doctor.DoctorManager doctorManager;
        /// <summary>
        /// 返回的医生查询信息
        /// </summary>
        BP.Doctor.DoctorManager DoctorManager
        {
            get
            {
                if (doctorManager == null)
                {
                    doctorManager = new BP.Doctor.DoctorManager();
                }
                return doctorManager;
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
        /// 科室信息查询
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [WebMethod(Description = "科室信息查询")]
        public string Departments(string req)
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

                String deptCode = Function.GetNoteValue(xmlDoc, "Request/data/deptCode");//科室代码
                String rank = Function.GetNoteValue(xmlDoc, "Request/data/rank");//科室等级公众号的开发
                //if (rank != null)
                //{
                //    rank = "1";
                //}
                if (string.IsNullOrEmpty(deptCode))
                {
                    List<DepartmentEntity> deptData = Bp_DepartmentEntity.QueryDepartmentAll(rank);

                    if (deptData == null)
                    {
                        throw new Exception("查询排班科室失败");
                    }
                    System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                    for (int i = 0; i < deptData.Count; i++)
                    {
                        dataXml.Append("<item>");
                        dataXml.AppendFormat("<deptCode>{0}</deptCode>", deptData[i].DEPT_CODE);
                        dataXml.AppendFormat("<deptName>{0}</deptName>", deptData[i].DEPT_NAME);
                        dataXml.AppendFormat("<hasChild>{0}</hasChild>", deptData[i].HSACHILD);
                        dataXml.AppendFormat("<parentDeptCode>{0}</parentDeptCode>", deptData[i].PARENTDEPTCODE);
                        dataXml.AppendFormat("<deptDescription>{0}</deptDescription>", deptData[i].DEPTDESCRIPTION);
                        dataXml.AppendFormat("<deptLocation>{0}</deptLocation>", deptData[i].DEPTLOCATION);
                        dataXml.AppendFormat("<rule>{0}</rule>", deptData[i].RULE);
                        dataXml.AppendFormat("<status>{0}</status>", 1 - deptData[i].STATUS);//接口 0 是 1 否 数据库 0 否 1 是
                        dataXml.AppendFormat("<expertise>{0}</expertise>", deptData[i].EXPERTISE);
                        dataXml.Append("</item>");
                    }
                    string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                    ServiceLogManager.Write("传出报文：" + resXml);
                    return resXml;
                }
                else
                {
                    List<DepartmentEntity> deptData = Bp_DepartmentEntity.QueryDepartments(deptCode, rank);

                    if (deptData == null)
                    {
                        throw new Exception("查询排班科室失败");
                    }
                    System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                    for (int i = 0; i < deptData.Count; i++)
                    {
                        dataXml.Append("<item>");
                        dataXml.AppendFormat("<deptCode>{0}</deptCode>", deptData[i].DEPT_CODE);
                        dataXml.AppendFormat("<deptName>{0}</deptName>", deptData[i].DEPT_NAME);
                        dataXml.AppendFormat("<hasChild>{0}</hasChild>", deptData[i].HSACHILD);
                        dataXml.AppendFormat("<parentDeptCode>{0}</parentDeptCode>", deptData[i].PARENTDEPTCODE);
                        dataXml.AppendFormat("<deptDescription>{0}</deptDescription>", deptData[i].DEPTDESCRIPTION);
                        dataXml.AppendFormat("<deptLocation>{0}</deptLocation>", deptData[i].DEPTLOCATION);
                        dataXml.AppendFormat("<rule>{0}</rule>", deptData[i].RULE);
                        dataXml.AppendFormat("<status>{0}</status>", deptData[i].STATUS);
                        dataXml.AppendFormat("<expertise>{0}</expertise>", deptData[i].EXPERTISE);
                        dataXml.Append("</item>");
                    }
                    string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                    ServiceLogManager.Write("传出报文：" + resXml);
                    return resXml;
                }
            }
            catch (Exception e)
            {
                string resXml = Function.GetResponseXML(false, e.Message, string.Empty);
                ServiceLogManager.Write("传出报文：" + resXml);
                return resXml;
            }





        }

        /// <summary>
        /// 医生信息查询
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [WebMethod(Description = "医生信息查询")]
        public string Doctors(string req)
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
                string deptCode = Function.GetNoteValue(xmlDoc, "Request/data/deptCode");//科室代码
                //Function.ValidateParameter(deptCode, "科室代码");
                string doctorCode = Function.GetNoteValue(xmlDoc, "Request/data/doctorCode");//医生代码
                                                                                             //Function.ValidateParameter(doctorCode, "医生代码");
                                                                                             //DataTable doRes = _DoctorManager.QueryDoctorList(deptCode, doctorCode);
                                                                                             //if(doRes == null)
                                                                                             //{
                                                                                             //    throw new Exception("查找医生信息失败！");
                                                                                             //}
                                                                                             //if(doRes.Rows.Count <= 0)
                                                                                             //{
                                                                                             //    throw new Exception("没有排班的医生信息！");
                                                                                             //}
                                                                                             //System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                                                                                             //for (int i = 0; i < doRes.Rows.Count; i++)
                                                                                             //{
                                                                                             //    dataXml.Append("<item>");
                                                                                             //    for (int j = 0; j < doRes.Columns.Count; j++)
                                                                                             //    {

                //        dataXml.AppendFormat("<{0}>{1}</{0}>", doRes.Columns[j], doRes.Rows[i][j]);
                //    }
                //    dataXml.Append("</item>");
                //}
                //string resXml = Function.GetResponseXML(true, "操作成功", dataXml.ToString());
                //ServiceLogManager.Write("传出报文：" + resXml);
                //return resXml;

                DataTable doRes = DoctorManager.QueryDoctorList(deptCode, doctorCode);
                if (doRes == null)
                {
                    throw new Exception("查找医生信息失败！");
                }
                if (doRes.Rows.Count <= 0)
                {
                    throw new Exception("没有排班的医生信息！");
                }
                System.Text.StringBuilder dataXml = new System.Text.StringBuilder();
                for (int i = 0; i < doRes.Rows.Count; i++)
                {
                    dataXml.Append("<item>");
                    for (int j = 0; j < doRes.Columns.Count; j++)
                    {

                        dataXml.AppendFormat("<{0}>{1}</{0}>", doRes.Columns[j], doRes.Rows[i][j]);
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
    }
}
