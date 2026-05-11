using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace Neusoft.HISFC.BizLogic.HealthRecord
{
    public class CaseRecover : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 插入病案回收记录
        /// </summary>
        /// <param name="patientinfo"></param>
        /// <returns></returns>
        public int InsertCaseRecoverinfo(Neusoft.HISFC.Models.RADT.PatientInfo patientinfo,string statues)
        {
            string strSQL = "";
            if (Sql.GetSql("CASE.CaseRecover.InsertCaseRecoverinfo", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, GetParams(patientinfo, statues));
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return -1;
                }
            }
            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 更新病案回收记录
        /// </summary>
        /// <param name="patientinfo"></param>
        /// <returns></returns>
        public int UpdateCaseRecoverinfo(Neusoft.HISFC.Models.RADT.PatientInfo patientinfo, string statues)
        {
            string strSQL = "";
            if (Sql.GetSql("CASE.CaseRecover.UpdateCaseRecoverinfo", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, GetParams(patientinfo, statues));
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return -1;
                }
            }

            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="patientinfo"></param>
        /// <returns></returns>
        public string[] GetParams(Neusoft.HISFC.Models.RADT.PatientInfo patientinfo, string statues)
        {
            string[] Pa = null;
            try
            {
                Pa = new string[] 
                {
                    patientinfo.ID,
                    patientinfo.PID.PatientNO,
                    patientinfo.Name,
                    patientinfo.PVisit.PatientLocation.Dept.ID,
                    patientinfo.PVisit.PatientLocation.Dept.Name,
                    patientinfo.InTimes.ToString(),
                    ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID,
                    Neusoft.FrameWork.Management.Connection.Operator.ID,
                    this.GetDateTimeFromSysDateTime().ToString(),
                    statues
                };
            }
            catch (Exception e)
            { }
            return Pa;
        }

        /// <summary>
        /// 查询患者
        /// </summary>
        /// <param name="inpatient_no">住院流水号</param>
        /// <param name="type">0 回收;1 打印</param>
        /// <returns></returns>
        public int QueryExist(string inpatient_no,string type)
        {
            string strSQL = "";
            if (Sql.GetSql("CASE.CaseRecover.QueryCaseRecoverinfo", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, inpatient_no, type);
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return -1;
                }
            }

            string result = this.ExecSqlReturnOne(strSQL, "0");
            return Neusoft.FrameWork.Function.NConvert.ToInt32(result);
        }

        /// <summary>
        /// 更新病案回收打印记录
        /// </summary>
        /// <param name="patientinfo"></param>
        /// <returns></returns>
        public int UpdateCaseRecoverPrintInfo(string inpatient_no)
        {
            string strSQL = "";
            if (Sql.GetSql("CASE.CaseRecover.UpdateCaseRecoverPrintinfo", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, inpatient_no, this.GetDateTimeFromSysDateTime().ToString(), this.Operator.ID);
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return -1;
                }
            }

            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 获取回收数据
        /// </summary>
        /// <returns></returns>
        public DataSet GetRecoverData(DateTime begin, DateTime end, string opercode)
        {
            DataSet ds = null;
            string strSQL = "";
            if (Sql.GetSql("CASE.CaseRecover.GetCaseRecoverInfo", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, opercode, begin.ToString(), end.ToString());
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return null;
                }
            }
            this.ExecQuery(strSQL, ref ds);
            return ds;
        }

        /// <summary>
        /// 获取回收数据
        /// </summary>
        /// <returns></returns>
        public DataSet GetRecoverDataAll(DateTime begin, DateTime end, string opercode,string deptcode)
        {
            DataSet ds = null;
            string strSQL = "";
            if (Sql.GetSql("CASE.CaseRecover.GetCaseRecoverInfoAll", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, opercode, begin.ToString(), end.ToString(), deptcode);
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return null;
                }
            }
            this.ExecQuery(strSQL, ref ds);
            return ds;
        }
        

        /// <summary>
        /// 获取打印回收数据
        /// </summary>
        /// <returns></returns>
        public DataSet GetPrintRecoverData(DateTime begin, DateTime end)
        {
            DataSet ds = null;
            string strSQL = "";
            if (Sql.GetSql("CASE.CaseRecover.GetCasePrintRecoverInfo", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, begin.ToString(), end.ToString());
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return null;
                }
            }
            this.ExecQuery(strSQL, ref ds);
            return ds;
        }


        /// <summary>
        /// 通过病人号下旬打印回收数据
        /// </summary>
        /// <returns></returns>
        public DataSet QueryPrintDataByPatientno(string patientNo)
        {
            DataSet ds = null;
            string strSQL = "";
            if (Sql.GetSql("CASE.CaseRecover.QueryPrintDataByPatientno", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, patientNo);
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return null;
                }
            }
            this.ExecQuery(strSQL, ref ds);
            return ds;
        }
        

        /// <summary>
        /// 获取打印回收数据
        /// </summary>
        /// <returns></returns>
        public DataSet GetRecoverQueryData(DateTime begin, DateTime end)
        {
            DataSet ds = null;
            string strSQL = "";
            if (Sql.GetSql("CASE.CaseRecover.GetRecoverQueryDataInfo", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, begin.ToString(), end.ToString());
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return null;
                }
            }
            this.ExecQuery(strSQL, ref ds);
            return ds;
        }
    }
}
