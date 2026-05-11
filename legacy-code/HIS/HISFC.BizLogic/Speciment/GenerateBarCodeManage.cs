using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Speciment;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Speciment
{
    /// <summary>
    /// Speciment <br></br>
    /// [功能描述: 原标本的条码生产管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-01-11]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-30' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class GenerateBarCodeManage : Neusoft.FrameWork.Management.Database
    {
        #region 私有方法

        #region 实体类的属性放入数组中
       
        private string[] GetParam(SourceCode code)
        {
            string[] str = new string[] 
                        { 
                            code.BarCodeID.ToString(),
                            code.BarCode,
                            code.InPatientNo
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

        #region 更新操作
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sqlIndex">sql语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateBarCode(string sqlIndex, params string[] args)
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
        /// 读取条码信息
        /// </summary>
        /// <returns>条码实体</returns>
        private SourceCode SetSourceCode()
        {
            SourceCode code = new SourceCode();
            try
            {
                code.BarCodeID = Convert.ToInt32(this.Reader[0].ToString());
                code.BarCode = this.Reader[1].ToString();
                code.InPatientNo = this.Reader[2].ToString();
            }
            catch (Exception e)
            {
                this.ErrCode = e.Message;
                this.Err = e.Message;
                return null;
            }
            return code;
        }

        /// <summary>
        /// 获取条码列表
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        private ArrayList GetSourceCode(string sqlIndex, params string[] parm)
        {
            string sql = "";
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
                return null;
            sql = string.Format(sql, parm);
            if (this.ExecQuery(sql) == -1)
                return null;
            ArrayList arrCode = new ArrayList();

            while (this.Reader.Read())
            {
                SourceCode c = SetSourceCode();
                arrCode.Add(c);
            }
            this.Reader.Close();
            return arrCode;
        }

        #endregion

        #endregion

        #region 公共方法
        /// <summary>
        /// 病种实体插入
        /// </summary>
        /// <param name="disType">即将插入的病种实体</param>
        /// <returns>影响的行数；－1－失败</returns>
        public int InsertBarCode(Neusoft.HISFC.Models.Speciment.SourceCode code)
        {
            return this.UpdateBarCode("Speciment.BizLogic.BarCodeManage.SouceBarCode", this.GetParam(code));
        }
        #endregion
    }
}
