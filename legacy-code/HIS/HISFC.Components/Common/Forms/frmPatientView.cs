using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.Common.Forms
{
    public partial class frmPatientView : Form
    {

        public delegate void SelectPatientInfoDelagate(object sender);

        public event SelectPatientInfoDelagate SelectedPatientinfo = null;
        Neusoft.HISFC.BizLogic.RADT.InPatient Inpatient = new Neusoft.HISFC.BizLogic.RADT.InPatient();
        Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();
        public frmPatientView()
        {
            InitializeComponent();
            this.ucQueryInpatientNo1.myEvent += new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(ucQueryInpatientNo1_myEvent);
        }

        void ucQueryInpatientNo1_myEvent()
        {
            Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = this.radtIntegrate.GetPatientInfomation(this.ucQueryInpatientNo1.InpatientNo);
            ArrayList alPatientInfo = new ArrayList();
            alPatientInfo.Add(patientInfo);
            this.SetPatientInfo(alPatientInfo);
        }

        public void SetPatientInfo(ArrayList alPatient)
        {

            this.neuSpread1_Sheet1.RowCount = alPatient.Count;
            for (int i = 0; i < alPatient.Count; i++)
            {
                Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = alPatient[i] as Neusoft.HISFC.Models.RADT.PatientInfo;

                this.neuSpread1_Sheet1.Cells[i, (int)EnumCol.patientNO].Text = patientInfo.PID.PatientNO;
                this.neuSpread1_Sheet1.Cells[i, (int)EnumCol.name].Text = patientInfo.Name;
                this.neuSpread1_Sheet1.Cells[i, (int)EnumCol.dept].Text = patientInfo.PVisit.PatientLocation.Dept.Name;
                this.neuSpread1_Sheet1.Cells[i, (int)EnumCol.nurseCell].Text = patientInfo.PVisit.PatientLocation.NurseCell.Name;
                this.neuSpread1_Sheet1.Cells[i, (int)EnumCol.inDate].Text = patientInfo.PVisit.InTime.ToString();
                //{B9F0D007-928A-4e96-805E-95F1C034718D}
                //this.neuSpread1_Sheet1.Cells[i, (int)EnumCol.instate].Text = patientInfo.PVisit.InState.ID.ToString();
                this.neuSpread1_Sheet1.Cells[i, (int)EnumCol.instate].Text = patientInfo.PVisit.InState.Name.ToString();
                this.neuSpread1_Sheet1.Rows[i].Tag = patientInfo; 

            }


        }

        protected virtual int QueryPatientInfoByDept()
        {
            if (this.cmbInhosDept.SelectedItem == null || string.IsNullOrEmpty(this.cmbInhosDept.Tag.ToString()))
            {
                return -1;
            }
            DateTime fromDate = this.dtpFromDate.Value.Date;
            DateTime toDate = this.dtpToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            ArrayList alInpatientNos= this.Inpatient.PatientQuery(this.cmbInhosDept.Tag.ToString(), fromDate, toDate);
            this.SetPatientInfo(alInpatientNos);
            return 1;
        }

        protected virtual int QueryPatientInfoByName()
        {
            if (string.IsNullOrEmpty(this.txtName.Text.Trim()))
            {
                MessageBox.Show("请输入姓名");
                this.txtName.Focus();
                return -1;
            }

            ArrayList alInpatientNos = this.Inpatient.PatientQueryByName(this.txtName.Text);

            this.SetPatientInfo(alInpatientNos);
            return 1;
        }

        private void btQueryByName_Click(object sender, EventArgs e)
        {
            this.QueryPatientInfoByName();

        }

        private void btQueryByDept_Click(object sender, EventArgs e)
        {
            this.QueryPatientInfoByDept();
        }

        protected virtual int InitDept()
        {
            Neusoft.HISFC.BizLogic.Manager.Department deptManager = new Neusoft.HISFC.BizLogic.Manager.Department();

            ArrayList alDeptList = deptManager.GetDeptment(Neusoft.HISFC.Models.Base.EnumDepartmentType.I);

            this.cmbInhosDept.AddItems(alDeptList);

            return 1;
        }


        protected override void OnLoad(EventArgs e)
        {
            this.InitDept();
            base.OnLoad(e);
        }

        private enum EnumCol
        {
            patientNO,
            name,
            dept,
            nurseCell,
            inDate,
            instate
        }

        private void neuSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (this.SelectedPatientinfo != null)
            {
                Neusoft.HISFC.Models.RADT.PatientInfo p = this.neuSpread1_Sheet1.Rows[e.Row].Tag as Neusoft.HISFC.Models.RADT.PatientInfo;

                this.SelectedPatientinfo(p);
            }

            this.DialogResult = DialogResult.OK;
        }
    }

    
}
