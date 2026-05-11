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
    /// ucBalanceRecall<br></br>
    /// [功能描述: 发票重打]<br></br>
    /// [创 建 者: maokb]<br></br>
    /// [创建时间: 2008-04-10]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public partial class ucBalanceReprintInvoice : Neusoft.FrameWork.WinForms.Controls.ucBaseControl, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {
        /// <summary>
        /// 构造函数

        /// </summary>
        public ucBalanceReprintInvoice()
        {
            InitializeComponent();
        }


        #region "变量"

        #region "业务层实体变量"

        Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();
        Neusoft.HISFC.Models.Fee.Inpatient.Balance OldBalanceMain = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();


        #endregion
        #region "业务层管理变量"

        private Neusoft.FrameWork.Management.ControlParam controlParm = new Neusoft.FrameWork.Management.ControlParam();
        private Neusoft.HISFC.BizLogic.Fee.InPatient feeInpatient = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        private Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        private Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();

        #endregion

        #region "控制类变量"

        // 是否打印预交金冲红发票

        bool IsPrintReturn = false;
        /// <summary>
        /// 负预交金发票是否走新票号
        /// </summary>
        bool IsReturnNewInvoice = false;
        /// <summary>
        /// 召回是否打印预交金发票

        /// </summary>
        bool IsPrintPrepayInvoice = false;
        /// <summary>
        /// 正记录是否使用新发票号码
        /// </summary>
        bool IsSupplyNewInvoice = false;

        #endregion
        //结算序号
        string balanceNO = "";

        /// <summary>
        /// 发票组头表信息

        /// </summary>
        ArrayList alInvoice = new ArrayList();
        ArrayList alBalancePay = new ArrayList();
        //toolbar控件
        Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();
        #endregion

        #region "函数"

        /// <summary>
        /// 增加ToolBar控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("重打", "重打患者结算发票", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.C重打, true, false, null);
            toolBarService.AddToolButton("帮助", "打开帮助文件", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.B帮助, true, false, null);

            return this.toolBarService;
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
                case "重打":
                    this.ExecuteBalanceRecall();
                    break;
                case "帮助":
                    break;
                //case "退出":
                //    break;}
            }

            base.ToolStrip_ItemClicked(sender, e);
        }
        /// <summary>
        /// 确认结算召回主功能函数

        /// </summary>
        protected virtual void ExecuteBalanceRecall()
        {
            //错误信息
            string errText = "";
            int returnValue = 0;

            //有效性判断

            if (this.VerifyExeCuteBalanceRecall() == -1)
            {
                return;
            }

            //定义balance实体
            Neusoft.HISFC.Models.Fee.Inpatient.Balance balanceMain = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
            balanceMain = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)this.txtInvoice.Tag;            /**
             * 发票重打的处理方式。
             * 更新原发票记录为作废状态，更新原结算明细表、支付方式表为作废状态               * *
              * 更新fin_com_realinvoice作废状态 

             * 插入新的发票信息，新的结算明细，新的支付方式表数据
             * 
             * 更新费用明细，处方明细表发票号为新的发票号，结算序号为新的结算序号
             * 更新医保主表...无论有没有数据都更新。
             */


            Neusoft.FrameWork.Models.NeuObject objOrg = new Neusoft.FrameWork.Models.NeuObject();
            objOrg.ID = balanceMain.Invoice.ID;



            //建立事务连接
            Neusoft.FrameWork.Management.Transaction t = new Neusoft.FrameWork.Management.Transaction();
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();


            //获得系统当前时间
            DateTime dtSys;
            dtSys = this.feeInpatient.GetDateTimeFromSysDateTime();

            //新结算序号

            string balNO = "";
            balNO = feeInpatient.GetNewBalanceNO(this.patientInfo.ID);

            if (balNO == "" || balNO == null)
            {
                errText = "获取新结算序号出错!" + feeInpatient.Err;
                goto Error;
            }
            //领取发票         
            string newInvoiceNO = string.Empty;

            string printInvoiceNo = string.Empty;
            returnValue = this.feeIntegrate.GetInvoiceNO("I", ref newInvoiceNO, ref printInvoiceNo, ref errText);
            if (returnValue != 1)
            {
                goto Error;
            }
            if (string.IsNullOrEmpty(newInvoiceNO))
            {
                errText = "请领取发票！";
                goto Error;
            }
            //更新feeinfo的结算发票号、结算序号：
            returnValue = this.feeInpatient.UpdateFeeInfoInvoiceNO(this.patientInfo.ID,
                 OldBalanceMain.Invoice.ID, OldBalanceMain.ID, newInvoiceNO, balNO);
            if (returnValue < 0)
            {
                errText = "更新feeinfo表出错!" + feeInpatient.Err;
                goto Error;
            }
            //更新itemlist的结算发票号、结算序号：
            if (this.feeInpatient.UpdateFeeItemListInvoiceNO(this.patientInfo.ID,
                OldBalanceMain.Invoice.ID, int.Parse(OldBalanceMain.ID), newInvoiceNO, int.Parse(balNO)) < 0)
            {
                errText = "更新itemlist表出错!" + feeInpatient.Err;
                goto Error;
            }
            //更新medicinelist的结算发票号、结算序号；
            if (this.feeInpatient.UpdateFeeMedListInvoiceNO(this.patientInfo.ID,
                OldBalanceMain.Invoice.ID, OldBalanceMain.ID, newInvoiceNO, balNO) < 0)
            {
                errText = "更新medicinelist表出错!" + feeInpatient.Err;
                goto Error;
            }
            //更新预交金表的发票号、发票流水号；

            if (this.feeInpatient.UpdateFeeInPrepayInvoiceNO(this.patientInfo.ID,
                OldBalanceMain.Invoice.ID, OldBalanceMain.ID, newInvoiceNO, balNO) < 0)
            {
                errText = "更新inprepay表出错!" + feeInpatient.Err;
                goto Error;
            }
            //更新住院主表--结算序号
            if (this.feeInpatient.UpdateInMainInfoBalanceNO(this.patientInfo.ID, int.Parse(balNO)) < 0)
            {
                errText = "更新inmaininfo表出错!" + feeInpatient.Err;
                goto Error;
            }
            //更新医保主表的结算序号、结算发票号；

            Neusoft.HISFC.BizLogic.Fee.Interface SiMgr = new Neusoft.HISFC.BizLogic.Fee.Interface();
            SiMgr.SetTrans(t.Trans);
            string oldBalanceNo = SiMgr.GetBalNo(this.patientInfo.ID);

            if (this.feeInpatient.UpdateFeeSIinmaininfoInvoiceNO(this.patientInfo.ID,
                OldBalanceMain.Invoice.ID, oldBalanceNo, newInvoiceNO, balNO) < 0)
            {
                errText = "更新siinmaininfo表出错!" + feeInpatient.Err;
                goto Error;
            }

            ArrayList alBalanceList = null;

            //处理结算明细表

            if (this.DealBalanceList(OldBalanceMain, newInvoiceNO, balNO, dtSys, out alBalanceList) < 0)
            {
                errText = "" + feeInpatient.Err;
                goto Error;
            }
            // 处理支付方式表

            if (this.DealBalancePay(OldBalanceMain, newInvoiceNO, balNO, dtSys) < 0)
            {
                errText = "" + feeInpatient.Err;
                goto Error;
            }
            ////处理结算头表
            //if (this.DealBalanceHead(OldBalanceMain, newInvoiceNO, balNO, dtSys) < 0)
            //{
            //    errText = "" + feeInpatient.Err;
            //    goto Error;
            //}            
            if (this.feeIntegrate.InsertInvoiceExtend(newInvoiceNO, "I", printInvoiceNo, "00") < 1)
            {
                errText = this.feeIntegrate.Err;
                goto Error;
            }

            //处理结算头表,原来方法没有处理实际发票号
            if (this.DealBalanceHead(OldBalanceMain, newInvoiceNO, printInvoiceNo, balNO, dtSys) < 0)
            {
                errText = "" + feeInpatient.Err;
                goto Error;
            }

            //处理变更记录表

            if (this.InsertShiftData(balNO) == -1)
            {
                errText = "插入变更记录出错!" + this.radtIntegrate.Err;
                goto Error;
            }

            returnValue = this.feeIntegrate.UseInvoiceNO("I", 1, ref newInvoiceNO, ref printInvoiceNo, ref errText);
            if (returnValue != 1)
            {
                errText = "发票走号出错!" + this.feeIntegrate.Err;
                goto Error;
            }


            Neusoft.FrameWork.Management.PublicTrans.Commit();


            //打印发票
            this.PrintInvoice(OldBalanceMain, alBalanceList, alBalancePay);
            Neusoft.FrameWork.WinForms.Classes.Function.Msg("发票重打成功!", 111);
            //清空
            this.ClearInfo();

            return;

        Error:
            Neusoft.FrameWork.Management.PublicTrans.RollBack();
            if (errText != "")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(errText, 211);
            }
            return;

        }
        private int DealBalanceList(Neusoft.HISFC.Models.Fee.Inpatient.Balance Balance, string newInvoiceNO, string newBalanceNO, DateTime dtSys, out ArrayList alBalanceList)
        {
            alBalanceList = new ArrayList();

            alBalanceList = feeInpatient.QueryBalanceListsByInpatientNOAndBalanceNO(this.patientInfo.ID, Balance.Invoice.ID, int.Parse(Balance.ID));

            if (alBalanceList == null)
            {
                this.feeInpatient.Err = "提取结算明细数组出错!" + this.feeInpatient.Err;
                return -1;
            }
            //更新结算明细表中数据
            #region delete
            for (int i = 0; i < alBalanceList.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.BalanceList Blist = alBalanceList[i] as Neusoft.HISFC.Models.Fee.Inpatient.BalanceList;
                if (Blist == null)
                {
                    this.feeInpatient.Err = "转换出错";
                    return -1;
                }
                //形成负记录

                Blist.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;

                Blist.BalanceBase.FT.TotCost = -Blist.BalanceBase.FT.TotCost;
                Blist.BalanceBase.FT.OwnCost = -Blist.BalanceBase.FT.OwnCost;
                Blist.BalanceBase.FT.PayCost = -Blist.BalanceBase.FT.PayCost;
                Blist.BalanceBase.FT.PubCost = -Blist.BalanceBase.FT.PubCost;
                Blist.BalanceBase.FT.RebateCost = -Blist.BalanceBase.FT.RebateCost;
                Blist.BalanceBase.BalanceOper.ID = this.feeInpatient.Operator.ID;
                Blist.BalanceBase.BalanceOper.OperTime = dtSys;
                if (this.feeInpatient.InsertBalanceList(this.patientInfo, Blist) == -1)
                {
                    return -1;
                }
                //形成正记录

                Blist.BalanceBase.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                Blist.BalanceBase.Invoice.ID = newInvoiceNO;
                Blist.ID = newBalanceNO;
                Blist.BalanceBase.FT.TotCost = -Blist.BalanceBase.FT.TotCost;
                Blist.BalanceBase.FT.OwnCost = -Blist.BalanceBase.FT.OwnCost;
                Blist.BalanceBase.FT.PayCost = -Blist.BalanceBase.FT.PayCost;
                Blist.BalanceBase.FT.PubCost = -Blist.BalanceBase.FT.PubCost;
                Blist.BalanceBase.FT.RebateCost = -Blist.BalanceBase.FT.RebateCost;
                Blist.BalanceBase.BalanceOper.ID = this.feeInpatient.Operator.ID;
                Blist.BalanceBase.BalanceOper.OperTime = dtSys;
                Blist.BalanceBase.ID = newBalanceNO;
                if (this.feeInpatient.InsertBalanceList(this.patientInfo, Blist) == -1)
                {
                    return -1;
                }

            }
            #endregion

            return 1;
        }
        /// <summary>
        /// 处理结算头表
        /// </summary>
        /// <param name="Balance"></param>
        /// <param name="newInvoiceNO"></param>
        /// <param name="balNo"></param>
        /// <param name="balNoSec"></param>
        /// <param name="dtSys"></param>
        /// <returns></returns>
        private int DealBalanceHead(Neusoft.HISFC.Models.Fee.Inpatient.Balance Balance, string newInvoiceNO, string newBalanceNO, DateTime dtSys)
        {
            //更新作废标记
            int iReturn = 0;
            iReturn = this.feeInpatient.UpdateBalanceHeadWasteFlag(this.patientInfo.ID, int.Parse(Balance.ID), "2", dtSys, Balance.Invoice.ID);
            if (iReturn <= 0) //并发
            {
                return -1;
            }


            //负记录赋值

            decimal ReturnCost = Balance.FT.ReturnCost;
            decimal SupplyCost = Balance.FT.SupplyCost;
            //Balance.ID = balNo;
            Balance.FT.TotCost = -Balance.FT.TotCost;
            Balance.FT.OwnCost = -Balance.FT.OwnCost;
            Balance.FT.PayCost = -Balance.FT.PayCost;
            Balance.FT.PubCost = -Balance.FT.PubCost;
            Balance.FT.RebateCost = -Balance.FT.RebateCost;
            Balance.FT.DerateCost = -Balance.FT.DerateCost;
            //Balance.FT.ChangePrepay = -Balance.Fee.ChangePrepay;
            //Balance.Fee.ChangeTotCost = -Balance.Fee.ChangeTotCost;
            Balance.FT.TransferPrepayCost = -Balance.FT.TransferPrepayCost;
            Balance.FT.TransferTotCost = -Balance.FT.TransferTotCost;
            Balance.FT.BalancedPrepayCost = -Balance.FT.BalancedPrepayCost;
            Balance.FT.PrepayCost = -Balance.FT.PrepayCost;
            Balance.FT.SupplyCost = ReturnCost;
            Balance.FT.ReturnCost = SupplyCost;

            Balance.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
            Balance.BalanceOper.ID = this.feeInpatient.Operator.ID;
            Balance.Oper.OperTime = dtSys;
            Balance.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
            // Balance.FinGrp.ID = this.OperGrpId;
            Balance.CancelOper.ID = this.feeInpatient.Operator.ID;
            //插入结算头表
            if (this.feeInpatient.InsertBalance(this.patientInfo, Balance) < 1) return -1;


            //正记录赋值

            Balance.ID = newBalanceNO;
            Balance.FT.TotCost = -Balance.FT.TotCost;
            Balance.FT.OwnCost = -Balance.FT.OwnCost;
            Balance.FT.PayCost = -Balance.FT.PayCost;
            Balance.FT.PubCost = -Balance.FT.PubCost;
            Balance.FT.RebateCost = -Balance.FT.RebateCost;
            Balance.FT.DerateCost = -Balance.FT.DerateCost;
            Balance.FT.TransferPrepayCost = -Balance.FT.TransferPrepayCost;
            Balance.FT.TransferTotCost = -Balance.FT.TransferTotCost;
            Balance.FT.BalancedPrepayCost = -Balance.FT.BalancedPrepayCost;
            Balance.FT.PrepayCost = -Balance.FT.PrepayCost;
            Balance.FT.SupplyCost = SupplyCost;
            Balance.FT.ReturnCost = ReturnCost;


            Balance.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
            Balance.BalanceOper.ID = this.feeInpatient.Operator.ID;
            Balance.Oper.OperTime = dtSys;
            Balance.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
            //Balance.FinGrp.ID = this.OperGrpId;
            Balance.Invoice.ID = newInvoiceNO;
            //插入正记录

            //插入结算头表
            if (this.feeInpatient.InsertBalance(this.patientInfo, Balance) < 1) return -1;



            return 1;
        }

        /// <summary>
        /// 处理结算头表
        /// </summary>
        /// <param name="Balance"></param>
        /// <param name="newInvoiceNO"></param>
        /// <param name="balNo"></param>
        /// <param name="balNoSec"></param>
        /// <param name="dtSys"></param>
        /// <returns></returns>
        private int DealBalanceHead(Neusoft.HISFC.Models.Fee.Inpatient.Balance Balance, string newInvoiceNO, string printInvoiceNo, string newBalanceNO, DateTime dtSys)
        {
            //更新作废标记
            int iReturn = 0;
            iReturn = this.feeInpatient.UpdateBalanceHeadWasteFlag(this.patientInfo.ID, int.Parse(Balance.ID), "2", dtSys, Balance.Invoice.ID);
            if (iReturn <= 0) //并发
            {
                return -1;
            }


            //负记录赋值

            decimal ReturnCost = Balance.FT.ReturnCost;
            decimal SupplyCost = Balance.FT.SupplyCost;
            //Balance.ID = balNo;
            Balance.FT.TotCost = -Balance.FT.TotCost;
            Balance.FT.OwnCost = -Balance.FT.OwnCost;
            Balance.FT.PayCost = -Balance.FT.PayCost;
            Balance.FT.PubCost = -Balance.FT.PubCost;
            Balance.FT.RebateCost = -Balance.FT.RebateCost;
            Balance.FT.DerateCost = -Balance.FT.DerateCost;
            //Balance.FT.ChangePrepay = -Balance.Fee.ChangePrepay;
            //Balance.Fee.ChangeTotCost = -Balance.Fee.ChangeTotCost;
            Balance.FT.TransferPrepayCost = -Balance.FT.TransferPrepayCost;
            Balance.FT.TransferTotCost = -Balance.FT.TransferTotCost;
            Balance.FT.BalancedPrepayCost = -Balance.FT.BalancedPrepayCost;
            Balance.FT.PrepayCost = -Balance.FT.PrepayCost;
            Balance.FT.SupplyCost = ReturnCost;
            Balance.FT.ReturnCost = SupplyCost;

            Balance.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
            Balance.BalanceOper.ID = this.feeInpatient.Operator.ID;
            Balance.Oper.OperTime = dtSys;
            Balance.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Reprint;
            // Balance.FinGrp.ID = this.OperGrpId;
            Balance.CancelOper.ID = this.feeInpatient.Operator.ID;
            //插入结算头表
            if (this.feeInpatient.InsertBalance(this.patientInfo, Balance) < 1) return -1;


            //正记录赋值

            Balance.ID = newBalanceNO;
            Balance.FT.TotCost = -Balance.FT.TotCost;
            Balance.FT.OwnCost = -Balance.FT.OwnCost;
            Balance.FT.PayCost = -Balance.FT.PayCost;
            Balance.FT.PubCost = -Balance.FT.PubCost;
            Balance.FT.RebateCost = -Balance.FT.RebateCost;
            Balance.FT.DerateCost = -Balance.FT.DerateCost;
            Balance.FT.TransferPrepayCost = -Balance.FT.TransferPrepayCost;
            Balance.FT.TransferTotCost = -Balance.FT.TransferTotCost;
            Balance.FT.BalancedPrepayCost = -Balance.FT.BalancedPrepayCost;
            Balance.FT.PrepayCost = -Balance.FT.PrepayCost;
            Balance.FT.SupplyCost = SupplyCost;
            Balance.FT.ReturnCost = ReturnCost;


            Balance.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
            Balance.BalanceOper.ID = this.feeInpatient.Operator.ID;
            Balance.Oper.OperTime = dtSys;
            Balance.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Valid;
            //Balance.FinGrp.ID = this.OperGrpId;
            Balance.Invoice.ID = newInvoiceNO;
            Balance.PrintedInvoiceNO = printInvoiceNo;
            //插入正记录

            //插入结算头表
            if (this.feeInpatient.InsertBalance(this.patientInfo, Balance) < 1) return -1;



            return 1;
        }
        /// <summary>
        /// 处理结算实付表

        /// </summary>
        /// <param name="BalNo"></param>
        /// <param name="dtSys"></param>
        /// <returns></returns>
        private int DealBalancePay(Neusoft.HISFC.Models.Fee.Inpatient.Balance Balance, string newInvoiceNo, string newBalanceNO, DateTime dtSys)
        {

            //检索支付方式

            alBalancePay = this.feeInpatient.QueryBalancePaysByInvoiceNOAndBalanceNO(Balance.Invoice.ID, int.Parse(this.balanceNO));
            if (alBalancePay == null) return -1;

            foreach (Neusoft.HISFC.Models.Fee.Inpatient.BalancePay bPay in alBalancePay)
            {

                //负记录赋值

                bPay.TransType = Neusoft.HISFC.Models.Base.TransTypes.Negative;
                bPay.FT.TotCost = -bPay.FT.TotCost;
                bPay.Qty = -bPay.Qty;
                bPay.BalanceOper.ID = this.feeInpatient.Operator.ID;
                bPay.BalanceOper.OperTime = dtSys;
                bPay.BalanceNO = int.Parse(Balance.ID);
                if (this.feeInpatient.InsertBalancePay(bPay) == -1)
                {
                    return -1;
                }
                //正记录赋值


                bPay.TransType = Neusoft.HISFC.Models.Base.TransTypes.Positive;
                bPay.FT.TotCost = -bPay.FT.TotCost;
                bPay.Qty = -bPay.Qty;
                bPay.BalanceOper.ID = this.feeInpatient.Operator.ID;
                bPay.BalanceOper.OperTime = dtSys;
                bPay.Invoice.ID = newInvoiceNo;
                //				if(bPay.ReturnOrSupplyFlag=="1")
                //				{
                //					bPay.ReturnOrSupplyFlag="2";
                //				}
                //				if(bPay.ReturnOrSupplyFlag=="2")
                //				{
                //					bPay.ReturnOrSupplyFlag="1";
                //				}
                bPay.BalanceNO = int.Parse(newBalanceNO);
                if (this.feeInpatient.InsertBalancePay(bPay) == -1)
                {
                    return -1;
                }

            }
            return 1;
        }
        /// <summary>
        /// 插入患者变更记录

        /// </summary>
        /// <param name="balanceNO">结算序号</param>
        /// <returns></returns>
        protected virtual int InsertShiftData(string balanceNO)
        {
            Neusoft.HISFC.Models.Fee.Inpatient.Balance balanceMain = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
            balanceMain = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)this.txtInvoice.Tag;
            Neusoft.FrameWork.Models.NeuObject oldObj = new Neusoft.FrameWork.Models.NeuObject();
            Neusoft.FrameWork.Models.NeuObject newObj = new Neusoft.FrameWork.Models.NeuObject();
            oldObj.ID = balanceMain.ID;
            oldObj.Name = "原结算序号";
            newObj.ID = balanceNO;
            newObj.Name = "新结算序号";
            //添加记录
            if (this.radtIntegrate.InsertShiftData(this.patientInfo.ID, Neusoft.HISFC.Models.Base.EnumShiftType.BB, "发票重打", oldObj, newObj) == -1)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 召回确认有效性判断

        /// </summary>
        /// <returns>1有效 －1无效</returns>
        protected virtual int VerifyExeCuteBalanceRecall()
        {
            //判断患者实体

            if (this.patientInfo == null)
            {
                return -1;
            }
            if (this.patientInfo.ID == null || this.patientInfo.ID.Trim() == "")
            {
                return -1;
            }
            //判断结算发票信息
            if (this.alInvoice.Count == 0)
            {
                return -1;
            }

            return 1;
        }
        /// <summary>
        /// 初始化函数

        /// </summary>
        protected virtual void initControl()
        {
            this.ucQueryInpatientNo1.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

            try
            {

                this.ReadControlInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            //try
            //{
            //    //提取操作员的财务编码
            //    this.OperGrp = this.myFee.GetOperGrp(this.FormParent.var.User.ID);
            //    if (this.OperGrp != null)
            //    {
            //        OperGrpId = OperGrp.ID;
            //        OperGrpName = OperGrp.Name;
            //    }
            //}
            //catch { }
            this.fpBalance_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.fpPrepay_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
        }
        /// <summary>
        /// 读取控制类参数

        /// </summary>
        /// <returns>1成功 －1失败</returns>
        protected virtual int ReadControlInfo()
        {
            try
            {

                //召回是否打印预交金发票

                this.IsPrintPrepayInvoice = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.controlParm.QueryControlerInfo("100013"));
                //是否打印预交金冲红发票

                this.IsPrintReturn = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.controlParm.QueryControlerInfo("100015"));
                //负预交金发票是否走新票号
                this.IsReturnNewInvoice = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.controlParm.QueryControlerInfo("100016"));
                //正记录是否使用新发票号码
                this.IsSupplyNewInvoice = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.controlParm.QueryControlerInfo("100019"));
            }
            catch
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("读取控制类信息出错!", 211);
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// 住院号回车处理

        /// </summary>
        protected virtual void EnterPatientNO()
        {
            if (this.ucQueryInpatientNo1.InpatientNo == null || this.ucQueryInpatientNo1.InpatientNo == "")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("此住院号不存在请重新输入！", 111);
                this.ucQueryInpatientNo1.Focus();
                return;
            }
            ArrayList alAllBill = feeInpatient.QueryBalancesByInpatientNO(this.ucQueryInpatientNo1.InpatientNo, "ALL");//出院结算发票。

            if (alAllBill == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("获取发票号出错，" + feeInpatient.Err, 111);
                return;
            }
            if (alAllBill.Count < 1)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者没有已结算的发票,请通过发票号查询!", 111);
                return;
            }
            if (alAllBill.Count == 1)
            {
                //只结算过一次

                Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
                balance = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)alAllBill[0];
                this.EnterInvoiceNO(balance.Invoice.ID);
                return;
            }
            if (alAllBill.Count > 1)
            {
                this.SelectInvoice(alAllBill);

                return;
            }

        }
        /// <summary>
        /// 选择要召回得票据
        /// </summary>
        /// <param name="alInvoice">多组发票</param>
        protected virtual void SelectInvoice(ArrayList alInvoice)
        {
            Form frmList = new Form();
            ListBox list = new ListBox();

            list.Dock = System.Windows.Forms.DockStyle.Fill;

            frmList.Size = new Size(200, 100);
            frmList.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;

            for (int i = 0; i < alInvoice.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
                balance = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)alInvoice[i];
                list.Items.Add(balance.Invoice.ID);
            }


            list.Visible = true;
            //定义选择事件

            list.DoubleClick += new EventHandler(list_DoubleClick);
            list.KeyDown += new KeyEventHandler(list_KeyDown);


            //显示
            list.Show();

            frmList.Controls.Add(list);
            frmList.TopMost = true;
            frmList.Show();
            frmList.Location = new Point(this.ucQueryInpatientNo1.Parent.Location.X + this.ucQueryInpatientNo1.Location.X + 60, this.ucQueryInpatientNo1.Parent.Location.Y + this.ucQueryInpatientNo1.Location.Y + this.ucQueryInpatientNo1.Height + 110);
        }


        /// <summary>
        /// 发票号回车处理

        /// </summary>
        protected virtual void EnterInvoiceNO(string invoiceNO)
        {
            //错误信息
            string errText = "";
            //    清空信息
            this.ClearInfo();
            //    获取输入发票号相关信息

            if (this.GetInvoiceInfo(invoiceNO) == -1) return;

            //找到主发票信息

            if (alInvoice.Count == 1)
            {
                OldBalanceMain = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)alInvoice[0];
            }
            else
            {
                //Neusoft.HISFC.Models.Fee.Inpatient.Balance balanceTemp;
                //for (int i=0;i<alInvoice.Count;i++)
                //{
                //    balanceTemp = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)alInvoice[i];
                //    if (balanceTemp.IsMainInvoice)
                //    {
                //        balanceMain = balanceTemp;
                //    }
                //}

                //对于多张发票的问题，暂时不处理...
                errText = "该发票存在相关联的发票，暂时无法处理，请联系信息科";
                goto Error;

            }
            //如果发票的结算日期不是当天，不允许重打....
            DateTime dtsys = this.feeInpatient.GetDateTimeFromSysDateTime();
            if (OldBalanceMain.BalanceOper.OperTime.Date != dtsys.Date)
            {
                errText = "不允许重打非当天发票！";
                goto Error;
            }

            //如果当前操作员不是收取操作员，不允许重打
            if (this.feeInpatient.Operator.ID != OldBalanceMain.BalanceOper.ID)
            {
                errText = "当前操作员不是原结算操作员，不允许重打！";
                goto Error;
            }
            //处理返还补收
            this.ComputeReturnSupply(OldBalanceMain);
            //显示本次可以召回的预交金
            if (this.ShowPrepay(OldBalanceMain.ID) == -1)
            {
                errText = this.feeInpatient.Err;
                goto Error;
            }
            //显示本次召回的balancelist信息
            if (this.ShowBalanceList(OldBalanceMain.ID) == -1)
            {
                errText = this.feeInpatient.Err;
                goto Error;
            }

            this.txtInvoice.Tag = OldBalanceMain;
            balanceNO = OldBalanceMain.ID;

            return;

        Error:
            this.alInvoice = new ArrayList();
            this.patientInfo.ID = null;
            if (errText != "")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg(errText, 211);
            }
            return;
        }

        /// <summary>
        /// 显示本次召回的balancelist信息
        /// </summary>
        /// <param name="balanceNO">结算序号</param>
        /// <returns>1成功－1失败</returns>
        protected virtual int ShowBalanceList(string balanceNO)
        {
            //获取结算明细信息
            ArrayList alBalanceList = feeInpatient.QueryBalanceListsByInpatientNOAndBalanceNO(this.patientInfo.ID, Neusoft.FrameWork.Function.NConvert.ToInt32(balanceNO));

            if (alBalanceList == null)
            {
                return -1;
            }

            Neusoft.HISFC.Models.Fee.Inpatient.BalanceList balanceList;

            for (int i = 0; i < alBalanceList.Count; i++)
            {
                balanceList = (Neusoft.HISFC.Models.Fee.Inpatient.BalanceList)alBalanceList[i];

                //获取结算人姓名                
                Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();
                //employee = this.managerIntegrate.GetEmployeeInfo(balanceList.BalanceBase.BalanceOper.ID);
                //if (employee == null)
                //{
                //    balanceList.BalanceBase.BalanceOper.Name = "";
                //}
                //else
                //{
                //    balanceList.BalanceBase.BalanceOper.Name = employee.Name;
                //}


                this.fpBalance_Sheet1.Rows.Add(this.fpBalance_Sheet1.Rows.Count, 1);
                //添加结算明细
                this.fpBalance_Sheet1.Cells[i, 0].Value = balanceList.FeeCodeStat.StatCate.Name;
                this.fpBalance_Sheet1.Cells[i, 1].Value = balanceList.BalanceBase.FT.TotCost;
                this.fpBalance_Sheet1.Cells[i, 2].Value = balanceList.BalanceBase.BalanceOper.ID;
                this.fpBalance_Sheet1.Cells[i, 3].Value = balanceList.BalanceBase.BalanceOper.OperTime.ToString();



            }
            this.fpBalance.Tag = alBalanceList;


            return 1;
        }

        /// <summary>
        /// //显示本次结算的预交金
        /// </summary>
        /// <param name="balanceNO">结算序号</param>
        /// <returns></returns>
        private int ShowPrepay(string balanceNO)
        {
            ArrayList alPrepay = feeInpatient.QueryPrepaysByInpatientNOAndBalanceNO(this.patientInfo.ID, balanceNO);
            if (alPrepay == null) return -1;

            Neusoft.HISFC.Models.Fee.Inpatient.Prepay prepay;
            for (int i = 0; i < alPrepay.Count; i++)
            {
                prepay = (Neusoft.HISFC.Models.Fee.Inpatient.Prepay)alPrepay[i];

                ////获取结算人姓名                
                //Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();
                //employee = this.managerIntegrate.GetEmployeeInfo(prepay.BalanceOper.ID);
                //if (employee == null)
                //{
                //    prepay.BalanceOper.Name = "";
                //}
                //else
                //{
                //    prepay.BalanceOper.Name = employee.Name;
                //}

                //获取支付方式name
                //prepay.PayType.Name = Function.GetPayTypeIdByName(prepay.PayType.ID.ToString());
                //添加一行

                this.fpPrepay_Sheet1.Rows.Add(this.fpPrepay_Sheet1.Rows.Count, 1);

                this.fpPrepay_Sheet1.Cells[i, 0].Value = prepay.RecipeNO;
                this.fpPrepay_Sheet1.Cells[i, 1].Value = prepay.PayType.Name;
                this.fpPrepay_Sheet1.Cells[i, 2].Value = prepay.FT.PrepayCost;
                this.fpPrepay_Sheet1.Cells[i, 3].Value = prepay.BalanceOper.Name;
                this.fpPrepay_Sheet1.Cells[i, 4].Value = prepay.BalanceOper.ID;


            }
            this.fpPrepay.Tag = alPrepay;
            return 1;
        }

        /// <summary>
        /// 计算返还和补收金额

        /// </summary>
        /// <param name="balance">结算实体</param>
        protected virtual void ComputeReturnSupply(Neusoft.HISFC.Models.Fee.Inpatient.Balance balance)
        {
            if (balance.FT.SupplyCost > 0)//结算时收取患者的费用要返还给患者
            {
                //this.GlPayFlag="-1";
                this.gbCost.Text = "返还金额";
                this.txtCash.Text = balance.FT.SupplyCost.ToString("###.00");
                this.txtTot.Text = balance.FT.SupplyCost.ToString("###.00");
            }
            else if (balance.FT.ReturnCost > 0)
            {
                //this.GlPayFlag="1";
                this.gbCost.Text = "补收金额";
                this.txtCash.Text = balance.FT.ReturnCost.ToString("###.00");
                this.txtTot.Text = balance.FT.ReturnCost.ToString("###.00");
            }
            else
            {
                //this.GlPayFlag="0";
                this.gbCost.Text = "收支平衡";
                this.txtCash.Text = "0.00";
                this.txtTot.Text = "0.00";
            }

        }
        /// <summary>
        /// 获取输入发票相关信息
        /// </summary>
        /// <param name="invoiceNO">主发票号码</param>
        /// <returns>1成功－1失败</returns>
        protected virtual int GetInvoiceInfo(string invoiceNO)
        {
            //判断发票号是否有效

            if (this.VerifyInvoice(invoiceNO) == -1)
            {
                return -1;
            }
            //获取输入发票实体信息
            ArrayList al = new ArrayList();
            al = this.feeInpatient.QueryBalancesByInvoiceNO(invoiceNO);

            Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = new Neusoft.HISFC.Models.Fee.Inpatient.Balance();
            balance = (Neusoft.HISFC.Models.Fee.Inpatient.Balance)al[0];

            if (this.VerifyAllowReCall(balance) == false)
            {
                return -1;
            }

            //通过住院号获取住院基本信息

            this.patientInfo = this.radtIntegrate.GetPatientInfomation(balance.Patient.ID);

            //判断出院结算后不允许进行中途结算召回

            if (balance.BalanceType.ID.ToString() == "I" &&
                this.patientInfo.PVisit.InState.ID.ToString() == "O")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("该患者已经出院,不能进行中途结算召回", 111);
                return -1;
            }
            if (balance.BalanceType.ID.ToString() == "D")
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("直接结算不能进行结算召回操作!", 111);
                return -1;
            }
            //单项打印数据结束时间和开始时间完全一样，其他中途结算不会出现这种情况

            if (balance.BeginTime == balance.EndTime)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("不能重打单项打印票据!", 111);
                return -1;
            }


            //获得发票列表,通过一组发票中的某一张,获得balance_no的其他发票;
            alInvoice = this.feeInpatient.QueryBalancesByBalanceNO(balance.Patient.ID, Neusoft.FrameWork.Function.NConvert.ToInt32(balance.ID));
            if (alInvoice == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("获得发票列表出错!" + this.feeInpatient.Err, 211);
                this.txtInvoice.SelectAll();
                return -1;
            }
            //判断是否有发票组
            if (alInvoice.Count > 1)
            {
                DialogResult r = MessageBox.Show("该笔结算有" + alInvoice.Count.ToString() + "张发票,召回操作会对所有这些发票进行召回,是否继续?",
                    "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.No)
                {
                    this.alInvoice = new ArrayList();
                    this.patientInfo.ID = null;
                    return -1;
                }
            }

            //赋值

            this.EvaluteByPatientInfo(patientInfo);

            this.txtInvoice.Text = invoiceNO;

            return 1;
        }
        /// <summary>
        /// 清空初始化

        /// </summary>
        protected virtual void ClearInfo()
        {
            this.txtInvoice.SelectAll();
            this.txtInvoice.Tag = "";

            //清空结算预交金

            this.fpPrepay_Sheet1.Rows.Count = 0;
            this.fpPrepay_Sheet1.Rows.Count = 14;
            //清空结算费用信息
            this.fpBalance_Sheet1.Rows.Count = 0;
            this.fpBalance_Sheet1.Rows.Count = 14;

            this.alInvoice.Clear();
            this.patientInfo.ID = null;
            this.EvaluteByPatientInfo(null);
            //this.GlPayFlag = "-1";
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
        /// //验证输入发票号是否有效: 
        /// </summary>
        /// <param name="invoiceNo">发票号</param>
        /// <returns>1成功 －1失败</returns>
        protected virtual int VerifyInvoice(string invoiceNo)
        {
            //验证输入发票号是否有效: 
            ArrayList al = new ArrayList();
            al = this.feeInpatient.QueryBalancesByInvoiceNO(invoiceNo);
            if (al == null)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("发票号不存在,请重新录入" + this.feeInpatient.Err, 111);
                this.txtInvoice.SelectAll();
                return -1;
            }
            if (al.Count == 0)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.Msg("发票号不存在,请重新录入" + this.feeInpatient.Err, 211);
                this.txtInvoice.SelectAll();
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 回车判断判断是否允许进行结算召回－－－－－

        ///      （为继承后本地特殊化使用）

        /// 
        /// </summary>
        /// <param name="balance">主发票信息实体</param>
        /// <returns>ture允许false不允许</returns>
        protected virtual bool VerifyAllowReCall(Neusoft.HISFC.Models.Fee.Inpatient.Balance balance)
        {
            ////如果该笔结算的结算操作员还未作日结，不允许其他操作员召回--by Maokb
            //if (this.FormParent.var.User.ID != balance.BalanceOper.ID)
            //{
            //    //获取操作员上次日结日期

            //    neusoft.HISFC.Management.Fee.FeeReport feeRep = new neusoft.HISFC.Management.Fee.FeeReport();

            //    string RepDate = feeRep.GetMaxTimeDayReport(balance.BalanceOper.ID);
            //    if (neusoft.neuFC.Function.NConvert.ToDateTime(RepDate) < balance.DtBalance)
            //    {
            //        MessageBox.Show("此患者的原结算操作员[" + balance.BalanceOper.ID +
            //            "]还没作结，必须原操作员召回！");
            //        return -1;
            //    }
            //}

            return true;
        }

        private void PrintInvoice(Neusoft.HISFC.Models.Fee.Inpatient.Balance balance, ArrayList alBalanceListHead, ArrayList alBalancePay)
        {
            Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy p = null;
            if (alBalanceListHead.Count > 0)
            {

                if (this.patientInfo.IsEncrypt)
                {
                    this.patientInfo.Name = Neusoft.FrameWork.WinForms.Classes.Function.Decrypt3DES(this.patientInfo.NormalName);
                }
                p = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(typeof(ucBalance), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy;
                p.PatientInfo = this.patientInfo;

                if (p.SetValueForPrint(this.patientInfo, balance, alBalanceListHead, alBalancePay) == -1)
                {
                    alBalanceListHead = new ArrayList();
                    return;
                }
                //调打印类
                p.Print();
            }
        }

        #endregion

        #region "事件"
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
        /// 住院号回车事件

        /// </summary>
        private void ucQueryInpatientNo1_myEvent()
        {
            this.EnterPatientNO();
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
                string invoiceNO = "";
                invoiceNO = this.txtInvoice.Text.Trim().PadLeft(12, '0');


                //输入发票号后续处理

                this.EnterInvoiceNO(invoiceNO);
            }

        }

        private void list_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    string invoiceNO = "";
                    ListBox listBox = (ListBox)sender;

                    invoiceNO = listBox.SelectedItem.ToString();

                    listBox.Parent.Hide();

                    if (invoiceNO != "")
                    {
                        this.EnterInvoiceNO(invoiceNO);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string invoiceNO = "";
                ListBox listBox = (ListBox)sender;
                try
                {
                    invoiceNO = listBox.SelectedItem.ToString();
                }
                catch { }

                listBox.Parent.Hide();


                if (invoiceNO != "")
                {
                    this.EnterInvoiceNO(invoiceNO);
                }
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        #endregion

        #region IInterfaceContainer 成员

        public Type[] InterfaceTypes
        {
            get
            {
                Type[] type = new Type[1];
                type[0] = typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBalanceInvoicePrintmy);

                return type;
            }
        }

        #endregion
    }
}
