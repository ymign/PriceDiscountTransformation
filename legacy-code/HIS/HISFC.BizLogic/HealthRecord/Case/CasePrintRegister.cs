using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.HealthRecord.Case
{
    public class CasePrintRegister : Neusoft.FrameWork.Management.Database
    {
        #region 数据库基本操作
        /// <summary>
        /// 插入病案库房信息
        /// </summary>
        /// <param name="caseStore"></param>
        /// <returns></returns>
        public int InsertCasePrintRegister(Neusoft.HISFC.Models.HealthRecord.Case.CaseStore caseStore)
        {

            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CasePrintRegister.Insert", ref strSql) == -1) return -1;
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
            string[] str = new string[7];
            str[0] = caseStore.ID; //主键序列
            str[1] = caseStore.PatientInfo.PID.PatientNO; //住院号
            str[2] = caseStore.PatientInfo.Name;
            str[3] = caseStore.PatientInfo.InTimes.ToString(); //住院次数
            str[4] = caseStore.OperEnv.Memo;//复印类型 0 医务科审批  1 病案室执行
            str[5] = caseStore.OperEnv.ID;  //操作员
            str[6] = caseStore.OperEnv.OperTime.ToString(); //复印时间
            return str;
        }
        /// <summary>
        /// 更新库房信息
        /// </summary>
        /// <param name="caseStore"></param>
        /// <returns></returns>
        public int UpdateCasePrintRegister(Neusoft.HISFC.Models.HealthRecord.Case.CaseStore caseStore)
        {
            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CasePrintRegister.Update", ref strSql) == -1) return -1;
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

        public int DeleteCasePrintRegister(string CaseNO, string InTimes)
        {

            //string deleteSql = string.Empty;
            //int intReturn = -1;
            //if (this.Sql.GetSql("HealthReacord.Case.CasePrintRegister.Delete", ref deleteSql) == -1) return -1;
            //try
            //{
            //    //插入
            //    deleteSql = string.Format(deleteSql, CaseNO, InTimes);
            //    intReturn = this.ExecNoQuery(deleteSql);
            //}
            //catch (Exception ex)
            //{
            //    this.Err = ex.Message;
            //    return -1;
            //}
            //if (intReturn < 0)
            //{
            //    this.Err = "删除病案库房信息失败" + this.Err;
            //    return -1;
            //}

            return -1;
        }
        /// <summary>
        /// 插入库房明细信息
        /// </summary>
        /// <param name="caseStore"></param>
        /// <param name="CaseId"></param>
        /// <returns></returns>
        public int InsertCasePrintRegisterDetail(Neusoft.FrameWork.Models.NeuObject obj)
        {
            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CasePrintRegisterDetail.Insert", ref strSql) == -1) return -1;
            try
            {
                //插入
                strSql = string.Format(strSql,obj.ID,obj.Memo,obj.Name,obj.User01);
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

        public ArrayList QueryCasePrintRegister(string patientNo)
        {
            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CasePrintRegister.Select", ref strSql) == -1) return null;
            ArrayList al = new ArrayList();
            try
            {
                //查询
                strSql = string.Format(strSql, patientNo);
                this.ExecQuery(strSql);
                Neusoft.HISFC.Models.HealthRecord.Case.CaseStore caseStore = null;

                while (this.Reader.Read())
                {
                    caseStore = new Neusoft.HISFC.Models.HealthRecord.Case.CaseStore();
                    caseStore.ID = this.Reader[0].ToString(); //主键
                    caseStore.PatientInfo.PID.PatientNO = this.Reader[1].ToString(); //住院号
                    caseStore.PatientInfo.Name = this.Reader[2].ToString();        //姓名
                    caseStore.PatientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[3].ToString());//住院次数
                    caseStore.OperEnv.Memo = this.Reader[4].ToString();//复印类型 0 医务科审批  1 病案室执行
                    caseStore.OperEnv.ID = this.Reader[5].ToString();//操作员
                    caseStore.OperEnv.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6].ToString());    //复印时间
                    al.Add(caseStore);
                }
                this.Reader.Close();
                return al;
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                this.Err = ex.Message;
                return null;
            }

        }

        public ArrayList  QueryCasePrintRegisterDetail(string Id)
        {
            string strSql = "";
            if (this.Sql.GetSql("HealthReacord.Case.CasePrintRegisterDetail.Select", ref strSql) == -1) return null;
            ArrayList al = new ArrayList();
            try
            {
                //查询
                strSql = string.Format(strSql, Id);
                this.ExecQuery(strSql);
                Neusoft.FrameWork.Models.NeuObject obj = null;
                
                while (this.Reader.Read())
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = this.Reader[0].ToString(); //主键
                    obj.Memo = this.Reader[1].ToString(); //复印编号
                    obj.Name = this.Reader[2].ToString(); //复印内容
                    obj.User01 = this.Reader[3].ToString();//份数
                    al.Add(obj);
                }
                this.Reader.Close();
                return al;
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                this.Err = ex.Message;
                return null;
            }

        }

        public string  QueryCasePrintRegisterCount(DateTime dtBegin,DateTime dtEnd)
        {
            string strSql = @"  SELECT sum(print_count) FROM MET_CAS_PRINTREGISTERDETAIL 
                                WHERE OPER_ID IN (
                                SELECT OPER_ID  FROM MET_CAS_PRINTREGISTER 
                                WHERE OPER_DATE BETWEEN to_date('{0}','yyyy-mm-dd HH24:mi:ss')  AND to_date('{1}','yyyy-mm-dd HH24:mi:ss')
                                )";
            try
            {
                //查询
                strSql = string.Format(strSql, dtBegin,dtEnd);
                this.ExecQuery(strSql);
                string Sum = "";
                while (this.Reader.Read())
                {
                    Sum = this.Reader[0].ToString(); //主键
                }
                this.Reader.Close();
                return Sum;
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                this.Err = ex.Message;
                return "0";
            }

        }

        public ArrayList QueryCasePrintRegisterDetailCount(DateTime dtBegin, DateTime dtEnd)
        {
            string strSql = @"SELECT  
                            print_code,
                            print_NAME ,
                            sum(print_count) 
                             FROM MET_CAS_PRINTREGISTERDETAIL 
                            WHERE OPER_ID  IN (
                            SELECT OPER_ID  FROM MET_CAS_PRINTREGISTER 
                            WHERE OPER_DATE BETWEEN to_date('{0}','yyyy-mm-dd HH24:mi:ss')  AND to_date('{1}','yyyy-mm-dd HH24:mi:ss')
                            )
                            GROUP BY print_code,
                            print_NAME ";
             ArrayList al = new ArrayList();
            try
            {
                //查询
                strSql = string.Format(strSql, dtBegin,dtEnd);
                this.ExecQuery(strSql);
                Neusoft.FrameWork.Models.NeuObject obj = null;

                while (this.Reader.Read())
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.Memo = this.Reader[0].ToString(); 
                    obj.Name = this.Reader[1].ToString(); 
                    obj.User01 = this.Reader[2].ToString(); 
                    al.Add(obj);
                }
                this.Reader.Close();
                return al;
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                this.Err = ex.Message;
                return null;
            }

        }

        #endregion 
    }
}
