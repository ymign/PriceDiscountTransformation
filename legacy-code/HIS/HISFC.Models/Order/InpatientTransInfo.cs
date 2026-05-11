using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Order
{

    public class InpatientTransInfo
    {
        private string id;
        /// <summary>
        /// 键
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }


        private string transtype;
        /// <summary>
        /// 转诊类别（0:上转；1:下转）
        /// </summary>
        public string Transtype
        {
            get
            {
                return transtype;
            }
            set
            {
                transtype = value;
            }
        }


        private string inpatientno;
        /// <summary>
        /// 住院流水号
        /// </summary>
        public string Inpatientno
        {
            get
            {
                return inpatientno;
            }
            set
            {
                inpatientno = value;
            }
        }


        private string patientno;
        /// <summary>
        /// 住院号
        /// </summary>
        public string Patientno
        {
            get
            {
                return patientno;
            }
            set
            {
                patientno = value;
            }
        }


        private string name;
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private string sex;
        public string Sex
        {
            get
            {
                return sex;
            }
            set
            {
                sex = value;
            }
        }

        private DateTime birthday;
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime Birthday
        {
            get
            {
                return birthday;
            }
            set
            {
                birthday = value;
            }
        }

        private string age;
        /// <summary>
        /// 年龄
        /// </summary>
        public string Age
        {
            get
            {
                return age;
            }
            set
            {
                age = value;
            }
        }


        private string tel;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel
        {
            get
            {
                return tel;
            }
            set
            {
                tel = value;
            }
        }


        private string diagcode;
        /// <summary>
        /// 出院诊断icd编码
        /// </summary>
        public string Diagcode
        {
            get
            {
                return diagcode;
            }
            set
            {
                diagcode = value;
            }
        }


        private string diagname;
        /// <summary>
        /// 出院诊断
        /// </summary>
        public string Diagname
        {
            get
            {
                return diagname;
            }
            set
            {
                diagname = value;
            }
        }


        private string deptcode;
        /// <summary>
        /// 转出科室编码
        /// </summary>
        public string Deptcode
        {
            get
            {
                return deptcode;
            }
            set
            {
                deptcode = value;
            }
        }


        private string deptname;
        /// <summary>
        /// 转出科室名称
        /// </summary>
        public string Deptname
        {
            get
            {
                return deptname;
            }
            set
            {
                deptname = value;
            }
        }


        private string indept;
        /// <summary>
        /// 下转单位名称
        /// </summary>
        public string Indept
        {
            get
            {
                return indept;
            }
            set
            {
                indept = value;
            }
        }


        private string addr;
        /// <summary>
        /// 患者住址
        /// </summary>
        public string Addr
        {
            get
            {
                return addr;
            }
            set
            {
                addr = value;
            }
        }


        private DateTime outdate;
        /// <summary>
        /// 转出日期
        /// </summary>
        public DateTime Outdate
        {
            get
            {
                return outdate;
            }
            set
            {
                outdate = value;
            }
        }

        private string note;
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                note = value;
            }
        }


        private string ext1;
        /// <summary>
        /// 拓展字段1
        /// </summary>
        public string Ext1
        {
            get
            {
                return ext1;
            }
            set
            {
                ext1 = value;
            }
        }

        private string ext2;
        /// <summary>
        /// 拓展字段2
        /// </summary>
        public string Ext2
        {
            get
            {
                return ext2;
            }
            set
            {
                ext2 = value;
            }
        }

        private string opercode;
        /// <summary>
        /// 操作人
        /// </summary>
        public string Opercode
        {
            get
            {
                return opercode;
            }
            set
            {
                opercode = value;
            }
        }


        private DateTime operdate;
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Operdate
        {
            get
            {
                return operdate;
            }
            set
            {
                operdate = value;
            }
        }


    }

}
