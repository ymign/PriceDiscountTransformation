using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.Components.InpatientFee.Controls
{
    public partial class tvPatientListForQuitFee : Neusoft.HISFC.Components.Common.Controls.tvPatientList
    {
        public tvPatientListForQuitFee()
        {
            InitializeComponent();
            #region {7655A89B-5996-4651-BAB4-62B53AACA6CF}
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv")
            {
                return;
            }
            #endregion
            this.Refresh();
        }

        public tvPatientListForQuitFee(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public string ShowType = "In";

        public string ShowDept = "1";

        Neusoft.HISFC.BizProcess.Integrate.RADT manager = null;
        Neusoft.HISFC.BizLogic.RADT.InPatient radtManager = new Neusoft.HISFC.BizLogic.RADT.InPatient();

        private ArrayList depts = null;
        private ArrayList GetDepts(string nurseCode)
        {
            if (depts == null)
            {
                Neusoft.HISFC.BizProcess.Integrate.Manager m = new Neusoft.HISFC.BizProcess.Integrate.Manager();
                depts = m.QueryDepartment(nurseCode);

            }
            return depts;
        }

        public new void Refresh()
        {
            this.BeginUpdate();
            this.Nodes.Clear();
            if (manager == null)
                manager = new Neusoft.HISFC.BizProcess.Integrate.RADT();

            ArrayList al = new ArrayList();//患者列表

            addPatientList(al);

            //显示所有患者列表
            this.SetPatient(al);

            this.EndUpdate();

        }

        /// <summary>
        /// 根据病区站得到欠费患者
        /// </summary>
        /// <param name="al"></param>
        private void addPatientList(ArrayList al)
        {
            Neusoft.HISFC.Models.Base.Employee employee = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;
             ArrayList alDept = this.GetDepts(employee.Nurse.ID);

             ArrayList al1 = new ArrayList();
             foreach (Neusoft.FrameWork.Models.NeuObject objDept in alDept)
             {
                 string deptName = objDept.Name;
                 string deptCode = objDept.ID;
                 al1.Clear();
                 al1 = this.radtManager.QueryQuitFeePatientByNurseCell(deptCode);
                 if (al1 != null && al1.Count > 0)
                 {
                     if (ShowDept == "1")//显示科室 
                     {
                         //应该显示患者科室名称
                         Neusoft.HISFC.Models.RADT.PatientInfo p = al1[0] as Neusoft.HISFC.Models.RADT.PatientInfo;
                         al.Add(p.PVisit.PatientLocation.Dept.Name);

                     }
                     al.AddRange(al1);
                 }
             }
        }

    }
}
