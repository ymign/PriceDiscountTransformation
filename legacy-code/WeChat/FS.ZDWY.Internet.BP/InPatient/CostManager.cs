using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.ZDWY.Internet.BP.InPatient
{
    /// <summary>
    /// 住院费用管理类
    /// </summary>
    public class CostManager
    {
        public DataTable QueryInMainDayFeeIn(string patientID, string inpatientNO,string visitno,string idenNO, DateTime startDate, DateTime endTime)
        {
            FS.ZDWY.Internet.BL.InPatient.InMainInfoLogic logic = new BL.InPatient.InMainInfoLogic();
            return logic.QueryInMainDayFeeIn(patientID, inpatientNO, visitno,idenNO, startDate, endTime);
        }

        public DataTable QueryInMainDayFeeALL(string patientID, string inpatientNO, string visitno, string idenNO, DateTime startDate, DateTime endTime)
        {
            FS.ZDWY.Internet.BL.InPatient.InMainInfoLogic logic = new BL.InPatient.InMainInfoLogic();
            return logic.QueryInMainDayFeeALL(patientID, inpatientNO, visitno, idenNO, startDate, endTime);
        }

        public DataTable QueryInMainDayFeeOut(string patientID, string inpatientNO, string visitno, string idenNO, DateTime startDate, DateTime endTime)
        {
            FS.ZDWY.Internet.BL.InPatient.InMainInfoLogic logic = new BL.InPatient.InMainInfoLogic();
            return logic.QueryInMainDayFeeOut(patientID, inpatientNO, visitno, idenNO, startDate, endTime);
        }

        public DataTable QueryInMainfoByPatients(string patientIDS, string inpatientNOs,string visitNumber, string idenNo, DateTime startDate, DateTime endDate)
        {
            FS.ZDWY.Internet.BL.InPatient.InMainInfoLogic logic = new BL.InPatient.InMainInfoLogic();
            return logic.QueryInMainfoByPatients(patientIDS, inpatientNOs, visitNumber, idenNo, startDate, endDate);
        }

        public DataTable QueryInMainInfoDetail(string inState, string patientID, string inpatientNO, string visitno,string idenNo, DateTime startDate, DateTime endDate)
        {
            FS.ZDWY.Internet.BL.InPatient.InMainInfoLogic logic = new BL.InPatient.InMainInfoLogic();
            return logic.QueryInMainInfoDetail(inState, patientID, inpatientNO, visitno, idenNo, startDate, endDate);
        }

        public DataTable QueryOutSummay(string visitNo, string inpatientNO)
        {
            FS.ZDWY.Internet.BL.InPatient.InMainInfoLogic logic = new BL.InPatient.InMainInfoLogic();
            return logic.QueryOutSummay(visitNo, inpatientNO);
        }
    }
}
