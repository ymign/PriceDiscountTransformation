using System;
using System.Collections.Generic;
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
    /// 历史限制收费与折价收费计算器。
    /// </summary>
    /// <remarks>
    /// 该类封装的是珠海旧 HIS 中最核心、也最难迁移的一批收费口径：
    /// 单项目限次、组套限次、床旁加收限制、胎心监护互斥、分组限次，以及“首项原价、其余折价”的金额改写规则。
    /// 它不负责界面交互，而专门负责把传入的 <see cref="FeeItemList"/> 改写成最终应收费金额和应保留数量。
    /// </remarks>
    public class Restrictingfee
    {
        #region 属性
        /// <summary>
        /// 规则数据访问入口。
        /// </summary>
        CTMRFeeRuleDB db = null;
        /// <summary>
        /// 是否启用 CT/MR 组套类特殊收费规则。
        /// </summary>
        private bool IsUseCtOrMRfeeRule = true;
        /// <summary>
        /// 当前收费过程缓存的项目主数据。
        /// </summary>
        DataSet dsItem = new DataSet();
        /// <summary>
        /// 项目查询时使用的科室编码。
        /// </summary>
        private string deptCode = "";
        /// <summary>
        /// 最近一次处理失败时的错误说明。
        /// </summary>
        public string errText = "";
        /// <summary>
        /// 当前患者挂号信息快照。
        /// </summary>
        protected Register rInfo = null;
        /// <summary>
        /// 是否处于转诊/转治场景。
        /// </summary>
        private bool isTransferTreat = false;
        #endregion

        /// <summary>
        /// 初始化历史限制收费/折价计算器。
        /// </summary>
        /// <remarks>
        /// 这个类本身不保存收费结果，但会在一次收费处理过程中频繁去数据库取历史规则、项目资料和既往收费次数，
        /// 所以构造时先把 <see cref="CTMRFeeRuleDB"/> 准备好，后面所有限制收费、组套拆分、折价计算都通过它取数。
        /// </remarks>
        public Restrictingfee()
        {
            db = new CTMRFeeRuleDB();
        }
        /// <summary>
        /// 对传入的门诊收费项目先做一轮“预处理”，把后续需要参与限制收费计算的明细整理出来。
        /// </summary>
        /// <param name="clincCode">
        /// 本次门诊收费对应的门诊流水号。
        /// 这里会先用它取患者挂号信息、合同单位信息等上下文，后面很多价格和折价判断都依赖这些资料。
        /// </param>
        /// <param name="feeArryList">
        /// 界面或上游逻辑传进来的原始收费项目集合。
        /// 里面既可能有普通项目，也可能有非药品组套主项，还可能混着 DR/CT 这类需要特殊处理的项目。
        /// </param>
        /// <returns>
        /// 返回一份“已经展开并清洗过”的收费明细集合。
        /// 后续真正做限制收费、历史次数判断、折价金额改写时，主要就是基于这个结果继续往下算。
        /// </returns>
        /// <remarks>
        /// 这个方法还没有真正开始算“能不能收费”，它更像正式计费前的整理入口：
        /// 1. 先补齐患者与合同单位上下文；
        /// 2. 再补齐开单医生所属科室；
        /// 3. 把需要拆的组套拆成明细；
        /// 4. 最后把 DR/CT 这种“只收一次”的集合级重复项提前清掉。
        /// 这样后续的限制收费方法拿到的就不是原始脏数据，而是一份更适合做规则计算的明细列表。
        /// </remarks>
        public ArrayList GetFeeItemList(string clincCode, ArrayList feeArryList)
        {
            // ========== 第一阶段：加载患者和项目上下文 ==========
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

            // ========== 第二阶段：补齐开单科室并做必要的组套展开 ==========
            for (int i = 0; i < feeArryList.Count; i++)
            {
                if (feeArryList[i] == null || !(feeArryList[i] is FeeItemList))
                {
                    // 原始列表里只要有空项或非费用对象，这里直接跳过，避免后面按收费对象取属性时报错。
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
                    // 命中 CT/MR 组套特殊路径后，不能直接把主项往结果里塞，
                    // 而是要先判断它是不是 ItemZT 这种特殊收费规则项目，再决定用哪种拆分方式。
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
                    // 普通项目或无需特殊拆分的项目，直接进入结果集，后面再统一做限制收费判断。
                    feeItemLists.Add(f);
                }

            }

            // ========== 第三阶段：对 DR/CT 特殊只收一次规则做集合级清洗 ==========
            for (int i = feeItemLists.Count - 1; i >= 0; i--)
            {
                FeeItemList f = feeItemLists[i] as FeeItemList;
                if (hsDROnlyOneItem.ContainsKey(f.Item.ID))
                {
                    // DR 特殊规则里，同一类只允许保留最终那一条，后续重复项直接删掉。
                    feeItemLists.RemoveAt(i);
                }
                if (hsCTOnlyOneItem.ContainsKey(f.Item.ID))
                {
                    if (hsCTOnlyOneItem[f.Item.ID].ToString() != "true")
                    {
                        // 第一次遇到允许保留的 CT 项时，把状态改成 true，表示它已经正式占位。
                        hsCTOnlyOneItem.Remove(f.Item.ID);
                        hsCTOnlyOneItem.Add(f.Item.ID, "true");
                    }
                    else
                    {
                        // 再次遇到同类 CT 项时说明已经有一条保留过了，这条就删掉。
                        feeItemLists.RemoveAt(i);
                    }
                }
            }
            foreach (DictionaryEntry de in hsDROnlyOneItem)
            {
                // DR 项里有一部分是前面暂存、最后统一补回的，这里把真正保留下来的项重新放回结果集。
                FeeItemList f = de.Value as FeeItemList;
                feeItemLists.Add(f);
            }

            return feeItemLists;
        }

        /// <summary>
        /// 对单个门诊明细项目执行限制收费计算。
        /// </summary>
        /// <param name="CARD_NO">患者卡号，用于查询历史已收费次数。</param>
        /// <param name="f">当前待处理收费明细。</param>
        /// <param name="hsREOnlyOneItem">记录本轮已被重算并准备替换的项目。</param>
        /// <param name="hsNOREOnlyOneItem">记录本次收费中仍保留原收费资格的项目，用于后续项目做互斥与累计判断。</param>
        /// <param name="hsREOnlylistItem">记录本轮运算后应重新回写到结果集的项目。</param>
        /// <param name="number">当前项目在原集合中的逻辑序号，用于构造唯一键。</param>
        /// <param name="LimitNumber">该项目配置的最大允许收费次数或数量。</param>
        /// <remarks>
        /// 这里的“限制收费”不是简单地全收或全不收，而是会结合：
        /// 1. 历史已收费次数；
        /// 2. 本次收费中已经保留的同类项目；
        /// 3. 床旁、胎心、分组收费等互斥关系；
        /// 4. 当前项目数量是否部分超限。
        /// 因此结果既可能是整条归零，也可能是数量截断后部分收费。
        /// </remarks>
        public void ConvertRestrictingfee(string CARD_NO, FeeItemList f, ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber)
        {
            // ========== 第一阶段：准备限制收费规则所需的分组与互斥字典 ==========
            string feecode = "";
            string GroupNumber = "";//组套组号
            Decimal feetype = 0;
            Decimal feeqty = 0;
            Hashtable hsCPItem = new Hashtable();
            Hashtable hsTXItem = new Hashtable();
            Hashtable hsTXxzItem = new Hashtable();
            Hashtable hsZTItem = new Hashtable();
            // Astrictpackagefee：组套内豁免项，不参与折价/历史次数扣减。
            ArrayList alfeecpxz = this.db.GetList("Astrictpackagefee");
            // RestrictingfeeZT：分组互斥规则，MARK/Memo 记录组号，同组只能收费一次。
            ArrayList alfeezt = this.db.GetList("RestrictingfeeZT");
            hsZTItem = this.BuildConstMemoHashtable(alfeezt, f.UndrugComb.ID, ref GroupNumber);
            hsTXxzItem = this.BuildConstItemHashtable(alfeecpxz);

            // ========== 第二阶段：先查询历史已收费次数 ==========
            // 如果项目属于分组收费，则历史次数也必须按分组维度回查，
            // 否则会把同编码但不同分组的收费错误地累计在一起。
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
                // 历史次数已经耗尽，当前项目整条保留但金额归零，方便前端继续展示收费痕迹。
                this.SetOutpatientAmount(f, 0);
                f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                this.RegisterOutpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
            }
            else
            {
                // ========== 第三阶段：叠加“本次收费过程中的已占用额度” ==========
                // 历史收费次数足够并不代表当前项目一定能收，
                // 因为同一次收费里的前序项目可能已经占掉了剩余额度。
                // RestrictingfeeCP：床旁加收项目限制。
                ArrayList alfeecp = this.db.GetList("RestrictingfeeCP");
                // RestrictingfeeTX1：胎心监护项目限制/互斥。
                ArrayList alfeetx = this.db.GetList("RestrictingfeeTX1");
                hsCPItem = this.BuildConstItemHashtable(alfeecp);
                hsTXItem = this.BuildConstItemHashtable(alfeetx);
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
                    // 本次收费中的前序项目已经把剩余额度耗尽，当前项目金额归零。
                    this.SetOutpatientAmount(f, 0);
                    f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                    this.RegisterOutpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
                }
                else if ((Limitsum - f.Item.Qty) <= 0)
                {
                    // 还有额度，但不足以覆盖当前整条数量，因此只保留剩余额度对应的数量和金额。
                    this.SetOutpatientCostsByQty(f, Limitsum);
                    f.Memo = "N" + Convert.ToDecimal(f.Item.Qty);
                    f.Item.Qty = Limitsum;
                    hsNOREOnlyOneItem.Add(f);
                    this.RegisterOutpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
                }
                else
                {
                    // 当前项目完全在可收费范围内，保留原有数量与金额。
                    if (f.FT.TotCost > 0 && f.FT.OwnCost > 0)
                    {
                        f.UndrugComb.Memo = GroupNumber;
                        hsNOREOnlyOneItem.Add(f);
                    }
                    else
                    {
                        this.SetOutpatientCostsByQty(f, f.Item.Qty);
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

            // ========== 第一阶段：准备组套限制收费所需的基础上下文 ==========
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
            // RestrictingfeeZT：止血/同组项目互斥，按组号累计历史次数。
            ArrayList alfeezt = this.db.GetList("RestrictingfeeZT");
            hsCPItem = this.BuildConstItemHashtable(alfeecp);
            hsTXItem = this.BuildConstItemHashtable(alfeetx);
            hsZTItem = this.BuildConstMemoHashtable(alfeezt, f.UndrugComb.ID, ref GroupNumber);
            if (drugFlag == "2")
            {
                // ========== 第二阶段：汇总项目按明细拆分后逐项重算 ==========
                // drugFlag == 2 表示这里本质是“汇总表现、明细收费”的组套项目，
                // 所以必须先拆细，再逐个明细判断剩余额度，最后把结果汇总回总项目。
                DateTime nowTime = this.db.GetDateTimeFromSysDateTime();
                int age = (int)((new TimeSpan(nowTime.Ticks - rInfo.Birthday.Ticks)).TotalDays / 365);
                alDetail = ConvertGroupToDetail1(f);
                // alDetail 只是“这个组套理论上有哪些子项”。
                // 真正的限制收费要在下面把每个子项单独拉出来，重新查价格、查历史次数、扣本次已占额度。
                foreach (UndrugComb undrugCombo in alDetail)
                {
                    // ========== 2A：准备当前子项的价格和历史收费次数 ==========
                    // 先把当前子项的价格资料取出来，再判断它历史上已经收了多少次。
                    rowFinds = dsItes.Tables[0].Select("ITEM_CODE = " + "'" + undrugCombo.ID + "'");
                    rowFind = rowFinds[0];
                    decimal unitPrice = NConvert.ToDecimal(rowFind["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFind["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFind["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFind["PURCHASE_PRICE"]);
                    undrugCombo.Package.ID = f.Item.ID;
                    Price = this.db.GetPrice(undrugCombo.ID, rInfo, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice);
                    feetype = this.db.getRestrictingfee(CARD_NO, undrugCombo.ID, ref feecode); //获取限制项目收费次数
                    if (hsZTItem.ContainsKey(f.Item.ID))
                    {
                        feetype = this.db.getRestrictingfeeZT(CARD_NO, undrugCombo.ID, GroupNumber, ref feecode);
                    }
                    returnRows = this.db.SetRestrictingfee(undrugCombo.ID, ref LimitNumber);
                    if (returnRows > 0)
                    {
                        // 当前子项本身配置了限制收费规则，这里的 Limitsum 先表示“扣掉历史次数以后还剩多少”。
                        // 注意：这还不是最终可收费数量，后面还要再减去“本次收费里已经占掉的数量”。
                        decimal Limitsum = LimitNumber - feetype;//计算是否次项目还可以收费有没有超越收费次数
                        if (Limitsum <= 0)
                        {
                            // 历史记录已经把这个子项的额度全部用完。
                            // 这里仍然按“金额加 0”处理，是为了让整个组套汇总流程继续走完，只是这个子项不再贡献金额。
                            sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty) - Convert.ToDecimal(Price * undrugCombo.Qty);
                        }
                        else
                        {
                            // ========== 2B：先检查本次收费里已经保留下来的普通收费项目 ==========
                            // hsNOREOnlyOneItem 存的是前面已经确认保留的 FeeItemList。
                            // 在这里有两种用途：
                            // 1. 普通限次项目：累计同编码项目已经占了多少数量；
                            // 2. 互斥项目：判断前面是否已经出现同类床旁/胎心/同组项目。
                            foreach (FeeItemList dsa in hsNOREOnlyOneItem)  //获取本次收费已经计算的数量
                            {
                                if (dsa.Item.ID == undrugCombo.ID)
                                {
                                    // 普通限次口径：前面已经保留过同编码项目，数量要继续累加。
                                    feeqty += Convert.ToDecimal(dsa.Item.Qty);
                                }
                                else if (hsCPItem.ContainsKey(f.Item.ID))
                                {
                                    // 床旁项目的口径不是“同编码累计”，而是“同类床旁项目互斥”。
                                    // 只要前面已经有床旁类项目保留，当前子项就直接失去收费资格。
                                    if (hsCPItem.ContainsKey(dsa.Item.ID))
                                    {
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsTXItem.ContainsKey(f.Item.ID))
                                {
                                    // 胎心项目看的是“是否已经有同类胎心组套/项目出现”，不是单纯比当前子项编码。
                                    if (hsTXItem.ContainsKey(dsa.UndrugComb.ID))
                                    {
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsZTItem.ContainsKey(f.Item.ID))
                                {
                                    // 分组收费更细：只有同属 ZT 规则且组号一致，才真正互斥。
                                    if (hsZTItem.ContainsKey(dsa.UndrugComb.ID))
                                    {
                                        if (dsa.UndrugComb.Memo == GroupNumber)
                                        {
                                            // 一旦前面已经有同组项目保留下来，当前子项就不用继续往后算了，直接判定为不能收。
                                            Limitsum = 0;
                                            break;
                                        }
                                    }
                                }

                            }

                            // ========== 2C：再检查前面别的组套拆出来、并已保留下来的子项 ==========
                            // hsZTNOREOnlyOneItem 不是普通收费明细，而是“前面组套拆出来并已经留下来的 UndrugComb 子项”。
                            // 如果不再扫这一遍，会漏掉“跨组套累计”和“跨组套互斥”的情况。
                            foreach (UndrugComb dsazt in hsZTNOREOnlyOneItem)  //获取本次收费已经计算的数量
                            {
                                if (hsCPItem.ContainsKey(undrugCombo.ID))
                                {
                                    // 当前子项属于床旁类时，要和前面任何组套里拆出的床旁类子项互斥。
                                    if (hsCPItem.ContainsKey(dsazt.ID))
                                    {
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsTXItem.ContainsKey(f.Item.ID))
                                {
                                    // 胎心口径这里看的是“前面那个拆分子项来自哪个组套主项”。
                                    // 因为旧规则更接近“同类组套互斥”，而不是简单子项编码互斥。
                                    if (hsTXItem.ContainsKey(dsazt.Package.ID))
                                    {
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsZTItem.ContainsKey(f.Item.ID))
                                {
                                    // 分组收费跨组套时，同样要按组号判断是否已经被前序子项占掉资格。
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
                                    // 如果当前不是互斥类口径，那前面其他组套拆出来的同编码子项，也会继续消耗当前额度。
                                    feeqty += Convert.ToDecimal(dsazt.Qty);
                                }
                            }

                            // 走到这里，Limitsum 才是真正的“历史次数 + 本次前序项目”全部扣完后的剩余额度。
                            Limitsum = Limitsum - feeqty;
                            if (Limitsum == 0)
                            {
                                // 刚好被前面的项目占满，当前子项不再收费。
                                sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty) - Convert.ToDecimal(Price * undrugCombo.Qty);
                            }
                            else if ((Limitsum - undrugCombo.Qty) <= 0)
                            {
                                // 剩余额度只够当前子项的一部分数量，所以这里走“部分保留、部分归零”。
                                sumPricecot += Convert.ToDecimal(Price * Limitsum);
                                undrugCombo.Qty = Limitsum;
                                hsZTNOREOnlyOneItem.Add(undrugCombo);
                            }
                            else
                            {
                                // 当前子项完整保留。
                                // 同时把组号写到 Memo，供后面同组互斥判断继续使用。
                                sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty);
                                undrugCombo.Memo = GroupNumber;
                                hsZTNOREOnlyOneItem.Add(undrugCombo);
                            }
                        }
                    }
                    else
                    {
                        // 当前细项本身不受限制收费控制，直接按计算价累计即可。
                        sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty);
                    }
                    feeqty = 0;
                }
                //f.Item.Price = sumPricecot;
                this.SetOutpatientAmount(f, sumPricecot);
                this.RegisterOutpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);

            }
            else
            {
                // ========== 第三阶段：普通项目路径，直接按单项限次逻辑判断 ==========
                feetype = this.db.getRestrictingfee(CARD_NO, f.Item.ID, ref feecode);
                if (hsZTItem.ContainsKey(f.Item.ID))
                {
                    feetype = this.db.getRestrictingfeeZT(CARD_NO, f.Item.ID, GroupNumber, ref feecode);
                }
                decimal Limitsum = LimitNumber - feetype;//计算是否次项目还可以收费有没有超越收费次数
                if (Limitsum <= 0)
                {
                    this.SetOutpatientAmount(f, 0);
                    this.RegisterOutpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
                }
                else
                {
                    // 这段和上面的组套子项路径思路一致，只是对象从“拆出来的组套子项”
                    // 变成了“当前这条普通收费项目本身”。
                    foreach (FeeItemList dsa in hsNOREOnlyOneItem)  //获取本次收费已经计算的数量
                    {

                        if (hsCPItem.ContainsKey(f.Item.ID))
                        {
                            // 床旁类项目：本次前面只要已经有同类床旁项目保留，当前项目就直接不能再收。
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
                            // 分组收费：不是所有 ZT 项目都互斥，只有同组号的前序项目才会顶掉当前项目。
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
                            // 非互斥类场景下，就按同编码项目继续累计本次已占数量。
                            feeqty += Convert.ToDecimal(f.Item.Qty);
                        }
                    }
                    foreach (UndrugComb dsazt in hsZTNOREOnlyOneItem)  //获取本次收费已经计算的数量
                    {
                        if (dsazt.ID == f.Item.ID)
                        {
                            // 前面别的组套拆出来的同编码子项，也会一起吃掉当前项目的剩余额度。
                            feeqty += Convert.ToDecimal(dsazt.Qty);
                        }
                        if (hsCPItem.ContainsKey(f.Item.ID))
                        {
                            // 普通项目和前面组套拆出的子项之间，同样可能存在床旁互斥关系。
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
                    Limitsum = Limitsum - feeqty;
                    if (Limitsum <= 0)
                    {
                        // 历史 + 本次前序累计之后，额度已经耗尽。
                        this.SetOutpatientAmount(f, 0);
                        this.RegisterOutpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
                    }
                    else if ((Limitsum - f.Item.Qty) <= 0)
                    {
                        // 额度还剩一点，但不够当前整条数量，因此只按剩余额度保留部分数量。
                        this.SetOutpatientCostsByQty(f, Limitsum);
                        f.Item.Qty = Limitsum;
                        hsNOREOnlyOneItem.Add(f);
                        this.RegisterOutpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
                    }
                    else
                    {
                        // 当前数量完全在可收费范围内，整条保留。
                        // 这里把组号塞回 Memo，是为了后面的同组项目继续识别“这条已经占了哪个组”的资格。
                        if (f.FT.TotCost > 0 && f.FT.OwnCost > 0)
                        {
                            f.UndrugComb.Memo = GroupNumber;
                            hsNOREOnlyOneItem.Add(f);
                        }
                        else
                        {
                            f.UndrugComb.Memo = GroupNumber;
                            this.SetOutpatientCostsByQty(f, f.Item.Qty);
                            hsNOREOnlyOneItem.Add(f);
                        }
                    }
                }
                feeqty = 0;
            }

        }

        /// <summary>
        /// 对住院收费明细执行限制收费计算。
        /// </summary>
        /// <param name="CARD_NO">
        /// 患者卡号。
        /// 主要用于回查该患者以前是否已经收过当前项目，或者是否已经在同组项目上占掉了限制次数。
        /// </param>
        /// <param name="f">
        /// 当前正在处理的住院收费明细。
        /// 它和门诊版的差别主要是对象类型不同，但业务判断口径基本一致。
        /// </param>
        /// <param name="hsREOnlyOneItem">
        /// 本轮已经被重算、并且需要回写的新项目登记表。
        /// 这里用它防止同一条住院明细被重复改写。
        /// </param>
        /// <param name="hsNOREOnlyOneItem">
        /// 本次住院收费里已经保留下来的项目清单。
        /// 后面的项目继续计算时，要把这些“本次已经保留的项目”一起算进累计额度里。
        /// </param>
        /// <param name="hsREOnlylistItem">
        /// 本轮最终要回写到结果集中的项目清单。
        /// 只要当前项目被截断、归零或重算，就会被加入这里。
        /// </param>
        /// <param name="number">
        /// 当前项目在本次处理里的逻辑顺序号。
        /// 主要拿来和项目编码拼成唯一键，避免哈希表键重复。
        /// </param>
        /// <param name="LimitNumber">
        /// 当前项目允许收费的最大上限值。
        /// 后续会拿它减去“历史已收次数”和“本次前序已占用数量”，算出还剩多少额度。
        /// </param>
        /// <remarks>
        /// 逻辑与门诊版 <see cref="ConvertRestrictingfee"/> 基本一致，
        /// 区别主要在于输入对象类型与住院收费集合的回写方式不同。
        /// 换句话说，门诊和住院虽然走的是两套费用对象，但历史限制收费、床旁互斥、胎心互斥、分组互斥这些口径并没有本质区别。
        /// </remarks>
        public void ConvertRestrictingfeeZY(string CARD_NO, Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f, ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber)
        {
            // 住院版和门诊版的算法骨架是同一套：
            // 先查历史次数，再扣本次前序项目，再做床旁/胎心/分组互斥。
            // 之所以没有强行合并成一个泛型方法，是因为旧 HIS 的门诊/住院费用对象虽然字段名接近，
            // 但真实类型不同、调用链也不同。这里保持两套公开入口，可以降低“为了抽象而抽象”带来的迁移风险。
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
            hsZTItem = this.BuildConstMemoHashtable(alfeezt, f.UndrugComb.ID, ref GroupNumber);
            hsTXxzItem = this.BuildConstItemHashtable(alfeecpxz);
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
                // 历史次数已经把额度耗尽时，旧口径不是删除项目，
                // 而是把金额压成 0 并保留原明细，便于前端继续展示“这条项目存在但不再收费”的痕迹。
                this.SetInpatientAmount(f, 0);
                f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                this.RegisterInpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
            }
            else
            {
                // 历史记录还有额度时，继续扣本次住院收费中前面已经保留的项目。
                ArrayList alfeecp = this.db.GetList("RestrictingfeeCP");
                ArrayList alfeetx = this.db.GetList("RestrictingfeeTX1");
                hsCPItem = this.BuildConstItemHashtable(alfeecp);
                hsTXItem = this.BuildConstItemHashtable(alfeetx);
                foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList dsa in hsNOREOnlyOneItem)  //获取本次收费已经计算的数量
                {

                    if (hsCPItem.ContainsKey(f.Item.ID))
                    {
                        // 床旁类互斥：前面已有床旁项目，就不再允许当前项目继续收费。
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
                        // 分组收费：只有同组号的前序项目，才算真正占了当前项目的资格。
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
                        // 非互斥类项目，则继续按同编码数量做本次累计。
                        feeqty += Convert.ToDecimal(dsa.Item.Qty);
                    }

                }
                Limitsum = Limitsum - feeqty;
                if (Limitsum <= 0)
                {
                    // 这里和历史次数耗尽是同一个落账策略：
                    // 不删除、不抛错，而是把金额归零并回写结果集。
                    this.SetInpatientAmount(f, 0);
                    f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                    this.RegisterInpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
                }
                else if ((Limitsum - f.Item.Qty) <= 0)
                {
                    // 剩余额度不足整条数量时，沿用旧口径“截断数量 + 按截断后数量重算金额”。
                    // 注意：这里会直接修改 f.Item.Qty，调用方后续看到的就是被截断后的数量，而不是原始录入数量。
                    this.SetInpatientCostsByQty(f, Limitsum);
                    f.Memo = "N" + Convert.ToDecimal(f.Item.Qty);
                    f.Item.Qty = Limitsum;
                    hsNOREOnlyOneItem.Add(f);
                    this.RegisterInpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
                }
                else
                {
                    // 当前项目完全在剩余额度范围内时，如果金额原本已经有值，就只登记资格；
                    // 如果金额还没初始化，再按“单价 * 数量”补齐金额。
                    if (f.FT.TotCost > 0 && f.FT.OwnCost > 0)
                    {
                        f.UndrugComb.Memo = GroupNumber;
                        hsNOREOnlyOneItem.Add(f);
                    }
                    else
                    {
                        this.SetInpatientCostsByQty(f, f.Item.Qty);
                        f.UndrugComb.Memo = GroupNumber;
                        hsNOREOnlyOneItem.Add(f);
                    }
                }
            }
        }

        /// <summary>
        /// 对门诊项目执行“首项原价、其余按折扣率收费”的折价计算。
        /// </summary>
        /// <param name="f">待折价的门诊收费项目。</param>
        /// <param name="DISCOUNT_RATE">折扣率，通常表示后续数量的收费比例。</param>
        /// <param name="TOPPRICE">封顶金额；小于等于 0 表示不封顶。</param>
        /// <param name="hsREOnlyOneItem">记录本轮已重算项目。</param>
        /// <param name="hsREOnlylistItem">记录需回写结果集的最终项目。</param>
        /// <param name="number">当前项目逻辑序号。</param>
        /// <remarks>
        /// 历史口径是“第一单位原价，其余单位按折扣率收费”，
        /// 并且在需要时再叠加一个总金额封顶值。
        /// </remarks>
        public void ConvertDiscountfee(FeeItemList f, decimal DISCOUNT_RATE, int TOPPRICE, ref Hashtable hsREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number)
        {
            // 先按旧公式算出“首项原价 + 其余折扣价”的基础金额。
            // 封顶判断故意放在后面，保持和历史代码相同的执行顺序。
            decimal discountAmount = this.CalculateDiscountAmount(f.Item.Price, f.Item.Qty, DISCOUNT_RATE);
            this.SetOutpatientAmount(f, discountAmount);
            if (TOPPRICE > 0)
            {
                // 旧折价口径：第一份原价，后面的份数按折扣率算，然后再看总金额是否要封顶。
                if (f.FT.TotCost > TOPPRICE)
                {
                    // 折后金额超过封顶时，不再继续保留计算结果，直接压到封顶价。
                    this.SetOutpatientAmount(f, TOPPRICE);
                }
            }

            // 没有封顶时，前面的折后金额就是最终结果。
            this.RegisterOutpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
        }

        /// <summary>
        /// 对住院项目执行和门诊版一致的折价金额改写。
        /// </summary>
        /// <param name="f">当前要改写金额的住院收费项目。</param>
        /// <param name="DISCOUNT_RATE">
        /// 折扣率。
        /// 旧规则的意思通常是“第一份按原价，后面的份数按这个比例收费”。
        /// </param>
        /// <param name="TOPPRICE">
        /// 封顶金额。
        /// 大于 0 表示最终总金额不能超过这个值；小于等于 0 表示只按折扣率算，不做封顶。
        /// </param>
        /// <param name="hsREOnlyOneItem">记录本轮已被重算过的项目，避免重复改价。</param>
        /// <param name="hsREOnlylistItem">记录本轮最终要回写到结果集中的项目。</param>
        /// <param name="number">当前项目逻辑序号，用于构造唯一键。</param>
        /// <remarks>
        /// 这个方法本身不判断“该不该折价”，它只负责在已经确定要折价时，
        /// 按旧系统的金额公式把住院项目的应收金额改写出来。
        /// </remarks>
        public void ConvertDiscountfeeZY(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f, decimal DISCOUNT_RATE, int TOPPRICE, ref Hashtable hsREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number)
        {
            // 住院版复用同一条金额公式，只是最终回写的对象类型不同。
            decimal discountAmount = this.CalculateDiscountAmount(f.Item.Price, f.Item.Qty, DISCOUNT_RATE);
            this.SetInpatientAmount(f, discountAmount);
            if (TOPPRICE > 0)
            {
                // 住院版沿用完全同一套金额公式，只是对象类型换成了住院费用对象。
                if (f.FT.TotCost > TOPPRICE)
                {
                    this.SetInpatientAmount(f, TOPPRICE);
                }
            }

            // 无封顶时直接保留折后金额；有封顶时则保留上面的压顶结果。
            this.RegisterInpatientRecalculatedItem(f, number, hsREOnlyOneItem, hsREOnlylistItem);
        }


        /// <summary>
        /// 按 CT/MR 特殊收费规则，把一个组套主项拆成可计费的子项明细。
        /// </summary>
        /// <param name="f">
        /// 当前要拆分的组套主项。
        /// 这个主项本身通常只是界面上的汇总展示，真正用于收费和限制收费计算的是拆出来的明细。
        /// </param>
        /// <param name="isFirst">
        /// 是否按“首项处理”口径拆分。
        /// CT/MR 老规则里常见“第一项原价、第二项起加收”或“只收第一项”这类口径，这个标记就是给这些规则分支使用的。
        /// </param>
        /// <param name="hsOnlyOneItem">
        /// 记录“只允许收一次”的特殊项目登记表。
        /// 拆分过程中如果命中了只收首项或只收一次规则，会在这里登记，供外层集合继续去重。
        /// </param>
        /// <returns>
        /// 返回组套拆出来的收费明细集合。
        /// 返回 null 表示拆分失败，通常意味着项目资料、价格资料或组套定义查询出了问题。
        /// </returns>
        /// <remarks>
        /// 这个方法不是普通“按组套定义平铺展开”，而是带有 CT/MR 特殊收费公式的拆分：
        /// 它会一边拆，一边决定每个子项该收多少数量、按原价还是加收价算、是否需要只收一次。
        /// </remarks>
        private ArrayList ConvertCTGroupToDetail(FeeItemList f, bool isFirst, ref Hashtable hsOnlyOneItem)
        {
            // ========== 第一阶段：读取 CT/MR 组套定义 ==========
            // 这里查到的是“这个组套主项下面理论上包含哪些子项”，
            // 后面还要结合价格规则、首项规则和只收一次规则，把它们转换成真正可收费的明细。
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

            // ========== 第二阶段：确保主项已经具备后续拆分所需的基础上下文 ==========
            // 组套拆分后的每个子项都要继承主项上的部分业务信息，
            // 例如医嘱流水号、患者信息、开单人、费用来源等，所以这里先保证主项自身是完整的。
            if (!this.EnsureOrderId(f))
            {
                return null;
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

            // ========== 第三阶段：逐个组套子项生成真正的收费明细 ==========
            // 这一段是核心：
            // 不是简单复制数据库里的组套定义，而是要把每个子项变成完整 FeeItemList，
            // 同时按 CT/MR 旧规则决定价格、数量、是否首项收费、是否只收一次。
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
                    // SortID == 3 代表“只收一次”这类 PACS 特殊口径。
                    // 这里不是简单删除重复项，而是先借助 hsOnlyOneItem 记录谁先占位、谁后淘汰。
                    if (hsOnlyOneItem.ContainsKey(undrugCombo.ID))
                    {
                        continue;
                    }
                    else
                    {
                        string itemName = rowFindZT["ITEM_NAME"].ToString();
                        if (itemName.Contains("三维重建"))
                        {
                            // 三维和四维重建之间有优先级关系。
                            // 四维一旦出现，会把前面临时保留的三维挤掉，所以这里先做占位标记。
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
                            // 四维优先级高于三维。
                            // 因此一旦出现四维，就要回头把之前临时占位的三维改成“真正淘汰”。
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
                            // 其他只收一次项目只需要登记“已经占位”即可。
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
                    // 这一段不是简单使用数据库默认单价，
                    // 而是按“项目基础价格 + 合同单位价格规则 + 年龄 + 组套内比例”重新计算子项实际收费单价。
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
                // 子项数量 = 主项数量 * 组套定义中该子项的倍数。
                // 也就是说主项开几次，这个子项就按组套比例展开多少次。
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

                this.CopyParentContextToDetail(f, feeDetail);
                this.ApplyPactItemRateForDetail(f, feeDetail);

                alTemp.Add(feeDetail);
            }

            // ========== 第四阶段：把主项上的减免、特殊自费等附加信息重新挂回拆分后的子项 ==========
            // 旧 HIS 的很多金额信息是先挂在主项上的。
            // 主项一旦被拆开，如果不把这些附加金额重新分配到子项上，后续收费落账金额就会和原界面显示对不上。
            if (!this.ApplyParentAdjustmentsToDetails(f, alTemp))
            {
                return null;
            }
            return alTemp;
        }
        /// <summary>
        /// 按普通非药品组套定义，把组套主项拆成收费明细。
        /// </summary>
        /// <param name="f">
        /// 当前要拆分的组套主项。
        /// 一般是非药品复合项目，界面传进来的是主项，真正收费要落到子项明细上。
        /// </param>
        /// <returns>
        /// 返回按照组套配置拆出来的明细列表。
        /// 如果查不到组套定义、项目资料或价格信息，会返回 null，并在 <see cref="errText"/> 中写失败原因。
        /// </returns>
        /// <remarks>
        /// 这是“普通组套”的拆分入口，不带 CT/MR 那些额外的历史收费公式。
        /// 它的主要职责是把主项还原成真实收费明细，并把主项上的价格、减免、特殊自费等信息合理分摊或挂接到子项上。
        /// </remarks>
        private ArrayList ConvertGroupToDetail(FeeItemList f)
        {
            // ========== 第一阶段：读取普通组套定义 ==========
            // 和 CT/MR 特殊组套不同，这里处理的是普通非药品组套，
            // 核心目的是把一个主项还原成若干真实收费子项。
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

            // ========== 第二阶段：补齐组套主项的收费基础数据 ==========
            // 后面生成的每个子项都要继承主项上的患者、处方、执行信息，
            // 所以在真正拆分前，先确保主项自身已经具备完整的医嘱流水和价格上下文。
            if (!this.EnsureOrderId(f))
            {
                return null;
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

            // ========== 第三阶段：逐个子项重建收费明细 ==========
            // 这里会把数据库里的组套成员，重新拼成完整 FeeItemList，
            // 包括项目编码、数量、单价、费用归属、医保比例、确认标志等。
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
                    // 普通组套这里同样不是简单照抄 UNIT_PRICE，
                    // 而是要结合转诊场景、患者年龄、合同单位和组套子项比例重新算出真正单价。
                    decimal unitPrice = NConvert.ToDecimal(rowFindZT["UNIT_PRICE"]);
                    decimal childPrice = NConvert.ToDecimal(rowFindZT["CHILD_PRICE"]);
                    decimal SPPrice = NConvert.ToDecimal(rowFindZT["SP_PRICE"]);
                    decimal purchasePrice = NConvert.ToDecimal(rowFindZT["PURCHASE_PRICE"]);

                    // 保存原始默认价格
                    feeDetail.Item.ChildPrice = unitPrice;

                    if (isTransferTreat == true)
                    {
                        // 转诊/转治场景下，旧逻辑直接采用 unitPrice，不再走完整组套价格换算。
                        decimal orgPrice = price;
                        itemRate = 1;// feeIntegrate.GetItemRateForZT(f.Item.ID, undrugCombo.ID);
                        price = unitPrice;// this.feeIntegrate.GetPrice(undrugCombo.ID, this.rInfo, age, unitPrice, childPrice, SPPrice, purchasePrice, ref orgPrice, itemRate);
                        feeDetail.OrgPrice = orgPrice;
                    }
                    else
                    {
                        // 正常场景下，仍按旧 HIS 的价格规则重新计算子项单价。
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
                // 子项最终数量 = 主项数量 * 组套中配置的子项数量。
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

                this.CopyParentContextToDetail(f, feeDetail);
                this.ApplyPactItemRateForDetail(f, feeDetail);

                alTemp.Add(feeDetail);
            }

            // ========== 第四阶段：把主项级别的附加金额重新分配回子项 ==========
            // 例如减免金额、特殊自费金额等，数据库原本未必按子项维护，
            // 所以拆完以后要重新找一个合适的子项承接，保证最终收费结果和主项原语义一致。
            if (!this.ApplyParentAdjustmentsToDetails(f, alTemp))
            {
                return null;
            }
            return alTemp;
        }

        /// <summary>
        /// 仅按组套关系查询原始组套明细，不做金额、数量和属性重建。
        /// </summary>
        /// <param name="f">当前组套主项。</param>
        /// <returns>
        /// 直接返回数据库里定义的组套明细列表。
        /// 这是一个轻量辅助方法，适合只想知道“这个组套下面挂了哪些子项”而不需要生成完整收费对象的场景。
        /// </returns>
        private ArrayList ConvertGroupToDetail1(FeeItemList f)
        {
            ArrayList undrugCombList = this.db.QueryUndrugPackagesBypackageCode(f.Item.ID);
            return undrugCombList;
        }

        /// <summary>
        /// 返回当前项目在当前合同单位下的协议比例信息。
        /// </summary>
        /// <param name="r">患者挂号/合同单位信息。</param>
        /// <param name="f">当前门诊收费项目。</param>
        /// <param name="errMsg">返回错误说明；当前实现基本未填充，仅保留旧接口形态。</param>
        /// <returns>
        /// 返回协议比例对象。
        /// 当前这份历史代码里只给了一个“默认减免比例为 0”的占位结果，说明这里原本预留过协议比例扩展，但实际未展开实现。
        /// </returns>
        /// <remarks>
        /// 这个方法目前更像保留接口，而不是完整业务逻辑。
        /// 真正的协议比例查询，多数情况下还是通过数据库层其他方法直接取数。
        /// </remarks>
        private Neusoft.HISFC.Models.Base.PactItemRate PactRate(Neusoft.HISFC.Models.Registration.Register r, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f, ref string errMsg)
        {
            Neusoft.HISFC.Models.Base.PactItemRate pRate = new Neusoft.HISFC.Models.Base.PactItemRate();
            pRate.Rate.RebateRate = 0;
            return pRate;
        }

        /// <summary>
        /// 确保组套主项已经具备稳定的医嘱流水号，便于后续所有拆分子项共用同一笔来源单据。
        /// </summary>
        /// <param name="feeItem">当前待拆分的组套主项。</param>
        /// <returns>
        /// true 表示当前主项已经拥有可用的 <c>Order.ID</c>；
        /// false 表示申请流水号失败，调用方应立即终止拆分流程。
        /// </returns>
        /// <remarks>
        /// 这一步虽然只是补一个字符串字段，但它决定了拆分后的子项是否还能被上游视为“同一笔医嘱”的组成部分。
        /// 因此失败时不能继续凑合往下走，而必须原地返回错误。
        /// </remarks>
        private bool EnsureOrderId(FeeItemList feeItem)
        {
            if (feeItem.Order.ID == null || feeItem.Order.ID == string.Empty)
            {
                feeItem.Order.ID = this.db.GetNewOrderID();
                if (feeItem.Order.ID == null || feeItem.Order.ID == string.Empty)
                {
                    this.errText = "获得医嘱流水号出错!";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 把主项中的待遇、来源和处方上下文复制到新建的组套子项上。
        /// </summary>
        /// <param name="parentItem">当前组套主项。</param>
        /// <param name="detailItem">当前正在构造的子项收费对象。</param>
        /// <remarks>
        /// 这些字段看起来像“普通复制”，但对旧 HIS 来说很关键：
        /// 一旦少复制某个字段，子项在后续结算、追溯、打印、执行科室识别里都可能表现成“孤儿明细”。
        /// </remarks>
        private void CopyParentContextToDetail(FeeItemList parentItem, FeeItemList detailItem)
        {
            detailItem.Order.ID = parentItem.Order.ID;
            detailItem.UndrugComb.ID = parentItem.Item.ID;
            detailItem.UndrugComb.Name = parentItem.Item.Name;
            detailItem.UndrugComb.Qty = parentItem.Item.Qty;
            detailItem.Order.Combo.ID = parentItem.Order.Combo.ID;
            detailItem.Item.IsMaterial = parentItem.Item.IsMaterial;
            detailItem.RecipeSequence = parentItem.RecipeSequence;
            detailItem.FTSource = parentItem.FTSource;
            detailItem.FeePack = parentItem.FeePack;

            // 复合项目拆细后，子项仍应保留主项的用法、申请单号、标本和检查部位记录。
            detailItem.Order.Usage = parentItem.Order.Usage;
            detailItem.Order.ApplyNo = parentItem.Order.ApplyNo;
            detailItem.Order.Sample.ID = parentItem.Order.Sample.ID;
            detailItem.Order.Sample.Name = parentItem.Order.Sample.Name;
            detailItem.Order.CheckPartRecord = parentItem.Order.CheckPartRecord;
        }

        /// <summary>
        /// 在协议/公费场景下，把主项的待遇属性补回拆分子项。
        /// </summary>
        /// <param name="parentItem">当前组套主项。</param>
        /// <param name="detailItem">当前拆出的子项。</param>
        /// <remarks>
        /// 旧系统里主项拆分前后的待遇属性必须连续。
        /// 这里仍完全保留原有分支：
        /// 1. 先按子项自身的协议目录判断；
        /// 2. 再用主项的 <c>ItemRateFlag</c> / 新旧比例做覆盖；
        /// 3. 未命中协议目录时也保留主项既有标志。
        /// </remarks>
        private void ApplyPactItemRateForDetail(FeeItemList parentItem, FeeItemList detailItem)
        {
            if (this.rInfo.Pact.PayKind.ID != "03")
            {
                return;
            }

            Neusoft.HISFC.Models.Base.PactItemRate pactRate = this.db.GetOnepPactUnitItemRateByItem(this.rInfo.Pact.ID, detailItem.Item.ID);
            if (pactRate != null)
            {
                if (pactRate.Rate.PayRate != this.rInfo.Pact.Rate.PayRate)
                {
                    if (pactRate.Rate.PayRate == 1)
                    {
                        detailItem.ItemRateFlag = "1";
                    }
                    else
                    {
                        detailItem.ItemRateFlag = "2";
                    }
                }
                else
                {
                    detailItem.ItemRateFlag = "2";
                }

                if (parentItem.ItemRateFlag == "3")
                {
                    detailItem.OrgItemRate = parentItem.OrgItemRate;
                    detailItem.NewItemRate = parentItem.NewItemRate;
                    detailItem.ItemRateFlag = "3";
                }

                return;
            }

            if (parentItem.ItemRateFlag == "3")
            {
                detailItem.OrgItemRate = parentItem.OrgItemRate;
                detailItem.NewItemRate = parentItem.NewItemRate;
                detailItem.ItemRateFlag = "3";
            }
            else
            {
                detailItem.OrgItemRate = parentItem.OrgItemRate;
                detailItem.NewItemRate = parentItem.NewItemRate;
                detailItem.ItemRateFlag = parentItem.ItemRateFlag;
            }
        }

        /// <summary>
        /// 计算组套拆分后“价格最高”的那条子项。
        /// </summary>
        /// <param name="detailItems">当前组套已生成的全部子项。</param>
        /// <returns>
        /// 返回价格最高的子项；如果集合为空则返回 null。
        /// </returns>
        /// <remarks>
        /// 旧代码里主项级别的特殊金额并不是平均分摊，而是挂到“价格最高”的那条子项上。
        /// 这个选择规则属于历史口径的一部分，所以这里只抽方法，不改变比较逻辑。
        /// </remarks>
        private FeeItemList FindHighestPriceDetail(ArrayList detailItems)
        {
            FeeItemList highestPriceDetail = null;
            decimal highestPrice = 0m;
            foreach (FeeItemList detailItem in detailItems)
            {
                if (detailItem.Item.Price > highestPrice)
                {
                    highestPriceDetail = detailItem;
                    highestPrice = detailItem.Item.Price;
                }
            }

            return highestPriceDetail;
        }

        /// <summary>
        /// 把主项上的减免、特殊自费和扩展金额重新挂回拆分后的子项。
        /// </summary>
        /// <param name="parentItem">当前组套主项。</param>
        /// <param name="detailItems">当前拆好的子项列表。</param>
        /// <returns>
        /// true 表示挂接成功；
        /// false 表示遇到不可继续的业务边界，例如非自费患者却试图分摊减免。
        /// </returns>
        /// <remarks>
        /// 这一步是拆分逻辑里最容易被误简化的部分：
        /// 减免按 ownCost 占比分摊，特殊自费和 FT.User03 则都挂到价格最高子项。
        /// 三种金额虽然都属于“主项级附加信息”，但继承策略并不相同，不能混成一个统一公式。
        /// </remarks>
        private bool ApplyParentAdjustmentsToDetails(FeeItemList parentItem, ArrayList detailItems)
        {
            if (detailItems.Count == 0)
            {
                return true;
            }

            if (parentItem.FT.RebateCost > 0)
            {
                if (this.rInfo.Pact.PayKind.ID != "01")
                {
                    this.errText = "暂时不允许非自费患者减免!";
                    return false;
                }

                decimal rebateRate = Neusoft.FrameWork.Public.String.FormatNumber(parentItem.FT.RebateCost / parentItem.FT.OwnCost, 2);
                decimal tempFix = 0;
                decimal tempRebateCost = 0;
                foreach (FeeItemList detailItem in detailItems)
                {
                    detailItem.FT.RebateCost = detailItem.FT.OwnCost * rebateRate;
                    tempRebateCost += detailItem.FT.RebateCost;
                }

                tempFix = parentItem.FT.RebateCost - tempRebateCost;
                FeeItemList firstDetail = detailItems[0] as FeeItemList;
                firstDetail.FT.RebateCost = firstDetail.FT.RebateCost + tempFix;
            }

            FeeItemList highestPriceDetail = this.FindHighestPriceDetail(detailItems);
            if (highestPriceDetail == null)
            {
                return true;
            }

            if (parentItem.SpecialPrice > 0)
            {
                highestPriceDetail.SpecialPrice = parentItem.SpecialPrice;
            }

            if (Neusoft.FrameWork.Function.NConvert.ToDecimal(parentItem.FT.User03) > 0)
            {
                highestPriceDetail.FT.User03 = parentItem.FT.User03;
            }

            return true;
        }

        /// <summary>
        /// 把常数字典列表按项目编码整理成哈希表，供限制收费逻辑做高频包含判断。
        /// </summary>
        /// <param name="constList">
        /// 从数据库常数字典表读出的配置集合。
        /// 例如床旁项目、胎心项目、组套豁免项等旧 HIS 规则，都会先以这个形态进入内存。
        /// </param>
        /// <returns>
        /// 返回以字典项 <c>ID</c> 为键、原始 <see cref="Const"/> 对象为值的哈希表。
        /// 这样后续逻辑就可以直接通过 <c>ContainsKey</c> 判断某个项目是否命中该类规则。
        /// </returns>
        /// <remarks>
        /// 这里没有做任何去重或容错增强，目的是保持和旧代码一致的语义：
        /// 旧代码如何遍历、如何因重复键抛错，这里就继续保留同样的行为，不偷偷改业务边界。
        /// </remarks>
        private Hashtable BuildConstItemHashtable(ArrayList constList)
        {
            Hashtable result = new Hashtable();
            foreach (Const item in constList)
            {
                result.Add(item.ID, item);
            }

            return result;
        }

        /// <summary>
        /// 把“按组号互斥”的常数字典整理成哈希表，并顺手找出当前项目所属的组号。
        /// </summary>
        /// <param name="constList">分组收费规则常数字典集合。</param>
        /// <param name="currentItemId">当前正在处理的项目或组套编码，用于定位它属于哪个分组。</param>
        /// <param name="matchedMemo">
        /// 输出当前项目匹配到的组号。
        /// 旧逻辑把组号放在字典的 <c>Memo</c> 字段里，后续同组互斥判断也是直接拿这个值继续传递。
        /// </param>
        /// <returns>
        /// 返回以字典项 <c>ID</c> 为键、组号 <c>Memo</c> 为值的哈希表。
        /// </returns>
        /// <remarks>
        /// 这个辅助方法只是在保留原始数据结构的前提下，避免多个限制收费方法重复写同一段循环。
        /// 它不改变任何分组判定口径，也不改变“最后一次命中哪个 Memo 就记哪个 Memo”的历史行为。
        /// </remarks>
        private Hashtable BuildConstMemoHashtable(ArrayList constList, string currentItemId, ref string matchedMemo)
        {
            Hashtable result = new Hashtable();
            foreach (Const item in constList)
            {
                if (item.ID == currentItemId)
                {
                    matchedMemo = item.Memo;
                }

                result.Add(item.ID, item.Memo);
            }

            return result;
        }

        /// <summary>
        /// 直接回写门诊项目的总金额和自付金额。
        /// </summary>
        /// <param name="feeItem">当前需要改写金额的门诊收费项目。</param>
        /// <param name="amount">
        /// 已经算好的最终金额。
        /// 这里会同时写入 <c>FT.TotCost</c> 和 <c>FT.OwnCost</c>，保持旧代码“总额与自付额同步改写”的行为。
        /// </param>
        /// <remarks>
        /// 老代码里这两个字段在本文件绝大多数场景都是成对出现的。
        /// 抽成统一入口的目的只是减少重复赋值，避免后续只改一个字段而破坏历史语义。
        /// </remarks>
        private void SetOutpatientAmount(FeeItemList feeItem, decimal amount)
        {
            feeItem.FT.TotCost = amount;
            feeItem.FT.OwnCost = amount;
        }

        /// <summary>
        /// 按“单价乘数量”的方式回写门诊项目金额。
        /// </summary>
        /// <param name="feeItem">当前门诊收费项目。</param>
        /// <param name="qty">
        /// 用于本次金额计算的数量。
        /// 这个值可能等于项目原数量，也可能是“剩余额度”这种部分保留数量。
        /// </param>
        private void SetOutpatientCostsByQty(FeeItemList feeItem, decimal qty)
        {
            this.SetOutpatientAmount(feeItem, Convert.ToDecimal(feeItem.Item.Price * qty));
        }

        /// <summary>
        /// 直接回写住院项目的总金额和自付金额。
        /// </summary>
        /// <param name="feeItem">当前需要改写金额的住院收费项目。</param>
        /// <param name="amount">已经算好的最终金额。</param>
        private void SetInpatientAmount(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem, decimal amount)
        {
            feeItem.FT.TotCost = amount;
            feeItem.FT.OwnCost = amount;
        }

        /// <summary>
        /// 按“单价乘数量”的方式回写住院项目金额。
        /// </summary>
        /// <param name="feeItem">当前住院收费项目。</param>
        /// <param name="qty">用于本次金额计算的数量。</param>
        private void SetInpatientCostsByQty(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem, decimal qty)
        {
            this.SetInpatientAmount(feeItem, Convert.ToDecimal(feeItem.Item.Price * qty));
        }

        /// <summary>
        /// 计算“首项原价、其余按折扣率收费”的历史折价金额。
        /// </summary>
        /// <param name="unitPrice">项目单价。</param>
        /// <param name="qty">项目数量。</param>
        /// <param name="discountRate">除首项外其余数量使用的折扣率。</param>
        /// <returns>按旧公式得到的折后总金额。</returns>
        /// <remarks>
        /// 这个公式是本文件折价逻辑的核心口径之一：
        /// 第一份永远按原价，其余份数统一乘折扣率。
        /// 这里只提炼公式，不引入任何封顶判断，封顶仍由调用方按原顺序控制。
        /// </remarks>
        private decimal CalculateDiscountAmount(decimal unitPrice, decimal qty, decimal discountRate)
        {
            return Convert.ToDecimal((unitPrice * discountRate) * (qty - 1)) + unitPrice;
        }

        /// <summary>
        /// 把已重算的门诊项目登记到“唯一键索引”和“最终回写列表”两个容器中。
        /// </summary>
        /// <param name="feeItem">本轮已经完成重算的门诊项目。</param>
        /// <param name="number">
        /// 当前处理顺序号。
        /// 旧代码使用“项目编码 + 顺序号”拼接成哈希键，这里保持完全一致。
        /// </param>
        /// <param name="hsREOnlyOneItem">按唯一键索引重算结果的哈希表。</param>
        /// <param name="hsREOnlylistItem">按处理顺序保存重算结果的列表。</param>
        private void RegisterOutpatientRecalculatedItem(FeeItemList feeItem, decimal number, Hashtable hsREOnlyOneItem, ArrayList hsREOnlylistItem)
        {
            hsREOnlyOneItem.Add(feeItem.Item.ID + number, feeItem);
            hsREOnlylistItem.Add(feeItem);
        }

        /// <summary>
        /// 把已重算的住院项目登记到“唯一键索引”和“最终回写列表”两个容器中。
        /// </summary>
        /// <param name="feeItem">本轮已经完成重算的住院项目。</param>
        /// <param name="number">当前处理顺序号。</param>
        /// <param name="hsREOnlyOneItem">按唯一键索引重算结果的哈希表。</param>
        /// <param name="hsREOnlylistItem">按处理顺序保存重算结果的列表。</param>
        private void RegisterInpatientRecalculatedItem(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem, decimal number, Hashtable hsREOnlyOneItem, ArrayList hsREOnlylistItem)
        {
            hsREOnlyOneItem.Add(feeItem.Item.ID + number, feeItem);
            hsREOnlylistItem.Add(feeItem);
        }

        #region 获取CT/MR收费规则的HashTable
        /// <summary>
        /// 把 CT/MR 特殊收费常数字典整理成内存哈希表，方便后续快速判断。
        /// </summary>
        /// <returns>
        /// 返回以项目编码为键、规则明细集合为值的哈希表。
        /// 每个值里会带出“按哪个公式收费”“数量怎么取整”“它属于 DR 还是 CT”等旧规则信息。
        /// </returns>
        /// <remarks>
        /// 旧系统里 CT/MR 规则不是强类型配置，而是塞在常数字典里，
        /// 这里的任务就是把那些字典字段重新翻译成代码更容易判断的结构，避免后面每次收费都重复解析同一堆字符串。
        /// </remarks>
        private Hashtable GetCTMRHashtabel()
        {
            // ========== 第一阶段：把旧常数字典整表取出来 ==========
            // 旧 HIS 没有把 CT/MR 收费规则建成结构化表，而是塞在常数字典 ItemZT 里。
            // 这里先整批取出来，再在内存里翻译成更好用的哈希结构。
            ArrayList alItemZT = this.db.GetAllList("ItemZT");
            Hashtable hsItemZT = new Hashtable();
            if (alItemZT != null)
            {
                hsItemZT = new Hashtable();
                foreach (Neusoft.HISFC.Models.Base.Const conObj in alItemZT)
                {
                    // ========== 第二阶段：跳过无效项，再把一个常数字典项拆成多个收费子项规则 ==========
                    // 一个 conObj 里往往不是一条规则，而是一组子项编码 + 一套收费公式描述，
                    // 所以后面会按 Memo 里的多个 itemID 逐个展开成 NeuObject。
                    Neusoft.FrameWork.Models.NeuObject obj = null;
                    if (!conObj.IsValid)
                    {
                        continue;
                    }
                    if (hsItemZT.ContainsKey(conObj.Name))
                    {
                        // 同一个主项编码下可能挂多条子项规则，所以这里不是覆盖旧值，而是继续往现有集合里追加。
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
                                    // 每个项目都按规则收费
                                    obj.Memo = "每个项目收取";
                                    break;
                                case "1":
                                    // 只有第一项按完整规则收费
                                    obj.Memo = "第一个项目收取";
                                    break;
                                case "2":
                                    // 第一项之后的项目走加收口径
                                    obj.Memo = "第二个项目起加收";
                                    break;
                                case "3":
                                    // 整组只收一次
                                    obj.Memo = "只收取一次";
                                    break;

                            }

                            //obj.Memo = temps[2];//公式 0 每个项目收取、1 第一个项目收取、2 第二个项目起加收
                            switch (conObj.SpellCode)
                            {
                                case "0":
                                    // 总量统一取整
                                    obj.User01 = "总量取整";
                                    break;
                                case "1":
                                    // 每个子项单独取整
                                    obj.User01 = "单个取整";
                                    break;
                                case "2":
                                    // 使用固定数量，不再按实际数量变化
                                    obj.User01 = "固定数量";
                                    break;
                            }
                            //obj.User01 = conObj.SpellCode;//0 总量取整、1 单个取整 2固定数量
                            switch (conObj.UserCode)
                            {
                                case "0":
                                    // DR 规则
                                    obj.User02 = "DR";
                                    break;
                                case "1":
                                    // CT 规则
                                    obj.User02 = "CT";
                                    break;
                            }
                            //obj.User02 = conObj.UserCode;//0 DR 1 CT

                            ((ArrayList)hsItemZT[conObj.Name]).Add(obj);
                        }
                    }
                    else
                    {
                        // 第一次遇到这个主项编码时，先创建一个新的规则集合，再把子项规则逐个放进去。
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
        /// 给收费项目补齐开方医生所属科室。
        /// </summary>
        /// <param name="feeItemList">
        /// 当前准备参与收费处理的项目列表。
        /// 里面有些项目可能只有开单人工号，没有科室编码，这里会逐条补齐。
        /// </param>
        /// <returns>
        /// true 表示全部补齐成功；
        /// false 表示中途发现医生信息或医生所属科室缺失，无法继续安全计算。
        /// </returns>
        /// <remarks>
        /// 旧收费逻辑里有些限制收费或价格查询会按医生所在科室分支处理，
        /// 所以如果 DoctDeptInfo 没补上，后面很容易查错项目资料或走错收费口径。
        /// </remarks>
        private bool AssignmentDoctDeptInfo(List<FeeItemList> feeItemList)
        {
            // 这一步本质上是收费前置校验，不只是“顺手补字段”。
            // 旧逻辑里有些价格和限制收费判断默认开单医生科室已经存在，所以这里一旦补不出来，就必须停止后续处理。
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
