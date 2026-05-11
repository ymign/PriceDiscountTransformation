using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS.ZDWY.Internet.BP.QuitFee
{
    public partial class QuitFee : Form
    {
        public QuitFee()
        {
            InitializeComponent();
        }

        private void Test()
        {
            try
            {
                BP.OutPatient.RegisterInfoManager mgr = new OutPatient.RegisterInfoManager();
                string url = "";
                url = this.texturl.Text;
                int res = mgr.QuitFeeTest(url);
                if(res<0)
                {
                    MessageBox.Show("服务异常！");
                }
                else
                {
                    MessageBox.Show("服务正常！");
                }
            }
            catch (Exception ex)
            {
                //报错不停止
            }
        }


        Timer showTextBoxTimer = null;
        private void Start()
        {
            string url = "";
            int time = 1;

            url = this.texturl.Text;
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("服务地址不能为空！");
            }
            time = Neusoft.FrameWork.Function.NConvert.ToInt32(this.texttime.Text);
            time = time <= 0 ? 1 : time;

            showTextBoxTimer = new Timer(); //新建一个Timer对象
            showTextBoxTimer.Interval = time * 60000;//设定多少秒后行动，单位是毫秒
            showTextBoxTimer.Tick += new EventHandler(timer_Tick);//到时所有执行的动作
            showTextBoxTimer.Start();//启动计时
            this.texturl.Enabled = false;
            this.texttime.Enabled = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                BP.OutPatient.RegisterInfoManager mgr = new OutPatient.RegisterInfoManager();
                string url = "";
                url = this.texturl.Text;
                mgr.QuitFee(url);
            }
            catch(Exception ex)
            {
                //报错不停止
            }
        }

        private void Stop()
        {

            if(showTextBoxTimer!=null)
            {
                showTextBoxTimer.Stop();
            }
            this.texturl.Enabled = true;
            this.texttime.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Test();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Stop();
        }
    }
}
