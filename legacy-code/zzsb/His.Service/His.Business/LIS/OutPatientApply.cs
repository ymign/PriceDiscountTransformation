using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.LIS
{
    public class OutPatientApply
    {
        private System.Collections.ArrayList GetOutPatientApplyData(His.Models.LIS.OutPatientApply outPatientApply)
        {
            #region sql
            #region 作废
//            string sql = @" 
//select distinct to_char(l.mo_order) aply_detl_id, --PK可用申请项目的流水号
//       0 aply_src, --申请来源 0 - HIS系统；1 - LIS系统；2 - 体检系统；3 - 其他来源
//       /*case when l.cost_source='1' then (select t.recipe_no from met_ord_recipedetail t
//               where t.clinic_code=l.clinic_code
//                     and t.sequence_no=l.mo_order)
//         else l.invoice_no end*/ l.recipe_no aply_flow_num,---l.invoice_no aply_flow_num, --申请流水号 申请单号
//       sysdate aply_create_date, --申请创建日期
//       case l.emc_flag
//         when '2' then
//          '1'
//         else
//          '0'
//       end emcy_mrk, --急诊标记(急诊赋1)
//       l.reg_date as aply_date, --申请日期
//       l.doct_dept dept_key, --  申请科室编码
//       fun_get_dept_name(l.doct_dept) dept_name, --申请科室名称
//       l.doct_code doc_key, --申请医生工号
//       (select e.empl_name
//          from com_employee e
//         where e.empl_code = l.doct_code) doc_name, -- 申请医生名称
//       l.clinic_code ptnt_id, --    就诊患者ID
//        (select r.in_times from fin_opr_register r
//       where r.clinic_code=l.clinic_code
//       and r.trans_type='1') visit_id,--就诊次数
//        (select r.idenno from fin_opr_register r
//       where r.clinic_code=l.clinic_code
//       and r.trans_type='1') as id_card, --身份证
//       (select ltrim(r.card_no, '0') from fin_opr_register r
//       where r.clinic_code=l.clinic_code
//       and r.trans_type='1') ic_card, -- IC卡号
//       (select r.address from fin_opr_register r
//       where r.clinic_code=l.clinic_code
//       and r.trans_type='1') ctat_addr,--联系地址
//       (select r.rela_phone from fin_opr_register r
//       where r.clinic_code=l.clinic_code
//       and r.trans_type='1') phone_num,--联系电话
//       l.card_no ptnt_no, --    　  病历号
//       0 ptnt_no_type, --    病历号类型：0-门诊号；1-住院号；2-体检号；
//       (select r.name from fin_opr_register r
//       where r.clinic_code=l.clinic_code
//       and r.trans_type='1')  as ptnt_name, --    　  患者姓名
//       (select (case nvl(r.sex_code, '0')
//         when 'M' then
//          1
//         when 'F' then
//          2
//         when '0' then
//          0
//         else
//          3
//       end) from fin_opr_register r
//       where r.clinic_code=l.clinic_code
//       and r.trans_type='1') as ptnt_sex, --  性别：1-男；2-女；3-性别不明确
//       (select fun_get_age_new(r.birthday,sysdate) from fin_opr_register r
//       where r.clinic_code=l.clinic_code
//       and r.trans_type='1') ptnt_age, --    　  患者年龄
//       null ptnt_age_unit, --      年龄类型：0-岁；1-月；2-天；3-时
//       l.reg_date admisse_date, --      入院日期/就诊日期
//       (select r.birthday from fin_opr_register r
//       where r.clinic_code=l.clinic_code
//       and r.trans_type='1')  as ptnt_birth, --      出生日期
//       null ptnt_bed_no, --    　  床号
//       (select max(m.diag_name)
//          from met_cas_diagnose m
//         where m.main_flag = '1'
//           and m.inpatient_no = l.clinic_code) diag_info, --诊断信息
//       nvl(l.package_code,l.item_code) aply_itm_key, --        申请项目对照编码
//       decode(nvl(l.package_code,'哈哈'),'哈哈',l.item_name,l.package_name) aply_itm_name, --      　  申请项目名称
//       (select t.LAB_TYPE from met_ord_recipedetail t
//               where t.clinic_code=l.clinic_code
//                     and t.sequence_no=l.mo_order) /*l.LAB_TYPE */smpl_key, --        样本类型编码
//       (select t.LAB_TYPE from met_ord_recipedetail t
//               where t.clinic_code=l.clinic_code
//                     and t.sequence_no=l.mo_order) smpl_name, --样本类型名称
//       null body_part,--取材部位
//       (select t.remark from met_ord_recipedetail t
//               where t.clinic_code=l.clinic_code
//                     and t.sequence_no=l.mo_order) remark,--执行说明
//       decode(l.sample_id,
//              '1',
//              1,
//              0) exec_status --    执行状态0 - 未在LIS产生相应样本条码；1 - 已在LIS产生相应样本条码
//  from fin_opb_feedetail l
// where l.fee_date>sysdate-300
//   and l.sample_id<>'1'
//   and l.pay_flag = '1'
//   and l.class_code = 'UL'
//   and l.cancel_flag ='1'
//   and (l.recipe_no='{0}' or 'ALL'='{0}')
//   and (l.invoice_no='{1}' or 'ALL'='{1}')
//   AND (l.clinic_code='{2}' or 'ALL'='{2}')
//   AND (l.Card_No='{3}' or 'ALL'='{3}')
            //            ";
            #endregion


//            string sql = @"select distinct to_char(l.mo_order) aply_detl_id, --PK可用申请项目的流水号
//                           0 aply_src, --申请来源 0 - HIS系统；1 - LIS系统；2 - 体检系统；3 - 其他来源
//                           case when l.cost_source ='1'
//                               then  l.recipe_no else l.invoice_no end aply_flow_num, --申请流水号 申请单号
//                           sysdate aply_create_date, --申请创建日期
//                           case l.emc_flag
//                             when '2' then
//                              '1'
//                             else
//                              '0'
//                           end emcy_mrk, --急诊标记(急诊赋1)
//                           l.reg_date as aply_date, --申请日期
//                           l.doct_dept dept_key, --  申请科室编码
//                           fun_get_dept_name(l.doct_dept) dept_name, --申请科室名称
//                           l.doct_code doc_key, --申请医生工号
//                           
//                           
//                           fun_get_employee_name(l.doct_code) doc_name, -- 申请医生名称
//                           l.clinic_code ptnt_id, --    就诊患者ID
//                           
//                           
//                           r.in_times visit_id,--就诊次数
//                           r.idenno as id_card, --身份证
//                           ltrim(r.card_no, '0') ic_card, -- IC卡号
//                           r.address ctat_addr,--联系地址
//                           r.rela_phone phone_num,--联系电话
//                           l.card_no ptnt_no, --    　  病历号
//                           0 ptnt_no_type, --    病历号类型：0-门诊号；1-住院号；2-体检号；
//                           r.name  as ptnt_name, --    　  患者姓名
//                           (case nvl(r.sex_code, '0')
//                             when 'M' then
//                              1
//                             when 'F' then
//                              2
//                             when '0' then
//                              0
//                             else
//                              3
//                           end)  as ptnt_sex, --  性别：1-男；2-女；3-性别不明确
//                           fun_get_age_new(r.birthday,sysdate) ptnt_age, --    　  患者年龄
//                           null ptnt_age_unit, --      年龄类型：0-岁；1-月；2-天；3-时
//                           l.reg_date admisse_date, --      入院日期/就诊日期
//                           r.birthday  as ptnt_birth, --      出生日期
//                           null ptnt_bed_no, --    　  床号
//                           (select max(m.diag_name)
//                              from met_cas_diagnose m
//                             where m.main_flag = '1'
//                               and m.inpatient_no = l.clinic_code
//                               and rownum=1) diag_info, --诊断信息
//                           nvl(l.package_code,l.item_code) aply_itm_key, --        申请项目对照编码
//                           decode(nvl(l.package_code,'哈哈'),'哈哈',l.item_name,l.package_name) aply_itm_name, --  　  申请项目名称
//                           (select cd.code from met_ord_recipedetail t,com_dictionary cd
//                                   where t.clinic_code=l.clinic_code
//                                         and t.sequence_no=l.mo_order
//                                         and t.LAB_TYPE=cd.name
//                                         and cd.type='LABSAMPLE'
//                                         and rownum=1) /*l.LAB_TYPE */smpl_key, --        样本类型编码
//                           (select t.LAB_TYPE from met_ord_recipedetail t
//                                   where t.clinic_code=l.clinic_code
//                                         and t.sequence_no=l.mo_order
//                                         and rownum=1) smpl_name, --样本类型名称
//                           null body_part,--取材部位
//                           (select t.remark from met_ord_recipedetail t
//                                   where t.clinic_code=l.clinic_code
//                                         and t.sequence_no=l.mo_order
//                                         and rownum=1) remark,--执行说明
//                           decode(l.sample_id,
//                                  '1',
//                                  1,
//                                  0) exec_status, --    执行状态0 - 未在LIS产生相应样本条码；1 - 已在LIS产生相应样本条码
//(select empi.empi_no from  EMPI_PAITINETINFO empi
//where empi.card_no=r.card_no and rownum=1) as empi
//                      from fin_opb_feedetail l, fin_opr_register r
//                     where l.fee_date>sysdate-300
//                       and l.sample_id is null
//                       and l.pay_flag = '1'
//                       and l.class_code = 'UL'
//                       and l.cancel_flag ='1'
//                       and l.clinic_code = r.clinic_code
//                       and (l.recipe_no='{0}' or 'ALL'='{0}')
//                       and (l.invoice_no='{1}' or 'ALL'='{1}')
//                       AND (r.clinic_code='{2}' or 'ALL'='{2}')
//                       AND (r.Card_No='{3}' or 'ALL'='{3}')";
            #endregion

            #region sql
            string sql = @"select distinct to_char(l.mo_order) aply_detl_id, --PK可用申请项目的流水号
       0 aply_src, --申请来源 0 - HIS系统；1 - LIS系统；2 - 体检系统；3 - 其他来源
       case when l.cost_source ='1'
           then  l.recipe_no else l.invoice_no end aply_flow_num,---l.invoice_no aply_flow_num, --申请流水号 申请单号
       sysdate aply_create_date, --申请创建日期
       case l.emc_flag
         when '2' then
          '1'
         else
          '0'
       end emcy_mrk, --急诊标记(急诊赋1)
       l.reg_date as aply_date, --申请日期
       l.doct_dept dept_key, --  申请科室编码
       fun_get_dept_name(l.doct_dept) dept_name, --申请科室名称
       l.doct_code doc_key, --申请医生工号
       fun_get_employee_name(l.doct_code) doc_name, -- 申请医生名称
       l.clinic_code ptnt_id, --    就诊患者ID
       r.in_times visit_id,--就诊次数
       r.idenno as id_card, --身份证
       ltrim(r.card_no, '0') ic_card, -- IC卡号
       r.address ctat_addr,--联系地址
       r.rela_phone phone_num,--联系电话
       r.card_no ptnt_no, --    　  病历号
       0 ptnt_no_type, --    病历号类型：0-门诊号；1-住院号；2-体检号；
       r.name  as ptnt_name, --    　  患者姓名
       (case nvl(r.sex_code, '0')
         when 'M' then
          1
         when 'F' then
          2
         when '0' then
          0
         else
          3
       end)  as ptnt_sex, --  性别：1-男；2-女；3-性别不明确
       fun_get_age_new(r.birthday,sysdate) ptnt_age, --    　  患者年龄
       null ptnt_age_unit, --      年龄类型：0-岁；1-月；2-天；3-时
       l.reg_date admisse_date, --      入院日期/就诊日期
       r.birthday  as ptnt_birth, --      出生日期
       null ptnt_bed_no, --    　  床号
       (select max(m.diag_name)
          from met_cas_diagnose m
         where m.main_flag = '1'
           and m.inpatient_no = l.clinic_code
           and rownum=1) diag_info, --诊断信息
       nvl(l.package_code,l.item_code) aply_itm_key, --        申请项目对照编码
       decode(nvl(l.package_code,'哈哈'),'哈哈',l.item_name,l.package_name) aply_itm_name, --      　  申请项目名称
       (select cd.code from met_ord_recipedetail t,com_dictionary cd
               where t.clinic_code=l.clinic_code
                     and t.sequence_no=l.mo_order
                     and t.LAB_TYPE=cd.name
                     and cd.type='LABSAMPLE' and rownum=1) /*l.LAB_TYPE */smpl_key, --        样本类型编码
       (select t.LAB_TYPE from met_ord_recipedetail t
               where t.clinic_code=l.clinic_code
                     and t.sequence_no=l.mo_order and rownum=1) smpl_name, --样本类型名称
       null body_part,--取材部位
       (select t.remark from met_ord_recipedetail t
               where t.clinic_code=l.clinic_code
                     and t.sequence_no=l.mo_order and rownum=1) remark,--执行说明
       decode(l.sample_id,
              '1',
              1,
              0) exec_status --    执行状态0 - 未在LIS产生相应样本条码；1 - 已在LIS产生相应样本条码
  from fin_opb_feedetail l, fin_opr_register r
                     where l.fee_date>sysdate-300
                       and l.sample_id is null
                       and l.pay_flag = '1'
                       and l.class_code = 'UL'
                       and l.cancel_flag ='1'
                       and l.clinic_code = r.clinic_code
                       and (l.recipe_no='{0}' or 'ALL'='{0}')
                       and (l.invoice_no='{1}' or 'ALL'='{1}')
                       AND (r.clinic_code='{2}' or 'ALL'='{2}')
                       AND (r.Card_No='{3}' or 'ALL'='{3}')";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, outPatientApply.APLY_FLOW_NUM, outPatientApply.BILL_NO, outPatientApply.PTNT_ID, outPatientApply.IC_CARD);

                System.Data.DataTable dt = new System.Data.DataTable();
                //申请单
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    outPatientApply = new His.Models.LIS.OutPatientApply();
                    outPatientApply.APLY_DETL_ID = dt.Rows[i][0].ToString();
                    outPatientApply.APLY_SRC = dt.Rows[i][1].ToString();
                    outPatientApply.APLY_FLOW_NUM = dt.Rows[i][2].ToString();
                    outPatientApply.APLY_CREATE_DATE = dt.Rows[i][3].ToString();
                    outPatientApply.EMCY_MRK = dt.Rows[i][4].ToString();
                    outPatientApply.APLY_DATE = dt.Rows[i][5].ToString();
                    outPatientApply.DEPT_KEY = dt.Rows[i][6].ToString();
                    outPatientApply.DEPT_NAME = dt.Rows[i][7].ToString();
                    outPatientApply.DOC_KEY = dt.Rows[i][8].ToString();
                    outPatientApply.DOC_NAME = dt.Rows[i][9].ToString();
                    outPatientApply.PTNT_ID = dt.Rows[i][10].ToString();
                    outPatientApply.VISIT_ID = dt.Rows[i][11].ToString();
                    outPatientApply.ID_CARD = dt.Rows[i][12].ToString();
                    outPatientApply.IC_CARD = dt.Rows[i][13].ToString();
                    outPatientApply.CTAT_ADDR = dt.Rows[i][14].ToString();
                    outPatientApply.PHONE_NUM = dt.Rows[i][15].ToString();
                    outPatientApply.PTNT_NO = dt.Rows[i][16].ToString();
                    outPatientApply.PTNT_NO_TYPE = dt.Rows[i][17].ToString();
                    outPatientApply.PTNT_NAME = dt.Rows[i][18].ToString();
                    outPatientApply.PTNT_SEX = dt.Rows[i][19].ToString();
                    outPatientApply.PTNT_AGE = dt.Rows[i][20].ToString();
                    outPatientApply.PTNT_AGE_UNIT = dt.Rows[i][21].ToString();
                    outPatientApply.ADMISSE_DATE = dt.Rows[i][22].ToString();
                    outPatientApply.PTNT_BIRTH = dt.Rows[i][23].ToString();
                    outPatientApply.PTNT_BED_NO = dt.Rows[i][24].ToString();
                    outPatientApply.DIAG_INFO = dt.Rows[i][25].ToString();
                    outPatientApply.APLY_ITM_KEY = dt.Rows[i][26].ToString();
                    outPatientApply.APLY_ITM_NAME = dt.Rows[i][27].ToString();
                    outPatientApply.SMPL_KEY = dt.Rows[i][28].ToString();
                    outPatientApply.SMPL_NAME = dt.Rows[i][29].ToString();
                    outPatientApply.BODY_PART = dt.Rows[i][30].ToString();
                    outPatientApply.REMARK = dt.Rows[i][31].ToString();
                    outPatientApply.EXEC_STATUS = dt.Rows[i][32].ToString();
                    //outPatientApply.EMPI = dt.Rows[i][33].ToString();
                    al.Add(outPatientApply);
                }
                return al;
                #endregion
            }
            catch
            {
                return null;
            }
        }

        private string GetOutPatientApplyXML(System.Collections.ArrayList al)
        {

            #region
            if (al.Count == 0)
            {
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
                System.Xml.XmlElement root = xml.CreateElement("DataSource");
                xml.AppendChild(root);

                System.Xml.XmlElement root1 = xml.CreateElement("return");
                root.AppendChild(root1);

                System.Xml.XmlElement Code = xml.CreateElement("Code");
                Code.InnerText = "0";
                root1.AppendChild(Code);

                System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
                ErrorMsg.InnerText = "查找信息失败，请核对后查询";
                root1.AppendChild(ErrorMsg);

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);

                return xml.InnerXml.ToString();
            }
            else
            {
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
                System.Xml.XmlElement root = xml.CreateElement("DataSource");
                xml.AppendChild(root);

                System.Xml.XmlElement root1 = xml.CreateElement("return");
                root.AppendChild(root1);

                System.Xml.XmlElement Code = xml.CreateElement("Code");
                Code.InnerText = "1";
                root1.AppendChild(Code);

                System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
                ErrorMsg.InnerText = "";
                root1.AppendChild(ErrorMsg);

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);

                System.Xml.XmlElement LISApply = xml.CreateElement("LISApply");
                root1.AppendChild(LISApply);

                System.Xml.XmlElement PATIENTINFO = xml.CreateElement("PATIENTINFO");
                LISApply.AppendChild(PATIENTINFO);

                His.Models.LIS.OutPatientApply p = al[0] as His.Models.LIS.OutPatientApply;

                System.Xml.XmlElement PTNT_ID = xml.CreateElement("PTNT_ID");
                PTNT_ID.InnerText = p.PTNT_ID;
                PATIENTINFO.AppendChild(PTNT_ID);

                System.Xml.XmlElement VISIT_ID = xml.CreateElement("VISIT_ID");
                VISIT_ID.InnerText = p.VISIT_ID;
                PATIENTINFO.AppendChild(VISIT_ID);

                System.Xml.XmlElement EMPI = xml.CreateElement("EMPI");
                EMPI.InnerText = p.EMPI;
                PATIENTINFO.AppendChild(EMPI);

                System.Xml.XmlElement ID_CARD = xml.CreateElement("ID_CARD");
                ID_CARD.InnerText = p.ID_CARD;
                PATIENTINFO.AppendChild(ID_CARD);

                System.Xml.XmlElement IC_CARD = xml.CreateElement("IC_CARD");
                IC_CARD.InnerText = p.IC_CARD;
                PATIENTINFO.AppendChild(IC_CARD);

                System.Xml.XmlElement CTAT_ADDR = xml.CreateElement("CTAT_ADDR");
                CTAT_ADDR.InnerText = p.CTAT_ADDR;
                PATIENTINFO.AppendChild(CTAT_ADDR);

                System.Xml.XmlElement PHONE_NUM = xml.CreateElement("PHONE_NUM");
                PHONE_NUM.InnerText = p.PHONE_NUM;
                PATIENTINFO.AppendChild(PHONE_NUM);

                System.Xml.XmlElement PTNT_NO = xml.CreateElement("PTNT_NO");
                PTNT_NO.InnerText = p.PTNT_NO;

                System.Xml.XmlElement PTNT_NO_TYPE = xml.CreateElement("PTNT_NO_TYPE");
                PTNT_NO_TYPE.InnerText = p.PTNT_NO_TYPE;
                PATIENTINFO.AppendChild(PTNT_NO_TYPE);

                System.Xml.XmlElement PTNT_NAME = xml.CreateElement("PTNT_NAME");
                PTNT_NAME.InnerText = p.PTNT_NAME;
                PATIENTINFO.AppendChild(PTNT_NAME);

                System.Xml.XmlElement PTNT_SEX = xml.CreateElement("PTNT_SEX");
                PTNT_SEX.InnerText = p.PTNT_SEX;
                PATIENTINFO.AppendChild(PTNT_SEX);

                System.Xml.XmlElement PTNT_AGE = xml.CreateElement("PTNT_AGE");
                PTNT_AGE.InnerText = p.PTNT_AGE;
                PATIENTINFO.AppendChild(PTNT_AGE);

                System.Xml.XmlElement PTNT_AGE_UNIT = xml.CreateElement("PTNT_AGE_UNIT");
                PTNT_AGE_UNIT.InnerText = p.PTNT_AGE_UNIT;
                PATIENTINFO.AppendChild(PTNT_AGE_UNIT);

                System.Xml.XmlElement ADMISSE_DATE = xml.CreateElement("ADMISSE_DATE");
                ADMISSE_DATE.InnerText = p.ADMISSE_DATE;
                PATIENTINFO.AppendChild(ADMISSE_DATE);

                System.Xml.XmlElement PTNT_BIRTH = xml.CreateElement("PTNT_BIRTH");
                PTNT_BIRTH.InnerText = p.PTNT_BIRTH;
                PATIENTINFO.AppendChild(PTNT_BIRTH);

                System.Xml.XmlElement PTNT_BED_NO = xml.CreateElement("PTNT_BED_NO");
                PTNT_BED_NO.InnerText = p.PTNT_BED_NO;
                PATIENTINFO.AppendChild(PTNT_BED_NO);

                System.Xml.XmlElement DIAG_INFO = xml.CreateElement("DIAG_INFO");
                DIAG_INFO.InnerText = p.DIAG_INFO;
                PATIENTINFO.AppendChild(DIAG_INFO);

                foreach (His.Models.LIS.OutPatientApply opa in al)
                {
                    System.Xml.XmlElement APPLYINFO = xml.CreateElement("APPLYINFO");
                    LISApply.AppendChild(APPLYINFO);

                    System.Xml.XmlElement APLY_DETL_ID = xml.CreateElement("APLY_DETL_ID");
                    APLY_DETL_ID.InnerText = opa.APLY_DETL_ID;
                    APPLYINFO.AppendChild(APLY_DETL_ID);

                    System.Xml.XmlElement APLY_SRC = xml.CreateElement("APLY_SRC");
                    APLY_SRC.InnerText = opa.APLY_SRC;
                    APPLYINFO.AppendChild(APLY_SRC);

                    System.Xml.XmlElement APLY_FLOW_NUM = xml.CreateElement("APLY_FLOW_NUM");
                    APLY_FLOW_NUM.InnerText = opa.APLY_FLOW_NUM;
                    APPLYINFO.AppendChild(APLY_FLOW_NUM);

                    System.Xml.XmlElement APLY_CREATE_DATE = xml.CreateElement("APLY_CREATE_DATE");
                    APLY_CREATE_DATE.InnerText = opa.APLY_CREATE_DATE;
                    APPLYINFO.AppendChild(APLY_CREATE_DATE);

                    System.Xml.XmlElement EMCY_MRK = xml.CreateElement("EMCY_MRK");
                    EMCY_MRK.InnerText = opa.EMCY_MRK;
                    APPLYINFO.AppendChild(EMCY_MRK);

                    System.Xml.XmlElement APLY_DATE = xml.CreateElement("APLY_DATE");
                    APLY_DATE.InnerText = opa.APLY_DATE;
                    APPLYINFO.AppendChild(APLY_DATE);

                    System.Xml.XmlElement DEPT_KEY = xml.CreateElement("DEPT_KEY");
                    DEPT_KEY.InnerText = opa.DEPT_KEY;
                    APPLYINFO.AppendChild(DEPT_KEY);

                    System.Xml.XmlElement DEPT_NAME = xml.CreateElement("DEPT_NAME");
                    DEPT_NAME.InnerText = opa.DEPT_NAME;
                    APPLYINFO.AppendChild(DEPT_NAME);

                    System.Xml.XmlElement DOC_KEY = xml.CreateElement("DOC_KEY");
                    DOC_KEY.InnerText = opa.DOC_KEY;
                    APPLYINFO.AppendChild(DOC_KEY);

                    System.Xml.XmlElement DOC_NAME = xml.CreateElement("DOC_NAME");
                    DOC_NAME.InnerText = opa.DOC_NAME;
                    APPLYINFO.AppendChild(DOC_NAME);

                    System.Xml.XmlElement APLY_ITM_KEY = xml.CreateElement("APLY_ITM_KEY");
                    APLY_ITM_KEY.InnerText = opa.APLY_ITM_KEY;
                    APPLYINFO.AppendChild(APLY_ITM_KEY);

                    System.Xml.XmlElement APLY_ITM_NAME = xml.CreateElement("APLY_ITM_NAME");
                    APLY_ITM_NAME.InnerText = opa.APLY_ITM_NAME;
                    APPLYINFO.AppendChild(APLY_ITM_NAME);

                    System.Xml.XmlElement SMPL_KEY = xml.CreateElement("SMPL_KEY");
                    SMPL_KEY.InnerText = opa.SMPL_KEY;
                    APPLYINFO.AppendChild(SMPL_KEY);

                    System.Xml.XmlElement SMPL_NAME = xml.CreateElement("SMPL_NAME");
                    SMPL_NAME.InnerText = opa.SMPL_NAME;
                    APPLYINFO.AppendChild(SMPL_NAME);

                    System.Xml.XmlElement BODY_PART = xml.CreateElement("BODY_PART");
                    BODY_PART.InnerText = opa.BODY_PART;
                    APPLYINFO.AppendChild(BODY_PART);

                    System.Xml.XmlElement REMARK = xml.CreateElement("REMARK");
                    REMARK.InnerText = opa.REMARK;
                    APPLYINFO.AppendChild(REMARK);

                    System.Xml.XmlElement EXEC_STATUS = xml.CreateElement("EXEC_STATUS");
                    EXEC_STATUS.InnerText = opa.EXEC_STATUS;
                    APPLYINFO.AppendChild(EXEC_STATUS);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private His.Models.LIS.OutPatientApply GetOutPatientModel(string xml)
        {
            His.Models.LIS.OutPatientApply opa = new His.Models.LIS.OutPatientApply();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList APLY_FLOW_NUM = doc.GetElementsByTagName("APLY_FLOW_NUM");
            System.Xml.XmlNode APLY_FLOW_NUM1 = APLY_FLOW_NUM[0];
            if (!string.IsNullOrEmpty(APLY_FLOW_NUM1.InnerText))
            {
                opa.APLY_FLOW_NUM = APLY_FLOW_NUM1.InnerText;
            }
            else
            {
                opa.APLY_FLOW_NUM = "ALL";
            }

            System.Xml.XmlNodeList BILL_NO_list = doc.GetElementsByTagName("BILL_NO");
            System.Xml.XmlNode BILL_NO = BILL_NO_list[0];
            if (!string.IsNullOrEmpty(BILL_NO.InnerText))
            {
                opa.BILL_NO = BILL_NO.InnerText;
            }
            else
            {
                opa.BILL_NO = "ALL";
            }

            System.Xml.XmlNodeList PTNT_ID = doc.GetElementsByTagName("PTNT_ID");
            System.Xml.XmlNode PTNT_ID1 = PTNT_ID[0];
            if (!string.IsNullOrEmpty(PTNT_ID1.InnerText))
            {
                opa.PTNT_ID = PTNT_ID1.InnerText;
            }
            else
            {
                opa.PTNT_ID = "ALL";
            }

            System.Xml.XmlNodeList IC_CARD_list = doc.GetElementsByTagName("IC_CARD");
            System.Xml.XmlNode IC_CARD = IC_CARD_list[0];
            if (!string.IsNullOrEmpty(IC_CARD.InnerText))
            {
                opa.IC_CARD = IC_CARD.InnerText;
            }
            else
            {
                opa.IC_CARD = "ALL";
            }

            //System.Xml.XmlNodeList EMPI_list = doc.GetElementsByTagName("EMPI");
            //System.Xml.XmlNode EMPI = EMPI_list[0];
            //if (!string.IsNullOrEmpty(EMPI.InnerText))
            //{
            //    opa.EMPI = EMPI.InnerText;
            //}
            //else
            //{
            //    opa.EMPI = "ALL";
            //}

            System.Xml.XmlNodeList LAB_TYPE_list = doc.GetElementsByTagName("LAB_TYPE");
            System.Xml.XmlNode LAB_TYPE = LAB_TYPE_list[0];
            if (!string.IsNullOrEmpty(LAB_TYPE.InnerText))
            {
                opa.LAB_TYPE = LAB_TYPE.InnerText;
            }
            else
            {
                opa.LAB_TYPE = "ALL";
            }

            return opa;
        }

        public string GetOutPatientApply(string xml)
        {
            string returnStr = "";
            His.Models.LIS.OutPatientApply opa = new His.Models.LIS.OutPatientApply();
            opa=this.GetOutPatientModel(xml);
            System.Collections.ArrayList al = this.GetOutPatientApplyData(opa);
            returnStr = this.GetOutPatientApplyXML(al);
            return returnStr;
        }
    }
}
