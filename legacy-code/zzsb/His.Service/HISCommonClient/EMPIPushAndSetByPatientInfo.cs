using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HISCommonClient
{
    public class EMPIPushAndSetByPatientInfo
    {
        WebReference1.CommonServiceForHIS commonServiceForHIS = new HISCommonClient.WebReference1.CommonServiceForHIS();
        public int PushAndSetByPatientEMPI(string patientNo, string patientType)
        {
            int i= commonServiceForHIS.PushPatientEMPIInfo(patientNo, patientType);
            return i;
        }
    }
}
