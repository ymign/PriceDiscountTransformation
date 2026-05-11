using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Account
{
    /// <summary>
    /// 读取身份证信息接口
    /// </summary>
   public  interface IReadIDCard
    {
       /// <summary>
        /// 获取身份证信息
       /// </summary>
       /// <param name="IDCode">身份证</param>
       /// <param name="Name">姓名</param>
       /// <param name="Sex">性别</param>
       /// <param name="BirthDay">出生日期</param>
       /// <param name="Nation">民族</param>
       /// <param name="Adress">地址</param>   
        /// <param name="Agency">发证机关</param>
       /// <param name="ExpireStart">有效期起</param>
       /// <param name="ExpireEnd">有效期至</param>        
        /// <param name="Message">图片</param> 
       /// <returns></returns>
       int GetIDInfo(ref string IDCode, ref string Name, ref string Sex, ref DateTime BirthDay, ref string Nation, ref string Adress, ref string Agency, ref DateTime ExpireStart, ref DateTime ExpireEnd,ref string PhotoFileName, ref string Message);
    }
}
