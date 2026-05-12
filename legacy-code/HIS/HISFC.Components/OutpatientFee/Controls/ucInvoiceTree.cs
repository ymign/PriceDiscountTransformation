using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.Models.Fee.Outpatient;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    /// <summary>
    /// 显示发票控件
    /// </summary>
    public partial class ucInvoiceTree : UserControl
    {
        #region 成员
        /// <summary>
        /// 默认宽度
        /// </summary>
        private int iUcWidth = 220;
        /// <summary>
        /// 费用综合业务层
        /// </summary>
        protected Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();
        /// <summary>
        /// 门诊费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        /// <summary>
        /// 卡号未找到
        /// </summary>
        public event CardNoFind evnCardNoFind;
        /// <summary>
        /// 未找到发票信息
        /// </summary>
        public event InvoiceNoFind evnInvoiceNoFind;
        /// <summary>
        /// 所选发票节点改变
        /// </summary>
        public event InvoiceNodeSelectChange evnInvoiceSelectChange;
        /// <summary>
        /// 当前卡号信息
        /// </summary>
        HISFC.Models.Account.AccountCard objAccCard = null;
        #endregion

        #region 属性
        /// <summary>
        /// 默认宽度
        /// </summary>
        public int UcWidth
        {
            get { return iUcWidth; }
            set { iUcWidth = value; }
        }
        /// <summary>
        /// 获取所选节点
        /// </summary>
        public TreeNode SelectNode
        {
            get { return this.trvInvoice.SelectedNode; }
        }
        /// <summary>
        /// 获取发票节点树
        /// </summary>
        public TreeView InvoiceTree
        {
            get { return this.trvInvoice; }
        }

        public string MCardNo
        {
            get { return txtCardNO.Text; }
            set
            {
                txtCardNO.Text = value;
                txtCardNO_KeyDown(null, new KeyEventArgs(Keys.Enter));
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ucInvoiceTree()
        {
            InitializeComponent();
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            iUcWidth = this.Width;
            this.Width = this.btnShow.Width;
            this.panelTree.Visible = false;
            this.dtpRegBegin.Visible = false;
            this.dtpRegEnd.Visible = false;
            this.txtCardNO.Visible = false;
            this.btnClose.Visible = false;
            this.btnShow.Visible = true;
        }
        int panelWidth = 0;
        private void btnShow_Click(object sender, EventArgs e)
        {
            if (panelWidth == 0)
            {
                panelWidth = iUcWidth;
            }

            this.Width = panelWidth;
            this.btnShow.Visible = false;

            this.panelTree.Visible = true;
            this.dtpRegBegin.Visible = true;
            this.dtpRegEnd.Visible = true;
            this.txtCardNO.Visible = true;
            this.btnClose.Visible = true;

            this.txtCardNO.Focus();
        }

        private void txtCardNO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            string strCard = txtCardNO.Text.Trim();
            if (string.IsNullOrEmpty(strCard))
                return;


            HISFC.Models.Account.AccountCard objCard = new Neusoft.HISFC.Models.Account.AccountCard();
            int iTemp = feeIntegrate.ValidMarkNO(strCard, ref objCard);
            if (iTemp <= 0 || objCard == null)
            {
                MessageBox.Show("无效卡号，请联系管理员！");

                if (this.evnCardNoFind != null)
                {
                    this.evnCardNoFind();
                }

                return;
            }

            objAccCard = objCard;

            this.trvInvoice.Nodes.Clear();

            DateTime dtReg = this.dtpRegBegin.Value.Date;
            DateTime dtRegEnd = this.dtpRegEnd.Value.Date.AddDays(1).AddSeconds(-1);
            List<Balance> lstInvoice = null;
            iTemp = outpatientManager.QueryInvoiceInfoByCardNo(objCard.Patient.PID.CardNO, dtReg, dtRegEnd, out lstInvoice);
            if (iTemp <= 0)
            {
                MessageBox.Show(outpatientManager.Err);

                if (evnInvoiceNoFind != null)
                {
                    this.evnInvoiceNoFind();
                }

                return;
            }

            if (lstInvoice == null || lstInvoice.Count <= 0)
            {
                MessageBox.Show("未找到发票信息！");

                if (evnInvoiceNoFind != null)
                {
                    this.evnInvoiceNoFind();
                }

                return;
            }

            AddInvoiceToTree(lstInvoice);
            this.txtCardNO.Text = objCard.MarkNO;
        }
        /// <summary>
        /// 添加发票信息到树
        /// </summary>
        /// <param name="lstInvoice"></param>
        private void AddInvoiceToTree(List<Balance> lstInvoice)
        {
            this.trvInvoice.Nodes.Clear();
            if (lstInvoice == null || lstInvoice.Count <= 0)
                return;

            foreach (Balance invoice in lstInvoice)
            {
                AddInvoiceToTree(invoice);
            }

            this.trvInvoice.ExpandAll();
        }

        /// <summary>
        /// 添加到树
        /// {7A484F24-EFB0-414d-8C25-F89D51BC4846}
        /// </summary>
        /// <param name="invoice"></param>
        private void AddInvoiceToTree(Balance invoice)
        {
            if (invoice == null)
                return;

            TreeNode[] tnArr = trvInvoice.Nodes.Find(invoice.Patient.ID, true);

            TreeNode tn = null;
            TreeNode tnTemp = null;
            if (tnArr == null || tnArr.Length <= 0)
            {
                tn = new TreeNode();
                tn.Name = invoice.Patient.ID;
                tn.Text = invoice.Patient.Name + " 【" + ((Neusoft.HISFC.Models.Registration.Register)invoice.Patient).DoctorInfo.SeeDate.ToString("yyyy-MM-dd HH:mm") + "】";

                trvInvoice.Nodes.Add(tn);

                tnTemp = new TreeNode();
                tnTemp.Text = "终端发票";
                tnTemp.Name = invoice.Patient.ID + " - " + tnTemp.Text;
                tnTemp.ForeColor = Color.Red;
                tn.Nodes.Add(tnTemp);

                tnTemp = new TreeNode();
                tnTemp.Text = "收费员发票";
                tnTemp.Name = invoice.Patient.ID + " - " + tnTemp.Text;
                tn.Nodes.Add(tnTemp);
                
                tnTemp = new TreeNode();
                tnTemp.Name = invoice.Invoice.ID + "-" + invoice.CombNO;
                tnTemp.Text = invoice.Invoice.ID;
                tnTemp.Tag = invoice;

                if (invoice.IsAccount)
                {
                    tn.Nodes[0].Nodes.Add(tnTemp);
                }
                else
                {
                    tn.Nodes[1].Nodes.Add(tnTemp);
                }
            }
            else
            {
                tnTemp = new TreeNode();
                tnTemp.Name = invoice.Invoice.ID + "-" + invoice.CombNO;
                tnTemp.Text = invoice.Invoice.ID;
                tnTemp.Tag = invoice;

                tn = tnArr[0];

                if (invoice.IsAccount)
                {
                    tn.Nodes[0].Nodes.Add(tnTemp);
                }
                else
                {
                    tn.Nodes[1].Nodes.Add(tnTemp);
                }
            }
        }

        private void trvInvoice_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                Balance invoice = e.Node.Tag as Balance;
                if (invoice != null && this.evnInvoiceSelectChange != null)
                {
                    this.evnInvoiceSelectChange(this, invoice);
                }

            }
        }
        /// <summary>
        /// 刷新控件
        /// {7A484F24-EFB0-414d-8C25-F89D51BC4846}
        /// </summary>
        public void RefurbishInvoice()
        {
            trvInvoice.Nodes.Clear();
            if (objAccCard == null)
            {
                return;
            }

            DateTime dtReg = this.dtpRegBegin.Value.Date;
            DateTime dtRegEnd = this.dtpRegEnd.Value.Date.AddDays(1).AddSeconds(-1);
            List<Balance> lstInvoice = null;
            int iTemp = outpatientManager.QueryInvoiceInfoByCardNo(objAccCard.Patient.PID.CardNO, dtReg, dtRegEnd, out lstInvoice);
            if (iTemp <= 0)
            {
                MessageBox.Show(outpatientManager.Err);

                if (evnInvoiceNoFind != null)
                {
                    this.evnInvoiceNoFind();
                }

                return;
            }

            if (lstInvoice == null || lstInvoice.Count <= 0)
            {
                if (evnInvoiceNoFind != null)
                {
                    this.evnInvoiceNoFind();
                }
                else
                {
                    MessageBox.Show("未找到发票信息！");
                }

                return;
            }

            AddInvoiceToTree(lstInvoice);
        }
        /// <summary>
        /// 隐藏控件
        /// </summary>
        public void UcClose()
        {
            btnClose_Click(null, null);
        }

    }
    /// <summary>
    /// 卡号未找到
    /// </summary>
    public delegate void CardNoFind();
    /// <summary>
    /// 未找到发票信息
    /// </summary>
    public delegate void InvoiceNoFind();
    /// <summary>
    /// 所选发票节点改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="invoice"></param>
    public delegate void InvoiceNodeSelectChange(object sender, Balance invoice);
}
