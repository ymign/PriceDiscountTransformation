using FS.ZDWY.Internet.BL.InPatient;
using FS.ZDWY.Internet.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FS.ZDWY.Internet.BP.InPatient
{
    /// <summary>
    /// 预交金管理
    /// </summary>
    public class Deposit
    {
        public DataTable QueryInPrepay(string patientId, string admissionNo, DateTime startDate, DateTime endDate)
        {
            FS.ZDWY.Internet.BL.InPatient.InPrepayLogic logic = new BL.InPatient.InPrepayLogic();
            return logic.QueryInPrepay(patientId, admissionNo, startDate, endDate);
        }

        public DataTable QueryInPrepay(string inpatientNO, string happenNO, string recipeNO)
        {
            FS.ZDWY.Internet.BL.InPatient.InPrepayLogic logic = new BL.InPatient.InPrepayLogic();
            return logic.QueryInPrepay(inpatientNO, happenNO, recipeNO);
        }

        public string GetHosChargeID(string chargeID)
        {
            FS.ZDWY.Internet.BL.InPatient.InPrepayLogic logic = new BL.InPatient.InPrepayLogic();
            return logic.GetHosChargeID(chargeID);
        }


        /// <summary>
        /// 住院预交金缴纳
        /// </summary>
        /// <param name="model">住院主表</param>
        /// <param name="chargeId">业务系统流水号</param>
        /// <param name="transactionNo">支付平台支付流水</param>
        /// <param name="chargeTime">预交时间</param>
        /// <param name="chargeChannel">预交渠道</param>
        /// <param name="chargeType">充值类型</param>
        /// <param name="amount">预交金额</param>
        /// <param name="prepayModel">返回支付实体</param>
        /// <param name="errInfo">返回错误信息</param>
        /// <returns></returns>
        public int PrePay(FS.ZDWY.Internet.Models.FIN_IPR_INMAININFO model, string chargeId, string transactionNo, string chargeTime, string chargeChannel, string chargeType, decimal amount, string operCode, string applicationOrderNo, string platformOrderNo, out FS.ZDWY.Internet.Models.FIN_IPB_INPREPAY prepayModel, out string errInfo)
        {
            errInfo = string.Empty;
            prepayModel = new Models.FIN_IPB_INPREPAY();
            if (amount == 0)
            {
                errInfo = "押金金额不能为零！";
                return 0;
            }

            string paytype = string.Empty;
            string recept_no = string.Empty;
            string happenNo = GetHappenNO(model.INPATIENT_NO);

            FS.ZDWY.Internet.BL.InPatient.InPrepayLogic logic = new BL.InPatient.InPrepayLogic();
            FS.ZDWY.Internet.BL.InPatient.InMainInfoLogic inMainInfoLogic = new BL.InPatient.InMainInfoLogic();
            FS.ZDWY.Internet.BL.InPatient.InPrepayLogLogic inprepayLogLogic = new BL.InPatient.InPrepayLogLogic();

            #region 支付方式对照
            //支付方式对照
            if (chargeType.Equals("1"))
            {
                paytype = "WX";
            }
            else if (chargeType.Equals("2"))
            {
                paytype = "ZFB";
            }
            else if (chargeType.Equals("3"))
            {
                paytype = "UP";
            }
            else if (chargeType.Equals("4"))
            {
                paytype = "MCZH";
            }
            else if (chargeType.Equals("6"))
            {
                paytype = "YBXYF";
            }
            else
            {
                paytype = "WX";
            }

            #endregion

            recept_no = GetInvoiceNo();

            if (string.IsNullOrEmpty(recept_no) || recept_no == "-1")
            {
                errInfo = "生成发票序列出错！";
                return 0;
            }

            logic.BeginTran();

            #region 插入预交金表

            prepayModel.INPATIENT_NO = model.INPATIENT_NO;
            prepayModel.HAPPEN_NO = Convert.ToInt32(happenNo);
            prepayModel.NAME = model.NAME;
            prepayModel.PAY_WAY = paytype;
            prepayModel.PREPAY_COST = Convert.ToDouble(amount / 100);
            prepayModel.DEPT_CODE = model.DEPT_CODE;
            prepayModel.RECEIPT_NO = recept_no;
            prepayModel.BALANCE_STATE = "0";
            prepayModel.PREPAY_STATE = "0";
            prepayModel.BALANCE_NO = 0;
            prepayModel.REPORT_FLAG = "0";
            prepayModel.TRANS_FLAG = "0";
            prepayModel.CHANGE_BALANCE_NO = 0;
            prepayModel.PRINT_FLAG = "0";
            prepayModel.EXT1_FLAG = "0";
            prepayModel.OPER_CODE = operCode;
            prepayModel.OPER_DATE = Convert.ToDateTime(chargeTime);
            prepayModel.DAYBALANCE_FLAG = "0";
            prepayModel.EXT_FLAG = "1";
            prepayModel.EXT1_FLAG = "0";

            if (!logic.Insert(prepayModel))
            {
                errInfo = "住院号:" + model.PATIENT_NO + "插入预交金表失败";
                logic.RollbackTran();
                return 0;
            }

            
            #endregion

            //插入支付平台记录
            if (!string.IsNullOrWhiteSpace(applicationOrderNo) && !string.IsNullOrWhiteSpace(platformOrderNo))
            {
                FinTransRecord payRecordInfo = new FinTransRecord();
                payRecordInfo.Id = Guid.NewGuid().ToString();
                payRecordInfo.TransactionNo = recept_no;
                payRecordInfo.TransType = "1";
                payRecordInfo.ClientCode = "ZDWY_WXGZH";
                payRecordInfo.PlatformOrderNo = platformOrderNo;
                payRecordInfo.ApplicationOrderNo = applicationOrderNo;
                string PayChannelCode = "";
                if (paytype == "WX")
                {
                    PayChannelCode = "WeChat_FKM";
                }
                else if (paytype == "ZFB")
                {
                    PayChannelCode = "ZFB_FKM";
                }
                else
                {
                    errInfo = "插入支付交易记录失败:支付方式不符合要求" + paytype + "";
                    logic.RollbackTran();
                    return 0;
                }
                payRecordInfo.PayChannelCode = PayChannelCode;
                payRecordInfo.TransAmount = Convert.ToDecimal(amount / 100);
                payRecordInfo.OrderBigType = "1";
                payRecordInfo.OrderSmallType = "01";
                payRecordInfo.PatientNo = model.CARD_NO;
                payRecordInfo.PatientName = model.NAME;
                payRecordInfo.Businessno = model.INPATIENT_NO;
                payRecordInfo.CreatedCode = operCode;
                payRecordInfo.CreatedName = "微信";
                payRecordInfo.CreatedTime = DateTime.Now;
                payRecordInfo.HospitalCode = "H44040200001";
                FinTransRecordLogic recordLogic = new FinTransRecordLogic();
                if (!recordLogic.Insert(payRecordInfo))
                {
                    errInfo = "住院号:" + model.PATIENT_NO + "插入支付平台记录表失败";
                    logic.RollbackTran();
                    return 0;
                }
                
            }


            #region 更新费用明细

            if (inMainInfoLogic.UpdatePrepayFee(model.INPATIENT_NO, Convert.ToDecimal(prepayModel.PREPAY_COST)) <= 0)
            {
                errInfo = "住院号:" + model.PATIENT_NO + "更新费用明细失败";
                logic.RollbackTran();
                return 0;
            }

            #endregion

            #region 记录日志表

            FS.ZDWY.Internet.Models.PLATFORM_INPREPAY_PAY platModel = new Models.PLATFORM_INPREPAY_PAY();
            platModel.CHARGEID = chargeId;
            platModel.TRANSACTIONNO = transactionNo;
            platModel.CHARGETIME = Convert.ToDateTime(chargeTime);
            platModel.CHARGECHANNEL = chargeChannel;
            platModel.CHARGETYPE = chargeType;
            platModel.AMOUNT = Math.Round(amount / 100, 2);
            platModel.PATIENTID = model.CARD_NO;
            platModel.ADMISSIONNO = model.PATIENT_NO;
            platModel.NAME = model.NAME;
            platModel.INPATIENT_NO = model.INPATIENT_NO;
            platModel.HOSPCHARGEID = model.INPATIENT_NO + "-" + happenNo;
            platModel.BALANCE = Convert.ToDecimal(model.FREE_COST) + Convert.ToDecimal(amount);
            platModel.RECEIPTID = recept_no;
            platModel.INVOICEID = recept_no;
            platModel.OPER_ID = operCode;
            platModel.OPER_TIME = Convert.ToDateTime(chargeTime);

            try
            {
                if (!inprepayLogLogic.Insert(platModel))
                {
                    errInfo = "住院号:" + model.PATIENT_NO + "更新费用明细失败";
                    logic.RollbackTran();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                errInfo = "住院号:" + model.PATIENT_NO + "更新费用明细失败";
                logic.RollbackTran();
                return 0;
            }


            #endregion

            logic.CommitTran();

            return 1;
        }

        /// <summary>
        /// 获取发生序号
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        private string GetHappenNO(string inpatientNO)
        {
            FS.ZDWY.Internet.BL.InPatient.InPrepayLogic logic = new BL.InPatient.InPrepayLogic();
            return logic.GetHappenNO(inpatientNO);
        }

        /// <summary>
        /// 获取账户收据
        /// </summary>
        /// <returns></returns>
        private string GetInvoiceNo()
        {
            FS.ZDWY.Internet.BL.InPatient.InPrepayLogic logic = new BL.InPatient.InPrepayLogic();
            return logic.GetInvoiceNo();
        }
    }
}
