using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace FS.ZDWY.Internet.Models
{
    ///<summary>
    ///预填写入院申请信息
    ///</summary>
    [SugarTable("INPATIENTREGIST")]
    public class InPatientRegistInfo
    {
        public InPatientRegistInfo()
        {


        }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 医保号
        /// </summary>
        public string SSN { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard { get; set; }

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 电脑号
        /// </summary>
        public string ProCreateNO { get; set; }

        /// <summary>
        /// 住院号
        /// </summary>
        public string PatientNO { get; set; }

        /// <summary>
        /// 门诊卡号
        /// </summary>
        public string CardNO { get; set; }

        /// <summary>
        /// 入院日期
        /// </summary>
        public DateTime? InTime { get; set; }

        /// <summary>
        /// 合同单位编码（字典）
        /// </summary>
        public string PactID { get; set; }

        /// <summary>
        /// 合同单位名称（字典）
        /// </summary>
        public string PactName { get; set; }

        /// <summary>
        /// 性别（字典）
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 民族（字典）
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 科室编码（字典）
        /// </summary>
        public string DeptID { get; set; }

        /// <summary>
        /// 工作单位
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 婚姻状况（字典）
        /// </summary>
        public string MaritalStatus { get; set; }

        /// <summary>
        /// 籍贯（字典）
        /// </summary>
        public string DIST { get; set; }

        /// <summary>
        /// 出生地
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 国籍ID（字典）
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 职位ID（字典）
        /// </summary>
        public string Profession { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string KinName { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string KinRelationPhone { get; set; }

        /// <summary>
        /// 与患者关系（字典）
        /// </summary>
        public string KinRelation { get; set; }

        /// <summary>
        /// 联系人地址
        /// </summary>
        public string KinRelationAddress { get; set; }

        /// <summary>
        /// 家庭地址邮编
        /// </summary>
        public string HomeZip { get; set; }

        /// <summary>
        /// 现住址（字典）
        /// </summary>
        public string NowAddr { get; set; }

        /// <summary>
        /// 现住址[街道}
        /// </summary>
        public string NowAdd { get; set; }

        /// <summary>
        /// 户口地址（字典）
        /// </summary>
        public string HomeAddr { get; set; }

        /// <summary>
        /// 户口地址{街道}
        /// </summary>
        public string HomeAdd { get; set; }

        /// <summary>
        /// 患者电话
        /// </summary>
        public string PhoneHome { get; set; }

        /// <summary>
        /// 单位电话
        /// </summary>
        public string PhoneBusiness { get; set; }

        /// <summary>
        /// 入院途径（字典）
        /// </summary>
        public string AdmitSource { get; set; }

        /// <summary>
        /// 入院来源（字典）
        /// </summary>
        public string InSource { get; set; }

        /// <summary>
        /// 入院情况（字典）
        /// </summary>
        public string Circs { get; set; }

        /// <summary>
        /// 收住医师（字典）
        /// </summary>
        public string DoctorReceiver { get; set; }

        /// <summary>
        /// 门诊诊断
        /// </summary>
        public string ClinicDiagnose { get; set; }

        /// <summary>
        /// 门诊诊断编码
        /// </summary>
        public string ClinicDiagnoseNo { get; set; }

        /// <summary>
        /// 是否日间手术标记（字典）
        /// </summary>
        public string DayOperationFlag { get; set; }

   
        public DateTime? OPERDATE { get; set; }
    }
}
