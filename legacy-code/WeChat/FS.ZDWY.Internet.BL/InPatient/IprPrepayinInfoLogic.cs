using FS.ZDWY.Internet.Models;
using System;
using System.Collections.Generic;

namespace FS.ZDWY.Internet.BL.InPatient
{
    public class IprPrepayinInfoLogic : SqlSugar.DbContext<IprPrepayinInfo>
    {
        public List<IprPrepayinInfo> GetIprPrepayinInfo(string deptCode,DateTime date)
        {
            var queryData = Db.Queryable<IprPrepayinInfo>().Where(q => q.CARD_NO == deptCode && q.PRE_DATE >= date && q.PRE_STATE == "0").ToList();
            return queryData;
        }
    }
}
