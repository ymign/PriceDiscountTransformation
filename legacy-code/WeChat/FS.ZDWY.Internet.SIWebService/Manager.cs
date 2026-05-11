using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FS.ZDWY.Internet.SIWebService
{
    public class Manager
    {
        /// <summary>
        /// hlht_省医保_入院登记后取业务信息
        /// </summary>
        /// <returns></returns>
        public string RYDJXX(string function_id,
            string bka895,
            string bka896,
            string akb020,
            string aka130,
            string bka891,
            string aae030,
            string aae031)
        {
            FS.ZDWY.Internet.SIWebService.Interface.AbstractService manger = new FS.ZDWY.Internet.SIWebService.Interface.Inpatient.rydjxx();
            string resxml = "";
            if(manger.CallService(ref resxml, new string[] { function_id, bka895, bka896, akb020, aka130, bka891, aae030, aae031 })<0)
            {
                throw new Exception(manger.ErrorMsg);
            }
            return resxml;
        }
        
        /// <summary>
        /// 2.3hlht_省医保_费用清单信息提取
        /// </summary>
        /// <returns></returns>
        public string FYQDXX(string function_id,
                string akb020,
                string aaz218,
                string Operate,
                string Secfalg,
                string aac002,
                string fromdate)
        {
            FS.ZDWY.Internet.SIWebService.Interface.AbstractService manger = new FS.ZDWY.Internet.SIWebService.Interface.Inpatient.fyqdxx();
            string resxml = "";
            if (manger.CallService(ref resxml, new string[] { function_id, akb020, aaz218, Operate, Secfalg, aac002, fromdate }) < 0)
            {
                throw new Exception(manger.ErrorMsg);
            }
            return resxml;
        }
    }
}