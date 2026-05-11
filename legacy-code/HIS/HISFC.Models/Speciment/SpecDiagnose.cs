using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 病人的诊断信息]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-23]<br></br>
    /// Table : SPEC_DIAGNOSE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecDiagnose : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private SpecSource specSource = new SpecSource();
        private int baseID;
        private string hcv_AB = "";
        private string hbSag = "";
        private string hiv_AB = "";
        private string rh_Blood = "";
        private string x_Times = "";
        private string mr_Times = "";
        private string dsa_Times = "";
        private string pet_Times = "";
        private string ect_Times = "";
        private string main_DiagState = "";
        private string diagnose_Oper_Flag = "";
        private string is30Disease = "";
        private DiaDetail diag = new DiaDetail();
        private DiaDetail diag1 = new DiaDetail();
        private DiaDetail diag2 = new DiaDetail();
        private string diagRemark = "";
        private DateTime inBaseTime = DateTime.Now;
        private string comment = "";
        private string operId = "";
        private string operName = "";
        private string ext1 = "";
        private string ext2 = "";
        private string ext3 = "";
        #endregion

        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public int BaseID
        {
            get
            {
                return baseID;
            }
            set
            {
                baseID = value;
            }
        }

        /// <summary>
        /// 标本
        /// </summary>
        public SpecSource SpecSource
        {
            get
            {
                return specSource;
            }
            set
            {
                specSource = value;
            }
        }

        /// <summary>
        /// 丙肝病毒抗体（阴性、阳性、未做）
        /// </summary>
        public string HCV_AB
        {
            get
            {
                return hcv_AB;
            }
            set
            {
                hcv_AB = value;
            }
        }

        /// <summary>
        /// 乙肝表面抗原（阴性、阳性、未做）
        /// </summary>
        public string HbSAG
        {
            get
            {
                return hbSag;
            }
            set
            {
                hbSag = value;
            }
        }

        /// <summary>
        /// 获得性人类免疫缺陷病毒抗体（阴性、阳性、未做）
        /// </summary>
        public string Hiv_AB
        {
            get
            {
                return hiv_AB;
            }
            set
            {
                hiv_AB = value;
            }
        }

        /// <summary>
        /// RH血型(阴、阳)
        /// </summary>
        public string RHBlood
        {
            get
            {
                return rh_Blood;
            }
            set
            {
                rh_Blood = value;
            }
        }

        /// <summary>
        /// X线次数
        /// </summary>
        public string X_Times
        {
            get
            {
                return x_Times;
            }
            set
            {
                x_Times = value;
            }
        }

        /// <summary>
        /// MR次数
        /// </summary>
        public string MR_Times
        {
            get
            {
                return mr_Times;
            }
            set
            {
                mr_Times = value;
            }
        }

        /// <summary>
        /// DSA次数
        /// </summary>
        public string DSA_Times
        {
            get
            {
                return dsa_Times;
            }
            set
            {
                dsa_Times = value;
            }
        }

        /// <summary>
        /// PET次数
        /// </summary>
        public string PET_Times
        {
            get
            {
                return pet_Times;
            }
            set
            {
                pet_Times = value;
            }
        }

        /// <summary>
        /// ECT次数
        /// </summary>
        public string ECT_Times
        {
            get
            {
                return ect_Times;
            }
            set
            {
                ect_Times = value;
            }
        }

        /// <summary>
        /// 诊断 治疗情况
        /// </summary>
        public string Main_DiagState
        {
            get
            {
                return main_DiagState;
            }
            set
            {
                main_DiagState = value;
            }
        }

        /// <summary>
        /// 是否做过手术
        /// </summary>
        public string Diagnose_Oper_Flag
        {
            get
            {
                return diagnose_Oper_Flag;
            }
            set
            {
                diagnose_Oper_Flag = value;
            }
        }

        /// <summary>
        /// 是否是30种疾病
        /// </summary>
        public string Is30Disease
        {
            get
            {
                return is30Disease;
            }
            set
            {
                is30Disease = value;
            }
        }

        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime InBaseTime
        {
            get
            {
                return inBaseTime;
            }
            set
            {
                inBaseTime = value;
            }
        }

        public DiaDetail Diag1
        {
            get
            {
                return diag1;
            }
            set
            {
                diag1 = value;
            }
        }

        public DiaDetail Diag2
        {
            get
            {
                return diag2;
            }
            set
            {
                diag2 = value;
            }
        }

        public DiaDetail Diag
        {
            get
            {
                return diag;
            }
            set
            {
                diag = value;
            }
        }

        public string DiagRemark
        {
            get
            {
                return diagRemark;
            }
            set
            {
                diagRemark = value;
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

        public string OperId
        {
            get
            {
                return operId;
            }
            set
            {
                operId = value;
            }
        }

        public string OperName
        {
            get
            {
                return operName;
            }
            set
            {
                operName = value;
            }
        }

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

        #region 方法
        public new SpecDiagnose Clone()
        {
            SpecDiagnose sp = base.Clone() as SpecDiagnose;
            sp.SpecSource = this.SpecSource.Clone();
            sp.diag1 = this.diag1.Clone();
            sp.diag2 = this.diag2.Clone();
            sp.diag = this.diag.Clone();
            return sp;
        }
        #endregion
    }
}
