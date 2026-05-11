using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InGetPrescriptionAndChargeDetailsForSRM
    {
        private string patientid;
        /// <summary>
        /// 患者ID号
        /// </summary>
        public string PATIENTID
        {
            get
            {
                return patientid;
            }
            set
            {
                patientid = value;
            }
        }

        private string outpatientno;
        /// <summary>
        /// 门诊号
        /// </summary>
        public string OUTPATIENTNO
        {
            get
            {
                return outpatientno;
            }
            set
            {
                outpatientno = value;
            }
        }

        private string startdate;
        /// <summary>
        /// 开始时间
        /// </summary>
        public string STARTDATE
        {
            get
            {
                return startdate;
            }
            set
            {
                startdate = value;
            }
        }

        private string enddate;
        /// <summary>
        /// 结束时间
        /// </summary>
        public string ENDDATE
        {
            get
            {
                return enddate;
            }
            set
            {
                enddate = value;
            }
        }

        private string note;
        /// <summary>
        /// 备注
        /// </summary>
        public string NOTE
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


        private string cardno;
        /// <summary>
        /// 卡号
        /// </summary>
        public string CARDNO
        {
            get
            {
                return cardno;
            }
            set
            {
                cardno = value;
            }
        }

        private string hospcode;
        /// <summary>
        /// 院区编号
        /// </summary>
        public string HOSPCODE
        {
            get
            {
                return hospcode;
            }
            set
            {
                hospcode = value;
            }
        }

        private string cardtypecode;
        /// <summary>
        /// 卡类型
        /// </summary>
        public string CARDTYPECODE
        {
            get
            {
                return cardtypecode;
            }
            set
            {
                cardtypecode = value;
            }
        }

        private string recipeno;
        /// <summary>
        /// 就诊记录编码
        /// </summary>
        public string RECIPENO
        {
            get
            {
                return recipeno;
            }
            set
            {
                recipeno = value;
            }
        }

        private string recipetime;
        /// <summary>
        /// 就诊日期
        /// </summary>
        public string RECIPETIME
        {
            get
            {
                return recipetime;
            }
            set
            {
                recipetime = value;
            }
        }

        private string deptname;
        /// <summary>
        /// 科室名称
        /// </summary>
        public string DEPTNAME
        {
            get
            {
                return deptname;
            }
            set
            {
                deptname = value;
            }
        }


        private string doctname;
        /// <summary>
        /// 医生姓名
        /// </summary>
        public string DOCTNAME
        {
            get
            {
                return doctname;
            }
            set
            {
                doctname = value;
            }
        }

        private string totalfee;
        /// <summary>
        /// 总金额
        /// </summary>
        public string TOTALFEE
        {
            get
            {
                return totalfee;
            }
            set
            {
                totalfee = value;
            }
        }

        private string payflag;
        /// <summary>
        /// 支付标记
        /// </summary>
        public string PAYFLAG
        {
            get
            {
                return payflag;
            }
            set
            {
                payflag = value;
            }
        }

        private string itemid;
        /// <summary>
        /// 医嘱编号
        /// </summary>
        public string ITEMID
        {
            get
            {
                return itemid;
            }
            set
            {
                itemid = value;
            }
        }

        private string itemname;
        /// <summary>
        /// 医嘱名称
        /// </summary>
        public string ITEMNAME
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

        private string itemtype;
        /// <summary>
        /// 医嘱类型
        /// </summary>
        public string ITEMTYPE
        {
            get
            {
                return itemtype;
            }
            set
            {
                itemtype = value;
            }
        }

        private string itemtotalfee;
        /// <summary>
        /// 医嘱总金额
        /// </summary>
        public string ITEMTOTALFEE
        {
            get
            {
                return itemtotalfee;
            }
            set
            {
                itemtotalfee = value;
            }
        }

        private string subitemid;
        /// <summary>
        /// 细项编号
        /// </summary>
        public string SUBITEMID
        {
            get
            {
                return subitemid;
            }
            set
            {
                subitemid = value;
            }
        }

        private string subitemname;
        /// <summary>
        /// 细项名称
        /// </summary>
        public string SUBITEMNAME
        {
            get
            {
                return subitemname;
            }
            set
            {
                subitemname = value;
            }
        }

        private string specs;
        /// <summary>
        /// 规格
        /// </summary>
        public string SPECS
        {
            get
            {
                return specs;
            }
            set
            {
                specs = value;
            }
        }

        private string unit;
        /// <summary>
        /// 单位
        /// </summary>
        public string UNIT
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

        private string quantity;
        /// <summary>
        /// 数量
        /// </summary>
        public string QUANTITY
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
            }
        }

        private string unitprice;
        /// <summary>
        /// 单价
        /// </summary>
        public string UNITPRICE
        {
            get
            {
                return unitprice;
            }
            set
            {
                unitprice = value;
            }
        }

        private string fee;
        /// <summary>
        /// 费用
        /// </summary>
        public string FEE
        {
            get
            {
                return fee;
            }
            set
            {
                fee = value;
            }
        }

        private string deptfloor;
        /// <summary>
        /// 执行地址
        /// </summary>
        public string DEPTFLOOR
        {
            get
            {
                return deptfloor;
            }
            set
            {
                deptfloor = value;
            }
        }



 
        private string deviceid;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DEVICEID
        {
            get
            {
                return deviceid;
            }
            set
            {
                deviceid = value;
            }
        }

        private string servicecode;
        /// <summary>
        /// 服务编号
        /// </summary>
        public string SERVICECODE
        {
            get
            {
                return servicecode;
            }
            set
            {
                servicecode = value;
            }
        }


    }
}
