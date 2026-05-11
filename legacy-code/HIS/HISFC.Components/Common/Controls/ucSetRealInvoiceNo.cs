using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.Common.Controls
{
    public partial class ucSetRealInvoiceNo : UserControl
    {
        public ucSetRealInvoiceNo()
        {
            InitializeComponent();
            this.Load += new EventHandler(ucSetRealInvoiceNo_Load);
            this.cmbInvoiceType.SelectedIndexChanged += new EventHandler(cmbInvoiceType_SelectedIndexChanged);
            this.cmbInvoiceType.KeyDown += new KeyEventHandler(cmbInvoiceType_KeyDown);
            this.lblMessage.Text = "收款员：" + Neusoft.FrameWork.Management.Connection.Operator.Name + "【" +
                Neusoft.FrameWork.Management.Connection.Operator.ID + "】";
            this.txtReal.KeyDown += new KeyEventHandler(txtReal_KeyDown);
        }

        void txtReal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                this.btOK.Focus();
            }
        }

        private Neusoft.HISFC.BizProcess.Integrate.Manager managerIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Manager();
        private Neusoft.HISFC.BizLogic.Fee.InPatient feeInpatient = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        private Neusoft.HISFC.BizLogic.Fee.RealInvoice invoiceManager = new Neusoft.HISFC.BizLogic.Fee.RealInvoice();
        string nextInvoiceType = string.Empty;//当前发票类型
        string nextRealInvoice = string.Empty;//当前发票类型对应下一张发票号

        void cmbInvoiceType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                this.GetNextRealInvoice();
            }
        }

        void cmbInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GetNextRealInvoice();
        }

        void GetNextRealInvoice()
        {
            this.txtReal.Text = string.Empty;
            nextInvoiceType = string.Empty;//当前发票类型
            nextRealInvoice = string.Empty;//当前发票类型对应下一张发票号

            //获取下一张实际发票号
            if (this.GetRealInvoice() == -1)
            {
                //在出错的地方已有错误提示
                //MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("获取" + this.cmbInvoiceType.SelectedItem.Name + "下一张实际发票号出错！"));
                return;
            }
            else
            {
                //nextInvoiceType = objNext.Name;
                this.txtReal.Text = nextRealInvoice;
                this.txtReal.Focus();
            }
        }

        int GetRealInvoice()
        {
            Neusoft.FrameWork.Models.NeuObject objNext = new Neusoft.FrameWork.Models.NeuObject();
            objNext.ID = Neusoft.FrameWork.Management.Connection.Operator.ID;
            objNext.Name = this.cmbInvoiceType.SelectedItem.ID;

            if (objNext.Name == null || objNext.Name == string.Empty)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("请选择发票类型！"));
                return -1;
            }
            //获取下一张实际发票号
            if (this.invoiceManager.QueryNextRealInvoiceNo(objNext, ref nextRealInvoice) == -1)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("获取" + this.cmbInvoiceType.SelectedItem.Name + "下一张实际发票号出错！"+feeInpatient.Err));
                return -1;
            }
            nextInvoiceType = objNext.Name;
            return 1;
        }

        void ucSetRealInvoiceNo_Load(object sender, EventArgs e)
        {
            this.Init();
        }

        public int GetRealInvoiceNo(string invoiceType, string operId)
        {
            /*
            //先根据操作员ID和发票类型获取当前发票号。
            Neusoft.FrameWork.Models.NeuObject objGetNextRealInvoice = new Neusoft.FrameWork.Models.NeuObject();
            Neusoft.HISFC.BizProcess.Integrate.Fee feeMgr = new Neusoft.HISFC.BizProcess.Integrate.Fee();
            objGetNextRealInvoice.ID = operId;  //.Operator.ID;
            objGetNextRealInvoice.Name = invoiceType;

             //维护了对照关系再执行该操作
            string realInvoiceNo = string.Empty;
            string errCode="";
            if (this.invoiceManager.QueryNextRealInvoiceNo(objGetNextRealInvoice, ref realInvoiceNo) == -1)
            {
                MessageBox.Show(invoiceManager.Err);
                return -1;
            }

            string invoiceNo_Comp = (Neusoft.FrameWork.Function.NConvert.ToDecimal(this.invoiceManager.GetUsingInvoiceNO(invoiceType)) + 1).ToString();

            //然后显示给操作员提示是否修改，
            if (string.IsNullOrEmpty(realInvoiceNo))
            {
                this.SetInvoiceKind(invoiceType);
                Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(ucRealinvoiceNo);
                if (ucRealinvoiceNo.FindForm().DialogResult == DialogResult.OK) //第一次设置实际发票号成功,插入电脑发票号
                {
                    Neusoft.HISFC.BizProcess.Integrate.Fee feeIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Fee();

                    if (this.invoiceManager.InsertNewInvoice(invoiceType) == -1)
                    {
                        MessageBox.Show("设置初始发票号失败,请联系信息科,谢谢");
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
                return 1;
            }
            else
            {
                DialogResult rs;

                rs = MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("当前物理发票号为：" + realInvoiceNo + "，电脑发票号为：" + invoiceNo_Comp + "；\n 是否进行修改？"), "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (rs == DialogResult.Yes)
                {
                    this.SetInvoiceKind(invoiceType);
                    Neusoft.FrameWork.WinForms.Classes.Function.PopShowControl(this);

                    return 1;
                }
                else
                {
                    return 1;
                }
            }**/
            return 1;
        }

        private void Init()
        {
            //获取发票类型
            this.cmbInvoiceType.AddItems((managerIntegrate.GetConstantList("GetInvoiceType")));

            if (this.cmbInvoiceType.alItems == null || this.cmbInvoiceType.alItems.Count == 0)
            {
                MessageBox.Show("请在常数维护中维护收据的类别");
                return;
            }
            this.lblMessage.Text = "收款员：" + Neusoft.FrameWork.Management.Connection.Operator.Name + "【" +
                Neusoft.FrameWork.Management.Connection.Operator.ID + "】";
            if (this.invoiceKind != "")
            {
                this.cmbInvoiceType.Tag = this.invoiceKind;
                this.cmbInvoiceType.Enabled = false;
            }

            /*
            Neusoft.HISFC.BizProcess.Integrate.Fee feeMgr = new Neusoft.HISFC.BizProcess.Integrate.Fee();
            string invoiceNo_Comp = (Neusoft.FrameWork.Function.NConvert.ToDecimal(feeMgr.GetUsingInvoiceNO(invoiceKind)) + 1).ToString();
            if (string.IsNullOrEmpty(invoiceNo_Comp))
            {
                MessageBox.Show("获取下一张电脑发票号失败！请联系电脑中心！");
                return;
            }
            this.txtInvoiceNoComp.Text = invoiceNo_Comp;
             * **/
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (this.txtReal.Text.Trim() == string.Empty)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("请输入下一张实际发票号！"));
                return;
            }
            if (this.txtInvoiceNoComp.Text.Trim() == string.Empty)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("请输入下一张电脑发票号！"));
                return;
            }


            Neusoft.FrameWork.Models.NeuObject objUpdate = new Neusoft.FrameWork.Models.NeuObject();
            objUpdate.ID = Neusoft.FrameWork.Management.Connection.Operator.ID;
            objUpdate.Name = this.cmbInvoiceType.SelectedItem.ID;
            objUpdate.Memo = this.txtReal.Text.Trim();
            objUpdate.User01 = nextRealInvoice;
            //对要更新的发票号进行判断，是否存在数据库中
            Neusoft.HISFC.Models.Fee.InvoiceExtend realInvoice = invoiceManager.GetInoviceByRealInvoice(this.txtReal.Text.Trim(), this.cmbInvoiceType.SelectedItem.ID);
            if (realInvoice != null)
            {
                MessageBox.Show("此实际发票号已经在数据库中存在，请进行修改" + invoiceManager.Err);
                return;
            }
            /*
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            feeInpatient.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            //先更新
            if (feeInpatient.UpdateNextRealInvoiceNo(objUpdate) <= 0)
            {
                //插入
                if (feeInpatient.InsertIntoNextRealInvoiceNo(objUpdate) <= 0)
                {
                    MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新下一张实际发票信息出错！") + feeInpatient.Err);
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    return;
                }
            }
            string txtInvoiceNo = this.txtInvoiceNoComp.Text;
            if (Neusoft.FrameWork.Function.NConvert.ToInt32(feeInpatient.ExecSqlReturnOne(@"SELECT count(*) FROM FIN_COM_REALINVOICE t WHERE t.INVOICE_NO='" + txtInvoiceNo + "' and t.INVOICE_TYPE='" + invoiceKind + "'")) > 0)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("此电脑发票号已经在数据库中存在，请进行修改！"));
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return;
            }

            //检查下一张？？

            //更新电脑发票号，输入的是未使用的发票号
            //此处按照输入的发票号的上一张更新到数据库，作为已使用的发票

            string preInvoiceNo = (Neusoft.FrameWork.Function.NConvert.ToDecimal(txtInvoiceNo) - 1).ToString();

            if (this.feeInpatient.ExecNoQuery(@"UPDATE FIN_COM_INVOICE 
                                                SET USED_NO='{2}'
                                                WHERE GET_PERSON_CODE='{1}'
                                                AND INVOICE_KIND='{0}'", invoiceKind, Neusoft.FrameWork.Management.Connection.Operator.ID, preInvoiceNo) == -1)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("更新电脑发票号失败：" + this.feeInpatient.Err));
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                return;
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();
            this.FindForm().DialogResult = DialogResult.OK;
            //MessageBox.Show("更新收款员下一张实际发票号成功！");
            MessageBox.Show("更新收款员下一张发票号成功！");
            this.FindForm().Close();
            **/
        }
        string invoiceKind = "";
        public void SetInvoiceKind(string invoiceKind)
        {
            this.invoiceKind = invoiceKind;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.FindForm().DialogResult = DialogResult.Cancel;
            this.FindForm().Close();
        }
    }
}
