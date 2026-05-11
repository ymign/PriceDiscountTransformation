using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using System.Data;

namespace FS.ZDWY.Internet.BL.OutPatient
{
    public class OrderLogic : SqlSugar.DbContext<FS.ZDWY.Internet.Models.COM_PATIENTINFO>
    {
        //  public System.Data.DataTable Xxx()
        //  {
        //      System.Data.DataTable dt = Db.Queryable<Models.COM_PATIENTINFO, Models.COM_PATIENTINFO>((st, sc) => new object[] {
        //  JoinType.Left,st.CARD_NO ==sc.CARD_NO})
        //.Select<FS.ZDWY.Internet.Models.COM_PATIENTINFO>().ToDataTable();
        //      return dt;

        //  }

        /*
          string parSequenceNo = nodesVales["parSequenceNo"];
            string parCardNo = nodesVales["parCardNo"];
            string parDoctcode = nodesVales["parDoctcode"];
            string parDeptcode = nodesVales["parDeptcode"];
            string parItemcode = nodesVales["parItemcode"];
            string parUnitPrice = nodesVales["parUnitPrice"];
            string parQty = nodesVales["parQty"];
            string parOwnCost = nodesVales["parOwnCost"];
            string parExecdeptcode = nodesVales["parExecdeptcode"];
            string parExecdeptname = nodesVales["parExecdeptname"];
            string parClinicCode = string.Empty;
            string parAppCode = string.Empty;
            string parErrMsg = string.Empty;
         */
        public DataTable AddNewOrder(string parSequenceNo, string parCardNo, string parDoctcode, string parDeptcode,
            string parItemcode, string parUnitPrice, string parQty, string parOwnCost,
            string parExecdeptcode, string parExecdeptname, ref string parAppCode, ref string parErrMsg)
        {
            List<SugarParameter> pars = new List<SugarParameter>()
            {
                new SugarParameter(":PAR_SEQUENCE_NO",parSequenceNo),
                new SugarParameter(":PAR_CARD_NO",parCardNo),
                new SugarParameter(":PAR_DOCTCODE",parDoctcode),
                new SugarParameter(":PAR_DEPTCODE",parDeptcode),
                new SugarParameter(":PAR_ITEMCODE",parItemcode),
                new SugarParameter(":PAR_UNIT_PRICE",parUnitPrice),
                new SugarParameter(":PAR_OWN_COST",parQty),
                new SugarParameter(":PAR_EXECDEPTCODE",parOwnCost),
                new SugarParameter(":PAR_EXECDEPTNAME",parExecdeptcode),
                new SugarParameter(":PAR_CLINIC_CODE",parExecdeptname),
                new SugarParameter(":PAR_APPCODE",parAppCode,true),
                new SugarParameter(":PAR_ERRMSG",parErrMsg,true)
            };

            var queryData = Db.Ado.UseStoredProcedure().GetDataTable("PRC_PE_INSERTFEEDETAIL", pars);
            parAppCode = pars[10].ToString();
            parErrMsg = pars[11].ToString();
           return queryData;
        }

        /// <summary>
        /// 获取就诊卡号
        /// </summary>
        /// <returns></returns>
        public string GetPatientCardNO()
        {
            string strSql = @" select SEQ_OPB_AUTOCARDNO.Nextval FROM DUAL ";
            return Db.Ado.GetScalar(strSql).ToString().PadLeft(10, '0');
        }

        /// <summary>
        /// 获取就诊卡号
        /// </summary>
        /// <returns></returns>
        public DateTime GetSysTime()
        {
            string strSql = @" select sysdate FROM DUAL ";
            return System.Convert.ToDateTime(Db.Ado.GetScalar(strSql).ToString());
        }

        public FS.ZDWY.Internet.Models.COM_PATIENTINFO GetPatientInfo(string idenNo)
        {
            string sql = @"select * from com_patientinfo a where a.card_no not like '9%'
and a.card_no not like '10%' 
and a.idenno = :idenno and rownum = 1 order BY  a.oper_date DESC,a.lreg_date DESC nulls last";
            List<FS.ZDWY.Internet.Models.COM_PATIENTINFO> queryData = Db.Ado.SqlQuery<FS.ZDWY.Internet.Models.COM_PATIENTINFO>(sql, new List<SugarParameter>(){
                            new SugarParameter(":idenNo", idenNo)     });

            string sqlin = @"Select Max(p.patient_no)
			From fin_ipr_inmaininfo p
			where p.idenno = :idenno ";
            List<string> inpatientno = Db.Ado.SqlQuery<string>(sqlin, new List<SugarParameter>(){
                            new SugarParameter(":idenNo", idenNo) });
            if (queryData != null && queryData.Count > 0)
            {
                FS.ZDWY.Internet.Models.COM_PATIENTINFO patientinfo = queryData[0];
                if (inpatientno != null && inpatientno.Count > 0)
                {
                    string no = inpatientno[0];
                    patientinfo.OLD_CARDNO = no;//临时存放住院号
                }
                else
                {
                    patientinfo.OLD_CARDNO = "";//临时存放住院号
                }
                return patientinfo;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 查询挂号排队
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="medicalNo"></param>
        /// <param name="certifcateNo"></param>
        /// <returns></returns>
        public DataTable QueryRegWaiting(string patientId, string medicalNo, string certifcateNo)
        {
            string sql = @"select a.triage_date as ""createTime"",--排队生成时间
       a.queue_code||a.clinic_code||a.see_sequence as ""id"",-- 院内排队ID
       a.queue_name as ""name"",--项目名称
       (select count(*) from met_nuo_assignrecord d where d.queue_code = a.queue_code)
       as ""totalNo"",--总排队人数
       a.see_sequence as ""sequenceNo"",--排队序号
       (select count(*) from met_nuo_assignrecord d where d.queue_code = a.queue_code 
       and d.see_sequence < a.see_sequence and d.assign_flag  in  ('1','5')) as ""remainNo"",--当前排队人数
        (select count(*)*10 from met_nuo_assignrecord d where d.queue_code = a.queue_code 
       and d.see_sequence < a.see_sequence and d.assign_flag  in  ('1','5')) as ""remainTime"",--大概剩余排队时间 
       a.dept_name as ""departmentName"",--科室名称
       fun_get_employee_name(a.doct_code) as ""doctorName"",--医生名称
        (select dd.remark from met_nuo_console dd where dd.console_code = a.console_code)  as ""address"",                   --排队的地址
       '' as remark,                      --备注
       (select e.levl_code from com_employee e where e.empl_code = a.doct_code)  as ""doctorTitle"",    --医生职称
       decode(a.assign_flag,'5','0','1') as ""status"" ,                   -- 排队状态  
        B.NAME AS ""patientName"" , 
        B.CARD_NO AS ""patientId""
 from  met_nuo_assignrecord a,fin_opr_register b,com_patientinfo c 
where a.reg_date >= trunc(sysdate)
 and  a.clinic_code = b.clinic_code
 and b.card_no = c.card_no
 and a.assign_flag in ('1','5')
 and b.card_no in (select * from　the (select cast(f_str2List(:card_no) as varchar2TableType) from　dual))
 and (c.case_no = :case_no or 'ALL' = :case_no)
 and (b.idenno = :certifcateNo or 'ALL' = :certifcateNo)";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                            new SugarParameter(":card_no", patientId),
                            new SugarParameter(":case_no", string.IsNullOrEmpty(medicalNo)?"ALL":medicalNo),
                            new SugarParameter(":certifcateNo",string.IsNullOrEmpty(certifcateNo)?"ALL":certifcateNo)
            });
            return queryData;
        }

        /// <summary>
        /// 查询取药排队
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="medicalNo"></param>
        /// <param name="certifcateNo"></param>
        /// <returns></returns>
        public DataTable QueryPhaWaiting(string patientId, string medicalNo, string certifcateNo)
        {
            string sql = @"select f.fee_date as ""createTime"",--排队生成时间
       f.recipe_no||f.druged_terminal as ""id"",-- 院内排队ID
       '取药排队' as ""name"",--项目名称
       (select count(*) from pha_sto_recipe d where d.fee_date >= trunc(sysdate) and d.druged_terminal = f.druged_terminal and d.recipe_state = '2')
       as ""totalNo"",--总排队人数
       (select count(*) from pha_sto_recipe d where d.fee_date >= trunc(sysdate) and d.druged_terminal = f.druged_terminal and d.fee_date < f.fee_date 
       and d.recipe_state = '2'） as ""sequenceNo"",--排队序号
       (select count(*) from pha_sto_recipe d where d.fee_date >= trunc(sysdate) and d.druged_terminal = f.druged_terminal and d.fee_date < f.fee_date 
       and d.recipe_state = '2'） as ""remainNo"",--当前排队人数
       (select count(*)*5 from pha_sto_recipe d where d.fee_date >= trunc(sysdate) and d.druged_terminal = f.druged_terminal and d.fee_date < f.fee_date 
       and d.recipe_state = '2'） as ""remainTime"",--大概剩余排队时间 
       fun_get_dept_name(f.drug_dept_code) as ""departmentName"",--科室名称
       fun_get_employee_name(f.doct_code) as ""doctorName"",--医生名称
       fun_get_dept_name(f.drug_dept_code)||(select ff.t_name from pha_sto_terminal ff where ff.t_code = f.druged_terminal)  as ""address"",                   --排队的地址
       '' as remark,                      --备注
       (select e.levl_code from com_employee e where e.empl_code = f.doct_code)  as ""doctorTitle"",    --医生职称
       '1' as ""status"",                    -- 排队状态  
        f.patient_name AS  ""patientName"" , 
        f.card_no AS ""patientId""
 from pha_sto_recipe f,com_patientinfo c
where f.card_no = c.card_no
 and f.fee_date >= trunc(sysdate)
 and f.recipe_state = '2'
 and f.card_no in (select * from　the (select cast(f_str2List(:card_no) as varchar2TableType) from　dual))
 and (c.case_no = :case_no or 'ALL' = :case_no)
 and (c.idenno = :certifcateNo or 'ALL' = :certifcateNo)";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                            new SugarParameter(":card_no", patientId),
                            new SugarParameter(":case_no", string.IsNullOrEmpty(medicalNo)?"ALL":medicalNo),
                            new SugarParameter(":certifcateNo",string.IsNullOrEmpty(certifcateNo)?"ALL":certifcateNo)
            });
            return queryData;
        }

        /// <summary>
        /// 查询对账信息
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isPay"></param>
        /// <returns></returns>
        public DataTable QueryFinanceBill(DateTime beginDate, DateTime endDate, string isPay)
        {
            string sql = @"select a.hospitalnum as ""hospitalNum"",  --医院订单号
      a.invoiceid as ""hospTradeId"",     --医院支付单号
       a.orderid as ""orderId"",          --业务系统订单号
       '1' as  ""isPay"",                 --是否支付
       a.paymode as ""payMode"",          --支付方式
       to_number(a.payamt) as ""payAmt"",            --支付金额
       a.tranno as ""transactionNo"",     --收单机构流水号
       a.paytime as ""payTime"",          --支付时间
       a.patientid as ""patientId"",      --院内用户ID
       a.certifcatetype as ""certifcateType"",--用户证件类型
       a.certifcateno as ""certifcateNo"",  --用户证件号码
       c.case_no as ""medicalNo"",          --病历号
       '' as ""cardType"",                    --用户卡类型
       a.patientid as ""cardNo"",           --用户卡号
       a.visitno as ""visitNo"",            --门诊号
       a.invoiceid as ""invoiceId"",        --发票号
       a.receiptid as ""receiptIds"",       --收据号
       c.name as ""name"",                  --就诊人姓名
       c.birthday as ""birthday"",          --就诊人出生日期
       c.sex_code as ""sex"",               --就诊人性别
       d.dept_name as ""departmentName"",  --科室名称 
       d.doct_name as ""doctorName"",       --医生名称 
       'mzjf' as ""bizType""
 from PLATFORM_BALANCE_PAY a,com_patientinfo c,fin_opr_register d
 where a.patientid = c.card_no
   and a.visitno = d.clinic_code
   and c.card_no = d.card_no
   and a.opertime >= :beginDate
   and a.opertime <  :endDate
   and '1'=:ispay
    union all
select 
  a.clinic_code as ""hospitalNum"",           --医院订单号
  r.invoice_no as ""hospTradeId"",           --医院支付单号
  a.orderid as ""orderId"",                  --业务系订单号
  decode(a.status,'2','1','2') as ""isPay"",            --是否支付
  a.paymethod as ""payMode"",                --支付方式
  to_number(a.regfee） as ""payAmt"",                    --支付金额
  (select b.transactionno from platform_register_pay b where b.orderid = a.orderid and rownum = 1) as ""transactionNo"",            --收单机构流水号
   (select b.paytime from platform_register_pay b where b.orderid = a.orderid and rownum = 1) as ""payTime"",            --支付时间
   r.card_no as ""patientId"",                                                                                         --院内用户ID
   a.certifcatetype as ""certifcateType"",                                                                               --用户证件类型
   a.certifcateno as ""certifcateNo"",                                                                                   --用户证件号码
   '' as ""medicalNo"",                                                                                                  --病历号
   '' as ""cardType"",                                                                                                   --用户卡类型
   '' as ""cardNo"",                                                                                                     --用户卡号
   r.clinic_code as ""visitNo"",                                                                                         --门诊号
   '' as ""invoiceId"",                                                                                                  --发票号
   '' as ""receiptIds"",                                                                                                 --收据号
   a.name as ""name"",                                                                                                   --就诊人姓名
   a.birth as ""birthday"",                                                                                              --就诊人出生日期 
   a.sex as ""sex"",                                                                                                     --就诊人性别 
   r.dept_name as ""departmentName"",                                                                                    --科室名称 
   r.doct_name as ""doctorName"",--医生名称
   'yygh' as ""bizType""
 from platform_register_order a,fin_opr_register r
where     a.registerid is not null 
 and  a.registerid = r.clinic_code
 and  a.ordertime >= :beginDate
 and  a.ordertime <  :endDate
 and decode(a.status,'2','1','2')=:ispay
  union all
select  a.hospchargeid as ""hospitalNum"",           --医院订单号
  a.hospchargeid as ""hospTradeId"",           --医院支付单号
  a.chargeid as ""orderId"",                  --业务系订单号
  '1' as ""isPay"",            --是否支付
  a.chargetype as ""payMode"",                --支付方式
  to_number(a.amount*100) as ""payAmt"",                    --支付金额
  a.transactionno as ""transactionNo"",            --收单机构流水号
  a.chargetime as ""payTime"",            --支付时间
   r.card_no as ""patientId"",                                                                                         --院内用户ID
   r.idcardtype as ""certifcateType"",                                                                               --用户证件类型
   r.idenno as ""certifcateNo"",                                                                                   --用户证件号码
   '' as ""medicalNo"",                                                                                                  --病历号
   '' as ""cardType"",                                                                                                   --用户卡类型
   '' as ""cardNo"",                                                                                                     --用户卡号
   r.inpatient_no as ""visitNo"",                                                                                         --门诊号
   '' as ""invoiceId"",                                                                                                  --发票号
   '' as ""receiptIds"",                                                                                                 --收据号
   r.name as ""name"",                                                                                                   --就诊人姓名
   r.birthday as ""birthday"",                                                                                              --就诊人出生日期 
   r.sex_code as ""sex"",                                                                                                     --就诊人性别 
   r.dept_name as ""departmentName"",                                                                                    --科室名称 
   r.house_doc_name as ""doctorName"",
   'zyyjj' as ""bizType""
 from platform_inprepay_pay a,fin_ipr_inmaininfo r
where a.inpatient_no = r.inpatient_no
 and  a.oper_time >= :beginDate
 and  a.oper_time <  :endDate
 and  '1'=:ispay
";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                            new SugarParameter(":beginDate", beginDate),
                            new SugarParameter(":endDate", endDate),
                            new SugarParameter(":ispay", isPay)
            });
            return queryData;
        }

        #region 消息通知相关

        /// <summary>
        /// 门诊缴费通知
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="createTime"></param>
        /// <param name="execTime"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        public int UpdateOutPatientFeeMsg(string clinicCode, DateTime createTime, DateTime execTime, string returnValue)
        {
            string sql = @"update PLATFORM_OUTPATIENTFEE_MESSAGE a set a.state = '1',a.exec_time = :execTime,a.message = :returnValue
where a.clinic_code = :clinicCode and a.create_time = :createTime";
            var queryData = Db.Ado.GetScalar(sql, new List<SugarParameter>(){
                                new SugarParameter(":clinic_code",clinicCode),
                                new SugarParameter(":createTime",createTime),
                                new SugarParameter(":execTime",execTime),
                                new SugarParameter(":returnValue",returnValue),
            });
            return Convert.ToInt16(queryData.ToString());
        }

        /// <summary>
        /// 查询门诊缴费记录
        /// </summary>
        /// <returns></returns>
        public DataTable QueryOutpatientFeeMsgList()
        {
            string sql = @"select b.card_no as ""patientId"",--就诊人院内ID 
       b.clinic_code as ""outpatId "",
       '' as ""cardType"",        --就诊卡类型
       b.card_no as ""cardNo"",   --就诊人卡号码
       '' as ""certifcateType"",  --就诊人证件类型
       b.idenno as ""certifcateNo"",--就诊人证件号码 
       b.name as ""name"",        --就诊人姓名 
       to_char(b.birthday,'yyyy-MM-dd') as ""birthday"",--就诊人出生日期
       decode(b.sex_code,'M','1','2') as ""sex"",     --就诊人性别 
       b.dept_name as ""departmentName"",--科室名称 
       b.doct_name as ""doctorName"", --医生名称 
       (select sum(f.own_cost * 100) from fin_opb_feedetail f where f.clinic_code = b.clinic_code
       and f.pay_flag = '0') as ""amount"", --待缴费金额 
       '' as ""billId"",                    --医院缴费id
       '' as ""content"",                    --提醒内容
       b.reg_date as ""outpatTime""
 from platform_outpatientfee_message a,
              fin_opr_register b,
              com_patientinfo c
           where a.clinic_code = b.clinic_code
             and b.card_no = c.card_no
             and a.state = '0'
             and a.create_time >= trunc(sysdate)";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>());
            return queryData;
        }
        /// <summary>
        /// 查询门诊挂号排队通知
        /// </summary>
        /// <returns></returns>
        public DataTable QueryRegWaitingALL()
        {
            string sql = @"select 
       b.card_no as ""patientId"",--就诊人院内ID 
       c.case_no as ""medicalNo"",--病历号
       '' as ""cardType"",        --就诊人卡类型 
       b.card_no as ""cardNo"",   --就诊人卡号码
       '' as ""certifcateType"",  --就诊人证件类型 
       b.idenno as ""certifcateNo"",--就诊人证件号码 
       b.name as ""name"",        --就诊人姓名
       b.birthday as ""birthday"",--就诊人出生日期 
       b.sex_code as ""sex"",     --就诊人性别 
       b.dept_name as ""departmentName"",--科室名称
       b.doct_name as ""doctorName"",    --医生名称 
       '1' as ""lineType"",              --推送类型
       '1' as ""排队类别"",              --排队类别 
       a.see_sequence as ""sequenceNo"",--排队序号
        (select count(*) from met_nuo_assignrecord d where d.queue_code = a.queue_code 
       and d.see_sequence < a.see_sequence and d.assign_flag = '1') as ""remainNo"",--当前排队人数
       a.queue_code||a.clinic_code||a.see_sequence as ""lineId"",-- 院内排队ID
       a.triage_date as ""lineTime"",--排队生成时间
       '' as ""note"",               --就医准备
       '' as ""content""            --提醒内容
 from  met_nuo_assignrecord a,fin_opr_register b,com_patientinfo c 
where a.reg_date >= trunc(sysdate)
 and  a.clinic_code = b.clinic_code
 and b.card_no = c.card_no
 and a.assign_flag = '1'";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>());
            return queryData;
        }

        /// <summary>
        /// 查询门诊取药排队通知
        /// </summary>
        /// <returns></returns>
        public DataTable QueryPhaWaitingALL()
        {
            string sql = @"select  c.card_no as ""patientId"",--就诊人院内ID 
       c.case_no as ""medicalNo"",--病历号
       '' as ""cardType"",        --就诊人卡类型 
       c.card_no as ""cardNo"",   --就诊人卡号码
       '' as ""certifcateType"",  --就诊人证件类型 
       c.idenno as ""certifcateNo"",--就诊人证件号码 
       c.name as ""name"",        --就诊人姓名
       c.birthday as ""birthday"",--就诊人出生日期 
       c.sex_code as ""sex"",     --就诊人性别 
       fun_get_dept_name(f.dept_code) as ""departmentName"",--科室名称
       fun_get_employee_name(f.doct_code) as ""doctorName"",    --医生名称 
       '1' as ""lineType"",              --推送类型
       '2' as ""排队类别"",              --排队类别 
       (select count(*) from pha_sto_recipe d where d.fee_date >= trunc(sysdate) and d.druged_terminal = f.druged_terminal and d.fee_date < f.fee_date 
       and d.recipe_state = '2'） as ""sequenceNo"",--排队序号
       (select count(*) from pha_sto_recipe d where d.fee_date >= trunc(sysdate) and d.druged_terminal = f.druged_terminal and d.fee_date < f.fee_date 
       and d.recipe_state = '2'） as ""remainNo"",--当前排队人数
       f.recipe_no||f.druged_terminal as ""lineId"",-- 院内排队ID
       f.fee_date as ""lineTime"",--排队生成时间
       '' as ""note"",               --就医准备
       '' as ""content""            --提醒内容
 from pha_sto_recipe f,com_patientinfo c
where f.card_no = c.card_no
 and f.fee_date >= trunc(sysdate)
 and f.recipe_state = '2'";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>());
            return queryData;
        }

        /// <summary>
        /// 查询取消挂号记录
        /// </summary>
        /// <returns></returns>
        public DataTable QueryQueryCancelRegList()
        {
            string sql = @"select 
       aa.clinic_code as ""CLINIC_CODE"",
       aa.create_time as ""CREATE_TIME"",
       BB.ORDERID AS ""orderId"",--业务系统订单号
       BB.CLINIC_CODE AS ""hospOrderId"",--医院订单号 
       AA.CREATE_TIME AS ""cancelTime"",--取消时间 
       TO_CHAR(AA.CREATE_TIME,'HH24:MI') as ""reason"",--取消原因 
       '0' as ""isStop""                               --是否停诊
 from PLATFORM_OUTPATIENT_REGCANCEL aa,platform_register_order  BB
WHERE AA.CLINIC_CODE = BB.REGISTERID
 AND AA.STATE = '0'
 AND AA.CREATE_TIME >= TRUNC(SYSDATE)";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>());
            return queryData;
        }

        /// <summary>
        /// 查询取消挂号接诊记录
        /// </summary>
        /// <returns></returns>
        public DataTable QueryQueryRegAcceptList()
        {
            string sql = @" SELECT AA.CLINIC_CODE AS ""CLINIC_CODE"",
        AA.CREATE_TIME AS ""CREATE_TIME"",
        BB.ORDERID AS ""orderId"",    --业务系统订单号
        BB.CLINIC_CODE AS ""hospOrderId"",--医院订单号 
        AA.CREATE_TIME AS ""getTime""     --接诊时间 
  FROM PLATFORM_OUTPATIENT_REGCACCEPT AA,platform_register_order bb
 where aa.clinic_code = bb.registerid
 and aa.state = '0' and aa.create_time >= trunc(sysdate)";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>());
            return queryData;
        }

        #endregion

        public DataTable QueryInpatientNo(string name, string inpatID,string idNO)
        {
            string sql = @"SELECT 
       '' ""cardType"",
       '' ""cardNo"",  
       a.card_no ""patientId"",
       a.patient_no ""inpatId""
 FROM fin_ipr_inmaininfo a 
WHERE a.name = :name
 AND  a.patient_no = :patientNO
 AND  (A.IDENNO = :IDENNO OR 'ALL' =  :IDENNO)
 AND  ROWNUM = 1";

            if (string.IsNullOrEmpty(idNO))
            {
                idNO = "ALL";
            }

            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                            new SugarParameter(":name", name),
                            new SugarParameter(":patientNO", inpatID),
                             new SugarParameter(":IDENNO", idNO)
            });
            return queryData;
        }
    }
}
