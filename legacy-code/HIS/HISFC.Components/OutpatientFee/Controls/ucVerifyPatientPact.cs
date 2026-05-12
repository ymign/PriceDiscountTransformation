using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucVerifyPatientPact : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        #region 变量
        /// <summary>
        /// 是否停止验证
        /// </summary>
        bool isStopVerify = false;
        /// <summary>
        /// 需要验证合同单位列表
        /// </summary>
        List<string> lstVerifyPact = new List<string>();

        /// <summary>
        /// 医疗待遇接口
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();
        /// <summary>
        /// 
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.Account accManage = new Neusoft.HISFC.BizLogic.Fee.Account();

        Neusoft.HISFC.Components.Common.Forms.frmProgressBar frmProgress = null;

        System.Timers.Timer time = null;
        #endregion

        #region 属性
        /// <summary>
        /// 需要验证的合同单位，多个以 | 隔开
        /// </summary>
        [Category("控件设置"), Description("需要验证的合同单位，多个以 | 隔开")]
        public string VerifyPact
        {
            get 
            {
                string strTemp = string.Empty;
                if (lstVerifyPact != null && lstVerifyPact.Count > 0)
                {
                    foreach (string strPact in lstVerifyPact)
                    {
                        if (!string.IsNullOrEmpty(strPact))
                        {
                            strTemp += strPact + "|";
                        }
                    }
                }

                strTemp = strTemp.Trim(new char[] { '|' });
                return strTemp;
            }
            set 
            {
                lstVerifyPact.Clear();
                string strTemp = value;
                if (!string.IsNullOrEmpty(strTemp))
                {
                    string[] strArr = strTemp.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strArr != null && strArr.Length > 0)
                    {
                        lstVerifyPact.AddRange(strArr);
                    }
                }
            }
        }

        #endregion

        public ucVerifyPatientPact()
        {
            InitializeComponent();

            time = new System.Timers.Timer();
            time.Enabled = false;
            time.AutoReset = true;
            time.Interval = 60000;

            time.Elapsed += new System.Timers.ElapsedEventHandler(time_Elapsed);
        }

        void time_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string strTime = this.accManage.GetSysDateTime();
            DateTime dtTime = Convert.ToDateTime(strTime);
            if (dtTime >= this.dtpDealTime.Value)
            {
                time.Stop();
                time.Enabled = false;

                Verify();
            }
        }

        bool frmProgress_evnProgressStop()
        {
            StopVerify();
            return true;
        }

        private void chkTimer_CheckedChanged(object sender, EventArgs e)
        {
            dtpDealTime.Enabled = chkTimer.Checked;
            time.Enabled = false;
        }

        /// <summary>
        /// toolBar
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();
        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("较验", "开始批量较验患者身份！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Z执行, true, false, null);
            toolBarService.AddToolButton("停止较验", "停止较验患者身份！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q取消, true, false, null);
            return toolBarService;
        }

        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "较验":
                    VerifyClick();
                    break;

                case "停止较验":
                    StopVerify();
                    break;
            }
            base.ToolStrip_ItemClicked(sender, e);
        }

        private void StopVerify()
        {
            isStopVerify = true;
            time.Stop();
            time.Enabled = false;
        }

        private void Verify()
        {
            if (isStopVerify)
            {
                return;
            }

            neuSpread1_Sheet1.RowCount = 0;

            frmProgress = new Neusoft.HISFC.Components.Common.Forms.frmProgressBar();
            frmProgress.evnProgressStop += new Neusoft.HISFC.Components.Common.Forms.ProgressStop(frmProgress_evnProgressStop);
            frmProgress.TopMost = true;
            frmProgress.MaxBar = 100;
            frmProgress.MinBar = 0;
            frmProgress.SetValue = 0;
            frmProgress.Title = "身份验证";
            frmProgress.SetInfo = "请稍后......正在获取患者信息！";
            frmProgress.Show();

            Application.DoEvents();

            List<Neusoft.HISFC.Models.RADT.PatientInfo> lstPatient = this.accManage.QueryPatientByPact(lstVerifyPact);
            if (lstPatient == null || lstPatient.Count <= 0)
            {
                frmProgress.Close();

                MessageBox.Show("查找患者失败！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int iStep = 30;
            frmProgress.MaxBar = lstPatient.Count;
            frmProgress.MinBar = 0;
            frmProgress.StepBar = iStep;

            Neusoft.HISFC.Models.RADT.PatientInfo patient = null;
            for (int idx = 0; idx < lstPatient.Count; idx++)
            {
                if (isStopVerify)
                {
                    this.frmProgress.Close();
                    break;
                }
                patient = lstPatient[idx];

                frmProgress.SetInfo = "正在验证患者【" + patient.Name + "】 ....";
                if ((idx + 1) % iStep == 0)
                {
                    frmProgress.MoveStep();
                }
                Application.DoEvents();

                medcareProxy.SetPactCode(patient.Pact.ID);
                medcareProxy.IsLocalProcess = false;

                //连接待遇接口
                long returnValue = this.medcareProxy.Connect();
                if (returnValue == -1)
                {
                    //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    //医保回滚可能出错，此处提示
                    //this.medcareProxy.Rollback();

                    SetUnVerifyPatient(patient, this.medcareProxy.ErrMsg);

                    continue;
                }

                // 判断是否允许报销
                if (this.medcareProxy.IsInBlackList(patient))
                {
                    //Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    //医保回滚可能出错，此处提示
                    //this.medcareProxy.Rollback();

                    SetUnVerifyPatient(patient, this.medcareProxy.ErrMsg);

                    continue;
                }
            }

            frmProgress.Close();
        }

        private void VerifyClick()
        {
            isStopVerify = false;

            if (this.chkTimer.Checked)
            {
                time.Start();
            }
            else
            {
                Verify();
            }
        }

        private void SetUnVerifyPatient(Neusoft.HISFC.Models.RADT.PatientInfo patient, string remark)
        {
            neuSpread1_Sheet1.RowCount += 1;

            int iAddRow = neuSpread1_Sheet1.RowCount - 1;
            neuSpread1_Sheet1.Cells[iAddRow, 0].Text = patient.PID.CardNO;
            neuSpread1_Sheet1.Cells[iAddRow, 1].Text = patient.Name;
            neuSpread1_Sheet1.Cells[iAddRow, 2].Text = patient.Sex.Name;
            neuSpread1_Sheet1.Cells[iAddRow, 3].Text = patient.IDCard;
            neuSpread1_Sheet1.Cells[iAddRow, 4].Text = patient.Pact.Name;
            neuSpread1_Sheet1.Cells[iAddRow, 5].Text = remark;
        }

        public override int Export(object sender, object neuObject)
        {
            if (this.neuSpread1_Sheet1.RowCount <= 0)
                return 0;

            return this.neuSpread1.Export();
        }
    }
}
