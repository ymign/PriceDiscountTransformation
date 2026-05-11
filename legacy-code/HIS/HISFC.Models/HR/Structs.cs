using System;
using System.Collections.Generic;
using System.Text;



namespace Neusoft.HISFC.Models.HR
{
    ///==================================================说明 begin============================
    /// <summary>
    /// <br></br>
    /// <br>[功能描述: 该文件存放各种结构体]</br>
    /// <br>[创 建 者: 赵阳]</br>
    /// <br>[创建时间: 2008-07-21]</br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    /// ==================================================说明 end============================


    //该结构体表示每一种保险对应的缴费金额
    [Serializable]
    public struct SIFee
    {
        #region 字段

        /// <summary>
        /// 保险类型
        /// </summary>
        private string siType;

        /// <summary>
        /// 个人缴费金额
        /// </summary>
        private decimal personalFee;

        #endregion

        #region 属性

        /// <summary>
        /// 保险类型
        /// </summary>
        public string SiType
        {
            get
            {
                return siType;
            }
            set
            {
                siType = value;
            }
        }

        /// <summary>
        /// 个人缴费金额
        /// </summary>
        public decimal PersonalFee
        {
            get
            {
                return personalFee;
            }
            set
            {
                personalFee = value;
            }
        }
        #endregion
    }//end of SIMoney

    //考勤扣款比例和费用
    [Serializable]
    public struct WCDeductFee
    {
        #region 字段

        /// <summary>
        /// 工资项目
        /// </summary>
        private string payItem;

        /// <summary>
        /// 扣款比例
        /// </summary>
        private decimal deductScale;

        /// <summary>
        /// 扣款金额
        /// </summary>
        private decimal deductMoney;

        #endregion

        #region 属性

        /// <summary>
        /// 工资项目
        /// </summary>
        public string PayItem
        {
            get
            {
                return payItem;
            }
            set
            {
                payItem = value;
            }
        }

        /// <summary>
        /// 扣款比例
        /// </summary>
        public decimal DeductScale
        {
            get
            {
                return deductScale;
            }
            set
            {
                deductScale = value;
            }
        }

        /// <summary>
        /// 扣款金额
        /// </summary>
        public decimal DeductMoney
        {
            get
            {
                return deductMoney;
            }
            set
            {
                deductMoney = value;
            }
        }

        #endregion

    }//end of WCDeductFee

}
