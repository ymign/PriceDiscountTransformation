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
    /// [功能描述: 病人病案信息管理]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-26]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-30' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class BaseManage : Neusoft.FrameWork.Management.Database
    {
        #region 设置参数数组
        /// <summary>
        /// 实体类的属性放入数组中
        /// </summary>
        /// <param name="patient">标本库病人实体</param>
        /// <returns></returns>
        private string[] GetParam(Neusoft.HISFC.Models.Speciment.SpecBase specBase)
        {            
            string[] str = new string[]
						{
							specBase.BaseID.ToString(),
                            specBase.InBaseTime.ToString(),
                            specBase.SpecSource.SpecId.ToString(),
                            specBase.HCV_AB,
                            specBase.HbSAG,
                            specBase.Hiv_AB,
                            specBase.RHBlood,
                            specBase.X_Times,
                            specBase.MR_Times,
                            specBase.DSA_Times,
                            specBase.PET_Times,
                            specBase.ECT_Times,
                            specBase.OutDiaICD,
                            specBase.OutDiaName,
                            specBase.Main_DiagState,
                            specBase.Diagnose_Oper_Flag,
                            specBase.Is30Disease,
                            specBase.InDiaICD,
                            specBase.InDiaName,
                            specBase.CliDiagICD,
                            specBase.CliDiagName,
                            specBase.MainDiaICD,
                            specBase.MainDiaName,
                            specBase.MainDiaICD1,
                            specBase.MainDiaName1,
                            specBase.MainDiaICD2,
                            specBase.MainDiagName2,
                            specBase.ModICD,
                            specBase.ModName,
                            specBase.Comment
                        };
            return str;
        }
        #endregion

        #region 获取参数
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
            sequence = this.GetSequence("Speciment.BizLogic.BaseManage.GetNextSequence");
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
        #endregion

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

        #region 更新病案
        /// <summary>
        /// 更新病人
        /// </summary>
        /// <param name="sqlIndex">sql索引</param>
        /// <param name="args"></param>
        /// <returns></returns>
        private int UpdateBase(string sqlIndex, params string[] args)
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

        private SpecBase SetBaseInfo()
        {
            try
            {
                SpecBase specBase = new SpecBase();
                if (!Reader.IsDBNull(0)) specBase.BaseID = Convert.ToInt32(this.Reader["BASEID"].ToString());
                if (!Reader.IsDBNull(1)) specBase.InBaseTime = Convert.ToDateTime(this.Reader["INBASETIME"].ToString());
                if (!Reader.IsDBNull(2)) specBase.SpecSource.SpecId = Convert.ToInt32(this.Reader["SPECID"].ToString());
                if (!Reader.IsDBNull(3)) specBase.HCV_AB = this.Reader["HCV_AB"].ToString();
                if (!Reader.IsDBNull(4)) specBase.HbSAG = this.Reader["HBSAG"].ToString();
                if (!Reader.IsDBNull(5)) specBase.Hiv_AB = this.Reader["HIV_AB"].ToString();
                if (!Reader.IsDBNull(6)) specBase.RHBlood = this.Reader["RH_BLOOD"].ToString();
                if (!Reader.IsDBNull(7)) specBase.X_Times = this.Reader["X_TIMES"].ToString();
                if (!Reader.IsDBNull(8)) specBase.MR_Times = this.Reader["MR_TIMES"].ToString();
                if (!Reader.IsDBNull(9)) specBase.DSA_Times = this.Reader["DSA_TIMES"].ToString();
                if (!Reader.IsDBNull(10)) specBase.PET_Times = this.Reader["PET_TIMES"].ToString();
                if (!Reader.IsDBNull(11)) specBase.ECT_Times = this.Reader["ECT_TIMES"].ToString();
                if (!Reader.IsDBNull(12)) specBase.OutDiaICD = this.Reader["M_DIAGICD"].ToString();
                if (!Reader.IsDBNull(13)) specBase.OutDiaName = this.Reader["M_DIAGICDNAME"].ToString();
                if (!Reader.IsDBNull(14)) specBase.Main_DiagState = this.Reader["MAIN_DIAGSTATE"].ToString();
                if (!Reader.IsDBNull(15)) specBase.Diagnose_Oper_Flag = this.Reader["DIAG_OPER_FLAG"].ToString();
                if (!Reader.IsDBNull(16)) specBase.Is30Disease = this.Reader["IS30DISEASE"].ToString();
                if (!Reader.IsDBNull(17)) specBase.InDiaICD = this.Reader["INHOS_DIACODE"].ToString();
                if (!Reader.IsDBNull(18)) specBase.InDiaName = this.Reader["INHOS_DIANAME"].ToString();
                if (!Reader.IsDBNull(19)) specBase.CliDiagICD = this.Reader["CILINIC_DIACODE"].ToString();
                if (!Reader.IsDBNull(20)) specBase.CliDiagName = this.Reader["CLINIC_DIANAME"].ToString();
                if (!Reader.IsDBNull(21)) specBase.MainDiaICD = this.Reader["MAIN_DIACODE"].ToString();
                if (!Reader.IsDBNull(22)) specBase.MainDiaName = this.Reader["MAIN_DIANAME"].ToString();
                if (!Reader.IsDBNull(23)) specBase.MainDiaICD1 = this.Reader["MAIN_DIACODE1"].ToString();
                if (!Reader.IsDBNull(24)) specBase.MainDiaName1 = this.Reader["MAIN_DIANAME1"].ToString();
                if (!Reader.IsDBNull(25)) specBase.MainDiaICD2 = this.Reader["MAIN_DIACODE2"].ToString();
                if (!Reader.IsDBNull(26)) specBase.MainDiagName2 = this.Reader["MAIN_DIANAME2"].ToString();
                if (!Reader.IsDBNull(27)) specBase.ModICD = this.Reader["MOD_ICD"].ToString();
                if (!Reader.IsDBNull(28)) specBase.ModName = this.Reader["MOD_NAME"].ToString();
                if (!Reader.IsDBNull(29)) specBase.Comment = this.Reader["MARK"].ToString();
                return specBase;
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return null;
            }
 
        }
        #endregion

        #region 标本病案操作
        /// <summary>
        /// 保存病案信息
        /// </summary>
        /// <param name="specBase"></param>
        /// <returns></returns>
        public int InsertBase(SpecBase specBase)
        {
            try
            {
                return UpdateBase("Speciment.BizLogic.BaseManage.Insert", GetParam(specBase));

            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }
        #endregion

        #region 更新病案诊断信息
        /// <summary>
        /// 更新病案信息
        /// </summary>
        /// <param name="specBase"></param>
        /// <returns></returns>
        public int UpdateBase(SpecBase specBase)
        {
            try
            {
                return UpdateBase("Speciment.BizLogic.BaseManage.Update", GetParam(specBase));

            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
        }

        #endregion

        /// <summary>
        /// 根据sql语句查询诊断
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList GetBaseInfo(string sql)
        {
            if (this.ExecQuery(sql) == -1)
                return null;
            ArrayList arrBaseInfo = new ArrayList();
            while (this.Reader.Read())
            {
                SpecBase baseInfo = SetBaseInfo();
                arrBaseInfo.Add(baseInfo);
            }
            Reader.Close();
            return arrBaseInfo;
        }

        /// <summary>
        /// 获取标本库中没有录入诊断的标本
        /// </summary>
        /// <returns></returns>
        public DataSet GetNotInBaseInfo()
        {
            DataSet ds = new DataSet();
            this.ExecQuery("Speciment.BizLogic.BaseManage.GetDiagnoseFromBase", ref ds, new string[] { });
            return ds;
        }

        /// <summary>
        /// 根据时间获取标本库中没有录入诊断的标本
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public DataSet GetNotInBaseByTime(string start, string end)
        {
            DataSet ds = new DataSet();
            this.ExecQuery("Speciment.BizLogic.BaseManage.GetDiagnoseFromBase.ByTimeStamp", ref ds, new string[] { start, end });
            return ds;
        }

        /// <summary>
        /// 根据时间获取标本库中没有录入诊断的标本
        /// </summary>
        /// <param name="inBase">是否录入诊断信息</param>
        /// <param name="strat"></param>
        /// <param name="end"></param>
        /// <param name="ds"></param>
        public void GetNotInBaseInfo(string inBase, string strat ,string end, ref DataSet ds)
        {
            this.ExecQuery("Speciment.BizLogic.BaseManage.GetNoDiagnose", ref ds, new string[] { inBase, strat, end }); 
        }

        /// <summary>
        /// 根据病历号获取诊断信息
        /// </summary>
        /// <param name="patientNo">住院号</param>
        /// <param name="ds"></param>
        public DataTable GetDiagnoseFromDiagnose(string patientNo)
        {
            DataSet ds = new DataSet();
            this.ExecQuery("Speciment.BizLogic.BaseManage.GetDiagnoseFromDiagnose", ref ds, new string[] { patientNo });
            if (ds == null || ds.Tables.Count <= 0)
            {
                return new DataTable();
            }
            else
            {
                return ds.Tables[0];
            }
        }
    }
}
