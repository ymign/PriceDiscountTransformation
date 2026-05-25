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
    /// <summary>
    /// 五院历史特殊收费规则处理器。
    /// 承载限制收费、同组互斥、组套拆分、比例折价四大类规则。
    /// 方法会直接修改传入的 FeeItemList 对象（数量、金额、Memo），真正输出在入参和集合中。
    /// </summary>
    public class Restrictingfee
    {
        #region 字段

        CTMRFeeRuleDB db = null;
        private bool IsUseCtOrMRfeeRule = true;
        DataSet dsItem = new DataSet();
        private string deptCode = "";
        public string errText = "";
        protected Register rInfo = null;
        private bool isTransferTreat = false;

        #endregion

        #region 内部数据结构

        /// <summary>
        /// 规则字典集合：CP(床旁)、TX(胎心)、ZT(止血/同组互斥)、TXxz(例外清单)。
        /// 一次加载后在整个限制收费流程中复用，避免重复查库。
        /// </summary>
        private class RuleDicts
        {
            public readonly Hashtable CPItems = new Hashtable();
            public readonly Hashtable TXItems = new Hashtable();
            public readonly Hashtable ZTItems = new Hashtable();
            public readonly Hashtable TXxzItems = new Hashtable();
            public string GroupNumber = "";
        }

        #endregion

        public Restrictingfee()
        {
            db = new CTMRFeeRuleDB();
        }

        // ================================================================
        //  公共业务入口
        // ================================================================

        #region GetFeeItemList — 组套预处理入口

        /// <summary>
        /// 独立规则类入口：读取患者上下文，把费用明细按 CT/MR/DR 组套规则展开。
        /// </summary>
        public ArrayList GetFeeItemList(string clincCode, ArrayList feeArryList)
        {
            bool isFindDRFirst = false;
            bool isFindCTFirst = false;
            var hsDROnlyOneItem = new Hashtable();
            var hsCTOnlyOneItem = new Hashtable();
            var feeItemLists = new ArrayList();
            var hsDoct = new Hashtable();

            this.rInfo = this.db.GetByClinic(clincCode);
            string tempPayKindid = this.rInfo.Pact.PayKind.ID;
            this.rInfo.Pact = this.db.GetPactUnitInfoByPactCode(this.rInfo.Pact.ID);
            this.rInfo.Pact.PayKind.ID = tempPayKindid;
            this.db.QueryItemList(deptCode, Neusoft.HISFC.Models.Base.ItemKind.All, ref dsItem);

            for (int i = 0; i < feeArryList.Count; i++)
            {
                if (feeArryList[i] == null || !(feeArryList[i] is FeeItemList))
                    continue;

                var f = feeArryList[i] as FeeItemList;
                FillDoctDept(f, hsDoct);

                if (f.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug
                    && IsUseCtOrMRfeeRule && this.db.IsZTUndrugInfo(f.Item.ID))
                {
                    var undrugInfo = this.db.GetUndrugByCode(f.Item.ID);
                    f.Item.NeedConfirm = undrugInfo.NeedConfirm;
                    var hsItemZT = this.GetCTMRHashtabel();
                    ArrayList alDetail = null;

                    if (hsItemZT.ContainsKey(f.Item.ID))
                    {
                        string type = ((ArrayList)hsItemZT[f.Item.ID])[0] is NeuObject obj ? obj.User02 : "";
                        if (type == "DR") isFindDRFirst = true;
                        else if (type == "CT") isFindCTFirst = true;
                    }
                    else
                    {
                        alDetail = ConvertGroupToDetail(f);
                    }

                    if (alDetail == null) { errText = "获得组套明细出错!" + errText; return null; }
                    if (alDetail.Count == 0) { errText = "处理组套项目出错！" + errText; return null; }
                    feeItemLists.AddRange(alDetail);
                }
                else
                {
                    feeItemLists.Add(f);
                }
            }

            CleanupDuplicateGroupItems(feeItemLists, hsDROnlyOneItem, hsCTOnlyOneItem);
            return feeItemLists;
        }

        #endregion

        #region ConvertRestrictingfee — 门诊普通明细限制收费

        /// <summary>
        /// 门诊普通明细项目的限制收费计算。
        /// 剩余额度 = 字典上限 - 历史2小时已收费量 - 本次已保留量。
        /// </summary>
        public void ConvertRestrictingfee(string CARD_NO, FeeItemList f,
            ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem,
            ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber)
        {
            var dicts = LoadRuleDicts(f.UndrugComb.ID);
            decimal limitSum = LimitNumber - GetHistoryUsage(CARD_NO, f.Item.ID, f.UndrugComb.ID, dicts);

            if (limitSum <= 0)
            {
                ApplyLimitToFee(f, limitSum, number, dicts.GroupNumber, true,
                    ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem);
                return;
            }

            // 扣本次收费动作内已保留项目
            limitSum = DeductBatchFeeItems(limitSum,
                f.Item.ID, f.UndrugComb.ID, f.Item.ID, dicts,
                hsNOREOnlyOneItem, useCurrentQty: false);

            ApplyLimitToFee(f, limitSum, number, dicts.GroupNumber, true,
                ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem);
        }

        #endregion

        #region ConvertRestrictingfeeCharge — 门诊组套显示价限制收费

        /// <summary>
        /// 对门诊组套主项重新做一次限制收费计算（用于界面显示价）。
        /// 组套拆子项逐个判断，把可收费金额汇总回主项。
        /// </summary>
        public void ConvertRestrictingfeeCharge(string CARD_NO, FeeItemList f,
            ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem,
            ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber,
            ref ArrayList hsZTNOREOnlyOneItem, DataSet dsItes, Register rInfo)
        {
            var dicts = LoadRuleDicts(f.UndrugComb.ID);

            // 重新读取当前主项的有效项目资料
            var outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            DataSet dtItemSerch = new DataSet();
            outpatientManager.QueryItemListForValid("8004", f.Item.ID, ref dtItemSerch);
            DataRow rowFind = dtItemSerch.Tables[0].Select("ITEM_CODE = '" + f.Item.ID + "'")[0];
            string drugFlag = rowFind["DRUG_FLAG"].ToString();

            if (drugFlag == "2")
            {
                // ===== 非药品组套：拆子项逐个计算，汇总回主项 =====
                decimal sumPricecot = CalcGroupSubItemLimits(
                    CARD_NO, f, LimitNumber, dicts, dsItes, rInfo,
                    hsNOREOnlyOneItem, ref hsZTNOREOnlyOneItem);

                f.FT.TotCost = sumPricecot;
                f.FT.OwnCost = sumPricecot;
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else
            {
                // ===== 普通项目 =====
                decimal histUsage = GetHistoryUsage(CARD_NO, f.Item.ID, f.UndrugComb.ID, dicts);
                decimal limitSum = LimitNumber - histUsage;

                if (limitSum <= 0)
                {
                    ApplyLimitToFee(f, limitSum, number, dicts.GroupNumber, false,
                        ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem);
                    return;
                }

                // 扣普通项目批次（保留原代码行为：使用当前项数量）
                limitSum = DeductBatchFeeItems(limitSum,
                    f.Item.ID, f.UndrugComb.ID, f.Item.ID, dicts,
                    hsNOREOnlyOneItem, useCurrentQty: true, currentQty: f.Item.Qty);

                // 扣组套子项批次（使用 if 而非 else if，保留原代码行为）
                decimal feeqty = 0;
                foreach (UndrugComb dsazt in hsZTNOREOnlyOneItem)
                {
                    if (dsazt.ID == f.Item.ID)
                        feeqty += dsazt.Qty;
                    if (dicts.CPItems.ContainsKey(f.Item.ID) && dicts.CPItems.ContainsKey(dsazt.ID))
                        { limitSum = 0; break; }
                    if (dicts.TXItems.ContainsKey(f.Item.ID) && dicts.TXItems.ContainsKey(dsazt.ID))
                        { limitSum = 0; break; }
                    if (dicts.ZTItems.ContainsKey(f.UndrugComb.ID) && dicts.ZTItems.ContainsKey(dsazt.Package.ID)
                        && dsazt.Memo == dicts.GroupNumber)
                        { limitSum = 0; break; }
                }
                limitSum -= feeqty;

                ApplyLimitToFee(f, limitSum, number, dicts.GroupNumber, false,
                    ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem);
            }
        }

        #endregion

        #region ConvertRestrictingfeeZY — 住院限制收费

        /// <summary>
        /// 住院明细项目的限制收费计算。核心口径与门诊一致，住院历史SQL额外排除已冲销记录。
        /// </summary>
        public void ConvertRestrictingfeeZY(string CARD_NO,
            Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f,
            ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem,
            ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber)
        {
            var dicts = LoadRuleDicts(f.UndrugComb.ID);
            decimal limitSum = LimitNumber - GetHistoryUsage(CARD_NO, f.Item.ID, f.UndrugComb.ID, dicts);

            if (limitSum <= 0)
            {
                ApplyLimitToFeeZY(f, limitSum, number, dicts.GroupNumber,
                    ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem);
                return;
            }

            // 扣本次收费动作内已保留项目
            decimal feeqty = 0;
            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList dsa in hsNOREOnlyOneItem)
            {
                if (CheckExclusion(f.Item.ID, f.UndrugComb.ID, dsa.Item.ID, dsa.UndrugComb.ID,
                    dsa.UndrugComb.Memo, dicts))
                    { limitSum = 0; break; }
                if (dsa.Item.ID == f.Item.ID)
                    feeqty += dsa.Item.Qty;
            }
            limitSum -= feeqty;

            ApplyLimitToFeeZY(f, limitSum, number, dicts.GroupNumber,
                ref hsREOnlyOneItem, ref hsNOREOnlyOneItem, ref hsREOnlylistItem);
        }

        #endregion

        #region ConvertDiscountfee / ConvertDiscountfeeZY — 比例折价

        /// <summary>
        /// 门诊比例折价：第一件原价，第二件起按 DISCOUNT_RATE 折价，TOPPRICE 封顶。
        /// </summary>
        public void ConvertDiscountfee(FeeItemList f, decimal DISCOUNT_RATE, decimal TOPPRICE,
            ref Hashtable hsREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number)
        {
            f.FT.TotCost = CalcDiscountCost(f.Item.Price, f.Item.Qty, DISCOUNT_RATE, TOPPRICE);
            f.FT.OwnCost = f.FT.TotCost;
            hsREOnlyOneItem.Add(f.Item.ID + number, f);
            hsREOnlylistItem.Add(f);
        }

        /// <summary>
        /// 住院比例折价：公式与门诊一致。
        /// </summary>
        public void ConvertDiscountfeeZY(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f,
            decimal DISCOUNT_RATE, int TOPPRICE,
            ref Hashtable hsREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number)
        {
            f.FT.TotCost = CalcDiscountCost(f.Item.Price, f.Item.Qty, DISCOUNT_RATE, TOPPRICE);
            f.FT.OwnCost = f.FT.TotCost;
            hsREOnlyOneItem.Add(f.Item.ID + number, f);
            hsREOnlylistItem.Add(f);
        }

        #endregion

        // ================================================================
        //  私有辅助方法 — 规则字典与历史查询
        // ================================================================

        #region LoadRuleDicts — 一次性加载所有规则字典

        private RuleDicts LoadRuleDicts(string combId)
        {
            var dicts = new RuleDicts();

            foreach (Neusoft.HISFC.Models.Base.Const dizt in this.db.GetList("RestrictingfeeZT"))
            {
                if (dizt.ID == combId) dicts.GroupNumber = dizt.Memo;
                dicts.ZTItems.Add(dizt.ID, dizt.Memo);
            }
            foreach (Neusoft.HISFC.Models.Base.Const dicxz in this.db.GetList("Astrictpackagefee"))
                dicts.TXxzItems.Add(dicxz.ID, dicxz);
            foreach (Neusoft.HISFC.Models.Base.Const dic in this.db.GetList("RestrictingfeeCP"))
                dicts.CPItems.Add(dic.ID, dic);
            foreach (Neusoft.HISFC.Models.Base.Const dis in this.db.GetList("RestrictingfeeTX1"))
                dicts.TXItems.Add(dis.ID, dis);

            return dicts;
        }

        #endregion

        #region GetHistoryUsage — 查询2小时历史收费量

        /// <summary>
        /// 查询历史2小时收费量，考虑例外清单（Astrictpackagefee）和同组互斥（RestrictingfeeZT）。
        /// </summary>
        private decimal GetHistoryUsage(string cardNo, string itemId, string combId, RuleDicts dicts)
        {
            if (dicts.TXxzItems.ContainsKey(combId))
                return 0;

            string feecode = "";
            decimal feetype = this.db.getRestrictingfee(cardNo, itemId, ref feecode);
            if (dicts.ZTItems.ContainsKey(combId))
                feetype = this.db.getRestrictingfeeZT(cardNo, itemId, dicts.GroupNumber, ref feecode);
            return feetype;
        }

        #endregion

        // ================================================================
        //  私有辅助方法 — 批次扣减
        // ================================================================

        #region CheckExclusion — 判断两个项目是否互斥

        /// <summary>
        /// CP/TX/ZT 同组互斥判断。返回 true 表示互斥命中，应将额度归零。
        /// </summary>
        private bool CheckExclusion(string currentItemId, string currentCombId,
            string batchItemId, string batchCombId, string batchCombMemo, RuleDicts dicts)
        {
            if (dicts.CPItems.ContainsKey(currentItemId) && dicts.CPItems.ContainsKey(batchItemId))
                return true;
            if (dicts.TXItems.ContainsKey(currentCombId) && dicts.TXItems.ContainsKey(batchCombId))
                return true;
            if (dicts.ZTItems.ContainsKey(currentCombId) && dicts.ZTItems.ContainsKey(batchCombId)
                && batchCombMemo == dicts.GroupNumber)
                return true;
            return false;
        }

        #endregion

        #region DeductBatchFeeItems — 门诊批次内已保留项目扣减

        /// <summary>
        /// 遍历门诊批次集合，扣减互斥/同项目已保留数量。
        /// useCurrentQty=true 时使用 currentQty 累计（保留 ConvertRestrictingfeeCharge 原代码行为）。
        /// </summary>
        private decimal DeductBatchFeeItems(decimal limitSum,
            string exclusionItemId, string exclusionCombId, string accKey,
            RuleDicts dicts, ArrayList batchItems,
            bool useCurrentQty, decimal currentQty = 0)
        {
            decimal feeqty = 0;
            foreach (FeeItemList dsa in batchItems)
            {
                if (CheckExclusion(exclusionItemId, exclusionCombId,
                    dsa.Item.ID, dsa.UndrugComb.ID, dsa.UndrugComb.Memo, dicts))
                    return limitSum - feeqty;  // 互斥归零（保留已累计的feeqty效果）

                if (dsa.Item.ID == accKey)
                    feeqty += useCurrentQty ? currentQty : dsa.Item.Qty;
            }
            return limitSum - feeqty;
        }

        #endregion

        // ================================================================
        //  私有辅助方法 — 限制收费结果应用
        // ================================================================

        #region ApplyLimitToFee — 门诊三路判定（归零/截断/保留）

        /// <summary>
        /// 根据剩余额度改写门诊费用明细。setMemo=true 时设置 P/N 标记（最终收费用）。
        /// </summary>
        private void ApplyLimitToFee(FeeItemList f, decimal limitSum, decimal number,
            string groupNumber, bool setMemo,
            ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem)
        {
            if (limitSum <= 0)
            {
                f.FT.TotCost = 0;
                f.FT.OwnCost = 0;
                if (setMemo) f.Memo = "P" + f.Item.Qty;
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else if (limitSum < f.Item.Qty)
            {
                f.FT.TotCost = f.Item.Price * limitSum;
                f.FT.OwnCost = f.Item.Price * limitSum;
                if (setMemo) f.Memo = "N" + f.Item.Qty;
                f.Item.Qty = limitSum;
                hsNOREOnlyOneItem.Add(f);
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else
            {
                EnsureCostCalculated(ref f);
                f.UndrugComb.Memo = groupNumber;
                hsNOREOnlyOneItem.Add(f);
            }
        }

        #endregion

        #region ApplyLimitToFeeZY — 住院三路判定

        private void ApplyLimitToFeeZY(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f,
            decimal limitSum, decimal number, string groupNumber,
            ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem)
        {
            if (limitSum <= 0)
            {
                f.FT.TotCost = 0;
                f.FT.OwnCost = 0;
                f.Memo = "P" + f.Item.Qty;
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else if (limitSum < f.Item.Qty)
            {
                f.FT.TotCost = f.Item.Price * limitSum;
                f.FT.OwnCost = f.Item.Price * limitSum;
                f.Memo = "N" + f.Item.Qty;
                f.Item.Qty = limitSum;
                hsNOREOnlyOneItem.Add(f);
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else
            {
                if (f.FT.TotCost <= 0 || f.FT.OwnCost <= 0)
                {
                    f.FT.TotCost = f.Item.Price * f.Item.Qty;
                    f.FT.OwnCost = f.Item.Price * f.Item.Qty;
                }
                f.UndrugComb.Memo = groupNumber;
                hsNOREOnlyOneItem.Add(f);
            }
        }

        #endregion

        #region EnsureCostCalculated — 金额为零时按单价×数量补齐

        private static void EnsureCostCalculated(ref FeeItemList f)
        {
            if (f.FT.TotCost <= 0 || f.FT.OwnCost <= 0)
            {
                f.FT.TotCost = f.Item.Price * f.Item.Qty;
                f.FT.OwnCost = f.Item.Price * f.Item.Qty;
            }
        }

        #endregion

        // ================================================================
        //  私有辅助方法 — 折价计算
        // ================================================================

        #region CalcDiscountCost — 比例折价公式

        /// <summary>
        /// 第一件原价 + 其余件按折扣率 + TOPPRICE 封顶。
        /// </summary>
        private static decimal CalcDiscountCost(decimal unitPrice, decimal qty, decimal discountRate, decimal topPrice)
        {
            decimal cost = unitPrice + (unitPrice * discountRate) * (qty - 1);
            if (topPrice > 0 && cost > topPrice)
                cost = topPrice;
            return cost;
        }

        #endregion

        // ================================================================
        //  私有辅助方法 — 组套子项限制收费
        // ================================================================

        #region CalcGroupSubItemLimits — 组套子项逐个限制收费并汇总

        /// <summary>
        /// 拆组套子项逐个计算限制收费，返回可收费金额合计。
        /// </summary>
        private decimal CalcGroupSubItemLimits(string cardNo, FeeItemList f, decimal limitNumber,
            RuleDicts dicts, DataSet dsItes, Register rInfo,
            ArrayList hsNOREOnlyOneItem, ref ArrayList hsZTNOREOnlyOneItem)
        {
            DateTime nowTime = this.db.GetDateTimeFromSysDateTime();
            int age = (int)((new TimeSpan(nowTime.Ticks - rInfo.Birthday.Ticks)).TotalDays / 365);
            ArrayList alDetail = ConvertGroupToDetail1(f);
            decimal sumPricecot = 0;

            foreach (UndrugComb undrugCombo in alDetail)
            {
                // 子项按患者合同单位重新取价
                DataRow[] rowFinds = dsItes.Tables[0].Select("ITEM_CODE = '" + undrugCombo.ID + "'");
                DataRow rowFind = rowFinds[0];
                decimal orgPrice = 0;
                decimal Price = this.db.GetPrice(undrugCombo.ID, rInfo,
                    NConvert.ToDecimal(rowFind["UNIT_PRICE"]),
                    NConvert.ToDecimal(rowFind["CHILD_PRICE"]),
                    NConvert.ToDecimal(rowFind["SP_PRICE"]),
                    NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]), ref orgPrice);
                undrugCombo.Package.ID = f.Item.ID;

                // 查子项历史收费
                decimal feetype = GetHistoryUsage(cardNo, undrugCombo.ID,
                    dicts.ZTItems.ContainsKey(f.Item.ID) ? f.Item.ID : undrugCombo.ID, dicts);

                // 查子项是否维护了限制收费
                decimal subLimitNumber = limitNumber;
                decimal returnRows = this.db.SetRestrictingfee(undrugCombo.ID, ref subLimitNumber);

                if (returnRows <= 0)
                {
                    // 子项无限制收费，按正常价格纳入
                    sumPricecot += Price * undrugCombo.Qty;
                }
                else
                {
                    decimal limitSum = subLimitNumber - feetype;
                    if (limitSum <= 0)
                    {
                        // 子项历史已占满
                    }
                    else
                    {
                        // 扣普通项目批次（子项在前，互斥用主项ID）
                        limitSum = DeductBatchSubItemFromFeeItems(limitSum,
                            f.Item.ID, undrugCombo.ID, dicts, hsNOREOnlyOneItem);

                        // 扣组套子项批次
                        limitSum = DeductBatchSubItemFromUndrugCombs(limitSum,
                            f.Item.ID, undrugCombo.ID, dicts, hsZTNOREOnlyOneItem);

                        // 子项三路判定
                        if (limitSum <= 0)
                        {
                            // 已占满
                        }
                        else if (limitSum < undrugCombo.Qty)
                        {
                            sumPricecot += Price * limitSum;
                            undrugCombo.Qty = limitSum;
                            hsZTNOREOnlyOneItem.Add(undrugCombo);
                        }
                        else
                        {
                            sumPricecot += Price * undrugCombo.Qty;
                            undrugCombo.Memo = dicts.GroupNumber;
                            hsZTNOREOnlyOneItem.Add(undrugCombo);
                        }
                    }
                }
            }
            return sumPricecot;
        }

        #endregion

        #region DeductBatchSubItem... — 子项批次扣减（同项累计在前，互斥用主项ID）

        private decimal DeductBatchSubItemFromFeeItems(decimal limitSum,
            string mainItemId, string subItemId, RuleDicts dicts, ArrayList batchItems)
        {
            decimal feeqty = 0;
            foreach (FeeItemList dsa in batchItems)
            {
                if (dsa.Item.ID == subItemId)
                {
                    feeqty += dsa.Item.Qty;
                }
                else if (CheckExclusion(mainItemId, mainItemId, dsa.Item.ID, dsa.UndrugComb.ID,
                    dsa.UndrugComb.Memo, dicts))
                {
                    return -feeqty; // 互斥命中
                }
            }
            return limitSum - feeqty;
        }

        private decimal DeductBatchSubItemFromUndrugCombs(decimal limitSum,
            string mainItemId, string subItemId, RuleDicts dicts, ArrayList batchItems)
        {
            decimal feeqty = 0;
            foreach (UndrugComb dsazt in batchItems)
            {
                if (dicts.CPItems.ContainsKey(subItemId) && dicts.CPItems.ContainsKey(dsazt.ID))
                    return -feeqty;
                if (dicts.TXItems.ContainsKey(mainItemId) && dicts.TXItems.ContainsKey(dsazt.Package.ID))
                    return -feeqty;
                if (dicts.ZTItems.ContainsKey(mainItemId) && dicts.ZTItems.ContainsKey(dsazt.Package.ID)
                    && dsazt.Memo == dicts.GroupNumber)
                    return -feeqty;
                if (dsazt.ID == subItemId)
                    feeqty += dsazt.Qty;
            }
            return limitSum - feeqty;
        }

        #endregion

        // ================================================================
        //  私有辅助方法 — 组套拆分
        // ================================================================

        #region ConvertCTGroupToDetail — CT 组套拆分

        /// <summary>
        /// 将 CT 类组套拆成可收费明细，处理 PACS 三维/四维"只收一次"规则。
        /// </summary>
        private ArrayList ConvertCTGroupToDetail(FeeItemList f, bool isFirst, ref Hashtable hsOnlyOneItem)
        {
            ArrayList undrugCombList = this.db.QueryUndrugZTBypackageCode(f.Item.ID);
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + db.Err;
                return null;
            }

            if (!EnsureOrderId(f)) return null;

            DataRow rowFind = FindItemRow(f.Item.ID);
            if (rowFind == null) return null;

            var priceContext = BuildPriceContext(f, rowFind);

            // 预扫描 PACS 只收一次项目
            ScanPacsOnlyOnceItems(undrugCombList, ref hsOnlyOneItem);

            // 逐子项构造 FeeItemList
            var alTemp = BuildSubItemDetails(undrugCombList, f, priceContext);
            if (alTemp == null) return null;

            DistributePostGroupAdjustments(alTemp, f);
            return alTemp;
        }

        #endregion

        #region ConvertGroupToDetail — 普通组套拆分

        /// <summary>
        /// 将普通非药品组套拆成最终收费明细。
        /// </summary>
        private ArrayList ConvertGroupToDetail(FeeItemList f)
        {
            ArrayList undrugCombList = this.db.QueryUndrugPackagesBypackageCode(f.Item.ID);
            if (undrugCombList == null)
            {
                errText = "获得组套明细出错!" + this.db.Err;
                return null;
            }

            if (!EnsureOrderId(f)) return null;

            DataRow rowFind = FindItemRow(f.Item.ID);
            if (rowFind == null) return null;

            var priceContext = BuildPriceContext(f, rowFind);

            // 逐子项构造 FeeItemList
            var alTemp = BuildSubItemDetails(undrugCombList, f, priceContext);
            if (alTemp == null) return null;

            DistributePostGroupAdjustments(alTemp, f);
            return alTemp;
        }

        #endregion

        #region ConvertGroupToDetail1 — 轻量组套明细查询

        private ArrayList ConvertGroupToDetail1(FeeItemList f)
        {
            return this.db.QueryUndrugPackagesBypackageCode(f.Item.ID);
        }

        #endregion

        // ================================================================
        //  私有辅助方法 — 组套拆分共享逻辑
        // ================================================================

        #region 价格上下文

        private class PriceContext
        {
            public int Age;
            public int Month;
            public int Day;
        }

        private PriceContext BuildPriceContext(FeeItemList f, DataRow rowFind)
        {
            DateTime nowTime = this.db.GetDateTimeFromSysDateTime();
            var ctx = new PriceContext();
            this.db.GetAge(this.rInfo.Birthday, nowTime, ref ctx.Age, ref ctx.Month, ref ctx.Day);
            return ctx;
        }

        #endregion

        #region EnsureOrderId — 保证组套子项共享医嘱号

        private bool EnsureOrderId(FeeItemList f)
        {
            if (string.IsNullOrEmpty(f.Order.ID))
            {
                f.Order.ID = this.db.GetNewOrderID();
                if (string.IsNullOrEmpty(f.Order.ID))
                {
                    this.errText = "获得医嘱流水号出错!";
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region FindItemRow — 从缓存中查找项目主数据

        private DataRow FindItemRow(string itemCode)
        {
            DataRow[] rowFinds = dsItem.Tables[0].Select("ITEM_CODE = '" + itemCode + "'");
            if (rowFinds == null || rowFinds.Length == 0)
            {
                this.errText = "查找组套明细出错!";
                return null;
            }
            return rowFinds[0];
        }

        #endregion

        #region BuildSubItemDetails — 逐子项构造 FeeItemList 收费明细

        private ArrayList BuildSubItemDetails(ArrayList undrugCombList, FeeItemList mainItem, PriceContext priceCtx)
        {
            var alTemp = new ArrayList();
            decimal itemRate = 1;

            foreach (UndrugComb undrugCombo in undrugCombList)
            {
                DataRow rowFindZT = FindItemRow(undrugCombo.ID);
                if (rowFindZT == null) continue;

                decimal price;
                decimal orgPrice = 0;
                try
                {
                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    if (isTransferTreat)
                    {
                        itemRate = 1;
                        price = unitPrice;
                    }
                    else
                    {
                        itemRate = this.db.GetItemRateForZT(mainItem.Item.ID, undrugCombo.ID);
                        price = this.db.GetPrice(undrugCombo.ID, this.rInfo, priceCtx.Age,
                            unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                    }
                }
                catch (Exception e)
                {
                    this.errText = e.Message;
                    return null;
                }

                // 合同单位优惠
                string errMsg = string.Empty;
                PactItemRate myRate = this.PactRate(this.rInfo, null, ref errMsg);
                if (myRate == null) { this.errText = errMsg; return null; }
                price *= 1 - myRate.Rate.RebateRate;

                decimal count = mainItem.Item.Qty * undrugCombo.Qty;
                decimal totCost = Neusoft.FrameWork.Public.String.FormatNumber(price * count, 2);

                var feeDetail = BuildFeeDetailFromRow(rowFindZT, mainItem, price, count, totCost, orgPrice);
                SetPactRateFlag(feeDetail, mainItem, rowFindZT);

                feeDetail.Order.Usage = mainItem.Order.Usage;
                feeDetail.Order.ApplyNo = mainItem.Order.ApplyNo;
                feeDetail.Order.Sample.ID = mainItem.Order.Sample.ID;
                feeDetail.Order.Sample.Name = mainItem.Order.Sample.Name;
                feeDetail.Order.CheckPartRecord = mainItem.Order.CheckPartRecord;

                alTemp.Add(feeDetail);
            }
            return alTemp;
        }

        #endregion

        #region BuildFeeDetailFromRow — 从 DataRow 构造单条收费明细

        private static FeeItemList BuildFeeDetailFromRow(DataRow row, FeeItemList mainItem,
            decimal price, decimal count, decimal totCost, decimal orgPrice)
        {
            var fd = new FeeItemList();
            fd.OrgPrice = orgPrice;
            fd.Item.ChildPrice = NConvert.ToDecimal(row["UNIT_PRICE"]);

            fd.Patient = mainItem.Patient.Clone();
            fd.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
            fd.Item.ID = row["ITEM_CODE"].ToString();
            fd.Item.Name = row["ITEM_NAME"].ToString();
            fd.Name = fd.Item.Name;
            fd.ID = fd.Item.ID;

            string itemType = row["DRUG_FLAG"].ToString();
            switch (itemType)
            {
                case "0":
                    fd.Item.ItemType = EnumItemType.UnDrug;
                    fd.IsGroup = false;
                    break;
                case "1":
                    fd.Item.ItemType = EnumItemType.Drug;
                    fd.IsGroup = false;
                    break;
                case "2":
                    fd.Item.ItemType = EnumItemType.UnDrug;
                    fd.IsGroup = true;
                    break;
            }

            fd.RecipeOper = mainItem.RecipeOper.Clone();
            fd.Item.Price = price;
            fd.Item.Specs = row["SPECS"].ToString();
            fd.Item.SysClass.ID = row["SYS_CLASS"].ToString();
            fd.Item.MinFee.ID = row["FEE_CODE"].ToString();
            fd.Item.PackQty = NConvert.ToDecimal(row["PACK_QTY"].ToString());
            fd.Item.Qty = count;
            fd.Days = NConvert.ToDecimal(mainItem.Days);
            fd.FT.TotCost = totCost;
            fd.FT.OwnCost = totCost;
            fd.ExecOper = mainItem.ExecOper.Clone();
            fd.Item.PriceUnit = string.IsNullOrEmpty(row["MIN_UNIT"].ToString()) ? "次" : row["MIN_UNIT"].ToString();

            // NeedConfirm
            string confirmFlag = row["CONFIRM_FLAG"].ToString();
            if (string.IsNullOrEmpty(confirmFlag))
            {
                fd.Item.NeedConfirm = EnumNeedConfirm.None;
            }
            else
            {
                int cfVal = NConvert.ToInt32(confirmFlag);
                if (Enum.IsDefined(typeof(EnumNeedConfirm), cfVal))
                    fd.Item.NeedConfirm = (EnumNeedConfirm)cfVal;
            }

            fd.Item.IsNeedBespeak = NConvert.ToBoolean(row["NEEDBESPEAK"].ToString());
            fd.Order.ID = mainItem.Order.ID;
            fd.UndrugComb.ID = mainItem.Item.ID;
            fd.UndrugComb.Name = mainItem.Item.Name;
            fd.UndrugComb.Qty = mainItem.Item.Qty;
            fd.Order.Combo.ID = mainItem.Order.Combo.ID;
            fd.Item.IsMaterial = mainItem.Item.IsMaterial;
            fd.RecipeSequence = mainItem.RecipeSequence;
            fd.FTSource = mainItem.FTSource;
            fd.FeePack = mainItem.FeePack;

            return fd;
        }

        #endregion

        #region SetPactRateFlag — 设置合同单位比例标志

        private void SetPactRateFlag(FeeItemList feeDetail, FeeItemList mainItem, DataRow rowFindZT)
        {
            if (this.rInfo.Pact.PayKind.ID != "03") return;

            Neusoft.HISFC.Models.Base.PactItemRate pactRate =
                this.db.GetOnepPactUnitItemRateByItem(this.rInfo.Pact.ID, feeDetail.Item.ID);

            if (pactRate != null)
            {
                if (pactRate.Rate.PayRate != this.rInfo.Pact.Rate.PayRate)
                    feeDetail.ItemRateFlag = pactRate.Rate.PayRate == 1 ? "1" : "2";
                else
                    feeDetail.ItemRateFlag = "2";

                if (mainItem.ItemRateFlag == "3")
                {
                    feeDetail.OrgItemRate = mainItem.OrgItemRate;
                    feeDetail.NewItemRate = mainItem.NewItemRate;
                    feeDetail.ItemRateFlag = "3";
                }
            }
            else
            {
                feeDetail.OrgItemRate = mainItem.OrgItemRate;
                feeDetail.NewItemRate = mainItem.NewItemRate;
                feeDetail.ItemRateFlag = mainItem.ItemRateFlag == "3" ? "3" : mainItem.ItemRateFlag;
            }
        }

        #endregion

        #region ScanPacsOnlyOnceItems — PACS 三维/四维只收一次预扫描

        private void ScanPacsOnlyOnceItems(ArrayList undrugCombList, ref Hashtable hsOnlyOneItem)
        {
            foreach (UndrugComb undrugCombo in undrugCombList)
            {
                if (undrugCombo.SortID != 3) continue;
                if (hsOnlyOneItem.ContainsKey(undrugCombo.ID)) continue;

                DataRow row = FindItemRow(undrugCombo.ID);
                if (row == null) continue;

                string itemName = row["ITEM_NAME"].ToString();
                if (itemName.Contains("三维重建"))
                {
                    hsOnlyOneItem.Add(undrugCombo.ID,
                        hsOnlyOneItem.ContainsValue("四维") ? "true" : "三维");
                }
                else if (itemName.Contains("四维重建"))
                {
                    // 四维出现后，已登记的三维标记为可删除
                    var hsTemp = hsOnlyOneItem.Clone() as Hashtable;
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

        #region DistributePostGroupAdjustments — 组套拆分后减免/特殊自费分摊

        /// <summary>
        /// 把主项上的人工减免、特殊自费金额分摊到拆出的子项。
        /// 减免按比例分摊（尾差补第一条），特殊自费/User03 挂到价格最高的子项。
        /// </summary>
        private void DistributePostGroupAdjustments(ArrayList alTemp, FeeItemList mainItem)
        {
            if (alTemp.Count == 0) return;

            // 减免分摊
            if (mainItem.FT.RebateCost > 0)
            {
                if (this.rInfo.Pact.PayKind.ID != "01")
                {
                    this.errText = "暂时不允许非自费患者减免!";
                    return;
                }
                decimal rebateRate = Neusoft.FrameWork.Public.String.FormatNumber(
                    mainItem.FT.RebateCost / mainItem.FT.OwnCost, 2);
                decimal tempRebateCost = 0;
                foreach (FeeItemList feeTemp in alTemp)
                {
                    feeTemp.FT.RebateCost = feeTemp.FT.OwnCost * rebateRate;
                    tempRebateCost += feeTemp.FT.RebateCost;
                }
                // 尾差补到第一条
                ((FeeItemList)alTemp[0]).FT.RebateCost += mainItem.FT.RebateCost - tempRebateCost;
            }

            // 特殊自费挂到价格最高的子项
            if (mainItem.SpecialPrice > 0)
                AssignToHighestPriceItem(alTemp, fd => fd.SpecialPrice = mainItem.SpecialPrice);

            // User03 挂到价格最高的子项
            if (NConvert.ToDecimal(mainItem.FT.User03) > 0)
                AssignToHighestPriceItem(alTemp, fd => fd.FT.User03 = mainItem.FT.User03);
        }

        private static void AssignToHighestPriceItem(ArrayList alTemp, Action<FeeItemList> assign)
        {
            decimal maxPrice = 0;
            string maxId = "";
            foreach (FeeItemList feeTemp in alTemp)
            {
                if (feeTemp.Item.Price > maxPrice)
                {
                    maxId = feeTemp.Item.ID;
                    maxPrice = feeTemp.Item.Price;
                }
            }
            foreach (FeeItemList fee in alTemp)
            {
                if (fee.Item.ID == maxId) { assign(fee); break; }
            }
        }

        #endregion

        // ================================================================
        //  私有辅助方法 — 其他
        // ================================================================

        #region PactRate — 合同单位优惠比例（当前固定返回0折扣）

        private Neusoft.HISFC.Models.Base.PactItemRate PactRate(
            Register r, FeeItemList f, ref string errMsg)
        {
            var pRate = new Neusoft.HISFC.Models.Base.PactItemRate();
            pRate.Rate.RebateRate = 0;
            return pRate;
        }

        #endregion

        #region GetCTMRHashtabel — CT/MR/DR 特殊组套规则哈希表

        private Hashtable GetCTMRHashtabel()
        {
            ArrayList alItemZT = this.db.GetAllList("ItemZT");
            var hsItemZT = new Hashtable();
            if (alItemZT == null) return hsItemZT;

            foreach (Neusoft.HISFC.Models.Base.Const conObj in alItemZT)
            {
                if (!conObj.IsValid || string.IsNullOrEmpty(conObj.Memo.Trim()))
                    continue;

                string[] itemIDs = conObj.Memo.Split('|');
                var newObjects = new ArrayList();
                foreach (string itemID in itemIDs)
                    newObjects.Add(BuildNeuObjectFromConst(itemID, conObj));

                if (hsItemZT.ContainsKey(conObj.Name))
                {
                    ((ArrayList)hsItemZT[conObj.Name]).AddRange(newObjects);
                }
                else
                {
                    hsItemZT.Add(conObj.Name, newObjects);
                }
            }
            return hsItemZT;
        }

        private static NeuObject BuildNeuObjectFromConst(string itemID, Neusoft.HISFC.Models.Base.Const conObj)
        {
            var obj = new NeuObject();
            obj.ID = itemID;
            obj.Name = conObj.WBCode;

            switch (conObj.SortID.ToString())
            {
                case "0": obj.Memo = "每个项目收取"; break;
                case "1": obj.Memo = "第一个项目收取"; break;
                case "2": obj.Memo = "第二个项目起加收"; break;
                case "3": obj.Memo = "只收取一次"; break;
            }
            switch (conObj.SpellCode)
            {
                case "0": obj.User01 = "总量取整"; break;
                case "1": obj.User01 = "单个取整"; break;
                case "2": obj.User01 = "固定数量"; break;
            }
            switch (conObj.UserCode)
            {
                case "0": obj.User02 = "DR"; break;
                case "1": obj.User02 = "CT"; break;
            }
            return obj;
        }

        #endregion

        #region FillDoctDept — 补齐开方医生科室

        private void FillDoctDept(FeeItemList f, Hashtable hsDoct)
        {
            if (string.IsNullOrEmpty(f.RecipeOper.ID) || !string.IsNullOrEmpty(f.DoctDeptInfo.ID))
                return;

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

        #endregion

        #region CleanupDuplicateGroupItems — CT/DR 组套去重

        private static void CleanupDuplicateGroupItems(ArrayList feeItemLists,
            Hashtable hsDROnlyOneItem, Hashtable hsCTOnlyOneItem)
        {
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
                feeItemLists.Add(de.Value as FeeItemList);
        }

        #endregion

        #region AssignmentDoctDeptInfo — 批量补齐医生科室

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
