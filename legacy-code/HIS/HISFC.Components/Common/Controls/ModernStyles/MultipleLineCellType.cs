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
    /// 多行文本单元格类型 - 简洁表格风格
    /// 支持左侧2px状态指示条、紧凑文本布局和右侧滚动条（内容过多时显示）
    /// 兼容.NET Framework 3.5
    /// </summary>
    [Serializable]
    public class MultipleLineCellType : BaseCellType
    {
        #region 枚举定义

        /// <summary>
        /// 单元格状态
        /// </summary>
        public enum ItemStatus
        {
            /// <summary>正常状态</summary>
            Normal,
            /// <summary>选中状态</summary>
            Selected,
            /// <summary>警告状态</summary>
            Warning,
            /// <summary>错误状态</summary>
            Error
        }

        #endregion

        #region 私有字段

        private List<string> _textLines;                    // 文本行列表
        private ItemStatus _status = ItemStatus.Normal;    // 单元格状态
        private int _leftBarWidth = 2;                      // 左侧状态指示条宽度
        private int _rightAreaWidth = 20;                   // 右侧滚动条区域宽度
        private int _padding = 6;                           // 内边距
        private Font _textFont = new Font("微软雅黑", 9F); // 字体
        private bool _showScrollbar = true;                 // 是否显示滚动条
        private int _scrollOffset = 0;                      // 滚动偏移量

        // 状态颜色定义（状态指示条颜色）
        private Color _normalBarColor = Color.FromArgb(59, 130, 246);    // 蓝色指示条
        private Color _selectedBarColor = Color.FromArgb(16, 185, 129);  // 绿色指示条
        private Color _warningBarColor = Color.FromArgb(245, 158, 11);   // 橙色指示条
        private Color _errorBarColor = Color.FromArgb(239, 68, 68);      // 红色指示条

        private Color _textColor = Color.FromArgb(75, 85, 99);           // 文本颜色
        private Color _backgroundColor = Color.White;                     // 背景颜色

        // 鼠标交互相关字段
        private bool _isDraggingScrollbar = false;                       // 是否正在拖动滚动条
        private int _lastMouseY = 0;                                     // 上次鼠标Y位置

        #endregion

        #region 公共属性

        /// <summary>文本行列表</summary>
        public List<string> TextLines
        {
            get { return _textLines ?? new List<string>(); }
            set { _textLines = value; }
        }

        /// <summary>单元格状态</summary>
        public ItemStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// 滚动偏移量</summary>
        public int ScrollOffset
        {
            get { return _scrollOffset; }
            set
            {
                if (_textLines != null)
                {
                    int maxVisibleLines = GetMaxVisibleLines(100); // 使用默认高度
                    int maxOffset = Math.Max(0, _textLines.Count - maxVisibleLines);
                    _scrollOffset = Math.Max(0, Math.Min(value, maxOffset));
                }
                else
                {
                    _scrollOffset = 0;
                }
            }
        }

        /// <summary>获取最大可见行数</summary>
        private int GetMaxVisibleLines(int containerHeight)
        {
            int lineHeight = 28;
            return (containerHeight - 12) / lineHeight;
        }

        /// <summary>
        /// 滚动到上一行
        /// </summary>
        public void ScrollUp()
        {
            ScrollOffset = _scrollOffset - 1;
        }

        /// <summary>
        /// 滚动到下一行
        /// </summary>
        public void ScrollDown()
        {
            ScrollOffset = _scrollOffset + 1;
        }

        /// <summary>
        /// 设置滚动位置（百分比）
        /// </summary>
        public void SetScrollPosition(float percentage)
        {
            if (_textLines == null || _textLines.Count == 0) return;

            int maxVisibleLines = GetMaxVisibleLines(100);
            int maxOffset = Math.Max(0, _textLines.Count - maxVisibleLines);
            ScrollOffset = (int)(maxOffset * Math.Max(0, Math.Min(1, percentage)));
        }

        /// <summary>
        /// 检查是否需要滚动条
        /// </summary>
        public bool NeedsScrollbar(int containerHeight)
        {
            if (_textLines == null) return false;
            int maxVisibleLines = GetMaxVisibleLines(containerHeight);
            return _textLines.Count > maxVisibleLines;
        }

        /// <summary>左侧状态指示条宽度</summary>
        public int LeftBarWidth
        {
            get { return _leftBarWidth; }
            set { _leftBarWidth = Math.Max(1, value); }
        }

        /// <summary>右侧滚动条区域宽度</summary>
        public int RightAreaWidth
        {
            get { return _rightAreaWidth; }
            set { _rightAreaWidth = Math.Max(12, value); }
        }

        /// <summary>是否显示滚动条</summary>
        public bool ShowScrollbar
        {
            get { return _showScrollbar; }
            set { _showScrollbar = value; }
        }

        /// <summary>文本字体</summary>
        public Font TextFont
        {
            get { return _textFont; }
            set { _textFont = value ?? new Font("微软雅黑", 9F); }
        }

        #endregion

        #region 构造函数

        public MultipleLineCellType()
        {
            _textLines = new List<string>();
        }

        public MultipleLineCellType(SerializationInfo info, StreamingContext context)
        {
            _textLines = new List<string>();
            try
            {
                _status = (ItemStatus)info.GetInt32("Status");
                _leftBarWidth = info.GetInt32("LeftBarWidth");
                _rightAreaWidth = info.GetInt32("RightAreaWidth");
                _showScrollbar = info.GetBoolean("ShowScrollbar");
                _scrollOffset = info.GetInt32("ScrollOffset");

                // 反序列化文本行
                int lineCount = info.GetInt32("LineCount");
                for (int i = 0; i < lineCount; i++)
                {
                    string line = info.GetString("Line" + i);
                    if (!string.IsNullOrEmpty(line))
                        _textLines.Add(line);
                }
            }
            catch
            {
                // 如果反序列化失败，使用默认值
            }
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

            // 绘制背景
            DrawBackground(g, r, isSelected);

            // 绘制每行的背景和指示器
            DrawLineBackgroundsAndIndicators(g, r);

            // 绘制文本内容
            DrawTextContent(g, r);

            // 绘制右侧滚动条（仅在内容过多时显示）
            if (_showScrollbar)
            {
                DrawScrollbar(g, r);
            }
        }

        /// <summary>
        /// 绘制背景 - 简洁表格风格
        /// </summary>
        private void DrawBackground(Graphics g, Rectangle r, bool isSelected)
        {
            // 整体背景色
            Color bgColor = _backgroundColor;

            using (SolidBrush bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, r);
            }
        }

        /// <summary>
        /// 绘制每行的背景和状态指示器
        /// </summary>
        private void DrawLineBackgroundsAndIndicators(Graphics g, Rectangle r)
        {
            if (_textLines == null || _textLines.Count == 0) return;

            Color indicatorColor = GetBarColor();
            int lineHeight = 28;
            int maxVisibleLines = (r.Height - 12) / lineHeight;
            int textStartY = r.Y + 6;

            // 绘制每行的背景和状态指示器
            int startIndex = _scrollOffset;
            int endIndex = Math.Min(startIndex + maxVisibleLines, _textLines.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                int displayIndex = i - startIndex;
                int lineY = textStartY + displayIndex * lineHeight;

                // 绘制行背景
                Rectangle lineBackgroundRect = new Rectangle(
                    r.X + 6,
                    lineY + 2,
                    r.Width - 12 - (_showScrollbar ? _rightAreaWidth : 0),
                    lineHeight - 4
                );

                // 交替背景色
                Color lineBgColor = (i % 2 == 0) ? Color.FromArgb(248, 250, 252) : Color.FromArgb(243, 244, 246);
                using (SolidBrush bgBrush = new SolidBrush(lineBgColor))
                {
                    DrawRoundedRectangle(g, bgBrush, lineBackgroundRect, 4);
                }

                // 绘制状态指示器 - 圆角弧度效果
                int indicatorY = lineY + (lineHeight - 16) / 2;
                Rectangle indicatorRect = new Rectangle(
                    r.X + 12,
                    indicatorY,
                    4,
                    16
                );

                using (SolidBrush indicatorBrush = new SolidBrush(indicatorColor))
                {
                    // 圆角半径不能超过宽度的一半，否则会变成圆点
                    int maxRadius = Math.Min(indicatorRect.Width, indicatorRect.Height) / 2;
                    int radius = Math.Min(2, maxRadius); // 限制圆角半径为2px
                    DrawRoundedRectangle(g, indicatorBrush, indicatorRect, radius);
                }
            }
        }

        /// <summary>
        /// 绘制圆角矩形
        /// </summary>
        private void DrawRoundedRectangle(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }

        /// <summary>
        /// 绘制文本内容 - 每行文本单独显示，支持滚动
        /// </summary>
        private void DrawTextContent(Graphics g, Rectangle r)
        {
            if (_textLines == null || _textLines.Count == 0) return;

            int textStartX = r.X + 26; // 给背景和指示器留出空间
            int textWidth = r.Width - 24 - _padding;
            if (_showScrollbar)
            {
                textWidth -= _rightAreaWidth;
            }

            // 更高的行高，更美观
            int lineHeight = 28;
            int maxVisibleLines = (r.Height - 12) / lineHeight;
            int textStartY = r.Y + 6;

            using (SolidBrush textBrush = new SolidBrush(_textColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.NoWrap;

                // 显示可见的文本行（考虑滚动偏移）
                int startIndex = _scrollOffset;
                int endIndex = Math.Min(startIndex + maxVisibleLines, _textLines.Count);

                for (int i = startIndex; i < endIndex; i++)
                {
                    int displayIndex = i - startIndex;
                    Rectangle lineRect = new Rectangle(
                        textStartX,
                        textStartY + displayIndex * lineHeight,
                        textWidth,
                        lineHeight
                    );

                    g.DrawString(_textLines[i], _textFont, textBrush, lineRect, sf);
                }

                sf.Dispose();
            }
        }

        /// <summary>
        /// 绘制滚动条（仅在内容过多时显示）
        /// </summary>
        private void DrawScrollbar(Graphics g, Rectangle r)
        {
            // 计算是否需要滚动条
            int lineHeight = 28; // 与文本行高保持一致
            int maxVisibleLines = (r.Height - 12) / lineHeight;

            if (_textLines == null || _textLines.Count <= maxVisibleLines)
                return;

            int scrollbarWidth = 12;
            int scrollbarX = r.Right - _rightAreaWidth + 4;

            Rectangle scrollbarRect = new Rectangle(
                scrollbarX,
                r.Top + 6,
                scrollbarWidth,
                r.Height - 12
            );

            // 绘制滚动条背景（圆角）
            using (Brush bgBrush = new SolidBrush(Color.FromArgb(240, 240, 240)))
            {
                DrawRoundedRectangle(g, bgBrush, scrollbarRect, 6);
            }

            // 计算滑块大小和位置
            float totalLines = _textLines.Count;
            float visibleRatio = (float)maxVisibleLines / totalLines;
            int thumbHeight = Math.Max(20, (int)(scrollbarRect.Height * visibleRatio));

            // 计算滑块位置基于滚动偏移
            float scrollRatio = (float)_scrollOffset / Math.Max(1, _textLines.Count - maxVisibleLines);
            int thumbY = scrollbarRect.Top + (int)((scrollbarRect.Height - thumbHeight) * scrollRatio);

            Rectangle thumbRect = new Rectangle(
                scrollbarRect.Left + 2,
                thumbY,
                scrollbarRect.Width - 4,
                thumbHeight
            );

            // 绘制滑块（圆角）
            using (Brush thumbBrush = new SolidBrush(Color.FromArgb(160, 160, 160)))
            {
                DrawRoundedRectangle(g, thumbBrush, thumbRect, 4);
            }
        }
        /// <summary>
        /// 解析值为文本行
        /// </summary>
        private void ParseValue(object value)
        {
            if (value == null) return;

            string valueStr = value.ToString();
            if (string.IsNullOrEmpty(valueStr)) return;

            // 按换行符分割文本
            string[] lines = valueStr.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            _textLines = new List<string>(lines);
        }

        /// <summary>
        /// 获取状态指示条颜色
        /// </summary>
        private Color GetBarColor()
        {
            switch (_status)
            {
                case ItemStatus.Selected:
                    return _selectedBarColor;
                case ItemStatus.Warning:
                    return _warningBarColor;
                case ItemStatus.Error:
                    return _errorBarColor;
                default:
                    return _normalBarColor;
            }
        }

        /// <summary>
        /// 设置文本行
        /// </summary>
        public void SetTextLines(params string[] lines)
        {
            _textLines = new List<string>();
            if (lines != null)
            {
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                        _textLines.Add(line);
                }
            }
        }

        /// <summary>
        /// 添加文本行
        /// </summary>
        public void AddTextLine(string line)
        {
            if (_textLines == null)
                _textLines = new List<string>();

            if (!string.IsNullOrEmpty(line))
                _textLines.Add(line);
        }

        /// <summary>
        /// 清空文本行
        /// </summary>
        public void ClearTextLines()
        {
            if (_textLines != null)
                _textLines.Clear();
        }

        #endregion

        #region BaseCellType 实现

        public override string Format(object obj)
        {
            if (obj == null) return string.Empty;
            return obj.ToString();
        }

        public override object Parse(string s)
        {
            return s ?? string.Empty;
        }

        public override Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            // 创建一个只读的多行文本框来显示内容
            TextBox textBox = new TextBox();
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.ReadOnly = true;
            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = _backgroundColor;
            textBox.Font = _textFont;
            textBox.ForeColor = _textColor;

            // 设置文本内容
            if (_textLines != null && _textLines.Count > 0)
            {
                textBox.Text = string.Join("\r\n", _textLines.ToArray());
            }

            return textBox;
        }

        public override object GetEditorValue()
        {
            if (_textLines != null && _textLines.Count > 0)
                return string.Join("\n", _textLines.ToArray());
            return string.Empty;
        }

        public override void SetEditorValue(object value)
        {
            ParseValue(value);
            // 注意：这里不需要更新编辑器控件，因为GetEditorControl会重新创建
        }

        public override Size GetPreferredSize(Graphics g, Size size, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            if (g == null) return new Size(200, 60);

            ParseValue(value);

            // 更高的行高，更美观
            int lineHeight = 28;
            int totalHeight = Math.Max(60, _textLines.Count * lineHeight + 12); // 给每行留出足够空间
            int totalWidth = Math.Max(280, 24 + 200 + (_showScrollbar ? _rightAreaWidth : 0)); // 给指示器和文本留出空间

            return new Size(totalWidth, totalHeight);
        }

        public override bool IsValid(object value)
        {
            return true;
        }

        public override object Clone()
        {
            MultipleLineCellType clone = new MultipleLineCellType();
            clone._status = this._status;
            clone._leftBarWidth = this._leftBarWidth;
            clone._rightAreaWidth = this._rightAreaWidth;
            clone._showScrollbar = this._showScrollbar;
            clone._scrollOffset = this._scrollOffset;
            clone._isDraggingScrollbar = false; // 不复制交互状态
            clone._textFont = new Font(this._textFont, this._textFont.Style);

            if (this._textLines != null)
            {
                clone._textLines = new List<string>(this._textLines);
            }

            return clone;
        }

        public override object IsReservedLocation(Graphics g, int x, int y, Rectangle rc, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            // 检查是否点击在滚动条区域
            if (_showScrollbar && NeedsScrollbar(rc.Height))
            {
                Rectangle scrollbarRect = GetScrollbarRect(rc);
                if (scrollbarRect.Contains(x, y))
                {
                    // 处理滚动条点击
                    HandleScrollbarClick(x, y, scrollbarRect, rc);
                    return "scrollbar"; // 返回非null表示这是保留区域
                }
            }
            return null;
        }

        #endregion

        #region 滚动支持方法

        /// <summary>
        /// 获取滚动条区域
        /// </summary>
        private Rectangle GetScrollbarRect(Rectangle cellRect)
        {
            int scrollbarWidth = 12;
            int scrollbarX = cellRect.Right - _rightAreaWidth + 4;

            return new Rectangle(
                scrollbarX,
                cellRect.Top + 6,
                scrollbarWidth,
                cellRect.Height - 12
            );
        }

        /// <summary>
        /// 处理滚动条点击
        /// </summary>
        private void HandleScrollbarClick(int x, int y, Rectangle scrollbarRect, Rectangle cellRect)
        {
            if (_textLines == null || _textLines.Count == 0) return;

            int lineHeight = 28;
            int maxVisibleLines = (cellRect.Height - 12) / lineHeight;

            if (_textLines.Count <= maxVisibleLines) return;

            // 计算点击位置相对于滚动条的百分比
            float clickRatio = (float)(y - scrollbarRect.Top) / scrollbarRect.Height;
            clickRatio = Math.Max(0, Math.Min(1, clickRatio));

            // 设置滚动位置
            SetScrollPosition(clickRatio);
        }

        /// <summary>
        /// 判断是否可以进入编辑模式
        /// </summary>
        public override bool CanOverflow()
        {
            return false;
        }

        #endregion

        #region 序列化支持

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //base.GetObjectData(info, context);
            info.AddValue("Status", (int)_status);
            info.AddValue("LeftBarWidth", _leftBarWidth);
            info.AddValue("RightAreaWidth", _rightAreaWidth);
            info.AddValue("ShowScrollbar", _showScrollbar);
            info.AddValue("ScrollOffset", _scrollOffset);

            // 序列化文本行
            if (_textLines != null)
            {
                info.AddValue("LineCount", _textLines.Count);
                for (int i = 0; i < _textLines.Count; i++)
                {
                    info.AddValue("Line" + i, _textLines[i]);
                }
            }
            else
            {
                info.AddValue("LineCount", 0);
            }
        }

        #endregion
    }
}
