using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace Neusoft.HISFC.BizLogic.HealthRecord.CaseHistory
{
    /// <summary>
    /// [instruction: case history callback manager]
    /// [create date: Mar.4 2010]
    /// [create by zhao.chf]
    /// [整理5.0版本 2011-8-2 chengym]
    /// </summary>
    public class CallBack : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 初始化特殊科室列表
        /// </summary>
        /// <param name="arraySpe"></param>
        public CallBack(string arraySpe,int firTimeOut ,int DeaTimeOut,int SecTimeOut ):base()
        {
            arraySpecifyDept = arraySpe;
            firstTimeOut = firTimeOut;
            secondTimeOut = SecTimeOut;
            deathTimeOut = DeaTimeOut;
        }

        public CallBack()
            : base()
        { ;}
        private string arraySpecifyDept;
        /// <summary>
        /// 特殊回收科室
        /// </summary>
        public string ArraySpecifyDept
        {
            get { return arraySpecifyDept; }
            set { arraySpecifyDept = value; }
        }
        /// <summary>
        /// 超时期限
        /// </summary>
        private int firstTimeOut = 5;
        /// <summary>
        /// 最大超时期限
        /// </summary>
        private int secondTimeOut = 8;

        private int deathTimeOut = 7;
        #region 回收主体业务
        /// <summary>
        /// 根据sql，读入查询数据到内存实体
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>实体集</returns>
        private List<Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack> GetCallBackInfo(string sql)
        {
            List<Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack> cb = new List<Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack>();
            try
            {
                if (this.ExecQuery(sql) < 0)
                {
                    return null;
                }
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack c = new Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack();
                    c.Patient.ID = this.Reader[0].ToString();//住院流水号
                    c.Patient.PID.PatientNO = this.Reader[1].ToString();//住院号
                    c.Patient.PVisit.PatientLocation.Dept.ID = this.Reader[2].ToString();//所属科室
                    c.Patient.PVisit.AdmittingDoctor.ID = this.Reader[3].ToString();//住院医生编码
                    c.Patient.PVisit.OutTime = Convert.ToDateTime(this.Reader[4].ToString());//出院日期
                    c.IsCallback = this.Reader[5].ToString();//是否回收  1 已回收 0 未回收
                    c.CallbackOper.ID = this.Reader[6].ToString();//回收人
                    if (c.IsCallback == "1")
                    {
                        c.CallbackOper.OperTime = Convert.ToDateTime(this.Reader[7].ToString());//回收日期
                    }
                    c.IsDocument = this.Reader[8].ToString();//是否归档
                    c.DocumentOper.ID = this.Reader[9].ToString();//归档人
                    if (c.IsDocument == "1")
                    {
                        c.DocumentOper.OperTime = Convert.ToDateTime(this.Reader[10].ToString());//归档日期
                    }
                    c.Patient.Name = this.Reader[11].ToString();//患者姓名
                    c.Patient.PVisit.PatientLocation.Dept.Name = this.Reader[12].ToString();//所属科室名称
                    c.Patient.PVisit.AdmittingDoctor.Name = this.Reader[13].ToString();//医生名称
                    c.CallbackOper.Name = this.Reader[14].ToString();//回收人姓名
                    c.DocumentOper.Name = this.Reader[15].ToString();//15归档人姓名
                    c.Patient.PVisit.PatientLocation.Bed.ID = this.Reader[16].ToString();//床位号
                    c.Patient.PVisit.ZG.ID = this.Reader[17].ToString();// 出院转归情况
                    cb.Add(c);
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return cb;
        }

        /// <summary>
        /// 根据出院时间范围查找病案召回数据
        /// </summary>
        /// <param name="isCallback">是否回收</param>
        /// <param name="dtBegin">开始时间</param>
        /// <param name="dtEnd">结束时间</param>
        /// <returns>回收数据实体集</returns>
        public List<Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack> QueryCaseHistoryCallBackInfo(string isCallback, DateTime dtBegin, DateTime dtEnd,string deptCode)
        {
            string sql = string.Empty;
            try
            {
                if (isCallback == "1")
                {
                    if ((this.Sql.GetSql("Case.Callback.QueryInfo.1", ref sql)) < 0)
                    {
                        return null;
                    }
                }
                else
                {
                    if ((this.Sql.GetSql("Case.Callback.QueryInfo.1.UnCallBack", ref sql)) < 0)
                    {
                        return null;
                    }
                }
                sql = string.Format(sql, isCallback, dtBegin, dtEnd, deptCode);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
            }

            return GetCallBackInfo(sql);
        }

        /// <summary>
        /// 根据回收时间查找已回收的病案信息
        /// </summary>
        /// <param name="callBackDateBegin">开始时间</param>
        /// <param name="callBackDateEnd">结束时间</param>
        /// <param name="deptCode">科室代码</param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack> QueryCaseHistoryCallBackInfo(DateTime callBackDateBegin, DateTime callBackDateEnd, string deptCode)
        {
            string sql = string.Empty;
            string where = string.Empty;

            try
            {
                if ((this.Sql.GetSql("Case.Callback.QueryInfo.2", ref sql)) < 0)
                {
                    return null;
                }
                sql = string.Format(sql, callBackDateBegin, callBackDateEnd, deptCode);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return null;
            }

            return GetCallBackInfo(sql);
        }

        /// <summary>
        /// 根据住院流水号查询病案回收表 查不找到数据则返回空 
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns>病人信息</returns>
        public List<Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack> QueryCaseHistorycallBackInfoByInpatientNO(string inpatientNO)
        {
            string sql = string.Empty;
            string where = string.Empty;

            try
            {
                if ((this.Sql.GetSql("Case.Callback.QueryInfo", ref sql)) < 0)
                {
                    return null;
                }
                if (this.Sql.GetSql("Case.Callback.QueryInfo.Where.1", ref where) < 0)
                {
                    return null;
                }
                sql += where;
                sql = string.Format(sql, inpatientNO);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return null;
            }

            return GetCallBackInfo(sql);

        }



        /// <summary>
        /// 更新病案回收信息 回收病案
        /// </summary>
        /// <param name="cb">回收实体</param>
        /// <returns>成功 非负 失败-1</returns>
        public int UpdateCaseHistoryCallBackInfo(Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack cb)
        {

            //1、 住院病历自病人出院后5天内回收（死亡病历7天内回收）
            int sevenTimeout = 0; //超过七天回收为1 未超过为0
            int tenTimeout = 0; //暂时不使用
            this.TimeOutOfCallBack(cb.Patient.PVisit.ZG.ID, cb.Patient.PVisit.OutTime, cb.CallbackOper.OperTime, cb.Patient.PVisit.PatientLocation.Dept.ID, ref sevenTimeout, ref tenTimeout);
            string sql = string.Empty;
            try
            {
                if (this.Sql.GetSql("Case.Callback.UpdateCallbackInfo.1", ref sql) < 0)
                {
                    return -1;
                }

                sql = string.Format(sql, cb.Patient.ID, cb.IsCallback, cb.CallbackOper.ID, cb.CallbackOper.OperTime, sevenTimeout, tenTimeout);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return -1;
            }

            return this.ExecNoQuery(sql);

        }

        /// <summary>
        /// 病案回收撤销
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns>成功 非负 失败-1</returns>
        public int CancelCaseHistoryCallBackInfo(string inpatientNO)
        {
            string sql = string.Empty;
            try
            {
                if (this.Sql.GetSql("Case.Callback.UpdateCallbackInfo.1", ref sql) < 0)
                {
                    return -1;
                }
                sql = string.Format(sql, inpatientNO, "0", "", "0001-01-01 00:00:00", 0, 0);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return -1;
            }

            return this.ExecNoQuery(sql);
        }

       
        #endregion

        #region 出院登记界面调用

        /// <summary>
        /// 根据住院流水号查询病案回收表 查不找到数据则返回空 
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns>病人信息</returns>
        public Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack QueryCaseHistorycallBackInfoByIninpatientNO(string inpatientNO)
        {
            string sql = string.Empty;
            string where = string.Empty;

            try
            {
                if ((this.Sql.GetSql("Case.Callback.QueryInfo", ref sql)) < 0)
                {
                    return null;
                }
                //if (this.Sql.GetSql("Case.Callback.QueryInfo.2.Where.1", ref where) < 0)
                //{
                //    return null;
                //}
                where = @"  where c.inpatient_no  = '{0}'";
                sql += where;
                sql = string.Format(sql, inpatientNO);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return null;
            }

            List<Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack> cb = GetCallBackInfo(sql);
            if (cb == null || cb.Count <= 0)
            { return null; }
            return cb[0];

        }


        /// <summary>
        /// 出院登记时插入病人信息到病案回收表
        /// </summary>
        /// <param name="cb">病案回收实体</param>
        /// <returns>成功 非负 失败-1</returns>
        public int InsertInpatientOutInfo(Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack cb)
        {
            string sql = string.Empty;
            try
            {
                if (this.Sql.GetSql("Case.Callback.InsertCallbackInfo.1", ref sql) < 0)
                {
                    return -1;
                }
                sql = string.Format(sql, cb.Patient.ID, cb.Patient.PID.PatientNO, cb.Patient.PVisit.PatientLocation.Dept.ID, cb.Patient.PVisit.AdmittingDoctor.ID, cb.Patient.PVisit.OutTime, "0");
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return -1;
            }

            return this.ExecNoQuery(sql);

        }

        /// <summary>
        /// 已经召回过的病人做出院登记 假如病案没有回收 则覆盖之前除了住院流水号之外的所有信息
        /// </summary>
        /// <param name="cb">病案收回信息表</param>
        /// <returns>成功非负 失败负</returns>
        public int UpdateInpatientOutInfo(Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack cb)
        {
            string sql = string.Empty;
            try
            {
                if (this.Sql.GetSql("Case.Callback.UpdateCallbackInfo.2", ref sql) < 0)
                {
                    return -1;
                }
                sql = string.Format(sql, cb.Patient.ID, cb.Patient.PID.PatientNO, cb.Patient.PVisit.AdmittingDoctor.ID, cb.Patient.PVisit.AdmittingDoctor.ID, cb.Patient.PVisit.OutTime, "0");
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 出院登记召回 且病人病案未回收 删除病案回收表中病人相关记录  P.S.此函数暂时不用 因为起初考虑错误
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns>成功非负 失败-1</returns>
        public int DelNotCallbackInfo(string inpatientNO)
        {
            string sql = string.Empty;
            try
            {
                if (this.Sql.GetSql("Case.Calllback.DeleteCallbackInfo.1", ref sql) < 0)
                {
                    return -1;
                }
                sql = string.Format(sql, inpatientNO);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return -1;
            }

            return this.ExecNoQuery(sql);
        }
        #endregion

        #region 报表业务
        /// <summary>
        /// 判断未回收病案是否超时 此处不用
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="outDate">出院日期</param>
        /// <param name="seven">七天</param>
        /// <param name="ten">十天</param>
        private void OutTimeJudger(string inpatientNO, DateTime outDate, out int seven, out int ten)
        {

            DateTime curDate = this.GetDateTimeFromSysDateTime();

            //判断是否超时 7天或者10天
            TimeSpan tsFrom = new TimeSpan(outDate.Ticks);
            TimeSpan tsTo = new TimeSpan(curDate.Ticks);
            TimeSpan ts = tsTo.Subtract(tsFrom).Duration();
            int daysDiff = ts.Days;
            if (daysDiff > 9)
            {
                if (daysDiff > 12)
                {
                    seven = 0;
                    ten = 1;
                }
                else
                {
                    seven = 1;
                    ten = 0;
                }
            }
            else
            {
                seven = 0;
                ten = 0;
            }
        }
        /// <summary>
        /// 病案回收率查询
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList GetCaseCallbackPercent(DateTime begin, DateTime end)
        {
            ArrayList al = new ArrayList();
            string sql = string.Empty;
            if (this.Sql.GetSql("Case.Callback.QueryInfo.CallBackPerCent", ref sql) < 0)
            {
                return null;
            }
            try
            {
                sql = string.Format(sql, begin, end);

                if (this.ExecQuery(sql) < 0)
                {
                    return null;
                }

                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack cb = new Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack();
                    cb.Patient.ID = this.Reader[0].ToString(); //科室代码
                    cb.Patient.Name = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[1]).ToString(); //总病例数
                    cb.Patient.Memo = (Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[2]) * 100).ToString("F2") + "%"; //七天回收率
                    cb.Patient.UserCode = this.Reader[3].ToString() + "(" + (Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[3]) * 10).ToString() + ")"; // 超七天病历数及罚金
                    cb.Patient.WBCode = (Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[4]) * 100).ToString("F2") + "%"; //十天回收率
                    cb.Patient.SpellCode = this.Reader[5].ToString() + "(" + (Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[5]) * 30).ToString() + ")"; // 超10天 及罚金
                    cb.Patient.PID.PatientNO = this.Reader[6].ToString();
                    cb.Patient.PVisit.PatientLocation.Dept.ID = this.Reader[7].ToString();
                    al.Add(cb);
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return al;
        }

        /// <summary>
        /// 病案回收率明细查询
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="dept_code"></param>
        /// <returns></returns>
        public ArrayList GetCaseCallbackPercentDetail(DateTime begin, DateTime end,string dept_code)
        {
            ArrayList al = new ArrayList();
            string sql = string.Empty;
            if (this.Sql.GetSql("Case.Callback.QueryInfo.CallBackPerCentDetail", ref sql) < 0)
            {
                return null;
            }
            try
            {
                sql = string.Format(sql, begin, end,dept_code);

                if (this.ExecQuery(sql) < 0)
                {
                    return null;
                }

                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack cb = new Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack();
                    cb.Patient.PID.PatientNO = this.Reader[0].ToString(); //住院号
                    cb.Patient.Name = this.Reader[1].ToString(); //姓名
                    cb.Patient.PVisit.AdmittingDoctor.Name = this.Reader[2].ToString(); //医生姓名
                    cb.Patient.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[3].ToString());//出院日期
                    cb.CallbackOper.ID = this.Reader[4].ToString(); // 回收人员
                    cb.CallbackOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[5].ToString());//回收日期
                    cb.CallbackOper.Memo = this.Reader[6].ToString(); //回收类别
                    al.Add(cb);
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return al;
        }

        /// <summary>
        /// 获取超时回收的病案明细信息
        /// </summary>
        /// <param name="deptCode"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList GetTimeOutSpecifyInfo(string deptCode, DateTime begin, DateTime end,params string[] isCallBack)
        {
            ArrayList al = new ArrayList();
            string sql = string.Empty;
            if (this.Sql.GetSql("Case.Callback.QueryInfo.OutTimeCallBack", ref sql) < 0)
            {
                return null;
            }
            try
            {
                sql = string.Format(sql, deptCode, begin, end);
                if (this.ExecQuery(sql) < 0)
                {
                    return null;
                }
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack c = new Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack();
                    c.Patient.ID = this.Reader[0].ToString();
                    c.Patient.PID.PatientNO = this.Reader[1].ToString();
                    c.Patient.PVisit.PatientLocation.Dept.ID = this.Reader[2].ToString();
                    c.Patient.PVisit.AdmittingDoctor.ID = this.Reader[3].ToString();
                    c.Patient.PVisit.OutTime = Convert.ToDateTime(this.Reader[4].ToString());
                    c.IsCallback = this.Reader[5].ToString();
                    c.CallbackOper.ID = this.Reader[6].ToString();
                    if (c.IsCallback == "1")
                    {
                        c.CallbackOper.OperTime = Convert.ToDateTime(this.Reader[7].ToString());
                    }
                    c.IsDocument = this.Reader[8].ToString();
                    c.DocumentOper.ID = this.Reader[9].ToString();
                    if (c.IsDocument == "1")
                    {
                        c.DocumentOper.OperTime = Convert.ToDateTime(this.Reader[10].ToString());
                    }
                    c.Patient.Name = this.Reader[11].ToString();
                    c.Patient.PVisit.PatientLocation.Dept.Name = this.Reader[12].ToString();
                    c.Patient.PVisit.AdmittingDoctor.Name = this.Reader[13].ToString();
                    c.CallbackOper.Name = this.Reader[14].ToString();
                    c.DocumentOper.Name = this.Reader[15].ToString();
                    c.SevenDaysTimeout = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[16]);
                    c.FourteenDaysTimeout = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[17]);
                    c.Patient.PVisit.PatientLocation.NurseCell.Name = this.Reader[18].ToString();
                    al.Add(c);
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return al;
        }

        /// <summary>
        /// 根据出院时间和科室查询回收的病案数量
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="ds">结果数据集</param>
        public void GetIsCallbackNum(string deptCode, DateTime begin, DateTime end, ref DataSet ds)
        {
//            string sql = @"SELECT  
//	                            (SELECT d.DEPT_NAME FROM COM_DEPARTMENT d WHERE d.DEPT_CODE = c.dept_code)   科室名称,
//	                            count(c.inpatient_no) 回收病历数
//                            FROM DONGGUAN_CASEHISTORY_CALLBACK c
//                            WHERE c.IS_CALLBACK = '1' 
//                            AND c.OUT_DATE BETWEEN '{0}' AND '{1}'
//                            AND (c.DEPT_CODE = '{2}' OR 'ALL' = '{2}' or '' = '{2}')
//                            GROUP BY c.DEPT_CODE";
            string sql = @"SELECT  
                        (SELECT d.DEPT_NAME FROM COM_DEPARTMENT d WHERE d.DEPT_CODE = c.dept_code)   科室名称,
                        count(c.inpatient_no) 回收病历数
                         FROM MET_CAS_CALLBACK c
                         WHERE c.IS_CALLBACK = '1' 
                        AND c.OUT_DATE BETWEEN to_date('{0}','yyyy-mm-dd HH24:mi:ss')  AND to_date('{1}','yyyy-mm-dd HH24:mi:ss') 
                        AND (c.DEPT_CODE = '{2}' OR 'ALL' = '{2}' or '' = '{2}')
                        GROUP BY c.DEPT_CODE";
            try
            {
                sql = string.Format(sql, begin, end, deptCode);
            }
            catch (System.FormatException formatEx)
            {
                this.Err = formatEx.Message.ToString();
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
            }
            catch
            {
                this.Err = "未知异常！";
            }

            this.ExecQuery(sql, ref ds);
        }



        /// <summary>
        /// 根据回收时间和科室查询回收的病案数量
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="ds">结果数据集</param>
        public void GetIsCallbackNumByCallDate(string deptCode, DateTime begin, DateTime end, ref DataSet ds)
        {
//            string sql = @"SELECT  
//	                            (SELECT d.DEPT_NAME FROM COM_DEPARTMENT d WHERE d.DEPT_CODE = c.dept_code)   科室名称,
//	                            count(c.inpatient_no) 回收病历数
//                            FROM DONGGUAN_CASEHISTORY_CALLBACK c
//                            WHERE c.IS_CALLBACK = '1' 
//                            AND c.callback_oper_date BETWEEN '{0}' AND '{1}'
//                            AND (c.DEPT_CODE = '{2}' OR 'ALL' = '{2}' or '' = '{2}')
//                            GROUP BY c.DEPT_CODE";
            string sql = @"SELECT  
                        (SELECT d.DEPT_NAME FROM COM_DEPARTMENT d WHERE d.DEPT_CODE = c.dept_code)   科室名称,
                        count(c.inpatient_no) 回收病历数
                         FROM MET_CAS_CALLBACK c
                         WHERE c.IS_CALLBACK = '1' 
                        AND c.callback_oper_date BETWEEN to_date('{0}','yyyy-mm-dd HH24:mi:ss')  AND to_date('{1}','yyyy-mm-dd HH24:mi:ss') 
                        AND (c.DEPT_CODE = '{2}' OR 'ALL' = '{2}' or '' = '{2}')
                        GROUP BY c.DEPT_CODE";
            try
            {
                sql = string.Format(sql, begin, end, deptCode);
            }
            catch (System.FormatException formatEx)
            {
                this.Err = formatEx.Message.ToString();
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
            }
            catch
            {
                this.Err = "未知异常！";
            }

            this.ExecQuery(sql, ref ds);
        }
        #endregion

        #region 修改病案回收时间
        public int UpdateCallBackDateByInpatientNO(string inpatientNO, DateTime callDate)
        {
            string sql = @"
                             UPDATE DONGGUAN_CASEHISTORY_CALLBACK 
                             SET CALLBACK_OPER_DATE = '{1}'
                             WHERE INPATIENT_NO = '{0}'
                             AND IS_CALLBACK = '1'
                        ";
            try
            {
                sql = string.Format(sql, inpatientNO, callDate);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return -1;
            }

            return this.ExecNoQuery(sql);
        }
        #endregion

        /// <summary>
        /// 时间段内包含的周六日总天数
        /// </summary>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        private int GetAllGeneralHolidaysByDateTime(DateTime dtBegin,DateTime dtEnd)
        {
            DateTime[] ret = null;

            string temp = "Saturday;Sunday";


            ArrayList al = new ArrayList();

            for (DateTime dt = dtBegin; dt <= dtEnd; dt = dt.AddDays(1))
            {
                if (temp.Contains(dt.DayOfWeek.ToString()))
                {
                    al.Add(dt);
                }
                if (dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    dt = dt.AddDays(5);
                }
            }
            int Tot = 0;
            if (al != null)
            {
                Tot = al.Count;
            }
            return Tot;
        }

        /// <summary>
        /// 根据住院流水号查询病案回收表 查不找到数据则返回空 
        /// QueryCaseHistorycallBackInfoByInpatientNO  改
        /// </summary>
        /// <param name="patientNO">住院流水号</param>
        /// <param name="Type">病案状态 1已回收 0未回收</param>
        /// <returns>病人信息</returns>
        public List<Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack> QueryCaseHistorycallBackInfoByPatientNO(string patientNO,string Type)
        {
            string sql = string.Empty;

            try
            {
                if (Type == "1")
                {
                    if (this.Sql.GetSql("Case.Callback.QueryInfo.CallBack", ref sql) < 0)
                    {
                        return null;
                    }
                    sql = string.Format(sql, patientNO,patientNO.TrimStart('0'));
                }
                else
                {
                    if (this.Sql.GetSql("Case.Callback.QueryInfo.UnCallBack", ref sql) < 0)
                    {
                        return null;
                    }
                    sql = string.Format(sql, patientNO);
                }

            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return null;
            }

            return GetCallBackInfo(sql);

        }


        /// <summary>
        /// 插入回收信息
        /// </summary>
        /// <param name="cb">回收实体</param>
        /// <returns>成功 非负 失败-1</returns>
        public int InsertCaseHistoryCallBackInfo(Neusoft.HISFC.Models.HealthRecord.CaseHistory.CallBack cb)
        {
            //1、 住院病历自病人出院后5天内回收（死亡病历7天内回收）
            string sql = string.Empty;
            int sevenTimeout = 0;
            int tenTimeout = 0;
            if (cb == null)
            {
                return -1;
            }

            DateTime dtOutDate = cb.Patient.PVisit.OutTime;
            DateTime dtOperDate = cb.CallbackOper.OperTime;
            this.TimeOutOfCallBack(cb.Patient.PVisit.ZG.ID, dtOutDate, dtOperDate, cb.Patient.PVisit.PatientLocation.Dept.ID, ref sevenTimeout, ref tenTimeout);
            //TimeSpan tsFrom = new TimeSpan(dtOutDate.Ticks);
            //TimeSpan tsTo = new TimeSpan(dtOperDate.Ticks);
            //TimeSpan ts = tsTo.Subtract(tsFrom).Duration();
            //int daysDiff = ts.Days;

            ////int SunSatDay = this.GetAllGeneralHolidaysByDateTime(dtOutDate, dtOperDate);//周六日天数

            ////daysDiff = daysDiff - SunSatDay;

            //if (cb.Patient.PVisit.ZG.ID == "4")//死亡情况
            //{
            //    if (daysDiff > 7)
            //    {
            //        sevenTimeout = 1;
            //        tenTimeout = 0;
            //    }
            //    else
            //    {
            //        sevenTimeout = 0;
            //        tenTimeout = 0;
            //    }
            //}
            //else
            //{
            //    if (daysDiff > firstTimeOut)
            //    {
            //        sevenTimeout = 1;
            //        tenTimeout = 0;
            //    }
            //    else
            //    {
            //        sevenTimeout = 0;
            //        tenTimeout = 0;
            //    }
            //}
            try
            {
                if (this.Sql.GetSql("Case.Callback.InsertCallbackInfo", ref sql) < 0)
                {
                    return -1;
                }

                sql = string.Format(sql, cb.Patient.ID,cb.Patient.PVisit.PatientLocation.Dept.ID,cb.Patient.PVisit.AdmittingDoctor.ID,
                                         cb.Patient.PVisit.OutTime,cb.IsCallback, cb.CallbackOper.ID, cb.CallbackOper.OperTime, 
                                         cb.IsDocument,cb.DocumentOper.ID,cb.DocumentOper.OperTime,cb.Patient.PID.PatientNO,sevenTimeout,
                                         tenTimeout);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message.ToString();
                return -1;
            }

            return this.ExecNoQuery(sql);

        }

        /// <summary>
        /// 计算超时
        /// </summary>
        /// <param name="ZgType">转归类型</param>
        /// <param name="OutDate">出院日期</param>
        /// <param name="CallBackDate">回收日期</param>
        /// <param name="deptCode">所属科室--用于判断特殊科室</param>
        /// <param name="sevenTimeout">超时 7天</param>
        /// <param name="tenTimeout">超时 10天 暂不用</param>
        private void TimeOutOfCallBack(string ZgType, DateTime OutDate, DateTime CallBackDate, string deptCode, ref int sevenTimeout, ref int tenTimeout)
        {
            //1、 住院病历自病人出院后5天内回收（死亡病历7天内回收）
            string sql = string.Empty;

            string[] dept = arraySpecifyDept.Split(',');

            DateTime dtOutDate = OutDate;
            DateTime dtOperDate = CallBackDate;

            TimeSpan tsFrom = new TimeSpan(dtOutDate.Ticks);
            TimeSpan tsTo = new TimeSpan(dtOperDate.Ticks);
            TimeSpan ts = tsTo.Subtract(tsFrom).Duration();
            int daysDiff = ts.Days;

            //int SunSatDay = this.GetAllGeneralHolidaysByDateTime(dtOutDate, dtOperDate);//周六日天数

            //daysDiff = daysDiff - SunSatDay;

            if (dept != null && dept.Length > 0 && dept[0]!="") //特殊科室 
            {
                foreach (string str in dept)
                {
                    if (deptCode == str)
                    {
                        if (daysDiff > 14)
                        {
                            sevenTimeout = 0;
                            tenTimeout = 1;
                        }
                        else
                        {
                            sevenTimeout = 0;
                            tenTimeout = 0;
                        }
                    }
                }
            }
            else//非特殊科室
            {
                if (ZgType == "4")//死亡情况
                {
                    if (daysDiff > deathTimeOut && daysDiff<=secondTimeOut)
                    {
                        sevenTimeout = 1;
                        tenTimeout = 0;
                    }
                    else if (daysDiff > secondTimeOut)
                    {
                        sevenTimeout = 0;
                        tenTimeout = 1;
                    }
                    else
                    {
                        sevenTimeout = 0;
                        tenTimeout = 0;
                    }
                }
                else
                {
                    if (daysDiff > firstTimeOut && daysDiff <= secondTimeOut)
                    {
                        sevenTimeout = 1;
                        tenTimeout = 0;
                    }
                    else if (daysDiff > secondTimeOut)
                    {
                        sevenTimeout = 0;
                        tenTimeout = 1;
                    }
                    else
                    {
                        sevenTimeout = 0;
                        tenTimeout = 0;
                    }
                }
            }
        }
    }
}
