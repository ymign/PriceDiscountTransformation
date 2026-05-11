using System;
using System.Collections;
using System.Data;


namespace Neusoft.HISFC.BizLogic.HealthRecord
{
    /// <summary>
    /// BaseDML 的摘要说明。
    /// </summary>
    public class Base : Neusoft.FrameWork.Management.Database
    {
        public Base()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 病案首页 患者基本信息操作函数

        #region 更新
        /// <summary>
        /// 更新患者在住院主表的登记病案标记
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="caseState">病案状态: 0 无需病案 1 需要病案 2 医生站形成病案 3 病案室形成病案 4病案封存 </param>
        /// <returns> 成功返回</returns>
        public int UpdateMainInfoCaseFlag(string inpatientNO, string caseState)
        {
            string strSQL = "";

            if (Sql.GetSql("CASE.BaseDML.UpdateMainInfoCaseFlag.Update", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, inpatientNO, caseState);
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
        /// 更新患者在住院主表的登记病案标记
        /// </summary>
        /// <param name="inpatientNO">住院流水号 </param>
        /// <param name="caseSendFlag">病案送入病案室否0未1送  </param>
        /// <returns> 成功返回</returns>
        public int UpdateMainInfoCaseSendFlag(string inpatientNO, string caseSendFlag)
        {
            string strSQL = "";

            if (Sql.GetSql("CASE.BaseDML.UpdateMainInfoCaseFlag.Update", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, inpatientNO, caseSendFlag);
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
        /// 更新病案主表
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int UpdateBaseInfo(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            string strSql = "";
            //if (this.Sql.GetSql("CASE.BaseDML.UpdateBaseInfo.Update", ref strSql) == -1) return -1;
            if (this.Sql.GetSql("CASE.BaseDML.UpdateBaseInfo.Update.HIS50", ref strSql) == -1) return -1;

            strSql = string.Format(strSql, GetBaseInfo(b));
            return this.ExecNoQuery(strSql);
            //return this.ExecNoQuery(strSql, GetBaseInfo(b));
        }

        /// <summary>
        /// 更新病案主表
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int UpdateBaseInfoSaveFlag(Neusoft.HISFC.Models.HealthRecord.Base b, bool status)
        {
            string strSql = "";
            if (status && ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID == "9182")
            {
                //strSql = @"UPDATE met_cas_base SET save_flag=save_flag+1 WHERE INPATIENT_NO = '{0}'";
                strSql = @"UPDATE met_cas_base t
                             SET t.save_flag = 1,
                                   t.save_oper = '{1}',
                                   t.last_save = to_date('{2}', 'yyyy-mm-dd hh24:mi:ss'),
                                   t.save_dept='{3}'
                             WHERE INPATIENT_NO = '{0}'";
            }
            else
            {
                strSql = @"UPDATE met_cas_base t
                             SET t.save_flag =0,
                                   t.save_oper = '{1}',
                                   t.last_save = to_date('{2}', 'yyyy-mm-dd hh24:mi:ss'),
                                   t.save_dept='{3}'
                             WHERE INPATIENT_NO = '{0}'";
            }
            return this.ExecNoQuery(strSql, b.PatientInfo.ID, this.Operator.ID, this.GetDateTimeFromSysDateTime().ToString(), ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID);
        }

        /// <summary>
        /// 更新病案主表
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int UpdateBaseInfoSaveFlag2(Neusoft.HISFC.Models.HealthRecord.Base b, bool status)
        {
            string strSql = "";
            if (status && ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID == "9182")
            {
                //strSql = @"UPDATE met_cas_base SET save_flag=save_flag+1 WHERE INPATIENT_NO = '{0}'";
                strSql = @"UPDATE met_cas_base t
                             SET t.save_flag = 1,
                                   t.save_oper = '{1}',
                                   t.last_save = to_date('{2}', 'yyyy-mm-dd hh24:mi:ss'),
                                   t.save_dept='{3}',
                                   t.diag_flag='{4}'
                             WHERE INPATIENT_NO = '{0}'";
            }
            else
            {
                strSql = @"UPDATE met_cas_base t
                             SET t.save_flag =0,
                                   t.save_oper = '{1}',
                                   t.last_save = to_date('{2}', 'yyyy-mm-dd hh24:mi:ss'),
                                   t.save_dept='{3}',
                                   t.diag_flag='{4}'
                             WHERE INPATIENT_NO = '{0}'";
            }
            return this.ExecNoQuery(strSql, b.PatientInfo.ID, this.Operator.ID, this.GetDateTimeFromSysDateTime().ToString(), ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID, b.Diag_Flag);
        }

        /// <summary>
        /// 更新病案主表
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int UpdateBaseInfoDiagFlag(Neusoft.HISFC.Models.HealthRecord.Base b, bool status)
        {
            string strSql = "";
            if (status)
            {
                //strSql = @"UPDATE met_cas_base SET save_flag=save_flag+1 WHERE INPATIENT_NO = '{0}'";
                strSql = @"UPDATE met_cas_base t
                             SET t.diag_flag = 1,
                                   t.save_oper = '{1}',
                                   t.last_save = to_date('{2}', 'yyyy-mm-dd hh24:mi:ss'),
                                   t.save_dept='{3}'
                             WHERE INPATIENT_NO = '{0}'";
            }
            else
            {
                strSql = @"UPDATE met_cas_base t
                             SET t.diag_flag =0,
                                   t.save_oper = '{1}',
                                   t.last_save = to_date('{2}', 'yyyy-mm-dd hh24:mi:ss'),
                                   t.save_dept='{3}'
                             WHERE INPATIENT_NO = '{0}'";
            }
            return this.ExecNoQuery(strSql, b.PatientInfo.ID, this.Operator.ID, this.GetDateTimeFromSysDateTime().ToString(), ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID);
        }

        /// <summary>
        /// 保存标记
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int GetBaseInfoSaveFlag(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            string strSql = "";
            strSql = @"select p.save_flag  from met_cas_base p where p.inpatient_no='{0}' and p.save_oper is not null and p.save_dept='9182'";
            strSql = string.Format(strSql, b.PatientInfo.ID);
            string saveflag = this.ExecSqlReturnOne(strSql, "0");
            return Neusoft.FrameWork.Function.NConvert.ToInt32(saveflag);
        }

        /// <summary>
        /// 录入限制
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int GetBaseInfoDiagFlag(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            string strSql = "";
            strSql = @"select nvl(p.diag_flag,'0') from met_cas_base p where p.inpatient_no='{0}'";
            strSql = string.Format(strSql, b.PatientInfo.ID);
            string saveflag = this.ExecSqlReturnOne(strSql, "0");
            return Neusoft.FrameWork.Function.NConvert.ToInt32(saveflag);
        }

        #endregion

        /// <summary>
        /// 查询未登记病案信息的患者的诊断信息,从met_com_diagnose中提取
        /// </summary>
        /// <param name="inpatientNO">患者住院流水号</param>
        /// <param name="diagType">诊断类别,要提取所有诊断输入%</param>
        /// <returns>诊断信息数组</returns>
        public ArrayList QueryInhosDiagnoseInfo(string inpatientNO, string diagType)
        {
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetInhosDiagInfo.Select", ref strSql) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSql = string.Format(strSql, inpatientNO, diagType);

            return this.myGetDiagInfo(strSql);
        }

        /// <summary>
        /// 从病案基本表中获取信息
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.Base GetCaseBaseInfo(string inpatientNO)
        {
            Neusoft.HISFC.Models.HealthRecord.Base info = new Neusoft.HISFC.Models.HealthRecord.Base();
            //获取主sql语句
            string strSQL = GetCaseSql();
            if (strSQL == null)
            {
                return null;
            }
            string str = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfo.Select.where", ref str) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSQL += str;
            strSQL = string.Format(strSQL, inpatientNO);
            ArrayList arrList = this.myGetCaseBaseInfo(strSQL);
            if (arrList == null)
            {
                return null;
            }
            if (arrList.Count > 0)
            {
                info = (Neusoft.HISFC.Models.HealthRecord.Base)arrList[0];
            }
            return info;
        }

        // {49D021BB-FC88-4070-A67B-4AD6A123EE26}进修医师，实习医师 改为可自定义
        /// <summary>
        /// 从病案基本表中获取信息
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.Base GetCaseBaseInfo2(string inpatientNO)
        {
            Neusoft.HISFC.Models.HealthRecord.Base info = new Neusoft.HISFC.Models.HealthRecord.Base();
            //获取主sql语句
            string strSQL = GetCaseSql2();
            if (strSQL == null)
            {
                return null;
            }
            string str = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfo.Select.where2", ref str) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSQL += str;
            strSQL = string.Format(strSQL, inpatientNO);
            ArrayList arrList = this.myGetCaseBaseInfo(strSQL);
            if (arrList == null)
            {
                return null;
            }
            if (arrList.Count > 0)
            {
                info = (Neusoft.HISFC.Models.HealthRecord.Base)arrList[0];
            }
            return info;
        }

        /// <summary>
        /// 根据病案号获取信息
        /// </summary>
        /// <param name="CaseNo"></param>
        /// <returns></returns>
        public ArrayList QueryCaseBaseInfoByCaseNO(string CaseNo)
        {
            ArrayList list = new ArrayList();
            //获取主sql语句
            string strSQL = GetCaseSql();
            string str = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfoByCaseNum.Select.where", ref str) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSQL += str;
            strSQL = string.Format(strSQL, CaseNo);
            return this.myGetCaseBaseInfo(strSQL);
        }

        /// <summary>
        /// 根据自定义条件查询病案信息
        /// </summary>
        /// <param name="where">where 条件信息</param>
        /// <returns></returns>
        public ArrayList QueryCaseBaseInfoByOwnConditions(string where)
        {
            ArrayList list = new ArrayList();
            //获取主sql语句
            string strSQL = GetCaseSql();
            strSQL += where;
            return this.myGetCaseBaseInfo(strSQL);
        }

        /// <summary>
        /// 向病案主表中插入一条记录
        /// </summary>
        /// <param name="b"></param>
        /// <returns> 成功返回 1 失败返回－1 ，0  </returns>
        public int InsertBaseInfo(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            string strSql = "";
            //if (this.Sql.GetSql("CASE.BaseDML.InsertBaseInfo.Insert", ref strSql) == -1) return -1;
            if (this.Sql.GetSql("CASE.BaseDML.InsertBaseInfo.Insert.HIS50", ref strSql) == -1) return -1;

            return this.ExecNoQuery(strSql, GetBaseInfo(b));
        }

        /// <summary>
        /// 根据住院号和住院次数查询住院流水号 
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="InNum"></param>
        /// <returns></returns>
        public ArrayList QueryPatientInfoByInpatientAndInNum(string inpatientNO, string InNum)
        {
            //先从病案主表中查询 如果没有查到 再在住院主表中查询 
            ArrayList list = new ArrayList();
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetPatientInfo.GetPatientInfo", ref strSql) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSql = string.Format(strSql, inpatientNO, InNum);
            this.ExecQuery(strSql);
            Neusoft.HISFC.Models.RADT.PatientInfo info = null;
            while (this.Reader.Read())
            {
                info = new Neusoft.HISFC.Models.RADT.PatientInfo();
                info.ID = this.Reader[0].ToString();
                list.Add(info);
                info = null;
            }
            if (list == null)
            {
                return list;
            }
            if (list.Count == 0)
            {
                //查询住院主表 获取病人信息
                if (this.Sql.GetSql("RADT.Inpatient.PatientInfoGetByTime", ref strSql) == -1)
                {
                    this.Err = "获取SQL语句失败";
                    return null;
                }
                strSql = string.Format(strSql, inpatientNO, InNum);
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    info = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    info.ID = this.Reader[0].ToString();
                    list.Add(info);
                    info = null;
                }
            }
            return list;
        }
        /// <summary>
        /// 根据住院号查询 病案信息和住院信息
        /// </summary>
        /// <param name="PatientNO"></param>
        /// <returns></returns>
        public ArrayList QueryPatientInfo(string PatientNO)
        {
            //先从病案主表中查询 如果没有查到 再在住院主表中查询 
            ArrayList list = new ArrayList();
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetPatientInfo.GetPatientInfo", ref strSql) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSql = string.Format(strSql, PatientNO);
            this.ExecQuery(strSql);
            Neusoft.HISFC.Models.HealthRecord.Base info = null;
            while (this.Reader.Read())
            {
                info = new Neusoft.HISFC.Models.HealthRecord.Base();
                info.OutDept.Name = this.Reader[0].ToString(); //出院科室
                info.PatientInfo.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[1].ToString()); //出院日期
                info.PatientInfo.Name = this.Reader[2].ToString(); //姓名
                info.PatientInfo.Sex.ID = this.Reader[3].ToString(); //性别
                info.CaseNO = this.Reader[4].ToString(); //病案号
                info.PatientInfo.PID.PatientNO = this.Reader[5].ToString(); //住院号
                info.PatientInfo.ID = this.Reader[6].ToString(); //住院流水号
                info.PatientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[7]); //住院次数
                info.PatientInfo.User01 = this.Reader[8].ToString();
                list.Add(info);
                info = null;
            }

            return list;
        }

        /// <summary>
        /// 根据姓名查询 病案信息和住院信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ArrayList QueryPatientInfoByName(string name)
        {
            //先从病案主表中查询 如果没有查到 再在住院主表中查询 
            ArrayList list = new ArrayList();
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetPatientInfo.GetPatientInfo.ByName", ref strSql) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSql = string.Format(strSql, name);
            this.ExecQuery(strSql);
            Neusoft.HISFC.Models.HealthRecord.Base info = null;
            while (this.Reader.Read())
            {
                info = new Neusoft.HISFC.Models.HealthRecord.Base();
                info.OutDept.Name = this.Reader[0].ToString(); //出院科室
                info.PatientInfo.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[1].ToString()); //出院日期
                info.PatientInfo.Name = this.Reader[2].ToString(); //姓名
                info.PatientInfo.Sex.ID = this.Reader[3].ToString(); //性别
                info.CaseNO = this.Reader[4].ToString(); //病案号
                info.PatientInfo.PID.PatientNO = this.Reader[5].ToString(); //住院号
                info.PatientInfo.ID = this.Reader[6].ToString(); //住院流水号
                info.PatientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[7]); //住院次数
                info.PatientInfo.User01 = this.Reader[8].ToString();
                list.Add(info);
                info = null;
            }

            return list;
        }

        /// <summary>
        /// 判断是否在海泰新电子病历系统存在病案首页数据
        /// </summary>
        /// <returns></returns>
        public bool IsExistNewEmrHomeMedicalRecord(string inpatientNo)
        {
            try
            {
                string sql = string.Format(@" select count(1) from  view_emr_front_sheet@DBLINK_HAITAI p where p.ipid='{0}' ", inpatientNo);
                string result = this.ExecSqlReturnOne(sql, "");
                if (string.IsNullOrEmpty(result) || result == "0")
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Err = "查询海泰新电子病历系统病案首页数据出现异常：" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 更新met_cas_base的门诊诊断 入院诊断 出院主要诊断 第一手术
        /// </summary>
        /// <param name="inpatienNO"></param>
        /// <param name="ClinicDiagName"></param>
        /// <param name="InHospDiagName"></param>
        /// <param name="frmType"></param>
        /// <returns></returns>
        public int UpdateBaseDiagAndOperationNew(string inpatienNO, string ClinicDiagCode, string ClinicDiagName, string InHospDiagName, Neusoft.HISFC.Models.HealthRecord.EnumServer.frmTypes frmType)
        {

            Neusoft.HISFC.BizLogic.HealthRecord.Diagnose dia = new Neusoft.HISFC.BizLogic.HealthRecord.Diagnose();
            Neusoft.HISFC.BizLogic.HealthRecord.Operation op = new Operation();
            if (this.Trans != null)
            {
                dia.SetTrans(Trans);
                op.SetTrans(Trans);
            }
            Neusoft.HISFC.Models.HealthRecord.Diagnose ClinicDiag = dia.GetFirstDiagnose(inpatienNO, Neusoft.HISFC.Models.HealthRecord.DiagnoseType.enuDiagnoseType.CLINIC, frmType);
            Neusoft.HISFC.Models.HealthRecord.Diagnose InhosDiag = dia.GetFirstDiagnose(inpatienNO, Neusoft.HISFC.Models.HealthRecord.DiagnoseType.enuDiagnoseType.IN, frmType);
            Neusoft.HISFC.Models.HealthRecord.Diagnose OutDiag = dia.GetFirstDiagnose(inpatienNO, Neusoft.HISFC.Models.HealthRecord.DiagnoseType.enuDiagnoseType.OUT, frmType);
            Neusoft.HISFC.Models.HealthRecord.OperationDetail ops = op.GetFirstOperation(inpatienNO, frmType);
            if (ClinicDiag == null || InhosDiag == null || OutDiag == null || ops == null)
            {
                return -1;
            }
            string[] str = new string[14];
            str[0] = inpatienNO;

            //if (string.IsNullOrEmpty(ClinicDiagCode) || string.IsNullOrEmpty(ClinicDiagName))
            //{
            //    return -1;
            //}
            if (ClinicDiag != null && ClinicDiag.DiagInfo.ICD10.ID != "")
            {
                str[1] = ClinicDiag.DiagInfo.ICD10.ID;
                str[2] = ClinicDiag.DiagInfo.ICD10.Name;
            }
            else
            {
                str[1] = ClinicDiagCode;
                str[2] = ClinicDiagName;
            }
            if (InhosDiag != null && InhosDiag.DiagInfo.ICD10.ID != "")
            {
                str[3] = InhosDiag.DiagInfo.ICD10.ID;
                str[4] = InhosDiag.DiagInfo.ICD10.Name;
            }
            else
            {
                str[3] = "";
                str[4] = InHospDiagName;
            }
            str[5] = OutDiag.DiagInfo.ICD10.ID;
            str[6] = OutDiag.DiagInfo.ICD10.Name;
            str[7] = OutDiag.DiagOutState;
            str[8] = OutDiag.CLPA;
            str[9] = ops.OperationInfo.ID;
            str[10] = ops.OperationInfo.Name;
            str[11] = ops.FirDoctInfo.ID;
            str[12] = ops.FirDoctInfo.Name;
            str[13] = ops.OperationDate.ToString();
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.UpdateBaseDiagAndOperation", ref strSql) == -1) return -1;

            strSql = string.Format(strSql, str);
            return this.ExecNoQuery(strSql);

        }
        /// <summary>
        /// 更新诊断表和手术表中的出院日期和出院科室 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int DiagnoseAndOperation(Neusoft.FrameWork.Models.NeuObject obj, string InpatientNo)
        {
            //obj.User01 出院日期
            //obj.User02 出院科室
            string strSql1 = "";
            string strSql2 = "";
            //诊断
            if (this.Sql.GetSql("CASE.Diagnose.DiagnoseAndOperation.1", ref strSql1) == -1) return -1;
            //手术 
            if (this.Sql.GetSql("CASE.Diagnose.DiagnoseAndOperation.2", ref strSql2) == -1) return -1;
            strSql1 = string.Format(strSql1, InpatientNo, obj.User01);
            strSql2 = string.Format(strSql2, InpatientNo, obj.User01, obj.User02);
            if (this.ExecNoQuery(strSql1) != -1)
            {
                return this.ExecNoQuery(strSql2);
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 判断某个住院号的某次入院是否已经存在
        /// </summary>
        /// <param name="InpatientNO"></param>
        /// <param name="PatientNo"></param>
        /// <param name="InNum"></param>
        /// <returns>没有记录 返回 0 ,查询失败返回-1 ,住院号,住院流水号,住院次数全相同 返回 1 住院号住院次数相同 ,住院流水号不同 返回2</returns>
        public int ExistCase(string InpatientNO, string PatientNo, string InNum)
        {
            string strSQL = GetCaseSql();
            string str = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfoByCaseNum.Select.ExistCase", ref str) == -1)
            {
                this.Err = "获取SQL语句失败";
                return -1;
            }
            strSQL += str;
            strSQL = string.Format(strSQL, PatientNo, InNum);
            ArrayList List = this.myGetCaseBaseInfo(strSQL);
            if (List == null)
            {
                return -1; //查询出错 
            }
            if (List.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.HealthRecord.Base obj in List)
                {
                    if (obj.PatientInfo.ID == InpatientNO) //住院流水号相同 住院号相同 住院次数相同 
                    {
                        return 1; //一般执行更新操作 
                    }
                }
                return 2; //住院号相同,住院次数相同 住院流水号不同 ,一般是住院次数填写错了 
            }
            return 0; //没有查到相关的记录 一般执行插入操作
        }

        /// <summary>
        /// 获取一段时间的患者
        /// </summary>
        /// <param name="BeginTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="DeptCode">科室编码</param>
        /// <returns></returns>
        public ArrayList QueryPatientOutHospital(string BeginTime, string EndTime, string DeptCode)
        {
            ArrayList list = new ArrayList();
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.QueryPatientOutHospital", ref strSql) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSql = string.Format(strSql, BeginTime, EndTime, DeptCode);
            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.RADT.PatientInfo patientObj = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    patientObj.Name = this.Reader[0].ToString();
                    patientObj.PID.PatientNO = this.Reader[1].ToString();
                    patientObj.ID = this.Reader[2].ToString();
                    list.Add(patientObj);
                }
                this.Reader.Close();
            }
            catch (Exception ex)
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                this.Err = "获得患者住院诊断信息出错!" + ex.Message;
                return null;
            }
            return list;
        }

        /// <summary>
        /// 获取一段时间的患者
        /// </summary>
        /// <param name="BeginTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="DeptCode">科室编码</param>
        /// <returns></returns>
        public ArrayList QueryPatientOutHospital(string BeginTime, string EndTime, string DeptCode, string statues, string name, string patientno)
        {
            ArrayList list = new ArrayList();
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.QueryPatientOutHospitalDiag", ref strSql) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSql = string.Format(strSql, BeginTime, EndTime, DeptCode, statues, name, patientno);
            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.RADT.PatientInfo patientObj = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    patientObj.Name = this.Reader[0].ToString();
                    patientObj.PID.PatientNO = this.Reader[1].ToString();
                    patientObj.ID = this.Reader[2].ToString();
                    patientObj.Memo = this.Reader[3].ToString();
                    list.Add(patientObj);
                }
                this.Reader.Close();
            }
            catch (Exception ex)
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                this.Err = "获得患者住院诊断信息出错!" + ex.Message;
                return null;
            }
            return list;
        }

        /// <summary>
        /// 获取一段时间的患者
        /// </summary>
        /// <param name="BeginTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="DeptCode">科室编码</param>
        /// <returns></returns>
        public ArrayList QueryPatientOutHospitalNotCommit(string BeginTime, string EndTime, string DeptCode, string statues, string name, string patientno)
        {
            ArrayList list = new ArrayList();
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.QueryPatientOutHospitalNotCommit", ref strSql) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSql = string.Format(strSql, BeginTime, EndTime, DeptCode, statues, name, patientno);
            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.RADT.PatientInfo patientObj = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    patientObj.Name = this.Reader[0].ToString();
                    patientObj.PID.PatientNO = this.Reader[1].ToString();
                    patientObj.ID = this.Reader[2].ToString();
                    patientObj.Memo = this.Reader[3].ToString();
                    list.Add(patientObj);
                }
                this.Reader.Close();
            }
            catch (Exception ex)
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                this.Err = "获得患者住院诊断信息出错!" + ex.Message;
                return null;
            }
            return list;
        }
        /// <summary>
        /// 获取手工录入病案时的住院流水号
        /// </summary>
        /// <returns></returns>
        public string GetCaseInpatientNO()
        {
            string str = this.GetSequence("CASE.BaseDML.GetCaseInpatientNO");
            if (str == null || str == "")
            {
                return str;
            }
            else
            {
                str = "BA" + str.PadLeft(12, '0');
            }
            return str;
        }
        #endregion

        //{7D094A18-0FC9-4e8b-A8E6-901E55D4C20C}

        #region  私有函数

        /// <summary>
        /// 将实体 转变成字符串数组
        /// </summary>
        /// <param name="b"> 病案的实体类</param>
        /// <returns>失败返回null</returns>
        private string[] GetBaseInfo(Neusoft.HISFC.Models.HealthRecord.Base b)
        {
            string[] s = new string[196];
            try
            {
                s[0] = b.PatientInfo.ID;//住院流水号

                s[1] = b.PatientInfo.PID.PatientNO;//住院病历号

                s[2] = b.PatientInfo.PID.CardNO;//卡号

                s[3] = b.PatientInfo.Name;//姓名

                s[4] = b.Nomen;//曾用名

                s[5] = b.PatientInfo.Sex.ID.ToString();//性别

                s[6] = b.PatientInfo.Birthday.ToString();//出生日期

                s[7] = b.PatientInfo.Country.ID;//国家

                s[8] = b.PatientInfo.Nationality.ID;//民族

                s[9] = b.PatientInfo.Profession.ID;//职业

                s[10] = b.PatientInfo.BloodType.ID.ToString();//血型编码

                s[11] = b.PatientInfo.MaritalStatus.ID.ToString();//婚否

                s[12] = b.PatientInfo.Age.ToString();//年龄

                s[13] = b.AgeUnit;//年龄单位

                s[14] = b.PatientInfo.IDCard;//身份证号

                s[15] = b.PatientInfo.PVisit.InSource.ID;//地区来源

                s[16] = b.PatientInfo.Pact.PayKind.ID;//结算类别号

                s[17] = b.PatientInfo.Pact.ID;//合同代码

                s[18] = b.PatientInfo.SSN;//医保公费号

                s[19] = b.PatientInfo.DIST;//籍贯

                s[20] = b.PatientInfo.AreaCode;//出生地

                s[21] = b.PatientInfo.AddressHome;//家庭住址

                s[22] = b.PatientInfo.PhoneHome;//家庭电话

                s[23] = b.PatientInfo.HomeZip;//住址邮编

                s[24] = b.PatientInfo.AddressBusiness;//单位地址

                s[25] = b.PatientInfo.PhoneBusiness;//单位电话

                s[26] = b.PatientInfo.BusinessZip;//单位邮编

                s[27] = b.PatientInfo.Kin.Name;//联系人

                s[28] = b.PatientInfo.Kin.RelationLink;//与患者关系

                s[29] = b.PatientInfo.Kin.RelationPhone;//联系电话

                s[30] = b.PatientInfo.Kin.RelationAddress;//联系地址

                s[31] = b.ClinicDoc.ID;//门诊诊断医生

                s[32] = b.ClinicDoc.Name;//门诊诊断医生姓名

                s[33] = b.ComeFrom;//转来医院

                s[34] = b.PatientInfo.PVisit.InTime.ToString();//入院日期

                s[35] = b.PatientInfo.InTimes.ToString();//住院次数

                s[36] = b.InDept.ID;//入院科室代码

                s[37] = b.InDept.Name;//入院科室名称

                s[38] = b.PatientInfo.PVisit.InSource.ID;//入院来源

                s[39] = b.PatientInfo.PVisit.Circs.ID;//入院状态

                s[40] = b.DiagDate.ToString();//确诊日期

                s[41] = b.OperationDate.ToString();//手术日期

                s[42] = b.PatientInfo.PVisit.OutTime.ToString();//出院日期

                s[43] = b.OutDept.ID;//出院科室代码

                s[44] = b.OutDept.Name;//出院科室名称

                s[45] = b.PatientInfo.PVisit.ZG.ID;//转归代码

                s[46] = b.DiagDays.ToString();//确诊天数

                s[47] = b.InHospitalDays.ToString();//住院天数

                s[48] = b.DeadDate.ToString();//死亡日期

                s[49] = b.DeadReason;//死亡原因

                s[50] = b.CadaverCheck;//尸检

                s[51] = b.DeadKind;//死亡种类

                s[52] = b.BodyAnotomize;//尸体解剖号

                s[53] = b.Hbsag;//乙肝表面抗原

                s[54] = b.HcvAb;//丙肝病毒抗体

                s[55] = b.HivAb;//获得性人类免疫缺陷病毒抗体

                s[56] = b.CePi;//门急_入院符合

                s[57] = b.PiPo;//入出_院符合

                s[58] = b.OpbOpa;//术前_后符合

                s[59] = b.ClX;//临床_X光符合

                s[60] = b.ClCt;//临床_CT符合

                s[61] = b.ClMri;//临床_MRI符合

                s[62] = b.ClPa;//临床_病理符合

                s[63] = b.FsBl;//放射_病理符合

                s[64] = b.SalvTimes.ToString();//抢救次数

                s[65] = b.SuccTimes.ToString();//成功次数

                s[66] = b.TechSerc;//示教科研

                s[67] = b.VisiStat;//是否随诊

                s[68] = b.VisiPeriod.ToString();//随访期限

                s[69] = b.InconNum.ToString();//院际会诊次数 70 远程会诊次数

                s[70] = b.OutconNum.ToString();//院际会诊次数 70 远程会诊次数

                s[71] = b.AnaphyFlag;//药物过敏

                s[72] = b.FirstAnaphyPharmacy.ID;//过敏药物名称

                s[73] = b.SecondAnaphyPharmacy.ID;//过敏药物名称

                s[74] = b.CoutDate.ToString();//更改后出院日期

                s[75] = b.PatientInfo.PVisit.AdmittingDoctor.ID;//住院医师代码

                s[76] = b.PatientInfo.PVisit.AdmittingDoctor.Name;//住院医师姓名

                s[77] = b.PatientInfo.PVisit.AttendingDoctor.ID;//主治医师代码

                s[78] = b.PatientInfo.PVisit.AttendingDoctor.Name;//主治医师姓名

                s[79] = b.PatientInfo.PVisit.ConsultingDoctor.ID;//主任医师代码

                s[80] = b.PatientInfo.PVisit.ConsultingDoctor.Name;//主任医师姓名

                s[81] = b.PatientInfo.PVisit.ReferringDoctor.ID;//科主任代码

                s[82] = b.PatientInfo.PVisit.ReferringDoctor.Name;//科主任名称

                s[83] = b.RefresherDoc.ID;//进修医师代码

                s[84] = b.RefresherDoc.Name;//进修医生名称

                s[85] = b.GraduateDoc.ID;//研究生实习医师代码

                s[86] = b.GraduateDoc.Name;//研究生实习医师名称

                s[87] = b.PatientInfo.PVisit.TempDoctor.ID;//实习医师代码

                s[88] = b.PatientInfo.PVisit.TempDoctor.Name;//实习医师名称

                s[89] = b.CodingOper.ID;//编码员代码

                s[90] = b.CodingOper.Name;//编码员名称

                s[91] = b.MrQuality;//病案质量

                s[92] = b.MrEligible;//合格病案

                s[93] = b.QcDoc.ID;//质控医师代码

                s[94] = b.QcDoc.Name;//质控医师名称

                s[95] = b.QcNurse.ID;//质控护士代码

                s[96] = b.QcNurse.Name;//质控护士名称

                s[97] = b.CheckDate.ToString();//检查时间

                s[98] = b.YnFirst;//手术操作治疗检查诊断为本院第一例项目

                s[99] = b.RhBlood;//Rh血型(阴阳)

                s[100] = b.ReactionBlood;//输血反应（有无）

                s[101] = b.BloodRed;//红细胞数

                s[102] = b.BloodPlatelet;//血小板数

                s[103] = b.BloodPlasma;//血浆数

                s[104] = b.BloodWhole;//全血数

                s[105] = b.BloodOther;//其他输血数

                s[106] = b.XNum;//X光号

                s[107] = b.CtNum;//CT号

                s[108] = b.MriNum;//MRI号

                s[109] = b.PathNum;//病理号

                s[110] = b.DsaNum;//DSA号

                s[111] = b.PetNum;//PET号

                s[112] = b.EctNum;//ECT号

                s[113] = b.XQty.ToString();//X线次数

                s[114] = b.CTQty.ToString();//CT次数

                s[115] = b.MRQty.ToString();//MR次数

                s[116] = b.DSAQty.ToString();//DSA次数

                s[117] = b.PetQty.ToString();//PET次数

                s[118] = b.EctQty.ToString();//ECT次数

                s[119] = b.PatientInfo.Memo;//说明

                s[120] = b.BarCode;//归档条码号

                s[121] = b.LendStat;//病案借阅状态(O借出 I在架)

                s[122] = b.PatientInfo.CaseState;//病案状态1科室质检2登记保存3整理4病案室质检5无效

                s[123] = b.OperInfo.ID;//操作员

                //				s[124]=b.OperDate.ToString() ;//操作时间
                s[124] = b.VisiPeriodWeek; //随访期限 周
                s[125] = b.VisiPeriodMonth; //随访期限 月
                s[126] = b.VisiPeriodYear;//随访期限 年
                s[127] = b.SpecalNus.ToString();  // 特殊护理(日)                                        
                s[128] = b.INus.ToString(); //I级护理时间(日)                                     
                s[129] = b.IINus.ToString(); //II级护理时间(日)                                    
                s[130] = b.IIINus.ToString(); //III级护理时间(日)                                   
                s[131] = b.StrictNuss.ToString(); //重症监护时间( 小时)                                 
                s[132] = b.SuperNus.ToString(); //特级护理时间(小时)     
                s[133] = b.PackupMan.ID; //整理员
                s[134] = b.Disease30; //单病种 
                s[135] = b.IsHandCraft;//手工录入病案 标志

                s[136] = b.ClinicDiag.ID;
                s[137] = b.ClinicDiag.Name;
                s[138] = b.InHospitalDiag.ID;
                s[139] = b.InHospitalDiag.Name;
                s[140] = b.OutDiag.ID;//出院主诊断 编码
                s[141] = b.OutDiag.Name;//出院主诊断 名称
                s[142] = b.OutDiag.User01;//出院主诊断 治疗情况
                s[143] = b.OutDiag.User02;//出院主诊断病理符合情况
                s[144] = b.FirstOperation.ID;//第一主手术代码
                s[145] = b.FirstOperation.Name;//第一主手术名称
                s[146] = b.FirstOperationDoc.ID;//第一主手术医师代码
                s[147] = b.FirstOperationDoc.Name;//第一主手术医师名称
                s[148] = b.SyndromeFlag; //是否有并犯症
                s[149] = b.InfectionNum.ToString();//院内感染次数 
                s[150] = b.OperationCoding.ID;//手术编码员 
                s[151] = b.InfectionPosition.ID; //院内感染部位编码
                s[152] = b.InfectionPosition.Name; //院内感染部位名称

                s[153] = b.PathologicalDiagCode;//病理诊断编码-广医2010-2-2
                s[154] = b.PathologicalDiagName;//病理诊断名称-广医2010-2-2
                s[155] = b.InjuryOrPoisoningCauseCode;//损伤中毒的外部因素编码-广医2010-2-2
                s[156] = b.InjuryOrPoisoningCause;//损伤中毒的外部因素-广医2010-2-2

                s[157] = b.CaseNO;//病案号
                s[158] = b.Out_Type; //出院方式（1、常规 2、自动 3、转院）
                s[159] = b.Cure_Type; //治疗类别（1、中      2、西      3、中西）
                s[160] = b.Use_CHA_Med; //自制中药制剂（0、未知   1、有    2、无）
                s[161] = b.Save_Type; //抢救方法（1、中     2、西       3、中西）
                s[162] = b.Ever_Sickintodeath; //是否出现危重（１、是　　　０、否）
                s[163] = b.Ever_Firstaid; //是否出现急症（１、是　　　０、否）
                s[164] = b.Ever_Difficulty; //是否出现疑难情况（１、是　０、否）
                s[165] = b.ReactionLiquid; //输液反应（１、有　２、无　３、未输）
                s[166] = b.InfectionDiseasesReport; //传染病报告
                s[167] = b.FourDiseasesReport; //四病报告
                s[168] = b.OutDept.Memo;//转往何医院

                s[169] = b.BabyAge;//不足一周岁年龄 
                s[170] = b.BabyBirthWeight;//新生儿出生体重 
                s[171] = b.BabyInWeight; //新生儿入院体重
                s[172] = b.DutyNurse.ID; //责任护士编码
                s[173] = b.DutyNurse.Name;//责任护士名称
                s[174] = b.HighReceiveHopital;//接收医疗机构
                s[175] = b.LowerReceiveHopital;//接收社区
                s[176] = b.ComeBackInMonth;//出院31天内再住院标志 1 无 2 有
                s[177] = b.ComeBackPurpose;//31天再住院目的
                s[178] = b.OutComeDay.ToString(); //颅脑损伤患者昏迷时间 入院前天
                s[179] = b.OutComeHour.ToString(); //颅脑损伤患者昏迷时间 入院前小时
                s[180] = b.OutComeMin.ToString(); //颅脑损伤患者昏迷时间 入院前分钟
                s[181] = b.InComeDay.ToString(); //颅脑损伤患者昏迷时间 入院后天
                s[182] = b.InComeHour.ToString(); //颅脑损伤患者昏迷时间 入院后小时
                s[183] = b.InComeMin.ToString(); //颅脑损伤患者昏迷时间 入院后分钟
                s[184] = b.Dept_Change; //转科科别
                s[185] = b.PatientInfo.Kin.Memo; //与关系备注
                s[186] = b.CurrentAddr;//现住址
                s[187] = b.CurrentPhone;//现住址电话
                s[188] = b.CurrentZip;//现住址邮编
                s[189] = b.InRoom;//入院病房
                s[190] = b.OutRoom;//出院病房
                s[191] = b.InPath;//入院途径 1急诊 2门诊 3其他医疗机构转入 9其他
                s[192] = b.ExampleType;//病例分型 A一般 B急 C疑难 D危重 
                s[193] = b.ClinicPath;//临床路径病例 1是 2否
                s[194] = b.UploadStatu;//广东省病案上传标志 1 未上传 2已上传
                s[195] = b.IsDrgs;//drgs后的数据 0 非 1 是 
                return s;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }

        }
        /// <summary>
        /// 获取主sql语句
        /// </summary>
        /// <returns></returns>
        private string GetCaseSql()
        {
            string strSQL = "";
            //if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfo.Select", ref strSQL) == -1)
            if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfo.Select.HIS50", ref strSQL) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            return strSQL;
        }

        /// <summary>
        /// 获取主sql语句
        /// </summary>
        /// <returns></returns>
        private string GetCaseSql2()
        {
            string strSQL = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfo.Select.HIS50.2", ref strSQL) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            return strSQL;
        }
        /// <summary>
        /// 根据SQL查询符合条件病案首页的信息
        /// zhangjunyi@neusoft.com 修改
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>失败返回 null 成功返回符合条件的信息</returns>
        private ArrayList myGetCaseBaseInfo(string strSQL)
        {
            //执行操查询操作
            this.ExecQuery(strSQL);
            //读取数据
            //			Neusoft.HISFC.Models.HealthRecord.Base b = ReaderBase();
            ArrayList list = new ArrayList();
            Neusoft.HISFC.Models.HealthRecord.Base b = null;
            try
            {
                while (this.Reader.Read())
                {
                    b = new Neusoft.HISFC.Models.HealthRecord.Base();
                    b.PatientInfo.ID = this.Reader[0] == DBNull.Value ? string.Empty : this.Reader[0].ToString();//住院流水号
                    b.PatientInfo.PID.PatientNO = this.Reader[1] == DBNull.Value ? string.Empty : this.Reader[1].ToString();//住院病历号

                    b.PatientInfo.PID.CardNO = this.Reader[2] == DBNull.Value ? string.Empty : this.Reader[2].ToString();//门诊号

                    b.PatientInfo.Name = this.Reader[3] == DBNull.Value ? string.Empty : this.Reader[3].ToString();//姓名
                    b.PatientInfo.Name = this.Reader[3] == DBNull.Value ? string.Empty : this.Reader[3].ToString();
                    b.PatientInfo.PID.Name = this.Reader[3] == DBNull.Value ? string.Empty : this.Reader[3].ToString();

                    b.Nomen = this.Reader[4] == DBNull.Value ? string.Empty : this.Reader[4].ToString();//曾用名

                    b.PatientInfo.Sex.ID = this.Reader[5] == DBNull.Value ? string.Empty : this.Reader[5].ToString();//性别

                    b.PatientInfo.Birthday = System.Convert.ToDateTime(this.Reader[6] == DBNull.Value ? "0001-01-01" : this.Reader[6].ToString());//出生日期

                    b.PatientInfo.Country.ID = this.Reader[7] == DBNull.Value ? string.Empty : this.Reader[7].ToString();//国家

                    b.PatientInfo.Nationality.ID = this.Reader[8] == DBNull.Value ? string.Empty : this.Reader[8].ToString();//民族

                    b.PatientInfo.Profession.ID = this.Reader[9] == DBNull.Value ? string.Empty : this.Reader[9].ToString();//职业

                    b.PatientInfo.BloodType.ID = this.Reader[10] == DBNull.Value ? string.Empty : this.Reader[10].ToString();//血型编码

                    b.PatientInfo.MaritalStatus.ID = this.Reader[11] == DBNull.Value ? string.Empty : this.Reader[11].ToString();//婚否

                    b.PatientInfo.Age = this.Reader[12] == DBNull.Value ? string.Empty : this.Reader[12].ToString();//年龄

                    b.AgeUnit = this.Reader[13] == DBNull.Value ? string.Empty : this.Reader[13].ToString();//年龄单位

                    b.PatientInfo.IDCard = this.Reader[14] == DBNull.Value ? string.Empty : this.Reader[14].ToString();//身份证号

                    b.PatientInfo.PVisit.InSource.ID = this.Reader[15] == DBNull.Value ? string.Empty : this.Reader[15].ToString();//地区来源

                    b.PatientInfo.Pact.PayKind.ID = this.Reader[16] == DBNull.Value ? string.Empty : this.Reader[16].ToString();//结算类别号

                    b.PatientInfo.Pact.ID = this.Reader[17] == DBNull.Value ? string.Empty : this.Reader[17].ToString();//合同代码

                    b.PatientInfo.SSN = this.Reader[18] == DBNull.Value ? string.Empty : this.Reader[18].ToString();//医保公费号

                    b.PatientInfo.DIST = this.Reader[19] == DBNull.Value ? string.Empty : this.Reader[19].ToString();//籍贯

                    b.PatientInfo.AreaCode = this.Reader[20] == DBNull.Value ? string.Empty : this.Reader[20].ToString();//出生地

                    b.PatientInfo.AddressHome = this.Reader[21] == DBNull.Value ? string.Empty : this.Reader[21].ToString();//家庭住址

                    b.PatientInfo.PhoneHome = this.Reader[22] == DBNull.Value ? string.Empty : this.Reader[22].ToString();//家庭电话

                    b.PatientInfo.HomeZip = this.Reader[23] == DBNull.Value ? string.Empty : this.Reader[23].ToString();//住址邮编

                    b.PatientInfo.AddressBusiness = this.Reader[24] == DBNull.Value ? string.Empty : this.Reader[24].ToString();//单位地址

                    b.PatientInfo.PhoneBusiness = this.Reader[25] == DBNull.Value ? string.Empty : this.Reader[25].ToString();//单位电话

                    b.PatientInfo.BusinessZip = this.Reader[26] == DBNull.Value ? string.Empty : this.Reader[26].ToString();//单位邮编

                    b.PatientInfo.Kin.Name = this.Reader[27] == DBNull.Value ? string.Empty : this.Reader[27].ToString();//联系人

                    b.PatientInfo.Kin.RelationLink = this.Reader[28] == DBNull.Value ? string.Empty : this.Reader[28].ToString();//与患者关系

                    b.PatientInfo.Kin.RelationPhone = this.Reader[29] == DBNull.Value ? string.Empty : this.Reader[29].ToString();//联系电话

                    b.PatientInfo.Kin.RelationAddress = this.Reader[30] == DBNull.Value ? string.Empty : this.Reader[30].ToString();//联系地址

                    b.ClinicDoc.ID = this.Reader[31] == DBNull.Value ? string.Empty : this.Reader[31].ToString();//门诊诊断医生

                    b.ClinicDoc.Name = this.Reader[32] == DBNull.Value ? string.Empty : this.Reader[32].ToString();//门诊诊断医生姓名

                    b.ComeFrom = this.Reader[33] == DBNull.Value ? string.Empty : this.Reader[33].ToString();//转来医院

                    b.PatientInfo.PVisit.InTime = System.Convert.ToDateTime(this.Reader[34] == DBNull.Value ? "0001-01-01" : this.Reader[34].ToString());//入院日期

                    b.PatientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[35] == DBNull.Value ? "1" : this.Reader[35].ToString());//住院次数

                    b.InDept.ID = this.Reader[36] == DBNull.Value ? string.Empty : this.Reader[36].ToString();//入院科室代码

                    b.InDept.Name = this.Reader[37] == DBNull.Value ? string.Empty : this.Reader[37].ToString();//入院科室名称

                    b.PatientInfo.PVisit.InSource.ID = this.Reader[38] == DBNull.Value ? string.Empty : this.Reader[38].ToString();//入院来源

                    b.PatientInfo.PVisit.Circs.ID = this.Reader[39] == DBNull.Value ? string.Empty : this.Reader[39].ToString();//入院状态

                    b.DiagDate = System.Convert.ToDateTime(this.Reader[40] == DBNull.Value ? "0001-01-01" : this.Reader[40].ToString());//确诊日期

                    b.OperationDate = System.Convert.ToDateTime(this.Reader[41] == DBNull.Value ? "0001-01-01" : this.Reader[41].ToString());//手术日期

                    b.PatientInfo.PVisit.OutTime = System.Convert.ToDateTime(this.Reader[42] == DBNull.Value ? "0001-01-01" : this.Reader[42].ToString());//出院日期

                    b.OutDept.ID = this.Reader[43] == DBNull.Value ? string.Empty : this.Reader[43].ToString();//出院科室代码

                    b.OutDept.Name = this.Reader[44] == DBNull.Value ? string.Empty : this.Reader[44].ToString();//出院科室名称

                    b.PatientInfo.PVisit.ZG.ID = this.Reader[45] == DBNull.Value ? string.Empty : this.Reader[45].ToString();//转归代码

                    b.DiagDays = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[46] == DBNull.Value ? "1" : this.Reader[46].ToString());//确诊天数

                    b.InHospitalDays = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[47] == DBNull.Value ? "0" : this.Reader[47].ToString());//住院天数

                    b.DeadDate = System.Convert.ToDateTime(this.Reader[48] == DBNull.Value ? "0001-01-01" : this.Reader[48].ToString());//死亡日期

                    b.DeadReason = this.Reader[49] == DBNull.Value ? string.Empty : this.Reader[49].ToString();//死亡原因

                    b.CadaverCheck = this.Reader[50] == DBNull.Value ? string.Empty : this.Reader[50].ToString();//尸检

                    b.DeadKind = this.Reader[51] == DBNull.Value ? string.Empty : this.Reader[51].ToString();//死亡种类

                    b.BodyAnotomize = this.Reader[52] == DBNull.Value ? string.Empty : this.Reader[52].ToString();//尸体解剖号

                    b.Hbsag = this.Reader[53] == DBNull.Value ? string.Empty : this.Reader[53].ToString();//乙肝表面抗原

                    b.HcvAb = this.Reader[54] == DBNull.Value ? string.Empty : this.Reader[54].ToString();//丙肝病毒抗体

                    b.HivAb = this.Reader[55] == DBNull.Value ? string.Empty : this.Reader[55].ToString();//获得性人类免疫缺陷病毒抗体

                    b.CePi = this.Reader[56] == DBNull.Value ? string.Empty : this.Reader[56].ToString();//门急_入院符合

                    b.PiPo = this.Reader[57] == DBNull.Value ? string.Empty : this.Reader[57].ToString();//入出_院符合

                    b.OpbOpa = this.Reader[58] == DBNull.Value ? string.Empty : this.Reader[58].ToString();//术前_后符合

                    b.ClX = this.Reader[59] == DBNull.Value ? string.Empty : this.Reader[59].ToString();//临床_X光符合

                    b.ClCt = this.Reader[60] == DBNull.Value ? string.Empty : this.Reader[60].ToString();//临床_CT符合

                    b.ClMri = this.Reader[61] == DBNull.Value ? string.Empty : this.Reader[61].ToString();//临床_MRI符合

                    b.ClPa = this.Reader[62] == DBNull.Value ? string.Empty : this.Reader[62].ToString();//临床_病理符合

                    b.FsBl = this.Reader[63] == DBNull.Value ? string.Empty : this.Reader[63].ToString();//放射_病理符合

                    b.SalvTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[64] == DBNull.Value ? "0" : this.Reader[64].ToString());//抢救次数

                    b.SuccTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[65] == DBNull.Value ? "0" : this.Reader[65].ToString());//成功次数

                    b.TechSerc = this.Reader[66] == DBNull.Value ? string.Empty : this.Reader[66].ToString();//示教科研

                    b.VisiStat = this.Reader[67] == DBNull.Value ? string.Empty : this.Reader[67].ToString();//是否随诊

                    b.VisiPeriod = System.Convert.ToDateTime(this.Reader[68] == DBNull.Value ? "0001-01-01" : this.Reader[68].ToString());//随访期限

                    b.InconNum = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[69] == DBNull.Value ? "0" : this.Reader[69].ToString());//院际会诊次数 

                    b.OutconNum = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[70] == DBNull.Value ? "0" : this.Reader[70].ToString());//70 远程会诊次数

                    b.AnaphyFlag = this.Reader[71] == DBNull.Value ? string.Empty : this.Reader[71].ToString();//药物过敏

                    b.FirstAnaphyPharmacy.ID = this.Reader[72] == DBNull.Value ? string.Empty : this.Reader[72].ToString();//过敏药物名称

                    b.SecondAnaphyPharmacy.ID = this.Reader[73] == DBNull.Value ? string.Empty : this.Reader[73].ToString();//过敏药物名称

                    b.CoutDate = System.Convert.ToDateTime(this.Reader[74] == DBNull.Value ? "0001-01-01" : this.Reader[74].ToString());//更改后出院日期

                    b.PatientInfo.PVisit.AdmittingDoctor.ID = this.Reader[75] == DBNull.Value ? string.Empty : this.Reader[75].ToString();//住院医师代码

                    b.PatientInfo.PVisit.AdmittingDoctor.Name = this.Reader[76] == DBNull.Value ? string.Empty : this.Reader[76].ToString();//住院医师姓名

                    b.PatientInfo.PVisit.AttendingDoctor.ID = this.Reader[77] == DBNull.Value ? string.Empty : this.Reader[77].ToString();//主治医师代码

                    b.PatientInfo.PVisit.AttendingDoctor.Name = this.Reader[78] == DBNull.Value ? string.Empty : this.Reader[78].ToString();//主治医师姓名

                    b.PatientInfo.PVisit.ConsultingDoctor.ID = this.Reader[79] == DBNull.Value ? string.Empty : this.Reader[79].ToString();//主任医师代码

                    b.PatientInfo.PVisit.ConsultingDoctor.Name = this.Reader[80] == DBNull.Value ? string.Empty : this.Reader[80].ToString();//主任医师姓名

                    b.PatientInfo.PVisit.ReferringDoctor.ID = this.Reader[81] == DBNull.Value ? string.Empty : this.Reader[81].ToString();//科主任代码

                    b.PatientInfo.PVisit.ReferringDoctor.Name = this.Reader[82] == DBNull.Value ? string.Empty : this.Reader[82].ToString();//科主任名称

                    b.RefresherDoc.ID = this.Reader[83] == DBNull.Value ? string.Empty : this.Reader[83].ToString();//进修医师代码

                    b.RefresherDoc.Name = this.Reader[84] == DBNull.Value ? string.Empty : this.Reader[84].ToString();//进修医生名称

                    b.GraduateDoc.ID = this.Reader[85] == DBNull.Value ? string.Empty : this.Reader[85].ToString();//研究生实习医师代码

                    b.GraduateDoc.Name = this.Reader[86] == DBNull.Value ? string.Empty : this.Reader[86].ToString();//研究生实习医师名称

                    b.PatientInfo.PVisit.TempDoctor.ID = this.Reader[87] == DBNull.Value ? string.Empty : this.Reader[87].ToString();//实习医师代码

                    b.PatientInfo.PVisit.TempDoctor.Name = this.Reader[88] == DBNull.Value ? string.Empty : this.Reader[88].ToString();//实习医师名称

                    b.CodingOper.ID = this.Reader[89] == DBNull.Value ? string.Empty : this.Reader[89].ToString();//编码员代码

                    b.CodingOper.Name = this.Reader[90] == DBNull.Value ? string.Empty : this.Reader[90].ToString();//编码员名称

                    b.MrQuality = this.Reader[91] == DBNull.Value ? string.Empty : this.Reader[91].ToString();//病案质量

                    b.MrEligible = this.Reader[92] == DBNull.Value ? string.Empty : this.Reader[92].ToString();//合格病案

                    b.QcDoc.ID = this.Reader[93] == DBNull.Value ? string.Empty : this.Reader[93].ToString();//质控医师代码

                    b.QcDoc.Name = this.Reader[94] == DBNull.Value ? string.Empty : this.Reader[94].ToString();//质控医师名称

                    b.QcNurse.ID = this.Reader[95] == DBNull.Value ? string.Empty : this.Reader[95].ToString();//质控护士代码

                    b.QcNurse.Name = this.Reader[96] == DBNull.Value ? string.Empty : this.Reader[96].ToString();//质控护士名称

                    b.CheckDate = System.Convert.ToDateTime(this.Reader[97] == DBNull.Value ? "0001-01-01" : this.Reader[97].ToString());//检查时间

                    b.YnFirst = this.Reader[98] == DBNull.Value ? string.Empty : this.Reader[98].ToString();//手术操作治疗检查诊断为本院第一例项目

                    b.RhBlood = this.Reader[99] == DBNull.Value ? string.Empty : this.Reader[99].ToString();//Rh血型(阴阳)

                    b.ReactionBlood = this.Reader[100] == DBNull.Value ? string.Empty : this.Reader[100].ToString();//输血反应（有无）

                    b.BloodRed = this.Reader[101] == DBNull.Value ? "0" : this.Reader[101].ToString();//红细胞数

                    b.BloodPlatelet = this.Reader[102] == DBNull.Value ? "0" : this.Reader[102].ToString();//血小板数

                    b.BloodPlasma = this.Reader[103] == DBNull.Value ? "0" : this.Reader[103].ToString();//血浆数

                    b.BloodWhole = this.Reader[104] == DBNull.Value ? "0" : this.Reader[104].ToString();//全血数

                    b.BloodOther = this.Reader[105] == DBNull.Value ? "0" : this.Reader[105].ToString();//其他输血数

                    b.XNum = this.Reader[106] == DBNull.Value ? string.Empty : this.Reader[106].ToString();//X光号

                    b.CtNum = this.Reader[107] == DBNull.Value ? string.Empty : this.Reader[107].ToString();//CT号

                    b.MriNum = this.Reader[108] == DBNull.Value ? string.Empty : this.Reader[108].ToString();//MRI号

                    b.PathNum = this.Reader[109] == DBNull.Value ? string.Empty : this.Reader[109].ToString();//病理号

                    b.DsaNum = this.Reader[110] == DBNull.Value ? string.Empty : this.Reader[110].ToString();//DSA号

                    b.PetNum = this.Reader[111] == DBNull.Value ? string.Empty : this.Reader[111].ToString();//PET号

                    b.EctNum = this.Reader[112] == DBNull.Value ? string.Empty : this.Reader[112].ToString();//ECT号

                    b.XQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[113] == DBNull.Value ? "0" : this.Reader[113].ToString());//X线次数

                    b.CTQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[114] == DBNull.Value ? "0" : this.Reader[114].ToString());//CT次数

                    b.MRQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[115] == DBNull.Value ? "0" : this.Reader[115].ToString());//MR次数

                    b.DSAQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[116] == DBNull.Value ? "0" : this.Reader[116].ToString());//DSA次数

                    b.PetQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[117] == DBNull.Value ? "0" : this.Reader[117].ToString());//PET次数

                    b.EctQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[118] == DBNull.Value ? "0" : this.Reader[118].ToString());//ECT次数

                    b.PatientInfo.Memo = this.Reader[119] == DBNull.Value ? string.Empty : this.Reader[119].ToString();//说明

                    b.BarCode = this.Reader[120] == DBNull.Value ? string.Empty : this.Reader[120].ToString();//归档条码号

                    b.LendStat = this.Reader[121] == DBNull.Value ? string.Empty : this.Reader[121].ToString();//病案借阅状态(O借出 I在架)

                    b.PatientInfo.CaseState = this.Reader[122] == DBNull.Value ? string.Empty : this.Reader[122].ToString();//病案状态1科室质检2登记保存3整理4病案室质检5无效

                    b.OperInfo.ID = this.Reader[123] == DBNull.Value ? string.Empty : this.Reader[123].ToString();//操作员

                    b.OperInfo.OperTime = System.Convert.ToDateTime(this.Reader[124] == DBNull.Value ? "0001-01-01" : this.Reader[124].ToString());//操作时间
                    b.VisiPeriodWeek = this.Reader[125] == DBNull.Value ? string.Empty : this.Reader[125].ToString();
                    b.VisiPeriodMonth = this.Reader[126] == DBNull.Value ? string.Empty : this.Reader[126].ToString();
                    b.VisiPeriodYear = this.Reader[127] == DBNull.Value ? string.Empty : this.Reader[127].ToString();
                    b.SpecalNus = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[128] == DBNull.Value ? 0 : this.Reader[128]); 	// 特殊护理(日)                                        
                    b.INus = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[129] == DBNull.Value ? 0 : this.Reader[129]); 	//I级护理时间(日)                                     
                    b.IINus = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[130] == DBNull.Value ? 0 : this.Reader[130]);	//II级护理时间(日)                                    
                    b.IIINus = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[131] == DBNull.Value ? 0 : this.Reader[131]);	//III级护理时间(日)                                   
                    b.StrictNuss = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[132] == DBNull.Value ? 0 : this.Reader[132]);	//重症监护时间( 小时)                                 
                    b.SuperNus = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[133] == DBNull.Value ? 0 : this.Reader[133]);	//特级护理时间(小时) 
                    b.PackupMan.ID = this.Reader[134] == DBNull.Value ? string.Empty : this.Reader[134].ToString(); // 整理人
                    b.Disease30 = this.Reader[135] == DBNull.Value ? string.Empty : this.Reader[135].ToString();// 单病种 
                    b.IsHandCraft = this.Reader[136] == DBNull.Value ? string.Empty : this.Reader[136].ToString(); //手动录病案
                    b.ClinicDiag.ID = this.Reader[137] == DBNull.Value ? string.Empty : this.Reader[137].ToString(); //门诊诊断 编码
                    b.ClinicDiag.Name = this.Reader[138] == DBNull.Value ? string.Empty : this.Reader[138].ToString();//门诊诊断 名称
                    b.InHospitalDiag.ID = this.Reader[139] == DBNull.Value ? string.Empty : this.Reader[139].ToString(); //入院诊断 编码
                    b.InHospitalDiag.Name = this.Reader[140] == DBNull.Value ? string.Empty : this.Reader[140].ToString();//入院诊断 名称
                    b.OutDiag.ID = this.Reader[141] == DBNull.Value ? string.Empty : this.Reader[141].ToString();//出院主诊断 编码
                    b.OutDiag.Name = this.Reader[142] == DBNull.Value ? string.Empty : this.Reader[142].ToString();//出院主诊断 名称
                    b.OutDiag.User01 = this.Reader[143] == DBNull.Value ? string.Empty : this.Reader[143].ToString();//出院主诊断 治疗情况
                    b.OutDiag.User02 = this.Reader[144] == DBNull.Value ? string.Empty : this.Reader[144].ToString();//出院主诊断病理符合情况
                    b.FirstOperation.ID = this.Reader[145] == DBNull.Value ? string.Empty : this.Reader[145].ToString();//第一主手术代码
                    b.FirstOperation.Name = this.Reader[146] == DBNull.Value ? string.Empty : this.Reader[146].ToString();//第一主手术名称
                    b.FirstOperationDoc.ID = this.Reader[147] == DBNull.Value ? string.Empty : this.Reader[147].ToString();//第一主手术医师代码
                    b.FirstOperationDoc.Name = this.Reader[148] == DBNull.Value ? string.Empty : this.Reader[148].ToString();//第一主手术医师名称
                    b.SyndromeFlag = this.Reader[149] == DBNull.Value ? string.Empty : this.Reader[149].ToString();//是否有并发症 1 有 0 无
                    b.InfectionNum = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[150] == DBNull.Value ? "0" : this.Reader[150].ToString()); //院内感染次数
                    b.OperationCoding.ID = this.Reader[151] == DBNull.Value ? string.Empty : this.Reader[151].ToString();//手术编码员
                    b.InfectionPosition.ID = this.Reader[152] == DBNull.Value ? string.Empty : this.Reader[152].ToString();//医院感染编码
                    b.InfectionPosition.Name = this.Reader[153] == DBNull.Value ? string.Empty : this.Reader[153].ToString();//医院感染名称
                    b.PathologicalDiagCode = this.Reader[154] == DBNull.Value ? string.Empty : this.Reader[154].ToString();//病理诊断编码
                    b.PathologicalDiagName = this.Reader[155] == DBNull.Value ? string.Empty : this.Reader[155].ToString();//病理诊断名称
                    b.InjuryOrPoisoningCauseCode = this.Reader[156] == DBNull.Value ? string.Empty : this.Reader[156].ToString();//损伤中毒的外部因素编码
                    b.InjuryOrPoisoningCause = this.Reader[157] == DBNull.Value ? string.Empty : this.Reader[157].ToString();//损伤中毒的外部因素名称
                    b.CaseNO = this.Reader[158] == DBNull.Value ? string.Empty : this.Reader[158].ToString();//病案号
                    b.Out_Type = this.Reader[159] == DBNull.Value ? string.Empty : this.Reader[159].ToString();////出院方式（1、常规 2、自动 3、转院）
                    b.Cure_Type = this.Reader[160] == DBNull.Value ? string.Empty : this.Reader[160].ToString();//治疗类别（1、中      2、西      3、中西）
                    b.Use_CHA_Med = this.Reader[161] == DBNull.Value ? string.Empty : this.Reader[161].ToString();//自制中药制剂（0、未知   1、有    2、无）
                    b.Save_Type = this.Reader[162] == DBNull.Value ? string.Empty : this.Reader[162].ToString();//抢救方法（1、中     2、西       3、中西）
                    b.Ever_Sickintodeath = this.Reader[163] == DBNull.Value ? string.Empty : this.Reader[163].ToString();//是否出现危重（１、是　　　０、否）
                    b.Ever_Firstaid = this.Reader[164] == DBNull.Value ? string.Empty : this.Reader[164].ToString();//是否出现急症（１、是　　　０、否）
                    b.Ever_Difficulty = this.Reader[165] == DBNull.Value ? string.Empty : this.Reader[165].ToString();//是否出现疑难情况（１、是　０、否）
                    b.ReactionLiquid = this.Reader[166] == DBNull.Value ? string.Empty : this.Reader[166].ToString();//输液反应（１、有　２、无　３、未输）
                    b.InfectionDiseasesReport = this.Reader[167] == DBNull.Value ? string.Empty : this.Reader[167].ToString();//传染病报告
                    b.FourDiseasesReport = this.Reader[168] == DBNull.Value ? string.Empty : this.Reader[168].ToString();//四病报告
                    b.OutDept.Memo = this.Reader[169] == DBNull.Value ? string.Empty : this.Reader[169].ToString();//转往何医院

                    b.BabyAge = this.Reader[170] == DBNull.Value ? string.Empty : this.Reader[170].ToString(); //不足一周岁年龄
                    b.BabyBirthWeight = this.Reader[171] == DBNull.Value ? string.Empty : this.Reader[171].ToString();//新生儿出生体重
                    b.BabyInWeight = this.Reader[172] == DBNull.Value ? string.Empty : this.Reader[172].ToString();//新生儿入院体重
                    b.DutyNurse.ID = this.Reader[173] == DBNull.Value ? string.Empty : this.Reader[173].ToString();//责任护士
                    b.DutyNurse.Name = this.Reader[174] == DBNull.Value ? string.Empty : this.Reader[174].ToString();//责任护士姓名
                    b.HighReceiveHopital = this.Reader[175] == DBNull.Value ? string.Empty : this.Reader[175].ToString();//接收医疗机构
                    b.LowerReceiveHopital = this.Reader[176] == DBNull.Value ? string.Empty : this.Reader[176].ToString();//接收社区
                    b.ComeBackInMonth = this.Reader[177] == DBNull.Value ? string.Empty : this.Reader[177].ToString();//出院31天内再住院标志 1 无 2 有
                    b.ComeBackPurpose = this.Reader[178] == DBNull.Value ? string.Empty : this.Reader[178].ToString();//31天再住院目的
                    b.OutComeDay = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[179] == DBNull.Value ? "0" : this.Reader[179].ToString());////颅脑损伤患者昏迷时间 入院前天
                    b.OutComeHour = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[180] == DBNull.Value ? "0" : this.Reader[180].ToString());//颅脑损伤患者昏迷时间 入院前小时
                    b.OutComeMin = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[181] == DBNull.Value ? "0" : this.Reader[181].ToString());//颅脑损伤患者昏迷时间 入院前分钟
                    b.InComeDay = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[182] == DBNull.Value ? "0" : this.Reader[182].ToString());//颅脑损伤患者昏迷时间 入院后天
                    b.InComeHour = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[183] == DBNull.Value ? "0" : this.Reader[183].ToString());//颅脑损伤患者昏迷时间 入院后小时
                    b.InComeMin = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[184] == DBNull.Value ? "0" : this.Reader[184].ToString());//颅脑损伤患者昏迷时间 入院后分钟
                    b.Dept_Change = this.Reader[185] == DBNull.Value ? string.Empty : this.Reader[185].ToString();//转科科别
                    b.PatientInfo.Kin.Memo = this.Reader[186] == DBNull.Value ? string.Empty : this.Reader[186].ToString();//与关系备注
                    b.CurrentAddr = this.Reader[187] == DBNull.Value ? string.Empty : this.Reader[187].ToString();//现住址
                    b.CurrentPhone = this.Reader[188] == DBNull.Value ? string.Empty : this.Reader[188].ToString(); ;//现住址电话
                    b.CurrentZip = this.Reader[189] == DBNull.Value ? string.Empty : this.Reader[189].ToString(); ;//现住址邮编
                    b.InRoom = this.Reader[190] == DBNull.Value ? string.Empty : this.Reader[190].ToString(); ;//入院病房
                    b.OutRoom = this.Reader[191] == DBNull.Value ? string.Empty : this.Reader[191].ToString(); ;//出院病房
                    b.InPath = this.Reader[192] == DBNull.Value ? string.Empty : this.Reader[192].ToString(); ;//入院途径 1急诊 2门诊 3其他医疗机构转入 9其他
                    b.ExampleType = this.Reader[193] == DBNull.Value ? string.Empty : this.Reader[193].ToString(); ;//病例分型 A一般 B急 C疑难 D危重
                    b.ClinicPath = this.Reader[194] == DBNull.Value ? string.Empty : this.Reader[194].ToString(); ;//入院途径 1是 2否
                    b.UploadStatu = this.Reader[195] == DBNull.Value ? string.Empty : this.Reader[195].ToString(); ;//上传标志 1是 2否
                    b.IsDrgs = this.Reader[196] == DBNull.Value ? string.Empty : this.Reader[196].ToString(); ;//新病案首页DRGS 1是 2否
                    // {1F545AEA-5D57-40db-A2F0-3232A7E856D6}病案室保存后不允许医生修改诊断
                    b.Diag_Flag = this.Reader[197] == DBNull.Value ? string.Empty : this.Reader[197].ToString(); ;//新病案首页DRGS 1是 2否
                    b.SaveOperation.ID = this.Reader[198] == DBNull.Value ? string.Empty : this.Reader[198].ToString();//责任护士
                    b.LastSaveTime = System.Convert.ToDateTime(this.Reader[199] == DBNull.Value ? "0001-01-01" : this.Reader[199].ToString());//出生日期
                    list.Add(b);
                }
                return list;
            }
            catch (Exception ex)
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                this.Err = "获得患者病案信息出错!" + ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }
        /// <summary>
        /// 得到未等登记病案信息的患者的诊断信息
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private ArrayList myGetDiagInfo(string strSql)
        {
            ArrayList al = new ArrayList();
            Neusoft.HISFC.Models.HealthRecord.DiagnoseBase dg;
            this.ExecQuery(strSql);

            try
            {
                while (this.Reader.Read())
                {
                    dg = new Neusoft.HISFC.Models.HealthRecord.DiagnoseBase();

                    dg.ID = this.Reader[0] == DBNull.Value ? string.Empty : Reader[0].ToString();//住院流水号
                    dg.Patient.ID = this.Reader[0] == DBNull.Value ? string.Empty : Reader[0].ToString();//住院流水号
                    dg.HappenNo = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[1] == DBNull.Value ? 0 : Reader[1]);//发生序号
                    dg.Patient.PID.CardNO = this.Reader[2] == DBNull.Value ? string.Empty : this.Reader[2].ToString();//就诊卡号
                    dg.DiagType.ID = this.Reader[3] == DBNull.Value ? string.Empty : this.Reader[3].ToString();//诊断类别
                    dg.ICD10.ID = this.Reader[4] == DBNull.Value ? string.Empty : this.Reader[4].ToString();//诊断代码
                    dg.ICD10.Name = this.Reader[5] == DBNull.Value ? string.Empty : this.Reader[5].ToString();//诊断名称
                    dg.DiagDate = System.Convert.ToDateTime(this.Reader[6] == DBNull.Value ? "0001-01-01" : this.Reader[6].ToString());//诊断日期
                    dg.Doctor.ID = this.Reader[7] == DBNull.Value ? string.Empty : this.Reader[7].ToString();//诊断医生代码
                    dg.Doctor.Name = this.Reader[8] == DBNull.Value ? string.Empty : this.Reader[8].ToString();//诊断医师名称
                    dg.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[9] == DBNull.Value ? "0" : this.Reader[9].ToString());//是否有效0有效1无效
                    dg.Dept.ID = this.Reader[10] == DBNull.Value ? string.Empty : this.Reader[10].ToString();//科室
                    dg.IsMain = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[11] == DBNull.Value ? "0" : this.Reader[11].ToString());//是否主诊断
                    dg.Memo = this.Reader[12] == DBNull.Value ? string.Empty : this.Reader[12].ToString();//备注
                    al.Add(dg);
                }
                this.Reader.Close();
            }
            catch (Exception ex)
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                this.Err = "获得患者住院诊断信息出错!" + ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            return al;
        }
        /// <summary>
        /// 计算两个DateTime时间差
        /// </summary>
        /// <param name="flag">"YYYY"年龄|"MM"|月|"DD"天</param>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>double</returns>
        private double DateDiff(string flag, DateTime dateBegin, DateTime dateEnd)
        {
            double diff = 0;
            try
            {
                TimeSpan TS = new TimeSpan(dateEnd.Ticks - dateBegin.Ticks);

                switch (flag.ToLower())
                {
                    case "m":
                        diff = Convert.ToDouble(TS.TotalMinutes);
                        break;
                    case "s":
                        diff = Convert.ToDouble(TS.TotalSeconds);
                        break;
                    case "t":
                        diff = Convert.ToDouble(TS.Ticks);
                        break;
                    case "mm":
                        diff = Convert.ToDouble(TS.TotalMilliseconds);
                        break;
                    case "yyyy":
                        diff = Convert.ToDouble(TS.TotalDays / 365);
                        break;
                    case "q":
                        diff = Convert.ToDouble((TS.TotalDays / 365) / 4);
                        break;
                    case "dd":
                        diff = Convert.ToDouble((TS.TotalDays));
                        break;
                    default:
                        diff = Convert.ToDouble(TS.TotalDays);
                        break;
                }
            }
            catch
            {

                diff = -1;
            }

            return diff;
        }
        #endregion

        /// <summary>
        /// 根据生日和当前时间得出患者得年龄和年龄单位
        /// ID 保存年龄 Name保存年龄单位
        /// </summary>
        /// <param name="bornDate">患者得出生日期</param>
        /// <returns>Neusoft.FrameWork.Models.NeuObject</returns>
        public new Neusoft.FrameWork.Models.NeuObject GetAge(DateTime bornDate)
        {
            DateTime nowDate;
            double temp;

            Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();

            nowDate = this.GetDateTimeFromSysDateTime();


            temp = DateDiff("YYYY", bornDate, nowDate);
            obj.Name = "Y";

            if (temp < 0) //小于一年
            {
                temp = DateDiff("DD", bornDate, nowDate);

                if (temp < 28)
                {
                    obj.Name = "D";
                }
                else
                {
                    obj.Name = "M";
                }
            }

            obj.ID = temp.ToString();

            return obj;
        }
        /// <summary>
        /// 通过函数获取年龄
        /// </summary>
        /// <param name="dtBirth"></param>
        /// <param name="dtInDate"></param>
        /// <returns></returns>
        public string GetAgeByFun(DateTime dtBirth, DateTime dtInDate)
        {
            string strSql = "";
            string strAge = "";
            if (this.Sql.GetSql("Case.BaseDML.GetAge.ByFunGetAgeNew", ref strSql) == -1) return null;
            try
            {
                strSql = string.Format(strSql, dtBirth, dtInDate);
                //查询
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    strAge = Reader[0].ToString(); //年龄
                }
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            finally
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            return strAge;
        }
        /// <summary>
        /// 获取入院登记入院科室 
        /// </summary>
        /// <returns></returns>
        public Neusoft.HISFC.Models.RADT.Location GetDeptIn(string inpatienNo)
        {
            string strSql = "";
            Neusoft.HISFC.Models.RADT.Location info = null;
            if (this.Sql.GetSql("Case.BaseDML.GetDeptIn.1", ref strSql) == -1) return null;
            try
            {
                strSql = string.Format(strSql, inpatienNo);
                //查询
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    info = new Neusoft.HISFC.Models.RADT.Location();
                    info.Dept.ID = Reader[0].ToString(); //科室编码
                    info.Dept.Name = Reader[1].ToString();//科室名称
                }
                this.Reader.Close();
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                info = null;
            }
            return info;
        }
        /// <summary>
        /// 获取接诊入院科室 
        /// </summary>
        /// <returns></returns>
        public Neusoft.HISFC.Models.RADT.Location GetDeptIn1(string inpatienNo)
        {
            string strSql = "";
            Neusoft.HISFC.Models.RADT.Location info = null;
            if (this.Sql.GetSql("Case.BaseDML.GetDeptIn.11", ref strSql) == -1) return null;
            try
            {
                strSql = string.Format(strSql, inpatienNo);
                //查询
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    info = new Neusoft.HISFC.Models.RADT.Location();
                    info.Dept.ID = Reader[0].ToString(); //科室编码
                    info.Dept.Name = Reader[1].ToString();//科室名称
                }
                this.Reader.Close();
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                info = null;
            }
            return info;
        }
        /// <summary>
        /// 获取出院科室
        /// </summary>
        /// <returns></returns>
        public Neusoft.HISFC.Models.RADT.Location GetDeptOut(string inpatienNo)
        {
            string strSql = "";
            Neusoft.HISFC.Models.RADT.Location info = null;
            if (this.Sql.GetSql("Case.BaseDML.GetDeptOut", ref strSql) == -1) return null;
            try
            {
                strSql = string.Format(strSql, inpatienNo);
                //查询
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    info = new Neusoft.HISFC.Models.RADT.Location();
                    info.Dept.ID = Reader[0].ToString(); //科室编码
                    info.Dept.Name = Reader[1].ToString();//科室名称
                }
                this.Reader.Close();
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                info = null;
            }
            return info;
        }
        /// <summary>
        /// 护理数量统计
        /// </summary>
        /// <param name="inpatienId">住院流水号</param>
        /// <param name="type">类型（首拼码）</param>
        /// <returns></returns>
        public int GetNursingNum(string inpatienId, string type)
        {
            string strSql = "";
            int iReturn = 0;

            this.Sql.GetSql("CASE.BaseDML.GetNursingNum", ref strSql);

            try
            {
                strSql = string.Format(strSql, inpatienId, type);
                //查询
                if (this.ExecQuery(strSql) < 0)
                {
                    this.Err = "执行sql失败!";
                    return -1;
                }
                this.Reader.Read();
                iReturn = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[0].ToString());
                this.Reader.Close();

            }
            catch (Exception ex)
            {
                this.Err = "" + ex.Message;
                this.Reader.Close();
                return -1;
            }
            return iReturn;
        }
        /// <summary>
        /// 获取出院志出院诊断
        /// </summary>
        /// <param name="InPatientNO"></param>
        /// <returns></returns>
        public ArrayList QueryCaseDiagnoseByInpatientNo(string InPatientNO)
        {
            string strSql = string.Empty;
            ArrayList al = new ArrayList();
            Neusoft.HISFC.Models.HealthRecord.Diagnose diagnoseObj;
            if (Sql.GetSql("Neusoft.HISFC.Management.HealthRecord.QueryHealthRecordCustomDiagnose", ref strSql) == -1)
            {
                Err = "获取[Neusoft.HISFC.Management.HealthRecord.QueryHealthRecordCustomDiagnose]ＳＱＬ语句出错";
                return null;
            }
            try
            {
                strSql = string.Format(strSql, InPatientNO);
                this.ExecQuery(strSql);

                while (Reader.Read())
                {
                    diagnoseObj = new Neusoft.HISFC.Models.HealthRecord.Diagnose();
                    diagnoseObj.ID = this.Reader["INPATIENT_NO"].ToString();
                    diagnoseObj.DiagInfo.ICD10.ID = this.Reader["ICD_CODE"].ToString();
                    diagnoseObj.DiagInfo.Name = this.Reader["DIAG_NAME"].ToString();
                    diagnoseObj.DiagInfo.Doctor.ID = this.Reader["DOCT_CODE"].ToString();
                    diagnoseObj.DiagInfo.Doctor.Name = this.Reader["DOCT_NAME"].ToString();
                    diagnoseObj.MainFlag = this.Reader["MAIN_FLAG"].ToString();
                    diagnoseObj.DiagInfo.DiagType.ID = this.Reader["DIAG_KIND"].ToString();
                    diagnoseObj.DiagOutState = this.Reader["DIAG_OUTSTATE"].ToString();
                    diagnoseObj.Memo = this.Reader["DIAG_OUTSTATE_NAME"].ToString();
                    diagnoseObj.DiagInfo.ICDF10.ID = this.Reader["SUBSIDIARY_ICDCODE"].ToString();
                    diagnoseObj.DiagInfo.ICDF10.Name = this.Reader["SUBSIDIARY_ICDNAME"].ToString();
                    diagnoseObj.DiagInfo.ICD10.Name = this.Reader["ICDNAME"].ToString();
                    al.Add(diagnoseObj);
                }

            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return null;
            }
            finally
            {
                Reader.Close();
            }
            return al;


        }

        // {ECED358D-1C17-4fcd-B7BC-B2E19DD0E5CA} 上传到省病案系统按“入院病情1”来上传
        /// <summary>
        /// 获取出院志出院诊断
        /// </summary>
        /// <param name="InPatientNO"></param>
        /// <returns></returns>
        public ArrayList QueryCaseDiagnoseByInpatientNo1(string InPatientNO)
        {
            string strSql = string.Empty;
            ArrayList al = new ArrayList();
            Neusoft.HISFC.Models.HealthRecord.Diagnose diagnoseObj;
            if (Sql.GetSql("Neusoft.HISFC.Management.HealthRecord.QueryHealthRecordCustomDiagnose1", ref strSql) == -1)
            {
                Err = "获取[Neusoft.HISFC.Management.HealthRecord.QueryHealthRecordCustomDiagnose1]ＳＱＬ语句出错";
                return null;
            }
            try
            {
                strSql = string.Format(strSql, InPatientNO);
                this.ExecQuery(strSql);

                while (Reader.Read())
                {
                    diagnoseObj = new Neusoft.HISFC.Models.HealthRecord.Diagnose();
                    diagnoseObj.ID = this.Reader["INPATIENT_NO"].ToString();
                    diagnoseObj.DiagInfo.ICD10.ID = this.Reader["ICD_CODE"].ToString();
                    diagnoseObj.DiagInfo.Name = this.Reader["DIAG_NAME"].ToString();
                    diagnoseObj.DiagInfo.Doctor.ID = this.Reader["DOCT_CODE"].ToString();
                    diagnoseObj.DiagInfo.Doctor.Name = this.Reader["DOCT_NAME"].ToString();
                    diagnoseObj.MainFlag = this.Reader["MAIN_FLAG"].ToString();
                    diagnoseObj.DiagInfo.DiagType.ID = this.Reader["DIAG_KIND"].ToString();
                    diagnoseObj.DiagOutState = this.Reader["DIAG_OUTSTATE"].ToString();
                    diagnoseObj.Memo = this.Reader["DIAG_OUTSTATE_NAME"].ToString();
                    diagnoseObj.DiagInfo.ICDF10.ID = this.Reader["SUBSIDIARY_ICDCODE"].ToString();
                    diagnoseObj.DiagInfo.ICDF10.Name = this.Reader["SUBSIDIARY_ICDNAME"].ToString();
                    diagnoseObj.DiagInfo.ICD10.Name = this.Reader["ICDNAME"].ToString();
                    al.Add(diagnoseObj);
                }

            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return null;
            }
            finally
            {
                Reader.Close();
            }
            return al;


        }


        /// <summary>
        /// 护理数量统计
        /// </summary>
        /// <param name="inpatienId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int StateHl(string inpatienId, string type)
        {
            string strSql = "";
            int iReturn = 0;
            if (type == "T")
            {
                this.Sql.GetSql("Case.State.GetTjhl", ref strSql);
            }
            else if (type == "Y")
            {
                this.Sql.GetSql("Case.State.GetYjhl", ref strSql);
            }
            else if (type == "E")
            {
                this.Sql.GetSql("Case.State.GetEjhl", ref strSql);
            }
            else if (type == "S")
            {
                this.Sql.GetSql("Case.State.GetSjhl", ref strSql);
            }
            else if (type == "TS")
            {
                this.Sql.GetSql("Case.State.GetTShl", ref strSql);
            }
            else if (type == "ZZ")
            {
                this.Sql.GetSql("Case.State.GetZZJH", ref strSql);
            }
            try
            {
                strSql = string.Format(strSql, inpatienId);
                //查询
                if (this.ExecQuery(strSql) < 0)
                {
                    this.Err = "执行sql失败!";
                    return -1;
                }
                this.Reader.Read();
                iReturn = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[0].ToString());
                this.Reader.Close();

            }
            catch (Exception ex)
            {
                this.Err = "" + ex.Message;
                this.Reader.Close();
                return -1;
            }
            return iReturn;
        }
        /// <summary>
        /// 获取医院名称
        /// </summary>
        /// <returns></returns>
        public string GetHosName()
        {
            string hosName = string.Empty;
            string strSql = @"select hos_name from com_hospitalinfo where rownum =1 ";
            this.ExecQuery(strSql);

            while (this.Reader.Read())
            {
                hosName = string.Empty;
                hosName = this.Reader[0].ToString(); //医院名称
            }
            return hosName;
        }
        /// <summary>
        /// 更新主表
        /// </summary>
        /// <param name="PatientInfo"></param>
        /// <returns></returns>
        public int UpdatePatient(Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo)
        {
            #region "接口说明"

            //			/接口名称 RADT.InPatient.UpdatePatient.1
            //			/max 66
            //					<!-- 0  --住院流水号,1 --姓名 2   --住院号   ,3   --就诊卡号  ,4   --医疗证号
            //			    ,5     --医疗类别,   ,6   --性别   ,7   --身份证号  ,8   --拼音     ,9   --生日
            //			    ,10   --职业代码     ,11   --工作单位    ,12   --工作单位电话      ,13   --单位邮编
            //			    ,14   --户口或家庭地址     ,15   --家庭电话   ,16   --户口或家庭邮编   , 17  --籍贯name
            //			    ,18   --出生地代码        ,19   --民族id    ,20  --民族name    ,21   --联系人姓名
            //			    ,22   --联系人电话       ,23   --联系人地址     ,24   --联系人关系id , 25   --联系人关系id 
            //			    ,26   --婚姻状况id              ,27  --婚姻状况name  ,28   --国籍id     29 --国籍名称
            //			    ,30   --身高           ,31   --体重         ,32   -- 职位id    ,33   --ABO血型
            //			    ,34   --重大疾病标志    ,35   --过敏标志            
            //			    ,36   --入院日期      ,37   --科室代码   , 38  --科室名称  , 39  --结算类别id 1-自费  2-保险 3-公费在职 4-公费退休 5-公费高干
            //			    ,40   --结算类别名称   , 41  --合同代码   , 42  --合同单位名称  , 43  --床号
            //			   , 44 --护理单元代码  , 45  --护理单元名称, 46 --医师代码(住院), 47 --医师姓名(住院)
            //			   , 48 --医师代码(主治) , 49 --医师姓名(主治) , 50 --医师代码(主任) , 51 --医师姓名(主任)
            //			   , 52 --医师代码(实习) , 53 --医师姓名(实习), 54  --护士代码(责任), 55  --护士姓名(责任)
            //			   , 56  --入院情况id  , 57  --入院情况name   , 58  --入院途径id    , 59  --入院途径name      
            //			   , 60  --入院来源id 1 -门诊 2 -急诊 3 -转科 4 -转院    , 61  --入院来源name
            //			   , 62  --住院登记  i-病房接诊 -出院登记 o-出院结算 p-预约出院 n-无费退院
            //			  ,  63  --出院日期(预约)  , 64  --出院日期 , 65  --是否在ICU 0 no 1 yes ,66 操作员 -->

            #endregion

            string strSql = string.Empty;
            if (Sql.GetSql("Case.BaseDML.UpdatePatient.1", ref strSql) == -1)
            {
                return -1;
            }

            try
            {
                string[] s = new string[22];
                try
                {
                    s[0] = PatientInfo.ID; // --住院流水号
                    s[1] = PatientInfo.Name; //--姓名
                    s[2] = PatientInfo.Sex.ID.ToString(); //  --性别
                    s[3] = PatientInfo.IDCard; //  --身份证号
                    s[4] = PatientInfo.Birthday.ToString(); //  --生日
                    s[5] = PatientInfo.Profession.ID; //  --职业 
                    s[6] = PatientInfo.CompanyName; //  --工作单位
                    s[7] = PatientInfo.PhoneBusiness; //  --工作单位电话
                    s[8] = PatientInfo.BusinessZip; //  --单位邮编
                    s[9] = PatientInfo.AddressHome; //  --户口或家庭地址
                    s[10] = PatientInfo.PhoneHome; //  --家庭电话
                    s[11] = PatientInfo.HomeZip; //  --户口或家庭邮编
                    s[12] = PatientInfo.DIST; // --籍贯name
                    s[13] = PatientInfo.Nationality.ID; //  --民族id
                    s[14] = PatientInfo.Kin.Name; //  --联系人姓名
                    s[15] = PatientInfo.Kin.RelationPhone; //  --联系人电话
                    s[16] = PatientInfo.Kin.RelationAddress; //  --联系人地址
                    s[17] = PatientInfo.Kin.Relation.ID; //  --联系人关系id
                    s[18] = PatientInfo.MaritalStatus.ID.ToString(); //  --婚姻状况id
                    s[19] = PatientInfo.Country.ID; //  --国籍id
                    s[20] = PatientInfo.BloodType.ID.ToString(); //  --ABO血型
                    s[21] = Neusoft.FrameWork.Function.NConvert.ToInt32(PatientInfo.Disease.IsAlleray).ToString(); //  --过敏标志
                    strSql = string.Format(strSql, s);
                }
                catch (Exception ex)
                {
                    Err = ex.Message;
                    WriteErr();
                }
            }
            catch (Exception ex)
            {
                Err = "赋值时候出错！" + ex.Message;
                WriteErr();
                return -1;
            }

            int parm = ExecNoQuery(strSql);
            if (parm != 1)
            {
                return parm;
            }
            return 1;
        }
        /// <summary>
        /// 更新基本信息表－不是患者主表  表名：com_patientinfo
        /// </summary>
        /// <param name="PatientInfo"></param>
        /// <returns></returns>
        public int UpdatePatientInfo(Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo)
        {
            string strSql = string.Empty;
            if (Sql.GetSql("Case.BaseDML.UpdatePatientInfo.1", ref strSql) == -1)
            {
                return -1;
            }

            try
            {
                string[] s = new string[22];
                try
                {
                    s[0] = PatientInfo.PID.CardNO; //就诊卡号
                    s[1] = PatientInfo.Name; //姓名
                    s[2] = PatientInfo.Sex.ID.ToString(); //性别
                    s[3] = PatientInfo.IDCard; //  --身份证号
                    s[4] = PatientInfo.Birthday.ToString(); //  --生日
                    s[5] = PatientInfo.Profession.ID; //  --职业 
                    s[6] = PatientInfo.CompanyName; //  --工作单位
                    s[7] = PatientInfo.PhoneBusiness; //  --工作单位电话
                    s[8] = PatientInfo.BusinessZip; //  --单位邮编
                    s[9] = PatientInfo.AddressHome; //  --户口或家庭地址
                    s[10] = PatientInfo.PhoneHome; //  --家庭电话
                    s[11] = PatientInfo.HomeZip; //  --户口或家庭邮编
                    s[12] = PatientInfo.DIST; // --籍贯name
                    s[13] = PatientInfo.Nationality.ID; //  --民族id
                    s[14] = PatientInfo.Kin.Name; //  --联系人姓名
                    s[15] = PatientInfo.Kin.RelationPhone; //  --联系人电话
                    s[16] = PatientInfo.Kin.RelationAddress; //  --联系人地址
                    s[17] = PatientInfo.Kin.Relation.ID; //  --联系人关系id
                    s[18] = PatientInfo.MaritalStatus.ID.ToString(); //  --婚姻状况id
                    s[19] = PatientInfo.Country.ID; //  --国籍id
                    s[20] = PatientInfo.BloodType.ID.ToString(); //  --ABO血型
                    s[21] = Neusoft.FrameWork.Function.NConvert.ToInt32(PatientInfo.Disease.IsAlleray).ToString(); //  --过敏标志
                }
                catch (Exception ex)
                {
                    Err = ex.Message;
                    WriteErr();
                    return -1;
                }
                return ExecNoQuery(strSql, s);
            }
            catch (Exception ex)
            {
                Err = "赋值时候出错！" + ex.Message;
                WriteErr();
                return -1;
            }


        }
        /// <summary>
        /// 获取病案提交信息
        /// 控制编辑状态 提交后不允许编辑
        /// </summary>
        /// <param name="InPatientNO">住院流水号</param>
        /// <returns></returns>
        public int GetEmrQcCommit(string inpatientNO)
        {
            int iReturn = 0;
            string strSql = @"select count(1) from emr_qc_commit  q
where q.inpatient_no='{0}' and q.status in ('1','2','3')  ";
            try
            {
                strSql = string.Format(strSql, inpatientNO);
                //查询
                if (this.ExecQuery(strSql) < 0)
                {
                    this.Err = "执行sql失败!";
                    return -1;
                }
                this.Reader.Read();
                iReturn = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[0].ToString());
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            catch (Exception ex)
            {
                this.Err = "" + ex.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                return -1;
            }
            return iReturn;
        }

        /// <summary>
        /// 取得年龄
        /// chengym 2012-11-19 总部最新电子病历的年龄算法
        /// </summary>
        /// <param name="birthday">出生日期</param>
        /// <param name="endDate">截至日期</param>
        /// <param name="age">年龄(不包含title)</param>
        /// <returns></returns>
        public string EmrGetAge(DateTime birthday, DateTime endDate, ref int age)
        {
            string result = "";

            int diffYear = endDate.Year - birthday.Year;
            int diffMonth = endDate.Month - birthday.Month;
            int diffDay = endDate.Day - birthday.Day;

            //未过生日的时候，年龄减1岁
            if (diffYear > 0)
            {
                if (diffMonth < 0)
                {
                    diffYear = diffYear - 1;
                    diffMonth = diffMonth + 12;
                }
                else if (diffMonth == 0 && diffDay < 0)
                {
                    diffYear = diffYear - 1;
                    diffMonth = 11;
                }
            }

            //一岁以下
            if (diffYear == 0)
            {
                TimeSpan ts = endDate - birthday;
                int diffDayReal = ts.Days;
                //这个算法有问题  导致大于一个月的没有按月算 如5月28日出生 当前时间7月4日变成了 显示 N周N天了 改为间隔30天的按月算 应该可以接受

                //一岁以下,且一个月及以上， 2012-6-2 郑勋modify,天数要大于0,解决5月28日出生，当前日期为6月2日，显示为1个月的Bug
                if (diffMonth >= 1)//&& diffDay >= 0) //diffDayReal>=30
                {
                    result = diffMonth.ToString() + " 月";
                }
                //不到一月，一周以上，要几周零几天
                //2012-6-2 郑勋 diffDay不能参与计算，因为他不是2个日期相差的天数
                //else if (diffDayReal > 7)//(diffDay > 7)
                //{
                //    //result = (diffDay / 7).ToString() + " 周" + (diffDay % 7).ToString() + " 天";
                //    result = (diffDayReal / 7).ToString() + " 周" + (diffDayReal % 7).ToString() + " 天";
                //}
                else
                {
                    //result = diffDay.ToString() + " 天";
                    if (diffDayReal == 0)
                    {
                        result = "1 天";
                    }
                    else
                    {
                        result = diffDayReal.ToString() + " 天";
                    }
                }
            }
            ////三岁以下，一岁以上要精确到几岁零几个月
            //else if (diffYear > 0 && diffYear < 3)
            //{
            //    //result = diffYear.ToString() + " 岁" + diffMonth.ToString() + " 月";
            //    result = diffYear * 3 + diffMonth + " 月";
            //}
            //三岁以上
            else
            {
                result = diffYear.ToString() + " 岁";
            }
            age = diffYear;
            return result;
        }

        /// <summary>
        /// 初步判断病例类型
        /// 2016-10-21 chengym
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns></returns>
        public string QueryPatientExampleType(string inpatientNO)
        {
            string strSql = @"select case when ageType='D' or illType='D' or inICU='D' or ops='D' then 'D' --其中一项是D取最高级别D
       when ageType='F' and illType='F' and inICU='F' and ops='F' then ''   --所有项是F 没有符合条件取空值
       when ageType='C' or illType='C' or inICU='C' or ops='C' then 'C' --其中一项是C取最高级别C
         else 'B' end ExampleType
         from 
         (
select  i.inpatient_no,case when  round(i.in_date-i.birthday)<29 then 'C' 
 when  round((i.in_date-i.birthday)/365)>70  then 'C' else 'F' end ageType  --1小于等于28天 大于70岁  C
 ,(select case when count(*)>0 then 'D' else 'F' end   from met_imp_dayreport d where 
 d.inpatient_no=i.inpatient_no and (d.item_name='病重' or d.item_name='病危')) illType , ---有病危病重医嘱 D
(select case when count(*)>0 then 'D' else 'F' end  from met_ipm_order o 
where o.inpatient_no=i.inpatient_no and o.dept_code='5002' and o.mo_stat in ('0','1','2') )
 inICU, --5002 重症医学科住院 D
 (select case when (l.ops_kind='2' or l.ops_kind='3') and (l.degree!='4' or l.degree is null)  then 'C'
  when l.degree='3'  then 'C'  when l.degree='4' then 'D' end  ops-- l.ops_kind 1普通 2急诊 3感染  l.degree , --1 一级 2 二级 3三级 4 四级
    from met_ops_apply l where l.clinic_code=i.inpatient_no and l.ynvalid='1' and l.execstatus='4')  ops
   
 from fin_ipr_inmaininfo i where  i.inpatient_no='{0}' )   --98551 C   173322 D  116727 D";

            string iReturn = string.Empty;
            try
            {
                strSql = string.Format(strSql, inpatientNO);
                //查询
                if (this.ExecQuery(strSql) < 0)
                {
                    this.Err = "执行sql失败!";
                    return "";
                }
                this.Reader.Read();
                iReturn = Reader[0].ToString();
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            catch (Exception ex)
            {
                this.Err = "" + ex.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                return "";
            }
            return iReturn;
        }
        #region 广东省病案上传
        /// <summary>
        /// 从病案基本表中获取信息 --病案上传用2010-4-29
        /// </summary>
        /// <param name="dtBegin">开始时间</param>
        /// <param name="dtEnd">结束时间</param>
        /// <returns></returns>
        public ArrayList GetCaseBaseInfo(DateTime dtBegin, DateTime dtEnd)
        {
            Neusoft.HISFC.Models.HealthRecord.Base info = new Neusoft.HISFC.Models.HealthRecord.Base();
            //获取主sql语句
            string strSQL = GetCaseSql();
            if (strSQL == null)
            {
                return null;
            }
            string str = @"    where out_date>=to_date('{0}','yyyy-mm-dd HH24:mi:ss')  and out_date<=to_date('{1}','yyyy-mm-dd HH24:mi:ss') ";

            strSQL += str;
            strSQL = string.Format(strSQL, dtBegin, dtEnd);
            return this.myGetCaseBaseInfo(strSQL);
        }

        /// <summary>
        /// 更新病案上传标志　顺德妇幼病案接口有使用
        /// </summary>
        /// <param name="patientNo">住院流水号</param>
        /// <returns></returns>
        public int UpdateBaseUploadFlat(string patientNo)
        {
            string strSql = "update  met_cas_base  set x_numb='2'  where inpatient_no='{0}'";

            try
            {
                strSql = string.Format(strSql, patientNo);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return -1;
            }

            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 电子病历归档操作
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="caseUpdateStus">空</param>
        /// <param name="sendStus">病案状态</param>
        /// <param name="sendFlow">借阅状态</param>
        /// <returns></returns>
        public int UpdateBaseCaseStus(string inpatientNO, string caseUpdateStus, string sendStus, string sendFlow)
        {
            string strSQL = "";

            if (Sql.GetSql("CASE.BaseDML.UpdateCaseStus.Update", ref strSQL) == 0)
            {
                try
                {
                    strSQL = string.Format(strSQL, inpatientNO, caseUpdateStus, sendStus, sendFlow);
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
        /// 病案诊断锁定状态更新操作
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns></returns>
        public int UpdateBaseCaseDiagStus(string inpatientNO)
        {
            string strSql = "";

            //病案室解锁后值为2
            strSql = @"UPDATE met_cas_base t
                             SET t.diag_flag = '2'  
                             WHERE t.INPATIENT_NO = '{0}'";

            strSql = string.Format(strSql, inpatientNO);
            return this.ExecNoQuery(strSql);
        }


        /// <summary>
        /// 查询院感次数
        /// </summary>
        /// <param name="inpatien_NO">住院流水号</param>
        /// <returns></returns>
        public int QueryInfCount(string inpatien_NO)
        {
            string strSql = "";
            int iReturn = 0;
            if (this.Sql.GetSql("CASE.BaseDML.GetInfectionReport", ref strSql) == -1)
            {
                this.Err = "获取CASE.BaseDML.GetInfectionReport出错!";
                return -1;
            }

            try
            {
                strSql = string.Format(strSql, inpatien_NO);
                //查询
                if (this.ExecQuery(strSql) < 0)
                {
                    this.Err = "执行sql失败!";
                    return -1;
                }
                this.Reader.Read();
                iReturn = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[0].ToString());
                this.Reader.Close();

            }
            catch (Exception ex)
            {
                this.Err = "" + ex.Message;
                this.Reader.Close();
                return -1;
            }
            return iReturn;
        }

        /// <summary>
        /// 上传时更新主表次数时同时更新病案主表
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="inTimes"></param>
        /// <returns></returns>
        public int UpdateMetCasBaseTimes(string inpatientNo, int inTimes)
        {
            //获取Sql语句
            string strSql = @"UPDATE MET_CAS_BASE SET IN_TIMES = {1} WHERE INPATIENT_NO = '{0}'";
            try
            {
                #region 格式化SQL
                strSql = string.Format(strSql, inpatientNo, inTimes.ToString());
                #endregion

            }
            catch (Exception ex)
            {

                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 从病案基本表中获取信息
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.Base GetCaseBaseInfoFromEmrView(string inpatientNO)
        {
            Neusoft.HISFC.Models.HealthRecord.Base info = new Neusoft.HISFC.Models.HealthRecord.Base();
            //获取主sql语句
            string strSQL = GetCaseSql();
            strSQL.Replace("met_cas_base", "view_met_cas_base");
            if (strSQL == null)
            {
                return null;
            }
            string str = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfo.Select.where", ref str) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSQL += str;
            strSQL = string.Format(strSQL, inpatientNO);
            ArrayList arrList = this.myGetCaseBaseInfo(strSQL);
            if (arrList == null)
            {
                return null;
            }
            if (arrList.Count > 0)
            {
                info = (Neusoft.HISFC.Models.HealthRecord.Base)arrList[0];
            }
            return info;
        }
        /// <summary>
        /// 查询住院主表病案上传状态 casesend_flag
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns></returns>
        public string GetCaseUploadState(string inpatientNO)
        {
            string UploadState = string.Empty;
            string strSql = @"select t.casesend_flag from fin_ipr_inmaininfo t where t.inpatient_no='{0}'";
            try
            {
                strSql = string.Format(strSql, inpatientNO);
            }
            catch
            {
            }
            this.ExecQuery(strSql);

            while (this.Reader.Read())
            {
                UploadState = string.Empty;
                UploadState = this.Reader[0].ToString(); //病案上传状态
            }
            return UploadState;
        }

        /// <summary>
        /// 查询住院主表ICD上传状态 ext_flag2
        /// 2016-12-22 修改成met_cas_diagnose.is_zhuhaii_cduploap 
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns></returns>
        public Neusoft.FrameWork.Models.NeuObject GetICDUploadState(string inpatientNO)
        {
            Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
            //            string strSql = @"select t.ext_flag2,
            //                               me.icd_code,
            //                               me.diag_name
            //                        from fin_ipr_inmaininfo t,met_cas_diagnose me 
            //                        where t.inpatient_no=me.inpatient_no
            //                        and  me.persson_type='1'
            //                        and me.main_flag='1'
            //                        and me.valid_flag='1'
            //                        and rownum='1'
            //                        and t.inpatient_no='{0}'";
            string strSql = @"select me.is_zhuhaii_cduploap,
                               me.icd_code,
                               me.diag_name,
                               (select x.errormess from CASEHISTORYUPLOAD x where x.inpatient_no = me.inpatient_no and x.status ='0') errormess
                        from fin_ipr_inmaininfo t,met_cas_diagnose me 
                        where me.inpatient_no='{0}'
                        and  me.persson_type='1'
                        and me.main_flag='1'
                        and me.valid_flag='1'
                        and rownum='1'";
            try
            {
                strSql = string.Format(strSql, inpatientNO);
            }
            catch
            {
            }
            this.ExecQuery(strSql);

            while (this.Reader.Read())
            {
                obj.Memo = string.IsNullOrEmpty(this.Reader[0].ToString()) ? "0" : this.Reader[0].ToString(); //病案上传状态
                obj.ID = this.Reader[1].ToString();
                obj.Name = this.Reader[2].ToString();
                obj.User02 = this.Reader[3].ToString();
            }
            return obj;
        }

        /// <summary>
        /// 根据住院号、出院日期判断是否存在之前还有未上传的数据
        /// 返回值： 0 需要上传 1 已经上传
        /// </summary>
        /// <param name="patient_no">住院号</param>
        /// <param name="out_date">出院日期</param>
        /// <returns></returns>
        public ArrayList GetIsHavedNoUpload(string patient_no, DateTime out_date)
        {
            int iReturn = 0;
            string strSql = @"select t.inpatient_no,t.patient_no,t.in_date from fin_ipr_inmaininfo t where t.patient_no= '{0}'
and t.out_date < to_date('{1}','yyyy-mm-dd HH24:mi:ss')
and (t.in_state ='B' or t.in_state='O')
and t.patient_no like '0%'
and t.casesend_flag='0'";
            ArrayList al = new ArrayList();
            try
            {
                strSql = string.Format(strSql, patient_no, out_date.Date.ToString("yyyy-MM-dd") + " 00:00:00");
                //查询
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    patientInfo.ID = Reader[0].ToString();
                    patientInfo.PID.PatientNO = Reader[1].ToString();
                    patientInfo.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[2].ToString());
                    al.Add(patientInfo);
                }
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            finally
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            return al;
        }
        #endregion
        #region  东莞桥头增加的方法
        /// <summary>
        /// 更新病案表中与主表对应的字段
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public int UpdateMainInfo(Neusoft.HISFC.Models.HealthRecord.Base patient)
        {
            string strSql = string.Empty;
            if (Sql.GetSql("CASE.BaseDML.UpdateMainBaseInfo.Update.HIS50", ref strSql) == -1)
            {
                return -1;
            }

            try
            {
                string[] s = new string[30];
                try
                {

                    s[0] = patient.PatientInfo.ID;//住院流水号
                    s[1] = patient.PatientInfo.PID.PatientNO;//住院病历号
                    s[2] = patient.PatientInfo.PID.CardNO;//卡号
                    s[3] = patient.PatientInfo.Name;//姓名
                    s[4] = patient.PatientInfo.Sex.ID.ToString();//性别
                    s[5] = patient.PatientInfo.Birthday.ToString();//出生日期
                    s[6] = patient.PatientInfo.Country.ID;//国家
                    s[7] = patient.PatientInfo.Nationality.ID;//民族
                    s[8] = patient.PatientInfo.Profession.ID;//职业
                    s[9] = patient.PatientInfo.MaritalStatus.ID.ToString();//婚否
                    s[10] = patient.PatientInfo.IDCard;//身份证号
                    s[11] = patient.InAvenue;//地区来源
                    s[12] = patient.PatientInfo.Pact.PayKind.ID;//结算类别号
                    s[13] = patient.PatientInfo.Pact.ID;//合同代码
                    s[14] = patient.PatientInfo.SSN;//医保公费号
                    s[15] = patient.PatientInfo.DIST;//籍贯
                    s[16] = patient.PatientInfo.AreaCode;//出生地
                    s[17] = patient.PatientInfo.AddressHome;//家庭住址
                    s[18] = patient.PatientInfo.PhoneHome;//家庭电话
                    s[19] = patient.PatientInfo.HomeZip;//住址邮编
                    s[20] = patient.PatientInfo.AddressBusiness;//单位地址
                    s[21] = patient.PatientInfo.PhoneBusiness;//单位电话
                    s[22] = patient.PatientInfo.BusinessZip;//单位邮编
                    s[23] = patient.PatientInfo.Kin.Name;//联系人
                    s[24] = patient.PatientInfo.Kin.RelationLink;//与患者关系
                    s[25] = patient.PatientInfo.Kin.RelationPhone;//联系电话                   
                    s[26] = patient.PatientInfo.PVisit.InTime.ToString();//入院日期
                    s[27] = patient.PatientInfo.InTimes.ToString();//入院次数
                    s[28] = patient.InAvenue;//入院来源（途径）
                    s[29] = patient.PatientInfo.PVisit.Circs.ID;//入院状态
                }
                catch (Exception ex)
                {
                    Err = "赋值时候出错！" + ex.Message;
                    WriteErr();
                    return -1;
                }
                strSql = string.Format(strSql, s);
            }
            catch (Exception ex)
            {
                Err = "赋值时候出错！" + ex.Message;
                WriteErr();
                return -1;
            }

            return ExecNoQuery(strSql);

        }

        #endregion
        #region  废弃
        /// <summary>
        /// 根据住院号和住院次数查询住院流水号 
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="InNum"></param>
        /// <returns></returns>
        [Obsolete(" 废弃,用QueryPatientInfoByInpatientAndInNum 代替", true)]
        public ArrayList GetPatientInfo(string inpatientNO, string InNum)
        {
            //先从病案主表中查询 如果没有查到 再在住院主表中查询 
            ArrayList list = new ArrayList();
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetPatientInfo.GetPatientInfo", ref strSql) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSql = string.Format(strSql, inpatientNO, InNum);
            this.ExecQuery(strSql);
            Neusoft.HISFC.Models.RADT.PatientInfo info = null;
            while (this.Reader.Read())
            {
                info = new Neusoft.HISFC.Models.RADT.PatientInfo();
                info.ID = this.Reader[0].ToString();
                list.Add(info);
                info = null;
            }
            if (list == null)
            {
                return list;
            }
            if (list.Count == 0)
            {
                //查询住院主表 获取病人信息
                if (this.Sql.GetSql("RADT.Inpatient.PatientInfoGetByTime", ref strSql) == -1)
                {
                    this.Err = "获取SQL语句失败";
                    return null;
                }
                strSql = string.Format(strSql, inpatientNO, InNum);
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    info = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    info.ID = this.Reader[0].ToString();
                    list.Add(info);
                    info = null;
                }
            }
            return list;
        }
        /// <summary>
        /// 查询未登记病案信息的患者的诊断信息,从met_com_diagnose中提取
        /// </summary>
        /// <param name="inpatientNO">患者住院流水号</param>
        /// <param name="diagType">诊断类别,要提取所有诊断输入%</param>
        /// <returns>诊断信息数组</returns>
        [Obsolete("废弃,用 QueryInhosDiagnoseInfo 代替", true)]
        public ArrayList GetInhosDiagInfo(string inpatientNO, string diagType)
        {
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetInhosDiagInfo.Select", ref strSql) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSql = string.Format(strSql, inpatientNO, diagType);

            return this.myGetDiagInfo(strSql);
        }
        /// <summary>
        /// 根据病案号获取信息
        /// </summary>
        /// <param name="CaseNo"></param>
        /// <returns></returns
        [Obsolete("废弃 用 QueryCaseBaseInfoByCaseNO 代替", true)]
        public ArrayList GetCaseBaseInfoByCaseNum(string CaseNo)
        {
            ArrayList list = new ArrayList();
            //获取主sql语句
            string strSQL = GetCaseSql();
            string str = "";
            if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfoByCaseNum.Select.where", ref str) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSQL += str;
            strSQL = string.Format(strSQL, CaseNo);
            return this.myGetCaseBaseInfo(strSQL);
        }
        /// <summary>
        /// 费用类别 
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，用 合同单位 代替", true)]
        public ArrayList GetPayKindCode()
        {
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "01";
            //info.Name = "自费";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "02";
            //info.Name = "医保";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "03";
            //info.Name = "公费";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "04";
            //info.Name = "特约单位";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "05";
            //info.Name = "本院职工";
            //list.Add(info);

            return list;
        }
        /// <summary>
        /// 血型列表
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，用 常数 BLOODTYPE 代替", true)]
        public ArrayList GetBloodType()
        {
            //血型列表 
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "0";
            //info.Name = "U";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "1";
            //info.Name = "A";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "2";
            //info.Name = "B";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "3";
            //info.Name = "AB";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "4";
            //info.Name = "O";
            //list.Add(info);

            return list;
            #region  住院处用的列表
            //			ArrayList list = new ArrayList();
            //			neusoft.HISFC.Object.Base.SpellCode info = null;
            //			ArrayList list2 = Neusoft.HISFC.Models.RADT.BloodType.List();
            //			foreach(Neusoft.FrameWork.Models.NeuObject obj in list2)
            //			{
            //				info = new neusoft.HISFC.Object.Base.SpellCode();
            //				info.ID = obj.ID;
            //				info.Name = obj.Name;
            //				list.Add(info);
            //			}
            #endregion
        }
        /// <summary>
        /// 输血反应
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，用常数 BloodReaction 代替", true)]
        public ArrayList GetReactionBlood()
        {
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "2";
            //info.Name = "无";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "1";
            //info.Name = "有";
            //list.Add(info);

            return list;
        }
        [Obsolete("废弃，用 常数 RHSTATE 代替", true)]
        public ArrayList GetRHType()
        {
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "1";
            //info.Name = "阴";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "2";
            //info.Name = "阳";
            //list.Add(info);

            return list;
        }
        /// <summary>
        /// 病案质量
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，用 常数 CASEQUALITY 代替", true)]
        public ArrayList GetCaseQC()
        {
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "0";
            //info.Name = "甲";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "1";
            //info.Name = "乙";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "2";
            //info.Name = "丙";
            //list.Add(info);

            return list;
        }
        /// <summary>
        /// 诊断符合情况
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，用常数 DIAGNOSEACCORD 代替", true)]
        public ArrayList GetDiagAccord()
        {
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "0";
            //info.Name = "未做";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "1";
            //info.Name = "符合";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "2";
            //info.Name = "不符合";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "3";
            //info.Name = "不肯定";
            //list.Add(info);

            return list;
        }
        /// <summary>
        /// 药物过敏
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，用常数 PHARMACYALLERGIC 代替", true)]
        public ArrayList GetHbsagList()
        {
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "0";
            //info.Name = "未做";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "1";
            //info.Name = "阴性";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "2";
            //info.Name = "阳性";
            //list.Add(info);

            return list;
        }
        /// <summary>
        /// 病人来源  
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，用 常数 INSOURCE 代替", true)]
        public ArrayList GetPatientSource()
        {
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "1";
            //info.Name = "本区";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "2";
            //info.Name = "本市";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "3";
            //info.Name = "外市";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "4";
            //info.Name = "外省";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "5";
            //info.Name = "港澳台";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "6";
            //info.Name = "外国";
            //list.Add(info);

            return list;
        }
        /// <summary>
        /// 获取性别列表
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，用枚举 代替", true)]
        public ArrayList GetSexList()
        {
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "M";
            //info.Name = "男";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "F";
            //info.Name = "女";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "U";
            //info.Name = "未知";
            //list.Add(info);

            //info = new neusoft.HISFC.Object.Base.SpellCode();
            //info.ID = "O";
            //info.Name = "其他";
            //list.Add(info);

            return list;
        }
        /// <summary>
        /// 婚姻列表
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，用枚举代替", true)]
        public ArrayList GetMaryList()
        {
            ArrayList list = new ArrayList();
            //neusoft.HISFC.Object.Base.SpellCode info = null;
            //ArrayList list2 = Neusoft.HISFC.Models.RADT.MaritalStatus.List();
            //foreach (Neusoft.FrameWork.Models.NeuObject obj in list2)
            //{
            //    info = new neusoft.HISFC.Object.Base.SpellCode();
            //    info.ID = obj.ID;
            //    info.Name = obj.Name;
            //    list.Add(info);
            //}
            return list;
        }
        /// <summary>
        /// 更新met_cas_base的门诊诊断 入院诊断 出院主要诊断 第一手术
        /// </summary>
        /// <param name="inpatienNO"></param>
        /// <param name="frmType"></param>
        /// <returns></returns>
        [Obsolete("废弃,用UpdateBaseDiagAndOperationNew代替", true)]
        public int UpdateBaseDiagAndOperation(string inpatienNO, Neusoft.HISFC.Models.HealthRecord.EnumServer.frmTypes frmType)
        {

            Neusoft.HISFC.BizLogic.HealthRecord.Diagnose dia = new Neusoft.HISFC.BizLogic.HealthRecord.Diagnose();
            Neusoft.HISFC.BizLogic.HealthRecord.Operation op = new Operation();
            if (this.Trans != null)
            {
                dia.SetTrans(Trans);
                op.SetTrans(Trans);
            }
            Neusoft.HISFC.Models.HealthRecord.Diagnose ClinicDiag = dia.GetFirstDiagnose(inpatienNO, Neusoft.HISFC.Models.HealthRecord.DiagnoseType.enuDiagnoseType.CLINIC, frmType);
            Neusoft.HISFC.Models.HealthRecord.Diagnose InhosDiag = dia.GetFirstDiagnose(inpatienNO, Neusoft.HISFC.Models.HealthRecord.DiagnoseType.enuDiagnoseType.IN, frmType);
            Neusoft.HISFC.Models.HealthRecord.Diagnose OutDiag = dia.GetFirstDiagnose(inpatienNO, Neusoft.HISFC.Models.HealthRecord.DiagnoseType.enuDiagnoseType.OUT, frmType);
            Neusoft.HISFC.Models.HealthRecord.OperationDetail ops = op.GetFirstOperation(inpatienNO, frmType);
            if (ClinicDiag == null || InhosDiag == null || OutDiag == null || ops == null)
            {
                return -1;
            }
            string[] str = new string[14];
            str[0] = inpatienNO;
            str[1] = ClinicDiag.DiagInfo.ICD10.ID;
            str[2] = ClinicDiag.DiagInfo.ICD10.Name;
            str[3] = InhosDiag.DiagInfo.ICD10.ID;
            str[4] = InhosDiag.DiagInfo.ICD10.Name;
            str[5] = OutDiag.DiagInfo.ICD10.ID;
            str[6] = OutDiag.DiagInfo.ICD10.Name;
            str[7] = OutDiag.DiagOutState;
            str[8] = OutDiag.CLPA;
            str[9] = ops.OperationInfo.ID;
            str[10] = ops.OperationInfo.Name;
            str[11] = ops.FirDoctInfo.ID;
            str[12] = ops.FirDoctInfo.Name;
            str[13] = ops.OperationDate.ToString();
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.UpdateBaseDiagAndOperation", ref strSql) == -1) return -1;

            strSql = string.Format(strSql, str);
            return this.ExecNoQuery(strSql);

        }
        #endregion

        #region 五院首页附页
        /// <summary>
        /// 向病案首页附页中插入一条记录
        /// </summary>
        /// <param name="b"></param>
        /// <returns> 成功返回 1 失败返回－1 ，0  </returns>
        public int InsertAdditional(Neusoft.HISFC.Models.HealthRecord.Additional b)
        {
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.Additional.Insert.new", ref strSql) == -1) return -1;

            return this.ExecNoQuery(strSql, GetAdditionalInfo(b));
        }
        /// <summary>
        /// 将实体 转变成字符串数组
        /// </summary>
        /// <param name="b"> 病案附页的实体类</param>
        /// <returns>失败返回null</returns>
        private string[] GetAdditionalInfo(Neusoft.HISFC.Models.HealthRecord.Additional b)
        {
            string[] s = new string[66];
            try
            {
                s[0] = b.Inpatient_No;//住院流水号

                s[1] = b.Preventivedrug;//Ⅰ类手术切口预防性应用抗菌药物 

                s[2] = b.Generaldrug;// 首选一、二代头孢

                s[3] = b.Medicationtime;//用药时机

                s[4] = b.Drugcombination;//联合用药

                s[5] = b.Drugadditional;//术中追加抗菌药

                s[6] = b.Drugstoptime;//术后停用抗菌药物时间

                s[7] = b.Pathway;//临床路径病种

                s[8] = b.Inpathway;//进入临床路径

                s[9] = b.Isendpathway;//完成临床路径

                s[10] = b.Isdrgs;//纳入特定（单）病种控制

                s[11] = b.Uploaddrgs;//上报特定（单）病种指标 

                s[12] = b.Inandout;//出院与入院

                s[13] = b.Pathology;//恶性肿瘤术前诊断与术后病理诊断

                s[14] = b.Pacsandpat;//放射与病理

                s[15] = b.Isoptical;//进行手术冰冻与石蜡病理检查

                s[16] = b.Opticaltrue;//手术冰冻与石蜡病理检查符合

                s[17] = b.Notplanback;//同一疾病(主要诊断)出院后非计划再住院

                s[18] = b.Notplanops;//非预期再次手术

                s[19] = b.Notplanopsdead;//非预期再次手术死亡

                s[20] = b.Notplanicuback;//非预期重返ICU

                s[21] = b.Apache;//入ICU患者APACHEⅡ评分

                s[22] = b.Opscomplication;//择期术后并发症

                s[23] = b.Complicationcode1;//并发症编码1

                s[24] = b.Complicationname1;//并发症名称1

                s[25] = b.Complicationcode2;//并发症编码2

                s[26] = b.Complicationname2;//并发症名称2

                s[27] = b.Complicationcode3;//并发症编码3

                s[28] = b.Complicationname3;//并发症名称3

                s[29] = b.Deathofcomp;//因术后并发症导致死亡

                s[30] = b.Suddendeath;//术后猝死

                s[31] = b.Foreignbody;//手术过程中发生异物遗留

                s[32] = b.Deathpenoperation;//择期手术患者围术期死亡

                s[33] = b.Wound;//发生医源性意外穿刺或撕裂伤

                s[34] = b.Pneumothorax;//发生医源性气胸

                s[35] = b.Infusion;//输液

                s[36] = b.Infusionreaction;//输液反应

                s[37] = b.Bloodtrans;//输血

                s[38] = b.Bloodtransreaction;//输血反应

                s[39] = b.Hemodialysis;//血液透析

                s[40] = b.Heminfection;//发生与血液透析相关血液感染

                s[41] = b.Livebirths;//产科新生儿情况：活产数

                s[42] = b.Modeofdelivery;//分娩方式

                s[43] = b.Birthinjury;//产伤

                s[44] = b.Morebaby;//其他补充：（多胎情况）

                s[45] = b.Vaginaldelivery;//产妇情况（经阴道分娩）：器械辅助阴道分娩

                s[46] = b.Obstetrictrauma;//产科创伤

                s[47] = b.Postpartumbleeding;//产后出血（500-1000ml）

                s[48] = b.Beriousbleeding;//产后严重出血（≥1000ml）

                s[49] = b.Mark1;//备注

                s[50] = b.Mark2;//备注

                s[51] = b.Mark3;//备注

                s[52] = b.Mark4;//备注

                s[53] = b.Mark5;//备注
                s[54] = b.Mark6;//备注
                s[55] = b.Mark7;//备注
                s[56] = b.Mark8;//备注
                s[57] = b.Mark9;//备注
                s[58] = b.Mark10;//备注
                s[59] = b.Mark11;//备注
                s[60] = b.Mark12;//备注
                s[61] = b.Mark13;//备注
                s[62] = b.Mark14;//备注
                s[63] = b.Mark15;//备注
                s[64] = b.Mark16;//备注
                s[65] = b.Mark17;//备注
                return s;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }

        }
        /// <summary>
        /// 更新病案附页表
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int UpdateAdditional(Neusoft.HISFC.Models.HealthRecord.Additional b)
        {
            string strSql = "";
            if (this.Sql.GetSql("CASE.BaseDML.Additional.Update.new", ref strSql) == -1) return -1;
            return this.ExecNoQuery(strSql, GetAdditionalInfo(b));
        }

        /// <summary>
        /// 根据患者流水号获取首页附页信息
        /// </summary>
        /// <param name="inpatienNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.Additional QueryAdditional(string inpatienNO)
        {
            //ArrayList list = new ArrayList();
            //获取主sql语句
            string strSQL = string.Empty;
            if (this.Sql.GetSql("CASE.BaseDML.Additional.Select.where.new", ref strSQL) == -1)
            {
                this.Err = "获取SQL语句失败";
                return null;
            }
            strSQL = string.Format(strSQL, inpatienNO);
            return this.myGetAdditionalInfo(strSQL);
        }
        /// <summary>
        /// 根据SQL查询符合条件病案首页的附页信息
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>失败返回 null 成功返回符合条件的信息</returns>
        private Neusoft.HISFC.Models.HealthRecord.Additional myGetAdditionalInfo(string strSQL)
        {
            //执行操查询操作
            this.ExecQuery(strSQL);
            //读取数据
            //ArrayList list = new ArrayList();
            Neusoft.HISFC.Models.HealthRecord.Additional b = null;
            try
            {
                while (this.Reader.Read())
                {
                    b = new Neusoft.HISFC.Models.HealthRecord.Additional();
                    b.Inpatient_No = this.Reader[0] == DBNull.Value ? string.Empty : this.Reader[0].ToString();//住院流水号
                    b.Preventivedrug = this.Reader[1] == DBNull.Value ? string.Empty : this.Reader[1].ToString();//Ⅰ类手术切口预防性应用抗菌药物 

                    b.Generaldrug = this.Reader[2] == DBNull.Value ? string.Empty : this.Reader[2].ToString();// 首选一、二代头孢

                    b.Medicationtime = this.Reader[3] == DBNull.Value ? string.Empty : this.Reader[3].ToString();//用药时机

                    b.Drugcombination = this.Reader[4] == DBNull.Value ? string.Empty : this.Reader[4].ToString();//联合用药

                    b.Drugadditional = this.Reader[5] == DBNull.Value ? string.Empty : this.Reader[5].ToString();//术中追加抗菌药

                    b.Drugstoptime = this.Reader[6] == DBNull.Value ? string.Empty : this.Reader[6].ToString();//术后停用抗菌药物时间

                    b.Pathway = this.Reader[7] == DBNull.Value ? string.Empty : this.Reader[7].ToString();//临床路径病种

                    b.Inpathway = this.Reader[8] == DBNull.Value ? string.Empty : this.Reader[8].ToString();//进入临床路径

                    b.Isendpathway = this.Reader[9] == DBNull.Value ? string.Empty : this.Reader[9].ToString();//完成临床路径

                    b.Isdrgs = this.Reader[10] == DBNull.Value ? string.Empty : this.Reader[10].ToString();//纳入特定（单）病种控制

                    b.Uploaddrgs = this.Reader[11] == DBNull.Value ? string.Empty : this.Reader[11].ToString();//上报特定（单）病种指标 

                    b.Inandout = this.Reader[12] == DBNull.Value ? string.Empty : this.Reader[12].ToString();//出院与入院

                    b.Pathology = this.Reader[13] == DBNull.Value ? string.Empty : this.Reader[13].ToString();//恶性肿瘤术前诊断与术后病理诊断

                    b.Pacsandpat = this.Reader[14] == DBNull.Value ? string.Empty : this.Reader[14].ToString();//放射与病理

                    b.Isoptical = this.Reader[15] == DBNull.Value ? string.Empty : this.Reader[15].ToString();//进行手术冰冻与石蜡病理检查

                    b.Opticaltrue = this.Reader[16] == DBNull.Value ? string.Empty : this.Reader[16].ToString();//手术冰冻与石蜡病理检查符合

                    b.Notplanback = this.Reader[17] == DBNull.Value ? string.Empty : this.Reader[17].ToString();//同一疾病(主要诊断)出院后非计划再住院

                    b.Notplanops = this.Reader[18] == DBNull.Value ? string.Empty : this.Reader[18].ToString();//非预期再次手术

                    b.Notplanopsdead = this.Reader[19] == DBNull.Value ? string.Empty : this.Reader[19].ToString();//非预期再次手术死亡

                    b.Notplanicuback = this.Reader[20] == DBNull.Value ? string.Empty : this.Reader[20].ToString();//非预期重返ICU

                    b.Apache = this.Reader[21] == DBNull.Value ? string.Empty : this.Reader[21].ToString();//入ICU患者APACHEⅡ评分

                    b.Opscomplication = this.Reader[22] == DBNull.Value ? string.Empty : this.Reader[22].ToString();//择期术后并发症

                    b.Complicationcode1 = this.Reader[23] == DBNull.Value ? string.Empty : this.Reader[23].ToString();//并发症编码1

                    b.Complicationname1 = this.Reader[24] == DBNull.Value ? string.Empty : this.Reader[24].ToString();//并发症名称1

                    b.Complicationcode2 = this.Reader[25] == DBNull.Value ? string.Empty : this.Reader[25].ToString();//并发症编码2

                    b.Complicationname2 = this.Reader[26] == DBNull.Value ? string.Empty : this.Reader[26].ToString();//并发症名称2

                    b.Complicationcode3 = this.Reader[27] == DBNull.Value ? string.Empty : this.Reader[27].ToString();//并发症编码3

                    b.Complicationname3 = this.Reader[28] == DBNull.Value ? string.Empty : this.Reader[28].ToString();//并发症名称3

                    b.Deathofcomp = this.Reader[29] == DBNull.Value ? string.Empty : this.Reader[29].ToString();//因术后并发症导致死亡

                    b.Suddendeath = this.Reader[30] == DBNull.Value ? string.Empty : this.Reader[30].ToString();//术后猝死

                    b.Foreignbody = this.Reader[31] == DBNull.Value ? string.Empty : this.Reader[31].ToString();//手术过程中发生异物遗留

                    b.Deathpenoperation = this.Reader[32] == DBNull.Value ? string.Empty : this.Reader[32].ToString();//择期手术患者围术期死亡

                    b.Wound = this.Reader[33] == DBNull.Value ? string.Empty : this.Reader[33].ToString();//发生医源性意外穿刺或撕裂伤

                    b.Pneumothorax = this.Reader[34] == DBNull.Value ? string.Empty : this.Reader[34].ToString();//发生医源性气胸

                    b.Infusion = this.Reader[35] == DBNull.Value ? string.Empty : this.Reader[35].ToString();//输液

                    b.Infusionreaction = this.Reader[36] == DBNull.Value ? string.Empty : this.Reader[36].ToString();//输液反应

                    b.Bloodtrans = this.Reader[37] == DBNull.Value ? string.Empty : this.Reader[37].ToString();//输血

                    b.Bloodtransreaction = this.Reader[38] == DBNull.Value ? string.Empty : this.Reader[38].ToString();//输血反应

                    b.Hemodialysis = this.Reader[39] == DBNull.Value ? string.Empty : this.Reader[39].ToString();//血液透析

                    b.Heminfection = this.Reader[40] == DBNull.Value ? string.Empty : this.Reader[40].ToString();//发生与血液透析相关血液感染

                    b.Livebirths = this.Reader[41] == DBNull.Value ? string.Empty : this.Reader[41].ToString();//产科新生儿情况：活产数

                    b.Modeofdelivery = this.Reader[42] == DBNull.Value ? string.Empty : this.Reader[42].ToString();//分娩方式

                    b.Birthinjury = this.Reader[43] == DBNull.Value ? string.Empty : this.Reader[43].ToString();//产伤

                    b.Morebaby = this.Reader[44] == DBNull.Value ? string.Empty : this.Reader[44].ToString();//其他补充：（多胎情况）

                    b.Vaginaldelivery = this.Reader[45] == DBNull.Value ? string.Empty : this.Reader[45].ToString();//产妇情况（经阴道分娩）：器械辅助阴道分娩

                    b.Obstetrictrauma = this.Reader[46] == DBNull.Value ? string.Empty : this.Reader[46].ToString();//产科创伤

                    b.Postpartumbleeding = this.Reader[47] == DBNull.Value ? string.Empty : this.Reader[47].ToString();//产后出血（500-1000ml）

                    b.Beriousbleeding = this.Reader[48] == DBNull.Value ? string.Empty : this.Reader[48].ToString();//产后严重出血（≥1000ml）

                    b.Mark1 = this.Reader[49] == DBNull.Value ? string.Empty : this.Reader[49].ToString();//备注

                    b.Mark2 = this.Reader[50] == DBNull.Value ? string.Empty : this.Reader[50].ToString();//备注

                    b.Mark3 = this.Reader[51] == DBNull.Value ? string.Empty : this.Reader[51].ToString();//备注

                    b.Mark4 = this.Reader[52] == DBNull.Value ? string.Empty : this.Reader[52].ToString();//备注

                    b.Mark5 = this.Reader[53] == DBNull.Value ? string.Empty : this.Reader[53].ToString();//备注
                    b.Mark6 = this.Reader[54] == DBNull.Value ? string.Empty : this.Reader[54].ToString();//备注

                    b.Mark7 = this.Reader[55] == DBNull.Value ? string.Empty : this.Reader[55].ToString();//备注

                    b.Mark8 = this.Reader[56] == DBNull.Value ? string.Empty : this.Reader[56].ToString();//备注

                    b.Mark9 = this.Reader[57] == DBNull.Value ? string.Empty : this.Reader[57].ToString();//备注

                    b.Mark10 = this.Reader[58] == DBNull.Value ? string.Empty : this.Reader[58].ToString();//备注

                    b.Mark11 = this.Reader[59] == DBNull.Value ? string.Empty : this.Reader[59].ToString();//备注

                    b.Mark12 = this.Reader[60] == DBNull.Value ? string.Empty : this.Reader[60].ToString();//备注

                    b.Mark13 = this.Reader[61] == DBNull.Value ? string.Empty : this.Reader[61].ToString();//备注

                    b.Mark14 = this.Reader[62] == DBNull.Value ? string.Empty : this.Reader[62].ToString();//备注

                    b.Mark15 = this.Reader[63] == DBNull.Value ? string.Empty : this.Reader[63].ToString();//备注

                    b.Mark16 = this.Reader[64] == DBNull.Value ? string.Empty : this.Reader[64].ToString();//备注

                    b.Mark17 = this.Reader[65] == DBNull.Value ? string.Empty : this.Reader[65].ToString();//备注
                    //list.Add(b);
                }
                return b;
            }
            catch (Exception ex)
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                this.Err = "获得患者病案信息出错!" + ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }
        #endregion

        #region icd编码上传(新)

        /// <summary>
        /// 获取上传信息
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.ICDScoreLog GetUploadICD(string inpatientNo)
        {
            Neusoft.HISFC.Models.HealthRecord.ICDScoreLog icdlog = new Neusoft.HISFC.Models.HealthRecord.ICDScoreLog();
            string strSQL = "";
            try
            {
                if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfo.GetUploadICD", ref strSQL) == -1)
                {
                    this.Err = "获取SQL语句失败";
                    return null;
                }
                strSQL = string.Format(strSQL, inpatientNo);
                this.ExecQuery(strSQL);
                while (this.Reader.Read())
                {
                    icdlog.Inptient_no = this.Reader[0].ToString();
                    icdlog.Icd10_1 = this.Reader[1].ToString();
                    icdlog.Icd10_2 = this.Reader[2].ToString();
                    icdlog.Icd9 = this.Reader[3].ToString();
                    icdlog.Si_type = this.Reader[4].ToString();
                }
                return icdlog;
            }
            catch
            {
                return null;
            }

        }

        //增加编码名称
        /// <summary>
        /// 获取上传信息
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.ICDScoreLog GetUploadICD1(string inpatientNo)
        {
            Neusoft.HISFC.Models.HealthRecord.ICDScoreLog icdlog = new Neusoft.HISFC.Models.HealthRecord.ICDScoreLog();
            string strSQL = "";
            try
            {
                if (this.Sql.GetSql("CASE.BaseDML.GetCaseBaseInfo.GetUploadICD1", ref strSQL) == -1)
                {
                    this.Err = "获取SQL语句失败";
                    return null;
                }
                strSQL = string.Format(strSQL, inpatientNo);
                this.ExecQuery(strSQL);
                while (this.Reader.Read())
                {
                    icdlog.Inptient_no = this.Reader[0].ToString();
                    icdlog.Icd10_1 = this.Reader[1].ToString();
                    icdlog.Icd10_1_Name = this.Reader[2].ToString();
                    icdlog.Icd10_2 = this.Reader[3].ToString();
                    icdlog.Icd10_2_Name = this.Reader[4].ToString();
                    icdlog.Icd9 = this.Reader[5].ToString();
                    icdlog.Icd9_Nmae = this.Reader[6].ToString();
                    icdlog.Si_type = this.Reader[7].ToString();
                }
                return icdlog;
            }
            catch
            {
                return null;
            }

        }
        #endregion
    }
}
