using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Controls
{
    public partial class ucUniReport : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucUniReport()
        {
            InitializeComponent();
        }

        private string uniUrl = "http://localhost:8080";

        /// <summary>
        /// 设置UniReport的Url
        /// </summary>
        [Category("控件设置"), Description("设置UniReport的Url")]
        public string UniReportUrl
        {
            get
            {
                return this.uniUrl;
            }
            set
            {
                this.uniUrl = value;
            }
        }

        private string uniUser = "";

        /// <summary>
        /// 设置UniReport的角色用户
        /// </summary>
        [Category("控件设置"), Description("设置UniReport的角色用户")]
        public string UniUser
        {
            get
            {
                return this.uniUser;
            }
            set
            {
                this.uniUser = value;
            }
        }

        private string uniPasswd = "";

        /// <summary>
        /// 设置UniReport的角色密码
        /// </summary>
        [Category("控件设置"), Description("设置UniReport的角色密码")]
        public string UniPasswd
        {
            get
            {
                return this.uniPasswd;
            }
            set
            {
                this.uniPasswd = value;
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            LoadUniReport();
            
            base.OnLoad(e);
        }

        private void LoadUniReport()
        {
            System.Uri url = new Uri(this.uniUrl + "?eap_username=" + this.uniUser + "&eap_password=" + this.uniPasswd);
            this.webBrowser1.Url = url;

            this.txtUrl.Text = this.uniUrl;
            this.txtUser.Text = this.uniUser;
            this.txtPasd.Text = this.uniPasswd;
        }

        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            webBrowser1.Navigate(webBrowser1.StatusText);  
        }

        private void txtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            System.Uri url = new Uri(this.txtUrl.Text);

            this.webBrowser1.Url = url;
        }

        private void btRefresh_Click(object sender, EventArgs e)
        {
            this.LoadUniReport();
        }
    }
}
