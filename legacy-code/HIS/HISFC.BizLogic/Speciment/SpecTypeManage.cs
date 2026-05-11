using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Neusoft.HISFC.Models.Speciment;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 标本类型管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-13]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-19' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
   public class SpecTypeManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">标本盒规格对象</param>
        /// <returns></returns>
       private string[] GetParam(Neusoft.HISFC.Models.Speciment.SpecType specType)
       {
           string[] str;
           str = new string[]
						{
                            specType.SpecTypeID.ToString(),
							specType.SpecTypeName,
                            specType.SpecColor,
                            specType.OrgType.OrgTypeID.ToString(),
                            specType.Comment,
                            specType.IsShow,
                            specType.DefaultCnt.ToString(),
                            specType.Ext1,
                            specType.Ext2,
                            specType.Ext3
						};
           return str;
       }
        /// <summary>
        /// 获取新的Sequence(1：成功/-1：失败)
        /// </summary>
        /// <param name="sequence">获取的新的Sequence</param>
        /// <returns>1：成功/-1：失败</returns>
        private int GetNextSequence(ref string sequence)
        {
            //
            // 执行SQL语句
            //
            sequence = this.GetSequence("Speciment.BizLogic.SpecTypeManage.GetNextSequence");
            //
            // 如果返回NULL，则获取失败
            //
            if (sequence == null)
            {
                this.SetError("", "获取Sequence失败");
                return -1;
            }
            //
            // 成功返回
            //
            return 1;
        }

        #region 设置错误信息
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="errorCode">错误代码发生行数</param>
        /// <param name="errorText">错误信息</param>
        private void SetError(string errorCode, string errorText)
        {
            this.ErrCode = errorCode;
            this.Err = errorText + "[" + this.Err + "]"; // + "在ShelfSpecManage.cs的第" + argErrorCode + "行代码";
            this.WriteErr();
        }
        #endregion
        #endregion

        #region 更新标本类型操作
        /// <summary>
        /// 更新标本类型信息
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateSpecType(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;//Update语句

            //获得Where语句
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return -1;
            }

            return this.ExecNoQuery(sql, args);
        }

       /// <summary>
       /// 根据sqlIndex获取符合条件的SpecType List
       /// </summary>
       /// <param name="sqlIndex"></param>
       /// <param name="args"></param>
       /// <returns>标本类型List</returns>
       private ArrayList GetSpecType(string sqlIndex, params string[] args)
       {
           string sql = "";
           if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
               return null;
           if (this.ExecQuery(sql, args) == -1)
               return null;
           ArrayList arrSpecList = new ArrayList();
           while (this.Reader.Read())
           {
               SpecType specType= SetSpecType();
               arrSpecList.Add(specType);
           }
           this.Reader.Close();
           return arrSpecList;
       }

        /// <summary>
        /// 读取标本类型信息
        /// </summary>
        /// <returns>组织类型实体</returns>
       private SpecType SetSpecType()
        {
            SpecType specType= new SpecType();
            try
            {
                specType.SpecTypeID = Convert.ToInt32(this.Reader[0].ToString());
                specType.SpecTypeName = this.Reader[1].ToString();
                specType.SpecColor = this.Reader[2].ToString();
                specType.OrgType.OrgTypeID = Convert.ToInt32(this.Reader[3].ToString());
                specType.Comment =this.Reader[4].ToString();
                specType.IsShow = this.Reader[5].ToString();
                specType.DefaultCnt =  Convert.ToInt32(this.Reader[6].ToString());
                specType.Ext1 = this.Reader[7].ToString();
                specType.Ext2 = this.Reader[8].ToString();
                specType.Ext3 = this.Reader[9].ToString();
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return specType;
        }
        #endregion

        #endregion

        #region 公共方法
        /// <summary>
        /// 标本类型插入
        /// </summary>
        /// <param name="specBox">即将插入的标本类型</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertSpecType(Neusoft.HISFC.Models.Speciment.SpecType specType)
        {
            // return this.UpdateSingleTable("Fee.OutPatient.DayBalance.Insert", this.GetDayBalanceParams(dayBalance));
            return this.UpdateSpecType("Speciment.BizLogic.SpecTypeManage.Insert", this.GetParam(specType));
        }

        /// <summary>
        /// 根据名称更新标本组织类型
        /// </summary>
        /// <param name="orgType"></param>
        /// <returns></returns>
        public int UpdateSpecType(SpecType specType)
        {

            // return this.UpdateSingleTable("Fee.OutPatient.DayBalance.Insert", this.GetDayBalanceParams(dayBalance));
            return this.UpdateSpecType("Speciment.BizLogic.SpecTypeManage.Update", this.GetParam(specType));
        }

       /// <summary>
       /// 获取sequence
       /// </summary>
       /// <returns></returns>
       public int GetSequence()
       {
           string sequence = "";
           this.GetNextSequence(ref sequence);
           try
           {
               return Convert.ToInt32(sequence);
           }
           catch
           {
               return 0;
           }
       }

        /// <summary>
        /// 根据名称删除标本类型
        /// </summary>
        /// <param name="orgTypeName"></param>
        /// <returns></returns>
        public int DeleteSpecTypeByID(string specTypeID)
        {
            // SQL语句
            string sql = "";
            //
            // 获取SQL语句
            //
            if (this.Sql.GetSql("Speciment.BizLogic.SpecTypeManage.Delete", ref sql) == -1)
            {
                this.Err = "获取SQL语句Speciment.BizLogic.SpecTypeManage.Delete失败";
                return -1;
            }
            // 匹配执行SQL语句
            try
            {
                sql = string.Format(sql, specTypeID);

                return this.ExecNoQuery(sql);
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                return -1;
            }

        }

        /// <summary>
        /// 加载所有的标本类型
        /// </summary>
        /// <returns>标本组织List</returns>
        public Dictionary<int, string> GetSpecTypeByOrgID(string orgTypeID)
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.SpecTypeManage.SelectByOrgID", ref sql) == -1)
                return null;
            sql = string.Format(sql, orgTypeID);

            if (this.ExecQuery(sql) == -1)
                return null;
            Dictionary<int, string> dicSpecType = new Dictionary<int, string>();
            //ArrayList specList = new ArrayList();
            while (this.Reader.Read())
            {
                SpecType type = SetSpecType();
                dicSpecType.Add(type.SpecTypeID, type.SpecTypeName);
            }
            this.Reader.Close();
            return dicSpecType;
        }

       /// <summary>
       /// 根据名称获取ID
       /// </summary>
       /// <param name="specName"></param>
       /// <returns></returns>
       public int GetSpecIDByName(string specName)
       {
           string sql = "";
           if (this.Sql.GetSql("Speciment.BizLogic.SpecTypeManage.SelectByName", ref sql) == -1)
               return 0;
           sql = string.Format(sql, specName);

           if (this.ExecQuery(sql) == -1)
               return 0;
           int typeID =  0;
           while (this.Reader.Read())
           {
               SpecType type = SetSpecType();
               typeID = type.SpecTypeID;
           }
           this.Reader.Close();
           return typeID;
       }

       /// <summary>
       /// 根据标本类型名称和种类名称获取标本类型ID
       /// </summary>
       /// <param name="specName">标本类型名称</param>
       /// <param name="orgName">标本类型ID</param>
       /// <returns></returns>
       public int GetSpecIDByName(string specName, string orgName)
       {
           int specTypeId = 0;
           ArrayList arrList = new ArrayList();
           arrList = GetSpecType("", new string[] { specName, orgName });
           foreach (SpecType spec in arrList)
           {
               specTypeId = spec.SpecTypeID;
           }
           return specTypeId;
 
       }

       /// <summary>
       /// 根据标本类型的ID获取标本颜色
       /// </summary>
       /// <param name="SpecTypeID"></param>
       /// <returns></returns>
       public string GetColorByType(int SpecTypeID)
       {
           string sql = "";
           if (this.Sql.GetSql("Speciment.BizLogic.SpecTypeManage.SelectColorByTypeID", ref sql) == -1)
               return "";
           sql = string.Format(sql, SpecTypeID);

           if (this.ExecQuery(sql) == -1)
               return "";
           string typeColor = "";
           while (this.Reader.Read())
           {
               SpecType type = SetSpecType();
               typeColor = type.SpecColor;
           }
           this.Reader.Close();
           return typeColor;
       }

       /// <summary>
       /// 根据组织名称获取标本类型list
       /// </summary>
       /// <param name="orgName"></param>
       /// <returns>标本类型List</returns>
       public ArrayList GetSpecByOrgName(string orgName)
       {
           string[] sql = new string[] { orgName };
           return GetSpecType("Speciment.BizLogic.SpecTypeManage.SelectByOrgName", sql);
       }

       /// <summary>
       /// 根据SpecTypeID查找标本类型
       /// </summary>
       /// <param name="specTypeId">标本类型ID</param>
       /// <returns></returns>
       public SpecType GetSpecTypeById(string specTypeId)
       {
           string[] sql = new string[] { specTypeId };
           ArrayList arrList = GetSpecType("Speciment.BizLogic.SpecTypeManage.SelectByID", sql);
           SpecType specType = new SpecType();
           foreach (SpecType s in arrList)
           {
               specType = s;
               break;              
           }
           return specType;
       }

       /// <summary>
       /// 获取标本类型
       /// </summary>
       /// <param name="sql"></param>
       /// <returns></returns>
       public ArrayList GetSpecType(string sql)
       {
           if (this.ExecQuery(sql) == -1)
               return null;
           ArrayList arrSpecType = new ArrayList();
           while (Reader.Read())
           {
               SpecType tmp = SetSpecType();
               arrSpecType.Add(tmp); 
           }
           Reader.Close();
           return arrSpecType;
       }

       /// <summary>
       /// 根据标本盒的ID查找盒子中存放的标本类型
       /// </summary>
       /// <param name="boxId"></param>
       /// <returns></returns>
       public SpecType GetSpecTypeByBoxId(string boxId)
       {
           string[] sql = new string[] { boxId };
           ArrayList arrSpecType = new ArrayList();
           arrSpecType = GetSpecType("Speciment.BizLogic.SpecTypeManage.GetByBoxId", sql);
           return arrSpecType[0] as SpecType;
      
       }

       /// <summary>
       /// 获取全部的标本类型
       /// </summary>
       /// <returns></returns>
       public ArrayList GetAllSpecType()
       {
           ArrayList arrSpecType = new ArrayList();
           arrSpecType = GetSpecType("Speciment.BizLogic.SpecTypeManage.SelectAll", new string[] { });
           return arrSpecType;
       }
        #endregion
    }
}
