using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Container<br></br>
    /// [功能描述: 标本架规格]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-10]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class ShelfSpec : Container
    {
        #region 变量域
        private int shelfSpecID = 0;
        private string shelfSpecName = "";
        private BoxSpec boxSpec = new BoxSpec();
        private string comment = "";
        private string iceBoxType = "";
        #endregion

        #region 属性
        /// <summary>
        /// 架子规格ID
        /// </summary>
        public int ShelfSpecID
        {
            get
            {
                return shelfSpecID;
            }
            set
            {
                shelfSpecID = value;
            }
        }

        public BoxSpec BoxSpec
        {
            get
            {
                return boxSpec;
            }
            set
            {
                boxSpec = value;
            }
        }

        /// <summary>
        /// 架子规格名称
        /// </summary>
        public string ShelfSpecName
        {
            get
            {
                return shelfSpecName;
            }
            set
            {
                shelfSpecName = value;
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

        public string IceBoxType
        {
            get
            {
                return iceBoxType;
            }
            set
            {
                iceBoxType = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// clone方法
        /// </summary>
        /// <returns></returns>
        public new ShelfSpec Clone()
        {
            ShelfSpec shelfSpec = base.Clone() as ShelfSpec;
            shelfSpec.BoxSpec = this.BoxSpec.Clone();
            return shelfSpec;
        }
        #endregion
    }
}
