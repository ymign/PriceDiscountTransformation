using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Neusoft.HISFC.BizLogic.RADT
{
    public class SISelfInpatient : Neusoft.FrameWork.Management.Database
    {
        public SISelfInpatient()
        {

        }


        /// <summary>
        /// 获取自助出院待办的住院患者信息
        /// </summary>
        /// <param name="operCode">操作人编码</param>
        /// <param name="parDeptCode">父级科室部门编码</param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.RADT.SISelfDealRecordDto> PatientQueryForSelfHelpOut(string operCode, string parDeptCode)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.PatientQueryByFeeDept.2", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            List<Neusoft.HISFC.Models.RADT.SISelfDealRecordDto> list = new List<Neusoft.HISFC.Models.RADT.SISelfDealRecordDto>();
            try
            {
                sql = string.Format(sql, operCode, parDeptCode);
                if (this.ExecQuery(sql) == -1) return null;

                Neusoft.HISFC.Models.RADT.SISelfDealRecordDto record = null;
                while (this.Reader.Read())
                {
                    record = new Neusoft.HISFC.Models.RADT.SISelfDealRecordDto()
                    {
                        InpatientNo = this.Reader[0].ToString(),
                        PatientNo = this.Reader[1].ToString(),
                        PatientName = this.Reader[2].ToString(),
                        Dept = new Neusoft.FrameWork.Models.NeuObject()
                        {
                            ID = this.Reader[3].ToString(),
                            Name = this.Reader[4].ToString()
                        },
                        PayKindCode = this.Reader[5].ToString(),
                        Pact = new Neusoft.FrameWork.Models.NeuObject()
                        {
                            ID = this.Reader[6].ToString(),
                            Name = this.Reader[7].ToString(),
                        },
                        DealRecord = new Neusoft.HISFC.Models.RADT.SISelfDealRecord()
                        {
                            InpatientNo = this.Reader[0].ToString(),
                            ReceiveOperCode = this.Reader[8].ToString(),
                            ReceiveDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[9].ToString()),
                            ReceiveState = this.Reader[10].ToString(),
                            RevokeOperCode = this.Reader[11].ToString(),
                            RevokeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[12].ToString()),
                            RevokeReason = this.Reader[13].ToString(),
                            PaseFlag = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[14].ToString()),
                            PaseOperCode = this.Reader[15].ToString(),
                            PaseDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[16].ToString()),
                            RevokeType = this.Reader[17].ToString()
                        },
                        IsSIBalanced = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[18].ToString()),
                        DateIn = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[19].ToString()),
                        DateOut = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[20].ToString()),
                        PathologyPassFlag = this.Reader[21].ToString(),
                        InState = this.Reader[22].ToString(),
                    };
                    list.Add(record);
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                list = null;
            }
            finally
            {
                if (!this.Reader.IsClosed) this.Reader.Close();
            }

            return list;
        }

        /// <summary>
        /// 新增自助出院待办处理记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public int AddSISelfDealRecord(Neusoft.HISFC.Models.RADT.SISelfDealRecord record)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.InsertSISelfDealRecord", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            sql = string.Format(sql, record.InpatientNo,
                record.ReceiveOperCode, record.ReceiveDate.ToString("yyyy-MM-dd HH:mm:ss"), record.ReceiveState,
                record.RevokeOperCode, record.RevokeDate.ToString("yyyy-MM-dd HH:mm:ss"), record.RevokeReason,
                record.PaseFlag ? "1" : "0", record.PaseOperCode, record.PaseDate.ToString("yyyy-MM-dd HH:mm:ss"));

            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 自助出院待办接单处理
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="receiveOperCode">接单操作人编码</param>
        /// <returns></returns>
        public int ReceiveSISelfDealRecord(string inpatientNo, string receiveOperCode)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.ReciveSISelfDealRecord", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            sql = string.Format(sql, inpatientNo, receiveOperCode);
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 自助出院待办撤单处理
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="revokeOperCode">撤单操作人</param>
        /// <param name="revokeReason">撤单类型</param>
        /// <returns></returns>
        public int RevokeSISelfDealRecord(string inpatientNo, string revokeOperCode, string revokeReason, string revokeType)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.RevokeSISelfDealRecord.2", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            try
            {
                sql = string.Format(sql, inpatientNo, revokeOperCode, revokeReason, revokeType);
                return this.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 根据住院流水号获取自助出院待办处理记录
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.RADT.SISelfDealRecord QuerySISelfDealRecord(string inpatientNo)
        {
            string sqlWhere = "";
            string sql = this.GetSelectSIDealRecordSql();
            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.Where.ByInpatientNo", ref sqlWhere) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            sql = sql + string.Format(sqlWhere, inpatientNo);
            var list = this.QuerySISelfDealRecordBySql(sql);

            if (list == null)
                return null;
            else if (list.Count == 0)
                return new Neusoft.HISFC.Models.RADT.SISelfDealRecord();
            else
                return list[0];
        }

        /// <summary>
        /// 获取查询所有字段的sql
        /// </summary>
        /// <returns></returns>
        private string GetSelectSIDealRecordSql()
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.QuerySISelfDealRecord.WithRevokeType", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            return sql;
        }

        /// <summary>
        /// 获取医保自助出院待办处理记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.RADT.SISelfDealRecord> QuerySISelfDealRecordBySql(string sql)
        {
            var list = new List<Neusoft.HISFC.Models.RADT.SISelfDealRecord>();
            try
            {
                if (this.ExecQuery(sql) == -1)
                {
                    return null;
                }

                Neusoft.HISFC.Models.RADT.SISelfDealRecord record = null;
                while (this.Reader.Read())
                {
                    record = new Neusoft.HISFC.Models.RADT.SISelfDealRecord()
                    {
                        InpatientNo = this.Reader[0].ToString(),
                        ReceiveOperCode = this.Reader[1].ToString(),
                        ReceiveDate =  Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[2].ToString()),
                        ReceiveState = this.Reader[3].ToString(),
                        RevokeOperCode = this.Reader[4].ToString(),
                        RevokeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[5].ToString()),
                        RevokeReason = this.Reader[6].ToString(),
                        PaseFlag = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[7].ToString()),
                        PaseOperCode = this.Reader[8].ToString(),
                        PaseDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[9].ToString()),
                        RevokeType = this.Reader[10].ToString()
                    };
                    list.Add(record);
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                list = null;
            }
            finally
            {
                if (!this.Reader.IsClosed) this.Reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 获取某个操作员的接单中的数量
        /// </summary>
        /// <param name="receiveOperCode">接单操作员工号</param>
        /// <returns></returns>
        public int GetReceivingCount(string receiveOperCode)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.GetReceivingCount", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            sql = string.Format(sql, receiveOperCode);
            string result = this.ExecSqlReturnOne(sql, "0");

            return int.Parse(result);
        }

        /// <summary>
        /// 更新自助出院待办接单状态
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="receiveState">接单状态：0 未接单；1 接单中；2 已处理；3 已撤单；</param>
        /// <returns></returns>
        public int UpdateSISelfDealRecordState(string inpatientNo, string receiveState)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.UpdateSISelfDealRecordState", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            sql = string.Format(sql, inpatientNo, receiveState);
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 更新自助出院待办处理记录放行状态
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="isPass">是否放行</param>
        /// <param name="passOperCode">放行操作人</param>
        /// <returns></returns>
        public int UpdateSISelfDealRecordPaseFlag(string inpatientNo, bool isPass, string passOperCode)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.UpdatePaseFlag", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            sql = string.Format(sql, inpatientNo, isPass ? "1" : "0", passOperCode);
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 查询接单撤单记录
        /// </summary>
        /// <param name="dtBegin">业务开始时间</param>
        /// <param name="dtEnd">业务结束时间</param>
        /// <param name="parDeptCode">父级科室部门编码</param>
        /// <param name="operCode">接单操作人编码</param>
        /// <param name="state">接单状态：0 未接单；1 接单中；2 已处理；3 已撤单；</param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.RADT.SISelfDealRecordDto> QuerySISelfDealRecordReport(DateTime dtBegin, DateTime dtEnd, string parDeptCode, string operCode, string state, string passFlag, string inState)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.QuerySISelfDealRecordReport", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            var list = new List<Neusoft.HISFC.Models.RADT.SISelfDealRecordDto>();
            try
            {
                sql = string.Format(sql, dtBegin.ToString("yyyy-MM-dd HH:mm:ss"), dtEnd.ToString("yyyy-MM-dd HH:mm:ss"), parDeptCode, operCode, state, passFlag, inState);
                if (this.ExecQuery(sql) == -1)
                {
                    return null;
                }

                Neusoft.HISFC.Models.RADT.SISelfDealRecordDto record = null;
                while (this.Reader.Read())
                {
                    record = new Neusoft.HISFC.Models.RADT.SISelfDealRecordDto()
                    {
                        InpatientNo = this.Reader[0].ToString(),
                        PatientNo = this.Reader[1].ToString(),
                        PatientName = this.Reader[2].ToString(),
                        Dept = new Neusoft.FrameWork.Models.NeuObject()
                        {
                            ID = this.Reader[3].ToString(),
                            Name = this.Reader[4].ToString()
                        },
                        PayKindCode = this.Reader[5].ToString(),
                        Pact = new Neusoft.FrameWork.Models.NeuObject()
                        {
                            ID = this.Reader[6].ToString(),
                            Name = this.Reader[7].ToString(),
                        },
                        DealRecord = new Neusoft.HISFC.Models.RADT.SISelfDealRecord()
                        {
                            InpatientNo = this.Reader[0].ToString(),
                            ReceiveOperCode = this.Reader[8].ToString(),
                            ReceiveDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[9].ToString()),
                            ReceiveState = this.Reader[10].ToString(),
                            RevokeOperCode = this.Reader[11].ToString(),
                            RevokeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[12].ToString()),
                            RevokeReason = this.Reader[13].ToString(),
                            PaseFlag = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[14].ToString()),
                            PaseOperCode = this.Reader[15].ToString(),
                            PaseDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[16].ToString()),
                            RevokeType = this.Reader[17].ToString()
                        },
                        IsSIBalanced = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[18].ToString()),
                        DateIn = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[19].ToString()),
                        DateOut = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[20].ToString()),
                        PathologyPassFlag = this.Reader[21].ToString(),
                        InState = this.Reader[22].ToString()
                    };
                    list.Add(record);
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                list = null;
            }
            finally
            {
                if (!this.Reader.IsClosed) this.Reader.Close();
            }

            return list;
        }

        /// <summary>
        /// 查询医保自助出院接单处理日志
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        public DataTable QuerySISelfDealRecordLog(string inpatientNo)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("RADT.SISelfInpatient.QuerySISelfDealRecordLog", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            try
            {
                sql = string.Format(sql, inpatientNo);

                DataSet ds = new DataSet();
                if (this.ExecQuery(sql, ref ds) == -1 || ds == null || ds.Tables.Count == 0) return null;
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 医保复核确认
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="regNo">就医登记号</param>
        /// <param name="balanceNo">结算序号</param>
        /// <param name="operCoder">操作员</param>
        /// <returns></returns>
        public int UpdateCheckConfirm(string inpatientNo, string regNo, string balanceNo, string operCoder)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Fee.Interface.Update.CheckConfirm", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            try
            {
                sql = string.Format(sql, inpatientNo, regNo, balanceNo, operCoder);
                return this.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 医保复核取消
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="regNo">就医登记号</param>
        /// <param name="balanceNo">结算序号</param>
        /// <param name="operCoder">操作员</param>
        /// <param name="cancelReason">取消原因</param>
        /// <returns></returns>
        public int UpdateCheckCancel(string inpatientNo, string regNo, string balanceNo, string operCoder, string cancelReason)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Fee.Interface.Update.CheckCancel", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            try
            {
                sql = string.Format(sql, inpatientNo, regNo, balanceNo, operCoder, cancelReason);
                return this.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }



    }
}
