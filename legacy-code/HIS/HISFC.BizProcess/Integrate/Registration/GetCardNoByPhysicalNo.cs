using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Integrate.Registration
{
    /// <summary>
    /// 根据物理卡号获取门诊病历号
    /// </summary>
    public class GetCardNoByPhysicalNo : Neusoft.HISFC.BizProcess.Interface.Registration.IGetCardNOByInputCardNO
    {
        #region 变量

        /// <summary>
        /// 门诊费用业务类
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();

        /// <summary>
        /// 入出转
        /// </summary>
        private RADT radtManager = new RADT();

        /// <summary>
        /// 费用综合管理
        /// </summary>
        Fee feeMgr = new Fee();

        /// <summary>
        /// 拼音码管理
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Spell spellMgr = new Neusoft.HISFC.BizLogic.Manager.Spell();

        /// <summary>
        /// 门诊挂号管理
        /// </summary>
        private Neusoft.HISFC.BizLogic.Registration.Register regMgr = new Neusoft.HISFC.BizLogic.Registration.Register();

        /// <summary>
        /// 门诊账户管理
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.Account accountManager = new Neusoft.HISFC.BizLogic.Fee.Account();

        /// <summary>
        /// 患者信息
        /// </summary>
        Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = null;

        private Manager manager = new Manager();

        /// <summary>
        /// 患者卡类型列表
        /// </summary>
        private static ArrayList cardTypeList = null;

        /// <summary>
        /// 常数管理
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Constant conMgr = new Neusoft.HISFC.BizLogic.Manager.Constant();

        #endregion

        #region IGetCardNOByInputCardNO 成员

        /// <summary>
        /// 根据输入的卡号判断患者的主索引和物理卡类型和患者主索引
        /// </summary>
        /// <param name="inputCardNO">输入的物理卡号</param>
        /// <param name="cardNO">患者主索引：门诊病历号</param>
        /// <param name="physicalTypeID">卡类型和关键字可以自行维护；常数类型为：CardType</param>
        /// <param name="errText">错误提示</param>
        /// <returns></returns>
        public int GetCardNOByInputCardNO(string inputCardNO, ref string cardNO, ref string cardType, ref string errText)
        {
            /* 规则：
             * 1、取输入字符串的第一个字符
             * 2、根据常数判断是哪个类型的卡
             * 3、根据卡类型，从对照表获得门诊病历号
             * 4、院内职工如果没有对照信息，可以设置为自动创建卡号
             * */

            //判断返回值
            int returnValue = 0;

            Neusoft.FrameWork.Models.NeuObject cardTypeObj=new Neusoft.FrameWork.Models.NeuObject();

            string realCardNo = inputCardNO;
            returnValue = this.GetCardType(inputCardNO, ref cardTypeObj, ref errText, ref realCardNo);
            inputCardNO = realCardNo;

            //异常出错
            if (returnValue == -1)
            {
                return -1;
            }
            //不在维护类型之内的，都按照直接输入病历号处理
            else if (returnValue == 0)
            {
                cardNO = realCardNo;

                return 1;
            }
            //已维护卡类型的
            else
            {
                patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

                //门诊账户实体
                Neusoft.HISFC.Models.Account.AccountCard accountCardObj = new Neusoft.HISFC.Models.Account.AccountCard();

                try
                {
                    //根据规则获取病历号
                    accountCardObj.MarkType = cardTypeObj;
                    returnValue = feeMgr.ValidMarkNO(inputCardNO, ref accountCardObj);

                    //院内职工，即使没有维护，也要自动生成
                    if (returnValue <= 0)
                    {
                        //暂时这么处理，也可以维护COM_CONTROLARGUMENT
                        if (accountCardObj.MarkType.Name.Contains("员工"))
                        {
                            Neusoft.HISFC.Models.Base.Employee empl = this.manager.GetEmployeeInfo(inputCardNO.PadLeft(6, '0'));
                            if (empl == null || string.IsNullOrEmpty(empl.ID))
                            {
                                errText = "没有找到该编号的职工！";
                                return -1;
                            }

                            //员工卡找不到的话，可以再从身份证找一次
                            #region 不按照身份证找了，没有规则

                            ////accountCardObj.MarkType.ID=？？
                            //returnValue = feeMgr.ValidMarkNO(empl.IDCard, ref accountCardObj);

                            //if (returnValue == 1)
                            //{
                            //    cardNO = accountCardObj.Patient.PID.CardNO;
                            //    cardType = accountCardObj.MarkType.ID;
                            //}
                            //else
                            //{
                            //    cardType = "4";//员工卡

                            //    returnValue = regMgr.AutoGetCardNO();

                            //    if (returnValue == -1)
                            //    {
                            //        errText = "未能成功自动产生卡号，请联系信息科！";
                            //    }
                            //    cardNO = returnValue.ToString().PadLeft(10, '0');

                            //    return ResetPatientInfo(empl, cardNO, ref errText);
                            //}
                            #endregion

                            //员工卡自动生成病历号
                            returnValue = regMgr.AutoGetCardNO();

                            if (returnValue == -1)
                            {
                                errText = "未能成功自动产生卡号，请联系信息科！";
                            }
                            cardNO = returnValue.ToString().PadLeft(10, '0');

                            return ResetPatientInfo(empl, cardNO, ref errText, accountCardObj.MarkType);
                        }
                        else
                        {
                            errText = feeMgr.Err;
                            return -1;
                        }
                    }
                    else
                    {
                        cardNO = accountCardObj.Patient.PID.CardNO;
                        cardType = accountCardObj.MarkType.ID;
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    errText = "没有患者信息或输入的信息不正确,请重试" + ex.Message;
                    return -1;
                }
            }
        }

        /// <summary>
        /// 获取卡类型
        /// </summary>
        /// <param name="inputCardNO">输入的卡信息</param>
        /// <param name="cardType">卡类型</param>
        /// <param name="errText">错误信息</param>
        /// <param name="realCardNo">真实的卡号，去掉了关键字</param>
        /// <returns></returns>
        private int GetCardType(string inputCardNO, ref Neusoft.FrameWork.Models.NeuObject cardTypeObj, ref string errText, ref string realCardNo)
        {
            try
            {
                //卡类型常数
                if (cardTypeList == null)
                {
                    cardTypeList = this.conMgr.GetList("CardType");
                    if (cardTypeList == null)
                    {
                        errText = "获取卡类型出错：" + conMgr.Err;
                        return -1;
                    }
                }

                //比较输入内容的第一个字符
                string keyWord = inputCardNO.Substring(0, 1);

                foreach (Neusoft.FrameWork.Models.NeuObject conObj in cardTypeList)
                {
                    if (conObj.Memo.Contains(keyWord))
                    {
                        cardTypeObj.ID = conObj.ID;
                        cardTypeObj.Name = conObj.Name;
                        realCardNo = inputCardNO.Substring(1);

                        if (cardTypeObj.Name.Contains("员工"))
                        {
                            realCardNo = realCardNo.PadLeft(6, '0');
                        }

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                errText = ex.Message;
                cardTypeObj = null;
                realCardNo = inputCardNO;
                return -1;
            }

            realCardNo = inputCardNO;
            return 0;
        }

        /// <summary>
        /// 根据职工信息保存就诊卡信息和患者基本信息
        /// </summary>
        /// <param name="empl"></param>
        /// <param name="cardno"></param>
        /// <param name="errtxt"></param>
        /// <returns></returns>
        private int ResetPatientInfo(Neusoft.HISFC.Models.Base.Employee empl, string cardno, ref string errtxt,Neusoft.FrameWork.Models.NeuObject cardTypeObj)
        {
            if (empl == null)
            {
                errtxt = "员工实体为空!";
                return -1;
            }

            if (string.IsNullOrEmpty(cardno))
            {
                errtxt = "员工卡获取病历号信息出错!";
                return -1;
            }

            patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

            patientInfo = ConvertEmplToPatient(empl);

            if (patientInfo == null)
            {
                errtxt = "员工信息转换为患者实体时出现异常";
                return -1;
            }
            patientInfo.PID.CardNO = cardno;

            Neusoft.HISFC.Models.Account.AccountCard accountCardObj = new Neusoft.HISFC.Models.Account.AccountCard();
            accountCardObj.Patient.PID.CardNO = cardno;
            accountCardObj.MarkNO = empl.ID;
            accountCardObj.MarkType = cardTypeObj;

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            if (accountManager.InsertAccountCard(accountCardObj) == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                errtxt = "保存员工卡记录失败！" + accountManager.Err;
                return -1;
            }

            if (radtManager.InsertPatientInfo(patientInfo) == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                errtxt = "保存员工就诊信息失败！" + accountManager.Err;
                return -1;
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            return 1;
        }

        /// <summary>
        /// 员工信息转换为患者信息实体
        /// </summary>
        /// <param name="empl"></param>
        /// <returns></returns>
        private Neusoft.HISFC.Models.RADT.PatientInfo ConvertEmplToPatient(Neusoft.HISFC.Models.Base.Employee empl)
        {
            patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

            patientInfo.Name = empl.Name;
            patientInfo.Sex.ID = empl.Sex.ID;
            patientInfo.Birthday = empl.Birthday;
            patientInfo.IDCard = empl.IDCard;
            Neusoft.HISFC.Models.Base.ISpell sp = spellMgr.Get(empl.Name);
            patientInfo.SpellCode = sp.SpellCode;
            patientInfo.WBCode = sp.WBCode;

            return patientInfo;
        }

        #endregion
    }
}
