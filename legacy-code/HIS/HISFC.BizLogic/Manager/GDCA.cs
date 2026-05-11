using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizLogic.Manager
{
    public class GDCA : DataBase
    {
        /// <summary>
        /// 根据工号获取签名图片
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public byte[] SignImg(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return null;
               
                byte[] bs=null;
                string sql = @"select a.签章 from emr.view_empl_data a where a.人员编码='{0}' ";
                sql = string.Format(sql, userId);

                this.ExecQuery(sql);
                if (this.Reader.Read())
                    bs = this.Reader[0] as byte[];

                return bs;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed)
                    this.Reader.Close();
            }
        }
    }
}
