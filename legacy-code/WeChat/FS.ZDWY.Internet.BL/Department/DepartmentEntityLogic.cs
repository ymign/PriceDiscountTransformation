using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using FS.ZDWY.Internet.Models;
using System.Data;

namespace FS.ZDWY.Internet.BL
{
    public class DepartmentEntityLogic : SqlSugar.DbContext<COM_DEPARTMENT>
    {
        public List<DepartmentEntity> DepartmentDataContains(string deptCode, string rank)
        {
            string sql = string.Empty;
            if (string.IsNullOrEmpty(deptCode))
            {
                deptCode = "ALL";
            }
            if (rank == "1")
            {
                #region 原有屏蔽
                //                sql = @"SELECT DISTINCT COM_DEPARTMENT.bro_id DEPT_CODE,COM_DEPARTMENT.Bro_Name DEPT_NAME,
                //1  AS STATUS,
                //'是' HSACHILD,
                //'' PARENTDEPTCODE
                //from COM_DEPARTMENT 
                //WHERE COM_DEPARTMENT.BRO_ID = :deptCode";
                sql = @"SELECT DISTINCT COM_DEPARTMENT.bro_id DEPT_CODE,COM_DEPARTMENT.Bro_Name DEPT_NAME,
1  AS STATUS,
'是' HSACHILD,
'' PARENTDEPTCODE
from COM_DEPARTMENT,FIN_OPR_SCHEMA 
WHERE COM_DEPARTMENT.BRO_ID = :deptCode
  AND COM_DEPARTMENT.DEPT_CODE = FIN_OPR_SCHEMA.DEPT_CODE
  AND FIN_OPR_SCHEMA.BEGIN_TIME >= TRUNC(SYSDATE)
  AND FIN_OPR_SCHEMA.TEL_LMT > 0";
                #endregion
            }
            else if (rank == "2")
            {
                #region 原有屏蔽
                //                sql = @"SELECT DEPT_CODE,DEPT_NAME,
                //nvl((SELECT 0 from  FIN_OPR_SCHEMA WHERE  COM_DEPARTMENT.DEPT_CODE = FIN_OPR_SCHEMA.DEPT_CODE AND Rownum<2),1)  AS STATUS,
                //'否' HSACHILD,
                //to_char(COM_DEPARTMENT.bro_id) PARENTDEPTCODE
                //from COM_DEPARTMENT 
                //WHERE  COM_DEPARTMENT.DEPT_CODE = :deptCode
                //union all
                //SELECT DISTINCT to_char(COM_DEPARTMENT.bro_id) DEPT_CODE,COM_DEPARTMENT.Bro_Name DEPT_NAME,
                //1  AS STATUS,
                //'是' HSACHILD,
                //'' PARENTDEPTCODE
                //from COM_DEPARTMENT 
                //WHERE BRO_ID  = :deptCode
                //";

                #endregion

                sql = @"SELECT DEPT_CODE,DEPT_NAME,
nvl((SELECT 0 from  FIN_OPR_SCHEMA WHERE  COM_DEPARTMENT.DEPT_CODE = FIN_OPR_SCHEMA.DEPT_CODE AND Rownum<2),1)  AS STATUS,
'否' HSACHILD,
to_char(COM_DEPARTMENT.bro_id) PARENTDEPTCODE
from COM_DEPARTMENT 
WHERE  COM_DEPARTMENT.DEPT_CODE = :deptCode
union all
SELECT DISTINCT to_char(AA.bro_id) DEPT_CODE,AA.Bro_Name DEPT_NAME,
1  AS STATUS,
'是' HSACHILD,
'' PARENTDEPTCODE
from COM_DEPARTMENT AA,FIN_OPR_SCHEMA BB
WHERE AA.DEPT_CODE  = :deptCode AND AA.DEPT_CODE = BB.DEPT_CODE 
AND BB.BEGIN_TIME >= TRUNC(SYSDATE)
AND BB.REG_LMT > 0
";
            }
            else if (rank == "3")
            {
                sql = @"SELECT DEPT_CODE,DEPT_NAME,
nvl((SELECT 0 from  FIN_OPR_SCHEMA WHERE  COM_DEPARTMENT.DEPT_CODE = FIN_OPR_SCHEMA.DEPT_CODE AND Rownum<2),1)  AS STATUS,
'否' HSACHILD,
to_char(COM_DEPARTMENT.bro_id) PARENTDEPTCODE
from COM_DEPARTMENT 
WHERE  COM_DEPARTMENT.DEPT_CODE = :deptCode";
            }
            var queryData = Db.Ado.SqlQuery<DepartmentEntity>(sql, new List<SugarParameter>(){
                          new SugarParameter(":deptCode",deptCode)
                        });
            return queryData;
        }

        public List<DepartmentEntity> DepartmentData(string rank)
        {
            #region 原sql屏蔽
            //            string sql = @"SELECT DEPT_CODE,DEPT_NAME,
            //nvl((SELECT 0 from  FIN_OPR_SCHEMA WHERE  COM_DEPARTMENT.DEPT_CODE = FIN_OPR_SCHEMA.DEPT_CODE AND Rownum<2),1)  AS STATUS,
            //'否' HSACHILD,
            //to_char(COM_DEPARTMENT.bro_id) PARENTDEPTCODE
            //from COM_DEPARTMENT 
            //WHERE DEPT_TYPE in('C','I') AND BRO_ID IS NOT NULL and ('2'=:rank or 'ALL'=:rank)
            //union all
            //SELECT DISTINCT to_char(COM_DEPARTMENT.bro_id) DEPT_CODE,COM_DEPARTMENT.Bro_Name DEPT_NAME,
            //1  AS STATUS,
            //'是' HSACHILD,
            //'' PARENTDEPTCODE
            //from COM_DEPARTMENT 
            //WHERE BRO_ID IS NOT NULL AND  DEPT_TYPE in('C','I') and ('1'=:rank or 'ALL'=:rank)
            //";
            #endregion

            string sql = @"SELECT AA.DEPT_CODE,AA.DEPT_NAME,
nvl((SELECT 0 from  FIN_OPR_SCHEMA WHERE  AA.DEPT_CODE = FIN_OPR_SCHEMA.DEPT_CODE AND Rownum<2),1)  AS STATUS,
'否' HSACHILD,
to_char(AA.bro_id) PARENTDEPTCODE
from COM_DEPARTMENT AA,FIN_OPR_SCHEMA BB
WHERE AA.DEPT_TYPE in('C','I') AND AA.BRO_ID IS NOT NULL and ('2'=:rank or 'ALL'=:rank)
AND   AA.DEPT_CODE = BB.DEPT_CODE
AND   BB.BEGIN_TIME >= TRUNC(SYSDATE)
AND   BB.TEL_LMT > 0
union all
SELECT DISTINCT to_char(COM_DEPARTMENT.bro_id) DEPT_CODE,COM_DEPARTMENT.Bro_Name DEPT_NAME,
1  AS STATUS,
'是' HSACHILD,
'' PARENTDEPTCODE
from COM_DEPARTMENT  
WHERE BRO_ID IS NOT NULL AND  DEPT_TYPE in('C','I') and ('1'=:rank or 'ALL'=:rank)";

            var queryData = Db.Ado.SqlQuery<DepartmentEntity>(sql, new List<SugarParameter>(){
                          new SugarParameter(":rank",rank)
                        });
            return queryData;
        }
    }
}
