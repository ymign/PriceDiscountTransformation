using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.BUltrasonic
{
    public class ExamApply
    {
        private List<ApplyBill> applyInfo = new List<ApplyBill>();
        public List<ApplyBill> APPLYINFO
        {
            get
            {
                return applyInfo;
            }
            set
            {
                applyInfo = value;
            }
        }

        //private PatientInfo patientinfo=new PatientInfo();
        //public PatientInfo PATIENTINFO
        //{
        //    get
        //    {
        //        return patientinfo;
        //    }
        //    set
        //    {
        //        patientinfo = value;
        //    }
        //}

        //private List<ApplyChargeInfo> feeinfo = new List<ApplyChargeInfo>();
        //public List<ApplyChargeInfo> FEEINFO
        //{
        //    get
        //    {
        //        return feeinfo;
        //    }
        //    set
        //    {
        //        feeinfo = value;
        //    }
        //}

    }

    public class Result
    {
        private ExamApply exam = new ExamApply();
        public ExamApply ExamApply
        {
            get
            {
                return exam;
            }
            set
            {
                exam = value;
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

        private Result result = new Result();
        public Result Result
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
