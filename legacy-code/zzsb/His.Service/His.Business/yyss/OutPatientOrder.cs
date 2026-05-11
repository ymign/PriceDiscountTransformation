using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.YYSS
{
    public class OutPatientOrder
    {
        string Erro = string.Empty;//错误信息;
        
        /// <summary>
        /// 由于一开始以为是营养膳食的门诊医嘱接口 所以命名成这样
        /// 后来发现,不是啊!!!!
        /// 所以把原有的住院信息加会进去.
        /// 真是仍人绝望!!!
        /// </summary>


        #region  门诊患者信息sql

        public static string PatentInfo = @"
                                            SELECT A.Clinic_Code PATIENT_ID,
                                                   A.Card_No INP_NO,
                                                   A.In_Times VISIT_ID,
                                                   A.DEPT_CODE DEPT_CODE,
                                                   A.DEPT_NAME DEPT_NAME,
                                                   '' WARD_CODE,
                                                   '' WARD_NAME,
                                                   '' BED_NO,
                                                   A.NAME NAME,
                                                   fun_get_sex(A.SEX_CODE) SEX,
                                                   A.BIRTHDAY BIRTHDAY,
                                                   fun_get_age_new(A.BIRTHDAY, SYSDATE) AGE,
                                                   A.HEIGHT HEIGHT,
                                                   A.WEIGHT WEIGHT,
                                                   A.rela_phone MOBILE_PHONE,
                                                   A.address ADDRESS,
                                                   A.IDENNO ID_NO,
                                                   '0' AS CHARGE_TYPE,
                                                   '' BALANCE,
                                                   A.Reg_Date IN_HOS_DATE_TIME,
                                                   (select p.diag_name
                                                      from met_cas_diagnose p
                                                     where p.inpatient_no = a.clinic_code
                                                       and p.happen_no = 1) DIAGNOSIS,
                                                   '否' SETTLED_INDICATOR,
                                                   '' OUT_HOS_DATE,
                                                   '' AS OUT_STATUS
                                              FROM fin_opr_register A
                                              where (a.card_no='{0}' or 'ALL'='{0}')
                                              and a.reg_date>=to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                                              and a.reg_date<=to_date('{2}','yyyy-mm-dd hh24:mi:ss')
                                              and (select nvl(count(*),0) from met_ord_recipedetail p where p.clinic_code=A.clinic_code and p.class_code in ('UC'))>0";

        #endregion

        #region 门诊患者医嘱信息

        public string OutPatientOrderSql = @"SELECT 
 O.CLINIC_CODE PATIENT_ID,
 I.IN_TIMES VISIT_ID,
 O.SEQUENCE_NO ORDER_NO,
 O.COMB_NO ORDER_SUB_NO,
 TO_CHAR(O.OPER_DATE,'YYYY-MM-DD HH24:MI:SS') START_DATE_TIME,
 '' STOP_DATE_TIME,
 '0' REPEAT_INDICATOR,
 'MF' AS ORDER_CLASS,
 '膳食' AS ORDER_CLASS_NAME,
 O.ITEM_CODE ORDER_CODE,
 O.ITEM_NAME ORDER_TEXT,
 DECODE(O.STATUS,'0','开立'，'1','审核'，'2','执行'，'3','作废'，'4','重整'，'5','需要上级医生审核'，'6','暂存'，'7','预停止') ORDER_STATUS,
 O.ONCE_DOSE DOSAGE,
 O.ONCE_UNIT DOSAGE_UNITS,
 '-' AS DURATION,
 '-' AS DURATION_UNITS,
 DECODE(O.FREQUENCY_CODE,'AA','2','BID','2','BIW','2','FID','5','Q12H','1','Q2H','1','Q2W','1','Q3D','1','Q3W','1','Q4H','1','Q4W','1','Q5D','1','Q6H','1','Q8H','1','QD','1','QH','1','QHH','1','QID','4','QMD','1','QN','1','QOD','1','QW','1','TID','3','TIW','3','BIW2','2','-') FREQ_COUNTER,
 DECODE(O.FREQUENCY_CODE,'AA','12','BID','12','BIW','3.5','FID','4.5','Q12H','12','Q2H','2','Q2W','2','Q3D','3','Q3W','3','Q4H','4','Q4W','4','Q5D','5','Q6H','6','Q8H','8','QD','1','QH','1','QHH','0.5','QID','6','QMD','1','QN','1','QOD','2','QW','1','TID','8','TIW','2','BIW2','3','-') FREQ_INTERVAL,
 DECODE(O.FREQUENCY_CODE,'AA','时','BID','时','BIW','天','FID','时','Q12H','时','Q2H','时','Q2W','周','Q3D','天','Q3W','周','Q4H','时','Q4W','周','Q5D','天','Q6H','时','Q8H','时','QD','天','QH','时','QHH','时','QID','时','QMD','月','QN','天','QOD','天','QW','周','TID','时','TIW','天','BIW2','天','-') FREQ_INTERVAL_UNIT,
 O.REMARK FREQ_DETAIL,
 O.CHARGE_DATE PERFORM_SCHEDULE,
 '-' AS PERFORM_RESULT,
 O.EXEC_DPCD ORDERING_DEPT,
 O.DOCT_CODE DOCTOR,
 '' STOP_DOCTOR,
 '' NURSE,
 '' STOP_NURSE,
 O.OPER_DATE ENTER_DATE_TIME,
 '' STOP_ORDER_DATE_TIME
 FROM MET_ORD_RECIPEDETAIL O,FIN_OPR_REGISTER I
 WHERE  I.CLINIC_CODE=O.CLINIC_CODE
 And O.CLASS_CODE in ('UC') 
 AND O.CLINIC_CODE='{0}'";

        #endregion

        #region 住院患者信息
        #region sql
        public static string PatentInfo2 = @"
                                    SELECT A.INPATIENT_NO PATIENT_ID,
                                           A.PATIENT_NO INP_NO,
                                           A.IN_TIMES VISIT_ID,
                                           A.DEPT_CODE DEPT_CODE,
                                           A.DEPT_NAME DEPT_NAME,
                                           A.NURSE_CELL_CODE WARD_CODE,
                                           A.NURSE_CELL_NAME WARD_NAME,
                                           SUBSTR(A.BED_NO, 5) BED_NO,
                                           A.NAME NAME,
                                           fun_get_sex(A.SEX_CODE) SEX,
                                           A.BIRTHDAY BIRTHDAY,
                                           fun_get_age_new(A.BIRTHDAY, SYSDATE) AGE,
                                           A.HEIGHT HEIGHT,
                                           A.WEIGHT WEIGHT,
                                           A.HOME_TEL MOBILE_PHONE,
                                           A.HOME ADDRESS,
                                           A.IDENNO ID_NO,
                                           '0' AS CHARGE_TYPE,
                                           A.PREPAY_COST BALANCE,
                                           A.IN_DATE IN_HOS_DATE_TIME,
                                           (select item.value vvalue
                                              from emr.vhis_inpatientinfo info, rcd_record_item item
                                             where info.id = item.inpatient_id
                                               and info.INPATIENT_NO = a.inpatient_no
                                               and item.element_id = '357'
                                               and rownum = 1) DIAGNOSIS,
                                           DECODE(A.IN_STATE, 'I', '否','是') SETTLED_INDICATOR,
                                           A.OUT_DATE OUT_HOS_DATE,
                                           '' AS OUT_STATUS
                                      FROM FIN_IPR_INMAININFO A
                                    ";

        public static string where2 = @"
                    WHERE A.IN_STATE not in  ('I','N')
                    AND (A.DEPT_CODE='{0}' OR 'ALL'='{0}')
                    AND (A.PATIENT_NO='{1}' OR 'ALL'='{1}')
                    ";

        public static string where1 = @"
                    WHERE A.IN_STATE='I'
                    AND (A.DEPT_CODE='{0}' OR 'ALL'='{0}')
                    AND (A.PATIENT_NO='{1}' OR 'ALL'='{1}')
                    ";
        public static string wheretime = @"
                    AND A.IN_DATE BETWEEN  TO_DATE('{2}','yyyy-mm-dd hh24:mi:ss') AND TO_DATE('{3}','yyyy-mm-dd hh24:mi:ss')
                    ";
        public static string orderby = @"
                    ORDER BY A.IN_DATE DESC
                    ";

        #endregion
        #endregion

        #region 住院医嘱信息sql

        public string InPatientOrderSql = @"
                                    SELECT 
                                    O.INPATIENT_NO PATIENT_ID,
                                    I.IN_TIMES VISIT_ID,
                                    O.MO_ORDER ORDER_NO,
                                    O.SUBCOMBNO ORDER_SUB_NO,
                                    TO_CHAR(O.DATE_BGN,'yyyy-mm-dd hh24:mi:ss') START_DATE_TIME,
                                    TO_CHAR(O.DATE_END,'yyyy-mm-dd hh24:mi:ss') STOP_DATE_TIME,
                                    DECODE(O.TYPE_CODE,'CZ','1','LZ','0','0') REPEAT_INDICATOR,
                                    O.CLASS_CODE AS ORDER_CLASS,
                                    o.class_name AS ORDER_CLASS_NAME,
                                    O.ITEM_CODE ORDER_CODE,
                                    O.ITEM_NAME ORDER_TEXT,
                                    DECODE(O.MO_STAT,'0','开立'，'1','审核'，'2','执行'，'3','作废'，'4','重整'，'5','需要上级医生审核'，'6','暂存'，'7','预停止') ORDER_STATUS,
                                    O.DOSE_ONCE DOSAGE,
                                    O.DOSE_UNIT DOSAGE_UNITS,
                                    '-' AS DURATION,
                                    '-' AS DURATION_UNITS,
                                    DECODE(O.FREQUENCY_CODE,'AA','2','BID','2','BIW','2','FID','5','Q12H','1','Q2H','1','Q2W','1','Q3D','1','Q3W','1','Q4H','1','Q4W','1','Q5D','1','Q6H','1','Q8H','1','QD','1','QH','1','QHH','1','QID','4','QMD','1','QN','1','QOD','1','QW','1','TID','3','TIW','3','BIW2','2','-') FREQ_COUNTER,
                                    DECODE(O.FREQUENCY_CODE,'AA','12','BID','12','BIW','3.5','FID','4.5','Q12H','12','Q2H','2','Q2W','2','Q3D','3','Q3W','3','Q4H','4','Q4W','4','Q5D','5','Q6H','6','Q8H','8','QD','1','QH','1','QHH','0.5','QID','6','QMD','1','QN','1','QOD','2','QW','1','TID','8','TIW','2','BIW2','3','-') FREQ_INTERVAL,
                                    DECODE(O.FREQUENCY_CODE,'AA','时','BID','时','BIW','天','FID','时','Q12H','时','Q2H','时','Q2W','周','Q3D','天','Q3W','周','Q4H','时','Q4W','周','Q5D','天','Q6H','时','Q8H','时','QD','天','QH','时','QHH','时','QID','时','QMD','月','QN','天','QOD','天','QW','周','TID','时','TIW','天','BIW2','天','-') FREQ_INTERVAL_UNIT,
                                    O.MARK1 FREQ_DETAIL,
                                    O.EXECUTE_DATE PERFORM_SCHEDULE,
                                    '-' AS PERFORM_RESULT,
                                    O.DEPT_CODE ORDERING_DEPT,
                                    O.REC_USERNM DOCTOR,
                                    O.DC_DOCNM STOP_DOCTOR,
                                    fun_get_employee_name(O.CONFIRM_USERCD) NURSE,
                                    fun_get_employee_name(O.DC_CONFIRM_OPER) STOP_NURSE,
                                    O.MO_DATE ENTER_DATE_TIME,
                                    TO_CHAR(O.DC_DATE,'yyyy-mm-dd hh24:mi:ss') STOP_ORDER_DATE_TIME
                                    FROM MET_IPM_ORDER O,FIN_IPR_INMAININFO I
                                    WHERE O.CLASS_CODE in ('UC') 
                                    AND I.INPATIENT_NO=O.INPATIENT_NO
                                    AND O.INPATIENT_NO='{0}'
                                    ";

        #endregion

        #region 门诊患者信息
        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="al"></param>
        /// <returns></returns>
        private string GetOutPatientYYSSXML(System.Collections.ArrayList al)
        {

            if (al == null || al.Count == 0)
            {
                #region
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

                #endregion
                return xml.InnerXml.ToString();
            }
            else
            {
                #region
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

                System.Xml.XmlElement PATIENTS = xml.CreateElement("PATIENTS");
                root1.AppendChild(PATIENTS);

                foreach (His.Models.YYSS.InPatientInfo obj in al)
                {
                    System.Xml.XmlElement PATIENTINFO = xml.CreateElement("PATIENTINFO");
                    PATIENTS.AppendChild(PATIENTINFO);

                    System.Xml.XmlElement PATIENT_ID = xml.CreateElement("PATIENT_ID");
                    PATIENT_ID.InnerText = obj.PATIENT_ID;
                    PATIENTINFO.AppendChild(PATIENT_ID);

                    System.Xml.XmlElement INP_NO = xml.CreateElement("INP_NO");
                    INP_NO.InnerText = obj.INP_NO;
                    PATIENTINFO.AppendChild(INP_NO);

                    System.Xml.XmlElement VISIT_ID = xml.CreateElement("VISIT_ID");
                    VISIT_ID.InnerText = obj.VISIT_ID;
                    PATIENTINFO.AppendChild(VISIT_ID);

                    System.Xml.XmlElement DEPT_CODE = xml.CreateElement("DEPT_CODE");
                    DEPT_CODE.InnerText = obj.DEPT_CODE;
                    PATIENTINFO.AppendChild(DEPT_CODE);

                    System.Xml.XmlElement DEPT_NAME = xml.CreateElement("DEPT_NAME");
                    DEPT_NAME.InnerText = obj.DEPT_NAME;
                    PATIENTINFO.AppendChild(DEPT_NAME);

                    System.Xml.XmlElement WARD_CODE = xml.CreateElement("WARD_CODE");
                    WARD_CODE.InnerText = obj.WARD_CODE;
                    PATIENTINFO.AppendChild(WARD_CODE);

                    System.Xml.XmlElement WARD_NAME = xml.CreateElement("WARD_NAME");
                    WARD_NAME.InnerText = obj.WARD_NAME;
                    PATIENTINFO.AppendChild(WARD_NAME);

                    System.Xml.XmlElement BED_NO = xml.CreateElement("BED_NO");
                    BED_NO.InnerText = obj.BED_NO;
                    PATIENTINFO.AppendChild(BED_NO);

                    System.Xml.XmlElement NAME = xml.CreateElement("NAME");
                    NAME.InnerText = obj.NAME;
                    PATIENTINFO.AppendChild(NAME);

                    System.Xml.XmlElement SEX = xml.CreateElement("SEX");
                    SEX.InnerText = obj.SEX;
                    PATIENTINFO.AppendChild(SEX);

                    System.Xml.XmlElement BIRTHDAY = xml.CreateElement("BIRTHDAY");
                    BIRTHDAY.InnerText = obj.BIRTHDAY.ToString();
                    PATIENTINFO.AppendChild(BIRTHDAY);

                    System.Xml.XmlElement AGE_OF_YEAR = xml.CreateElement("AGE_OF_YEAR");
                    AGE_OF_YEAR.InnerText = obj.AGE_OF_YEAR;
                    PATIENTINFO.AppendChild(AGE_OF_YEAR);

                    System.Xml.XmlElement AGE_OF_MONTH = xml.CreateElement("AGE_OF_MONTH");
                    AGE_OF_MONTH.InnerText = obj.AGE_OF_MONTH;
                    PATIENTINFO.AppendChild(AGE_OF_MONTH);

                    System.Xml.XmlElement AGE_OF_DAY = xml.CreateElement("AGE_OF_DAY");
                    AGE_OF_DAY.InnerText = obj.AGE_OF_DAY;
                    PATIENTINFO.AppendChild(AGE_OF_DAY);

                    System.Xml.XmlElement HEIGHT = xml.CreateElement("HEIGHT");
                    HEIGHT.InnerText = obj.HEIGHT;
                    PATIENTINFO.AppendChild(HEIGHT);

                    System.Xml.XmlElement WEIGHT = xml.CreateElement("WEIGHT");
                    WEIGHT.InnerText = obj.WEIGHT;
                    PATIENTINFO.AppendChild(WEIGHT);

                    System.Xml.XmlElement MOBILE_PHONE = xml.CreateElement("MOBILE_PHONE");
                    MOBILE_PHONE.InnerText = obj.MOBILE_PHONE;
                    PATIENTINFO.AppendChild(MOBILE_PHONE);

                    System.Xml.XmlElement ADDRESS = xml.CreateElement("ADDRESS");
                    ADDRESS.InnerText = obj.ADDRESS;
                    PATIENTINFO.AppendChild(ADDRESS);

                    System.Xml.XmlElement ID_NO = xml.CreateElement("ID_NO");
                    ID_NO.InnerText = obj.ID_NO;
                    PATIENTINFO.AppendChild(ID_NO);

                    System.Xml.XmlElement CHARGE_TYPE = xml.CreateElement("CHARGE_TYPE");
                    CHARGE_TYPE.InnerText = obj.CHARGE_TYPE;
                    PATIENTINFO.AppendChild(CHARGE_TYPE);

                    System.Xml.XmlElement BALANCE = xml.CreateElement("BALANCE");
                    BALANCE.InnerText = obj.BALANCE;
                    PATIENTINFO.AppendChild(BALANCE);

                    System.Xml.XmlElement IN_HOS_DATE_TIME = xml.CreateElement("IN_HOS_DATE_TIME");
                    IN_HOS_DATE_TIME.InnerText = obj.IN_HOS_DATE_TIME.ToString();
                    PATIENTINFO.AppendChild(IN_HOS_DATE_TIME);

                    System.Xml.XmlElement DIAGNOSIS = xml.CreateElement("DIAGNOSIS");
                    DIAGNOSIS.InnerText = obj.DIAGNOSIS;
                    PATIENTINFO.AppendChild(DIAGNOSIS);

                    System.Xml.XmlElement SETTLED_INDICATOR = xml.CreateElement("SETTLED_INDICATOR");
                    SETTLED_INDICATOR.InnerText = obj.SETTLED_INDICATOR;
                    PATIENTINFO.AppendChild(SETTLED_INDICATOR);

                    System.Xml.XmlElement OUT_HOS_DATE = xml.CreateElement("OUT_HOS_DATE");
                    OUT_HOS_DATE.InnerText = obj.OUT_HOS_DATE;
                    PATIENTINFO.AppendChild(OUT_HOS_DATE);

                    System.Xml.XmlElement OUT_STATUS = xml.CreateElement("OUT_STATUS");
                    OUT_STATUS.InnerText = obj.OUT_STATUS;
                    PATIENTINFO.AppendChild(OUT_STATUS);
                }
                return xml.InnerXml.ToString();
            }
                #endregion

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="intime1"></param>
        /// <param name="intime2"></param>
        /// <param name="inpid"></param>
        /// <returns></returns>
        private int GetParamet(string xml, ref string intime1, ref string intime2, ref string inpid)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                Erro = "载入xml有误!";
                return -1;
            }
            #region 获取数据
            try
            {
                System.Xml.XmlNodeList IN_HOS_DATE_TIME = doc.GetElementsByTagName("IN_HOS_DATE_TIME1");
                System.Xml.XmlNode IN_HOS_DATE_TIME1 = IN_HOS_DATE_TIME[0];
                if (!string.IsNullOrEmpty(IN_HOS_DATE_TIME1.InnerText))
                {
                    intime1 = IN_HOS_DATE_TIME1.InnerText;
                }
                else
                {
                    intime1 = "";
                }

                System.Xml.XmlNodeList OUT_HOS_DATE = doc.GetElementsByTagName("IN_HOS_DATE_TIME2");
                System.Xml.XmlNode OUT_HOS_DATE1 = OUT_HOS_DATE[0];
                if (!string.IsNullOrEmpty(OUT_HOS_DATE1.InnerText))
                {
                    intime2 = OUT_HOS_DATE1.InnerText;
                }
                else
                {
                    intime2 = "";
                }

                System.Xml.XmlNodeList INP_NO = doc.GetElementsByTagName("INP_NO");
                System.Xml.XmlNode INP_NO1 = INP_NO[0];
                if (!string.IsNullOrEmpty(INP_NO1.InnerText))
                {
                    inpid = INP_NO1.InnerText;
                }
                else
                {
                    inpid = "ALL";
                }

                if (string.IsNullOrEmpty(intime1) || string.IsNullOrEmpty(intime2))
                {
                    Erro = "查询需提供时间!";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Erro = "获取数据失败!";
                return -1;
            }
            #endregion
            return 1;
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string GetOutPatientYYSS(string xml)
        {
            string intime1 = "";
            string intime2 = "";
            string inpid = "";

            if (GetParamet(xml, ref intime1, ref intime2, ref inpid) > 0)
            {
                System.Collections.ArrayList al = GetOutPatientsData(intime1, intime2, inpid);
                string resultxml = GetOutPatientYYSSXML(al);
                return resultxml;
            }
            else
            {
                return GetErroXml();
            }
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        private System.Collections.ArrayList GetOutPatientsData(string intime1, string intime2,string inpid)
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            try
            {
                string sql = PatentInfo;

                string selectsql = string.Format(sql, inpid, intime1, intime2);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(selectsql);

                DateTime now = Function.GetSysDate();//当前时间
                #region 获取实体
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    His.Models.YYSS.InPatientInfo inpatient = new His.Models.YYSS.InPatientInfo();
                    inpatient.PATIENT_ID = dt.Rows[i][0].ToString();
                    inpatient.INP_NO = dt.Rows[i][1].ToString();
                    inpatient.VISIT_ID = dt.Rows[i][2].ToString();
                    inpatient.DEPT_CODE = dt.Rows[i][3].ToString();
                    inpatient.DEPT_NAME = dt.Rows[i][4].ToString();
                    inpatient.WARD_CODE = dt.Rows[i][5].ToString();
                    inpatient.WARD_NAME = dt.Rows[i][6].ToString();
                    inpatient.BED_NO = dt.Rows[i][7].ToString();
                    inpatient.NAME = dt.Rows[i][8].ToString();
                    inpatient.SEX = dt.Rows[i][9].ToString();
                    inpatient.BIRTHDAY = Convert.ToDateTime(dt.Rows[i][10].ToString());
                    #region 获取年龄
                    string AGE = dt.Rows[i][11].ToString();
                    //DateTime bir = Convert.ToDateTime(inpatient.BIRTHDAY);
                    if (AGE.Contains("岁"))
                    {
                        int yeasit = AGE.IndexOf("岁");
                        inpatient.AGE_OF_YEAR = AGE.Substring(0, yeasit);
                        int patage = Convert.ToInt32(inpatient.AGE_OF_YEAR);
                        if (patage >= 14)
                        {
                            inpatient.AGE_OF_MONTH = "";
                            inpatient.AGE_OF_DAY = "";
                        }
                        else if (patage < 14 && patage >= 1)
                        {
                            if (AGE.Contains("月"))
                            {
                                int monthsit = AGE.IndexOf("月");
                                inpatient.AGE_OF_MONTH = AGE.Substring(yeasit + 1, monthsit - yeasit - 1);
                            }
                            else
                            {
                                inpatient.AGE_OF_MONTH = "";
                                inpatient.AGE_OF_DAY = "";
                            }
                        }
                    }
                    else
                    {
                        inpatient.AGE_OF_YEAR = "";
                        if (AGE.Contains("月"))
                        {
                            int monthsit = AGE.IndexOf("月");
                            inpatient.AGE_OF_MONTH = AGE.Substring(0, monthsit);
                            if (AGE.Contains("天"))
                            {
                                int daysit = AGE.IndexOf("天");
                                inpatient.AGE_OF_DAY = AGE.Substring(monthsit + 1, daysit - monthsit - 1);
                            }
                            else
                            {
                                inpatient.AGE_OF_DAY = "";
                            }
                        }
                        else
                        {
                            inpatient.AGE_OF_YEAR = "";
                            inpatient.AGE_OF_MONTH = "";
                            if (AGE.Contains("天"))
                            {
                                int daysit = AGE.IndexOf("天");
                                inpatient.AGE_OF_DAY = AGE.Substring(0, daysit);
                            }
                            else
                            {
                                inpatient.AGE_OF_DAY = "";
                            }
                        }
                    }


                    #region
                    //int yea = now.Year - inpatient.BIRTHDAY.Year;
                    //if (yea >= 14)
                    //{
                    //    int agesit = AGE.IndexOf("岁");
                    //    inpatient.AGE_OF_YEAR = AGE.Substring(0, agesit);
                    //    inpatient.AGE_OF_MONTH = "";
                    //    inpatient.AGE_OF_DAY = "";
                    //}
                    //else if (yea < 14 && yea > 1)
                    //{
                    //    int yeasit = AGE.IndexOf("岁");
                    //    int monthsit = AGE.IndexOf("月");
                    //    inpatient.AGE_OF_YEAR = AGE.Substring(0, yeasit);
                    //    inpatient.AGE_OF_MONTH = AGE.Substring(yeasit + 1, monthsit - yeasit - 1);
                    //    inpatient.AGE_OF_DAY = "";
                    //}
                    //else
                    //{
                    //    if (AGE.Contains("月"))
                    //    {
                    //        int monthsit = AGE.IndexOf("月");
                    //        int daysit = AGE.IndexOf("天");
                    //        inpatient.AGE_OF_YEAR = "";
                    //        inpatient.AGE_OF_MONTH = AGE.Substring(0, monthsit);
                    //        inpatient.AGE_OF_DAY = AGE.Substring(monthsit + 1, daysit - monthsit - 1);
                    //    }
                    //    else
                    //    {
                    //        int daysit = AGE.IndexOf("天");
                    //        inpatient.AGE_OF_YEAR = "";
                    //        inpatient.AGE_OF_MONTH = "";
                    //        inpatient.AGE_OF_DAY = AGE.Substring(0, daysit);

                    //    }
                    //}
                    #endregion
                    #endregion
                    inpatient.HEIGHT = dt.Rows[i][12].ToString();
                    inpatient.WEIGHT = dt.Rows[i][13].ToString();
                    inpatient.MOBILE_PHONE = dt.Rows[i][14].ToString();
                    inpatient.ADDRESS = dt.Rows[i][15].ToString();
                    inpatient.ID_NO = dt.Rows[i][16].ToString();
                    inpatient.CHARGE_TYPE = dt.Rows[i][17].ToString();
                    inpatient.BALANCE = dt.Rows[i][18].ToString();
                    inpatient.IN_HOS_DATE_TIME = Convert.ToDateTime(dt.Rows[i][19].ToString());
                    inpatient.DIAGNOSIS = dt.Rows[i][20].ToString();
                    inpatient.SETTLED_INDICATOR = dt.Rows[i][21].ToString();
                    DateTime outdate = DateTime.MinValue;
                    if (!string.IsNullOrEmpty(dt.Rows[i][22].ToString()))
                    {
                        outdate = Convert.ToDateTime(dt.Rows[i][22].ToString());
                    }
                    if (outdate > DateTime.MinValue)
                    {
                        inpatient.OUT_HOS_DATE = dt.Rows[i][22].ToString();
                    }
                    else
                    {
                        inpatient.OUT_HOS_DATE = "";
                    }
                    inpatient.OUT_STATUS = dt.Rows[i][23].ToString();
                    al.Add(inpatient);
                }
                #endregion
            }
            catch (Exception e)
            {
                Erro = "获取病人信息失败!" + e.ToString();
                return new System.Collections.ArrayList();
            }

            return al;
        }

        #endregion

        #region 获取门诊医嘱

        /// <summary>
        /// 获取入参
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private int GetParamet(string xml, ref string inpatientno)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                Erro = "载入xml有误!";
                return -1;
            }
            #region 获取数据
            try
            {
                System.Xml.XmlNodeList PATIENT_ID = doc.GetElementsByTagName("PATIENT_ID");
                System.Xml.XmlNode PATIENT_ID1 = PATIENT_ID[0];
                if (!string.IsNullOrEmpty(PATIENT_ID1.InnerText))
                {
                    inpatientno = PATIENT_ID1.InnerText;
                }
                else
                {
                    inpatientno = "";
                }

            }
            catch (Exception ex)
            {
                Erro = "获取数据失败!";
                return -1;
            }
            #endregion
            return 1;
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string GetOutPatientOrderYYSS(string xml)
        {
            string patientid = "";
            if (GetParamet(xml, ref patientid) > 0)
            {
                if (string.IsNullOrEmpty(patientid))
                {
                    Erro = "病人唯一标识不能为空!";
                    return GetErroXml();
                }
                else
                {
                    System.Collections.ArrayList al = GetOutPatientsOrderData(patientid);
                    string resultxml = GetOutPatientOrderXML(al);
                    return resultxml;
                }
            }
            else
            {
                return GetErroXml();
            }
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        private System.Collections.ArrayList GetOutPatientsOrderData(string patientid)
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            try
            {
                string selectsql = string.Format(OutPatientOrderSql, patientid);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(selectsql);
                #region 获取实体
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    His.Models.YYSS.PatientOrder obj = new His.Models.YYSS.PatientOrder();
                    obj.PATIENT_ID = dt.Rows[i][0].ToString();
                    obj.VISIT_ID = dt.Rows[i][1].ToString();
                    obj.ORDER_NO = dt.Rows[i][2].ToString();
                    obj.ORDER_SUB_NO = dt.Rows[i][3].ToString();
                    obj.START_DATE_TIME = Convert.ToDateTime(dt.Rows[i][4].ToString());
                    obj.STOP_DATE_TIME = dt.Rows[i][5].ToString();
                    obj.REPEAT_INDICATOR = dt.Rows[i][6].ToString();
                    obj.ORDER_CLASS = dt.Rows[i][7].ToString();
                    obj.ORDER_CLASS_NAME = dt.Rows[i][8].ToString();
                    obj.ORDER_CODE = dt.Rows[i][9].ToString();
                    obj.ORDER_TEXT = dt.Rows[i][10].ToString();
                    obj.ORDER_STATUS = dt.Rows[i][11].ToString();
                    obj.DOSAGE = dt.Rows[i][12].ToString();
                    obj.DOSAGE_UNITS = dt.Rows[i][13].ToString();
                    obj.DURATION = dt.Rows[i][14].ToString();
                    obj.DURATION_UNITS = dt.Rows[i][15].ToString();
                    obj.FREQ_COUNTER = dt.Rows[i][16].ToString();
                    obj.FREQ_INTERVAL = dt.Rows[i][17].ToString();
                    obj.FREQ_INTERVAL_UNIT = dt.Rows[i][18].ToString();
                    obj.FREQ_DETAIL = dt.Rows[i][19].ToString();
                    obj.PERFORM_SCHEDULE = Convert.ToDateTime(dt.Rows[i][20].ToString());
                    obj.PERFORM_RESULT = dt.Rows[i][21].ToString();
                    obj.ORDERING_DEPT = dt.Rows[i][22].ToString();
                    obj.DOCTOR = dt.Rows[i][23].ToString();
                    obj.STOP_DOCTOR = dt.Rows[i][24].ToString();
                    obj.NURSE = dt.Rows[i][25].ToString();
                    obj.STOP_NURSE = dt.Rows[i][26].ToString();
                    obj.ENTER_DATE_TIME = Convert.ToDateTime(dt.Rows[i][27].ToString());
                    obj.STOP_ORDER_DATE_TIME = dt.Rows[i][28].ToString();
                    al.Add(obj);
                }
                #endregion
                return al;
            }
            catch (Exception e)
            {
                Erro = "获取数据失败!" + e.ToString();
                return new System.Collections.ArrayList(); ;
            }
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="al"></param>
        /// <returns></returns>
        private string GetOutPatientOrderXML(System.Collections.ArrayList al)
        {
            if (al == null || al.Count == 0)
            {
                #region
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
                ErrorMsg.InnerText = Erro;
                root1.AppendChild(ErrorMsg);

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);

                #endregion
                return xml.InnerXml.ToString();
            }
            else
            {
                #region
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

                System.Xml.XmlElement ORDERS = xml.CreateElement("ORDERS");
                root1.AppendChild(ORDERS);

                foreach (His.Models.YYSS.PatientOrder obj in al)
                {
                    System.Xml.XmlElement ORDER = xml.CreateElement("ORDER");
                    ORDERS.AppendChild(ORDER);

                    System.Xml.XmlElement PATIENT_ID = xml.CreateElement("PATIENT_ID");
                    PATIENT_ID.InnerText = obj.PATIENT_ID;
                    ORDER.AppendChild(PATIENT_ID);

                    System.Xml.XmlElement VISIT_ID = xml.CreateElement("VISIT_ID");
                    VISIT_ID.InnerText = obj.VISIT_ID;
                    ORDER.AppendChild(VISIT_ID);

                    System.Xml.XmlElement ORDER_NO = xml.CreateElement("ORDER_NO");
                    ORDER_NO.InnerText = obj.ORDER_NO;
                    ORDER.AppendChild(ORDER_NO);

                    System.Xml.XmlElement ORDER_SUB_NO = xml.CreateElement("ORDER_SUB_NO");
                    ORDER_SUB_NO.InnerText = obj.ORDER_SUB_NO;
                    ORDER.AppendChild(ORDER_SUB_NO);

                    System.Xml.XmlElement START_DATE_TIME = xml.CreateElement("START_DATE_TIME");
                    START_DATE_TIME.InnerText = obj.START_DATE_TIME.ToString();
                    ORDER.AppendChild(START_DATE_TIME);

                    System.Xml.XmlElement STOP_DATE_TIME = xml.CreateElement("STOP_DATE_TIME");
                    STOP_DATE_TIME.InnerText = obj.STOP_DATE_TIME;
                    ORDER.AppendChild(STOP_DATE_TIME);

                    System.Xml.XmlElement REPEAT_INDICATOR = xml.CreateElement("REPEAT_INDICATOR");
                    REPEAT_INDICATOR.InnerText = obj.REPEAT_INDICATOR;
                    ORDER.AppendChild(REPEAT_INDICATOR);

                    System.Xml.XmlElement ORDER_CLASS = xml.CreateElement("ORDER_CLASS");
                    ORDER_CLASS.InnerText = obj.ORDER_CLASS;
                    ORDER.AppendChild(ORDER_CLASS);

                    System.Xml.XmlElement ORDER_CLASS_NAME = xml.CreateElement("ORDER_CLASS_NAME");
                    ORDER_CLASS_NAME.InnerText = obj.ORDER_CLASS_NAME;
                    ORDER.AppendChild(ORDER_CLASS_NAME);

                    System.Xml.XmlElement ORDER_CODE = xml.CreateElement("ORDER_CODE");
                    ORDER_CODE.InnerText = obj.ORDER_CODE;
                    ORDER.AppendChild(ORDER_CODE);

                    System.Xml.XmlElement ORDER_TEXT = xml.CreateElement("ORDER_TEXT");
                    ORDER_TEXT.InnerText = obj.ORDER_TEXT;
                    ORDER.AppendChild(ORDER_TEXT);

                    System.Xml.XmlElement ORDER_STATUS = xml.CreateElement("ORDER_STATUS");
                    ORDER_STATUS.InnerText = obj.ORDER_STATUS;
                    ORDER.AppendChild(ORDER_STATUS);

                    System.Xml.XmlElement DOSAGE = xml.CreateElement("DOSAGE");
                    DOSAGE.InnerText = obj.DOSAGE;
                    ORDER.AppendChild(DOSAGE);

                    System.Xml.XmlElement DOSAGE_UNITS = xml.CreateElement("DOSAGE_UNITS");
                    DOSAGE_UNITS.InnerText = obj.DOSAGE_UNITS;
                    ORDER.AppendChild(DOSAGE_UNITS);

                    System.Xml.XmlElement DURATION = xml.CreateElement("DURATION");
                    DURATION.InnerText = obj.DURATION;
                    ORDER.AppendChild(DURATION);

                    System.Xml.XmlElement DURATION_UNITS = xml.CreateElement("DURATION_UNITS");
                    DURATION_UNITS.InnerText = obj.DURATION_UNITS;
                    ORDER.AppendChild(DURATION_UNITS);

                    System.Xml.XmlElement FREQ_COUNTER = xml.CreateElement("FREQ_COUNTER");
                    FREQ_COUNTER.InnerText = obj.FREQ_COUNTER;
                    ORDER.AppendChild(FREQ_COUNTER);

                    System.Xml.XmlElement FREQ_INTERVAL = xml.CreateElement("FREQ_INTERVAL");
                    FREQ_INTERVAL.InnerText = obj.FREQ_INTERVAL;
                    ORDER.AppendChild(FREQ_INTERVAL);

                    System.Xml.XmlElement FREQ_INTERVAL_UNIT = xml.CreateElement("FREQ_INTERVAL_UNIT");
                    FREQ_INTERVAL_UNIT.InnerText = obj.FREQ_INTERVAL_UNIT;
                    ORDER.AppendChild(FREQ_INTERVAL_UNIT);

                    System.Xml.XmlElement FREQ_DETAIL = xml.CreateElement("FREQ_DETAIL");
                    FREQ_DETAIL.InnerText = obj.FREQ_DETAIL;
                    ORDER.AppendChild(FREQ_DETAIL);

                    System.Xml.XmlElement PERFORM_SCHEDULE = xml.CreateElement("PERFORM_SCHEDULE");
                    PERFORM_SCHEDULE.InnerText = obj.PERFORM_SCHEDULE.ToString();
                    ORDER.AppendChild(PERFORM_SCHEDULE);

                    System.Xml.XmlElement PERFORM_RESULT = xml.CreateElement("PERFORM_RESULT");
                    PERFORM_RESULT.InnerText = obj.PERFORM_RESULT;
                    ORDER.AppendChild(PERFORM_RESULT);

                    System.Xml.XmlElement ORDERING_DEPT = xml.CreateElement("ORDERING_DEPT");
                    ORDERING_DEPT.InnerText = obj.ORDERING_DEPT;
                    ORDER.AppendChild(ORDERING_DEPT);

                    System.Xml.XmlElement DOCTOR = xml.CreateElement("DOCTOR");
                    DOCTOR.InnerText = obj.DOCTOR;
                    ORDER.AppendChild(DOCTOR);

                    System.Xml.XmlElement STOP_DOCTOR = xml.CreateElement("STOP_DOCTOR");
                    STOP_DOCTOR.InnerText = obj.STOP_DOCTOR;
                    ORDER.AppendChild(STOP_DOCTOR);

                    System.Xml.XmlElement NURSE = xml.CreateElement("NURSE");
                    NURSE.InnerText = obj.NURSE;
                    ORDER.AppendChild(NURSE);

                    System.Xml.XmlElement STOP_NURSE = xml.CreateElement("STOP_NURSE");
                    STOP_NURSE.InnerText = obj.STOP_NURSE;
                    ORDER.AppendChild(STOP_NURSE);

                    System.Xml.XmlElement ENTER_DATE_TIME = xml.CreateElement("ENTER_DATE_TIME");
                    ENTER_DATE_TIME.InnerText = obj.ENTER_DATE_TIME.ToString();
                    ORDER.AppendChild(ENTER_DATE_TIME);

                    System.Xml.XmlElement STOP_ORDER_DATE_TIME = xml.CreateElement("STOP_ORDER_DATE_TIME");
                    STOP_ORDER_DATE_TIME.InnerText = obj.STOP_ORDER_DATE_TIME;
                    ORDER.AppendChild(STOP_ORDER_DATE_TIME);
                }

                #endregion
                return xml.InnerXml.ToString();
            }
        }

        #endregion

        #region 住院患者信息

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="al"></param>
        /// <returns></returns>
        private string GetInPatientYYSSXML(System.Collections.ArrayList al)
        {

            if (al == null || al.Count == 0)
            {
                #region
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

                #endregion
                return xml.InnerXml.ToString();
            }
            else
            {
                #region
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

                System.Xml.XmlElement PATIENTS = xml.CreateElement("PATIENTS");
                root1.AppendChild(PATIENTS);

                foreach (His.Models.YYSS.InPatientInfo obj in al)
                {
                    System.Xml.XmlElement PATIENTINFO = xml.CreateElement("PATIENTINFO");
                    PATIENTS.AppendChild(PATIENTINFO);

                    System.Xml.XmlElement PATIENT_ID = xml.CreateElement("PATIENT_ID");
                    PATIENT_ID.InnerText = obj.PATIENT_ID;
                    PATIENTINFO.AppendChild(PATIENT_ID);

                    System.Xml.XmlElement INP_NO = xml.CreateElement("INP_NO");
                    INP_NO.InnerText = obj.INP_NO;
                    PATIENTINFO.AppendChild(INP_NO);

                    System.Xml.XmlElement VISIT_ID = xml.CreateElement("VISIT_ID");
                    VISIT_ID.InnerText = obj.VISIT_ID;
                    PATIENTINFO.AppendChild(VISIT_ID);

                    System.Xml.XmlElement DEPT_CODE = xml.CreateElement("DEPT_CODE");
                    DEPT_CODE.InnerText = obj.DEPT_CODE;
                    PATIENTINFO.AppendChild(DEPT_CODE);

                    System.Xml.XmlElement DEPT_NAME = xml.CreateElement("DEPT_NAME");
                    DEPT_NAME.InnerText = obj.DEPT_NAME;
                    PATIENTINFO.AppendChild(DEPT_NAME);

                    System.Xml.XmlElement WARD_CODE = xml.CreateElement("WARD_CODE");
                    WARD_CODE.InnerText = obj.WARD_CODE;
                    PATIENTINFO.AppendChild(WARD_CODE);

                    System.Xml.XmlElement WARD_NAME = xml.CreateElement("WARD_NAME");
                    WARD_NAME.InnerText = obj.WARD_NAME;
                    PATIENTINFO.AppendChild(WARD_NAME);

                    System.Xml.XmlElement BED_NO = xml.CreateElement("BED_NO");
                    BED_NO.InnerText = obj.BED_NO;
                    PATIENTINFO.AppendChild(BED_NO);

                    System.Xml.XmlElement NAME = xml.CreateElement("NAME");
                    NAME.InnerText = obj.NAME;
                    PATIENTINFO.AppendChild(NAME);

                    System.Xml.XmlElement SEX = xml.CreateElement("SEX");
                    SEX.InnerText = obj.SEX;
                    PATIENTINFO.AppendChild(SEX);

                    System.Xml.XmlElement BIRTHDAY = xml.CreateElement("BIRTHDAY");
                    BIRTHDAY.InnerText = obj.BIRTHDAY.ToString();
                    PATIENTINFO.AppendChild(BIRTHDAY);

                    System.Xml.XmlElement AGE_OF_YEAR = xml.CreateElement("AGE_OF_YEAR");
                    AGE_OF_YEAR.InnerText = obj.AGE_OF_YEAR;
                    PATIENTINFO.AppendChild(AGE_OF_YEAR);

                    System.Xml.XmlElement AGE_OF_MONTH = xml.CreateElement("AGE_OF_MONTH");
                    AGE_OF_MONTH.InnerText = obj.AGE_OF_MONTH;
                    PATIENTINFO.AppendChild(AGE_OF_MONTH);

                    System.Xml.XmlElement AGE_OF_DAY = xml.CreateElement("AGE_OF_DAY");
                    AGE_OF_DAY.InnerText = obj.AGE_OF_DAY;
                    PATIENTINFO.AppendChild(AGE_OF_DAY);

                    System.Xml.XmlElement HEIGHT = xml.CreateElement("HEIGHT");
                    HEIGHT.InnerText = obj.HEIGHT;
                    PATIENTINFO.AppendChild(HEIGHT);

                    System.Xml.XmlElement WEIGHT = xml.CreateElement("WEIGHT");
                    WEIGHT.InnerText = obj.WEIGHT;
                    PATIENTINFO.AppendChild(WEIGHT);

                    System.Xml.XmlElement MOBILE_PHONE = xml.CreateElement("MOBILE_PHONE");
                    MOBILE_PHONE.InnerText = obj.MOBILE_PHONE;
                    PATIENTINFO.AppendChild(MOBILE_PHONE);

                    System.Xml.XmlElement ADDRESS = xml.CreateElement("ADDRESS");
                    ADDRESS.InnerText = obj.ADDRESS;
                    PATIENTINFO.AppendChild(ADDRESS);

                    System.Xml.XmlElement ID_NO = xml.CreateElement("ID_NO");
                    ID_NO.InnerText = obj.ID_NO;
                    PATIENTINFO.AppendChild(ID_NO);

                    System.Xml.XmlElement CHARGE_TYPE = xml.CreateElement("CHARGE_TYPE");
                    CHARGE_TYPE.InnerText = obj.CHARGE_TYPE;
                    PATIENTINFO.AppendChild(CHARGE_TYPE);

                    System.Xml.XmlElement BALANCE = xml.CreateElement("BALANCE");
                    BALANCE.InnerText = obj.BALANCE;
                    PATIENTINFO.AppendChild(BALANCE);

                    System.Xml.XmlElement IN_HOS_DATE_TIME = xml.CreateElement("IN_HOS_DATE_TIME");
                    IN_HOS_DATE_TIME.InnerText = obj.IN_HOS_DATE_TIME.ToString();
                    PATIENTINFO.AppendChild(IN_HOS_DATE_TIME);

                    System.Xml.XmlElement DIAGNOSIS = xml.CreateElement("DIAGNOSIS");
                    DIAGNOSIS.InnerText = obj.DIAGNOSIS;
                    PATIENTINFO.AppendChild(DIAGNOSIS);

                    System.Xml.XmlElement SETTLED_INDICATOR = xml.CreateElement("SETTLED_INDICATOR");
                    SETTLED_INDICATOR.InnerText = obj.SETTLED_INDICATOR;
                    PATIENTINFO.AppendChild(SETTLED_INDICATOR);

                    System.Xml.XmlElement OUT_HOS_DATE = xml.CreateElement("OUT_HOS_DATE");
                    OUT_HOS_DATE.InnerText = obj.OUT_HOS_DATE;
                    PATIENTINFO.AppendChild(OUT_HOS_DATE);

                    System.Xml.XmlElement OUT_STATUS = xml.CreateElement("OUT_STATUS");
                    OUT_STATUS.InnerText = obj.OUT_STATUS;
                    PATIENTINFO.AppendChild(OUT_STATUS);
                }
                return xml.InnerXml.ToString();
            }
                #endregion

        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        private System.Collections.ArrayList GetInPatientsData(string statestr, string intime1, string intime2, string wordcode, string inpid)
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            try
            {
                string sql = PatentInfo2;
                if (statestr == "I" && !string.IsNullOrEmpty(intime1) && !string.IsNullOrEmpty(intime2))
                {
                    sql = PatentInfo2 + where1 + wheretime + orderby;
                }
                else if (statestr == "I")
                {
                    sql = PatentInfo2 + where1 + orderby;
                }
                else if (statestr == "O")
                {
                    sql = PatentInfo2 + where2 + wheretime + orderby;
                }

                string selectsql = string.Format(sql, wordcode, inpid, intime1, intime2);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(selectsql);

                DateTime now = Function.GetSysDate();//当前时间
                #region 获取实体
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    His.Models.YYSS.InPatientInfo inpatient = new His.Models.YYSS.InPatientInfo();
                    inpatient.PATIENT_ID = dt.Rows[i][0].ToString();
                    inpatient.INP_NO = dt.Rows[i][1].ToString();
                    inpatient.VISIT_ID = dt.Rows[i][2].ToString();
                    inpatient.DEPT_CODE = dt.Rows[i][3].ToString();
                    inpatient.DEPT_NAME = dt.Rows[i][4].ToString();
                    inpatient.WARD_CODE = dt.Rows[i][5].ToString();
                    inpatient.WARD_NAME = dt.Rows[i][6].ToString();
                    inpatient.BED_NO = dt.Rows[i][7].ToString();
                    inpatient.NAME = dt.Rows[i][8].ToString();
                    inpatient.SEX = dt.Rows[i][9].ToString();
                    inpatient.BIRTHDAY = Convert.ToDateTime(dt.Rows[i][10].ToString());
                    #region 获取年龄
                    string AGE = dt.Rows[i][11].ToString();
                    //DateTime bir = Convert.ToDateTime(inpatient.BIRTHDAY);
                    if (AGE.Contains("岁"))
                    {
                        int yeasit = AGE.IndexOf("岁");
                        inpatient.AGE_OF_YEAR = AGE.Substring(0, yeasit);
                        int patage = Convert.ToInt32(inpatient.AGE_OF_YEAR);
                        if (patage >= 14)
                        {
                            inpatient.AGE_OF_MONTH = "";
                            inpatient.AGE_OF_DAY = "";
                        }
                        else if (patage < 14 && patage >= 1)
                        {
                            if (AGE.Contains("月"))
                            {
                                int monthsit = AGE.IndexOf("月");
                                inpatient.AGE_OF_MONTH = AGE.Substring(yeasit + 1, monthsit - yeasit - 1);
                            }
                            else
                            {
                                inpatient.AGE_OF_MONTH = "";
                                inpatient.AGE_OF_DAY = "";
                            }
                        }
                    }
                    else
                    {
                        inpatient.AGE_OF_YEAR = "";
                        if (AGE.Contains("月"))
                        {
                            int monthsit = AGE.IndexOf("月");
                            inpatient.AGE_OF_MONTH = AGE.Substring(0, monthsit);
                            if (AGE.Contains("天"))
                            {
                                int daysit = AGE.IndexOf("天");
                                inpatient.AGE_OF_DAY = AGE.Substring(monthsit + 1, daysit - monthsit - 1);
                            }
                            else
                            {
                                inpatient.AGE_OF_DAY = "";
                            }
                        }
                        else
                        {
                            inpatient.AGE_OF_YEAR = "";
                            inpatient.AGE_OF_MONTH = "";
                            if (AGE.Contains("天"))
                            {
                                int daysit = AGE.IndexOf("天");
                                inpatient.AGE_OF_DAY = AGE.Substring(0, daysit);
                            }
                            else
                            {
                                inpatient.AGE_OF_DAY = "";
                            }
                        }
                    }


                    #region
                    //int yea = now.Year - inpatient.BIRTHDAY.Year;
                    //if (yea >= 14)
                    //{
                    //    int agesit = AGE.IndexOf("岁");
                    //    inpatient.AGE_OF_YEAR = AGE.Substring(0, agesit);
                    //    inpatient.AGE_OF_MONTH = "";
                    //    inpatient.AGE_OF_DAY = "";
                    //}
                    //else if (yea < 14 && yea > 1)
                    //{
                    //    int yeasit = AGE.IndexOf("岁");
                    //    int monthsit = AGE.IndexOf("月");
                    //    inpatient.AGE_OF_YEAR = AGE.Substring(0, yeasit);
                    //    inpatient.AGE_OF_MONTH = AGE.Substring(yeasit + 1, monthsit - yeasit - 1);
                    //    inpatient.AGE_OF_DAY = "";
                    //}
                    //else
                    //{
                    //    if (AGE.Contains("月"))
                    //    {
                    //        int monthsit = AGE.IndexOf("月");
                    //        int daysit = AGE.IndexOf("天");
                    //        inpatient.AGE_OF_YEAR = "";
                    //        inpatient.AGE_OF_MONTH = AGE.Substring(0, monthsit);
                    //        inpatient.AGE_OF_DAY = AGE.Substring(monthsit + 1, daysit - monthsit - 1);
                    //    }
                    //    else
                    //    {
                    //        int daysit = AGE.IndexOf("天");
                    //        inpatient.AGE_OF_YEAR = "";
                    //        inpatient.AGE_OF_MONTH = "";
                    //        inpatient.AGE_OF_DAY = AGE.Substring(0, daysit);

                    //    }
                    //}
                    #endregion
                    #endregion
                    inpatient.HEIGHT = dt.Rows[i][12].ToString();
                    inpatient.WEIGHT = dt.Rows[i][13].ToString();
                    inpatient.MOBILE_PHONE = dt.Rows[i][14].ToString();
                    inpatient.ADDRESS = dt.Rows[i][15].ToString();
                    inpatient.ID_NO = dt.Rows[i][16].ToString();
                    inpatient.CHARGE_TYPE = dt.Rows[i][17].ToString();
                    inpatient.BALANCE = dt.Rows[i][18].ToString();
                    inpatient.IN_HOS_DATE_TIME = Convert.ToDateTime(dt.Rows[i][19].ToString());
                    inpatient.DIAGNOSIS = dt.Rows[i][20].ToString();
                    inpatient.SETTLED_INDICATOR = dt.Rows[i][21].ToString();
                    DateTime outdate = DateTime.MinValue;
                    if (!string.IsNullOrEmpty(dt.Rows[i][22].ToString()))
                    {
                        outdate = Convert.ToDateTime(dt.Rows[i][22].ToString());
                    }
                    if (outdate > DateTime.MinValue)
                    {
                        inpatient.OUT_HOS_DATE = dt.Rows[i][22].ToString();
                    }
                    else
                    {
                        inpatient.OUT_HOS_DATE = "";
                    }
                    inpatient.OUT_STATUS = dt.Rows[i][23].ToString();
                    al.Add(inpatient);
                }
                #endregion
            }
            catch (Exception e)
            {
                Erro = "获取病人信息失败!" + e.ToString();
                return new System.Collections.ArrayList();
            }

            return al;
        }

        /// <summary>
        /// 获取入参
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private int GetParamet(string xml, ref string statestr, ref string intime1, ref string intime2, ref string wordcode, ref string inpid)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                Erro = "载入xml有误!";
                return -1;
            }
            #region 获取数据
            try
            {
                System.Xml.XmlNodeList IN_STATE = doc.GetElementsByTagName("IN_STATE");
                System.Xml.XmlNode IN_STATE1 = IN_STATE[0];
                if (!string.IsNullOrEmpty(IN_STATE1.InnerText))
                {
                    if (IN_STATE1.InnerText == "1")
                    {
                        statestr = "O";
                    }
                    else if (IN_STATE1.InnerText == "0")
                    {
                        statestr = "I";
                    }
                    else
                    {
                        statestr = "";
                    }
                }
                else
                {
                    statestr = "";
                }

                System.Xml.XmlNodeList IN_HOS_DATE_TIME = doc.GetElementsByTagName("IN_HOS_DATE_TIME1");
                System.Xml.XmlNode IN_HOS_DATE_TIME1 = IN_HOS_DATE_TIME[0];
                if (!string.IsNullOrEmpty(IN_HOS_DATE_TIME1.InnerText))
                {
                    intime1 = IN_HOS_DATE_TIME1.InnerText;
                }
                else
                {
                    intime1 = "";
                }

                System.Xml.XmlNodeList OUT_HOS_DATE = doc.GetElementsByTagName("IN_HOS_DATE_TIME2");
                System.Xml.XmlNode OUT_HOS_DATE1 = OUT_HOS_DATE[0];
                if (!string.IsNullOrEmpty(OUT_HOS_DATE1.InnerText))
                {
                    intime2 = OUT_HOS_DATE1.InnerText;
                }
                else
                {
                    intime2 = "";
                }

                System.Xml.XmlNodeList WORD_CODE = doc.GetElementsByTagName("DEPT_CODE");
                System.Xml.XmlNode WORD_CODE1 = WORD_CODE[0];
                if (!string.IsNullOrEmpty(WORD_CODE1.InnerText))
                {
                    wordcode = WORD_CODE1.InnerText;
                }
                else
                {
                    wordcode = "ALL";
                }

                System.Xml.XmlNodeList INP_NO = doc.GetElementsByTagName("INP_NO");
                System.Xml.XmlNode INP_NO1 = INP_NO[0];
                if (!string.IsNullOrEmpty(INP_NO1.InnerText))
                {
                    inpid = INP_NO1.InnerText;
                }
                else
                {
                    inpid = "ALL";
                }

                if (statestr == "O" && (string.IsNullOrEmpty(intime1) || string.IsNullOrEmpty(intime2)))
                {
                    Erro = "查询出院患者时,需提供时间!";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Erro = "获取数据失败!";
                return -1;
            }
            #endregion
            return 1;
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string GetInPatientYYSS(string xml)
        {
            string statestr = string.Empty;//在院标识
            string intime1 = string.Empty;//入院时间1
            string intime2 = string.Empty;//入院时间2
            string wordcode = string.Empty;//病区编码
            string inpid = string.Empty;//住院号
            //            string inparamet = @"<?xml version='1.0' encoding='UTF-8'?>
            //                                <DataSource>
            //                                <IN_STATE>1</IN_STATE>
            //                                <IN_HOS_DATE_TIME1>2016-08-06</IN_HOS_DATE_TIME1>
            //                                <IN_HOS_DATE_TIME2>2016-08-07</IN_HOS_DATE_TIME2>
            //                                <WORD_CODE></WORD_CODE>
            //                                <INP_NO></INP_NO>
            //                                </DataSource>";
            if (GetParamet(xml, ref statestr, ref intime1, ref intime2, ref wordcode, ref inpid) > 0)
            {
                if (string.IsNullOrEmpty(statestr))
                {
                    Erro = "在院标识不能为空!";
                    return GetErroXml();
                }
                else
                {
                    System.Collections.ArrayList al = GetInPatientsData(statestr, intime1, intime2, wordcode, inpid);
                    string resultxml = GetInPatientYYSSXML(al);
                    return resultxml;
                }
            }
            else
            {
                return GetErroXml();
            }
        }

        #endregion

        #region 住院患者医嘱信息

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="al"></param>
        /// <returns></returns>
        private string GetInPatientOrderXML(System.Collections.ArrayList al)
        {
            if (al == null || al.Count == 0)
            {
                #region
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
                ErrorMsg.InnerText = Erro;
                root1.AppendChild(ErrorMsg);

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);

                #endregion
                return xml.InnerXml.ToString();
            }
            else
            {
                #region
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

                System.Xml.XmlElement ORDERS = xml.CreateElement("ORDERS");
                root1.AppendChild(ORDERS);

                foreach (His.Models.YYSS.PatientOrder obj in al)
                {
                    System.Xml.XmlElement ORDER = xml.CreateElement("ORDER");
                    ORDERS.AppendChild(ORDER);

                    System.Xml.XmlElement PATIENT_ID = xml.CreateElement("PATIENT_ID");
                    PATIENT_ID.InnerText = obj.PATIENT_ID;
                    ORDER.AppendChild(PATIENT_ID);

                    System.Xml.XmlElement VISIT_ID = xml.CreateElement("VISIT_ID");
                    VISIT_ID.InnerText = obj.VISIT_ID;
                    ORDER.AppendChild(VISIT_ID);

                    System.Xml.XmlElement ORDER_NO = xml.CreateElement("ORDER_NO");
                    ORDER_NO.InnerText = obj.ORDER_NO;
                    ORDER.AppendChild(ORDER_NO);

                    System.Xml.XmlElement ORDER_SUB_NO = xml.CreateElement("ORDER_SUB_NO");
                    ORDER_SUB_NO.InnerText = obj.ORDER_SUB_NO;
                    ORDER.AppendChild(ORDER_SUB_NO);

                    System.Xml.XmlElement START_DATE_TIME = xml.CreateElement("START_DATE_TIME");
                    START_DATE_TIME.InnerText = obj.START_DATE_TIME.ToString();
                    ORDER.AppendChild(START_DATE_TIME);

                    System.Xml.XmlElement STOP_DATE_TIME = xml.CreateElement("STOP_DATE_TIME");
                    STOP_DATE_TIME.InnerText = obj.STOP_DATE_TIME;
                    ORDER.AppendChild(STOP_DATE_TIME);

                    System.Xml.XmlElement REPEAT_INDICATOR = xml.CreateElement("REPEAT_INDICATOR");
                    REPEAT_INDICATOR.InnerText = obj.REPEAT_INDICATOR;
                    ORDER.AppendChild(REPEAT_INDICATOR);

                    System.Xml.XmlElement ORDER_CLASS = xml.CreateElement("ORDER_CLASS");
                    ORDER_CLASS.InnerText = obj.ORDER_CLASS;
                    ORDER.AppendChild(ORDER_CLASS);

                    System.Xml.XmlElement ORDER_CLASS_NAME = xml.CreateElement("ORDER_CLASS_NAME");
                    ORDER_CLASS_NAME.InnerText = obj.ORDER_CLASS_NAME;
                    ORDER.AppendChild(ORDER_CLASS_NAME);

                    System.Xml.XmlElement ORDER_CODE = xml.CreateElement("ORDER_CODE");
                    ORDER_CODE.InnerText = obj.ORDER_CODE;
                    ORDER.AppendChild(ORDER_CODE);

                    System.Xml.XmlElement ORDER_TEXT = xml.CreateElement("ORDER_TEXT");
                    ORDER_TEXT.InnerText = obj.ORDER_TEXT;
                    ORDER.AppendChild(ORDER_TEXT);

                    System.Xml.XmlElement ORDER_STATUS = xml.CreateElement("ORDER_STATUS");
                    ORDER_STATUS.InnerText = obj.ORDER_STATUS;
                    ORDER.AppendChild(ORDER_STATUS);

                    System.Xml.XmlElement DOSAGE = xml.CreateElement("DOSAGE");
                    DOSAGE.InnerText = obj.DOSAGE;
                    ORDER.AppendChild(DOSAGE);

                    System.Xml.XmlElement DOSAGE_UNITS = xml.CreateElement("DOSAGE_UNITS");
                    DOSAGE_UNITS.InnerText = obj.DOSAGE_UNITS;
                    ORDER.AppendChild(DOSAGE_UNITS);

                    System.Xml.XmlElement DURATION = xml.CreateElement("DURATION");
                    DURATION.InnerText = obj.DURATION;
                    ORDER.AppendChild(DURATION);

                    System.Xml.XmlElement DURATION_UNITS = xml.CreateElement("DURATION_UNITS");
                    DURATION_UNITS.InnerText = obj.DURATION_UNITS;
                    ORDER.AppendChild(DURATION_UNITS);

                    System.Xml.XmlElement FREQ_COUNTER = xml.CreateElement("FREQ_COUNTER");
                    FREQ_COUNTER.InnerText = obj.FREQ_COUNTER;
                    ORDER.AppendChild(FREQ_COUNTER);

                    System.Xml.XmlElement FREQ_INTERVAL = xml.CreateElement("FREQ_INTERVAL");
                    FREQ_INTERVAL.InnerText = obj.FREQ_INTERVAL;
                    ORDER.AppendChild(FREQ_INTERVAL);

                    System.Xml.XmlElement FREQ_INTERVAL_UNIT = xml.CreateElement("FREQ_INTERVAL_UNIT");
                    FREQ_INTERVAL_UNIT.InnerText = obj.FREQ_INTERVAL_UNIT;
                    ORDER.AppendChild(FREQ_INTERVAL_UNIT);

                    System.Xml.XmlElement FREQ_DETAIL = xml.CreateElement("FREQ_DETAIL");
                    FREQ_DETAIL.InnerText = obj.FREQ_DETAIL;
                    ORDER.AppendChild(FREQ_DETAIL);

                    System.Xml.XmlElement PERFORM_SCHEDULE = xml.CreateElement("PERFORM_SCHEDULE");
                    PERFORM_SCHEDULE.InnerText = obj.PERFORM_SCHEDULE.ToString();
                    ORDER.AppendChild(PERFORM_SCHEDULE);

                    System.Xml.XmlElement PERFORM_RESULT = xml.CreateElement("PERFORM_RESULT");
                    PERFORM_RESULT.InnerText = obj.PERFORM_RESULT;
                    ORDER.AppendChild(PERFORM_RESULT);

                    System.Xml.XmlElement ORDERING_DEPT = xml.CreateElement("ORDERING_DEPT");
                    ORDERING_DEPT.InnerText = obj.ORDERING_DEPT;
                    ORDER.AppendChild(ORDERING_DEPT);

                    System.Xml.XmlElement DOCTOR = xml.CreateElement("DOCTOR");
                    DOCTOR.InnerText = obj.DOCTOR;
                    ORDER.AppendChild(DOCTOR);

                    System.Xml.XmlElement STOP_DOCTOR = xml.CreateElement("STOP_DOCTOR");
                    STOP_DOCTOR.InnerText = obj.STOP_DOCTOR;
                    ORDER.AppendChild(STOP_DOCTOR);

                    System.Xml.XmlElement NURSE = xml.CreateElement("NURSE");
                    NURSE.InnerText = obj.NURSE;
                    ORDER.AppendChild(NURSE);

                    System.Xml.XmlElement STOP_NURSE = xml.CreateElement("STOP_NURSE");
                    STOP_NURSE.InnerText = obj.STOP_NURSE;
                    ORDER.AppendChild(STOP_NURSE);

                    System.Xml.XmlElement ENTER_DATE_TIME = xml.CreateElement("ENTER_DATE_TIME");
                    ENTER_DATE_TIME.InnerText = obj.ENTER_DATE_TIME.ToString();
                    ORDER.AppendChild(ENTER_DATE_TIME);

                    System.Xml.XmlElement STOP_ORDER_DATE_TIME = xml.CreateElement("STOP_ORDER_DATE_TIME");
                    STOP_ORDER_DATE_TIME.InnerText = obj.STOP_ORDER_DATE_TIME;
                    ORDER.AppendChild(STOP_ORDER_DATE_TIME);
                }

                #endregion
                return xml.InnerXml.ToString();
            }
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        private System.Collections.ArrayList GetInPatientsOrderData(string patientid)
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            try
            {
                string selectsql = string.Format(InPatientOrderSql, patientid);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(selectsql);
                #region 获取实体
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    His.Models.YYSS.PatientOrder obj = new His.Models.YYSS.PatientOrder();
                    obj.PATIENT_ID = dt.Rows[i][0].ToString();
                    obj.VISIT_ID = dt.Rows[i][1].ToString();
                    obj.ORDER_NO = dt.Rows[i][2].ToString();
                    obj.ORDER_SUB_NO = dt.Rows[i][3].ToString();
                    obj.START_DATE_TIME = Convert.ToDateTime(dt.Rows[i][4].ToString());
                    obj.STOP_DATE_TIME = dt.Rows[i][5].ToString();
                    obj.REPEAT_INDICATOR = dt.Rows[i][6].ToString();
                    obj.ORDER_CLASS = dt.Rows[i][7].ToString();
                    obj.ORDER_CLASS_NAME = dt.Rows[i][8].ToString();
                    obj.ORDER_CODE = dt.Rows[i][9].ToString();
                    obj.ORDER_TEXT = dt.Rows[i][10].ToString();
                    obj.ORDER_STATUS = dt.Rows[i][11].ToString();
                    obj.DOSAGE = dt.Rows[i][12].ToString();
                    obj.DOSAGE_UNITS = dt.Rows[i][13].ToString();
                    obj.DURATION = dt.Rows[i][14].ToString();
                    obj.DURATION_UNITS = dt.Rows[i][15].ToString();
                    obj.FREQ_COUNTER = dt.Rows[i][16].ToString();
                    obj.FREQ_INTERVAL = dt.Rows[i][17].ToString();
                    obj.FREQ_INTERVAL_UNIT = dt.Rows[i][18].ToString();
                    obj.FREQ_DETAIL = dt.Rows[i][19].ToString();
                    obj.PERFORM_SCHEDULE = Convert.ToDateTime(dt.Rows[i][20].ToString());
                    obj.PERFORM_RESULT = dt.Rows[i][21].ToString();
                    obj.ORDERING_DEPT = dt.Rows[i][22].ToString();
                    obj.DOCTOR = dt.Rows[i][23].ToString();
                    obj.STOP_DOCTOR = dt.Rows[i][24].ToString();
                    obj.NURSE = dt.Rows[i][25].ToString();
                    obj.STOP_NURSE = dt.Rows[i][26].ToString();
                    obj.ENTER_DATE_TIME = Convert.ToDateTime(dt.Rows[i][27].ToString());
                    obj.STOP_ORDER_DATE_TIME = dt.Rows[i][28].ToString();
                    al.Add(obj);
                }
                #endregion
                return al;
            }
            catch (Exception e)
            {
                Erro = "获取数据失败!" + e.ToString();
                return new System.Collections.ArrayList(); ;
            }
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string GetInPatientOrderYYSS(string xml)
        {
            string patientid = "";
            //            xml = @"<?xml version='1.0' encoding='UTF-8'?>
            //                                            <DataSource>
            //                                            <PATIENT_ID>28043</PATIENT_ID>
            //                                            </DataSource>";
            if (GetParamet(xml, ref patientid) > 0)
            {
                if (string.IsNullOrEmpty(patientid))
                {
                    Erro = "病人唯一标识不能为空!";
                    return GetErroXml();
                }
                else
                {
                    System.Collections.ArrayList al = GetInPatientsOrderData(patientid);
                    string resultxml = GetInPatientOrderXML(al);
                    return resultxml;
                }
            }
            else
            {
                return GetErroXml();
            }
        }

        #endregion

        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <returns></returns>
        public string GetErroXml()
        {
            #region 错误xml
            System.Xml.XmlDocument resultxml = new System.Xml.XmlDocument();
            resultxml.AppendChild(resultxml.CreateXmlDeclaration("1.0", "UTF-8", null));
            System.Xml.XmlElement root = resultxml.CreateElement("DataSource");
            resultxml.AppendChild(root);

            System.Xml.XmlElement root1 = resultxml.CreateElement("return");
            root.AppendChild(root1);

            System.Xml.XmlElement Code = resultxml.CreateElement("Code");
            Code.InnerText = "0";
            root1.AppendChild(Code);

            System.Xml.XmlElement ErrorMsg = resultxml.CreateElement("ErrorMsg");
            ErrorMsg.InnerText = Erro;
            root1.AppendChild(ErrorMsg);

            System.Xml.XmlElement Result = resultxml.CreateElement("Result");
            root1.AppendChild(Result);
            #endregion
            return resultxml.InnerXml.ToString();
        }
    }
}
