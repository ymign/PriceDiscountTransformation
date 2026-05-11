using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

namespace Neusoft.HISFC.Components.Common.Controls.ModernStyles
{
    public class ModernProgressBar : Control
    {
        private int minimum = 0;
        private int maximum = 100;
        private int value = 0;

        [Category("Behavior")]
        public int Minimum
        {
            get { return minimum; }
            set
            {
                minimum = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Behavior")]
        public int Maximum
        {
            get { return maximum; }
            set
            {
                maximum = Math.Max(1, value);
                Invalidate();
            }
        }

        [Category("Behavior")]
        public int Value
        {
            get { return value; }
            set
            {
                if (value < minimum)
                    this.value = minimum;
                else if (value > maximum)
                    this.value = maximum;
                else
                    this.value = value;

                Invalidate();
            }
        }

        private int cornerRadius = 10;
        private Color barStartColor = Color.FromArgb(79, 157, 166);
        private Color barEndColor = Color.FromArgb(79, 157, 166);

        [Category("Appearance")]
        public int CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color BarStartColor
        {
            get { return barStartColor; }
            set
            {
                barStartColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color BarEndColor
        {
            get { return barEndColor; }
            set
            {
                barEndColor = value;
                Invalidate();
            }
        }


        private bool useGradient = true;  // 新增：是否使用渐变填充

        [Category("Appearance")]
        [Description("是否启用渐变颜色填充进度条")]
        [DefaultValue(true)]
        public bool UseGradient
        {
            get { return useGradient; }
            set
            {
                useGradient = value;
                Invalidate();
            }
        }
        public enum ProgressTextMode
        {
            Percent,
            ValueOverMax,
            CustomText,
            None
        }

        public enum TextAlignMode
        {
            Left,
            Center,
            Right
        }

        private ProgressTextMode displayMode = ProgressTextMode.Percent;
        private TextAlignMode textAlign = TextAlignMode.Center;
        private string customText = "进度: {value}/{max}";
        private Font textFont = SystemFonts.DefaultFont;
        private Color textColor = Color.Black;

        [Category("Appearance")]
        public ProgressTextMode DisplayMode
        {
            get { return displayMode; }
            set
            {
                displayMode = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public string CustomText
        {
            get { return customText; }
            set
            {
                customText = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public TextAlignMode TextAlignment
        {
            get { return textAlign; }
            set
            {
                textAlign = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Font TextFont
        {
            get { return textFont; }
            set
            {
                textFont = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                Invalidate();
            }
        }

        public ModernProgressBar()
        {
            DoubleBuffered = true;
            ForeColor = Color.White;
            BackColor = Color.LightGray;
            Size = new Size(200, 30);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // Top-left
            path.AddArc(arc, 180, 90);

            // Top-right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom-right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom-left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private string FormatCustomText(string template, float percent)
        {
            string result = template;

            result = result.Replace("{value}", value.ToString());
            result = result.Replace("{max}", maximum.ToString());

            result = Regex.Replace(result, @"\{percent(?::f(\d+))?\}", delegate(Match match)
            {
                int decimals = 0;
                if (match.Groups[1].Success)
                {
                    int.TryParse(match.Groups[1].Value, out decimals);
                }
                return (percent * 100).ToString("F" + decimals) + "%";
            });

            return result;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            float percent = (float)(value - minimum) / (maximum - minimum);
            int fillWidth = (int)((Width - 1) * percent);

            // 背景圆角
            using (GraphicsPath backgroundPath = RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), cornerRadius))
            using (Brush backgroundBrush = new SolidBrush(BackColor))
            {
                e.Graphics.FillPath(backgroundBrush, backgroundPath);
            }

            // 前景填充
            if (fillWidth > 0)
            {
                using (GraphicsPath fillPath = RoundedRect(new Rectangle(0, 0, fillWidth, Height - 1), cornerRadius))
                {
                    if (useGradient)
                    {
                        // 渐变填充
                        using (LinearGradientBrush gradient = new LinearGradientBrush(
                            new Point(0, 0), new Point(fillWidth, 0),
                            barStartColor, barEndColor))
                        {
                            e.Graphics.FillPath(gradient, fillPath);
                        }
                    }
                    else
                    {
                        // 单色填充
                        using (Brush solidBrush = new SolidBrush(barStartColor))
                        {
                            e.Graphics.FillPath(solidBrush, fillPath);
                        }
                    }
                }
            }

            // 边框
            using (Pen borderPen = new Pen(Color.Gray, 1))
            using (GraphicsPath borderPath = RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), cornerRadius))
            {
                e.Graphics.DrawPath(borderPen, borderPath);
            }

            // 文本显示逻辑不变
            string displayText = "";

            switch (displayMode)
            {
                case ProgressTextMode.Percent:
                    displayText = ((int)(percent * 100)).ToString() + "%";
                    break;
                case ProgressTextMode.ValueOverMax:
                    displayText = value.ToString() + " / " + maximum.ToString();
                    break;
                case ProgressTextMode.CustomText:
                    displayText = FormatCustomText(customText, percent);
                    break;
                case ProgressTextMode.None:
                    displayText = "";
                    break;
            }

            if (!string.IsNullOrEmpty(displayText))
            {
                RectangleF textRect = new RectangleF(4, 0, Width - 8, Height);
                StringFormat format = new StringFormat();
                switch (textAlign)
                {
                    case TextAlignMode.Left:
                        format.Alignment = StringAlignment.Near;
                        break;
                    case TextAlignMode.Right:
                        format.Alignment = StringAlignment.Far;
                        break;
                    default:
                        format.Alignment = StringAlignment.Center;
                        break;
                }
                format.LineAlignment = StringAlignment.Center;

                using (Brush textBrush = new SolidBrush(textColor))
                {
                    e.Graphics.DrawString(displayText, textFont, textBrush, textRect, format);
                }
            }
        }
    }
}
