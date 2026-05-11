using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.InpatientFee.Balance
{
    /// <summary>
    /// 住院发票重打、补打控件
    /// </summary>
    public partial class ucReprintInvoice :Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ucReprintInvoice()
        {
            InitializeComponent();
        }
        #endregion

        #region 变量

        #region 业务层实体变量
        /// <summary>
        /// 患者实体
        /// </summary>
        public Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = null;
        /// <summary>
        /// 此次需要重打、补打发票实体
        /// </summary>
        Neusoft.HISFC.Models.Fee.Inpatient.Balance objInvoice = null;
        /// <summary>
        /// 发票实体对应明细
        /// </summary>
        ArrayList arlBalanceList = null;
        /// <summary>
        /// 发票实体对应支付方式
        /// </summary>
        ArrayList arlBalancePay = null;
        /// <summary>
        /// 打印实体
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy printer = null;
        #endregion

        #region 业务层管理变量

        private Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        private Neusoft.FrameWork.Management.ControlParam controlParm = new Neusoft.FrameWork.Management.ControlParam();
        private Neusoft.HISFC.BizLogic.Fee.InPatient feeInpatient = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        private Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        private Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();
        private Neusoft.HISFC.BizLogic.Fee.Derate feeDerate = new Neusoft.HISFC.BizLogic.Fee.Derate();
        private Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareInterfaceProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();

        #endregion
       
        /// <summary>
        /// 工具栏
        /// </summary>
        Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();

        #endregion

        #region 事件
        /// <summary>
        /// 增加ToolBar控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("补打", "补打发票，不走号！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.C重打, true, false, null);
            toolBarService.AddToolButton("重打", "重打发票，作废原发票，产生新的发票号！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D打印, true, false, null);

            return this.toolBarService;
        }
        /// <summary>
        /// 控件初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucBalanceRecall_Load(object sender, EventArgs e)
        {
            this.initControl();
        }
        /// <summary>
        /// 发票号回车事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string invoiceNO = this.txtInvoice.Text.Trim();
                if (!string.IsNullOrEmpty(invoiceNO))
                {
                    invoiceNO = invoiceNO.PadLeft(12, '0');
                    this.txtInvoice.Text = invoiceNO;

                    // 输入发票号后续处理
                    EnterInvoiceNO(invoiceNO);
                }
            }
        }
        /// <summary>
        /// 定义toolbar按钮click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "补打":
                    PrintInvoiceOnly();
                    break;

                case "重打":

                    break;
            }
        }




        #endregion
        

        #region 函数
        /// <summary>
        /// 初始化函数
        /// </summary>
        protected virtual void initControl()
        {
            this.ucQueryInpatientNo1.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

            this.fpBalance_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
        }
        /// <summary>
        /// 清空初始化
        /// </summary>
        protected void ClearInfo()
        {
            this.txtInvoice.SelectAll();
            this.txtInvoice.Tag = "";

            //清空结算费用信息
            this.fpBalance_Sheet1.Rows.Count = 0;
            this.fpBalance_Sheet1.Rows.Count = 14;

            patientInfo = null;
            objInvoice = null;
            arlBalanceList = null;
            arlBalancePay = null;

            this.EvaluteByPatientInfo(patientInfo);
        }
        /// <summary>
        /// 利用患者信息实体进行控件赋值
        /// </summary>
        /// <param name="patientInfo">患者基本信息实体</param>
        protected void EvaluteByPatientInfo(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo)
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
            if (patientInfo.Birthday == DateTime.MinValue)
            {
                this.txtBirthday.Text = string.Empty;
            }
            else
            {
                txtBirthday.Text = patientInfo.Birthday.ToString("yyyy-MM-dd");
            }

            //所属病区
            txtNurseStation.Text = patientInfo.PVisit.PatientLocation.NurseCell.Name;

            //入院日期
            if (patientInfo.PVisit.InTime == DateTime.MinValue)
            {
                this.txtDateIn.Text = string.Empty;
            }
            else
            {
                txtDateIn.Text = patientInfo.PVisit.InTime.ToString();
            }

            // 医生
            txtDoctor.Text = patientInfo.PVisit.AdmittingDoctor.Name;

            //住院号
            this.ucQueryInpatientNo1.TextBox.Text = patientInfo.PID.PatientNO;

        }
        /// <summary>
        /// 发票号回车处理
        /// </summary>
        protected virtual void EnterInvoiceNO(string invoiceNO)
        {
            // 错误信息
            string errText = "";
            // 清空信息
            this.ClearInfo();
            // 获取输入发票号相关信息
            if (this.GetInvoiceInfo(invoiceNO, out this.objInvoice, out this.patientInfo) == -1) return;

            // 赋值
            EvaluteByPatientInfo(this.patientInfo);
            this.txtInvoice.Text = objInvoice.Invoice.ID;
            

            //处理返还补收
            this.ComputeReturnSupply(objInvoice);

            // 显示发票明细信息
            if (QueryAndShowBalanceList(this.objInvoice, out this.arlBalanceList) == -1)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("获取发票明细信息失败！", 211);
                return;
            }

            this.arlBalancePay = feeInpatient.QueryBalancePaysByInvoiceNOAndBalanceNO(this.objInvoice.Invoice.ID, Neusoft.FrameWork.Function.NConvert.ToInt32(this.objInvoice.ID));


            return;
        }
        /// <summary>
        /// 获取输入发票实体与患者实体
        /// </summary>
        /// <param name="invoiceNO">主发票号码</param>
        /// <returns>1成功－1失败</returns>
        protected virtual int GetInvoiceInfo(string invoiceNO, out Neusoft.HISFC.Models.Fee.Inpatient.Balance balance, out Neusoft.HISFC.Models.RADT.PatientInfo patient)
        {
            balance = null;
            patient = null;

            //获取输入发票实体信息
            ArrayList al = new ArrayList();
            al = this.feeInpatient.QueryBalancesByInvoiceNO(invoiceNO);
            if (al == null || al.Count != 1)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("查询指定发票信息失败！", 111);
                return -1;
            }

            balance = al[0] as Neusoft.HISFC.Models.Fee.Inpatient.Balance;

            //通过住院号获取住院基本信息
            patient = this.radtIntegrate.GetPatientInfomation(balance.Patient.ID);

            return 1;
        }
        /// <summary>
        /// 计算返还和补收金额
        /// </summary>
        /// <param name="balance">结算实体</param>
        protected void ComputeReturnSupply(Neusoft.HISFC.Models.Fee.Inpatient.Balance balance)
        {
            if (balance.FT.SupplyCost > 0)//结算时收取患者的费用要返还给患者
            {
                this.gbCost.Text = "返还金额";
                this.txtCash.Text = balance.FT.SupplyCost.ToString("###.00");
                this.txtTot.Text = balance.FT.SupplyCost.ToString("###.00");
            }
            else if (balance.FT.ReturnCost > 0)
            {
                this.gbCost.Text = "补收金额";
                this.txtCash.Text = balance.FT.ReturnCost.ToString("###.00");
                this.txtTot.Text = balance.FT.ReturnCost.ToString("###.00");
            }
            else
            {
                this.gbCost.Text = "收支平衡";
                this.txtCash.Text = "0.00";
                this.txtTot.Text = "0.00";
            }

        }
        /// <summary>
        /// 显示发票明细信息
        /// </summary>
        /// <param name="balance">发票实体</param>
        /// <returns>1成功－1失败</returns>
        protected int QueryAndShowBalanceList(Neusoft.HISFC.Models.Fee.Inpatient.Balance balance, out ArrayList arlBalanceList)
        {
            arlBalanceList = null;
            //获取结算明细信息
            arlBalanceList = feeInpatient.QueryBalanceListsByInvoiceNOAndBalanceNO(balance.Invoice.ID, Neusoft.FrameWork.Function.NConvert.ToInt32(balance.ID));

            if (arlBalanceList == null)
            {
                return -1;
            }

            Neusoft.HISFC.Models.Fee.Inpatient.BalanceList balanceList;

            for (int i = 0; i < arlBalanceList.Count; i++)
            {
                balanceList = (Neusoft.HISFC.Models.Fee.Inpatient.BalanceList)arlBalanceList[i];

                //获取结算人姓名                
                Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();
                employee = this.managerIntegrate.GetEmployeeInfo(balanceList.BalanceBase.BalanceOper.ID);
                if (employee == null)
                {
                    balanceList.BalanceBase.BalanceOper.Name = "";
                }
                else
                {
                    balanceList.BalanceBase.BalanceOper.Name = employee.Name;
                }


                this.fpBalance_Sheet1.Rows.Add(this.fpBalance_Sheet1.Rows.Count, 1);
                //添加结算明细
                this.fpBalance_Sheet1.Cells[i, 0].Value = balanceList.FeeCodeStat.StatCate.Name;
                this.fpBalance_Sheet1.Cells[i, 1].Value = balanceList.BalanceBase.FT.TotCost;
                this.fpBalance_Sheet1.Cells[i, 2].Value = balanceList.BalanceBase.BalanceOper.Name;
                this.fpBalance_Sheet1.Cells[i, 3].Value = balanceList.BalanceBase.BalanceOper.OperTime.ToString();
            }
            this.fpBalance.Tag = arlBalanceList;

            return 1;
        }

        /// <summary>
        /// 补打发票，补打不走号
        /// </summary>
        private void PrintInvoiceOnly()
        {
            if (this.objInvoice == null || this.patientInfo == null)
                return;

            try
            {
                if (printer == null)
                {
                    printer = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy;
                }

                printer.PatientInfo = this.patientInfo;

                printer.SetValueForPrint(patientInfo, this.objInvoice, arlBalanceList, arlBalancePay);

                printer.Print();

                this.ClearInfo();
            }
            catch (Exception objEx)
            {
                MessageBox.Show(objEx.Message);
            }
        }
        /// <summary>
        /// 重打走号
        /// </summary>
        private void PrintInvoiceRollCode()
        {
            if (this.patientInfo == null || this.objInvoice == null)
            {
                return;
            }

            return;




            if (printer == null)
            {
                printer = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy;
            }
            printer.PatientInfo = this.patientInfo;

            printer.SetValueForPrint(patientInfo, this.objInvoice, arlBalanceList, arlBalancePay);

            printer.Print();

            this.ClearInfo();
        }

        #endregion























       
        ///// <summary>
        ///// 确认结算召回主功能函数
        ///// </summary>
        //protected virtual void ExecuteBalanceRecall()
        //{
        //    //错误信息
        //    string errText = "";

        //    //有效性判断
        //    if (this.VerifyExeCuteBalanceRecall() == -1)
        //    {
        //        return;
        //    }

        //    //定义balance实体
        //    Neusoft.HISFC.Models.Fee.Inpatient.Balance balanceMain = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //    balanceMain = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)this.txtInvoice.Tag;

        //    alBalancePay = new ArrayList();
        //    //检索支付方式
        //    alBalancePay = this.feeInpatient.QueryBalancePaysByInvoiceNOAndBalanceNO(balanceMain.Invoice.ID, int.Parse(this.balanceNO));
        //    if (alBalancePay == null) return;

        //    ////在开始事务前处理支付方式，避免t开始后弹出窗口 
        //    //try
        //    //{
        //    //    //判断是否弹出选择框，如果支付方式是一种，并且是现金，不弹出。
        //    //    int payCount = 0;
        //    //    foreach (Neusoft.HISFC.Models.Fee.Inpatient.BalancePay balancePay in alBalancePay)
        //    //    {
        //    //        //因为结算实付表中的预交金支付方式都是"CA"
        //    //        if (balancePay.TransKind.ID == "1")
        //    //        {
        //    //            payCount++;
        //    //        }
        //    //        if (balancePay.PayType.ID!= "CA")
        //    //        {
        //    //            payCount++;
        //    //        }
        //    //    }
        //    //    if (payCount > 1)
        //    //    {
        //    //        //弹出支付方式
        //    //        InpatientFee.Balance.ucPayTypeSelect ucPaySelect = new ucPayTypeSelect();
        //    //        ucPaySelect.AlPayType = alBalancePay;
        //    //        Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(ucPaySelect);
        //    //    }
        //    //}
        //    //catch
        //    //{
        //    //    Neusoft.FrameWork.WinForms.Classes.Function.Msg("处理召回支付方式出错",211);
        //    //    return;
        //    //}
        //    long returnValue = this.medcareInterfaceProxy.SetPactCode(this.patientInfo.Pact.ID);
        //    //建立事务连接
        //    //Neusoft.FrameWork.Management.Transaction t = new Neusoft.FrameWork.Management.Transaction(this.feeInpatient.Connection);
        //    Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
        //    this.feeInpatient.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
        //    this.feeIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
        //    this.managerIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
        //    this.radtIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
        //    this.feeDerate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
           

        //    //获得系统当前时间
        //    DateTime dtSys;
        //    dtSys = this.feeInpatient.GetDateTimeFromSysDateTime();

        //    //新结算序号
        //    string balNO = "";
        //    balNO = feeInpatient.GetNewBalanceNO(this.patientInfo.ID);

        //    if (balNO == "" || balNO == null)
        //    {
        //        errText = "获取新结算序号出错!" + feeInpatient.Err;
        //        goto Error;
        //    }

        //    //处理feeinfo
        //    if (this.RecallFeeInfo(balNO, dtSys) == -1)
        //    {
        //        if (this.feeInpatient.DBErrCode == 1)
        //        {
        //            errText = "并发操作,该患者已经做过召回处理";
        //            goto Error;
        //        }
        //        else
        //        {
        //            errText = "处理feeinfo出错!" + this.feeInpatient.Err;
        //            goto Error;
        //        }
        //    }

        //    //处理费用明细
        //    if (this.RecallItemList(balNO) == -1)
        //    {
        //        errText = "处理费用明细表出错!" + this.feeInpatient.Err;
        //        goto Error;
        //    }

        //   // 处理主表
        //    if (this.RecallInmainInfo(balNO) == -1)
        //    {
        //        errText = "处理住院主表出错!" + this.feeInpatient.Err;
        //        goto Error;
        //    }
        //    //处理预交金
        //    if (this.RecallPrepayInfo(balNO,dtSys) == -1)
        //    {
        //        errText = "结算召回处理预交金！" + this.feeInpatient.Err;
        //        goto Error;
        //    }

        //    //处理结算明细表
        //    if(this.RecallBalanceList(balNO,dtSys)==-1)
        //    {
        //        errText = "处理结算明细出错!"+this.feeInpatient.Err;
        //        goto Error;
        //    }
        //    //处理结算头表
        //    if (this.RecallBalanceHead(balNO, dtSys) == -1)
        //    {
        //        errText = "处理结算头表出错!" + this.feeInpatient.Err;
        //        goto Error;
        //    }
        //    //// 处理主表
        //    //if (this.RecallInmainInfo(balNO) == -1)
        //    //{
        //    //    errText = "处理住院主表出错!" + this.feeInpatient.Err;
        //    //    goto Error;
        //    //}

        //    //处理结算实付表
        //    if (this.RecallBalancePay(balNO, dtSys) == -1)
        //    {
        //        errText = "处理结算实付表出错!" + this.feeInpatient.Err;
        //        goto Error;
        //    }


        //    //更新减免表
        //    if (this.RecallDerateFee(balNO) == -1)
        //    {
        //        errText = "处理减免表出错!" + this.feeDerate.Err;
        //        goto Error;
        //    }

        //    //处理变更记录表
        //    if (this.InsertShiftData(balNO) == -1)
        //    {
        //        errText = "插入变更记录出错!" + this.radtIntegrate.Err;
        //        goto Error;
        //    }
        //    //---------------处理医保——————————————————————－ 
        //    this.medcareInterfaceProxy.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
           
        //    if (returnValue != 1)
        //    {
        //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
        //        this.medcareInterfaceProxy.Rollback();

        //        errText = this.medcareInterfaceProxy.ErrMsg;

        //        goto Error;
        //    }

           

        //    returnValue = this.medcareInterfaceProxy.Connect();
        //    if (returnValue != 1)
        //    {
        //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
        //        this.medcareInterfaceProxy.Rollback();

        //        errText = this.medcareInterfaceProxy.ErrMsg;

        //        goto Error;
        //    }
        //    returnValue = this.medcareInterfaceProxy.GetRegInfoInpatient(this.patientInfo);
        //    if (returnValue != 1)
        //    {
        //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
        //        this.medcareInterfaceProxy.Rollback();

        //        errText = this.medcareInterfaceProxy.ErrMsg;

        //        goto Error;
        //    }
        //    // 
        //    ArrayList alFeeInfo = new ArrayList();
        //    this.patientInfo.SIMainInfo.InvoiceNo = balanceMain.Invoice.ID;
        //    this.patientInfo.SIMainInfo.BalNo = balNO;
        //    this.patientInfo.SIMainInfo.OperDate = dtSys;
        //    this.patientInfo.SIMainInfo.OperInfo.ID = this.feeInpatient.Operator.ID;
        //    returnValue = this.medcareInterfaceProxy.CancelBalanceInpatient(this.patientInfo, ref alFeeInfo);
        //    if (returnValue != 1)
        //    {
        //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
        //        this.medcareInterfaceProxy.Rollback();

        //        errText = this.medcareInterfaceProxy.ErrMsg;

        //        goto Error;
        //    }

        //    returnValue = this.medcareInterfaceProxy.Disconnect();
        //    if (returnValue != 1)
        //    {
        //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
        //        this.medcareInterfaceProxy.Rollback();

        //        errText = this.medcareInterfaceProxy.ErrMsg;

        //        goto Error;
        //    }

        //    Neusoft.FrameWork.Management.PublicTrans.Commit();
        //    this.medcareInterfaceProxy.Commit();
        //    Neusoft.FrameWork.WinForms.Classes.Function.Msg("结算召回成功!",111);
        //    //清空
        //    this.ClearInfo();

        //    return;

        //Error:
        //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
        //    if (errText != "")
        //    {
        //        Neusoft.FrameWork.WinForms.Classes.Function.Msg(errText,211);
        //    }
        //    return;

        //}
        ///// <summary>
        ///// 插入患者变更记录
        ///// </summary>
        ///// <param name="balanceNO">结算序号</param>
        ///// <returns></returns>
        //protected virtual int InsertShiftData(string balanceNO)
        //{
        //    Neusoft.HISFC.Models.Fee.Inpatient.Balance balanceMain = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //    balanceMain = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)this.txtInvoice.Tag;
        //    Neusoft.FrameWork.Models.NeuObject oldObj = new Neusoft.FrameWork.Models.NeuObject();
        //    Neusoft.FrameWork.Models.NeuObject newObj = new Neusoft.FrameWork.Models.NeuObject();
        //    oldObj.ID = balanceMain.ID;
        //    oldObj.Name = "原结算序号";
        //    newObj.ID = balanceNO;
        //    newObj.Name = "新结算序号";
        //    //添加记录
        //    if (this.radtIntegrate.InsertShiftData(this.patientInfo.ID, Neusoft.HISFC.Models.Base.EnumShiftType.BB, "结算召回", oldObj, newObj) == -1)
        //    {
        //        return -1;
        //    }
        //    return 1;
        //}
        ///// <summary>
        ///// 处理召回减免费用
        ///// </summary>
        ///// <param name="balanceNO">结算序号</param>
        ///// <returns>1成功 －1失败</returns>
        //protected virtual int RecallDerateFee(string balanceNO)
        //{

        //    Neusoft.HISFC.Models.Fee.Inpatient.Balance balanceMain = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //    balanceMain = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)this.txtInvoice.Tag;
        //    ArrayList alDerate = new ArrayList();
        //    alDerate = this.feeDerate.GetDerateByClinicAndBalance(this.patientInfo.ID, int.Parse(balanceMain.ID));
        //    if (alDerate == null)
        //    {
        //        return -1;
        //    }

        //    for (int i = 0; i < alDerate.Count; i++)
        //    {
        //        Neusoft.HISFC.Models.Fee.Rate derate = new Neusoft.HISFC.Models.Fee.Rate();
               
        //        derate = (Neusoft.HISFC.Models.Fee.Rate)alDerate[i];
        //        //负记录赋值
        //        derate.derate_Cost = -derate.derate_Cost;
        //        derate.BalanceNo = int.Parse(balanceNO);
        //        if (this.feeDerate.InsertDerate(derate) < 1)
        //        {
        //            return -1;
        //        }
        //        //正记录赋值
        //        derate.derate_Cost = -derate.derate_Cost;
        //        derate.BalanceNo = 0;
        //        derate.balanceState = "0";
        //        derate.invoiceNo = "";
        //        if (this.feeDerate.InsertDerate(derate) < 1)
        //        {
        //            return -1;
        //        }


        //    }
        //    return 1;
        //}
        ///// <summary>
        ///// 结算召回处理结算实付表信息
        ///// </summary>
        ///// <param name="balanceNO">结算序号</param>
        ///// <param name="dtBalanceRecall">结算召回时间</param>
        ///// <returns>1成功 －1失败</returns>
        //protected virtual int RecallBalancePay(string balanceNO, DateTime dtBalanceRecall)
        //{
        //    for (int i = 0; i < alBalancePay.Count; i++)
        //    {
        //        Neusoft.HISFC.Models.Fee.Inpatient.BalancePay balancePay = new Neusoft.HISFC.Models.Fee.Inpatient.BalancePay();
        //        //负记录赋值
        //        balancePay = alBalancePay[i] as Neusoft.HISFC.Models.Fee.Inpatient.BalancePay;
        //        balancePay.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
        //        balancePay.FT.TotCost = -balancePay.FT.TotCost;
        //        balancePay.Qty = -balancePay.Qty;
        //        balancePay.BalanceOper.ID = this.feeInpatient.Operator.ID;
        //        //bPay.PayType.ID = "CA";//召回全部算现金.
        //        balancePay.BalanceOper.OperTime = dtBalanceRecall;
        //        //不进行应收应返的转换 -Delete by Maokb
        //        //				if(bPay.ReturnOrSupplyFlag=="1")
        //        //				{
        //        //					bPay.ReturnOrSupplyFlag="2";
        //        //				}
        //        //				if(bPay.ReturnOrSupplyFlag=="2")
        //        //				{
        //        //					bPay.ReturnOrSupplyFlag="1";
        //        //				}

        //        balancePay.BalanceNO = int.Parse(balanceNO);
        //        if (this.feeInpatient.InsertBalancePay(balancePay) == -1)
        //        {
        //            return -1;
        //        }
        //    }
        //    return 1;
        //}
        ///// <summary>
        ///// 结算召回处理结算主表信息
        ///// </summary>
        ///// <param name="balanceNO">结算序号</param>
        ///// <param name="dtBalanceRecall">结算召回时间</param>
        ///// <returns>1成功 －1失败</returns>
        //protected virtual int RecallBalanceHead(string balanceNO, DateTime dtBalanceRecall)
        //{
        //    for (int i = 0; i < this.alInvoice.Count; i++)
        //    {
        //        Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //        balance = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)alInvoice[i];

        //        //将原有记录更新为作废
        //        if (this.feeInpatient.UpdateBalanceHeadWasteFlag(this.patientInfo.ID, int.Parse(balance.ID), ((int)Neusoft.HISFC.Models.Base.CancelTypes.Canceled).ToString(), dtBalanceRecall, balance.Invoice.ID) < 1)
        //        {
        //            return -1;
        //        }
        //        //负记录赋值
        //        decimal ReturnCost = balance.FT.ReturnCost;
        //        decimal SupplyCost = balance.FT.SupplyCost;
        //        balance.ID = balanceNO;
        //        balance.FT.TotCost = -balance.FT.TotCost;
        //        balance.FT.OwnCost = -balance.FT.OwnCost;
        //        balance.FT.PayCost = -balance.FT.PayCost;
        //        balance.FT.PubCost = -balance.FT.PubCost;
        //        balance.FT.RebateCost = -balance.FT.RebateCost;
        //        balance.FT.DerateCost = -balance.FT.DerateCost;
        //        //balance.FT.ChangePrepay = -balance.FT.ChangePrepay;
        //        balance.FT.TransferTotCost = -balance.FT.TransferTotCost;
        //        balance.FT.TransferPrepayCost = -balance.FT.TransferPrepayCost;
        //        balance.FT.PrepayCost = -balance.FT.PrepayCost;
        //        balance.FT.SupplyCost = ReturnCost;
        //        balance.FT.ReturnCost = SupplyCost;

        //        balance.TransType  = Neusoft.HISFC.Models.Base.TransTypes.Negative;
        //        balance.BalanceOper.ID = this.feeInpatient.Operator.ID;
        //        balance.BalanceOper.OperTime = dtBalanceRecall;
        //        balance.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Canceled;
        //        balance.FinanceGroup.ID = this.feeInpatient.GetFinGroupInfoByOperCode(this.feeInpatient.Operator.ID).ID;
        //        //添加负记录
        //        if (this.feeInpatient.InsertBalance(this.patientInfo, balance) == -1)
        //        {
        //            return -1;
        //        }


        //    }
        //    return 1;
        //}

        ///// <summary>
        /////  结算召回处理结算明细
        ///// </summary>
        ///// <param name="balanceNO">结算序号</param>
        ///// <param name="dtBalanceRecall">结算召回时间</param>
        ///// <returns>1成功 －1失败</returns>
        //protected virtual int RecallBalanceList(string balanceNO, DateTime dtBalanceRecall)
        //{
        //    ArrayList alBalanceList = new ArrayList();
        //    alBalanceList = (ArrayList)this.fpBalance.Tag;

        //    if (alBalanceList == null)
        //    {
        //        this.feeInpatient.Err = "提取结算明细数组出错!";
        //        return -1;
        //    }
        //    for (int i = 0; i < alBalanceList.Count; i++)
        //    {
        //        Neusoft.HISFC.Models.Fee.Inpatient.BalanceList balanceList = new Neusoft.HISFC.Models.Fee.Inpatient.BalanceList();
        //        balanceList = (Neusoft.HISFC.Models.Fee.Inpatient.BalanceList)alBalanceList[i];
        //        //形成负记录
        //        balanceList.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative ;
        //        balanceList.BalanceBase.ID = balanceNO;
        //        balanceList.ID = balanceNO;
        //        balanceList.BalanceBase.FT.TotCost = -balanceList.BalanceBase.FT.TotCost;
        //        balanceList.BalanceBase.FT.OwnCost = -balanceList.BalanceBase.FT.OwnCost;
        //        balanceList.BalanceBase.FT.PayCost = -balanceList.BalanceBase.FT.PayCost;
        //        balanceList.BalanceBase.FT.PubCost = -balanceList.BalanceBase.FT.PubCost;
        //        balanceList.BalanceBase.FT.RebateCost = -balanceList.BalanceBase.FT.RebateCost;
        //        balanceList.BalanceBase.BalanceOper.ID = this.feeInpatient.Operator.ID;
        //        balanceList.BalanceBase.BalanceOper.OperTime = dtBalanceRecall;
        //        //添加负记录
        //        if (this.feeInpatient.InsertBalanceList(this.patientInfo, balanceList) == -1)
        //        {
        //            return -1;
        //        }
        //    }
        //    return 1;
        //}

        ///// <summary>
        ///// 召回结算预交金
        ///// </summary>
        ///// <param name="balanceNO">结算序号</param>
        ///// <param name="dtBalanceRecall">结算召回时间</param>
        ///// <returns>1成功 －1失败</returns>
        //protected virtual int RecallPrepayInfo(string balanceNO,DateTime dtBalanceRecall)
        //{
        //    Neusoft.HISFC.Models.Fee.Inpatient.Balance MainInvoice = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //    MainInvoice = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)this.txtInvoice.Tag;
        //    //判断是否有转押金
        //    if (MainInvoice.FT.TransferPrepayCost!= 0)
        //    {
        //        //计算要插入预交金的金额值
        //        Neusoft.HISFC.Models.Fee.Inpatient.Prepay newPrepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay();
        //        newPrepay.FT.PrepayCost = MainInvoice.FT.PrepayCost - MainInvoice.FT.TransferPrepayCost;

        //        //提取发票号码  //发票类型-预交金
        //        string InvoiceNo = "";
        //        //{7CA01F7B-9DFC-41b7-A9FB-55403CA8B61A}
        //        //InvoiceNo = this.feeIntegrate.GetNewInvoiceNO(Neusoft.HISFC.Models.Fee.EnumInvoiceType.P);
        //        InvoiceNo = this.feeIntegrate.GetNewInvoiceNO("P");

        //        if (InvoiceNo == null || InvoiceNo == "")
        //        {
        //            return -1;
        //        }
        //        //实体赋值
        //        newPrepay.RecipeNO = InvoiceNo;


        //        newPrepay.TransferPrepayOper.ID = this.feeInpatient.Operator.ID;
        //        newPrepay.TransferPrepayOper.OperTime = dtBalanceRecall;
        //        newPrepay.PrepayOper.ID = this.feeInpatient.Operator.ID;
        //        newPrepay.PrepayOper.OperTime = dtBalanceRecall;

        //        //操作员科室
        //        Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();
        //        employee = this.managerIntegrate.GetEmployeeInfo(this.feeInpatient.Operator.ID);

        //        newPrepay.PrepayOper.Dept.ID = employee.Dept.ID;


        //        newPrepay.TransferPrepayBalanceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(balanceNO);
        //        newPrepay.TransferPrepayState = "1";
        //        newPrepay.BalanceState = "0";
        //        newPrepay.PrepayState = "0";
        //        newPrepay.FinGroup.ID = this.feeInpatient.GetFinGroupInfoByOperCode(this.feeInpatient.Operator.ID).ID;//this.OperGrpId;
        //        newPrepay.RecipeNO = "转押金";
        //        newPrepay.PayType.ID = "CA";

        //        //结算召回处理预交金 ext_falg = "2";与正常收退区分，用字段 User01  By Maokb 060804
        //        newPrepay.User01 = "2";
              
        //        //添加转押金记录
        //        if (this.feeInpatient.InsertPrepay(this.patientInfo, newPrepay) < 1) return -1;
               
        //    }
        //    else
        //    {
        //        ArrayList alPrepay = new ArrayList();
        //        alPrepay = (ArrayList)this.fpPrepay.Tag;
        //        if (alPrepay == null)
        //        {
        //            this.feeInpatient.Err = "提取预交金数组出错!";
        //            return -1;
        //        }
        //        for (int i = 0; i < alPrepay.Count; i++)
        //        {
        //            Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay = new Neusoft.HISFC.Models.Fee.Inpatient.Prepay(); 
        //            prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)alPrepay[i];


        //            //将原有记录作废
        //            prepay.PrepayState = "3"; //召回状态是3
        //            if (this.feeInpatient.UpdatePrepayHaveReturned(this.patientInfo, prepay) == -1) return -1;
        //            //负记录赋值
        //            prepay.FT.PrepayCost = -prepay.FT.PrepayCost;
        //            prepay.PrepayState = "1";
        //            //{64BD57CE-9361-41f6-AE91-2618CBA5047A}
        //            prepay.OrgInvoice.ID = prepay.RecipeNO;
        //            prepay.IsPrint = true;
                    
        //            if (this.IsReturnNewInvoice)
        //            {
        //                //生成新的发票号，统一在预交金管理中打印，打印的时候生成发票号
        //                //string InvoiceNo = "";
        //                //InvoiceNo = this.feeIntegrate.GetNewInvoiceNO("P");
        //                //if (InvoiceNo == null || InvoiceNo == "")
        //                //{
        //                //    return -1;
        //                //}
        //                //prepay.RecipeNO = InvoiceNo;
        //                prepay.RecipeNO = "召回发票";
        //                prepay.IsPrint = false;
        //            }
        //            prepay.BalanceNO = int.Parse(balanceNO);
        //            prepay.BalanceOper.ID = this.feeInpatient.Operator.ID;
        //            prepay.BalanceOper.OperTime = dtBalanceRecall;
        //            prepay.PrepayOper.ID = this.feeInpatient.Operator.ID;
        //            prepay.PrepayOper.OperTime = dtBalanceRecall;
        //            prepay.FinGroup.ID = this.feeInpatient.GetFinGroupInfoByOperCode(this.feeInpatient.Operator.ID).ID;
        //            prepay.IsTurnIn = false;
        //            //结算召回处理预交金 ext_falg = "2";与正常收退区分，用字段 User01  By Maokb 060804
        //            prepay.PrepaySourceState = "2";
                    
        //            //添加记录
        //            if (this.feeInpatient.InsertPrepay(this.patientInfo, prepay) == -1) return -1;
        //            //正记录赋值
        //            //{64BD57CE-9361-41f6-AE91-2618CBA5047A}
        //            //正记录使用新发票号的判断
        //            prepay.IsPrint = true;
        //            ////{64BD57CE-9361-41f6-AE91-2618CBA5047A}
        //            if (this.IsSupplyNewInvoice)
        //            {
        //                //生成新的发票号

        //                //提取发票号码
        //                //发票类型-预交金
        //                //{7CA01F7B-9DFC-41b7-A9FB-55403CA8B61A}
        //                //neusoft.HISFC.Models.Fee.InvoiceType invoicetype = new neusoft.HISFC.Models.Fee.InvoiceType();
        //                //invoicetype.ID = neusoft.HISFC.Models.Fee.InvoiceType.enuInvoiceType.P;
        //                //string InvoiceNo = "";
        //                //{7CA01F7B-9DFC-41b7-A9FB-55403CA8B61A}
        //                //InvoiceNo = this.feeIntegrate.GetNewInvoiceNO(Neusoft.HISFC.Models.Fee.EnumInvoiceType.P);
        //                //InvoiceNo = this.feeIntegrate.GetNewInvoiceNO("P");

        //                //if (InvoiceNo == null || InvoiceNo == "")
        //                //{
        //                //    return -1;
        //                //}
        //                //prepay.RecipeNO = InvoiceNo;

        //                prepay.RecipeNO = "召回发票";
        //                prepay.IsPrint = false;
        //            }
        //            prepay.FT.PrepayCost = -prepay.FT.PrepayCost;
        //            prepay.PrepayState = "0";
        //            prepay.OrgInvoice.ID = "";
        //            prepay.BalanceNO = 0;
        //            prepay.BalanceState = "0";
        //            prepay.BalanceOper.ID = "";
        //            prepay.BalanceOper.OperTime = DateTime.MinValue;
        //            prepay.Invoice.ID = "";
        //            //结算召回处理预交金 ext_falg = "2";与正常收退区分，用字段 User01  By Maokb 060804
        //            prepay.PrepaySourceState = "2";
        //            //添加正记录
        //            if (this.feeInpatient.InsertPrepay(this.patientInfo, prepay) == -1) return -1;


        //        }


        //    }


        //    return 1;
        //}

        ///// <summary>
        ///// 召回住院主表信息
        ///// </summary>
        ///// <param name="newBalanceNO">结算序号</param>
        ///// <returns>1成功 -1失败</returns>
        //protected virtual int RecallInmainInfo(string newBalanceNO)
        //{
        //    for (int i = 0; i < this.alInvoice.Count; i++)
        //    {
        //        Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = new Neusoft.HISFC.Models.Fee.Inpatient.Balance() ;
        //        balance = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)alInvoice[i];



        //        //如果为在院结算
        //        if (balance.BalanceType.ID.ToString() == "I")
        //        {
        //            //中途结算不更改在院状态
                   
        //        }
        //        //出院结算
        //        if (balance.BalanceType.ID.ToString() ==Neusoft.HISFC.Models.Fee.EnumBalanceType.O.ToString() )
        //        {
        //            this.patientInfo.PVisit.InState.ID = "B";
                   
        //        }

        //        if (balance.BalanceType.ID.ToString() == Neusoft.HISFC.Models.Fee.EnumBalanceType.Q.ToString())
        //        {
        //            this.patientInfo.PVisit.InState.ID = "B";

        //        }


        //        //中途结算存在转押金时更新住院主表 
        //        //{402C1A7D-6874-441e-B335-37B408C41C16}
        //        if (balance.FT.TransferPrepayCost > 0)
        //        {
        //            if (this.feeInpatient.UpdateInmaininfoMidBalanceRecall(this.patientInfo, int.Parse(newBalanceNO), balance.FT) == -1)
        //                return -1;
        //        }
        //        else
        //        {
        //            if (this.feeInpatient.UpdateInmaininfoBalanceRecall(this.patientInfo, int.Parse(newBalanceNO), balance.FT) == -1)
        //                return -1;
        //        }
        //        //{02B13899-6FE7-4266-AC64-D3C0CDBBBC3F} 婴儿的费用是否可以收取到妈妈身上
        //        Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlParamIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

        //        string motherPayAllFee = controlParamIntegrate.GetControlParam<string>(Neusoft.HISFC.BizProcess.Integrate.SysConst.Use_Mother_PayAllFee, false, "0");
        //        if (motherPayAllFee == "1")//婴儿的费用收在妈妈的身上 
        //        {
        //            ArrayList babyList = this.radtIntegrate.QueryBabiesByMother(this.patientInfo.ID);
        //            if (babyList != null && babyList.Count > 0)
        //            {
        //                foreach (Neusoft.HISFC.Models.RADT.PatientInfo p in babyList)
        //                {
        //                    Neusoft.HISFC.Models.RADT.PatientInfo pTemp = this.radtIntegrate.GetPatientInfomation(p.ID);
        //                    if (pTemp != null && !string.IsNullOrEmpty(pTemp.ID))
        //                    {
        //                        pTemp.PVisit = this.patientInfo.PVisit.Clone();
        //                        if (this.feeInpatient.UpdateInmaininfoBalanceRecall(pTemp, int.Parse(newBalanceNO), new Neusoft.HISFC.Models.Base.FT()) <= 0) return -1;
        //                    }
        //                }
        //            }
        //        }

        //        ////更新主表
             
                    
        //        //if (this.feeInpatient.UpdateInmaininfoBalanceRecall(this.patientInfo, int.Parse(newBalanceNO), balance.FT) == -1)
        //        //    return -1;
        //    }

        //    return 1;
        //}

        ///// <summary>
        ///// 召回费用明细
        ///// </summary>
        ///// <param name="newBalNo">新的结算序号</param>
        ///// <returns>1成功 -1失败</returns>
        //protected virtual int RecallItemList(string newBalNo)
        //{
        //    Neusoft.HISFC.Models.Fee.Inpatient.Balance balanceMain = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //    balanceMain = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)this.txtInvoice.Tag;

        //    //更新非药品明细
        //    if (this.feeInpatient.UpdateFeeItemListsBalanceNO(this.patientInfo.ID, int.Parse(balanceMain.ID), int.Parse(newBalNo)) == -1)
        //    {
        //        return -1;
        //    }
        //    //更新药品明细
        //    if (this.feeInpatient.UpdateMedItemListsBalanceNO(this.patientInfo.ID, int.Parse(balanceMain.ID), int.Parse(newBalNo)) == -1)
        //    {
        //        return -1;
        //    }
        //    return 1;
        //}

        ///// <summary>
        ///// 召回feeinfo信息
        ///// </summary>
        ///// <param name="balNO">结算序号</param>
        ///// <param name="dtBalanceRecall">结算召回时间</param>
        ///// <returns>1成功 -1失败</returns>
        //protected virtual int RecallFeeInfo(string balNO, DateTime dtBalanceRecall)
        //{
        //    Neusoft.HISFC.Models.Fee.Inpatient.Balance balanceMain = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //    balanceMain = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)this.txtInvoice.Tag;
        //    if (balanceMain == null)
        //    {
        //        this.feeInpatient.Err = "获取主发票出错!";
        //        return -1;
        //    }
        //    //检索要召回的费用信息
        //    ArrayList alFeeInfo = feeInpatient.QueryFeeInfosByInpatientNOAndBalanceNO(this.patientInfo.ID, balanceMain.ID);
        //    if (alFeeInfo == null)
        //        return -1;
        //    foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeInfo feeInfo in alFeeInfo)
        //    {
        //        //负记录实体赋值
        //        #region 修改中途结算后收费再召回再结算保存失败错误{A8525B6D-B418-42f7-A839-2FE801C18785}
        //        if (feeInfo.BalanceState == "0")
        //        {
        //            continue;
        //        }
        //        #endregion
        //        feeInfo.FT.TotCost = -feeInfo.FT.TotCost;
        //        feeInfo.FT.OwnCost = -feeInfo.FT.OwnCost;
        //        feeInfo.FT.PayCost = -feeInfo.FT.PayCost;
        //        feeInfo.FT.PubCost = -feeInfo.FT.PubCost;
        //        feeInfo.FT.RebateCost = -feeInfo.FT.RebateCost;
        //        //交易类型
        //        feeInfo.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
        //        //结算人
        //        feeInfo.BalanceOper.ID = this.feeInpatient.Operator.ID;
        //        //结算时间
        //        feeInfo.BalanceOper.OperTime = dtBalanceRecall;
        //        //结算序号
        //        feeInfo.BalanceNO = Neusoft.FrameWork.Function.NConvert.ToInt32(balNO);
        //        //收费人
        //        feeInfo.FeeOper.ID = this.feeInpatient.Operator.ID;
        //        //收费时间
        //        feeInfo.FeeOper.OperTime = dtBalanceRecall;
        //        //插入负记录
        //        if (this.feeInpatient.InsertFeeInfo(this.patientInfo,feeInfo) == -1)
        //        {
        //            return -1;
        //        }
               
        //        //正记录赋值
        //        feeInfo.FT.TotCost = -feeInfo.FT.TotCost;
        //        feeInfo.FT.OwnCost = -feeInfo.FT.OwnCost;
        //        feeInfo.FT.PayCost = -feeInfo.FT.PayCost;
        //        feeInfo.FT.PubCost = -feeInfo.FT.PubCost;
        //        feeInfo.FT.RebateCost = -feeInfo.FT.RebateCost;
        //        //交易类型
        //        feeInfo.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
        //        //结算人
        //        feeInfo.BalanceOper.ID = "";
        //        //结算时间
        //        feeInfo.BalanceOper.OperTime = DateTime.MinValue;
        //        //发票号
        //        feeInfo.Invoice.ID = "";
        //        //结算序号
        //        feeInfo.BalanceNO = 0;
        //        //结算状态
        //        feeInfo.BalanceState = "0";
        //        //插入正记录
        //        if (this.feeInpatient.InsertFeeInfo(this.patientInfo, feeInfo) == -1)
        //        {
        //            return -1;
        //        }

        //    }
        //    return 1;
        //}


        ///// <summary>
        ///// 召回确认有效性判断
        ///// </summary>
        ///// <returns>1有效 －1无效</returns>
        //protected virtual int VerifyExeCuteBalanceRecall()
        //{
        //    //判断患者实体
        //    if (this.patientInfo == null)
        //    {
        //        return -1;
        //    }
        //    if (this.patientInfo.ID == null || this.patientInfo.ID.Trim() == "")
        //    {
        //        return -1;
        //    }
        //    //判断结算发票信息
        //    if (this.alInvoice.Count == 0)
        //    {
        //        return -1;
        //    }

        //    //{E527CF1F-E96D-444f-BE0E-C2777F25A75C}

        //    DateTime dtcurrentDate = this.feeInpatient.GetDateTimeFromSysDateTime();

        //    if (this.isAllowBalancCallBack == false)
        //    {
        //        Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = alInvoice[0] as Neusoft.HISFC.Models.Fee.Inpatient.Balance;

        //        if (balance.BalanceOper.OperTime.Date != dtcurrentDate.Date)
        //        {
        //            MessageBox.Show("发票时间为：" + balance.BalanceOper.OperTime.ToShortDateString()+"\n当前服务器时间："+ dtcurrentDate.ToShortDateString()+",\n\n\n\n不能进行隔日结算召回操作","提示");
        //            return -1;
        //        }

        //    }

        //    return 1;
        //}
        
        ///// <summary>
        ///// 住院号回车处理
        ///// </summary>
        //protected virtual void EnterPatientNO()
        //{
        //    if (this.ucQueryInpatientNo1.InpatientNo == null || this.ucQueryInpatientNo1.InpatientNo == "")
        //    {
        //        Neusoft.FrameWork.WinForms.Classes.Function.Msg("此住院号不存在请重新输入！", 111);
        //        this.ucQueryInpatientNo1.Focus();
        //        return;
        //    }
        //    ArrayList alAllBill = feeInpatient.QueryBalancesByInpatientNO(this.ucQueryInpatientNo1.InpatientNo, "ALL");//出院结算发票。
        //    if (alAllBill == null)
        //    {
        //        Neusoft.FrameWork.WinForms.Classes.Function.Msg("获取发票号出错，" + feeInpatient.Err, 111);
        //        return;
        //    }
        //    if (alAllBill.Count < 1)
        //    {
        //        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者没有已结算的发票,请通过发票号查询!", 111);
        //        return;
        //    }
        //    if (alAllBill.Count == 1)
        //    {
        //        //只结算过一次
        //        Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //        balance = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)alAllBill[0];
        //        this.EnterInvoiceNO(balance.Invoice.ID);
        //        return;
        //    }
        //    if (alAllBill.Count > 1)
        //    {
        //        this.SelectInvoice(alAllBill);

        //        return;
        //    }

        //}
        ///// <summary>
        ///// 选择要召回得票据
        ///// </summary>
        ///// <param name="alInvoice">多组发票</param>
        //protected virtual void SelectInvoice(ArrayList alInvoice)
        //{
        //    Form frmList = new Form();
        //    ListBox list = new ListBox();
            
        //    list.Dock = System.Windows.Forms.DockStyle.Fill;

        //    frmList.Size = new Size(200, 100);
        //    frmList.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;

        //    for (int i = 0; i < alInvoice.Count; i++)
        //    {
        //        Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //        balance = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)alInvoice[i];
        //        list.Items.Add(balance.Invoice.ID);
        //    }
            

        //    list.Visible = true;
        //    //定义选择事件

        //    list.DoubleClick += new EventHandler(list_DoubleClick);
        //    list.KeyDown += new KeyEventHandler(list_KeyDown);


        //    //显示
        //    list.Show();

        //    frmList.Controls.Add(list);
        //    frmList.TopMost = true;
        //    frmList.Show();
        //    frmList.Location = new Point(this.ucQueryInpatientNo1.Parent.Location.X+this.ucQueryInpatientNo1.Location.X+60, this.ucQueryInpatientNo1.Parent.Location.Y+this.ucQueryInpatientNo1.Location.Y + this.ucQueryInpatientNo1.Height + 110);
        //}


        

        ///// <summary>
        ///// //显示本次结算的预交金
        ///// </summary>
        ///// <param name="balanceNO">结算序号</param>
        ///// <returns></returns>
        //private int ShowPrepay(string balanceNO)
        //{
        //    ArrayList alPrepay = feeInpatient.QueryPrepaysByInpatientNOAndBalanceNO(this.patientInfo.ID, balanceNO);
        //    if (alPrepay == null) return -1;

        //    Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay;
        //    for (int i = 0; i < alPrepay.Count; i++)
        //    {
        //        prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)alPrepay[i];

        //        //获取结算人姓名                
        //        Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();
        //        employee = this.managerIntegrate.GetEmployeeInfo(prepay.BalanceOper.ID);
        //        if (employee == null)
        //        {
        //            prepay.BalanceOper.Name = "";
        //        }
        //        else
        //        {
        //            prepay.BalanceOper.Name = employee.Name;
        //        }
                
        //        //获取支付方式name
        //        //prepay.PayType.Name = Function.GetPayTypeIdByName(prepay.PayType.ID.ToString());
        //        //添加一行
        //        this.fpPrepay_Sheet1.Rows.Add(this.fpPrepay_Sheet1.Rows.Count, 1);

        //        this.fpPrepay_Sheet1.Cells[i, 0].Value = prepay.RecipeNO;
        //        this.fpPrepay_Sheet1.Cells[i, 1].Value = prepay.PayType.Name;
        //        this.fpPrepay_Sheet1.Cells[i, 2].Value = prepay.FT.PrepayCost;
        //        this.fpPrepay_Sheet1.Cells[i, 3].Value = prepay.BalanceOper.Name;
        //        this.fpPrepay_Sheet1.Cells[i, 4].Value = prepay.BalanceOper.OperTime;


        //    }
        //    this.fpPrepay.Tag = alPrepay;
        //    return 1;
        //}
        
        
        
        
        
        

     


        

        //#region "事件"

        /// <summary>
        /// 住院号回车事件

        /// </summary>
        private void ucQueryInpatientNo1_myEvent()
        {
            //this.EnterPatientNO();
        }
        

        //private void list_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        try
        //        {
        //            string invoiceNO = "";
        //            ListBox listBox = (ListBox)sender;

        //            invoiceNO = listBox.SelectedItem.ToString();

        //            listBox.Parent.Hide();

        //            if (invoiceNO != "")
        //            {
        //                this.EnterInvoiceNO(invoiceNO);
        //            }
        //            return;
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }
        //    }
        //}

        //private void list_DoubleClick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string invoiceNO = "";
        //        ListBox listBox = (ListBox)sender;
        //        try
        //        {
        //            invoiceNO = listBox.SelectedItem.ToString();
        //        }
        //        catch { }

        //        listBox.Parent.Hide();


        //        if (invoiceNO != "")
        //        {
        //            this.EnterInvoiceNO(invoiceNO);
        //        }
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }


        //}
      
        //#endregion


        //#region 树操作
        ///// <summary>
        ///// 接收树选择的患者基本信息
        ///// </summary>
        ///// <param name="neuObject">患者基本信息实体</param>
        ///// <param name="e"></param>
        ///// <returns></returns>
        //protected override int OnSetValue(object neuObject, TreeNode e)
        //{
        //    this.patientInfo = neuObject as Neusoft.HISFC.Models.RADT.PatientInfo;

        //    if (patientInfo == null || patientInfo.ID == null || patientInfo.ID == "")
        //    {
        //        return -1;
        //    }

        //     EnterPatientNo(this.patientInfo.ID);
        //     return 0;
        //}

        ///// <summary>
        ///// 住院号回车处理

        ///// </summary>
        //protected virtual void EnterPatientNo(string inpatientno)
        //{
        //    this.ClearInfo();
        //    if (inpatientno == null || inpatientno == "")
        //    {
        //        Neusoft.FrameWork.WinForms.Classes.Function.Msg("此住院号不存在请重新输入！", 111);
        //        this.ucQueryInpatientNo1.Focus();
        //        return;
        //    }
        //    ArrayList alAllBill = feeInpatient.QueryBalancesByInpatientNO(inpatientno, "ALL");//出院结算发票。

        //    if (alAllBill == null)
        //    {
        //        Neusoft.FrameWork.WinForms.Classes.Function.Msg("获取发票号出错，" + feeInpatient.Err, 111);
        //        return;
        //    }
        //    if (alAllBill.Count < 1)
        //    {
        //        Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者没有已结算的发票,请通过发票号查询!", 111);
        //        return;
        //    }
        //    if (alAllBill.Count == 1)
        //    {
        //        //只结算过一次

        //        Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
        //        balance = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)alAllBill[0];
        //        this.EnterInvoiceNO(balance.Invoice.ID);
        //        return;
        //    }
        //    if (alAllBill.Count > 1)
        //    {
        //        this.SelectInvoice(alAllBill);

        //        return;
        //    }

        //}

        //#endregion

    }
}
