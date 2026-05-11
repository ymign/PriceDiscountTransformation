using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 标本组织表结构管理]<br></br>
    /// [创 建 者: 林国科]<br></br>
    /// [创建时间: 2011-10-14]<br></br>
    /// <修改记录 
    ///		修改人='' 
    ///		修改时间='' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class QueryEngineManage : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 存放用户表名及列名
        /// </summary>
        public class Table
        {
            private string tableName;
            private KeyValuePair<string,string>[] tableColumn;
            private KeyValuePair<string, string>[] tableChineseNote;
            /// <summary>
            /// 表名
            /// </summary>
            public string TableName
            {
                get
                {
                    return tableName;
                }
                set
                {
                    tableName = value;
                }
            }
            /// <summary>
            /// 表中列名
            /// </summary>
            public KeyValuePair<string, string>[] TableColumn
            {
                get
                {
                    return tableColumn;
                }
                set
                {
                    tableColumn = value;
                }
            }

            public KeyValuePair<string, string>[] ColumnChinese
            {
                get
                {
                    return tableChineseNote;
                }
                set
                {
                    tableChineseNote = value;
                }
            }
        }

        /// <summary>
        /// 根据sql索引获取用户表的属性
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="parm"></param>
        /// <returns></returns>
        private ArrayList GetTableProperity(string sqlIndex, string[] parm)
        {
            ArrayList arrTablePoertiy = new ArrayList();
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            if (this.ExecQuery(sql, parm) == -1)
                return null;
            if (parm.Length <= 0)
            {
                while (this.Reader.Read())
                {
                    arrTablePoertiy.Add(this.Reader[0].ToString());
                }
            }
            else
            {
                while (this.Reader.Read())
                {
                    string key = Reader[0].ToString();
                    string value =Reader[1].ToString();
                    KeyValuePair<string, string> tmp = new KeyValuePair<string, string>(key,value);
                    arrTablePoertiy.Add(tmp); 
                }
            }
            this.Reader.Close();
            return arrTablePoertiy;
        }

        public Dictionary<string, string> GetTableName()
        {
            ArrayList arrTableName = GetTableProperity("Speciment.BizLogic.QueryEngineManage.QueryTableName", new string[] { });
            Dictionary<string, string> tableNameNote = new Dictionary<string, string>();
            foreach (string tableName in arrTableName)
            {
                switch (tableName)
                {
                    case "SPEC_APPLICATIONTABLE":
                        tableNameNote.Add(tableName, "用户申请记录");
                        break;
                    case "SPEC_BOX":
                        tableNameNote.Add(tableName, "标本盒");
                        break;
                    case "SPEC_ICEBOX":
                        tableNameNote.Add(tableName, "冰箱");
                        break;
                    case "SPEC_ICEBOXLAYER":
                        tableNameNote.Add(tableName, "冰箱层");
                        break;
                    case "SPEC_SHELF":
                        tableNameNote.Add(tableName, "冻存架");
                        break;
                    default:
                        tableNameNote.Add(tableName, tableName);
                        break;
                }
            }
            return tableNameNote;
        }

        public Table GetTableProperity(string tableName)
        {
            ArrayList tableColumns = this.GetTableProperity("Speciment.BizLogic.QueryEngineManage.QueryTableColumns", new string[] { tableName });
            Table table = new Table();
            //记录字段名和字段名的类型
            KeyValuePair<string, string>[] columns = new KeyValuePair<string, string>[tableColumns.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> column in tableColumns)
            {
                columns[i] = column;
                i++;
            }
            table.TableName = tableName;
            table.TableColumn = columns;
            return table;
        }

        /// <summary>
        /// 根据sql查询结果
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ds">dataset</param>
        public void Query(string sql, ref DataSet ds)
        {
            this.ExecQuery(sql, ref ds); 
        }
    }
}
