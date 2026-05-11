using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 分装条码表]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2010-02-22]<br></br>
    /// Table : SPEC_SUBBARCODE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecBarCode : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private string disType = "";
        private string disAbrre = "";
        private string specType = "";
        private string specTypeAbrre = "";
        private string sequence = "";
        private string other = "";
        private string orgOrBlood = "";
        #endregion

        #region 属性
        /// <summary>
        /// 病种类型
        /// </summary>
        public string DisType
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
        /// 病种缩写
        /// </summary>
        public string DisAbrre
        {
            get
            {
                return disAbrre;
            }
            set
            {
                disAbrre = value;
            }
        }

        /// <summary>
        /// 标本类型
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
        /// 标本类型缩写
        /// </summary>
        public string SpecTypeAbrre
        {
            get
            {
                return specTypeAbrre;
            }
            set
            {
                specTypeAbrre = value;
            }
        }

        /// <summary>
        /// 存储的最大序列号
        /// </summary>
        public string Sequence
        {
            get
            {
                return sequence;
            }
            set
            {
                sequence = value;
            }
        }

        /// <summary>
        /// 其他
        /// </summary>
        public string Other
        {
            get
            {
                return other;
            }
            set
            {
                other = value;
            }
        }

        public string OrgOrBld
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
        #endregion

        #region 方法
        /// <summary>
        /// Clone 方法
        /// </summary>
        /// <returns></returns>
        public new SpecBarCode Clone()
        {
            SpecBarCode specBarCode = base.Clone() as SpecBarCode;
            return specBarCode;
        }
        #endregion
    }
}
