using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>全类名</br>
    /// <br>[功能描述: 奖惩模板实体]</br>
    /// <br>[创 建 者: 赵阳]</br>
    /// <br>[创建时间: 2008-09-22]</br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class RFSample : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段

        /// <summary>
        /// 相关人员
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject person = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 奖惩分类
        /// </summary>
        private string rfType = string.Empty;

        /// <summary>
        /// 模板内容
        /// </summary>
        private string sampleContent = string.Empty;

        /// <summary>
        /// 操作环境
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion

        #region 属性

        /// <summary>
        /// 相关人员
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Person
        {
            get
            {
                return person;
            }
            set
            {
                person = value;
            }
        }

        /// <summary>
        /// 奖惩分类
        /// </summary>
        public string RfType
        {
            get
            {
                return rfType;
            }
            set
            {
                rfType = value;
            }
        }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string SampleContent
        {
            get
            {
                return sampleContent;
            }
            set
            {
                sampleContent = value;
            }
        }

        /// <summary>
        /// 操作环境
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
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

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new RFSample Clone()
        {
            RFSample rfSample = base.Clone() as Neusoft.HISFC.Models.HR.RFSample;

            rfSample.Person = this.Person.Clone();
            rfSample.Oper = this.Oper.Clone();

            return rfSample;
        }

        #endregion
    }
}
