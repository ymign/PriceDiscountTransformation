using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZDWY.SpecialRule.Price.DB;
using System.Collections;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.FrameWork.Models;
using System.Data;
using Neusoft.HISFC.Models.Registration;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.Fee.Item;

namespace ZDWY.SpecialRule.Price
{
    public class CTMRFeeRule
    {
        #region 属性
        /// <summary>
        /// 数据库操作类1
        /// </summary>
        CTMRFeeRuleDB db = null;
        /// <summary>
        /// 计算限制收费项目
        /// </summary>
        Restrictingfee setRestrictingfee = new Restrictingfee();
        /// <summary>
        /// 是否启用ct和mr收费规则
        /// </summary>
        private bool IsUseCtOrMRfeeRule = true;
        DataSet dsItem = new DataSet();
        private string deptCode = "";
        public string errText = "";
        protected Register rInfo = null;
        private bool isTransferTreat = false;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public CTMRFeeRule()
        {
            db = new CTMRFeeRuleDB();
        }

        /// <summary>
        /// 获得收费信息
        /// </summary>
        /// <param name="clincCode"></param>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        public ArrayList GetFeeItemList(string clincCode, ArrayList feeArryList)
        {
            bool isFindDRFirst = false;
            bool isFindCTFirst = false;
            Hashtable hsDROnlyOneItem = new Hashtable();
            Hashtable hsCTOnlyOneItem = new Hashtable();
            ArrayList hsNOREOnlyOneItem = new ArrayList();
            Hashtable hsREOnlyOneItem = new Hashtable();
            decimal drCount = 0;
            ArrayList feeItemLists = new ArrayList();
            Hashtable hsDoct = new Hashtable();
            this.rInfo = this.db.GetByClinic(clincCode);
            string tempPayKindid = this.rInfo.Pact.PayKind.ID;
            this.rInfo.Pact = this.db.GetPactUnitInfoByPactCode(this.rInfo.Pact.ID);
            this.rInfo.Pact.PayKind.ID = tempPayKindid;
            this.db.QueryItemList(deptCode, Neusoft.HISFC.Models.Base.ItemKind.All, ref dsItem);
            for (int i = 0; i < feeArryList.Count; i++)
            {
                if (feeArryList[i] == null || !(feeArryList[i] is FeeItemList))
                {
                    continue;
                }
                var f = feeArryList[i] as FeeItemList;
                if (!string.IsNullOrEmpty(f.RecipeOper.ID) && string.IsNullOrEmpty(f.DoctDeptInfo.ID))
                {
                    if (hsDoct.ContainsKey(f.RecipeOper.ID))
                    {
                        f.DoctDeptInfo.ID = hsDoct[f.RecipeOper.ID].ToString();
                    }
                    else
                    {
                        var emplInfo = this.db.GetEmployeeInfoForEmplCode(f.RecipeOper.ID);
                        if (emplInfo != null && !string.IsNullOrEmpty(emplInfo.DEPT_CODE))
                        {
                            f.DoctDeptInfo.ID = emplInfo.DEPT_CODE;
                            hsDoct.Add(f.RecipeOper.ID, emplInfo.DEPT_CODE);
                        }
                    }
                }
                if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug && IsUseCtOrMRfeeRule && this.db.IsZTUndrugInfo(f.Item.ID))
                {
                    ArrayList alDetail = null;

                    var undrugInfo = this.db.GetUndrugByCode(f.Item.ID);
                    f.Item.NeedConfirm = undrugInfo.NeedConfirm;
                    var hsItemZT = this.GetCTMRHashtabel();
                    if (hsItemZT.ContainsKey(f.Item.ID))
                    {
                        ArrayList alItem = (ArrayList)hsItemZT[f.Item.ID];
                        string type = (alItem[0] as NeuObject).User02;
                        if (type == "DR")
                        {
                            alDetail = ConvertDRGroupToDetail(f, !isFindDRFirst, ref hsDROnlyOneItem, ref drCount);
                            isFindDRFirst = true;
                        }
                        else if (type == "CT")
                        {
                            alDetail = ConvertCTGroupToDetail(f, !isFindCTFirst, ref hsCTOnlyOneItem);
                            isFindCTFirst = true;
                        }
                    }
                    else
                    {
                        alDetail = ConvertGroupToDetail(f);
                    }

                    if (alDetail == null)
                    {
                        errText = "获得组套明细出错!" + errText;
                        return null;
                    }

                    if (alDetail.Count == 0)
                    {
                        errText = "处理组套项目出错！" + errText;
                        return null;
                    }
                    feeItemLists.AddRange(alDetail);
                }
                else
                {
                    feeItemLists.Add(f);
                }

            }
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                FeeItemList f = feeItemLists[i] as FeeItemList;
                if (hsDROnlyOneItem.ContainsKey(f.Item.ID))
                {
                    feeItemLists.RemoveAt(i);
                }
                if (hsCTOnlyOneItem.ContainsKey(f.Item.ID))
                {
                    if (hsCTOnlyOneItem[f.Item.ID].ToString() != "true")
                    {
                        hsCTOnlyOneItem.Remove(f.Item.ID);
                        hsCTOnlyOneItem.Add(f.Item.ID, "true");
                    }
                    else
                    {
                        feeItemLists.RemoveAt(i);
                    }
                }
            }
            foreach (DictionaryEntry de in hsDROnlyOneItem)
            {
                FeeItemList f = de.Value as FeeItemList;
                feeItemLists.Add(f);
            }
            int number = 1;
            int returnRows = 0;//是否为限制收费药品
            decimal LimitNumber = 1;
            ArrayList hsREOnlylistItem = new ArrayList();
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                string Discount_type = "1";//限制收费类型
                decimal TOPPRICE = 0;
                decimal DISCOUNT_RATE = 0;
                FeeItemList s = feeItemLists[i] as FeeItemList;
                returnRows = this.db.SetRestrictingfee(s.Item.ID, ref  LimitNumber);
                Discount_type = this.db.SetDiscountfee(s.Item.ID, ref  DISCOUNT_RATE, ref  TOPPRICE);
                if (returnRows > 0 && this.rInfo.DoctorInfo.Templet.Dept.ID != "7021")
                {
                    this.setRestrictingfee.ConvertRestrictingfee(rInfo.PID.CardNO, s, ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem, number, LimitNumber);
                }
                if (Discount_type == "2")
                {
                    this.setRestrictingfee.ConvertDiscountfee(s, DISCOUNT_RATE, TOPPRICE, ref hsREOnlyOneItem, ref hsREOnlylistItem, number);
                }
                number++;
            }
            number = 1;
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                FeeItemList s = feeItemLists[i] as FeeItemList;
                if (hsREOnlyOneItem.ContainsKey(s.Item.ID + number))
                {
                    feeItemLists.RemoveAt(i);
                }
                number++;
            }
            foreach (FeeItemList ds in hsREOnlylistItem)
            {
                feeItemLists.Add(ds);
            }
            return feeItemLists;
        }

        /// <summary>
        /// 获得收费信息(自助机临时屏蔽急诊折价) --自助机专用
        /// </summary>
        /// <param name="clincCode"></param>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        public ArrayList GetFeeItemListExcludingEmergencyDiscountZZSB(string clincCode, ArrayList feeArryList)
        {
            bool isFindDRFirst = false;
            bool isFindCTFirst = false;
            Hashtable hsDROnlyOneItem = new Hashtable();
            Hashtable hsCTOnlyOneItem = new Hashtable();
            ArrayList hsNOREOnlyOneItem = new ArrayList();
            Hashtable hsREOnlyOneItem = new Hashtable();
            decimal drCount = 0;
            ArrayList feeItemLists = new ArrayList();
            Hashtable hsDoct = new Hashtable();
            this.rInfo = this.db.GetByClinic(clincCode);
            string tempPayKindid = this.rInfo.Pact.PayKind.ID;
            this.rInfo.Pact = this.db.GetPactUnitInfoByPactCode(this.rInfo.Pact.ID);
            this.rInfo.Pact.PayKind.ID = tempPayKindid;
            this.db.QueryItemList(deptCode, Neusoft.HISFC.Models.Base.ItemKind.All, ref dsItem);
            for (int i = 0; i < feeArryList.Count; i++)
            {
                if (feeArryList[i] == null || !(feeArryList[i] is FeeItemList))
                {
                    continue;
                }
                var f = feeArryList[i] as FeeItemList;
                if (!string.IsNullOrEmpty(f.RecipeOper.ID) && string.IsNullOrEmpty(f.DoctDeptInfo.ID))
                {
                    if (hsDoct.ContainsKey(f.RecipeOper.ID))
                    {
                        f.DoctDeptInfo.ID = hsDoct[f.RecipeOper.ID].ToString();
                    }
                    else
                    {
                        var emplInfo = this.db.GetEmployeeInfoForEmplCode(f.RecipeOper.ID);
                        if (emplInfo != null && !string.IsNullOrEmpty(emplInfo.DEPT_CODE))
                        {
                            f.DoctDeptInfo.ID = emplInfo.DEPT_CODE;
                            hsDoct.Add(f.RecipeOper.ID, emplInfo.DEPT_CODE);
                        }
                    }
                }
                if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug && IsUseCtOrMRfeeRule && this.db.IsZTUndrugInfo(f.Item.ID))
                {
                    ArrayList alDetail = null;

                    var undrugInfo = this.db.GetUndrugByCode(f.Item.ID);
                    f.Item.NeedConfirm = undrugInfo.NeedConfirm;
                    var hsItemZT = this.GetCTMRHashtabel();
                    if (hsItemZT.ContainsKey(f.Item.ID))
                    {
                        ArrayList alItem = (ArrayList)hsItemZT[f.Item.ID];
                        string type = (alItem[0] as NeuObject).User02;
                        if (type == "DR")
                        {
                            alDetail = ConvertDRGroupToDetail(f, !isFindDRFirst, ref hsDROnlyOneItem, ref drCount);
                            isFindDRFirst = true;
                        }
                        else if (type == "CT")
                        {
                            alDetail = ConvertCTGroupToDetail(f, !isFindCTFirst, ref hsCTOnlyOneItem);
                            isFindCTFirst = true;
                        }
                    }
                    else
                    {
                        alDetail = ConvertGroupToDetail(f);
                    }

                    if (alDetail == null)
                    {
                        errText = "获得组套明细出错!" + errText;
                        return null;
                    }

                    if (alDetail.Count == 0)
                    {
                        errText = "处理组套项目出错！" + errText;
                        return null;
                    }
                    feeItemLists.AddRange(alDetail);
                }
                else
                {
                    feeItemLists.Add(f);
                }

            }
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                FeeItemList f = feeItemLists[i] as FeeItemList;
                if (hsDROnlyOneItem.ContainsKey(f.Item.ID))
                {
                    feeItemLists.RemoveAt(i);
                }
                if (hsCTOnlyOneItem.ContainsKey(f.Item.ID))
                {
                    if (hsCTOnlyOneItem[f.Item.ID].ToString() != "true")
                    {
                        hsCTOnlyOneItem.Remove(f.Item.ID);
                        hsCTOnlyOneItem.Add(f.Item.ID, "true");
                    }
                    else
                    {
                        feeItemLists.RemoveAt(i);
                    }
                }
            }
            foreach (DictionaryEntry de in hsDROnlyOneItem)
            {
                FeeItemList f = de.Value as FeeItemList;
                feeItemLists.Add(f);
            }
            int number = 1;
            int returnRows = 0;//是否为限制收费药品
            decimal LimitNumber = 1;
            ArrayList hsREOnlylistItem = new ArrayList();
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                string Discount_type = "1";//限制收费类型
                decimal TOPPRICE = 0;
                decimal DISCOUNT_RATE = 0;
                FeeItemList s = feeItemLists[i] as FeeItemList;
                if (this.rInfo.DoctorInfo.Templet.Dept.ID == "1026" || this.rInfo.DoctorInfo.Templet.Dept.ID == "6018")
                {
                    if (this.db.ISDFSitemfee(s.Item.ID, s.Item.Qty) == 0)
                    {
                        errText = "本次收费包含折价项目，请到窗口缴费!";
                        return null;
                    }
                }
                returnRows = this.db.SetRestrictingfee(s.Item.ID, ref  LimitNumber);
                Discount_type = this.db.SetDiscountfee(s.Item.ID, ref  DISCOUNT_RATE, ref  TOPPRICE);
                if (returnRows > 0 && this.rInfo.DoctorInfo.Templet.Dept.ID != "7021")
                {
                    this.setRestrictingfee.ConvertRestrictingfee(rInfo.PID.CardNO, s, ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem, number, LimitNumber);
                }
                if (Discount_type == "2")
                {
                    this.setRestrictingfee.ConvertDiscountfee(s, DISCOUNT_RATE, TOPPRICE, ref hsREOnlyOneItem, ref hsREOnlylistItem, number);
                }
                number++;
            }
            number = 1;
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                FeeItemList s = feeItemLists[i] as FeeItemList;
                if (hsREOnlyOneItem.ContainsKey(s.Item.ID + number))
                {
                    feeItemLists.RemoveAt(i);
                }
                number++;
            }
            foreach (FeeItemList ds in hsREOnlylistItem)
            {
                feeItemLists.Add(ds);
            }
            return feeItemLists;
        }

        /// <summary>
        /// 获得收费信息自助机修改
        /// </summary>
        /// <param name="clincCode"></param>
        /// <param name="feeItemList"></param>
        /// <returns></returns>
        public ArrayList GetFeeItemListnew(string clincCode, ArrayList feeArryList)
        {
            bool isFindDRFirst = false;
            bool isFindCTFirst = false;
            Hashtable hsDROnlyOneItem = new Hashtable();
            Hashtable hsCTOnlyOneItem = new Hashtable();
            ArrayList hsNOREOnlyOneItem = new ArrayList();
            ArrayList hsZTNOREOnlyOneItem = new ArrayList();
            Hashtable hsREOnlyOneItem = new Hashtable();
            decimal drCount = 0;
            ArrayList feeItemLists = new ArrayList();
            Hashtable hsDoct = new Hashtable();
            this.rInfo = this.db.GetByClinic(clincCode);
            string tempPayKindid = this.rInfo.Pact.PayKind.ID;
            this.rInfo.Pact = this.db.GetPactUnitInfoByPactCode(this.rInfo.Pact.ID);
            this.rInfo.Pact.PayKind.ID = tempPayKindid;
            this.db.QueryItemList(deptCode, Neusoft.HISFC.Models.Base.ItemKind.All, ref dsItem);
            for (int i = 0; i < feeArryList.Count; i++)
            {
                if (feeArryList[i] == null || !(feeArryList[i] is FeeItemList))
                {
                    continue;
                }
                var f = feeArryList[i] as FeeItemList;
                if (!string.IsNullOrEmpty(f.RecipeOper.ID) && string.IsNullOrEmpty(f.DoctDeptInfo.ID))
                {
                    if (hsDoct.ContainsKey(f.RecipeOper.ID))
                    {
                        f.DoctDeptInfo.ID = hsDoct[f.RecipeOper.ID].ToString();
                    }
                    else
                    {
                        var emplInfo = this.db.GetEmployeeInfoForEmplCode(f.RecipeOper.ID);
                        if (emplInfo != null && !string.IsNullOrEmpty(emplInfo.DEPT_CODE))
                        {
                            f.DoctDeptInfo.ID = emplInfo.DEPT_CODE;
                            hsDoct.Add(f.RecipeOper.ID, emplInfo.DEPT_CODE);
                        }
                    }
                }
                if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug && IsUseCtOrMRfeeRule && this.db.IsZTUndrugInfo(f.Item.ID))
                {
                    ArrayList alDetail = null;

                    var undrugInfo = this.db.GetUndrugByCode(f.Item.ID);
                    f.Item.NeedConfirm = undrugInfo.NeedConfirm;
                    var hsItemZT = this.GetCTMRHashtabel();
                    if (hsItemZT.ContainsKey(f.Item.ID))
                    {
                        ArrayList alItem = (ArrayList)hsItemZT[f.Item.ID];
                        string type = (alItem[0] as NeuObject).User02;
                        if (type == "DR")
                        {
                            alDetail = ConvertDRGroupToDetail(f, !isFindDRFirst, ref hsDROnlyOneItem, ref drCount);
                            isFindDRFirst = true;
                        }
                        else if (type == "CT")
                        {
                            alDetail = ConvertCTGroupToDetail(f, !isFindCTFirst, ref hsCTOnlyOneItem);
                            isFindCTFirst = true;
                        }
                    }
                    else
                    {
                        alDetail = ConvertGroupToDetail(f);
                    }

                    if (alDetail == null)
                    {
                        errText = "获得组套明细出错!" + errText;
                        return null;
                    }

                    if (alDetail.Count == 0)
                    {
                        errText = "处理组套项目出错！" + errText;
                        return null;
                    }
                    //传自助机时组套项目将计算完优惠价格在汇总给组套价格
                    decimal sumTotCost = 0;
                    decimal sumOwnCost = 0;
                    for (int e = 0; e < alDetail.Count; e++)
                    {
                        FeeItemList s = alDetail[e] as FeeItemList;
                        sumOwnCost += s.FT.OwnCost;
                        sumTotCost += s.FT.TotCost;
                    }
                    f.FT.OwnCost = sumOwnCost;
                    f.FT.TotCost = sumTotCost;
                    feeItemLists.Add(f);
                    //feeItemLists.AddRange(alDetail);

                }
                else
                {
                    feeItemLists.Add(f);
                }

            }
            //int returnRows = 0;//是否为限制收费药品
            //decimal LimitNumber = 1;
            ////限制药品收费
            //int number = 1;
            //ArrayList hsREOnlylistItem = new ArrayList();
            //for (int i = feeItemLists.Count - 1; i >= 0; i--)
            //{
            //    FeeItemList s = feeItemLists[i] as FeeItemList;
            //    returnRows = this.db.SetRestrictingfee(s.Item.ID, ref  LimitNumber);
            //    if (returnRows > 0)
            //    {
            //        this.setRestrictingfee.ConvertRestrictingfeeCharge(rInfo.PID.CardNO, s, ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem, number, LimitNumber, ref hsZTNOREOnlyOneItem, this.dsItem, this.rInfo);
            //    }
            //    number++;
            //}
            //number = 1;
            //for (int i = feeItemLists.Count - 1; i >= 0; i--)
            //{
            //    FeeItemList s = feeItemLists[i] as FeeItemList;
            //    if (hsREOnlyOneItem.ContainsKey(s.Item.ID + number))
            //    {
            //        feeItemLists.RemoveAt(i);
            //    }
            //    number++;
            //}
            //foreach (FeeItemList ds in hsREOnlylistItem)
            //{
            //    feeItemLists.Add(ds);
            //}



            return feeItemLists;
        }
        private ArrayList ConvertDRGroupToDetail(FeeItemList f, bool isFirst, ref Hashtable hsOnlyOneItem, ref decimal drCount)
        {
            ArrayList undrugCombList = this.db.QueryUndrugZTBypackageCode(f.Item.ID);
            ArrayList alTemp = new ArrayList();
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + db.Err;

                return null;
            }
            decimal price = 0;
            decimal priceSecond = 0; // {C41CAC71-0186-43cf-9167-2D33E4626D74}
            decimal count = 0;
            string feeCode = string.Empty;
            string itemType = string.Empty;
            decimal totCost = 0;
            FeeItemList feeDetail = null;
            if (f.Order.ID == null || f.Order.ID == string.Empty)
            {
                f.Order.ID = this.db.GetNewOrderID();
                if (f.Order.ID == null || f.Order.ID == string.Empty)
                {
                    this.errText = "获得医嘱流水号出错!";

                    return null;
                }
            }

            //有价格打折的
            DataRow rowFind;
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                this.errText = "查找组套明细出错!";
                return null;
            }
            rowFind = rowFinds[0];

            DateTime nowTime = this.db.GetDateTimeFromSysDateTime();
            int age = 0;
            int month = 0;
            int day = 0;
            this.db.GetAge(this.rInfo.Birthday, nowTime, ref age, ref month, ref day);

            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
            string priceForm = this.rInfo.Pact.PriceForm;

            decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
            decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
            decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
            decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);

            decimal orgGroupPrice = 0;
            decimal priceGroup = this.db.GetPrice(f.Item.ID, this.rInfo, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup, ref orgGroupPrice);

           // decimal rate = f.Item.Price / orgGroupPrice;
           // if (rate == 1)
          //  {
           //     rate = priceGroup / orgGroupPrice;
          //  }

            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                if (isFirst && undrugCombo.SortID == 2)
                {
                    //如果是第一个DR项目，并且细项是第二组起收的继续循环
                    continue;
                }
                else if (!isFirst && undrugCombo.SortID == 1)
                {
                    //如果不是第一个DR项目，并且细项是第一组收的继续循环
                    continue;
                }
                if (undrugCombo.SpellCode != "0")
                {
                    DataRow rowFindZT;
                    DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                    rowFindZT = rowFindZTs[0];
                    string itemName = rowFindZT["ITEM_NAME"].ToString();
                    if (itemName.ToUpper().Contains("DR"))
                    {
                        drCount += NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;
                    }
                }
            }

            //符合项目明细的加成（减免）比例
            decimal itemRate = 1;
            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                if (isFirst && undrugCombo.SortID == 2)
                {
                    //如果是第一个DR项目，并且细项是第二组起收的继续循环
                    continue;
                }
                else if (!isFirst && undrugCombo.SortID == 1)
                {
                    //如果不是第一个DR项目，并且细项是第一组收的继续循环
                    continue;
                }
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    this.errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                feeDetail = new FeeItemList();

                feeCode = rowFindZT["FEE_CODE"].ToString();
                try
                {
                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;

                    decimal orgPrice = price;
                    itemRate = db.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                    price = this.db.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                    feeDetail.OrgPrice = orgPrice;
                }
                catch (Exception e)
                {
                    this.errText = e.Message;

                    return null;
                }

                //组合项目原本就有打折的
                //if (rate > 0)
                //{
                //    price *= rate;
                //}

                //根据优惠比例重新计算单价------------------------- 
                string errMsg = string.Empty;
                PactItemRate myRate = this.PactRate(this.rInfo, feeDetail, ref errMsg);
                if (myRate == null)
                {
                    this.errText = errMsg;
                    return null;
                }

                price *= 1 - myRate.Rate.RebateRate;
                //--------------------------------------------------
                count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

                //组套拆分成明细的时候，也保存两位小数
                //totCost = price * count;
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

                feeDetail.Patient = f.Patient.Clone();
                feeDetail.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
                feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
                feeDetail.Name = feeDetail.Item.Name;
                feeDetail.ID = feeDetail.Item.ID;
                itemType = rowFindZT["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "1")
                {
                    //feeDetail.Item.IsPharmacy = true;
                    feeDetail.Item.ItemType = EnumItemType.Drug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "2")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = true;
                }
                feeDetail.RecipeOper = f.RecipeOper.Clone();
                feeDetail.Item.Price = price;
                feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
                feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
                feeDetail.Item.MinFee.ID = feeCode;
                feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
                feeDetail.Item.Qty = count;
                feeDetail.Days = NConvert.ToDecimal(f.Days);
                feeDetail.FT.TotCost = totCost;
                //自费如此，如果加上公费需要重新计算!!!
                feeDetail.FT.OwnCost = totCost;
                feeDetail.ExecOper = f.ExecOper.Clone();
                feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
                //if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
                //{
                //    feeDetail.Item.IsNeedConfirm = true;
                //}
                //else
                //{
                //    feeDetail.Item.IsNeedConfirm = false;
                //}

                //feeDetail.Item.NeedConfirm = f.Item.NeedConfirm;

                if (string.IsNullOrEmpty(rowFindZT["CONFIRM_FLAG"].ToString()))
                {
                    feeDetail.Item.NeedConfirm = EnumNeedConfirm.None;
                }
                else
                {
                    if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(rowFindZT["CONFIRM_FLAG"].ToString())))
                    {
                        feeDetail.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFindZT["CONFIRM_FLAG"].ToString());
                    }
                }

                feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

                feeDetail.Order.ID = f.Order.ID;

                feeDetail.UndrugComb.ID = f.Item.ID;
                feeDetail.UndrugComb.Name = f.Item.Name;
                feeDetail.UndrugComb.Qty = f.Item.Qty;

                feeDetail.Order.Combo.ID = f.Order.Combo.ID;
                feeDetail.Item.IsMaterial = f.Item.IsMaterial;
                feeDetail.RecipeSequence = f.RecipeSequence;
                feeDetail.FTSource = f.FTSource;
                feeDetail.FeePack = f.FeePack;
                if (this.rInfo.Pact.PayKind.ID == "03")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    if (pactRate == null)
                    {
                        pactRate = this.db.GetOnepPactUnitItemRateByItem(this.rInfo.Pact.ID, feeDetail.Item.ID);
                    }
                    if (pactRate != null)
                    {
                        if (pactRate.Rate.PayRate != this.rInfo.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeDetail.ItemRateFlag = "1";
                            }
                            else
                            {
                                //feeDetail.ItemRateFlag = "3";
                                feeDetail.ItemRateFlag = "2";
                            }
                        }
                        else
                        {
                            feeDetail.ItemRateFlag = "2";

                        }
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            //feeDetail.ItemRateFlag = "2";//DEL 30
                            feeDetail.ItemRateFlag = "3";
                        }
                    }
                    else
                    {
                        if (f.ItemRateFlag == "3")
                        {
                            //DEL 30
                            ////if (rowFindZT["ZF"].ToString() != "1")
                            ////{
                            ////    feeDetail.OrgItemRate = f.OrgItemRate;
                            ////    feeDetail.NewItemRate = f.NewItemRate;
                            ////    feeDetail.ItemRateFlag = "2";
                            ////}
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = f.ItemRateFlag;
                        }
                    }
                }

                //复合项目的用法赋给明细项目
                feeDetail.Order.Usage = f.Order.Usage;
                //使用原来的处方号
                //feeDetail.RecipeNO = f.RecipeNO;
                feeDetail.Order.ApplyNo = f.Order.ApplyNo;
                feeDetail.Order.Sample.ID = f.Order.Sample.ID;
                feeDetail.Order.Sample.Name = f.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

                if (undrugCombo.SpellCode == "0")
                {
                    //总量取整的，做标识
                    if (hsOnlyOneItem.ContainsKey(feeDetail.Item.ID))
                    {
                        FeeItemList temp = hsOnlyOneItem[feeDetail.Item.ID] as FeeItemList;
                        //temp.UndrugComb.User02 = (Neusoft.FrameWork.Function.NConvert.ToInt32(temp.UndrugComb.User02) + 1).ToString();
                        //if (Neusoft.FrameWork.Function.NConvert.ToInt32(temp.UndrugComb.User02) % 2 != 0)
                        //{
                        //    temp.Item.Qty += feeDetail.Item.Qty;
                        //    temp.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(temp.Item.Qty));
                        //    temp.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(temp.Item.Price * temp.Item.Qty, 2);
                        //    temp.FT.OwnCost = temp.FT.TotCost;
                        //}
                        //temp.Item.Qty += feeDetail.Item.Qty;
                        //temp.FT.TotCost += feeDetail.FT.TotCost;
                        //temp.FT.OwnCost += feeDetail.FT.OwnCost;

                        temp.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(drCount / 2));
                        temp.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(temp.Item.Price * temp.Item.Qty, 2);
                        temp.FT.OwnCost = temp.FT.TotCost;
                    }
                    else
                    {
                        //feeDetail.UndrugComb.User02 = "1";

                        feeDetail.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(Math.Ceiling(drCount / 2));
                        feeDetail.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(feeDetail.Item.Price * feeDetail.Item.Qty, 2);
                        feeDetail.FT.OwnCost = feeDetail.FT.TotCost;

                        hsOnlyOneItem.Add(feeDetail.Item.ID, feeDetail);
                    }
                }

                alTemp.Add(feeDetail);
            }
            if (alTemp.Count > 0)
            {
                if (f.FT.RebateCost > 0)//有减免
                {
                    if (this.rInfo.Pact.PayKind.ID != "01")
                    {
                        this.errText = "暂时不允许非自费患者减免";
                        return null;
                    }
                    //decimal rebateRate =
                    //    Neusoft.FrameWork.Public.String.FormatNumber(
                    //    f.FT.RebateCost / (f.FT.OwnCost + f.FT.RebateCost), 2);
                    //decimal tempFix = 0;
                    //decimal tempRebateCost = 0;
                    //foreach (FeeItemList feeTemp in alTemp)
                    //{
                    //    feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost + feeTemp.FT.RebateCost) * rebateRate;
                    //    tempRebateCost += feeTemp.FT.RebateCost;
                    //    feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                    //    feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    //}
                    //tempFix = f.FT.RebateCost - tempRebateCost;
                    //FeeItemList fFix = alTemp[0] as FeeItemList;
                    //fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                    //减免单独算
                    decimal rebateRate =
                        Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
                    decimal tempFix = 0;
                    decimal tempRebateCost = 0;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
                        tempRebateCost += feeTemp.FT.RebateCost;
                        //feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                        //feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    }
                    tempFix = f.FT.RebateCost - tempRebateCost;
                    FeeItemList fFix = alTemp[0] as FeeItemList;
                    fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                }
            }
            if (alTemp.Count > 0)
            {
                if (f.SpecialPrice > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.SpecialPrice = f.SpecialPrice;

                            break;
                        }
                    }
                }
            }
            if (alTemp.Count > 0)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.FT.User03 = f.FT.User03;

                            break;
                        }
                    }
                }
            }
            return alTemp;
        }

        /// <summary>
        /// 把组套拆分成明细
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private ArrayList ConvertCTGroupToDetail(FeeItemList f, bool isFirst, ref Hashtable hsOnlyOneItem)
        {
            ArrayList undrugCombList = this.db.QueryUndrugZTBypackageCode(f.Item.ID);
            ArrayList alTemp = new ArrayList();
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + db.Err;

                return null;
            }
            decimal price = 0;
            decimal priceSecond = 0; // {C41CAC71-0186-43cf-9167-2D33E4626D74}
            decimal count = 0;
            string feeCode = string.Empty;
            string itemType = string.Empty;
            decimal totCost = 0;
            FeeItemList feeDetail = null;
            if (f.Order.ID == null || f.Order.ID == string.Empty)
            {
                f.Order.ID = this.db.GetNewOrderID();
                if (f.Order.ID == null || f.Order.ID == string.Empty)
                {
                    this.errText = "获得医嘱流水号出错!";

                    return null;
                }
            }

            //有价格打折的
            DataRow rowFind;
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                this.errText = "查找组套明细出错!";
                return null;
            }
            rowFind = rowFinds[0];

            DateTime nowTime = this.db.GetDateTimeFromSysDateTime();
            int age = 0;
            int month = 0;
            int day = 0;
            this.db.GetAge(this.rInfo.Birthday, nowTime, ref age, ref month, ref day);

            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
            string priceForm = this.rInfo.Pact.PriceForm;

            decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
            decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
            decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
            decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);

            decimal orgGroupPrice = 0;
            decimal priceGroup = this.db.GetPrice(f.Item.ID, this.rInfo, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup, ref orgGroupPrice);

            decimal rate = f.Item.Price / orgGroupPrice;
            if (rate == 1)
            {
                rate = priceGroup / orgGroupPrice;
            }

            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    this.errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                #region pacs项目收费新模式

                if (undrugCombo.SortID == 3)
                {
                    if (hsOnlyOneItem.ContainsKey(undrugCombo.ID))
                    {
                        continue;
                    }
                    else
                    {
                        string itemName = rowFindZT["ITEM_NAME"].ToString();
                        if (itemName.Contains("三维重建"))
                        {
                            if (!hsOnlyOneItem.ContainsValue("四维"))
                            {
                                hsOnlyOneItem.Add(undrugCombo.ID, "三维");
                            }
                            else
                            {
                                hsOnlyOneItem.Add(undrugCombo.ID, "true");
                            }
                        }
                        else if (itemName.Contains("四维重建"))
                        {
                            Hashtable hsTemp = hsOnlyOneItem.Clone() as Hashtable;
                            foreach (DictionaryEntry de in hsTemp)
                            {
                                if (de.Value.ToString() == "三维")
                                {
                                    hsOnlyOneItem.Remove(de.Key);
                                    hsOnlyOneItem.Add(de.Key.ToString(), "true");
                                }
                            }
                            hsOnlyOneItem.Add(undrugCombo.ID, "四维");
                        }
                        else
                        {
                            hsOnlyOneItem.Add(undrugCombo.ID, "其他");
                        }
                    }
                }

                #endregion
            }

            //符合项目明细的加成（减免）比例
            decimal itemRate = 1;
            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                //if (undrugCombo.SortID == 3)
                //{
                //    if (hsOnlyOneItem.ContainsKey(undrugCombo.ID))
                //    {
                //        if (hsOnlyOneItem[undrugCombo.ID].ToString() != "true")
                //        {
                //            hsOnlyOneItem.Remove(undrugCombo.ID);
                //            hsOnlyOneItem.Add(undrugCombo.ID, "true");
                //        }
                //        else
                //        {
                //            continue;
                //        }
                //    }
                //    else
                //    {
                //        continue;
                //    }
                //}
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    this.errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                feeDetail = new FeeItemList();

                feeCode = rowFindZT["FEE_CODE"].ToString();
                try
                {
                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;

                    decimal orgPrice = price;
                    itemRate = db.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                    price = this.db.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                    feeDetail.OrgPrice = orgPrice;
                }
                catch (Exception e)
                {
                    this.errText = e.Message;

                    return null;
                }

                //组合项目原本就有打折的
                //if (rate > 0)
                //{
                //    price *= rate;
                //}

                //根据优惠比例重新计算单价------------------------- 
                string errMsg = string.Empty;
                PactItemRate myRate = this.PactRate(this.rInfo, feeDetail, ref errMsg);
                if (myRate == null)
                {
                    this.errText = errMsg;
                    return null;
                }

                price *= 1 - myRate.Rate.RebateRate;
                //--------------------------------------------------
                count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

                //组套拆分成明细的时候，也保存两位小数
                //totCost = price * count;
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

                feeDetail.Patient = f.Patient.Clone();
                feeDetail.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
                feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
                feeDetail.Name = feeDetail.Item.Name;
                feeDetail.ID = feeDetail.Item.ID;
                itemType = rowFindZT["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "1")
                {
                    //feeDetail.Item.IsPharmacy = true;
                    feeDetail.Item.ItemType = EnumItemType.Drug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "2")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = true;
                }
                feeDetail.RecipeOper = f.RecipeOper.Clone();
                feeDetail.Item.Price = price;
                feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
                feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
                feeDetail.Item.MinFee.ID = feeCode;
                feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
                feeDetail.Item.Qty = count;
                feeDetail.Days = NConvert.ToDecimal(f.Days);
                feeDetail.FT.TotCost = totCost;
                //自费如此，如果加上公费需要重新计算!!!
                feeDetail.FT.OwnCost = totCost;
                feeDetail.ExecOper = f.ExecOper.Clone();
                feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
                //if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
                //{
                //    feeDetail.Item.IsNeedConfirm = true;
                //}
                //else
                //{
                //    feeDetail.Item.IsNeedConfirm = false;
                //}

                //feeDetail.Item.NeedConfirm = f.Item.NeedConfirm;

                if (string.IsNullOrEmpty(rowFindZT["CONFIRM_FLAG"].ToString()))
                {
                    feeDetail.Item.NeedConfirm = EnumNeedConfirm.None;
                }
                else
                {
                    if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(rowFindZT["CONFIRM_FLAG"].ToString())))
                    {
                        feeDetail.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFindZT["CONFIRM_FLAG"].ToString());
                    }
                }

                feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

                feeDetail.Order.ID = f.Order.ID;

                feeDetail.UndrugComb.ID = f.Item.ID;
                feeDetail.UndrugComb.Name = f.Item.Name;
                feeDetail.UndrugComb.Qty = f.Item.Qty;

                feeDetail.Order.Combo.ID = f.Order.Combo.ID;
                feeDetail.Item.IsMaterial = f.Item.IsMaterial;
                feeDetail.RecipeSequence = f.RecipeSequence;
                feeDetail.FTSource = f.FTSource;
                feeDetail.FeePack = f.FeePack;
                if (this.rInfo.Pact.PayKind.ID == "03")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    if (pactRate == null)
                    {
                        pactRate = this.db.GetOnepPactUnitItemRateByItem(this.rInfo.Pact.ID, feeDetail.Item.ID);
                    }
                    if (pactRate != null)
                    {
                        if (pactRate.Rate.PayRate != this.rInfo.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeDetail.ItemRateFlag = "1";
                            }
                            else
                            {
                                //feeDetail.ItemRateFlag = "3";
                                feeDetail.ItemRateFlag = "2";
                            }
                        }
                        else
                        {
                            feeDetail.ItemRateFlag = "2";

                        }
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            //feeDetail.ItemRateFlag = "2";//DEL 30
                            feeDetail.ItemRateFlag = "3";
                        }
                    }
                    else
                    {
                        if (f.ItemRateFlag == "3")
                        {
                            //DEL 30
                            ////if (rowFindZT["ZF"].ToString() != "1")
                            ////{
                            ////    feeDetail.OrgItemRate = f.OrgItemRate;
                            ////    feeDetail.NewItemRate = f.NewItemRate;
                            ////    feeDetail.ItemRateFlag = "2";
                            ////}
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = f.ItemRateFlag;
                        }
                    }
                }

                //复合项目的用法赋给明细项目
                feeDetail.Order.Usage = f.Order.Usage;
                //使用原来的处方号
                //feeDetail.RecipeNO = f.RecipeNO;
                feeDetail.Order.ApplyNo = f.Order.ApplyNo;
                feeDetail.Order.Sample.ID = f.Order.Sample.ID;
                feeDetail.Order.Sample.Name = f.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

                alTemp.Add(feeDetail);
            }
            if (alTemp.Count > 0)
            {
                if (f.FT.RebateCost > 0)//有减免
                {
                    if (this.rInfo.Pact.PayKind.ID != "01")
                    {
                        this.errText = "暂时不允许非自费患者减免!";
                        return null;
                    }
                    //decimal rebateRate =
                    //    Neusoft.FrameWork.Public.String.FormatNumber(
                    //    f.FT.RebateCost / (f.FT.OwnCost + f.FT.RebateCost), 2);
                    //decimal tempFix = 0;
                    //decimal tempRebateCost = 0;
                    //foreach (FeeItemList feeTemp in alTemp)
                    //{
                    //    feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost + feeTemp.FT.RebateCost) * rebateRate;
                    //    tempRebateCost += feeTemp.FT.RebateCost;
                    //    feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                    //    feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    //}
                    //tempFix = f.FT.RebateCost - tempRebateCost;
                    //FeeItemList fFix = alTemp[0] as FeeItemList;
                    //fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                    //减免单独算
                    decimal rebateRate =
                        Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
                    decimal tempFix = 0;
                    decimal tempRebateCost = 0;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
                        tempRebateCost += feeTemp.FT.RebateCost;
                        //feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                        //feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    }
                    tempFix = f.FT.RebateCost - tempRebateCost;
                    FeeItemList fFix = alTemp[0] as FeeItemList;
                    fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                }
            }
            if (alTemp.Count > 0)
            {
                if (f.SpecialPrice > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.SpecialPrice = f.SpecialPrice;

                            break;
                        }
                    }
                }
            }
            if (alTemp.Count > 0)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.FT.User03 = f.FT.User03;

                            break;
                        }
                    }
                }
            }
            return alTemp;
        }

        private ArrayList ConvertGroupToDetail(FeeItemList f)
        {
            ArrayList undrugCombList = this.db.QueryUndrugPackagesBypackageCode(f.Item.ID);
            ArrayList alTemp = new ArrayList();
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + this.db.Err;

                return null;
            }
            decimal price = 0;
            decimal priceSecond = 0; // {C41CAC71-0186-43cf-9167-2D33E4626D74}
            decimal count = 0;
            string feeCode = string.Empty;
            string itemType = string.Empty;
            decimal totCost = 0;
            FeeItemList feeDetail = null;
            if (f.Order.ID == null || f.Order.ID == string.Empty)
            {
                f.Order.ID = this.db.GetNewOrderID();
                if (f.Order.ID == null || f.Order.ID == string.Empty)
                {
                    this.errText = "获得医嘱流水号出错!";

                    return null;
                }
            }

            //有价格打折的
            DataRow rowFind;
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                this.errText = "查找组套明细出错!";
                return null;
            }
            rowFind = rowFinds[0];

            DateTime nowTime = this.db.GetDateTimeFromSysDateTime();
            int age = 0;
            int month = 0;
            int day = 0;
            this.db.GetAge(this.rInfo.Birthday, nowTime, ref age, ref month, ref day);

            //{B9303CFE-755D-4585-B5EE-8C1901F79450}增加获取购入价
            string priceForm = this.rInfo.Pact.PriceForm;

            decimal unitPriceGroup = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
            decimal childPriceGroup = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
            decimal SPPriceGroup = NConvert.ToDecimal(rowFind["SP_PRICE"]);
            decimal purchasePriceGroup = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);

            decimal orgGroupPrice = 0;
            decimal priceGroup = this.db.GetPrice(f.Item.ID, this.rInfo, unitPriceGroup, childPriceGroup, SPPriceGroup, purchasePriceGroup, ref orgGroupPrice);

            //decimal rate = f.Item.Price / orgGroupPrice;
            //if (rate == 1)
            //{
            //    rate = priceGroup / orgGroupPrice;
            //} //因后续未使用并此处导致0元项目异常特注释

            //符合项目明细的加成（减免）比例
            decimal itemRate = 1;
            foreach (Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugCombo in undrugCombList)
            {
                DataRow rowFindZT;
                DataRow[] rowFindZTs = dsItem.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                if (rowFindZTs == null || rowFindZTs.Length == 0)
                {
                    this.errText = "查找组套明细出错!";

                    continue;
                }
                rowFindZT = rowFindZTs[0];

                feeDetail = new FeeItemList();

                feeCode = rowFindZT["FEE_CODE"].ToString();
                try
                {

                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;

                    if (isTransferTreat == true)
                    {
                        decimal orgPrice = price;
                        itemRate = 1;// feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                        price = unitPrice;// this.feeIntegrate.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                        feeDetail.OrgPrice = orgPrice;
                    }
                    else
                    {
                        decimal orgPrice = price;
                        itemRate = this.db.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                        price = this.db.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                        feeDetail.OrgPrice = orgPrice;
                    }
                }
                catch (Exception e)
                {
                    this.errText = e.Message;

                    return null;
                }

                //组合项目原本就有打折的
                //中五打折不需要用计算的rate
                //if (rate > 0)
                //{
                //    price *= rate;
                //}

                //根据优惠比例重新计算单价------------------------- 
                string errMsg = string.Empty;
                PactItemRate myRate = this.PactRate(this.rInfo, feeDetail, ref errMsg);
                if (myRate == null)
                {
                    this.errText = errMsg;
                    return null;
                }

                price *= 1 - myRate.Rate.RebateRate;
                //--------------------------------------------------
                count = NConvert.ToDecimal(f.Item.Qty) * undrugCombo.Qty;

                //组套拆分成明细的时候，也保存两位小数
                //totCost = price * count;
                totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

                feeDetail.Patient = f.Patient.Clone();
                feeDetail.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
                feeDetail.Item.ID = rowFindZT["ITEM_CODE"].ToString();
                feeDetail.Item.Name = rowFindZT["ITEM_NAME"].ToString();
                feeDetail.Name = feeDetail.Item.Name;
                feeDetail.ID = feeDetail.Item.ID;
                itemType = rowFindZT["DRUG_FLAG"].ToString();
                if (itemType == "0")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "1")
                {
                    //feeDetail.Item.IsPharmacy = true;
                    feeDetail.Item.ItemType = EnumItemType.Drug;
                    feeDetail.IsGroup = false;
                }
                if (itemType == "2")
                {
                    //feeDetail.Item.IsPharmacy = false;
                    feeDetail.Item.ItemType = EnumItemType.UnDrug;
                    feeDetail.IsGroup = true;
                }
                feeDetail.RecipeOper = f.RecipeOper.Clone();
                feeDetail.Item.Price = price;
                feeDetail.Item.Specs = rowFindZT["SPECS"].ToString();
                feeDetail.Item.SysClass.ID = rowFindZT["SYS_CLASS"].ToString();
                feeDetail.Item.MinFee.ID = feeCode;
                feeDetail.Item.PackQty = NConvert.ToDecimal(rowFindZT["PACK_QTY"].ToString());
                feeDetail.Item.Qty = count;
                feeDetail.Days = NConvert.ToDecimal(f.Days);
                feeDetail.FT.TotCost = totCost;
                //自费如此，如果加上公费需要重新计算!!!
                feeDetail.FT.OwnCost = totCost;
                feeDetail.ExecOper = f.ExecOper.Clone();
                feeDetail.Item.PriceUnit = rowFindZT["MIN_UNIT"].ToString() == string.Empty ? "次" : rowFindZT["MIN_UNIT"].ToString();
                //if (rowFindZT["CONFIRM_FLAG"].ToString() == "2" || rowFindZT["CONFIRM_FLAG"].ToString() == "3" || rowFindZT["CONFIRM_FLAG"].ToString() == "1")
                //{
                //    feeDetail.Item.IsNeedConfirm = true;
                //}
                //else
                //{
                //    feeDetail.Item.IsNeedConfirm = false;
                //}

                //feeDetail.Item.NeedConfirm = f.Item.NeedConfirm;

                if (string.IsNullOrEmpty(rowFindZT["CONFIRM_FLAG"].ToString()))
                {
                    feeDetail.Item.NeedConfirm = EnumNeedConfirm.None;
                }
                else
                {
                    if (Enum.IsDefined(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm),
                        Neusoft.FrameWork.Function.NConvert.ToInt32(rowFindZT["CONFIRM_FLAG"].ToString())))
                    {
                        feeDetail.Item.NeedConfirm = (Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm)Enum.Parse(typeof(Neusoft.HISFC.Models.Fee.Item.EnumNeedConfirm), rowFindZT["CONFIRM_FLAG"].ToString());
                    }
                }

                feeDetail.Item.IsNeedBespeak = NConvert.ToBoolean(rowFindZT["NEEDBESPEAK"].ToString());

                feeDetail.Order.ID = f.Order.ID;

                feeDetail.UndrugComb.ID = f.Item.ID;
                feeDetail.UndrugComb.Name = f.Item.Name;
                feeDetail.UndrugComb.Qty = f.Item.Qty;

                feeDetail.Order.Combo.ID = f.Order.Combo.ID;
                feeDetail.Item.IsMaterial = f.Item.IsMaterial;
                feeDetail.RecipeSequence = f.RecipeSequence;
                feeDetail.FTSource = f.FTSource;
                feeDetail.FeePack = f.FeePack;
                if (this.rInfo.Pact.PayKind.ID == "03")
                {
                    Neusoft.HISFC.Models.Base.PactItemRate pactRate = null;

                    if (pactRate == null)
                    {
                        pactRate = this.db.GetOnepPactUnitItemRateByItem(this.rInfo.Pact.ID, feeDetail.Item.ID);
                    }
                    if (pactRate != null)
                    {
                        if (pactRate.Rate.PayRate != this.rInfo.Pact.Rate.PayRate)
                        {
                            if (pactRate.Rate.PayRate == 1)//自费
                            {
                                feeDetail.ItemRateFlag = "1";
                            }
                            else
                            {
                                //feeDetail.ItemRateFlag = "3";
                                feeDetail.ItemRateFlag = "2";
                            }
                        }
                        else
                        {
                            feeDetail.ItemRateFlag = "2";

                        }
                        if (f.ItemRateFlag == "3")
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            //feeDetail.ItemRateFlag = "2";//DEL 30
                            feeDetail.ItemRateFlag = "3";
                        }
                    }
                    else
                    {
                        if (f.ItemRateFlag == "3")
                        {
                            //DEL 30
                            ////if (rowFindZT["ZF"].ToString() != "1")
                            ////{
                            ////    feeDetail.OrgItemRate = f.OrgItemRate;
                            ////    feeDetail.NewItemRate = f.NewItemRate;
                            ////    feeDetail.ItemRateFlag = "2";
                            ////}
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeDetail.OrgItemRate = f.OrgItemRate;
                            feeDetail.NewItemRate = f.NewItemRate;
                            feeDetail.ItemRateFlag = f.ItemRateFlag;
                        }
                    }
                }

                //复合项目的用法赋给明细项目
                feeDetail.Order.Usage = f.Order.Usage;
                //使用原来的处方号
                //feeDetail.RecipeNO = f.RecipeNO;
                feeDetail.Order.ApplyNo = f.Order.ApplyNo;
                feeDetail.Order.Sample.ID = f.Order.Sample.ID;
                feeDetail.Order.Sample.Name = f.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = f.Order.CheckPartRecord;

                alTemp.Add(feeDetail);
            }
            if (alTemp.Count > 0)
            {
                if (f.FT.RebateCost > 0)//有减免
                {
                    if (this.rInfo.Pact.PayKind.ID != "01")
                    {
                        this.errText = "暂时不允许非自费患者减免";
                        return null;
                    }
                    //decimal rebateRate =
                    //    Neusoft.FrameWork.Public.String.FormatNumber(
                    //    f.FT.RebateCost / (f.FT.OwnCost + f.FT.RebateCost), 2);
                    //decimal tempFix = 0;
                    //decimal tempRebateCost = 0;
                    //foreach (FeeItemList feeTemp in alTemp)
                    //{
                    //    feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost + feeTemp.FT.RebateCost) * rebateRate;
                    //    tempRebateCost += feeTemp.FT.RebateCost;
                    //    feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                    //    feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    //}
                    //tempFix = f.FT.RebateCost - tempRebateCost;
                    //FeeItemList fFix = alTemp[0] as FeeItemList;
                    //fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                    //减免单独算
                    decimal rebateRate =
                        Neusoft.FrameWork.Public.String.FormatNumber(f.FT.RebateCost / f.FT.OwnCost, 2);
                    decimal tempFix = 0;
                    decimal tempRebateCost = 0;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        feeTemp.FT.RebateCost = (feeTemp.FT.OwnCost) * rebateRate;
                        tempRebateCost += feeTemp.FT.RebateCost;
                        //feeTemp.FT.OwnCost = feeTemp.FT.OwnCost - feeTemp.FT.RebateCost;
                        //feeTemp.FT.TotCost = feeTemp.FT.TotCost - feeTemp.FT.RebateCost;
                    }
                    tempFix = f.FT.RebateCost - tempRebateCost;
                    FeeItemList fFix = alTemp[0] as FeeItemList;
                    fFix.FT.RebateCost = fFix.FT.RebateCost + tempFix;
                    //fFix.FT.OwnCost = fFix.FT.OwnCost - tempFix;
                    //fFix.FT.TotCost = fFix.FT.TotCost - tempFix;
                }
            }
            if (alTemp.Count > 0)
            {
                if (f.SpecialPrice > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.SpecialPrice = f.SpecialPrice;

                            break;
                        }
                    }
                }
            }
            if (alTemp.Count > 0)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToDecimal(f.FT.User03) > 0)//有特殊自费
                {
                    decimal tempPrice = 0m;
                    string id = string.Empty;
                    foreach (FeeItemList feeTemp in alTemp)
                    {
                        if (feeTemp.Item.Price > tempPrice)
                        {
                            id = feeTemp.Item.ID;
                            tempPrice = feeTemp.Item.Price;
                        }
                    }

                    foreach (FeeItemList fee in alTemp)
                    {
                        if (fee.Item.ID == id)
                        {
                            fee.FT.User03 = f.FT.User03;

                            break;
                        }
                    }
                }
            }
            return alTemp;
        }


        private Neusoft.HISFC.Models.Base.PactItemRate PactRate(Neusoft.HISFC.Models.Registration.Register r, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f, ref string errMsg)
        {
            Neusoft.HISFC.Models.Base.PactItemRate pRate = new Neusoft.HISFC.Models.Base.PactItemRate();
            pRate.Rate.RebateRate = 0;
            return pRate;
        }

        #region 获取CT/MR收费规则的HashTable
        /// <summary>
        /// 获取CT/MR收费规则的HashTable
        /// </summary>
        /// <returns></returns>
        private Hashtable GetCTMRHashtabel()
        {
            ArrayList alItemZT = this.db.GetAllList("ItemZT");
            Hashtable hsItemZT = new Hashtable();
            if (alItemZT != null)
            {
                hsItemZT = new Hashtable();
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
            }
            return hsItemZT;
        }
        #endregion

        #region 重新赋值开方医生所在科室
        /// <summary>
        /// 重新赋值开方医生所在科室
        /// </summary>
        /// <returns></returns>
        private bool AssignmentDoctDeptInfo(List<FeeItemList> feeItemList)
        {
            foreach (var f in feeItemList)
            {
                if (!string.IsNullOrEmpty(f.RecipeOper.ID) && string.IsNullOrEmpty(f.DoctDeptInfo.ID))
                {
                    var emplInfo = this.db.GetEmployeeInfoForEmplCode(f.RecipeOper.ID);
                    if (emplInfo == null)
                    {
                        this.errText = "未找到工号[" + f.RecipeOper.ID + "]对应的医生信息，无法进行下一步操作！";
                        return false;
                    }
                    if (string.IsNullOrEmpty(emplInfo.DEPT_CODE))
                    {
                        this.errText = "未找到工号[" + f.RecipeOper.ID + "]对应科室所属信息，无法进行下一步操作！";
                        return false;
                    }
                    f.DoctDeptInfo.ID = emplInfo.DEPT_CODE;
                }
            }
            return true;
        }

        #endregion

    }
}
