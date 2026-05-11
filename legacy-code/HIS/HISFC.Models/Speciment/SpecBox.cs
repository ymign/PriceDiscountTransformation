using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 标本盒实体类]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_BOX
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecBox : Neusoft.FrameWork.Models.NeuObject
    {
        #region 域变量
        private int boxID = 0;
        private string boxCode = "";
        private int colNum = 0;
        private int rowNum = 0;
        private int desCapCol = 0;
        private int desCapRow = 0;
        private int desCapID = 0;
        private int desCapSubLayer = 0;
        private DiseaseType dis = new DiseaseType();
        private char desCapType = '0';
        private int disTypeID = 0;
        private string disName = "";
        private BoxSpec boxSpec = new BoxSpec();
        private int capacity = 0;
        private string comment = "";
        private int occupyCount = 0;
        private int orgOrBlood = 0;
        private int specTypeID = 0;
        private char inIceBox = '0';
        private char isOccupy = '0';
        private string specUse = "";

        #endregion

        #region 属性
        public SpecBox()
        {

        }
        /// <summary>
        /// 标本盒ID
        /// </summary>
        public int BoxId
        {
            get
            {
                return boxID;
            }
            set
            {
                boxID = value;
            }
        }
        /// <summary>
        /// 标本盒条形码
        /// </summary>
        public string BoxBarCode
        {
            get
            {
                return boxCode;
            }
            set
            {
                boxCode = value;
            }
        }
        /// <summary>
        /// 盒子列
        /// </summary>
        public int ColNum
        {
            get
            {
                return colNum;
            }
            set
            {
                colNum = value;
            }
        }
        /// <summary>
        /// 盒子行
        /// </summary>
        public int RowNum
        {
            get
            {
                return rowNum;
            }
            set
            {
                rowNum = value;
            }
        }
        /// <summary>
        /// 盒子所放容器行
        /// </summary>
        public int DesCapRow
        {
            get
            {
                return desCapRow;
            }
            set
            {
                desCapRow = value;
            }
        }
        /// <summary>
        /// 盒子所放容器列
        /// </summary>
        public int DesCapCol
        {
            get
            {
                return desCapCol;
            }
            set
            {
                desCapCol = value;
            }
        }
        /// <summary>
        /// 盒子所放容器ID
        /// </summary>
        public int DesCapID
        {
            get
            {
                return desCapID;
            }
            set
            {
                desCapID = value;
            }
        }
        /// <summary>
        /// 盒子所放容器层
        /// </summary>
        public int DesCapSubLayer
        {
            get
            {
                return desCapSubLayer;
            }
            set
            {
                desCapSubLayer = value;
            }
        }
        /// <summary>
        /// 盒内置标本类型
        /// </summary>
        public DiseaseType DiseaseType
        {
            get
            {
                return dis;
            }
            set
            {
                dis = value;
            }
        }
        /// <summary>
        /// 目标容器类型，s:架子，i:冰箱层
        /// </summary>
        public char DesCapType
        {
            get
            {
                return desCapType;
            }
            set
            {
                desCapType = value;
            }
        }
        /// <summary>
        /// 标本盒存放的病种类型
        /// </summary>
        public int DisTypeId
        {
            get
            {
                return disTypeID;
            }
            set
            {
                disTypeID = value;
            }
        }
        /// <summary>
        /// 病种类型名称
        /// </summary>
        public string DisTypeName
        {
            get
            {
                return disName;
            }
            set
            {
                disName = value;
            }
        }

        /// <summary>
        /// 盒子规格
        /// </summary>
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
        /// 容量
        /// </summary>
        public int Capacity
        {
            get
            {
                return this.capacity;
            }
            set
            {
                this.capacity = value;
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
        /// 占用的格子
        /// </summary>
        public int OccupyCount
        {
            get
            {
                return this.occupyCount;
            }
            set
            {
                this.occupyCount = value;
            }
        }

        /// <summary>
        /// 盒子是否已满
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
        /// 存放的是组织Or血
        /// </summary>
        public int OrgOrBlood
        {
            get
            {
                return this.orgOrBlood;
            }
            set
            {
                this.orgOrBlood = value;
            }
        }

        /// <summary>
        /// 存放的标本类型
        /// </summary>
        public int SpecTypeID
        {
            get
            {
                return this.specTypeID;
            }
            set
            {
                this.specTypeID = value;
            }
        }

        /// <summary>
        /// 0:在冰箱中，1在机动冰箱中
        /// </summary>
        public char InIceBox
        {
            get
            {
                return inIceBox;
            }
            set
            {
                inIceBox = value;
            }

        }

        /// <summary>
        /// 标本盒专用，N NPC专用，8 863 专用
        /// </summary>
        public string SpecialUse
        {
            get
            {
                return specUse;
            }
            set
            {
                specUse = value;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// clone
        /// </summary>
        /// <returns></returns>
        public new SpecBox Clone()
        {
            SpecBox box = base.Clone() as SpecBox;
            box.DiseaseType = this.DiseaseType.Clone();
            box.BoxSpec = this.BoxSpec.Clone();
            return box;
        }

        /// <summary>
        /// 查看2个标本是否存放的同一种类型标本，病种
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool CheckSaveSameType(SpecBox box)
        {
            if (box.DiseaseType.DisTypeID != this.DiseaseType.DisTypeID)
            {
                return false;
            }
            if (box.OrgOrBlood != this.OrgOrBlood)
            {
                return false;
            }
            if (box.SpecTypeID != this.SpecTypeID)
            {
                return false;
            }
            return true;
        }
        #endregion

    }
}
