using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 标本架规格管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-10]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-18' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class ShelfSpecManage : Neusoft.FrameWork.Management.Database
    {

        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">架子规格对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.ShelfSpec shelfSpec)
        {
            string[] str = new string[]
						{
							shelfSpec.ShelfSpecID.ToString(), 
							shelfSpec.Row.ToString(),
                            shelfSpec.Col.ToString(),
                            shelfSpec.Height.ToString(),
                            shelfSpec.ShelfSpecName,
                            shelfSpec.BoxSpec.BoxSpecID.ToString(),
                            shelfSpec.Comment
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
            sequence = this.GetSequence("Speciment.BizLogic.ShelfSpecManage.GetNextSequence");
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

        /// <summary>
        /// 读取架子规格实体信息
        /// </summary>
        /// <returns>架子规格实体</returns>
        private ShelfSpec SetShelf()
        {
            ShelfSpec spec = new ShelfSpec();
            try
            {
                spec.ShelfSpecID = Convert.ToInt32(this.Reader[0].ToString());
                spec.Row = Convert.ToInt32(this.Reader[1].ToString());
                spec.Col = Convert.ToInt32(this.Reader[2].ToString());
                spec.Height = Convert.ToInt32(this.Reader[3].ToString());
                spec.ShelfSpecName = this.Reader[4].ToString();
                if (null == this.Reader[5].ToString())
                {
                    spec.BoxSpec.BoxSpecID = 0;
                }
                else
                {
                    spec.BoxSpec.BoxSpecID = Convert.ToInt32(this.Reader[5].ToString());
                }
                //if (null == this.Reader["MARK"]) spec.Comment = "";
                //else
                //    spec.Comment = this.Reader["MARK"].ToString();
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return spec; 
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

        #region 更新架子规格操作
        /// <summary>
        /// 更新架子规格
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateShelfSpec(string sqlIndex, params string[] args)
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
        #endregion

        /// <summary>
        /// 根据ID获取架子的规格
        /// </summary>
        /// <param name="id">规格Id或者架子Id</param>
        /// <returns></returns>
        private ShelfSpec GetShelfByID(string sqlIndex, string id)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            sql = string.Format(sql, id);
            if (this.ExecQuery(sql) == -1)
                return null;
            ShelfSpec spec = new ShelfSpec();
            while (this.Reader.Read())
            {
                spec = SetShelf();
            }
            this.Reader.Close();
            return spec;
        }

        #endregion

        #region 公共方法
        /// <summary>
        /// 架子规格插入
        /// </summary>
        /// <param name="specBox">即将插入的架子规格</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertShelfSpec(Neusoft.HISFC.Models.Speciment.ShelfSpec shelfSpec)
        {
            return this.UpdateShelfSpec("Speciment.BizLogic.ShelfSpecManage.Insert", this.GetParam(shelfSpec));
        }

        /// <summary>
        /// 加载所有的架子规格
        /// </summary>
        /// <returns>标本架子List</returns>
        public Dictionary<int, string> GetAllShelfSpec()
        {
            string sql = "";
            if (this.Sql.GetSql("Speciment.BizLogic.ShelfSpecManage.SelectAll", ref sql) == -1)
                return null;
            if (this.ExecQuery(sql) == -1)
                return null;
            Dictionary<int, string> dicSpec = new Dictionary<int, string>();
            //ArrayList specList = new ArrayList();
            while (this.Reader.Read())
            {
                ShelfSpec spec = SetShelf();
                dicSpec.Add(spec.ShelfSpecID, spec.ShelfSpecName + "  规格：" + spec.Row.ToString() + "×" + spec.Height.ToString());
                //specList.Add(spec); 
            }
            this.Reader.Close();
            return dicSpec;

        }

        /// <summary>
        /// 根据ID获取架子的规格
        /// </summary>
        /// <param name="specID"></param>
        /// <returns></returns>
        public ShelfSpec GetShelfByID(string specID)
        {
            return this.GetShelfByID("Speciment.BizLogic.ShelfSpecManage.SelectByID", specID);
        }

        /// <summary>
        /// 根据规格Id获取架子规格
        /// </summary>
        /// <param name="shelfId"></param>
        /// <returns></returns>
        public ShelfSpec GetShelfByShelf(string shelfId)
        {
            return this.GetShelfByID("Speciment.BizLogic.ShelfSpecManage.ByShelfID", shelfId);
        }
        #endregion
    
    }
}
