using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Operation
{
    /// <summary>
    /// 高值耗材收费明细
    /// </summary>
    public class OpsMdhChargeDetail
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 患者住院流水号
        /// </summary>
        public string VisitFlowId { get; set; }
        /// <summary>
        /// 患者住院号
        /// </summary>
        public string VisitId { get; set; }
        /// <summary>
        /// 手术申请单号
        /// </summary>
        public string OpsApplyNo { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 项目类型
        /// </summary>
        public string ProjectType { get; set; }
        /// <summary>
        /// 计费类型
        /// </summary>
        public string ChargeType { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotAmount { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 开单医生工号
        /// </summary>
        public string DoctId { get; set; }
        /// <summary>
        /// 开单科室
        /// </summary>
        public string DeptId { get; set; }
        /// <summary>
        /// 高值耗材码
        /// </summary>
        public string InputCode { get; set; }
        /// <summary>
        /// 计费数据接收时间
        /// </summary>
        public DateTime ReciveTime { get; set; }
        /// <summary>
        /// 收费标志；0：未收费 1：已收费
        /// </summary>
        public string FeeFlag { get; set; }
        /// <summary>
        /// 收费员信息
        /// </summary>
        public FrameWork.Models.NeuObject FeeUser = new FrameWork.Models.NeuObject();
        /// <summary>
        /// 收费时间
        /// </summary>
        public DateTime OperTime { get; set; }
    }
}
