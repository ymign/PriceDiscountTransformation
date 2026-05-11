using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Data;

namespace FS.ZDWY.Internet.BL.OutPatient
{
    public class PlatformOrderLogic : SqlSugar.DbContext<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER>
    {

        /// <summary>
        /// 查询听诊信息
        /// </summary>
        /// <returns></returns>
        public List<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER> QueryList()
        {
             string sql = @"select regord.*
from fin_opr_register reg
inner join fin_opr_schema sch on sch.id=reg.schema_no and trunc(reg.reg_date)>trunc(sysdate) and sch.valid_flag='0'
inner join platform_register_order regord on regord.registerid=reg.clinic_code and regord.status='2'
where reg.valid_flag='1'
and not exists(
select 1 from platform_register_order o where o.registerid=reg.clinic_code and o.status<>'8'
)";
             List<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER> queryData = Db.Ado.SqlQuery<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER>(sql, new List<SugarParameter>() { });
            if(queryData==null || queryData.Count<=0)
            {
                return new List<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_ORDER>();
            }
            else
            {
                return queryData;
            }
        }

        #region 其他

        /// <summary>
        /// 2.1.4.8.预约就诊提醒
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="medicalNo"></param>
        /// <param name="certifcateNo"></param>
        /// <returns></returns>
        public DataTable BookRemind()
        {
            string sql = @"select  
reg.invoice_no ""hospTradeId"",
reg.clinic_code ""visitNo"",
to_char(reg.reg_date,'yyyy-mm-dd') ""visitTime"",
ord.begintime ""beginTime"",
ord.endtime ""endTime"",
regexp_replace(reg.dept_name,'[＆&]','、') ""deptName"",
nvl(reg.doct_name,'当班医生') ""doctorName"",
regexp_replace(reg.dept_name,'[＆&]','、')||cord.queue_name ""address"",
ord.orderid ""orderId"",
ord.clinic_code ""hospitalNum"",
cord.see_sequence ""proof""
from fin_opr_register reg
inner join platform_register_order ord on ord.registerid=reg.clinic_code and ord.regtype='0'
inner join met_nuo_assignrecord  cord on cord.clinic_code=reg.clinic_code 
where reg.valid_flag='1'
and reg.ynsee='0'
and trunc(reg.reg_date)>=trunc(sysdate)
and trunc(reg.reg_date)<trunc(sysdate)+1";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>() { });
            return queryData;
        }

        /// <summary>
        /// 2.1.4.9.停诊通知
        /// </summary>
        /// <returns></returns>
        public DataTable StopSchedulRemind()
        {
            string sql = @"--线上支付
select 
reg.invoice_no  ""hospTradeId"",
reg.clinic_code ""visitNo"", 
to_char(reg.reg_date,'yyyy-mm-dd')  ""visitTime"",
regord.begintime ""beginTime"",
regord.endtime  ""endTime"",
regexp_replace(reg.dept_name,'[＆&]','、') ""deptName"",
reg.doct_name ""doctorName"",
regord.orderid ""orderId"",
regord.clinic_code ""hospitalNum"",
regord.scheduleid ""scheduleId"",
'' ""reason"",
regord.numberinfoid ""timeId"",
'1' ""ispay""
from fin_opr_register reg
inner join fin_opr_schema sch on sch.id=reg.schema_no and trunc(reg.reg_date)>=trunc(sysdate) and sch.valid_flag='0'
inner join platform_register_order regord on regord.registerid=reg.clinic_code and regord.status='2'
where reg.valid_flag='1' and  trunc(reg.reg_date) >= trunc(sysdate)

union all

--线下支付
select 
''  ""hospTradeId"",
'' ""visitNo"", 
to_char(regord.scheduledate ,'yyyy-mm-dd')  ""visitTime"",
regord.begintime ""beginTime"",
regord.endtime  ""endTime"",
regexp_replace(fun_get_dept_name(regord.deptcode),'[＆&]','、') ""deptName"",
fun_get_employee_name( regord.doctorcode) ""doctorName"",
regord.orderid ""orderId"",
regord.clinic_code ""hospitalNum"",
regord.scheduleid ""scheduleId"",
'' ""reason"",
regord.numberinfoid ""timeId"",
'0' ""ispay""
from fin_opr_booking book
inner join fin_opr_schema sch on sch.id=book.schema_no and trunc(sch.see_date)>=trunc(sysdate) and sch.valid_flag='0'
inner join platform_register_order regord on regord.clinic_code=book.clinic_code and regord.status='1' and regord.paymethod='0'
where book.valid_flag='1'
";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>() { });
            return queryData;
        }

        #endregion
    }
}
