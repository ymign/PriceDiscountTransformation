using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.Endoscope
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

        //private PatientInfo patientinfo = new PatientInfo();
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

    public class Result<T>
    {
        private T exam;
        public T ExamApply
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

    public class Return<T>
    {
        private string code=string.Empty;
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

        private string errMsg=string.Empty;
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

        private Result<T> result = new Result<T>();
        public Result<T> Result
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


    public class DataSource<T>
    {
        private Return<T> ret = new Return<T>();
        public Return<T> Return
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
