using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizLogic.HealthRecord
{
    /// <summary>
    /// 病种操作记录
    /// </summary>
    public class ICDScoreLog : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <returns></returns>
        public int CreateICDScoreLog(Neusoft.HISFC.Models.HealthRecord.ICDScoreLog obj)
        {
            string Sql = "";
            try
            {
                //获取查询SQL语句
                if (this.Sql.GetSql("HealthRecord.ICDScoreLog.Log", ref Sql) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDScoreLog.Log";
                    return -1;
                }
                Sql = string.Format(Sql, obj.Inptient_no, obj.Icd10_1, obj.Icd10_2, obj.Icd9, obj.Si_type, obj.HappenNo, obj.Oper_code, obj.Oper_dept, obj.Oper_date, Neusoft.HISFC.Models.HealthRecord.ICDScoreLog.OperType.CREATE.ToString(), obj.Mark);

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1; // 出现错误返回null
            }
            return this.ExecNoQuery(Sql);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <returns></returns>
        public int UpdateICDScoreLog(Neusoft.HISFC.Models.HealthRecord.ICDScoreLog obj)
        {
            string Sql = "";
            try
            {
                //获取查询SQL语句
                if (this.Sql.GetSql("HealthRecord.ICDScoreLog.Log", ref Sql) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDScoreLog.Log";
                    return -1;
                }
                Sql = string.Format(Sql, obj.Inptient_no, obj.Icd10_1, obj.Icd10_2, obj.Icd9, obj.Si_type, obj.HappenNo, obj.Oper_code, obj.Oper_dept, obj.Oper_date, Neusoft.HISFC.Models.HealthRecord.ICDScoreLog.OperType.UPDATE.ToString(), obj.Mark);

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1; // 出现错误返回null
            }
            return this.ExecNoQuery(Sql);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <returns></returns>
        public int UpLodaICDScoreLog(Neusoft.HISFC.Models.HealthRecord.ICDScoreLog obj)
        {
            string Sql = "";
            try
            {
                //获取查询SQL语句
                if (this.Sql.GetSql("HealthRecord.ICDScoreLog.Log", ref Sql) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDScoreLog.Log";
                    return -1;
                }
                Sql = string.Format(Sql, obj.Inptient_no, obj.Icd10_1, obj.Icd10_2, obj.Icd9, obj.Si_type, obj.HappenNo, obj.Oper_code, obj.Oper_dept, obj.Oper_date, Neusoft.HISFC.Models.HealthRecord.ICDScoreLog.OperType.UPLOAD.ToString(), obj.Mark);

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1; // 出现错误返回null
            }
            return this.ExecNoQuery(Sql);
        }

        /// <summary>
        /// 获取最新记录
        /// </summary>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.ICDScoreLog GetLastICDScoreLog(string inpatientno)
        {
            string Sql = "";
            try
            {
                //获取查询SQL语句
                if (this.Sql.GetSql("HealthRecord.ICDScoreLog.GetLastLog", ref Sql) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDScoreLog.GetLastLog";
                    return null;
                }
                Sql = string.Format(Sql, inpatientno);

                this.ExecQuery(Sql);
                Neusoft.HISFC.Models.HealthRecord.ICDScoreLog obj = null;
                while (this.Reader.Read())
                {
                    obj = new Neusoft.HISFC.Models.HealthRecord.ICDScoreLog();
                    obj.Inptient_no = this.Reader[0].ToString();
                    obj.Icd10_1 = this.Reader[1].ToString();
                    obj.Icd10_2 = this.Reader[2].ToString();
                    obj.Icd9 = this.Reader[3].ToString();
                    obj.Si_type = this.Reader[4].ToString();
                    obj.HappenNo = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[5].ToString());
                    obj.Oper_code = this.Reader[6].ToString();
                    obj.Oper_dept = this.Reader[7].ToString();
                    obj.Oper_date = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[8]);
                }
                this.Reader.Close();
                return obj;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null; // 出现错误返回null
            }
            return null;
        }

        /// <summary>
        /// 获取发生序号
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        public int GetHappenNo(string inpatientNo)
        {
            string strSQL = "";
            try
            {
                if (this.Sql.GetSql("HealthRecord.ICDScoreLog.GetHappenNo", ref strSQL) == -1)
                {
                    this.Err = "获取SQL语句失败";
                    return -1;
                }
                strSQL = string.Format(strSQL, inpatientNo);
                string num = this.ExecSqlReturnOne(strSQL);
                int hn = Neusoft.FrameWork.Function.NConvert.ToInt32(num);
                if (hn == 0)
                {
                    hn = 1;
                }
                return hn;
            }
            catch
            {
                return -1;
            }
        }
    }
}
