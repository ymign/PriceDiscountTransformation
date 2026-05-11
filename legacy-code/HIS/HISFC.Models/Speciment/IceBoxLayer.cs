using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 冰箱层对应的实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_ICEBOXLAYER
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class IceBoxLayer : Container
    {
        #region 变量域
        private string bloodOrOrgID = "";
        private string comment = "";
        private DiseaseType disType = new DiseaseType();
        private IceBox icebox = new IceBox();
        private short isOccupy = 0;
        private int layerID = 0;
        private short layerNum = 0;
        private char saveType = 'B';
        private int specID = 0;
        private int specTypeId = 0;

        #endregion

        #region 属性
        /// <summary>
        /// 存放的血还是组织
        /// </summary>
        public string BloodOrOrgId
        {
            get
            {
                return this.bloodOrOrgID;
            }
            set
            {
                this.bloodOrOrgID = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                this.comment = value;
            }
        }

        /// <summary>
        /// 存放的病种类型
        /// </summary>
        public DiseaseType DiseaseType
        {
            get
            {
                return this.disType;
            }
            set
            {
                this.disType = value;
            }
        }

        /// <summary>
        /// 所放的冰箱
        /// </summary>
        public IceBox IceBox
        {
            get
            {
                return this.icebox;
            }
            set
            {
                this.icebox = value;
            }
        }

        /// <summary>
        /// 是否已满
        /// </summary>
        public short IsOccupy
        {
            get
            {
                return this.isOccupy;
            }
            set
            {
                this.isOccupy = value;
            }
        }

        /// <summary>
        /// 冰箱层ID
        /// </summary>
        public int LayerId
        {
            get
            {
                return this.layerID;
            }
            set
            {
                this.layerID = value;
            }
        }

        /// <summary>
        /// 冰箱的第几层
        /// </summary>
        public short LayerNum
        {
            get
            {
                return this.layerNum;
            }
            set
            {
                this.layerNum = value;
            }
        }

        /// <summary>
        /// 保存的对象，是架子Or直接存放标本盒
        /// </summary>
        public char SaveType
        {
            get
            {
                return this.saveType;
            }
            set
            {
                this.saveType = value;
            }
        }

        /// <summary>
        /// 存放对象的规格ID
        /// </summary>
        public int SpecID
        {
            get
            {
                return this.specID;
            }
            set
            {
                this.specID = value;
            }
        }

        /// <summary>
        /// 存放的标本类型ID
        /// </summary>
        public int SpecTypeID
        {
            get
            {
                return this.specTypeId;
            }
            set
            {
                this.specTypeId = value;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 判断2层设置是否一致
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool CheckSameSpec(IceBoxLayer layer)
        {
            if (base.Col != layer.Col)
            {
                return false;
            }
            if (this.DiseaseType.DisTypeID != layer.DiseaseType.DisTypeID)
            {
                return false;
            }
            if (this.SpecTypeID != layer.SpecTypeID)
            {
                return false;
            }
            if (base.Height != layer.Height)
            {
                return false;
            }
            if (this.IceBox.IceBoxId != layer.IceBox.IceBoxId)
            {
                return false;
            }
            if (base.Row != layer.Row)
            {
                return false;
            }
            if (this.SaveType != layer.SaveType)
            {
                return false;
            }
            if (this.SpecID != layer.SpecID)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Clone方法
        /// </summary>
        /// <returns></returns>
        public new IceBoxLayer Clone()
        {
            IceBoxLayer layer = base.Clone() as IceBoxLayer;
            layer.IceBox = this.IceBox.Clone();
            layer.DiseaseType = this.DiseaseType.Clone();
            return layer;
        }
        #endregion 
    }
}
