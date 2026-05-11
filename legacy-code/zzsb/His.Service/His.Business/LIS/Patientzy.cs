using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.LIS
{

    public class Patientzy
    {
        /// <summary>
        /// 结果代码
        /// </summary>
        private string resultCode = string.Empty;
        /// <summary>
        /// 处理信息
        /// </summary>
        private string msg = string.Empty;

        /// <summary>
        /// 获取住院病人基本信息
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string GetInPatientInfoForZY(string xml)
        {
            string returnStr = "";
            His.Models.LIS.InPatient Ipa = new His.Models.LIS.InPatient();
            returnStr = this.GetInStrModel(xml, ref Ipa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            His.Models.LIS.InPatient ipaRet = new His.Models.LIS.InPatient();
            ipaRet = this.GetInPatientData(Ipa.Patient_no);
            if (ipaRet != null)
            {
                returnStr = this.GetOutStrXML(ipaRet);
            }
            else
            {
                return this.ERR();
            }
            return returnStr;
        }

        /// <summary>
        /// 生成返回数据
        /// </summary>
        /// <param name="inPatient"></param>
        /// <returns></returns>
        private string GetOutStrXML(His.Models.LIS.InPatient inPatient)
        {
            if (string.IsNullOrEmpty(inPatient.Dept_name))
            {
                return this.ERR();
            }
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

            System.Xml.XmlElement OpTime = xml.CreateElement("OpTime");
            OpTime.InnerText = "";
            root1.AppendChild(OpTime);

            System.Xml.XmlElement FunCode = xml.CreateElement("FunCode");
            FunCode.InnerText = "";
            root1.AppendChild(FunCode);

            System.Xml.XmlElement Result = xml.CreateElement("Result");
            root1.AppendChild(Result);

            System.Xml.XmlElement PATIENTNO = xml.CreateElement("PATIENTNO");
            PATIENTNO.InnerText = inPatient.Patient_no;
            Result.AppendChild(PATIENTNO);

            System.Xml.XmlElement PATIENTNAME = xml.CreateElement("PATIENTNAME");
            PATIENTNAME.InnerText = inPatient.Name;
            Result.AppendChild(PATIENTNAME);

            System.Xml.XmlElement PATIENTSEX = xml.CreateElement("PATIENTSEX");
            PATIENTSEX.InnerText = inPatient.Sex;
            Result.AppendChild(PATIENTSEX);

            System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
            DEPTNAME.InnerText = inPatient.Dept_name;
            Result.AppendChild(DEPTNAME);

            System.Xml.XmlElement DEPTCODE = xml.CreateElement("DEPTCODE");
            DEPTCODE.InnerText = inPatient.Dept_code;
            Result.AppendChild(DEPTCODE);

            return xml.InnerXml.ToString();
        }

        /// <summary>
        /// 根据住院号获取病人此次住院基本信息
        /// </summary>
        /// <param name="patientNo"></param>
        /// <returns></returns>
        private His.Models.LIS.InPatient GetInPatientData(string patientNo)
        {
            #region sql
            string sqlStr = @"SELECT PATIENT_NO, NAME, SEX_CODE, DEPT_CODE, DEPT_NAME
                              FROM FIN_IPR_INMAININFO A
                              WHERE A.PATIENT_NO = '{0}'
                              AND A.IN_STATE = 'I' ";
            #endregion

            #region sql1
            string sqlStr1 = @"SELECT r.card_no PATIENT_NO, r.name, r.sex_code, r.dept_code, r.dept_name 
                            FROM fin_opr_register r 
                            WHERE r.clinic_code = (SELECT  MAX(r.clinic_code) FROM fin_opr_register r WHERE r.card_no = '{0}') ";
            #endregion
            try
            {
                sqlStr = string.Format(sqlStr, patientNo);
                sqlStr1 = string.Format(sqlStr1, patientNo);
            }
            catch (Exception e)
            {
                this.resultCode = "0";
                this.msg = "格式化sql失败";
                this.ReturnFailure();
                return null;
            }

            System.Data.DataTable dt = new System.Data.DataTable();
            //网络测试
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sqlStr);
            if (dt != null)
            {
                if (dt.Rows.Count < 1)
                {
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(sqlStr1);
                    
                    //this.resultCode = "0";
                    //this.msg = "此住院号患者没有在院记录，请核实";
                    //this.ReturnFailure();
                    //return null;
                    if (dt != null)
                    {
                        if (dt.Rows.Count < 1)
                        {
                            this.resultCode = "0";
                            this.msg = "此号患者没有在院记录，请核实";
                            this.ReturnFailure();
                            return null;
                        }
                    }
                }
            }
            //else
            //{   
            //    this.resultCode = "0";
            //    this.msg = "此住院号患者没有在院记录，请核实";
            //    this.ReturnFailure();
            //    return null;
            //}
            His.Models.LIS.InPatient inPatientMode = new His.Models.LIS.InPatient();
            inPatientMode.Patient_no = dt.Rows[0]["PATIENT_NO"].ToString();
            inPatientMode.Name = dt.Rows[0]["NAME"].ToString();
            inPatientMode.Sex = dt.Rows[0]["SEX_CODE"].ToString();
            inPatientMode.Dept_code = dt.Rows[0]["DEPT_CODE"].ToString();
            inPatientMode.Dept_name = dt.Rows[0]["DEPT_NAME"].ToString();
            //inPatientMode.Patient_no = dt.Rows[0][0].ToString();
            //inPatientMode.Name = dt.Rows[0][1].ToString();
            //inPatientMode.Sex = dt.Rows[0][2].ToString();
            //inPatientMode.Dept_code = dt.Rows[0][3].ToString();
            //inPatientMode.Dept_name = dt.Rows[0][4].ToString();
            return inPatientMode;
        }

        /// <summary>
        /// 根据传入XML取得数据
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="Ipa"></param>
        /// <returns></returns>
        private string GetInStrModel(string xml, ref His.Models.LIS.InPatient Ipa)  //"patien_id"
        {
            string returnStr = "";
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                this.resultCode = "0";
                this.msg = e.Message;
                return this.ReturnFailure();
            }
            System.Xml.XmlNode inpatientNode = doc.GetElementsByTagName("patientNo")[0];
            if (Ipa == null)
            {
                Ipa = new His.Models.LIS.InPatient();
            }
            if (!string.IsNullOrEmpty(inpatientNode.InnerText))
            {
                Ipa.Patient_no = inpatientNode.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "住院流水号不能为空！";
                return this.ReturnFailure();
            }
            return returnStr;
        }


        #region 错误处理
        private string ReturnFailure()
        {
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            System.Xml.XmlElement root = xml.CreateElement("DataSource");
            xml.AppendChild(root);

            System.Xml.XmlElement root1 = xml.CreateElement("return");
            root.AppendChild(root1);

            System.Xml.XmlElement Code = xml.CreateElement("Code");
            Code.InnerText = this.resultCode;
            root1.AppendChild(Code);

            System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
            ErrorMsg.InnerText = this.msg;
            root1.AppendChild(ErrorMsg);

            System.Xml.XmlElement Result = xml.CreateElement("Result");
            Result.InnerText = this.resultCode;//resurt值
            root1.AppendChild(Result);

            return xml.InnerXml.ToString();
        }

        private string ERR()
        {
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            System.Xml.XmlElement root = xml.CreateElement("DataSource");

            System.Xml.XmlElement root1 = xml.CreateElement("return");
            root.AppendChild(root1);

            System.Xml.XmlElement Code = xml.CreateElement("Code");
            Code.InnerText = "0";
            root1.AppendChild(Code);

            System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
            ErrorMsg.InnerText = "查找信息失败，请核对后查询";
            root1.AppendChild(ErrorMsg);

            System.Xml.XmlElement OpTime = xml.CreateElement("OpTime");
            OpTime.InnerText = "0";
            root1.AppendChild(OpTime);

            System.Xml.XmlElement FunCode = xml.CreateElement("FunCode");
            FunCode.InnerText = "0";
            root1.AppendChild(FunCode);

            System.Xml.XmlElement Result = xml.CreateElement("Result");
            root1.AppendChild(Result);

            xml.AppendChild(root);

            return xml.InnerXml.ToString();
        }
        #endregion

    }

}
