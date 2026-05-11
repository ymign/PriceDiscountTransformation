using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    public class PLATFORM_INPREPAY_PAY
    {

        /// <summary>
        /// 互联网医院_住院预交金对照表
        /// </summary>
        public PLATFORM_INPREPAY_PAY()
        {
        }

        private System.String _CHARGEID;
        /// <summary>
        /// 业务系统押金单号
        /// </summary>
        public System.String CHARGEID { get { return this._CHARGEID; } set { this._CHARGEID = value; } }

        private System.String _TRANSACTIONNO;
        /// <summary>
        /// 支付平台支付流水
        /// </summary>
        public System.String TRANSACTIONNO { get { return this._TRANSACTIONNO; } set { this._TRANSACTIONNO = value; } }

        private System.DateTime? _CHARGETIME;
        /// <summary>
        /// 预交时间
        /// </summary>
        public System.DateTime? CHARGETIME { get { return this._CHARGETIME; } set { this._CHARGETIME = value; } }

        private System.String _CHARGECHANNEL;
        /// <summary>
        /// 预交渠道
        /// </summary>
        public System.String CHARGECHANNEL { get { return this._CHARGECHANNEL; } set { this._CHARGECHANNEL = value; } }

        private System.String _CHARGETYPE;
        /// <summary>
        /// 充值类型
        /// </summary>
        public System.String CHARGETYPE { get { return this._CHARGETYPE; } set { this._CHARGETYPE = value; } }

        private System.Decimal? _AMOUNT;
        /// <summary>
        /// 预交金额
        /// </summary>
        public System.Decimal? AMOUNT { get { return this._AMOUNT; } set { this._AMOUNT = value; } }

        private System.String _PATIENTID;
        /// <summary>
        /// 院内用户id
        /// </summary>
        public System.String PATIENTID { get { return this._PATIENTID; } set { this._PATIENTID = value; } }

        private System.String _ADMISSIONNO;
        /// <summary>
        /// 用户住院号
        /// </summary>
        public System.String ADMISSIONNO { get { return this._ADMISSIONNO; } set { this._ADMISSIONNO = value; } }

        private System.String _NAME;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String NAME { get { return this._NAME; } set { this._NAME = value; } }

        private System.String _INPATIENT_NO;
        /// <summary>
        /// 住院流水号
        /// </summary>
        public System.String INPATIENT_NO { get { return this._INPATIENT_NO; } set { this._INPATIENT_NO = value; } }

        private System.String _HOSPCHARGEID;
        /// <summary>
        /// 医院押金单号
        /// </summary>
        public System.String HOSPCHARGEID { get { return this._HOSPCHARGEID; } set { this._HOSPCHARGEID = value; } }

        private System.Decimal? _BALANCE;
        /// <summary>
        /// 余额
        /// </summary>
        public System.Decimal? BALANCE { get { return this._BALANCE; } set { this._BALANCE = value; } }

        private System.String _RECEIPTID;
        /// <summary>
        /// 收据号
        /// </summary>
        public System.String RECEIPTID { get { return this._RECEIPTID; } set { this._RECEIPTID = value; } }

        private System.String _INVOICEID;
        /// <summary>
        /// 发票号
        /// </summary>
        public System.String INVOICEID { get { return this._INVOICEID; } set { this._INVOICEID = value; } }

        private System.String _OPER_ID;
        /// <summary>
        /// 操作人
        /// </summary>
        public System.String OPER_ID { get { return this._OPER_ID; } set { this._OPER_ID = value; } }

        private System.DateTime? _OPER_TIME;
        /// <summary>
        /// 操作时间
        /// </summary>
        public System.DateTime? OPER_TIME { get { return this._OPER_TIME; } set { this._OPER_TIME = value; } }
    }
}
