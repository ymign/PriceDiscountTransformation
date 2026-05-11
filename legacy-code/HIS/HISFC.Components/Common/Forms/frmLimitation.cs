using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.Common.Forms
{
    public partial class frmLimitation : Form
    {
        public frmLimitation()
        {
            InitializeComponent();
        }

        private string memo = string.Empty;

        /// <summary>
        /// 限制备注信息
        /// </summary>
        public string Memo
        {
            get { return memo; }
            set { memo = value; }
        }

        public string drugName
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        private string isLimit = string.Empty;

        /// <summary>
        /// 是否满足报销条件
        /// </summary>
        public string IsLimit
        {
            get { return isLimit; }
            set { isLimit = value; }
        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.lblMemo.Text = drugName + ":" + memo;
            this.InitComobox();
        }

        private void InitComobox()
        {
            this.cmbLimit.ClearItems();
            List<Neusoft.FrameWork.Models.NeuObject> objList = new List<Neusoft.FrameWork.Models.NeuObject>();
            Neusoft.FrameWork.Models.NeuObject obj1 = new Neusoft.FrameWork.Models.NeuObject();
            obj1.ID = "1";
            obj1.Name = "是";
            objList.Add(obj1);
            Neusoft.FrameWork.Models.NeuObject obj2 = new Neusoft.FrameWork.Models.NeuObject();
            obj2.ID = "0";
            obj2.Name = "否";
            objList.Add(obj2);
            this.cmbLimit.AddItems(objList);
            //this.cmbLimit.Text = "否";
            //this.cmbLimit.Tag = "0";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //this.isLimit = this.cmbLimit.Tag.ToString();

            this.isLimit = "1";

            if (string.IsNullOrEmpty(isLimit))
            {
                MessageBox.Show("必须选择是否满足条件！");
                return;
            }
            this.Close();
        }

        private void btnNO_Click(object sender, EventArgs e)
        {
            this.isLimit = "0";
            this.Close();
        }
    }
}
