using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// CapLog<br></br>
    /// [功能描述: 标本诊断信息]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-12-14]<br></br>
    /// Table : SPEC_DIAGNOSE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class DiaDetail : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        //诊断码
        private string icd = "";
        private string icdName = "";
        //形态码
        private string mod = "";
        private string modName = "";
        //分期
        private string p_code = "";
        private string n_code = "";
        private string m_code = "";
        private string t_code = "";
        #endregion

        #region 属性
        public string Icd
        {
            get
            {
                return icd;
            }
            set
            {
                icd = value;
            }
        }

        public string IcdName
        {
            get
            {
                return icdName;
            }
            set
            {
                icdName = value;
            }
        }

        public string Mod
        {
            get
            {
                return mod;
            }
            set
            {
                mod = value;
            }
        }

        public string ModName
        {
            get
            {
                return modName;
            }
            set
            {
                modName = value;
            }
        }

        public string P_Code
        {
            get
            {
                return p_code;
            }
            set
            {
                p_code = value;
            }
        }

        public string N_Code
        {
            get
            {
                return n_code;
            }
            set
            {
                n_code = value;
            }
        }

        public string M_Code
        {
            get
            {
                return m_code;
            }
            set
            {
                m_code = value;
            }
        }

        public string T_Code
        {
            get
            {
                return t_code;
            }
            set
            {
                t_code = value;
            }
        }
        #endregion

        #region 方法
        public new DiaDetail Clone()
        {
            DiaDetail d = base.Clone() as DiaDetail;
            return d;
        }
        #endregion
    }
}
