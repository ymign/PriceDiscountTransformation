using FS.ZDWY.Internet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BL
{
    public class FinComUndrugInfoLogic : SqlSugar.DbContext<FS.ZDWY.Internet.Models.FIN_COM_UNDRUGINFO>
    {
        #region 根据非药品编码获取对应有效的非药品项目信息
        /// <summary>
        /// 根据非药品编码获取对应有效的非药品项目信息
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public FIN_COM_UNDRUGINFO GetUnDrugEntityForItemCode(string itemCode)
        {
            FIN_COM_UNDRUGINFO model = new FIN_COM_UNDRUGINFO();
            try
            {
                var data = this.Db.Queryable<FIN_COM_UNDRUGINFO>().Single(p => p.ITEM_CODE == itemCode && p.VALID_STATE == "1");
                return data == null ? model : data;//保证不返回空对象 以免其他地方用到的时候报错误
            }
            catch (Exception ex)
            {
                
                return model;
            }
        }
        #endregion
    }
}
