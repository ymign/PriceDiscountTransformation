using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

namespace His.Service
{
    /// <summary>
    /// CommonServiceForHIS 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class CommonServiceForHIS : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "HIS调用，通过住院患者流水号列表调用Web服务，推送住院患者检验申请单")]
        public int PushLisInpatientApplyByInpatientNoList(System.Collections.Generic.List<string> inpatientNoList)
        {
            His.Business.LIS.InPatientApply ipa = new His.Business.LIS.InPatientApply();
            return ipa.PushLisInpatientApplyByInpatientNoList(inpatientNoList);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "HIS调用，推送患者信息，并获取插入EMPI号")]
        public int PushPatientEMPIInfo(string patientNo,string patientType)
        {
            His.Business.EMPI.EMPI empi = new His.Business.EMPI.EMPI();
            return empi.GetAndSetPatientEMPI(patientNo, patientType);
        }
    }
}
