using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FS.ZDWY.Internet.Models;
using System.Data;
using SqlSugar;

namespace FS.ZDWY.Internet.BL.InPatient
{
    public class InMainInfoLogic : SqlSugar.DbContext<FIN_IPR_INMAININFO>
    {
        public DataTable QueryInMainInfo(string patientId, DateTime startDate, DateTime endDate)
        {
            string sql = @"select
                    t.inpatient_no ""visitNo""   --就诊号   必填
                    ,t.dept_code ""departmentId"" --科室ID  必填
                    ,t.dept_name ""departmenName""   --科室名称  必填
                    ,t.house_doc_code ""doctorId""  --医生ID  必填
                    ,t.house_doc_name ""doctorName""  --医生名称  必填
                    ,t.in_date ""registerDate""  --就诊日期  必填
                    ,'0'  ""amount""  --挂号金额  必填
                    from fin_ipr_inmaininfo t where t.card_no = :patientId and t.in_date >= :startDate and t.out_date <= :endDate order by t.in_date desc";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                        new SugarParameter(":patientId",patientId),
                        new SugarParameter(":startDate",startDate),
                        new SugarParameter(":endDate",endDate)
            });
            return queryData;
        }

        public DataTable QueryInMainDayFeeIn(string patientID, string inpatientNO, string visitNo,string idenNO, DateTime startDate, DateTime endTime)
        {
            if (string.IsNullOrEmpty(patientID))
            {
                patientID = "ALL";
            }
            if (string.IsNullOrEmpty(inpatientNO))
            {
                inpatientNO = "ALL";
            }
            if (string.IsNullOrEmpty(idenNO))
            {
                idenNO = "ALL";
            }
            if (string.IsNullOrEmpty(visitNo))
            {
                visitNo = "ALL";
            }
            string sql = @"select fee_date ""inDate"",--住院日期
       sum(decode(cost_type,'1',tot_cost,'0'))*100  ""dayAmount"",--当日产生总费用
       sum(decode(cost_type,'2',tot_cost,'0'))*100 ""chargeAmount"",--当日充值费用
       '' ""remark"",      --备注
       out_date   ""outDate""      --出院时间                
       from 
(
select trunc(a.fee_date) fee_date,'1' cost_type,sum(a.tot_cost) tot_cost,r.out_date from fin_ipb_feeinfo a,fin_ipr_inmaininfo r 
where a.inpatient_no = r.inpatient_no 
 and   (r.card_no = :patientID or 'ALL' = :patientID)
 and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
 and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
 and   (r.idenno = :idenNo or 'ALL' = :idenNo)
 and   a.fee_date >= :beginDate
 and   a.fee_date <  :endDate
 --and   r.in_state in ('R','I')
 and   r.in_state <>'O'
group by trunc(a.fee_date),r.out_date
union
select trunc(b.oper_date) fee_date,'2' cost_type,sum(b.prepay_cost) tot_cost,r.out_date from fin_ipb_inprepay b,fin_ipr_inmaininfo r 
 where b.inpatient_no = r.inpatient_no
   and   (r.card_no = :patientID or 'ALL' = :patientID)
   and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
   and   (r.idenno = :idenNo or 'ALL' = :idenNo)
   and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
   and b.oper_date >= :beginDate
   and b.oper_date <  :endDate
   --and r.in_state in ('R','I')
   and   r.in_state <>'O'
group by trunc(b.oper_date),r.out_date
) group by fee_date,out_date
order by fee_date";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                        new SugarParameter(":patientID",patientID),
                        new SugarParameter(":patientNO",inpatientNO),
                        new SugarParameter(":visitNo",visitNo),
                        new SugarParameter(":idenNo",idenNO),
                        new SugarParameter(":beginDate",startDate),
                        new SugarParameter(":endDate",endTime)
            });
            return queryData;
        }

        public DataTable QueryInMainDayFeeOut(string patientID, string inpatientNO, string visitNo,string idenNO, DateTime startDate, DateTime endTime)
        {
            if (string.IsNullOrEmpty(patientID))
            {
                patientID = "ALL";
            }
            if (string.IsNullOrEmpty(inpatientNO))
            {
                inpatientNO = "ALL";
            }
            if (string.IsNullOrEmpty(idenNO))
            {
                idenNO = "ALL";
            }
            if (string.IsNullOrEmpty(visitNo))
            {
                visitNo = "ALL";
            }
            string sql = @"select fee_date ""inDate"",--住院日期
       sum(decode(cost_type,'1',tot_cost,'0'))*100 ""dayAmount"",--当日产生总费用
       sum(decode(cost_type,'2',tot_cost,'0'))*100 ""chargeAmount"",--当日充值费用
       '' ""remark""      --备注                         
       from 
(
select trunc(a.fee_date) fee_date,'1' cost_type,sum(a.tot_cost) tot_cost from fin_ipb_feeinfo a,fin_ipr_inmaininfo r 
where a.inpatient_no = r.inpatient_no 
 and   (r.card_no =:patientID or 'ALL' = :patientID)
 and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
 and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
 --and   (r.idenno = :idenNo or 'ALL' = :idenNo)
 and   a.fee_date >= :beginDate
 and   a.fee_date <  :endDate
 and   r.in_state not in ('R','I')
group by trunc(a.fee_date)
union
select trunc(b.oper_date) fee_date,'2' cost_type,sum(b.prepay_cost) tot_cost from fin_ipb_inprepay b,fin_ipr_inmaininfo r 
 where b.inpatient_no = r.inpatient_no
   and   (r.card_no = :patientID or 'ALL' = :patientID)
   and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
 and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
  --and   (r.idenno = :idenNo or 'ALL' = :idenNo)
   and b.oper_date >= :beginDate
   and b.oper_date <  :endDate
   and r.in_state not in ('R','I')
group by trunc(b.oper_date)
) group by fee_date
order by fee_date";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                        new SugarParameter(":patientID",patientID),
                        new SugarParameter(":patientNO",inpatientNO),
                        new SugarParameter(":visitNo",visitNo),
                        new SugarParameter(":idenno",idenNO),
                        new SugarParameter(":beginDate",startDate),
                        new SugarParameter(":endDate",endTime)
            });
            return queryData;
        }

        public DataTable QueryInMainDayFeeALL(string patientID, string inpatientNO, string visitNo, string idenNO, DateTime startDate, DateTime endTime)
        {
            if (string.IsNullOrEmpty(patientID))
            {
                patientID = "ALL";
            }
            if (string.IsNullOrEmpty(inpatientNO))
            {
                inpatientNO = "ALL";
            }
            if (string.IsNullOrEmpty(idenNO))
            {
                idenNO = "ALL";
            }
            if (string.IsNullOrEmpty(visitNo))
            {
                visitNo = "ALL";
            }
            string sql = @"select fee_date ""inDate"",--住院日期
       sum(decode(cost_type,'1',tot_cost,'0'))*100 ""dayAmount"",--当日产生总费用
       sum(decode(cost_type,'2',tot_cost,'0'))*100 ""chargeAmount"",--当日充值费用
       '' ""remark""      --备注                         
       from 
(
select trunc(a.fee_date) fee_date,'1' cost_type,sum(a.tot_cost) tot_cost from fin_ipb_feeinfo a,fin_ipr_inmaininfo r 
where a.inpatient_no = r.inpatient_no 
 and   (r.card_no =:patientID or 'ALL' = :patientID)
 and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
 and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
 --and   (r.idenno = :idenNo or 'ALL' = :idenNo)
 and   a.fee_date >= :beginDate
 and   a.fee_date <  :endDate
group by trunc(a.fee_date)
union
select trunc(b.oper_date) fee_date,'2' cost_type,sum(b.prepay_cost) tot_cost from fin_ipb_inprepay b,fin_ipr_inmaininfo r 
 where b.inpatient_no = r.inpatient_no
   and   (r.card_no = :patientID or 'ALL' = :patientID)
   and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
 and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
  --and   (r.idenno = :idenNo or 'ALL' = :idenNo)
   and b.oper_date >= :beginDate
   and b.oper_date <  :endDate
group by trunc(b.oper_date)
) group by fee_date
order by fee_date";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                        new SugarParameter(":patientID",patientID),
                        new SugarParameter(":patientNO",inpatientNO),
                        new SugarParameter(":visitNo",visitNo),
                        new SugarParameter(":idenno",idenNO),
                        new SugarParameter(":beginDate",startDate),
                        new SugarParameter(":endDate",endTime)
            });
            return queryData;
        }

        public DataTable QueryInMainfoByPatients(string patientIDS, string inpatientNOs,string visitNumber, string idenNo, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(patientIDS))
            {
                patientIDS = "ALL";
            }
            if (string.IsNullOrEmpty(inpatientNOs))
            {
                inpatientNOs = "ALL";
            }
            if (string.IsNullOrEmpty(idenNo))
            {
                idenNo = "ALL";
            }
            if (string.IsNullOrEmpty(visitNumber))
            {
                visitNumber = "ALL";
            }
            if (startDate == DateTime.MinValue)
            {
                startDate = this.GetDateTime().AddYears(-10);
            }
            if (endDate == DateTime.MinValue)
            {
                endDate = this.GetDateTime();
            }
            //    string sql = @"select 
            //aa.name   ""patientName""  ,      --住院人
            //aa.patient_no   ""inpatId""  ,--住院号
            //aa.dept_code   ""departmentId""   ,--科室ID
            //aa.dept_name   ""departmenName""  ,--科室名称
            //aa.bed_no   ""bedNo""  ,          --床位号
            //decode(aa.in_state,'I','','R','','N','',to_char(aa.out_date,'yyyy-mm-dd'))   ""outDate""  ,      --出院日期
            //to_char(trunc(aa.in_date),'yyyy-mm-dd')   ""inDate""  ,        --住院日期
            //decode(aa.in_state,'I','0','O','1','B','2','R','3','-1')   ""type""  , --住院出院状态
            //aa.inpatient_no   ""inpatNumber""  ,                     --住院流水号
            //aa.card_no   ""patientId"" ,
            //(decode(aa.out_date,to_date('0002-01-01','yyyy-mm-dd'),trunc(SYSDATE),aa.out_date,to_date('0001-01-01','yyyy-mm-dd'),trunc(SYSDATE),aa.out_date) - trunc(aa.in_date)) ""dayCount"",
            //decode(aa.in_state,'O',aa.balance_prepay * 100,aa.prepay_cost * 100) ""depositAmount"",  
            //decode(aa.in_state,'O',aa.balance_cost * 100,aa.tot_cost * 100) ""totalAmount"",
            //aa.pub_cost*100 ""medicareAmount"",
            //aa.free_cost*100 ""amount"",
            //aa.charge_doc_code ""doctorId"",
            //decode(aa.paykind_code,'01','0','1') ""isMedicare"",
            //fun_get_employee_name(aa.charge_doc_code) ""doctorName"",
            //aa.duty_nurse_code ""nurseId"", 
            //fun_get_employee_name(aa.duty_nurse_code) ""nurseName"",
            //nvl((SELECT decode(d.record_status,'6','1','0') FROM met_mrs_base d
            //WHERE d.inpatient_no = aa.inpatient_no),'0') ""archive""
            //from fin_ipr_inmaininfo aa
            //where (aa.card_no in   (select * from　the (select cast(f_str2List(:patientId) as varchar2TableType) from　dual)) 
            //or 'ALL' = :patientId)
            //  and (aa.patient_no in (select * from　the (select cast(f_str2List(:admissionNo) as varchar2TableType) from　dual))
            //       or 'ALL' = :admissionNo)
            //  and (aa.idenno = :certifcateNo or 'ALL' = :certifcateNo)
            //  and (aa.Inpatient_No = :Inpatient_No or 'ALL' = :Inpatient_No)
            //  --and aa.in_date >= :startDate
            //  --and aa.out_date <  :endDate
            //  order by aa.inpatient_no";

            //    var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
            //                new SugarParameter(":patientId",patientIDS),
            //                new SugarParameter(":admissionNo",inpatientNOs),
            //                new SugarParameter(":Inpatient_No",visitNumber),
            //                new SugarParameter(":certifcateNo",idenNo),
            //                new SugarParameter(":startDate",startDate),
            //                new SugarParameter(":endDate",endDate)
            //    });

            string sql = @"select 
            aa.name   ""patientName""  ,      --住院人
            aa.patient_no   ""inpatId""  ,--住院号
            aa.dept_code   ""departmentId""   ,--科室ID
            aa.dept_name   ""departmenName""  ,--科室名称
            aa.bed_no   ""bedNo""  ,          --床位号
            decode(aa.in_state,'I','','R','','N','',to_char(aa.out_date,'yyyy-mm-dd'))   ""outDate""  ,      --出院日期
            to_char(trunc(aa.in_date),'yyyy-mm-dd')   ""inDate""  ,        --住院日期
            decode(aa.in_state,'I','0','O','1','B','2','R','3','N','1','-1')   ""type""  , --住院出院状态
            aa.inpatient_no   ""inpatNumber""  ,                     --住院流水号
            aa.card_no   ""patientId"" ,
            (decode(aa.out_date,to_date('0002-01-01','yyyy-mm-dd'),trunc(SYSDATE),aa.out_date,to_date('0001-01-01','yyyy-mm-dd'),trunc(SYSDATE),aa.out_date) - trunc(aa.in_date)) ""dayCount"",
            decode(aa.in_state,'O',aa.balance_prepay * 100,aa.prepay_cost * 100) ""depositAmount"",  
            decode(aa.in_state,'O',aa.balance_cost * 100,aa.tot_cost * 100) ""totalAmount"",
            aa.pub_cost*100 ""medicareAmount"",
            aa.free_cost*100 ""amount"",
            aa.charge_doc_code ""doctorId"",
            decode(aa.paykind_code,'01','0','1') ""isMedicare"",
            fun_get_employee_name(aa.charge_doc_code) ""doctorName"",
            aa.duty_nurse_code ""nurseId"", 
            fun_get_employee_name(aa.duty_nurse_code) ""nurseName"",
            nvl((SELECT decode(d.record_status,'6','1','0') FROM met_mrs_base d
            WHERE d.inpatient_no = aa.inpatient_no),'0') ""archive""
            from fin_ipr_inmaininfo aa
            where (aa.card_no in   (select * from　the (select cast(f_str2List('{0}') as varchar2TableType) from　dual)) 
            or 'ALL' = '{0}')
              and (aa.patient_no in (select * from　the (select cast(f_str2List('{1}') as varchar2TableType) from　dual))
                   or 'ALL' = '{1}')
              and (aa.idenno = '{2}' or 'ALL' = '{2}')
              and (aa.Inpatient_No = '{3}' or 'ALL' = '{3}')
              --and aa.in_date >= to_date('{4}','yyyy-mm-dd hh24:mi:ss')
              --and aa.out_date <  to_date('{5}','yyyy-mm-dd hh24:mi:ss')
              order by aa.inpatient_no";


            string strSql = string.Format(sql, patientIDS,inpatientNOs, idenNo,visitNumber, startDate, endDate);
            var queryData = Db.Ado.GetDataTable(strSql);



            return queryData;
        }

        public DataTable QueryInMainInfoDetail(string inState, string patientID, string inpatientNO, string visitno, string idenNo, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(patientID))
            {
                patientID = "ALL";
            }
            if (string.IsNullOrEmpty(inpatientNO))
            {
                inpatientNO = "ALL";
            }
            if (string.IsNullOrEmpty(idenNo))
            {
                idenNo = "ALL";
            }
            if (string.IsNullOrEmpty(visitno))
            {
                visitno = "ALL";
            }
            string sql = @"
select aa.inDate ""inDate"",--住院日期
       aa.dayAmount ""dayAmount"",--当日产生总费用
       aa.chargeAmount ""chargeAmount"",--当日充值费用
       bb.feeType ""feeType"",--费用分类编码
       bb.feeName ""feeName"",--费用分类名称
       bb.code ""code"",     --项目编码
       bb.name ""name"",     --项目名称
       bb.unit ""unit"",     --单位
       bb.price ""price"",   --单价
       bb.count ""count"",   --数量
       bb.spec ""spec"",     --规格
       bb.amount ""amount""  --项目总金额
 from 
(
select fee_date as inDate,sum(decode(cost_type,'1',tot_cost,0)) as dayAmount,
sum(decode(cost_type,'2',tot_cost,0)) as chargeAmount
  from
(
select trunc(a.fee_date) as fee_date,--住院日期
       '1' as cost_type,           --费用类型
       sum(a.tot_cost)*100 as tot_cost--当日产生总费用
 from fin_ipb_feeinfo a,fin_ipr_inmaininfo r
where a.inpatient_no = r.inpatient_no
and   r.in_state in (select * from　the (select cast(f_str2List(:inState) as varchar2TableType) from　dual))
and   (r.card_no = :patientID or 'ALL' = :patientID)
and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
 and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
and   (r.idenno = :idenNo or 'ALL' = :idenNo)
and   a.fee_date >= :beginDate
and   a.fee_date <  :endDate
group by trunc(a.fee_date)
union all
select trunc(b.oper_date) fee_date,
   '2' cost_type,sum(b.prepay_cost)*100 tot_cost from fin_ipb_inprepay b,fin_ipr_inmaininfo r 
 where b.inpatient_no = r.inpatient_no
   and   (r.card_no = :patientID or 'ALL' = :patientID)
   and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
 and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
   and   (r.idenno = :idenNo or 'ALL' = :idenNo)
   and b.oper_date >= :beginDate
   and b.oper_date <  :endDate
and   r.in_state in (select * from　the (select cast(f_str2List(:inState) as varchar2TableType) from　dual))
group by trunc(b.oper_date)
) group by fee_date
) aa,
(
select trunc(cc.fee_date) fee_date,cc.feeType,cc.feeName,cc.code,cc.name,cc.unit,cc.price,sum(cc.count) as count,
  cc.spec,sum(cc.amount) as amount from 
(
select c.fee_date,c.fee_code as feeType,fun_get_dictionary_name('MINFEE',c.fee_code) as  feeName,
       c.drug_code as code,c.drug_name as name,c.current_unit as unit,round(c.unit_price/c.pack_qty,2)*100 as price,c.qty as count,
       c.specs as spec,c.tot_cost*100 as amount
 from fin_ipb_medicinelist c,fin_ipr_inmaininfo r
where c.inpatient_no = r.inpatient_no
  and    (r.card_no = :patientID or 'ALL' = :patientID)
   and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
 and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
   and   (r.idenno = :idenNo or 'ALL' = :idenNo)
   and   c.fee_date >= :beginDate
   and   c.fee_date <  :endDate
   and   r.in_state in (select * from　the (select cast(f_str2List(:inState) as varchar2TableType) from　dual))
   union all
  select d.fee_date,d.fee_code as feeType,fun_get_dictionary_name('MINFEE',d.fee_code) as  feeName,
         d.item_code as code,d.item_name as name,d.current_unit as unit,d.unit_price*100 as price,
         d.qty as count,d.current_unit as spec,d.tot_cost*100 as amount
    from fin_ipb_itemlist d,fin_ipr_inmaininfo r
 where   d.inpatient_no = r.inpatient_no
   and     (r.card_no = :patientID or 'ALL' = :patientID)
   and   (r.patient_no = :patientNO or 'ALL' = :patientNO)
 and   (r.inpatient_no = :visitNo or 'ALL' = :visitNo)
   and   (r.idenno = :idenNo or 'ALL' = :idenNo)
   and   d.fee_date >= :beginDate
   and   d.fee_date <  :endDate
   and   r.in_state in (select * from　the (select cast(f_str2List(:inState) as varchar2TableType) from　dual))
   ) cc group by trunc(cc.fee_date),cc.feeType,cc.feeName,cc.code,cc.name,cc.unit,cc.price,
  cc.spec
  ) bb where aa.inDate = bb.fee_date order by aa.inDate";

            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                        new SugarParameter(":inState",inState),
                        new SugarParameter(":patientID",patientID),
                        new SugarParameter(":patientNO",inpatientNO),
                        new SugarParameter(":visitNo",visitno),
                        new SugarParameter(":idenNo",idenNo),
                        new SugarParameter(":beginDate",startDate),
                        new SugarParameter(":endDate",endDate)
            });
            return queryData;
        }

        public DataTable QueryOutSummay(string visitNo, string inpatientNO)
        {
            string sql = @"SELECT 
       AA.PATIENT_NAME ""patientName"",
       decode(AA.SEX_CODE,'F','女'，'男') ""sex"",
       BB.patient_no ""inpatId"",
       BB.DEPT_CODE ""departmentId"",
       BB.DEPT_NAME ""departmenName"",
       TO_CHAR(BB.OUT_DATE,'YYYY-MM-DD') ""outDate"",
       TO_CHAR(BB.IN_DATE,'YYYY-MM-DD') ""inDate"",
       round((BB.OUT_DATE - BB.IN_DATE),0) ""dayCount"",
       bb.clinic_diagnose ""inDiagnosis"",
       bb.diag_name ""outDiagnosis"",
       aa.dishospital_signs ""advice"",
       bb.prepay_cost * 100 depositAmount,
       bb.tot_cost * 100 ""totalAmount"",
       bb.pub_cost * 100 ""medicareAmount"",
       bb.charge_doc_code ""doctorId"",
       bb.charge_doc_name ""doctorName "",
       bb.duty_nurse_code ""nurseId"",
       bb.duty_nurse_name ""nurseName""
 FROM EMR.V_DISHOSPITAL_SUMMARY_wx aa,fin_ipr_inmaininfo bb
WHERE aa.inpatient_no = bb.inpatient_no
  AND aa.inp_no = bb.patient_no
  AND aa.inpatient_no = :inpatient_no
  AND bb.patient_no = :patient_no ";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                        new SugarParameter(":inpatient_no",visitNo),
                        new SugarParameter(":patient_no",inpatientNO)
            });
            return queryData;

        }

        public List<FS.ZDWY.Internet.Models.FIN_IPR_INMAININFO> QueryInMainInfoList(string patientId, string admissionNo,string visitNo, string certifcateNo, string name)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                patientId = "ALL";
            }
            if (string.IsNullOrEmpty(admissionNo))
            {
                admissionNo = "ALL";
            }
            if (string.IsNullOrEmpty(certifcateNo))
            {
                certifcateNo = "ALL";
            }
            if (string.IsNullOrEmpty(visitNo))
            {
                visitNo = "ALL";
            }
            string sql = @"select *
                    from fin_ipr_inmaininfo t 
                    where (t.card_no = :patientId or 'ALL' = :patientId)
                      and (t.patient_no = :admissionNo or 'ALL' = :admissionNo)
                      and (t.inpatient_no = :visitNo or 'ALL' = :visitNo)
                      and (t.idenno = :certifcateNo or 'ALL' = :certifcateNo)
                      and t.name = :name
                      order by t.in_date desc ";

            return Db.Ado.SqlQuery<FS.ZDWY.Internet.Models.FIN_IPR_INMAININFO>(sql, new List<SugarParameter>(){
                        new SugarParameter(":patientId",patientId),
                        new SugarParameter(":admissionNo",admissionNo),
                        new SugarParameter(":visitNo",visitNo),
                        new SugarParameter(":certifcateNo",certifcateNo),
                        new SugarParameter(":name",name)
            });
        }

        public int UpdatePrepayFee(string inpatientNO, decimal prepayCost)
        {
            string sql = @"UPDATE fin_ipr_inmaininfo  
                    SET prepay_cost = nvl(prepay_cost,0) + :prepayCost, 
                    free_cost = nvl(free_cost,0) + :prepayCost
                    WHERE inpatient_no = :inpatientNO";
            return Db.Ado.ExecuteCommand(sql, new List<SugarParameter>(){
                        new SugarParameter(":inpatientNO",inpatientNO),
                        new SugarParameter(":prepayCost",prepayCost)
            });
        }
        /// <summary>
        /// 根据门诊号获取最后一次住院的信息
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public FIN_IPR_INMAININFO GetINMAININFOByCardID(string cardID)
        {
            return Db.Queryable<FIN_IPR_INMAININFO>().OrderBy(q => q.IN_DATE, OrderByType.Desc).First(q => q.CARD_NO == cardID);
        }
    }
}
