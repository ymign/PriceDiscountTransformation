using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 病种类型管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-15]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-30' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class DisTypeManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="disType">标本组织类型对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.DiseaseType disType)
        {
            string[] str;
            str = new string[]
						{
							disType.DisTypeID.ToString(), 
                            disType.DiseaseName,
                            disType.DiseaseColor,
                            disType.Comment,
                            disType.OrgOrBld,
                            disType.Ext1,
                            disType.Ext2,
                            disType.Ext3
						};
            return str; 
            
        }

        /// <summary>
        /// 获取新的Sequence(1：成功/-1：失败)
        /// </summary>
        /// <param name="sequence">获取的新的Sequence</param>
        /// <returns>1：成功/-1：失败</returns>
        public int GetNextSequence(ref string sequence)
        {
            //
            // 执行SQL语句
            //
            sequence = this.GetSequence("Speciment.BizLogic.DisTypeManage.GetNextSequence");
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

        #region 更新病种类型操作
        /// <summary>
        /// 更新病种类型
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateDisType(string sqlIndex, params string[] args)
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
        /// 读取病种信息
        /// </summary>
        /// <returns>病种实体</returns>
        private DiseaseType SetDisType()
        {
            DiseaseType disType = new DiseaseType();
            try
            {
                 disType.DisTypeID = Convert.ToInt32(this.Reader[0].ToString());
                disType.DiseaseName = this.Reader[1].ToString();
                disType.DiseaseColor = this.Reader[2].ToString();
                disType.Comment = this.Reader[3].ToString();
                disType.OrgOrBld = this.Reader[4].ToString();
                disType.Ext1 = this.Reader[5].ToString();
                disType.Ext2 = this.Reader[6].ToString();
                disType.Ext3 = this.Reader[7].ToString();
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return disType;
        }

        /// <summary>
        /// 获取病种类型列表
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        private ArrayList GetDisType(string sqlIndex, params string[] parm)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            sql = string.Format(sql,parm);
            if (this.ExecQuery(sql) == -1)
                return null;
            ArrayList arrDis = new ArrayList();
           
            while (this.Reader.Read())
            {
                DiseaseType dis = SetDisType();
                arrDis.Add(dis);
            }
            this.Reader.Close();
            return arrDis;
        }

        #endregion

        #endregion

        #region 公共方法
        /// <summary>
        /// 病种实体插入
        /// </summary>
        /// <param name="disType">即将插入的病种实体</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertOrgType(Neusoft.HISFC.Models.Speciment.DiseaseType disType)
        {
            return this.UpdateDisType("Speciment.BizLogic.DisTypeManage.Insert", this.GetParam(disType));
           
        }

        /// <summary>
        /// 根据名称更新病种
        /// </summary>
        /// <param name="disType"></param>
        /// <returns></returns>
        public int UpdateDisType(DiseaseType disType)
        {
            return this.UpdateDisType("Speciment.BizLogic.DisTypeManage.Update", this.GetParam(disType));

        }

        /// <summary>
        /// 根据名称删除病种
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public int DeleteOrgTypeByName(string typeName)
        {
            return this.UpdateDisType("Speciment.BizLogic.DisTypeManage.Delete", new string[] { typeName });
        }

        /// <summary>
        /// 根据ID获取病种类型
        /// </summary>
        /// <param name="disID"></param>
        /// <returns></returns>
        public DiseaseType SelectDisByID(string disID)
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.DisTypeManage.SelectByID", ref sql) == -1)
                return null;
            sql = string.Format(sql, disID);
            if (this.ExecQuery(sql) == -1)
                return null;
            DiseaseType dis = new DiseaseType();
            while (this.Reader.Read())
            {
                dis = SetDisType();               
            }
            this.Reader.Close();
            return dis;
        }

        /// <summary>
        /// 加载所有的病种类型
        /// </summary>
        /// <returns>标本组织类型List</returns>
        public Dictionary<int, string> GetAllDisType()
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.DisTypeManage.SelectAll", ref sql) == -1)
                return null;

            if (this.ExecQuery(sql) == -1)
                return null;
            Dictionary<int, string> dicDisType = new Dictionary<int, string>();
            //ArrayList specList = new ArrayList();
            while (this.Reader.Read())
            {
                DiseaseType dis = SetDisType();
                if (dis.DiseaseName.Length == 2 || dis.DiseaseName == "内" || dis.DiseaseName == "放" || dis.DiseaseName.Length == 4)
                {
                   dicDisType.Add(dis.DisTypeID, dis.DiseaseName);
                }
            }
            this.Reader.Close();
            return dicDisType;
        }

        /// <summary>
        /// 加载所有的病种类型
        /// </summary>
        /// <returns>标本组织类型List</returns>
        public ArrayList GetAllValidDisType()
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.DisTypeManage.SelectAllValid", ref sql) == -1)
                return null;

            if (this.ExecQuery(sql) == -1)
                return null;
            Neusoft.HISFC.Models.Base.Const obj = null;
            ArrayList alDisType = new ArrayList();
            while (this.Reader.Read())
            {
                obj = new Neusoft.HISFC.Models.Base.Const();
                DiseaseType dis = SetDisType();
                if (dis != null)
                {
                    obj.ID = dis.DisTypeID.ToString();
                    obj.Name = dis.DiseaseName;
                    obj.SpellCode = dis.Ext1;
                    obj.WBCode = dis.Ext2;
                    obj.UserCode = dis.Ext3;
                    alDisType.Add(obj);
                }
            }
            this.Reader.Close();
            return alDisType;
        }

        /// <summary>
        /// 加载所有的病种类型
        /// </summary>
        /// <returns>标本组织类型List</returns>
        public ArrayList QueryAllDisType()
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.DisTypeManage.SelectAll", ref sql) == -1)
                return null;

            if (this.ExecQuery(sql) == -1)
                return null;
            ArrayList dicDisType = new ArrayList();
            //ArrayList specList = new ArrayList();
            while (this.Reader.Read())
            {
                DiseaseType dis = SetDisType();
                if (dis.DiseaseName.Length == 2 || dis.DiseaseName == "内" || dis.DiseaseName == "放")
                {
                    dicDisType.Add(dis);
                }
            }
            this.Reader.Close();
            return dicDisType;
        } 

        /// <summary>
        /// 加载组织的病种类型
        /// </summary>
        /// <returns>标本组织类型List</returns>
        public Dictionary<int, string> GetOrgDisType()
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.DisTypeManage.SelectOrg", ref sql) == -1)
                return null;

            if (this.ExecQuery(sql) == -1)
                return null;
            Dictionary<int, string> dicDisType = new Dictionary<int, string>();
            //ArrayList specList = new ArrayList();
            while (this.Reader.Read())
            {
                DiseaseType dis = SetDisType();
                dicDisType.Add(dis.DisTypeID, dis.DiseaseName);                
            }
            this.Reader.Close();
            return dicDisType;
        }


        /// <summary>
        /// 加载血的病种类型
        /// </summary>
        /// <returns>标本组织类型List</returns>
        public Dictionary<int, string> GetBldDisType()
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.DisTypeManage.SelectBld", ref sql) == -1)
                return null;

            if (this.ExecQuery(sql) == -1)
                return null;
            Dictionary<int, string> dicDisType = new Dictionary<int, string>();
            //ArrayList specList = new ArrayList();
            while (this.Reader.Read())
            {
                DiseaseType dis = SetDisType();
                dicDisType.Add(dis.DisTypeID, dis.DiseaseName);
            }
            this.Reader.Close();
            return dicDisType;
        }


        /// <summary>
        /// 根据boxId查找标本盒中存放的病种类型
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public DiseaseType GetDisTypeByBoxId(string boxId)
        {
            return this.GetDisType("Speciment.BizLogic.DisTypeManage.GetByBoxId", new string[] { boxId })[0] as DiseaseType;
        }

        /// <summary>
        /// 加载所有入院诊断对应的部位码名称
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetDiagToDis()
        {
            string sql = "select name, DISEASETYPEID from SPEC_DISEASETYPE, COM_DICTIONARY c where c.TYPE = 'DiagnosebyNurse' and c.INPUT_CODE = DISEASENAME";
            //if (this.Sql.GetSql("Neusoft.HISFC.BizLogic.Speciment.DisType.SelectAll", ref sql) == -1)
            //    return null;

            if (this.ExecQuery(sql) == -1)
                return null;
            Dictionary<string, int> dicDiaToDis = new Dictionary<string, int>();
            //ArrayList specList = new ArrayList();
            while (this.Reader.Read())
            {
                if (Reader["NAME"] != null && Reader["DISEASETYPEID"] != null)
                {
                    string diagName = Reader["NAME"].ToString();
                    if (Reader["DISEASETYPEID"].ToString().Trim() == "")
                    {
                        continue;
                    }
                    if (!dicDiaToDis.ContainsKey(diagName))
                    {
                        dicDiaToDis.Add(diagName,Convert.ToInt32(Reader["DISEASETYPEID"].ToString()));
                    }
                }
            }
            this.Reader.Close();
            return dicDiaToDis;
        }
        #endregion
    }
}
