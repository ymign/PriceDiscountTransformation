using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using Neusoft.HISFC.Models.Fee;
using System.Collections;
using Neusoft.HISFC.Models.Fee.Outpatient;
using packagService.DecoctionService;
using Neusoft.HISFC.Components.OutpatientFee.Forms;
using Neusoft.HISFC.Models.Pharmacy;
using Neusoft.HISFC.Models.MedicalTraceCode;
using Neusoft.HISFC.BizLogic.Pharmacy;
using Neusoft.HISFC.Components.Common.Forms.YbTraceCode;
using Neusoft.HISFC.Components.Common.Services.YBTraceCode;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucQuitItemConfirm : ucQuitItemApply, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {
        public ucQuitItemConfirm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 参数控制类
        /// </summary>
        private Neusoft.FrameWork.Management.ControlParam ctlMgr = new Neusoft.FrameWork.Management.ControlParam();

        /// <summary>
        /// 退费单打印接口
        /// </summary>
        private Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBackFeeRecipePrint IBackFeePrint = null;
        /// <summary>
        /// 是否启用取消退药确认
        /// </summary>
        bool isUseCancelQuitConfirm = true;
        [Category("设置"), Description("是否启用取消退药确认")]
        public bool IsUseCancelQuitConfirm
        {
            set
            {
                this.isUseCancelQuitConfirm = value;
            }
            get
            {
                return this.isUseCancelQuitConfirm;
            }
        }

        bool isPrintQuitDrug = true;
        [Category("控件设置"), Description("是否打印退药申请单")]
        public bool IsPrintQuitDrug
        {
            set
            {
                this.isPrintQuitDrug = value;
            }
            get
            {
                return this.isPrintQuitDrug;
            }
        }

        /// <summary>
        /// 是否限制退药天数
        /// </summary>
        bool isLimitBackPha = false;
        [Category("控件设置"), Description("是否限制退药天数")]
        public bool IsLimitBackPha
        {
            set
            {
                this.isLimitBackPha = value;
            }
            get
            {
                return this.isLimitBackPha;
            }
        }

        /// <summary>
        /// 限制退药的时间间隔
        /// </summary>
        int isLimitDays = 3;
        [Category("设置"), Description("限制的退药天数")]
        public int IsLimitDays
        {
            set
            {
                this.isLimitDays = value;
            }
            get
            {
                return this.isLimitDays;
            }
        }

        protected TraceCodeDAL TraceCodeDAL = new TraceCodeDAL();

        #region 方法

        public void ShowModal(UserControl userControl, Form modalForm)
        {
            if (userControl == null || modalForm == null)
                throw new ArgumentNullException();

            // 找到 UserControl 所属的父窗体
            Form parentForm = userControl.FindForm();
            if (parentForm == null)
                throw new InvalidOperationException("UserControl 不在一个有效的 Form 中。");

            // 创建遮罩层
            Panel overlayPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(128, 0, 0, 0), // 半透明黑色
                Visible = true
            };

            // 添加遮罩层到父窗体
            parentForm.Controls.Add(overlayPanel);
            parentForm.Controls.SetChildIndex(overlayPanel, 0); // 放置到顶层

            // 使父窗体的原有界面不被完全遮挡
            parentForm.Opacity = 0.7;  // 设置父窗体的透明度

            try
            {
                // 配置弹出的模态框
                modalForm.StartPosition = FormStartPosition.CenterParent; // 居中显示
                modalForm.ShowDialog(parentForm); // 显示模态框
            }
            finally
            {
                // 移除遮罩层
                parentForm.Controls.Remove(overlayPanel);
                overlayPanel.Dispose();

                // 恢复父窗体的透明度
                parentForm.Opacity = 1.0;
            }
        }
        protected TraceCodeBusinessService TraceCodeBusinessService = new TraceCodeBusinessService();

        /// <summary>
        /// 保存审核信息
        /// </summary>
        /// <returns></returns>
        protected override int Save()
        {


            int infoCounts = 0;

            foreach (FarPoint.Win.Spread.SheetView sv in this.fpSpread2.Sheets)
            {
                for (int i = 0; i < sv.RowCount; i++)
                {
                    if (sv.Rows[i].Tag is ReturnApply)
                    {
                        infoCounts++;
                    }
                }
            }

            if (infoCounts == 0)
            {
                MessageBox.Show("没有需要审核的费用!");

                return -1;
            }

            //在事务开始前进行追溯码采集
            var ybTraceCollectMainList = new List<YbTraceCollectMain>();
            var colResult = CollectReturnDrugtracCodg(ref ybTraceCollectMainList);
            if (colResult == -1)
            {
                return -1;
            }

            DateTime nowTime = this.outpatientManager.GetDateTimeFromSysDateTime();

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            this.outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.pharmacyIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.returnApplyManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            int returnValue = 0;
            ArrayList alBackFeeList = new ArrayList();

            Neusoft.SOC.HISFC.BizLogic.Pharmacy.DrugStore drugStoreMgr = new Neusoft.SOC.HISFC.BizLogic.Pharmacy.DrugStore();
            if (((Neusoft.HISFC.Models.Base.Employee)drugStoreMgr.Operator).Dept.ID.ToString() == "9092")//采芝林中药代煎处方取消
            {
                try
                {
                    packagService.Business.DecoctionMgr XHMgr = new packagService.Business.DecoctionMgr();
                    ReturnApply tempInsert = fpSpread2.Sheets[0].Rows[0].Tag as ReturnApply;
                    packagService.DecoctionService.Service XH = new Service();
                    //ReqCancelRecipeModel reqModel = new ReqCancelRecipeModel();
                    string BillId = tempInsert.RecipeNO;
                    string OrderCode = "";
                    XHMgr.Getinfolog(BillId, ref  OrderCode);
                    string HospitalOrderCode = OrderCode;
                    string Code = "";
                    string Msg = "";
                    XH.CancelRecipe(HospitalOrderCode, ref Code, ref Msg);
                    if (Code != "200")
                    {
                        MessageBox.Show("处方取消不成功，原因是" + Msg);

                        return -1;
                    }
                    else
                    {
                        MessageBox.Show("中药代煎处方取消操作结果：" + Msg);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("处方取消不成功，原因是" + ex.Message);
                    return -1;
                }
            }

            foreach (var item in ybTraceCollectMainList)
            {
                var errMsg = string.Empty;
                var traceResult = this.TraceCodeBusinessService.SaveMZReturnTraceCodeInfo(item, ref errMsg);
                if (traceResult != 1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("追溯码保存失败:" + errMsg);
                    return -1;
                }
            }

            foreach (FarPoint.Win.Spread.SheetView sv in this.fpSpread2.Sheets)
            {
                for (int i = 0; i < sv.RowCount; i++)
                {
                    //{077FF0B0-466D-4d24-B3B2-DDCE4BC7F4BF} 门诊退药确认后可以取消
                    if (sv.Rows[i].Tag is ReturnApply && sv.Cells[i, (int)DrugListQuit.Flag].Text != "确认")
                    {
                        ReturnApply tempInsert = sv.Rows[i].Tag as ReturnApply;
                        Neusoft.HISFC.Models.Pharmacy.ApplyOut applyInfoTmp = pharmacyIntegrate.QueryApplyOut(tempInsert.RecipeNO, tempInsert.SequenceNO);
                        if (applyInfoTmp != null && !string.IsNullOrEmpty(applyInfoTmp.ID))
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(tempInsert.Item.Name + "有未发药确认的项目，请先做发药保存，谢谢！");
                            return -1;
                        }



                        ReturnApply tempExist = this.returnApplyManager.GetReturnApplyByApplySequence(tempInsert.Patient.ID, tempInsert.ID);
                        //找到已经存在数据库的退费申请信息
                        if (tempExist != null)
                        {
                            //if (tempExist.CancelType != Neusoft.HISFC.Models.Base.CancelTypes.Valid)
                            //{
                            //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            //    MessageBox.Show(tempExist.Item.Name + "已经被确认或者作废,请刷新");

                            //    return -1;
                            //}
                            if (tempExist.IsConfirmed)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show(tempExist.Item.Name + "已经被确认或者作废,请刷新");

                                return -1;
                            }
                        }

                        returnValue = this.returnApplyManager.DeleteReturnApply(tempInsert.ID);
                        if (returnValue == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(tempExist.Item.Name + "删除失败!" + this.returnApplyManager.Err);

                            return -1;
                        }

                        tempInsert.ID = this.returnApplyManager.GetReturnApplySequence();
                        tempInsert.IsConfirmed = true;
                        tempInsert.CancelType = Neusoft.HISFC.Models.Base.CancelTypes.Canceled;

                        returnValue = this.returnApplyManager.InsertReturnApply(tempInsert);

                        if (returnValue == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(tempInsert.Item.Name + "审核失败!" + this.returnApplyManager.Err);

                            return -1;
                        }

                        Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = this.outpatientManager.GetFeeItemListBalanced(tempInsert.RecipeNO, tempInsert.SequenceNO);
                        if (feeItemList == null)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(tempInsert.Item.Name + "获得项目失败!" + this.outpatientManager.Err);

                            return -1;
                        }

                        if (feeItemList.Item.Qty < feeItemList.NoBackQty + tempInsert.Item.Qty)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("其他的操作员可能已经审核了" + feeItemList.Item.Name + "请刷新!");

                            return -1;
                        }

                        if (this.isLimitBackPha)
                        {
                            DateTime nowTimeTemp = this.returnApplyManager.GetDateTimeFromSysDateTime();
                            if ((nowTime - feeItemList.FeeOper.OperTime).TotalDays >= this.isLimitDays)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show("其他的操作员可能已经审核了" + feeItemList.Item.Name + "请刷新!");
                                return -1;
                            }
                        }

                        //更新可退数量和确认数量
                        returnValue = this.outpatientManager.UpdateConfirmFlag(tempInsert.RecipeNO, tempInsert.SequenceNO, "1", feeItemList.ConfirmOper.ID, feeItemList.ConfirmOper.Dept.ID, feeItemList.ConfirmOper.OperTime, feeItemList.NoBackQty + tempInsert.Item.Qty,
                            feeItemList.ConfirmedQty - tempInsert.Item.Qty);
                        if (returnValue <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("更新项目:" + feeItemList.Item.Name + "失败!" + this.outpatientManager.Err);

                            return -1;
                        }

                        //if (tempInsert.Item.IsPharmacy) 
                        if (tempInsert.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                        {
                            feeItemList.Item.Qty = tempInsert.Item.Qty;

                            returnValue = this.pharmacyIntegrate.OutputReturn(feeItemList, this.outpatientManager.Operator.ID, nowTime);
                            if (returnValue < 0)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show("药品退库失败!" + this.pharmacyIntegrate.Err);

                                return -1;
                            }

                            alBackFeeList.Add(feeItemList);

                        }
                    }
                }
            }



            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show("审核成功!");

            base.GetItemList();

            if (alBackFeeList.Count > 0)
            {
                //不打印，直接返回
                if (!this.isPrintQuitDrug) return 1;

                if (this.IBackFeePrint == null)
                {
                    this.IBackFeePrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBackFeeRecipePrint)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBackFeeRecipePrint;
                }

                if (this.IBackFeePrint != null)
                {
                    this.IBackFeePrint.Patient = this.patient;

                    this.IBackFeePrint.SetData(alBackFeeList);

                    this.IBackFeePrint.Print();
                }
            }
            return 1;
        }



        private int CollectReturnDrugtracCodg(ref List<YbTraceCollectMain> ybTraceCollectMain)
        {
            var returnApplyList = new List<ReturnApply>();

            foreach (FarPoint.Win.Spread.SheetView sv in this.fpSpread2.Sheets)
            {
                for (int i = 0; i < sv.RowCount; i++)
                {
                    if (sv.Cells[i, (int)DrugListQuit.Flag].Text == "确认")
                    {
                        continue;
                    }
                    if (!(sv.Rows[i].Tag is ReturnApply))
                    {
                        continue;
                    }
                    var item = sv.Rows[i].Tag as ReturnApply;
                    if (item.Item.ItemType != Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        continue;
                    }

                    returnApplyList.Add(item);
                }
            }

            if (!returnApplyList.Any())
            {
                return 1;
            }


            var applyList = new List<PhaComApplyout>();
            foreach (var item in returnApplyList)
            {
                var applyInfo = this.TraceCodeDAL.GetApplyInfo(item.Patient.ID, item.RecipeNO, item.SequenceNO.ToString());

                if (applyInfo == null || string.IsNullOrEmpty(applyInfo.ApplyNumber.ToString()))
                {
                    MessageBox.Show("未找到对应发药申请信息!");
                    return -1;
                }
                if (applyInfo.NeedCollectTraceCodeFlag == "1") 
                {
                    applyList.Add(applyInfo);
                }
                
            }

            if (!applyList.Any(a => a.NeedCollectTraceCodeFlag == "1"))
            {
                return 1;
            }

            if (!applyList.Any(a => a.Alreadycollectqty > 0 || a.AlreadyCollectSpiltQty > 0))
            {
                return 1;
            }
            FrmCollectReturnTraceCode f = new FrmCollectReturnTraceCode();
            f.LoadData(applyList);
                       
            var dialogResult = f.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return -1;
            }
            ybTraceCollectMain = f.YbTraceCollectMainList;
            return 1;

            //frmCollectMZReturnTraceCode f = new frmCollectMZReturnTraceCode();
            //f.InitData(applyList);
            //ybTraceCollectMain = f.YbTraceCollectMainList;
            //var dialogResult = f.ShowDialog();
            //if (dialogResult != DialogResult.OK)
            //{
            //    return -1;
            //}
            //return 1;

        }




        private bool SaveProductReturn(List<ReturnApply> list)
        {
            ProductInventoryDB db = new ProductInventoryDB();
            foreach (var item in list)
            {
                var arr = item.DrugTracCodg.Split(';');
                for (int i = 0; i < arr.Length; i++)
                {
                    var p = GetProductReturn(item.Patient.ID, item.RecipeNO, item.SequenceNO.ToString());
                    p.DrugTracCodg = arr[i].ToString();
                    var result = db.InsertProductInventory(p);
                    if (!result)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private ProductInventory GetProductReturn(string clincCode, string recipeNo, string sequenceNo)
        {
            ProductInventoryDB db = new ProductInventoryDB();
            var p = db.GetProductReturn(clincCode, recipeNo, sequenceNo);
            var aa = alPharPracCertNo.Where(a => a.ID == db.Operator.ID).FirstOrDefault();
            if (aa != null)
            {
                p.PharPracCertNo = aa.Name;
            }
            if (Neusoft.FrameWork.Management.Connection.Hospital.ID == "CORE_HIS502")
            {
                p.FixMedInsCode = "H44040200357";
                p.FixMedInsName = "中山大学珠海校区卫生服务中心";
            }

            p.PharName = db.Operator.Name;
            p.SelRetnOperName = db.Operator.Name;
            p.Opter = db.Operator.ID;
            p.Opter_Name = db.Operator.Name;
            p.CreatedCode = db.Operator.ID;
            p.CreatedName = db.Operator.Name;
            p.OpterCode = db.Operator.ID;
            p.OpterName = db.Operator.Name;
            return p;
        }


        private List<Neusoft.HISFC.Models.Base.Const> alPharPracCertNo = null;
        /// <summary>
        /// Y码与药品标识码对照
        /// </summary>
        private Dictionary<string, HashSet<string>> DrugCodeToTraceCodes = new Dictionary<string, HashSet<string>>();

        private void BatchScanReturnCode()
        {
            frmBatchScanReturn f = new frmBatchScanReturn();

            if (!DrugCodeToTraceCodes.Any())
            {
                var db = new ProductInventoryDB();
                var drugMapList = db.GetDrugCodeMappingList();

                foreach (var mapping in drugMapList)
                {
                    if (!DrugCodeToTraceCodes.ContainsKey(mapping.DrugCode))
                    {
                        DrugCodeToTraceCodes[mapping.DrugCode] = new HashSet<string>();
                    }

                    DrugCodeToTraceCodes[mapping.DrugCode].Add(mapping.IdentifierCode);
                }

            }


            var list = GetReturnDrugList();
            if (list == null || !list.Any())
            {
                MessageBox.Show("请先选择退费数据！");
                return;
            }

            f.DrugCodeToTraceCodes = DrugCodeToTraceCodes;
            f.LoadData(list);

            var dialogResult = f.ShowDialog();  // 弹出窗体

            if (dialogResult == DialogResult.OK)
            {
                UpdateDrugcWhenBatchScan(f.ScanDrugList);
            }

        }

        private void UpdateDrugcWhenBatchScan(List<ScanDrugInfo> scanDrugList)
        {
            foreach (FarPoint.Win.Spread.SheetView sv in this.fpSpread2.Sheets)
            {
                for (int i = 0; i < sv.RowCount; i++)
                {
                    if (sv.Rows[i].Tag is ReturnApply && sv.Cells[i, (int)DrugListQuit.Flag].Text != "确认")
                    {
                        ReturnApply tempInsert = sv.Rows[i].Tag as ReturnApply;

                        if (scanDrugList.Any(f => f.DrugCode == tempInsert.Item.ID))
                        {
                            var info = scanDrugList.FirstOrDefault(f => f.DrugCode == tempInsert.Item.ID);
                            tempInsert.DrugTracCodg = info.DrugTracCodg;

                            sv.Rows[i].Tag = tempInsert;
                        }


                    }
                }
            }
        }

        private List<ScanDrugInfo> GetReturnDrugList()
        {
            var list = new List<ScanDrugInfo>();
            ScanDrugInfo r;

            foreach (FarPoint.Win.Spread.SheetView sv in this.fpSpread2.Sheets)
            {
                for (int i = 0; i < sv.RowCount; i++)
                {
                    //{077FF0B0-466D-4d24-B3B2-DDCE4BC7F4BF} 门诊退药确认后可以取消
                    if (sv.Rows[i].Tag is ReturnApply && sv.Cells[i, (int)DrugListQuit.Flag].Text != "确认")
                    {
                        ReturnApply item = sv.Rows[i].Tag as ReturnApply;
                        if (item.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                        {
                            decimal qty = 0;
                            if (item.FeePack == "1")
                            {
                                qty = Neusoft.FrameWork.Public.String.FormatNumber(item.Item.Qty / item.Item.PackQty, 2);
                            }
                            else
                            {
                                qty = Neusoft.FrameWork.Public.String.FormatNumber(item.Item.Qty, 2);
                            }


                            int applyQtyInt = 0;//这个取得商，是整包装单位的量，必须是整数
                            decimal applyRe = 0;//这个取得余数，是最小单位的量，可能是小数

                            applyQtyInt = (int)(item.Item.Qty / item.Item.PackQty);

                            applyRe = item.Item.Qty - applyQtyInt * item.Item.PackQty;

                            r = new ScanDrugInfo();
                            r.DrugCode = item.Item.ID;
                            r.DrugName = item.Item.Name;
                            r.DrugSpecs = item.Item.Specs;
                            r.PackUnit = item.Item.PriceUnit;
                            r.ScanQty = (int)qty;
                            r.AlreadyScanQty = 0;

                            if (applyQtyInt > 0 && applyRe <= 0)
                            {
                                list.Add(r);
                            }

                        }
                    }
                }
            }

            return list;
        }


        #endregion

        #region 事件

        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
            base.tbQuitCost.Visible = false;
            base.tbReturnCost.Visible = false;
            base.tbQuitCash.Visible = false;
            base.lbLeftCost.Visible = false;
            base.lbQuitCash.Visible = false;
            base.lbReturnCost.Visible = false;

            this.fpSpread1_Sheet1.Columns[(int)DrugList.Cost].Visible = false;
            this.fpSpread1_Sheet2.Columns[(int)UndrugList.Cost].Visible = false;
            this.fpSpread1_Sheet1.Columns[this.fpSpread1_Sheet1.ColumnCount - 1].Visible = false;
            this.fpSpread1_Sheet2.Columns[this.fpSpread1_Sheet2.ColumnCount - 1].Visible = false;

            this.FindForm().Text = "退费审核";

            toolBarService.AddToolButton("退费审核", "审核申请信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.B保存, true, false, null);
            toolBarService.AddToolButton("追溯码扫码", "审核申请信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.A安排, true, false, null);
            toolBarService.AddToolButton("刷新", "重新刷新项目和退费申请信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.B帮助, true, false, null);
            toolBarService.AddToolButton("清空", "清除录入信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q清空, true, false, null);
            toolBarService.AddToolButton("全退", "全部退除所有费用", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q全退, true, false, null);
            return toolBarService;
        }

        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "退费审核":
                    this.Save();
                    break;
                case "追溯码扫码":
                    this.BatchScanReturnCode();
                    break;
                case "刷新":
                    base.GetItemList();
                    break;
                case "清空":
                    base.Clear();
                    break;
                case "全退":
                    base.AllQuit();
                    break;
                default:
                    break;
            }

            base.ToolStrip_ItemClicked(sender, e);
        }
        /// <summary>
        /// 处理取消退费操作
        /// </summary>
        protected virtual void DealCancelQuitOperation(bool isAllowQuitFeeHalf)
        {
            if (this.fpSpread2.ActiveSheet == this.fpSpread2_Sheet1)//药品 
            {
                int currRow = this.fpSpread2_Sheet1.ActiveRowIndex;

                if (fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Flag].Text == "确认")
                {

                    //取消确认

                    if (this.fpSpread2_Sheet1.Rows[currRow].Tag == null)
                    {
                        return;
                    }

                    if (!(this.fpSpread2_Sheet1.Rows[currRow].Tag is Neusoft.HISFC.Models.Fee.ReturnApply))
                    {
                        MessageBox.Show("没有核准药品不能取消!");

                        return;
                    }

                    #region 取消确认前弹出提示框
                    DialogResult dr = MessageBox.Show("是否确认取消" + (this.fpSpread2_Sheet1.Rows[currRow].Tag as Neusoft.HISFC.Models.Fee.ReturnApply).Item.Name + "的退药申请？", "提示", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.No)
                    {
                        return;
                    }

                    #endregion

                    Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                    try
                    {
                        if (isAllowQuitFeeHalf == false)
                        {
                            if (this.fpSpread2_Sheet1.Rows[currRow].Tag is Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList)
                            {
                                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f = this.fpSpread2_Sheet1.Rows[currRow].Tag as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;


                                this.outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                                this.pharmacyIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                                this.returnApplyManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                                //删除met_nui_cancelitem表中的申请信息
                                ReturnApply returnApply = this.fpSpread2_Sheet1.Rows[currRow].Tag as ReturnApply;

                                if (this.returnApplyManager.DeleteReturnApply(returnApply.ID) < 0)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();

                                    return;
                                }

                                //恢复fin_opb_feedetail中的可退数量和已退数量

                                Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = this.outpatientManager.GetFeeItemListBalanced(f.RecipeNO, f.SequenceNO);

                                if (feeItemList == null)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show(feeItemList.Item.Name + "获得项目失败!" + this.outpatientManager.Err);

                                    return;
                                }

                                //if (feeItemList.Item.Qty < feeItemList.NoBackQty + f.Item.Qty)
                                //{
                                //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                //    MessageBox.Show("其他的操作员可能已经审核了" + feeItemList.Item.Name + "请刷新!");

                                //    return -1;
                                //}

                                //更新可退数量和确认数量
                                int returnValue = this.outpatientManager.UpdateConfirmFlag(f.RecipeNO, f.SequenceNO, "0", feeItemList.ConfirmOper.ID, feeItemList.ConfirmOper.Dept.ID, feeItemList.ConfirmOper.OperTime, feeItemList.NoBackQty - f.Item.Qty,
                                       feeItemList.ConfirmedQty);

                                if (returnValue <= 0)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show("更新项目:" + feeItemList.Item.Name + "失败!" + this.outpatientManager.Err);

                                    return;
                                }

                                //扣药品库存表
                                ArrayList al = new ArrayList();
                                al.Add(feeItemList);
                                // public int ApplyOut(Neusoft.HISFC.Models.Registration.Register patient, ArrayList feeAl, string feeWindow, DateTime operDate, bool isModify,out string drugSendInfo)
                                string drugSendInfo = string.Empty;
                                pharmacyIntegrate.ApplyOut(this.patient, al, "", this.outpatientManager.GetDateTimeFromSysDateTime(), true, out drugSendInfo);

                                //界面显示


                                int findRow = FindItem(f.RecipeNO, f.SequenceNO, this.fpSpread1_Sheet1);

                                if (findRow == -1)
                                {
                                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    MessageBox.Show("查找未退药品失败!");

                                    return;
                                }
                                FeeItemList fFind = this.fpSpread1_Sheet1.Rows[findRow].Tag as FeeItemList;
                                fFind.Item.Qty += f.Item.Qty;

                                fFind.ConfirmedQty = fFind.Item.Qty;

                                fFind.NoBackQty += f.Item.Qty;
                                fFind.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(fFind.Item.Price * fFind.Item.Qty / fFind.Item.PackQty, 2) - fFind.FT.RebateCost;
                                fFind.FT.OwnCost = fFind.FT.TotCost;
                                //fFind.FT.TotCost += f.FT.TotCost;
                                //fFind.FT.PubCost += f.FT.PubCost;
                                //fFind.FT.PayCost += f.FT.PayCost;
                                //fFind.FT.OwnCost += f.FT.OwnCost;

                                this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.Amount].Text = fFind.FeePack == "1" ?
                                    Neusoft.FrameWork.Public.String.FormatNumber(fFind.Item.Qty / fFind.Item.PackQty, 2).ToString() :
                                    Neusoft.FrameWork.Public.String.FormatNumber(fFind.Item.Qty, 2).ToString();
                                this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.Cost].Text = fFind.FT.TotCost.ToString();
                                //this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.NoBackQty].Text = fFind.FeePack == "1" ?
                                //    Neusoft.FrameWork.Public.String.FormatNumber(fFind.NoBackQty / fFind.Item.PackQty, 2).ToString() :
                                //    Neusoft.FrameWork.Public.String.FormatNumber(fFind.NoBackQty, 2).ToString();
                                f.Item.Qty = 0;
                                if (f.Item.SysClass.ID.ToString() == "PCC")
                                {
                                    decimal doseOnce = (fFind.NoBackQty) / fFind.Days;

                                    (this.fpSpread1_Sheet1.Rows[findRow].Tag as FeeItemList).Order.DoseOnce = doseOnce;

                                    this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.DoseAndDays].Text = "每次量:" + Neusoft.FrameWork.Public.String.FormatNumberReturnString(doseOnce, 3) + f.Order.DoseUnit + " " + "付数:" + f.Days.ToString();
                                }
                                this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Amount].Text = "0";
                                this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Amount].Text = string.Empty;
                                this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Flag].Text = string.Empty;
                                this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.ItemName].Text = string.Empty;
                                this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.PriceUnit].Text = string.Empty;
                                this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Specs].Text = string.Empty;
                                //{433AA56A-264F-4c8c-BC7E-52DAEAFDC605}
                                this.fpSpread2_Sheet1.Rows[currRow].Tag = null;
                            }
                        }
                        else
                        {
                            Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList fSelect = this.fpSpread2_Sheet1.Rows[currRow].Tag as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                            for (int i = 0; i < this.fpSpread2_Sheet1.RowCount; i++)
                            {
                                currRow = i;

                                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList f = this.fpSpread2_Sheet1.Rows[currRow].Tag as Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList;
                                if (this.fpSpread2_Sheet1.Rows[currRow].Tag is Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList
                                    && (this.IsPharmacySameRecipeQuitAll == false ||
                                            (this.IsPharmacySameRecipeQuitAll == true && f.RecipeNO == fSelect.RecipeNO)
                                          )
                                    )
                                {


                                    this.outpatientManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                                    this.pharmacyIntegrate.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                                    this.returnApplyManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                                    //删除met_nui_cancelitem表中的申请信息
                                    ReturnApply returnApply = this.fpSpread2_Sheet1.Rows[currRow].Tag as ReturnApply;

                                    if (this.returnApplyManager.DeleteReturnApply(returnApply.ID) < 0)
                                    {
                                        Neusoft.FrameWork.Management.PublicTrans.RollBack();

                                        return;
                                    }

                                    //恢复fin_opb_feedetail中的可退数量和已退数量

                                    Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = this.outpatientManager.GetFeeItemListBalanced(f.RecipeNO, f.SequenceNO);

                                    if (feeItemList == null)
                                    {
                                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                        MessageBox.Show(feeItemList.Item.Name + "获得项目失败!" + this.outpatientManager.Err);

                                        return;
                                    }

                                    //if (feeItemList.Item.Qty < feeItemList.NoBackQty + f.Item.Qty)
                                    //{
                                    //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                    //    MessageBox.Show("其他的操作员可能已经审核了" + feeItemList.Item.Name + "请刷新!");

                                    //    return -1;
                                    //}

                                    //更新可退数量和确认数量
                                    feeItemList.ConfirmedQty += f.Item.Qty;
                                    int returnValue = this.outpatientManager.UpdateConfirmFlag(f.RecipeNO, f.SequenceNO, "0", feeItemList.ConfirmOper.ID, feeItemList.ConfirmOper.Dept.ID, feeItemList.ConfirmOper.OperTime, feeItemList.NoBackQty - f.Item.Qty,
                                           feeItemList.ConfirmedQty);

                                    if (returnValue <= 0)
                                    {
                                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                        MessageBox.Show("更新项目:" + feeItemList.Item.Name + "失败!" + this.outpatientManager.Err);

                                        return;
                                    }

                                    //扣药品库存表
                                    ArrayList al = new ArrayList();
                                    al.Add(feeItemList);
                                    // public int ApplyOut(Neusoft.HISFC.Models.Registration.Register patient, ArrayList feeAl, string feeWindow, DateTime operDate, bool isModify,out string drugSendInfo)
                                    string drugSendInfo = string.Empty;
                                    pharmacyIntegrate.ApplyOut(this.patient, al, "", this.outpatientManager.GetDateTimeFromSysDateTime(), true, out drugSendInfo);

                                    //界面显示


                                    int findRow = FindItem(f.RecipeNO, f.SequenceNO, this.fpSpread1_Sheet1);

                                    if (findRow == -1)
                                    {
                                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                        MessageBox.Show("查找未退药品失败!");

                                        return;
                                    }
                                    FeeItemList fFind = this.fpSpread1_Sheet1.Rows[findRow].Tag as FeeItemList;
                                    fFind.Item.Qty += f.Item.Qty;

                                    fFind.ConfirmedQty = fFind.Item.Qty;

                                    fFind.NoBackQty = fFind.FeePack == "1" ?
                                        int.Parse(this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.NoBackQty].Text) * fFind.Item.PackQty :
                                        int.Parse(this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.NoBackQty].Text); //int.Parse(this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.NoBackQty].Text);


                                    fFind.NoBackQty += f.Item.Qty;
                                    fFind.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(fFind.Item.Price * fFind.Item.Qty / fFind.Item.PackQty, 2) - fFind.FT.RebateCost;
                                    fFind.FT.OwnCost = fFind.FT.TotCost;
                                    //fFind.FT.TotCost += f.FT.TotCost;
                                    //fFind.FT.PubCost += f.FT.PubCost;
                                    //fFind.FT.PayCost += f.FT.PayCost;
                                    //fFind.FT.OwnCost += f.FT.OwnCost;

                                    this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.Amount].Text = fFind.FeePack == "1" ?
                                        Neusoft.FrameWork.Public.String.FormatNumber(fFind.Item.Qty / fFind.Item.PackQty, 2).ToString() :
                                        Neusoft.FrameWork.Public.String.FormatNumber(fFind.Item.Qty, 2).ToString();
                                    this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.Cost].Text = fFind.FT.TotCost.ToString();
                                    this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.NoBackQty].Text = fFind.FeePack == "1" ?
                                        Neusoft.FrameWork.Public.String.FormatNumber(fFind.NoBackQty / fFind.Item.PackQty, 2).ToString() :
                                        Neusoft.FrameWork.Public.String.FormatNumber(fFind.NoBackQty, 2).ToString();
                                    // this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.NoBackQty].Text = fFind.NoBackQty.ToString();

                                    f.Item.Qty = 0;
                                    if (f.Item.SysClass.ID.ToString() == "PCC")
                                    {
                                        decimal doseOnce = (fFind.NoBackQty) / fFind.Days;

                                        (this.fpSpread1_Sheet1.Rows[findRow].Tag as FeeItemList).Order.DoseOnce = doseOnce;

                                        this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.DoseAndDays].Text = "每次量:" + Neusoft.FrameWork.Public.String.FormatNumberReturnString(doseOnce, 3) + f.Order.DoseUnit + " " + "付数:" + f.Days.ToString();
                                    }
                                    this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Amount].Text = "0";
                                    this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Amount].Text = string.Empty;
                                    this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Flag].Text = string.Empty;
                                    this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.ItemName].Text = string.Empty;
                                    this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.PriceUnit].Text = string.Empty;
                                    this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Specs].Text = string.Empty;
                                    //{433AA56A-264F-4c8c-BC7E-52DAEAFDC605}
                                    this.fpSpread2_Sheet1.Rows[currRow].Tag = null;
                                }
                            }
                        }
                        Neusoft.FrameWork.Management.PublicTrans.Commit();
                    }
                    catch (Exception ex)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("门诊退药确认后可以取消异常!" + ex.Message.ToString());
                        return;
                    }
                }
                else
                {
                    #region 修改 mad
                    //未核准
                    if (this.fpSpread2_Sheet1.Rows[currRow].Tag == null)
                    {
                        return;
                    }

                    try
                    {
                        Neusoft.HISFC.Models.Fee.ReturnApply temp = this.fpSpread2_Sheet1.Rows[currRow].Tag as Neusoft.HISFC.Models.Fee.ReturnApply;
                        int findRow = FindItem(temp.RecipeNO, temp.SequenceNO, this.fpSpread1_Sheet1);

                        if (findRow == -1)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("查找未退药品失败!");

                            return;
                        }
                        FeeItemList fFind = this.fpSpread1_Sheet1.Rows[findRow].Tag as FeeItemList;
                        fFind.NoBackQty = int.Parse(this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.NoBackQty].Text);
                        fFind.NoBackQty += temp.FeePack == "1" ?
                                        Neusoft.FrameWork.Public.String.FormatNumber(temp.Item.Qty / temp.Item.PackQty, 2) :
                                        Neusoft.FrameWork.Public.String.FormatNumber(temp.Item.Qty, 2);

                        fFind.ConfirmedQty = fFind.FeePack == "1" ? fFind.NoBackQty * fFind.Item.PackQty : fFind.NoBackQty;



                        //fFind.NoBackQty += temp.NoBackQty;
                        //fFind.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(fFind.Item.Price * fFind.Item.Qty / fFind.Item.PackQty, 2) - fFind.FT.RebateCost;
                        //fFind.FT.OwnCost = fFind.FT.TotCost;

                        this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.NoBackQty].Text = (fFind.NoBackQty == 0 ? fFind.Item.Qty : fFind.NoBackQty).ToString();
                        this.fpSpread1_Sheet1.Cells[findRow, (int)DrugList.Cost].Text = fFind.FT.TotCost.ToString();

                        this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.ItemName].Text = "";
                        this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Specs].Text = "";
                        this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.PriceUnit].Text = "";
                        this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Flag].Text = "";
                        this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Price].Text = "";
                        this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Cost].Text = "";
                        this.fpSpread2_Sheet1.Cells[currRow, (int)DrugListQuit.Amount].Text = "";
                        this.fpSpread2_Sheet1.Rows[currRow].Tag = null;

                        this.fpSpread1_Sheet1.Rows[findRow].Tag = fFind;
                    #endregion

                    }
                    catch (Exception)
                    {

                        return;
                    }




                }
            }

            return;
        }
        protected override void fpSpread2_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            //{077FF0B0-466D-4d24-B3B2-DDCE4BC7F4BF} 门诊退药确认后可以取消
            if (this.isUseCancelQuitConfirm)
            {
                if (this.fpSpread2.ActiveSheet.RowCount > 0)
                {
                    //是否允许半退
                    if (IsAllowQuitFeeHalf == false)
                    {
                        this.DealCancelQuitOperation(false);
                        return;
                    }
                    else
                    {
                        this.DealCancelQuitOperation(true);
                    }
                }
            }
            else
            {
                return;
            }
        }

        private Neusoft.HISFC.BizProcess.Integrate.Manager conMgr = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.IsQuitDrugConfirm = true;
            this.ItemType = ItemTypes.Pharmarcy;
            alPharPracCertNo = this.conMgr.GetConstantList("PharPracCertNo").Cast<Neusoft.HISFC.Models.Base.Const>().ToList();
        }
        #endregion

        #region IInterfaceContainer 成员

        public Type[] InterfaceTypes
        {
            get
            {
                Type[] printType = new Type[1];
                printType[0] = typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBackFeeRecipePrint);

                return printType;
            }
        }

        #endregion

    }
}
