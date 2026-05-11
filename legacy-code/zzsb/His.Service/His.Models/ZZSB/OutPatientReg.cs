using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class OutPatientReg
    {
        #region 变量

        /// <summary>
        /// 操作员编号
        /// </summary>
        private string userID;

        /// <summary>
        /// 设备编号
        /// </summary>
        private string deviceID;

        /// <summary>
        /// 服务编码
        /// </summary>
        private string serviceCode;

        /// <summary>
        /// 业务编号
        /// </summary>
        private string funCode;

        /// <summary>
        /// 请求时间
        /// </summary>
        private string reqTime;

        /// <summary>
        /// 请求流水号
        /// </summary>
        private string reqTraceNo;

        /// <summary>
        /// 卡号
        /// </summary>
        private string cardNo;

        /// <summary>
        /// 挂号日期
        /// </summary>
        private string regDate;

        /// <summary>
        /// 科室编号
        /// </summary>
        private string deptCode;

        /// <summary>
        /// 出诊时段编号
        /// </summary>
        private string sessionCode;

        /// <summary>
        /// 医生编号
        /// </summary>
        private string doctorCode;

        /// <summary>
        /// 排班编号
        /// </summary>
        private string regSourceID;

        /// <summary>
        /// 锁号流水号
        /// </summary>
        private string tranSerNo;

        /// <summary>
        /// 总挂号费
        /// </summary>
        private decimal totalRegFee;

        /// <summary>
        /// 支付方式
        /// </summary>
        private string payType;

        /// <summary>
        /// POS终端号
        /// </summary>
        private string posID;

        /// <summary>
        /// 支付的银行卡号
        /// </summary>
        private string bankCardNo;

        /// <summary>
        /// 支付日期
        /// </summary>
        private string payDate;

        /// <summary>
        /// 支付时间
        /// </summary>
        private string payTime;

        /// <summary>
        /// 批次号
        /// </summary>
        private string batchNo;

        /// <summary>
        /// 凭证号
        /// </summary>
        private string vouchNo;

        /// <summary>
        /// 参考号
        /// </summary>
        private string referNo;

        /// <summary>
        /// 支付金额
        /// </summary>
        private decimal payAmt;

        /// <summary>
        /// 银行代码
        /// </summary>
        private string bankCode;

        /// <summary>
        /// 医保交易流水号
        /// </summary>
        private string medInsureTranNo;

        /// <summary>
        /// 医保字符串
        /// </summary>
        private string medInsureStr;

        /// <summary>
        /// 医保支付费用
        /// </summary>
        private decimal medInsureFee;

        /// <summary>
        /// 个人支付费用
        /// </summary>
        private decimal personalFee;

        /// <summary>
        /// 合同单位
        /// </summary>
        private string feeType;

        #endregion

        #region 属性

        public string ClincCode { get; set; }
        //private string payType;
        ///// <summary>
        ///// 支付方式
        ///// </summary>
        //public string PayType
        //{
        //    get
        //    {
        //        return payType;
        //    }
        //    set
        //    {
        //        payType = value;
        //    }
        //}

        private string Payinsufeestr_;
        /// <summary>
        /// 诊金减免字符串，分割
        /// 诊金登记单号^门特结算单号^医生级别代码^挂号金额^个人支付金额^医改减免金额^病种报销金额^险种
        /// </summary>
        public string Payinsufeestr
        {
            get
            {
                return Payinsufeestr_;
            }
            set
            {
                Payinsufeestr_ = value;
            }
        }

        /// <summary>
        /// 知情同意书结果
        /// </summary>
        public string InformedConsentResult { get; set; }

        /// <summary>
        /// 操作员编号
        /// </summary>
        public string UserID
        {
            get
            {
                return this.userID;
            }
            set
            {
                this.userID = value;
            }
        }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceID
        {
            get
            {
                return this.deviceID;
            }
            set
            {
                this.deviceID = value;
            }
        }

        /// <summary>
        /// 服务编码
        /// </summary>
        public string ServiceCode
        {
            get
            {
                return this.serviceCode;
            }
            set
            {
                this.serviceCode = value;
            }
        }

        /// <summary>
        /// 业务编号
        /// </summary>
        public string FunCode
        {
            get
            {
                return this.funCode;
            }
            set
            {
                this.funCode = value;
            }
        }

        /// <summary>
        /// 请求时间
        /// </summary>
        public string ReqTime
        {
            get
            {
                return this.reqTime;
            }
            set
            {
                this.reqTime = value;
            }
        }

        /// <summary>
        /// 请求流水号
        /// </summary>
        public string ReqTraceNo
        {
            get
            {
                return this.reqTraceNo;
            }
            set
            {
                this.reqTraceNo = value;
            }
        }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo
        {
            get
            {
                return this.cardNo;
            }
            set
            {
                this.cardNo = value;
            }
        }

        /// <summary>
        /// 挂号日期
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.regDate;
            }
            set
            {
                this.regDate = value;
            }
        }

        /// <summary>
        /// 科室编号
        /// </summary>
        public string DeptCode
        {
            get
            {
                return this.deptCode;
            }
            set
            {
                this.deptCode = value;
            }
        }

        /// <summary>
        /// 出诊时段编号
        /// </summary>
        public string SessionCode
        {
            get
            {
                return this.sessionCode;
            }
            set
            {
                this.sessionCode = value;
            }
        }

        /// <summary>
        /// 医生编号
        /// </summary>
        public string DoctorCode
        {
            get
            {
                return this.doctorCode;
            }
            set
            {
                this.doctorCode = value;
            }
        }

        /// <summary>
        /// 排班编号
        /// </summary>
        public string RegSourceID
        {
            get
            {
                return this.regSourceID;
            }
            set
            {
                this.regSourceID = value;
            }
        }

        /// <summary>
        /// 锁号流水号
        /// </summary>
        public string TranSerNo
        {
            get
            {
                return this.tranSerNo;
            }
            set
            {
                this.tranSerNo = value;
            }
        }

        /// <summary>
        /// 总挂号费
        /// </summary>
        public decimal TotalRegFee
        {
            get
            {
                return this.totalRegFee;
            }
            set
            {
                this.totalRegFee = value;
            }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayType
        {
            get
            {
                return this.payType;
            }
            set
            {
                this.payType = value;
            }
        }

        /// <summary>
        /// POS终端号
        /// </summary>
        public string PosID
        {
            get
            {
                return this.posID;
            }
            set
            {
                this.posID = value;
            }
        }

        /// <summary>
        /// 支付的银行卡号
        /// </summary>
        public string BankCardNo
        {
            get
            {
                return this.bankCardNo;
            }
            set
            {
                this.bankCardNo = value;
            }
        }

        /// <summary>
        /// 支付日期
        /// </summary>
        public string PayDate
        {
            get
            {
                return this.payDate;
            }
            set
            {
                this.payDate = value;
            }
        }

        /// <summary>
        /// 支付时间
        /// </summary>
        public string PayTime
        {
            get
            {
                return this.payTime;
            }
            set
            {
                this.payTime = value;
            }
        }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo
        {
            get
            {
                return this.batchNo;
            }
            set
            {
                this.batchNo = value;
            }
        }

        /// <summary>
        /// 凭证号
        /// </summary>
        public string VouchNo
        {
            get
            {
                return this.vouchNo;
            }
            set
            {
                this.vouchNo = value;
            }
        }

        /// <summary>
        /// 参考号
        /// </summary>
        public string ReferNo
        {
            get
            {
                return this.referNo;
            }
            set
            {
                this.referNo = value;
            }
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayAmt
        {
            get
            {
                return this.payAmt;
            }
            set
            {
                this.payAmt = value;
            }
        }

        /// <summary>
        /// 银行代码
        /// </summary>
        public string BankCode
        {
            get
            {
                return this.bankCode;
            }
            set
            {
                this.bankCode = value;
            }
        }

        /// <summary>
        /// 医保交易流水号
        /// </summary>
        public string MedInsureTranNo
        {
            get
            {
                return this.medInsureTranNo;
            }
            set
            {
                this.medInsureTranNo = value;
            }
        }

        /// <summary>
        /// 医保字符串
        /// </summary>
        public string MedInsureStr
        {
            get
            {
                return this.medInsureStr;
            }
            set
            {
                this.medInsureStr = value;
            }
        }

        /// <summary>
        /// 医保支付费用
        /// </summary>
        public decimal MedInsureFee
        {
            get
            {
                return this.medInsureFee;
            }
            set
            {
                this.medInsureFee = value;
            }
        }

        /// <summary>
        /// 个人支付费用
        /// </summary>
        public decimal PersonalFee
        {
            get
            {
                return this.personalFee;
            }
            set
            {
                this.personalFee = value;
            }
        }

        /// <summary>
        /// 合同单位
        /// </summary>
        public string FeeType
        {
            get
            {
                return this.feeType;
            }
            set
            {
                this.feeType = value;
            }
        }
        /// <summary>
        /// 急诊分诊流水号
        /// </summary>
        public string Triage_Serialnum { get; set; }


        public string ApplicationOrderNo { get; set; }

        public string PlatformOrderNo { get; set; }

        #endregion
    }
}
