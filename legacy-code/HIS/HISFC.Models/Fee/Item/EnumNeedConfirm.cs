using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Fee.Item
{
    /// <summary>
    /// 终端确认枚举（门诊，住院，其他，全部）
    /// </summary>
    public enum EnumNeedConfirm
    {
        [Neusoft.FrameWork.Public.Description("无")]
        None = 0,
        [Neusoft.FrameWork.Public.Description("全部")]
        All=1,
        [Neusoft.FrameWork.Public.Description("门诊")]
        Outpatient=2,
        [Neusoft.FrameWork.Public.Description("住院")]
        Inpatient = 3
    }
}
