using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.HealthRecord.EnumServer;

namespace Neusoft.HISFC.BizLogic.HealthRecord
{
    /// <summary>
    /// <br>ICDCategory</br>
    /// <Font color='#FF1111'>[功能描述: ICD分类业务层]</Font><br></br>
    /// [创 建 者: ]<br>喻赟</br>
    /// [创建时间: ]<br>2009-06-08</br>
    /// <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///		/>
    /// </summary>
    public class ICDCategory : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ICDCategory()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 方法

        #region 私有

        /// <summary>
        /// 获得update或者insert的传入参数数组
        /// </summary>
        /// <param name="undrug">非药品实体</param>
        /// <returns>参数数组</returns>
        private string[] GetParams(Neusoft.HISFC.Models.HealthRecord.ICDCategory ICDCategory)
        {
            string[] args = new string[]
			{	
				ICDCategory.ID,
                ICDCategory.ParentID,
                ICDCategory.SeqNO,
                ICDCategory.Name,
                ICDCategory.SpellCode,
                ICDCategory.SortID,
                ICDCategory.Range,
                ICDCategory.Sfbr
			};

            return args;
        }

        private ArrayList GetListBySql(string sql)
        {
            ArrayList alICDTemps = new ArrayList(); //用于返回非药品信息的数组

            //执行当前Sql语句
            if (this.ExecQuery(sql) == -1)
            {
                this.Err = this.Sql.Err;

                return null;
            }

            try
            {
                //循环读取数据
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.HealthRecord.ICDCategory ICDCategory = new Neusoft.HISFC.Models.HealthRecord.ICDCategory();

                    ICDCategory.ID = Reader[0].ToString();
                    ICDCategory.ParentID = Reader[1].ToString();
                    ICDCategory.SeqNO = Reader[2].ToString();
                    ICDCategory.Name = Reader[3].ToString();
                    ICDCategory.SpellCode = Reader[4].ToString();
                    ICDCategory.SortID = Reader[5].ToString();
                    ICDCategory.Range = Reader[6].ToString();
                    ICDCategory.Sfbr = Reader[7].ToString();

                    alICDTemps.Add(ICDCategory);
                }//循环结束
            }
            catch (Exception e)
            {
                this.Err = "获得诊断分类基本信息出错！" + e.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return alICDTemps;
        }


        private int SetICDCategory(Neusoft.HISFC.Models.HealthRecord.ICDCategory icdTemp, string sqlIndex)
        {
            string sql = "";

            try
            {
                //获取查询SQL语句
                if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:" + sqlIndex + "";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1; // 出现错误返回null
            }

            string[] strParam = this.GetParams(icdTemp);

            sql = string.Format(sql, strParam);
            return this.ExecNoQuery(sql);
        }

        #endregion

        #region 公开

        /// <summary>
        /// 查询ICD模板
        /// </summary>
        public ArrayList QueryCategoryBySortID(string sortID)
        {
            //定义字符变量 ,存储查询主体SQL语句
            string strSelect = "";
            //定义字符变量 ,存储WHERE 条件SQL语句
            string strWhere = "";
            //定义动态数组 ,存储查询出的信息
            ArrayList arryList = new ArrayList();
            try
            {
                //获取查询SQL语句
                if (this.Sql.GetSql("HealthRecord.ICDCategory.Query", ref strSelect) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDTemplate.Query";
                    return null;
                }
                //获取查询where语句
                if (this.Sql.GetSql("HealthRecord.ICDCategory.Where.BySortID", ref strWhere) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDCategory.Where.BySortID";
                    return null;
                }
                try
                {
                    //格式化SQL语句 --------------------------------icd_code-valid_state-type-owner
                    strSelect = string.Format(strSelect + strWhere, sortID);
                }
                catch (Exception ex)
                {
                    this.Err = "SQL语句赋值出错!" + ex.Message;
                }
                //读取数据
                arryList = this.GetListBySql(strSelect);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null; // 出现错误返回null
            }
            finally
            {
                if (this.Reader != null)
                {
                    this.Reader.Close();
                }
            }

            return arryList;
        }

        /// <summary>
        /// 查询ICD模板
        /// </summary>
        public ArrayList QueryCategoryByParentID(string parentID, string sortID)
        {
            //定义字符变量 ,存储查询主体SQL语句
            string strSelect = "";
            //定义字符变量 ,存储WHERE 条件SQL语句
            string strWhere = "";
            //定义动态数组 ,存储查询出的信息
            ArrayList arryList = new ArrayList();
            try
            {
                //获取查询SQL语句
                if (this.Sql.GetSql("HealthRecord.ICDCategory.Query", ref strSelect) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDTemplate.Query";
                    return null;
                }
                //获取查询where语句
                if (this.Sql.GetSql("HealthRecord.ICDCategory.Where.ByParentID", ref strWhere) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDCategory.Where.ByParentID";
                    return null;
                }
                try
                {
                    //格式化SQL语句 --------------------------------icd_code-valid_state-type-owner
                    strSelect = string.Format(strSelect + strWhere, parentID, sortID);
                }
                catch (Exception ex)
                {
                    this.Err = "SQL语句赋值出错!" + ex.Message;
                }
                //读取数据
                arryList = this.GetListBySql(strSelect);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null; // 出现错误返回null
            }
            finally
            {
                this.Reader.Close();
            }

            return arryList;
        }

        /// <summary>
        /// 查询ICD模板
        /// </summary>
        public ICDCategory GetICDCategory(string ID)
        {
            //定义字符变量 ,存储查询主体SQL语句
            string strSelect = "";
            //定义字符变量 ,存储WHERE 条件SQL语句
            string strWhere = "";
            //定义动态数组 ,存储查询出的信息
            ArrayList arryList = new ArrayList();
            try
            {
                //获取查询SQL语句
                if (this.Sql.GetSql("HealthRecord.ICDCategory.Query", ref strSelect) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDTemplate.Query";
                    return null;
                }
                //获取查询where语句
                if (this.Sql.GetSql("HealthRecord.ICDCategory.Where.ByID", ref strWhere) == -1)
                {
                    this.Err = "获取SQL语句失败,索引:HealthRecord.ICDCategory.Where.ByID";
                    return null;
                }
                try
                {
                    //格式化SQL语句 --------------------------------icd_code-valid_state-type-owner
                    strSelect = string.Format(strSelect + strWhere, ID);
                }
                catch (Exception ex)
                {
                    this.Err = "SQL语句赋值出错!" + ex.Message;
                }
                //读取数据
                arryList = this.GetListBySql(strSelect);
                if (arryList != null && arryList.Count > 0)
                {
                    return arryList[0] as ICDCategory;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null; // 出现错误返回null
            }
            finally
            {
                this.Reader.Close();
            }

            return null;
        }

        /// <summary>
        /// 插入诊断分类
        /// </summary>
        /// <param name="ICDCategory"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.HealthRecord.ICDCategory ICDCategory)
        {
            return SetICDCategory(ICDCategory, "HealthRecord.ICDCategory.Insert");
        }

        /// <summary>
        /// 更新诊断分类
        /// </summary>
        /// <param name="ICDCategory"></param>
        /// <returns></returns>
        public int Update(Neusoft.HISFC.Models.HealthRecord.ICDCategory ICDCategory)
        {
            return SetICDCategory(ICDCategory, "HealthRecord.ICDCategory.Update");
        }

        /// <summary>
        /// 删除诊断分类
        /// </summary>
        /// <param name="ICDCategory"></param>
        /// <returns></returns>
        public int Delete(Neusoft.HISFC.Models.HealthRecord.ICDCategory ICDCategory)
        {
            return SetICDCategory(ICDCategory, "HealthRecord.ICDCategory.Delete");
        }

        #endregion

        #endregion
    }
}
