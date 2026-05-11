using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.KangMei
{

    /// <summary>
    /// 
    /// </summary>
    public class OrderAddress:Neusoft.FrameWork.Models.NeuObject
    {
      
        private string cardNo;
        /// <summary> 
        ///CARD_NO
        /// 病历号 
        /// </summary>
        public string CardNo
        {
            get
            {
                return cardNo;
            }
            set
            {
                cardNo = value;
            }
        }


        private string name;
        /// <summary> 
        ///PATIENT_NAME
        /// 姓名 
        /// </summary>
        public string PatientName
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

        private string consignee;
        /// <summary>
        /// CONSIGNEE
        /// 收货人姓名 
        /// </summary>
        public string Consignee
        {
            get
            {
                return consignee;
            }
            set
            {
                consignee = value;
            }
        }

        private string tel;
        /// <summary> 
        ///TEL
        /// 电话 
        /// </summary>
        public string Tel
        {
            get
            {
                return tel;
            }
            set
            {
                tel = value;
            }
        }

        private string phone;
        /// <summary> 
        ///PHONE
        /// 手机 
        /// </summary>
        public string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                phone = value;
            }
        }

        private string zip;
        /// <summary>
        /// ZIP
        /// 邮编 
        /// </summary>
        public string Zip
        {
            get
            {
                return zip;
            }
            set
            {
                zip = value;
            }
        }

        private string isValid;
        /// <summary> 
        ///ISVALID
        /// 是否有效 
        /// </summary>
        public string IsVaild
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
            }
        }

        private string isDefault;
        /// <summary> 
        ///ISDEFAULT
        /// 是否默认地址
        /// </summary>
        public string IsDefault
        {
            get
            {
                return isDefault;
            }
            set
            {
                isDefault = value;
            }
        }

        private string addr;
        /// <summary> 
        ///ADDR
        /// 地址 
        /// </summary>
        public string Addr
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



        private string addr2;
        /// <summary> 
        ///ADDR2
        /// 备用地址 
        /// </summary>
        public string Addr2
        {
            get
            {
                return addr2;
            }
            set
            {
                addr2 = value;
            }
        }

        //MEMO
        private string memo;
        /// <summary> 
        ///MEMO
        /// 说明 
        /// </summary>
        public string Memo
        {
            get
            {
                return memo;
            }
            set
            {
                memo = value;
            }
        }


        private string mark;
        /// <summary> 
        ///MARK
        /// 备注 
        /// </summary>
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

        private string mark2;
        /// <summary> 
        ///MARK2
        /// 拓展1 
        /// </summary>
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

        private string mark3;
        /// <summary> 
        ///MARK3
        /// 拓展2 
        /// </summary>
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

        private DateTime oper_date;
        /// <summary> 
        ///OPER_DATE
        /// 操作时间 
        /// </summary>
        public DateTime OperDate
        {
            get
            {
                return oper_date;
            }
            set
            {
                oper_date = value;
            }
        }

        private string operCode;
        /// <summary> 
        ///OPER_CODE
        /// 操作人 
        /// </summary>
        public string OperCode
        {
            get
            {
                return operCode;
            }
            set
            {
                operCode = value;
            }
        }
    }
}
