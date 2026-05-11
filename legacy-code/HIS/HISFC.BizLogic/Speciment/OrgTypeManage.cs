using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 标本组织类型管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-13]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-14' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class OrgTypeManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">标本组织类型对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.OrgType orgType)
        {
            string sequence = ""; 
           
            GetNextSequence(ref sequence);   
            string[] str = new string[]
						{
							sequence, 
							orgType.OrgName,
                            orgType.IsShowOnApp.ToString()
						};
            return str;
        }

        private string[] GetParam(string orgTypeName, string orgTypeID)
        {
            
            string[] str = new string[]
						{
                            orgTypeName,
							orgTypeID
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
            sequence = this.GetSequence("Speciment.BizLogic.OrgTypeManage.GetNextSequence");
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

        #region 更新标本组织类型操作
        /// <summary>
        /// 更新标本组织类型信息
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateOrgType(string sqlIndex, params string[] args)
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
        /// 根据sql索引获取orgtype
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList GetOrgType(string sqlIndex, params string[] args)
        {           
            string sql = string.Empty;
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return null;
            }
            if (this.ExecQuery(sql, args) == -1)
                return null;
            ArrayList arrOrgType = new ArrayList();
            while (this.Reader.Read())
            {
                OrgType orgType = SetOrgType();
                arrOrgType.Add(orgType);
            }
            this.Reader.Close();
            return arrOrgType;
        }

        /// <summary>
        /// 读取标本组织类型信息
        /// </summary>
        /// <returns>组织类型实体</returns>
        private OrgType SetOrgType()
        {
            OrgType orgType = new OrgType();
            try
            {
                orgType.OrgTypeID = Convert.ToInt32(this.Reader[0].ToString());
                orgType.OrgName = this.Reader[1].ToString();
                orgType.IsShowOnApp = Convert.ToInt16(this.Reader[2].ToString());               
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return orgType;
        }
        #endregion

        #endregion

        #region 公共方法
        /// <summary>
        /// 标本组织类型插入
        /// </summary>
        /// <param name="specBox">即将插入的标本组织类型</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertOrgType(Neusoft.HISFC.Models.Speciment.OrgType orgType)
        {
            return this.UpdateOrgType("Speciment.BizLogic.OrgTypeManage.Insert", this.GetParam(orgType));
        }

        /// <summary>
        /// 根据名称更新标本组织类型
        /// </summary>
        /// <param name="orgType"></param>
        /// <returns></returns>
        public int UpdateOrgType(string orgTypeName,int orgTypeID)
        {
            return this.UpdateOrgType("Speciment.BizLogic.OrgTypeManage.Update", this.GetParam(orgTypeName, orgTypeID.ToString()));
        }

        /// <summary>
        /// 根据名称删除组织类型
        /// </summary>
        /// <param name="orgTypeName"></param>
        /// <returns></returns>
        public int DeleteOrgTypeByName(string orgTypeName)
        {
            return this.UpdateOrgType("Speciment.BizLogic.OrgTypeManage.Delete", new string[] { orgTypeName });
        }

        /// <summary>
        /// 根据标本种类名称获取ID
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public int SelectOrgByName(string orgName)
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.OrgTypeManage.SelectByName", ref sql) == -1)
                return 0;
            sql = string.Format(sql, orgName);
            if (this.ExecQuery(sql) == -1)
                return 0;
            int orgTypeID = 0;
            while (this.Reader.Read())
            {
                OrgType org = SetOrgType();
                orgTypeID = org.OrgTypeID;
            }
            this.Reader.Close();
            return orgTypeID; 
        }

        /// <summary>
        /// 加载所有的标本组织类型
        /// </summary>
        /// <returns>标本组织类型List</returns>
        public Dictionary<int, string> GetAllOrgType()
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.OrgTypeManage.SelectAll", ref sql) == -1)
                return null;
            
            if (this.ExecQuery(sql) == -1)
                return null;
            Dictionary<int, string> dicOrgType = new Dictionary<int, string>();
            //ArrayList specList = new ArrayList();
            while (this.Reader.Read())
            {
                OrgType org = SetOrgType();
                dicOrgType.Add(org.OrgTypeID, org.OrgName);
            }
            this.Reader.Close();
            return dicOrgType;
        }

        /// <summary>
        /// 根据spectypeid获取orgtype
        /// </summary>
        /// <param name="specTypeId"></param>
        /// <returns></returns>
        public OrgType GetBySpecType(string specTypeId)
        {
            ArrayList arr = this.GetOrgType("Speciment.BizLogic.OrgTypeManage.SelectBySpecId", new string[] { specTypeId });
            if (arr == null || arr.Count <= 0)
                return null;
            else
                return arr[0] as OrgType;
        }
        #endregion
    }
}
