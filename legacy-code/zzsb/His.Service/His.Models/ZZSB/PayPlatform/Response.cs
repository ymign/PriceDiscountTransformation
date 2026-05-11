using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB.PayPlatform
{
    public class Response<T> where T : class
    {
        public string Code { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }
    }
}
