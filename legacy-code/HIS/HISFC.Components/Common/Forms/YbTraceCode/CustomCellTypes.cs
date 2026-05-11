using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;

namespace Neusoft.HISFC.Components.Common.Forms.YbTraceCode
{
    /// <summary>
    /// 药品信息单元格类型 - 显示名称和规格（两行）
    /// </summary>
    [Serializable]
    public class DrugInfoCellType : BaseCellType
    {
        public override string Format(object obj)
        {
            if (obj == null) return "";
            return obj.ToString();
        }

        public override object Parse(string s)
        {
            return s;
        }

        public override Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            return null;
        }

        public override object GetEditorValue()
        {
            return null;
        }

        public override void SetEditorValue(object value)
        {
        }

        public override Size GetPreferredSize(Graphics g, Size size, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            return new Size(160, 50);
        }

        public override void PaintCell(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            Color bgColor = isSelected ? Color.FromArgb(240, 247, 255) : Color.White;
            using (SolidBrush brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, r);
            }

            string text = (value == null) ? "" : value.ToString();
            string[] lines = text.Split(new char[] { '\n' }, StringSplitOptions.None);

            string name = (lines.Length > 0) ? lines[0] : "";
            string spec = (lines.Length > 1) ? lines[1] : "";

            // 药品名称 - 字体13F，加粗
            using (Font font = new Font("宋体", 15F, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(51, 51, 51)))
            {
                RectangleF nameRect = new RectangleF(r.X + 4, r.Y + 6, r.Width - 8, 26);
                StringFormat sf = new StringFormat();
                sf.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString(name, font, brush, nameRect, sf);
            }

            // 规格 - 字体9F
            using (Font font = new Font("宋体", 9F))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(102, 102, 102)))
            {
                RectangleF specRect = new RectangleF(r.X + 4, r.Y + 36, r.Width - 8, 18);
                g.DrawString(spec, font, brush, specRect);
            }
        }

    }

    /// <summary>
    /// 序号单元格类型 - 带状态颜色指示
    /// </summary>
    [Serializable]
    public class IndexCellType : BaseCellType
    {
        private Color _statusColor = Color.FromArgb(234, 88, 12);

        public Color StatusColor
        {
            get { return _statusColor; }
            set { _statusColor = value; }
        }

        public override string Format(object obj)
        {
            if (obj == null) return "";
            return obj.ToString();
        }

        public override object Parse(string s)
        {
            return s;
        }

        public override Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            return null;
        }

        public override object GetEditorValue()
        {
            return null;
        }

        public override void SetEditorValue(object value)
        {
        }

        public override Size GetPreferredSize(Graphics g, Size size, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            return new Size(35, 50);
        }

        public override void PaintCell(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            const int LeftBarWidth = 4;

            // 背景色 - 选中时淡蓝色，否则白色
            Color bgColor = isSelected ? Color.FromArgb(240, 247, 255) : Color.White;
            using (SolidBrush brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, r);
            }

            // 左边框指示条
            Rectangle leftBar = new Rectangle(r.X, r.Y, LeftBarWidth, r.Height);
            using (SolidBrush brush = new SolidBrush(_statusColor))
            {
                g.FillRectangle(brush, leftBar);
            }

            // 序号文字
            string text = (value == null) ? "" : value.ToString();
            Rectangle textRect = new Rectangle(r.X + LeftBarWidth, r.Y, r.Width - LeftBarWidth, r.Height);
            using (Font font = new Font("微软雅黑", 13F, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(_statusColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(text, font, brush, textRect, sf);
            }
        }
    }

    /// <summary>
    /// 状态标签单元格类型 - 带圆角背景
    /// </summary>
    [Serializable]
    public class StatusTagCellType : BaseCellType
    {
        public override string Format(object obj)
        {
            if (obj == null) return "";
            return obj.ToString();
        }

        public override object Parse(string s)
        {
            return s;
        }

        public override Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            return null;
        }

        public override object GetEditorValue()
        {
            return null;
        }

        public override void SetEditorValue(object value)
        {
        }

        public override Size GetPreferredSize(Graphics g, Size size, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            return new Size(80, 24);
        }

        private const int SpinnerWidth = 18;

        public override void PaintCell(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, r);
            }

            string text = (value == null) ? "" : value.ToString();

            Color bgColor, textColor, borderColor;
            if (text.Contains("已完成") || text.Contains("拆零自动"))
            {
                bgColor = Color.FromArgb(240, 255, 244);
                textColor = Color.FromArgb(22, 163, 74);
                borderColor = Color.FromArgb(183, 235, 143);
            }
            else if (text.Contains("待采"))
            {
                bgColor = Color.FromArgb(255, 242, 232);
                textColor = Color.FromArgb(234, 88, 12);
                borderColor = Color.FromArgb(255, 187, 150);
            }
            else
            {
                bgColor = Color.FromArgb(245, 245, 245);
                textColor = Color.FromArgb(153, 153, 153);
                borderColor = Color.FromArgb(221, 221, 221);
            }

            // 胶囊形状徽章 - 更大更圆润
            int tagWidth = 85;
            int tagHeight = 26;
            int x = r.X + (r.Width - tagWidth) / 2;
            int y = r.Y + (r.Height - tagHeight) / 2;
            Rectangle tagRect = new Rectangle(x, y, tagWidth, tagHeight);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRectangle(tagRect, tagHeight / 2)) // 完全圆角(胶囊形)
            using (SolidBrush bgBrush = new SolidBrush(bgColor))
            using (Pen borderPen = new Pen(borderColor, 1))
            {
                g.FillPath(bgBrush, path);
                g.DrawPath(borderPen, path);
            }

            using (Font font = new Font("微软雅黑", 9F, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(text, font, brush, tagRect, sf);
            }
            g.SmoothingMode = SmoothingMode.Default;
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// 样式化按钮单元格类型 - 带边框和悬停效果
    /// </summary>
    [Serializable]
    public class StyledButtonCellType : ButtonCellType
    {
        private bool _isPrimary = false;

        public bool IsPrimary
        {
            get { return _isPrimary; }
            set { _isPrimary = value; }
        }

        public override void PaintCell(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, r);
            }

            string text;
            if (value != null)
                text = value.ToString();
            else if (Text != null)
                text = Text;
            else
                text = "";

            Color bgColor;
            Color textColor;
            Color borderColor;
            if (_isPrimary)
            {
                // 「已采」按钮 - 鲜明绿色
                bgColor = Color.FromArgb(220, 252, 231);
                textColor = Color.FromArgb(22, 163, 74);
                borderColor = Color.FromArgb(22, 163, 74);
            }
            else
            {
                // 「原码」按钮 - 增强对比度
                bgColor = Color.FromArgb(249, 250, 251);
                textColor = Color.FromArgb(55, 65, 81);
                borderColor = Color.FromArgb(156, 163, 175); // 更深的边框
            }

            // 胶囊形按钮 - 更大更圆润
            int btnWidth = Math.Min(60, r.Width - 8);
            int btnHeight = 24;
            int x = r.X + (r.Width - btnWidth) / 2;
            int y = r.Y + (r.Height - btnHeight) / 2;
            Rectangle btnRect = new Rectangle(x, y, btnWidth, btnHeight);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRectangle(btnRect, btnHeight / 2)) // 胶囊形
            using (SolidBrush bgBrush = new SolidBrush(bgColor))
            using (Pen borderPen = new Pen(borderColor, 1))
            {
                g.FillPath(bgBrush, path);
                g.DrawPath(borderPen, path);
            }

            using (Font font = new Font("微软雅黑", 9F))
            using (SolidBrush brush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(text, font, brush, btnRect, sf);
            }
            g.SmoothingMode = SmoothingMode.Default;
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// 开关单元格类型 - 模拟 Toggle Switch 样式（纯渲染，不进入编辑模式）
    /// </summary>
    [Serializable]
    public class ToggleSwitchCellType : BaseCellType
    {
        public override string Format(object obj)
        {
            if (obj is bool && (bool)obj)
            {
                return "True";
            }
            return "False";
        }

        public override object Parse(string s)
        {
            bool result;
            bool.TryParse(s, out result);
            return result;
        }

        public override Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            return null; // 不提供编辑器
        }

        public override object GetEditorValue()
        {
            return null;
        }

        public override void SetEditorValue(object value)
        {
        }

        public override Size GetPreferredSize(Graphics g, Size size, FarPoint.Win.Spread.Appearance appearance, object value, float zoomFactor)
        {
            return new Size(65, 30);
        }

        public override void PaintCell(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, r);
            }

            bool isChecked = false;
            if (value is bool)
            {
                isChecked = (bool)value;
            }
            else if (value != null)
            {
                bool.TryParse(value.ToString(), out isChecked);
            }

            int trackWidth = 36;
            int trackHeight = 20;
            int thumbSize = 16;

            int x = r.X + (r.Width - trackWidth - 30) / 2;
            int y = r.Y + (r.Height - trackHeight) / 2;

            Color trackColor = isChecked ? Color.FromArgb(22, 163, 74) : Color.FromArgb(204, 204, 204);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle trackRect = new Rectangle(x, y, trackWidth, trackHeight);
            using (GraphicsPath path = CreateRoundedRectangle(trackRect, trackHeight / 2))
            using (SolidBrush brush = new SolidBrush(trackColor))
            {
                g.FillPath(brush, path);
            }

            int thumbX = isChecked ? x + trackWidth - thumbSize - 2 : x + 2;
            int thumbY = y + 2;
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillEllipse(brush, thumbX, thumbY, thumbSize, thumbSize);
            }

            string labelText = isChecked ? "全退" : "不退";
            Color labelColor = isChecked ? Color.FromArgb(22, 163, 74) : Color.FromArgb(153, 153, 153);
            using (Font font = new Font("微软雅黑", 9F))
            using (SolidBrush brush = new SolidBrush(labelColor))
            {
                RectangleF textRect = new RectangleF(x + trackWidth + 4, r.Y, 30, r.Height);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(labelText, font, brush, textRect, sf);
            }
            g.SmoothingMode = SmoothingMode.Default;
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// 数量输入单元格类型 - 带边框样式
    /// </summary>
    [Serializable]
    public class StyledNumberCellType : NumberCellType
    {
        public const int BoxWidth = 60;
        public const int BoxHeight = 26;
        public const int SpinnerWidth = 18;

        public override Control GetEditorControl(FarPoint.Win.Spread.Appearance appearance, float zoomFactor)
        {
            Control editor = base.GetEditorControl(appearance, zoomFactor);
            if (editor != null)
            {
                editor.BackColor = Color.White;
                editor.ForeColor = Color.FromArgb(51, 51, 51);
                editor.Font = new Font("微软雅黑", 11F, FontStyle.Bold);
                TextBox tb = editor as TextBox;
                if (tb != null)
                {
                    tb.BorderStyle = BorderStyle.None;
                    tb.TextAlign = HorizontalAlignment.Center;
                }
            }
            return editor;
        }

        public override void PaintCell(Graphics g, Rectangle r, FarPoint.Win.Spread.Appearance appearance, object value, bool isSelected, bool isLocked, float zoomFactor)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, r);
            }

            string text = (value == null) ? "" : value.ToString();

            int x = r.X + (r.Width - BoxWidth) / 2;
            int y = r.Y + (r.Height - BoxHeight) / 2;
            Rectangle boxRect = new Rectangle(x, y, BoxWidth, BoxHeight);

            Color borderColor = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(209, 213, 219);
            using (SolidBrush bgBrush = new SolidBrush(Color.White))
            using (Pen borderPen = new Pen(borderColor, 1))
            {
                g.FillRectangle(bgBrush, boxRect);
                g.DrawRectangle(borderPen, boxRect);
            }

            Rectangle textRect = new Rectangle(boxRect.X + 6, boxRect.Y, boxRect.Width - SpinnerWidth - 8, boxRect.Height);

            using (Font font = new Font("微软雅黑", 11F, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(51, 51, 51)))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(text, font, brush, textRect, sf);
            }

            // 绘制加减按钮区域
            Rectangle spinnerRect = new Rectangle(boxRect.Right - SpinnerWidth, boxRect.Y, SpinnerWidth, boxRect.Height);
            using (Pen borderPen = new Pen(Color.FromArgb(209, 213, 219), 1))
            {
                g.DrawLine(borderPen, spinnerRect.Left, spinnerRect.Top, spinnerRect.Left, spinnerRect.Bottom);
                g.DrawLine(borderPen, spinnerRect.Left, spinnerRect.Top + spinnerRect.Height / 2, spinnerRect.Right, spinnerRect.Top + spinnerRect.Height / 2);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            DrawTriangle(g, new Rectangle(spinnerRect.Left + 3, spinnerRect.Top + 3, SpinnerWidth - 6, spinnerRect.Height / 2 - 4), true);
            DrawTriangle(g, new Rectangle(spinnerRect.Left + 3, spinnerRect.Top + spinnerRect.Height / 2 + 2, SpinnerWidth - 6, spinnerRect.Height / 2 - 4), false);
            g.SmoothingMode = SmoothingMode.Default;
        }

        private static void DrawTriangle(Graphics g, Rectangle rect, bool isUp)
        {
            Point p1, p2, p3;
            if (isUp)
            {
                p1 = new Point(rect.Left, rect.Bottom);
                p2 = new Point(rect.Right, rect.Bottom);
                p3 = new Point(rect.Left + rect.Width / 2, rect.Top);
            }
            else
            {
                p1 = new Point(rect.Left, rect.Top);
                p2 = new Point(rect.Right, rect.Top);
                p3 = new Point(rect.Left + rect.Width / 2, rect.Bottom);
            }

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(153, 153, 153)))
            {
                g.FillPolygon(brush, new[] { p1, p2, p3 });
            }
        }
    }
}
