using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    public partial class frmLockOrUnLock : Neusoft.FrameWork.WinForms.Forms.BaseForm
    {

        public frmLockOrUnLock()
        {
            InitializeComponent();
        }

        #region
        //传入类型    
        private Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = null;
        private Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();
        Neusoft.HISFC.Models.Base.EnumAlertTypeService alerTypeService = new Neusoft.HISFC.Models.Base.EnumAlertTypeService();
        Neusoft.HISFC.BizLogic.Fee.InPatient inpatientManager = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        #endregion

        #region 属性
        /// <summary>
        /// 患者信息
        /// </summary>
        public Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo
        {
            get
            {
                return this.patientInfo;
            }
            set
            {
                this.patientInfo = value;
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private int Init()
        {
            DateTime now = inpatientManager.GetDateTimeFromSysDateTime();
            this.dtBegin.Value = now;
            this.dtEnd.Value = now;
            this.lbName.Text = "姓名：" + this.PatientInfo.Name.ToString();
            this.lbPatientNo.Text = "住院号：" + this.PatientInfo.PID.PatientNO.ToString();
            DateTime dtNow = inpatientManager.GetDateTimeFromSysDateTime();
            if (this.patientInfo.PVisit.BeginDate != DateTime.MinValue)
            {
                this.dtBegin.Value = this.patientInfo.PVisit.BeginDate;
            }
            else
            {
                this.dtBegin.Value = dtNow;
            }
            if (this.patientInfo.PVisit.EndDate != DateTime.MinValue)
            {
                this.dtEnd.Value = this.patientInfo.PVisit.EndDate;
            }
            else
            {
                this.dtEnd.Value = dtNow;
            }
            if (JudPatientStatus())
            {
                this.lbStatus.Text = "已开锁";
            }
            else
            {
                this.lbStatus.Text = "未开锁";
                this.btnLock.Enabled = false;
            }
            return 1;
        }

        //判断患者是否开锁
        private bool JudPatientStatus()
        {
            DateTime dtNow = inpatientManager.GetDateTimeFromSysDateTime();
            if (dtNow > this.patientInfo.PVisit.BeginDate && dtNow < this.patientInfo.PVisit.EndDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //验证
        private bool Valid()
        {
            if (dtBegin.Value.Date > dtEnd.Value.Date)
            {
                MessageBox.Show("开始时间不能大于终止时间！");
                this.dtBegin.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新警戒线时间段用于开关锁
        /// </summary>    
        /// <param name="alertMoney"></param>
        /// <param name="alertType"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="LockOrUnLock">开锁OR关锁</param>
        /// <returns></returns>
        protected int Update(decimal alertMoney, string alertType, DateTime beginDate, DateTime endDate, string LockOrUnLock)
        {

            if (MessageBox.Show("确定"+LockOrUnLock+"？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return -1;
            }
            if (LockOrUnLock=="开锁")
            {
                if (JudPatientStatus())
                {
                    if (MessageBox.Show("改患者已经出于开锁状态，是否更改开锁时间？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        return -1;
                    }
                }
            }            
            if (!Valid())
            {
                return -1;
            }
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            radtIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            int retureValue = 0;
            retureValue = radtIntegrate.UpdatePatientAlert(this.PatientInfo.ID, alertMoney, alertType, beginDate, endDate);
            Neusoft.FrameWork.Models.NeuObject oldObj = new Neusoft.FrameWork.Models.NeuObject();
            Neusoft.FrameWork.Models.NeuObject newObj = new Neusoft.FrameWork.Models.NeuObject();
            oldObj.ID = this.lbStatus.Text;
            oldObj.Name = this.patientInfo.PVisit.BeginDate.ToString() + "|" + this.patientInfo.PVisit.EndDate.ToString();
            newObj.ID = LockOrUnLock;
            newObj.Name = beginDate.ToString() + "|" + endDate.ToString();
            int res;
            if (LockOrUnLock=="开锁")
            {
                
                res = radtIntegrate.InsertShiftData(this.PatientInfo.ID, Neusoft.HISFC.Models.Base.EnumShiftType.UNFL, "费用" + LockOrUnLock, oldObj, newObj);
            }
            else
            {
                res = radtIntegrate.InsertShiftData(this.PatientInfo.ID, Neusoft.HISFC.Models.Base.EnumShiftType.FL, "费用" + LockOrUnLock, oldObj, newObj);
            }
            if (retureValue < 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(LockOrUnLock+"失败" + Neusoft.FrameWork.Management.PublicTrans.Err);
                return -1;
            }
            else
            {
                Neusoft.FrameWork.Management.PublicTrans.Commit();
                MessageBox.Show(LockOrUnLock + "成功");
            }
            return 1;
        }

        #endregion

        //开锁
        private void btnUnLock_Click(object sender, EventArgs e)
        {
            //decimal alertMoney = Neusoft.FrameWork.Function.NConvert.ToDecimal(patientInfo.PVisit.MoneyAlert);
            DateTime beginDate = this.dtBegin.Value;
            DateTime endDate = this.dtEnd.Value;
            if (Update(patientInfo.PVisit.MoneyAlert, "D", beginDate, endDate, "开锁") > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        //关锁
        private void btnLock_Click(object sender, EventArgs e)
        {
            DateTime now = inpatientManager.GetDateTimeFromSysDateTime();

            if (Update(patientInfo.PVisit.MoneyAlert, "M", now, now, "关锁") > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void frmLockOrUnLock_Load(object sender, EventArgs e)
        {
            this.Init();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
