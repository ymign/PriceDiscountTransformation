using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models.Views
{
    /// <summary>
    /// 通用返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComResult<T> : BaseResult
    {
        public T ReturnData { get; set; }
    }
}
