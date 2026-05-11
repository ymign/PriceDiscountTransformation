using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    class BookReponse
    {
    }



    public class Result
    {

        /// <summary>
        /// 科室编号
        /// </summary>
        private string deptCode = string.Empty;
        /// <summary>
        /// 科室编号
        /// </summary>
        public string DeptCode
        {
            get
            {
                return this.deptCode;
            }
            set
            {
                this.deptCode = value;
            }
        }


        /// <summary>
        /// 医生编号
        /// </summary>
        private string doctorCode = string.Empty;
        /// <summary>
        /// 医生编号
        /// </summary>
        public string DoctorCode
        {
            get
            {
                return this.doctorCode;
            }
            set
            {
                this.doctorCode = value;
            }
        }

        /// <summary>
        /// 总挂号费
        /// </summary>
        private decimal totalRegFee ;
        /// <summary>
        /// 总挂号费
        /// </summary>
        public decimal TotalRegFee
        {
            get
            {
                return this.totalRegFee;
            }
            set
            {
                this.totalRegFee = value;
            }
        }


        private string deptname = string.Empty;
        /// <summary>
        /// 科室名称
        /// </summary>
        public string DeptName
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

        private string doctname = string.Empty;
        /// <summary>
        /// 医生姓名
        /// </summary>
        public string doctorName
        {
            get
            {
                return doctname;
            }
            set
            {
                doctname = value;
            }
        }

        private string vistDate = string.Empty;
        /// <summary>
        /// 就诊日期
        /// </summary>
        public string VistDate
        {
            get
            {
                return vistDate;
            }
            set
            {
                vistDate = value;
            }
        }

        private string admtAddress = string.Empty;
        /// <summary>
        /// 就诊地址
        /// </summary>
        public string AdmitAddress
        {
            get
            {
                return admtAddress;
            }
            set
            {
                admtAddress = value;
            }
        }

        private string ordercode_ = string.Empty;
        /// <summary>
        /// 预约单号
        /// </summary>
        public string ordercode
        {
            get
            {
                return ordercode_;
            }
            set
            {
                ordercode_ = value;
            }
        }
    }

    public class Return
    {
        private string code = string.Empty;
        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }

        private string errMsg = string.Empty;
        public string ErrorMsg
        {
            get
            {
                return errMsg;
            }
            set
            {
                errMsg = value;
            }
        }

        private string fun_code=string.Empty;
        public string FunCode
        {
            get
            {
                return fun_code;
            }
            set
            {
                fun_code = value;
            }
        }

        private string opTime;
        public string OpTime
        {
            get
            {
                return opTime;
            }
            set
            {
                opTime = value;
            }
        }

        private List<Result> result = new List<Result>();
        public List<Result> Results
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }
    }


    public class DataSource
    {
        private Return ret = new Return();
        public Return Return
        {
            get
            {
                return ret;
            }
            set
            {
                ret = value;
            }
        }
    }
}

