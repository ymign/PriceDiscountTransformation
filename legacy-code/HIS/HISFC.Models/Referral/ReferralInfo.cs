using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Referral
{
    public class ReferralInfo
    {
        string clinicCode;
        /// <summary>
        /// 门诊号
        /// </summary>
        public string ClinicCode
        {
            get { return clinicCode; }
            set { clinicCode = value; }
        }

        string name;
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string sex;

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex
        {
            get { return sex; }
            set { sex = value; }
        }

        string age;
        /// <summary>
        /// 年龄
        /// </summary>
        public string Age
        {
            get { return age; }
            set { age = value; }
        }

        string deptName;
        /// <summary>
        /// 就诊科别名称
        /// </summary>
        public string DeptName
        {
            get { return deptName; }
            set { deptName = value; }
        }

        string deptCode;
        /// <summary>
        /// 就诊科别代码
        /// </summary>
        public string DeptCode
        {
            get { return deptCode; }
            set { deptCode = value; }
        }

        string cardNo;
        /// <summary>
        /// 就诊卡号
        /// </summary>
        public string CardNo
        {
            get { return cardNo; }
            set { cardNo = value; }
        }

        string phone;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        string diagnosis;
        /// <summary>
        /// 门诊诊断
        /// </summary>
        public string Diagnosis
        {
            get { return diagnosis; }
            set { diagnosis = value; }
        }

        string proposal;
        /// <summary>
        /// 建议
        /// </summary>
        public string Proposal
        {
            get { return proposal; }
            set { proposal = value; }
        }

        string unitName;
        /// <summary>
        /// 下转单位
        /// </summary>
        public string UnitName
        {
            get { return unitName; }
            set { unitName = value; }
        }

        string doctCode;
        /// <summary>
        /// 医生ID
        /// </summary>
        public string DoctCode
        {
            get { return doctCode; }
            set { doctCode = value; }
        }

        string doctName;
        /// <summary>
        /// 医生姓名
        /// </summary>
        public string DoctName
        {
            get { return doctName; }
            set { doctName = value; }
        }

        DateTime operDate;
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperDate
        {
            get { return operDate; }
            set { operDate = value; }
        }
    }
}
