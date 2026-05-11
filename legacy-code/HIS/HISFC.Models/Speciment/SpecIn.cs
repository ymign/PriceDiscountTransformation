using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 入库标本实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-12-01]<br></br>
    /// Table : SPEC_IN
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecIn : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int inId = 0;
        private DateTime dtIntime = DateTime.Now;
        private string operId = "";
        private string operName = "";
        private int subSpecId = 0;
        private int typeId = 0;
        private int specTypeId = 0;
        private Decimal count = 0.0M;
        private string unit = "";
        private int relateId = 0;
        private int boxId = 0;
        private int col = 0;
        private int row = 0;
        private string comment = "";
        private string boxBarCode = "";
        private string oper = "";
        private string specBarCode = "";
        #endregion

        #region 属性
        /// <summary>
        /// 入库ID
        /// </summary>
        public int InId
        {
            get
            {
                return inId;
            }
            set
            {
                inId = value;
            }
        }

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime InTime
        {
            get
            {
                return dtIntime;
            }
            set
            {
                dtIntime = value;
            }
        }

        /// <summary>
        /// 操作人Id
        /// </summary>
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

        /// <summary>
        /// 操作人姓名
        /// </summary>
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

        /// <summary>
        /// 出库标本ID
        /// </summary>
        public int SubSpecId
        {
            get
            {
                return subSpecId;
            }
            set
            {
                subSpecId = value;
            }
        }

        /// <summary>
        /// 组织类型Id
        /// </summary>
        public int TypeId
        {
            get
            {
                return typeId;
            }
            set
            {
                typeId = value;
            }
        }

        /// <summary>
        /// 标本类型ID
        /// </summary>
        public int SpecTypeId
        {
            get
            {
                return specTypeId;
            }
            set
            {
                specTypeId = value;
            }
        }

        /// <summary>
        /// 出库的容量/份数
        /// </summary>
        public decimal Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        /// <summary>
        /// 规格
        /// </summary>
        public string Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
            }
        }

        /// <summary>
        /// 关联ID
        /// </summary>
        public int RelateId
        {
            get
            {
                return relateId;
            }
            set
            {
                relateId = value;
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

        public int Row
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
            }
        }

        public int BoxId
        {
            get
            {
                return boxId;
            }
            set
            {
                boxId = value;
            }
        }

        public int Col
        {
            get
            {
                return col;
            }
            set
            {
                col = value;
            }
        }

        public string BoxBarCode
        {
            get
            {
                return boxBarCode;
            }
            set
            {
                boxBarCode = value;
            }
        }

        public string Oper
        {
            get
            {
                return oper;
            }
            set
            {
                oper = value;
            }
        }

        public string SubSpecBarCode
        {
            get
            {
                return specBarCode;
            }
            set
            {
                specBarCode = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// Clone 方法
        /// </summary>
        /// <returns></returns>
        public new SpecIn Clone()
        {
            SpecIn specIn = base.Clone() as SpecIn;
            return specIn;
        }
        #endregion
    }
}
