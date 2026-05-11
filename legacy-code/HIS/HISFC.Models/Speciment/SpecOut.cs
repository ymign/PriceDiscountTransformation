using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 出库标本实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-12-01]<br></br>
    /// Table : SPEC_OUT
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecOut : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int outId = 0;
        private DateTime dtOutDate = DateTime.Now;
        private string operId = "";
        private string operName = "";
        private int subSpecId = 0;
        private int typeId = 0;
        private int specTypeId = 0;
        private Decimal count = 0.0M;
        private string unit = "";
        private int relateId = 0;
        private int boxId = 0;
        private int boxRow = 0;
        private int boxCol = 0;
        private string oper = "";
        private string comment = "";
        private string boxBarCode = "";
        private string specBarCode = "";
        private string isOut = "0";
        private int specId = 0;
        private string isReturnable = "1";
        #endregion

        #region 属性
        /// <summary>
        /// 出库ID
        /// </summary>
        public int OutId
        {
            get
            {
                return outId;
            }
            set
            {
                outId = value;
            }
        }

        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime OutDate
        {
            get
            {
                return dtOutDate;
            }
            set
            {
                dtOutDate = value;
            }
        }

        /// <summary>
        /// 操作人ID
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

        public int BoxCol
        {
            get
            {
                return boxCol;
            }
            set
            {
                boxCol = value;
            }
        }

        public int BoxRow
        {
            get
            {
                return boxRow;
            }
            set
            {
                boxRow = value;
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

        public string IsOut
        {
            get
            {
                return isOut;
            }
            set
            {
                isOut = value;
            }
        }

        public int SpecId
        {
            get
            {
                return specId;
            }
            set
            {
                specId = value;
            }
        }

        public string IsRetuanAble
        {
            get
            {
                return isReturnable;
            }
            set
            {
                isReturnable = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// Clone 方法
        /// </summary>
        /// <returns></returns>
        public new SpecOut Clone()
        {
            SpecOut specOut = base.Clone() as SpecOut;
            return specOut;
        }
        #endregion
    }
}
