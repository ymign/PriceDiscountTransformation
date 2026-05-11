using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Pharmacy
{
    /// <summary>
    /// 麻醉药品一类精神药品使用登记表实体
    /// </summary>
    [System.Serializable]
    public class PhaComMjregister : Neusoft.FrameWork.Models.NeuObject
    {			
        /// <summary>
        /// 出库科室编码
        /// </summary>
        private string drugdeptcode = string.Empty;
        public string DrugDeptCode
        {
            get
            {
                return drugdeptcode;
            }
            set
            {
                drugdeptcode = value;
            }
        }

        /// <summary>
        /// 出库单流水号
        /// </summary>
        private string outbillcode = string.Empty;
        public string OutBillCode
        {
            get
            {
                return outbillcode;
            }
            set
            {
                outbillcode = value;
            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        private string serialcode = string.Empty;
        public string SerialCode
        {
            get
            {
                return serialcode;
            }
            set
            {
                serialcode = value;
            }
        }

        /// <summary>
        /// 批号
        /// </summary>
        private string batchno = string.Empty;
        public string BatchNo
        {
            get
            {
                return batchno;
            }
            set
            {
                batchno = value;
            }
        }

        /// <summary>
        /// 是否归还药品
        /// </summary>
        private string isback = string.Empty;
        public string IsBack
        {
            get
            {
                return isback;
            }
            set
            {
                isback = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string remark = string.Empty;
        public string Remark
        {
            get
            {
                return remark;
            }
            set
            {
                remark = value;
            }
        }

        /// <summary>
        /// 执行人
        /// </summary>
        private string executor = string.Empty;
        public string Executor
        {
            get
            {
                return executor;
            }
            set
            {
                executor = value;
            }
        }

        /// <summary>
        /// 复核人
        /// </summary>
        private string reviewer = string.Empty;
        public string Reviewer
        {
            get
            {
                return reviewer;
            }
            set
            {
                reviewer = value;
            }
        }

        /// <summary>
        /// 执行时间
        /// </summary>
        private DateTime executedate = new DateTime();
        public DateTime ExecuteDate
        {
            get
            {
                return executedate;
            }
            set
            {
                executedate = value;
            }
        }

        /// <summary>
        /// 执行时间
        /// </summary>
        private DateTime reviewdate = new DateTime();
        public DateTime ReviewDate
        {
            get
            {
                return reviewdate;
            }
            set
            {
                reviewdate = value;
            }
        }
				
    }
}
