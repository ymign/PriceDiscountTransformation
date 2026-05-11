using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Base;


namespace Neusoft.HISFC.Models.RADT
{
    public class VisitInfo
    {
        #region 变量

        /// <summary>
        /// 住院流水号
        /// </summary>
        private string INPATIENT_NO;

        /// <summary>
        /// 住院号
        /// </summary>
        private string PATIENT_NO;

        /// <summary>
        /// 姓名
        /// </summary>
        private string NAME;

        /// <summary>
        /// 生日
        /// </summary>
        private System.DateTime BIRTHDAY;

        /// <summary>
        /// 就诊卡号
        /// </summary>
        private string CARD_NO;

        /// <summary>
        /// 身份证号
        /// </summary>
        private string IDENNO;

        /// <summary>
        /// 户口或家庭地址
        /// </summary>
        private string HOME;

        /// <summary>
        /// 联系人姓名
        /// </summary>
        private string LINKMAN_NAME;

        /// <summary>
        /// 联系人电话
        /// </summary>
        private string LINKMAN_TEL;

        /// <summary>
        /// 诊断名称（建议用此保存主诊断）
        /// </summary>
        private string DIAG_NAME;

        /// <summary>
        /// 出院日期
        /// </summary>
        private System.DateTime OUT_DATE;

        /// <summary>
        /// 随访日期（月）
        /// </summary>
        private int VISIT_TIME;

        /// <summary>
        /// 操作人工号
        /// </summary>
        private string OPER_CODE;

        /// <summary>
        /// T分期
        /// </summary>
        private string TFQ;

        /// <summary>
        /// N分期
        /// </summary>
        private string NFQ;

        /// <summary>
        /// M分期
        /// </summary>
        private string MFQ;

        /// <summary>
        /// TNM分期评估
        /// </summary>
        private string TNM;

        /// <summary>
        /// 随访时间
        /// </summary>
        private System.DateTime VISIT_DATE;

        /// <summary>
        /// 年龄
        /// </summary>
        private string AGE;

        /// <summary>
        /// 操作时间
        /// </summary>
        private System.DateTime OPER_DATE;

        /// <summary>
        /// 是否随访
        /// </summary>
        private string VISIT_FLAG;

        /// <summary>
        /// 是否随访
        /// </summary>
        private string ID;



        #endregion

        #region 属性
        /// <summary>
        /// 住院流水号
        /// </summary>
        public string inpatient_no
        {
            get { return INPATIENT_NO; }
            set { INPATIENT_NO = value; }
        }
        /// <summary>
        /// 住院号
        /// </summary>
        public string patient_no
        {
            get { return PATIENT_NO; }
            set { PATIENT_NO = value; }
        }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name
        {
            get { return NAME; }
            set { NAME = value; }
        }
        /// <summary>
        /// 生日
        /// </summary>
        public System.DateTime birthday
        {
            get { return BIRTHDAY; }
            set { BIRTHDAY = value; }
        }
        /// <summary>
        /// 就诊卡号
        /// </summary>
        public string card_no
        {
            get { return CARD_NO; }
            set { CARD_NO = value; }
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string idenno
        {
            get { return IDENNO; }
            set { IDENNO = value; }
        }
        /// <summary>
        /// 户口或家庭地址
        /// </summary>
        public string home
        {
            get { return HOME; }
            set { HOME = value; }
        }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string linkman_name
        {
            get { return LINKMAN_NAME; }
            set { LINKMAN_NAME = value; }
        }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string linkman_tel
        {
            get { return LINKMAN_TEL; }
            set { LINKMAN_TEL = value; }
        }

        /// <summary>
        /// 诊断名称（建议用此保存主诊断）
        /// </summary>
        public string diag_name
        {
            get { return DIAG_NAME; }
            set { DIAG_NAME = value; }
        }

        /// <summary>
        /// 出院日期
        /// </summary>
        public System.DateTime out_date
        {
            get { return OUT_DATE; }
            set { OUT_DATE = value; }
        }

        /// <summary>
        /// 随访日期（月）
        /// </summary>
        public int visit_time
        {
            get { return VISIT_TIME; }
            set { VISIT_TIME = value; }
        }

        /// <summary>
        /// 操作人工号
        /// </summary>
        public string oper_code
        {
            get { return OPER_CODE; }
            set { OPER_CODE = value; }
        }



        /// <summary>
        /// T分期
        /// </summary>
        public string tfq
        {
            get { return TFQ; }
            set { TFQ = value; }
        }

        /// <summary>
        /// N分期
        /// </summary>
        public string nfq
        {
            get { return NFQ; }
            set { NFQ = value; }
        }
        /// <summary>
        /// M分期
        /// </summary>
        public string mfq
        {
            get { return MFQ; }
            set { MFQ = value; }
        }
        /// <summary>
        /// TNM分期评估
        /// </summary>
        public string tnm
        {
            get { return TNM; }
            set { TNM = value; }
        }
        /// <summary>
        /// 随访时间
        /// </summary>
        public System.DateTime visit_date
        {
            get { return VISIT_DATE; }
            set { VISIT_DATE = value; }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public string age
        {
            get { return AGE; }
            set { AGE = value; }
        }

        /// <summary>
        /// 操作日期
        /// </summary>
        public System.DateTime oper_date
        {
            get { return OPER_DATE; }
            set { OPER_DATE = value; }
        }
        /// <summary>
        /// 是否随访
        /// </summary>
        public string visit_flag
        {
            get { return VISIT_FLAG; }
            set { VISIT_FLAG = value; }
        }
        /// <summary>
        /// 主键列
        /// </summary>
        public string id
        {
            get { return ID; }
            set { ID = value; }
        }

        #endregion
    }
}
