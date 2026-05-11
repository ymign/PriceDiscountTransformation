using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using His.Models.Endoscope;
using System.Data;
using His.Util.Common;
using System.Xml.Linq;

namespace His.Business.Endoscope
{
    /// <summary>
    /// 内镜 业务类
    /// </summary>
    public class Endoscope
    {
        /// <summary>
        /// 内镜门诊申请单
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public DataSource<ExamApply> GetOutPatientApplyBill(PathologicApplyBillRequestInfo reqInfo)
        {
            ExamApply examApply = new ExamApply();
            DataSource<ExamApply> source = new DataSource<ExamApply>();

            #region sql
            string sql = @" select distinct t.recipe_no as order_id,
                            decode(nvl(t.package_code,'呵呵'),'呵呵',t.item_name,t.package_name) order_name,
                            t.mo_order as aply_flow_num,
                            null aply_type,
                            case t.emc_flag when '2' then '1' else '0' end emcy_mrk, --急诊标记(急诊赋1)
                            null order_priority_code,--系统类别代码
                            null order_priority,--系统类别名称
                            ( select max(m.diag_name)
                            from met_cas_diagnose m
                            where m.main_flag = '1'
                            and m.inpatient_no = t.clinic_code ) as diag_info,--医生诊断信息
                            '' as clinic_disease,--临床病史
                            '' as operation_info,--手术资料
                            t.recipe_memo as other_info,--其它信息
                            (select l.remark from met_ord_recipedetail l
                            where l.clinic_code=t.clinic_code
                            and l.sequence_no=t.mo_order) remark,--执行说明
                            t.doct_code as doc_code,--开单医生
                            fun_get_company_name(t.doct_code) as doc_name,--开单医生名字
                            t.doct_dept as dept_code,--开单科室编码
                            fun_get_dept_name(t.doct_dept) as dept_name,--开单科室名称
                            to_char(t.reg_date,'HH24:MI:SS') as aply_cheate_time,--开单时间 
                            trunc(t.reg_date) as aply_date,--开单日期
                            t.exec_dpcd as exe_dept_code,--执行科室代码
                            fun_get_dept_name(t.exec_dpcd) as exe_dept_name,--执行科室名称
                            null bodypart_code,--取样编码
                            null bodypart_name,--取样名称
                            (select cd.code from met_ord_recipedetail d,com_dictionary cd
                            where d.clinic_code=t.clinic_code
                            and d.sequence_no=t.mo_order
                            and d.LAB_TYPE=cd.name
                            and cd.type='LABSAMPLE') /*l.LAB_TYPE */smpl_code, --        取样类型编码
                            (select f.LAB_TYPE from met_ord_recipedetail f
                            where f.clinic_code=t.clinic_code
                            and f.sequence_no=t.mo_order) smpl_name, --取样类型名称
                            null cur_case,--当前情况
                            null destination , --发送位置                           
                            p.prof_code patient_type,--患者类型代码
                            t.clinic_code patient_id,--就诊患者ID
                            (select empi.empi_no from empi_paitinetinfo empi 
                              where empi.card_no=f.card_no and rownum=1) empi, --患者主索引号码
                            p.card_no cardno,--卡号
                            p.name patient_name,--患者姓名
                            f.sex_code patient_sex,--患者性别
                            p.work_home patient_work,--工作单位
                            null patient_regilion,--宗教信仰
                            p.anaphy_flag patient_allergy,--过敏史
                            p.nation_code patient_nation,--民族
                            p.district patient_origin,--
                            p.home patient_address,--住址
                            p.home_tel patient_telephone,--联系电话
                            p.birthday patient_birth,--出生日期
                            f.dept_code word_code,--病区代码
                            f.dept_name ward,
                            null room_code,
                            null room,
                            null bed_no,
                            -- from com_patientinfo t, fin_opr_register f
                            t.item_code item_fee_code,--收费项目编码
                            t.item_name item_fee_name,--收费项目
                            t.qty fee_count,--收费次数
                            t.unit_price item_price,--收费单价
                            t.trans_type fee_status --收费状态
                            --from fin_opb_feedetail t join com_patientinfo p on t.card_no=p.card_no                                                                  
                            from fin_opb_feedetail  t join com_patientinfo p  on t.card_no=p.card_no   
                            join fin_opr_register f on  t.clinic_code=f.clinic_code     
                            where t.exec_dpcd in('7012','6003')
                            and t.pay_flag = '1'
                            and t.class_code = 'UC'
                            and t.cancel_flag ='1'
                            ";

            string whereSql = string.Empty;

            if (reqInfo != null)
            {
                if (!string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
                    whereSql += " and t.mo_order='" + reqInfo.APLY_FLOW_NUM + "'";
                if (!string.IsNullOrEmpty(reqInfo.CARDNO))
                    whereSql += " and f.card_no='" + reqInfo.CARDNO + "'";
                if (!string.IsNullOrEmpty(reqInfo.PATIENT_ID))
                    whereSql += " and t.clinic_code='" + reqInfo.PATIENT_ID + "'";
                if (!string.IsNullOrEmpty(reqInfo.PATIENT_NAME))
                    whereSql += " and p.name ='" + reqInfo.PATIENT_NAME + "'";
                if (!string.IsNullOrEmpty(reqInfo.BILL_NO))
                    whereSql += " and t.invoice_no='" + reqInfo.BILL_NO + "'";
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

            #region old
            //            #region applyBill sql
            //            string sql = @"
            //                  select distinct t.recipe_no as order_id,
            //                  decode(nvl(t.package_code,'呵呵'),'呵呵',t.item_name,t.package_name) order_name,
            //                  t.mo_order as aply_flow_num,
            //                  null aply_type,
            //                  case t.emc_flag
            //                         when '2' then
            //                          '1'
            //                         else
            //                          '0'
            //                       end emcy_mrk, --急诊标记(急诊赋1)
            //                    null order_priority_code,--系统类别代码
            //                    null order_priority,--系统类别名称
            //                    (select max(m.diag_name)
            //                          from met_cas_diagnose m
            //                         where m.main_flag = '1'
            //                           and m.inpatient_no = t.clinic_code) as diag_info,--医生诊断信息
            //                     '' as clinic_disease,--临床病史
            //                     '' as operation_info,--手术资料
            //                     t.recipe_memo as other_info,--其它信息
            //                     (select l.remark from met_ord_recipedetail l
            //                               where l.clinic_code=t.clinic_code
            //                                     and l.sequence_no=t.mo_order) remark,--执行说明
            //                     t.doct_code as doc_code,--开单医生
            //                     fun_get_company_name(t.doct_code) as doc_name,--开单医生名字
            //                     t.doct_dept as dept_code,--开单科室编码
            //                     fun_get_dept_name(t.doct_dept) as dept_name,--开单科室名称
            //                     to_char(t.reg_date,'HH24:MI:SS') as aply_cheate_time,--开单时间 
            //                     trunc(t.reg_date) as aply_date,--开单日期
            //                     t.exec_dpcd as exe_dept_code,--执行科室代码
            //                     fun_get_dept_name(t.exec_dpcd) as exe_dept_name,--执行科室名称
            //                     null bodypart_code,--取样编码
            //                     null bodypart_name,--取样名称
            //                     (select cd.code from met_ord_recipedetail d,com_dictionary cd
            //                               where d.clinic_code=t.clinic_code
            //                                     and d.sequence_no=t.mo_order
            //                                     and d.LAB_TYPE=cd.name
            //                                     and cd.type='LABSAMPLE') /*l.LAB_TYPE */smpl_code, --        取样类型编码
            //                       (select f.LAB_TYPE from met_ord_recipedetail f
            //                               where f.clinic_code=t.clinic_code
            //                                     and f.sequence_no=t.mo_order) smpl_name, --取样类型名称
            //                      null cur_case,--当前情况
            //                      null destination  --发送位置               
            //                   from fin_opb_feedetail  t join com_patientinfo p on t.card_no=p.card_no
            //                       -- join 
            //                   where t.exec_dpcd='7012'
            //                   and t.pay_flag = '1'
            //                   and t.class_code = 'UC'
            //                   and t.cancel_flag ='1'
            //                   ";
            //            string whereSql = string.Empty;

            //            if (reqInfo != null)
            //            {
            //                if (!string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
            //                    whereSql += " and t.mo_order='" + reqInfo.APLY_FLOW_NUM + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.CARDNO))
            //                    whereSql += " and t.card_no='" + reqInfo.CARDNO + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.PATIENT_ID))
            //                    whereSql += " and t.clinic_code='" + reqInfo.PATIENT_ID + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.PATIENT_NAME))
            //                    whereSql += " and p.name ='" + reqInfo.PATIENT_NAME + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.BILL_NO))
            //                    whereSql += " and t.invoice_no='" + reqInfo.BILL_NO + "'";
            //                //if ((!string.IsNullOrEmpty(reqInfo.START_TIME)))
            //                //    whereSql += " and o.mo_date >=timestamp'" + reqInfo.START_TIME + "'";
            //                //if (!string.IsNullOrEmpty(reqInfo.END_TIME))
            //                //    whereSql += " and o.mo_date <=timestamp'" + reqInfo.END_TIME + "'";
            //            }
            //            else
            //            {
            //                return null;
            //            }
            //            if (!string.IsNullOrEmpty(whereSql))
            //                sql += whereSql;
            //            #endregion

            //            #region patientInfo sql
            //            string patientSql = @"select t.prof_code patient_type,--患者类型代码
            //                                   f.clinic_code patient_id,--就诊患者ID
            //                                   null empi,--患者主索引号码
            //                                   t.card_no cardno,--卡号
            //                                   t.name patient_name,--患者姓名
            //                                   f.sex_code patient_sex,--患者性别
            //                                   t.work_home patient_work,--工作单位
            //                                   null patient_regilion,--宗教信仰
            //                                   t.anaphy_flag patient_allergy,--过敏史
            //                                   t.nation_code patient_nation,--民族
            //                                   t.district patient_origin,--
            //                                   t.home patient_address,--住址
            //                                   t.home_tel patient_telephone,--联系电话
            //                                   t.birthday patient_birth,--出生日期
            //                                   f.dept_code word_code,--病区代码
            //                                   f.dept_name ward,
            //                                   null room_code,
            //                                   null room,
            //                                   null bed_no
            //                                   from com_patientinfo t, fin_opr_register f
            //                                   where t.card_no = f.card_no";

            //            string patientWhereSql = string.Empty;
            //            if (reqInfo != null)
            //            {
            //                if (!string.IsNullOrEmpty(reqInfo.CARDNO))
            //                    patientWhereSql += " and t.card_no='" + reqInfo.CARDNO + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.PATIENT_ID))
            //                    patientWhereSql += " and f.clinic_code='" + reqInfo.PATIENT_ID + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.PATIENT_NAME))
            //                    patientWhereSql += " and t.name ='" + reqInfo.PATIENT_NAME + "'";
            //            }
            //            if (!string.IsNullOrEmpty(patientWhereSql))
            //                patientSql += patientWhereSql;


            //            string feeSql = @"select t.item_code item_fee_code,--收费项目编码
            //                            t.item_name item_fee_name,--收费项目
            //                            t.qty fee_count,--收费次数
            //                            t.unit_price item_price,--收费单价
            //                            t.trans_type fee_status --收费状态
            //                            from fin_opb_feedetail t join com_patientinfo p on t.card_no=p.card_no
            //                            where t.class_code='UC'
            //                            and t.drug_flag='0'
            //                            and t.exec_dpcd='7012'
            //                            and t.trans_type='1'";
            //            string feeWhereSql = string.Empty;

            //            if (reqInfo != null)
            //            {
            //                if (!string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
            //                    feeWhereSql += " and t.mo_order='" + reqInfo.APLY_FLOW_NUM + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.CARDNO))
            //                    feeWhereSql += " and t.card_no='" + reqInfo.CARDNO + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.PATIENT_ID))
            //                    feeWhereSql += " and t.clinic_code='" + reqInfo.PATIENT_ID + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.PATIENT_NAME))
            //                    feeWhereSql += " and p.name ='" + reqInfo.PATIENT_NAME + "'";
            //                if (!string.IsNullOrEmpty(reqInfo.BILL_NO))
            //                    feeWhereSql += " and t.invoice_no='" + reqInfo.BILL_NO + "'";

            //            }
            //            if (!string.IsNullOrEmpty(feeWhereSql))
            //                feeSql += feeWhereSql;



            //            #endregion
            #endregion
            try
            {
                #region 数据赋值
                #region old
                ////sql = string.Format(sql, reqInfo.APLY_FLOW_NUM, reqInfo.PATIENT_ID);

                ////申请单
                //ApplyBill bill = null;
                //DataTable dt = new DataTable();
                //dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    bill = new ApplyBill();
                //    bill.ORDER_ID = dt.Rows[i][0].ToString();
                //    bill.ORDER_NAME = dt.Rows[i][1].ToString();
                //    bill.APLY_FLOW_NUM = dt.Rows[i][2].ToString();
                //    bill.APLY_TYPE = dt.Rows[i][3].ToString();
                //    bill.EMCY_MRK = dt.Rows[i][4].ToString();
                //    bill.ORDER_PRIORITY_CODE = dt.Rows[i][5].ToString();
                //    bill.ORDER_PRIORITY = dt.Rows[i][6].ToString();
                //    bill.DIAG_INFO = dt.Rows[i][7].ToString();
                //    bill.CLINIC_DISEASE = dt.Rows[i][8].ToString();
                //    bill.OPERATION_INFO = dt.Rows[i][9].ToString();
                //    bill.OTHER_INFO = dt.Rows[i][10].ToString();
                //    bill.REMARK = dt.Rows[i][11].ToString();
                //    bill.DOC_CODE = dt.Rows[i][12].ToString();
                //    bill.DOC_NAME = dt.Rows[i][13].ToString();
                //    bill.DEPT_CODE = dt.Rows[i][14].ToString();
                //    bill.DEPT_NAME = dt.Rows[i][15].ToString();
                //    bill.APLY_CREATE_TIME = dt.Rows[i][16].ToString();
                //    bill.APLY_DATE = dt.Rows[i][17].ToString();
                //    bill.EXE_DEPT_CODE = dt.Rows[i][18].ToString();
                //    bill.EXE_DEPT_NAME = dt.Rows[i][19].ToString();
                //    bill.BODYPART_CODE = dt.Rows[i][20].ToString();
                //    bill.BODYPART_NAME = dt.Rows[i][21].ToString();
                //    bill.SAMPLE_CODE = dt.Rows[i][22].ToString();
                //    bill.SAMPLE_NAME = dt.Rows[i][23].ToString();
                //    bill.CUR_CASE = dt.Rows[i][24].ToString();
                //    bill.DESTINATION = dt.Rows[i][25].ToString();
                //    examApply.APPLYINFO.Add(bill);

                //}

                //// 病人信息
                //DataTable dtPatient = new DataTable();
                //PatientInfo patient;
                //dtPatient = DataBaseHelp.DataExecHelp.GetDataTable(patientSql);
                //if (dtPatient.Rows.Count > 0)
                //{
                //    foreach (DataRow row in dtPatient.Rows)
                //    {
                //        //	PATIENT_TYPE	PATIENT_ID	EMPI	CARDNO	PATIENT_NAME	PATIENT_SEX	PATIENT_WORK	PATIENT_REGILION	PATIENT_ALLERGY	PATIENT_NATION	PATIENT_ORIGIN	PATIENT_ADDRESS	PATIENT_TELEPHONE	PATIENT_BIRTH	WORD_CODE	WARD	ROOM_CODE	ROOM	BED_NO
                //        patient = new PatientInfo();
                //        patient.PATIENT_TYPE = row[0].ToString();
                //        patient.PATIENT_ID = row[1].ToString();
                //        patient.EMPI = row[2].ToString();
                //        patient.CARDNO = row[3].ToString();
                //        patient.PATIENT_NAME = row[4].ToString();
                //        patient.PATIENT_SEX = row[5].ToString();
                //        patient.PATIENT_WORK = row[6].ToString();
                //        patient.PATIENT_REGILION = row[7].ToString();
                //        patient.PATIENT_ALLERGY = row[8].ToString();
                //        patient.PATIENT_NATION = row[9].ToString();
                //        patient.PATIENT_ORIGIN = row[10].ToString();
                //        patient.PATIENT_ADDRESS = row[11].ToString();
                //        patient.PATIENT_TELEPHONE = row[12].ToString();
                //        patient.PATIENT_BIRTH = row[13].ToString();
                //        patient.WARD_CODE = row[14].ToString();
                //        patient.WARD = row[15].ToString();
                //        patient.ROOM_CODE = row[16].ToString();
                //        patient.ROOM = row[17].ToString();
                //        patient.BED_NO = row[18].ToString();
                //       examApply.PATIENTINFO = patient;

                //    }
                //}

                //DataTable dtFee = new DataTable();
                //ApplyChargeInfo feeInfo = new ApplyChargeInfo();
                //dtFee = DataBaseHelp.DataExecHelp.GetDataTable(feeSql);
                //if (dt.Rows.Count > 0)
                //{
                //    foreach (DataRow row in dt.Rows)
                //    {
                //        feeInfo.ITEM_FEE_CODE = row[0].ToString();
                //        feeInfo.ITEM_FEE_NAME = row[1].ToString();
                //        feeInfo.FEE_COUNT = row[2].ToString();
                //        feeInfo.ITEM_PRICE = row[3].ToString();
                //        feeInfo.FEE_STATUS = row[4].ToString();
                //        examApply.FEEINFO.Add(feeInfo);
                //    }
                //}
                #endregion
                ApplyBill bill = null;
                DataTable dt = new DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    bill = new ApplyBill();
                    //applybill
                    bill.ORDER_ID = row[0].ToString();
                    bill.ORDER_NAME = row[1].ToString();
                    bill.APLY_FLOW_NUM = row[2].ToString();
                    bill.APLY_TYPE = row[3].ToString();
                    bill.EMCY_MRK = row[4].ToString();
                    bill.ORDER_PRIORITY_CODE = row[5].ToString();
                    bill.ORDER_PRIORITY = row[6].ToString();
                    bill.DIAG_INFO = row[7].ToString();
                    bill.CLINIC_DISEASE = row[8].ToString();
                    bill.OPERATION_INFO = row[9].ToString();
                    bill.OTHER_INFO = row[10].ToString();
                    bill.REMARK = row[11].ToString();
                    bill.DOC_CODE = row[12].ToString();
                    bill.DOC_NAME = row[13].ToString();
                    bill.DEPT_CODE = row[14].ToString();
                    bill.DEPT_NAME = row[15].ToString();
                    bill.APLY_CREATE_TIME = row[16].ToString();
                    bill.APLY_DATE = row[17].ToString();
                    bill.EXE_DEPT_CODE = row[18].ToString();
                    bill.EXE_DEPT_NAME = row[19].ToString();
                    bill.BODYPART_CODE = row[20].ToString();
                    bill.BODYPART_NAME = row[21].ToString();
                    bill.SAMPLE_CODE = row[22].ToString();
                    bill.SAMPLE_NAME = row[23].ToString();
                    bill.CUR_CASE = row[24].ToString();
                    bill.DESTINATION = row[25].ToString();
                    //patient
                    bill.PATIENT_TYPE = row[26].ToString();
                    bill.PATIENT_ID = row[27].ToString();
                    bill.EMPI = row[28].ToString();
                    bill.CARDNO = row[29].ToString();
                    bill.PATIENT_NAME = row[30].ToString();
                    bill.PATIENT_SEX = row[31].ToString();
                    bill.PATIENT_WORK = row[32].ToString();
                    bill.PATIENT_REGILION = row[33].ToString();
                    bill.PATIENT_ALLERGY = row[34].ToString();
                    bill.PATIENT_NATION = row[35].ToString();
                    bill.PATIENT_ORIGIN = row[36].ToString();
                    bill.PATIENT_ADDRESS = row[37].ToString();
                    bill.PATIENT_TELEPHONE = row[38].ToString();
                    bill.PATIENT_BIRTH = row[39].ToString();
                    bill.WARD_CODE = row[40].ToString();
                    bill.WARD = row[41].ToString();
                    bill.ROOM_CODE = row[42].ToString();
                    bill.ROOM = row[43].ToString();
                    bill.BED_NO = row[44].ToString();
                    //fee
                    bill.ITEM_FEE_CODE = row[45].ToString();
                    bill.ITEM_FEE_NAME = row[46].ToString();
                    bill.FEE_COUNT = row[47].ToString();
                    bill.ITEM_PRICE = row[48].ToString();
                    bill.FEE_STATUS = row[49].ToString();

                    examApply.APPLYINFO.Add(bill);

                }


                #endregion
                source.Return.Code = "1";
                source.Return.Result.ExamApply = examApply;
                return source;
            }
            catch (Exception ex)
            {
                source.Return.ErrorMsg = ex.Message;
                source.Return.Code = "0";
                HisLog.WriteLog(His.Models.Common.HisLogType.Endoscope, "his.business.CancelCheckIn 发生程序错误：" + ex.Message);
                return source;
            }
        }

        /// <summary>
        /// 内镜住院申请单
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public DataSource<ExamApply> GetInpPatientApplyBill(PathologicApplyBillRequestInfo reqInfo)
        {
            DataSource<ExamApply> source = new DataSource<ExamApply>();
            if (string.IsNullOrEmpty(reqInfo.CARDNO)&&string.IsNullOrEmpty(reqInfo.PATIENT_ID)&&string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
            {
                source.Return.Code = "0";
                source.Return.ErrorMsg = "住院号&住院流水号&申请单号不能为空!";
                return source;
 
            }
           
            #region sql
            string sql = string.Empty, whereSql = string.Empty;
            sql = @"
                         select distinct t.exec_sqn as order_id,
                          --decode(nvl(t.package_code,'呵呵'),'呵呵',t.item_name,/*t.package_name*/) order_name,
                          t.undrug_name as order_name,
                          t.mo_order as aply_flow_num,
                          null aply_type,
                          case o.emc_flag when '2' then '1'  else '0' end emcy_mrk, --急诊标记(急诊赋1)
                          o.class_code order_priority_code,--系统类别代码
                          o.class_name order_priority,--系统类别名称
                          (select max(m.diag_name)
                          from met_cas_diagnose m
                          where m.main_flag = '1'
                          and m.inpatient_no = t.inpatient_no) as diag_info,--医生诊断信息
                          '' as clinic_disease,--临床病史
                          '' as operation_info,--手术资料
                          o.mark1 as other_info,--其它信息
                          null remark,--执行说明 
                          t.doc_code as doc_code,--开单医生
                          t.doc_name as doc_name,--开单医生名字
                          --t.recipe_deptcode as dept_code,--开单科室编码
                          t.list_dpcd as dept_code,--开单科室编码
                          fun_get_dept_name(t.list_dpcd) as dept_name,--开单科室名称
                          to_char(o.mo_date,'HH24:MI:SS') as aply_cheate_time,--开单时间 
                          trunc(o.mo_date) as aply_date,--开单日期
                          t.exec_dpcd as exe_dept_code,--执行科室代码
                          t.exec_dpnm as exe_dept_name,--执行科室名称
                          null bodypart_code,--取样编码
                          null bodypart_name,--取样名称
                          null smpl_code, --        取样类型编码
                         o.lab_code as smpl_name, --取样类型名称   
                          null cur_case,--当前情况
                          null destination , --发送位置
                         f.prof_code patient_type,--患者类型代码
                         f.inpatient_no patient_id,--就诊患者ID
                        (select empi.empi_no from empi_paitinetinfo empi 
                        where empi.card_no=f.card_no and rownum=1) empi, --患者主索引号码
                         f.patient_no cardno,--卡号
                         f.name patient_name,--患者姓名
                         f.sex_code patient_sex,--患者性别
                         f.work_name patient_work,--工作单位
                         null patient_regilion,--宗教信仰
                         f.anaphy_flag patient_allergy,--过敏史
                         f.nation_code patient_nation,--民族
                         f.dist patient_origin,--
                         f.home patient_address,--住址
                         f.home_tel patient_telephone,--联系电话
                         f.birthday patient_birth,--出生日期
                         f.dept_code word_code,--病区代码
                         f.dept_name ward,
                         t.nurse_cell_code room_code,
                         (select x.dept_name from com_department x where x.dept_code=t.nurse_cell_code and rownum=1) room,
                         substr(f.bed_no,5) bed_no ,
                         i.item_code item_fee_code,--收费项目编码
                         i.item_name item_fee_name,--收费项目
                         i.qty fee_count,--收费次数
                        i.unit_price  item_price,--收费单价
                        i.trans_type fee_status --收费状态   
                        from  met_ipm_execundrug t join met_ipm_order o on t.inpatient_no=o.inpatient_no and t.mo_order=o.mo_order
                        join fin_ipr_inmaininfo f on f.inpatient_no=t.inpatient_no
                        left join fin_ipb_itemlist i on t.mo_order=i.mo_order and t.inpatient_no=i.inpatient_no
                        where t.exec_dpcd='7012'         
                        and o.class_code='UC'
                        and t.charge_state='1'
";

            if (reqInfo != null)
            {
                if (!string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
                    whereSql += " and t.mo_order='" + reqInfo.APLY_FLOW_NUM + "'";
                if (!string.IsNullOrEmpty(reqInfo.CARDNO))
                    whereSql += " and f.patient_no='" + reqInfo.CARDNO + "'";
                if (!string.IsNullOrEmpty(reqInfo.PATIENT_ID))
                    whereSql += " and t.inpatient_no='" + reqInfo.PATIENT_ID + "'";
                if (!string.IsNullOrEmpty(reqInfo.PATIENT_NAME))
                    whereSql += " and f.name ='" + reqInfo.PATIENT_NAME + "'";
                if (!string.IsNullOrEmpty(reqInfo.BILL_NO))
                    whereSql += " and i.invoice_no='" + reqInfo.BILL_NO + "'";
                if ((!string.IsNullOrEmpty(reqInfo.START_TIME)))
                    whereSql += " and i.fee_date >=timestamp'" + reqInfo.START_TIME + "'";
                if (!string.IsNullOrEmpty(reqInfo.END_TIME))
                    whereSql += " and i.fee_date <=timestamp'" + reqInfo.END_TIME + "'";
            }
            else
            {
                source.Return.Code = "0";
                source.Return.ErrorMsg = "request 信息不能为空！";
                return source;
            }

            sql += whereSql;

            #endregion

            try
            {
                #region 数据赋值

                //                //申请单
                //                ApplyBill bill = null;
                //                DataTable dt = new DataTable();
                //                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                //                for (int i = 0; i < dt.Rows.Count; i++)
                //                {
                //                    bill = new ApplyBill();
                //                    bill.ORDER_ID = dt.Rows[i][0].ToString();
                //                    bill.ORDER_NAME = dt.Rows[i][1].ToString();
                //                    bill.APLY_FLOW_NUM = dt.Rows[i][2].ToString();
                //                    bill.APLY_TYPE = dt.Rows[i][3].ToString();
                //                    bill.EMCY_MRK = dt.Rows[i][4].ToString();
                //                    bill.ORDER_PRIORITY_CODE = dt.Rows[i][5].ToString();
                //                    bill.ORDER_PRIORITY = dt.Rows[i][6].ToString();
                //                    bill.DIAG_INFO = dt.Rows[i][7].ToString();
                //                    bill.CLINIC_DISEASE = dt.Rows[i][8].ToString();
                //                    bill.OPERATION_INFO = dt.Rows[i][9].ToString();
                //                    bill.OTHER_INFO = dt.Rows[i][10].ToString();
                //                    bill.REMARK = dt.Rows[i][11].ToString();
                //                    bill.DOC_CODE = dt.Rows[i][12].ToString();
                //                    bill.DOC_NAME = dt.Rows[i][13].ToString();
                //                    bill.DEPT_CODE = dt.Rows[i][14].ToString();
                //                    bill.DEPT_NAME = dt.Rows[i][15].ToString();
                //                    bill.APLY_CREATE_TIME = dt.Rows[i][16].ToString();
                //                    bill.APLY_DATE = dt.Rows[i][17].ToString();
                //                    bill.EXE_DEPT_CODE = dt.Rows[i][18].ToString();
                //                    bill.EXE_DEPT_NAME = dt.Rows[i][19].ToString();
                //                    bill.BODYPART_CODE = dt.Rows[i][20].ToString();
                //                    bill.BODYPART_NAME = dt.Rows[i][21].ToString();
                //                    bill.SAMPLE_CODE = dt.Rows[i][22].ToString();
                //                    bill.SAMPLE_NAME = dt.Rows[i][23].ToString();
                //                    bill.CUR_CASE = dt.Rows[i][24].ToString();
                //                    bill.DESTINATION = dt.Rows[i][25].ToString();
                //                    source.Return.Result.ExamApply.APPLYINFO.Add(bill);

                //                }
                //                // 病人信息
                //                DataTable dtPatient = new DataTable();
                //                PatientInfo patient;
                //                dtPatient = DataBaseHelp.DataExecHelp.GetDataTable(patientSql);
                //                if (dtPatient.Rows.Count > 0)
                //                {
                //                    foreach (DataRow row in dtPatient.Rows)
                //                    {
                //                        //	PATIENT_TYPE	PATIENT_ID	EMPI	CARDNO	PATIENT_NAME	PATIENT_SEX	PATIENT_WORK	PATIENT_REGILION	PATIENT_ALLERGY	PATIENT_NATION	PATIENT_ORIGIN	PATIENT_ADDRESS	PATIENT_TELEPHONE	PATIENT_BIRTH	WORD_CODE	WARD	ROOM_CODE	ROOM	BED_NO
                //                        patient = new PatientInfo();
                //                        patient.PATIENT_TYPE = row[0].ToString();
                //                        patient.PATIENT_ID = row[1].ToString();
                //                        patient.EMPI = row[2].ToString();
                //                        patient.CARDNO = row[3].ToString();
                //                        patient.PATIENT_NAME = row[4].ToString();
                //                        patient.PATIENT_SEX = row[5].ToString();
                //                        patient.PATIENT_WORK = row[6].ToString();
                //                        patient.PATIENT_REGILION = row[7].ToString();
                //                        patient.PATIENT_ALLERGY = row[8].ToString();
                //                        patient.PATIENT_NATION = row[9].ToString();
                //                        patient.PATIENT_ORIGIN = row[10].ToString();
                //                        patient.PATIENT_ADDRESS = row[11].ToString();
                //                        patient.PATIENT_TELEPHONE = row[12].ToString();
                //                        patient.PATIENT_BIRTH = row[13].ToString();
                //                        patient.WARD_CODE = row[14].ToString();
                //                        patient.WARD = row[15].ToString();
                //                        patient.ROOM_CODE = row[16].ToString();
                //                        patient.ROOM = row[17].ToString();
                //                        patient.BED_NO = row[18].ToString();
                //                        source.Return.Result.ExamApply.PATIENTINFO = patient;

                //                    }
                //                }

                //                DataTable dtFee = new DataTable();
                //                ApplyChargeInfo feeInfo = new ApplyChargeInfo();
                //                dtFee = DataBaseHelp.DataExecHelp.GetDataTable(feeSql);
                //                if (dt.Rows.Count > 0)
                //                {
                //                    foreach (DataRow row in dt.Rows)
                //                    {
                //                        feeInfo.ITEM_FEE_CODE = row[0].ToString();
                //                        feeInfo.ITEM_FEE_NAME = row[1].ToString();
                //                        feeInfo.FEE_COUNT = row[2].ToString();
                //                        feeInfo.ITEM_PRICE = row[3].ToString();
                //                        feeInfo.FEE_STATUS = row[4].ToString();
                //                        source.Return.Result.ExamApply.FEEINFO.Add(feeInfo);
                //                    }
                //                }
                #endregion

                #region
                ApplyBill bill = null;
                DataTable dt = new DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    source.Return.Code = "0";
                    source.Return.ErrorMsg = "没有找到相关数据！";
                    return source;
                }
                source.Return.Result.ExamApply = new ExamApply();
                foreach (DataRow row in dt.Rows)
                {
                    bill = new ApplyBill();
                    //applybill
                    bill.ORDER_ID = row[0].ToString();
                    bill.ORDER_NAME = row[1].ToString();
                    bill.APLY_FLOW_NUM = row[2].ToString();
                    bill.APLY_TYPE = row[3].ToString();
                    bill.EMCY_MRK = row[4].ToString();
                    bill.ORDER_PRIORITY_CODE = row[5].ToString();
                    bill.ORDER_PRIORITY = row[6].ToString();
                    bill.DIAG_INFO = row[7].ToString();
                    bill.CLINIC_DISEASE = row[8].ToString();
                    bill.OPERATION_INFO = row[9].ToString();
                    bill.OTHER_INFO = row[10].ToString();
                    bill.REMARK = row[11].ToString();
                    bill.DOC_CODE = row[12].ToString();
                    bill.DOC_NAME = row[13].ToString();
                    bill.DEPT_CODE = row[14].ToString();
                    bill.DEPT_NAME = row[15].ToString();
                    bill.APLY_CREATE_TIME = row[16].ToString();
                    bill.APLY_DATE = row[17].ToString();
                    bill.EXE_DEPT_CODE = row[18].ToString();
                    bill.EXE_DEPT_NAME = row[19].ToString();
                    bill.BODYPART_CODE = row[20].ToString();
                    bill.BODYPART_NAME = row[21].ToString();
                    bill.SAMPLE_CODE = row[22].ToString();
                    bill.SAMPLE_NAME = row[23].ToString();
                    bill.CUR_CASE = row[24].ToString();
                    bill.DESTINATION = row[25].ToString();
                    //patient
                    bill.PATIENT_TYPE = row[26].ToString();
                    bill.PATIENT_ID = row[27].ToString();
                    bill.EMPI = row[28].ToString();
                    bill.CARDNO = row[29].ToString();
                    bill.PATIENT_NAME = row[30].ToString();
                    bill.PATIENT_SEX = row[31].ToString();
                    bill.PATIENT_WORK = row[32].ToString();
                    bill.PATIENT_REGILION = row[33].ToString();
                    bill.PATIENT_ALLERGY = row[34].ToString();
                    bill.PATIENT_NATION = row[35].ToString();
                    bill.PATIENT_ORIGIN = row[36].ToString();
                    bill.PATIENT_ADDRESS = row[37].ToString();
                    bill.PATIENT_TELEPHONE = row[38].ToString();
                    bill.PATIENT_BIRTH = row[39].ToString();
                    bill.WARD_CODE = row[40].ToString();
                    bill.WARD = row[41].ToString();
                    bill.ROOM_CODE = row[42].ToString();
                    bill.ROOM = row[43].ToString();
                    bill.BED_NO = row[44].ToString();
                    //fee
                    bill.ITEM_FEE_CODE = row[45].ToString();
                    bill.ITEM_FEE_NAME = row[46].ToString();
                    bill.FEE_COUNT = row[47].ToString();
                    bill.ITEM_PRICE = row[48].ToString();
                    bill.FEE_STATUS = row[49].ToString();

                    source.Return.Result.ExamApply.APPLYINFO.Add(bill);// apply = new ExamApply();
                    //apply.Add(bill);

                }
                #endregion


                source.Return.Code = "1";
                source.Return.ErrorMsg = "Successful";
                return source;

            }
            catch (Exception ex)
            {
                source.Return.ErrorMsg = ex.Message;
                source.Return.Code = "0";
                HisLog.WriteLog(His.Models.Common.HisLogType.Endoscope, "his.business.CancelCheckIn 发生程序错误：" + ex.Message);
                return source;
            }

        }

        /// <summary>
        /// 内镜申请单接收确认
        /// 确认后更改his申请单状态
        /// 不可退费
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        //        public DataSource<ExamApply> PathologicReceivedConfirm(SampleReceivedRequestInfo reqInfo)
        //        {
        //            DataSource source = new DataSource();
        //            string tabName = string.Empty;
        //            string msg = string.Empty;
        //            if (string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM) && string.IsNullOrEmpty(reqInfo.ORDER_ID) &&
        //                string.IsNullOrEmpty(reqInfo.PATIENT_TYPE))
        //            {
        //                source.Return.Code = "0";
        //                source.Return.ErrorMsg = "医嘱单号,申请单号,患者来源(类型)不能为空！";
        //                //return source;
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    if (reqInfo.PATIENT_TYPE == "0")
        //                        tabName = "fin_opb_feedetail";
        //                    else
        //                        tabName = "  fin_ipb_itemlist";

        //                    string sql = @"update {0} a
        //                            set  a.noback_num=0.00
        //                            where  a.recipe_no='{1}' 
        //                            and a.mo_order='{2}' ";
        //                    sql = string.Format(sql, tabName, reqInfo.ORDER_ID, reqInfo.APLY_FLOW_NUM);


        //                    if (DataBaseHelp.DataExecHelp.ExecSql(sql, ref msg))
        //                    {
        //                        source.Return.Code = "1";
        //                        source.Return.ErrorMsg = "接收成功！";
        //                    }
        //                    else
        //                    {
        //                        source.Return.Code = "0";
        //                        source.Return.ErrorMsg = msg;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    source.Return.Code = "0";
        //                    source.Return.ErrorMsg = ex.Message;
        //                }

        //            }
        //            return source;
        // }



        public DataSource<List<ApplyChargeInfo>> EndoscopeFeeStatus(FeeStatusRequestInfo reqInfo)
        {
            DataSource<List<ApplyChargeInfo>> source = new DataSource<List<ApplyChargeInfo>>();
            if (string.IsNullOrEmpty(reqInfo.PATIENT_TYPE) && string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
            {
                source.Return.Code = "0";
                source.Return.ErrorMsg = "PATIENT_TYPE与APLY_FLOW_NUM 不能为空！";
                return source;
            }
            string sql = string.Empty;
            if (reqInfo.PATIENT_TYPE == "0")
            {


                sql = @"select distinct t.item_code item_fee_code,--收费项目编码
                            t.item_name item_fee_name,--收费项目
                            t.qty fee_count,--收费次数
                            t.unit_price item_price,--收费单价
                            t.pay_flag fee_status --收费状态
                            from fin_opb_feedetail t join com_patientinfo p on t.card_no=p.card_no
                            where t.class_code='UC'
                            and t.drug_flag='0'
                            and t.exec_dpcd='7012'
                            and t.trans_type='1'
";
                string feeWhereSql = string.Empty;

                //if (string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM) && string.IsNullOrEmpty(reqInfo.ORDER_ID))
                //{ }
                if (!string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
                    feeWhereSql += " and t.mo_order='" + reqInfo.APLY_FLOW_NUM + "'";
                if (!string.IsNullOrEmpty(reqInfo.CARDNO))
                    feeWhereSql += " and t.card_no='" + reqInfo.CARDNO + "'";
                if (!string.IsNullOrEmpty(reqInfo.PATIENT_ID))
                    feeWhereSql += " and t.clinic_code='" + reqInfo.PATIENT_ID + "'";
                if (!string.IsNullOrEmpty(reqInfo.PATIENT_NAME))
                    feeWhereSql += " and p.name ='" + reqInfo.PATIENT_NAME + "'";
                //if (!string.IsNullOrEmpty(reqInfo.BILL_NO))
                //    feeWhereSql += " and t.invoice_no='" + reqInfo.BILL_NO + "'";
                if (!string.IsNullOrEmpty(feeWhereSql))
                {
                    sql += feeWhereSql;
                }
            }
            else
            {
                sql = @"select distinct t.item_code item_fee_code,--收费项目编码
                        t.item_name item_fee_name,--收费项目
                        t.qty fee_count,--收费次数
                        t.unit_price item_price,--收费单价
                        t.send_flag fee_status --收费状态
                        from  fin_ipb_itemlist t 
                        join fin_ipr_inmaininfo p  on t.inpatient_no=p.inpatient_no 
                        where t.class_code='UC'
                        and t.drug_flag='0'
                        and t.exec_dpcd='7012'
                        and t.trans_type='1'
";
                string feeWhereSql = string.Empty;

                //if (string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM) && string.IsNullOrEmpty(reqInfo.ORDER_ID))
                //{ }
                if (!string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM))
                    feeWhereSql += " and t.mo_order='" + reqInfo.APLY_FLOW_NUM + "'";
                if (!string.IsNullOrEmpty(reqInfo.APLY_ITM_CODE))
                    feeWhereSql += " and t.APLY_ITM_CODE='" + reqInfo.CARDNO + "'";
                if (!string.IsNullOrEmpty(reqInfo.PATIENT_ID))
                    feeWhereSql += " and p.patient_no='" + reqInfo.PATIENT_ID + "'";
                if (!string.IsNullOrEmpty(reqInfo.PATIENT_NAME))
                    feeWhereSql += " and p.name ='" + reqInfo.PATIENT_NAME + "'";
                if (!string.IsNullOrEmpty(reqInfo.ORDER_ID))
                    feeWhereSql += " and t.recipe_no='" + reqInfo.ORDER_ID + "'";
                if (!string.IsNullOrEmpty(feeWhereSql))
                {
                    sql += feeWhereSql;
                }
            }

            try
            {
                DataSet ds = DataBaseHelp.DataExecHelp.GetDataSet(sql);
                List<ApplyChargeInfo> feeList = new List<ApplyChargeInfo>();
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    
                    // ds.DataSetName = "HisObj";
                    // ds.Tables[0].TableName = "FEEINFO";
                    // string xml = ds.GetXml();


                    //// xml = xml.Replace(ds.DataSetName, "ApplyChargeInfo");
                    // HisObj fee = new HisObj();
                    // fee = XmlUtil.Deserialize(typeof(HisObj), xml) as HisObj;

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ApplyChargeInfo info = new ApplyChargeInfo();
                        info.ITEM_FEE_CODE = row[0].ToString();
                        info.ITEM_FEE_NAME = row[1].ToString();
                        info.FEE_COUNT = row[2].ToString();
                        info.ITEM_PRICE = row[3].ToString();
                        info.FEE_STATUS = row[4].ToString();
                        feeList.Add(info);
                    }

                
                }
                else
                {
                    source.Return.Code = "0";
                    source.Return.ErrorMsg = "没有相关数据！";
                }
                source.Return.Result.ExamApply = feeList;
            }
            catch (Exception ex)
            {
                source.Return.ErrorMsg = ex.Message;
                source.Return.Code = "0";
                HisLog.WriteLog(His.Models.Common.HisLogType.Endoscope, "his.business.CancelCheckIn 发生程序错误：" + ex.Message);
                return source;

            }
            return source;
        }

        public DataSource<object> CancelCheckIn(FeeStatusRequestInfo reqInfo)
        {
            DataSource<object> source = new DataSource<object>();

            source.Return.Result.ExamApply = null;
            string tabName = string.Empty;
            string msg = string.Empty;
            if (string.IsNullOrEmpty(reqInfo.APLY_FLOW_NUM) ||
                string.IsNullOrEmpty(reqInfo.PATIENT_TYPE))
            {
                source.Return.Code = "0";
                source.Return.ErrorMsg = "申请单号,患者来源(类型)不能为空！";
                //return source;
            }
            else
            {
                try
                {
                    if (reqInfo.PATIENT_TYPE == "0")
                        tabName = "fin_opb_feedetail";
                    else
                        tabName = "  fin_ipb_itemlist";

                    string whereSql = string.Empty;

                    if (string.IsNullOrEmpty(reqInfo.ORDER_ID))
                        whereSql += " and a.recipe_no='" + reqInfo.ORDER_ID + "'";

                    string sql = @"update {0} a
                            set  a.noback_num=a.qty
                            where   a.mo_order='{1}' ";
                    sql = string.Format(sql, tabName, reqInfo.APLY_FLOW_NUM);
                    if (!string.IsNullOrEmpty(whereSql))
                        sql += whereSql;
                    int result=DataBaseHelp.DataExecHelp.ExecuteSql(sql, ref msg);
                    // HisLog.WriteLog(His.Models.Common.HisLogType.Endoscope,
                    if (result>0)
                    {
                        source.Return.Code = "1";
                        // source.Return.ErrorMsg = "接收成功！";
                    }
                    else if(result==0)
                    {
                        source.Return.Code = "0";
                        source.Return.ErrorMsg = "没有相关数据！";
                    }
                    else
                    {
                        source.Return.Code = "0";
                        source.Return.ErrorMsg = msg;
                    }
                }
                catch (Exception ex)
                {
                    source.Return.Code = "0";
                    source.Return.ErrorMsg = ex.Message;
                    HisLog.WriteLog(His.Models.Common.HisLogType.Endoscope, "his.business.CancelCheckIn 发生程序错误：" + ex.Message);
                }
            }
            return source;
        }

    }
}
