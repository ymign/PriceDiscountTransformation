using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Pharmacy
{

    /// <summary>
    /// 西药库日结日清管理类
    /// </summary>

    public class XYKStorageDaily : Neusoft.FrameWork.Management.Database
    {
        public XYKStorageDaily()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }


        

        /// <summary>
        /// 新增数据(查询有出入库台账的药品日结信息)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int AddXYKStorageDailyByTz(string DrugCode,DateTime DailyDate, ref string errMsg)
        {
            try
            {
                string sql = @"insert into PHA_COM_XYKSTORAGEDAILY (DRUG_CODE,VOUCHER,DAILYDATE,ABSTRACTINFO,INPUT_NUM,OUTPUT_NUM,BATCH_NO,VALID_DATE,BATCH_NOSUM,TOT_SUM,HANDOVERPERSON,REVIEWER,RECIPIENT,ORDERNO)
--查询有出入库台账的药品日结信息
select r.drug_code,
       nvl(o.out_list_code, i.in_list_code) pzh,
       TO_DATE('{3}', 'yyyy-mm-dd hh24:mi:ss') daily_date,
       nvl(fun_get_dept_name(r.goal_dept_code),
           fun_get_company_name(r.goal_dept_code)) zy,
       case
         when r.inout_num > 0 then
          round(r.inout_num, 2)
         else
          0
       end as input_num,
       case
         when r.inout_num < 0 then
          ABS(round(r.inout_num, 2))
         else
          0
       end as output_num,
       r.batch_no,
       r.valid_date,
       null phjcs,
       null zjcs,
       nvl(o.apply_opercode,
           i.oper_code) jbr,
       nvl(o.exam_opercode,
           i.EXT_CODE1) fhr,
       (SELECT f.oper_code
          FROM pha_com_input f
         where f.in_list_code = o.in_list_code
           and f.in_bill_code = o.in_bill_code) lyr,
       '0'
  from pha_com_record   r,
       com_priv_class3  a,
       pha_com_output   o,
       pha_com_input    i,
       pha_com_baseinfo d
 where r.record_type = a.class2_code
   and r.bill_code = o.out_bill_code(+)
   and r.bill_code = i.in_bill_code(+)
   and r.drug_code = d.drug_code
   and r.class3_meaning_code = a.class3_code
   and r.source_dept_code = '9006'
   and r.drug_code = '{0}'
   and r.oper_date between
       TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss') and
       TO_DATE('{2}', 'yyyy-mm-dd hh24:mi:ss')

union all

SELECT t.drug_code,
       '' pzh,
       t.daily_date,
       t.zy,
       sum(t.input_num) input_num,
       sum(t.output_num) output_num,
       t.batch_no,
       t.valid_date,
       t.phjcs,
       t.zjcs,
       t.jbr,
       t.fhr,
       '' lyr,
       '1'
  FROM (select r.drug_code,
               TO_DATE('{3}', 'yyyy-mm-dd hh24:mi:ss') daily_date,
               '日结' zy,
               case
                 when r.inout_num > 0 then
                  round(r.inout_num, 2)
                 else
                  0
               end as input_num,
               case
                 when r.inout_num < 0 then
                  ABS(round(r.inout_num, 2))
                 else
                  0
               end as output_num,
               r.batch_no,
               r.valid_date,
               (SELECT sum(round(f.store_sum, 2))
                  FROM pha_com_storage f
                 where f.drug_dept_code = '9006'
                   and f.drug_code = r.drug_code
                   and f.batch_no = r.batch_no
                   and f.valid_flag = '1'
                 group by f.drug_code, f.batch_no) phjcs,
               null zjcs,
               '{4}' jbr,
               '145117' fhr
          from pha_com_record r, com_priv_class3 a
         where r.record_type = a.class2_code
           and r.class3_meaning_code = a.class3_code
           and r.source_dept_code = '9006'
           and r.drug_code = '{0}'
           and r.oper_date between
               TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss') and
               TO_DATE('{2}', 'yyyy-mm-dd hh24:mi:ss')) t
 group by t.drug_code,
          t.daily_date,
          t.zy,
          t.batch_no,
          t.valid_date,
          t.phjcs,
          t.zjcs,
          t.jbr,
          t.fhr";
                DateTime startTime = Convert.ToDateTime(DailyDate.ToString("D").ToString());
                DateTime endTime = Convert.ToDateTime(DailyDate.AddDays(1).ToString("D").ToString()).AddSeconds(-1);
                sql = string.Format(sql, DrugCode, startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), DailyDate.ToString("yyyy-MM-dd"), this.Operator.ID);
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }


        }

        

        /// <summary>
        /// 新增数据2(没有出入库，但是有库存的药品日结信息)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int AddXYKStorageDailyByNoTz(string DrugCode,DateTime DailyDate, ref string errMsg)
        {
            try
            {
                string sql = @"insert into PHA_COM_XYKSTORAGEDAILY (DRUG_CODE,DAILYDATE,ABSTRACTINFO,INPUT_NUM,OUTPUT_NUM,BATCH_NO,VALID_DATE,BATCH_NOSUM,TOT_SUM,HANDOVERPERSON,REVIEWER,ORDERNO)
(
--查询没有出入库台账，但是有库存的药品日结信息
SELECT f.drug_code,TO_DATE('{3}','yyyy-mm-dd hh24:mi:ss') daily_date,'日结' zy,0 input_num,0 output_num,f.batch_no batch_no,f.valid_date valid_date,sum(round(f.store_sum,2)) phjcs,null zjcs,'{4}' jbr,'145117','2' fhr FROM pha_com_storage f where f.drug_dept_code = '9006' and f.drug_code = '{0}' and f.batch_no not in (
SELECT r.batch_no
  from pha_com_record r, com_priv_class3 a
 where r.record_type = a.class2_code 
   and r.class3_meaning_code = a.class3_code
   and  r.source_dept_code = '9006'
   and r.drug_code = '{0}'
   and r.oper_date between
       TO_DATE('{1}','yyyy-mm-dd hh24:mi:ss') and
       TO_DATE('{2}','yyyy-mm-dd hh24:mi:ss') group by r.batch_no
       )
   and f.valid_flag = '1'
   and f.store_sum > 0
group by f.drug_code,f.batch_no,f.valid_date
union all
SELECT '{0}' drug_code,
       TO_DATE('{3}', 'yyyy-mm-dd hh24:mi:ss') daily_date,
       '日结' zy,
       0 input_num,
       0 output_num,
       '' batch_no,
       null valid_date,
       null phjcs,
       0 zjcs,
       '{4}' jbr,
       '145117',
       '2' fhr
  FROM dual
 where (SELECT count(1)
          FROM pha_com_storage f
         where f.drug_dept_code = '9006'
           and f.drug_code = '{0}'
           and f.valid_flag = '1'
           and f.store_sum > 0) = 0
)";
                DateTime startTime = Convert.ToDateTime(DailyDate.ToString("D").ToString());
                DateTime endTime = Convert.ToDateTime(DailyDate.AddDays(1).ToString("D").ToString()).AddSeconds(-1);
                sql = string.Format(sql, DrugCode, startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), DailyDate.ToString("yyyy-MM-dd"), this.Operator.ID);
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }


        }

        

        /// <summary>
        /// 新增数据3(库存的药品日结信息和汇总信息)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int AddXYKStorageDailyByHaveTz(string DrugCode, DateTime DailyDate, ref string errMsg)
        {
            try
            {
                string sql = @"insert into PHA_COM_XYKSTORAGEDAILY (DRUG_CODE,DAILYDATE,ABSTRACTINFO,INPUT_NUM,OUTPUT_NUM,BATCH_NO,VALID_DATE,BATCH_NOSUM,TOT_SUM,HANDOVERPERSON,REVIEWER,ORDERNO)
(
--查询没有出入库台账，但是有库存的药品日结信息
SELECT f.drug_code,TO_DATE('{3}','yyyy-mm-dd hh24:mi:ss') daily_date,'日结' zy,0 input_num,0 output_num,f.batch_no batch_no,f.valid_date valid_date,sum(round(f.store_sum,2)) phjcs,null zjcs,'{4}' jbr,'145117' fhr,'2' FROM pha_com_storage f where f.drug_dept_code = '9006' and f.drug_code = '{0}' and f.batch_no not in (
SELECT r.batch_no
  from pha_com_record r, com_priv_class3 a
 where r.record_type = a.class2_code 
   and r.class3_meaning_code = a.class3_code
   and  r.source_dept_code = '9006'
   and r.drug_code = '{0}'
   and r.oper_date between
       TO_DATE('{1}','yyyy-mm-dd hh24:mi:ss') and
       TO_DATE('{2}','yyyy-mm-dd hh24:mi:ss') group by r.batch_no
       )
   and f.valid_flag = '1'
   and f.store_sum > 0
group by f.drug_code,f.batch_no,f.valid_date
union all
--汇总查询
SELECT t.drug_code,
       TO_DATE('{3}','yyyy-mm-dd hh24:mi:ss') daily_date,
       '日结' zy,
       sum(t.input_num) input_num,
       sum(t.output_num) output_num,
       '' batch_no,
       null valid_date,
       null phjcs,
       (SELECT sum(round(f.store_sum,2)) FROM pha_com_storage f where f.drug_dept_code = '9006' and f.drug_code = t.drug_code and f.valid_flag = '1') zjcs,
       '{4}' jbr,
       '145117' fhr,'3' FROM (select r.drug_code,
                    
                    case
                      when r.inout_num > 0 then
                       round(r.inout_num, 2)
                      else
                       0
                    end as input_num,
                    case
                      when r.inout_num < 0 then
                       ABS(round(r.inout_num, 2))
                      else
                       0
                    end as output_num
               from pha_com_record r, com_priv_class3 a
              where r.record_type = a.class2_code
                and r.class3_meaning_code = a.class3_code
                and r.source_dept_code = '9006'
                and r.drug_code = '{0}'
                and r.oper_date between
                    TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss') and
                    TO_DATE('{2}', 'yyyy-mm-dd hh24:mi:ss'))t group by t.drug_code
)";
                DateTime startTime = Convert.ToDateTime(DailyDate.ToString("D").ToString());
                DateTime endTime = Convert.ToDateTime(DailyDate.AddDays(1).ToString("D").ToString()).AddSeconds(-1);
                sql = string.Format(sql, DrugCode, startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), DailyDate.ToString("yyyy-MM-dd"), this.Operator.ID);
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }


        }

        /// <summary>
        /// 新增数据4(汇总信息)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int AddXYKStorageDailyTot(string DrugCode, DateTime DailyDate, ref string errMsg)
        {
            try
            {
                string sql = @"INSERT INTO PHA_COM_XYKSTORAGEDAILY
  (DRUG_CODE,
   DAILYDATE,
   ABSTRACTINFO,
   INPUT_NUM,
   OUTPUT_NUM,
   BATCH_NO,
   VALID_DATE,
   BATCH_NOSUM,
   TOT_SUM,
   HANDOVERPERSON,
   REVIEWER,
   ORDERNO)
  (SELECT F.DRUG_CODE,
          TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss') DAILY_DATE,
          '日结' ZY,
          0 INPUT_NUM,
          0 OUTPUT_NUM,
          '' BATCH_NO,
          NULL VALID_DATE,
          NULL PHJCS,
          SUM(ROUND(F.STORE_SUM, 2)) ZJCS,
          '{2}' JBR,
          '145117' FHR,
          '3'
     FROM PHA_COM_STORAGE F
    WHERE F.DRUG_DEPT_CODE = '9006'
      AND F.DRUG_CODE ='{0}'
      AND F.VALID_FLAG = '1'
    GROUP BY F.DRUG_CODE
   )";
                sql = string.Format(sql, DrugCode, DailyDate.ToString("yyyy-MM-dd"), this.Operator.ID);
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }


        }


        /// <summary>
        /// 绑定西药库日结日清药品下拉框
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryXykDailyDrugList()
        {
            string sql = @"select a.drug_code,a.trade_name,a.spell_code,a.wb_code,a.valid_state from pha_com_baseinfo a
                          where a.valid_state='1' and a.drug_quality in ('SY','S1','P1','YZ') ";
            ArrayList list = this.GetDrugList(sql);
            if (list == null || list.Count == 0) return null;
            return list;
        }

        /// <summary>
        /// 绑定精二药品下拉框
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryP2DrugList()
        {
            string sql = @"select a.drug_code,a.trade_name,a.spell_code,a.wb_code,a.valid_state from pha_com_baseinfo a
                          where a.valid_state='1' and a.drug_quality in ('P2','UC') ";
            ArrayList list = this.GetDrugList(sql);
            if (list == null || list.Count == 0) return null;
            return list;
        }

        public int QueryDrugList(ref DataTable dt)
        {
            string strSql = @"select a.drug_code,a.trade_name,a.spell_code,a.wb_code,a.valid_state from pha_com_baseinfo a
                          where a.valid_state='1' and a.drug_quality in ('SY','S1','P1','YZ') ";
            DataSet dataSet = new DataSet();
            try
            {
                if (this.ExecQuery(strSql, ref dataSet) == -1)
                {
                    this.Err = "查询医保结算合同单位失败！";
                    return -1;
                }
                dt = dataSet.Tables[0];
                return 1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public int DeleteXykDailyInfoByDateAndDrug(string DrugCode, DateTime DailyDate, ref string errMsg) 
        {
            try
            {
                string sql = @"delete from PHA_COM_XYKSTORAGEDAILY f where f.drug_code = '{0}' and f.dailydate between
       TO_DATE('{1}','yyyy-mm-dd hh24:mi:ss') and
       TO_DATE('{2}','yyyy-mm-dd hh24:mi:ss')";
                DateTime startTime = Convert.ToDateTime(DailyDate.ToString("D").ToString());
                DateTime endTime = Convert.ToDateTime(DailyDate.AddDays(1).ToString("D").ToString()).AddSeconds(-1);
                sql = string.Format(sql, DrugCode, startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }
        }

        public int PlDeleteXykDailyInfoByDateAndDrug(DateTime DailyDate, ref string errMsg)
        {
            try
            {
                string sql = @"delete from PHA_COM_XYKSTORAGEDAILY f where f.drug_code in (select a.drug_code from pha_com_baseinfo a
                          where a.valid_state='1' and a.drug_quality in ('SY','S1','P1','YZ')) and f.dailydate between
       TO_DATE('{0}','yyyy-mm-dd hh24:mi:ss') and
       TO_DATE('{1}','yyyy-mm-dd hh24:mi:ss')";
                DateTime startTime = Convert.ToDateTime(DailyDate.ToString("D").ToString());
                DateTime endTime = Convert.ToDateTime(DailyDate.AddDays(1).ToString("D").ToString()).AddSeconds(-1);
                sql = string.Format(sql,startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }
        }

        public int UpdateXykDailyInfo(string Batch_NoSum, string Tot_Sum, string Drug_Code, DateTime DailyDate,string Batch_No, ref string errMsg)
        {
            try
            {
                string sql = @"update PHA_COM_XYKSTORAGEDAILY f set f.batch_nosum = '{0}',f.tot_sum = '{1}' where f.drug_code = '{2}' and f.dailydate = TO_DATE('{3}','yyyy-mm-dd hh24:mi:ss') and f.batch_no = '{4}'";
                sql = string.Format(sql, Batch_NoSum,Tot_Sum,Drug_Code,DailyDate.ToString(),Batch_No);
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }
        }

        public int UpdateXykDailyInfoZJ(string Batch_NoSum,string Tot_Sum,string Drug_Code,DateTime DailyDate, ref string errMsg)
        {
            try
            {
                string sql = @"update PHA_COM_XYKSTORAGEDAILY f set f.batch_nosum = '{0}',f.tot_sum = '{1}' where f.drug_code = '{2}' and f.dailydate = TO_DATE('{3}','yyyy-mm-dd hh24:mi:ss') and f.batch_no is null";
                sql = string.Format(sql, Batch_NoSum,Tot_Sum,Drug_Code,DailyDate.ToString());
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList GetDrugList(string sql)
        {

            if (this.ExecQuery(sql) == -1) return null;
            ArrayList list = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Pharmacy.Item item = new Neusoft.HISFC.Models.Pharmacy.Item();
                if (!Reader.IsDBNull(0))
                    item.ID = Reader[0].ToString();
                if (!Reader.IsDBNull(1))
                    item.Name = Reader[1].ToString();
                if (!Reader.IsDBNull(2))
                    item.SpellCode = Reader[2].ToString();
                if (!Reader.IsDBNull(3))
                    item.WBCode = Reader[3].ToString();
                list.Add(item);
            }

            return list;
        }
    }
}
