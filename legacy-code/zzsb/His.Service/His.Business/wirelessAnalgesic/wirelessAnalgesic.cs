using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.wirelessAnalgesic
{
   public  class wirelessAnalgesic
    {

        /// <summary>
        /// 无线镇痛信息获取
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        private int GetwirelessAnalgesicInfo(ref His.Models.wirelessAnalgesic.PatientInfo patientInfo)
        {
            int result = -1;
            //DataSource source = new DataSource();
            #region  sql
            string sql = @"
                            SELECT t.patient_no as ZYHM, --住院号
                            t.NAME AS XM, --姓名
                            decode(t.sex_code, 'M', '男', 'F', '女') XB, --性别
                            fun_get_dept_name(t.dept_code) AS BQ, --病区
                            SUBSTR(t.BED_NO, 5) AS CH, --床号
                            round((sysdate - t.birthday) / 365, '0') as NL, --年龄
                            t.weight as TZ, --体重
                            (select t2.item_name
                            from met_ops_operationitem t2
                            where t1.operationno = t2.operationno
                            and t2.clinic_code = t1.clinic_code
                            and rownum = 1) as SSMC, --手术名称
                            '' as ASA, --ASA分级
                            (select fun_get_employee_name(a.empl_code)
                            from met_ops_arrange a
                            where a.role_code = 'Anaesthetist'
                            and a.operationno = t1.operationno) as YS, --麻醉医生
                            '' as ZTFS, --镇痛方式
                            '' as PF, --配方
                            t.home PATIENT_ADDR,--地址
                            t.idenno ID_CARD,--身份证
                            t.paykind_code FEE_TYPE,
                            fun_get_dept_name(t.dept_code) DEPT_NAME ,--科别
                            t.clinic_diagnose DIAGNOSE
                            -- select * 
                            from FIN_IPR_INMAININFO t, MET_OPS_APPLY t1
                            where t.inpatient_no = t1.clinic_code
                            and t1.ynvalid = 1
                            and t1.operationno=(select max(a.operationno) from met_ops_apply a
                            where a.patient_no='{0}')
                            ";

            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, patientInfo.PATIENT_ID);

                System.Data.DataTable dt = new System.Data.DataTable();
                //申请单
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                patientInfo = new His.Models.wirelessAnalgesic.PatientInfo();
                patientInfo.PATIENT_ID = dt.Rows[0][0].ToString();
                patientInfo.PATIENT_NAME = dt.Rows[0][1].ToString();
                patientInfo.PATIENT_SEX = dt.Rows[0][2].ToString();
                patientInfo.WARD = dt.Rows[0][3].ToString();
                patientInfo.BED_NO = dt.Rows[0][4].ToString();
                patientInfo.PATIENT_AGE = dt.Rows[0][5].ToString();
                patientInfo.PATIENT_WEIGHT = dt.Rows[0][6].ToString();
                patientInfo.OPERATION_NAME = dt.Rows[0][7].ToString();
                patientInfo.ASA_LEVEL = dt.Rows[0][8].ToString();
                patientInfo.DOCTOR_NAME = dt.Rows[0][9].ToString();
                patientInfo.ANALGESIA_STYLE = dt.Rows[0][10].ToString();
                patientInfo.FORMULA_NAME = dt.Rows[0][11].ToString();
                patientInfo.PATIENT_ADDR = dt.Rows[0][12].ToString();
                patientInfo.ID_CARD = dt.Rows[0][13].ToString();
                patientInfo.FEE_TYPE = dt.Rows[0][14].ToString();
                patientInfo.DEPT_NAME = dt.Rows[0][15].ToString();
                patientInfo.DIAGNOSE = dt.Rows[0][16].ToString();
                #endregion
                result= 1;
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        private string GetInOperationInfoXML(His.Models.wirelessAnalgesic.PatientInfo p)
        {
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UT-8", null));
            System.Xml.XmlElement root = xml.CreateElement("OperationInfo");
            xml.AppendChild(root);

            #region 
            System.Xml.XmlElement PATIENT_ID = xml.CreateElement("PATIENT_ID");
            PATIENT_ID.InnerText = p.PATIENT_ID;
            root.AppendChild(PATIENT_ID);

            System.Xml.XmlElement CARDNO = xml.CreateElement("CARDNO");
            CARDNO.InnerText = p.CARDNO;
            root.AppendChild(CARDNO);

            System.Xml.XmlElement EMPI = xml.CreateElement("EMPI");
            EMPI.InnerText = p.EMPI;
            root.AppendChild(EMPI);

            System.Xml.XmlElement PATIENT_NAME = xml.CreateElement("PATIENT_NAME");
            PATIENT_NAME.InnerText = p.PATIENT_NAME;
            root.AppendChild(PATIENT_NAME);

            System.Xml.XmlElement PATIENT_SEX = xml.CreateElement("PATIENT_SEX");
            PATIENT_SEX.InnerText = p.PATIENT_SEX;
            root.AppendChild(PATIENT_SEX);

            System.Xml.XmlElement WARD = xml.CreateElement("WARD");
            WARD.InnerText = p.WARD;
            root.AppendChild(WARD);

            System.Xml.XmlElement BED_NO = xml.CreateElement("BED_NO");
            BED_NO.InnerText = p.BED_NO;
            root.AppendChild(BED_NO);

            System.Xml.XmlElement PATIENT_AGE = xml.CreateElement("PATIENT_AGE");
            PATIENT_AGE.InnerText = p.PATIENT_AGE;
            root.AppendChild(PATIENT_AGE);

            System.Xml.XmlElement PATIENT_WEIGHT = xml.CreateElement("PATIENT_WEIGHT");
            PATIENT_WEIGHT.InnerText = p.PATIENT_WEIGHT;
            root.AppendChild(PATIENT_WEIGHT);

            System.Xml.XmlElement OPERATION_NAME = xml.CreateElement("OPERATION_NAME");
            OPERATION_NAME.InnerText = p.OPERATION_NAME;
            root.AppendChild(OPERATION_NAME);

            System.Xml.XmlElement ASA_LEVEL = xml.CreateElement("ASA_LEVEL");
            ASA_LEVEL.InnerText = p.ASA_LEVEL;
            root.AppendChild(ASA_LEVEL);

            System.Xml.XmlElement DOCTOR_NAME = xml.CreateElement("DOCTOR_NAME");
            DOCTOR_NAME.InnerText = p.DOCTOR_NAME;
            root.AppendChild(DOCTOR_NAME);

            System.Xml.XmlElement ANALGESIA_STYLE = xml.CreateElement("ANALGESIA_STYLE");
            ANALGESIA_STYLE.InnerText = p.ANALGESIA_STYLE;
            root.AppendChild(ANALGESIA_STYLE);

            System.Xml.XmlElement FORMULA_NAME = xml.CreateElement("FORMULA_NAME");
            FORMULA_NAME.InnerText = p.FORMULA_NAME;
            root.AppendChild(FORMULA_NAME);

            System.Xml.XmlElement OPERATION_TIME = xml.CreateElement("OPERATION_TIME");
            OPERATION_TIME.InnerText = p.OPERATION_TIME;
            root.AppendChild(OPERATION_TIME);

            System.Xml.XmlElement PATIENT_ADDR = xml.CreateElement("PATIENT_ADDR");
            OPERATION_TIME.InnerText = p.PATIENT_ADDR;
            root.AppendChild(PATIENT_ADDR);

            System.Xml.XmlElement ID_CARD = xml.CreateElement("ID_CARD");
            OPERATION_TIME.InnerText = p.ID_CARD;
            root.AppendChild(ID_CARD);

            System.Xml.XmlElement FEE_TYPE = xml.CreateElement("FEE_TYPE");
            OPERATION_TIME.InnerText = p.FEE_TYPE;
            root.AppendChild(FEE_TYPE);

            System.Xml.XmlElement DEPT_NAME = xml.CreateElement("DEPT_NAME");
            OPERATION_TIME.InnerText = p.DEPT_NAME;
            root.AppendChild(DEPT_NAME);

            System.Xml.XmlElement DIAGNOSE = xml.CreateElement("DIAGNOSE");
            OPERATION_TIME.InnerText = p.DIAGNOSE;
            root.AppendChild(DIAGNOSE);

            #endregion
            return xml.InnerXml.ToString();
        }

        private string GetInOperationPatientIdResult(string xml)
        {
            string PATIENT_ID = "";
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return "";
            }

            System.Xml.XmlNode PATIENTID_xmlNode = doc.SelectSingleNode("DataSource/PatientInfo/PATIENT_ID");
            if (PATIENTID_xmlNode != null)
            {
                PATIENT_ID = PATIENTID_xmlNode.InnerText;
            }
            else
            {
                PATIENT_ID = "";
            }
            return PATIENT_ID;
        }

        public string GetInOperationInfo(string patientId)
        {
           // His.Util.Common.HisLog.WriteLog("无线镇痛",xml);
            His.Models.wirelessAnalgesic.PatientInfo patientInfo = new His.Models.wirelessAnalgesic.PatientInfo();
            patientInfo.PATIENT_ID = patientId;// this.GetInOperationPatientIdResult(xml);
            string returnStr = "";
            this.GetwirelessAnalgesicInfo(ref patientInfo);
            returnStr = this.GetInOperationInfoXML(patientInfo);
          //  His.Util.Common.HisLog.WriteLog("无线镇痛",returnStr);
            return returnStr;
        }
    }
}
