using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 存储盒规格实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-10]<br></br>
    /// Table : SPEC_BOX_SPEC
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class BoxSpec : Container
    {
        #region 变量域
        private int id = 0;
        private string name = "";
        private string comment = "";
        #endregion

        #region 属性
        /// <summary>
        /// 标本盒规格ID
        /// </summary>
        public int BoxSpecID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// 标本规格盒名称
        /// </summary>
        public string BoxSpecName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
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
        public new BoxSpec Clone()
        {
            BoxSpec boxSpec = base.Clone() as BoxSpec;
            return boxSpec;
        }
        #endregion
    }
}
