using FS.ZDWY.Internet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace FS.ZDWY.Internet.WebService
{
    /// <summary>
    /// HISQuitFee 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class HISQuitFee : System.Web.Services.WebService
    {

        [WebMethod(Description = "HIS调用门诊退费")]
        public ServiceResult QuitRegFeeByClinicCode(string url, string clinicCode)
        {
            BP.OutPatient.RegisterInfoManager mgr = new FS.ZDWY.Internet.BP.OutPatient.RegisterInfoManager();
            return mgr.QuitRegFeeByClinicCode(url, clinicCode);
        }
    }
}
