using System;
using System.Collections;
using System.Collections.Generic;
using Neusoft.HISFC.Models.Operation;
namespace Neusoft.HISFC.BizLogic.Operation
{
	/// <summary>
	/// [功能描述: 麻醉登记控制类]<br></br>
	/// [创 建 者: 王铁全]<br></br>
	/// [创建时间: 2006-09-27]<br></br>
	/// <修改记录
	///		修改人=''
	///		修改时间='yyyy-mm-dd'
	///		修改目的=''
	///		修改描述=''
	///  />
	/// </summary>
	public abstract class AnaeRecord : Neusoft.FrameWork.Management.Database
	{
        private Neusoft.HISFC.BizLogic.Operation.OpsTableManage TableManage = new OpsTableManage();

        /// <summary>
        /// 
        /// </summary>
		public AnaeRecord()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		/// <summary>
        /// 手术申请单控制类实例
		/// </summary>
        protected abstract Neusoft.HISFC.BizLogic.Operation.Operation operationManager
        {
            get;
        }
		/// <summary>
		/// 获得指定序号的麻醉登记记录
		/// </summary>
        /// <param name="operatorNo">手术序号</param>
		/// <returns>麻醉登记记录对象</returns>
		public Neusoft.HISFC.Models.Operation.AnaeRecord GetAnaeRecord( string operatorNo )
		{
			if(operatorNo.Length == 0)
			{
				return null;
			}
			
			string strSql = string.Empty;
			string strWhere = string.Empty;

			if(this.Sql.GetSql("Operator.AnaeRecord.GetAnaeRecord.Select.1",ref strSql) == -1) 
			{
				return null;
			}

			if(this.Sql.GetSql("Operator.AnaeRecord.GetAnaeRecord.Where.2",ref strWhere) == -1) 
			{
				return null;
			}

			strWhere = string.Format(strWhere,operatorNo);
			strSql = strSql + " \n" + strWhere;
			Neusoft.HISFC.Models.Operation.AnaeRecord anaeRecord = new Neusoft.HISFC.Models.Operation.AnaeRecord();
			//先获得关联的手术申请单
			anaeRecord.OperationApplication = operationManager.GetOpsApp(operatorNo);
			//如果手术申请单没有实际值（即可能是补登的麻醉记录），则下面的关于thisOpsRec.m_objOpsApp的赋值还是有意义的。

			//查询SQL语句已经获得，开始查询
			this.ExecQuery(strSql);
			try
			{
				while(this.Reader.Read())
				{
					anaeRecord.OperationApplication.ID = Reader[0].ToString();					//手术序号
					anaeRecord.OperationApplication.PatientInfo.ID  = Reader[1].ToString();//住院流水号/门诊号(如'ZY010000000001')
					//----------------------------------------------------------------------------------------------------------
					anaeRecord.OperationApplication.PatientInfo.PID.ID = Reader[2].ToString();//门诊卡号/病案号
					anaeRecord.OperationApplication.PatientInfo.PID.PatientNO = Reader[2].ToString();//病案号(如'0000000001')
					anaeRecord.OperationApplication.PatientInfo.PID.CardNO = Reader[2].ToString();//门诊卡号(如'0000000001')
					//----------------------------------------------------------------------------------------------------------
					anaeRecord.OperationApplication.PatientInfo.Name = Reader[3].ToString();//姓名
					anaeRecord.OperationApplication.PatientInfo.Sex.ID = Reader[4].ToString();//性别
					anaeRecord.OperationApplication.PatientSouce = Reader[5].ToString();//1门诊/2住院
					anaeRecord.OperationApplication.AnesType.ID = Reader[6].ToString();//麻醉方式
					anaeRecord.AnaeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[7].ToString());//麻醉时间
					//麻醉医师、麻醉助手的信息已经存在于thisAnaeRec.m_objOpsApp.RoleAl中
					//Reader[8] 麻醉医师
					//Reader[9] 麻醉助手
					anaeRecord.AnaeResult.ID = Reader[10].ToString();//麻醉效果
					try
					{
						anaeRecord.IsPACU = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[11].ToString());//是否入PACU,1是 0否 
					}
					catch{}
					anaeRecord.InPacuDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[12].ToString());//入(PACU)室时间
					anaeRecord.InPacuStatus.ID = Reader[13].ToString();//入(PACU)室状态
					anaeRecord.OutPacuDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[14].ToString());//出(PACU)室时间
					anaeRecord.OutPacuStatus.ID = Reader[15].ToString();//入(PACU)室状态
					anaeRecord.Memo = Reader[16].ToString();//备注
					anaeRecord.IsDemulcent = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[17].ToString());//术后镇痛，1是0否
					anaeRecord.DemulcentType.ID = Reader[18].ToString();//镇痛方式
					anaeRecord.DemulcentModel.ID = Reader[19].ToString();//泵型
					anaeRecord.DemulcentDays = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[20].ToString());//镇痛天数
					anaeRecord.PullOutDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[21].ToString());//拔管时间
					anaeRecord.PullOutOperator.ID = Reader[22].ToString();//拔管人
					anaeRecord.DemulcentEffect.ID = Reader[23].ToString();//镇痛效果
					anaeRecord.IsCharged = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[24].ToString());//0未记帐/1已记帐
					anaeRecord.ExecDept.ID = Reader[25].ToString();//执行科室
                    //{C7BDDFBF-BD3A-43c7-8057-432EC8B59338}
                    anaeRecord.Direction = Reader[26].ToString();//术后去向
                    //{26E31402-7D3C-4798-B2BE-C34F06C4FCC7}
                    anaeRecord.DemuDrug = Reader[27].ToString(); //镇痛用药
				}
			}
			catch(Exception ex)
			{
				this.Err="获得麻醉登记单信息出错！"+ex.Message;
				this.ErrCode="-1";
				this.WriteErr();
				return null;
			}
			this.Reader.Close();	
			return anaeRecord;
		}

        /// <summary>
        /// 获得指定序号的麻醉登记记录
        /// </summary>
        /// <param name="operatorNo">手术序号</param>
        /// <returns>麻醉登记记录对象</returns>
        public Neusoft.HISFC.Models.Operation.AnaeRecord GetAnaeRecordNotApp(string operatorNo)
        {
            if (operatorNo.Length == 0)
            {
                return null;
            }

            string strSql = string.Empty;
            string strWhere = string.Empty;

            if (this.Sql.GetSql("Operator.AnaeRecord.GetAnaeRecord.Select.1", ref strSql) == -1)
            {
                return null;
            }

            if (this.Sql.GetSql("Operator.AnaeRecord.GetAnaeRecord.Where.2", ref strWhere) == -1)
            {
                return null;
            }

            strWhere = string.Format(strWhere, operatorNo);
            strSql = strSql + " \n" + strWhere;
            Neusoft.HISFC.Models.Operation.AnaeRecord anaeRecord = new Neusoft.HISFC.Models.Operation.AnaeRecord();
            //先获得关联的手术申请单
            //anaeRecord.OperationApplication = operationManager.GetOpsApp(operatorNo);
            //如果手术申请单没有实际值（即可能是补登的麻醉记录），则下面的关于thisOpsRec.m_objOpsApp的赋值还是有意义的。

            //查询SQL语句已经获得，开始查询
            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    anaeRecord.OperationApplication.ID = Reader[0].ToString();					//手术序号
                    anaeRecord.OperationApplication.PatientInfo.ID = Reader[1].ToString();//住院流水号/门诊号(如'ZY010000000001')
                    //----------------------------------------------------------------------------------------------------------
                    anaeRecord.OperationApplication.PatientInfo.PID.ID = Reader[2].ToString();//门诊卡号/病案号
                    anaeRecord.OperationApplication.PatientInfo.PID.PatientNO = Reader[2].ToString();//病案号(如'0000000001')
                    anaeRecord.OperationApplication.PatientInfo.PID.CardNO = Reader[2].ToString();//门诊卡号(如'0000000001')
                    //----------------------------------------------------------------------------------------------------------
                    anaeRecord.OperationApplication.PatientInfo.Name = Reader[3].ToString();//姓名
                    anaeRecord.OperationApplication.PatientInfo.Sex.ID = Reader[4].ToString();//性别
                    anaeRecord.OperationApplication.PatientSouce = Reader[5].ToString();//1门诊/2住院
                    anaeRecord.OperationApplication.AnesType.ID = Reader[6].ToString();//麻醉方式
                    anaeRecord.AnaeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[7].ToString());//麻醉时间
                    //麻醉医师、麻醉助手的信息已经存在于thisAnaeRec.m_objOpsApp.RoleAl中
                    //Reader[8] 麻醉医师
                    //Reader[9] 麻醉助手
                    anaeRecord.AnaeResult.ID = Reader[10].ToString();//麻醉效果
                    try
                    {
                        anaeRecord.IsPACU = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[11].ToString());//是否入PACU,1是 0否 
                    }
                    catch { }
                    anaeRecord.InPacuDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[12].ToString());//入(PACU)室时间
                    anaeRecord.InPacuStatus.ID = Reader[13].ToString();//入(PACU)室状态
                    anaeRecord.OutPacuDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[14].ToString());//出(PACU)室时间
                    anaeRecord.OutPacuStatus.ID = Reader[15].ToString();//入(PACU)室状态
                    anaeRecord.Memo = Reader[16].ToString();//备注
                    anaeRecord.IsDemulcent = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[17].ToString());//术后镇痛，1是0否
                    anaeRecord.DemulcentType.ID = Reader[18].ToString();//镇痛方式
                    anaeRecord.DemulcentModel.ID = Reader[19].ToString();//泵型
                    anaeRecord.DemulcentDays = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[20].ToString());//镇痛天数
                    anaeRecord.PullOutDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[21].ToString());//拔管时间
                    anaeRecord.PullOutOperator.ID = Reader[22].ToString();//拔管人
                    anaeRecord.DemulcentEffect.ID = Reader[23].ToString();//镇痛效果
                    anaeRecord.IsCharged = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[24].ToString());//0未记帐/1已记帐
                    anaeRecord.ExecDept.ID = Reader[25].ToString();//执行科室
                    //{C7BDDFBF-BD3A-43c7-8057-432EC8B59338}
                    anaeRecord.Direction = Reader[26].ToString();//术后去向
                    //{26E31402-7D3C-4798-B2BE-C34F06C4FCC7}
                    anaeRecord.DemuDrug = Reader[27].ToString(); //镇痛用药
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得麻醉登记单信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();
            return anaeRecord;
        }
		/// <summary>
		/// 查询指定时间段内的麻醉登记记录列表
		/// </summary>
		/// <param name="ExeDeptID">string 执行科室代码</param>
		/// <param name="BeginDate">DateTime 起始时间</param>
		/// <param name="EndDate">DateTime 截至时间</param>
		/// <returns>麻醉登记记录列表（元素为Neusoft.HISFC.Models.Operation.AnaeRecord类型）</returns>
		public ArrayList GetAnaeRecords(string ExeDeptID,DateTime BeginDate,DateTime EndDate)
		{
			ArrayList AnaeRecordAl = new ArrayList();
			string strSql = string.Empty;
			string strWhere = string.Empty;
			if(this.Sql.GetSql("Operator.AnaeRecord.GetAnaeRecord.Select.1",ref strSql) == -1) 
			{
				return AnaeRecordAl;
			}

			if(this.Sql.GetSql("Operator.AnaeRecord.GetAnaeRecord.Where.1",ref strWhere) == -1) 
			{
				return AnaeRecordAl;
			}

			strWhere = string.Format(strWhere,ExeDeptID,BeginDate.ToString(),EndDate.ToString());
			strSql = strSql + " \n" + strWhere;
			//查询SQL语句已经获得，开始查询啦，大家注意啦！
			this.ExecQuery(strSql);
			try
			{
				while(this.Reader.Read())
				{
					Neusoft.HISFC.Models.Operation.AnaeRecord thisAnaeRec = new Neusoft.HISFC.Models.Operation.AnaeRecord();
					
					thisAnaeRec.OperationApplication.ID = Reader[0].ToString();					//手术序号
					//先获得关联的手术申请单
					thisAnaeRec.OperationApplication = operationManager.GetOpsApp(thisAnaeRec.OperationApplication.ID);
					//如果手术申请单没有实际值（即可能是补登的麻醉记录），则下面的关于thisOpsRec.m_objOpsApp的赋值还是有意义的。

					thisAnaeRec.OperationApplication.PatientInfo.ID  = Reader[1].ToString();//住院流水号/门诊号(如'ZY010000000001')
					//----------------------------------------------------------------------------------------------------------
					thisAnaeRec.OperationApplication.PatientInfo.PID.ID = Reader[2].ToString();//门诊卡号/病案号
					thisAnaeRec.OperationApplication.PatientInfo.PID.PatientNO = Reader[2].ToString();//病案号(如'0000000001')
					thisAnaeRec.OperationApplication.PatientInfo.PID.CardNO = Reader[2].ToString();//门诊卡号(如'0000000001')
					//----------------------------------------------------------------------------------------------------------
					thisAnaeRec.OperationApplication.PatientInfo.Name = Reader[3].ToString();//姓名
					thisAnaeRec.OperationApplication.PatientInfo.Sex.ID = Reader[4].ToString();//性别
					thisAnaeRec.OperationApplication.PatientSouce = Reader[5].ToString();//1门诊/2住院
					thisAnaeRec.OperationApplication.AnesType.ID = Reader[6].ToString();//麻醉方式
					thisAnaeRec.AnaeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[7].ToString());//麻醉时间
					//麻醉医师、麻醉助手的信息已经存在于thisAnaeRec.m_objOpsApp.RoleAl中
					//Reader[8] 麻醉医师
					//Reader[9] 麻醉助手
					thisAnaeRec.AnaeResult.ID = Reader[10].ToString();//麻醉效果
					try
					{
						thisAnaeRec.IsPACU = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[11].ToString());//是否入PACU,1是 0否 
					}
					catch{}
					thisAnaeRec.InPacuDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[12].ToString());//入(PACU)室时间
					thisAnaeRec.InPacuStatus.ID = Reader[13].ToString();//入(PACU)室状态
					thisAnaeRec.OutPacuDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[14].ToString());//出(PACU)室时间
					thisAnaeRec.OutPacuStatus.ID = Reader[15].ToString();//入(PACU)室状态
					thisAnaeRec.Memo = Reader[16].ToString();//备注
					thisAnaeRec.IsDemulcent = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[17].ToString());//术后镇痛，1是0否
					thisAnaeRec.DemulcentType.ID = Reader[18].ToString();//镇痛方式
					thisAnaeRec.DemulcentModel.ID = Reader[19].ToString();//泵型
					thisAnaeRec.DemulcentDays = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[20].ToString());//镇痛天数
					thisAnaeRec.PullOutDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[21].ToString());//拔管时间
					thisAnaeRec.PullOutOperator.ID = Reader[22].ToString();//拔管人
					thisAnaeRec.DemulcentEffect.ID = Reader[23].ToString();//镇痛效果
					thisAnaeRec.IsCharged = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[24].ToString());//0未记帐/1已记帐
					thisAnaeRec.ExecDept.ID = Reader[25].ToString();//执行科室
                    //{C7BDDFBF-BD3A-43c7-8057-432EC8B59338}
                    thisAnaeRec.Direction = Reader[26].ToString();//术后去向
                    //{26E31402-7D3C-4798-B2BE-C34F06C4FCC7}
                    thisAnaeRec.DemuDrug = Reader[27].ToString(); //镇痛用药
					AnaeRecordAl.Add(thisAnaeRec);
				}
			}
			catch(Exception ex)
			{
				this.Err="获得麻醉登记单信息出错！"+ex.Message;
				this.ErrCode="-1";
				this.WriteErr();
				AnaeRecordAl.Clear();
				return AnaeRecordAl;
			}
			this.Reader.Close();	
			return AnaeRecordAl;
		}
		#region 麻醉登记单操作
		/// <summary>
		/// 新增麻醉登记
		/// </summary>
		/// <param name="AnaeRecord">麻醉登记单对象</param>
		/// <returns>0 success -1 fail</returns>
		public int AddAnaeRecord(Neusoft.HISFC.Models.Operation.AnaeRecord anaeRecord)
		{
			string strSql = string.Empty;	
			#region 获取患者基本信息
			//--------------------------------------------------------		
			//局部变量定义
			string ls_ClinicCode = string.Empty;//住院流水号/门诊号
			string ls_PatientNo = string.Empty; //病案号/病历号
			string ls_Name = string.Empty;	  //患者姓名
			string ls_Sex = string.Empty;		  //性别
			Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp;
			OpsApp = anaeRecord.OperationApplication;
			
			ls_ClinicCode = OpsApp.PatientInfo.ID;
			ls_PatientNo = OpsApp.PatientInfo.PID.ID;
			ls_Name =  OpsApp.PatientInfo.Name;
			ls_Sex =  OpsApp.PatientInfo.Sex.ID.ToString();			
			//--------------------------------------------------------
			#endregion			
			//bool标志值转换
			string strIsPACU = Neusoft.FrameWork.Function.NConvert.ToInt32(anaeRecord.IsPACU).ToString();
			string strDemulcent = Neusoft.FrameWork.Function.NConvert.ToInt32(anaeRecord.IsDemulcent).ToString();
			string strChargeFlag = Neusoft.FrameWork.Function.NConvert.ToInt32(anaeRecord.IsCharged).ToString();
			if(this.Sql.GetSql("Operator.AnaeRecord.AddAnaeRecord.1",ref strSql)==-1) 
			{
				return -1;
			}

			try
			{				
				//手术登记表中增加记录
				//每行5个参数
				strSql = string.Format(strSql,OpsApp.ID,ls_ClinicCode,ls_PatientNo,ls_Name,ls_Sex,OpsApp.PatientSouce,
					OpsApp.AnesType.ID.ToString(),anaeRecord.AnaeDate.ToString(),"","",anaeRecord.AnaeResult.ID.ToString(),
					strIsPACU,anaeRecord.InPacuDate.ToString(),anaeRecord.InPacuStatus.ID.ToString(),anaeRecord.OutPacuDate.ToString(),anaeRecord.OutPacuStatus.ID.ToString(),
					anaeRecord.Memo,strDemulcent,anaeRecord.DemulcentType.ID.ToString(),anaeRecord.DemulcentModel.ID.ToString(),anaeRecord.DemulcentDays.ToString(),
					anaeRecord.PullOutDate.ToString(),anaeRecord.PullOutOperator.ID.ToString(),anaeRecord.DemulcentEffect.ID.ToString(),strChargeFlag,this.Operator.ID.ToString(),
					anaeRecord.ExecDept.ID.ToString(),
                    //{C7BDDFBF-BD3A-43c7-8057-432EC8B59338}
                    anaeRecord.Direction,
                    //{26E31402-7D3C-4798-B2BE-C34F06C4FCC7}
                    anaeRecord.DemuDrug);
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;
				this.ErrCode = ex.Message;
				return -1;            
			}
			if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
		}
		/// <summary>
		/// 更新麻醉登记信息
		/// </summary>
		/// <param name="AnaeRecord">麻醉登记实体对象</param>
		/// <returns>0 success -1 fail</returns>
		public int UpdateAnaeRecord(Neusoft.HISFC.Models.Operation.AnaeRecord AnaeRecord)
		{
			string strSql = string.Empty;	
			#region 获取患者基本信息
			//--------------------------------------------------------		
			//局部变量定义
			string ls_ClinicCode = string.Empty;//住院流水号/门诊号
			string ls_PatientNo = string.Empty; //病案号/病历号
			string ls_Name = string.Empty;	  //患者姓名
			string ls_Sex = string.Empty;		  //性别
			Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp = new Neusoft.HISFC.Models.Operation.OperationAppllication();
			OpsApp = AnaeRecord.OperationApplication;
			
			ls_ClinicCode = OpsApp.PatientInfo.ID;
			ls_PatientNo = OpsApp.PatientInfo.PID.ID;
			ls_Name =  OpsApp.PatientInfo.Name;
			ls_Sex =  OpsApp.PatientInfo.Sex.ID.ToString();			
			//--------------------------------------------------------
			#endregion			
			//bool标志值转换
			string strIsPACU = Neusoft.FrameWork.Function.NConvert.ToInt32(AnaeRecord.IsPACU).ToString();
			string strDemulcent = Neusoft.FrameWork.Function.NConvert.ToInt32(AnaeRecord.IsDemulcent).ToString();
			string strChargeFlag = Neusoft.FrameWork.Function.NConvert.ToInt32(AnaeRecord.IsCharged).ToString();
			if(this.Sql.GetSql("Operator.AnaeRecord.UpdateAnaeRecord.1",ref strSql)==-1) 
			{
				return -1;
			}

			try
			{				
				//手术登记表中增加记录
				//每行5个参数
				strSql = string.Format(strSql,OpsApp.ID,ls_ClinicCode,ls_PatientNo,ls_Name,ls_Sex,OpsApp.PatientSouce,
					OpsApp.AnesType.ID.ToString(),AnaeRecord.AnaeDate.ToString(),"","",AnaeRecord.AnaeResult.ID.ToString(),
					strIsPACU,AnaeRecord.InPacuDate.ToString(),AnaeRecord.InPacuStatus.ID.ToString(),AnaeRecord.OutPacuDate.ToString(),AnaeRecord.OutPacuStatus.ID.ToString(),
					AnaeRecord.Memo,strDemulcent,AnaeRecord.DemulcentType.ID.ToString(),AnaeRecord.DemulcentModel.ID.ToString(),AnaeRecord.DemulcentDays.ToString(),
					AnaeRecord.PullOutDate.ToString(),AnaeRecord.PullOutOperator.ID.ToString(),AnaeRecord.DemulcentEffect.ID.ToString(),strChargeFlag,this.Operator.ID.ToString(),
                    AnaeRecord.ExecDept.ID.ToString(), 
                    //{C7BDDFBF-BD3A-43c7-8057-432EC8B59338}
                    AnaeRecord.Direction,
                    //{26E31402-7D3C-4798-B2BE-C34F06C4FCC7}
                    AnaeRecord.DemuDrug);
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;
				this.ErrCode = ex.Message;
				return -1;            
			}
			if (strSql == null) return -1;	
			
			if(this.ExecNoQuery(strSql) == -1) return -1;
			return 0;
		}
		#endregion
		/// <summary>
		/// 获取是否允许修改手术登记标志
		/// </summary>
		/// <returns>标志1允许修改 0不许修改，若为Error,则系统参数未设置</returns>
		public string GetModifyEnabled()
		{
			string strSql = string.Empty;
			string strFlag = string.Empty;
			if(this.Sql.GetSql("Operator.OpsRecord.GetRecordModifyFlag.1",ref strSql) == -1) 
			{
				return strFlag;				
			}

			this.ExecQuery(strSql);
			try
			{
				while(this.Reader.Read())
				{
					strFlag = this.Reader[0].ToString();
				}
			}
			catch(Exception ex)
			{
				this.Err = ex.Message;
				this.ErrCode = ex.Message;
				this.WriteErr();
				return "Error";            
			}
			this.Reader.Close();		
			if(strFlag == "") 
			{
				this.Err = "系统未维护是否允许修改麻醉登记记录参数，请联系系统管理员！";
				this.ErrCode = "系统未维护是否允许修改麻醉登记记录参数，请联系系统管理员！";	
				this.WriteErr();
				return "Error";
			}
			return strFlag;
		}

        #region {5F37177C-DE87-4b3e-9041-07A786B55D81}

        /// <summary>
        /// 置麻醉登记收费标志
        /// </summary>
        /// <param name="operationNo"></param>
        /// <returns></returns>
        public int UpdateAnaeFee(string operationNo)
        {
            string sql = string.Empty;

            if (this.Sql.GetSql("Operator.AnaeRecord.UpdateAnaeRecordFee.1", ref sql) == -1)
            {
                return -1;
            }

            try
            {
                sql = string.Format(sql, operationNo);

                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "置麻醉登记记录收费标志出错[Operator.AnaeRecord.UpdateAnaeRecordFee.1]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        #endregion

        #region 手术相关

        /// <summary>
        /// 获取所有未标记已麻醉过的手术申请单信息 (重载)
        /// 麻醉登记用
        /// </summary>
        /// <param name="beginTime">查询起始时间</param>
        /// <param name="endTime">查询截至时间</param>
        /// <param name="Valid">0无效 1 有效</param>
        /// <returns>手术申请单对象数组</returns>
        public ArrayList GetOpsAppList(DateTime beginTime, DateTime endTime, string Valid)
        {
            ArrayList myAl = new ArrayList();
            //业务规则：遴选出手术时间小于给定时间的所有有效的已进行过手术安排的手术申请单。			
            string strSql = string.Empty;

            if (this.Sql.GetSql("Operator.AnaeRecord.GetOpsApplication.1", ref strSql) == -1)
            {
                return myAl;
            }

            try
            {
                strSql = string.Format(strSql, beginTime.ToString(), endTime.ToString(), Valid);
            }
            catch (Exception ex)
            {
                this.Err = "Operator.AnaeRecord.GetOpsApplication.1";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }

        /// <summary>
        /// 获得给定SQL语句查询出的申请单对象数组
        /// </summary>
        /// <param name="strSql">指定的查询语句</param>
        /// <returns>手术申请单对象数组</returns>
        private ArrayList GetOpsAppListFromSql(string strSql)
        {
            ArrayList myAl = new ArrayList();

            //			Neusoft.HISFC.BizLogic.Manager.Person Person = new Neusoft.HISFC.BizLogic.Manager.Person();
            //			Neusoft.HISFC.BizLogic.Manager.Department Department = new Neusoft.HISFC.BizLogic.Manager.Department();

            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Operation.OperationAppllication opsApplication = new Neusoft.HISFC.Models.Operation.OperationAppllication();
                    opsApplication.ID = Reader[0].ToString();					//手术序号					

                    opsApplication.OperationDoctor.ID = Reader[1].ToString();	//手术医生				
                    opsApplication.OperationDoctor.Name = this.GetEmployeeName(opsApplication.OperationDoctor.ID);

                    opsApplication.GuideDoctor.ID = Reader[2].ToString();		//指导医生	

                    opsApplication.PreDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[3].ToString());		//手术预约时间					

                    if (Reader.IsDBNull(4))
                        opsApplication.Duration = 0m;
                    else
                        opsApplication.Duration = System.Convert.ToDecimal(Reader[4].ToString());		//手术预定用时					

                    opsApplication.AnesType.ID = Reader[5].ToString();					//麻醉类型					

                    opsApplication.ExeDept.ID = Reader[6].ToString();//执行科室					

                    opsApplication.OperateRoom =
                        opsApplication.ExeDept as Neusoft.HISFC.Models.Base.Department;	//手术室(对于需要填申请单的手术来说，手术室即执行科室)

                    opsApplication.TableType = Reader[7].ToString();					//0正台1加台2点台					

                    opsApplication.ApplyDoctor.ID = Reader[8].ToString();				//申请医生
                    opsApplication.ApplyDoctor.Name = this.GetEmployeeName(opsApplication.ApplyDoctor.ID);


                    opsApplication.ApplyDoctor.Dept.ID = Reader[9].ToString();//申请科室

                    opsApplication.ApplyDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[10].ToString());	//申请时间
                    opsApplication.ApplyNote = Reader[11].ToString();					//申请备注					

                    opsApplication.ApproveDoctor.ID = Reader[12].ToString();//审批医生

                    opsApplication.ApproveDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[13].ToString());	//审批时间
                    opsApplication.ApproveNote = Reader[14].ToString();					//审批备注					
                    opsApplication.OperationType.ID = Reader[15].ToString();				//手术规模
                    opsApplication.InciType.ID = Reader[16].ToString();					//切口类型					

                    string strGerm = Reader[17].ToString();						//1 有菌 0无菌
                    opsApplication.IsGermCarrying = Neusoft.FrameWork.Function.NConvert.ToBoolean(strGerm);

                    opsApplication.ScreenUp = Reader[18].ToString();					//1 幕上 2 幕下					
                    opsApplication.BloodType.ID = Reader[19].ToString();				//血液成分					
                    if (Reader.IsDBNull(20))
                        opsApplication.BloodNum = 0m;
                    else
                        opsApplication.BloodNum = System.Convert.ToDecimal(Reader[20].ToString());		//血量

                    opsApplication.BloodUnit = Reader[21].ToString();					//用血单位
                    opsApplication.OpsNote = Reader[22].ToString();						//手术注意事项
                    opsApplication.AneNote = Reader[23].ToString();						//麻醉注意事项					
                    opsApplication.ExecStatus = Reader[24].ToString();					//1手术申请 2 手术审批 3手术安排 4手术完成

                    string strFinished = Reader[25].ToString();						//0未做手术/1已做手术
                    opsApplication.IsFinished = Neusoft.FrameWork.Function.NConvert.ToBoolean(strFinished);

                    string strAnesth = Reader[26].ToString();					//0未麻醉/1已麻醉
                    opsApplication.IsAnesth = Neusoft.FrameWork.Function.NConvert.ToBoolean(strAnesth);

                    opsApplication.Folk = Reader[27].ToString();						//签字家属
                    opsApplication.RelaCode.ID = Reader[28].ToString();					//家属关系
                    opsApplication.FolkComment = Reader[29].ToString();					//家属意见					

                    string strUrgent = Reader[30].ToString();					//加急手术,1是/0否
                    opsApplication.IsUrgent = Neusoft.FrameWork.Function.NConvert.ToBoolean(strUrgent);

                    string strChange = Reader[31].ToString();					//1病危/0否
                    opsApplication.IsChange = Neusoft.FrameWork.Function.NConvert.ToBoolean(strChange);

                    string strHeavy = Reader[32].ToString();						//1重症/0否
                    opsApplication.IsHeavy = Neusoft.FrameWork.Function.NConvert.ToBoolean(strHeavy);

                    string strSpecial = Reader[33].ToString();					//1特殊手术/0否
                    opsApplication.IsSpecial = Neusoft.FrameWork.Function.NConvert.ToBoolean(strSpecial);

                    opsApplication.User.ID = Reader[34].ToString();	//操作员

                    opsApplication.IsUnite = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[35].ToString());//1合并/0否

                    opsApplication.OperateKind = Reader[37].ToString();					//1普通2急诊3感染
                    opsApplication.PatientSouce = Reader[38].ToString();					//1门诊/2住院					
                    //try
                    //{
                    //thisOpsApp.PatientInfo.Patient.PID.PatientNo = Reader[39].ToString();//住院号
                    //thisOpsApp.PatientInfo.Patient.ID  = this.GetInPatientNo(thisOpsApp.PatientInfo.Patient.PID.PatientNo);//住院流水号
                    //}
                    //catch{}

                    opsApplication.PatientInfo.ID = Reader[39].ToString();//门诊号/住院流水号
                    if (opsApplication.PatientSouce == "2")
                    {
                        opsApplication.PatientInfo = this.GetPatientInfo(opsApplication.PatientInfo.ID);
                    }
                    else
                    {
                        Neusoft.HISFC.Models.Registration.Register regObj = this.GetRegInfo(opsApplication.PatientInfo.ID);
                        Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
                        patientInfo.ID = regObj.ID;//流水号
                        patientInfo.PID.PatientNO = regObj.PID.CardNO;//卡号
                        patientInfo.PID.CardNO = regObj.PID.CardNO;//卡号
                        patientInfo.Name = regObj.Name;//姓名
                        patientInfo.Birthday = regObj.Birthday;
                        patientInfo.Sex.ID = regObj.Sex.ID;
                        if (regObj.SeeDoct.Dept.ID == null || regObj.SeeDoct.Dept.ID == "")
                        {
                            patientInfo.PVisit.PatientLocation.Dept.ID = regObj.DoctorInfo.Templet.Dept.ID;
                            patientInfo.PVisit.PatientLocation.Dept.Name = regObj.DoctorInfo.Templet.Dept.Name;
                        }
                        else
                        {
                            patientInfo.PVisit.PatientLocation.Dept.ID = regObj.SeeDoct.Dept.ID;
                        }
                        patientInfo.Pact.PayKind.ID = regObj.Pact.PayKind.ID;
                        opsApplication.PatientInfo = patientInfo;
                    }
                    //-----------------------------------------------------------------------------------
                    opsApplication.PatientInfo.PID.ID = Reader[40].ToString();//门诊卡号/住院号
                    opsApplication.PatientInfo.PID.CardNO = Reader[40].ToString();//门诊卡号
                    opsApplication.PatientInfo.PID.PatientNO = Reader[40].ToString();//住院号
                    //-----------------------------------------------------------------------------------
                    opsApplication.PatientInfo.Name = Reader[41].ToString();	//姓名
                    opsApplication.PatientInfo.Sex.ID = Reader[42].ToString();	//性别
                    opsApplication.PatientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[43].ToString());//生日					

                    if (Reader.IsDBNull(44))
                        opsApplication.PatientInfo.FT.PrepayCost = 0m;
                    else
                        opsApplication.PatientInfo.FT.PrepayCost = System.Convert.ToDecimal(Reader[44].ToString());//预交金

                    opsApplication.PatientInfo.PVisit.PatientLocation.Dept.ID = Reader[45].ToString();//住院科室

                    opsApplication.PatientInfo.PVisit.PatientLocation.Bed.ID = Reader[46].ToString();//病床号
                    opsApplication.PatientInfo.BloodType.ID = Reader[47].ToString();//血型					
                    try
                    {
                        opsApplication.OpsTable.ID = Reader[48].ToString();				//手术台
                        opsApplication.OpsTable.Name =
                            this.TableManage.GetTableNameFromID(opsApplication.OpsTable.ID.ToString());
                    }
                    catch { }

                    string strIsNeedAcco = Reader[49].ToString();					//是否需要随台护士
                    opsApplication.IsAccoNurse = Neusoft.FrameWork.Function.NConvert.ToBoolean(strIsNeedAcco);

                    string strIsNeedPrep = Reader[50].ToString();					//是否需要巡回护士
                    opsApplication.IsPrepNurse = Neusoft.FrameWork.Function.NConvert.ToBoolean(strIsNeedPrep);

                    opsApplication.RoomID = Reader[51].ToString();
                    opsApplication.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[52].ToString());//1有效0无效	
                    opsApplication.OperationDoctor.Dept.ID = Reader[54].ToString();
                    ////{B9DDCC10-3380-4212-99E5-BB909643F11B}
                    opsApplication.AnesWay = Reader[55].ToString();
                    //{F0B32D1F-99B6-4b1a-8393-C1F89B98543B}
                    opsApplication.Position = Reader[56].ToString();
                    opsApplication.Eneity = Reader[57].ToString();
                    opsApplication.LastTime = Reader[58].ToString();
                    opsApplication.IsOlation = Reader[59].ToString();
                    opsApplication.PatientInfo.Age = Reader[60].ToString();
                    myAl.Add(opsApplication);

                }
            }
            catch (Exception ex)
            {
                this.Err = "获得患者手术申请单信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return myAl;
            }
            this.Reader.Close();
            try
            {
                foreach (Neusoft.HISFC.Models.Operation.OperationAppllication opsApp in myAl)
                {
                    opsApp.DiagnoseAl = this.GetIcdFromApp(opsApp);	//诊断列表					
                    opsApp.OperationInfos = GetOpsInfoFromApp(opsApp.ID);//手术项目信息列表				
                    opsApp.RoleAl = GetRoleFromApp(opsApp.ID);//人员角色列表
                    //冗余属性赋值，为突出表现层申请部分业务调用方便
                    foreach (Neusoft.HISFC.Models.Operation.ArrangeRole thisRole in opsApp.RoleAl)
                    {
                        if (thisRole.RoleType.ID.ToString() == Neusoft.HISFC.Models.Operation.EnumOperationRole.Helper1.ToString()
                            || thisRole.RoleType.ID.ToString() == Neusoft.HISFC.Models.Operation.EnumOperationRole.Helper2.ToString()
                            || thisRole.RoleType.ID.ToString() == Neusoft.HISFC.Models.Operation.EnumOperationRole.Helper3.ToString())
                            //助手医师列表
                            opsApp.HelperAl.Add(thisRole.Clone());
                    }
                    //thisOpsApp.AppaRecAl = GetAppaRecFromApp(thisOpsApp.OperationNo);//手术资料安排列表
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得患者手术列表信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return myAl;
            }
            return myAl;
        }

        /// <summary>
        ///根据手术序号获得手术项目信息列表
        /// </summary>
        /// <param name="OperatorNo">手术序号</param>
        /// <returns>患者的项目信息对象数组</returns>
        public List<OperationInfo> GetOpsInfoFromApp(string operationNO)
        {
            List<OperationInfo> InfoAl = new List<OperationInfo>();
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetOpsInfoFromApp.1", ref strSql) == -1)
            {
                return InfoAl;//空数组
            }

            try
            {
                strSql = string.Format(strSql, operationNO);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsInfoFromApp";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return InfoAl;
            }

            if (strSql == null)
            {
                return InfoAl;
            }

            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Operation.OperationInfo thisOperateInfo = new Neusoft.HISFC.Models.Operation.OperationInfo();
                    thisOperateInfo.OperationItem.ID = Reader[0].ToString();//项目编码
                    thisOperateInfo.OperationItem.Name = Reader[1].ToString();//项目名称
                    if (Reader.IsDBNull(2) == false)
                        thisOperateInfo.OperationItem.Price = System.Convert.ToDecimal(Reader[2].ToString());//单价
                    if (Reader.IsDBNull(3) == false)
                        thisOperateInfo.FeeRate = System.Convert.ToDecimal(Reader[3].ToString());//收费比例
                    if (Reader.IsDBNull(4) == false)
                        thisOperateInfo.Qty = System.Convert.ToInt16(Reader[4]);//数量

                    thisOperateInfo.StockUnit = Reader[5].ToString();//单位
                    thisOperateInfo.OperateType.ID = Reader[6].ToString();//手术规模
                    thisOperateInfo.InciType.ID = Reader[7].ToString();//切口类型
                    thisOperateInfo.OpePos.ID = Reader[8].ToString();//手术部位
                    thisOperateInfo.IsMainFlag = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[9].ToString());//主手术标志 1是/0否

                    thisOperateInfo.IsValid = true;
                    InfoAl.Add(thisOperateInfo);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得手术项目信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return InfoAl;
            }
            this.Reader.Close();
            return InfoAl;
        }
        /// <summary>
        /// 根据手术序号获得人员角色安排列表
        /// </summary>
        /// <param name="OperatorNo">手术申请单序号</param>
        /// <returns>指定的手术人员安排类对象数组</returns>
        public ArrayList GetRoleFromApp(string OperatorNo)
        {
            ArrayList RoleAl = new ArrayList();
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetRoleFromApp.1", ref strSql) == -1)
            {
                return RoleAl;//空数组
            }

            try
            {
                strSql = string.Format(strSql, OperatorNo);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetRoleFromApp";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return RoleAl;
            }
            if (strSql == null)
            {
                return RoleAl;
            }

            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Operation.ArrangeRole thisRole = new Neusoft.HISFC.Models.Operation.ArrangeRole();
                    thisRole.RoleType.ID = Reader[0].ToString();		//角色编码
                    thisRole.ID = Reader[1].ToString();			//人员编码
                    thisRole.Name = Reader[2].ToString();		//人员姓名
                    thisRole.ForeFlag = Reader[3].ToString();			//0术前安排1术后记录
                    if (thisRole.ForeFlag == string.Empty || thisRole.ForeFlag == null)
                        thisRole.ForeFlag = "0";
                    thisRole.RoleOperKind.ID = Reader[4].ToString();//人员状态
                    //{69F783B4-65EB-4cc3-B489-2A7D5B5A5F00}接替时间
                    thisRole.SupersedeDATE = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[5].ToString());
                    RoleAl.Add(thisRole);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得手术人员角色信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return RoleAl;
            }
            this.Reader.Close();
            return RoleAl;
        }

        #endregion

        protected abstract Neusoft.HISFC.Models.RADT.PatientInfo GetPatientInfo(string id);
        protected abstract Neusoft.HISFC.Models.Registration.Register GetRegInfo(string id);
        protected abstract string GetEmployeeName(string id);
        public abstract ArrayList GetIcdFromApp(Neusoft.HISFC.Models.Operation.OperationAppllication opsApp);
    }
}
