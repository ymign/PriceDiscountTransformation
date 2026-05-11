using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BP.OutPatient
{
    public class QueryManager
    {
        Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        Neusoft.HISFC.BizLogic.Fee.Item undrugManager = new Neusoft.HISFC.BizLogic.Fee.Item();
        Neusoft.HISFC.BizLogic.Registration.Register registerManager = new Neusoft.HISFC.BizLogic.Registration.Register();
        #region Patient相关
        /// <summary>
        /// 查询卡号是否有效
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="cardType"></param>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public int CardEabled(string patientId, string cardType, string cardNo)
        {
            BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();
            bool isExist = patientInfoLogic.IsAny(w => w.CARD_NO == patientId && w.CARD_NO == cardNo);
            return isExist ? 1 : 0;
        }
        /// <summary>
        /// 查询患者信息
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="medicalNo"></param>
        /// <param name="certifcateType"></param>
        /// <param name="certifcateNo"></param>
        /// <returns></returns>
        public DataTable QueryPatientList(string patientId, string medicalNo, string certifcateType, string certifcateNo)
        {

            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();

            return patientInfoLogic.QueryPatientList(patientId, medicalNo, certifcateType, certifcateNo);
        }

        public DataTable QueryByCondition(string certifcateType, string certifcateNo, string name, string visitNo)
        {

            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();

            return patientInfoLogic.QueryByCondition(certifcateType, certifcateNo, name, visitNo);
        }
        #endregion

        #region Register相关
        public DataTable QueryResgisterInfo(string patientId, DateTime startDate, DateTime endDate)
        {
            FS.ZDWY.Internet.BL.RegisterInfoLogic registerInfo = new BL.RegisterInfoLogic();
            return registerInfo.QueryRegisterInfo(patientId, startDate, endDate);
        }

        public DataTable QueryBookingRegInfo(string patientId)
        {
            FS.ZDWY.Internet.BL.RegisterInfoLogic registerInfo = new BL.RegisterInfoLogic();
            return registerInfo.QueryAddRegInfo(patientId);
        }

        public Models.Views.ComResult<Models.Views.QueryResult> QueryOrder(string orderId, string hospitalNum, string queryFlag, string frontProviderId)
        {
            Models.Views.ComResult<Models.Views.QueryResult> result = new Models.Views.ComResult<Models.Views.QueryResult>();
            try
            {

                FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic plalogic = new FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic();
                Models.PLATFORM_REGISTER_ORDER ord = plalogic.Get(orderId);

                if(ord==null)
                {
                    throw new Exception("orderid不存在！");
                }

                FS.ZDWY.Internet.BL.OutPatient.BookingLogic booklogic = new FS.ZDWY.Internet.BL.OutPatient.BookingLogic();
                Models.FIN_OPR_BOOKING bookobj = booklogic.Get(hospitalNum);

                List<Models.FIN_OPR_REGISTER> reglist = null;
                Models.FIN_OPR_REGISTER regobj = null;
                if (!string.IsNullOrEmpty(bookobj.REG_ID))
                {
                    FS.ZDWY.Internet.BL.RegisterInfoLogic reglog = new FS.ZDWY.Internet.BL.RegisterInfoLogic();
                    reglist = reglog.GetList(o => o.CLINIC_CODE == bookobj.REG_ID);
                    if (reglist == null || reglist.Count == 0)
                    {
                        throw new Exception("查询患者信息信息出错！");
                    }
                    regobj = reglist[0];
                }

                if (regobj != null && regobj.YNSEE == "1")
                {
                    ord.STATUS = "6";
                }

                Models.Views.QueryResult res = new Models.Views.QueryResult();
                if (regobj == null)
                {
                    res.HospitalNum = hospitalNum;
                    res.VisitAddress = "";
                    res.VisitNo = "";
                    res.OrderTime = "";
                    res.PayTime = "";
                    res.TakeTime = "";
                    res.CancelTime = "";
                    res.RefundTime = "";
                    res.PayAmt = "";
                    res.RefundFee = "";
                    res.OrderStatus = ord.STATUS;
                }
                else
                {
                    res.HospitalNum = hospitalNum;
                    res.VisitAddress = regobj.DEPT_NAME;
                    res.VisitNo = regobj.SEENO.ToString();
                    res.OrderTime = ord.ORDERTIME.ToString();
                    res.PayTime = regobj.OPER_DATE.ToString();
                    res.TakeTime = regobj.OPER_DATE.ToString();
                    res.CancelTime = regobj.CANCEL_DATE.ToString();
                    res.RefundTime = regobj.CANCEL_DATE.ToString();
                    res.PayAmt = "";
                    res.RefundFee = "";
                    res.OrderStatus = ord.STATUS;
                }

                result.IsSuccessful = true;
                result.Message = string.Empty;
                result.ReturnData = res;

                return result;
            }
            catch (Exception e)
            {
                result.IsSuccessful = false;
                result.Message = e.Message.ToString();
                return result;
            }
        }
        #endregion

        #region Scheduling 相关
        /// <summary>
        /// 查询医生信息
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public System.Data.DataTable QueryDoctorList(DateTime beginDate, DateTime endDate, string deptCode)
        {
            FS.ZDWY.Internet.BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
            return schedulingLogic.QueryDoctList(beginDate, endDate, deptCode);
        }

        /// <summary>
        /// 查询长者券医生信息
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public System.Data.DataTable QueryZZQDoctorList(DateTime beginDate, DateTime endDate, string deptCode)
        {
            FS.ZDWY.Internet.BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
            return schedulingLogic.QueryZZQDoctList(beginDate, endDate, deptCode);
        }
        /// <summary>
        /// 查询排班信息
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="deptCode"></param>
        /// <param name="doctorCode"></param>
        /// <returns></returns>
        public System.Data.DataTable QuerySchedule(DateTime beginDate, DateTime endDate, string deptCode, string doctorCode)
        {
            FS.ZDWY.Internet.BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
            return schedulingLogic.QuerySchedule(beginDate, endDate, deptCode, doctorCode);
        }

        /// <summary>
        /// 查询长者券排班信息
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="deptCode"></param>
        /// <param name="doctorCode"></param>
        /// <returns></returns>
        public System.Data.DataTable QueryZZQSchedule(DateTime beginDate, DateTime endDate, string deptCode, string doctorCode)
        {
            FS.ZDWY.Internet.BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
            return schedulingLogic.QueryZZQSchedule(beginDate, endDate, deptCode, doctorCode);
        }
        /// <summary>
        /// 查询分时排班信息
        /// </summary>
        /// <param name="dtscheduleDate"></param>
        /// <param name="scheduleId"></param>
        /// <param name="deptCode"></param>
        /// <param name="doctorCode"></param>
        /// <returns></returns>
        public System.Data.DataTable QueryScheduleTime(DateTime dtscheduleDate, string scheduleId, string deptCode, string doctorCode)
        {
            FS.ZDWY.Internet.BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
            return schedulingLogic.QueryScheduleTime(dtscheduleDate, scheduleId, deptCode, doctorCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable QueryBookDept()
        {
            BL.OutPatient.SchedulingLogic scl = new BL.OutPatient.SchedulingLogic();
            return scl.QueryBookDept();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable QueryZZQBookDept()
        {
            BL.OutPatient.SchedulingLogic scl = new BL.OutPatient.SchedulingLogic();
            return scl.QueryZZQBookDept();
        }

        public System.Data.DataTable QueryScheduleByDoctor(DateTime beginDate, DateTime endDate, string doctorCode)
        {
            FS.ZDWY.Internet.BL.OutPatient.SchedulingLogic schedulingLogic = new BL.OutPatient.SchedulingLogic();
            return schedulingLogic.QueryByDoctorCode(beginDate, endDate, doctorCode);
        }
        #endregion

        #region 费用相关


        public List<FS.ZDWY.Internet.Models.FIN_OPR_REGISTER> GetRegisterList(string patientId, string medicalNo, string visitNo, string startTime, string endTime, ref string erro)
        {
            List<FS.ZDWY.Internet.Models.FIN_OPR_REGISTER> registerlist = new List<Models.FIN_OPR_REGISTER>();
            try
            {
                FS.ZDWY.Internet.BL.RegisterInfoLogic regmgr = new BL.RegisterInfoLogic();
                DateTime now = regmgr.GetDateTime();

                if (!string.IsNullOrEmpty(startTime) || !string.IsNullOrEmpty(endTime))
                {
                    DateTime dtbegin = Neusoft.FrameWork.Function.NConvert.ToDateTime(startTime);
                    DateTime dtend = Neusoft.FrameWork.Function.NConvert.ToDateTime(endTime);
                    registerlist = regmgr.GetList(o => o.CARD_NO == patientId && o.CLINIC_CODE.Contains(visitNo) && o.REG_DATE.Date >= dtbegin && o.REG_DATE <= dtend && o.VALID_FLAG == "1" && o.REG_DATE >= now.AddDays(-7));
                    //registerlist = registerlist.OrderByDescending(o => o.REG_DATE).ToList<FS.ZDWY.Internet.Models.FIN_OPR_REGISTER>();
                    return registerlist;
                }
                
                registerlist = regmgr.GetList(o => o.CARD_NO == patientId && o.CLINIC_CODE.Contains(visitNo) && o.VALID_FLAG == "1" && o.REG_DATE > now.AddDays(-7));
                //registerlist = registerlist.OrderByDescending(o => o.REG_DATE).ToList<FS.ZDWY.Internet.Models.FIN_OPR_REGISTER>();
                return registerlist;
            }
            catch (Exception e)
            {
                erro = e.Message.ToString();
                return null;
            }


        }

        public string GetEmployeeType(string empl_code)
        {
            FS.ZDWY.Internet.BL.EmployeeLogic logic = new BL.EmployeeLogic();
            List<FS.ZDWY.Internet.Models.COM_EMPLOYEE> emplList = logic.GetList(e => e.EMPL_CODE == empl_code);
            if (emplList != null && emplList.Count > 0)
            {
                return emplList[0].EMPL_TYPE;
            }
            else
            {
                return "";
            }
        }

        public DataTable billListOld(string visitNo, ref string erro)
        {
            try
            {
                FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
                #region sql
                string sql = @"select 
reg.clinic_code hospitalNum,
nvl(reg.see_dpcd ,reg.dept_code) deptCode,
fun_get_dept_name(nvl(reg.see_dpcd ,reg.dept_code)) deptName,
nvl(reg.see_docd,reg.doct_code) doctorCode,
fun_get_employee_name(nvl(reg.see_docd,reg.doct_code)) doctorName,
(select sum(fe.PUB_COST + fe.PAY_COST + fe.OWN_COST) from fin_opb_feedetail fe where fe.clinic_code=reg.clinic_code and fe.pay_flag = '0') totalAmt,
nvl(reg.see_date,SYSDATE) visitDate,
reg.clinic_code visitNo,
m.ITEM_CODE prescriptionId, --项目id
m.ITEM_NAME itemName, --  项目名称
m.drug_flag prescriptionType, --项目类型
(m.PUB_COST + m.PAY_COST + m.OWN_COST)*100 selfAmt, --缴费金额
m.RECIPE_NO,
m.class_code
  from fin_opb_feedetail m
  inner join fin_opr_register reg on reg.clinic_code=m.clinic_code and reg.valid_flag='1'
  where 1=1
  and pay_flag = '0'
  --and  nvl(m.extend_flag,'0')<>'1'
  and  nvl((select qq.extend_flag from met_ord_recipedetail qq where qq.sequence_no=m.mo_order and qq.clinic_code=m.clinic_code and rownum=1),nvl(m.extend_flag,'0'))<>'1'
  and m.clinic_code='{0}' 
  --and m.EXEC_DPCD != '7017' --屏蔽输血科
  --and (m.PUB_COST + m.PAY_COST + m.OWN_COST) > 0 
  and not exists (select 1 from fin_opb_feedetail a where a.clinic_code=m.clinic_code and a.pay_flag = '0' and a.hos_code!='CORE_HIS50' and pay_flag = '0' ) -- 屏蔽校区
  and m.RECIPE_FLAG not in ('5')--屏蔽日间预入院
 and reg.pact_code <> '258'
";
                sql = string.Format(sql, visitNo);
                #endregion
                DataSet ds = null;
                if (mgr.ExecQuery(sql, ref ds) <= 0)
                {
                    throw new Exception(mgr.Err);
                }
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("未找到数据！");
                }
                if (ds.Tables[0] == null)
                {
                    throw new Exception("未找到数据！");
                }
                DataTable resdt = ds.Tables[0];
                return resdt;
            }
            catch (Exception e)
            {
                erro = e.Message.ToString();
                return null;
            }
        }
        /// <summary>
        /// 获取医院待缴费列表
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="medicalNo"></param>
        /// <param name="visitNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="erro"></param>
        /// <returns></returns>
        public DataTable billList(string visitNo, ref string erro)
        {
            try
            {
                #region 旧版

               
                if (false)
                {
                    FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
                    #region sql
                    string sql = @"select 
reg.clinic_code hospitalNum,
nvl(reg.see_dpcd ,reg.dept_code) deptCode,
fun_get_dept_name(nvl(reg.see_dpcd ,reg.dept_code)) deptName,
nvl(reg.see_docd,reg.doct_code) doctorCode,
fun_get_employee_name(nvl(reg.see_docd,reg.doct_code)) doctorName,
(select sum(fe.PUB_COST + fe.PAY_COST + fe.OWN_COST) from fin_opb_feedetail fe where fe.clinic_code=reg.clinic_code and fe.pay_flag = '0') totalAmt,
nvl(reg.see_date,SYSDATE) visitDate,
reg.clinic_code visitNo,
m.ITEM_CODE prescriptionId, --项目id
m.ITEM_NAME itemName, --  项目名称
m.drug_flag prescriptionType, --项目类型
(m.PUB_COST + m.PAY_COST + m.OWN_COST)*100 selfAmt, --缴费金额
m.RECIPE_NO
  from fin_opb_feedetail m
  inner join fin_opr_register reg on reg.clinic_code=m.clinic_code and reg.valid_flag='1'
  where 1=1
  and pay_flag = '0'
  --and  nvl(m.extend_flag,'0')='0'
  and  nvl((select qq.extend_flag from met_ord_recipedetail qq where qq.sequence_no=m.mo_order and qq.clinic_code=m.clinic_code and rownum=1),'0')<>'1'
  and m.clinic_code='{0}' 
  --and m.EXEC_DPCD != '7017' --屏蔽输血科
  and (m.PUB_COST + m.PAY_COST + m.OWN_COST) > 0 
  and not exists (select 1 from fin_opb_feedetail a where a.clinic_code=m.clinic_code and a.pay_flag = '0' and a.hos_code!='CORE_HIS50' and pay_flag = '0' ) -- 屏蔽校区
  and m.RECIPE_FLAG not in ('5')--屏蔽日间预入院
";
                    sql = string.Format(sql, visitNo);
                    #endregion
                    DataSet ds = null;
                    if (mgr.ExecQuery(sql, ref ds) <= 0)
                    {
                        throw new Exception(mgr.Err);
                    }
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        throw new Exception("未找到数据！");
                    }
                    if (ds.Tables[0] == null)
                    {
                        throw new Exception("未找到数据！");
                    }
                DataTable resdt = ds.Tables[0];
                return resdt;
                }
                #endregion

                DataTable dataResdt = new DataTable();
                #region Columns
                dataResdt.Columns.Add(new DataColumn("hospitalNum", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("deptCode", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("deptName", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("doctorCode", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("doctorName", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("totalAmt", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("visitDate", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("visitNo", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("prescriptionId", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("itemName", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("prescriptionType", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("selfAmt", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("RECIPE_NO", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("isStore", typeof(String)));
                dataResdt.Columns.Add(new DataColumn("reciptType", typeof(String)));
                #endregion
                string getFeeSql = @"select 
              m.RECIPE_NO, --  处方号
              SEQUENCE_NO, --  处方内项目流水号
              m.TRANS_TYPE, --  交易类型,1正交易，2反交易
              m.CLINIC_CODE, --  门诊号
              m.CARD_NO, --  病历卡号
              m.REG_DATE, --  挂号日期
              REG_DPCD, --  挂号科室
              m.DOCT_CODE, --  开方医师
              DOCT_DEPT, --  开方医师所在科室
              m.ITEM_CODE, --  项目代码
              ITEM_NAME, --  项目名称
              DRUG_FLAG, --  1药品/2非要
              SPECS,  --  规格
              SELF_MADE, --  自制药标志
              DRUG_QUALITY, --  药品性质，麻药，普药
              DOSE_MODEL_CODE,--  剂型
              FEE_CODE, --  最小费用代码
              CLASS_CODE, --  系统类别
              UNIT_PRICE, --  单价
              QTY,  --  数量
              DAYS,  --  草药的付数，其他药品为1
              FREQUENCY_CODE, --  频次代码
              USAGE_CODE, --  用法代码
              USE_NAME, --  用法名称
              INJECT_NUMBER, --  院内注射次数
              EMC_FLAG, --  加急标记:1普通/2加急
              LAB_TYPE, --  样本类型
              CHECK_BODY, --  检体
              DOSE_ONCE, --  每次用量
              DOSE_UNIT, --  每次用量单位
              BASE_DOSE, --  基本剂量
              PACK_QTY, --  包装数量
              PRICE_UNIT, --  计价单位
              m.PUB_COST, --  可报效金额
              m.PAY_COST, --  自付金额
              m.OWN_COST, --  现金金额
              EXEC_DPCD, --  执行科室代码
              EXEC_DPNM, --  执行科室名称
              CENTER_CODE, --  医保中心项目代码
              ITEM_GRADE, --  项目等级，1甲类，2乙类，3丙类
              MAIN_DRUG, --  主药标志
              COMB_NO, --  组合号
              m.OPER_CODE, --  划价人
              m.OPER_DATE, --  划价时间
              PAY_FLAG, --  收费标志，1未收费，2收费
              CANCEL_FLAG, --  作废标志,1未作废,2作废
              FEE_CPCD, --  收费员代码
              FEE_DATE, --  收费日期
              m.INVOICE_NO, --  票据号
              INVO_CODE, --  发票科目代码
              INVO_SEQUENCE, --  发票内流水号
              CONFIRM_FLAG, --  1未确认/2确认
              CONFIRM_CODE, --  确认人
              CONFIRM_DEPT, --  确认科室
              CONFIRM_DATE, --  确认时间
              INVOICE_SEQ,
       NEW_ITEMRATE,--    NUMBER(6,2)   Y                新项目比例
              OLD_ITEMRATE,--    NUMBER(6,2)   Y                原项目比例
              EXT_FLAG,--        VARCHAR2(1)   Y        '0'     扩展标志 特殊项目标志 1 0 非
              EXT_FLAG1,--       VARCHAR2(1)   Y        '0'
              EXT_FLAG2,--       VARCHAR2(1)   Y        '0'
              PACT_UNIT_FLAG,--       VARCHAR2(1)   Y        '0'
              PACKAGE_CODE,--    VARCHAR2(12)  Y                复合项目代码
              PACKAGE_NAME,--    VARCHAR2(12)  Y                复合项目名称
              NOBACK_NUM,--      NUMBER(7,2)   Y                可退数量
              CONFIRM_NUM ,
       CONFIRM_INJECT,
              MO_ORDER,
              RECIPE_SEQ,
              m.ECO_COST,
             OVER_COST,
              EXCESS_COST,
              DRUG_OWNCOST,
              COST_SOURCE,
              SUBJOB_FLAG,
              ACCOUNT_FLAG,
              UPDATE_SEQUENCENO,
              m.PAYKIND_CODE, --77
              m.PACT_CODE,
              old_unit_price,
              package_qty,
              recipe_memo,
              memo,                  --82
              DOCTINDEPT,
              MEDICALGROUPCODE,--84
              EXT_FLAG3,
              Extend_Flag,
              nvl((select PREOUT_SUM  from  V_HTS_GETSTORE sre WHERE  sre.clinic_code=m.clinic_code and sre.ITEM_CODE=M.ITEM_CODE  AND ROWNUM=1 ),'1')PREOUT_SUM,
              recipe_flag
  from fin_opb_feedetail m
  inner join fin_opr_register reg on reg.clinic_code=m.clinic_code and reg.valid_flag='1'
  where 1=1
  and pay_flag = '0'
  --and  nvl(m.extend_flag,'0')<>'1'
  and  nvl((select qq.extend_flag from met_ord_recipedetail qq where qq.sequence_no=m.mo_order and qq.clinic_code=m.clinic_code and rownum=1),nvl(m.extend_flag,'0'))<>'1'
  and m.clinic_code='{0}' 
  --and m.EXEC_DPCD != '7017' --屏蔽输血科
  --and (m.PUB_COST + m.PAY_COST + m.OWN_COST) > 0 
  and not exists (select 1 from fin_opb_feedetail a where a.clinic_code=m.clinic_code and a.pay_flag = '0' and a.hos_code!='CORE_HIS50' and pay_flag = '0' ) -- 屏蔽校区
  and m.RECIPE_FLAG not in ('5')--屏蔽日间预入院
  and reg.pact_code <> '258'
AND NOT EXISTS (SELECT 1
          FROM met_ord_recipedetail t
         WHERE t.sequence_no = m.mo_order
           AND t.clinic_code = m.clinic_code
           AND t.drug_flag = '1'
           AND t.unit_price = 0) --屏蔽0元药品";
                Neusoft.HISFC.Models.Registration.Register reg = null; //患者基本信息
                reg = registerManager.GetByClinic(visitNo);
                //获取挂号的未收费项目信息
                RegisterInfoManager registerInfoManager = new RegisterInfoManager();
                ArrayList al = outpatientManager.QueryFeeDetailBySql(getFeeSql, visitNo);
                string errMsg = "";
                ArrayList comFeeItemLists = registerInfoManager.GetFeeItemList(al, reg, ref errMsg,true);
                if (comFeeItemLists == null || comFeeItemLists.Count <= 0)
                {
                    throw new Exception("您暂时无缴费信息!" + errMsg);
                }
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList item in comFeeItemLists)
                {
                    string prescriptionType = "1";
                    if (item.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
                    {
                        prescriptionType = "0";
                        item.IsStore = 1;
                    }
                    decimal selfAmt = (item.FT.OwnCost + item.FT.PubCost + item.FT.PayCost) * 100;
                    DataRow row = dataResdt.NewRow();
                    row["prescriptionId"] = item.Item.ID;
                    row["prescriptionType"] = prescriptionType;
                    row["selfAmt"] = ((int)Math.Round(selfAmt, 0)).ToString();
                    row["itemName"] = item.Item.Name;
                    row["RECIPE_NO"] = item.RecipeNO;
                    row["isStore"] = item.IsStore;
                    row["reciptType"] = item.RecipeFlag;
                    dataResdt.Rows.Add(row);
                }
                return dataResdt;
            }
            catch (Exception e)
            {
                erro = e.Message.ToString();
                return null;
            }
        }

        public DataTable billDetail(string visitNo, ref string erro)
        {
            erro = "";
            try
            {
                FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
                #region sql
                string sql = @"select 
m.fee_code feeType ,--费用分类编码
m.recipe_no|| m.SEQUENCE_NO itemId,--收费项目流水号
dic.name feeName,--费用分类名称
m.item_name itemName,--项目名称 
m.PRICE_UNIT unit,--项目单位 
m.QTY count,--项目数量 
m.UNIT_PRICE*100 price,--项目单价
m.SPECS spece,--项目规格
(m.PUB_COST + m.PAY_COST + m.OWN_COST)*100 amount--项目总金额
from fin_opb_feedetail m
left join com_dictionary dic on dic.type='MINFEE' and m.fee_code=dic.code
where 1=1
and pay_flag = '0'
and  nvl(m.extend_flag,'0')='0'
and m.clinic_code='{0}'
";
                sql = string.Format(sql, visitNo);
                #endregion
                DataSet ds = null;
                if (mgr.ExecQuery(sql, ref ds) <= 0)
                {
                    throw new Exception(mgr.Err);
                }
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("未找到数据！");
                }
                if (ds.Tables[0] == null)
                {
                    throw new Exception("未找到数据！");
                }
                DataTable resdt = ds.Tables[0];
                return resdt;
            }
            catch (Exception e)
            {
                erro = e.Message.ToString();
                return null;
            }
        }

        public FS.ZDWY.Internet.Models.Views.ComResult<FS.ZDWY.Internet.Models.PLATFORM_BALANCE_PAY> GetBillPayInfo(string orderid)
        {
            FS.ZDWY.Internet.Models.Views.ComResult<FS.ZDWY.Internet.Models.PLATFORM_BALANCE_PAY> result = new Models.Views.ComResult<Models.PLATFORM_BALANCE_PAY>();
            try
            {
                
                FS.ZDWY.Internet.BL.OutPatient.PlatformBillLogic billlogic = new BL.OutPatient.PlatformBillLogic();
                FS.ZDWY.Internet.Models.PLATFORM_BALANCE_PAY res = billlogic.Get(orderid);
                if (res != null)
                {
                    result.IsSuccessful = true;
                    result.ReturnData = res;
                    return result;
                }
                else
                {
                    throw new Exception("未找到信息！");
                }
            }
            catch (Exception e)
            {
                result.IsSuccessful = false;
                result.Message = e.Message.ToString();
                return result;
            }

        }
        /// <summary>
        /// 获取电子票信息
        /// </summary>
        /// <param name="CLINIC_CODE">票据号</param>
        /// <param name="Type">票据类型</param>
        /// <param name="erro"></param>
        /// <returns></returns>
        public DataTable GetElecBillInfo(string CLINIC_CODE, string Type, ref string erro)
        {
            erro = "";
            try
            {
                FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
                #region sql
                string sql = @"select e.billqrcode,--二维码图片数据
e.pictureurl,--内网地址
e.pictureneturl,--外网地址
e.random--校验码
from Elec_OutPatientRecord e where e.clinic_code = '{0}' and BILLTYPE = '{1}' and STATE = '0'";
                sql = string.Format(sql, CLINIC_CODE, Type);
                #endregion
                DataSet ds = null;
                if (mgr.ExecQuery(sql, ref ds) <= 0)
                {
                    return null;
                }
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                if (ds.Tables[0] == null)
                {
                    return null;
                }
                DataTable resdt = ds.Tables[0];
                return resdt;
            }
            catch (Exception e)
            {
                erro = e.Message.ToString();
                return null;
            }
        }

        public DataTable billPayDetail(string visitNo, string hospTradeId, ref string erro)
        {
            erro = "";
            try
            {
                FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
                #region sql
                string sql = @"select 
m.fee_code feeType ,--费用分类编码
m.recipe_no|| m.SEQUENCE_NO itemId,--收费项目流水号
dic.name feeName,--费用分类名称
m.item_name itemName,--项目名称 
--m.PRICE_UNIT unit,--项目单位 
nvl(dr.min_unit,m.dose_unit) unit,--项目单位 
m.QTY count,--项目数量 
m.UNIT_PRICE*100 price,--项目单价
m.SPECS spece,--项目规格
(m.PUB_COST + m.PAY_COST + m.OWN_COST)*100 amount--项目总金额
from fin_opb_feedetail m
left join com_dictionary dic on dic.type='MINFEE' and m.fee_code=dic.code
LEFT JOIN pha_com_baseinfo dr ON dr.drug_code=m.item_code
where 1=1
and pay_flag = '1'
and trans_type='1'
and clinic_code='{0}'
and invoice_no='{1}'
";
                sql = string.Format(sql, visitNo, hospTradeId);
                #endregion
                DataSet ds = null;
                if (mgr.ExecQuery(sql, ref ds) <= 0)
                {
                    throw new Exception(mgr.Err);
                }
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("未找到数据！");
                }
                if (ds.Tables[0] == null)
                {
                    throw new Exception("未找到数据！");
                }
                DataTable resdt = ds.Tables[0];
                return resdt;
            }
            catch (Exception e)
            {
                erro = e.Message.ToString();
                return null;
            }
        }

        public string GetDeptNameForCode(string dept_code)
        {
            FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
            string strSql = @" select p.dept_name from com_department p where p.dept_code='{0}'";
            strSql = string.Format(strSql, dept_code);

            try
            {
                string result = mgr.ExecSqlReturnOne(strSql);
                if (result == "-1")
                {
                    return string.Empty;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public string GetEmplName(string code)
        {
            try
            {
                Neusoft.HISFC.BizLogic.Manager.Person personMgr = new Neusoft.HISFC.BizLogic.Manager.Person();
                return personMgr.GetPersonByID(code).Name;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 其他

        public DataTable BookRemind()
        {
            FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic plalogic = new BL.OutPatient.PlatformOrderLogic();
            return plalogic.BookRemind();
        }

        public DataTable StopSchedulRemind()
        {
            FS.ZDWY.Internet.BL.OutPatient.PlatformOrderLogic plalogic = new BL.OutPatient.PlatformOrderLogic();
            return plalogic.StopSchedulRemind();
        }
        #endregion


        #region 获取在自助设备上不能缴费的项目
        public System.Collections.Hashtable getItemListWipeOffZZSB()
        {
            string strSql = string.Empty;

            FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
            strSql = @"select p.code,p.name from  com_dictionary p where p.type='ZZSBNOPayCTDRItem'";
            System.Collections.Hashtable hsLimit = new System.Collections.Hashtable();
            System.Collections.ArrayList alLimit = new System.Collections.ArrayList();

            try
            {


                mgr.ExecQuery(strSql);
                while (mgr.Reader.Read())
                {
                    Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = mgr.Reader[0].ToString();//项目编码
                    obj.Name = mgr.Reader[1].ToString();//项目名称
                    alLimit.Add(obj);
                }

            }
            catch
            {
            }

            if (alLimit != null && alLimit.Count > 0)
            {
                foreach (Neusoft.FrameWork.Models.NeuObject dic in alLimit)
                {
                    if (hsLimit.ContainsKey(dic.ID))
                    {
                        continue;
                    }
                    else
                    {
                        hsLimit.Add(dic.ID, dic);
                    }
                }
            }
            return hsLimit;
        }
        #endregion

        #region 是否有处方类型为空
        /// <summary>
        /// 是否有处方类型为空 返回 false 就不给结算
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public bool RecipeFlagIsNull(string clinicCode)
        {
            string strSql = string.Empty;

            FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
            strSql = @"select count(1) from fin_opb_feedetail p where p.clinic_code ='{0}' and p.pay_flag = '0' and p.recipe_flag is null ";

            try
            {
                int count = 0;
                strSql = string.Format(strSql, clinicCode);
                mgr.ExecQuery(strSql);
                while (mgr.Reader.Read())
                {
                    count = int.Parse(mgr.Reader[0].ToString());
                    break;
                }
                if (count > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region 验证缴费数据是否符合折价条件
        /// <summary>
        /// 验证缴费数据是否符合折价条件 返回true为会折价，这时不能收费，提示去窗口
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public bool ShouldTriggerPriceReduction(string cardNo,string clinicCode)
        {
            //按项目维护数量折价
            string strSql = string.Empty;
            FS.ZDWY.Internet.BP.OutPatient.Register.Manager mgr = new Register.Manager();
            strSql = @"select count(0) from (
        select f.item_code, sum(f.qty) qty,to_number(d.input_code) input  from 
        (select z.item_code,f.qty*z.qty as qty from 
        fin_opb_feedetail f ,fin_com_undruginfo u,fin_com_undrugztinfo z  
        where f.clinic_code = '{1}' and f.item_code = u.item_code and u.unitflag = '1' and z.package_code = f.item_code and f.pay_flag = '0'     
        union all
        select f.item_code,f.qty from fin_opb_feedetail f ,fin_com_undruginfo u
        where  f.clinic_code = '{1}' and f.item_code = u.item_code and u.unitflag = '0'and f.pay_flag = '0'
        union all
        select f.item_code,f.qty from fin_opb_feedetail f ,fin_com_undruginfo u
        where  f.card_no = '{0}' and f.item_code = u.item_code and  f.pay_flag <> '0' and  f.CANCEL_FLAG <> '0'
        and f.fee_date >= SYSDATE - INTERVAL '2' HOUR
        ) f,com_dictionary d where d.TYPE='Restrictingfee' and d.code = f.item_code 
        group by    f.item_code,d.input_code ) s where s.qty > input";
            try
            {
                int count = 0;
                strSql = string.Format(strSql, cardNo, clinicCode);
                mgr.ExecQuery(strSql);
                while (mgr.Reader.Read())
                {
                    count = int.Parse(mgr.Reader[0].ToString());
                    break;
                }
                if (count > 0)
                    return true;
            }
            catch 
            {
                throw;
            }

            //胎心
            strSql = @"select count(0) as co from ( 
        select f.item_code  from 
  (select z.item_code,f.qty*z.qty as qty from 
        fin_opb_feedetail f ,fin_com_undruginfo u,fin_com_undrugztinfo z  
        where f.clinic_code = '{1}' and f.item_code = u.item_code and u.unitflag = '1' and z.package_code = f.item_code and f.pay_flag = '0'     
        union all
        select f.item_code,f.qty from fin_opb_feedetail f ,fin_com_undruginfo u
        where  f.clinic_code = '{1}' and f.item_code = u.item_code and u.unitflag = '0'and f.pay_flag = '0'
        union all
        select f.item_code,f.qty from fin_opb_feedetail f ,fin_com_undruginfo u
        where  f.CARD_NO = '{1}' and f.item_code = u.item_code and  f.pay_flag <> '0' and  f.CANCEL_FLAG <> '0'
        and f.fee_date >= SYSDATE - INTERVAL '2' HOUR
        )f,com_dictionary d where d.TYPE='RestrictingfeeTX' and d.code = f.item_code ) s";
            try
            {
                int count = 0;
                strSql = string.Format(strSql, cardNo, clinicCode);
                mgr.ExecQuery(strSql);
                while (mgr.Reader.Read())
                {
                    count = int.Parse(mgr.Reader[0].ToString());
                    break;
                }
                if (count > 1)
                    return true;
            }
            catch
            {
                throw;
            }

            //床旁
            strSql = @"select count(0) as co from ( 
        select f.item_code  from 
  (select z.item_code,f.qty*z.qty as qty from 
        fin_opb_feedetail f ,fin_com_undruginfo u,fin_com_undrugztinfo z  
        where f.clinic_code = '{1}' and f.item_code = u.item_code and u.unitflag = '1' and z.package_code = f.item_code and f.pay_flag = '0'     
        union all
        select f.item_code,f.qty from fin_opb_feedetail f ,fin_com_undruginfo u
        where  f.clinic_code = '{1}' and f.item_code = u.item_code and u.unitflag = '0'and f.pay_flag = '0'
        union all
        select f.item_code,f.qty from fin_opb_feedetail f ,fin_com_undruginfo u
        where  f.CARD_NO = '{1}' and f.item_code = u.item_code and  f.pay_flag <> '0' and  f.CANCEL_FLAG <> '0'
        and f.fee_date >= SYSDATE - INTERVAL '2' HOUR
        )f,com_dictionary d where d.TYPE='RestrictingfeeCP' and d.code = f.item_code ) s";
            try
            {
                int count = 0;
                strSql = string.Format(strSql, cardNo, clinicCode);
                mgr.ExecQuery(strSql);
                while (mgr.Reader.Read())
                {
                    count = int.Parse(mgr.Reader[0].ToString());
                    break;
                }
                if (count > 1)
                    return true;
            }
            catch
            {
                throw;
            }
            return false;
        }
        #endregion
        public DateTime GetDateTimeFromSysDateTime()
        {
            return outpatientManager.GetDateTimeFromSysDateTime();
        }
    }
}
