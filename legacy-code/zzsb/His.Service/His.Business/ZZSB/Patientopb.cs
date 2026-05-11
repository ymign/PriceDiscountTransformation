using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDSI.MedicalOutpatientService;
using GDSI.CountryMedical.Model;
using Neusoft.HISFC.Models.SIInterface;
using GDSI.CountryMedical.Common;
using GDSI.CountryMedical.DAL;

namespace His.Business.ZZSB
{

    public class TestNetworkSr
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


        private System.Collections.ArrayList GetPatientopbData(His.Models.ZZSB.InTestNetworkSr TestNetworkSr)
        {
            #region sql
            string sql = @" select sysdate SYSTEMDATETIME,
                            null note
                            from dual
            ";
            #endregion

            try
            {
                #region 数据赋值
                //sql = string.Format(sql, TestNetworkSr.DEVICEID);

                System.Data.DataTable dt = new System.Data.DataTable();
                //网络测试
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TestNetworkSr = new His.Models.ZZSB.InTestNetworkSr();
                    TestNetworkSr.SYSTEMDATETIME = dt.Rows[i][0].ToString();
                    TestNetworkSr.NOTE = dt.Rows[i][1].ToString();
                    al.Add(TestNetworkSr);
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

        private string GetTestNetworkSrXML(System.Collections.ArrayList al)
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

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);


                //His.Models.ZZSB.InTestNetworkSr p = al[0] as His.Models.ZZSB.InTestNetworkSr;
                foreach (His.Models.ZZSB.InTestNetworkSr p in al)
                {
                    //if (p.DEVICEID == "ALL")
                    //{
                    //    return this.ERR();
                    //}

                    System.Xml.XmlElement SYSTEMDATETIME = xml.CreateElement("SYSTEMDATETIME");
                    SYSTEMDATETIME.InnerText = p.SYSTEMDATETIME;
                    Result.AppendChild(SYSTEMDATETIME);


                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutTestNetworkSrModel(string xml, ref His.Models.ZZSB.InTestNetworkSr opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InTestNetworkSr();
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

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();

            }



            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }


            return returnStr;
        }

        public string GetOutPatientInfoForZZSB(string xml)
        {
            // return "sss";
            string returnStr = "";
            His.Models.ZZSB.InTestNetworkSr opa = new His.Models.ZZSB.InTestNetworkSr();
            returnStr = this.GetOutTestNetworkSrModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetPatientopbData(opa);
            returnStr = this.GetTestNetworkSrXML(al);
            return returnStr;
        }
    }

    public class Patientopb
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


        private System.Collections.ArrayList GetPatientopbData(His.Models.ZZSB.Patientopr Patientopb)
        {
            #region sql
            string sql = @" select i.card_no patientid, --患者ID号
                         i.ic_cardno outpatientno, --门诊号
                         '1' cardstatus, --卡状态
                         null accountid, --预交金帐户ID
                         null accbalance, --预交金帐户余额
                         i.name, --患者姓名
                         decode(i.sex_code, 'F', 1, 'M', 0, 2) sex, --性别
                         fun_get_age(i.birthday) age, --年龄
                         null idcardtype, --证件类型
                         i.idenno idcardno, --身份号码
                         null bankcardno, --银行卡号
                         i.pact_code feetype, --患者费别
                         nvl(i.home_tel, '0') phoneno, --电话号码
                         nvl(i.mark, '无') note, --备用
                         NVL(i.elderlyvoucherflag,'0') elderlyvoucherflag
                    from com_patientinfo i
                    left join fin_opb_accountcard t
                      on i.card_no = t.card_no";

            string sqlWhere = string.Empty;

            if (Patientopb.CARDTYPECODE == "2")
            {

                if (Patientopb.IsShield == "1")
                {
                    sqlWhere = @" WHERE i.idenno = '{0}' and i.name= '{1}'  and i.card_no not like'99%'
                      and i.card_no not like'10%' and i.card_no not like '%L%'
and i.card_no not like '%C%' ";
                }
                else
                {
                    sqlWhere = @" WHERE i.idenno = '{0}' and i.name= '{1}'  ";//and i.card_no not like'99%' and i.card_no not like'10%' 
                }

            }
            else //if (Patientopb.IDCARDTYPE = "1")
            {
                sqlWhere = @" where (i.card_no = '{0}' or t.markno = '{0}') ";
            }
            sqlWhere = sqlWhere + "  order by nvl((select max(p.reg_date) from  fin_opr_register p where i.card_no=p.card_no ),to_date('0001-01-01','yyyy-MM-dd')) desc ";//and p.reg_date is not null
            sql = sql + sqlWhere;
            sql = "select * from ( " + sql + " ) where rownum=1";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, Patientopb.CARDNO, Patientopb.NAME);

                System.Data.DataTable dt = new System.Data.DataTable();
                //门诊患者信息
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Patientopb = new His.Models.ZZSB.Patientopr();
                    Patientopb.PATIENTID = dt.Rows[i][0].ToString();
                    Patientopb.OUTPATIENTNO = dt.Rows[i][1].ToString();
                    Patientopb.CARDSTATUS = dt.Rows[i][2].ToString();
                    Patientopb.ACCOUNTID = dt.Rows[i][3].ToString();
                    Patientopb.ACCBALANCE = dt.Rows[i][4].ToString();
                    Patientopb.NAME = dt.Rows[i][5].ToString();
                    Patientopb.SEX = dt.Rows[i][6].ToString();
                    Patientopb.AGE = dt.Rows[i][7].ToString();
                    Patientopb.IDCARDTYPE = dt.Rows[i][8].ToString();
                    Patientopb.IDCARDNO = dt.Rows[i][9].ToString();
                    Patientopb.BANKCARDNO = dt.Rows[i][10].ToString();
                    Patientopb.FEETYPE = dt.Rows[i][11].ToString();
                    Patientopb.PHONENO = dt.Rows[i][12].ToString();
                    Patientopb.NOTE = dt.Rows[i][13].ToString();
                    Patientopb.ELDERLYVOUCHERFLAG = dt.Rows[i][14].ToString();
                    al.Add(Patientopb);
                }

                al = this.ValidRegRealName(al);
                return al;
                #endregion
            }
            catch
            {
                return null;
            }
        }

        private System.Collections.ArrayList GetPatientopbDataForEmployee(His.Models.ZZSB.Patientopr Patientopb)
        {
            #region sql
            string sql = @"select * from ( select * from 
( select i.card_no patientid, --患者ID号
       i.ic_cardno outpatientno, --门诊号
       '1' cardstatus, --卡状态
       null accountid, --预交金帐户ID
       null accbalance, --预交金帐户余额 
       t.empl_name, --职工姓名
       decode(t.sex_code, 'F', 1, 'M', 0, 2) sex, --性别
       fun_get_age(t.birthday) age, --年龄
       null idcardtype, --证件类型
       t.idenno idcardno, --身份号码
       null bankcardno, --银行卡号
       '99' feetype, --患者费别
       nvl(i.home_tel, '0') phoneno, --电话号码
       nvl(i.mark, '无') note, --备用
			 nvl((select max(p.reg_date) from  fin_opr_register p where i.card_no=p.card_no ),to_date('0001-01-01','yyyy-MM-dd')) as reg_date --最新挂号时间
  from com_employee t, com_patientinfo i
 where t.idenno = i.idenno
   and t.empl_name = i.name
   and (t.valid_state = '1' or t.empstate in ('10','05'))
   and t.idenno = '{0}'
   and t.empl_name = '{1}'
   and i.card_no not like '99%'
	 and i.card_no not like '10%'
	 and not regexp_like(i.card_no, '[[:alpha:]]')
union all
select ii.card_no patientid, --患者ID号
       ii.ic_cardno outpatientno, --门诊号
       '1' cardstatus, --卡状态
       null accountid, --预交金帐户ID
       null accbalance, --预交金帐户余额 
       x.empl_name, --职工姓名
       decode(x.sex, '女', 1, '男', 0, 2) sex, --性别
       fun_get_age(x.birthday) age, --年龄
       null idcardtype, --证件类型
       x.idenno idcardno, --身份号码
       null bankcardno, --银行卡号
       '99' feetype, --患者费别
       nvl(ii.home_tel, '0') phoneno, --电话号码
       nvl(ii.mark, '无') note, --备用
			 nvl((select max(p.reg_date) from  fin_opr_register p where ii.card_no=p.card_no ),to_date('0001-01-01','yyyy-MM-dd')) as reg_date --最新挂号时间
 from com_employee_tx x, com_patientinfo ii
 where x.idenno = ii.idenno
   and x.empl_name = ii.name
   and x.idenno = '{0}'
   and x.empl_name = '{1}'
   and ii.card_no not like '99%'
	 and ii.card_no not like '10%'
	 and not regexp_like(ii.card_no, '[[:alpha:]]')
) order by reg_date desc ) where rownum=1";

            //            if (Patientopb.CARDTYPECODE == "2")
            //            {
            //                sqlWhere = @" WHERE i.idenno = '{0}' and i.name= '{1}' and rownum=1 and i.card_no not like'99%'
            //                      and i.card_no not like'10%' ";
            //            }
            //            else //if (Patientopb.IDCARDTYPE = "1")
            //            {
            //                sqlWhere = @" where (i.card_no = '{0}' or t.markno = '{0}')";
            //            }
            //sql = sql + sqlWhere;
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, Patientopb.CARDNO, Patientopb.NAME);

                System.Data.DataTable dt = new System.Data.DataTable();
                //门诊患者信息
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Patientopb = new His.Models.ZZSB.Patientopr();
                    Patientopb.PATIENTID = dt.Rows[i][0].ToString();
                    Patientopb.OUTPATIENTNO = dt.Rows[i][1].ToString();
                    Patientopb.CARDSTATUS = dt.Rows[i][2].ToString();
                    Patientopb.ACCOUNTID = dt.Rows[i][3].ToString();
                    Patientopb.ACCBALANCE = dt.Rows[i][4].ToString();
                    Patientopb.NAME = dt.Rows[i][5].ToString();
                    Patientopb.SEX = dt.Rows[i][6].ToString();
                    Patientopb.AGE = dt.Rows[i][7].ToString();
                    Patientopb.IDCARDTYPE = dt.Rows[i][8].ToString();
                    Patientopb.IDCARDNO = dt.Rows[i][9].ToString();
                    Patientopb.BANKCARDNO = dt.Rows[i][10].ToString();
                    Patientopb.FEETYPE = dt.Rows[i][11].ToString();
                    Patientopb.PHONENO = dt.Rows[i][12].ToString();
                    Patientopb.NOTE = dt.Rows[i][13].ToString();
                    al.Add(Patientopb);
                }

                al = this.ValidRegRealName(al);
                return al;
                #endregion
            }
            catch
            {
                return null;
            }
        }



        private System.Collections.ArrayList ValidRegRealName(System.Collections.ArrayList al)
        {
            try
            {
                if (al.Count > 0)
                {
                    foreach (His.Models.ZZSB.Patientopr item in al)
                    {
                        #region 实名挂号限制  超3次非实名不允许挂号 chengym 170321

                        bool isCheckRealName = false;
                        DateTime dtBegin = System.DateTime.MinValue;

                        bool isChild = true;//小于1岁的儿童

                        RegisterManager mgr = new RegisterManager();
                        Neusoft.FrameWork.Models.NeuObject conInfo = mgr.GetConstant("realnameregbegindate", "1");
                        if (conInfo != null && conInfo.Memo != "")
                        {
                            try
                            {
                                dtBegin = Neusoft.FrameWork.Function.NConvert.ToDateTime(conInfo.Memo);
                                isCheckRealName = true;
                            }
                            catch
                            {
                                isCheckRealName = false;
                            }


                            //可能需要对儿童进行放开
                            if (item.AGE.Contains("岁"))
                            {

                                string year = item.AGE.Substring(0, item.AGE.IndexOf("岁"));

                                if (string.IsNullOrEmpty(year))
                                {
                                    isChild = false;
                                }
                                else
                                {
                                    int i;
                                    int.TryParse(year, out i);
                                    if (i > 1)
                                        isChild = false;
                                    else
                                        isChild = true;
                                }
                            }
                            else
                            {
                                isChild = true;
                            }

                        }
                        if (isCheckRealName && !isChild)
                        {
                            string cardNo = item.CARDNO;
                            string IdCard = item.IDCARDNO;
                            if (string.IsNullOrEmpty(IdCard) || (IdCard.Trim().Length != 18 && IdCard.Trim().Length != 15))
                            {
                                int times = this.QueryRegiterByCardNOAndDtBegin(cardNo, dtBegin);
                                if (times < 3)
                                {
                                    item.NOTE = "Y";
                                }
                                else
                                {
                                    item.NOTE = "N";
                                }
                            }
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Shadow.Util.Data.Func.Log.WriteLog("zzsb", ex.Message);
            }

            return al;
        }


        int QueryRegiterByCardNOAndDtBegin(string cardNO, DateTime dtBegin)
        {
            string sql = string.Empty;
            sql = @"select sum(reg_times) times  from (
select case 
   when   length(p.idenno)=18  then 0
   when    length(p.idenno)=15 then 0 
   when p.idcardtype<>'01' and p.idenno is not null  then  0 
   when p.idcardtype<>'01' and p.idenno is not null  then  0 
   when r.trans_type='1' and p.idcardtype='01' and length(p.idenno)<>18 and length(p.idenno)<>15  then  1 
   when r.trans_type='2' and p.idcardtype='01' and length(p.idenno)<>18 and length(p.idenno)<>15  then  -1 
   when  r.trans_type='1' and  p.idenno is null   then 1
   when  r.trans_type='2' and  p.idenno is null   then -1
  else 1   end reg_times
    ,p.idenno,
r.* from fin_opr_register r ,com_patientinfo p 
where r.card_no='{0}' and r.card_no=p.card_no
and r.reg_date>to_date('{1}','yyyy-mm-dd HH24:mi:ss')
) ";
            int result = 0;
            try
            {
                sql = string.Format(sql, cardNO, dtBegin.Date.ToString());
                RegisterManager mgr = new RegisterManager();
                result = Neusoft.FrameWork.Function.NConvert.ToInt32(mgr.ExecSqlReturnOne(sql));
            }
            catch (Exception ex)
            {
                // this.Err = "出错" + e.Message;
                Shadow.Util.Data.Func.Log.WriteLog("zzsb", ex.Message);
                return -1;

            }


            return result;
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

        private string GetPatientopbXML(System.Collections.ArrayList al)
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


                //His.Models.ZZSB.Patientopr p = al[0] as His.Models.ZZSB.Patientopr;
                foreach (His.Models.ZZSB.Patientopr p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement CARDNO = xml.CreateElement("CARDNO");
                    CARDNO.InnerText = p.CARDNO;
                    Result.AppendChild(CARDNO);

                    System.Xml.XmlElement PATIENTID = xml.CreateElement("PATIENTID");
                    PATIENTID.InnerText = p.PATIENTID;
                    Result.AppendChild(PATIENTID);

                    System.Xml.XmlElement OUTPATIENTNO = xml.CreateElement("OUTPATIENTNO");
                    OUTPATIENTNO.InnerText = p.OUTPATIENTNO;
                    Result.AppendChild(OUTPATIENTNO);

                    System.Xml.XmlElement CARDSTATUS = xml.CreateElement("CARDSTATUS");
                    CARDSTATUS.InnerText = p.CARDSTATUS;
                    Result.AppendChild(CARDSTATUS);

                    System.Xml.XmlElement ACCOUNTID = xml.CreateElement("ACCOUNTID");
                    ACCOUNTID.InnerText = p.ACCOUNTID;
                    Result.AppendChild(ACCOUNTID);

                    System.Xml.XmlElement ACCBALANCE = xml.CreateElement("ACCBALANCE");
                    ACCBALANCE.InnerText = p.ACCBALANCE;
                    Result.AppendChild(ACCBALANCE);

                    System.Xml.XmlElement NAME = xml.CreateElement("NAME");
                    NAME.InnerText = p.NAME;
                    Result.AppendChild(NAME);

                    System.Xml.XmlElement SEX = xml.CreateElement("SEX");
                    SEX.InnerText = p.SEX;
                    Result.AppendChild(SEX);

                    System.Xml.XmlElement AGE = xml.CreateElement("AGE");
                    AGE.InnerText = p.AGE;
                    Result.AppendChild(AGE);

                    System.Xml.XmlElement IDCARDTYPE = xml.CreateElement("IDCARDTYPE");
                    IDCARDTYPE.InnerText = p.IDCARDTYPE;
                    Result.AppendChild(IDCARDTYPE);

                    System.Xml.XmlElement IDCARDNO = xml.CreateElement("IDCARDNO");
                    IDCARDNO.InnerText = p.IDCARDNO;
                    Result.AppendChild(IDCARDNO);

                    System.Xml.XmlElement BANKCARDNO = xml.CreateElement("BANKCARDNO");
                    BANKCARDNO.InnerText = p.BANKCARDNO;
                    Result.AppendChild(BANKCARDNO);

                    System.Xml.XmlElement FEETYPE = xml.CreateElement("FEETYPE");
                    FEETYPE.InnerText = p.FEETYPE;
                    Result.AppendChild(FEETYPE);

                    System.Xml.XmlElement PHONENO = xml.CreateElement("PHONENO");
                    PHONENO.InnerText = p.PHONENO;
                    Result.AppendChild(PHONENO);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                    System.Xml.XmlElement ELDERLYVOUCHERFLAG = xml.CreateElement("ELDERLYVOUCHERFLAG");
                    ELDERLYVOUCHERFLAG.InnerText = p.ELDERLYVOUCHERFLAG;
                    Result.AppendChild(ELDERLYVOUCHERFLAG);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutPatientModel(string xml, ref His.Models.ZZSB.Patientopr opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.Patientopr();
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

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();

            }

            try
            {
                System.Xml.XmlNodeList IsShield = doc.GetElementsByTagName("IsShield");
                System.Xml.XmlNode IsShield1 = IsShield[0];
                if (!string.IsNullOrEmpty(IsShield1.InnerText))
                {
                    opa.IsShield = IsShield1.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "是否屏蔽99卡号不能为空！";
                    return this.ReturnFailure();

                }
            }
            catch (Exception ex)
            {
                opa.IsShield = "1";

            }



            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList CARDTYPECODE1 = doc.GetElementsByTagName("CardTypeCode");
            System.Xml.XmlNode CARDTYPECODE = CARDTYPECODE1[0];
            if (!string.IsNullOrEmpty(CARDTYPECODE.InnerText))
            {
                opa.CARDTYPECODE = CARDTYPECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡类型不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
                if (opa.CARDNO == "0000000000")
                {
                    this.resultCode = "0";
                    this.msg = "非正常门诊号" + opa.CARDNO;
                    return this.ReturnFailure();
                }
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList NAME1 = doc.GetElementsByTagName("Name");
            System.Xml.XmlNode NAME = NAME1[0];
            if (!string.IsNullOrEmpty(NAME.InnerText))
            {
                opa.NAME = NAME.InnerText;
            }
            //else
            //{
            //    this.resultCode = "0";
            //    this.msg = "卡号不能为空！";
            //    return this.ReturnFailure();
            //}

            return returnStr;
        }

        public string GetOutPatientInfoForZZSB(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.Patientopr opa = new His.Models.ZZSB.Patientopr();
            returnStr = this.GetOutPatientModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetPatientopbData(opa);
            returnStr = this.GetPatientopbXML(al);
            return returnStr;
        }
        /// <summary>
        /// 查询本院职工信息
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string GetEmployeeInfoForZZSB(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.Patientopr opa = new His.Models.ZZSB.Patientopr();
            returnStr = this.GetOutPatientModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetPatientopbDataForEmployee(opa);
            returnStr = this.GetPatientopbXML(al);
            return returnStr;
        }


        private string GetReqModelForDZPZxml(string xml, ref GDSI.MedicalOutpatientService.PersonRequestModel reqModel)
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
            System.Xml.XmlNodeList MdtrtCertNo1 = doc.GetElementsByTagName("MdtrtCertNo");
            System.Xml.XmlNode MdtrtCertNo = MdtrtCertNo1[0];
            if (!string.IsNullOrEmpty(MdtrtCertNo.InnerText))
            {
                reqModel.MdtrtCertNo = MdtrtCertNo.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "就诊凭证编号不能为空！";
                return this.ReturnFailure();

            }

            System.Xml.XmlNodeList MdtrtCertType1 = doc.GetElementsByTagName("MdtrtCertType");
            System.Xml.XmlNode MdtrtCertType = MdtrtCertType1[0];
            if (!string.IsNullOrEmpty(MdtrtCertType.InnerText))
            {
                reqModel.MdtrtCertType = MdtrtCertType.InnerText;
                if (!"01,02,03".Contains(reqModel.MdtrtCertType))
                {
                    this.resultCode = "0";
                    this.msg = "就诊凭证类型不规范！";
                    return this.ReturnFailure();
                }
            }
            else
            {
                this.resultCode = "0";
                this.msg = "就诊凭证类型不能为空！";
                return this.ReturnFailure();

            }

            System.Xml.XmlNodeList CardSN1 = doc.GetElementsByTagName("CardSN");
            System.Xml.XmlNode CardSN = CardSN1[0];
            if (!string.IsNullOrEmpty(CardSN.InnerText))
            {
                reqModel.CardSN = CardSN.InnerText;

            }

            System.Xml.XmlNodeList BegnTime1 = doc.GetElementsByTagName("BegnTime");
            System.Xml.XmlNode BegnTime = BegnTime1[0];
            if (!string.IsNullOrEmpty(BegnTime.InnerText))
            {
                reqModel.BegnTime = BegnTime.InnerText;

            }
            System.Xml.XmlNodeList PsnCertType1 = doc.GetElementsByTagName("PsnCertType");
            System.Xml.XmlNode PsnCertType = PsnCertType1[0];
            if (!string.IsNullOrEmpty(PsnCertType.InnerText))
            {
                reqModel.PsnCertType = PsnCertType.InnerText;

            }
            System.Xml.XmlNodeList CertNo1 = doc.GetElementsByTagName("CertNo");
            System.Xml.XmlNode CertNo = CertNo1[0];
            if (!string.IsNullOrEmpty(CertNo.InnerText))
            {
                reqModel.CertNo = CertNo.InnerText;

            }
            System.Xml.XmlNodeList PsnName1 = doc.GetElementsByTagName("PsnName");
            System.Xml.XmlNode PsnName = PsnName1[0];
            if (!string.IsNullOrEmpty(PsnName.InnerText))
            {
                reqModel.PsnName = PsnName.InnerText;

            }
            return "";
        }

        public string GetPersonMedicalInfoForDZPZ(string reqxml)
        {
            string returnStr = "";
            GDSI.MedicalOutpatientService.PersonRequestModel reqModel = new GDSI.MedicalOutpatientService.PersonRequestModel();
            returnStr = this.GetReqModelForDZPZxml(reqxml, ref reqModel);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }

            GDSI.CountryMedical.Service.MedicalOutService medicalOutService = new GDSI.CountryMedical.Service.MedicalOutService();
            PersonResponseModel repModel = medicalOutService.CallMedicalApi<PersonRequestModel, PersonResponseModel>(reqModel, EnumCallAPIChannel.ZDWY_ZZJ_GH, EnumMedicalApiInfNo.API1101);
            if (!repModel.IsMedicalAPISucess())
            {
                this.resultCode = "0";
                this.msg = medicalOutService.errMsg;
                return this.ReturnFailure();
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

            System.Xml.XmlElement PsnNo = xml.CreateElement("PsnNo");
            PsnNo.InnerText = repModel.BaseInfo.PsnNo;
            Result.AppendChild(PsnNo);

            System.Xml.XmlElement PsnName = xml.CreateElement("PsnName");
            PsnName.InnerText = repModel.BaseInfo.PsnName;
            Result.AppendChild(PsnName);

            System.Xml.XmlElement Certno = xml.CreateElement("Certno");
            Certno.InnerText = repModel.BaseInfo.Certno;
            Result.AppendChild(Certno);

            return xml.InnerXml.ToString();
        }

        public string PerSonMZGJRecord(string reqxml)
        {
            string returnStr = "";
            GDSI.MedicalOutpatientService.PersonRequestModel reqModel = new GDSI.MedicalOutpatientService.PersonRequestModel();
            returnStr = this.GetReqModelForPerSonMZGJRecordxml(reqxml, ref reqModel);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            GDSI.CountryMedical.Service.MedicalOutService medicalOutService = new GDSI.CountryMedical.Service.MedicalOutService();
            PersonResponseModel outModel1101 = medicalOutService.CallMedicalApi<PersonRequestModel, PersonResponseModel>(reqModel, EnumCallAPIChannel.ZDWY_ZZJ_GH, EnumMedicalApiInfNo.API1101);
            if (!outModel1101.IsMedicalAPISucess())
            {
                this.resultCode = "0";
                this.msg = medicalOutService.errMsg;
                return this.ReturnFailure();
            }
            PersonRecordRequestModel InModel2505 = new PersonRecordRequestModel();
            InModel2505.InsuplcAdmdvs = reqModel.InsuplcAdmdvs;
            InModel2505.Data = new PersonRecordRQ();
            InModel2505.Data.PsnNo = outModel1101.BaseInfo.PsnNo;
            QueryDAL queryDB = new QueryDAL();
            var sysdate = queryDB.GetSysDate("yyyy-MM-dd");
            var patient = queryDB.GetPatientInfo(reqModel.CertNo);
            if (patient != null)
            {
                InModel2505.Data.Tel = patient.HOME_TEL;
                InModel2505.Data.Addr = patient.HOME;
            }
            else
            {
                InModel2505.Data.Tel = "-";
                InModel2505.Data.Addr = "-";
            }

            InModel2505.Data.BizAppyType = "99";
            InModel2505.Data.Begndate = sysdate;
            InModel2505.Data.Enddate = "2099-12-31";
            InModel2505.Data.AgnterName = "";
            InModel2505.Data.AgnterCertType = "";
            InModel2505.Data.AgnterCertno = "";
            InModel2505.Data.AgnterTel = "";
            InModel2505.Data.Insutype = "310";
            InModel2505.Data.AgnterRlts = "";
            InModel2505.Data.FixSrtNo = "1";
            InModel2505.Data.FixmedinsCode = "H44040200001";
            InModel2505.Data.FixmedinsName = "中山大学附属第五医院";
            InModel2505.Data.Memo = "";
            var OutModel2505 = medicalOutService.CallMedicalApi<PersonRecordRequestModel, PersonRecordResponseModel>(InModel2505, EnumCallAPIChannel.ZDWY_ZZJ_GH, EnumMedicalApiInfNo.API2505);
            if (!OutModel2505.IsMedicalAPISucess())
            {
                this.resultCode = "0";
                this.msg = medicalOutService.errMsg;
                if (medicalOutService.errMsg.Contains("需先登记门诊统筹机构"))
                {
                    this.msg = "您好，系统提示您尚未办理珠海门诊统筹定点，请先在珠海社保掌上办、粤医保微信小程序上做好门诊统筹定点登记，再选定我院为职工医保门诊共济定点，感谢您的配合！";
                }
                if (medicalOutService.errMsg.Contains("已经成功办理一次此病种业务"))
                {
                    this.msg = "您好，系统提示您近期已办理过门诊共济定点，若您需变更到我院，请移步到我院门诊大厅任一收费和医保窗口办理！如需查询已定点医疗机构名称可进入微信小程序“粤医保”或“珠海社保掌上办”查询，感谢您的配合！";
                }
                if (medicalOutService.errMsg.Contains("门诊共济只支持职工险种登记"))
                {
                    this.msg = "门诊共济只支持职工险种登记.";
                }

                return this.ReturnFailure();
            }
            GDSI.ZhuHaiSI.Model.PersonRecordModel model = new GDSI.ZhuHaiSI.Model.PersonRecordModel();
            model.PsnNo = outModel1101.BaseInfo.PsnNo;
            model.PsnName = outModel1101.BaseInfo.PsnName;
            model.CertNo = reqModel.CertNo;
            model.Tel = InModel2505.Data.Tel;
            model.Addr = InModel2505.Data.Addr;
            model.BizAppyType = "99";
            model.Begndate = InModel2505.Data.Begndate;
            model.Enddate = InModel2505.Data.Enddate;
            model.AgnterName = "";
            model.AgnterCertType = "";
            model.AgnterCertno = "";
            model.AgnterTel = "";
            //model.AgnterAddr = this.dateTimePickerAgnterAddr.Value.ToString("yyyy-MM-dd"); ;//本身为代办人地址 后续创智改成了预产期
            model.AgnterRlts = "";
            model.FixSrtNo = InModel2505.Data.FixSrtNo;
            model.Valid = "1";
            model.Memo = "";
            model.FixmedinsCode = InModel2505.Data.FixmedinsCode;
            model.FixmedinsName = InModel2505.Data.FixmedinsName;
            model.OpterType = "3";
            model.OpterCode = "00W999";
            model.OpterName = "自助机";
            model.TrtDclaDetlSn = OutModel2505.Result.TrtDclaDetlSn;
            GDSI.ZhuHaiSI.DB.DBFunction db = new GDSI.ZhuHaiSI.DB.DBFunction();
            if (db.InsertPersonRecord(model) < 0)
            {
                this.resultCode = "0";
                this.msg = "插入人员备案信息表失败:" + db.ErrorMessage;
                return this.ReturnFailure();
            }
            #region 返回串
            returnStr = string.Empty;
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
            returnStr = xml.InnerXml.ToString();
            #endregion
            return returnStr;
        }
        private string GetReqModelForPerSonMZGJRecordxml(string xml, ref GDSI.MedicalOutpatientService.PersonRequestModel reqModel)
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
            System.Xml.XmlNodeList MdtrtCertNo1 = doc.GetElementsByTagName("MdtrtCertNo");
            System.Xml.XmlNode MdtrtCertNo = MdtrtCertNo1[0];
            if (!string.IsNullOrEmpty(MdtrtCertNo.InnerText))
            {
                reqModel.MdtrtCertNo = MdtrtCertNo.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "就诊凭证编号不能为空！";
                return this.ReturnFailure();

            }

            System.Xml.XmlNodeList MdtrtCertType1 = doc.GetElementsByTagName("MdtrCertTyp");
            System.Xml.XmlNode MdtrtCertType = MdtrtCertType1[0];
            if (!string.IsNullOrEmpty(MdtrtCertType.InnerText))
            {
                reqModel.MdtrtCertType = MdtrtCertType.InnerText;
                if (!"01,02,03".Contains(reqModel.MdtrtCertType))
                {
                    this.resultCode = "0";
                    this.msg = "就诊凭证类型不规范！";
                    return this.ReturnFailure();
                }
            }
            else
            {
                this.resultCode = "0";
                this.msg = "就诊凭证类型不能为空！";
                return this.ReturnFailure();

            }

            System.Xml.XmlNodeList CardSN1 = doc.GetElementsByTagName("CardSN");
            System.Xml.XmlNode CardSN = CardSN1[0];
            if (!string.IsNullOrEmpty(CardSN.InnerText))
            {
                reqModel.CardSN = CardSN.InnerText;

            }

            System.Xml.XmlNodeList PsnCertType1 = doc.GetElementsByTagName("PsnCertType");
            System.Xml.XmlNode PsnCertType = PsnCertType1[0];
            if (!string.IsNullOrEmpty(PsnCertType.InnerText))
            {
                reqModel.PsnCertType = PsnCertType.InnerText;

            }
            System.Xml.XmlNodeList CertNo1 = doc.GetElementsByTagName("CertNo");
            System.Xml.XmlNode CertNo = CertNo1[0];
            if (!string.IsNullOrEmpty(CertNo.InnerText))
            {
                reqModel.CertNo = CertNo.InnerText;

            }

            System.Xml.XmlNodeList InsuplcAdmdvs1 = doc.GetElementsByTagName("InsuplcAdmdvs");
            System.Xml.XmlNode InsuplcAdmdvs = InsuplcAdmdvs1[0];
            if (!string.IsNullOrEmpty(InsuplcAdmdvs.InnerText))
            {
                reqModel.InsuplcAdmdvs = InsuplcAdmdvs.InnerText;

            }

            return "";
        }

    }

    public class RegisterDept
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

        private System.Collections.ArrayList GetRegisterDeptData(His.Models.ZZSB.InRegisterDept RegisterDept)
        {
            #region sql
            //            string sql = @" select distinct(t.dept_code) deptcode, --科室编号
            //                   fun_get_dept_name(t.dept_code) deptname,--科室名称
            //                   null DeptFloor,--科室楼层
            //                   null description,--科室简介
            //                   null message,--科室提示信息
            //                   'N' nextflag,--是否有下级科室
            //                   rr.sort_id1,
            //                   null note --备用
            //             from fin_opr_schema t,com_department rr
            //             where t.dept_code=rr.dept_code
            //             and to_char(t.see_date,'YYYY-MM-DD')='{0}'
            //             and t.end_time>=sysdate
            //             and t.valid_flag='1'
            //             and t.room_name not like'临时%'
            //             and t.dept_code in(select rr.dept_code
            //               from com_department rr
            //              where rr.valid_state = '1'
            //                and rr.dept_code not in('6049','6013','6014','6015','6017','6019','7021','9023','6126')
            //                and rr.hos_code = 'CORE_HIS50')  
            //             and (t.reg_lmt-t.reged)>0
            //             order by rr.sort_id1
            //            ";
            #endregion
            string sql = @"select distinct(t.dept_code) deptcode, --科室编号
                   fun_get_dept_name(t.dept_code) deptname,--科室名称
                   null DeptFloor,--科室楼层
                   null description,--科室简介
                   null message,--科室提示信息
                   'N' nextflag,--是否有下级科室
                   (select p.sort_id from com_dictionary p where p.type='ZZSBDepartmentSort' and p.code=t.dept_code) as sort_id1,
                   null note --备用
             from fin_opr_schema t,com_department rr
             where t.dept_code=rr.dept_code
             and to_char(t.see_date,'YYYY-MM-DD')='{0}'
             and t.end_time>=sysdate
             and t.valid_flag='1'
             and t.room_name not like'临时%'
             and t.dept_code in(select rr.dept_code
               from com_department rr
              where rr.valid_state = '1'
                 and rr.dept_name not  like '%义诊%'
                and rr.dept_code not in('6013','6014','6015','6017','6019','7021','6126')
                and rr.dept_name not like '%义诊%'
                and rr.hos_code = 'CORE_HIS50')  
             and (t.reg_lmt-t.reged)>0
             union 
             
              select SCHEMA_DEPT_CODE deptcode, --科室编号
                   SCHEMA_DEPT_Name deptname,--科室名称
                   null DeptFloor,--科室楼层
                   null description,--科室简介
                   null message,--科室提示信息
                   'N' nextflag,--是否有下级科室
                   (select p.sort_id from com_dictionary p where p.type='ZZSBDepartmentSort' and p.code=t.dept_code) as sort_id1,
                   null note --备用
             from fin_opr_schema t,com_department rr
             where t.dept_code=rr.dept_code
             and to_char(t.see_date,'YYYY-MM-DD')='{0}'
             and t.end_time>=sysdate
             and t.valid_flag='1'
             and t.room_name not like'临时%'
             and t.dept_code in(select rr.dept_code
               from com_department rr
              where rr.valid_state = '1'
                 and rr.dept_name not  like '%义诊%'
                and rr.dept_code not in('6013','6014','6015','6017','6019','7021','6126')
                and rr.dept_name not like '%义诊%'
                and rr.hos_code = 'CORE_HIS50')  
             and (t.reg_lmt-t.reged)>0           
             and SCHEMA_DEPT_NAME is not null";

            string zzqsql = @"select distinct(t.dept_code) deptcode, --科室编号
                   fun_get_dept_name(t.dept_code) deptname,--科室名称
                   null DeptFloor,--科室楼层
                   null description,--科室简介
                   null message,--科室提示信息
                   'N' nextflag,--是否有下级科室
                   (select p.sort_id from com_dictionary p where p.type='ZZSBDepartmentSort' and p.code=t.dept_code) as sort_id1,
                   null note --备用
             from fin_opr_schema t,com_department rr
             where t.dept_code=rr.dept_code
             and to_char(t.see_date,'YYYY-MM-DD')='{0}'
             and t.end_time>=sysdate
             and t.valid_flag='1'
             and t.room_name not like'临时%'
             and t.dept_code in(select rr.dept_code
               from com_department rr
              where rr.valid_state = '1'
                 and rr.dept_name not  like '%义诊%'
                and rr.dept_code not in('6013','6014','6015','6017','6019','7021','6126')
                and rr.dept_name not like '%义诊%'
                and rr.hos_code = 'CORE_HIS50')  
AND rr.dept_code IN (SELECT d.code FROM com_dictionary d WHERE d.type = 'ELDERLYVOUCHERREGDEPT' and d.valid_state = '1')
             and (t.reg_lmt-t.reged)>0
             union 
             
              select SCHEMA_DEPT_CODE deptcode, --科室编号
                   schema_dept_name deptname,--科室名称
                   null DeptFloor,--科室楼层
                   null description,--科室简介
                   null message,--科室提示信息
                   'N' nextflag,--是否有下级科室
                   (select p.sort_id from com_dictionary p where p.type='ZZSBDepartmentSort' and p.code=t.dept_code) as sort_id1,
                   null note --备用
             from fin_opr_schema t,com_department rr
             where t.dept_code=rr.dept_code
             and to_char(t.see_date,'YYYY-MM-DD')='{0}'
             and t.end_time>=sysdate
             and t.valid_flag='1'
             and t.room_name not like'临时%'
             and t.dept_code in(select rr.dept_code
               from com_department rr
              where rr.valid_state = '1'
                 and rr.dept_name not  like '%义诊%'
                and rr.dept_code not in('6013','6014','6015','6017','6019','7021','6126')
                and rr.dept_name not like '%义诊%'
                and rr.hos_code = 'CORE_HIS50')  
             and (t.reg_lmt-t.reged)>0       
AND rr.dept_code IN (SELECT d.code FROM com_dictionary d WHERE d.type = 'ELDERLYVOUCHERREGDEPT'  and d.valid_state = '1')    
             and SCHEMA_DEPT_NAME is not null";
            try
            {
                #region 数据赋值
                System.Data.DataTable dt = new System.Data.DataTable();
                if (RegisterDept.ELDERLYVOUCHERREGDEPTFLAG == "1")
                {
                    zzqsql = string.Format(zzqsql, RegisterDept.REGDATE);
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(zzqsql);
                }
                else
                {
                    sql = string.Format(sql, RegisterDept.REGDATE);
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                }
                // His.Util.Common.HisLog.WriteLog(His.Models.Common.HisLogType.ZZSB, sql);
                //获取挂号科室
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                List<string> DEPTCODE = new List<string>();//用来判断科室是否已经加入集合
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().IndexOf(',') == -1)
                    {
                        if (DEPTCODE.IndexOf(dt.Rows[i][0].ToString()) == -1)
                        {
                            DEPTCODE.Add(dt.Rows[i][0].ToString());
                            RegisterDept = new His.Models.ZZSB.InRegisterDept();
                            RegisterDept.DEPTCODE = dt.Rows[i][0].ToString();
                            RegisterDept.DEPTNAME = dt.Rows[i][1].ToString();
                            RegisterDept.DEPTFLOOR = dt.Rows[i][2].ToString();
                            RegisterDept.DESCRIPTION = dt.Rows[i][3].ToString();
                            RegisterDept.MESSAGE = dt.Rows[i][4].ToString();
                            RegisterDept.NEXTFLAG = dt.Rows[i][5].ToString();
                            RegisterDept.NOTE = dt.Rows[i][6].ToString();
                            al.Add(RegisterDept);
                        }
                    }
                    else
                    {
                        string[] deptCodeList = dt.Rows[i][0].ToString().Split(',');
                        string[] deptNameList = dt.Rows[i][1].ToString().Split(',');
                        for (int j = 0; j < deptCodeList.Count(); j++)
                        {
                            if (DEPTCODE.IndexOf(deptCodeList[j]) == -1)
                            {
                                DEPTCODE.Add(deptCodeList[j]);
                                RegisterDept = new His.Models.ZZSB.InRegisterDept();
                                RegisterDept.DEPTCODE = deptCodeList[j];
                                RegisterDept.DEPTNAME = deptNameList[j];
                                RegisterDept.DEPTFLOOR = dt.Rows[i][2].ToString();
                                RegisterDept.DESCRIPTION = dt.Rows[i][3].ToString();
                                RegisterDept.MESSAGE = dt.Rows[i][4].ToString();
                                RegisterDept.NEXTFLAG = dt.Rows[i][5].ToString();
                                RegisterDept.NOTE = dt.Rows[i][6].ToString();
                                al.Add(RegisterDept);
                            }
                        }
                    }
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

        private string GetRegisterDeptXML(System.Collections.ArrayList al)
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

                //His.Models.ZZSB.InRegisterDept p = al[0] as His.Models.ZZSB.InRegisterDept;
                foreach (His.Models.ZZSB.InRegisterDept p in al)
                {
                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.REGDATE == "ALL" && p.DEPTCODE == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement REGDATE = xml.CreateElement("REGDATE");
                    REGDATE.InnerText = p.REGDATE;
                    Result.AppendChild(REGDATE);


                    System.Xml.XmlElement DEPTCODE = xml.CreateElement("DEPTCODE");
                    DEPTCODE.InnerText = p.DEPTCODE;
                    Result.AppendChild(DEPTCODE);

                    System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                    DEPTNAME.InnerText = p.DEPTNAME;
                    Result.AppendChild(DEPTNAME);

                    System.Xml.XmlElement DEPTFLOOR = xml.CreateElement("DEPTFLOOR");
                    DEPTFLOOR.InnerText = p.DEPTFLOOR;
                    Result.AppendChild(DEPTFLOOR);

                    System.Xml.XmlElement DESCRIPTION = xml.CreateElement("DESCRIPTION");
                    DESCRIPTION.InnerText = p.DESCRIPTION;
                    Result.AppendChild(DESCRIPTION);

                    System.Xml.XmlElement MESSAGE = xml.CreateElement("MESSAGE");
                    MESSAGE.InnerText = p.MESSAGE;
                    Result.AppendChild(MESSAGE);

                    System.Xml.XmlElement NEXTFLAG = xml.CreateElement("NEXTFLAG");
                    NEXTFLAG.InnerText = p.NEXTFLAG;
                    Result.AppendChild(NEXTFLAG);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                    System.Xml.XmlElement ELDERLYVOUCHERREGDEPTFLAG = xml.CreateElement("ELDERLYVOUCHERREGDEPTFLAG");
                    ELDERLYVOUCHERREGDEPTFLAG.InnerText = p.ELDERLYVOUCHERREGDEPTFLAG;
                    Result.AppendChild(ELDERLYVOUCHERREGDEPTFLAG);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutRegisterDeptModel(string xml, ref His.Models.ZZSB.InRegisterDept opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InRegisterDept();
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

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList CARDTYPECODE1 = doc.GetElementsByTagName("CardTypeCode");
            System.Xml.XmlNode CARDTYPECODE = CARDTYPECODE1[0];
            if (!string.IsNullOrEmpty(CARDTYPECODE.InnerText))
            {
                opa.CARDTYPECODE = CARDTYPECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡类型不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList PATIENTID1 = doc.GetElementsByTagName("PatientID");
            System.Xml.XmlNode PATIENTID = PATIENTID1[0];
            if (!string.IsNullOrEmpty(PATIENTID.InnerText))
            {
                opa.PATIENTID = PATIENTID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "患者ID不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList REGDATE1 = doc.GetElementsByTagName("RegDate");
            System.Xml.XmlNode REGDATE = REGDATE1[0];
            if (!string.IsNullOrEmpty(REGDATE.InnerText))
            {
                opa.REGDATE = REGDATE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "挂号日期输入有误";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList ELDERLYVOUCHERREGDEPTFLAG1 = doc.GetElementsByTagName("ElderlyVoucherRegDeptFlag");
            System.Xml.XmlNode ELDERLYVOUCHERREGDEPTFLAG = ELDERLYVOUCHERREGDEPTFLAG1[0];
            if (!string.IsNullOrEmpty(ELDERLYVOUCHERREGDEPTFLAG.InnerText))
            {
                opa.ELDERLYVOUCHERREGDEPTFLAG = ELDERLYVOUCHERREGDEPTFLAG.InnerText;
            }

            return returnStr;
        }

        public string GetOutPatientInfoForZZSB(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InRegisterDept opa = new His.Models.ZZSB.InRegisterDept();
            returnStr = this.GetOutRegisterDeptModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetRegisterDeptData(opa);
            returnStr = this.GetRegisterDeptXML(al);
            return returnStr;
        }
    }

    public class DoctorSchedule
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

        private System.Collections.ArrayList GetDoctorScheduleData(His.Models.ZZSB.InDoctorSchedule DoctorSchedule)
        {
            #region sql
            string sql22 = @"select * from 
            (select 
                    t.id regsourceid,--排班编号,
                    null regsourcename,--排班名称
                    decode(t.schema_type,'1',2,'0',1,0) schematype,--排班类型
                    t.reglevl_code typecode,--号类编号
                    t.reglevl_name typename,--号类描述
                    t.dept_code deptcode,--科室编号
                    t.dept_name deptname,--科室名称
                    t.doct_code doctorcode,--医生编号
                    nvl(t.doct_name,'普通号') doctorname,--医生姓名
                    (select y.remark from com_employee y where y.empl_code=t.doct_code)  specify,--医生专长
                    (select y.levl_code from com_employee y where y.empl_code=t.doct_code)  rankid,--医生级别编号
                    fun_get_levelname((select y.levl_code from com_employee y where y.empl_code=t.doct_code)) rankname,--医生级别名称
                    t.begin_time starttime,--开始时间
                    t.end_time endtime,--结束时间
                    t.noon_code sessioncode,--出诊时段编号
                    decode(t.noon_code,'1','上午','2','下午','3','晚上',0) sessionname,--出诊时段名称
                    t.REG_LMT allcount, --全部号源数
                    t.REGED outcount,--已挂号数
                    t.reg_lmt-t.reged havecount,--剩余号源数
                    --tt.reg_fee+tt.chck_fee+tt.diag_fee+tt.oth_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee+tt.diag_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee regfee,--挂号费
                    tt.chck_fee treatfee,--检查费
                    tt.diag_fee servicefee,--服务费
                    null metafee,--材料费
                    tt.oth_fee otherfee,--其它费用
                    t.room_name||'/'||(select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1') admitaddress,--候诊地点
                    null note,--备用
                    (SELECT count(*)
              FROM met_nuo_assignrecord y   --护士分诊记录表
             WHERE y.dept_code=t.dept_code
               --AND y.queue_code=t.id
               AND y.assign_flag = '1'
               and y.dept_code=t.dept_code
               and y.room_id=t.room_id
               and trunc(y.triage_date)=trunc(sysdate)
               and exists (select 1 from fin_opr_register r
               where r.clinic_code=y.clinic_code
               and r.ynsee='0'
               and r.valid_flag='1')) waitno
              from FIN_OPR_SCHEMA t,fin_opr_regfeeonpact tt
              where nvl(t.reglevl_code,1) = tt.reglevl_code
              and tt.pact_code='1'
              and t.id in(select min(t.id) from FIN_OPR_SCHEMA t where t.dept_code not in('6068','7021','6126') 
              and t.stop<>'1'
              and t.doct_code<>'850201'
              and t.room_name not like'%临时%'
              and t.end_time>=sysdate
              and t.valid_flag='1' 
              and to_char(t.see_date,'YYYY-MM-DD')='{0}'
              and t.dept_code='{1}'
              and (t.reg_lmt-t.reged)>0
              and t.append_flag<>'1'
              and t.schema_type='1'
              and to_char(t.begin_time,'hh24:mi:ss')>='08:00:00' 
              and to_char(t.begin_time,'hh24:mi:ss')<>'13:00:00'
							and not exists (select 1 from FIN_OPR_SCHEMA p where p.doct_code=t.doct_code and  (p.reg_lmt-p.reged)>0 and p.stop<>'1'
              and p.doct_code<>'850201'
              and p.room_name not like'%临时%'
              and p.end_time>=sysdate
              and p.valid_flag='1' 
							and p.dept_code not in('6068','7021','6126') 
              and to_char(p.see_date,'YYYY-MM-DD')='{0}'
              and p.dept_code='{1}'
							and t.append_flag<>'1'
              and t.schema_type='1'
              and to_char(t.begin_time,'hh24:mi:ss')>='08:00:00' 
              and to_char(t.begin_time,'hh24:mi:ss')<>'13:00:00'
							and p.begin_time<t.begin_time
							)
              --and fun_get_noon(sysdate)=t.noon_code 
              group by t.doct_code 
            )
              --order by (select j.sort_id from com_employee j where j.empl_code=t.doct_code and j.valid_state='1')
              
              union all
              
              select 
                    t.id regsourceid,--排班编号,
                    null regsourcename,--排班名称
                    decode(t.schema_type,'1',2,'0',1,0) schematype,--排班类型
                    t.reglevl_code typecode,--号类编号
                    t.reglevl_name typename,--号类描述
                    t.dept_code deptcode,--科室编号
                    t.dept_name deptname,--科室名称
                    t.doct_code doctorcode,--医生编号
                    nvl(t.doct_name,'普通号') doctorname,--医生姓名
                    (select y.remark from com_employee y where y.empl_code=t.doct_code)  specify,--医生专长
                    (select y.levl_code from com_employee y where y.empl_code=t.doct_code)  rankid,--医生级别编号
                    fun_get_levelname((select y.levl_code from com_employee y where y.empl_code=t.doct_code)) rankname,--医生级别名称
                    t.begin_time starttime,--开始时间
                    t.end_time endtime,--结束时间
                    t.noon_code sessioncode,--出诊时段编号
                    decode(t.noon_code,'1','上午','2','下午','3','晚上',0) sessionname,--出诊时段名称
                    t.REG_LMT allcount, --全部号源数
                    t.REGED outcount,--已挂号数
                    t.reg_lmt-t.reged havecount,--剩余号源数
                    --tt.reg_fee+tt.chck_fee+tt.diag_fee+tt.oth_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee+tt.diag_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee regfee,--挂号费
                    tt.chck_fee treatfee,--检查费
                    tt.diag_fee servicefee,--服务费
                    null metafee,--材料费
                    tt.oth_fee otherfee,--其它费用
                    t.room_name||'/'||(select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1') admitaddress,--候诊地点
                    null note, --备用
                    (SELECT count(*)
              FROM met_nuo_assignrecord y   --护士分诊记录表
             WHERE y.dept_code=t.dept_code
               --AND y.queue_code=t.id
                and trunc(y.triage_date)=trunc(sysdate)
               AND y.assign_flag = '1'
               and y.room_id=t.room_id
               and exists (select 1 from fin_opr_register r
               where r.clinic_code=y.clinic_code
               and r.ynsee='0'
               and r.valid_flag='1')) waitno
              from FIN_OPR_SCHEMA t,fin_opr_regfeeonpact tt
              where nvl(t.reglevl_code,1) = tt.reglevl_code
              and tt.pact_code='1'
              and t.id in(select t.id from FIN_OPR_SCHEMA t where t.dept_code not in('6068','7021','6126') 
              and t.stop<>'1'
              and t.doct_code<>'850201'
              and t.room_name not like'%临时%'
              and t.end_time>=sysdate
              and t.valid_flag='1' 
              and to_char(t.see_date,'YYYY-MM-DD')='{0}'
              and t.dept_code='{1}'
              and (t.reg_lmt-t.reged)>0
              and t.schema_type='0'
              and t.append_flag<>'1'
              --and to_char(t.begin_time,'hh24:mi:ss')>='08:00:00' 
              --and to_char(t.begin_time,'hh24:mi:ss')<>'13:00:00'
              --and fun_get_noon(sysdate)=t.noon_code
            )) aa
              order by (select j.sort_id from com_employee j where j.empl_code=aa.doctorcode and j.valid_state='1')
                        ";
            #endregion

            #region 20210108 取所有排班sql
            //            string sql = @" select * from 
            //(select 
            //        t.id regsourceid,--排班编号,
            //        null regsourcename,--排班名称
            //        decode(t.schema_type,'1',2,'0',1,0) schematype,--排班类型
            //        t.reglevl_code typecode,--号类编号
            //        t.reglevl_name typename,--号类描述
            //        t.dept_code deptcode,--科室编号
            //        t.dept_name deptname,--科室名称
            //        t.doct_code doctorcode,--医生编号
            //        nvl(t.doct_name,'普通号') doctorname,--医生姓名
            //        (select y.remark from com_employee y where y.empl_code=t.doct_code)  specify,--医生专长
            //        (select y.levl_code from com_employee y where y.empl_code=t.doct_code)  rankid,--医生级别编号
            //        fun_get_levelname((select y.levl_code from com_employee y where y.empl_code=t.doct_code)) rankname,--医生级别名称
            //        t.begin_time starttime,--开始时间
            //        t.end_time endtime,--结束时间
            //        t.noon_code sessioncode,--出诊时段编号
            //        decode(t.noon_code,'1','上午','2','下午','3','晚上',0) sessionname,--出诊时段名称
            //        t.REG_LMT allcount, --全部号源数
            //        t.REGED outcount,--已挂号数
            //        t.reg_lmt-t.reged havecount,--剩余号源数
            //        --tt.reg_fee+tt.chck_fee+tt.diag_fee+tt.oth_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
            //        tt.reg_fee+tt.diag_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
            //        tt.reg_fee regfee,--挂号费
            //        tt.chck_fee treatfee,--检查费
            //        tt.diag_fee servicefee,--服务费
            //        null metafee,--材料费
            //        tt.oth_fee otherfee,--其它费用
            //        t.room_name||'/'||(select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1') admitaddress,--候诊地点
            //        null note,--备用
            //        (SELECT count(*)
            //  FROM met_nuo_assignrecord y   --护士分诊记录表
            // WHERE y.dept_code=t.dept_code
            //   --AND y.queue_code=t.id
            //   AND y.assign_flag = '1'
            //   and y.dept_code=t.dept_code
            //   and y.room_id=t.room_id
            //   and trunc(y.triage_date)=trunc(sysdate)
            //   and exists (select 1 from fin_opr_register r
            //   where r.clinic_code=y.clinic_code
            //   and r.ynsee='0'
            //   and r.valid_flag='1')) waitno
            //  from FIN_OPR_SCHEMA t,fin_opr_regfeeonpact tt
            //  where nvl(t.reglevl_code,1) = tt.reglevl_code
            //  and tt.pact_code='1'
            //  and t.id in(select t.id from FIN_OPR_SCHEMA t where t.dept_code not in('6068','7021') 
            //  and t.stop<>'1'
            //  and t.doct_code<>'850201'
            //  and t.room_name not like'%临时%'
            //  and t.end_time>=sysdate
            //  and t.valid_flag='1' 
            //  and to_char(t.see_date,'YYYY-MM-DD')='{0}'
            //  and t.dept_code='{1}'
            //  --and (t.reg_lmt-t.reged)>0
            //  and t.append_flag<>'1'
            //  and t.schema_type='1'
            //  and to_char(t.begin_time,'hh24:mi:ss')>='08:00:00' 
            //  and to_char(t.begin_time,'hh24:mi:ss')<>'13:00:00'
            //  --and fun_get_noon(sysdate)=t.noon_code 
            //	
            //  --group by t.doct_code
            //)  
            //  --order by (select j.sort_id from com_employee j where j.empl_code=t.doct_code and j.valid_state='1')
            //  
            //  union all
            //  
            //  select 
            //        t.id regsourceid,--排班编号,
            //        null regsourcename,--排班名称
            //        decode(t.schema_type,'1',2,'0',1,0) schematype,--排班类型
            //        t.reglevl_code typecode,--号类编号
            //        t.reglevl_name typename,--号类描述
            //        t.dept_code deptcode,--科室编号
            //        t.dept_name deptname,--科室名称
            //        t.doct_code doctorcode,--医生编号
            //        nvl(t.doct_name,'普通号') doctorname,--医生姓名
            //        (select y.remark from com_employee y where y.empl_code=t.doct_code)  specify,--医生专长
            //        (select y.levl_code from com_employee y where y.empl_code=t.doct_code)  rankid,--医生级别编号
            //        fun_get_levelname((select y.levl_code from com_employee y where y.empl_code=t.doct_code)) rankname,--医生级别名称
            //        t.begin_time starttime,--开始时间
            //        t.end_time endtime,--结束时间
            //        t.noon_code sessioncode,--出诊时段编号
            //        decode(t.noon_code,'1','上午','2','下午','3','晚上',0) sessionname,--出诊时段名称
            //        t.REG_LMT allcount, --全部号源数
            //        t.REGED outcount,--已挂号数
            //        t.reg_lmt-t.reged havecount,--剩余号源数
            //        --tt.reg_fee+tt.chck_fee+tt.diag_fee+tt.oth_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
            //        tt.reg_fee+tt.diag_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
            //        tt.reg_fee regfee,--挂号费
            //        tt.chck_fee treatfee,--检查费
            //        tt.diag_fee servicefee,--服务费
            //        null metafee,--材料费
            //        tt.oth_fee otherfee,--其它费用
            //        t.room_name||'/'||(select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1') admitaddress,--候诊地点
            //        null note, --备用
            //        (SELECT count(*)
            //  FROM met_nuo_assignrecord y   --护士分诊记录表
            // WHERE y.dept_code=t.dept_code
            //   AND y.queue_code=t.id
            //   AND y.assign_flag = '1'
            //   and y.room_id=t.room_id
            //   and exists (select 1 from fin_opr_register r
            //   where r.clinic_code=y.clinic_code
            //   and r.ynsee='0'
            //   and r.valid_flag='1')) waitno
            //  from FIN_OPR_SCHEMA t,fin_opr_regfeeonpact tt
            //  where nvl(t.reglevl_code,1) = tt.reglevl_code
            //  and tt.pact_code='1'
            //  and t.id in(select t.id from FIN_OPR_SCHEMA t where t.dept_code not in('6068','7021') 
            //  and t.stop<>'1'
            //  and t.doct_code<>'850201'
            //  and t.room_name not like'%临时%'
            //  and t.end_time>=sysdate
            //  and t.valid_flag='1' 
            //  and to_char(t.see_date,'YYYY-MM-DD')='{0}'
            //  and t.dept_code='{1}'
            //  --and (t.reg_lmt-t.reged)>0
            //  and t.schema_type='0'
            //  and t.append_flag<>'1'
            //  --and to_char(t.begin_time,'hh24:mi:ss')>='08:00:00' 
            //  --and to_char(t.begin_time,'hh24:mi:ss')<>'13:00:00'
            //  --and fun_get_noon(sysdate)=t.noon_code
            //)) aa
            //  order by (select j.sort_id from com_employee j where j.empl_code=aa.doctorcode and j.valid_state='1'),starttime ";
            #endregion


            #region 20210617sql1
            string sql = @"select 
                    t.id regsourceid,--排班编号,
                    null regsourcename,--排班名称
                    decode(t.schema_type,'1',2,'0',1,0) schematype,--排班类型
                    t.reglevl_code typecode,--号类编号
                    t.reglevl_name typename,--号类描述
                    t.dept_code deptcode,--科室编号
                    t.dept_name deptname,--科室名称
                    t.doct_code doctorcode,--医生编号
                    nvl(t.doct_name,'普通号') doctorname,--医生姓名
                    (select y.remark from com_employee y where y.empl_code=t.doct_code)  specify,--医生专长
                    (select y.levl_code from com_employee y where y.empl_code=t.doct_code)  rankid,--医生级别编号
                    fun_get_levelname((select y.levl_code from com_employee y where y.empl_code=t.doct_code)) rankname,--医生级别名称
                    t.begin_time starttime,--开始时间
                    t.end_time endtime,--结束时间
                    t.noon_code sessioncode,--出诊时段编号
                    decode(t.noon_code,'1','上午','2','下午','3','晚上',0) sessionname,--出诊时段名称
                    t.REG_LMT allcount, --全部号源数
                    t.REGED outcount,--已挂号数
                    t.reg_lmt-t.reged havecount,--剩余号源数
                    --tt.reg_fee+tt.chck_fee+tt.diag_fee+tt.oth_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee+tt.diag_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee regfee,--挂号费
                    tt.chck_fee treatfee,--检查费
                    tt.diag_fee servicefee,--服务费
                    null metafee,--材料费
                    tt.oth_fee otherfee,--其它费用
                    (select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1')||' / '|| t.room_name admitaddress,--候诊地点
                    null note,--备用
                    (SELECT count(*)
              FROM met_nuo_assignrecord y   --护士分诊记录表
             WHERE y.dept_code=t.dept_code
               --AND y.queue_code=t.id
               AND y.assign_flag = '1'
               and y.dept_code=t.dept_code
               and y.room_id=t.room_id
               and trunc(y.triage_date)=trunc(sysdate)
               and exists (select 1 from fin_opr_register r
               where r.clinic_code=y.clinic_code
               and r.ynsee='0'
               and r.valid_flag='1')) waitno
              from FIN_OPR_SCHEMA t,fin_opr_regfeeonpact tt
              where nvl(t.reglevl_code,1) = tt.reglevl_code
              and tt.pact_code='1'
              and t.id in(select t.id from FIN_OPR_SCHEMA t where t.dept_code not in('6068','7021','6126') 
              and t.stop<>'1'
              and t.doct_code<>'850201'
              and t.room_name not like'%临时%'
              and t.end_time>=sysdate
              and t.valid_flag='1' 
              and to_char(t.see_date,'YYYY-MM-DD')='{0}'
              and (t.dept_code='{1}' Or instr(t.schema_dept_code,'{1}')> 0)
              --and (t.reg_lmt-t.reged)>0
              and t.append_flag<>'1'
              and t.schema_type='1'
              and to_char(t.begin_time,'hh24:mi:ss')>='08:00:00' 
              --and to_char(t.begin_time,'hh24:mi:ss')<>'13:00:00'
							/*and not exists (select 1 from FIN_OPR_SCHEMA p where p.doct_code=t.doct_code and  (p.reg_lmt-p.reged)>0 and p.stop<>'1'
              and p.doct_code<>'850201'
              and p.room_name not like'%临时%'
              and p.end_time>=sysdate
              and p.valid_flag='1' 
							and p.dept_code not in('6068','7021','6126') 
              and to_char(p.see_date,'YYYY-MM-DD')='{0}'
              and p.dept_code='{1}'
							and t.append_flag<>'1'
              and t.schema_type='1'
              and to_char(t.begin_time,'hh24:mi:ss')>='08:00:00' 
              and to_char(t.begin_time,'hh24:mi:ss')<>'13:00:00'
							and p.begin_time<t.begin_time
							)*/
              --and fun_get_noon(sysdate)=t.noon_code 
             -- group by t.doct_code 
            )";

            string zzqsql = @"select 
                    t.id regsourceid,--排班编号,
                    null regsourcename,--排班名称
                    decode(t.schema_type,'1',2,'0',1,0) schematype,--排班类型
                    t.reglevl_code typecode,--号类编号
                    t.reglevl_name typename,--号类描述
                    t.dept_code deptcode,--科室编号
                    t.dept_name deptname,--科室名称
                    t.doct_code doctorcode,--医生编号
                    nvl(t.doct_name,'普通号') doctorname,--医生姓名
                    (select y.remark from com_employee y where y.empl_code=t.doct_code)  specify,--医生专长
                    (select y.levl_code from com_employee y where y.empl_code=t.doct_code)  rankid,--医生级别编号
                    fun_get_levelname((select y.levl_code from com_employee y where y.empl_code=t.doct_code)) rankname,--医生级别名称
                    t.begin_time starttime,--开始时间
                    t.end_time endtime,--结束时间
                    t.noon_code sessioncode,--出诊时段编号
                    decode(t.noon_code,'1','上午','2','下午','3','晚上',0) sessionname,--出诊时段名称
                    t.REG_LMT allcount, --全部号源数
                    t.REGED outcount,--已挂号数
                    t.reg_lmt-t.reged havecount,--剩余号源数
                    --tt.reg_fee+tt.chck_fee+tt.diag_fee+tt.oth_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee+tt.diag_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee regfee,--挂号费
                    tt.chck_fee treatfee,--检查费
                    tt.diag_fee servicefee,--服务费
                    null metafee,--材料费
                    tt.oth_fee otherfee,--其它费用
                    (select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1')||' / '|| t.room_name admitaddress,--候诊地点
                    null note,--备用
                    (SELECT count(*)
              FROM met_nuo_assignrecord y   --护士分诊记录表
             WHERE y.dept_code=t.dept_code
               --AND y.queue_code=t.id
               AND y.assign_flag = '1'
               and y.dept_code=t.dept_code
               and y.room_id=t.room_id
               and trunc(y.triage_date)=trunc(sysdate)
               and exists (select 1 from fin_opr_register r
               where r.clinic_code=y.clinic_code
               and r.ynsee='0'
               and r.valid_flag='1')) waitno
              from FIN_OPR_SCHEMA t,fin_opr_regfeeonpact tt
              where nvl(t.reglevl_code,1) = tt.reglevl_code
              and tt.pact_code='1'
              and t.id in(select t.id from FIN_OPR_SCHEMA t where t.dept_code not in('6068','7021','6126') 
              and t.stop<>'1'
              and t.doct_code<>'850201'
              and t.room_name not like'%临时%'
              and t.end_time>=sysdate
              and t.valid_flag='1' 
              and to_char(t.see_date,'YYYY-MM-DD')='{0}'
              and (t.dept_code='{1}' Or instr(t.schema_dept_code,'{1}')> 0)
              --and (t.reg_lmt-t.reged)>0
              and t.append_flag<>'1'
              and t.schema_type='1'
              and to_char(t.begin_time,'hh24:mi:ss')>='08:00:00')
              AND t.doct_code IN (SELECT d.code FROM com_dictionary d WHERE d.type = 'ELDERLYVOUCHERDOCTOR' and d.valid_state = '1')
              AND t.reglevl_code IN ('1','2','3','4','10') ";
            #endregion

            try
            {
                #region 数据赋值
                string REGDATE = DoctorSchedule.REGDATE;
                string DEPTCODE = DoctorSchedule.DEPTCODE;
                System.Data.DataTable dt = new System.Data.DataTable();
                if (DoctorSchedule.ElderlyVoucherDoctorFlag == "1")
                {
                    zzqsql = string.Format(zzqsql, DoctorSchedule.REGDATE, DoctorSchedule.DEPTCODE);
                    //获取长者券医生排班
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(zzqsql);
                }
                else
                {
                    sql = string.Format(sql, DoctorSchedule.REGDATE, DoctorSchedule.DEPTCODE);
                    //获取医生排班
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                }

                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DoctorSchedule = new His.Models.ZZSB.InDoctorSchedule();
                    DoctorSchedule.REGSOURCEID = dt.Rows[i][0].ToString();
                    DoctorSchedule.REGSOURCENAME = dt.Rows[i][1].ToString();
                    DoctorSchedule.SCHEMATYPE = dt.Rows[i][2].ToString();
                    DoctorSchedule.TYPECODE = dt.Rows[i][3].ToString();
                    DoctorSchedule.TYPENAME = dt.Rows[i][4].ToString();
                    DoctorSchedule.DEPTCODE = dt.Rows[i][5].ToString();
                    DoctorSchedule.DEPTNAME = dt.Rows[i][6].ToString();
                    DoctorSchedule.DOCTORCODE = dt.Rows[i][7].ToString();
                    DoctorSchedule.DOCTORNAME = dt.Rows[i][8].ToString();
                    DoctorSchedule.SPECIFY = dt.Rows[i][9].ToString();
                    DoctorSchedule.RANKID = dt.Rows[i][10].ToString();
                    DoctorSchedule.RANKNAME = dt.Rows[i][11].ToString();
                    DoctorSchedule.STARTTIME = dt.Rows[i][12].ToString();
                    DoctorSchedule.ENDTIME = dt.Rows[i][13].ToString();
                    DoctorSchedule.SESSIONCODE = dt.Rows[i][14].ToString();
                    DoctorSchedule.SESSIONNAME = dt.Rows[i][15].ToString();
                    DoctorSchedule.ALLCOUNT = dt.Rows[i][16].ToString();
                    DoctorSchedule.OUTCOUNT = dt.Rows[i][17].ToString();
                    DoctorSchedule.HAVECOUNT = dt.Rows[i][18].ToString();
                    DoctorSchedule.TOTALREGFEE = dt.Rows[i][19].ToString();
                    DoctorSchedule.REGFEE = dt.Rows[i][20].ToString();
                    DoctorSchedule.TREATFEE = dt.Rows[i][21].ToString();
                    DoctorSchedule.SERVICEFEE = dt.Rows[i][22].ToString();
                    DoctorSchedule.METAFEE = dt.Rows[i][23].ToString();
                    DoctorSchedule.OTHERFEE = dt.Rows[i][24].ToString();
                    DoctorSchedule.ADMITADDRESS = dt.Rows[i][25].ToString();
                    DoctorSchedule.NOTE = dt.Rows[i][26].ToString();
                    DoctorSchedule.WAITNO = dt.Rows[i][27].ToString();
                    al.Add(DoctorSchedule);
                }

                sql = @"select 
                    t.id regsourceid,--排班编号,
                    null regsourcename,--排班名称
                    decode(t.schema_type,'1',2,'0',1,0) schematype,--排班类型
                    t.reglevl_code typecode,--号类编号
                    t.reglevl_name typename,--号类描述
                    t.dept_code deptcode,--科室编号
                    t.dept_name deptname,--科室名称
                    t.doct_code doctorcode,--医生编号
                    nvl(t.doct_name,'普通号') doctorname,--医生姓名
                    (select y.remark from com_employee y where y.empl_code=t.doct_code)  specify,--医生专长
                    (select y.levl_code from com_employee y where y.empl_code=t.doct_code)  rankid,--医生级别编号
                    fun_get_levelname((select y.levl_code from com_employee y where y.empl_code=t.doct_code)) rankname,--医生级别名称
                    t.begin_time starttime,--开始时间
                    t.end_time endtime,--结束时间
                    t.noon_code sessioncode,--出诊时段编号
                    decode(t.noon_code,'1','上午','2','下午','3','晚上',0) sessionname,--出诊时段名称
                    t.REG_LMT allcount, --全部号源数
                    t.REGED outcount,--已挂号数
                    t.reg_lmt-t.reged havecount,--剩余号源数
                    --tt.reg_fee+tt.chck_fee+tt.diag_fee+tt.oth_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee+tt.diag_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee regfee,--挂号费
                    tt.chck_fee treatfee,--检查费
                    tt.diag_fee servicefee,--服务费
                    null metafee,--材料费
                    tt.oth_fee otherfee,--其它费用
                    (select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1')||' / '|| t.room_name admitaddress,--候诊地点
-- t.room_name||'/'||(select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1') admitaddress,--候诊地点
                    null note, --备用
                    (SELECT count(*)
              FROM met_nuo_assignrecord y   --护士分诊记录表
             WHERE y.dept_code=t.dept_code
               --AND y.queue_code=t.id
                and trunc(y.triage_date)=trunc(sysdate)
               AND y.assign_flag = '1'
               and y.room_id=t.room_id
               and exists (select 1 from fin_opr_register r
               where r.clinic_code=y.clinic_code
               and r.ynsee='0'
               and r.valid_flag='1')) waitno
              from FIN_OPR_SCHEMA t,fin_opr_regfeeonpact tt
              where nvl(t.reglevl_code,1) = tt.reglevl_code
              and tt.pact_code='1'
              and t.id in(select t.id from FIN_OPR_SCHEMA t where t.dept_code not in('6068','7021','6126') 
              and t.stop<>'1'
              and t.doct_code<>'850201'
              and t.room_name not like'%临时%'
              and t.end_time>=sysdate
              and t.valid_flag='1' 
              and to_char(t.see_date,'YYYY-MM-DD')='{0}'
              and (t.dept_code='{1}' Or instr(t.schema_dept_code,'{1}')> 0)
              --and (t.reg_lmt-t.reged)>0
              and t.schema_type='0'
              and t.append_flag<>'1'
              --and to_char(t.begin_time,'hh24:mi:ss')>='08:00:00' 
              --and to_char(t.begin_time,'hh24:mi:ss')<>'13:00:00'
              --and fun_get_noon(sysdate)=t.noon_code
            )";

                zzqsql = @"select 
                    t.id regsourceid,--排班编号,
                    null regsourcename,--排班名称
                    decode(t.schema_type,'1',2,'0',1,0) schematype,--排班类型
                    t.reglevl_code typecode,--号类编号
                    t.reglevl_name typename,--号类描述
                    t.dept_code deptcode,--科室编号
                    t.dept_name deptname,--科室名称
                    t.doct_code doctorcode,--医生编号
                    nvl(t.doct_name,'普通号') doctorname,--医生姓名
                    (select y.remark from com_employee y where y.empl_code=t.doct_code)  specify,--医生专长
                    (select y.levl_code from com_employee y where y.empl_code=t.doct_code)  rankid,--医生级别编号
                    fun_get_levelname((select y.levl_code from com_employee y where y.empl_code=t.doct_code)) rankname,--医生级别名称
                    t.begin_time starttime,--开始时间
                    t.end_time endtime,--结束时间
                    t.noon_code sessioncode,--出诊时段编号
                    decode(t.noon_code,'1','上午','2','下午','3','晚上',0) sessionname,--出诊时段名称
                    t.REG_LMT allcount, --全部号源数
                    t.REGED outcount,--已挂号数
                    t.reg_lmt-t.reged havecount,--剩余号源数
                    tt.reg_fee+tt.diag_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
                    tt.reg_fee regfee,--挂号费
                    tt.chck_fee treatfee,--检查费
                    tt.diag_fee servicefee,--服务费
                    null metafee,--材料费
                    tt.oth_fee otherfee,--其它费用
                    (select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1')||' / '|| t.room_name admitaddress,--候诊地点
                    null note, --备用
                    (SELECT count(*)
              FROM met_nuo_assignrecord y   --护士分诊记录表
             WHERE y.dept_code=t.dept_code
                and trunc(y.triage_date)=trunc(sysdate)
               AND y.assign_flag = '1'
               and y.room_id=t.room_id
               and exists (select 1 from fin_opr_register r
               where r.clinic_code=y.clinic_code
               and r.ynsee='0'
               and r.valid_flag='1')) waitno
              from FIN_OPR_SCHEMA t,fin_opr_regfeeonpact tt
              where nvl(t.reglevl_code,1) = tt.reglevl_code
              and tt.pact_code='1'
              and t.id in(select t.id from FIN_OPR_SCHEMA t where t.dept_code not in('6068','7021','6126') 
              and t.stop<>'1'
              and t.doct_code<>'850201'
              and t.room_name not like'%临时%'
              and t.end_time>=sysdate
              and t.valid_flag='1' 
              and to_char(t.see_date,'YYYY-MM-DD')='{0}'
              and (t.dept_code='{1}' Or instr(t.schema_dept_code,'{1}')> 0)
              and t.schema_type='0'
              and t.append_flag<>'1')
              AND t.doct_code IN (SELECT d.code FROM com_dictionary d WHERE d.type = 'ELDERLYVOUCHERDOCTOR' and d.valid_state = '1') and 1=2";
                if (DoctorSchedule.ElderlyVoucherDoctorFlag == "1")
                {
                    zzqsql = string.Format(zzqsql, REGDATE, DEPTCODE);
                    //获取长者券医生排班
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(zzqsql);
                }
                else
                {
                    sql = string.Format(sql, REGDATE, DEPTCODE);
                    //获取医生排班
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DoctorSchedule = new His.Models.ZZSB.InDoctorSchedule();
                    DoctorSchedule.REGSOURCEID = dt.Rows[i][0].ToString();
                    DoctorSchedule.REGSOURCENAME = dt.Rows[i][1].ToString();
                    DoctorSchedule.SCHEMATYPE = dt.Rows[i][2].ToString();
                    DoctorSchedule.TYPECODE = dt.Rows[i][3].ToString();
                    DoctorSchedule.TYPENAME = dt.Rows[i][4].ToString();
                    DoctorSchedule.DEPTCODE = dt.Rows[i][5].ToString();
                    DoctorSchedule.DEPTNAME = dt.Rows[i][6].ToString();
                    DoctorSchedule.DOCTORCODE = dt.Rows[i][7].ToString();
                    DoctorSchedule.DOCTORNAME = dt.Rows[i][8].ToString();
                    DoctorSchedule.SPECIFY = dt.Rows[i][9].ToString();
                    DoctorSchedule.RANKID = dt.Rows[i][10].ToString();
                    DoctorSchedule.RANKNAME = dt.Rows[i][11].ToString();
                    DoctorSchedule.STARTTIME = dt.Rows[i][12].ToString();
                    DoctorSchedule.ENDTIME = dt.Rows[i][13].ToString();
                    DoctorSchedule.SESSIONCODE = dt.Rows[i][14].ToString();
                    DoctorSchedule.SESSIONNAME = dt.Rows[i][15].ToString();
                    DoctorSchedule.ALLCOUNT = dt.Rows[i][16].ToString();
                    DoctorSchedule.OUTCOUNT = dt.Rows[i][17].ToString();
                    DoctorSchedule.HAVECOUNT = dt.Rows[i][18].ToString();
                    DoctorSchedule.TOTALREGFEE = dt.Rows[i][19].ToString();
                    DoctorSchedule.REGFEE = dt.Rows[i][20].ToString();
                    DoctorSchedule.TREATFEE = dt.Rows[i][21].ToString();
                    DoctorSchedule.SERVICEFEE = dt.Rows[i][22].ToString();
                    DoctorSchedule.METAFEE = dt.Rows[i][23].ToString();
                    DoctorSchedule.OTHERFEE = dt.Rows[i][24].ToString();
                    DoctorSchedule.ADMITADDRESS = dt.Rows[i][25].ToString();
                    DoctorSchedule.NOTE = dt.Rows[i][26].ToString();
                    DoctorSchedule.WAITNO = dt.Rows[i][27].ToString();
                    al.Add(DoctorSchedule);
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

        private string GetDoctorScheduleXML(System.Collections.ArrayList al)
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


                //His.Models.ZZSB.DoctorSchedule p = al[0] as His.Models.ZZSB.DoctorSchedule;
                foreach (His.Models.ZZSB.InDoctorSchedule p in al)
                {
                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.REGDATE == "ALL" && p.DEPTCODE == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement REGSOURCEID = xml.CreateElement("REGSOURCEID");
                    REGSOURCEID.InnerText = p.REGSOURCEID;
                    Result.AppendChild(REGSOURCEID);

                    System.Xml.XmlElement REGSOURCENAME = xml.CreateElement("REGSOURCENAME");
                    REGSOURCENAME.InnerText = p.REGSOURCENAME;
                    Result.AppendChild(REGSOURCENAME);

                    System.Xml.XmlElement SCHEMATYPE = xml.CreateElement("SCHEMATYPE");
                    SCHEMATYPE.InnerText = p.SCHEMATYPE;
                    Result.AppendChild(SCHEMATYPE);

                    System.Xml.XmlElement TYPECODE = xml.CreateElement("TYPECODE");
                    TYPECODE.InnerText = p.TYPECODE;
                    Result.AppendChild(TYPECODE);

                    System.Xml.XmlElement TYPENAME = xml.CreateElement("TYPENAME");
                    TYPENAME.InnerText = p.TYPENAME;
                    Result.AppendChild(TYPENAME);

                    System.Xml.XmlElement DEPTCODE = xml.CreateElement("DEPTCODE");
                    DEPTCODE.InnerText = p.DEPTCODE;
                    Result.AppendChild(DEPTCODE);


                    System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                    DEPTNAME.InnerText = p.DEPTNAME;
                    Result.AppendChild(DEPTNAME);

                    System.Xml.XmlElement DOCTORCODE = xml.CreateElement("DOCTORCODE");
                    DOCTORCODE.InnerText = p.DOCTORCODE;
                    Result.AppendChild(DOCTORCODE);

                    System.Xml.XmlElement DOCTORNAME = xml.CreateElement("DOCTORNAME");
                    DOCTORNAME.InnerText = p.DOCTORNAME;
                    Result.AppendChild(DOCTORNAME);

                    System.Xml.XmlElement SPECIFY = xml.CreateElement("SPECIFY");
                    SPECIFY.InnerText = p.SPECIFY;
                    Result.AppendChild(SPECIFY);

                    System.Xml.XmlElement RANKID = xml.CreateElement("RANKID");
                    RANKID.InnerText = p.RANKID;
                    Result.AppendChild(RANKID);

                    System.Xml.XmlElement RANKNAME = xml.CreateElement("RANKNAME");
                    RANKNAME.InnerText = p.RANKNAME;
                    Result.AppendChild(RANKNAME);

                    System.Xml.XmlElement STARTTIME = xml.CreateElement("STARTTIME");
                    STARTTIME.InnerText = p.STARTTIME;
                    Result.AppendChild(STARTTIME);

                    System.Xml.XmlElement ENDTIME = xml.CreateElement("ENDTIME");
                    ENDTIME.InnerText = p.ENDTIME;
                    Result.AppendChild(ENDTIME);

                    System.Xml.XmlElement SESSIONCODE = xml.CreateElement("SESSIONCODE");
                    SESSIONCODE.InnerText = p.SESSIONCODE;
                    Result.AppendChild(SESSIONCODE);

                    System.Xml.XmlElement SESSIONNAME = xml.CreateElement("SESSIONNAME");
                    SESSIONNAME.InnerText = p.SESSIONNAME;
                    Result.AppendChild(SESSIONNAME);

                    System.Xml.XmlElement ALLCOUNT = xml.CreateElement("ALLCOUNT");
                    ALLCOUNT.InnerText = p.ALLCOUNT;
                    Result.AppendChild(ALLCOUNT);

                    System.Xml.XmlElement OUTCOUNT = xml.CreateElement("OUTCOUNT");
                    OUTCOUNT.InnerText = p.OUTCOUNT;
                    Result.AppendChild(OUTCOUNT);

                    System.Xml.XmlElement HAVECOUNT = xml.CreateElement("HAVECOUNT");
                    HAVECOUNT.InnerText = p.HAVECOUNT;
                    Result.AppendChild(HAVECOUNT);

                    System.Xml.XmlElement TOTALREGFEE = xml.CreateElement("TOTALREGFEE");
                    TOTALREGFEE.InnerText = p.TOTALREGFEE;
                    Result.AppendChild(TOTALREGFEE);

                    System.Xml.XmlElement REGFEE = xml.CreateElement("REGFEE");
                    REGFEE.InnerText = p.REGFEE;
                    Result.AppendChild(REGFEE);

                    System.Xml.XmlElement TREATFEE = xml.CreateElement("TREATFEE");
                    TREATFEE.InnerText = p.TREATFEE;
                    Result.AppendChild(TREATFEE);

                    System.Xml.XmlElement SERVICEFEE = xml.CreateElement("SERVICEFEE");
                    SERVICEFEE.InnerText = p.SERVICEFEE;
                    Result.AppendChild(SERVICEFEE);

                    System.Xml.XmlElement METAFEE = xml.CreateElement("METAFEE");
                    METAFEE.InnerText = p.METAFEE;
                    Result.AppendChild(METAFEE);

                    System.Xml.XmlElement OTHERFEE = xml.CreateElement("OTHERFEE");
                    OTHERFEE.InnerText = p.OTHERFEE;
                    Result.AppendChild(OTHERFEE);

                    System.Xml.XmlElement ADMITADDRESS = xml.CreateElement("ADMITADDRESS");
                    ADMITADDRESS.InnerText = p.ADMITADDRESS;
                    Result.AppendChild(ADMITADDRESS);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                    System.Xml.XmlElement WAITNO = xml.CreateElement("WAITNO");
                    WAITNO.InnerText = p.WAITNO;
                    Result.AppendChild(WAITNO);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutPatientModel(string xml, ref His.Models.ZZSB.InDoctorSchedule opa)
        {


            string returnStr = "";
            opa = new His.Models.ZZSB.InDoctorSchedule();
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

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList HOSPCODE1 = doc.GetElementsByTagName("HospCode");
            System.Xml.XmlNode HOSPCODE = HOSPCODE1[0];
            if (!string.IsNullOrEmpty(HOSPCODE.InnerText))
            {
                opa.HOSPCODE = HOSPCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "院区编号不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList CARDTYPECODE1 = doc.GetElementsByTagName("CardTypeCode");
            System.Xml.XmlNode CARDTYPECODE = CARDTYPECODE1[0];
            if (!string.IsNullOrEmpty(CARDTYPECODE.InnerText))
            {
                opa.CARDTYPECODE = CARDTYPECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡类型不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList PATIENTID1 = doc.GetElementsByTagName("PatientID");
            System.Xml.XmlNode PATIENTID = PATIENTID1[0];
            if (!string.IsNullOrEmpty(PATIENTID.InnerText))
            {
                opa.PATIENTID = PATIENTID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "患者ID不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList REGDATE1 = doc.GetElementsByTagName("RegDate");
            System.Xml.XmlNode REGDATE = REGDATE1[0];
            if (!string.IsNullOrEmpty(REGDATE.InnerText))
            {
                opa.REGDATE = REGDATE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "挂号日期不能为空";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEPTCODE1 = doc.GetElementsByTagName("DeptCode");
            System.Xml.XmlNode DEPTCODE = DEPTCODE1[0];
            if (!string.IsNullOrEmpty(DEPTCODE.InnerText))
            {
                opa.DEPTCODE = DEPTCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "挂号科室编号不能为空";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList ElderlyVoucherDoctorFlag1 = doc.GetElementsByTagName("ElderlyVoucherDoctorFlag");
            System.Xml.XmlNode ElderlyVoucherDoctorFlag = ElderlyVoucherDoctorFlag1[0];
            if (!string.IsNullOrEmpty(ElderlyVoucherDoctorFlag.InnerText))
            {
                opa.ElderlyVoucherDoctorFlag = ElderlyVoucherDoctorFlag.InnerText;
            }
            else
            {
                opa.ElderlyVoucherDoctorFlag = "0";
            }
            //else
            //{
            //    this.resultCode = "0";
            //    this.msg = "长者券医生标识不能为空";
            //    return this.ReturnFailure();
            //}

            return returnStr;
        }

        public string GetOutDoctorScheduleForZZSB(string xml)
        {

            string returnStr = "";
            His.Models.ZZSB.InDoctorSchedule opa = new His.Models.ZZSB.InDoctorSchedule();
            returnStr = this.GetOutPatientModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetDoctorScheduleData(opa);
            returnStr = this.GetDoctorScheduleXML(al);
            return returnStr;
        }
    }

    public class TestNetwork
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

        private System.Collections.ArrayList GetTestNetworkData(His.Models.ZZSB.InTestNetwork TestNetwork)
        {
            #region sql
            string sql = @" SELECT  
                  a.reg_fee+a.diag_fee totalregfee,--总挂号费
                  a.reg_fee regfee,--挂号费
                  a.diag_fee treatfee,--诊查费
                  null servicesfee,--服务费
                  null metafee,--材料费
                  null otherfee,--其它费用
                  b.dept_name admitaddress,--候诊地点
                  null note --备用                                
                FROM fin_opr_regfeeonpact a,fin_opr_schema b
                where a.reglevl_code = b.reglevl_code
                and a.pact_code='1' --合同单位为现金
                and b.id='{0}'
                  ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, TestNetwork.REGSOURCEID);

                System.Data.DataTable dt = new System.Data.DataTable();
                //获取挂号费用
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TestNetwork = new His.Models.ZZSB.InTestNetwork();
                    TestNetwork.TOTALREGFEE = dt.Rows[i][0].ToString();
                    TestNetwork.REGFEE = dt.Rows[i][1].ToString();
                    TestNetwork.TREATFEE = dt.Rows[i][2].ToString();
                    TestNetwork.SERVICESFEE = dt.Rows[i][3].ToString();
                    TestNetwork.METAFEE = dt.Rows[i][4].ToString();
                    TestNetwork.OTHERFEE = dt.Rows[i][5].ToString();
                    TestNetwork.ADMITADDRESS = dt.Rows[i][6].ToString();
                    TestNetwork.NOTE = dt.Rows[i][7].ToString();
                    al.Add(TestNetwork);
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

        private string GetTestNetworkXML(System.Collections.ArrayList al)
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

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);


                //His.Models.ZZSB.TestNetwork p = al[0] as His.Models.ZZSB.TestNetwork;
                foreach (His.Models.ZZSB.InTestNetwork p in al)
                {
                    if (p.REGSOURCEID == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement TOTALREGFEE = xml.CreateElement("TOTALREGFEE");
                    TOTALREGFEE.InnerText = p.TOTALREGFEE;
                    Result.AppendChild(TOTALREGFEE);

                    System.Xml.XmlElement REGFEE = xml.CreateElement("REGFEE");
                    REGFEE.InnerText = p.REGFEE;
                    Result.AppendChild(REGFEE);

                    System.Xml.XmlElement TREATFEE = xml.CreateElement("TREATFEE");
                    TREATFEE.InnerText = p.TREATFEE;
                    Result.AppendChild(TREATFEE);

                    System.Xml.XmlElement SERVICESFEE = xml.CreateElement("SERVICESFEE");
                    SERVICESFEE.InnerText = p.SERVICESFEE;
                    Result.AppendChild(SERVICESFEE);

                    System.Xml.XmlElement METAFEE = xml.CreateElement("METAFEE");
                    METAFEE.InnerText = p.METAFEE;
                    Result.AppendChild(METAFEE);

                    System.Xml.XmlElement OTHERFEE = xml.CreateElement("OTHERFEE");
                    OTHERFEE.InnerText = p.OTHERFEE;
                    Result.AppendChild(OTHERFEE);


                    System.Xml.XmlElement ADMITADDRESS = xml.CreateElement("ADMITADDRESS");
                    ADMITADDRESS.InnerText = p.ADMITADDRESS;
                    Result.AppendChild(ADMITADDRESS);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutRegisterFeeModel(string xml, ref His.Models.ZZSB.InTestNetwork opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InTestNetwork();
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

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList HOSPCODE1 = doc.GetElementsByTagName("HospCode");
            System.Xml.XmlNode HOSPCODE = HOSPCODE1[0];
            if (!string.IsNullOrEmpty(HOSPCODE.InnerText))
            {
                opa.HOSPCODE = HOSPCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "院区编号不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList CARDTYPECODE1 = doc.GetElementsByTagName("CardTypeCode");
            System.Xml.XmlNode CARDTYPECODE = CARDTYPECODE1[0];
            if (!string.IsNullOrEmpty(CARDTYPECODE.InnerText))
            {
                opa.CARDTYPECODE = CARDTYPECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡类型不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList PATIENTID1 = doc.GetElementsByTagName("PatientID");
            System.Xml.XmlNode PATIENTID = PATIENTID1[0];
            if (!string.IsNullOrEmpty(PATIENTID.InnerText))
            {
                opa.PATIENTID = PATIENTID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "患者ID不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList REGDATE1 = doc.GetElementsByTagName("RegDate");
            System.Xml.XmlNode REGDATE = REGDATE1[0];
            if (!string.IsNullOrEmpty(REGDATE.InnerText))
            {
                opa.REGDATE = REGDATE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "挂号日期不能为空";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEPTCODE1 = doc.GetElementsByTagName("DeptCode");
            System.Xml.XmlNode DEPTCODE = DEPTCODE1[0];
            if (!string.IsNullOrEmpty(DEPTCODE.InnerText))
            {
                opa.DEPTCODE = DEPTCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "挂号科室编号不能为空";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList REGSOURCEID1 = doc.GetElementsByTagName("RegSourceID");
            System.Xml.XmlNode REGSOURCEID = REGSOURCEID1[0];
            if (!string.IsNullOrEmpty(REGSOURCEID.InnerText))
            {
                opa.REGSOURCEID = REGSOURCEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "排班编号不能为空";
                return this.ReturnFailure();
            }

            return returnStr;
        }

        public string GetOutPatientInfoForZZSB(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InTestNetwork opa = new His.Models.ZZSB.InTestNetwork();
            returnStr = this.GetOutRegisterFeeModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetTestNetworkData(opa);
            returnStr = this.GetTestNetworkXML(al);
            return returnStr;
        }
    }

    public class TestNetworktwo
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


        private System.Collections.ArrayList GetTestNetworktwoData(His.Models.ZZSB.InTestNetworktwo TestNetworktwo)
        {
            #region sql
            string sql = @" select * from (
               SELECT 
               a.dept_name deptname,   --科室名称
               (select fun_get_levelname(b.levl_code) from com_employee b where b.empl_code= a.doct_code)||fun_get_employee_name(a.doct_code) regsourcename,--医生级别+医生名称
               a.room_name execlocation,   --就诊位置
               a.see_sequence waitno, --当前序号
               null currentno,--前面人数
               a.reg_date time,  --就诊日期
               null note --备注
          FROM met_nuo_assignrecord a
           where a.clinic_code='{0}'
           group by a.dept_name,a.room_name,a.see_sequence,a.reg_date,a.doct_code
           order by a.reg_date desc       
           ) where  rownum=1
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, TestNetworktwo.CARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //查询候诊队列
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TestNetworktwo = new His.Models.ZZSB.InTestNetworktwo();
                    TestNetworktwo.DEPTNAME = dt.Rows[i][0].ToString();
                    TestNetworktwo.REGSOURCENAME = dt.Rows[i][1].ToString();
                    TestNetworktwo.EXECLOCATION = dt.Rows[i][2].ToString();
                    TestNetworktwo.WAITNO = dt.Rows[i][3].ToString();
                    TestNetworktwo.CURRENTNO = dt.Rows[i][4].ToString();
                    TestNetworktwo.TIME = dt.Rows[i][5].ToString();
                    TestNetworktwo.NOTE = dt.Rows[i][6].ToString();
                    al.Add(TestNetworktwo);
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

        private string GetTestNetworktwoXML(System.Collections.ArrayList al)
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



                //His.Models.ZZSB.InTestNetworktwo p = al[0] as His.Models.ZZSB.InTestNetworktwo;
                foreach (His.Models.ZZSB.InTestNetworktwo p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL" && p.PATIENTID == "all")
                    {
                        return this.ERR();
                    }


                    System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                    DEPTNAME.InnerText = p.DEPTNAME;
                    Result.AppendChild(DEPTNAME);

                    System.Xml.XmlElement REGSOURCENAME = xml.CreateElement("REGSOURCENAME");
                    REGSOURCENAME.InnerText = p.REGSOURCENAME;
                    Result.AppendChild(REGSOURCENAME);

                    System.Xml.XmlElement EXECLOCATION = xml.CreateElement("EXECLOCATION");
                    EXECLOCATION.InnerText = p.EXECLOCATION;
                    Result.AppendChild(EXECLOCATION);

                    System.Xml.XmlElement WAITNO = xml.CreateElement("WAITNO");
                    WAITNO.InnerText = p.WAITNO;
                    Result.AppendChild(WAITNO);

                    System.Xml.XmlElement CURRENTNO = xml.CreateElement("CURRENTNO");
                    CURRENTNO.InnerText = p.CURRENTNO;
                    Result.AppendChild(CURRENTNO);

                    System.Xml.XmlElement TIME = xml.CreateElement("TIME");
                    TIME.InnerText = p.TIME;
                    Result.AppendChild(TIME);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);
                }


            #endregion

                return xml.InnerXml.ToString();
            }
        }


        private string GetOutQueryWaitingQueenModel(string xml, ref His.Models.ZZSB.InTestNetworktwo opa)
        {


            string returnStr = "";
            opa = new His.Models.ZZSB.InTestNetworktwo();
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

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();

            }



            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }

        public string GetOutPatientInfoForZZSB(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InTestNetworktwo opa = new His.Models.ZZSB.InTestNetworktwo();
            returnStr = this.GetOutQueryWaitingQueenModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetTestNetworktwoData(opa);
            returnStr = this.GetTestNetworktwoXML(al);
            return returnStr;
        }
    }

    public class TestNetworkthr
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

        private System.Collections.ArrayList GetTestNetworkthrData(His.Models.ZZSB.InTestNetworkthr TestNetworkthr)
        {
            #region sql
            string sql = @" SELECT a.dept_name deptname, --科室名称
                   fun_get_levelname(b.levl_code) || fun_get_employee_name(a.doct_code) regsourcename, --医生级别 + 医生名称 
                   a.room_name execlocation, --就诊诊室
                   a.name, --病人姓名
                   a.see_sequence currentno, --病人候诊号
                   a.see_date time, --就诊日期
                   null note --备注
              FROM met_nuo_assignrecord a, com_employee b
             where a.doct_code = b.empl_code
               and a.assign_flag <> '3'
               and trunc(a.see_date)=trunc(sysdate)
               and a.dept_code='{0}'
                        ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, TestNetworkthr.DEPTID);

                System.Data.DataTable dt = new System.Data.DataTable();
                //查询科室队列情况
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TestNetworkthr = new His.Models.ZZSB.InTestNetworkthr();
                    TestNetworkthr.DEPTNAME = dt.Rows[i][0].ToString();
                    TestNetworkthr.REGSOURCENAME = dt.Rows[i][1].ToString();
                    TestNetworkthr.EXECLOCATION = dt.Rows[i][2].ToString();
                    TestNetworkthr.NAME = dt.Rows[i][3].ToString();
                    TestNetworkthr.CURRENTNO = dt.Rows[i][4].ToString();
                    TestNetworkthr.TIME = dt.Rows[i][5].ToString();
                    TestNetworkthr.NOTE = dt.Rows[i][6].ToString();
                    al.Add(TestNetworkthr);
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

        private string GetTestNetworkthrXML(System.Collections.ArrayList al)
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

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);


                //His.Models.ZZSB.InTestNetworkthr p = al[0] as His.Models.ZZSB.InTestNetworkthr;
                foreach (His.Models.ZZSB.InTestNetworkthr p in al)
                {
                    if (p.DEPTID == "ALL")
                    {
                        return this.ERR();
                    }


                    System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                    DEPTNAME.InnerText = p.DEPTNAME;
                    Result.AppendChild(DEPTNAME);

                    System.Xml.XmlElement REGSOURCENAME = xml.CreateElement("REGSOURCENAME");
                    REGSOURCENAME.InnerText = p.REGSOURCENAME;
                    Result.AppendChild(REGSOURCENAME);

                    System.Xml.XmlElement EXECLOCATION = xml.CreateElement("EXECLOCATION");
                    EXECLOCATION.InnerText = p.EXECLOCATION;
                    Result.AppendChild(EXECLOCATION);

                    System.Xml.XmlElement NAME = xml.CreateElement("NAME");
                    NAME.InnerText = p.NAME;
                    Result.AppendChild(NAME);

                    System.Xml.XmlElement CURRENTNO = xml.CreateElement("CURRENTNO");
                    CURRENTNO.InnerText = p.CURRENTNO;
                    Result.AppendChild(CURRENTNO);

                    System.Xml.XmlElement TIME = xml.CreateElement("TIME");
                    TIME.InnerText = p.TIME;
                    Result.AppendChild(TIME);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutQueryDeptQueenModel(string xml, ref His.Models.ZZSB.InTestNetworkthr opa)
        {


            string returnStr = "";
            opa = new His.Models.ZZSB.InTestNetworkthr();
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

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList DEPTID1 = doc.GetElementsByTagName("DeptID");
            System.Xml.XmlNode DEPTID = DEPTID1[0];
            if (!string.IsNullOrEmpty(DEPTID.InnerText))
            {
                opa.DEPTID = DEPTID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "科室编码不能为空！";
                return this.ReturnFailure();
            }


            return returnStr;
        }

        public string GetOutPatientInfoForZZSB(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InTestNetworkthr opa = new His.Models.ZZSB.InTestNetworkthr();
            returnStr = this.GetOutQueryDeptQueenModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetTestNetworkthrData(opa);
            returnStr = this.GetTestNetworkthrXML(al);
            return returnStr;
        }
    }

    public class TestNetworkfor
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
            root1.AppendChild(Result);

            return xml.InnerXml.ToString();
        }

        private System.Collections.ArrayList GetTestNetworkforData(His.Models.ZZSB.InTestNetworkfor TestNetworkfor)
        {
            #region sql
            string sql = @" select l.clinic_code regid,--就诊流水号
                l.see_date regdate,--就诊日期
                null regsourceid,--号源编码
                null regsourcename,--号源名称
                l.dept_code deptid,--科室编号
                l.dept_name deptname,--科室名称
                l.doct_code doctid,--医生编号
                l.dept_name doctname,--医生姓名
                y.levl_code rankid,--医生级别编号
                fun_get_levelname(y.levl_code) rankname,--医生级别名称
                l.begin_time starttime,--开始时间
                l.end_time endtime,--结束时间
                l.own_cost totalregfee,--总挂号费
                l.reg_fee regfee,--挂号费
                l.diag_fee treatfee,--诊疗费
                null servicefee,--服务费
                l.chck_fee metfee,--检查费
                l.oth_fee otherfee,--其它费用
                null specialty,--医生特长
                l.dept_name treatlocation,--候诊地点
                l.dept_code waittreatno, --候诊编码
                l.oper_date opdatetime,--操作时间
                l.invoice_no receiptno,--发票号
                decode(l.ynsee,'1',0,'0',1,1) canrefund,--是否能退号
                l.paykind_code paytype,--支付方式
                null posid,--POS终端号
                null bankcardno,--支付的银行卡号
                to_char(l.reg_date,'YYYY-MM-DD') paydate,--支付日期
                to_char(l.reg_date,'HH24:mm:ss') paytime,--支付时间
                null batchno,--批次号
                null vouchno,--凭证号
                null referno,--参考号
                l.pay_cost payamt,--支付金额
                null bankcode,--银行代码
                null medinsuretranno,--医保交易流水号
                null medinsurestr,--医保字符串
                l.pub_cost medinsurefee,--医保支付费用
                l.pay_cost personalfee,--个人支付费用
                null note --备用
          from  com_employee y,fin_opr_register l
          where l.doct_code=y.empl_code
           and l.trans_type='1'
           and trunc(l.reg_date)=trunc(sysdate)
           and l.card_no='{0}'
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, TestNetworkfor.CARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //获取挂号记录
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TestNetworkfor = new His.Models.ZZSB.InTestNetworkfor();
                    TestNetworkfor.REGID = dt.Rows[i][0].ToString();
                    TestNetworkfor.REGDATE = dt.Rows[i][1].ToString();
                    TestNetworkfor.REGSOURCEID = dt.Rows[i][2].ToString();
                    TestNetworkfor.REGSOURCENAME = dt.Rows[i][3].ToString();
                    TestNetworkfor.DEPTID = dt.Rows[i][4].ToString();
                    TestNetworkfor.DEPTNAME = dt.Rows[i][5].ToString();
                    TestNetworkfor.DOCTID = dt.Rows[i][6].ToString();
                    TestNetworkfor.DOCTNAME = dt.Rows[i][7].ToString();
                    TestNetworkfor.RANKID = dt.Rows[i][8].ToString();
                    TestNetworkfor.RANKNAME = dt.Rows[i][9].ToString();
                    TestNetworkfor.STARTTIME = dt.Rows[i][10].ToString();
                    TestNetworkfor.ENDTIME = dt.Rows[i][11].ToString();
                    TestNetworkfor.TOTALREGFEE = dt.Rows[i][12].ToString();
                    TestNetworkfor.REGFEE = dt.Rows[i][13].ToString();
                    TestNetworkfor.TREATFEE = dt.Rows[i][14].ToString();
                    TestNetworkfor.SERVICEFEE = dt.Rows[i][15].ToString();
                    TestNetworkfor.METFEE = dt.Rows[i][16].ToString();
                    TestNetworkfor.OTHERFEE = dt.Rows[i][17].ToString();
                    TestNetworkfor.SPECIALTY = dt.Rows[i][18].ToString();
                    TestNetworkfor.TREATLOCATION = dt.Rows[i][19].ToString();
                    TestNetworkfor.WAITTREATNO = dt.Rows[i][20].ToString();
                    TestNetworkfor.OPDATETIME = dt.Rows[i][21].ToString();
                    TestNetworkfor.RECEIPTNO = dt.Rows[i][22].ToString();
                    TestNetworkfor.CANREFUND = dt.Rows[i][23].ToString();
                    TestNetworkfor.PAYTYPE = dt.Rows[i][24].ToString();
                    TestNetworkfor.POSID = dt.Rows[i][25].ToString();
                    TestNetworkfor.BANKCARDNO = dt.Rows[i][26].ToString();
                    TestNetworkfor.PAYDATE = dt.Rows[i][27].ToString();
                    TestNetworkfor.PAYTIME = dt.Rows[i][28].ToString();
                    TestNetworkfor.BATCHNO = dt.Rows[i][29].ToString();
                    TestNetworkfor.VOUCHNO = dt.Rows[i][30].ToString();
                    TestNetworkfor.REFERNO = dt.Rows[i][31].ToString();
                    TestNetworkfor.PAYAMT = dt.Rows[i][32].ToString();
                    TestNetworkfor.BANKCODE = dt.Rows[i][33].ToString();
                    TestNetworkfor.MEDINSURETRANNO = dt.Rows[i][34].ToString();
                    TestNetworkfor.MEDINSURESTR = dt.Rows[i][35].ToString();
                    TestNetworkfor.MEDINSUREFEE = dt.Rows[i][36].ToString();
                    TestNetworkfor.PERSONALFEE = dt.Rows[i][37].ToString();
                    TestNetworkfor.NOTE = dt.Rows[i][38].ToString();
                    al.Add(TestNetworkfor);
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

        private string GetTestNetworkforXML(System.Collections.ArrayList al)
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

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);


                //His.Models.ZZSB.InTestNetworkfor p = al[0] as His.Models.ZZSB.InTestNetworkfor;
                foreach (His.Models.ZZSB.InTestNetworkfor p in al)
                {
                    if (p.CARDNO == "ALL")
                    {
                        return this.ERR();
                    }


                    System.Xml.XmlElement REGID = xml.CreateElement("REGID");
                    REGID.InnerText = p.REGID;
                    Result.AppendChild(REGID);

                    System.Xml.XmlElement REGDATE = xml.CreateElement("REGDATE");
                    REGDATE.InnerText = p.REGDATE;
                    Result.AppendChild(REGDATE);

                    System.Xml.XmlElement REGSOURCEID = xml.CreateElement("REGSOURCEID");
                    REGSOURCEID.InnerText = p.REGSOURCEID;
                    Result.AppendChild(REGSOURCEID);

                    System.Xml.XmlElement REGSOURCENAME = xml.CreateElement("REGSOURCENAME");
                    REGSOURCENAME.InnerText = p.REGSOURCENAME;
                    Result.AppendChild(REGSOURCENAME);

                    System.Xml.XmlElement DEPTID = xml.CreateElement("DEPTID");
                    DEPTID.InnerText = p.DEPTID;
                    Result.AppendChild(DEPTID);

                    System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                    DEPTNAME.InnerText = p.DEPTNAME;
                    Result.AppendChild(DEPTNAME);

                    System.Xml.XmlElement DOCTID = xml.CreateElement("DOCTID");
                    DOCTID.InnerText = p.DOCTID;
                    Result.AppendChild(DOCTID);

                    System.Xml.XmlElement DOCTNAME = xml.CreateElement("DOCTNAME");
                    DOCTNAME.InnerText = p.DOCTNAME;
                    Result.AppendChild(DOCTNAME);

                    System.Xml.XmlElement RANKID = xml.CreateElement("RANKID");
                    RANKID.InnerText = p.RANKID;
                    Result.AppendChild(RANKID);

                    System.Xml.XmlElement RANKNAME = xml.CreateElement("RANKNAME");
                    RANKNAME.InnerText = p.RANKNAME;
                    Result.AppendChild(RANKNAME);

                    System.Xml.XmlElement STARTTIME = xml.CreateElement("STARTTIME");
                    STARTTIME.InnerText = p.STARTTIME;
                    Result.AppendChild(STARTTIME);


                    System.Xml.XmlElement ENDTIME = xml.CreateElement("ENDTIME");
                    ENDTIME.InnerText = p.ENDTIME;
                    Result.AppendChild(ENDTIME);

                    System.Xml.XmlElement TOTALREGFEE = xml.CreateElement("TOTALREGFEE");
                    TOTALREGFEE.InnerText = p.TOTALREGFEE;
                    Result.AppendChild(TOTALREGFEE);

                    System.Xml.XmlElement REGFEE = xml.CreateElement("REGFEE");
                    REGFEE.InnerText = p.REGFEE;
                    Result.AppendChild(REGFEE);

                    System.Xml.XmlElement TREATFEE = xml.CreateElement("TREATFEE");
                    TREATFEE.InnerText = p.TREATFEE;
                    Result.AppendChild(TREATFEE);

                    System.Xml.XmlElement SERVICEFEE = xml.CreateElement("SERVICEFEE");
                    SERVICEFEE.InnerText = p.SERVICEFEE;
                    Result.AppendChild(SERVICEFEE);

                    System.Xml.XmlElement METFEE = xml.CreateElement("METFEE");
                    METFEE.InnerText = p.METFEE;
                    Result.AppendChild(METFEE);


                    System.Xml.XmlElement OTHERFEE = xml.CreateElement("OTHERFEE");
                    OTHERFEE.InnerText = p.OTHERFEE;
                    Result.AppendChild(OTHERFEE);

                    System.Xml.XmlElement SPECIALTY = xml.CreateElement("SPECIALTY");
                    SPECIALTY.InnerText = p.SPECIALTY;
                    Result.AppendChild(SPECIALTY);

                    System.Xml.XmlElement TREATLOCATION = xml.CreateElement("TREATLOCATION");
                    TREATLOCATION.InnerText = p.TREATLOCATION;
                    Result.AppendChild(TREATLOCATION);


                    System.Xml.XmlElement WAITTREATNO = xml.CreateElement("WAITTREATNO");
                    WAITTREATNO.InnerText = p.WAITTREATNO;
                    Result.AppendChild(WAITTREATNO);

                    System.Xml.XmlElement OPDATETIME = xml.CreateElement("OPDATETIME");
                    OPDATETIME.InnerText = p.OPDATETIME;
                    Result.AppendChild(OPDATETIME);

                    System.Xml.XmlElement RECEIPTNO = xml.CreateElement("RECEIPTNO");
                    RECEIPTNO.InnerText = p.RECEIPTNO;
                    Result.AppendChild(RECEIPTNO);

                    System.Xml.XmlElement CANREFUND = xml.CreateElement("CANREFUND");
                    CANREFUND.InnerText = p.CANREFUND;
                    Result.AppendChild(CANREFUND);

                    System.Xml.XmlElement PAYTYPE = xml.CreateElement("PAYTYPE");
                    PAYTYPE.InnerText = p.PAYTYPE;
                    Result.AppendChild(PAYTYPE);

                    System.Xml.XmlElement POSID = xml.CreateElement("POSID");
                    POSID.InnerText = p.POSID;
                    Result.AppendChild(POSID);

                    System.Xml.XmlElement BANKCARDNO = xml.CreateElement("BANKCARDNO");
                    BANKCARDNO.InnerText = p.BANKCARDNO;
                    Result.AppendChild(BANKCARDNO);

                    System.Xml.XmlElement PAYDATE = xml.CreateElement("PAYDATE");
                    PAYDATE.InnerText = p.PAYDATE;
                    Result.AppendChild(PAYDATE);

                    System.Xml.XmlElement PAYTIME = xml.CreateElement("PAYTIME");
                    PAYTIME.InnerText = p.PAYTIME;
                    Result.AppendChild(PAYTIME);

                    System.Xml.XmlElement BATCHNO = xml.CreateElement("BATCHNO");
                    BATCHNO.InnerText = p.BATCHNO;
                    Result.AppendChild(BATCHNO);

                    System.Xml.XmlElement VOUCHNO = xml.CreateElement("VOUCHNO");
                    VOUCHNO.InnerText = p.VOUCHNO;
                    Result.AppendChild(VOUCHNO);

                    System.Xml.XmlElement REFERNO = xml.CreateElement("REFERNO");
                    REFERNO.InnerText = p.REFERNO;
                    Result.AppendChild(REFERNO);

                    System.Xml.XmlElement PAYAMT = xml.CreateElement("PAYAMT");
                    PAYAMT.InnerText = p.PAYAMT;
                    Result.AppendChild(PAYAMT);

                    System.Xml.XmlElement BANKCODE = xml.CreateElement("BANKCODE");
                    BANKCODE.InnerText = p.BANKCODE;
                    Result.AppendChild(BANKCODE);

                    System.Xml.XmlElement MEDINSURETRANNO = xml.CreateElement("MEDINSURETRANNO");
                    MEDINSURETRANNO.InnerText = p.MEDINSURETRANNO;
                    Result.AppendChild(MEDINSURETRANNO);

                    System.Xml.XmlElement MEDINSURESTR = xml.CreateElement("MEDINSURESTR");
                    MEDINSURESTR.InnerText = p.MEDINSURESTR;
                    Result.AppendChild(MEDINSURESTR);

                    System.Xml.XmlElement MEDINSUREFEE = xml.CreateElement("MEDINSUREFEE");
                    MEDINSUREFEE.InnerText = p.MEDINSUREFEE;
                    Result.AppendChild(MEDINSUREFEE);


                    System.Xml.XmlElement PERSONALFEE = xml.CreateElement("PERSONALFEE");
                    PERSONALFEE.InnerText = p.PERSONALFEE;
                    Result.AppendChild(PERSONALFEE);


                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutPatientModel(string xml, ref His.Models.ZZSB.InTestNetworkfor opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InTestNetworkfor();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                this.resultCode = "0";
                this.msg = "输入参数为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "就诊卡号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }

        public string GetOutPatientInfoForZZSB(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InTestNetworkfor opa = new His.Models.ZZSB.InTestNetworkfor();
            returnStr = this.GetOutPatientModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetTestNetworkforData(opa);
            returnStr = this.GetTestNetworkforXML(al);
            return returnStr;
        }
    }

    public class QueryPriceDetailForSRM
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


        private System.Collections.ArrayList GetQueryPriceDetailForSRMData(His.Models.ZZSB.InQueryPriceDetailForSRM QueryPriceDetailForSRM)
        {
            #region sql
            string sql = @" select t.drug_code itemno,--编码
                               t.trade_name itemname,--名称
                               t.class_code itemtype,--类别
                               t.min_unit dosage,--剂型
                               t.dose_unit unit,--单位
                               t.retail_price2 price,--单价
                               fun_get_company_name(t.producer_code) factory，--生产厂家
                               null note  --备用字段
                               from pha_com_baseinfo t
                        union all
                        select y.item_code itemno,--编码
                               y.item_name itemname,--名称
                               y.sys_class itemtype,--类别
                               null dosage,--剂型
                               y.stock_unit unit,--单位
                               y.unit_price price,--单价
                               null factory，--生产厂家
                               null note  --备用字段
                        from fin_com_undruginfo y
            ";
            #endregion

            try
            {
                #region 数据赋值
                //sql = string.Format(sql, QueryPriceDetailForSRM.DEVICEID);

                System.Data.DataTable dt = new System.Data.DataTable();
                //查询物价费用明细
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryPriceDetailForSRM = new His.Models.ZZSB.InQueryPriceDetailForSRM();
                    QueryPriceDetailForSRM.ITEMNO = dt.Rows[i][0].ToString();
                    QueryPriceDetailForSRM.ITEMNAME = dt.Rows[i][1].ToString();
                    QueryPriceDetailForSRM.ITEMTYPE = dt.Rows[i][2].ToString();
                    QueryPriceDetailForSRM.DOSAGE = dt.Rows[i][3].ToString();
                    QueryPriceDetailForSRM.UNIT = dt.Rows[i][4].ToString();
                    QueryPriceDetailForSRM.PRICE = dt.Rows[i][5].ToString();
                    QueryPriceDetailForSRM.FACTORY = dt.Rows[i][6].ToString();
                    QueryPriceDetailForSRM.NOTE = dt.Rows[i][7].ToString();
                    al.Add(QueryPriceDetailForSRM);
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

        private string GetQueryPriceDetailForSRMXML(System.Collections.ArrayList al)
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



                //His.Models.ZZSB.InTestNetworkSr p = al[0] as His.Models.ZZSB.InTestNetworkSr;
                foreach (His.Models.ZZSB.InQueryPriceDetailForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    //if (p.DEVICEID == "ALL")
                    //{
                    //    return this.ERR();
                    //}

                    System.Xml.XmlElement ITEMNO = xml.CreateElement("ITEMNO");
                    ITEMNO.InnerText = p.ITEMNO;
                    Result.AppendChild(ITEMNO);

                    System.Xml.XmlElement ITEMNAME = xml.CreateElement("ITEMNAME");
                    ITEMNAME.InnerText = p.ITEMNAME;
                    Result.AppendChild(ITEMNAME);

                    System.Xml.XmlElement ITEMTYPE = xml.CreateElement("ITEMTYPE");
                    ITEMTYPE.InnerText = p.ITEMTYPE;
                    Result.AppendChild(ITEMTYPE);

                    System.Xml.XmlElement DOSAGE = xml.CreateElement("DOSAGE");
                    DOSAGE.InnerText = p.DOSAGE;
                    Result.AppendChild(DOSAGE);

                    System.Xml.XmlElement UNIT = xml.CreateElement("UNIT");
                    UNIT.InnerText = p.UNIT;
                    Result.AppendChild(UNIT);

                    System.Xml.XmlElement PRICE = xml.CreateElement("PRICE");
                    PRICE.InnerText = p.PRICE;
                    Result.AppendChild(PRICE);

                    System.Xml.XmlElement FACTORY = xml.CreateElement("FACTORY");
                    FACTORY.InnerText = p.FACTORY;
                    Result.AppendChild(FACTORY);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetQueryPriceDetailForSRMModel(string xml, ref His.Models.ZZSB.InQueryPriceDetailForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InQueryPriceDetailForSRM();
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

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();

            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList ITEMCODE1 = doc.GetElementsByTagName("ItemCode");
            System.Xml.XmlNode ITEMCODE = ITEMCODE1[0];
            if (!string.IsNullOrEmpty(ITEMCODE.InnerText))
            {
                opa.ITEMCODE = ITEMCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }


            return returnStr;
        }

        public string GetQueryPriceDetailForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InQueryPriceDetailForSRM opa = new His.Models.ZZSB.InQueryPriceDetailForSRM();
            returnStr = this.GetQueryPriceDetailForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetQueryPriceDetailForSRMData(opa);
            returnStr = this.GetQueryPriceDetailForSRMXML(al);
            return returnStr;
        }
    }

    public class QueryFeeRecordForSRM
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


        private System.Collections.ArrayList GetQueryFeeRecordForSRMData(His.Models.ZZSB.InQueryFeeRecordForSRM QueryFeeRecordForSRM)
        {
            #region sql
            string sql = @" select t.item_code itemid,--费用ID
                                   t.class_code busitype,--业务类型
                                   t.pub_cost+t.pay_cost+t.own_cost itemfee,--费用金额
                                   t.exec_dpnm execdeptname,--执行科室
                                   to_date(t.reg_date) feedate,--费用发生日期
                                   null note --备注
                              from fin_opb_feedetail t
                              where t.trans_type='1'
                              and t.card_no='{0}'
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, QueryFeeRecordForSRM.CARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //查询费用记录
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryFeeRecordForSRM = new His.Models.ZZSB.InQueryFeeRecordForSRM();
                    QueryFeeRecordForSRM.ITEMID = dt.Rows[i][0].ToString();
                    QueryFeeRecordForSRM.BUSITYPE = dt.Rows[i][1].ToString();
                    QueryFeeRecordForSRM.ITEMFEE = dt.Rows[i][2].ToString();
                    QueryFeeRecordForSRM.EXECDEPTNAME = dt.Rows[i][3].ToString();
                    QueryFeeRecordForSRM.FEEDATE = dt.Rows[i][4].ToString();
                    QueryFeeRecordForSRM.NOTE = dt.Rows[i][7].ToString();
                    al.Add(QueryFeeRecordForSRM);
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

        private string GetQueryFeeRecordForSRMXML(System.Collections.ArrayList al)
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



                //His.Models.ZZSB.QueryFeeRecordForSRM p = al[0] as His.Models.ZZSB.QueryFeeRecordForSRM;
                foreach (His.Models.ZZSB.InQueryFeeRecordForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement ITEMID = xml.CreateElement("ITEMID");
                    ITEMID.InnerText = p.ITEMID;
                    Result.AppendChild(ITEMID);

                    System.Xml.XmlElement BUSITYPE = xml.CreateElement("BUSITYPE");
                    BUSITYPE.InnerText = p.BUSITYPE;
                    Result.AppendChild(BUSITYPE);

                    System.Xml.XmlElement ITEMFEE = xml.CreateElement("ITEMFEE");
                    ITEMFEE.InnerText = p.ITEMFEE;
                    Result.AppendChild(ITEMFEE);

                    System.Xml.XmlElement EXECDEPTNAME = xml.CreateElement("EXECDEPTNAME");
                    EXECDEPTNAME.InnerText = p.EXECDEPTNAME;
                    Result.AppendChild(EXECDEPTNAME);

                    System.Xml.XmlElement FEEDATE = xml.CreateElement("FEEDATE");
                    FEEDATE.InnerText = p.FEEDATE;
                    Result.AppendChild(FEEDATE);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetQueryFeeRecordForSRMModel(string xml, ref His.Models.ZZSB.InQueryFeeRecordForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InQueryFeeRecordForSRM();
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

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();

            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList FUNCODE1 = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode FUNCODE = FUNCODE1[0];
            if (!string.IsNullOrEmpty(FUNCODE.InnerText))
            {
                opa.FUNCODE = FUNCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTIME1 = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode REQTIME = REQTIME1[0];
            if (!string.IsNullOrEmpty(REQTIME.InnerText))
            {
                opa.REQTIME = REQTIME.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList REQTRACENO1 = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode REQTRACENO = REQTRACENO1[0];
            if (!string.IsNullOrEmpty(REQTRACENO.InnerText))
            {
                opa.REQTRACENO = REQTRACENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }

        public string GetQueryFeeRecordForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InQueryFeeRecordForSRM opa = new His.Models.ZZSB.InQueryFeeRecordForSRM();
            returnStr = this.GetQueryFeeRecordForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetQueryFeeRecordForSRMData(opa);
            returnStr = this.GetQueryFeeRecordForSRMXML(al);
            return returnStr;
        }
    }

    public class QueryPaidRecordForSRM
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


        private System.Collections.ArrayList GetQueryPaidRecordForSRMData(His.Models.ZZSB.InVisitRecordForSRM QueryPaidRecordForSRM)
        {
            #region sql
            string sql = @" select t.clinic_code regid,--门诊流水号
               t.reg_date regdate,--就诊日期
               r.reglevl_code rankname,--级别名称
               fun_get_dept_name(t.dept_code) deptname,--科室名称
               fun_get_employee_name(t.doct_code) doctname,--医生姓名
               sum(t.own_cost+t.pub_cost+t.pay_cost) totalfee,--总费用
               null feetype,--费别类别
               sum(t.pub_cost) favorfee,--优惠金额
               sum(t.pub_cost) medinsurefee,--社保支付金额
               sum(t.own_cost) personalfee,--自费金额
               null diagnosis,--诊断
               null note   --备用
         from met_ord_recipedetail t,fin_opr_register r--,met_cas_diagnose y
         where t.clinic_code=r.clinic_code
         --and r.clinic_code=y.inpatient_no
         and t.charge_flag='1'
         and r.card_no='{0}'
         group by t.clinic_code,r.reglevl_code,t.dept_code,t.doct_code,t.reg_date
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, QueryPaidRecordForSRM.CARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //查询已缴费就诊记录
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryPaidRecordForSRM = new His.Models.ZZSB.InVisitRecordForSRM();
                    QueryPaidRecordForSRM.REGID = dt.Rows[i][0].ToString();
                    QueryPaidRecordForSRM.REGDATE = dt.Rows[i][1].ToString();
                    QueryPaidRecordForSRM.RANKNAME = dt.Rows[i][2].ToString();
                    QueryPaidRecordForSRM.DEPTNAME = dt.Rows[i][3].ToString();
                    QueryPaidRecordForSRM.DOCTNAME = dt.Rows[i][4].ToString();
                    QueryPaidRecordForSRM.TOTALFEE = dt.Rows[i][5].ToString();
                    QueryPaidRecordForSRM.FEETYPE = dt.Rows[i][6].ToString();
                    QueryPaidRecordForSRM.FAVORFEE = dt.Rows[i][7].ToString();
                    QueryPaidRecordForSRM.MEDINSUREFEE = dt.Rows[i][8].ToString();
                    QueryPaidRecordForSRM.PERSONALFEE = dt.Rows[i][9].ToString();
                    QueryPaidRecordForSRM.DIAGNOSIS = dt.Rows[i][10].ToString();
                    QueryPaidRecordForSRM.NOTE = dt.Rows[i][11].ToString();
                    al.Add(QueryPaidRecordForSRM);
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

        private string GetQueryPaidRecordForSRMXML(System.Collections.ArrayList al)
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


                //His.Models.ZZSB.InVisitRecordForSRM p = al[0] as His.Models.ZZSB.InVisitRecordForSRM;
                foreach (His.Models.ZZSB.InVisitRecordForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement REGID = xml.CreateElement("REGID");
                    REGID.InnerText = p.REGID;
                    Result.AppendChild(REGID);

                    System.Xml.XmlElement REGDATE = xml.CreateElement("REGDATE");
                    REGDATE.InnerText = p.REGDATE;
                    Result.AppendChild(REGDATE);

                    System.Xml.XmlElement RANKNAME = xml.CreateElement("RANKNAME");
                    RANKNAME.InnerText = p.RANKNAME;
                    Result.AppendChild(RANKNAME);

                    System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                    DEPTNAME.InnerText = p.DEPTNAME;
                    Result.AppendChild(DEPTNAME);

                    System.Xml.XmlElement DOCTNAME = xml.CreateElement("DOCTNAME");
                    DOCTNAME.InnerText = p.DOCTNAME;
                    Result.AppendChild(DOCTNAME);

                    System.Xml.XmlElement TOTALFEE = xml.CreateElement("TOTALFEE");
                    TOTALFEE.InnerText = p.TOTALFEE;
                    Result.AppendChild(TOTALFEE);

                    System.Xml.XmlElement FEETYPE = xml.CreateElement("FEETYPE");
                    FEETYPE.InnerText = p.FEETYPE;
                    Result.AppendChild(FEETYPE);

                    System.Xml.XmlElement FAVORFEE = xml.CreateElement("FAVORFEE");
                    FAVORFEE.InnerText = p.FAVORFEE;
                    Result.AppendChild(FAVORFEE);

                    System.Xml.XmlElement MEDINSUREFEE = xml.CreateElement("MEDINSUREFEE");
                    MEDINSUREFEE.InnerText = p.MEDINSUREFEE;
                    Result.AppendChild(MEDINSUREFEE);

                    System.Xml.XmlElement PERSONALFEE = xml.CreateElement("PERSONALFEE");
                    PERSONALFEE.InnerText = p.PERSONALFEE;
                    Result.AppendChild(PERSONALFEE);

                    System.Xml.XmlElement DIAGNOSIS = xml.CreateElement("DIAGNOSIS");
                    DIAGNOSIS.InnerText = p.DIAGNOSIS;
                    Result.AppendChild(DIAGNOSIS);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetQueryPaidRecordForSRMModel(string xml, ref His.Models.ZZSB.InVisitRecordForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InVisitRecordForSRM();
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


            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }



        public string GetQueryPaidRecordForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InVisitRecordForSRM opa = new His.Models.ZZSB.InVisitRecordForSRM();
            returnStr = this.GetQueryPaidRecordForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetQueryPaidRecordForSRMData(opa);
            returnStr = this.GetQueryPaidRecordForSRMXML(al);
            return returnStr;
        }
    }

    public class QueryPaidDetailForSRM
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


        private System.Collections.ArrayList GetQueryPaidDetailForSRMData(His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM QueryPaidDetailForSRM)
        {
            #region sql
            string sql = @"  select t.clinic_code recipeno,--就诊记录编码
        t.reg_date recipetime,--就诊日期
        fun_get_dept_name(t.reg_dpcd) deptname,--开单科室
        fun_get_employee_name(t.doct_code) doctname,--开单医生
        t.pub_cost+t.pay_cost+t.own_cost totalfee,--总金额
        t.pay_flag payflag,--支付标记
        t.item_code itemid,--医嘱编号
        t.item_name itemname,--医嘱名称
        t.drug_flag itemtype,--医嘱类型
        t.pub_cost+t.pay_cost+t.own_cost itemtotalfee,--医嘱总金额
        null subitemid,--细项编号
        null subitemname,--细项名称
        t.specs,--规格
        t.price_unit unit,--单位
        t.qty quantity,--数量
        t.unit_price unitprice,--单价
        t.pub_cost+t.pay_cost+t.own_cost fee,--费用
        null note
 from fin_opb_feedetail t
 where t.card_no='{0}'
 and t.trans_type='1'
 and t.pay_flag='1'
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, QueryPaidDetailForSRM.CARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //查询已缴费的处方及收费项明细
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryPaidDetailForSRM = new His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM();
                    QueryPaidDetailForSRM.RECIPENO = dt.Rows[i][0].ToString();
                    QueryPaidDetailForSRM.RECIPETIME = dt.Rows[i][1].ToString();
                    QueryPaidDetailForSRM.DEPTNAME = dt.Rows[i][2].ToString();
                    QueryPaidDetailForSRM.DOCTNAME = dt.Rows[i][3].ToString();
                    QueryPaidDetailForSRM.TOTALFEE = dt.Rows[i][4].ToString();
                    QueryPaidDetailForSRM.PAYFLAG = dt.Rows[i][5].ToString();
                    QueryPaidDetailForSRM.ITEMID = dt.Rows[i][6].ToString();
                    QueryPaidDetailForSRM.ITEMNAME = dt.Rows[i][7].ToString();
                    QueryPaidDetailForSRM.ITEMTYPE = dt.Rows[i][8].ToString();
                    QueryPaidDetailForSRM.ITEMTOTALFEE = dt.Rows[i][9].ToString();
                    QueryPaidDetailForSRM.SUBITEMID = dt.Rows[i][10].ToString();
                    QueryPaidDetailForSRM.SUBITEMNAME = dt.Rows[i][11].ToString();
                    QueryPaidDetailForSRM.SPECS = dt.Rows[i][12].ToString();
                    QueryPaidDetailForSRM.UNIT = dt.Rows[i][13].ToString();
                    QueryPaidDetailForSRM.QUANTITY = dt.Rows[i][14].ToString();
                    QueryPaidDetailForSRM.UNITPRICE = dt.Rows[i][15].ToString();
                    QueryPaidDetailForSRM.FEE = dt.Rows[i][16].ToString();
                    QueryPaidDetailForSRM.NOTE = dt.Rows[i][17].ToString();
                    al.Add(QueryPaidDetailForSRM);
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

        private string GetQueryPaidDetailForSRMXML(System.Collections.ArrayList al)
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


                //His.Models.ZZSB.InVisitRecordForSRM p = al[0] as His.Models.ZZSB.InVisitRecordForSRM;
                foreach (His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement RECIPENO = xml.CreateElement("RECIPENO");
                    RECIPENO.InnerText = p.RECIPENO;
                    Result.AppendChild(RECIPENO);

                    System.Xml.XmlElement RECIPETIME = xml.CreateElement("RECIPETIME");
                    RECIPETIME.InnerText = p.RECIPETIME;
                    Result.AppendChild(RECIPETIME);

                    System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                    DEPTNAME.InnerText = p.DEPTNAME;
                    Result.AppendChild(DEPTNAME);

                    System.Xml.XmlElement DOCTNAME = xml.CreateElement("DOCTNAME");
                    DOCTNAME.InnerText = p.DOCTNAME;
                    Result.AppendChild(DOCTNAME);

                    System.Xml.XmlElement TOTALFEE = xml.CreateElement("TOTALFEE");
                    TOTALFEE.InnerText = p.TOTALFEE;
                    Result.AppendChild(TOTALFEE);

                    System.Xml.XmlElement PAYFLAG = xml.CreateElement("PAYFLAG");
                    PAYFLAG.InnerText = p.PAYFLAG;
                    Result.AppendChild(PAYFLAG);

                    System.Xml.XmlElement ITEMID = xml.CreateElement("ITEMID");
                    ITEMID.InnerText = p.ITEMID;
                    Result.AppendChild(ITEMID);

                    System.Xml.XmlElement ITEMNAME = xml.CreateElement("ITEMNAME");
                    ITEMNAME.InnerText = p.ITEMNAME;
                    Result.AppendChild(ITEMNAME);

                    System.Xml.XmlElement ITEMTYPE = xml.CreateElement("ITEMTYPE");
                    ITEMTYPE.InnerText = p.ITEMTYPE;
                    Result.AppendChild(ITEMTYPE);

                    System.Xml.XmlElement ITEMTOTALFEE = xml.CreateElement("ITEMTOTALFEE");
                    ITEMTOTALFEE.InnerText = p.ITEMTOTALFEE;
                    Result.AppendChild(ITEMTOTALFEE);

                    System.Xml.XmlElement SUBITEMID = xml.CreateElement("SUBITEMID");
                    SUBITEMID.InnerText = p.SUBITEMID;
                    Result.AppendChild(SUBITEMID);

                    System.Xml.XmlElement SUBITEMNAME = xml.CreateElement("SUBITEMNAME");
                    SUBITEMNAME.InnerText = p.SUBITEMNAME;
                    Result.AppendChild(SUBITEMNAME);

                    System.Xml.XmlElement SPECS = xml.CreateElement("SPECS");
                    SPECS.InnerText = p.SPECS;
                    Result.AppendChild(SPECS);

                    System.Xml.XmlElement UNIT = xml.CreateElement("UNIT");
                    UNIT.InnerText = p.UNIT;
                    Result.AppendChild(UNIT);

                    System.Xml.XmlElement QUANTITY = xml.CreateElement("QUANTITY");
                    QUANTITY.InnerText = p.QUANTITY;
                    Result.AppendChild(QUANTITY);

                    System.Xml.XmlElement UNITPRICE = xml.CreateElement("UNITPRICE");
                    UNITPRICE.InnerText = p.UNITPRICE;
                    Result.AppendChild(UNITPRICE);

                    System.Xml.XmlElement FEE = xml.CreateElement("FEE");
                    FEE.InnerText = p.FEE;
                    Result.AppendChild(FEE);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetQueryPaidDetailForSRMModel(string xml, ref His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM();
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


            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }



        public string GetQueryPaidDetailForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM opa = new His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM();
            returnStr = this.GetQueryPaidDetailForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetQueryPaidDetailForSRMData(opa);
            returnStr = this.GetQueryPaidDetailForSRMXML(al);
            return returnStr;
        }
    }

    public class QueryVisitRecordForSRM
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


        private System.Collections.ArrayList GetQueryVisitRecordForSRMData(His.Models.ZZSB.InVisitRecordForSRM QueryVisitRecordForSRM, System.Collections.ArrayList feelist)
        {
            #region sql
            string sql = @"  select regid,--门诊流水号
              regdate,--就诊日期
                rankname,--级别名称
               deptname,--科室名称
               doctname,--医生姓名
               (case when   sum(itemqty)>1 then sum(own_cost+pub_cost+pay_cost)-(sum(itemqty)-1)*11 else sum(own_cost+pub_cost+pay_cost) end )totalfee,--总费用
                feetype,--费别类别
               sum(pub_cost) favorfee,--优惠金额
               sum(pub_cost) medinsurefee,--社保支付金额
               (case when   sum(itemqty)>1 then  sum(own_cost)-((sum(itemqty)-1)*11)else  sum(own_cost) end)personalfee,--自费金额
               null diagnosis,--诊断
               null note,   --备用
               elderlyvoucherflag elderlyvoucherflag
                 from (
select t.clinic_code regid,--门诊流水号
               t.reg_date regdate,--就诊日期
               r.reglevl_code rankname,--级别名称
               fun_get_dept_name(t.reg_dpcd) deptname,--科室名称
               fun_get_employee_name(t.doct_code) doctname,--医生姓名
               r.pact_code    feetype,--费别类别
               t.pay_cost,
               t.pub_cost,
               t.own_cost,
               0 as itemqty,
               DECODE(r.pact_code,'258','1','0') elderlyvoucherflag
         from fin_opb_feedetail t,fin_opr_register r
         where t.clinic_code=r.clinic_code
         and t.oper_date>sysdate -(select to_number(nvl(max(dic.code), '7')) 
                                        from com_dictionary dic 
                                        where dic.type = 'ZZSB_OP_FEE_DAYS' 
                                          and dic.valid_state = '1')
         and nvl((select p.extend_flag from met_ord_recipedetail  p where p.sequence_no=t.mo_order and rownum=1),nvl(t.extend_flag,'0'))='0'
         and t.trans_type='1'
         and t.pay_flag='0'
         and r.card_no='{0}'
and     not  EXISTS  (select 1 from com_dictionary dic where  dic.TYPE='setUltrasound' and dic.code=t.package_code ) 
union all
select t.clinic_code regid,--门诊流水号
               t.reg_date regdate,--就诊日期
               r.reglevl_code rankname,--级别名称
               fun_get_dept_name(t.reg_dpcd) deptname,--科室名称
               fun_get_employee_name(t.doct_code) doctname,--医生姓名
               r.pact_code    feetype,--费别类别
               t.pay_cost,
               t.pub_cost,
               t.own_cost,
              1 as itemqty,
               DECODE(r.pact_code,'258','1','0') elderlyvoucherflag
         from fin_opb_feedetail t,fin_opr_register r
         where t.clinic_code=r.clinic_code
         and t.oper_date>sysdate -(select to_number(nvl(max(dic.code), '7')) 
                                        from com_dictionary dic 
                                        where dic.type = 'ZZSB_OP_FEE_DAYS' 
                                          and dic.valid_state = '1')
         and nvl((select p.extend_flag from met_ord_recipedetail  p where p.sequence_no=t.mo_order and rownum=1),nvl(t.extend_flag,'0'))='0'
         and t.trans_type='1'
         and t.pay_flag='0'
         and r.card_no='{0}'
and  EXISTS  (select 1 from com_dictionary dic  where  dic.TYPE='setUltrasound' and dic.code=t.package_code ) 
) 
group by regid,regdate,rankname,deptname,doctname,feetype,elderlyvoucherflag
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, QueryVisitRecordForSRM.CARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //查询就诊记录
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryVisitRecordForSRM = new His.Models.ZZSB.InVisitRecordForSRM();
                    QueryVisitRecordForSRM.REGID = dt.Rows[i][0].ToString();
                    QueryVisitRecordForSRM.REGDATE = dt.Rows[i][1].ToString();
                    QueryVisitRecordForSRM.RANKNAME = dt.Rows[i][2].ToString();
                    QueryVisitRecordForSRM.DEPTNAME = dt.Rows[i][3].ToString();
                    QueryVisitRecordForSRM.DOCTNAME = dt.Rows[i][4].ToString();
                    QueryVisitRecordForSRM.TOTALFEE = dt.Rows[i][5].ToString();
                    QueryVisitRecordForSRM.FEETYPE = dt.Rows[i][6].ToString();
                    QueryVisitRecordForSRM.FAVORFEE = dt.Rows[i][7].ToString();
                    QueryVisitRecordForSRM.MEDINSUREFEE = dt.Rows[i][8].ToString();
                    QueryVisitRecordForSRM.PERSONALFEE = dt.Rows[i][9].ToString();
                    QueryVisitRecordForSRM.DIAGNOSIS = dt.Rows[i][10].ToString();
                    QueryVisitRecordForSRM.NOTE = dt.Rows[i][11].ToString();
                    QueryVisitRecordForSRM.ELDERLYVOUCHERFLAG = dt.Rows[i][12].ToString();
                    //foreach (His.Models.ZZSB.InVisitRecordForSRM dsa in feelist)  //获取本次收费已经计算的数量
                    //{
                    //    if (dsa.REGID == QueryVisitRecordForSRM.REGID && dsa.DEPTNAME == QueryVisitRecordForSRM.DEPTNAME)
                    //    {
                    //        QueryVisitRecordForSRM.TOTALFEE = (Convert.ToDecimal(QueryVisitRecordForSRM.TOTALFEE) + Convert.ToDecimal(dsa.TOTALFEE)).ToString();
                    //        QueryVisitRecordForSRM.PERSONALFEE = (Convert.ToDecimal(QueryVisitRecordForSRM.PERSONALFEE) + Convert.ToDecimal(dsa.TOTALFEE)).ToString();
                    //    }
                    //} 
                    al.Add(QueryVisitRecordForSRM);
                }
                return al;
                #endregion
            }
            catch
            {
                return null;
            }
        }
        private System.Collections.ArrayList GetQueryfeeData(His.Models.ZZSB.InVisitRecordForSRM QueryVisitRecordForSRM)
        {
            #region sql
            string sql = @"  select item_code,sum(qty)qty,unit_price,input_code as limitnum,(select  nvl(sum(FEE.QTY),0) from fin_opb_feedetail fee where fee.CARD_NO='{0}' and fee.item_code=f.item_code
              and    fee.pay_flag <> '0' and  fee.CANCEL_FLAG <> '0'   
              and  to_char(fee.fee_date,'yyyy-mm-dd')=to_char(sysdate,'yyyy-mm-dd')  and  OWN_COST<>'0.00' 
            and EXISTS ( select code from com_dictionary dic  where  TYPE='Restrictingfee'and dic.code=fee.item_code) )feeqty,regid,deptname
          from (
         select  fee.package_code,fee.item_code,fee.qty,fee.unit_price,nvl(fee.input_code,999)input_code,t.clinic_code regid, fun_get_dept_name(t.reg_dpcd) deptname
         from  (         select  ztinfo.package_code,ztinfo.item_code,ztinfo.qty,info.unit_price,resfee.input_code from   fin_com_undrugztinfo  ztinfo  inner join fin_com_undruginfo info   on ztinfo.item_code=info.item_code    AND ztinfo.valid_state='1'
           left join (select code,input_code from com_dictionary  dic where  dic.TYPE='Restrictingfee' )resfee on  ztinfo.item_code=resfee.code
           where   info.VALID_STATE='1'    
           and  EXISTS    (  select 1  FROM (
           select    info.package_code AS code,dic.input_code from fin_com_undrugztinfo info
           INNER JOIN (select code,input_code from com_dictionary  where  TYPE='Restrictingfee') dic
            on info.item_code=dic.code where VALID_STATE='1'
            ) resfee  where  resfee.code=ztinfo.package_code )  )fee,fin_opb_feedetail t,fin_opr_register r
         where t.clinic_code=r.clinic_code
         and fee.package_code=t.item_code
         and t.oper_date>sysdate -7
         and nvl((select p.extend_flag from met_ord_recipedetail  p 
         where p.sequence_no=t.mo_order and rownum=1),'0')='0'
         and t.trans_type='1'
         and t.pay_flag='0'
         and r.card_no='{0}'
         union all   
         select fee.package_code,fee.item_code,t.qty,fee.unit_price,nvl(fee.input_code,999)input_code,t.clinic_code regid, fun_get_dept_name(t.reg_dpcd) deptname  from (
      select  item_code as package_code,info.item_code, info.unit_price,dic.input_code  from fin_com_undruginfo info 
         inner join (select code,input_code from com_dictionary  dic where  dic.TYPE='Restrictingfee' )dic 
           on  info.item_code=dic.code
              where  info.UNITFLAG='0' and info.valid_state='1'  )fee ,fin_opb_feedetail t,fin_opr_register r   
         where   t.clinic_code=r.clinic_code
         and fee.item_code=t.item_code
         and t.oper_date>sysdate -7
         and nvl((select p.extend_flag from met_ord_recipedetail  p 
         where p.sequence_no=t.mo_order and rownum=1),'0')='0'
         and t.trans_type='1'
         and t.pay_flag='0'
         and r.card_no='{0}'
         )f group by item_code,unit_price,input_code,regid,deptname
            ";
            #endregion

            try
            {
                System.Collections.ArrayList hsREOnlylistItem = new  System.Collections.ArrayList();
                System.Collections.ArrayList hsZTlistItem = new System.Collections.ArrayList();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
                #region 数据赋值
                sql = string.Format(sql, QueryVisitRecordForSRM.CARDNO);
                System.Data.DataTable dt = new System.Data.DataTable();
                //查询就诊记录
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                //收费总数，项目价格，限制收费数量，收费数量，已收费数量
                string item_code;
                His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM ZTQueryVisitRecordForSRM = new His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM();
                Decimal feecost = 0, unit_price = 0, limitnum = 0, qty = 0, feeqty = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryVisitRecordForSRM = new His.Models.ZZSB.InVisitRecordForSRM();
                    item_code = dt.Rows[i][0].ToString();
                    qty = Convert.ToDecimal(dt.Rows[i][1].ToString());
                    unit_price = Convert.ToDecimal(dt.Rows[i][2].ToString());
                    limitnum = Convert.ToDecimal(dt.Rows[i][3].ToString());
                    feeqty = Convert.ToDecimal(dt.Rows[i][4].ToString());
                    foreach (His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM  dsa in hsZTlistItem)  //获取本次收费已经计算的数量
                    {
                        if (dsa.ITEMID == item_code)
                        {
                            feeqty += Convert.ToDecimal(dsa.QUANTITY);
                        }
                    }
                    if (limitnum > (qty + feeqty))
                    {
                        feecost = unit_price * qty;
                        ZTQueryVisitRecordForSRM.ITEMID = item_code;
                        ZTQueryVisitRecordForSRM.QUANTITY = qty.ToString();
                        QueryVisitRecordForSRM.REGID = dt.Rows[i][5].ToString();
                        QueryVisitRecordForSRM.DEPTNAME = dt.Rows[i][6].ToString();
                        QueryVisitRecordForSRM.TOTALFEE = feecost.ToString();
                        hsZTlistItem.Add(ZTQueryVisitRecordForSRM);
                        hsREOnlylistItem.Add(QueryVisitRecordForSRM);
                    }
                    else if (limitnum <= (qty + feeqty))
                    {
                        limitnum = limitnum - feeqty;
                        if (limitnum > 0 || limitnum <= qty)
                        {
                            feecost = unit_price * limitnum;
                            ZTQueryVisitRecordForSRM.ITEMID = item_code;
                            ZTQueryVisitRecordForSRM.QUANTITY = limitnum.ToString();
                            QueryVisitRecordForSRM.REGID = dt.Rows[i][5].ToString();
                            QueryVisitRecordForSRM.DEPTNAME = dt.Rows[i][6].ToString();
                            QueryVisitRecordForSRM.TOTALFEE = feecost.ToString();
                            hsZTlistItem.Add(ZTQueryVisitRecordForSRM);
                            hsREOnlylistItem.Add(QueryVisitRecordForSRM);
                        }
                    }
                }
                return hsREOnlylistItem;
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
            ErrorMsg.InnerText = "当时患者无处方缴费信息";
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

        private string GetQueryVisitRecordForSRMXML(System.Collections.ArrayList al)
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


                //His.Models.ZZSB.InVisitRecordForSRM p = al[0] as His.Models.ZZSB.InVisitRecordForSRM;
                foreach (His.Models.ZZSB.InVisitRecordForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement REGID = xml.CreateElement("REGID");
                    REGID.InnerText = p.REGID;
                    Result.AppendChild(REGID);

                    System.Xml.XmlElement REGDATE = xml.CreateElement("REGDATE");
                    REGDATE.InnerText = p.REGDATE;
                    Result.AppendChild(REGDATE);

                    System.Xml.XmlElement RANKNAME = xml.CreateElement("RANKNAME");
                    RANKNAME.InnerText = p.RANKNAME;
                    Result.AppendChild(RANKNAME);

                    System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                    DEPTNAME.InnerText = p.DEPTNAME;
                    Result.AppendChild(DEPTNAME);

                    System.Xml.XmlElement DOCTNAME = xml.CreateElement("DOCTNAME");
                    DOCTNAME.InnerText = p.DOCTNAME;
                    Result.AppendChild(DOCTNAME);

                    System.Xml.XmlElement TOTALFEE = xml.CreateElement("TOTALFEE");
                    TOTALFEE.InnerText = p.TOTALFEE;
                    Result.AppendChild(TOTALFEE);

                    System.Xml.XmlElement FEETYPE = xml.CreateElement("FEETYPE");
                    FEETYPE.InnerText = p.FEETYPE;
                    Result.AppendChild(FEETYPE);

                    System.Xml.XmlElement FAVORFEE = xml.CreateElement("FAVORFEE");
                    FAVORFEE.InnerText = p.FAVORFEE;
                    Result.AppendChild(FAVORFEE);

                    System.Xml.XmlElement MEDINSUREFEE = xml.CreateElement("MEDINSUREFEE");
                    MEDINSUREFEE.InnerText = p.MEDINSUREFEE;
                    Result.AppendChild(MEDINSUREFEE);

                    System.Xml.XmlElement PERSONALFEE = xml.CreateElement("PERSONALFEE");
                    PERSONALFEE.InnerText = p.PERSONALFEE;
                    Result.AppendChild(PERSONALFEE);

                    System.Xml.XmlElement DIAGNOSIS = xml.CreateElement("DIAGNOSIS");
                    DIAGNOSIS.InnerText = p.DIAGNOSIS;
                    Result.AppendChild(DIAGNOSIS);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                    System.Xml.XmlElement ELDERLYVOUCHERFLAG = xml.CreateElement("ELDERLYVOUCHERFLAG");
                    ELDERLYVOUCHERFLAG.InnerText = p.ELDERLYVOUCHERFLAG;
                    Result.AppendChild(ELDERLYVOUCHERFLAG);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetQueryVisitRecordForSRMModel(string xml, ref His.Models.ZZSB.InVisitRecordForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InVisitRecordForSRM();
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


            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }



        public string GetQueryVisitRecordForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InVisitRecordForSRM opa = new His.Models.ZZSB.InVisitRecordForSRM();
            returnStr = this.GetQueryVisitRecordForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList feelist = this.GetQueryfeeData(opa);
            System.Collections.ArrayList al = this.GetQueryVisitRecordForSRMData(opa, feelist);
            returnStr = this.GetQueryVisitRecordForSRMXML(al);
            return returnStr;
        }
    }

    public class GetPrescriptionAndChargeDetailsForSRM
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


        private System.Collections.ArrayList GetPrescriptionAndChargeDetailsForSRMData(His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM GetPrescriptionAndChargeDetailsForSRM)
        {
            try
            {
                HisDBHelp db = new HisDBHelp();
                return db.QueryUnpaidFeeDetail(GetPrescriptionAndChargeDetailsForSRM.PATIENTID);
            }
            catch
            {
                return null;
            }
        }
        private string ERRNoPay(string err)
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
            ErrorMsg.InnerText = err;
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



        private string ERR(string msg)
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
            ErrorMsg.InnerText = msg;
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

        private string GetPrescriptionAndChargeDetailsForSRMXML(System.Collections.ArrayList al)
        {

            System.Collections.Hashtable hsItemList = this.getItemListWipeOffZZSB();

            #region
            if (al == null || al.Count == 0)
            {
                return this.ERR("费用信息为空！");
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

                // 将ArrayList转换为List<FeeItemList>  
                List<Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList> feeItemList = al.Cast<Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList>().ToList();
                var groupedByRecipeNO = feeItemList.GroupBy(item => item.RecipeNO)
                                          .Select(group => new { RecipeNO = group.Key, Items = group.ToList() })
                                          .ToList();
                foreach (var group in groupedByRecipeNO)
                {
                    string ZZSBNOTPAYMSG = "";
                    System.Xml.XmlElement Recipe = xml.CreateElement("Recipe");
                    root1.AppendChild(Recipe);
                    System.Xml.XmlElement Items = xml.CreateElement("Items");
                    Recipe.AppendChild(Items);
                    foreach(Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList p in group.Items)
                    {
                        if (p.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug && (p.FT.PubCost + p.FT.PayCost + p.FT.OwnCost) == 0)
                        {
                            continue;
                        }
                        System.Xml.XmlElement Item = xml.CreateElement("Item");
                        Items.AppendChild(Item);

                        //if (p.CARDNO == "ALL")
                        //{
                        //    return this.ERR();
                        //}
                        if (hsItemList.Contains(p.Item.ID))
                        {
                            ZZSBNOTPAYMSG +=   p.Item.Name + ",";
                        }
                        System.Xml.XmlElement RECIPENO = xml.CreateElement("RECIPENO");
                        RECIPENO.InnerText = p.RecipeNO;
                        Item.AppendChild(RECIPENO);

                        System.Xml.XmlElement RECIPEFLAG = xml.CreateElement("RECIPEFLAG");
                        RECIPEFLAG.InnerText = p.RecipeFlag;
                        Item.AppendChild(RECIPEFLAG);

                        System.Xml.XmlElement RECIPETIME = xml.CreateElement("RECIPETIME");
                        RECIPETIME.InnerText = ((Neusoft.HISFC.Models.Registration.Register)p.Patient).DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm:ss");
                        Item.AppendChild(RECIPETIME);

                        System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                        DEPTNAME.InnerText = ((Neusoft.HISFC.Models.Registration.Register)p.Patient).DoctorInfo.Templet.Dept.Name;
                        Item.AppendChild(DEPTNAME);

                        System.Xml.XmlElement DOCTNAME = xml.CreateElement("DOCTNAME");
                        DOCTNAME.InnerText = p.RecipeOper.Name;
                        Item.AppendChild(DOCTNAME);

                        System.Xml.XmlElement TOTALFEE = xml.CreateElement("TOTALFEE");
                        TOTALFEE.InnerText = (p.FT.PubCost + p.FT.PayCost + p.FT.OwnCost).ToString();
                        Item.AppendChild(TOTALFEE);

                        System.Xml.XmlElement PAYFLAG = xml.CreateElement("PAYFLAG");
                        PAYFLAG.InnerText = "0";
                        Item.AppendChild(PAYFLAG);

                        System.Xml.XmlElement ITEMID = xml.CreateElement("ITEMID");
                        ITEMID.InnerText = p.Item.ID;
                        Item.AppendChild(ITEMID);

                        System.Xml.XmlElement ITEMNAME = xml.CreateElement("ITEMNAME");
                        ITEMNAME.InnerText = p.Item.Name;
                        Item.AppendChild(ITEMNAME);

                        System.Xml.XmlElement ITEMTYPE = xml.CreateElement("ITEMTYPE");
                        ITEMTYPE.InnerText = p.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug ? "1" : "0";
                        Item.AppendChild(ITEMTYPE);

                        System.Xml.XmlElement ITEMTOTALFEE = xml.CreateElement("ITEMTOTALFEE");
                        ITEMTOTALFEE.InnerText = (p.FT.PubCost + p.FT.PayCost + p.FT.OwnCost).ToString();
                        Item.AppendChild(ITEMTOTALFEE);

                        System.Xml.XmlElement SUBITEMID = xml.CreateElement("SUBITEMID");
                        SUBITEMID.InnerText = "";
                        Item.AppendChild(SUBITEMID);

                        System.Xml.XmlElement SUBITEMNAME = xml.CreateElement("SUBITEMNAME");
                        SUBITEMNAME.InnerText = "";
                        Item.AppendChild(SUBITEMNAME);

                        System.Xml.XmlElement SPECS = xml.CreateElement("SPECS");
                        SPECS.InnerText = p.Item.Specs;
                        Item.AppendChild(SPECS);

                        System.Xml.XmlElement UNIT = xml.CreateElement("UNIT");
                        UNIT.InnerText = p.Item.PriceUnit;
                        Item.AppendChild(UNIT);

                        System.Xml.XmlElement QUANTITY = xml.CreateElement("QUANTITY");
                        QUANTITY.InnerText = p.Item.Qty.ToString();
                        Item.AppendChild(QUANTITY);

                        System.Xml.XmlElement UNITPRICE = xml.CreateElement("UNITPRICE");
                        UNITPRICE.InnerText = p.Item.Price.ToString();
                        Item.AppendChild(UNITPRICE);

                        System.Xml.XmlElement FEE = xml.CreateElement("FEE");
                        FEE.InnerText = (p.FT.PubCost + p.FT.PayCost + p.FT.OwnCost).ToString();
                        Item.AppendChild(FEE);

                        System.Xml.XmlElement DEPTFLOOR = xml.CreateElement("DEPTFLOOR");
                        DEPTFLOOR.InnerText = "";
                        Item.AppendChild(DEPTFLOOR);

                        System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                        NOTE.InnerText = ((Neusoft.HISFC.Models.Registration.Register)p.Patient).DoctorInfo.Templet.Dept.ID;
                        Item.AppendChild(NOTE);

                        System.Xml.XmlElement address = xml.CreateElement("ADDRESS");
                        address.InnerText = p.ExecOper.Dept.Memo.ToString();
                        Item.AppendChild(address);
                    }
                    string ZZSBNOTPAY = "0";
                    
                    if (ZZSBNOTPAYMSG != "")
                    {
                        ZZSBNOTPAYMSG = "费用项目包含了:" + ZZSBNOTPAYMSG + ",请到人工窗口进行缴费，谢谢合作！";
                        ZZSBNOTPAY = "1";
                    }
                    System.Xml.XmlElement IZZSBNOTPAYMSG = xml.CreateElement("ZZSBNOTPAYMSG");
                    IZZSBNOTPAYMSG.InnerText = ZZSBNOTPAYMSG;
                    Recipe.AppendChild(IZZSBNOTPAYMSG);
                    System.Xml.XmlElement IZZSBNOTPAY = xml.CreateElement("ZZSBNOTPAY");
                    IZZSBNOTPAY.InnerText = ZZSBNOTPAY;
                    Recipe.AppendChild(IZZSBNOTPAY);
                }
            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetPrescriptionAndChargeDetailsForSRMModel(string xml, ref His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM();
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


            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }



            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList RegID1 = doc.GetElementsByTagName("RegID");
            System.Xml.XmlNode RegID = RegID1[0];
            if (!string.IsNullOrEmpty(RegID.InnerText))
            {
                opa.PATIENTID = RegID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "就诊记录号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }



        public string GetGetPrescriptionAndChargeDetailsForSRM(string xml)
        {
            Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
            string returnStr = "";
            int itemqty = 0;
            decimal pricesz = 0;
            decimal pricece = 0;
            this.GetPricesz("F00000010769", ref pricesz);//获取加收项目价格
            this.GetPricesz("F00000010768", ref pricece);//获取加收项目价格
            pricece = pricece - pricesz;
            His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM opa = new His.Models.ZZSB.InGetPrescriptionAndChargeDetailsForSRM();
            returnStr = this.GetPrescriptionAndChargeDetailsForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetPrescriptionAndChargeDetailsForSRMData(opa);
            if (!RecipeFlagIsNull(opa.PATIENTID))
            {
                string errMsg = "存在处方类型为空的医嘱，请到人工窗口处理";
                return this.ERRNoPay(errMsg);
            }
            System.Collections.Hashtable hsszItemList = this.getItemListWipeOffSZXM();

            for (int i = 0; i < al.Count; i++)
            {
                var feeItem = al[i] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                int Store = getItemListStore(opa.PATIENTID, feeItem.Item.ID);
                if (Store < 1)
                {
                    string errMsg = "该药品" + feeItem.Item.Name + "库存不足，不能缴费，请联系医生重新开立.";
                    return this.ERRNoPay(errMsg);
                }
            }

            ZDWY.SpecialRule.Price.CTMRFeeRule CTMRFeeRule = new ZDWY.SpecialRule.Price.CTMRFeeRule();
            try
            {
                var actualAll = CTMRFeeRule.GetFeeItemListnew(opa.PATIENTID, al);
                if (actualAll == null)
                {
                    return this.ERR("查询费用出现异常：" + CTMRFeeRule.errText);
                }
                if (actualAll.Count <= 0)
                {
                    return this.ERR("未查询到相关费用：" + CTMRFeeRule.errText);
                }
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList s in actualAll)
                {
                    if (hsszItemList.Contains(s.Item.ID))
                    {
                        if (itemqty > 0)
                        {
                            s.FT.OwnCost = s.FT.OwnCost - pricece;
                            s.Item.Price = s.Item.Price - pricece;
                        }
                        itemqty = itemqty + 1;

                    }
                }
                returnStr = this.GetPrescriptionAndChargeDetailsForSRMXML(actualAll);
                return returnStr;
            }
            catch (Exception ex)
            {

                return this.ERR("查询费用出现异常：" + ex.Message);
            }


        }

        #region 获取在自助机上不能缴费的项目
        private System.Collections.Hashtable getItemListWipeOffZZSB()
        {
            string strSql = string.Empty;
            RegisterManager mgr = new RegisterManager();
            strSql = @"select p.code,p.name from  com_dictionary p where p.type='ZZSBNOPayItem'";
            System.Collections.Hashtable hsLimit = new System.Collections.Hashtable();
            System.Collections.ArrayList alLimit = new System.Collections.ArrayList();

            try
            {


                mgr.ExecQuery(strSql);
                while (mgr.Reader.Read())
                {
                    Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = mgr.Reader[0].ToString();//项目编码
                    obj.Name = mgr.Reader[1].ToString();//项目名称
                    alLimit.Add(obj);
                }
                mgr.Reader.Close();
            }
            catch
            {
            }

            if (alLimit != null && alLimit.Count > 0)
            {
                foreach (Neusoft.FrameWork.Models.NeuObject dic in alLimit)
                {
                    if (hsLimit.ContainsKey(dic.ID))
                    {
                        continue;
                    }
                    else
                    {
                        hsLimit.Add(dic.ID, dic);
                    }
                }
            }
            return hsLimit;
        }
        #endregion
        #region 获取四肢血管项目判断是否需要转换
        private System.Collections.Hashtable getItemListWipeOffSZXM()
        {
            string strSql = string.Empty;
            RegisterManager mgr = new RegisterManager();
            strSql = @"select p.code,p.name from  com_dictionary p where p.type='setUltrasound'";
            System.Collections.Hashtable hsLimit = new System.Collections.Hashtable();
            System.Collections.ArrayList alLimit = new System.Collections.ArrayList();

            try
            {


                mgr.ExecQuery(strSql);
                while (mgr.Reader.Read())
                {
                    Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                    obj.ID = mgr.Reader[0].ToString();//项目编码
                    obj.Name = mgr.Reader[1].ToString();//项目名称
                    alLimit.Add(obj);
                }
                mgr.Reader.Close();
            }
            catch
            {
            }

            if (alLimit != null && alLimit.Count > 0)
            {
                foreach (Neusoft.FrameWork.Models.NeuObject dic in alLimit)
                {
                    if (hsLimit.ContainsKey(dic.ID))
                    {
                        continue;
                    }
                    else
                    {
                        hsLimit.Add(dic.ID, dic);
                    }
                }
            }
            return hsLimit;
        }
        #endregion

        #region 是否有处方类型为空 返回 false 就不给结算
        /// <summary>
        /// 是否有处方类型为空 返回 false 就不给结算
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public bool RecipeFlagIsNull(string clinicCode)
        {
            string strSql = string.Empty;
            strSql = @"select count(1) from fin_opb_feedetail p where p.clinic_code ='{0}' and p.pay_flag = '0' and p.recipe_flag is null ";
            strSql = string.Format(strSql, clinicCode);
            RegisterManager mgr = new RegisterManager();
            string strCount = mgr.ExecSqlReturnOne(strSql);
            if (!string.IsNullOrEmpty(strCount))
            {
                if (int.Parse(strCount.ToString())==0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        /// <summary>
        /// 获取四肢血管加收价格
        /// </summary>
        public int GetPricesz(string ItemID, ref decimal Price)
        {
            //{B9303CFE-755D-4585-B5EE-8C1901F79450} 整合时对照修改此函数
            string strSql = "";
            RegisterManager mgr = new RegisterManager();
            //获得患者合同单位
            try
            {
                #region 非药品取项目价格

                strSql = @"SELECT unit_price,   --三甲价
                           unit_price,   --儿童价
                           unit_price2    --特诊价
                           FROM fin_com_undruginfo   --非药品信息表
                           WHERE   item_code='{0}'";
                strSql = string.Format(strSql, ItemID);
                if (mgr.ExecQuery(strSql) == -1) return -1;
                int count = 0;

                while (mgr.Reader.Read())
                {
                    Price = Neusoft.FrameWork.Function.NConvert.ToDecimal(mgr.Reader[0].ToString());
                    count++;
                }
                mgr.Reader.Close();

                if (count == 0)
                {
                    return -1;
                }
                #endregion

            }
            catch (Exception e)
            {
                if (mgr.Reader.IsClosed == false) mgr.Reader.Close();
                return -1;
            }
            return 0;
        }
        #region 获取项目库存数
        private int getItemListStore(string ClinicCode, string Item_Code)
        {
            string strSql = string.Empty;
            int Store = 1;
            HisDBHelp mgr = new HisDBHelp();
            strSql = @"Select PREOUT_SUM From V_Hts_Getstore Where Clinic_Code='{0}'  AND  Item_Code='{1}'  ";
            strSql = string.Format(strSql, ClinicCode, Item_Code);
            //System.Collections.Hashtable hsLimit = new System.Collections.Hashtable();
            try
            {

                Store = mgr.GetStore(strSql);

            }
            catch (Exception e)
            {
            }
            return Store;
        }


        #endregion
    }

    public class GetGuideListInfoForSRM
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


        private System.Collections.ArrayList GetGuideListInfoForSRMData(His.Models.ZZSB.InGetGuideListInfoForSRM GetGuideListInfoForSRM)
        {
            #region sql
            string sql = @"  select t.invoice_no transerno,--交易流水号
       t.invoice_no invoiceno,--发票号
       t.dept_name execadress,--执行地点
       null message,--提示信息
       null note --备用字段
 from fin_opr_register t
 where t.card_no='{0}'
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, GetGuideListInfoForSRM.CARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //获取导诊信息
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GetGuideListInfoForSRM = new His.Models.ZZSB.InGetGuideListInfoForSRM();
                    GetGuideListInfoForSRM.TRANSERNO = dt.Rows[i][0].ToString();
                    GetGuideListInfoForSRM.INVOICENO = dt.Rows[i][1].ToString();
                    GetGuideListInfoForSRM.EXECADRESS = dt.Rows[i][2].ToString();
                    GetGuideListInfoForSRM.MESSAGE = dt.Rows[i][3].ToString();
                    GetGuideListInfoForSRM.NOTE = dt.Rows[i][4].ToString();
                    al.Add(GetGuideListInfoForSRM);
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

        private string GetGuideListInfoForSRMXML(System.Collections.ArrayList al)
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
                foreach (His.Models.ZZSB.InGetGuideListInfoForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement TRANSERNO = xml.CreateElement("TRANSERNO");
                    TRANSERNO.InnerText = p.TRANSERNO;
                    Result.AppendChild(TRANSERNO);

                    System.Xml.XmlElement INVOICENO = xml.CreateElement("INVOICENO");
                    INVOICENO.InnerText = p.INVOICENO;
                    Result.AppendChild(INVOICENO);

                    System.Xml.XmlElement EXECADRESS = xml.CreateElement("EXECADRESS");
                    EXECADRESS.InnerText = p.EXECADRESS;
                    Result.AppendChild(EXECADRESS);

                    System.Xml.XmlElement MESSAGE = xml.CreateElement("MESSAGE");
                    MESSAGE.InnerText = p.MESSAGE;
                    Result.AppendChild(MESSAGE);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetGuideListInfoForSRMModel(string xml, ref His.Models.ZZSB.InGetGuideListInfoForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InGetGuideListInfoForSRM();
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


            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList HOSPCODE1 = doc.GetElementsByTagName("HospCode");
            System.Xml.XmlNode HOSPCODE = HOSPCODE1[0];
            if (!string.IsNullOrEmpty(HOSPCODE.InnerText))
            {
                opa.HOSPCODE = HOSPCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "院区编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList CARDTYPECODE1 = doc.GetElementsByTagName("CardTypeCode");
            System.Xml.XmlNode CARDTYPECODE = CARDTYPECODE1[0];
            if (!string.IsNullOrEmpty(CARDTYPECODE.InnerText))
            {
                opa.CARDTYPECODE = CARDTYPECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡类型不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList PATIENTID1 = doc.GetElementsByTagName("PatientID");
            System.Xml.XmlNode PATIENTID = PATIENTID1[0];
            if (!string.IsNullOrEmpty(PATIENTID.InnerText))
            {
                opa.PATIENTID = PATIENTID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "患者ID不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList REGID1 = doc.GetElementsByTagName("RegID");
            System.Xml.XmlNode REGID = REGID1[0];
            if (!string.IsNullOrEmpty(REGID.InnerText))
            {
                opa.REGID = REGID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "就诊记录编码不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList RECIPENO1 = doc.GetElementsByTagName("RecipeNo");
            System.Xml.XmlNode RECIPENO = RECIPENO1[0];
            if (!string.IsNullOrEmpty(RECIPENO.InnerText))
            {
                opa.RECIPENO = RECIPENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "处方号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }



        public string GetGetGuideListInfoForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InGetGuideListInfoForSRM opa = new His.Models.ZZSB.InGetGuideListInfoForSRM();
            returnStr = this.GetGuideListInfoForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetGuideListInfoForSRMData(opa);
            returnStr = this.GetGuideListInfoForSRMXML(al);
            return returnStr;
        }
    }

    public class QueryFetchMedicineQueueForSRM
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


        private System.Collections.ArrayList GetQueryFetchMedicineQueueForSRMData(His.Models.ZZSB.InQueryFetchMedicineQueueForSRM QueryFetchMedicineQueueForSRM)
        {
            #region sql
            string sql = @"select fun_get_dept_name(t.drug_dept_code) deptname ,--科室名称
                count(t.recipe_no) receiptnum,--处方张数
                y.send_terminal_name execlocation,--执行位置
                null waitno,--取药序号
                null waitnum,--等候人次
                null quedate,--队列时间
                null currentno,--当前序号
                null note  
          from PHA_SOC_CallQueue y,PHA_STO_RECIPE t
          where t.recipe_no=y.recipe_no
          and y.card_no='{0}'
          and t.recipe_state='2'
          group by t.drug_dept_code,y.send_terminal_name
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, QueryFetchMedicineQueueForSRM.CARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //获取取药队列
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryFetchMedicineQueueForSRM = new His.Models.ZZSB.InQueryFetchMedicineQueueForSRM();
                    QueryFetchMedicineQueueForSRM.DEPTNAME = dt.Rows[i][0].ToString();
                    QueryFetchMedicineQueueForSRM.RECEIPTNUM = dt.Rows[i][1].ToString();
                    QueryFetchMedicineQueueForSRM.EXECLOCATION = dt.Rows[i][2].ToString();
                    QueryFetchMedicineQueueForSRM.WAITNO = dt.Rows[i][3].ToString();
                    QueryFetchMedicineQueueForSRM.WAITNUM = dt.Rows[i][4].ToString();
                    QueryFetchMedicineQueueForSRM.QUEDATE = dt.Rows[i][5].ToString();
                    QueryFetchMedicineQueueForSRM.CURRENTNO = dt.Rows[i][6].ToString();
                    QueryFetchMedicineQueueForSRM.NOTE = dt.Rows[i][7].ToString();
                    al.Add(QueryFetchMedicineQueueForSRM);
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

        private string GetQueryFetchMedicineQueueForSRMXML(System.Collections.ArrayList al)
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
                foreach (His.Models.ZZSB.InQueryFetchMedicineQueueForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement DEPTNAME = xml.CreateElement("DEPTNAME");
                    DEPTNAME.InnerText = p.DEPTNAME;
                    Result.AppendChild(DEPTNAME);

                    System.Xml.XmlElement RECEIPTNUM = xml.CreateElement("RECEIPTNUM");
                    RECEIPTNUM.InnerText = p.RECEIPTNUM;
                    Result.AppendChild(RECEIPTNUM);

                    System.Xml.XmlElement EXECLOCATION = xml.CreateElement("EXECLOCATION");
                    EXECLOCATION.InnerText = p.EXECLOCATION;
                    Result.AppendChild(EXECLOCATION);

                    System.Xml.XmlElement WAITNO = xml.CreateElement("WAITNO");
                    WAITNO.InnerText = p.WAITNO;
                    Result.AppendChild(WAITNO);

                    System.Xml.XmlElement WAITNUM = xml.CreateElement("WAITNUM");
                    WAITNUM.InnerText = p.WAITNUM;
                    Result.AppendChild(WAITNUM);

                    System.Xml.XmlElement QUEDATE = xml.CreateElement("QUEDATE");
                    QUEDATE.InnerText = p.QUEDATE;
                    Result.AppendChild(QUEDATE);

                    System.Xml.XmlElement CURRENTNO = xml.CreateElement("CURRENTNO");
                    CURRENTNO.InnerText = p.CURRENTNO;
                    Result.AppendChild(CURRENTNO);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetQueryFetchMedicineQueueForSRMModel(string xml, ref His.Models.ZZSB.InQueryFetchMedicineQueueForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InQueryFetchMedicineQueueForSRM();
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


            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList HOSPCODE1 = doc.GetElementsByTagName("HospCode");
            System.Xml.XmlNode HOSPCODE = HOSPCODE1[0];
            if (!string.IsNullOrEmpty(HOSPCODE.InnerText))
            {
                opa.HOSPCODE = HOSPCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "院区编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList CARDTYPECODE1 = doc.GetElementsByTagName("CardTypeCode");
            System.Xml.XmlNode CARDTYPECODE = CARDTYPECODE1[0];
            if (!string.IsNullOrEmpty(CARDTYPECODE.InnerText))
            {
                opa.CARDTYPECODE = CARDTYPECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡类型不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList PATIENTID1 = doc.GetElementsByTagName("PatientID");
            System.Xml.XmlNode PATIENTID = PATIENTID1[0];
            if (!string.IsNullOrEmpty(PATIENTID.InnerText))
            {
                opa.PATIENTID = PATIENTID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "患者ID不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList REGID1 = doc.GetElementsByTagName("RegID");
            System.Xml.XmlNode REGID = REGID1[0];
            if (!string.IsNullOrEmpty(REGID.InnerText))
            {
                opa.REGID = REGID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "就诊记录编码不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList RECIPENO1 = doc.GetElementsByTagName("RecipeNo");
            System.Xml.XmlNode RECIPENO = RECIPENO1[0];
            if (!string.IsNullOrEmpty(RECIPENO.InnerText))
            {
                opa.RECIPENO = RECIPENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "处方号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }



        public string GetQueryFetchMedicineQueueForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InQueryFetchMedicineQueueForSRM opa = new His.Models.ZZSB.InQueryFetchMedicineQueueForSRM();
            returnStr = this.GetQueryFetchMedicineQueueForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetQueryFetchMedicineQueueForSRMData(opa);
            returnStr = this.GetQueryFetchMedicineQueueForSRMXML(al);
            return returnStr;
        }
    }

    public class QueryExaminationQueueForSRM
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


        private System.Collections.ArrayList GetQueryExaminationQueueForSRMData(His.Models.ZZSB.InQueryExaminationQueueForSRM QueryExaminationQueueForSRM)
        {
            #region sql
            string sql = @"select distinct (t.comb_no) checkid, --检查编号
                                    t.item_name checkname, --检查名称
                                    fun_get_dept_name(t.exec_dpcd) execlocation, --执行位置
                                    null waitno, --检查序号
                                    null currentno, --当前序号
                                    null note
                      from fin_opb_feedetail t
                     where t.class_code = 'UC'
                       and t.trans_type = '1'
                       and cancel_flag = '1'
                       and t.card_no='{0}'
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, QueryExaminationQueueForSRM.CARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //查询检查队列
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryExaminationQueueForSRM = new His.Models.ZZSB.InQueryExaminationQueueForSRM();
                    QueryExaminationQueueForSRM.CHECKID = dt.Rows[i][0].ToString();
                    QueryExaminationQueueForSRM.CHECKNAME = dt.Rows[i][1].ToString();
                    QueryExaminationQueueForSRM.EXECLOCATION = dt.Rows[i][2].ToString();
                    QueryExaminationQueueForSRM.WAITNO = dt.Rows[i][3].ToString();
                    QueryExaminationQueueForSRM.CURRENTNO = dt.Rows[i][4].ToString();
                    QueryExaminationQueueForSRM.NOTE = dt.Rows[i][5].ToString();
                    al.Add(QueryExaminationQueueForSRM);
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

        private string GetQueryExaminationQueueForSRMXML(System.Collections.ArrayList al)
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
                foreach (His.Models.ZZSB.InQueryExaminationQueueForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement CHECKID = xml.CreateElement("CHECKID");
                    CHECKID.InnerText = p.CHECKID;
                    Result.AppendChild(CHECKID);

                    System.Xml.XmlElement CHECKNAME = xml.CreateElement("CHECKNAME");
                    CHECKNAME.InnerText = p.CHECKNAME;
                    Result.AppendChild(CHECKNAME);

                    System.Xml.XmlElement EXECLOCATION = xml.CreateElement("EXECLOCATION");
                    EXECLOCATION.InnerText = p.EXECLOCATION;
                    Result.AppendChild(EXECLOCATION);

                    System.Xml.XmlElement WAITNO = xml.CreateElement("WAITNO");
                    WAITNO.InnerText = p.WAITNO;
                    Result.AppendChild(WAITNO);

                    System.Xml.XmlElement CURRENTNO = xml.CreateElement("CURRENTNO");
                    CURRENTNO.InnerText = p.CURRENTNO;
                    Result.AppendChild(CURRENTNO);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetQueryExaminationQueueForSRMModel(string xml, ref His.Models.ZZSB.InQueryExaminationQueueForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InQueryExaminationQueueForSRM();
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


            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList HOSPCODE1 = doc.GetElementsByTagName("HospCode");
            System.Xml.XmlNode HOSPCODE = HOSPCODE1[0];
            if (!string.IsNullOrEmpty(HOSPCODE.InnerText))
            {
                opa.HOSPCODE = HOSPCODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "院区编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList CARDTYPECODE1 = doc.GetElementsByTagName("CardTypeCode");
            System.Xml.XmlNode CARDTYPECODE = CARDTYPECODE1[0];
            if (!string.IsNullOrEmpty(CARDTYPECODE.InnerText))
            {
                opa.CARDTYPECODE = CARDTYPECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡类型不能为空！";
                return this.ReturnFailure();
            }


            System.Xml.XmlNodeList PATIENTID1 = doc.GetElementsByTagName("PatientID");
            System.Xml.XmlNode PATIENTID = PATIENTID1[0];
            if (!string.IsNullOrEmpty(PATIENTID.InnerText))
            {
                opa.PATIENTID = PATIENTID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "患者ID不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList REGID1 = doc.GetElementsByTagName("RegID");
            System.Xml.XmlNode REGID = REGID1[0];
            if (!string.IsNullOrEmpty(REGID.InnerText))
            {
                opa.REGID = REGID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "就诊记录编码不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList RECIPENO1 = doc.GetElementsByTagName("RecipeNo");
            System.Xml.XmlNode RECIPENO = RECIPENO1[0];
            if (!string.IsNullOrEmpty(RECIPENO.InnerText))
            {
                opa.RECIPENO = RECIPENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "处方号不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }



        public string GetQueryExaminationQueueForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InQueryExaminationQueueForSRM opa = new His.Models.ZZSB.InQueryExaminationQueueForSRM();
            returnStr = this.GetQueryExaminationQueueForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetQueryExaminationQueueForSRMData(opa);
            returnStr = this.GetQueryExaminationQueueForSRMXML(al);
            return returnStr;
        }
    }

    public class QueryInPatientQlistForSRM
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


        private System.Collections.ArrayList GetQueryInPatientQlistForSRMData(His.Models.ZZSB.InQueryInPatientQlistForSRM QueryInPatientQlistForSRM)
        {
            #region sql
            string sql = @"select inpatientno,
                                   name,
                                   feetype,
                                   depname,
                                   totalfee,
                                   isprintable,
                                   prindate,
                                   startdate,
                                   enddate,
                                   feedate，
                                   itemclass，
                                   itemname，
                                   standard1，
                                   units，
                                   price，
                                   number1，
                                   itemfee，
                                   execdept，
                                   note
                              from
                            (

                            select r.patient_no inpatientno,--住院号
                                   m.name,--姓名
                                   r.pact_name feetype,--患者费别
                                   fun_get_dept_name(m.inhos_deptcode) depname,--住院科室名
                                   round(sum(m.tot_cost), 2) totalfee, --当前费用总计
                                   1 isprintable,--是否可以打印
                                   null prindate,
                                   TRUNC(m.fee_date) startdate, --费用开始时间
                                   null enddate,--费用结束时间
                                   TRUNC(m.fee_date) feedate,--费用日期
                                   (select t.fee_stat_name from fin_com_feecodestat t where t.report_code='ZY01' and t.fee_code=m.fee_code)  itemclass,--项目类型
                                   m.drug_name||decode(m.specs,'','','['||m.specs||']') itemname, --项目名称
                                   m.specs standard1,--规格
                                   m.current_unit units, --单位
                                   ROUND(M.UNIT_PRICE/M.PACK_QTY,2) price, --单价
                                   sum(m.qty) number1,  --数量,
                                   sum(m.qty) itemfee,  --合计,
                                   fun_get_dept_name(m.execute_deptcode) execdept,--执行科室
                                   null note
                              from  fin_ipr_inmaininfo r,fin_ipb_medicinelist m
                                    left join (SELECT f.USE_TIME,f.exec_sqn FROM MET_IPM_EXECDRUG f) f on m.mo_exec_sqn=f.exec_sqn
                             where m.inpatient_no = r.inpatient_no
                               and r.patient_no='{0}'
                               and m.fee_date >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                               and m.fee_date < to_date('{2}','yyyy-mm-dd hh24:mi:ss')
                             group by r.patient_no, m.name, r.pact_name, m.inhos_deptcode, m.fee_date, m.drug_type, m.drug_name, m.specs, m.inpatient_no, m.current_unit,
                                      TRUNC(m.FEE_DATE) ,m.execute_deptcode,m.UNIT_PRICE,m.PACK_QTY,m.fee_code
                            having sum(m.tot_cost) <> 0


                            union all
                            select  
                                   r.patient_no inpatientno,--住院号
                                   m.name,--姓名
                                   r.pact_name feetype,--患者费别
                                   fun_get_dept_name(m.inhos_deptcode) depname,--住院科室名
                                   sum(m.tot_cost) totalfee, --当前费用总计
                                   1 isprintable,--是否可以打印
                                   null prindate,
                                   TRUNC(m.fee_date) startdate, --费用开始时间
                                   null enddate,--费用结束时间
                                   TRUNC(m.fee_date) feedate,--费用日期
                                   (select t.fee_stat_name from fin_com_feecodestat t where t.report_code='ZY01' and t.fee_code=m.fee_code)  itemclass,--项目类型
                                   m.item_name itemname, --项目名称
                                   '1项' standard1,--规格
                                   m.current_unit units, --单位
                                   m.unit_price price, --单价
                                   sum(m.qty) number1,  --数量,
                                   sum(m.qty) itemfee,  --合计,
                                   fun_get_dept_name(m.execute_deptcode) execdept,--执行科室
                                   null note
                              from  fin_ipr_inmaininfo r,fin_ipb_itemlist m
                                      left join (SELECT f.USE_TIME,f.exec_sqn FROM MET_IPM_EXECUNDRUG f) f on f.exec_sqn=m.mo_exec_sqn
                             where m.inpatient_no = r.inpatient_no
                               and r.patient_no='{0}'
                               and m.fee_date >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
                               and m.fee_date < to_date('{2}','yyyy-mm-dd hh24:mi:ss')
                            group by r.patient_no, m.name, r.pact_name, m.inhos_deptcode, m.fee_date, m.item_name, m.inpatient_no, m.current_unit,
                                      TRUNC(m.FEE_DATE) ,m.execute_deptcode,m.UNIT_PRICE,m.fee_code
                            having sum(m.tot_cost)<>0
                            ) order by feedate desc
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, QueryInPatientQlistForSRM.INPATIENTNO, QueryInPatientQlistForSRM.STARTDATE, QueryInPatientQlistForSRM.ENDDATE);

                System.Data.DataTable dt = new System.Data.DataTable();
                //住院查询清单
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryInPatientQlistForSRM = new His.Models.ZZSB.InQueryInPatientQlistForSRM();
                    QueryInPatientQlistForSRM.INPATIENTNO = dt.Rows[i][0].ToString();
                    QueryInPatientQlistForSRM.NAME = dt.Rows[i][1].ToString();
                    QueryInPatientQlistForSRM.FEETYPE = dt.Rows[i][2].ToString();
                    QueryInPatientQlistForSRM.DEPNAME = dt.Rows[i][3].ToString();
                    QueryInPatientQlistForSRM.TOTALFEE = dt.Rows[i][4].ToString();
                    QueryInPatientQlistForSRM.ISPRINTABLE = dt.Rows[i][5].ToString();
                    QueryInPatientQlistForSRM.PRINTDATE = dt.Rows[i][6].ToString();
                    QueryInPatientQlistForSRM.STARTDATE = dt.Rows[i][7].ToString();
                    QueryInPatientQlistForSRM.ENDDATE = dt.Rows[i][8].ToString();
                    QueryInPatientQlistForSRM.FEEDATE = dt.Rows[i][9].ToString();
                    QueryInPatientQlistForSRM.ITEMCLASS = dt.Rows[i][10].ToString();
                    QueryInPatientQlistForSRM.ITEMNAME = dt.Rows[i][11].ToString();
                    QueryInPatientQlistForSRM.STANDARD1 = dt.Rows[i][12].ToString();
                    QueryInPatientQlistForSRM.UNITS = dt.Rows[i][13].ToString();
                    QueryInPatientQlistForSRM.PRICE = dt.Rows[i][14].ToString();
                    QueryInPatientQlistForSRM.NUMBER1 = dt.Rows[i][15].ToString();
                    QueryInPatientQlistForSRM.ITEMFEE = dt.Rows[i][16].ToString();
                    QueryInPatientQlistForSRM.EXECDEPT = dt.Rows[i][17].ToString();
                    QueryInPatientQlistForSRM.NOTE = dt.Rows[i][18].ToString();
                    al.Add(QueryInPatientQlistForSRM);
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

        private string GetQueryInPatientQlistForSRMXML(System.Collections.ArrayList al)
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
                foreach (His.Models.ZZSB.InQueryInPatientQlistForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.INPATIENTNO == "ALL" && p.STARTDATE == "ALL" && p.ENDDATE == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement INPATIENTNO = xml.CreateElement("INPATIENTNO");
                    INPATIENTNO.InnerText = p.INPATIENTNO;
                    Result.AppendChild(INPATIENTNO);

                    System.Xml.XmlElement NAME = xml.CreateElement("NAME");
                    NAME.InnerText = p.NAME;
                    Result.AppendChild(NAME);

                    System.Xml.XmlElement FEETYPE = xml.CreateElement("FEETYPE");
                    FEETYPE.InnerText = p.FEETYPE;
                    Result.AppendChild(FEETYPE);

                    System.Xml.XmlElement DEPNAME = xml.CreateElement("DEPNAME");
                    DEPNAME.InnerText = p.DEPNAME;
                    Result.AppendChild(DEPNAME);

                    System.Xml.XmlElement TOTALFEE = xml.CreateElement("TOTALFEE");
                    TOTALFEE.InnerText = p.TOTALFEE;
                    Result.AppendChild(TOTALFEE);

                    System.Xml.XmlElement ISPRINTABLE = xml.CreateElement("ISPRINTABLE");
                    ISPRINTABLE.InnerText = p.ISPRINTABLE;
                    Result.AppendChild(ISPRINTABLE);

                    System.Xml.XmlElement PRINTDATE = xml.CreateElement("PRINTDATE");
                    PRINTDATE.InnerText = p.PRINTDATE;
                    Result.AppendChild(PRINTDATE);

                    System.Xml.XmlElement STARTDATE = xml.CreateElement("STARTDATE");
                    STARTDATE.InnerText = p.STARTDATE;
                    Result.AppendChild(STARTDATE);

                    System.Xml.XmlElement ENDDATE = xml.CreateElement("ENDDATE");
                    ENDDATE.InnerText = p.ENDDATE;
                    Result.AppendChild(ENDDATE);

                    System.Xml.XmlElement FEEDATE = xml.CreateElement("FEEDATE");
                    FEEDATE.InnerText = p.FEEDATE;
                    Result.AppendChild(FEEDATE);

                    System.Xml.XmlElement ITEMCLASS = xml.CreateElement("ITEMCLASS");
                    ITEMCLASS.InnerText = p.ITEMCLASS;
                    Result.AppendChild(ITEMCLASS);

                    System.Xml.XmlElement ITEMNAME = xml.CreateElement("ITEMNAME");
                    ITEMNAME.InnerText = p.ITEMNAME;
                    Result.AppendChild(ITEMNAME);

                    System.Xml.XmlElement STANDARD1 = xml.CreateElement("STANDARD1");
                    STANDARD1.InnerText = p.STANDARD1;
                    Result.AppendChild(STANDARD1);

                    System.Xml.XmlElement UNITS = xml.CreateElement("UNITS");
                    UNITS.InnerText = p.UNITS;
                    Result.AppendChild(UNITS);

                    System.Xml.XmlElement PRICE = xml.CreateElement("PRICE");
                    PRICE.InnerText = p.PRICE;
                    Result.AppendChild(PRICE);

                    System.Xml.XmlElement NUMBER1 = xml.CreateElement("NUMBER1");
                    NUMBER1.InnerText = p.NUMBER1;
                    Result.AppendChild(NUMBER1);

                    System.Xml.XmlElement ITEMFEE = xml.CreateElement("ITEMFEE");
                    ITEMFEE.InnerText = p.ITEMFEE;
                    Result.AppendChild(ITEMFEE);

                    System.Xml.XmlElement EXECDEPT = xml.CreateElement("EXECDEPT");
                    EXECDEPT.InnerText = p.EXECDEPT;
                    Result.AppendChild(EXECDEPT);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutQueryInPatientQlistForSRMModel(string xml, ref His.Models.ZZSB.InQueryInPatientQlistForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InQueryInPatientQlistForSRM();
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


            System.Xml.XmlNodeList INPATIENTNO1 = doc.GetElementsByTagName("InPatientNo");
            System.Xml.XmlNode INPATIENTNO = INPATIENTNO1[0];
            if (!string.IsNullOrEmpty(INPATIENTNO.InnerText))
            {
                opa.INPATIENTNO = INPATIENTNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "住院号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList STARTDATE1 = doc.GetElementsByTagName("StartDate");
            System.Xml.XmlNode STARTDATE = STARTDATE1[0];
            if (!string.IsNullOrEmpty(STARTDATE.InnerText))
            {
                opa.STARTDATE = STARTDATE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "开始时间不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList ENDDATE1 = doc.GetElementsByTagName("EndDate");
            System.Xml.XmlNode ENDDATE = ENDDATE1[0];
            if (!string.IsNullOrEmpty(ENDDATE.InnerText))
            {
                opa.ENDDATE = ENDDATE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "结束时间不能为空！";
                return this.ReturnFailure();
            }

            return returnStr;
        }



        public string GetOutQueryInPatientQlistForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InQueryInPatientQlistForSRM opa = new His.Models.ZZSB.InQueryInPatientQlistForSRM();
            returnStr = this.GetOutQueryInPatientQlistForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetQueryInPatientQlistForSRMData(opa);
            returnStr = this.GetQueryInPatientQlistForSRMXML(al);
            return returnStr;
        }
    }

    public class QueryOutPatientListForSRM
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


        private System.Collections.ArrayList GetQueryOutPatientListForSRMData(His.Models.ZZSB.InQueryOutPatientListForSRM QueryOutPatientListForSRM)
        {
            #region sql
            //            string sql = @"select 
            //                   distinct(x.invoice_no) INVOICENO， --发票号
            //                   xx.name, --姓名
            //                   fun_get_dept_name(x.reg_dpcd) depname, --科室名称
            //                   fun_get_employee_name(x.doct_code) doctor, --医生
            //                       xx.reg_date regdate,
            //                     sum(x.own_cost+x.pub_cost+x.pay_cost) itemfee--合计
            //              from fin_opb_feedetail x,fin_opr_register xx
            //              where x.clinic_code=xx.clinic_code
            //              and x.trans_type='1'
            //              and x.pay_flag='1'
            //      and not exists(select null from fin_opb_invoiceinfo i where i.invoice_no=x.invoice_no and i.trans_type='2')
            //              --and (x.ext_flag3 is null or x.ext_flag3<>'1')
            //              and xx.card_no='{0}'
            //              --and to_char(xx.reg_date,'yyyy-mm-dd')='{1}'
            //              and xx.reg_date>= to_date('{1}','yyyy-mm-dd')
            //              group by x.invoice_no,x.reg_dpcd,x.doct_code,xx.name,xx.reg_date
            //              order by xx.reg_date desc
            //            ";
            string sql = @" select 
                   distinct(x.invoice_no) INVOICENO， --发票号
                   xx.name, --姓名
                   fun_get_dept_name(x.reg_dpcd) depname, --科室名称
                   fun_get_employee_name(x.doct_code) doctor, --医生
                       x.fee_date regdate,
                     sum(x.own_cost+x.pub_cost+x.pay_cost) itemfee--合计
              from fin_opb_feedetail x,fin_opr_register xx
              where x.clinic_code=xx.clinic_code
              and x.trans_type='1'
              and x.pay_flag='1'
      and not exists(select null from fin_opb_invoiceinfo i where i.invoice_no=x.invoice_no and i.trans_type='2')
              --and (x.ext_flag3 is null or x.ext_flag3<>'1')
              and xx.card_no='{0}'
                and x.invoice_no is not null
              --and to_char(xx.reg_date,'yyyy-mm-dd')='{1}'
              and xx.reg_date>= to_date('{1}','yyyy-mm-dd')
              group by x.invoice_no,x.reg_dpcd,x.doct_code,xx.name,x.fee_date
              order by x.fee_date desc ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, QueryOutPatientListForSRM.CARDNO, QueryOutPatientListForSRM.STARTDATE);

                System.Data.DataTable dt = new System.Data.DataTable();
                //门诊一日清单
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryOutPatientListForSRM = new His.Models.ZZSB.InQueryOutPatientListForSRM();
                    QueryOutPatientListForSRM.INVOICENO = dt.Rows[i][0].ToString();
                    QueryOutPatientListForSRM.NAME = dt.Rows[i][1].ToString();
                    QueryOutPatientListForSRM.DEPNAME = dt.Rows[i][2].ToString();
                    QueryOutPatientListForSRM.DOCTOR = dt.Rows[i][3].ToString(); //+ "/" + dt.Rows[i][4].ToString();
                    QueryOutPatientListForSRM.REGDATE = dt.Rows[i][4].ToString();
                    QueryOutPatientListForSRM.ITEMFEE = dt.Rows[i][5].ToString();
                    al.Add(QueryOutPatientListForSRM);
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

        private string GetQueryOutPatientListForSRMXML(System.Collections.ArrayList al)
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
                foreach (His.Models.ZZSB.InQueryOutPatientListForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.CARDNO == "ALL" && p.STARTDATE == "ALL" && p.ENDDATE == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement INVOICENO = xml.CreateElement("INVOICENO");
                    INVOICENO.InnerText = p.INVOICENO;
                    Result.AppendChild(INVOICENO);

                    System.Xml.XmlElement NAME = xml.CreateElement("NAME");
                    NAME.InnerText = p.NAME;
                    Result.AppendChild(NAME);

                    System.Xml.XmlElement DEPNAME = xml.CreateElement("DEPNAME");
                    DEPNAME.InnerText = p.DEPNAME;
                    Result.AppendChild(DEPNAME);

                    System.Xml.XmlElement DOCTOR = xml.CreateElement("DOCTOR");
                    DOCTOR.InnerText = p.DOCTOR;
                    Result.AppendChild(DOCTOR);

                    System.Xml.XmlElement REGDATE = xml.CreateElement("REGDATE");
                    REGDATE.InnerText = p.REGDATE;
                    Result.AppendChild(REGDATE);

                    System.Xml.XmlElement ITEMFEE = xml.CreateElement("ITEMFEE");
                    ITEMFEE.InnerText = p.ITEMFEE;
                    Result.AppendChild(ITEMFEE);


                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutQueryOutPatientListForSRMModel(string xml, ref His.Models.ZZSB.InQueryOutPatientListForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InQueryOutPatientListForSRM();
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


            System.Xml.XmlNodeList CARDNO1 = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode CARDNO = CARDNO1[0];
            if (!string.IsNullOrEmpty(CARDNO.InnerText))
            {
                opa.CARDNO = CARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "卡号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList STARTDATE1 = doc.GetElementsByTagName("StartDate");
            System.Xml.XmlNode STARTDATE = STARTDATE1[0];
            if (!string.IsNullOrEmpty(STARTDATE.InnerText))
            {
                opa.STARTDATE = STARTDATE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "开始时间不能为空！";
                return this.ReturnFailure();
            }


            return returnStr;
        }



        public string GetOutQueryOutPatientListForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InQueryOutPatientListForSRM opa = new His.Models.ZZSB.InQueryOutPatientListForSRM();
            returnStr = this.GetOutQueryOutPatientListForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetQueryOutPatientListForSRMData(opa);
            returnStr = this.GetQueryOutPatientListForSRMXML(al);
            return returnStr;
        }
    }

    public class QueryOutPatientListOneForSRM
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


        private System.Collections.ArrayList GetQueryOutPatientListOneForSRMData(His.Models.ZZSB.InQueryOutPatientListForSRM QueryOutPatientListOneForSRM)
        {
            #region sql
            string sql = @" select xx.name, --姓名
                   x.invoice_no INVOICENO， --发票号
                   xx.pact_name feetype, --患者费别
                   fun_get_dept_name(x.reg_dpcd) depname, --科室名称
                   fun_get_employee_name(x.doct_code) doctor, --医生
                   '1' isprintable, --是否可以打印
                   trunc(x.reg_date) startdate, -- 费用开始日期
                   trunc(x.reg_date) enddate,
                   trunc(x.fee_date) date1, --费用日期
                   case when x.drug_flag='1' then (select aa.custom_code from pha_com_baseinfo aa where aa.drug_code=x.item_code) else (select bb.gb_code from fin_com_undruginfo bb where bb.item_code=x.item_code) end itemcode, --项目代码
                   x.item_name itemname, --项目名称
                   (select d.fee_stat_name from fin_com_feecodestat d
                                        where d.report_code='MZ01'
                                        and d.fee_code=x.fee_code) invoicetype, --发票发类
                       case when x.drug_flag='1' then (select p.specs from pha_com_baseinfo p where p.drug_code=x.item_code and rownum=1) else x.specs end standard, --规格
                   (select decode(t.center_item_grade,
                                  '1',
                                  '甲类',
                                  '2',
                                  '乙类',
                                  '3',
                                  '丙类' ，null)
                      from fin_com_compare t
                     where t.his_code = x.item_code
                     and t.pact_code='14') feetype1,--医保类别
                     case when x.drug_flag='1' then (select p.min_unit from pha_com_baseinfo p where p.drug_code=x.item_code and rownum=1) else  x.price_unit  end units,--单位
                     round(x.own_cost/x.qty,4),--单价
                     x.qty,--数量
                     x.own_cost itemfee,--合计
                     sysdate printdate,--打印日期
                     null note,
                (select p.pictureurl from elec_outpatientrecord p where p.clinic_code=(x.invoice_no||x.clinic_code) and p.state=0 and rownum=1) AS pictureurl,
 (select '' from elec_outpatientrecord p where p.clinic_code=(x.invoice_no||x.clinic_code) and p.state=0 and rownum=1) AS billqrcode
              from fin_opb_feedetail x,fin_opr_register xx
              where x.clinic_code=xx.clinic_code
              and x.trans_type='1'
              and x.pay_flag='1'
             -- and (x.ext_flag3 is null or x.ext_flag3<>'1')
              and x.invoice_no='{0}'

            ";

            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, QueryOutPatientListOneForSRM.INVOICENO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //门诊一日清单
                //dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Data.DataSet ds = new System.Data.DataSet();
                ds = DataBaseHelp.DataExecHelp.GetDataSet(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    QueryOutPatientListOneForSRM = new His.Models.ZZSB.InQueryOutPatientListForSRM();
                    QueryOutPatientListOneForSRM.NAME = dt.Rows[i][0].ToString();
                    QueryOutPatientListOneForSRM.INVOICENO1 = dt.Rows[i][1].ToString();
                    QueryOutPatientListOneForSRM.FEETYPE = dt.Rows[i][2].ToString();
                    QueryOutPatientListOneForSRM.DEPNAME = dt.Rows[i][3].ToString();
                    QueryOutPatientListOneForSRM.DOCTOR = dt.Rows[i][4].ToString();
                    QueryOutPatientListOneForSRM.ISPRINTABLE = dt.Rows[i][5].ToString();
                    QueryOutPatientListOneForSRM.STARTDATE = dt.Rows[i][6].ToString();
                    QueryOutPatientListOneForSRM.ENDDATE = dt.Rows[i][7].ToString();
                    QueryOutPatientListOneForSRM.DATE1 = dt.Rows[i][8].ToString();
                    QueryOutPatientListOneForSRM.ITEMCODE = dt.Rows[i][9].ToString();
                    QueryOutPatientListOneForSRM.ITEMNAME = dt.Rows[i][10].ToString();
                    QueryOutPatientListOneForSRM.INVOICETYPE = dt.Rows[i][11].ToString();
                    QueryOutPatientListOneForSRM.STANDARD = dt.Rows[i][12].ToString();
                    QueryOutPatientListOneForSRM.FEETYPE1 = dt.Rows[i][13].ToString();
                    QueryOutPatientListOneForSRM.UNITS = dt.Rows[i][14].ToString();
                    QueryOutPatientListOneForSRM.PRICE = dt.Rows[i][15].ToString();
                    QueryOutPatientListOneForSRM.NUMBER1 = dt.Rows[i][16].ToString();
                    QueryOutPatientListOneForSRM.ITEMFEE = dt.Rows[i][17].ToString();
                    QueryOutPatientListOneForSRM.PRINTDATE = dt.Rows[i][18].ToString();
                    QueryOutPatientListOneForSRM.NOTE = dt.Rows[i][19].ToString();
                    QueryOutPatientListOneForSRM.Pictureurl = dt.Rows[i][20].ToString();
                    QueryOutPatientListOneForSRM.Billqrcode = dt.Rows[i][21].ToString();
                    al.Add(QueryOutPatientListOneForSRM);
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

        private string GetQueryOutPatientListOneForSRMXML(System.Collections.ArrayList al)
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
                foreach (His.Models.ZZSB.InQueryOutPatientListForSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.INVOICENO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement NAME = xml.CreateElement("NAME");
                    NAME.InnerText = p.NAME;
                    Result.AppendChild(NAME);

                    System.Xml.XmlElement INVOICENO1 = xml.CreateElement("INVOICENO1");
                    INVOICENO1.InnerText = p.INVOICENO1;
                    Result.AppendChild(INVOICENO1);

                    System.Xml.XmlElement FEETYPE = xml.CreateElement("FEETYPE");
                    FEETYPE.InnerText = p.FEETYPE;
                    Result.AppendChild(FEETYPE);

                    System.Xml.XmlElement DEPNAME = xml.CreateElement("DEPNAME");
                    DEPNAME.InnerText = p.DEPNAME;
                    Result.AppendChild(DEPNAME);

                    System.Xml.XmlElement DOCTOR = xml.CreateElement("DOCTOR");
                    DOCTOR.InnerText = p.DOCTOR;
                    Result.AppendChild(DOCTOR);

                    System.Xml.XmlElement ISPRINTABLE = xml.CreateElement("ISPRINTABLE");
                    ISPRINTABLE.InnerText = p.ISPRINTABLE;
                    Result.AppendChild(ISPRINTABLE);

                    System.Xml.XmlElement STARTDATE = xml.CreateElement("STARTDATE");
                    STARTDATE.InnerText = p.STARTDATE;
                    Result.AppendChild(STARTDATE);

                    System.Xml.XmlElement ENDDATE = xml.CreateElement("ENDDATE");
                    ENDDATE.InnerText = p.ENDDATE;
                    Result.AppendChild(ENDDATE);

                    System.Xml.XmlElement DATE1 = xml.CreateElement("DATE1");
                    DATE1.InnerText = p.DATE1;
                    Result.AppendChild(DATE1);

                    System.Xml.XmlElement ITEMCODE = xml.CreateElement("ITEMCODE");
                    ITEMCODE.InnerText = p.ITEMCODE;
                    Result.AppendChild(ITEMCODE);

                    System.Xml.XmlElement ITEMNAME = xml.CreateElement("ITEMNAME");
                    ITEMNAME.InnerText = p.ITEMNAME;
                    Result.AppendChild(ITEMNAME);

                    System.Xml.XmlElement INVOICETYPE = xml.CreateElement("INVOICETYPE");
                    INVOICETYPE.InnerText = p.INVOICETYPE;
                    Result.AppendChild(INVOICETYPE);

                    System.Xml.XmlElement STANDARD = xml.CreateElement("STANDARD");
                    STANDARD.InnerText = p.STANDARD;
                    Result.AppendChild(STANDARD);

                    System.Xml.XmlElement FEETYPE1 = xml.CreateElement("FEETYPE1");
                    FEETYPE1.InnerText = p.FEETYPE1;
                    Result.AppendChild(FEETYPE1);

                    System.Xml.XmlElement UNITS = xml.CreateElement("UNITS");
                    UNITS.InnerText = p.UNITS;
                    Result.AppendChild(UNITS);

                    System.Xml.XmlElement PRICE = xml.CreateElement("PRICE");
                    PRICE.InnerText = p.PRICE;
                    Result.AppendChild(PRICE);

                    System.Xml.XmlElement NUMBER1 = xml.CreateElement("NUMBER1");
                    NUMBER1.InnerText = p.NUMBER1;
                    Result.AppendChild(NUMBER1);

                    System.Xml.XmlElement ITEMFEE = xml.CreateElement("ITEMFEE");
                    ITEMFEE.InnerText = p.ITEMFEE;
                    Result.AppendChild(ITEMFEE);

                    System.Xml.XmlElement PRINTDATE = xml.CreateElement("PRINTDATE");
                    PRINTDATE.InnerText = p.PRINTDATE;
                    Result.AppendChild(PRINTDATE);

                    System.Xml.XmlElement NOTE = xml.CreateElement("NOTE");
                    NOTE.InnerText = p.NOTE;
                    Result.AppendChild(NOTE);


                    System.Xml.XmlElement PICTUREURL = xml.CreateElement("PICTUREURL");
                    PICTUREURL.InnerText = p.Pictureurl;
                    Result.AppendChild(PICTUREURL);


                    System.Xml.XmlElement BILLQRCODE = xml.CreateElement("BILLQRCODE");
                    BILLQRCODE.InnerText = p.Billqrcode;
                    Result.AppendChild(BILLQRCODE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutQueryOutPatientListOneForSRMModel(string xml, ref His.Models.ZZSB.InQueryOutPatientListForSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InQueryOutPatientListForSRM();
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


            System.Xml.XmlNodeList INVOICENO1 = doc.GetElementsByTagName("InvoiceNo");
            System.Xml.XmlNode INVOICENO = INVOICENO1[0];
            if (!string.IsNullOrEmpty(INVOICENO.InnerText))
            {
                opa.INVOICENO = INVOICENO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "发票号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }


            return returnStr;
        }



        public string GetOutQueryOutPatientListOneForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InQueryOutPatientListForSRM opa = new His.Models.ZZSB.InQueryOutPatientListForSRM();
            returnStr = this.GetOutQueryOutPatientListOneForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetQueryOutPatientListOneForSRMData(opa);
            returnStr = this.GetQueryOutPatientListOneForSRMXML(al);
            return returnStr;
        }
    }

    public class JudgeIDCardHasFileForSRM
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


        private System.Collections.ArrayList GetJudgeIDCardHasFileForSRMData(His.Models.ZZSB.InJudgeIDCardHasFileSRM JudgeIDCardHasFileForSRM)
        {
            #region sql
            string sql = @"
            select '身份证' IDCardType,
       d.idenno IDCardNO,
       d.name,
       d.sex_code sex,
       d.birthday,
       fun_get_age(d.birthday) age
  from com_patientinfo d
  where d.idenno='{0}'
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, JudgeIDCardHasFileForSRM.IDCARDNO);

                System.Data.DataTable dt = new System.Data.DataTable();
                //判断身份证是否已经建档
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JudgeIDCardHasFileForSRM = new His.Models.ZZSB.InJudgeIDCardHasFileSRM();
                    JudgeIDCardHasFileForSRM.IDCARDTYPE = dt.Rows[i][0].ToString();
                    JudgeIDCardHasFileForSRM.IDCARDNO = dt.Rows[i][1].ToString();
                    JudgeIDCardHasFileForSRM.NAME = dt.Rows[i][2].ToString();
                    JudgeIDCardHasFileForSRM.SEX = dt.Rows[i][3].ToString();
                    JudgeIDCardHasFileForSRM.BIRTHDAY = dt.Rows[i][4].ToString();
                    JudgeIDCardHasFileForSRM.AGE = dt.Rows[i][5].ToString();
                    al.Add(JudgeIDCardHasFileForSRM);
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

        private string GetJudgeIDCardHasFileForSRMXML(System.Collections.ArrayList al)
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
                foreach (His.Models.ZZSB.InJudgeIDCardHasFileSRM p in al)
                {

                    System.Xml.XmlElement Result = xml.CreateElement("Result");
                    root1.AppendChild(Result);

                    if (p.IDCARDNO == "ALL")
                    {
                        return this.ERR();
                    }

                    System.Xml.XmlElement IDCARDTYPE = xml.CreateElement("IDCARDTYPE");
                    IDCARDTYPE.InnerText = p.IDCARDTYPE;
                    Result.AppendChild(IDCARDTYPE);

                    System.Xml.XmlElement IDCARDNO = xml.CreateElement("IDCARDNO");
                    IDCARDNO.InnerText = p.IDCARDNO;
                    Result.AppendChild(IDCARDNO);

                    System.Xml.XmlElement NAME = xml.CreateElement("NAME");
                    NAME.InnerText = p.NAME;
                    Result.AppendChild(NAME);

                    System.Xml.XmlElement SEX = xml.CreateElement("SEX");
                    SEX.InnerText = p.SEX;
                    Result.AppendChild(SEX);

                    System.Xml.XmlElement BIRTHDAY = xml.CreateElement("BIRTHDAY");
                    BIRTHDAY.InnerText = p.BIRTHDAY;
                    Result.AppendChild(BIRTHDAY);

                    System.Xml.XmlElement AGE = xml.CreateElement("AGE");
                    AGE.InnerText = p.AGE;
                    Result.AppendChild(AGE);

                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private string GetOutJudgeIDCardHasFileForSRMModel(string xml, ref His.Models.ZZSB.InJudgeIDCardHasFileSRM opa)
        {

            string returnStr = "";
            opa = new His.Models.ZZSB.InJudgeIDCardHasFileSRM();
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


            System.Xml.XmlNodeList IDCARDNO1 = doc.GetElementsByTagName("IDCardNo");
            System.Xml.XmlNode IDCARDNO = IDCARDNO1[0];
            if (!string.IsNullOrEmpty(IDCARDNO.InnerText))
            {
                opa.IDCARDNO = IDCARDNO.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "身份证号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList DEVICEID1 = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode DEVICEID = DEVICEID1[0];
            if (!string.IsNullOrEmpty(DEVICEID.InnerText))
            {
                opa.DEVICEID = DEVICEID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList SERVICECODE1 = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode SERVICECODE = SERVICECODE1[0];
            if (!string.IsNullOrEmpty(SERVICECODE.InnerText))
            {
                opa.SERVICECODE = SERVICECODE.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编号不能为空！";
                return this.ReturnFailure();
            }


            return returnStr;
        }



        public string GetOutQueryOutPatientListOneForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.InJudgeIDCardHasFileSRM opa = new His.Models.ZZSB.InJudgeIDCardHasFileSRM();
            returnStr = this.GetOutJudgeIDCardHasFileForSRMModel(xml, ref opa);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            System.Collections.ArrayList al = this.GetJudgeIDCardHasFileForSRMData(opa);
            returnStr = this.GetJudgeIDCardHasFileForSRMXML(al);
            return returnStr;
        }
    }

}
