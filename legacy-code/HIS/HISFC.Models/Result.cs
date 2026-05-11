using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models
{
    public class Result<TOk, TErr>
    {
        public bool IsOk { get; set; }
        public bool IsErr { get { return !IsOk; } }
        public TOk OkValue { get; set; }
        public TErr ErrValue { get; set; }

        private Result(bool isOK, TOk value, TErr error)
        {
            if (typeof(TOk) == typeof(bool))//当TOk为bool类型时，为了使用的时候不用判断两遍 这么转换一下
            {
                IsOk = System.Convert.ToBoolean(value);
            }
            else
            {
                IsOk = isOK;
            }
            OkValue = value;
            ErrValue = error;
        }

        /// <summary>
        /// 这个无参构造函数必须添加 动态api接口的时候要是没有的话，会反序列化失败
        /// </summary>
        public Result()
        {
        }

        public static Result<TOk, TErr> Ok(TOk value)
        {
            return new Result<TOk, TErr>(true, value, default(TErr));
        }

        public static Result<TOk, TErr> Err(TErr error)
        {
            return new Result<TOk, TErr>(false, default(TOk), error);
        }

        public static implicit operator Result<TOk, TErr>(TErr error)
        {
            return Err(error);
        }

        public static implicit operator Result<TOk, TErr>(TOk value)
        {
            return Ok(value);
        }
    }
}
