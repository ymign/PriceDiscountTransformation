using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class ComPatient
    {
        #region 变量

        /// <summary>
        /// 门诊卡号
        /// </summary>
        private string cardNo;
        /// <summary>
        /// 姓名
        /// </summary>
        private string name;

        /// <summary>
        /// 性别编码
        /// </summary>
        private string sexCode;

        /// <summary>
        /// 生日
        /// </summary>
        private string birthday;

        /// <summary>
        /// 身份证
        /// </summary>
        private string idCard;

        /// <summary>
        /// 排班ID
        /// </summary>
        private string schemaID;
        /// <summary>
        /// 挂号级别
        /// </summary>
        private Common.ComObject regLevel;
        /// <summary>
        /// 星期
        /// </summary>
        private DayOfWeek week = DayOfWeek.Monday;
        /// <summary>
        /// 排班类型
        /// </summary>
        private string schemaType;
        /// <summary>
        /// 出诊科室
        /// </summary>
        private Common.ComObject dept;
        /// <summary>
        /// 出诊医生,模板类型为科室时,默认doct为'None'
        /// </summary>
        private Common.ComObject doct;
        /// <summary>
        /// 出诊医生类型
        /// </summary>
        private Common.ComObject doctType;
        /// <summary>
        /// 出诊午别
        /// </summary>
        private Common.ComObject noon;
        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime begin = DateTime.MinValue;
        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime end = DateTime.MinValue;
        /// <summary>
        /// 来人挂号限额
        /// </summary>
        private decimal regQuota = 0m;
        /// <summary>
        /// 电话挂号限额
        /// </summary>
        private decimal telQuota = 0m;
        /// <summary>
        /// 特诊挂号限额
        /// </summary>
        private decimal speQuota = 0m;
        /// <summary>
        /// 是否有效
        /// </summary>
        private bool isValid = false;
        /// <summary>
        /// 是否加号
        /// </summary>
        private bool isAppend = false;
        /// <summary>
        /// 停诊原因
        /// </summary>
        private Common.ComObject stopReason;
        /// <summary>
        /// 操作环境
        /// </summary>
        private Common.ComObject oper;
        /// <summary>
        /// 停止人
        /// </summary>
        private Common.ComObject stop;

        /// <summary>
        /// 诊室
        /// </summary>
        private Common.ComObject room;

        /// <summary>
        /// 诊台
        /// </summary>
        private Common.ComObject console;

        /// <summary>
        /// 出诊时间
        /// </summary>
        private DateTime seeDate = DateTime.MaxValue;

        /// <summary>
        /// 已挂号数
        /// </summary>
        private decimal regedQTY = 0m;

        /// <summary>
        /// 预约电话已挂
        /// </summary>
        private decimal telingQTY = 0m;

        /// <summary>
        /// 预约电话已确认数
        /// </summary>
        private decimal teledQTY = 0m;

        /// <summary>
        /// 特诊已挂
        /// </summary>
        private decimal spedQTY = 0m;

        /// <summary>
        /// 看诊序号
        /// </summary>
        private int seeNO = 0;

        /// <summary>
        /// 挂号费
        /// </summary>
        private decimal regFee = 0m;

        /// <summary>
        /// 检查费
        /// </summary>
        private decimal chkFee = 0m;
        /// <summary>
        /// 自费诊查费
        /// </summary>
        private decimal ownDigFee = 0m;

        /// <summary>
        /// 记帐诊查费
        /// </summary>
        private decimal pubDigFee = 0m;
        /// <summary>
        /// 其它费
        /// </summary>
        private decimal othFee = 0m;

        /// <summary>
        /// 分诊护士站
        /// </summary>
        private Common.ComObject nurseCell;

        /// <summary>
        /// 分诊队列
        /// </summary>
        private Common.ComObject queue;

        /// <summary>
        /// 真实发票号
        /// </summary>
        private string realInvoice;

        /// <summary>
        /// 系统发票号
        /// </summary>
        private string invoiceStr;

        /// <summary>
        /// 门诊流水号
        /// </summary>
        private string clinicCode;

        /// <summary>
        /// 挂号日期
        /// </summary>
        private DateTime regDate;

        /// <summary>
        /// 医疗证号
        /// </summary>
        private string mcardNo;

        /// <summary>
        /// 联系电话
        /// </summary>
        private string homePhone;

        /// <summary>
        /// 地址
        /// </summary>
        private string address;

        /// <summary>
        /// 看诊次数
        /// </summary>
        private int inTimes;

        /// <summary>
        /// 开始真实发票号
        /// </summary>
        private string beginInvoice;

        /// <summary>
        /// 结束真实发票号
        /// </summary>
        private string endInvoice;

        /// <summary>
        /// 下一张系统发票号
        /// </summary>
        private string nextInvoiceStr;

        /// <summary>
        /// 下一张真实发票号
        /// </summary>
        private string nextRealInvoice;

        /// <summary>
        /// 是否使用再用发票 ture 使用在用 false 使用未用
        /// </summary>
        private bool isUseingInvoice = false;

        /// <summary>
        /// 合同单位
        /// </summary>
        private PactInfo pact;
        
        #endregion

        #region 属性

        /// <summary>
        /// 门诊卡号
        /// </summary>
        public string CardNo
        {
            set
            {
                this.cardNo = value;
            }
            get
            {
                return this.cardNo;
            }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            set
            {
                this.name = value;
            }
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 性别编码
        /// </summary>
        public string SexCode
        {
            set
            {
                this.sexCode = value;
            }
            get
            {
                return this.sexCode;
            }
        }

        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday
        {
            set
            {
                this.birthday = value;
            }
            get
            {
                return this.birthday;
            }
        }

        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard
        {
            set
            {
                this.idCard = value;
            }
            get
            {
                return this.idCard;
            }
        }

        /// <summary>
        /// 排班ID
        /// </summary>
        public string SchemaID
        {
            set
            {
                this.schemaID = value;
            }
            get
            {
                return this.schemaID;
            }
        }

        /// <summary>
        /// 诊室
        /// </summary>
        public Common.ComObject Room
        {
            get
            {
                if (this.room == null)
                {
                    this.room = new Common.ComObject();
                }
                return room;
            }
            set { room = value; }
        }

        /// <summary>
        /// 诊台
        /// </summary>
        public Common.ComObject Console
        {
            get
            {
                if (console == null)
                {
                    console = new Common.ComObject();
                }
                return console;
            }
            set { console = value; }
        }

        /// <summary>
        /// 挂号级别
        /// </summary>
        public Common.ComObject RegLevel
        {
            get
            {
                if (this.regLevel == null)
                {
                    this.regLevel = new Common.ComObject();
                }
                return this.regLevel;
            }
            set { this.regLevel = value; }
        }

        /// <summary>
        /// 星期
        /// </summary>
        public DayOfWeek Week
        {
            get { return week; }
            set { week = value; }
        }

        /// <summary>
        /// 排班类型
        /// </summary>
        public string SchemaType
        {
            get { return schemaType; }
            set { schemaType = value; }
        }

        /// <summary>
        /// 出诊科室
        /// </summary>
        public Common.ComObject Dept
        {
            get
            {
                if (this.dept == null)
                {
                    this.dept = new Common.ComObject();
                }
                return dept;
            }
            set { dept = value; }
        }

        /// <summary>
        /// 出诊医师
        /// </summary>
        public Common.ComObject Doct
        {
            get
            {
                if (this.doct == null)
                {
                    this.doct = new Common.ComObject();
                }

                return doct;
            }
            set { doct = value; }
        }

        /// <summary>
        /// 医生类别
        /// </summary>
        public Common.ComObject DoctType
        {
            get
            {
                if (this.doctType == null)
                {
                    this.doctType = new Common.ComObject();
                }

                return this.doctType;
            }
            set { this.doctType = value; }
        }

        /// <summary>
        /// 午别
        /// </summary>
        public Common.ComObject Noon
        {
            get
            {
                if (this.noon == null)
                {
                    this.noon = new Common.ComObject();
                }
                return noon;
            }
            set { noon = value; }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Begin
        {
            get { return begin; }
            set { begin = value; }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End
        {
            get { return end; }
            set { end = value; }
        }

        /// <summary>
        /// 现场挂号限额
        /// </summary>
        public decimal RegQuota
        {
            get { return regQuota; }
            set { regQuota = value; }
        }

        /// <summary>
        /// 电话挂号限额
        /// </summary>
        public decimal TelQuota
        {
            get { return telQuota; }
            set { telQuota = value; }
        }

        /// <summary>
        /// 特诊挂号限额
        /// </summary>
        public decimal SpeQuota
        {
            get { return speQuota; }
            set { speQuota = value; }
        }

        /// <summary>
        /// 是否加号
        /// </summary>
        public bool IsAppend
        {
            get { return isAppend; }
            set { isAppend = value; }
        }

        /// <summary>
        /// 停诊原因
        /// </summary>
        public Common.ComObject StopReason
        {
            get
            {
                if (this.stopReason == null)
                {
                    this.stopReason = new Common.ComObject();
                }
                return this.stopReason;
            }
            set { this.stopReason = value; }
        }

        /// <summary>
        /// 操作环境变量
        /// </summary>
        public Common.ComObject Oper
        {
            get
            {
                if (this.oper == null)
                {
                    this.oper = new Common.ComObject();
                }
                return oper;
            }
            set { oper = value; }
        }

        /// <summary>
        /// 停止人
        /// </summary>
        public Common.ComObject Stop
        {
            get
            {
                if (this.stop == null)
                {
                    this.stop = new Common.ComObject();
                }
                return this.stop;
            }
            set { this.stop = value; }
        }

        /// <summary>
        /// 出诊时间
        /// </summary>
        public DateTime SeeDate
        {
            get { return this.seeDate; }
            set { this.seeDate = value; }
        }

        /// <summary>
        /// 已挂数量
        /// </summary>
        public decimal RegedQTY
        {
            get { return this.regedQTY; }
            set { this.regedQTY = value; }
        }

        /// <summary>
        /// 电话已预约
        /// </summary>
        public decimal TelingQTY
        {
            get { return this.telingQTY; }
            set { this.telingQTY = value; }
        }

        /// <summary>
        /// 预约电话已取
        /// </summary>
        public decimal TeledQTY
        {
            get { return this.teledQTY; }
            set { this.teledQTY = value; }
        }

        /// <summary>
        /// 特诊已挂
        /// </summary>
        public decimal SpedQTY
        {
            get { return this.spedQTY; }
            set { this.spedQTY = value; }
        }

        /// <summary>
        /// 看诊序号
        /// </summary>
        public int SeeNO
        {
            get { return this.seeNO; }
            set { this.seeNO = value; }
        }

        /// <summary>
        /// 挂号费
        /// </summary>
        public decimal RegFee
        {
            get { return this.regFee; }
            set { this.regFee = value; }
        }

        /// <summary>
        /// 检查费
        /// </summary>
        public decimal ChkFee
        {
            get { return this.chkFee; }
            set { this.chkFee = value; }
        }

        /// <summary>
        /// 自费诊查费
        /// </summary>
        public decimal OwnDigFee
        {
            get { return this.ownDigFee; }
            set { this.ownDigFee = value; }
        }
        /// <summary>
        /// 记帐诊查费
        /// </summary>
        public decimal PubDigFee
        {
            get { return this.pubDigFee; }
            set { this.pubDigFee = value; }
        }
        /// <summary>
        /// 其它费
        /// </summary>
        public decimal OthFee
        {
            get { return this.othFee; }
            set { this.othFee = value; }
        }

        /// <summary>
        /// 分诊护士站
        /// </summary>
        public Common.ComObject NurseCell
        {
            get
            {
                if (this.nurseCell == null)
                {
                    this.nurseCell = new Common.ComObject();
                }
                return this.nurseCell;
            }
            set { this.nurseCell = value; }
        }


        /// <summary>
        /// 分诊队列
        /// </summary>
        public Common.ComObject Queue
        {
            get
            {
                if (this.queue == null)
                {
                    this.queue = new Common.ComObject();
                }
                return this.queue;
            }
            set { this.queue = value; }
        }

        /// <summary>
        /// 真实发票号
        /// </summary>
        public string RealInvoice
        {
            get { return this.realInvoice; }
            set { this.realInvoice = value; }
        }

        /// <summary>
        /// 系统发票号
        /// </summary>
        public string InvoiceStr
        {
            get { return this.invoiceStr; }
            set { this.invoiceStr = value; }
        }

        /// <summary>
        /// 系统发票号
        /// </summary>
        public string ClinicCode
        {
            get { return this.clinicCode; }
            set { this.clinicCode = value; }
        }

        /// <summary>
        /// 挂号日期
        /// </summary>
        public DateTime RegDate
        {
            get { return this.regDate; }
            set { this.regDate = value; }
        }

        /// <summary>
        /// 医疗证号
        /// </summary>
        public string McardNo
        {
            get { return this.mcardNo; }
            set { this.mcardNo = value; }
        }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string HomePhone
        {
            get { return this.homePhone; }
            set { this.homePhone = value; }
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return this.address; }
            set { this.address = value; }
        }

        /// <summary>
        /// 看诊次数
        /// </summary>
        public int InTimes
        {
            get { return this.inTimes; }
            set { this.inTimes = value; }
        }

        /// <summary>
        /// 开始发票号（旧）
        /// </summary>
        public string BeginInvoice
        {
            get { return this.beginInvoice; }
            set { this.beginInvoice = value; }
        }

        /// <summary>
        /// 结束发票号（旧）
        /// </summary>
        public string EndInvoice
        {
            get { return this.endInvoice; }
            set { this.endInvoice = value; }
        }

        /// <summary>
        /// 下一张系统发票号
        /// </summary>
        public string NextRealInvoice
        {
            get { return this.nextRealInvoice; }
            set { this.nextRealInvoice = value; }
        }

        /// <summary>
        /// 下一张系统发票号
        /// </summary>
        public string NextInvoiceStr
        {
            get { return this.nextInvoiceStr; }
            set { this.nextInvoiceStr = value; }
        }

        /// <summary>
        /// 是否使用再用发票 ture 使用在用 false 使用未用
        /// </summary>
        public bool IsUseingInvoice
        {
            get { return this.isUseingInvoice; }
            set { this.isUseingInvoice = value; }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        private string payType;
        public string PayType
        {
            get
            {
                return payType;
            }
            set
            {
                payType = value;
            }
        }

       
        private RegisterFeeInfo feeInfo=new RegisterFeeInfo();
        /// <summary>
        /// 费用明细信息，检查费挂号费
        /// </summary>
        public RegisterFeeInfo FeeInfo
        {
            get
            {
                return feeInfo;
            }
            set
            {
                feeInfo = value;
            }
        }

        /// <summary>
        /// 合同单位
        /// </summary>
        public PactInfo Pact
        {
            get
            {
                if (this.pact == null)
                {
                    this.pact = new PactInfo();
                }
                return this.pact;
            }
            set { this.pact = value; }
        }

        private string reg_no;
        /// <summary>
        /// 诊金登记单号
        /// </summary>
        public string RegNo
        {
            get
            {
                return reg_no;
            }
            set
            {
                reg_no = value;
            }
        }

        private string diag_code;
        /// <summary>
        /// 诊金代码
        /// </summary>
        public string RegDiagCode
        {
            get
            {
                return diag_code;
            }
            set
            {
                diag_code = value;
            }
        }


        private BookInfo info;
        public BookInfo Book
        {
            get
            {
                return info;
            }
            set
            {
                info = value;
            }
        }
        

        #endregion
        

    }

    public class RegisterFeeInfo
    {
        private decimal reg_tot;
        /// <summary>
        /// 总挂号费
        /// </summary>
        public decimal RegTotCost
        {
            get
            {
                return reg_tot;
            }
            set
            {
                reg_tot = value;
            }
        }

        private decimal reg_own_cost;
        /// <summary>
        /// 挂号费 自费金额
        /// </summary>
        public decimal RegOwnCost
        {
            get
            {
                return reg_own_cost;
            }
            set
            {
                reg_own_cost = value;
            }
        }

        private decimal reg_Pay_Cost;
        /// <summary>
        /// 挂号费 自付金额
        /// </summary>
        public decimal RegPayCost
        {
            get
            {
                return reg_Pay_Cost;
            }
            set
            {
                reg_Pay_Cost = value;
            }
        }

        private decimal reg_pub_cost;
        /// <summary>
        /// 挂号费 报销金额
        /// </summary>
        public decimal RegPubCost
        {
            get
            {
                return reg_pub_cost;
            }
            set
            {
                reg_pub_cost = value;
            }
        }

        private decimal diag_tot_cost;
        /// <summary>
        /// 诊查费 总金额
        /// </summary>
        public decimal DiagTotCost
        {
            get
            {
                return diag_tot_cost;
            }
            set
            {
                diag_tot_cost = value;
            }
        }

        private decimal diag_Pay_cost;
        /// <summary>
        /// 诊查费 自付金额
        /// </summary>
        public decimal DiagPayCost
        {
            get
            {
                return diag_Pay_cost;
            }
            set
            {
                diag_Pay_cost = value;
            }
        }

        private decimal diag_pub_cost;
        /// <summary>
        /// 诊查费 报销金额
        /// </summary>
        public decimal DiagPubCost
        {
            get
            {
                return diag_pub_cost;
            }
            set
            {
                diag_pub_cost = value;
            }
        }

        private decimal diag_own_cost;
        /// <summary>
        /// 诊查费 自费金额
        /// </summary>
        public decimal DiagOwnCost
        {
            get
            {
                return diag_own_cost;
            }
            set
            {
                diag_own_cost = value;
            }
        }
    }
}
