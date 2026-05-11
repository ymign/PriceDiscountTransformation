using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Controls
{
    public partial class ucQueryInpatient : UserControl
    {
        public ucQueryInpatient()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 患者信息
        /// </summary>
        public static Neusoft.HISFC.Models.RADT.PatientInfo patient = null;

        private Neusoft.HISFC.BizLogic.RADT.InPatient inpatientManager = new Neusoft.HISFC.BizLogic.RADT.InPatient();

        private Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();

        #endregion


        private void ucQueryInpatient_Load(object sender, EventArgs e)
        {
            this.ActiveControl = this.txtInpatientNO;
            this.FindForm().Text = "患者信息查询";
            PatientEvent.GetPatientEvent += new PatientEvent.GetPatient(PatientEvent_GetPatientEvent);
            this.FindForm().FormClosing+=new FormClosingEventHandler(ucQueryInpatient_FormClosing);
            this.txtInpatientNO.Text = string.Empty;
            this.txtNurse.Text = string.Empty;
            this.txtBed.Text = string.Empty;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.FindForm().Close();
            }
            if (keyData == Keys.F12)
            {
                if (patient == null) return true;
                this.FindForm().DialogResult = DialogResult.OK;
                
            }
            return base.ProcessDialogKey(keyData);
        }

        private void btNuerse_Click(object sender, EventArgs e)
        {
            ucNurse uc = new ucNurse();
            Neusoft.FrameWork.WinForms.Classes.Function.ShowControl(uc);
            
        }

        private void PatientEvent_GetPatientEvent(Neusoft.HISFC.Models.RADT.PatientInfo p)
        {
            patient = p;
            this.FindForm().DialogResult = DialogResult.OK;
        }

        private void ucQueryInpatient_FormClosing(object sender, EventArgs e)
        {
            PatientEvent.GetPatientEvent -= new PatientEvent.GetPatient(PatientEvent_GetPatientEvent);
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.FindForm().DialogResult = DialogResult.Cancel;
        }

        private void txtNurse_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtNurse.Text == "-")
                {
                    btNuerse_Click(null, null);
                }
                else
                {
                    this.txtBed.Focus();
                    this.txtBed.SelectAll();
                }

            }
        }

        private void txtInpatientNO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (string.IsNullOrEmpty(this.txtInpatientNO.Text))
                {
                    this.txtNurse.Focus();
                    this.txtNurse.SelectAll();
                }
                else
                {
                    patient = new Neusoft.HISFC.Models.RADT.PatientInfo();
                    patient.PID.PatientNO = this.txtInpatientNO.Text;

                    ArrayList alPatients = inpatientManager.QueryInpatientNOByPatientNO(patient.PID.PatientNO.PadLeft(10, '0'));

                    foreach (Neusoft.FrameWork.Models.NeuObject obj in alPatients)
                    {
                        if (obj.Memo == "I")
                        {
                            patient = inpatientManager.QueryPatientInfoByInpatientNO(obj.ID);
                        }
                    }

                    this.FindForm().DialogResult = DialogResult.OK;

                }
            }
        }

        private void txtBed_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter) return;
            string nurseCode = this.txtNurse.Text;
            if (string.IsNullOrEmpty(nurseCode))
            {
                MessageBox.Show("请输入病区编码");
                return;
            }
            string bedNO = this.txtBed.Text;
            bedNO = nurseCode + bedNO;
            ArrayList al = inpatientManager.QueryInpatientNOByBedNO(bedNO);
            if (al == null)
            {
                MessageBox.Show("按病床查询患者信息失败！"+inpatientManager.Err);
                return;
            }
            if (al.Count == 0)
            {
                MessageBox.Show("该病床不存在患者！");
                return;
            }
            if (al.Count == 1)
            {
                patient = new Neusoft.HISFC.Models.RADT.PatientInfo();
                patient.PID.PatientNO = (al[0] as Neusoft.FrameWork.Models.NeuObject).ID.Substring(4, 10);
                this.FindForm().DialogResult = DialogResult.OK;
            }
            else
            {
                ucGetPatient uc = new ucGetPatient();
                uc.AlPatient = al;
                Neusoft.FrameWork.WinForms.Classes.Function.ShowControl(uc);
            }

        }

        private void btBed_Click(object sender, EventArgs e)
        {
            string nurseCode = this.txtNurse.Text;
            if (string.IsNullOrEmpty(nurseCode))
            {
                MessageBox.Show("请输入病区编码");
                return;
            }
            ArrayList al = radtIntegrate.QueryPatientByNurseCellAndState(nurseCode, Neusoft.HISFC.Models.Base.EnumInState.I);
            if (al == null)
            {
                MessageBox.Show("查询病区在院患者信息失败！");
                return;
            }
            if (al.Count == 0)
            {
                MessageBox.Show("该病区不存在在院患者！");
                return;
            }

            ucGetPatient uc = new ucGetPatient();
            uc.AlPatient = al;
            Neusoft.FrameWork.WinForms.Classes.Function.ShowControl(uc);
        }
    }

    
}
