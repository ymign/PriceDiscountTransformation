using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Integrate.Operation
{
    /// <summary>
    /// [功能描述: 麻醉安排业务层]<br></br>
    /// [创 建 者: 王铁全]<br></br>
    /// [创建时间: 2006-12-31]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public class AnaeRecord : Neusoft.HISFC.BizLogic.Operation.AnaeRecord
    {
        #region 业务层变量

        private Neusoft.HISFC.BizLogic.Registration.Register regManager = new Neusoft.HISFC.BizLogic.Registration.Register();
        private Neusoft.HISFC.BizProcess.Integrate.Registration.Registration regMgr = new Neusoft.HISFC.BizProcess.Integrate.Registration.Registration();
        private Neusoft.HISFC.BizLogic.RADT.InPatient inPatientManager = new Neusoft.HISFC.BizLogic.RADT.InPatient();
        private Neusoft.HISFC.BizProcess.Integrate.Manager manager = new Manager();
        private Neusoft.HISFC.BizProcess.Integrate.RADT radtManager = new RADT();

        private Neusoft.HISFC.BizLogic.Operation.OpsDiagnose diagMgr = new Neusoft.HISFC.BizLogic.Operation.OpsDiagnose();

        #endregion

        #region 字段
        private Operation operation = new Operation();
        #endregion

        #region 属性
        protected override Neusoft.HISFC.BizLogic.Operation.Operation operationManager
        {
            get
            {
                return this.operation;
            }
        }
        #endregion

        #region 方法

        protected override Neusoft.HISFC.Models.RADT.PatientInfo GetPatientInfo(string id)
        {
            return this.radtManager.GetPatientInfomation(id);
        }
        protected override Neusoft.HISFC.Models.Registration.Register GetRegInfo(string id)
        {
            ArrayList alreg = this.regMgr.QueryPatient(id);
            return alreg[0] as Neusoft.HISFC.Models.Registration.Register;
        }

        protected override string GetEmployeeName(string id)
        {
            return this.manager.GetEmployeeInfo(id).Name;
        }

        /// <summary>
        /// 根据手术序号获得手术诊断信息列表
        /// </summary>
        /// <param name="OperatorNo">手术申请单对象</param>
        /// <returns>患者的手术诊断对象数组</returns>
        public override ArrayList GetIcdFromApp(Neusoft.HISFC.Models.Operation.OperationAppllication opsApp)
        {
            ArrayList IcdAl = new ArrayList();
            ArrayList rtnAl = new ArrayList();

            //患者住院流水号strInPatientNo			
            switch (opsApp.PatientSouce)
            {
                case "1"://门诊手术
                    string strInPatientNo1 = string.Empty;//患者住院流水号 
                    strInPatientNo1 = opsApp.PatientInfo.ID.ToString();
                    try
                    {
                        //TODO:病案业务层
                        IcdAl = diagMgr.QueryOpsDiagnose(strInPatientNo1, "7");//"7"为术前诊断类型
                        foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase diag in IcdAl)
                        {
                            if (diag.OperationNo == opsApp.ID)
                                rtnAl.Add(diag);
                        }
                    }
                    catch
                    {
                        return rtnAl;
                    }
                    break;
                    break;
                case "2"://住院手术
                    string strInPatientNo = string.Empty;//患者住院流水号 
                    strInPatientNo = opsApp.PatientInfo.ID.ToString();
                    try
                    {
                        //TODO:病案业务层
                        IcdAl = diagMgr.QueryOpsDiagnose(strInPatientNo, "7");//"7"为术前诊断类型
                        foreach (Neusoft.HISFC.Models.HealthRecord.DiagnoseBase diag in IcdAl)
                        {
                            if (diag.OperationNo == opsApp.ID)
                                rtnAl.Add(diag);
                        }
                    }
                    catch
                    {
                        return rtnAl;
                    }
                    break;
            }
            return rtnAl;
        }
        
        #endregion
    }
}
