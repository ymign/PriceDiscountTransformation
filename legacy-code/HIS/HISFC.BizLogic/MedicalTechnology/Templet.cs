using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.MedicalTechnology
{
    /// <summary>
    /// 医技预约模板管理类
    /// </summary>
    public class Templet : Neusoft.FrameWork.Management.Database
    {
        public Templet() { }
        /// <summary>
        /// 登记一条医技排班模板
        /// </summary>
        /// <param name="templet"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.MedicalTechnology.Templet templet)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("MedicalTechnology.Templet.Insert", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql, templet.ID,
                    templet.ItemCode,
                    templet.ItemName,
                    templet.TypeCode,
                    templet.TypeName,
                    templet.DoctCode,
                    templet.DoctName,
                    templet.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    templet.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    templet.BookLimit,
                    templet.HostLimit,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(templet.IsStop),
                    templet.Remark,
                    templet.StopReason,
                    templet.OperCode,
                    templet.OperDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    (int)templet.Week,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(templet.IsValid)
                    );
            }
            catch (System.Exception ex)
            {
                this.Err = "[MedicalTechnology.Templet.Insert]格式不匹配!" + ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(sql);
        }
        /// <summary>
        /// 根据ID删除一条模板记录
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int Delete(string ID)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("MedicalTechnology.Templet.Delete", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, ID);
            }
            catch (Exception e)
            {
                this.Err = "[MedicalTechnology.Templet.Delete]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }
        #region 查询
        /// <summary>
        /// 根据星期和医技类型获取排班信息
        /// </summary>
        /// <param name="week"></param>
        /// <param name="TechType"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.MedicalTechnology.Templet> Query(DayOfWeek week, string ItemCode)
        {
            return Query("MedicalTechnology.Templet.Query.Where", (int)week, ItemCode);
        }
        /// <summary>
        /// 根据星期获取排班信息
        /// </summary>
        /// <param name="week"></param>
        /// <param name="TechType"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.MedicalTechnology.Templet> Query(DayOfWeek week)
        {
            return Query("MedicalTechnology.Templet.Query.Where", (int)week, "ALL");
        }
        #endregion

        /// <summary>
        /// 基础查询
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.MedicalTechnology.Templet> Query(string whereSql, params object[] args)
        {
            string sql = string.Empty;
            string where = string.Empty;
            string orderBy = string.Empty;
            if (this.Sql.GetCommonSql("MedicalTechnology.Templet.Query", ref sql) < 0)
            {
                this.Err = "没有找到索引为[MedicalTechnology.Templet.Query]的SQL";
                return null;
            }
            if (this.Sql.GetCommonSql(whereSql, ref where) < 0)
            {
                this.Err = "没有找到索引为[" + whereSql + "]的SQL";
                return null;
            }
            try
            {
                where = string.Format(where, args);
            }
            catch
            {
                this.Err = "[" + whereSql + "]格式不匹配";
                return null;
            }
            sql += " " + where;

            if (!sql.ToUpper().Contains("ORDER BY "))
                if (this.Sql.GetCommonSql("MedicalTechnology.Templet.OrderBy", ref orderBy) >= 0)
                    sql += orderBy;

            return this.QueryBase(sql);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.MedicalTechnology.Templet> QueryBase(string sql)
        {
            if (this.ExecQuery(sql) == -1) return null;

            List<Neusoft.HISFC.Models.MedicalTechnology.Templet> tempList = new List<Neusoft.HISFC.Models.MedicalTechnology.Templet>();

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.MedicalTechnology.Templet Templet = new Neusoft.HISFC.Models.MedicalTechnology.Templet();

                    Templet.ID = this.Reader[0].ToString();
                    Templet.ItemCode = this.Reader[1].ToString();
                    Templet.ItemName = this.Reader[2].ToString();
                    Templet.TypeCode = this.Reader[3].ToString();
                    Templet.TypeName = this.Reader[4].ToString();
                    Templet.DoctCode = this.Reader[5].ToString();
                    Templet.DoctName = this.Reader[6].ToString();
                    Templet.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[7].ToString());
                    Templet.EndTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[8].ToString());
                    Templet.BookLimit = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[9].ToString());
                    Templet.HostLimit = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[10].ToString());
                    Templet.IsStop = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[11].ToString());
                    Templet.Remark = this.Reader[12].ToString();
                    Templet.StopReason = this.Reader[13].ToString();
                    Templet.OperCode = this.Reader[14].ToString();
                    Templet.OperDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[15].ToString());
                    Templet.Week = (DayOfWeek)(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[16].ToString()));
                    Templet.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[17].ToString());
                    
                    tempList.Add(Templet);
                }

                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "查询排班模板信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return tempList;
        }
    }
}
