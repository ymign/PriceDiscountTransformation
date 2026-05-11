using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.MedicalTraceCode;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Pharmacy;
using Neusoft.HISFC.Models.SqlSugar;

namespace Neusoft.HISFC.BizLogic.Pharmacy
{
    public class TraceCodeDAL : Neusoft.FrameWork.Management.Database
    {

        /// <summary>
        /// 获取发药申请的追溯码采集状态
        /// </summary>
        /// <param name="applyNumber">发药申请流水号</param>
        /// <returns></returns>
        public string GetApplyOutTraceCodeCollectionStatus(string applyNumber)
        {

            var sql = @" select p.tracecodecollectionstatus from pha_com_applyout p where p.apply_number='{0}' ";
            sql = string.Format(sql, applyNumber);

            var status = this.ExecSqlReturnOne(sql, "");
            return status;

        }

        /// <summary>
        /// 获取发药信息实体
        /// </summary>
        /// <param name="applyNumber">pha_com_applyout表主键apply_number</param>
        /// <returns></returns>
        public PhaComApplyout GetApplyInfo(string applyNumber)
        {
            try
            {
                #region sql

                var sql = @" select p.apply_number,
       p.dept_code,
       p.drug_dept_code,
       p.class3_meaning_code,
       p.group_code,
       p.drug_code,
       p.trade_name,
       p.batch_no,
       p.drug_type,
       p.drug_quality,
       p.specs,
       p.pack_unit,
       p.pack_qty,
       p.min_unit,
       p.show_flag,
       p.show_unit,
       p.retail_price,
       p.wholesale_price,
       p.purchase_price,
       p.apply_billcode,
       p.apply_opercode,
       p.apply_date,
       p.apply_state,
       p.apply_num,
       p.days,
       p.preout_flag,
       p.charge_flag,
       p.patient_id,
       p.patient_dept,
       p.druged_bill,
       p.druged_dept,
       p.druged_empl,
       p.druged_date,
       p.druged_num,
       p.dose_once,
       p.dose_unit,
       p.usage_code,
       p.use_name,
       p.dfq_freq,
       p.dfq_cexp,
       p.dose_model_code,
       p.order_type,
       p.mo_order,
       p.comb_no,
       p.exec_sqn,
       p.recipe_no,
       p.sequence_no,
       p.send_type,
       p.billclass_code,
       p.print_state,
       p.relieve_flag,
       p.relieve_code,
       p.print_empl,
       p.print_date,
       p.out_bill_code,
       p.valid_state,
       p.mark,
       p.cancel_empl,
       p.cancel_date,
       p.place_code,
       p.recipe_dept,
       p.recipe_oper,
       p.baby_flag,
       p.ext_flag,
       p.ext_flag1,
       p.compound_group,
       p.compound_flag,
       p.compound_exec,
       p.compound_oper,
       p.compound_date,
       p.execseqall,
       p.pkstatus,
       p.smrecipe_no,
       p.tracecodecollectionstatus,
       p.alreadycollectqty,
       p.needcollectqty,
       p.appealcollectqty,
       p.alreadycollectspiltqty,
       p.needcollectspiltqty,
       p.appealcollectspiltqty,
       p.needcollecttracecodeflag,
       p.notcollecttracecodereason from pha_com_applyout p where p.apply_number='{0}' ";

                #endregion

                sql = string.Format(sql, applyNumber);

                this.ExecQuery(sql);

                PhaComApplyout info = new PhaComApplyout();

                while (this.Reader.Read())
                {
                    var i = 0;
                    info.ApplyNumber = NConvert.ToDecimal(this.Reader[i].ToString()); i++;//0
                    info.DeptCode = this.Reader[i].ToString(); i++;
                    info.DrugDeptCode = this.Reader[i].ToString(); i++;
                    info.Class3MeaningCode = this.Reader[i].ToString(); i++;
                    info.GroupCode = this.Reader[i].ToString(); i++;
                    info.DrugCode = this.Reader[i].ToString(); i++;
                    info.TradeName = this.Reader[i].ToString(); i++;
                    info.BatchNo = this.Reader[i].ToString(); i++;
                    info.DrugType = this.Reader[i].ToString(); i++;
                    info.DrugQuality = this.Reader[i].ToString(); i++;
                    info.Specs = this.Reader[i].ToString(); i++;//10
                    info.PackUnit = this.Reader[i].ToString(); i++;
                    info.PackQty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.MinUnit = this.Reader[i].ToString(); i++;
                    info.ShowFlag = this.Reader[i].ToString(); i++;
                    info.ShowUnit = this.Reader[i].ToString(); i++;
                    info.RetailPrice = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.WholesalePrice = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.PurchasePrice = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.ApplyBillcode = this.Reader[i].ToString(); i++;
                    info.ApplyOpercode = this.Reader[i].ToString(); i++;//20
                    info.ApplyDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.ApplyState = this.Reader[i].ToString(); i++;
                    info.ApplyNum = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.Days = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.PreoutFlag = this.Reader[i].ToString(); i++;
                    info.ChargeFlag = this.Reader[i].ToString(); i++;
                    info.PatientId = this.Reader[i].ToString(); i++;
                    info.PatientDept = this.Reader[i].ToString(); i++;
                    info.DrugedBill = this.Reader[i].ToString(); i++;
                    info.DrugedDept = this.Reader[i].ToString(); i++;//30
                    info.DrugedEmpl = this.Reader[i].ToString(); i++;
                    info.DrugedDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.DrugedNum = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.DoseOnce = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.DoseUnit = this.Reader[i].ToString(); i++;
                    info.UsageCode = this.Reader[i].ToString(); i++;
                    info.UseName = this.Reader[i].ToString(); i++;
                    info.DfqFreq = this.Reader[i].ToString(); i++;
                    info.DfqCexp = this.Reader[i].ToString(); i++;
                    info.DoseModelCode = this.Reader[i].ToString(); i++;//40
                    info.OrderType = this.Reader[i].ToString(); i++;
                    info.MoOrder = this.Reader[i].ToString(); i++;
                    info.CombNo = this.Reader[i].ToString(); i++;
                    info.ExecSqn = this.Reader[i].ToString(); i++;
                    info.RecipeNo = this.Reader[i].ToString(); i++;
                    info.SequenceNo = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.SendType = this.Reader[i].ToString(); i++;
                    info.BillclassCode = this.Reader[i].ToString(); i++;
                    info.PrintState = this.Reader[i].ToString(); i++;
                    info.RelieveFlag = this.Reader[i].ToString(); i++;//50
                    info.RelieveCode = this.Reader[i].ToString(); i++;
                    info.PrintEmpl = this.Reader[i].ToString(); i++;
                    info.PrintDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.OutBillCode = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.ValidState = this.Reader[i].ToString(); i++;
                    info.Mark = this.Reader[i].ToString(); i++;
                    info.CancelEmpl = this.Reader[i].ToString(); i++;
                    info.CancelDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.PlaceCode = this.Reader[i].ToString(); i++;
                    info.RecipeDept = this.Reader[i].ToString(); i++;//60
                    info.RecipeOper = this.Reader[i].ToString(); i++;
                    info.BabyFlag = this.Reader[i].ToString(); i++;
                    info.ExtFlag = this.Reader[i].ToString(); i++;
                    info.ExtFlag1 = this.Reader[i].ToString(); i++;
                    info.CompoundGroup = this.Reader[i].ToString(); i++;
                    info.CompoundFlag = this.Reader[i].ToString(); i++;
                    info.CompoundExec = this.Reader[i].ToString(); i++;
                    info.CompoundOper = this.Reader[i].ToString(); i++;
                    info.CompoundDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.Execseqall = this.Reader[i].ToString(); i++;//70
                    info.Pkstatus = this.Reader[i].ToString(); i++;
                    info.SmrecipeNo = this.Reader[i].ToString(); i++;
                    info.Tracecodecollectionstatus = this.Reader[i].ToString(); i++;
                    info.Alreadycollectqty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.Needcollectqty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.Appealcollectqty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.AlreadyCollectSpiltQty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.NeedCollectSpiltQty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.AppealCollectSpiltQty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.NeedCollectTraceCodeFlag = this.Reader[i].ToString(); i++;
                    info.NotCollectTraceCodeReason = this.Reader[i].ToString(); i++;

                    break;
                }
                return info;
            }
            catch (Exception ex)
            {
                this.Err = "查询发药信息实体出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        /// <summary>
        /// 获取发药信息实体
        /// </summary>
        /// <param name="applyNumber"></param>
        /// <returns></returns>
        public PhaComApplyout GetApplyInfo(string clincCode, string recipeNo, string sequenceNo)
        {
            try
            {
                #region sql

                var sql = @" select p.apply_number,
       p.dept_code,
       p.drug_dept_code,
       p.class3_meaning_code,
       p.group_code,
       p.drug_code,
       p.trade_name,
       p.batch_no,
       p.drug_type,
       p.drug_quality,
       p.specs,
       p.pack_unit,
       p.pack_qty,
       p.min_unit,
       p.show_flag,
       p.show_unit,
       p.retail_price,
       p.wholesale_price,
       p.purchase_price,
       p.apply_billcode,
       p.apply_opercode,
       p.apply_date,
       p.apply_state,
       p.apply_num,
       p.days,
       p.preout_flag,
       p.charge_flag,
       p.patient_id,
       p.patient_dept,
       p.druged_bill,
       p.druged_dept,
       p.druged_empl,
       p.druged_date,
       p.druged_num,
       p.dose_once,
       p.dose_unit,
       p.usage_code,
       p.use_name,
       p.dfq_freq,
       p.dfq_cexp,
       p.dose_model_code,
       p.order_type,
       p.mo_order,
       p.comb_no,
       p.exec_sqn,
       p.recipe_no,
       p.sequence_no,
       p.send_type,
       p.billclass_code,
       p.print_state,
       p.relieve_flag,
       p.relieve_code,
       p.print_empl,
       p.print_date,
       p.out_bill_code,
       p.valid_state,
       p.mark,
       p.cancel_empl,
       p.cancel_date,
       p.place_code,
       p.recipe_dept,
       p.recipe_oper,
       p.baby_flag,
       p.ext_flag,
       p.ext_flag1,
       p.compound_group,
       p.compound_flag,
       p.compound_exec,
       p.compound_oper,
       p.compound_date,
       p.execseqall,
       p.pkstatus,
       p.smrecipe_no,
       p.tracecodecollectionstatus,
       p.alreadycollectqty,
       p.needcollectqty,
       p.appealcollectqty,
       p.alreadycollectspiltqty,
       p.needcollectspiltqty,
       p.appealcollectspiltqty,
       p.needcollecttracecodeflag,
       p.notcollecttracecodereason from pha_com_applyout p where p.patient_id='{0}' and p.recipe_no='{1}' and p.sequence_no='{2}' ";

                #endregion

                sql = string.Format(sql, clincCode, recipeNo, sequenceNo);

                this.ExecQuery(sql);

                PhaComApplyout info = new PhaComApplyout();

                while (this.Reader.Read())
                {
                    var i = 0;
                    info.ApplyNumber = NConvert.ToDecimal(this.Reader[i].ToString()); i++;//0
                    info.DeptCode = this.Reader[i].ToString(); i++;
                    info.DrugDeptCode = this.Reader[i].ToString(); i++;
                    info.Class3MeaningCode = this.Reader[i].ToString(); i++;
                    info.GroupCode = this.Reader[i].ToString(); i++;
                    info.DrugCode = this.Reader[i].ToString(); i++;
                    info.TradeName = this.Reader[i].ToString(); i++;
                    info.BatchNo = this.Reader[i].ToString(); i++;
                    info.DrugType = this.Reader[i].ToString(); i++;
                    info.DrugQuality = this.Reader[i].ToString(); i++;
                    info.Specs = this.Reader[i].ToString(); i++;//10
                    info.PackUnit = this.Reader[i].ToString(); i++;
                    info.PackQty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.MinUnit = this.Reader[i].ToString(); i++;
                    info.ShowFlag = this.Reader[i].ToString(); i++;
                    info.ShowUnit = this.Reader[i].ToString(); i++;
                    info.RetailPrice = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.WholesalePrice = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.PurchasePrice = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.ApplyBillcode = this.Reader[i].ToString(); i++;
                    info.ApplyOpercode = this.Reader[i].ToString(); i++;//20
                    info.ApplyDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.ApplyState = this.Reader[i].ToString(); i++;
                    info.ApplyNum = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.Days = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.PreoutFlag = this.Reader[i].ToString(); i++;
                    info.ChargeFlag = this.Reader[i].ToString(); i++;
                    info.PatientId = this.Reader[i].ToString(); i++;
                    info.PatientDept = this.Reader[i].ToString(); i++;
                    info.DrugedBill = this.Reader[i].ToString(); i++;
                    info.DrugedDept = this.Reader[i].ToString(); i++;//30
                    info.DrugedEmpl = this.Reader[i].ToString(); i++;
                    info.DrugedDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.DrugedNum = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.DoseOnce = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.DoseUnit = this.Reader[i].ToString(); i++;
                    info.UsageCode = this.Reader[i].ToString(); i++;
                    info.UseName = this.Reader[i].ToString(); i++;
                    info.DfqFreq = this.Reader[i].ToString(); i++;
                    info.DfqCexp = this.Reader[i].ToString(); i++;
                    info.DoseModelCode = this.Reader[i].ToString(); i++;//40
                    info.OrderType = this.Reader[i].ToString(); i++;
                    info.MoOrder = this.Reader[i].ToString(); i++;
                    info.CombNo = this.Reader[i].ToString(); i++;
                    info.ExecSqn = this.Reader[i].ToString(); i++;
                    info.RecipeNo = this.Reader[i].ToString(); i++;
                    info.SequenceNo = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.SendType = this.Reader[i].ToString(); i++;
                    info.BillclassCode = this.Reader[i].ToString(); i++;
                    info.PrintState = this.Reader[i].ToString(); i++;
                    info.RelieveFlag = this.Reader[i].ToString(); i++;//50
                    info.RelieveCode = this.Reader[i].ToString(); i++;
                    info.PrintEmpl = this.Reader[i].ToString(); i++;
                    info.PrintDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.OutBillCode = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.ValidState = this.Reader[i].ToString(); i++;
                    info.Mark = this.Reader[i].ToString(); i++;
                    info.CancelEmpl = this.Reader[i].ToString(); i++;
                    info.CancelDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.PlaceCode = this.Reader[i].ToString(); i++;
                    info.RecipeDept = this.Reader[i].ToString(); i++;//60
                    info.RecipeOper = this.Reader[i].ToString(); i++;
                    info.BabyFlag = this.Reader[i].ToString(); i++;
                    info.ExtFlag = this.Reader[i].ToString(); i++;
                    info.ExtFlag1 = this.Reader[i].ToString(); i++;
                    info.CompoundGroup = this.Reader[i].ToString(); i++;
                    info.CompoundFlag = this.Reader[i].ToString(); i++;
                    info.CompoundExec = this.Reader[i].ToString(); i++;
                    info.CompoundOper = this.Reader[i].ToString(); i++;
                    info.CompoundDate = NConvert.ToDateTime(this.Reader[i]); i++;
                    info.Execseqall = this.Reader[i].ToString(); i++;//70
                    info.Pkstatus = this.Reader[i].ToString(); i++;
                    info.SmrecipeNo = this.Reader[i].ToString(); i++;
                    info.Tracecodecollectionstatus = this.Reader[i].ToString(); i++;
                    info.Alreadycollectqty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.Needcollectqty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.Appealcollectqty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.AlreadyCollectSpiltQty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.NeedCollectSpiltQty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.AppealCollectSpiltQty = NConvert.ToDecimal(this.Reader[i]); i++;
                    info.NeedCollectTraceCodeFlag = this.Reader[i].ToString(); i++;
                    info.NotCollectTraceCodeReason = this.Reader[i].ToString(); i++;

                    break;
                }
                return info;
            }
            catch (Exception ex)
            {
                this.Err = "查询发药信息实体出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        public PatientAndApplyInfo GetPatientAndApplyInfo(string applyNumber)
        {
            try
            {
                var sql = string.Format(@" select 
p.apply_number,
a.name,
a.card_no,
a.patient_no,
p.recipe_no,
fun_get_dept_name(p.drug_dept_code) drug_dept_name,
fun_get_dept_name(p.dept_code) dept_name,
fun_get_dept_name(p.recipe_dept) recipe_dept_name,
fun_get_employee_name(p.recipe_oper) recipe_oper_name,
decode(a.sex_code,'F','女','M','男','未知') sex,
p.drug_dept_code,
p.dept_code,
p.recipe_dept,
p.recipe_oper
from pha_com_applyout p left join fin_ipr_inmaininfo a on p.patient_id=a.inpatient_no where p.apply_number='{0}' ", applyNumber);

                this.ExecQuery(sql);

                var info = new PatientAndApplyInfo();
                int i = 0;
                while (this.Reader.Read())
                {
                    i = 0;
                    info.ApplyNumber = this.Reader[i].ToString(); i++;
                    info.Name = this.Reader[i].ToString(); i++;
                    info.CardNo = this.Reader[i].ToString(); i++;
                    info.PatientNo = this.Reader[i].ToString(); i++;
                    info.RecipeNo = this.Reader[i].ToString(); i++;
                    info.DrugDeptName = this.Reader[i].ToString(); i++;
                    info.DeptName = this.Reader[i].ToString(); i++;
                    info.RecipeDeptName = this.Reader[i].ToString(); i++;
                    info.RecipeOperName = this.Reader[i].ToString(); i++;
                    info.Sex = this.Reader[i].ToString(); i++;

                    info.DrugDeptCode = this.Reader[i].ToString(); i++;
                    info.DeptCode = this.Reader[i].ToString(); i++;
                    info.RecipeDeptCode = this.Reader[i].ToString(); i++;
                    info.RecipeOperCode = this.Reader[i].ToString(); i++;
                    break;
                }
                return info;
            }
            catch (Exception ex)
            {
                this.Err = "医保追溯码:查询[GetPatientAndApplyInfo]异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        public PatientAndApplyInfo GetMZPatientAndApplyInfo(string applyNumber)
        {

            try
            {
                var sql = string.Format(@" select 
p.apply_number,
a.name,
a.card_no,
a.card_no,
p.recipe_no,
fun_get_dept_name(p.drug_dept_code) drug_dept_name,
fun_get_dept_name(p.dept_code) dept_name,
fun_get_dept_name(p.recipe_dept) recipe_dept_name,
fun_get_employee_name(p.recipe_oper) recipe_oper_name,
decode(a.sex_code,'F','女','M','男','未知') sex,
p.drug_dept_code,
p.dept_code,
p.recipe_dept,
p.recipe_oper
from pha_com_applyout p left join fin_opr_register a  on p.patient_id=a.clinic_code and rownum=1 where  p.apply_number='{0}' ", applyNumber);

                this.ExecQuery(sql);

                var info = new PatientAndApplyInfo();
                int i = 0;
                while (this.Reader.Read())
                {
                    i = 0;
                    info.ApplyNumber = this.Reader[i].ToString(); i++;
                    info.Name = this.Reader[i].ToString(); i++;
                    info.CardNo = this.Reader[i].ToString(); i++;
                    info.PatientNo = this.Reader[i].ToString(); i++;
                    info.RecipeNo = this.Reader[i].ToString(); i++;
                    info.DrugDeptName = this.Reader[i].ToString(); i++;
                    info.DeptName = this.Reader[i].ToString(); i++;
                    info.RecipeDeptName = this.Reader[i].ToString(); i++;
                    info.RecipeOperName = this.Reader[i].ToString(); i++;
                    info.Sex = this.Reader[i].ToString(); i++;

                    info.DrugDeptCode = this.Reader[i].ToString(); i++;
                    info.DeptCode = this.Reader[i].ToString(); i++;
                    info.RecipeDeptCode = this.Reader[i].ToString(); i++;
                    info.RecipeOperCode = this.Reader[i].ToString(); i++;
                    break;
                }
                return info;
            }
            catch (Exception ex)
            {
                this.Err = "医保追溯码:查询[GetPatientAndApplyInfo]异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        public Dictionary<string, HashSet<string>> GetDrugCodeToIdentifierCodesMap()
        {
            string sql = @" select p.id, p.drug_code, p.identifier_code
  from pha_com_CodeMapping p
 where p.valid_flag = '1' ";

            try
            {
                this.ExecQuery(sql);

                var map = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

                while (this.Reader.Read())
                {
                    var i = 0;

                    // 跳过 id
                    i++;

                    string drugCode = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;
                    string identifierCode = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;

                    if (string.IsNullOrEmpty(drugCode) || string.IsNullOrEmpty(identifierCode))
                        continue;

                    HashSet<string> idSet;
                    if (!map.TryGetValue(drugCode, out idSet))
                    {
                        idSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        map[drugCode] = idSet;
                    }
                    idSet.Add(identifierCode);
                }

                return map;
            }
            catch (Exception ex)
            {
                this.Err = "查询药品标识码对照关系出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        public Dictionary<string, HashSet<string>> GetDrugCodeToIdentifierCodesMap(List<string> drugCodes)
        {
            var map = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            if (drugCodes == null || drugCodes.Count == 0)
            {
                return map;
            }

            var inValues = string.Join("','", drugCodes
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c => c.Trim())
                .Where(c => c.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(c => c.Replace("'", "''"))
                .ToArray());

            if (string.IsNullOrEmpty(inValues))
            {
                return map;
            }

            try
            {
                string sql = @" select p.id, p.drug_code, p.identifier_code
    from pha_com_CodeMapping p
   where p.valid_flag = '1'
     and p.drug_code in ('{0}') ";

                sql = string.Format(sql, inValues);

                this.ExecQuery(sql);

                while (this.Reader.Read())
                {
                    var i = 0;

                    // 跳过 id
                    i++;

                    string drugCode = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;
                    string identifierCode = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;

                    if (string.IsNullOrEmpty(drugCode) || string.IsNullOrEmpty(identifierCode))
                        continue;

                    HashSet<string> idSet;
                    if (!map.TryGetValue(drugCode, out idSet))
                    {
                        idSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        map[drugCode] = idSet;
                    }
                    idSet.Add(identifierCode);
                }

                return map;
            }
            catch (Exception ex)
            {
                this.Err = "查询药品标识码对照关系出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }


        public Dictionary<string, PhaComBaseinfo> GetDrugBaseInfoMap()
        {
            string sql = @" select p.specs, p.pack_unit, p.pack_qty, p.min_unit, p.trade_name, p.drug_code,p.custom_code 
                     from pha_com_baseinfo p 
                    where p.valid_state = '1' ";

            try
            {
                this.ExecQuery(sql);
                var map = new Dictionary<string, PhaComBaseinfo>(StringComparer.OrdinalIgnoreCase);

                while (this.Reader.Read())
                {
                    var i = 0;

                    string specs = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;
                    string packUnit = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;
                    string packQty = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;
                    string minUnit = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;
                    string tradeName = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;
                    string drugCode = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;
                    string customCode = this.Reader.IsDBNull(i) ? null : this.Reader[i].ToString().Trim(); i++;

                    if (string.IsNullOrEmpty(drugCode))
                        continue;

                    var drugInfo = new PhaComBaseinfo
                    {
                        Specs = specs,
                        PackUnit = packUnit,
                        PackQty = NConvert.ToDecimal(packQty),
                        MinUnit = minUnit,
                        TradeName = tradeName,
                        DrugCode = drugCode,
                        CustomCode = customCode
                    };

                    map[drugCode] = drugInfo;
                }

                return map;
            }
            catch (Exception ex)
            {
                this.Err = "查询药品基础信息出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        public List<PhaComCodemapping> GetDrugCodeMappingList()
        {
            string sql = @" select p.id, p.drug_code, p.identifier_code
  from pha_com_CodeMapping p
 where p.valid_flag = '1' ";

            try
            {
                this.ExecQuery(sql);
                var drugMappingList = new List<PhaComCodemapping>();
                PhaComCodemapping drugMapInfo;

                while (Reader.Read())
                {
                    var i = 0;
                    drugMapInfo = new PhaComCodemapping();
                    drugMapInfo.Id = Reader[i].ToString(); i++;
                    drugMapInfo.DrugCode = Reader[i].ToString(); i++;
                    drugMapInfo.IdentifierCode = Reader[i].ToString(); i++;
                    drugMappingList.Add(drugMapInfo);
                }

                return drugMappingList;
            }
            catch (Exception ex)
            {
                this.Err = "查询药品标识码对照关系出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

        }

        public List<PhaComCodemapping> GetDrugCodeMappingList(string identifierCode)
        {
            string sql = @" select p.id, p.drug_code, p.identifier_code
  from pha_com_CodeMapping p
 where p.valid_flag = '1' and p.identifier_code='{0}' ";

            try
            {

                sql = string.Format(sql, identifierCode);

                this.ExecQuery(sql);
                var drugMappingList = new List<PhaComCodemapping>();
                PhaComCodemapping drugMapInfo;

                while (Reader.Read())
                {
                    var i = 0;
                    drugMapInfo = new PhaComCodemapping();
                    drugMapInfo.Id = Reader[i].ToString(); i++;
                    drugMapInfo.DrugCode = Reader[i].ToString(); i++;
                    drugMapInfo.IdentifierCode = Reader[i].ToString(); i++;
                    drugMappingList.Add(drugMapInfo);
                }

                return drugMappingList;
            }
            catch (Exception ex)
            {
                this.Err = "查询药品标识码对照关系出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

        }

        public YbTraceStock GetYbTraceStockInfo(string drugCode, string drugDeptCode)
        {
            try
            {
                #region SQL

                var sql = @" select p.id,
       p.drug_code,
       p.drug_name,
       p.drug_specs,
       p.drug_custom_code,
       p.drug_pact_unit,
       p.drug_pact_qty,
       p.drug_min_unit,
       p.drug_dept_code,
       p.drug_dept_name,
       p.total_qty,
       p.available_qty,
       p.prededucted_qty,
       p.expired_qty,
       p.damaged_qty,
       p.first_inbound_time,
       p.last_inbound_time,
       p.last_outbound_time,
       p.created_code,
       p.created_name,
       p.create_time,
       p.modified_code,
       p.modified_name,
       p.modified_time,
       p.is_deleted,
       p.is_valid,
       p.memo,
       p.backup_1,
       p.backup_2,
       p.backup_3 from yb_trace_stock p where p.is_deleted='N' and p.is_valid='Y' and p.drug_code='{0}' and p.drug_dept_code='{1}' ";

                #endregion

                sql = string.Format(sql, drugCode, drugDeptCode);

                this.ExecQuery(sql);

                var info = new YbTraceStock();

                while (Reader.Read())
                {
                    var i = 0;

                    info.Id = Reader[i].ToString(); i++;
                    info.DrugCode = Reader[i].ToString(); i++;
                    info.DrugName = Reader[i].ToString(); i++;
                    info.DrugSpecs = Reader[i].ToString(); i++;
                    info.DrugCustomCode = Reader[i].ToString(); i++;
                    info.DrugPactUnit = Reader[i].ToString(); i++;
                    info.DrugPactQty = Reader[i].ToString(); i++;
                    info.DrugMinUnit = Reader[i].ToString(); i++;
                    info.DrugDeptCode = Reader[i].ToString(); i++;
                    info.DrugDeptName = Reader[i].ToString(); i++;
                    info.TotalQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.AvailableQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.PreDeductedQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.ExpiredQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.DamagedQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.FirstInboundTime = NConvert.ToDateTime(Reader[i]); i++;
                    info.LastInboundTime = NConvert.ToDateTime(Reader[i]); i++;
                    info.LastOutboundTime = NConvert.ToDateTime(Reader[i]); i++;
                    info.CreatedCode = Reader[i].ToString(); i++;
                    info.CreatedName = Reader[i].ToString(); i++;
                    info.CreateTime = NConvert.ToDateTime(Reader[i]); i++;
                    info.ModifiedCode = Reader[i].ToString(); i++;
                    info.ModifiedName = Reader[i].ToString(); i++;
                    info.ModifiedTime = NConvert.ToDateTime(Reader[i]); i++;
                    info.IsDeleted = Reader[i].ToString(); i++;
                    info.IsValid = Reader[i].ToString(); i++;
                    info.Memo = Reader[i].ToString(); i++;
                    info.Backup1 = Reader[i].ToString(); i++;
                    info.Backup2 = Reader[i].ToString(); i++;
                    info.Backup3 = Reader[i].ToString(); i++;
                    break;
                }

                return info;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_stock]查询库存信息出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        /// <summary>
        /// 获取在库可用的拆零追溯码信息
        /// </summary>
        /// <param name="drugCode"></param>
        /// <param name="drugDeptCode"></param>
        /// <param name="needQty"></param>
        /// <returns></returns>
        public List<YbTraceStateRecord> GetInStockSplitTraceCodeList(
            string drugCode,
            string drugDeptCode,
            decimal needQty
            )
        {
            try
            {
                var sql = @" SELECT id, parent_trace_code, child_trace_code, sequence_no, create_time
  FROM (SELECT p.id,
               p.parent_trace_code,
               p.child_trace_code,
               p.sequence_no,
               p.create_time
          FROM yb_trace_state_record p
         WHERE drug_code = '{0}'
           AND drug_dept_code = '{1}'
           AND TRACE_STATUS = '0'
           AND is_valid = 'Y'
           AND is_deleted = 'N'
         ORDER BY CREATE_TIME ASC)
 WHERE ROWNUM <= {2}
 ";

                sql = string.Format(sql,
                    drugCode,
                    drugDeptCode,
                    needQty
                    );

                this.ExecQuery(sql);

                var list = new List<YbTraceStateRecord>();
                YbTraceStateRecord info;

                while (Reader.Read())
                {
                    var i = 0;
                    info = new YbTraceStateRecord();
                    info.Id = Reader[i].ToString(); i++;
                    info.ParentTraceCode = Reader[i].ToString(); i++;
                    info.ChildTraceCode = Reader[i].ToString(); i++;
                    info.SequenceNo = NConvert.ToDecimal(Reader[i]); i++;
                    info.CreateTime = NConvert.ToDateTime(Reader[i]); i++;

                    list.Add(info);
                }

                return list;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_state_record]查询拆零数据出现异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        public List<YbTraceInboundOrder> GetYbTraceInboundOrderListForDrugDeptCode(string drugDeptCode)
        {
            try
            {

                #region sql

                var sql = string.Format(@" select *
  from (select p.id,
               p.drug_dept_code,
               p.drug_dept_name,
               p.inbound_no,
               p.supplier_id,
               p.supplier_code,
               p.supplier_name,
               p.drug_code,
               p.drug_name,
               p.drug_specs,
               p.drug_custom_code,
               p.drug_pact_unit,
               p.drug_pact_qty,
               p.drug_min_unit,
               p.bch_no,
               p.manu_lotnum,
               p.manu_date,
               p.expy_end,
               p.original_trace_code,
               p.original_qty,
               p.split_qty,
               p.status,
               p.source_type,
               p.inbound_client_ip,
               p.inbound_oper_code,
               p.inbound_oper_name,
               p.inbound_oper_time,
               p.created_code,
               p.created_name,
               p.create_time
          from yb_trace_inbound_order p
         where p.is_deleted = 'N'
           and p.is_valid = 'Y'
           and p.drug_dept_code = '{0}'
         order by p.create_time desc)
 WHERE ROWNUM <= 200 ", drugDeptCode);

                #endregion

                this.ExecQuery(sql);

                var list = new List<YbTraceInboundOrder>();

                YbTraceInboundOrder inboundOrder;

                while (Reader.Read())
                {
                    var i = 0;
                    inboundOrder = new YbTraceInboundOrder();

                    inboundOrder.Id = Reader[i].ToString(); i++;
                    inboundOrder.DrugDeptCode = Reader[i].ToString(); i++;
                    inboundOrder.DrugDeptName = Reader[i].ToString(); i++;
                    inboundOrder.InboundNo = Reader[i].ToString(); i++;
                    inboundOrder.SupplierId = Reader[i].ToString(); i++;
                    inboundOrder.SupplierCode = Reader[i].ToString(); i++;
                    inboundOrder.SupplierName = Reader[i].ToString(); i++;
                    inboundOrder.DrugCode = Reader[i].ToString(); i++;
                    inboundOrder.DrugName = Reader[i].ToString(); i++;
                    inboundOrder.DrugSpecs = Reader[i].ToString(); i++;
                    inboundOrder.DrugCustomCode = Reader[i].ToString(); i++;
                    inboundOrder.DrugPactUnit = Reader[i].ToString(); i++;
                    inboundOrder.DrugPactQty = Reader[i].ToString(); i++;
                    inboundOrder.DrugMinUnit = Reader[i].ToString(); i++;
                    inboundOrder.BchNo = Reader[i].ToString(); i++;
                    inboundOrder.ManuLotnum = Reader[i].ToString(); i++;
                    inboundOrder.ManuDate = NConvert.ToDateTime(Reader[i].ToString()); i++;
                    inboundOrder.ExpyEnd = NConvert.ToDateTime(Reader[i].ToString()); i++;
                    inboundOrder.OriginalTraceCode = Reader[i].ToString(); i++;
                    inboundOrder.OriginalQty = NConvert.ToDecimal(Reader[i].ToString()); i++;
                    inboundOrder.SplitQty = NConvert.ToDecimal(Reader[i].ToString()); i++;
                    inboundOrder.Status = Reader[i].ToString(); i++;
                    inboundOrder.SourceType = Reader[i].ToString(); i++;
                    inboundOrder.InboundClientIp = Reader[i].ToString(); i++;
                    inboundOrder.InboundOperCode = Reader[i].ToString(); i++;
                    inboundOrder.InboundOperName = Reader[i].ToString(); i++;
                    inboundOrder.InboundOperTime = NConvert.ToDateTime(Reader[i].ToString()); i++;
                    inboundOrder.CreatedCode = Reader[i].ToString(); i++;
                    inboundOrder.CreatedName = Reader[i].ToString(); i++;
                    inboundOrder.CreateTime = NConvert.ToDateTime(Reader[i].ToString()); i++;

                    list.Add(inboundOrder);

                }

                return list;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_inbound_order]查询数据异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        public List<YbTraceInboundOrder> GetYbTraceInboundOrderListForInboundNo(string inboundNo)
        {
            try
            {

                #region sql

                var sql = string.Format(@"select p.id,
               p.drug_dept_code,
               p.drug_dept_name,
               p.inbound_no,
               p.supplier_id,
               p.supplier_code,
               p.supplier_name,
               p.drug_code,
               p.drug_name,
               p.drug_specs,
               p.drug_custom_code,
               p.drug_pact_unit,
               p.drug_pact_qty,
               p.drug_min_unit,
               p.bch_no,
               p.manu_lotnum,
               p.manu_date,
               p.expy_end,
               p.original_trace_code,
               p.original_qty,
               p.split_qty,
               p.status,
               p.source_type,
               p.inbound_client_ip,
               p.inbound_oper_code,
               p.inbound_oper_name,
               p.inbound_oper_time,
               p.created_code,
               p.created_name,
               p.create_time
          from yb_trace_inbound_order p
         where p.is_deleted = 'N'
           and p.is_valid = 'Y'
           and p.inbound_no = '{0}'
         ", inboundNo);

                #endregion

                this.ExecQuery(sql);

                var list = new List<YbTraceInboundOrder>();

                YbTraceInboundOrder inboundOrder;

                while (Reader.Read())
                {
                    var i = 0;
                    inboundOrder = new YbTraceInboundOrder();

                    inboundOrder.Id = Reader[i].ToString(); i++;
                    inboundOrder.DrugDeptCode = Reader[i].ToString(); i++;
                    inboundOrder.DrugDeptName = Reader[i].ToString(); i++;
                    inboundOrder.InboundNo = Reader[i].ToString(); i++;
                    inboundOrder.SupplierId = Reader[i].ToString(); i++;
                    inboundOrder.SupplierCode = Reader[i].ToString(); i++;
                    inboundOrder.SupplierName = Reader[i].ToString(); i++;
                    inboundOrder.DrugCode = Reader[i].ToString(); i++;
                    inboundOrder.DrugName = Reader[i].ToString(); i++;
                    inboundOrder.DrugSpecs = Reader[i].ToString(); i++;
                    inboundOrder.DrugCustomCode = Reader[i].ToString(); i++;
                    inboundOrder.DrugPactUnit = Reader[i].ToString(); i++;
                    inboundOrder.DrugPactQty = Reader[i].ToString(); i++;
                    inboundOrder.DrugMinUnit = Reader[i].ToString(); i++;
                    inboundOrder.BchNo = Reader[i].ToString(); i++;
                    inboundOrder.ManuLotnum = Reader[i].ToString(); i++;
                    inboundOrder.ManuDate = NConvert.ToDateTime(Reader[i].ToString()); i++;
                    inboundOrder.ExpyEnd = NConvert.ToDateTime(Reader[i].ToString()); i++;
                    inboundOrder.OriginalTraceCode = Reader[i].ToString(); i++;
                    inboundOrder.OriginalQty = NConvert.ToDecimal(Reader[i].ToString()); i++;
                    inboundOrder.SplitQty = NConvert.ToDecimal(Reader[i].ToString()); i++;
                    inboundOrder.Status = Reader[i].ToString(); i++;
                    inboundOrder.SourceType = Reader[i].ToString(); i++;
                    inboundOrder.InboundClientIp = Reader[i].ToString(); i++;
                    inboundOrder.InboundOperCode = Reader[i].ToString(); i++;
                    inboundOrder.InboundOperName = Reader[i].ToString(); i++;
                    inboundOrder.InboundOperTime = NConvert.ToDateTime(Reader[i].ToString()); i++;
                    inboundOrder.CreatedCode = Reader[i].ToString(); i++;
                    inboundOrder.CreatedName = Reader[i].ToString(); i++;
                    inboundOrder.CreateTime = NConvert.ToDateTime(Reader[i].ToString()); i++;

                    list.Add(inboundOrder);

                }

                return list;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_inbound_order]查询数据异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        public List<YbTraceInboundDetail> GetYbTraceInboundDetailForInboundNo(string inboundNo)
        {
            try
            {

                #region sql

                var sql = @" select p.id,
       p.inbound_id,
       p.drug_dept_code,
       p.drug_dept_name,
       p.inbound_no,
       p.supplier_id,
       p.supplier_code,
       p.supplier_name,
       p.drug_code,
       p.drug_name,
       p.drug_specs,
       p.drug_custom_code,
       p.drug_pact_unit,
       p.drug_pact_qty,
       p.drug_min_unit,
       p.bch_no,
       p.manu_lotnum,
       p.manu_date,
       p.expy_end,
       p.original_trace_code,
       p.original_qty,
       p.split_qty,
       p.parent_trace_code,
       p.child_trace_code,
       p.child_qty,
       p.child_sequence_no,
       p.status,
       p.source_type,
       p.inbound_client_ip,
       p.inbound_oper_code,
       p.inbound_oper_name,
       p.inbound_oper_time,
       p.created_code,
       p.created_name,
       p.create_time,
       p.modified_code,
       p.modified_name,
       p.modified_time,
       p.is_deleted,
       p.is_valid,
       p.memo,
       p.backup_1,
       p.backup_2,
       p.backup_3 from yb_trace_inbound_detail p where p.is_deleted='N' and p.is_valid='Y' and p.inbound_no='{0}' order by p.parent_trace_code,p.child_trace_code ";

                #endregion

                sql = string.Format(sql, inboundNo);

                this.ExecQuery(sql);

                var list = new List<YbTraceInboundDetail>();

                YbTraceInboundDetail info;

                while (Reader.Read())
                {
                    var i = 0;
                    info = new YbTraceInboundDetail();

                    info.Id = Reader[i].ToString(); i++;
                    info.InboundId = Reader[i].ToString(); i++;
                    info.DrugDeptCode = Reader[i].ToString(); i++;
                    info.DrugDeptName = Reader[i].ToString(); i++;
                    info.InboundNo = Reader[i].ToString(); i++;
                    info.SupplierId = Reader[i].ToString(); i++;
                    info.SupplierCode = Reader[i].ToString(); i++;
                    info.SupplierName = Reader[i].ToString(); i++;
                    info.DrugCode = Reader[i].ToString(); i++;
                    info.DrugName = Reader[i].ToString(); i++;
                    info.DrugSpecs = Reader[i].ToString(); i++;
                    info.DrugCustomCode = Reader[i].ToString(); i++;
                    info.DrugPactUnit = Reader[i].ToString(); i++;
                    info.DrugPactQty = Reader[i].ToString(); i++;
                    info.DrugMinUnit = Reader[i].ToString(); i++;
                    info.BchNo = Reader[i].ToString(); i++;
                    info.ManuLotnum = Reader[i].ToString(); i++;
                    info.ManuDate = NConvert.ToDateTime(Reader[i]); i++;
                    info.ExpyEnd = NConvert.ToDateTime(Reader[i]); i++;
                    info.OriginalTraceCode = Reader[i].ToString(); i++;
                    info.OriginalQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.SplitQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.ParentTraceCode = Reader[i].ToString(); i++;
                    info.ChildTraceCode = Reader[i].ToString(); i++;
                    info.ChildQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.ChildSequenceNo = NConvert.ToDecimal(Reader[i]); i++;
                    info.Status = Reader[i].ToString(); i++;
                    info.SourceType = Reader[i].ToString(); i++;
                    info.InboundClientIp = Reader[i].ToString(); i++;
                    info.InboundOperCode = Reader[i].ToString(); i++;
                    info.InboundOperName = Reader[i].ToString(); i++;
                    info.InboundOperTime = NConvert.ToDateTime(Reader[i]); i++;
                    info.CreatedCode = Reader[i].ToString(); i++;
                    info.CreatedName = Reader[i].ToString(); i++;
                    info.CreateTime = NConvert.ToDateTime(Reader[i]); i++;
                    info.ModifiedCode = Reader[i].ToString(); i++;
                    info.ModifiedName = Reader[i].ToString(); i++;
                    info.ModifiedTime = NConvert.ToDateTime(Reader[i]); i++;
                    info.IsDeleted = Reader[i].ToString(); i++;
                    info.IsValid = Reader[i].ToString(); i++;
                    info.Memo = Reader[i].ToString(); i++;
                    info.Backup1 = Reader[i].ToString(); i++;
                    info.Backup2 = Reader[i].ToString(); i++;
                    info.Backup3 = Reader[i].ToString(); i++;

                    list.Add(info);
                }

                return list;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_inbound_detail]查询数据异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

        }

        public List<YbTraceSeed> GetAvailableSeeds(
            string drugDeptCode,
            string drugCode)
        {

            try
            {

                var sql = @" select p.id,
       p.inbound_order_id,
       p.inbound_order_no,
       p.drug_code,
       p.drug_name,
       p.drug_dept_code,
       p.drug_dept_name,
       p.drug_pack_unit,
       p.drug_pack_qty,
       p.drug_min_unit,
       p.drug_pack_level,
       p.batch_no,
       p.parent_trace_code,
       p.total_qty,
       p.available_qty,
       p.current_offset
  from yb_trace_seed p
 where p.drug_dept_code = '{0}'
   and p.drug_code = '{1}'
   and p.seed_status in ('0', '1')
   and p.available_qty > 0
   and p.is_deleted = 'N'
   and p.is_valid = 'Y'
 order by create_time asc
 ";

                sql = string.Format(sql,
                    drugDeptCode,
                    drugCode
                    );

                this.ExecQuery(sql);

                var list = new List<YbTraceSeed>();
                YbTraceSeed info;
                while (this.Reader.Read())
                {
                    var i = 0;
                    info = new YbTraceSeed();

                    info.Id = Reader[i].ToString(); i++;
                    info.InboundOrderId = Reader[i].ToString(); i++;
                    info.InboundOrderNo = Reader[i].ToString(); i++;
                    info.DrugCode = Reader[i].ToString(); i++;
                    info.DrugName = Reader[i].ToString(); i++;
                    info.DrugDeptCode = Reader[i].ToString(); i++;
                    info.DrugDeptName = Reader[i].ToString(); i++;
                    info.DrugPackUnit = Reader[i].ToString(); i++;
                    info.DrugPackQty = Reader[i].ToString(); i++;
                    info.DrugMinUnit = Reader[i].ToString(); i++;
                    info.DrugPackLevel = Reader[i].ToString(); i++;
                    info.BatchNo = Reader[i].ToString(); i++;
                    info.ParentTraceCode = Reader[i].ToString(); i++;
                    info.TotalQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.AvailableQty = NConvert.ToDecimal(Reader[i]); i++;
                    info.CurrentOffset = NConvert.ToDecimal(Reader[i]); i++;
                    list.Add(info);
                }

                return list;

            }
            catch (Exception ex)
            {
                this.Err = "[YB_TRACE_SEED]查询数据异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

        }

        public Dictionary<string, List<string>> GetApplyNumberTraceMap(List<string> applyNumberList)
        {
            var applyTraceMap = new Dictionary<string, List<string>>();
            if (applyNumberList == null || applyNumberList.Count == 0)
            {
                return applyTraceMap;
            }

            try
            {
                var sqlBuilder = new StringBuilder();
                sqlBuilder.Append("select p.apply_number, p.pact_trac_codgs ");
                sqlBuilder.Append("from yb_trace_collect_main p ");
                sqlBuilder.Append("where p.collect_type='0' and p.is_deleted='N' and p.is_valid='Y' ");
                sqlBuilder.Append("and p.apply_number in (");

                for (int i = 0; i < applyNumberList.Count; i++)
                {
                    if (i > 0)
                    {
                        sqlBuilder.Append(",");
                    }

                    sqlBuilder.AppendFormat("'{0}'", applyNumberList[i].Replace("'", "''"));
                }

                sqlBuilder.Append(")");

                this.ExecQuery(sqlBuilder.ToString());

                while (this.Reader.Read())
                {
                    string applyNumber = this.Reader["apply_number"].ToString();
                    string rawCodes = this.Reader["pact_trac_codgs"].ToString();

                    List<string> traceCodes;
                    if (!applyTraceMap.TryGetValue(applyNumber, out traceCodes))
                    {
                        traceCodes = new List<string>();
                        applyTraceMap[applyNumber] = traceCodes;
                    }

                    if (string.IsNullOrEmpty(rawCodes))
                    {
                        continue;
                    }

                    string[] splitted = rawCodes.Split(
                        new[] { ';' },
                        StringSplitOptions.RemoveEmptyEntries);

                    foreach (string code in splitted)
                    {
                        string trimmed = code.Trim();
                        if (trimmed.Length == 0 || traceCodes.Contains(trimmed))
                        {
                            continue;
                        }

                        traceCodes.Add(trimmed);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Err = "[GetApplyNumberTraceMap]查询数据异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return applyTraceMap;
        }

        /// <summary>
        /// 根据申请单号列表获取申请明细信息
        /// </summary>
        /// <param name="applyNumberList">申请单号列表</param>
        /// <returns>申请明细列表</returns>
        public List<PhaComApplyout> GetApplyList(List<string> applyNumberList)
        {
            var applyList = new List<PhaComApplyout>();
            if (applyNumberList == null || applyNumberList.Count == 0)
            {
                return applyList;
            }

            try
            {
                var sqlBuilder = new StringBuilder();
                sqlBuilder.Append("select p.apply_number, p.mo_order, p.exec_sqn, p.recipe_no, ");
                sqlBuilder.Append("p.patient_id, p.sequence_no ");
                sqlBuilder.Append("from pha_com_applyout p ");
                sqlBuilder.Append("where p.apply_number in (");

                for (int i = 0; i < applyNumberList.Count; i++)
                {
                    if (i > 0)
                    {
                        sqlBuilder.Append(",");
                    }

                    sqlBuilder.AppendFormat("'{0}'", applyNumberList[i].Replace("'", "''"));
                }

                sqlBuilder.Append(")");

                this.ExecQuery(sqlBuilder.ToString());

                while (this.Reader.Read())
                {
                    var item = new PhaComApplyout();

                    // 申请单号
                    if (this.Reader["apply_number"] != DBNull.Value)
                    {

                        item.ApplyNumber = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader["apply_number"].ToString());
                    }

                    // 医嘱号
                    if (this.Reader["mo_order"] != DBNull.Value)
                    {
                        item.MoOrder = this.Reader["mo_order"].ToString();
                    }

                    // 执行序号
                    if (this.Reader["exec_sqn"] != DBNull.Value)
                    {
                        item.ExecSqn = this.Reader["exec_sqn"].ToString();
                    }

                    // 处方号
                    if (this.Reader["recipe_no"] != DBNull.Value)
                    {
                        item.RecipeNo = this.Reader["recipe_no"].ToString();
                    }

                    // 患者ID
                    if (this.Reader["patient_id"] != DBNull.Value)
                    {
                        item.PatientId = this.Reader["patient_id"].ToString();
                    }

                    // 序列号
                    if (this.Reader["sequence_no"] != DBNull.Value)
                    {
                        item.SequenceNo = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader["sequence_no"].ToString()); ;
                    }

                    applyList.Add(item);
                }
            }
            catch (Exception ex)
            {
                this.Err = "[GetApplyList]查询数据异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return applyList;
        }

        /// <summary>
        /// 新增采集主表信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool InsertYbTraceCollectMain(YbTraceCollectMain info)
        {
            try
            {
                #region insertSql

                var sql = @" insert into yb_trace_collect_main p
  (p.id,
   p.serial_no,
   p.card_no,
   p.patient_name,
   p.patient_no,
   p.drug_code,
   p.drug_name,
   p.drug_specs,
   p.drug_custom_code,
   p.drug_pact_unit,
   p.drug_pact_qty,
   p.drug_min_unit,
   p.pharmacy_code,
   p.pharmacy_name,
   p.dept_code,
   p.dept_name,
   p.apply_number,
   p.mo_order_no,
   p.exec_order_no,
   p.invoice_no,
   p.identifiy_code_list,
   p.identifiy_code,
   p.is_have_split,
   p.is_have_pact,
   p.drug_split_unit,
   p.pact_trac_codgs,
   p.pact_need_collect_qty,
   p.pact_actual_collect_qty,
   p.pact_un_collect_qty,
   p.pact_appeal_collect_qty,
   p.pact_collect_complete_rate,
   p.pact_collect_status,
   p.pact_collect_method,
   p.collect_start_time,
   p.collect_end_time,
   p.collect_duration_ms,
   p.split_trac_codgs,
   p.split_need_collect_qty,
   p.split_actual_collect_qty,
   p.split_un_collect_qty,
   p.split_appeal_collect_qty,
   p.split_collect_complete_rate,
   p.split_collect_status,
   p.split_collect_method,
   p.collect_ip,
   p.collect_type,
   p.collect_oper_code,
   p.collect_oper_name,
   p.business_scenario,
   p.source_system,
   p.business_type,
   p.created_code,
   p.created_name,
   p.hospital_code,
   p.hospital_name,
   p.ext_field1,
   p.ext_field2,
   p.ext_field3
)
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
   '{10}',
   '{11}',
   '{12}',
   '{13}',
   '{14}',
   '{15}',
   '{16}',
   '{17}',
   '{18}',
   '{19}',
   '{20}',
   '{21}',
   '{22}',
   '{23}',
   '{24}',
   '{25}',
   '{26}',
   '{27}',
   '{28}',
   '{29}',
   '{30}',
   '{31}',
   '{32}',
   to_date('{33}', 'YYYY-MM-DD hh24:mi:ss'),
   to_date('{34}', 'YYYY-MM-DD hh24:mi:ss'),
   '{35}',
   '{36}',
   '{37}',
   '{38}',
   '{39}',
   '{40}',
   '{41}',
   '{42}',
   '{43}',
   '{44}',
   '{45}',
   '{46}',
   '{47}',
   '{48}',
   '{49}',
   '{50}',
   '{51}',
   '{52}',
   '{53}',
   '{54}',
   '{55}',
   '{56}',
   '{57}'
)
  ";

                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.SerialNo,
                    info.CardNo,
                    info.PatientName,
                    info.PatientNo,
                    info.DrugCode,
                    info.DrugName,
                    info.DrugSpecs,
                    info.DrugCustomCode,
                    info.DrugPactUnit,
                    info.DrugPactQty,//10
                    info.DrugMinUnit,
                    info.PharmacyCode,
                    info.PharmacyName,
                    info.DeptCode,
                    info.DeptName,
                    info.ApplyNumber,
                    info.MoOrderNo,
                    info.ExecOrderNo,
                    info.InvoiceNo,
                    info.IdentifiyCodeList,//20
                    info.IdentifiyCode,
                    info.IsHaveSplit,
                    info.IsHavePact,
                    info.DrugSplitUnit,
                    info.PactTracCodgs,
                    info.PactNeedCollectQty,
                    info.PactActualCollectQty,
                    info.PactUnCollectQty,
                    info.PactAppealCollectQty,
                    info.PactCollectCompleteRate,//30
                    info.PactCollectStatus,
                    info.PactCollectMethod,
                    info.CollectStartTime,
                    info.CollectEndTime,
                    info.CollectDurationMs,
                    info.SplitTracCodgs,
                    info.SplitNeedCollectQty,
                    info.SplitActualCollectQty,
                    info.SplitUnCollectQty,
                    info.SplitAppealCollectQty,//40
                    info.SplitCollectCompleteRate,
                    info.SplitCollectStatus,
                    info.SplitCollectMethod,
                    info.CollectIp,
                    info.CollectType,
                    info.CollectOperCode,
                    info.CollectOperName,
                    info.BusinessScenario,
                    info.SourceSystem,
                    info.BusinessType,//50
                    info.CreatedCode,
                    info.CreatedName,
                    info.HospitalCode,
                    info.HospitalName,
                    info.ExtField1,
                    info.ExtField2,
                    info.ExtField3
                   );

                if (this.ExecNoQuery(sql) < 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Err = "插入采集主表异常:" + ex.Message;
                return false;
            }
        }

        public bool InsertYbTraceCollectDetail(YbTraceCollectDetail info)
        {
            try
            {

                #region InsertSql

                var sql = @"  insert into yb_trace_collect_detail p
  (p.id,
   p.main_id,
   p.apply_number,
   p.trace_code,
   p.trace_code_type,
   p.trace_code_source,
   p.trace_code_format,
   p.collect_sequence,
   p.collect_timestamp,
   p.drug_code,
   p.drug_name,
   p.created_code,
   p.created_name)
values
  ('{0}',
   '{1}',
   '{2}',
   '{3}',
   '{4}',
   '{5}',
   '{6}',
   '{7}',
   to_date('{8}', 'YYYY-MM-DD hh24:mi:ss'),
   '{9}',
   '{10}',
   '{11}',
   '{12}')
 ";

                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.MainId,
                    info.ApplyNumber,
                    info.TraceCode,
                    info.TraceCodeType,
                    info.TraceCodeSource,
                    info.TraceCodeFormat,
                    info.CollectSequence,
                    info.CollectTimestamp,
                    info.DrugCode,
                    info.DrugName,
                    info.CreatedCode,
                    info.CreatedName
                    );

                if (this.ExecNoQuery(sql) < 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Err = "插入采集明细表异常:" + ex.Message;
                return false;
            }

        }

        public bool InsertYbTraceInboundOrder(YbTraceInboundOrder info)
        {
            try
            {
                #region sql

                var sql = @" insert into yb_trace_inbound_order p
(
p.id,
p.drug_dept_code,
p.drug_dept_name,
p.inbound_no,
p.supplier_id,
p.supplier_code,
p.supplier_name,
p.drug_code,
p.drug_name,
p.drug_specs,
p.drug_custom_code,
p.drug_pact_unit,
p.drug_pact_qty,
p.drug_min_unit,
p.bch_no,
p.manu_lotnum,
p.manu_date,
p.expy_end,
p.original_trace_code,
p.original_qty,
p.split_qty,
p.status,
p.source_type,
p.inbound_client_ip,
p.inbound_oper_code,
p.inbound_oper_name,
p.inbound_oper_time,
p.created_code,
p.created_name,
p.create_time
)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
to_date('{16}', 'YYYY-MM-DD hh24:mi:ss'),
to_date('{17}', 'YYYY-MM-DD hh24:mi:ss'),
'{18}',
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
to_date('{26}', 'YYYY-MM-DD hh24:mi:ss'),
'{27}',
'{28}',
to_date('{29}', 'YYYY-MM-DD hh24:mi:ss')
) ";

                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.DrugDeptCode,
                    info.DrugDeptName,
                    info.InboundNo,
                    info.SupplierId,
                    info.SupplierCode,
                    info.SupplierName,
                    info.DrugCode,
                    info.DrugName,
                    info.DrugSpecs,
                    info.DrugCustomCode, //10
                    info.DrugPactUnit,
                    info.DrugPactQty,
                    info.DrugMinUnit,
                    info.BchNo,
                    info.ManuLotnum,
                    info.ManuDate,
                    info.ExpyEnd,
                    info.OriginalTraceCode,
                    info.OriginalQty,
                    info.SplitQty, //20
                    info.Status,
                    info.SourceType,
                    info.InboundClientIp,
                    info.InboundOperCode,
                    info.InboundOperName,
                    info.InboundOperTime,
                    info.CreatedCode,
                    info.CreatedName,
                    info.CreateTime
                    );

                return this.ExecNoQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                this.Err = "插入[yb_trace_inbound_order]出现异常:" + ex.Message;
                return false;
            }
        }

        public bool InsertYbTraceInboundDetail(YbTraceInboundDetail info)
        {
            try
            {

                #region sql

                var sql = @" insert into yb_trace_inbound_detail p
(
p.id,
p.inbound_id,
p.drug_dept_code,
p.drug_dept_name,
p.inbound_no,
p.supplier_id,
p.supplier_code,
p.supplier_name,
p.drug_code,
p.drug_name,
p.drug_specs,
p.drug_custom_code,
p.drug_pact_unit,
p.drug_pact_qty,
p.drug_min_unit,
p.bch_no,
p.manu_lotnum,
p.manu_date,
p.expy_end,
p.original_trace_code,
p.original_qty,
p.split_qty,
p.parent_trace_code,
p.child_trace_code,
p.child_qty,
p.child_sequence_no,
p.status,
p.source_type,
p.inbound_client_ip,
p.inbound_oper_code,
p.inbound_oper_name,
p.inbound_oper_time,
p.created_code,
p.created_name
)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
to_date('{17}', 'YYYY-MM-DD hh24:mi:ss'),
to_date('{18}', 'YYYY-MM-DD hh24:mi:ss'),
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
'{26}',
'{27}',
'{28}',
'{29}',
'{30}',
to_date('{31}', 'YYYY-MM-DD hh24:mi:ss'),
'{32}',
'{33}'
) ";

                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.InboundId,
                    info.DrugDeptCode,
                    info.DrugDeptName,
                    info.InboundNo,
                    info.SupplierId,
                    info.SupplierCode,
                    info.SupplierName,
                    info.DrugCode,
                    info.DrugName,
                    info.DrugSpecs,//10
                    info.DrugCustomCode,
                    info.DrugPactUnit,
                    info.DrugPactQty,
                    info.DrugMinUnit,
                    info.BchNo,
                    info.ManuLotnum,
                    info.ManuDate,
                    info.ExpyEnd,
                    info.OriginalTraceCode,
                    info.OriginalQty,//20
                    info.SplitQty,
                    info.ParentTraceCode,
                    info.ChildTraceCode,
                    info.ChildQty,
                    info.ChildSequenceNo,
                    info.Status,
                    info.SourceType,
                    info.InboundClientIp,
                    info.InboundOperCode,
                    info.InboundOperName,//30
                    info.InboundOperTime,
                    info.CreatedCode,
                    info.CreatedName
                    );

                return this.ExecNoQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_inbound_detail]新增出现异常:" + ex.Message;
                return false;
            }
        }

        public bool InsertYbTraceStateRecord(YbTraceStateRecord info)
        {
            try
            {

                #region sql

                var sql = @" insert into yb_trace_state_record p
(
p.id,
p.drug_code,
p.drug_name,
p.drug_specs,
p.drug_custom_code,
p.drug_pact_unit,
p.drug_pact_qty,
p.drug_min_unit,
p.drug_dept_code,
p.drug_dept_name,
p.bch_no,
p.manu_lotnum,
p.manu_date,
p.expy_end,
p.parent_trace_code,
p.child_trace_code,
p.sequence_no,
p.trace_status,
p.inbound_time,
p.apply_number,
p.serial_no,
p.data_type,
p.patient_name,
p.card_no,
p.patient_no,
p.recipe_no,
p.created_code,
p.created_name
)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
to_date('{12}', 'YYYY-MM-DD hh24:mi:ss'),
to_date('{13}', 'YYYY-MM-DD hh24:mi:ss'),
'{14}',
'{15}',
'{16}',
'{17}',
to_date('{18}', 'YYYY-MM-DD hh24:mi:ss'),
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
'{26}',
'{27}'
) ";

                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.DrugCode,
                    info.DrugName,
                    info.DrugSpecs,
                    info.DrugCustomCode,
                    info.DrugPactUnit,
                    info.DrugPactQty,
                    info.DrugMinUnit,
                    info.DrugDeptCode,
                    info.DrugDeptName,
                    info.BchNo,//10
                    info.ManuLotnum,
                    info.ManuDate,
                    info.ExpyEnd,
                    info.ParentTraceCode,
                    info.ChildTraceCode,
                    info.SequenceNo,
                    info.TraceStatus,
                    info.InboundTime,
                    info.ApplyNumber,
                    info.SerialNo,//20
                    info.DataType,
                    info.PatientName,
                    info.CardNo,
                    info.PatientNo,
                    info.RecipeNo,
                    info.CreatedCode,
                    info.CreatedName //27
                    );

                return this.ExecNoQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_state_record]新增数据出现异常:" + ex.Message;
                return false;
            }
        }

        public bool InsertYbTraceCodeRecord(YbTraceCodeRecord info)
        {
            try
            {

                #region sql

                var sql = @" insert into yb_trace_code_record p
(
p.id,
p.drug_code,
p.drug_name,
p.parent_trace_code,
p.child_trace_code,
p.sequence_no,
p.related_order_no,
p.related_id,
p.related_table_name,
p.operation_type,
p.operation_time,
p.operation_description,
p.operation_json,
p.created_code,
p.created_name
)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
to_date('{10}', 'YYYY-MM-DD hh24:mi:ss'),
'{11}',
'{12}',
'{13}',
'{14}'
) ";

                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.DrugCode,
                    info.DrugName,
                    info.ParentTraceCode,
                    info.ChildTraceCode,
                    info.SequenceNo,
                    info.RelatedOrderNo,
                    info.RelatedId,
                    info.RelatedTableName.ToLower(),
                    info.OperationType,
                    info.OperationTime,//10
                    info.OperationDescription,
                    info.OperationJson,
                    info.CreatedCode,
                    info.CreatedName
                    );

                return this.ExecNoQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_code_record]新增数据异常:" + ex.Message;
                return false;
            }
        }

        public bool InsertYbTraceStockRecord(YbTraceStockRecord info)
        {
            try
            {

                #region sql

                var sql = @" insert into yb_trace_stock_record p
(
p.id,
p.drug_code,
p.drug_name,
p.drug_dept_code,
p.drug_dept_name,
p.change_type,
p.before_total_qty,
p.before_available_qty,
p.before_prededucted_qty,
p.before_expired_qty,
p.before_damaged_qty,
p.after_total_qty,
p.after_available_qty,
p.after_prededucted_qty,
p.after_expired_qty,
p.after_damaged_qty,
p.related_table,
p.related_id,
p.related_no,
p.created_code,
p.created_name
)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}'
) ";

                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.DrugCode,
                    info.DrugName,
                    info.DrugDeptCode,
                    info.DrugDeptName,
                    info.ChangeType,
                    info.BeforeTotalQty,
                    info.BeforeAvailableQty,
                    info.BeforePredeductedQty,
                    info.BeforeExpiredQty,
                    info.BeforeDamagedQty,
                    info.AfterTotalQty,
                    info.AfterAvailableQty,
                    info.AfterPredeductedQty,
                    info.AfterExpiredQty,
                    info.AfterDamagedQty,
                    info.RelatedTable.ToLower(),
                    info.RelatedId,
                    info.RelatedNo,
                    info.CreatedCode,
                    info.CreatedName
                    );

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_stock_record]新增数据异常:" + ex.Message;
                return false;
            }
        }

        public bool InsertYbTraceStock(YbTraceStock info)
        {
            try
            {
                #region sql

                var sql = @" insert into yb_trace_stock p
(
p.id,
p.drug_code,
p.drug_name,
p.drug_specs,
p.drug_custom_code,
p.drug_pact_unit,
p.drug_pact_qty,
p.drug_min_unit,
p.drug_dept_code,
p.drug_dept_name,
p.total_qty,
p.available_qty,
p.prededucted_qty,
p.expired_qty,
p.damaged_qty,
p.first_inbound_time,
p.created_code,
p.created_name,
p.create_time,
p.is_deleted,
p.is_valid,
p.memo,
p.backup_1,
p.backup_2,
p.backup_3
)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
to_date('{15}', 'YYYY-MM-DD hh24:mi:ss'),
'{16}',
'{17}',
to_date('{18}', 'YYYY-MM-DD hh24:mi:ss'),
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}'
) ";

                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.DrugCode,
                    info.DrugName,
                    info.DrugSpecs,
                    info.DrugCustomCode,
                    info.DrugPactUnit,
                    info.DrugPactQty,
                    info.DrugMinUnit,
                    info.DrugDeptCode,
                    info.DrugDeptName,
                    info.TotalQty,
                    info.AvailableQty,
                    info.PreDeductedQty,
                    info.ExpiredQty,
                    info.DamagedQty,
                    info.FirstInboundTime,
                    info.CreatedCode,
                    info.CreatedName,
                    info.CreateTime,
                    info.IsDeleted,
                    info.IsValid,
                    info.Memo,
                    info.Backup1,
                    info.Backup2,
                    info.Backup3
                    );

                return this.ExecNoQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_stock]新增库存品种出现异常:" + ex.Message;
                return false;
            }
        }

        public bool InsertYbTraceSeed(YbTraceSeed info)
        {
            try
            {

                #region sql

                var sql = @" insert into yb_trace_seed p
(
p.id,
p.inbound_order_id,
p.inbound_order_no,
p.drug_code,
p.drug_name,
p.drug_dept_code,
p.drug_dept_name,
p.drug_pack_unit,
p.drug_pack_qty,
p.drug_min_unit,
p.drug_pack_level,
p.batch_no,
p.parent_trace_code,
p.total_qty,
p.available_qty,
p.current_offset,
p.supplier_code,
p.supplier_name,
p.seed_status,
p.created_code,
p.created_name,
p.is_deleted,
p.is_valid,
p.memo,
p.backup_1,
p.backup_2,
p.backup_3
)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
'{26}'
) ";
                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.InboundOrderId,
                    info.InboundOrderNo,
                    info.DrugCode,
                    info.DrugName,
                    info.DrugDeptCode,
                    info.DrugDeptName,
                    info.DrugPackUnit,
                    info.DrugPackQty,
                    info.DrugMinUnit,
                    info.DrugPackLevel,
                    info.BatchNo,
                    info.ParentTraceCode,
                    info.TotalQty,
                    info.AvailableQty,
                    info.CurrentOffset,
                    info.SupplierCode,
                    info.SupplierName,
                    info.SeedStatus,
                    info.CreatedCode,
                    info.CreatedName,
                    info.IsDeleted,
                    info.IsValid,
                    info.Memo,
                    info.Backup1,
                    info.Backup2,
                    info.Backup3
                    );

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_seed]新增种子数据出现异常:" + ex.Message;
                return false;
            }
        }

        public bool InsertYbTraceAllocationRange(YbTraceAllocationRange info)
        {

            try
            {

                #region sql

                var sql = @" insert into yb_trace_allocation_range p
  (p.id,
   p.seed_id,
   p.trace_code,
   p.drug_code,
   p.drug_name,
   p.apply_number,
   p.serial_no,
   p.card_no,
   p.patient_name,
   p.patient_no,
   p.mo_order_no,
   p.exec_order_no,
   p.invoice_no,
   p.recipe_no,
   p.recipe_sequence_no,
   p.start_offset,
   p.end_offset,
   p.allocated_qty,
   p.range_status,
   p.created_code,
   p.created_name,
   p.is_deleted,
   p.is_valid,
   p.trans_type)
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
   '{10}',
   '{11}',
   '{12}',
   '{13}',
   '{14}',
   '{15}',
   '{16}',
   '{17}',
   '{18}',
   '{19}',
   '{20}',
   '{21}',
   '{22}',
   '{23}') ";

                #endregion

                sql = string.Format(sql,
                    info.Id,
                    info.SeedId,
                    info.TraceCode,
                    info.DrugCode,
                    info.DrugName,
                    info.ApplyNumber,
                    info.SerialNo,
                    info.CardNo,
                    info.PatientName,
                    info.PatientNo,
                    info.MoOrderNo,
                    info.ExecOrderNo,
                    info.InvoiceNo,
                    info.RecipeNo,
                    info.RecipeSequenceNo,
                    info.StartOffset,
                    info.EndOffset,
                    info.AllocatedQty,
                    info.RangeStatus,
                    info.CreatedCode,
                    info.CreatedName,
                    info.IsDeleted,
                    info.IsValid,
                    info.TransType
                    );

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_allocation_range]新增拆零分配数据出现异常:" + ex.Message;
                return false;
            }

        }

        public bool UpdateYbTraceStockWhenInboundSucess(
            string drugDeptCode,
            string drugCode,
            decimal inBoundQty,
            string modifiedCode,
            string modifiedName
            )
        {
            try
            {
                var sql = @" update yb_trace_stock p
   set p.total_qty         = p.total_qty + '{0}',
       p.available_qty     = p.available_qty + '{0}',
       p.last_inbound_time = sysdate,
       p.modified_code     = '{3}',
       p.modified_name     = '{4}',
       p.modified_time     = sysdate
 where p.drug_dept_code = '{1}'
   and p.drug_code = '{2}' ";

                sql = string.Format(sql,
                    inBoundQty,
                    drugDeptCode,
                    drugCode,
                    modifiedCode,
                    modifiedName
                    );

                return this.ExecNoQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_stock]更新数据出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateYbTraceStockWhenPredeductedSucess(
           string drugDeptCode,
           string drugCode,
           decimal predeductedQty,
           string modifiedCode,
           string modifiedName
           )
        {
            try
            {
                var sql = @" update yb_trace_stock p
   set p.available_qty         = p.available_qty - '{0}',
       p.prededucted_qty     = p.prededucted_qty + '{0}',
       p.last_inbound_time = sysdate,
       p.modified_code     = '{3}',
       p.modified_name     = '{4}',
       p.modified_time     = sysdate
 where p.drug_dept_code = '{1}'
   and p.drug_code = '{2}' ";

                sql = string.Format(sql,
                    predeductedQty,
                    drugDeptCode,
                    drugCode,
                    modifiedCode,
                    modifiedName
                    );

                return this.ExecNoQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_stock]更新数据出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateYbTraceStockWhenUseSuccess(
           string drugDeptCode,
           string drugCode,
           decimal useQty)
        {
            try
            {
                var sql = @" update yb_trace_stock p
    set p.available_qty      = p.available_qty - '{0}',
        p.total_qty          = p.total_qty - '{0}',
        p.last_outbound_time = sysdate
  where p.drug_dept_code = '{1}'
    and p.drug_code = '{2}'
    and p.available_qty > '{0}' ";

                sql = string.Format(sql,
                    useQty,
                    drugDeptCode,
                    drugCode
                    );

                return this.ExecNoQuery(sql) == 1;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_stock]更新数据出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateYbTraceStockWhenOutBoundSucess(
            string drugDeptCode,
            string drugCode,
            decimal inBoundQty,
            string modifiedCode,
            string modifiedName
            )
        {
            try
            {
                var sql = @" update yb_trace_stock p
   set p.total_qty         = p.total_qty - '{0}',
       p.available_qty     = p.available_qty - '{0}',
       p.last_outbound_time = sysdate,
       p.modified_code     = '{3}',
       p.modified_name     = '{4}',
       p.modified_time     = sysdate
 where p.drug_dept_code = '{1}'
   and p.drug_code = '{2}' ";

                sql = string.Format(sql,
                    inBoundQty,
                    drugDeptCode,
                    drugCode,
                    modifiedCode,
                    modifiedName
                    );

                return this.ExecNoQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_stock]更新数据出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateYbTraceInboundOrderState(
            string inboundNo,
            string modifiedCode,
            string modifiedName
            )
        {
            try
            {
                string sql = @" update yb_trace_inbound_order p
   set p.status        = '2',
       p.modified_code = '{1}',
       p.modified_name = '{2}',
       p.modified_time = sysdate
 where p.inbound_no = '{0}'
   and p.status = '0'
   and p.is_deleted = 'N'
   and p.is_valid = 'Y' ";

                sql = string.Format(sql,
                    inboundNo,
                    modifiedCode,
                    modifiedName
                    );

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_inbound_order]更新状态出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateYbTraceInboundDetailState(
            string inboundNo,
            string modifiedCode,
            string modifiedName
            )
        {
            try
            {
                string sql = @" update yb_trace_inbound_detail p
   set p.status        = '2',
       p.modified_code = '{1}',
       p.modified_name = '{2}',
       p.modified_time = sysdate
 where p.inbound_no = '{0}'
   and p.status = '0'
   and p.is_deleted = 'N'
   and p.is_valid = 'Y' ";

                sql = string.Format(sql,
                    inboundNo,
                    modifiedCode,
                    modifiedName
                    );

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_inbound_order]更新状态出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateYbTraceStateRecordTraceState(
            string parentTraceCode,
            string traceState,
            string modifiedCode,
            string modifiedName)
        {
            try
            {
                string sql = @" update yb_trace_state_record p
   set p.trace_status  = '{1}',
       p.modified_code = '{2}',
       p.modified_name = '{3}',
       p.modified_time = sysdate
 where p.parent_trace_code = '{0}'
   and p.is_deleted = 'N'
   and p.is_valid = 'Y' ";

                sql = string.Format(sql,
                    parentTraceCode,
                    traceState,
                    modifiedCode,
                    modifiedName
                    );

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_state_record]更新状态出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateYbTraceStateRecordTraceState(
            List<string> idList,
            string traceState,
            string modifiedCode,
            string modifiedName)
        {
            try
            {
                if (!idList.Any())
                {
                    return false;
                }

                var inValues = string.Join("','", idList.ToArray());

                string sql = @" update yb_trace_state_record p
   set p.trace_status  = '{1}',
       p.modified_code = '{2}',
       p.modified_name = '{3}',
       p.modified_time = sysdate
 where p.id in ('{0}')
   and p.is_deleted = 'N'
   and p.is_valid = 'Y' ";

                sql = string.Format(sql,
                    inValues,
                    traceState,
                    modifiedCode,
                    modifiedName
                    );

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_state_record]更新状态出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateApplyOutTheTraceCodeInfo(
            string applyNumber,
            decimal needCollectQty,
            decimal needCollectSpiltQty,
            string needCollectTraceCodeFlag,
            string notCollectTraceCodeReason,
            string traceCodeCollectionStatus)
        {
            try
            {

                var sql = @" update pha_com_applyout p set 
p.NeedCollectQty='{1}',
p.needCollectSpiltQty='{2}',
p.needCollectTraceCodeFlag='{3}',
p.notCollectTraceCodeReason='{4}',
p.TraceCodeCollectionStatus='{5}'
where p.apply_number='{0}' ";

                sql = string.Format(sql,
                    applyNumber,
                    needCollectQty,
                    needCollectSpiltQty,
                    needCollectTraceCodeFlag,
                    notCollectTraceCodeReason,
                    traceCodeCollectionStatus
                    );

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "[UpdateApplyOutTheTraceCodeInfo]更新数据出现异常:" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 更新入库主表状态
        /// 触发器触发新增入库明细数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateYbTraceInboundOrderState(string id)
        {
            try
            {
                var sql = @" update yb_trace_inbound_order p
   set p.status = '1'
 where p.id = '{0}'
   and p.status = '0' ";

                sql = string.Format(sql, id);

                return this.ExecNoQuery(sql) == 1;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_inbound_order]表状态更新出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateApplyOutInfo(ApplyOut info)
        {

            try
            {
                var sql = @" update pha_com_applyout p
   set p.tracecodecollectionstatus = '{1}',
       p.needcollectqty            = '{2}',
       p.alreadycollectqty         = '{3}',
       p.appealcollectqty          = '{4}'
 where p.apply_number = '{0}'
  ";

                sql = string.Format(sql,
                    info.ID,
                    info.TraceCodeCollectionStatus,
                    info.NeedCollectQty,
                    info.AlreadyCollectQty,
                    info.AppealCollectQty
                    );

                if (this.ExecNoQuery(sql) < 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Err = "更新发药申请表采集信息异常:" + ex.Message;
                return false;
            }

        }

        public bool UpdateTraceSeedWhenUseSucess(
            string id,
            decimal newAvailableQty,
            decimal newCurrentOffset,
            string newSeedStatus,
            decimal oldCurrentOffset
            )
        {
            try
            {

                var sql = @" update yb_trace_seed p
   set p.available_qty  = '{0}',
       p.current_offset = '{1}',
       p.seed_status    = '{2}'
 where p.id = '{3}'
   and p.current_offset = '{4}' ";

                sql = string.Format(sql,
                    newAvailableQty,
                    newCurrentOffset,
                    newSeedStatus,
                    id,
                    oldCurrentOffset
                    );

                return this.ExecNoQuery(sql) == 1;

            }
            catch (Exception ex)
            {
                this.Err = "[yb_trace_inbound_order]表状态更新出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateApplyOutWhenCollectSuccess(
            YbTraceCollectMain info, string packConvertToSplitFlag)
        {
            try
            {
                var sql = @" update pha_com_applyout p
   set p.alreadycollectqty         = '{0}',
       p.appealcollectqty          = '{1}',
       p.alreadycollectspiltqty    = '{2}',
       p.appealcollectspiltqty     = '{3}',
       p.tracecodecollectionstatus = '{4}',
       p.packConvertToSplitFlag='{6}'
 where p.apply_number = '{5}' ";

                var tracecodecollectionstatus = TraceCodeCollectionStatusEnum.Sucess;
                if (info.PactAppealCollectQty > 0 || info.SplitAppealCollectQty > 0)
                {
                    tracecodecollectionstatus = TraceCodeCollectionStatusEnum.Completed;
                }
                sql = string.Format(sql,
                    info.PactActualCollectQty,
                    info.PactAppealCollectQty,
                    info.SplitActualCollectQty,
                    info.SplitAppealCollectQty,
                    tracecodecollectionstatus,
                    info.ApplyNumber,
                    packConvertToSplitFlag
                    );

                return this.ExecNoQuery(sql) == 1;
            }
            catch (Exception ex)
            {
                this.Err = "[pha_com_applyout]表更新出现异常:" + ex.Message;
                return false;
            }
        }

        public bool IsExistStock(string drugDeptCode, string drugCode)
        {
            var sql = @" select count(1) from yb_trace_stock p where p.is_deleted='N' and p.is_valid='Y' and p.drug_dept_code='{0}' and p.drug_code='{1}' ";
            sql = string.Format(sql, drugDeptCode, drugCode);

            var res = this.ExecSqlReturnOne(sql, "0");

            return NConvert.ToInt32(res) > 0;
        }

        public bool IsExistInboundOrder(string traceCode)
        {
            var sql = string.Format(@" select count(1) from yb_trace_inbound_order p where p.is_deleted='N' and p.is_valid='Y' and p.status='0' and p.original_trace_code='{0}' ", traceCode);

            var result = this.ExecSqlReturnOne(sql, "0");

            return NConvert.ToInt32(result) > 0;
        }

        public bool IsCYDYApplyOut(List<string> applyNumbers)
        {
            try
            {
                if (!applyNumbers.Any())
                {
                    return false;
                }

                var inValues = string.Join("','", applyNumbers.ToArray());
                var sql = @" select count(1) from pha_com_applyout p where  p.apply_number in ('{0}') and p.billclass_code='R' and exists (select 1 from pha_com_applyout a where a.recipe_no=p.recipe_no and a.sequence_no=p.sequence_no and a.billclass_code='O' and a.class3_meaning_code='Z1') ";

                sql = string.Format(sql, inValues);
                var result = this.ExecSqlReturnOne(sql, "0");
                return Neusoft.FrameWork.Function.NConvert.ToInt32(result) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "[IsCYDYApplyOut]查询异常:" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取拆零入库单号
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public string GetInboundNo(string type)
        {
            try
            {

                var sql = @"SELECT TO_CHAR(SYSDATE, 'YYYYMMDD') || LPAD(seq_yb_trace_split_inbound.NEXTVAL, 7, '0') AS inbound_no FROM dual ";

                var seqNo = this.ExecSqlReturnOne(sql, "");
                return "IN-" + type + seqNo;
            }
            catch (Exception ex)
            {
                this.Err = "获取拆零入库单号异常:" + ex.Message;
                return "";
            }
        }

        /// <summary>
        /// 获取拆零出库单号
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetOutboundNo(string type)
        {
            try
            {

                var sql = @"SELECT TO_CHAR(SYSDATE, 'YYYYMMDD') || LPAD(seq_yb_trace_split_outbound.NEXTVAL, 7, '0') AS inbound_no FROM dual ";

                var seqNo = this.ExecSqlReturnOne(sql, "");
                return "OUT-" + type + seqNo;
            }
            catch (Exception ex)
            {
                this.Err = "获取拆零出库单号异常:" + ex.Message;
                return "";
            }
        }

        public FinOpbFeedetail GetFeeDetailInfo(string clinicCode, string recipeNo, string sequenceNo, string moOrder)
        {
            try
            {
                var sql = @" select p.clinic_code,p.recipe_no,p.sequence_no,p.old_mo_order,p.mo_order from fin_opb_feedetail p where p.clinic_code='{0}' and p.recipe_no='{1}' and p.sequence_no='{2}' and p.mo_order='{3}' and p.trans_type='1' and rownum=1 ";

                sql = string.Format(sql, clinicCode, recipeNo, sequenceNo, moOrder);

                this.ExecQuery(sql);

                var info = new FinOpbFeedetail();
                while (this.Reader.Read())
                {
                    var i = 0;
                    info.ClinicCode = Reader[i].ToString(); i++;
                    info.RecipeNo = Reader[i].ToString(); i++;
                    info.SequenceNo = NConvert.ToDecimal(Reader[i].ToString()); i++;
                    info.OldMoOrder = Reader[i].ToString(); i++;
                    info.MoOrder = Reader[i].ToString(); i++;
                    break;
                }
                return info;

            }
            catch (Exception ex)
            {
                this.Err = "[GetFeeDetailInfo]执行异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
        }

        /// <summary>
        /// 根据原医嘱信息查询原发药申请流水号
        /// </summary>
        public string GetOriginalApplyNumber(string patientId, string oldMoOrder, string recipeNo, string sequenceNo)
        {
            string applyNumber = "";

            try
            {
                var sqlBuilder = new StringBuilder();
                sqlBuilder.Append("select p.apply_number ");
                sqlBuilder.Append("from pha_com_applyout p ");
                sqlBuilder.AppendFormat("where p.patient_id='{0}' ", patientId.Replace("'", "''"));
                sqlBuilder.AppendFormat("and p.mo_order='{0}' ", oldMoOrder.Replace("'", "''"));
                sqlBuilder.AppendFormat("and p.recipe_no='{0}' ", recipeNo.Replace("'", "''"));
                sqlBuilder.AppendFormat("and p.sequence_no='{0}'", sequenceNo.Replace("'", "''"));
                this.ExecQuery(sqlBuilder.ToString());
                if (this.Reader.Read())
                {
                    if (this.Reader["apply_number"] != DBNull.Value)
                    {
                        applyNumber = this.Reader["apply_number"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Err = "[GetOriginalApplyNumber]查询数据异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return applyNumber;
        }

        /// <summary>
        /// 根据原发药申请流水号查询发药和退费采集的追溯码信息
        /// </summary>
        /// <param name="applyNumber">原发药申请流水号</param>
        /// <returns>Key: collect_type (0销售 1退货), Value: 采集信息</returns>
        public Dictionary<string, YbTraceCollectMain> GetTraceCollectInfoByApplyNumber(string applyNumber)
        {
            var result = new Dictionary<string, YbTraceCollectMain>();

            try
            {
                var sqlBuilder = new StringBuilder();
                sqlBuilder.Append("select p.id, p.collect_type, p.pact_trac_codgs, p.pact_actual_collect_qty, ");
                sqlBuilder.Append("p.split_trac_codgs, p.split_actual_collect_qty ");
                sqlBuilder.Append("from yb_trace_collect_main p ");
                sqlBuilder.AppendFormat("where p.apply_number='{0}' ", applyNumber.Replace("'", "''"));
                sqlBuilder.Append("and p.is_deleted='N' and p.is_valid='Y'");

                this.ExecQuery(sqlBuilder.ToString());

                while (this.Reader.Read())
                {
                    var info = new YbTraceCollectMain();
                    info.Id = this.Reader["id"].ToString();
                    info.CollectType = this.Reader["collect_type"].ToString();
                    info.PactTracCodgs = this.Reader["pact_trac_codgs"] != DBNull.Value
                        ? this.Reader["pact_trac_codgs"].ToString() : "";
                    info.SplitTracCodgs = this.Reader["split_trac_codgs"] != DBNull.Value
                        ? this.Reader["split_trac_codgs"].ToString() : "";

                    if (this.Reader["pact_actual_collect_qty"] != DBNull.Value)
                    {
                        info.PactActualCollectQty = Convert.ToDecimal(this.Reader["pact_actual_collect_qty"]);
                    }
                    if (this.Reader["split_actual_collect_qty"] != DBNull.Value)
                    {
                        info.SplitActualCollectQty = Convert.ToDecimal(this.Reader["split_actual_collect_qty"]);
                    }

                    // 解析包装追溯码列表
                    info.PactTracCodgsList = new List<string>();
                    if (!string.IsNullOrEmpty(info.PactTracCodgs))
                    {
                        info.PactTracCodgsList.AddRange(
                            info.PactTracCodgs.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(c => c.Trim())
                                .Where(c => c.Length > 0));
                    }

                    // 解析拆零追溯码列表
                    info.SplitTracCodgsList = new List<string>();
                    if (!string.IsNullOrEmpty(info.SplitTracCodgs))
                    {
                        info.SplitTracCodgsList.AddRange(
                            info.SplitTracCodgs.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(c => c.Trim())
                                .Where(c => c.Length > 0));
                    }

                    // 按 collect_type 存储
                    result[info.CollectType] = info;
                }
            }
            catch (Exception ex)
            {
                this.Err = "[GetTraceCollectInfoByApplyNumber]查询数据异常:" + ex.Message;
                return null;
            }
            finally
            {
                this.Reader.Close();
            }

            return result;
        }

        public bool GetOtherInfo(YbTraceCollectMain mainInfo)
        {
            try
            {
                var sql = @" select 
       p.invoice_no
  from fin_opb_feedetail p
  left join fin_com_pactunitinfo pa
    on pa.pact_code = p.pact_code
   and pa.flag = '0'
 where p.clinic_code = '{0}'
   and p.mo_order = '{1}'
   and p.trans_type = '1'
   and rownum = 1 ";
                sql = string.Format(sql, mainInfo.SerialNo, mainInfo.MoOrderNo);

                this.ExecQuery(sql);
                while (this.Reader.Read())
                {
                    mainInfo.InvoiceNo = Reader[0].ToString();
                    break;
                }

                mainInfo.ExtField1 = "0";
                mainInfo.ExtField2 = "01";

                sql = string.Format(@" select p.pact_name from fin_ipr_siinmaininfo_gd p where p.inpatient_no='{0}' and p.invoice_no='{1}' and p.type_code='1' and rownum=1 ", mainInfo.SerialNo, mainInfo.InvoiceNo);

                var pactName = this.ExecSqlReturnOne(sql, "");
                if (!string.IsNullOrEmpty(pactName)) 
                {
                    mainInfo.ExtField2 = "02";
                    if (pactName.Contains("工伤")) 
                    {
                        mainInfo.ExtField1 = "1";
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Err = "[GetOtherInfo]执行异常:" + ex.Message;
                return false;
            }
            finally
            {
                this.Reader.Close();
            }
        }

    }
}
