using System;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.Fee
{


    /// <summary>
    /// InvoiceExtend 的摘要说明
    /// 实际发票号与流水号对照表
    /// ID为发票系统流水号
    /// </summary>
    /// 
    [System.Serializable]
    public class InvoiceExtend : Neusoft.FrameWork.Models.NeuObject
    {
        
        public InvoiceExtend()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 变量

        /// <summary>
        /// 实际发票号
        /// </summary>
        private string realInvoiceNo;

        /// <summary>
        /// 发票类型
        /// </summary>
        private string invoiceType;

        /// <summary>
        /// 实际发票字母头
        /// </summary>
        private string invoiceHead;

        /// <summary>
        /// 有效标记:1正常 0作废
        /// </summary>
        private string invoiceState;

        /// <summary>
        /// 结算操作环境(具体结算操作员,结算时间)
        /// </summary>
        private OperEnvironment oper = new OperEnvironment();

        /// <summary>
        /// 作废人信息
        /// </summary>
        private OperEnvironment wasteOper = new OperEnvironment();

        #endregion

        #region 属性
     
        /// <summary>
        /// 实际发票号,发票上原始印的号
        /// </summary>
        public string RealInvoiceNo
        {
            set
            {
                realInvoiceNo = value;
            }
            get
            {
                return realInvoiceNo;
            }
        }

        /// <summary>
        /// 发票类型：C门诊收据;A门诊账户预交;R挂号票;I住院收据;P住院预交金
        /// </summary>
        public string InvoiceType
        {
            get
            {
                return invoiceType;
            }
            set
            {
                this.invoiceType = value;
            }
        }               

        /// <summary>
        /// 发票号字母头
        /// </summary>
        public string InvvoiceHead
        {
            get
            {
                return invoiceHead;
            }
            set
            {
                invoiceHead = value;
            }
        }

        /// <summary>
        /// 发票状态：1有效，0退费，2作废
        /// </summary>
        public string InvoiceState
        {
            get
            {
                return this.invoiceState;
            }
            set
            {
                this.invoiceState = value;
            }
        }

        /// <summary>
        /// 操作员
        /// </summary>
        public OperEnvironment Oper
        {
            get
            {
                return this.oper;
            }
            set
            {
                this.oper = value;
            }
        }

        /// <summary>
        /// 作废人信息
        /// </summary>
        public OperEnvironment WasteOper
        {
            get
            {
                return this.wasteOper;
            }
            set
            {
                this.wasteOper = value;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public new InvoiceExtend Clone()
        {
            Neusoft.HISFC.Models.Fee.InvoiceExtend obj = base.Clone() as Neusoft.HISFC.Models.Fee.InvoiceExtend;
            obj.Oper = this.Oper.Clone();
            obj.wasteOper = this.wasteOper.Clone();
            return obj;
        }

        #endregion
    }
}
