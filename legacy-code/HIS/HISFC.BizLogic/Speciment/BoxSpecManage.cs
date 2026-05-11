using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Neusoft.HISFC.Models.Speciment;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 标本盒规格管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-10]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-30' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class BoxSpecManage : Neusoft.FrameWork.Management.Database
    {        
        #region 私有方法

        #region 实体类的属性放入数组中
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">标本盒规格对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.BoxSpec spec)
        {
            string[] str = new string[]
						{
							spec.BoxSpecID.ToString(), 
							spec.BoxSpecName,
                            spec.Row.ToString(),
                            spec.Col.ToString(),
                            spec.Comment
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
            sequence = this.GetSequence("Speciment.BizLogic.BoxSpecManage.GetNextSequence");
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

        #region 更新架子规格操作
        /// <summary>
        /// 更新标本盒规格
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateBoxSpec(string sqlIndex, params string[] args)
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
        /// 读取标本规格实体信息
        /// </summary>
        /// <returns>标本规格实体</returns>
        private BoxSpec SetSpecBox()
        {
            BoxSpec spec = new BoxSpec();
            try
            {
                spec.BoxSpecID = Convert.ToInt32(this.Reader[0].ToString());
                spec.Row = Convert.ToInt32(this.Reader["SPECROW"].ToString());
                spec.Col = Convert.ToInt32(this.Reader["SPECCOL"].ToString());
                spec.BoxSpecName = this.Reader["SPECNAME"].ToString();
                spec.Comment = this.Reader["MARK"].ToString();
                spec.Capacity = Convert.ToInt32(this.Reader["CAPACITY"].ToString());
                spec.OccupyCount = Convert.ToInt32(this.Reader["OCCUPYCOUNT"].ToString());
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return spec; 
        }

        /// <summary>
        /// 根据索引查询标本盒的规格
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        private ArrayList SelectSpec(string sqlIndex, string[] parms)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            if (this.ExecQuery(sql,parms) == -1)
                return null;     
            ArrayList specList = new ArrayList();
            while (this.Reader.Read())
            {
                BoxSpec spec = SetSpecBox();
                specList.Add(spec); 
            }
            this.Reader.Close();
            return specList;
        }

        #endregion

        #endregion

        #region 公共方法
        /// <summary>
        /// 标本盒规格插入
        /// </summary>
        /// <param name="specBox">即将插入的标本盒规格</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertBoxSpec(Neusoft.HISFC.Models.Speciment.BoxSpec spec)
        {
            // return this.UpdateSingleTable("Fee.OutPatient.DayBalance.Insert", this.GetDayBalanceParams(dayBalance));
            return this.UpdateBoxSpec("Speciment.BizLogic.BoxSpecManage.Insert", this.GetParam(spec));
        
        }

        /// <summary>
        /// 加载所有的标本规格
        /// </summary>
        /// <returns>标本规格List</returns>
        public Dictionary<int,string> GetAllBoxSpec()
        {
            ArrayList arrAllSpec = new ArrayList();
            arrAllSpec = this.SelectSpec("Speciment.BizLogic.BoxSpecManage.SelectAll", new string[] { });
            Dictionary<int, string> dicSpec = new Dictionary<int, string>();
            foreach (BoxSpec spec in arrAllSpec)
            {
                dicSpec.Add(spec.BoxSpecID, spec.BoxSpecName + " " + spec.Row.ToString() + "×" + spec.Col.ToString());
            }
            return dicSpec;
            
        }

        /// <summary>
        /// 根据标本盒ID获取标本盒规格
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public BoxSpec GetSpecByBoxId(string boxId)
        {
            ArrayList arrSpec = new ArrayList();
            BoxSpec boxSpec = new BoxSpec();
            arrSpec = this.SelectSpec("Speciment.BizLogic.BoxSpecManage.SelectbySpecId", new string[] { boxId });
            foreach (BoxSpec spec in arrSpec)
            {
                boxSpec = spec;
                break;
            }
            return boxSpec;
        }

        /// <summary>
        /// 根据ID获取盒子规格
        /// </summary>
        /// <param name="specID">标本盒的规格ID</param>
        /// <returns></returns>
        public BoxSpec GetSpecById(string specID)
        {
            ArrayList arrSpec = new ArrayList();
            BoxSpec boxSpec = new BoxSpec();
            arrSpec = this.SelectSpec("Speciment.BizLogic.BoxSpecManage.SelectbyId", new string[] { specID });
            foreach (BoxSpec spec in arrSpec)
            {
                boxSpec = spec;
                break;
            }
            return boxSpec;
        }

        #endregion
    }
}
