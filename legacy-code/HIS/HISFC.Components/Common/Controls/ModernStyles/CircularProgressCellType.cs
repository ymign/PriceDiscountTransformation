using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using FarPoint.Win.Spread.CellType;

namespace Neusoft.HISFC.Components.Common.Controls.ModernStyles
{
    /// <summary>
    /// 圆形进度条单元格类型
    /// 支持百分比显示、状态颜色和状态文字
    /// </summary>
    [Serializable]
    public class CircularProgressCellType : BaseCellType
    {
        #region 枚举定义

        /// <summary>
        /// 进度状态
        /// </summary>
        public enum ProgressStatus
        {
            /// <summary>开始 0%</summary>
            NotStarted,
            /// <summary>进行中 1-69%</summary>
            InProgress,
            /// <summary>接近完成 70-99%</summary>
            NearCompletion,
            /// <summary>已完成 100%</summary>
            Completed,
            /// <summary>超量 >100%</summary>
            Exceeded
        }

        #endregion

        #region 私有字段

        private double _percentage = 0;                           // 进度百分比
        private int _currentValue = 0;                            // 当前进度数值
        private int _maxValue = 100;                              // 最大进度数值
        private string _statusText = "开始";                      // 状态文字
        private ProgressStatus _status = ProgressStatus.NotStarted; // 进度状态
        private int _circleSize = 60;                            // 圆形大小
        private int _strokeWidth = 6;                            // 进度条宽度
        private int _cellHeight = 80;                            // 单元格高度（可自定义）
        private Font _percentageFont = new Font("微软雅黑", 12F, FontStyle.Bold);
        private Font _statusFont = new Font("微软雅黑", 9F);
        private Color _backgroundColor = Color.FromArgb(240, 240, 240); // 背景圆颜色

        // 状态颜色定义 - 匹配界面青绿色主题
        private Color _notStartedColor = Color.FromArgb(148, 163, 184);   // 浅灰色 - 开始
        private Color _inProgressColor = Color.FromArgb(42, 164, 164);    // 青绿色 - 进行中 (#2aa4a4)
        private Color _nearCompletionColor = Color.FromArgb(251, 146, 60); // 橙色 - 接近完成
        private Color _completedColor = Color.FromArgb(16, 185, 129);     // 绿色 - 已完成
        private Color _exceededColor = Color.FromArgb(239, 68, 68);       // 红色 - 超量

        #endregion

        #region 公共属性

        /// <summary>进度百分比 (0-200)</summary>
        public double Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = Math.Max(0, value);
                UpdateStatus();
            }
        }

        /// <summary>当前进度数值</summary>
        public int CurrentValue
        {
            get { return _currentValue; }
            set { _currentValue = Math.Max(0, value); }
        }

        /// <summary>最大进度数值</summary>
        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = Math.Max(1, value); }
        }

        /// <summary>状态文字</summary>
        public string StatusText
        {
            get { return _statusText; }
            set { _statusText = value ?? "进行中"; }
        }

        /// <summary>进度状态</summary>
        public ProgressStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>圆形大小</summary>
        public int CircleSize
        {
            get { return _circleSize; }
            set { _circleSize = Math.Max(30, value); }
        }

        /// <summary>进度条宽度</summary>
        public int StrokeWidth
        {
            get { return _strokeWidth; }
            set { _strokeWidth = Math.Max(2, value); }
        }

        /// <summary>单元格高度（可自定义，默认80像素）</summary>
        public int CellHeight
        {
            get { return _cellHeight; }
            set
            {
                _cellHeight = Math.Max(60, value); // 最小60像素
                // 根据高度自动调整圆形大小和字体
                AutoAdjustSizes();
            }
        }

        #endregion

        #region 构造函数

        public CircularProgressCellType()
        {
        }

        public CircularProgressCellType(SerializationInfo info, StreamingContext context)
        {
            _percentage = info.GetDouble("Percentage");
            _statusText = info.GetString("StatusText") ?? "进行中";
            _status = (ProgressStatus)info.GetInt32("Status");
            _circleSize = info.GetInt32("CircleSize");
            _strokeWidth = info.GetInt32("StrokeWidth");
            _cellHeight = info.GetInt32("CellHeight");
            AutoAdjustSizes();
        }

        /// <summary>
        /// 根据单元格高度自动调整圆形大小、字体大小等
        /// </summary>
        private void AutoAdjustSizes()
        {
            // 根据高度自动计算圆形大小（留出状态文字空间）
            int availableHeight = _cellHeight - 25; // 预留25像素给状态文字
            int maxCircleSize = Math.Min(availableHeight, 80); // 最大不超过80像素
            _circleSize = Math.Max(30, Math.Min(maxCircleSize, _circleSize));

            // 根据圆形大小调整字体
            float percentageFontSize = Math.Max(8F, Math.Min(14F, _circleSize * 0.2F));
            float statusFontSize = Math.Max(7F, Math.Min(10F, _circleSize * 0.15F));

            // 释放旧字体
            if (_percentageFont != null) _percentageFont.Dispose();
            if (_statusFont != null) _statusFont.Dispose();

            // 创建新字体
            _percentageFont = new Font("微软雅黑", percentageFontSize, FontStyle.Bold);
            _statusFont = new Font("微软雅黑", statusFontSize);

            // 根据圆形大小调整进度条宽度
            _strokeWidth = Math.Max(2, Math.Min(8, _circleSize / 10));
        }

        #endregion

        #region 核心绘制方法

        public override void PaintCell(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance,
            object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            if (g == null || r.Width <= 0 || r.Height <= 0) return;

            // 启用抗锯齿
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 解析值
            ParseValue(value);

            // 计算圆形区域 - 根据实际单元格高度自适应
            int availableHeight = r.Height - 25; // 预留状态文字空间
            int actualSize = Math.Min(_circleSize, Math.Min(r.Width - 10, availableHeight));

            // 垂直居中圆形（在预留状态文字空间的区域内）
            int circleY = r.Y + (availableHeight - actualSize) / 2;
            Rectangle circleRect = new Rectangle(
                r.X + (r.Width - actualSize) / 2,
                circleY,
                actualSize,
                actualSize
            );

            // 绘制圆形进度条
            DrawCircularProgress(g, circleRect);

            // 绘制百分比文字
            DrawPercentageText(g, circleRect);

            // 绘制状态文字
            DrawStatusText(g, r, circleRect);
        }

        /// <summary>
        /// 绘制圆形进度条
        /// </summary>
        private void DrawCircularProgress(Graphics g, Rectangle rect)
        {
            // 计算进度条区域
            Rectangle progressRect = new Rectangle(
                rect.X + _strokeWidth / 2,
                rect.Y + _strokeWidth / 2,
                rect.Width - _strokeWidth,
                rect.Height - _strokeWidth
            );

            // 绘制背景圆
            using (Pen backgroundPen = new Pen(_backgroundColor, _strokeWidth))
            {
                backgroundPen.StartCap = LineCap.Round;
                backgroundPen.EndCap = LineCap.Round;
                g.DrawEllipse(backgroundPen, progressRect);
            }

            // 绘制进度弧
            if (_percentage > 0)
            {
                Color progressColor = GetProgressColor();
                using (Pen progressPen = new Pen(progressColor, _strokeWidth))
                {
                    progressPen.StartCap = LineCap.Round;
                    progressPen.EndCap = LineCap.Round;

                    // 计算角度 (从顶部开始，顺时针)
                    float sweepAngle = (float)(Math.Min(_percentage, 100) * 3.6); // 最多360度

                    // 如果超过100%，绘制完整圆圈
                    if (_percentage >= 100)
                    {
                        g.DrawEllipse(progressPen, progressRect);

                        // 如果超量，再绘制一个内圈表示超量部分
                        if (_percentage > 100)
                        {
                            Rectangle innerRect = new Rectangle(
                                progressRect.X + _strokeWidth,
                                progressRect.Y + _strokeWidth,
                                progressRect.Width - _strokeWidth * 2,
                                progressRect.Height - _strokeWidth * 2
                            );

                            float exceededAngle = (float)(Math.Min(_percentage - 100, 100) * 3.6);
                            using (Pen exceededPen = new Pen(_exceededColor, _strokeWidth / 2))
                            {
                                exceededPen.StartCap = LineCap.Round;
                                exceededPen.EndCap = LineCap.Round;
                                g.DrawArc(exceededPen, innerRect, -90, exceededAngle);
                            }
                        }
                    }
                    else
                    {
                        g.DrawArc(progressPen, progressRect, -90, sweepAngle);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制百分比文字
        /// </summary>
        private void DrawPercentageText(Graphics g, Rectangle rect)
        {
            string percentText = Math.Round(_percentage, 0).ToString() + "%";

            using (SolidBrush textBrush = new SolidBrush(GetProgressColor()))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                g.DrawString(percentText, _percentageFont, textBrush, rect, sf);
            }
        }

        /// <summary>
        /// 绘制状态文字
        /// </summary>
        private void DrawStatusText(Graphics g, Rectangle cellRect, Rectangle circleRect)
        {
            Rectangle statusRect = new Rectangle(
                cellRect.X,
                circleRect.Bottom + 5,
                cellRect.Width,
                cellRect.Bottom - circleRect.Bottom - 5
            );

            Color statusColor = GetStatusTextColor();
            Color statusBgColor = GetStatusBackgroundColor();

            // 绘制状态背景
            Rectangle statusBgRect = new Rectangle(
                statusRect.X + (statusRect.Width - 60) / 2,
                statusRect.Y,
                60,
                20
            );

            using (SolidBrush bgBrush = new SolidBrush(statusBgColor))
            {
                GraphicsHelper.FillRoundedRectangle(g, bgBrush, statusBgRect, 10);
            }

            // 绘制状态文字
            using (SolidBrush textBrush = new SolidBrush(statusColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                g.DrawString(_statusText, _statusFont, textBrush, statusBgRect, sf);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 解析单元格值
        /// </summary>
        private void ParseValue(object value)
        {
            if (value != null)
            {
                double percentage;
                if (double.TryParse(value.ToString(), out percentage))
                {
                    Percentage = percentage;
                }
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        private void UpdateStatus()
        {
            if (_percentage == 0)
            {
                _status = ProgressStatus.NotStarted;
                _statusText = "开始";
            }
            else if (_percentage > 100)
            {
                _status = ProgressStatus.Exceeded;
                _statusText = "超量";
            }
            else if (_percentage >= 100)
            {
                _status = ProgressStatus.Completed;
                _statusText = "完成";
            }
            else if (_percentage >= 70)
            {
                _status = ProgressStatus.NearCompletion;
                _statusText = "接近完成";
            }
            else
            {
                _status = ProgressStatus.InProgress;
                _statusText = "进行中";
            }
        }

        /// <summary>
        /// 获取进度条颜色
        /// </summary>
        private Color GetProgressColor()
        {
            switch (_status)
            {
                case ProgressStatus.NotStarted:
                    return _notStartedColor;
                case ProgressStatus.InProgress:
                    return _inProgressColor;
                case ProgressStatus.NearCompletion:
                    return _nearCompletionColor;
                case ProgressStatus.Completed:
                    return _completedColor;
                case ProgressStatus.Exceeded:
                    return _exceededColor;
                default:
                    return _inProgressColor;
            }
        }

        /// <summary>
        /// 获取状态文字颜色
        /// </summary>
        private Color GetStatusTextColor()
        {
            switch (_status)
            {
                case ProgressStatus.NotStarted:
                    return Color.FromArgb(75, 85, 99);
                case ProgressStatus.Exceeded:
                    return Color.White;
                default:
                    return Color.White;
            }
        }

        /// <summary>
        /// 获取状态背景颜色
        /// </summary>
        private Color GetStatusBackgroundColor()
        {
            switch (_status)
            {
                case ProgressStatus.NotStarted:
                    return Color.FromArgb(229, 231, 235);
                case ProgressStatus.InProgress:
                    return Color.FromArgb(42, 164, 164);
                case ProgressStatus.NearCompletion:
                    return Color.FromArgb(251, 146, 60);
                case ProgressStatus.Completed:
                    return Color.FromArgb(16, 185, 129);
                case ProgressStatus.Exceeded:
                    return Color.FromArgb(239, 68, 68);
                default:
                    return Color.FromArgb(42, 164, 164);
            }
        }

        /// <summary>
        /// 设置进度数据（百分比）
        /// </summary>
        public void SetProgress(double percentage)
        {
            Percentage = percentage;
        }

        /// <summary>
        /// 设置进度数据（当前值和最大值）
        /// </summary>
        public void SetProgress(int currentValue, int maxValue)
        {
            CurrentValue = currentValue;
            MaxValue = maxValue;
            if (maxValue > 0)
            {
                Percentage = (double)currentValue / maxValue * 100;
            }
            else
            {
                Percentage = 0;
            }
        }

        /// <summary>
        /// 设置进度数据（包含状态文字）
        /// </summary>
        public void SetProgress(double percentage, string statusText)
        {
            Percentage = percentage;
            if (!string.IsNullOrEmpty(statusText))
            {
                StatusText = statusText;
            }
        }

        /// <summary>
        /// 设置进度数据（当前值、最大值和状态文字）
        /// </summary>
        public void SetProgress(int currentValue, int maxValue, string statusText)
        {
            SetProgress(currentValue, maxValue);
            if (!string.IsNullOrEmpty(statusText))
            {
                StatusText = statusText;
            }
        }

        #endregion

        #region BaseCellType 实现

        public override string Format(object obj)
        {
            if (obj == null) return "0%";
            return obj.ToString() + "%";
        }

        public override object Parse(string s)
        {
            double result;
            if (double.TryParse(s.Replace("%", ""), out result))
                return result;
            return 0;
        }

        public override Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            return null; // 不支持编辑
        }

        public override object GetEditorValue()
        {
            return _percentage;
        }

        public override void SetEditorValue(object value)
        {
            // 不支持编辑
        }

        public override Size GetPreferredSize(Graphics g, Size size, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            return new Size(_circleSize + 20, _cellHeight);
        }

        public override bool IsValid(object value)
        {
            return true;
        }

        public override object Clone()
        {
            CircularProgressCellType clone = new CircularProgressCellType();
            clone._percentage = this._percentage;
            clone._statusText = this._statusText;
            clone._status = this._status;
            clone._circleSize = this._circleSize;
            clone._strokeWidth = this._strokeWidth;
            clone._cellHeight = this._cellHeight;
            clone.AutoAdjustSizes();
            return clone;
        }

        #endregion

        #region 序列化支持

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //base.GetObjectData(info, context);
            info.AddValue("Percentage", _percentage);
            info.AddValue("StatusText", _statusText);
            info.AddValue("Status", (int)_status);
            info.AddValue("CircleSize", _circleSize);
            info.AddValue("StrokeWidth", _strokeWidth);
            info.AddValue("CellHeight", _cellHeight);
        }

        #endregion
    }

    /// <summary>
    /// 图形绘制辅助类 (Framework 3.5兼容)
    /// </summary>
    public static class GraphicsHelper
    {
        /// <summary>
        /// 绘制圆角矩形
        /// </summary>
        public static void FillRoundedRectangle(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            try
            {
                path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
            finally
            {
                path.Dispose();
            }
        }
    }
}
