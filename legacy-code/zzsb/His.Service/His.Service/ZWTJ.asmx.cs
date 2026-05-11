using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using His.Models.Common;
using His.Models.ZWTJ;
using His.Util.Common;

namespace His.Service
{
    /// <summary>
    /// ZWTJ1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class ZWTJ1 : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "体检挂号")]
        public string PERegisterInfo(string xml)
        {
            His.Business.ZWTJ.PERegisterInfo tnf = new His.Business.ZWTJ.PERegisterInfo();
            //string err = string.Empty;
            return tnf.TjPatientNo(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "血透儿保挂号")]
        public string PERegisterInfoxt(string xml)
        {
            His.Business.ZWTJ.PERegisterInfoxt tnf = new His.Business.ZWTJ.PERegisterInfoxt();
            //string err = string.Empty;
            return tnf.TjPatientNo(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "作废项目")]
        public string CancelPEPatFee(string xml)
        {
            His.Business.ZWTJ.CancelPEPatFee tnf = new His.Business.ZWTJ.CancelPEPatFee();
            //string err = string.Empty;
            return tnf.TjCancelPEPatFee(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "确认收费项目")]
        public string ComfirmPEPatFee(string xml)
        {
            His.Business.ZWTJ.ComfirmPEPatFee tnf = new His.Business.ZWTJ.ComfirmPEPatFee();
            //string err = string.Empty;
            return tnf.TjComfirmPEPatFee(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "体检挂号信息变更")]
        public string PEPatInfoChange(string xml)
        {
            His.Business.ZWTJ.PEPatInfoChange tnf = new His.Business.ZWTJ.PEPatInfoChange();
            //string err = string.Empty;
            return tnf.TjPEPatInfoChange(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "挂号收费")]
        public string GetPEPatFee(string xml)
        {
            His.Business.ZWTJ.GetPEPatFee tnf = new His.Business.ZWTJ.GetPEPatFee();
            //string err = string.Empty;
            return tnf.TjGetPEPatFee(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "体检项目字典")]
        public string PeProjectDict(string xml)
        {
            His.Business.ZWTJ.PeProjectDict tnf = new His.Business.ZWTJ.PeProjectDict();
            //string err = string.Empty;
            return tnf.GetPeProjectDict(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "体检组套项目字典")]
        public string PeZtDictInfo(string xml)
        {
            His.Business.ZWTJ.PeZtDictInfo tnf = new His.Business.ZWTJ.PeZtDictInfo();
            //string err = string.Empty;
            return tnf.GetPeZtDictInfo(xml);
        }

        [WebMethod(EnableSession = true, BufferResponse = true, Description = "体检收费项目明细")]
        public string PeChargeitemDeail(string xml)
        {
            His.Business.ZWTJ.PeChargeitemDeail tnf = new His.Business.ZWTJ.PeChargeitemDeail();
            //string err = string.Empty;
            return tnf.GetPeChargeitemDeail(xml);
        }
    }
}
