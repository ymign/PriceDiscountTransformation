using System;
using System.Data;

namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    public partial class ZdwyFeeAlertPrintControl : Neusoft.FrameWork.WinForms.Controls.ucBaseControl,
        Neusoft.HISFC.BizProcess.Interface.FeeInterface.IMoneyAlert
    {
        public ZdwyFeeAlertPrintControl()
        {
            InitializeComponent();
        }

        DateTime _today = DateTime.Now;

        /// <summary>
        /// 患者信息实体
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.InPatient _inPatientManager = new Neusoft.HISFC.BizLogic.Fee.InPatient();

        /// <summary>
        /// 患者基本信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.PatientInfo _patientInfo = null;


        #region IMoneyAlert 成员

        public Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo
        {
            get
            {
                return _patientInfo;
            }
            set
            {
                _patientInfo = value;
                _today = _inPatientManager.GetDateTimeFromSysDateTime();
                this.SetPatientInfo();
            }
        }

        public void SetPatientInfo()
        {
            this.lblPatientNO.Text = "住院号：" + _patientInfo.PID.PatientNO;
            this.lblNurse.Text = "病区：" + _patientInfo.PVisit.PatientLocation.NurseCell.Name;
            this.lblBedNO.Text = "床位：" + _patientInfo.PVisit.PatientLocation.Bed.ID;
            this.lblName.Text = "姓名：" + _patientInfo.Name;
            this.lblGender.Text = "性别：" + _patientInfo.Sex.Name;

            this.lblPactName.Text = "结算方式：" + _patientInfo.Pact.Name;
            this.lblDateBegin.Text = "起日：" + _patientInfo.PVisit.InTime.ToString("yyyy-MM-dd");
            this.lblDateEnd.Text = "止日：" + _today.ToString("yyyy-MM-dd");

            this.lblOwnCost.Text = "合计：" + _patientInfo.FT.OwnCost.ToString("f2");
            this.lblNopay.Text = "未清金额：" + _patientInfo.FT.OwnCost.ToString("F2");
            this.lblPreCost.Text = "预交款：" + _patientInfo.FT.PrepayCost.ToString("f2");
            this.lblFreeCost.Text = "结余金额：" + _patientInfo.FT.LeftCost.ToString("F2");

            this.lblPrompt.Text = string.Format("您入院时的预交款已用完，为了您快速治愈出院，请与今日内交款{0}元。多谢合作！",
                (500.00m - _patientInfo.FT.LeftCost).ToString("F2"));
            this.lblToday.Text = _today.ToString("yyyy年MM月dd日");

            string sql = string.Format(@"
select d.fee_stat_name, d.print_order, sum(f.own_cost)
  from fin_ipb_feeinfo f, fin_com_feecodestat d
 where f.fee_code = d.fee_code
   and f.inpatient_no = '{0}'
 group by d.fee_stat_name, d.print_order
", _patientInfo.PID.PatientNO);

            DataSet dsResult = new DataSet();
            if (dsResult.Tables.Count <= 0) return;

            foreach (DataRow dr in dsResult.Tables[0].Rows)
            {
                string name = dr[0].ToString();
                string cost = dr[2].ToString();

                foreach (Neusoft.FrameWork.WinForms.Controls.NeuLabel nl in this.panelDetails.Controls)
                {
                    if (!nl.Text.Contains(name)) continue;
                    nl.Text = name + "：" + cost;
                }
            }
        }

        #endregion

    }

}
