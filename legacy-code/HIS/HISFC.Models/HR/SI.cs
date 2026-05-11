using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// Neusoft.HISFC.Models.HR.SocietyInsurance<br></br>
    /// [功能描述: 社会保险实体]<br></br>
    /// [创 建 者: 赵阳]<br></br>
    /// [创建时间: 2008-07-03]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class SI : Neusoft.FrameWork.Models.NeuObject //, HISFC.Object.Base.IValidState HISFC .Object .Base .IValid 
    {
        #region 字段

        /// <summary>
        /// 年度
        /// </summary>
        private string yearStr = "";

        /// <summary>
        /// 合同类型
        /// </summary>
        //private string contractType = "";

        /// <summary>
        /// 保险种类(失业、工伤、养老、医疗等)
        /// </summary>
        private string insuranceType = "";

        /// <summary>
        /// 保险个人缴费比例
        /// </summary>
        private decimal personalScale = 0;

        /// <summary>
        /// 保险个人缴费金额
        /// </summary>
        private decimal personalAmount = 0;

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion

        #region 属性

        /// <summary>
        /// 年度
        /// </summary>
        public string YearStr
        {
            get
            {
                return yearStr;
            }
            set
            {
                yearStr = value;
            }
        }

        /// <summary>
        /// 合同类型
        /// </summary>
        //public string ContractType
        //{
        //    get
        //    {
        //        return contractType;
        //    }
        //    set
        //    {
        //        contractType = value;
        //    }
        //}

        /// <summary>
        /// 保险种类(失业、工伤、养老、医疗等)
        /// </summary>
        public string InsuranceType
        {
            get
            {
                return insuranceType;
            }
            set
            {
                insuranceType = value;
            }
        }

        /// <summary>
        /// 保险个人缴费比例
        /// </summary>
        public decimal PersonalScale
        {
            get
            {
                return personalScale;
            }
            set
            {
                personalScale = value;
            }
        }

        /// <summary>
        /// 保险个人缴费金额
        /// </summary>
        public decimal PersonalAmount
        {
            get
            {
                return personalAmount;
            }
            set
            {
                personalAmount = value;
            }
        }

        /// <summary>
        /// 操作员
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
        public new SI Clone()
        {
            SI si = base.Clone() as SI;

            si.Oper = this.Oper.Clone();

            return si;
        }
        #endregion
    }
}
