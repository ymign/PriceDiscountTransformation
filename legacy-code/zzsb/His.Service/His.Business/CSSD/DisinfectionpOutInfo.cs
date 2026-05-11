using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.CSSD
{
    public class GetDisinfectionOutpInfo
    {
        /// <summary>
        /// 结果代码
        /// </summary>
        private string resultCode = string.Empty;
        /// <summary>
        /// 处理信息
        /// </summary>
        private string msg = string.Empty;

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


        private System.Collections.ArrayList GetDisinfectionOutpInfoData(His.Models.CSSD.InDisinfectionInfo GetDisinfectionOutpInfo)
        {
            #region sql
            string sql = @"select a.card_no clinicnumber, --诊疗号
       a.card_no serialnumber, --当前诊疗流水号
       a.name patientname, --病人姓名
       fun_get_age(a.birthday) patientage, --病人年龄
       a.sex_code patientsex, --病人年龄
       a.idenno patientidcard， --病人身份证号
       fun_get_employee_name(a.doct_code) doctorname,
       (select item_name
          from met_ord_recipedetail b
         where a.clinic_code = b.clinic_code
           and b.class_code = 'UO'
           and rownum = 1) operation, --手术名称
       a.reg_date operationtime, --手术时间
       a.dept_name inpatientarea, --病人所属科室
       null remark --备注
  from fin_opr_register a,fin_opb_accountcard t
 where a.card_no=t.card_no
 and (t.markno ='{0}' or a.card_no='{0}')
 and rownum=1
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, GetDisinfectionOutpInfo.CLINICNUMBER);

                System.Data.DataTable dt = new System.Data.DataTable();
                //门诊患者信息查询
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GetDisinfectionOutpInfo = new His.Models.CSSD.InDisinfectionInfo();
                    GetDisinfectionOutpInfo.CLINICNUMBER = dt.Rows[i][0].ToString();
                    GetDisinfectionOutpInfo.SERIALNUMBER = dt.Rows[i][1].ToString();
                    GetDisinfectionOutpInfo.PATIENTNAME = dt.Rows[i][2].ToString();
                    GetDisinfectionOutpInfo.PATIENTAGE = dt.Rows[i][3].ToString();
                    GetDisinfectionOutpInfo.PATIENTSEX = dt.Rows[i][4].ToString();
                    GetDisinfectionOutpInfo.PATIENTIDCARD = dt.Rows[i][5].ToString();
                    GetDisinfectionOutpInfo.DOCTORNAME = dt.Rows[i][6].ToString();
                    GetDisinfectionOutpInfo.OPERATION = dt.Rows[i][7].ToString();
                    GetDisinfectionOutpInfo.OPERATIONTIME = dt.Rows[i][8].ToString();
                    GetDisinfectionOutpInfo.INPATIENTAREA = dt.Rows[i][9].ToString();
                    GetDisinfectionOutpInfo.REMARK = dt.Rows[i][10].ToString();
                    al.Add(GetDisinfectionOutpInfo);
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

        private string GetDisinfectionOutpInfoXML(System.Collections.ArrayList al)
        {

            #region
            if (al == null || al.Count == 0)
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

                System.Xml.XmlElement OpTime = xml.CreateElement("OpTime");
                OpTime.InnerText = "";
                root1.AppendChild(OpTime);

                System.Xml.XmlElement FunCode = xml.CreateElement("FunCode");
                FunCode.InnerText = "";
                root1.AppendChild(FunCode);


                //His.Models.ZZSB.GetGuideListInfoForSRM p = al[0] as His.Models.ZZSB.GetGuideListInfoForSRM;
                foreach (His.Models.CSSD.InDisinfectionInfo p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CLINICNUMBER == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement CLINICNUMBER = xml.CreateElement("CLINICNUMBER");
                    CLINICNUMBER.InnerText = p.CLINICNUMBER;
                    Result.AppendChild(CLINICNUMBER);

                    System.Xml.XmlElement SERIALNUMBER = xml.CreateElement("SERIALNUMBER");
                    SERIALNUMBER.InnerText = p.SERIALNUMBER;
                    Result.AppendChild(SERIALNUMBER);

                    System.Xml.XmlElement PATIENTNAME = xml.CreateElement("PATIENTNAME");
                    PATIENTNAME.InnerText = p.PATIENTNAME;
                    Result.AppendChild(PATIENTNAME);

                    System.Xml.XmlElement PATIENTAGE = xml.CreateElement("PATIENTAGE");
                    PATIENTAGE.InnerText = p.PATIENTAGE;
                    Result.AppendChild(PATIENTAGE);

                    System.Xml.XmlElement PATIENTSEX = xml.CreateElement("PATIENTSEX");
                    PATIENTSEX.InnerText = p.PATIENTSEX;
                    Result.AppendChild(PATIENTSEX);

                    System.Xml.XmlElement PATIENTIDCARD = xml.CreateElement("PATIENTIDCARD");
                    PATIENTIDCARD.InnerText = p.PATIENTIDCARD;
                    Result.AppendChild(PATIENTIDCARD);

                    System.Xml.XmlElement DOCTORNAME = xml.CreateElement("DOCTORNAME");
                    DOCTORNAME.InnerText = p.DOCTORNAME;
                    Result.AppendChild(DOCTORNAME);

                    System.Xml.XmlElement OPERATION = xml.CreateElement("OPERATION");
                    OPERATION.InnerText = p.OPERATION;
                    Result.AppendChild(OPERATION);

                    System.Xml.XmlElement OPERATIONTIME = xml.CreateElement("OPERATIONTIME");
                    OPERATIONTIME.InnerText = p.OPERATIONTIME;
                    Result.AppendChild(OPERATIONTIME);

                    System.Xml.XmlElement INPATIENTAREA = xml.CreateElement("INPATIENTAREA");
                    INPATIENTAREA.InnerText = p.INPATIENTAREA;
                    Result.AppendChild(INPATIENTAREA);

                    System.Xml.XmlElement REMARK = xml.CreateElement("REMARK");
                    REMARK.InnerText = p.REMARK;
                    Result.AppendChild(REMARK);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetDisinfectionOutpInfoModel(string xml, ref His.Models.CSSD.InDisinfectionInfo opa)
        {

            string returnStr = "";
            opa = new His.Models.CSSD.InDisinfectionInfo();
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


            System.Xml.XmlNodeList CLINICNUMBER1 = doc.GetElementsByTagName("clinicNumber");
            System.Xml.XmlNode CLINICNUMBER = CLINICNUMBER1[0];
            if (!string.IsNullOrEmpty(CLINICNUMBER.InnerText))
            {
                opa.CLINICNUMBER = CLINICNUMBER.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "门诊号不能为空！";
                return this.ReturnFailure();
            }


            return returnStr;
        }



        public string GetDisinfectionOutpInfoForCSSD(string xml)
        {
            string returnStr = "";
            His.Models.CSSD.InDisinfectionInfo opa = new His.Models.CSSD.InDisinfectionInfo();
            returnStr = this.GetDisinfectionOutpInfoModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetDisinfectionOutpInfoData(opa);
            returnStr = this.GetDisinfectionOutpInfoXML(al);
            return returnStr;
        }
    }


    public class GetDisinfectionInpInfo
    {
        /// <summary>
        /// 结果代码
        /// </summary>
        private string resultCode = string.Empty;
        /// <summary>
        /// 处理信息
        /// </summary>
        private string msg = string.Empty;

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


        private System.Collections.ArrayList GetDisinfectionInpInfoDate(His.Models.CSSD.InDisinfectionInfo GetDisinfectionInpInfo)
        {
            #region sql
            string sql = @"select a.patient_no hospitalnumger, --诊疗号
                            a.inpatient_no serialnumber,--当前诊疗流水号
                            a.name patientname,--病人姓名
                            fun_get_age(a.birthday) patientage,--病人年龄
                            a.sex_code patientsex,--病人年龄
                            a.idenno patientidcard， --病人身份证号
                            fun_get_employee_name(a.charge_doc_code) doctorname,
                            (select b.diagnose from met_ops_apply b where a.inpatient_no=b.clinic_code and rownum = 1) operation,--手术名称
                            (select b.pre_date from met_ops_apply b where a.inpatient_no=b.clinic_code and  rownum = 1) operationtime,--手术时间
                            fun_get_dept_name(a.dept_code) inpatientarea,--病人所属科室
                            null remark  --备注
                             from fin_ipr_inmaininfo a
                             where a.patient_no='{0}'
                             and rownum=1
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, GetDisinfectionInpInfo.HOSPITALNUMBER);

                System.Data.DataTable dt = new System.Data.DataTable();
                //住院患者信息查询
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GetDisinfectionInpInfo = new His.Models.CSSD.InDisinfectionInfo();
                    GetDisinfectionInpInfo.HOSPITALNUMBER = dt.Rows[i][0].ToString();
                    GetDisinfectionInpInfo.SERIALNUMBER = dt.Rows[i][1].ToString();
                    GetDisinfectionInpInfo.PATIENTNAME = dt.Rows[i][2].ToString();
                    GetDisinfectionInpInfo.PATIENTAGE = dt.Rows[i][3].ToString();
                    GetDisinfectionInpInfo.PATIENTSEX = dt.Rows[i][4].ToString();
                    GetDisinfectionInpInfo.PATIENTIDCARD = dt.Rows[i][5].ToString();
                    GetDisinfectionInpInfo.DOCTORNAME = dt.Rows[i][6].ToString();
                    GetDisinfectionInpInfo.OPERATION = dt.Rows[i][7].ToString();
                    GetDisinfectionInpInfo.OPERATIONTIME = dt.Rows[i][8].ToString();
                    GetDisinfectionInpInfo.INPATIENTAREA = dt.Rows[i][9].ToString();
                    GetDisinfectionInpInfo.REMARK = dt.Rows[i][10].ToString();
                    al.Add(GetDisinfectionInpInfo);
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

        private string GetDisinfectionInpInfoXML(System.Collections.ArrayList al)
        {

            #region
            if (al == null || al.Count == 0)
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

                System.Xml.XmlElement OpTime = xml.CreateElement("OpTime");
                OpTime.InnerText = "";
                root1.AppendChild(OpTime);

                System.Xml.XmlElement FunCode = xml.CreateElement("FunCode");
                FunCode.InnerText = "";
                root1.AppendChild(FunCode);


                //His.Models.ZZSB.GetGuideListInfoForSRM p = al[0] as His.Models.ZZSB.GetGuideListInfoForSRM;
                foreach (His.Models.CSSD.InDisinfectionInfo p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CLINICNUMBER == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement HOSPITALNUMBER = xml.CreateElement("HOSPITALNUMBER");
                    HOSPITALNUMBER.InnerText = p.HOSPITALNUMBER;
                    Result.AppendChild(HOSPITALNUMBER);

                    System.Xml.XmlElement SERIALNUMBER = xml.CreateElement("SERIALNUMBER");
                    SERIALNUMBER.InnerText = p.SERIALNUMBER;
                    Result.AppendChild(SERIALNUMBER);

                    System.Xml.XmlElement PATIENTNAME = xml.CreateElement("PATIENTNAME");
                    PATIENTNAME.InnerText = p.PATIENTNAME;
                    Result.AppendChild(PATIENTNAME);

                    System.Xml.XmlElement PATIENTAGE = xml.CreateElement("PATIENTAGE");
                    PATIENTAGE.InnerText = p.PATIENTAGE;
                    Result.AppendChild(PATIENTAGE);

                    System.Xml.XmlElement PATIENTSEX = xml.CreateElement("PATIENTSEX");
                    PATIENTSEX.InnerText = p.PATIENTSEX;
                    Result.AppendChild(PATIENTSEX);

                    System.Xml.XmlElement PATIENTIDCARD = xml.CreateElement("PATIENTIDCARD");
                    PATIENTIDCARD.InnerText = p.PATIENTIDCARD;
                    Result.AppendChild(PATIENTIDCARD);

                    System.Xml.XmlElement DOCTORNAME = xml.CreateElement("DOCTORNAME");
                    DOCTORNAME.InnerText = p.DOCTORNAME;
                    Result.AppendChild(DOCTORNAME);

                    System.Xml.XmlElement OPERATION = xml.CreateElement("OPERATION");
                    OPERATION.InnerText = p.OPERATION;
                    Result.AppendChild(OPERATION);

                    System.Xml.XmlElement OPERATIONTIME = xml.CreateElement("OPERATIONTIME");
                    OPERATIONTIME.InnerText = p.OPERATIONTIME;
                    Result.AppendChild(OPERATIONTIME);

                    System.Xml.XmlElement INPATIENTAREA = xml.CreateElement("INPATIENTAREA");
                    INPATIENTAREA.InnerText = p.INPATIENTAREA;
                    Result.AppendChild(INPATIENTAREA);

                    System.Xml.XmlElement REMARK = xml.CreateElement("REMARK");
                    REMARK.InnerText = p.REMARK;
                    Result.AppendChild(REMARK);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetDisinfectionInpInfoModel(string xml, ref His.Models.CSSD.InDisinfectionInfo opa)
        {

            string returnStr = "";
            opa = new His.Models.CSSD.InDisinfectionInfo();
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


            System.Xml.XmlNodeList HOSPITALNUMBER1 = doc.GetElementsByTagName("hospitalNumber");
            System.Xml.XmlNode HOSPITALNUMBER = HOSPITALNUMBER1[0];
            if (!string.IsNullOrEmpty(HOSPITALNUMBER.InnerText))
            {
                opa.HOSPITALNUMBER = HOSPITALNUMBER.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "住院流水号不能为空！";
                return this.ReturnFailure();
            }


            return returnStr;
        }



        public string GetDisinfectionInpInfoForCSSD(string xml)
        {
            string returnStr = "";
            His.Models.CSSD.InDisinfectionInfo opa = new His.Models.CSSD.InDisinfectionInfo();
            returnStr = this.GetDisinfectionInpInfoModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetDisinfectionInpInfoDate(opa);
            returnStr = this.GetDisinfectionInpInfoXML(al);
            return returnStr;
        }
    }
}