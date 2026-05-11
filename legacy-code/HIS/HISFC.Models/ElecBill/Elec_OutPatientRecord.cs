using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.ElecBill
{
    /// <summary>
    /// 电子票信息实体类
    /// </summary>
    public class Elec_OutPatientRecord
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string clinic_code { get; set; }
        /// <summary>
        /// 就诊卡号
        /// </summary>
        public string card_no { get; set; }
        /// <summary>
        /// 电子票据代码
        /// </summary>
        public string billBatchCode { get; set; }
        /// <summary>
        /// 电子票据号码
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 电子校验码
        /// </summary>
        public string random { get; set; }
        /// <summary>
        /// 电子票据生成时间
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 电子票据二维码图片数据
        /// </summary>
        public string billQRCode { get; set; }
        /// <summary>
        /// 电子票据H5页面url
        /// </summary>
        public string pictureUrl { get; set; }
        /// <summary>
        /// 电子票据外网H5页面url
        /// </summary>
        public string pictureNetUrl { get; set; }
        /// <summary>
        /// 暂时使用
        /// </summary>
        public string billBusDate { get; set; }
        /// <summary>
        /// 类型 1：挂号  2：门诊收费
        /// </summary>
        public string billType { get; set; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        public string createCode { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string createName { get; set; }

        public string state { get; set; }
    }
}
