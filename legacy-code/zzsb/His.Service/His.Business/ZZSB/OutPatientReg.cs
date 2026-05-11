using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.FrameWork.Function;
using System.Data;
using His.Business.OutpatientWebService;
using His.Models.ZZSB.MedicalModel;
using His.Models.ZZSB;
using GDSI.CountryMedical.Common;

namespace His.Business.ZZSB
{
    public class OutPatientReg
    {
        /// <summary>
        /// 结果代码
        /// </summary>
        private string resultCode = string.Empty;
        /// <summary>
        /// 处理信息
        /// </summary>
        private string msg = string.Empty;

        public static string OPERID = RegisterManager.OPERID;

        private string ReturnFailure()
        {
            //事务回滚
            Shadow.Util.Data.Management.Trans.RollBack();

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

            //System.Xml.XmlElement OpTime = xml.CreateElement("OpTime");
            //OpTime.InnerText = this.msg;
            //root1.AppendChild(OpTime);

            //System.Xml.XmlElement FunCode = xml.CreateElement("FunCode");
            //FunCode.InnerText = this.msg;
            //root1.AppendChild(FunCode);

            System.Xml.XmlElement Result = xml.CreateElement("Result");
            root1.AppendChild(Result);

            return xml.InnerXml.ToString();
        }

        public string LockRegisterForSRM(string xml)
        {
            string returnStr = string.Empty;
            His.Models.ZZSB.OutPatientReg opr = new His.Models.ZZSB.OutPatientReg();
            returnStr = this.GetOutPatientLockModel(xml, opr);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            returnStr = this.LockRegSchema(opr);
            return returnStr;
        }

        private string GetOutPatientLockModel(string xml, His.Models.ZZSB.OutPatientReg opr)
        {
            string returnStr = string.Empty;
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

            System.Xml.XmlNodeList userIDList = doc.GetElementsByTagName("UserID");
            System.Xml.XmlNode userID = userIDList[0];
            if (!string.IsNullOrEmpty(userID.InnerText))
            {
                opr.UserID = userID.InnerText;
            }
            else
            {
                opr.UserID = string.Empty;
            }

            System.Xml.XmlNodeList deviceIDList = doc.GetElementsByTagName("DeviceID");
            System.Xml.XmlNode deviceID = deviceIDList[0];
            if (!string.IsNullOrEmpty(deviceID.InnerText))
            {
                opr.DeviceID = deviceID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "设备编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList serviceCodeList = doc.GetElementsByTagName("ServiceCode");
            System.Xml.XmlNode serviceCode = serviceCodeList[0];
            if (!string.IsNullOrEmpty(serviceCode.InnerText))
            {
                opr.ServiceCode = serviceCode.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "服务编码不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList funCodeList = doc.GetElementsByTagName("FunCode");
            System.Xml.XmlNode funCode = funCodeList[0];
            if (!string.IsNullOrEmpty(funCode.InnerText))
            {
                opr.FunCode = funCode.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "业务编号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList reqTimeList = doc.GetElementsByTagName("ReqTime");
            System.Xml.XmlNode reqTime = reqTimeList[0];
            if (!string.IsNullOrEmpty(reqTime.InnerText))
            {
                opr.ReqTime = reqTime.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求时间不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList reqTraceNoList = doc.GetElementsByTagName("ReqTraceNo");
            System.Xml.XmlNode reqTraceNo = reqTraceNoList[0];
            if (!string.IsNullOrEmpty(reqTraceNo.InnerText))
            {
                opr.ReqTraceNo = reqTraceNo.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "请求流水号不能为空！";
                return this.ReturnFailure();
            }

            System.Xml.XmlNodeList cardNoList = doc.GetElementsByTagName("CardNo");
            System.Xml.XmlNode cardNo = cardNoList[0];
            if (!string.IsNullOrEmpty(cardNo.InnerText))
            {
                opr.CardNo = cardNo.InnerText;
            }
            else
            {
                opr.CardNo = string.Empty;
            }

            System.Xml.XmlNodeList deptCodeList = doc.GetElementsByTagName("DeptCode");
            System.Xml.XmlNode deptCode = deptCodeList[0];
            if (!string.IsNullOrEmpty(deptCode.InnerText))
            {
                opr.DeptCode = deptCode.InnerText;
            }
            else
            {
                opr.DeptCode = string.Empty;
            }

            System.Xml.XmlNodeList sessionCodeList = doc.GetElementsByTagName("SessionCode");
            System.Xml.XmlNode sessionCode = sessionCodeList[0];
            if (!string.IsNullOrEmpty(sessionCode.InnerText))
            {
                opr.SessionCode = sessionCode.InnerText;
            }
            else
            {
                opr.SessionCode = string.Empty;
            }

            System.Xml.XmlNodeList doctorCodeList = doc.GetElementsByTagName("DoctorCode");
            System.Xml.XmlNode doctorCode = doctorCodeList[0];
            if (!string.IsNullOrEmpty(doctorCode.InnerText))
            {
                opr.DoctorCode = doctorCode.InnerText;
            }
            else
            {
                opr.DoctorCode = string.Empty;
            }

            System.Xml.XmlNodeList regSourceIDList = doc.GetElementsByTagName("RegSourceID");
            System.Xml.XmlNode regSourceID = regSourceIDList[0];
            if (!string.IsNullOrEmpty(regSourceID.InnerText))
            {
                opr.RegSourceID = regSourceID.InnerText;
            }
            else
            {
                this.resultCode = "0";
                this.msg = "排班编号不能为空！";
                return this.ReturnFailure();
            }

            try
            {
                System.Xml.XmlNodeList tranSerNoIDList = doc.GetElementsByTagName("TranSerNo");
                System.Xml.XmlNode tranSerNoID = tranSerNoIDList[0];
                if (!string.IsNullOrEmpty(tranSerNoID.InnerText))
                {
                    opr.TranSerNo = tranSerNoID.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "锁号流水号不能为空！";
                    return this.ReturnFailure();
                }
            }
            catch
            { }

            return returnStr;
        }

        private string GetOutPatientRegModel(string xml, His.Models.ZZSB.OutPatientReg opr)
        {
            string returnStr = string.Empty;
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

            try
            {
                #region 解析入参XML
                System.Xml.XmlNodeList userIDList = doc.GetElementsByTagName("UserID");
                System.Xml.XmlNode userID = userIDList[0];
                if (!string.IsNullOrEmpty(userID.InnerText))
                {
                    opr.UserID = userID.InnerText;
                }
                else
                {
                    opr.UserID = string.Empty;
                }

                System.Xml.XmlNodeList deviceIDList = doc.GetElementsByTagName("DeviceID");
                System.Xml.XmlNode deviceID = deviceIDList[0];
                if (!string.IsNullOrEmpty(deviceID.InnerText))
                {
                    opr.DeviceID = deviceID.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "设备编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList serviceCodeList = doc.GetElementsByTagName("ServiceCode");
                System.Xml.XmlNode serviceCode = serviceCodeList[0];
                if (!string.IsNullOrEmpty(serviceCode.InnerText))
                {
                    opr.ServiceCode = serviceCode.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "服务编码不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList funCodeList = doc.GetElementsByTagName("FunCode");
                System.Xml.XmlNode funCode = funCodeList[0];
                if (!string.IsNullOrEmpty(funCode.InnerText))
                {
                    opr.FunCode = funCode.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "业务编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList reqTimeList = doc.GetElementsByTagName("ReqTime");
                System.Xml.XmlNode reqTime = reqTimeList[0];
                if (!string.IsNullOrEmpty(reqTime.InnerText))
                {
                    opr.ReqTime = reqTime.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "请求时间不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList reqTraceNoList = doc.GetElementsByTagName("ReqTraceNo");
                System.Xml.XmlNode reqTraceNo = reqTraceNoList[0];
                if (!string.IsNullOrEmpty(reqTraceNo.InnerText))
                {
                    opr.ReqTraceNo = reqTraceNo.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "请求流水号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList cardNoList = doc.GetElementsByTagName("CardNo");
                System.Xml.XmlNode cardNo = cardNoList[0];
                if (!string.IsNullOrEmpty(cardNo.InnerText))
                {
                    opr.CardNo = cardNo.InnerText;
                }
                else
                {
                    opr.CardNo = string.Empty;
                }

                System.Xml.XmlNodeList deptCodeList = doc.GetElementsByTagName("DeptCode");
                System.Xml.XmlNode deptCode = deptCodeList[0];
                if (!string.IsNullOrEmpty(deptCode.InnerText))
                {
                    opr.DeptCode = deptCode.InnerText;
                }
                else
                {
                    opr.DeptCode = string.Empty;
                }

                System.Xml.XmlNodeList sessionCodeList = doc.GetElementsByTagName("SessionCode");
                System.Xml.XmlNode sessionCode = sessionCodeList[0];
                if (!string.IsNullOrEmpty(sessionCode.InnerText))
                {
                    opr.SessionCode = sessionCode.InnerText;
                }
                else
                {
                    opr.SessionCode = string.Empty;
                }

                System.Xml.XmlNodeList doctorCodeList = doc.GetElementsByTagName("DoctorCode");
                System.Xml.XmlNode doctorCode = doctorCodeList[0];
                if (!string.IsNullOrEmpty(doctorCode.InnerText))
                {
                    opr.DoctorCode = doctorCode.InnerText;
                }
                else
                {
                    opr.DoctorCode = string.Empty;
                }

                System.Xml.XmlNodeList regSourceIDList = doc.GetElementsByTagName("RegSourceID");
                System.Xml.XmlNode regSourceID = regSourceIDList[0];
                if (!string.IsNullOrEmpty(regSourceID.InnerText))
                {
                    opr.RegSourceID = regSourceID.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "排班编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList tranSerNoList = doc.GetElementsByTagName("TranSerNo");
                System.Xml.XmlNode tranSerNo = tranSerNoList[0];
                if (!string.IsNullOrEmpty(tranSerNo.InnerText))
                {
                    opr.TranSerNo = tranSerNo.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "锁号流水号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList totalRegFeeList = doc.GetElementsByTagName("TotalRegFee");
                System.Xml.XmlNode totalRegFee = totalRegFeeList[0];
                if (!string.IsNullOrEmpty(totalRegFee.InnerText))
                {
                    opr.TotalRegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(totalRegFee.InnerText);
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "总挂号费不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList payTypeList = doc.GetElementsByTagName("PayType");
                System.Xml.XmlNode payType = payTypeList[0];
                if (!string.IsNullOrEmpty(payType.InnerText))
                {
                    opr.PayType = payType.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "支付方式不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList ClincCodeList = doc.GetElementsByTagName("ClincCode");
                System.Xml.XmlNode ClincCode = ClincCodeList[0];
                if (!string.IsNullOrEmpty(ClincCode.InnerText))
                {
                    opr.ClincCode = ClincCode.InnerText;
                }
                //else
                //{
                //    this.resultCode = "0";
                //    this.msg = "门诊流水号不能为空！";
                //    return this.ReturnFailure();
                //}


                System.Xml.XmlNodeList posIDList = doc.GetElementsByTagName("PosID");
                System.Xml.XmlNode posID = posIDList[0];
                if (!string.IsNullOrEmpty(posID.InnerText))
                {
                    opr.PosID = posID.InnerText;
                }
                else
                {
                    opr.PosID = string.Empty;
                }

                System.Xml.XmlNodeList bankCardNoList = doc.GetElementsByTagName("BankCardNo");
                System.Xml.XmlNode bankCardNo = bankCardNoList[0];
                if (!string.IsNullOrEmpty(bankCardNo.InnerText))
                {
                    opr.BankCardNo = bankCardNo.InnerText;
                }
                else
                {
                    opr.BankCardNo = string.Empty;
                }

                System.Xml.XmlNodeList payDateList = doc.GetElementsByTagName("PayDate");
                System.Xml.XmlNode payDate = payDateList[0];
                if (!string.IsNullOrEmpty(payDate.InnerText))
                {
                    opr.PayDate = payDate.InnerText;
                }
                else
                {
                    opr.PayDate = string.Empty;
                }

                System.Xml.XmlNodeList payTimeList = doc.GetElementsByTagName("PayTime");
                System.Xml.XmlNode payTime = payTimeList[0];
                if (!string.IsNullOrEmpty(payTime.InnerText))
                {
                    opr.PayTime = payTime.InnerText;
                }
                else
                {
                    opr.PayTime = string.Empty;
                }

                System.Xml.XmlNodeList batchNoList = doc.GetElementsByTagName("BatchNo");
                System.Xml.XmlNode batchNo = batchNoList[0];
                if (!string.IsNullOrEmpty(batchNo.InnerText))
                {
                    opr.BatchNo = batchNo.InnerText;
                }
                else
                {
                    opr.BatchNo = string.Empty;
                }

                System.Xml.XmlNodeList vouchNoList = doc.GetElementsByTagName("VouchNo");
                System.Xml.XmlNode vouchNo = vouchNoList[0];
                if (!string.IsNullOrEmpty(vouchNo.InnerText))
                {
                    opr.VouchNo = vouchNo.InnerText;
                }
                else
                {
                    opr.VouchNo = string.Empty;
                }

                System.Xml.XmlNodeList referNoList = doc.GetElementsByTagName("ReferNo");
                System.Xml.XmlNode referNo = referNoList[0];
                if (!string.IsNullOrEmpty(referNo.InnerText))
                {
                    opr.ReferNo = referNo.InnerText;
                }
                else
                {
                    opr.ReferNo = string.Empty;
                }

                System.Xml.XmlNodeList payAmtList = doc.GetElementsByTagName("PayAmt");
                System.Xml.XmlNode payAmt = payAmtList[0];
                if (!string.IsNullOrEmpty(payAmt.InnerText))
                {
                    opr.PayAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(payAmt.InnerText);
                }
                else
                {
                    opr.PayAmt = 0m;
                }

                System.Xml.XmlNodeList bankCodeList = doc.GetElementsByTagName("BankCode");
                System.Xml.XmlNode bankCode = bankCodeList[0];
                if (!string.IsNullOrEmpty(bankCode.InnerText))
                {
                    opr.BankCode = bankCode.InnerText;
                }
                else
                {
                    opr.BankCode = string.Empty;
                }

                System.Xml.XmlNodeList medInsureTranNoList = doc.GetElementsByTagName("MedInsureTranNo");
                System.Xml.XmlNode medInsureTranNo = medInsureTranNoList[0];
                if (!string.IsNullOrEmpty(medInsureTranNo.InnerText))
                {
                    opr.MedInsureTranNo = medInsureTranNo.InnerText;
                }
                else
                {
                    opr.MedInsureTranNo = string.Empty;
                }

                System.Xml.XmlNodeList medInsureStrList = doc.GetElementsByTagName("MedInsureStr");
                System.Xml.XmlNode medInsureStr = medInsureStrList[0];
                if (!string.IsNullOrEmpty(medInsureStr.InnerText))
                {
                    opr.MedInsureStr = medInsureStr.InnerText;
                }
                else
                {
                    opr.MedInsureStr = string.Empty;
                }

                System.Xml.XmlNodeList medInsureFeeList = doc.GetElementsByTagName("MedInsureFee");
                System.Xml.XmlNode medInsureFee = medInsureFeeList[0];
                if (!string.IsNullOrEmpty(medInsureFee.InnerText))
                {
                    opr.MedInsureFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(medInsureFee.InnerText);
                }
                else
                {
                    opr.MedInsureFee = 0m;
                }

                System.Xml.XmlNodeList personalFeeList = doc.GetElementsByTagName("PersonalFee");
                System.Xml.XmlNode personalFee = personalFeeList[0];
                if (!string.IsNullOrEmpty(personalFee.InnerText))
                {
                    opr.PersonalFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(personalFee.InnerText);
                }
                else
                {
                    opr.PersonalFee = 0m;
                }


                System.Xml.XmlNodeList Payinsufeestrs = doc.GetElementsByTagName("Payinsufeestr");
                System.Xml.XmlNode Payinsufeestr = Payinsufeestrs[0];
                if (!string.IsNullOrEmpty(Payinsufeestr.InnerText))
                {
                    opr.Payinsufeestr = Payinsufeestr.InnerText;
                }
                else
                {
                    opr.Payinsufeestr = string.Empty;
                }

                try
                {
                    System.Xml.XmlNodeList InformedConsentResultXml = doc.GetElementsByTagName("InformedConsentResult");
                    System.Xml.XmlNode InformedConsentResultNode = InformedConsentResultXml[0];
                    if (!string.IsNullOrEmpty(InformedConsentResultNode.InnerText))
                    {
                        opr.InformedConsentResult = InformedConsentResultNode.InnerText;
                    }
                    else
                    {
                        opr.InformedConsentResult = string.Empty;
                    }
                }
                catch
                {

                    opr.InformedConsentResult = string.Empty;
                }

                try
                {
                    System.Xml.XmlNodeList Triage_SerialnumS = doc.GetElementsByTagName("Triage_Serialnum");
                    System.Xml.XmlNode Triage_Serialnum = Triage_SerialnumS[0];
                    if (!string.IsNullOrEmpty(Triage_Serialnum.InnerText))
                    {
                        opr.Triage_Serialnum = Triage_Serialnum.InnerText;
                    }
                    else
                    {
                        opr.Triage_Serialnum = string.Empty;
                    }
                }
                catch
                {

                    opr.Triage_Serialnum = string.Empty;
                }

                try
                {
                    System.Xml.XmlNodeList ApplicationOrderNoS = doc.GetElementsByTagName("ApplicationOrderNo");
                    System.Xml.XmlNode ApplicationOrderNo = ApplicationOrderNoS[0];
                    if (!string.IsNullOrEmpty(ApplicationOrderNo.InnerText))
                    {
                        opr.ApplicationOrderNo = ApplicationOrderNo.InnerText;
                    }
                    else
                    {
                        opr.ApplicationOrderNo = string.Empty;
                    }
                }
                catch
                {

                    opr.ApplicationOrderNo = string.Empty;
                }

                try
                {
                    System.Xml.XmlNodeList PlatformOrderNoS = doc.GetElementsByTagName("PlatformOrderNo");
                    System.Xml.XmlNode PlatformOrderNo = PlatformOrderNoS[0];
                    if (!string.IsNullOrEmpty(PlatformOrderNo.InnerText))
                    {
                        opr.PlatformOrderNo = PlatformOrderNo.InnerText;
                    }
                    else
                    {
                        opr.PlatformOrderNo = string.Empty;
                    }
                }
                catch
                {

                    opr.PlatformOrderNo = string.Empty;
                }

                try
                {
                    System.Xml.XmlNodeList FeeTypetrs = doc.GetElementsByTagName("FeeType");
                    System.Xml.XmlNode FeeTypestr = FeeTypetrs[0];
                    if (!string.IsNullOrEmpty(FeeTypestr.InnerText))
                    {
                        opr.FeeType = FeeTypestr.InnerText;
                    }
                    else
                    {
                        opr.FeeType = "1";
                    }
                }
                catch
                {
                    opr.FeeType = "1";
                }
                #endregion
            }
            catch (Exception e)
            {
                this.resultCode = "0";
                this.msg = "缺少必填参数！";
                return this.ReturnFailure();
            }

            return returnStr;
        }

        private string LockRegSchema(His.Models.ZZSB.OutPatientReg opr)
        {
            string returnStr = string.Empty;
            string sql = string.Empty;
            #region 判断是否有足够号源
            int regRemainCount = 0;


            sql = Sql.Sql.SelectSchemaRegRemain;
            sql = string.Format(sql, opr.RegSourceID);

            System.Data.DataTable dt = new System.Data.DataTable();
            //排班表
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        regRemainCount = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[i][1].ToString());
                        if (regRemainCount > 0)
                        {
                            resultCode = "1";
                            msg = "锁定号源成功";
                            break;
                        }
                        else
                        {
                            resultCode = "0";
                            msg = "号源名额不足";
                            return ReturnFailure();
                        }
                    }
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "没有找到相应号源！";
                    return this.ReturnFailure();
                }
            }
            else
            {
                this.resultCode = "0";
                this.msg = "没有找到相应号源！";
                return this.ReturnFailure();
            }
            #endregion

            #region 把排队号取出来存一下。不然操作时间过长，会有人插队，排队号不准。
            int schType = -1, seeNo = -1;
            //try
            //{
            //    if (Function.GetSchemaType(opr.RegSourceID,ref schType,ref msg) == 0)
            //    { }
            //    if (schType == 1)
            //    {
            //        if (Function.GetDocSeeNoBySchemaId(opr.RegSourceID,ref seeNo,ref msg)==0)
            //        {

            //        }
            //    }
            //    else if (schType == 0)
            //    {

            //    }
            //}
            //catch
            //{
            //    His.Util.Common.HisLog.WriteLog("ZZSB", 
            //        "把排队号取出来存一下。$$$$$" + msg + opr.CardNo + opr.RegSourceID);
            //}
            #endregion

            #region 更新排班表，插入号源表
            string updateSql = Sql.Sql.UpdateSchemaReged;
            string insertSql = Sql.Sql.InsertRegLock;
            ArrayList sqlList = new ArrayList();
            if (resultCode == "1")
            {
                // updateSql = string.Format(updateSql, opr.RegSourceID, "1");
                insertSql = string.Format(insertSql, opr.ReqTraceNo, opr.UserID, opr.DeviceID, opr.ServiceCode, opr.FunCode, opr.ReqTime, opr.CardNo, opr.DeptCode,
                                        opr.SessionCode, opr.DoctorCode, opr.RegSourceID, "0", OPERID, seeNo);
                sqlList.Add(insertSql);
                // sqlList.Add(updateSql);

                if (!DataBaseHelp.DataExecHelp.ExecArrayList(sqlList))
                {
                    foreach (string item in sqlList)
                    {
                        Shadow.Util.Data.Func.Log.WriteLog("zzsb", item);
                    }
                    resultCode = "0";
                    msg = "锁定号源失败！";
                    return ReturnFailure();
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

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);

                System.Xml.XmlElement TranSerNo = xml.CreateElement("TranSerNo");
                TranSerNo.InnerText = opr.ReqTraceNo;
                Result.AppendChild(TranSerNo);

                System.Xml.XmlElement Note = xml.CreateElement("Note");
                Result.AppendChild(Note);

                returnStr = xml.InnerXml.ToString();
            }

            #endregion

            return returnStr;
        }

        public string UnlockRegisterForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.OutPatientReg opr = new His.Models.ZZSB.OutPatientReg();
            returnStr = this.GetOutPatientLockModel(xml, opr);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            returnStr = this.UnlockRegSchema(opr);
            return returnStr;
        }



        /// <summary>
        /// 根据入参转换成所需实体数据
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public string GetReqInfoForXml(string xml, ref His.Models.ZZSB.InPatientReq opr)
        {
            string returnStr = string.Empty;
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

            try
            {
                #region 解析入参XML
                System.Xml.XmlNodeList userIDList = doc.GetElementsByTagName("UserID");//操作员编号
                System.Xml.XmlNode userID = userIDList[0];
                if (!string.IsNullOrEmpty(userID.InnerText))
                {
                    opr.UserID = userID.InnerText;
                }
                else
                {
                    opr.UserID = string.Empty;
                }

                System.Xml.XmlNodeList reqTimeList = doc.GetElementsByTagName("ReqTime");//请求时间
                System.Xml.XmlNode reqTime = reqTimeList[0];
                if (!string.IsNullOrEmpty(reqTime.InnerText))
                {
                    opr.ReqTime = reqTime.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "请求时间不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList passWordList = doc.GetElementsByTagName("PassWord");//操作员密码
                System.Xml.XmlNode passWord = passWordList[0];
                if (!string.IsNullOrEmpty(passWord.InnerText))
                {
                    opr.PassWord = passWord.InnerText;
                }
                else
                {
                    opr.PassWord = string.Empty;
                }

                System.Xml.XmlNodeList deviceIDList = doc.GetElementsByTagName("DeviceID");//设备编号
                System.Xml.XmlNode deviceID = deviceIDList[0];
                if (!string.IsNullOrEmpty(deviceID.InnerText))
                {
                    opr.DeviceID = deviceID.InnerText;
                }
                else
                {
                    opr.DeviceID = string.Empty;
                }

                System.Xml.XmlNodeList serviceCodeList = doc.GetElementsByTagName("ServiceCode");//服务编码
                System.Xml.XmlNode serviceCode = serviceCodeList[0];
                if (!string.IsNullOrEmpty(serviceCode.InnerText))
                {
                    opr.ServiceCode = serviceCode.InnerText;
                }
                else
                {
                    opr.ServiceCode = string.Empty;
                }

                System.Xml.XmlNodeList bankCodeList = doc.GetElementsByTagName("BankCode");
                System.Xml.XmlNode bankCode = bankCodeList[0];
                if (!string.IsNullOrEmpty(bankCode.InnerText))
                {
                    opr.BankCode = bankCode.InnerText;
                }
                else
                {
                    opr.BankCode = string.Empty;
                }

                System.Xml.XmlNodeList hospCodeList = doc.GetElementsByTagName("HospCode");
                System.Xml.XmlNode hospCode = hospCodeList[0];
                if (!string.IsNullOrEmpty(hospCode.InnerText))
                {
                    opr.HospCode = hospCode.InnerText;
                }
                else
                {
                    opr.HospCode = string.Empty;
                }

                System.Xml.XmlNodeList cardTypeCodeList = doc.GetElementsByTagName("CardTypeCode");
                System.Xml.XmlNode cardTypeCode = cardTypeCodeList[0];
                if (!string.IsNullOrEmpty(cardTypeCode.InnerText))
                {
                    opr.CardTypeCode = cardTypeCode.InnerText;
                }
                else
                {
                    opr.CardTypeCode = string.Empty;
                }

                System.Xml.XmlNodeList cardNoList = doc.GetElementsByTagName("CardNo");
                System.Xml.XmlNode cardNo = cardNoList[0];
                if (!string.IsNullOrEmpty(cardNo.InnerText))
                {
                    opr.CardNo = cardNo.InnerText;
                }
                else
                {
                    opr.CardNo = string.Empty;
                }

                System.Xml.XmlNodeList nameList = doc.GetElementsByTagName("Name");
                System.Xml.XmlNode name = nameList[0];
                if (!string.IsNullOrEmpty(name.InnerText))
                {
                    opr.Name = name.InnerText;
                }
                else
                {
                    opr.Name = string.Empty;
                }

                System.Xml.XmlNodeList patientIDList = doc.GetElementsByTagName("PatientID");
                System.Xml.XmlNode patientID = patientIDList[0];
                if (!string.IsNullOrEmpty(patientID.InnerText))
                {
                    opr.PatientID = patientID.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "住院号不能为空！";
                    return this.ReturnFailure();
                }


                #endregion
            }
            catch (Exception e)
            {
                this.resultCode = "0";
                this.msg = "缺少必填参数！";
                return this.ReturnFailure();
            }

            return returnStr;
        }


        #region 预约挂号锁号
        public string
            BookLockRegisterForSRM(string xml)
        {
            string returnStr = string.Empty;
            His.Models.ZZSB.OutPatientReg opr = new His.Models.ZZSB.OutPatientReg();
            returnStr = this.GetOutPatientLockModel(xml, opr);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            returnStr = new Booking().BookLock(opr);
            return returnStr;
        }
        public string GetOutPatientRegModelForXml(string xml, ref His.Models.ZZSB.OutPatientReg opr)
        {
            string returnStr = string.Empty;
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

            try
            {
                #region 解析入参XML
                System.Xml.XmlNodeList userIDList = doc.GetElementsByTagName("UserID");
                System.Xml.XmlNode userID = userIDList[0];
                if (!string.IsNullOrEmpty(userID.InnerText))
                {
                    opr.UserID = userID.InnerText;
                }
                else
                {
                    opr.UserID = string.Empty;
                }

                System.Xml.XmlNodeList deviceIDList = doc.GetElementsByTagName("DeviceID");
                System.Xml.XmlNode deviceID = deviceIDList[0];
                if (!string.IsNullOrEmpty(deviceID.InnerText))
                {
                    opr.DeviceID = deviceID.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "设备编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList serviceCodeList = doc.GetElementsByTagName("ServiceCode");
                System.Xml.XmlNode serviceCode = serviceCodeList[0];
                if (!string.IsNullOrEmpty(serviceCode.InnerText))
                {
                    opr.ServiceCode = serviceCode.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "服务编码不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList funCodeList = doc.GetElementsByTagName("FunCode");
                System.Xml.XmlNode funCode = funCodeList[0];
                if (!string.IsNullOrEmpty(funCode.InnerText))
                {
                    opr.FunCode = funCode.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "业务编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList reqTimeList = doc.GetElementsByTagName("ReqTime");
                System.Xml.XmlNode reqTime = reqTimeList[0];
                if (!string.IsNullOrEmpty(reqTime.InnerText))
                {
                    opr.ReqTime = reqTime.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "请求时间不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList reqTraceNoList = doc.GetElementsByTagName("ReqTraceNo");
                System.Xml.XmlNode reqTraceNo = reqTraceNoList[0];
                if (!string.IsNullOrEmpty(reqTraceNo.InnerText))
                {
                    opr.ReqTraceNo = reqTraceNo.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "请求流水号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList cardNoList = doc.GetElementsByTagName("CardNo");
                System.Xml.XmlNode cardNo = cardNoList[0];
                if (!string.IsNullOrEmpty(cardNo.InnerText))
                {
                    opr.CardNo = cardNo.InnerText;
                }
                else
                {
                    opr.CardNo = string.Empty;
                }

                System.Xml.XmlNodeList deptCodeList = doc.GetElementsByTagName("DeptCode");
                System.Xml.XmlNode deptCode = deptCodeList[0];
                if (!string.IsNullOrEmpty(deptCode.InnerText))
                {
                    opr.DeptCode = deptCode.InnerText;
                }
                else
                {
                    opr.DeptCode = string.Empty;
                }

                System.Xml.XmlNodeList sessionCodeList = doc.GetElementsByTagName("SessionCode");
                System.Xml.XmlNode sessionCode = sessionCodeList[0];
                if (!string.IsNullOrEmpty(sessionCode.InnerText))
                {
                    opr.SessionCode = sessionCode.InnerText;
                }
                else
                {
                    opr.SessionCode = string.Empty;
                }

                System.Xml.XmlNodeList doctorCodeList = doc.GetElementsByTagName("DoctorCode");
                System.Xml.XmlNode doctorCode = doctorCodeList[0];
                if (!string.IsNullOrEmpty(doctorCode.InnerText))
                {
                    opr.DoctorCode = doctorCode.InnerText;
                }
                else
                {
                    opr.DoctorCode = string.Empty;
                }

                System.Xml.XmlNodeList regSourceIDList = doc.GetElementsByTagName("RegSourceID");
                System.Xml.XmlNode regSourceID = regSourceIDList[0];
                if (!string.IsNullOrEmpty(regSourceID.InnerText))
                {
                    opr.RegSourceID = regSourceID.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "排班编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList tranSerNoList = doc.GetElementsByTagName("TranSerNo");
                System.Xml.XmlNode tranSerNo = tranSerNoList[0];
                if (!string.IsNullOrEmpty(tranSerNo.InnerText))
                {
                    opr.TranSerNo = tranSerNo.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "锁号流水号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList totalRegFeeList = doc.GetElementsByTagName("TotalRegFee");
                System.Xml.XmlNode totalRegFee = totalRegFeeList[0];
                if (!string.IsNullOrEmpty(totalRegFee.InnerText))
                {
                    opr.TotalRegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(totalRegFee.InnerText);
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "总挂号费不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList payTypeList = doc.GetElementsByTagName("PayType");
                System.Xml.XmlNode payType = payTypeList[0];
                if (!string.IsNullOrEmpty(payType.InnerText))
                {
                    opr.PayType = payType.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "支付方式不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList posIDList = doc.GetElementsByTagName("PosID");
                System.Xml.XmlNode posID = posIDList[0];
                if (!string.IsNullOrEmpty(posID.InnerText))
                {
                    opr.PosID = posID.InnerText;
                }
                else
                {
                    opr.PosID = string.Empty;
                }

                System.Xml.XmlNodeList bankCardNoList = doc.GetElementsByTagName("BankCardNo");
                System.Xml.XmlNode bankCardNo = bankCardNoList[0];
                if (!string.IsNullOrEmpty(bankCardNo.InnerText))
                {
                    opr.BankCardNo = bankCardNo.InnerText;
                }
                else
                {
                    opr.BankCardNo = string.Empty;
                }

                System.Xml.XmlNodeList payDateList = doc.GetElementsByTagName("PayDate");
                System.Xml.XmlNode payDate = payDateList[0];
                if (!string.IsNullOrEmpty(payDate.InnerText))
                {
                    opr.PayDate = payDate.InnerText;
                }
                else
                {
                    opr.PayDate = string.Empty;
                }

                System.Xml.XmlNodeList payTimeList = doc.GetElementsByTagName("PayTime");
                System.Xml.XmlNode payTime = payTimeList[0];
                if (!string.IsNullOrEmpty(payTime.InnerText))
                {
                    opr.PayTime = payTime.InnerText;
                }
                else
                {
                    opr.PayTime = string.Empty;
                }

                System.Xml.XmlNodeList batchNoList = doc.GetElementsByTagName("BatchNo");
                System.Xml.XmlNode batchNo = batchNoList[0];
                if (!string.IsNullOrEmpty(batchNo.InnerText))
                {
                    opr.BatchNo = batchNo.InnerText;
                }
                else
                {
                    opr.BatchNo = string.Empty;
                }

                System.Xml.XmlNodeList vouchNoList = doc.GetElementsByTagName("VouchNo");
                System.Xml.XmlNode vouchNo = vouchNoList[0];
                if (!string.IsNullOrEmpty(vouchNo.InnerText))
                {
                    opr.VouchNo = vouchNo.InnerText;
                }
                else
                {
                    opr.VouchNo = string.Empty;
                }

                System.Xml.XmlNodeList referNoList = doc.GetElementsByTagName("ReferNo");
                System.Xml.XmlNode referNo = referNoList[0];
                if (!string.IsNullOrEmpty(referNo.InnerText))
                {
                    opr.ReferNo = referNo.InnerText;
                }
                else
                {
                    opr.ReferNo = string.Empty;
                }

                System.Xml.XmlNodeList payAmtList = doc.GetElementsByTagName("PayAmt");
                System.Xml.XmlNode payAmt = payAmtList[0];
                if (!string.IsNullOrEmpty(payAmt.InnerText))
                {
                    opr.PayAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(payAmt.InnerText);
                }
                else
                {
                    opr.PayAmt = 0m;
                }

                System.Xml.XmlNodeList bankCodeList = doc.GetElementsByTagName("BankCode");
                System.Xml.XmlNode bankCode = bankCodeList[0];
                if (!string.IsNullOrEmpty(bankCode.InnerText))
                {
                    opr.BankCode = bankCode.InnerText;
                }
                else
                {
                    opr.BankCode = string.Empty;
                }

                System.Xml.XmlNodeList medInsureTranNoList = doc.GetElementsByTagName("MedInsureTranNo");
                System.Xml.XmlNode medInsureTranNo = medInsureTranNoList[0];
                if (!string.IsNullOrEmpty(medInsureTranNo.InnerText))
                {
                    opr.MedInsureTranNo = medInsureTranNo.InnerText;
                }
                else
                {
                    opr.MedInsureTranNo = string.Empty;
                }

                System.Xml.XmlNodeList medInsureStrList = doc.GetElementsByTagName("MedInsureStr");
                System.Xml.XmlNode medInsureStr = medInsureStrList[0];
                if (!string.IsNullOrEmpty(medInsureStr.InnerText))
                {
                    opr.MedInsureStr = medInsureStr.InnerText;
                }
                else
                {
                    opr.MedInsureStr = string.Empty;
                }

                System.Xml.XmlNodeList medInsureFeeList = doc.GetElementsByTagName("MedInsureFee");
                System.Xml.XmlNode medInsureFee = medInsureFeeList[0];
                if (!string.IsNullOrEmpty(medInsureFee.InnerText))
                {
                    opr.MedInsureFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(medInsureFee.InnerText);
                }
                else
                {
                    opr.MedInsureFee = 0m;
                }

                System.Xml.XmlNodeList personalFeeList = doc.GetElementsByTagName("PersonalFee");
                System.Xml.XmlNode personalFee = personalFeeList[0];
                if (!string.IsNullOrEmpty(personalFee.InnerText))
                {
                    opr.PersonalFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(personalFee.InnerText);
                }
                else
                {
                    opr.PersonalFee = 0m;
                }

                System.Xml.XmlNodeList clincCodeFeeList = doc.GetElementsByTagName("ClincCode");
                System.Xml.XmlNode clincCode = clincCodeFeeList[0];
                if (!string.IsNullOrEmpty(clincCode.InnerText))
                {
                    opr.ClincCode = clincCode.InnerText;
                }
                else
                {
                    opr.ClincCode = string.Empty;
                }


                System.Xml.XmlNodeList Payinsufeestrs = doc.GetElementsByTagName("Payinsufeestr");
                System.Xml.XmlNode Payinsufeestr = Payinsufeestrs[0];
                if (!string.IsNullOrEmpty(Payinsufeestr.InnerText))
                {
                    opr.Payinsufeestr = Payinsufeestr.InnerText;
                }
                else
                {
                    opr.Payinsufeestr = string.Empty;
                }

                try
                {
                    System.Xml.XmlNodeList FeeTypetrs = doc.GetElementsByTagName("FeeType");
                    System.Xml.XmlNode FeeTypestr = FeeTypetrs[0];
                    if (!string.IsNullOrEmpty(FeeTypestr.InnerText))
                    {
                        opr.FeeType = FeeTypestr.InnerText;
                    }
                    else
                    {
                        opr.FeeType = "1";
                    }
                }
                catch
                {
                    opr.FeeType = "1";
                }
                #endregion
            }
            catch (Exception e)
            {
                this.resultCode = "0";
                this.msg = "缺少必填参数！";
                return this.ReturnFailure();
            }

            return returnStr;
        }

        public string GetStoppedSchedulesXML(string xml)
        {
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

            System.Xml.XmlNodeList deviceIDList = doc.GetElementsByTagName("frontproviderid");//frontproviderid  FrontProviderID
            System.Xml.XmlNode deviceID = deviceIDList[0];
            if (!string.IsNullOrEmpty(deviceID.InnerText) && deviceID.InnerText == "ZDWY_ZZSB")
            {
                return "";
            }
            else
            {
                this.resultCode = "0";
                this.msg = "来源编号不能为空！";
                return this.ReturnFailure();
            }



        }

        public string BookUnlockRegisterForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.OutPatientReg opr = new His.Models.ZZSB.OutPatientReg();
            returnStr = GetOutPatientLockModel(xml, opr);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            returnStr = new Booking().BookUnLock(opr);
            return returnStr;
        }

        private string UnlockRegSchema(His.Models.ZZSB.OutPatientReg opr)
        {
            string returnStr = string.Empty;
            string sql = string.Empty;
            #region 判断锁定号源状态
            string lockState = string.Empty;
            sql = Sql.Sql.SelectRegLock;
            sql = string.Format(sql, opr.TranSerNo);

            System.Data.DataTable dt = new System.Data.DataTable();
            //排班表
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][2]))
                    {
                        lockState = dt.Rows[0][2].ToString();
                    }

                    switch (lockState)
                    {
                        case "0":
                            resultCode = "1";
                            msg = "解锁成功";
                            break;
                        case "2":
                            resultCode = "0";
                            msg = "该锁定已解锁";
                            break;
                        case "3":
                            resultCode = "0";
                            msg = "该锁定已占用";
                            break;
                        default:
                            resultCode = "0";
                            msg = "没有找到相应号源";
                            break;
                    }
                    if (resultCode == "0")
                    {
                        return ReturnFailure();
                    }

                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "没有找到相应号源！";
                    return this.ReturnFailure();
                }
            }
            else
            {
                this.resultCode = "0";
                this.msg = "没有找到相应号源！";
                return this.ReturnFailure();
            }
            #endregion

            #region 更新排班表，号源表
            string updateSchema = Sql.Sql.UpdateSchemaReged;
            string updateRegLock = Sql.Sql.UpdateRegLockState;
            ArrayList sqlList = new ArrayList();
            if (resultCode == "1")
            {
                //  updateSchema = string.Format(updateSchema, opr.RegSourceID, "-1");
                updateRegLock = string.Format(updateRegLock, opr.TranSerNo, OPERID, "2");
                //  sqlList.Add(updateSchema);
                sqlList.Add(updateRegLock);

                if (!DataBaseHelp.DataExecHelp.ExecArrayList(sqlList))
                {
                    resultCode = "0";
                    msg = "解锁号源失败！";
                    return ReturnFailure();
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

                System.Xml.XmlElement Result = xml.CreateElement("Result");
                root1.AppendChild(Result);

                System.Xml.XmlElement TranSerNo = xml.CreateElement("TranSerNo");
                TranSerNo.InnerText = opr.TranSerNo;
                Result.AppendChild(TranSerNo);

                System.Xml.XmlElement Note = xml.CreateElement("Note");
                Result.AppendChild(Note);

                returnStr = xml.InnerXml.ToString();
            }

            #endregion

            return returnStr;
        }

        #endregion

        public string FeeResultForSRM(string xml)
        {
            string returnStr = "";
            //His.Models.YYSS.InPatientApply opa = new His.Models.YYSS.InPatientApply();
            //opa = this.GetOutPatientModel(xml);
            //System.Collections.ArrayList al = this.GetOutPatientApplyData(opa);
            //returnStr = this.GetOutPatientApplyXML(al);
            return returnStr;
        }

        #region 医保减免属性
        private string FixmedinsCode = "H44040200001";
        private string FixmedinsName = "中山大学附属第五医院";
        private string MdtrtareaAdmvs = "440400";
        private string Opter = "00W999";
        private string OpterName = "自助设备";
        private string OpterType = "2";
        GDSI.ZhuHaiSI.Business.Comom.MedicalService ms = new GDSI.ZhuHaiSI.Business.Comom.MedicalService();
        His.Business.ZZSB.Medical.MedicalDB db = new His.Business.ZZSB.Medical.MedicalDB();
        #endregion

        #region 医保减免模块

        #region  根据xml入参解析成对应实体对象
        /// <summary>
        /// 根据xml入参解析成对应实体对象
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="opr"></param>
        /// <returns></returns>
        private string GetMedicalRegisterForXml(string xml, His.Models.ZZSB.MedicalModel.MedicalRegister opr)
        {
            string returnStr = string.Empty;
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

            try
            {
                #region 解析入参XML
                System.Xml.XmlNodeList userIDList = doc.GetElementsByTagName("UserID");
                System.Xml.XmlNode userID = userIDList[0];
                if (!string.IsNullOrEmpty(userID.InnerText))
                {
                    opr.UserID = userID.InnerText;
                }
                else
                {
                    opr.UserID = string.Empty;
                }

                System.Xml.XmlNodeList deviceIDList = doc.GetElementsByTagName("DeviceID");
                System.Xml.XmlNode deviceID = deviceIDList[0];
                if (!string.IsNullOrEmpty(deviceID.InnerText))
                {
                    opr.DeviceID = deviceID.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "设备编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList serviceCodeList = doc.GetElementsByTagName("ServiceCode");
                System.Xml.XmlNode serviceCode = serviceCodeList[0];
                if (!string.IsNullOrEmpty(serviceCode.InnerText))
                {
                    opr.ServiceCode = serviceCode.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "服务编码不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList funCodeList = doc.GetElementsByTagName("FunCode");
                System.Xml.XmlNode funCode = funCodeList[0];
                if (!string.IsNullOrEmpty(funCode.InnerText))
                {
                    opr.FunCode = funCode.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "业务编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList reqTimeList = doc.GetElementsByTagName("ReqTime");
                System.Xml.XmlNode reqTime = reqTimeList[0];
                if (!string.IsNullOrEmpty(reqTime.InnerText))
                {
                    opr.ReqTime = reqTime.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "请求时间不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList reqTraceNoList = doc.GetElementsByTagName("ReqTraceNo");
                System.Xml.XmlNode reqTraceNo = reqTraceNoList[0];
                if (!string.IsNullOrEmpty(reqTraceNo.InnerText))
                {
                    opr.ReqTraceNo = reqTraceNo.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "请求流水号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList regSourceIDList = doc.GetElementsByTagName("RegSourceID");
                System.Xml.XmlNode regSourceID = regSourceIDList[0];
                if (!string.IsNullOrEmpty(regSourceID.InnerText))
                {
                    opr.RegSourceID = regSourceID.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "排班编号不能为空！";
                    return this.ReturnFailure();
                }


                try
                {
                    System.Xml.XmlNodeList FeeTypetrs = doc.GetElementsByTagName("FeeType");
                    System.Xml.XmlNode FeeTypestr = FeeTypetrs[0];
                    if (!string.IsNullOrEmpty(FeeTypestr.InnerText))
                    {
                        opr.FeeType = FeeTypestr.InnerText;
                    }
                    else
                    {
                        opr.FeeType = "";
                    }
                }
                catch
                {
                    opr.FeeType = "";
                }


                string MdtrtCertType = doc.GetElementsByTagName("MdtrtCertType")[0].InnerText;
                if (!string.IsNullOrEmpty(MdtrtCertType))
                {
                    opr.MdtrtCertType = MdtrtCertType;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "就诊凭证类型不能为空！";
                    return this.ReturnFailure();
                }

                string MdtrtCertNo = doc.GetElementsByTagName("MdtrtCertNo")[0].InnerText;
                if (!string.IsNullOrEmpty(MdtrtCertNo))
                {
                    opr.MdtrtCertNo = MdtrtCertNo;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "就诊凭证编号不能为空！";
                    return this.ReturnFailure();
                }

                string CardSN = doc.GetElementsByTagName("CardSn")[0].InnerText;
                if (!string.IsNullOrEmpty(CardSN))
                {
                    opr.CardSN = CardSN;
                }
                else
                {
                    if (opr.MdtrtCertNo == "03")
                    {
                        this.resultCode = "0";
                        this.msg = "就诊凭证类型为03时,卡识别码必填！";
                        return this.ReturnFailure();
                    }
                }

                string RegFee = doc.GetElementsByTagName("RegFee")[0].InnerText;
                if (!string.IsNullOrEmpty(RegFee))
                {
                    opr.RegFee = RegFee;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "挂号费不能为空！";
                    return this.ReturnFailure();
                }

                string DeptCode = doc.GetElementsByTagName("DeptCode")[0].InnerText;
                if (!string.IsNullOrEmpty(DeptCode))
                {
                    opr.DeptCode = DeptCode;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "挂号科室不能为空！";
                    return this.ReturnFailure();
                }
                string CardNo = doc.GetElementsByTagName("CardNo")[0].InnerText;
                if (!string.IsNullOrEmpty(CardNo))
                {
                    opr.CardNo = CardNo;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "就诊号不能为空！";
                    return this.ReturnFailure();
                }


                string PsnCertType = doc.GetElementsByTagName("PsnCertType")[0].InnerText;
                if (!string.IsNullOrEmpty(PsnCertType))
                {
                    opr.PsnCertType = PsnCertType;
                }

                string CertNo = doc.GetElementsByTagName("CertNo")[0].InnerText;
                if (!string.IsNullOrEmpty(CertNo))
                {
                    opr.CertNo = CertNo;
                }

                string PsnName = doc.GetElementsByTagName("PsnName")[0].InnerText;
                if (!string.IsNullOrEmpty(PsnName))
                {
                    opr.PsnName = PsnName;
                }

                System.Xml.XmlNodeList InsuplcadmdvsList = doc.GetElementsByTagName("Insuplcadmdvs");
                if (InsuplcadmdvsList.Count > 0)
                {
                    string Insuplcadmdvs = doc.GetElementsByTagName("Insuplcadmdvs")[0].InnerText;
                    if (!string.IsNullOrEmpty(Insuplcadmdvs))
                    {
                        opr.Insuplcadmdvs = Insuplcadmdvs;
                    }
                }

                string SettlementType = doc.GetElementsByTagName("SettlementType")[0].InnerText;
                if (!string.IsNullOrEmpty(SettlementType))
                {
                    opr.SettlementType = SettlementType;
                }


                #endregion
            }
            catch (Exception e)
            {
                this.resultCode = "0";
                this.msg = "缺少必填参数！";
                return this.ReturnFailure();
            }

            return returnStr;
        }
        #endregion

        #region 获取挂号医保减免费用
        /// <summary>
        /// 获取挂号医保减免费用
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string GetRegMedicalFee(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.MedicalModel.MedicalRegister opr = new His.Models.ZZSB.MedicalModel.MedicalRegister();
            returnStr = this.GetMedicalRegisterForXml(xml, opr);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            returnStr = this.RegSettlement(opr);
            return returnStr;
        }
        #endregion

        public string RegSettlement(His.Models.ZZSB.MedicalModel.MedicalRegister opr)
        {
            Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
            GDSI.OutpatientWebService.ClinicBalanceResponseModel outModel2207 = new GDSI.OutpatientWebService.ClinicBalanceResponseModel();
            GDSI.ZhuHaiSI.Model.RegSettlementInModel inModel = new GDSI.ZhuHaiSI.Model.RegSettlementInModel();
            string errorMessage = string.Empty;
            inModel.ClincCode = db.GetClinicCode();
            inModel.CardNo = opr.CardNo;
            inModel.PactCode = opr.FeeType;
            inModel.SchemaID = opr.RegSourceID;
            inModel.MdtrtCertType = opr.MdtrtCertType;
            inModel.MdtrtCertNo = opr.MdtrtCertNo;
            inModel.CardSn = opr.CardSN;
            inModel.PsnCertType = opr.PsnCertType;
            inModel.Certno = opr.CertNo;
            inModel.PsnName = opr.PsnName;
            inModel.RegFee = opr.RegFee;
            inModel.DeptCode = opr.DeptCode;
            inModel.BirctrlType = "";
            inModel.BirctrlMatnDate = "";
            inModel.OpterType = OpterType;
            inModel.OpterCode = Opter;
            inModel.OpterName = OpterName;
            inModel.Insuplcadmdvs = opr.Insuplcadmdvs;

            if (ms.RegSettlementforSettlementType(inModel, opr.SettlementType, ref outModel2207) < 0)
            {
                ms.RollBack();
                this.resultCode = "0";
                this.msg = ms.ErrorMessage;
                if (this.msg.Contains("参保人只能去转诊机构就医"))
                {
                    this.msg = "根据珠海医保政策调整，自2022年12月1日起，取消门诊挂号减免诊金10元政策，统一纳入门诊共济和门特待遇报销。";
                }
                return this.ReturnFailure();

            }

            //
            var medicalFeeCalculatorInput = new MedicalFeeCalculatorInput();
            medicalFeeCalculatorInput.MedfeeSumamt = outModel2207.SetlInfo.MedfeeSumamt;
            medicalFeeCalculatorInput.FundPaySumamt = outModel2207.SetlInfo.FundPaySumamt;
            medicalFeeCalculatorInput.PsnCashPay = outModel2207.SetlInfo.PsnCashPay;
            medicalFeeCalculatorInput.AcctPay = outModel2207.SetlInfo.AcctPay;
            medicalFeeCalculatorInput.AcctMulaidPay = outModel2207.SetlInfo.AcctMulaidPay;
            var medicalFeeCalculatorOutput = MedicalFeeCalculator.Calculate(medicalFeeCalculatorInput);
            if (!medicalFeeCalculatorOutput.IsSuccess)
            {
                ms.RollBack();
                this.resultCode = "0";
                this.msg = ms.ErrorMessage;
                return this.ReturnFailure();

            }

            #region 返回串
            string returnStr = string.Empty;
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

            System.Xml.XmlElement PubCost = xml.CreateElement("PubCost");
            PubCost.InnerText = medicalFeeCalculatorOutput.PubCost.ToString();
            Result.AppendChild(PubCost);

            System.Xml.XmlElement RegNo = xml.CreateElement("RegNo");
            RegNo.InnerText = outModel2207.SetlInfo.MdtrtId.ToString();
            Result.AppendChild(RegNo);

            System.Xml.XmlElement TotCost = xml.CreateElement("TotCost");
            TotCost.InnerText = medicalFeeCalculatorOutput.TotCost.ToString();
            Result.AppendChild(TotCost);

            System.Xml.XmlElement OwnCost = xml.CreateElement("OwnCost");
            OwnCost.InnerText = medicalFeeCalculatorOutput.OwnCost.ToString();
            Result.AppendChild(OwnCost);

            System.Xml.XmlElement ClincCode = xml.CreateElement("ClincCode");
            ClincCode.InnerText = inModel.ClincCode;
            Result.AppendChild(ClincCode);

            returnStr = xml.InnerXml.ToString();
            #endregion

            return returnStr;

        }


        #region 调用医保减免接口获取减免金额
        /// <summary>
        /// 调用医保减免接口获取减免金额
        /// </summary>
        /// <param name="opr"></param>
        /// <returns></returns>
        public string GetRegMedicalFeeForModel(His.Models.ZZSB.MedicalModel.MedicalRegister opr)
        {
            His.Business.ZZSB.Medical.OutPatientService opService = new His.Business.ZZSB.Medical.OutPatientService();
            His.Business.ZZSB.Medical.MedicalDB db = new His.Business.ZZSB.Medical.MedicalDB();

            #region 1.调用医保人员信息获取接口【1101】
            PersonRequestModel Inmodel1101 = new PersonRequestModel();
            PersonResponseModel OutModel1101 = new PersonResponseModel();
            Inmodel1101.MdtrtCertType = opr.MdtrtCertType;
            Inmodel1101.MdtrtCertNo = opr.MdtrtCertNo;
            Inmodel1101.CardSN = opr.CardSN;
            Inmodel1101.BegnTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//获取历史参保信息时传入(非必传)
            Inmodel1101.PsnCertType = opr.PsnCertType;
            Inmodel1101.CertNo = opr.CertNo;//证件号码 非必传
            Inmodel1101.PsnName = opr.PsnName;//人员姓名 非必传
            Inmodel1101.MdtrtareaAdmvs = MdtrtareaAdmvs;
            Inmodel1101.Opter = Opter;
            Inmodel1101.OpterName = OpterName;
            Inmodel1101.OpterType = OpterType;
            Inmodel1101.FixmedinsCode = FixmedinsCode;
            Inmodel1101.FixmedinsName = FixmedinsName;
            OutModel1101 = opService.QueryPerson(Inmodel1101);
            if (OutModel1101.Status != "0")
            {
                this.resultCode = "0";
                this.msg = "调用医保人员信息获取接口1101出现异常：" + OutModel1101.ErrorMsg;
                return this.ReturnFailure();
            }
            string InsuplcAdmdvs = OutModel1101.Insuinfo[0].InsuplcAdmdvs;
            #endregion

            #region 2.若是门特门慢合同单位，则调用人员慢特病备案查询接口【5301】
            string diseCodg = string.Empty;
            string diseName = string.Empty;
            if (opr.FeeType == "248" || opr.FeeType == "252")
            {
                PersonDetailRequestModel InModel5301 = new PersonDetailRequestModel();
                PersonDetailResponseModel OutModel5301 = new PersonDetailResponseModel();
                InModel5301.FixmedinsCode = FixmedinsCode;
                InModel5301.FixmedinsName = FixmedinsName;
                InModel5301.InsuplcAdmdvs = InsuplcAdmdvs;
                InModel5301.MdtrtareaAdmvs = MdtrtareaAdmvs;
                InModel5301.Opter = Opter;
                InModel5301.OpterName = OpterName;
                InModel5301.OpterType = OpterType;
                InModel5301.PsnNo = OutModel1101.BaseInfo.PsnNo;
                OutModel5301 = opService.QueryPersonDetail(InModel5301);
                if (OutModel5301.Status != "0")
                {
                    this.resultCode = "0";
                    this.msg = "调用医保人员慢特病备案查询接口5301出现异常：" + OutModel5301.ErrorMsg;
                    return this.ReturnFailure();
                }
                //创智说默认第一个没问题
                if (OutModel5301.SpInfo != null && OutModel5301.SpInfo.Length > 0)
                {
                    diseCodg = OutModel5301.SpInfo[0].OpspDiseCode;
                    diseName = OutModel5301.SpInfo[0].OpspDiseName;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "没有查询到对应的病种信息！";
                    return this.ReturnFailure();
                }
            }
            #endregion

            #region 3.调用门诊挂号接口【2201】

            //1.先获取门诊流水号
            string clinicCode = db.GetClinicCode();
            if (string.IsNullOrEmpty(clinicCode))
            {
                this.resultCode = "0";
                this.msg = "获取门诊流水号出错,异常信息:" + db.ErrMsg;
                return this.ReturnFailure();
            }
            //2.根据排班ID获取对应挂号科室级别信息等
            opr.medicalSchema = db.GetSchemaForID(opr.RegSourceID);
            if (opr.medicalSchema == null)
            {
                this.resultCode = "0";
                this.msg = "根据排班ID无法查询到排班数据,异常信息:" + db.ErrMsg;
                return this.ReturnFailure();
            }

            ClinicRegisterRequestModel InModel2201 = new ClinicRegisterRequestModel();
            ClinicRegisterResponseModel OutModel2201 = new ClinicRegisterResponseModel();
            InModel2201.MdtrtInfo = new CRMdtrtInfoClass();
            InModel2201.MdtrtInfo.PsnNo = OutModel1101.BaseInfo.PsnNo;
            var listMedType = db.GetComDictionaryForType("NewPactToMedType", opr.FeeType);
            if (listMedType == null || listMedType.Count <= 0)
            {
                this.resultCode = "0";
                this.msg = "没有找到合同单位FeeType:" + opr.FeeType + "对应的医疗类别,请先维护！";
                return this.ReturnFailure();
            }
            string medType = listMedType[0].Name;
            if (string.IsNullOrEmpty(medType))
            {
                this.resultCode = "0";
                this.msg = "没有找到合同单位FeeType:" + opr.FeeType + "对应的医疗类别,请先维护！";
                return this.ReturnFailure();
            }
            //string medType = consMgr.GetConstant("NewPactToMedType", r.Pact.ID).Name == "" ? "11" : consMgr.GetConstant("NewPactToMedType", r.Pact.ID).Name;//若是没有维护合同单位对照，默认11普通门诊
            string insutype = OutModel1101.Insuinfo[0].Insutype;
            InModel2201.MdtrtInfo.Insutype = insutype;//险种类型暂时默认第一个
            InModel2201.MdtrtInfo.Begntime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            InModel2201.MdtrtInfo.MdtrtCertType = Inmodel1101.MdtrtCertType;
            InModel2201.MdtrtInfo.MdtrtCertNo = Inmodel1101.MdtrtCertNo;
            InModel2201.MdtrtInfo.IptOtpNo = clinicCode;
            string doctCode = opr.medicalSchema.DoctCode == "" ? "-" : opr.medicalSchema.DoctCode;//急诊（专科排班）没有医生
            string doctName = opr.medicalSchema.DoctName == "" ? "-" : opr.medicalSchema.DoctName;//急诊（专科排班）没医生
            InModel2201.MdtrtInfo.AtddrNo = doctCode;
            InModel2201.MdtrtInfo.DrName = doctName;
            InModel2201.MdtrtInfo.DeptCode = opr.medicalSchema.DeptCode;
            InModel2201.MdtrtInfo.DeptName = opr.medicalSchema.DeptName;
            InModel2201.MdtrtInfo.Caty = "100";//科别 暂时不知道传啥
            InModel2201.FixmedinsCode = FixmedinsCode;
            InModel2201.FixmedinsName = FixmedinsName;
            InModel2201.InsuplcAdmdvs = InsuplcAdmdvs;
            InModel2201.MdtrtareaAdmvs = MdtrtareaAdmvs;
            InModel2201.Opter = Opter;
            InModel2201.OpterName = OpterName;
            InModel2201.OpterType = OpterType;
            OutModel2201 = opService.Register(InModel2201);
            if (OutModel2201.Status != "0")
            {
                this.resultCode = "0";
                this.msg = "调用医保门诊挂号接口2201出现异常：" + OutModel2201.ErrorMsg;
                return this.ReturnFailure();
            }
            #endregion

            #region 优先记录取消数据（用于报错时回滚医保数据）
            CancelRegModel cancelRegModel = new CancelRegModel();
            cancelRegModel.FixmedinsCode = FixmedinsCode;
            cancelRegModel.FixmedinsName = FixmedinsName;
            cancelRegModel.InsuplcAdmdvs = InsuplcAdmdvs;
            cancelRegModel.IptOtpNo = clinicCode;
            cancelRegModel.MdtrtId = OutModel2201.Data.MdtrtId;
            cancelRegModel.PsnNo = OutModel2201.Data.PsnNo;
            #endregion


            #region 4.调用门诊就诊信息上传接口【2203】
            ClinicMedicalInfoUploadRequestModel InModel2203 = new ClinicMedicalInfoUploadRequestModel();
            ClinicMedicalInfoUploadResponseModel OutModel2203 = new ClinicMedicalInfoUploadResponseModel();
            InModel2203.MdtrtInfo = new CMMdtrtInfoClass();
            InModel2203.MdtrtInfo.MdtrtId = OutModel2201.Data.MdtrtId;
            InModel2203.MdtrtInfo.PsnNo = OutModel2201.Data.PsnNo;
            InModel2203.MdtrtInfo.MedType = medType;
            InModel2203.MdtrtInfo.Begntime = opr.medicalSchema.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
            InModel2203.MdtrtInfo.MainCondDscr = "";
            InModel2203.MdtrtInfo.DiseCodg = diseCodg;
            InModel2203.MdtrtInfo.DiseName = diseName;
            InModel2203.MdtrtInfo.BirctrlType = "";
            InModel2203.MdtrtInfo.BirctrlMatnDate = "";
            List<CMDiagnoseInfoClass> listCMDiagose = new List<CMDiagnoseInfoClass>();
            CMDiagnoseInfoClass CMDiagoseModel = new CMDiagnoseInfoClass();
            CMDiagoseModel.DiagType = "1";
            CMDiagoseModel.DiagSrtNo = "1";
            CMDiagoseModel.DiagCode = "A00.100";
            CMDiagoseModel.DiagName = "霍乱，由于O1群霍乱弧菌，霍乱生物型所致";
            CMDiagoseModel.DiseDorNo = doctCode;
            CMDiagoseModel.DiseDorName = doctName;
            CMDiagoseModel.DiagTime = opr.medicalSchema.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
            CMDiagoseModel.ValiFlag = "0";
            CMDiagoseModel.DiagDept = opr.medicalSchema.DeptCode;
            CMDiagoseModel.MaindiagFlag = "1";
            listCMDiagose.Add(CMDiagoseModel);
            InModel2203.DiseInfo = listCMDiagose.ToArray();
            InModel2203.FixmedinsCode = FixmedinsCode;
            InModel2203.FixmedinsName = FixmedinsName;
            InModel2203.InsuplcAdmdvs = InsuplcAdmdvs;
            InModel2203.MdtrtareaAdmvs = MdtrtareaAdmvs;
            InModel2203.Opter = Opter;
            InModel2203.OpterName = OpterName;
            InModel2203.OpterType = OpterType;
            OutModel2203 = opService.UploadMedInfo(InModel2203);
            if (OutModel2203.Status != "0")
            {
                this.resultCode = "0";
                this.msg = "调用医保门诊就诊信息上传接口2203失败，错误信息：" + OutModel2203.ErrorMsg;
                this.RollMedicalReg(cancelRegModel);
                return this.ReturnFailure();

            }
            #endregion


            #region 5.调用门诊费用明细上传接口【2204】
            ClinicFeeDetailUploadRequestModel InModel2204 = new ClinicFeeDetailUploadRequestModel();
            ClinicFeeDetailUploadResponseModel OutModel2204 = new ClinicFeeDetailUploadResponseModel();
            CFFeeDetailClass cFFeeDetail = new CFFeeDetailClass();
            List<CFFeeDetailClass> cFFeeDetailList = new List<CFFeeDetailClass>();

            //挂号只需要传一个侦查项目就完事了
            cFFeeDetail.FeedetlSn = clinicCode + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            cFFeeDetail.MdtrtId = OutModel2201.Data.MdtrtId;
            cFFeeDetail.PsnNo = OutModel1101.BaseInfo.PsnNo;
            string balanceNo = db.GetMaxBalanceNo(clinicCode);
            cFFeeDetail.ChrgBchno = balanceNo;
            cFFeeDetail.DiseCodg = diseCodg;
            cFFeeDetail.Rxno = "";
            cFFeeDetail.RxCircFlag = "";
            cFFeeDetail.FeeOcurTime = opr.medicalSchema.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
            cFFeeDetail.MedListCodg = "001102000010000-110200001";//目前诊金减免测试接口只有这个能用

            string regfee = "";
            int ret = db.getRegItemCode(opr.medicalSchema.ReglevlCode, ref regfee);
            if (ret < 0)
            {
                this.resultCode = "0";
                this.msg = "根据挂号级别获取挂号费失败!";
                this.RollMedicalReg(cancelRegModel);
                return this.ReturnFailure();

            }
            //if (fixmedinsCode == ZhuHaiSiFunction.FixmedinsCodeXQ)
            //{
            //    regfee = "F00000046581";//基层医疗卫生机构一般诊疗费,校区专用

            //}
            decimal price = NConvert.ToDecimal(db.GetPriceForItemCode(regfee));
            cFFeeDetail.MedinsListCodg = regfee;//医院内项目编码
            cFFeeDetail.DetItemFeeSumamt = price;
            cFFeeDetail.Cnt = 1;
            cFFeeDetail.Pric = price;
            cFFeeDetail.BilgDeptCodg = opr.medicalSchema.DeptCode;
            cFFeeDetail.BilgDeptName = opr.medicalSchema.DeptName;
            cFFeeDetail.BilgDrCodg = doctCode;
            cFFeeDetail.BilgDrName = doctName;
            cFFeeDetail.HospApprFlag = "1";
            cFFeeDetail.RxCircFlag = "0";
            cFFeeDetailList.Add(cFFeeDetail);
            InModel2204.FeeDetail = cFFeeDetailList.ToArray();
            InModel2204.FixmedinsCode = FixmedinsCode;
            InModel2204.FixmedinsName = FixmedinsName;
            InModel2204.InsuplcAdmdvs = InsuplcAdmdvs;
            InModel2204.MdtrtareaAdmvs = MdtrtareaAdmvs;
            InModel2204.Opter = Opter;
            InModel2204.OpterName = OpterName;
            InModel2204.OpterType = OpterType;
            OutModel2204 = opService.UploadFeeInfo(InModel2204);
            if (OutModel2204.Status != "0")
            {
                this.resultCode = "0";
                this.msg = "调用医保门诊费用明细上传接口2204出现异常：" + OutModel2204.ErrorMsg;
                this.RollMedicalReg(cancelRegModel);
                return this.ReturnFailure();
            }
            #endregion

            //记录回滚门诊费用上传收费批次号
            cancelRegModel.ChrgBchno = balanceNo;

            #region 6.调用门诊结算接口【2207】
            ClinicBalanceResponseModel OutModel2207 = new ClinicBalanceResponseModel();
            ClinicBalanceRequestModel InModel2207 = new ClinicBalanceRequestModel();
            //记得先实例化
            InModel2207.MdtrtInfo = new CBMdtrtInfoClass();
            InModel2207.MdtrtInfo.PsnNo = OutModel1101.BaseInfo.PsnNo;
            InModel2207.MdtrtInfo.MdtrtCertNo = opr.MdtrtCertNo;
            InModel2207.MdtrtInfo.MdtrtCertType = opr.MdtrtCertType;
            InModel2207.MdtrtInfo.MedType = medType;
            InModel2207.MdtrtInfo.MedfeeSumamt = price;
            InModel2207.MdtrtInfo.PsnSetlway = "02";
            InModel2207.MdtrtInfo.MdtrtId = OutModel2201.Data.MdtrtId;
            InModel2207.MdtrtInfo.ChrgBchno = balanceNo;
            InModel2207.MdtrtInfo.Insutype = insutype;
            InModel2207.MdtrtInfo.AcctUsedFlag = "0";
            InModel2207.MdtrtInfo.Invono = "";
            InModel2207.FixmedinsCode = FixmedinsCode;
            InModel2207.FixmedinsName = FixmedinsName;
            InModel2207.InsuplcAdmdvs = InsuplcAdmdvs;
            InModel2207.MdtrtareaAdmvs = MdtrtareaAdmvs;
            InModel2207.Opter = Opter;
            InModel2207.OpterName = OpterName;
            InModel2207.OpterType = OpterType;
            OutModel2207 = opService.Balance(InModel2207);
            if (OutModel2207.Status != "0")
            {
                this.resultCode = "0";
                this.msg = "调用医保门诊结算接口2207出现异常：" + OutModel2207.ErrorMsg;
                this.RollMedicalReg(cancelRegModel);
                return this.ReturnFailure();


            }
            #endregion

            //记录回滚结算id
            cancelRegModel.SetlId = OutModel2207.SetlInfo.SetlId;

            #region 6.将返回的数据插入医保主表中
            Neusoft.HISFC.Models.Registration.Register r = new Neusoft.HISFC.Models.Registration.Register();
            r.ID = clinicCode;
            r.Pact.ID = opr.FeeType;
            r.SIMainInfo.InvoiceNo = r.InvoiceNO;
            r.SIMainInfo.MdtrtareaAdmvs = MdtrtareaAdmvs;
            r.SIMainInfo.InsuplcAdmdvs = InsuplcAdmdvs;
            r.SIMainInfo.MdtrtCertType = opr.MdtrtCertType;
            r.SIMainInfo.MdtrtCertNo = opr.MdtrtCertNo;
            r.SIMainInfo.PsnNo = OutModel1101.BaseInfo.PsnNo;
            r.SIMainInfo.PsnCertType = OutModel1101.BaseInfo.PsnCertType;
            r.SIMainInfo.Certno = OutModel1101.BaseInfo.Certno;
            //r.SIMainInfo.PsnName = OutModel1101.BaseInfo.PsnName;
            r.SIMainInfo.Gend = OutModel1101.BaseInfo.Gend;
            r.SIMainInfo.Naty = OutModel1101.BaseInfo.Naty;
            //r.SIMainInfo.Brdy = OutModel1101.BaseInfo.Brdy;
            r.SIMainInfo.Age = OutModel1101.BaseInfo.Age;
            r.SIMainInfo.Insutype = insutype;
            r.SIMainInfo.OpterType = OpterType;
            r.SIMainInfo.RegNo = OutModel2201.Data.MdtrtId;
            r.SIMainInfo.MdtrtID = OutModel2201.Data.MdtrtId;
            r.SIMainInfo.BalNo = balanceNo;
            r.SIMainInfo.TotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.MedfeeSumamt);
            r.SIMainInfo.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.FundPaySumamt);
            r.SIMainInfo.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.PsnPartAmt);
            r.SIMainInfo.SetlId = OutModel2207.SetlInfo.SetlId;
            r.SIMainInfo.SetlTime = OutModel2207.SetlInfo.SetlTime;
            r.SIMainInfo.MedfeeSumamt = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.MedfeeSumamt);
            r.SIMainInfo.FulamtOwnpayAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.FulamtOwnpayAmt);
            r.SIMainInfo.OverlmtSelfpay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.OverlmtSelfpay);
            r.SIMainInfo.PreselfpayAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.PreselfpayAmt);
            r.SIMainInfo.InscpScpAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.InscpScpAmt);
            r.SIMainInfo.ActPayDedc = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.ActPayDedc);
            r.SIMainInfo.HifpPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.HifpPay);
            r.SIMainInfo.PoolPropSelfpay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.PoolPropSelfpay);
            r.SIMainInfo.CvlservPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.CvlservPay);
            r.SIMainInfo.HifesPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.HifesPay);
            r.SIMainInfo.HifmiPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.HifmiPay);
            r.SIMainInfo.HifobPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.HifobPay);
            r.SIMainInfo.MafPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.MafPay);
            r.SIMainInfo.HospPartAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.HospPartAmt);
            r.SIMainInfo.OthPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.OthPay);
            r.SIMainInfo.FundPaySumamt = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.FundPaySumamt);
            r.SIMainInfo.PsnPartAmt = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.PsnPartAmt);
            r.SIMainInfo.AcctPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.AcctPay);
            r.SIMainInfo.PsnCashPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.PsnCashPay);
            r.SIMainInfo.Balc = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.Balc);
            r.SIMainInfo.AcctMulaidPay = Neusoft.FrameWork.Function.NConvert.ToDecimal(OutModel2207.SetlInfo.AcctMulaidPay);
            r.SIMainInfo.MedinsSetlId = OutModel2207.SetlInfo.MedinsSetlId;
            r.SIMainInfo.ClrOptins = OutModel2207.SetlInfo.ClrOptins;
            r.SIMainInfo.ClrWay = OutModel2207.SetlInfo.ClrWay;
            r.SIMainInfo.ClrType = OutModel2207.SetlInfo.ClrType;
            r.SIMainInfo.TypeCode = "0";
            r.SIMainInfo.MedType = medType;
            if (db.NewInsertOutPatientReg(r) < 0)
            {
                this.resultCode = "0";
                this.msg = "插入医保主表信息失败：" + db.ErrMsg;
                this.RollMedicalReg(cancelRegModel);
                return this.ReturnFailure();


            }
            #endregion

            #region 返回串
            string returnStr = string.Empty;
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

            System.Xml.XmlElement FundPaySumamt = xml.CreateElement("FundPaySumamt");
            FundPaySumamt.InnerText = r.SIMainInfo.FundPaySumamt.ToString();
            Result.AppendChild(FundPaySumamt);

            System.Xml.XmlElement PsnPartAmt = xml.CreateElement("PsnPartAmt");
            PsnPartAmt.InnerText = r.SIMainInfo.PsnPartAmt.ToString();
            Result.AppendChild(PsnPartAmt);

            System.Xml.XmlElement ClincCode = xml.CreateElement("ClincCode");
            ClincCode.InnerText = clinicCode;
            Result.AppendChild(ClincCode);

            returnStr = xml.InnerXml.ToString();
            #endregion

            return returnStr;
        }
        #endregion

        #region 调用异常回滚医保
        /// <summary>
        /// 调用异常回滚医保
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int RollMedicalReg(CancelRegModel model)
        {
            His.Business.ZZSB.Medical.OutPatientService opService = new His.Business.ZZSB.Medical.OutPatientService();
            #region 1.回滚门诊结算
            //回滚结算ID不为空 证明需要回滚门诊结算数据
            if (!string.IsNullOrEmpty(model.SetlId))
            {
                ClinicCancelBalanceRequestModel InModel2208 = new ClinicCancelBalanceRequestModel();
                ClinicCancelBalanceResponseModel OutModel2208 = new ClinicCancelBalanceResponseModel();
                InModel2208.FixmedinsCode = FixmedinsCode;
                InModel2208.FixmedinsName = FixmedinsName;
                InModel2208.InsuplcAdmdvs = model.InsuplcAdmdvs;
                InModel2208.MdtrtareaAdmvs = MdtrtareaAdmvs;
                InModel2208.Opter = Opter;
                InModel2208.OpterName = OpterName;
                InModel2208.OpterType = OpterType;
                InModel2208.MdtrtInfo = new CCMdtrtInfoClass();
                InModel2208.MdtrtInfo.MdtrtId = model.MdtrtId;
                InModel2208.MdtrtInfo.PsnNo = model.PsnNo;
                InModel2208.MdtrtInfo.SetlId = model.SetlId;
                OutModel2208 = opService.CancelBalance(InModel2208);
                if (OutModel2208.Status != "0")
                {
                    //this.ErrMsg = "调用医保撤销门诊结算接口2208出现异常:" + OutModel2208.ErrorMsg;
                    return -1;
                }
            }
            #endregion

            #region 2.回滚门诊费用上传
            //若回滚收费批次号不为空，则证明需要回滚门诊费用上传接口
            if (!string.IsNullOrEmpty(model.ChrgBchno))
            {
                CancelFeeDetailUploadRequestModel InModel2205 = new CancelFeeDetailUploadRequestModel();
                CancelFeeDetailUploadResponseModel OutModel2205 = new CancelFeeDetailUploadResponseModel();
                InModel2205.FeeInfo = new CFUploadInfoClass();
                InModel2205.FeeInfo.ChrgBchno = model.ChrgBchno;
                InModel2205.FeeInfo.MdtrtId = model.MdtrtId;
                InModel2205.FeeInfo.PsnNo = model.PsnNo;
                InModel2205.FixmedinsCode = FixmedinsCode;
                InModel2205.FixmedinsName = FixmedinsName;
                InModel2205.InsuplcAdmdvs = model.InsuplcAdmdvs;
                InModel2205.MdtrtareaAdmvs = MdtrtareaAdmvs;
                InModel2205.Opter = Opter;
                InModel2205.OpterName = OpterName;
                InModel2205.OpterType = OpterType;
                OutModel2205 = opService.CancelUploadFeeInfo(InModel2205);
                if (OutModel2205.Status != "0")
                {
                    //this.ErrMsg = "调用医保撤销门诊费用上传接口2205出现异常:" + OutModel2205.ErrorMsg;
                    return -1;
                }
            }


            #endregion

            #region 3.回滚门诊挂号接口
            ClinicCancelRegisterRequestModel InModel2202 = new ClinicCancelRegisterRequestModel();
            ClinicCancelRegisterResponseModel OutModel2202 = new ClinicCancelRegisterResponseModel();
            InModel2202.MdtrtInfo = new CCRMdtrtInfoClass();
            InModel2202.MdtrtInfo.PsnNo = model.PsnNo;
            InModel2202.MdtrtInfo.MdtrtId = model.MdtrtId;
            InModel2202.MdtrtInfo.IptOtpNo = model.IptOtpNo;
            InModel2202.FixmedinsCode = FixmedinsCode;
            InModel2202.FixmedinsName = FixmedinsName;
            InModel2202.InsuplcAdmdvs = model.InsuplcAdmdvs;
            InModel2202.MdtrtareaAdmvs = MdtrtareaAdmvs;
            InModel2202.Opter = Opter;
            InModel2202.OpterName = OpterName;
            InModel2202.OpterType = OpterType;
            OutModel2202 = opService.CancelRegister(InModel2202);
            if (OutModel2202.Status != "0")
            {
                // this.ErrMsg = "调用医保撤销门诊挂号接口接口2202出现异常:" + OutModel2202.ErrorMsg;
                return -1;
            }
            #endregion

            #region 4.回滚成功以后，将对应的医保主表数据置为无效状态
            His.Business.ZZSB.Medical.MedicalDB db = new His.Business.ZZSB.Medical.MedicalDB();
            if (db.NewUpdateSiMainInfoValidFlag(model.IptOtpNo, model.MdtrtId, model.ChrgBchno, "0") < 0)
            {
                //this.ErrMsg = "更新医保主表有效性出错:" + this.db.Err;
                return -1;
            }
            #endregion
            return 1;
        }
        #endregion

        #endregion

        /// <summary>
        /// 取消医保减免
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string CancelRegMedicalFee(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.MedicalModel.MedicalRegister opr = new His.Models.ZZSB.MedicalModel.MedicalRegister();
            returnStr = this.CancelRegMedicalFeeForXml(xml, opr);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            returnStr = this.CancelRegSettlement(opr);
            return returnStr;
        }

        public string CancelRegSettlement(His.Models.ZZSB.MedicalModel.MedicalRegister opr)
        {
            Neusoft.FrameWork.Management.Connection.Hospital.ID = "CORE_HIS50";
            if (ms.CancelRegSettlement(opr.ClincCode, Opter, OpterName, OpterType) < 0)
            {
                this.resultCode = "0";
                this.msg = ms.ErrorMessage;
                return this.ReturnFailure();
            }
            #region 返回串
            string returnStr = string.Empty;
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

        private string CancelRegMedicalFeeForXml(string xml, His.Models.ZZSB.MedicalModel.MedicalRegister opr)
        {
            string returnStr = string.Empty;
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

            try
            {
                #region 解析入参XML
                System.Xml.XmlNodeList userIDList = doc.GetElementsByTagName("UserID");
                System.Xml.XmlNode userID = userIDList[0];
                if (!string.IsNullOrEmpty(userID.InnerText))
                {
                    opr.UserID = userID.InnerText;
                }
                else
                {
                    opr.UserID = string.Empty;
                }

                System.Xml.XmlNodeList deviceIDList = doc.GetElementsByTagName("DeviceID");
                System.Xml.XmlNode deviceID = deviceIDList[0];
                if (!string.IsNullOrEmpty(deviceID.InnerText))
                {
                    opr.DeviceID = deviceID.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "设备编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList serviceCodeList = doc.GetElementsByTagName("ServiceCode");
                System.Xml.XmlNode serviceCode = serviceCodeList[0];
                if (!string.IsNullOrEmpty(serviceCode.InnerText))
                {
                    opr.ServiceCode = serviceCode.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "服务编码不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList funCodeList = doc.GetElementsByTagName("FunCode");
                System.Xml.XmlNode funCode = funCodeList[0];
                if (!string.IsNullOrEmpty(funCode.InnerText))
                {
                    opr.FunCode = funCode.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "业务编号不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList reqTimeList = doc.GetElementsByTagName("ReqTime");
                System.Xml.XmlNode reqTime = reqTimeList[0];
                if (!string.IsNullOrEmpty(reqTime.InnerText))
                {
                    opr.ReqTime = reqTime.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "请求时间不能为空！";
                    return this.ReturnFailure();
                }

                System.Xml.XmlNodeList ClincCodeNoList = doc.GetElementsByTagName("ClincCode");
                System.Xml.XmlNode ClincCode = ClincCodeNoList[0];
                if (!string.IsNullOrEmpty(ClincCode.InnerText))
                {
                    opr.ClincCode = ClincCode.InnerText;
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "门诊流水号不能为空！";
                    return this.ReturnFailure();
                }


                #endregion
            }
            catch (Exception e)
            {
                this.resultCode = "0";
                this.msg = "缺少必填参数！";
                return this.ReturnFailure();
            }

            return returnStr;
        }

        private string CancelRegMedicalFeeForModel(His.Models.ZZSB.MedicalModel.MedicalRegister opr)
        {
            His.Business.ZZSB.Medical.OutPatientService opService = new His.Business.ZZSB.Medical.OutPatientService();
            His.Business.ZZSB.Medical.MedicalDB db = new His.Business.ZZSB.Medical.MedicalDB();

            #region 1.根据流水号查询是否存在医保数据
            Neusoft.HISFC.Models.Registration.Register r = new Neusoft.HISFC.Models.Registration.Register();
            r = db.NewGetRegPersonInfo(opr.ClincCode, "0", r);
            if (r == null)
            {
                this.resultCode = "0";
                this.msg = "根据门诊流水号未在his中查询到指定医保业务:" + db.ErrMsg;
                return this.ReturnFailure();

            }
            #endregion

            string insuplcAdmdvs = r.SIMainInfo.InsuplcAdmdvs;
            string mdtrtareaAdmvs = r.SIMainInfo.MdtrtareaAdmvs;

            #region 2.调用门诊结算撤销接口【2208】
            ClinicCancelBalanceRequestModel InModel2208 = new ClinicCancelBalanceRequestModel();
            ClinicCancelBalanceResponseModel OutModel2208 = new ClinicCancelBalanceResponseModel();
            InModel2208.MdtrtInfo = new CCMdtrtInfoClass();
            InModel2208.MdtrtInfo.MdtrtId = r.SIMainInfo.MdtrtID;
            InModel2208.MdtrtInfo.SetlId = r.SIMainInfo.SetlId;
            InModel2208.MdtrtInfo.PsnNo = r.SIMainInfo.PsnNo;
            InModel2208.FixmedinsCode = FixmedinsCode;
            InModel2208.FixmedinsName = FixmedinsName;
            InModel2208.InsuplcAdmdvs = insuplcAdmdvs;
            InModel2208.MdtrtareaAdmvs = mdtrtareaAdmvs;
            InModel2208.Opter = Opter;
            InModel2208.OpterName = OpterName;
            InModel2208.OpterType = OpterType;
            OutModel2208 = opService.CancelBalance(InModel2208);
            if (OutModel2208.Status != "0")
            {
                this.resultCode = "0";
                this.msg = "调用医保门诊结算撤销接口2208异常:" + OutModel2208.ErrorMsg;
                return this.ReturnFailure();

            }
            #endregion

            #region 3.调用门诊费用上传撤销接口【2205】
            CancelFeeDetailUploadRequestModel InModel2205 = new CancelFeeDetailUploadRequestModel();
            CancelFeeDetailUploadResponseModel OutModel2205 = new CancelFeeDetailUploadResponseModel();
            InModel2205.FeeInfo = new CFUploadInfoClass();
            InModel2205.FeeInfo.ChrgBchno = r.SIMainInfo.BalNo;
            InModel2205.FeeInfo.MdtrtId = r.SIMainInfo.MdtrtID;
            InModel2205.FeeInfo.PsnNo = r.SIMainInfo.PsnNo;
            InModel2205.FixmedinsCode = FixmedinsCode;
            InModel2205.FixmedinsName = FixmedinsName;
            InModel2205.InsuplcAdmdvs = insuplcAdmdvs;
            InModel2205.MdtrtareaAdmvs = mdtrtareaAdmvs;
            InModel2205.Opter = Opter;
            InModel2205.OpterName = OpterName;
            InModel2205.OpterType = OpterType;
            OutModel2205 = opService.CancelUploadFeeInfo(InModel2205);
            if (OutModel2205.Status != "0")
            {
                this.resultCode = "0";
                this.msg = "调用医保门诊费用上传撤销接口2205异常:" + OutModel2205.ErrorMsg;
                return this.ReturnFailure();

            }
            #endregion

            #region 4.调用门诊挂号撤销接口【2202】
            ClinicCancelRegisterRequestModel InModel2202 = new ClinicCancelRegisterRequestModel();
            ClinicCancelRegisterResponseModel OutModel2202 = new ClinicCancelRegisterResponseModel();
            InModel2202.MdtrtInfo = new CCRMdtrtInfoClass();
            InModel2202.MdtrtInfo.PsnNo = r.SIMainInfo.PsnNo;
            InModel2202.MdtrtInfo.MdtrtId = r.SIMainInfo.MdtrtID;
            InModel2202.MdtrtInfo.IptOtpNo = r.ID;
            InModel2202.FixmedinsCode = FixmedinsCode;
            InModel2202.FixmedinsName = FixmedinsName;
            InModel2202.InsuplcAdmdvs = insuplcAdmdvs;
            InModel2202.MdtrtareaAdmvs = mdtrtareaAdmvs;
            InModel2202.Opter = Opter;
            InModel2202.OpterName = OpterName;
            InModel2202.OpterType = OpterType;
            OutModel2202 = opService.CancelRegister(InModel2202);
            if (OutModel2202.Status != "0")
            {
                this.resultCode = "0";
                this.msg = "调用医保挂号撤销接口2202出现异常:" + OutModel2202.ErrorMsg;
                return this.ReturnFailure();

            }
            #endregion

            #region 5.将对应的医保主表数据置为无效状态
            if (db.NewUpdateSiMainInfoValidFlag(r.ID, r.SIMainInfo.RegNo, r.SIMainInfo.BalNo, "0") < 0)
            {
                this.resultCode = "0";
                this.msg = "更新医保主表有效性出错:" + db.ErrMsg;
                return this.ReturnFailure();
            }
            #endregion

            #region 返回串
            string returnStr = string.Empty;
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



        public string CancelRegInfoOutpatient()
        {
            return "";
        }


        public string SubmitTheRegisterForSRM(string xml)
        {
            string returnStr = "";
            His.Models.ZZSB.OutPatientReg opr = new His.Models.ZZSB.OutPatientReg();
            returnStr = this.GetOutPatientRegModel(xml, opr);
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr;
            }
            returnStr = this.SubmitRegister(opr);
            return returnStr;
        }

        private string SubmitRegister(His.Models.ZZSB.OutPatientReg opr)
        {
            RegisterManager mgr = new RegisterManager();
            this.resultCode = "190302";
            string returnStr = string.Empty;
            //DateTime now = this.GetSysDate();
            DateTime now = Shadow.Util.Data.Func.NConvert.ToDateTime(mgr.GetSysDateTime());

            try
            {
                //这里开始增加事务控制 20161117 alter by  y_ming
                Shadow.Util.Data.Management.Trans.BeginTransaction();


                #region 这里只做数据锁行作用

                //                string lockBook = @"update fin_opr_schema a
                //                    set a.reged=a.reged
                //                    where a.id='{0}'
                //                    and a.valid_flag='1'
                //                    and a.stop<>'1' ";
                string lockBook = @"update com_dictionary a
                                           set a.name = '0'
                                         where upper(a.type) = 'REGLOCK'
                                           and a.code = 'ZZSB0001'";
                //lockBook = string.Format(lockBook, opr.RegSourceID);

                if (mgr.ExecNoQuery(lockBook) != 1)
                {
                    this.msg = "更新号源锁号发生错误！" + mgr.Err;
                    return ReturnFailure();
                }

                #endregion

                #region 验证挂号患者信息和挂号级别

                //returnStr = this.ValidData(opr);
                //if (!string.IsNullOrEmpty(returnStr))
                //{
                //    return returnStr;
                //}

                #endregion

                #region 获取患者信息，排班信息，挂号登记费用等

                string schemaSql = Sql.Sql.GetSchema;
                string compatientSql = Sql.Sql.GetPatientInfo;
                string regfeeSql = Sql.Sql.GetRegFee;
                string nurQueueSql1 = Sql.Sql.GetNurQueueByDept;
                string nurQueueSql2 = Sql.Sql.GetNurQueueByDoct;
                string invoicenoSql1 = Sql.Sql.GetInvoiceInfoUsed;
                string invoicenoSql2 = Sql.Sql.GetInvoiceR;
                string invoicenoSql3 = Sql.Sql.GetInvoiceUserCode;
                string seenoSql = Sql.Sql.GetSeeNo;
                string clinicCodeSql = Sql.Sql.GetClinicCode;
                string noonSql = Sql.Sql.GetNoonName;
                string intimesSql = Sql.Sql.GetOutPatientInTimes;
                string pactSql = Sql.Sql.GetPactInfo;

                string getnewseeno = Sql.Sql.GetNewSeeNo;
                string updateseeno = Sql.Sql.UpdateSeeNo;
                string getPediatricsDeptCodeListSQL = Sql.Sql.GetPediatricsDeptCodeList;
                string get14AgelimitDeptCodeList = Sql.Sql.Get14AgelimitDeptCodeList;
                #region 判断是否有足够号源

                int regRemainCount = 0;
                string sql = @"select (t.reg_lmt - t.reged) regRemain
                                                          from fin_opr_schema t
                                                         where t.id = '{0}'";
                sql = string.Format(sql, opr.RegSourceID);
                //排班表
                regRemainCount = Neusoft.FrameWork.Function.NConvert.ToInt32(mgr.ExecSqlReturnOne(sql));

                if (regRemainCount <= 0)
                {
                    this.msg = "没有足够号源，请选择其他排班！";
                    return this.ReturnFailure();
                }
                mgr.ExecQuery("select '" + opr.RegSourceID + "-" + regRemainCount.ToString() + "' from dual ");
                #endregion

                #region 获取患者基本信息

                compatientSql = string.Format(compatientSql, opr.CardNo);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(compatientSql);
                His.Models.ZZSB.ComPatient patient = null;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient = new His.Models.ZZSB.ComPatient();
                            patient.CardNo = dt.Rows[i][0].ToString();
                            patient.Name = dt.Rows[i][1].ToString();
                            patient.Birthday = dt.Rows[i][2].ToString();
                            patient.SexCode = dt.Rows[i][3].ToString();
                            patient.IDCard = dt.Rows[i][4].ToString();
                            patient.McardNo = dt.Rows[i][5].ToString();
                            patient.HomePhone = dt.Rows[i][6].ToString();
                            patient.Address = dt.Rows[i][7].ToString();
                            patient.RegDate = now;
                            break;
                        }
                        if (patient == null || string.IsNullOrEmpty(patient.CardNo))
                        {
                            // resultCode = "0";
                            msg = "获取患者信息出错！";
                            return ReturnFailure();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.msg = "没有找到患者信息！";
                        return this.ReturnFailure();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.msg = "没有找到患者信息！";
                    return this.ReturnFailure();
                }

                #endregion

                //代表走医保 需要从医保结算表获取合同单位
                His.Business.ZZSB.Medical.MedicalDB db = new His.Business.ZZSB.Medical.MedicalDB();
                if (!string.IsNullOrEmpty(opr.ClincCode))
                {
                    opr.FeeType = db.GetSiPactCodeForClinCode(opr.ClincCode);
                    if (opr.FeeType == "-1" || string.IsNullOrEmpty(opr.FeeType))
                    {
                        //this.resultCode = "0";
                        this.msg = "没有找到门诊流水号对应的医保结算合同单位！";
                        return this.ReturnFailure();
                    }
                }

                #region 获取合同单位
                if (!string.IsNullOrEmpty(opr.Payinsufeestr))
                {
                    List<string> infos = opr.Payinsufeestr.Split('^').ToList();
                    if (infos.Count >= 2)
                    {
                        //if (!string.IsNullOrEmpty(infos[1]))
                        //{
                        //    opr.FeeType = "252";
                        //}
                        if (infos.Count >= 10)
                        {
                            if (!string.IsNullOrEmpty(infos[8]) && string.IsNullOrEmpty(patient.IDCard))
                            {
                                patient.IDCard = infos[8];
                            }
                            if (!string.IsNullOrEmpty(infos[9]) && string.IsNullOrEmpty(patient.McardNo))
                            {
                                patient.McardNo = infos[9];
                            }
                        }
                    }
                }
                pactSql = string.Format(pactSql, opr.FeeType);
                dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(pactSql);
                His.Models.ZZSB.PactInfo pactUnit = null;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            pactUnit = new His.Models.ZZSB.PactInfo();

                            pactUnit.ID = dt.Rows[i][0].ToString();//合同代码          
                            pactUnit.Name = dt.Rows[i][1].ToString();//合同单位名称                    
                            pactUnit.PayKind.ID = dt.Rows[i][2].ToString();//结算类别                    
                            pactUnit.Rate.PubRate = NConvert.ToDecimal(dt.Rows[i][3].ToString().Trim());//公费比例                    
                            pactUnit.Rate.PayRate = NConvert.ToDecimal(dt.Rows[i][4].ToString().Trim());//自付比例                   
                            pactUnit.Rate.OwnRate = NConvert.ToDecimal(dt.Rows[i][5].ToString().Trim()); //自费比例                   
                            pactUnit.Rate.RebateRate = NConvert.ToDecimal(dt.Rows[i][6].ToString().Trim()); //优惠比例                    
                            pactUnit.Rate.ArrearageRate = NConvert.ToDecimal(dt.Rows[i][7].ToString().Trim());//欠费比例                    
                            pactUnit.Rate.IsBabyShared = NConvert.ToBoolean(dt.Rows[i][8].ToString());//婴儿标志 0 无关 1 有关                                
                            pactUnit.IsNeedMCard = NConvert.ToBoolean(dt.Rows[i][9].ToString().Trim()); //是否要求必须有医疗证号 0 否 1 是                      
                            pactUnit.IsInControl = NConvert.ToBoolean(dt.Rows[i][10].ToString().Trim());//是否受监控 1受监控0不受监控                   
                            pactUnit.ItemType = dt.Rows[i][11].ToString().Trim(); //标志  0 全部 1 药品 2 非药品   
                            pactUnit.DayQuota = NConvert.ToDecimal(dt.Rows[i][12].ToString().Trim());//日限额                     
                            pactUnit.MonthQuota = NConvert.ToDecimal(dt.Rows[i][13].ToString().Trim()); //月限额                    
                            pactUnit.YearQuota = NConvert.ToDecimal(dt.Rows[i][14].ToString().Trim());//年限额
                            pactUnit.OnceQuota = NConvert.ToDecimal(dt.Rows[i][15].ToString().Trim());//一次限
                            string PriceForm = dt.Rows[i][16].ToString();
                            if (PriceForm == "0")
                            {
                                pactUnit.PriceForm = "默认价";
                            }
                            else if (PriceForm == "1")
                            {
                                pactUnit.PriceForm = "特诊价";
                            }
                            else if (PriceForm == "2")
                            {
                                pactUnit.PriceForm = "儿童价";
                            }
                            //{B9303CFE-755D-4585-B5EE-8C1901F79450}maokb增加购入价
                            else if (PriceForm == "3")
                            {
                                pactUnit.PriceForm = "购入价";
                            }
                            else
                            {
                                pactUnit.PriceForm = "默认价";
                            }

                            pactUnit.BedQuota = NConvert.ToDecimal(dt.Rows[i][17].ToString());//床位限额
                            pactUnit.AirConditionQuota = NConvert.ToDecimal(dt.Rows[i][18].ToString());//空调限额
                            pactUnit.SortID = NConvert.ToInt32(dt.Rows[i][19]);//序号             
                            pactUnit.ShortName = dt.Rows[i][20].ToString();//合同单位简称
                            pactUnit.PactDllName = dt.Rows[i][21].ToString(); //待遇dll名称
                            pactUnit.PactDllDescription = dt.Rows[i][22].ToString();//待遇dll说明
                            pactUnit.PactSystemType = dt.Rows[i][23].ToString().Trim();

                            switch (pactUnit.PactSystemType)
                            {
                                case "1":
                                    pactUnit.PactSystemType = "门诊";
                                    break;
                                case "2":
                                    pactUnit.PactSystemType = "住院";
                                    break;
                                case "3":
                                    pactUnit.PactSystemType = "系统";
                                    break;
                                default:
                                    pactUnit.PactSystemType = "全院";
                                    break;
                            }
                            pactUnit.SpellCode = dt.Rows[i][24].ToString();//拼音码
                            pactUnit.WBCode = dt.Rows[i][25].ToString();//五笔码
                            pactUnit.PatientType.ID = dt.Rows[i][26].ToString();//人员类型编码
                            pactUnit.PatientType.Name = dt.Rows[i][27].ToString();//人员类型名称
                            pactUnit.IsUseInOutPatientFee = NConvert.ToBoolean(dt.Rows[i][28].ToString().Trim());

                            break;
                        }
                        if (pactUnit == null || string.IsNullOrEmpty(pactUnit.ID))
                        {
                            // resultCode = "0";
                            msg = "获取合同单位信息出错！";
                            return ReturnFailure();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.msg = "没有找到合同单位信息！";
                        return this.ReturnFailure();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.msg = "没有找到合同单位信息！";
                    return this.ReturnFailure();
                }
                patient.Pact = pactUnit;
                #endregion

                #region 支付方式

                if (Function.SetPayType(opr.PayType, ref  patient, ref msg) == 0)
                {
                    // this.resultCode = "0";
                    //this.msg = "支付方式赋值错误!";
                    return this.ReturnFailure();
                }

                #endregion

                #region 获取排班信息

                schemaSql = string.Format(schemaSql, opr.RegSourceID);
                dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(schemaSql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.SchemaID = dt.Rows[i][0].ToString();
                            patient.SchemaType = dt.Rows[i][1].ToString();//排班类型，0科室/1医生
                            patient.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][2].ToString());
                            patient.Noon.ID = dt.Rows[i][4].ToString();
                            patient.Dept.ID = dt.Rows[i][5].ToString();
                            patient.Dept.Name = dt.Rows[i][6].ToString();
                            patient.Doct.ID = dt.Rows[i][7].ToString();
                            patient.Doct.Name = dt.Rows[i][8].ToString();
                            patient.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][20].ToString());
                            patient.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[i][21].ToString());
                            patient.RegLevel.ID = dt.Rows[i][29].ToString();
                            patient.RegLevel.Name = dt.Rows[i][30].ToString();
                            patient.Room.ID = dt.Rows[i][31].ToString();
                            patient.Room.Name = dt.Rows[i][32].ToString();
                            patient.Console.ID = dt.Rows[i][33].ToString();
                            patient.Console.Name = dt.Rows[i][34].ToString();
                            break;
                        }
                        if (string.IsNullOrEmpty(patient.SchemaID))
                        {
                            // resultCode = "0";
                            msg = "获取排班信息出错！";
                            return ReturnFailure();
                        }
                    }
                    else
                    {
                        // this.resultCode = "0";
                        this.msg = "没有找到排班信息！";
                        return this.ReturnFailure();
                    }
                }
                else
                {
                    //this.resultCode = "0";
                    this.msg = "没有找到排班信息！";
                    return this.ReturnFailure();
                }

                #endregion

                #region 14岁以下不能挂急诊内科

                if (patient.Dept.ID == "1026")
                {
                    if (string.IsNullOrEmpty(patient.Birthday))
                    {
                        DateTime dd = DateTime.MinValue;
                        if (DateTime.TryParse(patient.Birthday, out dd))
                            if (dd.AddDays(14 * 365) < DateTime.Now)
                            {
                                this.msg = "14周岁以下不能挂急诊内科！";
                                return this.ReturnFailure();
                            }
                    }
                }

                if (!string.IsNullOrEmpty(patient.Birthday))
                {
                    DateTime dd = DateTime.MinValue;
                    if (DateTime.TryParse(patient.Birthday, out dd))
                    {
                        if (dd.AddDays(14 * 365) > mgr.GetDateTimeFromSysDateTime())
                        {
                            DataTable deptDt = DataBaseHelp.DataExecHelp.GetDataTable(get14AgelimitDeptCodeList);
                            string Age14limitDeptCodeList = "";
                            try
                            {
                                if (deptDt.Rows.Count > 0)
                                {
                                    Age14limitDeptCodeList = deptDt.Rows[0][0].ToString();
                                }
                                if (Age14limitDeptCodeList.IndexOf(patient.Dept.ID) != -1)
                                {
                                    this.msg = "14周岁以下不能挂此科室！";
                                    return this.ReturnFailure();
                                }
                            }
                            catch (Exception)
                            {


                            }

                        }
                        else
                        {
                            DataTable deptDt = DataBaseHelp.DataExecHelp.GetDataTable(getPediatricsDeptCodeListSQL);
                            string PediatricsDeptCodeList = "";
                            try
                            {
                                if (deptDt.Rows.Count > 0)
                                {
                                    PediatricsDeptCodeList = deptDt.Rows[0][0].ToString();
                                }
                                if (PediatricsDeptCodeList.IndexOf(patient.Dept.ID) != -1)
                                {
                                    this.msg = "14周岁以上不能挂儿科！";
                                    return this.ReturnFailure();
                                }
                            }
                            catch (Exception)
                            {


                            }

                            if (patient.Dept.ID == "6002")
                            {
                                this.msg = "14周岁以上不能挂儿科！";
                                return this.ReturnFailure();

                            }
                            if (patient.Dept.ID == "6181")
                            {
                                this.msg = "14周岁以上不能挂儿童发热门诊！";
                                return this.ReturnFailure();

                            }
                        }
                    }
                }

                //男性
                if (patient.SexCode == "M")
                {
                    if (patient.Dept.ID == "6070")
                    {
                        this.msg = "男性不能挂该科室！";
                        return this.ReturnFailure();

                    }
                }

                #endregion

                #region 获取挂号等级费用

                regfeeSql = string.Format(regfeeSql, "1", patient.RegLevel.ID);
                dt = new System.Data.DataTable();
                dt = DataBaseHelp.DataExecHelp.GetDataTable(regfeeSql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][4]);//挂号费
                            patient.OwnDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(dt.Rows[i][6]);//诊查费
                            break;
                        }
                        if (patient.OwnDigFee == null || string.IsNullOrEmpty(patient.OwnDigFee.ToString()))
                        {
                            //resultCode = "0";
                            msg = "获取费用信息出错！";
                            return ReturnFailure();
                        }
                    }
                    else
                    {
                        //this.resultCode = "0";
                        this.msg = "没有找到费用信息！";
                        return this.ReturnFailure();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.msg = "没有找到费用信息！";
                    return this.ReturnFailure();
                }

                #endregion

                #region 获取护士分诊队列信息
                dt = new System.Data.DataTable();
                if (patient.SchemaType == "0")
                {
                    //为科室排班
                    // nurQueueSql1 = string.Format(nurQueueSql1, now.ToString("yyyy-MM-dd HH:mm:ss"), patient.Dept.ID, patient.Noon.ID, patient.Room.ID);
                    nurQueueSql1 = string.Format(nurQueueSql1, patient.SchemaID);
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(nurQueueSql1);
                }
                else if (patient.SchemaType == "1")
                {
                    //为医生排班
                    string nurQueueNewSql2 = string.Format(@"SELECT nurse_cell_code, --门诊护士站代码
                                                           queue_code, --队列代码
                                                           queue_name, --队列名称
                                                           noon_code, --午别
                                                           queue_flag, --1医生队列/2自定义队列
                                                           sort_id, --显示顺序
                                                           valid_flag, --1有效/0无效
                                                           remark, --备注
                                                           oper_code, --操作员
                                                           oper_date, --操作时间
                                                           queue_date, --队列日期
                                                           doct_code, --看诊医生
                                                           ROOM_ID,
                                                           ROOM_NAME,
                                                           CONSOLE_CODE,
                                                           CONSOLE_NAME,
                                                           EXPERT_FLAG,
                                                           dept_code,
                                                           dept_name,
                                                           waiting_count
                                                      FROM met_nuo_queue --门诊护士站分诊队列表

                                                     where doct_code = '{1}'
                                                       and trunc(queue_date) =
                                                           trunc(to_date('{0}', 'yyyy-mm-dd hh24:mi:ss'))
                                                       and noon_code = '{2}'
                                                       and dept_code='{3}'
                                                       and valid_flag = fun_get_valid", now.ToString("yyyy-MM-dd HH:mm:ss"), patient.Doct.ID, patient.Noon.ID, opr.DeptCode);//yhm 2021-03-19 此处该加上科室判断，因为五院发现会有医生同时出诊两个不同科室
                    dt = DataBaseHelp.DataExecHelp.GetDataTable(nurQueueNewSql2);
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.NurseCell.ID = dt.Rows[i][0].ToString();
                            patient.Queue.ID = dt.Rows[i][1].ToString();
                            patient.Queue.Name = dt.Rows[i][2].ToString();
                            break;
                        }
                        if (string.IsNullOrEmpty(patient.Queue.ID) || string.IsNullOrEmpty(patient.NurseCell.ID))
                        {
                            //resultCode = "0";
                            msg = "获取分诊队列信息出错！";
                            return ReturnFailure();
                        }
                    }
                    else
                    {
                        //this.resultCode = "0";
                        this.msg = "没有找到分诊队列信息！";
                        return this.ReturnFailure();
                    }
                }
                else
                {
                    //this.resultCode = "0";
                    this.msg = "没有找到分诊队列信息！";
                    return this.ReturnFailure();
                }
                #endregion

                #region 获取发票信息
                string realInvoice = string.Empty;
                string invoiceStr = string.Empty;
                dt = new System.Data.DataTable();
                invoicenoSql1 = string.Format(invoicenoSql1, OPERID, "1");
                dt = DataBaseHelp.DataExecHelp.GetDataTable(invoicenoSql1);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            patient.BeginInvoice = dt.Rows[i][0].ToString();
                            patient.EndInvoice = dt.Rows[i][1].ToString();
                            break;
                        }
                        returnStr = this.GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr);
                        if (!string.IsNullOrEmpty(returnStr))
                        {
                            return returnStr;
                        }

                        patient.RealInvoice = realInvoice;
                        patient.InvoiceStr = invoiceStr;
                        patient.IsUseingInvoice = true;
                    }
                    else
                    {
                        invoicenoSql1 = Sql.Sql.GetInvoiceInfoUsed;
                        invoicenoSql1 = string.Format(invoicenoSql1, OPERID, "0");
                        dt = DataBaseHelp.DataExecHelp.GetDataTable(invoicenoSql1);
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                if (!Convert.IsDBNull(dt.Rows[0][0]))
                                {
                                    patient.RealInvoice = dt.Rows[0][0].ToString();
                                    patient.BeginInvoice = dt.Rows[0][0].ToString();
                                    patient.EndInvoice = dt.Rows[0][1].ToString();
                                }
                                returnStr = this.GetInvoiceR(invoicenoSql2, now, ref realInvoice, ref invoiceStr);
                                if (!string.IsNullOrEmpty(returnStr))
                                {
                                    return returnStr;
                                }

                                patient.InvoiceStr = invoiceStr;
                                patient.IsUseingInvoice = false;
                            }
                            else
                            {
                                //this.resultCode = "0";
                                this.msg = "没有找到发票信息！";
                                return this.ReturnFailure();
                            }
                        }
                        else
                        {
                            // this.resultCode = "0";
                            this.msg = "没有找到发票信息！";
                            return this.ReturnFailure();
                        }
                    }
                    patient.NextRealInvoice = this.AddNumber(patient.RealInvoice);
                    patient.NextInvoiceStr = this.AddNumber(patient.InvoiceStr);
                }
                else
                {
                    //this.resultCode = "0";
                    this.msg = "没有找到发票信息！";
                    return this.ReturnFailure();
                }

                #endregion

                #region 获取门诊流水号
                if (string.IsNullOrEmpty(opr.ClincCode))
                {
                    opr.ClincCode = db.GetClinicCode();
                    if (opr.ClincCode == "-1" || string.IsNullOrEmpty(opr.ClincCode))
                    {
                        //this.resultCode = "0";
                        this.msg = "没有找到门诊流水号！";
                        return this.ReturnFailure();
                    }


                }

                patient.ClinicCode = opr.ClincCode;




                #endregion

                #region 获取门诊看诊次数

                dt = new System.Data.DataTable();
                intimesSql = string.Format(intimesSql, patient.CardNo);
                dt = DataBaseHelp.DataExecHelp.GetDataTable(intimesSql);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (!Convert.IsDBNull(dt.Rows[0][0]))
                        {
                            patient.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[0][0]);
                        }
                        else
                        {
                            //  this.resultCode = "0";
                            this.msg = "获取门诊看诊次数出错！";
                            return this.ReturnFailure();
                        }
                    }
                    else
                    {
                        //  this.resultCode = "0";
                        this.msg = "没有找到门诊看诊次数！";
                        return this.ReturnFailure();
                    }
                }
                else
                {
                    // this.resultCode = "0";
                    this.msg = "没有找到门诊看诊次数！";
                    return this.ReturnFailure();
                }

                #endregion

                #region 减免费用处理
                //string msg = string.Empty;
                if (!string.IsNullOrEmpty(opr.Payinsufeestr))
                {
                    if (Function.DualSIFeeInfo(opr.Payinsufeestr, ref patient, ref msg) == 0)
                    {
                        // this.resultCode = "0";
                        this.msg = "处理诊金减免出错！";
                        return this.ReturnFailure();
                    }
                }

                if (patient.Pact.ID == "258")
                {
                    patient.OwnDigFee = 0;
                }

                #endregion

                #endregion



                #region 更新号源
                //
                //string updateLmtSql = Sql.Sql.UpdateSchemaReged;
                //if (regRemainCount != -1)
                //{
                //    if (regRemainCount > 0)
                //    {
                //        

                //}


                string updateLmtSql = string.Format(Sql.Sql.UpdateSchemaReged, opr.RegSourceID, "1");

                int rt = mgr.ExecuteSql(updateLmtSql, ref msg);
                if (rt <= 0)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    if (string.IsNullOrEmpty(msg))
                        this.msg = "挂号失败，当前时段号源已被抢完，请选后一时段排班挂号";
                    return this.ReturnFailure();
                }


                #endregion

                #region 获取seeNo

                dt = new System.Data.DataTable();
                DataTable dt2 = new DataTable();
                /*  if (patient.SchemaType == "0")
                  {
                      //为科室排班
                      //seenoSql = string.Format(seenoSql, now.ToString("yyyy-MM-dd"), patient.Noon.ID, "", patient.Dept.ID, patient.SchemaType);
                      seenoSql = string.Format(getnewseeno, now.ToString("yyyy-MM-dd"), patient.Room.ID, "5", patient.Noon.ID);
                      dt = DataBaseHelp.DataExecHelp.GetDataTable(seenoSql);
                      if (dt == null || dt.Rows.Count <= 0 || Convert.IsDBNull(dt.Rows[0][0]))
                      {
                          seenoSql = string.Format(Sql.Sql.GetSeeNo, now.ToString("yyyy-MM-dd"), patient.Noon.ID, "", patient.Dept.ID, patient.SchemaType, patient.End.ToString());
                          dt = DataBaseHelp.DataExecHelp.GetDataTable(seenoSql);
                      }

                      if (dt != null)
                      {
                          if (dt.Rows.Count > 0)
                          {
                              if (!Convert.IsDBNull(dt.Rows[0][0]))
                              {
                                  patient.SeeNO = Neusoft.FrameWork.Function.NConvert.ToInt32(dt.Rows[0][0]);
                              }
                              else
                              {
                                  Shadow.Util.Data.Management.Trans.RollBack();
                                  //this.resultCode = "0";
                                  this.msg = "获取看诊序号出错！";
                                  return this.ReturnFailure();
                              }
                          }
                          else
                          {
                              Shadow.Util.Data.Management.Trans.RollBack();
                              //this.resultCode = "0";
                              this.msg = "没有找到看诊序号！";
                              return this.ReturnFailure();
                          }
                      }
                      else
                      {
                          Shadow.Util.Data.Management.Trans.RollBack();
                          //this.resultCode = "0";
                          this.msg = "没有找到看诊序号！";
                          return this.ReturnFailure();
                      }
                  }
                  else if (patient.SchemaType == "1")*/
                {
                    //为医生排班

                    //min最小看诊序号，seeNO当前看诊序号，cnt当前排班限额
                    int minNo = -1, seeNo = 0, cnt = 0 ,Residue = 0;

                    if (mgr.GetMinSeeNo(patient.SchemaID, ref minNo) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();//
                        msg = mgr.Err;
                        return this.ReturnFailure(); ;
                    }
                    if (mgr.GetCurrentSeeNo(patient.SchemaID, ref seeNo) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        msg = mgr.Err;
                        return this.ReturnFailure(); ;
                    }


                    if (minNo < 1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        msg = "取出最小看诊序号不正确，排班ID：" + patient.SchemaID.ToString();
                        return this.ReturnFailure();
                    }
                    if (mgr.GetSourceCount(patient.SchemaID, ref cnt) != -1)
                    {
                        mgr.GetResidue(patient.SchemaID, ref Residue);
                        if (Residue >= cnt)
                        {
                            Shadow.Util.Data.Management.Trans.RollBack();
                            msg = "已经没有足够号源可以，请选择其他时段排班";
                            return this.ReturnFailure();
                        }
                    }

                    if ((patient.RegLevel.ID != "4") && (seeNo == 0 || seeNo < minNo))//RegLevel.ID==4是急诊，seeNo==0为排班当天第一个挂号，seeNo<minNo 为上一时段未挂完的号，时段过了，则从下一个时段最小序号开始
                    {
                        seeNo = minNo;
                    }
                    else
                    {
                        seeNo = seeNo + 1;
                    }
                    patient.SeeNO = seeNo;
                    #region old
                    //                string lockSql = @"select min(a.seeno) seeNo
                    //                                      from fin_opr_schemalock a
                    //                                     where a.regsourceid = '{0}'
                    //                                       and a.cardno = '{1}' ";

                    //                seenoSql = string.Format(seenoSql, now.ToString("yyyy-MM-dd"), patient.Noon.ID, patient.Doct.ID, "", patient.SchemaType, patient.End.ToString());

                    //                try
                    //                {

                    //                   // lockSql = string.Format(lockSql, opr.RegSourceID, opr.CardNo);
                    //                    dt = DataBaseHelp.DataExecHelp.GetDataTable(seenoSql);
                    //                   // dt2 = DataBaseHelp.DataExecHelp.GetDataTable(lockSql);
                    //                }
                    //                catch
                    //                {
                    //                    His.Util.Common.HisLog.WriteLog("ZZSB", seenoSql + "$$$$$" + lockSql);
                    //                }

                    #endregion
                }

                //   int lockSeeNo = -1;
                //if (dt2 != null)
                //{
                //    if (dt2.Rows.Count > 0)
                //    {
                //        if (!Convert.IsDBNull(dt2.Rows[0][0]))
                //        {
                //            lockSeeNo = Neusoft.FrameWork.Function.NConvert.ToInt32(dt2.Rows[0][0]);
                //        }
                //    }
                //}
                //His.Util.Common.HisLog.WriteLog("SeeNo",patient.ClinicCode+"&lockNo:"+lockSeeNo.ToString()+"&Patient.SeeNO:"+patient.SeeNO.ToString());
                //if(lockSeeNo!=-1)
                //{
                //    if(Math.Abs(lockSeeNo-patient.SeeNO)<3)
                //    {
                //        patient.SeeNO = lockSeeNo;

                //    }
                //}
                //His.Util.Common.HisLog.WriteLog("SeeNo", patient.ClinicCode + "&SeeNo:" +patient.SeeNO.ToString());
                #endregion

                #region 更新排班表，插入号源表
                //插入挂号主表
                string insertReg = Sql.Sql.insertReg;
                //插入挂号费用表 挂号费
                string insertRegFee = Sql.Sql.insertRegFee;
                //插入挂号费用表 诊查费
                string insertDiagFee = Sql.Sql.insertRegFee;
                //插入护士分诊记录表
                string insertAssignRecord = Sql.Sql.insertAssignRecord;
                //插入交易记录表
                string InsertTradeRecords = Sql.Sql.InsertTradeRecords;
                //更新护士分诊队列表
                string updateNurQueue = Sql.Sql.updateNurQueues;
                //更新com_Dictionary发票信息
                string updatecomDictionarySql = Sql.Sql.updatecomDictionary;
                //更新占用状态
                string updateShemaLockState = Sql.Sql.UpdateRegLockState;
                //跟新看诊序号
                string setseeno = Sql.Sql.SetSeeNo;



                ArrayList sqlList = new ArrayList();
                if (patient.Doct.ID == "None")
                {
                    patient.Doct.ID = string.Empty;
                }


                #region 获取交易记录信息
                Models.ZZSB.TradeRecords recordsInfo = new His.Models.ZZSB.TradeRecords();
                recordsInfo.TranserNo = opr.ReqTraceNo;//交易流水号
                recordsInfo.INVOICE_NO = patient.InvoiceStr;//发票号
                recordsInfo.CLINIC_NO = patient.ClinicCode;//
                recordsInfo.CARDNO = patient.CardNo;//卡号
                recordsInfo.NAME = patient.Name;//姓名
                recordsInfo.ORDERID = opr.BankCardNo;//订单号或者银行卡卡号
                recordsInfo.PAY_TYPE = patient.PayType;//支付方式
                recordsInfo.TYPE = "1";//交易类型
                recordsInfo.TOT_COST = opr.PayAmt.ToString("0.00");//交易金额
                recordsInfo.DEVICEID = opr.DeviceID;//设备编号
                recordsInfo.REMARK = patient.SeeNO.ToString();//备注,挂号插入的是看诊序号
                recordsInfo.PACTCODE = patient.Pact.ID;//合同单位
                #endregion

                //查询GD表中合同单位 如果有值，就将patient.pact.id替换成GD表中合同单位
                string rPactCode = db.GetSiPactCodeForClinCode(opr.ClincCode);
                string rPactName = db.GetSiPactNameForClinCode(opr.ClincCode);
                string rPayKindCode = db.GetSiPayKindCodeForClinCode(opr.ClincCode);
                if (!string.IsNullOrEmpty(rPactCode) && !string.IsNullOrEmpty(rPactName) && !string.IsNullOrEmpty(rPayKindCode))
                {
                    patient.Pact.ID = rPactCode;
                    patient.Pact.Name = rPactName;
                    patient.Pact.PayKind.ID = rPayKindCode;
                }

                string[] argm = this.GetRegInfo(patient, opr.Triage_Serialnum, opr.InformedConsentResult);
                string[] regFeeArgm = this.GetRegFeeInfo(patient);
                string[] diagFeeArgm = Function.GetDiagFeeInfo(patient);// this.GetDiagFeeInfo(patient);
                string[] assignRecordArgm = this.GetAssignRecordInfo(patient);
                string[] tradeRecordsArgm = Function.GetTradeRecordsInfo(recordsInfo);

                insertReg = string.Format(insertReg, argm);
                insertRegFee = string.Format(insertRegFee, regFeeArgm);
                insertDiagFee = string.Format(insertDiagFee, diagFeeArgm);
                InsertTradeRecords = string.Format(InsertTradeRecords, tradeRecordsArgm);
                insertAssignRecord = string.Format(insertAssignRecord, assignRecordArgm);
                updateNurQueue = string.Format(updateNurQueue, patient.Queue.ID);
                updateShemaLockState = string.Format(updateShemaLockState, opr.TranSerNo, OPERID, "3");

                string InsertSISql = string.Empty;//处理诊金减免的sql
                string InsertGDSIinfo = string.Empty;//省集中平台的sql
                #region 医保减免

                //if (opr.Payinsufeestr.Length > 1)
                //{
                //    //省集中平台医保主表插入数据
                //    if (Function.getGDSIinfoSql(opr.Payinsufeestr, patient, ref msg, ref InsertGDSIinfo) == 0)
                //    {
                //        Shadow.Util.Data.Management.Trans.RollBack();
                //        return this.ReturnFailure();
                //    }
                //    else
                //    {
                //        sqlList.Add(InsertGDSIinfo);
                //    }

                //}

                #region 此处代码没什么卵用20180607by zhaoyiqiang
                if (patient.SchemaType == "0")//科室
                {
                    updateseeno = string.Format(updateseeno, now.ToString("yyyy-MM-dd"), patient.Room.ID, "5", patient.Noon.ID);
                    sqlList.Add(updateseeno);
                }
                else if (patient.SchemaType == "1" && patient.RegLevel.ID == "1")//普通医生
                {
                    updateseeno = string.Format(setseeno, now.ToString("yyyy-MM-dd"), "5", patient.Room.ID, patient.Noon.ID, patient.SeeNO);
                    sqlList.Add(updateseeno);
                }
                #endregion

                #endregion

                sqlList.Add(insertReg);//挂号主表
                sqlList.Add(insertRegFee);//挂号费插入fin_opb_accountcardfee
                sqlList.Add(insertDiagFee);//诊查费插入fin_opb_accountcardfee
                sqlList.Add(InsertTradeRecords);//交易记录表插入数据
                sqlList.Add(insertAssignRecord);//护士分诊记录表met_nuo_assignrecord
                //sqlList.Add(updateNurQueue);
                sqlList.Add(updateShemaLockState);//更新自助锁号表
                //长者券合同单位，需要插入一条诊查费到门诊费用表
                if (patient.Pact.ID == "258")
                {
                    //根据挂号级别获取需要插入的项目编码
                    string itemCode = string.Empty;
                    int ret =  db.getRegItemCode(patient.RegLevel.ID, ref itemCode);
                    if (ret < 0)
                    {
                        msg = "获取诊疗项目出错!";
                        return ReturnFailure();
                    }
                    string itemName = db.GetItemNameForItemCode(itemCode);
                    if (string.IsNullOrEmpty(itemName))
                    {
                        msg = "获取诊疗项目名称出错!";
                        return ReturnFailure();
                    }
                    string itemPrice = db.GetPriceForItemCode(itemCode);
                    if (string.IsNullOrEmpty(itemPrice))
                    {
                        msg = "获取诊疗项目价格出错!";
                        return ReturnFailure();
                    }
                    string insertRegFeeDetail = Sql.Sql.insertRegFeeDetail;
                    string[] regFeeDetail = this.GetRegFeeDetailInfo(patient,itemCode,itemName,itemPrice);
                    insertRegFeeDetail = string.Format(insertRegFeeDetail, regFeeDetail);
                    sqlList.Add(insertRegFeeDetail);
                }
                #region 处理发票

                if (patient.IsUseingInvoice)
                {
                    //使用在用的发票组
                    if (patient.EndInvoice == patient.NextRealInvoice)
                    {
                        //如果结束发票号=下一张发票号，说明该发票组已经用完了，更新使用标识为-1，并找到下一组发票更新使用标识为1，更新COM_DICTIONARY
                        string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                        string updateComInvoiceSql2 = Sql.Sql.updateComInvoiceNew;
                        string starInvoice = string.Empty;
                        string invoiceGetTime = string.Empty;
                        this.GetUnUseInvoice(ref starInvoice, ref invoiceGetTime);
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "-1");
                        //更新新发票组
                        updateComInvoiceSql2 = string.Format(updateComInvoiceSql2, OPERID, patient.RealInvoice, "1", invoiceGetTime);

                        updatecomDictionarySql = string.Format(updatecomDictionarySql, OPERID, starInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updateComInvoiceSql2);
                        sqlList.Add(updatecomDictionarySql);
                    }
                    else
                    {
                        string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                        //更新旧发票组
                        updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                        updatecomDictionarySql = string.Format(updatecomDictionarySql, OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                        sqlList.Add(updateComInvoiceSql1);
                        sqlList.Add(updatecomDictionarySql);

                    }
                }
                else
                {
                    string updateComInvoiceSql1 = Sql.Sql.updateComInvoice;
                    //更新旧发票组
                    updateComInvoiceSql1 = string.Format(updateComInvoiceSql1, OPERID, patient.BeginInvoice, patient.EndInvoice, patient.RealInvoice, "1");
                    updatecomDictionarySql = string.Format(updatecomDictionarySql, OPERID, patient.NextRealInvoice, patient.NextInvoiceStr);

                    sqlList.Add(updateComInvoiceSql1);
                    sqlList.Add(updatecomDictionarySql);
                }

                #endregion

                for (int i = 0; i < sqlList.Count; i++)
                {
                    if (mgr.ExecuteSql(sqlList[i].ToString(), ref msg) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        His.Util.Common.HisLog.WriteLog("ZZSB", "挂号失败，执行sql错误;\n" + sqlList[i].ToString());
                        msg = "挂号登记失败！" + msg;
                        return ReturnFailure();
                    }
                }

                //插入支付平台交易记录
                if (!string.IsNullOrEmpty(opr.ApplicationOrderNo) || !string.IsNullOrEmpty(opr.PlatformOrderNo))
                {
                    FinTransRecord payRecordInfo = new FinTransRecord();
                    payRecordInfo.Id = Guid.NewGuid().ToString();
                    payRecordInfo.TransactionNo = patient.InvoiceStr;
                    payRecordInfo.TransType = "1";
                    payRecordInfo.ClientCode = "ZDWY_ZZSB";
                    payRecordInfo.PlatformOrderNo = opr.PlatformOrderNo;
                    payRecordInfo.ApplicationOrderNo = opr.ApplicationOrderNo;
                    string PayChannelCode = "";
                    if (patient.PayType == "WX")
                    {
                        PayChannelCode = "WeChat_FKM";
                    }
                    else if (patient.PayType == "ZFB")
                    {
                        PayChannelCode = "ZFB_FKM";
                    }
                    else
                    {

                        Shadow.Util.Data.Management.Trans.RollBack();
                        msg = "挂号登记失败！" + "插入支付交易记录失败:支付方式不符合要求" + patient.PayType + "";
                        return ReturnFailure();

                    }
                    payRecordInfo.PayChannelCode = PayChannelCode;
                    payRecordInfo.TransAmount = patient.OwnDigFee;
                    payRecordInfo.OrderBigType = "3";
                    //payRecordInfo.OrderSmallType = "01";
                    payRecordInfo.PatientNo = patient.CardNo;
                    payRecordInfo.PatientName = patient.Name;
                    payRecordInfo.BusinessNo = patient.ClinicCode;
                    payRecordInfo.CreatedCode = "00W999";
                    payRecordInfo.CreatedName = "自助机";
                    payRecordInfo.HospitalCode = "H44040200001";

                    string strSql = @"
insert into FIN_Trans_RECORD(
                            id,
                            trans_type,
                            platform_order_no,
                            client_code,
                            application_order_no,
                            pay_channel_code,
                            pay_trans_finish_time,
                            TRANS_AMOUNT,
                            order_big_type,
                            order_small_type,
                            patient_no,
                            patient_name,
                            hospital_code,
                            created_code,
                            created_name,
                            transactionno，
                            businessno
                           ) 
values(
       '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', to_date('{6}','YYYY-MM-DD hh24:mi:ss'), 
       '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', 
       '{14}', '{15}','{16}'
      )";

                    string formattedSql = string.Format(strSql,
                        payRecordInfo.Id,
                        payRecordInfo.TransType,
                        payRecordInfo.PlatformOrderNo,
                        payRecordInfo.ClientCode,
                        payRecordInfo.ApplicationOrderNo,
                        payRecordInfo.PayChannelCode,
                        payRecordInfo.PayTransFinishTime,
                        payRecordInfo.TransAmount,
                        payRecordInfo.OrderBigType,
                        payRecordInfo.OrderSmallType,
                        payRecordInfo.PatientNo,
                        payRecordInfo.PatientName,
                        payRecordInfo.HospitalCode,
                        payRecordInfo.CreatedCode,
                        payRecordInfo.CreatedName,
                        payRecordInfo.TransactionNo,
                        payRecordInfo.BusinessNo
                        );
                    if (mgr.ExecNoQuery(formattedSql) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();

                        msg = "挂号登记失败！" + "插入支付交易记录失败！";
                        return ReturnFailure();

                    }
                }


                #region 支付平台发票绑定
                if (patient.PayType == "YBXYF")
                {
                    ZFPTService zfptSer = new ZFPTService();
                    His.Models.ZZSB.PayPlatform.InvoiceBinding invoiceBinding = new His.Models.ZZSB.PayPlatform.InvoiceBinding();
                    string zMsg = "";
                    invoiceBinding.invoiceNo = patient.InvoiceStr;//发票号
                    invoiceBinding.payorderId = opr.BankCardNo;
                    invoiceBinding.payMode = "1";
                    invoiceBinding.orderType = "1";
                    if (!zfptSer.ZFPTInvoiceBinding(invoiceBinding, ref zMsg))
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        His.Util.Common.HisLog.WriteLog("ZZSB", "挂号失败，绑定支付平台订单失败;\n" + zMsg);
                        msg = "挂号登记失败！" + zMsg;
                        return ReturnFailure();
                    }
                }
                #endregion
                //if (!DataBaseHelp.DataExecHelp.ExecArrayList(sqlList))
                //{
                //    //resultCode = "0";
                //    msg = "挂号登记失败！";
                //    return ReturnFailure();
                //}
                Shadow.Util.Data.Management.Trans.Commit();

                //记录挂号费用的操作。
                His.Util.Common.HisLog.WriteLog("提交挂号===========", insertDiagFee + "***************************" + insertRegFee);

                #region 返回串
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

                System.Xml.XmlElement TranSerNo = xml.CreateElement("TranSerNo");
                TranSerNo.InnerText = opr.TranSerNo;
                Result.AppendChild(TranSerNo);

                System.Xml.XmlElement TotalRegFee = xml.CreateElement("TotalRegFee");
                TotalRegFee.InnerText = (patient.RegFee + patient.PubDigFee + patient.OwnDigFee).ToString("0.00");
                Result.AppendChild(TotalRegFee);

                System.Xml.XmlElement RegFee = xml.CreateElement("RegFee");
                RegFee.InnerText = patient.RegFee.ToString("0.00");
                Result.AppendChild(RegFee);

                System.Xml.XmlElement TreatFee = xml.CreateElement("TreatFee");
                TreatFee.InnerText = patient.OwnDigFee.ToString("0.00");
                Result.AppendChild(TreatFee);

                System.Xml.XmlElement PatientBookFee = xml.CreateElement("PatientBookFee");
                PatientBookFee.InnerText = "0.00";
                Result.AppendChild(PatientBookFee);

                System.Xml.XmlElement ServicesFee = xml.CreateElement("ServicesFee");
                ServicesFee.InnerText = "0.00";
                Result.AppendChild(ServicesFee);

                System.Xml.XmlElement MetaFee = xml.CreateElement("MetaFee");
                MetaFee.InnerText = "0.00";
                Result.AppendChild(MetaFee);

                System.Xml.XmlElement OtherFee = xml.CreateElement("OtherFee");
                OtherFee.InnerText = "0.00";
                Result.AppendChild(OtherFee);

                System.Xml.XmlElement MedInsureFee = xml.CreateElement("MedInsureFee");
                MedInsureFee.InnerText = "0.00";
                Result.AppendChild(MedInsureFee);

                System.Xml.XmlElement PersonalFee = xml.CreateElement("PersonalFee");
                PersonalFee.InnerText = "0.00";
                Result.AppendChild(PersonalFee);

                System.Xml.XmlElement TreatLocation = xml.CreateElement("TreatLocation");
                TreatLocation.InnerText = "";
                Result.AppendChild(TreatLocation);

                System.Xml.XmlElement WaitTreatNo = xml.CreateElement("WaitTreatNo");
                WaitTreatNo.InnerText = "";
                Result.AppendChild(WaitTreatNo);

                System.Xml.XmlElement ReceiptNo = xml.CreateElement("ReceiptNo");
                ReceiptNo.InnerText = patient.InvoiceStr;
                Result.AppendChild(ReceiptNo);

                System.Xml.XmlElement SortNo = xml.CreateElement("SortNo");
                SortNo.InnerText = patient.SeeNO.ToString();
                Result.AppendChild(SortNo);

                System.Xml.XmlElement Note = xml.CreateElement("Note");
                Note.InnerText = patient.ClinicCode.ToString();
                Result.AppendChild(Note);

                returnStr = xml.InnerXml.ToString();
                #endregion

                return returnStr;
                #endregion
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Shadow.Util.Data.Management.Trans.RollBack();
                return this.ReturnFailure();

            }



        }

        public string GetRegisterElecInvoiceUrl(string req, string limitDays)
        {

            string sql = @"select d.pictureurl,p.reg_date,p.dept_name,p.invoice_no,p.reg_fee + p.diag_fee as fee,
fun_get_employee_name(p.see_docd) as see_doct_ame from fin_opr_register p,Elec_OutPatientRecord d where d.clinic_code = p.clinic_code and p.ynsee = '1' and p.trans_type = '1'
and  p.card_no='{0}' 
and  p.reg_date > sysdate - {1}
and not EXISTS (select clinic_code from fin_opr_register p1 where  p1.clinic_code = p.clinic_code and p1.trans_type = '2' ) order by p.reg_date desc";
            string xml = string.Empty;
            if (string.IsNullOrEmpty(req))
            {
                return Function.DataSource("0", "门诊号不能为空", req).ToString();
            }
            sql = string.Format(sql, req, limitDays);
            DataSet ds = new DataSet();
            ds = DataBaseHelp.DataExecHelp.GetDataSet(sql);
            if (ds == null)
                return Function.DataSource("0", "没有相关数据", req).ToString();
            System.Xml.Linq.XElement rt = new System.Xml.Linq.XElement("Result");
            if (ds.Tables[0].Rows.Count < 0 && ds != null)
            {
                return Function.DataSource("0", "没有相关数据", req).ToString();
            }
            else
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        System.Xml.Linq.XElement d = new System.Xml.Linq.XElement("URL");
                        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                            d.Add(new System.Xml.Linq.XElement(ds.Tables[0].Columns[i].ColumnName, row[i].ToString()));
                        rt.Add(d);
                    }

                }
                System.Xml.Linq.XElement source = Function.DataSource("1", string.Empty, "");
                source.Element("return").Add(rt);
                xml = source.ToString();
                return xml;
            }



        }

        private string ValidData(His.Models.ZZSB.OutPatientReg opr)
        {
            string returnStr = "";

            string sql = Sql.Sql.SelectRegLock;
            sql = string.Format(sql, opr.TranSerNo);
            System.Data.DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            string schemaID = string.Empty;
            string lockState = string.Empty;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        schemaID = dt.Rows[i][1].ToString();
                        lockState = dt.Rows[i][2].ToString();
                        break;
                    }
                    if (schemaID != opr.RegSourceID)
                    {
                        this.resultCode = "190302";
                        this.msg = "挂号排班和锁号排班信息不一致！";
                        return this.ReturnFailure();
                    }
                    if (lockState != "0")
                    {
                        this.resultCode = "190302";
                        this.msg = "锁号排班状态无效！";
                        return this.ReturnFailure();
                    }
                }
                else
                {
                    this.resultCode = "190302";
                    this.msg = "没有找到号源锁定信息！";
                    return this.ReturnFailure();
                }
            }
            else
            {
                this.resultCode = "190302";
                this.msg = "没有找到号源锁定信息！";
                return this.ReturnFailure();
            }

            return returnStr;
        }

        private DateTime GetSysDate()
        {
            string sql = Sql.Sql.GetSysDate;
            System.Data.DataTable dt = new System.Data.DataTable();
            DateTime now = new DateTime();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        now = Neusoft.FrameWork.Function.NConvert.ToDateTime(dt.Rows[0][0]);
                    }
                }
            }
            return now;
        }

        private string GetInvoiceCode(string operID)
        {
            string sql = Sql.Sql.GetInvoiceUserCode;
            sql = string.Format(sql, operID);
            System.Data.DataTable dt = new System.Data.DataTable();
            string userCode = string.Empty;
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        userCode = dt.Rows[0][0].ToString();
                    }
                }
            }
            return userCode;
        }

        private string GetInvoiceR(string sql, DateTime now, ref string realInvoice, ref string invoiceStr)
        {
            string returnStr = string.Empty;
            sql = string.Format(sql, OPERID);
            System.Data.DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        realInvoice = dt.Rows[i][0].ToString();
                        invoiceStr = dt.Rows[i][1].ToString();
                        break;
                    }
                    if (invoiceStr.Substring(0, 6) != now.ToString("yyMMdd"))
                    {
                        string userCode = this.GetInvoiceCode(OPERID);
                        invoiceStr = now.ToString("yyMMdd") + userCode + "0001";
                    }
                }
                else
                {
                    this.resultCode = "0";
                    this.msg = "没有找到发票信息！";
                    return this.ReturnFailure();
                }
            }
            else
            {
                this.resultCode = "0";
                this.msg = "没有找到发票信息！";
                return this.ReturnFailure();
            }

            return returnStr;
        }

        private string[] GetRegInfo(His.Models.ZZSB.ComPatient patient, string triage_serialnum, string informedConsentResult)
        {
            string[] argm = {
                               patient.ClinicCode, //门诊号/发票号
                               patient.CardNo, //就诊卡号
                               patient.Begin.ToString("yyyy-MM-dd HH:mm:ss"), //挂号日期
                               patient.Noon.ID, //午别
                               patient.Name, //姓名
                               patient.IDCard, //身份证号
                               patient.SexCode, //性别
                               patient.Birthday, //出生日
                               patient.Pact.PayKind.ID, //结算类别号
                               patient.Pact.PayKind.Name, //结算类别名称
                               patient.Pact.ID, //合同号
                               patient.Pact.Name, //合同单位名称
                               patient.McardNo, //医疗证号
                               patient.RegLevel.ID, //挂号级别
                               patient.RegLevel.Name, //挂号级别名称
                               patient.Dept.ID, //科室号
                               patient.Dept.Name, //科室名称
                               patient.SeeNO.ToString(), //看诊序号
                               patient.Doct.ID, //医师代号
                               patient.Doct.Name, //医师姓名
                               //"", //看诊日期
                               "1", //挂号收费标志
                               "0", //是否预约
                               "0", //1初诊/2复诊
                               patient.RegFee.ToString(), //挂号费
                               "0", //检查费
                               (patient.OwnDigFee+patient.PubDigFee-patient.RegFee).ToString(), //诊察费
                               "0", //附加费
                               (patient.RegFee + patient.OwnDigFee).ToString(), //自费金额
                               patient.PubDigFee.ToString(), //报销金额
                               "0", //自付金额
                               "1", //退号标志
                               OPERID, //操作员代码
                               "0", //是否看诊
                               "0", //1未核查/2已核查
                               patient.HomePhone, //联系电话
                               patient.Address, //地址
                               "1", //交易类型
                               "", //证件类型
                               patient.Begin.ToString("yyyy-MM-dd HH:mm:ss"), //开始时间段
                               patient.End.ToString("yyyy-MM-dd HH:mm:ss"), //结束时间段
                               "", //作废人
                               "", //作废时间
                               patient.InvoiceStr,//发票号
                               "",//处方号
                               "0",//是否加号
                               "",//每日顺序号
                               patient.SchemaID,//排班序号
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"), //操作时间
                               "",//患者来源
                               "0",//1：需要提取病案0：不需要提取病案
                               "0",//是否加密姓名
                               "",//密文
                               "",//优惠金额
                               "0",//账户流程标识1 账户挂号 0普通
                               "0",//是否急诊号
                               "",//扩展字段1
                               "",//56当前使用卡号
                               "",//57当前使用卡类型
                               patient.InTimes.ToString(),//58登记次数
                               "1",//患者类别（普通、VIP、特诊等） 常数PersonType
                               patient.RegNo,//诊金登记单号
                               (patient.OwnDigFee + patient.PubDigFee).ToString(),//诊金金额
                               patient.RegDiagCode, //诊金代码
                               "1",//分诊标志,0未分/1已分
                               OPERID,//分诊护士代码
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//分诊时间
                               "CORE_HIS50",
                               triage_serialnum,
                               informedConsentResult
                            };

            return argm;
        }

        private string[] GetRegFeeDetailInfo(His.Models.ZZSB.ComPatient patient,string itemCode,string itemName,string itemPrice)
        {
            string[] argm = {
                               db.GetOpbRecipeNoSequece(),
                               "1",
                               "1",
                               patient.ClinicCode,
                               patient.CardNo,
                               patient.Begin.ToString("yyyy-MM-dd HH:mm:ss"),
                               patient.Dept.ID,
                               patient.Doct.ID,
                               patient.Dept.ID,
                               itemCode,
                               itemName,
                               "0",
                               "次",
                               "015",
                               "U",
                               itemPrice,
                               "1",
                               "1",
                               "0",
                               "0",
                               "0",
                               "1",
                               "次",
                               "0",
                               "0",
                               itemPrice,
                               patient.Dept.ID,
                               patient.Dept.Name,
                               "0",
                               "00W999",
                               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                               "0",
                               "1",
                               "0",
                               "0",
                               "1",
                               "1",
                               "0",
                               db.GetMetMOOrderIDSequece(),
                               itemPrice,
                               "0",
                               "0",
                               "0",
                               "0",
                               "0",
                               db.GetBelongDeptCodeForEmplCode(patient.Doct.ID),//医生所属科室
                               "01",
                               patient.Pact.ID,
                               itemPrice,
                               "0",
                               db.GetBelongDeptCodeForEmplCode(patient.Doct.ID),//开立医生所属科室
                               "CORE_HIS50",
                               "NULL"
                            };

            return argm;
        }

        private string[] GetRegFeeInfo(His.Models.ZZSB.ComPatient patient)
        {
            string[] argm = {
                               patient.InvoiceStr,//发票
                               "1",//交易类型
                               patient.CardNo,//门诊卡号
                               patient.McardNo,//医疗证号
                               "",//身份标识卡类别 0无卡1磁卡 2IC卡
                               patient.RegFee.ToString(),//总额
                               OPERID,//收费人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//收费时间
                               OPERID,//操作人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//操作时间
                               "0",//0未日结/1已日结
                               "",//日结标识号
                               "",//日结人
                               "",//日结时间
                               "1",//‘0’ 无效 ‘1’ 有效,2退费
                               patient.RealInvoice,//实际发票打印号码
                               "3",//1=卡费用，2=病历本费用，3=挂号费，4=诊金，5=检查费，6=空调费
                               patient.ClinicCode,//病历号/门诊号
                               "",//备注
                               patient.RegFee.ToString(),//自费金额
                               "0",//报销金额
                               "0",//自付金额
                               "COMM"//支付方式
                            };

            return argm;
        }

        private string[] GetDiagFeeInfo(His.Models.ZZSB.ComPatient patient)
        {
            string[] argm = {
                               patient.InvoiceStr,//发票
                               "1",//交易类型
                               patient.CardNo,//门诊卡号
                               patient.McardNo,//医疗证号
                               "",//身份标识卡类别 0无卡1磁卡 2IC卡
                               patient.OwnDigFee.ToString(),//总额
                               OPERID,//收费人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//收费时间
                               OPERID,//操作人
                               patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),//操作时间
                               "0",//0未日结/1已日结
                               "",//日结标识号
                               "",//日结人
                               "",//日结时间
                               "1",//‘0’ 无效 ‘1’ 有效,2退费
                               patient.RealInvoice,//实际发票打印号码
                               "4",//1=卡费用，2=病历本费用，3=挂号费，4=诊金，5=检查费，6=空调费
                               patient.ClinicCode,//病历号/门诊号
                               "",//备注
                               "0",//自费金额
                               "0",//报销金额
                               patient.OwnDigFee.ToString(),//自付金额
                               "COMM"//支付方式
                            };

            return argm;
        }

        private string[] GetAssignRecordInfo(His.Models.ZZSB.ComPatient patient)
        {
            string[] argm = {
                                patient.ClinicCode,   //门诊号
                                patient.SeeNO.ToString(),   //看诊序号
                                patient.CardNo,   //病历号
                                patient.Begin.ToString("yyyy-MM-dd HH:mm:ss"),   //挂号日期
                                patient.Name,   //患者姓名
                                patient.SexCode,   //性别
                                "01",   //结算类别
                                "0",   //1急诊/0普通
                                "0",   //1预约/0普通
                                patient.Dept.ID,   //看诊科室
                                patient.Dept.Name,   //科室名称
                                patient.Queue.Name,   //队列名称
                                patient.Room.ID,   //出诊诊室
                                patient.Queue.ID,   //队列代码
                                patient.Room.Name,   //诊室名称
                                patient.Doct.ID,   //看诊医生
                                patient.RegDate.ToString("yyyy-MM-dd"),   //看诊时间
                                "1",   //1分诊/2进诊/3诊出
                                patient.NurseCell.ID,   //分诊科室
                                patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),   //分诊时间
                                "",   //进诊时间
                                "",   //出诊时间
                                OPERID,   //操作员
                                patient.RegDate.ToString("yyyy-MM-dd HH:mm:ss"),  //操作时间
                                patient.Console.ID,//诊台代码
                                patient.Console.Name,//诊台名称
                                patient.RegLevel.ID,// 挂号级别代码
                                patient.RegLevel.Name,//挂号级别
                                "" //每日顺序号
                            };

            return argm;
        }

        private string AddNumber(string number)
        {
            string returnNumber = string.Empty;
            string sql = Sql.Sql.addnumber;
            sql = string.Format(sql, number);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        returnNumber = dt.Rows[0][0].ToString();
                    }
                }
            }
            return returnNumber;
        }

        private void GetUnUseInvoice(ref string starInvoice, ref string invoiceGetTime)
        {
            string returnNumber = string.Empty;
            string sql = Sql.Sql.GetUnUseInvoce;
            sql = string.Format(sql, OPERID);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        invoiceGetTime = dt.Rows[0][0].ToString();
                        starInvoice = dt.Rows[0][1].ToString();
                    }
                }
            }
        }
    }
}
