using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BP.InPatient
{
    public class QueryManager
    {
        /// <summary>
        /// 住院就诊查询
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable QueryInMainInfo(string patientId, DateTime startDate, DateTime endDate)
        {
            BL.InPatient.InMainInfoLogic queryInMainInfo = new BL.InPatient.InMainInfoLogic();
            return queryInMainInfo.QueryInMainInfo(patientId, startDate, endDate);
        }
        /// <summary>
        /// 预约金记录查询
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="admissionNo"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable QueryInPrepay(string patientId, string admissionNo, DateTime startDate, DateTime endDate)
        {
            BL.InPatient.InPrepayLogic queryInPrepay = new BL.InPatient.InPrepayLogic();
            return queryInPrepay.QueryInPrepay(patientId, admissionNo, startDate, endDate);
        }

    }
}
