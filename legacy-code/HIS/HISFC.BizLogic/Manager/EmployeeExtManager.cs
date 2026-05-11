using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Manager
{
    /// <summary>
    ///<br>[功能描述: 员工字段扩展业务类]</br>
    ///<br>[创 建 者: 宋德宏]</br>
    ///<br>[创建时间: 2008-09-25]</br>
    ///    <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public class EmployeeExtManager : Neusoft.FrameWork.Management.Database
    {

        /// <summary>
        /// 插入扩展列记录方法
        /// </summary>
        /// <param name="expandColumn">扩展列</param>
        /// <returns>返回操作结果值：大于1代表操作成功，等于0代表没有进行操作，-1代表操作失败</returns>
        public int InsertExpandColumn(Neusoft.HISFC.Models.HR.ExpandColumn expandColumn)
        {
            return this.ExecUpdateSql("HR.ExpandColumn.InsertExpandColumn", GetExpandColumn(expandColumn));
        }

        /// <summary>
        /// 插入扩展列记录方法
        /// </summary>
        /// <param name="expandColumn">扩展列</param>
        /// <returns>返回操作结果值：大于1代表操作成功，等于0代表没有进行操作，-1代表操作失败</returns>
        public int InsertExpandColumnData(Neusoft.HISFC.Models.HR.ExpandColumn expandColumn)
        {
            return this.ExecUpdateSql("HR.ExpandColumnData.InsertExpandColumnData", new string[] { expandColumn.TableName,expandColumn.ColumnName,expandColumn.ID,expandColumn.Memo});
        }
        /// <summary>
        /// 更新扩展列记录方法
        /// </summary>
        /// <param name="expandColumn">扩展列</param>
        /// <returns>返回操作结果值：大于1代表操作成功，等于0代表没有进行操作，-1代表操作失败</returns>
        public int UpdateExpandColumn(Neusoft.HISFC.Models.HR.ExpandColumn expandColumn)
        {
            return this.ExecUpdateSql("HR.ExpandColumn.UpdateExpandColumn", GetExpandColumn(expandColumn));
        }

        /// <summary>
        /// 更新扩展列记录方法
        /// </summary>
        /// <param name="expandColumn">扩展列</param>
        /// <returns>返回操作结果值：大于1代表操作成功，等于0代表没有进行操作，-1代表操作失败</returns>
        public int UpdateExpandColumnData(Neusoft.HISFC.Models.HR.ExpandColumn expandColumn)
        {
            return this.ExecUpdateSql("HR.ExpandColumnData.UpdateExpandColumnData", new string[] { expandColumn.TableName, expandColumn.ColumnName, expandColumn.ID, expandColumn.Memo });
        }
        /// <summary>
        /// 删除扩展列记录方法
        /// </summary>
        /// <param name="expandColumn">扩展列</param>
        /// <returns>返回操作结果值：大于1代表操作成功，等于0代表没有进行操作，-1代表操作失败</returns>
        public int DeleteExpandColumn(string tableName,string columnName)
        {
            return this.ExecUpdateSql("HR.ExpandColumn.DeleteExpandColumn", new string[] { tableName,columnName});
        }
         /// <summary>
        /// 用于执行插入、更新、删除语句
        /// </summary>
        /// <param name="strSql">待执行的SQL索引</param>
        /// <param name="strParam">传过来的参数数组</param>
        /// <returns>返回操作结果值：大于1代表操作成功，等于0代表没有进行操作，-1代表操作失败</returns>
        public int ExecUpdateSql(string sqlParam, string[] strParam)
        {
            //定义变量来用来作为SQL语句载体
            string strSql = "";
            //用于得到Sql语句
            if (this.Sql.GetSql(sqlParam, ref strSql) == -1)
            {
                this.Err = "获得索引为" + sqlParam + "的SQL语句出错!";
                return -1;
            }

            try
            {
                strSql = string.Format(strSql, strParam);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return -1;
            }

            return this.ExecNoQuery(strSql);

        }
     
        /// <summary>
        /// 获得扩展列数据
        /// </summary>
        /// <returns></returns>
        public DataSet GetExpandColumn()
        {

            string strsql = string.Empty;
            DataSet ds = new DataSet();
            if (this.Sql.GetSql("HR.ExpandColumn.QueryExpandColumn", ref strsql) == -1)
            {
                this.Err = "没有找到‘HR.ExpandColumn.QueryExpandColumn’SQL语句";

                return null;
            }

            strsql = string.Format(strsql);
            if (this.ExecQuery(strsql, ref ds) == -1)
            {
                this.Err = "执行" + strsql + "发生错误" + this.Err;

                return null;
            }

            return ds;

        }

        /// <summary>
        /// 通过表名得到列结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataSet QueryExpandColumnByTableNameAndObjID(string tableName,string objID)
        {

            string strsql = string.Empty;
            DataSet ds = new DataSet();
            if (this.Sql.GetSql("HR.ExpandColumnData.QueryExpandColumnDataByTabNameAndObjID", ref strsql) == -1)
            {
                this.Err = "没有找到‘HR.ExpandColumnData.QueryExpandColumnDataByTabNameAndObjID’SQL语句";

                return null;
            }

            strsql = string.Format(strsql, tableName,objID);
            if (this.ExecQuery(strsql, ref ds) == -1)
            {
                this.Err = "执行" + strsql + "发生错误" + this.Err;

                return null;
            }

            return ds;

        }

        /// <summary>
        /// 通过表名得到列结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataSet QueryExpandColumnByTableName(string tableName)
        {

            string strsql = string.Empty;
            DataSet ds = new DataSet();
            if (this.Sql.GetSql("HR.ExpandColumn.QueryExpandColumnByTableName", ref strsql) == -1)
            {
                this.Err = "没有找到‘HR.ExpandColumn.QueryExpandColumnByTableName’SQL语句";

                return null;
            }

            strsql = string.Format(strsql,tableName);
            if (this.ExecQuery(strsql, ref ds) == -1)
            {
                this.Err = "执行" + strsql + "发生错误" + this.Err;

                return null;
            }

            return ds;

        }
        /// <summary>
        /// 获得扩展列数据
        /// </summary>
        /// <returns></returns>
        public DataSet GetExpandColumnData(string tableName,string columnName)
        {

            string strsql = string.Empty;
            DataSet ds = new DataSet();
            if (this.Sql.GetSql("HR.ExpandColumnData.QueryExpandColumnData", ref strsql) == -1)
            {
                this.Err = "没有找到‘HR.ExpandColumnData.QueryExpandColumnData’SQL语句";

                return null;
            }

            strsql = string.Format(strsql,tableName,columnName);
            if (this.ExecQuery(strsql, ref ds) == -1)
            {
                this.Err = "执行" + strsql + "发生错误" + this.Err;

                return null;
            }

            return ds;

        }

        #region 把实体转换成参数数组
        /// <summary>
        /// 传入实体得到参数数组
        /// </summary>
        /// <param name="expandColumn">扩展字段实体</param>
        /// <returns>实体的参数数组</returns>
        private string[] GetExpandColumn(Neusoft.HISFC.Models.HR.ExpandColumn expandColumn)
        {
            string[] strParam = new string[]
            {
                expandColumn.TableName ,
                expandColumn.ColumnName,
                expandColumn.ColumnType,
                expandColumn.ColumnLength.ToString(),
                expandColumn.ColumnDecimalLen.ToString(),
                expandColumn.DefaultValue,
                Neusoft.FrameWork.Function.NConvert.ToInt32(expandColumn.IsNull).ToString(),
                Neusoft.FrameWork.Function.NConvert.ToInt32( expandColumn.IsValid).ToString(),
               
                expandColumn.Remark ,
               expandColumn.SortID.ToString()
            };
            return strParam;

        }
        #endregion
    }
}
