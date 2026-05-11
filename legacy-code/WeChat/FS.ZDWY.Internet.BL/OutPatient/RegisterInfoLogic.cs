using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Data;

namespace FS.ZDWY.Internet.BL
{
    public class RegisterInfoLogic : SqlSugar.DbContext<FS.ZDWY.Internet.Models.FIN_OPR_REGISTER>
    {
        public DataTable QueryRegisterInfo(string patientId, DateTime startDate, DateTime endDate)
        {
            string sql = @"select
                    f.clinic_code ""visitNo""   --就诊号   必填
                    ,f.dept_code ""deptCode""  --科室code  必填
                    ,f.dept_name ""deptName""  --科室名称  必填
                    ,nvl(f.doct_code,'无') ""doctorCode""  --医生code  必填
                    ,nvl(f.doct_name,'无') ""doctorName""  --医生名称  必填
                    ,f.see_date ""registerDate""  --就诊日期  必填
                    ,f.reg_date ""regDate"" --挂号日期  必填
                    ,f.clinic_code ""registrationNo""  --门诊挂号单号  必填
                    ,f.clinic_code ""outpatId""  --门诊号 必填
                    ,f.reg_fee ""amount""  --挂号金额  必填
                    from fin_opr_register f
                    where f.card_no = :patientId
                    AND f.begin_time >= :startDate AND f.end_time <= :endDate 
                    AND f.valid_flag = '1'  ORDER BY F.REG_DATE DESC";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                        new SugarParameter(":patientId",patientId),
                        new SugarParameter(":startDate",startDate),
                        new SugarParameter(":endDate",endDate)
            });
            return queryData;
        }

        /// <summary>
        /// 查询患者加号信息
        /// </summary>
        /// <param name="patientId">门诊号</param>
        /// <returns></returns>
        public DataTable QueryAddRegInfo(string patientId)
        {
            string sql = @"select clinic_code ""hospitalNum"",
                                  (select p.orderid from PLATFORM_REGISTER_ORDER p where p.clinic_code=b.clinic_code) ""orderid"",
                                  to_char(oper_date, 'yyyy-MM-dd hh24:mi:ss') ""orderTime"",
                                  dept_code ""deptCode"",
                                  doct_code ""doctorCode"",
                                  to_char(booking_date, 'yyyy-MM-dd hh24:mi:ss') ""scheduleDate"",
                                  schema_no ""scheduleId"",
                                  '' ""numberinfoId"",
                                  to_char(begin_time, 'hh24:mi') ""beginTime"",
                                  to_char(end_time, 'hh24:mi') ""endTime"",
                                  nvl((select r.diag_fee
                                        from fin_opr_regfeeonpact r
                                       where r.reglevl_code = b.reglevl_code
                                         and r.pact_code = (select pact_code
                                                             from com_patientinfo p
                                                            where p.card_no = b.card_no)),
                                     0)*100 ""regFee"",
                                  '1' ""regType"",
                                  '' ""payChannel"",
                                  decode(nvl((select patient_type
                                               from com_patientinfo p
                                              where p.card_no = b.card_no),
                                             '0'),
                                         '4',
                                         '1',
                                         '0') ""identityType"",
                                  '' ""visitNo"",
                                  '' ""visitAddress"",
                                  '' ""remark"",
                                  '' ""proof""
                             from FIN_OPR_BOOKING b 
                            where clinic_code = (select max(t.clinic_code) 
                                                   from FIN_OPR_BOOKING t  ,PLATFORM_REGISTER_ORDER a 
                                                  where  t.clinic_code=a.clinic_code and t.card_no = :cardNo
                                                    and t.valid_flag = '1'
                                                    and t.see_flag = '0'
                                                    and t.app_flag = '1'  and a.status='1' and t.oper_date>sysdate-1/24) ";
            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>()
            {
                new SugarParameter(":cardNo", patientId)
            });
            return queryData;
        }

        /// <summary>
        /// 更新排班的挂号限额
        /// </summary>
        /// <param name="schemaId">排班ID</param>
        /// <returns></returns>
        public int UpdateSchemaRegLmt(string schemaId)
        {
            try
            {
                string updSql = @"update fin_opr_schema set reg_lmt = reg_lmt + 1 where id = '{0}'";
                updSql = string.Format(updSql, schemaId);
                var execRev = Db.Ado.ExecuteCommand(updSql);
                int result = Convert.ToInt16(execRev.ToString());
                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public string GetHSTodayClincCodeForCardNo(string cardNo)
        {
            string sql = string.Format(" select max(p.clinic_code) from fin_opr_register p where p.card_no='{0}' and trunc(p.reg_date)=trunc(sysdate) and p.valid_flag='1' and p.dept_code in('6126','6212') ", cardNo);
            return this.Db.Ado.GetString(sql);

        }
    }
}

