using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    public partial class ChooseDate : Neusoft.FrameWork.WinForms.Forms.BaseForm
    {
        public delegate void ButtonHandle(DateTime date);
        public event ButtonHandle OkEvent;
        public event ButtonHandle CloseEvent;
        public ChooseDate()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (CloseEvent != null)
                CloseEvent(new DateTime());
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OkEvent != null)
                OkEvent(this.dateTimePicker1.Value);
            this.Close();
        }
    }
}
