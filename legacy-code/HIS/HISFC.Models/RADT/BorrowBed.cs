using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.RADT
{
    public class BorrowBed
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
        /// 入院日期
        /// </summary>
        private System.DateTime IN_DATE;

        /// <summary>
        /// 所在科室代码
        /// </summary>
        private string DEPT_CODE;

        /// <summary>
        /// 所在科室名称
        /// </summary>
        private string DEPT_NAME;

        /// <summary>
        /// 性别
        /// </summary>
        private SexEnumService SEX_CODE;

        /// <summary>
        /// 病床来源科室代码
        /// </summary>
        private string FROM_DEPT_CODE;

        /// <summary>
        /// 病床来源科室名称
        /// </summary>
        private string FROM_DEPT_NAME;

        /// <summary>
        /// 病床编号
        /// </summary>
        private string FROM_BED_NO;

        /// <summary>
        /// 借床日期
        /// </summary>
        private System.DateTime BORROW_DATE;

        /// <summary>
        /// 归还日期
        /// </summary>
        private System.DateTime RETURN_DATE;

        /// <summary>
        /// 操作人工号
        /// </summary>
        private string OPER_CODE;

        /// <summary>
        /// 操作时间
        /// </summary>
        private System.DateTime OPER_DATE;




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
        /// 入院日期
        /// </summary>
        public System.DateTime in_date
        {
            get { return IN_DATE; }
            set { IN_DATE = value; }
        }
        /// <summary>
        /// 所在科室代码
        /// </summary>
        public string dept_code
        {
            get { return DEPT_CODE; }
            set { DEPT_CODE = value; }
        }
        /// <summary>
        /// 所在科室名称
        /// </summary>
        public string dept_name
        {
            get { return DEPT_NAME; }
            set { DEPT_NAME = value; }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public SexEnumService sex_code
        {
            get
            {
                if (SEX_CODE == null)
                {
                    SEX_CODE = new SexEnumService();
                }
                return this.SEX_CODE;
            }
            set
            {
                this.SEX_CODE = value;
            }
        }
        /// <summary>
        /// 病床来源科室代码
        /// </summary>
        public string from_dept_code
        {
            get { return FROM_DEPT_CODE; }
            set { FROM_DEPT_CODE = value; }
        }
        /// <summary>
        /// 病床来源科室名称
        /// </summary>
        public string from_dept_name
        {
            get { return FROM_DEPT_NAME; }
            set { FROM_DEPT_NAME = value; }
        }
        /// <summary>
        /// 病床编号
        /// </summary>
        public string from_bed_no
        {
            get { return FROM_BED_NO; }
            set { FROM_BED_NO = value; }
        }
        /// <summary>
        /// 借床日期
        /// </summary>
        public System.DateTime borrow_date
        {
            get { return BORROW_DATE; }
            set { BORROW_DATE = value; }
        }
        /// <summary>
        /// 归还日期
        /// </summary>
        public System.DateTime return_date
        {
            get { return RETURN_DATE; }
            set { RETURN_DATE = value; }
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
        /// 操作日期
        /// </summary>
        public System.DateTime oper_date
        {
            get { return OPER_DATE; }
            set { OPER_DATE = value; }
        }

        #endregion

        #region 方法
        #endregion
    }
}
