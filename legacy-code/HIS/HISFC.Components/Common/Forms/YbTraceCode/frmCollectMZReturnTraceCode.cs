using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FarPoint.Win.Spread.CellType;
using Neusoft.HISFC.Models.Pharmacy;
using FarPoint.Win.Spread;
using Neusoft.HISFC.Components.Common.Controls.ModernStyles;
using Neusoft.HISFC.Models.MedicalTraceCode;
using Neusoft.HISFC.Components.Common.Services.YBTraceCode;
using Neusoft.HISFC.BizLogic.Pharmacy;
using Neusoft.HISFC.Components.Common.Forms.YbTraceCode;

namespace Neusoft.HISFC.Components.Common.Forms.YbTraceCode
{
    public partial class frmCollectMZReturnTraceCode : Form
    {
        #region 属性

        private TraceCodeDAL QueryService = new TraceCodeDAL();

        private PatientAndApplyInfo PatientAndApplyInfo = new PatientAndApplyInfo();

        protected Dictionary<string, List<string>> ApplyTraceMap =
                new Dictionary<string, List<string>>();

        /// <summary>
        /// FP控件列RichTextCellType设置
        /// </summary>
        protected FarPoint.Win.Spread.CellType.RichTextCellType rtct = new FarPoint.Win.Spread.CellType.RichTextCellType
        {
            WordWrap = true,
            Multiline = true
        };

        /// <summary>
        /// FP控件列TextCellType设置
        /// </summary>
        protected FarPoint.Win.Spread.CellType.TextCellType textCellType = new FarPoint.Win.Spread.CellType.TextCellType
        {
            WordWrap = true,
            Multiline = true,
            ReadOnly = true,

            ScrollBars = ScrollBars.Vertical
        };

        protected Font Font15 = new Font("宋体", 15, FontStyle.Bold);

        private Neusoft.HISFC.Models.Base.Employee LoginEmployee = new Neusoft.HISFC.Models.Base.Employee();

        public List<YbTraceCollectMain> YbTraceCollectMainList = new List<YbTraceCollectMain>();

        /// <summary>
        /// 已经扫描的追溯码
        /// </summary>
        private HashSet<string> AlreadyScannedTraceCodes = new HashSet<string>();

        private DateTime collectStartTime = DateTime.Now;

        #endregion

        public frmCollectMZReturnTraceCode()
        {
            InitializeComponent();

            Neusoft.FrameWork.WinForms.Classes.Function.EnableDrag(this.pnlTitleBar, this);

            this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(fpSpread1_ButtonClicked);

            SetStyle();

            if (Neusoft.FrameWork.Management.Connection.Operator != null)
            {
                LoginEmployee = ((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator);

            }

            //ExampleUsage();
            //var list = GetInitTestDataSource();
            //InitData(list);
        }

        /// <summary>
        /// 初始化设置界面样式
        /// </summary>
        private void SetStyle()
        {
            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this, 32);
            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.panelTopLeft_Big, 18);
            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.panelTopRight_Big, 18);

            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.panelError, 8);

            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.fpSpread1 as Control, 32);

            SetButtonStyle(btnYJFM, Color.FromArgb(255, 33, 150, 243), Color.FromArgb(255, 25, 118, 210));
            // 拆零维护
            SetButtonStyle(btnSplit, Color.FromArgb(255, 76, 175, 80), Color.FromArgb(255, 56, 142, 60));
            // 无码维护
            SetButtonStyle(btn五码维护, Color.FromArgb(255, 96, 125, 139), Color.FromArgb(255, 69, 90, 100));
            // 提交保存
            SetButtonStyle(btnSave, Color.FromArgb(255, 0, 188, 212), Color.FromArgb(255, 0, 151, 167));
            // 直接发药
            SetButtonStyle(this.btnDirectSendDrug, Color.FromArgb(255, 255, 152, 0), Color.FromArgb(255, 245, 124, 0));

            AddShortcutHintForButton(this.btnDirectSendDrug, "Alt+Enter");

            // 设置标题行高度
            fpSpread1_Sheet1.ColumnHeader.Rows[0].Height = 36;
            fpSpread1_Sheet1.ColumnHeader.Rows[0].Font = new Font("宋体", 9, FontStyle.Bold);

            //this.fpSpread1_Sheet1.AddColumnHeaderSpanCell(0, 5, 1, 2);
            this.fpSpread1_Sheet1.ColumnHeader.Cells[0, 5].Text = "操作";
            //this.fpSpread1_Sheet1.ColumnHeader.Columns[5].Width = 80;
            //this.fpSpread1_Sheet1.ColumnHeader.Columns[6].Width = 80;

            ////将FP表格线条设置为web风格样式
            SetWebStyleGridLine(this.fpSpread1_Sheet1);
        }

        public void InitData(List<PhaComApplyout> list)
        {
            var collectList = new List<YbTraceCollectMain>();

            int SortIndex = 0;
            foreach (var applyOut in list)
            {
                var traceCollectMainInfo = new YbTraceCollectMain();
                traceCollectMainInfo.Id = Guid.NewGuid().ToString();
                traceCollectMainInfo.ApplyNumber = applyOut.ApplyNumber.ToString();

                traceCollectMainInfo.BusinessScenario = BusinessScenarioEnum.OutpatientReturnAudit;
                traceCollectMainInfo.CollectType = CollectTypeEnum.ReturnOfGoods;
                traceCollectMainInfo.SourceSystem = SourceSystemEnum.HIS;
                traceCollectMainInfo.BusinessType = BusinessTypeEnum.MZ;

                traceCollectMainInfo.SerialNo = applyOut.PatientId;
                traceCollectMainInfo.PatientName = "";
                traceCollectMainInfo.DrugCode = applyOut.DrugCode;
                traceCollectMainInfo.DrugName = applyOut.TradeName;
                var drugInfo = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(applyOut.DrugCode);
                traceCollectMainInfo.DrugSpecs = drugInfo.Specs;
                traceCollectMainInfo.DrugCustomCode = drugInfo.UserCode;
                traceCollectMainInfo.DrugPactQty = applyOut.PackQty.ToString();
                traceCollectMainInfo.DrugPactUnit = drugInfo.PackUnit;
                traceCollectMainInfo.DrugMinUnit = drugInfo.MinUnit;
                traceCollectMainInfo.DrugSplitUnit = drugInfo.MinUnit;
                traceCollectMainInfo.PharmacyCode = applyOut.DrugDeptCode;
                traceCollectMainInfo.PharmacyName = "";
                traceCollectMainInfo.DeptCode = applyOut.DeptCode;
                traceCollectMainInfo.DeptName = "";
                traceCollectMainInfo.MoOrderNo = applyOut.MoOrder;
                traceCollectMainInfo.ExecOrderNo = applyOut.ExecSqn;

                traceCollectMainInfo.CollectIp = Neusoft.FrameWork.WinForms.Classes.Function.GetLocalIP();

                if (this.QueryService.Operator != null)
                {
                    traceCollectMainInfo.CreatedCode = this.QueryService.Operator.ID;
                    traceCollectMainInfo.CreatedName = this.QueryService.Operator.Name;
                    traceCollectMainInfo.CollectOperCode = traceCollectMainInfo.CreatedCode;
                    traceCollectMainInfo.CollectOperName = traceCollectMainInfo.CreatedName;
                }

                if (Neusoft.FrameWork.Management.Connection.Hospital.ID == "CORE_HIS502")
                {
                    traceCollectMainInfo.HospitalCode = "H44040200357";
                    traceCollectMainInfo.HospitalName = "中山大学珠海校区卫生服务中心";
                }
                else
                {
                    traceCollectMainInfo.HospitalCode = "H44040200001";
                    traceCollectMainInfo.HospitalName = "中山大学附属第五医院";
                }

                //需要采集 且采集完成的数据才进行赋值采集退费数据
                if (applyOut.NeedCollectTraceCodeFlag == "1" && TraceCodeCollectionStatusEnum.IsCollectCompleted(applyOut.Tracecodecollectionstatus))
                {
                    if (applyOut.Alreadycollectqty > 0)
                    {
                        traceCollectMainInfo.IsHavePact = "1";
                        traceCollectMainInfo.PactNeedCollectQty = applyOut.Alreadycollectqty;
                        traceCollectMainInfo.PactActualCollectQty = 0;
                        traceCollectMainInfo.PactAppealCollectQty = applyOut.Appealcollectqty;
                        traceCollectMainInfo.PactUnCollectQty = applyOut.Alreadycollectqty;
                        traceCollectMainInfo.PactCollectStatus = "0";
                        traceCollectMainInfo.PactCollectMethod = "1";
                    }
                    else
                    {
                        traceCollectMainInfo.IsHavePact = "0";
                        traceCollectMainInfo.PactNeedCollectQty = 0;
                        traceCollectMainInfo.PactActualCollectQty = 0;
                        traceCollectMainInfo.PactAppealCollectQty = 0;
                        traceCollectMainInfo.PactUnCollectQty = 0;
                        traceCollectMainInfo.PactCollectStatus = "2";
                        traceCollectMainInfo.PactCollectMethod = "1";
                    }

                    if (applyOut.AlreadyCollectSpiltQty > 0)
                    {
                        traceCollectMainInfo.IsHaveSplit = "1";
                        traceCollectMainInfo.SplitNeedCollectQty = applyOut.AlreadyCollectSpiltQty;
                        traceCollectMainInfo.SplitActualCollectQty = 0;
                        traceCollectMainInfo.SplitAppealCollectQty = applyOut.AppealCollectSpiltQty;
                        traceCollectMainInfo.SplitUnCollectQty = applyOut.AlreadyCollectSpiltQty;
                        traceCollectMainInfo.SplitCollectStatus = "0";
                        traceCollectMainInfo.SplitCollectMethod = "1";
                    }
                    else
                    {
                        traceCollectMainInfo.IsHaveSplit = "0";
                        traceCollectMainInfo.SplitNeedCollectQty = 0;
                        traceCollectMainInfo.SplitActualCollectQty = 0;
                        traceCollectMainInfo.SplitAppealCollectQty = 0;
                        traceCollectMainInfo.SplitUnCollectQty = 0;
                        traceCollectMainInfo.SplitCollectStatus = "2";
                        traceCollectMainInfo.SplitCollectMethod = "1";
                    }


                }

                //非数据库属性
                traceCollectMainInfo.SortIndex = SortIndex;

                collectList.Add(traceCollectMainInfo);
                SortIndex++;
            }

            InitData(collectList);

        }


        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="list"></param>
        private void InitData(List<YbTraceCollectMain> list)
        {
            if (!list.Any())
            {
                MessageBox.Show("待采集集合信息为空!");
                return;
            }
            YbTraceCollectMainList = list;

            ApplyTraceMap = this.QueryService.GetApplyNumberTraceMap(list.Select(a => a.ApplyNumber).ToList());

            var info = list[0];

            InitPatientInfoAndApplyOutInfo(info);

            InitTitle(info);

            InitFPData(list);

        }

        /// <summary>
        /// 初始化窗口标题
        /// </summary>
        /// <param name="info"></param>
        private void InitTitle(YbTraceCollectMain info)
        {
            var businessTypeDes = BusinessTypeEnum.GetDescription(info.BusinessType);
            var collectTypeDes = CollectTypeEnum.GetDescription(info.CollectType);
            var businessScenarioDes = BusinessScenarioEnum.GetDescription(info.BusinessScenario);

            this.lblTitle.Text = @"追溯码采集窗口\" + businessTypeDes + @"\" + collectTypeDes + @"\" + businessScenarioDes;

        }

        /// <summary>
        /// 初始化患者与发药信息
        /// </summary>
        /// <param name="info"></param>
        private void InitPatientInfoAndApplyOutInfo(YbTraceCollectMain info)
        {
            var errMsg = "初始化患者与发药信息失败:";

            if (!BusinessTypeEnum.IsValid(info.BusinessType))
            {
                MessageBox.Show(errMsg + "业务类型非法!");
                return;
            }

            if (info.BusinessType == BusinessTypeEnum.MZ)
            {
                SetMZPatientAndApplyInfo(info.ApplyNumber);
            }

            if (info.BusinessType == BusinessTypeEnum.ZY)
            {
                SetZYPatientAndApplyInfo(info.ApplyNumber);
            }


        }

        /// <summary>
        /// 设置住院患者与发药信息
        /// </summary>
        /// <param name="applyNumber"></param>
        private void SetZYPatientAndApplyInfo(string applyNumber)
        {
            var info = QueryService.GetPatientAndApplyInfo(applyNumber);
            PatientAndApplyInfo = info;
            this.lblName.Text = info.Name;
            this.lblNoText.Text = "住院号";
            this.lblNoValue.Text = info.PatientNo;
            this.lblRecipeNo.Text = info.RecipeNo;
            this.lblDrugDeptName.Text = info.DrugDeptName;
            this.lblDeptName.Text = info.DeptName;
            this.lblRecipeDeptName.Text = info.RecipeDeptName;
            this.lblRecipeOperName.Text = info.RecipeOperName;
            this.lblSex.Text = info.Sex;

        }

        private void SetMZPatientAndApplyInfo(string applyNumber)
        {
            var info = QueryService.GetMZPatientAndApplyInfo(applyNumber);
            PatientAndApplyInfo = info;
            this.lblName.Text = info.Name;
            this.lblNoText.Text = "门诊号";
            this.lblNoValue.Text = info.PatientNo;
            this.lblRecipeNo.Text = info.RecipeNo;
            this.lblDrugDeptName.Text = info.DrugDeptName;
            this.lblDeptName.Text = info.DeptName;
            this.lblRecipeDeptName.Text = info.RecipeDeptName;
            this.lblRecipeOperName.Text = info.RecipeOperName;
            this.lblSex.Text = info.Sex;
        }


        /// <summary>
        /// 初始化FP数据
        /// </summary>
        /// <param name="list"></param>
        private void InitFPData(List<YbTraceCollectMain> list)
        {
            this.fpSpread1_Sheet1.RowCount = 0;

            for (int i = 0; i < list.Count; i++)
            {
                this.fpSpread1_Sheet1.Rows.Add(i, 1);

                var info = list[i];
                info.PatientName = this.PatientAndApplyInfo.Name;
                info.PharmacyName = this.PatientAndApplyInfo.DrugDeptName;
                info.CardNo = this.PatientAndApplyInfo.CardNo;
                info.PatientNo = this.PatientAndApplyInfo.PatientNo;

                //拆零药品自动赋码
                if (info.IsHaveSplit == YesNoEnum.Yes)
                {
                    AutoAssignTraceCodeForSplitDrug(i, info);
                }

                #region 第0列药品信息赋值

                SetDrugInfoFPColumnValue(i, info);

                #endregion

                #region 第1列数量需求赋值

                SetCollectQtyFPColumnValue(i, info);

                #endregion

                #region 第2列采集进度赋值

                SetCollectProgressFPColumnValue(i, info);

                #endregion

                #region 第3列包装追溯码信息赋值

                SetPactTraceCodeFPColumnValue(i, info);

                #endregion

                #region 第4列拆零追溯码信息赋值

                SetSplitTraceCodeFPColumnValue(i, info);

                #endregion

                #region 第5列拆零按钮赋值

                //SetSplitBtnFPColumnValue(i, info);

                #endregion

                #region 第6列申诉按钮赋值

                SetAppealBtnFPColumnValue(i, info);

                #endregion

                this.fpSpread1_Sheet1.Rows[i].Tag = info;

                // 设置垂直/水平对齐
                //fpSpread1_Sheet1.Cells[i, 0].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                //fpSpread1_Sheet1.Cells[i, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                //自适应行高
                AutoAdjustRowHeight(i);

            }
        }

        /// <summary>
        /// 拆零自动赋码
        /// </summary>
        /// <param name="info"></param>
        private void AutoAssignTraceCodeForSplitDrug(
            int rowIndex,
            YbTraceCollectMain info)
        {
            var errMsg = "拆零自动赋码失败:";

            if (info == null)
            {
                SetSplitTraceCodeFPErrMsg(rowIndex, errMsg + "传入对象为空!");
                return;
            }

            info.SplitActualCollectQty = info.SplitNeedCollectQty;
            info.SplitCollectStatus = TraceCodeCollectionStatusEnum.Sucess;

            //设置采集数量
            SetCollectQtyFPColumnValue(rowIndex, info);
            //设置拆零追溯码列值
            SetSplitTraceCodeFPColumnValue(rowIndex, info);
            //设置采集进度条
            SetCollectProgressFPColumnValue(rowIndex, info);

            return;//等发药保存时候再进行真正分配与扣减库存

        }

        // 通用按钮样式设置方法
        private void SetButtonStyle(Button btn, Color backColor, Color hoverColor)
        {
            //btn.Height = 50;
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("微软雅黑", 10, FontStyle.Bold);

            btn.MouseEnter += (s, e) => { btn.BackColor = hoverColor; };
            btn.MouseLeave += (s, e) => { btn.BackColor = backColor; };
        }


        private List<YbTraceCollectMain> GetInitTestDataSource()
        {
            var list = new List<YbTraceCollectMain>();

            string[] drugNameArr = { "玻璃酸钠滴眼液（国产）", "七叶洋地黄双苷滴眼液（施图伦）", "富马酸依美斯汀滴眼液", "硫酸阿托品眼用凝胶", "（安达美）多种微量元素注射液（Ⅱ中选）" };
            string[] drugCodeArr = { "Y00000015930", "Y00000015931", "Y00000015932", "Y00000015934", "Y00000015108" };
            string[] drugSpecsArr = { "5ml:5mg/支", "0.4ml*10支/盒", "5ml:2.5mg/支", "2.5g/支", "10ml/支" };

            YbTraceCollectMain info;
            for (int i = 0; i < drugNameArr.Length; i++)
            {
                var drugName = drugNameArr[i];
                var drugSpecs = drugSpecsArr[i];
                var drugCode = drugCodeArr[i];

                info = new YbTraceCollectMain();
                info.PactTracCodgsList = new List<string>();
                info.SplitTracCodgsList = new List<string>();
                info.DrugName = drugName;
                info.DrugSpecs = drugSpecs;
                info.DrugCode = drugCode;
                if (i / 2 == 0)
                {
                    info.IsHaveSplit = "1";
                    info.DrugPactUnit = "盒";
                    info.DrugSplitUnit = "支";
                    info.PactNeedCollectQty = 3;
                    info.PactActualCollectQty = 0;
                    info.PactUnCollectQty = 0;
                    info.PactNeedCollectQty = 3;

                    info.SplitNeedCollectQty = 5;
                    info.SplitActualCollectQty = 0;
                    info.SplitUnCollectQty = 0;
                    info.PactTracCodgsList.AddRange(new List<string> { "81341970379957454004", "81508621333476857229", "83890480048451438977" });
                    info.SplitTracCodgsList.AddRange(new List<string> { "81341970379957454004", "81508621333476857229", "83890480048451438977", "84379610103935802107", "84379610103934527527" });
                }
                else
                {
                    info.IsHaveSplit = "0";
                    info.DrugPactUnit = "瓶";
                    info.PactNeedCollectQty = 2;
                    info.PactActualCollectQty = 0;
                    info.PactUnCollectQty = 0;

                    info.SplitActualCollectQty = 0;
                    info.SplitUnCollectQty = 0;

                    info.PactTracCodgsList.AddRange(new List<string> { "81341970379957454004", "81508621333476857229", "83890480048451438977", "84379610103935802107", "84379610103934527527", "81341970379957454004", "81508621333476857229", "81341970379957454004", "81508621333476857229" });
                }
                list.Add(info);
            }

            return list;

        }

        /// <summary>
        /// 创建新RichTextBox对象
        /// </summary>
        /// <returns></returns>
        private RichTextBox BuildRichTextBox()
        {
            var rtb = new RichTextBox
            {
                //Width = (int)this.fpSpread1_Sheet1.Columns[0].Width,
                Multiline = true,
                WordWrap = true,
                //ScrollBars = RichTextBoxScrollBars.None
            };
            rtb.SelectAll();
            rtb.SelectionAlignment = HorizontalAlignment.Center;
            return rtb;
        }

        /// <summary>
        /// 设置FP表格中的DrugInfo列信息
        /// </summary>
        private void SetDrugInfoFPColumnValue(
            int rowIndex,
            YbTraceCollectMain info)
        {

            var columnIndex = (int)FPColumn.DrugInfo;
            var rtb = BuildRichTextBox();
            rtb.AppendText("\n");
            // 药品名称
            rtb.SelectionFont = Font15;
            rtb.SelectionColor = Color.FromArgb(52, 58, 64);
            rtb.AppendText(info.DrugName.Substring(0, Math.Min(15, info.DrugName.Length)));
            // 拆零标识
            if (info.IsHaveSplit == "1")
            {
                rtb.SelectionFont = Font15;
                rtb.SelectionColor = Color.FromArgb(250, 250, 250);
                rtb.SelectionBackColor = Color.FromArgb(79, 157, 166);
                rtb.AppendText("拆零");
                rtb.SelectionBackColor = Color.Transparent;
            }
            rtb.AppendText("\n");
            // 药品规格和代码
            rtb.SelectionFont = new Font("宋体", 12);
            rtb.SelectionColor = Color.FromArgb(134, 142, 150);
            rtb.AppendText(info.DrugCode + " | " + info.DrugSpecs);
            // 设置富文本内容
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = rtct;
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value = rtb.Rtf;
        }

        private void SetCollectQtyFPColumnValue(
           int rowIndex,
           YbTraceCollectMain info)
        {
            var columnIndex = (int)FPColumn.CollectQty;

            var quantityCell = new QuantityDisplayCellType();
            quantityCell.ShowBorder = false;
            quantityCell.SetQuantityData(
                (int)info.PactNeedCollectQty,
                (int)info.PactActualCollectQty,
                (int)info.SplitNeedCollectQty,
                (int)info.SplitActualCollectQty,
                info.DrugPactUnit, info.DrugSplitUnit);

            this.fpSpread1_Sheet1.Rows[rowIndex].Tag = info;

            this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = quantityCell;

        }

        private void SetCollectProgressFPColumnValue(
            int rowIndex,
            YbTraceCollectMain info)
        {
            var columnIndex = (int)FPColumn.CollectProgress;

            var overallProgress = new CircularProgressCellType();
            overallProgress.CellHeight = 72;

            var currentValue = (int)info.PactActualCollectQty + (int)info.PactAppealCollectQty + (int)info.SplitActualCollectQty + (int)info.SplitAppealCollectQty;
            var maxValue = (int)info.PactNeedCollectQty + (int)info.SplitNeedCollectQty;
            overallProgress.SetProgress(currentValue, maxValue);

            this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = overallProgress;

        }

        private void SetPactTraceCodeFPColumnValue(
           int rowIndex,
           YbTraceCollectMain info)
        {

            var columnIndex = (int)FPColumn.PactTraceCode;

            if (info.IsHavePact == YesNoEnum.No)
            {
                //this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = Color.Red;
                //this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Text = "无需采集:收费时未采集包装追溯码!";
                return;
            }

            if (info.PactTracCodgsList == null || !info.PactTracCodgsList.Any())
            {
                return;
            }
            var pactTracCodgsList =
                               info.PactTracCodgsList.Select(code =>
                               {
                                   if (code.Length > 7)
                                   {
                                       string ident = code.Substring(0, 7);
                                       string serial = code.Substring(7);
                                       return ident + " " + serial;
                                   }
                                   else
                                   {
                                       return code;
                                   }
                               }).ToArray();


            this.fpSpread1_Sheet1.Rows[rowIndex].Tag = info;

            var cellType = new MultipleLineCellType();
            cellType.Status = MultipleLineCellType.ItemStatus.Normal; // 蓝色细线
            cellType.ShowScrollbar = true; // 启用滚动条
            cellType.SetTextLines(pactTracCodgsList);
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = Color.FromArgb(79, 157, 166);
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Font = new Font("宋体", 10, FontStyle.Regular);
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = cellType;


        }

        /// <summary>
        /// 设置FP表格中的SplitTraceCode列信息
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="info"></param>
        private void SetSplitTraceCodeFPColumnValue(
            int rowIndex,
            YbTraceCollectMain info)
        {
            var columnIndex = (int)FPColumn.SplitTraceCode;
            if (info.IsHaveSplit == YesNoEnum.No)
            {
                //this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = Color.Red;
                //this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Text = "无需采集:收费时未采集拆零追溯码!";
                return;
            }

            if (info.SplitTracCodgsList == null || !info.SplitTracCodgsList.Any())
            {
                return;
            }



            if (info.IsHaveSplit == "1")
            {
                var splitTracCodgsList =
                              info.SplitTracCodgsList.Select(code =>
                              {
                                  if (code.Length > 7)
                                  {
                                      string ident = code.Substring(0, 7);
                                      string serial = code.Substring(7);
                                      return ident + " " + serial;
                                  }
                                  else
                                  {
                                      return code;
                                  }
                              }).ToArray();


                this.fpSpread1_Sheet1.Rows[rowIndex].Tag = info;

                var cellType = new MultipleLineCellType();
                cellType.Status = MultipleLineCellType.ItemStatus.Normal;
                cellType.ShowScrollbar = true;
                cellType.SetTextLines(splitTracCodgsList);
                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = Color.FromArgb(79, 157, 166);
                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Font = new Font("宋体", 10, FontStyle.Regular);
                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = cellType;
            }

        }

        private void SetSplitBtnFPColumnValue(
           int rowIndex,
           YbTraceCollectMain info)
        {
            //var columnIndex = (int)FPColumn.SplitBtn;

            //var fpButton = new FarPointButtonCellType
            //{
            //    ButtonText = "拆零",
            //    ButtonWidth = 76,
            //    ButtonHeight = 28,

            //    ButtonAlignment = ContentAlignment.MiddleCenter,
            //    PrimaryColor = Color.FromArgb(42, 164, 164),
            //    HoverColor = Color.FromArgb(32, 144, 144),
            //    BorderRadius = 4
            //};

            //this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = fpButton;
        }

        private void SetAppealBtnFPColumnValue(
           int rowIndex,
           YbTraceCollectMain info)
        {
            var columnIndex = (int)FPColumn.AppealBtn;

            var fpButton = new FarPointButtonCellType
            {
                ButtonText = "无码申诉",
                ButtonWidth = 80,
                ButtonHeight = 28,
                ButtonAlignment = ContentAlignment.MiddleCenter,

                PrimaryColor = Color.FromArgb(59, 130, 246),
                HoverColor = Color.FromArgb(37, 99, 235),
                BorderRadius = 6
            };

            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = fpButton;
        }

        /// <summary>
        /// 自动拆零赋码失败时,显示错误信息
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="errMsg"></param>
        private void SetSplitTraceCodeFPErrMsg(
            int rowIndex,
            string errMsg)
        {

            var columnIndex = (int)FPColumn.SplitTraceCode;

            this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = Color.FromArgb(220, 53, 69);
            this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Font = new Font("宋体", 10, FontStyle.Regular);
            this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = textCellType;
            this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value = errMsg;
        }


        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            // 获取当前行和列
            int row = e.Row;
            int col = e.Column;

            // 检查是否是我们要处理的列（比如第5列）
            if (col == 5)
            {
                // 获取当前单元格的按钮文本
                string buttonText = fpSpread1_Sheet1.Cells[row, col].Text;
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
        /// 给按钮右下角绘制快捷键文字说明
        /// </summary>
        /// <param name="button"></param>
        /// <param name="shortcutText"></param>
        public void AddShortcutHintForButton(Button button, string shortcutText)
        {
            button.Paint += (s, e) =>
            {
                using (Font f = new Font("Segoe UI", 6.5f, FontStyle.Regular))
                using (Brush b = new SolidBrush(Color.Gray))
                {
                    SizeF sz = e.Graphics.MeasureString(shortcutText, f);

                    float x = button.Width - sz.Width - 1;
                    float y = button.Height - sz.Height - 1;

                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.DrawString(shortcutText, f, b, x, y);
                }
            };

            // 强制刷新按钮显示
            button.Invalidate();
        }

        private float GetRichTextHeight(RichTextBox rtb)
        {
            if (string.IsNullOrEmpty(rtb.Text)) return 20;

            int lastChar = rtb.TextLength - 1;
            var lastCharPos = rtb.GetPositionFromCharIndex(lastChar);
            return lastCharPos.Y + rtb.Font.Height;
        }

        /// <summary>
        /// 设置表格样式 - 模拟HTML表格的外观
        /// </summary>
        private void SetupTableStyle()
        {
            // 设置表头样式 - 对应HTML中的th样式
            fpSpread1_Sheet1.ColumnHeader.Rows[0].Height = 35;
            fpSpread1_Sheet1.ColumnHeader.Rows[0].BackColor = Color.FromArgb(79, 157, 166); // #4f9da6
            fpSpread1_Sheet1.ColumnHeader.Rows[0].ForeColor = Color.White;
            fpSpread1_Sheet1.ColumnHeader.Rows[0].Font = new Font("Microsoft YaHei", 11, FontStyle.Bold);

            // 设置表头文本
            fpSpread1_Sheet1.ColumnHeader.Cells[0, 0].Text = "药品信息";
            fpSpread1_Sheet1.ColumnHeader.Cells[0, 1].Text = "数量需求";
            fpSpread1_Sheet1.ColumnHeader.Cells[0, 2].Text = "状态";
            fpSpread1_Sheet1.ColumnHeader.Cells[0, 3].Text = "完整包装追溯码";
            fpSpread1_Sheet1.ColumnHeader.Cells[0, 4].Text = "拆零追溯码";
            fpSpread1_Sheet1.ColumnHeader.Cells[0, 5].Text = "操作";

            // 设置表格边框和网格线 - 对应HTML表格的边框样式
            //fpSpread1_Sheet1.GridLineColor = Color.FromArgb(233, 236, 239); // #e9ecef
            //fpSpread1_Sheet1.GridLineStyle = GridLineStyle.Solid;

            // 设置行交替颜色 - 增强可读性
            //fpSpread1_Sheet1.AlternatingRowCount = 1;
            //fpSpread1_Sheet1.AlternatingRowStyle.BackColor = Color.FromArgb(248, 250, 252); // #f8fafc

            // 设置选择样式
            fpSpread1_Sheet1.SelectionStyle = FarPoint.Win.Spread.SelectionStyles.SelectionColors;
            fpSpread1_Sheet1.SelectionBackColor = Color.FromArgb(79, 157, 166); // #4f9da6
            fpSpread1_Sheet1.SelectionForeColor = Color.Black;
        }


        /// <summary>
        /// 自适应行高
        /// </summary>
        /// <param name="rowIndex"></param>
        private void AutoAdjustRowHeight(int rowIndex)
        {
            var height = this.fpSpread1_Sheet1.GetPreferredRowHeight(rowIndex);
            if (this.fpSpread1_Sheet1.Rows[rowIndex].Height < height)
            {
                this.fpSpread1_Sheet1.Rows[rowIndex].Height = height + 6;
            }
            if (this.fpSpread1_Sheet1.Rows[rowIndex].Height > 80)
            {
                this.fpSpread1_Sheet1.Rows[rowIndex].Height = 80;
            }

        }

        private void frmCollectTraceCode_Load(object sender, EventArgs e)
        {
            //this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(fpSpread1_ButtonClicked);
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnDirectSendDrug_Click(object sender, EventArgs e)
        {
            MessageBox.Show("模拟发药操作...");
        }

        private void btn五码维护_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 提交保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            var list = GetYbTraceCollectMainListForFPData();

            if (!IsValid(list))
            {
                return;
            }

            //SetDetailList(list);

            this.YbTraceCollectMainList = list;

            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private bool SetDetailList(List<YbTraceCollectMain> listMain)
        {

            foreach (var mainInfo in listMain)
            {
                var listDetail = new List<YbTraceCollectDetail>();

                if (mainInfo.IsHavePact == YesNoEnum.Yes)
                {
                    for (int i = 0; i < mainInfo.PactTracCodgsList.Count; i++)
                    {
                        var traceCode = mainInfo.PactTracCodgsList[i].ToString();
                        var detatilInfo = new YbTraceCollectDetail();
                        detatilInfo.Id = Guid.NewGuid().ToString();
                        detatilInfo.MainId = mainInfo.Id;
                        detatilInfo.ApplyNumber = mainInfo.ApplyNumber;
                        detatilInfo.TraceCode = traceCode;
                        detatilInfo.TraceCodeType = "0";
                        detatilInfo.TraceCodeSource = "SCAN";
                        detatilInfo.TraceCodeFormat = "CODE128";
                        detatilInfo.CollectSequence = i;
                        detatilInfo.DrugCode = mainInfo.DrugCode;
                        detatilInfo.DrugName = mainInfo.DrugName;
                        detatilInfo.CreatedCode = mainInfo.CreatedCode;
                        detatilInfo.CreatedName = mainInfo.CreatedName;
                        listDetail.Add(detatilInfo);
                    }


                }

                //拆零明细赋码等发药保存时候再进行
                if (mainInfo.IsHaveSplit == "666")// == YesNoEnum.Yes)
                {
                    for (int i = 0; i < mainInfo.SplitTracCodgsList.Count; i++)
                    {
                        var traceCode = mainInfo.SplitTracCodgsList[i].ToString();
                        var detatilInfo = new YbTraceCollectDetail();
                        detatilInfo.Id = Guid.NewGuid().ToString();
                        detatilInfo.MainId = mainInfo.Id;
                        detatilInfo.ApplyNumber = mainInfo.ApplyNumber;
                        detatilInfo.TraceCode = traceCode;
                        detatilInfo.TraceCodeType = "1";
                        detatilInfo.TraceCodeSource = "SCAN";
                        detatilInfo.TraceCodeFormat = "CODE128";
                        detatilInfo.CollectSequence = i;
                        detatilInfo.DrugCode = mainInfo.DrugCode;
                        detatilInfo.DrugName = mainInfo.DrugName;
                        detatilInfo.CreatedCode = mainInfo.CreatedCode;
                        detatilInfo.CreatedName = mainInfo.CreatedName;
                        listDetail.Add(detatilInfo);
                    }
                }

                mainInfo.DetailList = listDetail;

            }

            return true;

        }

        private bool IsValid(List<YbTraceCollectMain> list)
        {
            if (!list.Any())
            {
                MessageBox.Show("数据验证失败:采集集合为空!");
                return false;
            }

            foreach (var item in list)
            {
                if (item.IsHavePact == YesNoEnum.Yes)
                {
                    if (!TraceCodeCollectionStatusEnum.IsCollectCompleted(item.PactCollectStatus))
                    {
                        MessageBox.Show(item.DrugName + "包装采集未完成!");
                        return false;
                    }

                    if (item.PactActualCollectQty + item.PactAppealCollectQty != item.PactNeedCollectQty)
                    {
                        MessageBox.Show(item.DrugName + "包装数量采集不正确!");
                        return false;
                    }

                    if (item.PactTracCodgsList == null || item.PactTracCodgsList.Count != item.PactActualCollectQty)
                    {
                        MessageBox.Show(item.DrugName + "包装实采数量与追溯码数量不一致!");
                        return false;
                    }

                }

                if (item.IsHaveSplit == YesNoEnum.Yes && 1 == 2)//暂时弃用 发药保存时才进行分配
                {
                    if (!TraceCodeCollectionStatusEnum.IsCollectCompleted(item.SplitCollectStatus))
                    {
                        MessageBox.Show(item.DrugName + "拆零采集未完成!");
                        return false;

                    }

                    if (item.SplitActualCollectQty + item.SplitAppealCollectQty != item.SplitNeedCollectQty)
                    {
                        MessageBox.Show(item.DrugName + "数量采集不正确!");
                        return false;
                    }

                    if (item.SplitTracCodgsList == null || item.SplitTracCodgsList.Count != item.SplitActualCollectQty)
                    {
                        MessageBox.Show(item.DrugName + "拆零实采数量与追溯码数量不一致!");
                        return false;
                    }

                }



            }

            return true;
        }

        /// <summary>
        /// 追溯码采集回车事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCollectTraceCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            var scanCode = this.txtCollectTraceCode.Text.Trim();
            this.txtCollectTraceCode.Clear();
            this.txtCollectTraceCode.Focus();

            if (string.IsNullOrEmpty(scanCode))
            {
                return;
            }

            var applyNumber = string.Empty;

            foreach (KeyValuePair<string, List<string>> kv in ApplyTraceMap)
            {
                List<string> traceCodes = kv.Value;
                if (traceCodes == null)
                {
                    continue;
                }

                foreach (string code in traceCodes)
                {
                    if (string.Equals(code, scanCode, StringComparison.Ordinal))
                    {
                        applyNumber = kv.Key;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(applyNumber))
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(applyNumber))
            {
                ShowErrorMessage(scanCode, "该追溯码不在收费时采集的码值集合中。");
                return;
            }

            var collectMainInfo = new YbTraceCollectMain();

            var rowIndex = -1;
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                var info = this.fpSpread1_Sheet1.Rows[i].Tag as YbTraceCollectMain;
                if (info.ApplyNumber == applyNumber)
                {
                    collectMainInfo = info;
                    rowIndex = i;
                    break;
                }
            }

            if (collectMainInfo == null || collectMainInfo.ApplyNumber != applyNumber)
            {
                ShowErrorMessage(scanCode, "该追溯码未找到对应Tag信息实体！");
                return;
            }

            ChangeCollectMainInfo(scanCode, collectStartTime, collectMainInfo);

            SetCollectQtyFPColumnValue(rowIndex, collectMainInfo);

            SetCollectProgressFPColumnValue(rowIndex, collectMainInfo);

            if (collectMainInfo.IsHavePact == YesNoEnum.Yes)
            {
                SetPactTraceCodeFPColumnValue(rowIndex, collectMainInfo);
            }

            return;
        }


        private HashSet<string> _chargedTraceSet;

        private void BuildTraceLookup()
        {
            _chargedTraceSet = new HashSet<string>(StringComparer.Ordinal);
            if (ApplyTraceMap == null) return;

            foreach (List<string> traceCodes in ApplyTraceMap.Values)
            {
                foreach (string code in traceCodes)
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        _chargedTraceSet.Add(code.Trim());
                    }
                }
            }
        }

        /// <summary>
        /// 拆零维护
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSplit_Click(object sender, EventArgs e)
        {
            frmOnSiteInbound f = new frmOnSiteInbound();
            f.ShowDialog();

        }

        /// <summary>
        /// 每次扫码后修改采集信息实体
        /// </summary>
        /// <param name="scanCode"></param>
        /// <param name="collectStartTime"></param>
        /// <param name="info"></param>
        private void ChangeCollectMainInfo(
            string scanCode,
            DateTime collectStartTime,
            YbTraceCollectMain info)
        {
            if (string.IsNullOrEmpty(scanCode))
            {
                return;
            }

            if (info.PactTracCodgsList == null)
            {
                info.PactTracCodgsList = new List<string>();
            }

            if (!TraceCodeCollectionStatusEnum.IsValid(info.PactCollectStatus))
            {
                ShowErrorMessage(scanCode, "药品[" + info.DrugName + "]当前采集状态非法!");
                return;
            }

            if (TraceCodeCollectionStatusEnum.IsCollectCompleted(info.PactCollectStatus))
            {
                ShowErrorMessage(scanCode, "药品[" + info.DrugName + "]" + TraceCodeCollectionStatusEnum.GetDescription(info.PactCollectStatus) + ",请勿重复采集!");
                return;
            }

            if (info.PactTracCodgsList.Contains(scanCode))
            {
                ShowErrorMessage(scanCode, "该码已采集,请勿重复扫码!");
                return;
            }

            if (info.PactActualCollectQty + info.PactAppealCollectQty + 1 > info.PactNeedCollectQty)
            {
                ShowErrorMessage(scanCode, "药品[" + info.DrugName + "]采集数量非法,应采[" + info.PactNeedCollectQty + "] 已采[" + info.PactActualCollectQty + "] 申诉数量[" + info.PactAppealCollectQty + "]");
                return;
            }

            //TODO 提前拷贝一下对象,防止出现异常情况 导致值没回滚

            info.PactActualCollectQty = info.PactActualCollectQty + 1;
            info.PactTracCodgsList.Add(scanCode);
            if (string.IsNullOrEmpty(info.PactTracCodgs))
            {
                info.PactTracCodgs = scanCode;
            }
            else
            {
                info.PactTracCodgs = info.PactTracCodgs + ";" + scanCode;
            }

            info.PactUnCollectQty = info.PactNeedCollectQty - info.PactActualCollectQty - info.PactAppealCollectQty;

            info.PactCollectCompleteRate =
info.PactNeedCollectQty == 0
? "0%"
: Math.Round((info.PactActualCollectQty / info.PactNeedCollectQty) * 100, 2).ToString("0.##") + "%";

            info.PactCollectStatus = TraceCodeCollectionStatusEnum.GetStatusForQty(info.PactNeedCollectQty, info.PactActualCollectQty, info.PactAppealCollectQty);

            info.IdentifiyCode = scanCode.Substring(0, 7);

            info.CardNo = PatientAndApplyInfo.CardNo;
            info.PatientNo = PatientAndApplyInfo.PatientNo;
            info.PharmacyCode = PatientAndApplyInfo.DrugDeptCode;
            info.PharmacyName = PatientAndApplyInfo.DrugDeptName;
            info.DeptCode = PatientAndApplyInfo.DeptCode;
            info.DeptName = PatientAndApplyInfo.DeptName;

            if (TraceCodeCollectionStatusEnum.IsCollectCompleted(info.PactCollectStatus))
            {
                info.CollectStartTime = collectStartTime;
                info.CollectEndTime = DateTime.Now;
                info.CollectDurationMs =
    Convert.ToDecimal((info.CollectEndTime - info.CollectStartTime).TotalMilliseconds);
            }


        }

        /// <summary>
        /// 获取指定药品编码FP行索引
        /// </summary>
        /// <param name="drugCode"></param>
        /// <returns></returns>
        private int GetDrugCodeFPRowIndex(string drugCode)
        {
            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                var info = this.fpSpread1_Sheet1.Rows[i].Tag as YbTraceCollectMain;
                if (info.DrugCode == drugCode)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取FP绑定的数据源集合信息
        /// </summary>
        /// <returns></returns>
        private List<YbTraceCollectMain> GetYbTraceCollectMainListForFPData()
        {
            if (this.fpSpread1_Sheet1.RowCount <= 0)
            {
                return null;
            }

            var list = new List<YbTraceCollectMain>();

            for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
            {
                var info = this.fpSpread1_Sheet1.Rows[i].Tag as YbTraceCollectMain;
                list.Add(info);
            }

            return list;

        }

        /// <summary>
        /// 展示错误信息
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ShowErrorMessage(string scanCode, string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage) || string.IsNullOrEmpty(scanCode))
            {
                return;
            }

            //string ident = scanCode.Substring(0, 7);
            //string serial = scanCode.Substring(7);
            //var showcode = ident + " " + serial;

            this.panelError.Visible = true;
            this.lblErr.Text = "[" + scanCode + "]" + System.Environment.NewLine + errorMessage;

        }


        #region 弃用函数

        /// <summary>
        /// 设置FP表格中的CollectProgress列信息
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="info"></param>
        private void SetOldCollectProgressFPColumnValue(
            int rowIndex,
            YbTraceCollectMain info)
        {

            var columnIndex = (int)FPColumn.CollectProgress;

            var max = (int)info.PactNeedCollectQty + (int)info.SplitNeedCollectQty;
            var value = (int)info.PactActualCollectQty + (int)info.PactAppealCollectQty + (int)info.SplitActualCollectQty + (int)info.SplitAppealCollectQty;

            int percent = (int)(value * 100.0 / max);
            var progressCell = new ModernProgressCellType();
            progressCell.Minimum = 0;
            progressCell.Maximum = 100;

            this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = progressCell;
            this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value = percent;
            //var columnIndex = (int)FPColumn.CollectProgress;
            //int value = new Random().Next(0, 6);
            //int max = 5;
            //int percent = (int)(value * 100.0 / max);

            //var progressCell = new ModernProgressCellType();
            //progressCell.Minimum = 0;
            //progressCell.Maximum = 100;

            //this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = progressCell;
            //this.fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value = percent;
        }

        /// <summary>
        /// 设置FP表格中的CollectQty列信息
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="info"></param>
        private void SetOldCollectQtyFPColumnValue(
            int rowIndex,
            YbTraceCollectMain info)
        {

            var columnIndex = (int)FPColumn.CollectQty;

            var rtb = BuildRichTextBox();
            rtb.AppendText("\n");
            if (info.IsHaveSplit != YesNoEnum.Yes)
            {
                rtb.AppendText("\n");
            }

            if (info.IsHavePact == YesNoEnum.Yes)
            {
                rtb.SelectionFont = new Font("宋体", 12, FontStyle.Bold);
                rtb.SelectionColor = Color.FromArgb(52, 58, 64);
                var pactQtyStr = info.PactActualCollectQty + "/" + info.PactNeedCollectQty + " " + info.DrugPactUnit;
                rtb.AppendText(pactQtyStr);
                rtb.AppendText("\n");
            }


            if (info.IsHaveSplit == YesNoEnum.Yes)
            {
                rtb.SelectionFont = new Font("宋体", 12, FontStyle.Bold);
                rtb.SelectionColor = Color.FromArgb(40, 167, 69);
                var splitQtyStr = string.Empty;
                if (info.IsHaveSplit == YesNoEnum.Yes)
                {
                    splitQtyStr = info.SplitActualCollectQty + "/" + info.SplitNeedCollectQty + " " + info.DrugSplitUnit;
                }
                rtb.AppendText(splitQtyStr);
                rtb.SelectionBackColor = Color.Transparent;
            }

            this.fpSpread1_Sheet1.Rows[rowIndex].Tag = info;

            // 设置富文本内容
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = rtct;
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value = rtb.Rtf;
        }

        /// <summary>
        /// 设置FP表格中的PactTraceCode列信息
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="info"></param>
        private void SetOldPactTraceCodeFPColumnValue(
            int rowIndex,
            YbTraceCollectMain info)
        {
            var columnIndex = (int)FPColumn.PactTraceCode;

            if (info.PactTracCodgsList == null || !info.PactTracCodgsList.Any())
            {
                return;
            }

            var pactTracCodgsListText = string.Join(Environment.NewLine,
                               info.PactTracCodgsList.Select(code =>
                               {
                                   if (code.Length > 7)
                                   {
                                       string ident = code.Substring(0, 7);
                                       string serial = code.Substring(7);
                                       return ident + " " + serial;
                                   }
                                   else
                                   {
                                       return code;
                                   }
                               }).ToArray());


            this.fpSpread1_Sheet1.Rows[rowIndex].Tag = info;

            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = Color.FromArgb(79, 157, 166);
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Font = new Font("宋体", 10, FontStyle.Regular);
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = textCellType;
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value = pactTracCodgsListText;
        }

        /// <summary>
        /// 设置FP表格中的SplitTraceCode列信息
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="info"></param>
        private void SetOldSplitTraceCodeFPColumnValue(
            int rowIndex,
            YbTraceCollectMain info)
        {

            if (info.SplitTracCodgsList == null || !info.SplitTracCodgsList.Any())
            {
                return;
            }

            var columnIndex = (int)FPColumn.SplitTraceCode;

            if (info.IsHaveSplit == "1")
            {
                var splitTracCodgsListText = string.Join(Environment.NewLine,
                              info.SplitTracCodgsList.Select(code =>
                              {
                                  if (code.Length > 7)
                                  {
                                      string ident = code.Substring(0, 7);
                                      string serial = code.Substring(7);
                                      return ident + " " + serial;
                                  }
                                  else
                                  {
                                      return code;
                                  }
                              }).ToArray());


                this.fpSpread1_Sheet1.Rows[rowIndex].Tag = info;

                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = Color.FromArgb(79, 157, 166);
                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Font = new Font("宋体", 10, FontStyle.Regular);
                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = textCellType;
                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value = splitTracCodgsListText;
            }

        }


        /// <summary>
        /// 设置FP表格中的SplitBtn列信息
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="info"></param>
        private void SetOldSplitBtnFPColumnValue(
            int rowIndex,
            YbTraceCollectMain info)
        {
            //var columnIndex = (int)FPColumn.SplitBtn;

            //var buttonCellType = new ModernButtonCellType();

            //if (info.IsHaveSplit != "1")
            //{
            //    buttonCellType.TextColor = Color.Gray;
            //    buttonCellType.ButtonColor = Color.LightGray;
            //    buttonCellType.ButtonColor2 = Color.LightGray;
            //    buttonCellType.DarkColor = Color.Gray;
            //    buttonCellType.LightColor = Color.White;
            //}

            //buttonCellType.ButtonText = "拆零";
            //fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = buttonCellType;
        }

        /// <summary>
        /// 设置FP表格中的AppealBtn列信息
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="info"></param>
        private void SetOldAppealBtnFPColumnValue(
            int rowIndex,
            YbTraceCollectMain info)
        {
            var columnIndex = (int)FPColumn.AppealBtn;
            var buttonCellType = new ModernButtonCellType();
            buttonCellType.Style = Neusoft.HISFC.Components.Common.Controls.ModernStyles.ModernButtonCellType.ButtonStyle.Danger;
            buttonCellType.ButtonText = "申诉";
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = buttonCellType;
        }

        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        /// <summary>
        /// 一键赋码功能 (无码采集)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnYJFM_Click(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, List<string>> kv in ApplyTraceMap)
            {
                List<string> traceCodes = kv.Value;
                if (traceCodes == null)
                {
                    continue;
                }

                foreach (string code in traceCodes)
                {
                    var applyNumber = kv.Key;

                    var collectMainInfo = new YbTraceCollectMain();

                    var rowIndex = -1;
                    for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                    {
                        var info = this.fpSpread1_Sheet1.Rows[i].Tag as YbTraceCollectMain;
                        if (info.ApplyNumber == applyNumber)
                        {
                            collectMainInfo = info;
                            rowIndex = i;
                            break;
                        }
                    }

                    ChangeCollectMainInfo(code, collectStartTime, collectMainInfo);

                    SetCollectQtyFPColumnValue(rowIndex, collectMainInfo);

                    SetCollectProgressFPColumnValue(rowIndex, collectMainInfo);

                    if (collectMainInfo.IsHavePact == YesNoEnum.Yes)
                    {
                        SetPactTraceCodeFPColumnValue(rowIndex, collectMainInfo);
                    }

                }


            }
        }



    }

    /// <summary>
    /// FP控件的列定义
    /// </summary>
    public enum FPColumn
    {
        /// <summary>
        /// 药品信息
        /// </summary>
        DrugInfo = 0,

        /// <summary>
        /// 已采/应采
        /// </summary>
        CollectQty = 1,

        /// <summary>
        /// 采集进度
        /// </summary>
        CollectProgress = 2,

        /// <summary>
        /// 包装追溯码
        /// </summary>
        PactTraceCode = 3,

        /// <summary>
        /// 拆零追溯码
        /// </summary>
        SplitTraceCode = 4,

        /// <summary>
        /// 拆零
        /// </summary>
        //SplitBtn = 5,

        /// <summary>
        /// 申诉
        /// </summary>
        AppealBtn = 5
    }

}
