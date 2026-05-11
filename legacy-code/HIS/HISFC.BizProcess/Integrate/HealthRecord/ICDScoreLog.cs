using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Integrate.HealthRecord
{
    public class ICDScoreLog : IntegrateBase
    {
        protected Neusoft.HISFC.BizLogic.HealthRecord.ICDScoreLog icdscoreMgr = new Neusoft.HISFC.BizLogic.HealthRecord.ICDScoreLog();

        public override void SetTrans(System.Data.IDbTransaction trans)
        {
            this.trans = trans;
            icdscoreMgr.SetTrans(trans);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int UpdateICDScoreLog(Neusoft.HISFC.Models.HealthRecord.ICDScoreLog obj)
        {
            this.SetDB(icdscoreMgr);
            int hanppenNo = icdscoreMgr.GetHappenNo(obj.Inptient_no);
            if (hanppenNo < 0)
            {
                this.Err = "获取发生序号错误!";
                return -1;
            }
            Neusoft.HISFC.Models.HealthRecord.ICDScoreLog lastObj = icdscoreMgr.GetLastICDScoreLog(obj.Inptient_no);
            if (lastObj == null || string.IsNullOrEmpty(lastObj.Inptient_no))
            {
                obj.Oper_code = Neusoft.FrameWork.Management.Connection.Operator.ID;
                obj.Oper_dept = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID;
                obj.Oper_date = icdscoreMgr.GetDateTimeFromSysDateTime();
                obj.HappenNo = hanppenNo;
                return icdscoreMgr.CreateICDScoreLog(obj);
            }
            else
            {
                obj.Oper_code = Neusoft.FrameWork.Management.Connection.Operator.ID;
                obj.Oper_dept = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID;
                obj.Oper_date = icdscoreMgr.GetDateTimeFromSysDateTime();
                obj.HappenNo = hanppenNo;
                return icdscoreMgr.UpdateICDScoreLog(obj);
            }
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int UpLodaICDScoreLog(Neusoft.HISFC.Models.HealthRecord.ICDScoreLog obj)
        {
            this.SetDB(icdscoreMgr);
            int hanppenNo = icdscoreMgr.GetHappenNo(obj.Inptient_no);
            if (hanppenNo < 0)
            {
                this.Err = "获取发生序号错误!";
                return -1;
            }
            obj.Oper_code = Neusoft.FrameWork.Management.Connection.Operator.ID;
            obj.Oper_dept = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).Dept.ID;
            obj.Oper_date = icdscoreMgr.GetDateTimeFromSysDateTime();
            obj.HappenNo = hanppenNo;
            return icdscoreMgr.UpLodaICDScoreLog(obj);
        }
    }
}
