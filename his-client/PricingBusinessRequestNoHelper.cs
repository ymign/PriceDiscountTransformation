using System;

namespace HIS.Pricing.Client
{
    /// <summary>
    /// BusinessRequestNo 生成辅助类。
    /// </summary>
    public static class PricingBusinessRequestNoHelper
    {
        /// <summary>
        /// 根据调用方传入值或 HIS 收费单号生成稳定业务请求号。
        /// 无法推导稳定业务号时返回空字符串，交由 confirm 前置校验阻断。
        /// </summary>
        public static string EnsureBusinessRequestNo(string existingBusinessRequestNo, string chargeNo)
        {
            if (!string.IsNullOrEmpty(existingBusinessRequestNo))
            {
                return existingBusinessRequestNo;
            }

            if (!string.IsNullOrEmpty(chargeNo))
            {
                return "HIS_CHARGE_" + chargeNo;
            }

            return string.Empty;
        }
    }
}
