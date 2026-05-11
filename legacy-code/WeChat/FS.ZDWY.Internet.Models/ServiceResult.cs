using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models
{
    public class ServiceResult
    {
        public bool Status { get; set; } 
        public string Message { get; set; } // 状态消息
        //public Exception Exception { get; set; } // 异常信息
         

        public ServiceResult()
        {
            Status = true;
            Message = "Success";
        }

        public ServiceResult(Exception exception)
        {
            Status = false;
            Message = exception.Message;
            //Exception = exception;
        }

        public ServiceResult(bool status, string message, Exception exception = null)
        {
            Status = status;
            Message = message;
            //Exception = exception;
        }
    }
}
