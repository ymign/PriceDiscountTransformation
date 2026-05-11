using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 冰箱实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_ICEBOX
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class IceBox : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量域
        private int iceboxid = 0;
        private int layernum = 0;
        private short occupy = 0;
        private string iceboxtypeid = "";
        private string iceboxName = "";
        private string comment = "";
        private string orgOrBlood = "";
        private string specTypeID = "";
        private string useStatus = "";

        #endregion

        #region 构造函数
        public IceBox()
        {

        }
        public IceBox(int iceid)
        {
            iceboxid = iceid;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 冰箱Id
        /// </summary>
        public int IceBoxId
        {
            get
            {
                return iceboxid;
            }
            set
            {
                iceboxid = value;
            }
        }
        /// <summary>
        /// 冰箱有多少层
        /// </summary>
        public int LayerNum
        {
            get
            {
                return layernum;
            }
            set
            {
                layernum = value;
            }
        }
        /// <summary>
        /// 冰箱是否被占用
        /// </summary>
        public short IsOccupy
        {
            get
            {
                return occupy;
            }

            set
            {
                occupy = value;
            }
        }
        /// <summary>
        /// 冰箱类型
        /// </summary>
        public string IceBoxTypeId
        {
            get
            {
                return iceboxtypeid;
            }
            set
            {
                iceboxtypeid = value;
            }
        }
        /// <summary>
        /// 冰箱名称
        /// </summary>
        public string IceBoxName
        {
            get
            {
                return iceboxName;
            }
            set
            {
                iceboxName = value;
            }
        }

        /// <summary>
        /// 存放的是血还是组织或其他
        /// </summary>
        public string OrgOrBlood
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
        /// 标本类型
        /// </summary>
        public string SpecTypeId
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
        /// 冰箱使用状态
        /// </summary>
        public string UseStaus
        {
            get
            {
                return this.useStatus;
            }
            set
            {
                this.useStatus = value;
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
        #endregion

        #region Clone方法
        /// <summary>
        /// Clone 
        /// </summary>
        /// <returns></returns>
        public new IceBox Clone()
        {
            IceBox iceBox = base.Clone() as IceBox;
            return iceBox;
        }
        #endregion
    }
}
