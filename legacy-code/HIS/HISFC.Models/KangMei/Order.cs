using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.KangMei
{
    public class KangMeiOrder : Neusoft.FrameWork.Models.NeuObject
    {
      
        //CLINIC_CODE
        private string clinicCode;
        public string ClinicCode
        {
            get
            {
                return clinicCode;
            }
            set
            {
                clinicCode = value;
            }
        }
        //CARD_NO
        private string cardNo;
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

        //PATIENT_NAME
        private string name;
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
        //RECIPENO
        private string receipeNo;
        public string RecipeNo
        {
            get
            {
                return receipeNo;
            }
            set
            {
                receipeNo = value;
            }
        }
        //ORDNO
        private string ordNo;
        public string OrderNo
        {
            get
            {
                return ordNo;
            }
            set
            {
                ordNo = value;
            }
        }
        //ADDR
        private string addr;
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
        //ADDR2
        private string addr2;
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
        //TEL
        private string tel;
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
        //PHONE
        private string phone;
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
        //ZIP
        private string zip;
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
        //CONSIGNEE
        private string consignee;
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
        //SEX
        private string sex;
        public string Sex
        {
            get
            {
                return sex;
            }
            set
            {
                sex = value;
            }
        }
        //AGE
        private string age;
        public string Age
        {
            get
            {
                return age;
            }
            set
            {
                age = value;
            }
        }
        //ORD_DATE
        private DateTime orderDate;
        public DateTime OrderDate
        {
            get
            {
                return orderDate;
            }
            set
            {
                orderDate = value;
            }
        }
        //DRUG_DEPT_CODE
        private string drug_dept_code;
        public string DrugDeptCode
        {
            get
            {
                return drug_dept_code;
            }
            set
            {
                drug_dept_code = value;
            }
        }
        //ORD_STATE
        private string state;
        public string State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }
        //ISSEND
        private string isSend;
        public string IsSend
        {
            get
            {
                return isSend;
            }
            set
            {
                isSend = value;
            }
        }
        //ISCOOK
        private string isCook;
        public string IsCook
        {
            get
            {
                return isCook;
            }
            set
            {
                isCook = value;
            }
        }
        //MEMO
        private string memo;
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
        //MARK
        private string mark;
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
        //MARK2
        private string mark2;
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
        //MARK3
        private string mark3;
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
        //OPER_DATE
        private DateTime oper_date;
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
        //OPER_CODE
        private string operCode;
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
