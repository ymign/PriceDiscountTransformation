using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.BizLogic.Pharmacy;
using Neusoft.HISFC.Components.Common.Controls.ModernStyles;
using Neusoft.HISFC.Models.MedicalTraceCode;
using FarPoint.Win.Spread;

namespace Neusoft.HISFC.Components.Common.Forms.YbTraceCode
{
    public partial class frmInboundDetailTraceCode : Form
    {

        protected TraceCodeDAL TraceCodeDAL = new TraceCodeDAL();

        public frmInboundDetailTraceCode()
        {
            InitializeComponent();
            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this, 12);

            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.btnSave, 12);
            Neusoft.FrameWork.WinForms.Classes.Function.ApplyRoundCorners(this.btnExport, 12);

            SetWebStyleGridLine(this.fpSpread1_Sheet1);
            this.fpSpread1.CellClick += OnFP1CellClick;
        }

        public bool Init(string inboundNo)
        {

            var list = this.TraceCodeDAL.GetYbTraceInboundOrderListForInboundNo(inboundNo);

            if (!list.Any())
            {
                MessageBox.Show("未找到指定入库记录!");
                return false;
            }

            var info = list.FirstOrDefault();
            var splitCount = list.Sum(p => p.SplitQty).ToString();
            var drugCount = list.Select(x => x.DrugCode).Distinct().Count();


            this.lblInboundNo.Text = info.InboundNo;
            this.lblDrugDeptName.Text = info.DrugDeptName;

            this.lblDrugCount.Text = drugCount.ToString();
            this.lblSplitCount.Text = splitCount;

            this.lblManuLotnum.Text = info.ManuLotnum;
            this.lblBchNo.Text = info.BchNo;

            this.lblManuDate.Text = info.ManuDate.ToString();
            this.lblExpyEnd.Text = info.ExpyEnd.ToString();

            this.lblInboundOperName.Text = info.InboundOperName.ToString();
            this.lblInboundOperTime.Text = info.InboundOperTime.ToString();

            var statusDes = "识别异常";
            var sourceTypeDes = "识别异常";

            switch (info.Status)
            {
                case "0":
                    statusDes = "入库成功";
                    break;
                case "1":
                    statusDes = "出库使用";
                    break;
                case "2":
                    statusDes = "退库成功";
                    break;
                case "3":
                    statusDes = "申请入库";
                    break;
                case "4":
                    statusDes = "申请退库";
                    break;
                default:
                    break;
            }

            switch (info.SourceType)
            {
                case "0":
                    sourceTypeDes = "HIS入库";
                    break;
                case "1":
                    sourceTypeDes = "手工拆零";
                    break;

                default:
                    break;
            }

            this.lblState.Text = statusDes;
            this.lblSourceType.Text = sourceTypeDes;

            var detailList = this.TraceCodeDAL.GetYbTraceInboundDetailForInboundNo(inboundNo);

            if (!detailList.Any()) 
            {
                MessageBox.Show("入库明细未找到!");
                return false;
            }

            this.fpSpread1_Sheet1.RowCount = 0;

            for (int i = 0; i < detailList.Count; i++)
            {

                this.fpSpread1_Sheet1.Rows.Add(i, 1);
                var detailInfo = detailList[i];
                this.fpSpread1_Sheet1.Rows[i].Tag = detailInfo;

                this.fpSpread1_Sheet1.Cells[i, 0].Value = detailInfo.DrugName;
                //this.fpSpread1_Sheet1.Cells[i, 0].ForeColor = Color.FromArgb(42, 164, 164);
                this.fpSpread1_Sheet1.Cells[i, 1].Value = detailInfo.SplitQty;
                this.fpSpread1_Sheet1.Cells[i, 2].Value = detailInfo.OriginalTraceCode;
                this.fpSpread1_Sheet1.Cells[i, 3].Value = detailInfo.ChildTraceCode;
                this.fpSpread1_Sheet1.Cells[i, 4].Value = detailInfo.Status;

                FarPointButtonCellType detailButton = new FarPointButtonCellType
                {
                    ButtonText = "追溯详情",
                    ButtonWidth = 76,
                    ButtonHeight = 28,
                    PrimaryColor = Color.FromArgb(64, 158, 158), 
                    HoverColor = Color.FromArgb(84, 178, 178),
                    ButtonAlignment = ContentAlignment.MiddleCenter,
                    //PrimaryColor = Color.FromArgb(24, 144, 255),
                    //HoverColor = Color.FromArgb(64, 169, 255),
                    BorderRadius = 4
                };
                this.fpSpread1_Sheet1.Cells[i, 5].CellType = detailButton;
            }

            return true;

        }

        private void OnFP1CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            var columnIndex = e.Column;
            var rowIndex = e.Row;

            if (columnIndex != 5)
            {
                return;
            }

            MessageBox.Show("功能暂未开放!");

            return;

            //var info = this.fpSpread1_Sheet1.Rows[rowIndex].Tag as YbTraceInboundDetail;

            //this.fpSpread1_Sheet1.Rows.Remove(rowIndex, 1);

        }

        private void frmInboundDetailTraceCode_Load(object sender, EventArgs e)
        {
            ApplyRoundCornersToAllPanels(this);
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能暂未开放!");

            return;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能暂未开放!");

            return;
        }

    }
}
