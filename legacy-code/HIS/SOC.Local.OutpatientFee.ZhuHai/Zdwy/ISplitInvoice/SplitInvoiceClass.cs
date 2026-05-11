using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.HISFC.Models.Fee.Outpatient;
using System.Data;
using Neusoft.HISFC.Models.Registration;
using Neusoft.FrameWork.Function;
using System.Windows.Forms;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.ISplitInvoice
{
    class SplitInvoiceClass : Neusoft.HISFC.BizProcess.Interface.FeeInterface.ISplitInvoice
    {
        /// <summary>
        /// 门诊业务层
        /// </summary>
        public Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();

        #region 形成门诊发票

        /// <summary>
        /// 生成主发票信息
        /// </summary>
        /// <param name="feeItemLists">费用明细集合</param>
        /// <param name="register">挂号信息</param>
        /// <param name="invoiceNO">发票号</param>
        /// <param name="realInvoiceNO">实际发票号</param>
        /// <param name="invoiceFlag">发票类别</param>
        /// <param name="splitFlag">分发票标志</param>
        /// <returns></returns>
        private Balance MakeInvoiceInfo(ArrayList feeItemLists, Register register, string invoiceNO,
            string realInvoiceNO, string invoiceFlag, string splitFlag)
        {
            Balance invoice = new Balance();
            decimal totCost = 0;//总金额
            decimal ownCost = 0;//自费金额
            decimal pubCost = 0;//记账金额
            decimal payCost = 0;//自付金额

            decimal rebateCost = 0;//优惠价格 by niuxy

            if (invoiceFlag == "1")//自费发票
            {
                foreach (FeeItemList f in feeItemLists)
                {
                    ownCost += f.FT.OwnCost;
                    payCost += f.FT.PayCost;
                    pubCost += f.FT.PubCost;
                    rebateCost += f.FT.RebateCost;
                }
                totCost = ownCost + payCost + pubCost;
            }
            else if (invoiceFlag == "5")//所有发票,如果是公费患者这里要写算法.
            {
                foreach (FeeItemList f in feeItemLists)
                {
                    payCost += f.FT.PayCost;
                    ownCost += f.FT.OwnCost;
                    pubCost += f.FT.PubCost;

                    rebateCost += f.FT.RebateCost;
                }
                totCost = ownCost + payCost + pubCost;
            }
            #region 给明细发票号赋值

            foreach (FeeItemList f in feeItemLists)
            {
                f.Invoice.ID = invoiceNO;
            }

            #endregion

            invoice.Invoice.ID = invoiceNO;
            invoice.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
            invoice.Patient = register.Clone();

            invoice.FT.OwnCost = ownCost;
            invoice.FT.PayCost = payCost;
            invoice.FT.PubCost = pubCost;
            invoice.FT.TotCost = totCost;
            invoice.FT.RebateCost = rebateCost;
            
            string tempExamineFlag = null;
            if (register.ChkKind.Length > 0)
            {
                tempExamineFlag = register.ChkKind;
            }
            else
            {
                tempExamineFlag = "0";
            }
            invoice.ExamineFlag = tempExamineFlag;
            invoice.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
            invoice.CanceledInvoiceNO = "";
            invoice.IsDayBalanced = false;
            invoice.Memo = invoiceFlag;
            invoice.PrintTime = outpatientManager.GetDateTimeFromSysDateTime() ;
            invoice.PrintedInvoiceNO = realInvoiceNO;
            if (!string.IsNullOrEmpty(register.LSH))
            {
                invoice.User02 = register.LSH;
            }
            if (!string.IsNullOrEmpty(register.User03))
            {
                invoice.User03 = register.User03;
            }
            return invoice;
        }

        /// <summary>
        /// 清空发票统计大类的费用,供下一张发票使用.
        /// </summary>
        /// <param name="table"></param>
        private  void ResetInvoiceTable(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                row["TOT_COST"] = 0;
                row["OWN_COST"] = 0;
                row["PAY_COST"] = 0;
                row["PUB_COST"] = 0;
            }
        }

        /// <summary>
        /// 生成发票明细
        /// </summary>
        /// <param name="feeItemLists">费用明细集合</param>
        /// <param name="register">挂号信息</param>
        /// <param name="invoiceNO">发票号</param>
        /// <param name="dtInvoice">发票大类</param>
        /// <param name="invoiceFlag">发票标志</param>
        /// <param name="splitType">分发票标志</param>
        /// <param name="errText">错误信息</param>
        /// <returns>成功: 发票明细集合 失败 null</returns>
        private  ArrayList MakeInvoiceDetail(ArrayList feeItemLists, Register register, string invoiceNO, DataTable dtInvoice, string invoiceFlag, string splitType, ref string errText)
        {
            ArrayList balanceLists = new ArrayList();

            foreach (FeeItemList f in feeItemLists)
            {
                DataRow rowFind = dtInvoice.Rows.Find(new object[] { f.Item.MinFee.ID });
                if (rowFind == null)
                {
                    errText = "最小费用为" + f.Item.MinFee.ID + "的最小费用没有在MZ01的发票大类中维护";
                    return null;
                }
                if (invoiceFlag == "1")//自费发票
                {
                    rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.OwnCost;
                    rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + f.FT.OwnCost;
                    rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                    rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                }
                //if (splitType == "0")//广医
                //{
                //    if (invoiceFlag == "2" || invoiceFlag == "3")//记账发票,特殊发票
                //    {
                //        rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.PayCost + f.FT.PubCost;
                //        rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + 0;
                //        rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                //        rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                //    }
                //}
                //if (splitType == "1")//中山
                //{
                //    if (invoiceFlag == "2" || invoiceFlag == "3")//记账发票,特殊发票
                //    {
                //        rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.PayCost + f.FT.PubCost + f.FT.OwnCost;
                //        rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + f.FT.OwnCost;
                //        rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                //        rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                //    }
                //}
                //if (invoiceFlag == "4")//医保发票
                //{
                //    rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.PayCost + f.FT.PubCost + f.FT.OwnCost;
                //    rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + f.FT.OwnCost;
                //    rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                //    rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                //}

                if (invoiceFlag == "5")
                {
                    rowFind["TOT_COST"] = NConvert.ToDecimal(rowFind["TOT_COST"].ToString()) + f.FT.PayCost + f.FT.PubCost + f.FT.OwnCost;
                    rowFind["OWN_COST"] = NConvert.ToDecimal(rowFind["OWN_COST"].ToString()) + f.FT.OwnCost;
                    rowFind["PAY_COST"] = NConvert.ToDecimal(rowFind["PAY_COST"].ToString()) + f.FT.PayCost;
                    rowFind["PUB_COST"] = NConvert.ToDecimal(rowFind["PUB_COST"].ToString()) + f.FT.PubCost;
                }

                //费用表记录
                f.FeeCodeStat.ID = rowFind["FEE_STAT_CATE"].ToString();
                f.FeeCodeStat.Name = rowFind["FEE_STAT_NAME"].ToString();
                f.FeeCodeStat.SortID = NConvert.ToInt32(rowFind["SEQ"].ToString());

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
                    ///liuq
                    ///2007-8-20修改，保存打印序号到实体。
                    ///----------------------------------------------------
                    detail.FeeCodeStat.SortID = NConvert.ToInt32(rowFind[0]["SEQ"].ToString());
                    ///---------------------------------------------------
                    detail.BalanceBase.IsDayBalanced = false;
                    detail.BalanceBase.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
                    detail.Memo = invoiceFlag;
                    foreach (DataRow row in rowFind)
                    {
                        detail.BalanceBase.FT.PubCost += NConvert.ToDecimal(row["PUB_COST"].ToString());
                        detail.BalanceBase.FT.OwnCost += NConvert.ToDecimal(row["OWN_COST"].ToString());
                        detail.BalanceBase.FT.PayCost += NConvert.ToDecimal(row["PAY_COST"].ToString());
                    }
                    detail.BalanceBase.FT.TotCost = detail.BalanceBase.FT.PubCost + detail.BalanceBase.FT.OwnCost + detail.BalanceBase.FT.PayCost;

                    //如果费用为0 说明次统计类别(打印序号)下没有费用
                    //处理四舍五入费用，暂时屏蔽，小于0也可以打印在发票上{DE54BEAE-EF40-4aa4-8DF5-8CCB2A3DDA1D}
                    if (detail.BalanceBase.FT.TotCost == 0 && detail.BalanceBase.FT.PubCost == 0 && detail.BalanceBase.FT.PayCost == 0 && detail.BalanceBase.FT.OwnCost == 0)
                    {
                        continue;
                    }
                    detail.BalanceBase.RecipeOper.Dept.ID = register.SeeDoct.Dept.ID;
                    detail.BalanceBase.RecipeOper.Dept.Name = register.SeeDoct.Dept.Name;
                    if (string.IsNullOrEmpty(detail.BalanceBase.RecipeOper.Dept.ID))
                    {
                        detail.BalanceBase.RecipeOper.Dept.ID = register.DoctorInfo.Templet.Dept.ID;
                        detail.BalanceBase.RecipeOper.Dept.Name = register.DoctorInfo.Templet.Dept.Name;
                    }

                    balanceLists.Add(detail);
                }
            }

            return balanceLists;
        }

        /// <summary>
        /// 根据明细形成门诊发票和发票明细
        /// </summary>
        /// <param name="feeIntegrate">费用业务层</param>
        /// <param name="feeItemLists">费用明细集合</param>
        /// <param name="register">挂号信息</param>
        /// <param name="invoiceNO">发票号电脑</param>
        /// <param name="realInvoiceNO">发票号打印号</param>
        /// <param name="dtInvoice">发票统计大类MZ01</param>
        /// <param name="invoiceFlag">发票类别 1 自费 2 记账 3 特殊 4 医保</param>
        /// <param name="balance">发票主表实体</param>
        /// <param name="balanceLists">发票明细集合</param>
        /// <param name="splitType">分发票类别</param>
        /// <param name="errText">错误信息</param>
        /// <returns>成功 1 失败 -1</returns>
        private int MakeInvoiceAndDetail(Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate, ArrayList feeItemLists, Register register, ref string invoiceNO, ref string realInvoiceNO, DataTable dtInvoice,
            string invoiceFlag, ref Balance balance, ref ArrayList balanceLists, string splitType, ref string errText, int countI)
        {
            //形成主发票
            balance = MakeInvoiceInfo(feeItemLists, register, invoiceNO, realInvoiceNO, invoiceFlag, splitType);
            if (balance.FT.TotCost <= 0)
            {
                return -2;
            }
            //清空发票统计大类的费用合计.
            ResetInvoiceTable(dtInvoice);
            //形成发票明细
            ArrayList tempBalanceLists = MakeInvoiceDetail(feeItemLists, register, invoiceNO, dtInvoice, invoiceFlag, splitType, ref errText);
            if (tempBalanceLists == null)
            {
                return -1;
            }
            //把自费发票加入发票明细表集合.
            balanceLists.Add(tempBalanceLists);

            //明细重新赋值发票 

            {
                foreach (FeeItemList f in feeItemLists)
                {
                    f.Invoice = balance.Invoice;
                }
            }

            invoiceNO = Neusoft.FrameWork.Public.String.AddNumber(invoiceNO, 1);
            realInvoiceNO = Neusoft.FrameWork.Public.String.AddNumber(realInvoiceNO, 1);

            return 1;
        }

        /// <summary>
        /// 根据明细生成发票和发票明细信息
        /// </summary>
        /// <param name="feeIntegrate">费用业务层</param>
        /// <param name="register">患者挂号基本信息</param>
        /// <param name="feeItemLists">费用明细集合</param>
        /// <param name="invoiceBeginNO">起始发票号</param>
        /// <param name="realInvoiceBeginNO">起始发票打印号</param>
        /// <param name="errText">错误信息</param>
        /// <param name="t">事务</param>
        /// <returns>发票主表和发票明细表集合[0]发票主表 [1]发票明细表</returns>
        public ArrayList MakeInvoice(Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate, Register register, ArrayList feeItemLists,
            string invoiceBeginNO, string realInvoiceBeginNO, ref string errText, System.Data.IDbTransaction t)
        {
            const string OWN_INVOICE = "1";//自费发票
            const string MAIN_INVOICE = "5";//所有费用信息形成的发票

            int returnValue = 0;//返回值
            DataSet dsInvoice = new DataSet();//发票大类
            ArrayList balancesAndBalanceListsAndFeeListsAll = new ArrayList();//所有发票和发票明细信息
            ArrayList balances = new ArrayList();  //发票主表集合
            ArrayList balanceLists = new ArrayList();//发票明细集合
            //发票费用明细
            ArrayList feeLists = new ArrayList();
            if (t != null)
            {
                feeIntegrate.SetTrans(t);
            }

            Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint iInvoicePrint = null;

            iInvoicePrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(typeof(Neusoft.HISFC.BizProcess.Integrate.Fee), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IInvoicePrint;
            if (iInvoicePrint == null)
            {
                errText = "请维护打印票据，查找打印票据失败！";
                return null;
            }
            iInvoicePrint.Register = register;

            returnValue = feeIntegrate.GetInvoiceClass(iInvoicePrint.InvoiceType, ref dsInvoice);
            if (dsInvoice.Tables[0].PrimaryKey.Length == 0)
            {
                dsInvoice.Tables[0].PrimaryKey = new DataColumn[] { dsInvoice.Tables[0].Columns["FEE_CODE"] };
            }
            foreach (FeeItemList f in feeItemLists)
            {
                DataRow rowFind = dsInvoice.Tables[0].Rows.Find(new object[] { f.Item.MinFee.ID });
                //找到相应对应的发票项目
                f.Invoice.Type.ID = rowFind["FEE_STAT_CATE"].ToString();

            }

            string splitType = "0";

            if (feeItemLists.Count > 0)
            {
                string tempInvoiceNO = invoiceBeginNO;//主体发票的发票号不需要累加,主要是为了

                ArrayList feeItemListSplit = this.SplitInvoice(feeItemLists);
                if (feeItemListSplit == null)
                {
                    //{20DC3F9B-39E8-46dd-8941-C5AC75FD652B}
                    errText = feeIntegrate.Err;
                    return null;
                }
                //{1C257F28-1A60-437d-9E61-A6A3D99357EE}
                int i = 0;

                //获得发票序列
                string invoiceCombNO = outpatientManager.GetInvoiceCombNO();

                foreach (ArrayList list in feeItemListSplit)
                {
                    Balance invoice = new Balance();//发票实体
                    ArrayList tempBalanceLists = new ArrayList();//发票明细实体集合

                    if (t != null)
                    {
                        outpatientManager.SetTrans(t);
                    }

                    returnValue = MakeInvoiceAndDetail(
                        feeIntegrate, list, register, ref tempInvoiceNO, ref realInvoiceBeginNO,
                        dsInvoice.Tables[0], OWN_INVOICE, ref invoice,
                        ref tempBalanceLists, splitType, ref errText, i);
                    if (returnValue == -1)
                    {
                        return null;
                    }
                    if (returnValue != -2)
                    {
                        invoice.CombNO = invoiceCombNO;
                        foreach (FeeItemList f in list)
                        {
                            f.InvoiceCombNO = invoiceCombNO;
                            //f.Invoice.ID = tempInvoiceNO;
                        }
                        foreach (ArrayList tempBalanceList in tempBalanceLists)
                        {
                            foreach (BalanceList detail in tempBalanceList)
                            {
                                ((Balance)detail.BalanceBase).CombNO = invoiceCombNO;
                            }
                        }

                        //发票所对应的费用明细
                        feeLists.Add(list);
                        balances.Add(invoice);
                        balanceLists.Add(tempBalanceLists);
                    }
                    i++;
                }

                //当患者上传医保后公费字段有值时走一下方法，可使发票显示正确
                if (register.Pact.PayKind.ID == "02")
                {
                    if (register.SIMainInfo.PubCost + register.SIMainInfo.PayCost > 0)
                    {
                        if (balances != null && balances.Count != 0)
                        {
                            //广医肿瘤分发票
                            //输血费单独打印发票
                            //医保患者，将所有医保金额分配到第一张发票上，如果第一张发票总金额不够，则剩余分配到第二张发票

                            decimal rate = 0;
                            decimal totPubCost = 0;
                            decimal totPayCost = 0;
                            Balance balance = new Balance();
                            int intCount = 0;
                            for (intCount = 0; intCount < balances.Count - 1; intCount++)
                            {
                                balance = balances[intCount] as Balance;

                                rate = balance.FT.TotCost / register.SIMainInfo.TotCost;
                                balance.FT.PubCost = register.SIMainInfo.PubCost * rate;
                                balance.FT.PayCost = register.SIMainInfo.PayCost * rate;
                                balance.FT.PayCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.PayCost, 2);
                                balance.FT.PubCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.PubCost, 2);
                                balance.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(balance.FT.TotCost, 2);
                                balance.FT.OwnCost = balance.FT.TotCost - balance.FT.PayCost - balance.FT.PayCost;
                                totPayCost += balance.FT.PayCost;
                                totPubCost += balance.FT.PubCost;
                            }
                            balance = balances[intCount] as Balance;
                            balance.FT.PubCost = register.SIMainInfo.PubCost - totPubCost;
                            balance.FT.PayCost = register.SIMainInfo.PayCost - totPayCost;
                            balance.FT.OwnCost = register.SIMainInfo.TotCost - balance.FT.PubCost - balance.FT.PayCost;
                        }
                    }
                }
            }


            balancesAndBalanceListsAndFeeListsAll.Add(balances);
            balancesAndBalanceListsAndFeeListsAll.Add(balanceLists);
            balancesAndBalanceListsAndFeeListsAll.Add(feeLists);

            return balancesAndBalanceListsAndFeeListsAll;
        }

        public ArrayList SplitInvoice(ArrayList feeItemLists)
        { 
            //为了使速度更快，默认分发票接口里面不在走下面的代码，直接返回
            ArrayList finalSplitList = new ArrayList();
            ArrayList alBooldFee = new ArrayList();
            ArrayList alOtherFee = new ArrayList();

            //输血费单独发票
            foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList itemList in feeItemLists)
            {
                if (itemList.Item.MinFee.ID.Equals("025"))//输血费一组，单独写死不在从数据库里面查询
                {
                    alBooldFee.Add(itemList);
                }
                else
                {
                    alOtherFee.Add(itemList);
                }
            }

            if (alOtherFee.Count > 0)
            {
                finalSplitList.Add(alOtherFee);
            }

            if (alBooldFee.Count > 0)
            {
                finalSplitList.Add(alBooldFee);
            }

            return finalSplitList;
        }

        #endregion

        #region ISplitInvoice 成员

        Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        public void SetTrans(System.Data.IDbTransaction trans)
        {
        }

        public System.Collections.ArrayList SplitInvoice(Neusoft.HISFC.Models.Registration.Register register, ref System.Collections.ArrayList feeItemLists)
        {
            Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();
            employee.ID = Neusoft.FrameWork.Management.Connection.Operator.ID;

            string invoiceNO = string.Empty;
            string realInvoiceNO = string.Empty;
            string errText = string.Empty;
            //获得本次收费起始发票号
            int iReturnValue = this.feeIntegrate.GetInvoiceNO(employee, "C", ref invoiceNO, ref realInvoiceNO, ref errText);
            if (iReturnValue == -1)
            {
                MessageBox.Show("获取发票号失败，" + errText);
                return null;
            }

            return MakeInvoice(feeIntegrate, register, feeItemLists, invoiceNO, realInvoiceNO, ref errText, null);
        }

        public System.Data.IDbTransaction Trans
        {
            set {  }
        }

        #endregion
    }
}
