using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Neusoft.HISFC.BizLogic.HealthRecord.Case
{
    public class CaseStroe : Neusoft.FrameWork.Management.Database
    {
        #region 数据库基本操作
        /// <summary>
        /// 插入病案库房信息
        /// </summary>
        /// <param name="caseStore"></param>
        /// <returns></returns>
        public int InsertCaseStore(Neusoft.HISFC.Models.HealthRecord.Case.CaseStore caseStore)
        {

            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CaseStore.Insert", ref strSql) == -1) return -1;
            try
            {
                //插入
                strSql = string.Format(strSql, GetInfo(caseStore));
                return this.ExecNoQuery(strSql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }
        private string[] GetInfo(Neusoft.HISFC.Models.HealthRecord.Case.CaseStore caseStore)
        {
            string[] str = new string[15];
            str[0] = caseStore.PatientInfo.PID.PatientNO; //住院号
            str[1] = caseStore.PatientInfo.Name;
            str[2] = caseStore.PatientInfo.InTimes.ToString(); //住院次数
            str[3] = caseStore.Store.ID.ToString();//病案库房编号
            str[4] = caseStore.Cabinet.ID;  //病案柜编号
            str[5] = caseStore.Grid.ID; //病案柜格数
            str[6] = caseStore.CaseState.ToString(); //操作类型
            if (caseStore.IsVaild)//有效性
            {
                str[7] = "1";
            }
            else
            {
                str[7] = "0";
            }
            str[8] = caseStore.CaseMemo.ToString();//备注
            str[9] = caseStore.OperEnv.ID.ToString();//操作员
            str[10] = caseStore.OperEnv.OperTime.ToString();//操作时间
            str[11] = caseStore.Extend1.ToString();//扩展1
            str[12] = caseStore.Extend2.ToString();//扩展2
            str[13] = caseStore.Extend3.ToString();//扩展3
            str[14] = caseStore.Extend4.ToString();//扩展4
            return str;
        }
        /// <summary>
        /// 更新库房信息
        /// </summary>
        /// <param name="caseStore"></param>
        /// <returns></returns>
        public int UpdateCaseStore(Neusoft.HISFC.Models.HealthRecord.Case.CaseStore caseStore)
        {
            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CaseStore.Update", ref strSql) == -1) return -1;
            try
            {
                string[] strParm = GetInfo(caseStore);
                strSql = string.Format(strSql, strParm);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return -1;
            }

            //　执行SQL语句返回
            return this.ExecNoQuery(strSql);

        }

        public int DeleteCaseStore(string CaseNO ,string InTimes)
        {

            string deleteSql = string.Empty;
            int intReturn = -1;
            if (this.Sql.GetSql("HealthReacord.Case.CaseStore.Delete", ref deleteSql) == -1) return -1;
            try
            {
                //插入
                deleteSql = string.Format(deleteSql,CaseNO,InTimes);
                intReturn = this.ExecNoQuery(deleteSql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            if ( intReturn < 0 )
            {
                this.Err = "删除病案库房信息失败" + this.Err;
                return -1;
            }

            return intReturn;
        }
        /// <summary>
        /// 插入库房明细信息
        /// </summary>
        /// <param name="caseStore"></param>
        /// <param name="CaseId"></param>
        /// <returns></returns>
        public int InsertCaseStoreDetail(Neusoft.HISFC.Models.HealthRecord.Case.CaseStore caseStore,string CaseId)
        {
            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CaseStoreDetail.Insert", ref strSql) == -1) return -1;
            try
            {
                //插入
                strSql = string.Format(strSql, CaseId,caseStore.PatientInfo.PID.PatientNO,caseStore.PatientInfo.Name,caseStore.PatientInfo.InTimes.ToString(),caseStore.CaseState,caseStore.OperEnv.ID,caseStore.OperEnv.OperTime.ToString());
                return this.ExecNoQuery(strSql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }
        #endregion 

        #region 查询

        public Neusoft.HISFC.Models.HealthRecord.Case.CaseStore QueryCaseStore(string patientNo, string inTimes)
        {
            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CaseStroe.Select", ref strSql) == -1) return null;
            try
            {
                //查询
                strSql = string.Format(strSql, patientNo, inTimes);
                this.ExecQuery(strSql);
                Neusoft.HISFC.Models.HealthRecord.Case.CaseStore caseStore = null;
               
                while (this.Reader.Read())
                {
                    caseStore = new Neusoft.HISFC.Models.HealthRecord.Case.CaseStore();
                    caseStore.PatientInfo.PID.PatientNO = this.Reader[0].ToString(); //住院号
                    caseStore.PatientInfo.Name = this.Reader[1].ToString();        //姓名
                    caseStore.PatientInfo.InTimes =Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString());//住院次数
                    caseStore.Store.ID = this.Reader[3].ToString();//病案库房编号
                    caseStore.Cabinet.ID = this.Reader[4].ToString();//病案柜编号
                    caseStore.Grid.ID = this.Reader[5].ToString();    //病案柜格数
                    caseStore.CaseState = this.Reader[6].ToString();//操作类型 
                    caseStore.IsVaild = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[7].ToString());//有效性
                    caseStore.CaseMemo = this.Reader[8].ToString();//备注
                    caseStore.OperEnv.ID = this.Reader[9].ToString();//操作员               
                    caseStore.OperEnv.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[10].ToString());  //操作时间
                    caseStore.Extend1 = this.Reader[11].ToString(); 
                    caseStore.Extend2 = this.Reader[12].ToString(); 
                    caseStore.Extend3 = this.Reader[13].ToString(); 
                    caseStore.Extend4 = this.Reader[14].ToString(); 
                }
                this.Reader.Close();
                return caseStore;
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                this.Err = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// 根据住院号查询库存
        /// </summary>
        /// <param name="patientNo"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int QueryCaseStoreByPatientNo(string patientNo,ref DataSet ds)
        {
            try
            {
                //if (this.Sql.GetSql("HealthReacord.Case.CaseStroe.Select1", ref strSql) == -1) return -1;
                string strSql = "";
                strSql = @"SELECT 
                            t.patient_no as 病案号,
                            t.NAME as 姓名,
                            t.in_times  as 次数,
                            CASE WHEN t.CASE_STATE='2'    THEN '出库-复印借出' ELSE      (SELECT MARK FROM COM_DICTIONARY WHERE TYPE='CaseState' AND CODE=t.CASE_STATE) END  as 库房状态,
                            (SELECT NAME FROM COM_DICTIONARY WHERE TYPE='CaseStore' AND CODE=t.STORE_CODE) as 库房名称,
                            (SELECT NAME FROM COM_DICTIONARY WHERE TYPE='CaseCabinet' AND CODE=t.CABINET_CODE) as 库房柜排名称
                             FROM MET_CAS_STORE  t
                            WHERE t.PATIENT_NO='{0}'
                            ";
                    try
                    {
                        //查询
                        strSql = string.Format(strSql, patientNo);
                    }
                    catch (Exception ex)
                    {
                        this.Err = "SQL语句赋值出错!" + ex.Message;
                        return -1;
                    }
                
            
               
                return this.ExecQuery(strSql, ref ds);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            

        }
        /// <summary>
        /// 根据姓名查询库存
        /// </summary>
        /// <param name="patientName"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int QueryCaseStoreByPatientName(string patientName,  ref DataSet ds)
        {
            try
            {
                //if (this.Sql.GetSql("HealthReacord.Case.CaseStroe.Select1", ref strSql) == -1) return -1;
                string strSql = "";
                strSql = @"SELECT 
                            t.patient_no as 病案号,
                            t.NAME as 姓名,
                            t.in_times  as 次数,
                            CASE WHEN t.CASE_STATE='2'    THEN '出库-复印借出' ELSE      (SELECT MARK FROM COM_DICTIONARY WHERE TYPE='CaseState' AND CODE=t.CASE_STATE) END  as 库房状态,
                            (SELECT NAME FROM COM_DICTIONARY WHERE TYPE='CaseStore' AND CODE=t.STORE_CODE) as 库房名称,
                            (SELECT NAME FROM COM_DICTIONARY WHERE TYPE='CaseCabinet' AND CODE=t.CABINET_CODE) as 库房柜排名称
                             FROM MET_CAS_STORE  t
                            WHERE t.NAME LIKE '{0}'
                            ";
                
                try
                {
                    //查询
                    strSql = string.Format(strSql, patientName);
                }
                catch (Exception ex)
                {
                    this.Err = "SQL语句赋值出错!" + ex.Message;
                    return -1;
                }



                return this.ExecQuery(strSql, ref ds);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }


        }

        #endregion 
    }
}
