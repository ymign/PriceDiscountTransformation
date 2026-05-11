using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 标本架实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_SHELF
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class Shelf : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int id = 0;
        private string barCode = "";
        private ShelfSpec shelfSpec = new ShelfSpec();
        private int capacity = 0;
        private int occupyCount = 0;
        private char isOccupy = '0';
        //private int shelfRow;
        //private int _shelfcol;
        //private int _speclayer;
        private IceBoxLayer iceboxLayer = new IceBoxLayer();
        private int disTypeId = 0;
        private int specTypeId = 0;
        private string comment = "";
        private string tumorType = "";
        #endregion

        #region 属性
        /// <summary>
        /// 架子ID
        /// </summary>
        public int ShelfID
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
        /// 规格条形码：位置信息
        /// </summary>
        public string SpecBarCode
        {
            get
            {
                return barCode;
            }
            set
            {
                barCode = value;
            }
        }

        /// <summary>
        /// 架子所在的冰箱层
        /// </summary>
        public IceBoxLayer IceBoxLayer
        {
            get
            {
                return iceboxLayer;
            }
            set
            {
                iceboxLayer = value;
            }
        }

        /// <summary>
        /// 架子的规格
        /// </summary>
        public ShelfSpec ShelfSpec
        {
            get
            {
                return shelfSpec;
            }
            set
            {
                shelfSpec = value;
            }
        }

        /// <summary>
        /// 架子中空位占用数量
        /// </summary>
        public int OccupyCount
        {
            get
            {
                return occupyCount;
            }
            set
            {
                occupyCount = value;
            }
        }

        /// <summary>
        /// 架子的容量
        /// </summary>
        public int Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                capacity = value;
            }
        }

        /// <summary>
        /// 架子是否已满
        /// </summary>
        public char IsOccupy
        {
            get
            {
                return isOccupy;
            }
            set
            {
                isOccupy = value;
            }
        }

        /// <summary>
        /// 病种类型
        /// </summary>
        public int DisTypeId
        {
            get
            {
                return disTypeId;
            }
            set
            {
                disTypeId = value;
            }
        }

        /// <summary>
        /// 标本类型
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

        /// <summary>
        /// 肿瘤类型
        /// </summary>
        public string TumorType
        {
            get
            {
                return tumorType;
            }
            set
            {
                tumorType = value;
            }
        }
        #endregion

        #region 方法
        public new Shelf Clone()
        {
            Shelf shelf = base.Clone() as Shelf;
            shelf.IceBoxLayer = this.IceBoxLayer.Clone();
            shelf.shelfSpec = this.ShelfSpec.Clone();
            return shelf;
        }
        #endregion
    }
}
