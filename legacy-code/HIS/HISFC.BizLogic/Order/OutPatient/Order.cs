using System;
using System.Collections;
//{55BBD9DB-F5C9-4e0a-94E5-9F7FCB121350}
using System.Collections.Generic;
using Neusoft.FrameWork.Models;
namespace Neusoft.HISFC.BizLogic.Order.OutPatient
{
    /// <summary>
    /// Order 的摘要说明。
    /// 门诊医嘱
    /// </summary>
    public class Order : Neusoft.FrameWork.Management.Database
    {
        public Order()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }


        #region 基本操作，增删改

        /// <summary>
        /// 插入一条
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int InsertOrder(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            string sql = "Order.OutPatient.Order.Insert";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            sql = this.myGetCommonSql(sql, order);
            if (sql == null) return -1;
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;
        }

        /// <summary>
        /// 插入一条
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int InsertOrderWithHosCode(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            string sql = "Order.OutPatient.Order.Insert.HosCode";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            sql = this.myGetCommonSql(sql, order);
            if (sql == null) return -1;
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateOrder(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            if (this.DeleteOrder(order.SeeNO, Neusoft.FrameWork.Function.NConvert.ToInt32(order.ID)) < 0)
            {
                return -1;//删除不成功
            }
            return this.InsertOrder(order);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateOrderWithHosCode(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            if (this.DeleteOrder(order.SeeNO, Neusoft.FrameWork.Function.NConvert.ToInt32(order.ID)) < 0)
            {
                return -1;//删除不成功
            }
            return this.InsertOrderWithHosCode(order);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="seeNo"></param>
        /// <param name="seqNo"></param>
        /// <returns></returns>
        public int DeleteOrder(string seeNo, int seqNo)
        {
            /*
             * DELETE 
             * FROM met_ord_recipedetail   --诊间处方明细表
                WHERE     see_no='{0}' and sequence_no = {1} 
                AND status = '0'
             * */

            string sql = "Order.OutPatient.Order.Delete";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                sql = string.Format(sql, seeNo, seqNo);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(sql);
        }

        #endregion

        #region 门诊医嘱变更表操作add by sunm

        public int InsertOrderChangeInfo(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            string sql = "Order.OutPatient.Order.InsertChangeInfo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1) return -1;
            sql = this.myGetCommonSql(sql, order);
            if (sql == null) return -1;
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;
        }
        /// <summary>
        /// 更新医嘱变更纪录
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateOrderChangedInfo(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            string sql = "Order.OutPatient.Order.UpdateChangeInfo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1) return -1;
            sql = System.String.Format(sql, order.DCOper.ID, order.SeeNO, order.SequenceNO);
            if (sql == null) return -1;
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;
        }

        /// <summary>
        /// 作废医嘱
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateOrderBeCaceled(Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            string sql = "Order.OutPatient.Order.CancelOrder";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = "Can't Find Sql:Order.OutPatient.Order.CancelOrder";
                return -1;
            }
            sql = System.String.Format(sql, order.ID);
            if (sql == null) return -1;
            return this.ExecNoQuery(sql);
        }

        #endregion

        #region 获得新的看诊序号
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public int GetNewSeeNo(string cardNo)
        {
            string sql = "Order.OutPatient.Order.GetNewSeeNo.1";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                sql = string.Format(sql, cardNo);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
        }
        #endregion

        /// <summary>
        /// 获得新医嘱组合序号
        /// </summary>
        /// <returns></returns>
        public string GetNewOrderComboID()
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Management.Order.GetComboID", ref sql) == -1) return null;
            string strReturn = this.ExecSqlReturnOne(sql);
            if (strReturn == "-1" || strReturn == "") return null;
            return strReturn;
        }

        /// <summary>
        /// 按组合号查询医嘱
        /// 按照数据库的SQL语句来看，目前isSubtbl参数没用了
        /// </summary>
        /// <param name="combno">组合号</param>
        /// <param name="isSubtbl">目前含辅材</param>
        /// <returns></returns>
        public ArrayList QueryOrderByCombNO(string clinicCode, string combno, bool isSubtbl)
        {
            return this.QueryOrderBase("Order.OutOrder.QueryOrderByCombno.where.1", clinicCode, combno, isSubtbl ? "1" : "0");
        }


        /// <summary>
        /// 查询申请单医嘱信息
        /// </summary>
        /// <param name="combno">组合号</param>
        /// <param name="isSubtbl">目前含辅材</param>
        /// <returns></returns>
        public ArrayList QueryApplyOrderByMark(string clinicCode, string sequence, string applyCode, string applyExec)
        {
            return this.QueryOrderBase2("Order.OutOrder.QueryApplyOrderByMark", clinicCode, sequence, applyCode, applyExec);
        }

        #region 更新评估标记（内镜中心流程改造添加）
        /// <summary>
        /// 更新医嘱特殊流程标记
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public int UpdateASSESSS_FLAG(string orderID)
        {
            string sql = "Order.OutPatient.Order.Update.UpdateAssess_FLAG";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1) return -1;
            return this.ExecNoQuery(sql, orderID);
        }        
        #endregion

        #region 更新医嘱已经收费
        /// <summary>
        /// 更新医嘱已经收费
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public int UpdateOrderCharged(string orderID)
        {
            string sql = "Order.OutPatient.Order.Update.UpdateOrderCharged.2";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1) return -1;
            return this.ExecNoQuery(sql, orderID);
        }
        /// <summary>
        /// 更新医嘱已经收费
        /// </summary>
        /// <param name="reciptNo"></param>
        /// <param name="seqNo"></param>
        /// <returns></returns>
        public int UpdateOrderCharged(string reciptNo, string seqNo)
        {
            string sql = "Order.OutPatient.Order.Update.UpdateOrderCharged.1";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            return this.ExecNoQuery(sql, reciptNo, seqNo, this.Operator.ID);
        }
        /// <summary>
        /// 更新医嘱已经收费
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="chargeOperID"></param>
        /// <returns></returns>
        public int UpdateOrderChargedByOrderID(string orderID, string chargeOperID)
        {
            string sql = "Order.OutPatient.Order.Update.UpdateOrderCharged.4";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1) return -1;
            return this.ExecNoQuery(sql, orderID, chargeOperID);
        }
        #endregion

        #region  更新医嘱序号
        /// <summary>
        /// 更新医嘱序号
        /// 增加clinic_code优化查询速率{BE4B33A4-D86A-47da-87EF-1A9923780A5C}
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="sortID"></param>
        /// <returns></returns>
        public int UpdateOrderSortID(string orderID, int sortID, string clinicCode)
        {
            string sql = "Order.OutPatient.Order.Update.UpdateOrderSortID.1";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                sql = string.Format(sql, orderID, sortID, clinicCode);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(sql);
        }

        #endregion

        #region  更新医嘱皮试结果
        /// <summary>
        /// 更新医嘱皮试结果//{26E88889-B2CF-4965-AFD8-6D9BE4519EBF}
        /// </summary>
        /// <param name="sequenceNO"></param>
        /// <returns></returns>
        public int UpdateOrderHyTest(string hytestValue, string sequenceNO)
        {
            string sql = "Order.OutPatient.Order.UpdateHyTest.1";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                sql = string.Format(sql, hytestValue, sequenceNO);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(sql);
        }
        /// <summary>
        /// 更新医嘱皮试结果{55BBD9DB-F5C9-4e0a-94E5-9F7FCB121350}
        /// </summary>
        /// <param name="sequenceNO"></param>
        /// <returns></returns>
        public int UpdateOrderHyTest(string hytestValue, string hytestName, string sequenceNO, string seeNO)
        {
            string sql = "Order.OutPatient.Order.UpdateHyTest.2";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                sql = string.Format(sql, hytestValue, hytestName, sequenceNO, seeNO);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 查询皮试处方信息
        /// </summary>
        /// <param name="cardNO"></param>
        /// <param name="beginDtime"></param>
        /// <param name="endDtime"></param>
        /// <returns></returns>
        public List<Neusoft.FrameWork.Models.NeuObject> QueryHytoRecord(string cardNO, string beginDtime, string endDtime)
        {
            string strSql = string.Empty;

            int returnValue = this.Sql.GetCommonSql("Order.OutPatient.Order.QueryHyRecord", ref strSql);

            if (returnValue < 0)
            {
                this.Err = "查询对应[Order.OutPatient.Order.QueryHyRecord]的sql语句失败";
                return null;
            }

            try
            {
                strSql = string.Format(strSql, cardNO, beginDtime, endDtime);
            }
            catch (Exception ex)
            {

                this.Err = "格式化出错！\n" + ex.Message;
                return null;
            }

            if (this.ExecQuery(strSql) < 0)
            {
                return null;
            }
            List<Neusoft.FrameWork.Models.NeuObject> orderList = new List<Neusoft.FrameWork.Models.NeuObject>();
            while (this.Reader.Read())
            {

                Neusoft.FrameWork.Models.NeuObject order = new Neusoft.FrameWork.Models.NeuObject();
                order.ID = this.Reader[0].ToString();
                order.Name = this.Reader[1].ToString();
                order.Memo = this.Reader[2].ToString();
                orderList.Add(order);
            }

            this.Reader.Close();

            return orderList;


        }

        // 根据病历号，门诊流水号，查询需要做皮试的有效医嘱
        /// <summary>
        /// 根据病历号，门诊流水号，查询需要做皮试的有效医嘱{55BBD9DB-F5C9-4e0a-94E5-9F7FCB121350}
        /// </summary>
        /// <param name="cardNO"></param>
        /// <param name="clinicNO"></param>
        /// <returns></returns>
        public ArrayList QueryOrderByCardNOClinicNO(string cardNO, string clinicNO)
        {
            string sql = "", sqlSelect = "", sqlWhere = "Order.OutPatient.Order.Query.Where.5";
            if (this.myGetSelectSql(ref sqlSelect) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            if (this.Sql.GetCommonSql(sqlWhere, ref sqlWhere) == -1) return null;
            sql = sqlSelect + " " + sqlWhere;
            sql = string.Format(sql, cardNO, clinicNO);
            return this.myGetExecOrder(sql);
        }

        /// <summary>
        /// 根据病历号，门诊流水号查询
        /// </summary>
        /// <param name="cardNO"></param>
        /// <param name="clinicNO"></param>
        /// <returns></returns>
        public ArrayList QueryOrderByCardNOandClinicNO(string cardNO, string clinicNO)
        {
            string sql = "", sqlSelect = "", sqlWhere = "Order.OutPatient.Order.Query.Where.9";
            if (this.myGetSelectSql(ref sqlSelect) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            if (this.Sql.GetCommonSql(sqlWhere, ref sqlWhere) == -1) return null;
            sql = sqlSelect + " " + sqlWhere;
            sql = string.Format(sql, cardNO, clinicNO);
            return this.myGetExecOrder(sql);
        }

        /// <summary>
        /// 根据病历号，门诊流水号查询
        /// </summary>
        /// <param name="cardNO"></param>
        /// <param name="clinicNO"></param>
        /// <returns></returns>
        public ArrayList QueryOrderByTime(string cardNO, string beginTime, string endTime, string itemID)
        {
            string sql = "", sqlSelect = "", sqlWhere = "Order.OutPatient.Order.Query.Where.13";
            if (this.myGetSelectSql(ref sqlSelect) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            if (this.Sql.GetCommonSql(sqlWhere, ref sqlWhere) == -1) return null;
            sql = sqlSelect + " " + sqlWhere;
            sql = string.Format(sql, cardNO, beginTime, endTime, itemID, this.Operator.ID);
            return this.myGetExecOrder(sql);
        }

        /// <summary>
        /// 根据主键查询医嘱
        /// </summary>
        /// <param name="seeNO"></param>
        /// <param name="sqeNO"></param>
        /// <returns></returns>{55BBD9DB-F5C9-4e0a-94E5-9F7FCB121350}
        public ArrayList QueryOrderByKey(string seeNO, string sqeNO)
        {
            string sql = "", sqlSelect = "", sqlWhere = "Order.OutPatient.Order.Query.Where.6";
            if (this.myGetSelectSql(ref sqlSelect) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            if (this.Sql.GetCommonSql(sqlWhere, ref sqlWhere) == -1) return null;
            sql = sqlSelect + " " + sqlWhere;
            sql = string.Format(sql, seeNO, sqeNO);
            return this.myGetExecOrder(sql);
        }
        #endregion

        #region 查询

        /// <summary>
        /// 查询执行医嘱--通过看诊序号查询
        /// </summary>
        /// <param name="seeNo"></param>
        /// <returns></returns>
        public ArrayList QueryOrder(string seeNo)
        {
            string sql = "", sqlSelect = "", sqlWhere = "Order.OutPatient.Order.Query.Where.1";
            if (this.myGetSelectSql(ref sqlSelect) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            if (this.Sql.GetCommonSql(sqlWhere, ref sqlWhere) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            sql = sqlSelect + " " + sqlWhere;
            sql = string.Format(sql, seeNo);
            return this.myGetExecOrder(sql);
        }

        /// <summary>
        /// 查询门诊处方
        /// </summary>
        /// <param name="clinicCode">门诊看诊流水号</param>
        /// <param name="seeNo">看诊序号</param>
        /// <returns></returns>
        public ArrayList QueryOrder(string clinicCode, string seeNo)
        {
            return this.QueryOrderBase("Order.OutPatient.Order.Query.ByClinicCodeSeeNo", clinicCode, seeNo);
        }

        /// <summary>
        /// 根据门诊号查询门诊处方
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public ArrayList QueryOrder(string whereSql, string clinicCode, string seeNO)
        {
            string sqlStr = "";
            if (this.myGetSelectSql(ref sqlStr) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            sqlStr = sqlStr + "\r\n" + whereSql;
            sqlStr = string.Format(sqlStr, clinicCode);
            return this.myGetExecOrder(sqlStr);
        }

        /// <summary>
        /// 根据whereIndex查询门诊处方
        /// </summary>
        /// <param name="SqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList QueryOrderBase(string SqlIndex, params string[] args)
        {
            string sqlStr = "";
            if (this.myGetSelectSql(ref sqlStr) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            if (this.Sql.GetCommonSql(SqlIndex, ref SqlIndex) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            sqlStr = sqlStr + "\r\n" + SqlIndex;

            sqlStr = string.Format(sqlStr, args);

            return this.myGetExecOrder(sqlStr);
        }

        // {CA46F1AD-388F-4a93-86CF-96F0AEE3567B}  同一类型同一执行科室的单放一起
        /// <summary>
        /// 根据whereIndex查询门诊处方
        /// </summary>
        /// <param name="SqlIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList QueryOrderBase2(string SqlIndex, params string[] args)
        {
            string sqlStr = "";
            if (this.myGetSelectSql2(ref sqlStr) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            if (this.Sql.GetCommonSql(SqlIndex, ref SqlIndex) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            sqlStr = sqlStr + "\r\n" + SqlIndex;

            sqlStr = string.Format(sqlStr, args);

            return this.myGetExecOrder(sqlStr);
        }

        /// <summary>
        /// 根据SQL语句查询门诊处方
        /// </summary>
        /// <param name="whereSQL">这里是SQL语句，不是SQLID</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ArrayList QueryOrder(string whereSQL, params string[] args)
        {
            string sqlStr = "";
            if (this.myGetSelectSql(ref sqlStr) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            sqlStr = sqlStr + "\r\n" + whereSQL;

            sqlStr = string.Format(sqlStr, args);

            return this.myGetExecOrder(sqlStr);
        }

        /// <summary>
        /// 根据处方号查询医嘱
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="recipeNO"></param>
        /// <returns></returns>
        public ArrayList QueryOrderByRecipeNO(string clinicCode, string recipeNO)
        {
            return this.QueryOrderBase("Order.OutPatient.Order.Query.Where.4", clinicCode, recipeNO);
        }

        /// <summary>
        /// 根据处方号查询医嘱
        /// </summary>
        /// <param name="recipeNO"></param>
        /// <returns></returns>
        public ArrayList QueryOrderByRecipeNO(string recipeNO)
        {
            return this.QueryOrderBase("Order.OutPatient.Order.Query.ByRecipeNO", recipeNO);
        }

        /// <summary>
        /// 查询一条医嘱
        /// 增加clinic_code优化查询速率{BE4B33A4-D86A-47da-87EF-1A9923780A5C}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.OutPatient.Order QueryOneOrder(string clinicCode, string sequenceNO)
        {
            ArrayList al = this.QueryOrderBase("Order.OutPatient.Order.Query.Where.2", sequenceNO, clinicCode);
            if (al == null)
            {
                return null;
            }
            if (al.Count <= 0)
            {
                return null;
            }
            return al[0] as Neusoft.HISFC.Models.Order.OutPatient.Order;
        }
        /// <summary>
        /// 查询一条医嘱
        /// 增加clinic_code优化查询速率{BE4B33A4-D86A-47da-87EF-1A9923780A5C}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.OutPatient.Order QueryOneOrder(string clinicCode, string sequenceNO, string recipeNO)
        {
            ArrayList al = this.QueryOrderBase("Order.OutPatient.Order.Query.Where.8", sequenceNO, clinicCode, recipeNO);
            if (al == null)
            {
                return null;
            }
            if (al.Count <= 0)
            {
                return null;
            }
            return al[0] as Neusoft.HISFC.Models.Order.OutPatient.Order;
        }
        /// <summary>
        /// 批量查询门诊处方
        /// 增加clinic_code优化查询速率{BE4B33A4-D86A-47da-87EF-1A9923780A5C}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArrayList QueryBatchOrder(string clinicCode, string[] batchSeq)
        {
            string strBatchSeq = "''";
            for (int i = 0; i < batchSeq.Length; i++)
            {
                strBatchSeq += ",'" + batchSeq[i] + "'";
            }

            return this.QueryOrderBase("Order.OutPatient.Order.BatchQuery.ByClinicAndSeq", clinicCode, strBatchSeq);
        }

        /// <summary>
        /// 根据医嘱序号查询一条医嘱
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.OutPatient.Order QueryOneOrder(string sequenceNO)
        {
            ArrayList al = this.QueryOrderBase("Order.OutPatient.Order.Query.Where.7", sequenceNO);
            if (al == null)
            {
                return null;
            }
            if (al.Count <= 0)
            {
                return null;
            }
            return al[0] as Neusoft.HISFC.Models.Order.OutPatient.Order;
        }

        /// <summary>
        /// 获得看诊序号列表
        /// </summary>
        /// <param name="cardNo">门诊卡号</param>
        /// <returns></returns>
        public ArrayList QuerySeeNoListByCardNo(string cardNo)
        {
            string sql = "Order.OutPatient.Order.GetSeeNoList";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                sql = string.Format(sql, cardNo);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(sql) == -1) return null;
            ArrayList al = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();
                obj.Name = this.Reader[1].ToString();
                obj.Memo = this.Reader[2].ToString();
                try
                {
                    obj.User01 = this.Reader[3].ToString();
                    obj.User02 = this.Reader[4].ToString();
                    obj.User03 = this.Reader[5].ToString();
                }
                catch { }
                al.Add(obj);
            }
            this.Reader.Close();
            return al;
        }
        /// <summary>
        /// 获得看诊序号列表
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public ArrayList QuerySeeNoListByCardNo(string clinicNo, string cardNo)
        {
            string sql = "Order.OutPatient.Order.GetSeeNoList.2";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1) return null;
            try
            {
                sql = string.Format(sql, clinicNo, cardNo);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(sql) == -1) return null;
            ArrayList al = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();
                obj.Name = this.Reader[1].ToString();
                obj.Memo = this.Reader[2].ToString();
                try
                {
                    obj.User01 = this.Reader[3].ToString();
                    obj.User02 = this.Reader[4].ToString();
                    if (Reader.FieldCount > 5)
                    {
                        obj.User03 = this.Reader[5].ToString();
                    }
                }
                catch { }
                al.Add(obj);
            }
            this.Reader.Close();
            return al;
        }
        /// <summary>
        /// 查询看诊序号根据名子
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ArrayList QuerySeeNoListByName(string name)
        {
            string sql = "Order.OutPatient.Order.GetSeeNoList.Name";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                sql = string.Format(sql, name);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(sql) == -1) return null;
            ArrayList al = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();
                obj.Name = this.Reader[1].ToString();
                obj.Memo = this.Reader[2].ToString();
                try
                {
                    obj.User01 = this.Reader[3].ToString();
                    obj.User02 = this.Reader[4].ToString();
                    obj.User03 = this.Reader[5].ToString();
                }
                catch { }
                al.Add(obj);
            }
            this.Reader.Close();
            return al;
        }

        /// <summary>
        /// 取得药品处方号通过门诊号和看诊号
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <param name="seeNo"></param>
        /// <returns></returns>
        public ArrayList GetPhaRecipeNoByClinicNoAndSeeNo(string clinicNo, string seeNo)
        {
            string sql = "Order.OutPatient.Order.GetPhaRecipeNoByClinicNoAndSeeNo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                sql = string.Format(sql, clinicNo, seeNo);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(sql) == -1) return null;
            ArrayList alRecipe = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();

                alRecipe.Add(obj);
            }
            this.Reader.Close();
            return alRecipe;
        }

        /// <summary>
        /// 获取处方号通过门诊号和看诊号
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <param name="seeNo"></param>
        /// <param name="flag">0：全部、1：药品、2非药品</param>
        /// <returns></returns>
        public IList<Neusoft.FrameWork.Models.NeuObject> GetRecipeNoByClinicNoAndSeeNo(string clinicNo, string seeNo, string flag)
        {
            string sql = "Order.OutPatient.Order.GetRecipeNoByClinicNoAndSeeNo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                sql = string.Format(sql, clinicNo, seeNo, flag);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(sql) == -1) return null;
            IList<Neusoft.FrameWork.Models.NeuObject> iRecipe = new List<Neusoft.FrameWork.Models.NeuObject>();
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();

                iRecipe.Add(obj);
            }
            this.Reader.Close();
            return iRecipe;
        }



        /// <summary>
        /// 根据发票号获取处方信息
        /// </summary>
        /// <param name="invociceNo"></param>
        /// <returns></returns>
        public ArrayList QueryRecipeListByInvoiceNo(string invociceNo)
        {
            string sql = "Order.OutPatient.Order.QueryRecipeNOByInvoiceNo";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                sql = string.Format(sql, invociceNo);

                if (this.ExecQuery(sql) == -1)
                {
                    return null;
                }

                ArrayList alRecipe = new ArrayList();
                Neusoft.HISFC.Models.Base.Spell obj = null;
                while (this.Reader.Read())
                {
                    obj = new Neusoft.HISFC.Models.Base.Spell();
                    //处方号
                    obj.ID = this.Reader[0].ToString();
                    //医生
                    obj.Name = this.Reader[1].ToString();
                    //操作时间
                    obj.Memo = this.Reader[2].ToString();
                    //卡号
                    obj.SpellCode = this.Reader[3].ToString();
                    //姓名
                    obj.WBCode = this.Reader[4].ToString();
                    //发票号
                    obj.UserCode = this.Reader[5].ToString();

                    alRecipe.Add(obj);
                }
                return alRecipe;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            finally
            {
                if (this.Reader != null && !this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }

        #endregion

        #region 门诊病历

        #region 废弃的方法
        /// <summary>
        /// 根据传入的实体更新或者插入门诊病历
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="casehistory"></param>
        /// <returns></returns>

        //public int SetCaseHistory(Neusoft.HISFC.Models.Registration.Register reg, Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory casehistory)
        //{
        //    int iReturn = this.UpdateCaseHistory(reg, casehistory);
        //    if (iReturn == -1)
        //        return -1;
        //    else if (iReturn == 0)
        //        return this.InsertCaseHistory(reg, casehistory);
        //    else
        //        return 1;
        //}
        #endregion

        /// <summary>
        /// 插入一条病历
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="casehistory"></param>
        /// <returns></returns>
        public int InsertCaseHistory(Neusoft.HISFC.Models.Registration.Register reg, Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory casehistory)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.InsertCase", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = System.String.Format(strSql,
                                              reg.ID, //门诊流水号，需替换
                                              reg.PID.CardNO,
                                              reg.Name, //患者姓名
                                              reg.Sex.Name,
                                              reg.Age,
                                              reg.DoctorInfo.Templet.Dept.ID,
                                              reg.Pact.PayKind.Name,
                                              casehistory.CaseMain,
                                              casehistory.CaseNow,
                                              casehistory.CaseOld,
                                              casehistory.CaseAllery,
                                              casehistory.IsAllery == true ? "1" : "0",
                                              casehistory.IsInfect == true ? "1" : "0",
                                              casehistory.CheckBody,
                                              casehistory.CaseDiag,
                                              casehistory.Memo,
                                              this.Operator.ID, casehistory.CaseOper.OperTime.ToString());
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新一条病历
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="casehistory"></param>
        /// <returns></returns>
        public int UpdateCaseHistory(Neusoft.HISFC.Models.Registration.Register reg, Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory casehistory, string oldOperTime)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.UpdateCase", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                /*
                 UPDATE MET_CAS_HISTORY
                    SET    CASEMAIN = '{0}',--主诉
                           CASENOW = '{1}',--现病史
                           CASEOLD = '{2}',--既往史
                           CASEALLERY = '{3}',--过敏史
                           ALLERY_FLAG = '{4}',--是否过敏
                           INFECT_FLAG = '{5}',--是否传染病
                           CHECKBODY = '{6}',--查体 
                           DIAGNOSE = '{7}',--诊断
                           MEMO = '{8}',--备注
                           OPER_CODE = '{9}',--操作员
                           OPER_DATE = to_date('{10}','YYYY-MM-DD hh24:Mi:SS')--操作日期
                    WHERE  CLINIC_CODE = '{11}'--门诊流水号 
                           and oper_date=to_date('{12}','YYYY-MM-DD hh24:Mi:SS')--操作时
                 */
                strSql = System.String.Format(strSql,
                                              casehistory.CaseMain,
                                              casehistory.CaseNow,
                                              casehistory.CaseOld,
                                              casehistory.CaseAllery,
                                              casehistory.IsAllery == true ? "1" : "0",
                                              casehistory.IsInfect == true ? "1" : "0",
                                              casehistory.CheckBody,
                                              casehistory.CaseDiag,
                                              casehistory.Memo,
                                              this.Operator.ID,
                                              casehistory.CaseOper.OperTime.ToString(),//本次操作时间
                                              reg.ID,
                                              oldOperTime //上一次的操作时间
                                              ); //门诊流水号，需替换
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 根据门诊流水号查询一条门诊病历
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory QueryCaseHistoryByClinicCode(string clinicCode)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.GetCase", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = System.String.Format(strSql, clinicCode);
            ArrayList al = this.GetMyObject(strSql);
            if (al == null)
                return null;
            else if (al.Count == 0)
                return null;
            else
                return al[0] as Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory;
        }

        /// <summary>
        /// 根据门诊流水号和操作时间查询一条门诊病历
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory QueryCaseHistoryByClinicCode(string clinicCode, string operTime)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.GetCase1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = System.String.Format(strSql, clinicCode, operTime);
            ArrayList al = this.GetMyObject(strSql);
            if (al == null)
                return null;
            else if (al.Count == 0)
                return null;
            else
                return al[0] as Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory;
        }

        /// <summary>
        /// 根据门诊号查询门诊所有病历
        /// </summary>
        /// <param name="CardNO"></param>
        /// <returns></returns>
        public ArrayList QueryAllCaseHistory(string CardNO)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.GetAllCase", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = System.String.Format(strSql, CardNO);
            return this.GetMyObjectByCardNO(strSql);
        }

        /// <summary>
        /// 通过门诊号取病历最大操作时间
        /// </summary>
        /// <param name="ClinicCode"></param>
        /// <returns></returns>
        public DateTime QueryMaxOperTimeByClinicCode(string ClinicCode)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.GetMaxOperDateByClinicCode", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return System.DateTime.MinValue;
            }
            strSql = System.String.Format(strSql, ClinicCode);
            string strReturn = "";
            strReturn = this.ExecSqlReturnOne(strSql);
            if (strReturn != "" && strReturn != null)
            {
                return Neusoft.FrameWork.Function.NConvert.ToDateTime(strReturn);
            }
            else
            {
                return System.DateTime.MinValue;
            }
        }

        #region 私有函数

        /// <summary>
        /// 得到病历实体
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private ArrayList GetMyObjectByCardNO(string strSql)
        {
            ArrayList al = new ArrayList();
            if (this.ExecQuery(strSql) == -1) return null;
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();//流水号
                obj.Name = this.Reader[1].ToString();//姓名
                if (!this.Reader.IsDBNull(2))
                    obj.Memo = this.Reader[2].ToString();
                //User01是操作时间 路志鹏 2007-5-9
                obj.User01 = this.Reader[3].ToString();
                al.Add(obj);
            }
            this.Reader.Close();
            return al;
        }

        /// <summary>
        /// 得到病历实体
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private ArrayList GetMyObject(string strSql)
        {
            ArrayList al = new ArrayList();
            if (this.ExecQuery(strSql) == -1) return null;
            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory casehistory = new Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory();
                casehistory.CaseMain = this.Reader.GetValue(0).ToString();//主诉
                casehistory.CaseNow = this.Reader.GetValue(1).ToString();//现病史
                casehistory.CaseOld = this.Reader.GetValue(2).ToString();//既往史
                casehistory.CaseAllery = this.Reader.GetValue(3).ToString();//过敏史
                casehistory.CheckBody = this.Reader.GetValue(4).ToString();//查体
                casehistory.CaseDiag = this.Reader.GetValue(5).ToString();//诊断
                casehistory.Memo = this.Reader.GetValue(6).ToString();//备注
                casehistory.Name = this.Reader.GetValue(7).ToString();//姓名
                casehistory.ID = this.Reader.GetValue(8).ToString();//门诊流水号
                if (!this.Reader.IsDBNull(9))
                    casehistory.IsAllery = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader.GetValue(9).ToString());//是否过敏
                if (!this.Reader.IsDBNull(10))
                    casehistory.IsInfect = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader.GetValue(10).ToString());//是否传染病
                //操作时间
                casehistory.CaseOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader.GetValue(11));
                al.Add(casehistory);
            }
            this.Reader.Close();
            return al;
        }

        #endregion

        #endregion

        #region 门诊病历模板

        /// <summary>
        /// 获取病历模板流水号
        /// </summary>
        /// <returns></returns>
        public string GetModuleSeq()
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.GetModuleSeq", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return "";
            }
            if (this.ExecQuery(strSql) == -1)
            {
                this.Err = "执行错误";
                return "";
            }
            string ID = "";
            while (this.Reader.Read())
            {
                ID = this.Reader[0].ToString();
            }
            this.Reader.Close();
            ID = ID.PadLeft(10, '0');
            return ID;
        }

        /// <summary>
        /// 根据传入的实体更新或者插入门诊病历模板
        /// </summary>
        /// <param name="casehistory"></param>
        /// <returns></returns>
        public int SetCaseModule(Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory casehistory)
        {
            int i = this.UpdateCaseModule(casehistory);
            if (i == -1)
                return -1;
            else if (i == 0)
                return this.InsertCaseModule(casehistory);
            else
                return 1;
        }

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <param name="casehistory"></param>
        /// <returns></returns>
        public int InsertCaseModule(Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory casehistory)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.InsertModule", ref strSql) == -1)
            {
                this.Err = "没有找到Order.OutPatient.Case.InsertModule字段";
                return -1;
            }
            try
            {
                strSql = System.String.Format(strSql,
                                              casehistory.ID,
                                              casehistory.Name,
                                              casehistory.DeptID,
                                              casehistory.CaseMain,
                                              casehistory.CaseNow,
                                              casehistory.CaseOld,
                                              casehistory.CaseAllery,
                                              casehistory.CheckBody,
                                              casehistory.CaseDiag,
                                              casehistory.Memo,
                                              casehistory.ModuleType,
                                              casehistory.DoctID,
                                              this.Operator.ID);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新病历模板Type
        /// </summary>
        /// <param name="ModuleType">模板类型</param>
        /// <param name="Module_NO">模板ID</param>
        /// <returns></returns>
        public int UpdateCaseModuleType(string ModuleType, string Module_NO)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.UpdateModuleType", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = System.String.Format(strSql,
                                              ModuleType, Module_NO);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="casehistory"></param>
        /// <returns></returns>
        public int UpdateCaseModule(Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory casehistory)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.UpdateModule", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = System.String.Format(strSql,
                                              casehistory.Name,
                                              casehistory.DeptID,
                                              casehistory.ModuleType,
                                              casehistory.CaseMain,
                                              casehistory.CaseNow,
                                              casehistory.CaseOld,
                                              casehistory.CaseAllery,
                                              casehistory.CheckBody,
                                              casehistory.CaseDiag,
                                              casehistory.Memo,
                                              casehistory.DoctID,
                                              this.Operator.ID,
                                              casehistory.ID);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="moduleNo"></param>
        /// <returns></returns>
        public int DeleteCaseModule(string moduleNo)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.DelModule", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = System.String.Format(strSql, moduleNo);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 根据模板流水号查询一条记录
        /// </summary>
        /// <param name="moduleNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory QueryCaseModule(string moduleNO)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Case.GetModule", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = System.String.Format(strSql, moduleNO);
            ArrayList al = this.GetMyModule(strSql);
            if (al == null)
                return null;
            else if (al.Count == 0)
                return new Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory();
            else
                return al[0] as Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory;
        }

        /// <summary>
        /// 根据类别获得所有模板
        /// </summary>
        /// <param name="moduletype"></param>
        /// <param name="Code"></param>
        /// <returns></returns>
        public ArrayList QueryAllCaseModule(string moduletype, string Code)
        {
            string strSql = "";
            if (moduletype == "1")//科室
            {
                if (this.Sql.GetCommonSql("Order.OutPatient.Case.GetAllModuleByDeptCode", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return null;
                }
            }
            else
            {
                if (this.Sql.GetCommonSql("Order.OutPatient.Case.GetAllModuleByOperId", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return null;
                }
            }
            strSql = System.String.Format(strSql, moduletype, Code);
            return this.GetMyModule(strSql);
        }

        #region 私有函数
        /// <summary>
        /// 得到病历模板实体
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private ArrayList GetMyModule(string strSql)
        {
            ArrayList al = new ArrayList();
            if (this.ExecQuery(strSql) == -1) return null;
            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory casehistory = new Neusoft.HISFC.Models.Order.OutPatient.ClinicCaseHistory();
                casehistory.CaseMain = this.Reader.GetValue(0).ToString();//主诉
                casehistory.CaseNow = this.Reader.GetValue(1).ToString();//现病史
                casehistory.CaseOld = this.Reader.GetValue(2).ToString();//既往史
                casehistory.CaseAllery = this.Reader.GetValue(3).ToString();//过敏史
                casehistory.CheckBody = this.Reader.GetValue(4).ToString();//查体
                casehistory.CaseDiag = this.Reader.GetValue(5).ToString();//诊断
                casehistory.Memo = this.Reader.GetValue(6).ToString();//备注
                casehistory.Name = this.Reader.GetValue(7).ToString();//模板名称
                casehistory.ID = this.Reader.GetValue(8).ToString();//模板流水号
                casehistory.ModuleType = this.Reader.GetValue(9).ToString();//类别
                casehistory.DoctID = this.Reader.GetValue(10).ToString();//医师编码
                casehistory.DeptID = this.Reader.GetValue(11).ToString();//科室
                al.Add(casehistory);
            }
            this.Reader.Close();
            return al;
        }
        #endregion

        #endregion

        #region 私有函数

        /// <summary>
        /// 获得sql，传入参数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        protected string myGetCommonSql(string sql, Neusoft.HISFC.Models.Order.OutPatient.Order order)
        {
            #region sql
            //   0--看诊序号 ,1 --项目流水号,2 --门诊号,3   --病历号 ,4    --挂号日期
            //   5 --挂号科室,6   --项目代码,7   --项目名称, 8  --规格, 9  --1药品，2非药品
            //   10   --系统类别,   --最小费用代码,   --单价,   --开立数量,   --付数
            //    --包装数量,   --计价单位,   --自费金额0,   --自负金额0,   --报销金额0
            //   --基本剂量,   --自制药,   --药品性质，普药、贵药,   --每次用量
            //     --每次用量单位,   --剂型代码,   --频次,   --频次名称,   --使用方法
            //     --用法名称,   --用法英文缩写,   --执行科室代码,   --执行科室名称
            //      --主药标志,   --组合号,   --1不需要皮试/2需要皮试，未做/3皮试阳/4皮试阴
            //     --院内注射次数,   --备注,   --开立医生,   --开立医生名称,   --医生科室
            //     --开立时间,   --处方状态,1开立，2收费，3确认，4作废,   --作废人,   --作废时间
            //        --加急标记0普通/1加急,   --样本类型,   --检体,   --申请单号
            //     --0不是附材/1是附材,   --是否需要确认，1需要，0不需要,   --确认人
            //        --确认科室,   --确认时间,   --0未收费/1收费,   --收费员
            //       --收费时间,   --处方号,    --处方内流水号,     --发药药房，    
            //      --开立单位是否是最小单位 1 是 0 不是，      --医嘱类型（目前没有）
            #endregion

            //if(order.Item.IsPharmacy)//药品
            if (order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
            {
                Neusoft.HISFC.Models.Pharmacy.Item pItem = order.Item as Neusoft.HISFC.Models.Pharmacy.Item;
                //{9BAE643C-57BF-4dc5-889E-6B5F6B3E1E38} 由于接入电子申请单，apply_no字段赋order.ApplyNo20100505 yangw
                System.Object[] s = {
                                        order.SeeNO ,                                        
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.ID),
                                        order.Patient.ID,                                        
                                        order.Patient.PID.CardNO,                                        
                                        order.RegTime,                                        
										order.InDept.ID,                                        
                                        pItem.ID,                                        
                                        pItem.Name,                                        
                                        pItem.Specs,                                        
                                        "1",                                        
										order.Item.SysClass.ID,                                        
                                        order.Item.MinFee.ID,                                        
                                        order.Item.Price,
                                        order.Qty,
                                        order.HerbalQty,                                        
										pItem.PackQty,
                                        pItem.PriceUnit,
                                        order.FT.OwnCost ,
                                        order.FT.PayCost,
                                        order.FT.PubCost,                                        
										pItem.BaseDose,//20
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(pItem.Product.IsSelfMade),
                                        pItem.Quality.ID,
                                        order.DoseOnce,                                        
										order.DoseUnit,
                                        pItem.DosageForm.ID,
                                        order.Frequency.ID,
                                        order.Frequency.Name,
                                        order.Usage.ID,                                        
										order.Usage.Name,
                                        order.Usage.Memo,
                                        order.ExeDept.ID,
                                        order.ExeDept.Name,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.Combo.IsMainDrug),
                                        order.Combo.ID,
                                        ((Int32)order.HypoTest).ToString(),
										order.InjectCount,
                                        order.Memo,
                                        order.ReciptDoctor.ID,
                                        order.ReciptDoctor.Name,    //
                                        order.ReciptDept.ID,            //40                                        
										order.MOTime,                                        
                                        order.Status,
                                        order.DCOper.ID,
                                        order.DCOper.OperTime,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsEmergency),
                                        order.Sample.Name,
                                        order.CheckPartRecord,
                                        order.ApplyNo,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsSubtbl),
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsNeedConfirm),
                                        order.ConfirmOper.ID,                                        
										order.ConfirmOper.Dept.ID,
                                        order.ConfirmOper.OperTime,
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsHaveCharged),
                                        order.ChargeOper.ID,                                        
										order.ChargeOper.OperTime,
                                        order.ReciptNO,
                                        order.SequenceNO,                                        
                                        order.StockDept.ID,
                                        order.MinunitFlag,
                                        order.UseDays.ToString(),
                                        order.SubCombNO,
                                        order.ExtendFlag1,                  //63                      
										order.ReciptSequence,
                                        order.NurseStation.Memo,
                                        order.SortID,
                                        order.DoseOnceDisplay,
                                        order.DoseUnitDisplay,
                                        order.FirstUseNum,
                                        order.Patient.Pact.ID,
                                        order.Patient.Pact.PayKind.ID,         //71 
                                        order.HosCode, //72
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsExtendRecipe), //处方外延标记 - MK 73
                                    };

                try
                {
                    string sReturn = string.Format(sql, s);
                    return sReturn;
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
            else//非药品
            {
                Neusoft.HISFC.Models.Fee.Item.Undrug pItem = order.Item as Neusoft.HISFC.Models.Fee.Item.Undrug;
                //{9BAE643C-57BF-4dc5-889E-6B5F6B3E1E38} 由于接入电子申请单，apply_no字段赋order.ApplyNo 20100505 yangw
                System.Object[] s = {
                                        order.SeeNO,
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.ID),
                                        order.Patient.ID,
                                        order.Patient.PID.CardNO,
                                        order.RegTime,                                        
										order.InDept.ID,
                                        pItem.ID,
                                        pItem.Name,
                                        pItem.Specs,
                                        "2",                                        
										order.Item.SysClass.ID,
                                        order.Item.MinFee.ID,
                                        order.Item.Price,
                                        order.Qty,
                                        order.HerbalQty,                                        
										pItem.PackQty,
                                        pItem.PriceUnit,
                                        order.FT.OwnCost ,
                                        order.FT.PayCost,
                                        order.FT.PubCost,                                        
										"0",
                                        0,
                                        "",
                                        order.DoseOnce,                                        
										order.DoseUnit,
                                        "",
                                        order.Frequency.ID,
                                        order.Frequency.Name,
                                        order.Usage.ID,                                        
										order.Usage.Name,
                                        order.Usage.Memo,
                                        order.ExeDept.ID,
                                        order.ExeDept.Name,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.Combo.IsMainDrug),
                                        order.Combo.ID,
                                        ((Int32)order.HypoTest).ToString(),                                        
										order.InjectCount,
                                        order.Memo,
                                        order.ReciptDoctor.ID,
                                        order.ReciptDoctor.Name,
                                        order.ReciptDept.ID,                                        
										order.MOTime,                                        
                                        order.Status,
                                        order.DCOper.ID,
                                        order.DCOper.OperTime,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsEmergency),
                                        order.Sample.Name,
                                        order.CheckPartRecord,
                                        order.ApplyNo,                                        
										Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsSubtbl),
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsNeedConfirm),
                                        order.ConfirmOper.ID,                                        
										order.ConfirmOper.Dept.ID,
                                        order.ConfirmOper.OperTime,
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsHaveCharged),
                                        order.ChargeOper.ID,                                        
										order.ChargeOper.OperTime,
                                        order.ReciptNO,
                                        order.SequenceNO,                                        
                                        order.StockDept.ID,
                                        order.MinunitFlag,
                                        "",                                        
                                        order.SubCombNO,
                                        order.ExtendFlag1,                                        
										order.ReciptSequence,
                                        order.NurseStation.Memo,
                                        order.SortID,
                                        order.DoseOnceDisplay,
                                        order.DoseUnitDisplay,
                                        order.FirstUseNum,
                                        order.Patient.Pact.ID,
                                        order.Patient.Pact.PayKind.ID,
                                        order.HosCode,
                                        Neusoft.FrameWork.Function.NConvert.ToInt32(order.IsExtendRecipe), //处方外延标记 - MK 73
                                    };
                try
                {
                    string sReturn = string.Format(sql, s);
                    return sReturn;
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
        }


        /// <summary>
        /// 获得查询sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected int myGetSelectSql(ref string sql)
        {
            return this.Sql.GetCommonSql("Order.OutPatient.Order.Query.Select", ref sql);
        }

        /// <summary>
        /// 获得查询sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected int myGetSelectSql2(ref string sql)
        {
            return this.Sql.GetCommonSql("Order.OutPatient.Order.Query.Select2", ref sql);
        }

        /// <summary>
        /// 获得执行医嘱信息
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected ArrayList myGetExecOrder(string sql)
        {
            if (this.ExecQuery(sql) == -1)
            {
                return null;
            }
            ArrayList al = new ArrayList();

            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Order.OutPatient.Order order = new Neusoft.HISFC.Models.Order.OutPatient.Order();
                try
                {
                    order.SeeNO = this.Reader[0].ToString();
                    order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[1].ToString());//项目流水好
                    order.ID = this.Reader[1].ToString();//项目流水好
                    order.Patient.ID = this.Reader[2].ToString();//门诊号
                    order.Patient.PID.CardNO = this.Reader[3].ToString();//病历卡号
                    order.RegTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4]);//挂号日期
                    order.ReciptDept.ID = this.Reader[5].ToString();//挂号科室 编码
                    if (this.Reader[9].ToString() == "1")//药品
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item item = new Neusoft.HISFC.Models.Pharmacy.Item();
                        item.ID = this.Reader[6].ToString();
                        item.Name = this.Reader[7].ToString();
                        item.Specs = this.Reader[8].ToString();
                        item.SysClass.ID = this.Reader[10].ToString();
                        item.MinFee.ID = this.Reader[11].ToString();
                        item.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12]);
                        item.BaseDose = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20]);
                        item.DoseUnit = this.Reader[24].ToString();
                        item.Product.IsSelfMade = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[21]);
                        item.Quality.ID = this.Reader[22].ToString();
                        item.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[15]);
                        item.DosageForm.ID = this.Reader[25].ToString();
                        item.PriceUnit = this.Reader[16].ToString();

                        //{6DBBDC62-2303-4d97-85EF-8BA2A622117A} 拆分属性 xuc
                        item.SplitType = this.Reader[61].ToString();

                        order.Item = item;

                    }
                    else if (this.Reader[9].ToString() == "2")//非药品
                    {
                        Neusoft.HISFC.Models.Fee.Item.Undrug item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        item.ID = this.Reader[6].ToString();
                        item.Name = this.Reader[7].ToString();
                        item.Specs = this.Reader[8].ToString();
                        item.SysClass.ID = this.Reader[10].ToString();
                        item.MinFee.ID = this.Reader[11].ToString();
                        item.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[12]);
                        item.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[15]);
                        item.PriceUnit = this.Reader[16].ToString();
                        order.Item = item;

                    }
                    else
                    {
                        this.Err = "读取met_ord_recipedetail，区分药品非药品出错，drug_flag=" + this.Reader[9].ToString();
                        this.WriteErr();
                        return null;
                    }
                    order.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13]);
                    order.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[14]);
                    order.Unit = this.Reader[16].ToString();
                    order.FT.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[17]);
                    order.FT.PayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[18]);
                    order.FT.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[19]);

                    order.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[23]);
                    order.DoseUnit = this.Reader[24].ToString();

                    order.Frequency.ID = this.Reader[26].ToString();
                    order.Frequency.Name = this.Reader[27].ToString();
                    order.Usage.ID = this.Reader[28].ToString();
                    order.Usage.Name = this.Reader[29].ToString();
                    order.Usage.Memo = this.Reader[30].ToString();
                    order.ExeDept.ID = this.Reader[31].ToString();
                    order.ExeDept.Name = this.Reader[32].ToString();
                    order.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[33]);
                    order.Combo.ID = this.Reader[34].ToString();
                    order.HypoTest = (Neusoft.HISFC.Models.Order.EnumHypoTest)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[35]);
                    order.InjectCount = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[36]);
                    order.Memo = this.Reader[37].ToString();
                    order.ReciptDoctor.ID = this.Reader[38].ToString();
                    order.ReciptDoctor.Name = this.Reader[39].ToString();
                    order.ReciptDept.ID = this.Reader[40].ToString();
                    //order.ReciptDept.Name =this.Reader[41].ToString();
                    order.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[41]);
                    order.Status = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[42]);
                    order.DCOper.ID = this.Reader[43].ToString();
                    order.DCOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[44]);
                    order.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[45]);
                    order.Sample.Name = this.Reader[46].ToString();
                    order.CheckPartRecord = this.Reader[47].ToString();
                    order.ApplyNo = this.Reader[48].ToString();
                    order.IsSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[49]);
                    order.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[50]);
                    order.ConfirmOper.ID = this.Reader[51].ToString();
                    order.ConfirmOper.Dept.ID = this.Reader[52].ToString();
                    order.ConfirmOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[53]);
                    order.IsHaveCharged = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[54]);
                    order.ChargeOper.ID = this.Reader[55].ToString();
                    order.ChargeOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[56]);
                    order.ReciptNO = this.Reader[57].ToString();
                    order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[58]);
                    order.StockDept.ID = this.Reader[59].ToString();
                    order.MinunitFlag = this.Reader[60].ToString();//最小单位标志
                    order.UseDays = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[62]);//{08024C29-12FE-4629-B982-C50AE9034B82}
                    order.SubCombNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[63].ToString());//附材组合号（检验）
                    order.ExtendFlag1 = this.Reader[64].ToString();//接瓶信息
                    order.ReciptSequence = this.Reader[65].ToString();//收费序列
                    order.NurseStation.Memo = this.Reader[66].ToString();
                    order.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[67]);
                    #region {C3DF9328-3458-4bb4-895E-5B122B6582BB}

                    if (this.Reader[9].ToString() == "1")
                    {
                        order.DoseOnceDisplay = this.Reader[68].ToString();
                        if (order.DoseOnceDisplay.Length <= 0)
                            order.DoseOnceDisplay = order.DoseOnce.ToString();
                    }

                    order.DoseUnitDisplay = this.Reader[69].ToString();
                    order.FirstUseNum = this.Reader[70].ToString();

                    //处方外延标记 - MK
                    if (this.Reader.FieldCount > 71)
                    {
                        order.IsExtendRecipe = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[71].ToString());
                    } 

                    //if (this.Reader.FieldCount > 71)
                    //{
                    //    order.Patient.Pact.ID = Reader[71].ToString();
                    //}
                    //if (this.Reader.FieldCount > 72)
                    //{
                    //    order.Patient.Pact.PayKind.ID = Reader[72].ToString();
                    //}

                    #endregion
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return null;
                }
                finally
                {
                    if (!this.Reader.IsClosed)
                    {
                        this.Reader.Close();
                    }
                }
                al.Add(order);
            }
            this.Reader.Close();
            return al;
        }
        #endregion

        #region
        /// <summary>
        /// 获得用法和用法所带的附材(旧界面fin_opb_inject)
        /// </summary>
        /// <returns></returns>
        public Hashtable GetUsageAndSub()
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.OutPatient.Order.GetUsageAndSub", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            if (this.ExecQuery(strSql) < 0)
            {
                this.Err = "Exec Err" + this.Err;
                return null;
            }

            string usageCode = "";

            Hashtable hsUsageAndSub = new Hashtable();

            while (this.Reader.Read())
            {
                usageCode = this.Reader[0].ToString();

                if (!hsUsageAndSub.Contains(usageCode))
                {
                    ArrayList al = new ArrayList();

                    Neusoft.HISFC.Models.Order.OrderSubtbl o = new Neusoft.HISFC.Models.Order.OrderSubtbl();

                    o.ID = this.Reader[1].ToString();
                    o.QtyRule = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString());

                    al.Add(o);

                    hsUsageAndSub.Add(usageCode, al);
                }
                else
                {
                    Neusoft.HISFC.Models.Order.OrderSubtbl o = new Neusoft.HISFC.Models.Order.OrderSubtbl();

                    o.ID = this.Reader[1].ToString();
                    o.QtyRule = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString());

                    (hsUsageAndSub[usageCode] as ArrayList).Add(o);
                }
            }
            this.Reader.Close();
            return hsUsageAndSub;
        }

        /// <summary>
        /// 得用法和用法所带的附材(新界面met_com_subtblitem)
        /// </summary>
        /// <returns></returns>
        public Hashtable GetNewUsageAndSub()
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.OutPatient.Order.GetNewUsageAndSub", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            if (this.ExecQuery(strSql) < 0)
            {
                this.Err = "Exec Err" + this.Err;
                return null;
            }

            string usageCode = "";

            Hashtable hsUsageAndSub = new Hashtable();

            while (this.Reader.Read())
            {
                usageCode = this.Reader[0].ToString();

                if (!hsUsageAndSub.Contains(usageCode))
                {
                    ArrayList al = new ArrayList();

                    Neusoft.HISFC.Models.Order.OrderSubtbl o = new Neusoft.HISFC.Models.Order.OrderSubtbl();

                    o.ID = this.Reader[1].ToString();
                    o.QtyRule = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString());

                    al.Add(o);

                    hsUsageAndSub.Add(usageCode, al);
                }
                else
                {
                    Neusoft.HISFC.Models.Order.OrderSubtbl o = new Neusoft.HISFC.Models.Order.OrderSubtbl();

                    o.ID = this.Reader[1].ToString();
                    o.QtyRule = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString());

                    (hsUsageAndSub[usageCode] as ArrayList).Add(o);
                }
            }
            this.Reader.Close();
            return hsUsageAndSub;
        }
        #endregion

        #region 门诊新处方输入
        ///add by liuww 2011-3-8


        /// <summary>
        /// 按照卡号查处方历史
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="recipeType"></param>
        /// <returns></returns>
        public ArrayList QueryPatientRecipeByCardAndType(string cardNo, string recipeType, DateTime dtBegin, DateTime dtEnd)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Order.QueryPatientRecipeByCardAndType", ref strSql) == -1)
            {
                this.Err = "没有找到索引为 Order.Order.QueryPatientRecipeByCardAndType的SQL语句";
                return null;
            }

            strSql = string.Format(strSql, cardNo, recipeType, dtBegin, dtEnd);

            if (this.ExecQuery(strSql) == -1)
            {
                return null;
            }

            ArrayList orderList = new ArrayList();

            Neusoft.HISFC.Models.Order.OutPatient.Order order = null;
            while (this.Reader.Read())
            {
                order = new Neusoft.HISFC.Models.Order.OutPatient.Order();

                order.Patient.ID = this.Reader[0].ToString();
                order.ReciptSequence = this.Reader[1].ToString();
                order.RecipeType.ID = this.Reader[2].ToString();
                order.ReciptNO = this.Reader[4].ToString();
                order.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[5].ToString());
                order.Status = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[3].ToString());

                orderList.Add(order);
            }

            this.Reader.Close();

            return orderList;
        }

        /// <summary>
        /// 根据处方类别,和患者流水号,查询处方信息
        /// </summary>
        /// <param name="clinicNO"></param>
        /// <param name="recipeType"></param>
        /// <returns></returns>
        public ArrayList QueryPatientRecipeByType(string clinicNO, string recipeType)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Order.QueryPatientRecipeByType", ref strSql) == -1)
            {
                this.Err = "没有找到索引为 Order.Order.QueryPatientRecipeByType的SQL语句";
                return null;
            }

            strSql = string.Format(strSql, clinicNO, recipeType);

            if (this.ExecQuery(strSql) == -1)
            {
                return null;
            }

            ArrayList orderList = new ArrayList();

            Neusoft.HISFC.Models.Order.OutPatient.Order order = null;

            while (this.Reader.Read())
            {
                order = new Neusoft.HISFC.Models.Order.OutPatient.Order();

                order.Patient.ID = this.Reader[0].ToString();
                order.ReciptSequence = this.Reader[1].ToString();
                order.RecipeType.ID = this.Reader[2].ToString();
                order.ReciptNO = this.Reader[4].ToString();
                order.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[5].ToString());
                order.Status = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[3].ToString());

                orderList.Add(order);
            }

            this.Reader.Close();

            return orderList;
        }

        /// <summary>
        /// 更新健康信息：身高、体重、血压
        /// </summary>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP"></param>
        /// <param name="DBP"></param>
        /// <param name="clinicCode"></param>
        /// <param name="tem"></param>
        /// <returns></returns>
        public int UpdateHealthInfo(string height, string weight, string SBP, string DBP, string clinicCode, string tem, string bloodGlu)
        {
            return this.ExecNoQueryByIndex("Order.OutPatient.HealthInfo.UpdateByClinicCode", height, weight, SBP, DBP, clinicCode, tem, bloodGlu);
        }

        /// <summary>
        /// 更新健康信息：身高、体重、血压 和体征
        /// </summary>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP"></param>
        /// <param name="DBP"></param>
        /// <param name="clinicCode"></param>
        /// <param name="tem"></param>
        /// <returns></returns>
        public int UpdateHealthInfoAndSymptom(string height, string weight, string SBP, string DBP, string clinicCode, string tem, string bloodGlu, string symptom)
        {
            return this.ExecNoQueryByIndex("Order.OutPatient.HealthInfoAndSymptom.UpdateByClinicCode", height, weight, SBP, DBP, clinicCode, tem, bloodGlu, symptom);
        }

        /// <summary>
        /// 获取健康信息：身高、体重、血压
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP">血压：收缩压</param>
        /// <param name="DBP">血压：舒张压</param>
        /// <returns></returns>
        public int GetHealthInfo(string sqlIndex, ref string height, ref string weight, ref string SBP, ref string DBP, ref string tem, ref string bloodGlu, params string[] param)
        {
            try
            {
                if (this.ExecQueryByIndex(sqlIndex, param) < 0)
                {
                    return -1;
                }

                if (this.Reader != null && this.Reader.Read())
                {
                    height = this.Reader[0].ToString();
                    weight = this.Reader[1].ToString();
                    SBP = this.Reader[2].ToString();
                    DBP = this.Reader[3].ToString();
                    tem = this.Reader[4].ToString();
                    bloodGlu = this.Reader[5].ToString();
                    return 1;
                }
                else
                {
                    Err = "未找到挂号信息！";
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return -1;
            }
            finally
            {
                if (this.Reader != null && !this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }

        /// <summary>
        /// 获取健康信息：身高、体重、血压和体征
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP">血压：收缩压</param>
        /// <param name="DBP">血压：舒张压</param>
        /// <returns></returns>
        public int GetHealthInfoAndSymptom(string sqlIndex, ref string height, ref string weight, ref string SBP, ref string DBP, ref string tem, ref string bloodGlu, ref string symptom, params string[] param)
        {
            try
            {
                if (this.ExecQueryByIndex(sqlIndex, param) < 0)
                {
                    return -1;
                }

                if (this.Reader != null && this.Reader.Read())
                {
                    height = this.Reader[0].ToString();
                    weight = this.Reader[1].ToString();
                    SBP = this.Reader[2].ToString();
                    DBP = this.Reader[3].ToString();
                    tem = this.Reader[4].ToString();
                    bloodGlu = this.Reader[5].ToString();
                    symptom = this.Reader[6].ToString();
                    return 1;
                }
                else
                {
                    Err = "未找到挂号信息！";
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return -1;
            }
            finally
            {
                if (this.Reader != null && !this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }

        /// <summary>
        /// 根据门诊流水号获取健康信息：身高、体重、血压
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP">血压：收缩压</param>
        /// <param name="DBP">血压：舒张压</param>
        /// <returns></returns>
        public int GetHealthInfo(string clinicCode, ref string height, ref string weight, ref string SBP, ref string DBP, ref string tem, ref string bloodGlu)
        {
            return this.GetHealthInfo("Order.OutPatient.HealthInfo.GetByClinicCode", ref height, ref weight, ref SBP, ref DBP, ref tem, ref bloodGlu, clinicCode);
        }

        /// <summary>
        /// 根据门诊流水号获取健康信息：身高、体重、血压和体征
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP">血压：收缩压</param>
        /// <param name="DBP">血压：舒张压</param>
        /// <returns></returns>
        public int GetHealthInfoAndSymptom(string clinicCode, ref string height, ref string weight, ref string SBP, ref string DBP, ref string tem, ref string bloodGlu,ref string symptom)
        {
            return this.GetHealthInfoAndSymptom("Order.OutPatient.HealthInfoAndSymptom.GetByClinicCode", ref height, ref weight, ref SBP, ref DBP, ref tem, ref bloodGlu, ref symptom, clinicCode);
        }

        /// <summary>
        /// 根据门诊卡号获取最近一次健康信息：身高、体重、血压
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="days">查询时间段</param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP"></param>
        /// <param name="DBP"></param>
        /// <param name="tem"></param>
        /// <param name="bloodGlu"></param>
        /// <returns></returns>
        public int GetHealthInfo(string cardNo, int days, ref string height, ref string weight, ref string SBP, ref string DBP, ref string tem, ref string bloodGlu)
        {
            DateTime dt = this.GetDateTimeFromSysDateTime().Date.AddDays(0 - days);
            return this.GetHealthInfo("Order.OutPatient.HealthInfo.GetByCardNo", ref height, ref weight, ref SBP, ref DBP, ref tem, ref bloodGlu, cardNo, dt.ToString());
        }

        /// <summary>
        /// 根据门诊卡号获取最近一次健康信息：身高、体重、血压和体征
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="days">查询时间段</param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP"></param>
        /// <param name="DBP"></param>
        /// <param name="tem"></param>
        /// <param name="bloodGlu"></param>
        /// <returns></returns>
        public int GetHealthInfoAndSymptom(string cardNo, int days, ref string height, ref string weight, ref string SBP, ref string DBP, ref string tem, ref string bloodGlu, ref string symptom)
        {
            DateTime dt = this.GetDateTimeFromSysDateTime().Date.AddDays(0 - days);
            return this.GetHealthInfoAndSymptom("Order.OutPatient.HealthInfoAndSymptom.GetByCardNo", ref height, ref weight, ref SBP, ref DBP, ref tem, ref bloodGlu, ref symptom, cardNo, dt.ToString());
        }

        #endregion

        /// <summary>
        /// 根据卡号查询时间段内的处方号
        /// </summary>
        /// <param name="cardNO"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public ArrayList QueryRecipeNOByCardNO(string cardNO, DateTime dtBegin, DateTime dtEnd)
        {
            string sql = "";
            string errText = "";
            if (this.Sql.GetCommonSql("Order.OutPatient.Order.QueryRecipeNOByCardNO", ref sql) == -1)
            {
                return null;
            }
            sql = string.Format(sql, cardNO, dtBegin.ToString(), dtEnd.ToString());
            try
            {
                if (this.ExecQuery(sql) == -1)
                {
                    errText = "执行查询sql失败";
                    return null;
                }
                ArrayList al = new ArrayList();
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Order.OutPatient.Order orderObj = new Neusoft.HISFC.Models.Order.OutPatient.Order();
                    orderObj.ID = this.Reader[0].ToString();
                    orderObj.Name = this.Reader[1].ToString();
                    orderObj.Memo = this.Reader[2].ToString();
                    al.Add(orderObj);
                }
                return al;
            }
            catch (Exception ex)
            {
                this.Err = errText + ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }

        /// <summary>
        /// 翻译皮试信息
        /// </summary>
        /// <param name="i"></param>
        /// <returns>1 [免试] 2 [需皮试] 3 [+] 4 [-]</returns>
        public string TransHypotest(Neusoft.HISFC.Models.Order.EnumHypoTest HypotestCode)
        {
            //return Neusoft.FrameWork.Public.EnumHelper.Current.GetName(HypotestCode);

            switch ((int)HypotestCode)
            {
                case 0:
                    //return "不需要皮试";
                    return "";
                case 1:
                    return "[免试]";
                case 2:
                    return "[需皮试]";
                case 3:
                    return "[+]";
                case 4:
                    return "[-]";
                default:
                    return "[免试]";
            }
        }

        /// <summary>
        /// 草药插入处方
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int InsertInRecipe(Neusoft.HISFC.Models.Order.Inpatient.Order order)
        {
            string strsql = string.Empty;
            if (this.Sql.GetCommonSql("Order.Item.InpatientRecipe.Insert", ref strsql) == -1)
            {
                return -1;
            }
            try
            {
                strsql = string.Format(strsql, order.Patient.ID,
                    //order.Patient.FT.User08,
                                                           order.ID,
                                                           order.Combo.ID,
                                                           order.Item.ID,
                                                           order.Item.Name,
                                                           order.DoseOnce,
                                                           order.Unit,
                                                           order.HerbalQty
                    //,
                    //order.Usage.User08
                    //,
                    //order.Usage.Name,
                    //order.Usage.User07,
                    //"0",
                    //order.Doctor.User05,
                    //order.Doctor.User06,
                    //order.Doctor.User07,
                    //order.Doctor.User08,
                    //order.DoctorDept.User03,
                    //order.DoctorDept.User04,
                    //order.DoctorDept.User05,
                    //order.DoctorDept.User06,
                    //order.DoctorDept.User07,
                    //order.DoctorDept.User08
                                                           );
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }

            if (this.ExecQuery(strsql) == -1)
            {
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// 根据项目编号查询申请单类型{D793A341-AD35-4685-8817-5614217969AD} 2014-12-16 by lixuelong
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public Neusoft.FrameWork.Models.NeuObject QueryApplyTypeByItemCode(string itemCode)
        {
            string sql = "Order.OutPatient.Order.QueryApplyTypeByItemCode";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                sql = string.Format(sql, itemCode);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(sql) == -1) return null;
            Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
            this.Reader.Read();
            try
            {
                obj.ID = this.Reader[0].ToString();
                obj.Name = this.Reader[1].ToString();
            }
            catch { }
            this.Reader.Close();
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public bool OrderIsExists(string clinicCode, string itemCode)
        {
            try
            {
                string sql = @"Select count(*)
  FROM Met_Ord_Recipedetail a
 where a.clinic_code = '{0}'
   and a.item_code = '{1}'
   and a.status in ('0', '1', '2')
   and a.class_code in('UC','UL')";

                sql = string.Format(sql, clinicCode, itemCode);

                int cnt = int.Parse(this.ExecSqlReturnOne(sql));

                if (cnt > 0)
                {
                    return true;
                }
                return false;
            }

            catch(Exception ex) {
                this.Err = ex.Message;
                return false;
            }
           
        }



        // {1F9AC411-F9A2-4296-8005-5750C5DD4374}
        /// <summary>
        /// 当天是否开立过 *(免费)新型冠状病毒（COVID-19）核酸检测  F00000082306 项目
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public bool OrderIsExistsCOVID(string cardNo)
        {
            try
            {
                string sql = @"Select count(*)
  FROM Met_Ord_Recipedetail a
 where a.CARD_NO = '{0}'
   --and a.OPER_DATE > TRUNC(sysdate)
   and a.OPER_DATE > sysdate-7
   and a.item_code = 'F00000082306'
   and a.status in ('0', '1', '2')
";
                sql = string.Format(sql, cardNo);

                int cnt = int.Parse(this.ExecSqlReturnOne(sql));
                if (cnt > 0)
                {
                    return true;
                }
                return false;
            }

            catch (Exception ex)
            {
                this.Err = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// 门诊医生CA证书绿色通道
        /// 特殊情况可以不插电子签名开方
        /// </summary>
        /// <param name="doctCode"></param>
        /// <returns></returns>
        public bool IsGreenLineCA(string doctCode)
        {
            return true;
            string sql = @"select count(*) from com_dictionary a
                where a.type='CAGREENLINE'
                and a.valid_state='1'
                and a.code='{0}' ";

            try
            {
                sql = string.Format(sql, doctCode);
                return Neusoft.FrameWork.Function.NConvert.ToBoolean(this.ExecSqlReturnOne(sql));
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 取药品的注射费及辅材费
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public decimal GetFuCaiCost(string clinicCode,string comboNo)
        {
            string sql = @"select sum(a.own_cost+a.pub_cost+a.pay_cost)t_cost
 from fin_opb_feedetail a
where a.clinic_code='{0}'
and a.comb_no='{1}'
and a.trans_type='1'  
and a.drug_flag='0' 
and a.fee_code not in ('015','016','017','018','019','020','021','022') ";
            sql = string.Format(sql, clinicCode, comboNo);

            return FrameWork.Function.NConvert.ToDecimal(this.ExecSqlReturnOne(sql));
        }
		
		
		  /// <summary>
        /// 门诊就诊患者登记筛查插入数据 20200213
        /// </summary>
        /// <param name="regNCPInfo"></param>
        /// <returns></returns>
        public int InsertRegNCP(Neusoft.HISFC.Models.Registration.RegNCP regNCPInfo)
        {
             string sql = string.Empty;
            //已经存在，那么更新
            if (QueryRegNCPInfo(regNCPInfo.CARD_NO,regNCPInfo.DEPT_CODE)!=null)
            {
                #region UpdateSql
                sql = @"update FIN_OPR_REGISTER_NCP set
                                           NAME= '{0}',
                                           IDENNO= '{1}',
                                           RELA_PHONE= '{2}',
                                           ISINWUHAN= '{3}',
                                           ISTOUCHWUHAN= '{4}',
                                           ISTOUCHANIMAL= '{5}',
                                           SYMPTOM_TEMPERATURE= '{6}',
                                           SYMPTOM_ERYTHRA= '{7}',
                                           SYMPTOM_COUGH= '{8}',
                                           SYMPTOM_VOMIT= '{9}',
                                           SYMPTOM_DIARRHOEA= '{10}',
                                           SYMPTOM_HEADACHE= '{11}',
                                           SYMPTOM_OTHER= '{12}',
                                           OPER_CODE= '{13}',
                                           OPER_NAME= '{14}',
                                           OPER_DATE= to_date('{15}','yyyy-mm-dd hh24:mi:ss'),
                                           SEX= '{16}',
                                           ADDRESS= '{17}',
                                           HOMEPHONE= '{18}',
                                           ISTOUCHWUHAN_NOTE= '{19}',
                                           ISTOUR= '{20}',
                                           SYMPTOM_TIME= '{21}',
                                           ISNEEDHSJC= '{22}',
                                           HSJC_TYPE_NEWIN= '{23}',
                                           HSJC_TYPE_NEWIN_DEPT= '{24}',
                                           HSJC_TYPE_NEWIN_DATE= '{25}',
                                           HSJC_TYPE_INPATIENT= '{26}',
                                           HSJC_TYPE_INPATIENT_DEPT= '{27}',
                                           HOMETOWN= '{28}',
                                           WORK= '{29}',
                                           HSJC_TYPE_OTHER= '{30}',
                                           HSJC_TYPE_OTHERTEXT= '{31}'
                             where CARD_NO = '{32}'
                               and DEPT_CODE = '{33}'
                               and OPER_DATE > trunc(sysdate)";

                sql = string.Format(sql, regNCPInfo.NAME,
                                         regNCPInfo.IDENNO,
                                         regNCPInfo.RELA_PHONE,
                                         regNCPInfo.ISINWUHAN,
                                         regNCPInfo.ISTOUCHWUHAN,
                                         regNCPInfo.ISTOUCHANIMAL,
                                         regNCPInfo.SYMPTOM_TEMPERATURE,
                                         regNCPInfo.SYMPTOM_ERYTHRA,
                                         regNCPInfo.SYMPTOM_COUGH,
                                         regNCPInfo.SYMPTOM_VOMIT,
                                         regNCPInfo.SYMPTOM_DIARRHOEA,
                                         regNCPInfo.SYMPTOM_HEADACHE,
                                         regNCPInfo.SYMPTOM_OTHER,
                                         regNCPInfo.OPER_CODE,
                                         regNCPInfo.OPER_NAME,
                                         regNCPInfo.OPER_DATE,
                                         regNCPInfo.SEX,
                                         regNCPInfo.ADDRESS,
                                         regNCPInfo.HOMEPHONE,
                                         regNCPInfo.ISTOUCHWUHAN_NOTE,
                                         regNCPInfo.ISTOUR,
                                         regNCPInfo.SYMPTOM_TIME,
                                         regNCPInfo.ISNEEDHSJC,
                                         regNCPInfo.HSJC_TYPE_NEWIN,
                                         regNCPInfo.HSJC_TYPE_NEWIN_DEPT,
                                         regNCPInfo.HSJC_TYPE_NEWIN_DATE,
                                         regNCPInfo.HSJC_TYPE_INPATIENT,
                                         regNCPInfo.HSJC_TYPE_INPATIENT_DEPT,
                                         regNCPInfo.HOMETOWN,
                                         regNCPInfo.WORK,
                                         regNCPInfo.HSJC_TYPE_OTHER,
                                         regNCPInfo.HSJC_TYPE_OTHERTEXT,
                                         regNCPInfo.CARD_NO,
                                         regNCPInfo.DEPT_CODE);
                #endregion
            }
            else
            {
                #region InsertSql
                sql = @"insert into FIN_OPR_REGISTER_NCP
                                          (CLINIC_CODE,
                                           CARD_NO,
                                           NAME,
                                           IDENNO,
                                           RELA_PHONE,
                                           ISINWUHAN,
                                           ISTOUCHWUHAN,
                                           ISTOUCHANIMAL,
                                           SYMPTOM_TEMPERATURE,
                                           SYMPTOM_ERYTHRA,
                                           SYMPTOM_COUGH,
                                           SYMPTOM_VOMIT,
                                           SYMPTOM_DIARRHOEA,
                                           SYMPTOM_HEADACHE,
                                           SYMPTOM_OTHER,
                                           DEPT_CODE,
                                           OPER_CODE,
                                           OPER_NAME,
                                           OPER_DATE,
                                           SEX,
                                           ADDRESS,
                                           HOMEPHONE,
                                           ISTOUCHWUHAN_NOTE,
                                           ISTOUR,
                                           SYMPTOM_TIME,
                                           ISNEEDHSJC,
                                           HSJC_TYPE_NEWIN,
                                           HSJC_TYPE_NEWIN_DEPT,
                                           HSJC_TYPE_NEWIN_DATE,
                                           HSJC_TYPE_INPATIENT,
                                           HSJC_TYPE_INPATIENT_DEPT,
                                           HOMETOWN,
                                           WORK,
                                           HSJC_TYPE_OTHER,
                                           HSJC_TYPE_OTHERTEXT)
                                        values
                                          ('{0}',
                                           '{1}',
                                           '{2}',
                                           '{3}',
                                           '{4}',
                                           '{5}',
                                           '{6}',
                                           '{7}',
                                           '{8}',
                                           '{9}',
                                           '{10}',--SYMPTOM_ERYTHRA
                                           '{11}',
                                           '{12}',
                                           '{13}',
                                           '{14}',
                                           '{15}',
                                           '{16}',
                                           '{17}',
                                            to_date('{18}','yyyy-mm-dd hh24:mi:ss'),--OPER_DATE
                                           '{19}',
                                           '{20}',
                                           '{21}',
                                           '{22}',
                                           '{23}',
                                           '{24}',--SYMPTOM_TIME
                                           '{25}',
                                           '{26}',
                                           '{27}',
                                           '{28}',--HSJC_TYPE_NEWIN_DATE
                                           '{29}',
                                           '{30}',
                                           '{31}',
                                           '{32}',
                                           '{33}',
                                           '{34}')";

                sql = string.Format(sql, regNCPInfo.CLINIC_CODE,
                                         regNCPInfo.CARD_NO,
                                         regNCPInfo.NAME,
                                         regNCPInfo.IDENNO,
                                         regNCPInfo.RELA_PHONE,
                                         regNCPInfo.ISINWUHAN,
                                         regNCPInfo.ISTOUCHWUHAN,
                                         regNCPInfo.ISTOUCHANIMAL,
                                         regNCPInfo.SYMPTOM_TEMPERATURE,
                                         regNCPInfo.SYMPTOM_ERYTHRA,
                                         regNCPInfo.SYMPTOM_COUGH,
                                         regNCPInfo.SYMPTOM_VOMIT,
                                         regNCPInfo.SYMPTOM_DIARRHOEA,
                                         regNCPInfo.SYMPTOM_HEADACHE,
                                         regNCPInfo.SYMPTOM_OTHER,
                                         regNCPInfo.DEPT_CODE,
                                         regNCPInfo.OPER_CODE,
                                         regNCPInfo.OPER_NAME,
                                         regNCPInfo.OPER_DATE,
                                         regNCPInfo.SEX,
                                         regNCPInfo.ADDRESS,
                                         regNCPInfo.HOMEPHONE,
                                         regNCPInfo.ISTOUCHWUHAN_NOTE,
                                         regNCPInfo.ISTOUR,
                                         regNCPInfo.SYMPTOM_TIME,
                                         regNCPInfo.ISNEEDHSJC,
                                         regNCPInfo.HSJC_TYPE_NEWIN,
                                         regNCPInfo.HSJC_TYPE_NEWIN_DEPT,
                                         regNCPInfo.HSJC_TYPE_NEWIN_DATE,
                                         regNCPInfo.HSJC_TYPE_INPATIENT,
                                         regNCPInfo.HSJC_TYPE_INPATIENT_DEPT,
                                         regNCPInfo.HOMETOWN,
                                         regNCPInfo.WORK,
                                         regNCPInfo.HSJC_TYPE_OTHER,
                                         regNCPInfo.HSJC_TYPE_OTHERTEXT);
                #endregion
            }
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 获取门诊就诊患者登记筛查 20200213
        /// </summary>
        /// <param name="cliniCode"></param>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.RegNCP QueryRegNCPInfo(string cardNo, string deptCode)
        {
            #region MyRegion
            string sql = @"select  CLINIC_CODE,
                                   CARD_NO,
                                   NAME,
                                   IDENNO,
                                   RELA_PHONE,
                                   ISINWUHAN,
                                   ISTOUCHWUHAN,
                                   ISTOUCHANIMAL,
                                   SYMPTOM_TEMPERATURE,
                                   SYMPTOM_ERYTHRA,
                                   SYMPTOM_COUGH,
                                   SYMPTOM_VOMIT,
                                   SYMPTOM_DIARRHOEA,
                                   SYMPTOM_HEADACHE,
                                   SYMPTOM_OTHER,
                                   DEPT_CODE,
                                   OPER_CODE,
                                   OPER_NAME,
                                   OPER_DATE,
                                   SEX,
                                   ADDRESS,
                                   HOMEPHONE,
                                   ISTOUCHWUHAN_NOTE,
                                   ISTOUR,
                                   SYMPTOM_TIME,
                                   ISNEEDHSJC,
                                   HSJC_TYPE_NEWIN,
                                   HSJC_TYPE_NEWIN_DEPT,
                                   HSJC_TYPE_NEWIN_DATE,
                                   HSJC_TYPE_INPATIENT,
                                   HSJC_TYPE_INPATIENT_DEPT,
                                   HOMETOWN,
                                   WORK,
                                   HSJC_TYPE_OTHER,
                                   HSJC_TYPE_OTHERTEXT
                              from FIN_OPR_REGISTER_NCP
                             where CARD_NO = '{0}'
                               and DEPT_CODE = '{1}' 
                               and OPER_DATE > trunc(sysdate)";
            sql = string.Format(sql, cardNo, deptCode);

            if (this.ExecQuery(sql) == -1)
            {
                return null;
            }

            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Registration.RegNCP regNCPInfo = new Neusoft.HISFC.Models.Registration.RegNCP();
                regNCPInfo.CLINIC_CODE = this.Reader[0].ToString();
                regNCPInfo.CARD_NO = this.Reader[1].ToString();
                regNCPInfo.NAME = this.Reader[2].ToString();
                regNCPInfo.IDENNO = this.Reader[3].ToString();
                regNCPInfo.RELA_PHONE = this.Reader[4].ToString();
                regNCPInfo.ISINWUHAN = this.Reader[5].ToString();
                regNCPInfo.ISTOUCHWUHAN = this.Reader[6].ToString();
                regNCPInfo.ISTOUCHANIMAL = this.Reader[7].ToString();
                regNCPInfo.SYMPTOM_TEMPERATURE = this.Reader[8].ToString();
                regNCPInfo.SYMPTOM_ERYTHRA = this.Reader[9].ToString();
                regNCPInfo.SYMPTOM_COUGH = this.Reader[10].ToString();
                regNCPInfo.SYMPTOM_VOMIT = this.Reader[11].ToString();
                regNCPInfo.SYMPTOM_DIARRHOEA = this.Reader[12].ToString();
                regNCPInfo.SYMPTOM_HEADACHE = this.Reader[13].ToString();
                regNCPInfo.SYMPTOM_OTHER = this.Reader[14].ToString();
                regNCPInfo.DEPT_CODE = this.Reader[15].ToString();
                regNCPInfo.OPER_CODE = this.Reader[16].ToString();
                regNCPInfo.OPER_NAME = this.Reader[17].ToString();
                regNCPInfo.OPER_DATE = this.Reader[18].ToString();
                regNCPInfo.SEX = this.Reader[19].ToString();
                regNCPInfo.ADDRESS = this.Reader[20].ToString();
                regNCPInfo.HOMEPHONE = this.Reader[21].ToString();
                regNCPInfo.ISTOUCHWUHAN_NOTE = this.Reader[22].ToString();
                regNCPInfo.ISTOUR = this.Reader[23].ToString();
                regNCPInfo.SYMPTOM_TIME = this.Reader[24].ToString();
                regNCPInfo.ISNEEDHSJC = this.Reader[25].ToString();
                regNCPInfo.HSJC_TYPE_NEWIN = this.Reader[26].ToString();
                regNCPInfo.HSJC_TYPE_NEWIN_DEPT = this.Reader[27].ToString();
                regNCPInfo.HSJC_TYPE_NEWIN_DATE = this.Reader[28].ToString();
                regNCPInfo.HSJC_TYPE_INPATIENT = this.Reader[29].ToString();
                regNCPInfo.HSJC_TYPE_INPATIENT_DEPT = this.Reader[30].ToString();
                regNCPInfo.HOMETOWN = this.Reader[31].ToString();
                regNCPInfo.WORK = this.Reader[32].ToString();
                regNCPInfo.HSJC_TYPE_OTHER = this.Reader[33].ToString();
                regNCPInfo.HSJC_TYPE_OTHERTEXT = this.Reader[34].ToString();
                return regNCPInfo;
            }
            return null; 
            #endregion
        }


        // Neusoft.HISFC.BizLogic.Order.OutPatient.Order
        public Hashtable getZLDrugList()
        {
            Hashtable hashtable = new Hashtable();
            List<NeuObject> list = new List<NeuObject>();
            string strSql = @"select tt.drug_code,tt.trade_name from com_dictionary t ,pha_com_baseinfo tt where t.type = 'SJZHLDRUG' 
            and trim(t.code) = trim(tt.formal_custom) ";
            Hashtable result;
            if (base.ExecQuery(strSql) == -1)
            {
                result = null;
            }
            else
            {
                while (base.Reader.Read())
                {
                    list.Add(new NeuObject
                    {
                        ID = base.Reader[0].ToString(),
                        Name = base.Reader[1].ToString()
                    });
                }
                if (list != null && list.Count > 0)
                {
                    foreach (NeuObject current in list)
                    {
                        if (!hashtable.ContainsKey(current.ID))
                        {
                            hashtable.Add(current.ID, current);
                        }
                    }
                }
                result = hashtable;
            }
            return result;
        }

    }
}
