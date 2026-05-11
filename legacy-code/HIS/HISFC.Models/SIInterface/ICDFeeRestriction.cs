using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.SIInterface
{
    /// <summary>
    /// 超限原因实体
    /// </summary>
    [Serializable]
    public class ICDFeeRestriction
    {
        /// <summary>
        /// 住院号
        /// </summary>
        private string inpatient_no = string.Empty;
        /// <summary>
        /// 住院号
        /// </summary>
        public string Inpatient_no
        {
            get { return inpatient_no; }
            set { inpatient_no = value; }
        }

        /// <summary>
        /// 超限原因类型
        /// </summary>
        private string icdFeeRestriction = string.Empty;
        /// <summary>
        /// 超限原因类型
        /// </summary>
        public string IcdFeeRestriction
        {
            get { return icdFeeRestriction; }
            set { icdFeeRestriction = value; }
        }

        /// <summary>
        /// 原因描述1
        /// </summary>
        private string reason1 = string.Empty;
        /// <summary>
        /// 原因描述1
        /// </summary>
        public string Reason1
        {
            get { return reason1; }
            set { reason1 = value; }
        }

        /// <summary>
        /// 原因描述2
        /// </summary>
        private string reason2 = string.Empty;
        /// <summary>
        /// 原因描述2
        /// </summary>
        public string Reason2
        {
            get { return reason2; }
            set { reason2 = value; }
        }

        /// <summary>
        /// 原因描述3
        /// </summary>
        private string reason3 = string.Empty;
        /// <summary>
        /// 原因描述3
        /// </summary>
        public string Reason3
        {
            get { return reason3; }
            set { reason3 = value; }
        }

        /// <summary>
        /// 操作员
        /// </summary>
        private string opercode = string.Empty;
        /// <summary>
        /// 操作员
        /// </summary>
        public string Opercode
        {
            get { return opercode; }
            set { opercode = value; }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime operdate;
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Operdate
        {
            get { return operdate; }
            set { operdate = value; }
        }

        /// <summary>
        /// icd10
        /// </summary>
        private string icd10 = string.Empty;

        public string Icd10
        {
            get { return icd10; }
            set { icd10 = value; }
        }

        private string icd9 = string.Empty;

        public string Icd9
        {
            get { return icd9; }
            set { icd9 = value; }
        }
    }
}
