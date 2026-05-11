using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FarPoint.Win.Spread.CellType;
using System.Drawing;
using System.Runtime.Serialization;
using FarPoint.Win.Spread;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Drawing.Text;

namespace Neusoft.HISFC.Components.Common.Controls.ModernStyles
{
    /// <summary>
    /// FarPoint专用的通用按钮单元格类型
    /// 实现Web风格的按钮样式，支持悬停效果和现代化视觉设计
    /// 可用于查看详情、申请退款、删除入库等各种操作按钮
    /// </summary>
    [Serializable]
    [Description("FarPoint专用的通用按钮单元格")]
    public class FarPointButtonCellType : ButtonCellType
    {
        private bool _isHovered = false;
        private Color _primaryColor = Color.FromArgb(42, 164, 164); // #2aa4a4
        private Color _hoverColor = Color.FromArgb(32, 144, 144);
        private Color _textColor = Color.White;
        private int _borderRadius = 6;
        private string _buttonText = "操作";
        private int _buttonWidth = 80;  // 按钮宽度
        private int _buttonHeight = 28; // 按钮高度
        private ContentAlignment _buttonAlignment = ContentAlignment.MiddleCenter; // 按钮对齐方式
        private string _fontName = "微软雅黑"; // 字体名称
        private float _fontSize = 9F; // 字体大小
        private FontStyle _fontStyle = FontStyle.Regular; // 字体样式

        public FarPointButtonCellType()
            : base()
        {
            this.Text = _buttonText;
            this.TextColor = _textColor;
        }

        public FarPointButtonCellType(FarPointButtonCellType original)
            : base(original)
        {
            this._primaryColor = original._primaryColor;
            this._hoverColor = original._hoverColor;
            this._textColor = original._textColor;
            this._borderRadius = original._borderRadius;
            this._buttonText = original._buttonText;
            this._buttonWidth = original._buttonWidth;
            this._buttonHeight = original._buttonHeight;
            this._buttonAlignment = original._buttonAlignment;
            this._fontName = original._fontName;
            this._fontSize = original._fontSize;
            this._fontStyle = original._fontStyle;
        }

        protected FarPointButtonCellType(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // 反序列化自定义属性
            _primaryColor = (Color)info.GetValue("PrimaryColor", typeof(Color));
            _hoverColor = (Color)info.GetValue("HoverColor", typeof(Color));
            _textColor = (Color)info.GetValue("TextColor", typeof(Color));
            _borderRadius = info.GetInt32("BorderRadius");
            _buttonText = info.GetString("ButtonText");
            _buttonWidth = info.GetInt32("ButtonWidth");
            _buttonHeight = info.GetInt32("ButtonHeight");
            _buttonAlignment = (ContentAlignment)info.GetValue("ButtonAlignment", typeof(ContentAlignment));
            _fontName = info.GetString("FontName");
            _fontSize = (float)info.GetValue("FontSize", typeof(float));
            _fontStyle = (FontStyle)info.GetValue("FontStyle", typeof(FontStyle));
        }

        /// <summary>
        /// 主色调
        /// </summary>
        [DefaultValue(typeof(Color), "42, 164, 164")]
        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set { _primaryColor = value; }
        }

        /// <summary>
        /// 悬停颜色
        /// </summary>
        [DefaultValue(typeof(Color), "32, 144, 144")]
        public Color HoverColor
        {
            get { return _hoverColor; }
            set { _hoverColor = value; }
        }

        /// <summary>
        /// 按钮文字颜色
        /// </summary>
        [DefaultValue(typeof(Color), "White")]
        public new Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        /// <summary>
        /// 圆角半径
        /// </summary>
        [DefaultValue(6)]
        public int BorderRadius
        {
            get { return _borderRadius; }
            set { _borderRadius = Math.Max(0, value); }
        }

        /// <summary>
        /// 按钮文字
        /// </summary>
        [DefaultValue("操作")]
        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                _buttonText = value != null ? value : "操作";
                this.Text = _buttonText;
            }
        }

        /// <summary>
        /// 按钮宽度（像素）
        /// </summary>
        [DefaultValue(80)]
        public int ButtonWidth
        {
            get { return _buttonWidth; }
            set { _buttonWidth = Math.Max(20, value); }
        }

        /// <summary>
        /// 按钮高度（像素）
        /// </summary>
        [DefaultValue(28)]
        public int ButtonHeight
        {
            get { return _buttonHeight; }
            set { _buttonHeight = Math.Max(16, value); }
        }

        /// <summary>
        /// 按钮在单元格中的对齐方式
        /// </summary>
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment ButtonAlignment
        {
            get { return _buttonAlignment; }
            set { _buttonAlignment = value; }
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
        /// 字体样式（粗体、斜体等）
        /// </summary>
        [DefaultValue(FontStyle.Regular)]
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        /// <summary>
        /// 重写PaintCell方法，实现现代化按钮样式
        /// </summary>
        public override void PaintCell(Graphics g, Rectangle r, Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            if (g == null || r.Width <= 0 || r.Height <= 0)
                return;

            // 设置高质量渲染
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // 首先绘制单元格背景，覆盖原始边框
            DrawCellBackground(g, r, appearance, isSelected);

            // 计算按钮区域（根据设置的大小和对齐方式）
            Rectangle buttonRect = CalculateButtonRect(r);

            // 确保按钮区域有效
            if (buttonRect.Width <= 0 || buttonRect.Height <= 0)
                return;

            // 绘制按钮背景
            DrawModernButton(g, buttonRect, isSelected, isLocked);

            // 绘制按钮文字
            DrawButtonText(g, buttonRect, isLocked);

            // 绘制图标（可选）
            //DrawButtonIcon(g, buttonRect);
        }

        /// <summary>
        /// 绘制现代化按钮背景
        /// </summary>
        private void DrawModernButton(Graphics g, Rectangle rect, bool isSelected, bool isLocked)
        {
            // 不使用_isHovered状态，只根据isSelected来判断
            Color bgColor = isLocked ? Color.FromArgb(200, 200, 200) :
                           (isSelected ? _hoverColor : _primaryColor);

            // 创建圆角路径
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, _borderRadius))
            {
                // 绘制渐变背景
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    bgColor,
                    Color.FromArgb(Math.Max(0, bgColor.R - 20), Math.Max(0, bgColor.G - 20), Math.Max(0, bgColor.B - 20)),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // 绘制边框
                Color borderColor = isLocked ? Color.FromArgb(150, 150, 150) :
                                   Color.FromArgb(Math.Max(0, bgColor.R - 30), Math.Max(0, bgColor.G - 30), Math.Max(0, bgColor.B - 30));

                using (Pen borderPen = new Pen(borderColor, 1))
                {
                    g.DrawPath(borderPen, path);
                }

                // 添加高光效果
                if (!isLocked)
                {
                    Rectangle highlightRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 3);
                    using (GraphicsPath highlightPath = CreateRoundedRectanglePath(highlightRect, _borderRadius))
                    {
                        using (LinearGradientBrush highlightBrush = new LinearGradientBrush(
                            highlightRect,
                            Color.FromArgb(60, Color.White),
                            Color.FromArgb(10, Color.White),
                            LinearGradientMode.Vertical))
                        {
                            g.FillPath(highlightBrush, highlightPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 绘制按钮文字
        /// </summary>
        private void DrawButtonText(Graphics g, Rectangle rect, bool isLocked)
        {
            string text = string.IsNullOrEmpty(_buttonText) ? "操作" : _buttonText;
            Color textColor = isLocked ? Color.FromArgb(120, 120, 120) : _textColor;

            // 使用自定义字体属性
            using (Font font = new Font(_fontName, _fontSize, _fontStyle))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                // 为文字添加阴影效果
                if (!isLocked)
                {
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(50, Color.Black)))
                    {
                        Rectangle shadowRect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width, rect.Height);
                        g.DrawString(text, font, shadowBrush, shadowRect, sf);
                    }
                }

                g.DrawString(text, font, textBrush, rect, sf);
            }
        }


        /// <summary>
        /// 绘制单元格背景，保持与表格一致的背景色
        /// </summary>
        //private void DrawCellBackground(Graphics g, Rectangle r, Appearance appearance, bool isSelected)
        //{
        //    // 不强制绘制背景，让表格自身的背景色显示
        //    // 这样可以保持与其他单元格一致的背景色

        //    // 只绘制细边框（如果需要的话）
        //    using (Pen borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
        //    {
        //        g.DrawRectangle(borderPen, r.X, r.Y, r.Width - 1, r.Height - 1);
        //    }
        //}

        /// <summary>
        /// 绘制单元格背景，覆盖原始边框样式
        /// </summary>
        private void DrawCellBackground(Graphics g, Rectangle r, Appearance appearance, bool isSelected)
        {
            // 使用单元格的背景色或默认白色（Framework 3.5兼容写法）
            Color bgColor = (appearance != null && appearance.BackColor != Color.Empty) ? appearance.BackColor : Color.White;

            // 绘制背景，完全覆盖原始单元格
            using (SolidBrush bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, r);
            }

            // 绘制自定义边框（细线，浅色）
            //using (Pen borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
            //{
            //    g.DrawRectangle(borderPen, r.X, r.Y, r.Width - 1, r.Height - 1);
            //}
        }

        /// <summary>
        /// 计算按钮在单元格中的实际绘制区域
        /// </summary>
        private Rectangle CalculateButtonRect(Rectangle cellRect)
        {
            int buttonWidth = Math.Min(_buttonWidth, cellRect.Width - 4);
            int buttonHeight = Math.Min(_buttonHeight, cellRect.Height - 4);

            int x = cellRect.X;
            int y = cellRect.Y;

            // 根据对齐方式计算位置
            switch (_buttonAlignment)
            {
                case ContentAlignment.TopLeft:
                    x = cellRect.X + 2;
                    y = cellRect.Y + 2;
                    break;
                case ContentAlignment.TopCenter:
                    x = cellRect.X + (cellRect.Width - buttonWidth) / 2;
                    y = cellRect.Y + 2;
                    break;
                case ContentAlignment.TopRight:
                    x = cellRect.Right - buttonWidth - 2;
                    y = cellRect.Y + 2;
                    break;
                case ContentAlignment.MiddleLeft:
                    x = cellRect.X + 2;
                    y = cellRect.Y + (cellRect.Height - buttonHeight) / 2;
                    break;
                case ContentAlignment.MiddleCenter:
                    x = cellRect.X + (cellRect.Width - buttonWidth) / 2;
                    y = cellRect.Y + (cellRect.Height - buttonHeight) / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    x = cellRect.Right - buttonWidth - 2;
                    y = cellRect.Y + (cellRect.Height - buttonHeight) / 2;
                    break;
                case ContentAlignment.BottomLeft:
                    x = cellRect.X + 2;
                    y = cellRect.Bottom - buttonHeight - 2;
                    break;
                case ContentAlignment.BottomCenter:
                    x = cellRect.X + (cellRect.Width - buttonWidth) / 2;
                    y = cellRect.Bottom - buttonHeight - 2;
                    break;
                case ContentAlignment.BottomRight:
                    x = cellRect.Right - buttonWidth - 2;
                    y = cellRect.Bottom - buttonHeight - 2;
                    break;
            }

            return new Rectangle(x, y, buttonWidth, buttonHeight);
        }

        /// <summary>
        /// 绘制按钮图标
        /// </summary>
        private void DrawButtonIcon(Graphics g, Rectangle rect)
        {
            // 绘制一个简单的查看图标
            int iconSize = Math.Min(16, rect.Height - 8);
            int iconX = rect.X + 8;
            int iconY = rect.Y + (rect.Height - iconSize) / 2;

            using (Pen iconPen = new Pen(_textColor, 1.5f))
            {
                // 绘制眼睛图标
                Rectangle eyeRect = new Rectangle(iconX, iconY + iconSize / 3, iconSize, iconSize / 3);
                g.DrawEllipse(iconPen, eyeRect);

                // 绘制瞳孔
                int pupilSize = iconSize / 6;
                Rectangle pupilRect = new Rectangle(
                    iconX + iconSize / 2 - pupilSize / 2,
                    iconY + iconSize / 2 - pupilSize / 2,
                    pupilSize,
                    pupilSize
                );
                using (SolidBrush pupilBrush = new SolidBrush(_textColor))
                {
                    g.FillEllipse(pupilBrush, pupilRect);
                }
            }
        }

        /// <summary>
        /// 创建圆角矩形路径
        /// </summary>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            // 左上角
            path.AddArc(arc, 180, 90);

            // 右上角
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // 右下角
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // 左下角
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// 防止单元格进入编辑模式，保持自定义样式
        /// </summary>
        public override System.Windows.Forms.Control GetEditorControl(Appearance appearance, float zoomFactor)
        {
            return null; // 不返回编辑控件，点击时不会进入编辑状态
        }

        /// <summary>
        /// 防止单元格获得焦点，避免边框变化
        /// </summary>
        public override void StartEditing(EventArgs e, bool selectAll, bool autoClipboard)
        {
            // 不执行任何操作，防止进入编辑状态
        }

        /// <summary>
        /// 防止单元格进入编辑状态
        /// </summary>
        public override bool StopEditing()
        {
            return true; // 直接返回true，表示编辑已停止
        }

        /// <summary>
        /// 防止单元格取消编辑状态
        /// </summary>
        public override void CancelEditing()
        {
            // 不执行任何操作
        }

        /// <summary>
        /// 重写获取预留光标，防止显示文本光标
        /// </summary>
        public override System.Windows.Forms.Cursor GetReservedCursor(object o)
        {
            return System.Windows.Forms.Cursors.Hand; // 始终显示手型光标
        }

        /// <summary>
        /// 重写键盘事件处理，防止键盘操作影响单元格
        /// </summary>
        public override bool IsReservedKey(System.Windows.Forms.KeyEventArgs e)
        {
            return true; // 所有键盘事件都被保留，不传递给单元格
        }

        /// <summary>
        /// 处理鼠标位置检测
        /// </summary>
        public override object IsReservedLocation(Graphics g, int x, int y, Rectangle rc, Appearance appearance, object value, float zoomFactor)
        {
            // 不设置悬停状态，避免颜色混乱
            return this; // 返回this表示整个区域都是按钮区域，防止单元格获得焦点
        }

        /// <summary>
        /// 重写格式化方法，防止值变化
        /// </summary>
        public override string Format(object o)
        {
            return _buttonText; // 始终返回按钮文字
        }

        /// <summary>
        /// 重写解析方法，防止值变化
        /// </summary>
        public override object Parse(string s)
        {
            return _buttonText; // 始终返回按钮文字
        }

        /// <summary>
        /// 重写获取编辑器值，防止编辑
        /// </summary>
        public override object GetEditorValue()
        {
            return _buttonText;
        }

        /// <summary>
        /// 重写设置编辑器值，防止编辑
        /// </summary>
        public override void SetEditorValue(object value)
        {
            // 不执行任何操作
        }

        /// <summary>
        /// 序列化支持
        /// </summary>
        public  void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PrimaryColor", _primaryColor);
            info.AddValue("HoverColor", _hoverColor);
            info.AddValue("TextColor", _textColor);
            info.AddValue("BorderRadius", _borderRadius);
            info.AddValue("ButtonText", _buttonText);
            info.AddValue("ButtonWidth", _buttonWidth);
            info.AddValue("ButtonHeight", _buttonHeight);
            info.AddValue("ButtonAlignment", _buttonAlignment);
            info.AddValue("FontName", _fontName);
            info.AddValue("FontSize", _fontSize);
            info.AddValue("FontStyle", _fontStyle);
        }

        /// <summary>
        /// XML序列化支持
        /// </summary>
        public override bool Serialize(XmlTextWriter w)
        {
            bool result = base.Serialize(w);

            w.WriteStartElement("PrimaryColor");
            w.WriteString(ColorTranslator.ToHtml(_primaryColor));
            w.WriteEndElement();

            w.WriteStartElement("HoverColor");
            w.WriteString(ColorTranslator.ToHtml(_hoverColor));
            w.WriteEndElement();

            w.WriteStartElement("TextColor");
            w.WriteString(ColorTranslator.ToHtml(_textColor));
            w.WriteEndElement();

            w.WriteStartElement("BorderRadius");
            w.WriteString(_borderRadius.ToString());
            w.WriteEndElement();

            w.WriteStartElement("ButtonText");
            w.WriteString(_buttonText);
            w.WriteEndElement();

            w.WriteStartElement("ButtonWidth");
            w.WriteString(_buttonWidth.ToString());
            w.WriteEndElement();

            w.WriteStartElement("ButtonHeight");
            w.WriteString(_buttonHeight.ToString());
            w.WriteEndElement();

            w.WriteStartElement("ButtonAlignment");
            w.WriteString(_buttonAlignment.ToString());
            w.WriteEndElement();

            w.WriteStartElement("FontName");
            w.WriteString(_fontName);
            w.WriteEndElement();

            w.WriteStartElement("FontSize");
            w.WriteString(_fontSize.ToString());
            w.WriteEndElement();

            w.WriteStartElement("FontStyle");
            w.WriteString(_fontStyle.ToString());
            w.WriteEndElement();

            return result;
        }

        /// <summary>
        /// XML反序列化支持
        /// </summary>
        public override bool Deserialize(XmlNodeReader r)
        {
            bool result = base.Deserialize(r);

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.LocalName)
                    {
                        case "PrimaryColor":
                            _primaryColor = ColorTranslator.FromHtml(r.ReadString());
                            break;
                        case "HoverColor":
                            _hoverColor = ColorTranslator.FromHtml(r.ReadString());
                            break;
                        case "TextColor":
                            _textColor = ColorTranslator.FromHtml(r.ReadString());
                            break;
                        case "BorderRadius":
                            _borderRadius = int.Parse(r.ReadString());
                            break;
                        case "ButtonText":
                            _buttonText = r.ReadString();
                            break;
                        case "ButtonWidth":
                            _buttonWidth = int.Parse(r.ReadString());
                            break;
                        case "ButtonHeight":
                            _buttonHeight = int.Parse(r.ReadString());
                            break;
                        case "ButtonAlignment":
                            _buttonAlignment = (ContentAlignment)Enum.Parse(typeof(ContentAlignment), r.ReadString());
                            break;
                        case "FontName":
                            _fontName = r.ReadString();
                            break;
                        case "FontSize":
                            _fontSize = float.Parse(r.ReadString());
                            break;
                        case "FontStyle":
                            _fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), r.ReadString());
                            break;
                    }
                }
            }

            return result;
        }
    }
}
