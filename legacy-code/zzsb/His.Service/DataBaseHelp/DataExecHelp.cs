using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;


namespace DataBaseHelp
{
    /// <summary>
    /// DESC:底层数据访问公共类
    /// Creater;杨明
    /// Version：1.0.0.1
    /// Date:2015-04-08
    /// Alter:2015-06-28 修改说明：底层log4net 与 his 的log有冲突，修正使用his.util.common.hislog替代
    /// </summary>
    public class DataExecHelp
    {
       private const string logName = "Error";
      //  private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 执行sql语句  事务原子化
        /// </summary>
        /// <param name="sqllist"></param>
        /// <returns></returns>
        public static bool ExecArrayList(ArrayList sqllist)
        {
            bool bz = false;

            using (System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection(IBatisDbHelper.ConnectionString))
            {
                conn.Open();
                using (System.Data.OracleClient.OracleTransaction t = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (string sql in sqllist)
                        {
                            OracleCommand cmd = new OracleCommand(sql, conn, t);
                            cmd.CommandType = CommandType.Text;
                            int result = cmd.ExecuteNonQuery();
                        }
                        t.Commit();
                        bz = true;
                    }
                    catch (Exception ex)
                    {
                        His.Util.Common.HisLog.WriteLog(logName, ex.Message);
                      //  log.Debug(ex.Message, ex);
                        t.Rollback();
                        bz = false;
                    }
                    finally
                    {
                        t.Dispose();
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }

            }
            return bz;
        }

        /// <summary>
        /// 通过语句获取查询结果
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDataTable(string sqlstr)
        {
            try
            {
                DataSet ds = IBatisDbHelper.ExecuteDataset(CommandType.Text, sqlstr);
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                His.Util.Common.HisLog.WriteLog(logName, ex.Message);
                //log.Debug(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// 通过语句获取查询结果
        /// </summary>
        /// <returns></returns>

        public static DataSet GetDataSet(string sqlstr)
        {
            try
            {
                DataSet ds = IBatisDbHelper.ExecuteDataset(CommandType.Text, sqlstr);
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                His.Util.Common.HisLog.WriteLog(logName, ex.Message);
                //log.Debug(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// 执行单条sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool ExecSql(string sql, ref string errMsg)
        {
            bool bz = false;

            using (System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection(IBatisDbHelper.ConnectionString))
            {
                conn.Open();
                using (System.Data.OracleClient.OracleTransaction t = conn.BeginTransaction())
                {
                    try
                    {
                        OracleCommand cmd = new OracleCommand(sql, conn, t);
                        cmd.CommandType = CommandType.Text;
                        int result = cmd.ExecuteNonQuery();
                        t.Commit();
                        bz = true;
                    }
                    catch (Exception ex)
                    {
                        His.Util.Common.HisLog.WriteLog(logName, ex.Message);
                        //log.Debug(ex.Message, ex);
                        errMsg = ex.Message;
                        t.Rollback();
                        bz = false;
                    }
                    finally
                    {
                        t.Dispose();
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }

            }
            return bz;
        }


        /// <summary>
        /// 执行单条sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static int ExecuteSql(string sql, ref string errMsg)
        {
          //  bool bz = false;
            int retResult = -1;
            using (System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection(IBatisDbHelper.ConnectionString))
            {
                conn.Open();
                using (System.Data.OracleClient.OracleTransaction t = conn.BeginTransaction())
                {
                    try
                    {
                        OracleCommand cmd = new OracleCommand(sql, conn, t);
                        cmd.CommandType = CommandType.Text;
                        retResult = cmd.ExecuteNonQuery();
                        t.Commit();
                        //bz = true;
                    }
                    catch (Exception ex)
                    {
                        His.Util.Common.HisLog.WriteLog(logName, ex.Message);
                        //log.Debug(ex.Message, ex);
                        errMsg = ex.Message;
                        t.Rollback();
                        retResult = -1;
                    }
                    finally
                    {
                        t.Dispose();
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }

            }
            return retResult;
        }

        /// <summary>
        /// 执行两条插入sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool ExecSql(string sql1, string sql2, ref string errMsg)
        {
            bool bz = false;

            using (System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection(IBatisDbHelper.ConnectionString))
            {
                conn.Open();
                using (System.Data.OracleClient.OracleTransaction t = conn.BeginTransaction())
                {
                    try
                    {
                        OracleCommand cmd1 = new OracleCommand(sql1, conn, t);
                        cmd1.CommandType = CommandType.Text;
                        int result = cmd1.ExecuteNonQuery();
                        OracleCommand cmd2 = new OracleCommand(sql2, conn, t);
                        cmd2.CommandType = CommandType.Text;
                        result = cmd2.ExecuteNonQuery();
                        t.Commit();
                        bz = true;
                    }
                    catch (Exception ex)
                    {
                        His.Util.Common.HisLog.WriteLog(logName, ex.Message);
                        //log.Debug(ex.Message, ex);
                        errMsg = ex.Message;
                        t.Rollback();
                        bz = false;
                    }
                    finally
                    {
                        t.Dispose();
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }

            }
            return bz;
        }

    }
}