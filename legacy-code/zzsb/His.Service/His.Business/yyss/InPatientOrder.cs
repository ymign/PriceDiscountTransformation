using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.YYSS
{
    public class InPatientOrder
    {
        public string Erro = "";//错误提示

        #region sql
        public string InPatientOrderSql = @"
                                    SELECT 
                                    O.INPATIENT_NO PATIENT_ID,
                                    I.IN_TIMES VISIT_ID,
                                    O.MO_ORDER ORDER_NO,
                                    O.SUBCOMBNO ORDER_SUB_NO,
                                    TO_CHAR(O.DATE_BGN,'yyyy-mm-dd hh24:mi:ss') START_DATE_TIME,
                                    TO_CHAR(O.DATE_END,'yyyy-mm-dd hh24:mi:ss') STOP_DATE_TIME,
                                    DECODE(O.TYPE_CODE,'CZ','1','LZ','0','0') REPEAT_INDICATOR,
                                    'MF' AS ORDER_CLASS,
                                    '膳食' AS ORDER_CLASS_NAME,
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
                                    WHERE O.CLASS_CODE in ('M','MF') 
                                    AND I.INPATIENT_NO=O.INPATIENT_NO
                                    AND O.INPATIENT_NO='{0}'
                                    ";
        #endregion

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
            catch(Exception e)
            {
                Erro = "获取数据失败!"+e.ToString();
                return new System.Collections.ArrayList(); ;
            }
        }

        /// <summary>
        /// 获取入参
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private int GetParamet(string xml,ref string inpatientno)
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
        /// 返回错误信息
        /// </summary>
        /// <returns></returns>
        private string GetErroXml()
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
            if (GetParamet(xml,ref patientid) > 0)
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

    }
}
