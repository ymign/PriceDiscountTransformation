using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.Fee
{
    public class SIBalanceList:Neusoft.FrameWork.Models.NeuObject
    {

        //INPATIENTNO, FEECODE, COST, BALANCEDATE, VALID_STATE, TYPE
        string inpatientNo = string.Empty;
        string feeCode = string.Empty;
        decimal cost = 0;
        DateTime balancedate = new DateTime();
        string validState = string.Empty;
        string type = string.Empty;

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string InpatientNo        
        {
            get
            {
                return inpatientNo;
            }
            set
            {
                inpatientNo = value;
            }
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string FeeCode
        {
            get
            {
                return feeCode;
            }
            set
            {
                feeCode = value;
            }
        }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
            }
        }
        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime Balancedate
        {
            get
            {
                return balancedate;
            }
            set
            {
                balancedate = value;
            }
        }
        /// <summary>
        /// 有效标准 1有效 0无效
        /// </summary>
        public string ValidState
        {
            get
            {
                return validState;
            }
            set
            {
                validState = value;
            }
        }
        /// <summary>
        /// 类别 C
        /// </summary>
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                type= value;
            }
        }

    }
}
