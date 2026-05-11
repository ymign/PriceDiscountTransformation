using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace FS.ZDWY.Internet.Models
{
    public class FIN_IPB_INPREPAY
    {
        /// <summary>
        /// 预交金表
        /// </summary>

        private System.String _INPATIENT_NO;
        /// <summary>
        /// 住院流水号
        /// </summary>
        public System.String INPATIENT_NO { get { return this._INPATIENT_NO; } set { this._INPATIENT_NO = value; } }

        private System.Int32 _HAPPEN_NO;
        /// <summary>
        /// 发生序号
        /// </summary>
        public System.Int32 HAPPEN_NO { get { return this._HAPPEN_NO; } set { this._HAPPEN_NO = value; } }

        private System.String _NAME;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String NAME { get { return this._NAME; } set { this._NAME = value; } }

        private System.Double? _PREPAY_COST;
        /// <summary>
        /// 预交金额
        /// </summary>
        public System.Double? PREPAY_COST { get { return this._PREPAY_COST; } set { this._PREPAY_COST = value; } }

        private System.String _PAY_WAY;
        /// <summary>
        /// 支付方式CA现金CH支票CD信用卡DB借记卡AJ转押金PO汇票PS保险帐户YS院内账户
        /// </summary>
        public System.String PAY_WAY { get { return this._PAY_WAY; } set { this._PAY_WAY = value; } }

        private System.String _DEPT_CODE;
        /// <summary>
        /// 科室代码
        /// </summary>
        public System.String DEPT_CODE { get { return this._DEPT_CODE; } set { this._DEPT_CODE = value; } }

        private System.String _RECEIPT_NO;
        /// <summary>
        /// 预交金收据号码
        /// </summary>
        public System.String RECEIPT_NO { get { return this._RECEIPT_NO; } set { this._RECEIPT_NO = value; } }

        private System.DateTime? _STAT_DATE;
        /// <summary>
        /// 统计日期
        /// </summary>
        public System.DateTime? STAT_DATE { get { return this._STAT_DATE; } set { this._STAT_DATE = value; } }

        private System.DateTime? _BALANCE_DATE;
        /// <summary>
        /// 结算时间
        /// </summary>
        public System.DateTime? BALANCE_DATE { get { return this._BALANCE_DATE; } set { this._BALANCE_DATE = value; } }

        private System.String _BALANCE_STATE;
        /// <summary>
        /// 结算标志 0:未结算；1:已结算 2:已结转
        /// </summary>
        public System.String BALANCE_STATE { get { return this._BALANCE_STATE; } set { this._BALANCE_STATE = value; } }

        private System.String _PREPAY_STATE;
        /// <summary>
        /// 预交金状态0:收取；1:作废;2:补打,3结算召回作废
        /// </summary>
        public System.String PREPAY_STATE { get { return this._PREPAY_STATE; } set { this._PREPAY_STATE = value; } }

        private System.String _OLD_RECIPENO;
        /// <summary>
        /// 原票据号
        /// </summary>
        public System.String OLD_RECIPENO { get { return this._OLD_RECIPENO; } set { this._OLD_RECIPENO = value; } }

        private System.String _OPEN_BANK;
        /// <summary>
        /// 开户银行
        /// </summary>
        public System.String OPEN_BANK { get { return this._OPEN_BANK; } set { this._OPEN_BANK = value; } }

        private System.String _OPEN_ACCOUNTS;
        /// <summary>
        /// 开户帐户
        /// </summary>
        public System.String OPEN_ACCOUNTS { get { return this._OPEN_ACCOUNTS; } set { this._OPEN_ACCOUNTS = value; } }

        private System.String _INVOICE_NO;
        /// <summary>
        /// 结算发票号
        /// </summary>
        public System.String INVOICE_NO { get { return this._INVOICE_NO; } set { this._INVOICE_NO = value; } }

        private System.Int16 _BALANCE_NO;
        /// <summary>
        /// 结算序号
        /// </summary>
        public System.Int16 BALANCE_NO { get { return this._BALANCE_NO; } set { this._BALANCE_NO = value; } }

        private System.String _BALANCE_OPERCODE;
        /// <summary>
        /// 结算人代码
        /// </summary>
        public System.String BALANCE_OPERCODE { get { return this._BALANCE_OPERCODE; } set { this._BALANCE_OPERCODE = value; } }

        private System.String _REPORT_FLAG;
        /// <summary>
        /// 上缴标志（1是 0否）
        /// </summary>
        public System.String REPORT_FLAG { get { return this._REPORT_FLAG; } set { this._REPORT_FLAG = value; } }

        private System.String _CHECK_NO;
        /// <summary>
        /// 审核序号
        /// </summary>
        public System.String CHECK_NO { get { return this._CHECK_NO; } set { this._CHECK_NO = value; } }

        private System.String _FINGRP_CODE;
        /// <summary>
        /// 财务组代码
        /// </summary>
        public System.String FINGRP_CODE { get { return this._FINGRP_CODE; } set { this._FINGRP_CODE = value; } }

        private System.String _WORK_NAME;
        /// <summary>
        /// 工作单位
        /// </summary>
        public System.String WORK_NAME { get { return this._WORK_NAME; } set { this._WORK_NAME = value; } }

        private System.String _TRANS_FLAG;
        /// <summary>
        /// 0非转押金，1转押金，2转押金已打印
        /// </summary>
        public System.String TRANS_FLAG { get { return this._TRANS_FLAG; } set { this._TRANS_FLAG = value; } }

        private System.Int16? _CHANGE_BALANCE_NO;
        /// <summary>
        /// 转押金时结算序号
        /// </summary>
        public System.Int16? CHANGE_BALANCE_NO { get { return this._CHANGE_BALANCE_NO; } set { this._CHANGE_BALANCE_NO = value; } }

        private System.String _TRANS_CODE;
        /// <summary>
        /// 转押金结算员
        /// </summary>
        public System.String TRANS_CODE { get { return this._TRANS_CODE; } set { this._TRANS_CODE = value; } }

        private System.DateTime? _TRANS_DATE;
        /// <summary>
        /// 转押金时间
        /// </summary>
        public System.DateTime? TRANS_DATE { get { return this._TRANS_DATE; } set { this._TRANS_DATE = value; } }

        private System.String _PRINT_FLAG;
        /// <summary>
        /// 打印标志
        /// </summary>
        public System.String PRINT_FLAG { get { return this._PRINT_FLAG; } set { this._PRINT_FLAG = value; } }

        private System.String _EXT_FLAG;
        /// <summary>
        /// 正常收取 1 结算召回 2
        /// </summary>
        public System.String EXT_FLAG { get { return this._EXT_FLAG; } set { this._EXT_FLAG = value; } }

        private System.String _EXT1_FLAG;
        /// <summary>
        /// 日结标志 0未日结 1日结
        /// </summary>
        public System.String EXT1_FLAG { get { return this._EXT1_FLAG; } set { this._EXT1_FLAG = value; } }

        private System.String _POSTRANS_NO;
        /// <summary>
        /// pos交易流水号或支票号或汇票号
        /// </summary>
        public System.String POSTRANS_NO { get { return this._POSTRANS_NO; } set { this._POSTRANS_NO = value; } }

        private System.String _OPER_CODE;
        /// <summary>
        /// 操作员
        /// </summary>
        public System.String OPER_CODE { get { return this._OPER_CODE; } set { this._OPER_CODE = value; } }

        private System.DateTime _OPER_DATE;
        /// <summary>
        /// 操作日期
        /// </summary>
        public System.DateTime OPER_DATE { get { return this._OPER_DATE; } set { this._OPER_DATE = value; } }

        private System.String _OPER_DEPTCODE;
        /// <summary>
        /// 操作员科室
        /// </summary>
        public System.String OPER_DEPTCODE { get { return this._OPER_DEPTCODE; } set { this._OPER_DEPTCODE = value; } }

        private System.String _MARK;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String MARK { get { return this._MARK; } set { this._MARK = value; } }

        private System.String _DAYBALANCE_FLAG;
        /// <summary>
        /// 日结标志 0未日结 1日结
        /// </summary>
        public System.String DAYBALANCE_FLAG { get { return this._DAYBALANCE_FLAG; } set { this._DAYBALANCE_FLAG = value; } }

        private System.String _DAYBALANCE_NO;
        /// <summary>
        /// 日结标识号
        /// </summary>
        public System.String DAYBALANCE_NO { get { return this._DAYBALANCE_NO; } set { this._DAYBALANCE_NO = value; } }

        private System.String _DAYBALANCE_OPCD;
        /// <summary>
        /// 日结人
        /// </summary>
        public System.String DAYBALANCE_OPCD { get { return this._DAYBALANCE_OPCD; } set { this._DAYBALANCE_OPCD = value; } }

        private System.DateTime? _DAYBALANCE_DATE;
        /// <summary>
        /// 日结时间
        /// </summary>
        public System.DateTime? DAYBALANCE_DATE { get { return this._DAYBALANCE_DATE; } set { this._DAYBALANCE_DATE = value; } }

        private System.String _REPAYPRINT;
        /// <summary>
        /// 补打员
        /// </summary>
        public System.String REPAYPRINT { get { return this._REPAYPRINT; } set { this._REPAYPRINT = value; } }

        private System.String _REPAYPRINTFLAG;
        /// <summary>
        /// 补打标记
        /// </summary>
        public System.String REPAYPRINTFLAG { get { return this._REPAYPRINTFLAG; } set { this._REPAYPRINTFLAG = value; } }
    }
}

