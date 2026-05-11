using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 标本入库管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-12-01]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-19' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecInManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中

        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specIn">入库标本ID</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.SpecIn specIn)
        {
            //string sequence = "";

            //GetNextSequence(ref sequence);
            string[] str = new string[]
						{
							specIn.InId.ToString(),
                            specIn.InTime.ToString(),
                            specIn.OperId,
                            specIn.OperName,
                            specIn.SubSpecId.ToString(),
                            specIn.TypeId.ToString(),
                            specIn.SpecTypeId.ToString(),
                            specIn.Count.ToString(),
                            specIn.Unit,
                            specIn.RelateId.ToString(),                        
                            specIn.BoxId.ToString(),
                            specIn.Row.ToString(),
                            specIn.Col.ToString(),
                            specIn.Comment,
                            specIn.BoxBarCode,
                            specIn.Oper,
                            specIn.SubSpecBarCode
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
            sequence = this.GetSequence("Speciment.BizLogic.SpecInManage.GetNextSequence");
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
        /// 更新入库信息
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateSpecIn(string sqlIndex, params string[] args)
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
        /// 读取入库信息
        /// </summary>
        /// <returns>入库实体</returns>
        private SpecIn SetSubSpecIn()
        {
            SpecIn subSpecIn = new SpecIn();
            try
            {
                subSpecIn.InId = Convert.ToInt32(this.Reader["INID"].ToString());
                subSpecIn.InTime = Convert.ToDateTime(this.Reader["INDATE"].ToString());
                subSpecIn.OperId = this.Reader["OPERATORID"].ToString();
                subSpecIn.OperName = this.Reader["OPERATORNAME"].ToString();
                subSpecIn.SubSpecId = Convert.ToInt32(this.Reader["SUBSPECID"].ToString());
                subSpecIn.TypeId = Convert.ToInt32(this.Reader["TYPEID"].ToString());
                subSpecIn.SpecTypeId = Convert.ToInt32(this.Reader["SPECTYPEID"].ToString());
                subSpecIn.Count = Convert.ToDecimal(this.Reader["SPECCOUNT"].ToString());
                subSpecIn.RelateId = Convert.ToInt32(this.Reader["RELATEID"].ToString());
                subSpecIn.Unit = this.Reader["UNIT"].ToString();
                subSpecIn.Col = Convert.ToInt32(this.Reader["BOXCOL"].ToString());
                subSpecIn.Row = Convert.ToInt32(this.Reader["BOXROW"].ToString());
                subSpecIn.BoxId = Convert.ToInt32(this.Reader["BOXID"].ToString());
                subSpecIn.BoxBarCode = this.Reader["BOXBARCODE"].ToString();
                subSpecIn.Oper = this.Reader["OPER"].ToString();
                subSpecIn.Comment = this.Reader["MARK"].ToString();
                subSpecIn.SubSpecBarCode = this.Reader["SUBSPECBARCODE"].ToString();
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return subSpecIn;
        }

        /// <summary>
        /// 根据索引获取符合条件的入库标本
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList GetSubSpecIn(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";
                return null;
            }
            if (this.ExecQuery(sql, args) == -1)
                return null;
            ArrayList arrSubSpecIn = new ArrayList();
            while (this.Reader.Read())
            {
                SpecIn inTmp = SetSubSpecIn();
                arrSubSpecIn.Add(inTmp);
            }
            Reader.Close();
            return arrSubSpecIn;
        }

        #endregion

        #endregion

        #region 公共方法

        /// <summary>
        /// 入库标本插入
        /// </summary>
        /// <param name="specIn">即将标本</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertSubSpecIn(Neusoft.HISFC.Models.Speciment.SpecIn specIn)
        {
            return this.UpdateSpecIn("Speciment.BizLogic.SpecInManage.Insert", this.GetParam(specIn));

        }

        /// <summary>
        /// 直接根据sql语句更新记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int UpdateSpecIn(string sql)
        {
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int QuerySpecIn(string sql, ref DataSet ds)
        {
            return this.ExecQuery(sql, ref ds);
        }

        /// <summary>
        /// 查询一个标本盒当中的入库标本列表
        /// </summary>
        /// <param name="boxId">标本盒ID</param>
        /// <returns>标本列表</returns>
        public ArrayList GetInSpecInBox(string boxId)
        {
            return this.GetSubSpecIn("Speciment.BizLogic.SpecInManage.InOneBox", new string[] { boxId });
        }

        public int DeleteById(string inId)
        {
            return this.UpdateSpecIn("Speciment.BizLogic.SpecInManage.DeleteById", new string[] { inId });
        }

        #endregion
    }
}
