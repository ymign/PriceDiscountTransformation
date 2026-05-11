using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.Test
{
   public class TestTrans:Shadow.Util.Data.Management.OracleBase
    {
       public int UpdateReged(string regid)
       {
           
         string sql =Sql.Sql.UpdateSchemaReged;
         sql = string.Format(sql, regid,1);
         Shadow.Util.Data.Management.Trans.BeginTransaction();

         if (this.UpdateDic("REGLOCK", "ZZSB0001") != 1)
         {
             Shadow.Util.Data.Management.Trans.RollBack();
             return -1;
         }
         if (this.ExecNoQuery(sql)!=1)
         {
             Shadow.Util.Data.Management.Trans.RollBack();
             return -1;
         }

           
         Shadow.Util.Data.Management.Trans.Commit();

         return 1;
       }
     /*   */
       public int UpdateDic(string type, string code)
       {
           string sql = @"update com_dictionary a
                                       set a.name = 'Test'
                                     where upper(a.type) = '{0}'
                                       and a.code = '{1}'";
           return this.ExecNoQuery(string.Format(sql,type,code));
       }
    }
}
