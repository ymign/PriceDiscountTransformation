using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.YYSS
{
    public class InPatientYYSS
    {
        string Erro = string.Empty;//错误信息;
        #region sql
        public static string PatentInfo = @"
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

        public static string where2= @"
                    WHERE A.IN_STATE not in  ('I','N')
                    AND (A.DEPT_CODE='{0}' OR 'ALL'='{0}')
                    AND (A.PATIENT_NO='{1}' OR 'ALL'='{1}')
                    ";

        public static string where1 = @"
                    WHERE A.IN_STATE in ('I','R')
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
        private System.Collections.ArrayList GetInPatientsData(string statestr,string intime1,string intime2,string wordcode ,string inpid)
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            try
            {
                string sql = PatentInfo;
                if (statestr == "I" && !string.IsNullOrEmpty(intime1) && !string.IsNullOrEmpty(intime2))
                {
                    sql = PatentInfo + where1 + wheretime + orderby;
                }
                else if (statestr == "I")
                {
                    sql = PatentInfo + where1 + orderby;
                }
                else if (statestr == "O")
                {
                    sql = PatentInfo + where2 + wheretime + orderby;
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
            catch(Exception e)
            {
                Erro = "获取病人信息失败!"+e.ToString();
                return new System.Collections.ArrayList();
            }

            return al;
        }

        /// <summary>
        /// 获取入参
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private int GetParamet(string xml,ref string statestr,ref string intime1,ref string intime2,ref string wordcode,ref string inpid)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch(Exception e)
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
