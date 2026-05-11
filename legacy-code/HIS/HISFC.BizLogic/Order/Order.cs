using System;
using Neusoft.HISFC.Models;
using System.Collections;
using Neusoft.FrameWork.Models;
using System.Data;
using System.Collections.Generic;
namespace Neusoft.HISFC.BizLogic.Order
{
    /// <summary>
    /// 医嘱管理类。
    /// 
    /// <说明>
    ///     1 2007-04 增加配液中心处理
    ///         执行挡插入函数增加参数是否需配液
    ///     2 增加查询、更新函数
    /// </说明>
    /// </summary>
    public class Order : Neusoft.FrameWork.Management.Database
    {
        public Order()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 静态量

        /// <summary>
        /// 需配液用法
        /// </summary>
        private static System.Collections.Hashtable hsCompoundUsage = null;

        /// <summary>
        /// 是否使用药品执行、扣费分开流程 0 同时进行 1 不同时进行
        /// </summary>
        bool bCharge;

        /// <summary>
        /// 是否已经重新查询过变量bCharge
        /// </summary>
        bool isGet_bCharge = false;

        /// <summary>
        /// 药品后计费时，药品附材是否与药品同时计费 1 护士站计费 0 药房计费
        /// </summary>
        bool bChargeSubtbl;

        /// <summary>
        /// 是否已经重新查询过变量bChargeSubtbl
        /// </summary>
        bool isGet_bChargeSubtbl = false;

        /// <summary>
        /// 分解截止时间（小时）
        /// </summary>
        protected int iHour = 12;

        /// <summary>
        /// 分解截止时间（分钟）
        /// </summary>
        protected int iMinute = 1;

        /// <summary>
        /// 当前护理站代码
        /// </summary>
        protected string strNurseStationCode = "";

        protected Neusoft.FrameWork.Management.ControlParam controler = new Neusoft.FrameWork.Management.ControlParam();

        /// <summary>
        /// 系统当前时间
        /// </summary>
        DateTime dtCurTime = new DateTime();

        /// <summary>
        /// 小时计费医嘱的频次代码 {97FA5C9D-F454-4aba-9C36-8AF81B7C9CCF} 小时频次
        /// </summary>
        private string hourFerquenceID = "";

        #endregion

        #region 作废
        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [Obsolete("用InsertOrder代替！", true)]
        public int CreateOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order)
        {
            return this.InsertOrder(order);
        }


        [Obsolete("QueryOrderSubtbl代替了", true)]
        public ArrayList QueryOrderSub(string InPatientNo)
        {
            return this.QueryOrderSubtbl(InPatientNo);
        }
        [Obsolete("用InsertExecOrder代替了", true)]
        public int CreateExec(Neusoft.HISFC.Models.Order.ExecOrder ExecOrder)
        {
            return this.InsertExecOrder(ExecOrder);
        }

        [Obsolete("UpdateRecordExec代替", true)]
        public int RecordExec(Neusoft.HISFC.Models.Order.ExecOrder ExecOrder)
        {
            return this.UpdateRecordExec(ExecOrder);
        }

        [Obsolete("用UpdateChargeExec代替", true)]
        public int ChargeExec(Neusoft.HISFC.Models.Order.ExecOrder ExecOrder)
        {
            return UpdateChargeExec(ExecOrder);
        }
        [Obsolete("UpdateDrugExec代替", true)]
        public int DrugExec(Neusoft.HISFC.Models.Order.ExecOrder ExecOrder)
        {
            return this.UpdateDrugExec(ExecOrder);
        }

        [Obsolete("QueryExecOrder(inpatientNo,itemType);代替", true)]
        public ArrayList QueryPatientExec(string inpatientNo, string itemType)
        {
            return this.QueryExecOrder(inpatientNo, itemType);
        }
        [Obsolete("用QueryExecOrder(InPatientNo,ItemType,IsValid)代替", true)]
        public ArrayList QueryValidOrder(string InPatientNo, string ItemType, bool IsValid)
        {
            return this.QueryExecOrder(InPatientNo, ItemType, IsValid);
        }
        [Obsolete("QueryExecOrder(inpatientNo,itemType,isCharge)代替", true)]
        public ArrayList QueryChargeOrder(string inpatientNo, string itemType, bool isCharge)
        {
            return QueryExecOrder(inpatientNo, itemType, isCharge);
        }
        [Obsolete("QueryExecOrderByDrugFlag(InPatientNo,DateTimeBegin,DateTimeEnd,  DrugFlag)代替", true)]
        public ArrayList QueryOrderDrugFlag(string InPatientNo, DateTime DateTimeBegin, DateTime DateTimeEnd, int DrugFlag)
        {
            return this.QueryExecOrderByDrugFlag(InPatientNo, DateTimeBegin, DateTimeEnd, DrugFlag);
        }
        [Obsolete("QueryExecOrderByDrugFlag(InPatientNo,DrugFlag)代替", true)]
        public ArrayList QueryOrderDrugFlag(string InPatientNo, int DrugFlag)
        {
            return this.QueryExecOrderByDrugFlag(InPatientNo, DrugFlag);
        }
        #endregion

        #region 增删改

        /// <summary>
        /// 开立新医嘱(插入新医嘱记录)
        /// </summary>
        /// <param name="order"></param>
        /// <returns>0 success -1 fail</returns>
        public int InsertOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order)
        {
            #region 开立新医嘱
            //开立新医嘱
            //Order.Order.CreateOrder.1
            //传入：71
            //			//传出：0 
            #endregion

            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Order.CreateOrder.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = GetOrderInfo(strSql, order);
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// CA(插入新医嘱加密记录)
        /// </summary>
        /// <param name="string"></param>
        /// <returns>0 success -1 fail</returns>
        public int InsertOrderSign(string operid, string str, string key, string patientno, string certdata)
        {
            #region 开立新医嘱加密信息
            //开立新医嘱
            //Order.Order.CreateOrderSign.1
            //传入：71
            //			//传出：0 
            /*
             *  INSERT INTO FIN_CA_SIGN
                  (OPER_ID, SIGN_TIME, PRIVATE_INFO, CERT,CLINIC_NO,CERTDATA)
                VALUES
                  ('{0}', sysdate, '{1}', '{2}','{3}','{4}')
             */
            #endregion

            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Order.CreateOrderSign.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = string.Format(strSql, operid, str, key, patientno, certdata);
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// CA更新医嘱加密信息
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public int UpdateOrderSign(string operid, string str, string key, string patientno, string certdata, string signtime)
        {
            #region 更新医嘱加密信息
            //更新医嘱
            //Order.Order.updateOrderSign.1
            //传入：71
            //传出：0 
            /* UPDATE FIN_CA_SIGN
               SET 
                   OPER_ID = '{0}',
                   SIGN_TIME=sysdate,
                   PRIVATE_INFO = '{1}',
                   CERT ='{2}',
                   CERTDATA='{4}'
             where CLINIC_NO = '{3}'AND CERT ='{2}'and sign_time=to_date('{5}', 'yyyy-mm-dd hh24:mi:ss')
             */
            #endregion
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Order.updateOrderSign.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = string.Format(strSql, operid, str, key, patientno, certdata, signtime);
            if (strSql == null) return -1;
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// CA(查询新医嘱加密记录)
        /// </summary>
        /// <param name="string"></param>
        /// <returns>0 success -1 fail</returns>
        public string[] QueryOrderSign(string patientno, string key)
        {
            #region 查询医嘱加密信息是否已存在
            //开立新医嘱
            //Order.Order.queryOrderSign.1
            //传入：71
            //			//传出：0 
            /*
             *  select CLINIC_NO,PRIVATE_INFO,CERTDATA,max(SIGN_TIME)
                   from fin_ca_sign
                  where CLINIC_NO = '{0}'
                  and CERT='{1}'
                  group by clinic_no,PRIVATE_INFO,CERTDATA
             */
            #endregion

            string strSql = "";
            string[] str = new string[3];
            ArrayList al = new ArrayList();
            if (this.Sql.GetCommonSql("Order.Order.queryOrderSign.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = string.Format(strSql, patientno, key);
            if (this.ExecQuery(strSql) < 0) return null;
            if (this.Reader.Read())
            {
                string patientNo = this.Reader[0].ToString();
                str[0] = this.Reader[1].ToString();
                str[1] = this.Reader[2].ToString();
                str[2] = this.Reader[3].ToString();

            }
            this.Reader.Close();


            return str;

        }
        /// <summary>
        /// 根据员工编号查询签章
        /// </summary>
        /// <param name="string"></param>
        /// <returns>0 success -1 fail</returns>
        public byte[] GetData(string employeeNO)
        {
            #region 根据员工编号查询签章
            //开立新医嘱
            //Order.Order.queryEmployeeData.1
            //传入：71
            //			//传出：0 
            #endregion

            string strSql = "";
            byte[] EmployeeData = new byte[64];
            if (this.Sql.GetCommonSql("Order.Order.queryEmployeeData.2", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = string.Format(strSql, employeeNO);

            if (this.ExecQuery(strSql) < 0) return null;


            if (this.Reader.Read())
            {
                EmployeeData = this.Reader[0] as byte[];

            }
            this.Reader.Close();


            return EmployeeData;

        }
        /// <summary>
        /// CA(查询新医嘱加密记录)
        /// </summary>
        /// <param name="string"></param>
        /// <returns>0 success -1 fail</returns>
        //public ArrayList addExecOrderSign(ArrayList al, string strSql)
        //{
        //    if (this.ExecQuery(strSql) == -1)
        //    {
        //        return null;
        //    }
        //     try
        //    {
        //        while (this.Reader.Read())
        //        {
        //            Neusoft.HISFC.Models.Order.Inpatient.Order objOrder = new Neusoft.HISFC.Models.Order.Inpatient.Order();
        //            #region "患者信息"
        //            //患者信息——  
        //            //			1 住院流水号   2住院病历号     3患者科室id      4患者护理id
        //            try
        //            {
        //                objOrder.Patient.ID = this.Reader[0].ToString();
        //                objOrder.orderEncrypt = this.Reader[1].ToString();

        //            }
        //            catch (Exception ex)
        //            {
        //                this.Err = "获得患者医嘱加密信息出错！" + ex.Message;
        //                this.WriteErr();
        //                return null;
        //            }
        //            #endregion


        //            al.Add(objOrder);
        //        }
        //        return al;
        //    }
        //     catch (Exception ex)
        //     {
        //         this.Err = "获得患者医嘱加密信息出错！" + ex.Message;
        //         this.WriteErr();
        //         return null;
        //     }
        //}

        /// <summary>
        /// 更新医嘱
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order)
        {
            #region 更新医嘱
            //更新医嘱
            //Order.Order.CreateOrder.1
            //传入：71
            //传出：0 
            #endregion
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Order.updateOrder.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = GetOrderInfo(strSql, order);
            if (strSql == null) return -1;
            return this.ExecNoQuery(strSql);
        }


        /// <summary>
        /// 删除医嘱
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int DeleteOrder(Neusoft.HISFC.Models.Order.Order order)
        {

            #region 删除医嘱
            //删除医嘱(医嘱未生效状态)
            //Order.Order.delOrder.1
            //传入：0 id
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.delOrder.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, order.ID);
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.delOrder.1";
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }


        /// <summary>
        /// 建立执行档(插入执行档记录)
        /// </summary>
        /// <param name="execOrder"></param>
        /// <returns>0 success -1 fail</returns>
        public int InsertExecOrder(Neusoft.HISFC.Models.Order.ExecOrder execOrder)
        {
            string strSql = "";
            string strItemType = "";

            strItemType = JudgeItemType(execOrder.Order);
            if (strItemType == "")
            {
                return -1;
            }

            #region "药嘱执行档"
            if (strItemType == "1")
            {
                Neusoft.HISFC.Models.Pharmacy.Item objPharmacy;
                objPharmacy = (Neusoft.HISFC.Models.Pharmacy.Item)execOrder.Order.Item;

                if (this.Sql.GetCommonSql("Order.ExecOrder.CreateExec.Drug.1", ref strSql) == -1)
                    return -1;

                #region 配液判断  愁啊愁 不想引用Manager

                if (execOrder.Order.OrderType.IsDecompose)      //对需要分解的医嘱进行如下处理
                {
                    if (Order.hsCompoundUsage == null)
                    {
                        Order.hsCompoundUsage = new Hashtable();

                        Neusoft.HISFC.BizLogic.Manager.Constant consManager = new Neusoft.HISFC.BizLogic.Manager.Constant();
                        consManager.SetTrans(this.Trans);

                        ArrayList alList = consManager.GetList("CompoundUsage");
                        if (alList == null)  //不进行错误返回
                        {
                            Order.hsCompoundUsage = new Hashtable();
                        }
                        foreach (Neusoft.HISFC.Models.Base.Const cons in alList)
                        {
                            Order.hsCompoundUsage.Add(cons.ID, null);
                        }
                    }

                    if (Order.hsCompoundUsage.ContainsKey(execOrder.Order.Usage.ID))
                    {
                        execOrder.Order.Compound.IsNeedCompound = true;
                    }
                }

                #endregion

                try
                {
                    string[] s ={
                                    execOrder.ID,
                                    execOrder.Order.Patient.ID,
                                    execOrder.Order.Patient.PID.PatientNO,
                                    execOrder.Order.Patient.PVisit.PatientLocation.Dept.ID,
                                    execOrder.Order.Patient.PVisit.PatientLocation.NurseCell.ID,
                                    strItemType,
                                    execOrder.Order.Item.ID,
                                    execOrder.Order.Item.Name,
                                    execOrder.Order.Item.UserCode,
                                    execOrder.Order.Item.SpellCode,
                                    execOrder.Order.Item.SysClass.ID.ToString(),
                                    execOrder.Order.Item.SysClass.Name,
                                    objPharmacy.Specs,
                                    objPharmacy.BaseDose.ToString(),
                                    objPharmacy.DoseUnit,
                                    objPharmacy.MinUnit,
                                    objPharmacy.PackQty.ToString(),
                                    objPharmacy.DosageForm.ID,
                                    objPharmacy.Type.ID,
                                    objPharmacy.Quality.ID.ToString(),
                                    objPharmacy.Price.ToString(),
                                    execOrder.Order.Usage.ID,
                                    execOrder.Order.Usage.Name,
                                    execOrder.Order.Usage.Memo,
                                    execOrder.Order.Frequency.ID,
                                    execOrder.Order.Frequency.Name,
                                    execOrder.Order.DoseOnce.ToString(),
                                    execOrder.Order.Qty.ToString(),
                                    execOrder.Order.Unit,
                                    execOrder.Order.HerbalQty.ToString(),
                                    execOrder.Order.OrderType.ID,
                                    execOrder.Order.ID,
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.OrderType.IsDecompose).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.OrderType.IsCharge).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.OrderType.IsNeedPharmacy).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.OrderType.IsPrint).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.Item.IsNeedConfirm).ToString(),
                                    execOrder.Order.ReciptDoctor.ID,
                                    execOrder.Order.ReciptDoctor.Name,
                                    execOrder.DateUse.ToString(),
                                    execOrder.DCExecOper.OperTime.ToString(),
                                    execOrder.Order.ReciptDept.ID,
                                    execOrder.Order.BeginTime.ToString(),
                                    execOrder.DCExecOper.ID,
                                    execOrder.ChargeOper.ID,
                                    execOrder.ChargeOper.Dept.ID,
                                    execOrder.ChargeOper.OperTime.ToString(),
                                    execOrder.Order.StockDept.ID,
                                    execOrder.Order.ExeDept.ID,
                                    execOrder.Order.ExecOper.ID,
                                    execOrder.ExecOper.Dept.ID,
                                    execOrder.ExecOper.OperTime.ToString(),
                                    execOrder.DateDeco.ToString(),
                                    execOrder.Order.BabyNO.ToString(),
                                    execOrder.Order.Combo.ID,
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.Combo.IsMainDrug).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.IsHaveSubtbl).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsValid).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.IsStock).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsExec).ToString(),
                                    execOrder.DrugFlag.ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsCharge).ToString(),
                                    execOrder.Order.Note,
                                    execOrder.Order.Memo,
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.IsBaby).ToString(),
                                    execOrder.Order.ReciptNO,
                                    execOrder.Order.SequenceNO.ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.Compound.IsNeedCompound).ToString()
                                };

                    strSql = string.Format(strSql, s);
                }
                catch (Exception ex)
                {
                    this.Err = "付数值时候出错！" + ex.Message;
                    this.WriteErr();
                    return -1;
                }
            }

            #endregion

            #region "非药嘱执行档"

            else if (strItemType == "2")
            {
                Neusoft.HISFC.Models.Fee.Item.Undrug undrug = (Neusoft.HISFC.Models.Fee.Item.Undrug)execOrder.Order.Item;

                if (this.Sql.GetCommonSql("Order.ExecOrder.CreateExec.Undrug.1", ref strSql) == -1)
                {
                    return -1;
                }

                try
                {
                    string[] s ={
                                    execOrder.ID,
                                    execOrder.Order.Patient.ID,
                                    execOrder.Order.Patient.PID.PatientNO,
                                    execOrder.Order.Patient.PVisit.PatientLocation.Dept.ID,
                                    execOrder.Order.Patient.PVisit.PatientLocation.NurseCell.ID,
                                    strItemType,
                                    execOrder.Order.Item.ID,
                                    execOrder.Order.Item.Name,
                                    execOrder.Order.Item.UserCode,
                                    execOrder.Order.Item.SpellCode,
                                    execOrder.Order.Item.SysClass.ID.ToString(),
                                    execOrder.Order.Item.SysClass.Name,
                                    undrug.Specs,
                                    undrug.Price.ToString(),
                                    execOrder.Order.Usage.ID,
                                    execOrder.Order.Usage.Name,
                                    execOrder.Order.Usage.Memo,
                                    execOrder.Order.Frequency.ID,
                                    execOrder.Order.Frequency.Name,
                                    execOrder.Order.DoseOnce.ToString(),
                                    execOrder.Order.Qty.ToString(),
                                    execOrder.Order.Unit,
                                    execOrder.Order.HerbalQty.ToString(),
                                    execOrder.Order.OrderType.ID,
                                    execOrder.Order.ID,
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.OrderType.IsDecompose).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.OrderType.IsCharge).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.OrderType.IsNeedPharmacy).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.OrderType.IsPrint).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.Item.IsNeedConfirm).ToString(),
                                    execOrder.Order.ReciptDoctor.ID,
                                    execOrder.Order.ReciptDoctor.Name,
                                    execOrder.DateUse.ToString(),
                                    execOrder.DCExecOper.OperTime.ToString(),
                                    execOrder.Order.ReciptDept.ID,
                                    execOrder.Order.BeginTime.ToString(),
                                    execOrder.DCExecOper.ID,
                                    execOrder.ChargeOper.ID,
                                    execOrder.ChargeOper.Dept.ID,
                                    execOrder.ChargeOper.OperTime.ToString(),
                                    execOrder.Order.StockDept.ID,
                                    execOrder.Order.ExeDept.ID,
                                    execOrder.ExecOper.ID,
                                    execOrder.ExecOper.Dept.ID,
                                    execOrder.ExecOper.OperTime.ToString(),
                                    execOrder.DateDeco.ToString(),
                                    execOrder.Order.ExeDept.Name,
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.IsBaby).ToString(),
                                    execOrder.Order.BabyNO.ToString(),
                                    execOrder.Order.Combo.ID,
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.Combo.IsMainDrug).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.IsSubtbl).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.IsHaveSubtbl).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsValid).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsExec).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsCharge).ToString(),
                                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.Order.IsEmergency).ToString(),
                                    execOrder.Order.CheckPartRecord,
                                    execOrder.Order.Note,
                                    execOrder.Order.Memo,
                                    execOrder.Order.ReciptNO,
                                    execOrder.Order.SequenceNO.ToString(),
                                    execOrder.Order.Sample.Name,
                                    execOrder.IsConfirm?"1":"0"/*确认标记{DA77B01B-63DF-4559-B264-798E54F24ABB}*/,
                                    execOrder.Order.ApplyNo
                                };

                    strSql = string.Format(strSql, s);
                }
                catch (Exception ex)
                {
                    this.Err = "付数值时候出错！" + ex.Message;
                    this.WriteErr();
                    return -1;
                }
            }
            #endregion

            if (strSql == null)
            {
                return -1;
            }

            return this.ExecNoQuery(strSql);
        }

        #endregion

        #region "医嘱处理"

        #region 医嘱操作
        /// <summary>
        /// 将该条已停止的医嘱所附带的附材也停止
        /// </summary>
        /// <param name="orderIDs"></param>
        /// <returns></returns>
        public int StopOrder(List<string> orderIDs)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("Order.Order.StopOrder", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
            }
            foreach (string orderID in orderIDs)
            {
                string sql = string.Format(strSql, orderID);
                if (this.ExecNoQuery(sql) < 0)
                    return -1;
            }
            return 1;
        }


        /// <summary>
        /// 保存医嘱 -根据Order.ID =="" or =="-1" 是新的 插入，其他的更新
        /// 设置医嘱
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        public int SetOrder(Neusoft.HISFC.Models.Order.Inpatient.Order Order)
        {
            if (Order.ID == "" || Order.ID == "-1")
            {
                string s = this.GetNewOrderID();
                if (s == null || s == "-1") return -1;
                Order.ID = s;
                return this.InsertOrder(Order);
            }
            else
            {
                return this.UpdateOrder(Order);
            }
        }

        /// <summary>
        /// 停止医嘱
        /// Order.Status = 1预停止;Order.Status = 3直接停止
        /// </summary>
        /// <param name="Order">医嘱信息</param>
        /// <returns>0 success -1 fail</returns>
        public int DcOneOrder(Neusoft.HISFC.Models.Order.Order Order)
        {
            #region 停止医嘱
            //停止医嘱(医嘱已生效状态)
            //Order.Order.dcOrder.1
            //传入：0 id，1 停止人id,2停止人姓名，3停止时间,4医嘱状态 ,5停止原因代码，6停止原因名称 
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.dcOrder.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                if (Order.EndTime == DateTime.MinValue)//判断停止时间
                    Order.EndTime = this.GetDateTimeFromSysDateTime();

                strSql = string.Format(strSql, Order.ID, Order.DCOper.ID, Order.DCOper.Name, Order.EndTime.ToString(), Order.Status.ToString(), Order.DcReason.ID, Order.DcReason.Name);
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.dcOrder.1";
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 停止医嘱
        /// Order.Status = 1预停止;Order.Status = 3直接停止
        /// </summary>
        /// <param name="Order">医嘱信息</param>
        /// <returns>0 success -1 fail</returns>
        public int NoFeeDcOneOrder(Neusoft.HISFC.Models.Order.Order Order)
        {
            #region 停止医嘱
            //停止医嘱(医嘱已生效状态)
            //Order.Order.dcOrder.1
            //传入：0 id，1 停止人id,2停止人姓名，3停止时间,4医嘱状态 ,5停止原因代码，6停止原因名称 
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.dcOrder.2", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                if (Order.EndTime == DateTime.MinValue)//判断停止时间
                    Order.EndTime = this.GetDateTimeFromSysDateTime();

                strSql = string.Format(strSql, Order.ID, Order.DCOper.ID, Order.DCOper.Name, Order.EndTime.ToString(), Order.Status.ToString(), Order.DcReason.ID, Order.DcReason.Name);
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.dcOrder.2";
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 删除医嘱附材
        /// </summary>
        /// <param name="ComboID"></param>
        /// <returns></returns>
        public int DeleteOrderSubtbl(string ComboID)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.delOrder.2", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, ComboID);
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.delOrder.2";
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 删除凤凰园医保购药审批申请
        /// </summary>
        /// <param name="sequence_no">医嘱组合号</param>
        /// <returns></returns>
        public int DeleteCheckDrugBySeqNoFHY(string sequence_no)
        {
            string sql = "delete met_ord_outsidedrug t where t.sequence_no={0} and t.state='0'";

            sql = string.Format(sql, sequence_no);

            return this.ExecNoQuery(sql);
        }
        /// <summary>
        /// '组合
        ///将医嘱组合成组合医嘱一起执行
        ///条件：usage用法相同
        ///      frq  频次相同
        /// </summary>
        /// <param name="alOrder"></param>
        /// <returns></returns>
        public int ComboOrder(ArrayList alOrder)
        {
            string strUsage = "", strFrq = "";
            string strSql = "";
            string strCombNo = "";
            #region 组合
            //组合
            //Order.Order.ComboOrder.1
            //传入：0 orderid 1组合号 2是否主药
            //传出：0 
            #endregion

            if (alOrder == null)
            {
                return -1;
            }

            strCombNo = this.GetNewOrderComboID();
            if (strCombNo == "" || strCombNo == null)
            {
                this.Err = Neusoft.FrameWork.Management.Language.Msg("医嘱组合号为空，不能组合！");
                return -1;
            }
            for (int i = 0; i < alOrder.Count; i++)
            {
                Neusoft.HISFC.Models.Order.Order objOrder = new Neusoft.HISFC.Models.Order.Order();
                objOrder = (Neusoft.HISFC.Models.Order.Order)alOrder[i];
                if (i == 0)
                {
                    strUsage = objOrder.Usage.ID;
                    strFrq = objOrder.Frequency.ID;
                }
                if (strUsage != objOrder.Usage.ID)
                {
                    this.Err = Neusoft.FrameWork.Management.Language.Msg(objOrder.SubCombNO.ToString() + "组  " + objOrder.Item.Name + "[" + objOrder.Item.Specs + "]" + "用法不一致");
                    return -1;
                }
                if (strFrq != objOrder.Frequency.ID)
                {
                    this.Err = objOrder.Item.Name + Neusoft.FrameWork.Management.Language.Msg(objOrder.SubCombNO.ToString() + "组  " + objOrder.Item.Name + "[" + objOrder.Item.Specs + "]" + "频次不一致");
                    return -1;
                }

                if (this.Sql.GetCommonSql("Order.Order.ComboOrder.1", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, objOrder.ID, strCombNo, Neusoft.FrameWork.Function.NConvert.ToInt32(objOrder.Combo.IsMainDrug).ToString());
                }
                catch
                {
                    this.Err = "传入参数不对！Order.Order.ComboOrder.1";
                    return -1;
                }
                if (this.ExecNoQuery(strSql) <= 0)
                {
                    return -1;
                }
            }
            return 0;
        }

        #endregion

        #region "查询医嘱"

        /// <summary>
        /// 抗生素查询患者手术信息
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        //public ArrayList QueryOrderOpera(string inpatientNo)
        //{
        //    string sqlStr = string.Empty;
        //    if (this.Sql.GetCommonSql("", ref sqlStr) == -1)
        //    {
        //        this.Err = "没有找到字段";
        //        return null;
        //    }
        //    sqlStr = string.Format(sqlStr, inpatientNo);
        //    return this.PatientOpera(sqlStr);
        //}

        /// <summary>
        /// 查询所有医嘱
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public ArrayList QueryOrder(string inpatientNO)
        {
            #region 查询所有医嘱
            //查询所有医嘱
            //Order.Order.QueryOrder.1
            //传入：0 inpatientno
            //传出：ArrayList
            #endregion

            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO);
            return this.MyOrderQuery(sql);
        }

        public bool IsExistHisOrder(string inpatientNO)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Order.Order.IsExistHisOrder", ref sql) == -1)
            {
                this.Err = "Order.Order.IsExistHisOrder!";
                return false;
            }
            sql = string.Format(sql, inpatientNO);
            string result = this.ExecSqlReturnOne(sql, "");
            if (string.IsNullOrEmpty(result) || result == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 查询所有医嘱 Allan
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.Order.Inpatient.Order> QueryOrderLists(string inpatientNO, DateTime dtBegin)
        {
            #region 查询所有医嘱
            //查询所有医嘱
            //Order.Order.QueryOrder.1
            //传入：0 inpatientno
            //传出：ArrayList
            #endregion

            string sql = "", sql1 = "";
            List<Neusoft.HISFC.Models.Order.Inpatient.Order> al = new List<Neusoft.HISFC.Models.Order.Inpatient.Order>();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.1.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, dtBegin.ToString("yyyy-MM-dd HH:mm:ss"));
            return this.MyOrderQueryLists(sql);
        }
        //add by allan 
        public List<Neusoft.HISFC.Models.Order.Inpatient.Order> QueryOrderList(string inpatientNO)
        {
            #region 查询所有医嘱
            //查询所有医嘱
            //Order.Order.QueryOrder.1
            //传入：0 inpatientno
            //传出：ArrayList
            #endregion

            string sql = "", sql1 = "";
            List<Neusoft.HISFC.Models.Order.Inpatient.Order> lists = new List<Neusoft.HISFC.Models.Order.Inpatient.Order>();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO);
            return this.MyOrderQueryLists(sql);
        }


        public ArrayList QueryOrderEX(string inpatientNO, string RecipeNOS)
        {


            string strSelect = "";  //获得全部药品信息的SELECT语句
            string strWhere = "";
            string strRecipeNO = "";

            //取SELECT语句
            if (this.Sql.GetCommonSql("Pharmacy.InPatientPoisonList.Info", ref strSelect) == -1)
            {
                this.Err = "没有找到Pharmacy.Item.Info字段!";
                return null;
            }

            //取Where语句
            if (this.Sql.GetCommonSql("Pharmacy.InPatientPoisonList.Where", ref strWhere) == -1)
            {
                this.Err = "没有找到Pharmacy.Item.Info字段!";
                return null;
            }

            RecipeNOS = RecipeNOS.Trim();
            if (RecipeNOS == "")
            {
                strRecipeNO = "'AAAA'";
                RecipeNOS = "'AAAA'";
            }
            else
            {
                strRecipeNO = "'AA'";
            }
            strSelect = strSelect + " " + strWhere;
            strSelect = string.Format(strSelect, inpatientNO, strRecipeNO, RecipeNOS);

            return this.MyOrderQueryEX(strSelect);
        }

        /// <summary>
        /// 私有函数，查询医嘱信息
        /// </summary>
        /// <param name="SQLPatient"></param>
        /// <returns></returns>
        private ArrayList MyOrderQueryEX(string SQLPatient)
        {
            ArrayList al = new ArrayList();

            if (this.ExecQuery(SQLPatient) == -1)
            {
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Order.Inpatient.Order objOrder = new Neusoft.HISFC.Models.Order.Inpatient.Order();
                    #region "患者信息"
                    //患者信息——  
                    //			1 住院流水号   2住院病历号     3患者科室id      4患者护理id
                    try
                    {
                        objOrder.ID = this.Reader[0].ToString();
                        objOrder.Patient.ID = this.Reader[1].ToString();
                        objOrder.Patient.PID.PatientNO = this.Reader[2].ToString();
                        objOrder.Patient.PVisit.PatientLocation.Dept.ID = this.Reader[3].ToString();
                        objOrder.InDept.ID = this.Reader[3].ToString();
                        objOrder.Patient.PVisit.PatientLocation.NurseCell.ID = this.Reader[4].ToString();

                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得患者基本信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    #region "项目信息"
                    //医嘱辅助信息
                    // ——项目信息
                    //	       5项目类别      6项目编码       7项目名称      8项目输入码,    9项目拼音码 
                    //	       10项目类别代码 11项目类别名称  12药品规格     13药品基本剂量  14剂量单位       
                    //         15最小单位     16包装数量,     17剂型代码     18药品类别  ,   19药品性质
                    //         20零售价       21用法代码      22用法名称     23用法英文缩写  24频次代码  
                    //         25频次名称     26每次剂量      27项目总量     28计价单位      29使用天数			  
                    // 判断药品/非药品
                    //         25频次名称     26每次剂量      27项目总量     28计价单位      29使用天数			  
                    // 73 样本类型 名称

                    Neusoft.HISFC.Models.Pharmacy.Item objPharmacy = new Neusoft.HISFC.Models.Pharmacy.Item();
                    try
                    {
                        objPharmacy.ID = this.Reader[6].ToString();
                        objPharmacy.Name = this.Reader[7].ToString();
                        objPharmacy.UserCode = this.Reader[8].ToString();
                        objPharmacy.SpellCode = this.Reader[9].ToString(); ;
                        objPharmacy.Specs = this.Reader[12].ToString();

                        if (!this.Reader.IsDBNull(13))
                            objPharmacy.BaseDose = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());

                        objPharmacy.DoseUnit = this.Reader[14].ToString();
                        objOrder.DoseUnit = this.Reader[14].ToString();
                        objPharmacy.MinUnit = this.Reader[15].ToString();

                        if (!this.Reader.IsDBNull(16))
                            objPharmacy.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[16].ToString());

                        objPharmacy.DosageForm.ID = this.Reader[17].ToString();
                        objPharmacy.Type.ID = this.Reader[18].ToString();
                        objPharmacy.Quality.ID = this.Reader[19].ToString();
                        // 计价单位
                        objPharmacy.PriceUnit = this.Reader[28].ToString();

                        //try
                        //{
                        if (!this.Reader.IsDBNull(20)) objPharmacy.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20].ToString());
                        //}
                        //catch{}					
                        objOrder.Usage.ID = this.Reader[21].ToString();
                        objOrder.Usage.Name = this.Reader[22].ToString();
                        objOrder.Usage.Memo = this.Reader[23].ToString();
                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得医嘱项目信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    objOrder.Item = objPharmacy;
                    objOrder.ExtendFlag2 = this.Reader[19].ToString();
                    objOrder.Frequency.ID = this.Reader[24].ToString();
                    objOrder.Frequency.Name = this.Reader[25].ToString();
                    //try
                    //{
                    if (!this.Reader.IsDBNull(26)) objOrder.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26].ToString());//}
                    //catch{}
                    //try
                    //{
                    if (!this.Reader.IsDBNull(27)) objOrder.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27].ToString());//}
                    //catch{}
                    objOrder.Unit = this.Reader[28].ToString();
                    //try
                    //{
                    if (!this.Reader.IsDBNull(29)) objOrder.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[29].ToString());//}
                    //catch{}

                    #endregion

                    #region "执行情况"
                    // ——执行情况
                    //		   37开立医师Id   38开立医师name  39开始时间      40结束时间     41开立科室
                    //		   42开立时间     43录入人员代码  44录入人员姓名  45审核人ID     46审核时间       
                    //		   47DC原因代码   48DC原因名称    49DC医师代码    50DC医师姓名   51Dc时间
                    //         52执行人员代码 53执行时间      54执行科室代码  55执行科室名称 
                    //		   56本次分解时间 57下次分解时间  58医嘱状态
                    try
                    {
                        objOrder.ReciptDoctor.ID = this.Reader[37].ToString();
                        objOrder.ReciptDoctor.Name = this.Reader[38].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(39)) objOrder.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[39].ToString());
                        //}
                        //catch{}
                        //try{
                        if (!this.Reader.IsDBNull(40)) objOrder.EndTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[40].ToString());
                        //}
                        //catch{}
                        objOrder.ReciptDept.ID = this.Reader[41].ToString();//InDept.ID
                        //try{
                        if (!this.Reader.IsDBNull(42)) objOrder.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[42].ToString());
                        //}
                        //catch{}
                        objOrder.Oper.ID = this.Reader[43].ToString();
                        objOrder.Oper.Name = this.Reader[44].ToString();
                        objOrder.Nurse.ID = this.Reader[45].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(46)) objOrder.ConfirmTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[46].ToString());
                        //}
                        //catch{}
                        objOrder.DcReason.ID = this.Reader[47].ToString();
                        objOrder.DcReason.Name = this.Reader[48].ToString();
                        objOrder.DCOper.ID = this.Reader[49].ToString();
                        objOrder.DCOper.Name = this.Reader[50].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(51))
                            objOrder.DCOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[51].ToString());
                        //}
                        //catch{}
                        objOrder.ExecOper.ID = this.Reader[52].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(53)) objOrder.ExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[53].ToString());

                        objOrder.ExeDept.ID = this.Reader[54].ToString();
                        objOrder.ExeDept.Name = this.Reader[55].ToString();

                        objOrder.ExecOper.Dept.ID = this.Reader[54].ToString();
                        objOrder.ExecOper.Dept.Name = this.Reader[55].ToString();

                        if (!this.Reader.IsDBNull(56)) objOrder.CurMOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[56].ToString());

                        if (!this.Reader.IsDBNull(57)) objOrder.NextMOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[57].ToString());

                        if (!this.Reader.IsDBNull(58))
                        {
                            objOrder.Status = Convert.ToInt32(this.Reader[58].ToString());//记录打印状态
                        }
                        else
                        {
                            objOrder.Status = 0;//记录打印状态
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得医嘱执行情况信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion
                    al.Add(objOrder);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得医嘱信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();
            return al;
        }

        /// <summary>
        /// 跟新手术批费药品打印标志
        /// 
        /// </summary>
        /// <param name="execOrder">执行档信息</param>
        /// <returns>0 success -1 fail</returns>
        public int UpdatePrintFlag(string RecipeNO, string DrugID)
        {
            #region 配药记录
            //配药记录
            //fin_ipb_medicinelist.ext_flag1
            //传入：RecipeNO 
            #endregion
            string strSql = "";


            if (this.Sql.GetCommonSql("Order.SurgeryItem.UpdateFlag", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, RecipeNO, DrugID);
            }
            catch (Exception ex)
            {
                this.Err = "Order.SurgeryItem.UpdateFlag" + ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 返回同一个组合医嘱数量
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="combno"></param>
        /// <returns></returns>
        public int QueryOrderCountByCombno(string inpatientNO, string combno)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            if (sql == null) return 0;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.CombCount", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.CombCount字段!";
                return 0;
            }
            sql = string.Format(sql1, inpatientNO, combno);
            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
        }

        /// <summary>
        /// 查询所有医嘱的附材
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryOrderSubtbl(string inpatientNO)
        {
            #region 查询所有医嘱的附材
            //查询所有医嘱
            //Order.Order.QueryOrder.1
            //传入：0 inpatientno
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.Sub.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO);
            return this.MyOrderQuery(sql); ;
        }

        public List<Neusoft.HISFC.Models.Order.Inpatient.Order> QueryOrderSubtblList(string inpatientNO)
        {
            #region 查询所有医嘱的附材
            //查询所有医嘱
            //Order.Order.QueryOrder.1
            //传入：0 inpatientno
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            List<Neusoft.HISFC.Models.Order.Inpatient.Order> lists = new List<Neusoft.HISFC.Models.Order.Inpatient.Order>();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.Sub.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO);
            return this.MyOrderQueryLists(sql); ;
        }

        /// <summary>
        /// 查询所有医嘱的附材
        /// </summary>
        /// <returns></returns>
        public List<Neusoft.HISFC.Models.Order.Inpatient.Order> QueryOrderSubtblLists(string inpatientNO, DateTime dtBegin)
        {
            #region 查询所有医嘱的附材
            //查询所有医嘱
            //Order.Order.QueryOrder.1
            //传入：0 inpatientno
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            List<Neusoft.HISFC.Models.Order.Inpatient.Order> al = new List<Neusoft.HISFC.Models.Order.Inpatient.Order>();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.Sub.2", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, dtBegin.ToString("yyyy-MM-dd HH:mm:ss"));
            return this.MyOrderQueryLists(sql); ;
        }

        /// <summary>
        /// 查找某一个患者的有效附材 {24F859D1-3399-4950-A79D-BCCFBEEAB939}
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <returns></returns>
        public ArrayList QueryOrdeSub(string inpatientNO, string itemcode)
        {
            #region 查询所有医嘱
            //查询所有医嘱
            //Order.Order.QueryOrder.1
            //传入：0 inpatientno
            //传出：ArrayList
            #endregion

            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.6", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, itemcode);
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 按状态查询医嘱
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ArrayList QueryOrder(string inpatientNO, int status)
        {
            return this.QueryOrderByState(inpatientNO, status.ToString());
            #region 按状态查询医嘱
            //按状态查询医嘱
            //Order.Order.QueryOrder.2
            //传入：0 inpatientno 2 status
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.2", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.2字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, "'" + status.ToString() + "'");
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 根据状态查询医嘱，可以多个状态
        /// </summary>
        /// <param name="inPateintNo"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ArrayList QueryOrderByState(string inPateintNo, string state)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.2", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.2字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inPateintNo, state);
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 查询审核的非附材医嘱
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="status"></param>
        /// <param name="isSubtbl"></param>
        /// <returns></returns>
        public ArrayList QueryOrder(string inpatientNO, int status, bool isSubtbl)
        {
            #region 按状态查询医嘱
            //按状态查询医嘱
            //Order.Order.QueryOrder.2
            //传入：0 inpatientno 2 status
            //传出：ArrayList
            #endregion

            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.ForConfirmQuery", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.ForConfirmQuery字段!";
                return null;
            }
            string flag = "";
            if (isSubtbl)
            {
                flag = "1";
            }
            else
            {
                flag = "0";
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, status.ToString(), flag);
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 按开立时间查询医嘱
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ArrayList QueryOrder(string inpatientNO, DateTime beginTime, DateTime endTime)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            #region 按开立时间查询医嘱
            //按开立时间查询医嘱
            //Order.Order.QueryOrder.3
            //传入：0 inpatientno 1BeginTime 2EndTime
            //传出：ArrayList
            #endregion

            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.3", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.3字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, beginTime, endTime);
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 检索有效需要打印的患者巡回卡信息
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="usage"></param>
        /// <param name="isPrint"></param>
        /// <returns></returns>
        public ArrayList QueryCircuitCard(string inpatientNO, string usage, bool isPrint)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            #region 检索有效需要打印的患者巡回卡信息
            //检索有效需要打印的患者巡回卡信息
            //传入：0 inpatientno 1Usage, 2 Isprint
            //传出：ArrayList
            #endregion

            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryCircuitCard.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryCircuitCard.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, usage, Neusoft.FrameWork.Function.NConvert.ToInt32(isPrint).ToString());
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 按医嘱类型查询医嘱
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ArrayList QueryOrder(string inpatientNO, string type)
        {
            #region 按医嘱类型查询医嘱
            //按医嘱类型查询医嘱
            //Order.Order.QueryOrder.4
            //传入：0 inpatientno 1Type
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.4", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.4字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, type);
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 查询是否审核的医嘱 - 不包括附材
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="type"></param>
        /// <param name="recipeDept">开立科室过滤</param>
        /// <param name="isConfirmed"></param>
        /// <returns></returns>
        public ArrayList QueryIsConfirmOrder(string inpatientNO, Neusoft.HISFC.Models.Order.EnumType type, string recipeDept, bool isConfirmed)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.Order.QueryIsConfirmOrder.where.isConfirm", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryIsConfirmOrder.where.isConfirm字段!";
                return null;
            }
            string strType = "1";
            if (type == Neusoft.HISFC.Models.Order.EnumType.LONG)
            {
                strType = "1";
            }
            else
            {
                strType = "0";
            }
            try
            {
                sql = sql + " \r\n" + string.Format(sql1, inpatientNO, Neusoft.FrameWork.Function.NConvert.ToInt32(isConfirmed).ToString(), strType, recipeDept);
            }
            catch
            {
                return null;
            }
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 查询出院带药单
        /// </summary>
        /// <param name="inpatientNO">患者流水号</param>
        /// <returns></returns>
        public System.Data.DataSet QueryOutHosDrug(string inpatientNO)
        {
            string sql = "Order.Order.Query.QueryOutHosDrug";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            sql = string.Format(sql, inpatientNO);
            System.Data.DataSet ds = new System.Data.DataSet();
            if (this.ExecQuery(sql, ref ds) == -1) return null;
            return ds;
        }
        /// <summary>
        /// 查询请假带药单
        /// </summary>
        /// <param name="inpatientNO">患者流水号</param>
        /// <returns></returns>
        public System.Data.DataSet QueryTempOutHosDrug(string inpatientNO)
        {
            string sql = "Order.Order.Query.QueryTempOutHosDrug";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            sql = string.Format(sql, inpatientNO);
            System.Data.DataSet ds = new System.Data.DataSet();
            if (this.ExecQuery(sql, ref ds) == -1) return null;
            return ds;
        }
        /// <summary>
        /// 查询出院带疗项目
        /// </summary>
        /// <param name="inpatientNO">患者流水号</param>
        /// <returns></returns>
        public ArrayList QueryOutHosCure(string inpatientNO)
        {
            string sql = "Order.Order.Query.QueryOutHosCure";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            sql = string.Format(sql, inpatientNO);
            ArrayList alCure = new ArrayList();
            try
            {
                alCure = this.myGetOutHosCure(sql);
            }
            catch
            {
                return null;
            }
            return alCure;
        }

        /// <summary>
        /// 按医嘱流水号查询医嘱信息-不分有效作废
        /// </summary>
        /// <param name="OrderNO"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.Inpatient.Order QueryOneOrder(string OrderNO)
        {
            string sql = "", sql1 = "";
            ArrayList al = null;
            #region 按医嘱流水号查询医嘱信息
            //按医嘱流水号查询医嘱信息
            //Order.Order.QueryOrder.5
            //传入：0 OrderNo
            //传出：ArrayList
            #endregion
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.5", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.5字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, OrderNO);
            al = this.MyOrderQuery(sql);
            if (al == null || al.Count == 0 || al.Count > 1) return null;
            return al[0] as Neusoft.HISFC.Models.Order.Inpatient.Order;
        }

        /// <summary>
        /// 通过医嘱号查询医嘱状态
        /// </summary>
        /// <param name="OrderNO"></param>
        /// <returns></returns>
        public int QueryOneOrderState(string OrderNO)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Order.Order.QueryOneOrderState.1", ref sql) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOneOrderState.1字段!";
                return -1;
            }
            try
            {
                sql = string.Format(sql, OrderNO);
            }
            catch
            {
                this.Err = "付数值出错！";
                this.WriteErr();
                return -1;
            }

            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
        }

        /// <summary>
        /// 按医嘱类型（长期/临时），状态）查询医嘱（不包含附材）
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ArrayList QueryOrder(string inpatientNO, Neusoft.HISFC.Models.Order.EnumType type, int status)
        {
            #region 按医嘱类型（长期/临时），状态）查询医嘱（不包含附材）
            //按医嘱类型（长期/临时），状态）查询医嘱（不包含附材）
            //Order.Order.QueryOrder.where.6
            //传入：0 inpatientno 1 Type 2 status
            //传出：ArrayList
            #endregion

            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.where.6", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.where.6字段!";
                return null;
            }
            string strType = "1";
            if (type == Neusoft.HISFC.Models.Order.EnumType.LONG)
            {
                strType = "1";
            }
            else
            {
                strType = "0";
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, strType, status.ToString());
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 查询是否审核的医嘱 - 不包括附材
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="type"></param>
        /// <param name="isConfirmed"></param>
        /// <returns></returns>
        public ArrayList QueryIsConfirmOrder(string inpatientNO, Neusoft.HISFC.Models.Order.EnumType type, bool isConfirmed)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryIsConfirmOrder.where.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryIsConfirmOrder.where.1字段!";
                return null;
            }
            string strType = "1";
            if (type == Neusoft.HISFC.Models.Order.EnumType.LONG)
            {
                strType = "1";
            }
            else
            {
                strType = "0";
            }
            try
            {
                sql = sql + " " + string.Format(sql1, inpatientNO, Neusoft.FrameWork.Function.NConvert.ToInt32(isConfirmed).ToString(), strType);
            }
            catch { return null; }
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 查询是否审核的医嘱 - 不包括附材
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="type"></param>
        /// <param name="isConfirmed"></param>
        /// <returns></returns>
        public ArrayList QueryIsConfirmOrder(string inpatientNO, Neusoft.HISFC.Models.Order.EnumType type, bool isConfirmed, string nurseCode)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            //查询是否是介入手术区
            string sql3 = string.Format("SELECT d.code FROM com_dictionary d where d.type='OpenOrderDepd' and d.valid_state = '1' and d.code = '{0}'", nurseCode);
            string result = Sql.ExecSqlReturnOne(sql3, "");
            if (string.IsNullOrEmpty(result))
            {
                if (this.Sql.GetCommonSql("Order.Order.QueryIsConfirmOrder.where.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.Order.QueryIsConfirmOrder.where.1字段!";
                    return null;
                }
            }
            else
            {
                if (this.Sql.GetCommonSql("Order.Order.QueryIsConfirmOrder.where.1.New", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.Order.QueryIsConfirmOrder.where.1.New字段!";
                    return null;
                }
            }
            string strType = "1";
            if (type == Neusoft.HISFC.Models.Order.EnumType.LONG)
            {
                strType = "1";
            }
            else
            {
                strType = "0";
            }
            try
            {
                sql = sql + " " + string.Format(sql1, inpatientNO, Neusoft.FrameWork.Function.NConvert.ToInt32(isConfirmed).ToString(), strType);
            }
            catch { return null; }
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 查询所有医嘱
        /// </summary>
        /// <param name="InPatientNO"></param>
        /// <returns></returns>
        public ArrayList QueryDcOrder(string InPatientNO)
        {
            #region 查询所有医嘱
            //查询所有医嘱
            //Order.Order.QueryOrder.1
            //传入：0 inpatientno
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.OrderPrint", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.1字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, InPatientNO);
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 按医嘱状态,停止审核标志 查询医嘱（不包含附材）
        /// 停止未审核医嘱查询
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="isConfirm"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ArrayList QueryDcOrder(string inpatientNO, int status, bool isConfirm)
        {
            #region  按医嘱状态,审核标志 查询医嘱（不包含附材）
            // 按医嘱状态,审核标志 查询医嘱（不包含附材）
            //Order.Order.QueryDcOrder.where.1
            //传入：0 inpatientno 1 status 2 IsConfirm
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryDcOrder.where.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryDcOrder.where.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, status.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isConfirm));
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 查询有效的医嘱
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ArrayList QueryValidOrderWithSubtbl(string inpatientNO, Neusoft.HISFC.Models.Order.EnumType type, string recipeDept)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrderWithSubtbl.where.vilidOrder", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrderWithSubtbl.where.vilidOrder字段!";
                return null;
            }
            string strType = "1";
            if (type == Neusoft.HISFC.Models.Order.EnumType.LONG)
            {
                strType = "1";
            }
            else
            {
                strType = "0";
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, strType, recipeDept);
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 获得医嘱项目的附材信息(组合号）
        /// </summary>
        /// <param name="combNo"></param>
        /// <returns></returns>
        public ArrayList QuerySubtbl(string combNo)
        {
            #region 获得医嘱项目的附材信息(组合号）
            //获得医嘱项目的附材信息(组合号）
            //Order.Order.QueryOrder.where.7
            //传入：0 inpatientno 1 CombNo 
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.where.7", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.where.7字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, combNo);
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 从非药品执行档查找某个非药品当天是否存在执行记录
        /// {24F859D1-3399-4950-A79D-BCCFBEEAB939}
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="undrugID"></param>
        /// <param name="deptID"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrderSubtblCurrentDay(string inpatientNo, string undrugID, string deptID)
        {

            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect("2");

            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryExecOrderSubtblCurrentDay", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryExecOrderBySubtblFeeMode.1字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo, undrugID, deptID);
                addExecOrder(al, sql);
            }
            return al;
        }
        /// <summary>
        /// 获得医嘱皮试信息
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns>-1错误 1不需要皮试/2需要皮试，未做/3皮试阳/4皮试阴</returns>
        public int QueryOrderHypotest(string orderNo)
        {
            #region 获得医嘱皮试信息
            //Order.Order.QueryOrderHypotest.1
            //传入：0 OrderNo 
            //传出：int 1不需要皮试/2需要皮试，未做/3皮试阳/4皮试阴
            #endregion
            string sql = "";
            int Hypotest = -1;

            if (this.Sql.GetCommonSql("Order.Order.QueryOrderHypotest.1", ref sql) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrderHypotest.1字段!";
                return -1;
            }
            sql = string.Format(sql, orderNo);
            if (this.ExecQuery(sql) < 0) return -1;

            if (this.Reader.Read())
            {
                Hypotest = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[0]);
            }
            else
            {
                Hypotest = 1;
            }

            this.Reader.Close();

            return Hypotest;
        }

        /// <summary>
        /// 获得医嘱批注信息
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public string QueryOrderNote(string orderNo)
        {
            #region 获得医嘱反馈批注信息
            //Order.Order.QueryOrderNote.1
            //传入：0 OrderNo 
            //传出：string
            #endregion
            string sql = "";
            string Note = "";

            if (this.Sql.GetCommonSql("Order.Order.QueryOrderNote.1", ref sql) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrderNote.1字段!";
                return "";
            }
            sql = string.Format(sql, orderNo);
            if (this.ExecQuery(sql) < 0) return "";

            if (this.Reader.Read())
            {
                Note = this.Reader[0].ToString();
            }
            this.Reader.Close();

            return Note;
        }

        /// <summary>
        /// 按医嘱类型（长期/临时），状态）查询医嘱（包含附材）
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ArrayList QueryOrderWithSubtbl(string inpatientNO, Neusoft.HISFC.Models.Order.EnumType type, int status)
        {
            #region 按医嘱类型（长期/临时），状态）查询医嘱（包含附材）
            //按医嘱类型（长期/临时），状态）查询医嘱（包含附材）
            //Order.Order.QueryOrder.where.6
            //传入：0 inpatientno 1 Type 2 status
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrderWithSubtbl.where.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrderWithSubtbl.where.1字段!";
                return null;
            }
            string strType = "1";
            if (type == Neusoft.HISFC.Models.Order.EnumType.LONG)
            {
                strType = "1";
            }
            else
            {
                strType = "0";
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, strType, status.ToString());
            return this.MyOrderQuery(sql);
        }
        /// <summary>
        /// 查询有效的医嘱
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ArrayList QueryValidOrderWithSubtbl(string inpatientNO, Neusoft.HISFC.Models.Order.EnumType type)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrderWithSubtbl.where.2", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrderWithSubtbl.where.2字段!";
                return null;
            }
            string strType = "1";
            if (type == Neusoft.HISFC.Models.Order.EnumType.LONG)
            {
                strType = "1";
            }
            else
            {
                strType = "0";
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, strType);
            return this.MyOrderQuery(sql);
        }
        #endregion

        #region 流水号
        /// <summary>
        /// 获得医嘱流水号
        /// </summary>
        /// <returns></returns>
        public string GetNewOrderID()
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Management.Order.GetNewOrderID", ref sql) == -1) return null;
            string strReturn = this.ExecSqlReturnOne(sql);
            if (strReturn == "-1" || strReturn == "") return null;
            return strReturn;
        }

        public bool IsExistEmrOrder(string inpatientNo)
        {
            string sql = string.Format(@" select count(1) from met_ipm_order p where p.inpatient_no='{0}' and p.source_flag='H' ", inpatientNo);
            try
            {
                string strReturn = this.ExecSqlReturnOne(sql, "0");
                if (strReturn == "0" || strReturn == "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return false;
            }
        }

        public string GetOrderSourceFlag(string orderID)
        {
            string sql = string.Format(@" select p.source_flag from met_ipm_order p where p.mo_order='{0}' ", orderID);
            try
            {
                string result = this.ExecSqlReturnOne(sql, "");
                return result;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return "";
            }
        }

        /// <summary>
        /// 获得医嘱执行流水号
        /// </summary>
        /// <returns></returns>
        public string GetNewOrderExecID()
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Management.Order.GetNewOrderExecID", ref sql) == -1) return null;
            string strReturn = this.ExecSqlReturnOne(sql);
            if (strReturn == "-1" || strReturn == "") return null;
            return strReturn;
        }
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
        #endregion

        #region "医嘱审核"

        /// <summary>   
        /// 更新医嘱反馈信息（护士批注，皮试结果）
        /// 更新皮试前请判断该标志是否需要皮试（1不需要皮试/2需要皮试，未做/3皮试阳/4皮试阴）
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="orderNo"></param>
        /// <param name="notes">批注</param>
        /// <param name="hypotest">皮试（1不需要皮试/2需要皮试，未做/3皮试阳/4皮试阴）</param>
        /// <returns></returns>
        public int UpdateFeedback(string inpatientNo, string orderNo, string notes, int hypotest)
        {
            #region 更新医嘱反馈信息
            //更新医嘱反馈信息
            //Order.Order.Updatefeedback.1
            //传入：0 inpatientNo,1orderID,2 NOTES,3 hypotest
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.Updatefeedback.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, inpatientNo, orderNo, notes, hypotest.ToString());
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.Updatefeedback.1";
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新医嘱执行标记
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public int UpdateOrderExecuted(string orderNo)
        {
            #region 更新医嘱执行情况
            //更新医嘱执行情况
            //Order.Order.UpdateExecOrder.1
            //传入：0 orderID 1 操作员 2 操作时间
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.UpdateExecOrder.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, orderNo, this.Operator.ID, this.GetSysDateTime().ToString());
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.UpdateExecOrder.1";
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新医嘱记帐情况
        /// 更新为记帐
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public int UpdateChargeOrder(string inpatientNo, string orderNo)
        {
            #region 更新医嘱记帐情况
            //更新医嘱记帐情况
            //Order.Order.UpdateChargeOrder.1
            //传入：0 inpatientNo,1orderID 2 操作员 3 操作时间
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.UpdateChargeOrder.1", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, inpatientNo, orderNo, this.Operator.ID, this.GetSysDateTime().ToString());
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.UpdateChargeOrder.1";
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新医嘱附材数量
        /// </summary>
        /// <param name="orderNo">附材医嘱编码</param>
        /// <param name="num"></param>
        /// <returns></returns>
        public int UpdateSubtblNum(string orderNo, decimal num)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.UpdateSubNum.1", ref strSql) == -1)
            {
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, orderNo, num);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return -1;
            }

            return this.ExecNoQuery(strSql);

        }
        #endregion


        #endregion

        #region "医嘱执行处理

        #region "更新执行档"

        /// <summary>
        /// 护士站更新临时医嘱备注信息
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public int UpdateExecTime(string orderNo, string txt)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateExecTime", ref strSql) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.UpdateExecTimeDrug字段";
                return -1;
            }
            strSql = string.Format(strSql, orderNo, txt);
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新执行档有效标记
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public int UpdateExecValidFlag(string execOrderID, bool isPharmacy, string flag)
        {
            string strSql = "";
            string strIndex = "";

            if (isPharmacy)			//药品执行档
                strIndex = "Order.Update.UpdateExecValidFlag.1";
            else					//非药品执行档
                strIndex = "Order.Update.UpdateExecValidFlag.2";

            if (this.Sql.GetCommonSql(strIndex, ref strSql) == -1)
            {
                return -1;
            }

            try
            {
                strSql = string.Format(strSql, execOrderID, flag);
            }
            catch (Exception ex)
            {
                this.Err = "传入参数不对!" + strIndex + ex.Message;
                return -1;
            }
            return ExecNoQuery(strSql);
        }

        /// <summary>
        /// 作废执行档 非常规操作
        /// </summary>
        /// <param name="SqnNo"></param>
        /// <param name="isDrug"></param>
        /// <param name="dcPerson"></param>
        /// <returns></returns>
        public int DcExecImmediate(string SqnNo, bool isDrug, Neusoft.FrameWork.Models.NeuObject dcPerson)
        {
            string strSql = "";
            if (isDrug)
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.DcExecImmediate.UnNormal.Drug", ref strSql) == -1)
                {
                    this.Err = "Can't Find Sql";
                    return -1;
                }
            }
            else
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.DcExecImmediate.UnNormal.UnDrug", ref strSql) == -1)
                {
                    this.Err = "Can't Find Sql";
                    return -1;
                }
            }
            try
            {
                strSql = System.String.Format(strSql, SqnNo, dcPerson.ID, "0");
            }
            catch
            {
                this.Err = "传入参数不对";
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 作废执行档
        /// </summary>
        /// <param name="dcPerson">执行档信息</param>
        /// <returns>0 success -1 fail</returns>
        public int DcExecImmediate(Neusoft.HISFC.Models.Order.Order Order, Neusoft.FrameWork.Models.NeuObject dcPerson)
        {
            #region 作废执行档
            //作废执行档(医嘱停止或直接作废)
            //Order.ExecOrder.DcExecImmediate
            //传入：0 id，1 停止人id,2停止人姓名，3停止时间,4作废标志 
            //传出：0 
            #endregion

            string strSql = "", strSqlName = "Order.ExecOrder.DcExecImmediate.";
            string strType = "";
            strType = this.JudgeItemType(Order);
            if (strType == "")
            {
                return -1;
            }

            #region 检验医嘱,临嘱口服药取消后自动作废执行档(不判断收费状态)
            // {B1C5287C-F7BA-410d-9E5C-B9FA779D0D14}
            //if ((Order as Neusoft.HISFC.Models.Order.Inpatient.Order).OrderType.ID.ToString() == "LZ")
            //{
            //    if (Order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug && Order.Item.SysClass.ID.ToString() == "UL")
            //    {
            //        strSqlName = strSqlName + "4";
            //    }

            //    if (Order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug && Order.Usage.ID.ToString() == "1")
            //    {
            //        strSqlName = strSqlName + "3";
            //    }
            //    if (strSqlName == "Order.ExecOrder.DcExecImmediate.")
            //    {
            //        strSqlName = strSqlName + strType;
            //    }
            //}
            //else
            //{
            //strSqlName = strSqlName + strType;
            //}
            strSqlName = strSqlName + strType;
            #endregion

            if (this.Sql.GetCommonSql(strSqlName, ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, Order.ID, dcPerson.ID, dcPerson.Name);
            }
            catch
            {
                this.Err = "传入参数不对" + strSqlName;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 停止指定执行档
        /// </summary>
        /// <param name="execOrder"></param>
        /// <param name="dcPerson"></param>
        /// <returns></returns>
        public int DcExecImmediate(Neusoft.HISFC.Models.Order.ExecOrder execOrder, Neusoft.FrameWork.Models.NeuObject dcPerson)
        {
            #region 作废执行档
            //作废执行档(医嘱停止或直接作废)
            //Order.ExecOrder.DcExecImmediate
            //传入：0 id，1 停止人id,2停止人姓名，3停止时间,4作废标志 
            //传出：0 
            #endregion
            string strSql = "", strSqlName = "Order.ExecOrder.DcExecImmediateByExecOrderID.";
            string strType = "";

            strType = this.JudgeItemType(execOrder.Order);
            if (strType == "") return -1;

            // {0EC08D81-B36B-4153-9665-D6C4599711CE} 非药品不需要执行确认的取消执行单，直接取消收费标记
            if (strType == "2" && execOrder.Order.OrderType.Type == Neusoft.HISFC.Models.Order.EnumType.SHORT && execOrder.Order.ReciptDept.ID == execOrder.Order.ExeDept.ID)
            {
                strSqlName = "Order.ExecOrder.DcExecImmediateByExecOrderID.22";
            }
            else
            {
                strSqlName = strSqlName + strType;
            }

            if (this.Sql.GetCommonSql(strSqlName, ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrder.ID, dcPerson.ID, dcPerson.Name);
            }
            catch
            {
                this.Err = "传入参数不对" + strSqlName;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 按医嘱流水号作废执行档
        /// </summary>
        /// <param name="Order"></param>
        /// <returns>0 success -1 fail</returns>
        public int DcExecLater(Neusoft.HISFC.Models.Order.Inpatient.Order Order, Neusoft.FrameWork.Models.NeuObject dcPerson)
        {
            #region 按医嘱流水号作废执行档
            //作废执行档(医嘱停止或直接作废)
            //Order.ExecOrder.DcExecLater
            //传入：0 orderid，1 停止人id,2停止人姓名，3停止时间
            //传出：0 
            #endregion

            string strSql = "", strSqlName = "Order.ExecOrder.DcExecLater.";

            string strType = "";
            strType = this.JudgeItemType(Order);
            if (strType == "") return -1;

            strSqlName = strSqlName + strType;

            if (this.Sql.GetCommonSql(strSqlName, ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                if (Order.OrderType.IsDecompose)
                {
                    //长嘱作废停止时间之后的执行档
                    strSql = string.Format(strSql, Order.ID, dcPerson.ID, dcPerson.Name, Order.EndTime);
                }
                else
                {
                    //临嘱作废开立（使用）时间之后的执行档
                    strSql = string.Format(strSql, Order.ID, dcPerson.ID, dcPerson.Name, Order.MOTime);
                }
            }
            catch
            {
                this.Err = "传入参数不对" + strSqlName;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }


        /// <summary>
        /// 更新医嘱状态
        /// 为已经执行
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateOrderStatus(string orderNo, int status)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Update.OrderStatus", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, orderNo, status.ToString());
            }
            catch (Exception ex)
            {
                this.Err = "传入参数不对！Order.Update.OrderStatus" + ex.Message;
                this.WriteErr();
                return -1;
            }
            //if(this.ExecNoQuery(strSql) <= 0) return -1;
            //return 0;
            return this.ExecNoQuery(strSql);
        }


        /// <summary>
        /// 更新医嘱排序号
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="sortID"></param>
        /// <returns></returns>
        public int UpdateOrderSortID(string orderID, string sortID)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.updateOrderSort.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            return this.ExecNoQuery(strSql, orderID, sortID);
        }
        #endregion

        #region 更新打印标记

        /// <summary>
        /// 更新执行单打印标记通过执行单流水号
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <param name="itemType">1 药品 ,2 非药品</param>
        /// <returns></returns>
        public int UpdateExecOrderPrinted(string execOrderID, string itemType)
        {
            string strSql = "";
            if (itemType == "2")
            {
                //Order.ExecOrder.UpdateExecUndrugPrintFlag
                if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateExecUndrugPrintFlag", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, execOrderID, this.Operator.ID);
                }
                catch
                {
                    this.Err = "传入参数不对！Order.ExecOrder.UpdateExecUndrugPrintFlag";
                    this.WriteErr();
                    return -1;
                }
            }
            else if (itemType == "1")
            {
                //Order.ExecOrder.UpdateExecDrugPrintFlag
                if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateExecDrugPrintFlag", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, execOrderID, this.Operator.ID);
                }
                catch
                {
                    this.Err = "传入参数不对！Order.ExecOrder.UpdateExecDrugPrintFlag";
                    this.WriteErr();
                    return -1;
                }
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新执行单打印标记通过医嘱流水号
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <param name="itemType">1 药品 ,2 非药品</param>
        /// <returns></returns>
        public int UpdateExecOrderPrintedByMoOrder(string moOrder, string dt1, string dt2, string itemType)
        {
            string strSql = "";
            if (itemType == "2")
            {
                //Order.ExecOrder.UpdateExecUndrugPrintFlag
                if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateExecUndrugPrintFlagByMoOrder", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, moOrder, dt1, dt2, this.Operator.ID);
                }
                catch
                {
                    this.Err = "传入参数不对！Order.ExecOrder.UpdateExecUndrugPrintFlagByMoOrder";
                    this.WriteErr();
                    return -1;
                }
            }
            else if (itemType == "1")
            {
                //Order.ExecOrder.UpdateExecDrugPrintFlag
                if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateExecDrugPrintFlagByMoOrder", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, moOrder, dt1, dt2, this.Operator.ID);
                }
                catch
                {
                    this.Err = "传入参数不对！Order.ExecOrder.UpdateExecDrugPrintFlagByMoOrder";
                    this.WriteErr();
                    return -1;
                }
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新收费打印
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int UpdateExecNeedFeePrinted(string execOrderID, string itemType)
        {
            string strSql = "";
            if (itemType == "1")
            {
                //Order.ExecOrder.Drug.UpdateExecNeedFeePrinted
                if (this.Sql.GetCommonSql("Order.ExecOrder.Drug.UpdateExecNeedFeePrinted", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, execOrderID, this.Operator.ID);
                }
                catch
                {
                    this.Err = "传入参数不对！Order.ExecOrder.Drug.UpdateExecNeedFeePrinted";
                    this.WriteErr();
                    return -1;
                }
            }
            else if (itemType == "2")
            {
                //Order.ExecOrder.Undrug.UpdateExecNeedFeePrinted
                if (this.Sql.GetCommonSql("Order.ExecOrder.Undrug.UpdateExecNeedFeePrinted", ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, execOrderID, this.Operator.ID);
                }
                catch
                {
                    this.Err = "传入参数不对！Order.ExecOrder.Undrug.UpdateExecNeedFeePrinted";
                    this.WriteErr();
                    return -1;
                }
            }
            return this.ExecNoQuery(strSql);
        }
        ///<summary>
        /// 更新收费打印
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <returns></returns>
        public int UpdateTransfusionPrinted(string execOrderID)
        {
            int rev = -1;

            string strSql = "";
            //Order.ExecOrder.Drug.UpdateExecNeedFeePrinted
            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateTransfusionPrinted", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrderID, this.Operator.ID);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.UpdateTransfusionPrinted";
                this.WriteErr();
                return -1;
            }
            rev = this.ExecNoQuery(strSql);
            if (rev == -1)
            {
                return rev;
            }

            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateTransfusionPrinted.Undrug", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrderID, this.Operator.ID);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.UpdateTransfusionPrinted.Undrug";
                this.WriteErr();
                return -1;
            }
            rev = this.ExecNoQuery(strSql);
            return rev;
        }

        /// <summary>
        /// 更新巡回卡打印标记
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <returns></returns>
        public int UpdateCircultPrinted(string execOrderID)
        {
            int rev = -1;
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateCircultPrinted", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrderID, this.Operator.ID);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.UpdateCircultPrinted";
                this.WriteErr();
                return -1;
            }
            rev = this.ExecNoQuery(strSql);
            if (rev == -1)
            {
                return -1;
            }

            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateCircultPrinted.Undrug", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrderID, this.Operator.ID);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.UpdateCircultPrinted.Undrug";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        public int UpdateCircultPrinted(string moOrder, string dt1, string dt2)
        {
            int rev = -1;
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateCircultPrinted.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, moOrder, dt1, dt2);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.UpdateCircultPrinted.1";
                this.WriteErr();
                return -1;
            }
            rev = this.ExecNoQuery(strSql);
            if (rev == -1)
            {
                return -1;
            }

            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateCircultPrinted.Undrug.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, moOrder, dt1, dt2);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.UpdateCircultPrinted.Undrug.1";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }



        #endregion

        #region "查询医嘱执行信息"
        /// <summary>
        /// 查询所有医嘱执行情况
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="itemType">"" 全部，1药2非药</param>
        /// <returns></returns>
        public ArrayList QueryExecOrder(string inpatientNo, string itemType)
        {
            #region 查询所有医嘱执行情况
            //查询所有医嘱执行情况（药，非药）
            //Order.ExecOrder.QueryPatientExec.1
            //传入：0 inpatientno
            //传出：ArrayList
            #endregion
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect(itemType);
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryPatientExec.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryPatientExec.1字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo);
                addExecOrder(al, sql);
            }
            return al;
        }
        /// <summary>
        /// 按查询是否有效执行医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="itemType">"" 全部，1药2非药</param>
        /// <param name="isValid">是否有效</param>
        /// <returns></returns>
        public ArrayList QueryExecOrder(string inpatientNo, string itemType, bool isValid)
        {
            #region 按查询有效执行医嘱
            //按查询有效执行医嘱
            //Order.ExecOrder.QueryValidOrder.1
            //传入：0 inpatientno 1  IsValid
            //传出：ArrayList
            #endregion

            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect(itemType);
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryValidOrder.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryValidOrder.1字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo, Neusoft.FrameWork.Function.NConvert.ToInt32(isValid).ToString());
                addExecOrder(al, sql);
            }
            return al;
        }

        /// <summary>
        /// 根据申请单号查找未进行执行确认的医嘱（只包含非药品的）
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="applyNO"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrderByApplyNO(string inpatientNO, string applyNO)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect("2");
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryApplyOrder.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryApplyOrder.1字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, inpatientNO, applyNO);
                addExecOrder(al, sql);
            }
            return al;
        }

        /// <summary>
        /// 按查询是否执行医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="itemType">"" 全部，1药2非药</param>
        /// <param name="isExec"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrderIsExec(string inpatientNo, string itemType, bool isExec)
        {
            #region 按查询是否执行医嘱
            //按查询是否执行医嘱
            //Order.ExecOrder.QueryExecOrder.1
            //传入：0 inpatientno 1 IsExec
            //传出：ArrayList
            #endregion

            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            s = ExecOrderQuerySelect(itemType);
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;

                //{DA77B01B-63DF-4559-B264-798E54F24ABB}
                if (itemType == "")
                {
                    return null;
                }
                string strSqlName = "Order.ExecOrder.QueryExecOrder." + itemType;
                //{DA77B01B-63DF-4559-B264-798E54F24ABB}
                if (this.Sql.GetCommonSql(strSqlName, ref sql1) == -1)
                {
                    this.Err = "没有找到" + strSqlName + "字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo, Neusoft.FrameWork.Function.NConvert.ToInt32(isExec).ToString());
                addExecOrder(al, sql);
            }
            return al;
        }
        /// <summary>
        /// 按查询执行医嘱信息
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <param name="itemType"></param>
        /// <returns>Neusoft.HISFC.Models.Order.ExecOrder</returns>
        public Neusoft.HISFC.Models.Order.ExecOrder QueryExecOrderByExecOrderID(string execOrderID, string itemType)
        {
            #region 按查询是否执行医嘱
            //按查询是否执行医嘱
            //Order.ExecOrder.QueryExecOrder.0
            //传入：0 ExecOrderID
            //传出：Neusoft.HISFC.Models.Order.ExecOrder
            #endregion
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            s = ExecOrderQuerySelect(itemType);
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryExecOrder.0", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryExecOrder.0字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, execOrderID);
                addExecOrder(al, sql);
            }
            if (al.Count > 0) return al[0] as Neusoft.HISFC.Models.Order.ExecOrder;
            return null;
        }

        /// <summary>
        /// 按执行部门查询是否执行医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="itemType"></param>
        /// <param name="isExec"></param>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrderByDept(string inpatientNo, string itemType, bool isExec, string deptCode)
        {
            #region 按执行部门查询是否执行医嘱
            //Order.ExecOrder.QueryExecOrderByDept.1
            //传入：0 inpatientno 1 IsExec 2 deptid
            //传出：ArrayList
            #endregion
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            s = ExecOrderQuerySelect(itemType);
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryExecOrderByDept.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryExecOrder.1字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo, Neusoft.FrameWork.Function.NConvert.ToInt32(isExec).ToString(), deptCode);
                addExecOrder(al, sql);
            }
            return al;
        }

        /// <summary>
        /// 按查询是否收费医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="itemType"></param>
        /// <param name="isCharge"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrderIsCharg(string inpatientNo, string itemType, bool isCharge)
        {
            #region 按查询是否收费医嘱
            //按查询是否收费医嘱
            //Order.ExecOrder.QueryChargeOrder.1
            //传入：0 inpatientno 1  IsCharge
            //传出：ArrayList
            #endregion
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect(itemType);
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryChargeOrder.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryChargeOrder.1字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo, Neusoft.FrameWork.Function.NConvert.ToInt32(isCharge).ToString());
                addExecOrder(al, sql);
            }
            return al;
        }

        /// <summary>
        /// 按查询配药状态医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="drugFlag"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrderByDrugFlag(string inpatientNo, DateTime beginTime, DateTime endTime, int drugFlag)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect("1");
            sql = s[0];
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderDrugFlag.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.QueryOrderDrugFlag.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNo, drugFlag.ToString(), beginTime, endTime);
            return this.myExecOrderQuery(sql);
        }

        /// <summary>
        /// 根据条件查询执行档
        /// </summary>
        /// <param name="Type">1 药品， 2 非药品</param>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        public ArrayList BetchQueryExecOrder(string Type, string whereSql)
        {
            string sql = ExecOrderQuerySelect(Type)[0];
            sql = sql + "\r\n" + whereSql;
            return myExecOrderQuery(sql);
        }

        /// <summary>
        /// 按查询配药状态医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="drugFlag"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrderByDrugFlag(string inpatientNo, int drugFlag)
        {
            #region 按查询配药状态医嘱
            //按查询配药状态医嘱
            //Order.ExecOrder.QueryOrderDrugFlag.1
            //传入：0 inpatientno 1  DrugFlag
            //传出：ArrayList
            #endregion

            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect("1");
            sql = s[0];
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderDrugFlag.2", ref sql1) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.QueryOrderDrugFlag.2字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNo, drugFlag.ToString());
            return this.myExecOrderQuery(sql);
        }
        /// <summary>
        /// 按医嘱流水号查询医嘱执行信息
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="itemType">1药2非药""全部</param>
        /// <returns></returns>
        public ArrayList QueryExecOrderByOneOrder(string orderNo, string itemType)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            #region 按医嘱流水号查询医嘱执行信息
            //按医嘱流水号查询医嘱执行信息
            //Order.ExecOrder.QueryOrder.where.5
            //传入：0 OrderNo
            //传出：ArrayList
            #endregion
            s = ExecOrderQuerySelect(itemType);
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.Query.where.5", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.Query.where.5字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, orderNo);
                addExecOrder(al, sql);
            }
            return al;
        }

        /// <summary>
        /// 通过一条主医嘱查询其对应的附材信息{92A0CF31-27BC-472e-9C15-1ED2C8A81F36} by zhang.xs 2010.10.19
        /// </summary>
        /// <param name="execSqn">执行序列号</param>
        /// <param name="combNO">组合号</param>
        /// <returns></returns>
        public ArrayList QueryExecOrderSubtblByMainOrder(string execSqn, string combNO)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            s = ExecOrderQuerySelect("2");
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrderSubtbl.Query.where", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrderSubtbl.Query.where字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, execSqn, combNO);
                addExecOrder(al, sql);
            }
            return al;
        }

        /// <summary>
        ///  通过一条主医嘱查询其对应的执行信息
        /// </summary>
        /// <param name="moOrder">医嘱流水号</param>
        /// <param name="undrugCode">非药品编号</param>
        /// <param name="confirmFlag">确认标记 0未确认/1已确认</param>
        /// <returns></returns>
        public ArrayList QueryExecOrderByOrderNo(string moOrder, string undrugCode, string confirmFlag)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            s = ExecOrderQuerySelect("2");
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null) return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.Query.where.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.Query.where.1字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, moOrder, undrugCode, confirmFlag);
                addExecOrder(al, sql);
            }
            return al;
        }

        /// <summary>
        ///  患者输液卡查询
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="usageCode"></param>
        /// <param name="isPrinted"></param>
        /// <returns></returns>
        public ArrayList QueryOrderExec(string inpatientNo, DateTime beginTime, DateTime endTime, string usageCode, bool isPrinted)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect("1");
            sql = s[0];
            if (sql == null)
                return null;

            if (usageCode.Contains("'"))
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderExec.NotSeprate.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryOrderExec.NotSeprate.1字段!";
                    this.WriteErr();
                    return null;
                }
            }
            else
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderExec.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryOrderExec.1字段!";
                    this.WriteErr();
                    return null;
                }
            }
            sql = sql + " " + string.Format(sql1, inpatientNo, beginTime.ToString(), endTime.ToString(), usageCode, Neusoft.FrameWork.Function.NConvert.ToInt32(isPrinted).ToString());
            al = this.myExecOrderQuery(sql);

            s = ExecOrderQuerySelect("2");
            sql = s[0];
            if (sql == null)
                return null;

            if (usageCode.Contains("'"))
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryUndrugOrderExec.NotSeprate.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryUndrugOrderExec.1字段!";
                    this.WriteErr();
                    return null;
                }
            }
            else
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryUndrugOrderExec.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryUndrugOrderExec.1字段!";
                    this.WriteErr();
                    return null;
                }
            }
            sql = sql + " " + string.Format(sql1, inpatientNo, beginTime.ToString(), endTime.ToString(), usageCode, Neusoft.FrameWork.Function.NConvert.ToInt32(isPrinted).ToString());
            al.AddRange(this.myExecOrderQuery(sql));

            return al;
        }

        /// <summary>
        /// 查询巡回卡信息
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="usageCode"></param>
        /// <param name="isPrinted"></param>
        /// <returns></returns>
        public ArrayList QueryOrderCircult(string inpatientNo, DateTime beginTime, DateTime endTime, string usageCode, bool isPrinted)
        {
            string[] s;
            string sql = "", sql1 = "";

            s = ExecOrderQuerySelect("1");

            sql = s[0];
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderExec.Circlue", ref sql1) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.QueryOrderExec.Circlue字段!";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNo, beginTime.ToString(), endTime.ToString(), usageCode, Neusoft.FrameWork.Function.NConvert.ToInt32(isPrinted).ToString());
            return this.myExecOrderQuery(sql);
        }

        /// <summary>
        /// 获得患者执行单-药品和非药品
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isPrinted"></param>
        /// <returns></returns>
        public ArrayList QueryOrderExecBill(string inpatientNo, DateTime beginTime, DateTime endTime, string billNo, bool isPrinted)
        {
            #region 患者执行单查询
            //患者执行单查询
            //Order.ExecOrder.QueryOrderExecBill
            //传入：0InpatientNo,1 执行单流水号 2DateTimeBegin,3 DateTimeEnd,4 PrintFlag
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderExecBill.1", ref sql) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.QueryOrderExecBill.1字段!";
                return null;
            }
            sql = string.Format(sql, inpatientNo, billNo, beginTime.ToString(), endTime.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isPrinted).ToString());
            addExecOrder(al, sql);
            //
            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderExecBill.2", ref sql1) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.QueryOrderExecBill.2字段!";
                return null;
            }
            sql1 = string.Format(sql1, inpatientNo, billNo, beginTime.ToString(), endTime.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isPrinted).ToString());
            addExecOrder(al, sql1);

            return al;
        }

        /// <summary>
        ///  查询一个组合有多少条需要执行的医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="combno"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="billNo"></param>
        /// <param name="isPrinted"></param>
        /// <returns></returns>
        public ArrayList QueryOrderExecCountByCombno(string inpatientNo, string combno, DateTime beginTime, DateTime endTime, string billNo, bool isPrinted)
        {

            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderExecBill.3", ref sql) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.QueryOrderExecBill.1字段!";
                return null;
            }
            sql = string.Format(sql, inpatientNo, combno, billNo, beginTime.ToString(), endTime.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isPrinted).ToString());
            addExecOrder(al, sql);
            //
            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderExecBill.4", ref sql1) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.QueryOrderExecBill.2字段!";
                return null;
            }
            sql1 = string.Format(sql1, inpatientNo, combno, billNo, beginTime.ToString(), endTime.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isPrinted).ToString());
            addExecOrder(al, sql1);
            return al;
        }

        /// <summary>
        /// 获得患者检验单信息
        /// </summary>
        /// <param name="inpatientNo">患者住院号</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isPrinted">打印标记</param>
        /// <returns></returns>
        public ArrayList QueryOrderLisApplyBill(string inpatientNo, DateTime beginTime, DateTime endTime, bool isPrinted)
        {
            #region 患者执行单查询
            //患者执行单查询
            //Order.ExecOrder.QueryOrderLisApplyBill
            //传入：0 InpatientNo,1 DateTimeBegin,2 DateTimeEnd,4 PrintFlag
            //传出：ArrayList
            #endregion

            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            string[] s = ExecOrderQuerySelect("2");
            if (s.Length > 0)
            {
                sql = s[0];
            }
            else
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrderLisApplyBill", ref sql1) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.QueryOrderLisApplyBill字段!";
                return null;
            }
            sql1 = string.Format(sql1, inpatientNo, beginTime.ToString(), endTime.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isPrinted).ToString());
            sql1 = sql + " " + sql1;
            addExecOrder(al, sql1);
            return al;
        }
        /// <summary>
        /// 查询医嘱收费单
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="itemType"></param>
        /// <param name="isPrinted"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrderBillNeedCharge(string inpatientNo, string itemType, bool isPrinted)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect(itemType);
            sql = s[0];
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.ExecDrugUnDrug.QueryNoCharged.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.ExecDrugUnDrug.QueryNoCharged.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNo, Neusoft.FrameWork.Function.NConvert.ToInt32(isPrinted).ToString());
            return this.myExecOrderQuery(sql);
        }
        /// <summary>
        /// 查询医嘱执行档根据处方号
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrderBillByReceiptNo(string itemType, string receiptNo)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect(itemType);
            sql = s[0];
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.QueryOrderExecBill.ReceiptNo", ref sql1) == -1)
            {
                this.Err = "没有找到Order.QueryOrderExecBill.ReceiptNo字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, receiptNo);
            return this.myExecOrderQuery(sql);
        }
        /// <summary>
        /// 查询需要发药的医嘱信息
        /// </summary>
        /// <param name="deptcode"></param>
        /// <returns></returns>
        public ArrayList QureyExecOrderNeedSendDrug(string deptcode)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect("1");

            sql = s[0];
            if (sql == null) return null;

            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryNeedDrug", ref sql1) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.QueryNeedDrug字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, deptcode);
            addExecOrder(al, sql);

            return al;
        }
        /// <summary>
        /// 查询药品执行医嘱通过住院流水号
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        public DataSet QueryExecDrugOrderByInpatientNo(string inpatientNo)
        {
            string strSql = "";
            DataSet dataSet = new DataSet();

            //取SQL语句
            if (this.Sql.GetCommonSql("Order.Report.ExecDrugByInpatientNo", ref strSql) == -1)
            {
                this.Err = "没有找到Order.Report.ExecDrugByInpatientNo字段!";
                return null;
            }
            try
            {
                strSql = string.Format(strSql, inpatientNo);    //替换SQL语句中的参数。
            }
            catch (Exception ex)
            {
                this.Err = "付数值时候出错Order.Report.ExecDrugByInpatientNo！" + ex.Message;
                this.WriteErr();
                return null;
            }

            //根据SQL语句取查询结果
            if (this.ExecQuery(strSql, ref dataSet) == -1) return null;

            return dataSet;
        }

        /// <summary>
        /// 查询药品执行医嘱通过住院流水号
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        public DataSet QueryExecDrugOrderByInpatientNo(string inpatientNo, string nurseCode)
        {
            string strSql = "";
            DataSet dataSet = new DataSet();

            //取SQL语句
            //查询是否是介入手术区
            string sql3 = string.Format("SELECT d.code FROM com_dictionary d where d.type='OpenOrderDepd' and d.valid_state = '1' and d.code = '{0}'", nurseCode);
            string result = Sql.ExecSqlReturnOne(sql3, "");
            if (string.IsNullOrEmpty(result))
            {
                if (this.Sql.GetCommonSql("Order.Report.ExecDrugByInpatientNo", ref strSql) == -1)
                {
                    this.Err = "没有找到Order.Report.ExecDrugByInpatientNo字段!";
                    return null;
                }
                try
                {
                    strSql = string.Format(strSql, inpatientNo);    //替换SQL语句中的参数。
                }
                catch (Exception ex)
                {
                    this.Err = "付数值时候出错Order.Report.ExecDrugByInpatientNo！" + ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
            else
            {
                if (this.Sql.GetCommonSql("Order.Report.ExecDrugByInpatientNo.New", ref strSql) == -1)
                {
                    this.Err = "没有找到Order.Report.ExecDrugByInpatientNo.New字段!";
                    return null;
                }
                try
                {
                    strSql = string.Format(strSql, inpatientNo);    //替换SQL语句中的参数。
                }
                catch (Exception ex)
                {
                    this.Err = "付数值时候出错Order.Report.ExecDrugByInpatientNo.New！" + ex.Message;
                    this.WriteErr();
                    return null;
                }
            }


            //根据SQL语句取查询结果
            if (this.ExecQuery(strSql, ref dataSet) == -1) return null;

            return dataSet;
        }

        /// <summary>
        /// 根据流水号查询 一段时间内的医嘱执行情况
        /// </summary>
        /// <param name="inPatientNo"></param>
        /// <param name="Type">类别 1 药品 2非药品</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ArrayList QueryExecOrder(string inPatientNo, string type, DateTime begin, DateTime end)
        {
            string[] s;
            string sql = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect(type);
            sql = s[0];
            if (sql == null)
            {
                return null;
            }

            string whereSql = "";
            if (type == "1")
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.Query.byInpatientNo.Durg", ref whereSql) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.Query.byInpatientNo.Durg字段!";
                    return null;
                }
            }
            else
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.Query.byInpatientNo.UnDurg", ref whereSql) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.Query.byInpatientNo.UnDurg字段!";
                    return null;
                }
            }
            whereSql = string.Format(whereSql, inPatientNo, begin, end);

            return this.myExecOrderQuery(sql + whereSql);
        }

        /// <summary>
        /// 根据住院流水号和医嘱流水号查询所有医嘱
        /// </summary>
        /// <param name="inPatientNo"></param>
        /// <param name="type">类型 1药品 2非药品</param>
        /// <param name="strOrderID">所有医嘱流水号组成的字符串 用IN查询</param>
        /// <returns></returns>
        public ArrayList QueryOrder(string inPatientNo, string type, string strOrderID)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.ByID", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.ByID字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, inPatientNo, type, strOrderID);
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 根据患者住院流水号，查询非药品医嘱执行档信息
        /// writed by cuipeng
        /// 2005-06
        /// </summary>
        /// <param name="inpatientNo">患者住院流水号</param>
        /// <returns></returns>
        public DataSet QueryExecUndrugOrderByInpatientNo(string inpatientNo)
        {
            string strSql = "";
            DataSet dataSet = new DataSet();

            //取SQL语句
            if (this.Sql.GetCommonSql("Order.Report.ExecUndrugByInpatientNo", ref strSql) == -1)
            {
                this.Err = "没有找到Order.Report.ExecUndrugByInpatientNo字段!";
                return null;
            }
            try
            {
                strSql = string.Format(strSql, inpatientNo);    //替换SQL语句中的参数。
            }
            catch (Exception ex)
            {
                this.Err = "付数值时候出错Order.Report.ExecUndrugByInpatientNo！" + ex.Message;
                this.WriteErr();
                return null;
            }

            //根据SQL语句取查询结果
            if (this.ExecQuery(strSql, ref dataSet) == -1) return null;

            return dataSet;
        }

        /// <summary>
        /// 根据患者住院流水号，查询非药品医嘱执行档信息
        /// writed by tangyi
        /// 2023-05-12
        /// </summary>
        /// <param name="inpatientNo">患者住院流水号</param>
        /// <returns></returns>
        public DataSet QueryExecUndrugOrderByInpatientNo(string inpatientNo, string nurseCode)
        {
            string strSql = "";
            DataSet dataSet = new DataSet();

            //取SQL语句
            //查询是否是介入手术区
            string sql3 = string.Format("SELECT d.code FROM com_dictionary d where d.type='OpenOrderDepd' and d.valid_state = '1' and d.code = '{0}'", nurseCode);
            string result = Sql.ExecSqlReturnOne(sql3, "");
            if (string.IsNullOrEmpty(result))
            {
                if (this.Sql.GetCommonSql("Order.Report.ExecUndrugByInpatientNo", ref strSql) == -1)
                {
                    this.Err = "没有找到Order.Report.ExecUndrugByInpatientNo字段!";
                    return null;
                }
                try
                {
                    strSql = string.Format(strSql, inpatientNo);    //替换SQL语句中的参数。
                }
                catch (Exception ex)
                {
                    this.Err = "付数值时候出错Order.Report.ExecUndrugByInpatientNo！" + ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
            else
            {
                if (this.Sql.GetCommonSql("Order.Report.ExecUndrugByInpatientNo.New", ref strSql) == -1)
                {
                    this.Err = "没有找到Order.Report.ExecUndrugByInpatientNo.New字段!";
                    return null;
                }
                try
                {
                    strSql = string.Format(strSql, inpatientNo);    //替换SQL语句中的参数。
                }
                catch (Exception ex)
                {
                    this.Err = "付数值时候出错Order.Report.ExecUndrugByInpatientNo.New！" + ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
            //根据SQL语句取查询结果
            if (this.ExecQuery(strSql, ref dataSet) == -1) return null;

            return dataSet;
        }



        /// <summary>
        /// 根据住院流水号、药品编码、时间段检索患者累计用药情况  
        /// writed by liangjz 2005-06
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="drugCode">药品编码</param>
        /// <param name="myBeginTime">起始时间</param>
        /// <param name="myEndTime">终止时间</param>
        /// <returns>dataset</returns>
        public DataSet QueryTotalUseDrug(string inpatientNo, string drugCode, DateTime myBeginTime, DateTime myEndTime)
        {
            string strSql = "";
            DataSet dataSet = new DataSet();

            //取SQL语句
            if (this.Sql.GetCommonSql("Order.Report.TotalUseDrug", ref strSql) == -1)
            {
                this.Err = "没有找到Order.Report.TotalUseDrug字段!";
                return null;
            }
            try
            {
                strSql = string.Format(strSql, inpatientNo, drugCode, myBeginTime.ToString(), myEndTime.ToString());    //替换SQL语句中的参数。
            }
            catch (Exception ex)
            {
                this.Err = "付数值时候出错Order.Report.TotalUseDrug！" + ex.Message;
                this.WriteErr();
                return null;
            }

            //根据SQL语句取查询结果
            if (this.ExecQuery(strSql, ref dataSet) == -1) return null;

            return dataSet;
        }
        /// <summary>
        /// 按照时间，最小费用，科室查询收费的药品信息
        /// </summary>
        /// <param name="minFee"></param>
        /// <param name="deptCode"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public DataSet QueryChargedMedicine(string minFee, string deptCode, string dtBegin, string dtEnd)
        {
            string strSql = "";
            DataSet dsMedicine = new DataSet();
            if (this.Sql.GetCommonSql("Fee.Item.QueryChargedMedicine", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql";
                return null;
            }
            strSql = System.String.Format(strSql, minFee, deptCode, dtBegin, dtEnd);
            this.ExecQuery(strSql, ref dsMedicine);
            return dsMedicine;
        }
        /// <summary>
        /// 按照时间，最小费用，科室查询收费的非药品信息
        /// </summary>
        /// <param name="minFee"></param>
        /// <param name="deptCode"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public DataSet QueryChargedItem(string minFee, string deptCode, string dtBegin, string dtEnd)
        {
            string strSql = "";
            DataSet dsItem = new DataSet();
            if (this.Sql.GetCommonSql("Fee.Item.QueryChargedItem", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql";
                return null;
            }
            strSql = System.String.Format(strSql, minFee, deptCode, dtBegin, dtEnd);
            this.ExecQuery(strSql, ref dsItem);
            return dsItem;
        }
        /// <summary>
        /// 按照时间，编码查询收费的药品明细信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public DataSet QueryChargedMedicineDetail(string code, string deptCode, string dtBegin, string dtEnd)
        {
            string strSql = "";
            DataSet dsMedicine = new DataSet();
            if (this.Sql.GetCommonSql("Fee.Item.QueryChargedMedicineDetail", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql";
                return null;
            }
            strSql = System.String.Format(strSql, code, deptCode, dtBegin, dtEnd);
            this.ExecQuery(strSql, ref dsMedicine);
            return dsMedicine;
        }
        /// <summary>
        /// 按照时间，编码查询收费的非药品明细信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public DataSet QueryChargedItemDetail(string code, string deptCode, string dtBegin, string dtEnd)
        {
            string strSql = "";
            DataSet dsItem = new DataSet();
            if (this.Sql.GetCommonSql("Fee.Item.QueryChargedItemDetail", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql";
                return null;
            }
            strSql = System.String.Format(strSql, code, deptCode, dtBegin, dtEnd);
            this.ExecQuery(strSql, ref dsItem);
            return dsItem;
        }
        #endregion

        #region 判断出院带药单是否全部收费
        /// <summary>
        /// 判断出院带药单是否全部收费
        /// </summary>
        /// <param name="inpatient_no"></param>
        /// <returns></returns>
        public int IsCanPrintOutHosDrug(string inpatient_no, ref bool bReturn)
        {
            bReturn = false;
            string sql = "Order.ExecOrder.QueryIsCanPrintOutHosDrug";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = "无法查到Order.ExecOrder.QueryIsCanPrintOutHosDrug";
                return -1;
            }
            try
            {
                sql = string.Format(sql, inpatient_no);
            }
            catch
            {
                this.Err = "Order.ExecOrder.QueryIsCanPrintOutHosDrug参数错误!";
                return -1;
            }
            int i = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
            if (i < 0)
            {

                return -1;
            }
            else if (i == 0)
            {
                bReturn = true;//可以出院
                return 0;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// 根据医嘱号获取相应费用
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="moOrder"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int QueryOrderFees(string inpatientNo, string moOrder, ref DataSet ds)
        {
            string sql = "Order.ExecOrder.QueryOrderFees";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1)
            {
                this.Err = "无法查到Order.ExecOrder.QueryOrderFees";
                return -1;
            }
            try
            {
                sql = string.Format(sql, inpatientNo, moOrder);
            }
            catch
            {
                this.Err = "Order.ExecOrder.QueryOrderFees参数错误!";
                return -1;
            }

            this.ExecQuery(sql, ref ds);

            return 1;
        }

        #endregion

        #region "医嘱审核保存"
        /// <summary>
        /// 审核医嘱 -审核未审核和作废的医嘱
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int ConfirmOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order, bool IsCharge, System.DateTime dt)
        {
            #region 审核医嘱
            //审核医嘱，使医嘱处于有效状态
            //Order.Order.ConfirmOrder.1
            //传入：0 id,1 confirmcode,2 status 3confirmtime
            //传出：0 
            #endregion

            try
            {
                string strSql = "";
                if (order.Status == 0)//未审核的医嘱
                {
                    if (this.Sql.GetCommonSql("Order.Order.ConfirmOrder.1", ref strSql) == -1)
                    {
                        return -1;
                    }

                    //if(order.Item.IsPharmacy==false && IsCharge)//收费的非药品
                    if (order.Item.ItemType != Neusoft.HISFC.Models.Base.EnumItemType.Drug
                        && IsCharge)//收费的非药品    
                    {
                        order.Status = 2;//给执行标记
                    }
                    else
                    {
                        order.Status = 1;//给审核标记
                    }
                }
                else if (order.Status == 3 || order.Status == 4)//停止作废医嘱
                {
                    if (this.Sql.GetCommonSql("Order.Order.ConfirmOrder.2", ref strSql) == -1)
                        return -1;
                }
                else//其他
                {
                    if (order.IsSubtbl)
                    {
                        if (this.Sql.GetCommonSql("Order.Order.ConfirmOrder.2", ref strSql) == -1)
                            return -1;
                    }
                    else
                    {
                        this.Err = Neusoft.FrameWork.Management.Language.Msg("医嘱状态不符合！不能审核！");
                        return -1;
                    }
                }
                try//付值
                {
                    strSql = string.Format(strSql, order.ID, this.Operator.ID, order.Status.ToString(), dt);
                }
                catch
                {
                    this.Err = "传入参数不对！Order.Order.ConfirmOrder.1";
                    this.WriteErr();
                    return -1;
                }
                int intErr = 0;
                intErr = this.ExecNoQuery(strSql);//执行医嘱
                if (intErr == 0)
                {
                    this.Err = order.Patient.PVisit.PatientLocation.Bed.ID.Substring(4) + "床 " + order.Patient.Name + "\r\n\r\n" + order.SubCombNO.ToString() + "组 " + order.Item.Name + "[" + order.Item.Specs + "] " + "\r\n\r\n发生变化，不能审核！\n 请刷新界面重新检索医嘱信息！";
                    return -1;
                }
                else if (intErr < 0)
                {
                    this.Err = Neusoft.FrameWork.Management.Language.Msg(order.Patient.PVisit.PatientLocation.Bed.ID.Substring(4) + "床 " + order.Patient.Name + "\r\n\r\n" + order.SubCombNO.ToString() + "组 " + order.Item.Name + "[" + order.Item.Specs + "] " + "\r\n\r\n审核失败！") + Err;
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = "审核医嘱发生异常：" + ex.Message;
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// 审核医嘱处理（涉及执行档、收费接口由表现层完成）
        ///(审核、执行本科室;通过是否收费更新收费标记）
        /// </summary>
        /// <param name="order"></param>
        /// <param name="isCharge"></param>
        /// <param name="execID"></param>
        /// <returns></returns>
        private int ConfirmAndExecOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order, bool isCharge, string execID)
        {
            Neusoft.HISFC.Models.Base.Employee CurUser = (Neusoft.HISFC.Models.Base.Employee)this.Operator;
            Neusoft.HISFC.Models.Order.ExecOrder objExec = new Neusoft.HISFC.Models.Order.ExecOrder();

            //赋值医嘱项目
            objExec.Order = order;

            //标记该项目是否是终端确认项目
            if (order.Item.GetType() == typeof(Neusoft.HISFC.Models.Fee.Item.Undrug))
            {
                objExec.Order.Item.IsNeedConfirm = ((Neusoft.HISFC.Models.Fee.Item.Undrug)order.Item).IsNeedConfirm;
            }

            //这个标记护士已经处理完医嘱了，终端确认用IsExec来区分是否确认执行
            objExec.IsConfirm = true;

            #region 临期医嘱
            if (order.OrderType.IsDecompose == false) //临时医嘱
            {
                //执行科室是本护士站 或 不是终端批费
                //if ((order.ExeDept.ID == order.Patient.PVisit.PatientLocation.Dept.ID) ||
                //    (JudgeItemType(objExec.Order) == "2" &&
                //    ((Neusoft.HISFC.Models.Fee.Item.Undrug)order.Item).IsNeedConfirm == false))
                //{
                //    //非药品 Order.OrderType.IsCharge == false
                //    if (JudgeItemType(objExec.Order) == "2" &&
                //        ((Neusoft.HISFC.Models.Fee.Item.Undrug)order.Item).IsNeedConfirm == false)
                //    {
                //        //更新执行档执行标志
                //        objExec.IsExec = true;
                //        objExec.ExecOper.ID = CurUser.ID;
                //        objExec.Order.ExeDept.ID = order.ExeDept.ID;
                //        objExec.ExecOper.OperTime = dtCurTime;
                //        //更新医嘱主档执行标志
                //        if (this.UpdateOrderExecuted(order.ID) < 0)
                //            return -1;
                //    }
                //}


                //----表现层记费----
                if (isCharge)
                {
                    //更新执行档记帐标志
                    objExec.IsCharge = true;
                    objExec.ChargeOper.ID = CurUser.ID;
                    objExec.ChargeOper.Dept.ID = CurUser.Dept.ID;
                    objExec.ChargeOper.OperTime = dtCurTime;

                    //对于已收费项目 设置执行标记为已执行。
                    objExec.IsExec = true;
                    objExec.ExecOper.ID = CurUser.ID;
                    objExec.Order.ExeDept.ID = order.ExeDept.ID;
                    objExec.ExecOper.OperTime = dtCurTime;

                    //更新医嘱主档执行标志
                    if (this.UpdateOrderExecuted(order.ID) < 0)
                        return -1;
                }

                //对不需要终端确认的 仍然需要对执行科室进行赋值
                objExec.ExecOper.ID = CurUser.ID;
                objExec.Order.ExeDept.ID = order.ExeDept.ID;
                objExec.ExecOper.Dept = order.ExeDept;
                objExec.ExecOper.OperTime = dtCurTime;

                //插入执行档
                if (execID == "")
                {
                    try
                    {
                        objExec.ID = GetNewOrderExecID();
                    }
                    catch { }
                }
                else
                {
                    objExec.ID = execID;
                }

                if (objExec.ID == "-1" || objExec.ID == "") return -1;


                if (JudgeItemType(objExec.Order) == "1")//药品置执行标记
                {
                    objExec.IsExec = true;
                    objExec.ExecOper.ID = CurUser.ID;
                    objExec.Order.ExeDept.ID = order.ExeDept.ID;
                    objExec.ExecOper.OperTime = dtCurTime;
                    //药品存最小单位
                    if (objExec.Order.Unit == ((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).MinUnit)//最小单位
                    {

                    }
                    else
                    {
                        objExec.Order.Qty = objExec.Order.Qty * objExec.Order.Item.PackQty; ;//变成最小单位
                        objExec.Order.Unit = ((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).MinUnit;
                    }
                }
                else//非药品
                {
                    /*
                     * 非药品的执行情况由是否需要终端确认决定，与收费与否无关
                     * 护士站确认收费只查询已执行标志的项目，终端项目不在护士站收费
                     */
                    objExec.IsExec = !objExec.Order.Item.IsNeedConfirm;
                }

                objExec.IsValid = true;
                objExec.DateUse = order.BeginTime;
                objExec.DateDeco = dtCurTime;
                objExec.DrugFlag = 0; //默认为不需要发送

                if (this.InsertExecOrder(objExec) < 0)
                {
                    return -1;
                }
            }
            #endregion


            //审核医嘱
            if (this.ConfirmOrder(order, isCharge, dtCurTime) < 0)
            {
                return -1;
            }
            return 0;

        }
        /// <summary>
        /// 重载审核函数
        /// </summary>
        /// <param name="order"></param>
        /// <param name="isCharge"></param>
        /// <param name="execID"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int ConfirmAndExecOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order, bool isCharge, string execID, DateTime dt)
        {
            this.dtCurTime = dt;
            return this.ConfirmAndExecOrder(order, isCharge, execID);
        }
        #endregion

        #region "医嘱分解"

        /// <summary>
        /// 当前所有医嘱
        /// </summary>
        public ArrayList AlAllOrders = new ArrayList();

        public Hashtable HsUsageAndTime = new Hashtable();

        /// <summary>
        /// 获得系统设置的分解时间
        /// </summary>
        /// <returns></returns>
        public void GetDecomposeTime(Neusoft.HISFC.Models.Order.Inpatient.Order order, ref int hour, ref int minute)
        {
            int hour_back = hour;
            int minute_back = minute;

            #region 非药品

            if (order.Item.ItemType != Neusoft.HISFC.Models.Base.EnumItemType.Drug)
            {
                if (order.IsSubtbl)
                {
                    //辅材去找对应的医嘱
                    string usageCode = "NONE";

                    foreach (Neusoft.HISFC.Models.Order.Inpatient.Order ord in this.AlAllOrders)
                    {
                        if (ord.Combo.ID == order.Combo.ID)
                        {
                            usageCode = ord.Usage.ID;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(usageCode))
                    {
                        usageCode = "NONE";
                    }

                    Neusoft.FrameWork.Models.NeuObject obj = new NeuObject();

                    obj = HsUsageAndTime[usageCode] as Neusoft.FrameWork.Models.NeuObject;

                    if (obj != null && obj.Memo.Length > 0)
                    {
                        try
                        {
                            hour = Neusoft.FrameWork.Function.NConvert.ToDateTime(obj.Memo).Hour;
                            minute = Neusoft.FrameWork.Function.NConvert.ToDateTime(obj.Memo).Minute;
                        }
                        catch
                        {
                            //没有维护就按照现在的控制参数来,外面已经传入时间，此处不做处理
                            hour = hour_back;//默认12点
                            minute = minute_back;
                        }
                    }
                    else
                    {
                        //没有维护就按照现在的控制参数来,外面已经传入时间，此处不做处理
                        hour = hour_back;//默认12点
                        minute = minute_back;
                    }
                }
                else
                {
                    //非药品如果有用法，取用法对应的分解时间，没有取NONE
                    Neusoft.FrameWork.Models.NeuObject obj = new NeuObject();

                    if (!string.IsNullOrEmpty(order.Usage.ID))
                    {
                        obj = HsUsageAndTime[order.Usage.ID] as Neusoft.FrameWork.Models.NeuObject;
                    }
                    else
                    {
                        obj = HsUsageAndTime["NONE"] as Neusoft.FrameWork.Models.NeuObject;
                    }

                    if (obj != null && obj.Memo.Length > 0)
                    {
                        try
                        {
                            hour = Neusoft.FrameWork.Function.NConvert.ToDateTime(obj.Memo).Hour;
                            minute = Neusoft.FrameWork.Function.NConvert.ToDateTime(obj.Memo).Minute;
                        }
                        catch
                        {
                            //没有维护就按照现在的控制参数来,外面已经传入时间，此处不做处理
                            hour = hour_back;//默认12点
                            minute = minute_back;
                        }
                    }
                    else
                    {
                        //没有维护就按照现在的控制参数来,外面已经传入时间，此处不做处理
                        hour = hour_back;//默认12点
                        minute = minute_back;
                    }
                }
            }
            #endregion

            #region 药品
            else
            {
                Neusoft.FrameWork.Models.NeuObject obj = new NeuObject();

                if (order.Usage.ID.Length > 0)
                {
                    obj = HsUsageAndTime[order.Usage.ID] as Neusoft.FrameWork.Models.NeuObject;
                }
                else
                {
                    obj = HsUsageAndTime["NONE"] as Neusoft.FrameWork.Models.NeuObject;
                }

                if (obj != null && obj.Memo.Length > 0)
                {
                    try
                    {
                        hour = Neusoft.FrameWork.Function.NConvert.ToDateTime(obj.Memo).Hour;
                        minute = Neusoft.FrameWork.Function.NConvert.ToDateTime(obj.Memo).Minute;
                    }
                    catch
                    {
                        //没有维护就按照现在的控制参数来,外面已经传入时间，此处不做处理
                        hour = hour_back;//默认12点
                        minute = minute_back;
                    }
                }
                else
                {
                    //没有维护就按照现在的控制参数来,外面已经传入时间，此处不做处理
                    hour = hour_back;//默认12点
                    minute = minute_back;
                }
            }
            #endregion
            return;
        }

        /// <summary>
        /// 医嘱分解到下次时间
        /// </summary>
        string s = null;

        /// <summary>
        /// 获得系统设置的分解时间
        /// </summary>
        /// <returns></returns>
        public void DecomposeTime(string nurseStationCode)
        {
            //if (nurseStationCode == strNurseStationCode)
            //{

            //}
            if (nurseStationCode == "")
            {

            }
            else //变化
            {
                //待更改
                controler.SetTrans(this.Trans);
                //医嘱分解到下次的时间
                if (string.IsNullOrEmpty(s))
                {
                    s = controler.QueryControlerInfo("200011", false);
                }
                this.strNurseStationCode = nurseStationCode;
                if (s == "-1" || s == "")
                {
                    iHour = 12;//默认12点
                    iMinute = 01;
                    return;
                }
                iHour = Neusoft.FrameWork.Function.NConvert.ToDateTime(s).Hour;
                iMinute = Neusoft.FrameWork.Function.NConvert.ToDateTime(s).Minute;
            }
            return;
        }

        /// <summary>
        /// {97FA5C9D-F454-4aba-9C36-8AF81B7C9CCF} 分解方法
        /// </summary>
        /// <param name="order"></param>
        /// <param name="days"></param>
        /// <param name="isCharge"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int DecomposeOrderToNow(Neusoft.HISFC.Models.Order.Inpatient.Order order, int days, bool isCharge, DateTime dt)
        {
            dtCurTime = dt;
            int myDays = 0;
            myDays = System.DateTime.Compare(order.NextMOTime.Date, dt.Date);
            return DecomposeOrder(order, days, isCharge);
        }


        /// <summary>
        /// 重载分解函数
        /// </summary>
        /// <param name="order"></param>
        /// <param name="days"></param>
        /// <param name="isCharge"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int DecomposeOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order, int days, bool isCharge, DateTime dt)
        {
            //dtCurTime = dt;

            //上次分解时间 也取设定值 houwb 2012-2-9 22:07:12
            //dtCurTime = new DateTime(dt.Year, dt.Month, dt.Day, 12, 01, 0);

            CurUser = (Neusoft.HISFC.Models.Base.Employee)this.Operator;
            DecomposeTime(CurUser.Nurse.ID);
            GetDecomposeTime(order, ref iHour, ref iMinute);

            dtCurTime = new DateTime(dt.Year, dt.Month, dt.Day, iHour, iMinute, 0);
            return DecomposeOrder(order, days, isCharge);
        }

        /// <summary>
        /// 当前收费员
        /// </summary>
        Neusoft.HISFC.Models.Base.Employee CurUser = null;

        Neusoft.HISFC.Models.Order.ExecOrder objExec = null;

        /// <summary>
        ///  药、非药嘱分解
        ///  Days分解天数IsCharge 是否收费
        /// </summary>
        /// <param name="order"></param>
        /// <param name="days">分解天数</param>
        /// <param name="isCharge">是否收费</param>
        /// <returns></returns>
        public int DecomposeOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order, int days, bool isCharge)
        {
            #region 医嘱分解时如果药品附材摆药计费，分解不收费，需要确认状态

            //是否使用药品执行、扣费分开流程 0 同时进行 1 不同时进行
            if (!isGet_bCharge)
            {
                bCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.controler.QueryControlerInfo("S00020", false));
                isGet_bCharge = true;
            }

            //摆药计费
            if (!bCharge)
            {
                if (order.IsSubtbl)
                {
                    //药品后计费时，药品附材是否与药品同时计费 1 护士站计费 0 药房计费
                    if (!this.isGet_bChargeSubtbl)
                    {
                        bChargeSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.controler.QueryControlerInfo("200050", false));
                        isGet_bChargeSubtbl = true;
                    }
                    //药品附材随药品后计费
                    if (!bChargeSubtbl)
                    {
                        ArrayList alSubtblOrder = this.QueryOrderByCombNO(order.Combo.ID, true);
                        if (alSubtblOrder == null || alSubtblOrder.Count <= 1)
                        {
                            this.Err = "根据附材查询主药出错！";
                            return -1;
                        }
                        foreach (Neusoft.HISFC.Models.Order.Inpatient.Order subtblOrder in alSubtblOrder)
                        {
                            if (!subtblOrder.IsSubtbl)
                            {
                                //确认其主药为药品
                                if (subtblOrder.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                                {
                                    order.Item.IsNeedConfirm = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            //获得当前操作员
            CurUser = (Neusoft.HISFC.Models.Base.Employee)this.Operator;

            objExec = new Neusoft.HISFC.Models.Order.ExecOrder();

            //赋值医嘱项目
            objExec.Order = order;

            //标记该项目是否是终端确认项目
            if (order.Item.GetType() == typeof(Neusoft.HISFC.Models.Fee.Item.Undrug))
            {
                objExec.Order.Item.IsNeedConfirm = ((Neusoft.HISFC.Models.Fee.Item.Undrug)order.Item).IsNeedConfirm;
            }

            //上次分解时间
            //DateTime dtCurDeco = new DateTime(dtCurTime.Year, dtCurTime.Month, dtCurTime.Day, iHour, iMinute, 00);
            DateTime dtCurDeco = dtCurTime;

            //分解数量
            decimal DecAmount = -1;

            //分解间隔天数
            int Cycle = 0;

            #region 分解长嘱

            //长期、已生效、分解时间小于指定时间的医嘱和附材(嘱托长嘱分解不收费)
            if (order.OrderType.IsDecompose
                && (order.Status == 1 || order.Status == 2)
                && order.NextMOTime <= dtCurDeco.AddDays(days))
            {

                //-------计算每次用量--------	
                if (order.OrderType.IsCharge)
                {
                    #region 需要计费、药品医嘱需要根据药品取整规则计算
                    if (order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item objPharmacy;
                        objPharmacy = (Neusoft.HISFC.Models.Pharmacy.Item)order.Item;

                        DecAmount = ComputeAmount(objPharmacy.ID, objPharmacy.DosageForm.ID, order.DoseOnce, objPharmacy.BaseDose, order.Patient.PVisit.PatientLocation.Dept.ID);
                        if (DecAmount >= 0)
                        {
                            order.Item.Qty = DecAmount;
                        }
                    }
                    #endregion
                    //不需计费、药品医嘱直接获得每次用量
                    //非药品（附材）直接获得每次执行数量		
                }

                //分解间隔天数
                Cycle = System.Convert.ToInt16(order.Frequency.Days[0]);

                if (order.Frequency.Days.Length > 1)//分解日期为星期
                {
                    Cycle = 1;
                }

                //是否已分解，分解后更新分解时间
                bool bIsHaveDecompose = false;

                //分解插入执行档次数
                int iMOCount = 0;

                //获得默认的分解截止时间点
                DecomposeTime(CurUser.Nurse.ID);
                this.GetDecomposeTime(order, ref iHour, ref iMinute);


                //小时计费医嘱的频次
                if (string.IsNullOrEmpty(this.hourFerquenceID))
                {
                    this.hourFerquenceID = this.controler.QueryControlerInfo("200042", false);
                    if (string.IsNullOrEmpty(hourFerquenceID) == true)
                    {
                        this.hourFerquenceID = "NONE";
                    }
                }

                //计时医嘱分解时间到本次分解时间＋days
                if (order.Frequency.ID == this.hourFerquenceID)
                {
                    iHour = dtCurDeco.Hour;
                    iMinute = dtCurDeco.Minute;
                }

                //结束时间
                DateTime dtTemp = dtCurDeco.AddDays(days);
                DateTime dtEndTime;

                dtEndTime = new DateTime(dtTemp.Year, dtTemp.Month, dtTemp.Day, iHour, iMinute, 0);

                //下次执行日期<指定的日期＋分解天数？
                int Count = System.Convert.ToInt16(new TimeSpan(order.NextMOTime.Date.Ticks - dtCurDeco.Date.Ticks).TotalDays);

                if (order.Frequency.Days.Length > 1)
                {
                    #region 分解医嘱为星期的
                    for (int i = 0; i <= days; i++)
                    {
                        for (int k = 1; k < order.Frequency.Days.Length; k++)//循环找星期
                        {
                            int week = dtCurTime.AddDays(i).DayOfWeek.GetHashCode();
                            if (week == 0)
                                week = 7;

                            if (order.Frequency.Days[k] == week.ToString())//找到更新的
                            {
                                if (this.DecomposeTime(order, dtCurDeco, dtEndTime, isCharge, objExec, dtCurTime, CurUser, i) == -1)
                                {
                                    return -1;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 分解医嘱为正常每天的
                    while (Count <= (days + Cycle - 1))
                    {
                        #region 分解时间
                        for (int i = 0; i <= order.Frequency.Times.GetUpperBound(0); i++)
                        {
                            DateTime dt = new DateTime();
                            try
                            {
                                dt = Neusoft.FrameWork.Function.NConvert.ToDateTime(order.Frequency.Times[i]);
                            }
                            catch
                            {
                            }
                            if (dt.GetType().ToString() != "System.DateTime")
                            {
                                return -1;
                            }
                            DateTime dtUseTime = new DateTime(dtCurDeco.AddDays(Count).Year, dtCurDeco.AddDays(Count).Month, dtCurDeco.AddDays(Count).Day, dt.Hour, dt.Minute, dt.Second);

                            //服药时间>=下次执行日期and服药时间<分解结束时间？
                            //wolf 更改了，不知道会不会死掉,必须靠唯一索引来锁重复记录Date_NexMO
                            if (dtUseTime >= order.NextMOTime && dtUseTime < dtEndTime)
                            {
                                //附材的预停止处理
                                if (order.IsSubtbl)
                                {
                                    ArrayList al = this.QueryOrderByCombNO(order.Combo.ID, false);
                                    if (al.Count > 0)
                                    {
                                        Neusoft.HISFC.Models.Order.Inpatient.Order subOrder = al[0] as Neusoft.HISFC.Models.Order.Inpatient.Order;
                                        if (dtUseTime > subOrder.EndTime && subOrder.EndTime != DateTime.MinValue)
                                        {
                                            order.EndTime = subOrder.EndTime;
                                        }
                                    }
                                }



                                //服药时间是否大于医嘱停止时间?
                                if (dtUseTime > order.EndTime && order.EndTime != DateTime.MinValue)
                                {
                                    //停止作废医嘱
                                    order.Status = 3;
                                    order.DCOper.OperTime = order.EndTime;
                                    if (DcOneOrder(order) < 0)
                                        return -1;
                                }
                                else
                                {
                                    //----表现层记费----
                                    if (isCharge)
                                    {
                                        //更新执行档记帐标志
                                        objExec.IsCharge = true;
                                        objExec.ChargeOper.ID = CurUser.ID;
                                        objExec.ChargeOper.Dept.ID = CurUser.Dept.ID;
                                        objExec.ChargeOper.OperTime = dtCurTime;
                                    }
                                    //插入执行档
                                    try
                                    {
                                        objExec.ID = GetNewOrderExecID();
                                    }
                                    catch { }
                                    if (objExec.ID == "-1" || objExec.ID == "") return -1;
                                    objExec.IsValid = true;
                                    objExec.DateUse = dtUseTime;
                                    objExec.DateDeco = dtCurTime;
                                    objExec.DrugFlag = 0; //默认为不需要发送

                                    if (objExec.Order.Item.GetType() == typeof(Neusoft.HISFC.Models.Pharmacy.Item))//药品
                                    {
                                        try
                                        {   //数量通过基本剂量找最小单位便于计算费用
                                            //原程序判断的Frequency.User01 更改为使用 ExecDose属性
                                            //if(objExec.Order.Frequency.User01 != "" && objExec.Order.Frequency.User01 != null) //特殊频次
                                            if (objExec.Order.ExecDose != "" && objExec.Order.ExecDose != null) //特殊频次
                                            {
                                                string[] tempDoseOnce = objExec.Order.ExecDose.Split('-');
                                                decimal tempDoseOnceDec = Convert.ToDecimal(tempDoseOnce[i]);
                                                objExec.Order.Qty = tempDoseOnceDec / ((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).BaseDose;
                                                objExec.Order.DoseOnce = tempDoseOnceDec;
                                                if (objExec.Order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)//(objExec.Order.Item.GetType().ToString() == "Neusoft.HISFC.Models.Pharmacy.Item")
                                                {
                                                    decimal decAmount = 0;
                                                    Neusoft.HISFC.Models.Pharmacy.Item objPharmacy;
                                                    objPharmacy = (Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item;
                                                    if (objExec.Order.Item.ID == "999")
                                                    {
                                                        if (objExec.Order.Item.Qty == 0)
                                                        {
                                                            objExec.Order.Item.Qty = objExec.Order.DoseOnce;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        decAmount = ComputeAmount(objPharmacy.ID, objPharmacy.DosageForm.ID, tempDoseOnceDec, objPharmacy.BaseDose, order.Patient.PVisit.PatientLocation.Dept.ID);
                                                        if (decAmount >= 0)
                                                        {
                                                            objExec.Order.Item.Qty = decAmount;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (objExec.Order.Item.ID == "999")
                                                {
                                                    if (objExec.Order.Item.Qty == 0)
                                                    {
                                                        objExec.Order.Item.Qty = objExec.Order.DoseOnce;
                                                    }
                                                }
                                                else
                                                {
                                                    if (DecAmount != -1)
                                                        objExec.Order.Qty = DecAmount;
                                                    else
                                                        objExec.Order.Qty = objExec.Order.DoseOnce / ((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).BaseDose;
                                                }
                                            }
                                            if (objExec.Order.Item.ID == "999")
                                            {
                                                if (string.IsNullOrEmpty(((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).MinUnit))
                                                {
                                                    objExec.Order.DoseUnit = ((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).DoseUnit;
                                                }
                                                if (string.IsNullOrEmpty(((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).MinUnit))
                                                {
                                                    ((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).MinUnit = objExec.Order.DoseUnit;
                                                }
                                            }
                                            objExec.Order.Unit = ((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).MinUnit;
                                        }
                                        catch
                                        {
                                            this.Err = "剂量为零。";
                                            this.WriteErr();
                                        }
                                    }


                                    //对于Q1H的 开始时间为整点的，开始时间这次不收钱，不算执行
                                    if (objExec.DateUse == order.BeginTime && order.Frequency.ID.ToUpper() == "Q1H")
                                    {
                                    }
                                    else
                                    {
                                        //插入执行档
                                        if (this.InsertExecOrder(objExec) == -1)
                                        {
                                            if (this.DBErrCode != 1)
                                                return -1;
                                        }
                                    }
                                    iMOCount++;//{F1C96C96-F829-4ea1-AD07-10DBCC091C16}分解次数
                                    bIsHaveDecompose = true;//{F1C96C96-F829-4ea1-AD07-10DBCC091C16}
                                }
                            }
                        }
                        #endregion
                        Count = Count + Cycle;
                    }
                    #endregion
                }

                #region 处理下次更新时间

                DateTime dtNex = new DateTime();

                //{97FA5C9D-F454-4aba-9C36-8AF81B7C9CCF}
                //将医嘱置下次分解时间Days + Cycle
                if (days < Cycle)
                {
                    if (days == 0)
                    {
                        //只分解到当天,下次分解时间赋成当天,
                    }
                    else
                    {
                        days = Cycle;
                    }
                }


                if (Cycle > 1)
                {
                    //{F1C96C96-F829-4ea1-AD07-10DBCC091C16}修改QOD（20）问题
                    if (bIsHaveDecompose)
                    {
                        //如果频次的基本天数〉1医嘱的下次分解时间应该加上（本次分解次数*频次的基本天数）天
                        dtNex = order.NextMOTime.AddDays(iMOCount * Cycle);

                        order.NextMOTime = new DateTime(dtNex.Year, dtNex.Month, dtNex.Day, 0, 0, 0);

                    }
                    else
                    {
                    }
                }
                else
                {
                    dtNex = dtCurDeco.AddDays(days);

                    order.NextMOTime = new DateTime(dtNex.Year, dtNex.Month, dtNex.Day, iHour, iMinute, 0);
                }

                order.CurMOTime = dtCurDeco;

                //更新分解时间（本次，下次）
                if (UpdateDecoTime(order) <= 0)
                {
                    return -1;
                }
                #endregion
            }
            #endregion

            return 0;
        }

        /// <summary>
        /// 按星期分解医嘱
        /// </summary>
        /// <param name="order"></param>
        /// <param name="dtCurDeco"></param>
        /// <param name="dtEndTime"></param>
        /// <param name="isCharge"></param>
        /// <param name="objExec"></param>
        /// <param name="dtCurTime"></param>
        /// <param name="curUser"></param>
        /// <param name="addDays"></param>
        /// <returns></returns>
        private int DecomposeTime(Neusoft.HISFC.Models.Order.Inpatient.Order order, DateTime dtCurDeco, DateTime dtEndTime, bool isCharge, Neusoft.HISFC.Models.Order.ExecOrder objExec, DateTime dtCurTime, Neusoft.HISFC.Models.Base.Employee curUser, int addDays)
        {
            #region 分解时间
            for (int i = 0; i <= order.Frequency.Times.GetUpperBound(0); i++)
            {
                DateTime dt = new DateTime();
                try
                {
                    dt = Neusoft.FrameWork.Function.NConvert.ToDateTime(order.Frequency.Times[i]);
                }
                catch
                {
                }
                if (dt.GetType().ToString() != "System.DateTime")
                {
                    return -1;
                }
                DateTime dtUseTime = new DateTime(dtCurDeco.AddDays(addDays).Year, dtCurDeco.AddDays(addDays).Month, dtCurDeco.AddDays(addDays).Day, dt.Hour, dt.Minute, dt.Second);

                //服药时间>=下次执行日期and服药时间<分解结束时间？
                if (dtUseTime >= order.NextMOTime && dtUseTime < dtEndTime)
                {
                    //服药时间是否大于医嘱停止时间?
                    if (dtUseTime > order.EndTime && order.EndTime != DateTime.MinValue)
                    {
                        //停止作废医嘱
                        order.Status = 3;
                        order.DCOper.OperTime = order.EndTime;
                        if (DcOneOrder(order) < 0) return 0;
                    }
                    else
                    {
                        //----表现层记费----
                        if (isCharge)
                        {
                            //更新执行档记帐标志
                            objExec.IsCharge = true;
                            objExec.ChargeOper.ID = curUser.ID;
                            objExec.ChargeOper.Dept.ID = curUser.Dept.ID;
                            objExec.ChargeOper.OperTime = dtCurTime;
                        }
                        //插入执行档
                        try
                        {
                            objExec.ID = GetNewOrderExecID();
                        }
                        catch
                        {
                        }
                        if (objExec.ID == "-1" || objExec.ID == "") return 0;
                        objExec.IsValid = true;
                        objExec.DateUse = dtUseTime;
                        objExec.DateDeco = dtCurTime;
                        objExec.DrugFlag = 0; //默认为不需要发送

                        if (objExec.Order.Item.GetType() == typeof(Neusoft.HISFC.Models.Pharmacy.Item))//药品
                        {
                            try
                            {   //数量通过基本剂量找最小单位便于计算费用
                                objExec.Order.Qty = objExec.Order.DoseOnce / ((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).BaseDose;
                                objExec.Order.Unit = ((Neusoft.HISFC.Models.Pharmacy.Item)objExec.Order.Item).MinUnit;
                            }
                            catch
                            {
                                this.Err = Neusoft.FrameWork.Management.Language.Msg("剂量为零。");
                                this.WriteErr();
                            }
                        }

                        if (this.InsertExecOrder(objExec) == -1)
                        {
                            return -1;
                        }
                    }
                }
            }
            return 0;
            #endregion
        }

        /// <summary>
        /// 获得分解取整标准
        /// </summary>
        /// <param name="drugCode"></param>
        /// <param name="doseCode"></param>
        /// <param name="doseOnce"></param>
        /// <param name="baseDose"></param>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public decimal ComputeAmount(string drugCode, string doseCode, decimal doseOnce, decimal baseDose, string deptCode)
        {
            #region 获得分解取整标准
            //获得分解取整标准
            //Order.Order.ComputeAmount.1
            //传入：0 DrugCode
            //传出：0 UNIT_STAT
            #endregion

            #region {AD50C155-BE2D-47b8-8AF9-4AF3548A2726}
            //性能优化
            string UnitSate = string.Empty;

            if (htDrugedProperty.Contains(drugCode + deptCode))
            {
                UnitSate = htDrugedProperty[drugCode + deptCode].ToString();
            }
            else
            {
                UnitSate = this.GetDrugProperty(drugCode, doseCode, deptCode);

                htDrugedProperty.Add(drugCode + deptCode, UnitSate);
            }
            #endregion

            decimal Amount = 0;
            if (baseDose == 0)
                return Amount;

            //0 不可拆分 1 可拆分不取整 2 可拆分上取整
            switch (UnitSate)
            {
                case "0"://不可以，向上取整
                    Amount = (decimal)System.Math.Ceiling((decimal)doseOnce / (decimal)baseDose);
                    //Amount = (decimal)System.Math.Ceiling((double)((decimal)doseOnce / (decimal)baseDose));
                    break;
                case "1"://可以，配药时不取整
                    Amount = System.Convert.ToDecimal(doseOnce) / baseDose;
                    break;
                case "2"://可以，配药时上取整 
                    Amount = System.Convert.ToDecimal(doseOnce) / baseDose;
                    break;
                default://
                    Amount = System.Convert.ToDecimal(doseOnce) / baseDose;
                    break;
            }
            return Amount;
        }

        /// <summary>
        /// 获取药品配药属性
        /// </summary>
        /// <param name="drugCode">药品编码</param>
        /// <param name="deptCode">科室编码</param>
        /// <returns>成功返回配药属性 0 不可拆分 1 可拆分不取整 2 可拆分上取整，失败返回NULL</returns>
        public string GetDrugProperty(string drugCode, string doseCode, string deptCode)
        {
            string strSQL = "";
            //取SELECT语句
            if (this.Sql.GetCommonSql("Pharmacy.Item.GetDrugProperty", ref strSQL) == -1)
            {
                this.Err = "没有找到Pharmacy.Item.GetDrugProperty字段!";
                return null;
            }

            //格式化SQL语句
            try
            {
                strSQL = string.Format(strSQL, drugCode, doseCode, deptCode);
            }
            catch (Exception ex)
            {
                this.Err = "格式化SQL语句时出错Pharmacy.Item.GetDrugProperty:" + ex.Message;
                return null;
            }

            //执行查询语句
            if (this.ExecQuery(strSQL) == -1)
            {
                this.Err = "获得配药属性信息时，执行SQL语句出错！" + this.Err;
                this.ErrCode = "-1";
                return null;
            }
            string drugProperty = "";
            if (this.Reader.Read())
            {
                drugProperty = this.Reader[0].ToString();
            }
            else
            {
                drugProperty = "1";
            }
            this.Reader.Close();
            return drugProperty;
        }

        /// <summary>
        /// 更新分解时间（本次，下次）
        /// </summary>
        /// <param name="order">医嘱id,(本次，下次)分解时间</param>
        /// <returns></returns>
        public int UpdateDecoTime(Neusoft.HISFC.Models.Order.Order order)
        {
            #region 更新分解时间
            //更新分解时间
            //Order.Order.UpdateDecoTime.1
            //传入：0 orderID 1 Date_CurMO 2 Date_NexMO
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.UpdateDecoTime.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, order.ID, order.CurMOTime.ToString(), order.NextMOTime.ToString());
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.UpdateDecoTime.1";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 下次分解时间
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="days">+_天数</param>
        /// <returns></returns>
        public int UpdateDecoTime(string inpatientNo, int days)
        {
            #region 更新分解时间
            //更新分解时间
            //Order.Order.UpdateDecoTime.2
            //传入：0 inpatientNo 1 days
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.UpdateDecoTime.2", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                if (days > 0)
                    strSql = string.Format(strSql, inpatientNo, "+" + days.ToString());
                else
                    strSql = string.Format(strSql, inpatientNo, days.ToString());
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.UpdateDecoTime.2";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 下次分解时间
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="nextDecoDateTime">下次分解时间</param>
        /// <returns></returns>
        public int UpdateDecoTime(string inpatientNo, DateTime nextDecoDateTime)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.UpdateDecoTime.3", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, inpatientNo, nextDecoDateTime.ToString());
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.UpdateDecoTime.3";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        //{C685B311-7747-45fa-A62C-E53C24B67CAD}
        /// <summary>
        /// 查询是否存在同一天内同一护理级别的其他计费信息
        /// </summary>
        /// <param name="unDrugCode">项目代码</param>
        /// <param name="useTime">执行时间</param>
        /// <returns>成功返回所有条件相同的执行流水号数组，否则NULL</returns>
        public ArrayList SelectIfExistTheSameUN(string unDrugCode, DateTime useTime, string patientID)
        {
            string strSql = "";
            ArrayList sqnList = new ArrayList();
            Neusoft.FrameWork.Models.NeuObject obj = null;
            if (this.Sql.GetCommonSql("Order.ExecOrder.SelectIfExistTheSameUN", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                strSql = string.Format(strSql, unDrugCode, useTime.ToString(), patientID);
                this.ExecQuery(strSql);
                while (this.Reader.Read())
                {
                    obj = new NeuObject();
                    obj.ID = this.Reader[0].ToString(); //执行单流水号
                    obj.Memo = this.Reader[1].ToString(); //医嘱流水号
                    sqnList.Add(obj);
                }
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.SelectIfExistTheSameUN";
                this.WriteErr();
                return null;
            }
            return sqnList;
        }

        #region {AD50C155-BE2D-47b8-8AF9-4AF3548A2726}

        /// <summary>
        /// 配药属性表
        /// </summary>
        private Hashtable htDrugedProperty = new Hashtable();

        /// <summary>
        /// 为了优化性能整合的医嘱分解保存时更新药品执行档用的函数
        /// </summary>
        /// <param name="execOrder"></param>
        /// <param name="isExec">更新时是否是已执行状态，欠费患者的确认收费，是对已执行未收费执行档进行收费</param>
        /// <returns></returns>
        public int UpdateForConfirmExecDrugNew(Neusoft.HISFC.Models.Order.ExecOrder execOrder, bool isExec)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.UpdateForConfirmExecDrug.1.New", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrder.ID,//执行档ID
                    execOrder.DrugFlag,//药品发送标记
                    execOrder.ChargeOper.ID,//记账人代码
                    execOrder.ChargeOper.Dept.ID,//记账科室代码
                    execOrder.ChargeOper.OperTime.ToString(),//记账时间
                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsCharge).ToString(),//记账标记
                    execOrder.Order.ReciptNO,//处方号
                    execOrder.Order.SequenceNO,//处方流水号 
                    execOrder.ExecOper.ID, //执行护士代码
                    execOrder.Order.ExeDept.ID, //执行科室代码
                    execOrder.ExecOper.OperTime.ToString(), //执行时间
                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsExec).ToString(),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(isExec).ToString(),//0 医嘱执行；1 医嘱确认收费
                    execOrder.Order.StockDept.ID
                                         );//执行标记
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.UpdateForConfirmExecDrug.1.New";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 为了优化性能整合的医嘱分解保存时更新药品执行档用的函数
        /// </summary>
        /// <param name="execOrder"></param>
        /// <param name="isExec">更新时是否是已执行状态，欠费患者的确认收费，是对已执行未收费执行档进行收费</param>
        /// <returns></returns>
        public int UpdateForConfirmExecDrug(Neusoft.HISFC.Models.Order.ExecOrder execOrder, bool isExec)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.UpdateForConfirmExecDrug.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrder.ID,//执行档ID
                    execOrder.DrugFlag,//药品发送标记
                    execOrder.ChargeOper.ID,//记账人代码
                    execOrder.ChargeOper.Dept.ID,//记账科室代码
                    execOrder.ChargeOper.OperTime.ToString(),//记账时间
                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsCharge).ToString(),//记账标记
                    execOrder.Order.ReciptNO,//处方号
                    execOrder.Order.SequenceNO,//处方流水号 
                    execOrder.ExecOper.ID, //执行护士代码
                    execOrder.Order.ExeDept.ID, //执行科室代码
                    execOrder.ExecOper.OperTime.ToString(), //执行时间
                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsExec).ToString(),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(isExec).ToString()//0 医嘱执行；1 医嘱确认收费
                                         );//执行标记
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.UpdateForConfirmExecDrug.1";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 为了优化性能整合的医嘱分解保存时更新药品执行档用的函数
        /// </summary>
        /// <param name="execOrder">药品执行档</param>
        /// <returns></returns>
        [Obsolete("作废，貌似没用，不推荐这种模式", true)]
        public int UpdateForConfirmExecDrugNoExecFlag(Neusoft.HISFC.Models.Order.ExecOrder execOrder)
        {
            string strSql = @"UPDATE met_ipm_execdrug --药嘱执行档
                               SET druged_flag   = '{1}', --1不需发送/2集中发送/3分散发送/4已配药
                                   charge_usercd = '{2}', --记账人代码
                                   charge_deptcd = '{3}', --记账科室代码
                                   charge_date   = to_date('{4}', 'yyyy-mm-dd HH24:mi:ss'), --记账时间
                                   charge_flag   = '{5}', --记账标记1待记账/2已
                                   RECIPE_NO     = '{6}', --处方号
                                   SEQUENCE_NO   = {7}, --处方流水序号
                                   exec_usercd   = '{8}', --执行护士代码
                                   exec_deptcd   = '{9}', --执行科室代码
                                   exec_date     = to_date('{10}', 'yyyy-mm-dd HH24:mi:ss'), --执行时间
                                   exec_flag     = '{11}' --0待执行/1已                   
                             WHERE exec_sqn = '{0}'
                               and valid_flag = fun_get_valid --and exec_flag = '0' ";
            //if (this.Sql.GetCommonSql("Order.Order.UpdateForConfirmExecDrug.1", ref strSql) == -1)
            //{
            //    this.Err = this.Sql.Err;
            //    return -1;
            //}
            try
            {
                strSql = string.Format(strSql, execOrder.ID,//执行档ID
                                         execOrder.DrugFlag,//药品发送标记
                                         execOrder.ChargeOper.ID,//记账人代码
                                         execOrder.ChargeOper.Dept.ID,//记账科室代码
                                         execOrder.ChargeOper.OperTime.ToString(),//记账时间
                                         Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsCharge).ToString(),//记账标记
                                         execOrder.Order.ReciptNO,//处方号
                                         execOrder.Order.SequenceNO,//处方流水号 
                                         execOrder.ExecOper.ID, //执行护士代码
                                         execOrder.Order.ExeDept.ID, //执行科室代码
                                         execOrder.ExecOper.OperTime.ToString(), //执行时间
                                         Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsExec).ToString());//执行标记
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.UpdateForConfirmExecDrug.1";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 为了优化性能整合的医嘱分解保存时更新非药品执行档用的函数
        /// </summary>
        /// <param name="execOrder">非药品执行档</param>
        /// <param name="isExec">更新时是否是已执行状态，欠费患者的确认收费，是对已执行未收费执行档进行收费</param>
        /// <returns></returns>
        public int UpdateForConfirmExecUnDrug(Neusoft.HISFC.Models.Order.ExecOrder execOrder, bool isExec)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.UpdateForConfirmExecUnDrug.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrder.ID,//执行档ID
                    execOrder.ChargeOper.ID,//记账人代码
                    execOrder.ChargeOper.Dept.ID,//记账科室代码
                    execOrder.ChargeOper.OperTime.ToString(),//记账时间
                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsCharge).ToString(),//记账标记
                    execOrder.Order.ReciptNO,//处方号
                    execOrder.Order.SequenceNO,//处方流水号 
                    execOrder.ExecOper.ID, //执行护士代码
                    execOrder.Order.ExeDept.ID, //执行科室代码
                    execOrder.ExecOper.OperTime.ToString(), //执行时间
                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsExec).ToString(),//执行标记
                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsConfirm).ToString(),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(isExec).ToString() //0 医嘱执行；1 医嘱确认收费
                    );//更新确认标记

            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.UpdateForConfirmExecUnDrug.1";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        #endregion

        #endregion

        #region "医嘱停止处理"
        /// <summary>
        /// 停止医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="sysClass"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int DcOrder(string inpatientNo, Neusoft.HISFC.Models.Base.SysClassEnumService sysClass, DateTime dt)
        {
            ArrayList al = new ArrayList();
            DateTime dtBegin = new DateTime(2005, 1, 1);
            al = this.QueryOrder(inpatientNo, dtBegin, dt);
            if (al == null) return -1;
            foreach (Neusoft.HISFC.Models.Order.Inpatient.Order order in al)
            {
                if (order.Status == 1 || order.Status == 0)//有效，非作废,非执行医嘱。
                {
                    if (sysClass == null)//全部都停止
                    {
                        order.Status = 3;
                        order.DCOper.OperTime = dt;
                        order.DCOper.ID = this.Operator.ID;
                        order.DCOper.Name = this.Operator.Name;
                        order.DcReason.ID = "0";
                        if (order.OrderType.IsDecompose)
                        {
                            if (this.DcOneOrder(order) == -1)
                            {
                                return -1;
                            }
                        }
                    }
                    else //类别停止 //停止指定的长期医嘱  如：护理等等。
                    {
                        if (order.Item.SysClass.ID.ToString() == sysClass.ID.ToString() && order.OrderType.IsDecompose)
                        {
                            order.Status = 3;
                            order.DCOper.OperTime = dt;
                            order.DCOper.ID = this.Operator.ID;
                            order.DCOper.Name = this.Operator.Name;
                            order.DcReason.ID = "0";
                            if (this.DcOneOrder(order) == -1)
                            {
                                return -1;
                            }
                        }
                    }
                }
            }
            this.Err = "医嘱停止成功！";
            return 0;
        }
        /// <summary>
        /// 停止医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int DcOrder(string inpatientNo, DateTime dt)
        {
            return DcOrder(inpatientNo, null, dt);
        }
        /// <summary>
        /// 停止医嘱处理（包括直接停止，预停止）
        /// 停止原因Order.DcReason
        /// </summary>
        /// <param name="order"></param>
        /// <param name="isAllDc">组合医嘱是否全部停止</param>
        /// <param name="ReturnMemo">返回医嘱原始状态</param>
        /// <returns></returns>
        public int DcOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order, bool isAllDc, out string ReturnMemo)
        {
            //返回医嘱执行情况
            ReturnMemo = "";
            if (order.Status == 2)
            {
                ReturnMemo = "该条医嘱已经已经执行，请确认后退费！";
            }
            //是否是组合医嘱
            ArrayList al = new ArrayList();
            al = this.QueryOrderByCombNO(order.Combo.ID, false);

            ArrayList temp = this.QueryOrderByCombNO(order.Combo.ID, true);
            al.AddRange(temp);
            //是否是组合医嘱
            if (al.Count > 1)
            {
                //是否停止组合项目中的单项
                if (isAllDc)
                {
                    Neusoft.HISFC.Models.Order.Inpatient.Order obj;
                    for (int i = 0; i < al.Count; i++)
                    {
                        obj = (Neusoft.HISFC.Models.Order.Inpatient.Order)al[i];
                        //本记录在下面处理
                        if (obj.ID == order.ID)
                            continue;
                        //执行单条医嘱DC
                        obj.DCOper = order.DCOper;
                        obj.DcReason = order.DcReason;
                        if (obj.EndTime == DateTime.MinValue)
                        {
                            obj.EndTime = order.DCOper.OperTime;
                        }
                        obj.Status = 3;
                        if (DcOneOrderDeal(obj) < 0)
                            return -1;
                    }
                }
                else
                {
                    //是否主药
                    if (order.Combo.IsMainDrug)
                    {
                        //主药医嘱不能单条DC
                        this.Err = "不能单独作废主药医嘱！";
                        return -1;
                    }
                }
            }
            //else非组合项目执行单条医嘱作废
            //执行单条医嘱DC

            if (order.EndTime == DateTime.MinValue)
            {
                order.EndTime = order.DCOper.OperTime;
            }
            if (DcOneOrderDeal(order) < 0)
                return -1;
            return 0;

        }

        /// <summary>
        /// 获取出院医嘱（包含“出院”两个字的临嘱）
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public int GetOutOrder(string inpatientNo, ref Neusoft.HISFC.Models.Order.Inpatient.Order order)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null)
            {
                return -1;
            }

            try
            {
                if (this.Sql.GetCommonSql("Order.Order.QueryOrder.OutOrder", ref sql1) == -1)
                {
                    this.Err = "没有找到[Order.Order.QueryOrder.OutOrder]字段!";
                    this.ErrCode = "-1";
                    this.WriteErr();
                    return -1;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo);

                ArrayList alOrder = this.MyOrderQuery(sql);
                if (alOrder == null)
                {
                    return -1;
                }
                else if (alOrder.Count <= 0)
                {
                    return 0;
                }
                else
                {
                    order = alOrder[0] as Neusoft.HISFC.Models.Order.Inpatient.Order;
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
        /// 获取转科医嘱（包含“转科”两个字的临嘱）
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public int GetShiftOutOrder(string inpatientNo, ref Neusoft.HISFC.Models.Order.Inpatient.Order order)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null)
            {
                return -1;
            }

            try
            {
                if (this.Sql.GetCommonSql("Order.Order.QueryOrder.ShiftOutOrder", ref sql1) == -1)
                {
                    this.Err = "没有找到[Order.Order.QueryOrder.ShiftOutOrder]字段!";
                    this.ErrCode = "-1";
                    this.WriteErr();
                    return -1;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo);

                ArrayList alOrder = this.MyOrderQuery(sql);
                if (alOrder == null)
                {
                    return -1;
                }
                else if (alOrder.Count <= 0)
                {
                    return 0;
                }
                else
                {
                    order = alOrder[0] as Neusoft.HISFC.Models.Order.Inpatient.Order;
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
        /// 获取病人在病区所产生的数据
        /// add by yerl
        /// </summary>
        public int GetFeeInfoCount(string inpatientNo)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Order.OrderCount", ref sql) < 0)
            {
                return -1;
            }
            sql = string.Format(sql, inpatientNo);
            string result = this.ExecSqlReturnOne(sql);
            int rtn = -1;
            int.TryParse(result, out rtn);
            return rtn;
        }
        /// <summary>
        /// 获取转科医嘱（包含转科类型的医嘱）
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public int GetShiftOutOrderType(string inpatientNo, ref Neusoft.HISFC.Models.Order.Inpatient.Order order)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null)
            {
                return -1;
            }

            try
            {
                if (this.Sql.GetCommonSql("Order.Order.QueryOrder.GetShiftOutOrderType", ref sql1) == -1)
                {
                    //                    sql1 = @" where inpatient_no='{0}'
                    //                                    and (class_code = 'MRD'
                    //                                    or item_name like '%转科%')
                    //                                    and decmps_state='0'";

                    sql1 = @" where inpatient_no='{0}'
                                   and (class_code = 'MRD'
                                  or item_name like '%转科%')
                                    and decmps_state='0'
                                  AND  dept_code=( SELECT t.dept_code FROM  FIN_IPR_INMAININFO t WHERE t.inpatient_no='{0}' ) "

                        ;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo);

                ArrayList alOrder = this.MyOrderQuery(sql);
                if (alOrder == null)
                {
                    return -1;
                }
                else if (alOrder.Count <= 0)
                {
                    return 0;
                }
                else
                {
                    order = alOrder[0] as Neusoft.HISFC.Models.Order.Inpatient.Order;
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
        /// 检查患者是否有开立过医嘱
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns>-1 出错  0 没有医嘱  1 已开立过医嘱</returns>
        public int IsOwnOrders(string inpatientNo)
        {
            string sql = OrderQuerySelect();
            if (sql == null)
            {
                return -1;
            }

            string sqlWhere = "";
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.IsOwnOrders", ref sqlWhere) == -1)
            {
                sqlWhere = @" where inpatient_no = '{0}'";
            }
            sql = sql + " " + string.Format(sqlWhere, inpatientNo);

            ArrayList alOrder = this.MyOrderQuery(sql);
            if (alOrder == null)
            {
                return -1;
            }
            else if (alOrder.Count == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 护士站出院登记，自动停止全部长嘱
        /// 停止后，为已停止、已审核状态
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="dcDoct">停止医生</param>
        /// <param name="confirmNurse">审核护士</param>
        /// <param name="dcReasonCode">停止原因编码</param>
        /// <param name="dcReasonName">停止原因</param>
        /// <returns></returns>
        public int AutoDcOrder(string inpatientNo, Neusoft.FrameWork.Models.NeuObject dcDoct, Neusoft.FrameWork.Models.NeuObject confirmNurse, string dcReasonCode, string dcReasonName)
        {
            #region SQL
            //update met_ipm_order
            //set 
            //dc_flag='1', --停止标记
            //date_end=to_date('{1}','yyyy-mm-dd	HH24:mi:ss'),--医嘱结束时间
            //dc_date=to_date('{1}','yyyy-mm-dd	HH24:mi:ss'), --停止时间
            //dc_code='{2}',--DC原因代码
            //dc_name='{3}',--DC原因名称
            //dc_doccd='{4}',--DC医师代码
            //dc_docnm='{5}',--DC医师姓名
            //dc_usercd='{6}',--Dc人员代码
            //dc_usernm='{7}',--Dc人员名称
            //DC_CONFIRM_FLAG  ='1', --出院登记停止 自动审核
            //DC_CONFIRM_DATE = to_date('{8}','yyyy-mm-dd	HH24:mi:ss'),	--确认时间
            //DC_CONFIRM_OPER='{9}'	 --确认人员代码
            //where inpatient_no='{0}'--住院流水号
            //and decmps_state='1' --长嘱
            //and mo_stat in ('1','2') --只停止已审核和已执行的长嘱
            #endregion

            string strSql = "";
            //停止全部长嘱
            if (this.Sql.GetCommonSql("Order.Order.DcOrder.AutoDcAllLong", ref strSql) == -1)
            {
                this.Err = "找不到SQL语句:Order.Order.DcOrder.AutoDcAllLong";
                return -1;
            }
            try
            {
                strSql = string.Format(strSql,
                                        inpatientNo,        //患者住院流水号
                                        this.GetDateTimeFromSysDateTime(),//医嘱结束时间
                    //this.GetDateTimeFromSysDateTime(),//停止时间
                                        dcReasonCode,//DC原因代码
                                        dcReasonName,//DC原因名称
                                        dcDoct.ID,//DC医师代码
                                        dcDoct.Name,//DC医师姓名
                                        confirmNurse.ID,//Dc人员代码
                                        confirmNurse.Name,//Dc人员名称
                                        this.GetDateTimeFromSysDateTime(),//确认时间
                                        confirmNurse.ID//确认人员代码
                                    );
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.DcOrder.AutoDcAllLong";
                return -1;
            }

            return this.ExecNoQuery(strSql);

            return 1;
        }

        /// <summary>
        /// 根据患者住院号、系统类别和停止原因，停止患者此大类的医嘱主档。如果没有大类则还要停止医嘱执行档
        /// 0 患者住院流水号
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="sysClassCode">系统类别  00时停止全部医嘱及执行档</param>
        /// <param name="dcDate">停止时间</param>
        /// <param name="dcReasonCode">停止原因代码</param>
        /// <param name="dcReasonName">停止原因名称</param>
        /// <returns></returns>
        public int DcOrder(string inpatientNo, string sysClassCode, DateTime dcDate, string dcReasonCode, string dcReasonName)
        {
            /*1、停止医嘱档状态为2、3
             *2、停止执行档为：当前停止时间之后的未收费的有效执行档
             * */
            string strSql = "";
            //停止医嘱主档
            if (this.Sql.GetCommonSql("Order.Order.DcOrder.All", ref strSql) == -1)
            {
                this.Err = "找不到SQL语句:Order.Order.DcOrder.All";
                return -1;
            }
            try
            {
                strSql = string.Format(strSql,
                                        inpatientNo,        //患者住院流水号
                                        sysClassCode,       //系统类别
                                        dcDate.ToString(),  //停止日期
                                        dcReasonCode,       //停止原因代码
                                        dcReasonName,       //停止原因名称
                                        this.Operator.ID,   //停止人
                                        this.Operator.Name  //停止人姓名
                                    );
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.DcOrder.All";
                return -1;
            }
            int parm = this.ExecNoQuery(strSql);
            if (parm == -1) return -1;

            //如果sysClassCode有效（不等于"00"），则只按大类停止医嘱，不处理执行档数据，否则停止执行档
            if (sysClassCode == "00")
            {
                //停止药品医嘱执行档
                if (this.Sql.GetCommonSql("Order.ExecOrder.DcExecAll.Drug", ref strSql) == -1)
                {
                    this.Err = "找不到SQL语句:Order.ExecOrder.DcExecAll.Drug";
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, inpatientNo, dcDate.ToString(), this.Operator.ID, dcReasonName);
                }
                catch
                {
                    this.Err = "传入参数不对！Order.ExecOrder.DcExecAll.Drug";
                    return -1;
                }

                parm = this.ExecNoQuery(strSql);
                if (parm == -1) return -1;

                //停止非药品医嘱执行档
                if (this.Sql.GetCommonSql("Order.ExecOrder.DcExecAll.Undrug", ref strSql) == -1)
                {
                    this.Err = "找不到SQL语句:Order.ExecOrder.DcExecAll.Undrug";
                    return -1;
                }
                try
                {
                    strSql = string.Format(strSql, inpatientNo, dcDate.ToString(), this.Operator.ID, dcReasonName);
                }
                catch
                {
                    this.Err = "传入参数不对！Order.ExecOrder.DcExecAll.Undrug";
                    return -1;
                }
                parm = this.ExecNoQuery(strSql);
                if (parm == -1)
                {
                    return -1;
                }
            }

            return parm;
        }

        /// <summary>
        /// /// 根据患者住院号和停止原因，停止患者所有医嘱主档和医嘱执行档
        /// </summary>
        /// <param name="inpatientNo">0 患者住院流水号</param>
        /// <param name="dcDate">停止时间</param>
        /// <param name="dcReasonCode">停止原因代码</param>
        /// <param name="dcReasonName">停止原因名称</param>
        /// <returns></returns>
        public int DcOrder(string inpatientNo, DateTime dcDate, string dcReasonCode, string dcReasonName)
        {
            return this.DcOrder(inpatientNo, "00", dcDate, dcReasonCode, dcReasonName);
        }

        /// <summary>
        /// 执行单条医嘱DC操作
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private int DcOneOrderDeal(Neusoft.HISFC.Models.Order.Inpatient.Order order)
        {
            //提取系统时间
            DateTime CurTime = this.GetDateTimeFromSysDateTime();

            //不管是否预停止，都只作废停止时间之后的执行档
            #region 作废执行档
            ////立即停止（临时医嘱 or 停止时间小于等于当前时间）
            //if (order.OrderType.IsDecompose == false || order.EndTime <= CurTime)
            //{
            //    //置作废标记
            //    order.Status = 3;
            //    if (this.DcExecImmediate(order, this.Operator) < 0)
            //    {
            //        this.Err = Neusoft.FrameWork.Management.Language.Msg("作废医嘱执行信息失败！");
            //        return -1;
            //    }
            //}
            ////预停止（长期医嘱并且停止时间大于等于当前时间）
            //else
            //{
            if (this.DcExecLater(order, this.Operator) < 0)
            {
                this.Err = Neusoft.FrameWork.Management.Language.Msg("作废医嘱执行信息失败！");
                return -1;
            }
            //}
            #endregion

            #region 停止医嘱
            //执行停止医嘱挡
            if (this.DcOneOrder(order) < 0)
            {
                this.Err = Neusoft.FrameWork.Management.Language.Msg("停止医嘱失败！");
                return -1;
            }
            #endregion
            return 0;
        }

        /// <summary>
        /// 按组合号查询医嘱
        /// 按照数据库的SQL语句来看，目前isSubtbl参数没用了
        /// </summary>
        /// <param name="combno">组合号</param>
        /// <param name="isSubtbl">目前含辅材</param>
        /// <returns></returns>
        public ArrayList QueryOrderByCombNO(string combno, bool isSubtbl)
        {
            #region 按状态查询医嘱
            //按组合号查询医嘱
            //Order.Order.QueryOrderByCombno.where.1
            //传入：0 combno 1 IsSub
            //传出：ArrayList
            #endregion
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null) return null;
            if (this.Sql.GetCommonSql("Order.Order.QueryOrderByCombno.where.1", ref sql1) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrderByCombno.where.1字段!";
                return null;
            }
            sql = sql + " " + string.Format(sql1, combno, Neusoft.FrameWork.Function.NConvert.ToInt32(isSubtbl).ToString());
            return this.MyOrderQuery(sql);
        }

        #endregion

        #region 取消停止医嘱

        /// <summary>
        /// 某条医嘱是否已停止审核
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public string GetDCConfirmFlag(string orderID)
        {
            string sql = "";
            try
            {
                if (this.Sql.GetCommonSql("NewOrder.Order.Query.DCConfirmFlag", ref sql) == -1)
                {
                    Err = "查找SQL出错：NewOrder.Order.Query.DCConfirmFlag";
                    return null;
                }
                sql = string.Format(sql, orderID);
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return null;
            }
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 取消停止医嘱
        /// </summary>
        /// <param name="order"></param>
        /// <param name="isAllDc">组合医嘱是否全部停止</param>
        /// <returns></returns>
        public int CancelDcOrder(Neusoft.HISFC.Models.Order.Inpatient.Order order, bool isAllDc)
        {
            order = this.QueryOneOrder(order.ID);

            if (!(order.Status == 3 || order.EndTime > DateTime.MinValue))
            {
                this.Err = "医嘱不是停止状态，请刷新医嘱！";
                return -1;
            }

            //是否是组合医嘱
            ArrayList al = new ArrayList();
            al = this.QueryOrderByCombNO(order.Combo.ID, false);

            //停止组合相同的附材
            ArrayList temp = this.QueryOrderByCombNO(order.Combo.ID, true);
            al.AddRange(temp);

            //是否停止组合项目中的单项
            if (isAllDc)
            {
                Neusoft.HISFC.Models.Order.Inpatient.Order obj = null;
                for (int i = 0; i < al.Count; i++)
                {
                    obj = (Neusoft.HISFC.Models.Order.Inpatient.Order)al[i];
                    //本记录在下面处理
                    if (obj.ID == order.ID)
                        continue;

                    //执行单条医嘱DC
                    if (CancelDcOneOrderDeal(obj) < 0)
                    {
                        return -1;
                    }
                }
            }

            //执行单条医嘱DC
            if (CancelDcOneOrderDeal(order) < 0)
            {
                return -1;
            }
            return 0;

        }

        /// <summary>
        /// 取消停止单条医嘱
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private int CancelDcOneOrderDeal(Neusoft.HISFC.Models.Order.Inpatient.Order order)
        {

            order.DcReason = new NeuObject("", "", "");
            order.DCOper.ID = string.Empty;
            order.DCOper.Name = string.Empty;
            order.Status = 2;

            //取消停止执行档
            if (this.CancelDcExecImmediate(order, order.DcReason) < 0)
            {
                return -1;
            }
            order.DCOper.OperTime = DateTime.MinValue;
            order.EndTime = DateTime.MinValue;

            //取消停止医嘱挡
            if (this.CancelDcOneOrder(order) < 0)
            {
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// 取消停止医嘱档
        /// Order.Status = 1预停止;Order.Status = 3直接停止
        /// </summary>
        /// <param name="Order">医嘱信息</param>
        /// <returns>0 success -1 fail</returns>
        public int CancelDcOneOrder(Neusoft.HISFC.Models.Order.Inpatient.Order Order)
        {
            #region 取消停止医嘱
            //停止医嘱(医嘱已生效状态)
            //Order.Order.dcOrder.1
            //传入：0 id，1 停止人id,2停止人姓名，3停止时间,4医嘱状态 ,5停止原因代码，6停止原因名称 
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.DCOrder.CanCel", ref strSql) == -1)
            {
                this.Err = "取消停止医嘱失败：" + this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, Order.ID, Order.DCOper.ID, Order.DCOper.Name, Order.EndTime.ToString(), Order.Status.ToString(), Order.DcReason.ID, Order.DcReason.Name);
            }
            catch
            {
                this.Err = "取消停止医嘱失败：" + "传入参数不对！Order.Order.DCOrder.CanCel";
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 取消停止执行档
        /// </summary>
        /// <param name="dcPerson">执行档信息</param>
        /// <returns>0 success -1 fail</returns>
        public int CancelDcExecImmediate(Neusoft.HISFC.Models.Order.Inpatient.Order Order, Neusoft.FrameWork.Models.NeuObject dcPerson)
        {
            #region 作废执行档
            //作废执行档(医嘱停止或直接作废)
            //Order.ExecOrder.DcExecImmediate
            //传入：0 id，1 停止人id,2停止人姓名，3停止时间,4作废标志 
            //传出：0 
            #endregion

            string strSql = "";
            string strSqlName = "";

            if (Order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
            {
                strSqlName = "Order.ExecOrder.DcExecImmediate.Cancel.Drug";
            }
            else
            {
                strSqlName = "Order.ExecOrder.DcExecImmediate.Cancel.unDrug";
            }

            if (this.Sql.GetCommonSql(strSqlName, ref strSql) == -1)
            {
                this.Err = "取消停止医嘱执行档失败：" + this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, Order.ID, dcPerson.ID, Order.DCOper.OperTime.ToString());
            }
            catch
            {
                this.Err = "取消停止医嘱执行档失败：" + "传入参数不对" + strSqlName;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        #endregion

        #region 配液信息

        /// <summary>
        /// 检索待配液信息
        /// </summary>
        /// <param name="isNurse">是否按病区检索 即deptCode传入为病区编码</param>
        /// <param name="deptCode">科室</param>
        /// <param name="dtBegin">开始执行时间</param>
        /// <param name="dtEnd">终止执行时间</param>
        /// <param name="isExec">状态 是否查询已配液的</param>
        /// <returns>成功返回待配液信息 失败返回null</returns>
        public ArrayList QueryExecOrderForCompound(bool isNurse, string deptCode, DateTime dtBegin, DateTime dtEnd, bool isExec)
        {
            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect("1");
            sql = s[0];
            if (sql == null)
            {
                return null;
            }

            if (isNurse)
            {
                if (this.Sql.GetCommonSql("Order.QueryOrderExecCompound.NurseCell", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.QueryOrderExecCompound.NurseCell字段!";
                    return null;
                }
            }
            else
            {
                if (this.Sql.GetCommonSql("Order.QueryOrderExecCompound.DeptCode", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.QueryOrderExecCompound.DeptCode字段!";
                    return null;
                }
            }

            string state = "0";
            if (isExec)
            {
                state = "1";
            }
            else
            {
                state = "0";
            }

            sql = sql + " " + string.Format(sql1, deptCode, dtBegin.ToString(), dtEnd.ToString(), state);

            return this.myExecOrderQuery(sql);
        }

        /// <summary>
        /// 配液信息更新
        /// </summary>
        /// <param name="execOrderID">执行挡流水号</param>
        /// <param name="compound">配液信息</param>
        /// <returns>成功返回1 失败返回-1 无记录更新返回0</returns>
        public int UpdateOrderCompound(string execOrderID, Neusoft.HISFC.Models.Order.Compound compound)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateOrderCompound", ref strSql) == -1)
            {
                this.Err = "没有找到Order.ExecOrder.UpdateOrderCompound字段";
                return -1;
            }
            strSql = string.Format(strSql, execOrderID,
                                           Neusoft.FrameWork.Function.NConvert.ToInt32(compound.IsExec).ToString(),
                                           compound.CompoundOper.ID, compound.CompoundOper.Dept.ID,
                                           compound.CompoundOper.OperTime.ToString());

            return this.ExecNoQuery(strSql);
        }

        #endregion

        #region 互斥
        /// <summary>
        /// 获得互斥
        /// </summary>
        /// <param name="sysClassID"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.EnumMutex QueryMutex(string sysClassID)
        {
            string sql = "";
            Neusoft.HISFC.Models.Order.EnumMutex mutex = Neusoft.HISFC.Models.Order.EnumMutex.None;
            if (this.Sql.GetCommonSql("Order.QueryMutex.1", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return mutex;
            }
            sql = string.Format(sql, sysClassID);
            string strMutex = "";
            strMutex = this.ExecSqlReturnOne(sql);
            try
            {
                mutex = (Neusoft.HISFC.Models.Order.EnumMutex)Neusoft.FrameWork.Function.NConvert.ToInt32(strMutex);
                return mutex;
            }
            catch
            {
                return mutex;
            }
        }
        #endregion

        #region 私有函数

        /// <summary>
        /// 获得项目集合
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private ArrayList myGetOutHosCure(string strSql)
        {
            ArrayList al = new ArrayList();
            if (this.ExecQuery(strSql) == -1) return null;
            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList item = new Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList();
                try
                {
                    item.Name = this.Reader[0].ToString();//项目名称
                    item.Item.Price = System.Convert.ToDecimal(this.Reader[1].ToString());//价格
                    item.Order.Qty = System.Convert.ToDecimal(this.Reader[2].ToString());//总量
                    item.Order.Unit = this.Reader[3].ToString();//单位
                    item.Order.ExeDept.ID = this.Reader[4].ToString();//执行科室
                    item.User01 = this.Reader[5].ToString();//执行序号
                }
                catch (Exception ex)
                {
                    this.Err = "获得检查单信息出错！" + ex.Message;
                    this.WriteErr();
                    return null;
                }
                al.Add(item);
            }
            this.Reader.Close();
            return al;
        }

        /// <summary>
        /// 获得医嘱信息
        /// </summary>
        /// <param name="sqlOrder"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        private string GetOrderInfo(string sqlOrder, Neusoft.HISFC.Models.Order.Inpatient.Order Order)
        {
            #region "接口说明"
            //0 ID医嘱流水号
            //患者信息——  
            //			1 住院流水号   2住院病历号     3患者科室id      4患者护理id
            //医嘱辅助信息
            // ——项目信息
            //	       5项目类别      6项目编码       7项目名称      8项目输入码,    9项目拼音码 
            //	       10项目类别代码 11项目类别名称  12药品规格     13药品基本剂量  14剂量单位       
            //         15最小单位     16包装数量,     17剂型代码     18药品类别  ,   19药品性质
            //         20零售价       21用法代码      22用法名称     23用法英文缩写  24频次代码  
            //         25频次名称     26每次剂量      27项目总量     28计价单位      29使用天数			  
            // ——医嘱属性
            //		   30医嘱类别代码 31医嘱类别名称  32医嘱是否分解:1长期/2临时     33是否计费 
            //		   34药房是否配药 35打印执行单    36是否需要确认  
            // ——执行情况
            //		   37开立医师Id   38开立医师name  39开始时间      40结束时间     41开立科室
            //		   42开立时间     43录入人员代码  44录入人员姓名  45审核人ID     46审核时间       
            //		   47DC原因代码   48DC原因名称    49DC医师代码    50DC医师姓名   51Dc时间
            //         52执行人员代码 53执行时间      54执行科室代码  55执行科室名称 
            //		   56本次分解时间 57下次分解时间
            // ——医嘱类型
            //		   58是否婴儿（1是/2否）          59发生序号  	  60组合序号     61主药标记 
            //		   62是否附材'1'  63是否包含附材  64医嘱状态      65扣库标记     66执行标记1未执行/2已执行/3DC执行 
            //		   67医嘱说明                     68加急标记:1普通/2加急         69排列序号
            //         70检查部位备注                 71批注          72整档,          73 样本类型名称,
            //         74 取药药房编码 
            #endregion
            string strItemType = "";
            if (Order.CurMOTime == DateTime.MinValue)
            {
                Order.CurMOTime = Order.BeginTime;
            }
            if (Order.NextMOTime == DateTime.MinValue)
            {
                Order.NextMOTime = Order.BeginTime;
            }
            //判断药品/非药品

            if (Order.Item.GetType() == typeof(Neusoft.HISFC.Models.Pharmacy.Item))
            {
                Neusoft.HISFC.Models.Pharmacy.Item objPharmacy;
                objPharmacy = (Neusoft.HISFC.Models.Pharmacy.Item)Order.Item;
                strItemType = "1";
                try
                {
                    System.Object[] s = { Order.ID,
                                          Order.Patient.ID,
                                          Order.Patient.PID.PatientNO,
                                          Order.Patient.PVisit.PatientLocation.Dept.ID,
                                          Order.Patient.PVisit.PatientLocation.NurseCell.ID,
										  strItemType,
                                          Order.Item.ID,
                                          Order.Item.Name,
                                          Order.Item.UserCode,
                                          Order.Item.SpellCode,
										  Order.Item.SysClass.ID.ToString(),
                                          Order.Item.SysClass.Name,
                                          objPharmacy.Specs,
                                          objPharmacy.BaseDose,
                                          objPharmacy.DoseUnit,
                                          objPharmacy.MinUnit,
                                          objPharmacy.PackQty,
										  objPharmacy.DosageForm.ID,
                                          objPharmacy.Type.ID,
                                          objPharmacy.Quality.ID,
                                          //objPharmacy.Price,
                                          Order.Item.Price,
										  Order.Usage.ID,
                                          Order.Usage.Name,
                                          Order.Usage.Memo,
                                          Order.Frequency.ID,
                                          Order.Frequency.Name,
										  Order.DoseOnce,
                                          Order.Qty,Order.Unit,
                                          Order.HerbalQty.ToString(),
										  Order.OrderType.ID,
                                          Order.OrderType.Name,
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsDecompose),
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsCharge),
										  Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsNeedPharmacy),
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsPrint),
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.Item.IsNeedConfirm),
										  Order.ReciptDoctor.ID,
                                          Order.ReciptDoctor.Name,
                                          Order.BeginTime,
                                          Order.EndTime,
                                          Order.ReciptDept.ID,
										  Order.MOTime,
                                          Order.Oper.ID,
                                          Order.Oper.Name,
                                          Order.Nurse.ID,
                                          Order.ConfirmTime,
										  Order.DcReason.ID,
                                          Order.DcReason.Name,
                                          Order.DCOper.ID,
                                          Order.DCOper.Name,
                                          Order.DCOper.OperTime,
										  Order.ExecOper.ID,
                                          Order.ExecOper.OperTime,
                                          Order.ExeDept.ID,
                                          Order.ExeDept.Name,
										  Order.CurMOTime,
                                          Order.NextMOTime,
										  Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsBaby),
                                          Order.BabyNO,
                                          Order.Combo.ID,
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.Combo.IsMainDrug),
										  Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsSubtbl),
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsHaveSubtbl),
                                          Order.Status,
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsStock),
                                          Order.ExecStatus,
										  Order.Note,
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsEmergency),
                                          Order.SortID,
                                          Order.Memo,
                                          Order.CheckPartRecord,
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.Reorder),
                                          Order.Sample.Name,
                                          Order.StockDept.ID,
                                          //objPharmacy.IsAllergy==true ?"2":"1",
                                          ((Int32)Order.HypoTest).ToString(),
                                          Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsPermission),
                                          Order.Package.ID,
                                          Order.Package.Name,
                                          Order.ExtendFlag1,
                                          Order.ExtendFlag2,
                                          Order.ReTidyInfo,
                                          Order.Frequency.Time,
                                          Order.ExecDose,
                                          Order.ApplyNo,
                                          Order.DoseOnceDisplay,//{0D81AD2A-4F10-4fe5-9F79-5C6393384318}
                                          Order.DoseUnitDisplay,//{0D81AD2A-4F10-4fe5-9F79-5C6393384318}
                                          Order.FirstUseNum,//{0D81AD2A-4F10-4fe5-9F79-5C6393384318}
                                          Order.Patient.PVisit.PatientLocation.Bed.ID,
                                          Order.PageNo.ToString(),
                                          Order.RowNo.ToString(),
                                          Order.SpeOrderType,
                                          Order.SubCombNO.ToString(), //组号 2011-7-3 houwb
                                          Order.GetFlag, //医嘱打印标记
                                          Order.ExtendFlag,
                                          Order.ExtendFlag0,
                                          Order.ExtendFlag7,  //手术
                                          Order.ExtendFlag8,  //切口
                                          Order.ExtendFlag9   //抗生素特注
                                         };//新加特殊频次

                    string myErr = "";
                    if (Neusoft.FrameWork.Public.String.CheckObject(out myErr, s) == -1)
                    {
                        this.Err = myErr;
                        this.WriteErr();
                        return null;
                    }
                    sqlOrder = string.Format(sqlOrder, s);
                }
                catch (Exception ex)
                {
                    this.Err = "付数值时候出错！" + ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
            else if (Order.Item.GetType() == typeof(Neusoft.HISFC.Models.Fee.Item.Undrug))
            {
                Neusoft.HISFC.Models.Fee.Item.Undrug objAssets;
                objAssets = (Neusoft.HISFC.Models.Fee.Item.Undrug)Order.Item;
                strItemType = "2";

                try
                {
                    string[] s = { Order.ID,
                                   Order.Patient.ID,
                                   Order.Patient.PID.PatientNO,
                                   Order.Patient.PVisit.PatientLocation.Dept.ID,
                                   Order.Patient.PVisit.PatientLocation.NurseCell.ID,
								   strItemType,
                                   Order.Item.ID,
                                   Order.Item.Name,
                                   Order.Item.UserCode,
                                   Order.Item.SpellCode,
								   Order.Item.SysClass.ID.ToString(),
                                   Order.Item.SysClass.Name,
                                   objAssets.Specs,
                                   "0",
                                   "",
                                   "",
                                   "0",
                                   "",
                                   "",
                                   "",
                                   objAssets.Price.ToString(),
								   Order.Usage.ID,
                                   Order.Usage.Name,
                                   Order.Usage.Memo,
                                   Order.Frequency.ID,
                                   Order.Frequency.Name,
								   Order.DoseOnce.ToString(),
                                   Order.Qty.ToString(),
                                   Order.Unit,
                                   Order.HerbalQty.ToString(),
								   Order.OrderType.ID,
                                   Order.OrderType.Name,
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsDecompose).ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsCharge).ToString(),
								   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsNeedPharmacy).ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsPrint).ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.Item.IsNeedConfirm).ToString(),
								   Order.ReciptDoctor.ID,
                                   Order.ReciptDoctor.Name,
                                   Order.BeginTime.ToString(),
                                   Order.EndTime.ToString(),
                                   Order.ReciptDept.ID,
								   Order.MOTime.ToString(),
                                   Order.Oper.ID,
                                   Order.Oper.Name,
                                   Order.Nurse.ID,
                                   Order.ConfirmTime.ToString(),
								   Order.DcReason.ID,
                                   Order.DcReason.Name,
                                   Order.DCOper.ID,
                                   Order.DCOper.Name,
                                   Order.DCOper.OperTime.ToString(),
								   Order.ExecOper.ID,
                                   Order.ExecOper.OperTime.ToString(),
                                   Order.ExeDept.ID,
                                   Order.ExeDept.Name,
								   Order.CurMOTime.ToString(),
                                   Order.NextMOTime.ToString(),
								   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsBaby).ToString(),
                                   Order.BabyNO.ToString(),
                                   Order.Combo.ID,
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.Combo.IsMainDrug).ToString(),
								   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsSubtbl).ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsHaveSubtbl).ToString(),
                                   Order.Status.ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsStock).ToString(),
                                   Order.ExecStatus.ToString(),
								   Order.Note,
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsEmergency).ToString(),
                                   Order.SortID.ToString(),
                                   Order.Memo,
                                   Order.CheckPartRecord,
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.Reorder).ToString(),
                                   Order.Sample.Name,
                                   Order.StockDept.ID,
								   "",
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsPermission).ToString(),
                                   Order.Package.ID,
                                   Order.Package.Name,
                                   Order.ExtendFlag1,
                                   Order.ExtendFlag2,
                                   Order.ReTidyInfo,
                                   Order.Frequency.Time,
                                   Order.ExecDose,
                                   Order.ApplyNo,
                                   Order.DoseOnceDisplay,//{0D81AD2A-4F10-4fe5-9F79-5C6393384318}
                                   Order.DoseUnitDisplay,//{0D81AD2A-4F10-4fe5-9F79-5C6393384318}
                                   Order.FirstUseNum,//{0D81AD2A-4F10-4fe5-9F79-5C6393384318}
                                   Order.Patient.PVisit.PatientLocation.Bed.ID,
                                   Order.PageNo.ToString(),
                                   Order.RowNo.ToString(),
                                   Order.SpeOrderType,
                                          Order.SubCombNO.ToString(), //组号 2011-7-3 houwb
                                          Order.GetFlag, //医嘱打印标记
                                          Order.ExtendFlag,
                                          Order.ExtendFlag0,
                                          Order.ExtendFlag7,  //手术
                                          Order.ExtendFlag8,  //切口
                                          Order.ExtendFlag9   //抗生素特注
                                 };//新加特殊频次
                    sqlOrder = string.Format(sqlOrder, s);
                }
                catch (Exception ex)
                {
                    this.Err = "付数值时候出错！" + ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
            //{8F86BB0D-9BB4-4c63-965D-969F1FD6D6B2} 医嘱附材绑定物资 by gengxl //{BE3C743B-7343-4e12-A1AF-4B2793C8F9BB}
            else if (Order.Item.GetType() == typeof(Neusoft.HISFC.Models.FeeStuff.MaterialItem))
            {
                //{BE3C743B-7343-4e12-A1AF-4B2793C8F9BB}
                Neusoft.HISFC.Models.FeeStuff.MaterialItem objAssets;
                //{BE3C743B-7343-4e12-A1AF-4B2793C8F9BB}
                objAssets = (Neusoft.HISFC.Models.FeeStuff.MaterialItem)Order.Item;
                strItemType = "2";

                try
                {
                    string[] s = { Order.ID,
                                   Order.Patient.ID,
                                   Order.Patient.PID.PatientNO,
                                   Order.Patient.PVisit.PatientLocation.Dept.ID,
                                   Order.Patient.PVisit.PatientLocation.NurseCell.ID,
								   strItemType,
                                   Order.Item.ID,
                                   Order.Item.Name,
                                   Order.Item.UserCode,
                                   Order.Item.SpellCode,
								   Order.Item.SysClass.ID.ToString(),
                                   Order.Item.SysClass.Name,
                                   objAssets.Specs,
                                   "0",
                                   "",
                                   "",
                                   "0",
                                   "",
                                   "",
                                   "",
                                   objAssets.Price.ToString(),
								   Order.Usage.ID,
                                   Order.Usage.Name,
                                   Order.Usage.Memo,
                                   Order.Frequency.ID,
                                   Order.Frequency.Name,
								   Order.DoseOnce.ToString(),
                                   Order.Qty.ToString(),
                                   Order.Unit,
                                   Order.HerbalQty.ToString(),
								   Order.OrderType.ID,
                                   Order.OrderType.Name,
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsDecompose).ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsCharge).ToString(),
								   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsNeedPharmacy).ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.OrderType.IsPrint).ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.Item.IsNeedConfirm).ToString(),
								   Order.ReciptDoctor.ID,
                                   Order.ReciptDoctor.Name,
                                   Order.BeginTime.ToString(),
                                   Order.EndTime.ToString(),
                                   Order.ReciptDept.ID,
								   Order.MOTime.ToString(),
                                   Order.Oper.ID,
                                   Order.Oper.Name,
                                   Order.Nurse.ID,
                                   Order.ConfirmTime.ToString(),
								   Order.DcReason.ID,
                                   Order.DcReason.Name,
                                   Order.DCOper.ID,
                                   Order.DCOper.Name,
                                   Order.DCOper.OperTime.ToString(),
								   Order.ExecOper.ID,
                                   Order.ExecOper.OperTime.ToString(),
                                   Order.ExeDept.ID,
                                   Order.ExeDept.Name,
								   Order.CurMOTime.ToString(),
                                   Order.NextMOTime.ToString(),
								   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsBaby).ToString(),
                                   Order.BabyNO.ToString(),
                                   Order.Combo.ID,
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.Combo.IsMainDrug).ToString(),
								   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsSubtbl).ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsHaveSubtbl).ToString(),
                                   Order.Status.ToString(),
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsStock).ToString(),
                                   Order.ExecStatus.ToString(),
								   Order.Note,
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsEmergency).ToString(),
                                   Order.SortID.ToString(),
                                   Order.Memo,
                                   Order.CheckPartRecord,
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.Reorder).ToString(),
                                   Order.Sample.Name,
                                   Order.StockDept.ID,
								   "",
                                   Neusoft.FrameWork.Function.NConvert.ToInt32(Order.IsPermission).ToString(),
                                   Order.Package.ID,
                                   Order.Package.Name,
                                   Order.ExtendFlag1,
                                   Order.ExtendFlag2,
                                   Order.ReTidyInfo,
                                   Order.Frequency.Time,
                                   Order.ExecDose,
                                   Order.ApplyNo,
                                   Order.DoseOnceDisplay,//{0D81AD2A-4F10-4fe5-9F79-5C6393384318}
                                   Order.DoseUnitDisplay,//{0D81AD2A-4F10-4fe5-9F79-5C6393384318}
                                   Order.FirstUseNum,//{0D81AD2A-4F10-4fe5-9F79-5C6393384318}
                                   Order.Patient.PVisit.PatientLocation.Bed.ID,
                                   Order.PageNo.ToString(),
                                   Order.RowNo.ToString(),
                                   Order.SpeOrderType,
                                          Order.SubCombNO.ToString(), //组号 2011-7-3 houwb
                                          Order.GetFlag, //医嘱打印标记
                                          Order.ExtendFlag,
                                          Order.ExtendFlag0,
                                          Order.ExtendFlag7,  //手术
                                          Order.ExtendFlag8,  //切口
                                          Order.ExtendFlag9   //抗生素特注
                                  };//新加特殊频次
                    sqlOrder = string.Format(sqlOrder, s);
                }
                catch (Exception ex)
                {
                    this.Err = "付数值时候出错！" + ex.Message;
                    this.WriteErr();
                    return null;
                }
            }
            else
            {
                this.Err = "项目类型出错！";
                return null;
            }
            return sqlOrder;
        }

        // {9510034F-0721-4966-95D0-36C8BD392A93}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public bool OrderIsExists(string inpatientNo, string itemCode)
        {
            try
            {
                string sql = @"Select count(*)
  FROM MET_IPM_ORDER a
 where a.Inpatient_No = '{0}'
   and a.item_code = '{1}'
   and a.Mo_Stat in ('1','2')
   and a.TYPE_CODE ='CZ'";

                sql = string.Format(sql, inpatientNo, itemCode);

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

        /// 查询患者信息的select语句（无where条件）
        private string OrderQuerySelect()
        {
            #region 接口说明
            //Order.Order.QueryOrder.Select.1
            //传入：0
            //传出：sql.select
            #endregion
            string sql = "";
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.Select.1", ref sql) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.Select.1字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            return sql;
        }


        // {62002B3A-B84B-4d14-82D2-B4EF7F92A510}    组套项目显示明细
        /// 查询患者信息的select语句
        private string OrderQuerySelect2()
        {
            #region 接口说明
            //Order.Order.QueryOrder.Select.2
            //传入：0
            //传出：sql.select
            #endregion
            string sql = "";
            if (this.Sql.GetCommonSql("Order.Order.QueryOrder.Select.2", ref sql) == -1)
            {
                this.Err = "没有找到Order.Order.QueryOrder.Select.2字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            return sql;
        }

        /// <summary>
        /// 根据WhereSQLID查询医嘱
        /// </summary>
        /// <param name="whereSQLIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ArrayList QueryOrderBase(string whereSQLIndex, params object[] args)
        {
            string whereSQL = "";
            if (this.Sql.GetCommonSql(whereSQLIndex, ref whereSQL) == -1)
            {
                this.Err = "没有找到SQL语句，ID为" + whereSQLIndex + "!";
                return null;
            }

            whereSQL = string.Format(whereSQL, args);

            return this.QueryOrderBase(whereSQL);
        }

        /// <summary>
        /// 根据Where条件查询医嘱
        /// </summary>
        /// <param name="whereSQL"></param>
        /// <returns></returns>
        public ArrayList QueryOrderBase(string whereSQL)
        {
            string sql = OrderQuerySelect();
            if (sql == null)
            {
                return null;
            }

            sql = sql + "\r\n" + whereSQL;

            return this.MyOrderQuery(sql);
        }

        //private ArrayList PatientOpera(string sql)
        //{
        //    ArrayList al = new ArrayList();
        //    if (this.ExecQuery(sql) == -1)
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        while(this.Reader.Read())
        //    }
        //}

        /// <summary>
        /// 私有函数，查询医嘱信息
        /// </summary>
        /// <param name="SQLPatient"></param>
        /// <returns></returns>
        private ArrayList MyOrderQuery(string SQLPatient)
        {
            ArrayList al = new ArrayList();

            if (this.ExecQuery(SQLPatient) == -1)
            {
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Order.Inpatient.Order objOrder = new Neusoft.HISFC.Models.Order.Inpatient.Order();
                    #region "患者信息"
                    //患者信息——  
                    //			1 住院流水号   2住院病历号     3患者科室id      4患者护理id
                    try
                    {
                        objOrder.Patient.ID = this.Reader[1].ToString();
                        objOrder.Patient.PID.PatientNO = this.Reader[2].ToString();
                        objOrder.Patient.PVisit.PatientLocation.Dept.ID = this.Reader[3].ToString();
                        objOrder.InDept.ID = this.Reader[3].ToString();
                        objOrder.Patient.PVisit.PatientLocation.NurseCell.ID = this.Reader[4].ToString();

                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得患者基本信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    #region "项目信息"
                    //医嘱辅助信息
                    // ——项目信息
                    //	       5项目类别      6项目编码       7项目名称      8项目输入码,    9项目拼音码 
                    //	       10项目类别代码 11项目类别名称  12药品规格     13药品基本剂量  14剂量单位       
                    //         15最小单位     16包装数量,     17剂型代码     18药品类别  ,   19药品性质
                    //         20零售价       21用法代码      22用法名称     23用法英文缩写  24频次代码  
                    //         25频次名称     26每次剂量      27项目总量     28计价单位      29使用天数			  
                    // 判断药品/非药品
                    //         25频次名称     26每次剂量      27项目总量     28计价单位      29使用天数			  
                    // 73 样本类型 名称
                    if (this.Reader[5].ToString() == "1")//药品
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item objPharmacy = new Neusoft.HISFC.Models.Pharmacy.Item();
                        try
                        {
                            objPharmacy.ID = this.Reader[6].ToString();
                            objPharmacy.Name = this.Reader[7].ToString();
                            objPharmacy.UserCode = this.Reader[8].ToString();
                            objPharmacy.SpellCode = this.Reader[9].ToString();
                            objPharmacy.SysClass.ID = this.Reader[10].ToString();

                            objPharmacy.Specs = this.Reader[12].ToString();

                            if (!this.Reader.IsDBNull(13))
                                objPharmacy.BaseDose = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());

                            objPharmacy.DoseUnit = this.Reader[14].ToString();
                            objOrder.DoseUnit = this.Reader[14].ToString();
                            objPharmacy.MinUnit = this.Reader[15].ToString();

                            if (!this.Reader.IsDBNull(16))
                                objPharmacy.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[16].ToString());

                            objPharmacy.DosageForm.ID = this.Reader[17].ToString();
                            objPharmacy.Type.ID = this.Reader[18].ToString();
                            objPharmacy.Quality.ID = this.Reader[19].ToString();
                            // 计价单位
                            objPharmacy.PriceUnit = this.Reader[28].ToString();

                            //try
                            //{
                            if (!this.Reader.IsDBNull(20)) objPharmacy.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20].ToString());
                            //}
                            //catch{}					
                            objOrder.Usage.ID = this.Reader[21].ToString();
                            objOrder.Usage.Name = this.Reader[22].ToString();
                            objOrder.Usage.Memo = this.Reader[23].ToString();


                            if (!this.Reader.IsDBNull(87))
                            {
                                objOrder.DoseOnceDisplay = this.Reader[87].ToString();
                            }
                            if (!this.Reader.IsDBNull(88))
                            {
                                objOrder.DoseUnitDisplay = this.Reader[88].ToString();
                            }


                        }
                        catch (Exception ex)
                        {
                            this.Err = "获得医嘱项目信息出错！" + ex.Message;
                            this.WriteErr();
                            return null;
                        }
                        objOrder.Item = objPharmacy;
                    }
                    else if (this.Reader[5].ToString() == "2")//非药品
                    {
                        Neusoft.HISFC.Models.Fee.Item.Undrug objAssets = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        try
                        {
                            objAssets.ID = this.Reader[6].ToString();
                            objAssets.Name = this.Reader[7].ToString();
                            objAssets.UserCode = this.Reader[8].ToString();
                            objAssets.SpellCode = this.Reader[9].ToString();
                            objAssets.SysClass.ID = this.Reader[10].ToString();
                            //objAssets.SysClass.Name = this.Reader[11].ToString();
                            objAssets.Specs = this.Reader[12].ToString();
                            objOrder.Usage.ID = this.Reader[21].ToString();
                            objOrder.Usage.Name = this.Reader[22].ToString();
                            objOrder.Usage.Memo = this.Reader[23].ToString();
                            //							try
                            //							{
                            if (!this.Reader.IsDBNull(20)) objAssets.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20].ToString());
                            //}
                            //							catch{}	
                            objAssets.PriceUnit = this.Reader[28].ToString();
                            //样本类型名称
                            objOrder.Sample.Name = this.Reader[73].ToString();
                        }
                        catch (Exception ex)
                        {
                            this.Err = "获得医嘱项目信息出错！" + ex.Message;
                            this.WriteErr();
                            return null;
                        }
                        objOrder.Item = objAssets;
                    }


                    objOrder.Frequency.ID = this.Reader[24].ToString();
                    objOrder.Frequency.Name = this.Reader[25].ToString();
                    //try
                    //{
                    if (!this.Reader.IsDBNull(26)) objOrder.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26].ToString());//}
                    //catch{}
                    //try
                    //{
                    if (!this.Reader.IsDBNull(27)) objOrder.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27].ToString());//}
                    //catch{}
                    objOrder.Unit = this.Reader[28].ToString();
                    //try
                    //{
                    if (!this.Reader.IsDBNull(29)) objOrder.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[29].ToString());//}
                    //catch{}

                    #endregion

                    #region "医嘱属性"
                    // ——医嘱属性
                    //		   30医嘱类别代码 31医嘱类别名称  32医嘱是否分解:1长期/2临时     33是否计费 
                    //		   34药房是否配药 35打印执行单    36是否需要确认   
                    try
                    {
                        objOrder.ID = this.Reader[0].ToString();
                        objOrder.ExtendFlag1 = this.Reader[78].ToString();//临时医嘱执行时间－－自定义
                        objOrder.OrderType.ID = this.Reader[30].ToString();
                        objOrder.OrderType.Name = this.Reader[31].ToString();
                        //try
                        //{
                        if (!this.Reader.IsDBNull(32)) objOrder.OrderType.IsDecompose = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[32].ToString()));//}
                        //catch{}
                        //try
                        //{
                        if (!this.Reader.IsDBNull(33)) objOrder.OrderType.IsCharge = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[33].ToString()));//}
                        //catch{}
                        //try
                        //{
                        if (!this.Reader.IsDBNull(34)) objOrder.OrderType.IsNeedPharmacy = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[34].ToString()));//}
                        //catch{}
                        //try
                        //{
                        if (!this.Reader.IsDBNull(35)) objOrder.OrderType.IsPrint = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[35].ToString()));//}
                        //catch{}
                        //try
                        //{
                        if (!this.Reader.IsDBNull(36)) objOrder.Item.IsNeedConfirm = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[36].ToString()));//}
                        //catch{}
                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得医嘱属性信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    #region "执行情况"
                    // ——执行情况
                    //		   37开立医师Id   38开立医师name  39开始时间      40结束时间     41开立科室
                    //		   42开立时间     43录入人员代码  44录入人员姓名  45审核人ID     46审核时间       
                    //		   47DC原因代码   48DC原因名称    49DC医师代码    50DC医师姓名   51Dc时间
                    //         52执行人员代码 53执行时间      54执行科室代码  55执行科室名称 
                    //		   56本次分解时间 57下次分解时间
                    try
                    {
                        objOrder.ReciptDoctor.ID = this.Reader[37].ToString();
                        objOrder.ReciptDoctor.Name = this.Reader[38].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(39)) objOrder.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[39].ToString());
                        //}
                        //catch{}
                        //try{
                        if (!this.Reader.IsDBNull(40)) objOrder.EndTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[40].ToString());
                        //}
                        //catch{}
                        objOrder.ReciptDept.ID = this.Reader[41].ToString();//InDept.ID
                        //try{
                        if (!this.Reader.IsDBNull(42)) objOrder.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[42].ToString());
                        //}
                        //catch{}
                        objOrder.Oper.ID = this.Reader[43].ToString();
                        objOrder.Oper.Name = this.Reader[44].ToString();
                        objOrder.Nurse.ID = this.Reader[45].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(46)) objOrder.ConfirmTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[46].ToString());
                        //}
                        //catch{}
                        objOrder.DcReason.ID = this.Reader[47].ToString();
                        objOrder.DcReason.Name = this.Reader[48].ToString();
                        objOrder.DCOper.ID = this.Reader[49].ToString();
                        objOrder.DCOper.Name = this.Reader[50].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(51))
                            objOrder.DCOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[51].ToString());
                        //}
                        //catch{}
                        objOrder.ExecOper.ID = this.Reader[52].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(53)) objOrder.ExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[53].ToString());

                        objOrder.ExeDept.ID = this.Reader[54].ToString();
                        objOrder.ExeDept.Name = this.Reader[55].ToString();

                        objOrder.ExecOper.Dept.ID = this.Reader[54].ToString();
                        objOrder.ExecOper.Dept.Name = this.Reader[55].ToString();

                        if (!this.Reader.IsDBNull(56)) objOrder.CurMOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[56].ToString());

                        if (!this.Reader.IsDBNull(57)) objOrder.NextMOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[57].ToString());

                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得医嘱执行情况信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    #region "医嘱类型"
                    // ——医嘱类型
                    //		   58是否婴儿（1是/2否）          59发生序号  	  60组合序号     61主药标记 
                    //		   62是否附材'1'  63是否包含附材  64医嘱状态      65扣库标记     66执行标记1未执行/2已执行/3DC执行 
                    //		   67医嘱说明                     68加急标记:1普通/2加急         69排列序号
                    //         70 批注       ,       71检查部位备注    ,72 整档标记,74 取药药房编码 81 是否皮试 
                    //         84 申请单号
                    try
                    {

                        if (!this.Reader.IsDBNull(58)) objOrder.IsBaby = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[58].ToString()));

                        if (!this.Reader.IsDBNull(59)) objOrder.BabyNO = this.Reader[59].ToString();

                        objOrder.Combo.ID = this.Reader[60].ToString();

                        if (!this.Reader.IsDBNull(61)) objOrder.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[61].ToString()));

                        if (!this.Reader.IsDBNull(62)) objOrder.IsSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[62].ToString()));

                        if (!this.Reader.IsDBNull(63)) objOrder.IsHaveSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[63].ToString()));

                        if (!this.Reader.IsDBNull(64)) objOrder.Status = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[64].ToString());

                        if (!this.Reader.IsDBNull(65)) objOrder.IsStock = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[65].ToString()));


                        if (!this.Reader.IsDBNull(66)) objOrder.ExecStatus = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[66].ToString());

                        objOrder.Note = this.Reader[67].ToString();

                        if (!this.Reader.IsDBNull(68)) objOrder.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[68].ToString()));

                        if (!this.Reader.IsDBNull(69)) objOrder.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[69]);

                        objOrder.Memo = this.Reader[70].ToString();
                        objOrder.CheckPartRecord = this.Reader[71].ToString();
                        objOrder.Reorder = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[72].ToString());
                        objOrder.StockDept.ID = this.Reader[74].ToString();//取药药房编码
                        try
                        {
                            if (!this.Reader.IsDBNull(75)) objOrder.IsPermission = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[75]);//患者用药知情
                        }
                        catch { }
                        objOrder.Package.ID = this.Reader[76].ToString();
                        objOrder.Package.Name = this.Reader[77].ToString();
                        objOrder.ExtendFlag2 = this.Reader[79].ToString();
                        objOrder.ReTidyInfo = this.Reader[80].ToString();
                        try
                        {
                            if (!this.Reader.IsDBNull(81))
                            {
                                objOrder.HypoTest = (Neusoft.HISFC.Models.Order.EnumHypoTest)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[81].ToString());
                            }
                        }
                        catch
                        {
                            objOrder.HypoTest = 0;
                        }

                        objOrder.Frequency.Time = this.Reader[82].ToString(); //执行时间
                        objOrder.ExecDose = this.Reader[83].ToString();//特殊频次剂量
                        if (!this.Reader.IsDBNull(84)) objOrder.ApplyNo = this.Reader[84].ToString(); //申请单号


                        if (!this.Reader.IsDBNull(85))
                        {
                            objOrder.DCNurse.ID = this.Reader[85].ToString();
                        }
                        if (!this.Reader.IsDBNull(86))
                        {
                            objOrder.DCNurse.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[86].ToString());
                        }


                        if (!this.Reader.IsDBNull(89))
                        {
                            objOrder.FirstUseNum = this.Reader[89].ToString();
                        }

                        if (objOrder.FirstUseNum.Length <= 0)
                            objOrder.FirstUseNum = "0";

                        if (!this.Reader.IsDBNull(90))
                        {
                            objOrder.Patient.PVisit.PatientLocation.Bed.ID = this.Reader[90].ToString();
                        }
                        if (!this.Reader.IsDBNull(91))
                        {
                            objOrder.PageNo = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[91].ToString());
                        }
                        if (!this.Reader.IsDBNull(92))
                        {
                            objOrder.RowNo = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[92].ToString());
                        }

                        if (this.Reader[5].ToString() == "1")
                        {
                            if (objOrder.DoseOnceDisplay.Length <= 0)
                                objOrder.DoseOnceDisplay = objOrder.DoseOnce.ToString();

                            if (objOrder.DoseUnitDisplay.Length <= 0)
                                objOrder.DoseUnitDisplay = (objOrder.Item as Neusoft.HISFC.Models.Pharmacy.Item).DoseUnit.ToString();
                        }
                        // {C903923D-C0F7-4665-83C4-6CB8CC529155}
                        if (this.Reader.FieldCount >= 94)
                        {
                            objOrder.GetFlag = this.Reader[93].ToString();
                        }

                        // {C903923D-C0F7-4665-83C4-6CB8CC529155}
                        if (this.Reader.FieldCount >= 95)
                        {
                            objOrder.SpeOrderType = this.Reader[94].ToString();
                        }


                        //组号 2011-7-3 houwb
                        if (this.Reader.FieldCount >= 96)
                        {
                            objOrder.SubCombNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[95]);
                        }

                        //执行结果 2011-7-3 houwb
                        if (this.Reader.FieldCount >= 97)
                        {
                            objOrder.ExecResult = this.Reader[96].ToString();
                        }

                        if (this.Reader.FieldCount >= 98)
                        {
                            objOrder.IsGCPOrder = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[97].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得医嘱类型信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    al.Add(objOrder);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得医嘱信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();
            return al;
        }




        /// <summary>
        /// 私有函数，查询医嘱信息 allan
        /// </summary>
        /// <param name="SQLPatient"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.Order.Inpatient.Order> MyOrderQueryLists(string SQLPatient)
        {
            List<Neusoft.HISFC.Models.Order.Inpatient.Order> lists = new List<Neusoft.HISFC.Models.Order.Inpatient.Order>();
            if (this.ExecQuery(SQLPatient) == -1)
            {
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Order.Inpatient.Order objOrder = new Neusoft.HISFC.Models.Order.Inpatient.Order();
                    #region "患者信息"
                    //患者信息——  
                    //			1 住院流水号   2住院病历号     3患者科室id      4患者护理id
                    try
                    {
                        objOrder.Patient.ID = this.Reader[1].ToString();
                        objOrder.Patient.PID.PatientNO = this.Reader[2].ToString();
                        objOrder.Patient.PVisit.PatientLocation.Dept.ID = this.Reader[3].ToString();
                        objOrder.InDept.ID = this.Reader[3].ToString();
                        objOrder.Patient.PVisit.PatientLocation.NurseCell.ID = this.Reader[4].ToString();

                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得患者基本信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    #region "项目信息"
                    //医嘱辅助信息
                    // ——项目信息
                    //	       5项目类别      6项目编码       7项目名称      8项目输入码,    9项目拼音码 
                    //	       10项目类别代码 11项目类别名称  12药品规格     13药品基本剂量  14剂量单位       
                    //         15最小单位     16包装数量,     17剂型代码     18药品类别  ,   19药品性质
                    //         20零售价       21用法代码      22用法名称     23用法英文缩写  24频次代码  
                    //         25频次名称     26每次剂量      27项目总量     28计价单位      29使用天数			  
                    // 判断药品/非药品
                    //         25频次名称     26每次剂量      27项目总量     28计价单位      29使用天数			  
                    // 73 样本类型 名称
                    if (this.Reader[5].ToString() == "1")//药品
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item objPharmacy = new Neusoft.HISFC.Models.Pharmacy.Item();
                        try
                        {
                            objPharmacy.ID = this.Reader[6].ToString();
                            objPharmacy.Name = this.Reader[7].ToString();
                            objPharmacy.UserCode = this.Reader[8].ToString();
                            objPharmacy.SpellCode = this.Reader[9].ToString();
                            objPharmacy.SysClass.ID = this.Reader[10].ToString();

                            objPharmacy.Specs = this.Reader[12].ToString();

                            if (!this.Reader.IsDBNull(13))
                                objPharmacy.BaseDose = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());

                            objPharmacy.DoseUnit = this.Reader[14].ToString();
                            objOrder.DoseUnit = this.Reader[14].ToString();
                            objPharmacy.MinUnit = this.Reader[15].ToString();

                            if (!this.Reader.IsDBNull(16))
                                objPharmacy.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[16].ToString());

                            objPharmacy.DosageForm.ID = this.Reader[17].ToString();
                            objPharmacy.Type.ID = this.Reader[18].ToString();
                            objPharmacy.Quality.ID = this.Reader[19].ToString();
                            // 计价单位
                            objPharmacy.PriceUnit = this.Reader[28].ToString();

                            //try
                            //{
                            if (!this.Reader.IsDBNull(20)) objPharmacy.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20].ToString());
                            //}
                            //catch{}					
                            objOrder.Usage.ID = this.Reader[21].ToString();
                            objOrder.Usage.Name = this.Reader[22].ToString();
                            objOrder.Usage.Memo = this.Reader[23].ToString();


                            if (!this.Reader.IsDBNull(87))
                            {
                                objOrder.DoseOnceDisplay = this.Reader[87].ToString();
                            }
                            if (!this.Reader.IsDBNull(88))
                            {
                                objOrder.DoseUnitDisplay = this.Reader[88].ToString();
                            }


                        }
                        catch (Exception ex)
                        {
                            this.Err = "获得医嘱项目信息出错！" + ex.Message;
                            this.WriteErr();
                            return null;
                        }
                        objOrder.Item = objPharmacy;
                    }
                    else if (this.Reader[5].ToString() == "2")//非药品
                    {
                        Neusoft.HISFC.Models.Fee.Item.Undrug objAssets = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        try
                        {
                            objAssets.ID = this.Reader[6].ToString();
                            objAssets.Name = this.Reader[7].ToString();
                            objAssets.UserCode = this.Reader[8].ToString();
                            objAssets.SpellCode = this.Reader[9].ToString();
                            objAssets.SysClass.ID = this.Reader[10].ToString();
                            //objAssets.SysClass.Name = this.Reader[11].ToString();
                            objAssets.Specs = this.Reader[12].ToString();
                            objOrder.Usage.ID = this.Reader[21].ToString();
                            objOrder.Usage.Name = this.Reader[22].ToString();
                            objOrder.Usage.Memo = this.Reader[23].ToString();
                            //							try
                            //							{
                            if (!this.Reader.IsDBNull(20)) objAssets.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20].ToString());
                            //}
                            //							catch{}	
                            objAssets.PriceUnit = this.Reader[28].ToString();
                            //样本类型名称
                            objOrder.Sample.Name = this.Reader[73].ToString();
                        }
                        catch (Exception ex)
                        {
                            this.Err = "获得医嘱项目信息出错！" + ex.Message;
                            this.WriteErr();
                            return null;
                        }
                        objOrder.Item = objAssets;
                    }


                    objOrder.Frequency.ID = this.Reader[24].ToString();
                    objOrder.Frequency.Name = this.Reader[25].ToString();
                    //try
                    //{
                    if (!this.Reader.IsDBNull(26)) objOrder.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26].ToString());//}
                    //catch{}
                    //try
                    //{
                    if (!this.Reader.IsDBNull(27)) objOrder.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27].ToString());//}
                    //catch{}
                    objOrder.Unit = this.Reader[28].ToString();
                    //try
                    //{
                    if (!this.Reader.IsDBNull(29)) objOrder.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[29].ToString());//}
                    //catch{}

                    #endregion

                    #region "医嘱属性"
                    // ——医嘱属性
                    //		   30医嘱类别代码 31医嘱类别名称  32医嘱是否分解:1长期/2临时     33是否计费 
                    //		   34药房是否配药 35打印执行单    36是否需要确认   
                    try
                    {
                        objOrder.ID = this.Reader[0].ToString();
                        objOrder.ExtendFlag1 = this.Reader[78].ToString();//临时医嘱执行时间－－自定义
                        objOrder.OrderType.ID = this.Reader[30].ToString();
                        objOrder.OrderType.Name = this.Reader[31].ToString();
                        //try
                        //{
                        if (!this.Reader.IsDBNull(32)) objOrder.OrderType.IsDecompose = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[32].ToString()));//}
                        //catch{}
                        //try
                        //{
                        if (!this.Reader.IsDBNull(33)) objOrder.OrderType.IsCharge = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[33].ToString()));//}
                        //catch{}
                        //try
                        //{
                        if (!this.Reader.IsDBNull(34)) objOrder.OrderType.IsNeedPharmacy = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[34].ToString()));//}
                        //catch{}
                        //try
                        //{
                        if (!this.Reader.IsDBNull(35)) objOrder.OrderType.IsPrint = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[35].ToString()));//}
                        //catch{}
                        //try
                        //{
                        if (!this.Reader.IsDBNull(36)) objOrder.Item.IsNeedConfirm = System.Convert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[36].ToString()));//}
                        //catch{}
                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得医嘱属性信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    #region "执行情况"
                    // ——执行情况
                    //		   37开立医师Id   38开立医师name  39开始时间      40结束时间     41开立科室
                    //		   42开立时间     43录入人员代码  44录入人员姓名  45审核人ID     46审核时间       
                    //		   47DC原因代码   48DC原因名称    49DC医师代码    50DC医师姓名   51Dc时间
                    //         52执行人员代码 53执行时间      54执行科室代码  55执行科室名称 
                    //		   56本次分解时间 57下次分解时间
                    try
                    {
                        objOrder.ReciptDoctor.ID = this.Reader[37].ToString();
                        objOrder.ReciptDoctor.Name = this.Reader[38].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(39)) objOrder.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[39].ToString());
                        //}
                        //catch{}
                        //try{
                        if (!this.Reader.IsDBNull(40)) objOrder.EndTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[40].ToString());
                        //}
                        //catch{}
                        objOrder.ReciptDept.ID = this.Reader[41].ToString();//InDept.ID
                        //try{
                        if (!this.Reader.IsDBNull(42)) objOrder.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[42].ToString());
                        //}
                        //catch{}
                        objOrder.Oper.ID = this.Reader[43].ToString();
                        objOrder.Oper.Name = this.Reader[44].ToString();
                        objOrder.Nurse.ID = this.Reader[45].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(46)) objOrder.ConfirmTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[46].ToString());
                        //}
                        //catch{}
                        objOrder.DcReason.ID = this.Reader[47].ToString();
                        objOrder.DcReason.Name = this.Reader[48].ToString();
                        objOrder.DCOper.ID = this.Reader[49].ToString();
                        objOrder.DCOper.Name = this.Reader[50].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(51))
                            objOrder.DCOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[51].ToString());
                        //}
                        //catch{}
                        objOrder.ExecOper.ID = this.Reader[52].ToString();
                        //try{
                        if (!this.Reader.IsDBNull(53)) objOrder.ExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[53].ToString());

                        objOrder.ExeDept.ID = this.Reader[54].ToString();
                        objOrder.ExeDept.Name = this.Reader[55].ToString();

                        objOrder.ExecOper.Dept.ID = this.Reader[54].ToString();
                        objOrder.ExecOper.Dept.Name = this.Reader[55].ToString();

                        if (!this.Reader.IsDBNull(56)) objOrder.CurMOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[56].ToString());

                        if (!this.Reader.IsDBNull(57)) objOrder.NextMOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[57].ToString());

                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得医嘱执行情况信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    #region "医嘱类型"
                    // ——医嘱类型
                    //		   58是否婴儿（1是/2否）          59发生序号  	  60组合序号     61主药标记 
                    //		   62是否附材'1'  63是否包含附材  64医嘱状态      65扣库标记     66执行标记1未执行/2已执行/3DC执行 
                    //		   67医嘱说明                     68加急标记:1普通/2加急         69排列序号
                    //         70 批注       ,       71检查部位备注    ,72 整档标记,74 取药药房编码 81 是否皮试 
                    //         84 申请单号
                    try
                    {

                        if (!this.Reader.IsDBNull(58)) objOrder.IsBaby = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[58].ToString()));

                        if (!this.Reader.IsDBNull(59)) objOrder.BabyNO = this.Reader[59].ToString();

                        objOrder.Combo.ID = this.Reader[60].ToString();

                        if (!this.Reader.IsDBNull(61)) objOrder.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[61].ToString()));

                        if (!this.Reader.IsDBNull(62)) objOrder.IsSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[62].ToString()));

                        if (!this.Reader.IsDBNull(63)) objOrder.IsHaveSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[63].ToString()));

                        if (!this.Reader.IsDBNull(64)) objOrder.Status = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[64].ToString());

                        if (!this.Reader.IsDBNull(65)) objOrder.IsStock = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[65].ToString()));


                        if (!this.Reader.IsDBNull(66)) objOrder.ExecStatus = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[66].ToString());

                        objOrder.Note = this.Reader[67].ToString();

                        if (!this.Reader.IsDBNull(68)) objOrder.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[68].ToString()));

                        if (!this.Reader.IsDBNull(69)) objOrder.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[69]);

                        objOrder.Memo = this.Reader[70].ToString();
                        objOrder.CheckPartRecord = this.Reader[71].ToString();
                        objOrder.Reorder = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[72].ToString());
                        objOrder.StockDept.ID = this.Reader[74].ToString();//取药药房编码
                        try
                        {
                            if (!this.Reader.IsDBNull(75)) objOrder.IsPermission = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[75]);//患者用药知情
                        }
                        catch { }
                        objOrder.Package.ID = this.Reader[76].ToString();
                        objOrder.Package.Name = this.Reader[77].ToString();
                        objOrder.ExtendFlag2 = this.Reader[79].ToString();
                        objOrder.ReTidyInfo = this.Reader[80].ToString();
                        try
                        {
                            if (!this.Reader.IsDBNull(81))
                            {
                                objOrder.HypoTest = (Neusoft.HISFC.Models.Order.EnumHypoTest)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[81].ToString());
                            }
                        }
                        catch
                        {
                            objOrder.HypoTest = 0;
                        }

                        objOrder.Frequency.Time = this.Reader[82].ToString(); //执行时间
                        objOrder.ExecDose = this.Reader[83].ToString();//特殊频次剂量
                        if (!this.Reader.IsDBNull(84)) objOrder.ApplyNo = this.Reader[84].ToString(); //申请单号


                        if (!this.Reader.IsDBNull(85))
                        {
                            objOrder.DCNurse.ID = this.Reader[85].ToString();
                        }
                        if (!this.Reader.IsDBNull(86))
                        {
                            objOrder.DCNurse.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[86].ToString());
                        }


                        if (!this.Reader.IsDBNull(89))
                        {
                            objOrder.FirstUseNum = this.Reader[89].ToString();
                        }

                        if (objOrder.FirstUseNum.Length <= 0)
                            objOrder.FirstUseNum = "0";

                        if (!this.Reader.IsDBNull(90))
                        {
                            objOrder.Patient.PVisit.PatientLocation.Bed.ID = this.Reader[90].ToString();
                        }
                        if (!this.Reader.IsDBNull(91))
                        {
                            objOrder.PageNo = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[91].ToString());
                        }
                        if (!this.Reader.IsDBNull(92))
                        {
                            objOrder.RowNo = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[92].ToString());
                        }

                        if (this.Reader[5].ToString() == "1")
                        {
                            if (objOrder.DoseOnceDisplay.Length <= 0)
                                objOrder.DoseOnceDisplay = objOrder.DoseOnce.ToString();

                            if (objOrder.DoseUnitDisplay.Length <= 0)
                                objOrder.DoseUnitDisplay = (objOrder.Item as Neusoft.HISFC.Models.Pharmacy.Item).DoseUnit.ToString();
                        }
                        // {C903923D-C0F7-4665-83C4-6CB8CC529155}
                        if (this.Reader.FieldCount >= 94)
                        {
                            objOrder.GetFlag = this.Reader[93].ToString();
                        }

                        // {C903923D-C0F7-4665-83C4-6CB8CC529155}
                        if (this.Reader.FieldCount >= 95)
                        {
                            objOrder.SpeOrderType = this.Reader[94].ToString();
                        }


                        //组号 2011-7-3 houwb
                        if (this.Reader.FieldCount >= 96)
                        {
                            objOrder.SubCombNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[95]);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得医嘱类型信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    lists.Add(objOrder);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得医嘱信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();
            return lists;
        }

        /// <summary>
        /// 判断医嘱项目类别，1药品/2非药品
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        private string JudgeItemType(Neusoft.HISFC.Models.Order.Order Order)
        {
            string strItem = "";

            if (Order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
            {
                strItem = "1";
            }
            else
            {
                strItem = "2";
            }
            return strItem;
        }


        //添加查询信息
        private void addExecOrder(ArrayList al, string sqlOrder)
        {
            ArrayList al1 = null;
            try
            {
                al1 = this.myExecOrderQuery(sqlOrder);
                //al1 = myExecOrderQueryByDataSet(sqlOrder);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
            }
            if (al1 == null) return;

            al.AddRange(al1);
            //			foreach(Object.Order.ExecOrder ExecOrder in al1)
            //			{
            //				al.Add(ExecOrder);
            //			}
        }

        /// <summary>
        /// 查询患者信息的select语句（无where条件）
        /// </summary>
        /// <param name="Type">"" 药品非药品都查，1 药品， 2 非药品</param>
        /// <returns></returns>
        private string[] ExecOrderQuerySelect(string Type)
        {
            #region 接口说明
            //Order.ExecOrder.QueryOrder.Select.1
            //传入：0
            //传出：sql.select
            #endregion
            string[] s = null;
            string sql = "";
            if (Type == "")
            {
                s = new string[2];
            }
            else
            {
                s = new string[1];
            }
            if (Type == "1" || Type == "")
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrder.Select.1", ref sql) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryOrder.Select.1字段!";
                    this.ErrCode = "-1";
                    this.WriteErr();
                    return null;
                }
                s[0] = sql;
            }
            else if (Type == "2" || Type == "")
            {
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryOrder.Select.2", ref sql) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryOrder.Select.2字段!";
                    this.ErrCode = "-1";
                    this.WriteErr();
                    return null;
                }
                if (Type == "")
                {
                    s[1] = sql;
                }
                else
                {
                    s[0] = sql;
                }
            }
            return s;
        }

        private ArrayList myExecOrderQueryByDataSet(string sql)
        {
            DataSet ds = new DataSet();
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            if (ds == null)
            {
                return null;
            }
            if (ds.Tables[0] == null)
            {
                return null;
            }

            ArrayList al = new ArrayList();

            Neusoft.HISFC.Models.Order.ExecOrder objOrder;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                objOrder = new Neusoft.HISFC.Models.Order.ExecOrder();

                objOrder.Order.Patient.ID = row[0].ToString();
                objOrder.Order.Patient.PID.PatientNO = row[1].ToString();
                objOrder.Order.Patient.PVisit.PatientLocation.Dept.ID = row[3].ToString();
                objOrder.Order.Patient.PVisit.PatientLocation.NurseCell.ID = row[4].ToString();
                objOrder.Order.NurseStation.ID = row[4].ToString();
                objOrder.Order.InDept.ID = row[3].ToString();

                if (row[5].ToString() == "1")//药品
                {
                    Neusoft.HISFC.Models.Pharmacy.Item objPharmacy = new Neusoft.HISFC.Models.Pharmacy.Item();

                    #region 药品
                    objPharmacy.ID = row[6].ToString();
                    objPharmacy.Name = row[7].ToString();
                    objPharmacy.UserCode = row[8].ToString();
                    objPharmacy.SpellCode = row[9].ToString();
                    objPharmacy.SysClass.ID = row[10].ToString();
                    //objPharmacy.SysClass.Name = row[11].ToString();
                    objPharmacy.Specs = row[12].ToString();
                    //try
                    //{
                    objPharmacy.BaseDose = Neusoft.FrameWork.Function.NConvert.ToDecimal(row[13]);
                    //}
                    //catch{}
                    objPharmacy.DoseUnit = row[14].ToString();
                    objPharmacy.MinUnit = row[15].ToString();
                    //try
                    //{
                    objPharmacy.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(row[16]);
                    //}
                    //catch{}
                    objPharmacy.DosageForm.ID = row[17].ToString();
                    objPharmacy.Type.ID = row[18].ToString();
                    objPharmacy.Quality.ID = row[19].ToString();
                    //try
                    //{
                    objPharmacy.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(row[20]);
                    //}
                    //catch{}	
                    objOrder.Order.Item = objPharmacy;


                    objOrder.Order.Usage.ID = row[21].ToString();
                    objOrder.Order.Usage.Name = row[22].ToString();
                    objOrder.Order.Usage.Memo = row[23].ToString();
                    objOrder.Order.Frequency.ID = row[24].ToString();
                    objOrder.Order.Frequency.Name = row[25].ToString();
                    //try
                    //{
                    objOrder.Order.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(row[26]);
                    //}
                    //catch{}
                    objOrder.Order.DoseUnit = objPharmacy.DoseUnit;
                    //try
                    //{
                    objOrder.Order.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(row[27]);
                    //}
                    //catch{}
                    objOrder.Order.Unit = row[28].ToString();
                    //try
                    //{
                    objOrder.Order.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToInt32(row[29]);

                    objOrder.ID = row[0].ToString();
                    objOrder.Order.ID = row[30].ToString();
                    objOrder.Order.OrderType.ID = row[31].ToString();
                    objOrder.Order.OrderType.IsDecompose = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[32]);
                    objOrder.Order.OrderType.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[33]);
                    objOrder.Order.OrderType.IsNeedPharmacy = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[34]);
                    objOrder.Order.OrderType.IsPrint = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[35]);
                    objOrder.Order.Item.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[36]);
                    objOrder.Order.ReciptDoctor.ID = row[37].ToString();
                    objOrder.Order.ReciptDoctor.Name = row[38].ToString();
                    objOrder.DateUse = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[39]);
                    objOrder.DCExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[40]);
                    objOrder.Order.ReciptDept.ID = row[41].ToString();
                    objOrder.Order.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[42]);
                    objOrder.DCExecOper.ID = row[43].ToString();
                    objOrder.ChargeOper.ID = row[44].ToString();
                    objOrder.ChargeOper.Dept.ID = row[45].ToString();
                    objOrder.ChargeOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[46]);
                    objOrder.Order.StockDept.ID = row[47].ToString();
                    objOrder.Order.ExeDept.ID = row[48].ToString();
                    objOrder.ExecOper.ID = row[49].ToString();

                    if (row[50].ToString() != "")
                        objOrder.Order.ExeDept.ID = row[50].ToString();
                    objOrder.Order.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[51]);
                    objOrder.DateDeco = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[52]);
                    objOrder.Order.IsBaby = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[53]);
                    objOrder.Order.BabyNO = row[54].ToString();
                    objOrder.Order.Combo.ID = row[55].ToString();
                    objOrder.Order.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[56]);
                    objOrder.Order.IsHaveSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[57]);
                    objOrder.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[58]);
                    objOrder.Order.IsStock = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[59]);
                    objOrder.IsExec = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[60]);
                    objOrder.DrugFlag = Neusoft.FrameWork.Function.NConvert.ToInt32(row[61]);
                    objOrder.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[62]);
                    objOrder.Order.Note = row[63].ToString();
                    objOrder.Order.Memo = row[64].ToString();
                    objOrder.Order.ReciptNO = row[65].ToString();
                    objOrder.Order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(row[66]);
                    //objOrder.Order.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[67]);//药品紧急标识----暂时不用
                    #endregion
                }
                if (row[5].ToString() == "2")//非药品
                {
                    Neusoft.HISFC.Models.Fee.Item.Undrug objAssets = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                    #region 非药品

                    // ——项目信息
                    //	       5项目类别      6项目编码       7项目名称      8项目输入码,    9项目拼音码 
                    //	       10项目类别代码 11项目类别名称  12规格         13零售价        14用法代码   
                    //         15用法名称     16用法英文缩写  17频次代码     18频次名称      19每次用量
                    //         20项目总量     21计价单位      22使用次数	
                    objAssets.ID = row[6].ToString();
                    objAssets.Name = row[7].ToString();
                    objAssets.UserCode = row[8].ToString();
                    objAssets.SpellCode = row[9].ToString();
                    objAssets.SysClass.ID = row[10].ToString();
                    //objAssets.SysClass.Name = row[11].ToString();
                    objAssets.Specs = row[12].ToString();
                    objAssets.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(row[13].ToString());
                    objAssets.PriceUnit = row[21].ToString();
                    objOrder.Order.Item = objAssets;
                    objOrder.Order.Usage.ID = row[14].ToString();
                    objOrder.Order.Usage.Name = row[15].ToString();
                    objOrder.Order.Usage.Memo = row[16].ToString();
                    objOrder.Order.Frequency.ID = row[17].ToString();
                    objOrder.Order.Frequency.Name = row[18].ToString();
                    objOrder.Order.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(row[19]);
                    objOrder.Order.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(row[20]);
                    objOrder.Order.Unit = row[21].ToString();
                    objOrder.Order.DoseUnit = objOrder.Order.Unit;
                    objOrder.Order.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToInt32(row[22]);

                    objOrder.ID = row[0].ToString();
                    objOrder.Order.OrderType.ID = row[23].ToString();
                    objOrder.Order.ID = row[24].ToString();
                    objOrder.Order.OrderType.IsDecompose = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[25]);
                    objOrder.Order.OrderType.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[26]);
                    objOrder.Order.OrderType.IsNeedPharmacy = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[27]);
                    objOrder.Order.OrderType.IsPrint = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[28]);
                    objOrder.Order.Item.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[29]);
                    objOrder.Order.ReciptDoctor.ID = row[30].ToString();
                    objOrder.Order.ReciptDoctor.Name = row[31].ToString();
                    objOrder.DateUse = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[32]);
                    objOrder.DCExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[33]);
                    objOrder.Order.ReciptDept.ID = row[34].ToString();
                    objOrder.Order.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[35]);
                    objOrder.DCExecOper.ID = row[36].ToString();
                    objOrder.ChargeOper.ID = row[37].ToString();
                    objOrder.ChargeOper.Dept.ID = row[38].ToString();
                    objOrder.ChargeOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[39]);
                    objOrder.Order.StockDept.ID = row[40].ToString();
                    objOrder.Order.ExeDept.ID = row[41].ToString();
                    objOrder.ExecOper.ID = row[42].ToString();
                    objOrder.Order.ExeDept.ID = row[43].ToString();
                    objOrder.ExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[44]);
                    objOrder.DateDeco = Neusoft.FrameWork.Function.NConvert.ToDateTime(row[45]);
                    objOrder.Order.ExeDept.Name = row[46].ToString();
                    objOrder.Order.IsBaby = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[47]);
                    objOrder.Order.BabyNO = row[48].ToString();
                    objOrder.Order.Combo.ID = row[49].ToString();
                    objOrder.Order.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[50]);
                    objOrder.Order.IsSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[51]);
                    objOrder.Order.IsHaveSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[52]);
                    objOrder.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[53]);
                    objOrder.IsExec = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[54]);
                    objOrder.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[55]);
                    objOrder.Order.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(row[56]);
                    objOrder.Order.CheckPartRecord = row[57].ToString();

                    objOrder.Order.Note = row[58].ToString();
                    objOrder.Order.Memo = row[59].ToString();
                    objOrder.Order.ReciptNO = row[60].ToString();
                    objOrder.Order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(row[61]);
                    objOrder.Order.StockDept.ID = row[62].ToString();
                    try
                    {
                        objOrder.Order.Sample.Name = row[63].ToString();			//样本类型
                        objOrder.Order.Sample.Memo = row[64].ToString();			//检验条码号
                    }
                    catch { }
                    #endregion
                }

                al.Add(objOrder);
            }
            return al;
        }

        /// <summary>
        /// 私有函数，查询医嘱信息
        /// </summary>
        /// <param name="SQLPatient"></param>
        /// <returns></returns>
        private ArrayList myExecOrderQuery(string SQLPatient)
        {
            ArrayList al = new ArrayList();

            if (this.ExecQuery(SQLPatient) == -1)
            {
                return null;
            }

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Order.ExecOrder objOrder = new Neusoft.HISFC.Models.Order.ExecOrder();

                    //0、执行流水号
                    objOrder.ID = this.Reader[0].ToString();
                    //1、住院流水号
                    objOrder.Order.Patient.ID = this.Reader[1].ToString();
                    //2、住院号
                    objOrder.Order.Patient.PID.PatientNO = this.Reader[2].ToString();
                    //3、住院科室
                    objOrder.Order.Patient.PVisit.PatientLocation.Dept.ID = this.Reader[3].ToString();
                    objOrder.Order.InDept.ID = this.Reader[3].ToString();
                    //4、住院病区
                    objOrder.Order.Patient.PVisit.PatientLocation.NurseCell.ID = this.Reader[4].ToString();
                    objOrder.Order.NurseStation.ID = this.Reader[4].ToString();

                    #region 药品

                    //5、项目类型
                    if (this.Reader[5].ToString() == "1")//药品
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item objPharmacy = new Neusoft.HISFC.Models.Pharmacy.Item();

                        #region "项目信息"

                        //6、药品项目编码
                        objPharmacy.ID = this.Reader[6].ToString();
                        //7、药品名称
                        objPharmacy.Name = this.Reader[7].ToString();
                        //8、药品自定义码
                        objPharmacy.UserCode = this.Reader[8].ToString();
                        //9、药品拼音码
                        objPharmacy.SpellCode = this.Reader[9].ToString();
                        //10、系统类别编码
                        objPharmacy.SysClass.ID = this.Reader[10].ToString();
                        //11、系统类别名称
                        objPharmacy.SysClass.ID = this.Reader[11].ToString();
                        //12、药品规格
                        objPharmacy.Specs = this.Reader[12].ToString();
                        //13、基本计量
                        objPharmacy.BaseDose = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13]);
                        //14、剂量单位
                        objPharmacy.DoseUnit = this.Reader[14].ToString();
                        //15、最小单位
                        objPharmacy.MinUnit = this.Reader[15].ToString();
                        //16、包装数量
                        objPharmacy.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[16]);
                        //17、剂型编码
                        objPharmacy.DosageForm.ID = this.Reader[17].ToString();
                        //18、药品类别
                        objPharmacy.Type.ID = this.Reader[18].ToString();
                        //19、药品性质
                        objPharmacy.Quality.ID = this.Reader[19].ToString();
                        //20、零售价
                        objPharmacy.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20]);

                        objOrder.Order.Item = objPharmacy;
                        #endregion

                        //21、用法编码
                        objOrder.Order.Usage.ID = this.Reader[21].ToString();
                        //22、用法名称
                        objOrder.Order.Usage.Name = this.Reader[22].ToString();
                        //23、用法英文名称
                        objOrder.Order.Usage.Memo = this.Reader[23].ToString();
                        //24、频次编码
                        objOrder.Order.Frequency.ID = this.Reader[24].ToString();
                        //25、频次名称
                        objOrder.Order.Frequency.Name = this.Reader[25].ToString();
                        //26、每次用量
                        objOrder.Order.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26]);
                        //每次用量单位 貌似这个为空的...
                        objOrder.Order.DoseUnit = objPharmacy.DoseUnit;
                        //27、数量
                        objOrder.Order.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27]);
                        //28、数量单位
                        objOrder.Order.Unit = this.Reader[28].ToString();
                        //29、付数/天数
                        objOrder.Order.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[29]);

                        #region "医嘱属性"

                        //30、医嘱流水号
                        objOrder.Order.ID = this.Reader[30].ToString();
                        //31、医嘱类别编码
                        objOrder.Order.OrderType.ID = this.Reader[31].ToString();
                        //32、是否分解
                        objOrder.Order.OrderType.IsDecompose = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[32]);
                        //33、是否收费医嘱
                        objOrder.Order.OrderType.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[33]);
                        //34、是否需要配药
                        objOrder.Order.OrderType.IsNeedPharmacy = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[34]);
                        //35、是否需要打印
                        objOrder.Order.OrderType.IsPrint = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[35]);
                        //36、是否需要确认
                        objOrder.Order.Item.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[36]);
                        #endregion

                        #region "执行情况"
                        //37、开立医生编码
                        objOrder.Order.ReciptDoctor.ID = this.Reader[37].ToString();
                        //38、开立医生名称
                        objOrder.Order.ReciptDoctor.Name = this.Reader[38].ToString();
                        //39、使用时间
                        objOrder.DateUse = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[39]);
                        //40、作废时间
                        objOrder.DCExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[40]);
                        //41、开立科室编码
                        objOrder.Order.ReciptDept.ID = this.Reader[41].ToString();
                        //42、开立时间
                        objOrder.Order.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[42]);
                        //43、作废人编码
                        objOrder.DCExecOper.ID = this.Reader[43].ToString();
                        //44、收费人编码
                        objOrder.ChargeOper.ID = this.Reader[44].ToString();
                        //45、收费科室
                        objOrder.ChargeOper.Dept.ID = this.Reader[45].ToString();
                        //46、收费时间
                        objOrder.ChargeOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[46]);
                        //47、取药药房编码
                        objOrder.Order.StockDept.ID = this.Reader[47].ToString();
                        //48、执行科室编码
                        objOrder.Order.ExeDept.ID = this.Reader[48].ToString();
                        //49、执行者
                        objOrder.ExecOper.ID = this.Reader[49].ToString();
                        //50、执行科室编码
                        if (this.Reader[50].ToString() != "")
                        {
                            objOrder.Order.ExeDept.ID = this.Reader[50].ToString();
                        }
                        //51、执行时间
                        objOrder.ExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[51]);
                        //52、分解时间
                        objOrder.DateDeco = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[52]);
                        #endregion

                        #region "医嘱类型"

                        try
                        {
                            //53、是否婴儿
                            objOrder.Order.IsBaby = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[53]);

                            //54、婴儿序号
                            objOrder.Order.BabyNO = this.Reader[54].ToString();
                            //55、组合号
                            objOrder.Order.Combo.ID = this.Reader[55].ToString();
                            //56、是否主药
                            objOrder.Order.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[56]);
                            //57、是否包含附材
                            objOrder.Order.IsHaveSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[57]);
                            //58、有效标记
                            objOrder.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[58]);
                            //59、是否扣药房库存
                            objOrder.Order.IsStock = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[59]);
                            //60、是否已执行
                            objOrder.IsExec = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[60]);
                            //61、发送标记 1不需发送/2集中发送/3分散发送/4已配药
                            objOrder.DrugFlag = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[61]);
                            //62、是否已收费
                            objOrder.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[62]);
                            //63、批注信息
                            objOrder.Order.Note = this.Reader[63].ToString();
                            //64、医嘱备注
                            objOrder.Order.Memo = this.Reader[64].ToString();
                            //65、处方号
                            objOrder.Order.ReciptNO = this.Reader[65].ToString();
                            //66、处方内流水号
                            objOrder.Order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[66]);


                            //67、配药科室...木有存储？？？
                            //objOrder.dr = this.Reader[67].ToString();

                            if (!Reader.IsDBNull(68))
                            {
                                //68、发药时间
                                objOrder.DrugedTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[68].ToString());
                            }
                            if (this.Reader.FieldCount >= 70)
                            {
                                //69、医嘱开始时间
                                objOrder.Order.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[69].ToString());
                            }
                            if (this.Reader.FieldCount >= 71)
                            {
                                //70、特殊医嘱标识 手术医嘱等
                                objOrder.Order.SpeOrderType = this.Reader[70].ToString();
                            }
                            if (this.Reader.FieldCount >= 72)
                            {
                                //71、是否加急
                                objOrder.Order.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[71]);
                            }
                            if (this.Reader.FieldCount >= 73)
                            {
                                //72、皮试标记
                                objOrder.Order.HypoTest = (Neusoft.HISFC.Models.Order.EnumHypoTest)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[72].ToString());
                            }
                            if (this.Reader.FieldCount >= 74)
                            {
                                //73、组号
                                objOrder.Order.SubCombNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[73]);
                            }
                            if (this.Reader.FieldCount >= 75)
                            {
                                //74、医嘱排序号
                                objOrder.Order.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[74]);
                            }
                            if (this.Reader.FieldCount >= 76)
                            {
                                //75、床号
                                objOrder.Order.Patient.PVisit.PatientLocation.Bed.ID = this.Reader[75].ToString();
                            }
                            if (this.Reader.FieldCount >= 77)
                            {
                                //76、医嘱状态
                                objOrder.Order.Status = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[76]);
                            }
                            if (this.Reader.FieldCount >= 78)
                            {
                                //77、医嘱停止时间
                                objOrder.Order.EndTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[77]);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Err = "获得医嘱类型信息出错！" + ex.Message;
                            this.WriteErr();
                            return null;
                        }
                        #endregion
                    }
                    #endregion

                    #region 非药品

                    //5、项目类型
                    else if (this.Reader[5].ToString() == "2")
                    {
                        Neusoft.HISFC.Models.Fee.Item.Undrug undrugItem = new Neusoft.HISFC.Models.Fee.Item.Undrug();

                        //6、非药品编码
                        undrugItem.ID = this.Reader[6].ToString();
                        //7、非药品项目
                        undrugItem.Name = this.Reader[7].ToString();
                        //8、非药品自定义码
                        undrugItem.UserCode = this.Reader[8].ToString();
                        //9、非药品拼音码
                        undrugItem.SpellCode = this.Reader[9].ToString();
                        //10、系统类别
                        undrugItem.SysClass.ID = this.Reader[10].ToString();
                        //11、系统类别名称
                        undrugItem.SysClass.Name = this.Reader[11].ToString();
                        //12、规格
                        undrugItem.Specs = this.Reader[12].ToString();
                        //13、价格
                        undrugItem.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());
                        //14、用法编码
                        objOrder.Order.Usage.ID = this.Reader[14].ToString();
                        //15、用法名称
                        objOrder.Order.Usage.Name = this.Reader[15].ToString();
                        //16、用法英文名..用法备注
                        objOrder.Order.Usage.Memo = this.Reader[16].ToString();
                        //17、频次编码
                        objOrder.Order.Frequency.ID = this.Reader[17].ToString();
                        //18、频次名称
                        objOrder.Order.Frequency.Name = this.Reader[18].ToString();
                        //19、每次用量
                        objOrder.Order.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[19]);
                        //20、数量
                        objOrder.Order.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20]);
                        undrugItem.Qty = objOrder.Order.Qty;
                        //21、单位
                        undrugItem.PriceUnit = this.Reader[21].ToString();
                        objOrder.Order.Unit = this.Reader[21].ToString();
                        objOrder.Order.DoseUnit = objOrder.Order.Unit;
                        //22、天数
                        objOrder.Order.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[22]);

                        objOrder.Order.Item = undrugItem;

                        //23、医嘱类别
                        objOrder.Order.OrderType.ID = this.Reader[23].ToString();
                        //24、医嘱流水号
                        objOrder.Order.ID = this.Reader[24].ToString();
                        //25、是否分解
                        objOrder.Order.OrderType.IsDecompose = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[25]);
                        //26、是否计费医嘱
                        objOrder.Order.OrderType.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[26]);
                        //27、是否需要发药...
                        objOrder.Order.OrderType.IsNeedPharmacy = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[27]);
                        //28、是否需要打印
                        objOrder.Order.OrderType.IsPrint = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[28]);
                        //29、是否需要确认
                        objOrder.Order.Item.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[29]);
                        //30、开立医生编码
                        objOrder.Order.ReciptDoctor.ID = this.Reader[30].ToString();
                        //31、开立医生名称
                        objOrder.Order.ReciptDoctor.Name = this.Reader[31].ToString();
                        //32、使用时间
                        objOrder.DateUse = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[32]);
                        //33、作废时间
                        objOrder.DCExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[33]);
                        //34、开立科室编码
                        objOrder.Order.ReciptDept.ID = this.Reader[34].ToString();
                        //35、医嘱时间
                        objOrder.Order.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[35]);
                        //36、作废者
                        objOrder.DCExecOper.ID = this.Reader[36].ToString();
                        //37、收费人
                        objOrder.ChargeOper.ID = this.Reader[37].ToString();
                        //38、收费科室
                        objOrder.ChargeOper.Dept.ID = this.Reader[38].ToString();
                        //39、收费时间
                        objOrder.ChargeOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[39]);
                        //40、扣库科室
                        objOrder.Order.StockDept.ID = this.Reader[40].ToString();
                        //41、执行科室编码
                        objOrder.Order.ExeDept.ID = this.Reader[41].ToString();
                        //42、执行者
                        objOrder.ExecOper.ID = this.Reader[42].ToString();
                        //43、执行科室编码 无用，参考41
                        //objOrder.ExeDept.ID = this.Reader[43].ToString();//这个字段就是没用的
                        //44、执行时间
                        objOrder.ExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[44]);
                        //45、分解时间
                        objOrder.DateDeco = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[45]);
                        //46、执行科室名称
                        objOrder.Order.ExeDept.Name = this.Reader[46].ToString();

                        //47、是否婴儿
                        objOrder.Order.IsBaby = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[47]);

                        //48、婴儿序号
                        objOrder.Order.BabyNO = this.Reader[48].ToString();

                        //49、组合号
                        objOrder.Order.Combo.ID = this.Reader[49].ToString();
                        //50、是否主项目
                        objOrder.Order.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[50]);
                        //51、是否附材
                        objOrder.Order.IsSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[51]);
                        //52、是否包含附材
                        objOrder.Order.IsHaveSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[52]);
                        //53、是否有效
                        objOrder.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[53]);
                        //54、是否已执行
                        objOrder.IsExec = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[54]);
                        //55、是否已收费
                        objOrder.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[55]);
                        //56、是否加急
                        objOrder.Order.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[56]);//非药品紧急
                        //57、检查部位、样本类型
                        objOrder.Order.CheckPartRecord = this.Reader[57].ToString();
                        //58、批注信息
                        objOrder.Order.Note = this.Reader[58].ToString();
                        //59、医嘱备注
                        objOrder.Order.Memo = this.Reader[59].ToString();
                        //60、处方号
                        objOrder.Order.ReciptNO = this.Reader[60].ToString();
                        //61、方内流水号
                        objOrder.Order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[61]);
                        //62、扣库科室编码
                        objOrder.Order.StockDept.ID = this.Reader[62].ToString();


                        if (this.Reader.FieldCount >= 64)
                        {
                            //63、样本类型
                            objOrder.Order.Sample.Name = this.Reader[63].ToString();			//样本类型
                        }

                        if (this.Reader.FieldCount >= 65)
                        {
                            //64、检验条码号
                            objOrder.Order.Sample.Memo = this.Reader[64].ToString();			//检验条码号
                        }

                        if (this.Reader.FieldCount >= 66)
                        {
                            //65、医嘱开始时间
                            objOrder.Order.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[65].ToString());
                        }

                        if (this.Reader.FieldCount >= 67)
                        {
                            //66、特殊医嘱标识 手术医嘱等
                            objOrder.Order.SpeOrderType = this.Reader[66].ToString();
                        }
                        if (this.Reader.FieldCount >= 68)
                        {
                            //67、是否加急
                            objOrder.Order.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[67]);
                        }
                        if (this.Reader.FieldCount >= 69)
                        {
                            //68、皮试标记
                            objOrder.Order.HypoTest = (Neusoft.HISFC.Models.Order.EnumHypoTest)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[68].ToString());
                        }
                        if (this.Reader.FieldCount >= 70)
                        {
                            //69、组号
                            objOrder.Order.SubCombNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[69]);
                        }
                        if (this.Reader.FieldCount >= 71)
                        {
                            //70、医嘱排序号
                            objOrder.Order.SortID = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[70]);
                        }
                        if (this.Reader.FieldCount >= 72)
                        {
                            //71、床号
                            objOrder.Order.Patient.PVisit.PatientLocation.Bed.ID = this.Reader[71].ToString();
                        }
                        if (this.Reader.FieldCount >= 73)
                        {
                            //72、医嘱状态
                            objOrder.Order.Status = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[72]);
                        }
                        if (this.Reader.FieldCount >= 74)
                        {
                            //73、医嘱停止时间
                            objOrder.Order.EndTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[73]);
                        }
                    }
                    #endregion

                    al.Add(objOrder);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得医嘱信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return al;
        }

        #endregion

        #region 对药房，医技接口
        /// <summary>
        /// 执行记录
        /// 更新医嘱执行信息
        /// 对医技开放使用
        /// </summary>
        /// <param name="execOrder">执行档信息</param>
        /// <returns>0 success -1 fail</returns>
        public int UpdateRecordExec(Neusoft.HISFC.Models.Order.ExecOrder execOrder)
        {
            #region 执行记录
            //执行记录
            //Order.ExecOrder.RecordExec.1
            //传入：0 id，1 执行人id,2执行科室，3执行科室名称 4执行时间,5执行标志 
            //传出：0 
            #endregion

            string strSql = "", strSqlName = "Order.ExecOrder.RecordExec.";
            string strItemType = "";

            strItemType = JudgeItemType(execOrder.Order);
            if (strItemType == "") return -1;
            strSqlName = strSqlName + strItemType;

            if (this.Sql.GetCommonSql(strSqlName, ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrder.ID, execOrder.Order.Oper.ID, execOrder.Order.ExeDept.ID, execOrder.Order.ExeDept.Name, execOrder.Order.ExecOper.OperTime.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsExec).ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsConfirm).ToString()/*确认标记{DA77B01B-63DF-4559-B264-798E54F24ABB}*/);
            }
            catch
            {
                this.Err = "传入参数不对！" + strSqlName;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 收费记录
        /// 更新执行医嘱收费人，收费标记，发票号等
        /// 对费用开放使用
        /// </summary>
        /// <param name="execOrder">执行档信息</param>
        /// <returns>0 success -1 fail</returns>
        public int UpdateChargeExec(Neusoft.HISFC.Models.Order.ExecOrder execOrder)
        {
            #region 收费记录
            //收费记录
            //Order.ExecOrder.Charge.
            //传入：0 id，1 收费人id,2收费科室ID，3收费时间,5收费标志 6 处方号 7处方流水序号
            //传出：0 
            #endregion
            string strSql = "", strSqlName = "Order.ExecOrder.Charge.";
            string strItemType = "";
            strItemType = JudgeItemType(execOrder.Order);
            if (strItemType == "") return -1;
            //广四添加
            string strSqlNameExt = "Order.ExecOrder.Charge.Ext.";
            strSqlNameExt = strSqlNameExt + strItemType;
            this.Sql.GetCommonSql(strSqlNameExt, ref strSql);
            if (string.IsNullOrEmpty(strSql))
            {
                strSqlName = strSqlName + strItemType;
                if (this.Sql.GetCommonSql(strSqlName, ref strSql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }
            }
            try
            {
                strSql = string.Format(strSql, execOrder.ID,
                    execOrder.ChargeOper.ID, execOrder.ChargeOper.Dept.ID, execOrder.ChargeOper.OperTime.ToString(),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsCharge).ToString(),
                    execOrder.Order.ReciptNO, execOrder.Order.SequenceNO, Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsValid));
            }
            catch (Exception ex)
            {
                this.Err = "传入参数不对！" + strSqlName + ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 配药记录
        /// 对药房开放使用,更新DrugFlag
        /// </summary>
        /// <param name="execOrder">执行档信息</param>
        /// <returns>0 success -1 fail</returns>
        public int UpdateDrugExec(Neusoft.HISFC.Models.Order.ExecOrder execOrder)
        {
            #region 配药记录
            //配药记录
            //Order.ExecOrder.DrugExec.
            //传入：0 id，1 配药状态 
            //传出：0 
            #endregion
            string strSql = "";
            string strItemType = "";

            strItemType = JudgeItemType(execOrder.Order);
            if (strItemType != "1")
            {
                this.Err = Neusoft.FrameWork.Management.Language.Msg("请传入药品医嘱!");
                return -1;
            }

            if (this.Sql.GetCommonSql("Order.ExecOrder.DrugExec.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrder.ID, execOrder.DrugFlag.ToString());
            }
            catch (Exception ex)
            {
                this.Err = "传入参数不对！Order.ExecOrder.DrugExec.1" + ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 更新医嘱配药标记
        /// 对药房的接口
        /// 对药房开放使用
        /// </summary>
        /// <param name="execOrderID">执行单ID</param>
        /// <param name="orderNo">主挡ID</param>
        /// <param name="userID">操作员</param>
        /// <param name="deptID">配药科室</param>
        /// <returns>-1 失败 0 成功</returns>
        public int UpdateOrderDruged(string execOrderID, string orderNo, string userID, string deptID)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Update.ExecOrder.Druged", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrderID, userID, deptID);
            }
            catch (Exception ex)
            {
                this.Err = "传入参数不对！Order.Update.ExecOrder.Druged" + ex.Message;
                this.WriteErr();
                return -1;
            }
            if (this.ExecNoQuery(strSql) <= 0) return -1;//更新执行档发药标记

            if (orderNo == "")
            {
                Neusoft.HISFC.Models.Order.ExecOrder obj = this.QueryExecOrderByExecOrderID(execOrderID, "1");//获得主挡信息
                if (obj == null)
                {
                    this.Err = "更新配药标记出错！" + this.Err;
                    this.WriteErr();
                    return -1;
                }
                return this.UpdateOrderStatus(obj.Order.ID, 3);
            }
            else
            {
                return this.UpdateOrderStatus(orderNo, 3);
            }
        }
        /// <summary>
        /// 更新医嘱配药标记
        /// 对药房开放使用
        /// </summary>
        /// <param name="execOrderID">执行单ID</param>
        /// <param name="userID">操作员</param>
        /// <param name="deptID">配药科室</param>
        /// <returns>-1 失败 0 成功</returns>
        public int UpdateOrderDruged(string execOrderID, string userID, string deptID)
        {
            return UpdateOrderDruged(execOrderID, "", userID, deptID);
        }


        /// <summary>
        /// 发送摆药通知\设置发药方式
        /// 0不需发送/1集中发送/2分散发送/3已配药
        /// 对药房开放使用
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <param name="drugFlag">0不需发送/1集中发送/2分散发送/3已配药</param>
        /// <returns></returns>
        public int SetDrugFlag(string execOrderID, int drugFlag)
        {
            #region 更新发送方式
            //更新发送方式
            //Order.Order.SetDrugFlag.1
            //传入：0 OrderExecID，1 DrugFlag
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.SetDrugFlag.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrderID, drugFlag.ToString());
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.SetDrugFlag.1";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 更新发送通知
        /// 对药房开放使用
        /// </summary>
        /// <param name="nurse"></param>
        /// <returns></returns>
        public int SendInformation(Neusoft.FrameWork.Models.NeuObject nurse)
        {
            #region 更新发送通知
            //更新发送通知
            //传入：0 nurseid，1 nursename,2操作人ID 3 操作人姓名，3操作时间
            //传出：0 
            #endregion
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.send.insert.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, nurse.ID, nurse.Name, this.Operator.ID, this.Operator.Name, (this.GetDateTimeFromSysDateTime().Date).ToString());
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.send.insert.1";
                return -1;
            }
            if (this.ExecNoQuery(strSql) >= 0) return 0;

            if (this.Sql.GetCommonSql("Order.Order.send.update.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, nurse.ID, nurse.Name, this.Operator.ID, this.Operator.Name, (this.GetDateTimeFromSysDateTime().Date).ToString());
            }
            catch
            {
                this.Err = "传入参数不对！Order.Order.send.update.1";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);

        }



        #endregion

        #region "Lis更新条码"
        /// <summary>
        /// 更新lis条码号
        /// 对LIS开放使用
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public int UpdateExecOrderLisBarCode(string execOrderID, string barCode)
        {
            string strSql = "";
            //Order.ExecOrder.UpdateLisBarCode
            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateLisBarCode", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrderID, barCode);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.UpdateLisBarCode";
                this.WriteErr();
                return -1;
            }

            return this.ExecNoQuery(strSql);
        }
        /// <summary>
        /// 更新LIS已经打印
        /// 对LIS开放使用
        /// </summary>
        /// <param name="execOrderID"></param>
        /// <returns></returns>
        public int UpdateExecOrderLisPrint(string execOrderID)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.ExecOrder.UpdateLisPrinted", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, execOrderID);
            }
            catch
            {
                this.Err = "传入参数不对！UpdateExecOrderLisPrint";
                this.WriteErr();
                return -1;
            }

            return this.ExecNoQuery(strSql);
        }

        #endregion

        #region 医技查询

        /// <summary>
        /// 根据执行科室查询需要确认项目的患者的所在科室
        /// </summary>
        /// <param name="deptID">执行科室</param>
        /// <returns></returns>
        public ArrayList QueryPatientDeptByConfirmDeptID(string deptID)
        {
            ArrayList alDept = new ArrayList();
            string strSQL = "";

            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryPatientDept.NeedConfirm.1", ref strSQL) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                strSQL = string.Format(strSQL, deptID);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.QueryPatientDept.NeedConfirm.1";
                return null;
            }

            if (this.ExecQuery(strSQL) == -1)
            {
                return null;
            }

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.FrameWork.Models.NeuObject obj = new NeuObject();

                    obj.ID = this.Reader[0].ToString();

                    alDept.Add(obj);
                }
            }
            catch (Exception ex)
            {
                this.Err = "查询出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();

            return alDept;
        }


        /// <summary>
        /// 根据执行科室查询需要确认项目的患者的所在科室
        /// </summary>
        /// <param name="deptID">执行科室</param>
        /// <returns></returns>
        public ArrayList QueryPatientDeptByConfirmDeptID(string deptID, DateTime beginTime, DateTime endTime)
        {
            ArrayList alDept = new ArrayList();
            string strSQL = "";

            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryPatientDept.NeedConfirm.2", ref strSQL) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                strSQL = string.Format(strSQL, deptID, beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.QueryPatientDept.NeedConfirm.2";
                return null;
            }

            if (this.ExecQuery(strSQL) == -1)
            {
                return null;
            }

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.FrameWork.Models.NeuObject obj = new NeuObject();

                    obj.ID = this.Reader[0].ToString();

                    alDept.Add(obj);
                }
            }
            catch (Exception ex)
            {
                this.Err = "查询出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();

            return alDept;
        }

        /// <summary>
        /// 根据执行科室、患者所在科室查询需要确认项目的患者
        /// </summary>
        /// <param name="confirmDept">执行科室</param>
        /// <param name="patientDept">患者所在科室</param>
        /// <returns></returns>
        public ArrayList QueryPatientByConfirmDeptAndPatDept(string confirmDept, string patientDept)
        {
            ArrayList alPatient = new ArrayList();
            string strSQL = "";

            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryPatient.NeedConfirm.1", ref strSQL) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                strSQL = string.Format(strSQL, confirmDept, patientDept);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.QueryPatient.NeedConfirm.1";
                return null;
            }

            if (this.ExecQuery(strSQL) == -1)
            {
                return null;
            }

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.FrameWork.Models.NeuObject obj = new NeuObject();

                    obj.ID = this.Reader[0].ToString();

                    alPatient.Add(obj);
                }
            }
            catch (Exception ex)
            {
                this.Err = "查询出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();

            return alPatient;
        }

        /// <summary>
        /// 根据执行科室、患者所在科室查询需要确认项目的患者
        /// </summary>
        /// <param name="confirmDept">执行科室</param>
        /// <param name="patientDept">患者所在科室</param>
        /// <returns></returns>
        public ArrayList QueryPatientByConfirmDeptAndPatDept(string confirmDept, string patientDept, DateTime beginTime, DateTime endTime)
        {
            ArrayList alPatient = new ArrayList();
            string strSQL = "";

            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryPatient.NeedConfirm.2", ref strSQL) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            try
            {
                strSQL = string.Format(strSQL, confirmDept, patientDept, beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.QueryPatient.NeedConfirm.2";
                return null;
            }

            if (this.ExecQuery(strSQL) == -1)
            {
                return null;
            }

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.FrameWork.Models.NeuObject obj = new NeuObject();

                    obj.ID = this.Reader[0].ToString();

                    alPatient.Add(obj);
                }
            }
            catch (Exception ex)
            {
                this.Err = "查询出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();

            return alPatient;
        }

        #region {5197289A-AB55-410b-81EE-FC7C1B7CB5D7}
        /// <summary>
        /// 校验长期非药品医嘱执行档护士是否分解保存
        /// </summary>
        /// <param name="execOrderID">执行档流水号</param>
        /// <returns></returns>
        public bool CheckLongUndrugIsConfirm(string execOrderID)
        {
            string strSQL = "";

            if (this.Sql.GetCommonSql("Order.ExecOrder.LongUndrug.CheckIsConfirm.1", ref strSQL) == -1)
            {
                this.Err = this.Sql.Err;
                return false;
            }

            try
            {
                strSQL = string.Format(strSQL, execOrderID);
            }
            catch
            {
                this.Err = "传入参数不对！Order.ExecOrder.LongUndrug.CheckIsConfirm.1";
                return false;
            }

            string flag = this.ExecSqlReturnOne(strSQL, "0");

            return Neusoft.FrameWork.Function.NConvert.ToBoolean(flag);
        }
        #endregion

        #endregion

        #region 医嘱打印

        /// <summary>
        /// 查询医嘱单打印的医嘱
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <returns></returns>
        public ArrayList QueryPrnOrder(string inpatientNO)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatient", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatient字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO);
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 查询医嘱单打印的当前页医嘱
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="decmpsState">医嘱类型:1长期/0临时</param>
        /// <param name="currentPageNo">当前页码</param>
        /// <returns></returns>
        public ArrayList QueryPrnOrderByPageNo(string inpatientNO, string orderType, int currentPageNo)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatientAndPageNo", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatientAndPageNo字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, orderType, currentPageNo);
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 查询医嘱单打印的当前页医嘱
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="decmpsState">医嘱类型:1长期/0临时</param>
        /// <returns></returns>
        public ArrayList QueryPrnOrderByOrderType(string inpatientNO, string orderType)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatientAndOrderType", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatientAndOrderType字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, orderType);
            return this.MyOrderQuery(sql);
        }

        // {62002B3A-B84B-4d14-82D2-B4EF7F92A510}    组套项目显示明细
        /// <summary>
        /// 查询医嘱单打印的当前页医嘱
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="decmpsState">医嘱类型:1长期/0临时</param>
        /// <returns></returns>
        public ArrayList QueryPrnOrderByOrderType2(string inpatientNO, string orderType)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect2();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatientAndOrderType2", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatientAndOrderType2字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, orderType);
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 医嘱单界面查询麻醉医嘱数据
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryPrnMZOrder(string inpatientNO)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryPrnMZOrder", ref sql) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryPrnMZOrder字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = string.Format(sql, inpatientNO);
            return this.MyOrderQuery(sql);
        }

        // {1FCC9C27-CDBF-4f3f-900C-FFB3F8CE160A}
        /// <summary>
        /// 查询医嘱中的开立过的明细条目
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="itemCode">项目编码</param>
        /// <returns></returns>
        public ArrayList QueryOrderByPatientNOAndItemCOde(string inpatientNO, string itemCode)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect2();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatientNOAndItemCOde", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatientNOAndItemCOde字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, itemCode);
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 查询医嘱中的开立过的重复项目
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="itemCode">项目编码</param>
        /// <returns></returns>
        public ArrayList QueryOrderByPatientNOAndItemCOde2(string inpatientNO, string itemCode)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect2();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatientNOAndItemCOde2", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatientNOAndItemCOde字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, itemCode);
            return this.MyOrderQuery(sql);
        }

        // {62002B3A-B84B-4d14-82D2-B4EF7F92A510}    组套项目显示明细
        /// <summary>
        /// 查询医嘱单打印的当前页医嘱
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="decmpsState">医嘱类型:1长期/0临时</param>
        /// <returns></returns>
        public ArrayList QueryPrnOrderByOrderType3(string inpatientNO, string orderType)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect2();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatientAndOrderType3", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatientAndOrderType3字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, orderType);
            return this.MyOrderQuery(sql);
        }

        public ArrayList QueryPrnOrderByOrderType3ForDate(string inpatientNO, string orderType, string begin, string end)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect2();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatientAndOrderType3ForDate", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatientAndOrderType3ForDate字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, orderType, begin, end);
            return this.MyOrderQuery(sql);
        }

        public ArrayList QueryPrnOrderByOrderType4ForDate(string inpatientNO, string orderType, string begin, string end)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatientAndOrderType4ForDate", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatientAndOrderType4字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, orderType, begin, end);
            return this.MyOrderQuery(sql);
        }



        // {62002B3A-B84B-4d14-82D2-B4EF7F92A510}    组套项目显示明细
        /// <summary>
        /// 查询医嘱单打印的当前页医嘱
        /// </summary>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="decmpsState">医嘱类型:1长期/0临时</param>
        /// <returns></returns>
        public ArrayList QueryPrnOrderByOrderType4(string inpatientNO, string orderType)
        {
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();
            sql = OrderQuerySelect();
            if (sql == null)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Order.OrderPrn.QueryOrderByPatientAndOrderType4", ref sql1) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.QueryOrderByPatientAndOrderType4字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = sql + " " + string.Format(sql1, inpatientNO, orderType);
            return this.MyOrderQuery(sql);
        }

        /// <summary>
        /// 查询CA签名图片以"byte[]"类型传出 GDCA接口 {F343D875-59FD-4401-A193-84DF7B506BD0} 2014-12-05 by lixuelong
        /// </summary>
        /// <param name="employeeNO">员工代码</param>
        /// <returns></returns>
        public byte[] QueryEmplSignDataByEmplNo(string employeeNO)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Ca.OrderPrn.QueryEmplSignDataByEmplNo", ref sql) == -1)
            {
                this.Err = "没有找到Ca.OrderPrn.QueryEmplSignDataByEmplNo字段!";
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            sql = string.Format(sql, employeeNO);
            System.Data.DataSet ds = new System.Data.DataSet();
            if (this.ExecQuery(sql, ref ds) == -1) return null;
            if (ds.Tables[0].Rows.Count == 0) return null;
            byte[] byteData = (byte[])ds.Tables[0].Rows[0].ItemArray[0];
            return byteData;
        }

        /// <summary>
        /// 更新提取标志
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="moOrder"></param>
        /// <param name="newFlag"></param>
        /// <param name="oldFlag"></param>
        /// <returns></returns>
        public int UpdateGetFlag(string inpatientNo, string moOrder, string newFlag, string oldFlag)
        {
            string strSql = "";
            /*
                update met_ipm_order o
                set o.get_flag = '{2}'
                where o.inpatient_no = '{0}'
                and   o.mo_order = '{1}'
                and   o.get_flag = '{3}'
             */
            if (this.Sql.GetCommonSql("Order.Order.UpdateGetFlag", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql:Order.Order.UpdateGetFlag";
                return -1;
            }

            strSql = System.String.Format(strSql, inpatientNo, moOrder, newFlag, oldFlag);

            return this.ExecNoQuery(strSql); ;
        }

        /// <summary>
        /// 更新页码和提取标志
        /// -----合并 UpdateGetFlag 和 UpdatePageNoAndRowNo 方法  by huangchw 2012-09-12
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="moOrder"></param>
        /// <param name="pageNo"></param>
        /// <param name="rowNo"></param>
        /// <param name="newFlag"></param>
        /// <param name="oldFlag"></param>
        /// <returns></returns>
        public int UpdatePageRowNoAndGetflag(
            string inpatientNo, string moOrder, string pageNo, string rowNo, string newFlag, string oldFlag)
        {
            string strSql = "";
            /*
                update met_ipm_order o
                set o.pageno   = '{2}',
                    o.rowno    = '{3',
                    o.get_flag = '{4}'
                where o.inpatient_no = '{0}'
                  and o.mo_order = '{1}'
                  and o.get_flag = '{5}'
             */
            if (this.Sql.GetCommonSql("Order.Order.UpdatePageRowNoAndGetflag", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql:Order.Order.UpdatePageRowNoAndGetflag";
                return -1;
            }

            strSql = System.String.Format(strSql, inpatientNo, moOrder, pageNo, rowNo, newFlag, oldFlag);

            return this.ExecNoQuery(strSql); ;
        }

        /// <summary>
        /// 更新页码和行号
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="moOrder"></param>
        /// <param name="pageNo"></param>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        public int UpdatePageNoAndRowNo(string inpatientNo, string moOrder, string pageNo, string rowNo)
        {
            string strSql = "";
            /*
                update met_ipm_order o
                set o.pageno = '{2}',
                o.rowno = '{3}'
                where o.inpatient_no = '{0}'
                and   o.mo_order = '{1}'
             */
            if (this.Sql.GetCommonSql("Order.Order.UpdatePageNoAndRowNo", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql:Order.Order.UpdatePageNoAndRowNo";
                return -1;
            }

            strSql = System.String.Format(strSql, inpatientNo, moOrder, pageNo, rowNo);

            return this.ExecNoQuery(strSql); ;
        }

        /// <summary>
        /// 获取已打印的最大页码
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <returns></returns>
        [Obsolete("不推荐使用，建议使用GetPrintInfo", false)]
        public int GetMaxPageNo(string inpatientNo, string orderType)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Order.GetMaxPageNo", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql:Order.Order.GetMaxPageNo";
                return -1;
            }

            strSql = System.String.Format(strSql, inpatientNo, orderType);

            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(strSql));
        }

        /// <summary>
        /// 获取已打印的最大页码和行号
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="isLong"></param>
        /// <param name="maxPageNo"></param>
        /// <param name="maxRowNo"></param>
        /// <returns></returns>
        public int GetPrintInfo(string inpatientNo, bool isLong, ref int maxPageNo, ref int maxRowNo)
        {
            string strSql = @"select t.pageno,t.rowno
                            from met_ipm_order t
                            where t.inpatient_no='{0}'
                            and t.DECMPS_STATE = '{1}'
                            order by t.pageno desc,t.rowno desc";

            //if (this.Sql.GetCommonSql("Order.Order.GetMaxPageNo", ref strSql) == -1)
            //{
            //    this.Err = "Can't Find Sql:Order.Order.GetMaxPageNo";
            //    return -1;
            //}

            string orderType = "1";
            if (!isLong)
            {
                orderType = "0";
            }

            try
            {
                strSql = System.String.Format(strSql, inpatientNo, orderType);

                this.ExecQuery(strSql);
                while (Reader.Read())
                {
                    maxPageNo = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[0]);
                    maxRowNo = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[1]);
                    break;
                }
            }
            catch (Exception ex)
            {
                Err = Err + "\r\n" + ex.Message;
                maxPageNo = 0;
                maxRowNo = 0;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 根据最大页号获得最大行号
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="orderType"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        [Obsolete("不推荐使用，建议使用GetPrintInfo", false)]
        public int GetMaxRowNoByPageNo(string inpatientNo, string orderType, string pageNo)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Order.GetMaxRowNoByPageNo", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql:Order.Order.GetMaxRowNoByPageNo";
                return -1;
            }

            strSql = System.String.Format(strSql, inpatientNo, orderType, pageNo);

            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(strSql));
        }

        #region 医嘱单重置

        /// <summary>
        /// 医嘱单打印更新
        /// </summary>
        /// <param name="lineNO">行号</param>
        /// <param name="pageNO">页号</param>
        /// <param name="inpatientNO">住院流水号</param>
        /// <param name="orderType">0 临嘱；1 长嘱；ALL 全部</param>
        /// <param name="prnFlag">打印标记</param>
        /// <returns></returns>
        public int ResetOrderPrint(string lineNO, string pageNO, string inpatientNO, string orderType, string prnFlag)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Order.OrderPrn.UpdateOrderPrint", ref sql) == -1)
            {
                this.Err = "没有找到Order.OrderPrn.UpdateOrderPrint字段!";
                return -1;
            }
            sql = string.Format(sql, lineNO, pageNO, inpatientNO, orderType, prnFlag);
            return this.ExecNoQuery(sql);
        }

        #endregion

        #endregion

        #region


        /// <summary>
        /// {97FA5C9D-F454-4aba-9C36-8AF81B7C9CCF} 扩展医嘱
        /// 按医嘱流水号查询一段时间内应该收费但是未收费的医嘱
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="itemType">项目类型</param>
        /// <param name="orderID">医嘱流水号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public ArrayList QueryUnFeeExecOrderByOrderID(string inpatientNo, string itemType, string orderID, DateTime beginDate, DateTime endDate)
        {

            string[] s;
            string sql = "", sql1 = "";
            ArrayList al = new ArrayList();

            s = ExecOrderQuerySelect(itemType);
            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                sql = s[i];
                if (sql == null)
                    return null;
                if (this.Sql.GetCommonSql("Order.ExecOrder.QueryUnFeeExecOrderByOrderID.1", ref sql1) == -1)
                {
                    this.Err = "没有找到Order.ExecOrder.QueryUnFeeExecOrderByOrderID.1字段!";
                    return null;
                }
                sql = sql + " " + string.Format(sql1, inpatientNo, orderID, beginDate.ToString(), endDate.ToString());
                addExecOrder(al, sql);
            }
            return al;
        }



        #endregion

        #region 医嘱重整  {FB86E7D8-A148-4147-B729-FD0348A3D670} 增加函数

        /// <summary>
        /// 医嘱重整，给一部分医嘱状态置4，表示该医嘱归档
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public int OrderReform(string OrderID)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Update.Reform", ref strSql) == -1)
            {
                return -1;
            }

            return this.ExecNoQuery(strSql, OrderID);
        }

        #endregion
        //{7ADC94B3-C691-4c55-89E9-09398DCAA498}
        #region 临时医嘱批量查询与录入
        /// <summary>
        /// 通过护士站查询患者的医嘱执行情况
        /// </summary>
        /// <param name="nurseCell"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="isInPuted">是否已录入执行情况</param>
        /// <returns></returns>
        public ArrayList QueryOrderExedInfoByNurseCell(string nurseCell, string patientNo, string fromDate, string toDate, bool isInPuted)
        {
            ArrayList al = new ArrayList();
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Order.ExecOrder.QueryExedInfoByNurseCell", ref sql) < 0)
            {
                this.Err = "没有找到Order.ExecOrder.QueryExedInfoByNurseCell字段!";
                return null;
            }
            if (isInPuted)
            {
                sql = string.Format(sql, nurseCell, patientNo, fromDate, toDate, "1");
            }
            else
            {
                sql = string.Format(sql, nurseCell, patientNo, fromDate, toDate, "0");
            }

            if (this.ExecQuery(sql) == -1)
            {
                return null;
            }
            try
            {
                Neusoft.HISFC.Models.Order.OrderBill objOrderBill;
                while (this.Reader.Read())
                {
                    objOrderBill = new Neusoft.HISFC.Models.Order.OrderBill();
                    objOrderBill.Order.ID = this.Reader[2].ToString();
                    objOrderBill.Order.Patient.ID = this.Reader[0].ToString();
                    objOrderBill.Order.Patient.Name = this.Reader[1].ToString();
                    objOrderBill.Order.Name = this.Reader[3].ToString();
                    objOrderBill.Order.MOTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[7].ToString());
                    objOrderBill.Order.ExecOper.ID = this.Reader[4].ToString();
                    objOrderBill.Order.ExecOper.Name = this.Reader[5].ToString();
                    objOrderBill.Order.ExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6].ToString());
                    objOrderBill.Order.Status = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[8].ToString());
                    al.Add(objOrderBill);
                }
            }
            catch (Exception e)
            {
                this.Err = "实体赋值出错" + e.Message;
                return null;
            }
            this.Reader.Close();
            return al;




        }
        /// <summary>
        /// 更新临时医嘱单的打印表的执行护士和执行时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ord"></param>
        /// <returns></returns>
        public int UpdatePrnOrder(string id, Neusoft.HISFC.Models.Order.Order ord)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Order.OrderPrn.Update", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                sql = string.Format(sql, id, ((Neusoft.FrameWork.Models.NeuObject)(ord)).ID, Neusoft.FrameWork.Function.NConvert.ToDateTime(ord.ExtendFlag1).ToString(), ord.ExtendFlag2);
            }
            catch
            {
                this.Err = "传入参数不对！Order.OrderPrn.Update";
                this.WriteErr();
                return -1;
            }

            return this.ExecNoQuery(sql);
        }
        #endregion

        /// <summary>
        /// 翻译皮试信息
        /// </summary>
        /// <param name="i"></param>
        /// <returns>0 (不需要皮试);1 [免试]; 2 [需皮试]; 3 [+]; 4 [-];</returns>
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
        /// 费用处方号发生变更时同步更新医嘱执行档内相应处方号
        /// </summary>
        /// <param name="execOrderID">医嘱执行档流水号</param>
        /// <param name="isPharmacy">是否药品</param>
        /// <param name="newRecipeNo">变更后新处方号</param>
        /// <returns>成功返回 更新 条数 失败返回-1 无相应记录返回 0</returns>
        public int UpdateExecRecipeNo(string execOrderID, bool isPharmacy, string newRecipeNo)
        {
            string strSql = "";
            string strIndex = "";
            if (isPharmacy)			//药品执行档
                strIndex = "Order.Update.UpdateExecRecipeNo.1";
            else					//非药品执行档
                strIndex = "Order.Update.UpdateExecRecipeNo.2";
            if (this.Sql.GetCommonSql(strIndex, ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, execOrderID, newRecipeNo);
            }
            catch (Exception ex)
            {
                this.Err = "传入参数不对!" + strIndex + ex.Message;
                return -1;
            }
            return ExecNoQuery(strSql);
        }

        /// <summary>
        /// 查询页码 行号 提取标志 是否未更新成功
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int CheckPageRowNoAndGetFlag(string patientID, string type)
        {
            string strSql = "";
            int count = 0;
            if (this.Sql.GetCommonSql("Order.Order.CheckPageRowNoAndGetFlag", ref strSql) == -1)
            {
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, patientID, type);
            }
            catch (Exception ex)
            {
                this.Err = "传入参数不对!";
                return -1;
            }

            try
            {
                count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(strSql));
            }
            catch (Exception ex)
            {
                this.Err = "获取页码信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return -1;
            }
            this.Reader.Close();
            return count;
        }


        /// <summary>
        /// 护士站医嘱确认收费
        /// </summary>
        /// <param name="patientID"></param>
        /// <param name="orderType"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int ExecChargeConfirm(string patientID, string orderType, ref DataSet ds)
        {
            string strSql = null;
            string decmpsState = null;

            if (this.Sql.GetCommonSql("Nurse.Order.ExecChargeComfirm.Query", ref strSql) == -1)
            {
                this.Err = "未找到 Nurse.Order.ExecChargeComfirm.Query 语句。";
                return -1;
            }

            if (orderType.ToUpper().Equals("LONG"))
            {
                decmpsState = "1";
            }
            else if (orderType.ToUpper().Equals("SHORT"))
            {
                decmpsState = "0";
            }

            try
            {
                strSql = string.Format(strSql, patientID, decmpsState);
            }
            catch (Exception ex)
            {
                this.Err = "传入参数有误。";
                return -1;
            }

            return this.ExecQuery(strSql, ref ds);
        }


        /// <summary>
        /// 获取自费药品目录
        /// </summary>
        /// <returns></returns>
        public List<Neusoft.FrameWork.Models.NeuObject> GetOwnFeeDrugs()
        {
            //Order.CheckOwnFeeDrug.Query
            string strSql = "";
            //int count = 0;
            List<Neusoft.FrameWork.Models.NeuObject> list = new List<NeuObject>();
            if (this.Sql.GetCommonSql("Order.CheckOwnFeeDrug.Query", ref strSql) == -1)
            {
                return list;
            }
            NeuObject obj = new NeuObject();
            try
            {
                if (this.ExecQuery(strSql) == -1)
                    return list;
                else
                {
                    while (this.Reader.Read())
                    {
                        obj.ID = this.Reader[0].ToString();
                        obj.Name = this.Reader[1].ToString();
                        obj.User01 = this.Reader[2].ToString();
                        obj.User02 = this.Reader[3].ToString();
                        list.Add(obj);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                this.Err = "获取页码信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return list;
            }
            this.Reader.Close();
            return list;
        }

        /// <summary>
        /// 获取患者危急药物
        /// 护士查询医嘱时候在名称前加【危】
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryPatientCriticalDrugs(string inpatientNO)
        {
            //Order.QueryPatientCriticalDrugs.Query
            string strSql = "";
            //int count = 0;
            ArrayList al = new ArrayList();
            if (this.Sql.GetCommonSql("Order.QueryPatientCriticalDrugs.Query", ref strSql) == -1)
            {
                this.Err = "没有找到Order.QueryPatientCriticalDrugs.Query字段!";
                return null;
            }
            strSql = string.Format(strSql, inpatientNO);
            if (this.ExecQuery(strSql) == -1)
            {
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    NeuObject obj = new NeuObject();
                    obj.ID = this.Reader[0].ToString();
                    obj.Name = this.Reader[1].ToString();
                    al.Add(obj);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得医嘱信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();
            return al;
        }


        /// <summary>
        /// 获取患者危急药物
        /// 护士查询医嘱时候在名称前加【危】
        /// </summary>
        /// <returns></returns>
        public List<NeuObject> QueryPatientCriticalDrugsaList(string inpatientNO)
        {
            //Order.QueryPatientCriticalDrugs.Query
            string strSql = "";
            //int count = 0;
            List<NeuObject> lists = new List<NeuObject>();
            if (this.Sql.GetCommonSql("Order.QueryPatientCriticalDrugs.Query", ref strSql) == -1)
            {
                this.Err = "没有找到Order.QueryPatientCriticalDrugs.Query字段!";
                return null;
            }
            strSql = string.Format(strSql, inpatientNO);
            if (this.ExecQuery(strSql) == -1)
            {
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    NeuObject obj = new NeuObject();
                    obj.ID = this.Reader[0].ToString();
                    obj.Name = this.Reader[1].ToString();
                    lists.Add(obj);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得医嘱信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();
            return lists;
        }

        /// <summary>
        /// 获取患者危急药物
        /// 护士查询医嘱时候在名称前加【危】allan
        /// </summary>
        /// <returns></returns>
        public List<NeuObject> QueryPatientCriticalDrugsLists(string inpatientNO)
        {
            //Order.QueryPatientCriticalDrugs.Query
            string strSql = "";
            //int count = 0;
            List<NeuObject> al = new List<NeuObject>();
            if (this.Sql.GetCommonSql("Order.QueryPatientCriticalDrugs.Query", ref strSql) == -1)
            {
                this.Err = "没有找到Order.QueryPatientCriticalDrugs.Query字段!";
                return null;
            }
            strSql = string.Format(strSql, inpatientNO);
            if (this.ExecQuery(strSql) == -1)
            {
                return null;
            }
            try
            {
                while (this.Reader.Read())
                {
                    NeuObject obj = new NeuObject();
                    obj.ID = this.Reader[0].ToString();
                    obj.Name = this.Reader[1].ToString();
                    al.Add(obj);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得医嘱信息出错！" + ex.Message;
                this.ErrCode = "-1";
                this.WriteErr();
                return null;
            }
            this.Reader.Close();
            return al;
        }

        /// <summary>
        /// 查询出院带药有效条数【单次住院，出院带药不能超过五个药品】
        /// </summary>
        /// <param name="inpNo"></param>
        /// <returns></returns>
        public int QueryCdOrderCnt(string inpNo)
        {
            string sql = @"select count(*)
                  from met_ipm_order a
                 where a.inpatient_no = '{0}'
                   --and a.class_code<> 'U'
                   and a.class_code in ('P','PCZ')
                   and a.mo_stat <> '3'
                   and a.type_code = 'CD'";
            sql = string.Format(sql, inpNo);
            return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
        }

        /// <summary>
        /// 查询出院带药有效条数【单次住院，出院带药不能超过五个药品】
        /// </summary>
        /// <param name="inpNo"></param>
        /// <returns></returns>
        public string QueryCdOrderID(string inpNo)
        {
            string sql = @"select to_char(wm_concat(a.mo_order))
                              from met_ipm_order a
                             where a.inpatient_no = '{0}'
                               and a.class_code in ('P','PCZ','PCC')
                               and a.mo_stat <> '3'
                               and a.type_code = 'CD'";
            sql = string.Format(sql, inpNo);
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 开启CA自动签名
        /// [Guid("139912F4-93DB-429F-8574-3C7264AF4C33")]
        /// </summary>
        /// <param name="operid"></param>
        /// <param name="autoSignToken"></param>
        /// <param name="signTime"></param>
        /// <returns></returns>
        public int OpenUserAutoSign(string employeeID, string autoSignToken, DateTime signTime, int CAValidTime)
        {
            string strSql = @"INSERT INTO COSS_USER_AUTOSIGN
                              (EMPCODE, SIGNTOKEN, OPERTIME, VAIDSEC, ISVALID)
                            VALUES
                              ('{0}', '{1}', TO_DATE('{2}', 'yyyy-MM-dd hh24:mi:ss'), {3}, '1')";
            //if (this.Sql.GetCommonSql("Order.Order.CreateUserSign.1", ref strSql) == -1)
            //{
            //    this.Err = this.Sql.Err;
            //    return -1;
            //}
            strSql = string.Format(strSql, employeeID, autoSignToken, signTime, CAValidTime);
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 查询有效时间内的CA签名秘钥
        /// [Guid("139912F4-93DB-429F-8574-3C7264AF4C33")]
        /// </summary>
        /// <param name="operid"></param>
        /// <returns></returns>
        public string QueryValidSignToken(string employeeID)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.QueryUserSign.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = string.Format(strSql, employeeID);

            return this.ExecSqlReturnOne(strSql);
        }

        /// <summary>
        /// 关闭的时候设置为无效
        /// [Guid("139912F4-93DB-429F-8574-3C7264AF4C33")]
        /// </summary>
        /// <param name="operid"></param>
        /// <param name="autoSignToken"></param>
        /// <returns></returns>
        public int CloseUserAutoSign(string employeeID, string autoSignToken)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Order.UpdateUserSign.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = string.Format(strSql, employeeID, autoSignToken);
            if (strSql == null)
            {
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 根据传入信息获取消息
        /// </summary>
        /// <param name="ItemID">项目ID</param>
        /// <param name="MesType">消息类型</param>
        /// <returns>消息列表</returns>
        public List<string> GetItemMessage(string ItemID, string MesType)
        {
            List<string> MessageList = new List<string>();
            string strSql = "select CM.Message from COM_ORDERMESSAGE CM where CM.ITEMID = '{0}' and CM.Messagetype = '{1}'";
            strSql = string.Format(strSql, ItemID, MesType);
            DataSet ds = null;
            this.ExecQuery(strSql, ref ds);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                        MessageList.Add(row[0].ToString());
                }
            }
            return MessageList;
        }

        public string GetMedListCodg(string itemCode, decimal price, DateTime birthday)
        {
            var isUnderSixYearsOld = (birthday > DateTime.MinValue) && (birthday.AddYears(6) >= DateTime.Now.Date);

            var ds = new DataSet();
            var queryUndrugInfoSql = string.Format(@" select p.item_code,p.gb_code,p.sixyearsoldcountrycode,p.unit_price,p.unit_price1,p.unitflag,p.mark5 from fin_com_undruginfo p where p.item_code='{0}' ", itemCode);
            var queryUndrugInfoResult = this.ExecQuery(queryUndrugInfoSql, ref ds);

            var queryCenterCodeSql = string.Format(@" select p.center_code from fin_com_compare p where p.pact_code='16' and p.his_code='{0}' ", itemCode);

            //药品直接取对照表中的国家医保编码
            if (ds == null || ds.Tables == null || ds.Tables[0].Rows.Count <= 0)
            {
                return this.ExecSqlReturnOne(queryCenterCodeSql, "");
            }

            var row = ds.Tables[0].Rows[0];
            var gbCode = row["gb_code"].ToString();
            var sixyearsoldcountrycode = row["sixyearsoldcountrycode"].ToString();
            var unit_price = (decimal)row["unit_price"];
            var unit_price1 = (decimal)row["unit_price1"];
            var unitflag = row["unitflag"].ToString();
            var mark5 = row["mark5"].ToString();

            //组套项目没有国家医保编码 直接返回空
            if (unitflag == "1")
            {
                return "";
            }

            //如果是耗材项目，则取gbcode，耗材是由herp那边通过接口把国家医保编码同步到gbcode字段
            var centerCode = this.ExecSqlReturnOne(queryCenterCodeSql, "");
            if (string.IsNullOrEmpty(centerCode))
            {
                return gbCode;
            }

            if (isUnderSixYearsOld && (!string.IsNullOrEmpty(gbCode) && (gbCode.Substring(0, 1).ToString() == "3")) || mark5 == "1")
            {
                if (price == unit_price1 && price == unit_price * 1.3M)
                {
                    return sixyearsoldcountrycode;

                }
                else if (price == unit_price1 && price == unit_price)
                {
                  
                    return centerCode;

                }
                else if (price != unit_price1 && price == unit_price * 1.3M && unit_price1 == unit_price)
                {
                   
                    return sixyearsoldcountrycode;

                }
                else if (price != unit_price1 && price == unit_price)
                {                   
                    return centerCode;
                }
                else
                {
                    return "";
                }
            }


            return centerCode;


        }

    }
}
