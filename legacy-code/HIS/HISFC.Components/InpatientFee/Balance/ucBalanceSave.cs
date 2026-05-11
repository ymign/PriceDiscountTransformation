using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Neusoft.FrameWork.WinForms.Forms;

namespace Neusoft.HISFC.Components.InpatientFee.Balance
{
    public partial class ucBalanceSave : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucBalanceSave()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime beginTime = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime endTime = DateTime.Now;

        /// <summary>
        /// 住院收费业务层
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.InPatient inpatientFeeManager = new Neusoft.HISFC.BizLogic.Fee.InPatient();

        /// <summary>
        /// 住院入出转业务层
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.RADT radtIntegrate = new Neusoft.HISFC.BizProcess.Integrate.RADT();

        /// <summary>
        /// toolBar
        /// </summary>
        ToolBarService toolBarService = new ToolBarService();

        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("平账", "平账结算患者费用", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.B保存, true, false, null);
            toolBarService.AddToolButton("取消", "取消平账操作", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q取消, true, false, null);

            return this.toolBarService;
        }

        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "平账") 
            {
                this.Save(false);
            }
            if (e.ClickedItem.Text == "取消")
            {
                this.Save(true);
            }
            
            base.ToolStrip_ItemClicked(sender, e);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <returns></returns>
        protected override int OnQuery(object sender, object neuObject)
        {
            beginTime = this.dtpBegin.Value;
            endTime = this.dtpEnd.Value;

            this.spDetail_Sheet1.RowCount = 0;
            this.spDetail_Sheet1.RowCount = 10;

            this.spQuery_Sheet1.RowCount = 0;
            this.spQuery_Sheet1.RowCount = 10;

            //查询未处理的 欠费记录
            ArrayList detail = this.inpatientFeeManager.QueryBalancesBySaveTime(this.beginTime, this.endTime, "0");
            if (detail == null) 
            {
                MessageBox.Show("查询未处理欠费记录出错!" + this.inpatientFeeManager.Err);

                return -1;
            }

            this.SetValue(this.spDetail_Sheet1, detail);

            //查询已经处理的 欠费记录
            ArrayList query = this.inpatientFeeManager.QueryBalancesBySaveTime(this.beginTime, this.endTime, "1");
            if (query == null)
            {
                MessageBox.Show("查询已经处理欠费记录出错!" + this.inpatientFeeManager.Err);

                return -1;
            }

            this.SetValue(this.spQuery_Sheet1, query);
            
            return base.OnQuery(sender, neuObject);
        }

        /// <summary>
        /// 保存结果
        /// </summary>
        /// <param name="isCancel">是否是取消操作</param>
        /// <returns>成功 1 失败 -1</returns>
        private int Save(bool isCancel) 
        {
            if (!isCancel)//平账 
            {
                if (this.neuTabControl1.SelectedTab != this.tpDetail)
                {
                    MessageBox.Show("请选择未处理记录Tab页,并选择相应记录!");

                    return -1;
                }
            }
            else 
            {
                if (this.neuTabControl1.SelectedTab != this.tpQuery)
                {
                    MessageBox.Show("请选择已处理记录Tab页,并选择相应记录!");

                    return -1;
                }
            }

            TabPage tpSelected = this.neuTabControl1.SelectedTab;

            FarPoint.Win.Spread.SheetView svSelected = (tpSelected.Controls[0] as Neusoft.FrameWork.WinForms.Controls.NeuSpread).Sheets[0];

            if (svSelected.RowCount == 0) 
            {
                return -1;
            }
            int rowSelected = svSelected.ActiveRowIndex;
            if (svSelected.Rows[rowSelected].Tag == null) return -1;

            DialogResult result = MessageBox.Show("是否处理当前选择患者?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.No) 
            {
                return -1;
            }

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            this.inpatientFeeManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            

            Neusoft.HISFC.Models.Fee.Inpatient.Balance balance = svSelected.Rows[rowSelected].Tag as Neusoft.HISFC.Models.Fee.Inpatient.Balance;

            //更新处理结果
            int returnValue = this.inpatientFeeManager.UpdateBalanceSaveInfo(balance,Neusoft.FrameWork.Function.NConvert.ToInt32(!isCancel).ToString());
            if (returnValue <= 0) 
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();

                MessageBox.Show("处理失败!" + this.inpatientFeeManager.Err);

                return -1;
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show(isCancel ? "取消平账记录成功!" : "平账成功!");

            //重新检索记录
            this.OnQuery(this, new Neusoft.FrameWork.Models.NeuObject());

            return 1;
        }

        /// <summary>
        /// 显示值
        /// </summary>
        /// <param name="sv">当前Sheet页</param>
        /// <param name="balances">结算信息</param>
        private void SetValue(FarPoint.Win.Spread.SheetView sv, ArrayList balances) 
        {
            if (balances == null || balances.Count == 0)
            {
                return; 
            }

            sv.RowCount = balances.Count;

            for (int i = 0; i < balances.Count; i++) 
            {
                Neusoft.HISFC.Models.Fee.Inpatient.Balance b = balances[i] as Neusoft.HISFC.Models.Fee.Inpatient.Balance;
                Neusoft.HISFC.Models.RADT.PatientInfo patient = this.radtIntegrate.GetPatientInfomation(b.Patient.ID);
                if (patient == null)
                {
                    MessageBox.Show("获得患者基本信息出错!" + this.radtIntegrate.Err);

                    return;
                }
                sv.Cells[i, 0].Text = patient.PID.PatientNO;
                sv.Cells[i, 1].Text = patient.Name;
                sv.Cells[i, 2].Text = patient.PVisit.PatientLocation.Dept.Name;

                string balanceTypeName = string.Empty;
                if (b.BalanceType.ID.ToString() == "Q") 
                {
                    balanceTypeName = "欠费";
                }
                sv.Cells[i, 3].Text = balanceTypeName;
                sv.Cells[i, 4].Text = b.FT.TotCost.ToString();
                sv.Cells[i, 5].Text = b.FT.SupplyCost > 0 ? "-"+b.FT.SupplyCost.ToString() :b.FT.ReturnCost.ToString();
                sv.Cells[i, 6].Text = b.BalanceOper.OperTime.ToString();
                sv.Rows[i].Tag = b.Clone();
            }
        }

        #endregion

        #region 事件

        #endregion
    }
}
