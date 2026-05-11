using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.RADT;
using Neusoft.HISFC.Models.Order;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Fee.Inpatient;

namespace Neusoft.HISFC.BizProcess.Integrate.FeeInterface
{
    /// <summary>
    /// [功能描述: 后台规则收费 ]<br></br>
    /// [创 建 者: 不清楚]<br></br>
    /// [整合人:maokb]<br></br>
    /// 
    /// <说明>
    /// </说明>
    /// </summary>
    public class InpatientRuleFee : IntegrateBase
    {
        #region 说明

        /*处理过程
         * 1、查询所有已执行未收费的医嘱（单价大于0），按照方号、应执行日期分组排序
         * 2、循环医嘱信息放到hsAllExecOrder中，用项目编码+应执行日期作为键值（之前以项目编码作为键值，只能满足判断一天收费）
         * 3、排除出院当天不收费项目，并且判断半小时收费，如果超过半小时多收一个 
         * 4、按照规则处理应收数量
         * 5、收费后更新所有判断的医嘱信息（包括收费的和判断不收费的）
         * 
         * 
         * 注意：
         * 1、每次收费判断从患者入院到结束时间内未收费的项目，防止分解不及时导致漏收费 
         * 2、收费采取后开立先收费、收取高费用的原则
         * 3、这里只对医嘱做了判断，查询所有的执行档  
         * 4、获取收费规则按照FEE_TYPE规则类型排序，后面收费受此影响
         *      02：每天不超过限定数量
         *      03：
         * 
         * */

        #endregion

        #region  变量
        /// <summary>
        /// 收费规则
        /// </summary>
        DataSet dsFeeRule = null;

        /// <summary>
        /// 费用信息
        /// </summary>
        ArrayList alFee = new ArrayList();

        /// <summary>
        /// 退费
        /// </summary>
        ArrayList alQuitFee = new ArrayList();

        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime beginTime;

        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime endTime;

        /// <summary>
        /// 患者信息
        /// </summary>
        PatientInfo patientInfo = null;

        /// <summary>
        /// 所有的医嘱信息
        /// </summary>
        Dictionary<string, ArrayList> hsAllExecOrder = new Dictionary<string, ArrayList>();

        /// <summary>
        /// 所有的医嘱信息（更新医嘱收费标记专用）
        /// </summary>
        Dictionary<string, ArrayList> hsAllExecOrderNew = new Dictionary<string, ArrayList>();

        /// <summary>
        /// 存放不收费和没有医嘱的项目编码：超限额、互斥
        /// </summary>
        List<string> NoFeeItemCodeList = new List<string>();

        /// <summary>
        /// 所有未收费医嘱项目编码
        /// </summary>
        List<string> allListCode = new List<string>();

        /// <summary>
        /// 费用收费的数量已足够，不收取开立医嘱的费用
        /// </summary>
        //Dictionary<string, decimal> NoFeeOrderItem = new Dictionary<string, decimal>();

        /// <summary>
        /// 存放收费医嘱
        /// </summary>
        Dictionary<string, ArrayList> hsFeeExecOrder = new Dictionary<string, ArrayList>();

        /// <summary>
        /// 所有小时收费项目
        /// </summary>
        List<string> listHourItemCode = new List<string>();

        /// <summary>
        /// 出院当天不收费项目列表（ArrayList）
        /// </summary>
        ArrayList alNoFee = new ArrayList();

        /// <summary>
        /// 出院当天不收费项目（string）
        /// </summary>
        string outNoFeeItem = "";

        /// <summary>
        /// 不够1小时的够多少分钟算一次
        /// </summary>
        private const int Q1HMinute = 0;

        #endregion

        #region 业务层变量
        Neusoft.HISFC.BizLogic.Order.Order orderManager = new Neusoft.HISFC.BizLogic.Order.Order();
        Neusoft.HISFC.BizLogic.Fee.UndrugFeeRegularManager feeruleManager = new Neusoft.HISFC.BizLogic.Fee.UndrugFeeRegularManager();
        Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Fee();
        Neusoft.HISFC.BizLogic.RADT.InPatient radtInpatient = new Neusoft.HISFC.BizLogic.RADT.InPatient();
        Neusoft.HISFC.BizLogic.Fee.Item itemManager = new Neusoft.HISFC.BizLogic.Fee.Item();
        Neusoft.HISFC.BizLogic.Fee.PactUnitInfo pactManager = new Neusoft.HISFC.BizLogic.Fee.PactUnitInfo();
        Neusoft.HISFC.BizLogic.Fee.InPatient inpatientManager = new Neusoft.HISFC.BizLogic.Fee.InPatient();

        /// <summary>
        /// 常数管理
        /// </summary>
        Neusoft.HISFC.BizLogic.Manager.Constant constMgr = new Neusoft.HISFC.BizLogic.Manager.Constant();
               
        #endregion

        #region 收费主函数

        /// <summary>
        /// 处理规则收费
        /// </summary>
        /// <param name="patient">住院患者实体</param>
        /// <param name="isOutHos">是否出院时调用</param>
        /// <param name="bTime">医嘱执行开始时间</param>
        /// <param name="eTime">结束时间</param>
        /// <returns></returns>
        public int DoRuleFee(PatientInfo patient, Neusoft.HISFC.Models.Fee.Inpatient.FTSource ftSource,DateTime bTime, DateTime eTime)
        {
            //首先清除数据
            this.ClearData();

            //对于传入参数的注释，每天晚上自动收费传入的参数一般是前天的23:59:50到昨天的23：59:59
            //但是后面对于开始时间没有用到

            //收费开始时间取患者入院时间(提早1个月，考虑急诊患者是后续补录入院的)，保证如果分解不及时，之前的费用也能记上
            //beginTime = bTime;
            alFee = new ArrayList();
            beginTime = patient.PVisit.InTime.AddMonths(-1);
            endTime = eTime;
            //取患者最新状态信息
            patientInfo = this.radtInpatient.QueryPatientInfoByInpatientNO(patient.ID);

            //获得出院时间，下面用到了
            patientInfo.PVisit.OutTime = patient.PVisit.OutTime;
            patientInfo.PVisit.PreOutTime = patient.PVisit.PreOutTime;

            #region 婴儿处理
            if (patientInfo.IsBaby)
            {
                string motherID = this.radtInpatient.QueryBabyMotherInpatientNO(patient.ID);

                if (string.IsNullOrEmpty(motherID))
                {
                    this.Err = radtInpatient.Err;
                    return -1;
                }

                Neusoft.HISFC.Models.RADT.PatientInfo motherPath = this.radtInpatient.QueryPatientInfoByInpatientNO(motherID);

                if (motherPath == null)
                {
                    this.Err = radtInpatient.Err;
                    return -1;
                }

                patientInfo.Pact = motherPath.Pact;
            }
            #endregion

            try
            {

                #region 获取所有非药品收费规则

                //保证单项目的规则按照FEE_TYPE规则类型排序，后面收费受此影响

                if (dsFeeRule == null)
                {
                    dsFeeRule = feeruleManager.GetAlFeeRegular();
                    if (dsFeeRule == null)
                    {
                        this.Err = feeruleManager.Err;
                        return -1;
                    }

                    foreach (DataRow row in dsFeeRule.Tables[0].Rows)
                    {
                        if (!NConvert.ToBoolean(row["OUTFEE_FLAG"]))
                        {
                            outNoFeeItem += row["ITEM_CODE"] + "|";
                        }
                    }
                }
                #endregion

                #region  获取出院当天不收费项目

                //if (alNoFee == null || alNoFee.Count <= 0)
                //{
                //    alNoFee = constMgr.GetAllList("OutNoFee");

                //    if (alNoFee == null)
                //    {
                //        this.Err = "获取出院当天不收费项目列表出错：" + constMgr.Err;
                //    }

                //    foreach (Neusoft.FrameWork.Models.NeuObject constObj in alNoFee)
                //    {
                //         outNoFeeItem += row[""] + "|";
                //    }
                //}

                #endregion

                #region 获取所有小时收费项目

                //屏蔽掉了，根据医嘱的用法是否是Q1H判断是否是小时收费项目，此处就没必要判断了
                //如果不是小时收费项目，他开立为Q1H也按照小时收费项目处理
                //if (listHourItemCode == null || listHourItemCode.Count <= 0)
                //{
                //    listHourItemCode = this.GetHoureFeeItemCode();
                //}

                #endregion

                #region 按个人获取未收费的医嘱项目

                //获取患者所有已执行未收费的项目
                //此处获取所有执行档医嘱，排除了小时收费中小于半个小时的
                if (GetExecOrder() < 0)
                {
                    return -1;
                }

                //没有医嘱
                if (hsAllExecOrder.Count == 0)
                {
                    return 1;
                }

                //用hsAllExecOrderNew存储最初的医嘱信息，后面更新医嘱收费标记用到
                //如果这样的方式新建的哈希表的顺序为原来的倒叙？
                //foreach (string s in hsAllExecOrder.Keys)
                ////foreach(object obj in hsAllExecOrder.Values)
                //{
                //    hsAllExecOrderNew.Add(s, hsAllExecOrder[s]);
                //    //hsAllExecOrderNew.
                //}
                #endregion

                #region 按照患者未收费医嘱中的收费规则处理费用

                if (GetExecOrderByRule() < 0)
                {
                    return -1;
                }

                //没有需要收费的医嘱
                if (hsFeeExecOrder.Count <= 0)
                {
                    return 1;
                }

                #endregion

                #region 收费

                //转换所有医嘱信息到费用实体
                if (this.GetAllFee(patientInfo,ftSource) < 0)
                {
                    return -1;
                }

                //收取项目
                if (alFee.Count > 0)
                {
                    //收费
                    feeIntegrate.MessageType = MessType.N;
                    if (feeIntegrate.FeeItem(patientInfo, ref alFee) == -1)
                    {
                        this.Err = feeIntegrate.Err;
                        return -1;
                    }
                }

                #endregion

                #region 更新收费标记
                //更新医嘱标记，不管有没有收费都更新标记，此处按照查询的执行档来更新，保证不会重复收费
                if (this.hsAllExecOrderNew.Count > 0)
                {
                    if (UpdateExec() < 0)
                    {
                        return -1;
                    }
                }

                ClearData();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return -1;
            }
            #endregion

            return 1;
        }

        #endregion

        #region 查询获取未收费医嘱

        /// <summary>
        /// 获取患者所有已执行未收费的医嘱
        /// </summary>
        /// <returns></returns>
        private int GetExecOrder()
        {
            //获取所有未收费医嘱
            ArrayList alUnChargeOrder = this.feeruleManager.GetPatientNoFeeExecOrder(patientInfo, beginTime, endTime);
            if (alUnChargeOrder == null)
            {
                this.Err = "获取患者医嘱信息失败！" + orderManager.Err;
                return -1;
            }
            if (alUnChargeOrder.Count == 0)
            {
                return 1;
            }

            ArrayList tempal = null;
            foreach (ExecOrder execOrd in alUnChargeOrder)
            {
                //排除出院当天不收费项目
                if (patientInfo.PVisit.OutTime <= new DateTime(2000, 1, 1, 1, 0, 0, 1))
                {
                    patientInfo.PVisit.OutTime = patientInfo.PVisit.PreOutTime;
                }
                if (patientInfo.PVisit.OutTime.Date == this.feeruleManager.GetDateTimeFromSysDateTime().Date)
                {
                    if (execOrd.DateUse.Date == patientInfo.PVisit.OutTime.Date && outNoFeeItem.Contains(execOrd.Order.Item.ID))
                    {
                        continue;
                    }
                }

                //改为用处方号和应执行日期作为键值
                //if (hsAllExecOrder.ContainsKey(execOrd.Order.Item.ID))
                string keys = execOrd.Order.Item.ID + execOrd.DateUse.ToString("yyyyMMdd");

                if (hsAllExecOrder.ContainsKey(keys))
                {
                    tempal = hsAllExecOrder[keys] as ArrayList;
                    tempal.Add(execOrd);

                    hsAllExecOrderNew[keys] = tempal;
                }
                else
                {
                    tempal = new ArrayList();
                    tempal.Add(execOrd);
                    hsAllExecOrder.Add(keys, tempal);
                    hsAllExecOrderNew.Add(keys, tempal);
                    allListCode.Add(keys);
                }
            }

            if (hsAllExecOrder.Count == 0)
            {
                return 1;
            }

            //如果包含小时（Q1H）收费项目，单独处理
            if (this.DealHourExecOrder() < 0)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 处理小时（Q1H）收费项目医嘱 大于30分钟算一次
        /// </summary>
        /// <returns></returns>
        private int DealHourExecOrder()
        {
            //屏蔽掉了，根据医嘱的用法是否是Q1H判断是否是小时收费项目，此处就没必要判断了
            //如果不是小时收费项目，他开立为Q1H也按照小时收费项目处理
            //foreach (string code in listHourItemCode)
            //{
            //if (hsAllExecOrder.ContainsKey(code))
            //{

            foreach (string code in hsAllExecOrder.Keys)
            {
                ArrayList alExecOrder = hsAllExecOrder[code] as ArrayList;
                for (int i = 0; i < alExecOrder.Count; i++)
                {
                    ExecOrder execOrder = alExecOrder[i] as ExecOrder;
                    //查询医嘱信息
                    Neusoft.HISFC.Models.Order.Order order = orderManager.QueryOneOrder(execOrder.Order.ID);
                    if (order == null)
                    {
                        this.Err = "查询医嘱失败！" + orderManager.Err;
                        return -1;
                    }

                    if (order.Frequency.ID.ToUpper() != "Q1H")
                    {
                        continue;
                    }

                    //附加数量
                    int tSpan = 0;
                    //分解明细中第一条为下一个整点然后依次每个整点都形成一条数据。
                    //因此对于首次量，如果分钟>30，则开始小于半小时不收费,tSpan=tSpan-1，如果分钟<30，
                    //则前大于半小时要收取费用,tSpan=tspan。

                    #region 开立当天收费 OK

                    //开立当天收费
                    //修改意见maokb：原来用截止时间来判断是否是收费当天，不准确，因为若医嘱在开立当天未分解，第二天收费时
                    //没有收费，但是第三天收费时再收第一天的费用则无法判断是否是当天开立医嘱
                    if (order.MOTime.Date == execOrder.DateUse.Date)
                    {
                        //修改意见maokb:没必要排除当天停止的医嘱，这样导致当天开立，当天停止的医嘱无法收费
                        #region delete
                        //排除掉昨天停止的项目
                        //if (order.EndTime > new DateTime(2009, 1, 1, 1, 1, 1) && order.EndTime.Date < this.orderManager.GetDateTimeFromSysDateTime().Date)
                        //{
                        //    alExecOrder.Clear();
                        //    continue;
                        //}
                        #endregion
                        //当天开立当天停止
                        if (order.EndTime.Date == order.MOTime.Date)
                        {
                            //修改意见maokb：对于当天开立当天停止的，直接用停止时间减去开立时间然后判断半小时。
                            #region delete
                            //tSpan -= order.BeginTime.Minute > 30 ? 1 : 0;  //开始时间分钟大于半小时就少收一次
                            //tSpan += order.EndTime.Minute >= 30 ? 1 : 0; //停止时间分钟大于等于半小时就多收一次

                            /*停止当天，开始时间和截止时间一起参与半小时判断{80E98AB3-5D27-4c75-BA93-50F44B283028}
                            // * 如果开始时间不满半小时，截止时间也不满半小时，要计算两个分钟数合计是否满半小时
                            // * */
                            //if (order.BeginTime.Minute > 30 && order.EndTime.Minute < 30)
                            //{
                            //    tSpan +=(60 - order.BeginTime.Minute) + order.EndTime.Minute >= 30 ? 1 : 0;
                            //}
                            #endregion
                            TimeSpan ts = order.EndTime - order.BeginTime;
                            if (ts.Minutes < Q1HMinute)
                            {
                                tSpan--;
                            }

                        }
                        //正常开立当天未停止
                        //修改意见maokb：因为要把0点那一次算到0点前一天，所以若开立时间小于30min，需要增加一次
                        else
                        {
                            //增加判断开始时间不是整点，整点不多收
                            //if (order.BeginTime.Minute < 60 - Q1HMinute && order.BeginTime.Minute != 0)
                            //{
                            //    tSpan += 1;
                            //}
                        }

                        if (tSpan != 0)
                        {
                            //alExecOrder.Add(execOrder);
                            execOrder.Order.Item.Qty += tSpan ;
                        }

                        if (execOrder.Order.Item.Qty <= 0)
                        {
                            execOrder.Order.Item.Qty = 1 ;
                        }
                    }

                    #endregion 

                    #region 停止的医嘱，判断是否有半小时和出院当天不收费项目
                      //修改意见maokb:判断停止当天也用执行时间和医嘱停止日期来判断  
                    else if (order.EndTime.Date == execOrder.DateUse.Date && order.Status == 3)
                    {
                        if (order.Frequency.ID != "Q1H")
                        {
                            continue;
                        }
                        //修改意见maokb：因为0点的那次医嘱已经被昨天收掉了，所以今天要减去那一次
                        //tSpan--;
                        //截止时间分钟数大于30分钟，多收一次
                        if (order.EndTime.Minute >= Q1HMinute && order.EndTime.Minute != 0)
                        {
                            tSpan += 1;
                        }
                        /*停止当天，开始时间和截止时间一起参与半小时判断{80E98AB3-5D27-4c75-BA93-50F44B283028}
                            * 如果开始时间不满半小时，截止时间也不满半小时，要计算两个分钟数合计是否满半小时
                            * */
                        //修改意见maokb:应该是开立当天舍掉的与停止当天舍掉的和>30则加收1次
                        else if (order.BeginTime.Minute > Q1HMinute && order.EndTime.Minute < Q1HMinute)
                        {
                            tSpan += (60 - order.BeginTime.Minute) + order.EndTime.Minute >= Q1HMinute ? 1 : 0;
                        }
                        //修改意见maokb:增加开立当天多收的和停止当天多收的之间的和>30则减去1次
                        else if (order.BeginTime.Minute < Q1HMinute && order.EndTime.Minute > Q1HMinute)
                        {
                            tSpan -= order.BeginTime.Minute + (60 - order.EndTime.Minute) > Q1HMinute ? 1 : 0;
                        }                        

                        if (tSpan != 0)
                        {
                            execOrder.Order.Item.Qty += tSpan ;
                        }
                    }
                    #endregion
                }
            }
            //    }
            //}
            return 1;
        }

        #endregion

        #region 按照规则处理医嘱费用

        /// <summary>
        /// 按照患者未收费医嘱中的收费规则处理费用
        /// </summary>
        /// <returns></returns>
        private int GetExecOrderByRule()
        {
            DataRow[] vdr = null;

            foreach (string itemCode in this.hsAllExecOrderNew.Keys)
            {
                ArrayList alExecOrder = hsAllExecOrder[itemCode] as ArrayList;
                if (alExecOrder == null)
                {
                    continue;
                }

                //for (int i = 0; i < alExecOrder.Count; i++)
                //此处不用循环了，对于相同项目在单项目限制SingleOrderItem中已有循环
                if(alExecOrder.Count>0)
                {
                    Neusoft.HISFC.Models.Order.ExecOrder execOrder = alExecOrder[0] as ExecOrder;

                    vdr = dsFeeRule.Tables[0].Select("ITEM_CODE = '" + execOrder.Order.Item.ID + "'");
                    if (vdr == null)
                        continue;
                    string feeType = string.Empty;

                    #region 获取相关项目已收费数量

                    //查询关联项目（包括自身）
                    string relateItems = "";
                    if (feeruleManager.GetRelateItems(execOrder.Order.Item.ID, ref relateItems) <= 0)
                    {
                        this.Err = "查询相关项目失败！" + feeruleManager.Err;
                        return -1;
                    }
                    relateItems = "'" + relateItems.Replace("|", "','") + "'";

                    //获取互斥项目已收费数量，时间限定为医嘱执行时间的当天
                    decimal count = feeruleManager.GetFeeCountByExecOrder(patientInfo, execOrder.DateUse, execOrder.DateUse, relateItems);
                    if (count < 0)
                    {
                        this.Err = "查询患者医嘱已收数量失败！" + orderManager.Err;
                        return -1;
                    }
                    #endregion

                    foreach (DataRow dr in vdr)
                    {
                        feeType = dr[4].ToString();  //限制类别
                        switch (feeType)
                        {
                            //单项限额
                            case "02":
                                    SingleOrderItem(dr, itemCode, count);
                                    break;
                            //组合项目限额
                            case "03":
                                    ComOrderItem(dr, itemCode, count);
                                    break;
                            //项目互斥
                            case "04":
                                    MutexOrder(dr, itemCode, count);
                                    break;
                            //按条件项目互斥
                            case "05":
                                    MutexOrderByCondition(dr, itemCode, count);
                                    break;
                            default:
                                break;
                        }
                    }
                }
            }
            return 1;
        }

        #region 收费规则

        #region FeeType =02单项目限额

        /// <summary>
        /// 对单项目限额的根据条件获取执行医嘱信息
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="itemCode">收费项目编码</param>
        /// <param name="count">已收数量</param>
        /// <returns></returns>
        private int SingleOrderItem(DataRow dr, string itemCode, decimal count)
        {
            /* 算法：
             * 判断项目itemcode收费数量不允许超过count    
             * */

            if (NoFeeItemCodeList.Contains(itemCode))
                return 1;

            //限制条件 1时间 2是次数
            string limitCondition = dr[3].ToString();
            //限制数量
            decimal limit = NConvert.ToDecimal(dr[5]);

            if (!hsFeeExecOrder.ContainsKey(itemCode) && !hsAllExecOrder.ContainsKey(itemCode))
            {
                NoFeeItemCodeList.Add(itemCode);
                return 1;
            }

            ArrayList alExecOrderTemp = new ArrayList();

            //是否是收费医嘱
            bool isFee = hsFeeExecOrder.ContainsKey(itemCode);
            if (isFee)
            {
                alExecOrderTemp = hsFeeExecOrder[itemCode] as ArrayList;
            }
            else
            {
                alExecOrderTemp = hsAllExecOrder[itemCode] as ArrayList;
            } 

            if (alExecOrderTemp == null || alExecOrderTemp.Count == 0)
            {
                if (!NoFeeItemCodeList.Contains(itemCode))
                {
                    NoFeeItemCodeList.Add(itemCode);
                    hsAllExecOrder.Remove(itemCode);
                }

                return 1;
            }

            //获取项目收费次数
            //decimal count = 0m;

            #region 获取项目次数

            ArrayList alTemp = new ArrayList();
            foreach (ExecOrder execOrder in alExecOrderTemp)
            {
                if (count >= limit)
                {
                    if (!NoFeeItemCodeList.Contains(itemCode))
                    {
                        NoFeeItemCodeList.Add(itemCode);
                    }
                    break;
                }

                count += NConvert.ToDecimal(execOrder.Order.Item.Qty);
                if (count <= limit)
                {
                    alTemp.Add(execOrder);
                }
                //超限额了,就少收点 
                else if (count > limit)
                {
                    execOrder.Order.Qty = limit - (count - execOrder.Order.Item.Qty);
                    alTemp.Add(execOrder);

                    if (!NoFeeItemCodeList.Contains(itemCode))
                    {
                        NoFeeItemCodeList.Add(itemCode);
                    }
                }
            }

            if (isFee)
            {
                hsFeeExecOrder[itemCode] = alTemp;
            }
            else
            {
                hsFeeExecOrder.Add(itemCode, alTemp);
                hsAllExecOrder.Remove(itemCode);
            }

            #endregion

            return 1;
        }

        #endregion

        #region FeeType =03 多项目组合限额 判断了单项目的限额

        /// <summary>
        /// 多项目组合限额
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        private int ComOrderItem(DataRow dr, string itemCode, decimal count)
        {
            /* 算法：
             * 判断相关项目在收费列表hsFeeExecOrder中已有的数量
             * 保证相关项目数量+itemcode的数量不超过限定数量
             * */

            /*加载医嘱信息列表的时候按照开立时间和使用时间排序，优先收取后开立，先执行的
             * 
             * 所有收费医嘱hsAllExecOrder
             * 确定收费的医嘱hsFeeExecOrder
             * 不收费的医嘱NoFeeItemCodeList
             * 
             **/
            //if (NoFeeItemCodeList.Contains(itemCode))
            //{
            //    return 1;
            //}

            //限制条件 1时间 2是次数
            string limitCondition = dr[3].ToString();
            //限制数量
            decimal limit = NConvert.ToDecimal(dr[5]);
            //同种类项目
            string vCom = dr[6].ToString();

            //所有同种类项目编码集合
            List<string> allItemCodeList = new List<string>();

            #region 获取所有收费项目编码

            //取出所有同种类项目编码
            if (!string.IsNullOrEmpty(vCom))
            {
                string[] vs = vCom.Split('|');
                foreach (string s in vs)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    if (allItemCodeList.Contains(s))
                        continue;
                    //if (NoFeeItemCodeList.Contains(s))
                    //    continue;
                    allItemCodeList.Add(s);
                }
            }

            if (allItemCodeList.Count == 0)
            {
                return 1;
            }

            #endregion

            #region 根据组合规则获取医嘱信息

            ArrayList alExecOrder = new ArrayList();

            //decimal count = 0m;
            foreach (string code in allItemCodeList)
            {
                string orderCode = code + itemCode.Substring(itemCode.Length - 8);

                //如果费用医嘱列表里面已经存在，计算数量
                if (hsFeeExecOrder.ContainsKey(orderCode))
                {
                    ArrayList alTemp = hsFeeExecOrder[orderCode] as ArrayList;
                    if (alTemp != null)
                    {
                        foreach (ExecOrder execOrder in alTemp)
                        {
                            count += execOrder.Order.Item.Qty;
                        }
                    }
                }

                if (count >= limit)
                {
                    if (hsFeeExecOrder.ContainsKey(itemCode))
                    {
                        hsFeeExecOrder.Remove(itemCode);
                    }
                    else if (hsAllExecOrder.ContainsKey(itemCode))
                    {
                        hsAllExecOrder.Remove(itemCode);
                    }

                    return 1;
                }
            }

            if (hsAllExecOrder.ContainsKey(itemCode))
            {
                alExecOrder.AddRange(hsAllExecOrder[itemCode] as ArrayList);
                hsAllExecOrder.Remove(itemCode);
            }
            else
            {
                if (hsFeeExecOrder.ContainsKey(itemCode))
                {
                    alExecOrder.AddRange(hsFeeExecOrder[itemCode] as ArrayList);
                    hsFeeExecOrder.Remove(itemCode);
                }
            }

            this.GetSortComItem(alExecOrder, limit, count);

            #endregion

            return 1;
        }

        /// <summary>
        /// 按照执行时间获取应收数量
        /// </summary>
        /// <param name="al">医嘱项目</param>
        /// <param name="limit">限额</param>
        /// <returns></returns>
        private int GetSortComItem(ArrayList al, decimal limit, decimal count)
        {
            //按照应使用时间排序
            CompareExecOrderByExecTime compareByExecTime = new CompareExecOrderByExecTime();
            al.Sort(compareByExecTime);
            //项目数量
            //decimal count = 0m;

            ArrayList alTemp = new ArrayList();
            foreach (ExecOrder order in al)
            {
                if (count >= limit)
                {
                    continue;
                }

                count += order.Order.Qty;
                if (count < limit)
                {
                    alTemp.Add(order);
                }
                else if (count > limit)
                {
                    decimal amod = count - limit;
                    order.Order.Qty -= amod;
                    alTemp.Add(order);
                }
                else
                {
                    alTemp.Add(order);
                }
            }

            ArrayList al1 = null;
            foreach (ExecOrder obj in alTemp)
            {
                string itemCode = obj.Order.Item.ID + obj.DateUse.ToString("yyyyMMdd");
                if (hsFeeExecOrder.ContainsKey(itemCode))
                {
                    al1 = hsFeeExecOrder[itemCode] as ArrayList;
                    al1.Add(obj);
                }
                else
                {
                    al1 = new ArrayList();
                    al1.Add(obj);
                    hsFeeExecOrder.Add(itemCode, al1);
                }
            }
            return 1;
        }

        #endregion

        #region FeeType = 04 项目互斥 判断了单项目的限额

        /// <summary>
        /// 互斥项目
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        private int MutexOrder(DataRow dr, string itemCode, decimal count)
        {
            List<string> comCode = new List<string>();
            //decimal count = 0m;
            //限额数量
            decimal limit = NConvert.ToDecimal(dr[5]);
            string[] limitItem = null;

            //如果互斥项目已收费，则退出
            foreach (string code in hsFeeExecOrder.Keys)
            {
                if (dr[6].ToString().Contains(code.Substring(0, code.Length - 8)))
                {
                    return 1;
                }
            }

            //互斥项目
            if (!string.IsNullOrEmpty(dr[6].ToString()))
            {
                limitItem = dr[6].ToString().Split('|');
            }

            //不收费包括：超限额、互斥
            if (NoFeeItemCodeList.Contains(itemCode))
            {
                return 1;
            }
            else
            {
                return this.SingleOrderItem(dr, itemCode, count);
            }
        }

        #endregion

        #region FeeType =05 按条件项目互斥，当数量大于指定值或星期大于指定值则互斥

        /// <summary>
        /// 互斥项目
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        private int MutexOrderByCondition(DataRow dr, string itemCode, decimal count)
        {
            string limitCondition = dr[3].ToString();
            decimal limit = NConvert.ToDecimal(dr[5]);
            //互斥项目
            string[] limitItem = null;
            if (!string.IsNullOrEmpty(dr[6].ToString()))
            {
                limitItem = dr[6].ToString().Split('|');
            }

            bool isLimit = false;
            if ("0".Equals(limitCondition))
            {
                #region 大于指定数量

                 //如果费用医嘱列表里面已经存在，计算数量
                if (hsFeeExecOrder.ContainsKey(itemCode))
                {
                    ArrayList alTemp = hsFeeExecOrder[itemCode] as ArrayList;
                    if (alTemp != null)
                    {
                        foreach (ExecOrder execOrder in alTemp)
                        {
                            count += execOrder.Order.Item.Qty;
                        }
                    }
                }

                if (count >= limit)//如果数量超额
                {
                    isLimit = true;
                }

                #endregion
            }
            else if ("1".Equals(limitCondition))
            {
                #region 大于指定星期

                //获取当前时间
                DateTime dtNow = this.feeruleManager.GetDateTimeFromSysDateTime();

                System.Windows.Forms.Day day=  Neusoft.FrameWork.Public.EnumHelper.Current.GetEnum<System.Windows.Forms.Day>(dtNow.DayOfWeek.ToString());
                if ((int)day + 1 >= (int)limit)
                {
                    isLimit = true;
                }

                #endregion
            }

            if (isLimit)
            {
                //限制互斥项目收费
                foreach (string limitCode in limitItem)
                {
                    string temp = limitCode + itemCode.Substring(itemCode.Length - 8);
                    if (hsFeeExecOrder.ContainsKey(temp))
                    {
                        hsFeeExecOrder.Remove(temp);
                    }
                    else if (hsAllExecOrder.ContainsKey(temp))
                    {
                        hsAllExecOrder.Remove(temp);
                    }

                    if (!NoFeeItemCodeList.Contains(limitCode))
                    {
                        NoFeeItemCodeList.Add(limitCode);
                    }
                }
            }
            else
            {
                //无限制的话，则将全部收取
                if (hsFeeExecOrder.ContainsKey(itemCode) == false)
                {
                    if (hsAllExecOrder.ContainsKey(itemCode))
                    {
                        hsFeeExecOrder.Add(itemCode, hsAllExecOrder[itemCode]);
                        hsAllExecOrder.Remove(itemCode);
                    }
                }
            }

            return 1;
        }

        #endregion

        #endregion

        #endregion

        #region 更新执行档收费标记

        /// <summary>
        /// 从哈希表中获得医嘱列表
        /// </summary>
        /// <param name="hsOrder"></param>
        /// <returns></returns>
        private ArrayList GetAllFeeOrder(Dictionary<string, ArrayList> hsOrder)
        {
            ArrayList alTemp = new ArrayList();
            ArrayList al = new ArrayList();
            IDictionaryEnumerator id = hsOrder.GetEnumerator();
            while (id.MoveNext())
            {
                alTemp = id.Value as ArrayList;
                if (alTemp.Count > 0)
                {
                    al.AddRange(alTemp.ToArray());
                }
            }
            return al;
        }

        /// <summary>
        /// 更新医嘱收费标记
        /// </summary>
        /// <returns></returns>
        private int UpdateExec()
        {
            ArrayList al = new ArrayList();

            al = GetAllFeeOrder(hsAllExecOrderNew);

            if (al.Count == 0) return 0;
            foreach (ExecOrder objOrder in al)
            {
                #region old作废
                //按照执行档，单条更新
                //if (orderManager.UpdateChargeExec(objOrder) == -1)//更新收费标记
                //{
                //    this.Err = orderManager.Err;
                //    return -1;
                //}
                #endregion

                //因为查询执行档时，已经把医嘱数量汇总了，此处更新的时候也相应按照汇总更新
                //此处objOrder已经是汇总后的数量
                if (this.feeruleManager.UpdateChargeExecNew(this.patientInfo, objOrder) == -1)//更新收费标记
                {
                    this.Err = orderManager.Err;
                    return -1;
                }
            }
            return 1;
        }
        #endregion

        #region 转换医嘱信息到费用实体

        /// <summary>
        /// 转化所有医嘱到费用实体
        /// </summary>
        /// <param name="p"></param>
        /// <param name="hsOrder"></param>
        /// <returns></returns>
        private int GetAllFee(PatientInfo p, Neusoft.HISFC.Models.Fee.Inpatient.FTSource ftSource)
        {
            FeeItemList f = null; 
            DateTime feeDate = new DateTime();
            ArrayList al = new ArrayList();
            IDictionaryEnumerator id = this.hsFeeExecOrder.GetEnumerator();
            while (id.MoveNext())
            {
                al = id.Value as ArrayList;
                foreach (ExecOrder execOrder in al)
                {
                    //出院当天，收费时间为出院时间
                    if (execOrder.DateUse.Date == p.PVisit.OutTime.Date)
                    {
                        feeDate = p.PVisit.OutTime;
                    }
                    //其他收费时间为应使用时间的当晚
                    else
                    {
                        feeDate = new DateTime(execOrder.DateUse.Year, execOrder.DateUse.Month, execOrder.DateUse.Day, 23, 59, 59);
                    }

                    if (execOrder.Order.Item.Qty <= 0)
                    {
                        continue;
                    }

                    //收费时间取医嘱应执行的当天晚上时间
                    f = this.ConvertExecOrderToFeeItemList(p, execOrder, feeDate);

                    if (f == null)
                    {
                        this.Err = "转化费用信息失败！";
                        return -1;
                    }
                    f.FTSource = ftSource.Clone();//记录费用来源
                    alFee.Add(f);
                }
            }
            return 1;
        }

        /// <summary>
        /// 将执ExecOrder转成FeeItemList
        /// </summary>
        /// <returns></returns>
        private Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList ConvertExecOrderToFeeItemList(PatientInfo patient, ExecOrder eOrder, DateTime feeDate)
        {
            //更新医嘱实体
            Neusoft.HISFC.Models.Fee.Item.Undrug undrugItem = itemManager.GetValidItemByUndrugCode(eOrder.Order.Item.ID);
            if (undrugItem == null)
            {
                this.Err = "获取非药品项目信息失败！" + itemManager.Err;
                return null;
            }

            Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList = new Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList();

            feeItemList.Item = undrugItem;

            if (eOrder.Order.HerbalQty == 0)
            {
                eOrder.Order.HerbalQty = 1;
            }           
            decimal price = feeItemList.Item.Price;
            //处理浮动价格

            if (feeItemList.Item.ItemType == EnumItemType.UnDrug)
            {
                Neusoft.HISFC.Models.Fee.Item.Undrug undrug = feeItemList.Item as Neusoft.HISFC.Models.Fee.Item.Undrug;
                decimal orgPrice = 0;
                if(this.feeIntegrate.GetPriceForInpatient(patient,feeItemList.Item,ref price,ref orgPrice)==-1)
                {
                    this.Err = "取项目:" + feeItemList.Item.Name + "的价格出错!" + pactManager.Err;

                    return null;
                }
            }
            feeItemList.Item.Price = price;
            feeItemList.Item.DefPrice = price;
            feeItemList.Item.Qty = eOrder.Order.Qty * eOrder.Order.HerbalQty;
            //增加付数的赋值 {AE53ACB5-3684-42e8-BF28-88C2B4FF2360}
            feeItemList.Days = eOrder.Order.HerbalQty;

            feeItemList.Item.PriceUnit = eOrder.Order.Unit;//单位重新付
            feeItemList.RecipeOper.Dept = eOrder.Order.ReciptDept.Clone();
            feeItemList.RecipeOper.ID = eOrder.Order.ReciptDoctor.ID;
            feeItemList.RecipeOper.Name = eOrder.Order.ReciptDoctor.Name;
            feeItemList.ExecOper = eOrder.Order.ExecOper.Clone();
            feeItemList.ExecOper.Dept.ID = eOrder.Order.ExeDept.ID;
            feeItemList.StockOper.Dept = eOrder.Order.StockDept.Clone();
            if (feeItemList.Item.PackQty == 0)
            {
                feeItemList.Item.PackQty = 1;
            }
            feeItemList.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber((feeItemList.Item.Price * feeItemList.Item.Qty / feeItemList.Item.PackQty), 2);
            feeItemList.FT.OwnCost = feeItemList.FT.TotCost;
            feeItemList.IsBaby = eOrder.Order.IsBaby;
            feeItemList.IsEmergency = eOrder.Order.IsEmergency;
            feeItemList.Order = eOrder.Order.Clone();
            feeItemList.ExecOrder.ID = eOrder.Order.User03;
            feeItemList.NoBackQty = feeItemList.Item.Qty;
            feeItemList.FTRate.OwnRate = 1;
            feeItemList.BalanceState = "0";

            eOrder.IsCharge = true;

            //操作环境
            eOrder.ChargeOper.Dept = eOrder.Order.NurseStation;
            eOrder.ChargeOper.ID = "000000";
            eOrder.ChargeOper.Name = "日计费";
            eOrder.ChargeOper.OperTime = feeDate; //收费时间取

            //收费环境
            feeItemList.ChargeOper.ID = eOrder.ChargeOper.ID;
            feeItemList.ChargeOper.Name = eOrder.ChargeOper.Name;
            feeItemList.ChargeOper.OperTime = feeDate;

            feeItemList.FeeOper.ID = eOrder.ChargeOper.ID;
            feeItemList.FeeOper.Name = eOrder.ChargeOper.Name;
            feeItemList.FeeOper.OperTime = this.feeruleManager.GetDateTimeFromSysDateTime(); //操作时间取当前时间

            //扩展信息存 费用时间段的截止时间 endTime 用于后面更新
            feeItemList.ExtOper.OperTime = endTime;

            feeItemList.TransType = TransTypes.Positive;
            feeItemList.UndrugComb.ID = eOrder.Order.Package.ID;
            feeItemList.UndrugComb.Name = eOrder.Order.Package.Name;

            //user02为扩展操作员
            feeItemList.User02 = eOrder.Order.Oper.ID;

            return feeItemList;
        }
        #endregion

        #region 最后清空
        /// <summary>
        /// 清空列表
        /// </summary>
        private void ClearData()
        {
            alFee.Clear();            
            alQuitFee.Clear();
            hsFeeExecOrder.Clear();
            hsAllExecOrder.Clear();
            hsAllExecOrderNew.Clear();
            NoFeeItemCodeList.Clear();          
            allListCode.Clear();         
        }
        #endregion

        //////////////////////////////////////////

        /// <summary>
        /// 按照收费规则获取项目编码
        /// </summary>
        /// <returns></returns>
        public List<string> GetFeeRuleItemCode()
        {
            
            List<string> listItemCode = new List<string>();
            try
            {
                DataSet ds = feeruleManager.GetAlFeeRegular();
                if (ds == null)
                {
                    this.Err = feeruleManager.Err;
                    return null;
                }
                string itemCode = string.Empty;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    itemCode = dr[1].ToString();  //取项目编码
                    if (!listItemCode.Contains(itemCode))
                    {
                        listItemCode.Add(itemCode);
                    }
                    itemCode = dr[6].ToString();
                    if (!string.IsNullOrEmpty(itemCode))
                    {
                        string[] s = itemCode.Split('|');
                        foreach (string ss in s)
                        {
                            if (!listItemCode.Contains(ss))
                            {
                                listItemCode.Add(ss);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            return listItemCode;
        }
    }

    /// <summary>
    /// 项目排序类
    /// </summary>
    class CompareExecOrderByExecTime : IComparer
    {
        public int Compare(object x, object y)
        {
            ExecOrder o1 = (x as ExecOrder).Clone();
            ExecOrder o2 = (y as ExecOrder).Clone();

            DateTime oX = o1.DateUse;// ExecOper.OperTime;
            DateTime oY = o2.DateUse; //ExecOper.OperTime;

            return DateTime.Compare(oX, oY);
        }

    }
}
