using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.Fee.Outpatient;
using System.Data;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Fee.Item;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy
{
    /// <summary>
    /// ct和mr收费规则
    /// </summary>
    public class CtMrFeeRule
    {
        #region 属性
        private Neusoft.HISFC.BizLogic.Manager.Constant consManager = new Neusoft.HISFC.BizLogic.Manager.Constant();
        private Neusoft.HISFC.BizLogic.Fee.Item undrugManager = new Neusoft.HISFC.BizLogic.Fee.Item();
        private Neusoft.HISFC.BizLogic.Fee.UndrugPackAge undrugPackAgeManager = new Neusoft.HISFC.BizLogic.Fee.UndrugPackAge();
        private Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        private Neusoft.HISFC.BizProcess.Integrate.Order orderIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Order();
        private Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        private Neusoft.HISFC.BizLogic.Fee.PactUnitItemRate pactUnitItemRateManager = new Neusoft.HISFC.BizLogic.Fee.PactUnitItemRate();
        private DBCtMrFeeRuleFunction db = new DBCtMrFeeRuleFunction();
        public string ErrMsg = "";
        #endregion

        public ArrayList GetFeeListForArry(ArrayList arry, Neusoft.HISFC.Models.Registration.Register reg)
        {
            //是否使用新的CT/MR收费规则
            bool IsUseNewCTMRFeeRule = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam().GetControlParam<bool>("IsUseNewCTMRFeeRule", false, false);
            Hashtable hsItemZT = null; //this.GetCTMRHashtabel();
            ArrayList feeItemLists = new ArrayList();//返回值
            bool isFindDRFirst = false;
            bool isFindCTFirst = false;
            Hashtable hsDROnlyOneItem = new Hashtable();
            Hashtable hsCTOnlyOneItem = new Hashtable();
            try
            {
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f in arry)
                {
                    //不是非药品的直接添加到返回值集合中
                    if (f.Item.ItemType != EnumItemType.UnDrug)
                    {
                        feeItemLists.Add(f);
                        continue;
                    }
                    //获取非药品信息
                    Neusoft.HISFC.Models.Fee.Item.Undrug undrugItem = undrugManager.GetUndrugByCode(f.Item.ID);
                    //非复合项目直接添加到返回值集合中
                    if (undrugItem == null || undrugItem.UnitFlag != "1")
                    {
                        feeItemLists.Add(f);
                        continue;
                    }
                    else
                    {
                        if (hsItemZT == null)
                        {
                            hsItemZT = this.GetCTMRHashtabel();
                        }
                    }
                    ArrayList alDetail = null;
                    if (IsUseNewCTMRFeeRule && hsItemZT.ContainsKey(f.Item.ID))
                    {
                        ArrayList alItem = (ArrayList)hsItemZT[f.Item.ID];
                        string type = (alItem[0] as NeuObject).User02;
                        if (type == "DR")
                        {
                            //alDetail = ConvertDRGroupToDetail(f, !isFindDRFirst, ref hsDROnlyOneItem, ref drCount);
                            isFindDRFirst = true;
                        }
                        else if (type == "CT")
                        {
                            //alDetail = ConvertCTGroupToDetail(f, !isFindCTFirst, ref hsCTOnlyOneItem);
                            isFindCTFirst = true;
                        }
                    }
                    else
                    {
                        //alDetail = ConvertGroupToDetail(f);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                this.ErrMsg = ex.Message;
                return null;
            }

            return feeItemLists;
        }

        #region 获取CT/MR收费规则的HashTable
        /// <summary>
        /// 获取CT/MR收费规则的HashTable
        /// </summary>
        /// <returns></returns>
        public Hashtable GetCTMRHashtabel()
        {
            ArrayList alItemZT = this.consManager.GetAllList("ItemZT");
            Hashtable hsItemZT = new Hashtable();//返回值
            if (alItemZT == null)
            {
                return null;
            }
            foreach (Neusoft.HISFC.Models.Base.Const conObj in alItemZT)
            {
                Neusoft.FrameWork.Models.NeuObject obj = null;
                if (!conObj.IsValid)
                {
                    continue;
                }
                if (hsItemZT.ContainsKey(conObj.Name))
                {
                    if (string.IsNullOrEmpty(conObj.Memo.Trim()))
                    {
                        continue;
                    }
                    string[] itemIDs = null;
                    //string[] temps = conObj.Memo.Split('&');

                    itemIDs = conObj.Memo.Split('|');
                    foreach (string itemID in itemIDs)
                    {
                        obj = new NeuObject();
                        obj.ID = itemID;
                        obj.Name = conObj.WBCode;//数量
                        switch (conObj.SortID.ToString())
                        {
                            case "0":
                                obj.Memo = "每个项目收取";
                                break;
                            case "1":
                                obj.Memo = "第一个项目收取";
                                break;
                            case "2":
                                obj.Memo = "第二个项目起加收";
                                break;
                            case "3":
                                obj.Memo = "只收取一次";
                                break;

                        }

                        //obj.Memo = temps[2];//公式 0 每个项目收取、1 第一个项目收取、2 第二个项目起加收
                        switch (conObj.SpellCode)
                        {
                            case "0":
                                obj.User01 = "总量取整";
                                break;
                            case "1":
                                obj.User01 = "单个取整";
                                break;
                            case "2":
                                obj.User01 = "固定数量";
                                break;
                        }
                        //obj.User01 = conObj.SpellCode;//0 总量取整、1 单个取整 2固定数量
                        switch (conObj.UserCode)
                        {
                            case "0":
                                obj.User02 = "DR";
                                break;
                            case "1":
                                obj.User02 = "CT";
                                break;
                        }
                        //obj.User02 = conObj.UserCode;//0 DR 1 CT

                        ((ArrayList)hsItemZT[conObj.Name]).Add(obj);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(conObj.Memo.Trim()))
                    {
                        continue;
                    }
                    ArrayList al = new ArrayList();
                    string[] itemIDs = null;
                    //string[] temps = conObj.Memo.Split('&');
                    itemIDs = conObj.Memo.Split('|');
                    foreach (string itemID in itemIDs)
                    {
                        obj = new NeuObject();
                        obj.ID = itemID;
                        obj.Name = conObj.WBCode;//数量
                        switch (conObj.SortID.ToString())
                        {
                            case "0":
                                obj.Memo = "每个项目收取";
                                break;
                            case "1":
                                obj.Memo = "第一个项目收取";
                                break;
                            case "2":
                                obj.Memo = "第二个项目起加收";
                                break;
                        }

                        //obj.Memo = temps[2];//公式 0 每个项目收取、1 第一个项目收取、2 第二个项目起加收
                        switch (conObj.SpellCode)
                        {
                            case "0":
                                obj.User01 = "总量取整";
                                break;
                            case "1":
                                obj.User01 = "单个取整";
                                break;
                            case "2":
                                obj.User01 = "固定数量";
                                break;
                        }
                        //obj.User01 = conObj.SpellCode;//0 总量取整、1 单个取整 2固定数量
                        switch (conObj.UserCode)
                        {
                            case "0":
                                obj.User02 = "DR";
                                break;
                            case "1":
                                obj.User02 = "CT";
                                break;
                        }
                        //obj.User02 = conObj.UserCode;//0 DR 1 CT

                        al.Add(obj);
                        hsItemZT.Add(conObj.Name, al);
                    }
                }
            }
            return hsItemZT;
        } 
        #endregion

        /// <summary>
        /// 把普通组套拆分成明细
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        //private ArrayList ConvertGroupToDetail(FeeItemList f, Neusoft.HISFC.Models.Registration.Register rInfo, ref string errText)
        //{
        //    ArrayList undrugCombList = this.db.QueryUndrugPackagesBypackageCode(f.Item.ID);
        //    ArrayList alTemp = new ArrayList();
        //    if (undrugCombList == null)
        //    {
        //        errText = "获得组套明细出错!" + db.Err;
        //        return null;
        //    }
        //    decimal price = 0;
        //    decimal priceSecond = 0; // {C41CAC71-0186-43cf-9167-2D33E4626D74}
        //    decimal count = 0;
        //    string feeCode = string.Empty;
        //    string itemType = string.Empty;
        //    decimal totCost = 0;
        //    FeeItemList feeDetail = null;
        //    if (f.Order.ID == null || f.Order.ID == string.Empty)
        //    {
        //        f.Order.ID = this.db.GetNewOrderID();
        //        if (f.Order.ID == null || f.Order.ID == string.Empty)
        //        {
        //            errText = "获得医嘱流水号出错!";
        //            return null;
        //        }
        //    }

        //    //有价格打折的
        //    DataSet dsItem = new DataSet();
        //    if (this.outpatientManager.QueryItemList(deptCode, Neusoft.HISFC.Models.Base.ItemKind.Undrug, ref dsItem) == -1)
        //    {
        //        errMsg = "获得项目列表出错!" + this.outpatientManager.Err;
        //        return null;
        //    }
        //    DataRow rowFind;
        //    DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
        //    if (rowFinds == null || rowFinds.Length == 0)
        //    {
        //        errText = "查找组套明细出错!";
        //        return null;
        //    }
        //    rowFind = rowFinds[0];

        //    DateTime nowTime = this.db.GetDateTimeFromSysDateTime();
        //    int age = 0;
        //    int month = 0;
        //    int day = 0;
        //    this.db.GetAge(rInfo.Birthday, nowTime, ref age, ref month, ref day);

        //    //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
        //    string priceForm = rInfo.Pact.PriceForm;

        //    decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
        //    decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
        //    decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
        //    decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);

        //    decimal orgGroupPrice = 0;
        //    Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IGetItemPrice.ItemPrice priceBll = new Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IGetItemPrice.ItemPrice();
        //    decimal priceGroup = priceBll.GetPrice(f.Item.ID, rInfo, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup, ref orgGroupPrice);

        //    decimal rate = f.Item.Price / orgGroupPrice;
        //    if (rate == 1)
        //    {
        //        rate = priceGroup / orgGroupPrice;
        //    }

        //    //符合项目明细的加成（减免）比例
        //    decimal itemRate = 1;
        //    foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
        //    {
        //        DataRow rowFindZT;
        //        DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
        //        if (rowFindZTs == null || rowFindZTs.Length == 0)
        //        {
        //            errText = "查找组套明细出错!";

        //            continue;
        //        }
        //        rowFindZT = rowFindZTs[0];

        //        feeDetail = new FeeItemList();

        //        feeCode = rowFindZT["FEE_CODE"].ToString();
        //        try
        //        {

        //            decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
        //            decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
        //            decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
        //            decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

        //            // 保存原始默认价格
        //            feeDetail.Item.ChildPrice = unitPrice;
        //            bool isTransferTreat = false;//是否转诊
        //            if (isTransferTreat == true)
        //            {
        //                decimal orgPrice = price;
        //                itemRate = 1;// feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
        //                price = unitPrice;// this.feeIntegrate.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
        //                feeDetail.OrgPrice = orgPrice;
        //            }
        //            else
        //            {
        //                decimal orgPrice = price;
        //                itemRate = feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
        //                price = this.feeIntegrate.GetPrice(undrugCombo.ID, rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
        //                feeDetail.OrgPrice = orgPrice;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            errText = e.Message;

        //            return null;
        //        }

        //        //组合项目原本就有打折的
        //        //中五打折不需要用计算的rate
        //        //if (rate > 0)
        //        //{
        //        //    price *= rate;
        //        //}

        //        //根据优惠比例重新计算单价------------------------- 
        //        string errMsg = string.Empty;
        //        PactItemRate myRate = this.PactRate(rInfo, feeDetail, ref errMsg);
        //        if (myRate == null)
        //        {
        //            errText = errMsg;
        //            return null;
        //        }

        //        price *= 1 - myRate.Rate.RebateRate;
        //        //--------------------------------------------------
        //        count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

        //        //组套拆分成明细的时候，也保存两位小数
        //        //totCost = price * count;
        //        totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

        //        feeDetail.Patient = f.Patient.Clone();
        //        feeDetail.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
        //        feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
        //        feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
        //        feeDetail.Name = feeDetail.Item.Name;
        //        feeDetail.ID = feeDetail.Item.ID;
        //        itemType = rowFindZT["DRUG_FLAG"].ToString();
        //        if (itemType == "0")
        //        {
        //            //feeDetail.Item.IsPharmacy = false;
        //            feeDetail.Item.ItemType = EnumItemType.UnDrug;
        //            feeDetail.IsGroup = false;
        //        }
        //        if (itemType == "1")
        //        {
        //            //feeDetail.Item.IsPharmacy = true;
        //            feeDetail.Item.ItemType = EnumItemType.Drug;
        //            feeDetail.IsGroup = false;
        //        }
        //        if (itemType == "2")
        //        {
        //            //feeDetail.Item.IsPharmacy = false;
        //            feeDetail.Item.ItemType = EnumItemType.UnDrug;
        //            feeDetail.IsGroup = true;
        //        }
        //        feeDetail.RecipeOper = f.RecipeOper.Clone();
        //        feeDetail.Item.Price = price;
        //        feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
        //        feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
        //        feeDetail.Item.MinFee.ID = feeCode;
        //        feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
        //        feeDetail.Item.Qty = count;
        //        feeDetail.Days = NConvert.ToDecimal(f.Days);
        //        feeDetail.FT.TotCost = totCost;
        //        //自费如此，如果加上公费需要重新计算!!!
        //        feeDetail.FT.OwnCost = totCost;
        //        feeDetail.ExecOper = f.ExecOper.Clone();
        //        feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
        //        //if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
        //        //{
        //        //    feeDetail.Item.IsNeedConfirm = true;
        //        //}
        //        //else
        //        //{
        //        //    feeDetail.Item.IsNeedConfirm = false;
        //        //}

        //        //feeDetail.Item.NeedConfirm = f.Item.NeedConfirm;

        //        if (string.IsNullOrEmpty(rowFindZT["CONFIRM_FLAG"].ToString()))
        //        {
        //            feeDetail.Item.NeedConfirm = EnumNeedConfirm.None;
        //        }
        //        else
        //        {
        //            if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
        //                Neusoft.FrameWork.Function.NConvert.ToInt32(rowFindZT["CONFIRM_FLAG"].ToString())))
        //            {
        //                feeDetail.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFindZT["CONFIRM_FLAG"].ToString());
        //            }
        //        }

        //        feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

        //        feeDetail.Order.ID = f.Order.ID;

        //        feeDetail.UndrugComb.ID = f.Item.ID;
        //        feeDetail.UndrugComb.Name = f.Item.Name;
        //        feeDetail.UndrugComb.Qty = f.Item.Qty;

        //        feeDetail.Order.Combo.ID = f.Order.Combo.ID;
        //        feeDetail.Item.IsMaterial = f.Item.IsMaterial;
        //        feeDetail.RecipeSequence = f.RecipeSequence;
        //        feeDetail.FTSource = f.FTSource;
        //        feeDetail.FeePack = f.FeePack;
        //        if (rInfo.Pact.PayKind.ID == "03")
        //        {
        //            Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

        //            if (pactRate == null)
        //            {
        //                pactRate = this.pactUnitItemRateManager.GetOnepPactUnitItemRateByItem(rInfo.Pact.ID, feeDetail.Item.ID);
        //            }
        //            if (pactRate != null)
        //            {
        //                if (pactRate.Rate.PayRate != rInfo.Pact.Rate.PayRate)
        //                {
        //                    if (pactRate.Rate.PayRate == 1)//自费
        //                    {
        //                        feeDetail.ItemRateFlag = "1";
        //                    }
        //                    else
        //                    {
        //                        //feeDetail.ItemRateFlag = "3";
        //                        feeDetail.ItemRateFlag = "2";
        //                    }
        //                }
        //                else
        //                {
        //                    feeDetail.ItemRateFlag = "2";

        //                }
        //                if (f.ItemRateFlag == "3")
        //                {
        //                    feeDetail.OrgItemRate = f.OrgItemRate;
        //                    feeDetail.NewItemRate = f.NewItemRate;
        //                    //feeDetail.ItemRateFlag = "2";//DEL 30
        //                    feeDetail.ItemRateFlag = "3";
        //                }
        //            }
        //            else
        //            {
        //                if (f.ItemRateFlag == "3")
        //                {
        //                    //DEL 30
        //                    ////if (rowFindZT["ZF"].ToString() != "1")
        //                    ////{
        //                    ////    feeDetail.OrgItemRate = f.OrgItemRate;
        //                    ////    feeDetail.NewItemRate = f.NewItemRate;
        //                    ////    feeDetail.ItemRateFlag = "2";
        //                    ////}
        //                    feeDetail.OrgItemRate = f.OrgItemRate;
        //                    feeDetail.NewItemRate = f.NewItemRate;
        //                    feeDetail.ItemRateFlag = "3";
        //                }
        //                else
        //                {
        //                    feeDetail.OrgItemRate = f.OrgItemRate;
        //                    feeDetail.NewItemRate = f.NewItemRate;
        //                    feeDetail.ItemRateFlag = f.ItemRateFlag;
        //                }
        //            }
        //        }

        //        //复合项目的用法赋给明细项目
        //        feeDetail.Order.Usage = f.Order.Usage;
        //        //使用原来的处方号
        //        //feeDetail.RecipeNO = f.RecipeNO;
        //        feeDetail.Order.ApplyNo = f.Order.ApplyNo;
        //        feeDetail.Order.Sample.ID = f.Order.Sample.ID;
        //        feeDetail.Order.Sample.Name = f.Order.Sample.Name;
        //        feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

        //        alTemp.Add(feeDetail);
        //    }
        //    if (alTemp.Count > 0)
        //    {
        //        if (f.FT.RebateCost > 0)//有减免
        //        {
        //            if (rInfo.Pact.PayKind.ID != "01")
        //            {
        //                errText = "暂时不允许非自费患者减免!";
        //                return null;
        //            }
        //            //decimal rebateRate =
        //            //    Neusoft.FrameWork.Public.String.FormatNumber(
        //            //    f.FT.RebateCost / (f.FT.OwnCost + f.FT.RebateCost), 2);
        //            //decimal tempFix = 0;
        //            //decimal tempRebateCost = 0;
        //            //foreach (FeeItemList feeTemp in alTemp)
        //            //{
        //            //    feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost + feeTemp.FT.RebateCost) * rebateRate;
        //            //    tempRebateCost += feeTemp.FT.RebateCost;
        //            //    feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
        //            //    feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
        //            //}
        //            //tempFix = f.FT.RebateCost - tempRebateCost;
        //            //FeeItemList fFix = alTemp[0] as FeeItemList;
        //            //fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
        //            //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
        //            //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
        //            //减免单独算
        //            decimal rebateRate =
        //                Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
        //            decimal tempFix = 0;
        //            decimal tempRebateCost = 0;
        //            foreach (FeeItemList feeTemp in alTemp)
        //            {
        //                feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
        //                tempRebateCost += feeTemp.FT.RebateCost;
        //                //feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
        //                //feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
        //            }
        //            tempFix = f.FT.RebateCost - tempRebateCost;
        //            FeeItemList fFix = alTemp[0] as FeeItemList;
        //            fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
        //            //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
        //            //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
        //        }
        //    }
        //    if (alTemp.Count > 0)
        //    {
        //        if (f.SpecialPrice > 0)//有特殊自费
        //        {
        //            decimal tempPrice = 0m;
        //            string id = string.Empty;
        //            foreach (FeeItemList feeTemp in alTemp)
        //            {
        //                if (feeTemp.Item.Price > tempPrice)
        //                {
        //                    id = feeTemp.Item.ID;
        //                    tempPrice = feeTemp.Item.Price;
        //                }
        //            }

        //            foreach (FeeItemList fee in alTemp)
        //            {
        //                if (fee.Item.ID == id)
        //                {
        //                    fee.SpecialPrice = f.SpecialPrice;

        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    if (alTemp.Count > 0)
        //    {
        //        if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
        //        {
        //            decimal tempPrice = 0m;
        //            string id = string.Empty;
        //            foreach (FeeItemList feeTemp in alTemp)
        //            {
        //                if (feeTemp.Item.Price > tempPrice)
        //                {
        //                    id = feeTemp.Item.ID;
        //                    tempPrice = feeTemp.Item.Price;
        //                }
        //            }

        //            foreach (FeeItemList fee in alTemp)
        //            {
        //                if (fee.Item.ID == id)
        //                {
        //                    fee.FT.User03 = f.FT.User03;

        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return alTemp;
        //}
       


        /// <summary>
        /// 返回项目比例
        /// </summary>
        /// <param name="r"></param>
        /// <param name="f"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Base.PactItemRate PactRate(Neusoft.HISFC.Models.Registration.Register r, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f, ref string errMsg)
        {
            Neusoft.HISFC.Models.Base.PactItemRate pRate = new Neusoft.HISFC.Models.Base.PactItemRate();
            pRate.Rate.RebateRate = 0;
            return pRate;
        }

    }


    /// <summary>
    /// 临时数据库处理类(方便微信自助机使用，无需引用N多DLL，导致其他不必要的问题)
    /// </summary>
    public class DBCtMrFeeRuleFunction : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 根据组套编码查询对应的非药品组套明细数据 
        /// </summary>
        /// <param name="packageCode"></param>
        /// <returns></returns>
        public ArrayList QueryUndrugPackagesBypackageCode(string packageCode)
        {
            ArrayList List = null;
            string strSql = "";
            if (this.Sql.GetCommonSql("Fee.undrugzt.GetUndrugztinfo", ref strSql) == -1) return null;
            try
            {
                if (packageCode != "")
                {
                    List = new ArrayList();

                    strSql = string.Format(strSql, packageCode);
                    this.ExecQuery(strSql);
                    Neusoft.HISFC.Models.Fee.Item.UndrugComb info = null;
                    while (this.Reader.Read())
                    {
                        info = new Neusoft.HISFC.Models.Fee.Item.UndrugComb();

                        info.Package.ID = Reader[0].ToString(); //组套编码
                        info.Name = Reader[1].ToString();//非药品名称
                        info.ID = Reader[2].ToString();  //非药品编码
                        if (Reader[3] != DBNull.Value)
                        {
                            info.SortID = Convert.ToInt32(Reader[3]); //顺序号
                        }
                        else
                        {
                            info.SortID = 0;
                        }
                        info.SpellCode = Reader[4].ToString();  //取拼音码
                        info.WBCode = Reader[5].ToString();    //取五笔码
                        info.UserCode = Reader[6].ToString(); //输入码
                        info.User01 = Reader[7].ToString(); //标志
                        info.User02 = Reader[8].ToString(); // 是否特殊医疗项目 0 否 1 是
                        info.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[9].ToString()); //数量
                        List.Add(info);
                        info = null;
                    }
                    this.Reader.Close();
                }
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                List = null;
            }
            return List;
        }

        /// <summary>
        /// 获得医嘱流水号
        /// </summary>
        /// <returns></returns>
        public string GetNewOrderID()
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Management.Order.GetNewOrderID", ref sql) == -1) return null;
            string strReturn = this.ExecSqlReturnOne(sql);
            if (strReturn == "-1" || strReturn == "") return null;
            return strReturn;
        }

        /// <summary>
        /// 获得门诊批费项目列表
        /// </summary>
        /// <param name="deptCode">收费员所在科室</param>
        /// <param name="itemKind">项目列表类别</param>
        /// <param name="ds">项目列表</param>
        /// <returns> -1 失败 > 0 成功</returns>
        public int QueryItemList(string deptCode, Neusoft.HISFC.Models.Base.ItemKind itemKind, ref DataSet ds)
        {
            if (itemKind == ItemKind.All)
            {
                return this.ExecQuery("Fee.Item.GetOutPatientItemList.Select", ref ds, deptCode);
            }
            if (itemKind == ItemKind.Undrug)
            {
                return this.ExecQuery("Fee.Item.GetOutPatientItemList.Select.Undrug", ref ds, deptCode);
            }
            if (itemKind == ItemKind.Pharmacy)
            {
                return this.ExecQuery("Fee.Item.GetOutPatientItemList.Select.Pharmacy", ref ds, deptCode);
            }
            return 1;
        }


    }

}
