using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 标本类型]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_TYPE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecType : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int specTypeID = 0;
        private string specTypeName = "";
        private string specColor = "";
        private OrgType orgType = new OrgType();
        private string comment = "";
        private string isShow = "";
        private int defaultCn = 0;
        private string ext1 = "";
        private string ext2 = "";
        private string ext3 = "";
        #endregion

        #region 属性
        /// <summary>
        /// 标本类型ID
        /// </summary>
        public int SpecTypeID
        {
            get
            {
                return specTypeID;
            }
            set
            {
                specTypeID = value;
            }
        }
        /// <summary>
        /// 标本类型名称
        /// </summary>
        public string SpecTypeName
        {
            get
            {
                return specTypeName;
            }
            set
            {
                specTypeName = value;
            }
        }
        /// <summary>
        /// 标本的表示颜色
        /// </summary>
        public string SpecColor
        {
            get
            {
                return specColor;
            }
            set
            {
                specColor = value;
            }
        }
        /// <summary>
        /// 标本组织类型
        /// </summary>
        public OrgType OrgType
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

        public string IsShow
        {
            get
            {
                return isShow;
            }
            set
            {
                isShow = value;
            }
        }

        public int DefaultCnt
        {
            get
            {
                return defaultCn;
            }
            set
            {
                defaultCn = value;
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
        public SpecType()
        {

        }
        public SpecType(int specid, string specname)
        {
            specTypeID = specid;
            specTypeName = specname;
        }
        public new SpecType Clone()
        {
            return base.Clone() as SpecType;
        }
        #endregion
    }
}
