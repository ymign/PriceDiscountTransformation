using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Xml;



namespace Neusoft.HISFC.Components.Common.Classes
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlServerInterface : Neusoft.FrameWork.Management.Database
    {
        private SqlConnection conn = new SqlConnection();
        private SqlCommand cmd = new SqlCommand();
        private SqlTransaction transaction = null;
        private SqlDataReader reader;
        string err = "";
        /// <summary>
        /// 错误提示
        /// </summary>
        public string Err1
        {
            get
            {
                return err;
            }
            set
            {
                err = value;
            }
        }

        /// <summary>
        /// 中五项目获取望海条码信息
        /// </summary>
        public SqlServerInterface()
        {
            this.conn.ConnectionString = @"Data Source=<SQL_SERVER_HOST>;Initial Catalog=<DATABASE>;User ID=<USER>;Password=<PASSWORD>";
            this.conn.Open();
            this.transaction = this.conn.BeginTransaction();
            this.cmd.Connection = this.conn;
            this.cmd.Transaction = transaction;
            CreatFile();
        }
        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            this.transaction.Commit();
        }
        /// <summary>
        /// 回退
        /// </summary>
        public void Rollback()
        {
            this.transaction.Rollback();
        }
        /// <summary>
        /// 连接
        /// </summary>
        public SqlConnection Connection
        {
            get
            {
                this.IsOpen();
                return this.conn;
            }
        }
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            try
            {
                if (this.conn != null && this.conn.State == ConnectionState.Closed)
                {
                    this.conn.Open();
                    this.transaction = this.conn.BeginTransaction();
                    this.cmd.Connection = this.conn;
                    this.cmd.Transaction = this.transaction;
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        #region 写日志

        private string fileName = "./SQLServerTEST.log";

        private void CreatFile()
        {
            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.CreateText(fileName);
            }
        }
        private System.IO.TextWriter output;

        private void WriteLog(string log)
        {
            try
            {
                output = System.IO.File.AppendText(fileName);
                output.WriteLine(System.DateTime.Now + "\n" + log);
                output.Close();
            }
            catch
            {
                //System.w
            }
        }

        private void ReadSQL(string sql)
        {
            this.WriteLog(sql);
            this.cmd.CommandText = sql;
        }

        #endregion
       
        /// <summary>
        /// 根据条码从herp获取代销材料标准码
        /// </summary>
        /// <param name="barCode">条码号</param>
        /// <returns></returns>
        public Neusoft.FrameWork.Models.NeuObject GetItembyBarCode(string barCode)
        {
            string strSQL = @" select inv_code,inv_name,bar_code,inv_model,plan_price from mate_bar_view where bar_code='{0}' ";
            this.cmd.CommandText = string.Empty;
            try
            {
                strSQL = string.Format(strSQL, barCode);
            }
            catch (Exception ex)
            {
                return null;
            }
            ReadSQL(strSQL);
            Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
            this.reader = this.cmd.ExecuteReader();
            try
            {
                while (this.reader.Read())
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = this.reader[0].ToString();//物价码
                    obj.Name = this.reader[1].ToString();//物价名称
                    obj.Memo = this.reader[2].ToString();//条码
                    obj.User01 = this.reader[3].ToString();
                    obj.User02 = this.reader[4].ToString();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.reader.Close();
            }
            return obj;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql, ref DataSet ds)
        {
            try
            {
                //DataSet ds = new DataSet();
                this.cmd.CommandText = string.Empty;
                ReadSQL(sql);
                SqlDataAdapter datareader = new SqlDataAdapter();
                datareader.SelectCommand = this.cmd;
                datareader.Fill(ds);
                return 1;
            }
            catch (Exception ex)
            {
                this.Err = ex.ToString();
                return -1;
            }
        }
    }
}