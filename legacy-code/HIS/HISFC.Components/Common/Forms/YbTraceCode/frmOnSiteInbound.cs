using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.BizLogic.Pharmacy;
using Neusoft.HISFC.Models.MedicalTraceCode;

namespace Neusoft.HISFC.Components.Common.Forms.YbTraceCode
{
    public partial class frmOnSiteInbound : Form
    {

        protected TraceCodeDAL TraceCodeDAL = new TraceCodeDAL();
        private Neusoft.HISFC.Models.Base.Employee LoginEmployee = new Neusoft.HISFC.Models.Base.Employee();
        public frmOnSiteInbound()
        {
            InitializeComponent();

            Neusoft.FrameWork.WinForms.Classes.Function.EnableDrag(this.pnlTitleBar, this);

            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this, 8);

            Neusoft.FrameWork.WinForms.Classes.Function.ApplyCustomBorder(this, Color.FromArgb(77, 182, 172), 2, 8);

            if (Neusoft.FrameWork.Management.Connection.Operator != null)
            {
                LoginEmployee = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator);

            }


            this.fpSpread1_Sheet1.ColumnHeader.Rows[0].Height = 36;

            Neusoft.FrameWork.WinForms.Classes.Function.SetWebStyleGridLine(this.fpSpread1_Sheet1);

            int currentYear = DateTime.Now.Year;

            this.dateTimeManuDate.Value = new DateTime(currentYear, 1, 1);
            this.dateTimeExpyEnd.Value = new DateTime(currentYear, 12, 31);

        }

        /// <summary>
        /// 追溯码回车事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtScanCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            var scanCode = this.txtScanCode.Text.Trim();

            this.txtScanCode.Clear();
            this.txtScanCode.Focus();


            if (string.IsNullOrEmpty(this.txtManuLotnum.Text))
            {
                MessageBox.Show("请先设置生产批号!");
                return;
            }

            if (string.IsNullOrEmpty(this.txtBchno.Text))
            {
                MessageBox.Show("请先设置批次号!");
                return;
            }

            if (scanCode.Length < 7)
            {
                MessageBox.Show("扫码格式错误，长度不足7位！");
                return;
            }

            var identifiyCode = scanCode.Substring(0, 7);

            var mapList = this.TraceCodeDAL.GetDrugCodeMappingList(identifiyCode);

            if (!mapList.Any())
            {
                MessageBox.Show("未找到标识码对应的药品编码:" + identifiyCode);
                return;

            }

            var drugCode = mapList.FirstOrDefault().DrugCode;
            var drugInfo = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(drugCode);

            if (drugInfo == null || drugInfo.ID != drugCode)
            {
                MessageBox.Show("未找到药品编码对应的药品信息:" + drugCode);
                return;
            }

            var fpInfo = new FP_Info();
            fpInfo.DrugCode = drugInfo.ID;
            fpInfo.DrugName = drugInfo.Name;
            fpInfo.Specs = drugInfo.Specs;
            fpInfo.SpiltQty = drugInfo.PackQty;
            fpInfo.BatchNo = this.txtBchno.Text;
            fpInfo.OperCode = this.TraceCodeDAL.Operator.ID;
            fpInfo.OperName = this.TraceCodeDAL.Operator.Name;
            fpInfo.OperTime = DateTime.Now;
            fpInfo.ManuLotnum = this.txtManuLotnum.Text;
            fpInfo.ExpyEnd = this.dateTimeExpyEnd.Value.ToString("yyyy-MM-dd");
            fpInfo.ManuDate = this.dateTimeManuDate.Value.ToString("yyyy-MM-dd");
            fpInfo.TraceCode = scanCode;

            SetFpInfo(fpInfo);

        }


        private void SetFpInfo(FP_Info info)
        {
            var rowCount = this.fpSpread1_Sheet1.Rows.Count;

            var rowIndex = rowCount;

            this.fpSpread1_Sheet1.Rows.Add(rowIndex, 1);

            this.fpSpread1_Sheet1.Rows[rowIndex].Tag = info;

            this.fpSpread1_Sheet1.Cells[rowIndex, 0].Value = info.DrugName;
            this.fpSpread1_Sheet1.Cells[rowIndex, 0].ForeColor = Color.FromArgb(42, 164, 164);

            this.fpSpread1_Sheet1.Cells[rowIndex, 1].Value = info.Specs;
            this.fpSpread1_Sheet1.Cells[rowIndex, 2].Value = info.TraceCode;
            this.fpSpread1_Sheet1.Cells[rowIndex, 3].Value = info.SpiltQty;
            this.fpSpread1_Sheet1.Cells[rowIndex, 4].Value = info.ManuLotnum;
            this.fpSpread1_Sheet1.Cells[rowIndex, 5].Value = info.BatchNo;
            this.fpSpread1_Sheet1.Cells[rowIndex, 6].Value = info.ExpyEnd;
            this.fpSpread1_Sheet1.Cells[rowIndex, 7].Value = info.ManuDate;


        }

        /// <summary>
        /// 拆零入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSplitInbound_Click(object sender, EventArgs e)
        {
            var inboundNo = this.TraceCodeDAL.GetInboundNo("SM");
            var rowCount = this.fpSpread1_Sheet1.RowCount;
            if (rowCount <= 0)
            {
                MessageBox.Show("暂无数据需要入库!");
                return;
            }

            DialogResult rs = MessageBox.Show("共" + rowCount + "条数据" + "\n是否确认入库？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (rs != DialogResult.Yes)
            {
                return;
            }
            string errMsg = "保存入库失败:";
            //1.获取FP控件数据(待入库数据)
            var fpList = GetFpDataSource();
            if (!fpList.Any())
            {
                MessageBox.Show("获取待入库数据失败!");
                return;
            }


            try
            {
                //开启事务
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

                //3.保存入库相关数据
                foreach (FP_Info fpInfo in fpList)
                {
                    //验证追溯码是否已经入库过
                    if (TraceCodeDAL.IsExistInboundOrder(fpInfo.TraceCode))
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(errMsg + "[" + fpInfo.TraceCode + "]已经存在入库记录!");
                        return;
                    }

                    #region 1.保存入库主表 [yb_trace_inbound_order]

                    var inboundOrder = new YbTraceInboundOrder();

                    inboundOrder.Id = Guid.NewGuid().ToString();
                    inboundOrder.DrugDeptCode = LoginEmployee.Dept.ID;
                    inboundOrder.DrugDeptName = LoginEmployee.Dept.Name;
                    inboundOrder.InboundNo = inboundNo;
                    inboundOrder.SupplierId = "";
                    inboundOrder.SupplierCode = "";
                    inboundOrder.SupplierName = "";

                    var drugInfo = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(fpInfo.DrugCode);

                    inboundOrder.DrugCode = fpInfo.DrugCode;
                    inboundOrder.DrugName = fpInfo.DrugName;
                    inboundOrder.DrugSpecs = fpInfo.Specs;
                    inboundOrder.DrugPactUnit = drugInfo.PackUnit;
                    inboundOrder.DrugPactQty = drugInfo.PackQty.ToString();
                    inboundOrder.DrugMinUnit = drugInfo.MinUnit;

                    inboundOrder.BchNo = fpInfo.BatchNo;
                    inboundOrder.ManuLotnum = fpInfo.ManuLotnum;
                    inboundOrder.ManuDate = Convert.ToDateTime(fpInfo.ManuDate);
                    inboundOrder.ExpyEnd = Convert.ToDateTime(fpInfo.ExpyEnd);

                    inboundOrder.OriginalTraceCode = fpInfo.TraceCode;
                    inboundOrder.OriginalQty = drugInfo.PackQty;
                    inboundOrder.SplitQty = fpInfo.SpiltQty;
                    inboundOrder.Status = "0";
                    inboundOrder.SourceType = "0";

                    inboundOrder.InboundClientIp = Neusoft.FrameWork.WinForms.Classes.Function.GetLocalIP();
                    inboundOrder.InboundOperCode = LoginEmployee.ID;
                    inboundOrder.InboundOperName = LoginEmployee.Name;
                    inboundOrder.InboundOperTime = DateTime.Now;

                    inboundOrder.CreatedCode = LoginEmployee.ID;
                    inboundOrder.CreatedName = LoginEmployee.Name;
                    inboundOrder.CreateTime = DateTime.Now;

                    inboundOrder.IsDeleted = "N";
                    inboundOrder.IsValid = "Y";

                    if (!TraceCodeDAL.InsertYbTraceInboundOrder(inboundOrder))
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(errMsg + TraceCodeDAL.Err);
                        return;
                    }
                    #endregion

                    #region 2.处理库存 [yb_trace_stock]

                    var isExistStockReuslt = true;//= TraceCodeDAL.IsExistStock(inboundOrder.DrugDeptCode, inboundOrder.DrugCode);

                    var stockInfo = this.TraceCodeDAL.GetYbTraceStockInfo(inboundOrder.DrugCode, inboundOrder.DrugDeptCode);
                    if (stockInfo == null || stockInfo.DrugCode != inboundOrder.DrugCode)
                    {
                        isExistStockReuslt = false;
                    }
                    else
                    {
                        isExistStockReuslt = true;
                    }

                    if (isExistStockReuslt)
                    {
                        var updateResult = TraceCodeDAL.UpdateYbTraceStockWhenInboundSucess(
                            inboundOrder.DrugDeptCode,
                            inboundOrder.DrugCode,
                            inboundOrder.SplitQty,
                            inboundOrder.InboundOperCode,
                            inboundOrder.InboundOperName);
                        if (!updateResult)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(errMsg + TraceCodeDAL.Err);
                            return;
                        }

                        //记录库存变化
                        var stockRecordInfo = new YbTraceStockRecord();

                        stockRecordInfo.Id = Guid.NewGuid().ToString();
                        stockRecordInfo.DrugCode = inboundOrder.DrugCode;
                        stockRecordInfo.DrugName = inboundOrder.DrugName;
                        stockRecordInfo.DrugDeptCode = inboundOrder.DrugDeptCode;
                        stockRecordInfo.DrugDeptName = inboundOrder.DrugDeptName;
                        stockRecordInfo.ChangeType = "0";
                        stockRecordInfo.BeforeTotalQty = stockInfo.TotalQty;
                        stockRecordInfo.BeforeAvailableQty = stockInfo.AvailableQty;
                        stockRecordInfo.BeforePredeductedQty = stockInfo.PreDeductedQty;
                        stockRecordInfo.BeforeExpiredQty = stockInfo.ExpiredQty;
                        stockRecordInfo.BeforeDamagedQty = stockInfo.DamagedQty;

                        stockRecordInfo.AfterTotalQty = stockRecordInfo.BeforeTotalQty + fpInfo.SpiltQty;
                        stockRecordInfo.AfterAvailableQty = stockRecordInfo.BeforeAvailableQty + fpInfo.SpiltQty;
                        stockRecordInfo.AfterPredeductedQty = stockRecordInfo.BeforePredeductedQty;
                        stockRecordInfo.AfterExpiredQty = stockRecordInfo.BeforeExpiredQty;
                        stockRecordInfo.AfterDamagedQty = stockRecordInfo.BeforeDamagedQty;
                        stockRecordInfo.RelatedTable = "yb_trace_inbound_order";
                        stockRecordInfo.RelatedId = inboundOrder.Id;
                        stockRecordInfo.RelatedNo = inboundOrder.InboundNo;
                        stockRecordInfo.CreatedCode = inboundOrder.CreatedCode;
                        stockRecordInfo.CreatedName = inboundOrder.CreatedName;

                        if (!this.TraceCodeDAL.InsertYbTraceStockRecord(stockRecordInfo))
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(errMsg + TraceCodeDAL.Err);
                            return;
                        }


                    }
                    else
                    {
                        var stock = new YbTraceStock();

                        stock.Id = Guid.NewGuid().ToString();
                        stock.DrugCode = inboundOrder.DrugCode;
                        stock.DrugName = inboundOrder.DrugName;
                        stock.DrugSpecs = drugInfo.Specs;
                        stock.DrugCustomCode = "";
                        stock.DrugPactUnit = drugInfo.PackUnit;
                        stock.DrugPactQty = drugInfo.PackQty.ToString();
                        stock.DrugMinUnit = drugInfo.MinUnit;
                        stock.DrugDeptCode = inboundOrder.DrugDeptCode;
                        stock.DrugDeptName = inboundOrder.DrugDeptName;

                        stock.TotalQty = inboundOrder.SplitQty;
                        stock.AvailableQty = inboundOrder.SplitQty;
                        stock.PreDeductedQty = 0;
                        stock.ExpiredQty = 0;
                        stock.DamagedQty = 0;
                        stock.FirstInboundTime = DateTime.Now;

                        stock.CreatedCode = inboundOrder.CreatedCode;
                        stock.CreatedName = inboundOrder.CreatedName;
                        stock.CreateTime = DateTime.Now;

                        stock.IsDeleted = "N";
                        stock.IsValid = "Y";

                        stock.Memo = "";
                        stock.Backup1 = "";
                        stock.Backup2 = "";
                        stock.Backup3 = "";

                        var insertResult = TraceCodeDAL.InsertYbTraceStock(stock);
                        if (!insertResult)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(errMsg + TraceCodeDAL.Err);
                            return;
                        }

                        //记录库存变化
                        var stockRecordInfo = new YbTraceStockRecord();

                        stockRecordInfo.Id = Guid.NewGuid().ToString();
                        stockRecordInfo.DrugCode = inboundOrder.DrugCode;
                        stockRecordInfo.DrugName = inboundOrder.DrugName;
                        stockRecordInfo.DrugDeptCode = inboundOrder.DrugDeptCode;
                        stockRecordInfo.DrugDeptName = inboundOrder.DrugDeptName;
                        stockRecordInfo.ChangeType = "0";
                        stockRecordInfo.BeforeTotalQty = 0;
                        stockRecordInfo.BeforeAvailableQty = 0;
                        stockRecordInfo.BeforePredeductedQty = 0;
                        stockRecordInfo.BeforeExpiredQty = 0;
                        stockRecordInfo.BeforeDamagedQty = 0;

                        stockRecordInfo.AfterTotalQty = fpInfo.SpiltQty;
                        stockRecordInfo.AfterAvailableQty = fpInfo.SpiltQty;
                        stockRecordInfo.AfterPredeductedQty = stockRecordInfo.BeforePredeductedQty;
                        stockRecordInfo.AfterExpiredQty = stockRecordInfo.BeforeExpiredQty;
                        stockRecordInfo.AfterDamagedQty = stockRecordInfo.BeforeDamagedQty;
                        stockRecordInfo.RelatedTable = "yb_trace_inbound_order";
                        stockRecordInfo.RelatedId = inboundOrder.Id;
                        stockRecordInfo.RelatedNo = inboundOrder.InboundNo;
                        stockRecordInfo.CreatedCode = inboundOrder.CreatedCode;
                        stockRecordInfo.CreatedName = inboundOrder.CreatedName;

                        if (!this.TraceCodeDAL.InsertYbTraceStockRecord(stockRecordInfo))
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show(errMsg + TraceCodeDAL.Err);
                            return;
                        }

                    }
                    #endregion

                    #region 3.新增种子数据 用于发药时分配子码

                    var traceSeedInfo = new YbTraceSeed();
                    traceSeedInfo.Id = Guid.NewGuid().ToString();
                    traceSeedInfo.InboundOrderId = inboundOrder.Id;
                    traceSeedInfo.InboundOrderNo = inboundOrder.InboundNo;
                    traceSeedInfo.DrugCode = drugInfo.ID;
                    traceSeedInfo.DrugName = drugInfo.Name;
                    traceSeedInfo.DrugDeptCode = inboundOrder.DrugDeptCode;
                    traceSeedInfo.DrugDeptName = inboundOrder.DrugDeptName;
                    traceSeedInfo.DrugPackUnit = drugInfo.PackUnit;
                    traceSeedInfo.DrugPackQty = drugInfo.PackQty.ToString();
                    traceSeedInfo.DrugPackLevel = "0";
                    traceSeedInfo.DrugMinUnit = drugInfo.MinUnit;
                    traceSeedInfo.BatchNo = inboundOrder.BchNo;
                    traceSeedInfo.ParentTraceCode = inboundOrder.OriginalTraceCode;
                    traceSeedInfo.TotalQty = fpInfo.SpiltQty;
                    traceSeedInfo.AvailableQty = fpInfo.SpiltQty;
                    traceSeedInfo.CurrentOffset = 0;
                    traceSeedInfo.SupplierCode = "";
                    traceSeedInfo.SupplierName = "";
                    traceSeedInfo.SeedStatus = "0";
                    traceSeedInfo.CreatedCode = this.LoginEmployee.ID;
                    traceSeedInfo.CreatedName = this.LoginEmployee.Name;
                    traceSeedInfo.IsDeleted = "N";
                    traceSeedInfo.IsValid = "Y";

                    if (!this.TraceCodeDAL.InsertYbTraceSeed(traceSeedInfo))
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show(errMsg + TraceCodeDAL.Err);
                        return;
                    }

                    #endregion


                }

                //提交事务
                Neusoft.FrameWork.Management.PublicTrans.Commit();

                MessageBox.Show("入库成功!" + System.Environment.NewLine + "入库单据号码:[" + inboundNo + "]");

                this.Close();
               
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("保存出现异常:" + ex.Message);
                return;
            }


            
        }

        private List<FP_Info> GetFpDataSource()
        {
            List<FP_Info> list = new List<FP_Info>();
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                var info = this.fpSpread1_Sheet1.Rows[i].Tag as FP_Info;
                if (info != null)
                {
                    list.Add(info);
                }

            }
            return list;
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {

            DialogResult rs = MessageBox.Show("点击确认将清空界面所有数据!" + "\n是否确认清空？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (rs != DialogResult.Yes)
            {
                return;
            }

            this.fpSpread1_Sheet1.RowCount = 0;
            this.txtBchno.Text = "";
            this.txtManuLotnum.Text = "";
            this.txtScanCode.Clear();
            this.txtScanCode.Focus();

        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }




    }

}
