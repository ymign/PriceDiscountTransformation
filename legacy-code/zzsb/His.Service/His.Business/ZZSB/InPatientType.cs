using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.ZZSB
{
    public class InPatientType
    
    {

        private int ZZSBBarcodeOutPrintNotificationData(His.Models.ZZSB.OutPatientType inpatientType, ref string err)
        {
            #region sql
            string sql = @" 
         update fin_opb_feedetail t
     set t.ext_flag3 = '{1}'
   where t.invoice_no ='{0}'
     and t.card_no ='{2}'     
        ";
            sql = string.Format(sql, inpatientType.INVOICENO,inpatientType.ISPRINTABLE,inpatientType.PATIENTID);
            #endregion
            try
            {
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                {
                    return -1;
                }

                try
                {
                    string sql2 = @"insert into prc_com_log
                                     values('{0}',
                                     '{1}',
                                    '{2}',
                                     sysdate)";
                    sql2 = string.Format(sql2, inpatientType.PATIENTID, "自助设备打印",
                        inpatientType.INVOICENO+"||"+ inpatientType.ISPRINTABLE);
                    DataBaseHelp.DataExecHelp.ExecSql(sql2, ref err);
                }

                catch { }
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 更新打印状态
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private His.Models.ZZSB.OutPatientType GetInPatientModel(string xml)
        {
            His.Models.ZZSB.OutPatientType opa = new His.Models.ZZSB.OutPatientType();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList INVOICENO1 = doc.GetElementsByTagName("InvoiceNo");
            System.Xml.XmlNode INVOICENO = INVOICENO1[0];
            if (!string.IsNullOrEmpty(INVOICENO.InnerText))
            {
                opa.INVOICENO = INVOICENO.InnerText;
            }
            else
            {
                opa.INVOICENO = "ALL";
            }

            System.Xml.XmlNodeList ISPRINTABLE1 = doc.GetElementsByTagName("IsPrintable");
            System.Xml.XmlNode ISPRINTABLE = ISPRINTABLE1[0];
            if (!string.IsNullOrEmpty(ISPRINTABLE.InnerText))
            {
                opa.ISPRINTABLE = ISPRINTABLE.InnerText;
            }
            else
            {
                opa.ISPRINTABLE = "ALL";
            }

            System.Xml.XmlNodeList PATIENTID1 = doc.GetElementsByTagName("PatientID");
            System.Xml.XmlNode PATIENTID = PATIENTID1[0];
            if (!string.IsNullOrEmpty(PATIENTID.InnerText))
            {
                opa.PATIENTID = PATIENTID.InnerText;
            }
            else
            {
                opa.PATIENTID = "ALL";
            }
            return opa;
        }

        /// <summary>
        /// 获取返回信息
        /// </summary>
        /// <param name="i"></param>
        /// <param name="err"></param>
        private void GetLisReturnResult(int i, ref string message)
        {
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            System.Xml.XmlElement root = xml.CreateElement("DataSource");
            xml.AppendChild(root);

            System.Xml.XmlElement root1 = xml.CreateElement("return");
            root.AppendChild(root1);

            if (i == 1)
            {
                System.Xml.XmlElement Code = xml.CreateElement("Code");
                Code.InnerText = "1";
                root1.AppendChild(Code);

                System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
                root1.AppendChild(ErrorMsg);
            }
            else
            {
                System.Xml.XmlElement Code = xml.CreateElement("Code");
                Code.InnerText = "0";
                root1.AppendChild(Code);

                System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
                ErrorMsg.InnerText = message;
                root1.AppendChild(ErrorMsg);
            }

            System.Xml.XmlElement Result = xml.CreateElement("Result");
            root1.AppendChild(Result);

            message = xml.InnerXml.ToString();
        }

        /// <summary>
        /// 更新条码打印状态
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        public string ZZSBBarcodePrintNotification(string xml)
        {
            int i = -1;
            string err = "";
            His.Models.ZZSB.OutPatientType ipa = new His.Models.ZZSB.OutPatientType();
            ipa = this.GetInPatientModel(xml);
            if (ipa.ISPRINTABLE == "1")
            {
                i = this.ZZSBBarcodeOutPrintNotificationData(ipa, ref err);
            }
            else
            {
                i = -1;
                err = "传入参数有误，请核实";
            }
            this.GetLisReturnResult(i, ref err);
            return err;
        }

    }
}