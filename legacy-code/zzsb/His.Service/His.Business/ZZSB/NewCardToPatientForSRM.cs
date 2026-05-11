using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace His.Business.ZZSB
{
    public class NewCardToPatientForSRM
    {

        private int NewCardToPatientForSRMData(His.Models.ZZSB.OutNewCardToPatientForSRM NewCardToPatientForSRM, ref string err)
        {
            try
            {
                string sqlStr = @"select SEQ_OPB_AUTOCARDNO.Nextval FROM DUAL";
                string card_No = string.Empty;
                DataSet ds = new DataSet();
                ds = DataBaseHelp.DataExecHelp.GetDataSet(sqlStr);
                card_No = ds.Tables[0].Rows[0][0].ToString();
                card_No = card_No.PadLeft(10, '0');
                #region sql
                string sql = @" 
         INSERT INTO com_patientinfo --病人基本信息表
              (card_no,
               ic_cardno,
               name,
               spell_code,
               wb_code,
               birthday,
               sex_code,
               idenno,
               blood_code,
               prof_code,
               work_home,
               work_tel,
               work_zip,
               home,
               home_tel,
               home_zip,
               district,
               nation_code,
               linkman_name,
               linkman_tel,
               linkman_add,
               rela_code,
               mari,
               coun_code,
               paykind_code,
               paykind_name,
               pact_code,
               pact_name,
               mcard_no,
               area_code,
               framt,
               anaphy_flag,
               hepatitis_flag,
               act_code,
               act_amt,
               lact_sum,
               lbank_sum,
               arrear_times,
               arrear_sum,
               inhos_source,
               lihos_date,
               inhos_times,
               louthos_date,
               fir_see_date,
               lreg_date,
               disoby_cnt,
               end_date,
               mark,
               oper_code,
               oper_date,
               IS_ENCRYPTNAME,
               normalname,
               IDCARDTYPE,
               VIP_FLAG,
               MONTHER_NAME,
               IS_TREATMENT,
               CASE_NO,
               INSURANCE_ID,
               INSURANCE_NAME,
               LINKMAN_DOOR_NO,
               HOME_DOOR_NO,
               EMAIL,
               HOME_NOW,
               patient_type)
            VALUES
              ('{9}', --就诊卡号
               '{0}', --电脑号
               '{1}', --姓名
               '', --拼音码
               '', --五笔
               to_date('{3}', 'yyyy-mm-dd'), --出生日期
               '{2}', --性别
               '{4}', --身份证号
               '', --血型
               '', --职业
               '', --工作单位
               '', --单位电话
               '', --单位邮编
               '{7}', --户口或家庭所在
               '{8}', --家庭电话
               '', --户口或家庭邮政编码
               '', --籍贯
               '{6}', --民族
               '', --联系人姓名
               '', --联系人电话
               '', --联系人住址
               '', --联系人关系
               '', --婚姻状况
               '{5}', --国籍
               '01', --结算类别
               '自费', --结算类别名称
               '1', --合同代码
               '现金', --合同单位名称
               '', --医疗证号
               '', --出生地
               '', --医疗费用
               '', --药物过敏
               '', --重要疾病
               '', --帐户密码
               '', --帐户总额
               '', --上期帐户余额
               '', --上期银行余额
               '', --欠费次数
               '', --欠费金额
               '', --住院来源
               '',
               '0', --住院次数
               '',
               '',
               '',
               '',
               '',
               '',
               '"  +ZZSB.RegisterManager.OPERID+ @"',
               sysdate,
               '',
               '',
               '',
               '',
               '',
               '',
               '',
               '',
               '',
               '',
               '',
               '',
               '',
               '')    
        ";
                sql = string.Format(sql, NewCardToPatientForSRM.CARDNO, NewCardToPatientForSRM.NAME, NewCardToPatientForSRM.SEX, NewCardToPatientForSRM.BIRTHDAY, NewCardToPatientForSRM.IDCARDNO, NewCardToPatientForSRM.NATIONALITY, NewCardToPatientForSRM.NATION, NewCardToPatientForSRM.ADDRESS, NewCardToPatientForSRM.PHONENO, card_No);
                #endregion
                //Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
               // Neusoft.FrameWork.Management.Connection.Sql.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                //if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                //{
                //    Neusoft.FrameWork.Management.PublicTrans.Trans.Rollback();
                //    return -1;
                //}

                string sql2 = @"INSERT INTO fin_opb_accountcard --卡对照表
                                      (card_no,--系统卡号
                                       markno,--物理卡号
                                       type,
                                       state,
                                       reflag,
                                       createoper,
                                       createdate,
                                       stopoper,
                                       stopdate,
                                       backoper,
                                       backdate,
                                       securitycode
                                       )
                                    VALUES
                                      ('{0}', --就诊卡号
                                       '{0}', --电脑号
                                       'Card_No', --卡类型
                                       '1',
                                       '0', 
                                       '" + RegisterManager.OPERID + @"', 
                                       sysdate, 
                                       '', 
                                       '', 
                                       '', 
                                       '', 
                                       '')";
                sql2 = string.Format(sql2,  card_No);
                //if (!DataBaseHelp.DataExecHelp.ExecSql(sql2, ref err))
                //{
                //    //Neusoft.FrameWork.Management.PublicTrans.Trans.Rollback();
                //    return -1;
                //}
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, sql2, ref err))
                {
                    //Neusoft.FrameWork.Management.PublicTrans.Trans.Rollback();
                    return -1;
                }

                //this.EMPIApply(NewCardToPatientForSRM, card_No);
                //Neusoft.FrameWork.Management.PublicTrans.Trans.Commit();
                return 1;
            }
            catch
            {
                //Neusoft.FrameWork.Management.PublicTrans.Trans.Rollback();
                return -1;
            }
        }

        /// <summary>
        /// 插入信息表
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private His.Models.ZZSB.OutNewCardToPatientForSRM NewCardToPatientModel(string xml)
        {
            His.Models.ZZSB.OutNewCardToPatientForSRM opa = new His.Models.ZZSB.OutNewCardToPatientForSRM();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                opa.CARDNO = "ALL";
            }

            System.Xml.XmlNodeList NAME1 = doc.GetElementsByTagName("Name");
            System.Xml.XmlNode NAME = NAME1[0];
            if (!string.IsNullOrEmpty(NAME.InnerText))
            {
                opa.NAME = NAME.InnerText;
            }
            else
            {
                opa.NAME = "ALL";
            }

            System.Xml.XmlNodeList SEX1 = doc.GetElementsByTagName("Sex");
            System.Xml.XmlNode SEX = SEX1[0];
            if (!string.IsNullOrEmpty(SEX.InnerText))
            {
                opa.SEX = SEX.InnerText;
            }
            else
            {
                opa.SEX = "ALL";
            }

            System.Xml.XmlNodeList BIRTHDAY1 = doc.GetElementsByTagName("Birthday");
            System.Xml.XmlNode BIRTHDAY = BIRTHDAY1[0];
            if (!string.IsNullOrEmpty(BIRTHDAY.InnerText))
            {
                opa.BIRTHDAY = BIRTHDAY.InnerText;
            }
            else
            {
                opa.BIRTHDAY = "ALL";
            }

            System.Xml.XmlNodeList IDCARDNO1 = doc.GetElementsByTagName("IDCardNo");
            System.Xml.XmlNode IDCARDNO = IDCARDNO1[0];
            if (!string.IsNullOrEmpty(IDCARDNO.InnerText))
            {
                opa.IDCARDNO = IDCARDNO.InnerText;
            }
            else
            {
                opa.IDCARDNO = "ALL";
            }

            System.Xml.XmlNodeList NATIONALITY1 = doc.GetElementsByTagName("Nationality");
            System.Xml.XmlNode NATIONALITY = NATIONALITY1[0];
            if (!string.IsNullOrEmpty(NATIONALITY.InnerText))
            {
                opa.NATIONALITY = NATIONALITY.InnerText;
            }
            else
            {
                opa.NATIONALITY = "ALL";
            }

            System.Xml.XmlNodeList NATION1 = doc.GetElementsByTagName("Nation");
            System.Xml.XmlNode NATION = NATION1[0];
            if (!string.IsNullOrEmpty(NATION.InnerText))
            {
                opa.NATION = NATION.InnerText;
            }
            else
            {
                opa.NATION = "ALL";
            }

            System.Xml.XmlNodeList ADDRESS1 = doc.GetElementsByTagName("Address");
            System.Xml.XmlNode ADDRESS = ADDRESS1[0];
            if (!string.IsNullOrEmpty(ADDRESS.InnerText))
            {
                opa.ADDRESS = ADDRESS.InnerText;
            }
            else
            {
                opa.ADDRESS = "ALL";
            }

            System.Xml.XmlNodeList PHONENO1 = doc.GetElementsByTagName("PhoneNo");
            System.Xml.XmlNode PHONENO = PHONENO1[0];
            if (!string.IsNullOrEmpty(PHONENO.InnerText))
            {
                opa.PHONENO = PHONENO.InnerText;
            }
            else
            {
                opa.PHONENO = "ALL";
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
        /// 插入病人信息表
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        public string NewCardToPatientFor(string xml)
        {
            int i = -1;
            string err = "";
            His.Models.ZZSB.OutNewCardToPatientForSRM ipa = new His.Models.ZZSB.OutNewCardToPatientForSRM();
            ipa = this.NewCardToPatientModel(xml);
            if (!string.IsNullOrEmpty(ipa.CARDNO))
            {
                i = this.NewCardToPatientForSRMData(ipa, ref err);
            }
            else
            {
                i = -1;
                err = "传入参数有误，请核实";
            }
            this.GetLisReturnResult(i, ref err);
            return err;
        }

        /// <summary>
        /// EMPI 数据处理 add by allan
        /// </summary>
        private void EMPIApply(His.Models.ZZSB.OutNewCardToPatientForSRM srmPat,string cardNo)
        {

            try
            {
                if (srmPat.IDCARDNO!=null && srmPat.IDCARDNO.Trim().Length == 18)
                {
                     DateTime dateBirthday = System.DateTime.MinValue;
                         //获得出生日期
                     try
                     {
                         dateBirthday = Convert.ToDateTime(srmPat.IDCARDNO.Trim().Substring(6, 4) + "-" + srmPat.IDCARDNO.Trim().Substring(10, 2) + "-" + srmPat.IDCARDNO.Trim().Substring(12, 2));
                     }
                     catch
                     {
                         return;
                     }
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate
                    {
                        //注册服务
                        Nesoft.EMPI.EMPI.PATIENTINFO pInfo = new Nesoft.EMPI.EMPI.PATIENTINFO(); //病人信息
                        Nesoft.EMPI.EMPI.PATIENT pat = new Nesoft.EMPI.EMPI.PATIENT();
                        pat.NAME = srmPat.NAME;                                 //姓名
                        pat.IDNO = srmPat.IDCARDNO;                                 //身份证号
                        pat.BIRTHDAY = dateBirthday.ToString("yyyy-MM-dd");// pat.BIRTHDAY = srmPat.BIRTHDAY;    //出生日期
                        if (srmPat.SEX == "F")
                        {
                            pat.SEX = "女";                              //性别
                        }
                        else
                        {
                            pat.SEX = "男"; 
                        }
                        pat.CNY = srmPat.NATIONALITY;                            //国家代码
                        pat.CNYNAME = "";                      //国家名称
                        pat.ACT = "";                                           //户籍代码
                        pat.ADDR = srmPat.ADDRESS;                          //家庭住址
                        pat.ZPCODE = "";                            //邮政编码
                        pat.ABOBLD = "";                     //血型
                        pat.RHBLD =  "";  //RH   
                        pat.NTN = srmPat.NATION;        //民族
                        pat.BCP = "";
                        pat.CTOR = "";
                        pat.CTORTEL = "";
                        pat.CTORLTN = "";
                        pat.HMTEL = srmPat.PHONENO;
                        pat.MOBILE = srmPat.PHONENO;                              //手机号码
                        pat.EML ="";
                        pat.CPY = "";
                        pat.CPYTEL = "";
                        pat.MRG = "";
                        pat.PFSN = "";     //职业代码
                        pat.MEMO = "";
                        Nesoft.EMPI.EMPI.CARD card = new Nesoft.EMPI.EMPI.CARD();
                        card.CARDNO = cardNo;
                        card.CARDTYPE = "O"; //门诊都是O
                        card.OPERCODE =  "00W999";
                        card.OPERNAME = "自助机";
                        pInfo.PATIENT = pat;
                        pInfo.CARDINFOS = new List<Nesoft.EMPI.EMPI.CARD> { card };
                        pInfo.DOMAIN = "001";
                        Nesoft.EMPI.EMPIOperate op = new Nesoft.EMPI.EMPIOperate();
                        op.EmpiReg(pInfo);
                    }));
                }
            }
            catch { }
        }
    }
}