using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BL.OutPatient
{
    public class FinOpbFeeDetailLogic : SqlSugar.DbContext<FS.ZDWY.Internet.Models.FIN_OPB_FEEDETAIL>
    {

        public string GetClincCode() 
        {
            return this.Db.Ado.GetString(" SELECT seq_fin_clinicno.nextval FROM dual ");
        }

        /// <summary>
        /// 获取处方号
        /// </summary>
        /// <returns></returns>
        public string GetRecipeNo() 
        {
            return this.Db.Ado.GetString(" select SEQ_OPB_RECIPE_NO.NEXTVAL from dual ");
        }

        public string GetCombono() 
        {
            return this.Db.Ado.GetString(" select SEQ_MET_ORDER_COMBONO.NEXTVAL from dual ");
        }

        public string GetMoOrder()
        {
            return this.Db.Ado.GetString(" select SEQ_MET_ORDER_ID.NEXTVAL from dual ");
        }

    }
}
