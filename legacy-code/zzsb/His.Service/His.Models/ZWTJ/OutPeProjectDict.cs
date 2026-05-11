using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZWTJ
{
    public class OutPeProjectDict
    {
        private string item_code;
        /// <summary>
        /// 项目编号
        /// </summary>
        public string ITEM_CODE
        {
            get
            {
                return item_code;
            }
            set
            {
                item_code = value;
            }
        }

        private string item_name;
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ITEM_NAME
        {
            get
            {
                return item_name;
            }
            set
            {
                item_name = value;
            }
        }

        private string unit_price;
        /// <summary>
        /// 单价
        /// </summary>
        public string UNIT_PRICE
        {
            get
            {
                return unit_price;
            }
            set
            {
                unit_price = value;
            }
        }

        private string exedept_code;
        /// <summary>
        /// 执行科室
        /// </summary>
        public string EXEDEPT_CODE
        {
            get
            {
                return exedept_code;
            }
            set
            {
                exedept_code = value;
            }
        }


        private string package_code;
        /// <summary>
        /// 组套代码
        /// </summary>
        public string PACKAGE_CODE
        {
            get
            {
                return package_code;
            }
            set
            {
                package_code = value;
            }
        }

        private string qty;
        /// <summary>
        /// 项目数量
        /// </summary>
        public string QTY
        {
            get
            {
                return qty;
            }
            set
            {
                qty = value;
            }
        }

        private string package_name;
        /// <summary>
        /// 组套名称
        /// </summary>
        public string PACKAGE_NAME
        {
            get
            {
                return package_name;
            }
            set
            {
                package_name = value;
            }
        }


        private string recipe_no;
        /// <summary>
        /// 处方号
        /// </summary>
        public string RECIPE_NO
        {
            get
            {
                return recipe_no;
            }
            set
            {
                recipe_no = value;
            }
        }

        private string card_no;
        /// <summary>
        /// 卡号
        /// </summary>
        public string CARD_NO
        {
            get
            {
                return card_no;
            }
            set
            {
                card_no = value;
            }
        }

        private string clinic_code;
        /// <summary>
        /// 门诊流水号
        /// </summary>
        public string CLINIC_CODE
        {
            get
            {
                return clinic_code;
            }
            set
            {
                clinic_code = value;
            }
        }

        private string name;
        /// <summary>
        /// 名字
        /// </summary>
        public string NAME
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private string fee_date;
        /// <summary>
        /// 收费时间
        /// </summary>
        public string FEE_DATE
        {
            get
            {
                return fee_date;
            }
            set
            {
                fee_date = value;
            }
        }

        private string reg_dpcd;
        /// <summary>
        /// 开单科室
        /// </summary>
        public string REG_DPCD
        {
            get
            {
                return reg_dpcd;
            }
            set
            {
                reg_dpcd = value;
            }
        }

        private string pay_flag;
        /// <summary>
        /// 收费状态
        /// </summary>
        public string PAY_FLAG
        {
            get
            {
                return pay_flag;
            }
            set
            {
                pay_flag = value;
            }
        }

        private string noback_num;
        /// <summary>
        /// 可退数量
        /// </summary>
        public string NOBACK_NUM
        {
            get
            {
                return noback_num;
            }
            set
            {
                noback_num = value;
            }
        }

        private string own_cost;
        /// <summary>
        /// 金额
        /// </summary>
        public string OWN_COST
        {
            get
            {
                return own_cost;
            }
            set
            {
                own_cost = value;
            }
        }

        private string sequence_no;
        /// <summary>
        /// 项目流水号
        /// </summary>
        public string SEQUENCE_NO
        {
            get
            {
                return sequence_no;
            }
            set
            {
                sequence_no = value;
            }
        }

        private string trans_type;
        /// <summary>
        /// 退费状态
        /// </summary>
        public string TRANS_TYPE
        {
            get
            {
                return trans_type;
            }
            set
            {
                trans_type = value;
            }
        }
    }
}
