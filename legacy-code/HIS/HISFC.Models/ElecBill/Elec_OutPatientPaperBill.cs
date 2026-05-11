using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.ElecBill
{
    /// <summary>
    /// 电子纸质票信息实体
    /// </summary>
    public class Elec_OutPatientPaperBill
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 电子票据代码
        /// </summary>
        public string billBatchCode { get; set; }
        /// <summary>
        /// 电子票据号
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 纸质票据代码
        /// </summary>
        public string pBillBatchCode { get; set; }
        /// <summary>
        /// 纸质票据号
        /// </summary>
        public string pBillNo { get; set; }
        /// <summary>
        /// 状态(1:有效,2:作废,3:重新换开,4:空白票
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        public string createCode { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string createName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime lastmodifytime { get; set; }
        /// <summary>
        /// 最后修改编码
        /// </summary>
        public string lastmodifycode { get; set; }
        /// <summary>
        /// 最后修改人名称
        /// </summary>
        public string lastmodifyname { get; set; }
        /// <summary>
        /// 类型 1挂号 2门诊
        /// </summary>
        public string billType { get; set; }
    }
}
