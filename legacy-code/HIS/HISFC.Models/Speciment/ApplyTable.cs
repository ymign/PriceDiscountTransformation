using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 申请表实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_APPLICATIONTABLE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class ApplyTable : Neusoft.FrameWork.Models.NeuObject
    {
        #region 域变量
        private int applyID = 0;
        private string applyUserID = "";
        private string applyUserName = "";
        private string impDocID = "";
        private string impEmail = "";
        private string impTel = "";
        private string impName = "";
        private string deptID = "";
        private string deptName = "";
        private string subjectName = "";
        private string subjectID = "";
        private string resPlan = "";
        private string resPlanAtt = "";
        private DateTime fundStartTime = DateTime.Now;
        private DateTime fundEndTime = DateTime.Now;
        private string fundName = "";
        private string fundID = "";
        private string applyEmail = "";
        private string applyTel = "";
        private DateTime applyTime = DateTime.Now;
        private string diseaseType = "";
        private int specAmount = 0;
        private string otherDemand = "";
        private string sepcDetAmout = "";
        private string specType = "";
        private string cancerType = "";
        private string specList = "";
        private string outputReslut = "";
        private DateTime outPutTime = DateTime.Now;
        private string deptFromComm = "";
        private DateTime deptFromDate = DateTime.Now;
        private string outputOperDoc = "";
        private string specAdmComment = "";
        private DateTime specAdmDate = DateTime.Now;
        private string acceptConfirm = "";
        private DateTime accConfirmDate = DateTime.Now;
        private string isImmdBackList = "";
        private string specCountInDept = "";
        private string percent = "";
        private string leftAmount = "";
        private string researchResult = "";
        private string comment = "";
        private string impProcess = "U";
        private string impReslut = "";
        #endregion

        #region 属性

        /// <summary>
        /// 申请单Id
        /// </summary>
        public int ApplyId
        {
            get
            {
                return applyID;
            }
            set
            {
                applyID = value;
            }
        }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplyUserId
        {
            get
            {
                return applyUserID;
            }
            set
            {
                applyUserID = value;
            }
        }

        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string ApplyUserName
        {
            get
            {
                return applyUserName;
            }
            set
            {
                applyUserName = value;
            }
        }

        /// <summary>
        /// 执行医生ID
        /// </summary>
        public string ImpDocId
        {
            get
            {
                return impDocID;
            }
            set
            {
                impDocID = value;
            }
        }

        /// <summary>
        /// 执行人的Email
        /// </summary>
        public string ImpEmail
        {
            get
            {
                return impEmail;
            }
            set
            {
                impEmail = value;
            }
        }

        /// <summary>
        /// 执行人姓名
        /// </summary>
        public string ImpName
        {
            get
            {
                return impName;
            }
            set
            {
                impName = value;
            }
        }

        /// <summary>
        /// 执行人电话
        /// </summary>
        public string ImpTel
        {
            get
            {
                return impTel;
            }
            set
            {
                impTel = value;
            }
        }

        /// <summary>
        /// 科室ID
        /// </summary>
        public string DeptId
        {
            get
            {
                return deptID;
            }
            set
            {
                deptID = value;
            }
        }

        /// <summary>
        /// 科室名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return deptName;
            }
            set
            {
                deptName = value;
            }
        }

        /// <summary>
        /// 课题名称
        /// </summary>
        public string SubjectName
        {
            get
            {
                return subjectName;
            }
            set
            {
                subjectName = value;
            }
        }

        /// <summary>
        /// 课题Id
        /// </summary>
        public string SubjectId
        {
            get
            {
                return subjectID;
            }
            set
            {
                subjectID = value;
            }
        }

        /// <summary>
        /// 研究计划
        /// </summary>
        public string ResPlan
        {
            get
            {
                return resPlan;
            }
            set
            {
                resPlan = value;
            }
        }

        /// <summary>
        /// 基金启动时间
        /// </summary>
        public DateTime FundStartTime
        {
            get
            {
                return fundStartTime;
            }
            set
            {
                fundStartTime = value;
            }
        }

        /// <summary>
        /// 基金结束时间
        /// </summary>
        public DateTime FundEndTime
        {
            get
            {
                return fundEndTime;
            }
            set
            {
                fundEndTime = value;
            }
        }

        /// <summary>
        /// 基金名称
        /// </summary>
        public string FundName
        {
            get
            {
                return fundName;
            }
            set
            {
                fundName = value;
            }
        }

        /// <summary>
        /// 基金Id
        /// </summary>
        public string FundId
        {
            get
            {
                return fundID;
            }
            set
            {
                fundID = value;
            }
        }

        /// <summary>
        /// 标本执行输出情况
        /// </summary>
        public string OutPutResult
        {
            get
            {
                return outputReslut;
            }
            set
            {
                outputReslut = value;
            }
        }

        /// <summary>
        /// 申请人Email
        /// </summary>
        public string ApplyEmail
        {
            get
            {
                return applyEmail;
            }
            set
            {
                applyEmail = value;
            }
        }

        /// <summary>
        /// 申请人电话
        /// </summary>
        public string ApplyTel
        {
            get
            {
                return applyTel;
            }
            set
            {
                applyTel = value;
            }
        }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime
        {
            get
            {
                return applyTime;
            }
            set
            {
                applyTime = value;
            }
        }

        /// <summary>
        /// 申请病种类型，如有多种用分隔符分割
        /// </summary>
        public string DiseaseType
        {
            get
            {
                return diseaseType;
            }
            set
            {
                diseaseType = value;
            }
        }

        /// <summary>
        /// 申请标本数量
        /// </summary>
        public int SpecAmout
        {
            get
            {
                return specAmount;
            }
            set
            {
                specAmount = value;
            }
        }

        /// <summary>
        /// 其他要求
        /// </summary>
        public string OtherDemand
        {
            get
            {
                return otherDemand;
            }
            set
            {
                otherDemand = value;
            }
        }

        /// <summary>
        /// 申请标本种类对应的数量，如有多种，用分隔符分割，与DiseaseType 对应
        /// </summary>
        public string SpecDetAmout
        {
            get
            {
                return sepcDetAmout;
            }
            set
            {
                sepcDetAmout = value;
            }
        }

        /// <summary>
        /// 申请标本的类型，多种用分隔符分开
        /// </summary>
        public string SpecType
        {
            get
            {
                return specType;
            }
            set
            {
                specType = value;
            }
        }

        /// <summary>
        /// 标本是否是癌变，与标本类型一一对应，分隔符分割
        /// </summary>
        public string SpecIsCancer
        {
            get
            {
                return cancerType;
            }
            set
            {
                cancerType = value;
            }
        }

        /// <summary>
        /// 输出标本号列表
        /// </summary>
        public string SpecList
        {
            get
            {
                return specList;
            }
            set
            {
                specList = value;
            }
        }

        /// <summary>
        /// 输出标本时间
        /// </summary>
        public DateTime OutPutTime
        {
            get
            {
                return outPutTime;
            }
            set
            {
                outPutTime = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string IsImmdBackList
        {
            get
            {
                return isImmdBackList;
            }
            set
            {
                isImmdBackList = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
            }
        }

        /// <summary>
        /// 研究成果
        /// </summary>
        public string ResearchResult
        {
            get
            {
                return researchResult;
            }
            set
            {
                researchResult = value;
            }
        }

        /// <summary>
        /// 剩余标本数量
        /// </summary>
        public string LeftAmount
        {
            get
            {
                return leftAmount;
            }
            set
            {
                leftAmount = value;
            }
        }

        /// <summary>
        /// 占总量的比例
        /// </summary>
        public string Percent
        {
            get
            {
                return percent;
            }
            set
            {
                percent = value;
            }
        }

        /// <summary>
        /// 取用标本科室的所存标本的总量
        /// </summary>
        public string SpecCountInDpet
        {
            get
            {
                return specCountInDept;
            }
            set
            {
                specCountInDept = value;
            }
        }

        /// <summary>
        /// 研究计划附件
        /// </summary>
        public string ResPlanAtt
        {
            get
            {
                return resPlanAtt;
            }
            set
            {
                resPlanAtt = value;
            }
        }

        /// <summary>
        /// 接受标本确认签名日期
        /// </summary>
        public DateTime AcceptConfrimDate
        {
            get
            {
                return accConfirmDate;
            }
            set
            {
                accConfirmDate = value;
            }
        }

        /// <summary>
        /// 接受标本确认签名
        /// </summary>
        public string AcceptConfirm
        {
            get
            {
                return acceptConfirm;
            }
            set
            {
                acceptConfirm = value;
            }
        }

        /// <summary>
        /// 标本库负责人审核意见 日期
        /// </summary>
        public DateTime SepcAdmDate
        {
            get
            {
                return specAdmDate;
            }
            set
            {
                specAdmDate = value;
            }

        }

        /// <summary>
        /// 标本库负责人审核意见
        /// </summary>
        public string SpecAdmComment
        {
            get
            {
                return specAdmComment;
            }
            set
            {
                specAdmComment = value;
            }
        }

        /// <summary>
        /// 输出执行医生名称
        /// </summary>
        public string OutPutOperDoc
        {
            get
            {
                return outputOperDoc;
            }
            set
            {
                outputOperDoc = value;
            }
        }

        /// <summary>
        /// 标本来源科室主任意见
        /// </summary>
        public string DeptFromComm
        {
            get
            {
                return deptFromComm;
            }
            set
            {
                deptFromComm = value;
            }
        }

        /// <summary>
        /// 标本所在部门审核日期
        /// </summary>
        public DateTime DeptFromDate
        {
            get
            {
                return deptFromDate;
            }
            set
            {
                deptFromDate = value;
            }

        }

        /// <summary>
        /// 审批进程
        /// </summary>
        public string ImpProcess
        {
            get
            {
                return impProcess;
            }
            set
            {
                impProcess = value;
            }
        }

        /// <summary>
        /// 审批结果
        /// </summary>
        public string ImpResult
        {
            get
            {
                return impReslut;
            }
            set
            {
                impReslut = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new ApplyTable Clone()
        {
            ApplyTable applyTable = base.Clone() as ApplyTable;
            return applyTable;
        }
        #endregion
    }
}
