using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Pharmacy
{
    /// <summary>
    /// 西药库日结日清实体
    /// </summary>
    [System.Serializable]
    public class XYKStorageDaily : Neusoft.FrameWork.Models.NeuObject
    {
        /// <summary>
        /// 日结操作时间
        /// </summary>
        private DateTime dailydate = new DateTime();
        public DateTime DailyDate
        {
            get
            {
                return dailydate;
            }
            set
            {
                dailydate = value;
            }
        }

        /// <summary>
        /// 摘要
        /// </summary>
        private string abstractinfo = string.Empty;
        public string AbstractInfo
        {
            get
            {
                return abstractinfo;
            }
            set
            {
                abstractinfo = value;
            }
        }

        /// <summary>
        /// 入库数量
        /// </summary>
        private decimal input_num = 0;
        public decimal Input_Num
        {
            get
            {
                return input_num;
            }
            set
            {
                this.input_num = value;
            }
        }

        /// <summary>
        /// 出库数量
        /// </summary>
        private decimal output_num = 0;
        public decimal OutPut_Num
        {
            get
            {
                return output_num;
            }
            set
            {
                this.output_num = value;
            }
        }

        /// <summary>
        /// 批号
        /// </summary>
        private string batch_no = string.Empty;
        public string Batch_No
        {
            get
            {
                return batch_no;
            }
            set
            {
                batch_no = value;
            }
        }

        /// <summary>
        /// 有效期
        /// </summary>
        private DateTime valid_date = new DateTime();
        public DateTime Valid_Date
        {
            get
            {
                return valid_date;
            }
            set
            {
                valid_date = value;
            }
        }

        /// <summary>
        /// 批号结存数
        /// </summary>
        private decimal batch_nosum = 0;
        public decimal Batch_NoSum
        {
            get
            {
                return batch_nosum;
            }
            set
            {
                this.batch_nosum = value;
            }
        }

        /// <summary>
        /// 总结存数
        /// </summary>
        private decimal tot_sum = 0;
        public decimal Tot_Sum
        {
            get
            {
                return tot_sum;
            }
            set
            {
                this.tot_sum = value;
            }
        }

        /// <summary>
        /// 交班人
        /// </summary>
        private string handoverperson = string.Empty;
        public string Handoverperson
        {
            get
            {
                return handoverperson;
            }
            set
            {
                handoverperson = value;
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
        /// 药品编码
        /// </summary>
        private string drug_code = string.Empty;
        public string Drug_Code
        {
            get
            {
                return drug_code;
            }
            set
            {
                drug_code = value;
            }
        }

        /// <summary>
        /// 顺序号
        /// </summary>
        private string orderno = string.Empty;
        public string OrderNo
        {
            get
            {
                return orderno;
            }
            set
            {
                orderno = value;
            }
        }

    }
}
