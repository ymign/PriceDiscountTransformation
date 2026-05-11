using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using FarPoint.Win.Spread.CellType;

namespace Neusoft.HISFC.Components.Common.Controls.ModernStyles
{
    /// <summary>
    /// 图标类型枚举
    /// </summary>
    public enum IconType
    {
        /// <summary>
        /// 圆形图标
        /// </summary>
        Circle,

        /// <summary>
        /// 三角形图标
        /// </summary>
        Triangle,

        /// <summary>
        /// 方形图标
        /// </summary>
        Rectangle,

        /// <summary>
        /// 菱形图标
        /// </summary>
        Diamond
    }

    /// <summary>
    /// 状态图片单元格类型
    /// 可以显示自定义颜色和样式的图标和文字
    /// </summary>
    [Serializable]
    [Description("状态图片单元格")]
    public class StatusImageCellType : BaseCellType
    {
        private string _statusText = "未使用";
        private bool _showIcon = true;
        private int _iconSize = 16;
        private string _fontName = "微软雅黑";
        private float _fontSize = 9F;
        private FontStyle _fontStyle = FontStyle.Regular;

        // 自定义颜色属性
        private Color _iconColor = Color.FromArgb(156, 163, 175); // 默认灰色
        private Color _textColor = Color.FromArgb(107, 114, 128); // 默认深灰色
        private Color _backgroundColor = Color.Transparent; // 默认透明背景
        private Color _borderColor = Color.Transparent; // 默认无边框

        // 图标和样式属性
        private IconType _iconType = IconType.Circle;
        private string _iconSymbol = "-"; // 图标内的符号，如"✓"、"×"、"!"、"-"
        private bool _showBackground = false;
        private int _borderWidth = 0;
        private int _borderRadius = 12; // 圆角半径，默认12px
        private int _paddingHorizontal = 8; // 水平内边距
        private int _paddingVertical = 4; // 垂直内边距

        public StatusImageCellType()
        {
        }

        public StatusImageCellType(StatusImageCellType original)
        {
            this._statusText = original._statusText;
            this._showIcon = original._showIcon;
            this._iconSize = original._iconSize;
            this._fontName = original._fontName;
            this._fontSize = original._fontSize;
            this._fontStyle = original._fontStyle;
            this._iconColor = original._iconColor;
            this._textColor = original._textColor;
            this._backgroundColor = original._backgroundColor;
            this._borderColor = original._borderColor;
            this._iconType = original._iconType;
            this._iconSymbol = original._iconSymbol;
            this._showBackground = original._showBackground;
            this._borderWidth = original._borderWidth;
            this._borderRadius = original._borderRadius;
            this._paddingHorizontal = original._paddingHorizontal;
            this._paddingVertical = original._paddingVertical;
        }

        protected StatusImageCellType(SerializationInfo info, StreamingContext context)
        {
            _statusText = info.GetString("StatusText");
            _showIcon = info.GetBoolean("ShowIcon");
            _iconSize = info.GetInt32("IconSize");
            _fontName = info.GetString("FontName");
            _fontSize = (float)info.GetValue("FontSize", typeof(float));
            _fontStyle = (FontStyle)info.GetValue("FontStyle", typeof(FontStyle));
            _iconColor = (Color)info.GetValue("IconColor", typeof(Color));
            _textColor = (Color)info.GetValue("TextColor", typeof(Color));
            _backgroundColor = (Color)info.GetValue("BackgroundColor", typeof(Color));
            _borderColor = (Color)info.GetValue("BorderColor", typeof(Color));
            _iconType = (IconType)info.GetValue("IconType", typeof(IconType));
            _iconSymbol = info.GetString("IconSymbol");
            _showBackground = info.GetBoolean("ShowBackground");
            _borderWidth = info.GetInt32("BorderWidth");
            _borderRadius = info.GetInt32("BorderRadius");
            _paddingHorizontal = info.GetInt32("PaddingHorizontal");
            _paddingVertical = info.GetInt32("PaddingVertical");
        }

        /// <summary>
        /// 状态文字
        /// </summary>
        [DefaultValue("未使用")]
        public string StatusText
        {
            get { return _statusText; }
            set { _statusText = value != null ? value : "未使用"; }
        }

        /// <summary>
        /// 图标颜色
        /// </summary>
        [DefaultValue(typeof(Color), "156, 163, 175")]
        public Color IconColor
        {
            get { return _iconColor; }
            set { _iconColor = value; }
        }

        /// <summary>
        /// 文字颜色
        /// </summary>
        [DefaultValue(typeof(Color), "107, 114, 128")]
        public Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        /// <summary>
        /// 背景颜色
        /// </summary>
        [DefaultValue(typeof(Color), "Transparent")]
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        /// <summary>
        /// 边框颜色
        /// </summary>
        [DefaultValue(typeof(Color), "Transparent")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        /// <summary>
        /// 图标类型
        /// </summary>
        [DefaultValue(IconType.Circle)]
        public IconType IconType
        {
            get { return _iconType; }
            set { _iconType = value; }
        }

        /// <summary>
        /// 图标内符号
        /// </summary>
        [DefaultValue("-")]
        public string IconSymbol
        {
            get { return _iconSymbol; }
            set { _iconSymbol = value ?? "-"; }
        }

        /// <summary>
        /// 是否显示背景
        /// </summary>
        [DefaultValue(false)]
        public bool ShowBackground
        {
            get { return _showBackground; }
            set { _showBackground = value; }
        }

        /// <summary>
        /// 边框宽度
        /// </summary>
        [DefaultValue(0)]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = Math.Max(0, value); }
        }

        /// <summary>
        /// 圆角半径（像Web中border-radius）
        /// </summary>
        [DefaultValue(12)]
        public int BorderRadius
        {
            get { return _borderRadius; }
            set { _borderRadius = Math.Max(0, value); }
        }

        /// <summary>
        /// 水平内边距
        /// </summary>
        [DefaultValue(8)]
        public int PaddingHorizontal
        {
            get { return _paddingHorizontal; }
            set { _paddingHorizontal = Math.Max(0, value); }
        }

        /// <summary>
        /// 垂直内边距
        /// </summary>
        [DefaultValue(4)]
        public int PaddingVertical
        {
            get { return _paddingVertical; }
            set { _paddingVertical = Math.Max(0, value); }
        }

        /// <summary>
        /// 是否显示图标
        /// </summary>
        [DefaultValue(true)]
        public bool ShowIcon
        {
            get { return _showIcon; }
            set { _showIcon = value; }
        }

        /// <summary>
        /// 图标大小
        /// </summary>
        [DefaultValue(16)]
        public int IconSize
        {
            get { return _iconSize; }
            set { _iconSize = Math.Max(8, Math.Min(32, value)); }
        }

        /// <summary>
        /// 字体名称
        /// </summary>
        [DefaultValue("微软雅黑")]
        public string FontName
        {
            get { return _fontName; }
            set { _fontName = value != null ? value : "微软雅黑"; }
        }

        /// <summary>
        /// 字体大小
        /// </summary>
        [DefaultValue(9F)]
        public float FontSize
        {
            get { return _fontSize; }
            set { _fontSize = Math.Max(6F, value); }
        }

        /// <summary>
        /// 字体样式
        /// </summary>
        [DefaultValue(FontStyle.Regular)]
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

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
        public override Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            // 状态单元格不需要编辑器
            return null;
        }

        /// <summary>
        /// 获取编辑器值
        /// </summary>
        public override object GetEditorValue()
        {
            return _statusText;
        }

        /// <summary>
        /// 设置编辑器值
        /// </summary>
        public override void SetEditorValue(object value)
        {
            if (value != null)
            {
                _statusText = value.ToString();
            }
        }

        /// <summary>
        /// 获取首选大小
        /// </summary>
        public override Size GetPreferredSize(Graphics g, Size size, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            if (g == null) return new Size(80, 20);

            string text = _statusText ?? (value != null ? value.ToString() : "");
            if (string.IsNullOrEmpty(text)) return new Size(80, 20);

            Font font = new Font(_fontName, _fontSize, _fontStyle);
            SizeF textSize = g.MeasureString(text, font);

            int width = (int)textSize.Width + (_showIcon ? _iconSize + 6 : 0) + 8;
            int height = Math.Max((int)textSize.Height, _showIcon ? _iconSize : 0) + 6;

            return new Size(width, height);
        }

        /// <summary>
        /// 重写PaintCell方法以绘制状态图标和文字
        /// </summary>
        public override void PaintCell(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            if (g == null || r.Width <= 0 || r.Height <= 0)
                return;

            // 设置高质量渲染
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // 绘制单元格背景
            Color cellBgColor = appearance != null ? appearance.BackColor : Color.White;
            using (SolidBrush cellBgBrush = new SolidBrush(cellBgColor))
            {
                g.FillRectangle(cellBgBrush, r);
            }

            // 计算内容区域（Web风格的矩形背景）
            string displayText = _statusText ?? "";
            int iconWidth = _showIcon ? _iconSize : 0;
            int contentWidth = 0;
            int contentHeight = 0;

            // 计算内容尺寸
            if (!string.IsNullOrEmpty(displayText))
            {
                using (Font font = new Font(_fontName, _fontSize, _fontStyle))
                {
                    // 使用更精确的文字测量，防止省略号
                    SizeF textSize = g.MeasureString(displayText, font);
                    // 给文字宽度预留一些额外空间
                    int textWidth = (int)Math.Ceiling(textSize.Width) + 4;
                    contentWidth = textWidth + _paddingHorizontal * 2;
                    if (iconWidth > 0)
                    {
                        contentWidth += iconWidth + 6; // 图标和文字间距稍微增加
                    }
                    contentHeight = Math.Max((int)Math.Ceiling(textSize.Height), iconWidth) + _paddingVertical * 2;
                }
            }
            else if (iconWidth > 0)
            {
                contentWidth = iconWidth + _paddingHorizontal * 2;
                contentHeight = iconWidth + _paddingVertical * 2;
            }

            // 计算背景矩形区域（居中显示）
            int bgX = r.X + (r.Width - contentWidth) / 2;
            int bgY = r.Y + (r.Height - contentHeight) / 2;
            Rectangle contentRect = new Rectangle(bgX, bgY, contentWidth, contentHeight);

            // 绘制圆角矩形背景（如果启用）
            if (_showBackground && _backgroundColor != Color.Transparent)
            {
                DrawRoundedRectangle(g, contentRect, _borderRadius, _backgroundColor, true, 1);
            }

            // 绘制圆角矩形边框（如果启用）
            if (_borderWidth > 0 && _borderColor != Color.Transparent)
            {
                DrawRoundedRectangle(g, contentRect, _borderRadius, _borderColor, false, _borderWidth);
            }

            // 计算图标和文字位置（在背景矩形内居中）
            int centerX = contentRect.X + contentRect.Width / 2;
            int centerY = contentRect.Y + contentRect.Height / 2;

            int iconX = 0;
            int textStartX = 0;

            if (iconWidth > 0 && !string.IsNullOrEmpty(displayText))
            {
                // 有图标和文字
                using (Font font = new Font(_fontName, _fontSize, _fontStyle))
                {
                    int textWidth = (int)Math.Ceiling(g.MeasureString(displayText, font).Width) + 4;
                    int totalContentWidth = iconWidth + 6 + textWidth;
                    iconX = centerX - totalContentWidth / 2;
                    textStartX = iconX + iconWidth + 6;
                }
            }
            else if (iconWidth > 0)
            {
                // 只有图标
                iconX = centerX - iconWidth / 2;
            }
            else if (!string.IsNullOrEmpty(displayText))
            {
                // 只有文字
                textStartX = contentRect.X + _paddingHorizontal;
            }

            // 绘制图标
            if (_showIcon && iconWidth > 0)
            {
                Rectangle iconRect = new Rectangle(
                    iconX,
                    centerY - _iconSize / 2,
                    _iconSize,
                    _iconSize
                );
                DrawCustomIcon(g, iconRect);
            }

            // 绘制文字
            if (!string.IsNullOrEmpty(displayText))
            {
                Rectangle textRect;
                if (iconWidth > 0)
                {
                    // 有图标时，文字在图标右侧，给文字预留足够空间
                    int availableWidth = contentRect.Right - textStartX - _paddingHorizontal;
                    textRect = new Rectangle(
                        textStartX,
                        contentRect.Y,
                        Math.Max(availableWidth, 50), // 确保最小宽度
                        contentRect.Height);
                }
                else
                {
                    // 无图标时，文字居中，给文字预留足够空间
                    textRect = new Rectangle(
                        contentRect.X + _paddingHorizontal,
                        contentRect.Y,
                        Math.Max(contentRect.Width - _paddingHorizontal * 2, 50),
                        contentRect.Height);
                }
                DrawCustomText(g, textRect, displayText);
            }
        }

        /// <summary>
        /// 绘制自定义图标
        /// </summary>
        private void DrawCustomIcon(Graphics g, Rectangle iconRect)
        {
            // 绘制图标形状
            using (SolidBrush brush = new SolidBrush(_iconColor))
            {
                switch (_iconType)
                {
                    case IconType.Circle:
                        g.FillEllipse(brush, iconRect);
                        break;
                    case IconType.Rectangle:
                        g.FillRectangle(brush, iconRect);
                        break;
                    case IconType.Triangle:
                        Point[] trianglePoints = {
                            new Point(iconRect.X + iconRect.Width / 2, iconRect.Y),
                            new Point(iconRect.X, iconRect.Bottom),
                            new Point(iconRect.Right, iconRect.Bottom)
                        };
                        g.FillPolygon(brush, trianglePoints);
                        break;
                    case IconType.Diamond:
                        Point[] diamondPoints = {
                            new Point(iconRect.X + iconRect.Width / 2, iconRect.Y),
                            new Point(iconRect.Right, iconRect.Y + iconRect.Height / 2),
                            new Point(iconRect.X + iconRect.Width / 2, iconRect.Bottom),
                            new Point(iconRect.X, iconRect.Y + iconRect.Height / 2)
                        };
                        g.FillPolygon(brush, diamondPoints);
                        break;
                }
            }

            // 绘制图标内的符号
            if (!string.IsNullOrEmpty(_iconSymbol))
            {
                using (Font symbolFont = new Font(_fontName, _fontSize * 1.2f, FontStyle.Bold))
                using (SolidBrush symbolBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(_iconSymbol, symbolFont, symbolBrush, iconRect, sf);
                }
            }
        }

        /// <summary>
        /// 绘制自定义文字
        /// </summary>
        private void DrawCustomText(Graphics g, Rectangle textRect, string text)
        {
            using (Font font = new Font(_fontName, _fontSize, _fontStyle))
            using (SolidBrush textBrush = new SolidBrush(_textColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = _showIcon ? StringAlignment.Near : StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.None, // 禁用省略号
                    FormatFlags = StringFormatFlags.NoWrap  // 禁止换行
                };

                g.DrawString(text, font, textBrush, textRect, sf);
            }
        }

        /// <summary>
        /// 绘制圆角矩形（模拟Web的border-radius）
        /// </summary>
        private void DrawRoundedRectangle(Graphics g, Rectangle rect, int radius, Color color, bool fill, int borderWidth)
        {
            if (borderWidth <= 0)
            {
                borderWidth = 1;
            }
            if (radius <= 0)
            {
                // 无圆角，直接绘制矩形
                if (fill)
                {
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }
                else
                {
                    using (Pen pen = new Pen(color, borderWidth))
                    {
                        g.DrawRectangle(pen, rect);
                    }
                }
                return;
            }

            // 确保圆角半径不超过矩形的一半
            int actualRadius = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));

            using (GraphicsPath path = new GraphicsPath())
            {
                // 构建圆角矩形路径
                path.AddArc(rect.X, rect.Y, actualRadius * 2, actualRadius * 2, 180, 90); // 左上角
                path.AddArc(rect.Right - actualRadius * 2, rect.Y, actualRadius * 2, actualRadius * 2, 270, 90); // 右上角
                path.AddArc(rect.Right - actualRadius * 2, rect.Bottom - actualRadius * 2, actualRadius * 2, actualRadius * 2, 0, 90); // 右下角
                path.AddArc(rect.X, rect.Bottom - actualRadius * 2, actualRadius * 2, actualRadius * 2, 90, 90); // 左下角
                path.CloseFigure();

                if (fill)
                {
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    using (Pen pen = new Pen(color, borderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        /// <summary>
        /// 重写编辑相关方法，防止状态单元格进入编辑模式
        /// </summary>
        public override void StartEditing(EventArgs e, bool selectAll, bool autoClipboard)
        {
            // 状态单元格不支持编辑
        }

        public override bool StopEditing()
        {
            // 状态单元格不支持编辑
            return true;
        }

        public override void CancelEditing()
        {
            // 状态单元格不支持编辑
        }

        /// <summary>
        /// 重写IsReservedLocation以处理鼠标交互
        /// </summary>
        public override object IsReservedLocation(Graphics g, int x, int y, Rectangle rc, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            // 状态单元格不需要特殊的鼠标处理
            return null;
        }
    }
}
