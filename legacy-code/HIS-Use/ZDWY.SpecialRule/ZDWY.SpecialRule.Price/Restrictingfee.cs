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
    public class Restrictingfee
    {
        #region 属性
        /// <summary>
        /// 数据库操作类
        /// </summary>
        CTMRFeeRuleDB db = null;
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
        public Restrictingfee()
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
                            isFindDRFirst = true;
                        }
                        else if (type == "CT")
                        {
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

            return feeItemLists;
        }

        //明细单独项目限制收费计算规则
        public void ConvertRestrictingfee(string CARD_NO, FeeItemList f, ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber)
        {
            decimal Price = 0;
            string feecode = "";
            string GroupNumber = "";//组套组号
            Decimal feetype = 0;
            Decimal feeqty = 0;
            Hashtable hsCPItem = new Hashtable();
            Hashtable hsTXItem = new Hashtable();
            Hashtable hsTXxzItem = new Hashtable();
            Hashtable hsZTItem = new Hashtable();
            ArrayList alfeecpxz = this.db.GetList("Astrictpackagefee");
            ArrayList alfeezt = this.db.GetList("RestrictingfeeZT");
            foreach (Neusoft.HISFC.Models.Base.Const dizt in alfeezt)  //获取分组收费项目
            {
                if (dizt.ID == f.UndrugComb.ID)
                {
                    GroupNumber = dizt.Memo;
                }
                hsZTItem.Add(dizt.ID, dizt.Memo);
            }
            foreach (Neusoft.HISFC.Models.Base.Const dicxz in alfeecpxz)  //获取本次收费已经计算的数量
            {
                hsTXxzItem.Add(dicxz.ID, dicxz);

            }
            if (hsTXxzItem.ContainsKey(f.UndrugComb.ID))
            {
                feetype = 0;
            }
            else
            {
                feetype = this.db.getRestrictingfee(CARD_NO, f.Item.ID, ref feecode);
                if (hsZTItem.ContainsKey(f.UndrugComb.ID))
                {
                    feetype = this.db.getRestrictingfeeZT(CARD_NO, f.Item.ID, GroupNumber, ref feecode);
                }
            }
            Decimal Limitsum = LimitNumber - feetype;//计算是否次项目还可以收费有没有超越收费次数
            if (Limitsum <= 0)
            {
                //f.Item.Price = Convert.ToDecimal(Price * f.Item.Qty) - Convert.ToDecimal(Price * f.Item.Qty);
                f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else
            {
                ArrayList alfeecp = this.db.GetList("RestrictingfeeCP");
                ArrayList alfeetx = this.db.GetList("RestrictingfeeTX1");
                foreach (Neusoft.HISFC.Models.Base.Const dic in alfeecp)  //获取本次收费已经计算的数量
                {
                    hsCPItem.Add(dic.ID, dic);

                }
                foreach (Neusoft.HISFC.Models.Base.Const dis in alfeetx)  //获取胎心项目
                {
                    hsTXItem.Add(dis.ID, dis);
                }
                foreach (FeeItemList dsa in hsNOREOnlyOneItem)  //获取本次收费已经计算的数量
                {

                    if (hsCPItem.ContainsKey(f.Item.ID))
                    {

                        if (hsCPItem.ContainsKey(dsa.Item.ID))
                        {
                            Limitsum = 0;
                            break;
                        }
                    }
                    else if (hsTXItem.ContainsKey(f.UndrugComb.ID))
                    {

                        if (hsTXItem.ContainsKey(dsa.UndrugComb.ID))
                        {
                            Limitsum = 0;
                            break;
                        }
                    }
                    else if (hsZTItem.ContainsKey(f.UndrugComb.ID))
                    {
                        if (hsZTItem.ContainsKey(dsa.UndrugComb.ID))
                        {
                            if (dsa.UndrugComb.Memo == GroupNumber)
                            {
                                Limitsum = 0;
                                break;
                            }
                        }
                    }

                    else if (dsa.Item.ID == f.Item.ID)
                    {
                        feeqty += Convert.ToDecimal(dsa.Item.Qty);
                    }

                }
                Limitsum = Limitsum - feeqty;
                if (Limitsum <= 0)
                {
                    f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                    hsREOnlyOneItem.Add(f.Item.ID + number, f);
                    hsREOnlylistItem.Add(f);
                }
                else if ((Limitsum - f.Item.Qty) <= 0)
                {
                    //f.Item.Price =  Convert.ToDecimal(Price * Limitsum);
                    f.FT.TotCost = Convert.ToDecimal(f.Item.Price * Limitsum);
                    f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * Limitsum);
                    f.Memo = "N" + Convert.ToDecimal(f.Item.Qty);
                    f.Item.Qty = Limitsum;
                    hsNOREOnlyOneItem.Add(f);
                    hsREOnlyOneItem.Add(f.Item.ID + number, f);
                    hsREOnlylistItem.Add(f);
                }
                else
                {
                    if (f.FT.TotCost > 0 && f.FT.OwnCost > 0)
                    {
                        f.UndrugComb.Memo = GroupNumber;
                        hsNOREOnlyOneItem.Add(f);
                    }
                    else
                    {
                        f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                        f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                        f.UndrugComb.Memo = GroupNumber;
                        hsNOREOnlyOneItem.Add(f);
                    }
                }
            }
        }

        /// <summary>
        /// 对门诊组套主项重新做一次“限制收费”计算。
        /// </summary>
        /// <param name="CARD_NO">
        /// 当前患者的卡号。
        /// 这段方法里会拿它去查该患者以前有没有收过同类项目、同组项目，判断这次还能不能继续收费。
        /// </param>
        /// <param name="f">
        /// 当前正在处理的组套主项，也就是界面上先看到的那个“汇总项目”。
        /// 这里不是直接对拆开的子项做计算，而是先拿这个主项进来，再在方法里把它拆成明细逐个判断。
        /// </param>
        /// <param name="hsREOnlyOneItem">
        /// “本轮已经重算过哪些汇总项目”的登记表。
        /// 作用是防止同一个组套主项在一次处理中被重复重算、重复回写。
        /// </param>
        /// <param name="hsNOREOnlyOneItem">
        /// “本次收费里已经保留下来的普通收费明细”清单。
        /// 后面算限制收费时，除了看历史记录，还要把本次已经算进去的普通项目一起累计，避免同一次收费里超限。
        /// </param>
        /// <param name="hsREOnlylistItem">
        /// “本轮重算后最终要回写的项目”清单。
        /// 方法跑完以后，哪些项目需要作为最终结果替换回收费列表，就会放到这里。
        /// </param>
        /// <param name="number">
        /// 当前处理到的逻辑顺序号。
        /// 它更像流程里的“第几个项目”标记，不是收费数量，也不是限次值。
        /// </param>
        /// <param name="LimitNumber">
        /// 当前这条组套主项或它拆开的明细项目，允许收费的上限值。
        /// 方法里的很多判断，最终都会围绕“已经收了多少”和“这个上限还剩多少”来决定是否保留收费。
        /// </param>
        /// <param name="hsZTNOREOnlyOneItem">
        /// “本次收费里已经保留下来的组套拆分明细”清单。
        /// 它和 hsNOREOnlyOneItem 的区别是：这里专门记组套拆开的子项，目的是让后续别的组套再计算时，也能把这些已保留子项一起累计进去。
        /// </param>
        /// <param name="dsItes">
        /// 本次计算时用到的项目主数据快照。
        /// 可以理解成一份临时查阅表，里面放着项目编码、类别、价格、组套关系等基础资料，供后面拆组套和判断项目属性时使用。
        /// </param>
        /// <param name="rInfo">
        /// 当前患者的挂号/就诊信息。
        /// 里面通常会带科室、合同单位、就诊相关信息，某些限制收费或折价判断会依赖这些上下文。
        /// </param>
        /// <remarks>
        /// 这类组套项目不能直接拿“主项汇总数量”去做限制收费判断，
        /// 因为真正受限的往往是组套拆开后的子项，而不是界面上那个汇总主项本身。
        /// 所以这里的处理顺序是：
        /// 先把组套拆成明细，
        /// 再逐个明细去看历史收费、本次已保留项目、同组互斥和剩余额度，
        /// 最后把还能收费的明细金额重新汇总回原来的组套主项。
        /// </remarks>
        public void ConvertRestrictingfeeCharge(string CARD_NO, FeeItemList f, ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber, ref ArrayList hsZTNOREOnlyOneItem, DataSet dsItes, Register rInfo)
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            //RE 这里大概率是 Re，意思是“重算 / 重新处理 / 重新写回”。
            //所以：
            //- hs = hashtable
            //- RE = recalculate / reprocess
            //- hsREOnlylistItem = “重算后要保留/回写的列表”

            decimal Price = 0;
            decimal orgPrice = 0;
            decimal Pricecot = 0;
            decimal sumPricecot = 0;
            string feecode = "";
            string GroupNumber = "";//组套组号
            decimal feetype = 0;
            decimal returnRows = 0;//是否为限制收费药品
            decimal feeqty = 0;
            ArrayList alDetail = null;
            DataRow rowFind;
            //床旁项目
            Hashtable hsCPItem = new Hashtable();
            //胎心项目
            Hashtable hsTXItem = new Hashtable();
            //分组收费项目
            Hashtable hsZTItem = new Hashtable();
            //DataRow[] rowFinds = dsItes.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            //重新赋值
            DataSet dtItemSerch = new DataSet();
            outpatientManager.QueryItemListForValid("8004", f.Item.ID, ref dtItemSerch);
            DataRow[] rowFinds = dtItemSerch.Tables[0].Select("ITEM_CODE = " + "'" + f.Item.ID + "'");
            rowFind = rowFinds[0];

            //drugFlag: 0非药品明细项目 1药品 2非药品组套项目 3不知道
            //4药品标志协定处方  6物资项目(暂时木有数据)
            string drugFlag = rowFind["DRUG_FLAG"].ToString();

            // RestrictingfeeCP：床旁加收项目限制。
            ArrayList alfeecp = this.db.GetList("RestrictingfeeCP");
            // RestrictingfeeTX1：胎心监护项目限制/互斥。
            ArrayList alfeetx = this.db.GetList("RestrictingfeeTX1");
            // RestrictingfeeZT：止血/同组项目互斥，按组号累计历史次数
            ArrayList alfeezt = this.db.GetList("RestrictingfeeZT");

            foreach (Neusoft.HISFC.Models.Base.Const dic in alfeecp)  //获取床旁项目
            {
                hsCPItem.Add(dic.ID, dic);
            }
            foreach (Neusoft.HISFC.Models.Base.Const dis in alfeetx)  //获取胎心项目
            {
                hsTXItem.Add(dis.ID, dis);
            }
            foreach (Neusoft.HISFC.Models.Base.Const dizt in alfeezt)  //获取分组收费项目
            {
                if (dizt.ID == f.UndrugComb.ID)
                {
                    GroupNumber = dizt.Memo;
                }
                hsZTItem.Add(dizt.ID, dizt.Memo);
            }


            #region 非药品组套项目

            if (drugFlag == "2")
            {
                DateTime nowTime = this.db.GetDateTimeFromSysDateTime();
                int age = (int)((new TimeSpan(nowTime.Ticks - rInfo.Birthday.Ticks)).TotalDays / 365);
                alDetail = ConvertGroupToDetail1(f);
                foreach (UndrugComb undrugCombo in alDetail)
                {
                    rowFinds = dsItes.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                    rowFind = rowFinds[0];
                    decimal unitPrice = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFind["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);
                    undrugCombo.Package.ID = f.Item.ID;

                    //重新获取价格
                    Price = this.db.GetPrice(undrugCombo.ID, rInfo, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice);

                    //获取限制项目收费次数
                    feetype = this.db.getRestrictingfee(CARD_NO, undrugCombo.ID, ref feecode);

                    if (hsZTItem.ContainsKey(f.Item.ID))
                    {
                        feetype = this.db.getRestrictingfeeZT(CARD_NO, undrugCombo.ID, GroupNumber, ref feecode);
                    }

                    #region 限制收费项目规则

                    //获取本项目维护的限制收费的次数
                    returnRows = this.db.SetRestrictingfee(undrugCombo.ID, ref LimitNumber);
                    if (returnRows > 0)
                    {
                        //计算本项目本次剩余的收费次数
                        decimal Limitsum = LimitNumber - feetype;
                        if (Limitsum <= 0)
                        {
                            sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty) - Convert.ToDecimal(Price * undrugCombo.Qty);
                        }
                        else
                        {
                            foreach (FeeItemList dsa in hsNOREOnlyOneItem)  //获取本次收费已经计算的数量
                            {
                                if (dsa.Item.ID == undrugCombo.ID)
                                {
                                    feeqty += Convert.ToDecimal(dsa.Item.Qty);
                                }
                                else if (hsCPItem.ContainsKey(f.Item.ID))
                                {

                                    if (hsCPItem.ContainsKey(dsa.Item.ID))
                                    {
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsTXItem.ContainsKey(f.Item.ID))
                                {

                                    if (hsTXItem.ContainsKey(dsa.UndrugComb.ID))
                                    {
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsZTItem.ContainsKey(f.Item.ID))
                                {
                                    if (hsZTItem.ContainsKey(dsa.UndrugComb.ID))
                                    {
                                        if (dsa.UndrugComb.Memo == GroupNumber)
                                        {
                                            Limitsum = 0;
                                            break;
                                        }
                                    }
                                }

                            }

                            foreach (UndrugComb dsazt in hsZTNOREOnlyOneItem)  //获取本次收费已经计算的数量
                            {
                                if (hsCPItem.ContainsKey(undrugCombo.ID))
                                {

                                    if (hsCPItem.ContainsKey(dsazt.ID))
                                    {
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsTXItem.ContainsKey(f.Item.ID))
                                {

                                    if (hsTXItem.ContainsKey(dsazt.Package.ID))
                                    {
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsZTItem.ContainsKey(f.Item.ID))
                                {
                                    if (hsZTItem.ContainsKey(dsazt.Package.ID))
                                    {
                                        if (dsazt.Memo == GroupNumber)
                                        {
                                            Limitsum = 0;
                                            break;
                                        }
                                    }
                                }

                                else if (dsazt.ID == undrugCombo.ID)
                                {
                                    feeqty += Convert.ToDecimal(dsazt.Qty);
                                }
                            }
                            Limitsum = Limitsum - feeqty;
                            if (Limitsum == 0)
                            {
                                sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty) - Convert.ToDecimal(Price * undrugCombo.Qty);
                            }
                            else if ((Limitsum - undrugCombo.Qty) <= 0)
                            {
                                sumPricecot += Convert.ToDecimal(Price * Limitsum);
                                undrugCombo.Qty = Limitsum;
                                hsZTNOREOnlyOneItem.Add(undrugCombo);
                            }
                            else
                            {
                                sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty);
                                undrugCombo.Memo = GroupNumber;
                                hsZTNOREOnlyOneItem.Add(undrugCombo);
                            }
                        }
                    }
                    else
                    {
                        sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty);
                    }
                    #endregion

                    feeqty = 0;

                }
                //f.Item.Price = sumPricecot;
                f.FT.TotCost = sumPricecot;
                f.FT.OwnCost = sumPricecot;
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);

            }

            #endregion

            #region 普通项目

            else
            {
                feetype = this.db.getRestrictingfee(CARD_NO, f.Item.ID, ref feecode);
                if (hsZTItem.ContainsKey(f.Item.ID))
                {
                    feetype = this.db.getRestrictingfeeZT(CARD_NO, f.Item.ID, GroupNumber, ref feecode);
                }
                //计算本项目本次剩余的收费次数
                decimal Limitsum = LimitNumber - feetype;//
                if (Limitsum <= 0)
                {
                    //f.Item.Price = Convert.ToDecimal(Price * f.Item.Qty) - Convert.ToDecimal(Price * f.Item.Qty);
                    f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    hsREOnlyOneItem.Add(f.Item.ID + number, f);
                    hsREOnlylistItem.Add(f);
                }
                else
                {

                    #region 获取本次收费已经计算的数量

                    foreach (FeeItemList dsa in hsNOREOnlyOneItem)
                    {

                        if (hsCPItem.ContainsKey(f.Item.ID))
                        {

                            if (hsCPItem.ContainsKey(dsa.Item.ID))
                            {
                                Limitsum = 0;
                                break;
                            }
                        }
                        else if (hsTXItem.ContainsKey(f.UndrugComb.ID))
                        {

                            if (hsTXItem.ContainsKey(dsa.UndrugComb.ID))
                            {
                                Limitsum = 0;
                                break;
                            }
                        }
                        else if (hsZTItem.ContainsKey(f.UndrugComb.ID))
                        {
                            if (hsZTItem.ContainsKey(dsa.UndrugComb.ID))
                            {
                                if (dsa.UndrugComb.Memo == GroupNumber)
                                {
                                    Limitsum = 0;
                                    break;
                                }
                            }
                        }
                        else if (dsa.Item.ID == f.Item.ID)
                        {
                            feeqty += Convert.ToDecimal(f.Item.Qty);
                        }
                    }

                    #endregion

                    #region 获取组套本次收费已经计算的数量
                    foreach (UndrugComb dsazt in hsZTNOREOnlyOneItem)
                    {
                        if (dsazt.ID == f.Item.ID)
                        {
                            feeqty += Convert.ToDecimal(dsazt.Qty);
                        }
                        if (hsCPItem.ContainsKey(f.Item.ID))
                        {

                            if (hsCPItem.ContainsKey(dsazt.ID))
                            {
                                Limitsum = 0;
                                break;
                            }
                        }
                        if (hsTXItem.ContainsKey(f.Item.ID))
                        {

                            if (hsTXItem.ContainsKey(dsazt.ID))
                            {
                                Limitsum = 0;
                                break;
                            }
                        }
                        if (hsZTItem.ContainsKey(f.UndrugComb.ID))
                        {
                            if (hsZTItem.ContainsKey(dsazt.Package.ID))
                            {
                                if (dsazt.Memo == GroupNumber)
                                {
                                    Limitsum = 0;
                                    break;
                                }
                            }
                        }

                    }

                    #endregion

                    Limitsum = Limitsum - feeqty;
                    if (Limitsum <= 0)
                    {
                        f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                        f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                        hsREOnlyOneItem.Add(f.Item.ID + number, f);
                        hsREOnlylistItem.Add(f);
                    }
                    else if ((Limitsum - f.Item.Qty) <= 0)
                    {
                        f.FT.TotCost = Convert.ToDecimal(f.Item.Price * Limitsum);
                        f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * Limitsum);
                        f.Item.Qty = Limitsum;
                        hsNOREOnlyOneItem.Add(f);
                        hsREOnlyOneItem.Add(f.Item.ID + number, f);
                        hsREOnlylistItem.Add(f);
                    }
                    else
                    {
                        if (f.FT.TotCost > 0 && f.FT.OwnCost > 0)
                        {
                            f.UndrugComb.Memo = GroupNumber;
                            hsNOREOnlyOneItem.Add(f);
                        }
                        else
                        {
                            f.UndrugComb.Memo = GroupNumber;
                            f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                            f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                            hsNOREOnlyOneItem.Add(f);
                        }
                    }
                }
                feeqty = 0;
            }

            #endregion

        }

        //住院明细单独项目限制收费计算规则
        public void ConvertRestrictingfeeZY(string CARD_NO, Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f, ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber)
        {
            decimal Price = 0;
            string feecode = "";
            Decimal feetype = 0;
            Decimal feeqty = 0;
            string GroupNumber = "";//组套组号
            Hashtable hsCPItem = new Hashtable();
            Hashtable hsTXItem = new Hashtable();
            Hashtable hsTXxzItem = new Hashtable();
            Hashtable hsZTItem = new Hashtable();
            ArrayList alfeecpxz = this.db.GetList("Astrictpackagefee");
            ArrayList alfeezt = this.db.GetList("RestrictingfeeZT");
            foreach (Neusoft.HISFC.Models.Base.Const dizt in alfeezt)  //获取分组收费项目
            {
                if (dizt.ID == f.UndrugComb.ID)
                {
                    GroupNumber = dizt.Memo;
                }
                hsZTItem.Add(dizt.ID, dizt.Memo);
            }
            foreach (Neusoft.HISFC.Models.Base.Const dicxz in alfeecpxz)  //获取本次收费已经计算的数量
            {
                hsTXxzItem.Add(dicxz.ID, dicxz);
            }
            if (hsTXxzItem.ContainsKey(f.UndrugComb.ID))
            {
                feetype = 0;
            }
            else
            {
                feetype = this.db.getRestrictingfee(CARD_NO, f.Item.ID, ref feecode);
                if (hsZTItem.ContainsKey(f.UndrugComb.ID))
                {
                    feetype = this.db.getRestrictingfeeZT(CARD_NO, f.Item.ID, GroupNumber, ref feecode);
                }
            }

            Decimal Limitsum = LimitNumber - feetype;//计算是否次项目还可以收费有没有超越收费次数
            if (Limitsum <= 0)
            {
                //f.Item.Price = Convert.ToDecimal(Price * f.Item.Qty) - Convert.ToDecimal(Price * f.Item.Qty);
                f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else
            {
                ArrayList alfeecp = this.db.GetList("RestrictingfeeCP");
                ArrayList alfeetx = this.db.GetList("RestrictingfeeTX1");
                foreach (Neusoft.HISFC.Models.Base.Const dic in alfeecp)  //获取床旁项目
                {
                    hsCPItem.Add(dic.ID, dic);
                }
                foreach (Neusoft.HISFC.Models.Base.Const dis in alfeetx)  //获取本次收费已经计算的数量
                {
                    hsTXItem.Add(dis.ID, dis);
                }
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList dsa in hsNOREOnlyOneItem)  //获取本次收费已经计算的数量
                {

                    if (hsCPItem.ContainsKey(f.Item.ID))
                    {

                        if (hsCPItem.ContainsKey(dsa.Item.ID))
                        {
                            Limitsum = 0;
                            break;
                        }
                    }
                    else if (hsTXItem.ContainsKey(f.UndrugComb.ID))
                    {

                        if (hsTXItem.ContainsKey(dsa.UndrugComb.ID))
                        {
                            Limitsum = 0;
                            break;
                        }
                    }
                    else if (hsZTItem.ContainsKey(f.UndrugComb.ID))
                    {
                        if (hsZTItem.ContainsKey(dsa.UndrugComb.ID))
                        {
                            if (dsa.UndrugComb.Memo == GroupNumber)
                            {
                                Limitsum = 0;
                                break;
                            }
                        }
                    }
                    else if (dsa.Item.ID == f.Item.ID)
                    {
                        feeqty += Convert.ToDecimal(dsa.Item.Qty);
                    }

                }
                Limitsum = Limitsum - feeqty;
                if (Limitsum <= 0)
                {
                    f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                    hsREOnlyOneItem.Add(f.Item.ID + number, f);
                    hsREOnlylistItem.Add(f);
                }
                else if ((Limitsum - f.Item.Qty) <= 0)
                {
                    //f.Item.Price =  Convert.ToDecimal(Price * Limitsum);
                    f.FT.TotCost = Convert.ToDecimal(f.Item.Price * Limitsum);
                    f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * Limitsum);
                    f.Memo = "N" + Convert.ToDecimal(f.Item.Qty);
                    f.Item.Qty = Limitsum;
                    hsNOREOnlyOneItem.Add(f);
                    hsREOnlyOneItem.Add(f.Item.ID + number, f);
                    hsREOnlylistItem.Add(f);
                }
                else
                {
                    if (f.FT.TotCost > 0 && f.FT.OwnCost > 0)
                    {
                        f.UndrugComb.Memo = GroupNumber;
                        hsNOREOnlyOneItem.Add(f);
                    }
                    else
                    {
                        f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                        f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                        f.UndrugComb.Memo = GroupNumber;
                        hsNOREOnlyOneItem.Add(f);
                    }
                }
            }
        }

        /// <summary>
        /// 按"第一件原价、其余件打折、总价封顶"规则计算费用，并将结果写入费用项及汇总集合
        /// </summary>
        /// <param name="f">待计算的费用项，包含单价、数量及费用结果字段</param>
        /// <param name="DISCOUNT_RATE">折扣率，例如 0.8 表示八折</param>
        /// <param name="TOPPRICE">费用封顶价；大于 0 时生效，超出部分按封顶价计算</param>
        /// <param name="hsREOnlyOneItem">以"费用项目编码 + 序号"为键的去重哈希表，用于防止同一项目重复计费</param>
        /// <param name="hsREOnlylistItem">费用项有序列表，用于后续统一汇总或输出</param>
        /// <param name="number">当前费用项的序号，与 ID 拼接后作为哈希表的唯一键</param>
        public void ConvertDiscountfee(
            FeeItemList f,
            decimal DISCOUNT_RATE,
            decimal TOPPRICE,
            ref Hashtable hsREOnlyOneItem,
            ref ArrayList hsREOnlylistItem,
            decimal number)
        {
            // 折价规则：第一件按原价，第二件起按折扣率计算
            // 公式：总费用 = 原价 + 原价 × 折扣率 × (数量 - 1)
            decimal cost = Convert.ToDecimal((f.Item.Price * DISCOUNT_RATE) * (f.Item.Qty - 1)) + f.Item.Price;

            // 若设置了封顶价且计算费用超出上限，则按封顶价收取
            if (TOPPRICE > 0 && cost > TOPPRICE)
                cost = TOPPRICE;

            // 将计算结果同步写入总费用与自付费用（当前规则下两者一致）
            f.FT.TotCost = cost;
            f.FT.OwnCost = cost;

            // 以"费用项目编码 + 序号"为键写入去重哈希表，避免同一项目重复计费
            hsREOnlyOneItem.Add(f.Item.ID + number, f);

            // 同步写入有序列表，供后续汇总或批量处理使用
            hsREOnlylistItem.Add(f);
        }

        public void ConvertDiscountfeeZY(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f, decimal DISCOUNT_RATE, int TOPPRICE, ref Hashtable hsREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number)
        {
            if (TOPPRICE > 0)
            {
                f.FT.TotCost = Convert.ToDecimal((f.Item.Price * DISCOUNT_RATE) * (f.Item.Qty - 1)) + f.Item.Price;
                f.FT.OwnCost = Convert.ToDecimal((f.Item.Price * DISCOUNT_RATE) * (f.Item.Qty - 1)) + f.Item.Price;
                if (f.FT.TotCost > TOPPRICE)
                {
                    f.FT.TotCost = TOPPRICE;
                    f.FT.OwnCost = TOPPRICE;
                }
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else
            {
                f.FT.TotCost = Convert.ToDecimal((f.Item.Price * DISCOUNT_RATE) * (f.Item.Qty - 1)) + f.Item.Price;
                f.FT.OwnCost = Convert.ToDecimal((f.Item.Price * DISCOUNT_RATE) * (f.Item.Qty - 1)) + f.Item.Price;
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);

            }
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

            decimal rate = f.Item.Price / orgGroupPrice;
            if (rate == 1)
            {
                rate = priceGroup / orgGroupPrice;
            }

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

        private ArrayList ConvertGroupToDetail1(FeeItemList f)
        {
            ArrayList undrugCombList = this.db.QueryUndrugPackagesBypackageCode(f.Item.ID);
            return undrugCombList;
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
