using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.EMPI
{
    /// <summary>
    /// EMPI相关操作 add by allan
    /// </summary>
    public class EMPIOperate
    {
        #region ======病人信息注册========
        /// <summary>
        /// 病人信息注册
        /// </summary>
        /// <param name="pInfo">病人信息</param>
        /// <returns></returns>
        public string EmpiReg(EMPI.PATIENTINFO pInfo)
        {
            EMPIWebReference.EMPI empiServer = new Nesoft.EMPI.EMPIWebReference.EMPI();
            string strSendXml = XMLHelper.XmlSerialize<EMPI.PATIENTINFO>(pInfo, "", "");
            string strBackXml = empiServer.regEmpi(strSendXml);
            EMPIbusiness.EMPIDBOperate op = new Nesoft.EMPI.EMPIbusiness.EMPIDBOperate();
            bool bOpe = op.Insert(strBackXml, pInfo.CARDINFOS[0].CARDNO,pInfo.CARDINFOS[0].CARDTYPE);
            if (bOpe)
            {
                return strBackXml;
            }
            return "";
        }
        #endregion

        #region ======通过卡号获得病人信息========
        /// <summary>
        /// 通过卡号获得病人信息
        /// </summary>
        /// <param name="card">卡信息</param>
        /// <returns></returns>
        public EMPI.PATIENTINFO GetPatientInfoByCard(EMPI.CARDINFO.CARD card)
        {
            EMPIWebReference.EMPI empiServer = new Nesoft.EMPI.EMPIWebReference.EMPI();
            EMPI.CARDINFO.REQUEST req = new Nesoft.EMPI.EMPI.CARDINFO.REQUEST();
            req.CARDNO = card.CARDNO;
            req.CARDTYPE = card.CARDTYPE;
            req.DOMAIN = card.DOMAIN;
            string strSendXml = XMLHelper.XmlSerialize<EMPI.CARDINFO.REQUEST>(req, "", "");
            string strBackXml = empiServer.getPatientInfoByLocalIndex(strSendXml);
            EMPI.PATIENTINFO pInfo = XMLHelper.XmlDeserialize<EMPI.PATIENTINFO>(strBackXml, "", "");
            return pInfo;
        }
        #endregion

        #region =======通过EMPI查询病人信息=========
        /// <summary>
        /// 通过EMPI查询病人信息
        /// </summary>
        /// <param name="empiId"></param>
        /// <returns></returns>
        public EMPI.PATIENTINFO GetPatientInfoByEmpiId(string empiId)
        {
            EMPIWebReference.EMPI empiServer = new Nesoft.EMPI.EMPIWebReference.EMPI();
            EMPI.Empi.REQUEST req = new Nesoft.EMPI.EMPI.Empi.REQUEST();
            req.EMPI = empiId;
            string strSendXml = XMLHelper.XmlSerialize<EMPI.Empi.REQUEST>(req, "", "");
            string strBackXml = empiServer.getPatientInfoByLocalIndex(strSendXml);
            EMPI.PATIENTINFO pInfo = XMLHelper.XmlDeserialize<EMPI.PATIENTINFO>(strBackXml, "", "");
            return pInfo;
        }
        #endregion

        #region ======同过一张卡号获得病人所有的关联卡号========
        /// <summary>
        /// 同过一张卡号获得病人所有的关联卡号
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public List<EMPI.CARDINFO.CARD> GetPatientCards(EMPI.CARDINFO.CARD card)
        {
            EMPIWebReference.EMPI empiServer = new Nesoft.EMPI.EMPIWebReference.EMPI();
            EMPI.CARDINFO.REQUEST req = new Nesoft.EMPI.EMPI.CARDINFO.REQUEST();
            req.CARDNO = card.CARDNO;
            req.CARDTYPE = card.CARDTYPE;
            req.DOMAIN = card.DOMAIN;
            string strSendXml = XMLHelper.XmlSerialize<EMPI.CARDINFO.REQUEST>(req, "", "");
            string strBackXml = empiServer.getIndexByLocalIndex(strSendXml);
            if (strBackXml != "")
            {
                EMPI.CARDINFO.RESPONSE res = XMLHelper.XmlDeserialize<EMPI.CARDINFO.RESPONSE>(strBackXml, "", "");
                return res.CARDINFOS;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region ==========病人信息修改推送===========
        /// <summary>
        /// 病人信息修改推送
        /// </summary>
        /// <param name="reqData"></param>
        /// <returns></returns>
        public string HISPushPatientInfo(PushPatientInfo.PATIENT reqData)
        {
            HisPushServer.HISPushPatientInfoService empiServer = new Nesoft.EMPI.HisPushServer.HISPushPatientInfoService();
            HisPushServer.HISPushPatientInfoRequest req = new Nesoft.EMPI.HisPushServer.HISPushPatientInfoRequest();
            req.message = new Nesoft.EMPI.HisPushServer.HISPushPatientInfoRequestMessage();
            req.message.NAME = reqData.NAME;
            req.message.IDNO = reqData.IDNO;
            req.message.SEX = reqData.SEX;
            req.message.BIRTHDAY = reqData.BIRTHDAY;
            req.message.CNY = reqData.CNY;
            req.message.CNYNAME = reqData.CNYNAME;
            req.message.ACT = reqData.ACT;
            req.message.ADDR = reqData.ADDR;
            req.message.ZPCODE = reqData.ZPCODE;
            req.message.ABOBLD = reqData.ABOBLD;
            req.message.RHBLD = reqData.RHBLD;
            req.message.NTN = reqData.NTN;
            req.message.BCP = reqData.BCP;
            req.message.CTOR = reqData.CTOR;
            req.message.CTORTEL = reqData.CTORTEL;
            //当为null值时，平台生成xml会忽略该字段 2017-2-15 chengym
            if (reqData.CTORLTN == null)
            {
                req.message.CTORLTN = string.Empty;
            }
            else
            {
                req.message.CTORLTN = reqData.CTORLTN;
            }
            req.message.HMTEL = reqData.HMTEL;
            req.message.MOBILE = reqData.MOBILE;
            req.message.EML = reqData.EML;
            req.message.CPY = reqData.CPY;
            req.message.CPYTEL = reqData.CPYTEL;
            req.message.MRG = reqData.MRG;
            req.message.PFSN = reqData.PFSN;
            req.message.MEMO = reqData.MEMO;
            req.message.CARDNO = reqData.CARDNO;
            req.message.CARDTYPE = reqData.CARDTYPE;
            req.message.PATIENTTYPE = reqData.PATIENTTYPE;
            req.message.EMPINO = reqData.EMPINO;
            req.message.OPERCODE = reqData.OPERCODE;
            req.message.OPERNAME = reqData.OPERNAME;
            req.message.DEVOTE = reqData.DEVOTE;
            req.message.NOTE1 = reqData.NOTE1;
            req.message.NOTE2 = reqData.NOTE2;
            req.message.NOTE3 = reqData.NOTE3;
            Neusoft.FrameWork.WinForms.Classes.HisLog.WriteLog("empi", Neusoft.FrameWork.WinForms.Classes.XmlUtil.Serializer(req.GetType(), req));
            HisPushServer.HISPushPatientInfoResponse res = empiServer.HISPushPatientInfo(req);
            return res.HISPushPatientInfoResult;
        }
        #endregion
    }
}
