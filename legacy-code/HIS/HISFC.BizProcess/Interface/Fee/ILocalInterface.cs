using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.HISFC.Models.Fee.Outpatient;

namespace Neusoft.HISFC.BizProcess.Interface.FeeInterface
{
    /// <summary>
    /// 本地医保结算统一接口
    /// {7CB82175-3384-49e0-8869-1CE350F85200}
    /// </summary>
    public interface ILocalMedcare : IMedcareTranscation
    {
        /// <summary>
        /// 本地数据连接
        /// </summary>
        System.Data.IDbTransaction Trans
        {
            set;
        }
        /// <summary>
        /// 设置本地数据库连接
        /// </summary>
        /// <param name="t"></param>
        void SetTrans(System.Data.IDbTransaction t);

        /// <summary>
        /// 错误编码
        /// </summary>
        string ErrCode
        {
            get;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        string ErrMsg
        {
            get;
        }

        /// <summary>
        /// 控件描述，最好填写。
        /// </summary>
        string Description
        {
            get;
        }
        /// <summary>
        /// 根据合同单位，计算病人费用信息
        /// pub_cost、 pay_cost、 own_cost、 eco_cost
        /// </summary>
        /// <param name="r">病人挂号信息</param>
        /// <param name="lstFeeItem">费用明细信息</param>
        /// <returns></returns>
        long ComputeOutPatientFeeCost(Neusoft.HISFC.Models.Registration.Register r, ref List<FeeItemList> lstFeeItem);
    }
}
