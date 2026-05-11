using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Fee.Outpatient
{
    /// <summary>
    /// 指引单实体
    /// </summary>
    [System.Serializable]
    public class MZGuide : Neusoft.FrameWork.Models.NeuObject
    {

        //ITEM_CODE  ITEM_NAME  MO_ORDER  EXEC_DPCD  DEPT_NAME  CLINIC_CODE  RECIPE_NO 
        //CLASS_CODE  DRUGED_TERMINAL  SEND_TERMINAL  SUBJOB_FLAG  ADDRRESS  NOTE

        #region 属性

        /// <summary>
        /// 项目
        /// </summary>
        private string item_code = string.Empty;
        public string Item_Code
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

        /// <summary>
        /// 项目名称
        /// </summary>
        private string item_name = string.Empty;
        public string Item_Name
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

        /// <summary>
        /// order
        /// </summary>
        private string mo_order = string.Empty;
        public string MO_Order
        {
            get
            {
                return mo_order;
            }
            set
            {
                mo_order = value;
            }
        }

        /// <summary>
        /// 执行科室代码
        /// </summary>
        private string exec_dpcd = string.Empty;
        public string Exec_Dpcd
        {
            get
            {
                return exec_dpcd;
            }
            set
            {
                exec_dpcd = value;
            }
        }

        /// <summary>
        /// 执行科室名称
        /// </summary>
        private string exec_dpnm = string.Empty;
        public string Exec_Dpnm
        {
            get
            {
                return exec_dpnm;
            }
            set
            {
                exec_dpnm = value;
            }
        }


        /// <summary>
        /// 门诊号
        /// </summary>
        private string clinic_code = string.Empty;
        public string Clinic_Code
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


        /// <summary>
        /// 处方号
        /// </summary>
        private string recipe_no = string.Empty;
        public string Recipe_NO
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


        /// <summary>
        /// 系统代码
        /// </summary>
        private string class_code = string.Empty;
        public string Class_Code
        {
            get
            {
                return class_code;
            }
            set
            {
                class_code = value;
            }
        }


        /// <summary>
        /// 配药台
        /// </summary>
        private string drug_terminal = string.Empty;
        public string Drug_Terminal
        {
            get
            {
                return drug_terminal;
            }
            set
            {
                drug_terminal = value;
            }
        }


        /// <summary>
        /// 发药窗口
        /// </summary>
        private string send_terminal = string.Empty;
        public string Send_Terminal
        {
            get
            {
                return send_terminal;
            }
            set
            {
                send_terminal = value;
            }
        }


        /// <summary>
        /// 是否辅材
        /// </summary>
        private string subjob_flag = string.Empty;
        public string Subjob_Flag
        {
            get
            {
                return subjob_flag;
            }
            set
            {
                subjob_flag = value;
            }
        }


        /// <summary>
        /// 执行科室地址
        /// </summary>
        private string address = string.Empty;
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }


        /// <summary>
        /// 注意事项
        /// </summary>
        private string note = string.Empty;
        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                note = value;
            }
        }

        /// <summary>
        ///是否药品
        /// </summary>
        private string drug_flag = string.Empty;
        public string Drug_Flag
        {
            get
            {
                return drug_flag;
            }
            set
            {
                drug_flag = value;
            }
        }

        /// <summary>
        /// 药品规格
        /// </summary>
        private string spes = string.Empty;
        public string Spes
        {
            get
            {
                return spes;
            }
            set
            {
                spes = value;
            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        private string qty = string.Empty;
        public string Qty
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


        /// <summary>
        /// 金额
        /// </summary>
        private string tot_cost = string.Empty;
        public string Tot_Cost
        {
            get
            {
                return tot_cost;
            }
            set
            {
                tot_cost = value;
            }
        }

        /// <summary>
        /// 金额
        /// </summary>
        private string fee_date = string.Empty;
        public string Fee_Date
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

        /// <summary>
        /// 用法代码
        /// </summary>
        private string usage_code = string.Empty;
        public string Usage_Code
        {
            get
            {
                return usage_code;
            }
            set
            {
                usage_code = value;
            }
        }

        /// <summary>
        /// 用法名称
        /// </summary>
        private string usage_name = string.Empty;
        public string Usage_Name
        {
            get
            {
                return usage_name;
            }
            set
            {
                usage_name = value;
            }
        }


        /// <summary>
        /// 单位
        /// </summary>
        private string unit = string.Empty;
        public string Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
            }
        }

        /// <summary>
        /// 检查部位
        /// </summary>
        private string check_body = string.Empty;
        public string Check_Body
        {
            get
            {
                return check_body;
            }
            set
            {
                check_body = value;
            }
        }


        /// <summary>
        ///标本类型
        /// </summary>
        private string lab_type = string.Empty;
        public string Lab_Type
        {
            get
            {
                return lab_type;
            }
            set
            {
                lab_type = value;
            }
        }

        /// <summary>
        /// 发票号
        /// </summary>
        private string invoice_no = string.Empty;
        public string InvoiceNo
        {
            get
            {
                return invoice_no;
            }
            set
            {
                invoice_no = value;
            }
        }


        /// <summary>
        /// 挂号科室代码
        /// </summary>
        private string see_dpcd = string.Empty;
        public string See_Dpcd
        {
            get
            {
                return see_dpcd;
            }
            set
            {
                see_dpcd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool isChecked = true;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
            }
        }

        // {097AA15C-C4CB-4d19-B5C0-76EE20C1ACDE} 内镜中心用药单独备注
        /// <summary>
        /// 内镜评估标记
        /// </summary>
        private string assess_Flag = string.Empty;
        public string Assess_Flag
        {
            get
            {
                return assess_Flag;
            }
            set
            {
                assess_Flag = value;
            }
        }
        #endregion
    }

    [System.Serializable]
    public class MZGuideSpecialExecDept : Neusoft.FrameWork.Models.NeuObject
    {

        /// <summary>
        /// 用法代码
        /// </summary>
        private string usage_code = string.Empty;
        public string Usage_Code
        {
            get
            {
                return usage_code;
            }
            set
            {
                usage_code = value;
            }
        }

        /// <summary>
        /// 用法名称
        /// </summary>
        private string usage_name = string.Empty;
        public string Usage_Name
        {
            get
            {
                return usage_name;
            }
            set
            {
                usage_name = value;
            }
        }

        /// <summary>
        /// 执行科室地址
        /// </summary>
        private string address = string.Empty;
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }

        /// <summary>
        /// 执行科室代码
        /// </summary>
        private string exec_dpcd = string.Empty;
        public string Exec_Dpcd
        {
            get
            {
                return exec_dpcd;
            }
            set
            {
                exec_dpcd = value;
            }
        }

        /// <summary>
        /// 执行科室名称
        /// </summary>
        private string exec_dpnm = string.Empty;
        public string Exec_Dpnm
        {
            get
            {
                return exec_dpnm;
            }
            set
            {
                exec_dpnm = value;
            }
        }

    }
}
