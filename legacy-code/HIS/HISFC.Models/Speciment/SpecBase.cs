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
    /// Table : SPEC_NOHISPATIENT
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecBase : Neusoft.FrameWork.Models.NeuObject
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
        private string out_DiagICD = "";
        private string out_DiaName = "";
        private string main_DiagState = "";
        private string diagnose_Oper_Flag = "";
        private string is30Disease = "";
        private string inHosDiagICD = "";
        private string inHosDiagName = "";
        private string cliDiagICD = "";
        private string cliDiagName = "";
        private string mainDiaICD = "";
        private string mainDiaName = "";
        private string mainDiaICD1 = "";
        private string mainDiaICD2 = "";
        private string mainDiaName1 = "";
        private string mainDiaName2 = "";
        private string mod_ICD = "";
        private string mod_Name = "";
        private DateTime inBaseTime = DateTime.Now;
        private string comment = "";
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
        /// 出院诊断 编码
        /// </summary>
        public string OutDiaICD
        {
            get
            {
                return out_DiagICD;
            }
            set
            {
                out_DiagICD = value;
            }
        }

        /// <summary>
        /// 出院诊断 名称
        /// </summary>
        public string OutDiaName
        {
            get
            {
                return out_DiaName;
            }
            set
            {
                out_DiaName = value;
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
        /// 入院诊断
        /// </summary>
        public string InDiaICD
        {
            get
            {
                return inHosDiagICD;
            }
            set
            {
                inHosDiagICD = value;
            }
        }

        /// <summary>
        /// 入院诊断名称
        /// </summary>
        public string InDiaName
        {
            get
            {
                return inHosDiagName;
            }
            set
            {
                inHosDiagName = value;
            }
        }

        /// <summary>
        /// 门诊ICD
        /// </summary>
        public string CliDiagICD
        {
            get
            {
                return cliDiagICD;
            }
            set
            {
                cliDiagICD = value;
            }
        }

        /// <summary>
        /// 门诊名称
        /// </summary>
        public string CliDiagName
        {
            get
            {
                return cliDiagName;
            }
            set
            {
                cliDiagName = value;
            }
        }

        /// <summary>
        /// 主诊断ICD
        /// </summary>
        public string MainDiaICD
        {
            get
            {
                return mainDiaICD;
            }
            set
            {
                mainDiaICD = value;
            }
        }

        /// <summary>
        /// 主诊断ICD名称
        /// </summary>
        public string MainDiaName
        {
            get
            {
                return mainDiaName;
            }
            set
            {
                mainDiaName = value;
            }
        }

        /// <summary>
        /// 主诊断ICD1
        /// </summary>
        public string MainDiaICD1
        {
            get
            {
                return mainDiaICD1;
            }
            set
            {
                mainDiaICD1 = value;
            }
        }

        /// <summary>
        /// 主诊断ICD1名称
        /// </summary>
        public string MainDiaName1
        {
            get
            {
                return mainDiaName1;
            }
            set
            {
                mainDiaName1 = value;
            }
        }

        /// <summary>
        /// 主诊断ICD2
        /// </summary>
        public string MainDiaICD2
        {
            get
            {
                return mainDiaICD2;
            }
            set
            {
                mainDiaICD2 = value;
            }
        }

        /// <summary>
        /// 主诊断ICD2名称
        /// </summary>
        public string MainDiagName2
        {
            get
            {
                return mainDiaName2;
            }
            set
            {
                mainDiaName2 = value;
            }
        }

        /// <summary>
        /// 形态码
        /// </summary>
        public string ModICD
        {
            get
            {
                return mod_ICD;
            }
            set
            {
                mod_ICD = value;
            }
        }

        /// <summary>
        /// 形态码名称
        /// </summary>
        public string ModName
        {
            get
            {
                return mod_Name;
            }
            set
            {
                mod_Name = value;
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
        #endregion

        #region 方法
        public new SpecBase Clone()
        {
            SpecBase sp = base.Clone() as SpecBase;
            sp.SpecSource = this.SpecSource.Clone();
            return sp;
        }
        #endregion
    }
}
