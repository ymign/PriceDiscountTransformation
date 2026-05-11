using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models.Views.OutPatient
{
    public class HcareResult
    {
        /// <summary>
        /// 医保减免
        /// </summary>
        public decimal HcareAmount { get; set; }
        /// <summary>
        /// 自费金额
        /// </summary>
        public decimal SelfAmount { get; set; }
        /// <summary>
        /// 医保报销
        /// </summary>
        public decimal ExpenseAmount { get; set; }
        /// <summary>
        /// 费用总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal EcostAmount { get; set; }
        /// <summary>
        /// 门诊流水号
        /// </summary>
        public string ClincCode { get; set; }

        public string BalanceNo { get; set; }
        public string regNO { get; set; }

    }
}
