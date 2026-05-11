using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.HISFC.Models.Registration;
using System.Data;
using Neusoft.FrameWork.Function;
using System.Windows.Forms;
using System.Collections;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.BizProcess.Integrate.AccountFee
{
    /// <summary>
    /// 预交金流程，终端扣费发票管理类
    /// {C1AB00E1-8278-4037-8F53-C3A6876FBBBD}
    /// </summary>
    public class OutPatientInvoiceManage : IntegrateBase
    {
        public OutPatientInvoiceManage()
        {
            ArrayList alPayModes = this.managerIntegrate.GetConstantList(Neusoft.HISFC.Models.Base.EnumConstant.PAYMODES);
            if (alPayModes == null || alPayModes.Count <= 0)
            {
                this.Err = "获取支付方式错误";
            }
            else
            {
                helpPayMode.ArrayObject = alPayModes;
            }
        }

        /// <summary>
        /// 生成发票信息。
        /// 发票信息、发票明细信息
        /// </summary>
        /// <param name="register">挂号信息</param>
        /// <param name="lstFeeItems">费用明细信息</param>
        /// <param name="invoiceInfo">发票信息</param>
        /// <param name="lstInvoiceDetial">发票明细信息</param>
        /// <param name="errText">错误信息</param>
        /// <returns>1 成功，-1 失败</returns>
        public int BuildInvoiceInfo(Employee employee, Register register, List<FeeItemList> lstFeeItems, out Balance invoiceInfo, out List<BalanceList> lstInvoiceDetial, out string errText)
        {
            invoiceInfo = null;
            lstInvoiceDetial = null;
            errText = "";

            #region 获取发票号与打印发票号

            string invoiceNO = "";
            string realInvoiceNO = "";

            int iRes = feeIntegrate.GetInvoiceNO(employee, "C", true, ref invoiceNO, ref realInvoiceNO, ref errText);
            if (iRes <= 0)
            {
                return iRes;
            }

            #endregion

            #region 获取发票大类
            DataSet dsInvoice = new DataSet();

            string invoicePrintDll = controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.INVOICEPRINT, false, string.Empty);
            if (string.IsNullOrEmpty(invoicePrintDll))
            {
                errText = "没有维护发票打印方案!请维护";

                return -1;
            }

            invoicePrintDll = System.Windows.Forms.Application.StartupPath + invoicePrintDll;
            Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint iInvoicePrint = null;
            object obj = null;
            System.Reflection.Assembly a = System.Reflection.Assembly.LoadFrom(invoicePrintDll);
            try
            {
                System.Type[] types = a.GetTypes();
                foreach (System.Type type in types)
                {
                    if (type.GetInterface("IInvoicePrint") != null)
                    {
                        try
                        {
                            obj = System.Activator.CreateInstance(type);
                            iInvoicePrint = obj as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint;

                            break;
                        }
                        catch (Exception ex)
                        {
                            errText = ex.Message;

                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errText = ex.Message;
                return -1;
            }
            iInvoicePrint.Register = register;
            feeIntegrate.GetInvoiceClass(iInvoicePrint.InvoiceType, ref dsInvoice);
            if (dsInvoice == null || dsInvoice.Tables.Count <= 0)
            {
                errText = "没有维护发票大类!请维护";
                return -1;
            }
            if (dsInvoice.Tables[0].PrimaryKey.Length == 0)
            {
                dsInvoice.Tables[0].PrimaryKey = new DataColumn[] { dsInvoice.Tables[0].Columns["FEE_CODE"] };
            }
            #endregion

            invoiceInfo = MakeInvoiceInfo(lstFeeItems, register, invoiceNO, realInvoiceNO);
            if (invoiceInfo == null)
            {
                errText = "生成发票信息失败！";
                return -1;
            }

            lstInvoiceDetial = MakeInvoiceDetail(lstFeeItems, register, invoiceNO, dsInvoice.Tables[0], ref errText);
            if (lstInvoiceDetial == null)
            {
                return -1;
            }

            return 1;
        }


        #region 生成主发票信息
        /// <summary>
        /// 生成主发票信息
        /// </summary>
        /// <param name="feeItemLists">费用明细集合</param>
        /// <param name="register">挂号信息</param>
        /// <param name="invoiceNO">发票号</param>
        /// <param name="realInvoiceNO">实际发票号</param>
        /// <returns></returns>
        private Balance MakeInvoiceInfo(List<FeeItemList> lstFeeItems, Register register, string invoiceNO, string realInvoiceNO)
        {
            Balance invoice = new Balance();
            decimal totCost = 0;//总金额
            decimal ownCost = 0;//自费金额
            decimal pubCost = 0;//记账金额
            decimal payCost = 0;//自付金额
            decimal rebateCost = 0;//优惠价格

            foreach (FeeItemList feeItem in lstFeeItems)
            {
                ownCost += feeItem.FT.OwnCost;
                pubCost += feeItem.FT.PubCost;
                payCost += feeItem.FT.PayCost;
                rebateCost += feeItem.FT.RebateCost;
            }
            totCost = ownCost + pubCost + payCost;

            invoice.Invoice.ID = invoiceNO;
            invoice.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
            invoice.Patient = register.Clone();

            invoice.FT.OwnCost = ownCost;
            invoice.FT.PayCost = payCost;
            invoice.FT.PubCost = pubCost;
            invoice.FT.TotCost = totCost;
            invoice.FT.RebateCost = rebateCost;
            invoice.User01 = rebateCost.ToString();

            if (string.IsNullOrEmpty(register.ChkKind))
            {
                invoice.ExamineFlag = "0";
            }
            else
            {
                invoice.ExamineFlag = register.ChkKind;
            }

            invoice.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
            invoice.CanceledInvoiceNO = "";
            invoice.IsDayBalanced = false;
            invoice.PrintTime = DateTime.Now;
            invoice.PrintedInvoiceNO = realInvoiceNO;
            invoice.IsAccount = true;

            return invoice;
        } 
        #endregion

        #region 生成发票明细
        /// <summary>
        /// 生成发票明细
        /// </summary>
        /// <param name="feeItemLists">费用明细集合</param>
        /// <param name="register">挂号信息</param>
        /// <param name="invoiceNO">发票号</param>
        /// <param name="dtInvoice">发票大类</param>
        /// <param name="errText">错误信息</param>
        /// <returns>成功: 发票明细集合 失败 null</returns>
        private List<BalanceList> MakeInvoiceDetail(List<FeeItemList> lstFeeItems, Register register, string invoiceNO, DataTable dtInvoice, ref string errText)
        {
            List<BalanceList> balanceLists = new List<BalanceList>();

            foreach (FeeItemList f in lstFeeItems)
            {
                DataRow rowFind = dtInvoice.Rows.Find(new object[] { f.Item.MinFee.ID });
                if (rowFind == null)
                {
                    errText = "最小费用为" + f.Item.MinFee.ID + "的最小费用没有在MZ01的发票大类中维护";
                    return null;
                }

                rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.OwnCost + f.FT.PubCost + f.FT.PayCost;
                rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + f.FT.OwnCost;
                rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
            }

            BalanceList detail = null;//发票明细实体

            for (int i = 1; i < 100; i++)
            {
                //找到相同打印序号,即同一统计类别的费用集合
                DataRow[] rowFind = dtInvoice.Select("SEQ = " + i.ToString(), "SEQ ASC");
                //如果没有找到说明已经找过了最大的打印序号,所有费用已经累加完毕.
                if (rowFind.Length == 0)
                {

                }
                else
                {
                    detail = new BalanceList();
                    detail.BalanceBase.Invoice.ID = invoiceNO;
                    detail.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                    detail.InvoiceSquence = i;
                    detail.FeeCodeStat.ID = rowFind[0]["FEE_STAT_CATE"].ToString();
                    detail.FeeCodeStat.Name = rowFind[0]["FEE_STAT_NAME"].ToString();

                    /// 保存打印序号到实体。
                    ///----------------------------------------------------
                    detail.FeeCodeStat.SortID = NConvert.ToInt32(rowFind[0]["SEQ"].ToString());
                    ///---------------------------------------------------
                    detail.BalanceBase.IsDayBalanced = false;
                    detail.BalanceBase.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                    foreach (DataRow row in rowFind)
                    {
                        detail.BalanceBase.FT.PubCost += NConvert.ToDecimal(row["PUB_COST"].ToString());
                        detail.BalanceBase.FT.OwnCost += NConvert.ToDecimal(row["OWN_COST"].ToString());
                        detail.BalanceBase.FT.PayCost += NConvert.ToDecimal(row["PAY_COST"].ToString());
                    }
                    detail.BalanceBase.FT.TotCost = detail.BalanceBase.FT.PubCost + detail.BalanceBase.FT.OwnCost + detail.BalanceBase.FT.PayCost;
                    //如果费用为0 说明次统计类别(打印序号)下没有费用
                    if (detail.BalanceBase.FT.TotCost <= 0)
                    {
                        continue;
                    }

                    balanceLists.Add(detail);
                }
            }

            return balanceLists;
        } 
        #endregion

        #region 生成发票支付方式
        /// <summary>
        /// 生成发票支付方式
        /// 门诊账户支付
        /// </summary>
        /// <param name="invoiceInfo">发票信息</param>
        /// <param name="errText">错误信息</param>
        /// <returns></returns>
        public List<BalancePay> MakeInvoicePayModes(Balance invoiceInfo, ref string errText)
        {
            List<BalancePay> lstBalancePay = new List<BalancePay>();
            BalancePay payMode = null;
            if (invoiceInfo.FT.RebateCost > 0)
            {
                // 减免支付（优惠金额、二次减免、老年减免）
                payMode = new BalancePay();
                payMode.PayType.ID = "RC";
                payMode.PayType.Name = helpPayMode.GetName(payMode.PayType.ID);
                payMode.FT.TotCost = invoiceInfo.FT.RebateCost;

                lstBalancePay.Add(payMode);
            }
            if (invoiceInfo.FT.OwnCost > invoiceInfo.FT.RebateCost)
            {
                // 门诊账户支付，系统固定为“YS”
                payMode = new BalancePay();
                payMode.PayType.ID = "YS";
                payMode.PayType.Name = helpPayMode.GetName(payMode.PayType.ID);
                payMode.FT.TotCost = invoiceInfo.FT.OwnCost - invoiceInfo.FT.RebateCost;

                lstBalancePay.Add(payMode);
            }

            return lstBalancePay;
        }

        #endregion


        #region 成员

        /// <summary>
        /// 参数控制类
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

        Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// 门诊费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();

        /// <summary>
        /// 管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// payMode列表查询
        /// </summary>
        protected Neusoft.FrameWork.Public.ObjectHelper helpPayMode = new Neusoft.FrameWork.Public.ObjectHelper();

        #endregion

    }
}
