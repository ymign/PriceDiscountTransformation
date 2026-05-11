using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Neusoft.HISFC.Components.Common.Controls
{
    public class ModernButton : Button
    {
        // 自定义属性字段
        private Color _hoverColor = Color.FromArgb(50, 150, 250);
        private Color _clickColor = Color.FromArgb(40, 120, 200);
        private int _borderRadius = 4;
        private Color _borderColor = Color.FromArgb(200, 200, 200);

        public ModernButton()
        {
            // 基础样式设置
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(63, 136, 239);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Size = new Size(120, 40);
            Padding = new Padding(5);

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        // 自定义属性（使用兼容写法）
        public Color HoverColor
        {
            get { return _hoverColor; }
            set { _hoverColor = value; Invalidate(); }
        }

        public Color ClickColor
        {
            get { return _clickColor; }
            set { _clickColor = value; Invalidate(); }
        }

        public int BorderRadius
        {
            get { return _borderRadius; }
            set { _borderRadius = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            // 在绘制边框前添加抗锯齿
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 创建绘图区域
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 创建圆角路径
            using (var path = GetRoundPath(ClientRectangle))
            {
                // 设置区域裁剪（关键修复）
                this.Region = new Region(path);

                // 绘制背景
                using (var brush = new SolidBrush(GetStateColor()))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // 绘制边框
                using (var pen = new Pen(BorderColor, 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            // 绘制文字（保持不变）
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), ClientRectangle, sf);
            }
        }



        private Color GetStateColor()
        {
            if (!Enabled) return Color.Gray;
            if (ClientRectangle.Contains(PointToClient(Cursor.Position)))
                return Capture ? ClickColor : HoverColor;
            return BackColor;
        }

        // 更新路径生成方法（更精确的圆角计算）
        private GraphicsPath GetRoundPath(Rectangle rect)
        {
            var path = new GraphicsPath();
            float diameter = BorderRadius * 2;

            // 调整边界防止溢出
            rect.Width -= 1;
            rect.Height -= 1;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        // 状态事件处理
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }
    }
}
