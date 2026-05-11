using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 标本用户实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_SOURCE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    class SpecUser : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int userId;
        private string _username;
        private string _idcardno;
        private string _companyname;
        private string _realname;
        private DateTime? _birth;
        private string _resachieve;
        private string _email;
        private string _tel;
        public string _userid;
        #endregion

        #region 属性
        public int ID
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
            }
        }
        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
        public string IdCardNo
        {
            get
            {
                return _idcardno;
            }
            set
            {
                _idcardno = value;
            }
        }
        public string CompanyName
        {
            get
            {
                return _companyname;
            }
            set
            {
                _companyname = value;
            }
        }
        public string RealName
        {
            get
            {
                return _realname;
            }
            set
            {
                _realname = value;
            }
        }
        public string ResAchieve
        {
            get
            {
                return _resachieve;
            }
            set
            {
                _resachieve = value;
            }
        }
        public DateTime? BirthDay
        {
            get
            {
                return _birth;
            }
            set
            {
                _birth = value;
            }
        }
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }
        public string Tel
        {
            get
            {
                return _tel;
            }
            set
            {
                _tel = value;
            }
        }
        public string UserId
        {
            get
            {
                return _userid;
            }
            set
            {
                _userid = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// Clone 方法
        /// </summary>
        /// <returns></returns>
        public new SpecUser Clone()
        {
            SpecUser specUser = base.Clone() as SpecUser;
            return specUser;
        }
        #endregion
    }
}
