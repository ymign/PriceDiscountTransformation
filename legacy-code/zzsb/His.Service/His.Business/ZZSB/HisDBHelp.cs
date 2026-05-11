using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.HISFC.Models.Base;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Registration;

namespace His.Business.ZZSB
{
    public class HisDBHelp : Neusoft.FrameWork.Management.Database
    {
        public ArrayList QueryUnpaidFeeDetail(string cliincCode)
        {
            #region sql
            string sql = string.Format(@"  select 
              m.RECIPE_NO, --  处方号
              SEQUENCE_NO, --  处方内项目流水号
              m.TRANS_TYPE, --  交易类型,1正交易，2反交易
              m.CLINIC_CODE, --  门诊号
              m.CARD_NO, --  病历卡号
              m.REG_DATE, --  挂号日期
              REG_DPCD, --  挂号科室
              m.DOCT_CODE, --  开方医师
              DOCT_DEPT, --  开方医师所在科室
              ITEM_CODE, --  项目代码
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
fun_get_dept_name(m.REG_DPCD) deptname,
fun_get_employee_name(m.doct_code) doctname,
 (select  name from com_dictionary y  where y.type='DeptExecAddress' and code=m.EXEC_DPCD and rownum=1 )as  address,
RECIPE_FLAG
  from fin_opb_feedetail m
  inner join fin_opr_register reg on reg.clinic_code=m.clinic_code and reg.valid_flag='1'
  where 1=1
	and m.pay_flag='0'
	and m.item_code not like 'H%' 
	--and (m.PUB_COST + m.PAY_COST + m.OWN_COST) > 0 
  and m.item_code not in('F00000011454','F00000011465','F00000011469','F00000011472','F00000011449','F00000011450','F00000011451')
  and nvl((select p.extend_flag from met_ord_recipedetail  p where p.sequence_no=m.mo_order and rownum=1),nvl(m.extend_flag,'0'))='0'
	and not exists (select 1 from fin_opb_feedetail a where a.clinic_code=m.clinic_code and a.pay_flag = '0' and a.hos_code!='CORE_HIS50' and pay_flag = '0' ) 
	and m.clinic_code='{0}' 
 and package_code is  null
 union all
select 
  m.RECIPE_NO, 
  max(SEQUENCE_NO)SEQUENCE_NO,
 m.TRANS_TYPE,
 m.CLINIC_CODE,
 m.CARD_NO,
  m.REG_DATE,
 REG_DPCD,
m.doct_code,
 DOCT_DEPT,
 package_code as item_code,
 package_name as ITEM_NAME,
 DRUG_FLAG,
 SPECS,  --  规格
 SELF_MADE, --  自制药标志
 DRUG_QUALITY, --  药品性质，麻药，普药
 DOSE_MODEL_CODE,--  剂型
 FEE_CODE, --  最小费用代码
 CLASS_CODE, --  系统类别
 sum(UNIT_PRICE)UNIT_PRICE,  --  单价
 nvl((select QTY  FROM  met_ord_recipedetail a WHERE a.clinic_code=clinic_code and a.sequence_no=mo_order and a.item_code=item_code),1)  QTY,
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
              sum(m.PUB_COST)PUB_COST, --  可报效金额
              sum(m.PAY_COST)PAY_COST, --  自付金额
              sum(m.OWN_COST)OWN_COST, --  现金金额
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
              NEW_ITEMRATE, 
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
             sum(m.ECO_COST)ECO_COST,    
             sum(m.OVER_COST)OVER_COST,
             sum(EXCESS_COST)EXCESS_COST,
             sum(DRUG_OWNCOST)DRUG_OWNCOST,
              COST_SOURCE,
              SUBJOB_FLAG,
              ACCOUNT_FLAG,
              UPDATE_SEQUENCENO,
              m.PAYKIND_CODE, --77
              m.PACT_CODE,
              sum(old_unit_price)old_unit_price,
              package_qty,
              recipe_memo,
              memo,                  --82
              DOCTINDEPT,
              MEDICALGROUPCODE,--84
              EXT_FLAG3,
              Extend_Flag,
fun_get_dept_name(m.REG_DPCD) deptname,
fun_get_employee_name(m.doct_code) doctname,
 (select  name from com_dictionary y  where y.type='DeptExecAddress' and code=m.EXEC_DPCD and rownum=1 )as  address,
RECIPE_FLAG
   from fin_opb_feedetail m
  inner join fin_opr_register reg on reg.clinic_code=m.clinic_code and reg.valid_flag='1'
  where 1=1
  and m.pay_flag='0'
  and m.item_code not like 'H%' 
  --and (m.PUB_COST + m.PAY_COST + m.OWN_COST) > 0 
  and m.item_code not in('F00000011454','F00000011465','F00000011469','F00000011472','F00000011449','F00000011450','F00000011451')
  and nvl((select p.extend_flag from met_ord_recipedetail  p where p.sequence_no=m.mo_order and rownum=1),nvl(m.extend_flag,'0'))='0'
  and not exists (select 1 from fin_opb_feedetail a where a.clinic_code=m.clinic_code and a.pay_flag = '0' and a.hos_code!='CORE_HIS50' and pay_flag = '0' ) 
and m.clinic_code='{0}' 
  and package_code is not null
 group by 
 m.RECIPE_NO, 
 m.TRANS_TYPE,
 m.CLINIC_CODE,
 m.CARD_NO,
 m.REG_DATE,
 REG_DPCD,
 m.doct_code,
 DOCT_DEPT,
 DRUG_FLAG,
 SPECS,  --  规格
              SELF_MADE, --  自制药标志
              DRUG_QUALITY, --  药品性质，麻药，普药
              DOSE_MODEL_CODE,--  剂型
              FEE_CODE, --  最小费用代码
              CLASS_CODE, --  系统类别              
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
              NEW_ITEMRATE,
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
              COST_SOURCE,
              SUBJOB_FLAG,
              ACCOUNT_FLAG,
              UPDATE_SEQUENCENO,
              m.PAYKIND_CODE, --77
              m.PACT_CODE,
              package_qty,
              recipe_memo,
              memo,                  --82
              DOCTINDEPT,
              MEDICALGROUPCODE,--84
              EXT_FLAG3,
              Extend_Flag,
              RECIPE_FLAG
", cliincCode); 
            #endregion
            if (this.ExecQuery(sql) == -1)
            {
                return null;
            }

            ArrayList feeItemLists = new ArrayList();//费用明细数组
            FeeItemList feeItemList = null;//费用明细实体

            try
            {
                //循环读取数据
                while (this.Reader.Read())
                {
                    feeItemList = new FeeItemList();

                    //feeItemList.Item.IsPharmacy = NConvert.ToBoolean(this.Reader[11].ToString());

                    feeItemList.Item.ItemType = (EnumItemType)NConvert.ToInt32(this.Reader[11]);

                    //if (feeItemList.Item.IsPharmacy)
                    if (feeItemList.Item.ItemType == EnumItemType.Drug)
                    {
                        feeItemList.Item = new Neusoft.HISFC.Models.Pharmacy.Item();
                        feeItemList.Item.ItemType = EnumItemType.Drug;
                        //feeItemList.Item.IsPharmacy = true;
                    }
                    //{40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}
                    else if (feeItemList.Item.ItemType == EnumItemType.UnDrug)
                    {
                        feeItemList.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                        //feeItemList.Item.IsPharmacy = false;
                        feeItemList.Item.ItemType = EnumItemType.UnDrug;
                    }
                    //物资 {40DFDC91-0EC1-4cd4-81BC-0EAE4DE1D3AB}
                    else
                    {
                        feeItemList.Item = new Neusoft.HISFC.Models.FeeStuff.MaterialItem();
                        feeItemList.Item.ItemType = EnumItemType.MatItem;

                    }

                    feeItemList.RecipeNO = this.Reader[0].ToString();
                    feeItemList.SequenceNO = NConvert.ToInt32(this.Reader[1].ToString());
                    if (this.Reader[2].ToString() == "1")
                    {
                        feeItemList.TransType = TransTypes.Positive;
                    }
                    else
                    {
                        feeItemList.TransType = TransTypes.Negative;
                    }
                    feeItemList.Patient.ID = this.Reader[3].ToString();
                    feeItemList.Patient.PID.CardNO = this.Reader[4].ToString();
                    ((Register)feeItemList.Patient).DoctorInfo.SeeDate = NConvert.ToDateTime(this.Reader[5].ToString());
                    ((Register)feeItemList.Patient).DoctorInfo.Templet.Dept.ID = this.Reader[6].ToString();
                    feeItemList.RecipeOper.ID = this.Reader[7].ToString();
                    ((Register)feeItemList.Patient).DoctorInfo.Templet.Doct.ID = this.Reader[7].ToString();
                    feeItemList.RecipeOper.Dept.ID = this.Reader[8].ToString();
                    feeItemList.Item.ID = this.Reader[9].ToString();
                    feeItemList.Item.Name = this.Reader[10].ToString();
                    feeItemList.Item.Specs = this.Reader[12].ToString();

                    //if (feeItemList.Item.IsPharmacy)
                    if (feeItemList.Item.ItemType == EnumItemType.Drug)
                    {
                        ((Neusoft.HISFC.Models.Pharmacy.Item)feeItemList.Item).Product.IsSelfMade = NConvert.ToBoolean(this.Reader[13].ToString());
                        ((Neusoft.HISFC.Models.Pharmacy.Item)feeItemList.Item).Quality.ID = this.Reader[14].ToString();
                        ((Neusoft.HISFC.Models.Pharmacy.Item)feeItemList.Item).DosageForm.ID = this.Reader[15].ToString();
                    }
                    feeItemList.Item.MinFee.ID = this.Reader[16].ToString();
                    feeItemList.Item.SysClass.ID = this.Reader[17].ToString();
                    feeItemList.Item.Price = NConvert.ToDecimal(this.Reader[18].ToString());
                    feeItemList.Item.Qty = NConvert.ToDecimal(this.Reader[19].ToString());
                    feeItemList.Days = NConvert.ToDecimal(this.Reader[20].ToString());
                    feeItemList.Order.Frequency.ID = this.Reader[21].ToString();
                    feeItemList.Order.Usage.ID = this.Reader[22].ToString();
                    feeItemList.Order.Usage.Name = this.Reader[23].ToString();
                    feeItemList.InjectCount = NConvert.ToInt32(this.Reader[24].ToString());
                    feeItemList.IsUrgent = NConvert.ToBoolean(this.Reader[25].ToString());
                    feeItemList.Order.Sample.ID = this.Reader[26].ToString();
                    feeItemList.Order.CheckPartRecord = this.Reader[27].ToString();
                    feeItemList.Order.DoseOnce = NConvert.ToDecimal(this.Reader[28].ToString());
                    feeItemList.Order.DoseUnit = this.Reader[29].ToString();
                    //if (feeItemList.Item.IsPharmacy)
                    if (feeItemList.Item.ItemType == EnumItemType.Drug)
                    {
                        ((Neusoft.HISFC.Models.Pharmacy.Item)feeItemList.Item).BaseDose = NConvert.ToDecimal(this.Reader[30].ToString());
                    }
                    feeItemList.Item.PackQty = NConvert.ToDecimal(this.Reader[31].ToString());
                    feeItemList.Item.PriceUnit = this.Reader[32].ToString();
                    feeItemList.FT.PubCost = NConvert.ToDecimal(this.Reader[33].ToString());
                    feeItemList.FT.PayCost = NConvert.ToDecimal(this.Reader[34].ToString());
                    feeItemList.FT.OwnCost = NConvert.ToDecimal(this.Reader[35].ToString());
                    feeItemList.ExecOper.Dept.ID = this.Reader[36].ToString();
                    feeItemList.ExecOper.Dept.Name = this.Reader[37].ToString();
                    feeItemList.Compare.CenterItem.ID = this.Reader[38].ToString();
                    feeItemList.Compare.CenterItem.ItemGrade = this.Reader[39].ToString();
                    feeItemList.Order.Combo.IsMainDrug = NConvert.ToBoolean(this.Reader[40].ToString());
                    feeItemList.Order.Combo.ID = this.Reader[41].ToString();
                    feeItemList.ChargeOper.ID = this.Reader[42].ToString();
                    feeItemList.ChargeOper.OperTime = NConvert.ToDateTime(this.Reader[43].ToString());
                    feeItemList.PayType = (PayTypes)(NConvert.ToInt32(this.Reader[44].ToString()));
                    feeItemList.CancelType = (CancelTypes)(NConvert.ToInt32(this.Reader[45].ToString()));
                    feeItemList.FeeOper.ID = this.Reader[46].ToString();
                    feeItemList.FeeOper.OperTime = NConvert.ToDateTime(this.Reader[47].ToString());
                    feeItemList.Invoice.ID = this.Reader[48].ToString();
                    feeItemList.Invoice.Type.ID = this.Reader[49].ToString();
                    feeItemList.FeeCodeStat.ID = this.Reader[49].ToString();
                    feeItemList.FeeCodeStat.SortID = NConvert.ToInt32(this.Reader[50].ToString());
                    feeItemList.IsConfirmed = NConvert.ToBoolean(this.Reader[51].ToString());
                    feeItemList.ConfirmOper.ID = this.Reader[52].ToString();
                    feeItemList.ConfirmOper.Dept.ID = this.Reader[53].ToString();
                    feeItemList.ConfirmOper.OperTime = NConvert.ToDateTime(this.Reader[54].ToString());

                    //扣库科室
                    feeItemList.StockOper.Dept.ID = feeItemList.ConfirmOper.Dept.ID;//扣库科室

                    feeItemList.InvoiceCombNO = this.Reader[55].ToString();
                    feeItemList.NewItemRate = NConvert.ToDecimal(this.Reader[56].ToString());
                    feeItemList.OrgItemRate = NConvert.ToDecimal(this.Reader[57].ToString());
                    feeItemList.ItemRateFlag = this.Reader[58].ToString();
                    feeItemList.Item.SpecialFlag1 = this.Reader[59].ToString();
                    feeItemList.Item.SpecialFlag2 = this.Reader[60].ToString();
                    feeItemList.FeePack = this.Reader[61].ToString();
                    feeItemList.UndrugComb.ID = this.Reader[62].ToString();
                    feeItemList.UndrugComb.Name = this.Reader[63].ToString();
                    feeItemList.NoBackQty = NConvert.ToDecimal(this.Reader[64].ToString());
                    feeItemList.ConfirmedQty = NConvert.ToDecimal(this.Reader[65].ToString());
                    feeItemList.ConfirmedInjectCount = NConvert.ToInt32(this.Reader[66].ToString());
                    feeItemList.Order.ID = this.Reader[67].ToString();
                    feeItemList.RecipeSequence = this.Reader[68].ToString();
                    feeItemList.FT.RebateCost = NConvert.ToDecimal(this.Reader[69].ToString());
                    feeItemList.SpecialPrice = NConvert.ToDecimal(this.Reader[70].ToString());
                    feeItemList.FT.ExcessCost = NConvert.ToDecimal(this.Reader[71].ToString());
                    feeItemList.FT.DrugOwnCost = NConvert.ToDecimal(this.Reader[72].ToString());
                    feeItemList.FTSource = this.Reader[73].ToString();
                    feeItemList.Item.IsMaterial = NConvert.ToBoolean(this.Reader[74].ToString());
                    feeItemList.IsAccounted = NConvert.ToBoolean(this.Reader[75].ToString());
                    //{143CA424-7AF9-493a-8601-2F7B1D635026}
                    //物资出库流水号
                    feeItemList.UpdateSequence = NConvert.ToInt32(this.Reader[76].ToString());

                    //判断77（结算类别）是否存在
                    if (this.Reader.FieldCount > 78)
                    {
                        feeItemList.Order.Patient.Pact.PayKind.ID = this.Reader[77].ToString();
                        feeItemList.Order.Patient.Pact.ID = this.Reader[78].ToString();
                    }

                    if (this.Reader.FieldCount > 82)
                    {
                        feeItemList.OrgPrice = NConvert.ToDecimal(this.Reader[79]);
                        feeItemList.UndrugComb.Qty = NConvert.ToDecimal(this.Reader[80]);
                        feeItemList.Order.Memo = this.Reader[81].ToString();
                        feeItemList.Memo = this.Reader[82].ToString();
                    }

                    if (this.Reader.FieldCount > 84)
                    {
                        feeItemList.DoctDeptInfo.ID = this.Reader[83].ToString();
                        feeItemList.MedicalGroupCode.ID = this.Reader[84].ToString();
                    }

                    if (this.Reader.FieldCount > 85)
                    {
                        feeItemList.FT.FTRate.User03 = this.Reader[85].ToString();
                    }

                    //处方外延标记 - MK
                    if (this.Reader.FieldCount > 86)
                    {
                        feeItemList.IsExtendRecipe = NConvert.ToBoolean(this.Reader[86].ToString());
                    }
                    if (this.Reader.FieldCount > 87)
                    {
                        ((Register)feeItemList.Patient).DoctorInfo.Templet.Dept.Name = this.Reader[87].ToString();
                    }
                    if (this.Reader.FieldCount > 88)
                    {
                        feeItemList.RecipeOper.Name = this.Reader[88].ToString();
                    }
                    if (this.Reader.FieldCount > 89)
                    {
                        feeItemList.ExecOper.Dept.Memo = this.Reader[89].ToString();
                    }
                    if (this.Reader.FieldCount > 90)
                    {
                        feeItemList.RecipeFlag = this.Reader[90].ToString();
                    }
                    feeItemLists.Add(feeItemList);
                }//循环结束

                this.Reader.Close();

                return feeItemLists;
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }

                return null;
            }
        }
        /// <summary>
        /// 查询库存数量
        /// </summary>
        /// <returns></returns>
        public int GetStore(string sql)
        {
            try
            {
                int Store = 1;
                if (this.ExecQuery(sql) == -1)
                    return 0;
                while (this.Reader.Read())
                {
                    Store = int.Parse(Reader[0].ToString()) ;//是否有库存
                }
                this.Reader.Close();
                return Store;
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                return 0;
            }


        }


    }
}
