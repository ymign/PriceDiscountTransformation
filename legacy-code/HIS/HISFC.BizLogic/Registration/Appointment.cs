using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Registration
{
    public class Appointment : Neusoft.FrameWork.Management.Database
    {
        public Appointment()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        private ArrayList al;

        #region 查询
        /// <summary>
        /// 查询预约平台订单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="cardNo"></param>
        /// <param name="cardType"></param>
        /// <returns></returns>
        public ArrayList QueryAppointmentOrder(DateTime startTime, DateTime endTime, string cardNo, string cardType)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.AppointmentOrder.Select", ref sql) == -1)
                return null;
            if (cardType == "01")
            {
                if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where1", ref where) == -1)
                    return null;
            }
            else if (cardType == "02")
            {
                if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where2", ref where) == -1)
                    return null;
            }
            else if (cardType == "03")
            {
                if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where3", ref where) == -1)
                    return null;
            }
            else if (cardType == "04")
            {
                if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where4", ref where) == -1)
                    return null;
            }

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, startTime.ToString(), endTime.ToString(), cardNo);
            }
            catch (Exception e)
            {
                this.Err = "格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return this.QueryBase(sql);
        }

        /// <summary>
        /// 查询预约平台订单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ArrayList QueryAppointmentOrder(DateTime startTime, DateTime endTime)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.AppointmentOrder.Select", ref sql) == -1)
                return null;

            if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where6", ref where) == -1)
                return null;

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, startTime.ToString(), endTime.ToString());
            }
            catch (Exception e)
            {
                this.Err = "格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return this.QueryBase(sql);
        }

        /// <summary>
        /// 查询预约平台订单
        /// </summary>
        /// <param name="schemaId"></param>
        /// <returns></returns>
        public ArrayList QueryAppointmentOrder(string schemaId)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.AppointmentOrder.Select", ref sql) == -1)
                return null;
            if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where5", ref where) == -1)
                return null;

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, schemaId);
            }
            catch (Exception e)
            {
                this.Err = "格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return this.QueryBase(sql);
        }

        /// <summary>
        /// 通过订单号查询预约平台订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.AppointmentOrder QueryAppointmentOrderByOrderId(string orderId)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.AppointmentOrder.Select", ref sql) == -1)
                return null;
            if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where7", ref where) == -1)
                return null;

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, orderId);
            }
            catch (Exception e)
            {
                this.Err = "格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            ArrayList alOrder = this.QueryBase(sql);
            if (alOrder.Count > 0)
            {
                return alOrder[0] as Neusoft.HISFC.Models.Registration.AppointmentOrder;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 通过预约流水号查询预约平台订单
        /// </summary>
        /// <param name="serialNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.AppointmentOrder QueryAppointmentOrderBySerialNO(string serialNO)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.AppointmentOrder.Select", ref sql) == -1)
                return null;
            if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where8", ref where) == -1)
                return null;

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, serialNO);
            }
            catch (Exception e)
            {
                this.Err = "格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            ArrayList alOrder = this.QueryBase(sql);
            if (alOrder.Count > 0)
            {
                return alOrder[0] as Neusoft.HISFC.Models.Registration.AppointmentOrder;
            }
            else
            {
                return null;
            }
        }

        public Neusoft.HISFC.Models.Registration.AppointmentOrder QueryAppointmentOrderByClinicNO(string ClinicNO)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.AppointmentOrder.Select", ref sql) == -1)
                return null;
            if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where9", ref where) == -1)
                return null;

            sql = sql + " " + where;

            try
            {
                sql = string.Format(sql, ClinicNO);
            }
            catch (Exception e)
            {
                this.Err = "格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            ArrayList alOrder = this.QueryBase(sql);
            if (alOrder.Count > 0)
            {
                return alOrder[0] as Neusoft.HISFC.Models.Registration.AppointmentOrder;
            }
            else
            {
                return null;
            }
        }

        ///// <summary>
        ///// 通过专家代码查询预约平台订单
        ///// </summary>
        ///// <param name="doctorId"></param>
        ///// <returns></returns>
        //public ArrayList QueryAppointmentOrder(DateTime startTime, DateTime endTime, string doctorId)
        //{
        //    string sql = "", where = "";

        //    if (this.Sql.GetCommonSql("Registration.AppointmentOrder.Select", ref sql) == -1)
        //        return null;
        //    if (this.Sql.GetCommonSql("Registration.QueryAppointmentOrder.Where9", ref where) == -1)
        //        return null;

        //    sql = sql + " " + where;

        //    try
        //    {
        //        sql = string.Format(sql, startTime.ToString(), endTime.ToString(), doctorId);
        //    }
        //    catch (Exception e)
        //    {
        //        this.Err = "格式不匹配!" + e.Message;
        //        this.ErrCode = e.Message;
        //        return null;
        //    }
        //    return this.QueryBase(sql);
        //}

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private ArrayList QueryBase(string sql)
        {
            if (this.ExecQuery(sql) == -1) return null;

            this.al = new ArrayList();

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Registration.AppointmentOrder appointmentOrder = new Neusoft.HISFC.Models.Registration.AppointmentOrder();
                    appointmentOrder.OrderID = this.Reader[0].ToString();//订单ID
                    appointmentOrder.UserIdCard = this.Reader[1].ToString();//身份证件号码
                    if (!this.Reader.IsDBNull(2))
                        appointmentOrder.UserJKK = this.Reader[2].ToString();//健康卡号码
                    if (!this.Reader.IsDBNull(3))
                        appointmentOrder.UserSMK = this.Reader[3].ToString();//市民卡号码
                    if (!this.Reader.IsDBNull(4))
                        appointmentOrder.UserYBK = this.Reader[4].ToString();//医保卡号码
                    appointmentOrder.UserName = this.Reader[5].ToString();//用户姓名
                    appointmentOrder.UserGender = this.Reader[6].ToString();//用户性别
                    appointmentOrder.UserMobile = this.Reader[7].ToString();//用户电话
                    appointmentOrder.UserBirthday = DateTime.Parse(this.Reader[8].ToString());
                    if (!this.Reader.IsDBNull(9))
                        appointmentOrder.AgentId = this.Reader[9].ToString();//座席工号
                    appointmentOrder.UserChoice = this.Reader[10].ToString();//用户选择
                    appointmentOrder.OrderType = this.Reader[11].ToString();//预约方式
                    appointmentOrder.Dept.ID = this.Reader[12].ToString();
                    appointmentOrder.Dept.Name = this.Reader[13].ToString();
                    appointmentOrder.Doct.ID = this.Reader[14].ToString();
                    appointmentOrder.Doct.Name = this.Reader[15].ToString();
                    appointmentOrder.RegDate = DateTime.Parse(this.Reader[16].ToString());
                    appointmentOrder.TimeFlag = this.Reader[17].ToString();
                    if (!this.Reader.IsDBNull(18))
                        appointmentOrder.StartTime = this.Reader[18].ToString();
                    if (!this.Reader.IsDBNull(19))
                        appointmentOrder.EndTime = this.Reader[19].ToString();
                    appointmentOrder.RegFee = int.Parse(this.Reader[20].ToString());
                    appointmentOrder.TreatFee = int.Parse(this.Reader[21].ToString());
                    appointmentOrder.OrderState = this.Reader[22].ToString();
                    if (!this.Reader.IsDBNull(23))
                        appointmentOrder.SchemaID = this.Reader[23].ToString();
                    if (!this.Reader.IsDBNull(24))
                        appointmentOrder.VisitingNum = this.Reader[24].ToString();
                    if (!this.Reader.IsDBNull(25))
                        appointmentOrder.SerialNO = this.Reader[25].ToString();
                    if (!this.Reader.IsDBNull(26))
                        appointmentOrder.Note = this.Reader[26].ToString();
                    appointmentOrder.OperDate = DateTime.Parse(this.Reader[27].ToString());
                    appointmentOrder.CardNo = this.Reader[28].ToString();
                    if (!this.Reader.IsDBNull(29))
                        appointmentOrder.PayMode = this.Reader[29].ToString();
                    this.al.Add(appointmentOrder);
                }

                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "获取预约平台订单出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return al;
        }

        /// <summary>
        /// 获取预约平台对账汇总
        /// </summary>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        public DataSet GetAppointmentBalance(string Begin, string End)
        {
            DataSet ds = new DataSet();
            try
            {
                string strSql = "";
                if (this.Sql.GetCommonSql("Fee.FeeReport.AppointmentBalanceReport", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return null;
                }
                else
                {
                    strSql = string.Format(strSql, Begin, End);
                }

                this.ExecQuery(strSql, ref ds);
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                return null;
            }
            return ds;
        }

        /// <summary>
        /// 获取预约平台对账明细
        /// </summary>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <param name="PayMode"></param>
        /// <returns></returns>
        public DataSet GetAppointmentBalanceDetail(string Begin, string End, string PayMode)
        {
            DataSet ds = new DataSet();
            try
            {
                string strSql = "";
                if (this.Sql.GetCommonSql("Fee.FeeReport.AppointmentBalanceReportDetail", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return null;
                }
                else
                {
                    strSql = string.Format(strSql, Begin, End, PayMode);
                }

                this.ExecQuery(strSql, ref ds);
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                return null;
            }
            return ds;
        }

        #endregion

        #region 更新
        /// <summary>
        /// 更新预约平台订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderState"></param>
        /// <param name="operID"></param>
        /// <returns></returns>
        public int UpdateAppointmentOrder(string orderId, string orderState, string operID)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.UpdateAppointmentOrder", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, orderId, orderState, operID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.UpdateAppointmentOrder]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 更新预约平台就诊流水号以及订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderState"></param>
        /// <param name="operID"></param>
        /// <param name="visitNUM"></param>
        /// <returns></returns>
        public int UpdateAppointmentOrderSerialNO(string orderId, string orderState, string operID, string visitNUM)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.UpdateAppointmentOrderSerialNO", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, orderId, orderState, operID, visitNUM);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.UpdateAppointmentOrderSerialNO]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 插入预约平台支付记录表
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="returnReason"></param>
        /// <param name="operID"></param>
        /// <param name="refundTime"></param>
        /// <returns></returns>
        public int InsertBookingPayHistory(string orderId, string returnReason, string operID, DateTime refundTime)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.InsertBookingPayHistory", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, orderId, returnReason, operID, refundTime.ToString());
            }
            catch (Exception e)
            {
                this.Err = "[Registration.InsertBookingPayHistory]格式不匹配!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return this.ExecNoQuery(sql);
        }

        #endregion
    }
}
