using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Integrate.AccountFee
{
    /// <summary>
    /// 
    /// </summary>
    public class Function
    {
        /// <summary>
        /// 临时增加指定最小费用项目不受执行科室限定
        /// 参数 800010 控制
        /// </summary>
        static List<string> lstFeeCodeMustExcu = null;
        /// <summary>
        /// 不支持终端扣费的合同单位
        /// 参数 800012 控制
        /// </summary>
        static List<string> lstUnTerminalPactCode = null;
        /// <summary>
        /// 增加指定最小费用项目不受执行科室限定
        /// 参数 800010 控制
        /// </summary>
        public static List<string> LstFeeCodeMustExcu
        {
            get
            {
                if (lstFeeCodeMustExcu == null)
                {
                    Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam ctlParam = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                    string strFeeCode = ctlParam.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.AccountConstant.MustExcuAccountFee);
                    if (string.IsNullOrEmpty(strFeeCode))
                    {
                        lstFeeCodeMustExcu = new List<string>();
                    }
                    else
                    {
                        lstFeeCodeMustExcu = new List<string>();
                        lstFeeCodeMustExcu.AddRange(strFeeCode.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
                return lstFeeCodeMustExcu;
            }
        }
        /// <summary>
        /// 不支持终端扣费的合同单位
        /// 参数 800012 控制
        /// </summary>
        public static List<string> LstUnTerminalPactCode
        {
            get
            {
                if (lstUnTerminalPactCode == null)
                {
                    Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam ctlParam = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                    string strCode = ctlParam.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.AccountConstant.UnTerminalPactCode);
                    if (string.IsNullOrEmpty(strCode))
                    {
                        lstUnTerminalPactCode = new List<string>();
                    }
                    else
                    {
                        lstUnTerminalPactCode = new List<string>();
                        lstUnTerminalPactCode.AddRange(strCode.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
                return lstUnTerminalPactCode;
            }
        }
        /// <summary>
        /// 医院编码- 医保上传编码
        /// </summary>
        static string strHospitalCode = null;
        /// <summary>
        /// 医院编码
        /// </summary>
        public static string HospitalCode
        {
            get
            {
                if (string.IsNullOrEmpty(strHospitalCode))
                {
                    Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam ctlParam = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                    strHospitalCode = ctlParam.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.Const.HosCode);
                }
                return strHospitalCode;
            }
        }
    }
}
