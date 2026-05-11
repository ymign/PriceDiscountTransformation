using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.Pharmacy;
using Neusoft.HISFC.Models.MedicalTraceCode;
using Neusoft.HISFC.Components.Common.Forms;
using System.Windows.Forms;
using Neusoft.HISFC.BizLogic.Pharmacy;
using Neusoft.HISFC.Components.Common.Classes;

namespace Neusoft.HISFC.Components.Common.Services.YBTraceCode
{
    /// <summary>
    /// 追溯码服务
    /// </summary>
    public class TraceCodeBusinessService
    {

        #region 属性

        private TraceCodeDAL traceCodeQueryService = new TraceCodeDAL();

        /// <summary>
        /// 无码目录集合
        /// </summary>
        private static List<Const> NoTraceCodeDrugList = null;

        /// <summary>
        /// 开关业务类
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam ControlParam = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

        public string Err { get; private set; }

        #endregion

        public Dictionary<string, HashSet<string>> GetMap()
        {
            var map = traceCodeQueryService.GetDrugCodeToIdentifierCodesMap();
            return map ?? new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 按药品集合实时查询标识码对照
        /// </summary>
        private Dictionary<string, HashSet<string>> GetMapByDrugCodes(List<string> drugCodes, out string errMsg)
        {
            errMsg = string.Empty;
            var map = traceCodeQueryService.GetDrugCodeToIdentifierCodesMap(drugCodes);
            if (map == null)
            {
                errMsg = string.IsNullOrEmpty(traceCodeQueryService.Err)
                    ? "查询药品标识码对照失败!"
                    : traceCodeQueryService.Err;
                return null;
            }
            return map;
        }

        /// <summary>
        /// 获取无码目录集合
        /// </summary>
        /// <returns></returns>
        private List<Const> GetNoTraceCodeDrugList()
        {
            if (NoTraceCodeDrugList == null)
            {
                NoTraceCodeDrugList = new List<Const>();
            }

            if (NoTraceCodeDrugList.Any())
            {
                return NoTraceCodeDrugList;
            }

            var conMgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();

            NoTraceCodeDrugList = conMgr.GetConstantList("DrugTracCodgFreeSacn").Cast<Const>().ToList();
            return NoTraceCodeDrugList;

        }

        private static void CopyTraceCodeCollectInfo(ApplyOut target, PhaComApplyout source)
        {
            target.NeedCollectQty = (int)source.Needcollectqty;
            target.AlreadyCollectQty = (int)source.Alreadycollectqty;
            target.AppealCollectQty = (int)source.Appealcollectqty;
            target.NeedCollectSpiltQty = (int)source.NeedCollectSpiltQty;
            target.AlreadyCollectSpiltQty = (int)source.AlreadyCollectSpiltQty;
            target.AppealCollectSpiltQty = (int)source.AppealCollectSpiltQty;

            target.NeedCollectTraceCodeFlag = source.NeedCollectTraceCodeFlag;
            target.NotCollectTraceCodeReason = source.NotCollectTraceCodeReason;
            target.TraceCodeCollectionStatus = source.Tracecodecollectionstatus;
        }

        /// <summary>
        /// 设置不采集原因，是否需要标记为“不需要采集”
        /// </summary>
        private static void SetNotCollectInfo(ApplyOut info, string reason, bool markNotRequired)
        {
            info.NotCollectTraceCodeReason = reason;
            info.NeedCollectTraceCodeFlag = "0";
            if (markNotRequired)
            {
                info.TraceCodeCollectionStatus = TraceCodeCollectionStatusEnum.NotRequired;
            }
        }

        /// <summary>
        /// 设置需采集数量与状态，并按需回写数据库
        /// </summary>
        private void SetCollectRequiredInfo(
            ApplyOut info,
            int pactCount,
            int splitCount,
            bool isDirectUpdateTraceCodeInfo)
        {
            info.NeedCollectQty = pactCount;
            info.NeedCollectSpiltQty = splitCount;
            info.NeedCollectTraceCodeFlag = "1";
            info.TraceCodeCollectionStatus = TraceCodeCollectionStatusEnum.Pending;

            if (isDirectUpdateTraceCodeInfo)
            {
                if (!this.traceCodeQueryService.UpdateApplyOutTheTraceCodeInfo(
                    info.ID,
                    info.NeedCollectQty,
                    info.NeedCollectSpiltQty,
                    info.NeedCollectTraceCodeFlag,
                    info.NotCollectTraceCodeReason,
                    info.TraceCodeCollectionStatus))
                {
                    throw new Exception(this.traceCodeQueryService.Err);
                }
            }
        }

        /// <summary>
        /// 根据药品信息判断是否无需采集，并给出原因
        /// </summary>
        private bool TryGetNotCollectReasonByDrug(PhaComApplyout applyOutInfo, out string reason)
        {
            reason = string.Empty;
            var item = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(applyOutInfo.DrugCode);

            // 特定剂型且非指定药理，按规则不采集
            if (item.DosageForm.ID == "01" && item.PhyFunction3.ID != "11603")
            {
                reason = "[" + applyOutInfo.TradeName + "]剂型为01,但是三级药理非[11603]胰岛激素及其他影响血糖的药物";
                return true;
            }

            // 中草药不采集
            if (item.Type.ID == "C")
            {
                reason = "[" + applyOutInfo.TradeName + "]中草药无需采集追溯码!";
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断项目是否在无码目录里面
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool IsInNoTraceCodeDrugList(string code)
        {
            var noTraceCodeDrugList = GetNoTraceCodeDrugList();

            return noTraceCodeDrugList.Any(x => x.ID == code);
        }

        public void SetTraceCodeInfoToApplyOutList(List<ApplyOut> list, bool isDirectUpdateTraceCodeInfo)
        {
            foreach (var item in list)
            {
                SetTraceCodeInfoToApplyOut(item, isDirectUpdateTraceCodeInfo);
            }
        }

        public void SetTraceCodeInfoToApplyOut(ApplyOut info, bool isDirectUpdateTraceCodeInfo)
        {

            if (string.IsNullOrEmpty(info.ID))
            {
                throw new Exception("[SetTraceCodeInfoToApplyOut]发药申请流水号不能为空!");

            }

            var applyOutInfo = this.traceCodeQueryService.GetApplyInfo(info.ID);

            if (applyOutInfo == null)
            {
                throw new Exception("[" + info.ID + "]未找到对应发药申请记录!");
            }
            info.TraceCodeCollectionStatus = applyOutInfo.Tracecodecollectionstatus;
            // 已存在采集标记时直接回填，避免重复计算
            if (!string.IsNullOrEmpty(applyOutInfo.NeedCollectTraceCodeFlag))
            {
                CopyTraceCodeCollectInfo(info, applyOutInfo);
                return;
            }

            // 1) 先按无码目录排除
            if (IsInNoTraceCodeDrugList(applyOutInfo.DrugCode))
            {
                SetNotCollectInfo(info, "[" + applyOutInfo.TradeName + "]在无码目录字典中!", true);
                return;
            }

            // 2) 采集状态不可采集（维持原状态）
            if (TraceCodeCollectionStatusEnum.IsCanNotCollect(applyOutInfo.Tracecodecollectionstatus))
            {
                SetNotCollectInfo(
                    info,
                    "[Tracecodecollectionstatus]状态为" + TraceCodeCollectionStatusEnum.GetDescription(applyOutInfo.Tracecodecollectionstatus) + ",暂无需扫码!",
                    false);
                return;
            }

            // 3) 按药品属性排除
            string notCollectReason;
            if (TryGetNotCollectReasonByDrug(applyOutInfo, out notCollectReason))
            {
                SetNotCollectInfo(info, notCollectReason, true);
                return;
            }

            // 4) 计算需采集数量并设置状态
            int packQty = (int)applyOutInfo.PackQty;
            int applyQty = (int)applyOutInfo.ApplyNum;
            var pactCount = GetPactCount(applyQty, packQty);
            var splitCount = GetSplitCount(applyQty, packQty);

            SetCollectRequiredInfo(info, pactCount, splitCount, isDirectUpdateTraceCodeInfo);

            return;
        }

        /// <summary>
        /// 判断发药申请是否需要采集追溯码
        /// </summary>
        /// <param name="applyNumber">发药申请流水号</param>
        /// <returns></returns>
        private bool ShouldCollectTraceCode(string applyNumber, string drugCode)
        {

            if (IsInNoTraceCodeDrugList(drugCode))
            {
                return false;
            }

            // 判断发药申请的采集状态是否可以采集
            var applyInfo = traceCodeQueryService.GetApplyInfo(applyNumber);
            var canNotCollectResult = TraceCodeCollectionStatusEnum.IsCanNotCollect(applyInfo.Tracecodecollectionstatus);
            if (canNotCollectResult)
            {
                return false;
            }

            // 拆零采集开关
            var collectSplitSwitch = ControlParam.GetControlParam<bool>("TraceCodeSplit", false, false);
            if (!collectSplitSwitch)
            {
                int packQty = (int)applyInfo.PackQty;
                int applyQty = (int)applyInfo.ApplyNum;
                var pactCount = GetPactCount(applyQty, packQty);
                var splitCount = GetSplitCount(applyQty, packQty);
                if (pactCount <= 0 && splitCount > 0)
                {
                    return false;
                }
            }


            // 与 SetTraceCodeInfoToApplyOut 保持一致的药品排除规则
            string notCollectReason;
            if (TryGetNotCollectReasonByDrug(applyInfo, out notCollectReason))
            {
                return false;
            }

            return true;

        }

        /// <summary>
        /// 判断发药申请是否需要采集追溯码
        /// </summary>
        /// <param name="applyOutList"></param>
        /// <returns></returns>
        public bool IsTraceCodeCollectionRequired(List<ApplyOut> applyOutList)
        {
            return applyOutList.Any(a => ShouldCollectTraceCode(a.ID, a.Item.ID));
        }


        /// <summary>
        /// 获取应该采集追溯码的发药申请集合
        /// </summary>
        /// <param name="applyOutList"></param>
        /// <returns></returns>
        private List<ApplyOut> GetShouldCollectTraceCodeList(List<ApplyOut> applyOutList)
        {
            return applyOutList
        .Where(applyOut => ShouldCollectTraceCode(applyOut.ID, applyOut.Item.ID))
        .ToList();
        }

        /// <summary>
        /// 校验采集相关枚举参数是否合法
        /// </summary>
        private static void ValidateCollectParams(ApplyOut applyOut)
        {
            if (!BusinessScenarioEnum.IsValid(applyOut.BusinessScenario))
            {
                throw new Exception("业务场景非法!");
            }

            if (!CollectTypeEnum.IsValid(applyOut.CollectType))
            {
                throw new Exception("采集方式非法!");
            }

            if (!SourceSystemEnum.IsValid(applyOut.SourceSystem))
            {
                throw new Exception("系统来源非法!");
            }

            if (!BusinessTypeEnum.IsValid(applyOut.BusinessType))
            {
                throw new Exception("业务类型非法!");
            }
        }

        /// <summary>
        /// 清空非门诊场景的业务信息字段
        /// </summary>
        private static void ClearOrderInfo(YbTraceCollectMain info)
        {
            info.PharmacyCode = string.Empty;
            info.PharmacyName = string.Empty;
            info.DeptCode = string.Empty;
            info.DeptName = string.Empty;
            info.MoOrderNo = string.Empty;
            info.ExecOrderNo = string.Empty;
        }

        /// <summary>
        /// 门诊场景：补全药房/科室/医嘱等信息
        /// </summary>
        private static void SetOrderInfoForMZ(YbTraceCollectMain info, ApplyOut applyOut)
        {
            info.PharmacyCode = applyOut.StockDept.ID;
            info.PharmacyName = applyOut.StockDept.Name;
            info.DeptCode = applyOut.ApplyDept.ID;
            info.DeptName = applyOut.ApplyDept.Name;
            info.MoOrderNo = applyOut.OrderNO;
            info.ExecOrderNo = applyOut.ExecNO;
        }

        /// <summary>
        /// 填充操作员信息
        /// </summary>
        private void SetOperatorInfo(YbTraceCollectMain info)
        {
            if (traceCodeQueryService.Operator != null)
            {
                info.CreatedCode = this.traceCodeQueryService.Operator.ID;
                info.CreatedName = this.traceCodeQueryService.Operator.Name;
                info.CollectOperCode = info.CreatedCode;
                info.CollectOperName = info.CreatedName;
            }
        }

        /// <summary>
        /// 填充医院信息
        /// </summary>
        private static void SetHospitalInfo(YbTraceCollectMain info)
        {
            if (Neusoft.FrameWork.Management.Connection.Hospital.ID == "CORE_HIS502")
            {
                info.HospitalCode = "H44040200357";
                info.HospitalName = "中山大学珠海校区卫生服务中心";
            }
            else
            {
                info.HospitalCode = "H44040200001";
                info.HospitalName = "中山大学附属第五医院";
            }
        }

        /// <summary>
        /// 统一设置包装采集数量与状态（状态 0:待采集, 2:不需要）
        /// </summary>
        private static void SetPactCollectInfo(
            YbTraceCollectMain info,
            int needQty,
            int actualQty,
            int appealQty,
            int unCollectQty)
        {
            if (needQty > 0)
            {
                info.IsHavePact = "1";
                info.PactNeedCollectQty = needQty;
                info.PactActualCollectQty = actualQty;
                info.PactAppealCollectQty = appealQty;
                info.PactUnCollectQty = unCollectQty;
                info.PactCollectStatus = "0";
            }
            else
            {
                info.IsHavePact = "0";
                info.PactNeedCollectQty = 0;
                info.PactActualCollectQty = 0;
                info.PactAppealCollectQty = 0;
                info.PactUnCollectQty = 0;
                info.PactCollectStatus = "2";
            }
            info.PactCollectMethod = "1";
        }

        /// <summary>
        /// 统一设置拆零采集数量与状态（状态 0:待采集, 2:不需要）
        /// </summary>
        private static void SetSplitCollectInfo(
            YbTraceCollectMain info,
            int needQty,
            int actualQty,
            int appealQty,
            int unCollectQty)
        {
            if (needQty > 0)
            {
                info.IsHaveSplit = "1";
                info.SplitNeedCollectQty = needQty;
                info.SplitActualCollectQty = actualQty;
                info.SplitAppealCollectQty = appealQty;
                info.SplitUnCollectQty = unCollectQty;
                info.SplitCollectStatus = "0";
            }
            else
            {
                info.IsHaveSplit = "0";
                info.SplitNeedCollectQty = 0;
                info.SplitActualCollectQty = 0;
                info.SplitAppealCollectQty = 0;
                info.SplitUnCollectQty = 0;
                info.SplitCollectStatus = "2";
            }
            info.SplitCollectMethod = "1";
        }

        /// <summary>
        /// 构建采集主对象的公共字段（业务标识、药品信息、医院/操作员、排序等）
        /// </summary>
        private YbTraceCollectMain CreateCollectMainBase(ApplyOut applyOut)
        {
            ValidateCollectParams(applyOut);

            var info = new YbTraceCollectMain();
            info.Id = Guid.NewGuid().ToString();
            info.ApplyNumber = applyOut.ID;

            info.BusinessScenario = applyOut.BusinessScenario;
            info.CollectType = applyOut.CollectType;
            info.SourceSystem = applyOut.SourceSystem;
            info.BusinessType = applyOut.BusinessType;

            info.SerialNo = applyOut.PatientNO;
            info.PatientName = applyOut.PatientName;
            info.DrugCode = applyOut.Item.ID;
            info.DrugName = applyOut.Item.Name;
            info.ExtField3 = applyOut.RecipeNO;

            var drugInfo = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(applyOut.Item.ID);
            info.DrugSpecs = drugInfo.Specs;
            info.DrugCustomCode = drugInfo.UserCode;
            info.DrugPactQty = applyOut.Item.PackQty.ToString();
            info.DrugPactUnit = drugInfo.PackUnit;
            info.DrugMinUnit = drugInfo.MinUnit;
            info.DrugSplitUnit = drugInfo.MinUnit;

            // 非门诊场景默认清空业务字段，由具体场景补全
            ClearOrderInfo(info);

            info.CollectIp = Neusoft.FrameWork.WinForms.Classes.Function.GetLocalIP();

            SetOperatorInfo(info);
            SetHospitalInfo(info);

            // 保持界面排序一致
            info.SortIndex = applyOut.SortIndex;

            return info;
        }

        /// <summary>
        /// 将发药申请实体转换为采集记录信息实体对象
        /// </summary>
        /// <param name="applyOut"></param>
        /// <returns></returns>
        private YbTraceCollectMain ConvertApplyOutToYbTraceCollectMain(ApplyOut applyOut)
        {
            var traceCollectMainInfo = CreateCollectMainBase(applyOut);

            int packQty = (int)applyOut.Item.PackQty;
            int totalQty = (int)applyOut.Operation.ApplyQty;

            // 由开方数量换算包装/拆零数量
            int packCount = GetPactCount(totalQty, packQty);
            int splitCount = GetSplitCount(totalQty, packQty);

            SetPactCollectInfo(traceCollectMainInfo, packCount, 0, 0, packCount);

            //拆零采集开关
            var collectSplitSwitch = ControlParam.GetControlParam<bool>("TraceCodeSplit", false, false);
            int splitNeed = (splitCount > 0 && collectSplitSwitch) ? splitCount : 0;
            SetSplitCollectInfo(traceCollectMainInfo, splitNeed, 0, 0, splitNeed);


            return traceCollectMainInfo;
        }

        /// <summary>
        /// 批量将发药申请列表转换为追溯码采集记录列表
        /// </summary>
        /// <param name="applyOutList">发药申请集合</param>
        /// <returns>追溯采集信息集合</returns>
        private List<YbTraceCollectMain> ConvertApplyOutListToTraceCollectList(List<ApplyOut> applyOutList)
        {
            var resultList = new List<YbTraceCollectMain>();

            if (applyOutList == null || applyOutList.Count == 0)
                return resultList;

            foreach (ApplyOut applyOut in applyOutList)
            {
                var traceRecord = ConvertApplyOutToYbTraceCollectMain(applyOut);
                if (traceRecord != null)
                {
                    resultList.Add(traceRecord);
                }
            }

            return resultList;
        }


        /// <summary>
        /// 获取包装数量
        /// </summary>
        /// <param name="applyQty"></param>
        /// <param name="packQty"></param>
        /// <returns></returns>
        private int GetPactCount(int applyQty, int packQty)
        {
            return applyQty / packQty;
        }

        /// <summary>
        /// 获取拆零数量
        /// </summary>
        /// <param name="applyQty"></param>
        /// <param name="packQty"></param>
        /// <returns></returns>
        private int GetSplitCount(int applyQty, int packQty)
        {
            return applyQty % packQty;
        }

        /// <summary>
        /// 设置采集信息实体的药品标识码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <summary>
        /// 获取本次采集涉及的药品编码集合（用于一次性查对照表）
        /// </summary>
        private static List<string> GetDistinctDrugCodes(List<YbTraceCollectMain> collectList)
        {
            if (collectList == null)
            {
                return new List<string>();
            }

            return collectList
                .Where(a => a != null && !string.IsNullOrEmpty(a.DrugCode))
                .Select(a => a.DrugCode.Trim())
                .Where(a => a.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>
        /// 根据对照表回填单个药品的标识码列表
        /// </summary>
        private static bool TryAssignIdentifiyCode(
            YbTraceCollectMain info,
            Dictionary<string, HashSet<string>> dicIdentifiyCode)
        {
            if (info == null || dicIdentifiyCode == null)
            {
                return false;
            }

            var drugCode = string.IsNullOrEmpty(info.DrugCode) ? string.Empty : info.DrugCode.Trim();
            if (drugCode.Length == 0)
            {
                return false;
            }

            HashSet<string> identifiyCodes;
            if (!dicIdentifiyCode.TryGetValue(drugCode, out identifiyCodes) || identifiyCodes == null || identifiyCodes.Count == 0)
            {
                return false;
            }

            // 写回药品标识码列表
            info.IdentifiyCodeList = string.Join(";", identifiyCodes.ToArray());
            return true;
        }

        /// <summary>
        /// 只加载本次药品的对照表并回填标识码
        /// </summary>
        private bool TryAssignIdentifiyCodes(List<YbTraceCollectMain> collectList)
        {
            Err = string.Empty;

            var drugCodes = GetDistinctDrugCodes(collectList);
            if (!drugCodes.Any())
            {
                Err = "待采集药品为空!";
                return false;
            }

            // 实时查询对照表（仅本次药品）
            string loadErr;
            var dicIdentifiyCode = GetMapByDrugCodes(drugCodes, out loadErr);
            if (dicIdentifiyCode == null)
            {
                Err = loadErr;
                return false;
            }

            var missingCodes = new List<string>();
            var assignFailed = false;
            foreach (var item in collectList)
            {
                if (!TryAssignIdentifiyCode(item, dicIdentifiyCode))
                {
                    assignFailed = true;
                    if (item != null && !string.IsNullOrEmpty(item.DrugCode))
                    {
                        missingCodes.Add(item.DrugCode.Trim());
                    }
                }
            }

            if (!assignFailed)
            {
                return true;
            }

            if (missingCodes.Any())
            {
                Err = "未找到药品标识码对照: "
                    + string.Join(",", missingCodes.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
                return false;
            }

            Err = "存在未设置药品编码的采集数据!";
            return false;
        }


        private YbTraceCollectMain ConvertMZApplyOutToYbTraceCollectMain(ApplyOut applyOut)
        {
            var traceCollectMainInfo = CreateCollectMainBase(applyOut);
            SetOrderInfoForMZ(traceCollectMainInfo, applyOut);

            int pactUnCollectQty = applyOut.NeedCollectQty - applyOut.AlreadyCollectQty - applyOut.AppealCollectQty;
            SetPactCollectInfo(
                traceCollectMainInfo,
                applyOut.NeedCollectQty,
                applyOut.AlreadyCollectQty,
                applyOut.AppealCollectQty,
                pactUnCollectQty);

            // 维持原有拆零未采集数量计算规则
            int splitUnCollectQty = applyOut.NeedCollectSpiltQty - applyOut.AppealCollectSpiltQty - applyOut.AppealCollectSpiltQty;
            SetSplitCollectInfo(
                traceCollectMainInfo,
                applyOut.NeedCollectSpiltQty,
                applyOut.AlreadyCollectSpiltQty,
                applyOut.AppealCollectSpiltQty,
                splitUnCollectQty);


            return traceCollectMainInfo;
        }

        public int StartCollectMZTraceCode(
            List<ApplyOut> applyOutList,
            ref List<YbTraceCollectMain> collectCompletedMainList)
        {
            Err = string.Empty;

            if (applyOutList.Any(a => a.NeedCollectTraceCodeFlag != "1"))
            {
                this.Err = "不存在需要采集的数据! ";
                return -1;
            }

            if (applyOutList.Any(a => a.StockDept.ID == "9484"))
            {
                this.Err = "减免药房不需要采集追溯码信息! ";
                return 1;
            }



            var collectList = new List<YbTraceCollectMain>();
            foreach (var item in applyOutList)
            {
                var info = ConvertMZApplyOutToYbTraceCollectMain(item);
                collectList.Add(info);
            }
            //设置药品标识码
            if (!TryAssignIdentifiyCodes(collectList))
            {
                return -1;
            }

            var f = new frmCollectTraceCode();
            f.InitData(collectList);

            //var dialogResult = ModalDialog.ShowDialog(f, this);
            var dialogResult = f.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                return -1;
            }
            collectCompletedMainList = f.YbTraceCollectMainList;
            //补充获取相关信息
            foreach (var mainInfo in collectCompletedMainList)
            {
                if (!this.traceCodeQueryService.GetOtherInfo(mainInfo))
                {
                    this.Err = "数据赋值出错:" + this.traceCodeQueryService.Err;
                    return -1;
                }


            }


            return 1;
        }

        /// <summary>
        /// 开始追溯码采集
        /// </summary>
        /// <param name="applyOutList"></param>
        /// <param name="collectCompletedMainList"></param>
        /// <returns></returns>
        public int StartTraceCodeCollectProcess(
            List<ApplyOut> applyOutList,
            ref List<YbTraceCollectMain> collectCompletedMainList)
        {
            Err = string.Empty;

            var list = GetShouldCollectTraceCodeList(applyOutList);
            if (!list.Any())
            {
                return -1;
            }

            var collectList = ConvertApplyOutListToTraceCollectList(list);
            if (!collectList.Any())
            {
                return -1;
            }
            collectList = collectList.OrderBy(a => a.SortIndex).ToList();

            //设置药品标识码
            if (!TryAssignIdentifiyCodes(collectList))
            {
                return -1;
            }

            var f = new frmCollectTraceCode();
            f.InitData(collectList);
            var dialogResult = f.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                return -1;
            }
            collectCompletedMainList = f.YbTraceCollectMainList;
            return 1;
        }


        /// <summary>
        /// 保存采集到的追溯码相关信息
        /// </summary>
        /// <param name="applyOutInfo"></param>
        /// <returns></returns>
        public int SaveTraceCodeInfo(ApplyOut applyOutInfo, ref string errMsg)
        {
            if (applyOutInfo.StockDept.ID == "9484")
            {
                return 1;
            }

            if (applyOutInfo.NeedCollectTraceCodeFlag == "0")
            {
                return 1;
            }

            var newInfo = this.traceCodeQueryService.GetApplyInfo(applyOutInfo.ID);

            if (TraceCodeCollectionStatusEnum.IsCollectCompleted(newInfo.Tracecodecollectionstatus))
            {
                return 1;
            }

            errMsg = "保存追溯码信息失败:";

            if (string.IsNullOrEmpty(applyOutInfo.NeedCollectTraceCodeFlag))
            {
                errMsg += "采集标识为空!";
                return -1;
            }



            if (applyOutInfo.TraceCollectMain == null)
            {
                errMsg += "采集主实体为null!";
                return -1;
            }

            //if (applyOutInfo.TraceCollectMain.DetailList == null || !applyOutInfo.TraceCollectMain.DetailList.Any())
            //{
            //    errMsg += "采集明细实体为null!";
            //    return -1;
            //}

            if (applyOutInfo.TraceCollectMain.ApplyNumber != applyOutInfo.ID)
            {
                errMsg += "申请流水号不一致!";
                return -1;
            }

            //扣减拆零库存
            var mainInfo = applyOutInfo.TraceCollectMain;
            var drugCode = mainInfo.DrugCode;
            var drugDeptCode = mainInfo.PharmacyCode;
            var EnableSplitDrugFixedUniversalCodeUpload = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam().GetControlParam<bool>("EnableSplitDrugFixedUniversalCodeUpload", false, false);
            if (mainInfo.IsHaveSplit == YesNoEnum.Yes)
            {
                if (EnableSplitDrugFixedUniversalCodeUpload)
                {
                    mainInfo.SplitTracCodgs = "99999999999999999999";
                    mainInfo.SplitActualCollectQty = mainInfo.SplitNeedCollectQty;
                    mainInfo.SplitUnCollectQty = mainInfo.SplitNeedCollectQty - mainInfo.SplitActualCollectQty;
                }
                else
                {
                    #region 拆零逻辑
                    var needQty = mainInfo.SplitNeedCollectQty;
                    var stockInfo = this.traceCodeQueryService.GetYbTraceStockInfo(drugCode, drugDeptCode);

                    if (stockInfo == null || stockInfo.AvailableQty < needQty)
                    {
                        errMsg += "[" + mainInfo.DrugName + "]库存不足!";
                        return -1;
                    }

                    var seedList = this.traceCodeQueryService.GetAvailableSeeds(drugDeptCode, drugCode);
                    if (!seedList.Any())
                    {
                        errMsg += "[" + mainInfo.DrugName + "]种子数据不足!";
                        return -1;
                    }

                    if (seedList.Sum(s => s.AvailableQty) < needQty)
                    {
                        errMsg += "[" + mainInfo.DrugName + "]种子数据库存不足!";
                        return -1;
                    }
                    //TODO后续加上并发考虑
                    var remainingQty = needQty;
                    decimal totalAllocated = 0;//总分配数量

                    foreach (var seed in seedList)
                    {
                        if (remainingQty <= 0) break;

                        var allocateQty = Math.Min(remainingQty, seed.AvailableQty);
                        var startOffset = seed.CurrentOffset + 1;
                        var endOffset = seed.CurrentOffset + allocateQty;

                        var seedNewCurrentOffset = seed.CurrentOffset + allocateQty;
                        var seedNewAvailableQty = seed.AvailableQty - allocateQty;
                        var seedNewStatus = (seed.AvailableQty - allocateQty) <= 0 ? "2" : "1";
                        mainInfo.SplitTracCodgs = string.IsNullOrEmpty(mainInfo.SplitTracCodgs) ? seed.ParentTraceCode : mainInfo.SplitTracCodgs + ";" + seed.ParentTraceCode;
                        mainInfo.SplitActualCollectQty = mainInfo.SplitNeedCollectQty;
                        mainInfo.SplitUnCollectQty = mainInfo.SplitNeedCollectQty - mainInfo.SplitActualCollectQty;

                        remainingQty -= allocateQty;
                        totalAllocated += allocateQty;

                        if (!this.traceCodeQueryService.UpdateTraceSeedWhenUseSucess(
                            seed.Id,
                            seedNewAvailableQty,
                            seedNewCurrentOffset,
                            seedNewStatus,
                            seed.CurrentOffset
                            ))
                        {
                            errMsg += this.traceCodeQueryService.Err;
                            return -1;
                        }

                        var allocationRangeInfo = new YbTraceAllocationRange();
                        allocationRangeInfo.Id = Guid.NewGuid().ToString();
                        allocationRangeInfo.SeedId = seed.Id;
                        allocationRangeInfo.TransType = "0";
                        allocationRangeInfo.TraceCode = seed.ParentTraceCode;
                        allocationRangeInfo.DrugCode = seed.DrugCode;
                        allocationRangeInfo.DrugName = seed.DrugName;
                        allocationRangeInfo.ApplyNumber = applyOutInfo.ID;
                        allocationRangeInfo.SerialNo = applyOutInfo.PatientNO;
                        allocationRangeInfo.CardNo = mainInfo.CardNo;
                        allocationRangeInfo.PatientName = mainInfo.PatientName;
                        allocationRangeInfo.PatientNo = mainInfo.PatientNo;
                        allocationRangeInfo.MoOrderNo = mainInfo.MoOrderNo;
                        allocationRangeInfo.ExecOrderNo = mainInfo.ExecOrderNo;
                        allocationRangeInfo.InvoiceNo = mainInfo.InvoiceNo;
                        allocationRangeInfo.RecipeNo = applyOutInfo.RecipeNO;
                        allocationRangeInfo.RecipeSequenceNo = applyOutInfo.SequenceNO.ToString();
                        allocationRangeInfo.StartOffset = startOffset;
                        allocationRangeInfo.EndOffset = endOffset;
                        allocationRangeInfo.AllocatedQty = allocateQty;
                        allocationRangeInfo.RangeStatus = "0";
                        allocationRangeInfo.CreatedCode = mainInfo.CreatedCode;
                        allocationRangeInfo.CreatedName = mainInfo.CreatedName;
                        allocationRangeInfo.IsDeleted = "N";
                        allocationRangeInfo.IsValid = "Y";

                        if (!this.traceCodeQueryService.InsertYbTraceAllocationRange(allocationRangeInfo))
                        {
                            errMsg += this.traceCodeQueryService.Err;
                            return -1;
                        }


                    }

                    if (totalAllocated != needQty)
                    {
                        errMsg += "种子数量分配错误!";
                        return -1;
                    }

                    //扣减库存
                    if (!this.traceCodeQueryService.UpdateYbTraceStockWhenUseSuccess(
                        drugDeptCode,
                        drugCode,
                        needQty))
                    {
                        errMsg += this.traceCodeQueryService.Err;
                        return -1;
                    }

                    //记录库存变化
                    var stockRecordInfo = new YbTraceStockRecord();

                    stockRecordInfo.Id = Guid.NewGuid().ToString();
                    stockRecordInfo.DrugCode = mainInfo.DrugCode;
                    stockRecordInfo.DrugName = mainInfo.DrugName;
                    stockRecordInfo.DrugDeptCode = mainInfo.PharmacyCode;
                    stockRecordInfo.DrugDeptName = mainInfo.PharmacyName;
                    stockRecordInfo.ChangeType = "2";
                    stockRecordInfo.BeforeTotalQty = stockInfo.TotalQty;
                    stockRecordInfo.BeforeAvailableQty = stockInfo.AvailableQty;
                    stockRecordInfo.BeforePredeductedQty = stockInfo.PreDeductedQty;
                    stockRecordInfo.BeforeExpiredQty = stockInfo.ExpiredQty;
                    stockRecordInfo.BeforeDamagedQty = stockInfo.DamagedQty;

                    stockRecordInfo.AfterTotalQty = stockRecordInfo.BeforeTotalQty - mainInfo.SplitNeedCollectQty;
                    stockRecordInfo.AfterAvailableQty = stockRecordInfo.BeforeAvailableQty - mainInfo.SplitNeedCollectQty;
                    stockRecordInfo.AfterPredeductedQty = stockRecordInfo.BeforePredeductedQty;
                    stockRecordInfo.AfterExpiredQty = stockRecordInfo.BeforeExpiredQty;
                    stockRecordInfo.AfterDamagedQty = stockRecordInfo.BeforeDamagedQty;
                    stockRecordInfo.RelatedTable = "pha_com_applyout";
                    stockRecordInfo.RelatedId = mainInfo.ApplyNumber;
                    stockRecordInfo.RelatedNo = mainInfo.ApplyNumber;
                    stockRecordInfo.CreatedCode = mainInfo.CreatedCode;
                    stockRecordInfo.CreatedName = mainInfo.CreatedName;

                    if (!this.traceCodeQueryService.InsertYbTraceStockRecord(stockRecordInfo))
                    {
                        errMsg += this.traceCodeQueryService.Err;
                        return -1;
                    }
                    #endregion

                }



            }

            //插入采集主表
            if (!this.traceCodeQueryService.InsertYbTraceCollectMain(mainInfo))
            {
                errMsg += this.traceCodeQueryService.Err;
                return -1;
            }

            //更新发药申请表
            if (!this.traceCodeQueryService.UpdateApplyOutWhenCollectSuccess(mainInfo, applyOutInfo.PackConvertToSplitFlag))
            {
                errMsg += this.traceCodeQueryService.Err;
            }

            return 1;

            //foreach (var traceDetailInfo in mainInfo.DetailList)
            //{
            //    if (!this.traceCodeQueryService.InsertYbTraceCollectDetail(traceDetailInfo))
            //    {
            //        errMsg += this.traceCodeQueryService.Err;
            //        return -1;
            //    }
            //}


        }

        public int SaveMZReturnTraceCodeInfo(YbTraceCollectMain mainInfo, ref string errMsg)
        {

            if (mainInfo.IsHaveSplit == YesNoEnum.Yes || mainInfo.IsHavePact == YesNoEnum.Yes)
            {
                //插入采集主表
                if (!this.traceCodeQueryService.InsertYbTraceCollectMain(mainInfo))
                {
                    errMsg += this.traceCodeQueryService.Err;
                    return -1;
                }
            }


            return 1;
        }

    }
}
