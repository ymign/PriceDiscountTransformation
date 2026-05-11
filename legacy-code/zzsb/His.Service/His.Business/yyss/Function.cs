using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.YYSS
{
    public class Function
    {
        /// <summary>
        /// 取服务器当前时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetSysDate()
        {
            string sql = Sql.Sql.GetSysDate;
            System.Data.DataTable dt = new System.Data.DataTable();
            DateTime now = new DateTime();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        now = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[0][0]);
                    }
                }
            }
            return now;
        }
    }
}
