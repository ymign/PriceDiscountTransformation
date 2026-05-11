using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.MedicalTraceCode
{
    public static class YesNoEnum
    {
        public const string No = "0";
        public const string Yes = "1";

        public static string GetDescription(string code)
        {
            switch (code)
            {
                case Yes: return "是";
                case No: return "否";
                default: return "未知";
            }
        }

        public static bool IsValid(string code)
        {
            return code == Yes || code == No;
        }
    }
}
