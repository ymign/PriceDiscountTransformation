using System;
using Neusoft.HISFC.Models;
using System.Collections;
using System.Collections.Generic;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.Operation;
using System.Data;
namespace Neusoft.HISFC.BizLogic.Operation
{
    #region 手术管理业务描述
    //手术管理包括下列业务：
    //手术人员排班、手术正台分配、手术申请、手术室更换、手术安排
    //麻醉安排、收费模板维护、手术麻醉批费、手术退费、手术登记、麻醉登记、手术取消
    //业务处理对象：
    //手术申请、手术安排、手术取消 业务实际是处理手术申请单类的信息
    //手术登记 业务实际是处理手术登记记录类的信息
    //麻醉登记 业务实施是处理麻醉登记记录类的信息
    //手术麻醉批费、手术退费 业务实际是调用费用接口处理费用
    //收费模板维护 业务实际是调用通用模板类维护本类型模板
    //手术人员安排 业务实际是调用通用排班类维护本类型排班信息
    //手术正台分配 业务实际是处理手术台类信息
    #endregion
    /// [功能描述: 手术管理类]<br></br>
    /// [创 建 者: 王铁全]<br></br>
    /// [创建时间: 2006-09-28]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public abstract class Operation : Neusoft.FrameWork.Management.Database
    {
        public Operation()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 字段

        private OperationRoleEnumService roleType = new OperationRoleEnumService();
        private Neusoft.HISFC.BizLogic.Operation.OpsTableManage TableManage = new OpsTableManage();

        #endregion
        /// <summary>
        /// 手术人员排班
        /// </summary>
        /// <param name=""></param>
        /// <returns>0 success -1 fail</returns>
        public int PlanOperator()
        {
            return 0;
        }

        #region 获取手术申请单信息
        /// <summary>
        /// 获取主要SQL 
        /// </summary>
        /// <returns></returns>
        private string GetOperationSql()
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.Select.2", ref strSql) == -1)
            {
                return null;
            }
            return strSql;
        }
        /// <summary>
        /// 获取主要SQL (新增，仅用于手术安排)
        /// </summary>
        /// <returns></returns>
        private string GetOperationSqlNew()
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.Select.New", ref strSql) == -1)
            {
                return null;
            }
            return strSql;
        }
        /// <summary>
        /// 获取主要SQL (新增，仅用于手术安排菜单下的已完成未收费手术)
        /// </summary>
        /// <returns></returns>
        private string GetAlreadyOperationSql()
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.Select.AlreadyOps", ref strSql) == -1)
            {
                return null;
            }
            return strSql;
        }
        /// <summary>
        /// 根据手术序号获得手术申请单
        /// </summary>
        /// <param name="OperatorNo">手术序号</param>
        /// <returns>手术申请单对象</returns>
        public Neusoft.HISFC.Models.Operation.OperationAppllication GetOpsApp(string operationNo)
        {
            Neusoft.HISFC.Models.Operation.OperationAppllication opsApp = new Neusoft.HISFC.Models.Operation.OperationAppllication();
            ArrayList myAl = new ArrayList();
            //业务规则：遴选出手术时间小于给定时间的所有有效的已进行过手术安排的手术申请单。			
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }

            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.0", ref strSqlwhere) == -1)
            {
                return opsApp;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, operationNo);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.GetOpsApp";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return opsApp;
            }

            strSql = strSql + " \n" + strSqlwhere;
            myAl = this.GetOpsAppListFromSql(strSql);
            //myAl中应该只有一个元素
            if (myAl.Count == 0)
            {
                return null;
            }

            opsApp = myAl[0] as Neusoft.HISFC.Models.Operation.OperationAppllication;


            return opsApp;
        }

        /// <summary>
        /// 获取所有已经发送作废申请的手术列表
        /// </summary>
        /// <param name="?"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ArrayList GetCancelAppList(string deptCode, DateTime beginTime, DateTime endTime)
        {
            string strSql = this.GetOperationSql();
            string strSql1 = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetCancelApplicationApply", ref strSql1) == -1)
            {
                this.Err = "没有找到SQL语句Operator.Operator.GetCancelApplicationApply";
                return null;
            }
            strSql = strSql + "\n" + strSql1;
            strSql = string.Format(strSql, deptCode, beginTime.ToString(), endTime.ToString());
            ArrayList alCancelAppList = this.GetOpsAppListFromSql(strSql);
            return alCancelAppList;
        }

        /// <summary>
        /// 获取患者的手术申请单信息
        /// </summary>
        /// <param name="PatientInfo">患者对象</param>
        /// <param name="Pasource">手术类型1门诊2住院</param>
        /// <param name="endTime">查询截至时间</param>
        /// <returns>患者的手术申请单对象数组</returns>
        public ArrayList GetOpsAppList(Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo, string Pasource, string endTime)
        {
            if (PatientInfo == null)
            {
                return null;	//传入的患者对象为空
            }

            ArrayList myAl = new ArrayList();

            //业务规则：遴选出该患者手术时间小于给定时间的所有有效的未做的手术申请单。

            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.1", ref strSql) == -1)
            {
                return myAl;
            }

            try
            {
                switch (Pasource)
                {
                    case "1":
                        //strSql = string.Format(strSql,PatientInfo.Patient.PID.CardNo,Pasource,endTime);
                        strSql = string.Format(strSql, PatientInfo.ID.ToString(), Pasource, endTime);
                        break;
                    case "2":
                        //患者住院流水号赋值			
                        //PatientInfo.Patient.ID = this.GetInPatientNo(PatientInfo.Patient.PID.ID.ToString());
                        //strSql = string.Format(strSql,PatientInfo.Patient.PID.PatientNo,Pasource,endTime);
                        strSql = string.Format(strSql, PatientInfo.ID.ToString(), Pasource, endTime);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsAppList 1";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }

            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Operation.OperationAppllication opsApplication = new Neusoft.HISFC.Models.Operation.OperationAppllication();

                    opsApplication.ID = Reader[0].ToString();										//手术序号					
                    try
                    {
                        opsApplication.OperationDoctor.ID = Reader[1].ToString();					//手术医生
                        // by zlw 2006-5-24
                        opsApplication.User01 = Reader[1].ToString();	//手术医生所在科室
                    }
                    catch { }

                    opsApplication.GuideDoctor.ID = Reader[2].ToString();											//指导医生	

                    opsApplication.PreDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[3].ToString());		//手术预约时间					
                    try
                    {
                        opsApplication.Duration = System.Convert.ToDecimal(Reader[4].ToString());					//手术预定用时					
                    }
                    catch { }
                    opsApplication.AnesType.ID = Reader[5].ToString();												//麻醉类型

                    opsApplication.ExeDept.ID = Reader[6].ToString();												//执行科室

                    opsApplication.OperateRoom = opsApplication.ExeDept as Neusoft.HISFC.Models.Base.Department;	//手术室(对于需要填申请单的手术来说，手术室即执行科室)

                    opsApplication.TableType = Reader[7].ToString();					//0正台1加台2点台					

                    opsApplication.ApplyDoctor.ID = Reader[8].ToString();				//申请医生


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
                    try
                    {
                        opsApplication.BloodNum = System.Convert.ToDecimal(Reader[20].ToString());		//血量
                    }
                    catch { }
                    opsApplication.BloodUnit = Reader[21].ToString();					//用血单位
                    opsApplication.OpsNote = Reader[22].ToString();						//手术注意事项
                    opsApplication.AneNote = Reader[23].ToString();						//麻醉注意事项				
                    opsApplication.ExecStatus = Reader[24].ToString();					//1手术申请 2 手术审批 3手术安排 4手术完成


                    string strFinished = Reader[25].ToString();					//0未做手术/1已做手术
                    opsApplication.IsFinished = Neusoft.FrameWork.Function.NConvert.ToBoolean(strFinished);


                    string strAnesth = Reader[26].ToString();					//0未麻醉/1已麻醉
                    opsApplication.IsAnesth = Neusoft.FrameWork.Function.NConvert.ToBoolean(strAnesth);

                    opsApplication.Folk = Reader[27].ToString();						//签字家属
                    opsApplication.RelaCode.ID = Reader[28].ToString();					//家属关系
                    opsApplication.FolkComment = Reader[29].ToString();					//家属意见

                    try
                    {
                        string strUrgent = Reader[30].ToString();					//加急手术,1是/0否
                        opsApplication.IsUrgent = Neusoft.FrameWork.Function.NConvert.ToBoolean(strUrgent);
                    }
                    catch { }
                    try
                    {
                        string strChange = Reader[31].ToString();					//1病危/0否
                        opsApplication.IsChange = Neusoft.FrameWork.Function.NConvert.ToBoolean(strChange);
                    }
                    catch { }
                    try
                    {
                        string strHeavy = Reader[32].ToString();						//1重症/0否
                        opsApplication.IsHeavy = Neusoft.FrameWork.Function.NConvert.ToBoolean(strHeavy);
                    }
                    catch { }
                    try
                    {
                        opsApplication.IsSpecial = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[33].ToString());	//1特殊手术/0否
                    }
                    catch { }

                    opsApplication.User.ID = Reader[34].ToString();	//操作员

                    try
                    {
                        opsApplication.IsUnite = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[35].ToString());//1合并/0否	
                    }
                    catch { }
                    opsApplication.OperateKind = Reader[37].ToString();					//1普通2急诊3感染				
                    try
                    {
                        string strIsNeedAcco = Reader[38].ToString();					//是否需要随台护士
                        opsApplication.IsAccoNurse = Neusoft.FrameWork.Function.NConvert.ToBoolean(strIsNeedAcco);
                    }
                    catch { }
                    try
                    {
                        string strIsNeedPrep = Reader[39].ToString();					//是否需要巡回护士
                        opsApplication.IsPrepNurse = Neusoft.FrameWork.Function.NConvert.ToBoolean(strIsNeedPrep);
                    }
                    catch { }
                    opsApplication.RoomID = Reader[40].ToString();
                    opsApplication.PatientInfo = PatientInfo.Clone();					//患者基本信息					
                    opsApplication.PatientSouce = Pasource;									//1门诊2住院
                    opsApplication.IsValid = true;										//1有效0无效	

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
                foreach (Neusoft.HISFC.Models.Operation.OperationAppllication opsApplication in myAl)
                {
                    opsApplication.DiagnoseAl = this.GetIcdFromApp(opsApplication);							//诊断列表					
                    opsApplication.OperationInfos = this.GetOpsInfoFromApp(opsApplication.ID);				//手术项目信息列表				
                    opsApplication.RoleAl = this.GetRoleFromApp(opsApplication.ID);							//人员角色列表
                    //冗余属性赋值，为突出表现层申请部分业务调用方便
                    foreach (Neusoft.HISFC.Models.Operation.ArrangeRole thisRole in opsApplication.RoleAl)
                    {
                        if (thisRole.RoleType.ID.ToString() == Neusoft.HISFC.Models.Operation.EnumOperationRole.Helper1.ToString()
                            || thisRole.RoleType.ID.ToString() == Neusoft.HISFC.Models.Operation.EnumOperationRole.Helper2.ToString()
                            || thisRole.RoleType.ID.ToString() == Neusoft.HISFC.Models.Operation.EnumOperationRole.Helper3.ToString())
                            //助手医师列表
                            opsApplication.HelperAl.Add(thisRole.Clone());
                    }
                    opsApplication.AppaRecAl = GetAppaRecFromApp(opsApplication.ID);//手术资料安排列表
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
        /// 获取手术科室手术申请单信息 (重载)
        /// </summary>
        /// <param name="OpsRoomID">手术科室编码</param>
        /// <param name="endTime">查询截至时间</param>
        /// <returns>手术申请单对象数组</returns>
        public ArrayList GetOpsAppList(string opsRoomID, string endTime)
        {
            ArrayList myAl = new ArrayList();
            //业务规则：遴选出手术室手术时间小于给定时间的所有有效的未做的手术申请单。			
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return myAl;
            }
            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.2", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, opsRoomID, endTime);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsAppList 2";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            return GetOpsAppListFromSql(strSql);
        }
        /// <summary>
        /// 获取手术科室手术申请单信息 (重载)
        /// </summary>
        /// <param name="OpsRoomID">手术科室编码</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">查询截至时间</param>
        /// <param name="isNeedApprove">是否需要审核 true 是 false 否</param>
        /// <returns>手术申请单对象数组</returns>
        public ArrayList GetOpsAppList(string OpsRoomID, string beginTime, string endTime, bool isNeedApprove)
        {
            ArrayList myAl = new ArrayList();

            string strSql = GetOperationSqlNew();
            if (strSql == null)
            {
                return null;
            }

            string strSqlwhere = string.Empty;
            if (isNeedApprove == false)
            {
                if (this.Sql.GetSql("Operator.Operator.GetApplication.6", ref strSqlwhere) == -1)
                {
                    return myAl;
                }
            }
            else
            {
                if (this.Sql.GetSql("Operator.Operator.GetApplication.NeedApprove", ref strSqlwhere) == -1)
                {
                    return myAl;
                }
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, OpsRoomID, beginTime, endTime);
            }
            catch (Exception ex)
            {
                if (isNeedApprove == false)
                {
                    this.Err = "HISFC.Operation.Operation.GetOpsAppList.6";
                }
                else
                {
                    this.Err = "HISFC.Operation.Operation.GetOpsAppList.NeedApprove";
                }
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            return GetOpsAppListFromSqlNew(strSql);
        }
        /// <summary>
        /// 获取手术科室已完成未收费的手术信息，仅仅用于手术安排菜单下的已完成未收费手术 add by zhy
        /// </summary>
        /// <param name="OpsRoomID">手术科室编码</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">查询截至时间</param>
        /// <param name="isNeedApprove">是否需要审核 true 是 false 否</param>
        /// <returns>手术申请单对象数组</returns>
        public ArrayList GetAlreadyOpsList(string OpsRoomID, string beginTime, string endTime)
        {
            ArrayList myAl = new ArrayList();
            string strSql = GetAlreadyOperationSql();
            if (strSql == null)
            {
                return null;
            }
            try
            {
                strSql = string.Format(strSql, OpsRoomID, beginTime, endTime);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }

            //return GetAlreadyOpsListFromSql(strSql);
            return GetOpsAppListFromSqlNew(strSql);
        }
        /// <summary>
        /// 获取某个时间段内的所有的有效的手术申请，麻醉收费时参照用
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ArrayList GetAnaesthAppList(string beginTime, string endTime)
        {
            ArrayList myAl = new ArrayList();

            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operation.Select.2", ref strSql) == -1)
            {
                return myAl;
            }

            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operation.GetApplication.zjy", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, beginTime, endTime);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsAppList zjy";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            return GetOpsAppListFromSql(strSql);
        }
        /// <summary>
        /// 获取某手术室在某个时间内所有的有效的手术申请单信息
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="DeptId"></param>
        /// <returns></returns>
        public ArrayList GetApplistByDeptIdandTime(string beginTime, string endTime, string DeptId)
        {
            ArrayList myAl = new ArrayList();

            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }

            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.GetApplistByDeptIdandTime", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, beginTime, endTime, DeptId);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsAppList GetApplistByDeptIdandTime";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            return GetOpsAppListFromSql(strSql);
        }
        /// <summary>
        /// 获取所有已安排过的手术申请单信息 (重载)
        /// </summary>
        /// <param name="endTime">查询截至时间</param>
        /// <returns>手术申请单对象数组</returns>
        public ArrayList GetOpsAppList(string endTime)
        {
            ArrayList myAl = new ArrayList();
            //业务规则：遴选出手术时间小于给定时间的所有有效的已进行过手术安排的手术申请单。			
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }

            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.3", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, endTime);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsAppList 3";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }
        /// <summary>
        /// 获取所有已安排过的手术申请单信息 (重载)hxw
        /// </summary>
        /// <param name="beginDate">查询开始时间</param>
        /// <param name="endTime">查询截至时间</param>
        /// <returns>手术申请单对象数组</returns>
        public ArrayList GetOpsAppList(DateTime beginDate, DateTime endTime)
        {
            ArrayList myAl = new ArrayList();

            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }

            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.7", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, beginDate.ToString(), endTime.ToString());
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsAppList 7";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }
        //{F86F131E-E35D-47ba-A090-38199D6C7584}feng.ch
        /// <summary>
        /// 查询特定手术室的所有已经过手术安排的信息
        /// </summary>
        /// <param name="deptCode">手术室编码</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="ds">数据集</param>
        /// <returns>成功返回数据集</returns>
        public DataSet GetOpsAppList(string deptCode, DateTime beginDate, DateTime endTime, ref DataSet ds)
        {
            ds = new DataSet();

            string strSql = "";
            if (this.Sql.GetSql("Operator.Operator.GetApplication.ds", ref strSql) == -1)
            {
                this.Err = "Operator.Operator.GetApplication.ds";
                this.WriteErr();
                return null;
            }
            strSql = string.Format(strSql, deptCode, beginDate, endTime);
            if (this.ExecQuery(strSql, ref ds) == -1) return null;
            return ds;
        }
        /// <summary>
        /// 根据住院流水号获取该患者最大手术序号
        /// </summary>
        /// <param name="inpatientno"></param>
        /// <returns></returns>
        public string GetMaxByPatient(string inpatientno)
        {
            string strSql = string.Empty;
            #region sql
            //			  select max(operationno)
            //				from met_ops_apply
            //			where inpatient_no='{0}'
            //				and ynvalid='1'
            #endregion
            if (this.Sql.GetSql("Operator.Operator.GetApplication.8", ref strSql) == -1)
            {
                return string.Empty;
            }

            try
            {
                strSql = string.Format(strSql, inpatientno);
            }
            catch (Exception e)
            {
                this.Err = "HISFC.Operator.Operator.GetMaxByPatient" + e.Message;
                this.ErrCode = e.Message;
                WriteErr();

                return string.Empty;
            }

            return this.ExecSqlReturnOne(strSql);
        }
        /// <summary>
        /// 获取所有未登记过的手术申请单信息 (重载)
        /// </summary>
        /// <param name="OpsRoomID">手术科室编码</param>
        /// <param name="beginTime">查询起始时间</param>
        /// <param name="endTime">查询截至时间</param>
        /// <param name="Valid">0无效 1 有效</param>
        /// <returns>手术申请单对象数组</returns>
        public ArrayList GetOpsAppList(string OpsRoomID, DateTime beginTime, DateTime endTime, string Valid)
        {
            ArrayList myAl = new ArrayList();
            //业务规则：遴选出手术时间小于给定时间的所有有效的已进行过手术安排的手术申请单。			
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }

            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.4", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, OpsRoomID, beginTime.ToString(), endTime.ToString(), Valid);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsList 4";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }
        /// <summary>
        /// 获取已安排未登记手术，以及后来加的已手术未收费状态的患者(by zhy)
        /// </summary>
        /// <param name="OpsRoomID"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="Isvalid"></param>
        /// <returns></returns>
        public ArrayList GetOpsAppList(string OpsRoomID, DateTime begin, DateTime end, bool Isvalid)
        {
            ArrayList myAl = new ArrayList();
            //业务规则：遴选出手术时间小于给定时间的所有有效的已进行过手术安排的手术申请单。			
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }
            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.9", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, OpsRoomID, begin.ToString(), end.ToString(),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(Isvalid));
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsList 9";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }

        /// <summary>
        /// 获取已安排未登记手术，以及后来加的已手术未收费状态的患者(by zhy)
        /// </summary>
        /// <param name="OpsRoomID"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="Isvalid"></param>
        /// <returns></returns>
        public ArrayList GetOpsAppListForOpsNur(string inPatientNo)
        {
            ArrayList myAl = new ArrayList();
            //业务规则：遴选出手术时间小于给定时间的所有有效的已进行过手术安排的手术申请单。			
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }
            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.81", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, inPatientNo);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsList 81";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }

        /// <summary>
        /// 获取已安排未登记手术，以及后来加的已手术未收费状态的患者(by zhy)
        /// </summary>
        /// <param name="OpsRoomID"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="Isvalid"></param>
        /// <returns></returns>
        public ArrayList GetOpsAppListForAnaNur(string inPatientNo)
        {
            ArrayList myAl = new ArrayList();
            //业务规则：遴选出手术时间小于给定时间的所有有效的已进行过手术安排的手术申请单。			
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }
            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.82", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, inPatientNo);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsList 82";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }

        //==============================================================================================
        //==============================================================================================
        #region 新增加代码
        //修改人：路志鹏 时间：2007-4-12 
        //目的：在手术登记中增加按照住院号查询到该住院号以安排的手术申请
        public ArrayList GetOpsAppListByPatient(string OpsRoomID, string Patient_code)
        {
            ArrayList myAl = new ArrayList();
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }
            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.11", ref strSqlwhere) == -1)
            {
                return myAl;
            }
            strSqlwhere = string.Format(strSqlwhere, OpsRoomID, Patient_code);
            strSql = strSql + "\n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }

        //目的：在手术登记窗口中显示作废的手术单
        public ArrayList GetOpsCancelRecord(string ExeDeptID, DateTime begin, DateTime end)
        {
            ArrayList myAl = new ArrayList();
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.Select.3", ref strSql) == -1)
            {
                return myAl;
            }
            strSql = string.Format(strSql, ExeDeptID, begin.ToString(), end.ToString());
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }

        /// <summary>
        /// 在手术登记中增加按照住院号查询到该住院号的手术申请
        /// 中大五院要求不安排直接可以登记 chengym 2016-10-18
        /// </summary>
        /// <param name="OpsRoomID">执行科室</param>
        /// <param name="Patient_code">住院号</param>
        /// <returns></returns>
        public ArrayList GetOpsAppListByPatientNo(string OpsRoomID, string Patient_code)
        {
            ArrayList myAl = new ArrayList();
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }
            string strSqlwhere = string.Empty;
            strSqlwhere = @"
                     where  exec_dept = '{0}'  and patient_no='{1}'and ynvalid = '1'      order by apply_dpcd";
            //if (this.Sql.GetSql("Operator.Operator.GetApplication.11", ref strSqlwhere) == -1)
            //{
            //    return myAl;
            //}
            strSqlwhere = string.Format(strSqlwhere, OpsRoomID, Patient_code);
            strSql = strSql + "\n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }
        #endregion

        //===========================================================================================
        //===========================================================================================

        /// <summary>
        /// 获取安排在同一个房间的所有手术 
        /// </summary>
        /// <param name="dept">房间号</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList GetOpsApplistInSameRoom(Neusoft.FrameWork.Models.NeuObject dept, DateTime begin, DateTime end)
        {
            ArrayList myAl = new ArrayList();

            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }

            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplicationInSameRoom", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, dept.ID, begin.ToString(), end.ToString());
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsList 10";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }
        /// <summary>
        /// 获取科室一段时间范围内所有申请单
        /// </summary>
        /// <param name="dept">科室</param>
        /// <param name="begin">预约开始时间</param>
        /// <param name="end">预约结束时间</param>
        /// <returns></returns>
        public ArrayList GetOpsAppList(Neusoft.FrameWork.Models.NeuObject dept, DateTime begin, DateTime end)
        {
            ArrayList myAl = new ArrayList();

            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }
            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.10", ref strSqlwhere) == -1)
            {
                return myAl;
            }
            try
            {
                strSqlwhere = string.Format(strSqlwhere, dept.ID, begin.ToString(), end.ToString());
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsList 10";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }

        public ArrayList GetOpsApplyListForBlood(string inpatientNo)
        {
            string sqlStr = string.Empty;
            ArrayList al = new ArrayList();
            if (this.Sql.GetSql("Operator.Operator.Select.Antibiotic", ref sqlStr) == -1)
            {
                return null;
            }
            try
            {
                Neusoft.FrameWork.Models.NeuObject obj;
                sqlStr = string.Format(sqlStr, inpatientNo);
                this.ExecQuery(sqlStr);
                while (this.Reader.Read())
                {
                    obj = new NeuObject();
                    obj.ID = Reader[0].ToString();// 手术序号
                    obj.Name = Reader[3].ToString();// 项目名称
                    al.Add(obj);
                }
            }
            catch (Exception ex)
            {
                this.Err = "Operator.Operator.Select.Antibiotic";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return null;
            }
            return al;
        }

        public Neusoft.HISFC.Models.Operation.OperationAppllication GetOpsAppListbyInpNo(string inpatientNo)
        {
            ArrayList al = new ArrayList();
            Neusoft.HISFC.Models.Operation.OperationAppllication opsApplication = new Neusoft.HISFC.Models.Operation.OperationAppllication();
            string sqlStr = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.Select.Antibiotic", ref sqlStr) == -1)
            {
                return null;
            }
            try
            {
                sqlStr = string.Format(sqlStr, inpatientNo);
                this.ExecQuery(sqlStr);
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Operation.OperationInfo opsInfo = new OperationInfo();
                    opsApplication.ID = Reader[0].ToString();// 手术序号
                    opsApplication.PatientInfo.ID = Reader[1].ToString();// 门诊号
                    opsInfo.OperationItem.ID = Reader[2].ToString();// 项目编号
                    opsInfo.OperationItem.Name = Reader[3].ToString();// 项目名称
                    opsApplication.InciType.ID = Reader[4].ToString();// 切口
                    opsApplication.OperationInfos.Add(opsInfo);
                }
            }
            catch (Exception ex)
            {
                this.Err = "Operator.Operator.Select.Antibiotic";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return null;
            }
            return opsApplication;
        }
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
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }

            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.5", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, beginTime.ToString(), endTime.ToString(), Valid);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsAppList 5";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
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
                    opsApplication.SPECIAL2 = Reader[61].ToString();
                    opsApplication.FROZEN = Reader[62].ToString();
                    opsApplication.FROZENDATE = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[63].ToString());
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
        /// 获得给定SQL语句查询出的申请单对象数组,新增，用meno来存手术不安排的原因，add by zhy
        /// </summary>
        /// <param name="strSql">指定的查询语句</param>
        /// <returns>手术申请单对象数组</returns>
        private ArrayList GetOpsAppListFromSqlNew(string strSql)
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
                    opsApplication.Memo = Reader[60].ToString();
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
        /// <summary>
        /// 根据手术序号获得手术资料安排列表
        /// </summary>
        /// <param name="OperatorNo">手术申请单序号</param>
        /// <returns>指定的手术人员安排类对象数组</returns>
        public ArrayList GetAppaRecFromApp(string OperatorNo)
        {
            ArrayList AppaRecAl = new ArrayList();
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetAppaRec.1", ref strSql) == -1)
            {
                return AppaRecAl;//空数组
            }

            try
            {
                strSql = string.Format(strSql, OperatorNo);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetAppaRecFromApp";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return AppaRecAl;
            }
            if (strSql == null)
            {
                return AppaRecAl;
            }

            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Operation.OpsApparatusRec thisRec = new Neusoft.HISFC.Models.Operation.OpsApparatusRec();

                    thisRec.OperationNo = OperatorNo;//手术序号
                    thisRec.OpsAppa.ID = Reader[0].ToString();//设备代码					
                    thisRec.OpsAppa.Name = Reader[1].ToString();//设备名称
                    thisRec.foreflag = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[2].ToString());//1.术前安排/2.术后记录						
                    if (thisRec.foreflag == 0)
                        thisRec.foreflag = 1;
                    thisRec.Qty = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[3].ToString());//数量
                    thisRec.AppaUnit = Reader[4].ToString();//单位

                    AppaRecAl.Add(thisRec);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得手术资料安排信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return AppaRecAl;
            }
            this.Reader.Close();
            return AppaRecAl;
        }

        #endregion
        #region 针对手术申请单的操作
        /// <summary>
        /// 获取新手术申请单序号
        /// </summary>
        /// <returns>新申请单序号</returns>
        public string GetNewOperationNo()
        {
            string strNewNo = string.Empty;
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetNewOperationNo.1", ref strSql) == -1)
            {
                return strNewNo; //空字符串
            }
            if (strSql == null) return strNewNo;
            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    strNewNo = Reader[0].ToString();
                }
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetNewOperationNo";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return strNewNo;
            }
            this.Reader.Close();
            return strNewNo;
        }
        /// <summary>
        /// 获取手术申请单状态
        /// </summary>
        /// <param name="OperatorNo">手术申请单序号</param>
        /// <returns>申请单状态 1已申请 2已审批 3已安排 4已完成</returns>
        public string GetApplicationStatus(string operatorNO)
        {
            string strStatus = string.Empty;
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplicationStatus.1", ref strSql) == -1)
            {
                return strStatus;//空数组

            }

            try
            {
                strSql = string.Format(strSql, operatorNO);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetApplicationStatus";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return strStatus;
            }

            if (strSql == null)
                return strStatus;

            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    strStatus = Reader[0].ToString();
                }
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.GetApplicationStatus";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return strStatus;
            }
            this.Reader.Close();

            return strStatus;
        }

        /// <summary>
        /// 手术申请(新建手术申请单)
        /// </summary>
        /// <param name="OpsApp">手术申请单实例</param>
        /// <returns>0 success -1 fail</returns>
        public int CreateApplication(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            #region 新建手术申请单
            ///新建手术申请单
            ///Operation.Operation.CreateApplication.1
            ///传入：58
            ///传出：0 
            #endregion
            string strSql = string.Empty;
            #region 获取患者基本信息
            //--------------------------------------------------------		
            //局部变量定义
            string ls_ClinicCode = string.Empty;		//住院流水号/门诊号
            string ls_PatientNo = string.Empty;			//病案号/病历号
            string ls_Name = string.Empty;				//患者姓名
            string ls_Sex = string.Empty;				//性别
            DateTime ldt_Birthday = DateTime.MinValue;	//生日
            string ls_DeptCode = string.Empty;			//住院科室
            string ls_BedNo = string.Empty;				//床位号
            string ls_BloodCode = string.Empty;			//患者血型
            decimal ld_PrePay = 0;						//预交金
            string ls_SickRoom = string.Empty;			//病房号		

            #region 首先判断是门诊手术还是住院手术，确定应该输入的患者号
            switch (OpsApp.PatientSouce)
            {
                case "1":  //门诊手术
                    ls_PatientNo = OpsApp.PatientInfo.PID.CardNO;
                    break;
                case "2":  //住院手术
                    ls_PatientNo = OpsApp.PatientInfo.PID.PatientNO;
                    break;
                default:
                    break;
            }
            #endregion

            ls_ClinicCode = OpsApp.PatientInfo.ID.ToString();
            //ls_PatientNo = OpsApp.PatientInfo.Patient.PID.RecordNo;
            ls_Name = OpsApp.PatientInfo.Name;
            ls_Sex = OpsApp.PatientInfo.Sex.ID.ToString();
            ldt_Birthday = OpsApp.PatientInfo.Birthday;
            ls_DeptCode = OpsApp.PatientInfo.PVisit.PatientLocation.Dept.ID.ToString();
            ls_BedNo = OpsApp.PatientInfo.PVisit.PatientLocation.Bed.ID.ToString();
            ls_BloodCode = OpsApp.PatientInfo.BloodType.ID.ToString();
            ld_PrePay = OpsApp.PatientInfo.FT.PrepayCost;
            ls_SickRoom = OpsApp.PatientInfo.PVisit.PatientLocation.Room;
            //--------------------------------------------------------
            #endregion
            DateTime ldt_ReceptDate = DateTime.MinValue;//接患者时间
            //对新增申请单：申请时间即为当前系统时间
            OpsApp.ApplyDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.GetSysDateTime());
            OpsApp.ExeDept = OpsApp.OperateRoom;//执行科室(对于需要填申请单的手术来说，手术室即执行科室)
            string strGerm = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsGermCarrying).ToString();
            string strFinished = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsFinished).ToString();
            string strAnesth = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsAnesth).ToString();
            string strUrgent = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsUrgent).ToString();
            string strChange = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsChange).ToString();
            string strHeavy = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsHeavy).ToString();
            string strSpecial = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsSpecial).ToString();
            string strValid = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsValid).ToString();
            string strUnite = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsUnite).ToString();
            string strNeedAcco = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsAccoNurse).ToString();
            string strNeedPrep = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsPrepNurse).ToString();
            //默认取第一个诊断为统计术前诊断
            string strDiagnose = string.Empty;
            if (OpsApp.DiagnoseAl.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase MainDiagnose in OpsApp.DiagnoseAl)
                {
                    if (MainDiagnose.IsValid)
                    {
                        strDiagnose = MainDiagnose.Name + "(" + MainDiagnose.ID.ToString() + ")";
                        break;
                    }
                }
            }

            if (this.Sql.GetSql("Operator.Operator.CreateApplication.1", ref strSql) == -1)
            {
                return -1;
            }

            try
            {
                string[] str = new string[]{OpsApp.ID,ls_ClinicCode,ls_PatientNo,OpsApp.PatientSouce,ls_Name,
												   ls_Sex,ldt_Birthday.ToString(),ld_PrePay.ToString(),ls_DeptCode,ls_BedNo,
												  ls_BloodCode,strDiagnose,OpsApp.OperateKind,OpsApp.OperationDoctor.ID.ToString(),OpsApp.GuideDoctor.ID.ToString(),
						ls_SickRoom,OpsApp.PreDate.ToString(),OpsApp.Duration.ToString(),OpsApp.AnesType.ID.ToString(),OpsApp.HelperAl.Count.ToString(),
						"0","0","0",OpsApp.ExeDept.ID.ToString(),OpsApp.TableType,
						OpsApp.ApplyDoctor.ID.ToString(),OpsApp.ApplyDoctor.Dept.ID.ToString(),OpsApp.ApplyDate.ToString(),OpsApp.ApplyNote,OpsApp.ApproveDoctor.ID.ToString(),
						OpsApp.ApproveDate.ToString(),OpsApp.ApproveNote,"",OpsApp.OperationType.ID.ToString(),OpsApp.InciType.ID.ToString(),
						strGerm,OpsApp.ScreenUp,OpsApp.OpsTable.ID.ToString(),ldt_ReceptDate.ToString(),OpsApp.BloodType.ID.ToString(),
						OpsApp.BloodNum.ToString(),OpsApp.BloodUnit,OpsApp.OpsNote,OpsApp.AneNote,OpsApp.ExecStatus,
						strFinished,strAnesth,OpsApp.Folk,OpsApp.RelaCode.ID.ToString(),OpsApp.FolkComment,
						strUrgent,strChange,strHeavy,strSpecial,OpsApp.User.ID.ToString(),
						System.DateTime.Now.ToString(),strValid,strUnite,"",strNeedAcco,
                        strNeedPrep,OpsApp.OperationDoctor.Dept.ID,OpsApp.AnesWay,OpsApp.Position,OpsApp.Eneity,
                        OpsApp.LastTime,OpsApp.IsOlation,OpsApp.IsPlanToICU,OpsApp.SPECIAL2,OpsApp.FROZEN,
                        OpsApp.FROZENDATE.ToString()
											   };
                //手术申请单表中增加记录
                //				//每行5个参数
                //				strSql = string.Format(strSql,OpsApp.OperationNo,ls_ClinicCode,ls_PatientNo,OpsApp.Pasource,ls_Name,
                //					ls_Sex,ldt_Birthday.ToString(),ld_PrePay.ToString(),ls_DeptCode,ls_BedNo,
                //					ls_BloodCode,strDiagnose,OpsApp.OperateKind,OpsApp.Ops_docd.ID.ToString(),OpsApp.Gui_docd.ID.ToString(),
                //					ls_SickRoom,OpsApp.Pre_Date.ToString(),OpsApp.Duration.ToString(),OpsApp.Anes_type.ID.ToString(),OpsApp.HelperAl.Count,
                //					0,0,0,OpsApp.ExeDept.ID.ToString(),OpsApp.TableType,
                //					OpsApp.Apply_Doct.ID.ToString(),OpsApp.Apply_Doct.Dept.ID.ToString(),OpsApp.Apply_Date.ToString(),OpsApp.ApplyNote,OpsApp.ApprDocd.ID.ToString(),
                //					OpsApp.ApprDate.ToString(),OpsApp.ApprNote,"",OpsApp.OperateType.ID.ToString(),OpsApp.InciType.ID.ToString(),
                //					strGerm,OpsApp.ScreenUp,OpsApp.OpsTable.ID.ToString(),ldt_ReceptDate.ToString(),OpsApp.BloodType.ID.ToString(),
                //					OpsApp.BloodNum.ToString(),OpsApp.BloodUnit,OpsApp.OpsNote,OpsApp.AneNote,OpsApp.ExecStatus,
                //					strFinished,strAnesth,OpsApp.Folk,OpsApp.RelaCode.ID.ToString(),OpsApp.FolkComment,
                //					strUrgent,strChange,strHeavy,strSpecial,OpsApp.User.ID.ToString(),
                //					this.GetSysDateTime(),strValid,strUnite,"",strNeedAcco,strNeedPrep);
                if (this.ExecNoQuery(strSql, str) == -1)
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.CreateApplication";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            #region 处理手术项目基本信息
            //--------------------------------------------------------			
            //针对本申请单中涉及到的手术添加手术信息
            foreach (Neusoft.HISFC.Models.Operation.OperationInfo OperateInfo in OpsApp.OperationInfos)
            {
                //添加手术信息
                try
                {
                    if (AddOperationInfo(OpsApp, OperateInfo) == -1)
                    {
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    this.Err = "HISFC.Operation.Operation.CreateApplication OperateInfo";
                    this.ErrCode = ex.Message;
                    this.WriteErr();
                    return -1;
                }
            }
            //--------------------------------------------------------
            #endregion
            #region 处理手术诊断信息
            //获得患者已有的所有术前诊断
            ArrayList al = this.GetIcdFromApp(OpsApp);
            //判断是否存在记录的标志
            bool bIsExist = false;
            //遍历要加入的诊断信息列表(OpsApp.DiagnoseAl)
            foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase willAddDiagnose in OpsApp.DiagnoseAl)
            {
                bIsExist = false;
                //遍历患者已有的所有手术诊断，如果willAddDiagnose已经存在，更新其状态，
                //如果willAddDiagnose尚不存在，则新增该记录到数据库中
                foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase thisDiagnose in al)
                {
                    if (thisDiagnose.HappenNo == willAddDiagnose.HappenNo && thisDiagnose.Patient.ID.ToString() == willAddDiagnose.Patient.ID.ToString())
                    {
                        //已经存在	更新				
                        //TODO:病案业务层
                        //if(this.DiagnoseManager.UpdatePatientDiagnose(willAddDiagnose) == -1) return -1;
                        //bIsExist = true;
                    }
                }
                //遍历完毕后发现不存在 新增
                if (bIsExist == false)
                {
                    //TODO:病案业务层
                    //if(this.DiagnoseManager.CreatePatientDiagnose(willAddDiagnose) == -1) return -1;
                }
            }
            #endregion
            #region 处理手术人员角色信息
            try
            {
                if (this.ProcessRoleForApply(OpsApp) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.CreateApplication Role";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            #endregion
            return 0;
        }

        /// <summary>
        /// 修改手术患者的状态，从ExecStatus=3的已安排状态改为ExecStatus=6的已手术未收费状态，此状态为医院要求添加
        /// </summary>
        /// <param name="OpsApp">手术申请单实例</param>
        /// <returns>0 success -1 fail</returns>
        public int UpdateAlreadyOperation(Neusoft.HISFC.Models.Operation.OperationAppllication AlreadyOpsApp)
        {
            // AlreadyOpsApp.ExecStatus = '6';
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.UpdateAlreadyOperation.1", ref strSql) == -1) return -1;
            try
            {
                string[] str = new string[] { AlreadyOpsApp.ID, "6" };
                if (this.ExecNoQuery(strSql, str) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.UpdateAlreadyOperation";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 修改手术患者的状态，从ExecStatus=6的已安排状态改为ExecStatus=3的已手术未收费状态，此状态为医院要求添加
        /// </summary>
        /// <param name="OpsApp">手术申请单实例</param>
        /// <returns>0 success -1 fail</returns>
        public int UpdateCancelAlreadyOperation(Neusoft.HISFC.Models.Operation.OperationAppllication CancelAlreadyOps)
        {
            // AlreadyOpsApp.ExecStatus = '6';
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.UpdateCancelAlreadyOperation.1", ref strSql) == -1) return -1;
            try
            {
                string[] str = new string[] { CancelAlreadyOps.ID, "3" };
                if (this.ExecNoQuery(strSql, str) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.UpdateCancelAlreadyOperation";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 获取已经做手术而为收费的患者信息
        /// </summary>
        /// <param name="OpsRoomID">手术科室编码</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">查询截至时间</param>
        /// <param name="isNeedApprove">是否需要审核 true 是 false 否</param>
        /// <returns>手术申请单对象数组</returns>
        public ArrayList GetAlreadyOpsAppList(string OpsRoomID, string beginTime, string endTime, bool isNeedApprove)
        {
            ArrayList myAl = new ArrayList();
            string sqlStr = string.Empty;
            sqlStr = @"select a.operationno,a.clinic_code,a.name,a.execstatus  
                       from met_ops_apply a
                       where pre_date >= to_date('{1}','yyyy-mm-dd HH24:mi:ss') 
                       and pre_date <= to_date('{2}','yyyy-mm-dd HH24:mi:ss')
                       --and ynanesth = '1'
                       and a.ynvalid='1'
                       and a.execstatus='3'";
            try
            {
                sqlStr = string.Format(sqlStr, OpsRoomID, beginTime, endTime, isNeedApprove);
                if (this.ExecQuery(sqlStr) == -1)
                {
                    this.Err = "查找已手术患者信息失败";
                    return myAl;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return myAl;
            }
            return GetAlreadyOpsAppListFromSqlNew(sqlStr);
        }

        /// <summary>
        /// 获得给定SQL语句查询出的已手术未收费对象数组,新增，add by zhy
        /// </summary>
        /// <param name="strSql">指定的查询语句</param>
        /// <returns>已手术未收费患者对象数组</returns>
        private ArrayList GetAlreadyOpsAppListFromSqlNew(string strSql)
        {
            ArrayList myAl = new ArrayList();
            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Operation.OperationAppllication opsApplication = new Neusoft.HISFC.Models.Operation.OperationAppllication();
                    opsApplication.ID = Reader[0].ToString();					//手术序号	
                    opsApplication.PatientInfo.ID = Reader[1].ToString();//门诊号/住院流水号
                    opsApplication.PatientInfo.Name = Reader[2].ToString();	//姓名
                    opsApplication.ExecStatus = Reader[3].ToString();	//手术状态
                    myAl.Add(opsApplication);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得已手术患者信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return myAl;
            }
            this.Reader.Close();

            return myAl;
        }

        /// <summary>
        /// 手术申请(修改手术申请单)
        /// </summary>
        /// <param name="OpsApp">手术申请单实例</param>
        /// <returns>0 success -1 fail</returns>
        public int UpdateApplicationByFee(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            #region 修改手术申请单
            ///修改手术申请单
            ///Operation.Operation.UpdateApplication.1
            ///传入：58
            ///传出：0 
            #endregion

            string strSql = string.Empty;
            #region 获取患者基本信息
            //--------------------------------------------------------		
            //局部变量定义
            string ls_ClinicCode = string.Empty;//住院流水号/门诊号
            string ls_PatientNo = string.Empty; //病案号/病历号
            string ls_Name = string.Empty;	  //患者姓名
            string ls_Sex = string.Empty;		  //性别
            DateTime ldt_Birthday = DateTime.MinValue; //生日
            string ls_DeptCode = string.Empty;  //住院科室
            string ls_BedNo = string.Empty;	  //床位号
            string ls_BloodCode = string.Empty; //患者血型
            decimal ld_PrePay = 0;	  //预交金
            string ls_SickRoom = string.Empty;  //病房号

            #region 首先判断是门诊手术还是住院手术，确定应该输入的患者号
            switch (OpsApp.PatientSouce)
            {
                case "1":  //门诊手术
                    ls_PatientNo = OpsApp.PatientInfo.PID.CardNO;
                    break;
                case "2":  //住院手术
                    ls_PatientNo = OpsApp.PatientInfo.PID.PatientNO;
                    break;
            }
            #endregion
            ls_ClinicCode = OpsApp.PatientInfo.ID.ToString();
            //ls_PatientNo = OpsApp.PatientInfo.Patient.PID.RecordNo;
            ls_Name = OpsApp.PatientInfo.Name;
            ls_Sex = OpsApp.PatientInfo.Sex.ID.ToString();
            ldt_Birthday = OpsApp.PatientInfo.Birthday;
            ls_DeptCode = OpsApp.PatientInfo.PVisit.PatientLocation.Dept.ID.ToString();
            ls_BedNo = OpsApp.PatientInfo.PVisit.PatientLocation.Bed.ID.ToString();
            ls_BloodCode = OpsApp.PatientInfo.BloodType.ID.ToString();
            ld_PrePay = OpsApp.PatientInfo.FT.PrepayCost;
            ls_SickRoom = OpsApp.PatientInfo.PVisit.PatientLocation.Room;
            //--------------------------------------------------------
            #endregion
            DateTime ldt_ReceptDate = DateTime.MinValue;//接患者时间
            OpsApp.ExeDept = OpsApp.OperateRoom;//执行科室(对于需要填申请单的手术来说，手术室即执行科室)
            string strGerm = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsGermCarrying).ToString();
            string strFinished = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsFinished).ToString();
            string strAnesth = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsAnesth).ToString();
            string strUrgent = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsUrgent).ToString();
            string strChange = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsChange).ToString();
            string strHeavy = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsHeavy).ToString();
            string strSpecial = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsSpecial).ToString();
            string strValid = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsValid).ToString();
            string strUnite = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsUnite).ToString();
            string strNeedAcco = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsAccoNurse).ToString();
            string strNeedPrep = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsPrepNurse).ToString();
            //默认取第一个诊断为统计术前诊断
            string strDiagnose = string.Empty;
            if (OpsApp.DiagnoseAl.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase MainDiagnose in OpsApp.DiagnoseAl)
                {
                    if (MainDiagnose.IsValid)
                    {
                        strDiagnose = MainDiagnose.Name + "(" + MainDiagnose.ID.ToString() + ")";
                        break;
                    }
                }
            }
            if (this.Sql.GetSql("Operator.Operator.UpdateApplication.1", ref strSql) == -1) return -1;
            try
            {
                string[] str = new string[]{
											   OpsApp.ID,ls_ClinicCode,ls_PatientNo,OpsApp.PatientSouce,ls_Name,
											   ls_Sex,ldt_Birthday.ToString(),ld_PrePay.ToString(),ls_DeptCode,ls_BedNo,
											   ls_BloodCode,strDiagnose,OpsApp.OperationDoctor.ID.ToString(),OpsApp.GuideDoctor.ID.ToString(),ls_SickRoom,
											   OpsApp.PreDate.ToString(),OpsApp.Duration.ToString(),OpsApp.AnesType.ID.ToString(),OpsApp.HelperAl.Count.ToString(),"0",
											   "0","0",OpsApp.ExeDept.ID.ToString(),OpsApp.TableType,OpsApp.ApplyDoctor.ID.ToString(),
											   OpsApp.ApplyDoctor.Dept.ID.ToString(),OpsApp.ApplyDate.ToString(),OpsApp.ApplyNote,OpsApp.ApproveDoctor.ID,OpsApp.ApproveDate.ToString(),
											   OpsApp.ApproveNote,"",OpsApp.OperationType.ID.ToString(),OpsApp.InciType.ID.ToString(),strGerm,
											   OpsApp.ScreenUp,OpsApp.OpsTable.ID.ToString(),ldt_ReceptDate.ToString(),OpsApp.BloodType.ID.ToString(),OpsApp.BloodNum.ToString(),
											   OpsApp.BloodUnit,OpsApp.OpsNote,OpsApp.AneNote,OpsApp.ExecStatus,strFinished,
											   strAnesth,OpsApp.Folk,OpsApp.RelaCode.ID.ToString(),OpsApp.FolkComment,strUrgent,
											   strChange,strHeavy,strSpecial,OpsApp.User.ID.ToString(),this.GetSysDateTime(),
											   strValid,strUnite,"",OpsApp.OperateKind,strNeedAcco,strNeedPrep,OpsApp.RoomID,OpsApp.OperationDoctor.Dept.ID,
                                               /*{B9DDCC10-3380-4212-99E5-BB909643F11B}*/OpsApp.AnesWay,
                                               //{F0B32D1F-99B6-4b1a-8393-C1F89B98543B}
                                               OpsApp.Position,OpsApp.Eneity,OpsApp.LastTime,OpsApp.IsOlation,
                                               OpsApp.Memo, //add by zhy
                                               OpsApp.SPECIAL2,OpsApp.FROZEN,OpsApp.FROZENDATE.ToString()
										   };
                //每行5个参数
                //				strSql = string.Format(strSql,OpsApp.OperationNo,ls_ClinicCode,ls_PatientNo,OpsApp.Pasource,ls_Name,
                //					ls_Sex,ldt_Birthday.ToString(),ld_PrePay.ToString(),ls_DeptCode,ls_BedNo,
                //					ls_BloodCode,strDiagnose,OpsApp.Ops_docd.ID.ToString(),OpsApp.Gui_docd.ID.ToString(),ls_SickRoom,
                //					OpsApp.Pre_Date.ToString(),OpsApp.Duration.ToString(),OpsApp.Anes_type.ID.ToString(),OpsApp.HelperAl.Count,0,
                //					0,0,OpsApp.ExeDept.ID.ToString(),OpsApp.TableType,OpsApp.Apply_Doct.ID.ToString(),
                //					OpsApp.Apply_Doct.Dept.ID.ToString(),OpsApp.Apply_Date.ToString(),OpsApp.ApplyNote,OpsApp.ApprDocd.ID.ToString(),OpsApp.ApprDate.ToString(),
                //					OpsApp.ApprNote,"",OpsApp.OperateType.ID.ToString(),OpsApp.InciType.ID.ToString(),strGerm,
                //					OpsApp.ScreenUp,OpsApp.OpsTable.ID.ToString(),ldt_ReceptDate.ToString(),OpsApp.BloodType.ID.ToString(),OpsApp.BloodNum.ToString(),
                //					OpsApp.BloodUnit,OpsApp.OpsNote,OpsApp.AneNote,OpsApp.ExecStatus,strFinished,
                //					strAnesth,OpsApp.Folk,OpsApp.RelaCode.ID.ToString(),OpsApp.FolkComment,strUrgent,
                //					strChange,strHeavy,strSpecial,OpsApp.User.ID.ToString(),this.GetSysDateTime(),
                //					strValid,strUnite,"",OpsApp.OperateKind,strNeedAcco,strNeedPrep,OpsApp.RoomID);	

                if (this.ExecNoQuery(strSql, str) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.UpdateApplication";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }


            #region 处理手术项目基本信息
            if (DelOperationInfo(OpsApp) == -1) return -1;
            //针对本申请单中涉及到的手术添加手术项目信息
            foreach (Neusoft.HISFC.Models.Operation.OperationInfo OperateInfo in OpsApp.OperationInfos)
            {
                //添加手术项目信息
                if (AddOperationInfo(OpsApp, OperateInfo) == -1) return -1;
            }
            #endregion
            #region 处理手术诊断信息
            //获得患者已有的所有术前诊断
            ArrayList al = this.GetIcdFromApp(OpsApp);
            //判断是否存在记录的标志
            bool bIsExist = false;
            //遍历要加入的诊断信息列表(OpsApp.DiagnoseAl)
            foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase willAddDiagnose in OpsApp.DiagnoseAl)
            {
                bIsExist = false;
                //遍历患者已有的所有手术诊断，如果willAddDiagnose已经存在，更新其状态，
                //如果willAddDiagnose尚不存在，则新增该记录到数据库中
                foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase thisDiagnose in al)
                {
                    if (thisDiagnose.HappenNo == willAddDiagnose.HappenNo && thisDiagnose.Patient.ID.ToString() == willAddDiagnose.Patient.ID.ToString())
                    {
                        //已经存在	更新	
                        //TODO:病案业务层
                        //if(this.DiagnoseManager.UpdatePatientDiagnose(willAddDiagnose) == -1) return -1;
                        bIsExist = true;
                    }
                }
                //遍历完毕后发现不存在 新增
                if (bIsExist == false)
                {
                    //TODO:病案业务层
                    //if(this.DiagnoseManager.CreatePatientDiagnose(willAddDiagnose) == -1) return -1;
                }
            }
            #endregion
            #region 处理手术人员角色信息
            try
            {
                if (this.ProcessRoleForApply(OpsApp) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.UpdateApplication Role";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            #endregion
            return 1;
        }

        /// <summary>
        /// 手术申请(修改手术申请单)
        /// </summary>
        /// <param name="OpsApp">手术申请单实例</param>
        /// <returns>0 success -1 fail</returns>
        public int UpdateApplication(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            #region 修改手术申请单
            ///修改手术申请单
            ///Operation.Operation.UpdateApplication.1
            ///传入：58
            ///传出：0 
            #endregion

            string strSql = string.Empty;
            #region 获取患者基本信息
            //--------------------------------------------------------		
            //局部变量定义
            string ls_ClinicCode = string.Empty;//住院流水号/门诊号
            string ls_PatientNo = string.Empty; //病案号/病历号
            string ls_Name = string.Empty;	  //患者姓名
            string ls_Sex = string.Empty;		  //性别
            DateTime ldt_Birthday = DateTime.MinValue; //生日
            string ls_DeptCode = string.Empty;  //住院科室
            string ls_BedNo = string.Empty;	  //床位号
            string ls_BloodCode = string.Empty; //患者血型
            decimal ld_PrePay = 0;	  //预交金
            string ls_SickRoom = string.Empty;  //病房号

            #region 首先判断是门诊手术还是住院手术，确定应该输入的患者号
            switch (OpsApp.PatientSouce)
            {
                case "1":  //门诊手术
                    ls_PatientNo = OpsApp.PatientInfo.PID.CardNO;
                    break;
                case "2":  //住院手术
                    ls_PatientNo = OpsApp.PatientInfo.PID.PatientNO;
                    break;
            }
            #endregion
            ls_ClinicCode = OpsApp.PatientInfo.ID.ToString();
            //ls_PatientNo = OpsApp.PatientInfo.Patient.PID.RecordNo;
            ls_Name = OpsApp.PatientInfo.Name;
            ls_Sex = OpsApp.PatientInfo.Sex.ID.ToString();
            ldt_Birthday = OpsApp.PatientInfo.Birthday;
            ls_DeptCode = OpsApp.PatientInfo.PVisit.PatientLocation.Dept.ID.ToString();
            ls_BedNo = OpsApp.PatientInfo.PVisit.PatientLocation.Bed.ID.ToString();
            ls_BloodCode = OpsApp.PatientInfo.BloodType.ID.ToString();
            ld_PrePay = OpsApp.PatientInfo.FT.PrepayCost;
            ls_SickRoom = OpsApp.PatientInfo.PVisit.PatientLocation.Room;
            //--------------------------------------------------------
            #endregion
            DateTime ldt_ReceptDate = DateTime.MinValue;//接患者时间
            OpsApp.ExeDept = OpsApp.OperateRoom;//执行科室(对于需要填申请单的手术来说，手术室即执行科室)
            string strGerm = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsGermCarrying).ToString();
            string strFinished = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsFinished).ToString();
            string strAnesth = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsAnesth).ToString();
            string strUrgent = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsUrgent).ToString();
            string strChange = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsChange).ToString();
            string strHeavy = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsHeavy).ToString();
            string strSpecial = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsSpecial).ToString();
            string strValid = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsValid).ToString();
            string strUnite = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsUnite).ToString();
            string strNeedAcco = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsAccoNurse).ToString();
            string strNeedPrep = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsPrepNurse).ToString();
            //默认取第一个诊断为统计术前诊断
            string strDiagnose = string.Empty;
            if (OpsApp.DiagnoseAl.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase MainDiagnose in OpsApp.DiagnoseAl)
                {
                    if (MainDiagnose.IsValid)
                    {
                        strDiagnose = MainDiagnose.Name + "(" + MainDiagnose.ID.ToString() + ")";
                        break;
                    }
                }
            }
            if (this.Sql.GetSql("Operator.Operator.UpdateApplication.1", ref strSql) == -1) return -1;
            try
            {
                string[] str = new string[]{
											   OpsApp.ID,ls_ClinicCode,ls_PatientNo,OpsApp.PatientSouce,ls_Name,
											   ls_Sex,ldt_Birthday.ToString(),ld_PrePay.ToString(),ls_DeptCode,ls_BedNo,
											   ls_BloodCode,strDiagnose,OpsApp.OperationDoctor.ID.ToString(),OpsApp.GuideDoctor.ID.ToString(),ls_SickRoom,
											   OpsApp.PreDate.ToString(),OpsApp.Duration.ToString(),OpsApp.AnesType.ID.ToString(),OpsApp.HelperAl.Count.ToString(),"0",
											   "0","0",OpsApp.ExeDept.ID.ToString(),OpsApp.TableType,OpsApp.ApplyDoctor.ID.ToString(),
											   OpsApp.ApplyDoctor.Dept.ID.ToString(),OpsApp.ApplyDate.ToString(),OpsApp.ApplyNote,OpsApp.ApproveDoctor.ID,OpsApp.ApproveDate.ToString(),
											   OpsApp.ApproveNote,"",OpsApp.OperationType.ID.ToString(),OpsApp.InciType.ID.ToString(),strGerm,
											   OpsApp.ScreenUp,OpsApp.OpsTable.ID.ToString(),ldt_ReceptDate.ToString(),OpsApp.BloodType.ID.ToString(),OpsApp.BloodNum.ToString(),
											   OpsApp.BloodUnit,OpsApp.OpsNote,OpsApp.AneNote,OpsApp.ExecStatus,strFinished,
											   strAnesth,OpsApp.Folk,OpsApp.RelaCode.ID.ToString(),OpsApp.FolkComment,strUrgent,
											   strChange,strHeavy,strSpecial,OpsApp.User.ID.ToString(),this.GetSysDateTime(),
											   strValid,strUnite,"",OpsApp.OperateKind,strNeedAcco,
                                               strNeedPrep,OpsApp.RoomID,OpsApp.OperationDoctor.Dept.ID,OpsApp.AnesWay,OpsApp.Position,
                                               OpsApp.Eneity,OpsApp.LastTime,OpsApp.IsOlation,OpsApp.IsPlanToICU,OpsApp.SPECIAL2,
                                               OpsApp.FROZEN,OpsApp.FROZENDATE.ToString() 
										   };

                //每行5个参数
                //				strSql = string.Format(strSql,OpsApp.OperationNo,ls_ClinicCode,ls_PatientNo,OpsApp.Pasource,ls_Name,
                //					ls_Sex,ldt_Birthday.ToString(),ld_PrePay.ToString(),ls_DeptCode,ls_BedNo,
                //					ls_BloodCode,strDiagnose,OpsApp.Ops_docd.ID.ToString(),OpsApp.Gui_docd.ID.ToString(),ls_SickRoom,
                //					OpsApp.Pre_Date.ToString(),OpsApp.Duration.ToString(),OpsApp.Anes_type.ID.ToString(),OpsApp.HelperAl.Count,0,
                //					0,0,OpsApp.ExeDept.ID.ToString(),OpsApp.TableType,OpsApp.Apply_Doct.ID.ToString(),
                //					OpsApp.Apply_Doct.Dept.ID.ToString(),OpsApp.Apply_Date.ToString(),OpsApp.ApplyNote,OpsApp.ApprDocd.ID.ToString(),OpsApp.ApprDate.ToString(),
                //					OpsApp.ApprNote,"",OpsApp.OperateType.ID.ToString(),OpsApp.InciType.ID.ToString(),strGerm,
                //					OpsApp.ScreenUp,OpsApp.OpsTable.ID.ToString(),ldt_ReceptDate.ToString(),OpsApp.BloodType.ID.ToString(),OpsApp.BloodNum.ToString(),
                //					OpsApp.BloodUnit,OpsApp.OpsNote,OpsApp.AneNote,OpsApp.ExecStatus,strFinished,
                //					strAnesth,OpsApp.Folk,OpsApp.RelaCode.ID.ToString(),OpsApp.FolkComment,strUrgent,
                //					strChange,strHeavy,strSpecial,OpsApp.User.ID.ToString(),this.GetSysDateTime(),
                //					strValid,strUnite,"",OpsApp.OperateKind,strNeedAcco,strNeedPrep,OpsApp.RoomID);	

                if (this.ExecNoQuery(strSql, str) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.UpdateApplication";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }


            #region 处理手术项目基本信息
            if (DelOperationInfo(OpsApp) == -1) return -1;
            //针对本申请单中涉及到的手术添加手术项目信息
            foreach (Neusoft.HISFC.Models.Operation.OperationInfo OperateInfo in OpsApp.OperationInfos)
            {
                //添加手术项目信息
                if (AddOperationInfo(OpsApp, OperateInfo) == -1) return -1;
            }
            #endregion
            #region 处理手术诊断信息
            //获得患者已有的所有术前诊断
            ArrayList al = this.GetIcdFromApp(OpsApp);
            //判断是否存在记录的标志
            bool bIsExist = false;
            //遍历要加入的诊断信息列表(OpsApp.DiagnoseAl)
            foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase willAddDiagnose in OpsApp.DiagnoseAl)
            {
                bIsExist = false;
                //遍历患者已有的所有手术诊断，如果willAddDiagnose已经存在，更新其状态，
                //如果willAddDiagnose尚不存在，则新增该记录到数据库中
                foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase thisDiagnose in al)
                {
                    if (thisDiagnose.HappenNo == willAddDiagnose.HappenNo && thisDiagnose.Patient.ID.ToString() == willAddDiagnose.Patient.ID.ToString())
                    {
                        //已经存在	更新	
                        //TODO:病案业务层
                        //if(this.DiagnoseManager.UpdatePatientDiagnose(willAddDiagnose) == -1) return -1;
                        bIsExist = true;
                    }
                }
                //遍历完毕后发现不存在 新增
                if (bIsExist == false)
                {
                    //TODO:病案业务层
                    //if(this.DiagnoseManager.CreatePatientDiagnose(willAddDiagnose) == -1) return -1;
                }
            }
            #endregion
            #region 处理手术人员角色信息
            try
            {
                if (this.ProcessRoleForApply(OpsApp) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.UpdateApplication Role";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            #endregion
            #region 处理手术资料信息
            try
            {
                if (this.ProcessAppaRecForApply(OpsApp) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.UpdateApplication Apparatus";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            #endregion
            return 0;
        }
        /// <summary>
        /// 手术取消
        /// </summary>
        /// <param name="OpsApplication">手术申请单实例</param>
        /// <returns>0 success -1 fail</returns>
        public int CancelApplication(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            #region 取消手术申请单
            ///取消手术申请单
            ///Operation.Operation.CancelApplication.1
            ///传入：4
            ///传出：0 
            #endregion
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.UpdateApplication.2", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OpsApp.ID, OpsApp.User.ID.ToString(), this.GetSysDateTime(), "0");
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.CancelApplication";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            if (strSql == null) return -1;
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 申请取消手术
        /// </summary>
        /// <param name="OpsApp"></param>
        /// <param name="cancelReason"></param>
        /// <returns></returns>
        public int InsertCancelApply(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp, string cancelReason)
        {
            //申请取消手术
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.InsertCancelApply", ref strSql) == -1)
            {
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, OpsApp.ID, cancelReason, this.Operator.ID, this.GetDateTimeFromSysDateTime());
            }
            catch (Exception ex)
            {
                this.Err = "Operator.Operator.InsertCancelApply";
                this.WriteErr();
                return -1;
            }
            if (strSql == null) { return -1; }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 根据手术申请号查找手术申请是否作废
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Operation.OperationAppllication QueryCancelApplyByAppID(string appId)
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.QueryCancelApply", ref strSql) == -1)
            {
                this.Err = "Operator.Operator.QueryCancelApply";
                return null;
            }
            Neusoft.HISFC.Models.Operation.OperationAppllication application = new OperationAppllication();
            try
            {
                strSql = string.Format(strSql, appId);
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    application = new OperationAppllication();
                    application.ID = this.Reader[0].ToString();
                    application.Memo = this.Reader[1].ToString();
                    application.OperationDoctor.ID = this.Reader[2].ToString();
                    application.PreDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[3].ToString());
                }
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return application;
        }

        /// <summary>
        /// 根据申请科室和拟手术日期获取手术台序情况
        /// </summary>
        /// <param name="deptCode"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetOrderByApplyDeptAndTime(string deptCode, DateTime dateTime)
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetOrderByApplyDeptAndTime", ref strSql) == -1)
            {
                return string.Empty;
            }
            strSql = string.Format(strSql, deptCode, dateTime);
            return this.ExecSqlReturnOne(strSql, string.Empty);
        }


        /// <summary>
        /// 手术登记后更新申请单状态
        /// </summary>
        /// <param name="OperatorNo">手术申请单序号</param>
        /// <returns>0 success -1 fail</returns>
        public int DoOperatorRecord(string OperatorNo)
        {
            //已做手术置为1，手术状态置为4
            string strSql = string.Empty;

            if (this.Sql.GetSql("Operator.Operator.UpdateApplication.3", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OperatorNo);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.DoOperatorRecord";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            if (strSql == null) return -1;
            return this.ExecNoQuery(strSql);
        }


        /// <summary>
        /// 麻醉登记后更新申请单状态
        /// </summary>
        /// <param name="OperatorNo"></param>
        /// <returns></returns>
        public int DoAnaeRecord(string OperatorNo)
        {
            string strSql = string.Empty;

            if (this.Sql.GetSql("Operator.Operator.UpdateApplication.4", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OperatorNo);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            if (strSql == null) return -1;
            return this.ExecNoQuery(strSql);
        }

        /// <summary>//{80D89813-7B64-4acf-A2CD-55BFD9F1E7C6}
        /// 麻醉登记后更新申请单状态
        /// </summary>
        /// <param name="OperatorNo"></param>
        /// <returns></returns>
        public int DoAnaeRecord(string OperatorNo, string status)
        {
            string strSql = string.Empty;

            if (this.Sql.GetSql("Operator.Operator.UpdateApplication.Status", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OperatorNo, status);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            if (strSql == null) return -1;
            return this.ExecNoQuery(strSql);
        }
        #endregion
        #region 手术申请业务规则判断
        /// <summary>
        /// 获得指定日期科室在指定手术室剩余可申请的正台数
        /// </summary>
        /// <param name="OpsRoom">手术室对象</param>
        /// <param name="DeptID">手术申请科室编号</param>
        /// <param name="PreDate">手术预约日期</param>
        /// <returns> >=0 可申请的正台数， -1 fail </returns>
        public int GetEnableTableNum(Neusoft.HISFC.Models.Base.Department operationRoom, string deptID, DateTime preDate)
        {
            int iTotalNum = 0;	//总数
            int iAlreadyNum = 0;//已申请数
            int iEnableNum = 0;	//尚可申请数

            string strWeekDay = string.Empty;//星期几
            switch (preDate.DayOfWeek)
            {
                case System.DayOfWeek.Monday:
                    strWeekDay = "1";
                    break;
                case System.DayOfWeek.Tuesday:
                    strWeekDay = "2";
                    break;
                case System.DayOfWeek.Wednesday:
                    strWeekDay = "3";
                    break;
                case System.DayOfWeek.Thursday:
                    strWeekDay = "4";
                    break;
                case System.DayOfWeek.Friday:
                    strWeekDay = "5";
                    break;
                case System.DayOfWeek.Saturday:
                    strWeekDay = "6";
                    break;
                case System.DayOfWeek.Sunday:
                    strWeekDay = "7";
                    break;
            }
            //根据科室和手术室以及星期几来获得原始分配的正台数
            string strSql1 = string.Empty;
            if (this.Sql.GetSql("Operator.OpsTableAlloc.GetAllotInfo.3", ref strSql1) == -1) return -1;
            try
            {
                strSql1 = string.Format(strSql1, operationRoom.ID.ToString(), deptID, strWeekDay);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            if (strSql1 == null) return -1;
            this.ExecQuery(strSql1);
            try
            {
                while (this.Reader.Read())
                {
                    iTotalNum = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[0].ToString());//获得原分配的总数
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            this.Reader.Close();
            //再获取已经申请的正台数
            string strSql2 = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetAlreadyNum.1", ref strSql2) == -1) return -1;
            try
            {
                string strBegin, strEnd;
                strBegin = preDate.Date.ToString();
                strEnd = preDate.Date.AddDays(1).ToString();
                strSql2 = string.Format(strSql2, strBegin, strEnd, operationRoom.ID.ToString(), deptID);

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            if (strSql2 == null) return -1;
            this.ExecQuery(strSql2);
            try
            {
                while (this.Reader.Read())
                {
                    iAlreadyNum = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[0].ToString());//获得已经申请掉的正台数
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            this.Reader.Close();
            //尚可申请的正台数
            iEnableNum = iTotalNum - iAlreadyNum;
            return iEnableNum;
        }
        /// <summary>
        /// 手术预约日期时效性判断
        /// </summary>
        /// <param name="PreDate">手术预约日期</param>
        /// <returns>Error:系统值未维护或格式非法，Before:预约时间小于现在，Over:不能申请该日的正台，OK:可以申请</returns>
        public string PreDateValidity(DateTime PreDate)
        {
            //规则：如果预约日期是明天，则判断申请时间是否过了手术申请截至时间，返回相应值
            //如果小于现在时间，返回相应值
            DateTime dtNow = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.GetSysDateTime());
            DateTime dtTomorrow = dtNow.AddDays(1).Date;
            DateTime dtLimited = DateTime.MinValue;
            if (PreDate < dtNow)//小于现在
            { return "Before"; }
            else if (PreDate.Date.Date == dtNow.Date)//当日预约
            {
                //超过截至时间是否允许手术申请
                string allowApply = GetControlArgument("100031");
                if (allowApply == "Error") return "Error";

                if (allowApply.Trim() == "1")//允许
                {
                    return "OK";
                }
                else//不允许
                {
                    return "Over";
                }
            }
            else if (PreDate.Date == dtTomorrow) //预约日期是明天
            {
                //日手术申请截至时间
                string strTimeLimited = GetControlArgument("optime");
                if (strTimeLimited == "Error" || strTimeLimited == string.Empty) return "Error";
                //判断strTimeLimited是否是有效的时间格式
                try
                {
                    string Today = dtNow.Year.ToString() + "-" + dtNow.Month.ToString() + "-" + dtNow.Day.ToString();
                    string TodayTime = Today + " " + strTimeLimited;
                    dtLimited = Neusoft.FrameWork.Function.NConvert.ToDateTime(TodayTime);
                }
                catch
                {
                    this.Err = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                    this.ErrCode = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                    this.WriteErr();
                    return "Error";
                }
                if (dtNow > dtLimited)
                {
                    string allowApply = GetControlArgument("100031");
                    if (allowApply == "Error") return "Error";
                    if (allowApply.Trim() == "1")
                    {
                        return "OK";
                    }
                    else
                    {
                        return "Over";
                    }
                }
            }
            return "OK";
        }

        /// <summary>
        /// {603DF5A1-C4E8-FB59-9C51-3F6BC040DD7C} 开到中心手术区的择期都需要做对时间特殊判断
        /// </summary>
        /// <param name="PreDate"></param>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public int JudgeOperationDateValid(DateTime PreDate, int tableType, ref string errMsg)
        {
            DateTime dtNow = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.GetSysDateTime());
            DateTime dtTomorrow = dtNow.AddDays(1).Date;
            DateTime dtLimited = DateTime.MinValue;
            DateTime dtBegin = DateTime.MinValue;

            int curdatename = (int)dtNow.DayOfWeek;//星期几
            int seledateName = (int)PreDate.DayOfWeek;//星期几
            TimeSpan ts = PreDate.Date - dtNow.Date;
            int Differ = ts.Days;
            if (PreDate < dtNow)//小于现在
            {
                errMsg = "拟手术日期不能小于当前时间！";
                return -1;
            }

            if (curdatename == 0) //周天一律不给订单（除非是法定的工作日，比如日历上规定的要加班的日子）
            {
                errMsg = "中心手术区周日不允许提交手术申请，除非是法定工作日！";
                return -1;
            }
            if (seledateName == 1) //周一的手术，截止递单时间是周六17点前
            {
                if (curdatename == 6)
                {
                    if (dtNow.Date.Hour > 17)
                    {
                        errMsg = "周一的手术，截止递单时间是周六17点前！";
                        return -1;
                    }
                } 
                //else
                //{
                //    errMsg = "周一的手术，截止递单时间是周六17点前！";
                //    return -1;
                //}
            }
            //周二~周五的手术，术前一日11点前截止递单，加台手术可延至15点
            if (seledateName == 2 || seledateName == 3 || seledateName == 4 || seledateName == 5)
            {
                if (Differ >= 2)
                {
                    errMsg = "周二至周五的手术，术前一日11点前截止递单，加台手术可延至15点";
                    return -1;
                }
                else
                {
                    if (tableType == 2)//代表加台
                    {
                        if (dtNow.Date.Hour >= 15)
                        {
                            errMsg = "周二至周五的手术，术前一日11点前截止递单，加台手术可延至15点";
                            return -1;
                        }
                    }
                    else
                    {
                        if (dtNow.Date.Hour >= 11)
                        {
                            errMsg = "周二至周五的手术，术前一日11点前截止递单，加台手术可延至15点";
                            return -1;
                        }
                    }
                }


            }
            return 1;

        }

        /// <summary>
        /// 手术预约日期时效性判断
        /// </summary>
        /// <param name="PreDate">手术预约日期</param>
        /// <param name="tableType">台类型</param>
        /// <param name="isWeekend">是否周末</param>
        /// <returns>Error:系统值未维护或格式非法，Before:预约时间小于现在，Over:不能申请该日的正台，OK:可以申请</returns>
        public string PreDateValidity(DateTime PreDate, int tableType, bool isWeekend)
        {
            //规则：如果预约日期是明天，则判断申请时间是否过了手术申请截至时间，返回相应值
            //如果小于现在时间，返回相应值
            DateTime dtNow = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.GetSysDateTime());
            DateTime dtTomorrow = dtNow.AddDays(1).Date;
            DateTime dtLimited = DateTime.MinValue;
            DateTime dtBegin = DateTime.MinValue;
            string allowApply = GetControlArgument("100031");

            int curdatename = (int)dtNow.DayOfWeek;//星期几
            int seledateName = (int)PreDate.DayOfWeek;//星期几
            TimeSpan ts = PreDate.Date - dtNow.Date;
            int Differ = ts.Days;

            if (PreDate < dtNow)//小于现在
            { return "Before"; }
            //加台且周六日申请周一手术
            //else if (tableType == 2 && isWeekend && PreDate.DayOfWeek == System.DayOfWeek.Monday)
            //{
            //    return "OK";
            //}
            else if (isWeekend)
            {
                //周日
                if (curdatename == 0 && Differ < 2)
                {
                    this.Err = "系统手术申请截至时间" + dtLimited.ToString();
                    this.ErrCode = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                    this.WriteErr();
                    return "Over";
                }

                //日手术申请截至时间
                string strTimeLimited = GetControlArgument("vatime");
                if (strTimeLimited == "Error" || strTimeLimited == string.Empty) return "Error";
                //判断strTimeLimited是否是有效的时间格式
                try
                {
                    string Today = dtNow.Year.ToString() + "-" + dtNow.Month.ToString() + "-" + dtNow.Day.ToString();
                    string TodayTime = Today + " " + strTimeLimited;
                    dtLimited = Neusoft.FrameWork.Function.NConvert.ToDateTime(TodayTime);
                }
                catch
                {
                    this.Err = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                    this.ErrCode = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                    this.WriteErr();
                    return "Over";
                }

                //周六
                if (curdatename == 6 && Differ < 2)
                {
                    if (dtNow > dtLimited)
                    {
                        this.Err = "系统手术申请截至时间" + dtLimited.ToString();
                        this.ErrCode = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                        this.WriteErr();
                        return "Over";
                    }
                }
                return "OK";
            }
            else if (PreDate.Date.Date == dtNow.Date)//当日预约
            {
                //超过截至时间是否允许手术申请
                //string allowApply = GetControlArgument("100031");
                if (allowApply == "Error") return "Error";

                if (allowApply.Trim() == "1")//允许
                {
                    return "OK";
                }
                else//不允许
                {
                    return "Over";
                }
            }
            else if (PreDate.Date == dtTomorrow) //预约日期是明天
            {
                if (tableType != 2)
                {
                    //日手术申请截至时间
                    string strTimeLimited = GetControlArgument("optime");
                    if (strTimeLimited == "Error" || strTimeLimited == string.Empty) return "Error";
                    //判断strTimeLimited是否是有效的时间格式
                    try
                    {
                        string Today = dtNow.Year.ToString() + "-" + dtNow.Month.ToString() + "-" + dtNow.Day.ToString();
                        string TodayTime = Today + " " + strTimeLimited;
                        dtLimited = Neusoft.FrameWork.Function.NConvert.ToDateTime(TodayTime);
                    }
                    catch
                    {
                        this.Err = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                        this.ErrCode = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                        this.WriteErr();
                        return "Error";
                    }
                    if (dtNow > dtLimited)
                    {
                        //string allowApply = GetControlArgument("100031");
                        if (allowApply == "Error") return "Error";
                        if (allowApply.Trim() == "1")
                        {
                            return "OK";
                        }
                        else
                        {
                            return "Over";
                        }
                    }
                }
                else
                {
                    //加台手术申请起止时间
                    string addTableBeginTime = GetControlArgument("obtime");
                    if (addTableBeginTime == "Error" || addTableBeginTime == string.Empty) return "Error";
                    string addTableEndTime = GetControlArgument("oetime");
                    if (addTableEndTime == "Error" || addTableEndTime == string.Empty) return "Error";
                    //判断strTimeLimited是否是有效的时间格式
                    try
                    {
                        string beginToday = dtNow.Year.ToString() + "-" + dtNow.Month.ToString() + "-" + dtNow.Day.ToString();
                        string beginTodayTime = beginToday + " " + addTableBeginTime;
                        dtBegin = Neusoft.FrameWork.Function.NConvert.ToDateTime(beginTodayTime);
                        string endToday = dtNow.Year.ToString() + "-" + dtNow.Month.ToString() + "-" + dtNow.Day.ToString();
                        string endTodayTime = beginToday + " " + addTableEndTime;
                        dtLimited = Neusoft.FrameWork.Function.NConvert.ToDateTime(endTodayTime);
                    }
                    catch
                    {
                        this.Err = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                        this.ErrCode = "系统手术申请截至时间参数格式非法，请联系系统管理员！";
                        this.WriteErr();
                        return "Error";
                    }
                    if (dtNow < dtBegin)
                    {
                        if (allowApply == "Error") return "Error";
                        if (allowApply.Trim() == "1")
                        {
                            return "OK";
                        }
                        else
                        {
                            return "Over";
                        }
                    }
                    else if (dtNow > dtLimited)
                    {

                        if (allowApply == "Error") return "Error";
                        if (allowApply.Trim() == "1")
                        {
                            return "OK";
                        }
                        else
                        {
                            return "Over";
                        }
                    }
                    else
                    {
                        return "OK";
                    }
                }
            }
            return "OK";
        }

        /// <summary>
        /// 获取系统设置
        /// </summary>
        /// <returns>截至时间字符串，若为Error,则系统参数未设置</returns>
        private string GetControlArgument(string ctlID)
        {
            string strSql = string.Empty;
            string ctlValue = string.Empty;

            if (this.Sql.GetSql("QueryControlerInfo.2", ref strSql) == -1) return string.Empty;
            if (strSql == null) return string.Empty;
            try
            {
                //{8892F821-0519-47a0-A4DF-B1C96EE8E679}取医院编码
                strSql = string.Format(strSql, ctlID, this.Hospital.ID);
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    ctlValue = this.Reader[0].ToString();
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                this.WriteErr();
                if (Reader.IsClosed == false) Reader.Close();
                return "Error";
            }
            this.Reader.Close();

            if (ctlValue == string.Empty)
            {
                this.Err = "系统未维护参数设置，参数代码:" + ctlID + "请联系系统管理员！";
                this.ErrCode = "系统未维护参数设置，参数代码:" + ctlID + "请联系系统管理员！";
                this.WriteErr();
                return "Error";
            }
            return ctlValue;
        }
        #endregion
        #region 手术项目记录信息处理
        /// <summary>
        /// 添加手术记录信息
        /// </summary>
        /// <param name="OpsApp">手术申请单实例</param>
        /// <param name="OperateInfo">手术信息类实例</param>
        /// <returns>0 success -1 fail</returns>
        public int AddOperationInfo(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp, Neusoft.HISFC.Models.Operation.OperationInfo OperateInfo)
        {
            #region 添加手术记录信息
            ///添加手术记录信息
            ///Operation.Operation.AddOperationInfo.1
            ///传入：23
            ///传出：0 
            #endregion
            #region 获取手术项目基本信息
            //----------------------------------------------------
            //局部变量定义 和费用有关的信息
            string ls_ItemCode = string.Empty;	//手术项目编码
            string ls_ItemName = string.Empty;	//手术项目名称
            decimal ld_UnitPrice = 0;	//单价
            decimal ld_FeeRate = 1;		//收费比例
            int li_Qty = 0;			//数量
            string ls_StockUnit = string.Empty;	//单位

            ls_ItemCode = OperateInfo.OperationItem.ID.ToString();
            ls_ItemName = OperateInfo.OperationItem.Name;
            ld_UnitPrice = OperateInfo.OperationItem.Price;
            ld_FeeRate = OperateInfo.FeeRate;
            li_Qty = OperateInfo.Qty;
            ls_StockUnit = OperateInfo.StockUnit;

            //局部变量定义 和手术有关的信息
            string ls_OperateType = string.Empty;	//手术规模
            string ls_InciType = string.Empty;		//切口类型
            string ls_OpePos = string.Empty;		//手术部位

            //ls_OperateType = OpsApp.OperateType.ID.ToString();
            //ls_InciType = OpsApp.InciType.ID.ToString();
            //ls_OpePos = OpsApp.OpePos.ID.ToString();
            ls_OperateType = OperateInfo.OperateType.ID.ToString();
            ls_InciType = OperateInfo.InciType.ID.ToString();
            ls_OpePos = OperateInfo.OpePos.ID.ToString();
            //----------------------------------------------------
            #endregion
            #region 获取患者基本信息
            //--------------------------------------------------------
            //局部变量定义 患者基本信息
            string ls_ClinicCode = string.Empty;//住院流水号/门诊号
            string ls_DeptCode = string.Empty;  //住院科室
            ls_ClinicCode = OpsApp.PatientInfo.ID.ToString();
            /*
            #region 首先判断是门诊手术还是住院手术，确定应该输入的患者号			
            switch (OpsApp.Pasource)
            {
                case "1":  //门诊手术
                    ls_ClinicCode = OpsApp.PatientInfo.Patient.PID.CardNo;
                    break;
                case "2":  //住院手术
                    ls_ClinicCode = OpsApp.PatientInfo.Patient.PID.PatientNo;
                    break;
                default:
                    break;
            }			
            #endregion
            */
            ls_DeptCode = OpsApp.PatientInfo.PVisit.PatientLocation.Dept.ID;
            //--------------------------------------------------------
            #endregion
            string strSql = string.Empty;
            string strGerm = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsGermCarrying).ToString();
            string strUrgent = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsUrgent).ToString();
            string strChange = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsChange).ToString();
            string strHeavy = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsHeavy).ToString();
            string strSpecial = Neusoft.FrameWork.Function.NConvert.ToInt32(OpsApp.IsSpecial).ToString();
            string strValid = Neusoft.FrameWork.Function.NConvert.ToInt32(OperateInfo.IsValid).ToString();
            string strMainFlag = Neusoft.FrameWork.Function.NConvert.ToInt32(OperateInfo.IsMainFlag).ToString();
            if (this.Sql.GetSql("Operator.Operator.AddOperationInfo.1", ref strSql) == -1) return -1;
            try
            {
                string[] str2 = new string[]{
												OpsApp.ID,ls_ClinicCode,ls_DeptCode,ls_ItemCode,ls_ItemName,
												ld_UnitPrice.ToString(),ld_FeeRate.ToString(),li_Qty.ToString(),ls_StockUnit,ls_OperateType,
												ls_InciType,OpsApp.ScreenUp,strGerm,ls_OpePos,strUrgent,
												strChange,strHeavy,strSpecial,strMainFlag,OperateInfo.Remark,
												strValid,OpsApp.User.ID.ToString(),System.DateTime.Now.ToString()
											};
                //在手术记录表中增加记录
                //每行5个参数
                //				strSql = string.Format(strSql,OpsApp.OperationNo,ls_ClinicCode,ls_DeptCode,ls_ItemCode,ls_ItemName,
                //					ld_UnitPrice.ToString(),ld_FeeRate.ToString(),li_Qty.ToString(),ls_StockUnit,ls_OperateType,
                //					ls_InciType,OpsApp.ScreenUp,strGerm,ls_OpePos,strUrgent,
                //					strChange,strHeavy,strSpecial,strMainFlag,OperateInfo.Remark,
                //					strValid,OpsApp.User.ID.ToString(),System.DateTime.Now.ToString());
                return this.ExecNoQuery(strSql, str2);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }

        }
        /// <summary>
        /// 删除手术申请单中手术记录信息
        /// </summary>
        /// <param name="OpsApp">手术申请单实例</param>
        /// <returns>0 success -1 fail</returns>
        public int DelOperationInfo(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            #region 删除手术记录信息
            ///删除手术记录信息
            ///Operation.Operation.DelOperationInfo.1
            ///传入：23
            ///传出：0 
            #endregion

            string strSql = string.Empty;

            if (this.Sql.GetSql("Operator.Operator.DelOperationInfo.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OpsApp.ID);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }
        #endregion
        #region 手术人员角色处理
        /// <summary>
        /// 手术申请单涉及的人员角色处理
        /// </summary>
        /// <param name="OpsApp">手术申请单对象</param>
        /// <returns>0 success,-1 fail</returns>
        public int ProcessRoleForApply(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            //先不管三七二十一清除掉该手术申请单中原有的所有角色安排
            try
            {
                if (DelArrangeRoleAll(OpsApp) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }

            //所有角色(包含了手术医生、指导医生、助手、手术护士、麻醉医生等角色)
            try
            {
                foreach (Neusoft.HISFC.Models.Operation.ArrangeRole thisRole in OpsApp.RoleAl)
                {
                    //{69F783B4-65EB-4cc3-B489-2A7D5B5A5F00}
                    //if(AddArrangeRole(OpsApp,thisRole,thisRole.RoleType.ID.ToString(),thisRole.RoleOperKind.ID.ToString(),thisRole.ForeFlag) == -1) return -1;
                    if (AddArrangeRole(OpsApp, thisRole, thisRole.RoleType.ID.ToString(), thisRole.RoleOperKind.ID.ToString(), thisRole.ForeFlag, thisRole.SupersedeDATE) == -1) return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return 0;
        }
        /// <summary>
        ///  添加手术人员角色
        /// </summary>
        /// <param name="OpsApp">手术申请单对象</param>
        /// <param name="Person">角色人员对象</param>
        /// <param name="strRole">角色编码</param>
        /// <param name="strOperKind">角色状态 正常/接班/直落等</param>
        /// <param name="strForeFlag">角色编码</param>
        /// <returns>0 success, -1 fail</returns>
        /// {69F783B4-65EB-4cc3-B489-2A7D5B5A5F00}
        public int AddArrangeRole(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp, Neusoft.HISFC.Models.Operation.ArrangeRole employee, string strRole, string strOperKind, string strForeFlag, DateTime supersedeDATE)
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.AddArrangeRole.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OpsApp.ID, strRole, employee.ID.ToString(),
                    employee.Name, strForeFlag, OpsApp.User.ID.ToString(), strOperKind, supersedeDATE.ToString());
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 删除手术单中的所有人员角色安排
        /// </summary>
        /// <param name="OpsApp">手术申请单对象实例</param>
        /// <returns>0 success -1 fail</returns>
        public int DelArrangeRoleAll(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.DelArrangeRole.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OpsApp.ID);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 删除手术单中某一角色的人员安排
        /// </summary>
        /// <param name="OpsApp">手术申请单对象</param>
        /// <param name="strRole">角色编码</param>
        /// <returns>0 success -1 fail</returns>
        public int DelArrangeRole(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp, string strRole)
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.DelArrangeRole.2", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OpsApp.ID, strRole);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }

        #endregion
        #region 手术资料处理（仪器设备）
        /// <summary>
        /// 手术申请单涉及的手术资料处理
        /// </summary>
        /// <param name="OpsApp">手术申请单对象</param>
        /// <returns>0 success,-1 fail</returns>
        public int ProcessAppaRecForApply(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            //先清除掉该手术申请单中原有的所有手术资料安排信息
            try
            {
                if (DelAppaRecAll(OpsApp) == -1) return -1;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }

            //所有手术资料安排信息
            try
            {
                foreach (Neusoft.HISFC.Models.Operation.OpsApparatusRec thisRec in OpsApp.AppaRecAl)
                {
                    if (AddAppaRec(thisRec) == -1) return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 增加手术资料记录信息
        /// </summary>
        /// <param name="OpsAppaRec">手术资料记录对象</param>
        /// <returns> 0 success -1 fail</returns>
        public int AddAppaRec(Neusoft.HISFC.Models.Operation.OpsApparatusRec OpsAppaRec)
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.AddAppaRec.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OpsAppaRec.OperationNo, OpsAppaRec.OpsAppa.ID.ToString(),
                    OpsAppaRec.OpsAppa.Name, OpsAppaRec.foreflag.ToString(), OpsAppaRec.Qty.ToString(),
                    OpsAppaRec.AppaUnit, OpsAppaRec.User.ID.ToString());
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 删除某一手术申请单所有手术资料记录信息
        /// </summary>
        /// <param name="OpsApp">手术申请单对象</param>
        /// <param name="strRole">角色编码</param>
        /// <returns>0 success -1 fail</returns>
        public int DelAppaRecAll(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.DelAppaRec.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OpsApp.ID);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }
        #endregion
        /// <summary>
        /// 手术室更换
        /// </summary>
        /// <param name="OpsApplication">手术申请单实例</param>
        /// <returns>0 success -1 fail</returns>
        public int ChangeOperatorRoom(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            #region 手术室更换
            ///更换手术室
            ///Operation.Operation.ChangeOperatorRoom.1
            ///传入：60
            ///传出：0 
            #endregion
            string strSql = string.Empty;

            if (this.Sql.GetSql("Operator.Operator.UpdateApplication.5", ref strSql) == -1) return -1;
            try
            {
                //根据手术序号更新手术日期、手术室(执行科室)、操作人
                strSql = string.Format(strSql, OpsApp.ID, OpsApp.PreDate.ToString(), OpsApp.OperateRoom.ID.ToString(),
                    OpsApp.TableType, this.Operator.ID.ToString());
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.ChangeOperatorRoom";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }

            if (strSql == null)
                return -1;

            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 置手术收费标志
        /// </summary>
        /// <param name="operationNo"></param>
        /// <returns></returns>
        public int UpdateOpsFee(string operationNo)
        {
            string sql = string.Empty;

            if (this.Sql.GetSql("Operator.Operator.UpdateFee", ref sql) == -1)
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
                this.Err = "置手术收费标志出错[Operator.Operator.UpdateFee]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        /// <summary>
        /// 判断患者手术是否已收费: 1已收费、0未收费、2没有手术、-1出错
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        public int IsFee(string inpatientNo)
        {
            string sql = string.Empty;

            if (this.Sql.GetSql("Operator.Operator.QueryIsFee", ref sql) == -1)
            {
                return -1;
            }

            try
            {
                sql = string.Format(sql, inpatientNo);

                if (this.ExecQuery(sql) == -1)
                {
                    return -1;
                }

                //没有进行手术
                while (this.Reader.Read())
                {
                    if (this.Reader[0].ToString() == "0")
                    {
                        this.Reader.Close();
                        return 0;
                    }
                }
                this.Reader.Close();

                return 1;
            }
            catch (Exception e)
            {
                this.Err = "查询患者手术费用时出错!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        /// <summary>
        /// 判断时候手术申请是否已经存在 -1 出错，1 不存在重复申请，2 存在重复申请
        /// </summary>
        /// <param name="InaptientNo"></param>
        /// <param name="Diagnose"></param>
        /// <param name="PreDate"></param>
        /// <returns></returns>
        public int IsExistSameApplication(string InaptientNo, string Diagnose, string PreDate)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.IsExistSameApplication", ref sql) == -1) return -1;
            try
            {
                string[] str = new string[] { InaptientNo, Diagnose, PreDate };
                //sql=string.Format(sql,InaptientNo,Diagnose,PreDate);
                if (this.ExecQuery(sql, str) == -1)
                {
                    return -1;
                }

                //存在重复的手术申请 需要提示医生(该SQL语句)
                if (this.Reader.Read())
                {
                    decimal dQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[0].ToString());
                    if (dQty > 0)
                    {
                        this.Reader.Close();
                        return 2;  //查询出来相同的手术申请记录：存在重复手术
                    }
                    else
                    {
                        this.Reader.Close();
                        return 1;
                    }
                }
                else
                { //没有进行手术
                    this.Reader.Close();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }
        /// <summary>
        /// 某个科室某天的手术申请的台序是否存在重复 ，1 不存在重复申请，2 存在重复申请
        /// </summary>
        /// <param name="InaptientNo"></param>
        /// <param name="Diagnose"></param>
        /// <param name="PreDate"></param>
        /// <returns></returns>
        public int IsExistSameApplicationTableSeq(string strBegin, string strEnd, string ExeDept, string TableType)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.IsExistSameApplicationTableSeq", ref sql) == -1)
            {
                return -1;
            }

            try
            {
                sql = string.Format(sql, ExeDept, TableType, strBegin, strEnd);
                if (this.ExecQuery(sql) == -1)
                {
                    return -1;
                }

                //没有进行手术
                if (this.Reader.Read())
                {
                    this.Reader.Close();
                    return 1;
                }
                else
                { //存在重复的手术申请 需要提示医生
                    this.Reader.Close();
                    return 2;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }
        /// <summary>
        /// 判断时候手术申请是否应该是正台  1 已经有人申请正台 2 没有人申请正台   -1 出错
        /// </summary>
        /// <param name="BeginDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="ExeDept">执行科室</param>
        /// <param name="ExeDept">申请科室</param>
        /// <param name="Type">类型</param>
        /// <returns></returns>
        public int SameDeptApplication(string BeginDate, string EndDate, string ExeDept, string AppDept, string Type)
        {
            string sql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.SameDeptApplication", ref sql) == -1)
            {
                return -1;
            }

            try
            {
                sql = string.Format(sql, BeginDate, EndDate, ExeDept, AppDept, Type);
                if (this.ExecQuery(sql) == -1)
                {
                    return -1;
                }

                //没有进行手术 需要提示医生
                if (this.Reader.Read())
                {
                    this.Reader.Close();
                    return 1;
                }
                else
                { //存在手术申请 
                    this.Reader.Close();
                    return 2;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }


        #region 按传入的Sql语句查询患者信息列表--私有
        //患者查询
        /// <summary>
        /// 患者查询-按住院号查
        /// </summary>
        /// <param name="inPatientNO"></param>
        /// <returns>患者信息 PatientInfo</returns>
        public ArrayList QueryPatientInfoBypatientNO(string PatientNO)
        {
            string strSql1 = string.Empty;
            string strSql2 = string.Empty;

            #region 接口说明

            /////RADT.Inpatient.PatientQuery.where.7
            //传入:住院流水号
            //传出：患者信息

            #endregion

            //strSql1 = PatientQuerySelect();
            if (Sql.GetSql("RADT.Inpatient.PatientQuery.Select.1", ref strSql1) == -1)
            {
                return null;
            }
            if (strSql1 == null) return null;

            //if (Sql.GetSql("RADT.Inpatient.PatientQuery.Where.7", ref strSql2) == -1)
            //{
            //    return null;
            //}
            strSql2 = " where   patient_no = '{0}' ";
            try
            {
                strSql1 = strSql1 + " " + string.Format(strSql2, PatientNO);
            }
            catch
            {
                Err = " where   patient_no = '{0}'赋值不匹配！";
                ErrCode = " where   patient_no = '{0}'赋值不匹配！";
                return null;
            }
            ArrayList al = new ArrayList();
            try
            {
                al = myPatientQuery(strSql1);
            }
            catch (Exception ee)
            {
                Err = "没有找到患者信息!" + ee.Message;
                WriteErr();
                return null;
            }
            return al;
        }

        private ArrayList myPatientQuery(string SQLPatient)
        {
            ArrayList al = new ArrayList();
            Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo;
            ProgressBarText = "正在查询患者...";
            ProgressBarValue = 0;

            if (ExecQuery(SQLPatient) == -1)
            {
                return null;
            }
            //取系统时间,用来得到年龄字符串
            DateTime sysDate = GetDateTimeFromSysDateTime();

            try
            {
                while (Reader.Read())
                {
                    PatientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

                    #region "接口说明"

                    //<!-- 0  住院流水号,1 姓名 ,2   住院号   ,3 就诊卡号  ,4  病历号, 5  医疗证号
                    //,6    医疗类别,   7   性别   ,8   身份证号  ,9   拼音     ,10  生日
                    //,11   职业代码     ,12 职业名称,13   工作单位    ,14   工作单位电话      ,15   单位邮编
                    //,16   户口或家庭地址     ,17   家庭电话   ,18   户口或家庭邮编   , 19  籍贯id,20  籍贯name
                    //,21   出生地代码    , 22 出生地名称   ,23   民族id    ,24  民族name    ,25   联系人id
                    //,26   联系人姓名    ,27   联系人电话       ,28   联系人地址     ,29   联系人关系id , 30   联系人关系name
                    //,31   婚姻状况id    ,32  婚姻状况name  ,33   国籍id    , 34 国籍名称
                    //,35   身高           ,36   体重         ,37   血压      ,38   ABO血型
                    //,39   重大疾病标志    ,40   过敏标志            
                    //,41   入院日期      ,42   科室代码   , 43  科室名称  , 44  结算类别id 1-自费  2-保险 3-公费在职 4-公费退休 5-公费高干
                    //,45   结算类别名称   , 46 合同代码   , 47  合同单位名称  , 48  床号
                    //, 49 护理单元代码  , 50  护理单元名称, 51 医师代码(住院), 52 医师姓名(住院)
                    //, 53 医师代码(主治) , 54 医师姓名(主治) , 55 医师代码(主任) , 56 医师姓名(主任)
                    //, 57 医师代码(实习) , 58 医师姓名(实习), 59  护士代码(责任), 60  护士姓名(责任)
                    //, 61  入院情况id  , 62  入院情况name   , 63  入院途径id    , 64  入院途径name      
                    //, 65  入院来源id 1 -门诊 2 -急诊 3 -转科 4 -转院    , 66  入院来源name
                    //, 67  在院状态 住院登记  i-病房接诊 -出院登记 o-出院结算 p-预约出院 n-无费退院
                    //,  68  出院日期(预约)  , 69  出院日期 , 70  是否在ICU 0 no 1 yes,71 icu code,72 icu name
                    //,73 楼 ,74 层,75 屋 
                    //,76 总共金额TotCost ,77 自费金额 OwnCost,	78 自付金额 PayCost,79 公费金额 PubCost
                    //,80 剩余金额 LeftCost,81 优惠金额
                    //,82  预交金额 ，83    费用金额(已结)，84    预交金额(已结) ， 85 结算日期(上次)     
                    //，86 警戒线, 87 转归代号,88 TransferPrepayCost 转入预交金额（未结)  ,89 转入费用金额(未结),90 病床状态91公费日限额超标部分
                    //,92 特注93公费日限额94血滞纳金95住院次数96床位上限97空调上限98门诊诊断99收住医师100生育保险电脑号
                    //-->

                    #endregion

                    try
                    {
                        if (!Reader.IsDBNull(0)) PatientInfo.ID = Reader[0].ToString(); // 住院流水号
                        if (!Reader.IsDBNull(0)) PatientInfo.ID = Reader[0].ToString(); // 住院流水号
                        if (!Reader.IsDBNull(1)) PatientInfo.Name = Reader[1].ToString(); //姓名
                        if (!Reader.IsDBNull(1)) PatientInfo.Name = Reader[1].ToString(); //姓名
                        if (!Reader.IsDBNull(2)) PatientInfo.PID.PatientNO = Reader[2].ToString(); //  住院号						
                        if (!Reader.IsDBNull(3)) PatientInfo.PID.CardNO = Reader[3].ToString(); //就诊卡号
                        if (!Reader.IsDBNull(4)) PatientInfo.PID.CaseNO = Reader[4].ToString(); // 病历号
                        if (!Reader.IsDBNull(5)) PatientInfo.SSN = Reader[5].ToString(); // 医疗证号
                        if (!Reader.IsDBNull(6)) PatientInfo.PVisit.MedicalType.ID = Reader[6].ToString(); //   医疗类别id
                        if (!Reader.IsDBNull(7)) PatientInfo.Sex.ID = Reader[7].ToString(); //  性别
                        if (!Reader.IsDBNull(8)) PatientInfo.IDCard = Reader[8].ToString(); //  身份证号
                        if (!Reader.IsDBNull(9)) PatientInfo.Memo = Reader[9].ToString(); //  拼音
                        if (!Reader.IsDBNull(10)) PatientInfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[10]); // 生日
                        if (!Reader.IsDBNull(11)) PatientInfo.Profession.ID = Reader[11].ToString(); //  职业代码
                        if (!Reader.IsDBNull(12)) PatientInfo.Profession.Name = Reader[12].ToString(); //职业名称
                        if (!Reader.IsDBNull(13)) PatientInfo.CompanyName = Reader[13].ToString(); //  工作单位
                        if (!Reader.IsDBNull(14)) PatientInfo.PhoneBusiness = Reader[14].ToString(); //  工作单位电话
                        if (!Reader.IsDBNull(15)) PatientInfo.User01 = Reader[15].ToString(); //  单位邮编
                        if (!Reader.IsDBNull(16)) PatientInfo.AddressHome = Reader[16].ToString(); //  户口或家庭地址
                        if (!Reader.IsDBNull(17)) PatientInfo.PhoneHome = Reader[17].ToString(); //  家庭电话
                        if (!Reader.IsDBNull(18)) PatientInfo.User02 = Reader[18].ToString(); //  户口或家庭邮编
                        if (!Reader.IsDBNull(20)) PatientInfo.DIST = Reader[20].ToString(); // 籍贯name
                        if (!Reader.IsDBNull(21)) PatientInfo.AreaCode = Reader[21].ToString(); //  出生地代码
                        if (!Reader.IsDBNull(23)) PatientInfo.Nationality.ID = Reader[23].ToString(); //  民族id
                        if (!Reader.IsDBNull(24)) PatientInfo.Nationality.Name = Reader[24].ToString(); // 民族name
                        if (!Reader.IsDBNull(25)) PatientInfo.Kin.ID = Reader[25].ToString(); //  联系人id
                        if (!Reader.IsDBNull(26)) PatientInfo.Kin.Name = Reader[26].ToString(); //  联系人姓名
                        if (!Reader.IsDBNull(27)) PatientInfo.Kin.RelationPhone = Reader[27].ToString(); //  联系人电话
                        if (!Reader.IsDBNull(28)) PatientInfo.Kin.RelationAddress = Reader[28].ToString(); //  联系人地址
                        if (!Reader.IsDBNull(29)) PatientInfo.Kin.Relation.ID = Reader[29].ToString(); //  联系人关系id
                        if (!Reader.IsDBNull(30)) PatientInfo.Kin.Relation.Name = Reader[30].ToString(); //  联系人关系name
                        if (!Reader.IsDBNull(31)) PatientInfo.MaritalStatus.ID = Reader[31].ToString(); //  婚姻状况id
                        if (!Reader.IsDBNull(33)) PatientInfo.Country.ID = Reader[33].ToString(); //  国籍id
                        if (!Reader.IsDBNull(34)) PatientInfo.Country.Name = Reader[34].ToString(); //国籍名称
                        if (!Reader.IsDBNull(35)) PatientInfo.Height = Reader[35].ToString(); //  身高
                        if (!Reader.IsDBNull(36)) PatientInfo.Weight = Reader[36].ToString(); //  体重
                        if (!Reader.IsDBNull(38)) PatientInfo.BloodType.ID = Reader[38].ToString(); //  ABO血型
                        if (!Reader.IsDBNull(39)) PatientInfo.Disease.IsMainDisease = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[39]); //  重大疾病标志
                        if (!Reader.IsDBNull(40)) PatientInfo.Disease.IsAlleray = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[40]); //  过敏标志
                        if (!Reader.IsDBNull(41)) PatientInfo.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[41]); //  入院日期
                        if (!Reader.IsDBNull(42)) PatientInfo.PVisit.PatientLocation.Dept.ID = Reader[42].ToString(); //  科室代码
                        if (!Reader.IsDBNull(43)) PatientInfo.PVisit.PatientLocation.Dept.Name = Reader[43].ToString(); // 科室名称
                        if (!Reader.IsDBNull(44)) PatientInfo.Pact.PayKind.ID = Reader[44].ToString();
                        // 结算类别id 1-自费  2-保险 3-公费在职 4-公费退休 5-公费高干
                        if (!Reader.IsDBNull(45)) PatientInfo.Pact.PayKind.Name = Reader[45].ToString(); //  结算类别名称
                        if (!Reader.IsDBNull(46)) PatientInfo.Pact.ID = Reader[46].ToString(); //合同代码
                        if (!Reader.IsDBNull(47)) PatientInfo.Pact.Name = Reader[47].ToString(); // 合同单位名称
                        if (!Reader.IsDBNull(48)) PatientInfo.PVisit.PatientLocation.Bed.ID = Reader[48].ToString(); // 床号
                        if (!Reader.IsDBNull(48))
                            PatientInfo.PVisit.PatientLocation.Bed.Name = PatientInfo.PVisit.PatientLocation.Bed.ID.Length > 4
                                                                            ? PatientInfo.PVisit.PatientLocation.Bed.ID.Substring(4)
                                                                            : PatientInfo.PVisit.PatientLocation.Bed.ID; // 床号
                        if (!Reader.IsDBNull(90)) PatientInfo.PVisit.PatientLocation.Bed.Status.ID = Reader[90].ToString(); //床位状态
                        if (!Reader.IsDBNull(90)) PatientInfo.PVisit.PatientLocation.Bed.InpatientNO = Reader[0].ToString(); // 住院流水号
                        if (!Reader.IsDBNull(49)) PatientInfo.PVisit.PatientLocation.NurseCell.ID = Reader[49].ToString(); //护理单元代码
                        if (!Reader.IsDBNull(50)) PatientInfo.PVisit.PatientLocation.NurseCell.Name = Reader[50].ToString(); // 护理单元名称
                        if (!Reader.IsDBNull(51)) PatientInfo.PVisit.AdmittingDoctor.ID = Reader[51].ToString(); //医师代码(住院)
                        if (!Reader.IsDBNull(52)) PatientInfo.PVisit.AdmittingDoctor.Name = Reader[52].ToString(); //医师姓名(住院)
                        if (!Reader.IsDBNull(53)) PatientInfo.PVisit.AttendingDoctor.ID = Reader[53].ToString(); //医师代码(主治)
                        if (!Reader.IsDBNull(54)) PatientInfo.PVisit.AttendingDoctor.Name = Reader[54].ToString(); //医师姓名(主治)
                        if (!Reader.IsDBNull(55)) PatientInfo.PVisit.ConsultingDoctor.ID = Reader[55].ToString(); //医师代码(主任)
                        if (!Reader.IsDBNull(56)) PatientInfo.PVisit.ConsultingDoctor.Name = Reader[56].ToString(); //医师姓名(主任)
                        if (!Reader.IsDBNull(57)) PatientInfo.PVisit.TempDoctor.ID = Reader[57].ToString(); //医师代码(实习)
                        if (!Reader.IsDBNull(58)) PatientInfo.PVisit.TempDoctor.Name = Reader[58].ToString(); //医师姓名(实习)
                        if (!Reader.IsDBNull(59)) PatientInfo.PVisit.AdmittingNurse.ID = Reader[59].ToString(); // 护士代码(责任)
                        if (!Reader.IsDBNull(60)) PatientInfo.PVisit.AdmittingNurse.Name = Reader[60].ToString(); // 护士姓名(责任)
                        if (!Reader.IsDBNull(61)) PatientInfo.PVisit.Circs.ID = Reader[61].ToString(); // 入院情况id
                        if (!Reader.IsDBNull(62)) PatientInfo.PVisit.Circs.Name = Reader[62].ToString(); // 入院情况name
                        if (!Reader.IsDBNull(63)) PatientInfo.PVisit.AdmitSource.ID = Reader[63].ToString(); // 入院途径id
                        if (!Reader.IsDBNull(64)) PatientInfo.PVisit.AdmitSource.Name = Reader[64].ToString(); // 入院途径name
                        if (!Reader.IsDBNull(65)) PatientInfo.PVisit.InSource.ID = Reader[65].ToString();
                        // 入院来源id 1 -门诊 2 -急诊 3 -转科 4 -转院
                        if (!Reader.IsDBNull(66)) PatientInfo.PVisit.InSource.Name = Reader[66].ToString(); // 入院来源name
                        if (!Reader.IsDBNull(67)) PatientInfo.PVisit.InState.ID = Reader[67].ToString();
                        // 在院状态 住院登记  i-病房接诊 -出院登记 o-出院结算 p-预约出院 n-无费退院
                        if (!Reader.IsDBNull(68)) PatientInfo.PVisit.PreOutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[68]); // 出院日期(预约)
                        #region {8D72F2C7-624C-41e4-9922-7A5556B9D82E}
                        if (!Reader.IsDBNull(69))
                        {
                            if (Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[69]) < Neusoft.FrameWork.Function.NConvert.ToDateTime("1000-01-01"))
                            {
                                PatientInfo.PVisit.OutTime = DateTime.MinValue;
                            }
                            else//{3D0766DE-A5AA-409f-8A04-C56F4C9D53DA}
                            {
                                PatientInfo.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[69]);
                            }

                        }
                        #endregion
                        if (!Reader.IsDBNull(71)) PatientInfo.PVisit.ICULocation.ID = Reader[71].ToString(); //icu code
                        if (!Reader.IsDBNull(72)) PatientInfo.PVisit.ICULocation.Name = Reader[72].ToString(); //icu name
                        if (!Reader.IsDBNull(73)) PatientInfo.PVisit.PatientLocation.Building = Reader[73].ToString(); //楼
                        if (!Reader.IsDBNull(74)) PatientInfo.PVisit.PatientLocation.Floor = Reader[74].ToString(); //层
                        if (!Reader.IsDBNull(75)) PatientInfo.PVisit.PatientLocation.Room = Reader[75].ToString(); //屋
                        if (!Reader.IsDBNull(76)) PatientInfo.FT.TotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[76]); //总共金额TotCost
                        if (!Reader.IsDBNull(77)) PatientInfo.FT.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[77]); //自费金额 OwnCost
                        if (!Reader.IsDBNull(78)) PatientInfo.FT.PayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[78]); //自付金额 PayCost
                        if (!Reader.IsDBNull(79)) PatientInfo.FT.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[79]); //公费金额 PubCost
                        if (!Reader.IsDBNull(80)) PatientInfo.FT.LeftCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[80]); //剩余金额 LeftCost
                        if (!Reader.IsDBNull(81)) PatientInfo.FT.RebateCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[81]); //优惠金额
                        if (!Reader.IsDBNull(82)) PatientInfo.FT.PrepayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[82]); // 预交金额
                        if (!Reader.IsDBNull(83)) PatientInfo.FT.BalancedCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[83]); //   费用金额(已结)
                        if (!Reader.IsDBNull(84)) PatientInfo.FT.BalancedPrepayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[84]); //   预交金额(已结)
                        if (!Reader.IsDBNull(85))
                        {
                            try
                            {
                                PatientInfo.BalanceDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[85]); //结算时间
                            }
                            catch { }
                        }
                        if (!Reader.IsDBNull(86)) PatientInfo.PVisit.MoneyAlert = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[86]); //警戒线
                        if (!Reader.IsDBNull(87)) PatientInfo.PVisit.ZG.ID = Reader[87].ToString(); //  转归代码
                        if (!Reader.IsDBNull(88)) PatientInfo.FT.TransferPrepayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[88]); // 转入预交金额（未结) 
                        if (!Reader.IsDBNull(89)) PatientInfo.FT.TransferTotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[89]); //  转入费用金额（未结) 
                        if (!Reader.IsDBNull(91)) PatientInfo.FT.OvertopCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[91]); //   公费超标金额
                        if (!Reader.IsDBNull(92)) PatientInfo.Memo = Reader[92].ToString(); //特注
                        if (!Reader.IsDBNull(93)) PatientInfo.FT.DayLimitCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[93]); //   公费日限额
                        if (!Reader.IsDBNull(94)) PatientInfo.FT.BloodLateFeeCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[94]); //  血滞纳金
                        if (!Reader.IsDBNull(95)) PatientInfo.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[95]); //住院次数
                        if (!Reader.IsDBNull(96)) PatientInfo.FT.BedLimitCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[96].ToString()); //床位上限
                        if (!Reader.IsDBNull(97)) PatientInfo.FT.AirLimitCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[97].ToString()); //空调上限
                        if (!Reader.IsDBNull(99)) PatientInfo.DoctorReceiver.ID = Reader[99].ToString();
                        if (!Reader.IsDBNull(98)) PatientInfo.ClinicDiagnose = Reader[98].ToString();
                        if (!Reader.IsDBNull(100)) PatientInfo.ProCreateNO = Reader[100].ToString(); //生育保险电脑号
                        PatientInfo.IsHasBaby = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[101].ToString()); //是否有婴儿
                        PatientInfo.Disease.Tend.Name = Reader[102].ToString(); //护理级别
                        //费用收取间隔
                        PatientInfo.FT.FixFeeInterval = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[103].ToString());
                        //上次收取时间
                        PatientInfo.FT.PreFixFeeDateTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[104].ToString());
                        //床费超标处理 0超标不限1超标自理
                        PatientInfo.FT.BedOverDeal = Reader[105].ToString();
                        //日限额累计
                        PatientInfo.FT.DayLimitTotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[106].ToString());
                        //病案状态: 0 无需病案 1 需要病案 2 医生站形成病案 3 病案室形成病案 4病案封存
                        PatientInfo.CaseState = Reader[107].ToString();

                        PatientInfo.ExtendFlag = Reader[108].ToString(); //是否允许日限额超标 0 不同意 1 同意 中山一院需求
                        PatientInfo.ExtendFlag1 = Reader[109].ToString(); //扩展标记1
                        PatientInfo.ExtendFlag2 = Reader[110].ToString(); //扩展标记2
                        PatientInfo.FT.BoardCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[111]); //膳食花费总额
                        PatientInfo.FT.BoardPrepayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[112]); //膳食预交金额
                        PatientInfo.PVisit.BoardState = Reader[113].ToString(); //膳食结算状态：0在院 1出院
                        PatientInfo.FT.FTRate.OwnRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[114].ToString()); //自费比例
                        PatientInfo.FT.FTRate.PayRate = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[115].ToString()); //自付比例
                        PatientInfo.FT.DrugFeeTotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[116].ToString()); //公费患者公费药品累计(日限额)
                        PatientInfo.MainDiagnose = Reader[117].ToString(); //患者住院主诊断

                        PatientInfo.Age = GetAge(PatientInfo.Birthday, sysDate); //根据出生日期取患者年龄
                        PatientInfo.IsEncrypt = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[118].ToString());
                        PatientInfo.NormalName = Reader[119].ToString();
                        if (!Reader.IsDBNull(120)) PatientInfo.BalanceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[120].ToString());//结算次数
                        //{F0C48258-8EFB-4356-B730-E852EE4888A0}
                        PatientInfo.ExtendFlag1 = Reader[121].ToString();//病情


                        if (PatientInfo.PID.PatientNO.StartsWith("B"))
                        {
                            //是否婴儿
                            PatientInfo.IsBaby = true;
                        }
                        else
                        {
                            PatientInfo.IsBaby = false;
                        }

                        //{2FA0D4CE-E2EB-4bc7-975A-3693B71C62CF}
                        if (!Reader.IsDBNull(122))
                        {
                            PatientInfo.IsStopAcount = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[122].ToString());
                        }
                        PatientInfo.PVisit.AlertType.ID = this.Reader[123].ToString();
                        PatientInfo.PVisit.BeginDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[124]);
                        PatientInfo.PVisit.EndDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[125]);
                    }
                    catch (Exception ex)
                    {
                        Err = ex.Message;
                        WriteErr();
                        if (!Reader.IsClosed)
                        {
                            Reader.Close();
                        }
                        return null;
                    }
                    //获得变更信息

                    #region "获得变更信息"

                    //deleted by cuipeng 2005-5 不知道此功能为啥用,而且有问题
                    //this.myGetTempLocation(PatientInfo);

                    #endregion

                    ProgressBarValue++;
                    al.Add(PatientInfo);
                }
            } //抛出错误
            catch (Exception ex)
            {
                Err = "获得患者基本信息出错！" + ex.Message;
                ErrCode = "-1";
                WriteErr();
                if (!Reader.IsClosed)
                {
                    Reader.Close();
                }
                return al;
            }
            Reader.Close();

            ProgressBarValue = -1;
            return al;
        }


        public ArrayList GetALLOpsApp(string beginTime, string endTime)
        {
            ArrayList myAl = new ArrayList();
            //业务规则：获取所有的手术单信息		
            string strSql = GetOperationSql();
            if (strSql == null)
            {
                return null;
            }

            string strSqlwhere = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.33", ref strSqlwhere) == -1)
            {
                return myAl;
            }

            try
            {
                strSqlwhere = string.Format(strSqlwhere, beginTime, endTime);
            }
            catch (Exception ex)
            {
                this.Err = "Operator.Operator.GetApplication.33";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }
            strSql = strSql + " \n" + strSqlwhere;
            myAl = GetOpsAppListFromSql(strSql);
            return myAl;
        }





        public ArrayList GetOpsAppListforzdly(Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo, string Pasource, string endTime)
        {
            if (PatientInfo == null)
            {
                return null;	//传入的患者对象为空
            }

            ArrayList myAl = new ArrayList();

            //业务规则：遴选出该患者手术时间小于给定时间的所有有效的未做的手术申请单。

            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.GetApplication.99", ref strSql) == -1)
            {
                return myAl;
            }

            try
            {
                switch (Pasource)
                {
                    case "1":
                        //strSql = string.Format(strSql,PatientInfo.Patient.PID.CardNo,Pasource,endTime);
                        strSql = string.Format(strSql, PatientInfo.ID.ToString(), Pasource, endTime);
                        break;
                    case "2":
                        //患者住院流水号赋值			
                        //PatientInfo.Patient.ID = this.GetInPatientNo(PatientInfo.Patient.PID.ID.ToString());
                        //strSql = string.Format(strSql,PatientInfo.Patient.PID.PatientNo,Pasource,endTime);
                        strSql = string.Format(strSql, PatientInfo.ID.ToString(), Pasource, endTime);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operator.Operator.GetOpsAppList 99";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return myAl;
            }

            this.ExecQuery(strSql);
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Operation.OperationAppllication opsApplication = new Neusoft.HISFC.Models.Operation.OperationAppllication();

                    opsApplication.ID = Reader[0].ToString();										//手术序号					
                    try
                    {
                        opsApplication.OperationDoctor.ID = Reader[1].ToString();					//手术医生
                        // by zlw 2006-5-24
                        opsApplication.User01 = Reader[1].ToString();	//手术医生所在科室
                    }
                    catch { }

                    opsApplication.GuideDoctor.ID = Reader[2].ToString();											//指导医生	

                    opsApplication.PreDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[3].ToString());		//手术预约时间					
                    try
                    {
                        opsApplication.Duration = System.Convert.ToDecimal(Reader[4].ToString());					//手术预定用时					
                    }
                    catch { }
                    opsApplication.AnesType.ID = Reader[5].ToString();												//麻醉类型

                    opsApplication.ExeDept.ID = Reader[6].ToString();												//执行科室

                    opsApplication.OperateRoom = opsApplication.ExeDept as Neusoft.HISFC.Models.Base.Department;	//手术室(对于需要填申请单的手术来说，手术室即执行科室)

                    opsApplication.TableType = Reader[7].ToString();					//0正台1加台2点台					

                    opsApplication.ApplyDoctor.ID = Reader[8].ToString();				//申请医生


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
                    try
                    {
                        opsApplication.BloodNum = System.Convert.ToDecimal(Reader[20].ToString());		//血量
                    }
                    catch { }
                    opsApplication.BloodUnit = Reader[21].ToString();					//用血单位
                    opsApplication.OpsNote = Reader[22].ToString();						//手术注意事项
                    opsApplication.AneNote = Reader[23].ToString();						//麻醉注意事项				
                    opsApplication.ExecStatus = Reader[24].ToString();					//1手术申请 2 手术审批 3手术安排 4手术完成


                    string strFinished = Reader[25].ToString();					//0未做手术/1已做手术
                    opsApplication.IsFinished = Neusoft.FrameWork.Function.NConvert.ToBoolean(strFinished);


                    string strAnesth = Reader[26].ToString();					//0未麻醉/1已麻醉
                    opsApplication.IsAnesth = Neusoft.FrameWork.Function.NConvert.ToBoolean(strAnesth);

                    opsApplication.Folk = Reader[27].ToString();						//签字家属
                    opsApplication.RelaCode.ID = Reader[28].ToString();					//家属关系
                    opsApplication.FolkComment = Reader[29].ToString();					//家属意见

                    try
                    {
                        string strUrgent = Reader[30].ToString();					//加急手术,1是/0否
                        opsApplication.IsUrgent = Neusoft.FrameWork.Function.NConvert.ToBoolean(strUrgent);
                    }
                    catch { }
                    try
                    {
                        string strChange = Reader[31].ToString();					//1病危/0否
                        opsApplication.IsChange = Neusoft.FrameWork.Function.NConvert.ToBoolean(strChange);
                    }
                    catch { }
                    try
                    {
                        string strHeavy = Reader[32].ToString();						//1重症/0否
                        opsApplication.IsHeavy = Neusoft.FrameWork.Function.NConvert.ToBoolean(strHeavy);
                    }
                    catch { }
                    try
                    {
                        opsApplication.IsSpecial = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[33].ToString());	//1特殊手术/0否
                    }
                    catch { }

                    opsApplication.User.ID = Reader[34].ToString();	//操作员

                    try
                    {
                        opsApplication.IsUnite = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[35].ToString());//1合并/0否	
                    }
                    catch { }
                    opsApplication.OperateKind = Reader[37].ToString();					//1普通2急诊3感染				
                    try
                    {
                        string strIsNeedAcco = Reader[38].ToString();					//是否需要随台护士
                        opsApplication.IsAccoNurse = Neusoft.FrameWork.Function.NConvert.ToBoolean(strIsNeedAcco);
                    }
                    catch { }
                    try
                    {
                        string strIsNeedPrep = Reader[39].ToString();					//是否需要巡回护士
                        opsApplication.IsPrepNurse = Neusoft.FrameWork.Function.NConvert.ToBoolean(strIsNeedPrep);
                    }
                    catch { }
                    opsApplication.RoomID = Reader[40].ToString();
                    opsApplication.PatientInfo = PatientInfo.Clone();					//患者基本信息					
                    opsApplication.PatientSouce = Pasource;									//1门诊2住院
                    opsApplication.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[41].ToString());//1有效0无效					//1有效0无效	

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
                foreach (Neusoft.HISFC.Models.Operation.OperationAppllication opsApplication in myAl)
                {
                    opsApplication.DiagnoseAl = this.GetIcdFromApp(opsApplication);							//诊断列表					
                    opsApplication.OperationInfos = this.GetOpsInfoFromApp(opsApplication.ID);				//手术项目信息列表				
                    opsApplication.RoleAl = this.GetRoleFromApp(opsApplication.ID);							//人员角色列表
                    //冗余属性赋值，为突出表现层申请部分业务调用方便
                    foreach (Neusoft.HISFC.Models.Operation.ArrangeRole thisRole in opsApplication.RoleAl)
                    {
                        if (thisRole.RoleType.ID.ToString() == Neusoft.HISFC.Models.Operation.EnumOperationRole.Helper1.ToString()
                            || thisRole.RoleType.ID.ToString() == Neusoft.HISFC.Models.Operation.EnumOperationRole.Helper2.ToString()
                            || thisRole.RoleType.ID.ToString() == Neusoft.HISFC.Models.Operation.EnumOperationRole.Helper3.ToString())
                            //助手医师列表
                            opsApplication.HelperAl.Add(thisRole.Clone());
                    }
                    opsApplication.AppaRecAl = GetAppaRecFromApp(opsApplication.ID);//手术资料安排列表
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
        /// 更新确认标记
        /// </summary>
        /// <param name="OpsApp"></param>
        /// <returns></returns>
        public int UpdateSubmitted(Neusoft.HISFC.Models.Operation.OperationAppllication OpsApp)
        {
            string strSql = string.Empty;
            if (this.Sql.GetSql("Operator.Operator.UpdateSubmitted", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, OpsApp.ID);
            }
            catch (Exception ex)
            {
                this.Err = "HISFC.Operation.Operation.UpdateSubmitted";
                this.ErrCode = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        #endregion
        protected abstract Neusoft.HISFC.Models.RADT.PatientInfo GetPatientInfo(string id);
        protected abstract Neusoft.HISFC.Models.Registration.Register GetRegInfo(string id);
        protected abstract string GetEmployeeName(string id);
        public abstract ArrayList GetIcdFromApp(Neusoft.HISFC.Models.Operation.OperationAppllication opsApp);
    }
}
