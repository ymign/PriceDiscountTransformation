using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.YYSS
{
    public class OutPatientApply
    {
        private System.Collections.ArrayList GetOutPatientApplyData(His.Models.YYSS.InPatientApply outPatientApply)
        {
            #region sql
            string sql = @" select null empi, --患者主索引号码
                   f.seeno visit_id, --就诊次序号
                   f.clinic_code ptnt_id, --患者就诊ID
                   t.card_no ptnt_no, --患者就诊ID
                   t.card_no ic_card, --卡号
                   t.name patient_name, --患者姓名
                   decode(f.sex_code,'F','女','M','男','0') patient_sex, --患者性别
                   f.idenno id_card, --身份证号
                   t.birthday patient_birth, --出生日期
                   fun_get_age(t.birthday) patient_age, --年龄
                   t.home_tel patient_telephone, --联系电话
                   t.home_zip zip_code, --邮政编码
                   t.home address, --住址
                   f.dept_code, --病区代码
                   f.dept_name, --病区名字
                   f.begin_time clinic_date, --看诊时间
                     fun_get_diagnose(f.clinic_code) diagnose
              from com_patientinfo t left join  fin_opr_register f on f.card_no=t.card_no
             where 1=1
                   --and f.dept_code='6048'
                   and (f.clinic_code='{0}' OR '{0}'='ALL')
                   and (t.card_no='{1}' or '{1}'='ALL')
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, outPatientApply.PTNT_ID,outPatientApply.IC_CARD);

                System.Data.DataTable dt = new System.Data.DataTable();
                //申请单
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    outPatientApply = new His.Models.YYSS.InPatientApply();
                    outPatientApply.EMPI = dt.Rows[i][0].ToString();
                    outPatientApply.VISIT_ID = dt.Rows[i][1].ToString();
                    outPatientApply.PTNT_ID = dt.Rows[i][2].ToString();
                    outPatientApply.PTNT_NO = dt.Rows[i][3].ToString();
                    outPatientApply.IC_CARD = dt.Rows[i][4].ToString();
                    outPatientApply.PATIENT_NAME = dt.Rows[i][5].ToString();
                    outPatientApply.PATIENT_SEX = dt.Rows[i][6].ToString();
                    outPatientApply.ID_CARD = dt.Rows[i][7].ToString();
                    outPatientApply.PATIENT_BIRTH = dt.Rows[i][8].ToString();
                    outPatientApply.PATIENT_AGE = dt.Rows[i][9].ToString();
                    outPatientApply.PATIENT_TELEPHONE = dt.Rows[i][10].ToString();
                    outPatientApply.ZIP_CODE = dt.Rows[i][11].ToString();
                    outPatientApply.ADDRESS = dt.Rows[i][12].ToString();
                    outPatientApply.DEPT_ID = dt.Rows[i][13].ToString();
                    outPatientApply.DEPT_NAME = dt.Rows[i][14].ToString();
                    outPatientApply.CLINIC_DATE = dt.Rows[i][15].ToString();
                    outPatientApply.DIAGNOSE = dt.Rows[i][16].ToString();
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

        private string ERR()
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

        private string GetOutPatientApplyXML(System.Collections.ArrayList al)
        {

            #region
            if (al == null||al.Count == 0)
            {
                return this.ERR();
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

              
                //His.Models.YYSS.OutPatientApply p = al[0] as His.Models.YYSS.OutPatientApply;
                foreach (His.Models.YYSS.InPatientApply p in al)
                {
                    if (p.PTNT_NO=="ALL"&&p.IC_CARD=="ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement PATIENTINFO = xml.CreateElement("PATIENTINFO");
                    Result.AppendChild(PATIENTINFO);

                    System.Xml.XmlElement EMPI = xml.CreateElement("EMPI");
                    EMPI.InnerText = p.EMPI;
                    PATIENTINFO.AppendChild(EMPI);

                    System.Xml.XmlElement VISIT_ID = xml.CreateElement("VISIT_ID");
                    VISIT_ID.InnerText = p.VISIT_ID;
                    PATIENTINFO.AppendChild(VISIT_ID);

                    System.Xml.XmlElement PTNT_ID = xml.CreateElement("PTNT_ID");
                    PTNT_ID.InnerText = p.PTNT_ID;
                    PATIENTINFO.AppendChild(PTNT_ID);

                    System.Xml.XmlElement PTNT_NO = xml.CreateElement("PTNT_NO");
                    PTNT_NO.InnerText = p.PTNT_NO;
                    PATIENTINFO.AppendChild(PTNT_NO);

                    System.Xml.XmlElement IC_CARD = xml.CreateElement("IC_CARD");
                    IC_CARD.InnerText = p.IC_CARD;
                    PATIENTINFO.AppendChild(IC_CARD);

                    System.Xml.XmlElement PATIENT_NAME = xml.CreateElement("PATIENT_NAME");
                    PATIENT_NAME.InnerText = p.PATIENT_NAME;
                    PATIENTINFO.AppendChild(PATIENT_NAME);

                    System.Xml.XmlElement PATIENT_SEX = xml.CreateElement("PATIENT_SEX");
                    PATIENT_SEX.InnerText = p.PATIENT_SEX;
                    PATIENTINFO.AppendChild(PATIENT_SEX);

                    System.Xml.XmlElement ID_CARD = xml.CreateElement("ID_CARD");
                    ID_CARD.InnerText = p.ID_CARD;
                    PATIENTINFO.AppendChild(ID_CARD);

                    System.Xml.XmlElement PATIENT_BIRTH = xml.CreateElement("PATIENT_BIRTH");
                    PATIENT_BIRTH.InnerText = p.PATIENT_BIRTH;
                    PATIENTINFO.AppendChild(PATIENT_BIRTH);

                    System.Xml.XmlElement PATIENT_AGE = xml.CreateElement("PATIENT_AGE");
                    PATIENT_AGE.InnerText = p.PATIENT_AGE;
                    PATIENTINFO.AppendChild(PATIENT_AGE);

                    System.Xml.XmlElement PATIENT_PHONE = xml.CreateElement("PATIENT_PHONE");
                    PATIENT_PHONE.InnerText = p.PATIENT_PHONE;
                    PATIENTINFO.AppendChild(PATIENT_PHONE);

                    System.Xml.XmlElement ZIP_CODE = xml.CreateElement("ZIP_CODE");
                    ZIP_CODE.InnerText = p.ZIP_CODE;
                    PATIENTINFO.AppendChild(ZIP_CODE);

                    System.Xml.XmlElement ADDRESS = xml.CreateElement("ADDRESS");
                    ADDRESS.InnerText = p.ADDRESS;
                    PATIENTINFO.AppendChild(ADDRESS);

                    System.Xml.XmlElement DEPT_CODE = xml.CreateElement("DEPT_CODE");
                    DEPT_CODE.InnerText = p.DEPT_ID;
                    PATIENTINFO.AppendChild(DEPT_CODE);

                    System.Xml.XmlElement DEPT_NAME = xml.CreateElement("DEPT_NAME");
                    DEPT_NAME.InnerText = p.DEPT_NAME;
                    PATIENTINFO.AppendChild(DEPT_NAME);

                    System.Xml.XmlElement CLINIC_DATE = xml.CreateElement("CLINIC_DATE");
                    CLINIC_DATE.InnerText = p.CLINIC_DATE;
                    PATIENTINFO.AppendChild(CLINIC_DATE);

                    System.Xml.XmlElement DIAGNOSIS = xml.CreateElement("DIAGNOSIS");
                    DIAGNOSIS.InnerText = p.DIAGNOSIS;
                    PATIENTINFO.AppendChild(DIAGNOSIS);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private His.Models.YYSS.InPatientApply GetOutPatientModel(string xml)
        {
            His.Models.YYSS.InPatientApply opa = new His.Models.YYSS.InPatientApply();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList PATIENT_NAME1 = doc.GetElementsByTagName("PATIENT_NAME");
            System.Xml.XmlNode PATIENT_NAME = PATIENT_NAME1[0];
            if (!string.IsNullOrEmpty(PATIENT_NAME.InnerText))
            {
                opa.PATIENT_NAME = PATIENT_NAME.InnerText;
            }
            else
            {
                opa.PATIENT_NAME = "ALL";
            }

            System.Xml.XmlNodeList PTNT_ID1 = doc.GetElementsByTagName("PTNT_ID");
            System.Xml.XmlNode PTNT_ID = PTNT_ID1[0];
            if (!string.IsNullOrEmpty(PTNT_ID.InnerText))
            {
                opa.PTNT_ID = PTNT_ID.InnerText;
            }
            else
            {
                opa.PTNT_ID = "ALL";
            }

            System.Xml.XmlNodeList EMPI1 = doc.GetElementsByTagName("EMPI");
            System.Xml.XmlNode EMPI = EMPI1[0];
            if (!string.IsNullOrEmpty(EMPI.InnerText))
            {
                opa.EMPI = EMPI.InnerText;
            }
            else
            {
                opa.EMPI = "ALL";
            }

            System.Xml.XmlNodeList IC_CARD1 = doc.GetElementsByTagName("IC_CARD");
            System.Xml.XmlNode IC_CARD = IC_CARD1[0];
            if (!string.IsNullOrEmpty(IC_CARD.InnerText))
            {
                opa.IC_CARD = IC_CARD.InnerText;
            }
            else
            {
                opa.IC_CARD = "ALL";
            }

            System.Xml.XmlNodeList ID_CARD1 = doc.GetElementsByTagName("ID_CARD");
            System.Xml.XmlNode ID_CARD = ID_CARD1[0];
            if (!string.IsNullOrEmpty(ID_CARD.InnerText))
            {
                opa.ID_CARD = ID_CARD.InnerText;
            }
            else
            {
                opa.ID_CARD = "ALL";
            }

            System.Xml.XmlNodeList DEPT_ID1 = doc.GetElementsByTagName("DEPT_ID");
            System.Xml.XmlNode DEPT_ID = DEPT_ID1[0];
            if (!string.IsNullOrEmpty(DEPT_ID.InnerText))
            {
                opa.DEPT_ID = DEPT_ID.InnerText;
            }
            else
            {
                opa.DEPT_ID = "ALL";
            }

            System.Xml.XmlNodeList START_TIME1 = doc.GetElementsByTagName("START_TIME");
            System.Xml.XmlNode START_TIME = START_TIME1[0];
            if (!string.IsNullOrEmpty(START_TIME.InnerText))
            {
                opa.START_TIME = START_TIME.InnerText;
            }
            else
            {
                opa.START_TIME = "ALL";
            }

            System.Xml.XmlNodeList END_TIME1 = doc.GetElementsByTagName("END_TIME");
            System.Xml.XmlNode END_TIME = END_TIME1[0];
            if (!string.IsNullOrEmpty(END_TIME.InnerText))
            {
                opa.END_TIME = END_TIME.InnerText;
            }
            else
            {
                opa.END_TIME = "ALL";
            }

            return opa;
        }

        public string GetOutPatientInfoForYYMZ(string xml)
        {
            string returnStr = "";
            His.Models.YYSS.InPatientApply opa = new His.Models.YYSS.InPatientApply();
            opa=this.GetOutPatientModel(xml);
            System.Collections.ArrayList al = this.GetOutPatientApplyData(opa);
            returnStr = this.GetOutPatientApplyXML(al);
            return returnStr;
        }
    }
}
