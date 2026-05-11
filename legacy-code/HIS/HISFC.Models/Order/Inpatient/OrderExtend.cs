using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Order.Inpatient
{
    /// <summary>
    /// [功能描述: 住院医嘱扩展表]<br></br>
    /// [创 建 者: ]<br></br>
    /// [创建时间: ]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [Serializable]
    public class OrderExtend : Neusoft.FrameWork.Models.NeuObject
    {
        public OrderExtend()
        {
        }

        #region 变量

        /// <summary>
        /// 住院流水号
        /// </summary>
        private string inPatientNo;

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string InPatientNo
        {
            get { return inPatientNo; }
            set { inPatientNo = value; }
        }

        /// <summary>
        /// 医嘱流水号
        /// </summary>
        private string moOrder;

        /// <summary>
        /// 医嘱流水号
        /// </summary>
        public string MoOrder
        {
            get { return moOrder; }
            set { moOrder = value; }
        }

        /// <summary>
        /// 适应症标记
        /// </summary>
        private string indications;

        /// <summary>
        /// 适应症标记
        /// </summary>
        public string Indications
        {
            get { return indications; }
            set { indications = value; }
        }

        /// <summary>
        /// 扩展1
        /// </summary>
        private string extend1 = string.Empty;

        /// <summary>
        /// 扩展1
        /// </summary>
        public string Extend1
        {
            get { return extend1; }
            set { extend1 = value; }
        }

        /// <summary>
        /// 扩展2
        /// </summary>
        private string extend2 = string.Empty;

        /// <summary>
        /// 扩展2
        /// </summary>
        public string Extend2
        {
            get { return extend2; }
            set { extend2 = value; }
        }

        /// <summary>
        /// 扩展3
        /// </summary>
        private string extend3 = string.Empty;

        /// <summary>
        /// 扩展3
        /// </summary>
        public string Extend3
        {
            get { return extend3; }
            set { extend3 = value; }
        }

        /// <summary>
        /// 扩展4
        /// </summary>
        private string extend4 = string.Empty;

        /// <summary>
        /// 扩展4
        /// </summary>
        public string Extend4
        {
            get { return extend4; }
            set { extend4 = value; }
        }

        /// <summary>
        /// 扩展5
        /// </summary>
        private string extend5 = string.Empty;

        /// <summary>
        /// 扩展5
        /// </summary>
        public string Extend5
        {
            get { return extend5; }
            set { extend5 = value; }
        }

        /// <summary>
        /// 扩展6
        /// </summary>
        private string extend6 = string.Empty;

        /// <summary>
        /// 扩展6
        /// </summary>
        public string Extend6
        {
            get
            {
                return extend6;
            }
            set
            {
                extend6 = value;
            }
        }

        /// <summary>
        /// 扩展7
        /// </summary>
        private string extend7 = string.Empty;

        /// <summary>
        /// 扩展7
        /// </summary>
        public string Extend7
        {
            get { return extend7; }
            set { extend7 = value; }
        }

        /// <summary>
        /// 扩展8
        /// </summary>
        private string extend8 = string.Empty;

        /// <summary>
        /// 扩展8
        /// </summary>
        public string Extend8
        {
            get { return extend8; }
            set { extend8 = value; }
        }

        /// <summary>
        /// 扩展9
        /// </summary>
        private string extend9 = string.Empty;

        /// <summary>
        /// 扩展9
        /// </summary>
        public string Extend9
        {
            get { return extend9; }
            set { extend9 = value; }
        }

        /// <summary>
        /// 扩展10
        /// </summary>
        private string extend10 = string.Empty;

        /// <summary>
        /// 扩展10
        /// </summary>
        public string Extend10
        {
            get { return extend10; }
            set { extend10 = value; }
        }

        /// <summary>
        /// 操作人员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        /// <summary>
        /// 操作人员
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get { return oper; }
            set { oper = value; }
        }

        #endregion

        #region 属性

        #endregion

        public new OrderExtend Clone()
        {
            OrderExtend orderExt = base.Clone() as OrderExtend;
            orderExt.oper = this.oper.Clone();
            return orderExt;
        }
    }
}
