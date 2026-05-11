using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.RADT
{
    public class Notice
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
        /// 科室代码
        /// </summary>
        private string DEPT_CODE;
        
        /// <summary>
        /// 科室名称
        /// </summary>
        private string DEPT_NAME;
        
        /// <summary>
        /// 医保归属地
        /// </summary>
        private string PACT_LOCAL;
        
        /// <summary>
        /// 出院情况
        /// </summary>
        private string OUT_STATE;
        
        /// <summary>
        /// 出院情况备注
        /// </summary>
        private string OUT_MARK;
        
        /// <summary>
        /// 诊治方式
        /// </summary>
        private string ZZFS;
        
        /// <summary>
        /// 诊治方式备注
        /// </summary>
        private string ZZFS_MARK;

        
        /// <summary>
        /// 诊断一名称
        /// </summary>
        private string DIAGN1_NAME;
        
        /// <summary>
        /// 诊断二名称
        /// </summary>
        private string DIAGN2_NAME;

        
        /// <summary>
        /// 诊断三名称
        /// </summary>
        private string DIAGN3_NAME;
        
        /// <summary>
        /// 诊断一icd
        /// </summary>
        private string DIAGN1_ICD;
        
        /// <summary>
        /// 诊断二icd
        /// </summary>
        private string DIAGN2_ICD;
        
        /// <summary>
        /// 诊断三icd
        /// </summary>
        private string DIAGN3_ICD;
        
        /// <summary>
        /// 珠海医保结算方式
        /// </summary>
        private string ZHUHAI_WAY;
        
        /// <summary>
        /// 专项名称
        /// </summary>
        private string ZX_NAME;

        /// <summary>
        /// 床号
        /// </summary>
        private string BEDNO;

        /// <summary>
        /// 医生
        /// </summary>
        private string DOCTCODE;

        /// <summary>
        /// 日间手术
        /// </summary>
        private string DayOperation;
        /// <summary>
        /// 新生儿入院类型
        /// </summary>
        private string NWB_ADM_TYPE;
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
        /// 科室代码
        /// </summary>
        public string dept_code
        {
            get { return DEPT_CODE; }
            set { DEPT_CODE = value; }
        }
        /// <summary>
        /// 科室名称
        /// </summary>
        public string dept_name
        {
            get { return DEPT_NAME; }
            set { DEPT_NAME = value; }
        }
        /// <summary>
        /// 医保归属地
        /// </summary>
        public string pact_local
        {
            get { return PACT_LOCAL; }
            set { PACT_LOCAL = value; }
        }
        /// <summary>
        /// 医生
        /// </summary>
        public string doctCode
        {
            get { return DOCTCODE; }
            set { DOCTCODE = value; }
        }
        
        /// <summary>
        /// 出院情况
        /// </summary>
        public string out_state
        {
            get { return OUT_STATE; }
            set { OUT_STATE = value; }
        }
        /// <summary>
        /// 出院情况备注
        /// </summary>
        public string out_mark
        {
            get { return OUT_MARK; }
            set { OUT_MARK = value; }
        }
        /// <summary>
        /// 诊治方式
        /// </summary>
        public string zzfs
        {
            get { return ZZFS; }
            set { ZZFS = value; }
        }
        /// <summary>
        /// 诊治方式备注
        /// </summary>
        public string zzfs_mark
        {
            get { return ZZFS_MARK; }
            set { ZZFS_MARK = value; }
        }
        /// <summary>
        /// 诊断一名称
        /// </summary>
        public string diagn1_name
        {
            get { return DIAGN1_NAME; }
            set { DIAGN1_NAME = value; }
        }

        /// <summary>
        /// 诊断二名称
        /// </summary>
        public string diagn2_name
        {
            get { return DIAGN2_NAME; }
            set { DIAGN2_NAME = value; }
        }
        /// <summary>
        /// 诊断三名称
        /// </summary>
        public string diagn3_name
        {
            get { return DIAGN3_NAME; }
            set { DIAGN3_NAME = value; }
        }
        /// <summary>
        /// 诊断一icd
        /// </summary>
        public string diagn1_icd
        {
            get { return DIAGN1_ICD; }
            set { DIAGN1_ICD = value; }
        }
        /// <summary>
        /// 诊断二icd
        /// </summary>
        public string diagn2_icd
        {
            get { return DIAGN2_ICD; }
            set { DIAGN2_ICD = value; }
        }
        /// <summary>
        /// 诊断三icd
        /// </summary>
        public string diagn3_icd
        {
            get { return DIAGN3_ICD; }
            set { DIAGN3_ICD = value; }
        }
        /// <summary>
        /// 珠海医保结算方式
        /// </summary>
        public string zhuhai_way
        {
            get { return ZHUHAI_WAY; }
            set { ZHUHAI_WAY = value; }
        }
        /// <summary>
        /// 专项名称
        /// </summary>
        public string zx_name
        {
            get { return ZX_NAME; }
            set { ZX_NAME = value; }
        }
        /// <summary>
        /// 床号
        /// </summary>
        public string bedno
        {
            get { return BEDNO; }
            set { BEDNO = value; }
        }
        /// <summary>
        /// 日间手术
        /// </summary>
        public string dayOperation
        {
            get { return DayOperation; }
            set { DayOperation = value; }
        }
        /// <summary>
        /// 新生儿入院类型
        /// </summary>
        public string nwb_aim_type
        {
            get { return NWB_ADM_TYPE; }
            set { NWB_ADM_TYPE = value; }
        }

        /// <summary>
        /// 康复付费住院(康复期) 0：否 1：是
        /// </summary>
        public string heal_fee_flag
        {
            get { return NWB_ADM_TYPE; }
            set { NWB_ADM_TYPE = value; }
        }
        #endregion

        /// <summary>
        /// 是否市内转院：0：否 1：是
        /// </summary>
        public string CITY_TRANSFER_FLAG { get; set; }
        /// <summary>
        /// 市内转院转诊理由
        /// </summary>
        public string CITY_TRANSFER_REASON { get; set; }
        /// <summary>
        /// 转往医院编码
        /// </summary>
        public string TARGET_HOSPITAL_CODE { get; set; }
        /// <summary>
        /// 转往医院名称
        /// </summary>
        public string TARGET_HOSPITAL_NAME { get; set; }
        #region 方法
        #endregion
    }
}
