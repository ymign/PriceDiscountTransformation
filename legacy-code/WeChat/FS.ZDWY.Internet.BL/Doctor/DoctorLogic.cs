using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Data;

namespace FS.ZDWY.Internet.BL.Doctor
{
    public class DoctorLogic : SqlSugar.DbContext<FS.ZDWY.Internet.Models.Doctor.COM_EMPLOYEE>
    {

        public DataTable QueryDoctorListAll()//科室代码为空，医生代码为空
        {
            #region 原sql屏蔽

            //            string sql = @"Select e.dept_code ""deptCode"" --科室代码 必填
            //            ,fun_get_dept_name(e.dept_code) ""deptName"" --科室名称 必填
            //            , e.empl_code ""doctorCode""  --医生代码 必填
            //            , e.empl_name ""doctorName"" --医生名称 必填
            //            , '' ""doctorSkill"" --医生擅长 
            //            , '' ""doctorIntrodution""  -- 医生介绍 
            //            , e.levl_code ""techTitle"" --医生职称 必填  
            //            , e.posi_code ""rank"" --医生职级 必填  
            //            , e.idenno ""idCard"" --医生身份证号 
            //            , '' ""doctorPicture"" --医生图片 
            //            , '' ""mobile"" --手机号码 
            //            , '' ""telephone"" --办公室号码 
            //            ,nvl((SELECT 0 FROM FIN_OPR_SCHEMA f WHERE f.doct_code = e.empl_code AND Rownum<2),1) AS ""status"" --是否预约医生 必填
            //            FROM COM_EMPLOYEE e  
            //            WHERE  e.empl_type = 'D'";

            #endregion

            string sql = @"Select     DISTINCT  BB.DEPT_CODE ""deptCode"", --科室代码 必填
            fun_get_dept_name(BB.DEPT_CODE) ""deptName"", --科室名称 必填
            BB.DOCT_CODE ""doctorCode"",  --医生代码 必填
            fun_get_employee_name(BB.DOCT_CODE)  ""doctorName"",  --医生名称 必填
            '' ""doctorSkill"", --医生擅长 
            '' ""doctorIntrodution"",  -- 医生介绍 
            e.levl_code ""techTitle"", --医生职称 必填  
            e.posi_code ""rank"", --医生职级 必填  
            e.idenno ""idCard"", --医生身份证号 
            '' ""doctorPicture"", --医生图片 
            '' ""mobile"",  --手机号码 
            '' ""telephone"", --办公室号码 
            '1' ""status"" --是否预约医生 必填
            FROM COM_EMPLOYEE e,FIN_OPR_SCHEMA BB  
            WHERE  e.empl_type = 'D'
              AND E.EMPL_CODE = BB.DOCT_CODE
              AND BB.BEGIN_TIME >= TRUNC(SYSDATE)
              AND BB.TEL_LMT > 0";

            var queryData = Db.Ado.GetDataTable(sql);
            return queryData;

        }
        public DataTable QueryDoctorListDept(string deptCode)//科室代码不为空，医生代码为空
        {
            #region 原sql屏蔽

            //            string sql = @"Select e.dept_code ""deptCode"" --科室代码 必填
            //            ,fun_get_dept_name(e.dept_code) ""deptName"" --科室名称 必填
            //            , e.empl_code ""doctorCode""  --医生代码 必填
            //            , e.empl_name ""doctorName"" --医生名称 必填
            //            , '' ""doctorSkill"" --医生擅长 
            //            , '' ""doctorIntrodution""  -- 医生介绍 
            //            , e.levl_code ""techTitle"" --医生职称 必填  
            //            , e.posi_code ""rank"" --医生职级 必填  
            //            , e.idenno ""idCard"" --医生身份证号 
            //            , '' ""doctorPicture"" --医生图片 
            //            , '' ""mobile"" --手机号码 
            //            , '' ""telephone"" --办公室号码 
            //            ,nvl((SELECT 0 FROM FIN_OPR_SCHEMA f WHERE f.doct_code = e.empl_code AND Rownum<2),1) AS ""status"" --是否预约医生 必填
            //            FROM COM_EMPLOYEE e  
            //            WHERE  e.empl_type = 'D'
            //            and e.dept_code=:deptCode";

            #endregion

            string sql = @"Select   DISTINCT   B.dept_code ""deptCode"", --科室代码 必填
            fun_get_dept_name(B.dept_code) ""deptName"", --科室名称 必填
            e.empl_code ""doctorCode"",  --医生代码 必填
            e.empl_name ""doctorName"", --医生名称 必填
            ''  ""doctorSkill"", --医生擅长 
            '' ""doctorIntrodution"",  -- 医生介绍 
            e.levl_code ""techTitle"", --医生职称 必填  
            e.posi_code ""rank"", --医生职级 必填  
            e.idenno ""idCard"", --医生身份证号 
            '' ""doctorPicture"", --医生图片 
            '' ""mobile"", --手机号码 
            '' ""telephone"", --办公室号码 
            '1' ""status"" --是否预约医生 必填
            FROM COM_EMPLOYEE e,FIN_OPR_SCHEMA B  
            WHERE  e.empl_type = 'D'
            AND E.EMPL_CODE = B.DOCT_CODE
            AND B.BEGIN_TIME >= TRUNC(SYSDATE)
            AND B.TEL_LMT > 0
            and B.dept_code=:deptCode";

            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                          new SugarParameter(":deptCode",deptCode),
                        });
            return queryData;

        }
        public DataTable QueryDoctorList(string deptCode, string doctorCode)//科室代码不为空，医生代码不为空
        {
            #region 原sql屏蔽

            //            string sql = @"Select e.dept_code ""deptCode"" --科室代码 必填
            //            ,fun_get_dept_name(e.dept_code) ""deptName"" --科室名称 必填
            //            , e.empl_code ""doctorCode""  --医生代码 必填
            //            , e.empl_name ""doctorName"" --医生名称 必填
            //            , '' ""doctorSkill"" --医生擅长 
            //            , '' ""doctorIntrodution""  -- 医生介绍 
            //            , e.levl_code ""techTitle"" --医生职称 必填  
            //            , e.posi_code ""rank"" --医生职级 必填  
            //            , e.idenno ""idCard"" --医生身份证号 
            //            , '' ""doctorPicture"" --医生图片 
            //            , '' ""mobile"" --手机号码 
            //            , '' ""telephone"" --办公室号码 
            //            ,nvl((SELECT 0 FROM FIN_OPR_SCHEMA f WHERE f.doct_code = e.empl_code AND Rownum<2),1) AS ""status"" --是否预约医生 必填
            //            FROM COM_EMPLOYEE e  
            //            WHERE  e.empl_type = 'D'
            //            and (e.dept_code=:deptCode or 'ALL'=:deptCode)
            //            and (e.empl_code=:doctorCode or 'ALL'=:doctorCode)";

            #endregion

            string sql = @"Select   DISTINCT   B.dept_code ""deptCode"", --科室代码 必填
            fun_get_dept_name(B.dept_code) ""deptName"", --科室名称 必填
            e.empl_code ""doctorCode"",  --医生代码 必填
            e.empl_name ""doctorName"", --医生名称 必填
            ''  ""doctorSkill"", --医生擅长 
            '' ""doctorIntrodution"",  -- 医生介绍 
            e.levl_code ""techTitle"", --医生职称 必填  
            e.posi_code ""rank"", --医生职级 必填  
            e.idenno ""idCard"", --医生身份证号 
            '' ""doctorPicture"", --医生图片 
            '' ""mobile"", --手机号码 
            '' ""telephone"", --办公室号码 
            '1' ""status"" --是否预约医生 必填
            FROM COM_EMPLOYEE e,FIN_OPR_SCHEMA B  
            WHERE  e.empl_type = 'D'
            AND E.EMPL_CODE = B.DOCT_CODE
            AND B.BEGIN_TIME >= TRUNC(SYSDATE)
            AND B.TEL_LMT > 0
            AND (B.dept_code=:deptCode or 'ALL'=:deptCode)
            AND (B.Doct_Code=:doctorCode or 'ALL'=:doctorCode)";

            var queryData = Db.Ado.GetDataTable(sql, new List<SugarParameter>(){
                          new SugarParameter(":deptCode",string.IsNullOrEmpty(deptCode)?"ALL":deptCode),
                          new SugarParameter(":doctorCode",string.IsNullOrEmpty(doctorCode)?"ALL":doctorCode)
                        });
            return queryData;
        }
    }
}
