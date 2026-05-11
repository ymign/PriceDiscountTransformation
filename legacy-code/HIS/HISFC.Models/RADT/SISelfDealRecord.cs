using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.RADT
{
    /// <summary>
    /// 医保自助出院待办处理记录
    /// </summary>
    public class SISelfDealRecord : Neusoft.FrameWork.Models.NeuObject
    {
        public SISelfDealRecord()
        {

        }

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string InpatientNo = "";

        /// <summary>
        /// 接单操作人
        /// </summary>
        public string ReceiveOperCode = "";

        /// <summary>
        /// 接单时间
        /// </summary>
        public DateTime ReceiveDate;

        /// <summary>
        /// 接单状态：0 未接单；1 接单中；2 已处理；3 已撤单；
        /// </summary>
        public string ReceiveState = "";

        /// <summary>
        /// 撤单操作人
        /// </summary>
        public string RevokeOperCode = "";

        /// <summary>
        /// 撤单时间
        /// </summary>
        public DateTime RevokeDate;

        /// <summary>
        /// 撤单原因
        /// </summary>
        public string RevokeReason = "";

        /// <summary>
        /// 撤单分类
        /// </summary>
        public string RevokeType = "";

        /// <summary>
        /// 是否已自助结算放行
        /// </summary>
        public bool PaseFlag = false;

        /// <summary>
        /// 结算放行的操作人
        /// </summary>
        public string PaseOperCode = "";

        /// <summary>
        /// 结算放行的操作时间
        /// </summary>
        public DateTime PaseDate;

    }
}
