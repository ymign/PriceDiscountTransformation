using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.RADT
{
    /// <summary>
    /// 医保自助出院待办处理记录
    /// </summary>
    public class SISelfDealRecordDto : Neusoft.FrameWork.Models.NeuObject
    {
        public SISelfDealRecordDto()
        {

        }

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string InpatientNo = string.Empty;

        /// <summary>
        /// 住院号
        /// </summary>
        public string PatientNo = string.Empty;

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName = string.Empty;

        /// <summary>
        /// 结算类别编码：01 自费；02 医保；03 公费；
        /// </summary>
        public string PayKindCode = string.Empty;

        /// <summary>
        /// 是否已医保结算状态
        /// </summary>
        public bool IsSIBalanced = false;

        /// <summary>
        /// 住院科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Dept = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 合同单位
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Pact = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 接单记录
        /// </summary>
        public SISelfDealRecord DealRecord = new SISelfDealRecord();

        /// <summary>
        /// 入院时间
        /// </summary>
        public DateTime DateIn;

        /// <summary>
        /// 出院时间
        /// </summary>
        public DateTime DateOut;

        /// <summary>
        /// 病理放行标志：0 未放行；1 放行；空 不判断；
        /// </summary>
        public string PathologyPassFlag = string.Empty;

        /// <summary>
        /// 在院状态：R-住院登记  I-病房接诊 B-出院登记 C-封账 O-出院结算 P-预约出院,N-无费退院
        /// </summary>
        public string InState = string.Empty;


    }
}
