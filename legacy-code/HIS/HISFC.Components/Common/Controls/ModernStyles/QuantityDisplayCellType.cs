using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using FarPoint.Win.Spread.CellType;

namespace Neusoft.HISFC.Components.Common.Controls.ModernStyles
{
    /// <summary>
    /// 双行数量显示单元格类型
    /// 上行显示"应采"，下行显示"已采"
    /// 支持包装、拆零、混合三种显示模式
    /// </summary>
    [Serializable]
    public class QuantityDisplayCellType : BaseCellType
    {
        #region 枚举定义

        /// <summary>
        /// 显示模式
        /// </summary>
        public enum DisplayMode
        {
            /// <summary>只显示包装</summary>
            PackageOnly,
            /// <summary>只显示拆零</summary>
            SplitOnly,
            /// <summary>显示包装和拆零</summary>
            Both
        }

        /// <summary>
        /// 数量状态
        /// </summary>
        public enum QuantityStatus
        {
            /// <summary>未开始</summary>
            NotStarted,
            /// <summary>进行中</summary>
            InProgress,
            /// <summary>已完成</summary>
            Completed,
            /// <summary>超量</summary>
            Exceeded
        }

        #endregion

        #region 私有字段

        private int _packageRequired = 0;      // 应采包装数
        private int _packageCollected = 0;     // 已采包装数
        private int _splitRequired = 0;        // 应采拆零数
        private int _splitCollected = 0;       // 已采拆零数
        private string _packageUnit = "盒";    // 包装单位
        private string _splitUnit = "支";      // 拆零单位
        private DisplayMode _displayMode = DisplayMode.Both;
        private Font _font = new Font("微软雅黑", 9F);
        private Color _requiredColor = Color.FromArgb(64, 64, 64);        // 应采颜色（深灰色）
        private Color _collectedColor = Color.FromArgb(0, 123, 255);      // 已采颜色（亮蓝色）
        private Color _completedColor = Color.FromArgb(40, 167, 69);      // 完成颜色（鲜绿色）
        private Color _exceededColor = Color.FromArgb(220, 53, 69);       // 超量颜色（鲜红色）

        // 边框相关属性
        private bool _showBorder = true;                                  // 是否显示边框
        private int _borderWidth = 1;                                     // 边框宽度
        private Color _borderColor = Color.FromArgb(221, 221, 221);      // 边框颜色
        private DashStyle _borderStyle = DashStyle.Solid;                // 边框样式

        #endregion

        #region 公共属性

        /// <summary>应采包装数</summary>
        public int PackageRequired
        {
            get { return _packageRequired; }
            set { _packageRequired = Math.Max(0, value); }
        }

        /// <summary>已采包装数</summary>
        public int PackageCollected
        {
            get { return _packageCollected; }
            set { _packageCollected = Math.Max(0, value); }
        }

        /// <summary>应采拆零数</summary>
        public int SplitRequired
        {
            get { return _splitRequired; }
            set { _splitRequired = Math.Max(0, value); }
        }

        /// <summary>已采拆零数</summary>
        public int SplitCollected
        {
            get { return _splitCollected; }
            set { _splitCollected = Math.Max(0, value); }
        }

        /// <summary>显示模式</summary>
        public DisplayMode Mode
        {
            get { return _displayMode; }
            set { _displayMode = value; }
        }

        /// <summary>字体</summary>
        public Font Font
        {
            get { return _font; }
            set { _font = value != null ? value : new Font("微软雅黑", 9F); }
        }

        /// <summary>包装单位</summary>
        public string PackageUnit
        {
            get { return _packageUnit; }
            set { _packageUnit = string.IsNullOrEmpty(value) ? "盒" : value; }
        }

        /// <summary>拆零单位</summary>
        public string SplitUnit
        {
            get { return _splitUnit; }
            set { _splitUnit = string.IsNullOrEmpty(value) ? "支" : value; }
        }

        /// <summary>是否显示边框</summary>
        public bool ShowBorder
        {
            get { return _showBorder; }
            set { _showBorder = value; }
        }

        /// <summary>边框宽度（像素）</summary>
        public int BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = Math.Max(0, value); }
        }

        /// <summary>边框颜色</summary>
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        /// <summary>边框样式</summary>
        public DashStyle BorderStyle
        {
            get { return _borderStyle; }
            set { _borderStyle = value; }
        }

        #endregion

        #region 构造函数

        public QuantityDisplayCellType()
        {
        }

        public QuantityDisplayCellType(SerializationInfo info, StreamingContext context)
        {
            _packageRequired = info.GetInt32("PackageRequired");
            _packageCollected = info.GetInt32("PackageCollected");
            _splitRequired = info.GetInt32("SplitRequired");
            _splitCollected = info.GetInt32("SplitCollected");
            _displayMode = (DisplayMode)info.GetInt32("DisplayMode");
            _packageUnit = info.GetString("PackageUnit") ?? "盒";
            _splitUnit = info.GetString("SplitUnit") ?? "支";
            _showBorder = info.GetBoolean("ShowBorder");
            _borderWidth = info.GetInt32("BorderWidth");
            _borderColor = Color.FromArgb(info.GetInt32("BorderColor"));
            _borderStyle = (DashStyle)info.GetInt32("BorderStyle");
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

            // 绘制背景
            DrawBackground(g, r, appearance, isSelected);

            // 绘制内容
            DrawQuantityContent(g, r, zoomFactor);
        }

        /// <summary>
        /// 绘制背景
        /// </summary>
        private void DrawBackground(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance, bool isSelected)
        {
            Color bgColor = isSelected ? SystemColors.Highlight :
                           (appearance != null && appearance.BackColor != Color.Empty ? appearance.BackColor : SystemColors.Window);

            SolidBrush brush = new SolidBrush(bgColor);
            try
            {
                g.FillRectangle(brush, r);
            }
            finally
            {
                brush.Dispose();
            }

            // 绘制边框（如果启用）
            if (_showBorder && _borderWidth > 0)
            {
                DrawBorder(g, r);
            }
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        private void DrawBorder(Graphics g, Rectangle r)
        {
            if (_borderWidth <= 0) return;

            using (Pen pen = new Pen(_borderColor, _borderWidth))
            {
                pen.DashStyle = _borderStyle;
                pen.StartCap = LineCap.Square;
                pen.EndCap = LineCap.Square;

                // 根据边框宽度调整绘制区域
                int halfWidth = _borderWidth / 2;
                Rectangle borderRect = new Rectangle(
                    r.X + halfWidth,
                    r.Y + halfWidth,
                    r.Width - _borderWidth,
                    r.Height - _borderWidth
                );

                g.DrawRectangle(pen, borderRect);
            }
        }

        /// <summary>
        /// 绘制数量内容
        /// </summary>
        private void DrawQuantityContent(Graphics g, Rectangle r, float zoomFactor)
        {
            // 根据边框宽度调整内容区域
            int margin = _showBorder ? Math.Max(4, _borderWidth + 2) : 4;
            Rectangle contentRect = new Rectangle(r.X + margin, r.Y + 2, r.Width - margin * 2, r.Height - 4);
            int lineHeight = contentRect.Height / 2;

            switch (_displayMode)
            {
                case DisplayMode.PackageOnly:
                    DrawPackageQuantity(g, contentRect, zoomFactor);
                    break;
                case DisplayMode.SplitOnly:
                    DrawSplitQuantity(g, contentRect, zoomFactor);
                    break;
                case DisplayMode.Both:
                    DrawBothQuantities(g, contentRect, lineHeight, zoomFactor);
                    break;
            }
        }

        /// <summary>
        /// 绘制包装数量（单独显示）
        /// </summary>
        private void DrawPackageQuantity(Graphics g, Rectangle rect, float zoomFactor)
        {
            QuantityStatus status = GetPackageStatus();
            string requiredText = "应采：" + _packageRequired.ToString() + _packageUnit;
            string collectedText = "已采：" + _packageCollected.ToString() + _packageUnit;

            DrawQuantityLine(g, rect, 0, requiredText, _requiredColor, zoomFactor);
            DrawQuantityLine(g, rect, rect.Height / 2, collectedText, GetStatusColor(status), zoomFactor);
        }

        /// <summary>
        /// 绘制拆零数量（单独显示）
        /// </summary>
        private void DrawSplitQuantity(Graphics g, Rectangle rect, float zoomFactor)
        {
            QuantityStatus status = GetSplitStatus();
            string requiredText = "应采：" + _splitRequired.ToString() + _splitUnit;
            string collectedText = "已采：" + _splitCollected.ToString() + _splitUnit;

            DrawQuantityLine(g, rect, 0, requiredText, _requiredColor, zoomFactor);
            DrawQuantityLine(g, rect, rect.Height / 2, collectedText, GetStatusColor(status), zoomFactor);
        }

        /// <summary>
        /// 绘制包装和拆零数量（混合显示）
        /// </summary>
        private void DrawBothQuantities(Graphics g, Rectangle rect, int lineHeight, float zoomFactor)
        {
            QuantityStatus packageStatus = GetPackageStatus();
            QuantityStatus splitStatus = GetSplitStatus();

            // 上半部分 - 应采数量
            string requiredText = "应采：" + _packageRequired.ToString() + _packageUnit + "+" + _splitRequired.ToString() + _splitUnit;
            DrawQuantityLine(g, rect, 0, requiredText, _requiredColor, zoomFactor);

            // 下半部分 - 已采数量
            string collectedText = "已采：" + _packageCollected.ToString() + _packageUnit + "+" + _splitCollected.ToString() + _splitUnit;
            QuantityStatus overallStatus = GetOverallStatus();
            DrawQuantityLine(g, rect, lineHeight, collectedText, GetStatusColor(overallStatus), zoomFactor);
        }

        /// <summary>
        /// 绘制单行数量文本
        /// </summary>
        private void DrawQuantityLine(Graphics g, Rectangle rect, int yOffset, string text, Color color, float zoomFactor)
        {
            Font font = new Font(_font.FontFamily, _font.Size * zoomFactor, _font.Style);
            Rectangle textRect = new Rectangle(rect.X, rect.Y + yOffset, rect.Width, rect.Height / 2);

            SolidBrush brush = new SolidBrush(color);
            StringFormat format = new StringFormat();
            try
            {
                format.Alignment = StringAlignment.Center;  // 水平居中
                format.LineAlignment = StringAlignment.Center;  // 垂直居中
                format.Trimming = StringTrimming.EllipsisCharacter;

                g.DrawString(text, font, brush, textRect, format);
            }
            finally
            {
                brush.Dispose();
                format.Dispose();
                font.Dispose();
            }
        }

        #endregion

        #region 状态判断方法

        /// <summary>
        /// 获取包装状态
        /// </summary>
        private QuantityStatus GetPackageStatus()
        {
            if (_packageRequired == 0) return QuantityStatus.NotStarted;
            if (_packageCollected > _packageRequired) return QuantityStatus.Exceeded;
            if (_packageCollected == _packageRequired) return QuantityStatus.Completed;
            if (_packageCollected > 0) return QuantityStatus.InProgress;
            return QuantityStatus.NotStarted;
        }

        /// <summary>
        /// 获取拆零状态
        /// </summary>
        private QuantityStatus GetSplitStatus()
        {
            if (_splitRequired == 0) return QuantityStatus.NotStarted;
            if (_splitCollected > _splitRequired) return QuantityStatus.Exceeded;
            if (_splitCollected == _splitRequired) return QuantityStatus.Completed;
            if (_splitCollected > 0) return QuantityStatus.InProgress;
            return QuantityStatus.NotStarted;
        }

        /// <summary>
        /// 获取整体状态
        /// </summary>
        private QuantityStatus GetOverallStatus()
        {
            QuantityStatus packageStatus = GetPackageStatus();
            QuantityStatus splitStatus = GetSplitStatus();

            // 任一超量则整体超量
            if (packageStatus == QuantityStatus.Exceeded || splitStatus == QuantityStatus.Exceeded)
                return QuantityStatus.Exceeded;

            // 全部完成则整体完成
            if (packageStatus == QuantityStatus.Completed && splitStatus == QuantityStatus.Completed)
                return QuantityStatus.Completed;

            // 任一进行中则整体进行中
            if (packageStatus == QuantityStatus.InProgress || splitStatus == QuantityStatus.InProgress)
                return QuantityStatus.InProgress;

            return QuantityStatus.NotStarted;
        }

        /// <summary>
        /// 根据状态获取颜色
        /// </summary>
        private Color GetStatusColor(QuantityStatus status)
        {
            switch (status)
            {
                case QuantityStatus.Completed:
                    return _completedColor;
                case QuantityStatus.Exceeded:
                    return _exceededColor;
                case QuantityStatus.InProgress:
                    return _collectedColor;
                default:
                    return _collectedColor;
            }
        }

        #endregion

        #region 序列化支持

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //base.GetObjectData(info, context);
            info.AddValue("PackageRequired", _packageRequired);
            info.AddValue("PackageCollected", _packageCollected);
            info.AddValue("SplitRequired", _splitRequired);
            info.AddValue("SplitCollected", _splitCollected);
            info.AddValue("DisplayMode", (int)_displayMode);
            info.AddValue("PackageUnit", _packageUnit);
            info.AddValue("SplitUnit", _splitUnit);
            info.AddValue("ShowBorder", _showBorder);
            info.AddValue("BorderWidth", _borderWidth);
            info.AddValue("BorderColor", _borderColor.ToArgb());
            info.AddValue("BorderStyle", (int)_borderStyle);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 设置数量数据
        /// </summary>
        public void SetQuantityData(int packageRequired, int packageCollected, int splitRequired, int splitCollected)
        {
            PackageRequired = packageRequired;
            PackageCollected = packageCollected;
            SplitRequired = splitRequired;
            SplitCollected = splitCollected;

            // 自动判断显示模式
            if (packageRequired > 0 && splitRequired > 0)
                _displayMode = DisplayMode.Both;
            else if (packageRequired > 0)
                _displayMode = DisplayMode.PackageOnly;
            else if (splitRequired > 0)
                _displayMode = DisplayMode.SplitOnly;
        }

        /// <summary>
        /// 设置数量数据（包含单位）
        /// </summary>
        public void SetQuantityData(int packageRequired, int packageCollected, int splitRequired, int splitCollected, string packageUnit, string splitUnit)
        {
            SetQuantityData(packageRequired, packageCollected, splitRequired, splitCollected);
            PackageUnit = packageUnit;
            SplitUnit = splitUnit;
        }

        /// <summary>
        /// 获取进度百分比
        /// </summary>
        public double GetProgressPercentage()
        {
            int totalRequired = _packageRequired + _splitRequired;
            int totalCollected = _packageCollected + _splitCollected;

            if (totalRequired == 0) return 0;
            return Math.Min(100, (double)totalCollected / totalRequired * 100);
        }

        #endregion

        #region BaseCellType 抽象方法实现

        /// <summary>
        /// 格式化数据为字符串
        /// </summary>
        public override string Format(object obj)
        {
            if (obj == null) return string.Empty;
            return obj.ToString();
        }

        /// <summary>
        /// 解析字符串为对象
        /// </summary>
        public override object Parse(string s)
        {
            return s;
        }

        /// <summary>
        /// 获取编辑器控件
        /// </summary>
        public override System.Windows.Forms.Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            // 数量显示单元格不支持编辑，返回null
            return null;
        }

        /// <summary>
        /// 获取编辑器值
        /// </summary>
        public override object GetEditorValue()
        {
            // 不支持编辑，返回null
            return null;
        }

        /// <summary>
        /// 设置编辑器值
        /// </summary>
        public override void SetEditorValue(object value)
        {
            // 不支持编辑，空实现
        }

        /// <summary>
        /// 获取首选大小（渲染器版本）
        /// </summary>
        public override Size GetPreferredSize(Graphics g, Size size, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            if (g == null) return new Size(120, 40);

            // 计算文本所需的尺寸
            Font font = new Font(_font.FontFamily, _font.Size * zoomFactor, _font.Style);
            try
            {
                string sampleText = "应采：99" + _packageUnit + "+99" + _splitUnit;
                SizeF textSize = g.MeasureString(sampleText, font);

                // 双行显示，高度需要乘以2，加上边距
                int width = Math.Max(120, (int)textSize.Width + 8);
                int height = Math.Max(40, (int)textSize.Height * 2 + 8);

                return new Size(width, height);
            }
            finally
            {
                font.Dispose();
            }
        }

        /// <summary>
        /// 重写IsValid方法
        /// </summary>
        public override bool IsValid(object value)
        {
            return true; // 数量显示单元格总是有效的
        }

        /// <summary>
        /// 重写Clone方法
        /// </summary>
        public override object Clone()
        {
            QuantityDisplayCellType clone = new QuantityDisplayCellType();
            clone._packageRequired = this._packageRequired;
            clone._packageCollected = this._packageCollected;
            clone._splitRequired = this._splitRequired;
            clone._splitCollected = this._splitCollected;
            clone._displayMode = this._displayMode;
            clone._font = new Font(this._font, this._font.Style);
            clone._requiredColor = this._requiredColor;
            clone._collectedColor = this._collectedColor;
            clone._completedColor = this._completedColor;
            clone._exceededColor = this._exceededColor;
            clone._showBorder = this._showBorder;
            clone._borderWidth = this._borderWidth;
            clone._borderColor = this._borderColor;
            clone._borderStyle = this._borderStyle;
            return clone;
        }

        /// <summary>
        /// 重写ToString方法
        /// </summary>
        public override string ToString()
        {
            return "QuantityDisplayCellType";
        }

        #endregion
    }
}
