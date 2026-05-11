using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Neusoft.FrameWork.WinForms.Forms;

namespace Neusoft.HISFC.Components.InpatientFee.Prepay
{
    public partial class ucPrepaySelectPrint : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucPrepaySelectPrint()
        {
            InitializeComponent();
        }

        Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = null;
        Neusoft.HISFC.BizLogic.Fee.InPatient myInpatient = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        /// <summary>
        /// 住院费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.InPatient feeInpatient = new Neusoft.HISFC.BizLogic.Fee.InPatient();

        /// <summary>
        /// 管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// toolBarService
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();

        private void ucQueryInpatientNo1_myEvent()
        {
            if (this.ucQueryInpatientNo1.InpatientNo == null || this.ucQueryInpatientNo1.InpatientNo.Trim() == "")
            {
                MessageBox.Show("住院号错误，没有找到该患者");
                return;
            }

            Neusoft.HISFC.BizLogic.RADT.InPatient myRADT = new Neusoft.HISFC.BizLogic.RADT.InPatient();
            patientInfo = myRADT.QueryPatientInfoByInpatientNO(this.ucQueryInpatientNo1.InpatientNo);

            //控件赋值患者信息
            this.EvaluteByPatientInfo(this.patientInfo);
            //farpoint显示预交金信息
            this.QueryPatientPrepay(this.patientInfo);

        }


        /// <summary>
        /// 打印
        /// </summary>
        protected void onPrint()
        {
            if (this.patientInfo == null)
            {
                MessageBox.Show("请选择患者");
                return;
            }
            int rowCount = this.fpPrepay_Sheet1.RowCount;
            ArrayList alPrint = new ArrayList();
            for (int i = 0; i < rowCount; i++)
            {
                if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpPrepay_Sheet1.Cells[i, 0].Text))
                {
                    alPrint.Add(this.fpPrepay_Sheet1.Rows[i].Tag);
                }
            }
            if (alPrint.Count < 1 || alPrint == null)
            {
                MessageBox.Show("请选择打印预交金数据");
                return;
            }
            ucPatientInfoPrint ucPatientInfoPrint1 = new ucPatientInfoPrint();
            ucPatientInfoPrint1.SetPatientInfo(this.patientInfo, alPrint);
            ucPatientInfoPrint1.Print();

        }

        /// <summary>
        /// 清空
        /// </summary>
        protected virtual void Clear()
        {
            this.patientInfo = null;
            this.txtName.Text = "";
            this.txtDept.Text = "";
            this.txtPact.Text = "";
            this.txtBedNo.Text = "";
            txtBirthday.Text = "";
            txtNurseStation.Text = "";
            txtDateIn.Text = "";
            txtDoctor.Text = "";
            this.txtIntimes.Text = "";
            this.txtClinicDiagnose.Text = "";
            this.fpPrepay_Sheet1.RowCount = 0;
            this.ucQueryInpatientNo1.Text = "";
        }

        /// <summary>
        /// 利用患者信息实体进行控件赋值
        /// </summary>
        /// <param name="patientInfo">患者基本信息实体</param>
        protected virtual void EvaluteByPatientInfo(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo)
        {
            if (patientInfo == null)
            {
                patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
            }
            // 姓名
            this.txtName.Text = patientInfo.Name;
            // 科室
            this.txtDept.Text = patientInfo.PVisit.PatientLocation.Dept.Name;
            // 合同单位
            this.txtPact.Text = patientInfo.Pact.Name;
            //床号
            this.txtBedNo.Text = patientInfo.PVisit.PatientLocation.Bed.ID;
            //生日
            txtBirthday.Text = patientInfo.Birthday.ToString("yyyy-MM-dd");
            //所属病区
            txtNurseStation.Text = patientInfo.PVisit.PatientLocation.NurseCell.Name;
            //入院日期
            txtDateIn.Text = patientInfo.PVisit.InTime.ToString("yyyy-MM-dd");
            // 医生
            txtDoctor.Text = patientInfo.PVisit.AdmittingDoctor.Name;
            //住院号
            this.ucQueryInpatientNo1.Text = patientInfo.PID.PatientNO;

            //门诊诊断
            this.txtClinicDiagnose.Text = patientInfo.ClinicDiagnose;
            //住院次数
            this.txtIntimes.Text = patientInfo.InTimes.ToString();

        }

        /// <summary>
        /// 查询患者预交金信息
        /// </summary>
        /// <param name="patientInfo">住院患者基本信息实体</param>
        /// <returns>1 成功 －1失败</returns>
        protected virtual int QueryPatientPrepay(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo)
        {
            //添加行


            ArrayList al = new ArrayList();

            try
            {
                //根据住院号提取患者预交金信息到ArrayList
                al = this.feeInpatient.QueryPrepays(patientInfo.ID);
                if (al == null)
                    return 0;
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(ex.Message, 211);
                return -1;
            }
            this.fpPrepay_Sheet1.RowCount = 0;
            this.fpPrepay_Sheet1.RowCount = al.Count;
            //交款次数
            int PayCount = 0;
            //返款次数
            int WasteCount = 0;

            for (int i = 0; i < al.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
                prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)al[i];

                string PrepayState = "";
                if (prepay.FT.PrepayCost > 0)
                {
                    PayCount++;
                    PrepayState = "收取";
                }
                else
                {
                    WasteCount++;
                    switch (prepay.PrepayState)
                    {
                        case "1":
                            PrepayState = "作废";

                            break;
                        case "2":
                            PrepayState = "补打";
                            break;
                        default:
                            PrepayState = "收取";
                            break;
                    }
                }
                //更新一些没有的字段()
                string PrepaySource = "";
                if (prepay.TransferPrepayState == "0")
                {
                    PrepaySource = "预交金";
                }
                else
                {
                    PrepaySource = "转押金";
                }
                //结算标记
                string BalanceFlag = "";
                if (prepay.BalanceState == "0")
                {
                    BalanceFlag = "未结算";
                }
                else
                {
                    BalanceFlag = "已结算";
                }
                //收款员姓名


                Neusoft.HISFC.BizProcess.Integrate.Manager managerIntergrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
                Neusoft.HISFC.Models.Base.Employee empl = new Neusoft.HISFC.Models.Base.Employee();
                empl = managerIntergrate.GetEmployeeInfo(prepay.PrepayOper.ID);

                if (empl == null)
                {
                    prepay.PrepayOper.Name = "";
                }
                else
                {
                    prepay.PrepayOper.Name = empl.Name;
                }
                //支付方式

                Neusoft.FrameWork.Models.NeuObject payObj = this.managerIntegrate.GetConstant("PAYMODES", prepay.PayType.ID);
                if (payObj == null)
                {
                    MessageBox.Show("获得支付方式定义信息出错!" + this.managerIntegrate.Err);

                    return -1;
                }

                //添加farpoint显示内容
                //{4E569A30-8655-4461-86B8-450BD5D245D4}

                Object[] o = new Object[] { false, prepay.RecipeNO, PrepayState, prepay.FT.PrepayCost, payObj.Name, PrepaySource, BalanceFlag, prepay.PrepayOper.Name, prepay.OrgInvoice.ID, prepay.PrepayOper.OperTime.ToString() };

                for (int j = 0; j <= o.GetUpperBound(0); j++)
                {
                    try
                    {
                        fpPrepay_Sheet1.Cells[i, j].Value = o[j];
                    }
                    catch (Exception ex)
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg(ex.Message, 211);
                        return -1;
                    }
                }
                if (prepay.PrepayState != "0")
                    this.fpPrepay_Sheet1.Cells[i, 1].ForeColor = System.Drawing.Color.Red;
                fpPrepay_Sheet1.Rows[i].Tag = prepay;
            }
            return 1;
        }

        /// <summary>
        /// 增加ToolBar控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override ToolBarService OnInit(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("清屏", "清除录入的信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q清空, true, false, null);
            toolBarService.AddToolButton("补打", "补打预交金清单", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印清单, true, false, null);
            return this.toolBarService;
        }

        /// <summary>
        /// 工具栏按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "补打":
                    this.onPrint();
                    break;
                case "清屏":
                    this.Clear();
                    break;
            }
        }

        private void btCheck_Click(object sender, EventArgs e)
        {
            int rowCount = this.fpPrepay_Sheet1.RowCount;

            for (int i = 0; i < rowCount; i++)
            {
                this.fpPrepay_Sheet1.Cells[i, 0].Text = "true";
            }
        }

        private void btUnchek_Click(object sender, EventArgs e)
        {
            int rowCount = this.fpPrepay_Sheet1.RowCount;

            for (int i = 0; i < rowCount; i++)
            {
                this.fpPrepay_Sheet1.Cells[i, 0].Text = "false";
            }
        }


    }
}
