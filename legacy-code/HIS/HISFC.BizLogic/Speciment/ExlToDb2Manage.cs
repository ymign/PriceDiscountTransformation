using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Neusoft.FrameWork.Management;
using Microsoft.VisualBasic;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: Excel 与 db2互导数据管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-11-4]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public class ExlToDb2Manage : Neusoft.FrameWork.Management.Database
    {

        /// <summary>
        /// 连接Excel
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int ExlConnect(string fileName, ref DataSet ds)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                string version = excel.Version;

                string connectString = @"Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=""Excel 8.0;HDR=YES;""";
                //connectString = connectString.Replace("8.0", version);
                connectString += " ;Data Source= " + fileName + ";";
                DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
                ds = new DataSet();

                using (DbDataAdapter adapter = factory.CreateDataAdapter())
                {
                    using (DbCommand command = factory.CreateCommand())
                    {
                        DbConnection conn = factory.CreateConnection();
                        conn.ConnectionString = connectString;
                        command.Connection = conn;
                        command.CommandText = "SELECT * FROM[Sheet1$]";
                        adapter.SelectCommand = command;
                        adapter.Fill(ds);
                        ds.Tables[0].TableName = "TABLE1";
                        //ExlToDb2Manage manage = new ExlToDb2Manage();
                        //manage.InsertDataFromExl(ds, ds.Tables[0].TableName);
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 将dataset里的数据插入数据表为tableName中
        /// </summary>
        /// <param name="ds">Excel中的值</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public int InsertDataFromExl(DataSet ds, string tableName)
        {
            //System.Data adapter = new IBM.Data.DB2.DB2DataAdapter();
            //IBM.Data.DB2.DB2Command command = new IBM.Data.DB2.DB2Command();
            //IBM.Data.DB2.DB2Connection connection = new IBM.Data.DB2.DB2Connection();
            //DataSet ds1 = new DataSet();
            //IBM.Data.DB2.DB2CommandBuilder cmdb = new IBM.Data.DB2.DB2CommandBuilder(adapter);
            ////获取导入表的表头
            //command.CommandText = "select * from " + tableName + " fetch first 1 row only";
            //adapter.SelectCommand = command;
            //connection = this.Connection as IBM.Data.DB2.DB2Connection;
            //adapter.SelectCommand.Connection = connection;
            //adapter.Fill(ds1);
            //ds1.Clear();
            ////将要插入的数据遍历循环加入ds中
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    DataRow drNew = ds1.Tables[0].NewRow();
            //    drNew.ItemArray = dr.ItemArray;
            //    ds1.Tables[0].Rows.Add(drNew);
            //}
            //try
            //{
            //    adapter.InsertCommand = new IBM.Data.DB2.DB2Command("", connection, this.Trans as IBM.Data.DB2.DB2Transaction);
            //    adapter.InsertCommand = cmdb.GetInsertCommand();
            //    cmdb.DataAdapter.Update(ds1, ds1.Tables[0].TableName.ToString());
            //    return 1;
            //}
            //catch (Exception ex)
            //{
            //    this.ErrCode = ex.ToString();
            //    this.Trans.Rollback();
            //    return -1;
            //}
            return -1;
        }
    }
}
