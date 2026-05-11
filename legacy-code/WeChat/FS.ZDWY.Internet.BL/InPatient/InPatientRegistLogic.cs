using FS.ZDWY.Internet.Models;
using System;
using System.Collections.Generic;

namespace FS.ZDWY.Internet.BL.InPatient
{
    public class InPatientRegistLogic : SqlSugar.DbContext<InPatientRegistInfo>
    {
        public int InPatientSave(InPatientRegistInfo inPatient)
        {
            inPatient.OPERDATE = DateTime.Now;
            int count = 0;
            Db.Deleteable<InPatientRegistInfo>().Where(q => q.CardNO == inPatient.CardNO && (q.ID == null || q.ID == "")).ExecuteCommand();
            count = Db.Insertable(inPatient).ExecuteCommand();
            return count;
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="CardNO"></param>
        /// <returns></returns>
        public InPatientRegistInfo GetInPatientRegistInfo(string CardNO)
        {
            var queryData = Db.Queryable<InPatientRegistInfo>().First(q => q.CardNO == CardNO && (q.ID == null || q.ID == ""));
            return queryData;
        }
    }
}
