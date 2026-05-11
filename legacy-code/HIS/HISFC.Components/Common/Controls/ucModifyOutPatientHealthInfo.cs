using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Controls
{
    public delegate void OnSave();

    /// <summary>
    /// 患者健康体征
    /// </summary>
    public partial class ucModifyOutPatientHealthInfo : UserControl
    {
        public ucModifyOutPatientHealthInfo()
        {
            InitializeComponent();
        }

        #region 变量

        private Neusoft.HISFC.BizLogic.Order.OutPatient.Order outOrderMgr = new Neusoft.HISFC.BizLogic.Order.OutPatient.Order();

        private Neusoft.HISFC.Models.Registration.Register regInfo = new Neusoft.HISFC.Models.Registration.Register();

        private Neusoft.HISFC.BizProcess.Integrate.Registration.Registration regIntergrate = new Neusoft.HISFC.BizProcess.Integrate.Registration.Registration();

        /// <summary>
        /// 默认保存体征信息的天数 0 标识不默认
        /// </summary>
        private int rememberHelthHistoryDays = 7;

        /// <summary>
        /// 默认保存体征信息的天数 0 标识不默认
        /// </summary>
        [Category("参数设置"), Description("默认保存体征信息的天数 0 标识不默认 默认:身高、体重"), DefaultValue(0)]
        public int RememberHelthHistoryDays
        {
            get
            {
                return rememberHelthHistoryDays;
            }
            set
            {
                rememberHelthHistoryDays = value;
            }
        }

        private string errInfo = "";

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrInfo
        {
            get
            {
                return errInfo;
            }
        }

        /// <summary>
        /// 当切患者信息
        /// </summary>
        public Neusoft.HISFC.Models.Registration.Register RegInfo
        {
            get
            {
                return regInfo;
            }
            set
            {
                regInfo = value;

                this.Clear();

                if (regInfo != null && !string.IsNullOrEmpty(regInfo.ID))
                {
                    if (this.ShowHealthInfo(regInfo.ID) == -1)
                    {
                        MessageBox.Show(errInfo);
                    }
                }
            }
        }

        private bool isVisibleSave = false;

        /// <summary>
        /// 保存按钮是否可见
        /// </summary>
        public bool IsVisibleSave
        {
            get
            {
                return isVisibleSave;
            }
            set
            {
                isVisibleSave = value;
                this.btOK.Visible = value;
            }
        }

        /// <summary>
        /// 调用保存
        /// </summary>
        public event OnSave OnSave;

        #endregion

        private void btOK_Click(object sender, EventArgs e)
        {
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            if (this.Save() == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(errInfo);
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();
            MessageBox.Show("保存成功！");
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            return this.UpdateHealthInfo(this.regInfo.ID, this.txtHeight.Text, this.txtWeight.Text,
                this.txtSBP.Text, this.txtDBP.Text, this.txtTem.Text, this.txtBloodGlu.Text, this.tbsymptom.Text.Trim());
        }

        /// <summary>
        /// 更新保存
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP">血压：收缩压</param>
        /// <param name="DBP">血压：舒张压</param>
        /// <returns></returns>
        private int UpdateHealthInfo(string clinicCode, string height, string weight, string SBP, string DBP, string Tem, string bloodGlu)
        {
            if (this.regInfo == null || string.IsNullOrEmpty(this.regInfo.ID))
            {
                errInfo = "患者信息为空！请选择患者！";
                return -1;
            }

            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(txtHeight.Text);
            }
            catch
            {
                errInfo = "身高输入错误：非法数字！";
                this.txtHeight.Focus();
                return -1;
            }
            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtWeight.Text);
            }
            catch
            {
                errInfo = "体重输入错误：非法数字！";
                this.txtWeight.Focus();
                return -1;
            }
            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtSBP.Text);
            }
            catch
            {
                errInfo = "收缩压输入错误：非法数字！";
                this.txtSBP.Focus();
                return -1;
            }
            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtDBP.Text);
            }
            catch
            {
                errInfo = "舒张压输入错误：非法数字！";
                this.txtDBP.Focus();
                return -1;
            }

            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtTem.Text);
            }
            catch
            {
                errInfo = "体温输入错误：非法数字！";
                this.txtDBP.Focus();
                return -1;
            }

            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtBloodGlu.Text);
            }
            catch
            {
                errInfo = "血糖输入错误：非法数字！";
                this.txtBloodGlu.Focus();
                return -1;
            }

            int rev = this.outOrderMgr.UpdateHealthInfo(height, weight, SBP, DBP, clinicCode, Tem, bloodGlu);
            //int rev = this.outOrderMgr.UpdateHealthInfoAndSymptom(height, weight, SBP, DBP, clinicCode, Tem, bloodGlu, symptom);
            if (rev == -1)
            {
                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                errInfo = "更新患者体征信息失败：" + this.outOrderMgr.Err;
                return -1;
            }
            return rev;
        }

        /// <summary>
        /// 更新保存
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP">血压：收缩压</param>
        /// <param name="DBP">血压：舒张压</param>
        /// <returns></returns>
        private int UpdateHealthInfo(string clinicCode, string height, string weight, string SBP, string DBP, string Tem, string bloodGlu,string symptom)
        {
            if (this.regInfo == null || string.IsNullOrEmpty(this.regInfo.ID))
            {
                errInfo = "患者信息为空！请选择患者！";
                return -1;
            }

            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(txtHeight.Text);
            }
            catch
            {
                errInfo = "身高输入错误：非法数字！";
                this.txtHeight.Focus();
                return -1;
            }
            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtWeight.Text);
            }
            catch
            {
                errInfo = "体重输入错误：非法数字！";
                this.txtWeight.Focus();
                return -1;
            }
            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtSBP.Text);
            }
            catch
            {
                errInfo = "收缩压输入错误：非法数字！";
                this.txtSBP.Focus();
                return -1;
            }
            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtDBP.Text);
            }
            catch
            {
                errInfo = "舒张压输入错误：非法数字！";
                this.txtDBP.Focus();
                return -1;
            }

            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtTem.Text);
            }
            catch
            {
                errInfo = "体温输入错误：非法数字！";
                this.txtDBP.Focus();
                return -1;
            }

            try
            {
                decimal i = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtBloodGlu.Text);
            }
            catch
            {
                errInfo = "血糖输入错误：非法数字！";
                this.txtBloodGlu.Focus();
                return -1;
            }

            int rev = this.outOrderMgr.UpdateHealthInfoAndSymptom(height, weight, SBP, DBP, clinicCode, Tem, bloodGlu, symptom);
            if (rev == -1)
            {
                //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                errInfo = "更新患者体征信息失败：" + this.outOrderMgr.Err;
                return -1;
            }
            return rev;
        }

        private void Clear()
        {
            this.txtHeight.Text = string.Empty;
            this.txtWeight.Text = string.Empty;
            this.txtSBP.Text = string.Empty;
            this.txtDBP.Text = string.Empty;
            this.txtTem.Text = string.Empty;
            this.txtBloodGlu.Text = string.Empty;
            this.tbsymptom.Text = string.Empty;
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        NotifyIcon notify = null;

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="SBP">血压：收缩压</param>
        /// <param name="DBP">血压：舒张压</param>
        /// <returns></returns>
        private int ShowHealthInfo(string clinicCode)
        {
            if (string.IsNullOrEmpty(clinicCode))
            {
                return 1;
            }

            ////没有挂号信息时，不报错，因为有些地方医生站可以自动挂号
            //Neusoft.HISFC.Models.Registration.Register regObj = this.regIntergrate.GetByClinic(clinicCode);
            //if (regObj == null || string.IsNullOrEmpty(regObj.ID))
            //{
            //    return 1;
            //}

            string height = "";
            string weight = "";
            string SBP = "";
            string DBP = "";
            string TEM = "";
            string bloodGlu = "";
            string symptom = "";

            if (notify == null)
            {
                notify = new NotifyIcon();
                notify.Icon = Components.Common.Properties.Resources.MEDICAL;
            }
            notify.Visible = false;

            //int rev = this.outOrderMgr.GetHealthInfo(clinicCode, ref height, ref weight, ref SBP, ref DBP, ref TEM, ref bloodGlu);
            int rev = this.outOrderMgr.GetHealthInfoAndSymptom(clinicCode, ref height, ref weight, ref SBP, ref DBP, ref TEM, ref bloodGlu,ref symptom);

            if (rev == -1)
            {
                errInfo = "获取患者体征信息失败：" + this.outOrderMgr.Err;
                return -1;
            }
            //没有挂号记录
            else if (rev == 0)
            {
                if (this.rememberHelthHistoryDays > 0)
                {
                    //if (this.outOrderMgr.GetHealthInfo(regInfo.PID.CardNO, this.rememberHelthHistoryDays, ref height, ref weight, ref SBP, ref DBP, ref TEM, ref bloodGlu) > 0)
                    if (this.outOrderMgr.GetHealthInfoAndSymptom(regInfo.PID.CardNO, this.rememberHelthHistoryDays, ref height, ref weight, ref SBP, ref DBP, ref TEM, ref bloodGlu, ref symptom) > 0)
                    {
                        this.txtHeight.Text = height;
                        this.txtWeight.Text = weight;
                        this.txtSBP.Text = SBP;
                        this.txtDBP.Text = DBP;
                        this.txtTem.Text = TEM;
                        this.txtBloodGlu.Text = bloodGlu;
                        this.tbsymptom.Text = symptom;

                        notify.Visible = true;
                        notify.ShowBalloonTip(2, "体征信息提示", "当前显示体征信息为上次默认值!\r\n如有变化，请注意修改保存！", ToolTipIcon.Info);
                    }
                }
                else
                {
                    errInfo = "没有挂号记录";
                    return 0;
                }
            }
            else
            {
                this.txtHeight.Text = height;
                this.txtWeight.Text = weight;
                this.txtSBP.Text = SBP;
                this.txtDBP.Text = DBP;
                this.txtTem.Text = TEM;
                this.txtBloodGlu.Text = bloodGlu;
                this.tbsymptom.Text = symptom;
            }

            return 1;
        }

        private void ucModifyOutPatientHealthInfo_Load(object sender, EventArgs e)
        {
            this.Clear();
        }

        /// <summary>
        /// 获取当前界面的体征信息，保存在患者实体
        /// </summary>
        /// <param name="regObj"></param>
        /// <returns></returns>
        public int GetHealthInfo(ref Neusoft.HISFC.Models.Registration.Register regObj)
        {
            regObj.Height = this.txtHeight.Text;
            regObj.Weight = this.txtWeight.Text;
            regObj.SBP = this.txtSBP.Text;
            regObj.DBP = this.txtDBP.Text;
            regObj.Temperature = this.txtTem.Text;
            regObj.BloodGlu = this.txtBloodGlu.Text;
            regObj.Mark1 = this.tbsymptom.Text.Trim();

            return 1;
        }
    }
}