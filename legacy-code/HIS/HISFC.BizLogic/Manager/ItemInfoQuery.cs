using System;
using System.Collections.Generic;
using System.Text;
using System.Data ;
using System.Collections ;

namespace Neusoft.HISFC.BizLogic.Manager
{
    public class ItemInfoQuery : DataBase 
    {
        ///非药品项目查询维护功能
        ///按类别查询,按科室查询,按项目查询 项目收费情况
        ///维护

        public ItemInfoQuery()
        {
            
        }

        /// <summary>
        /// 获取查询类型序号
        /// </summary>
        /// <returns></returns>
        public string GetSequence()
        {
            string querySql = "SELECT SEQ_FIN_COM_ITEMQUERY.Nextval FROM dual";

            try
            {
                return this.ExecSqlReturnOne(querySql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();
                return "";
            }
        }

        /// <summary>
        /// 插入项目查询信息维护,单体插入
        /// </summary>
        /// <param name="ament"></param>
        /// <returns></returns>
        public int InsertItemInfoAment(Neusoft.HISFC.Models.Base.Const ament)
        {
            if (ament == null)
            {
                return 0;
            }

            string insertSql = string.Empty;

            if (this.GetSQL("UniReport.InsertItemQueryInfo", ref insertSql) == -1)
            {
                this.Err = "获取Sql语句出错,索引号:UniReport.InsertItemQueryInfo";
                WriteErr();
                return -1;
            }

            try
            {
                insertSql = string.Format(insertSql, Neusoft.FrameWork.Function.NConvert.ToInt32(ament.ID),     // 主键
                                                  ament.Name,    //查询类型
                                                  ament.User01,  //科室代码
                                                  ament.User02,  //项目代码
                                                  ament.User03,   //项目名称
                                                  Neusoft.FrameWork.Function.NConvert.ToInt32(ament.Memo),  //顺序号
                                                  this.Operator.ID,//操作员
                                                  ament.SpellCode);  
                if (ExecNoQuery(insertSql) < 0)
                {
                    this.Err = "插入数据失败,错误号:" + this.ErrCode;
                    WriteErr();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();

                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// 插入维护数据 (批量插入)
        /// </summary>
        /// <param name="mentArr"></param>
        /// <returns></returns>
        public int InsertItemInfoAment(ArrayList mentArr)
        {
            if (mentArr == null || mentArr.Count <= 0)
            {
                this.Err = "插入数据为空,请核对后再进行保存";
                WriteErr();
                return 0;
            }

            string insertSql = string.Empty;

            if (this.GetSQL("UniReport.InsertItemQueryInfo", ref insertSql) == -1)
            {
                this.Err = "获取Sql语句出错,索引号:UniReport.InsertItemQueryInfo";
                WriteErr();
                return -1;
            }

            for (int i = 0; i < mentArr.Count; i++)
            {
                Neusoft.HISFC.Models.Base.Const ament = mentArr[i] as Neusoft.HISFC.Models.Base.Const;

                string sqleach = insertSql;

                try
                {
                    sqleach = string.Format(insertSql, ament.ID,     // 查询类型
                                                      ament.Name,    //科室代码
                                                      ament.User01,  //科室名称
                                                      ament.User02,  //项目代码
                                                      ament.User03,   //项目名称
                                                      Neusoft.FrameWork.Function.NConvert.ToInt32(ament.Memo),  //顺序号
                                                      this.Operator.ID,
                                                      ament.SpellCode);  //操作员
                    if (ExecNoQuery(sqleach) < 0)
                    {
                        this.Err = "插入数据失败,错误号:" + this.ErrCode;
                        WriteErr();
                        return -1;
                    }                         
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    WriteErr();

                    if (!this.Reader.IsClosed)
                    {
                        this.Reader.Close();
                    }
                    return -1;
                }
            }
            return 1;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="amentObj"></param>
        /// <returns></returns>
        public int UpdateItemInfoAment(Neusoft.HISFC.Models.Base.Const amentObj)
        {
            if (amentObj == null)
            {
                this.Err = "更新实体为空!";
                WriteErr();
                return 0;
            }

            string updateSql = "";

            if (this.GetSQL("UniReport.UpdateItemQueryInfo", ref updateSql) == -1)
            {
                this.Err = "索引更新语句出错,索引号:UniReport.UpdateItemQueryInfo";
                WriteErr();
                return -1;
            }

            try
            {
                updateSql = string.Format(updateSql, Neusoft.FrameWork.Function.NConvert.ToInt32(amentObj.ID),     // 主键
                                                   amentObj.Name,    //查询类型
                                                   amentObj.User01,  //科室代码
                                                   amentObj.User02,  //项目代码
                                                   amentObj.User03,   //项目名称
                                                   Neusoft.FrameWork.Function.NConvert.ToInt32(amentObj.Memo),  //顺序号
                                                   this.Operator.ID,
                                                   amentObj.SpellCode);  //操作员
                if (ExecNoQuery(updateSql) < 0)
                {
                    this.Err = "更新数据失败,错误号:" + this.ErrCode;
                    WriteErr();
                    return -1;
                }   
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();

                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amentObj"></param>
        /// <returns></returns>
        public int DeleteItemInfoAment(Neusoft.FrameWork.Models.NeuObject amentObj)
        {
            if (amentObj == null)
            {
                this.Err = "信息实体为空,请核对后再进行删除!";
                WriteErr();
                return 0;
            }

            string deleteSql = "";
            if (this.GetSQL("UniReport.DeleteItemQueryInfo", ref deleteSql) == -1)
            {
                this.Err = "获取Sql语句失败,索引号:UniReport.DeleteItemQueryInfo";
                WriteErr();
                return -1;
            }

            try
            {
                deleteSql = string.Format(deleteSql, Neusoft.FrameWork.Function.NConvert.ToInt32(amentObj.ID));
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();
                return -1;
            }
            return this.ExecNoQuery(deleteSql);
        }

        /// <summary>
        /// 查询已维护的项目查询信息
        /// </summary>
        /// <param name="deptCode">权限科室</param>
        /// <param name="queryType">查询类型</param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int QueryItemQueryMend(string deptCode,string queryType,ref DataSet ds)
        {
            string querySql = "";

            if (this.GetSQL("UniReport.GetItemQueryInfoByDeptQueryType", ref querySql) == -1)
            {
                this.Err = "获取Sql语句出错,索引号:UniReport.GetItemQueryInfoByDeptQueryType";
                WriteErr();
                ds = null;
                return -1;
            }
            try
            {
                querySql = string.Format(querySql, deptCode, queryType);
            }
            catch (Exception ex)
            {
                this.Err = "参数格式化错误" + ex.Message;
                WriteErr();
                ds = null;
                return -1;
            }

            return this.ExecQuery(querySql, ref ds);
        }

        /// <summary>
        /// 查询已维护的项目查询信息
        /// </summary>
        /// <param name="deptCode">权限科室</param>
        /// <param name="queryType">查询类型</param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int QueryItemQueryMend_Const(string deptCode, string queryType, ref DataSet ds)
        {
            string querySql = "";

            if (this.GetSQL("UniReport.GetItemQueryInfoByDeptQueryType.Const", ref querySql) == -1)
            {
                this.Err = "获取Sql语句出错,索引号:UniReport.GetItemQueryInfoByDeptQueryType.Const";
                WriteErr();
                ds = null;
                return -1;
            }
            try
            {
                querySql = string.Format(querySql, deptCode, queryType);
            }
            catch (Exception ex)
            {
                this.Err = "参数格式化错误" + ex.Message;
                WriteErr();
                ds = null;
                return -1;
            }

            return this.ExecQuery(querySql, ref ds);
        }

        /// <summary>
        /// 通过查询类型获得科室编码和名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ArrayList GetDeptByItemQueryType(string type)
        {
            string querySql = "";
            ArrayList al = new ArrayList();
            Neusoft.FrameWork.Models.NeuObject obj;

            if (this.GetSQL("UniReport.GetDeptByItemQuery", ref querySql) == -1)
            {
                this.Err = "获取Sql语句出错,索引号:UniReport.GetDeptByItemQuery";
                WriteErr();
                return null;
            }

            try
            {
                querySql = string.Format(querySql, type);
            }
            catch (Exception ex)
            {
                this.Err = "参数格式化错误" + ex.Message;
                WriteErr();
                return null;
            }

            if (this.ExecQuery(querySql) == -1)
            {
                return null;
            }

            try
            {
                while (this.Reader.Read())
                {
                    obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = this.Reader[0].ToString();
                    obj.Name = this.Reader[1].ToString();
                    al.Add(obj);
                }
            }
            catch(Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = "获取科室信息失败";
                this.WriteErr();
                return null;
            }

            return al;
        }

        /// <summary>
        /// 删除查询类型下的具体项目
        /// </summary>
        /// <param name="queryType">查询类型</param>
        /// <param name="deptCode">科室编码</param>
        /// <returns></returns>
        public int DeleteItemInfoAment(string queryType, string deptCode)
        {
            string deleteSql = "";
            if (this.GetSQL("UniReport.DeleteItemQueryList", ref deleteSql) == -1)
            {
                this.Err = "UniReport.DeleteItemQueryList";
                WriteErr();
                return -1;
            }

            try
            {
                deleteSql = string.Format(deleteSql, queryType, deptCode);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();
                return -1;
            }
            return this.ExecNoQuery(deleteSql);
        }    
    }
}
