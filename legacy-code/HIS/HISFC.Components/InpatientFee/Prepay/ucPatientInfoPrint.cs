using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;
using System.Collections;

namespace Neusoft.HISFC.Components.InpatientFee.Prepay
{
    public partial class ucPatientInfoPrint : UserControl
    {  

        public ucPatientInfoPrint()
        {
            InitializeComponent();
        }

        #region 变量
        /// <summary>
        /// 住院流水号
        /// </summary>
        string inPatientNo;
        public string InPatientNo
        {
            get { return this.inPatientNo; }
            set { this.inPatientNo = value; }
        }

        /// <summary>
        /// 住院患者信息实体
        /// </summary>
        public  Neusoft.HISFC.Models.RADT.PatientInfo pInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
        Neusoft.HISFC.BizLogic.RADT.InPatient patientMgr = new Neusoft.HISFC.BizLogic.RADT.InPatient();
        Neusoft.HISFC.BizLogic.Fee.InPatient feeMgr = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        private CommonController commonController = CommonController.CreateInstance();

        /// <summary>
        /// 管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        #endregion

        #region 方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this.BackColor = Color.White;
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134))); 
        }

        /// <summary>
        /// 显示患者基本信息
        /// </summary>
        private void DisplayPatient()
        {
            //在控件中显示患者的基本信息
            this.lblPatientNo.Text = this.pInfo.PID.PatientNO; 
            this.lblName.Text = this.pInfo.Name;
            this.lblSEX.Text = this.pInfo.Sex.Name;
            //this.lblDateIn.Text = this.pInfo.PVisit.InTime.ToString("yyyy年MM月dd日");
            this.lblDept.Text = this.pInfo.PVisit.PatientLocation.Dept.Name;
            this.lblPrintOper.Text = this.patientMgr.Operator.ID;
            this.lblPrintDate.Text = this.feeMgr.GetDateTimeFromSysDateTime().ToString("yyyy-MM-dd HH:mm");    

        }

        #endregion

        #region 公有方法
        /// <summary>
        /// 清空标签控件
        /// </summary>
        public void Clear()
        {
            try
            {
                //在控件中显示患者的基本信息
                this.lblPatientNo.Text = "";
                this.lblName.Text = "";
                this.lblSEX.Text = "";
                //this.lblDateIn.Text = "";
                this.lblDept.Text = "";
                this.lblPrintOper.Text = "";
                this.lblPrintDate.Text = "";
                this.neuSpread1_Sheet1.RowCount = 0;
            }
            catch { }
        }

        public void Print()
        {
            if (this.pInfo == null || this.pInfo.ID == "")
            {
                MessageBox.Show("请输入患者住院号,并按回车键确认。", "提示");
                return;
            }

            try
            {
                Neusoft.FrameWork.WinForms.Classes.Print p = new Neusoft.FrameWork.WinForms.Classes.Print();          
                p.ControlBorder = Neusoft.FrameWork.WinForms.Classes.enuControlBorder.None;
                p.IsDataAutoExtend = false;
                p.PrintDocument.DefaultPageSettings.PrinterSettings.PrinterName = "ZYFP";
                Neusoft.HISFC.Models.Base.PageSize ps = (new Neusoft.HISFC.BizLogic.Manager.PageSize()).GetPageSize("PrintPatientInfo");
                p.SetPageSize(ps);
                //p.PrintPreview(this.panel1);
               // MessageBox.Show("系统将选择“住院发票”打印机，请放入正确的纸张！");
                p.PrintPage(ps.Left, ps.Top, this.panel1);
                return;
            }
            catch { }
        }

        #endregion

        private void ucBalanceBill_Load(object sender, System.EventArgs e)
        {
            //初始化控件
            this.Init();
            //清空文本框
            this.Clear();
        }
        /// <summary>
        /// 设置打印患者预交金信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="prePayInfo"></param>
        public void SetPatientInfo(Neusoft.HISFC.Models.RADT.PatientInfo p, ArrayList prePayInfo)
        {
            this.pInfo = p;
            this.DisplayPatient();
            this.AddDateToFP(prePayInfo);
            return;
        }

        /// <summary>
        /// 添加预交金数据到FP
        /// </summary>
        /// <param name="prePayInfo"></param>
        private void AddDateToFP(ArrayList prePayInfo)
        {
            this.neuSpread1_Sheet1.RowCount = 0;
            this.neuSpread1_Sheet1.RowCount = prePayInfo.Count;
            for (int i = 0; i < prePayInfo.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
                prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)prePayInfo[i];

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

                //添加farpoint显示内容

                Object[] o = new Object[] { prepay.RecipeNO, prepay.FT.PrepayCost, prepay.PrepayOper.OperTime.ToString(), prepay.PrepayOper.Name, payObj.Name };

                for (int j = 0; j <= o.GetUpperBound(0); j++)
                {
                    try
                    {
                        this.neuSpread1_Sheet1 .Cells[i, j].Value = o[j];
                    }
                    catch (Exception ex)
                    {
                        Neusoft.FrameWork.WinForms.Classes.Function.Msg(ex.Message, 211);
                        return ;
                    }
                }             
            }
            //显示合计信息
            this.neuSpread1_Sheet1.RowCount = this.neuSpread1_Sheet1.RowCount + 1;
            int rowCount = this.neuSpread1_Sheet1.RowCount;
            this.neuSpread1_Sheet1.Cells[rowCount - 1, 0].Text = "合计";
            this.neuSpread1_Sheet1.Cells[rowCount - 1, 1].Formula = string.Format("SUM(B{0}:B{1})", 1, this.neuSpread1_Sheet1.Rows.Count - 1);
        }

    }
}
