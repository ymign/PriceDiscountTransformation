using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FS.ZDWY.Internet.BP.Doctor
{
    public class DoctorManager
    {
        public DataTable QueryDoctorList(string deptCode,string doctorCode)
        {
            FS.ZDWY.Internet.BL.Doctor.DoctorLogic doctorLogic = new BL.Doctor.DoctorLogic();
            return doctorLogic.QueryDoctorList(deptCode, doctorCode);
        }
       
    }
}
