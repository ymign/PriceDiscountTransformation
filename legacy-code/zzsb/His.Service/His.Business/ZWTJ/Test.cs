using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.FrameWork;
using Neusoft.FrameWork.Function;
using System.Data;

namespace His.Business.ZWTJ
{
    public class PERegisterInfo
    {

        private int TjPatientNoData(His.Models.ZWTJ.OutPERegisterInfo PERegisterInfo, ref string err, ref string clinicno)
        {
            #region sql
            string sql = @" 
        INSERT INTO FIN_OPR_REGISTER --挂号主表
      (CLINIC_CODE, --门诊号/发票号
       CARD_NO, --就诊卡号
       REG_DATE, --挂号日期
       NOON_CODE, --午别
       NAME, --姓名
       IDENNO, --身份证号
       SEX_CODE, --性别
       BIRTHDAY, --出生日
       PAYKIND_CODE, --结算类别号
       PAYKIND_NAME, --结算类别名称
       PACT_CODE, --合同号
       PACT_NAME, --合同单位名称
       MCARD_NO, --医疗证号
       REGLEVL_CODE, --挂号级别
       REGLEVL_NAME, --挂号级别名称
       DEPT_CODE, --科室号
       DEPT_NAME, --科室名称
       SEENO, --看诊序号
       DOCT_CODE, --医师代号
       DOCT_NAME, --医师姓名
       SEE_DATE, --看诊日期
       YNREGCHRG, --挂号收费标志
       YNBOOK, --是否预约
       YNFR, --1初诊/2复诊
       REG_FEE, --挂号费
       CHCK_FEE, --检查费
       DIAG_FEE, --诊察费
       OTH_FEE, --附加费
       OWN_COST, --自费金额
       PUB_COST, --报销金额
       PAY_COST, --自付金额
       VALID_FLAG, --退号标志
       OPER_CODE, --操作员代码
       YNSEE, --是否看诊
       CHECK_FLAG, --1未核查/2已核查
       RELA_PHONE, --联系电话
       ADDRESS, --地址
       TRANS_TYPE, --交易类型
       CARD_TYPE, --证件类型
       BEGIN_TIME, --开始时间段
       END_TIME, --结束时间段
       CANCEL_OPCD, --作废人
       CANCEL_DATE, --作废时间
       INVOICE_NO,
       RECIPE_NO,
       APPEND_FLAG,
       ORDER_NO,
       SCHEMA_NO,
       OPER_DATE, --操作时间
       IN_SOURCE,
       IS_SENDINHOSCASE,
       IS_ENCRYPTNAME,
       NORMALNAME,
       ECO_COST,
       IS_ACCOUNT,
       HOS_CODE)
    VALUES
      (SEQ_FIN_CLINICNO.NEXTVAL, --门诊号/发票号
       --SELECT SEQ_FIN_CLINICNO.NEXTVAL FROM DUAL
       '{0}', --就诊卡号
       SYSDATE, --挂号日期
       '', --午别
       '{1}', --姓名2
       '{3}', --身份证号3
       '{2}', --性别4
       TO_DATE('{4}', 'yyyy-mm-dd'), --出生日 5
       '01', --结算类别号
       '', --结算类别名称
       '1', --合同号
       '普通', --合同单位名称
       '', --医疗证号
       '1', --挂号级别
       '普通', --挂号级别名称
       '{7}', --科室号  6
       '{8}', --科室名称 7
       '-1', --看诊序号
       '', --医师代号
       '', --医师姓名
       NULL, --看诊日期
       '0', --挂号收费标志
       '0', --是否预约
       '1', --1初诊/2复诊
       '0', --挂号费
       '0', --检查费
       '0', --诊察费
       '0', --附加费
       '0', --自费金额
       '0', --报销金额
       '0', --自付金额
       '1', --有效标志
       '{9}', --操作员代码 8
       '0', --是否看诊
       '0', --1未核查/2已核查
       '{5}', --联系电话
       '{6}', --地址 10
       '1', --交易类型
       '', --证件类型
       TO_DATE('0001-01-01 00:00:00', 'yyyy-mm-dd HH24:mi:ss'), --开始时间
       TO_DATE('0001-01-01 00:00:00', 'yyyy-mm-dd HH24:mi:ss'), --开始时间
       '', --作废人
       TO_DATE('0001-01-01 00:00:00', 'yyyy-mm-dd hh24:mi:ss'),
       '',
       '',
       '0',
       '0',
       '',
       SYSDATE,--操作时间
       '',
       '0',
       '0',
       '',
       '0',
       '0',
       'CORE_HIS50')     
        ";
            sql = string.Format(sql, PERegisterInfo.par_card_no.PadLeft(10, '0'), PERegisterInfo.par_name, PERegisterInfo.par_sex_code, PERegisterInfo.par_idenno, PERegisterInfo.par_birthday, PERegisterInfo.par_rela_phone, PERegisterInfo.par_address, PERegisterInfo.par_dept_code, PERegisterInfo.par_dept_name, PERegisterInfo.par_oper_code);
            #endregion
            try
            {
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                {
                    return -1;
                }

                try
                {
                    string sql2 = @"INSERT INTO com_patientinfo --挂号主表
                                  (CARD_NO, --就诊卡号
                                   NAME, --姓名
                                   IDENNO, --身份证号
                                   SEX_CODE, --性别
                                   BIRTHDAY, --出生日
                                   PAYKIND_CODE, --结算类别号
                                   PAYKIND_NAME, --结算类别名称
                                   PACT_CODE, --合同号
                                   PACT_NAME, --合同单位名称
                                   MARK, --备注
                                   OPER_CODE, --操作员代码
                                   OPER_DATE, --操作时间
                                   IS_VALID, --有效标志
                                   PAR_TJLX,
                                   HOME_TEL
                                   )
                                VALUES
                                  (
                                   '{0}', --就诊卡号
                                   '{1}', --姓名2
                                   '{3}', --身份证号3
                                   '{2}', --性别
                                   TO_DATE('{4}', 'yyyy-mm-dd'), --出生日 5
                                   '01', --结算类别号
                                   '', --结算类别名称
                                   '1', --合同号
                                   '普通', --合同单位名称
                                   '体检挂号',
                                   '{5}',
                                   sysdate,
                                  '1',
                                  '{6}',
                                  '{7}'   )";
                    //string card_no = PERegisterInfo.par_card_no.PadLeft(10, '0');
                    sql2 = string.Format(sql2, PERegisterInfo.par_card_no.PadLeft(10, '0'), PERegisterInfo.par_name, PERegisterInfo.par_sex_code, PERegisterInfo.par_idenno, PERegisterInfo.par_birthday, PERegisterInfo.par_oper_code, PERegisterInfo.par_tjlx, PERegisterInfo.par_rela_phone);
                    //sql2 = string.Format(sql2, card_no, PERegisterInfo.par_name, PERegisterInfo.par_sex_code, PERegisterInfo.par_idenno, PERegisterInfo.par_birthday, PERegisterInfo.par_oper_code);
                    if (!DataBaseHelp.DataExecHelp.ExecSql(sql2, ref err))
                    {
                        err = string.Empty;
                        string sql4 = @"
                            update com_patientinfo
                                  set NAME = '{1}',
                                  SEX_CODE = '{2}', 
                                  IDENNO = '{3}'
                                where CARD_NO = '{0}'";
                        sql4 = string.Format(sql4, PERegisterInfo.par_card_no.PadLeft(10, '0'), PERegisterInfo.par_name, PERegisterInfo.par_sex_code, PERegisterInfo.par_idenno);
                        DataBaseHelp.DataExecHelp.ExecSql(sql4, ref err);
                    }

                    string sql6 = @"INSERT INTO fin_opb_accountcard --挂号主表
                                  (CARD_NO, --就诊卡号
                                   MARKNO, --姓名
                                   TYPE, --身份证号
                                   STATE, --状态
                                   REFLAG, 
                                   CREATEOPER, --操作人             
                                   CREATEDATE --操作时间
                                   )
                                VALUES
                                  (
                                   '{0}', --就诊卡号
                                   '{0}', --卡号
                                   'Card_No', --卡类型
                                   '1', --卡状态
                                   '0', --出生日 5
                                   '009999', --结算类别号
                                   sysdate)";
                    sql6 = string.Format(sql6, PERegisterInfo.par_card_no.PadLeft(10, '0'));
                    if (!DataBaseHelp.DataExecHelp.ExecSql(sql6, ref err))
                    {
                        err = string.Empty;
                        string sql7 = @"
                            update fin_opb_accountcard
                                  set CREATEDATE = sysdate
                                where CARD_NO = '{0}'";
                        sql7 = string.Format(sql7, PERegisterInfo.par_card_no.PadLeft(10, '0'));
                        DataBaseHelp.DataExecHelp.ExecSql(sql7, ref err);
                    }

                    string sql3 = @"select CLINIC_CODE from FIN_OPR_REGISTER where CARD_NO='{0}' order by REG_DATE desc";
                    sql3 = string.Format(sql3, PERegisterInfo.par_card_no.PadLeft(10, '0'));
                    DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql3);
                    if (dt == null || dt.Rows.Count < 0)
                    {
                        clinicno = "";
                        return -1;
                    }
                    else
                    {
                        clinicno = dt.Rows[0][0].ToString();
                    }
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
        /// 体检挂号
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private His.Models.ZWTJ.OutPERegisterInfo GetInPatientModel(string xml)
        {
            His.Models.ZWTJ.OutPERegisterInfo opa = new His.Models.ZWTJ.OutPERegisterInfo();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList par_card_no1 = doc.GetElementsByTagName("PAR_CARD_NO");
            System.Xml.XmlNode par_card_no = par_card_no1[0];
            if (!string.IsNullOrEmpty(par_card_no.InnerText))
            {
                opa.par_card_no = par_card_no.InnerText;
            }
            else
            {
                opa.par_card_no = "ALL";
            }

            System.Xml.XmlNodeList par_name1 = doc.GetElementsByTagName("PAR_NAME");
            System.Xml.XmlNode par_name = par_name1[0];
            if (!string.IsNullOrEmpty(par_name.InnerText))
            {
                opa.par_name = par_name.InnerText;
            }
            else
            {
                opa.par_name = "ALL";
            }

            System.Xml.XmlNodeList par_sex_code1 = doc.GetElementsByTagName("PAR_SEX_CODE");
            System.Xml.XmlNode par_sex_code = par_sex_code1[0];
            if (!string.IsNullOrEmpty(par_sex_code.InnerText))
            {
                opa.par_sex_code = par_sex_code.InnerText;
            }
            else
            {
                opa.par_sex_code = "ALL";
            }

            System.Xml.XmlNodeList par_idenno1 = doc.GetElementsByTagName("PAR_IDENNO");
            System.Xml.XmlNode par_idenno = par_idenno1[0];
            if (!string.IsNullOrEmpty(par_idenno.InnerText))
            {
                opa.par_idenno = par_idenno.InnerText;
            }
            else
            {
                opa.par_idenno = "ALL";
            }

            System.Xml.XmlNodeList par_birthday1 = doc.GetElementsByTagName("PAR_BIRTHDAY");
            System.Xml.XmlNode par_birthday = par_birthday1[0];
            if (!string.IsNullOrEmpty(par_birthday.InnerText))
            {
                opa.par_birthday = par_birthday.InnerText;
            }
            else
            {
                opa.par_birthday = "ALL";
            }

            System.Xml.XmlNodeList par_rela_phone1 = doc.GetElementsByTagName("PAR_RELA_PHONE");
            System.Xml.XmlNode par_rela_phone = par_rela_phone1[0];
            if (!string.IsNullOrEmpty(par_rela_phone.InnerText))
            {
                opa.par_rela_phone = par_rela_phone.InnerText;
            }
            else
            {
                opa.par_rela_phone = "ALL";
            }

            System.Xml.XmlNodeList par_address1 = doc.GetElementsByTagName("PAR_ADDRESS");
            System.Xml.XmlNode par_address = par_address1[0];
            if (!string.IsNullOrEmpty(par_address.InnerText))
            {
                opa.par_address = par_address.InnerText;
            }
            else
            {
                opa.par_address = "ALL";
            }

            System.Xml.XmlNodeList par_dept_code1 = doc.GetElementsByTagName("PAR_DEPT_CODE");
            System.Xml.XmlNode par_dept_code = par_dept_code1[0];
            if (!string.IsNullOrEmpty(par_dept_code.InnerText))
            {
                opa.par_dept_code = par_dept_code.InnerText;
            }
            else
            {
                opa.par_dept_code = "ALL";
            }

            System.Xml.XmlNodeList par_dept_name1 = doc.GetElementsByTagName("PAR_DEPT_NAME");
            System.Xml.XmlNode par_dept_name = par_dept_name1[0];
            if (!string.IsNullOrEmpty(par_dept_name.InnerText))
            {
                opa.par_dept_name = par_dept_name.InnerText;
            }
            else
            {
                opa.par_dept_name = "ALL";
            }

            System.Xml.XmlNodeList par_oper_code1 = doc.GetElementsByTagName("PAR_OPER_CODE");
            System.Xml.XmlNode par_oper_code = par_oper_code1[0];
            if (!string.IsNullOrEmpty(par_oper_code.InnerText))
            {
                opa.par_oper_code = par_oper_code.InnerText;
            }
            else
            {
                opa.par_oper_code = "ALL";
            }
            System.Xml.XmlNodeList par_tjlx1 = doc.GetElementsByTagName("PAR_TJLX");
            System.Xml.XmlNode par_tjlx = par_tjlx1[0];
            if (!string.IsNullOrEmpty(par_tjlx.InnerText))
            {
                opa.par_tjlx = par_tjlx.InnerText;
            }
            else
            {
                opa.par_tjlx = "ALL";
            }

            return opa;
        }

        /// <summary>
        /// 获取返回信息
        /// </summary>
        /// <param name="i"></param>
        /// <param name="err"></param>
        private string GetLisReturnResult(int i, ref string message, ref string clinicno)
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

                System.Xml.XmlElement CLINIC_CODE = xml.CreateElement("CLINIC_CODE");
                CLINIC_CODE.InnerText = clinicno;
                root1.AppendChild(CLINIC_CODE);
            }

            else
            {
                System.Xml.XmlElement Code = xml.CreateElement("Code");
                Code.InnerText = "0";
                root1.AppendChild(Code);

                System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
                ErrorMsg.InnerText = message;
                root1.AppendChild(ErrorMsg);

                System.Xml.XmlElement CLINIC_CODE = xml.CreateElement("CLINIC_CODE");
                CLINIC_CODE.InnerText = clinicno;
                root1.AppendChild(CLINIC_CODE);
            }

            System.Xml.XmlElement Result = xml.CreateElement("Result");
            root1.AppendChild(Result);

            return xml.InnerXml.ToString();
        }

        /// <summary>
        /// 体检挂号
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        public string TjPatientNo(string xml)
        {
            int i = -1;
            string err = "";
            string clinicno = "";
            His.Models.ZWTJ.OutPERegisterInfo ipa = new His.Models.ZWTJ.OutPERegisterInfo();
            ipa = this.GetInPatientModel(xml);
            if (ipa.par_card_no != null)
            {
                i = this.TjPatientNoData(ipa, ref err, ref clinicno);
            }
            else
            {
                i = -1;
                err = "输入卡号不能为空，请核实";
            }
            return this.GetLisReturnResult(i, ref err, ref clinicno);
        }
    }

    public class PERegisterInfoxt
    {

        private int TjPatientNoData(His.Models.ZWTJ.OutPERegisterInfo PERegisterInfoxt, ref string err, ref string clinicno)
        {
            #region sql
            string sql = @" 
        INSERT INTO FIN_OPR_REGISTER --挂号主表
      (CLINIC_CODE, --门诊号/发票号
       CARD_NO, --就诊卡号
       REG_DATE, --挂号日期
       NOON_CODE, --午别
       NAME, --姓名
       IDENNO, --身份证号
       SEX_CODE, --性别
       BIRTHDAY, --出生日
       PAYKIND_CODE, --结算类别号
       PAYKIND_NAME, --结算类别名称
       PACT_CODE, --合同号
       PACT_NAME, --合同单位名称
       MCARD_NO, --医疗证号
       REGLEVL_CODE, --挂号级别
       REGLEVL_NAME, --挂号级别名称
       DEPT_CODE, --科室号
       DEPT_NAME, --科室名称
       SEENO, --看诊序号
       DOCT_CODE, --医师代号
       DOCT_NAME, --医师姓名
       SEE_DATE, --看诊日期
       YNREGCHRG, --挂号收费标志
       YNBOOK, --是否预约
       YNFR, --1初诊/2复诊
       REG_FEE, --挂号费
       CHCK_FEE, --检查费
       DIAG_FEE, --诊察费
       OTH_FEE, --附加费
       OWN_COST, --自费金额
       PUB_COST, --报销金额
       PAY_COST, --自付金额
       VALID_FLAG, --退号标志
       OPER_CODE, --操作员代码
       YNSEE, --是否看诊
       CHECK_FLAG, --1未核查/2已核查
       RELA_PHONE, --联系电话
       ADDRESS, --地址
       TRANS_TYPE, --交易类型
       CARD_TYPE, --证件类型
       BEGIN_TIME, --开始时间段
       END_TIME, --结束时间段
       CANCEL_OPCD, --作废人
       CANCEL_DATE, --作废时间
       INVOICE_NO,
       RECIPE_NO,
       APPEND_FLAG,
       ORDER_NO,
       SCHEMA_NO,
       OPER_DATE, --操作时间
       IN_SOURCE, --自助设备 4为血透 5为儿保
       IS_SENDINHOSCASE,
       IS_ENCRYPTNAME,
       NORMALNAME,
       ECO_COST,
       IS_ACCOUNT,
       HOS_CODE)
    VALUES
      (SEQ_FIN_CLINICNO.NEXTVAL, --门诊号/发票号
       --SELECT SEQ_FIN_CLINICNO.NEXTVAL FROM DUAL
       '{0}', --就诊卡号
       SYSDATE, --挂号日期
       '', --午别
       '{1}', --姓名2
       '{3}', --身份证号3
       '{2}', --性别4
       TO_DATE('{4}', 'yyyy-mm-dd'), --出生日 5
       '01', --结算类别号
       '', --结算类别名称
       '1', --合同号
       '普通', --合同单位名称
       '', --医疗证号
       '1', --挂号级别
       '普通', --挂号级别名称
       '{7}', --科室号  6
       '{8}', --科室名称 7
       '-1', --看诊序号
       '', --医师代号
       '', --医师姓名
       NULL, --看诊日期
       '0', --挂号收费标志
       '0', --是否预约
       '1', --1初诊/2复诊
       '0', --挂号费
       '0', --检查费
       '0', --诊察费
       '0', --附加费
       '{11}', --自费金额
       '{12}', --报销金额
       '0', --自付金额
       '1', --有效标志
       '{9}', --操作员代码 8
       '0', --是否看诊
       '0', --1未核查/2已核查
       '{5}', --联系电话
       '{6}', --地址 10
       '1', --交易类型
       '', --证件类型
       TO_DATE('0001-01-01 00:00:00', 'yyyy-mm-dd HH24:mi:ss'), --开始时间
       TO_DATE('0001-01-01 00:00:00', 'yyyy-mm-dd HH24:mi:ss'), --开始时间
       '', --作废人
       TO_DATE('0001-01-01 00:00:00', 'yyyy-mm-dd hh24:mi:ss'),
       '',
       '',
       '0',
       '0',
       '',
       SYSDATE,--操作时间
       '{10}',
       '0',
       '0',
       '',
       '0',
       '0',
       'CORE_HIS50')     
        ";
            sql = string.Format(sql, PERegisterInfoxt.par_card_no.PadLeft(10, '0'), PERegisterInfoxt.par_name, PERegisterInfoxt.par_sex_code, PERegisterInfoxt.par_idenno, PERegisterInfoxt.par_birthday, PERegisterInfoxt.par_rela_phone, PERegisterInfoxt.par_address, PERegisterInfoxt.par_dept_code, PERegisterInfoxt.par_dept_name, PERegisterInfoxt.par_oper_code, PERegisterInfoxt.par_in_source, PERegisterInfoxt.par_own_cost, PERegisterInfoxt.par_pub_cost);
            #endregion
            try
            {
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                {
                    return -1;
                }

                try
                {
                    string sql2 = @"INSERT INTO com_patientinfo --挂号主表
                                  (CARD_NO, --就诊卡号
                                   NAME, --姓名
                                   IDENNO, --身份证号
                                   SEX_CODE, --性别
                                   BIRTHDAY, --出生日
                                   PAYKIND_CODE, --结算类别号
                                   PAYKIND_NAME, --结算类别名称
                                   PACT_CODE, --合同号
                                   PACT_NAME, --合同单位名称
                                   MARK, --备注
                                   OPER_CODE, --操作员代码
                                   OPER_DATE, --操作时间
                                   IS_VALID --有效标志
                                   )
                                VALUES
                                  (
                                   '{0}', --就诊卡号
                                   '{1}', --姓名2
                                   '{3}', --身份证号3
                                   '{2}', --性别
                                   TO_DATE('{4}', 'yyyy-mm-dd'), --出生日 5
                                   '01', --结算类别号
                                   '', --结算类别名称
                                   '1', --合同号
                                   '普通', --合同单位名称
                                   '体检挂号',
                                   '{5}',
                                   sysdate,
                                   '1')";
                    //string card_no = PERegisterInfo.par_card_no.PadLeft(10, '0');
                    sql2 = string.Format(sql2, PERegisterInfoxt.par_card_no.PadLeft(10, '0'), PERegisterInfoxt.par_name, PERegisterInfoxt.par_sex_code, PERegisterInfoxt.par_idenno, PERegisterInfoxt.par_birthday, PERegisterInfoxt.par_oper_code);
                    //sql2 = string.Format(sql2, card_no, PERegisterInfo.par_name, PERegisterInfo.par_sex_code, PERegisterInfo.par_idenno, PERegisterInfo.par_birthday, PERegisterInfo.par_oper_code);
                    if (!DataBaseHelp.DataExecHelp.ExecSql(sql2, ref err))
                    {
                        err = string.Empty;
                        string sql4 = @"
                            update com_patientinfo
                                  set NAME = '{1}',
                                  SEX_CODE = '{2}', 
                                  IDENNO = '{3}'
                                where CARD_NO = '{0}'";
                        sql4 = string.Format(sql4, PERegisterInfoxt.par_card_no.PadLeft(10, '0'), PERegisterInfoxt.par_name, PERegisterInfoxt.par_sex_code, PERegisterInfoxt.par_idenno);
                        DataBaseHelp.DataExecHelp.ExecSql(sql4, ref err);
                    }

                    string sql6 = @"INSERT INTO fin_opb_accountcard --挂号主表
                                  (CARD_NO, --就诊卡号
                                   MARKNO, --姓名
                                   TYPE, --身份证号
                                   STATE, --状态
                                   REFLAG, 
                                   CREATEOPER, --操作人             
                                   CREATEDATE --操作时间
                                   )
                                VALUES
                                  (
                                   '{0}', --就诊卡号
                                   '{0}', --卡号
                                   'Card_No', --卡类型
                                   '1', --卡状态
                                   '0', --出生日 5
                                   '009999', --结算类别号
                                   sysdate)";
                    sql6 = string.Format(sql6, PERegisterInfoxt.par_card_no.PadLeft(10, '0'));
                    if (!DataBaseHelp.DataExecHelp.ExecSql(sql6, ref err))
                    {
                        err = string.Empty;
                        string sql7 = @"
                            update fin_opb_accountcard
                                  set CREATEDATE = sysdate
                                where CARD_NO = '{0}'";
                        sql7 = string.Format(sql7, PERegisterInfoxt.par_card_no.PadLeft(10, '0'));
                        DataBaseHelp.DataExecHelp.ExecSql(sql7, ref err);
                    }

                    string sql3 = @"select CLINIC_CODE from FIN_OPR_REGISTER where CARD_NO='{0}' order by REG_DATE desc";
                    sql3 = string.Format(sql3, PERegisterInfoxt.par_card_no.PadLeft(10, '0'));
                    DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql3);
                    if (dt == null || dt.Rows.Count < 0)
                    {
                        clinicno = "";
                        return -1;
                    }
                    else
                    {
                        clinicno = dt.Rows[0][0].ToString();
                    }
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
        /// 血透挂号
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private His.Models.ZWTJ.OutPERegisterInfo GetInPatientModel(string xml)
        {
            His.Models.ZWTJ.OutPERegisterInfo opa = new His.Models.ZWTJ.OutPERegisterInfo();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList par_card_no1 = doc.GetElementsByTagName("PAR_CARD_NO");
            System.Xml.XmlNode par_card_no = par_card_no1[0];
            if (!string.IsNullOrEmpty(par_card_no.InnerText))
            {
                opa.par_card_no = par_card_no.InnerText;
            }
            else
            {
                opa.par_card_no = "ALL";
            }

            System.Xml.XmlNodeList par_name1 = doc.GetElementsByTagName("PAR_NAME");
            System.Xml.XmlNode par_name = par_name1[0];
            if (!string.IsNullOrEmpty(par_name.InnerText))
            {
                opa.par_name = par_name.InnerText;
            }
            else
            {
                opa.par_name = "ALL";
            }

            System.Xml.XmlNodeList par_sex_code1 = doc.GetElementsByTagName("PAR_SEX_CODE");
            System.Xml.XmlNode par_sex_code = par_sex_code1[0];
            if (!string.IsNullOrEmpty(par_sex_code.InnerText))
            {
                opa.par_sex_code = par_sex_code.InnerText;
            }
            else
            {
                opa.par_sex_code = "ALL";
            }

            System.Xml.XmlNodeList par_idenno1 = doc.GetElementsByTagName("PAR_IDENNO");
            System.Xml.XmlNode par_idenno = par_idenno1[0];
            if (!string.IsNullOrEmpty(par_idenno.InnerText))
            {
                opa.par_idenno = par_idenno.InnerText;
            }
            else
            {
                opa.par_idenno = "ALL";
            }

            System.Xml.XmlNodeList par_birthday1 = doc.GetElementsByTagName("PAR_BIRTHDAY");
            System.Xml.XmlNode par_birthday = par_birthday1[0];
            if (!string.IsNullOrEmpty(par_birthday.InnerText))
            {
                opa.par_birthday = par_birthday.InnerText;
            }
            else
            {
                opa.par_birthday = "ALL";
            }

            System.Xml.XmlNodeList par_rela_phone1 = doc.GetElementsByTagName("PAR_RELA_PHONE");
            System.Xml.XmlNode par_rela_phone = par_rela_phone1[0];
            if (!string.IsNullOrEmpty(par_rela_phone.InnerText))
            {
                opa.par_rela_phone = par_rela_phone.InnerText;
            }
            else
            {
                opa.par_rela_phone = "ALL";
            }

            System.Xml.XmlNodeList par_address1 = doc.GetElementsByTagName("PAR_ADDRESS");
            System.Xml.XmlNode par_address = par_address1[0];
            if (!string.IsNullOrEmpty(par_address.InnerText))
            {
                opa.par_address = par_address.InnerText;
            }
            else
            {
                opa.par_address = "ALL";
            }

            System.Xml.XmlNodeList par_dept_code1 = doc.GetElementsByTagName("PAR_DEPT_CODE");
            System.Xml.XmlNode par_dept_code = par_dept_code1[0];
            if (!string.IsNullOrEmpty(par_dept_code.InnerText))
            {
                opa.par_dept_code = par_dept_code.InnerText;
            }
            else
            {
                opa.par_dept_code = "ALL";
            }

            System.Xml.XmlNodeList par_dept_name1 = doc.GetElementsByTagName("PAR_DEPT_NAME");
            System.Xml.XmlNode par_dept_name = par_dept_name1[0];
            if (!string.IsNullOrEmpty(par_dept_name.InnerText))
            {
                opa.par_dept_name = par_dept_name.InnerText;
            }
            else
            {
                opa.par_dept_name = "ALL";
            }

            System.Xml.XmlNodeList par_oper_code1 = doc.GetElementsByTagName("PAR_OPER_CODE");
            System.Xml.XmlNode par_oper_code = par_oper_code1[0];
            if (!string.IsNullOrEmpty(par_oper_code.InnerText))
            {
                opa.par_oper_code = par_oper_code.InnerText;
            }
            else
            {
                opa.par_oper_code = "ALL";
            }

            System.Xml.XmlNodeList par_in_source1 = doc.GetElementsByTagName("PAR_IN_SOURCE");
            System.Xml.XmlNode par_in_source = par_in_source1[0];
            if (!string.IsNullOrEmpty(par_in_source.InnerText))
            {
                opa.par_in_source = par_in_source.InnerText;
            }
            else
            {
                opa.par_in_source = "ALL";
            }

            System.Xml.XmlNodeList par_own_cost1 = doc.GetElementsByTagName("PAR_OWN_COST");
            System.Xml.XmlNode par_own_cost = par_own_cost1[0];
            if (!string.IsNullOrEmpty(par_own_cost.InnerText))
            {
                opa.par_own_cost = par_own_cost.InnerText;
            }
            else
            {
                opa.par_own_cost = "ALL";
            }

            System.Xml.XmlNodeList par_pub_cost1 = doc.GetElementsByTagName("PAR_PUB_COST");
            System.Xml.XmlNode par_pub_cost = par_pub_cost1[0];
            if (!string.IsNullOrEmpty(par_pub_cost.InnerText))
            {
                opa.par_pub_cost = par_pub_cost.InnerText;
            }
            else
            {
                opa.par_pub_cost = "ALL";
            }

            return opa;
        }

        /// <summary>
        /// 获取返回信息
        /// </summary>
        /// <param name="i"></param>
        /// <param name="err"></param>
        private string GetLisReturnResult(int i, ref string message, ref string clinicno)
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

                System.Xml.XmlElement CLINIC_CODE = xml.CreateElement("CLINIC_CODE");
                CLINIC_CODE.InnerText = clinicno;
                root1.AppendChild(CLINIC_CODE);
            }

            else
            {
                System.Xml.XmlElement Code = xml.CreateElement("Code");
                Code.InnerText = "0";
                root1.AppendChild(Code);

                System.Xml.XmlElement ErrorMsg = xml.CreateElement("ErrorMsg");
                ErrorMsg.InnerText = message;
                root1.AppendChild(ErrorMsg);

                System.Xml.XmlElement CLINIC_CODE = xml.CreateElement("CLINIC_CODE");
                CLINIC_CODE.InnerText = clinicno;
                root1.AppendChild(CLINIC_CODE);
            }

            System.Xml.XmlElement Result = xml.CreateElement("Result");
            root1.AppendChild(Result);

            return xml.InnerXml.ToString();
        }

        /// <summary>
        /// 血透挂号
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        public string TjPatientNo(string xml)
        {
            int i = -1;
            string err = "";
            string clinicno = "";
            His.Models.ZWTJ.OutPERegisterInfo ipa = new His.Models.ZWTJ.OutPERegisterInfo();
            ipa = this.GetInPatientModel(xml);
            if (ipa.par_card_no != null)
            {
                i = this.TjPatientNoData(ipa, ref err, ref clinicno);
            }
            else
            {
                i = -1;
                err = "输入卡号不能为空，请核实";
            }
            return this.GetLisReturnResult(i, ref err, ref clinicno);
        }
    }

    public class CancelPEPatFee
    {

        private int TjCancelPEPatFeeData(His.Models.ZWTJ.OutGetPEPatFee CancelPEPatFee, ref string err)
        {
            #region sql
            string sql = @" 
           update fin_opb_feedetail t
     set t.pay_flag = '2'
   where t.recipe_no = '{0}'
     and t.trans_type = '1'
     and t.pay_flag = '0'
     and t.cancel_flag = '1'
     and t.reg_dpcd='7021'     
        ";
            sql = string.Format(sql, CancelPEPatFee.par_recipe_num);
            #endregion
            try
            {
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                {
                    return -1;
                }

                try
                {
                    string sql2 = @"insert into ZWTJ_COM_LOG
                                     values('{0}',
                                     '{1}',
                                    null,
                                     sysdate)";
                    sql2 = string.Format(sql2, CancelPEPatFee.par_recipe_num, "体检作废");
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
        /// 体检作废项目
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private His.Models.ZWTJ.OutGetPEPatFee GetInPatientModel(string xml)
        {
            His.Models.ZWTJ.OutGetPEPatFee opa = new His.Models.ZWTJ.OutGetPEPatFee();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList par_recipe_num1 = doc.GetElementsByTagName("PAR_RECIPE_NUM");
            System.Xml.XmlNode par_recipe_num = par_recipe_num1[0];
            if (!string.IsNullOrEmpty(par_recipe_num.InnerText))
            {
                opa.par_recipe_num = par_recipe_num.InnerText;
            }
            else
            {
                opa.par_recipe_num = "ALL";
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
        /// 体检作废项目
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        public string TjCancelPEPatFee(string xml)
        {
            int i = -1;
            string err = "";
            His.Models.ZWTJ.OutGetPEPatFee ipa = new His.Models.ZWTJ.OutGetPEPatFee();
            ipa = this.GetInPatientModel(xml);
            if (ipa.par_recipe_num != null)
            {
                i = this.TjCancelPEPatFeeData(ipa, ref err);
            }
            else
            {
                i = -1;
                err = "输入处方号不能为空，请核实";
            }
            this.GetLisReturnResult(i, ref err);
            return err;
        }

    }

    public class ComfirmPEPatFee
    {

        private int TjComfirmPEPatFeeData(His.Models.ZWTJ.OutGetPEPatFee ComfirmPEPatFee, ref string err)
        {
            #region sql
            string sql = @" 
             update fin_opb_feedetail t
     set t.noback_num = '{1}'
   where t.recipe_no = '{0}'
     and t.trans_type = '1'
     and t.pay_flag = '1'
     and t.cancel_flag = '1'
     and t.reg_dpcd='7021'  
        ";
            sql = string.Format(sql, ComfirmPEPatFee.par_recipe_num, ComfirmPEPatFee.par_qty);
            #endregion
            try
            {
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                {
                    return -1;
                }

                try
                {
                    string sql2 = @"insert into ZWTJ_COM_LOG
                                     values('{0}',
                                     '{1}',
                                    null,
                                     sysdate)";
                    sql2 = string.Format(sql2, ComfirmPEPatFee.par_recipe_num, "确认收费项目",
                        ComfirmPEPatFee.par_qty);
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
        /// 确认收费项目
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private His.Models.ZWTJ.OutGetPEPatFee GetInPatientModel(string xml)
        {
            His.Models.ZWTJ.OutGetPEPatFee opa = new His.Models.ZWTJ.OutGetPEPatFee();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList par_recipe_num1 = doc.GetElementsByTagName("PAR_RECIPE_NUM");
            System.Xml.XmlNode par_recipe_num = par_recipe_num1[0];
            if (!string.IsNullOrEmpty(par_recipe_num.InnerText))
            {
                opa.par_recipe_num = par_recipe_num.InnerText;
            }
            else
            {
                opa.par_recipe_num = "ALL";
            }

            System.Xml.XmlNodeList par_qty1 = doc.GetElementsByTagName("PAR_QTY");
            System.Xml.XmlNode par_qty = par_qty1[0];
            if (!string.IsNullOrEmpty(par_qty.InnerText))
            {
                opa.par_qty = par_qty.InnerText;
            }
            else
            {
                opa.par_qty = "ALL";
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
        /// 确认收费项目
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        public string TjComfirmPEPatFee(string xml)
        {
            int i = -1;
            string err = "";
            His.Models.ZWTJ.OutGetPEPatFee ipa = new His.Models.ZWTJ.OutGetPEPatFee();
            ipa = this.GetInPatientModel(xml);
            if (ipa.par_recipe_num != null)
            {
                i = this.TjComfirmPEPatFeeData(ipa, ref err);
            }
            else
            {
                i = -1;
                err = "输入处方号不能为空，请核实";
            }
            this.GetLisReturnResult(i, ref err);
            return err;
        }

    }

    public class PEPatInfoChange
    {

        private int TjPEPatInfoChangeData(His.Models.ZWTJ.OutPERegisterInfo PEPatInfoChange, ref string err)
        {
            #region sql
            string sql = @" 
           update fin_opr_register t
     set t.name = '{1}',
     t.sex_code = '{2}',
     t.idenno = '{3}',
     t.birthday = to_date('{4}','yyyy-mm-dd'),
     t.rela_phone = '{5}',
     t.address = '{6}',
     t.dept_code = '{7}',
     t.dept_name = '{8}',
     t.oper_code = '{9}'
   where t.card_no = '{0}'
     and t.dept_code='7021'
     and (t.card_no like '95%' or t.card_no like'99%' or t.card_no like'10%')  
        ";
            sql = string.Format(sql, PEPatInfoChange.par_card_no, PEPatInfoChange.par_name, PEPatInfoChange.par_sex_code, PEPatInfoChange.par_idenno, PEPatInfoChange.par_birthday, PEPatInfoChange.par_rela_phone, PEPatInfoChange.par_address, PEPatInfoChange.par_dept_code, PEPatInfoChange.par_dept_name, PEPatInfoChange.par_oper_code);
            #endregion
            try
            {
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                {
                    return -1;
                }

                try
                {
                    string sql2 = @"insert into ZWTJ_COM_LOG
                                     values('{0}',
                                     '{1}',
                                    '{2}',
                                     sysdate)";
                    sql2 = string.Format(sql2, PEPatInfoChange.par_card_no, "体检挂号信息变更",
                        PEPatInfoChange.par_name + "||" + PEPatInfoChange.par_dept_code);
                    DataBaseHelp.DataExecHelp.ExecSql(sql2, ref err);
                }

                catch { }
                try
                {
                    His.Business.ZZSB.Medical.MedicalDB db = new His.Business.ZZSB.Medical.MedicalDB();
                    string sqlUpdateRegister = "";
                    if (PEPatInfoChange.par_elderlyvoucher_flag == "1")
                    {
                        sqlUpdateRegister = "UPDATE fin_opr_register SET pact_code = '258',pact_name = '香港长者券' WHERE clinic_code = (SELECT MAX(clinic_code) FROM fin_opr_register WHERE card_no = '{0}')";
                    }
                    else
                    {
                        sqlUpdateRegister = "UPDATE fin_opr_register SET pact_code = '1',pact_name = '普通' WHERE clinic_code = (SELECT MAX(clinic_code) FROM fin_opr_register WHERE card_no = '{0}')";
                    }
                    //DataTable dtRegInfo = db.GetRegisterInfoByCardNo(PEPatInfoChange.par_card_no);
                    //string ClinicCode = dtRegInfo.Rows[0][0].ToString();
                    //string RegDate = DateTime.Parse(dtRegInfo.Rows[0][1].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    //string DeptCode = dtRegInfo.Rows[0][2].ToString();
                    //string DeptName = dtRegInfo.Rows[0][3].ToString();
                    //string DoctCode = dtRegInfo.Rows[0][4].ToString();
                    //string PactCode = dtRegInfo.Rows[0][5].ToString();
                    //string RegLevelCode = dtRegInfo.Rows[0][6].ToString();
                    //string sqlInsertRegFeeDetail = Sql.Sql.insertRegFeeDetail;

                    //根据挂号级别获取需要插入的项目编码
                    //string ItemCode = string.Empty;
                    //int ret =  db.getRegItemCode(RegLevelCode, ref ItemCode);
                    //if (!db.CheckRegFeeDetail(ClinicCode,ItemCode))//根据门诊流水号和项目编码查询是否已经有收取诊疗费，防止重复收取
                    //{
                    //    string ItemName = db.GetItemNameForItemCode(ItemCode);
                    //    string ItemPrice = db.GetPriceForItemCode(ItemCode);
                    //    sqlInsertRegFeeDetail = string.Format(sqlInsertRegFeeDetail,
                    //        db.GetOpbRecipeNoSequece(),
                    //           "1",
                    //           "1",
                    //           ClinicCode,
                    //           PEPatInfoChange.par_card_no,
                    //           RegDate,
                    //           DeptCode,
                    //           DoctCode,
                    //           DeptCode,
                    //           ItemCode,
                    //           ItemName,
                    //           "0",
                    //           "次",
                    //           "015",
                    //           "U",
                    //           ItemPrice,
                    //           "1",
                    //           "1",
                    //           "0",
                    //           "0",
                    //           "0",
                    //           "1",
                    //           "次",
                    //           "0",
                    //           "0",
                    //           ItemPrice,
                    //           DeptCode,
                    //           DeptName,
                    //           "0",
                    //           DoctCode,
                    //           DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //           "0",
                    //           "1",
                    //           "0",
                    //           "0",
                    //           "1",
                    //           "1",
                    //           "0",
                    //           db.GetMetMOOrderIDSequece(),
                    //           ItemPrice,
                    //           "0",
                    //           "0",
                    //           "0",
                    //           "0",
                    //           "0",
                    //           db.GetBelongDeptCodeForEmplCode(DoctCode),//医生所属科室
                    //           "01",
                    //           "258",
                    //           ItemPrice,
                    //           "0",
                    //           db.GetBelongDeptCodeForEmplCode(DoctCode),//开立医生所属科室
                    //           "CORE_HIS50",
                    //           "NULL");

                    sqlUpdateRegister = string.Format(sqlUpdateRegister, PEPatInfoChange.par_card_no);
                    if (!DataBaseHelp.DataExecHelp.ExecSql(sqlUpdateRegister, ref err))
                    {
                        return -1;
                    }
                    //if (!DataBaseHelp.DataExecHelp.ExecSql(sqlInsertRegFeeDetail, ref err))
                    //{
                    //    return -1;
                    //} 
                    //}

                }
                catch (Exception e)
                {
                    return -1;
                }
                return 1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        /// <summary>
        /// 体检信息变更
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private His.Models.ZWTJ.OutPERegisterInfo GetInPatientModel(string xml)
        {
            His.Models.ZWTJ.OutPERegisterInfo opa = new His.Models.ZWTJ.OutPERegisterInfo();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList par_card_no1 = doc.GetElementsByTagName("PAR_CARD_NO");
            System.Xml.XmlNode par_card_no = par_card_no1[0];
            if (!string.IsNullOrEmpty(par_card_no.InnerText))
            {
                opa.par_card_no = par_card_no.InnerText;
            }
            else
            {
                opa.par_card_no = "ALL";
            }

            System.Xml.XmlNodeList par_name1 = doc.GetElementsByTagName("PAR_NAME");
            System.Xml.XmlNode par_name = par_name1[0];
            if (!string.IsNullOrEmpty(par_name.InnerText))
            {
                opa.par_name = par_name.InnerText;
            }
            else
            {
                opa.par_name = "ALL";
            }

            System.Xml.XmlNodeList par_sex_code1 = doc.GetElementsByTagName("PAR_SEX_CODE");
            System.Xml.XmlNode par_sex_code = par_sex_code1[0];
            if (!string.IsNullOrEmpty(par_sex_code.InnerText))
            {
                opa.par_sex_code = par_sex_code.InnerText;
            }
            else
            {
                opa.par_sex_code = "ALL";
            }

            System.Xml.XmlNodeList par_idenno1 = doc.GetElementsByTagName("PAR_IDENNO");
            System.Xml.XmlNode par_idenno = par_idenno1[0];
            if (!string.IsNullOrEmpty(par_idenno.InnerText))
            {
                opa.par_idenno = par_idenno.InnerText;
            }
            else
            {
                opa.par_idenno = "ALL";
            }

            System.Xml.XmlNodeList par_birthday1 = doc.GetElementsByTagName("PAR_BIRTHDAY");
            System.Xml.XmlNode par_birthday = par_birthday1[0];
            if (!string.IsNullOrEmpty(par_birthday.InnerText))
            {
                opa.par_birthday = par_birthday.InnerText;
            }
            else
            {
                opa.par_birthday = "ALL";
            }

            System.Xml.XmlNodeList par_rela_phone1 = doc.GetElementsByTagName("PAR_RELA_PHONE");
            System.Xml.XmlNode par_rela_phone = par_rela_phone1[0];
            if (!string.IsNullOrEmpty(par_rela_phone.InnerText))
            {
                opa.par_rela_phone = par_rela_phone.InnerText;
            }
            else
            {
                opa.par_rela_phone = "ALL";
            }

            System.Xml.XmlNodeList par_address1 = doc.GetElementsByTagName("PAR_ADDRESS");
            System.Xml.XmlNode par_address = par_address1[0];
            if (!string.IsNullOrEmpty(par_address.InnerText))
            {
                opa.par_address = par_address.InnerText;
            }
            else
            {
                opa.par_address = "ALL";
            }

            System.Xml.XmlNodeList par_dept_code1 = doc.GetElementsByTagName("PAR_DEPT_CODE");
            System.Xml.XmlNode par_dept_code = par_dept_code1[0];
            if (!string.IsNullOrEmpty(par_dept_code.InnerText))
            {
                opa.par_dept_code = par_dept_code.InnerText;
            }
            else
            {
                opa.par_dept_code = "ALL";
            }

            System.Xml.XmlNodeList par_dept_name1 = doc.GetElementsByTagName("PAR_DEPT_NAME");
            System.Xml.XmlNode par_dept_name = par_dept_name1[0];
            if (!string.IsNullOrEmpty(par_dept_name.InnerText))
            {
                opa.par_dept_name = par_dept_name.InnerText;
            }
            else
            {
                opa.par_dept_name = "ALL";
            }

            System.Xml.XmlNodeList par_oper_code1 = doc.GetElementsByTagName("PAR_OPER_CODE");
            System.Xml.XmlNode par_oper_code = par_oper_code1[0];
            if (!string.IsNullOrEmpty(par_oper_code.InnerText))
            {
                opa.par_oper_code = par_oper_code.InnerText;
            }
            else
            {
                opa.par_oper_code = "ALL";
            }

            System.Xml.XmlNodeList par_elderlyvoucher_flag1 = doc.GetElementsByTagName("PAR_ELDERLYVOUCHER_FLAG");
            System.Xml.XmlNode par_elderlyvoucher_flag = par_elderlyvoucher_flag1[0];
            if (!string.IsNullOrEmpty(par_elderlyvoucher_flag.InnerText))
            {
                opa.par_elderlyvoucher_flag = par_elderlyvoucher_flag.InnerText;
            }
            else
            {
                opa.par_elderlyvoucher_flag = "ALL";
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
        /// 体检信息变更
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        public string TjPEPatInfoChange(string xml)
        {
            int i = -1;
            string err = "";
            His.Models.ZWTJ.OutPERegisterInfo ipa = new His.Models.ZWTJ.OutPERegisterInfo();
            ipa = this.GetInPatientModel(xml);
            if (ipa.par_card_no != null)
            {
                i = this.TjPEPatInfoChangeData(ipa, ref err);
            }
            else
            {
                i = -1;
                err = "输入卡号不能为空，请核实";
            }
            this.GetLisReturnResult(i, ref err);
            return err;
        }

    }

    public class GetPEPatFee
    {

        private int TjGetPEPatFeeData(His.Models.ZWTJ.OutGetPEPatFee GetPEPatFee, ref string err)
        {
            #region sql
            string sql = @" 
           DECLARE
           s1 VARCHAR2(500);
           s2 INTEGER;
           s3 VARCHAR2(500);
           begin 
           PRC_PE_INSERTFEEDETAIL(
           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',s1,s2,s3);
           end;     
        ";
            sql = string.Format(sql, GetPEPatFee.par_sequence_no, GetPEPatFee.par_card_no, GetPEPatFee.par_doctcode, GetPEPatFee.par_deptcode, GetPEPatFee.par_itemcode, GetPEPatFee.par_unit_price, GetPEPatFee.par_qty, GetPEPatFee.par_own_cost, GetPEPatFee.par_execdeptcode, GetPEPatFee.par_execdeptname);
            #endregion
            try
            {
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                {
                    return -1;
                }

                try
                {
                    string sql2 = @"insert into ZWTJ_COM_LOG
                                     values('{0}',
                                     '{1}',
                                    '{2}',
                                     sysdate)";
                    sql2 = string.Format(sql2, GetPEPatFee.par_card_no, "体检收费",
                        GetPEPatFee.par_itemcode + "||" + GetPEPatFee.par_own_cost + "||" + GetPEPatFee.par_execdeptcode + "||" + GetPEPatFee.par_unit_price);
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
        /// 体检收费
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private His.Models.ZWTJ.OutGetPEPatFee GetInPatientModel(string xml)
        {
            His.Models.ZWTJ.OutGetPEPatFee opa = new His.Models.ZWTJ.OutGetPEPatFee();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList par_sequence_no1 = doc.GetElementsByTagName("PAR_SEQUENCE_NO");
            System.Xml.XmlNode par_sequence_no = par_sequence_no1[0];
            if (!string.IsNullOrEmpty(par_sequence_no.InnerText))
            {
                opa.par_sequence_no = par_sequence_no.InnerText;
            }
            else
            {
                opa.par_sequence_no = "ALL";
            }

            System.Xml.XmlNodeList par_card_no1 = doc.GetElementsByTagName("PAR_CARD_NO");
            System.Xml.XmlNode par_card_no = par_card_no1[0];
            if (!string.IsNullOrEmpty(par_card_no.InnerText))
            {
                opa.par_card_no = par_card_no.InnerText;
            }
            else
            {
                opa.par_card_no = "ALL";
            }


            System.Xml.XmlNodeList par_doctcode1 = doc.GetElementsByTagName("PAR_DOCTCODE");
            System.Xml.XmlNode par_doctcode = par_doctcode1[0];
            if (!string.IsNullOrEmpty(par_doctcode.InnerText))
            {
                opa.par_doctcode = par_doctcode.InnerText;
            }
            else
            {
                opa.par_doctcode = "ALL";
            }

            System.Xml.XmlNodeList par_deptcode1 = doc.GetElementsByTagName("PAR_DEPTCODE");
            System.Xml.XmlNode par_deptcode = par_deptcode1[0];
            if (!string.IsNullOrEmpty(par_deptcode.InnerText))
            {
                opa.par_deptcode = par_deptcode.InnerText;
            }
            else
            {
                opa.par_deptcode = "ALL";
            }

            System.Xml.XmlNodeList par_itemcode1 = doc.GetElementsByTagName("PAR_ITEMCODE");
            System.Xml.XmlNode par_itemcode = par_itemcode1[0];
            if (!string.IsNullOrEmpty(par_itemcode.InnerText))
            {
                opa.par_itemcode = par_itemcode.InnerText;
            }
            else
            {
                opa.par_itemcode = "ALL";
            }

            System.Xml.XmlNodeList par_unit_price1 = doc.GetElementsByTagName("PAR_UNIT_PRICE");
            System.Xml.XmlNode par_unit_price = par_unit_price1[0];
            if (!string.IsNullOrEmpty(par_unit_price.InnerText))
            {
                opa.par_unit_price = par_unit_price.InnerText;
            }
            else
            {
                opa.par_unit_price = "ALL";
            }


            System.Xml.XmlNodeList par_qty1 = doc.GetElementsByTagName("PAR_QTY");
            System.Xml.XmlNode par_qty = par_qty1[0];
            if (!string.IsNullOrEmpty(par_qty.InnerText))
            {
                opa.par_qty = par_qty.InnerText;
            }
            else
            {
                opa.par_qty = "ALL";
            }


            System.Xml.XmlNodeList par_own_cost1 = doc.GetElementsByTagName("PAR_OWN_COST");
            System.Xml.XmlNode par_own_cost = par_own_cost1[0];
            if (!string.IsNullOrEmpty(par_own_cost.InnerText))
            {
                opa.par_own_cost = par_own_cost.InnerText;
            }
            else
            {
                opa.par_own_cost = "ALL";
            }

            System.Xml.XmlNodeList par_execdeptcode1 = doc.GetElementsByTagName("PAR_EXECDEPTCODE");
            System.Xml.XmlNode par_execdeptcode = par_execdeptcode1[0];
            if (!string.IsNullOrEmpty(par_execdeptcode.InnerText))
            {
                opa.par_execdeptcode = par_execdeptcode.InnerText;
            }
            else
            {
                opa.par_execdeptcode = "ALL";
            }

            System.Xml.XmlNodeList par_execdeptname1 = doc.GetElementsByTagName("PAR_EXECDEPTNAME");
            System.Xml.XmlNode par_execdeptname = par_execdeptname1[0];
            if (!string.IsNullOrEmpty(par_execdeptname.InnerText))
            {
                opa.par_execdeptname = par_execdeptname.InnerText;
            }
            else
            {
                opa.par_execdeptname = "ALL";
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
        /// 体检收费
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        public string TjGetPEPatFee(string xml)
        {
            int i = -1;
            string err = "";
            His.Models.ZWTJ.OutGetPEPatFee ipa = new His.Models.ZWTJ.OutGetPEPatFee();
            ipa = this.GetInPatientModel(xml);
            if (ipa.par_card_no != null)
            {
                i = this.TjGetPEPatFeeData(ipa, ref err);
            }
            else
            {
                i = -1;
                err = "输入卡号不能为空，请核实";
            }
            this.GetLisReturnResult(i, ref err);
            return err;
        }

    }

    public class PeProjectDict
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


        private System.Collections.ArrayList GetPeProjectDictData(His.Models.ZWTJ.OutPeProjectDict PeProjectDict)
        {
            #region sql
            string sql = @"
            select w.item_code,
                   w.item_name,
                   w.unit_price,
                   w.exedept_code
              from fin_com_undruginfo w
             where w.valid_state = '1'
            --and w.unitflag='0'
            ";
            #endregion

            try
            {
                #region 数据赋值
                //sql = string.Format(sql, PeProjectDict.IDCARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //体检项目字典
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PeProjectDict = new His.Models.ZWTJ.OutPeProjectDict();
                    PeProjectDict.ITEM_CODE = dt.Rows[i][0].ToString();
                    PeProjectDict.ITEM_NAME = dt.Rows[i][1].ToString();
                    PeProjectDict.UNIT_PRICE = dt.Rows[i][2].ToString();
                    PeProjectDict.EXEDEPT_CODE = dt.Rows[i][3].ToString();
                    al.Add(PeProjectDict);
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

        private string GetPeProjectDictXML(System.Collections.ArrayList al)
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

                //System.Xml.XmlElement Result = xml.CreateElement("Result");
                //root1.AppendChild(Result);


                //His.Models.ZZSB.GetGuideListInfoForSRM p = al[0] as His.Models.ZZSB.GetGuideListInfoForSRM;
                foreach (His.Models.ZWTJ.OutPeProjectDict p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    //if (p.IDCARDNO == "ALL")
                    //{
                    //    return this.ERR();
                    //}

                    System.Xml.XmlElement ITEM_CODE = xml.CreateElement("item_code");
                    ITEM_CODE.InnerText = p.ITEM_CODE;
                    Result.AppendChild(ITEM_CODE);

                    System.Xml.XmlElement ITEM_NAME = xml.CreateElement("item_name");
                    ITEM_NAME.InnerText = p.ITEM_NAME;
                    Result.AppendChild(ITEM_NAME);

                    System.Xml.XmlElement UNIT_PRICE = xml.CreateElement("unit_price");
                    UNIT_PRICE.InnerText = p.UNIT_PRICE;
                    Result.AppendChild(UNIT_PRICE);

                    System.Xml.XmlElement EXEDEPT_CODE = xml.CreateElement("exedept_code");
                    EXEDEPT_CODE.InnerText = p.EXEDEPT_CODE;
                    Result.AppendChild(EXEDEPT_CODE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetPeProjectDictModel(string xml, ref His.Models.ZWTJ.OutPeProjectDict opa)
        {

            string returnStr = "";
            opa = new His.Models.ZWTJ.OutPeProjectDict();
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


            //System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            //System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            //if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            //{
            //    opa.DEVICEID = DEVICEID.InnerText;
            //}
            //else
            //{
            //    this.resultCode = "0";
            //    this.msg = "设备编号不能为空！";
            //    return this.ReturnFailure();
            //}

            //System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            //System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            //if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            //{
            //    opa.SERVICECODE = SERVICECODE.InnerText;
            //}
            //else
            //{
            //    this.resultCode = "0";
            //    this.msg = "服务编号不能为空！";
            //    return this.ReturnFailure();
            //}


            return returnStr;
        }



        public string GetPeProjectDict(string xml)
        {
            string returnStr = "";
            His.Models.ZWTJ.OutPeProjectDict opa = new His.Models.ZWTJ.OutPeProjectDict();
            returnStr = this.GetPeProjectDictModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetPeProjectDictData(opa);
            returnStr = this.GetPeProjectDictXML(al);
            return returnStr;
        }
    }

    public class PeZtDictInfo
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


        private System.Collections.ArrayList GetPeZtDictInfoData(His.Models.ZWTJ.OutPeProjectDict PeZtDictInfo)
        {
            #region sql
            string sql = @"
            select t.package_code package_code,
                   t.item_code    item_code,
                   t.qty          qty,
                   t.package_name package_name,
                   t.item_name item_name,
                   (select f.exedept_code from fin_com_undruginfo f where f.item_code=t.item_code）exedept_code
              from fin_com_undrugztinfo t
             where t.valid_state = '1'
            ";
            #endregion

            try
            {
                #region 数据赋值
                //sql = string.Format(sql, PeZtDictInfo.IDCARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //体检组套项目字典
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PeZtDictInfo = new His.Models.ZWTJ.OutPeProjectDict();
                    PeZtDictInfo.PACKAGE_CODE = dt.Rows[i][0].ToString();
                    PeZtDictInfo.ITEM_CODE = dt.Rows[i][1].ToString();
                    PeZtDictInfo.QTY = dt.Rows[i][2].ToString();
                    PeZtDictInfo.PACKAGE_NAME = dt.Rows[i][3].ToString();
                    PeZtDictInfo.ITEM_NAME = dt.Rows[i][4].ToString();
                    PeZtDictInfo.EXEDEPT_CODE = dt.Rows[i][5].ToString();
                    al.Add(PeZtDictInfo);
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

        private string GetPeZtDictInfoXML(System.Collections.ArrayList al)
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

                //System.Xml.XmlElement Result = xml.CreateElement("Result");
                //root1.AppendChild(Result);


                //His.Models.ZZSB.GetGuideListInfoForSRM p = al[0] as His.Models.ZZSB.GetGuideListInfoForSRM;
                foreach (His.Models.ZWTJ.OutPeProjectDict p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    //if (p.IDCARDNO == "ALL")
                    //{
                    //    return this.ERR();
                    //}

                    System.Xml.XmlElement PACKAGE_CODE = xml.CreateElement("package_code");
                    PACKAGE_CODE.InnerText = p.PACKAGE_CODE;
                    Result.AppendChild(PACKAGE_CODE);

                    System.Xml.XmlElement ITEM_CODE = xml.CreateElement("item_code");
                    ITEM_CODE.InnerText = p.ITEM_CODE;
                    Result.AppendChild(ITEM_CODE);

                    System.Xml.XmlElement QTY = xml.CreateElement("qty");
                    QTY.InnerText = p.QTY;
                    Result.AppendChild(QTY);

                    System.Xml.XmlElement PACKAGE_NAME = xml.CreateElement("package_name");
                    PACKAGE_NAME.InnerText = p.PACKAGE_NAME;
                    Result.AppendChild(PACKAGE_NAME);

                    System.Xml.XmlElement ITEM_NAME = xml.CreateElement("item_name");
                    ITEM_NAME.InnerText = p.ITEM_NAME;
                    Result.AppendChild(ITEM_NAME);

                    System.Xml.XmlElement EXEDEPT_CODE = xml.CreateElement("exedept_code");
                    EXEDEPT_CODE.InnerText = p.EXEDEPT_CODE;
                    Result.AppendChild(EXEDEPT_CODE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetPeZtDictInfoModel(string xml, ref His.Models.ZWTJ.OutPeProjectDict opa)
        {

            string returnStr = "";
            opa = new His.Models.ZWTJ.OutPeProjectDict();
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


            return returnStr;
        }



        public string GetPeZtDictInfo(string xml)
        {
            string returnStr = "";
            His.Models.ZWTJ.OutPeProjectDict opa = new His.Models.ZWTJ.OutPeProjectDict();
            returnStr = this.GetPeZtDictInfoModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetPeZtDictInfoData(opa);
            returnStr = this.GetPeZtDictInfoXML(al);
            return returnStr;
        }
    }

    public class PeChargeitemDeail
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


        private System.Collections.ArrayList GetPeChargeitemDeailData(His.Models.ZWTJ.OutPeProjectDict PeChargeitemDeail)
        {
            #region sql
            string sql = @"
            select b.recipe_no,
                   b.sequence_no,
                   t.card_no,
                   b.clinic_code,
                   t.name,
                   b.item_code,
                   b.item_name,
                   b.fee_date,--收费时间
                   b.reg_dpcd, --开单科室
                   b.pay_flag,   ---收费状态，1已收费，0未收费,2已作废
                   b.trans_type,--退费状态
                   b.noback_num,--可退数量
                   b.own_cost
              from fin_opb_feedetail b,fin_opr_register t
             where b.clinic_code=t.clinic_code
             and b.clinic_code='{0}'
             and b.card_no='{1}'
             --and b.belong_dept='7021'
             and (t.card_no like'95%' or t.card_no like'99%' or t.card_no like'10%')
                        ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, PeChargeitemDeail.CLINIC_CODE, PeChargeitemDeail.CARD_NO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //体检收费项目明细
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PeChargeitemDeail = new His.Models.ZWTJ.OutPeProjectDict();
                    PeChargeitemDeail.RECIPE_NO = dt.Rows[i][0].ToString();
                    PeChargeitemDeail.SEQUENCE_NO = dt.Rows[i][1].ToString();
                    PeChargeitemDeail.CARD_NO = dt.Rows[i][2].ToString();
                    PeChargeitemDeail.CLINIC_CODE = dt.Rows[i][3].ToString();
                    PeChargeitemDeail.NAME = dt.Rows[i][4].ToString();
                    PeChargeitemDeail.ITEM_CODE = dt.Rows[i][5].ToString();
                    PeChargeitemDeail.ITEM_NAME = dt.Rows[i][6].ToString();
                    PeChargeitemDeail.FEE_DATE = dt.Rows[i][7].ToString();
                    PeChargeitemDeail.REG_DPCD = dt.Rows[i][8].ToString();
                    PeChargeitemDeail.PAY_FLAG = dt.Rows[i][9].ToString();
                    PeChargeitemDeail.TRANS_TYPE = dt.Rows[i][10].ToString();
                    PeChargeitemDeail.NOBACK_NUM = dt.Rows[i][11].ToString();
                    PeChargeitemDeail.OWN_COST = dt.Rows[i][12].ToString();
                    al.Add(PeChargeitemDeail);
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

        private string GetPeChargeitemDeailXML(System.Collections.ArrayList al)
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

                //System.Xml.XmlElement Result = xml.CreateElement("Result");
                //root1.AppendChild(Result);


                foreach (His.Models.ZWTJ.OutPeProjectDict p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CLINIC_CODE == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement RECIPE_NO = xml.CreateElement("recipe_no");
                    RECIPE_NO.InnerText = p.RECIPE_NO;
                    Result.AppendChild(RECIPE_NO);

                    System.Xml.XmlElement SEQUENCE_NO = xml.CreateElement("sequence_no");
                    SEQUENCE_NO.InnerText = p.SEQUENCE_NO;
                    Result.AppendChild(SEQUENCE_NO);

                    System.Xml.XmlElement CARD_NO = xml.CreateElement("card_no");
                    CARD_NO.InnerText = p.CARD_NO;
                    Result.AppendChild(CARD_NO);

                    System.Xml.XmlElement CLINIC_CODE = xml.CreateElement("clinic_code");
                    CLINIC_CODE.InnerText = p.CLINIC_CODE;
                    Result.AppendChild(CLINIC_CODE);

                    System.Xml.XmlElement NAME = xml.CreateElement("name");
                    NAME.InnerText = p.NAME;
                    Result.AppendChild(NAME);

                    System.Xml.XmlElement ITEM_CODE = xml.CreateElement("item_code");
                    ITEM_CODE.InnerText = p.ITEM_CODE;
                    Result.AppendChild(ITEM_CODE);

                    System.Xml.XmlElement ITEM_NAME = xml.CreateElement("item_name");
                    ITEM_NAME.InnerText = p.ITEM_NAME;
                    Result.AppendChild(ITEM_NAME);

                    System.Xml.XmlElement FEE_DATE = xml.CreateElement("fee_date");
                    FEE_DATE.InnerText = p.FEE_DATE;
                    Result.AppendChild(FEE_DATE);

                    System.Xml.XmlElement REG_DPCD = xml.CreateElement("reg_dpcd");
                    REG_DPCD.InnerText = p.REG_DPCD;
                    Result.AppendChild(REG_DPCD);

                    System.Xml.XmlElement PAY_FLAG = xml.CreateElement("pay_flag");
                    PAY_FLAG.InnerText = p.PAY_FLAG;
                    Result.AppendChild(PAY_FLAG);

                    System.Xml.XmlElement TRANS_TYPE = xml.CreateElement("trans_type");
                    TRANS_TYPE.InnerText = p.TRANS_TYPE;
                    Result.AppendChild(TRANS_TYPE);

                    System.Xml.XmlElement NOBACK_NUM = xml.CreateElement("noback_num");
                    NOBACK_NUM.InnerText = p.NOBACK_NUM;
                    Result.AppendChild(NOBACK_NUM);

                    System.Xml.XmlElement OWN_COST = xml.CreateElement("own_cost");
                    OWN_COST.InnerText = p.OWN_COST;
                    Result.AppendChild(OWN_COST);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetPeChargeitemDeailModel(string xml, ref His.Models.ZWTJ.OutPeProjectDict opa)
        {

            string returnStr = "";
            opa = new His.Models.ZWTJ.OutPeProjectDict();
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


            System.Xml.XmlNodeList CLINIC_CODE1 = doc.GetElementsByTagName("clinic_code");
            System.Xml.XmlNode CLINIC_CODE = CLINIC_CODE1[0];
            if (!string.IsNullOrEmpty(CLINIC_CODE.InnerText))
            {
                opa.CLINIC_CODE = CLINIC_CODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "门诊流水号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList CARD_NO1 = doc.GetElementsByTagName("card_no");
            System.Xml.XmlNode CARD_NO = CARD_NO1[0];
            if (!string.IsNullOrEmpty(CARD_NO.InnerText))
            {
                opa.CARD_NO = CARD_NO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }


            return returnStr;
        }



        public string GetPeChargeitemDeail(string xml)
        {
            string returnStr = "";
            His.Models.ZWTJ.OutPeProjectDict opa = new His.Models.ZWTJ.OutPeProjectDict();
            returnStr = this.GetPeChargeitemDeailModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetPeChargeitemDeailData(opa);
            returnStr = this.GetPeChargeitemDeailXML(al);
            return returnStr;
        }
    }
}
