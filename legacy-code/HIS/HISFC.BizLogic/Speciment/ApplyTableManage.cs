using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 申请表管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-11-24]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-30' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class ApplyTableManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">标本组织类型对象</param>
        /// <returns></returns>
        private string[] GetParam(ApplyTable applyTable)
        {
            string[] str = new string[] 
            { 
                applyTable.ApplyId.ToString(),
                applyTable.ApplyUserId,
                applyTable.ImpDocId,
                applyTable.ImpEmail,
                applyTable.ImpTel,
                applyTable.DeptId,
                applyTable.DeptName,
                applyTable.SubjectName,
                applyTable.SubjectId,
                applyTable.ResPlan,
                applyTable.ResPlanAtt,
                applyTable.FundStartTime.ToString(),
                applyTable.FundEndTime.ToString(),
                applyTable.FundName,
                applyTable.FundId,                
                applyTable.ApplyEmail,
                applyTable.ApplyTel,
                applyTable.ApplyTime.ToString(),
                applyTable.DiseaseType,
                applyTable.SpecAmout.ToString(),
                applyTable.SpecDetAmout,
                applyTable.OtherDemand,
                applyTable.SpecType,
                applyTable.SpecIsCancer,
                applyTable.SpecList,
                applyTable.OutPutResult,
                applyTable.OutPutTime.ToString(),
                applyTable.OutPutOperDoc,
                applyTable.DeptFromComm,
                applyTable.DeptFromDate.ToString(),
                applyTable.SpecAdmComment,
                applyTable.SepcAdmDate.ToString(),
                applyTable.AcceptConfirm,
                applyTable.AcceptConfrimDate.ToString(),
                applyTable.IsImmdBackList,
                applyTable.SpecCountInDpet,
                applyTable.Percent,
                applyTable.LeftAmount,
                applyTable.ResearchResult,
                applyTable.Comment,
                applyTable.ApplyUserName,
                applyTable.ImpProcess,
                applyTable.ImpResult,
                applyTable.ImpName
            };
            return str;
        }

        #region 设置错误信息
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="errorCode">错误代码发生行数</param>
        /// <param name="errorText">错误信息</param>
        private void SetError(string errorCode, string errorText)
        {
            this.ErrCode = errorCode;
            this.Err = errorText + "[" + this.Err + "]"; // + "在ShelfSpecManage.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion
        #endregion

        #region 更新申请表操作
        /// <summary>
        /// 更新申请表
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateApplyTable(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;//Update语句

            //获得Where语句
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return -1;
            }

            return this.ExecNoQuery(sql, args);
        }

        /// <summary>
        /// 根据sql索引获取申请表列表
        /// </summary>
        /// <param name="sqlIndex">sql索引</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        private ArrayList QueryApplyTable(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return null;
            }
            if (this.ExecQuery(sql, args) == -1)
                return null;
            ArrayList arrApplyTable = new ArrayList();
            while (this.Reader.Read())
            {
                ApplyTable appTable = SetApplyTable();
                arrApplyTable.Add(appTable);
            }
            this.Reader.Close();
            return arrApplyTable;
        }

        /// <summary>
        /// 根据sql查询申请表列表
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        private ArrayList QueryApplyTable(string sql)
        {
            if (this.ExecQuery(sql) == -1)
                return null;
            ArrayList arrApplyTable = new ArrayList();
            while (this.Reader.Read())
            {
                ApplyTable appTable = SetApplyTable();
                arrApplyTable.Add(appTable);
            }
            this.Reader.Close();
            return arrApplyTable;
        }

        /// <summary>
        /// 读取申请表信息
        /// </summary>
        /// <returns>申请表实体</returns>
        private ApplyTable SetApplyTable()
        {
            ApplyTable applyTable = new ApplyTable();
            try
            {
                applyTable.ApplyId = Convert.ToInt32(this.Reader["APPLICATIONID"].ToString());
                applyTable.ApplyUserId = this.Reader["APPLYUSERID"].ToString();
                applyTable.ImpDocId = this.Reader["IMPLEMENTDOCID"].ToString();
                applyTable.ImpEmail = this.Reader["IMPLEMENTEMAIL"].ToString();
                applyTable.ImpTel = this.Reader["IMPLEMENTTEL"].ToString();
                applyTable.DeptId = this.Reader["DEPARTMENTID"].ToString();
                applyTable.DeptName = this.Reader["DEPARTMENTNAME"].ToString();
                applyTable.SubjectName = this.Reader["SUBJECTNAME"].ToString();
                applyTable.SubjectId = this.Reader["SUBJECTID"].ToString();
                applyTable.ResPlan = this.Reader["RESEARCHPLAN"].ToString();
                applyTable.ResPlanAtt = this.Reader["RESEARCHPLANATT"].ToString();
                applyTable.FundStartTime = Convert.ToDateTime(this.Reader["FUNDSTARTTIME"].ToString());
                applyTable.FundEndTime = Convert.ToDateTime(this.Reader["FUNDENDTIME"].ToString());
                applyTable.FundName = this.Reader["FUNDNAME"].ToString();
                applyTable.FundId = this.Reader["FUNDID"].ToString();
                applyTable.ApplyEmail = this.Reader["APPLYEMAIL"].ToString();
                applyTable.ApplyTel = this.Reader["APPLYTEL"].ToString();
                applyTable.ApplyTime = Convert.ToDateTime(this.Reader["APPLYTIME"].ToString());
                applyTable.DiseaseType = this.Reader["DISEASETYPE"].ToString();
                applyTable.SpecAmout = Convert.ToInt32(this.Reader["SPECAMOUNT"].ToString());
                applyTable.SpecDetAmout = this.Reader["SPECDETAMOUNT"].ToString();
                applyTable.OtherDemand = this.Reader["OTHERDEMAND"].ToString();
                applyTable.SpecType = this.Reader["SPECTYPE"].ToString();
                applyTable.SpecIsCancer = this.Reader["SPECISCANCER"].ToString();
                applyTable.SpecList = this.Reader["SPECLIST"].ToString();
                applyTable.OutPutResult = this.Reader["OUTPUTRESULT"].ToString();
                applyTable.OutPutTime = Convert.ToDateTime(this.Reader["OUTPUTTIME"].ToString());
                applyTable.OutPutOperDoc = this.Reader["OUTPUTOPERDOC"].ToString();
                applyTable.DeptFromComm = this.Reader["SPECDEPTFROM"].ToString();
                applyTable.DeptFromDate = Convert.ToDateTime(this.Reader["DEPTDATE"].ToString());
                applyTable.SpecAdmComment = this.Reader["SPECADM"].ToString();
                applyTable.SepcAdmDate = Convert.ToDateTime(this.Reader["SPECADMDATE"].ToString());
                applyTable.AcceptConfirm = this.Reader["ACCEPTCONFRIM"].ToString();
                applyTable.AcceptConfrimDate = Convert.ToDateTime(this.Reader["ACCEPTCONDATE"].ToString());
                applyTable.IsImmdBackList = this.Reader["ISIMMEDBACKLIST"].ToString();
                applyTable.SpecCountInDpet = this.Reader["SPECCOUNTINDEPT"].ToString();
                applyTable.Percent = this.Reader["PERINALL"].ToString();
                applyTable.LeftAmount = this.Reader["LEFTAMOUT"].ToString();
                applyTable.ResearchResult = this.Reader["RESREACHRESULT"].ToString();
                applyTable.Comment = this.Reader["MARK"].ToString();
                applyTable.ApplyUserName = this.Reader["APPLYUSERNAME"].ToString();
                applyTable.ImpProcess = this.Reader["IMPPROCESS"].ToString();
                applyTable.ImpResult = this.Reader["IMPRESLUT"].ToString();
                applyTable.ImpName = this.Reader["IMPLEMENTNAME"].ToString();
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return applyTable;
        }
        #endregion

        #endregion

        #region 公有方法


        /// <summary>
        /// 获取新的Sequence(1：成功/-1：失败)
        /// </summary>
        /// <param name="sequence">获取的新的Sequence</param>
        /// <returns>1：成功/-1：失败</returns>
        public int GetNextSequence(ref string sequence)
        {
            //
            // 执行SQL语句
            //
            sequence = this.GetSequence("Speciment.BizLogic.ApplyTableManage.GetNextSequence");
            //
            // 如果返回NULL，则获取失败
            //
            if (sequence == null)
            {
                this.SetError("", "获取Sequence失败");
                return -1;
            }
            //
            // 成功返回
            //
            return 1;
        }

        /// <summary>
        /// 插入申请表
        /// </summary>
        /// <param name="applyTable">申请表实体</param>
        /// <returns></returns>
        public int InsertApplyTable(ApplyTable applyTable)
        {
            return this.UpdateApplyTable("Speciment.BizLogic.ApplyTableManage.Insert", this.GetParam(applyTable));

        }

        /// <summary>
        /// 根据申请ID获取申请表
        /// </summary>
        /// <param name="applyNum"></param>
        /// <returns></returns>
        public ApplyTable QueryApplyByID(string applyNum)
        {
            ArrayList arrTmp = new ArrayList();
            string sql = "SELECT * FROM SPEC_APPLICATIONTABLE WHERE APPLICATIONID = " + applyNum;
            arrTmp = this.QueryApplyTable(sql);
            ApplyTable appTable = new ApplyTable();
            foreach (ApplyTable a in arrTmp)
            {
                appTable = a;
                break;
            }

            #region 增加个当前进度情况
            try
            {
                string sqlIdx = @"select b.SCHEDULE 进度描述 from SPEC_USERAPPLICATION b where 
                                        b.USERAPPLID = (select max(USERAPPLID) from SPEC_USERAPPLICATION where APPLICATIONID = {0})";
                sqlIdx = string.Format(sqlIdx, applyNum);
                if (this.ExecQuery(sqlIdx) == -1)
                    return null;
                while (this.Reader.Read())
                {
                    appTable.User03 = this.Reader[0].ToString();
                }
                this.Reader.Close();
            }
            catch
            {
            }
            #endregion

            return appTable;
        }

        /// <summary>
        /// 根据申请人ID获取申请表
        /// 根据impprocess标志获取已经审批的（‘O’）、审批中（‘U’）、取消（‘C’）的申请表
        /// </summary>
        /// <param name="applyUserNum"></param>
        /// <param name="impprocess"></param>
        /// <returns></returns>
        public ArrayList QueryApplyByApplyUserID(string applyUserId, string impprocess)
        {
            ArrayList arrApplyListApplyId = new ArrayList();
            //ArrayList arrTmp = new ArrayList();
            string sql = "SELECT SPEC_APPLICATIONTABLE.APPLICATIONID FROM SPEC_APPLICATIONTABLE WHERE APPLYUSERID = " + applyUserId + " and IMPPROCESS =" + impprocess;
            arrApplyListApplyId = this.QueryApplyTable(sql);
            return arrApplyListApplyId;
        }

        /// <summary>
        /// 更新申请表
        /// </summary>
        /// <param name="applyTable"></param>
        /// <returns></returns>
        public int UpdateApplyTable(ApplyTable applyTable)
        {
            return this.UpdateApplyTable("Speciment.BizLogic.ApplyTableManage.Update", GetParam(applyTable));
        }

        /// <summary>
        /// 获取下一个申请ID
        /// </summary>
        /// <returns></returns>
        public int GetMaxApplyId()
        {
            try
            {
                return Convert.ToInt32(this.ExecSqlReturnOne("select max(a.APPLICATIONID) + 1	from SPEC_APPLICATIONTABLE a"));
            }
            catch
            {
                return -1;
            }

        }

        /// <summary>
        /// 根据申请人、科室及起止时间查询申请记录
        /// </summary>
        /// <param name="empl"></param>
        /// <param name="dept"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public ArrayList GetSubApply(string empl, string dept, DateTime dtBegin, DateTime dtEnd)
        {
            ArrayList arrApplyList = new ArrayList();
            string sql = @"SELECT * FROM SPEC_APPLICATIONTABLE WHERE APPLYUSERID like '{0}' 
                           and DEPARTMENTID like '{1}' 
                           and APPLYTIME between timestamp('{2}') and timestamp('{3}')";
            try
            {
                sql = string.Format(sql, empl, dept, dtBegin, dtEnd);
                arrApplyList = this.QueryApplyTable(sql);
            }
            catch
            {
                return null;
            }
            return arrApplyList;
        }

        public int appExecQuery(string sqlIdx, ref DataSet ds)
        {
            return this.ExecQuery(sqlIdx, ref ds);
        }
        #endregion
    }
}
