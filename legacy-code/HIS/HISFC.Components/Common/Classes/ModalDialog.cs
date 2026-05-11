using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection;

namespace Neusoft.HISFC.Components.Common.Classes
{
    /// <summary>
    /// 模态对话框助手类，提供类似Web模态框的效果
    /// </summary>
    public static class ModalDialog
    {
        /// <summary>
        /// 显示带蒙版的模态对话框
        /// </summary>
        /// <param name="childForm">要显示的子窗体</param>
        /// <param name="owner">父窗体</param>
        /// <param name="maskOpacity">蒙版透明度 (0.0-1.0)</param>
        /// <param name="maskColor">蒙版颜色</param>
        /// <returns>对话框结果</returns>
        public static DialogResult ShowDialog(Form childForm, IWin32Window owner, double maskOpacity, Color maskColor)
        {
            if (childForm == null)
                throw new ArgumentNullException("childForm");

            if (owner == null)
                return childForm.ShowDialog();

            try
            {
                Control ownerControl = (Control)owner;

                // 预先计算位置和大小
                Rectangle ownerBounds = new Rectangle(
                    ownerControl.PointToScreen(Point.Empty),
                    ownerControl.Size);

                TransparentMaskForm maskForm = new TransparentMaskForm(
                    ownerBounds, maskOpacity, maskColor, childForm);

                return maskForm.ShowDialog(owner);
            }
            catch (Exception)
            {
                // 如果出现异常，回退到普通模态对话框
                return childForm.ShowDialog(owner);
            }
        }

        /// <summary>
        /// 显示带蒙版的模态对话框（使用默认设置）
        /// </summary>
        /// <param name="childForm">要显示的子窗体</param>
        /// <param name="owner">父窗体</param>
        /// <returns>对话框结果</returns>
        public static DialogResult ShowDialog(Form childForm, IWin32Window owner)
        {
            return ShowDialog(childForm, owner, 0.5, Color.Black);
        }
    }

    /// <summary>
    /// 透明蒙版窗体
    /// </summary>
    internal class TransparentMaskForm : Form
    {
        #region Windows API
        private const int WM_ACTIVATE = 6;
        private const int WM_ACTIVATEAPP = 28;
        private const int WM_NCACTIVATE = 134;
        private const int WA_INACTIVE = 0;
        private const int WM_MOUSEACTIVATE = 33;
        private const int MA_NOACTIVATE = 3;

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr handle);
        #endregion

        #region 属性
        private Form _childForm;
        private double _maskOpacity = 0.5;
        private Color _maskColor = Color.Black;

        /// <summary>
        /// 子窗体
        /// </summary>
        public Form ChildForm
        {
            get { return _childForm; }
            set { _childForm = value; }
        }

        /// <summary>
        /// 蒙版透明度
        /// </summary>
        public double MaskOpacity
        {
            get { return _maskOpacity; }
            set { _maskOpacity = value; }
        }

        /// <summary>
        /// 蒙版颜色
        /// </summary>
        public Color MaskColor
        {
            get { return _maskColor; }
            set { _maskColor = value; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // WS_EX_COMPOSITED
                return cp;
            }
        }
        #endregion

        #region 构造函数
        public TransparentMaskForm()
        {
            InitializeForm();
        }

        public TransparentMaskForm(Rectangle bounds, double opacity, Color color, Form child)
        {
            _maskOpacity = opacity;
            _maskColor = color;
            _childForm = child;

            InitializeForm();

            // 直接设置位置和大小
            this.Bounds = bounds;
            this.BackColor = color;
            this.Opacity = opacity;
        }

        private void InitializeForm()
        {
            // 最小化样式设置，提升性能
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            // 设置窗体属性
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 绑定事件
            this.Shown += TransparentMaskForm_Shown;
        }
        #endregion

        #region 事件处理
        private void TransparentMaskForm_Shown(object sender, EventArgs e)
        {
            if (ChildForm != null)
            {
                // 窗体显示后立即显示子对话框
                ChildForm.StartPosition = FormStartPosition.CenterParent;
                DialogResult result = ChildForm.ShowDialog(this);
                this.DialogResult = result;
                this.Close();
            }
        }

        protected override void WndProc(ref Message m)
        {
            // 简化消息处理，只处理必要的消息
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }

            base.WndProc(ref m);
        }
        #endregion
    }
}
