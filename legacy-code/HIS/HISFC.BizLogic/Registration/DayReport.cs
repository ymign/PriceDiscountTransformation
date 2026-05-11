using System;
using System.Collections;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Registration
{
	/// <summary>
	/// 日结管理类
	/// </summary>
	public class DayReport : Neusoft.FrameWork.Management.Database
	{
		/// <summary>
		/// 
		/// </summary>
		public DayReport()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

        /// <summary>
        /// 获取挂号明细
        /// </summary>
        /// <param name="operCode"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int QueryRegisterDetails(string operCode, DateTime beginTime, DateTime endTime, ref DataSet ds) 
        {
            return this.ExecQuery("Registration.Register.Query.11", ref ds, beginTime.ToString(), endTime.ToString(), operCode.ToString());
        }

        /// <summary>
        /// 获取挂号明细[新]
        /// </summary>
        /// <param name="operCode"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="transType"></param>
        /// <param name="dsResult"></param>
        /// <returns></returns>
        public int QueryRegisterDetails(string operCode, DateTime beginTime, DateTime endTime, string transType, ref DataSet dsResult)
        {
            return this.ExecQuery("Registration.Register.QueryAccountFee.1", ref dsResult, beginTime.ToString(), endTime.ToString(), operCode, transType); 
        }

        /// <summary>
        /// 根据有效性获取挂号明
        /// </summary>
        /// <param name="operCode"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="cancelFlag"></param>
        /// <param name="dsResult"></param>
        /// <returns></returns>
        public int QueryRegisterDetailsByCancelFlag(string operCode, DateTime beginTime, DateTime endTime, string cancelFlag, ref DataSet dsResult)
        {
            return this.ExecQuery("Registration.Register.QueryAccountFee.2", ref dsResult, beginTime.ToString(), endTime.ToString(), operCode, cancelFlag);
        }

        /// <summary>
        /// 获取日结信息按科室统计
        /// </summary>
        /// <param name="operCode"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="dsResult"></param>
        /// <returns></returns>
        public int QueryRegisterDetailsForDept(string operCode, DateTime beginTime, DateTime endTime, ref DataSet dsResult)
        {
            return this.ExecQuery("Registration.Register.QueryAccountFee.3", ref dsResult, beginTime.ToString(), endTime.ToString(), operCode);
        }

        /// <summary>
        /// 获取补打日结信息按科室统计
        /// </summary>
        /// <param name="operCode"></param>
        /// <param name="balanceNo"></param>
        /// <param name="dsResult"></param>
        /// <returns></returns>
        public int QueryRegisterDetailsForDept(string balanceNo, ref DataSet dsResult)
        {
            return this.ExecQuery("Registration.Register.QueryAccountFee.4", ref dsResult, balanceNo);
        }

		/// <summary>
		/// 登记日结信息
		/// </summary>
		/// <param name="dayReport"></param>
		/// <returns></returns>
		public int Insert(Neusoft.HISFC.Models.Registration.DayReport dayReport)
		{
			string sql="";

			if(this.Sql.GetCommonSql("Registration.DayReport.Insert.1",ref sql) == -1)return -1;

			try
			{
				sql = string.Format(sql,dayReport.ID,dayReport.BeginDate.ToString(),dayReport.EndDate.ToString(),
					dayReport.SumCount,dayReport.SumRegFee,dayReport.SumChkFee,dayReport.SumDigFee,
					dayReport.SumOthFee,dayReport.SumOwnCost,dayReport.SumPayCost,dayReport.SumPubCost,
					dayReport.Oper.ID,dayReport.Oper.OperTime.ToString(),dayReport.Memo ) ;
			}
			catch(Exception e)
			{
				this.Err = "[Registration.DayReport.Insert.1]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return -1;
			}			

			if(this.ExecNoQuery(sql) == -1) return -1;

			foreach(Neusoft.HISFC.Models.Registration.DayDetail detail in dayReport.Details)
			{
				if(this.Insert(detail) == -1) return -1 ;
			}

			return 0 ;
		}

        /// <summary>
        /// 挂号日结表头信息-新
        /// </summary>
        /// <param name="dayReport"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.Registration.DayReport dayReport, ref string errText)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.DayReport.Insert.2", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, dayReport.ID, dayReport.BeginDate.ToString(), dayReport.EndDate.ToString(),
                    dayReport.SumCount, dayReport.SumRegFee, dayReport.SumChkFee, dayReport.SumDigFee,
                    dayReport.SumOthFee, dayReport.SumOwnCost, dayReport.SumPayCost, dayReport.SumPubCost,
                    dayReport.Oper.ID, dayReport.Oper.OperTime.ToString(), dayReport.Memo, dayReport.SumCardFee, dayReport.SumCaseFee);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.DayReport.Insert.2]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                errText = this.Err;
                return -1;
            }

            if (this.ExecNoQuery(sql) == -1)
            {
                errText = "执行Registration.DayReport.Insert.2出错!" + this.Err;
                return -1;
            } 

            foreach (Neusoft.HISFC.Models.Registration.DayDetail detail in dayReport.Details)
            {
                if (this.Insert(detail, errText) == -1)
                {
                    errText = "插入挂号日结明细表出错" + this.Err;
                    return -1;
                }
            }

            return 0;
        }

		/// <summary>
		/// 登记日结明细
		/// </summary>
		/// <param name="dayDetail"></param>
		/// <returns></returns>
		public int Insert(Neusoft.HISFC.Models.Registration.DayDetail dayDetail)
		{
			string sql="";

			if(this.Sql.GetCommonSql("Registration.DayReport.Insert.Detail",ref sql) == -1)return -1;

			try
			{
				sql = string.Format(sql,dayDetail.ID,dayDetail.OrderNO,dayDetail.BeginRecipeNo,
					dayDetail.EndRecipeNo,dayDetail.Count,dayDetail.RegFee,dayDetail.ChkFee,dayDetail.DigFee,
					dayDetail.OthFee,dayDetail.OwnCost,dayDetail.PayCost,dayDetail.PubCost,(int)dayDetail.Status) ;
			}
			catch(Exception e)
			{
				this.Err = "[Registration.DayReport.Insert.Detail]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return -1;
			}			

			return this.ExecNoQuery(sql) ;
		}

        /// <summary>
        /// 挂号日结费用明细-新
        /// </summary>
        /// <param name="dayDetail"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.Registration.DayDetail dayDetail, string errText)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.DayReport.Insert.Detail.2", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, dayDetail.ID, dayDetail.OrderNO, dayDetail.BeginRecipeNo,
                    dayDetail.EndRecipeNo, dayDetail.Count, dayDetail.RegFee, dayDetail.ChkFee, dayDetail.DigFee,
                    dayDetail.OthFee, dayDetail.OwnCost, dayDetail.PayCost, dayDetail.PubCost, (int)dayDetail.Status,
                    dayDetail.CardFee, dayDetail.CaseFee);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.DayReport.Insert.Detail.2]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                errText = this.Err;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 挂号日结_更新fin_opb_accountCardFee的费用明细已日结[新]
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="OperID"></param>
        /// <param name="BalanceNO"></param>
        /// <returns></returns>
        public int UpdateAccountCardFeeForBalanced(DateTime begin, DateTime end, string OperID, string BalanceNO)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Update.AccountCardFee", ref sql) == -1)
            {
                this.Err = "获取Registration.Register.Update.AccountCardFee失败!" + this.Err;
                return -1;
            } 

            try
            {
                sql = string.Format(sql, begin.ToString(), end.ToString(), OperID, BalanceNO);

                if (this.ExecNoQuery(sql) <= 0)
                {
                    this.Err = "执行SQL语句Registration.Register.Update.AccountCardFee失败!" + this.Err;
                    return -1;
                }

                return 1;
            }
            catch (Exception e)
            {
                this.Err = "置挂号信息日结标志出错![Registration.Register.Update.AccountCardFee]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

		/// <summary>
		/// 根据操作员获得日结起始时间
		/// </summary>
		/// <param name="OperID"></param>		
		/// <returns></returns>
		public string GetBeginDate(string OperID)
		{
			string sql="";			

			if(this.Sql.GetCommonSql("Registration.DayReport.Query.1",ref sql)==-1)return "-1";
						
			try
			{
				sql=string.Format(sql,OperID);
			}
			catch(Exception e)
			{
				this.Err="查询日结信息时出错![Registration.DayReport.Query.1]"+e.Message;
				this.ErrCode=e.Message;
				return "-1";
			}

			string rtn = this.ExecSqlReturnOne(sql,DateTime.MinValue.ToString()) ;
						
			return rtn ;				
		}

		/// <summary>
		/// 查询日结信息
		/// </summary>
		/// <param name="OperId"></param>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public ArrayList Query(string OperId,DateTime begin,DateTime end)
		{
			string sql="";
			
			if(this.Sql.GetCommonSql("Registration.DayReport.Query.2",ref sql)==-1)return null;
						
			try
			{
				sql=string.Format(sql,OperId,begin.ToString(),end.ToString() );
			}
			catch(Exception e)
			{
				this.Err="查询日结信息时出错![Registration.DayReport.Query.2]"+e.Message;
				this.ErrCode=e.Message;
				return null;
			}

			if(this.ExecQuery(sql) == -1)return null ;

			ArrayList al = new ArrayList() ;
			try
			{
				while(this.Reader.Read())
				{
					Neusoft.HISFC.Models.Registration.DayReport report = new Neusoft.HISFC.Models.Registration.DayReport() ;
					report.ID = this.Reader[2].ToString() ;
					report.BeginDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[3].ToString()) ;
					report.EndDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4].ToString()) ;
					report.SumCount = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[5].ToString()) ;
					report.SumRegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[6].ToString()) ;
					report.SumChkFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[7].ToString()) ;
					report.SumDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[8].ToString()) ;
					report.SumOthFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[9].ToString()) ;
					report.SumOwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[10].ToString()) ;
					report.SumPayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[11].ToString()) ;
					report.SumPubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12].ToString()) ;
					report.Oper.ID = this.Reader[13].ToString() ;
					report.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[14].ToString()) ;

					al.Add(report) ;
				}
				this.Reader.Close() ;
			}
			catch(Exception e)
			{
				this.Err="查询日结信息时出错![Registration.DayReport.Query.2]"+e.Message;
				this.ErrCode=e.Message;
				return null;
			}

			return al ;
		}

        /// <summary>
        /// 查询日结信息-新
        /// </summary>
        /// <param name="OperId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList QueryNew(string OperId, DateTime begin, DateTime end)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.DayReport.Query.4", ref sql) == -1) return null;

            try
            {
                sql = string.Format(sql, OperId, begin.ToString(), end.ToString());
            }
            catch (Exception e)
            {
                this.Err = "查询日结信息时出错![Registration.DayReport.Query.4]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            if (this.ExecQuery(sql) == -1) return null;

            ArrayList al = new ArrayList();
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Registration.DayReport report = new Neusoft.HISFC.Models.Registration.DayReport();
                    report.ID = this.Reader[2].ToString();
                    report.BeginDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[3].ToString());
                    report.EndDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4].ToString());
                    report.SumCount = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[5].ToString());
                    report.SumRegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[6].ToString());
                    report.SumChkFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[7].ToString());
                    report.SumDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[8].ToString());
                    report.SumCardFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[9].ToString());
                    report.SumCaseFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[10].ToString());
                    report.SumOthFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[11].ToString());
                    report.SumOwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12].ToString());
                    report.SumPayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());
                    report.SumPubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[14].ToString());
                    report.Oper.ID = this.Reader[15].ToString();
                    report.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[16].ToString());

                    al.Add(report);
                }
                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "查询日结信息时出错![Registration.DayReport.Query.4]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return al;
        }

		/// <summary>
		/// 按日结序号查询日结明细
		/// </summary>
		/// <param name="BalanceNo"></param>
		/// <returns></returns>
		public ArrayList Query(string BalanceNo)
		{
			string sql="";
			
			if(this.Sql.GetCommonSql("Registration.DayReport.Query.3",ref sql)==-1)return null;
						
			try
			{
				sql=string.Format(sql,BalanceNo);
			}
			catch(Exception e)
			{
				this.Err="查询日结信息时出错![Registration.DayReport.Query.3]"+e.Message;
				this.ErrCode=e.Message;
				return null;
			}

			if(this.ExecQuery(sql) == -1)return null ;

			ArrayList al = new ArrayList() ;
			try
			{
				while(this.Reader.Read())
				{
					Neusoft.HISFC.Models.Registration.DayDetail report = new Neusoft.HISFC.Models.Registration.DayDetail() ;

					report.ID = this.Reader[2].ToString() ;
					report.OrderNO = this.Reader[3].ToString() ;
					report.BeginRecipeNo = this.Reader[4].ToString() ;
					report.EndRecipeNo = this.Reader[5].ToString() ;
					report.Count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[6].ToString()) ;
					report.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[7].ToString()) ;
					report.ChkFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[8].ToString()) ;
					report.DigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[9].ToString()) ;
					report.OthFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[10].ToString()) ;
					report.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[11].ToString()) ;
					report.PayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12].ToString()) ;
					report.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString()) ;
					report.Status = (Neusoft.HISFC.Models.Base.EnumRegisterStatus)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[14].ToString()) ;
					
					al.Add(report) ;
				}
				this.Reader.Close() ;
			}
			catch(Exception e)
			{
				this.Err="查询日结信息时出错![Registration.DayReport.Query.3]"+e.Message;
				this.ErrCode=e.Message;
				return null;
			}

			return al ;
		}

        /// <summary>
        /// 按日结序号查询日结明细-新
        /// </summary>
        /// <param name="BalanceNo"></param>
        /// <returns></returns>
        public ArrayList QueryNew(string BalanceNo)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.DayReport.Query.5", ref sql) == -1) return null;

            try
            {
                sql = string.Format(sql, BalanceNo);
            }
            catch (Exception e)
            {
                this.Err = "查询日结信息时出错![Registration.DayReport.Query.5]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            if (this.ExecQuery(sql) == -1) return null;

            ArrayList al = new ArrayList();
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Registration.DayDetail report = new Neusoft.HISFC.Models.Registration.DayDetail();

                    report.ID = this.Reader[2].ToString();
                    report.OrderNO = this.Reader[3].ToString();
                    report.BeginRecipeNo = this.Reader[4].ToString();
                    report.EndRecipeNo = this.Reader[5].ToString();
                    report.Count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[6].ToString());
                    report.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[7].ToString());
                    report.ChkFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[8].ToString());
                    report.DigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[9].ToString());
                    report.CardFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[10].ToString());
                    report.CaseFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[11].ToString());
                    report.OthFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12].ToString());
                    report.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());
                    report.PayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[14].ToString());
                    report.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[15].ToString());
                    report.Status = (Neusoft.HISFC.Models.Base.EnumRegisterStatus)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[16].ToString());

                    al.Add(report);
                }
                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "查询日结信息时出错![Registration.DayReport.Query.5]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return al;
        }


        #region 中六挂号日结添加
        /// <summary>
        /// 查询现金总金额
        /// </summary>
        /// <param name="operId"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <param name="payType">支付类别</param>
        /// <returns></returns>
        public decimal QueryPayModeTotal(string operId, string dtBegin, string dtEnd,string payType)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Registration.DayReport.QueryPayModeTotal", ref sql) == -1)
            {
                sql = @"	select NVL(SUM(OWN_COST+PAY_COST),0)  from fin_opb_accountcardfee  a where a.oper_code ='{0}' 
	and a.oper_date between to_date('{1}','yyyy-MM-dd hh24:mi:ss') and to_date('{2}','yyyy-MM-dd hh24:mi:ss')
	and a.pay_type='{3}'";
            }
            try
            {
                sql = string.Format(sql, operId, dtBegin, dtEnd, payType);
                return Neusoft.FrameWork.Function.NConvert.ToDecimal(this.ExecSqlReturnOne(sql, "0"));
 
            }
            catch (Exception e)
            {

                this.Err = "查询现金总金额![Registration.DayReport.QueryPayModeTotal]" + e.Message;
                this.ErrCode = e.Message;
                return 0;
            }
           
        }
        /// <summary>
        /// 获取诊疗卡预交金操作记录
        /// </summary>
        /// <param name="operId"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public DataSet  QueryAccountPay(string operId, string dtBegin, string dtEnd)
        {
            string sql = "";
            DataSet ds = new DataSet();
            if (this.Sql.GetCommonSql("Registration.DayReport.QueryAccountPay", ref sql) == -1)
            {
                sql = @"	select * from fin_opb_accountrecord c where c.oper_code='{0}' 
	and c.oper_date between to_date('{1}','yyyy-MM-dd hh24:mi:ss') and to_date('{2}','yyyy-MM-dd hh24:mi:ss')
	and c.opertype in ('0','16')";
            }
            try
            {
                sql = string.Format(sql, operId, dtBegin, dtEnd);
                this.ExecQuery(sql,ref ds);
                return ds;

            }
            catch (Exception e)
            {

                this.Err = "获取诊疗卡预交金操作记录![Registration.DayReport.QueryAccountPay]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
        }

        #endregion


    }	
}
