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
    public class MZGuideContrast : Neusoft.FrameWork.Models.NeuObject   
    {
        /// <summary>
        /// 项目代码
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
        /// 项目名称
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
        /// 标本代码
        /// </summary>
        private string lab = string.Empty;
        public string LabCode
        {
            get
            {
                return lab;
            }
            set
            {
                lab = value;
            }
        }

        /// <summary>
        /// 标本名称
        /// </summary>
        private string labname = string.Empty;
        public string LabName
        {
            get
            {
                return labname;
            }
            set
            {
                labname = value;
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        private string addr_code = string.Empty;
        public string Addr_Code
        {
            get
            {
                return addr_code;
            }
            set
            {
                addr_code = value;
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        private string addr = string.Empty;
        public string Addresses
        {
            get
            {
                return addr;
            }
            set
            {
                addr = value;
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
        private string fine = string.Empty;
        public string FineCode
        {
            get
            {
                return fine;
            }
            set
            {
                fine = value;
            }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        private string operdate = string.Empty;
        public string OperDate
        {
            get
            {
                return operdate;
            }
            set
            {
                operdate = value;
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
        /// 备注
        /// </summary>
        private string valid_state = string.Empty;
        public string ValidState
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
        /// 是否能加急
        /// </summary>
        private string urgency = string.Empty;
        /// <summary>
        /// 是否能加急
        /// </summary>
        public string Urgency
        {
            get { return urgency; }
            set { urgency = value; }
        }
    }
}
