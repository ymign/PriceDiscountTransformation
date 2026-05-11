using System;
using System.Collections;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.BizLogic.Manager;


namespace Neusoft.HISFC.BizLogic.RADT 
{
	/// <summary>
	/// 住院床位日报管理类
	/// writed by cuipeng
	/// 2005-3
	/// </summary>
	public class InpatientDayReport : Neusoft.FrameWork.Management.Database 
	{
		public InpatientDayReport() 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

	
		#region 住院日报汇总表
		/// <summary>
		/// 取全院某一天的住院日报数据
		/// </summary>
		/// <param name="dateBegin">起始日期</param>
		/// <param name="dateEnd">终止日期</param>
		/// <returns>住院日报数组，出错返回null</returns>
		public ArrayList GetInpatientDayReportList(DateTime dateBegin, DateTime dateEnd) 
		{
			string strSQL = "";
			//string strWhere = "";
			//取SELECT语句
			if (this.Sql.GetCommonSql("Case.InpatientDayReport.GetInpatientDayReportList",ref strSQL) == -1) 
			{
				this.Err="没有找到Case.InpatientDayReport.GetInpatientDayReportList字段!";
				return null;
			}

			//格式化SQL语句
			try 
			{
				strSQL = string.Format(strSQL, dateBegin.ToString(), dateEnd.ToString(),"AAAA");
			}
			catch (Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.InpatientDayReport.GetInpatientDayReportList:" + ex.Message;
				return null;
			}

			//取住院日报数据
			return this.myGetInpatientDayReport(strSQL);
		}


		
		/// <summary>
		/// 取全院某一天的住院日报数据
		/// </summary>
		/// <param name="dateStat">日报的日期</param>
		/// <returns>住院日报数组，出错返回null</returns>
		public ArrayList GetInpatientDayReportList(DateTime dateStat) 
		{
            //return this.GetInpatientDayReportList(Convert.ToDateTime(dateStat.ToShortDateString() + " 00:00:00"), Convert.ToDateTime(dateStat.ToShortDateString() + " 23:59:59"));
            return this.GetInpatientDayReportList(Convert.ToDateTime(dateStat.ToShortDateString() + " 00:00:00"), Convert.ToDateTime(dateStat.AddDays(1).ToShortDateString() + " 00:00:00"));

		}


		/// <summary>
		/// 取某一科室某一天的住院日报数据
		/// </summary>
		/// <param name="dateStat">统计大类</param>
		/// <param name="deptCode">科室编码</param>
		/// <param name="nurseStation">护理站编码</param>
		/// <returns>住院日报实体</returns>
		public Neusoft.HISFC.Models.HealthRecord.InpatientDayReport GetInpatientDayReport(DateTime dateStat, string deptCode, string nurseStation) 
		{
			string strSQL = "";
			//取SELECT语句
			if (this.Sql.GetCommonSql("Case.InpatientDayReport.GetInpatientDayReport",ref strSQL) == -1) 
			{
				this.Err="没有找到Case.InpatientDayReport.GetInpatientDayReport字段!";
				return null;
			}

			string strWhere= "";
			//取Where语句
			if (this.Sql.GetCommonSql("Case.InpatientDayReport.GetInpatientDayReport.ByDept",ref strWhere) == -1) 
			{
				this.Err="没有找到Case.InpatientDayReport.GetInpatientDayReport.ByDept字段!";
				return null;
			}

			//格式化SQL语句
			try 
			{
				strSQL = string.Format(strSQL + strWhere, Convert.ToDateTime(dateStat.ToShortDateString() + " 00:00:00"), Convert.ToDateTime(dateStat.AddDays(1).ToShortDateString()+ " 00:00:00"), deptCode, nurseStation);
			}
			catch (Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.InpatientDayReport.GetdayReportBill:" + ex.Message;
				return null;
			}

			//执行SQL语句，取住院日报单数据
			ArrayList al = this.myGetInpatientDayReport(strSQL);
			if (al == null) 
			{
				this.Err = "取住院日报单列时出错：" + this.Err;
				return null;
			}

			//如果没有找到数据，则返回新建的对象，否则返回数组首项
			if (al.Count == 0) 
				return new Neusoft.HISFC.Models.HealthRecord.InpatientDayReport();
			else
				return al[0] as Neusoft.HISFC.Models.HealthRecord.InpatientDayReport;
		}


		/// <summary>
		/// 向住院日报汇总表中插入一条记录
		/// </summary>
		/// <param name="dayReport">住院日报实体</param>
		/// <returns>0没有更新 1成功 -1失败</returns>
		public int InsertInpatientDayReport(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport) 
		{
			string strSQL="";
			if(this.Sql.GetCommonSql("Case.InpatientDayReport.InsertInpatientDayReport",ref strSQL) == -1) 
			{
				this.Err="没有找到Case.InpatientDayReport.InsertInpatientDayReport字段!";
				return -1;
			}
			try 
			{  
				string[] strParm = myGetParmInpatientDayReport( dayReport );     //取参数列表
				strSQL=string.Format(strSQL, strParm);            //替换SQL语句中的参数。
			}
			catch(Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.InpatientDayReport.InsertInpatientDayReport:" + ex.Message;
				this.WriteErr();
				return -1;
			}
			return this.ExecNoQuery(strSQL);
		}
		
		
		/// <summary>
		/// 更新住院日报汇总表中一条记录
		/// </summary>
		/// <param name="dayReport">住院日报实体</param>
		/// <returns>0没有更新 1成功 -1失败</returns>
		public int UpdateInpatientDayReport(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport) 
		{
			string strSQL="";
			if(this.Sql.GetCommonSql("Case.InpatientDayReport.UpdateInpatientDayReport",ref strSQL) == -1) 
			{
				this.Err="没有找到Case.InpatientDayReport.UpdateInpatientDayReport字段!";
				return -1;
			}
			try 
			{  
				string[] strParm = myGetParmInpatientDayReport( dayReport );     //取参数列表
				strSQL=string.Format(strSQL, strParm);            //替换SQL语句中的参数。
			}
			catch(Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.InpatientDayReport.UpdateInpatientDayReport:" + ex.Message;
				this.WriteErr();
				return -1;
			}
			return this.ExecNoQuery(strSQL);
		}
		
		
		/// <summary>
		/// 删除住院日报汇总表中一条记录
		/// </summary>
		/// <param name="dayReportID">库存记录类</param>
		/// <returns>0没有更新 1成功 -1失败</returns>
		public int DeleteInpatientDayReport(string dayReportID) 
		{
			string strSQL="";
			if(this.Sql.GetCommonSql("Case.InpatientDayReport.DeleteInpatientDayReport",ref strSQL) == -1) 
			{
				this.Err="没有找到Case.InpatientDayReport.DeleteInpatientDayReport字段!";
				return -1;
			}
			try 
			{  
				//如果是新增加的住院日报单，则直接返回
				strSQL=string.Format(strSQL, dayReportID);            //替换SQL语句中的参数。
			}
			catch(Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.InpatientDayReport.DeleteInpatientDayReport:" + ex.Message;
				this.WriteErr();
				return -1;
			}
			return this.ExecNoQuery(strSQL);
		}
		

		/// <summary>
		/// 先更新住院日报，如果没有找到数据则插入一条新数据
		/// </summary>
		/// <returns></returns>
		public int SetInpatientDayReport(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport) 
		{
			int parm;
			//先更新住院日报
			parm = this.UpdateInpatientDayReport(dayReport);
			if (parm == 0) 
			{
				//如果没有找到数据则插入一条新数据
				parm = this.InsertInpatientDayReport(dayReport);
			}
			return parm;
		}
		
		Neusoft.FrameWork.Management.ExtendParam managerExtendParam = new Neusoft.FrameWork.Management.ExtendParam();

		/// <summary>
		/// 日报动态更新，在每次发生床位变动的时候更新
		/// 先更新住院日报，如果没有找到数据则找昨天日报中的期末数和固定床位数，插入一条新数据
		/// </summary>
		/// <returns></returns>
		public int DynamicUpdate(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport) 
		{
			try 
			{
				int parm;
				//更新住院日报
				parm = this.UpdateInpatientDayReport(dayReport);
				//如果没有找到当天数据，说明是本日第一次更新，则取昨日日报的期末数作为今日期初数和固定床位数等数据插入数据库
				if(parm == 0) 
				{		
					//取昨日住院日报的期末数
					Neusoft.HISFC.Models.HealthRecord.InpatientDayReport lastReport = this.GetInpatientDayReport(dayReport.DateStat.AddDays(-1), dayReport.ID, dayReport.NurseStation.ID);
					if(lastReport == null) return -1;
					
					//如果没有找到昨天的数据则认为昨日期末数为0
					if (lastReport.ID == "")  lastReport.EndNum = GetDeptPatientNum(dayReport.ID, dayReport.NurseStation.ID);

					//今天期初数＝昨日期末数
					dayReport.BeginningNum = lastReport.EndNum;
					//今天期末数＝今日期末数 ＋ 昨日期末数（今日期初数）
					dayReport.EndNum = dayReport.EndNum + lastReport.EndNum;
				
					//取固定床位数
					Neusoft.HISFC.Models.Base.ExtendInfo deptExt = managerExtendParam.GetComExtInfo(Neusoft.HISFC.Models.Base.EnumExtendClass.DEPT,"CASE_BED_STAND",dayReport.ID);
					if(deptExt == null) return -1;
					dayReport.BedStand = Convert.ToInt32(deptExt.NumberProperty);

					//插入住院日报表中一条新记录
					parm = this.InsertDayReportDetail(dayReport);
				}

				
				if (parm == -1) return parm;
				 
				//调用日报明细插入函数
				return this.SetDayReportDetail(dayReport);
				//return parm;
			}
			catch(Exception ex) 
			{
				this.Err = ex.Message;
				return -1;
			}
		}


		/// <summary>
		/// 日报动态更新，在每次发生床位变动的时候更新
		/// 先更新住院日报，如果没有找到数据则找昨天日报中的期末数和固定床位数，插入一条新数据
		/// </summary>
		/// <param name="patientInfo">患者实体</param>
		/// <param name="type">床位变动类型</param>
		/// <returns></returns>
		public int DynamicUpdate(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, string type) 
		{
			
			DateTime sysDate = Convert.ToDateTime(this.GetSysDate() + " 00:00:00");

			//更新动态日报
			//取本日住院日报数据，作为更新的实体
			Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport = this.GetInpatientDayReport(sysDate, patientInfo.PVisit.PatientLocation.Dept.ID, patientInfo.PVisit.PatientLocation.NurseCell.ID);
			if(dayReport == null) return -1;

			
			//给日报实体赋值
			//动态日报数据需要保存类型,患者流水号,床号
			dayReport.User01 = type;
			dayReport.User02 = patientInfo.ID;
			dayReport.User03 = patientInfo.PVisit.PatientLocation.Bed.ID;
			
			//如果没有找到今日的日报记录，dayReport中没有科室数据
			//此处ID不能赋值，因为后面代码用此ID判断是否需要插入新记录。dayReport.ID = patientInfo.PVisit.PatientLocation.Dept.ID;  //日报科室编码
			dayReport.NurseStation.ID = patientInfo.PVisit.PatientLocation.NurseCell.ID; //日报病区编码
			dayReport.DateStat = sysDate; //日报日期
            switch (type.ToString())
            {
                case "K":
                    //接珍处理：常规入院加1，期末人数加1
                    //if (Type.ToString()=="K") 
                    {
                        dayReport.InNormal = dayReport.InNormal + 1;
                        dayReport.EndNum = dayReport.EndNum + 1;
                    }
                    break;

                case "RI":
                case "RO":
                    //转入处理RI：他科转入加1，期末人数加1
                    //转出处理RO：转往他科加1，期末人数减1
                    {
                        //取原科室、现科室信息
                        Neusoft.HISFC.BizLogic.Manager.Department department = new Neusoft.HISFC.BizLogic.Manager.Department();
                        department.SetTrans(this.Trans);

                        //取原科室信息
                        Neusoft.HISFC.Models.Base.Department oldDept = department.GetDeptmentById(patientInfo.PVisit.PatientLocation.Dept.ID);
                        if (oldDept == null)
                        {
                            this.Err = department.Err;
                            return -1;
                        }

                        //取新科室信息
                        Neusoft.HISFC.Models.Base.Department newDept = department.GetDeptmentById(patientInfo.PVisit.PatientLocation.Dept.User03);
                        if (newDept == null)
                        {
                            this.Err = department.Err;
                            return -1;
                        }

                        //转入处理：他科转入加1，期末人数加1
                        if (type.ToString() == "RI")
                        {

                            if (oldDept.SpecialFlag == "C" && newDept.SpecialFlag == "C")
                            {
                                //科室特殊属性为产科，中山特殊需求，内部转入
                                //内部转入数加1
                                dayReport.InTransferInner = dayReport.InTransferInner + 1;
                            }
                            else
                            {
                                //他科转入数加1
                                dayReport.InTransfer = dayReport.InTransfer + 1;
                            }
                            //期末人数加1
                            dayReport.EndNum = dayReport.EndNum + 1;

                            //[2011-5-23]by zhaozf 修改床位日报
                            dayReport.RelationDeptID = patientInfo.PVisit.PatientLocation.Dept.User03;
                            dayReport.RelationNurseCellID = patientInfo.PVisit.PatientLocation.NurseCell.User03;
                        }
                        else
                        {
                            //转出处理：转出加1，期末人数减1
                            if (oldDept.SpecialFlag == "C" && newDept.SpecialFlag == "C")
                            {
                                //科室特殊属性为产科，中山特殊需求，内部转出
                                //内部转出数加1
                                dayReport.OutTransferInner = dayReport.OutTransferInner + 1;
                            }
                            else
                            {
                                //转出他科数加1
                                dayReport.OutTransfer = dayReport.OutTransfer + 1;
                            }
                            dayReport.EndNum = dayReport.EndNum - 1;

                            //[2011-5-23]by zhaozf 修改床位日报
                            dayReport.RelationDeptID = newDept.ID;
                            dayReport.RelationNurseCellID = patientInfo.PVisit.PatientLocation.NurseCell.User03;
                        }
                    }
                    break;
                case "C":
                    //住院召回：住院召回加1，期末人数加1
                    {
                        if (patientInfo.PVisit.OutTime.Date >= sysDate.Date)
                        {
                            //dayReport.DateStat = Convert.ToDateTime(patientInfo.PVisit.OutTime.Date + " 00:00:00");			
                            dayReport.DateStat = patientInfo.PVisit.OutTime.Date;

                            //查找此患者是否有大于等于今日的出院登记记录，如果有则作废那条出院登记日报明细记录
                            #region {8997C648-0AE4-42f4-943A-4E34EC127B39}
                            int iMyReturn = this.CancelDayReportDetail(dayReport);
                            if (iMyReturn == 1)
                            #endregion
                            {
                                //当该患者出院日期等于今日，则将出院人数减1，明细已经作废。不再插入明细记录
                                if (patientInfo.PVisit.OutTime.Date == sysDate.Date)
                                {
                                    dayReport.OutNormal = dayReport.OutNormal - 1;
                                    //用NurseStation.User03 ＝ “1”来表示不需要处理日报明细记录
                                    dayReport.NurseStation.User03 = "N";
                                }
                                else
                                {
                                    //如果患者出院日期大于今天，则不处理日报汇总表，明细已经作废
                                    return 1;
                                }
                            }
                            else
                            {
                                //如果没有可以作废的出院记录，说明召回的是以前的数据，则召回人数加1
                                dayReport.InReturn = dayReport.InReturn + 1;
                            }
                        }

                        dayReport.EndNum = dayReport.EndNum + 1;
                        switch (patientInfo.PVisit.ZG.ID)
                        {
                            case "1":
                                dayReport.OutCure = dayReport.OutCure - 1;
                                break;
                            case "2":
                                dayReport.OutBetter = dayReport.OutBetter - 1;
                                break;
                            case "3":
                                dayReport.OutUnCure = dayReport.OutUnCure - 1;
                                break;
                            case "4":
                                dayReport.OutDeath = dayReport.OutDeath - 1;
                                break;
                            case "5":
                                dayReport.OutOther = dayReport.OutOther - 1;
                                break;                          
                        }
                    }
                    break;
                case "O":
                    {
                        //如果患者出院日期大于今天，日报统计日期等于患者出院日期。不处理日报汇总表，只插明细表
                        if (patientInfo.PVisit.OutTime >= sysDate.AddDays(1))
                        {
                            #region {BDB95DB2-E42B-4ee5-82F1-6DB5CDA072FA}
                            //dayReport.DateStat = Convert.ToDateTime(patientInfo.PVisit.OutTime + " 00:00:00");
                            dayReport.DateStat = patientInfo.PVisit.OutTime.Date;
                            #endregion
                            dayReport.ID = patientInfo.PVisit.PatientLocation.Dept.ID;  //日报科室编码
                            //调用日报明细插入函数
                            return this.SetDayReportDetail(dayReport);
                        }

                        //如果出院日期大于今日，则不更新日报表，只插入一条日报明细记录。每晚后台日报处理程序中，将那天的出院人数加1
                        //出院登记：常规出院加1，期末人数减1
                        dayReport.OutNormal = dayReport.OutNormal + 1;
                        dayReport.EndNum = dayReport.EndNum - 1; //此处有可能是-1

                        switch (patientInfo.PVisit.ZG.ID)
                        {
                            case "1":
                                dayReport.OutCure = dayReport.OutCure + 1;
                                break;
                            case "2":
                                dayReport.OutBetter = dayReport.OutBetter + 1;
                                break;
                            case "3":
                                dayReport.OutUnCure = dayReport.OutUnCure + 1;
                                break;
                            case "4":
                                dayReport.OutDeath = dayReport.OutDeath + 1;
                                break;
                            case "5":
                                dayReport.OutOther = dayReport.OutOther + 1;
                                break;
                        }

                    }
                    break;

                case "OF":
                    //无费退院：无费退院加1，期末人数减1
                    //if (Type.ToString()=="OF") 
                    {
                        if (patientInfo.PVisit.PatientLocation.Bed.ID == "")
                        {
                            return 1; //如果患者没有分配床位，则不写住院日志
                        }

                        //houwb 2012-3-8
                        //当天无费退院 入院人数-1
                        //隔日无费退院 出院人数+1
                        if (patientInfo.PVisit.InTime.Date == this.GetDateTimeFromSysDateTime().Date)
                        {
                            dayReport.InNormal = dayReport.InNormal - 1;
                            dayReport.EndNum = dayReport.EndNum - 1;
                        }
                        else
                        {
                            dayReport.OutWithdrawal = dayReport.OutWithdrawal + 1;
                            dayReport.EndNum = dayReport.EndNum - 1; //此处有可能是-1
                        }
                    }
                    break;

                //[2011-5-23]by zhaozf 修改床位日报
                case "CNO":
                    {
                        dayReport.RelationDeptID = patientInfo.PVisit.PatientLocation.NurseCell.User03;
                        dayReport.RelationNurseCellID = patientInfo.PVisit.PatientLocation.Dept.User03;
                    }
                    break;
                case "CN":
                    {
                        dayReport.RelationDeptID = patientInfo.PVisit.PatientLocation.NurseCell.User03;
                        dayReport.RelationNurseCellID = patientInfo.PVisit.PatientLocation.Dept.User03;
                    }
                    break;
            }


			//床位日报表处理－－更新或者插入
			//如果没有找到当天数据，说明是本日第一次更新，则给日报实体赋初始值，并插入一条记录
			if(dayReport.ID == "") 
			{					
				//如果每晚凌晨，后台存储过程自动生产新一天的日报数据，就不会出现这段代码

                //给科室编码赋值
                #region {275A3935-042C-4e62-8717-3B21ADB77D6E}
                dayReport.ID = patientInfo.PVisit.PatientLocation.Dept.ID;  //日报科室编码
                //dayReport.ID = patientInfo.PVisit.PatientLocation.NurseCell.ID;  //日报科室编码
                #endregion
                dayReport.NurseStation.ID = patientInfo.PVisit.PatientLocation.NurseCell.ID;

				//取固定床位数
                //Neusoft.HISFC.Models.Base.DepartmentExt deptExt = this.GetDepartmentExt("CASE_BED_STAND",dayReport.ID);
                //if(deptExt == null) return -1;
                //dayReport.BedStand = Convert.ToInt32(deptExt.NumberProperty);
                //------------------
                Neusoft.FrameWork.Management.ExtendParam deptExtThing = new Neusoft.FrameWork.Management.ExtendParam();
                deptExtThing.SetTrans(this.Trans);
                decimal decBedNumber = deptExtThing.GetComExtInfoNumber(Neusoft.HISFC.Models.Base.EnumExtendClass.DEPT, "CASE_BED_STAND", dayReport.ID);
                dayReport.BedStand = Convert.ToInt32(decBedNumber);
                //------------------

				//取昨日住院日报的期末数
				Neusoft.HISFC.Models.HealthRecord.InpatientDayReport lastReport = this.GetInpatientDayReport(dayReport.DateStat.AddDays(-1), dayReport.ID, dayReport.NurseStation.ID);
				if(lastReport == null) return -1;
					
				//如果没有找到昨天的数据则取此时最新的患者在院数作为初始的期末数
				//由于没有初始期初数，只能根据当前的患者数作为期末数，然后计算本日期初数。
				if (lastReport.ID == "")  
				{
					//取科室在院人数作为此时期末数
					dayReport.EndNum = GetDeptPatientNum(dayReport.ID, dayReport.NurseStation.ID);
					//计算今日期初数
					this.ComputeBeginingNum(ref dayReport);
				}
				else 
				{
					//今天期初数＝昨日期末数
					dayReport.BeginningNum = lastReport.EndNum;
					//计算今日期末数
					this.ComputeEndNum(ref dayReport);
				}
		

				//插入住院日报表中一条新记录
				if (this.InsertInpatientDayReport(dayReport) == -1) return -1;

			}
			else  
			{
				//更新住院日报
				if (this.UpdateInpatientDayReport(dayReport) != 1) return -1;				
			}
				 
			//如果NurseStation.User03 == "N"，表示不需要处理明细数据
			if (dayReport.NurseStation.User03 == "N") 
			{
				return 1;
			}

			//调用日报明细插入函数
			return this.SetDayReportDetail(dayReport);
			return 1;
		}


		/// <summary>
		/// 根据日报实体中的各分项数据计算期末人数
		/// </summary>
		/// <param name="dayReport"></param>
		/// <returns></returns>
		public void ComputeEndNum(ref Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport) 
		{
			if (dayReport == null) return;
			dayReport.EndNum = dayReport.BeginningNum  
				+ dayReport.InNormal  
				+ dayReport.InEmergency 
				+ dayReport.InReturn
				+ dayReport.InTransfer 
				- dayReport.OutNormal
				- dayReport.OutTransfer
				- dayReport.OutWithdrawal;
		}


		/// <summary>
		/// 根据日报实体中的各分项数据计算期初人数
		/// </summary>
		/// <param name="dayReport"></param>
		/// <returns></returns>
		public void ComputeBeginingNum(ref Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport) 
		{
			if (dayReport == null) return;
			dayReport.BeginningNum = dayReport.EndNum  
				- dayReport.InNormal  
				- dayReport.InEmergency 
				- dayReport.InReturn
				- dayReport.InTransfer 
				+ dayReport.OutNormal
				+ dayReport.OutTransfer
				+ dayReport.OutWithdrawal;
		}


		/// <summary>
		/// 清空日报中的各项数值(除了期初数,期末数等于期初数)
		/// </summary>
		/// <param name="dayReport"></param>
		public void Clear(ref Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport)  
		{
			if (dayReport == null) return;
			
			dayReport.InNormal  = 0;
			dayReport.InEmergency = 0;
			dayReport.InReturn= 0;
			dayReport.InTransfer = 0;
			dayReport.OutNormal= 0;
			dayReport.OutTransfer= 0;
			dayReport.OutWithdrawal= 0;
			dayReport.EndNum = dayReport.BeginningNum;
		}


		/// <summary>
		/// 取住院日报信息列表，可能是一条或者多条库存记录
		/// 私有方法，在其他方法中调用
		/// writed by cuipeng
		/// 2005-1
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <returns>住院日报信息对象数组</returns>
		private ArrayList myGetInpatientDayReport(string SQLString) 
		{
			ArrayList al=new ArrayList();                
			Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport; //住院日报信息实体
			this.ProgressBarText="正在检索住院日报单信息...";
			this.ProgressBarValue=0;
			
			//执行查询语句
			if (this.ExecQuery(SQLString)==-1) 
			{
				this.Err="获得库存信息时，执行SQL语句出错！"+this.Err;
				this.ErrCode="-1";
				return null;
			}
			try 
			{
				while (this.Reader.Read()) 
				{
					//取查询结果中的记录
					dayReport = new Neusoft.HISFC.Models.HealthRecord.InpatientDayReport();
					dayReport.DateStat =    NConvert.ToDateTime(this.Reader[0].ToString()); //0 统计日期
					dayReport.ID =          this.Reader[1].ToString();                      //1 住院日报单号
					dayReport.BedStand =    NConvert.ToInt32(this.Reader[2].ToString());    //2 编制内病床数
					dayReport.BedAdd =      NConvert.ToInt32(this.Reader[3].ToString());    //3 加床数 
					dayReport.BedFree =     NConvert.ToInt32(this.Reader[4].ToString());    //4 空床数
					dayReport.BeginningNum= NConvert.ToInt32(this.Reader[5].ToString());    //5 期初病人数
					dayReport.InNormal =    NConvert.ToInt32(this.Reader[6].ToString());    //6 常规入院数
					dayReport.InEmergency = NConvert.ToInt32(this.Reader[7].ToString());    //7 急诊入院数
					dayReport.InTransfer =  NConvert.ToInt32(this.Reader[8].ToString());    //8 其他科转入数
					dayReport.InReturn =    NConvert.ToInt32(this.Reader[9].ToString());    //9  招回入院人数
					dayReport.OutNormal =   NConvert.ToInt32(this.Reader[10].ToString());   //10 常规出院数
					dayReport.OutTransfer = NConvert.ToInt32(this.Reader[11].ToString());   //11 转出其他科数
					dayReport.OutWithdrawal=NConvert.ToInt32(this.Reader[12].ToString());   //12 退院人数
					dayReport.EndNum =      NConvert.ToInt32(this.Reader[13].ToString());   //13 期末病人数
					dayReport.DeadIn24 =    NConvert.ToInt32(this.Reader[14].ToString());   //14 24小时内死亡数
					dayReport.DeadOut24 =   NConvert.ToInt32(this.Reader[15].ToString());   //15 24小时外死亡数
					dayReport.BedRate =     NConvert.ToDecimal(this.Reader[16].ToString()); //16 床位使用率
					dayReport.Other1Num =   NConvert.ToInt32(this.Reader[17].ToString());   //17 其他1数量
					dayReport.Other2Num =   NConvert.ToInt32(this.Reader[18].ToString());   //18 其他2数量
					dayReport.OperInfo.ID =    this.Reader[19].ToString();                     //19 操作员编码
					dayReport.OperInfo.OperTime=     NConvert.ToDateTime(this.Reader[20].ToString());//20 操作时间
					dayReport.Memo =        this.Reader[21].ToString();                     //21 备注
					dayReport.NurseStation.ID = this.Reader[22].ToString();                 //22 护士站编码
					dayReport.Name =        this.Reader[23].ToString();                     //23 科室名称
					dayReport.InTransferInner = NConvert.ToInt32(this.Reader[24].ToString());//24 内部转入数
					dayReport.OutTransferInner = NConvert.ToInt32(this.Reader[25].ToString());//25 内部转出数

                    if (this.Reader.FieldCount >= 27)
                    {
                        dayReport.OutCure = NConvert.ToInt32(this.Reader[26].ToString());// 出院治愈
                        dayReport.OutUnCure = NConvert.ToInt32(this.Reader[27].ToString());//出院未愈
                        dayReport.OutBetter = NConvert.ToInt32(this.Reader[28].ToString());//出院好转
                        dayReport.OutDeath = NConvert.ToInt32(this.Reader[29].ToString());//出院死亡
                        dayReport.OutOther = NConvert.ToInt32(this.Reader[30].ToString());//出院其他
                    }
				
                    this.ProgressBarValue++;
					al.Add(dayReport);
				}
			}//抛出错误
			catch(Exception ex) 
			{
				this.Err="获得住院日报信息时出错！"+ex.Message;
				this.ErrCode="-1";
				return null;
			}
			this.Reader.Close();

			this.ProgressBarValue=-1;
			return al;
		}


		/// <summary>
		/// 获得update或者insert库存表的传入参数数组
		/// </summary>
		/// <param name="dayReport">库存类</param>
		/// <returns>字符串数组</returns>
		private string[] myGetParmInpatientDayReport(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport) 
		{

			string[] strParm={   
								 dayReport.DateStat.ToString(),      //0 统计日期
								 dayReport.ID,                       //1 科室编码
								 dayReport.NurseStation.ID,          //2 护士站编码
								 dayReport.BedStand.ToString(),      //3 编制内病床数
								 dayReport.BedAdd.ToString(),        //4 加床数 
								 dayReport.BedFree.ToString(),       //5 空床数
								 dayReport.BeginningNum.ToString(),  //6 期初病人数
								 dayReport.InNormal.ToString(),      //7 常规入院数
								 dayReport.InEmergency.ToString(),   //8 急诊入院数
								 dayReport.InTransfer.ToString(),    //9 其他科转入数
								 dayReport.InReturn.ToString(),      //10 招回入院人数
								 dayReport.OutNormal.ToString(),     //11 常规出院数
								 dayReport.OutTransfer.ToString(),   //12 转出其他科数
								 dayReport.OutWithdrawal.ToString(), //13 退院人数
								 dayReport.EndNum.ToString(),        //14 期末病人数
								 dayReport.DeadIn24.ToString(),      //15 24小时内死亡数
								 dayReport.DeadOut24.ToString(),     //16 24小时外死亡数
								 dayReport.BedRate.ToString(),       //17 床位使用率
								 dayReport.Other1Num.ToString(),     //18 其他1数量
								 dayReport.Other2Num.ToString(),     //19 其他2数量
								 dayReport.Memo.ToString(),          //20 备注
								 this.Operator.ID,                    //21 操作员编码
								 dayReport.InTransferInner.ToString(),//22 内部转入数 （中山需求）
								 dayReport.OutTransferInner.ToString(),//23 内部转出数 （中山需求）
                                 dayReport.OutCure.ToString(),//出院治愈
                                 dayReport.OutUnCure.ToString(),//出院未愈
                                 dayReport.OutBetter.ToString(),//出院好转
                                 dayReport.OutDeath.ToString(),//出院死亡
                                 dayReport.OutOther.ToString()//出院其他
                             
							 };								 
			return strParm;
		}

		
//		/// <summary>
//		/// 取某一科室，特定编码的扩展属性
//		/// 此方法跟Neusoft.HisFC.Management.Manager.DepartmentExt中的GetDepartmentExt方法相同
//		/// </summary>
//		/// <param name="PropertyCode">属性编码</param>
//		/// <param name="DeptID">科室编码</param>
//		/// <returns>科室属性</returns>
		private Neusoft.HISFC.Models.Base.DepartmentExt GetDepartmentExt(string PropertyCode,string DeptID) 
		{
			string strSQL = "";
			string strWhere = "";
			//取SELECT语句
			if (this.Sql.GetCommonSql("Manager.DepartmentExt.GetDepartmentExtList",ref strSQL) == -1) 
			{
				this.Err="没有找到Manager.DepartmentExt.GetDepartmentExtList字段!";
				return null;
			}
			if (this.Sql.GetCommonSql("Manager.DepartmentExt.And.DeptID",ref strWhere) == -1) 
			{
				this.Err="没有找到Manager.DepartmentExt.And.DeptID字段!";
				return null;
			}
			//格式化SQL语句
			try 
			{
				strSQL += " " +strWhere;
				strSQL = string.Format(strSQL, PropertyCode,DeptID);
			}
			catch (Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Manager.DepartmentExt.And.DeptID:" + ex.Message;
				return null;
			}

			//取科室属性数据
			Neusoft.HISFC.Models.Base.DepartmentExt departmentExt = new Neusoft.HISFC.Models.Base.DepartmentExt();
			
			//执行查询语句
			if (this.ExecQuery(strSQL)==-1) 
			{
				this.Err="获得科室属性信息时，执行SQL语句出错！"+this.Err;
				this.ErrCode="-1";
				return null;
			}
			try 
			{
				while (this.Reader.Read()) 
				{
					//取查询结果中的记录
					departmentExt = new Neusoft.HISFC.Models.Base.DepartmentExt();
					departmentExt.Dept.ID   = this.Reader[0].ToString();          //0 科室编码
					departmentExt.Dept.Name = this.Reader[1].ToString();          //1 科室名称
					departmentExt.PropertyCode   = this.Reader[2].ToString();     //2 属性编码
					departmentExt.PropertyName   = this.Reader[3].ToString();     //3 属性名称
					departmentExt.StringProperty = this.Reader[4].ToString();     //4 字符属性 
					departmentExt.NumberProperty = NConvert.ToDecimal(this.Reader[5].ToString()); //5 数值属性
					departmentExt.DateProperty   = NConvert.ToDateTime(this.Reader[6].ToString());//6 日期属性
					departmentExt.Memo      = this.Reader[7].ToString();          //7 备注
					departmentExt.OperEnvironment.ID  = this.Reader[8].ToString();          //8 操作日期
					departmentExt.OperEnvironment.OperTime  = NConvert.ToDateTime(this.Reader[9].ToString());     //9 操作时间
					departmentExt.User01    = this.Reader[10].ToString();         //科室类型
				}
				this.Reader.Close();
			}//抛出错误
			catch(Exception ex) 
			{
				this.Err="获得科室属性信息时出错！"+ex.Message;
				this.ErrCode="-1";
				return null;
			}

			return departmentExt;
		}


		/// <summary>
		/// 获取某个科室中的住院的人数
		/// </summary>
		/// <param name="DeptId"></param>
		/// <returns></returns>
		private int GetDeptPatientNum(string DeptId,string NurseID)
		{
			try 
			{ 
				string strSQL="";
				if(this.Sql.GetCommonSql("Case.InpatientDayReport.GetDeptPatientNum",ref strSQL) == -1) 
				{
					this.Err="没有找到Case.InpatientDayReport.GetDeptPatientNum字段!";
					return -1;
				}
 
				//如果是新增加的住院日报单，则直接返回
				strSQL=string.Format(strSQL, DeptId, NurseID);            //替换SQL语句中的参数。
			
				return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(strSQL));
			}
			catch(Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.InpatientDayReport.GetDeptPatientNum:" + ex.Message;
				this.WriteErr();
				return -1;
			}
		}

		#endregion 

		#region 日报明细处理
		/// <summary>
		/// 取全院某一天的住院日报数据
		/// </summary>
		/// <param name="dateBegin">起始日期</param>
		/// <param name="dateEnd">终止日期</param>
		/// <returns>住院日报数组，出错返回null</returns>
		public ArrayList GetDayReportDetailList(DateTime dateBegin, DateTime dateEnd) 
		{
			return this.GetDayReportDetailList(dateBegin, dateEnd, "ALL", "ALL");
		}


		/// <summary>
		/// 取全院某一天的住院日报数据
		/// </summary>
		/// <param name="dateBegin">起始日期</param>
		/// <param name="dateEnd">终止日期</param>
		/// <returns>住院日报数组，出错返回null</returns>
		public ArrayList GetDayReportDetailList(DateTime dateBegin, DateTime dateEnd, string deptCode, string nurseCellCode) 
		{
			string strSQL = "";
			//string strWhere = "";
			//取SELECT语句
			if (this.Sql.GetCommonSql("Case.DayReport.GetDayReportDetailList",ref strSQL) == -1) 
			{
				this.Err="没有找到Case.DayReport.GetDayReportDetailList字段!";
				return null;
			}

			//格式化SQL语句
			try 
			{
				strSQL = string.Format(strSQL, dateBegin.ToString(), dateEnd.ToString(), deptCode);
			}
			catch (Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.DayReport.GetDayReportDetailList:" + ex.Message;
				return null;
			}

			//取住院日报数据
			return this.myGetDayReportDetail(strSQL);
		}


		/// <summary>
		/// 向日报明细表中插入一条记录
		/// </summary>
		/// <param name="reportDetail">住院日报实体</param>
		/// <returns>0没有更新 1成功 -1失败</returns>
		public int InsertDayReportDetail(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport reportDetail) 
		{
			string strSQL="";
			if(this.Sql.GetCommonSql("Case.DayReport.InsertDayReportDetail",ref strSQL) == -1) 
			{
				this.Err="没有找到Case.DayReport.InsertDayReportDetail字段!";
				return -1;
			}
			try 
			{  
				string[] strParm = myGetParmDayReportDetail( reportDetail );     //取参数列表
				strSQL=string.Format(strSQL, strParm);            //替换SQL语句中的参数。
			}
			catch(Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.DayReport.InsertDayReportDetail:" + ex.Message;
				this.WriteErr();
				return -1;
			}
			return this.ExecNoQuery(strSQL);
		}
		
		
		/// <summary>
		/// 更新日报明细表中一条记录
		/// </summary>
		/// <param name="reportDetail">住院日报实体</param>
		/// <returns>0没有更新 1成功 -1失败</returns>
		public int UpdateDayReportDetail(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport reportDetail) 
		{
			string strSQL="";
			if(this.Sql.GetCommonSql("Case.DayReport.UpdateDayReportDetail",ref strSQL) == -1) 
			{
				this.Err="没有找到Case.DayReport.UpdateDayReportDetail字段!";
				return -1;
			}
			try 
			{  
				string[] strParm = myGetParmDayReportDetail( reportDetail );     //取参数列表
				strSQL=string.Format(strSQL, strParm);            //替换SQL语句中的参数。
			}
			catch(Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.DayReport.UpdateDayReportDetail:" + ex.Message;
				this.WriteErr();
				return -1;
			}
			return this.ExecNoQuery(strSQL);
		}
		
				
		/// <summary>
		/// 作废日报明细表中一条记录
		/// </summary>
		/// <param name="reportDetail">住院日报实体</param>
		/// <returns>0没有更新 1成功 -1失败</returns>
		public int CancelDayReportDetail(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport reportDetail) 
		{
			string strSQL="";
			if(this.Sql.GetCommonSql("Case.DayReport.CancelDayReportDetail",ref strSQL) == -1) 
			{
				this.Err="没有找到Case.DayReport.CancelDayReportDetail字段!";
				return -1;
			}
			try 
			{  
				//参数说明：0患者住院流水号，1日报统计类型（目前只有出院可以作废），2统计日期
				strSQL=string.Format(strSQL, reportDetail.User02, "O", reportDetail.DateStat.ToString());            //替换SQL语句中的参数。
			}
			catch(Exception ex) 
			{
				this.Err = "格式化SQL语句时出错Case.DayReport.CancelDayReportDetail:" + ex.Message;
				this.WriteErr();
				return -1;
			}
			return this.ExecNoQuery(strSQL);
		}
		

		/// <summary>
		/// 先更新住院日报，如果没有找到数据则插入一条新数据
		/// </summary>
		/// <returns></returns>
		public int SetDayReportDetail(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport dayReport) 
		{
			//根据dayReport中的user01来区分是何种类型
			//1 K －接珍，
			//2 RB－转入，
			//3 RD－转出，
			//4 O －出院登记，
			//5 C －召回，
			//6 OF－无费退院
			//			switch (dayReport.User01) {
			//				case "K":	//接诊
			//					dayReport.User01 = "I_NORMAL";
			//					break;
			//				case "RI":	//转入
			//					dayReport.User01 = "I_TRANSFER";
			//					break;
			//				case "C":	//召回
			//					dayReport.User01 = "I_RETURN";
			//					break;
			//				case "RO":	//转出
			//					dayReport.User01 = "O_TRANSFER";
			//					break;
			//				case "O":	//出院登记
			//					dayReport.User01 = "O_NORMAL";
			//					break;
			//				case "OF":	//无费退院
			//					dayReport.User01 = "O_WITHDRAWAL";
			//					break;
			//			}
			//			int parm;
			//			//先更新住院日报
			//			parm = this.InsertInpatientDayReportDetail(dayReport);
			//			return parm;
			return this.InsertDayReportDetail(dayReport);
		}


		/// <summary>
		/// 取日报明细信息列表，可能是一条或者多条库存记录
		/// 私有方法，在其他方法中调用
		/// writed by cuipeng
		/// 2006-3
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <returns>NeuObject对象数组</returns>
		private ArrayList myGetDayReportDetail(string SQLString) 
		{
			ArrayList al=new ArrayList();                
			Neusoft.HISFC.Models.HealthRecord.InpatientDayReport reportDetail; //住院日报信息实体
			this.ProgressBarText="正在检索住院日报单信息...";
			this.ProgressBarValue=0;
			
			//执行查询语句
			if (this.ExecQuery(SQLString)==-1) 
			{
				this.Err="获得库存信息时，执行SQL语句出错！"+this.Err;
				this.ErrCode="-1";
				return null;
			}
			try 
			{
				while (this.Reader.Read()) 
				{
					//取查询结果中的记录
					reportDetail = new Neusoft.HISFC.Models.HealthRecord.InpatientDayReport();
					reportDetail.DateStat = NConvert.ToDateTime(this.Reader[0].ToString()); //0 统计日期
					reportDetail.ID       = this.Reader[1].ToString();                      //1 科室编码
					reportDetail.NurseStation.ID = this.Reader[2].ToString();               //2 护理站编码
					reportDetail.User01   = this.Reader[3].ToString();                      //3 统计类型
					reportDetail.User02   = this.Reader[4].ToString();                      //4 住院流水号
					reportDetail.User03   = this.Reader[5].ToString();                      //5 床号
					reportDetail.Memo     = this.Reader[6].ToString();                      //6 备注
					reportDetail.OperInfo.ID = this.Reader[7].ToString();                      //7 操作员
					reportDetail.OperInfo.OperTime = NConvert.ToDateTime(this.Reader[8].ToString()); //8 操作时间
					
					this.ProgressBarValue++;
					al.Add(reportDetail);
				}
			}//抛出错误
			catch(Exception ex) 
			{
				this.Err="获得住院日报信息时出错！"+ex.Message;
				this.ErrCode="-1";
				return null;
			}
			this.Reader.Close();

			this.ProgressBarValue=-1;
			return al;
		}


		/// <summary>
		/// 获得update或者insert库存表的传入参数数组
		/// </summary>
		/// <param name="reportDetail">库存类</param>
		/// <returns>字符串数组</returns>
		private string[] myGetParmDayReportDetail(Neusoft.HISFC.Models.HealthRecord.InpatientDayReport reportDetail) 
		{
            string strSQL = "";
            string patientNo = reportDetail.User02;
            string zg = "";
            if (this.Sql.GetCommonSql("Case.DayReport.GetDayReportDetailZg", ref strSQL) == -1)
            {
                this.Err = "没有找到Case.DayReport.GetDayReportDetailZg字段!";
                this.ErrCode = "-1";
                return null;
            }
            try
            {
                strSQL = string.Format(strSQL, patientNo);            //替换SQL语句中的参数。
            }
            catch (Exception ex)
            {
                this.Err = "格式化SQL语句时出错Case.DayReport.GetDayReportDetailZg:" + ex.Message;
                this.WriteErr();
                this.ErrCode = "-1";
                return null;
            }
           
     
         
          
            if (this.ExecQuery(strSQL) == -1)
            {
                this.Err = "获得转归编码时出错，执行SQL语句出错！" + this.Err;
                this.ErrCode = "-1";
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    //取查询结果中的记录
                    zg = this.Reader[0].ToString();          //0 转归编码                 
                }
                this.Reader.Close();
            }//抛出错误
            catch (Exception ex)
            {
                this.Err = "获得转归编码时出错！" + ex.Message;
                this.ErrCode = "-1";
                return null;
            }
            string[] strParm={   
								 reportDetail.DateStat.ToString(),      //0 统计日期
								 reportDetail.ID,                       //1 科室编码
								 reportDetail.NurseStation.ID,          //2 护士站编码
								 reportDetail.User02,					//3 住院流水号
								 reportDetail.User03,					//4 床号
								 reportDetail.User01,					//5 统计类型
								 this.Operator.ID,						//6 操作员编码
								 reportDetail.Memo,					//7 备注
                                 zg,                                 //8 转归代码\
                                 //[2011-5-23]by zhaozf 修改床位日报
                                 reportDetail.RelationDeptID,
                                 reportDetail.RelationNurseCellID
                                
							 };								 
			return strParm;
		}

		#endregion 


        #region 新日报处理 2012-8-5 以后新日报以此为准
        /// <summary>
        /// 上床下床标识，1-上床，0-下床
        /// </summary>
        private string strType = string.Empty;
        /// <summary>
        /// 操作来源编码
        /// </summary>
        private string strSource = string.Empty;
        /// <summary>
        /// 床位表处理
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="type">场景:入院登记（接诊）、护士站接诊、无费退院、出院登记、出院召回 </param>
        /// <returns></returns>
        public int ArriveBed(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.HISFC.Models.Base.EnumShiftType shiftType)
        {
            if (patientInfo == null)
            {
                return -1;
            }
            this.strSource = shiftType.ToString();
            switch (shiftType)
            {
                case  Neusoft.HISFC.Models.Base.EnumShiftType.B://入院登记
                    this.strType = "0";
                    break;
                case Neusoft.HISFC.Models.Base.EnumShiftType.K://接诊
                    this.strType = "0";
                    break;
                case Neusoft.HISFC.Models.Base.EnumShiftType.OF://无费退院
                    this.strType = "1";
                    break;
                case Neusoft.HISFC.Models.Base.EnumShiftType.O://出院
                    this.strType = "1";
                    break;
                case Neusoft.HISFC.Models.Base.EnumShiftType.C://出院召回
                    this.strType = "0";
                    break;
                default :
                    break;
            }
            //插入包/解床记录
            Neusoft.HISFC.Models.RADT.InPatientBedTransReord objBedInfo = this.TransBedInfo(patientInfo);
            if (objBedInfo == null)
            {
                return -1;
            }
            return this.InSertInPatientBedTransReord(objBedInfo);
        }
      
        /// <summary>
        /// 床位表处理:转科
        /// </summary>
        /// <param name="patientInfoOld">转科前病人信息</param>
        /// <param name="patientInfoNew">转科前病人信息</param>
        /// <returns></returns>
        public int TransBed(Neusoft.HISFC.Models.RADT.PatientInfo patientInfoOld,Neusoft.HISFC.Models.RADT.PatientInfo patientInfoNew)
        {
            if (patientInfoOld == null || patientInfoNew == null)
            {
                return -1;
            }

            //插入原科室的下床记录
            this.strSource = Neusoft.HISFC.Models.Base.EnumShiftType.RO.ToString();
            this.strType = "1";
            Neusoft.HISFC.Models.RADT.InPatientBedTransReord objBedInfo = this.TransBedInfoForChangeDept(patientInfoOld, patientInfoNew);
            if (objBedInfo == null)
            {
                return -1;
            }
            if (this.InSertInPatientBedTransReord(objBedInfo) == -1)
            {
                return -1;
            }
            //插入新科室的上床记录
            this.strType = "0";
            this.strSource = Neusoft.HISFC.Models.Base.EnumShiftType.RI.ToString();
            objBedInfo = this.TransBedInfoForChangeDept(patientInfoOld, patientInfoNew);
            if (objBedInfo == null)
            {
                return -1;
            }
            return this.InSertInPatientBedTransReord(objBedInfo);
        }

        /// <summary>
        /// 床位表处理:包床
        /// </summary>
        /// <param name="patientInfo">包床病人信息</param>
        /// <param name="bedInfo">被包的床位信息 </param>
        /// <param name="isAdd">是否包床，true-包床，false-解除包床 </param>
        /// <returns></returns>
        public int AddExtentBed(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.HISFC.Models.Base.Bed bedInfo,bool isAdd)
        {
            if (patientInfo == null || bedInfo == null)
            {
                return -1;
            }
            if (isAdd)
            {
                this.strType = "0";
                this.strSource = Neusoft.HISFC.Models.Base.EnumShiftType.ABD.ToString();
            }
            else
            {
                this.strType = "1";
                this.strSource = Neusoft.HISFC.Models.Base.EnumShiftType.RBD.ToString();
            }
            Neusoft.HISFC.Models.RADT.InPatientBedTransReord objBedInfo = this.TransBedInfoAddBed(patientInfo, bedInfo);
            if (objBedInfo == null)
            {
                return -1;
            }
            return this.InSertInPatientBedTransReord(objBedInfo);
        }

        /// <summary>
        /// 转换各种信息城标准的床位记录表插入格式
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <returns></returns>
        private Neusoft.HISFC.Models.RADT.InPatientBedTransReord TransBedInfo(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo)
        {
            Neusoft.HISFC.Models.RADT.InPatientBedTransReord objBedInfo = null;
            if (patientInfo == null)
            {
                return objBedInfo;
            }
            objBedInfo = new Neusoft.HISFC.Models.RADT.InPatientBedTransReord();
            objBedInfo.INPATIENT_NO = patientInfo.ID;
            objBedInfo.PATIENT_NO = patientInfo.PID.PatientNO;
            objBedInfo.OLD_DEPT_ID =string.Empty;
            objBedInfo.OLD_DEPT_NAME = string.Empty;
            objBedInfo.TARGET_DEPT_ID = patientInfo.PVisit.PatientLocation.Dept.ID;
            objBedInfo.TARGET_DEPT_NAME = patientInfo.PVisit.PatientLocation.Dept.Name;
            objBedInfo.BED_NO = patientInfo.PVisit.PatientLocation.Bed.ID;
            objBedInfo.TRANS_TYPE = this.strType;
            objBedInfo.TRANS_CODE = this.strSource;
            objBedInfo.MEDICAL_GROUP_CODE = string.Empty;
            objBedInfo.CARE_GROUP_CODE = string.Empty;
            objBedInfo.IN_DOCT_CODE = patientInfo.PVisit.AdmittingDoctor.ID;
            objBedInfo.NURSE_STATION_CODE = patientInfo.PVisit.PatientLocation.NurseCell.ID;
            objBedInfo.ZG = patientInfo.PVisit.ZG.ID;
            objBedInfo.SEQUENCE_NO = string.Empty ;
            objBedInfo.OPER_CODE = "009999";
            //objBedInfo.OPER_DATE = this.GetDateTimeFromSysDateTime();
            objBedInfo.DEPT_CODE = patientInfo.PVisit.PatientLocation.Dept.ID;
            objBedInfo.OLD_NURSE_ID = string.Empty;
            objBedInfo.OLD_NURSE_NAME = string.Empty;
            objBedInfo.TARGET_NURSE_ID = patientInfo.PVisit.PatientLocation.NurseCell.ID;
            objBedInfo.TARGET_NURSE_NAME = patientInfo.PVisit.PatientLocation.NurseCell.Name;
            return objBedInfo;

        }
        /// <summary>
        /// 转换各种信息城标准的床位记录表插入格式(转科操作专用)
        /// </summary>
        /// <param name="patientInfoOld"></param>
        /// <param name="patientInfoNew"></param>
        /// <returns></returns>
        private Neusoft.HISFC.Models.RADT.InPatientBedTransReord TransBedInfoForChangeDept(Neusoft.HISFC.Models.RADT.PatientInfo patientInfoOld,Neusoft.HISFC.Models.RADT.PatientInfo patientInfoNew)
        {
            Neusoft.HISFC.Models.RADT.InPatientBedTransReord objBedInfo = null;
            if (patientInfoOld == null || patientInfoNew==null)
            {
                return objBedInfo;
            }
            objBedInfo = new Neusoft.HISFC.Models.RADT.InPatientBedTransReord();
            objBedInfo.INPATIENT_NO = patientInfoOld.ID;
            objBedInfo.PATIENT_NO = patientInfoOld.PID.PatientNO;
            //科室
            objBedInfo.OLD_DEPT_ID = patientInfoOld.PVisit.PatientLocation.Dept.ID;
            objBedInfo.OLD_DEPT_NAME = patientInfoOld.PVisit.PatientLocation.Dept.Name;
            objBedInfo.TARGET_DEPT_ID = patientInfoNew.PVisit.PatientLocation.Dept.ID;
            objBedInfo.TARGET_DEPT_NAME = patientInfoNew.PVisit.PatientLocation.Dept.Name;
            //护士站
            objBedInfo.OLD_NURSE_ID = patientInfoOld.PVisit.PatientLocation.NurseCell.ID;
            objBedInfo.OLD_NURSE_NAME = patientInfoOld.PVisit.PatientLocation.NurseCell.Name;
            objBedInfo.TARGET_NURSE_ID = patientInfoNew.PVisit.PatientLocation.NurseCell.ID;
            objBedInfo.TARGET_NURSE_NAME = patientInfoNew.PVisit.PatientLocation.NurseCell.Name;
            if (this.strType == "1")
            {
                objBedInfo.BED_NO = patientInfoOld.PVisit.PatientLocation.Bed.ID;
                objBedInfo.DEPT_CODE = patientInfoOld.PVisit.PatientLocation.Dept.ID;
                objBedInfo.IN_DOCT_CODE = patientInfoOld.PVisit.AdmittingDoctor.ID;
                objBedInfo.NURSE_STATION_CODE = patientInfoOld.PVisit.PatientLocation.NurseCell.ID;
            }
            else
            {

                objBedInfo.BED_NO = patientInfoNew.PVisit.PatientLocation.Bed.ID;
                objBedInfo.DEPT_CODE = patientInfoNew.PVisit.PatientLocation.Dept.ID;
                objBedInfo.IN_DOCT_CODE = patientInfoNew.PVisit.AdmittingDoctor.ID;
                objBedInfo.NURSE_STATION_CODE = patientInfoNew.PVisit.PatientLocation.NurseCell.ID;
            }
            objBedInfo.TRANS_TYPE = this.strType;
            objBedInfo.TRANS_CODE = this.strSource;
            objBedInfo.MEDICAL_GROUP_CODE = string.Empty;
            objBedInfo.CARE_GROUP_CODE = string.Empty;

            objBedInfo.ZG = string.Empty;
            objBedInfo.SEQUENCE_NO = string.Empty;
            objBedInfo.OPER_CODE = "009999";
            //objBedInfo.OPER_DATE = this.GetDateTimeFromSysDateTime();

            return objBedInfo;

        }


        /// <summary>
        /// 转换各种信息城标准的床位记录表插入格式
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="bedInfo"></param>
        /// <returns></returns>
        private Neusoft.HISFC.Models.RADT.InPatientBedTransReord TransBedInfoAddBed(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.HISFC.Models.Base.Bed bedInfo)
        {
            Neusoft.HISFC.Models.RADT.InPatientBedTransReord objBedInfo = null;
            if (patientInfo == null)
            {
                return objBedInfo;
            }
            objBedInfo = new Neusoft.HISFC.Models.RADT.InPatientBedTransReord();
            objBedInfo.INPATIENT_NO = patientInfo.ID;
            objBedInfo.PATIENT_NO = patientInfo.PID.PatientNO;
            objBedInfo.OLD_DEPT_ID = string.Empty;
            objBedInfo.OLD_DEPT_NAME = string.Empty;
            objBedInfo.TARGET_DEPT_ID = patientInfo.PVisit.PatientLocation.Dept.ID;
            objBedInfo.TARGET_DEPT_NAME = patientInfo.PVisit.PatientLocation.Dept.Name;
            if (this.strType == "1")
            {
                objBedInfo.BED_NO = patientInfo.PVisit.PatientLocation.Bed.ID;
            }
            else
            {
                objBedInfo.BED_NO = bedInfo.ID;
            }
            objBedInfo.TRANS_TYPE = this.strType;
            objBedInfo.TRANS_CODE = this.strSource;
            objBedInfo.MEDICAL_GROUP_CODE =string.Empty;
            objBedInfo.CARE_GROUP_CODE = string.Empty;
            objBedInfo.IN_DOCT_CODE = patientInfo.PVisit.AdmittingDoctor.ID;
            objBedInfo.NURSE_STATION_CODE = patientInfo.PVisit.PatientLocation.NurseCell.ID;
            objBedInfo.ZG = patientInfo.PVisit.ZG.ID;
            objBedInfo.SEQUENCE_NO = string.Empty;
            objBedInfo.OPER_CODE = "009999";
            //objBedInfo.OPER_DATE = this.GetDateTimeFromSysDateTime();
            objBedInfo.DEPT_CODE = patientInfo.PVisit.PatientLocation.Dept.ID;

            objBedInfo.OLD_NURSE_ID = string.Empty;
            objBedInfo.OLD_NURSE_NAME = string.Empty;
            objBedInfo.TARGET_NURSE_ID = patientInfo.PVisit.PatientLocation.NurseCell.ID;
            objBedInfo.TARGET_NURSE_NAME = patientInfo.PVisit.PatientLocation.NurseCell.Name;
            return objBedInfo;

        }

      
        /// <summary>
        /// 插入床位变更记录
        /// </summary>
        /// <param name="p_objBedInfo">床位记录信息</param>
        /// <returns></returns>
        private int InSertInPatientBedTransReord(Neusoft.HISFC.Models.RADT.InPatientBedTransReord p_objBedInfo)
        {
            if (p_objBedInfo == null)
            {
                return -1;
            }
            string strSql = string.Empty;

            if (Sql.GetCommonSql("RADT.InPatient.InSertInPatientBedTransReord", ref strSql) == 0)
            {
                #region SQL
                /*insert into met_cas_bedinfodailyreport
                  (inpatient_no,
                   patient_no,
                   old_dept_id,
                   old_dept_name,
                   target_dept_id,
                   target_dept_name,
                   bed_no,
                   trans_type,
                   trans_code,
                   medical_group_code,
                   care_group_code,
                   in_doct_code,
                   nurse_station_code,
                   zg,
                   sequence_no,
                   oper_code,
                   oper_date,
                   OLD_NURSTATION_CODE,
                   OLD_NURSTATION_NAME,
                   TARGET_NURSTATION_CODE,
                   TARGET_NURSTATION_NAME)*/
                #endregion
                try
                {
                     strSql = string.Format(strSql,
                                            p_objBedInfo.INPATIENT_NO,
                                            p_objBedInfo.PATIENT_NO,
                                            p_objBedInfo.OLD_DEPT_ID,
                                            p_objBedInfo.OLD_DEPT_NAME,
                                            p_objBedInfo.TARGET_DEPT_ID,
                                            p_objBedInfo.TARGET_DEPT_NAME,
                                            p_objBedInfo.BED_NO,
                                            p_objBedInfo.TRANS_TYPE,
                                            p_objBedInfo.TRANS_CODE,
                                            p_objBedInfo.MEDICAL_GROUP_CODE,
                                            p_objBedInfo.CARE_GROUP_CODE,
                                            p_objBedInfo.IN_DOCT_CODE,
                                            p_objBedInfo.NURSE_STATION_CODE,
                                            p_objBedInfo.ZG,
                                            p_objBedInfo.SEQUENCE_NO,
                                            p_objBedInfo.OPER_CODE,
                                            p_objBedInfo.DEPT_CODE,
                                            p_objBedInfo.OLD_NURSE_ID,
                                            p_objBedInfo.OLD_NURSE_NAME,
                                            p_objBedInfo.TARGET_NURSE_ID,
                                            p_objBedInfo.TARGET_NURSE_NAME); //备注
                }
                catch
                {
                    Err = "传入参数错误！RADT.InPatient.InSertInPatientBedTransReord!";
                    WriteErr();
                    return -1;
                }
            }
            return ExecNoQuery(strSql);

        }
        #endregion

    }

}
