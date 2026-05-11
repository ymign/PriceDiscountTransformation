using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.LIS
{
    public class InPatientApply
    {
        public int PushLisInpatientApplyByInpatientNo(string inpatientNo)
        {
            LisService.LisBusiness.LisService ls = new LisService.LisBusiness.LisService();
            ls.PushInPatientApply(inpatientNo);
            return 1;
        }

        /// <summary>
        /// 推送住院申请单
        /// </summary>
        /// <param name="inpatientNoList"></param>
        /// <returns></returns>
        public int PushLisInpatientApplyByInpatientNoList(List<string> inpatientNoList)
        {
            inpatientNoList = this.GetPushInaptientList();
            if (inpatientNoList.Count() > 0)
            {
                foreach (string inpatientNo in inpatientNoList)
                {
                    this.PushLisInpatientApplyByInpatientNo(inpatientNo);
                }
            }
            return 1;
        }

        /// <summary>
        /// 获取一天内所有未推送的申请进行推送
        /// </summary>
        /// <returns></returns>
        private List<string> GetPushInaptientList()
        {
            List<string> l = new List<string>();
             #region sql
            string sql = @" 
                        select  distinct o.inpatient_no
                      from met_ipm_order o,met_ipm_execundrug il
                     where il.mo_order=o.mo_order
                       and o.mo_stat in ('1','2')
                       and o.type_code in ('CZ','LZ','BL','SQ','SH')
                       and o.mo_date>sysdate -10
                       and o.class_code = 'UL'
                       and il.lab_barcode is  null
                            ";
             #endregion
            try
            {
                #region 数据赋值

                System.Data.DataTable dt = new System.Data.DataTable();
                //申请单
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    l.Add(dt.Rows[i][0].ToString());
                }
                #endregion
            }
            catch { }
            return l;
        }

        /// <summary>
        /// 更新条码打印状态
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private int LisBarcodeInPrintNotificationData(His.Models.LIS.InPatientApply inpatientApply,ref string err)
        {
            #region sql
            string sql = @" 
        update met_ipm_execundrug t
        set t.lab_barcode = '{0}'
        where t.mo_order = '{1}'
        --and t.exec_flag = '1'
        and t.charge_state = '1'
        and t.class_code = 'UL'
        and (t.inpatient_no='{2}' or '{2}'='ALL')
        and not exists (select g.*
            from fin_ipb_itemlist g
           where g.inpatient_no = t.inpatient_no
             and g.trans_type = '2'
             and (g.inpatient_no='{2}' or '{2}'='ALL')
             and g.mo_exec_sqn = t.exec_sqn)
        ";
            string sqlOrder = @"update met_ipm_order o
        set o.mark2='{0}'
        where o.mo_order='{1}' and  o.inpatient_no='{2}'
         and o.class_code = 'UL'";
            sql = string.Format(sql, inpatientApply.EXEC_STATUS, inpatientApply.APLY_ID, inpatientApply.PTNT_ID);
            sqlOrder = string.Format(sqlOrder, inpatientApply.EXEC_STATUS, inpatientApply.APLY_ID, inpatientApply.PTNT_ID);
            #endregion
            try
            {
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql,ref err))
                {
                    return -1;
                }
                if (!DataBaseHelp.DataExecHelp.ExecSql(sqlOrder, ref err))
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
                    sql2 = string.Format(sql2, inpatientApply.PTNT_ID, "住院条码打印",
                        inpatientApply.EMPI + "||" + inpatientApply.APLY_ID + "||" + inpatientApply.APLY_FLOW_NUM + "||" + inpatientApply.BARCODE + "||" + inpatientApply.REMARK + "||" + inpatientApply.EXEC_STATUS);
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
        /// 更新条码打印状态
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private int LisBarcodeOutPrintNotificationData(His.Models.LIS.InPatientApply inpatientApply, ref string err)
        {
            #region sql
            string sql = @" 
        update fin_opb_feedetail t
     set t.SAMPLE_ID = '{1}'
   where t.MO_ORDER ='{0}'
        ";
            sql = string.Format(sql, inpatientApply.APLY_ID,inpatientApply.EXEC_STATUS);
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
                    sql2 = string.Format(sql2, inpatientApply.PTNT_ID, "门诊条码打印",
                        inpatientApply.EMPI + "||" + inpatientApply.APLY_ID + "||" + inpatientApply.APLY_FLOW_NUM + "||" + inpatientApply.BARCODE + "||" + inpatientApply.REMARK);
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
        /// 样本接收确认通知(终端确认)
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        private int LisSampleReceivedNotificationData(His.Models.LIS.InPatientApply inpatientApply,ref string err)
        {
            #region sql
            string sql = "";
            if (inpatientApply.EXEC_STATUS == "1")
            {
                 sql = @" 
            update fin_ipb_itemlist t
            set t.apprno='{2}',
                t.noback_num=0
            where t.mo_order='{0}'
            and t.inpatient_no='{1}'
            ";
            }
            else
            {
                 sql = @" 
            update fin_ipb_itemlist t
            set t.apprno='{2}',
                t.noback_num=t.qty
            where t.mo_order='{0}'
            and t.inpatient_no='{1}'
            ";
            }

             sql = string.Format(sql, inpatientApply.APLY_ID, inpatientApply.PTNT_ID, inpatientApply.EXEC_STATUS);
            #endregion
             try
             {
                 if (!DataBaseHelp.DataExecHelp.ExecSql(sql,ref err))
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
                     sql2 = string.Format(sql2, inpatientApply.PTNT_ID, "住院样本接收",
                         inpatientApply.EMPI + "||" + inpatientApply.APLY_ID + "||" + inpatientApply.APLY_FLOW_NUM + "||" + inpatientApply.BARCODE + "||" + inpatientApply.REMARK + "||" + inpatientApply.EXEC_STATUS);
                     DataBaseHelp.DataExecHelp.ExecSql(sql2, ref err);
                    // His.Util.Common.HisLog.WriteLog("LisInpatient", sql2);
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
        /// 提取申请单信息（条码打印、样本接收公用）
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private His.Models.LIS.InPatientApply GetInPatientModel(string xml)
        {
            His.Models.LIS.InPatientApply opa = new His.Models.LIS.InPatientApply();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return opa;
            }

            System.Xml.XmlNodeList PTNT_ID_list = doc.GetElementsByTagName("PTNT_ID");
            System.Xml.XmlNode PTNT_ID = PTNT_ID_list[0];
            if (!string.IsNullOrEmpty(PTNT_ID.InnerText))
            {
                opa.PTNT_ID = PTNT_ID.InnerText;
            }
            else
            {
                opa.PTNT_ID = "ALL";
            }

            System.Xml.XmlNodeList PATIENT_TYPE_list = doc.GetElementsByTagName("PATIENT_TYPE");
            System.Xml.XmlNode PATIENT_TYPE = PATIENT_TYPE_list[0];
            if (!string.IsNullOrEmpty(PATIENT_TYPE.InnerText))
            {
                opa.PATIENT_TYPE = PATIENT_TYPE.InnerText;
            }
            else
            {
                opa.PATIENT_TYPE = "ALL";
            }

            System.Xml.XmlNodeList EMPI_list = doc.GetElementsByTagName("EMPI");
            System.Xml.XmlNode EMPI = EMPI_list[0];
            if (!string.IsNullOrEmpty(EMPI.InnerText))
            {
                opa.EMPI = EMPI.InnerText;
            }
            else
            {
                opa.EMPI = "ALL";
            }

            System.Xml.XmlNodeList APLY_ID_list = doc.GetElementsByTagName("APLY_ID");
            System.Xml.XmlNode APLY_ID = APLY_ID_list[0];
            if (!string.IsNullOrEmpty(APLY_ID.InnerText))
            {
                opa.APLY_ID = APLY_ID.InnerText;
            }
            else
            {
                opa.APLY_ID = "ALL";
            }

            System.Xml.XmlNodeList APLY_FLOW_NUM_list = doc.GetElementsByTagName("APLY_FLOW_NUM");
            System.Xml.XmlNode APLY_FLOW_NUM = APLY_FLOW_NUM_list[0];
            if (!string.IsNullOrEmpty(APLY_FLOW_NUM.InnerText))
            {
                opa.APLY_FLOW_NUM = APLY_FLOW_NUM.InnerText;
            }
            else
            {
                opa.APLY_FLOW_NUM = "ALL";
            }

            System.Xml.XmlNodeList APLY_SRC_list = doc.GetElementsByTagName("APLY_SRC");
            System.Xml.XmlNode APLY_SRC = APLY_SRC_list[0];
            if (!string.IsNullOrEmpty(APLY_SRC.InnerText))
            {
                opa.APLY_SRC = APLY_SRC.InnerText;
            }
            else
            {
                opa.APLY_SRC = "ALL";
            }

            System.Xml.XmlNodeList BARCODE_list = doc.GetElementsByTagName("BARCODE");
            System.Xml.XmlNode BARCODE = BARCODE_list[0];
            if (!string.IsNullOrEmpty(BARCODE.InnerText))
            {
                opa.BARCODE = BARCODE.InnerText;
            }
            else
            {
                opa.BARCODE = "ALL";
            }

            System.Xml.XmlNodeList EXEC_STATUS_list = doc.GetElementsByTagName("EXEC_STATUS");
            System.Xml.XmlNode EXEC_STATUS = EXEC_STATUS_list[0];
            if (!string.IsNullOrEmpty(EXEC_STATUS.InnerText))
            {
                opa.EXEC_STATUS = EXEC_STATUS.InnerText;
            }
            else
            {
                opa.EXEC_STATUS = "ALL";
            }

            System.Xml.XmlNodeList REMARK_list = doc.GetElementsByTagName("REMARK");
            System.Xml.XmlNode REMARK = REMARK_list[0];
            if (!string.IsNullOrEmpty(REMARK.InnerText))
            {
                opa.REMARK = REMARK.InnerText;
            }
            else
            {
                opa.REMARK = "ALL";
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
        public string LisBarcodePrintNotification(string xml)
        {
            int i = -1;
            string err = "";
            His.Models.LIS.InPatientApply ipa = new His.Models.LIS.InPatientApply();
            ipa=this.GetInPatientModel(xml);
            if (ipa.PATIENT_TYPE == "1")
            {
                i = this.LisBarcodeInPrintNotificationData(ipa, ref err);
            }
            if (ipa.PATIENT_TYPE == "0")
            {
                i = this.LisBarcodeOutPrintNotificationData(ipa, ref err);
            }
            else
            {
                i = -1;
                err = "传入患者类别有误，请核实（0门诊 1住院）";
            }
            this.GetLisReturnResult(i, ref err);
            return err;
        }

        /// <summary>
        /// 样本接收确认通知(终端确认)
        /// </summary>
        /// <param name="inpatientApply"></param>
        /// <returns></returns>
        public string LisSampleReceivedNotification(string xml)
        {
            int i = -1;
            string err = "";
            His.Models.LIS.InPatientApply ipa = new His.Models.LIS.InPatientApply();
            ipa = this.GetInPatientModel(xml);
            i = this.LisSampleReceivedNotificationData(ipa,ref err);
            this.GetLisReturnResult(i, ref err);
            return err;
        }

        #region 住院申请单LIS主动获取
        private System.Collections.ArrayList GetInPatientApplyData(His.Models.LIS.InPatientApply inPatientApply)
        {
            #region sql
            string sql = @" select          o.mo_order aply_detl_id, --    　  PK
                                    0 aply_src, --申请来源 0 - HIS系统；1 - LIS系统；2 - 体检系统；3 - 其他来源
                                    o.mo_order aply_flow_num, --申请流水号
                                    sysdate aply_create_date, --申请创建日期
                                    case o.emc_flag
                                      when '1' then
                                       '1'
                                      else
                                       '0'
                                    end emcy_mrk, --急诊标记(急诊赋1)
                                    o.mo_date aply_date, --申请日期
                                    o.list_dpcd dept_key, --  申请来源科室编码
                                    fun_get_dept_name(o.list_dpcd) dept_name, --申请来源科室名称
                                    o.doc_code doc_key, --      　  申请医生工号
                                    o.doc_name doc_name, -- 申请医生名称
                                    o.inpatient_no ptnt_id, --    就诊患者ID
                                    im.in_times visit_id,--就诊次数
                                    im.idenno id_card, --身份证
                                    ltrim(o.patient_no, '0') ic_card, --      　  IC卡号
                                    im.home ctat_addr,--联系地址
                                    im.home_tel phone_num,--联系电话
                                    o.patient_no ptnt_no, --    　  病历号
                                    1 ptnt_no_type, --    病历号类型：0-门诊号；1-住院号；2-体检号；
                                    im.name ptnt_name, --    　  患者姓名
                                    decode(nvl(im.sex_code, '0'), 'M', 1, 'F', 2, '0', 0, 3) ptnt_sex, --  性别：1-男；2-女；3-性别不明确
                                    fun_get_age_new(im.birthday,sysdate) ptnt_age, --    　  患者年龄
                                    null ptnt_age_unit, --      年龄类型：0-岁；1-月；2-天；3-时
                                    im.in_date admisse_date, --      入院日期
                                    im.birthday ptnt_birth, --      出生日期
                                    substr(im.bed_no, 5) ptnt_bed_no, --    　  床号
                                    (select max(m.diag_name)
                                       from met_cas_diagnose m
                                      where o.main_drug = '1'
                                        and m.inpatient_no = o.inpatient_no) diag_info, --诊断信息
                                    o.item_code aply_itm_key, --        申请项目对照编码
                                    o.item_name aply_itm_name, --      　  申请项目名称
                                    (select y.code from com_dictionary y
                           where y.type='LABSAMPLE'
                           and y.name=o.LAB_CODE)  smpl_key, --        样本类型编码

                                    o.LAB_CODE smpl_name, --        样本类型
                                   /* (select y.name from com_dictionary y
                           where y.type='LABSAMPLE'
                           and y.code=t.LAB_CODE) smpl_name, --        样本类型*/
                                    null body_part,--取材部位
                                    o.mo_note1 remark,--执行说明
                                     decode((select sum(nvl(il.lab_barcode,0)) from met_ipm_execundrug il
                                     where il.mo_order=o.mo_order ),0, 0, 1) exec_status, --    执行状态
                                    (select empi.empi_no from  EMPI_PAITINETINFO empi
                                    where empi.card_no=im.patient_no and rownum=1) as empi
                      from met_ipm_order o, fin_ipr_inmaininfo im
                     where o.mo_stat in ('1','2')
                       and o.class_code = 'UL'
                       and o.type_name not like '%嘱托%'
                       and o.inpatient_no = im.inpatient_no
                       and im.inpatient_no='{0}'
            ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql,inPatientApply.PTNT_ID);

                System.Data.DataTable dt = new System.Data.DataTable();
                //申请单
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    inPatientApply = new His.Models.LIS.InPatientApply();
                    inPatientApply.APLY_ID = dt.Rows[i][0].ToString();
                    inPatientApply.APLY_SRC = dt.Rows[i][1].ToString();
                    inPatientApply.APLY_FLOW_NUM = dt.Rows[i][2].ToString();
                    inPatientApply.APLY_CREATE_DATE = dt.Rows[i][3].ToString();
                    inPatientApply.EMCY_MRK = dt.Rows[i][4].ToString();
                    inPatientApply.APLY_DATE = dt.Rows[i][5].ToString();
                    inPatientApply.DEPT_KEY = dt.Rows[i][6].ToString();
                    inPatientApply.DEPT_NAME = dt.Rows[i][7].ToString();
                    inPatientApply.DOC_KEY = dt.Rows[i][8].ToString();
                    inPatientApply.DOC_NAME = dt.Rows[i][9].ToString();
                    inPatientApply.PTNT_ID = dt.Rows[i][10].ToString();
                    inPatientApply.VISIT_ID = dt.Rows[i][11].ToString();
                    inPatientApply.ID_CARD = dt.Rows[i][12].ToString();
                    inPatientApply.IC_CARD = dt.Rows[i][13].ToString();
                    inPatientApply.CTAT_ADDR = dt.Rows[i][14].ToString();
                    inPatientApply.PHONE_NUM = dt.Rows[i][15].ToString();
                    inPatientApply.PTNT_NO = dt.Rows[i][16].ToString();
                    inPatientApply.PTNT_NO_TYPE = dt.Rows[i][17].ToString();
                    inPatientApply.PTNT_NAME = dt.Rows[i][18].ToString();
                    inPatientApply.PTNT_SEX = dt.Rows[i][19].ToString();
                    inPatientApply.PTNT_AGE = dt.Rows[i][20].ToString();
                    inPatientApply.PTNT_AGE_UNIT = dt.Rows[i][21].ToString();
                    inPatientApply.ADMISSE_DATE = dt.Rows[i][22].ToString();
                    inPatientApply.PTNT_BIRTH = dt.Rows[i][23].ToString();
                    inPatientApply.PTNT_BED_NO = dt.Rows[i][24].ToString();
                    inPatientApply.DIAG_INFO = dt.Rows[i][25].ToString();
                    inPatientApply.APLY_ITM_KEY = dt.Rows[i][26].ToString();
                    inPatientApply.APLY_ITM_NAME = dt.Rows[i][27].ToString();
                    inPatientApply.SMPL_KEY = dt.Rows[i][28].ToString();
                    inPatientApply.SMPL_NAME = dt.Rows[i][29].ToString();
                    inPatientApply.BODY_PART = dt.Rows[i][30].ToString();
                    inPatientApply.REMARK = dt.Rows[i][31].ToString();
                    inPatientApply.EXEC_STATUS = dt.Rows[i][32].ToString();
                    inPatientApply.EMPI = dt.Rows[i][33].ToString();

                    al.Add(inPatientApply);
                }
                return al;
                #endregion
            }
            catch
            {
                return null;
            }
        }
        #endregion
        private string GetInPatientApplyXML(System.Collections.ArrayList al)
        {

            #region
            if (al.Count == 0)
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

                System.Xml.XmlElement LISApply = xml.CreateElement("LISApply");
                root1.AppendChild(LISApply);

                System.Xml.XmlElement PATIENTINFO = xml.CreateElement("PATIENTINFO");
                LISApply.AppendChild(PATIENTINFO);

                His.Models.LIS.InPatientApply p = al[0] as His.Models.LIS.InPatientApply;

                System.Xml.XmlElement PTNT_ID = xml.CreateElement("PTNT_ID");
                PTNT_ID.InnerText = p.PTNT_ID;
                PATIENTINFO.AppendChild(PTNT_ID);

                System.Xml.XmlElement VISIT_ID = xml.CreateElement("VISIT_ID");
                VISIT_ID.InnerText = p.VISIT_ID;
                PATIENTINFO.AppendChild(VISIT_ID);

                System.Xml.XmlElement EMPI = xml.CreateElement("EMPI");
                EMPI.InnerText = p.EMPI;
                PATIENTINFO.AppendChild(EMPI);

                System.Xml.XmlElement ID_CARD = xml.CreateElement("ID_CARD");
                ID_CARD.InnerText = p.ID_CARD;
                PATIENTINFO.AppendChild(ID_CARD);

                System.Xml.XmlElement IC_CARD = xml.CreateElement("IC_CARD");
                IC_CARD.InnerText = p.IC_CARD;
                PATIENTINFO.AppendChild(IC_CARD);

                System.Xml.XmlElement CTAT_ADDR = xml.CreateElement("CTAT_ADDR");
                CTAT_ADDR.InnerText = p.CTAT_ADDR;
                PATIENTINFO.AppendChild(CTAT_ADDR);

                System.Xml.XmlElement PHONE_NUM = xml.CreateElement("PHONE_NUM");
                PHONE_NUM.InnerText = p.PHONE_NUM;
                PATIENTINFO.AppendChild(PHONE_NUM);

                System.Xml.XmlElement PTNT_NO = xml.CreateElement("PTNT_NO");
                PTNT_NO.InnerText = p.PTNT_NO;

                System.Xml.XmlElement PTNT_NO_TYPE = xml.CreateElement("PTNT_NO_TYPE");
                PTNT_NO_TYPE.InnerText = p.PTNT_NO_TYPE;
                PATIENTINFO.AppendChild(PTNT_NO_TYPE);

                System.Xml.XmlElement PTNT_NAME = xml.CreateElement("PTNT_NAME");
                PTNT_NAME.InnerText = p.PTNT_NAME;
                PATIENTINFO.AppendChild(PTNT_NAME);

                System.Xml.XmlElement PTNT_SEX = xml.CreateElement("PTNT_SEX");
                PTNT_SEX.InnerText = p.PTNT_SEX;
                PATIENTINFO.AppendChild(PTNT_SEX);

                System.Xml.XmlElement PTNT_AGE = xml.CreateElement("PTNT_AGE");
                PTNT_AGE.InnerText = p.PTNT_AGE;
                PATIENTINFO.AppendChild(PTNT_AGE);

                System.Xml.XmlElement PTNT_AGE_UNIT = xml.CreateElement("PTNT_AGE_UNIT");
                PTNT_AGE_UNIT.InnerText = p.PTNT_AGE_UNIT;
                PATIENTINFO.AppendChild(PTNT_AGE_UNIT);

                System.Xml.XmlElement ADMISSE_DATE = xml.CreateElement("ADMISSE_DATE");
                ADMISSE_DATE.InnerText = p.ADMISSE_DATE;
                PATIENTINFO.AppendChild(ADMISSE_DATE);

                System.Xml.XmlElement PTNT_BIRTH = xml.CreateElement("PTNT_BIRTH");
                PTNT_BIRTH.InnerText = p.PTNT_BIRTH;
                PATIENTINFO.AppendChild(PTNT_BIRTH);

                System.Xml.XmlElement PTNT_BED_NO = xml.CreateElement("PTNT_BED_NO");
                PTNT_BED_NO.InnerText = p.PTNT_BED_NO;
                PATIENTINFO.AppendChild(PTNT_BED_NO);

                System.Xml.XmlElement DIAG_INFO = xml.CreateElement("DIAG_INFO");
                DIAG_INFO.InnerText = p.DIAG_INFO;
                PATIENTINFO.AppendChild(DIAG_INFO);

                foreach (His.Models.LIS.InPatientApply ipa in al)
                {
                    System.Xml.XmlElement APPLYINFO = xml.CreateElement("APPLYINFO");
                    LISApply.AppendChild(APPLYINFO);

                    System.Xml.XmlElement APLY_ID = xml.CreateElement("APLY_ID");
                    APLY_ID.InnerText = ipa.APLY_ID;
                    APPLYINFO.AppendChild(APLY_ID);

                    System.Xml.XmlElement APLY_SRC = xml.CreateElement("APLY_SRC");
                    APLY_SRC.InnerText = ipa.APLY_SRC;
                    APPLYINFO.AppendChild(APLY_SRC);

                    System.Xml.XmlElement APLY_FLOW_NUM = xml.CreateElement("APLY_FLOW_NUM");
                    APLY_FLOW_NUM.InnerText = ipa.APLY_FLOW_NUM;
                    APPLYINFO.AppendChild(APLY_FLOW_NUM);

                    System.Xml.XmlElement APLY_CREATE_DATE = xml.CreateElement("APLY_CREATE_DATE");
                    APLY_CREATE_DATE.InnerText = ipa.APLY_CREATE_DATE;
                    APPLYINFO.AppendChild(APLY_CREATE_DATE);

                    System.Xml.XmlElement EMCY_MRK = xml.CreateElement("EMCY_MRK");
                    EMCY_MRK.InnerText = ipa.EMCY_MRK;
                    APPLYINFO.AppendChild(EMCY_MRK);

                    System.Xml.XmlElement APLY_DATE = xml.CreateElement("APLY_DATE");
                    APLY_DATE.InnerText = ipa.APLY_DATE;
                    APPLYINFO.AppendChild(APLY_DATE);

                    System.Xml.XmlElement DEPT_KEY = xml.CreateElement("DEPT_KEY");
                    DEPT_KEY.InnerText = ipa.DEPT_KEY;
                    APPLYINFO.AppendChild(DEPT_KEY);

                    System.Xml.XmlElement DEPT_NAME = xml.CreateElement("DEPT_NAME");
                    DEPT_NAME.InnerText = ipa.DEPT_NAME;
                    APPLYINFO.AppendChild(DEPT_NAME);

                    System.Xml.XmlElement DOC_KEY = xml.CreateElement("DOC_KEY");
                    DOC_KEY.InnerText = ipa.DOC_KEY;
                    APPLYINFO.AppendChild(DOC_KEY);

                    System.Xml.XmlElement DOC_NAME = xml.CreateElement("DOC_NAME");
                    DOC_NAME.InnerText = ipa.DOC_NAME;
                    APPLYINFO.AppendChild(DOC_NAME);

                    System.Xml.XmlElement APLY_ITM_KEY = xml.CreateElement("APLY_ITM_KEY");
                    APLY_ITM_KEY.InnerText = ipa.APLY_ITM_KEY;
                    APPLYINFO.AppendChild(APLY_ITM_KEY);

                    System.Xml.XmlElement APLY_ITM_NAME = xml.CreateElement("APLY_ITM_NAME");
                    APLY_ITM_NAME.InnerText = ipa.APLY_ITM_NAME;
                    APPLYINFO.AppendChild(APLY_ITM_NAME);

                    System.Xml.XmlElement SMPL_KEY = xml.CreateElement("SMPL_KEY");
                    SMPL_KEY.InnerText = ipa.SMPL_KEY;
                    APPLYINFO.AppendChild(SMPL_KEY);

                    System.Xml.XmlElement SMPL_NAME = xml.CreateElement("SMPL_NAME");
                    SMPL_NAME.InnerText = ipa.SMPL_NAME;
                    APPLYINFO.AppendChild(SMPL_NAME);

                    System.Xml.XmlElement BODY_PART = xml.CreateElement("BODY_PART");
                    BODY_PART.InnerText = ipa.BODY_PART;
                    APPLYINFO.AppendChild(BODY_PART);

                    System.Xml.XmlElement REMARK = xml.CreateElement("REMARK");
                    REMARK.InnerText = ipa.REMARK;
                    APPLYINFO.AppendChild(REMARK);

                    System.Xml.XmlElement EXEC_STATUS = xml.CreateElement("EXEC_STATUS");
                    EXEC_STATUS.InnerText = ipa.EXEC_STATUS;
                    APPLYINFO.AppendChild(EXEC_STATUS);
                }

            #endregion
                return xml.InnerXml.ToString();
            }
        }

        private His.Models.LIS.InPatientApply GetInPatientModel2(string xml)
        {
            His.Models.LIS.InPatientApply ipa = new His.Models.LIS.InPatientApply();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception e)
            {
                return ipa;
            }

            System.Xml.XmlNodeList APLY_FLOW_NUM = doc.GetElementsByTagName("APLY_FLOW_NUM");
            System.Xml.XmlNode APLY_FLOW_NUM1 = APLY_FLOW_NUM[0];
            if (!string.IsNullOrEmpty(APLY_FLOW_NUM1.InnerText))
            {
                ipa.APLY_FLOW_NUM = APLY_FLOW_NUM1.InnerText;
            }
            else
            {
                ipa.APLY_FLOW_NUM = "ALL";
            }

            System.Xml.XmlNodeList BILL_NO_list = doc.GetElementsByTagName("BILL_NO");
            System.Xml.XmlNode BILL_NO = BILL_NO_list[0];
            if (!string.IsNullOrEmpty(BILL_NO.InnerText))
            {
                ipa.BILL_NO = BILL_NO.InnerText;
            }
            else
            {
                ipa.BILL_NO = "ALL";
            }

            System.Xml.XmlNodeList PTNT_ID = doc.GetElementsByTagName("PTNT_ID");
            System.Xml.XmlNode PTNT_ID1 = PTNT_ID[0];
            if (!string.IsNullOrEmpty(PTNT_ID1.InnerText))
            {
                ipa.PTNT_ID = PTNT_ID1.InnerText;
            }
            else
            {
                ipa.PTNT_ID = "ALL";
            }

            System.Xml.XmlNodeList IC_CARD_list = doc.GetElementsByTagName("IC_CARD");
            System.Xml.XmlNode IC_CARD = IC_CARD_list[0];
            if (!string.IsNullOrEmpty(IC_CARD.InnerText))
            {
                ipa.IC_CARD = IC_CARD.InnerText;
            }
            else
            {
                ipa.IC_CARD = "ALL";
            }

            System.Xml.XmlNodeList EMPI_list = doc.GetElementsByTagName("EMPI");
            System.Xml.XmlNode EMPI = EMPI_list[0];
            if (!string.IsNullOrEmpty(EMPI.InnerText))
            {
                ipa.EMPI = EMPI.InnerText;
            }
            else
            {
                ipa.EMPI = "ALL";
            }

            System.Xml.XmlNodeList LAB_TYPE_list = doc.GetElementsByTagName("LAB_TYPE");
            System.Xml.XmlNode LAB_TYPE = LAB_TYPE_list[0];
            if (!string.IsNullOrEmpty(LAB_TYPE.InnerText))
            {
                ipa.LAB_TYPE = LAB_TYPE.InnerText;
            }
            else
            {
                ipa.LAB_TYPE = "ALL";
            }

            return ipa;
        }

        public string GetInPatientApply(string xml)
        {
            string returnStr = "";
            His.Models.LIS.InPatientApply ipa = new His.Models.LIS.InPatientApply();
            ipa = this.GetInPatientModel2(xml);
            System.Collections.ArrayList al = this.GetInPatientApplyData(ipa);
            returnStr = this.GetInPatientApplyXML(al);
            return returnStr;
        }
    }
}