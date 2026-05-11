using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using His.Models.BUltrasonic;
using System.Data;

namespace His.Business.BUltrasonic
{
   public class BUltrasonic
    {

        /// <summary>
        /// B超申请单
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
       public His.Models.BUltrasonic.DataSource GetApplyBill(His.Models.BUltrasonic.RequestApplyModel reqInfo)
        {
            His.Models.BUltrasonic.DataSource source = new His.Models.BUltrasonic.DataSource();
            if (reqInfo != null)
            {
                if (string.IsNullOrEmpty(reqInfo.PATIENT_TYPE))
                {
                      source.Return.ErrorMsg = "PATIENT_TYPE 不能为空！";
                    source.Return.Code = "0";
                    return source;
                }
                if (string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM) && string.IsNullOrEmpty(reqInfo.PATIENT_ID)  && string.IsNullOrEmpty(reqInfo.CARDNO) && string.IsNullOrEmpty(reqInfo.BILL_NO))
                {
                    source.Return.ErrorMsg = "APLY_FLOW_NUM,PATIENT_ID,CARDNO,BILL_NO 不能全为空！";
                    source.Return.Code = "0";
                    return source;
                }
            }
            else
            {
                source.Return.ErrorMsg = "请输入有效请求参数！";
                source.Return.Code = "0";
                return source;
            }

           
            #region sql
            string sql = string.Empty;
            if (reqInfo.PATIENT_TYPE=="1")
            {
                sql = @"
select distinct t.mo_order,
                '' aply_type,
                to_char(t.reg_date, 'HH24:MI:SS') as aply_cheate_time,
                case t.emc_flag
                  when '2' then
                   '1'
                  else
                   '0'
                end emcy_mrk, --急诊标记(急诊赋1)
                trunc(t.reg_date) as aply_date, --开单日期
                (select max(m.diag_name)
                   from met_cas_diagnose m
                  where m.main_flag = '1'
                    and m.inpatient_no = t.clinic_code) as diag_info, --医生诊断信息
                t.check_body as BODY_PART, --检查部位
                '' as MACHINE_NAME, --machine
                '' as REMARK, --mark
                t.doct_code as doc_code, --开单医生
                fun_get_employee_name(t.doct_code) as doc_name, --开单医生名字
                t.doct_dept as dept_code, --开单科室编码
                fun_get_dept_name(t.doct_dept) as dept_name, --开单科室名称
                
                p.prof_code   patient_type, --患者类型代码
                t.clinic_code patient_id, --就诊患者ID
                (select empi.empi_no from empi_paitinetinfo empi 
                where empi.card_no=f.card_no and rownum=1) empi, --患者主索引号码
                p.card_no     cardno, --卡号
                t.recipe_no   as order_id,
                p.name        patient_name, --患者姓名
                f.sex_code    patient_sex, --患者性别
                p.work_home   patient_work, --工作单位
                null          patient_regilion, --宗教信仰
                p.anaphy_flag patient_allergy, --过敏史
                p.nation_code patient_nation, --民族
                p.district    patient_origin, --
                p.home        patient_address, --住址
                p.home_tel    patient_telephone, --联系电话
                p.birthday    patient_birth, --出生日期
                f.dept_code   word_code, --病区代码
                f.dept_name   ward,
                null          room_code,
                null          room,
                null          bed_no,
                nvl(t.package_code,t.item_code) item_fee_code, --收费项目编码
                nvl(t.package_name,t.item_name) item_fee_name, --收费项目
                '1' as fee_count, --收费次数
               (select sum(fee.qty*fee.unit_price)  from fin_opb_feedetail fee where fee.pay_flag=t.pay_flag 
               /*and fee.class_code=t.class_code*/ and t.cancel_flag=fee.cancel_flag 
               and nvl(t.package_code,t.item_code)=nvl(fee.package_code,fee.item_code)
                and fee.clinic_code=t.clinic_code) as item_price --收费单价                                                                                     
  from fin_opb_feedetail t  join fin_opr_register f on t.clinic_code = f.clinic_code
  join com_patientinfo p on f.card_no = p.card_no
 
                            -- where t.exec_dpcd='7002'
                         and t.pay_flag = '1'
                         and t.class_code in( 'UC','UZ')
                         and t.cancel_flag = '1'
                         and t.pay_flag='1'
";
            }
            if (reqInfo.PATIENT_TYPE=="2")
            {
                 sql = @"select distinct t.mo_order as aply_flow_num,
                null aply_type,      
 to_char(o.mo_date, 'HH24:MI:SS') as aply_cheate_time, --开单时间          
                case o.emc_flag
                  when '2' then
                   '1'
                  else
                   '0'
                end emcy_mrk, --急诊标记(急诊赋1)
               
                trunc(o.mo_date) as aply_date, --开单日期
                (select max(m.diag_name)
                   from met_cas_diagnose m
                  where m.main_flag = '1'
                    and m.inpatient_no = t.inpatient_no) as diag_info, --医生诊断信息
                
                ITEM_NOTE as BODY_PART, --检查部位
                '' as MACHINE_NAME, --machine
                '' as REMARK, --mark
                t.recipe_doccode as doc_code, --开单医生
                fun_get_employee_name(t.recipe_doccode) as doc_name, --开单医生名字
                t.recipe_deptcode as dept_code, --开单科室编码
                fun_get_dept_name(t.recipe_deptcode) as dept_name, --开单科室名称
                f.prof_code patient_type, --患者类型代码
                f.inpatient_no patient_id, --就诊患者ID
                 (select empi.empi_no from empi_paitinetinfo empi 
                where empi.card_no=f.card_no and rownum=1) empi, --患者主索引号码
                f.patient_no cardno, --卡号
                t.recipe_no as order_id,
                f.name patient_name, --患者姓名
                f.sex_code patient_sex, --患者性别
                f.work_name patient_work, --工作单位
                null patient_regilion, --宗教信仰
                f.anaphy_flag patient_allergy, --过敏史
                f.nation_code patient_nation, --民族
                f.dist patient_origin, --
                f.home patient_address, --住址
                f.home_tel patient_telephone, --联系电话
                f.birthday patient_birth, --出生日期
                f.dept_code word_code, --病区代码
                f.dept_name ward,
                t.nurse_cell_code room_code,
                fun_get_dept_name(t.nurse_cell_code) room,
                substr(f.bed_no,5) bed_no,
                nvl(t.package_code,t.item_code) item_fee_code, --收费项目编码
                nvl(t.package_name,t.item_name) item_fee_name, --收费项目
                  '1' as fee_count, --收费次数
                 (select sum(fee.qty*fee.unit_price)  from fin_ipb_itemlist fee 
                where  nvl(t.package_code,t.item_code)=nvl(fee.package_code,fee.item_code)
                and fee.inpatient_no=t.inpatient_no) item_price --收费单价    
  from fin_ipb_itemlist t, met_ipm_order o, fin_ipr_inmaininfo f
 where t.inpatient_no = o.inpatient_no
   and t.mo_order = o.mo_order
   and t.inpatient_no = f.inpatient_no   
   and t.trans_type = '1'
   and o.class_code in ('UC','UZ')
   and t.noback_num>0
 -- and t.pay_flag='1'
  -- and t.item_name like '%B超%'
";
            }
            

            string whereSql = string.Empty;

            if (reqInfo != null)
            {
                if (reqInfo.PATIENT_TYPE=="1")
                {
                    if (!string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
                        whereSql += " and t.mo_order='" + reqInfo.APLY_FLOW_NUM + "'";
                    if (!string.IsNullOrEmpty(reqInfo.CARDNO))
                        whereSql += " and f.card_no='" + reqInfo.CARDNO + "'";
                    if (!string.IsNullOrEmpty(reqInfo.PATIENT_ID))
                        whereSql += " and t.clinic_code='" + reqInfo.PATIENT_ID + "'";
                    //if (!string.IsNullOrEmpty(reqInfo.PATIENT_NAME))
                    //    whereSql += " and p.name ='" + reqInfo.PATIENT_NAME + "'";
                    if (!string.IsNullOrEmpty(reqInfo.BILL_NO))
                        whereSql += " and t.invoice_no='" + reqInfo.BILL_NO + "'";
                    if (!string.IsNullOrEmpty(reqInfo.EXECUTIVE_DEPT))
                        whereSql += " and t.exec_dpcd='" + reqInfo.EXECUTIVE_DEPT + "'";
                    if ((!string.IsNullOrEmpty(reqInfo.START_TIME)))
                        whereSql += " and t.fee_date >=timestamp'" + reqInfo.START_TIME + "'";
                    if (!string.IsNullOrEmpty(reqInfo.END_TIME))
                        whereSql += " and t.fee_date <=timestamp'" + reqInfo.END_TIME + "'";
                }
                else
                {
                    if (!string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
                        whereSql += " and t.mo_order='" + reqInfo.APLY_FLOW_NUM + "'";
                    if (!string.IsNullOrEmpty(reqInfo.CARDNO))
                        whereSql += " and f.patient_no='" + reqInfo.CARDNO + "'";
                    if (!string.IsNullOrEmpty(reqInfo.PATIENT_ID))
                        whereSql += " and t.inpatient_no='" + reqInfo.PATIENT_ID + "'";
                    //if (!string.IsNullOrEmpty(reqInfo.PATIENT_NAME))
                    //    whereSql += " and p.name ='" + reqInfo.PATIENT_NAME + "'";
                    if (!string.IsNullOrEmpty(reqInfo.EXECUTIVE_DEPT))
                        whereSql += " and t.execute_deptcode='" + reqInfo.EXECUTIVE_DEPT + "'";
                    if (!string.IsNullOrEmpty(reqInfo.BILL_NO))
                        whereSql += " and t.invoice_no='" + reqInfo.BILL_NO + "'";
                    if ((!string.IsNullOrEmpty(reqInfo.START_TIME)))
                        whereSql += " and t.fee_date >=timestamp'" + reqInfo.START_TIME + "'";
                    if (!string.IsNullOrEmpty(reqInfo.END_TIME))
                        whereSql += " and t.fee_date <=timestamp'" + reqInfo.END_TIME + "'";
                }
              
                //if ((!string.IsNullOrEmpty(reqInfo.START_TIME)))
                //    whereSql += " and o.mo_date >=timestamp'" + reqInfo.START_TIME + "'";
                //if (!string.IsNullOrEmpty(reqInfo.END_TIME))
                //    whereSql += " and o.mo_date <=timestamp'" + reqInfo.END_TIME + "'";
            }
            else
            {
                return null;
            }
            if (!string.IsNullOrEmpty(whereSql))
                sql += whereSql;
            #endregion
            try
            {
                #region 数据赋值
               
                //申请单
                ApplyBill bill = null;
                DataTable dt = new DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    bill = new ApplyBill();
                    //applybill
                    //APLY_FLOW_NUM	APLY_TYPE	APLY_CHEATE_TIME	EMCY_MRK	APLY_DATE	DIAG_INFO	ITEM_NOTE	''	''	DOC_CODE	DOC_NAME	DEPT_CODE	DEPT_NAME	PATIENT_TYPE	PATIENT_ID	EMPI	CARDNO	ORDER_ID	PATIENT_NAME	PATIENT_SEX	PATIENT_WORK	PATIENT_REGILION	PATIENT_ALLERGY	PATIENT_NATION	PATIENT_ORIGIN	PATIENT_ADDRESS	PATIENT_TELEPHONE	PATIENT_BIRTH	WORD_CODE	WARD	ROOM_CODE	ROOM	BED_NO	ITEM_FEE_CODE	ITEM_FEE_NAME	FEE_COUNT	ITEM_PRICE
                    bill.APLY_FLOW_NUM = row[0].ToString();
                    bill.APLY_TYPE = row[1].ToString();
                    bill.APLY_CREATE_TIME = row[2].ToString();
                    bill.EMCY_MRK = row[3].ToString();
                    bill.APLY_DATE = row[4].ToString();
                    bill.DIAG_INFO = row[5].ToString();
                    bill.BODY_PART = row[6].ToString();
                    bill.MACHINE_NAME = row[7].ToString();
                    bill.REMARK = row[8].ToString();                  
                    bill.DOC_CODE = row[9].ToString();
                    bill.DOC_NAME = row[10].ToString();
                    bill.DEPT_CODE = row[11].ToString();
                    bill.DEPT_NAME = row[12].ToString();


                  
                    //patient
                    bill.PATIENT_TYPE = row[13].ToString();
                    bill.PATIENT_ID = row[14].ToString();
                    bill.EMPI = row[15].ToString();
                    bill.CARDNO = row[16].ToString();
                    bill.ORDER_ID = row[17].ToString();
                    bill.PATIENT_NAME = row[18].ToString();
                    bill.PATIENT_SEX = row[19].ToString();
                    bill.PATIENT_WORK = row[20].ToString();
                    bill.PATIENT_REGILION = row[21].ToString();
                    bill.PATIENT_ALLERGY = row[22].ToString();
                    bill.PATIENT_NATION = row[23].ToString();
                    bill.PATIENT_ORIGIN = row[24].ToString();
                    bill.PATIENT_ADDRESS = row[25].ToString();
                    bill.PATIENT_TELEPHONE = row[26].ToString();
                    bill.PATIENT_BIRTH = row[27].ToString();
                    bill.WARD_CODE = row[28].ToString();
                    bill.WARD = row[29].ToString();
                    bill.ROOM_CODE = row[30].ToString();
                    bill.ROOM = row[31].ToString();
                    bill.BED_NO = row[32].ToString();
                    //fee
                    bill.ITEM_FEE_CODE = row[33].ToString();
                    bill.ITEM_FEE_NAME = row[34].ToString();
                    bill.FEE_COUNT = row[35].ToString();
                    bill.ITEM_PRICE = row[36].ToString();
                   // bill.FEE_STATUS = row[49].ToString();


                    source.Return.Result.ExamApply.APPLYINFO.Add(bill);

                }

           
                #endregion
                source.Return.Code = "1";
                return source;
            }
            catch (Exception ex)
            {
                source.Return.ErrorMsg = ex.Message;
                source.Return.Code = "0";
                return source;
            }
        }
    }
}
