using System;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Nurse
{
	/// <summary>
	/// 队列信息管理类
	/// </summary>
	public class Queue : Neusoft.FrameWork.Management.Database
    {
        public Queue()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 声明变量
        protected ArrayList al = null;
        protected Neusoft.HISFC.Models.Nurse.Queue queue = null;
        #endregion

        /// <summary>
        /// 获得插入参数列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected string[] myGetParmInsertQueue(Neusoft.HISFC.Models.Nurse.Queue obj)
        {
            string[] strParm ={	
								    obj.Dept.ID,//代码1
									obj.Name,//队列名称2
									obj.Noon.ID,//午别3
									obj.User01,//队列类别4
									obj.Order.ToString(),//顺序5
									obj.IsValid?"1":"0",//是否有效6
									obj.Memo,//备注7
									obj.Oper.ID,//操作员8
									obj.QueueDate.ToString(),//队列时间9
									obj.Doctor.ID,//医生代码10
									obj.ID,//队列号11
									obj.SRoom.ID,//诊室代码12
									obj.SRoom.Name,//诊室名称13
									obj.Console.ID,//诊台代码14
									obj.Console.Name,//诊台名称15
									obj.ExpertFlag,//专家16
								    obj.AssignDept.ID,
									obj.AssignDept.Name
							 };

            return strParm;

        }
        /// <summary>
        /// 获得修改队列参数列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected string[] myGetParmUpdateQueue(Neusoft.HISFC.Models.Nurse.Queue obj)
        {
            string[] strParm ={	
								obj.ID,//队列号
								obj.Dept.ID,//科室代码
								obj.Name,//队列名称
								obj.Noon.ID,//午别
								obj.User01,//队列类别
								obj.Order.ToString(),//顺序
								obj.IsValid?"1":"0",//是否有效
								obj.Memo,//备注
								obj.Oper.ID,//操作员
								obj.QueueDate.ToString(),//操作时间
								obj.Doctor.ID,//医生代码
								obj.SRoom.ID,//诊室代码
								obj.SRoom.Name,//诊室名称
								obj.Console.ID,//诊台
								obj.Console.Name,//诊台名称
								 obj.ExpertFlag,//专家16
								 obj.AssignDept.ID,
								 obj.AssignDept.Name
							 };
            return strParm;
        }

        /// <summary>
        /// 获得处方号
        /// </summary>
        /// <returns></returns>
        public string GetQueueNo()
        {
            return this.GetSequence("Nurse.GetRecipeNo.Select");
        }
        /// <summary>
        /// 插入队列
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertQueue(Neusoft.HISFC.Models.Nurse.Queue obj)
        {
            string strSQL = "";
            //取插入操作的SQL语句
            string[] strParam;
            if (this.Sql.GetCommonSql("Nurse.Queue.InsertQueue", ref strSQL) == -1)
            {
                this.Err = "没有找到字段!";
                return -1;
            }
            try
            {
                if (obj.ID == null) return -1;
                obj.ID = "T" + this.GetQueueNo();
                strParam = this.myGetParmInsertQueue(obj);

            }
            catch (Exception ex)
            {
                this.Err = "格式化SQL语句时出错:" + ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSQL, strParam);


        }

        /// <summary>
        /// 插入叫号队列
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="dept"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public int InsertCallQueue(Neusoft.HISFC.Models.Registration.Register reg,Neusoft.FrameWork.Models.NeuObject dept,
            Neusoft.FrameWork.Models.NeuObject room)
        {
            string strSQL = "";
            //取插入操作的SQL语句
       
            if (this.Sql.GetCommonSql("Nurse.Queue.InsertCallQueue", ref strSQL) == -1)
            {
                this.Err = "没有找到字段!";
                return -1;
            }
            try
            {
                strSQL = String.Format(strSQL,reg.ID,
                reg.Name,
                dept.ID,
                dept.Name,
                room.ID,
                room.Name,
                room.ID,
                room.Name.Substring(0, room.Name.IndexOf("-")),
                this.Operator.ID,"");

            }
            catch (Exception ex)
            {
                this.Err = "格式化SQL语句时出错:" + ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSQL);


        }

        /// <summary>
        /// 获得修改队列参数列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int UpdateQueue(Neusoft.HISFC.Models.Nurse.Queue obj)
        {
            string strSql = "";
            string[] strParam;
            if (this.Sql.GetCommonSql("Nurse.Queue.UpdateQueue", ref strSql) == -1) return -1;
            try
            {
                //获取参数列表
                strParam = this.myGetParmUpdateQueue(obj);
                strSql = string.Format(strSql, strParam);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }

            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 根据排版信息更新护士站队列有效性
        /// </summary>
        /// <param name="validFlag"></param>
        /// <param name="seeDate"></param>
        /// <param name="noonCode"></param>
        /// <param name="deptCode"></param>
        /// <param name="doctCode"></param>
        /// <returns></returns>
        public int UpdateQueueValid(string validFlag, string seeDate, string noonCode, string deptCode, string doctCode)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Schema.UpdateQueueValid", ref sql) == -1)
            {
                sql = @"UPDATE met_nuo_queue   --医师出诊表
                        SET valid_flag='{0}'   --1正常/0停诊
                        where Trunc(queue_date) = to_date('{1}','yyyy:mm:dd') 
                        and noon_code='{2}'
                        and dept_code='{3}'
                        and doct_code='{4}'";
            }
            try
            {
                sql = string.Format(sql, validFlag, seeDate, noonCode, deptCode, doctCode);
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
        /// 删除队列
        /// </summary>
        /// <param name="queueNo"></param>
        /// <returns></returns>
        public int DelQueue(string queueNo)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Nurse.DelQueue.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, queueNo);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 删除叫号队列
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public int DelCallQueue(string clinicCode)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Nurse.DelCallQueue.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, clinicCode);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 按护士站/分诊日期/午别查询分诊队列信息
        /// </summary>
        /// <param name="nurseID"></param>
        /// <param name="queueDate"></param>
        /// <param name="noonID"></param>
        /// <returns></returns>
        public ArrayList Query(string nurseID, DateTime queueDate, string noonID)
        {
            return this.QueryBase("Nurse.Queue.Query.2", nurseID, queueDate.ToString(), noonID);
        }

        /// <summary>
        /// 根据where条件查询队列信息
        /// </summary>
        /// <param name="whereSQL"></param>
        /// <returns></returns>
        public ArrayList QueryBase(string whereSQL)
        {
            string sql_Select = "";

            if (this.Sql.GetCommonSql("Nurse.Queue.Query.1", ref sql_Select) == -1)
            {
                this.Err = "查询分诊队列信息出错![Nurse.Queue.Query.1]";
                return null;
            }

            sql_Select = sql_Select + "\r\n" + whereSQL;

            return this.Query(sql_Select);
        }

        /// <summary>
        /// 根据SQLID查询队列信息
        /// </summary>
        /// <param name="whereIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList QueryBase(string whereIndex, params string[] args)
        {
            string whereSQL = "";

            if (this.Sql.GetCommonSql(whereIndex, ref whereSQL) == -1)
            {
                this.Err = "查询分诊队列信息出错![" + whereIndex + "]";
                return null;
            }

            try
            {
                whereSQL = string.Format(whereSQL, args);
            }
            catch (Exception e)
            {
                this.Err = "查询分诊队列信息出错![" + whereIndex + "]\r\n" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return this.QueryBase(whereSQL);
        }

        /// <summary>
        /// 按护士站/分诊日期/午别/科室 查询分诊队列信息
        /// </summary>
        /// <param name="nurseID"></param>
        /// <param name="queueDate"></param>
        /// <param name="noonID"></param>
        /// <returns></returns>
        public ArrayList Query(string nurseID, DateTime queueDate, string noonID, string deptCode)
        {
            return this.QueryBase("Nurse.Queue.Query.5", nurseID, queueDate.ToString(), noonID, deptCode);
        }
        /// <summary>
        /// 根据序列号去序列信息
        /// </summary>
        /// <param name="queueID">序列号</param> 
        /// <returns></returns>
        public ArrayList QueryByQueueID(string queueID)
        {
            return this.QueryBase("Nurse.Queue.Query.7", queueID);
        }

        /// <summary>
        /// 查询met_nuo_assignrecord中是否有符合条件的诊室是否在用
        /// </summary>
        /// <param name="roomID">诊室代码</param>
        /// <param name="currentDateStr"></param>
        /// <returns></returns>
        public int QueryQueueByQueueID(string roomID, string currentDateStr)
        {
            string strsql = string.Empty;
            if (this.Sql.GetCommonSql("Nurse.Room.GetQueueByQueueID", ref strsql) == -1)
            {
                this.Err = "得到Nurse.Room.GetQueueByQueueID失败";
                return -1;
            }

            try
            {
                strsql = string.Format(strsql, roomID, currentDateStr);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }

            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(strsql));
        }
        /// <summary>
        /// 查询分诊队列信息 queueDate格式为 2013-01-01
        /// </summary>
        /// <param name="nurseID"></param>
        /// <param name="queueDate"></param>
        /// <returns></returns>
        public ArrayList Query(string nurseID, string queueDate)
        {
            string strBegin = queueDate + " " + "00:00:00", strEnd = queueDate + " " + "23:59:59";

            return this.QueryBase("Nurse.Queue.Query.3", nurseID, strBegin, strEnd);
        }

        /// <summary>
        /// 根据sql查询队列信息
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected ArrayList Query(string sql)
        {
            if (this.ExecQuery(sql) == -1)
            {
                this.Err = "基本sql出错!" + sql;
                this.ErrCode = "基本sql出错!" + sql;
                return null;
            }

            this.al = new ArrayList();

            try
            {
                while (this.Reader.Read())
                {
                    this.queue = new Neusoft.HISFC.Models.Nurse.Queue();

                    //所属护士站
                    this.queue.Dept.ID = this.Reader[0].ToString();
                    //队列代码
                    this.queue.ID = this.Reader[1].ToString();
                    //队列名称
                    this.queue.Name = this.Reader[2].ToString();
                    //午别代码
                    this.queue.Noon.ID = this.Reader[3].ToString();

                    //队列类型[2007/03/27]
                    this.queue.User01 = this.Reader[4].ToString();

                    //显示顺序
                    if (!this.Reader.IsDBNull(5))
                        this.queue.Order = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[5].ToString());
                    //是否有效
                    this.queue.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[6].ToString());
                    //备注
                    this.queue.Memo = this.Reader[7].ToString();
                    //操作员
                    this.queue.Oper.ID = this.Reader[8].ToString();
                    //操作时间
                    this.queue.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[9].ToString());
                    //队列日期
                    this.queue.QueueDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[10].ToString());
                    //看诊医生
                    this.queue.Doctor.ID = this.Reader[11].ToString();
                    //诊室
                    this.queue.SRoom.ID = this.Reader[12].ToString();
                    this.queue.SRoom.Name = this.Reader[13].ToString();
                    //诊台
                    this.queue.Console.ID = this.Reader[14].ToString();
                    this.queue.Console.Name = this.Reader[15].ToString();
                    //专家标志
                    this.queue.ExpertFlag = this.Reader[16].ToString();
                    //分诊科室
                    this.queue.AssignDept.ID = this.Reader[17].ToString();
                    this.queue.AssignDept.Name = this.Reader[18].ToString();
                    this.queue.WaitingCount = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[19]);
                    this.al.Add(this.queue);
                }

                this.Reader.Close();
            }
            catch (Exception e)
            {
                if (!this.Reader.IsClosed) this.Reader.Close();
                this.Err = "查询门诊护士站分诊队列信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return this.al;
        }

        /// <summary>
        ///  根据护理站，时间 获取一个最少待诊人数的队列实体
        /// </summary>
        /// <param name="nurseID"></param>
        /// <param name="queueDate"></param>
        /// <param name="noonID"></param>
        /// <returns></returns>
        public ArrayList QueryMinCount(string nurseID, DateTime queueDate, string noonID, string deptCode)
        {
            return this.QueryBase("Nurse.Queue.Query.4", nurseID, queueDate.ToString(), noonID, deptCode);
        }


        /// <summary>
        /// 查询分诊表中队列诊台的 分诊,进诊信息
        /// </summary>
        /// <returns></returns>
        public ArrayList Query(string nurse, Neusoft.HISFC.Models.Nurse.EnuTriageStatus status,
            DateTime dtfrom, DateTime dtto, string noon)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Nurse.Assign.Auto.Query.3", ref sql) == -1)
            {
                this.Err = "查询sql出错,索引为[Nurse.Assign.Auto.Query.3]";
                this.ErrCode = "查询sql出错,索引为[Nurse.Assign.Auto.Query.3]";
                return null;
            }
            try
            {
                sql = string.Format(sql, nurse, status, dtfrom.ToString(), dtto.ToString(), noon);
            }
            catch (Exception e)
            {
                this.Err = "字符转换出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            if (this.ExecQuery(sql) == -1)
            {
                this.Err = "基本sql出错!" + sql;
                this.ErrCode = "基本sql出错!" + sql;
                return null;
            }
            this.al = new ArrayList();

            try
            {
                while (this.Reader.Read())
                {
                    this.queue = new Neusoft.HISFC.Models.Nurse.Queue();
                    this.queue.ID = this.Reader[0].ToString();
                    this.queue.Console.ID = this.Reader[1].ToString();
                    this.queue.WaitingCount = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[2]);
                    this.al.Add(this.queue);
                }

                this.Reader.Close();
            }
            catch (Exception e)
            {
                if (!this.Reader.IsClosed) this.Reader.Close();
                this.Err = "查询队列信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return this.al;
        }
        /// <summary>
        /// 根据诊室诊台午别日期队列日期查询诊台是否已用
        /// </summary>
        /// <param name="consoleCode">诊台</param>
        /// <param name="noonID">午别</param>
        /// <param name="queueDate">队列时间</param>
        /// <returns>false：取sql出错或该诊台已被使用 true ：没有被使用</returns>
        public bool QueryUsed(string consoleCode,string noonID,string queueDate)
        {
            string sqlStr = string.Empty;
            int returnValue = this.Sql.GetCommonSql("Nurse.Room.GetQueueByConsolecodeNoonIdQDate", ref sqlStr);
            if (returnValue < 0)
            {
                this.Err = "查询sql出错,索引为[Nurse.Room.GetQueueByConsolecodeNoonIdQDate]";
                this.ErrCode = "查询sql出错,索引为[Nurse.Room.GetQueueByConsolecodeNoonIdQDate]";
                return false;
            }
            try
            {
                sqlStr = string.Format(sqlStr,consoleCode, noonID, queueDate);
            }
            catch (Exception e)
            {
                
                this.Err = "字符转换出错!" + e.Message;
                this.ErrCode = e.Message;
            }
            int myReturn = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sqlStr));
            if (myReturn > 0)
            {
                
                this.Err = "该诊台已经被使用，请选择其他诊台";

                return false;
            }
            else if (myReturn < 0)
            {
                this.Err = "查询失败";
                return false;
            }
            return true;

        }

        #region addby xuewj 2010-11-2 增加叫号按钮{5A8B39E0-76A8-4e68-AF14-E2E0F45617D1}
        /// <summary>
        /// 根据诊台午别日期队列日期查询队列ID
        /// </summary>
        /// <param name="consoleCode">诊台</param>
        /// <param name="noonID">午别</param>
        /// <param name="queueDate">队列时间</param>
        /// <returns>false：取sql出错或该诊台已被使用 true ：没有被使用</returns>
        public string QueryQueueID(string consoleCode, string noonID, string queueDate)
        {
            string queue = "";
            string sqlStr = string.Empty;
            int returnValue = this.Sql.GetCommonSql("Nurse.Room.GetQueueIDByConsolecodeNoonIdQDate", ref sqlStr);
            if (returnValue < 0)
            {
                this.Err = "查询sql出错,索引为[Nurse.Room.GetQueueIDByConsolecodeNoonIdQDate]";
                this.ErrCode = "查询sql出错,索引为[Nurse.Room.GetRoomByConsolecodeNoonIdQDate]";
                return null;
            }
            try
            {
                sqlStr = string.Format(sqlStr, consoleCode, noonID, queueDate);
                this.ExecQuery(sqlStr);

                int count = 1;
                while (this.Reader.Read())
                {
                    if (count > 1)
                    {
                        this.Err = "该诊台已经被使用，请选择其他诊台";
                        break;
                    }
                    queue = this.Reader[0].ToString();
                    count++;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return queue;
        } 
        #endregion

        /// <summary>
        /// 判断是否有患者
        /// </summary>
        /// <param name="roomID">诊室编号</param>
        /// <param name="seatID">诊台编号</param>
        /// <param name="queueID">队列编号</param>
        /// <param name="noonID">午别编号</param>
        /// <returns>true,有患者</returns>
        public bool ExistPatient(string roomID, string seatID, string queueID, string noonID)
        {
            string strsql = "";
            if (this.Sql.GetCommonSql("Nurse.Queue.Query.6", ref strsql) == -1)
            {
                return true;
            }

            try
            {
                strsql = string.Format(strsql, roomID, seatID, queueID, noonID);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return true;
            }
            try
            {
                this.ExecSqlReturnOne(strsql);
            }
            catch (Exception ex)
            {

                string aaaa = ex.Message;
            }
                    
               
            string retv = this.ExecSqlReturnOne(strsql);

            if (retv == null || retv.Trim().Equals("0"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 根据医生编码、队列日期、午别取分诊队列
        /// </summary>
        /// <param name="doctID">医生ID</param>
        /// <param name="queDate">队列日期</param>
        /// <param name="noonID">午别ID</param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Nurse.Queue GetQueueByDoct(string doctID, DateTime queDate, string noonID)
        {
            string where = "";

            Neusoft.HISFC.Models.Nurse.Queue queue = null;

            if (this.Sql.GetCommonSql("Nurse.Queue.Query.Where.1", ref where) == -1)
            {
                this.Err = "查询分诊队列信息出错![Nurse.Queue.Query.Where.1]";
                if (string.IsNullOrEmpty(where))
                {
                    where = @"
                        where doct_code='{0}'
                        and trunc(queue_date)=trunc(to_date('{1}','yyyy-mm-dd hh24:mi:ss'))
                        and noon_code='{2}'
                        and valid_flag = '1'";
                }
            }

            if (string.IsNullOrEmpty(where))
            {
                where = @"
                        where doct_code='{0}'
                        and trunc(queue_date)=trunc(to_date('{1}','yyyy-mm-dd hh24:mi:ss'))
                        and noon_code='{2}'
                        and valid_flag = '1'";
            }

            try
            {
                where = string.Format(where, doctID, queDate.ToString(), noonID);
            }
            catch (Exception e)
            {
                this.Err = "查询分诊队列信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            //sql = sql + " " + where;

            ArrayList alQueue = this.QueryBase(where);

            if (alQueue != null && alQueue.Count > 0)
            {
                queue = alQueue[0] as Neusoft.HISFC.Models.Nurse.Queue;
            }

            return queue;
        }

        /// <summary>
        /// 检查当天同午别的诊台是否已被其他医生占用
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private bool IsRoomInUse(Neusoft.HISFC.Models.Nurse.Queue queue)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Nurse.AutoTriage.DocorQueue.Query", ref strSql) == -1)
            {
                strSql = @"select tt.doct_code from met_nuo_queue tt
                   where tt.queue_date > to_date(to_char(sysdate, 'yyyy-mm-dd')||' 00:00:00','yyyy-mm-dd hh24:mi:ss')
                     and tt.noon_code = '{0}'
                     and tt.room_id = '{1}'";
            }
            strSql = string.Format(strSql, queue.Noon.ID, queue.SRoom.ID);
            string currentDoctCode = this.ExecSqlReturnOne(strSql);
            return !currentDoctCode.Equals("-1") && !currentDoctCode.Equals(queue.Doctor.ID);
        }


        /// <summary>
        /// 将当前登录的医生更新进门诊队列表，用于新分诊流程
        /// </summary>
        /// <returns>0 诊台被占用  1 队列被正常写入  -1 出错 </returns>
        public int UpdateDoctQueue(Neusoft.HISFC.Models.Nurse.Queue queue)
        {
            return this.IsRoomInUse(queue) ? 0 : this.InsertDoctQueue(queue);
        }

        /// <summary>
        /// 写入门诊医生队列表
        /// </summary>
        /// <param name="queue"></param>
        /// <returns>1 写入正常  -1 写入出错</returns>
        private int InsertDoctQueue(Neusoft.HISFC.Models.Nurse.Queue queue)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Nurse.AutoTriage.DocorQueue.Insert", ref strSql) == -1)
            {
                strSql =
                @"insert into met_nuo_queue
                (
                    Nurse_cell_code,queue_code,queue_name,
                    noon_code,queue_flag,sort_id,
                    valid_flag,oper_code, oper_date,
                    queue_date,doct_code,room_id,
                    room_name,console_code,console_name,
                    expert_flag,waiting_count,dept_code,
                    dept_name
                )
                values
                (
                    '{0}',  --Nurse_cell_code
                    '{1}',	--queue_code
                    '{2}',	--queue_name
                    '{3}',	--noon_code
                    '{4}',	--queue_flag
                    '{5}',	--sort_id
                    '{6}',	--valid_flag
                    '{7}',	--oper_code
                    to_date('{8}','yyyy-mm-dd HH24:mi:ss'),	--oper_date
                    to_date('{9}','yyyy-mm-dd HH24:mi:ss'),	--queue_date
                    '{10}',	--doct_code
                    '{11}',	--room_id
                    '{12}',	--room_name
                    '{13}',	--console_code
                    '{14}',	--console_name
                    '{15}',	--expert_flag
                    '{16}',	--waiting_count
                    '{17}',	--dept_code
                    '{18}'	--dept_name
                )";
            }

            strSql = string.Format(strSql,
                queue.ID,
                "T" + this.GetQueueNo(), //queueCode
                queue.Doctor.Name,//queueName
                queue.Noon.ID,
                "1",  //医生队列标志
                "1",  //序号
                "1",  //有效标志
                queue.Doctor.ID,
                queue.QueueDate.ToString(),
                queue.QueueDate.ToString(),
                queue.Doctor.ID,
                queue.SRoom.ID,
                queue.SRoom.Name,
                queue.Console.ID,
                queue.Console.Name,
                queue.ExpertFlag,
                "0",  //waitingCount初始化为0
                queue.Dept.ID,
                queue.Dept.Name
                );

            if (this.ExecNoQuery(strSql) <= 0)//插入不成功
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 医生退出医生站时自动清除该医生的队列信息
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public int DeleteDoctQueue(Neusoft.HISFC.Models.Nurse.Queue queue)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Nurse.AutoTriage.DocorQueue.Delete", ref strSql) == -1)
            {
                strSql = @"
                    delete from met_nuo_queue t
                    where t.doct_code = '{0}'
	                  and t.console_code = '{1}' --为何ucOutPatientTree的roomID会对应数据库里的consoleID
                      and t.noon_code = '{2}'
	                  and t.queue_date > to_date(to_char(sysdate, 'yyyy-mm-dd')||' 00:00:00','yyyy-mm-dd hh24:mi:ss')
                    ";
            }
            strSql = string.Format(strSql, queue.Doctor.ID, queue.SRoom.ID, queue.Noon.ID);
            return this.ExecNoQuery(strSql);
        }

    }
}
