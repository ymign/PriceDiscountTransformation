using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizLogic.MedicalTechnology
{
    public class Arrange : Neusoft.FrameWork.Management.Database
    {
        public Arrange() { }
        #region 增
        /// <summary>
        /// 登记一条医技排班记录
        /// </summary>
        /// <param name="templet"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.MedicalTechnology.Arrange templet)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("MedicalTechnology.Arrange.Insert", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql,
                    templet.ID,
                    templet.SeeDate.ToString("yyyy-MM-dd"),
                    (int)templet.Week,
                    templet.ItemCode,
                    templet.ItemName,
                    templet.TypeCode,
                    templet.TypeName,
                    templet.DoctCode,
                    templet.DoctName,
                    templet.SeeDate.ToString("yyyy-MM-dd") + templet.BeginTime.ToString(" HH:mm:00"),
                    templet.SeeDate.ToString("yyyy-MM-dd") + templet.EndTime.ToString(" HH:mm:00"),
                    templet.BookLimit,
                    templet.HostLimit,
                    templet.BookNum,
                    templet.HostNum,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(templet.IsStop),
                    templet.StopReason,
                    templet.StopDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    templet.StopOper,
                    templet.OperCode,
                    templet.OperDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(templet.IsValid)
                    );
            }
            catch (System.Exception ex)
            {
                this.Err = "[MedicalTechnology.Arrange.Insert]格式不匹配!" + ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(sql);
        }
        #endregion

        #region 删
        /// <summary>
        /// 根据ID删除一条模板记录
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int Delete(string ID)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("MedicalTechnology.Arrange.Delete", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, ID);
            }
            catch (Exception e)
            {
                this.Err = "[MedicalTechnology.Arrange.Delete]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }
        #endregion

        #region 改
        /// <summary>
        /// 根据ID修改一条排班记录(已使用的)
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public int Update(Neusoft.HISFC.Models.MedicalTechnology.Arrange arrange)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("MedicalTechnology.Arrange.Update", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql,
                    arrange.ID,
                    arrange.SeeDate.ToString("yyyy-MM-dd"),
                    (int)arrange.Week,
                    arrange.ItemCode,
                    arrange.ItemName,
                    arrange.TypeCode,
                    arrange.TypeName,
                    arrange.DoctCode,
                    arrange.DoctName,
                    arrange.SeeDate.ToString("yyyy-MM-dd") + arrange.BeginTime.ToString(" HH:mm:00"),
                    arrange.SeeDate.ToString("yyyy-MM-dd") + arrange.EndTime.ToString(" HH:mm:00"),
                    arrange.BookLimit,
                    arrange.HostLimit,
                    arrange.BookNum,
                    arrange.HostNum,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(arrange.IsStop),
                    arrange.StopReason,
                    arrange.StopDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    arrange.StopOper,
                    arrange.OperCode,
                    arrange.OperDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(arrange.IsValid));

            }
            catch (Exception e)
            {
                this.Err = "[MedicalTechnology.Arrange.Update]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }


        /// <summary>
        /// 更新医技号源
        /// </summary>
        /// <param name="ID">排班ID</param>
        /// <param name="IsBook">是否预约</param>
        /// <param name="IsHost">是否住院预约</param>
        /// <returns></returns>
        public int Increase(string ID, bool IsBook, bool IsHost)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("MedicalTechnology.Arrange.Update.Increase", ref sql) == -1) return -1;
            int book = 0, host = 0;
            if (IsBook) book = 1;
            if (IsHost) host = 1;
            try
            {
                sql = string.Format(sql, ID, book, host);
            }
            catch (Exception e)
            {
                this.Err = "[MedicalTechnology.Arrange.Update.Increase]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
            return this.ExecNoQuery(sql);
        }
        /// <summary>
        /// 更新医技号源
        /// </summary>
        /// <param name="ID">排班ID</param>
        /// <param name="IsBook">是否预约</param>
        /// <param name="IsHost">是否住院预约</param>
        /// <returns></returns>
        public int Reduce(string ID, bool IsBook, bool IsHost)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("MedicalTechnology.Arrange.Update.Reduce", ref sql) == -1) return -1;
            int book = 0, host = 0;
            if (IsBook) book = 1;
            if (IsHost) host = 1;
            try
            {
                sql = string.Format(sql, ID, book, host);
            }
            catch (Exception e)
            {
                this.Err = "[MedicalTechnology.Arrange.Update.Reduce]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
            return this.ExecNoQuery(sql);
        }
        #endregion

        #region 查

        /// <summary>
        /// 根据项目代码,项目类型和日期查询
        /// </summary>
        /// <param name="ItemCode"></param>
        /// <param name="TypeCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.MedicalTechnology.Arrange> GetArrange(DateTime date,string ItemCode, string TypeCode)
        {
            return Query("MedicalTechnology.Arrange.Query.Where.ItemCodeAndTypeAndDate", date.ToString("yyyy-MM-dd"), ItemCode, TypeCode);
        }
        /// <summary>
        /// 根据日期和医技类型获取排班信息
        /// </summary>
        /// <param name="week"></param>
        /// <param name="TechType"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.MedicalTechnology.Arrange> Query(DateTime date, string ItemCode)
        {
            return Query("MedicalTechnology.Arrange.Query.Where.SeeDateAndItemCode", date.ToString("yyyy-MM-dd"), ItemCode);
        }
        /// <summary>
        /// 根据日期获取排班信息
        /// </summary>
        /// <param name="week"></param>
        /// <param name="TechType"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.MedicalTechnology.Arrange> Query(DateTime date)
        {
            return this.Query(date, "ALL");
        }
        /// <summary>
        /// 根据排班ID查询
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.MedicalTechnology.Arrange GetByID(string ID)
        {
            return Query("MedicalTechnology.Arrange.Query.Where.ID", ID).FirstOrDefault();
        }
        /// <summary>
        /// 根据ID检查该排班是否已使用
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int CheckIsExpiredByID(string ID)
        {
            string sql = string.Empty;
            if (Sql.GetCommonSql("MedicalTechnology.Arrange.CheckIsExpired", ref sql) == -1)
            {
                this.Err = "获得索引为[MedicalTechnology.Arrange.CheckIsExpired]的sql语句失败";
                return -1;
            }
            try
            {
                sql = string.Format(sql, ID);
            }
            catch (Exception ex)
            {
                this.Err = "[MedicalTechnology.Arrange.CheckIsExpired]格式不匹配!" + ex.Message;
                this.ErrCode = ex.Message;
                return -1;

            }
            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
        }

        /// <summary>
        /// 基础查询
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.MedicalTechnology.Arrange> Query(string whereSql, params object[] args)
        {
            string sql = string.Empty;
            string where = string.Empty;
            string orderBy = string.Empty;

            if (this.Sql.GetCommonSql("MedicalTechnology.Arrange.Query", ref sql) < 0)
            {
                this.Err = "没有找到索引为[MedicalTechnology.Arrange.Query]的SQL";
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
                if (this.Sql.GetCommonSql("MedicalTechnology.Arrange.OrderBy", ref orderBy) >= 0)
                    sql += orderBy;

            return this.QueryBase(sql);
        }
        /// <summary>
        /// 基础查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.MedicalTechnology.Arrange> QueryBase(string sql)
        {
            if (this.ExecQuery(sql) == -1) return null;

            List<Neusoft.HISFC.Models.MedicalTechnology.Arrange> tempList = new List<Neusoft.HISFC.Models.MedicalTechnology.Arrange>();

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.MedicalTechnology.Arrange Templet = new Neusoft.HISFC.Models.MedicalTechnology.Arrange();

                    Templet.ID = this.Reader[0].ToString();
                    Templet.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[1].ToString());
                    Templet.Week = (DayOfWeek)(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString()));
                    Templet.ItemCode = this.Reader[3].ToString();
                    Templet.ItemName = this.Reader[4].ToString();
                    Templet.TypeCode = this.Reader[5].ToString();
                    Templet.TypeName = this.Reader[6].ToString();
                    Templet.DoctCode = this.Reader[7].ToString();
                    Templet.DoctName = this.Reader[8].ToString();
                    Templet.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[9].ToString());
                    Templet.EndTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[10].ToString());
                    Templet.BookLimit = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[11].ToString());
                    Templet.HostLimit = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[12].ToString());
                    Templet.BookNum = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[13].ToString());
                    Templet.HostNum = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[14].ToString());
                    Templet.IsStop = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[15].ToString());
                    Templet.StopReason = this.Reader[16].ToString();
                    Templet.StopDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[17].ToString());
                    Templet.StopOper = this.Reader[18].ToString();
                    Templet.OperCode = this.Reader[19].ToString();
                    Templet.OperDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[20].ToString());
                    Templet.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[21].ToString());
                    tempList.Add(Templet);
                }

                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "查询排班信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return tempList;
        }
        #endregion

    }
}
