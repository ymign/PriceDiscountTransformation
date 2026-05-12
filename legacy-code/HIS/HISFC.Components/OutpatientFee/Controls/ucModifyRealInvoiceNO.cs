using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    /// <summary>
    /// 修改实际发票号.gmz
    /// </summary>
    public partial class ucModifyRealInvoiceNO : Neusoft.FrameWork.WinForms.Controls.ucBaseControl 
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ucModifyRealInvoiceNO()
        {
            InitializeComponent();
        }

        #region 变量

        Neusoft.HISFC.BizLogic.Fee.InPatient inpatientFeeManager = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        Neusoft.HISFC.BizLogic.Fee.InvoiceServiceNoEnum invoiceServiceNoEnumManager = new Neusoft.HISFC.BizLogic.Fee.InvoiceServiceNoEnum();
        Neusoft.HISFC.BizLogic.Fee.RealInvoice invoiceManager = new Neusoft.HISFC.BizLogic.Fee.RealInvoice();

        #endregion



        #region 方法

        /// <summary>
        /// 加载函数
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.init();
            base.OnLoad(e);
        }

        private void init()
        {
            if (this.AddInvoiceType() == -1)
            {
                return;
            }
        }


        /// <summary>
        /// 添加收据类型
        /// </summary>
        /// <returns></returns>
        protected virtual int AddInvoiceType()
        {

            Neusoft.FrameWork.Models.NeuObject myObj = this.GetFinGroupID(this.inpatientFeeManager.Operator.ID);
            if (myObj == null)
            {
                return -1;
            }

            string finGroupID = myObj.ID;

            System.Collections.ArrayList alInvoiceType = new System.Collections.ArrayList();
            alInvoiceType = this.invoiceServiceNoEnumManager.GetInvoiceTypeByOperIDORFinGroupID(this.inpatientFeeManager.Operator.ID, finGroupID);

            if (alInvoiceType == null)
            {
                MessageBox.Show("查询操作员的发票类型出错" + this.inpatientFeeManager.Err);
                return -1;
            }

            this.cmbInvoiceType.AddItems(alInvoiceType);
            return 1;

        }


        /// <summary>
        /// 查询操作员所在的财务组
        /// </summary>
        /// <param name="operID"></param>
        /// <returns></returns>
        private Neusoft.FrameWork.Models.NeuObject GetFinGroupID(string operID)
        {
            Neusoft.FrameWork.Models.NeuObject myObj = this.inpatientFeeManager.GetFinGroupInfoByOperCode(operID);

            if (myObj == null)
            {
                MessageBox.Show("查询操作员所在的财务组失败" + this.inpatientFeeManager.Err);
            }

            return myObj;

        }

        /// <summary>
        /// 设置发票信息显示
        /// </summary>
        private void SetList()
        {
            Neusoft.HISFC.Models.Fee.InvoiceExtend riBegin = (this.txtOldBeginNO.Tag as Neusoft.HISFC.Models.Fee.InvoiceExtend).Clone();
            Neusoft.HISFC.Models.Fee.InvoiceExtend riEnd = (this.txtOldEndNO.Tag as Neusoft.HISFC.Models.Fee.InvoiceExtend).Clone();

            List<Neusoft.HISFC.Models.Fee.InvoiceExtend> list = this.invoiceManager.QueryByBeginEndID(riBegin.ID, riEnd.ID, this.cmbInvoiceType.Tag.ToString());

            if (list == null)
            {
                MessageBox.Show("查找需要修改的发票信息出错" + this.invoiceManager.Err);
                return;
            }

            //清空
            this.neuSpread1_Sheet1.Rows.Count = 0;
            FarPoint.Win.Spread.CellType.TextCellType txtType = new FarPoint.Win.Spread.CellType.TextCellType();

            for (int i = 0; i < list.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.InvoiceExtend r = list[i] as Neusoft.HISFC.Models.Fee.InvoiceExtend;
                this.neuSpread1_Sheet1.Rows.Add(i, 1);

                this.neuSpread1_Sheet1.Cells[i, 0].CellType = txtType;
                this.neuSpread1_Sheet1.Cells[i, 0].Text = r.ID;
                this.neuSpread1_Sheet1.Cells[i, 1].CellType = txtType;
                this.neuSpread1_Sheet1.Cells[i, 1].Text = r.RealInvoiceNo;

                if (i > 0 && (Neusoft.FrameWork.Function.NConvert.ToInt32(r.RealInvoiceNo) - 1) != Neusoft.FrameWork.Function.NConvert.ToInt32(this.neuSpread1_Sheet1.Cells[i - 1, 1].Text))
                {
                    this.neuSpread1_Sheet1.Rows[i].BackColor = Color.Red;
                }

            }

        }

        /// <summary>
        /// 判断是否有效
        /// </summary>
        /// <returns></returns>
        private bool Valid()
        {

            if (this.cmbInvoiceType.Tag == null)
            {
                MessageBox.Show("请选择数据类型!");
                this.cmbInvoiceType.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(this.txtOldBeginNO.Text.Trim()))
            {
                MessageBox.Show("请输入起始号，按回车!");
                this.txtBeginInvoice.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(this.txtOldEndNO.Text.Trim()))
            {
                MessageBox.Show("请输入终止号，按回车!");
                this.txtEndInvoice.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(this.txtNewBeginNO.Text.Trim()))
            {
                MessageBox.Show("请输入新的起始号!");
                this.txtNewBeginNO.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(this.txtNewEndNO.Text.Trim()))
            {
                MessageBox.Show("请输入新的终止号!");
                this.txtNewEndNO.Focus();
                return false;
            }

            if (this.txtNewBeginNO.Text.Trim().Length != 8 || this.txtNewEndNO.Text.Trim().Length != 8)
            {
                MessageBox.Show("发票印刷号必须是8位数字!");
                this.txtNewBeginNO.Focus();
                return false;
            }

            try
            {
                decimal start = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtNewBeginNO.Text.Trim());
                decimal end = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtNewEndNO.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("发票印刷号中不需要输入字符! "+ex.Message);
                this.txtNewBeginNO.Focus();
                return false;
            }

            return true;

        }


        /// <summary>
        /// 清空控件的值
        /// </summary>
        private void Clear()
        {
            this.txtBeginInvoice.Text = "";
            this.txtEndInvoice.Text = "";
            this.txtOldBeginNO.Text = "";
            this.txtOldBeginNO.Tag = null;
            this.txtOldEndNO.Tag = null;
            this.txtOldEndNO.Text = "";
            this.txtNewBeginNO.Text = "";
            this.txtNewEndNO.Text = "";
        }



        #endregion

        private void cmbInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtBeginInvoice.Focus();
        }

        /// <summary>
        /// 开始号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBeginInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (this.cmbInvoiceType.Tag == null)
                {
                    MessageBox.Show("请选择数据类型!");
                    return;
                }

                if (this.txtBeginInvoice.Text.Trim().Length < 1)
                {
                    MessageBox.Show("请输入起始发票号!");
                    return;
                }

                Neusoft.HISFC.Models.Fee.InvoiceExtend oldBeginNO = this.QueryRealNO(this.txtBeginInvoice.Text, cmbInvoiceType.Tag.ToString());

                if (oldBeginNO == null)
                {
                    return;
                }

                if (oldBeginNO.Oper.ID != this.invoiceManager.Operator.ID)
                {
                    MessageBox.Show(oldBeginNO.ID + " 不是属于当前操作员,不允许修改其他人的发票号!");
                    return;
                }

                this.txtOldBeginNO.Text = oldBeginNO.RealInvoiceNo;
                this.txtOldBeginNO.Tag = oldBeginNO;
                this.txtNewBeginNO.Focus();

            }
        }

        /// <summary>
        /// 结束号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEndInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (this.cmbInvoiceType.Tag == null)
                {
                    MessageBox.Show("请选择数据类型!");
                    return;
                }

                if (this.txtEndInvoice.Text.Trim().Length < 1)
                {
                    MessageBox.Show("请输入终止发票!");
                    return;
                }

                Neusoft.HISFC.Models.Fee.InvoiceExtend oldEndNO = this.QueryRealNO(this.txtEndInvoice.Text, cmbInvoiceType.Tag.ToString());
                if (oldEndNO == null)
                {
                    return;
                }

                if (oldEndNO.Oper.ID != this.invoiceManager.Operator.ID)
                {
                    MessageBox.Show(oldEndNO.ID + " 不是属于当前操作员,不允许修改其他人的发票号!");
                    return;
                }

                this.txtOldEndNO.Text = oldEndNO.RealInvoiceNo;
                this.txtOldEndNO.Tag = oldEndNO;
                this.txtNewEndNO.Focus();

                this.SetList();
            }
        }

        private Neusoft.HISFC.Models.Fee.InvoiceExtend QueryRealNO(string invoiceNO, string invoiceType)
        {
            //if (!string.IsNullOrEmpty(invoiceNO.Trim()) && invoiceNO.Length < 12)
            //{
            //    invoiceNO = invoiceNO.PadLeft(12, '0');
            //}

            Neusoft.HISFC.Models.Fee.InvoiceExtend realInvoice = invoiceManager.GetRealInvoice(invoiceNO, invoiceType);

            if (realInvoice == null)
            {
                MessageBox.Show("查找发票对照记录失败" + this.invoiceManager.Err);
                return null;
            }

            return realInvoice;

        }

        private void txtNewBeginNO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                this.txtEndInvoice.Focus();
            }
        }

        private void txtNewEndNO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                this.btConfirm.Focus();
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtOldBeginNO.Text))
            {
                if (MessageBox.Show("确认关闭窗口?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }
            }

            this.FindForm().Close();
        }

        private void btConfirm_Click(object sender, EventArgs e)
        {
            if (Valid() == false)
            {
                return;
            }

            DateTime dtNow = this.inpatientFeeManager.GetDateTimeFromSysDateTime();

            Neusoft.HISFC.Models.Fee.InvoiceExtend riBegin = (this.txtOldBeginNO.Tag as Neusoft.HISFC.Models.Fee.InvoiceExtend).Clone();
            Neusoft.HISFC.Models.Fee.InvoiceExtend riEnd = (this.txtOldEndNO.Tag as Neusoft.HISFC.Models.Fee.InvoiceExtend).Clone();

            //修改数量
            int count = 0;
            decimal startNO = 0m;
            decimal endNO = 0m;

            try
            {
                startNO = Neusoft.FrameWork.Function.NConvert.ToDecimal(riBegin.ID);
                endNO = Neusoft.FrameWork.Function.NConvert.ToDecimal(riEnd.ID);

                count = Neusoft.FrameWork.Function.NConvert.ToInt32(endNO - startNO) + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            List<Neusoft.HISFC.Models.Fee.InvoiceExtend> list = invoiceManager.QueryByBeginEndID(riBegin.ID, riEnd.ID, this.cmbInvoiceType.Tag.ToString());

            if (list == null)
            {
                MessageBox.Show("查找需要修改的发票信息出错" + invoiceManager.Err);
                return;
            }
            else if (list.Count != count)
            {
                MessageBox.Show("查询到的发票数量跟需要修改的数量不符, 请重新设置修改区间");
                return;
            }

            //印刷号中的数字个数
            int digitalNum = this.txtNewBeginNO.Text.Length;


            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            // 从后开始调整
            Neusoft.HISFC.Models.Fee.InvoiceExtend invo = null;
            Neusoft.HISFC.Models.Fee.InvoiceExtend invoBak = null;
            for (int idx = count - 1; idx >= 0; idx--)
            {
                invo = list[idx];
                invoBak = invo.Clone();

                if (invo.Oper.ID != invoiceManager.Operator.ID)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("发票号:" + invo.ID + ",印刷号:" + invo.RealInvoiceNo + " 不是属于当前操作员，不允许修改!");

                    return;
                }

                invo.Oper.ID = this.invoiceManager.Operator.ID;
                invo.Oper.OperTime = dtNow;

                string invoTemp = this.txtNewBeginNO.Text.Trim();
                long invoNO = long.Parse(invoTemp) + idx;
                invo.RealInvoiceNo = invoNO.ToString().PadLeft(digitalNum, '0');

                Neusoft.HISFC.Models.Fee.InvoiceExtend validInvoice = this.invoiceManager.GetInoviceByRealInvoice(invo.RealInvoiceNo, this.cmbInvoiceType.Tag.ToString());

                if (validInvoice != null)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新失败! " + validInvoice.RealInvoiceNo + "已经使用，不能更新!");
                    return;
                }

                //更新fin_com_realinvoice表,realInvoice的位数8位
                if (invoiceManager.UpdateRealInvoice(invo) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新失败! " + this.invoiceManager.Err);
                    return;
                }

                if (this.cmbInvoiceType.Tag.ToString() == "C")
                {
                    //更新fin_opb_invoiceinfo表，realInvoice的位数有12位
                    if (invoiceManager.UpdateInvoiceInfoRealInvoice(invo.ID, invo.RealInvoiceNo) < 1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("更新失败! " + this.invoiceManager.Err);
                        return;
                    }
                }
                else if (this.cmbInvoiceType.Tag.ToString() == "R")
                {
                    //更新FIN_OPB_ACCOUNTCARDFEE表，realInvoice的位数有12位
                    if (invoiceManager.UpdateInvoiceInfoRealInvoiceForAccountCardFee(invo.ID, invo.RealInvoiceNo) < 1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("更新失败!" + this.invoiceManager.Err);
                        return;
                    }
                }
                else
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("目前只是支持门诊发票号和挂号发票的修改!");
                    return;
                }
            }






            //long index = -1; //用来控制每次印刷号往后+1
            //foreach (Neusoft.HISFC.Models.Fee.InvoiceExtend invo in list)
            //{
            //    index++;
            //    Neusoft.HISFC.Models.Fee.InvoiceExtend invoBak = invo.Clone();

            //    if (invo.Oper.ID != invoiceManager.Operator.ID)
            //    {
            //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //        MessageBox.Show("发票号:" + invo.ID + ",印刷号:" + invo.RealInvoiceNo + " 不是属于当前操作员，不允许修改!");
                    
            //        return;
            //    }

            //    invo.Oper.ID = this.invoiceManager.Operator.ID;
            //    invo.Oper.OperTime = dtNow;

            //    string invoTemp = this.txtNewBeginNO.Text.Trim();
            //    long invoNO = long.Parse(invoTemp) + index;
            //    invo.RealInvoiceNo = invoNO.ToString().PadLeft(digitalNum, '0');

            //    Neusoft.HISFC.Models.Fee.InvoiceExtend validInvoice = this.invoiceManager.GetInoviceByRealInvoice(invo.RealInvoiceNo, this.cmbInvoiceType.Tag.ToString());

            //    if (validInvoice != null)
            //    {
            //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //        MessageBox.Show("更新失败! " + validInvoice.RealInvoiceNo + "已经使用，不能更新!");
            //        return;
            //    }

            //    //更新fin_com_realinvoice表,realInvoice的位数8位
            //    if (invoiceManager.UpdateRealInvoice(invo) == -1)
            //    {
            //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //        MessageBox.Show("更新失败! " + this.invoiceManager.Err);
            //        return;
            //    }

            //    if (this.cmbInvoiceType.Tag.ToString() == "C")
            //    {
            //        //更新fin_opb_invoiceinfo表，realInvoice的位数有12位
            //        if (invoiceManager.UpdateInvoiceInfoRealInvoice(invo.ID, invo.RealInvoiceNo) < 1)
            //        {
            //            Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //            MessageBox.Show("更新失败! " + this.invoiceManager.Err);
            //            return;
            //        }
            //    }
            //    else if (this.cmbInvoiceType.Tag.ToString() == "R")
            //    {
            //        //更新FIN_OPB_ACCOUNTCARDFEE表，realInvoice的位数有12位
            //        if (invoiceManager.UpdateInvoiceInfoRealInvoiceForAccountCardFee(invo.ID, invo.RealInvoiceNo) < 1)
            //        {
            //            Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //            MessageBox.Show("更新失败!" + this.invoiceManager.Err);
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //        MessageBox.Show("目前只是支持门诊发票号和挂号发票的修改!");
            //        return;
            //    }

            //}

            //提交
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show("全部更新成功!");

            this.Clear();

        }

        private void neuButton1_Click(object sender, EventArgs e)
        {
            if (Valid() == false)
            {
                return;
            }

            DateTime dtNow = this.inpatientFeeManager.GetDateTimeFromSysDateTime();

            Neusoft.HISFC.Models.Fee.InvoiceExtend riBegin = (this.txtOldBeginNO.Tag as Neusoft.HISFC.Models.Fee.InvoiceExtend).Clone();
            Neusoft.HISFC.Models.Fee.InvoiceExtend riEnd = (this.txtOldEndNO.Tag as Neusoft.HISFC.Models.Fee.InvoiceExtend).Clone();

            //修改数量
            int count = 0;
            decimal startNO = 0m;
            decimal endNO = 0m;

            try
            {
                startNO = Neusoft.FrameWork.Function.NConvert.ToDecimal(riBegin.ID);
                endNO = Neusoft.FrameWork.Function.NConvert.ToDecimal(riEnd.ID);

                count = Neusoft.FrameWork.Function.NConvert.ToInt32(endNO - startNO) + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            List<Neusoft.HISFC.Models.Fee.InvoiceExtend> list = invoiceManager.QueryByBeginEndID(riBegin.ID, riEnd.ID, this.cmbInvoiceType.Tag.ToString());

            if (list == null)
            {
                MessageBox.Show("查找需要修改的发票信息出错" + invoiceManager.Err);
                return;
            }
            else if (list.Count != count)
            {
                MessageBox.Show("查询到的发票数量跟需要修改的数量不符, 请重新设置修改区间");
                return;
            }

            //印刷号中的数字个数
            int digitalNum = this.txtNewBeginNO.Text.Length;

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            long index = -1; //用来控制每次印刷号往后+1
            foreach (Neusoft.HISFC.Models.Fee.InvoiceExtend invo in list)
            {
                index++;
                Neusoft.HISFC.Models.Fee.InvoiceExtend invoBak = invo.Clone();

                if (invo.Oper.ID != invoiceManager.Operator.ID)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("发票号:" + invo.ID + ",印刷号:" + invo.RealInvoiceNo + " 不是属于当前操作员，不允许修改!");

                    return;
                }

                invo.Oper.ID = this.invoiceManager.Operator.ID;
                invo.Oper.OperTime = dtNow;

                string invoTemp = this.txtNewBeginNO.Text.Trim();
                long invoNO = long.Parse(invoTemp) + index;
                invo.RealInvoiceNo = invoNO.ToString().PadLeft(digitalNum, '0');

                Neusoft.HISFC.Models.Fee.InvoiceExtend validInvoice = this.invoiceManager.GetInoviceByRealInvoice(invo.RealInvoiceNo, this.cmbInvoiceType.Tag.ToString());

                if (validInvoice != null)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新失败! " + validInvoice.RealInvoiceNo + "已经使用，不能更新!");
                    return;
                }

                //更新fin_com_realinvoice表,realInvoice的位数8位
                if (invoiceManager.UpdateRealInvoice(invo) == -1)
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("更新失败! " + this.invoiceManager.Err);
                    return;
                }

                if (this.cmbInvoiceType.Tag.ToString() == "C")
                {
                    //更新fin_opb_invoiceinfo表，realInvoice的位数有12位
                    if (invoiceManager.UpdateInvoiceInfoRealInvoice(invo.ID, invo.RealInvoiceNo) < 1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("更新失败! " + this.invoiceManager.Err);
                        return;
                    }
                }
                else if (this.cmbInvoiceType.Tag.ToString() == "R")
                {
                    //更新FIN_OPB_ACCOUNTCARDFEE表，realInvoice的位数有12位
                    if (invoiceManager.UpdateInvoiceInfoRealInvoiceForAccountCardFee(invo.ID, invo.RealInvoiceNo) < 1)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("更新失败!" + this.invoiceManager.Err);
                        return;
                    }
                }
                else
                {
                    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                    MessageBox.Show("目前只是支持门诊发票号和挂号发票的修改!");
                    return;
                }

            }

            //提交
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show("全部更新成功!");

            this.Clear();

        }



        


    }
}
