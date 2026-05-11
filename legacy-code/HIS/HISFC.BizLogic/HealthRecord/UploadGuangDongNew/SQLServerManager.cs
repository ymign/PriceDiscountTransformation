using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.IO;

namespace Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDongNew
{
    public class SQLServerManager : Neusoft.FrameWork.Management.Database
    {
        #region 数据库
        private SqlConnection conn = new SqlConnection();
        private SqlCommand cmd = new SqlCommand();
        private SqlTransaction transaction = null;
        private SqlDataReader reader;
        private SqlDataAdapter datareader = new SqlDataAdapter();
        private string profileName = System.Windows.Forms.Application.StartupPath + @".\Profile\CaseDataBase.xml";//病案数据库连接设置;

        #region 链接
        /// <summary>
        /// sqlserver创建链接
        /// </summary>
        public SQLServerManager()
        {
            this.conn.ConnectionString = this.GetConnectString();
            this.conn.Open();
            this.transaction = this.conn.BeginTransaction();
            this.cmd.Connection = this.conn;
            this.cmd.Transaction = transaction;

            CreatFile();
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        public void Commit()
        {
            this.transaction.Commit();
            Colse();
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void Rollback()
        {
            this.transaction.Rollback();
            Colse();
        }

        /// <summary>
        /// sql链接实体
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
        /// sqlserver链接状态
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

        /// <summary>
        /// 关闭链接
        /// </summary>
        private void Colse()
        {
            try
            {
                if (this.conn != null && this.conn.State != ConnectionState.Closed)
                {
                    this.conn.Close();
                }
            }
            catch
            {
            }
            finally
            {
                this.conn.Close();
            }
        }

        /// <summary>
        /// 获得连接串
        /// </summary>
        /// <returns></returns>
        public string GetConnectString()
        {
            string dbInstance = "";
            string DataBaseName = "";
            string userName = "";
            string password = "";
            string connString = "";

            if (!System.IO.File.Exists(profileName))
            {
                Neusoft.FrameWork.Xml.XML myXml = new Neusoft.FrameWork.Xml.XML();
                XmlDocument doc = new XmlDocument();
                XmlElement root;
                root = myXml.CreateRootElement(doc, "SqlServerConnectForHis5.0", "1.0");

                XmlElement dbName = myXml.AddXmlNode(doc, root, "设置", "");

                myXml.AddNodeAttibute(dbName, "数据库实例名", "");
                myXml.AddNodeAttibute(dbName, "数据库名", "");
                myXml.AddNodeAttibute(dbName, "用户名", "");
                myXml.AddNodeAttibute(dbName, "密码", "");

                try
                {
                    StreamWriter sr = new StreamWriter(profileName, false, System.Text.Encoding.Default);
                    string cleandown = doc.OuterXml;
                    sr.Write(cleandown);
                    sr.Close();
                }
                catch (Exception ex)
                {
                    this.Err = "创建医保连接服务配置出错!" + ex.Message;
                    this.ErrCode = "-1";
                    this.WriteErr();
                    return "";
                }

                return "";
            }
            else
            {
                XmlDocument doc = new XmlDocument();

                try
                {
                    StreamReader sr = new StreamReader(profileName, System.Text.Encoding.Default);
                    string cleandown = sr.ReadToEnd();
                    doc.LoadXml(cleandown);
                    sr.Close();
                }
                catch { return ""; }

                XmlNode node = doc.SelectSingleNode("//设置");

                try
                {

                    dbInstance = node.Attributes["数据库实例名"].Value.ToString();
                    DataBaseName = node.Attributes["数据库名"].Value.ToString();
                    userName = node.Attributes["用户名"].Value.ToString();
                    password = node.Attributes["密码"].Value.ToString();
                }
                catch { return ""; }

                connString = "packet size=4096;user id=" + userName + ";data source=" + dbInstance + ";pers" +
                    "ist security info=True;initial catalog=" + DataBaseName + ";password=" + password;
            }

            return connString;
        }
        #endregion

        #region 写日志

        private string fileName = "./SQLServer.log";

        /// <summary>
        /// 创建日志
        /// </summary>
        private void CreatFile()
        {
            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.CreateText(fileName);
            }
        }

        private System.IO.TextWriter output;

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        private void ReadSQL(string sql)
        {
            this.WriteLog(sql);
            this.cmd.CommandText = sql;
        }

        #endregion

        #region 数据读写
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            ReadSQL(sql);
            return this.cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Execute(string sql, ref DataSet ds)
        {
            try
            {
                //DataSet ds = new DataSet();
                ReadSQL(sql);
                datareader = new SqlDataAdapter();
                datareader.SelectCommand = this.cmd;
                datareader.Fill(ds);
                return 1;
            }
            catch(Exception ex)
            {
                this.Err = ex.ToString();
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int QueryDataSet(string sql, ref DataSet ds)
        {
            try
            {
                //DataSet ds = new DataSet();
                ReadSQL(sql);
                datareader = new SqlDataAdapter();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="str"></param>
        /// <param name="dual"></param>
        /// <returns></returns>
        public int ExecuteQueryReturnOne(string sql, ref string str)
        {
            try
            {
                DataSet ds = new DataSet();

                ReadSQL(sql);

                this.reader = this.cmd.ExecuteReader();

                if (this.reader.Read())
                {
                    str = this.reader[0].ToString();
                }
                else
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                    return -1;
                }
            }
            catch
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }
                return -1;
            }
            finally
            {
                if (!this.reader.IsClosed)
                {
                    this.reader.Close();
                }
            }
            return 1;
                
                //datareader = new SqlDataAdapter(this.cmd.CommandText, this.conn);
                
                //datareader.Fill(ds);
                //try
                //{
                //    foreach (DataRow dr in ds.Tables[0].Rows)
                //    {
                //        str = dr[0].ToString();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    return -1;
                //}
                //return 1;
            //}
            //catch (Exception ex)
            //{
            //    this.Err = ex.ToString();
            //    return -1;
            //}
        }

        #endregion
        #endregion
    }
}
