using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Nesoft.EMPI.EMPIbusiness
{
    /// <summary>
    /// 数据库相关操作
    /// </summary>
    public class EMPIDBOperate : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 添加 
        /// </summary>
        /// <param name="empiNo">EMPI主索引</param>
        /// <param name="patId">病人编号</param>
        /// <returns></returns>
        public  bool Insert(string empiNo, string patId,string card_type)
        {
            //判断之前有没有相同的的数据
            string sql = string.Format(@" select card_no,empi_no from  empi_paitinetinfo where card_no='{0}' and CARD_TYPE='{1}'", patId,card_type);
            System.Data.DataSet ds = new System.Data.DataSet();
            this.ExecQuery(sql,ref ds);
            if (ds == null || ds.Tables.Count == 0)
            {
                return false;
            }
            DataTable dtTemp =ds.Tables[0];
            if (dtTemp.Rows.Count == 0)
            {
                sql = @" insert into empi_paitinetinfo
                                        (card_no,empi_no,oper_date,CARD_TYPE)
                                        values('{0}', '{1}',sysdate,'{2}')";
                sql = string.Format(sql, patId, empiNo,card_type);
                return ExecNoQuery(sql) > 0;
                
            }
            else
            {
                if (!dtTemp.Rows[0]["empi_no"].ToString().Equals(empiNo))  //不相同 就更新
                {
                    sql = @" update empi_paitinetinfo set empi_no='{0}',oper_date=sysdate where card_no='{1}' and  CARD_TYPE='{2}'";
                    sql = string.Format(sql, empiNo, patId,card_type);
                    return ExecNoQuery(sql) > 0;
                }
                return true;
            }
            return false;
        }
    }
}
