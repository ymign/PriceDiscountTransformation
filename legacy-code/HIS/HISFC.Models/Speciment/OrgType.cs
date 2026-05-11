using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 标本类型的组织类型实体,血or组织]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_ORGTYPE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class OrgType : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int _orgtypeid;
        private string _orgname;
        private short _isshowonapp;
        #endregion

        #region 方法
        public OrgType(int orgtypeid)
        {
            _orgtypeid = orgtypeid;
        }
        public OrgType()
        {

        }
        public OrgType(int orgtypeid, short isshow)
        {
            _orgtypeid = orgtypeid;
            _isshowonapp = isshow;
        }

        /// <summary>
        /// Clone 方法
        /// </summary>
        /// <returns></returns>
        public new OrgType Clone()
        {
            OrgType orgType = base.Clone() as OrgType;
            return orgType;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 组织类型ID
        /// </summary>
        public int OrgTypeID
        {
            get
            {
                return _orgtypeid;
            }
            set
            {
                _orgtypeid = value;
            }
        }
        /// <summary>
        /// 组织类型名称
        /// </summary>
        public string OrgName
        {
            get
            {
                return _orgname;
            }
            set
            {
                _orgname = value;
            }
        }
        /// <summary>
        /// 是否显示在申请单上
        /// </summary>
        public short IsShowOnApp
        {
            get
            {
                return _isshowonapp;
            }
            set
            {
                _isshowonapp = value;
            }
        }
        #endregion
    }
}
