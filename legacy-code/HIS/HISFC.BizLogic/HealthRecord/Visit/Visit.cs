using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.HealthRecord;
using Neusoft.FrameWork.Function;
using System.Data;
using System.Collections;
using Neusoft.HISFC.Models.HealthRecord.EnumServer;
using Neusoft.HISFC.Models.HealthRecord.Case;

namespace Neusoft.HISFC.BizLogic.HealthRecord.Visit
{
    /// <summary>
    /// Visit<br></br>
    /// [功能描述: 随访主记录基本业务层]<br></br>
    /// [创 建 者: 王立]<br></br>
    /// [创建时间: 2007-08-21]<br></br>
    /// <修改记录
    ///		修改人=金鹤
    ///		修改时间='2009-09-08'
    ///		修改目的='完善随访功能'
    ///		修改描述=''
    ///  />
    /// </summary>
    public class Visit : Neusoft.FrameWork.Management.Database
    {
        #region 数据库基本操作

        /// <summary>
        /// 插入随访主记录
        /// </summary>
        /// <param name="visit">随访主记录类</param>
        /// <returns>影响的行数、-1 失败</returns>
        public int Insert(Neusoft.HISFC.Models.HealthRecord.Visit.Visit visit)
        {
            string strSQL = "";

            if(this.Sql.GetSql("HealthReacord.Visit.Vist.Insert", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.Vist.Insert字段！";
                return -1;
            }

            try
            {
                string[] strParm = GetVisitParmItem(visit);
                strSQL = string.Format(strSQL, strParm);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return -1;
            }

            //　执行SQL并返回
            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 更新随访主记录
        /// </summary>
        /// <param name="visit">随访主记录类</param>
        /// <returns>影响的行数；-1－失败</returns>
        public int Update(Neusoft.HISFC.Models.HealthRecord.Visit.Visit visit)
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.Vist.Update", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.Vist.Update字段！";
                return -1;
            }
            try
            {
                string[] strParm = GetVisitParmItem(visit);
                strSQL = string.Format(strSQL, strParm);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return -1;
            }

            //　执行SQL语句返回
            return this.ExecNoQuery(strSQL);
        }

        /// <summary>
        /// 将某个患者的随访状态设为停止随访
        /// </summary>
        /// <param name="cardNo">病历号</param>
        /// <returns>1 成功；-1－失败</returns>
        public int UpdateStat(string cardNo)
        {
            string strSQL = "";

            if (this.Sql.GetSql("HealthReacord.Visit.Vist.UpdateStat", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.Vist.UpdateStat字段！";
                return -1;
            }
            try
            {
                strSQL = string.Format(strSQL, cardNo);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时候出错！" + ex.Message;
                return -1;
            }

            //　执行SQL语句
            if (this.ExecNoQuery(strSQL) != 1)
            {
                this.Err = "更新的不是一条记录!";

                return -1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 获得update或者insert随访表的传入参数数组
        /// </summary>
        /// <param name="company">随访主记录信息</param>
        /// <returns>参数数组</returns>
        private string[] GetVisitParmItem(Neusoft.HISFC.Models.HealthRecord.Visit.Visit visit)
        {
            string[] strParm = new string[16];

            strParm[0] = visit.Patient.PID.CardNO;
            strParm[1] = visit.Linkway.Address;
            strParm[2] = visit.Linkway.Mail;
            strParm[3] = visit.Linkway.Phone;
            strParm[4] = visit.LastVisitTime.ToString();
            strParm[5] = visit.Linkway.LinkWayType.ID;
            strParm[6] = visit.Linkway.ZIP;
            if (visit.VisitState == Neusoft.HISFC.Models.HealthRecord.Visit.EnumVisitState.Normal)
            {
                strParm[7] = "1";
            }
            else
            {
                strParm[7] = "0";
            }
            if (visit.LastIsPassive)
            {
                strParm[8] = "1";
            }
            else
            {
                strParm[8] = "0";
            }
            strParm[9] = visit.Linkway.OtherLinkway;
            strParm[10] = visit.Linkway.LinkMan.Name;
            if (visit.Linkway.IsLinkMan)
            {
                strParm[11] = "1";
            }
            else
            {
                strParm[11] = "0";
            }
            strParm[12] = visit.Linkway.Relation.ID;
            strParm[13] = visit.User01;
            strParm[14] = visit.User02;
            strParm[15] = visit.User03;

            //返回数组
            return strParm;             
        }

        #endregion

        #region 查询

        /// <summary>
        ///　根据病历号获取患者的随访主记录
        /// </summary>
        /// <param name="visit">随访主记录类</param>
        /// <param name="cardNo">患者病历号</param>
        /// <returns>1-成功、0-没有查询到结果、-1-失败</returns>
        public int Select(ref Neusoft.HISFC.Models.HealthRecord.Visit.Visit visit, string cardNo)
        {
            string strSQL = "";

            //读取SQL语句
            if (this.Sql.GetSql("HealthReacord.Visit.Visit.Select", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.Visit.Select字段！";
                return -1;
            }
            try
            {
                //传递病历号参数
                strSQL = string.Format(strSQL, cardNo);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;
                return -1;
            }

            ArrayList alVisit = new ArrayList();

            this.ExecQuery(strSQL);

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.HealthRecord.Visit.Visit visitTemp = new Neusoft.HISFC.Models.HealthRecord.Visit.Visit();

                    visitTemp.Patient.PID.CardNO = this.Reader[0].ToString();
                    visitTemp.Linkway.Address = this.Reader[1].ToString();
                    visitTemp.Linkway.Mail = this.Reader[2].ToString();
                    visitTemp.Linkway.Phone = this.Reader[3].ToString();
                    visitTemp.LastVisitTime = NConvert.ToDateTime(this.Reader[4].ToString());
                    visitTemp.Linkway.LinkWayType.ID = this.Reader[5].ToString();
                    visitTemp.Linkway.ZIP = this.Reader[6].ToString();
                    //随访状态
                    if (this.Reader[7].ToString().Equals("0"))
                    {
                        visitTemp.VisitState = Neusoft.HISFC.Models.HealthRecord.Visit.EnumVisitState.Stop;
                    }
                    else
                    {
                        visitTemp.VisitState = Neusoft.HISFC.Models.HealthRecord.Visit.EnumVisitState.Normal;
                    }
                    if (this.Reader[8].ToString().Equals("1"))
                    {
                        visitTemp.LastIsPassive = true;
                    }
                    else
                    {
                        visitTemp.LastIsPassive = false;
                    }
                    visitTemp.Linkway.OtherLinkway = this.Reader[9].ToString();
                    visitTemp.Linkway.LinkMan.Name = this.Reader[10].ToString();
                    if (this.Reader[11].ToString().Equals("1"))
                    {
                        visitTemp.Linkway.IsLinkMan = true;
                    }
                    else
                    {
                        visitTemp.Linkway.IsLinkMan = false;
                    }
                    visitTemp.Linkway.Relation.ID = this.Reader[12].ToString();
                    visitTemp.User01 = this.Reader[13].ToString();
                    visitTemp.User02 = this.Reader[14].ToString();
                    visitTemp.User03 = this.Reader[15].ToString();
                    visitTemp.Linkway.LinkWayType.Name = this.Reader[16].ToString();
                    visitTemp.Linkway.Relation.Name = this.Reader[17].ToString();

                    alVisit.Add(visitTemp);
                }
            }
            catch (Exception ex)
            {
                this.Err = "读取随访主记录出错！" + ex.Message;
                return -1;
            }
            finally
            {
                this.Reader.Close();
            }

            if (alVisit.Count == 0)
            {
                return 0;
            }
            else if (alVisit.Count == 1)
            {
                visit = alVisit[0] as Neusoft.HISFC.Models.HealthRecord.Visit.Visit;

                return 1;
            }
            else
            {
                this.Err = "存在多条记录！";

                return -1;
            }
        }

        /// <summary>
        /// 传入病历号判断该患者是否已经停止随访
        /// </summary>
        /// <param name="cardNo">病历号</param>
        /// <returns>-1 失败、0 停止随访、1 正常随访</returns>
        public int IsVisitStop(string cardNo)
        {
            Neusoft.HISFC.Models.HealthRecord.Visit.Visit visit = new Neusoft.HISFC.Models.HealthRecord.Visit.Visit();

            int intReturn = this.Select(ref visit, cardNo);
            if (intReturn == -1 || intReturn == 0)
            {
                return -1;
            }

            if (visit.VisitState == Neusoft.HISFC.Models.HealthRecord.Visit.EnumVisitState.Stop)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        #endregion

        #region {E9F858A6-BDBC-4052-BA57-68755055FB80}

        /// <summary>
        /// 查询回访ICD列表
        /// </summary>
        /// <param name="ICDType">诊断类型枚举</param>
        /// <param name="ds">符合条件的数据集</param>
        /// <returns>出现未知错误 返回 -1 成功返回 1</returns>
        public int QueryVisitICD(ICDTypes ICDType, ref DataSet ds)
        {
            //定义字符变量 ,存储查询主体SQL语句
            string strQuerySql = "";
            //定义字符变量, 存储查询条件
            try
            {
                switch (ICDType)
                {
                    case ICDTypes.ICD10:
                        //获取查询SQL语句
                        if (this.Sql.GetSql("HealthReacord.Visit.Query.ICD10", ref strQuerySql) == -1)
                        {
                            this.Err = "获取SQL语句失败,索引:HealthReacord.Visit.Query.ICD10";
                            return -1;
                        }
                        break;
                    case ICDTypes.ICD9:
                        //获取查询SQL语句
                        if (this.Sql.GetSql("HealthReacord.Visit.Query.ICD9", ref strQuerySql) == -1)
                        {
                            this.Err = "获取SQL语句失败,索引:HealthReacord.Visit.Query.ICD9";
                            return -1;
                        }
                        break;
                    case ICDTypes.ICDOperation:
                        //获取查询SQL语句
                        if (this.Sql.GetSql("HealthReacord.Visit.Query.ICDoperation", ref strQuerySql) == -1)
                        {
                            this.Err = "获取SQL语句失败, 索引:HealthReacord.Visit.Query.ICDoperation";
                            return -1;
                        }
                        break;
                }

                //执行查询操作
                return this.ExecQuery(strQuerySql, ref ds);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message; //获取错误信息
                return -1; //产生未处理的错误
            }
        }

        /// <summary>
        /// 增加随访ICD范围
        /// </summary>
        /// <param name="begin">开始ICD编码</param>
        /// <param name="end">结束ICD编码</param>
        /// <returns>成功返回 0 ; 失败返回 -1</returns>
        public int InsertVisitICD(string begin,string end)
        {

            string strSql = string.Empty;

            if (begin != end)
            {
                string headStr = begin.Substring(0, 1).ToUpper();

                string beginInt = begin.Substring(1, begin.IndexOf('.') - 1) + begin.Substring(begin.IndexOf('.') + 1, 3);

                string endInt = end.Substring(1, end.IndexOf('.') - 1) + end.Substring(end.IndexOf('.') + 1, 3);

                if (this.Sql.GetSql("HealthReacord.Visit.Insert.VISITICD10", ref strSql) == -1)
                {
                    this.Err = "没有找到HealthReacord.Visit.Insert.VISITICD10字段！";
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, headStr, beginInt, endInt, begin + "-" + end,
                        this.Operator.ID, this.GetSysDateTime());
                }
                catch (Exception ex)
                {
                    this.Err = "赋值时出错！" + ex.Message;
                    return -1;
                }
            }
            else
            {
                if (this.Sql.GetSql("HealthReacord.Visit.Insert.VISITONEICD10", ref strSql) == -1)
                {
                    this.Err = "没有找到HealthReacord.Visit.Insert.VISITONEICD10字段！";
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql,begin, begin,
                        this.Operator.ID, this.GetSysDateTime());
                }
                catch (Exception ex)
                {
                    this.Err = "赋值时出错！" + ex.Message;
                    return -1;
                }
            }


            return this.ExecQuery(strSql);

        }

        /// <summary>
        /// 删除随访ICD
        /// </summary>
        /// <param name="icdNo">icd流水号</param>
        /// <returns>成功返回 0 ; 失败返回 -1</returns>
        public int DelVisitICD(string icdNo)
        {
            string strSql = "";

            if (this.Sql.GetSql("HealthReacord.Visit.Delete.VISITICD10", ref strSql) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.Delete.VISITICD10字段！";
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, icdNo);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;
                return -1;
            }


            return this.ExecQuery(strSql);
        }

        

        #endregion

        #region wubiqiu 优化性能
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardNolist"></param>
        /// <param name="isNeedTemTable"></param>
        /// <returns></returns>
        public List<string> QueryByCardNoList(string cardNolist, Boolean isNeedTemTable)
        {
            string strSQL = "";
            string strSQL1 = "";

            //读取SQL语句
            if (this.Sql.GetSql("HealthReacord.Visit.VisitRecord.Select1List", ref strSQL) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.VisitRecord.Select字段！";
                return null;
            }
            if (isNeedTemTable)
            {
                //读取SQL语句
                if (this.Sql.GetSql("HealthReacord.Visit.VisitRecord.WhereByCardNoListWithTempTable", ref strSQL1) == -1)
                {
                    this.Err = "没有找到HealthReacord.Visit.VisitRecord.WhereByCardNoListWithTempTable字段！";
                    return null;
                }

            }
            //读取SQL语句
            else if (this.Sql.GetSql("HealthReacord.Visit.VisitRecord.WhereByCardNoList", ref strSQL1) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.VisitRecord.WhereByCardNo字段！";
                return null;
            }

            try
            {
                if (!isNeedTemTable)
                {
                    strSQL1 = string.Format(strSQL1, cardNolist);
                }
                //else
                //{
                //    strSQL1 = string.Format(strSQL1, "select id from SUIFANG");
                //}
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;
                return null;
            }
           
            strSQL = strSQL + "\n" + strSQL1;
            string sqlInsertTem = @"insert into SUIFANG{0}";
            if (isNeedTemTable)
            {
                this.ExecNoQuery("delete from SUIFANG");
                
                sqlInsertTem = string.Format(sqlInsertTem, cardNolist);
                if (this.ExecNoQuery(sqlInsertTem) == -1)
                {
                    this.Err = "执行SQL语句出错！" + this.Err;
                    this.ErrCode = "-1";
                    return null;
                }
            }
            if (this.ExecQuery(strSQL) == -1)
            {
                this.Err = "执行SQL语句出错！" + this.Err;
                this.ErrCode = "-1";
                return null;
            }

            List<string> list = new List<string>();

            try
            {
                while (this.Reader.Read())
                {
                    list.Add(this.Reader[0].ToString());
                }

                return list;
            }
            catch (System.Exception ex)
            {
                this.Err = "获得随访信息出错！" + ex.Message;
                this.ErrCode = "-1";
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }
        #endregion

        #region 获取到上次住院手术的问题
        /// <summary>
        /// 获取当次住院手术名称
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public string GetOperaByPatientNoAndInDate(string cardNo)
        {
            string operName = string.Empty;
            string strSql = @"SELECT t.operation_cnname
  FROM MET_CAS_OPERATIONDETAIL t
 WHERE t.inpatient_no in (select a.inpatient_no
                            from fin_ipr_inmaininfo a
                           where a.card_no = '{0}')";
            try
            {
                strSql = string.Format(strSql, cardNo);
            }
            catch
            {
                this.Err = "格式化字符串出错";
                return string.Empty;
            }
            //执行SQL语句
            this.ExecQuery(strSql);

            try
            {
                while (this.Reader.Read())
                {
                    operName += this.Reader[0].ToString();
                    operName += ",";
                }

                //去掉字符串最后一个“,”
                operName = operName.Substring(0, (operName.Length - 1));
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return operName;
        }
        #endregion

        #region 保存之后不显示的问题
        /// <summary>
        /// 根据病历号获取基本信息
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Visit.Base GetCaseBaseInfo(string cardNo)
        {
            string strSQL = @"SELECT 
                                 t.INPATIENT_NO  AS INPATIENT_NO    -- VARCHAR2  #0
                                ,t.PATIENT_NO  AS PATIENT_NO    -- VARCHAR2  #1
                                ,t.CARD_NO  AS CARD_NO    -- VARCHAR2  #2
                                ,t.NAME  AS NAME    -- VARCHAR2  #3
                                ,t.SEX_CODE  AS SEX_CODE    -- VARCHAR2  #4
                                ,t.IDENNO  AS IDENNO    -- VARCHAR2  #5
                                ,t.BIRTHDAY  AS BIRTHDAY    -- DATE  #6
                                ,t.PROF_CODE  AS PROF_CODE    -- VARCHAR2  #7
                                ,t.WORK_NAME  AS WORK_NAME    -- VARCHAR2  #8
                                ,t.WORK_TEL  AS WORK_TEL    -- VARCHAR2  #9
                                ,t.HOME  AS HOME    -- VARCHAR2  #10
                                ,t.HOME_TEL  AS HOME_TEL    -- VARCHAR2  #11
                                ,t.MARI  AS MARI    -- VARCHAR2  #12
                                ,t.IN_DATE  AS IN_DATE    -- DATE  #13
                                ,t.DEPT_CODE  AS DEPT_CODE    -- VARCHAR2  #14
                                ,t.DEPT_NAME  AS DEPT_NAME    -- VARCHAR2  #15
                                ,t.BED_NO  AS BED_NO    -- VARCHAR2  #16
                                ,t.NURSE_CELL_CODE  AS NURSE_CELL_CODE    -- VARCHAR2  #17
                                ,t.NURSE_CELL_NAME  AS NURSE_CELL_NAME    -- VARCHAR2  #18
                                ,t.HOUSE_DOC_CODE  AS HOUSE_DOC_CODE    -- VARCHAR2  #19
                                ,t.HOUSE_DOC_NAME  AS HOUSE_DOC_NAME    -- VARCHAR2  #20
                                ,t.CHARGE_DOC_CODE  AS CHARGE_DOC_CODE    -- VARCHAR2  #21
                                ,t.CHARGE_DOC_NAME  AS CHARGE_DOC_NAME    -- VARCHAR2  #22
                                ,t.CHIEF_DOC_CODE  AS CHIEF_DOC_CODE    -- VARCHAR2  #23
                                ,t.CHIEF_DOC_NAME  AS CHIEF_DOC_NAME    -- VARCHAR2  #24
                                ,t.DUTY_NURSE_CODE  AS DUTY_NURSE_CODE    -- VARCHAR2  #25
                                ,t.DUTY_NURSE_NAME  AS DUTY_NURSE_NAME    -- VARCHAR2  #26
                                ,t.IN_CIRCS  AS IN_CIRCS    -- VARCHAR2  #27
                                ,t.IN_AVENUE  AS IN_AVENUE    -- VARCHAR2  #28
                                ,t.IN_SOURCE  AS IN_SOURCE    -- VARCHAR2  #29
                                ,t.IN_TIMES  AS IN_TIMES    -- NUMBER  #30
                                ,t.IN_STATE  AS IN_STATE    -- VARCHAR2  #31
                                ,t.OUT_DATE  AS OUT_DATE    -- DATE  #32
                                ,t.ZG  AS ZG    -- VARCHAR2  #33
                                ,t.IN_ICU  AS IN_ICU    -- VARCHAR2  #34
                                ,t.TEND  AS TEND    -- VARCHAR2  #35
                                ,t.CRITICAL_FLAG  AS CRITICAL_FLAG    -- VARCHAR2  #36
                                ,t.DIAG_NAME  AS DIAG_NAME    -- VARCHAR2  #37
                            From VIEW_VISIT_PATIENTINFO t";
            string sqlWhere = @" where card_no = '{0}' ORDER BY in_date desc";
            strSQL += sqlWhere;
            strSQL = string.Format(strSQL, cardNo);

            //执行操查询操作
            this.ExecQuery(strSQL);

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Visit.Base det = new Neusoft.HISFC.Models.Visit.Base();

                    //INPATIENT_NO  VARCHAR2 #0
                    det.InPatientNO = this.Reader[0].ToString();
                    //PATIENT_NO  VARCHAR2 #1
                    det.PatientNO = this.Reader[1].ToString();
                    //CARD_NO  VARCHAR2 #2
                    det.CardNO = this.Reader[2].ToString();
                    //NAME  VARCHAR2 #3
                    det.Name = this.Reader[3].ToString();
                    //SEX_CODE  VARCHAR2 #4
                    det.SexName = this.Reader[4].ToString();
                    //IDENNO  VARCHAR2 #5
                    det.IdenNO = this.Reader[5].ToString();
                    //BIRTHDAY  DATE #6
                    det.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6].ToString());
                    //PROF_CODE  VARCHAR2 #7
                    det.Profession = this.Reader[7].ToString();
                    //WORK_NAME  VARCHAR2 #8
                    det.WorkName = this.Reader[8].ToString();
                    //WORK_TEL  VARCHAR2 #9
                    det.WorkTel = this.Reader[9].ToString();
                    //HOME  VARCHAR2 #10
                    det.HomeAddress = this.Reader[10].ToString();
                    //HOME_TEL  VARCHAR2 #11
                    det.HomeTel = this.Reader[11].ToString();
                    //MARI  VARCHAR2 #12
                    det.Marriage = this.Reader[12].ToString();
                    //IN_DATE  DATE #13
                    det.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[13].ToString());
                    //DEPT_CODE  VARCHAR2 #14
                    det.Dept.ID = this.Reader[14].ToString();
                    //DEPT_NAME  VARCHAR2 #15
                    det.Dept.Name = this.Reader[15].ToString();
                    //BED_NO  VARCHAR2 #16
                    det.BedNo = this.Reader[16].ToString();
                    //NURSE_CELL_CODE  VARCHAR2 #17
                    det.NurseCellDept.ID = this.Reader[17].ToString();
                    //NURSE_CELL_NAME  VARCHAR2 #18
                    det.NurseCellDept.Name = this.Reader[18].ToString();
                    //HOUSE_DOC_CODE  VARCHAR2 #19
                    det.HouseDoctor.ID = this.Reader[19].ToString();
                    //HOUSE_DOC_NAME  VARCHAR2 #20
                    det.HouseDoctor.Name = this.Reader[20].ToString();
                    //CHARGE_DOC_CODE  VARCHAR2 #21
                    det.ChargeDoctor.ID = this.Reader[21].ToString();
                    //CHARGE_DOC_NAME  VARCHAR2 #22
                    det.ChargeDoctor.Name = this.Reader[22].ToString();
                    //CHIEF_DOC_CODE  VARCHAR2 #23
                    det.ChiefDoctor.ID = this.Reader[23].ToString();
                    //CHIEF_DOC_NAME  VARCHAR2 #24
                    det.ChiefDoctor.Name = this.Reader[24].ToString();
                    //DUTY_NURSE_CODE  VARCHAR2 #25
                    det.DutyNurse.ID = this.Reader[25].ToString();
                    //DUTY_NURSE_NAME  VARCHAR2 #26
                    det.DutyNurse.Name = this.Reader[26].ToString();
                    //IN_CIRCS  VARCHAR2 #27
                    det.InCircs = this.Reader[27].ToString();
                    //IN_AVENUE  VARCHAR2 #28
                    det.InAvenue = this.Reader[28].ToString();
                    //IN_SOURCE  VARCHAR2 #29
                    det.InSource = this.Reader[29].ToString();
                    //IN_TIMES  NUMBER #30
                    det.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[30].ToString());
                    //IN_STATE  VARCHAR2 #31
                    //det.InState = this.Reader[31] == null ? Neusoft.HISFC.Models.Visit.EnumInState.R : this.Reader[31] as Neusoft.HISFC.Models.Visit.EnumInState;

                    det.InState = this.GetEnumInState(this.Reader[31].ToString());

                    //OUT_DATE  DATE #32
                    det.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[32].ToString());
                    //ZG  VARCHAR2 #33
                    det.Zg = this.Reader[33].ToString();
                    //IN_ICU  VARCHAR2 #34
                    det.InIcu = this.Reader[34].ToString();
                    //TEND  VARCHAR2 #35
                    det.Tend = this.Reader[35].ToString();
                    //CRITICAL_FLAG  VARCHAR2 #36
                    det.CriticalFlag = this.Reader[36].ToString();
                    //DIAG_NAME  VARCHAR2 #37
                    det.DiagName = this.Reader[37].ToString();

                    return det;
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得患者病案信息出错!" + ex.Message;
                return null;
            }
            return null;
        }



        //随访修改(添加新方法)
        /// <summary>
        /// 根据住院流水号获取基本信息
        /// </summary>
        /// <param name="INPATIENT_NO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Visit.Base GetCaseBaseInfoByInpatientNO(string INPATIENT_NO)
        {
            string strSQL = @"SELECT 
                                 t.INPATIENT_NO  AS INPATIENT_NO    -- VARCHAR2  #0
                                ,t.PATIENT_NO  AS PATIENT_NO    -- VARCHAR2  #1
                                ,t.CARD_NO  AS CARD_NO    -- VARCHAR2  #2
                                ,t.NAME  AS NAME    -- VARCHAR2  #3
                                ,t.SEX_CODE  AS SEX_CODE    -- VARCHAR2  #4
                                ,t.IDENNO  AS IDENNO    -- VARCHAR2  #5
                                ,t.BIRTHDAY  AS BIRTHDAY    -- DATE  #6
                                ,t.PROF_CODE  AS PROF_CODE    -- VARCHAR2  #7
                                ,t.WORK_NAME  AS WORK_NAME    -- VARCHAR2  #8
                                ,t.WORK_TEL  AS WORK_TEL    -- VARCHAR2  #9
                                ,t.HOME  AS HOME    -- VARCHAR2  #10
                                ,t.HOME_TEL  AS HOME_TEL    -- VARCHAR2  #11
                                ,t.MARI  AS MARI    -- VARCHAR2  #12
                                ,t.IN_DATE  AS IN_DATE    -- DATE  #13
                                ,t.DEPT_CODE  AS DEPT_CODE    -- VARCHAR2  #14
                                ,t.DEPT_NAME  AS DEPT_NAME    -- VARCHAR2  #15
                                ,t.BED_NO  AS BED_NO    -- VARCHAR2  #16
                                ,t.NURSE_CELL_CODE  AS NURSE_CELL_CODE    -- VARCHAR2  #17
                                ,t.NURSE_CELL_NAME  AS NURSE_CELL_NAME    -- VARCHAR2  #18
                                ,t.HOUSE_DOC_CODE  AS HOUSE_DOC_CODE    -- VARCHAR2  #19
                                ,t.HOUSE_DOC_NAME  AS HOUSE_DOC_NAME    -- VARCHAR2  #20
                                ,t.CHARGE_DOC_CODE  AS CHARGE_DOC_CODE    -- VARCHAR2  #21
                                ,t.CHARGE_DOC_NAME  AS CHARGE_DOC_NAME    -- VARCHAR2  #22
                                ,t.CHIEF_DOC_CODE  AS CHIEF_DOC_CODE    -- VARCHAR2  #23
                                ,t.CHIEF_DOC_NAME  AS CHIEF_DOC_NAME    -- VARCHAR2  #24
                                ,t.DUTY_NURSE_CODE  AS DUTY_NURSE_CODE    -- VARCHAR2  #25
                                ,t.DUTY_NURSE_NAME  AS DUTY_NURSE_NAME    -- VARCHAR2  #26
                                ,t.IN_CIRCS  AS IN_CIRCS    -- VARCHAR2  #27
                                ,t.IN_AVENUE  AS IN_AVENUE    -- VARCHAR2  #28
                                ,t.IN_SOURCE  AS IN_SOURCE    -- VARCHAR2  #29
                                ,t.IN_TIMES  AS IN_TIMES    -- NUMBER  #30
                                ,t.IN_STATE  AS IN_STATE    -- VARCHAR2  #31
                                ,t.OUT_DATE  AS OUT_DATE    -- DATE  #32
                                ,t.ZG  AS ZG    -- VARCHAR2  #33
                                ,t.IN_ICU  AS IN_ICU    -- VARCHAR2  #34
                                ,t.TEND  AS TEND    -- VARCHAR2  #35
                                ,t.CRITICAL_FLAG  AS CRITICAL_FLAG    -- VARCHAR2  #36
                                ,t.DIAG_NAME  AS DIAG_NAME    -- VARCHAR2  #37
                            From VIEW_VISIT_PATIENTINFO t";
            string sqlWhere = @" where INPATIENT_NO = '{0}'";
            strSQL += sqlWhere;
            strSQL = string.Format(strSQL, INPATIENT_NO);

            //执行操查询操作
            this.ExecQuery(strSQL);

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Visit.Base det = new Neusoft.HISFC.Models.Visit.Base();

                    //INPATIENT_NO  VARCHAR2 #0
                    det.InPatientNO = this.Reader[0].ToString();
                    //PATIENT_NO  VARCHAR2 #1
                    det.PatientNO = this.Reader[1].ToString();
                    //CARD_NO  VARCHAR2 #2
                    det.CardNO = this.Reader[2].ToString();
                    //NAME  VARCHAR2 #3
                    det.Name = this.Reader[3].ToString();
                    //SEX_CODE  VARCHAR2 #4
                    det.SexName = this.Reader[4].ToString();
                    //IDENNO  VARCHAR2 #5
                    det.IdenNO = this.Reader[5].ToString();
                    //BIRTHDAY  DATE #6
                    det.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6].ToString());
                    //PROF_CODE  VARCHAR2 #7
                    det.Profession = this.Reader[7].ToString();
                    //WORK_NAME  VARCHAR2 #8
                    det.WorkName = this.Reader[8].ToString();
                    //WORK_TEL  VARCHAR2 #9
                    det.WorkTel = this.Reader[9].ToString();
                    //HOME  VARCHAR2 #10
                    det.HomeAddress = this.Reader[10].ToString();
                    //HOME_TEL  VARCHAR2 #11
                    det.HomeTel = this.Reader[11].ToString();
                    //MARI  VARCHAR2 #12
                    det.Marriage = this.Reader[12].ToString();
                    //IN_DATE  DATE #13
                    det.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[13].ToString());
                    //DEPT_CODE  VARCHAR2 #14
                    det.Dept.ID = this.Reader[14].ToString();
                    //DEPT_NAME  VARCHAR2 #15
                    det.Dept.Name = this.Reader[15].ToString();
                    //BED_NO  VARCHAR2 #16
                    det.BedNo = this.Reader[16].ToString();
                    //NURSE_CELL_CODE  VARCHAR2 #17
                    det.NurseCellDept.ID = this.Reader[17].ToString();
                    //NURSE_CELL_NAME  VARCHAR2 #18
                    det.NurseCellDept.Name = this.Reader[18].ToString();
                    //HOUSE_DOC_CODE  VARCHAR2 #19
                    det.HouseDoctor.ID = this.Reader[19].ToString();
                    //HOUSE_DOC_NAME  VARCHAR2 #20
                    det.HouseDoctor.Name = this.Reader[20].ToString();
                    //CHARGE_DOC_CODE  VARCHAR2 #21
                    det.ChargeDoctor.ID = this.Reader[21].ToString();
                    //CHARGE_DOC_NAME  VARCHAR2 #22
                    det.ChargeDoctor.Name = this.Reader[22].ToString();
                    //CHIEF_DOC_CODE  VARCHAR2 #23
                    det.ChiefDoctor.ID = this.Reader[23].ToString();
                    //CHIEF_DOC_NAME  VARCHAR2 #24
                    det.ChiefDoctor.Name = this.Reader[24].ToString();
                    //DUTY_NURSE_CODE  VARCHAR2 #25
                    det.DutyNurse.ID = this.Reader[25].ToString();
                    //DUTY_NURSE_NAME  VARCHAR2 #26
                    det.DutyNurse.Name = this.Reader[26].ToString();
                    //IN_CIRCS  VARCHAR2 #27
                    det.InCircs = this.Reader[27].ToString();
                    //IN_AVENUE  VARCHAR2 #28
                    det.InAvenue = this.Reader[28].ToString();
                    //IN_SOURCE  VARCHAR2 #29
                    det.InSource = this.Reader[29].ToString();
                    //IN_TIMES  NUMBER #30
                    det.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[30].ToString());
                    //IN_STATE  VARCHAR2 #31
                    //det.InState = this.Reader[31] == null ? Neusoft.HISFC.Models.Visit.EnumInState.R : this.Reader[31] as Neusoft.HISFC.Models.Visit.EnumInState;

                    det.InState = this.GetEnumInState(this.Reader[31].ToString());

                    //OUT_DATE  DATE #32
                    det.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[32].ToString());
                    //ZG  VARCHAR2 #33
                    det.Zg = this.Reader[33].ToString();
                    //IN_ICU  VARCHAR2 #34
                    det.InIcu = this.Reader[34].ToString();
                    //TEND  VARCHAR2 #35
                    det.Tend = this.Reader[35].ToString();
                    //CRITICAL_FLAG  VARCHAR2 #36
                    det.CriticalFlag = this.Reader[36].ToString();
                    //DIAG_NAME  VARCHAR2 #37
                    det.DiagName = this.Reader[37].ToString();

                    return det;
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得患者病案信息出错!" + ex.Message;
                return null;
            }
            return null;
        }

        /// <summary>
        /// 获取入院枚举
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Visit.EnumInState GetEnumInState(string str)
        {
            Neusoft.HISFC.Models.Visit.EnumInState enums = new Neusoft.HISFC.Models.Visit.EnumInState();

            switch (str)
            {
                case "I": enums = Neusoft.HISFC.Models.Visit.EnumInState.I; break;
                case "R": enums = Neusoft.HISFC.Models.Visit.EnumInState.R; break;
                case "B": enums = Neusoft.HISFC.Models.Visit.EnumInState.B; break;
                case "O": enums = Neusoft.HISFC.Models.Visit.EnumInState.O; break;
                case "P": enums = Neusoft.HISFC.Models.Visit.EnumInState.P; break;
                case "N": enums = Neusoft.HISFC.Models.Visit.EnumInState.N; break;
            }

            return enums;
        }
        #endregion

        #region 查询历史回访记录时增加随访员以及备注
        /// <summary>
        /// 根据病历号查询历史回访记录
        /// </summary>
        /// <param name="cardNO"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.Visit.VisitRecord> QueryByCardNo(string cardNO)
        {
            string strSql = @"SELECT 
       met_cas_visitrecord.visit_time,
       --met_cas_visitrecord.link_type,
       met_cas_visitrecord.RELATION,
       (SELECT COM_DICTIONARY.NAME
          FROM COM_DICTIONARY
         WHERE COM_DICTIONARY.CODE = MET_CAS_VISITRECORD.Visit_TYPE
           AND COM_DICTIONARY.TYPE = 'CASE06'
           AND COM_DICTIONARY.VALID_STATE = fun_get_valid) AS Visit_TYPENAME,
       (SELECT COM_DICTIONARY.NAME
          FROM COM_DICTIONARY
         WHERE COM_DICTIONARY.CODE = MET_CAS_VISITRECORD.visit_result
           AND COM_DICTIONARY.TYPE = 'CASE14'
           AND COM_DICTIONARY.VALID_STATE = fun_get_valid) AS visit_resultName,
       (SELECT COM_DICTIONARY.NAME
          FROM COM_DICTIONARY
         WHERE COM_DICTIONARY.CODE = MET_CAS_VISITRECORD.CIRCS
           AND COM_DICTIONARY.TYPE = 'CASE07'
           AND COM_DICTIONARY.VALID_STATE = fun_get_valid) AS CIRCS_NAME,
       met_cas_visitrecord.Feedback,
       met_cas_visitrecord.IS_FEEDBACK_PRAISE,
       met_cas_visitrecord.IS_FEEDBACK_SUGGEST,
       met_cas_visitrecord.IS_FEEDBACK_COMPLAINT,
       met_cas_visitrecord.FEEDBACK_DEAL_COMMUNICATION,
       met_cas_visitrecord.FEEDBACK_DEAL_FEEDBACK,
       met_cas_visitrecord.FEEDBACK_DEAL_MODIFY,
       met_cas_visitrecord.FAIL_REASON,
       met_cas_visitrecord.CHANGEPHONE,
       fun_get_employee_name(met_cas_visitrecord.visit_oper), --操作员
       met_cas_visitrecord.extend2, --这里代表的是填写的备注
       met_cas_visitrecord.record_id --这里代表的是随访记录ID
  FROM met_cas_visitrecord";
            string sqlWhere = string.Empty;
            if (this.Sql.GetSql("HealthReacord.Visit.VisitRecord.WhereByCardNo", ref sqlWhere) == -1)
            {
                this.Err = "没有找到HealthReacord.Visit.VisitRecord.WhereByCardNo字段！";
                return null;
            }
            try
            {
                sqlWhere = string.Format(sqlWhere, cardNO);
            }
            catch (Exception ex)
            {
                this.Err = "赋值时出错！" + ex.Message;
                return null;
            }

            strSql = strSql + "\n " + sqlWhere;
            if (this.ExecQuery(strSql) == -1)
            {
                this.Err = "执行SQL语句出错！" + this.Err;
                this.ErrCode = "-1";
                return null;
            }
            List<Neusoft.HISFC.Models.Visit.VisitRecord> list = new List<Neusoft.HISFC.Models.Visit.VisitRecord>();
            try
            {
                Neusoft.HISFC.Models.Visit.VisitRecord obj = null;

                while (this.Reader.Read())
                {
                    obj = new Neusoft.HISFC.Models.Visit.VisitRecord();

                    obj.VisitOper.OperTime = Convert.ToDateTime(this.Reader[0]);
                    obj.LinkWay.Relation.ID = this.Reader[1].ToString();
                    obj.VisitType.Name = this.Reader[2].ToString();
                    obj.VisitResult.Name = this.Reader[3].ToString();
                    obj.Circs.Name = this.Reader[4].ToString();
                    obj.Feedback = this.Reader[5].ToString();
                    obj.IsFeedbackPraise = this.Reader[6].ToString();
                    obj.IsFeedbackSuggest = this.Reader[7].ToString();
                    obj.IsFeedbackComplaint = this.Reader[8].ToString();
                    obj.FeedbackDealCommunication = this.Reader[9].ToString();
                    obj.FeedbackDealFeedback = this.Reader[10].ToString();
                    obj.FeedbackDealModify = this.Reader[11].ToString();
                    obj.FailReason = this.Reader[12].ToString();
                    obj.ChangePhone = this.Reader[13].ToString();
                    obj.VisitOper.Name = this.Reader[14].ToString();
                    obj.Memo = this.Reader[15].ToString();
                    obj.ID = this.Reader[16].ToString();

                    list.Add(obj);
                }
            }
            catch (System.Exception ex)
            {
                this.Err = "获得随访信息出错！" + ex.Message;
                this.ErrCode = "-1";
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return list;
        }
        #endregion
    }
}
