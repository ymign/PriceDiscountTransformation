using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BP.Common
{
    public static class Function
    {
        /// <summary>
        /// 性别转换
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ConvertHISSexCode(string code)
        {
            switch (code)
            {
                case "1": return "M";
                case "2": return "F";
                case "9": return "U";
                default: return "U";
            }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        /// <param name="paytype"></param>
        /// <param name="patient"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string SetPayType(string paytype)
        {
            if (paytype == "3")
            {
                return "PTYL";
            }
            else if (paytype == "4")
            {
                //珠海医保
                return "PTYBK";
            }
            else if (paytype == "2")
            {
                //支付宝
                return "PTZFB";
            }
            else if (paytype == "1")
            {
                //微信
                return "PTWX";
            }
            else if (paytype == "6")
            {
                //医保信用付
                return "YBXYF";
            }
            else if (paytype == "7")
            {
                //长者券
                return "XGZZQ";
            }
            else
            {
                throw new Exception("未知支付方式！");
            }

        }

        #region 默认操作人信息
        static Models.OperInfo oper;
        public static Models.OperInfo DefaultOper
        {
            get
            {
                oper = new Models.OperInfo()
                {
                    Code = "00A105",// Platfo
                    Name = "新微信"
                };
                return oper;
            }
        }

        /// <summary>
        /// 支付宝操作人员信息
        /// </summary>
        public static Models.OperInfo ZFBOper
        {
            get
            {
                oper = new Models.OperInfo()
                {
                    Code = "00A106",// Platfo
                    Name = "平台支付宝"
                };
                return oper;
            }
        }
        
        /// <summary>
        /// 手机APP操作人员信息
        /// </summary>
        public static Models.OperInfo APPOper
        {
            get
            {
                oper = new Models.OperInfo()
                {
                    Code = "00A107",// Platfo
                    Name = "平台手机APP"
                };
                return oper;
            }
        }
        #endregion
    }
}
