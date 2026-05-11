using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Forms
{
    /// <summary>
    /// 处理进程停止
    /// </summary>
    public delegate bool ProgressStop(); 
    /// <summary>
    /// 进度条
    /// </summary>
    public partial class frmProgressBar : Form
    {
        #region 变量
        /// <summary>
        /// 处理进程停止
        /// </summary>
        public event ProgressStop evnProgressStop;

        #endregion 

        #region 属性

        public int MinBar
        {
            get { return pbBar.Minimum; }
            set 
            { 
                pbBar.Minimum = value;
            }
        }

        public int MaxBar
        {
            get { return pbBar.Maximum; }
            set { pbBar.Maximum = value; }
        }

        public int StepBar
        {
            get { return pbBar.Step; }
            set { pbBar.Step = value; }
        }

        public string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public string SetInfo
        {
            get { return lblInfo.Text; }
            set { lblInfo.Text = value; }
        }

        public int SetValue
        {
            get { return pbBar.Value; }
            set { pbBar.Value = value; }
        }

        #endregion

        public frmProgressBar()
        {
            InitializeComponent();

            pbBar.Minimum = 0;
            pbBar.Maximum = 100;
        }

        private void frmProgressBar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (evnProgressStop != null)
            {
                bool blnRes = evnProgressStop();
                if (!blnRes)
                {
                    e.Cancel = true;

                    MessageBox.Show("取消操作失败！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void MoveStep()
        {
            this.pbBar.PerformStep();
        }
    }
}
