using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using Neusoft.HISFC.BizLogic.Pharmacy;
using Neusoft.HISFC.Components.Common.Services.YBTraceCode;
using Neusoft.HISFC.Models.SqlSugar;
using Neusoft.FrameWork.WinForms;
using Neusoft.HISFC.Models.MedicalTraceCode;
using Neusoft.HISFC.Components.Common.Controls.ModernStyles;
using Neusoft.HISFC.Components.Common.Classes;

namespace Neusoft.HISFC.Components.Common.Forms.YbTraceCode
{
    public partial class frmStockTraceCode : Form
    {

        #region 属性

        /// <summary>
        /// 数据访问类
        /// </summary>
        protected TraceCodeDAL TraceCodeDAL = new TraceCodeDAL();

        /// <summary>
        /// 已经扫描的追溯码
        /// </summary>
        private HashSet<string> AlreadyScannedTraceCodes = new HashSet<string>();

        private Neusoft.HISFC.Models.Base.Employee LoginEmployee = new Neusoft.HISFC.Models.Base.Employee();

        #endregion

        public Dictionary<string, HashSet<string>> GetMap()
        {
            return DrugCodeMappingCache.GetOrLoad(() => TraceCodeDAL.GetDrugCodeToIdentifierCodesMap());
        }

        public Dictionary<string, PhaComBaseinfo> GetDrugBaseInfoMap()
        {
            return DrugBaseInfoCache.GetOrLoad(() => TraceCodeDAL.GetDrugBaseInfoMap());
        }

        public PhaComBaseinfo GetDrugInfo(string drugCode)
        {
            var map = GetDrugBaseInfoMap();
            if (map == null || string.IsNullOrEmpty(drugCode))
                return null;

            PhaComBaseinfo drugInfo;
            map.TryGetValue(drugCode, out drugInfo);
            return drugInfo;
        }


        public frmStockTraceCode()
        {
            InitializeComponent();

            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.btnClear, 4);
            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.btnSaveInboundOrder, 4);
            this.fpSpread1_Sheet1.ColumnHeader.Rows[0].Height = 50;
            this.fpSpread2_Sheet1.ColumnHeader.Rows[0].Height = 50;

            SetWebStyleGridLine(this.fpSpread1_Sheet1);
            SetWebStyleGridLine(this.fpSpread2_Sheet1);

            BeautifyTextBox(this.txtBoxQuery, "搜索药品名称或入库单号...", null);

            DrugCodeMappingCache.SetTtlMinutes(30);
            DrugBaseInfoCache.SetTtlMinutes(100);

            Clear();

            Init();
        }

        private void Init()
        {
            var inboundNo = TraceCodeDAL.GetInboundNo("SM");
            this.lblInboundNo.Text = inboundNo;

            if (Neusoft.FrameWork.Management.Connection.Operator != null)
            {
                LoginEmployee = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator);

            }

            this.lblTitle.Text = "药品追溯码拆零库录入界面[" + LoginEmployee.Dept.Name + "]";

            InitFp2Data();
            this.fpSpread2.CellClick += OnFP2CellClick;
            this.fpSpread1.CellClick += OnFP1CellClick;

            this.txtManuLotnum.Text = "2024-01-01";
            this.txtBchno.Text = "2024-01-01";
            this.dateTimeManuDate.Value = DateTime.Parse("2024-01-01");
            this.dateTimeExpyEnd.Value = DateTime.Parse("2026-12-31");

        }

        /// <summary>
        /// 扫码回车事件
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

            if (AlreadyScannedTraceCodes.Contains(scanCode))
            {
                MessageBox.Show("[" + scanCode + "]已经扫码使用过,不要重复拆零!");
                return;
            }

            var rowCount = this.fpSpread1_Sheet1.RowCount;
            //if (rowCount > 10)
            //{
            //    MessageBox.Show("一次入库暂不允许超过10个药品!");
            //    return;
            //}

            var identifiyCode = scanCode.Substring(0, 7);

            // 1) 先查缓存
            var map = GetMap();
            string drugCode = null;

            if (map != null && map.Count > 0)
            {
                foreach (var kv in map)
                {
                    if (kv.Value != null && kv.Value.Contains(identifiyCode))
                    {
                        drugCode = kv.Key;
                        break;
                    }
                }
            }

            // 2) 未命中则刷新缓存（查数据库）再查一次
            if (string.IsNullOrEmpty(drugCode))
            {
                DrugCodeMappingCache.ForceReload(() => TraceCodeDAL.GetDrugCodeToIdentifierCodesMap());

                map = GetMap();
                if (map != null && map.Count > 0)
                {
                    foreach (var kv in map)
                    {
                        if (kv.Value != null && kv.Value.Contains(identifiyCode))
                        {
                            drugCode = kv.Key;
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(drugCode))
            {
                MessageBox.Show("未找到标识码对应的药品编码:" + identifiyCode);
                return;
            }

            var drugInfo = GetDrugInfo(drugCode);

            if (drugInfo == null)
            {
                MessageBox.Show("未找到药品编码对应的药品信息:" + drugCode);
                return;
            }

            var fpInfo = new FP_Info();
            fpInfo.DrugCode = drugInfo.DrugCode;
            fpInfo.DrugName = drugInfo.TradeName;
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

            AlreadyScannedTraceCodes.Add(scanCode);
        }

        private void SetFpInfo(FP_Info info)
        {
            var rowCount = this.fpSpread1_Sheet1.Rows.Count;

            var rowIndex = rowCount;

            this.fpSpread1_Sheet1.Rows.Add(rowIndex, 1);

            this.fpSpread1_Sheet1.Rows[rowIndex].Tag = info;

            this.fpSpread1_Sheet1.Cells[rowIndex, 0].Value = (rowCount + 1).ToString();
            this.fpSpread1_Sheet1.Cells[rowIndex, 1].Value = info.DrugName;
            this.fpSpread1_Sheet1.Cells[rowIndex, 1].ForeColor = Color.FromArgb(42, 164, 164);
            this.fpSpread1_Sheet1.Cells[rowIndex, 2].Value = info.Specs;
            this.fpSpread1_Sheet1.Cells[rowIndex, 3].Value = info.SpiltQty;
            this.fpSpread1_Sheet1.Cells[rowIndex, 4].Value = info.TraceCode;
            this.fpSpread1_Sheet1.Cells[rowIndex, 5].Value = info.BatchNo;
            this.fpSpread1_Sheet1.Cells[rowIndex, 6].Value = info.ExpyEnd;

            this.fpSpread1_Sheet1.Cells[rowIndex, 7].Value = info.OperTime;

            FarPointButtonCellType removeButton = new FarPointButtonCellType
            {
                ButtonText = "X 移除",

                ButtonWidth = 76,
                ButtonHeight = 28,

                ButtonAlignment = ContentAlignment.MiddleCenter,
                PrimaryColor = Color.FromArgb(220, 53, 69),
                HoverColor = Color.FromArgb(32, 144, 144),
                BorderRadius = 4,
            };

            this.fpSpread1_Sheet1.Cells[rowIndex, 8].CellType = removeButton;

            //fpSpread2_Sheet1.ClearSelection();
            //fpSpread2_Sheet1.SetActiveCell(-1, -1);

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
        /// 初始化已入库列表 前200条？
        /// </summary>
        private void InitFp2Data()
        {
            var list = TraceCodeDAL.GetYbTraceInboundOrderListForDrugDeptCode(LoginEmployee.Dept.ID);

            if (!list.Any())
            {
                return;
            }

            this.fpSpread2_Sheet1.RowCount = 0;

            for (int i = 0; i < list.Count; i++)
            {
                this.fpSpread2_Sheet1.Rows.Add(i, 1);

                var info = list[i];
                this.fpSpread2_Sheet1.Rows[i].Tag = info;

                this.fpSpread2_Sheet1.Cells[i, 0].Value = i + 1;
                this.fpSpread2_Sheet1.Cells[i, 1].Value = info.InboundNo;
                this.fpSpread2_Sheet1.Cells[i, 2].Value = info.DrugName;
                this.fpSpread2_Sheet1.Cells[i, 2].ForeColor = Color.FromArgb(42, 164, 164);
                this.fpSpread2_Sheet1.Cells[i, 3].Value = info.DrugSpecs;
                this.fpSpread2_Sheet1.Cells[i, 4].Value = info.SplitQty;
                this.fpSpread2_Sheet1.Cells[i, 5].Value = info.OriginalTraceCode;
                this.fpSpread2_Sheet1.Cells[i, 6].Value = info.BchNo;

                var cellType = GetStatusCell(info.Status);
                this.fpSpread2_Sheet1.Cells[i, 7].CellType = cellType;
                this.fpSpread2_Sheet1.Cells[i, 8].Value = info.InboundOperName;
                this.fpSpread2_Sheet1.Cells[i, 9].Value = info.InboundOperTime;

                FarPointButtonCellType detailButton = new FarPointButtonCellType
                {
                    ButtonText = "查看详情",
                    ButtonWidth = 76,
                    ButtonHeight = 28,

                    ButtonAlignment = ContentAlignment.MiddleCenter,
                    PrimaryColor = Color.FromArgb(42, 164, 164),
                    HoverColor = Color.FromArgb(32, 144, 144),
                    BorderRadius = 4
                };

                this.fpSpread2_Sheet1.Cells[i, 10].CellType = detailButton;

                FarPointButtonCellType ReturnButton = new FarPointButtonCellType
                {
                    ButtonText = "申请退库",
                    ButtonWidth = 76,
                    ButtonHeight = 28,

                    ButtonAlignment = ContentAlignment.MiddleCenter,
                    PrimaryColor = Color.FromArgb(220, 53, 69),
                    BorderRadius = 4
                };

                this.fpSpread2_Sheet1.Cells[i, 11].CellType = ReturnButton;

            }
            this.fpSpread2_Sheet1.AddColumnHeaderSpanCell(0, 10, 1, 2);
            this.fpSpread2_Sheet1.ColumnHeader.Cells[0, 10].Text = "操作";
        }

        /// <summary>
        /// 保存入库单据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveInboundOrder_Click(object sender, EventArgs e)
        {
            string errMsg = "保存入库失败:";

            var inboundNo = this.lblInboundNo.Text;
            if (string.IsNullOrEmpty(inboundNo))
            {
                MessageBox.Show(errMsg + "入库单号不能为空!");
                return;
            }

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

                    var drugInfo = GetDrugInfo(fpInfo.DrugCode);

                    inboundOrder.DrugCode = drugInfo.DrugCode;
                    inboundOrder.DrugName = drugInfo.TradeName;
                    inboundOrder.DrugSpecs = drugInfo.Specs;
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
                        stock.DrugCode = drugInfo.DrugCode;
                        stock.DrugName = drugInfo.TradeName;
                        stock.DrugSpecs = drugInfo.Specs;
                        stock.DrugCustomCode = drugInfo.CustomCode;
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
                        stockRecordInfo.AfterAvailableQty =fpInfo.SpiltQty;
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
                    traceSeedInfo.DrugCode = drugInfo.DrugCode;
                    traceSeedInfo.DrugName = drugInfo.TradeName;
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

                    #region 4.弃用处理入库明细与相关状态表

                    //for (int i = 0; i < fpInfo.SpiltQty; i++)
                    //{
                    //    var inboundDetail = new YbTraceInboundDetail();

                    //    inboundDetail.Id = Guid.NewGuid().ToString();
                    //    inboundDetail.InboundId = inboundOrder.Id;
                    //    inboundDetail.DrugDeptCode = inboundOrder.DrugDeptCode;
                    //    inboundDetail.DrugDeptName = inboundOrder.DrugDeptName;
                    //    inboundDetail.InboundNo = inboundOrder.InboundNo;
                    //    inboundDetail.SupplierId = inboundOrder.SupplierId;
                    //    inboundDetail.SupplierCode = inboundOrder.SupplierCode;
                    //    inboundDetail.SupplierName = inboundOrder.SupplierName;
                    //    inboundDetail.DrugCode = inboundOrder.DrugCode;
                    //    inboundDetail.DrugName = inboundOrder.DrugName;
                    //    inboundDetail.DrugSpecs = inboundOrder.DrugSpecs;
                    //    inboundDetail.DrugCustomCode = inboundOrder.DrugCustomCode;
                    //    inboundDetail.DrugPactUnit = inboundOrder.DrugPactUnit;
                    //    inboundDetail.DrugPactQty = inboundOrder.DrugPactQty;
                    //    inboundDetail.DrugMinUnit = inboundOrder.DrugMinUnit;
                    //    inboundDetail.BchNo = inboundOrder.BchNo;
                    //    inboundDetail.ManuLotnum = inboundOrder.ManuLotnum;
                    //    inboundDetail.ManuDate = inboundOrder.ManuDate;
                    //    inboundDetail.ExpyEnd = inboundOrder.ExpyEnd;
                    //    inboundDetail.OriginalTraceCode = fpInfo.TraceCode;
                    //    inboundDetail.OriginalQty = drugInfo.PackQty;
                    //    inboundDetail.SplitQty = fpInfo.SpiltQty;
                    //    inboundDetail.ParentTraceCode = fpInfo.TraceCode;
                    //    inboundDetail.ChildTraceCode = inboundDetail.OriginalTraceCode + "-" + i.ToString();
                    //    inboundDetail.ChildQty = 1;
                    //    inboundDetail.ChildSequenceNo = i;
                    //    inboundDetail.Status = "0";
                    //    inboundDetail.SourceType = "0";
                    //    inboundDetail.InboundClientIp = inboundOrder.InboundClientIp;
                    //    inboundDetail.InboundOperCode = inboundOrder.InboundOperCode;
                    //    inboundDetail.InboundOperName = inboundOrder.InboundOperName;
                    //    inboundDetail.InboundOperTime = inboundOrder.InboundOperTime;
                    //    inboundDetail.CreatedCode = inboundOrder.CreatedCode;
                    //    inboundDetail.CreatedName = inboundOrder.CreatedName;
                    //    if (!TraceCodeDAL.InsertYbTraceInboundDetail(inboundDetail))
                    //    {
                    //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    //        MessageBox.Show(errMsg + TraceCodeDAL.Err);
                    //        return;
                    //    }

                    //    var stateRecord = new YbTraceStateRecord();

                    //    stateRecord.Id = Guid.NewGuid().ToString();
                    //    stateRecord.DrugCode = inboundOrder.DrugCode;
                    //    stateRecord.DrugName = inboundOrder.DrugName;
                    //    stateRecord.DrugSpecs = inboundOrder.DrugSpecs;
                    //    stateRecord.DrugCustomCode = inboundOrder.DrugCustomCode;
                    //    stateRecord.DrugPactUnit = inboundOrder.DrugPactUnit;
                    //    stateRecord.DrugPactQty = inboundOrder.DrugPactQty;
                    //    stateRecord.DrugMinUnit = inboundOrder.DrugMinUnit;
                    //    stateRecord.DrugDeptCode = inboundOrder.DrugDeptCode;
                    //    stateRecord.DrugDeptName = inboundOrder.DrugDeptName;
                    //    stateRecord.BchNo = inboundOrder.BchNo;
                    //    stateRecord.ManuLotnum = inboundOrder.ManuLotnum;
                    //    stateRecord.ManuDate = inboundOrder.ManuDate;
                    //    stateRecord.ExpyEnd = inboundOrder.ExpyEnd;
                    //    stateRecord.ParentTraceCode = fpInfo.TraceCode;
                    //    stateRecord.ChildTraceCode = stateRecord.ParentTraceCode + "-" + i.ToString();
                    //    stateRecord.SequenceNo = i;
                    //    stateRecord.TraceStatus = "0";
                    //    stateRecord.InboundTime = inboundOrder.InboundOperTime;
                    //    stateRecord.CreatedCode = inboundOrder.CreatedCode;
                    //    stateRecord.CreatedName = inboundOrder.CreatedName;

                    //    if (!TraceCodeDAL.InsertYbTraceStateRecord(stateRecord))
                    //    {
                    //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    //        MessageBox.Show(errMsg + TraceCodeDAL.Err);
                    //        return;
                    //    }

                    //    var codeRecord = new YbTraceCodeRecord();

                    //    codeRecord.Id = Guid.NewGuid().ToString();
                    //    codeRecord.DrugCode = inboundOrder.DrugCode;
                    //    codeRecord.DrugName = inboundOrder.DrugName;
                    //    codeRecord.ParentTraceCode = fpInfo.TraceCode;
                    //    codeRecord.ChildTraceCode = codeRecord.ParentTraceCode + "-" + i.ToString();
                    //    codeRecord.SequenceNo = i;
                    //    codeRecord.RelatedOrderNo = inboundOrder.InboundNo;
                    //    codeRecord.RelatedId = inboundDetail.Id;
                    //    codeRecord.RelatedTableName = "yb_trace_inbound_detail";
                    //    codeRecord.BusinessScenario = BusinessScenarioEnum.SplitInbound;
                    //    codeRecord.OperationType = "0";
                    //    codeRecord.OperationTime = inboundOrder.InboundOperTime;
                    //    codeRecord.OperationJson = "";
                    //    codeRecord.OperationDescription = "入库成功,子追溯码:" + codeRecord.ChildTraceCode + "状态变更为已入库";
                    //    codeRecord.CreatedCode = inboundOrder.CreatedCode;
                    //    codeRecord.CreatedName = inboundOrder.CreatedName;

                    //    if (!TraceCodeDAL.InsertYbTraceCodeRecord(codeRecord))
                    //    {
                    //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    //        MessageBox.Show(errMsg + TraceCodeDAL.Err);
                    //        return;
                    //    }

                    //}

                    #endregion

                }

                //提交事务
                Neusoft.FrameWork.Management.PublicTrans.Commit();

                MessageBox.Show("入库成功!" + System.Environment.NewLine + "入库单据号码:[" + inboundNo + "]");

                Clear();

                this.lblInboundNo.Text = TraceCodeDAL.GetInboundNo("SM");

                InitFp2Data();
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("保存出现异常:" + ex.Message);
                return;
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            var rowCount = this.fpSpread1_Sheet1.RowCount;
            if (rowCount <= 0)
            {
                MessageBox.Show("暂无数据需要清空!");
                return;
            }

            DialogResult rs = MessageBox.Show("共清空" + rowCount + "条数据" + "\n是否确认清空？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (rs != DialogResult.Yes)
            {
                return;
            }

            Clear();
        }

        private void OnFP1CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            var columnIndex = e.Column;
            var rowIndex = e.Row;

            if (columnIndex != 8)
            {
                return;
            }

            var info = this.fpSpread1_Sheet1.Rows[rowIndex].Tag as FP_Info;

            DialogResult rs = MessageBox.Show("确认将[" + info.DrugName + "]删除？" + "\n是否确认移除？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (rs != DialogResult.Yes)
            {
                return;
            }

            this.fpSpread1_Sheet1.Rows.Remove(rowIndex, 1);

            if (AlreadyScannedTraceCodes.Contains(info.TraceCode))
            {
                AlreadyScannedTraceCodes.Remove(info.TraceCode);
            }

        }

        private void OnFP2CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                var columnIndex = e.Column;
                var rowIndex = e.Row;

                if (columnIndex != 10 && columnIndex != 11)
                {
                    return;
                }

                var info = this.fpSpread2_Sheet1.Rows[rowIndex].Tag as YbTraceInboundOrder;

                if (info == null)
                {
                    MessageBox.Show("FP绑定信息为空!");
                    return;
                }

                if (columnIndex == 10)
                {
                    ShowInboundDetail(info);
                    return;
                }



                if (columnIndex == 11)
                {
                    DialogResult rs = MessageBox.Show("确认将入库记录[" + info.InboundNo + "]进行退库？" + "\n是否确认退库？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (rs != DialogResult.Yes)
                    {
                        return;
                    }

                    if (!ReturnInboundOrder(info))
                    {
                        return;
                    }

                    MessageBox.Show("入库单号:[" + info.InboundNo + "]" + Environment.NewLine + "申请退库成功!");

                    InitFp2Data();

                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("按钮操作出现异常:" + ex.Message, "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowInboundDetail(YbTraceInboundOrder info)
        {
            frmInboundDetailTraceCode dialog = new frmInboundDetailTraceCode();

            var initResult = dialog.Init(info.InboundNo);
            if (!initResult)
            {
                return;
            }

            DialogResult result = ModalDialog.ShowDialog(dialog, this);
        }

        private bool ReturnInboundOrder(YbTraceInboundOrder info)
        {
            if (info == null || string.IsNullOrEmpty(info.InboundNo))
            {
                MessageBox.Show("申请退库失败:入参对象或入库单号为空!");
                return false;
            }

            var list = TraceCodeDAL.GetYbTraceInboundOrderListForInboundNo(info.InboundNo);

            if (!list.Any())
            {
                MessageBox.Show("申请退库失败:[" + info.InboundNo + "]未找到对应入库记录!");
                return false;
            }

            if (list.All(a => a.Status != "0"))
            {
                MessageBox.Show("申请退库失败:[" + info.InboundNo + "]状态不允许进行退库申请!");
                return false;
            }

            try
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

                if (!TraceCodeDAL.UpdateYbTraceInboundOrderState(info.InboundNo, LoginEmployee.ID, LoginEmployee.Name))
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("申请退库失败:" + TraceCodeDAL.Err);
                    return false;
                }

                //弃用
                //if (!TraceCodeDAL.UpdateYbTraceInboundDetailState(info.InboundNo, LoginEmployee.ID, LoginEmployee.Name))
                //{
                //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //    MessageBox.Show("申请退库失败:" + TraceCodeDAL.Err);
                //    return false;
                //}

                foreach (var item in list)
                {
                    if (!TraceCodeDAL.UpdateYbTraceStockWhenOutBoundSucess(item.DrugDeptCode, item.DrugCode, item.SplitQty, LoginEmployee.ID, LoginEmployee.Name))
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("申请退库失败:" + TraceCodeDAL.Err);
                        return false;
                    }
                    
                    //记录库存变换记录

                    //弃用
                    //if (!TraceCodeDAL.UpdateYbTraceStateRecordTraceState(item.OriginalTraceCode, "2", LoginEmployee.ID, LoginEmployee.Name))
                    //{
                    //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    //    MessageBox.Show("申请退库失败:" + TraceCodeDAL.Err);
                    //    return false;
                    //}
                }

                Neusoft.FrameWork.Management.PublicTrans.Commit();

                return true;
            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show("申请退库出现异常:" + ex.Message);
                return false;
            }

        }

        private void frmStockTraceCode_Load(object sender, EventArgs e)
        {
            try
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("映射关系初始化...", 0, false);
                Application.DoEvents();

                GetMap();

                Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("药品信息初始化...", 0, false);

                GetDrugBaseInfoMap();

                ApplyRoundCornersToAllPanels(this);


                //Neusoft.FrameWork.WinForms.Classes.Function.ShowWaitForm("控件自动缩放初始化...", 0, false);
                ////对窗口应用控件缩放器
                //zoomer = new WindowZoomer(this, false);

                this.txtScanCode.Clear();
                this.txtScanCode.Focus();

            }
            catch (Exception ex)
            {
                Neusoft.FrameWork.WinForms.Classes.Function.ShowExceptionDialog(ex);
                return;
            }
            finally
            {
                Neusoft.FrameWork.WinForms.Classes.Function.HideWaitForm();
            }



        }

        protected override void OnShown(EventArgs e)
        {

            base.OnShown(e);

            if (this.Parent != null)
            {
                // 对于嵌入在容器中的控件
                this.Dock = DockStyle.Fill;
            }
            else
            {
                // 对于独立窗口
                Form parentForm = this.ParentForm ?? this.Owner;

                if (parentForm != null && parentForm.Visible)
                {
                    this.Size = parentForm.ClientSize;
                    this.Location = parentForm.PointToScreen(Point.Empty);
                }
                else
                {
                    this.WindowState = FormWindowState.Maximized;
                }
            }
        }

        /// <summary>
        /// 根据状态值创建状态单元格
        /// </summary>
        /// <param name="state">状态值：0入库成功 1出库使用 2退库成功 3申请入库 4申请退库</param>
        /// <returns>配置好的StatusImageCellType对象</returns>
        public StatusImageCellType GetStatusCell(string state)
        {
            StatusImageCellType cell = new StatusImageCellType
            {
                ShowIcon = true,
                IconSize = 14,
                FontSize = 9F,
                FontName = "微软雅黑",
                BorderRadius = 12,
                PaddingHorizontal = 8,
                PaddingVertical = 4,
                ShowBackground = true
            };

            switch (state)
            {
                case "0": // 入库成功
                    cell.StatusText = "入库成功";
                    cell.IconColor = Color.FromArgb(34, 197, 94);
                    cell.TextColor = Color.FromArgb(22, 163, 74);
                    cell.BackgroundColor = Color.FromArgb(240, 253, 244);
                    cell.IconType = IconType.Circle;
                    cell.IconSymbol = "✓";
                    break;

                case "1": // 出库使用
                    cell.StatusText = "出库使用";
                    cell.IconColor = Color.FromArgb(59, 130, 246);
                    cell.TextColor = Color.FromArgb(37, 99, 235);
                    cell.BackgroundColor = Color.FromArgb(239, 246, 255);
                    cell.IconType = IconType.Circle;
                    cell.IconSymbol = "→";
                    break;

                case "2": // 退库成功
                    cell.StatusText = "退库成功";
                    cell.IconColor = Color.FromArgb(245, 158, 11);
                    cell.TextColor = Color.FromArgb(217, 119, 6);
                    cell.BackgroundColor = Color.FromArgb(255, 251, 235);
                    cell.IconType = IconType.Circle;
                    cell.IconSymbol = "↺";
                    break;

                case "3": // 申请入库
                    cell.StatusText = "申请入库";
                    cell.IconColor = Color.FromArgb(14, 165, 233);
                    cell.TextColor = Color.FromArgb(2, 132, 199);
                    cell.BackgroundColor = Color.FromArgb(240, 249, 255);
                    cell.IconType = IconType.Triangle;
                    cell.IconSymbol = "?";
                    break;

                case "4": // 申请退库
                    cell.StatusText = "申请退库";
                    cell.IconColor = Color.FromArgb(251, 191, 36);
                    cell.TextColor = Color.FromArgb(245, 158, 11);
                    cell.BackgroundColor = Color.FromArgb(255, 252, 240);
                    cell.IconType = IconType.Triangle;
                    cell.IconSymbol = "!";
                    break;

                default: // 未知状态
                    cell.StatusText = "未知状态";
                    cell.IconColor = Color.FromArgb(156, 163, 175);
                    cell.TextColor = Color.FromArgb(107, 114, 128);
                    cell.BackgroundColor = Color.FromArgb(249, 250, 251);
                    cell.IconType = IconType.Circle;
                    cell.IconSymbol = "?";
                    break;
            }

            return cell;
        }

        /// <summary>
        /// 将所有panel控件设置为圆角
        /// </summary>
        /// <param name="parentControl"></param>
        private void ApplyRoundCornersToAllPanels(Control parentControl)
        {
            foreach (Control control in parentControl.Controls)
            {
                // 检查是否是 Panel
                if (control is Panel)
                {
                    Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(control, 4);
                }

                // 递归处理子控件
                if (control.HasChildren)
                {
                    ApplyRoundCornersToAllPanels(control);
                }
            }
        }

        /// <summary>
        /// 将FP表格线条设置为web风格样式
        /// </summary>
        /// <param name="fp"></param>
        private void SetWebStyleGridLine(SheetView fp)
        {
            // 去掉表格垂直线
            fp.VerticalGridLine = new GridLine(GridLineType.None);
            // 设置表格浅灰色水平线      
            fp.HorizontalGridLine = new GridLine(GridLineType.Flat, Color.FromArgb(153, 241, 243, 244));

            //去掉列标题的水平线
            fp.ColumnHeader.HorizontalGridLine = new GridLine(GridLineType.None);
        }

        /// <summary>
        /// 美化搜索输入框
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="placeholder"></param>
        /// <param name="customStyle"></param>
        public void BeautifyTextBox(TextBox textBox, string placeholder, Action<TextBox> customStyle)
        {
            textBox.Font = new Font("宋体", 16F, FontStyle.Regular);
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;
            textBox.TextAlign = HorizontalAlignment.Left;

            textBox.GotFocus += (sender, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                    textBox.Font = new Font("宋体", 16F, FontStyle.Regular);
                    textBox.TextAlign = HorizontalAlignment.Left;
                }
            };

            textBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Font = new Font("宋体", 16F, FontStyle.Regular);
                    textBox.ForeColor = Color.Gray;
                    textBox.Text = placeholder;
                    textBox.TextAlign = HorizontalAlignment.Left;
                }
            };

            if (customStyle != null)
            {
                customStyle.Invoke(textBox);
            }
        }

        public void Clear()
        {
            this.fpSpread1_Sheet1.RowCount = 0;
            AlreadyScannedTraceCodes.Clear();
        }

    }

    public class FP_Info
    {
        public string DrugCode { get; set; }

        public string DrugName { get; set; }

        public string TraceCode { get; set; }

        public string Specs { get; set; }

        public decimal SpiltQty { get; set; }

        public string BatchNo { get; set; }

        public string OperCode { get; set; }

        public string OperName { get; set; }

        public DateTime OperTime { get; set; }

        public string ExpyEnd { get; set; }

        public string ManuLotnum { get; set; }

        public string ManuDate { get; set; }


    }

}
