using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.Models.Views
{
    /// <summary>
    /// 基本返回结果
    /// </summary>
    public class BaseResult
    {
        public bool IsSuccessful { get; set; }

        public string Message { get; set; }
    }
}
