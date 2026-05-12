using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Neusoft.HISFC.Components.OutpatientFee.DB;
using System.Collections.Generic;
using Neusoft.HISFC.Models.SqlSugar;
using System.Net;

namespace Neusoft.HISFC.Components.OutpatientFee.Forms
{
    /// <summary>
    /// 业务状态修改对话框
    /// </summary>
    public partial class frmBizStatusModify : Form
    {
        #region 字段

        private DataRow dataRow = null;
        private Point mouseOffset;
        private bool isDragging = false;
        private DbBizStatus _db = new DbBizStatus();
        #endregion

        #region 属性
        public DataRow DataRow
        {
            get { return dataRow; }
            set { dataRow = value; }
        }
        public List<MntBizAdjustLog> listMntBizAdjustLog = new List<MntBizAdjustLog>();

        #endregion

        #region 构造函数

        public frmBizStatusModify()
        {
            InitializeComponent();
            InitializeEvents();
            Init();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化事件
        /// </summary>
        private void InitializeEvents()
        {
            // 窗体拖动
            this.pnlHeader.MouseDown += PnlHeader_MouseDown;
            this.pnlHeader.MouseMove += PnlHeader_MouseMove;
            this.pnlHeader.MouseUp += PnlHeader_MouseUp;

            // 关闭按钮悬停效果
            this.btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(245, 245, 245);
            this.btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.White;

            // 提交按钮悬停效果
            this.btnOK.MouseEnter += (s, e) => btnOK.BackColor = Color.FromArgb(110, 68, 230);
            this.btnOK.MouseLeave += (s, e) => btnOK.BackColor = Color.FromArgb(124, 77, 255);
        }

        private void Init()
        {
            var payModes = _db.GetComDictionaryForType("PAYMODES", "");
            this.cmbPayModes.AddItems(payModes);

            var list = new List<Neusoft.FrameWork.Models.NeuObject>();
            var obj = new Neusoft.FrameWork.Models.NeuObject();
            obj.ID = "1";
            obj.Name = "业务调整";
            list.Add(obj);
            obj = new Neusoft.FrameWork.Models.NeuObject();
            obj.ID = "2";
            obj.Name = "错误操作";
            list.Add(obj);
            obj = new Neusoft.FrameWork.Models.NeuObject();
            obj.ID = "3";
            obj.Name = "系统异常";
            list.Add(obj);
            obj = new Neusoft.FrameWork.Models.NeuObject();
            obj.ID = "4";
            obj.Name = "其它";
            list.Add(obj);
            this.cmbChangeReason.AddItems(list);
            this.cmbChangeReason.Tag = "1";

            var listSeeFlag = new List<Neusoft.FrameWork.Models.NeuObject>();
            var objSeeFlag = new Neusoft.FrameWork.Models.NeuObject();
            objSeeFlag.ID = "0";
            objSeeFlag.Name = "未看诊";
            listSeeFlag.Add(objSeeFlag);

            objSeeFlag = new Neusoft.FrameWork.Models.NeuObject();
            objSeeFlag.ID = "1";
            objSeeFlag.Name = "已看诊";
            listSeeFlag.Add(objSeeFlag);
            this.cmbSeeFlag.AddItems(listSeeFlag);
        }

        #endregion

        #region 窗体拖动

        private void PnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                mouseOffset = new Point(-e.X, -e.Y);
            }
        }

        private void PnlHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                this.Location = mousePos;
            }
        }

        private void PnlHeader_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 加载数据并显示
        /// </summary>
        public void LoadData()
        {
            if (dataRow == null) return;

            var bizType = dataRow["biz_type"].ToString();
            var clinicNo = dataRow["clinic_no"].ToString();
            var invoiceNo = dataRow["invoice_no"].ToString();
            var bizState = dataRow["biz_state"].ToString();
            var payType = dataRow["pay_type"].ToString();

            this.cmbPayModes.Tag = payType;

            this.cmbSeeFlag.Tag = bizState;
            if (bizType != "挂号")
            {
                this.cmbSeeFlag.Enabled = false;
            }


        }

        #endregion

        #region 事件处理

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            var payType = this.cmbPayModes.Tag.ToString();
            var seeFlag = this.cmbSeeFlag.Tag.ToString();
            var changeReason = this.cmbChangeReason.Tag.ToString();

            var remark = this.txtRemark.Text;
            if (string.IsNullOrEmpty(remark))
            {
                MessageBox.Show("备注不能为空!");
                return;
            }

            var bizType = dataRow["biz_type"].ToString();
            var oldBizState = dataRow["biz_state"].ToString();
            var oldPayType = dataRow["pay_type"].ToString();

            listMntBizAdjustLog = new List<MntBizAdjustLog>();
            if (payType != oldPayType)
            {
                var info = new MntBizAdjustLog();
                info.BizType = bizType;
                info.OriginPk = dataRow["clinic_no"].ToString();
                info.InvoiceNo = dataRow["invoice_no"].ToString();
                info.PatientName = dataRow["name"].ToString();
                info.OutpatientId = dataRow["card_no"].ToString();
                info.IdCard = dataRow["idenno"].ToString();
                info.ItemType = "1";
                info.OldValue = oldPayType;
                info.NewValue = payType;
                info.OperCode = this._db.Operator.ID;
                info.OperName = this._db.Operator.Name;
                info.Remark = remark;
                info.OperIp = GetLocalIP();
                info.ChangeReason = changeReason;
                listMntBizAdjustLog.Add(info);
            }

            if (seeFlag != oldBizState)
            {
                var info = new MntBizAdjustLog();
                info.BizType = bizType;
                info.OriginPk = dataRow["clinic_no"].ToString();
                info.InvoiceNo = dataRow["invoice_no"].ToString();
                info.PatientName = dataRow["name"].ToString();
                info.OutpatientId = dataRow["card_no"].ToString();
                info.IdCard = dataRow["idenno"].ToString();
                info.ItemType = "0";
                info.OldValue = oldBizState;
                info.NewValue = seeFlag;
                info.OperCode = this._db.Operator.ID;
                info.OperName = this._db.Operator.Name;
                info.Remark = remark;
                info.OperIp = GetLocalIP();
                info.ChangeReason = changeReason;
                listMntBizAdjustLog.Add(info);
            }



            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private string GetLocalIP()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily.ToString() == "InterNetwork")
                        return ip.ToString();
                }
            }
            catch { }
            return "未知";
        }


        #endregion
    }
}
