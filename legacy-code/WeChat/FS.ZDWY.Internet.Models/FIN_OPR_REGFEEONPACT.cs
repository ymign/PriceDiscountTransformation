using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    /// <summary>
    /// 挂号费分配方案
    /// </summary>
    public class FIN_OPR_REGFEEONPACT
    {
        /// <summary>
        /// 流水号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String ID { get; set; }

        /// <summary>
        /// 合同单位
        /// </summary>
        public System.String PACT_CODE { get; set; }

        /// <summary>
        /// 挂号级别
        /// </summary>
        public System.String REGLEVL_CODE { get; set; }

        /// <summary>
        /// 适用范围 ALL全院,其余为特殊科室代码
        /// </summary>
        public System.String DEPT_CODE { get; set; }

        /// <summary>
        /// 挂号费
        /// </summary>
        public Decimal REG_FEE { get; set; }

        /// <summary>
        /// 检查费
        /// </summary>
        public Decimal CHCK_FEE { get; set; }

        /// <summary>
        /// 诊查费自费金额
        /// </summary>
        public Decimal DIAG_FEE { get; set; }

        /// <summary>
        /// 附加费
        /// </summary>
        public Decimal OTH_FEE { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public System.String OPER_CODE { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public System.DateTime? OPER_DATE { get; set; }

        /// <summary>
        /// 诊查费记帐金额
        /// </summary>
        public System.Single? DIAG_PUBFEE { get; set; }
    }
}
