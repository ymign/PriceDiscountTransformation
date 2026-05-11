using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using FarPoint.Win.Spread.CellType;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Controls.ModernStyles
{
    // 现代化医疗系统风格的自定义按钮单元格类型（兼容.NET 3.5）
    public class ModernButtonCellType : ButtonCellType
    {
        // 按钮样式枚举
        public enum ButtonStyle
        {
            Primary,    // 深蓝主按钮
            Secondary,  // 深灰次按钮
            Success,    // 绿色成功按钮
            Warning,    // 橙色警告按钮
            Danger,     // 红色危险按钮
            Info        // 青色信息按钮
        }

        // 可自定义的属性
        public int ButtonWidth = 55;      // 按钮宽度  Segoe UI, 9pt
        public int ButtonHeight = 54;      // 按钮高度
        public int CornerRadius = 6;       // 圆角半径
        public string ButtonText = "未知"; // 默认按钮文字
        public ButtonStyle Style = ButtonStyle.Secondary; // 按钮样式
        public Font ButtonFont = new Font("宋体", 12, FontStyle.Bold);

        // 新增：是否渐变、是否高光属性
        public bool EnableGradient = true; // 是否启用渐变
        public bool EnableGloss = true;    // 是否启用高光

        // 支持外部设置按钮颜色
        public Color CustomBackColor = Color.Empty;
        public Color CustomBorderColor = Color.Empty;
        public Color CustomTextColor = Color.Empty;

        public ModernButtonCellType() : base() { }
        public ModernButtonCellType(ButtonCellType g) : base(g) { }
        protected ModernButtonCellType(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        // 保证按钮点击后不进入编辑模式，始终保持自定义样式
        public override Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            return null; // 不返回编辑控件，点击时不会进入编辑状态
        }


        // 重写单元格绘制方法
        public override void PaintCell(
            Graphics g,
            Rectangle r,
            FarPoint.Win.Spread.Appearance appearance,
            object value,
            bool isSelected,
            bool isLocked,
            float zoomFactor)
        {
            string buttonText = value != null ? value.ToString() : ButtonText;
            int btnWidth = (int)(ButtonWidth * zoomFactor);
            int btnHeight = (int)(ButtonHeight * zoomFactor);
            int radius = (int)(CornerRadius * zoomFactor);

            Rectangle buttonRect = new Rectangle(
                r.X + (r.Width - btnWidth) / 2,
                r.Y + (r.Height - btnHeight) / 2,
                btnWidth,
                btnHeight
            );

            // Web风格颜色定义
            Color colorTop, colorBottom, borderColor, textColor, shadowColor, glossColor;
            if (Style == ButtonStyle.Primary)
            {
                colorTop = Color.FromArgb(66, 139, 202);    // 深蓝渐变顶部
                colorBottom = Color.FromArgb(51, 122, 183); // 深蓝渐变底部
                borderColor = Color.FromArgb(46, 109, 164);
                textColor = Color.White;
                shadowColor = Color.FromArgb(180, 200, 220);
                glossColor = Color.FromArgb(100, 255, 255, 255);
            }
            else if (Style == ButtonStyle.Secondary)
            {
                colorTop = Color.FromArgb(91, 192, 222);    // 青色渐变顶部
                colorBottom = Color.FromArgb(70, 184, 218); // 青色渐变底部
                borderColor = Color.FromArgb(49, 176, 213);
                textColor = Color.White;
                shadowColor = Color.FromArgb(180, 220, 230);
                glossColor = Color.FromArgb(100, 255, 255, 255);
            }
            else if (Style == ButtonStyle.Success)
            {
                colorTop = Color.FromArgb(92, 184, 92);     // 绿色渐变顶部
                colorBottom = Color.FromArgb(76, 174, 76);  // 绿色渐变底部
                borderColor = Color.FromArgb(62, 164, 62);
                textColor = Color.White;
                shadowColor = Color.FromArgb(180, 220, 180);
                glossColor = Color.FromArgb(100, 255, 255, 255);
            }
            else if (Style == ButtonStyle.Warning)
            {
                colorTop = Color.FromArgb(240, 173, 78);    // 橙色渐变顶部
                colorBottom = Color.FromArgb(238, 162, 54); // 橙色渐变底部
                borderColor = Color.FromArgb(235, 151, 30);
                textColor = Color.White;
                shadowColor = Color.FromArgb(255, 220, 180);
                glossColor = Color.FromArgb(100, 255, 255, 255);
            }
            else if (Style == ButtonStyle.Danger)
            {
                colorTop = Color.FromArgb(217, 83, 79);     // 红色渐变顶部
                colorBottom = Color.FromArgb(212, 63, 58);  // 红色渐变底部
                borderColor = Color.FromArgb(201, 48, 44);
                textColor = Color.White;
                shadowColor = Color.FromArgb(255, 200, 200);
                glossColor = Color.FromArgb(100, 255, 255, 255);
            }
            else // Info
            {
                colorTop = Color.FromArgb(91, 192, 222);    // 青色渐变顶部
                colorBottom = Color.FromArgb(70, 184, 218); // 青色渐变底部
                borderColor = Color.FromArgb(49, 176, 213);
                textColor = Color.White;
                shadowColor = Color.FromArgb(180, 220, 230);
                glossColor = Color.FromArgb(100, 255, 255, 255);
            }

            if (isLocked)
            {
                colorTop = Color.FromArgb(238, 238, 238);
                colorBottom = Color.FromArgb(204, 204, 204);
                borderColor = Color.FromArgb(189, 189, 189);
                textColor = Color.FromArgb(119, 119, 119);
                shadowColor = Color.FromArgb(240, 240, 240);
                glossColor = Color.FromArgb(50, 255, 255, 255);
            }

            // 阴影
            Rectangle shadowRect = buttonRect;
            shadowRect.Offset(0, 2);
            using (GraphicsPath shadowPath = CreateRoundRectPath(shadowRect, radius))
            using (SolidBrush shadowBrush = new SolidBrush(shadowColor))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(shadowBrush, shadowPath);
            }

            // 渐变背景
            using (GraphicsPath path = CreateRoundRectPath(buttonRect, radius))
            using (LinearGradientBrush brush = new LinearGradientBrush(buttonRect, colorTop, colorBottom, LinearGradientMode.Vertical))
            using (Pen pen = new Pen(borderColor, 1.5f))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);

                // 高光（顶部半透明白色）
                RectangleF glossRect = new RectangleF(buttonRect.X, buttonRect.Y, buttonRect.Width, buttonRect.Height / 2f);
                using (GraphicsPath glossPath = CreateRoundRectPath(Rectangle.Round(glossRect), radius))
                using (SolidBrush glossBrush = new SolidBrush(glossColor))
                {
                    g.SetClip(path);
                    g.FillPath(glossBrush, glossPath);
                    g.ResetClip();
                }
            }

            // 按钮文字
            TextRenderer.DrawText(
                g,
                buttonText,
                ButtonFont,
                buttonRect,
                textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
            );

            // 选中时高亮边框
            if (isSelected)
            {
                using (Pen pen = new Pen(Color.FromArgb(120, 180, 255), 2))
                {
                    g.DrawPath(pen, CreateRoundRectPath(buttonRect, radius));
                }
            }
        }

        // 创建圆角矩形路径（兼容.NET 3.5）
        private GraphicsPath CreateRoundRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            // 左上角
            path.AddArc(arcRect, 180, 90);

            // 右上角
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // 右下角
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // 左下角
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
