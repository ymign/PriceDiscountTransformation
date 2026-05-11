using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Account
{
    public interface IOperCard
    {
       
        /// <summary>
        /// 读取卡面号 0为读取成功 -1失败
        /// </summary>
        /// <param name="MCardNO"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        int ReadMCardNO(ref string MCardNO,ref string Message);

       /// <summary>
       /// 读取病历号0为读取成功 -1失败
       /// </summary>
       /// <param name="CardNO"></param>
       /// <param name="Message"></param>
       /// <returns></returns>
        int ReadCardNO(ref string CardNO, ref string Message);

        /// <summary>
        /// 写入病历号 0为成功 -1失败
        /// </summary>
        /// <param name="CardNO"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        int WriterCardNO(string CardNO, ref string Message);
    }
}
