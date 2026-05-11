using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;

namespace Neusoft.HISFC.Components.InpatientFee.Controls
{
    public partial class tvPatientListForPrepay : Neusoft.HISFC.Components.Common.Controls.tvPatientList
    {
        public tvPatientListForPrepay()
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

        public string ShowType = "In";

        public string ShowDept = "1";

        public tvPatientListForPrepay(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public override void Refresh()
        {
            //{707F2343-20AC-445b-9ACB-2B707C8EA249}
            InitControlParam();
            this.BeginUpdate();
            this.Nodes.Clear();
            if (manager == null)
                manager = new Neusoft.HISFC.BizProcess.Integrate.RADT();

            try
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("患者树加载中...", 0, false);
                System.Windows.Forms.Application.DoEvents();
                //var al = addPatientList();

                var al = GetPatientList();

                //显示所有患者列表
                this.SetPatient(al);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }


            this.EndUpdate();
            this.CollapseAll();
            //this.Scrollable = false;     
        }

        Neusoft.HISFC.BizProcess.Integrate.RADT manager = null;
        Neusoft.HISFC.BizLogic.RADT.InPatient radtManager = new Neusoft.HISFC.BizLogic.RADT.InPatient();

        //出院召回的有效天数
        private int callBackVaildDays;
        public const string control_id = "ZY0001";

        /// <summary>
        /// 初始化控制参数,获得出院召回的有效天数
        /// </summary>
        private void InitControlParam()
        {
            Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam ctrlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
            this.callBackVaildDays = ctrlParamIntegrate.GetControlParam<int>(control_id, true, 1);
        }

        private ArrayList GetPatientList()
        {
            Neusoft.HISFC.BizLogic.Manager.Department deptmanager = new Neusoft.HISFC.BizLogic.Manager.Department();
            var deplist = deptmanager.GetNurseAll();
            var al = this.radtManager.QueryPatientAllForTree();

            var patientList = al.Cast<Neusoft.HISFC.Models.RADT.PatientInfo>().ToList();

            var patientArry = new ArrayList();
            foreach (Neusoft.HISFC.Models.Base.Department tempdept in deplist)
            {
                if (ShowDept == "1")//显示科室 
                {
                    patientArry.Add(tempdept.Name);
                }

                patientArry.AddRange(patientList.Where(a => a.PVisit.PatientLocation.NurseCell.ID == tempdept.ID).ToList());

            }
            return patientArry;
        }

        /// <summary>
        /// 根据病区站得到患者
        /// </summary>
        /// <param name="al"></param>
        private ArrayList addPatientList()
        {

            ArrayList al = new ArrayList();//患者列表
            ArrayList al1 = new ArrayList();
            ArrayList deplist = new ArrayList();
            Neusoft.HISFC.BizLogic.Manager.Department deptmanager = new Neusoft.HISFC.BizLogic.Manager.Department();
            deplist = deptmanager.GetNurseAll();
            foreach (Neusoft.HISFC.Models.Base.Department tempdept in deplist)
            {
                if (Function.IsContainYKDept(tempdept.ID).Equals(Function.IsContainYKDept()))
                {
                    al1.Clear();
                    al1.AddRange(this.radtManager.PatientQueryByNurseCell(tempdept.ID, Neusoft.HISFC.Models.Base.EnumInState.B));

                    al1.AddRange(this.radtManager.PatientQueryByNurseCell(tempdept.ID, Neusoft.HISFC.Models.Base.EnumInState.R));

                    al1.AddRange(this.radtManager.PatientQueryByNurseCell(tempdept.ID, Neusoft.HISFC.Models.Base.EnumInState.I));

                    if (al1 != null && al1.Count > 0)
                    {
                        if (ShowDept == "1")//显示科室 
                        {
                            al.Add(tempdept.Name);

                        }
                        al.AddRange(al1);
                    }
                }
            }

            return al;
        }


    }
}
