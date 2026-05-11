using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Models.HealthRecord
{
    /// <summary>
    /// 病种修改记录
    /// </summary>
    [Serializable]
    public class ICDScoreLog : NeuObject
    {
        /// <summary>
        /// 住院流水号
        /// </summary>
        private string inptient_no = "";
        /// <summary>
        /// 住院流水号
        /// </summary>
        public string Inptient_no
        {
            get { return inptient_no; }
            set { inptient_no = value; }
        }


        /// <summary>
        /// 诊断1
        /// </summary>
        private string icd10_1 = "";
        /// <summary>
        /// 诊断1
        /// </summary>
        public string Icd10_1
        {
            get { return icd10_1; }
            set { icd10_1 = value; }
        }

        /// <summary>
        /// 诊断1
        /// </summary>
        private string icd10_1_Name = "";
        /// <summary>
        /// 诊断1
        /// </summary>
        public string Icd10_1_Name
        {
            get { return icd10_1_Name; }
            set { icd10_1_Name = value; }
        }

        /// <summary>
        /// 诊断2
        /// </summary>
        private string icd10_2 = "";
        /// <summary>
        /// 诊断2
        /// </summary>
        public string Icd10_2
        {
            get { return icd10_2; }
            set { icd10_2 = value; }
        }

        /// <summary>
        /// 诊断2名称
        /// </summary>
        private string icd10_2_Name = "";
        /// <summary>
        /// 诊断2名称
        /// </summary>
        public string Icd10_2_Name
        {
            get { return icd10_2_Name; }
            set { icd10_2_Name = value; }
        }

        /// <summary>
        /// 手术码
        /// </summary>
        private string icd9 = "";
        /// <summary>
        /// 手术码
        /// </summary>
        public string Icd9
        {
            get { return icd9; }
            set { icd9 = value; }
        }

        /// <summary>
        /// 手术名称
        /// </summary>
        private string icd9_Nmae = "";
        /// <summary>
        /// 手术名称
        /// </summary>
        public string Icd9_Nmae
        {
            get { return icd9_Nmae; }
            set { icd9_Nmae = value; }
        }

        /// <summary>
        /// 参保险种
        /// </summary>
        private string si_type = "";
        /// <summary>
        /// 参保险种
        /// </summary>
        public string Si_type
        {
            get { return si_type; }
            set { si_type = value; }
        }

        /// <summary>
        /// 发生序号
        /// </summary>
        private int happenNo = 0;
        /// <summary>
        /// 发生序号
        /// </summary>
        public int HappenNo
        {
            get { return happenNo; }
            set { happenNo = value; }
        }

        /// <summary>
        /// 操作员
        /// </summary>
        private string oper_code = "";
        /// <summary>
        /// 操作员
        /// </summary>
        public string Oper_code
        {
            get { return oper_code; }
            set { oper_code = value; }
        }

        /// <summary>
        /// 操作科室
        /// </summary>
        private string oper_dept = "";
        /// <summary>
        /// 操作科室
        /// </summary>
        public string Oper_dept
        {
            get { return oper_dept; }
            set { oper_dept = value; }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime oper_date = DateTime.Now;
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Oper_date
        {
            get { return oper_date; }
            set { oper_date = value; }
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        private OperType oper_type = OperType.CREATE;
        /// <summary>
        /// 操作类型
        /// </summary>
        public OperType Oper_type
        {
            get { return oper_type; }
            set { oper_type = value; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string mark = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark
        {
            get { return mark; }
            set { mark = value; }
        }

        //Diagnose DiagnoseClone = (Diagnose)base.Clone(); 
        public ICDScoreLog Clone()
        {
            return this.MemberwiseClone() as ICDScoreLog;
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public enum OperType
        {
            CREATE,
            UPDATE,
            UPLOAD
        }
    }


}
