using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FS.ZDWY.Internet.BP.InPatient
{
    public class InMainInfoManager
    {
        public List<FS.ZDWY.Internet.Models.FIN_IPR_INMAININFO> QueryInMainInfoList(string patientId, string admissionNo,string visitno, string certifcateNo, string name)
        {
            FS.ZDWY.Internet.BL.InPatient.InMainInfoLogic logic = new FS.ZDWY.Internet.BL.InPatient.InMainInfoLogic();
            return logic.QueryInMainInfoList(patientId, admissionNo, visitno, certifcateNo, name);
        }
    }
}
