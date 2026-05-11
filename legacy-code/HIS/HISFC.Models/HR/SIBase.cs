using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>Neusoft.HISFC.Models.HR.SIBase</br>
    /// <br>[功能描述: 社会保险基数实体类]</br>
    /// <br>[创 建 者: 赵阳]</br>
    /// <br>[创建时间: 2008-07-15]</br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class SIBase : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段

        /// <summary>
        /// 年度
        /// </summary>
        private string yearStr = "";

        /// <summary>
        /// 合同种类
        /// </summary>
        private string contractType = "";

        /// <summary>
        /// 缴费参照类型
        /// </summary>
        private string referType = "";

        /// <summary>
        /// 上限
        /// </summary>
        private decimal limitUp = 0;

        /// <summary>
        /// 下限
        /// </summary>
        private decimal limitDown = 0;

        /// <summary>
        /// 市平均工资
        /// </summary>
        private decimal avgPay = 0;

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
        /// 合同种类
        /// </summary>
        public string ContractType
        {
            get
            {
                return contractType;
            }
            set
            {
                contractType = value;
            }
        }

        /// <summary>
        /// 缴费参照类型
        /// </summary>
        public string ReferType
        {
            get
            {
                return referType;
            }
            set
            {
                referType = value;
            }
        }

        /// <summary>
        /// 上限
        /// </summary>
        public decimal LimitUp
        {
            get
            {
                return limitUp;
            }
            set
            {
                limitUp = value;
            }
        }

        /// <summary>
        /// 下限
        /// </summary>
        public decimal LimitDown
        {
            get
            {
                return limitDown;
            }
            set
            {
                limitDown = value;
            }
        }

        /// <summary>
        /// 市平均工资
        /// </summary>
        public decimal AvgPay
        {
            get
            {
                return avgPay;
            }
            set
            {
                avgPay = value;
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
        public new SIBase Clone()
        {
            SIBase siBase = base.Clone() as SIBase;

            siBase.Oper = this.Oper.Clone();

            return siBase;
        }
        #endregion
    }
}
