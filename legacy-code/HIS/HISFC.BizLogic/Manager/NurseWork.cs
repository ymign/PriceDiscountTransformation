using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Manager
{
    public class NurseWork : DataBase
    {

        public DataSet GetData(string date, string dataName, string deptCode)
        {
            string sql = @"
select T1.MYDATE, T2.DATA_NUM, T2.VALID_STATE FROM
(select to_char(mydate,'yyyy""年""mm""月""dd""日""') MYDATE from 
(SELECT (TRUNC(TO_DATE('{0}','YYYY-MM-DD'), 'MM') + ROWNUM - 1) mydate
  FROM DUAL
CONNECT BY ROWNUM <= TO_NUMBER(TO_CHAR(LAST_DAY(TO_DATE('{0}','YYYY-MM-DD')), 'dd'))))T1
LEFT JOIN
(select TO_CHAR(A.DATA_DATE,'yyyy""年""mm""月""dd""日""') DATA_DATE,A.DATA_NAME, A.DATA_NUM, A.VALID_STATE
  from FIN_COM_DATA A
 where A.DATA_NAME = '{1}'
   and A.DEPT_CODE = '{2}'
   and a.is_state='0')T2
   ON T1.MYDATE = T2.DATA_DATE
   ORDER BY T1.MYDATE
";
            DataSet ds = new DataSet();
            sql = string.Format(sql, date, dataName, deptCode);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds; 
        }

        public DataSet GetLevlCodeState(string OperId)
        {
            string sql = @"select n.posi_code
                          from com_employee n
                         where n.empl_code='{0}'
                             and n.valid_state='1'

";
            DataSet ds = new DataSet();
            sql = string.Format(sql, OperId);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public DataSet GetDeptName()
        {
            string sql = @"
                        select distinct dd.dept_code from fin_com_data dd
";
            DataSet ds = new DataSet();
            ///sql = string.Format();
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public DataSet GetData1 (string date, string dataName, string deptCode)
        {
            string sql = @"
 select T1.NAME,T2.DATA_NUM,T2.VALID_STATE
   from
   (select h.name,h.code,h.sort_id from FIN_COM_dictionary h where h.type='ZYKSGZL' and h.code='{2}') T1 left join 
   (select A.DATA_NAME, A.DATA_NUM, A.VALID_STATE,a.dept_code,a.month_data
  from FIN_COM_DATA A
 where to_char(a.data_date,'YYYY-MM')='{0}'
and A.DATA_NAME = '{1}'
   and A.DEPT_CODE = '{2}'
   and is_state='1')T2
   ON T1.code = T2.dept_code and T2.month_data=T1.NAME
   order by T1.sort_id
";
            DataSet ds = new DataSet();
            sql = string.Format(sql, date,dataName, deptCode);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public DataSet GetHerpDept()
            //(string date, string dataName, string deptCode)
        {
            string sql = @"
select T1.dept_code,t1.dept_name,t2.sort_id,t2.Hos_Code,T2.dept_code2,t2.dept_name2
   from
   (select h.dept_code,h.dept_name from Com_Department h ) T1 left join 
   (select A.DEPT_CODE, A.Dept_Name,a.sort_id,A.Hos_Code,a.dept_code2,a.dept_name2
  from com_department_yf_temp A)T2
   ON T1.Dept_Code = T2.dept_code
   order by t2.Hos_Code
";
            DataSet ds = new DataSet();
            //sql = string.Format(sql, date, dataName, deptCode);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public DataSet GetHerpDept1()
        //(string date, string dataName, string deptCode)
        {
            string sql = @"
select T1.dept_code,t1.dept_name,t2.sort_id,t2.Hos_Code,T2.dept_code2,t2.dept_name2
   from
   (select h.dept_code,h.dept_name from Com_Department h ) T1 left join 
   (select A.DEPT_CODE, A.Dept_Name,a.sort_id,A.Hos_Code,a.dept_code2,a.dept_name2
  from com_department_in_temp A)T2
   ON T1.Dept_Code = T2.dept_code
   order by t2.dept_code2
";
            DataSet ds = new DataSet();
            //sql = string.Format(sql, date, dataName, deptCode);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public int UpdateHerpData(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
            update com_department_yf_temp
            set 
            sort_id='{0}',
            hos_code='{1}',
            dept_code2='{2}',
            dept_name2='{3}'
            where dept_code='{4}'
            ";
            try
            {
                strSql = string.Format(strSql, nurseWork.Soid_Id, nurseWork.Hos_Code, nurseWork.Dept_code2, nurseWork.Dept_name2, nurseWork.Dept_code);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int UpdateHerpData1(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
            update com_department_in_temp
            set 
            sort_id='{0}',
            hos_code='{1}',
            dept_code2='{2}',
            dept_name2='{3}'
            where dept_code='{4}'
            ";
            try
            {
                strSql = string.Format(strSql, nurseWork.Soid_Id, nurseWork.Hos_Code, nurseWork.Dept_code2, nurseWork.Dept_name2, nurseWork.Dept_code);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int InsertHerpData(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
                    INSERT INTO com_department_yf_temp --数据主表
                      (dept_code, --HIS科室编码
                       dept_name, --HIS科室名称
                       sort_id, --序号
                       hos_code, --herp科室编码
                       dept_code2,--herp科室编码2
                       dept_name2 --herp科室名称2             
                       )
                    VALUES
                      ('{0}', --HIS科室编码
                       '{1}', --HIS科室名称
                       '{2}', --序号
                       '{3}', --herp科室编码
                       '{4}', --herp科室编码2 5
                       '{5}' --herp科室名称2
                       ) 
                    ";

            try
            {
                strSql = string.Format(strSql, nurseWork.Dept_code, nurseWork.Dept_Name, nurseWork.Soid_Id, nurseWork.Hos_Code, nurseWork.Dept_code2, nurseWork.Dept_name2);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int InsertHerpData1(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
                    INSERT INTO com_department_in_temp --数据主表
                      (dept_code, --HIS科室编码
                       dept_name, --HIS科室名称
                       sort_id, --序号
                       hos_code, --herp科室编码
                       dept_code2,--herp科室编码2
                       dept_name2 --herp科室名称2             
                       )
                    VALUES
                      ('{0}', --HIS科室编码
                       '{1}', --HIS科室名称
                       '{2}', --序号
                       '{3}', --herp科室编码
                       '{4}', --herp科室编码2 5
                       '{5}' --herp科室名称2
                       ) 
                    ";

            try
            {
                strSql = string.Format(strSql, nurseWork.Dept_code, nurseWork.Dept_Name, nurseWork.Soid_Id, nurseWork.Hos_Code, nurseWork.Dept_code2, nurseWork.Dept_name2);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public DataSet GetDayState(string date, string deptCode, string isMonth)
        {
            string sql = @"
  select distinct a.valid_state
    from fin_com_data a
   where to_char(a.data_date, 'yyyy-mm') = '{0}'
     and a.dept_code = '{1}'
     and a.is_state = '{2}'
     --and a.valid_state='0'
     and rownum ='1'
";
            DataSet ds1 = new DataSet();
            sql = string.Format(sql, date, deptCode, isMonth);
            if (this.ExecQuery(sql, ref ds1) == -1)
            {
                return null;
            }
            return ds1;
        }

        public DataSet GetOpsState(string date, string deptCode)
        {
            string sql = @"
  select distinct a.valid_state
    from fin_ops_data a
   where a.op_date = to_date('{0}','yyyy-MM-dd')
     and a.dept_code = '{1}'
     and rownum ='1'
";
            DataSet ds1 = new DataSet();
            sql = string.Format(sql, date, deptCode);
            if (this.ExecQuery(sql, ref ds1) == -1)
            {
                return null;
            }
            return ds1;
        }

        public DataSet GetYearState(string date,string deptCode)
        {
            string sql = @"
  select distinct a.valid_state
    from fin_com_data a
   where to_char(a.data_date, 'yyyy-mm') = '{0}'
     and a.dept_code = '{1}'
     and a.is_state = '1'
     and rownum ='1'
";
            DataSet ds = new DataSet();
            sql = string.Format(sql, date, deptCode);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public DataSet GetopsDate(string deptCode, string date)
        {
            string sql = @"
                      select a.soid_id, a.dept_name, a.doc_name, a.operation_name, a.operation
                          from fin_ops_data a
                         where a.dept_code = '{0}'
                         and a.op_date =to_date( '{1}','yyyy-MM-dd')
                         order by a.soid_id
                    ";
            DataSet ds = new DataSet();
            sql = string.Format(sql, deptCode, date);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public DataSet GetData2(string date, string dataName, string deptCode)
        {
            string sql = @"
 select T1.monthlist,T2.DATA_NUM,T2.VALID_STATE
   from
   (SELECT substr(to_char(TO_DATE('{0}-01', 'yyyy-MM-DD'), 'yyyymm'),
                0,
                4) || '年' ||
         ltrim(to_char(TO_DATE('{0}-01', 'yyyy-MM-DD'), 'mm'), 0)||'月' monthlist
  
    FROM DUAL) T1 left join 
   (select A.DATA_NAME, A.DATA_NUM, A.VALID_STATE,a.month_data
  from FIN_COM_DATA A
 where to_char(a.data_date,'YYYY-MM')='{0}'
and A.DATA_NAME = '{1}'
   and A.DEPT_CODE = '{2}'
   and is_state='1')T2
   on T2.month_data=T1.monthlist
";
            DataSet ds = new DataSet();
            sql = string.Format(sql, date, dataName, deptCode);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public int UpdateData(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
                update FIN_COM_DATA
                   set DATA_NUM = {0},
                      oper_date = sysdate
                 where DATA_DATE = to_date( '{1}','yyyy-mm-dd') and DATA_NAME='{2}' and DEPT_CODE='{3}' and is_state='0'
                ";
            try
            {
                strSql = string.Format(strSql, nurseWork.Data_num, nurseWork.Data_date, nurseWork.Data_name, nurseWork.Dept_code);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int UpdateData1(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
update FIN_COM_DATA
   set DATA_NUM = {0},
      oper_date = sysdate
 where DATA_DATE = to_date( '{1}-01','yyyy-mm-dd') and DATA_NAME='{2}' and DEPT_CODE='{3}' and month_data='{4}' and is_state='1'
";
            try
            {
                strSql = string.Format(strSql, nurseWork.Data_num, nurseWork.Data_date, nurseWork.Data_name, nurseWork.Dept_code, nurseWork.Month_data);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int UpdateOpsData(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
            update fin_ops_data t
            set t.dept_name='{0}',
                t.doc_name='{1}',
                t.operation_name='{2}',
                t.operation='{3}',
                t.oper_code='{4}',
                t.oper_date=sysdate
                where t.op_date =to_date('{5}','yyyy-MM-dd')
                and t.dept_code ='{6}'
                and t.soid_id ='{7}'
";
            try
            {
                strSql = string.Format(strSql, nurseWork.Dept_Name, nurseWork.Doc_name, nurseWork.Operation_Name, nurseWork.Operation, nurseWork.Oper_code, nurseWork.Op_date, nurseWork.Dept_code, nurseWork.Soid_Id);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int UpdateCheckOps(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
                         update fin_ops_data a
                         set a.valid_state = '1',
                             a.check_code ={0},
                             a.check_date =sysdate
                          where a.dept_code = '{1}'
                         and a.valid_state = '0'
                         and a.op_date =to_date( '{2}','yyyy-MM-dd')
                          ";
            try
            {
                strSql = string.Format(strSql, nurseWork.Oper_code, nurseWork.Dept_code, nurseWork.Op_date);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }


        public int UpdateCancelCheckOps(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
                         update fin_ops_data a
                         set a.valid_state = '0',
                             a.check_code =null,
                             a.check_date =null
                          where a.dept_code = '{1}'
                         and a.valid_state= '1'
                         and a.op_date =to_date( '{2}','yyyy-MM-dd')
                          ";
            try
            {
                strSql = string.Format(strSql, nurseWork.Oper_code, nurseWork.Dept_code, nurseWork.Op_date);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int InsertData(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
                    INSERT INTO FIN_COM_DATA --数据主表
                      (DATA_DATE, --时间
                       DATA_CODE, --编码
                       DATA_NAME, --名称
                       DATA_NUM, --数量
                       DEPT_CODE,--科室编码
                       OPER_CODE, --操作人             
                       OPER_DATE, --操作时间
                       VALID_STATE,--解锁状态
                       IS_STATE    --报表状态
                       )
                    VALUES
                      (TO_DATE('{0}', 'yyyy-mm-dd'), --时间
                       '{1}', --编码
                       '{2}', --名称
                       {3}, --数量
                       '{4}', --科室编码 5
                       '{5}', --操作人
                       sysdate,
                       '{6}',--解锁状态
                       '{7}')  --报表状态
                    ";

            try
            {
                strSql = string.Format(strSql, nurseWork.Data_date, nurseWork.Data_code, nurseWork.Data_name, nurseWork.Data_num, nurseWork.Dept_code, nurseWork.Oper_code, nurseWork.Valid_state, nurseWork.Is_state);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int InsertData3(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
                INSERT INTO fin_ops_data --数据主表
                  (op_date, --时间
                   dept_code,--科室
                   dept_name, --科室名称
                   doc_name, --医生名称
                   operation_name,--手术名称
                   operation,--手术例数
                   is_state,--报表类型
                   valid_state,--解锁状态
                   OPER_CODE, --操作人             
                   OPER_DATE, --操作时间
                   soid_id --序号
                   )
                VALUES
                  (TO_DATE('{0}', 'yyyy-mm-dd'), --时间
                   '{1}', --科室编码
                   '{2} ', --科室名称
                   '{3}', --医生名称
                   '{4}', --手术名称
                   '{5}', --手术例数
                   '1', --报表类型
                   '0', --解锁状态
                   '{6}', --操作人 
                   sysdate,--操作时间
                   '{7}')  --序号
                ";

            try
            {
                strSql = string.Format(strSql, nurseWork.Op_date, nurseWork.Dept_code, nurseWork.Dept_Name, nurseWork.Doc_name, nurseWork.Operation_Name, nurseWork.Operation, nurseWork.Oper_code, nurseWork.Soid_Id);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int InsertData1(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"
INSERT INTO FIN_COM_DATA --数据主表
  (DATA_DATE, --时间
   DATA_CODE, --编码
   DATA_NAME, --名称
   DATA_NUM, --数量
   DEPT_CODE,--科室编码
   OPER_CODE, --操作人             
   OPER_DATE, --操作时间
   VALID_STATE,--解锁状态
   IS_STATE,    --报表状态
   month_data   --月报数据名称
   )
VALUES
  (TO_DATE('{0}-01', 'yyyy-mm-dd'), --时间
   '{1}', --编码
   '{2}', --名称
   {3}, --数量
   '{4}', --科室编码 5
   '{5}', --操作人
   sysdate,
   '{6}',--解锁状态
   '{7}',--报表状态
   '{8}')  
";

            try
            {
                strSql = string.Format(strSql, nurseWork.Data_date, nurseWork.Data_code, nurseWork.Data_name, nurseWork.Data_num, nurseWork.Dept_code, nurseWork.Oper_code, nurseWork.Valid_state, nurseWork.Is_state, nurseWork.Month_data);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public DataSet GetItemName( string type,string deptCode)
        {
            string sql = @"   select h.name from fin_COM_dictionary h where h.type='{0}' and h.code='{1}' order by h.sort_id";
            DataSet ds = new DataSet();
            sql = string.Format(sql, type, deptCode);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public DataSet Getnum()
        {
            string sql = @"select 1 num from dual 
                        union all
                        select 2 num from dual
                        union all
                        select 3 num from dual
                        union all
                        select 4 num from dual
                        union all
                        select 5 num from dual
                        union all
                        select 6 num from dual
                        union all
                        select 7 num from dual
                        union all
                        select 8 num from dual
                        union all
                        select 9 num from dual
                        union all
                        select 10 num from dual
                        union all
                        select 11 num from dual
                        union all
                        select 12 num from dual
                        union all
                        select 13 num from dual
                        union all
                        select 14 num from dual
                        union all
                        select 15 num from dual
                        union all
                        select 16 num from dual
                        union all
                        select 17 num from dual
                        union all
                        select 18 num from dual
                        union all
                        select 19 num from dual
                        union all
                        select 20 num from dual
                        union all
                        select 21 num from dual
                        union all
                        select 22 num from dual
                        union all
                        select 23 num from dual
                        union all
                        select 24 num from dual
                        union all
                        select 25 num from dual
                        union all
                        select 26 num from dual
                        union all
                        select 27 num from dual
                        union all
                        select 28 num from dual
                        union all
                        select 29 num from dual
                        union all
                        select 30 num from dual";
            DataSet ds = new DataSet();
            sql = string.Format(sql);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public DataSet GetItemName1(string date)
        {
            string sql = @"
  SELECT substr(to_char(TO_DATE('{0}', 'yyyy-MM-DD'), 'yyyymm'),
                0,
                4) || '年' ||
         ltrim(to_char(TO_DATE('{0}', 'yyyy-MM-DD'), 'mm'), 0)||'月' asmonthlist
  
    FROM DUAL
";
            DataSet ds1 = new DataSet();
            sql = string.Format(sql, date);
            if (this.ExecQuery(sql, ref ds1) == -1)
            {
                return null;
            }
            return ds1;
        }

        public int UpdateDayMetCheck(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"update fin_com_data a
                            set a.valid_state='1',
                                a.check_code={0},
                                a.check_date=sysdate
                            where to_char(a.data_date,'yyyy-mm')='{1}'
                            and a.valid_state='0'
                            and a.is_state='0'
                            and a.dept_code='{2}'";
            try
            {
                strSql = string.Format(strSql, nurseWork.Oper_code, nurseWork.Data_date, nurseWork.Dept_code);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        public int UpdateDayMetCancelCheck(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"update fin_com_data a
                        set a.valid_state='0',
                            a.check_code=null,
                            a.check_date=null
                        where to_char(a.data_date,'yyyy-mm')='{0}'
                        and a.valid_state='1'
                        and a.is_state='0'
                        and a.dept_code='{1}'";
            try
            {
                strSql = string.Format(strSql, nurseWork.Data_date, nurseWork.Dept_code);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        public int UpdateYearMetCheck(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"update fin_com_data a
                            set a.valid_state='1',
                                a.check_code={0},
                                a.check_date=sysdate
                            where to_char(a.data_date,'yyyy-mm')='{1}'
                            and a.valid_state='0'
                            and a.is_state='1'
                            and a.dept_code='{2}'";
            try
            {
                strSql = string.Format(strSql, nurseWork.Oper_code, nurseWork.Data_date, nurseWork.Dept_code);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        public int UpdateYearMetCancelCheck(Neusoft.HISFC.Models.NuerseWork.NuerseWork nurseWork)
        {
            string strSql = @"update fin_com_data a
                        set a.valid_state='0',
                            a.check_code=null,
                            a.check_date=null
                        where to_char(a.data_date,'yyyy-mm')='{0}'
                        and a.valid_state='1'
                        and a.is_state='1'
                        and a.dept_code='{1}'";
            try
            {
                strSql = string.Format(strSql, nurseWork.Data_date, nurseWork.Dept_code);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        #region 通用

        public int InsertReportData(Neusoft.HISFC.Models.NuerseWork.ReportData data)
        {

            string Sqlstr = @"
                            insert into FIN_COM_REPORTDATA
                              (DEPT,
                               DATATYPENAME,
                               ITEM_NAME,
                               SORTID,
                               DATA_DATA,
                               DATA_DATE,
                               OPER_CODE,
                               OPER_DATE,
                               VALID,
                               DATA_CHECK,
                               CHECK_DATE,
                               CHECK_OPERCODE)
                            values
                              ('{0}',
                               '{1}',
                               '{2}',
                               {3},
                               '{4}',
                               to_date('{5}', 'yyyy-mm-dd'),
                               '{6}',
                               to_date('{7}', 'yyyy-mm-dd hh24:mi:ss'),
                               '{8}',
                               '{9}',
                               to_date('{10}', 'yyyy-mm-dd hh24:mi:ss'),
                               '{11}')

                            ";

            try
            {
                Sqlstr = string.Format(Sqlstr, GetParm(data));
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(Sqlstr);


        }

        public int UpdateReportData(Neusoft.HISFC.Models.NuerseWork.ReportData data)
        {

            string Sqlstr = @"
                            update FIN_COM_REPORTDATA p
                               set p.DATA_DATA = '{4}'
                             where p.DEPT = '{0}'
                               and p.DATATYPENAME = '{1}'
                               and p.ITEM_NAME = '{2}'
                               and p.SORTID = {3}
                               and p.DATA_DATE = to_date('{5}', 'yyyy-mm-dd')
                            ";

            try
            {
                Sqlstr = string.Format(Sqlstr, GetParm(data));
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(Sqlstr);


        }

        public string[] GetParm(Neusoft.HISFC.Models.NuerseWork.ReportData data)
        {
            string[] par = new string[] 
            {
                data.Dept,
                data.Datatypename,
                data.Item_name,
                data.Sort_id.ToString(),
                data.Data_data,
                data.Data_date.ToShortDateString(),
                data.Oper_code,
                data.Oper_date.ToString(),
                data.Valid,
                data.Check,
                data.Check_date.ToString(),
                data.Check_opercode
            };
            return par;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="DicType">字典类型</param>
        /// <param name="datatypename">报表名称</param>
        /// <returns></returns>
        public ArrayList GetData(string DicType, string datatypename, DateTime dt)
        {
            ArrayList al = new ArrayList();
            string Sqlstr = @"
                            select p.DEPT,
                                   p.DATATYPENAME,
                                   p.ITEM_NAME,
                                   p.SORTID,
                                   p.DATA_DATA,
                                   p.DATA_DATE,
                                   p.OPER_CODE,
                                   p.OPER_DATE,
                                   p.VALID,
                                   p.DATA_CHECK,
                                   p.CHECK_DATE,
                                   p.CHECK_OPERCODE
                              from FIN_COM_REPORTDATA p ,com_dictionary t
                            where t.type='{0}'
                            and t.code='{1}'
                            and p.data_date=to_date('{2}','yyyy-mm-dd')
                            and p.DEPT=t.input_code
                            and p.DATATYPENAME=t.code
                            order by p.SORTID
                            ";

            try
            {
                Sqlstr = string.Format(Sqlstr, DicType, datatypename, dt.ToShortDateString());
                if (this.ExecQuery(Sqlstr) == -1)
                    return null;
                Neusoft.HISFC.Models.NuerseWork.ReportData data = null;
                while (this.Reader.Read())
                {
                    data = new Neusoft.HISFC.Models.NuerseWork.ReportData();
                    data.Dept = this.Reader[0].ToString();
                    data.Datatypename = this.Reader[1].ToString();
                    data.Item_name = this.Reader[2].ToString();
                    data.Sort_id = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[3].ToString());
                    data.Data_data = this.Reader[4].ToString();
                    data.Data_date = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[5].ToString());
                    data.Check = this.Reader[9].ToString();
                    al.Add(data);
                }
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return null;
            }
            return al;
        }

        public int SaveCheck(string DicType, string datatypename, DateTime dt, string check_sta)
        {
            string Sqlstr = @"
                             update FIN_COM_REPORTDATA p
                                set p.data_check = '{3}'
                              where p.DEPT = (select u.input_code
                                                from com_dictionary u
                                               where u.type = '{0}'
                                                 and u.code = '{1}')
                                and p.DATATYPENAME = '{1}'
                                and p.DATA_DATE = to_date('{2}', 'yyyy-mm-dd')
                            ";

            try
            {
                Sqlstr = string.Format(Sqlstr, DicType, datatypename, dt.ToShortDateString(), check_sta);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(Sqlstr);
        }

        #endregion


        public DataSet GetObstetricsData(string date)
        {
            string sql = @"select 
SOID_ID,
LOCAL,
NON_LOCAL,
FULL_NAME,
ADMINISTRATIVEREGION,
FIRSTAPPLICATION,
GESTATIONAL,
NIPT,
FIRST_PREGNANCYTEST,
PREGNANCYTEST_RETEST,
HIV,
SYPHILIS,
HEPATITISB,
WHOLEBLOODCELLANALYSIS,
DOWNSYNDROME,
NT,
B_MODEULTRASONOGRAPHY,
BLOODSUGAR,
TEL
 from FIN_Obstetrics_DATA
 where OP_DATE =to_date( '{0}','yyyy-MM-dd')
                         order by SOID_ID
                    ";
            DataSet ds = new DataSet();
            sql = string.Format(sql, date);
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds;
        }

        public int UpdateOpsData(Neusoft.HISFC.Models.NuerseWork.ObstetricsData nurseWork)
        {
            string strSql = @"
            update  FIN_Obstetrics_DATA F
            set 
            f.LOCAL = '{0}',
            f.NON_LOCAL = '{1}',
            f.FULL_NAME = '{2}',
            f.ADMINISTRATIVEREGION = '{3}',
            f.FIRSTAPPLICATION = '{4}',
            f.GESTATIONAL = '{5}',
            f.NIPT = '{6}',
            f.FIRST_PREGNANCYTEST = '{7}',
            f.PREGNANCYTEST_RETEST = '{8}',
            f.HIV = '{9}',
            f.SYPHILIS = '{10}',
            f.HEPATITISB = '{11}',
            f.WHOLEBLOODCELLANALYSIS = '{12}',
            f.DOWNSYNDROME = '{13}',
            f.NT = '{14}',
            f.B_MODEULTRASONOGRAPHY = '{15}',
            f.BLOODSUGAR = '{16}',
            f.TEL = '{17}',
            f.OPER_CODE = '{18}',
            f.OPER_DATE = sysdate
            where
            f.SOID_ID  = '{19}'
            and f.OP_DATE = to_date( '{20}','yyyy-MM-dd')
            ";
            try
            {
                strSql = string.Format(strSql,
                    nurseWork.local,
                    nurseWork.non_local,
                    nurseWork.full_name,
                    nurseWork.administrativeregion,
                    nurseWork.firstapplication,
                    nurseWork.gestational,
                    nurseWork.nipt,
                    nurseWork.first_pregnancytest,
                    nurseWork.pregnancytest_retest,
                    nurseWork.hiv,
                    nurseWork.syphilis,
                    nurseWork.hepatitisb,
                    nurseWork.wholebloodcellanalysis,
                    nurseWork.downsyndrome,
                    nurseWork.nt,
                    nurseWork.b_modeultrasonography,
                    nurseWork.bloodsugar,
                    nurseWork.tel,
                    nurseWork.oper_code,

                    nurseWork.soid_id,
                    nurseWork.op_Date.ToString("yyyy-MM-dd")
                    );
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int InsertData(Neusoft.HISFC.Models.NuerseWork.ObstetricsData nurseWork)
        {
            string strSql = @"
           insert into FIN_Obstetrics_DATA(
LOCAL,
NON_LOCAL,
FULL_NAME,
ADMINISTRATIVEREGION,
FIRSTAPPLICATION,
GESTATIONAL,
NIPT,
FIRST_PREGNANCYTEST,
PREGNANCYTEST_RETEST,
HIV,
SYPHILIS,
HEPATITISB,
WHOLEBLOODCELLANALYSIS,
DOWNSYNDROME,
NT,
B_MODEULTRASONOGRAPHY,
BLOODSUGAR,
TEL,
OPER_CODE,
OPER_DATE,
OP_DATE,
SOID_ID
)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
 sysdate,
 to_date( '{19}','yyyy-MM-dd'),
'{20}'

)
            ";
            try
            {
                strSql = string.Format(strSql,
                    nurseWork.local,
                    nurseWork.non_local,
                    nurseWork.full_name,
                    nurseWork.administrativeregion,
                    nurseWork.firstapplication,
                    nurseWork.gestational,
                    nurseWork.nipt,
                    nurseWork.first_pregnancytest,
                    nurseWork.pregnancytest_retest,
                    nurseWork.hiv,
                    nurseWork.syphilis,
                    nurseWork.hepatitisb,
                    nurseWork.wholebloodcellanalysis,
                    nurseWork.downsyndrome,
                    nurseWork.nt,
                    nurseWork.b_modeultrasonography,
                    nurseWork.bloodsugar,
                    nurseWork.tel,
                    nurseWork.oper_code,
                    nurseWork.op_Date.ToString("yyyy-MM-dd"),
                    nurseWork.soid_id
                    );
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }

        public int Delete(Neusoft.HISFC.Models.NuerseWork.ObstetricsData nurseWork)
        {
            string strSql = @"
           delete FIN_Obstetrics_DATA
            where
            SOID_ID  = '{0}'
            and OP_DATE = to_date( '{1}','yyyy-MM-dd')
            ";
            try
            {
                strSql = string.Format(strSql,nurseWork.soid_id,
                    nurseWork.op_Date.ToString("yyyy-MM-dd")
                    
                    );
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            //执行SQL语句
            return this.ExecNoQuery(strSql);
        }
    }
}

