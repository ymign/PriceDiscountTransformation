using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    public partial class frmBatchScanReturn : Form
    {
        public List<ScanDrugInfo> ScanDrugList = new List<ScanDrugInfo>();

        protected FarPoint.Win.Spread.StyleInfo validStyle = new FarPoint.Win.Spread.StyleInfo
         {
             ForeColor = Color.Green,
             Font = new Font("Arial", 10, FontStyle.Bold)
         };

        protected FarPoint.Win.Spread.StyleInfo invalidStyle = new FarPoint.Win.Spread.StyleInfo
         {
             ForeColor = Color.Red,
             Font = new Font("Arial", 10, FontStyle.Bold) // 
         };

        /// <summary>
        /// Y码与药品标识码对照
        /// </summary>
        public Dictionary<string, HashSet<string>> DrugCodeToTraceCodes = new Dictionary<string, HashSet<string>>();

        public frmBatchScanReturn()
        {
            InitializeComponent();
            Init();
        }


        private void Init()
        {
            InitStyle();

            // 设置列锁定
            this.fpSpread2_Sheet1.Columns[1].Locked = true;

        }

        public void LoadData(List<ScanDrugInfo> list)
        {
            if (!list.Any())
            {
                MessageBox.Show("绑定数据失败，数据源为空。");
                return;
            }
            ScanDrugList = list;

            this.fpSpread1_Sheet1.RowCount = 0;

            for (int i = 0; i < ScanDrugList.Count; i++)
            {
                this.fpSpread1_Sheet1.Rows.Add(i, 1);
                var info = list[i];
                this.fpSpread1_Sheet1.Rows[i].Tag = info;

                this.fpSpread1_Sheet1.Cells[i, 0].Text = (i + 1).ToString();
                this.fpSpread1_Sheet1.Cells[i, 1].Text = info.DrugName;
                this.fpSpread1_Sheet1.Cells[i, 2].Text = info.DrugSpecs;
                this.fpSpread1_Sheet1.Cells[i, 3].Text = info.ScanQty + info.PackUnit;
                this.fpSpread1_Sheet1.Cells[i, 4].Text = info.AlreadyScanQty + "/" + info.ScanQty;
                this.fpSpread1_Sheet1.Cells[i, 5].Text = "少扫" + info.ScanQty + info.PackUnit;

                this.fpSpread1_Sheet1.Cells[i, 4].ForeColor = Color.Red;
                this.fpSpread1_Sheet1.Cells[i, 5].ForeColor = Color.Red;

            }
            this.label1.Text = "共" + ScanDrugList.Count + "个品种:需扫码" + ScanDrugList.Sum(a => a.ScanQty) + "次,已扫码成功0次";


        }

        private void InitStyle()
        {
            this.fpSpread1_Sheet1.ColumnHeader.Rows[0].Height = 36;

            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.btnOK.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.btnOK.BackColor = Color.Transparent;
            this.btnOK.FlatStyle = FlatStyle.Flat;

            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.btnCancel.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.btnCancel.BackColor = Color.Transparent;
            this.btnCancel.FlatStyle = FlatStyle.Flat;


            this.btnOK.Paint += btnCancel_Paint;
            this.btnOK.MouseEnter += btnOK_MouseEnter;
            this.btnOK.MouseLeave += btnOK_MouseLeave;
            this.btnOK.Click += btnOK_Click;

            this.btnCancel.Click += btnCancel_Click;
            this.btnCancel.Paint += btnOK_Paint;
            this.btnCancel.MouseEnter += btnCancel_MouseEnter;
            this.btnCancel.MouseLeave += btnCancel_MouseLeave;

        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void InitializeSpread2()
        {





            //this.fpSpread2.ActiveSheet.SetActiveCell(0, 0);

            //this.fpSpread2.EditMode = true;

            //this.fpSpread2.Focus();


        }

        private void frmPreAlert_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.ClientRectangle, // 渐变区域
                    Color.FromArgb(255, 180, 230, 255), // 温和的浅蓝色
                    Color.FromArgb(255, 180, 255, 180), // 浅绿色
                45f)) // 渐变角度
            {
                // 使用渐变刷填充整个窗体
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void btnCancel_MouseEnter(object sender, EventArgs e)
        {
            isHovered = true;  // 设置鼠标悬停标志
            Button btn = (Button)sender;
            btn.Invalidate();  // 强制重新绘制按钮
        }

        private void btnCancel_MouseLeave(object sender, EventArgs e)
        {
            isHovered = false; // 设置鼠标离开标志
            Button btn = (Button)sender;
            btn.Invalidate();  // 强制重新绘制按钮
        }

        private void btnCancel_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;

            // 设置抗锯齿
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 获取按钮的背景颜色（鼠标悬停时改变）
            Color startColor = isHovered ? Color.FromArgb(255, 255, 140, 140) : Color.FromArgb(255, 245, 108, 108); // 浅色 vs 正常色
            Color endColor = isHovered ? Color.FromArgb(255, 235, 98, 98) : Color.FromArgb(255, 225, 88, 88); // 浅色 vs 深色

            // 清除默认背景
            e.Graphics.Clear(btn.Parent.BackColor);

            // 定义圆角半径
            int radius = 6;

            // 创建圆角矩形路径
            Rectangle rect = btn.ClientRectangle;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.Left, rect.Top, radius * 2, radius * 2, 180, 90); // 左上角
                path.AddArc(rect.Right - radius * 2, rect.Top, radius * 2, radius * 2, 270, 90); // 右上角
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90); // 右下角
                path.AddArc(rect.Left, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90); // 左下角
                path.CloseFigure();

                // 填充渐变背景
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    startColor,
                    endColor,
                    45f)) // 渐变角度
                {
                    e.Graphics.FillPath(brush, path);
                }

                // 绘制按钮文字
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(btn.Text, btn.Font, textBrush, rect, sf);
                }
            }
        }


        private void btnOK_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;

            // 设置抗锯齿
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 获取按钮的背景颜色（鼠标悬停时改变）
            Color startColor = isHovered ? Color.FromArgb(255, 100, 180, 255) : Color.FromArgb(255, 64, 158, 255); // 浅色 vs 正常色
            Color endColor = isHovered ? Color.FromArgb(255, 80, 150, 255) : Color.FromArgb(255, 50, 130, 240); // 浅色 vs 深色

            // 创建圆角矩形路径
            int radius = 6; // 圆角半径
            Rectangle rect = btn.ClientRectangle;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.Left, rect.Top, radius * 2, radius * 2, 180, 90); // 左上角
                path.AddArc(rect.Right - radius * 2, rect.Top, radius * 2, radius * 2, 270, 90); // 右上角
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90); // 右下角
                path.AddArc(rect.Left, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90); // 左下角
                path.CloseFigure();

                // 填充圆角背景
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    startColor,
                    endColor,
                    45f))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // 仅绘制按钮文字，无边框
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(btn.Text, btn.Font, textBrush, rect, sf);
                }
            }
        }

        private void btnOK_MouseEnter(object sender, EventArgs e)
        {
            isHovered = true;  // 设置鼠标悬停标志
            Button btn = (Button)sender;
            btn.Invalidate();  // 强制重新绘制按钮
        }

        private void btnOK_MouseLeave(object sender, EventArgs e)
        {
            isHovered = false; // 设置鼠标离开标志
            Button btn = (Button)sender;
            btn.Invalidate();  // 强制重新绘制按钮
        }

        // 声明一个变量来跟踪鼠标是否悬停
        private bool isHovered = false;

        // 辅助方法：绘制圆角矩形路径
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.Left, rect.Top, radius * 2, radius * 2, 180, 90); // 左上角
            path.AddArc(rect.Right - radius * 2, rect.Top, radius * 2, radius * 2, 270, 90); // 右上角
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90); // 右下角
            path.AddArc(rect.Left, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90); // 左下角
            path.CloseFigure();
            return path;
        }

        private void frmBatchScanReturn_Load(object sender, EventArgs e)
        {
            //InitializeSpread2();
        }

        private void frmBatchScanReturn_Shown(object sender, EventArgs e)
        {
            this.fpSpread2.Focus();

            this.fpSpread2_Sheet1.SetActiveCell(0, 0);

            this.fpSpread2.EditMode = true;

        }


        /// <summary>
        /// fp控件取消编辑模式事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread2_EditModeOff(object sender, EventArgs e)
        {
            int currentRow = fpSpread2_Sheet1.ActiveRowIndex;
            int currentCol = 0;//fpSpread2_Sheet1.ActiveColumnIndex;
            string errMsg = "";

            var traceCode = string.Empty;
            var drugCode = string.Empty;

            try
            {
                if (fpSpread2_Sheet1.GetValue(currentRow, currentCol) != null)
                {
                    traceCode = fpSpread2_Sheet1.GetValue(currentRow, currentCol).ToString();

                }

                if (string.IsNullOrEmpty(traceCode))
                {
                    errMsg = "空字符";
                    return;
                }

                this.fpSpread2_Sheet1.Cells[currentRow, 0].Locked = true;


                if (traceCode.Length != 20)
                {
                    errMsg = "长度非法";
                    return;

                }
                if (!traceCode.All(char.IsDigit))
                {
                    errMsg = "格式错误";
                    return;
                }

                //追溯码前7位代表药品标识码
                var identifierCode = traceCode.Substring(0, 7);
                drugCode = FindDrugCodeByIdentifierCode(identifierCode);

                if (string.IsNullOrEmpty(drugCode))
                {
                    errMsg = "未对照";
                    return;
                }
                else
                {
                    if (!ScanDrugList.Any(a => a.DrugCode == drugCode))
                    {
                        errMsg = "非列表";
                        return;
                    }
                }





            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

                if (string.IsNullOrEmpty(errMsg))
                {
                    fpSpread2_Sheet1.SetValue(currentRow, 1, "正确");
                    fpSpread2_Sheet1.SetStyleInfo(currentRow, 0, validStyle);
                    fpSpread2_Sheet1.SetStyleInfo(currentRow, 1, validStyle);
                    UpdateInfoWhenScan(drugCode,traceCode);
                }
                else
                {
                    fpSpread2_Sheet1.SetValue(currentRow, 1, errMsg);
                    fpSpread2_Sheet1.SetStyleInfo(currentRow, 0, invalidStyle);
                    fpSpread2_Sheet1.SetStyleInfo(currentRow, 1, invalidStyle);
                }
                if (errMsg != "空字符")
                {

                    fpSpread2_Sheet1.AddRows(currentRow + 1, 1);
                    fpSpread2_Sheet1.SetActiveCell(currentRow + 1, 0);
                    this.fpSpread2.EditModePermanent = true;//重新设置编辑模式
                }

                if (!ScanDrugList.Any(a => a.ScanQty != a.AlreadyScanQty)) 
                {
                    btnOK_Click(null, null);
                }

            }
        }

        /// <summary>
        /// 当扫码完成后 更新相关信息
        /// </summary>
        private void UpdateInfoWhenScan(string drugCode, string traceCode)
        {
            if (!ScanDrugList.Any(a => a.DrugCode == drugCode))
            {
                return;
            }

            foreach (var item in ScanDrugList)
            {
                if (item.DrugCode != drugCode)
                {
                    continue;
                }

                if (item.AlreadyScanQty == item.ScanQty)
                {
                    item.ScanState = "扫码完成";
                    continue;
                }

                if (item.AlreadyScanQty > item.ScanQty)
                {
                    item.AlreadyScanQty = item.ScanQty;
                }

                item.AlreadyScanQty = item.AlreadyScanQty + 1;

                if (item.AlreadyScanQty == item.ScanQty)
                {
                    item.ScanState = "扫码完成";
                }
                else
                {
                    item.ScanState = "少扫" + (item.ScanQty - item.AlreadyScanQty) + item.PackUnit;
                }

                if (string.IsNullOrEmpty(item.DrugTracCodg))
                {
                    item.DrugTracCodg = traceCode;
                }
                else {
                    item.DrugTracCodg = item.DrugTracCodg + ";" + traceCode;
                }


                for (int i = 0; i < this.fpSpread1_Sheet1.RowCount; i++)
                {
                    var info = this.fpSpread1_Sheet1.Rows[i].Tag as ScanDrugInfo;
                    if (info.DrugCode != item.DrugCode) 
                    {
                        continue;
                    }
                    this.fpSpread1_Sheet1.Cells[i, 4].Text = item.AlreadyScanQty + "/" + item.ScanQty;
                    this.fpSpread1_Sheet1.Cells[i, 5].Text = item.ScanState;

                    if (item.ScanQty == item.AlreadyScanQty) 
                    {
                        this.fpSpread1_Sheet1.Cells[i, 4].ForeColor = Color.Green;
                        this.fpSpread1_Sheet1.Cells[i, 5].ForeColor = Color.Green;
                    }
                   

                }


            }


        }

        // 查找标识码对应的药品编码
        private string FindDrugCodeByIdentifierCode(string identifierCode)
        {

            foreach (var entry in DrugCodeToTraceCodes)
            {
                if (entry.Value.Contains(identifierCode))
                {
                    return entry.Key; // 返回药品编码
                }
            }
            return "";
        }

    }


    public class ScanDrugInfo
    {
        /// <summary>
        /// 药品编码
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 药品名称
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string DrugSpecs { get; set; }

        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackUnit { get; set; }

        /// <summary>
        /// 需扫码数量
        /// </summary>
        public int ScanQty { get; set; }

        /// <summary>
        /// 已扫码数量
        /// </summary>
        public int AlreadyScanQty { get; set; }

        /// <summary>
        /// 扫码状态
        /// </summary>
        public string ScanState { get; set; }

        /// <summary>
        /// 追溯码
        /// </summary>
        public string DrugTracCodg { get; set; }

        /// <summary>
        /// 标识码(即追溯码前七位)
        /// </summary>
        public string DrugIdentifiyCode { get; set; }
    }
}
