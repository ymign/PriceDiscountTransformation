using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    /// <summary>
    /// 发药申请的追溯码采集状态枚举
    /// </summary>
    public class TraceCodeCollectionStatusEnum
    {
        /// <summary>
        /// 待采集
        /// </summary>
        public const string Pending = "0"; // 待采集

        /// <summary>
        /// 采集中
        /// </summary>
        public const string Collecting = "1"; // 采集中

        /// <summary>
        /// 不用采集
        /// </summary>
        public const string NotRequired = "2"; // 不用采集

        /// <summary>
        /// 采集成功
        /// </summary>
        public const string Sucess = "3"; // 采集成功

        /// <summary>
        /// 跳过采集
        /// </summary>
        public const string Skipped = "4"; // 跳过采集

        /// <summary>
        /// 采集失败
        /// </summary>
        public const string Failed = "5"; // 采集失败

        /// <summary>
        ///  采集完成（特殊场景下,无法全部采集成功的情况）
        /// </summary>
        public const string Completed = "6"; //

        /// <summary>
        /// 部分采集成功
        /// </summary>
        public const string PartiallyCollected = "7"; // 

        /// <summary>判断码值是否合法</summary>
        public static bool IsValid(string code)
        {
            return string.IsNullOrEmpty(code)
                || code == Pending
                || code == Collecting
                || code == NotRequired
                || code == Sucess
                || code == Skipped
                || code == Failed
                || code == Completed
                || code == PartiallyCollected
                ;
        }



        /// <summary>
        /// 根据相关数量获取采集状态
        /// </summary>
        /// <param name="needCollectQty">应采数量</param>
        /// <param name="actualCollectQty">实采数量</param>
        /// <param name="appealCollectQty">申诉数量</param>
        /// <returns></returns>
        public static string GetStatusForQty(
            decimal needCollectQty,
            decimal actualCollectQty,
            decimal appealCollectQty)
        {
            if (needCollectQty == 0)
            {
                return NotRequired;
            }

            if (needCollectQty == actualCollectQty)
            {
                return Sucess;
            }

            if (actualCollectQty + appealCollectQty == needCollectQty)
            {
                return Completed;
            }

            if (actualCollectQty + appealCollectQty < needCollectQty)
            {
                return PartiallyCollected;
            }

            if (actualCollectQty + appealCollectQty > needCollectQty)
            {
                return "-1";
            }

            if (actualCollectQty == 0 && appealCollectQty >= 0)
            {
                return Collecting;
            }

            return "-1";
        }

        /// <summary>
        /// 是否不允许采集
        /// </summary>
        public static bool IsCanNotCollect(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                code = Pending;
            }

            return code == NotRequired ||
                   code == Sucess ||
                   code == Skipped ||
                   code == Completed;
        }

        /// <summary>
        /// 是否已经采集完成
        /// </summary>
        /// <returns></returns>
        public static bool IsCollectCompleted(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                code = Pending;
            }
            return code == NotRequired ||
                      code == Sucess ||
                      code == Skipped ||
                      code == Completed;
        }

        /// <summary>
        /// 获取状态描述
        /// </summary>
        public static string GetDescription(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                code = Pending;
            }

            switch (code)
            {
                case Pending: return "待采集";
                case Collecting: return "采集中";
                case NotRequired: return "不用采集";
                case Sucess: return "采集成功";
                case Skipped: return "跳过采集";
                case Failed: return "采集失败";
                case Completed: return "采集完成";
                case PartiallyCollected: return "部分采集成功";
                default: return "未知状态";
            }
        }
    }
}
