using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Pharmacy 
{
    /// <summary>
    /// 中选药品实体
    /// </summary>
    [System.Serializable]
    public class PcContro : Neusoft.FrameWork.Models.NeuObject
    {
        /// <summary>
        /// 中选代码
        /// </summary>
        private string itemcode = string.Empty;
        public string ItemCode
        {
            get
            {
                return itemcode;
            }
            set
            {
                itemcode = value;
            }
        }



        /// <summary>
        /// 中选项目名称
        /// </summary>
        private string itemname = string.Empty;
        public string ItemName
        {
            get
            {
                return itemname;
            }
            set
            {
                itemname = value;
            }
        }

        /// <summary>
        /// 非中选药品代码
        /// </summary>
        private string itemcode1 = string.Empty;
        public string Itemcode1
        {
            get
            {
                return itemcode1;
            }
            set
            {
                itemcode1 = value;
            }
        }

        /// <summary>
        /// 同类药品
        /// </summary>
        private string itemcode2 = string.Empty;
        public string Itemcode2
        {
            get
            {
                return itemcode2;
            }
            set
            {
                itemcode2 = value;
            }
        }

        /// <summary>
        /// 非中选药品指定开具专科
        /// </summary>
        private string deptcode = string.Empty;
        public string Deptcode
        {
            get
            {
                return deptcode;
            }
            set
            {
                deptcode = value;
            }
        }


        /// <summary>
        /// 中选药品指定开具专科
        /// </summary>
        private string deptcode2 = string.Empty;
        public string Deptcode2
        {
            get
            {
                return deptcode2;
            }
            set
            {
                deptcode2 = value;
            }
        }

        /// <summary>
        /// 拼音
        /// </summary>
        private string sepll = string.Empty;
        public string SpellCode
        {
            get
            {
                return sepll;
            }
            set
            {
                sepll = value;
            }
        }

        /// <summary>
        /// 五笔
        /// </summary>
        private string wb_code = string.Empty;
        public string Wb_code
        {
            get
            {
                return wb_code;
            }
            set
            {
                wb_code = value;
            }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime operdate = new DateTime();
        public DateTime OperDate
        {
            get
            {
                if (operdate == null)
                {
                    operdate = new DateTime();
                }
                return this.operdate;
            }
            set
            {
                this.operdate = value;
            }
        }

        /// <summary>
        /// 操作人
        /// </summary>
        private string oper_code = string.Empty;
        public string OperCode
        {
            get
            {
                return oper_code;
            }
            set
            {
                oper_code = value;
            }
        }

        /// <summary>
        /// 有效性
        /// </summary>
        private bool valid_state;
        public bool Valid_State
        {
            get
            {
                return valid_state;
            }
            set
            {
                valid_state = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string mark = string.Empty;
        public string Mark
        {
            get
            {
                return mark;
            }
            set
            {
                mark = value;
            }
        }

        /// <summary>
        /// 备注1
        /// </summary>
        private string mark1 = string.Empty;
        public string Mark1
        {
            get
            {
                return mark1;
            }
            set
            {
                mark1 = value;
            }
        }

        /// <summary>
        /// 备注2
        /// </summary>
        private string mark2 = string.Empty;
        public string Mark2
        {
            get
            {
                return mark2;
            }
            set
            {
                mark2 = value;
            }
        }

        /// <summary>
        /// 备注3
        /// </summary>
        private string mark3 = string.Empty;
        public string Mark3
        {
            get
            {
                return mark3;
            }
            set
            {
                mark3 = value;
            }
        }

        /// <summary>
        /// 备注4
        /// </summary>
        private string mark4 = string.Empty;
        public string Mark4
        {
            get
            {
                return mark4;
            }
            set
            {
                mark4 = value;
            }
        }

    }
}
