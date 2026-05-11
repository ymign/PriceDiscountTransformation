using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.EMPI
{
    public class EMPI
    {
        public int GetAndSetPatientEMPI(string patientNo, string patientType)
        {
            EMPIService.EMPIService.EMPIService es = new EMPIService.EMPIService.EMPIService();
            int i = es.PushEMPIPatientInfo(patientNo, patientType);
            return i;
        }
    }
}
