using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.HISFC.Models.Fee.Outpatient;

namespace Neusoft.HISFC.BizProcess.Integrate.AccountFee
{
    /// <summary>
    /// 预交金流程，终端扣费管理类
    /// {42CDFA33-9FE5-42b0-BBC5-533922960DE8}
    /// </summary>
    public class OutPatientFeeManage : IntegrateBase
    {
        #region 变量

        /// <summary>
        /// 预交金流程，终端扣费发票管理类
        /// </summary>
        OutPatientInvoiceManage invoiceManager = new OutPatientInvoiceManage();

        /// <summary>
        /// 费用管理类
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Fee feeManager = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        /// <summary>
        /// 本地医疗待遇接口
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();
        
        #endregion

        /// <summary>
        /// 预交金流程收费操作
        /// </summary>
        /// <param name="r">病人挂号信息</param>
        /// <param name="feeItemList">费用明细信息</param>
        /// <param name="strMsg">提示信息</param>
        /// <returns> -1 失败，0 账户余额不足，1 成功扣费 </returns>
        public int ChargeFee(Neusoft.HISFC.Models.Registration.Register r, ArrayList feeItemList, out string strMsg)
        {
            strMsg = "";
            int lngRes = 1;
            if (r == null || string.IsNullOrEmpty(r.ID) || string.IsNullOrEmpty(r.Pact.ID))
            {
                strMsg = "患者信息为空！";
                return -1;
            }

            // {9635BF11-D633-409e-8880-2DB29CB830F7}
            if (Neusoft.HISFC.BizProcess.Integrate.AccountFee.Function.LstUnTerminalPactCode.Contains(r.Pact.ID))
            {
                lngRes = -1;
                strMsg = r.Pact.Name + " 身份病人，请到收费处收费！";
                return lngRes;
            }

            if (feeItemList == null || feeItemList.Count <= 0)
            {
                strMsg = "患者费用信息为空！";
                return 1;
            }

            Neusoft.HISFC.Models.Base.Employee employee = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;
            // 医生站自助扣费
            // 指定一个固定终端员工
            if (!employee.ID.StartsWith("T"))
            {
                // 系统必须定义一个 T00001 的员工 为医生站扣费时分配发票用
                employee = new Neusoft.HISFC.Models.Base.Employee();
                employee.ID = "T00001"; // 终端全院
                employee.Name = "T-全院";
                employee.UserCode = "99";
            }

            if (this.trans == null)
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            }

            invoiceManager.SetTrans(this.trans);
            feeManager.SetTrans(this.trans);

            medcareProxy.BeginTranscation();
            medcareProxy.SetPactCode(r.Pact.ID);
            medcareProxy.IsLocalProcess = true;

            if (!medcareProxy.IsInBlackList(r))
            {
                lngRes = medcareProxy.LocalBalanceOutpatient(r, ref feeItemList, null);
                if (lngRes <= 0)
                {
                    strMsg = medcareProxy.ErrMsg;
                    //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    return lngRes;
                }
            }
            else
            {
                // 特殊处理
                // {9832026E-02FE-4118-A3F5-51C20E79742B}
                if (Function.HospitalCode == "A-19")
                {
                    // 南庄医院特殊处理 -- 老年减免医保-6 第二次报销时按 老年减免自费-7 减免
                    switch (r.Pact.ID)
                    {
                        case "6":
                            r.Pact.ID = "7";
                            medcareProxy.SetPactCode(r.Pact.ID);
                            medcareProxy.IsLocalProcess = true;
                            lngRes = medcareProxy.LocalBalanceOutpatient(r, ref feeItemList, null);
                            if (lngRes <= 0)
                            {
                                strMsg = medcareProxy.ErrMsg;
                                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                return lngRes;
                            }

                            r.Pact.ID = "6";
                            break;

                        default:
                            break;
                    }
                }
            }


            // 生成发票信息
            Balance invoiceInfo = null;
            List<BalanceList> lstInvoiceDetial = null;

            List<FeeItemList> lstFeeItem = new List<FeeItemList>();
            lstFeeItem.AddRange((FeeItemList[])feeItemList.ToArray(typeof(FeeItemList)));

            lngRes = invoiceManager.BuildInvoiceInfo(employee, r, lstFeeItem, out invoiceInfo, out lstInvoiceDetial, out strMsg);
            if (lngRes <= 0 || invoiceInfo == null || lstInvoiceDetial == null || lstInvoiceDetial.Count <= 0)
            {                
                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }

            // 生成支付方式信息
            List<BalancePay> lstPayModes = invoiceManager.MakeInvoicePayModes(invoiceInfo, ref strMsg);
            if (lstPayModes == null || lstPayModes.Count <= 0)
            {
                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return -1;
            }

            bool blnRes = false;
            // 
            ArrayList arlInvoices = new ArrayList();
            ArrayList arlInvoiceDetial = new ArrayList();
            ArrayList arlPayModes = new ArrayList();
            ArrayList arlFeeItem = new ArrayList();
            ArrayList arlTemp = new ArrayList();

            // 发票主信息
            arlInvoices.Add(invoiceInfo);
            // 发票明细信息
            arlTemp.AddRange(lstInvoiceDetial.ToArray());
            ArrayList arlTemp2 = new ArrayList();
            arlTemp2.Add(arlTemp);
            arlInvoiceDetial.Add(arlTemp2);
            // 支付方式
            arlPayModes.AddRange(lstPayModes.ToArray());
            // 费用明细
            foreach (FeeItemList item in lstFeeItem)
            {
                // 设置为帐户扣费
                item.IsAccounted = true;
            }
            arlFeeItem.AddRange(lstFeeItem.ToArray());

            strMsg = "";
            blnRes = feeManager.ClinicFee(Neusoft.HISFC.Models.Base.ChargeTypes.Fee, "C", true, r, arlInvoices, arlInvoiceDetial, arlFeeItem, new ArrayList(), arlPayModes, ref strMsg, employee);

            this.Err = strMsg;
            if (!blnRes)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
