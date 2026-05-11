using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EMPIService.EMPIService
{
    public class EMPIService
    {
        string err = "";

        public System.Collections.ArrayList GetPatientFunction(string patientNo/*,int count*/)
        {
            #region sql
            string sql = @"
                            select p.card_no,
                                   p.name,
                                   p.sex_code,
                                   trunc(p.birthday),
                                   p.pact_code,
                                   p.paykind_code,
                                   p.mcard_no,
                                   p.home_tel,
                                   p.home,
                                   p.idenno,
                                   p.mark,
                                   p.normalname,
                                   p.is_encryptname
                            from com_patientinfo p 
                            left join empi_paitinetinfo ep on p.card_no=ep.card_no
where p.oper_date>sysdate-1
and ep.card_no is null
--and p.idenno is not null
                            ";
            /*p.fir_see_date>=(to_date('2012-06-01 00:00:00','yyyy-mm-dd hh24:mi:ss')+{0})
and p.fir_see_date<(to_date('2012-06-02 00:00:00','yyyy-mm-dd hh24:mi:ss')+{0})*/
            #endregion
            try
            {
                #region 数据赋值
                //sql = string.Format(sql, patientNo);
               // sql = string.Format(sql,patientNo/*, count*/);
                System.Data.DataTable dt = new System.Data.DataTable();
                //申请单
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                if (dt==null && dt.Rows.Count==0)
                {
                    return null;
                }
                System.Collections.ArrayList al = new System.Collections.ArrayList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Neusoft.HISFC.Models.RADT.Patient p = new Neusoft.HISFC.Models.RADT.Patient();
                    p.PID.CardNO = dt.Rows[i][0].ToString();
                    p.Name = dt.Rows[i][1].ToString();
                    p.Sex.ID = dt.Rows[i][2].ToString();
                    try
                    {
                        p.Birthday = DateTime.Parse(dt.Rows[i][3].ToString());
                    }
                    catch { }
                    p.Pact.ID = dt.Rows[i][4].ToString();
                    p.Pact.PayKind.ID = dt.Rows[i][5].ToString();
                    p.SSN = dt.Rows[i][6].ToString();
                    p.PhoneHome = dt.Rows[i][7].ToString();
                    p.AddressHome = dt.Rows[i][8].ToString();
                    p.IDCard = dt.Rows[i][9].ToString();
                    p.Memo = dt.Rows[i][10].ToString();
                    p.NormalName = dt.Rows[i][11].ToString();
                    if (dt.Rows[i][12].ToString() == "1")
                    {
                        p.IsEncrypt = true;
                    }
                    else
                    {
                        p.IsEncrypt = false;
                    }
                    al.Add(p);
                }

                #endregion
                return al;
            }
            catch { return null; }
        }

        public int PushEMPIPatientInfo(string patientNo,string patientType)
        {
            string strEmpi="";
            EMPIWebReference.EMPI regEmpi = new global::EMPIService.EMPIWebReference.EMPI();
            System.Collections.ArrayList al = new System.Collections.ArrayList();
            patientType = "I";
            //for (int k = 0; k < 1262; k++)
            //{
            His.Util.Common.HisLog.WriteLog("EMPI", "查询待注册EMPI的患者.");
            int i = -1;
            
                al = this.GetPatientFunction(patientNo);
                His.Util.Common.HisLog.WriteLog("EMPI", "待注册的患者一共有:" + al.Count.ToString());
                if (al == null || al.Count == 0)
                {
                    return i;
                }
               
                foreach (Neusoft.HISFC.Models.RADT.Patient p in al)
                {
                    string patientInfo = this.GetPatientEmpiXML(p, patientType);
                    His.Util.Common.HisLog.WriteLog("EMPI", patientInfo);
                    strEmpi = regEmpi.regEmpi(patientInfo);
                    His.Util.Common.HisLog.WriteLog("EMPI", strEmpi);
                    i = this.InsertEMPIPatientInfo(strEmpi, p);
                   
                }
               return i;
            //}
            //return 0;
        }

        /// <summary>
        /// 获取patient XML并返回
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public string GetPatientEmpiXML(Neusoft.HISFC.Models.RADT.Patient patient, string pt)
        {
            #region 造xml
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            System.Xml.XmlElement PATIENTINFO = xml.CreateElement("PATIENTINFO");
            xml.AppendChild(PATIENTINFO);

            System.Xml.XmlElement PATIENT = xml.CreateElement("PATIENT");
            PATIENTINFO.AppendChild(PATIENT);

            System.Xml.XmlElement NAME = xml.CreateElement("NAME");
            NAME.InnerText = patient.Name;
            PATIENT.AppendChild(NAME);

            System.Xml.XmlElement IDNO = xml.CreateElement("IDNO");
            IDNO.InnerText = patient.IDCard;
            PATIENT.AppendChild(IDNO);

            System.Xml.XmlElement SEX = xml.CreateElement("SEX");
            SEX.InnerText = patient.Sex.ID.ToString();
            PATIENT.AppendChild(SEX);

            System.Xml.XmlElement BIRTHDAY = xml.CreateElement("BIRTHDAY");
            BIRTHDAY.InnerText = patient.Birthday.ToString("yyyy-MM-dd");
            PATIENT.AppendChild(BIRTHDAY);

            System.Xml.XmlElement CNY = xml.CreateElement("CNY");
            CNY.InnerText = "";
            PATIENT.AppendChild(CNY);

            System.Xml.XmlElement CNYNAME = xml.CreateElement("CNYNAME");
            CNYNAME.InnerText = "";
            PATIENT.AppendChild(CNYNAME);

            System.Xml.XmlElement ACT = xml.CreateElement("ACT");
            ACT.InnerText = "";
            PATIENT.AppendChild(ACT);

            System.Xml.XmlElement ADDR = xml.CreateElement("ADDR");
            ADDR.InnerText = patient.AddressHome;
            PATIENT.AppendChild(ADDR);

            System.Xml.XmlElement ZPCODE = xml.CreateElement("ZPCODE");
            ZPCODE.InnerText = "";
            PATIENT.AppendChild(ZPCODE);

            System.Xml.XmlElement ABOBLD = xml.CreateElement("ABOBLD");
            ABOBLD.InnerText = "";
            PATIENT.AppendChild(ABOBLD);

            System.Xml.XmlElement RHBLD = xml.CreateElement("RHBLD");
            RHBLD.InnerText = "";
            PATIENT.AppendChild(RHBLD);

            System.Xml.XmlElement NTN = xml.CreateElement("NTN");
            NTN.InnerText = "";
            PATIENT.AppendChild(NTN);

            System.Xml.XmlElement BCP = xml.CreateElement("BCP");
            BCP.InnerText = "";
            PATIENT.AppendChild(BCP);

            System.Xml.XmlElement CTOR = xml.CreateElement("CTOR");
            CTOR.InnerText = "";
            PATIENT.AppendChild(CTOR);

            System.Xml.XmlElement CTORTEL = xml.CreateElement("CTORTEL");
            CTORTEL.InnerText = "";
            PATIENT.AppendChild(CTORTEL);

            System.Xml.XmlElement CTORLTN = xml.CreateElement("CTORLTN");
            CTORLTN.InnerText = "";
            PATIENT.AppendChild(CTORLTN);

            System.Xml.XmlElement MOBILE = xml.CreateElement("MOBILE");
            MOBILE.InnerText = "";
            PATIENT.AppendChild(MOBILE);

            System.Xml.XmlElement EML = xml.CreateElement("EML");
            EML.InnerText = "";
            PATIENT.AppendChild(EML);

            System.Xml.XmlElement CPY = xml.CreateElement("CPY");
            CPY.InnerText = "";
            PATIENT.AppendChild(CPY);

            System.Xml.XmlElement CPYTEL = xml.CreateElement("CPYTEL");
            CPYTEL.InnerText = "";
            PATIENT.AppendChild(CPYTEL);

            System.Xml.XmlElement MRG = xml.CreateElement("MRG");
            MRG.InnerText = "";
            PATIENT.AppendChild(MRG);

            System.Xml.XmlElement PFSN = xml.CreateElement("PFSN");
            PFSN.InnerText = "";
            PATIENT.AppendChild(PFSN);

            System.Xml.XmlElement MEMO = xml.CreateElement("MEMO");
            MEMO.InnerText = "";
            PATIENT.AppendChild(MEMO);

            System.Xml.XmlElement CARDINFOS = xml.CreateElement("CARDINFOS");
            PATIENTINFO.AppendChild(CARDINFOS);

            System.Xml.XmlElement CARD = xml.CreateElement("CARD");
            CARDINFOS.AppendChild(CARD);

            System.Xml.XmlElement CARDNO = xml.CreateElement("CARDNO");
            CARDNO.InnerText = patient.PID.CardNO;
            CARD.AppendChild(CARDNO);

            System.Xml.XmlElement CARDTYPE = xml.CreateElement("CARDTYPE");
            CARDTYPE.InnerText = pt;
            CARD.AppendChild(CARDTYPE);

            System.Xml.XmlElement OPERCODE = xml.CreateElement("OPERCODE");
            OPERCODE.InnerText = "";
            CARD.AppendChild(OPERCODE);

            System.Xml.XmlElement OPERNAME = xml.CreateElement("OPERNAME");
            OPERNAME.InnerText = "";
            CARD.AppendChild(OPERNAME);

            System.Xml.XmlElement DOMAIN = xml.CreateElement("DOMAIN");
            DOMAIN.InnerText = "001";
            PATIENTINFO.AppendChild(DOMAIN);
            #endregion

            return xml.InnerXml.ToString();
        }

        /// <summary>
        /// 插入数据库对照关系
        /// </summary>
        /// <param name="empiNo"></param>
        /// <param name="patient"></param>
        /// <returns></returns>
        public int InsertEMPIPatientInfo(string empiNo, Neusoft.HISFC.Models.RADT.Patient patient)
        {

            #region sql
            string sql = @" insert into empi_paitinetinfo
                            (card_no,empi_no,oper_date)
                            values('{0}', '{1}',sysdate)
                            ";
            sql = string.Format(sql,patient.PID.CardNO, empiNo);
            #endregion

            try
            {
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                {
                    His.Util.Common.HisLog.WriteLog("EMPI", err);
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
            return 1;
        }
    }
}