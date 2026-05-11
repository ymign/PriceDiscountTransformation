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
    /// [功能描述: 标本申请明细管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-10]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-10-18' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecApplyOutManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中

        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specOut">出库标本ID</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.SpecOut specOut)
        {
            //string sequence = "";

            //GetNextSequence(ref sequence);
            string[] str = new string[]
						{
							specOut.OutId.ToString(),
                            specOut.OutDate.ToString(),
                            specOut.OperId,
                            specOut.OperName,
                            specOut.SubSpecId.ToString(),
                            specOut.TypeId.ToString(),
                            specOut.SpecTypeId.ToString(),
                            specOut.Count.ToString(),
                            specOut.Unit,
                            specOut.RelateId.ToString(),
                            specOut.BoxBarCode,
                            specOut.BoxCol.ToString(),
                            specOut.BoxId.ToString(),
                            specOut.BoxRow.ToString(),
                            specOut.Comment,
                            specOut.Oper,
                            specOut.SubSpecBarCode,
                            specOut.SpecId.ToString(),
                            specOut.IsOut,
                            specOut.IsRetuanAble
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
            sequence = this.GetSequence("Speciment.BizLogic.SpecApplyOutManage.GetNextSequence");
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
        /// 更新出库信息
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateSpecOut(string sqlIndex, params string[] args)
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
        /// 读取出库信息
        /// </summary>
        /// <returns>出库实体</returns>
        private SpecOut SetSubSpecOut()
        {
            SpecOut subSpecOut = new SpecOut();
            try
            {
                subSpecOut.OutId = Convert.ToInt32(this.Reader["APPLYID"].ToString());
                subSpecOut.OutDate = Convert.ToDateTime(this.Reader["APPLYDATE"].ToString());
                subSpecOut.OperId = this.Reader["OPERATORID"].ToString();
                subSpecOut.OperName = this.Reader["OPERATORNAME"].ToString();
                subSpecOut.SubSpecId = Convert.ToInt32(this.Reader["SUBSPECID"].ToString());
                subSpecOut.TypeId = Convert.ToInt32(this.Reader["TYPEID"].ToString());
                subSpecOut.SpecTypeId = Convert.ToInt32(this.Reader["SPECTYPEID"].ToString());
                subSpecOut.Count = Convert.ToDecimal(this.Reader["SPECCOUNT"].ToString());
                subSpecOut.RelateId = Convert.ToInt32(this.Reader["RELATEID"].ToString());
                subSpecOut.Unit = this.Reader["UNIT"].ToString();
                subSpecOut.BoxCol = Convert.ToInt32(this.Reader["BOXCOL"].ToString());
                subSpecOut.BoxRow = Convert.ToInt32(this.Reader["BOXROW"].ToString());
                subSpecOut.BoxId = Convert.ToInt32(this.Reader["BOXID"].ToString());
                subSpecOut.BoxBarCode = this.Reader["BOXBARCODE"].ToString();
                subSpecOut.Oper = this.Reader["OPER"].ToString();
                subSpecOut.Comment = this.Reader["MARK"].ToString();
                subSpecOut.SubSpecBarCode = this.Reader["SUBSPECBARCODE"].ToString();
                subSpecOut.SpecId = Convert.ToInt32(this.Reader["SPECID"].ToString());
                subSpecOut.IsOut = this.Reader["ISOUT"].ToString();
                subSpecOut.IsRetuanAble = this.Reader["ISRETURNABLE"].ToString();
                 
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return subSpecOut;
        }

        /// <summary>
        /// 根据索引获取符合条件的出库标本
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList GetSubSpecOut(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";
                return null;
            }
            if (this.ExecQuery(sql, args) == -1)
                return null;
            ArrayList arrSubSpecOut = new ArrayList();
            while (this.Reader.Read())
            {
                SpecOut outTmp = SetSubSpecOut();
                arrSubSpecOut.Add(outTmp);
            }
            return arrSubSpecOut;
        }

        #endregion

        #endregion

        #region 公共方法

        /// <summary>
        /// 出库标本插入
        /// </summary>
        /// <param name="SpecOut">即将标本</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertSubSpecApplyOut(Neusoft.HISFC.Models.Speciment.SpecOut specOut)
        {
            return this.UpdateSpecOut("Speciment.BizLogic.SpecApplyOutManage.Insert", this.GetParam(specOut));

        }
        /// <summary>
        /// 直接根据sql语句更新记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int UpdateSpecOut(string sql)
        {
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int QuerySpecOut(string sql, ref DataSet ds)
        {
            return this.ExecQuery(sql, ref ds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="applyNum"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Speciment.SpecOut GetBySpecId(string specId, string applyNum)
        {
            return this.GetSubSpecOut("Speciment.BizLogic.SpecApplyOutManage.GetApplyOutBySubspecIdAndRelateId", new string[] { specId, applyNum })[0] as SpecOut;
        }

        /// <summary>
        /// 根据对应申请ID找已筛选保存出库记录
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public ArrayList GetSubSpecOut(string applyId)
        {
            return this.GetSubSpecOut("Speciment.BizLogic.SpecApplyOutManage.ByRELATEID", new string[] { applyId });
        }
        #endregion
    }

}
