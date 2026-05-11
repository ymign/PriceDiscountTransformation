using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizLogic.RADT
{
    /// [功能描述: 终止妊娠登记管理数据访问层]<br></br>
    /// [创 建 者: zhaoj]<br></br>
    /// [创建时间: 2012-2-10]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    public class StopPregnancy:Neusoft.FrameWork.Management.Database
    {
        /// <终止妊娠登记管理插入方法>
        /// <returns>-1 失败 1成功</returns>
        public int Insert(Neusoft.HISFC.Models.RADT.StopPregnancy stopPregnancy)
        {
            return this.UpdateSingleTable("RADT.Inpatient.StopPregnancy.Insert", stopPregnancy.ID, stopPregnancy.Weeks.ToString(), stopPregnancy.Area.ID, stopPregnancy.RegDate.ToString("yyyy-MM-dd HH:mm:ss"), stopPregnancy.Oper.ID);
        }

        /// <终止妊娠登记管理更新方法>
        /// <returns></returns>
        public int Update(Neusoft.HISFC.Models.RADT.StopPregnancy stopPregnancy)
        {
            return this.UpdateSingleTable("RADT.Inpatient.StopPregnancy.Update",stopPregnancy.ID, stopPregnancy.Weeks.ToString(), stopPregnancy.Area.ID, stopPregnancy.RegDate.ToString("yyyy-MM-dd HH:mm:ss"), stopPregnancy.Oper.ID);
        }

        /// <终止妊娠登记管理删除方法>
        /// 根据住院流水号删除
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public int Delete(string inpatientNO)
        {
            return this.UpdateSingleTable("RADT.Inpatient.StopPregnancy.Delete", inpatientNO);
        }

        /// <终止妊娠登记管理获取数据方法>
        /// 根据住院流水号获取
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.RADT.StopPregnancy Get(string inpatientNO)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("RADT.Inpatient.StopPregnancy.Get", ref strSql) < 0)
            {
                this.Err = "未找到SQL语句：RADT.Inpatient.StopPregnancy.Get，"+this.Sql.Err;
                return null;
            }

            strSql = string.Format(strSql, inpatientNO);
            if (this.ExecQuery(strSql) < 0)
            {
                this.Err = "执行SQL语句：RADT.Inpatient.StopPregnancy.Get失败，"+this.Err;
                return null;
            }
            if (this.Reader != null)
            {
                if (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.RADT.StopPregnancy stopPregnancy = new Neusoft.HISFC.Models.RADT.StopPregnancy();
                    stopPregnancy.ID = this.Reader.GetString(0);
                    stopPregnancy.Weeks = this.Reader.GetInt32(1);
                    stopPregnancy.Area.ID = this.Reader.GetString(2);
                    stopPregnancy.RegDate = this.Reader.GetDateTime(3);
                    stopPregnancy.Oper.ID = this.Reader.GetString(4);
                    stopPregnancy.Oper.OperTime = this.Reader.GetDateTime(5);

                    return stopPregnancy;
                }
            }
            return null;
        }
    }
}
