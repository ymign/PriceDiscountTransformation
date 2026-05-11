using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 原标本实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_SOURCE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecSource : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int specID = 0;
        private string hisBarCode = "";
        private DiseaseType disType = new DiseaseType();
        private decimal anticolBldCapacity = 10.0M;
        private int antiColBldCount = 1;
        private decimal nonantiBldCapacity = 10.0M;
        private int nonantiBldCount = 1;
        private Base.Employee sendEmp = new Base.Employee();
        private Base.Employee colEmp = new Base.Employee();
        private Base.Employee operEmp = new Base.Employee();
        private DateTime operTime = DateTime.Now;
        private string cardNO = "";
        private string inPatientNO = "";
        private string comment = "";
        private char limitUse = ' ';
        private char fromHis = ' ';
        private string deptNo = "";
        private string orgType = "";
        private string orgOrBlood = "";
        private string getPeriod = "";
        private DateTime sendTime = DateTime.Now;
        private string icCardNo = "";
        private string operPosCode = "";
        private string radScheme = "";
        private string medScheme = "";
        private string mediComment = "";
        private string medGrp = "";
        private MedDoc medDoc = new MedDoc();
        private SpecPatient patient = new SpecPatient();
        private string tumorPor = "";
        private string operApplyId = "";
        private string operPosName = "";
        private string inDeptNo = "";
        private char isInbase = '0';
        private string matchFlag = "0";
        private string specNo = "";
        #endregion

        #region 属性
        #region 新增lingk
        private string isCancer = "1";//是否癌变1是0否
        public string IsCancer
        {
            get
            {
                return isCancer;
            }
            set
            {
                isCancer = value;
            }
        }

        private string ext1 = string.Empty;//扩展1
        public string Ext1
        {
            get
            {
                return ext1;
            }
            set
            {
                ext1 = value;
            }
        }

        private string ext2 = string.Empty;//扩展2
        public string Ext2
        {
            get
            {
                return ext2;
            }
            set
            {
                ext2 = value;
            }
        }

        private string ext3 = string.Empty;//扩展3
        public string Ext3
        {
            get
            {
                return ext3;
            }
            set
            {
                ext3 = value;
            }
        }
        #endregion

        /// <summary>
        /// 执行医生
        /// </summary>
        public Base.Employee OperEmp
        {
            get
            {
                return operEmp;
            }
            set
            {
                operEmp = value;
            }
        }

        /// <summary>
        /// 非抗凝血数量
        /// </summary>
        public int NonantiBldCount
        {
            get
            {
                return nonantiBldCount;
            }
            set
            {
                nonantiBldCount = value;
            }
        }

        /// <summary>
        /// 非抗凝血容量
        /// </summary>
        public decimal NonAntiBldCapcacity
        {
            get
            {
                return nonantiBldCapacity;
            }
            set
            {
                nonantiBldCapacity = value;
            }
        }

        /// <summary>
        /// 抗凝血数量
        /// </summary>
        public int AnticolBldCount
        {
            get
            {
                return antiColBldCount;
            }
            set
            {
                antiColBldCount = value;
            }
        }

        /// <summary>
        /// 抗凝血容量
        /// </summary>
        public decimal AnticolBldCapcacity
        {
            get
            {
                return anticolBldCapacity;
            }
            set
            {
                anticolBldCapacity = value;
            }
        }

        /// <summary>
        /// 手术申请Id
        /// </summary>
        public string OperApplyId
        {
            get
            {
                return operApplyId;
            }
            set
            {
                operApplyId = value;
            }
        }

        /// <summary>
        /// 肿瘤属性，原发，复发，or转移
        /// </summary>
        public string TumorPor
        {
            get
            {
                return tumorPor;
            }
            set
            {
                tumorPor = value;
            }
        }

        /// <summary>
        /// 标本所属病人
        /// </summary>
        public SpecPatient Patient
        {
            get
            {
                return patient;
            }
            set
            {
                patient = value;
            }
        }

        /// <summary>
        /// 治疗医生组
        /// </summary>
        public MedDoc MediDoc
        {
            get
            {
                return medDoc;
            }
            set
            {
                medDoc = value;
            }
        }

        /// <summary>
        /// 医疗组
        /// </summary>
        public string MedGrp
        {
            get
            {
                return medGrp;
            }
            set
            {
                medGrp = value;
            }
        }

        /// <summary>
        /// 治疗备注
        /// </summary>
        public string MedComment
        {
            get
            {
                return mediComment;
            }
            set
            {
                mediComment = value;
            }
        }

        //放疗方案
        public string RadScheme
        {
            get
            {
                return radScheme;
            }
            set
            {
                radScheme = value;
            }
        }

        /// <summary>
        /// 化疗方案
        /// </summary>
        public string MedScheme
        {
            get
            {
                return medScheme;
            }
            set
            {
                medScheme = value;
            }
        }

        /// <summary>
        /// 手术部位代码
        /// </summary>
        public string OperPosCode
        {
            get
            {
                return operPosCode;
            }
            set
            {
                operPosCode = value;
            }
        }

        /// <summary>
        /// 手术部位名称
        /// </summary>
        public string OperPosName
        {
            get
            {
                return operPosName;
            }
            set
            {
                operPosName = value;
            }
        }

        /// <summary>
        /// 住院科室
        /// </summary>
        public string InDeptNo
        {
            get
            {
                return inDeptNo;
            }
            set
            {
                inDeptNo = value;
            }
        }

        public SpecSource()
        {

        }

        /// <summary>
        /// 标本ID号
        /// </summary>
        public int SpecId
        {
            get
            {
                return specID;
            }

            set
            {
                specID = value;
            }
        }

        /// <summary>
        /// 原标本的条形码，从Lis中获取
        /// </summary>
        public string HisBarCode
        {
            get
            {
                return hisBarCode;
            }
            set
            {
                hisBarCode = value;
            }
        }

        /// <summary>
        /// 标本的病种类型
        /// </summary>
        public DiseaseType DiseaseType
        {
            get
            {
                return disType;
            }
            set
            {
                disType = value;
            }

        }

        /// <summary>
        /// 送标本医生
        /// </summary>
        public Base.Employee SendDoctor
        {
            get
            {
                return sendEmp;
            }
            set
            {
                sendEmp = value;
            }
        }

        /// <summary>
        /// 搜集标本医生
        /// </summary>
        public Base.Employee ColDoctor
        {
            get
            {
                return colEmp;
            }
            set
            {
                colEmp = value;
            }
        }

        /// <summary>
        /// 执行时间时间
        /// </summary>
        public DateTime OperTime
        {
            get
            {
                return operTime;
            }
            set
            {
                operTime = value;
            }
        }

        /// <summary>
        /// 病例号
        /// </summary>
        public string CardNo
        {
            get
            {
                return cardNO;
            }
            set
            {
                cardNO = value;
            }
        }

        /// <summary>
        /// 病人的住院流水号
        /// </summary>
        public string InPatientNo
        {
            get
            {
                return inPatientNO;
            }
            set
            {
                inPatientNO = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Commet
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
        /// 是否限制使用，0不限制，1：限本人，2：限科室
        /// </summary>
        public char LimitUse
        {
            get
            {
                return limitUse;
            }
            set
            {
                limitUse = value;
            }
        }

        /// <summary>
        /// 原标本是否来源于HIs病人
        /// </summary>
        public char IsHis
        {
            get
            {
                return fromHis;
            }
            set
            {
                fromHis = value;
            }
        }

        /// <summary>
        /// 来源科室
        /// </summary>
        public string DeptNo
        {
            get
            {
                return deptNo;
            }
            set
            {
                deptNo = value;
            }
        }

        /// <summary>
        /// 如果是组织类型的标本，则保存该字段。肿瘤：T,子瘤
        /// </summary>
        public string OrgType
        {
            get
            {
                return orgType;
            }
            set
            {
                orgType = value;
            }
        }

        /// <summary>
        /// 组织还是血标本
        /// </summary>
        public string OrgOrBoold
        {
            get
            {
                return orgOrBlood;
            }
            set
            {
                orgOrBlood = value;
            }
        }

        /// <summary>
        /// 标本取自病人的哪个阶段：放疗前，放疗后等等
        /// </summary>
        public string GetSpecPeriod
        {
            get
            {
                return getPeriod;
            }
            set
            {
                getPeriod = value;
            }
        }

        /// <summary>
        /// 诊疗卡条形码
        /// </summary>
        public string ICCardNO
        {
            get
            {
                return icCardNo;
            }
            set
            {
                icCardNo = value;
            }
        }

        public DateTime SendTime
        {
            get
            {
                return sendTime;
            }
            set
            {
                sendTime = value;
            }
        }

        public char IsInBase
        {
            get
            {
                return isInbase;
            }
            set
            {
                isInbase = value;
            }
        }

        /// <summary>
        /// 标本的配对标志
        /// </summary>
        public string MatchFlag
        {
            get
            {
                return matchFlag;
            }
            set
            {
                matchFlag = value;
            }
        }

        /// <summary>
        /// 设置标本号
        /// </summary>
        public string SpecNo
        {
            get
            {
                return specNo;
            }
            set
            {
                specNo = value;
            }

        }
        #endregion

        #region 方法
        public new SpecSource Clone()
        {
            SpecSource spec = base.Clone() as SpecSource;
            spec.SendDoctor = this.SendDoctor.Clone();
            spec.ColDoctor = this.ColDoctor.Clone();
            spec.MediDoc = this.MediDoc.Clone();

            return spec;
        }
        #endregion
    }
}
