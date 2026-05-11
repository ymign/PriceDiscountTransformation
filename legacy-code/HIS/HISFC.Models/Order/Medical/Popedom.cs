using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Models.Order.Medical
{
    /// <summary>
    /// Popedom <br></br>
    /// [功能描述: 医疗权限实体类]<br></br>
    /// [创 建 者: 孙久海]<br></br>
    /// [创建时间: 2008-07-23]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class Popedom : NeuObject
    {
        /// <summary>
        /// 构造函数

        /// </summary>
        public Popedom()
        {

        }

        #region 字段

        /// <summary>
        /// 医生代码
        /// </summary>
        private string emplCode;
        /// <summary>
        /// 医生名称
        /// </summary>
        private string emplName;
        /// <summary>
        /// 权限类型
        /// </summary>
        private NeuObject popedomType;
        /// <summary>
        /// 权限
        /// </summary>
        private NeuObject popedoms;
        /// <summary>
        /// 审核标识
        /// </summary>
        private string checkFlag;

        #endregion

        #region 属性


        /// <summary>
        /// 医生代码
        /// </summary>
        public string EmplCode
        {
            get
            {
                if (emplCode == null)
                {
                    emplCode = string.Empty;
                }
                return emplCode;
            }
            set { emplCode = value; }
        }
        /// <summary>
        /// 医生名称
        /// </summary>
        public string EmplName
        {
            get
            {
                if (emplName == null)
                {
                    emplName = string.Empty;
                }
                return emplName;
            }
            set { emplName = value; }
        }
        /// <summary>
        /// 权限类型
        /// </summary>
        public NeuObject PopedomType
        {
            get
            {
                if (popedomType == null)
                {
                    popedomType = new NeuObject();
                } 
                return popedomType;
            }
            set { popedomType = value; }
        }
        /// <summary>
        /// 权限
        /// </summary>
        public NeuObject Popedoms
        {
            get
            {
                if (popedoms == null)
                {
                    popedoms = new NeuObject();
                } 
                return popedoms;
            }
            set { popedoms = value; }
        }
        /// <summary>
        /// 审核标识
        /// </summary>
        public string CheckFlag
        {
            get
            {
                if (checkFlag == null)
                {
                    checkFlag = string.Empty;
                } 
                return checkFlag;
            }
            set { checkFlag = value; }
        }

        #endregion

        #region 方法

        public new Popedom Clone()
        {
            Popedom popedom = base.Clone() as Popedom;
            popedom.popedomType = this.PopedomType.Clone();
            popedom.popedoms = this.Popedoms.Clone();

            return popedom;
        }

        #endregion
    }
}
