using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.YYSS
{
    /// <summary>
    /// 医嘱类别字典表
    /// </summary>
    public class OrderType
    {
        private string empi;
        /// <summary>
        /// 患者主索引
        /// </summary>
        public string EMPI
        {
            get
            {
                return empi;
            }
            set
            {
                empi = value;
            }
        }

        private string order_class_code=string.Empty;

        /// <summary>
        /// 类别代码
        /// </summary>
        public string ORDER_CLASS_CODE
        {
            get
            {
                return this.order_class_code;
            }
            set
            {
                this.order_class_code = value;
            }
        }

        private string order_class_name=string.Empty;
        /// <summary>
        /// 类别名称  饮食医嘱、肠内肠外制剂的医嘱
        /// </summary>
        public string ORDER_CLASS_NAME
        {
            get
            {
                return this.order_class_name;
            }
            set
            {
                this.order_class_name = value;
            }
        }

        private string order_code=string.Empty;
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ORDER_CODE
        {
            get
            {
                return order_code;
            }
            set
            {
                order_code = value;
            }
        }

        private string order_name=string.Empty;
        /// <summary>
        ///项目名称
        /// </summary>
        public string ORDER_NAME
        {
            get 
            {
                return order_name;
            }
            set
            {
                order_name = value;
            }
        }

        private string order_spec=string.Empty;
        /// <summary>
        /// 项目规格
        /// </summary>
        public string ORDER_SPEC
        {
            get
            {
                return order_spec;
            }
            set
            {
                order_spec = value;
            }
        }

       

        private string units=string.Empty;
        /// <summary>
        ///单位
        /// </summary>
        public string UNITS
        {
            get
            {
                return units;
            }
            set
            {
                units = value;
            }
        }

    }
}
