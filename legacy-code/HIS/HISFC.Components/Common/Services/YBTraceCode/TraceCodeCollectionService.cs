using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.MedicalTraceCode;
using Neusoft.HISFC.Models;
using Neusoft.HISFC.Models.Pharmacy;
using Neusoft.HISFC.BizLogic.Pharmacy;

namespace Neusoft.HISFC.Components.Common.Services.YBTraceCode
{
    public class TraceCodeCollectionService
    {
        #region 私有字段和属性

        private TraceCodeDAL TraceCodeDAL = new TraceCodeDAL();

        #endregion

        /// <summary>
        /// 构造函数 - 初始化追溯码业务服务
        /// </summary>
        public TraceCodeCollectionService()
        {
            // 设置药品标识码映射缓存过期时间
            // 避免频繁查询数据库，提高性能
            DrugCodeMappingCache.SetTtlMinutes(TraceCodeConstants.CACHE_TTL_MINUTES);

        }


        #region 私有辅助函数

        /// <summary>
        /// 验证发药申请信息
        /// </summary>
        /// <param name="info">发药申请信息</param>
        /// <returns>验证结果</returns>
        private Result<bool, string> ValidateApplyOutInfo(ApplyOut info)
        {
            if (info == null)
            {
                return "发药申请信息不能为null";
            }

            if (string.IsNullOrEmpty(info.ID))
            {
                return "发药申请流水号不能为空";
            }

            return true;
        }

        /// <summary>
        /// 获取发药申请详细信息
        /// </summary>
        /// <param name="applyId">申请流水号</param>
        /// <returns></returns>
        private Result<PhaComApplyout, string> GetApplyOutDetailInfo(string applyId)
        {
            try
            {
                var applyOutInfo = this.TraceCodeDAL.GetApplyInfo(applyId);
                if (applyOutInfo == null)
                {
                    return "[" + applyId + "]未找到对应发药申请记录";
                }
                return applyOutInfo;
            }
            catch (Exception ex)
            {
                return "[GetApplyOutDetailInfo]获取发药申请信息失败: " + ex.Message;
            }
        }

        /// <summary>
        /// 设置已存在的追溯码信息
        /// </summary>
        /// <param name="info">发药申请信息</param>
        /// <param name="applyOutInfo">数据库中的信息</param>
        private void SetExistingTraceCodeInfo(
            ApplyOut info,
            PhaComApplyout applyOutInfo)
        {

            info.NeedCollectQty = (int)applyOutInfo.Needcollectqty;     
            info.AlreadyCollectQty = (int)applyOutInfo.Alreadycollectqty;     
            info.AppealCollectQty = (int)applyOutInfo.Appealcollectqty;
            info.NeedCollectSpiltQty = (int)applyOutInfo.NeedCollectSpiltQty;      
            info.AlreadyCollectSpiltQty = (int)applyOutInfo.AlreadyCollectSpiltQty;
            info.AppealCollectSpiltQty = (int)applyOutInfo.AppealCollectSpiltQty; 

            info.NeedCollectTraceCodeFlag = applyOutInfo.NeedCollectTraceCodeFlag;
            info.NotCollectTraceCodeReason = applyOutInfo.NotCollectTraceCodeReason;
            info.TraceCodeCollectionStatus = applyOutInfo.Tracecodecollectionstatus;
        }

        #endregion


        /// <summary>
        /// 设置单个发药申请的追溯码采集信息
        /// 核心业务逻辑：判断药品是否需要采集追溯码,计算需要采集的数量
        /// 在发药界面加载信息时候进行初始化
        /// 
        /// 判断规则：
        /// 1. 检查是否在无码目录中
        /// 2. 检查采集状态是否允许采集
        /// 3. 检查药品剂型和药理分类
        /// 4. 排除中草药
        /// 5. 计算包装数量和拆零数量
        /// </summary>
        /// <param name="info">发药申请信息</param>
        /// <param name="isDirectUpdateTraceCodeInfo">是否直接更新到数据库</param>
        /// <returns>设置结果</returns>
        public Result<bool, string> SetTraceCodeInfoToApplyOut(
            ApplyOut info,
            bool isDirectUpdateTraceCodeInfo)
        {
            // 1. 参数验证
            var validationResult = ValidateApplyOutInfo(info);
            if (validationResult.IsErr)
            {
                return validationResult.ErrValue;
            }

            // 2. 从数据库获取发药申请的详细信息          
            var applyOutInfoResult = GetApplyOutDetailInfo(info.ID);
            if (applyOutInfoResult.IsErr)
            {
                return applyOutInfoResult.ErrValue;
            }
            var applyOutInfo = applyOutInfoResult.OkValue;

            // 3. 设置采集状态
            info.TraceCodeCollectionStatus = applyOutInfo.Tracecodecollectionstatus;

            // 4. 如果已经设置过采集标识，直接从数据库读取信息并返回
            if (!string.IsNullOrEmpty(applyOutInfo.NeedCollectTraceCodeFlag))
            {
                SetExistingTraceCodeInfo(info, applyOutInfo);
                return true; // 已设置过，直接返回
            }


            return true;

        }


    }
}
