using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    /// 取价格函数接口
    /// </summary>
    public interface IGetItemPrice
    {
        /// <summary>
        /// 门诊获取项目价格函数
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <param name="register">挂号信息</param>
        /// <param name="UnitPrice">三甲价</param>
        /// <param name="ChildPrice">儿童价</param>
        /// <param name="SPPrice">特诊价</param>
        /// <param name="PurchasePrice">购入价</param>
        /// <param name="orgPrice">原始价格</param>
        /// <returns></returns>
        decimal GetPrice(string itemCode, Neusoft.HISFC.Models.Registration.Register register, decimal UnitPrice, decimal ChildPrice, decimal SPPrice, decimal PurchasePrice, ref decimal orgPrice);

        /// <summary>
        /// 住院取价格函数，根据合同单位和项目信息获取患者收费价格和原始价格
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <param name="patientInfo">患者信息</param>
        /// <param name="UnitPrice">三甲价</param>
        /// <param name="ChildPrice">儿童价</param>
        /// <param name="SPPrice">特诊价</param>
        /// <param name="PurchasePrice">购入价</param>
        /// <param name="orgPrice">原始价格</param>
        /// <returns></returns>
        decimal GetPriceForInpatient(string itemCode, Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, decimal UnitPrice, decimal ChildPrice, decimal SPPrice, decimal PurchasePrice, ref decimal orgPrice);
    }
}
