using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizLogic.MedicalTechnology
{
    public class Appointment : Neusoft.FrameWork.Management.Database
    {
        public Appointment() { }
        /// <summary>
        /// 排班管理类
        /// </summary>
        private Arrange arrangeMgr = new Arrange();
        /// <summary>
        /// 挂号管理类
        /// </summary>
        private HISFC.BizLogic.Registration.Register registerMgr=new HISFC.BizLogic.Registration.Register();
        /// <summary>
        /// 门诊医嘱单管理类
        /// </summary>
        Neusoft.HISFC.BizLogic.Order.OutPatient.Order outPatientMgr = new Neusoft.HISFC.BizLogic.Order.OutPatient.Order();
        /// <summary>
        /// 住院医嘱单管理类
        /// </summary>
        Neusoft.HISFC.BizLogic.Order.Order inPatientMgr = new Neusoft.HISFC.BizLogic.Order.Order();
        #region 增
        public int Insert(Neusoft.HISFC.Models.MedicalTechnology.Appointment app)
        {
            #region 重新获取一下排班信息
            HISFC.Models.MedicalTechnology.Arrange arr = arrangeMgr.GetByID(app.ArrangeID);

            if (arr.IsStop == true)
            {
                this.Err = "预约已停班,请选择另一排班";
                return -1;
            }

            if (app.OrderType == Models.MedicalTechnology.EnumApplyType.Clinic)
            {
                if (arr.BookLimit - arr.BookNum < 1)
                {
                    this.Err = "预约已满额,请选择另一排班";
                    return -1;
                }

                if (arrangeMgr.Increase(arr.ID, true, false) < 0)
                {
                    this.Err = "更新预约限额出错,请联系管理员";
                    return -1;
                }
            }
            else
            {
                if (arr.HostLimit - arr.HostNum < 1)
                {
                    this.Err = "预约已满额,请选择另一排班";
                    return -1;
                }

                if (arrangeMgr.Increase(arr.ID, false, true) < 0)
                {
                    this.Err = "更新预约限额出错,请联系管理员";
                    return -1;
                }
            }
            #endregion

            //统一强制将操作员改为当前登录用户
            app.OperCode = Neusoft.FrameWork.Management.Connection.Operator.ID;
            //重新取一下序号
            app.OrderNo = GetOrderNo(new Models.MedicalTechnology.Arrange() { ID = app.ArrangeID });
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("MedicalTechnology.Appointment.Insert", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql,
                    app.ID,
                    app.CardNo,
                    app.ClinicCode,
                    app.Name,
                    app.Sex,
                    app.SequenceNo,
                    app.ItemCode,
                    app.ItemName,
                    app.MTCode,
                    app.MTName,
                    app.TypeCode,
                    app.TypeName,
                    app.ArrangeID,
                    app.ArrangeDate.ToString("yyyy-MM-dd"),
                    app.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    app.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    (int)app.OrderType,
                    app.OrderNo,
                    app.MedicalHistory,
                    app.ExecDeptCode,
                    app.ExecDeptName,
                    app.ExecDoctCode,
                    app.ExecDoctName,
                    app.DeptCode,
                    app.DeptName,
                    app.DoctCode,
                    app.DoctName,
                    app.ApplyDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    app.OperCode,
                    app.OperDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(app.IsValid),
                    app.Diagnosis,
                    app.CancleAppointment,
                    (int)app.CancleFlag,
                    app.Telephone,
                    app.Age,
                    app.ArrivalPattern,
                    app.BedNo,
                    app.ItemCost
                    );
            }
            catch (System.Exception ex)
            {
                this.Err = "[MedicalTechnology.Appointment.Insert]格式不匹配,请重新登录后再进行操作!" + ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(sql);
        }
        #endregion

        #region 删
        public int Cancle(Neusoft.HISFC.Models.MedicalTechnology.Appointment app,Neusoft.HISFC.Models.Base.CancelTypes type)
        {
            if (app.OrderType == Models.MedicalTechnology.EnumApplyType.Clinic)
            {
                if (arrangeMgr.Reduce(app.ArrangeID, true, false) < 0)
                {
                    this.Err = "更新预约限额出错,请联系管理员";
                    return -1;
                }
            }
            else
            {
                if (arrangeMgr.Reduce(app.ArrangeID, false, true) < 0)
                {
                    this.Err = "更新预约限额出错,请联系管理员";
                    return -1;
                }
            }

            //统一强制将操作员改为当前登录用户
            app.OperCode = Neusoft.FrameWork.Management.Connection.Operator.ID;

            string sql = "";

            if (this.Sql.GetCommonSql("MedicalTechnology.Appointment.Cancle", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql,
                    app.OperCode,
                    app.OperDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(type),
                    app.ID
                    );
            }
            catch (Exception e)
            {
                this.Err = "[MedicalTechnology.Appointment.Cancle]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }
        public int Cancle(string SequenceNo)
        {
            HISFC.Models.MedicalTechnology.Appointment app = GetAppointmentBySequence(SequenceNo);
            if (app == null) return 0;
            return Cancle(app, Neusoft.HISFC.Models.Base.CancelTypes.Canceled);
        }
        #endregion

        #region 改
        #endregion

        #region 查
        /// <summary>
        /// 获取序号
        /// </summary>
        /// <param name="SeeDate"></param>
        /// <returns></returns>
        public string GetOrderNo(HISFC.Models.MedicalTechnology.Arrange arr)
        {
            string sql = string.Empty;
            if (arr.ItemCode == "016")
            {
                if (this.Sql.GetCommonSql("MedicalTechnology.Appointment.OrderNoForCT", ref sql) < 0)
                {
                    throw new Exception("获取序号出错,请联系信息科");
                }
                sql = string.Format(sql, arr.SeeDate.ToString("yyyy-MM-dd"), arr.ItemCode);
            }
            else
            {
                if (this.Sql.GetCommonSql("MedicalTechnology.Appointment.OrderNo", ref sql) < 0)
                {
                    throw new Exception("获取序号出错,请联系信息科");
                }
                sql = string.Format(sql, arr.ID);
            }
            return ExecSqlReturnOne(sql);
        }
        public HISFC.Models.MedicalTechnology.Appointment GetAppointmentBySequence(string SequenceNo)
        {
            List<HISFC.Models.MedicalTechnology.Appointment> list = Query("MedicalTechnology.Appointment.Where.SequenceNo", SequenceNo);
            if (list == null || list.Count < 1)
            {
                return null;
            }
            else return list.First();
        }
        /// <summary>
        /// 根据医生工号查询多少天之内的病人信息
        /// </summary>
        /// <param name="DoctCode"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<HISFC.Models.MedicalTechnology.Appointment> GetDoctHistory(string DoctCode, int day)
        {
            return Query("MedicalTechnology.Appointment.Where.DoctCodeAndDay", day.ToString(), DoctCode);
        }
        /// <summary>
        /// 根据病历号查询多少天之内的历史记录
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<HISFC.Models.MedicalTechnology.Appointment> GetHistory(string CardNo, int day)
        {
            return Query("MedicalTechnology.Appointment.Where.CardNoAndDay", day.ToString(), CardNo);
        }
        /// <summary>
        /// 病历号,姓名为关键字,查询一段时间之内的预约信息
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<HISFC.Models.MedicalTechnology.Appointment> GetHistory(string keys, DateTime beginTime, DateTime endTime)
        {
            return Query("MedicalTechnology.Appointment.Where.KeysAndDays", keys, beginTime.ToString("yyyy-MM-dd"), endTime.ToString("yyyy-MM-dd"));
        }
        /// <summary>
        /// 根据时间和科室代码(全部为ALL)查询预约患者
        /// </summary>
        /// <param name="date"></param>
        /// <param name="MTCode"></param>
        /// <returns></returns>
        public List<HISFC.Models.MedicalTechnology.Appointment> GetAppointmentByDateAndDept(DateTime date,string DeptCode)
        {
            return Query("MedicalTechnology.Appointment.Where.DateAndDept", date.ToString("yyyy-MM-dd"), DeptCode);
        }
        /// <summary>
        /// 根据病历号和门诊号查询历史记录
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<HISFC.Models.MedicalTechnology.Appointment> GetHistoryByCardNoAndClinicNo(string CardNo, string ClinicNo)
        {
            return Query("MedicalTechnology.Appointment.Where.CardNoAndClinicNo", CardNo, ClinicNo);
        }
        /// <summary>
        /// 基础查询
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.MedicalTechnology.Appointment> Query(string whereSql, params object[] args)
        {
            string sql = string.Empty;
            string where = string.Empty;
            string orderBy = string.Empty;
            if (this.Sql.GetCommonSql("MedicalTechnology.Appointment.Query", ref sql) < 0)
            {
                this.Err = "没有找到索引为[MedicalTechnology.Appointment.Query]的SQL";
                return null;
            }
            if (this.Sql.GetCommonSql(whereSql, ref where) < 0)
            {
                this.Err = "没有找到索引为[" + whereSql + "]的SQL";
                return null;
            }
            try
            {
                where = string.Format(where, args);
            }
            catch
            {
                this.Err = "[" + whereSql + "]格式不匹配";
                return null;
            }
            sql += " " + where;

            if (!sql.ToUpper().Contains("ORDER BY "))
                if (this.Sql.GetCommonSql("MedicalTechnology.Appointment.OrderBy", ref orderBy) >= 0)
                    sql += orderBy;

            return this.QueryBase(sql);
        }
        /// <summary>
        /// 基础查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.MedicalTechnology.Appointment> QueryBase(string sql)
        {
            if (this.ExecQuery(sql) == -1) return null;

            List<Neusoft.HISFC.Models.MedicalTechnology.Appointment> AppList = new List<Neusoft.HISFC.Models.MedicalTechnology.Appointment>();

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.MedicalTechnology.Appointment app = new Neusoft.HISFC.Models.MedicalTechnology.Appointment();

                    app.ID = this.Reader[0].ToString();
                    app.CardNo = this.Reader[1].ToString();
                    app.ClinicCode = this.Reader[2].ToString();
                    app.Name = this.Reader[3].ToString();
                    app.Sex = this.Reader[4].ToString();
                    app.SequenceNo = this.Reader[5].ToString();
                    app.ItemCode = this.Reader[6].ToString();
                    app.ItemName = this.Reader[7].ToString();
                    app.MTCode = this.Reader[8].ToString();
                    app.MTName = this.Reader[9].ToString();
                    app.TypeCode = this.Reader[10].ToString();
                    app.TypeName = this.Reader[11].ToString();
                    app.ApplyStatus = (Neusoft.HISFC.Models.MedicalTechnology.EnumApplyStatus)int.Parse(Reader[12].ToString());
                    app.ArrangeID = this.Reader[13].ToString();
                    app.ArrangeDate = DateTime.Parse(this.Reader[14].ToString());
                    app.BeginTime = DateTime.Parse(this.Reader[15].ToString());
                    app.EndTime = DateTime.Parse(this.Reader[16].ToString());
                    app.OrderType = (Neusoft.HISFC.Models.MedicalTechnology.EnumApplyType)int.Parse(this.Reader[17].ToString());
                    app.OrderNo = this.Reader[18].ToString();
                    app.MedicalHistory = this.Reader[19].ToString();
                    app.ExecDeptCode = this.Reader[20].ToString();
                    app.ExecDeptName = this.Reader[21].ToString();
                    app.ExecDoctCode = this.Reader[22].ToString();
                    app.ExecDoctName = this.Reader[23].ToString();
                    app.DeptCode = this.Reader[24].ToString();
                    app.DeptName = this.Reader[25].ToString();
                    app.DoctCode = this.Reader[26].ToString();
                    app.DoctName = this.Reader[27].ToString();
                    app.ApplyDate = DateTime.Parse(this.Reader[28].ToString());
                    app.OperCode = this.Reader[29].ToString();
                    app.OperDate = DateTime.Parse(this.Reader[30].ToString());
                    app.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[31].ToString());
                    app.Diagnosis = this.Reader[32].ToString();
                    app.Telephone = this.Reader[33].ToString();
                    app.CancleAppointment = this.Reader[34].ToString();
                    app.CancleFlag = (Neusoft.HISFC.Models.Base.CancelTypes)int.Parse(this.Reader[35].ToString());
                    app.Age = this.Reader[36].ToString();
                    app.ArrivalPattern = this.Reader[37].ToString();
                    app.BedNo = this.Reader[38].ToString();
                    app.ItemCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[39].ToString());
                    AppList.Add(app);
                }

                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "查询排班信息出错,请重新登录后再进行操作!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return AppList;
        }
        
        #endregion
    }
}
