using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LisService.LisBusiness
{
    public class LisService
    {
        private System.Collections.ArrayList GetInPatientApplyData(string inpatientNo)
        {
              #region sql
            string sql = @" 
                    select          o.mo_order aply_detl_id, --    　  PK
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
                        (select m.empi_no from empi_paitinetinfo m where m.card_no=im.card_no and rownum=1) empi
                      from met_ipm_order o, fin_ipr_inmaininfo im
                     where o.mo_stat in ('1','2')
                       and o.class_code = 'UL'
                       and o.type_name not like '%嘱托%'
                       and o.inpatient_no = im.inpatient_no
                       --and o.confirm_date>sysdate -1/24
                       and im.inpatient_no='{0}'
                       and mo_order  in (select mo_order from met_ipm_execundrug x where x.mo_order=o.mo_order and x.lab_barcode is null)
                        ";
            #endregion

            try
            {
                #region 数据赋值
                sql = string.Format(sql, inpatientNo);
                System.Data.DataTable dt = new System.Data.DataTable();
                //申请单
                dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                System.Collections.ArrayList al = new System.Collections.ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    LisModel.LisModel ls = new LisModel.LisModel();
                    ls.APLY_ID = dt.Rows[i][0].ToString();
                    ls.APLY_SRC = dt.Rows[i][1].ToString();
                    ls.APLY_FLOW_NUM = dt.Rows[i][2].ToString();
                    ls.APLY_CREATE_DATE = dt.Rows[i][3].ToString();
                    ls.EMCY_MRK = dt.Rows[i][4].ToString();
                    ls.APLY_DATE = dt.Rows[i][5].ToString();
                    ls.DEPT_KEY = dt.Rows[i][6].ToString();
                    ls.DEPT_NAME = dt.Rows[i][7].ToString();
                    ls.DOC_KEY = dt.Rows[i][8].ToString();
                    ls.DOC_NAME = dt.Rows[i][9].ToString();
                    ls.PTNT_ID = dt.Rows[i][10].ToString();
                    ls.VISIT_ID = dt.Rows[i][11].ToString();
                    ls.ID_CARD = dt.Rows[i][12].ToString();
                    ls.IC_CARD = dt.Rows[i][13].ToString();
                    ls.CTAT_ADDR = dt.Rows[i][14].ToString();
                    ls.PHONE_NUM = dt.Rows[i][15].ToString();
                    ls.PTNT_NO = dt.Rows[i][16].ToString();
                    ls.PTNT_NO_TYPE = dt.Rows[i][17].ToString();
                    ls.PTNT_NAME = dt.Rows[i][18].ToString();
                    ls.PTNT_SEX = dt.Rows[i][19].ToString();
                    ls.PTNT_AGE = dt.Rows[i][20].ToString();
                    ls.PTNT_AGE_UNIT = dt.Rows[i][21].ToString();
                    ls.ADMISSE_DATE = dt.Rows[i][22].ToString();
                    ls.PTNT_BIRTH = dt.Rows[i][23].ToString();
                    ls.PTNT_BED_NO = dt.Rows[i][24].ToString();
                    ls.DIAG_INFO = dt.Rows[i][25].ToString();
                    ls.APLY_ITM_KEY = dt.Rows[i][26].ToString();
                    ls.APLY_ITM_NAME = dt.Rows[i][27].ToString();
                    ls.SMPL_KEY = dt.Rows[i][28].ToString();
                    ls.SMPL_NAME = dt.Rows[i][29].ToString();
                    ls.BODY_PART = dt.Rows[i][30].ToString();
                    ls.REMARK = dt.Rows[i][31].ToString();
                    ls.EXEC_STATUS = dt.Rows[i][32].ToString();
                    ls.EMPI = dt.Rows[i][33].ToString();

                    al.Add(ls);
                }
                #endregion
                return al;
            }
            catch { return null; }
        }

        private LisServiceReference1.PushInpLabApplyRequestMessageLISApplyPATIENTINFO SetPatientInfoValue(LisModel.LisModel lm)
        {
            LisServiceReference1.PushInpLabApplyRequestMessageLISApplyPATIENTINFO patientInfo = new global::LisService.LisServiceReference1.PushInpLabApplyRequestMessageLISApplyPATIENTINFO();
            patientInfo.PTNT_ID = lm.PTNT_ID;
            patientInfo.VISIT_ID = lm.VISIT_ID;
            patientInfo.EMPI = lm.EMPI;
            patientInfo.ID_CARD = lm.ID_CARD;
            patientInfo.IC_CARD = lm.IC_CARD;
            patientInfo.CTAT_ADDR = lm.CTAT_ADDR;
            patientInfo.PHONE_NUM = lm.PHONE_NUM;
            patientInfo.PTNT_NO = lm.PTNT_NO;
            patientInfo.PTNT_NO_TYPE = lm.PTNT_NO_TYPE;
            patientInfo.PTNT_NAME = lm.PTNT_NAME;
            patientInfo.PTNT_SEX = lm.PTNT_SEX;
            patientInfo.PTNT_AGE = lm.PTNT_AGE;
            patientInfo.PTNT_AGE_UNIT = lm.PTNT_AGE_UNIT;
            patientInfo.ADMISSE_DATE = lm.ADMISSE_DATE;
            patientInfo.PTNT_BIRTH = lm.PTNT_BIRTH;
            patientInfo.PTNT_BED_NO = lm.PTNT_BED_NO;
            patientInfo.DIAG_INFO = lm.DIAG_INFO;

            return patientInfo;
        }

        private LisServiceReference1.PushInpLabApplyRequestMessageLISApplyAPPLYINFO SetApplyInfoValue(LisModel.LisModel lm)
        {
            LisServiceReference1.PushInpLabApplyRequestMessageLISApplyAPPLYINFO applyInfo = new global::LisService.LisServiceReference1.PushInpLabApplyRequestMessageLISApplyAPPLYINFO();
            applyInfo.APLY_ID = lm.APLY_ID;
            applyInfo.APLY_SRC = lm.APLY_SRC;
            applyInfo.APLY_FLOW_NUM = lm.APLY_FLOW_NUM;
            applyInfo.APLY_CREATE_DATE = lm.APLY_CREATE_DATE;
            applyInfo.EMCY_MRK = lm.EMCY_MRK;
            applyInfo.APLY_DATE = lm.APLY_DATE;
            applyInfo.DEPT_KEY = lm.DEPT_KEY;
            applyInfo.DEPT_NAME = lm.DEPT_NAME;
            applyInfo.DOC_KEY = lm.DOC_KEY;
            applyInfo.DOC_NAME = lm.DOC_NAME;
            applyInfo.APLY_ITM_KEY = lm.APLY_ITM_KEY;
            applyInfo.APLY_ITM_NAME = lm.APLY_ITM_NAME;
            applyInfo.SMPL_KEY = lm.SMPL_KEY;
            applyInfo.SMPL_NAME = lm.SMPL_NAME;
            applyInfo.BODY_PART = lm.BODY_PART;
            applyInfo.REMARK = lm.REMARK;
            applyInfo.EXEC_STATUS = lm.EXEC_STATUS;

            return applyInfo;
        }

        private LisServiceReference1.PushInpLabApplyRequestMessageLISApply SetLisApply(System.Collections.ArrayList al)
        {
            LisServiceReference1.PushInpLabApplyRequestMessageLISApply lisApply = new global::LisService.LisServiceReference1.PushInpLabApplyRequestMessageLISApply();
            LisServiceReference1.PushInpLabApplyRequestMessageLISApplyAPPLYINFO[] applyInfoList = new global::LisService.LisServiceReference1.PushInpLabApplyRequestMessageLISApplyAPPLYINFO[al.Count];
            LisModel.LisModel lm = al[0] as LisModel.LisModel;
            lisApply.PATIENTINFO = this.SetPatientInfoValue(lm);
            int i=0;
            foreach (LisModel.LisModel lm1 in al)
            {
                //LisServiceReference1.PushInpLabApplyRequestMessageLISApplyAPPLYINFO applyInfo = new global::LisService.LisServiceReference1.PushInpLabApplyRequestMessageLISApplyAPPLYINFO();
                //applyInfo= this.SetApplyInfoValue(lm1);
                //applyInfoList[i]=applyInfo;
                applyInfoList[i] = this.SetApplyInfoValue(lm1);
                i = i + 1;
            }
            lisApply.APPLYINFO = applyInfoList;
            return lisApply;
        }

        private LisServiceReference1.PushInpLabApplyRequestMessage SetMessageValue(System.Collections.ArrayList al)
        {
            LisServiceReference1.PushInpLabApplyRequestMessage message = new global::LisService.LisServiceReference1.PushInpLabApplyRequestMessage();
            message.LISApply = this.SetLisApply(al);

            return message;
        }

        private int LisServiceSesult(LisServiceReference1.PushInpLabApplyRequestMessage message)
        {
            try
            {
                LisServiceReference1.PushInpLabApplyService client = new global::LisService.LisServiceReference1.PushInpLabApplyService();
                LisServiceReference1.PushInpLabApplyRequest pInpatientRequset = new global::LisService.LisServiceReference1.PushInpLabApplyRequest();
                pInpatientRequset.message = message;

                //LisServiceReference1.RequestHeader rh = new global::LisService.LisServiceReference1.RequestHeader();
                //rh.Token = "PushInpLabApply";
                //rh.SystemCode = "986dc7c4521ca062";
                //rh.ChannelMark = "PushInpLabApply";
                //rh.ConverterMark = "";

                LisServiceReference1.PushInpLabApplyResponse pr = new global::LisService.LisServiceReference1.PushInpLabApplyResponse();
                client.RequestHeaderValue = new global::LisService.LisServiceReference1.RequestHeader();
                client.RequestHeaderValue.Token = "PushInpLabApply";
                client.RequestHeaderValue.SystemCode = "986dc7c4521ca062";
                client.RequestHeaderValue.ChannelMark = "PushInpLabApply";
                client.RequestHeaderValue.ConverterMark = "";

                pr = client.PushInpLabApply(pInpatientRequset);
                His.Util.Common.HisLog.WriteLog("LisPush", client.Url);
                His.Util.Common.HisLog.WriteLog("LisPush", pr.@return);
                this.GetResult(pr.@return);
               
                return 1;
            }
            catch (Exception ex)
            {
                His.Util.Common.HisLog.WriteLog("LisPush", ex.Message);
                return 0;
            }
       
        }

        public void PushInPatientApply(string str)
        {
            System.Collections.ArrayList al = this.GetInPatientApplyData(str);
            LisServiceReference1.PushInpLabApplyRequestMessage message = this.SetMessageValue(al);
            His.Util.Common.HisLog.WriteLog("LisPush", His.Util.Common.XmlUtil.Serializer(message.GetType(),message));
            try
            {
                this.LisServiceSesult(message);
            }
            catch (Exception ex)
            {
                His.Util.Common.HisLog.WriteLog("LisPush", ex.Message);
                
            }
            
        }

        private int GetResult(string resurltXML)
        {
            string resultStr = "";
            int k = -1;
            try
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(resurltXML);
                System.Xml.XmlNodeList PTNT_ID = xmlDoc.GetElementsByTagName("PTNT_ID");
                System.Xml.XmlNodeList Code = xmlDoc.GetElementsByTagName("Code");
                resultStr = PTNT_ID[0].InnerText + "|" + Code[0].InnerText;
                System.Xml.XmlNodeList APLY_ITM_KEY = xmlDoc.GetElementsByTagName("APLY_ITM_KEY");
                System.Xml.XmlNodeList APLY_ITM_NAME = xmlDoc.GetElementsByTagName("APLY_ITM_NAME");
                System.Xml.XmlNodeList APLY_ID = xmlDoc.GetElementsByTagName("APLY_ID");
                for (int i = 0; i < APLY_ITM_KEY.Count; i++)
                {
                    resultStr += '|' + APLY_ITM_KEY[i].InnerText + APLY_ITM_NAME[i].InnerText;
                    this.updateItemResult(PTNT_ID[0].InnerText, APLY_ID[i].InnerText);
                }
                k= this.insertPushResult(PTNT_ID[0].InnerText, resultStr);
            }
            catch { }
            return k;
        }

        private int updateItemResult(string inpatientNo, string APLY_ID)
        {
            try
            {
                string sql = @"update met_ipm_execundrug il
                                set il.lab_barcode='0'
                                where il.inpatient_no='{0}' and il.mo_order='{1}' and  il.lab_barcode is null";
                sql = string.Format(sql, inpatientNo, APLY_ID);
                string err = "";
                His.Util.Common.HisLog.WriteLog("LisPush", sql);
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql, ref err))
                {
                    return -1;
                }
            }
            catch { }
            return 1;
        }
        private int insertPushResult(string inpatientNo, string result)
        {
            try
            {
                string sql2 = @"insert into prc_com_log
                                     values('{0}',
                                     '{1}',
                                    '{2}',
                                     sysdate)";
                sql2 = string.Format(sql2, inpatientNo, "住院数据推送返回结果", result);
                string err = "";
                if (!DataBaseHelp.DataExecHelp.ExecSql(sql2, ref err))
                {
                    return -1;
                }
                
            }
            catch { }
            return 1;
        }
    }
}
    