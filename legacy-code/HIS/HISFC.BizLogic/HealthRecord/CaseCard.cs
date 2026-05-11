using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
namespace Neusoft.HISFC.BizLogic.HealthRecord
{
    /// <summary>
    /// 借阅归还
    /// </summary>
    public class CaseCard : Neusoft.FrameWork.Management.Database
    {
        #region 借阅卡 基础数据维护
        /// <summary>
        /// 获取所有的借阅卡号信息 
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int GetCardInfo(ref System.Data.DataSet ds)
        {
            try
            {
                string strSql = GeCardSql();
                //查询
                return this.ExecQuery(strSql, ref ds);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }
        private string GeCardSql()
        {
            string strSql = "";
            if (this.Sql.GetSql("Case.CaseCard.GetCardInfo", ref strSql) == -1) return null;
            return strSql;
        }
        /// <summary>
        /// 根据卡号获取信息 
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.HealthRecord.ReadCard GetCardInfo(string CardID)
        {
            Neusoft.HISFC.Models.HealthRecord.ReadCard info = new Neusoft.HISFC.Models.HealthRecord.ReadCard();
            try
            {
                string strSql = "";
                string strSql1 = GeCardSql();
                if (strSql1 == null)
                {
                    return null;
                }
                if (this.Sql.GetSql("Case.CaseCard.GetCardInfo.1", ref strSql) == -1) return null;
                strSql1 += strSql;
                strSql1 = string.Format(strSql1, CardID);
                //查询
                this.ExecQuery(strSql1);
                while (this.Reader.Read())
                {
                    info.CardID = this.Reader[0].ToString(); //卡号
                    info.EmployeeInfo.ID = this.Reader[1].ToString(); //员工号
                    info.EmployeeInfo.Name = this.Reader[2].ToString();//员工姓名
                    info.DeptInfo.ID = this.Reader[3].ToString();//科室代码
                    info.DeptInfo.Name = this.Reader[4].ToString();//科室名称
                    info.User01 = this.Reader[5].ToString();//操作员
                    info.EmployeeInfo.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6].ToString());//操作时间
                    info.ValidFlag = this.Reader[7].ToString();//有效
                    info.CancelOperInfo.Name = this.Reader[8].ToString();//作废人
                    info.CancelDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[9].ToString());//作废时间
                }
                this.Reader.Close();
                return info;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
        }
        /// <summary>
        /// 借阅卡号是否已经存在 
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns> -1 出错 ，1 存在 ，2 不存在 </returns>
        public int IsExist(string CardID)
        {
            try
            {
                string strSql = "";
                if (this.Sql.GetSql("Case.CaseCard.GetCardInfo.1", ref strSql) == -1) return -1;
                strSql = string.Format(strSql, CardID);
                //查询
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    return 1;
                }
                this.Reader.Close();
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            return 2;
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.HealthRecord.ReadCard info)
        {
            try
            {
                string strSql = "";
                if (this.Sql.GetSql("Case.CaseCard.Insert", ref strSql) == -1) return -1;
                string[] Str = GetInfo(info);
                strSql = string.Format(strSql, Str);
                //查询
                return this.ExecNoQuery(strSql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int Update(Neusoft.HISFC.Models.HealthRecord.ReadCard info)
        {
            try
            {
                string strSql = "";
                if (this.Sql.GetSql("Case.CaseCard.Update", ref strSql) == -1) return -1;
                string[] Str = GetInfo(info);
                strSql = string.Format(strSql, Str);
                //查询
                return this.ExecNoQuery(strSql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }
        private string[] GetInfo(Neusoft.HISFC.Models.HealthRecord.ReadCard obj)
        {
            string[] str = new string[10];
            try
            {
                str[0] = obj.CardID; //卡号
                str[1] = obj.EmployeeInfo.ID; //员工号
                str[2] = obj.EmployeeInfo.Name;//员工姓名
                str[3] = obj.DeptInfo.ID;//科室代码
                str[4] = obj.DeptInfo.Name;//科室名称
                str[5] = obj.User01;//操作员
                str[6] = obj.EmployeeInfo.OperTime.ToString();//操作时间
                str[7] = obj.ValidFlag;//有效
                str[8] = obj.CancelOperInfo.Name;//作废人
                str[9] = obj.CancelDate.ToString();//作废时间
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            return str;
        }
        #endregion

        #region  病案借阅
        /// <summary>
        /// 借出
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int LendCase(Neusoft.HISFC.Models.HealthRecord.Lend info)
        {
            string[] arrStr = getInfo(info);
            string strSql = "";
            if (this.Sql.GetSql("Case.CaseCard.LendCase", ref strSql) == -1) return -1;
            strSql = string.Format(strSql, arrStr);
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新病案的标志 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="PaptientNO"></param>
        /// <returns></returns>
        public int UpdateBase(Neusoft.HISFC.Models.HealthRecord.EnumServer.LendType type, string CaseNO)
        {
            string strSql = "";
            if (this.Sql.GetSql("Case.CaseCard.UpdateBase", ref strSql) == -1) return -1;
            strSql = string.Format(strSql, type.ToString(), CaseNO);
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 归还 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int ReturnCase(Neusoft.HISFC.Models.HealthRecord.Lend info)
        {
            string[] arrStr = getInfo(info);
            string strSql = "";
            if (this.Sql.GetSql("Case.CaseCard.ReturnCase", ref strSql) == -1) return -1;
            strSql = string.Format(strSql, arrStr);
            return this.ExecNoQuery(strSql);
        }
        private string[] getInfo(Neusoft.HISFC.Models.HealthRecord.Lend info)
        {
            string[] str = new string[28];
            str[0] = info.CaseBase.PatientInfo.ID;//住院流水号
            str[1] = info.CaseBase.CaseNO;//病人住院号
            str[2] = info.CaseBase.PatientInfo.Name; //病人姓名
            str[3] = info.CaseBase.PatientInfo.Sex.ID.ToString();//性别
            str[4] = info.CaseBase.PatientInfo.Birthday.ToString();//出生日期
            str[5] = info.CaseBase.PatientInfo.PVisit.InTime.ToString();//入院日期
            str[6] = info.CaseBase.PatientInfo.PVisit.OutTime.ToString();//出院日期
            str[7] = info.CaseBase.InDept.ID; //入院科室代码
            str[8] = info.CaseBase.InDept.Name; //入院科室名称
            str[9] = info.CaseBase.OutDept.ID;  //出院科室代码
            str[10] = info.CaseBase.OutDept.Name; //出院科室名称
            str[11] = info.EmployeeInfo.ID;//借阅人代号
            str[12] = info.EmployeeInfo.Name;//借阅人姓名
            str[13] = info.EmployeeDept.ID; //借阅人所在科室代码
            str[14] = info.EmployeeDept.Name; //借阅人所在科室名称
            str[15] = info.LendDate.ToString(); //借阅日期
            str[16] = info.PrerDate.ToString(); //预定还期
            str[17] = info.LendKind; //借阅性质
            str[18] = info.LendStus;//病历状态 1借出/2返还
            str[19] = info.ID; //操作员代号
            str[20] = info.OperInfo.OperTime.ToString(); //操作时间
            str[21] = info.ReturnOperInfo.ID;   //归还操作员代号
            str[22] = info.ReturnDate.ToString();   //实际归还日期
            str[23] = info.CardNO;//卡号
            str[24] = info.Memo; //返还情况
            str[25] = info.LendNum;// 份数
            str[26] = info.SeqNO; //序列
            //str[27] = info.PatientInfo.PatientInfo.InTimes.ToString();//住院次数
            return str;
        }
        /// <summary>
        /// 根据卡号查询需要归还的信息
        /// </summary>
        /// <param name="LendCardNo"></param>
        /// <returns></returns>
        public ArrayList QueryLendInfo(string LendCardNo)
        {
            string StrSql = GetLendSql();
            if (StrSql == null)
            {
                return null;
            }
            string strSql = "";
            if (this.Sql.GetSql("Case.CaseCard.GetLendSql.where", ref strSql) == -1) return null;
            StrSql += strSql;
            StrSql = string.Format(StrSql, LendCardNo);
            this.ExecQuery(StrSql);
            return QueryLendInfoBase();
        }
        /// <summary>
        /// 根据病案号病案借阅信息
        /// </summary>
        /// <param name="CaseNO"></param>
        /// <returns></returns>
        public ArrayList QueryLendInfoByCaseNO(string CaseNO) 
        {
            string StrSql = GetLendSql();
            if (StrSql == null)
            {
                return null;
            }
            string strSql = "";
            if (this.Sql.GetSql("Case.CaseCard.QueryLendInfo.where", ref strSql) == -1) return null;
            StrSql += strSql;
            StrSql = string.Format(StrSql, CaseNO);
            this.ExecQuery(StrSql);
            return QueryLendInfoBase();
        }
        /// <summary>
        /// 私有函数
        /// </summary>
        /// <returns></returns>
        private ArrayList QueryLendInfoBase()
        {
            try
            {
                ArrayList list = new ArrayList();
                Neusoft.HISFC.Models.HealthRecord.Lend info = null;
                while (this.Reader.Read())
                {
                    info = new Neusoft.HISFC.Models.HealthRecord.Lend();
                    info.CaseBase.PatientInfo.ID = this.Reader[0].ToString();//住院流水号
                    info.CaseBase.CaseNO = this.Reader[1].ToString();//病人住院号
                    info.CaseBase.PatientInfo.Name = this.Reader[2].ToString(); //病人姓名
                    info.CaseBase.PatientInfo.Sex.ID = this.Reader[3].ToString();//性别
                    info.CaseBase.PatientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4].ToString());//出生日期
                    info.CaseBase.PatientInfo.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[5].ToString());//入院日期
                    info.CaseBase.PatientInfo.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6].ToString());//出院日期
                    info.CaseBase.InDept.ID = this.Reader[7].ToString(); //入院科室代码
                    info.CaseBase.InDept.Name = this.Reader[8].ToString(); //入院科室名称
                    info.CaseBase.OutDept.ID = this.Reader[9].ToString();  //出院科室代码
                    info.CaseBase.OutDept.Name = this.Reader[10].ToString(); //出院科室名称
                    info.EmployeeInfo.ID = this.Reader[11].ToString();//借阅人代号
                    info.EmployeeInfo.Name = this.Reader[12].ToString();//借阅人姓名
                    info.EmployeeDept.ID = this.Reader[13].ToString(); //借阅人所在科室代码
                    info.EmployeeDept.Name = this.Reader[14].ToString(); //借阅人所在科室名称
                    info.LendDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[15].ToString()); //借阅日期
                    info.PrerDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[16].ToString()); //预定还期
                    info.LendKind = this.Reader[17].ToString(); //借阅性质
                    info.LendStus = this.Reader[18].ToString();//病历状态 1借出/2返还
                    info.ID = this.Reader[19].ToString(); //操作员代号
                    info.OperInfo.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[20].ToString()); //操作时间
                    info.ReturnOperInfo.ID = this.Reader[21].ToString();   //归还操作员代号
                    info.ReturnDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[22].ToString());   //实际归还日期
                    info.CardNO = this.Reader[23].ToString();//卡号
                    info.Memo = this.Reader[24].ToString(); //返还情况
                    info.LendNum = this.Reader[25].ToString();//份数
                    info.SeqNO = this.Reader[26].ToString(); //发生序号
                    //info.CaseBase.PatientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[27].ToString());//住院次数
                    list.Add(info);
                }
                this.Reader.Close();
                return list;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
        }
        private string GetLendSql()
        {
            string strSql = "";
            if (this.Sql.GetSql("Case.CaseCard.GetLendSql", ref strSql) == -1) return null;
            return strSql;
        }

        /// <summary>
        /// 自设定where条件检索借阅信息
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public ArrayList QueryLendInfoSetWhere(string strWhere)
        {
            string StrSql = GetLendSql();
            if (StrSql == null)
            {
                return null;
            }

            StrSql += strWhere;
            StrSql = string.Format(StrSql);

            if (this.ExecQuery(StrSql) == -1)
            {
                return null;
            }

            return QueryLendInfoBase();

        }

        /// <summary>
        /// 更新借阅信息的状态 add by lk
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int UpdateLendInfo(Neusoft.HISFC.Models.HealthRecord.Lend info)
        {
            string strSql = "";

            //if (this.Sql.GetSql("Case.CaseCard.UpdateLendCase.stus", ref strSql) == -1)
            //{
            //    return -1;

            //}
            if (this.Sql.GetSql("Case.CaseCard.ReturnCase", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, 
                    //info.SeqNO,
                                               info.CaseBase.PatientInfo.ID,
                                               info.CaseBase.CaseNO,//病人住院号
                    info.CaseBase.PatientInfo.Name, //病人姓名
                    info.CaseBase.PatientInfo.Sex.ID,//性别
                    info.CaseBase.PatientInfo.Birthday,//出生日期
                    info.CaseBase.PatientInfo.PVisit.InTime,//入院日期
                    info.CaseBase.PatientInfo.PVisit.OutTime,//出院日期
                    info.CaseBase.InDept.ID,//入院科室代码
                    info.CaseBase.InDept.Name,//入院科室名称
                    info.CaseBase.OutDept.ID,  //出院科室代码
                    info.CaseBase.OutDept.Name,//出院科室名称
                    info.EmployeeInfo.ID,//借阅人代号
                    info.EmployeeInfo.Name,//借阅人姓名
                    info.EmployeeDept.ID, //借阅人所在科室代码
                    info.EmployeeDept.Name, //借阅人所在科室名称
                    info.LendDate, //借阅日期
                    info.PrerDate, //预定还期
                    info.LendKind, //借阅性质
                    info.LendStus,//病历状态 1借出/2返还
                    info.ID, //操作员代号
                    info.OperInfo.OperTime, //操作时间
                    info.ReturnOperInfo.ID,//归还操作员代号
                    info.ReturnDate,  //实际归还日期
                    info.CardNO,//卡号
                    info.Memo ,//返还情况
                    info.LendNum.ToString(),//份数
                    info.SeqNO,//流水号
                    info.CaseBase.PatientInfo.InTimes.ToString()//住院次数
                    
                     );

                return this.ExecNoQuery(strSql);

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;

                return -1;
            }

            return 1;
        }

       /// <summary>
        /// 查询病案借阅信息或者医生电子病历借阅申请信息
       /// </summary>
        /// <param name="Type">类型 PaperCase 纸质病案借阅 ElectronCase 电子病历医生借阅申请</param>
       /// <returns></returns>
        public List<Neusoft.HISFC.Models.HealthRecord.Lend> QueryNeedBack(Neusoft.HISFC.Models.HealthRecord.EnumServer.LendCaseType Type )
        {
            string StrSql = string.Empty;
            //if (Type == Neusoft.HISFC.Models.HealthRecord.EnumServer.LendCaseType.Return)
            //{
            //    if (this.Sql.GetSql("Case.CaseCard.QueryNeedBack", ref StrSql) == -1) return null;
            //}
            //else 
            //{
                if (this.Sql.GetSql("Case.CaseCard.ElectronCase.LendPetition", ref StrSql) == -1) return null;
                StrSql = string.Format(StrSql, (int)Type);
            //}
            this.ExecQuery(StrSql);
            try
            {
                List<Neusoft.HISFC.Models.HealthRecord.Lend> list = new List<Neusoft.HISFC.Models.HealthRecord.Lend>();
                Neusoft.HISFC.Models.HealthRecord.Lend info = null;
                while (this.Reader.Read())
                {
                    info = new Neusoft.HISFC.Models.HealthRecord.Lend();

                    info.CardNO = this.Reader[11].ToString();//卡号
                    info.EmployeeInfo.Name = this.Reader[12].ToString();//借阅人姓名
                    info.EmployeeDept.Name = this.Reader[14].ToString(); //借阅人所在科室名称
                    info.LendDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[15].ToString()); //借阅日期
                    list.Add(info);
                }
                this.Reader.Close();
                return list;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 组合条件查询借阅信息
        /// </summary>
        /// <param name="personCode">借阅人编号</param>
        /// <param name="deptCode">科室编号</param>
        /// <param name="LendState">借阅状态</param>
        /// <param name="dtBegin">借阅开始日期</param>
        /// <param name="dtEnd">借阅结束日期</param>
        /// <param name="ds">返回 数据集</param>
        /// <returns></returns>
        public int QueryLendCaseByMoreCondition(string personCode, string deptCode, string LendState, DateTime dtBegin, DateTime dtEnd, ref DataSet ds)
        {
            try
            {
                //if (this.Sql.GetSql("HealthReacord.Case.CaseStroe.Select1", ref strSql) == -1) return -1;
                string strSql = "";
                strSql = @"
SELECT 
l.PATIENT_NO as 住院号,
l.NAME as 姓名,
l.OUT_DATE as 出院日期,
l.EMPL_NAME as 借阅人 ,
l.DEPT_NAME as 借阅科室,
l.LEND_DATE as 借阅日期,
(SELECT NAME  FROM  COM_DICTIONARY WHERE TYPE='CASE_LEND_TYPE' AND CODE= l.LEND_KIND) as 借阅类型,
--days (CURRENT TIMESTAMP) - days(l.LEND_DATE) as 借阅天数,
trunc(sysdate) - trunc(l.LEND_DATE) as 借阅天数,
CASE WHEN l.len_stus='1' THEN '未还' WHEN l.len_stus='2' THEN '已还' WHEN l.len_stus='3' THEN '申请医务科未审核' WHEN l.len_stus='4' THEN '申请病案室未通过' END AS 归还情况
FROM MET_CAS_LEND  l
WHERE (l.EMPL_CODE='{0}' OR 'ALL'='{0}')
AND (l.DEPT_CODE='{1}' OR 'ALL'='{1}')
AND (l.LEN_STUS='{2}' OR 'ALL'='{2}')
--AND l.LEND_DATE BETWEEN '{3}' AND '{4}'
AND l.LEND_DATE BETWEEN to_date('{3}','yyyy-mm-dd HH24:mi:ss') AND to_date('{4}','yyyy-mm-dd HH24:mi:ss')
ORDER BY  l.LEND_DATE
";
                try
                {
                    //查询
                    strSql = string.Format(strSql, personCode, deptCode, LendState, dtBegin, dtEnd);
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
        /// 查询借阅信息
        /// </summary>
        /// <param name="DtBegin">借阅开始日期</param>
        /// <param name="dtEnd">借阅结束日期</param>
        /// <param name="lendState">借阅状态</param>
        /// <param name="ds">返回数据集</param>
        /// <returns></returns>
        public int QueryLendCaseByLendDate(DateTime DtBegin, DateTime dtEnd, string lendState, ref DataSet ds)
        {
            try
            {
                //if (this.Sql.GetSql("HealthReacord.Case.CaseStroe.Select1", ref strSql) == -1) return -1;
                DtBegin = DtBegin.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
                dtEnd = dtEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                string strSql = "";
                strSql = @"
SELECT 
l.PATIENT_NO as 住院号,
l.NAME as 姓名,
l.OUT_DATE as 出院日期,
l.EMPL_NAME as 借阅人 ,
l.DEPT_NAME as 借阅科室,
l.LEND_DATE as 借阅日期,
(SELECT NAME  FROM  COM_DICTIONARY WHERE TYPE='CASE_LEND_TYPE' AND CODE= l.LEND_KIND) as 借阅类型,
--days (CURRENT TIMESTAMP) - days(l.LEND_DATE) as 借阅天数
trunc(sysdate) - trunc(l.LEND_DATE) as 借阅天数
FROM MET_CAS_LEND  l
WHERE  l.LEND_DATE BETWEEN to_date('{3}','yyyy-mm-dd HH24:mi:ss') AND to_date('{4}','yyyy-mm-dd HH24:mi:ss')
--l.LEND_DATE BETWEEN '{0}' AND '{1}'
AND (l.LEN_STUS='{2}' OR 'ALL'='{2}')
";
                try
                {
                    //查询
                    strSql = string.Format(strSql, DtBegin, dtEnd, lendState);
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
        /// 根据病案号病案借阅信息
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryUNCallBack(DateTime dtBegin, DateTime dtEnd)
        {
            string StrSql = @"SELECT patient_no,out_date  FROM DONGGUAN_CASEHISTORY_CALLBACK  WHERE IS_CALLBACK='0' and out_date BETWEEN '{0}' and '{1}'";
            StrSql = string.Format(StrSql, dtBegin, dtEnd);
            this.ExecQuery(StrSql);
            try
            {
                ArrayList list = new ArrayList();
                Neusoft.HISFC.Models.HealthRecord.Lend info = null;
                while (this.Reader.Read())
                {
                    info = new Neusoft.HISFC.Models.HealthRecord.Lend();

                    info.CardNO = this.Reader[0].ToString();//卡号
                    info.LendDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[1].ToString()); //借阅日期
                    list.Add(info);
                }
                this.Reader.Close();
                return list;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
        }


        /// <summary>
        /// 根据姓名病案借阅信息
        /// </summary>
        /// <param name="Name">姓名</param>
        /// <returns></returns>
        public ArrayList QueryLendInfoByName(string Name)
        {
            string StrSql = GetLendSql();
            if (StrSql == null)
            {
                return null;
            }
            string strSql = "WHERE NAME LIKE '{0}'";
            //if (this.Sql.GetSql("Case.CaseCard.QueryLendInfo.where", ref strSql) == -1) return null;
            StrSql += strSql;
            StrSql = string.Format(StrSql, Name);
            this.ExecQuery(StrSql);
            return QueryLendInfoBase();
        }
        /// <summary>
        /// 检查是否允许查询电子病历
        /// 允许：有借出、未回收（归档）
        /// 不允许：回收未借出
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="OperCode">操作员号</param>
        /// <returns></returns>
        public Neusoft.FrameWork.Models.NeuObject IsCheckAllowQueryEmr(string inpatientNo, string OperCode)
        {
            Neusoft.FrameWork.Models.NeuObject neuobj = new Neusoft.FrameWork.Models.NeuObject();
            int callBackCount = this.CheckCallBack(inpatientNo);
            int lendCaseCount = 0;
            if (callBackCount == 0)
            {
                neuobj = new Neusoft.FrameWork.Models.NeuObject();
                neuobj.ID = "1";
                neuobj.Memo = "未回收（归档），可以查询该患者病历！";
            }
            else if (callBackCount > 0)
            {
                lendCaseCount = this.CheckLendCase(inpatientNo, OperCode);
                if (lendCaseCount == 0)
                {
                    neuobj = new Neusoft.FrameWork.Models.NeuObject();
                    neuobj.ID = "0";
                    neuobj.Memo = "已回收（归档），无借出，不能查询该患者病历！";
                }
                else if (lendCaseCount > 0)
                {
                    neuobj = new Neusoft.FrameWork.Models.NeuObject();
                    neuobj.ID = "1";
                    neuobj.Memo = "已回收（归档），有借出，可以查询该患者病历！";
                }
                else
                {
                    neuobj = new Neusoft.FrameWork.Models.NeuObject();
                    neuobj.ID = "-1";
                    neuobj.Memo = "未找检查回收语句：Case.LendCase！";
                }
            }
            else
            {
                neuobj = new Neusoft.FrameWork.Models.NeuObject();
                neuobj.ID = "-1";
                neuobj.Memo = "未找检查回收语句：Case.CheckCallBack！";
            }
            return neuobj;
        }
        /// <summary>
        /// 检查是否回收
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <returns></returns>
        private int CheckCallBack(string inpatientNo)
        {
            string StrSql = string.Empty;
            if (this.Sql.GetSql("Case.CheckCallBack", ref StrSql) == -1) return -1;

            StrSql = string.Format(StrSql, inpatientNo);
            this.ExecQuery(StrSql);
            try
            {
                int count = 0;
                while (this.Reader.Read())
                {
                    count = Neusoft.FrameWork.Function.NConvert.ToInt32( this.Reader[0].ToString());//回收数量
                }
                this.Reader.Close();
                return count;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return 0;
            }
        }
        /// <summary>
        /// 检查是否借出
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="OperCode">操作员号</param>
        /// <returns></returns>
        public int CheckLendCase(string inpatientNo, string OperCode)
        {
            string StrSql = string.Empty;
            if (this.Sql.GetSql("Case.LendCase", ref StrSql) == -1) return -1;

            StrSql = string.Format(StrSql, inpatientNo,OperCode);
            this.ExecQuery(StrSql);
            try
            {
                int count = 0;
                while (this.Reader.Read())
                {
                    count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[0].ToString());//回收数量
                }
                this.Reader.Close();
                return count;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return 0;
            }
        }


        /// <summary>
        /// 查询病案提交信息
        /// </summary>
        /// <param name="DtBegin">提交开始日期</param>
        /// <param name="dtEnd">提交结束日期</param>
        /// <param name="Status">提交状态</param>
        /// <param name="ds">返回数据集</param>
        /// <returns></returns>
        public int QueryCaseCommitByCommitDateAndStatus(DateTime DtBegin, DateTime dtEnd, string Status, ref DataSet ds)
        {
            try
            {
                DtBegin = DtBegin.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
                dtEnd = dtEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                string strSql = "";
                if (this.Sql.GetSql("Case.CaseCard.ElectronCase.EmrQcCommit", ref strSql) == -1) return -1;
                try
                {
                    //查询
                    strSql = string.Format(strSql, DtBegin, dtEnd, Status);
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
        /// 更新电子病历提交状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int UpdateEmrQcCommit(Neusoft.HISFC.Models.HealthRecord.Lend info)
        {
            try
            {
                string strSql = "";
                if (this.Sql.GetSql("Case.CaseCard.ElectronCase.EmrQcCommit.Update", ref strSql) == -1) return -1;

                strSql = string.Format(strSql, info.ID,info.LendStus,info.LendDate,info.PrerDate,info.OperInfo.ID);

                return this.ExecNoQuery(strSql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 查询病案提交信息
        /// </summary>
        /// <param name="inpatientNo">提交开始日期</param>
        /// <param name="Status">提交状态</param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.HealthRecord.Lend> QueryCaseCommitByInpatienNoAndStatus(string inpatientNo, string Status)
        {
            try
            {
                string strSql = "";
                if (this.Sql.GetSql("Case.CaseCard.ElectronCase.EmrQcCommit.ByinpatientNo", ref strSql) == -1) return null;
                try
                {
                    //查询
                    strSql = string.Format(strSql, inpatientNo, Status);
                }
                catch (Exception ex)
                {
                    this.Err = "SQL语句赋值出错!" + ex.Message;
                    return null;
                }

                this.ExecQuery(strSql);
                try
                {
                    List<Neusoft.HISFC.Models.HealthRecord.Lend> list = new List<Neusoft.HISFC.Models.HealthRecord.Lend>();
                    Neusoft.HISFC.Models.HealthRecord.Lend info = null;
                    while (this.Reader.Read())
                    {
                        info = new Neusoft.HISFC.Models.HealthRecord.Lend();

                        info.ID = this.Reader[0].ToString();//卡号
                        info.Name = this.Reader[1].ToString();//借阅人姓名
                        info.EmployeeDept.Name = this.Reader[2].ToString(); //借阅人所在科室名称
                        info.LendDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[3].ToString()); //借阅日期
                        list.Add(info);
                    }
                    this.Reader.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return null;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }


        }
        #endregion

        #region 废弃
        [Obsolete("废弃,用 QueryLendInfo 代替")]
        public ArrayList GetLendInfo(string LendCardNo)
        {
            string StrSql = GetLendSql();
            if (StrSql == null)
            {
                return null;
            }
            string strSql = "";
            if (this.Sql.GetSql("Case.CaseCard.GetLendSql.where", ref strSql) == -1) return null;
            StrSql += strSql;
            StrSql = string.Format(StrSql, LendCardNo);
            this.ExecQuery(StrSql);
            return QueryLendInfoBase();
        }
        #endregion
    }
}
