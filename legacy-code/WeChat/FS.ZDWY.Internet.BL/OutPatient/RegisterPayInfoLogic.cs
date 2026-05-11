using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Data;

namespace FS.ZDWY.Internet.BL.OutPatient
{
    public class RegisterPayInfoLogic : SqlSugar.DbContext<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY>
    {
        public List<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY> GetRefundList()
        {
            string sql = @"SELECT pay.* 
FROM fin_opr_register reg
INNER JOIN platform_register_order ord ON ord.registerid=reg.clinic_code AND (ord.status='2' or ord.status='6')
INNER JOIN platform_register_pay pay ON pay.orderid=ord.orderid
WHERE reg.valid_flag='0'
AND reg.trans_type='2'
AND (pay.refundreason IS NULL OR nvl(pay.refundreason,' ')=' ')";
            List<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY> queryData = Db.Ado.SqlQuery<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY>(sql);
            if(queryData!=null || queryData.Count>0)
            {
                return queryData;
            }
            else
            {
                return null;
            }
        }

        public FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY GetRefundPayInfoByClinicCode(string ClinicCode)
        {
            string sql = @"SELECT pay.* 
FROM fin_opr_register reg
INNER JOIN platform_register_order ord ON ord.registerid=reg.clinic_code AND (ord.status='2' or ord.status='6')
INNER JOIN platform_register_pay pay ON pay.orderid=ord.orderid
WHERE reg.valid_flag='0'
AND reg.trans_type='2'
AND (pay.refundreason IS NULL OR nvl(pay.refundreason,' ')=' ')
and reg.clinic_code = '"+ClinicCode+"'";
            List<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY> queryData = Db.Ado.SqlQuery<FS.ZDWY.Internet.Models.PLATFORM_REGISTER_PAY>(sql);
            if (queryData != null || queryData.Count > 0)
            {
                return queryData[0];
            }
            else
            {
                return null;
            }
        }
    }
}
