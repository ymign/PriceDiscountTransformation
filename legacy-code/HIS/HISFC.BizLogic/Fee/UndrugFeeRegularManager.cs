using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using Neusoft.HISFC.Models.Fee.Item;
using Neusoft.FrameWork.Function;

namespace Neusoft.HISFC.BizLogic.Fee
{
    public class UndrugFeeRegularManager : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 东莞非药品收费规则管理类
        /// </summary>
        public UndrugFeeRegularManager()
        {
        }

        /// <summary>
        /// 查找单个非药品收费规则信息
        /// </summary>
        /// <param name="itemcode"></param>
        /// <returns></returns>
        public ArrayList GetSingleFeeRegular(string itemcode)
        {
            // string sqlIndex = "DongGuan.InpatientFee.UndrugFeeRegular.GetByItemCode";

            string querySql = "";

            ArrayList feeRegularList = new ArrayList();

            if (this.Sql.GetCommonSql("DongGuan.InpatientFee.UndrugFeeRegular.GetByItemCode", ref querySql) == -1)
            {
                this.Err = "获取Sql语句出错,索引号:DongGuan.InpatientFee.UndrugFeeRegular.GetByItemCode";
                WriteErr();
                return null;
            }
            try
            {
                querySql = string.Format(querySql, itemcode);
            }
            catch (Exception ex)
            {
                this.Err = "格式化Sql语句出错,错误:" + ex.Message;
                WriteErr();
                return null;
            }
            try
            {
                if (this.ExecQuery(querySql) == -1)
                {
                    this.Err = "执行Sql语句出错";
                    return null;
                }
                while (this.Reader.Read())
                {
                    UndrugFeeRegular undrugFeeRegular = new UndrugFeeRegular();
                    undrugFeeRegular.ID = this.Reader[0].ToString();
                    undrugFeeRegular.Item.ID = this.Reader[1].ToString();
                    undrugFeeRegular.Item.Name  = this.Reader[2].ToString();
                    undrugFeeRegular.LimitCondition = this.Reader[3].ToString();
                    undrugFeeRegular.Regular.ID = this.Reader[4].ToString();
                    undrugFeeRegular.DayLimit = NConvert.ToDecimal(this.Reader[5].ToString());
                    undrugFeeRegular.LimitItem.ID = this.Reader[6].ToString();
                    undrugFeeRegular.Oper.ID = this.Reader[7].ToString();
                    undrugFeeRegular.Oper.OperTime = NConvert.ToDateTime(this.Reader[8].ToString());
                    undrugFeeRegular.IsOutFee = NConvert.ToBoolean(this.Reader[9].ToString());
                    feeRegularList.Add(undrugFeeRegular);
                }

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();
                return null;
            }
            return feeRegularList;
        }

        /// <summary>
        /// 更新非药品收费规则
        /// </summary>
        /// <param name="itemcode"></param>
        /// <returns></returns>
        public int UpdateUndrugFeeRegular(UndrugFeeRegular undrugFeeRegular)
        {
            if (undrugFeeRegular == null)
            {
                this.Err = "无法更新无效信息,请确认!";
                return 0;
            }

            string updateSql = "";

            if (this.Sql.GetCommonSql("DongGuan.InpatientFee.UndrugFeeRegular.Update", ref updateSql) == -1)
            {
                this.Err = "获取更新Sql出错,索引号:DongGuan.InpatientFee.UndrugFeeRegular.Update";
                return -1;
            }
            try
            {
                updateSql = string.Format(updateSql, Int32.Parse(undrugFeeRegular.ID), undrugFeeRegular.Item.ID.ToString(), undrugFeeRegular.Item.Name .ToString(), undrugFeeRegular.LimitCondition.ToString(), undrugFeeRegular.Regular.ID.ToString(), NConvert.ToInt32(undrugFeeRegular.DayLimit.ToString()), undrugFeeRegular.LimitItem.ID.ToString(), undrugFeeRegular.Oper.ID.ToString(), undrugFeeRegular.Oper.OperTime.ToString("yyyy-MM-dd HH:mm:ss"),NConvert.ToInt32(undrugFeeRegular.IsOutFee));

                if (this.ExecNoQuery(updateSql) == -1)
                {
                    this.Err = "更新收费规则时出错,对应项目名称:" + undrugFeeRegular.Item.Name .ToString();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 插入非药品收费规则信息
        /// </summary>
        /// <param name="undrugFeeRegular"></param>
        /// <returns></returns>
        public int InsertUndrugFeeRegular(UndrugFeeRegular undrugFeeRegular)
        {
            if (undrugFeeRegular == null)
            {
                this.Err = "无法插入无效信息,请确认";
                return 0;
            }

            string insertSql = "";

            if (this.Sql.GetCommonSql("DongGuan.InpatientFee.UndrugFeeRegular.Insert", ref insertSql) == -1)
            {
                this.Err = "获取更新Sql出错,索引号:DongGuan.InpatientFee.UndrugFeeRegular.Insert";
                return -1;
            }

            try
            {
                insertSql = string.Format(insertSql, NConvert.ToInt32(undrugFeeRegular.ID), undrugFeeRegular.Item.ID.ToString(), undrugFeeRegular.Item.Name.ToString(), undrugFeeRegular.LimitCondition.ToString(), undrugFeeRegular.Regular.ID.ToString(), NConvert.ToInt32(undrugFeeRegular.DayLimit.ToString()), undrugFeeRegular.LimitItem.ID.ToString(), undrugFeeRegular.Oper.ID.ToString(), undrugFeeRegular.Oper.OperTime.ToString("yyyy-MM-dd HH:mm:ss"), NConvert.ToInt32(undrugFeeRegular.IsOutFee));

                if (this.ExecNoQuery(insertSql) == -1)
                {
                    this.Err = "插入收费规则时出错,对应项目名称:" + undrugFeeRegular.Item.Name .ToString();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// 删除非药品收费规则
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public int DeleteUndrugFeeRegular(string ruleId)
        {
            if (string.IsNullOrEmpty(ruleId))
            {
                this.Err = "未找到对应项目信息，请重试！";
                return -1;
            }
            string deleteSql = "";
            if (this.Sql.GetCommonSql("DongGuan.InpatientFee.UndrugFeeRegular.Delete", ref deleteSql) == -1)
            {
                this.Err = "获取删除Sql出错,索引号:DongGuan.InpatientFee.UndrugFeeRegular.Delete";
                return -1;
            }
            try
            {
                deleteSql = string.Format(deleteSql, ruleId);
                if (this.ExecNoQuery(deleteSql) == -1)
                {
                    this.Err = "删除收费规则时出错，对应规则编码:" + ruleId;
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 获取所有非药品收费规则列表
        /// </summary>
        /// <param name="itemcode"></param>
        /// <returns></returns>
        public ArrayList GetFeeRegularByItemCode(string itemcode)
        {
            return GetSingleFeeRegular(itemcode);
        }

        /// <summary>
        /// 获得按规则收费相关项目编码
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="relateItems"></param>
        /// <returns></returns>
        public int GetRelateItems(string itemCode, ref string relateItems)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("DongGuan.InpatientFee.QueryRelateItems", ref sql) < 0)
            {
                this.Err = "查找索引为DongGuan.InpatientFee.QueryRelateItems的SQL语句失败！";
                return -1;
            }
            sql = string.Format(sql, itemCode);

            relateItems = this.ExecSqlReturnOne(sql);
            return 1;
        }

        /// <summary>
        /// 查询所有非药品收费规则
        /// </summary>
        /// <returns></returns>
        public DataSet GetAlFeeRegular()
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("DongGuan.InpatientFee.QueryAllFeeRegular", ref sql) < 0)
            {
                this.Err = "查询索引为DongGuan.InpatientFee.QueryAllFeeRegular的SQL语句失败！";
                return null;
            }
            DataSet ds = new DataSet();
            if (this.ExecQuery(sql, ref ds) < 0)
            {
                this.Err = "查询非药品收费规则失败！";
                return null;
            }
            return ds;
        }

        public ArrayList QueryAllFeeRegular()
        {
            string querySql = "";

            ArrayList feeRegularList = new ArrayList();

            if (this.Sql.GetCommonSql("DongGuan.InpatientFee.QueryAllFeeRegular", ref querySql) == -1)
            {
                this.Err = "获取Sql语句出错,索引号:DongGuan.InpatientFee.QueryAllFeeRegular";
                WriteErr();
                return null;
            }
            try
            {
                querySql = string.Format(querySql);
            }
            catch (Exception ex)
            {
                this.Err = "格式化Sql语句出错,错误:" + ex.Message;
                WriteErr();
                return null;
            }
            try
            {
                if (this.ExecQuery(querySql) == -1)
                {
                    this.Err = "执行Sql语句出错";
                    return null;
                }
                while (this.Reader.Read())
                {
                    UndrugFeeRegular undrugFeeRegular = new UndrugFeeRegular();
                    undrugFeeRegular.ID = this.Reader[0].ToString();
                    undrugFeeRegular.Item.ID = this.Reader[1].ToString();
                    undrugFeeRegular.Item.Name = this.Reader[2].ToString();
                    undrugFeeRegular.LimitCondition = this.Reader[3].ToString();
                    undrugFeeRegular.Regular.ID = this.Reader[4].ToString();
                    undrugFeeRegular.DayLimit = NConvert.ToDecimal(this.Reader[5].ToString());
                    undrugFeeRegular.LimitItem.ID = this.Reader[6].ToString();
                    undrugFeeRegular.Oper.ID = this.Reader[7].ToString();
                    undrugFeeRegular.Oper.OperTime = NConvert.ToDateTime(this.Reader[8].ToString());
                    undrugFeeRegular.IsOutFee = NConvert.ToBoolean(this.Reader[9].ToString());
                    feeRegularList.Add(undrugFeeRegular);
                }

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();
                return null;
            }
            return feeRegularList;
        }

        /// <summary>
        /// 通过规则编码获取收费规则实体
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public UndrugFeeRegular GetSingleFeeRegularById(string ruleId)
        {
            UndrugFeeRegular feeRegular = new UndrugFeeRegular();
            string querySql = "";
            string sqlIndex = "DongGuan.InpatientFee.UndrugFeeRule.SelectByID";

            if (string.IsNullOrEmpty(ruleId))
            {
                this.Err = "传递的参数为空!";
                WriteErr();
                return null;
            }
            if (this.Sql.GetCommonSql(sqlIndex, ref querySql) == -1)
            {
                this.Err = "获取Sql语句失败,索引号:" + sqlIndex;
                WriteErr();
                return null;
            }

            try
            {
                querySql = string.Format(querySql, ruleId);

                if (this.ExecQuery(querySql) == -1)
                {
                    this.Err = "执行者Sql语句出错,";
                    WriteErr();
                    return null;
                }
                if (this.Reader.Read())
                {
                    feeRegular.ID = this.Reader[0].ToString();
                    feeRegular.Item.ID = this.Reader[1].ToString();
                    feeRegular.Item.Name  = this.Reader[2].ToString();
                    feeRegular.LimitCondition = this.Reader[3].ToString();
                    feeRegular.Regular.ID = this.Reader[4].ToString();
                    feeRegular.DayLimit = NConvert.ToDecimal(this.Reader[5].ToString());
                    feeRegular.LimitItem.ID = this.Reader[6].ToString();
                    feeRegular.Oper.ID = this.Reader[7].ToString();
                    feeRegular.Oper.OperTime = NConvert.ToDateTime(this.Reader[8].ToString());
                    feeRegular.IsOutFee = NConvert.ToBoolean(this.Reader[9].ToString());
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();
                return null;
            }

            return feeRegular;
        }

        /// <summary>
        /// 获取收费规则序号
        /// </summary>
        /// <returns></returns>
        public string GetFeeRegularSequence()
        {
            string querySql = "";

            try
            {
                if (this.Sql.GetCommonSql("DongGuan.InpatientFee.UndrugFeeRegular.GetRuleSeqence", ref querySql) == -1)
                {
                    this.Err = "获取收费规则序列号出错,未找到Sql索引:DongGuan.InpatientFee.UndrugFeeRegular.GetRuleSeqence";
                    WriteErr();
                    return "";
                }

                return this.ExecSqlReturnOne(querySql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                WriteErr();
                return "";
            }

        }


        /// <summary>
        /// 获得某个项目已收费数量（通用名收取）
        /// </summary>
        /// <param name="p"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public decimal GetFeeCountByExecOrder(Neusoft.HISFC.Models.RADT.Patient p, DateTime beginTime, DateTime endTime, string itemCode)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Order.Query.GetSumQtyByItemCode", ref sql) < 0)
            {
                this.Err = "查找索引为Order.Query.GetSumQtyByItemCode的SQL语句失败！";
                return -1;
            }
            sql = string.Format(sql, p.ID, beginTime.ToString(), endTime.ToString(), itemCode);

            return Neusoft.FrameWork.Function.NConvert.ToDecimal(this.ExecSqlReturnOne(sql));
        }


        /// <summary>
        /// 获取所有已执行为收费的医嘱
        /// </summary>
        /// <param name="p">患者信息</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public ArrayList GetPatientNoFeeExecOrder(Neusoft.HISFC.Models.RADT.Patient p, DateTime beginTime, DateTime endTime)
        {
            string sql = string.Empty;
            #region 废旧用新

            if (this.Sql.GetCommonSql("Order.Query.GetNOFeeExecOrder.New", ref sql) < 0)
            {
                this.Err = "查找索引为Order.Query.GetNOFeeExecOrder.New的SQL语句失败！";
                return null;
            }
            #endregion
            sql = string.Format(sql, p.ID, beginTime.ToString(), endTime.ToString());


            ArrayList al = new ArrayList();

            if (this.ExecQuery(sql) == -1) return null;
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Order.ExecOrder objOrder = new Neusoft.HISFC.Models.Order.ExecOrder();

                    #region "患者信息"
                    //患者信息——  
                    //			1 住院流水号   2住院病历号     3患者科室id      4患者护理id
                    try
                    {
                        objOrder.Order.Patient.ID = this.Reader[1].ToString();
                        objOrder.Order.Patient.PID.PatientNO = this.Reader[2].ToString();
                        objOrder.Order.Patient.PVisit.PatientLocation.Dept.ID = this.Reader[3].ToString();
                        objOrder.Order.Patient.PVisit.PatientLocation.NurseCell.ID = this.Reader[4].ToString();
                        objOrder.Order.NurseStation.ID = this.Reader[4].ToString();
                        objOrder.Order.InDept.ID = this.Reader[3].ToString();
                    }
                    catch (Exception ex)
                    {
                        this.Err = "获得患者基本信息出错！" + ex.Message;
                        this.WriteErr();
                        return null;
                    }
                    #endregion

                    if (this.Reader[5].ToString() == "1")//药品
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item objPharmacy = new Neusoft.HISFC.Models.Pharmacy.Item();
                        try
                        {
                            #region "项目信息"
                            //医嘱辅助信息
                            // ——项目信息
                            //	       5项目类别      6项目编码       7项目名称      8项目输入码,    9项目拼音码 
                            //	       10项目类别代码 11项目类别名称  12药品规格     13药品基本剂量  14剂量单位       
                            //         15最小单位     16包装数量,     17剂型代码     18药品类别  ,   19药品性质
                            //         20零售价       21用法代码      22用法名称     23用法英文缩写  24频次代码  
                            //         25频次名称     26每次剂量      27项目总量     28计价单位      29使用天数			
                            objPharmacy.ID = this.Reader[6].ToString();
                            objPharmacy.Name = this.Reader[7].ToString();
                            objPharmacy.UserCode = this.Reader[8].ToString();
                            objPharmacy.SpellCode = this.Reader[9].ToString();
                            objPharmacy.SysClass.ID = this.Reader[10].ToString();
                            //objPharmacy.SysClass.Name = this.Reader[11].ToString();
                            objPharmacy.Specs = this.Reader[12].ToString();
                            //try
                            //{
                            objPharmacy.BaseDose = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13]);
                            //}
                            //catch{}
                            objPharmacy.DoseUnit = this.Reader[14].ToString();
                            objPharmacy.MinUnit = this.Reader[15].ToString();
                            //try
                            //{
                            objPharmacy.PackQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[16]);
                            //}
                            //catch{}
                            objPharmacy.DosageForm.ID = this.Reader[17].ToString();
                            objPharmacy.Type.ID = this.Reader[18].ToString();
                            objPharmacy.Quality.ID = this.Reader[19].ToString();
                            //try
                            //{
                            objPharmacy.PriceCollection.RetailPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20]);
                            //}
                            //catch{}	
                            objOrder.Order.Item = objPharmacy;
                            #endregion

                            objOrder.Order.Usage.ID = this.Reader[21].ToString();
                            objOrder.Order.Usage.Name = this.Reader[22].ToString();
                            objOrder.Order.Usage.Memo = this.Reader[23].ToString();
                            objOrder.Order.Frequency.ID = this.Reader[24].ToString();
                            objOrder.Order.Frequency.Name = this.Reader[25].ToString();
                            //try
                            //{
                            objOrder.Order.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26]);
                            //}
                            //catch{}
                            objOrder.Order.DoseUnit = objPharmacy.DoseUnit;
                            //try
                            //{
                            objOrder.Order.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27]);
                            //}
                            //catch{}
                            objOrder.Order.Unit = this.Reader[28].ToString();
                            //try
                            //{
                            objOrder.Order.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[29]);
                            //}
                            //catch{}
                        }
                        catch (Exception ex)
                        {
                            this.Err = "获得医嘱项目信息出错！" + ex.Message;
                            this.WriteErr();
                            return null;
                        }
                        //objOrder.Order.Item = objPharmacy;

                        #region "医嘱属性"
                        // ——医嘱属性
                        //		  30医嘱流水号 , 31医嘱类别代码  32医嘱是否分解:1长期/2临时     33是否计费 
                        //		   34药房是否配药 35打印执行单    36是否需要确认  
                        try
                        {
                            objOrder.ID = this.Reader[0].ToString();
                            objOrder.Order.ID = this.Reader[30].ToString();
                            objOrder.Order.OrderType.ID = this.Reader[31].ToString();
                            //try
                            //{
                            objOrder.Order.OrderType.IsDecompose = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[32]);
                            //}
                            //catch{}
                            //try
                            //{
                            objOrder.Order.OrderType.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[33]);
                            //}
                            //catch{}
                            //try
                            //{
                            objOrder.Order.OrderType.IsNeedPharmacy = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[34]);
                            //}
                            //catch{}
                            //try
                            //{
                            objOrder.Order.OrderType.IsPrint = Neusoft.FrameWork.Function.NConvert.ToBoolean(Reader[35]);
                            //}
                            //catch{}
                            //try
                            //{
                            objOrder.Order.Item.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[36]);
                            //}
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
                        //		   37开立医师Id   38开立医师name  39要求执行时间  40作废时间     41开立科室
                        //		   42开立时间     43作废人代码    44记账人代码    45记账科室代码 46记账时间       
                        //		   47取药药房     48执行科室      49执行护士代码  50执行科室代码 51执行时间
                        //         52分解时间
                        try
                        {
                            objOrder.Order.ReciptDoctor.ID = this.Reader[37].ToString();
                            objOrder.Order.ReciptDoctor.Name = this.Reader[38].ToString();
                            //try{
                            objOrder.DateUse = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[39]);
                            //}
                            //catch{}
                            //try{
                            objOrder.DCExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[40]);
                            //}
                            //catch{}
                            objOrder.Order.ReciptDept.ID = this.Reader[41].ToString();
                            //try{
                            objOrder.Order.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[42]);
                            //}
                            //catch{}
                            objOrder.DCExecOper.ID = this.Reader[43].ToString();
                            objOrder.ChargeOper.ID = this.Reader[44].ToString();
                            objOrder.ChargeOper.Dept.ID = this.Reader[45].ToString();
                            //try{
                            objOrder.ChargeOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[46]);
                            //}
                            //catch{}
                            objOrder.Order.StockDept.ID = this.Reader[47].ToString();
                            objOrder.Order.ExeDept.ID = this.Reader[48].ToString();
                            objOrder.ExecOper.ID = this.Reader[49].ToString();
                            //try
                            //{
                            if (this.Reader[50].ToString() != "")
                                objOrder.Order.ExeDept.ID = this.Reader[50].ToString();
                            //}
                            //catch{}
                            //try{
                            objOrder.ExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[51]);
                            //}
                            //catch{}
                            objOrder.DateDeco = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[52]);

                            if (!Reader.IsDBNull(68))
                            {
                                objOrder.DrugedTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[68].ToString());
                            }

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
                        //		   64是否婴儿（1是/2否）          53发生序号  	  54组合序号     55主药标记 
                        //		   56是否包含附材                 57是否有效      58扣库标记     59是否执行 
                        //		   60配药标记                     61收费标记      62医嘱说明     63备注
                        //         65处方号                       66处方序号
                        try
                        {
                            //try{
                            objOrder.Order.IsBaby = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[53]);
                            //}
                            //catch{}
                            //try{
                            objOrder.Order.BabyNO = this.Reader[54].ToString();
                            //}
                            //catch{}
                            objOrder.Order.Combo.ID = this.Reader[55].ToString();
                            //try{
                            objOrder.Order.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[56]);
                            //}
                            //catch{}
                            //try{
                            objOrder.Order.IsHaveSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[57]);
                            //}
                            //catch{}
                            //try{
                            objOrder.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[58]);
                            //}
                            //catch{}
                            //try{
                            objOrder.Order.IsStock = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[59]);
                            //}
                            //catch{}
                            //try{
                            objOrder.IsExec = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[60]);
                            //}
                            //catch{}
                            //try{
                            objOrder.DrugFlag = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[61]);
                            //}
                            //catch{}
                            //try{
                            objOrder.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[62]);
                            //}
                            //catch{}
                            objOrder.Order.Note = this.Reader[63].ToString();
                            objOrder.Order.Memo = this.Reader[64].ToString();
                            objOrder.Order.ReciptNO = this.Reader[65].ToString();
                            //try{
                            objOrder.Order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[66]);
                            //}
                            //catch{}
                        }
                        catch (Exception ex)
                        {
                            this.Err = "获得医嘱类型信息出错！" + ex.Message;
                            this.WriteErr();
                            return null;
                        }
                        #endregion
                    }
                    else if (this.Reader[5].ToString() == "2")//非药品
                    {
                        Neusoft.HISFC.Models.Fee.Item.Undrug objAssets = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        try
                        {
                            #region "项目信息"
                            // ——项目信息
                            //	       5项目类别      6项目编码       7项目名称      8项目输入码,    9项目拼音码 
                            //	       10项目类别代码 11项目类别名称  12规格         13零售价        14用法代码   
                            //         15用法名称     16用法英文缩写  17频次代码     18频次名称      19每次用量
                            //         20项目总量     21计价单位      22使用次数	
                            objAssets.ID = this.Reader[6].ToString();
                            objAssets.Name = this.Reader[7].ToString();
                            objAssets.UserCode = this.Reader[8].ToString();
                            objAssets.SpellCode = this.Reader[9].ToString();
                            objAssets.SysClass.ID = this.Reader[10].ToString();
                            //objAssets.SysClass.Name = this.Reader[11].ToString();
                            objAssets.Specs = this.Reader[12].ToString();
                            //try
                            //{
                            objAssets.Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());
                            //}
                            //catch{}	
                            objAssets.PriceUnit = this.Reader[21].ToString();
                            objOrder.Order.Item = objAssets;
                            #endregion

                            objOrder.Order.Usage.ID = this.Reader[14].ToString();
                            objOrder.Order.Usage.Name = this.Reader[15].ToString();
                            objOrder.Order.Usage.Memo = this.Reader[16].ToString();
                            objOrder.Order.Frequency.ID = this.Reader[17].ToString();
                            objOrder.Order.Frequency.Name = this.Reader[18].ToString();
                            //try
                            //{
                            objOrder.Order.DoseOnce = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[19]);
                            //}
                            //catch{}
                            //try
                            //{
                            objOrder.Order.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[20]);
                            //}
                            //catch{}
                            objOrder.Order.Unit = this.Reader[21].ToString();
                            objOrder.Order.DoseUnit = objOrder.Order.Unit;
                            //try
                            //{
                            objOrder.Order.HerbalQty = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[22]);
                            //}
                            //catch{}
                        }
                        catch (Exception ex)
                        {
                            this.Err = "获得医嘱项目信息出错！" + ex.Message;
                            this.WriteErr();
                            return null;
                        }

                        #region "医嘱属性"
                        // ——医嘱属性
                        //		   23医嘱类别代码 24医嘱流水号    25医嘱是否分解:1长期/2临时     26是否计费 
                        //		   27药房是否配药 28打印执行单    29是否需要确认    
                        try
                        {
                            objOrder.ID = this.Reader[0].ToString();
                            objOrder.Order.OrderType.ID = this.Reader[23].ToString();
                            objOrder.Order.ID = this.Reader[24].ToString();
                            //							try
                            //							{
                            objOrder.Order.OrderType.IsDecompose = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[25]);
                            //}
                            //catch{}
                            //try
                            //{
                            objOrder.Order.OrderType.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[26]);
                            //}
                            //catch{}
                            //try
                            //{
                            objOrder.Order.OrderType.IsNeedPharmacy = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[27]);
                            //}
                            //catch{}
                            //try
                            //{
                            objOrder.Order.OrderType.IsPrint = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[28]);
                            //}
                            //catch{}
                            //try
                            //{
                            objOrder.Order.Item.IsNeedConfirm = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[29]);
                            //}
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
                        //		   30开立医师Id   31开立医师name  32要求执行时间  33作废时间     34开立科室
                        //		   35开立时间     36作废人代码    37记账人代码    38记账科室代码 39记账时间       
                        //		   40取药药房     41执行科室      42执行护士代码  43执行科室代码 44执行时间
                        //         45分解时间     46执行科室名称
                        try
                        {
                            objOrder.Order.ReciptDoctor.ID = this.Reader[30].ToString();
                            objOrder.Order.ReciptDoctor.Name = this.Reader[31].ToString();
                            //try{
                            objOrder.DateUse = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[32]);
                            //}
                            //catch{}
                            //try{
                            objOrder.DCExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[33]);
                            //}
                            //catch{}
                            objOrder.Order.ReciptDept.ID = this.Reader[34].ToString();
                            //try{
                            objOrder.Order.BeginTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[35]);
                            //}
                            //catch{}
                            objOrder.DCExecOper.ID = this.Reader[36].ToString();
                            objOrder.ChargeOper.ID = this.Reader[37].ToString();
                            objOrder.ChargeOper.Dept.ID = this.Reader[38].ToString();
                            //try{
                            objOrder.ChargeOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[39]);
                            //}
                            //catch{}
                            objOrder.Order.StockDept.ID = this.Reader[40].ToString();
                            objOrder.Order.ExeDept.ID = this.Reader[41].ToString();//执行科室用项目执行科室
                            objOrder.ExecOper.ID = this.Reader[42].ToString();
                            //objOrder.ExeDept.ID = this.Reader[43].ToString();//这个字段就是没用的
                            //try{
                            objOrder.ExecOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[44]);
                            //}
                            //catch{}
                            objOrder.DateDeco = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[45]);
                            objOrder.Order.ExeDept.Name = this.Reader[46].ToString();

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
                        //		   47是否婴儿（1是/2否）          48发生序号  	  49组合序号     50主项标记 
                        //		   51是否附材                     52是否包含附材  53是否有效     54是否执行 
                        //		   55收费标记     56加急标记      57检查部位检体  58医嘱说明     59备注 
                        //         60处方号                       61处方序号      62配药科室ID
                        try
                        {
                            //try{
                            objOrder.Order.IsBaby = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[47]);
                            //}
                            //catch{}
                            //try{
                            objOrder.Order.BabyNO = this.Reader[48].ToString();
                            //}
                            //catch{}
                            objOrder.Order.Combo.ID = this.Reader[49].ToString();
                            //try{
                            objOrder.Order.Combo.IsMainDrug = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[50]);
                            //}
                            //catch{}
                            //try{
                            objOrder.Order.IsSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[51]);
                            //}
                            //catch{}
                            //try{
                            objOrder.Order.IsHaveSubtbl = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[52]);
                            //}
                            //catch{}
                            //try{
                            objOrder.IsValid = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[53]);
                            //}
                            //catch{}
                            //try{
                            objOrder.IsExec = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[54]);
                            //}
                            //catch{}
                            //try{
                            objOrder.IsCharge = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[55]);
                            //}
                            //catch{}
                            //try{
                            objOrder.Order.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[56]);
                            //}
                            //catch{}
                            objOrder.Order.CheckPartRecord = this.Reader[57].ToString();

                            objOrder.Order.Note = this.Reader[58].ToString();
                            objOrder.Order.Memo = this.Reader[59].ToString();
                            objOrder.Order.ReciptNO = this.Reader[60].ToString();
                            //try{
                            objOrder.Order.SequenceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[61]);
                            //}
                            //catch{}
                            objOrder.Order.StockDept.ID = this.Reader[62].ToString();

                            try
                            {
                                objOrder.Order.Sample.Name = this.Reader[63].ToString();			//样本类型
                                objOrder.Order.Sample.Memo = this.Reader[64].ToString();			//检验条码号
                            }
                            catch { }



                        }
                        catch (Exception ex)
                        {
                            this.Err = "获得医嘱类型信息出错！" + ex.Message;
                            this.WriteErr();
                            return null;
                        }
                        #endregion
                    }
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


        /// <summary>
        /// 收费记录
        /// 更新执行医嘱收费人，收费标记，发票号等
        /// 对费用开放使用
        /// </summary>
        /// <param name="execOrder">执行档信息</param>
        /// <returns>0 success -1 fail</returns>
        public int UpdateChargeExecNew(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, Neusoft.HISFC.Models.Order.ExecOrder execOrder)
        {

            string strSql = "", strSqlName = "Order.ExecOrder.ChargeNew.";
            string strItemType = "";

            strItemType = JudgeItemType(execOrder.Order);
            if (strItemType == "")
            {
                return -1;
            }
            strSqlName = strSqlName + strItemType;

            if (this.Sql.GetCommonSql(strSqlName, ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                strSql = string.Format(strSql, patientInfo.ID,
                    execOrder.ChargeOper.ID, execOrder.ChargeOper.Dept.ID, execOrder.ChargeOper.OperTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(execOrder.IsCharge).ToString(),
                    execOrder.Order.ReciptNO, execOrder.Order.SequenceNO);
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
        /// 判断医嘱项目类别，药品/非药品
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        private string JudgeItemType(Neusoft.HISFC.Models.Order.Order Order)
        {
            string strItem = "";
            //判断药品/非药品 
            //if (Order.Item.IsPharmacy)
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
    }
}
