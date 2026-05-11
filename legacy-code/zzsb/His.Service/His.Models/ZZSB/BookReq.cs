using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
  public  class BookReq
    {

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

        private string pwd;
      /// <summary>
      /// 操作员密码
      /// </summary>
        public string PassWord
        {
            get
            {
                return pwd;
            }
            set
            {
                pwd = value;
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

        private string bandCode;
      /// <summary>
      /// 银行编码
      /// </summary>
        public string BankCode
        {
            get
            {
                return bandCode;
            }
            set
            {
                bandCode = value;
            }
        }

        private string cardno;
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo
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
        public string HospCode
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

        private string cardTypeCode;
      /// <summary>
      /// 卡类型编码
      /// </summary>
        public string CardTypeCode
        {
            get
            {
                return cardTypeCode;
            }
            set
            {
                cardTypeCode = value;
            }
        }

        private string patientid;
        /// <summary>
        /// 患者ID号
        /// </summary>
        public string PatientID
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

        private string regDate;
        /// <summary>
        /// 取号的日期
        /// </summary>
        public string RegDate
        {
            get
            {
                return regDate;
            }
            set
            {
                regDate = value;
            }
        }

        private string reqTraceNo;
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string ReqTraceNo
        {
            get
            {
                return reqTraceNo;
            }
            set
            {
                reqTraceNo = value;
            }
        }
        private string payAmt;
        /// <summary>
        /// 交易金额
        /// </summary>
        public string PayAmt
        {
            get
            {
                return payAmt;
            }
            set
            {
                payAmt = value;
            }
        }
        private string feeType;
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
        public string AppCode { get; set; }
        public string AppTypeCode { get; set; }
        public DateTime ReqTime { get; set; }
      //  public string ReqTraceNo { get; set; }
    }

  public class SubmitBookingReq :BookReq
  {
      public string TotalRegFee { get; set; }
      public string ordercode { get; set; }
      public string PayType { get; set; }
      public string Payinsufeestr { get; set; }
      public bool IsBook { get; set; }
      public string BankCardNo { get; set; }
      public string VouchNo { get; set; }
  }

  public class BookDeptReq : BookReq
  {
      /// <summary>
      /// 出诊科室
      /// </summary>
      public string DeptCode { get; set; }

      /// <summary>
      /// 预约日期
      /// </summary>
      public string RegDate { get; set; }
  }

  public class BookDoctReq : BookReq
  {
      /// <summary>
      /// 出诊科室
      /// </summary>
      public string DeptCode { get; set; }

      /// <summary>
      /// 预约日期
      /// </summary>
      public string RegDate { get; set; }

      /// <summary>
      /// 预约出诊医生
      /// </summary>
      public string DoctCode { get; set; }
  }

  public class ItemDictionaries : BookReq
  {
      //public string AppCode { get; set; }
      public string AppTypeCode { get; set; }
     // public string ReqTime { get; set; }
  }

  public class ItemDictionary : BookReq
  {
      //public string AppCode { get; set; }
      public string AppTypeCode { get; set; }
      //public string ReqTime { get; set; }
      public string TypeId { get; set; }
  }


}
