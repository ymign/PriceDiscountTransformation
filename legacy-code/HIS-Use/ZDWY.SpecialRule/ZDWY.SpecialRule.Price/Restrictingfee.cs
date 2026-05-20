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
    /// </summary>
    /// <remarks>
    /// 这个类承载 HIS 旧逻辑中最难理解的一部分：限制收费、同组互斥、组套拆分后重算、
    /// 以及“第一件原价、其余折价、最高封顶”的折价规则。
    ///
    /// 核心规则可以拆成两大类：
    /// 1. Restrictingfee 系列：按最近 2 小时历史收费量、本次已保留量、同组互斥清单决定本次还能收多少；
    /// 2. FIN_DISCOUNT_FEE 系列：按 DISCOUNT_RATE 执行“第一件原价、第二件起比例折价”，再用 TOPPRICE 做总金额封顶。
    ///
    /// 该类不是单纯的公式库，也不是只读校验器。它同时承担了以下旧系统职责：
    /// 1. 查询 COM_DICTIONARY / COM_SQL / FIN_DISCOUNT_FEE；
    /// 2. 拆组套、按患者合同单位重新取价；
    /// 3. 在调用方传入的集合中登记“旧行要删除、新行要追加”；
    /// 4. 直接改写 FeeItemList 的数量、金额和 Memo。
    ///
    /// 需要特别注意：这里的方法大多不是纯计算函数，而是会直接修改传入的 FeeItemList：
    /// 1. 修改 <c>FT.TotCost</c> / <c>FT.OwnCost</c>；
    /// 2. 部分超限时修改 <c>Item.Qty</c>；
    /// 3. 通过 <c>Memo</c> 标记原数量，供后续界面或流程还原；
    /// 4. 把需要替换回收费列表的对象登记到 Hashtable / ArrayList。
    ///
    /// 因此阅读本类时不要只看返回值。真正的输出在入参对象和几个集合里。
    /// 如果迁移到新规则中心，不能只按“方法返回集合”建模，而要把这些副作用拆成：
    /// 规则匹配、公式计算、限额占用、同组互斥、追溯步骤和最终明细替换。
    /// </remarks>
    public class Restrictingfee
    {
        #region 属性
        /// <summary>
        /// 数据库访问对象，负责读取 COM_SQL、COM_DICTIONARY、FIN_DISCOUNT_FEE 和历史收费明细。
        /// </summary>
        CTMRFeeRuleDB db = null;
        /// <summary>
        /// 是否启用 CT/MR 组套拆分与去重规则。旧逻辑里它和限制收费共用同一个类，职责并不单一。
        /// </summary>
        private bool IsUseCtOrMRfeeRule = true;
        /// <summary>
        /// 项目主数据缓存，组套拆分和价格重算时使用。
        /// </summary>
        DataSet dsItem = new DataSet();
        /// <summary>
        /// 查询项目主数据时使用的科室编码。旧逻辑中默认空值，实际含义依赖调用方上下文。
        /// </summary>
        private string deptCode = "";
        /// <summary>
        /// 最近一次处理失败或规则异常时的错误文本。调用方通过该字段拿错误信息。
        /// </summary>
        public string errText = "";
        /// <summary>
        /// 当前挂号/就诊信息，价格重算、年龄判断、合同单位取价时会使用。
        /// </summary>
        protected Register rInfo = null;
        /// <summary>
        /// 转诊治疗标志。当前片段未看到核心限制收费分支使用，保留历史状态位。
        /// </summary>
        private bool isTransferTreat = false;
        #endregion

        /// <summary>
        /// 初始化历史特殊收费规则处理器。
        /// </summary>
        /// <remarks>
        /// 构造时只创建数据库访问对象，不加载任何规则缓存。
        /// 这意味着每次执行限制收费或折价时仍可能访问数据库读取字典、SQL 或项目明细，
        /// 性能上不理想，但符合旧 HIS “规则实时生效”的使用习惯。
        /// 迁移新规则中心时应把这种运行时查询拆成规则缓存和历史占用查询两类职责。
        /// </remarks>
        public Restrictingfee()
        {
            db = new CTMRFeeRuleDB();
        }
        /// <summary>
        /// 独立规则类入口：根据门诊流水号重新读取患者上下文，并把传入的费用明细按 CT/MR/DR 组套规则展开。
        /// </summary>
        /// <param name="clincCode">门诊就诊流水号，用于读取挂号信息、合同单位和患者上下文。</param>
        /// <param name="feeArryList">调用方传入的原始费用明细集合。</param>
        /// <returns>
        /// 经过组套拆分后的费用明细集合；发生组套明细读取错误时返回 <c>null</c> 并写入 <see cref="errText"/>。
        /// </returns>
        /// <remarks>
        /// 这个方法容易和 <c>ucDisplay.GetFeeItemList</c> 混淆。它主要负责“组套展开/去重”的公共处理，
        /// 并没有在当前代码片段中直接调用 <see cref="ConvertRestrictingfee"/> 或 <see cref="ConvertDiscountfee"/>。
        /// 门诊界面真正执行限制收费和比例折价的位置仍在 <c>ucDisplay.SetChargeInfo</c>、
        /// <c>ucDisplay.SetItem</c> 和 <c>ucDisplay.GetFeeItemList</c>。
        ///
        /// 换句话说，本方法是特殊规则类的“组套预处理入口”，不是完整折价入口。
        /// 迁移到新规则中心时，不能只对照这个方法，否则会漏掉 Restrictingfee / Discountfee 的主流程。
        /// </remarks>
        public ArrayList GetFeeItemList(string clincCode, ArrayList feeArryList)
        {
            // ========== 第一阶段：初始化组套拆分和去重状态 ==========
            // DR/CT 组套在旧 HIS 中不是简单展开全部明细。
            // 这里用 hsDROnlyOneItem、hsCTOnlyOneItem 记录“只保留一次”的项目，
            // 后续倒序删除重复明细，再把需要保留的 DR 汇总项追加回集合。
            bool isFindDRFirst = false;
            bool isFindCTFirst = false;
            Hashtable hsDROnlyOneItem = new Hashtable();
            Hashtable hsCTOnlyOneItem = new Hashtable();
            decimal drCount = 0;
            ArrayList feeItemLists = new ArrayList();
            Hashtable hsDoct = new Hashtable();

            // ========== 第二阶段：恢复患者上下文和项目主数据 ==========
            // clincCode 用于重新读取挂号记录。读取后又重新查合同单位，是因为价格计算依赖 Pact.PriceForm、
            // 儿童价/特价/合同单位价等上下文。这里先保留 PayKind.ID，再覆盖 Pact，最后再还原 PayKind.ID，
            // 属于旧 HIS 中常见的“补全合同单位但保留原支付类别”的兼容写法。
            this.rInfo = this.db.GetByClinic(clincCode);
            string tempPayKindid = this.rInfo.Pact.PayKind.ID;
            this.rInfo.Pact = this.db.GetPactUnitInfoByPactCode(this.rInfo.Pact.ID);
            this.rInfo.Pact.PayKind.ID = tempPayKindid;
            this.db.QueryItemList(deptCode, Neusoft.HISFC.Models.Base.ItemKind.All, ref dsItem);

            // ========== 第三阶段：逐条读取原始费用，必要时拆分组套 ==========
            // 该循环只做“明细展开”和“医生科室补齐”，不执行 Restrictingfee / Discountfee。
            // 真正的限制收费入口在 ucDisplay.SetChargeInfo、ucDisplay.SetItem、ucDisplay.GetFeeItemList。
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
                    // 当前项目是非药品组套，并且启用了 CT/MR/DR 特殊规则。
                    // 如果在 CTMR 字典中，则按 DR/CT 特殊去重逻辑处理；
                    // 如果不在字典中，则按普通组套 ConvertGroupToDetail 展开。
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

            // ========== 第四阶段：执行 CT/DR 展开后的重复明细清理 ==========
            // 前面拆组套时会把所有明细先放入 feeItemLists。
            // 这里倒序删除重复项，是为了避免 RemoveAt 后下标移动导致漏删。
            // DR 项目还会从 hsDROnlyOneItem 追加回最终集合，保留旧逻辑的汇总口径。
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

            // ========== 第五阶段：返回组套预处理结果 ==========
            // 本方法不做最终折价，只返回拆分/去重后的明细集合。
            // 调用方如果要得到最终可收费金额，仍需要继续调用限制收费或比例折价方法。
            return feeItemLists;
        }

        /// <summary>
        /// 门诊普通明细项目的限制收费计算。
        /// </summary>
        /// <param name="CARD_NO">
        /// 患者卡号。历史 SQL 会用它向前查询 2 小时内门诊/住院已收费记录。
        /// 旧 SQL 同时 UNION 门诊和住院，但两个分支都使用同一个入参，
        /// 所以是否真的跨门诊住院累计，取决于调用方传入的是卡号还是住院流水号。
        /// </param>
        /// <param name="f">
        /// 当前正在处理的收费明细。方法会直接修改它的数量、金额和 Memo。
        /// </param>
        /// <param name="hsREOnlyOneItem">
        /// 已被限制收费逻辑接管、后续需要从原列表删除的项目索引表。
        /// 键是“项目编码 + 当前遍历序号”，不是单纯项目编码，目的是区分同一次收费中的重复项目行。
        /// </param>
        /// <param name="hsNOREOnlyOneItem">
        /// 本次收费中已经通过限制校验、可以保留收费的普通项目集合。
        /// 后续项目会把它作为“本次已占用量”继续扣减，避免同一次收费动作内突破上限。
        /// </param>
        /// <param name="hsREOnlylistItem">
        /// 重算后的最终项目集合。调用方会先删除被接管的旧行，再把该集合追加回收费列表。
        /// </param>
        /// <param name="number">
        /// 当前遍历序号。它不是收费数量，只用于和项目编码拼接出本次处理的临时键。
        /// </param>
        /// <param name="LimitNumber">
        /// 字典 Restrictingfee.INPUT_CODE 配置的允许收费上限。
        /// 典型语义是 2 小时窗口内最多允许收费的数量。
        /// </param>
        /// <remarks>
        /// 该方法的核心公式不是金额折扣，而是“剩余额度”：
        /// <code>
        /// 剩余额度 = 字典上限 - 历史 2 小时已收费量 - 本次已保留量
        /// </code>
        /// 剩余额度小于等于 0 时，本行金额改为 0；
        /// 剩余额度不足本行数量时，本行数量被截断为剩余额度，金额按截断后的数量计算；
        /// 剩余额度足够时，本行按原金额保留。
        ///
        /// RestrictingfeeCP、RestrictingfeeTX1、RestrictingfeeZT 不是普通数量累计：
        /// 它们会把“同组已有项目”直接视为已占满额度，从而把后续同组项目归零。
        /// </remarks>
        public void ConvertRestrictingfee(string CARD_NO, FeeItemList f, ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber)
        {
            // ========== 第一阶段：准备计算变量和规则字典 ==========
            // Price 在旧代码中预留但当前分支主要使用 f.Item.Price。
            // feecode 由历史 SQL 输出，用于调试时识别命中的历史收费项目编码。
            // feetype 表示历史已收费数量或同组互斥命中标志；feeqty 表示本次收费动作内已保留数量。
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

            // ========== 第二阶段：读取同组互斥配置 ==========
            // RestrictingfeeZT 使用 COM_DICTIONARY.MARK/Memo 作为组号。
            // 当前项目如果属于该组，后续历史查询会改用 getRestrictingfeeZT，
            // 含义从“查同项目历史数量”变成“查同组历史是否已有收费”。
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

            // ========== 第三阶段：读取历史 2 小时已收费量 ==========
            // Astrictpackagefee 是历史例外清单。
            // 当前项目属于该清单时，不查询历史收费量，直接把历史已占用量视为 0。
            // 这不是“不限制”，只是忽略历史记录，本次收费内仍可能被后续逻辑扣减。
            if (hsTXxzItem.ContainsKey(f.UndrugComb.ID))
            {
                feetype = 0;
            }
            else
            {
                // RestrictingfeePay2 查询最近 2 小时已收费量。
                // 如果当前项目属于 RestrictingfeeZT，则改查同 MARK 组内历史，SQL 返回 99 表示已存在互斥收费。
                feetype = this.db.getRestrictingfee(CARD_NO, f.Item.ID, ref feecode);
                if (hsZTItem.ContainsKey(f.UndrugComb.ID))
                {
                    feetype = this.db.getRestrictingfeeZT(CARD_NO, f.Item.ID, GroupNumber, ref feecode);
                }
            }

            // ========== 第四阶段：先扣历史已收费量 ==========
            // 旧 SQL 已经排除了 OWN_COST=0、退费/作废等记录。
            // 如果当前项目是 ZT 同组互斥，feetype 可能不是实际数量，而是 SQL 返回的 99，
            // 用于把 Limitsum 直接压成负数，达到“已有同组项目则当前项目归零”的效果。
            Decimal Limitsum = LimitNumber - feetype;//计算是否次项目还可以收费有没有超越收费次数
            if (Limitsum <= 0)
            {
                // 历史已占满：本行保留在结果集中，但金额改成 0。
                // Memo=P{原数量} 表示本行全部超限；后续流程可能依赖 Memo 还原或展示原数量。
                //f.Item.Price = Convert.ToDecimal(Price * f.Item.Qty) - Convert.ToDecimal(Price * f.Item.Qty);
                f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else
            {
                // ========== 第五阶段：再扣本次收费已经保留的项目 ==========
                // 同一次收费动作里如果有多条相同项目或同组互斥项目，不能只看数据库历史，
                // 否则用户一次录入多条时会绕过 2 小时上限。
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

                    // RestrictingfeeCP：床旁项目共享一组额度。当前项是床旁项目时，
                    // 只要本次已保留项目里也有床旁项目，就把剩余额度置 0。
                    // 注意：这里不是按同项目累计，而是任意 CP 项目之间互斥/共享额度。
                    if (hsCPItem.ContainsKey(f.Item.ID))
                    {

                        if (hsCPItem.ContainsKey(dsa.Item.ID))
                        {
                            Limitsum = 0;
                            break;
                        }
                    }
                    // RestrictingfeeTX1：胎心项目按组套编码互斥。
                    // 本次已保留过任一胎心组套后，后续胎心项目归零。
                    // 旧 SQL 历史侧返回 99；本次收费侧则通过 Limitsum=0 达到同样效果。
                    else if (hsTXItem.ContainsKey(f.UndrugComb.ID))
                    {

                        if (hsTXItem.ContainsKey(dsa.UndrugComb.ID))
                        {
                            Limitsum = 0;
                            break;
                        }
                    }
                    // RestrictingfeeZT：同 MARK 组互斥。
                    // GroupNumber 写在 UndrugComb.Memo 里，这个字段在这里被临时当作“组号缓存”使用。
                    // 这也是为什么前面“全部可收费”分支要把 GroupNumber 写回 Memo。
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
                        // 普通限制项目才按同项目数量累计。
                        feeqty += Convert.ToDecimal(dsa.Item.Qty);
                    }

                }
                Limitsum = Limitsum - feeqty;

                // ========== 第六阶段：根据最终剩余额度改写当前费用明细 ==========
                // 这里有三个结果：
                // 1. Limitsum <= 0：全部超限，金额归零，Memo=P{原数量}；
                // 2. Limitsum < 当前数量：部分可收，数量截断，Memo=N{原数量}；
                // 3. Limitsum 足够：完整保留，并登记到 hsNOREOnlyOneItem 供后续项目继续扣减。
                if (Limitsum <= 0)
                {
                    // 历史 + 本次已保留量已经占满，本行全部归零。
                    f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                    hsREOnlyOneItem.Add(f.Item.ID + number, f);
                    hsREOnlylistItem.Add(f);
                }
                else if ((Limitsum - f.Item.Qty) <= 0)
                {
                    // 部分可收费：金额按剩余额度计算，数量被改成剩余额度。
                    // Memo=N{原数量} 用来保留被截断前的原数量。
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
                    // 剩余额度足够：保留本行。若原金额为 0，则按单价×数量补回金额。
                    // 同组互斥的组号继续写入 UndrugComb.Memo，供后续项目判断同组占用。
                    // 这里不写 hsREOnlyOneItem / hsREOnlylistItem，因为原行不需要删除替换，只要保留即可。
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
        ///
        /// 这个方法主要服务 <c>ucDisplay.SetChargeInfo</c> 和 <c>ucDisplay.SetItem</c> 的界面显示/录入试算。
        /// 最终提交收费时，<c>ucDisplay.GetFeeItemList</c> 会先把组套拆成真实收费明细，
        /// 然后调用 <see cref="ConvertRestrictingfee"/> 逐条处理，所以不要把本方法误认为唯一收费口径。
        /// </remarks>
        public void ConvertRestrictingfeeCharge(string CARD_NO, FeeItemList f, ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber, ref ArrayList hsZTNOREOnlyOneItem, DataSet dsItes, Register rInfo)
        {
            // ========== 第一阶段：初始化门诊显示价/录入试算所需上下文 ==========
            // 本方法服务界面显示，不直接产生最终收费明细。
            // 它会把组套拆成子项计算，但最终仍把金额汇总回传入的组套主项 f。
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

            // ========== 第二阶段：重新读取当前主项的有效项目资料 ==========
            // 这里没有直接使用传入 dsItes 中的主项行，而是重新按有效项目查询。
            // 旧 HIS 这样做的目的通常是避免组套主项已停用或缓存脏数据导致显示价继续使用旧资料。
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

            // ========== 第三阶段：把规则字典装入哈希表，方便后续快速判断 ==========
            // 旧代码大量使用 Hashtable.ContainsKey 来判断某个项目是否属于 CP/TX/ZT。
            // 这里保留原结构，不改成 LINQ 或泛型集合，避免影响 .NET Framework 旧工程兼容性。
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
                // ========== 第四阶段A：非药品组套分支 ==========
                // 界面上传进来的是组套主项，但限制收费实际要落到组套明细子项上。
                // 这里先把组套拆成 UndrugComb 明细，再逐个子项查询历史 2 小时收费量和本次已保留量，
                // 最后把可收费子项金额累计到 sumPricecot，并写回主项 f.FT。
                DateTime nowTime = this.db.GetDateTimeFromSysDateTime();
                int age = (int)((new TimeSpan(nowTime.Ticks - rInfo.Birthday.Ticks)).TotalDays / 365);
                alDetail = ConvertGroupToDetail1(f);
                foreach (UndrugComb undrugCombo in alDetail)
                {
                    // 子项价格不能直接用组套主项价格，需要按患者合同单位、儿童价/特价等重新取价。
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
                        // 主项属于 RestrictingfeeZT 时，子项历史不再按单个项目累计，
                        // 而是按主项所在的同组组号判断是否已有互斥收费。
                        feetype = this.db.getRestrictingfeeZT(CARD_NO, undrugCombo.ID, GroupNumber, ref feecode);
                    }

                    #region 限制收费项目规则

                    //获取本项目维护的限制收费的次数
                    returnRows = this.db.SetRestrictingfee(undrugCombo.ID, ref LimitNumber);
                    if (returnRows > 0)
                    {
                        // ========== 第四阶段A-1：子项维护了 Restrictingfee，计算子项剩余额度 ==========
                        // 对组套来说，LimitNumber 以子项维护为准，不以主项维护为准。
                        // 这样才能复刻旧系统“组套主项显示，但限制收费实际落到明细项目”的行为。
                        //计算本项目本次剩余的收费次数
                        decimal Limitsum = LimitNumber - feetype;
                        if (Limitsum <= 0)
                        {
                            // 子项历史已占满：该子项本次金额贡献为 0，但仍继续处理其他子项。
                            sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty) - Convert.ToDecimal(Price * undrugCombo.Qty);
                        }
                        else
                        {
                            // ========== 第四阶段A-2：扣减本次收费动作内已保留项目 ==========
                            // 子项历史还有额度时，还要扣掉本次收费动作中已经保留的普通明细和组套明细。
                            // 这两组集合分开维护，是因为同一次收费中可能既有普通项目，也有组套拆出的子项。
                            foreach (FeeItemList dsa in hsNOREOnlyOneItem)  //获取本次收费已经计算的数量
                            {
                                if (dsa.Item.ID == undrugCombo.ID)
                                {
                                    // 同一子项在本次收费动作中已保留的数量。
                                    feeqty += Convert.ToDecimal(dsa.Item.Qty);
                                }
                                else if (hsCPItem.ContainsKey(f.Item.ID))
                                {

                                    if (hsCPItem.ContainsKey(dsa.Item.ID))
                                    {
                                        // 床旁组内已有项目，本组套子项视为不能再收费。
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsTXItem.ContainsKey(f.Item.ID))
                                {

                                    if (hsTXItem.ContainsKey(dsa.UndrugComb.ID))
                                    {
                                        // 胎心组内已有项目，本组套子项视为不能再收费。
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
                                // hsZTNOREOnlyOneItem 保存的是前面组套拆出来并已经保留的子项。
                                // 如果当前也是 CP/TX/ZT 同组项目，不能只看普通项目集合，否则两个组套之间会突破互斥限制。
                                if (hsCPItem.ContainsKey(undrugCombo.ID))
                                {

                                    if (hsCPItem.ContainsKey(dsazt.ID))
                                    {
                                        // 另一个组套已经拆出并保留了床旁子项，当前子项归零。
                                        Limitsum = 0;
                                        break;
                                    }
                                }
                                else if (hsTXItem.ContainsKey(f.Item.ID))
                                {

                                    if (hsTXItem.ContainsKey(dsazt.Package.ID))
                                    {
                                        // 另一个组套已保留胎心组套，当前胎心组套归零。
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
                                            // 同 MARK 组的其他组套子项已保留，当前子项归零。
                                            Limitsum = 0;
                                            break;
                                        }
                                    }
                                }

                                else if (dsazt.ID == undrugCombo.ID)
                                {
                                    // 同一子项已经由前面的组套保留，继续扣减本次已占用量。
                                    feeqty += Convert.ToDecimal(dsazt.Qty);
                                }
                            }
                            Limitsum = Limitsum - feeqty;

                            // ========== 第四阶段A-3：把子项结果累计回组套主项 ==========
                            // 注意这里不是把子项加入收费列表，而是只把可收费金额累加到 sumPricecot。
                            // 最终收费界面仍显示一个主项；只有最终提交 GetFeeItemList 时才会拆成真实收费明细。
                            if (Limitsum == 0)
                            {
                                // 本次已占用刚好占满额度，当前子项不再贡献金额。
                                sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty) - Convert.ToDecimal(Price * undrugCombo.Qty);
                            }
                            else if ((Limitsum - undrugCombo.Qty) <= 0)
                            {
                                // 子项部分可收费：只累计剩余额度对应金额，并把子项数量截断后登记。
                                sumPricecot += Convert.ToDecimal(Price * Limitsum);
                                undrugCombo.Qty = Limitsum;
                                hsZTNOREOnlyOneItem.Add(undrugCombo);
                            }
                            else
                            {
                                // 子项全部可收费：累计完整金额，并把组号放入 Memo 供后续同组互斥判断。
                                sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty);
                                undrugCombo.Memo = GroupNumber;
                                hsZTNOREOnlyOneItem.Add(undrugCombo);
                            }
                        }
                    }
                    else
                    {
                        // 子项本身没有维护 Restrictingfee，按正常价格纳入组套主项汇总。
                        sumPricecot += Convert.ToDecimal(Price * undrugCombo.Qty);
                    }
                    #endregion

                    feeqty = 0;

                }
                //f.Item.Price = sumPricecot;
                // 组套最终只回写主项金额，不把拆出来的子项直接返回收费列表。
                // 调用方后续会用 hsREOnlyOneItem 删除旧主项，再从 hsREOnlylistItem 添加重算后的主项。
                f.FT.TotCost = sumPricecot;
                f.FT.OwnCost = sumPricecot;
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);

            }

            #endregion

            #region 普通项目

            else
            {
                // ========== 第四阶段B：普通非组套项目分支 ==========
                // 这里和 ConvertRestrictingfee 的普通明细逻辑基本一致，只是多看了
                // hsZTNOREOnlyOneItem，以便把前面组套拆出来并保留的子项也计入本次已占用量。
                feetype = this.db.getRestrictingfee(CARD_NO, f.Item.ID, ref feecode);
                if (hsZTItem.ContainsKey(f.Item.ID))
                {
                    feetype = this.db.getRestrictingfeeZT(CARD_NO, f.Item.ID, GroupNumber, ref feecode);
                }
                //计算本项目本次剩余的收费次数
                decimal Limitsum = LimitNumber - feetype;//
                if (Limitsum <= 0)
                {
                    // 历史已占满，普通项目整行归零。
                    //f.Item.Price = Convert.ToDecimal(Price * f.Item.Qty) - Convert.ToDecimal(Price * f.Item.Qty);
                    f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    hsREOnlyOneItem.Add(f.Item.ID + number, f);
                    hsREOnlylistItem.Add(f);
                }
                else
                {

                    #region 获取本次收费已经计算的数量

                    // ========== 第四阶段B-1：扣减本次已保留的普通项目 ==========
                    // 普通项目和 CP/TX/ZT 项目的扣减方式不同：
                    // - 普通 Restrictingfee：累计同项目数量；
                    // - CP/TX/ZT：只要同组已有项目保留，就把当前项目归零。
                    foreach (FeeItemList dsa in hsNOREOnlyOneItem)
                    {

                        // 先扣本次已保留的普通项目。
                        // 注意：这里保留旧代码原行为，不调整任何数量累计表达式。
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
                    // ========== 第四阶段B-2：扣减本次已保留的组套拆分子项 ==========
                    // 这个分支是 ConvertRestrictingfeeCharge 相比 ConvertRestrictingfee 多出来的关键点。
                    // 显示价阶段可能先处理组套，再处理普通项目；普通项目必须能看到前面组套子项已经占用的额度。
                    foreach (UndrugComb dsazt in hsZTNOREOnlyOneItem)
                    {
                        // 再扣本次已保留的组套拆分子项。
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
                    // ========== 第四阶段B-3：按剩余额度改写普通项目显示价 ==========
                    // 与门诊最终收费不同，本方法主要服务界面显示，所以即使是普通项目，
                    // 结果也会通过 hsREOnlylistItem 走“删除旧行 + 追加重算行”的替换模型。
                    if (Limitsum <= 0)
                    {
                        // 历史 + 本次已保留量已经占满，本行归零。
                        f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                        f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                        hsREOnlyOneItem.Add(f.Item.ID + number, f);
                        hsREOnlylistItem.Add(f);
                    }
                    else if ((Limitsum - f.Item.Qty) <= 0)
                    {
                        // 部分可收费：数量改成剩余额度，金额按剩余额度重算。
                        f.FT.TotCost = Convert.ToDecimal(f.Item.Price * Limitsum);
                        f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * Limitsum);
                        f.Item.Qty = Limitsum;
                        hsNOREOnlyOneItem.Add(f);
                        hsREOnlyOneItem.Add(f.Item.ID + number, f);
                        hsREOnlylistItem.Add(f);
                    }
                    else
                    {
                        // 全部可收费：保留或补齐原金额，并记录同组组号。
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

        /// <summary>
        /// 住院明细项目的限制收费计算。
        /// </summary>
        /// <param name="CARD_NO">
        /// 住院侧调用方传入的患者标识。历史 SQL 的住院分支按 INPATIENT_NO 查询，
        /// 因此这里实质上要求调用方传入能匹配住院明细的流水号。
        /// </param>
        /// <param name="f">当前住院费用明细。方法会直接修改数量、金额和 Memo。</param>
        /// <param name="hsREOnlyOneItem">已被重算逻辑接管、后续需要替换的项目索引表。</param>
        /// <param name="hsNOREOnlyOneItem">本次住院收费动作内已经保留收费资格的明细集合。</param>
        /// <param name="hsREOnlylistItem">重算后需要回写给调用方的最终费用明细集合。</param>
        /// <param name="number">当前遍历序号，只用于生成“项目编码 + 序号”的临时键。</param>
        /// <param name="LimitNumber">限制收费上限，来自 Restrictingfee.INPUT_CODE。</param>
        /// <remarks>
        /// 该方法是 <see cref="ConvertRestrictingfee"/> 的住院版，核心口径一致：
        /// 先扣历史 2 小时已收费量，再扣本次已保留量，最后决定整行归零、部分截断或完整保留。
        /// 与门诊不同的是，历史 SQL 的住院分支还会排除已经被反交易冲销的记录。
        /// </remarks>
        public void ConvertRestrictingfeeZY(string CARD_NO, Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f, ref Hashtable hsREOnlyOneItem, ref ArrayList hsNOREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number, decimal LimitNumber)
        {
            // ========== 第一阶段：准备住院限制收费上下文 ==========
            // 住院版没有门诊显示价里的组套拆分集合 hsZTNOREOnlyOneItem，
            // 因此只在住院明细集合 hsNOREOnlyOneItem 内扣减本次已保留量。
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

            // ========== 第二阶段：加载同组互斥配置，并确定当前住院明细所在的组号 ==========
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

            // ========== 第三阶段：查询历史 2 小时住院/门诊占用 ==========
            // Astrictpackagefee 例外清单命中时，不查历史收费量，直接从 0 开始算本次额度。
            // 否则普通项目查同项目历史，ZT 项目查同组历史。
            // 住院历史 SQL 还会排除已反交易冲销的记录，避免退费后仍占用额度。
            if (hsTXxzItem.ContainsKey(f.UndrugComb.ID))
            {
                feetype = 0;
            }
            else
            {
                // 普通项目查同项目历史；同组互斥项目改查同组历史。
                feetype = this.db.getRestrictingfee(CARD_NO, f.Item.ID, ref feecode);
                if (hsZTItem.ContainsKey(f.UndrugComb.ID))
                {
                    feetype = this.db.getRestrictingfeeZT(CARD_NO, f.Item.ID, GroupNumber, ref feecode);
                }
            }

            // ========== 第四阶段：先扣历史占用量 ==========
            // Limitsum 是本次收费动作开始前还剩的额度。
            Decimal Limitsum = LimitNumber - feetype;//计算是否次项目还可以收费有没有超越收费次数
            if (Limitsum <= 0)
            {
                // 历史已占满，本行整行归零，并用 Memo=P{原数量} 标记全部超限。
                //f.Item.Price = Convert.ToDecimal(Price * f.Item.Qty) - Convert.ToDecimal(Price * f.Item.Qty);
                f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);
            }
            else
            {
                // ========== 第五阶段：扣本次收费动作内已经保留的项目 ==========
                // 住院侧没有单独处理组套拆分集合，因此只看 hsNOREOnlyOneItem。
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

                    // 床旁、胎心、止血同组互斥命中时，直接把剩余额度置 0；
                    // 普通项目才按同项目数量累计。
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
                // ========== 第六阶段：根据剩余额度改写住院明细 ==========
                // 住院与门诊同样使用 Memo=P/N 保存原数量：
                // P 表示全部超限归零，N 表示部分超限被截断。
                // 这些标记后续可能用于界面展示、重算或人工排查，不能随意删除。
                if (Limitsum <= 0)
                {
                    // 历史 + 本次已保留量已经占满，本行归零。
                    f.FT.TotCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.FT.OwnCost = Convert.ToDecimal(f.Item.Price * f.Item.Qty) - Convert.ToDecimal(f.Item.Price * f.Item.Qty);
                    f.Memo = "P" + Convert.ToDecimal(f.Item.Qty);
                    hsREOnlyOneItem.Add(f.Item.ID + number, f);
                    hsREOnlylistItem.Add(f);
                }
                else if ((Limitsum - f.Item.Qty) <= 0)
                {
                    // 部分可收费：数量截断为剩余额度，金额按截断数量计算。
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
                    // 全部可收费：保留原金额，必要时按单价×数量补齐金额。
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
            // ========== 第一阶段：按历史比例折价公式计算理论金额 ==========
            // 折价规则：第一件按原价，第二件起按折扣率计算。
            // 公式：总费用 = 原价 + 原价 × 折扣率 × (数量 - 1)。
            // 这个公式和 IncrementPercentExecutor 的 Rate 参数一致：
            // 当数量为 1 时只收一件原价；当数量大于 1 时，只有超出第一件的部分按 DISCOUNT_RATE 计算。
            // 注意：DISCOUNT_RATE 是 0-1 的比例，不是 80/50 这种百分数。
            decimal cost = Convert.ToDecimal((f.Item.Price * DISCOUNT_RATE) * (f.Item.Qty - 1)) + f.Item.Price;

            // ========== 第二阶段：应用 TOPPRICE 总金额封顶 ==========
            // TOPPRICE 来自 FIN_DISCOUNT_FEE.TOPPRICE，是最高限价，不是最低限价。
            // TOPPRICE <= 0 表示不封顶。旧表没有最低限价字段，不能在迁移时凭空生成金额下限。
            if (TOPPRICE > 0 && cost > TOPPRICE)
                cost = TOPPRICE;

            // ========== 第三阶段：把折价金额写回 FeeItemList ==========
            // 旧 HIS 当前规则下 TotCost 和 OwnCost 保持一致。
            // 这里没有单独写 RebateCost，避免后续 HIS 普通优惠逻辑再次折扣。
            f.FT.TotCost = cost;
            f.FT.OwnCost = cost;

            // ========== 第四阶段：登记替换结果 ==========
            // 以“费用项目编码 + 序号”为键写入去重哈希表，避免同一项目重复计费。
            // 调用方会根据该键删除旧行，再把 hsREOnlylistItem 中的重算行追加回去。
            hsREOnlyOneItem.Add(f.Item.ID + number, f);

            // 同步写入有序列表，供后续汇总或批量处理使用。
            hsREOnlylistItem.Add(f);
        }

        /// <summary>
        /// 住院费用明细的比例折价计算。
        /// </summary>
        /// <param name="f">待折价的住院费用明细。方法会直接修改 TotCost 和 OwnCost。</param>
        /// <param name="DISCOUNT_RATE">折扣率，例如 0.8 表示第二件起八折。</param>
        /// <param name="TOPPRICE">总金额封顶值；大于 0 时生效。</param>
        /// <param name="hsREOnlyOneItem">已被重算接管的项目索引表。</param>
        /// <param name="hsREOnlylistItem">重算后需要回写的项目集合。</param>
        /// <param name="number">当前遍历序号，只用于生成临时键。</param>
        /// <remarks>
        /// 住院折价公式与门诊 <see cref="ConvertDiscountfee"/> 一致：
        /// 第一件按原价，第二件起按 DISCOUNT_RATE 计算，然后再应用 TOPPRICE 封顶。
        /// 旧实现把 TOPPRICE 大于 0 和等于 0 拆成两个分支，本质只是“是否执行封顶判断”不同。
        /// </remarks>
        public void ConvertDiscountfeeZY(Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f, decimal DISCOUNT_RATE, int TOPPRICE, ref Hashtable hsREOnlyOneItem, ref ArrayList hsREOnlylistItem, decimal number)
        {
            // ========== 第一阶段：按 TOPPRICE 是否配置选择分支 ==========
            // 旧代码把“有封顶”和“无封顶”写成两个分支，虽然公式重复，但保留原结构可以降低改坏风险。
            // 两个分支的共同点：先按第一件原价、后续折价计算，再登记替换集合。
            if (TOPPRICE > 0)
            {
                // ========== 第二阶段A：有 TOPPRICE 时，先算公式再封顶 ==========
                // 先按“第一件原价、其余件打折”计算，再执行总金额封顶。
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
                // ========== 第二阶段B：未配置 TOPPRICE 时，仅执行比例折价公式 ==========
                // 未配置封顶时，仅执行比例折价公式。
                f.FT.TotCost = Convert.ToDecimal((f.Item.Price * DISCOUNT_RATE) * (f.Item.Qty - 1)) + f.Item.Price;
                f.FT.OwnCost = Convert.ToDecimal((f.Item.Price * DISCOUNT_RATE) * (f.Item.Qty - 1)) + f.Item.Price;
                hsREOnlyOneItem.Add(f.Item.ID + number, f);
                hsREOnlylistItem.Add(f);

            }
        }


        /// <summary>
        /// 将 CT 类特殊组套拆成可收费明细，并处理 PACS 三维/四维重建“只收一次”的旧规则。
        /// </summary>
        /// <param name="f">
        /// 组套主项。方法会读取主项数量、患者、合同单位、医嘱号、优惠信息，并把这些上下文复制到拆出的明细。
        /// </param>
        /// <param name="isFirst">
        /// 是否为本次收费动作中遇到的第一个 CT 组套。
        /// 当前方法体里没有直接使用该参数，但它和 DR 版本保持同一签名，属于旧接口兼容参数。
        /// </param>
        /// <param name="hsOnlyOneItem">
        /// CT/PACS 特殊项目去重表。
        /// 旧逻辑用它记录三维重建、四维重建和其他只收一次项目，后续调用方再根据该表删除重复收费明细。
        /// </param>
        /// <returns>
        /// 拆分后的收费明细集合；组套明细、项目主数据、医嘱号或价格处理失败时返回 <c>null</c>，
        /// 并把失败原因写入 <see cref="errText"/>。
        /// </returns>
        /// <remarks>
        /// 这个方法和普通 <see cref="ConvertGroupToDetail"/> 的差异在于：
        ///
        /// 1. 明细来源是 <c>QueryUndrugZTBypackageCode</c>，即 CT/MR/DR 特殊组套维护；
        /// 2. <c>SortID == 3</c> 的 PACS 项目会进入“只收一次”登记；
        /// 3. 三维重建与四维重建之间存在替代关系：如果后来出现四维，会把前面登记的三维改成可删除状态；
        /// 4. 方法只负责拆分和标记，不直接执行 Restrictingfee 数量限制；数量限制在调用方后续阶段完成。
        /// </remarks>
        private ArrayList ConvertCTGroupToDetail(FeeItemList f, bool isFirst, ref Hashtable hsOnlyOneItem)
        {
            // ========== 第一阶段：读取 CT 特殊组套明细 ==========
            // 这里不用普通组套表，而是读取 CT/MR/DR 专用组套维护。
            // 如果拿不到明细，调用方不能继续计费，否则会出现界面主项有金额但落账明细缺失的问题。
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
            // ========== 第二阶段：保证拆出的明细沿用同一个医嘱号 ==========
            // 组套拆分后会生成多条收费明细，但它们在业务上属于同一次医嘱/申请。
            // 如果主项没有医嘱流水号，这里先补一个，避免拆分后每条明细各自生成不同医嘱号。
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

            // ========== 第三阶段：预扫描 PACS 只收一次项目 ==========
            // SortID == 3 表示“只收取一次”的 PACS 子项。
            // 三维/四维重建不是简单同编码去重：四维出现后优先级更高，前面已登记的三维要改为 true，
            // 调用方后续看到 true 会删除重复项，避免三维和四维重复收费。
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
            // ========== 第四阶段：逐个子项构造 FeeItemList 明细 ==========
            // 每个子项都要按当前患者合同单位、年龄价格、特价、购入价和组套加成比例重新取价。
            // 不能直接使用组套主项价格平摊，否则会丢失儿童价、特价、合同单位价和项目级比例。
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
                // PactRate 这里用于处理合同单位优惠比例。
                // 当前抽取版本的 PactRate 固定返回 0 折扣，但保留调用顺序可以保证和旧 HIS 主线一致。
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
            // ========== 第五阶段：把主项上的人工减免、特殊自费金额分摊到子项 ==========
            // 旧 HIS 不把这些金额留在组套主项上，而是尝试下沉到拆出的明细。
            // 减免按比例分摊，尾差补到第一条明细；特殊自费/User03 则挂到价格最高的明细上。
            // 这些字段会影响后续结算、自费、公费和医保口径，所以只加注释，不改变算法。
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

        /// <summary>
        /// 将普通非药品组套拆成最终收费明细。
        /// </summary>
        /// <param name="f">
        /// 组套主项。主项上的患者、医嘱、申请单、合同单位、优惠、自费标记会复制或分摊到拆出的明细。
        /// </param>
        /// <returns>
        /// 普通组套拆分后的 <see cref="FeeItemList"/> 集合；读取组套明细、医嘱号或价格失败时返回 <c>null</c>。
        /// </returns>
        /// <remarks>
        /// 该方法服务最终收费明细生成，是 Restrictingfee 数量限制之前的“明细化”步骤。
        /// 它不处理 CT/DR 的首项/二项/只收一次规则，也不处理 2 小时限制收费；
        /// 它只把一个组套主项转换为一批可以被后续限制收费逻辑逐条处理的真实明细。
        /// </remarks>
        private ArrayList ConvertGroupToDetail(FeeItemList f)
        {
            // ========== 第一阶段：读取普通组套明细 ==========
            // 普通组套和 CT/DR 特殊组套走不同维护表。
            // 这里产生的是最终收费阶段真正要提交的明细基础集合。
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
            // ========== 第二阶段：保证拆出的明细共享主项医嘱号 ==========
            // 组套主项如果没有医嘱号，需要先生成一个，再复制给所有明细。
            // 这样后续撤销、退费、执行确认才能按同一组业务来源追踪。
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
            // ========== 第三阶段：逐个子项按患者合同单位重算价格并构造收费明细 ==========
            // 子项价格不是简单继承组套主项价格。
            // 旧 HIS 需要叠加项目基础价、儿童价、特价、购入价、合同单位、组套子项加成比例。
            // 这也是迁移规则中心时必须单独建模“组套拆分 + 子项取价”的原因。
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
                // 合同单位优惠比例在拆分时直接影响子项单价。
                // 这里先算价格、再乘以 1 - RebateRate，和旧收费明细金额保持一致。
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
            // ========== 第四阶段：处理主项减免和特殊自费字段 ==========
            // 主项如果带 RebateCost，旧逻辑按子项 OwnCost 比例下沉到明细，并把尾差补到第一条。
            // SpecialPrice 和 FT.User03 不做比例分摊，而是挂到价格最高的子项上。
            // 这不是通用财务设计，但属于旧 HIS 已落地口径，迁移时需要按兼容规则复刻。
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
        /// 只读取组套原始明细定义，不构造收费明细、不重新取价。
        /// </summary>
        /// <param name="f">组套主项。</param>
        /// <returns>组套维护表中的 <see cref="UndrugComb"/> 原始明细集合。</returns>
        /// <remarks>
        /// 这个轻量方法主要被 <see cref="ConvertRestrictingfeeCharge"/> 使用。
        /// 显示价限制收费只需要知道组套拆出了哪些子项、每个子项数量是多少，
        /// 然后把可收费金额汇总回主项；它不需要构造完整 FeeItemList。
        /// </remarks>
        private ArrayList ConvertGroupToDetail1(FeeItemList f)
        {
            ArrayList undrugCombList = this.db.QueryUndrugPackagesBypackageCode(f.Item.ID);
            return undrugCombList;
        }


        /// <summary>
        /// 获取合同单位对当前项目的优惠比例。
        /// </summary>
        /// <param name="r">当前患者挂号信息，包含合同单位和支付类别。</param>
        /// <param name="f">当前正在拆分或计算价格的费用明细。</param>
        /// <param name="errMsg">失败时返回的错误文本。当前抽取版本不会写入错误。</param>
        /// <returns>
        /// 合同单位项目优惠比例对象。
        /// 当前代码固定返回 <c>RebateRate = 0</c>，表示不额外折扣。
        /// </returns>
        /// <remarks>
        /// 这里看起来像无效封装，但不要轻易删除。
        /// 旧 HIS 主线通常在这个位置接合同单位项目比例，当前项目副本把逻辑简化为 0 折扣。
        /// 保留方法有两个价值：
        /// 1. 保持组套拆分时的价格计算顺序不变；
        /// 2. 迁移新规则中心时可以明确识别“合同单位优惠”和“FIN_DISCOUNT_FEE 折价”不是同一类规则。
        /// </remarks>
        private Neusoft.HISFC.Models.Base.PactItemRate PactRate(Neusoft.HISFC.Models.Registration.Register r, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f, ref string errMsg)
        {
            Neusoft.HISFC.Models.Base.PactItemRate pRate = new Neusoft.HISFC.Models.Base.PactItemRate();
            pRate.Rate.RebateRate = 0;
            return pRate;
        }

        #region 获取CT/MR收费规则的HashTable
        /// <summary>
        /// 读取 CT/MR/DR 特殊组套规则，并转换成以组套编码为键的哈希表。
        /// </summary>
        /// <returns>
        /// 键为组套主项编码，值为该组套下的特殊收费规则集合。
        /// 每个规则元素用 <see cref="NeuObject"/> 承载项目编码、收费数量、收费方式、取整方式和 DR/CT 类型。
        /// </returns>
        /// <remarks>
        /// 规则来源是 COM_DICTIONARY 的 <c>ItemZT</c> 常量。
        /// 字段含义在旧系统里比较隐晦，这里按实际转换口径说明：
        ///
        /// 1. <c>Name</c>：组套主项编码；
        /// 2. <c>Memo</c>：子项编码列表，使用 <c>|</c> 分隔；
        /// 3. <c>WBCode</c>：数量；
        /// 4. <c>SortID</c>：收费方式，0 每个项目收、1 第一个项目收、2 第二个项目起加收、3 只收一次；
        /// 5. <c>SpellCode</c>：取整方式，0 总量取整、1 单个取整、2 固定数量；
        /// 6. <c>UserCode</c>：组套类型，0 DR、1 CT。
        ///
        /// 该方法只做字典转换，不执行价格计算。真正拆分计费在 ConvertDRGroupToDetail / ConvertCTGroupToDetail。
        /// </remarks>
        private Hashtable GetCTMRHashtabel()
        {
            // ========== 第一阶段：读取 ItemZT 字典 ==========
            // ItemZT 是 CT/MR/DR 特殊组套的配置入口。
            // 旧 HIS 没有强类型规则表，只能把多段含义塞进常量表字段。
            ArrayList alItemZT = this.db.GetAllList("ItemZT");
            Hashtable hsItemZT = new Hashtable();
            if (alItemZT != null)
            {
                hsItemZT = new Hashtable();
                // ========== 第二阶段：把一条常量拆成多个 NeuObject 规则 ==========
                // 同一个组套主项可能在多条常量里维护，也可能一条常量 Memo 放多个子项。
                // 因此这里按 conObj.Name 聚合成 ArrayList，供 ucDisplay.GetFeeItemList 判断 DR/CT 类型并选择拆分方法。
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
        /// 为费用明细补齐开方医生所在科室。
        /// </summary>
        /// <param name="feeItemList">需要补齐医生科室的费用明细集合。</param>
        /// <returns>
        /// 全部明细补齐成功返回 <c>true</c>；找不到医生或医生科室时返回 <c>false</c>，
        /// 并把错误原因写入 <see cref="errText"/>。
        /// </returns>
        /// <remarks>
        /// 医生科室不是折价公式的一部分，但会影响收费落账、执行统计和后续追溯。
        /// 旧界面有些入口只带开方医生工号，不带医生科室，因此在进入组套拆分或特殊规则前需要补齐。
        /// </remarks>
        private bool AssignmentDoctDeptInfo(List<FeeItemList> feeItemList)
        {
            // ========== 第一阶段：逐条检查医生科室是否缺失 ==========
            // 只在 RecipeOper.ID 存在且 DoctDeptInfo.ID 为空时查库补齐。
            // 已经带科室的明细不覆盖，避免把调用方已经修正过的科室改回医生默认科室。
            foreach (var f in feeItemList)
            {
                if (!string.IsNullOrEmpty(f.RecipeOper.ID) && string.IsNullOrEmpty(f.DoctDeptInfo.ID))
                {
                    // ========== 第二阶段：按工号读取医生默认科室 ==========
                    // 找不到医生或医生无科室时直接失败。
                    // 这里不做“继续收费但科室为空”的容错，因为后续收费明细缺科室会更难排查。
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
