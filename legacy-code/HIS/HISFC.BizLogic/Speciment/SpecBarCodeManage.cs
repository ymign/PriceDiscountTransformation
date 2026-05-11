using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Neusoft.HISFC.Models.Speciment;

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
    public class SpecBarCodeManage : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="specBox">标本组织类型对象</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.SpecBarCode specBarCode)
        {
            string[] str = new string[]
                           {
                               specBarCode.DisType,
                               specBarCode.DisAbrre,
                               specBarCode.SpecType,
                               specBarCode.SpecTypeAbrre,
                               specBarCode.Sequence,
                               specBarCode.Other,
                               specBarCode.OrgOrBld
                           };
            return str;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        private Neusoft.HISFC.Models.Speciment.SpecBarCode SetBarCode()
        {
            Neusoft.HISFC.Models.Speciment.SpecBarCode barCode = new Neusoft.HISFC.Models.Speciment.SpecBarCode();
            try
            {
                barCode.DisType = this.Reader["DISEASETYPE"].ToString();
                barCode.DisAbrre = this.Reader["DISABRRE"].ToString();
                barCode.SpecType = this.Reader["SPECTYPE"].ToString();
                barCode.SpecTypeAbrre = this.Reader["SPECTYPEABRRE"].ToString();
                barCode.Sequence = this.Reader["SEQ"].ToString();
                barCode.Other = this.Reader["OTHER"].ToString();
                barCode.OrgOrBld = this.Reader["ORGORBLD"].ToString();
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
            return barCode;
        }

        /// <summary>
        /// 更新条码
        /// </summary>
        /// <param name="sqlIndex">sql索引</param>
        /// <param name="args">参数</param>
        /// <returns>影响的记录条数</returns>
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
        /// 根据索引获取符合条件的条码序列号
        /// </summary>
        /// <param name="sqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList GetSubBarCode(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return null;
            }
            if (this.ExecQuery(sql, args) == -1)
                return null;
            ArrayList arrSubBarCode = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Speciment.SpecBarCode tmp = SetBarCode();
                arrSubBarCode.Add(tmp);
            }
            this.Reader.Close();
            return arrSubBarCode;
        }

        /// <summary>
        /// 更新对应的病种,对应的类型 的条码序号
        /// </summary>
        /// <param name="disType">病种</param>
        /// <param name="specType">标本类型</param>
        /// <param name="sequence">条码序号</param>
        /// <returns></returns>
        public int UpdateBarCode(string disType, string specType, string sequence)
        {
            return this.UpdateBarCode("Speciment.BizLogic.SpecBarCodeManage.Update", new string[] { disType, specType, sequence });
        }

        public Neusoft.HISFC.Models.Speciment.SpecBarCode GetSpecBarCode(string disType, string specType)
        {
            ArrayList arr = this.GetSubBarCode("Speciment.BizLogic.SpecBarCodeManage.GetBarCode", new string[] { disType, specType });
            if (arr == null || arr.Count <= 0)
            {
                return null;
            }
            return arr[0] as Neusoft.HISFC.Models.Speciment.SpecBarCode;
        }

        /// <summary>
        /// 根据病种和标本类型大类取最大序列号
        /// </summary>
        /// <param name="disTye">病种类型</param>
        /// <param name="orgType">标本类型大类，血，or组织</param>
        /// <returns></returns>
        public string GetMaxSeqByDisAndType(string disType, string orgType)
        {
            string sql = "select max(seq) from SPEC_SUBBARCODE where ORGORBLD = '" + orgType + "' and DISEASETYPE = '" + disType + "'";
            string sequence = this.ExecSqlReturnOne(sql);
            try
            {
                return sequence;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 更新seq
        /// </summary>
        /// <param name="disType"></param>
        /// <param name="orgType"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        public int UpdateMaxSeqByDisAndType(string disType, string orgType ,string seq)
        {
            string sql = "UPDATE SPEC_SUBBARCODE SET seq = '" + seq + "' where ORGORBLD = '" + orgType + "' and DISEASETYPE = '" + disType + "'";
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 获取一种病种的所有标本类型的信息
        /// </summary>
        /// <returns></returns>
        public ArrayList GetAllSpecTypeByDisType()
        {
            ArrayList arr = this.GetSubBarCode("Speciment.BizLogic.SpecBarCodeManage.GetAllSpecTypeByDistype", new string[] { });
            if (arr == null || arr.Count <= 0)
            {
                return null;
            }
            return arr;
        }

        public ArrayList GetAllDisTypeBySpecType()
        {
            ArrayList arr = this.GetSubBarCode("Speciment.BizLogic.SpecBarCodeManage.GetAllDisTypeBySpectype", new string[] { });
            if (arr == null || arr.Count <= 0)
            {
                return null;
            }
            return arr;
        }

        /// <summary>
        ///插入新纪录
        /// </summary>
        /// <param name="bar"></param>
        /// <returns></returns>
        public int InsertBarCode(SpecBarCode bar)
        {
            return this.UpdateBarCode("Speciment.BizLogic.SpecBarCodeManage.Insert", GetParam(bar));
        }
    }
}
