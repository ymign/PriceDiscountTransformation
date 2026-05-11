using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FS.ZDWY.Internet.BP.OutPatient
{
    public class PatientInfoManager
    {

        public DateTime GetSysTime()
        {
            BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();
            return patientInfoLogic.GetSysTime();
        }

        public string GetCardNo()
        {
            BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();
            return patientInfoLogic.GetPatientCardNO();
        }

        public int InsertPatientInfo(Models.COM_PATIENTINFO patientinfo, ref string errorMsg, ref Models.COM_PATIENTINFO patientReturn)
        {
            patientReturn = null;

            if (patientinfo == null)
            {
                errorMsg = "建档失败，患者信息传值不正确";
                return -1;
            }
            try
            {
                BL.OutPatient.PatientInfoLogic patientInfoLogic = new BL.OutPatient.PatientInfoLogic();
                if (patientInfoLogic.IsAny(w => w.IDENNO == patientinfo.IDENNO && !w.CARD_NO.StartsWith("9") && !w.CARD_NO.StartsWith("10") && !w.CARD_NO.StartsWith("0C")))
                {
                    string carno = "";
                    //根据姓名，身份证号查询
                    if (patientInfoLogic.IsAny(w => w.IDENNO == patientinfo.IDENNO && w.NAME == patientinfo.NAME && !w.CARD_NO.StartsWith("9") && !w.CARD_NO.StartsWith("10") && !w.CARD_NO.StartsWith("0C")))
                    {
                        carno = patientInfoLogic.GetList(w => w.IDENNO == patientinfo.IDENNO && w.NAME == patientinfo.NAME && !w.CARD_NO.StartsWith("9") && !w.CARD_NO.StartsWith("10") && !w.CARD_NO.StartsWith("0C")).First().CARD_NO;
                    }
                    patientReturn = patientInfoLogic.Get(carno);
                    if (patientReturn == null)
                    {
                        //根据身份证号查询
                        patientReturn = patientInfoLogic.GetPatientInfo(patientinfo.IDENNO);
                        if (patientReturn != null)
                        {
                            errorMsg = "建档失败，该患者身份证号已经存在,patientid为：" + patientReturn.CARD_NO;
                            return -1;
                        }
                        else
                        {
                            errorMsg = "建档失败，该患者身份证号已经存在";
                            return -1;
                        }
                    }
                    else
                    {
                        errorMsg = "建档失败，该患者身份证号已经存在,patientid为：" + patientReturn.CARD_NO;
                        return -1;
                    }
                }
                Models.FIN_OPB_ACCOUNTCARD card = new Models.FIN_OPB_ACCOUNTCARD();
                card.CARD_NO = patientinfo.CARD_NO;
                card.MARKNO = patientinfo.CARD_NO;
                card.TYPE = "Card_No";
                card.STATE = "1";
                card.REFLAG = "0";
                card.CREATEOPER = patientinfo.OPER_CODE;
                card.CREATEDATE = patientInfoLogic.GetDateTime();

                BL.OutPatient.AccountCard accountLogic = new BL.OutPatient.AccountCard();
                var res = accountLogic.Insert(card);
                if (!res)
                {
                    errorMsg = "建档失败，保存信息到账号表失败。";
                    return -1;
                }
                res = patientInfoLogic.InsertReturnEntity(patientinfo);
                if (!res)
                {
                    errorMsg = "建档失败，保存信息到患者表失败。";
                    return -1;
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询挂号排队
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="medicalNo"></param>
        /// <param name="certifcateNo"></param>
        /// <returns></returns>
        public DataTable QueryRegWaiting(string patientId, string medicalNo, string certifcateNo)
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.QueryRegWaiting(patientId, medicalNo, certifcateNo);
        }

        /// <summary>
        /// 查询取药排队
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="medicalNo"></param>
        /// <param name="certifcateNo"></param>
        /// <returns></returns>
        public DataTable QueryPhaWaiting(string patientId, string medicalNo, string certifcateNo)
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.QueryPhaWaiting(patientId, medicalNo, certifcateNo);
        }

        /// <summary>
        /// 查询对账信息
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isPay"></param>
        /// <returns></returns>
        public DataTable QueryFinanceBill(DateTime beginDate, DateTime endDate, string isPay)
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.QueryFinanceBill(beginDate, endDate, isPay);
        }

        /// <summary>
        /// 更新缴费通知表
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public int UpdateOutPatientFeeMsg(string clinicCode, DateTime createTime, DateTime execTime, string returnValue)
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.UpdateOutPatientFeeMsg(clinicCode, createTime, execTime, returnValue);
        }

        /// <summary>
        /// 查询门诊缴费记录
        /// </summary>
        /// <returns></returns>
        public DataTable QueryOutpatientFeeMsgList()
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.QueryOutpatientFeeMsgList();
        }

        /// <summary>
        /// 查询门诊挂号记录
        /// </summary>
        /// <returns></returns>
        public DataTable QueryRegWaitingALL()
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.QueryRegWaitingALL();
        }

        /// <summary>
        /// 查询取药排队记录
        /// </summary>
        /// <returns></returns>
        public DataTable QueryPhaWaitingALL()
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.QueryPhaWaitingALL();
        }

        /// <summary>
        /// 查询取消挂号记录
        /// </summary>
        /// <returns></returns>
        public DataTable QueryQueryCancelRegList()
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.QueryQueryCancelRegList();
        }

        /// <summary>
        /// 查询挂号接诊列表
        /// </summary>
        /// <returns></returns>
        public DataTable QueryQueryRegAcceptList()
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.QueryQueryRegAcceptList();
        }

        public DataTable QueryInpatientNo(string name, string inpatientID,string idNo)
        {
            FS.ZDWY.Internet.BL.OutPatient.PatientInfoLogic logic = new BL.OutPatient.PatientInfoLogic();
            return logic.QueryInpatientNo(name, inpatientID, idNo);
        }

    }
}
