using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;
using FarPoint.Win.Spread.CellType;
using FarPoint.Win.Spread;


namespace Neusoft.HISFC.Components.Common.Controls.ModernStyles
{
    /// <summary>
    /// 现代化进度条单元格类型
    /// 具有立体感和现代化配色
    /// 兼容.NET Framework 3.5
    /// </summary>
    [Serializable]
    [Description("现代化进度条单元格类型，具有立体感和现代化配色")]
    public class ModernProgressCellType : ProgressCellType
    {
        // 现代化配色方案
        private Color _primaryColor = Color.FromArgb(79, 157, 166);      // 主色调 - 蓝色
        private Color _secondaryColor = Color.FromArgb(46, 204, 113);

        // 温馨的医疗系统进度条颜色
        private Color _warningColor = Color.FromArgb(129, 199, 132);    // 温暖的橙色，用于警告状态
        private Color _dangerColor = Color.FromArgb(255, 138, 101);    // 柔和的珊瑚红，用于危险状态  
        private Color _successColor = Color.FromArgb(79, 157, 166);   // 温和的薄荷绿，用于成功状态

        //private Color _warningColor = Color.FromArgb(33, 150, 243);    
        //private Color _dangerColor = Color.FromArgb(231, 76, 60);      
        //private Color _successColor = Color.FromArgb(79, 157, 166);   


        private Color _infoColor = Color.FromArgb(52, 152, 219);

        private Color _backgroundColor = Color.FromArgb(248, 249, 250);  // 背景色
        private Color _borderColor = Color.FromArgb(233, 236, 239);      // 边框色
        private Color _textColor = Color.FromArgb(33, 37, 41);           // 文字色

        private bool _showPercentage = true;                             // 是否显示百分比
        private bool _showValue = true;                                  // 是否显示数值
        private bool _enableGradient = true;                             // 是否启用渐变
        private bool _enableShadow = true;                               // 是否启用阴影
        private bool _enableRoundedCorners = true;                       // 是否启用圆角
        private float _cornerRadius = 6f;                                // 圆角半径
        private float _shadowOffset = 16f;                                // 阴影偏移
        private float _borderWidth = 0f;                                 // 边框宽度

        /// <summary>
        /// 主色调
        /// </summary>
        [Category("现代化样式")]
        [Description("进度条主色调")]
        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set { _primaryColor = value; }
        }

        /// <summary>
        /// 次要色
        /// </summary>
        [Category("现代化样式")]
        [Description("进度条次要色")]
        public Color SecondaryColor
        {
            get { return _secondaryColor; }
            set { _secondaryColor = value; }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        [Category("现代化样式")]
        [Description("进度条背景色")]
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        /// <summary>
        /// 边框色
        /// </summary>
        [Category("现代化样式")]
        [Description("进度条边框色")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        /// <summary>
        /// 文字色
        /// </summary>
        [Category("现代化样式")]
        [Description("进度条文字色")]
        public Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        /// <summary>
        /// 是否显示百分比
        /// </summary>
        [Category("现代化样式")]
        [Description("是否显示百分比")]
        public bool ShowPercentage
        {
            get { return _showPercentage; }
            set { _showPercentage = value; }
        }

        /// <summary>
        /// 是否显示数值
        /// </summary>
        [Category("现代化样式")]
        [Description("是否显示数值")]
        public bool ShowValue
        {
            get { return _showValue; }
            set { _showValue = value; }
        }

        /// <summary>
        /// 是否启用渐变
        /// </summary>
        [Category("现代化样式")]
        [Description("是否启用渐变效果")]
        public bool EnableGradient
        {
            get { return _enableGradient; }
            set { _enableGradient = value; }
        }

        /// <summary>
        /// 是否启用阴影
        /// </summary>
        [Category("现代化样式")]
        [Description("是否启用阴影效果")]
        public bool EnableShadow
        {
            get { return _enableShadow; }
            set { _enableShadow = value; }
        }

        /// <summary>
        /// 是否启用圆角
        /// </summary>
        [Category("现代化样式")]
        [Description("是否启用圆角效果")]
        public bool EnableRoundedCorners
        {
            get { return _enableRoundedCorners; }
            set { _enableRoundedCorners = value; }
        }

        /// <summary>
        /// 圆角半径
        /// </summary>
        [Category("现代化样式")]
        [Description("圆角半径")]
        public float CornerRadius
        {
            get { return _cornerRadius; }
            set { _cornerRadius = value; }
        }

        /// <summary>
        /// 阴影偏移
        /// </summary>
        [Category("现代化样式")]
        [Description("阴影偏移量")]
        public float ShadowOffset
        {
            get { return _shadowOffset; }
            set { _shadowOffset = value; }
        }

        /// <summary>
        /// 边框宽度
        /// </summary>
        [Category("现代化样式")]
        [Description("边框宽度")]
        public float BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = value; }
        }

        /// <summary>
        /// 绘制单元格
        /// </summary>
        public override void PaintCell(Graphics g, Rectangle r, Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            // 设置高质量绘图
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 获取进度值
            double progress = GetProgressValue(value);

            // 根据进度值选择颜色
            Color progressColor = GetProgressColor(progress);

            // 绘制背景
            DrawBackground(g, r, appearance, isSelected);

            // 绘制进度条
            DrawProgressBar(g, r, progress, progressColor);

            // 绘制文字
            DrawText(g, r, progress, appearance);
        }

        /// <summary>
        /// 获取进度值
        /// </summary>
        private double GetProgressValue(object value)
        {
            if (value == null) return 0;

            double progress = 0;
            if (double.TryParse(value.ToString(), out progress))
            {
                // 确保进度值在0-100之间
                if (progress < 0) progress = 0;
                if (progress > 100) progress = 100;
                return progress;
            }
            return 0;
        }

        private Color GetProgressColor111(double progress)
        {
            if (progress < 30)
                return Color.FromArgb(240, 188, 212); // 柔和粉色
            else if (progress < 70)
                return Color.FromArgb(100, 181, 246); // 柔和蓝色
            else
                return Color.FromArgb(129, 199, 132); // 柔和绿色
        }

        /// <summary>
        /// 根据进度值获取颜色
        /// </summary>
        private Color GetProgressColor(double progress)
        {
            if (progress < 30)
                return _dangerColor;
            else if (progress < 70)
                return _warningColor;
            else
                return _successColor;
        }

        /// <summary>
        /// 绘制背景
        /// </summary>
        private void DrawBackground(Graphics g, Rectangle r, Appearance appearance, bool isSelected)
        {
            Color bgColor;
            if (isSelected)
            {
                bgColor = Color.FromArgb(52, 152, 219, 20);
            }
            else
            {
                bgColor = _backgroundColor;
            }

            SolidBrush brush = new SolidBrush(bgColor);
            g.FillRectangle(brush, r);
            brush.Dispose();
        }

        /// <summary>
        /// 绘制进度条
        /// </summary>
        private void DrawProgressBar(Graphics g, Rectangle r, double progress, Color progressColor)
        {
            // 计算进度条区域
            int padding = 4;
            Rectangle progressRect = new Rectangle(
                r.X + padding,
                r.Y + padding,
                r.Width - padding * 2,
                r.Height - padding * 2
            );

            // 绘制背景条
            DrawProgressBackground(g, progressRect);

            // 绘制进度条
            if (progress > 0)
            {
                int progressWidth = (int)(progressRect.Width * progress / 100.0);
                Rectangle fillRect = new Rectangle(progressRect.X, progressRect.Y, progressWidth, progressRect.Height);
                DrawProgressFill(g, fillRect, progressColor);
            }
        }

        /// <summary>
        /// 绘制进度条背景
        /// </summary>
        private void DrawProgressBackground(Graphics g, Rectangle rect)
        {
            if (_enableRoundedCorners)
            {
                // 绘制圆角背景
                GraphicsPath path = CreateRoundedRectangle(rect, _cornerRadius);
                SolidBrush brush = new SolidBrush(Color.FromArgb(240, 240, 240));
                g.FillPath(brush, path);

                // 绘制边框
                if (_borderWidth > 0)
                {
                    Pen pen = new Pen(_borderColor, _borderWidth);
                    g.DrawPath(pen, path);
                    pen.Dispose();
                }

                brush.Dispose();
                path.Dispose();
            }
            else
            {
                // 绘制矩形背景
                SolidBrush brush = new SolidBrush(Color.FromArgb(240, 240, 240));
                g.FillRectangle(brush, rect);

                // 绘制边框
                if (_borderWidth > 0)
                {
                    Pen pen = new Pen(_borderColor, _borderWidth);
                    g.DrawRectangle(pen, rect);
                    pen.Dispose();
                }

                brush.Dispose();
            }
        }

        /// <summary>
        /// 绘制进度条填充
        /// </summary>
        private void DrawProgressFill(Graphics g, Rectangle rect, Color color)
        {
            if (rect.Width <= 0) return;

            if (_enableRoundedCorners)
            {
                // 绘制圆角填充
                GraphicsPath path = CreateRoundedRectangle(rect, _cornerRadius);

                if (_enableGradient)
                {
                    // 绘制渐变填充
                    Color lighterColor = GetLighterColor(color);
                    LinearGradientBrush brush = new LinearGradientBrush(
                        rect,
                        lighterColor,
                        color,
                        LinearGradientMode.Horizontal);
                    g.FillPath(brush, path);
                    brush.Dispose();
                }
                else
                {
                    // 绘制纯色填充
                    SolidBrush brush = new SolidBrush(color);
                    g.FillPath(brush, path);
                    brush.Dispose();
                }

                // 绘制高光效果
                DrawHighlight(g, rect, path);
                path.Dispose();
            }
            else
            {
                // 绘制矩形填充
                if (_enableGradient)
                {
                    Color lighterColor = GetLighterColor(color);
                    LinearGradientBrush brush = new LinearGradientBrush(
                        rect,
                        lighterColor,
                        color,
                        LinearGradientMode.Horizontal);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else
                {
                    SolidBrush brush = new SolidBrush(color);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }

                // 绘制高光效果
                DrawHighlight(g, rect, null);
            }

            // 绘制阴影
            if (_enableShadow)
            {
                DrawShadow(g, rect);
            }
        }

        /// <summary>
        /// 绘制高光效果
        /// </summary>
        private void DrawHighlight(Graphics g, Rectangle rect, GraphicsPath path)
        {
            // 创建高光区域
            Rectangle highlightRect = new Rectangle(
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height / 2
            );

            LinearGradientBrush highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(80, Color.White),
                Color.FromArgb(20, Color.White),
                LinearGradientMode.Vertical);

            if (path != null)
            {
                // 使用路径裁剪高光
                Region clip = new Region(path);
                g.SetClip(clip, CombineMode.Intersect);
                g.FillRectangle(highlightBrush, highlightRect);
                g.ResetClip();
                clip.Dispose();
            }
            else
            {
                g.FillRectangle(highlightBrush, highlightRect);
            }

            highlightBrush.Dispose();
        }

        /// <summary>
        /// 绘制阴影效果
        /// </summary>
        private void DrawShadow(Graphics g, Rectangle rect)
        {
            // 创建阴影区域
            Rectangle shadowRect = new Rectangle(
                rect.X + (int)_shadowOffset,
                rect.Y + (int)_shadowOffset,
                rect.Width,
                rect.Height
            );

            GraphicsPath shadowPath;
            if (_enableRoundedCorners)
            {
                shadowPath = CreateRoundedRectangle(shadowRect, _cornerRadius);
            }
            else
            {
                shadowPath = new GraphicsPath();
                shadowPath.AddRectangle(shadowRect);
            }

            PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath);
            shadowBrush.CenterColor = Color.FromArgb(40, Color.Black);
            Color[] surroundColors = new Color[] { Color.FromArgb(0, Color.Black) };
            shadowBrush.SurroundColors = surroundColors;
            g.FillPath(shadowBrush, shadowPath);

            shadowBrush.Dispose();
            shadowPath.Dispose();
        }

        /// <summary>
        /// 绘制文字
        /// </summary>
        private void DrawText(Graphics g, Rectangle r, double progress, Appearance appearance)
        {
            string text = "";

            if (_showValue && _showPercentage)
            {
                text = string.Format("{0:F1}%", progress);
            }
            else if (_showPercentage)
            {
                text = string.Format("{0:F0}%", progress);
            }
            else if (_showValue)
            {
                text = progress.ToString("F1");
            }

            if (string.IsNullOrEmpty(text)) return;

            // 设置字体
            Font font = new Font("Segoe UI", 12f, FontStyle.Bold);

            // 计算文字位置
            SizeF textSize = g.MeasureString(text, font);
            PointF textPoint = new PointF(
                r.X + (r.Width - textSize.Width) / 2,
                r.Y + (r.Height - textSize.Height) / 2
            );

            // 绘制文字阴影
            SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
            g.DrawString(text, font, shadowBrush,
                new PointF(textPoint.X + 1, textPoint.Y + 1));
            shadowBrush.Dispose();

            // 绘制文字
            SolidBrush textBrush = new SolidBrush(_textColor);
            g.DrawString(text, font, textBrush, textPoint);
            textBrush.Dispose();

            font.Dispose();
        }

        /// <summary>
        /// 创建圆角矩形路径
        /// </summary>
        private GraphicsPath CreateRoundedRectangle(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();

            float diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// 获取更亮的颜色
        /// </summary>
        private Color GetLighterColor(Color color)
        {
            int r = Math.Min(255, color.R + 30);
            int g = Math.Min(255, color.G + 30);
            int b = Math.Min(255, color.B + 30);
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// 创建预设主题
        /// </summary>
        public static ModernProgressCellType CreateTheme(ProgressTheme theme)
        {
            ModernProgressCellType cellType = new ModernProgressCellType();

            switch (theme)
            {
                case ProgressTheme.Blue:
                    cellType.PrimaryColor = Color.FromArgb(52, 152, 219);
                    break;
                case ProgressTheme.Green:
                    cellType.PrimaryColor = Color.FromArgb(46, 204, 113);
                    break;
                case ProgressTheme.Purple:
                    cellType.PrimaryColor = Color.FromArgb(155, 89, 182);
                    break;
                case ProgressTheme.Orange:
                    cellType.PrimaryColor = Color.FromArgb(243, 156, 18);
                    break;
                case ProgressTheme.Red:
                    cellType.PrimaryColor = Color.FromArgb(231, 76, 60);
                    break;
                case ProgressTheme.Cyan:
                    cellType.PrimaryColor = Color.FromArgb(26, 188, 156);
                    break;
            }

            return cellType;
        }
    }

    /// <summary>
    /// 进度条主题枚举
    /// </summary>
    public enum ProgressTheme
    {
        Blue,
        Green,
        Purple,
        Orange,
        Red,
        Cyan
    }
}
