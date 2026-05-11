using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZWTJ
{
    public class OutGetPEPatFee
    {
        private string PAR_RECIPE_NUM;
        /// <summary>
        /// 处方号
        /// </summary>
        public string par_recipe_num
        {
            get
            {
                return PAR_RECIPE_NUM;
            }
            set
            {
                PAR_RECIPE_NUM = value;
            }
        }

        private string PAR_QTY;
        /// <summary>
        /// 数量
        /// </summary>
        public string par_qty
        {
            get
            {
                return PAR_QTY;
            }
            set
            {
                PAR_QTY = value;
            }
        }

        private string PAR_SEQUENCE_NO;
        /// <summary>
        /// 项目流水号
        /// </summary>
        public string par_sequence_no
        {
            get
            {
                return PAR_SEQUENCE_NO;
            }
            set
            {
                PAR_SEQUENCE_NO = value;
            }
        }

        private string PAR_CARD_NO;
        /// <summary>
        /// 卡号
        /// </summary>
        public string par_card_no
        {
            get
            {
                return PAR_CARD_NO;
            }
            set
            {
                PAR_CARD_NO = value;
            }
        }

        private string PAR_DOCTCODE;
        /// <summary>
        /// 医生编码
        /// </summary>
        public string par_doctcode
        {
            get
            {
                return PAR_DOCTCODE;
            }
            set
            {
                PAR_DOCTCODE = value;
            }
        }

        private string PAR_DEPTCODE;
        /// <summary>
        /// 医生所在科室编码
        /// </summary>
        public string par_deptcode
        {
            get
            {
                return PAR_DEPTCODE;
            }
            set
            {
                PAR_DEPTCODE = value;
            }
        }

        private string PAR_ITEMCODE;
        /// <summary>
        /// 项目代码
        /// </summary>
        public string par_itemcode
        {
            get
            {
                return PAR_ITEMCODE;
            }
            set
            {
                PAR_ITEMCODE = value;
            }
        }

        private string PAR_UNIT_PRICE;
        /// <summary>
        /// 单价
        /// </summary>
        public string par_unit_price
        {
            get
            {
                return PAR_UNIT_PRICE;
            }
            set
            {
                PAR_UNIT_PRICE = value;
            }
        }


        private string PAR_OWN_COST;
        /// <summary>
        /// 金额
        /// </summary>
        public string par_own_cost
        {
            get
            {
                return PAR_OWN_COST;
            }
            set
            {
                PAR_OWN_COST = value;
            }
        }

        private string PAR_EXECDEPTCODE;
        /// <summary>
        /// 执行科室
        /// </summary>
        public string par_execdeptcode
        {
            get
            {
                return PAR_EXECDEPTCODE;
            }
            set
            {
                PAR_EXECDEPTCODE = value;
            }
        }

        private string PAR_EXECDEPTNAME;
        /// <summary>
        /// 执行科室
        /// </summary>
        public string par_execdeptname
        {
            get
            {
                return PAR_EXECDEPTNAME;
            }
            set
            {
                PAR_EXECDEPTNAME = value;
            }
        }
    }
}
