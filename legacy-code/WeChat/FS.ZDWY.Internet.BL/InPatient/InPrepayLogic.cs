using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Data;

namespace FS.ZDWY.Internet.BL.InPatient
{
    public class InPrepayLogic : SqlSugar.DbContext<FS.ZDWY.Internet.Models.FIN_IPB_INPREPAY>
    {
        public string GetHosChargeID(string chargeID)
        {
            string sql = "select a.hospchargeid from platform_inprepay_pay a where a.chargeid = :chargeid";
            var queryData = Db.Ado.GetScalar(sql, new List<SugarParameter>(){
                                new SugarParameter(":chargeid",chargeID)
            });
            if (queryData != null)
            {
                return queryData.ToString();
            }
            return string.Empty;
        }

        public DataTable QueryInPrepay(string patientId, string admissionNo, DateTime startDate, DateTime endDate)
        {
            string sql = @"select
NVL((select A.CHARGEID from PLATFORM_INPREPAY_PAY A WHERE A.INPATIENT_NO = F.INPATIENT_NO AND A.RECEIPTID = F.RECEIPT_NO AND ROWNUM = 1),F.INPATIENT_NO||'-'||F.HAPPEN_NO) ""chargeId""  --业务系统押金单号  必填
,F.INPATIENT_NO||'-'||F.HAPPEN_NO ""hospChargeId""  --院内押金单号  必填
,f.receipt_no ""receiptId"" --收据号 
,f.invoice_no ""invoiceId"" --发票号 
,nvl((select A.Transactionno from PLATFORM_INPREPAY_PAY A WHERE A.INPATIENT_NO = F.INPATIENT_NO AND A.RECEIPTID = F.RECEIPT_NO AND ROWNUM = 1),'无') ""transactionNo"" --支付平台支付流水  必填
,f.prepay_cost*100 ""amount""  --预交金额  必填
,f.balance_date ""chargeTime""  --预交时间  必填
,(case f.prepay_state when '0' then '1'
  else '3' end) ""status""  --预交状态  必填
,f.mark ""remark""  --备注  
from fin_ipb_inprepay f,fin_ipr_inmaininfo t 
where f.prepay_state in('0','1') and f.inpatient_no = t.inpatient_no 
and f.oper_date >= :startDate and f.oper_date <= :endDate  
and ((t.card_no = :patientId or 'ALL' = :patientId) or (t.patient_no = :admissionNo or 'ALL' = :admissionNo ))";

            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                                new SugarParameter(":startDate",startDate),
                                new SugarParameter(":endDate",endDate),
                                new SugarParameter(":patientId",patientId),
                                new SugarParameter(":admissionNo",admissionNo)
            });
            return queryData;
        }

        public DataTable QueryInPrepay(string inpatientNO, string happenNO, string recipeNO)
        {
            string sql = @"select
f.inpatient_no||f.happen_no  ""hospChargeId"",--院内押金单号
f.receipt_no ""receiptId"",   --收据号
f.invoice_no ""invoiceId"",   --发票号
f.prepay_cost *100 ""amount"" ,--预交金额
'1' ""chargeChannel"",  --预交渠道
decode(f.pay_way,'WX','1','ZFB','2','UP','3','MCZH','4','3') ""chargeType"",     --预交类型,
f.oper_date  ""chargeTime"",   --预交时间
(case f.prepay_state when '0' then '1'
  else '3' end) ""status"",  --预交状态  
f.mark ""remark""            --备注
from     fin_ipb_inprepay f,fin_ipr_inmaininfo t 
where    f.inpatient_no = t.inpatient_no 
  and f.prepay_state in ('0','1') 
  and （f.inpatient_no = {0}）
  and （f.happen_no = {1}）";
            sql = string.Format(sql, inpatientNO, happenNO);
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>());
            return queryData;
        }

        public string GetHappenNO(string inpatientNO)
        {
            string sql = @"                  select nvl(max(p.happen_no) + 1,1) from fin_ipb_inprepay p 
                 where p.inpatient_no = :inpatient_no";
            return this.Db.Ado.GetScalar(sql, new List<SugarParameter>(){
                                new SugarParameter(":inpatient_no",inpatientNO)
            }).ToString();
        }

        public string GetInvoiceNo()
        {
            string sql = @"select 'WX'||lpad(Seq_WX_PrePay.Nextval,8,'0') from dual";
            return this.Db.Ado.GetScalar(sql, new List<SugarParameter>()).ToString();
        }
    }
}
