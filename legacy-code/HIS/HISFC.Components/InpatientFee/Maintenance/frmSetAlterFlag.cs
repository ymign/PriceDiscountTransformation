using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.InpatientFee.Maintenance
{
    /// <summary>
    /// 开关警戒线
    /// </summary>
    public partial class frmSetAlterFlag : Form
    {
        public frmSetAlterFlag()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                this.cmbType.SelectedIndexChanged += new EventHandler(cmbType_SelectedIndexChanged);
                btCloseAlter.Click += new EventHandler(btCloseAlter_Click);
                btStartAlter.Click += new EventHandler(btStartAlter_Click);
                btCloseWindow.Click += new EventHandler(btCloseWindow_Click);
            }
        }

        /// <summary>
        /// 合同单位类别列表
        /// </summary>
        private ArrayList alPayKind = null;

        /// <summary>
        /// 合同单位列表
        /// </summary>
        private ArrayList alPact = null;

        /// <summary>
        /// 病区列表
        /// </summary>
        private ArrayList alNurse = null;


        private Neusoft.HISFC.BizProcess.Integrate.Manager interMgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        private Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();

        private Neusoft.HISFC.BizLogic.RADT.InPatient inpatientMgr = new Neusoft.HISFC.BizLogic.RADT.InPatient();

        private Neusoft.HISFC.Models.RADT.PatientInfo pInfo = null;

        string operCode = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).ID;
        /// <summary>
        /// 当前患者
        /// </summary>
        public Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo
        {
            get
            {
                return pInfo;
            }
            set
            {
                pInfo = value;
                if (pInfo != null && !string.IsNullOrEmpty(pInfo.ID))
                {
                    lblPaeintInfo.Text = "当前患者：" + pInfo.PVisit.PatientLocation.Dept.Name + " " + pInfo.PVisit.PatientLocation.Bed.ID.Substring(4) + " " + pInfo.Name + " " + pInfo.Sex.Name + " " + inpatientMgr.GetAge(pInfo.Birthday) + "\r\n\r\n警戒线启用状态：" + (pInfo.PVisit.AlertFlag ? "启用" : "关闭");
                }
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btCloseWindow_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 获取界面参数
        /// </summary>
        /// <param name="payKindCode"></param>
        /// <param name="pactCode"></param>
        /// <param name="nurseCode"></param>
        /// <param name="inPatientNo"></param>
        /// <returns></returns>
        private int GetArgs(ref string payKindCode, ref string pactCode, ref string nurseCode, ref string inPatientNo)
        {
            payKindCode = "ALL";
            pactCode = "ALL";
            nurseCode = "ALL";
            inPatientNo = "ALL";

            #region 0 合同单位类别
            if (this.cmbType.SelectedIndex == 3)
            {
                payKindCode = cmbPatientType.Tag.ToString();
            }
            #endregion

            #region 1 合同单位

            else if (this.cmbType.SelectedIndex == 1)
            {
                pactCode = cmbPatientType.Tag.ToString();
            }
            #endregion

            #region 2 科室
            else if (this.cmbType.SelectedIndex == 2)
            {
                nurseCode = cmbPatientType.Tag.ToString();
            }
            #endregion

            #region 3 患者
            else if (this.cmbType.SelectedIndex == 0)
            {
                inPatientNo = "";
                if (pInfo != null)
                {
                    inPatientNo = pInfo.ID;
                }

                if (pInfo == null || string.IsNullOrEmpty(inPatientNo))
                {
                    MessageBox.Show("患者信息为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return -1;
                }
            }
            #endregion

            return 1;
        }

        /// <summary>
        /// 启用警戒线限制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btStartAlter_Click(object sender, EventArgs e)
        {
            string payKindCode = "ALL";
            string pactCode = "ALL";
            string nurseCode = "ALL";
            string inPatientNo = "ALL";

            if (GetArgs(ref payKindCode, ref pactCode, ref nurseCode, ref inPatientNo) == -1)
            {
                return;
            }

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            if (this.radtIntegrate.UpdatePatientAlertFlag(payKindCode, pactCode, nurseCode, inPatientNo, true, operCode) <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("启用警戒线限制失败！\r\n\r\n" + radtIntegrate.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            Neusoft.FrameWork.WinForms.Classes.Function.ShowBalloonTip(3, "提示", "\r\n启用警戒线限制成功！\r\n\r\n", ToolTipIcon.Info);
            this.Close();
        }

        /// <summary>
        /// 关闭警戒线设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btCloseAlter_Click(object sender, EventArgs e)
        {
            string payKindCode = "ALL";
            string pactCode = "ALL";
            string nurseCode = "ALL";
            string inPatientNo = "ALL";

            if (GetArgs(ref payKindCode, ref pactCode, ref nurseCode, ref inPatientNo) == -1)
            {
                return;
            }

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            if (this.radtIntegrate.UpdatePatientAlertFlag(payKindCode, pactCode, nurseCode, inPatientNo, false, operCode) <= 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("关闭警戒线限制失败！\r\n\r\n" + radtIntegrate.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            Neusoft.FrameWork.WinForms.Classes.Function.ShowBalloonTip(3, "提示", "\r\n关闭警戒线限制成功！\r\n\r\n", ToolTipIcon.Info);
            this.Close();
        }

        /// <summary>
        /// 选择设置类别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cmbPatientType.Enabled = true;

            #region 0 合同单位类别
            if (this.cmbType.SelectedIndex == 3)
            {
                if (alPayKind == null)
                {
                    Neusoft.FrameWork.Models.NeuObject con01 = new Neusoft.FrameWork.Models.NeuObject("01", "自费", "自费");
                    Neusoft.FrameWork.Models.NeuObject con02 = new Neusoft.FrameWork.Models.NeuObject("02", "医保", "医保");
                    Neusoft.FrameWork.Models.NeuObject con03 = new Neusoft.FrameWork.Models.NeuObject("03", "公费", "公费");
                    alPayKind = new ArrayList();
                    alPayKind.Add(con01);
                    alPayKind.Add(con02);
                    alPayKind.Add(con03);
                }

                cmbPatientType.AddItems(alPayKind);
            }
            #endregion

            #region 1 合同单位
            else if (this.cmbType.SelectedIndex == 1)
            {
                if (alPact == null)
                {
                    alPact = interMgr.QueryPactUnitInPatient();
                    if (alPact == null)
                    {
                        MessageBox.Show("获取住院合同单位信息失败！\r\n\r\n" + interMgr.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (alPact != null)
                {
                    cmbPatientType.AddItems(alPact);
                }
            }
            #endregion

            #region 2 科室
            else if (this.cmbType.SelectedIndex == 2)
            {
                if (alNurse == null)
                {
                    alNurse = interMgr.GetDepartment(Neusoft.HISFC.Models.Base.EnumDepartmentType.N);
                    if (alNurse == null)
                    {
                        MessageBox.Show("获取住院病区列表失败！\r\n\r\n" + interMgr.Err, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (alNurse != null)
                {
                    cmbPatientType.AddItems(alNurse);
                }
            }
            #endregion

            #region 3 患者
            else if (this.cmbType.SelectedIndex == 0)
            {
                cmbPatientType.Tag = "";
                this.cmbPatientType.Text = "";
                this.cmbPatientType.Enabled = false;
            }
            #endregion
        }

        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
        }


    }
}
