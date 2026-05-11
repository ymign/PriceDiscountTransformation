using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Models.RADT
{
    /// <summary>
    /// 出生医学证明父母身份信息
    /// </summary>
    [System.Serializable]
   public class ParentsInfo : NeuObject
    {
       public string MomName { get; set; }

       public string MomSex { get; set; }

       public string MomNation { get; set; }

       public string MomBirthDay { get; set; }

       public string MomAddress { get; set; }

       public string MomIDNo { get; set; }

       /// <summary>
       /// 发证机关
       /// </summary>
       public string MomAgncy { get; set; }

       /// <summary>
       /// 有效期
       /// </summary>
       public string MomEndDate { get; set; }

       public string DadName { get; set; }

       public string DadSex { get; set; }

       public string DadNation { get; set; }

       public string DadBirthDay { get; set; }

       public string DadAddress { get; set; }

       public string DadIDNo { get; set; }

       /// <summary>
       /// 发证机关
       /// </summary>
       public string DadAgncy { get; set; }

       /// <summary>
       /// 有效期
       /// </summary>
       public string DadEndDate { get; set; }

        /// <summary>
		/// 构造函数
		/// </summary>
       public ParentsInfo()
		{
		}
    }
}
