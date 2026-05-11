using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BL.OutPatient
{
    public class SchedulingLogic : SqlSugar.DbContext<FS.ZDWY.Internet.Models.FIN_OPR_SCHEMA>
    {

        public System.Data.DataTable QueryDoctList(DateTime beginDate, DateTime endDate, string deptCode)
        {
            string sql = @"select  b.deptCode ""deptCode"",b.deptName  ""deptName"",b.doctorCode ""doctorCode"",b.doctorName ""doctorName"",
            b.doctorTitle ""doctorTitle"",b.doctorIntrodution ""doctorIntrodution"",b.workStatus ""workStatus"",b.regFee ""regFee"",
            sum(b.totalNum) ""totalNum"",sum(b.leftNum) ""leftNum"",b.level1 ""level"",trunc(b.scheduleTime) ""scheduleTime""  ,sum(b.leftappNum) ""leftappNum"" ,sum(b.totalappNum) ""totalappNum"" 
,nvl((select  emp.sort_id from com_employee  emp where  emp.empl_code=b.doctorCode),1000) ""sequenceNo""
,b.amTotalNum ""amTotalNum""
,b.amAppNum ""amAppNum""
,b.pmTotalNum ""pmTotalNum""
,b.pmAppNum ""pmAppNum""
from 
(
select t.dept_code deptCode     --科室代码    必填
            ,regexp_replace(t.dept_name,'[＆&]','、') deptName--科室名称 必填
            , t.doct_code doctorCode--医生代码 必填
            , nvl(t.doct_name,t.dept_name) doctorName--医生名称 必填
            ,'' doctorTitle--医生职称
            ,'' doctorIntrodution--医生介绍
            ,t.valid_flag workStatus--出诊状态  0 - 停诊 1 - 正常 2 - 未开放 必填
            ,round(f.diag_fee*100) regFee--挂号费 单位分 必填
            ,t.reg_lmt totalNum--号源总数 必填  --给平台的号源数量
            ,t.reg_lmt - t.reged leftNum--剩余号源数 必填
            ,decode(t.reglevl_code,'5','3','1','1','2') level1--号源类别  1 - 普通 2 - 专家 3 - 特需  必填
            ,t.reglevl_code
            ,t.see_date scheduleTime  --排班日期
            ,t.tel_lmt-t.tel_reging  leftappNum --剩余预约号源
            ,t.tel_lmt              totalappNum--预约号源总数
            ,t.NOON_CODE --午别
            ,(select nvl(decode(trunc(t.see_date),trunc(sysdate),sum(b.reg_lmt+b.tel_reged),sum(b.tel_lmt)),0) from FIN_OPR_SCHEMA b where b.see_date = t.see_date and b.dept_code = t.dept_code and b.doct_code = t.doct_code and  b.NOON_CODE = '1' and valid_flag = '1' and t.reglevl_code =b.reglevl_code and (b.dept_code=:deptCode or instr(b.schema_dept_code,:deptCode)> 0)) as amTotalNum
            ,(select nvl(decode(trunc(t.see_date),trunc(sysdate),sum(b.reged+b.tel_reged +(case when b.end_time<sysdate then (b.reg_lmt - b.reged) else 0 end  )),sum(b.tel_reged)),0)from FIN_OPR_SCHEMA b where b.see_date = t.see_date and b.dept_code = t.dept_code and b.doct_code = t.doct_code and b.NOON_CODE = '1'  and valid_flag = '1' and t.reglevl_code =b.reglevl_code  and (b.dept_code=:deptCode or instr(b.schema_dept_code,:deptCode)> 0)) as amAppNum
            ,(select nvl(decode(trunc(t.see_date),trunc(sysdate),sum(b.reg_lmt+b.tel_reged),sum(b.tel_lmt)),0) from FIN_OPR_SCHEMA b where b.see_date = t.see_date and b.dept_code = t.dept_code and b.doct_code = t.doct_code and  b.NOON_CODE = '2'  and valid_flag = '1' and t.reglevl_code =b.reglevl_code  and (b.dept_code=:deptCode or instr(b.schema_dept_code,:deptCode)> 0)) as pmTotalNum
            ,(select nvl(decode(trunc(t.see_date),trunc(sysdate),sum(b.reged+b.tel_reged+(case when b.end_time<sysdate then (b.reg_lmt - b.reged) else 0 end  )),sum(b.tel_reged)),0)from FIN_OPR_SCHEMA b where b.see_date = t.see_date and b.dept_code = t.dept_code and b.doct_code = t.doct_code and b.NOON_CODE = '2'  and valid_flag = '1' and t.reglevl_code =b.reglevl_code  and (b.dept_code=:deptCode or instr(b.schema_dept_code,:deptCode)> 0)) as pmAppNum
            from FIN_OPR_SCHEMA t JOIN fin_opr_regfeeonpact f ON t.reglevl_code = f.reglevl_code
            WHERE f.pact_code='1'  and t.stop<>'1' and  t.room_id is not null  and t.valid_flag='1' and t.end_time > sysdate and (t.dept_code=:deptCode or instr(t.schema_dept_code,:deptCode)> 0)
            and t.see_date between :beginDate
            and :endDate 
            ) b group by b.deptCode,b.deptName,b.doctorCode,b.doctorName,
            b.doctorTitle,b.doctorIntrodution,b.workStatus,b.regFee,b.amTotalNum,b.amAppNum,b.pmTotalNum,b.pmAppNum,
            b.level1,trunc(b.scheduleTime),reglevl_code
            ORDER BY b.deptCode,b.doctorCode ";
            var dt = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                          new SugarParameter(":deptCode",deptCode),
                          new SugarParameter(":beginDate",beginDate),
                          new SugarParameter(":endDate",endDate),
                        });
            return dt;
        }

        public System.Data.DataTable QueryZZQDoctList(DateTime beginDate, DateTime endDate, string deptCode)
        {
            string sql = @"select  b.deptCode ""deptCode"",b.deptName  ""deptName"",b.doctorCode ""doctorCode"",b.doctorName ""doctorName"",
            b.doctorTitle ""doctorTitle"",b.doctorIntrodution ""doctorIntrodution"",b.workStatus ""workStatus"",b.regFee ""regFee"",
            sum(b.totalNum) ""totalNum"",sum(b.leftNum) ""leftNum"",b.level1 ""level"",trunc(b.scheduleTime) ""scheduleTime""  ,sum(b.leftappNum) ""leftappNum"" ,sum(b.totalappNum) ""totalappNum"" 
,nvl((select  emp.sort_id from com_employee  emp where  emp.empl_code=b.doctorCode),1000) ""sequenceNo""
,b.amTotalNum ""amTotalNum""
,b.amAppNum ""amAppNum""
,b.pmTotalNum ""pmTotalNum""
,b.pmAppNum ""pmAppNum""
from 
(
select t.dept_code deptCode     --科室代码    必填
            ,regexp_replace(t.dept_name,'[＆&]','、') deptName--科室名称 必填
            , t.doct_code doctorCode--医生代码 必填
            , nvl(t.doct_name,t.dept_name) doctorName--医生名称 必填
            ,'' doctorTitle--医生职称
            ,'' doctorIntrodution--医生介绍
            ,t.valid_flag workStatus--出诊状态  0 - 停诊 1 - 正常 2 - 未开放 必填
            ,round(f.diag_fee*100) regFee--挂号费 单位分 必填
            ,t.reg_lmt totalNum--号源总数 必填  --给平台的号源数量
            ,t.reg_lmt - t.reged leftNum--剩余号源数 必填
            ,decode(t.reglevl_code,'5','3','1','1','2') level1--号源类别  1 - 普通 2 - 专家 3 - 特需  必填
            ,t.reglevl_code
            ,t.see_date scheduleTime  --排班日期
            ,t.tel_lmt-t.tel_reging  leftappNum --剩余预约号源
            ,t.tel_lmt              totalappNum--预约号源总数
            ,t.NOON_CODE --午别
            ,(select nvl(decode(trunc(t.see_date),trunc(sysdate),sum(b.reg_lmt+b.tel_reged),sum(b.tel_lmt)),0) from FIN_OPR_SCHEMA b where b.see_date = t.see_date and b.dept_code = t.dept_code and b.doct_code = t.doct_code and  b.NOON_CODE = '1' and valid_flag = '1' and t.reglevl_code =b.reglevl_code and (b.dept_code=:deptCode or instr(b.schema_dept_code,:deptCode)> 0)) as amTotalNum
            ,(select nvl(decode(trunc(t.see_date),trunc(sysdate),sum(b.reged+b.tel_reged +(case when b.end_time<sysdate then (b.reg_lmt - b.reged) else 0 end  )),sum(b.tel_reged)),0)from FIN_OPR_SCHEMA b where b.see_date = t.see_date and b.dept_code = t.dept_code and b.doct_code = t.doct_code and b.NOON_CODE = '1'  and valid_flag = '1' and t.reglevl_code =b.reglevl_code  and (b.dept_code=:deptCode or instr(b.schema_dept_code,:deptCode)> 0)) as amAppNum
            ,(select nvl(decode(trunc(t.see_date),trunc(sysdate),sum(b.reg_lmt+b.tel_reged),sum(b.tel_lmt)),0) from FIN_OPR_SCHEMA b where b.see_date = t.see_date and b.dept_code = t.dept_code and b.doct_code = t.doct_code and  b.NOON_CODE = '2'  and valid_flag = '1' and t.reglevl_code =b.reglevl_code  and (b.dept_code=:deptCode or instr(b.schema_dept_code,:deptCode)> 0)) as pmTotalNum
            ,(select nvl(decode(trunc(t.see_date),trunc(sysdate),sum(b.reged+b.tel_reged+(case when b.end_time<sysdate then (b.reg_lmt - b.reged) else 0 end  )),sum(b.tel_reged)),0)from FIN_OPR_SCHEMA b where b.see_date = t.see_date and b.dept_code = t.dept_code and b.doct_code = t.doct_code and b.NOON_CODE = '2'  and valid_flag = '1' and t.reglevl_code =b.reglevl_code  and (b.dept_code=:deptCode or instr(b.schema_dept_code,:deptCode)> 0)) as pmAppNum
            from FIN_OPR_SCHEMA t JOIN fin_opr_regfeeonpact f ON t.reglevl_code = f.reglevl_code
            WHERE f.pact_code='1'  and t.stop<>'1' and  t.room_id is not null  and t.valid_flag='1' and t.end_time > sysdate and (t.dept_code=:deptCode or instr(t.schema_dept_code,:deptCode)> 0)
            and t.see_date between :beginDate
            and :endDate 
            AND (t.doct_code IN (SELECT d.code FROM com_dictionary d WHERE d.type = 'ELDERLYVOUCHERDOCTOR') or t.doct_code = 'None')
            and t.reglevl_code in ('1','2','3','4','10')
            ) b group by b.deptCode,b.deptName,b.doctorCode,b.doctorName,
            b.doctorTitle,b.doctorIntrodution,b.workStatus,b.regFee,b.amTotalNum,b.amAppNum,b.pmTotalNum,b.pmAppNum,
            b.level1,trunc(b.scheduleTime),reglevl_code
            ORDER BY b.deptCode,b.doctorCode ";
            var dt = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                          new SugarParameter(":deptCode",deptCode),
                          new SugarParameter(":beginDate",beginDate),
                          new SugarParameter(":endDate",endDate),
                        });
            return dt;
        }

        public System.Data.DataTable QuerySchedule(DateTime beginDate, DateTime endDate, string deptCode, string doctorCode)
        {
            string sql = @"select 
t.dept_code ""deptCode""      --科室代码    必填
,regexp_replace(t.dept_name,'[＆&]','、') ""deptName""--科室名称 必填
, t.doct_code ""doctorCode""--医生代码 必填
, nvl(t.doct_name,t.dept_name)""doctorName""--医生名称 必填
,'' ""doctorTitle""--医生职称
,'' ""doctorIntrodution""--医生介绍
,t.see_date ""scheduleDate""--号源日期 必填
, t.noon_code ""timeFlag""--时段 必填
,decode(t.Schema_Type,'0','0','1') ""hasDetailTime""--是否有分时 必填 0 - 否 1 - 是
,t.begin_time ""beginTime""--开始时间 必填
, t.end_time ""endTime""--结束时间 必填
, t.valid_flag ""workStatus""--出诊状态 必填
,round(f.diag_fee*100) ""regFee""--挂号费 单位分 必填
--, t.spe_lmt ""totalNum""--号源总数 必填
--, t.spe_lmt - t.spe_reged ""leftNum""--剩余号源数 必填
,t.reg_lmt ""totalNum""--号源总数 必填  --给平台的号源数量
,t.reg_lmt - t.reged ""leftNum""--剩余号源数 必填
, t.id ""scheduleId""--班次Id 必填
, t.reglevl_code ""level""--号源类别 必填
,'' ""sequenceNo""--排序
,t.tel_lmt-t.tel_reging  ""leftappNum"" --剩余预约号源
,t.tel_lmt              ""totalappNum""--预约号源总数
from FIN_OPR_SCHEMA t JOIN fin_opr_regfeeonpact f ON t.reglevl_code = f.reglevl_code
WHERE f.pact_code='1'  and t.stop<>'1' and t.valid_flag='1' and  t.room_id is not null  and t.end_time > sysdate  and (t.dept_code=:deptCode or instr(t.schema_dept_code,:deptCode)> 0)
and (t.doct_code=:doctorCode or 'ALL' =:doctorCode)
and t.see_date between :beginDate and :endDate  order by t.begin_time";
            var dt = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
              new SugarParameter(":deptCode",deptCode),
              new SugarParameter(":doctorCode",string.IsNullOrEmpty(doctorCode)?"ALL":doctorCode),
              new SugarParameter(":beginDate",beginDate),
              new SugarParameter(":endDate",endDate)
            });
            return dt;
        }


        public System.Data.DataTable QueryZZQSchedule(DateTime beginDate, DateTime endDate, string deptCode, string doctorCode)
        {
            string sql = @"select 
t.dept_code ""deptCode""      --科室代码    必填
,regexp_replace(t.dept_name,'[＆&]','、') ""deptName""--科室名称 必填
, t.doct_code ""doctorCode""--医生代码 必填
, nvl(t.doct_name,t.dept_name)""doctorName""--医生名称 必填
,'' ""doctorTitle""--医生职称
,'' ""doctorIntrodution""--医生介绍
,t.see_date ""scheduleDate""--号源日期 必填
, t.noon_code ""timeFlag""--时段 必填
,decode(t.Schema_Type,'0','0','1') ""hasDetailTime""--是否有分时 必填 0 - 否 1 - 是
,t.begin_time ""beginTime""--开始时间 必填
, t.end_time ""endTime""--结束时间 必填
, t.valid_flag ""workStatus""--出诊状态 必填
,round(f.diag_fee*100) ""regFee""--挂号费 单位分 必填
--, t.spe_lmt ""totalNum""--号源总数 必填
--, t.spe_lmt - t.spe_reged ""leftNum""--剩余号源数 必填
,t.reg_lmt ""totalNum""--号源总数 必填  --给平台的号源数量
,t.reg_lmt - t.reged ""leftNum""--剩余号源数 必填
, t.id ""scheduleId""--班次Id 必填
, t.reglevl_code ""level""--号源类别 必填
,'' ""sequenceNo""--排序
,t.tel_lmt-t.tel_reging  ""leftappNum"" --剩余预约号源
,t.tel_lmt              ""totalappNum""--预约号源总数
from FIN_OPR_SCHEMA t JOIN fin_opr_regfeeonpact f ON t.reglevl_code = f.reglevl_code
WHERE f.pact_code='1'  and t.stop<>'1' and t.valid_flag='1' and  t.room_id is not null  and t.end_time > sysdate  and (t.dept_code=:deptCode or instr(t.schema_dept_code,:deptCode)> 0)
and t.reglevl_code in ('1','2','3','4','10')
AND (t.doct_code IN (SELECT d.code FROM com_dictionary d WHERE d.type = 'ELDERLYVOUCHERDOCTOR') or t.doct_code = 'None')
and (t.doct_code=:doctorCode or 'ALL' =:doctorCode)
and t.see_date between :beginDate and :endDate  order by t.begin_time";
            var dt = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
              new SugarParameter(":deptCode",deptCode),
              new SugarParameter(":doctorCode",string.IsNullOrEmpty(doctorCode)?"ALL":doctorCode),
              new SugarParameter(":beginDate",beginDate),
              new SugarParameter(":endDate",endDate)
            });
            return dt;
        }

        /// <summary>
        /// 查询医生当天的排班信息
        /// </summary>
        /// <param name="schemaType">排班类型;0:科室,1:医生</param>
        /// <param name="addRegTime">加号时间</param>
        /// <param name="noonId">午别;1:上午,2:下午</param>
        /// <param name="deptCode">科室编码</param>
        /// <param name="doctorCode">医生编码</param>
        /// <returns></returns>
        public System.Data.DataTable QuerySchedule(string schemaType, DateTime addRegTime, string noonId, string deptCode, string doctorCode)
        {
            string sql = @"select t.dept_code ""deptCode"",
                                   regexp_replace(t.dept_name, '[＆&]', '、') ""deptName"",
                                   t.doct_code ""doctorCode"",
                                   nvl(t.doct_name, t.dept_name) ""doctorName"",
                                   '' ""doctorTitle"",
                                   '' ""doctorIntrodution"",
                                   t.see_date ""scheduleDate"",
                                   t.noon_code ""timeFlag"",
                                   decode(t.Schema_Type, '0', '0', '1') ""hasDetailTime"",
                                   t.begin_time ""beginTime"",
                                   t.end_time ""endTime"",
                                   t.valid_flag ""workStatus"",
                                   round(f.diag_fee * 100) ""regFee"",
                                   t.reg_lmt ""totalNum"",
                                   t.reg_lmt - t.reged ""leftNum"",
                                   t.id ""scheduleId"",
                                   t.reglevl_code ""level"",
                                   '' ""sequenceNo"",
                                   t.tel_lmt - t.tel_reging ""leftappNum"",
                                   t.tel_lmt ""totalappNum""
                              from FIN_OPR_SCHEMA t
                              JOIN FIN_OPR_REGFEEONPACT f ON t.reglevl_code = f.reglevl_code
                             WHERE f.pact_code = '1'
                               and t.stop <> '1'
                               and t.valid_flag = '1'
                               and t.room_id is not null
                               and t.see_date = to_date(:addRegTime, 'yyyy-MM-dd')
                               and (t.dept_code = :deptCode or instr(t.schema_dept_code, :deptCode) > 0)
                               and t.noon_code = :noonCode
                               and t.schema_type = :schemaType ";
            sql += schemaType == "1" ? "and (t.doct_code = :doctorCode or 'ALL' = :doctorCode)" : "";
            sql += " order by t.begin_time";
            var dt = Db.Ado.GetDataTable(sql, new List<SugarParameter>() {
                new SugarParameter(":addRegTime", addRegTime.ToString("yyyy-MM-dd")),
                new SugarParameter(":deptCode", deptCode),
                new SugarParameter(":doctorCode", string.IsNullOrEmpty(doctorCode) ? "ALL" : doctorCode),
                new SugarParameter(":noonCode", noonId),
                new SugarParameter(":schemaType", schemaType)
            });
            return dt;
        }

        public System.Data.DataTable QueryScheduleTime(DateTime dtscheduleDate, string scheduleId, string deptCode, string doctorCode)
        {
            string sql = @"select 
t.dept_code ""deptCode""      --科室代码    必填
,regexp_replace(t.dept_name,'[＆&]','、') ""deptName""--科室名称 必填
, t.doct_code ""doctorCode""--医生代码 必填
, nvl(t.doct_name,t.dept_name) ""doctorName""--医生名称 必填
,'' ""doctorTitle""--医生职称
,'' ""doctorIntrodution""--医生介绍
,t.see_date ""scheduleDate""--号源日期 必填
, t.noon_code ""timeFlag""--时段 必填
,decode(t.Schema_Type,'0','0','1') ""hasDetailTime""--是否有分时 必填 0 - 否 1 - 是
,t.begin_time ""beginTime""--开始时间 必填
, t.end_time ""endTime""--结束时间 必填
, t.valid_flag ""workStatus""--出诊状态 必填
,round(f.diag_fee*100) ""regFee""--挂号费 单位分 必填
--, t.spe_lmt ""totalNum""--号源总数 必填
--, t.spe_lmt - t.spe_reged ""leftNum""--剩余号源数 必填
,t.reg_lmt ""totalNum""--号源总数 必填  --给平台的号源数量
,t.reg_lmt - t.reged ""leftNum""--剩余号源数 必填
, t.id ""scheduleId""--班次Id 必填
, t.reglevl_code ""level""--号源类别 必填
,'' ""sequenceNo""--排序
,t.tel_lmt-t.tel_reging  ""leftappNum"" --剩余预约号源
,t.tel_lmt              ""totalappNum""--预约号源总数
from FIN_OPR_SCHEMA t JOIN fin_opr_regfeeonpact f ON t.reglevl_code = f.reglevl_code
WHERE f.pact_code='1'  and t.stop<>'1' and t.valid_flag='1' and  t.room_id is not null  and t.end_time > sysdate  and t.dept_code=:deptCode and t.doct_code=:doctorCode
and t.see_date between :beginDate and :endDate
and t.id = :scheduleId order by t.begin_time";
            DateTime beginDate = dtscheduleDate.Date;
            DateTime endDate = dtscheduleDate.Date.AddDays(1).AddSeconds(-1);
            var dt = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
              new SugarParameter(":deptCode",deptCode),
              new SugarParameter(":doctorCode",doctorCode),
              new SugarParameter(":scheduleId",scheduleId),
              new SugarParameter(":beginDate",beginDate),
              new SugarParameter(":endDate",endDate)
            });
            //new SugarParameter(":beginDate",dtscheduleDate),
            return dt;
        }

        public int UpdateNum(string orderType, string schemaID)
        {
            string strLockSchema = string.Empty;
            if (orderType == "0")
            {
                strLockSchema = string.Format(@"UPDATE fin_opr_schema   --医师出诊表
SET reged=reged + 0,   --挂号已挂
   tel_reged=tel_reged + 0,   --来电已挂
   tel_reging=tel_reging - 1,   --来电已预约
   spe_reged=spe_reged + 0,   --特诊已挂       
   order_no = nvl(order_no,0) + 0 
WHERE id = '{0}'", schemaID);
            }
            else if (orderType == "1")
            {
                strLockSchema = string.Format(@"UPDATE fin_opr_schema   --医师出诊表
SET reged=reged - 1,   --挂号已挂
   tel_reged=tel_reged + 0,   --来电已挂
   tel_reging=tel_reging + 0,   --来电已预约
   spe_reged=spe_reged + 0,   --特诊已挂       
   order_no = nvl(order_no,0) + 0 
WHERE id = '{0}'", schemaID);
            }
            else
            {
                throw new Exception("挂号类型不正确！");
            }
            return this.Db.Ado.ExecuteCommand(strLockSchema);
        }

        public System.Data.DataTable QueryBookDept()
        {
            string sql = @"SELECT 
DISTINCT 
t.bro_id broId,
regexp_replace( t.bro_name,'[＆&]','、')  broName ,
t.sort_id1 sortId,
t.dept_code deptCode,
regexp_replace( t.dept_name,'[＆&]','、') deptName 
FROM com_department t 
--INNER JOIN fin_opr_schema sc ON sc.dept_code=t.dept_code
WHERE t.bro_id IS NOT NULL 
AND t.valid_state='1' 
--and regexp_like(t.dept_name,'[＆&]')
--AND trunc(sc.see_date)=trunc(:settime)
ORDER BY t.bro_id,t.sort_id1";

            System.Data.DataTable dat = Db.Ado.GetDataTable(sql, new List<SugarParameter>() { });

            return dat;
        }

        public System.Data.DataTable QueryZZQBookDept()
        {
            string sql = @"SELECT 
DISTINCT 
t.bro_id broId,
regexp_replace( t.bro_name,'[＆&]','、')  broName ,
t.sort_id1 sortId,
t.dept_code deptCode,
t.dept_name deptName  
FROM com_department t 
--INNER JOIN fin_opr_schema sc ON sc.dept_code=t.dept_code
WHERE t.bro_id IS NOT NULL 
AND t.valid_state='1' 
--and regexp_like(t.dept_name,'[＆&]')
--AND trunc(sc.see_date)=trunc(:settime)
AND t.dept_code IN (SELECT d.code FROM com_dictionary d WHERE d.TYPE = 'ELDERLYVOUCHERREGDEPT')
ORDER BY t.bro_id,t.sort_id1";

            System.Data.DataTable dat = Db.Ado.GetDataTable(sql, new List<SugarParameter>() { });

            return dat;
        }


        public string GetEMPLNAMEbyEMPLCODE(string CODE)
        {
            string sql = @"select empl_name from Com_Employee e where e.empl_code = '"+ CODE + "'";
            string str = Db.Ado.GetString(sql, new List<SugarParameter>() { });
            return str;
        }

        public System.Data.DataTable QueryByDoctorCode(DateTime beginDate, DateTime endDate, string doctorCode)
        {
            /*string sql = @"select 
t.dept_code ""deptCode""      --科室代码    必填
,regexp_replace(t.dept_name,'[＆&]','、') ""deptName""--科室名称 必填
, t.doct_code ""doctorCode""--医生代码 必填
, nvl(t.doct_name,t.dept_name)""doctorName""--医生名称 必填
,'' ""doctorTitle""--医生职称
,'' ""doctorIntrodution""--医生介绍
,t.see_date ""scheduleDate""--号源日期 必填
, t.noon_code ""timeFlag""--时段 必填
,decode(t.Schema_Type,'0','0','1') ""hasDetailTime""--是否有分时 必填 0 - 否 1 - 是
,t.begin_time ""beginTime""--开始时间 必填
, t.end_time ""endTime""--结束时间 必填
, t.valid_flag ""workStatus""--出诊状态 必填
,round(f.diag_fee*100) ""regFee""--挂号费 单位分 必填
--, t.spe_lmt ""totalNum""--号源总数 必填
--, t.spe_lmt - t.spe_reged ""leftNum""--剩余号源数 必填
,t.reg_lmt ""totalNum""--号源总数 必填  --给平台的号源数量
,t.reg_lmt - t.reged ""leftNum""--剩余号源数 必填
, t.id ""scheduleId""--班次Id 必填
, t.reglevl_code ""level""--号源类别 必填
,'' ""sequenceNo""--排序
,t.tel_lmt-t.tel_reging  ""leftappNum"" --剩余预约号源
,t.tel_lmt              ""totalappNum""--预约号源总数
from FIN_OPR_SCHEMA t JOIN fin_opr_regfeeonpact f ON t.reglevl_code = f.reglevl_code
WHERE f.pact_code='1' and t.valid_flag='1' and  t.room_id is not null  and t.end_time > sysdate and t.doct_code=:doctorCode
and t.see_date between :beginDate and :endDate  order by t.dept_name, t.begin_time";*/
//            string sql = @"select 
//distinct 
//t.dept_code ""deptCode"",
//t.dept_name ""deptName"",
//t.doct_code ""doctorCode"",
//t.doct_name ""doctorName"",
//' ' ""doctorSkill"",
//' ' ""doctorIntrodution"",
//decode(emp.levl_code,'09','1','10','2','11','3','13','4','9') ""techTitle"",
//decode(emp.levl_code,'01','1','02','2','03','3','4') ""rank"",
//' ' ""mobile"",
//' ' ""telephone""
//from FIN_OPR_SCHEMA t 
//inner JOIN fin_opr_regfeeonpact f ON t.reglevl_code = f.reglevl_code
//inner join com_employee emp on emp.empl_code=t.doct_code
//WHERE f.pact_code='1' 
//and t.stop<>'1'
//and t.valid_flag='1' 
//and  t.room_id is not null  
//and t.end_time > sysdate and t.doct_code=:doctorCode
//and t.see_date between :beginDate and :endDate  
//order by t.dept_code  ";
            string sql = @"select distinct * from (select 
distinct 
t.dept_code ""deptCode"",
t.dept_name ""deptName"",
t.doct_code ""doctorCode"",
t.doct_name ""doctorName"",
' ' ""doctorSkill"",
' ' ""doctorIntrodution"",
decode(emp.levl_code,'09','1','10','2','11','3','13','4','9') ""techTitle"",
decode(emp.levl_code,'01','1','02','2','03','3','4') ""rank"",
' ' ""mobile"",
' ' ""telephone"",
(SELECT decode(count(1),0,'0','1') FROM com_dictionary d where d.type = 'ELDERLYVOUCHERREGDEPT' and d.valid_state = '1' AND d.code = t.dept_code) ""elderlyVoucherRegDeptFlag"",
(SELECT decode(count(1),0,'0','1') FROM com_dictionary d where d.type = 'ELDERLYVOUCHERDOCTOR' and d.valid_state = '1' AND d.code = t.doct_code) ""elderlyVoucherDoctorFlag""
from FIN_OPR_SCHEMA t 
inner JOIN fin_opr_regfeeonpact f ON t.reglevl_code = f.reglevl_code
inner join com_employee emp on emp.empl_code=t.doct_code
inner join com_department d on d.dept_code = t.dept_code
WHERE f.pact_code='1' 
and d.bro_id IS NOT NULL 
and t.stop<>'1'
and t.valid_flag='1' 
and  t.room_id is not null  
and t.end_time > sysdate and t.doct_code=:doctorCode
and t.see_date between :beginDate and :endDate  
union all
select 
distinct 
SCHEMA_DEPT_CODE ""deptCode"",
SCHEMA_DEPT_NAME ""deptName"",
doctorCode ""doctorCode"",
doctorName ""doctorName"",
' ' ""doctorSkill"",
' ' ""doctorIntrodution"",
techTitle ""techTitle"",
rank ""rank"",
' ' ""mobile"",
' ' ""telephone"",
elderlyVoucherRegDeptFlag ""elderlyVoucherRegDeptFlag"",
elderlyVoucherDoctorFlag ""elderlyVoucherDoctorFlag""
from(
select 
SCHEMA_DEPT_CODE,
SCHEMA_DEPT_NAME,
t.doct_code doctorCode,
t.doct_name doctorName,
decode(emp.levl_code,'09','1','10','2','11','3','13','4','9') techTitle,
decode(emp.levl_code,'01','1','02','2','03','3','4') rank,
(SELECT decode(count(1),0,'0','1') FROM com_dictionary c where c.type = 'ELDERLYVOUCHERREGDEPT' and c.valid_state = '1' AND c.code = d.dept_code) elderlyVoucherRegDeptFlag,
(SELECT decode(count(1),0,'0','1') FROM com_dictionary d where d.type = 'ELDERLYVOUCHERDOCTOR' and d.valid_state = '1' AND d.code = t.doct_code) elderlyVoucherDoctorFlag
from FIN_OPR_SCHEMA t 
inner JOIN fin_opr_regfeeonpact f ON t.reglevl_code = f.reglevl_code
inner join com_employee emp on emp.empl_code=t.doct_code
inner join com_department d on d.dept_code = t.dept_code
WHERE f.pact_code='1' 
and d.bro_id IS NOT NULL 
and t.stop<>'1'
and t.valid_flag='1' 
and  t.room_id is not null  
and t.end_time > sysdate and t.doct_code=:doctorCode
and t.see_date between :beginDate and :endDate
and SCHEMA_DEPT_NAME is not null
))a ";
            var dt = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
              new SugarParameter(":doctorCode",doctorCode),
              new SugarParameter(":beginDate",beginDate),
              new SugarParameter(":endDate",endDate)
            });
            return dt;
        }

    }
}
