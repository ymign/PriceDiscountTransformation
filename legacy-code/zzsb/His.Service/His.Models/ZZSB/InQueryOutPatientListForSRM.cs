using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InQueryOutPatientListForSRM
    {
        /// <summary>
        /// 电子票内网H5页面url
        /// </summary>
        public string Pictureurl { get; set; }

        /// <summary>
        /// 电子票二维码数据
        /// </summary>
        public string Billqrcode { get; set; }

        private string transerno;
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string TRANSERNO
        {
            get
            {
                return transerno;
            }
            set
            {
                transerno = value;
            }
        }

        private string regdate;
        /// <summary>
        /// 挂号时间
        /// </summary>
        public string REGDATE
        {
            get
            {
                return regdate;
            }
            set
            {
                regdate = value;
            }
        }

        
        private string invoiceno;
        /// <summary>
        /// 发票号
        /// </summary>
        public string INVOICENO
        {
            get
            {
                return invoiceno;
            }
            set
            {
                invoiceno = value;
            }
        }

        private string invoiceno1;
        /// <summary>
        /// 发票号
        /// </summary>
        public string INVOICENO1
        {
            get
            {
                return invoiceno1;
            }
            set
            {
                invoiceno1 = value;
            }
        }

        private string execadress;
        /// <summary>
        /// 执行地点
        /// </summary>
        public string EXECADRESS
        {
            get
            {
                return execadress;
            }
            set
            {
                execadress = value;
            }
        }

        private string message;
        /// <summary>
        /// 发票号
        /// </summary>
        public string MESSAGE
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
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


        private string regid;
        /// <summary>
        /// 就诊记录编码
        /// </summary>
        public string REGID
        {
            get
            {
                return regid;
            }
            set
            {
                regid = value;
            }
        }


        private string recipeno;
        /// <summary>
        /// 处方号
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

        private string name;
        /// <summary>
        /// 姓名
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


        private string feetype;
        /// <summary>
        /// 消费类别
        /// </summary>
        public string FEETYPE
        {
            get
            {
                return feetype;
            }
            set
            {
                feetype = value;
            }
        }

        private string depname;
        /// <summary>
        /// 住院科室名
        /// </summary>
        public string DEPNAME
        {
            get
            {
                return depname;
            }
            set
            {
                depname = value;
            }
        }

        private string doctor;
        /// <summary>
        /// 住院科室名
        /// </summary>
        public string DOCTOR
        {
            get
            {
                return doctor;
            }
            set
            {
                doctor = value;
            }
        }

        private string isprintable;
        /// <summary>
        /// 是否可以打印
        /// </summary>
        public string ISPRINTABLE
        {
            get
            {
                return isprintable;
            }
            set
            {
                isprintable = value;
            }
        }

        private string startdate;
        /// <summary>
        /// 费用开始时间
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
        /// 费用结束时间
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

        private string date1;
        /// <summary>
        /// 费用日期
        /// </summary>
        public string DATE1
        {
            get
            {
                return date1;
            }
            set
            {
                date1 = value;
            }
        }

        private string itemcode;
        /// <summary>
        /// 项目代码
        /// </summary>
        public string ITEMCODE
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

        private string itemname;
        /// <summary>
        /// 项目名称
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

        private string invoicetype;
        /// <summary>
        /// 发票分类
        /// </summary>
        public string INVOICETYPE
        {
            get
            {
                return invoicetype;
            }
            set
            {
                invoicetype = value;
            }
        }

        private string standard;
        /// <summary>
        /// 规格
        /// </summary>
        public string STANDARD
        {
            get
            {
                return standard;
            }
            set
            {
                standard = value;
            }
        }

        private string feetype1;
        /// <summary>
        /// 医保类别
        /// </summary>
        public string FEETYPE1
        {
            get
            {
                return feetype1;
            }
            set
            {
                feetype1 = value;
            }
        }

        private string units;
        /// <summary>
        /// 单位
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

        private string price;
        /// <summary>
        /// 单价
        /// </summary>
        public string PRICE
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }

        private string number1;
        /// <summary>
        /// 数量
        /// </summary>
        public string NUMBER1
        {
            get
            {
                return number1;
            }
            set
            {
                number1 = value;
            }
        }

        private string itemfee;
        /// <summary>
        /// 合计
        /// </summary>
        public string ITEMFEE
        {
            get
            {
                return itemfee;
            }
            set
            {
                itemfee = value;
            }
        }

        private string printdate;
        /// <summary>
        /// 打印日期
        /// </summary>
        public string PRINTDATE
        {
            get
            {
                return printdate;
            }
            set
            {
                printdate = value;
            }
        }

    }
}
