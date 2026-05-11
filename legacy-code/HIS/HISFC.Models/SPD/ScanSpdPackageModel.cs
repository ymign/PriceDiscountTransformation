using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.SPD
{
    public class ScanSpdPackageModel
    {
        /// <summary>
        /// 数据主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string InPatientNo { get; set; }

        /// <summary>
        /// 处方号
        /// </summary>
        public string RecipeNo { get; set; }

        /// <summary>
        /// 处方内序号
        /// </summary>
        public string SequenceNo { get; set; }

        /// <summary>
        /// 交易类型,1正交易，2反交易
        /// </summary>
        public string TransType { get; set; }

        public string ScanSpdCode { get; set; }

        /// <summary>
        /// 实际收费的项目编码(套包的编码)
        /// </summary>
        public string FeetemCode { get; set; }

        /// <summary>
        ///  实际收费的项目名称(套包的名称)
        /// </summary>
        public string FeeItemName { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specs { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string OperCode { get; set; }

        /// <summary>
        /// 操作员名称
        /// </summary>
        public string OperName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperTime { get; set; }

    }
}
