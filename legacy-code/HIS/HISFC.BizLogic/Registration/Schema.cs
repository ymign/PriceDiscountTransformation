using System;
using System.Collections;
using System.Data ;

namespace Neusoft.HISFC.BizLogic.Registration
{
	/// <summary>
	/// 排班管理类
	/// </summary>
    public class Schema : Neusoft.FrameWork.Management.Database
	{
		/// <summary>
		/// 
		/// </summary>
		public Schema()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#region 增加
		/// <summary>
		/// 登记一条排班记录
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		public int Insert(Neusoft.HISFC.Models.Registration.Schema schema)
		{
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Insert.WithSchemaDept", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, schema.Templet.ID, (int)schema.Templet.EnumSchemaType, schema.SeeDate.ToString(),
                    (int)schema.Templet.Week, schema.Templet.Noon.ID, schema.Templet.Dept.ID, schema.Templet.Dept.Name,
                    schema.Templet.Doct.ID, schema.Templet.Doct.Name, schema.Templet.DoctType.ID, schema.Templet.RegQuota,
                    schema.RegedQTY, Neusoft.FrameWork.Function.NConvert.ToInt32(schema.Templet.IsValid),
                    schema.Templet.StopReason.ID, schema.Templet.StopReason.Name, schema.Templet.Stop.ID, schema.Templet.Stop.OperTime.ToString(),
                    schema.Templet.Memo, schema.Templet.Oper.ID, schema.Templet.Oper.OperTime.ToString(),
                    schema.Templet.Begin.ToString(), schema.Templet.End.ToString(), schema.Templet.TelQuota,
                    schema.TeledQTY, schema.TelingQTY, schema.Templet.SpeQuota, schema.SpedQTY,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(schema.Templet.IsAppend),
                    schema.Templet.RegLevel.ID, schema.Templet.RegLevel.Name,
                    schema.Templet.Room.ID, schema.Templet.Room.Name,
                    schema.Templet.Console.ID, schema.Templet.Console.Name,schema.Templet.SchemaDeptCode,schema.Templet.SchemaDeptName);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Insert.WithSchemaDept]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
		}
		#endregion

		#region 删除
		/// <summary>
		/// 根据ID删除一条排班记录
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public int Delete(string ID)
		{
			string sql = "";

			if(this.Sql.GetCommonSql("Registration.Schema.Delete",ref sql) == -1)return -1;

			try
			{
				sql = string.Format(sql,ID);
			}
			catch(Exception e)
			{
				this.Err = "[Registration.Schema.Delete]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return -1;
			}

			return this.ExecNoQuery(sql);
		}
		#endregion

		#region 修改
		/// <summary>
		/// 根据ID修改一条排班记录(已使用的)
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		public int Update(Neusoft.HISFC.Models.Registration.Schema schema)
		{
			string sql = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Update.WithSchemaDept", ref sql) == -1) return -1;

			try
			{
                sql = string.Format(sql, schema.Templet.RegQuota, Neusoft.FrameWork.Function.NConvert.ToInt32(schema.Templet.IsValid),
                    schema.Templet.StopReason.ID, schema.Templet.StopReason.Name, schema.Templet.Stop.ID,
                    schema.Templet.Stop.OperTime.ToString(), schema.Templet.Memo, schema.Templet.Oper.ID,
                    schema.Templet.Oper.OperTime.ToString(), schema.Templet.TelQuota,
                    schema.Templet.SpeQuota, schema.Templet.ID, 
                    schema.Templet.RegLevel.ID, schema.Templet.RegLevel.Name,
                    schema.Templet.Room.ID, schema.Templet.Room.Name,
                    schema.Templet.Console.ID, schema.Templet.Console.Name,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(schema.Templet.IsStop),
                    schema.Templet.SchemaDeptCode,schema.Templet.SchemaDeptName
                );
			}
			catch(Exception e)
			{
				this.Err = "[Registration.Schema.Update]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return -1;
			}

			return this.ExecNoQuery(sql);
		}

        /// <summary>
        ///  停止门诊护士队列，停止门诊排版
        /// </summary>
        /// <returns></returns>
        public int UpdateDoctSchemaValid(string validFlag, string seeDate, string noonCode, string deptCode, string doctCode, string reasonNo, string reasonName, string stopOpcd, DateTime stopDate)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Schema.UpdateDoctSchemaValid", ref sql) == -1) 
            {
                sql = @"UPDATE fin_opr_schema   --医师出诊表
                        SET valid_flag='{0}',   --1正常/0停诊
                        Reason_No ='{5}',
                        reason_name='{6}',
                        stop_opcd ='{7}',
                        stop_date =to_date('{8}','yyyy-mm-dd hh24:mi:ss')
                        where see_date= to_date('{1}','yyyy:mm:dd') 
                        and noon_code='{2}'
                        and dept_code='{3}'
                        and doct_code='{4}'";
            }
            try
            {
                sql = string.Format(sql, validFlag, seeDate, noonCode, deptCode, doctCode, reasonNo, reasonName, stopOpcd, stopDate);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Update]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
            return this.ExecNoQuery(sql);
        }




        /// <summary>
        /// 根据ID更新排班信息(未使用的)
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public int UpdateUnused(Neusoft.HISFC.Models.Registration.Schema schema)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Update.WithSchemaDept", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, schema.Templet.ID, schema.Templet.Noon.ID, schema.Templet.Begin.ToString(),
                        schema.Templet.End.ToString(), schema.Templet.Dept.ID, schema.Templet.Dept.Name,
                        schema.Templet.Doct.ID, schema.Templet.Doct.Name, schema.Templet.DoctType.ID,
                        schema.Templet.RegQuota, schema.Templet.TelQuota, schema.Templet.SpeQuota,
                        Neusoft.FrameWork.Function.NConvert.ToInt32(schema.Templet.IsValid),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(schema.Templet.IsAppend),
                        schema.Templet.StopReason.ID, schema.Templet.StopReason.Name, schema.Templet.Stop.ID,
                        schema.Templet.Stop.OperTime.ToString(), schema.Templet.Memo, schema.Templet.Oper.ID,
                        schema.Templet.Oper.OperTime.ToString(), schema.Templet.RegLevel.ID, schema.Templet.RegLevel.Name,
                    schema.Templet.Room.ID, schema.Templet.Room.Name,
                    schema.Templet.Console.ID, schema.Templet.Console.Name,
                        Neusoft.FrameWork.Function.NConvert.ToInt32(schema.Templet.IsStop),
                    schema.Templet.SchemaDeptCode, schema.Templet.SchemaDeptName
                    );
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Update.WithSchemaDept]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }

		/// <summary>
		/// 更新看诊数量
		/// IsReged:是否是来人挂号,IsTeling:是否是来电预约,IsTeled:是否来电挂号
		/// IsSped:是否特诊挂号
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="IsReged"></param>
		/// <param name="IsTeling"></param>
		/// <param name="IsTeled"></param>
		/// <param name="IsSped"></param>
		/// <returns></returns>
		public int Increase(string ID,bool IsReged,bool IsTeling, bool IsTeled,bool IsSped)
		{
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Update.4", ref sql) == -1) return -1;

            int reged = 0, teling = 0, teled = 0, sped = 0, add = 1;

            if (IsReged) reged = 1;
            if (IsTeling)//电话预约不用增加看诊序号
            {
                teling = 1;
                add = 0;
            }
            if (IsTeled) teled = 1;
            if (IsSped) sped = 1;

            try
            {
                sql = string.Format(sql, reged, teled, teling, sped, ID, add);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Update.4]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
		}

		/// <summary>
		///  减少看诊数量
		///  IsReged:是否是来人挂号,IsTeling:是否是来电预约,IsTeled:是否来电挂号
		///  IsSped:是否特诊挂号
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="IsReged"></param>
		/// <param name="IsTeling"></param>
		/// <param name="IsTeled"></param>
		/// <param name="IsSped"></param>
		/// <returns></returns>
		public int Reduce(string ID,bool IsReged,bool IsTeling, bool IsTeled,bool IsSped)
        {
            #region 按小时排班，退号时，该时段本地限额会回滚，挂号会出现插队情况，故退号的号源加在最大时段 y_ming

            string MaxSchemaSql = string.Empty;
            try
            {
                if (IsReged)
                {
                    if (this.Sql.GetCommonSql("Registration.Schema.GetMaxSchemaById", ref MaxSchemaSql) == -1)
                    { return -1; 
                    }
                    else
                    {
                        MaxSchemaSql = string.Format(MaxSchemaSql, ID);
                        string maxId = this.ExecSqlReturnOne(MaxSchemaSql);
                        if (!string.IsNullOrEmpty(maxId)&&(ID!=maxId))
                        {
                            string updateSql = @"update fin_opr_schema a
                                            set a.reg_lmt=a.reg_lmt-1
                                            where a.id='{0}' ";
                                            string updateSql2=@"update fin_opr_schema a
                                            set a.reg_lmt=a.reg_lmt+1 
                                            where a.id='{0}' ";
                            updateSql = string.Format(updateSql, ID);
                            int x= this.ExecNoQuery(updateSql);
                            updateSql2 = string.Format(updateSql2, maxId);
                            int y = this.ExecNoQuery(updateSql2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Err = "sql格式不匹配!" + ex.Message;
                this.ErrCode = ex.Message;
                return -1;         
            }

            #endregion


            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Update.1", ref sql) == -1) return -1;

            int reged = 0, teling = 0, teled = 0, sped = 0;

            if (IsReged) reged = -1;
            if (IsTeling) teling = -1;
            if (IsTeled) teled = -1;
            if (IsSped) sped = -1;

            try
            {
                sql = string.Format(sql, reged, teled, teling, sped, ID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Update.1]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
		}
        /// <summary>
        ///  减少看诊数量
        ///  IsReged:是否是来人挂号,IsTeling:是否是来电预约,IsTeled:是否来电挂号
        ///  IsSped:是否特诊挂号
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="IsReged"></param>
        /// <param name="IsTeling"></param>
        /// <param name="IsTeled"></param>
        /// <param name="IsSped"></param>
        /// <returns></returns>
        public int AddLimit(string ID, bool IsReged, bool IsTeling, bool IsTeled, bool IsSped)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Update.1", ref sql) == -1) return -1;

            int reged = 0, teling = 0, teled = 0, sped = 0;

            if (IsReged) reged = 1;
            if (IsTeling) teling = 1;
            if (IsTeled) teled = 1;
            if (IsSped) sped = 1;

            try
            {
                sql = string.Format(sql, reged, teled, teling, sped, ID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Update.1]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        public int UploadSchema()
        {
            string sql = "update  com_dictionary  a set a.code=to_char(sysdate,'yyyy-mm-dd hh24:mi:ss') where a.type='UploadSchemaRealTime'";
            try
            {
                return this.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                return -1;
            }
        
        }
		#endregion

		#region 查询

        /// <summary>
        /// 查询当前时间点医生的排班信息
        /// {231D1A80-6BF6-413f-8BBF-727DC2BF47D9}
        /// </summary>
        /// <param name="doctCode"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.Schema GetSchema(string doctCode, DateTime time)
        {
            ArrayList al = QuerySchema("Fee.OutPatient.GetSchemaByDoctAndTime", time.ToString(), doctCode);
            if (al == null || al.Count == 0)
            {
                return null;
            }
            return al[0] as Neusoft.HISFC.Models.Registration.Schema;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doctCode"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public ArrayList GetSchemaUnStop(string doctCode,DateTime time)
        {
            ArrayList al = QuerySchema("Fee.OutPatient.GetSchemaByDoctAndTime2", time.ToString(), doctCode);
            if (al == null || al.Count == 0)
            {
                return null;
            }
            return al;
        }

        /// <summary>
        /// 基础查询
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList QuerySchema(string whereSql, params string[] args)
        {
            string strSql = string.Empty;
            int returnValue = this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept", ref strSql);
            if (returnValue < 0)
            {
                this.Err = "没有找到[Registration.Schema.Query.WithSchemaDept]对应的sql语句";
                return null;
            }
            returnValue = this.Sql.GetCommonSql(whereSql, ref whereSql);
            if (returnValue < 0)
            {
                this.Err = "没有找到[" + whereSql + "]对应的sql语句";
                return null;
            }

            try
            {
                whereSql = string.Format(whereSql, args);
            }
            catch (Exception ex)
            {

                this.Err = "[" + whereSql + "]格式不匹配";
                return null;
            }

            strSql = strSql + " " + whereSql;

            return this.QueryBase(strSql);
        }

		/// <summary>
		/// 
		/// </summary>
		protected Neusoft.HISFC.Models.Registration.Schema objSchema ;
		private ArrayList al ;

		/// <summary>
		/// 根据出诊时间、排班类型检索排班信息
		/// </summary>
		/// <param name="seeDate"></param>
		/// <param name="type"></param>
		/// <param name="deptID"></param>
		/// <returns></returns>
		public ArrayList Query(DateTime seeDate,Neusoft.HISFC.Models.Base.EnumSchemaType type,string deptID)
		{
			string sql = "",where = "";

			if(this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept",ref sql) == -1)return null;
			if(this.Sql.GetCommonSql("Registration.Schema.Query.2",ref where ) == -1)return null;

			sql = sql + " " + where;

			try
			{
				sql = string.Format(sql,seeDate.ToString(),(int)type,deptID);
			}
			catch(Exception e)
			{
				this.Err = "[Registration.Schema.Query.2]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return null;
			}
			return this.QueryBase(sql);
		}

		#region 选择医生出诊列表使用
        
		/// <summary>
		/// 根据出诊时间、科室、检索排班信息
		/// </summary>
		/// <param name="seeDate"></param>
		/// <param name="deptID"></param>		
		/// <returns></returns>
		public ArrayList QueryByDept(DateTime seeDate,string deptID)
		{
			string sql = "",where = "";

			if(this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept",ref sql) == -1)return null;
			if(this.Sql.GetCommonSql("Registration.Schema.Query.3",ref where ) == -1)return null;

			sql = sql + " " + where;

			try
			{
				sql = string.Format(sql,seeDate.Date.ToString(),deptID);
			}
			catch(Exception e)
			{
				this.Err = "[Registration.Schema.Query.3]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return null;
			}
			return this.QueryBase(sql);
		}

        /// <summary>
        /// 根据出诊时间、科室、检索排班信息
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="deptID"></param>		
        /// <returns></returns>
        public ArrayList QueryByDeptAll(DateTime seeDate, string deptID)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Schema.Query.ByDept", ref where) == -1) return null;

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, seeDate.Date.ToString(), deptID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Query.ByDept]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return this.QueryBase(sql);
        }


        /// <summary>
        /// 根据出诊时间、科室、检索排班信息
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="deptID"></param>		
        /// <returns></returns>
        public ArrayList QueryByDept(DateTime seeDate, string deptID,string regLevelID)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Schema.QueryDept.ByDeptAndRegLevel", ref where) == -1)
            {
                where = @"  WHERE see_date = to_date('{0}','yyyy-mm-dd hh24:mi:ss')
                                AND dept_code = '{1}'
                                AND (schema_type = '0' or (schema_type='1' and (reglevl_code='{2}' or 'ALL'='{2}') ))
                                AND valid_flag = '1'
                              ORDER BY noon_code,append_flag,begin_time 
                             ";
            }

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, seeDate.Date.ToString(), deptID, regLevelID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.QueryDept.ByDeptAndRegLevel]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return this.QueryBase(sql);
        }

		/// <summary>
		/// 根据出诊时间、医生、检索医生排班信息
        /// 适合挂号不选择科室,直接选择医生,一个医生可出诊多个科室
		/// </summary>
		/// <param name="seeDate"></param>
		/// <param name="doctID"></param>
		/// <returns></returns>
		public ArrayList QueryByDoct(DateTime seeDate,string doctID)
		{
			string sql = "",where = "";

			if(this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept",ref sql) == -1)return null;
			if(this.Sql.GetCommonSql("Registration.Schema.Query.4",ref where ) == -1)return null;

			sql = sql + " " + where;

			try
			{
				sql = string.Format(sql,seeDate.Date.ToString(),doctID);
			}
			catch(Exception e)
			{
				this.Err = "[Registration.Schema.Query.4]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return null;
			}
			return this.QueryBase(sql);
		}

        /// <summary>
        /// 根据出诊时间、科室、医生、检索医生排班信息
        /// 适合挂号时先选择科室,再选择医生,一个医生可出诊多个科室,
        /// 只显示某一个出诊科室的排班信息
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="deptID"></param>
        /// <param name="doctID"></param>
        /// <returns></returns>
        public ArrayList QueryByDoct(DateTime seeDate, string deptID, string doctID)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Schema.Query.14", ref where) == -1) return null;

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, seeDate.Date.ToString(), doctID, deptID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Query.14]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return this.QueryBase(sql);
        }
        
		/// <summary>
		/// 检索加号排班信息实体
		/// </summary>
		/// <param name="seeDate"></param>
		/// <param name="deptID"></param>
		/// <param name="doctID"></param>
		/// <param name="noonID"></param>		
		/// <returns></returns>
		public Neusoft.HISFC.Models.Registration.Schema QueryAppend(DateTime seeDate, string deptID, string doctID, string noonID)
		{
			string sql = "",where = "";

			if(this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept",ref sql) == -1)return null;
			if(this.Sql.GetCommonSql("Registration.Schema.Query.5",ref where ) == -1)return null;

			sql = sql + " " + where;

			try
			{
				sql = string.Format(sql,seeDate.ToString("yyyy-MM-dd"),deptID,doctID,noonID	) ;
			}
			catch(Exception e)
			{
				this.Err = "[Registration.Schema.Query.5]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return null;
			}

			if(this.QueryBase(sql) == null) return null ;

			if(al == null)return null ;
			if(al.Count == 0) return new Neusoft.HISFC.Models.Registration.Schema() ;

			return (Neusoft.HISFC.Models.Registration.Schema)al[0] ;
		}      
		#endregion

		/// <summary>
		/// 查询专科日出诊情况/出诊专家的科室情况
		/// </summary>
		/// <param name="seeDate"></param>
		/// <param name="endDate"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public DataSet QueryDept( DateTime seeDate,DateTime endDate, Neusoft.HISFC.Models.Base.EnumSchemaType type)
		{
			string sql = "" ;
			DataSet ds = new DataSet() ;
 
			if(type == Neusoft.HISFC.Models.Base.EnumSchemaType.Dept)
			{
				if(this.Sql.GetCommonSql("Registration.Schema.Query.6",ref sql) == -1) return null ;
				
				try
				{
					sql = string.Format(sql,seeDate.Date.ToString(), endDate.ToString()) ;
				}
				catch(Exception e)
				{
					this.Err = "[Registration.Schema.Query.6]格式不匹配!"+e.Message;
					this.ErrCode = e.Message;
					return null ;
				}

				if(this.ExecQuery(sql, ref ds) == -1) return null; 

				return ds ;
			}
			else 
			{
				if(this.Sql.GetCommonSql("Registration.Schema.Query.7",ref sql) == -1) return null ;
				
				try
				{
					sql = string.Format(sql,seeDate.Date.ToString(), endDate.ToString()) ;
				}
				catch(Exception e)
				{
					this.Err = "[Registration.Schema.Query.7]格式不匹配!"+e.Message;
					this.ErrCode = e.Message;
					return null ;
				}

				if(this.ExecQuery(sql, ref ds) == -1) return null; 

				return ds ;
			}
		}

        /// <summary>
        /// 查询专科日出诊情况/出诊专家的科室情况
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="endDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataSet QueryDeptWithHosCode(DateTime seeDate, DateTime endDate, Neusoft.HISFC.Models.Base.EnumSchemaType type, string hosCode)
        {
            string sql = "";
            DataSet ds = new DataSet();

            if (type == Neusoft.HISFC.Models.Base.EnumSchemaType.Dept)
            {
                if (this.Sql.GetCommonSql("Registration.Schema.Query.HosCode.6", ref sql) == -1) return null;

                try
                {
                    sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString(), hosCode);
                }
                catch (Exception e)
                {
                    this.Err = "[Registration.Schema.Query.HosCode.6]格式不匹配!" + e.Message;
                    this.ErrCode = e.Message;
                    return null;
                }

                if (this.ExecQuery(sql, ref ds) == -1) return null;

                return ds;
            }
            else
            {
                if (this.Sql.GetCommonSql("Registration.Schema.Query.HosCode.7", ref sql) == -1) return null;

                try
                {
                    sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString(), hosCode);
                }
                catch (Exception e)
                {
                    this.Err = "[Registration.Schema.Query.HosCode.7]格式不匹配!" + e.Message;
                    this.ErrCode = e.Message;
                    return null;
                }

                if (this.ExecQuery(sql, ref ds) == -1) return null;

                return ds;
            }
        }

        /// <summary>
        /// 查询专科日出诊情况/出诊专家的科室情况
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="endDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataSet QueryDept(DateTime seeDate, DateTime endDate, Neusoft.HISFC.Models.Base.EnumSchemaType type,string reglevelID)
        {
            string sql = "";
            DataSet ds = new DataSet();

            if (type == Neusoft.HISFC.Models.Base.EnumSchemaType.Dept)
            {
                if (this.Sql.GetCommonSql("Registration.Schema.QueryDept.ByRegLevel", ref sql) == -1)
                {
                    sql = @"       SELECT 
                                              a.dept_code,
                                               a.dept_name,
                                               a.noon_code,
                                               a.begin_time,
                                               a.end_time,
                                               a.reg_lmt,
                                               a.reged,
                                               a.tel_lmt,
                                               a.tel_reged,
                                               a.append_flag,
                                               b.spell_code,
                                               b.wb_code,
                                               b.user_code
                                          FROM fin_opr_schema a, com_department b
                                         WHERE a.dept_code = b.dept_code
                                           AND a.see_date = to_date('{0}','yyyy-mm-dd hh24:mi:ss')
                                           AND a.end_time >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                                           AND (a.reglevl_code = '{2}' or 'ALL'='{2}')
                                           AND a.valid_flag = '1'
                                         ORDER BY a.dept_code, a.noon_code, a.append_flag, a.begin_time ";
                }

                try
                {
                    sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString(), reglevelID);
                }
                catch (Exception e)
                {
                    this.Err = "[Registration.Schema.QueryDept.ByRegLevel]格式不匹配!" + e.Message;
                    this.ErrCode = e.Message;
                    return null;
                }

                if (this.ExecQuery(sql, ref ds) == -1) return null;

                return ds;
            }
            else
            {
                if (this.Sql.GetCommonSql("Registration.Schema.QueryDoct.ByRegLevel", ref sql) == -1)
                {
                    sql = @"SELECT DISTINCT a.dept_code,
                                                                   a.dept_name,       
                                                                   b.spell_code,
                                                                   b.wb_code,
                                                                   b.user_code
                                                              FROM fin_opr_schema a, com_department b
                                                             WHERE a.dept_code = b.dept_code
                                                               AND a.see_date = to_date('{0}','yyyy-mm-dd hh24:mi:ss')
                                                               AND a.end_time >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                                                               AND a.schema_type = '1'
                                                               and a.reglevl_code='{2}'
                                                               AND a.valid_flag = '1'";
                }

                try
                {
                    sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString(),reglevelID);
                }
                catch (Exception e)
                {
                    this.Err = "[Registration.Schema.Query.7]格式不匹配!" + e.Message;
                    this.ErrCode = e.Message;
                    return null;
                }

                if (this.ExecQuery(sql, ref ds) == -1) return null;

                return ds;
            }
        }

        /// <summary>
        /// 查询专科日出诊情况/出诊专家的科室情况
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="endDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataSet QueryDeptWithHosCode(DateTime seeDate, DateTime endDate, Neusoft.HISFC.Models.Base.EnumSchemaType type, string reglevelID,string hosCode)
        {
            string sql = "";
            DataSet ds = new DataSet();

            if (type == Neusoft.HISFC.Models.Base.EnumSchemaType.Dept)
            {
                if (this.Sql.GetCommonSql("Registration.Schema.QueryDept.ByRegLevel.HosCode", ref sql) == -1)
                {
                    sql = @"       SELECT 
                                              a.dept_code,
                                               a.dept_name,
                                               a.noon_code,
                                               a.begin_time,
                                               a.end_time,
                                               a.reg_lmt,
                                               a.reged,
                                               a.tel_lmt,
                                               a.tel_reged,
                                               a.append_flag,
                                               b.spell_code,
                                               b.wb_code,
                                               b.user_code
                                          FROM fin_opr_schema a, com_department b
                                         WHERE a.dept_code = b.dept_code
                                           AND a.see_date = to_date('{0}','yyyy-mm-dd hh24:mi:ss')
                                           AND a.end_time >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                                           AND (a.reglevl_code = '{2}' or 'ALL'='{2}')
                                           AND a.valid_flag = '1'
                                           and b.hos_code = '{3}'
                                         ORDER BY a.dept_code, a.noon_code, a.append_flag, a.begin_time ";
                }

                try
                {
                    sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString(), reglevelID, hosCode);
                }
                catch (Exception e)
                {
                    this.Err = "[Registration.Schema.QueryDept.ByRegLevel.HosCode]格式不匹配!" + e.Message;
                    this.ErrCode = e.Message;
                    return null;
                }

                if (this.ExecQuery(sql, ref ds) == -1) return null;

                return ds;
            }
            else
            {
                if (this.Sql.GetCommonSql("Registration.Schema.QueryDoct.ByRegLevel.HosCode", ref sql) == -1)
                {
                    sql = @"SELECT DISTINCT a.dept_code,
                                                                   a.dept_name,       
                                                                   b.spell_code,
                                                                   b.wb_code,
                                                                   b.user_code
                                                              FROM fin_opr_schema a, com_department b
                                                             WHERE a.dept_code = b.dept_code
                                                               AND a.see_date = to_date('{0}','yyyy-mm-dd hh24:mi:ss')
                                                               AND a.end_time >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                                                               AND a.schema_type = '1'
                                                               and a.reglevl_code='{2}'
                                                               and b.hos_Code = '{3}'
                                                               AND a.valid_flag = '1'";
                }

                try
                {
                    sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString(), reglevelID, Neusoft.FrameWork.Management.Connection.Hospital.ID);
                }
                catch (Exception e)
                {
                    this.Err = "Registration.Schema.QueryDoct.ByRegLevel.HosCode格式不匹配!" + e.Message;//"[Registration.Schema.Query.7]格式不匹配!" + e.Message;
                    this.ErrCode = e.Message;
                    return null;
                }

                if (this.ExecQuery(sql, ref ds) == -1) return null;

                return ds;
            }
        }

		/// <summary>
		/// 查询某日全部出诊专家
		/// </summary>
		/// <param name="seeDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public DataSet QueryDoct( DateTime seeDate,DateTime endDate )
		{
			string sql = "" ;
			DataSet ds = new DataSet() ;

			if(this.Sql.GetCommonSql("Registration.Schema.Query.8",ref sql) == -1) return null ;
				
			try
			{
				sql = string.Format(sql,seeDate.Date.ToString(), endDate.ToString()) ;
			}
			catch(Exception e)
			{
				this.Err = "[Registration.Schema.Query.8]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return null ;
			}

			if(this.ExecQuery(sql, ref ds) == -1) return null; 

			return ds ;
		}


        /// <summary>
        /// 查询某日全部出诊专家
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataSet QueryDoctForLED(DateTime seeDate, DateTime endDate)
        {
            string sql = "";
            DataSet ds = new DataSet();

            if (this.Sql.GetCommonSql("Registration.Schema.Query.99", ref sql) == -1) return null;

            try
            {
                sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString());
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Query.99]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            if (this.ExecQuery(sql, ref ds) == -1) return null;

            return ds;
        }
		/// <summary>
		/// 查询某日、某科室出诊专家
		/// </summary>
		/// <param name="seeDate"></param>
		/// <param name="endDate"></param>
		/// <param name="deptID"></param>
		/// <returns></returns>
		public DataSet QueryDoct(DateTime seeDate, DateTime endDate, string deptID)
		{
			string sql = "" ;
			DataSet ds = new DataSet() ;

			if(this.Sql.GetCommonSql("Registration.Schema.Query.9",ref sql) == -1) return null ;
				
			try
			{
				sql = string.Format(sql,seeDate.Date.ToString(), endDate.ToString(), deptID) ;
			}
			catch(Exception e)
			{
				this.Err = "[Registration.Schema.Query.9]格式不匹配!"+e.Message;
				this.ErrCode = e.Message;
				return null ;
			}

			if(this.ExecQuery(sql, ref ds) == -1) return null; 

			return ds ;
		}

        /// <summary>
        /// 查询某日、某科室出诊专家
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="endDate"></param>
        /// <param name="deptID"></param>
        /// <returns></returns>
        public DataSet QueryDoct(DateTime seeDate, DateTime endDate, string deptID, string levelID)
        {
            string sql = "";
            DataSet ds = new DataSet();

            if (this.Sql.GetCommonSql("Registration.Schema.Query.ByDeptAndLevel", ref sql) == -1)
            {
                sql = @"SELECT a.doct_code,
                                           a.doct_name,
                                           a.noon_code,
                                           a.begin_time,
                                           a.end_time,
                                           a.reg_lmt,
                                           a.reged,
                                           a.tel_lmt,
                                           a.tel_reged,
                                           a.spe_lmt,
                                           a.spe_reged,
                                           a.append_flag,
                                           b.spell_code,
                                           b.wb_code,
                                           a.remark,
                                           decode(b.levl_code,'17','1','2','1','21','1','33','1','0')
                                      FROM fin_opr_schema a, com_employee b
                                     WHERE a.doct_code = b.empl_code
                                       AND a.see_date = to_date('{0}','yyyy-mm-dd hh24:mi:ss')
                                       AND a.end_time >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                                       AND a.dept_code= '{2}'
                                       and a.reglevl_code='{3}'
                                       AND a.schema_type = '1'
                                       AND a.valid_flag = '1'";
            }

            try
            {
                sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString(), deptID, levelID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Query.ByDeptAndLevel]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            if (this.ExecQuery(sql, ref ds) == -1)
                return null;

            return ds;
        }

        /// <summary>
        /// 查询某日、某科室出诊专家
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="endDate"></param>
        /// <param name="deptID"></param>
        /// <returns></returns>
        public DataSet QueryDoctByRegLevel(DateTime seeDate, DateTime endDate, string levelID)
        {
            string sql = "";
            DataSet ds = new DataSet();

            if (this.Sql.GetCommonSql("Registration.Schema.Query.ByLevel", ref sql) == -1)
            {
                sql = @"SELECT  a.doct_code,
                                               a.doct_name,
                                               a.noon_code,
                                               a.begin_time,
                                               a.end_time,
                                               a.reg_lmt,
                                               a.reged,
                                               a.tel_lmt,
                                               a.tel_reged,
                                               a.spe_lmt,
                                               a.spe_reged,
                                               a.append_flag,
                                               b.spell_code,
                                               b.wb_code,
                                               a.remark,
                                               a.reglevl_code
                                      FROM fin_opr_schema a, com_employee b,fin_opr_noon c
                                     WHERE a.doct_code = b.empl_code
                                       AND a.see_date = to_date('{0}','yyyy-mm-dd hh24:mi:ss')
                                       AND a.end_time >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                                       AND a.schema_type = '1'  
                                       AND a.valid_flag = '1'  
                                       and a.reglevl_code='{2}'
                                       and a.noon_code = c.noon_code
                                      order by a.see_date,a.dept_code,a.doct_code,a.noon_code,a.begin_time";
            }

            try
            {
                sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString(), levelID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Query.ByLevel]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            if (this.ExecQuery(sql, ref ds) == -1)
                return null;

            return ds;
        }


        /// <summary>
        /// 根据排班序号获取排班实体
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.Schema GetByID(string ID)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Schema.Query.11", ref where) == -1) return null;

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, ID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Query.11]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            if (this.QueryBase(sql) == null) return null;

            if (al == null) return null;

            if (al.Count == 0) return new Neusoft.HISFC.Models.Registration.Schema();

            return (Neusoft.HISFC.Models.Registration.Schema)al[0];
        }

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public ArrayList QueryBase(string sql)
		{
			if(this.ExecQuery(sql) == -1) return null;

			this.al = new ArrayList();

			try
			{
				while(this.Reader.Read())
				{
					this.objSchema = new Neusoft.HISFC.Models.Registration.Schema();
					
					this.objSchema.Templet.ID = this.Reader[2].ToString();
					this.objSchema.Templet.EnumSchemaType = (Neusoft.HISFC.Models.Base.EnumSchemaType)(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[3].ToString()));
					this.objSchema.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4].ToString());
					this.objSchema.Templet.Week = (DayOfWeek)(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[5].ToString()));
					this.objSchema.Templet.Noon.ID = this.Reader[6].ToString();
					this.objSchema.Templet.Dept.ID = this.Reader[7].ToString();
					this.objSchema.Templet.Dept.Name = this.Reader[8].ToString();
					this.objSchema.Templet.Doct.ID = this.Reader[9].ToString();
					this.objSchema.Templet.Doct.Name = this.Reader[10].ToString();
					this.objSchema.Templet.DoctType.ID = this.Reader[11].ToString();					
					this.objSchema.Templet.RegQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12].ToString());
					this.objSchema.RegedQTY = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());
					this.objSchema.Templet.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[14].ToString());
					this.objSchema.Templet.StopReason.ID = this.Reader[15].ToString();
					this.objSchema.Templet.StopReason.Name = this.Reader[16].ToString();
					this.objSchema.Templet.Stop.ID = this.Reader[17].ToString();
					this.objSchema.Templet.Stop.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[18].ToString());
					this.objSchema.Templet.Memo = this.Reader[19].ToString();
					this.objSchema.Templet.Oper.ID = this.Reader[20].ToString();
					this.objSchema.Templet.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[21].ToString());
					this.objSchema.Templet.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[22].ToString()) ;
					this.objSchema.Templet.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[23].ToString()) ;
					this.objSchema.Templet.TelQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[24].ToString()) ;
					this.objSchema.TeledQTY = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[25].ToString()) ;
					this.objSchema.TelingQTY = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26].ToString()) ;
					this.objSchema.Templet.SpeQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27].ToString()) ;
					this.objSchema.SpedQTY = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[28].ToString()) ;
					this.objSchema.Templet.IsAppend = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[29].ToString()) ;
                    this.objSchema.SeeNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[30].ToString());
                    this.objSchema.Templet.RegLevel.ID = this.Reader[31].ToString();
                    this.objSchema.Templet.RegLevel.Name = this.Reader[32].ToString();

                    this.objSchema.Templet.Room.ID = this.Reader[33].ToString();
                    this.objSchema.Templet.Room.Name = this.Reader[34].ToString(); 
                    this.objSchema.Templet.Console.ID = this.Reader[35].ToString();
                    this.objSchema.Templet.Console.Name = this.Reader[36].ToString();
                    this.objSchema.Templet.IsStop = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[37].ToString());
                    if (this.Reader.FieldCount > 38)
                    {
                        this.objSchema.Templet.SchemaDeptCode = Reader[38].ToString();
                        this.objSchema.Templet.SchemaDeptName = Reader[39].ToString();
                    } 
                    if (this.Reader.FieldCount > 40)
                    {
                        this.objSchema.ElderlyVoucherDoctorFlag = Reader[40].ToString();
                    }
					this.al.Add(this.objSchema);
				}

				this.Reader.Close();
			}
			catch(Exception e)
			{
				this.Err = "查询排班信息出错!" + e.Message;
				this.ErrCode = e.Message;
				return null;
			}

			return al;
		}


		/// <summary>
		/// 获取最近的有效排班信息
		/// </summary>
		/// <param name="schemaType"></param>		
		/// <param name="current"></param>
		/// <param name="deptID"></param>
		/// <param name="doctID"></param>
		/// <returns></returns>
		public Neusoft.HISFC.Models.Registration.Schema Query(Neusoft.HISFC.Models.Base.EnumSchemaType schemaType,
				DateTime current, string deptID, string doctID)
		{
			#region 所有情况 太他妈的紧急了,不写sql了， 上线前夜
			/*
			//预约专家
			WHERE see_date >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')
			AND schema_type = '1'
			AND doct_code = '{1}'
			AND valid_flag = '1'
			AND (tel_lmt > tel_reging OR append_flag = '1')
			ORDER BY noon_code,append_flag,begin_time

			//预约专科
			WHERE see_date >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')
			AND schema_type = '0'
			AND dept_code = '{1}'
			AND valid_flag = '1'
			AND (tel_lmt > tel_reging OR append_flag = '1')
			ORDER BY noon_code,append_flag,begin_time

			//专科
			WHERE see_date >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')
			AND schema_type = '0'
			AND dept_code = '{1}'
			AND valid_flag = '1'
			AND (reg_lmt > reged OR append_flag = '1')
			ORDER BY noon_code,append_flag,begin_time

			//专家
			WHERE see_date >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')
			AND schema_type = '1'
			AND doct_code = '{1}'
			AND valid_flag = '1'
			AND (reg_lmt > reged OR append_flag = '1')
			ORDER BY noon_code,append_flag,begin_time

			//特诊
			WHERE see_date >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')
			AND schema_type = '0'
			AND doct_code = '{1}'
			AND valid_flag = '1'
			AND (spe_lmt > spe_reged OR append_flag = '1')
			ORDER BY noon_code,append_flag,begin_time*/
			#endregion

			string where = " WHERE see_date >= to_date('"+ current.Date.ToString() + "','yyyy-mm-dd hh24:mi:ss')" ;

			//排班类型
			where = where +" AND schema_type = '" + ((int)schemaType).ToString() + "'" ;
			
			
			if( schemaType == Neusoft.HISFC.Models.Base.EnumSchemaType.Dept)
			{
				where = where + " AND dept_code = '" + deptID + "'" +
					" AND end_time > to_date('"+current.ToString() + "','yyyy-mm-dd hh24:mi:ss')" +
					" AND valid_flag = '1'" +
					" ORDER BY see_date,noon_code,append_flag,begin_time" ;
			}
			else
			{
				//预约到专家
				where  = where + " AND doct_code = '" + doctID + "'" +
					" AND end_time > to_date('"+current.ToString() + "','yyyy-mm-dd hh24:mi:ss')" +
					" AND valid_flag = '1'" +
					" ORDER BY see_date,noon_code,append_flag,begin_time" ;
			}			

			string sql = "" ;

			if(this.Sql.GetCommonSql("Registration.Schema.Query.WithSchemaDept",ref sql) == -1)return null;

			sql = sql + where ;

			if(this.QueryBase(sql) == null) return null ;

			if(al == null)return null ;

			if(al.Count == 0) return new Neusoft.HISFC.Models.Registration.Schema() ;

			return (Neusoft.HISFC.Models.Registration.Schema)al[0] ;
		}

        /// <summary>
        /// 根据模板id查找end_time小于当前时间的记录数
        /// by niuxy
        /// 2007-05-16
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int QuerySchemaById(string ID)
        {
            string strsql = "";
            int returnValue = this.Sql.GetCommonSql("Registration.Schema.Query.98", ref strsql);
            if (returnValue == -1)
            {
                this.Err = "获得索引为[Registration.Schema.Query.98]的sql语句失败";
                return -1;
            }
            try
            {
                strsql = string.Format(strsql, ID);
            }
            catch (Exception ex)
            {
                this.Err = "[Registration.Schema.Query.98]格式不匹配!" + ex.Message;
                this.ErrCode = ex.Message;
                return -1;    
                
            }

            return  Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(strsql));
           
        }

        public string QueryElderVoucherDoctorFlag(string doctCode)
        {
            string strsql = "SELECT decode(count(1),0,'否','是') FROM com_dictionary d where d.type = 'ELDERLYVOUCHERDOCTOR' and d.valid_state = '1' AND d.code = '{0}'";
            
            try
            {
                strsql = string.Format(strsql, doctCode);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return null;

            }

            return this.ExecSqlReturnOne(strsql);

        }

        /// <summary>
        /// Registration.SchemaWeek.1
        /// </summary>
        /// <returns></returns>
        public DataSet  QuerySchemaForRegister(string fromDate,string toDate,string schemaType,string deptCode,string doctCode )
        {

            DataSet ds = new DataSet ();
            this.ExecQuery("Registration.SchemaForRegister.1", ref ds, fromDate, toDate, schemaType, deptCode, doctCode);
            return ds;

        }

        /// <summary>
        /// 查询某日、某科室出诊专家(带候诊人数)
        /// </summary>
        /// <param name="seeDate"></param>
        /// <param name="endDate"></param>
        /// <param name="deptID"></param>
        /// <returns></returns>
        public DataSet QueryDoct(DateTime seeDate, DateTime endDate, string deptID, DateTime seeDateEnd, string noon)
        {
            string sql = "";
            DataSet ds = new DataSet();

            if (this.Sql.GetCommonSql("Registration.Schema.Query.88", ref sql) == -1) return null;

            try
            {
                sql = string.Format(sql, seeDate.Date.ToString(), endDate.ToString(), deptID, seeDateEnd.Date.ToString(), noon);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Schema.Query.88]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            if (this.ExecQuery(sql, ref ds) == -1) return null;

            return ds;
        }

        /// <summary>
        /// 获取医生当前午别最后一个时段的排班 20191009
        /// {726F4339-DAD0-4263-A279-5D75DF46B58F}
        /// </summary>
        /// <param name="doctCode"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.Schema GetSchemaLastTime(string doctCode)
        {
            string strSql = @"select t.id,t.see_date,t.begin_time,t.end_time,t.reg_lmt,t.reged,t.dept_code,t.dept_name,t.doct_code,t.doct_name from  fin_opr_schema t
                            where t.see_date=trunc(sysdate) and t.noon_code= fun_get_noon(sysdate)
                            and t.schema_type <> '0'
                            and t.doct_code = '{0}'
                            and t.valid_flag = '1'
                            and t.id = (select max(a.id) from fin_opr_schema a where a.see_date=trunc(sysdate) and a.noon_code=fun_get_noon(sysdate)
                            and a.schema_type <> '0'
                            and a.doct_code = '{0}' 
                            and a.valid_flag = '1'
                            )";

            strSql = string.Format(strSql,doctCode);

            if(this.ExecQuery(strSql) == -1) return null;

			this.al = new ArrayList();

            try
            {
                while (this.Reader.Read())
                {
                    this.objSchema = new Neusoft.HISFC.Models.Registration.Schema();

                    this.objSchema.Templet.ID = this.Reader[0].ToString();
                    this.objSchema.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[1].ToString());
                    this.objSchema.Templet.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[2].ToString());
                    this.objSchema.Templet.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[3].ToString());
                    this.objSchema.Templet.RegQuota = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[4].ToString());
                    this.objSchema.RegedQTY = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[5].ToString());
                    this.objSchema.Templet.Dept.ID = this.Reader[6].ToString();
                    this.objSchema.Templet.Dept.Name = this.Reader[7].ToString();
                    this.objSchema.Templet.Doct.ID = this.Reader[8].ToString();
                    this.objSchema.Templet.Doct.Name = this.Reader[9].ToString();

                    al.Add(this.objSchema);
                }
            }
            catch (Exception e)
            {
                this.Err = "查询排班信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            if (al == null || al.Count == 0)
            {
                return null;
            }
            return al[0] as Neusoft.HISFC.Models.Registration.Schema;
        }

        #region 获取医生最后一个有效排班{71927F0A-E4B6-4ce0-B253-865D19C802C8}20191108
        public Neusoft.HISFC.Models.Registration.RegLevel getSchemaRegLevelLast(string doctCode)
        {
            string strSql = @"select t.reglevl_code,t.reglevl_name from fin_opr_schema t where t.doct_code = '{0}'
                              and t.id = (select max(to_number(a.id)) from fin_opr_schema a where a.doct_code = '{0}') and t.valid_flag = '1'";
            strSql = string.Format(strSql,doctCode);
            if (this.ExecQuery(strSql) == -1) return null;
            this.al = new ArrayList();
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Registration.RegLevel obj = new Neusoft.HISFC.Models.Registration.RegLevel();

                    obj.ID = this.Reader[0].ToString();
                    obj.Name = this.Reader[1].ToString();
                    al.Add(obj);
                }
            }
            catch (Exception e)
            {
                this.Err = "查询排班信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            if (al == null || al.Count == 0)
            {
                return null;
            }
            return al[0] as Neusoft.HISFC.Models.Registration.RegLevel;

        }
        #endregion

        #endregion
    }
}
