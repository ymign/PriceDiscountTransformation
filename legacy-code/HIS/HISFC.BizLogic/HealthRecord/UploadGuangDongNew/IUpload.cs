using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Neusoft.HISFC.BizLogic.HealthRecord.UploadGuangDongNew
{
    public interface IUpload
    {
        int GetIsHavedNoUpload(string fprn, DateTime in_date);

        int GetIsNeedUpload(string fprn, string fzyid);

        int GetInTimes(string prn, string fzyid, int itype, ref string inTimes);

        Neusoft.HISFC.Models.RADT.PatientInfo GetPatientFromBA(string cardNO);

        int DeleteHISBA1ByFzyid(string inpatientNO);

        int DeleteHISBA1ByFzyid(string patientNO,int intimes);

        int InsertPatientInfoBA1Drgs(Neusoft.HISFC.Models.HealthRecord.Base b, DataSet Feeds,
            System.Collections.ArrayList alChangeDepe, System.Collections.ArrayList alDose, bool isMetCasBase);

        int UpdateHISBA1Fzkdate(string fprn);

        int DeleteHISBA2(string inpatientNO, int times);

        int InsertHISBA2(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.RADT.Location obj);

        int DeleteHISBA3(string inpatientNO, int times);

        int InsertHISBA3Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Diagnose obj);

        int DeleteHISBA4(string inpatientNO, int times);

        int insertHisBa4Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.OperationDetail obj);

        int DeleteHISBA5(string inpatientNO, int times);

        int insertHisBa5Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Baby obj);

        int DeleteHISBA6(string inpatientNO, int times);

        int InsertHISBA6Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.Tumour obj);

        int UpdateHISBA6FYRQ(string FPRN);

        int UpdateHISBA6FQRQ(string FPRN);

        int UpdateHISBA6FZRQ(string FPRN);

        int DeleteHISBA7(string inpatientNO, int times);

        int InsertHISBA7Drgs(Neusoft.HISFC.Models.HealthRecord.Base patientInfo, Neusoft.HISFC.Models.HealthRecord.TumourDetail obj);

        /// <summary>
        /// 获取上传错误信息
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        int GetUploadErro(string inpatientNO, int times);
    }
}
