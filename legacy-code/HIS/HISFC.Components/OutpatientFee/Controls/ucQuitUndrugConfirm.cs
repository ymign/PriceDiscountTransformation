using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.Fee.Outpatient;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public partial class ucQuitUndrugConfirm : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucQuitUndrugConfirm()
        {
            InitializeComponent();
        }


        #region 变量
        /// <summary>
        /// 门诊费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();

        ArrayList FeeItemListSelect = new ArrayList();

        ArrayList FeeItemListALL = new ArrayList();
        /// <summary>
        /// 工具条
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();

        private string undrugItemType = string.Empty;
        /// <summary>
        /// 是否可以选择项目收费//{EE98C7B7-AC32-4b2c-93A5-9A62A33D6457}
        /// </summary>
        [Category("控件设置"), Description("费药品项目类别UL检验 UC检查")]
        public string UndrugItemType
        {
            get
            {
                return this.undrugItemType;
            }
            set
            {
                this.undrugItemType = value;
            }
        }

        #endregion

        #region 事件


        protected override Neusoft.FrameWork.WinForms.Forms.ToolBarService OnInit(object sender, object neuObject, object param)
        {
            this.FindForm().Text = "退费审核";

            toolBarService.AddToolButton("退费审核", "审核申请信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.B保存, true, false, null);
            toolBarService.AddToolButton("刷新", "重新刷新项目和退费申请信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.B帮助, true, false, null);
            toolBarService.AddToolButton("清空", "清除录入信息", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q清空, true, false, null);
            toolBarService.AddToolButton("全选", "全部退除所有费用", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q全退, true, false, null);
            toolBarService.AddToolButton("还原", "还原可退数量为0", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Q取消, true, false, null);
            return toolBarService;
        }

        /// <summary>
        /// 按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "退费审核":
                    this.Save();
                    break;
                case "还原":
                    Cancle();
                    break;
                case "清空":
                    Clear();
                    break;
                case "全选":
                    AllQuit();
                    break;
                default:
                    break;
            }

            base.ToolStrip_ItemClicked(sender, e);
        }


        //发票号回车事件
        private void tbInvoiceNO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                getItemList();
            }
        }

        //farpoint行双击事件
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.ColumnHeader) return;
            FeeItemList feeItem = this.fpSpread1_Sheet2.Rows[e.Row].Tag as FeeItemList;

            setListConfirm(feeItem);
        }

        //farpoint行双击事件
        private void fpSpread2_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.ColumnHeader) return;
            FeeItemList feeItem = this.fpSpread1_Sheet2.Rows[e.Row].Tag as FeeItemList;
            FeeItemListSelect.RemoveAt(FeeItemListSelect.IndexOf(feeItem));
            //FeeItemListSelect.Remove(feeItem);

            this.fpSpread2_Sheet2.Rows.Remove(e.Row, 1);
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取项目明细
        /// </summary>
        private void getItemList()
        {
            #region MyRegion
            ArrayList balances = new ArrayList();//结算信息实体数组
            balances = outpatientManager.QueryBalancesByInvoiceNO(tbInvoiceNO.Text.Trim());
            if (balances.Count<0)
            {
                MessageBox.Show("发票号不存在，请重新录入");
                return;
            }

            //初始化患者信息界面
            setPatientValue(balances);

            ArrayList ItemList = outpatientManager.QueryFeeItemListsByClinicNO((balances[0] as Balance).Patient.ID);

            if (ItemList == null)
            {
                MessageBox.Show("没有已缴费的费用明细");
                return;
            }
            //初始化非药品费用明细表
            setUndrugItemList(ItemList);
            #endregion
        }

        /// <summary>
        /// 初始化患者信息界面
        /// </summary>
        /// <param name="balances">结算信息</param>
        private void setPatientValue(ArrayList balances)
        {
            #region MyRegion
            Balance balance = balances[0] as Balance;//结算信息实体
            tbCardNo.Text = balance.Patient.PID.CardNO;
            tbName.Text = balance.Patient.Name;
            tbPactName.Text = balance.Patient.Pact.Name;
            tbTotCost.Text = balance.FT.TotCost.ToString("0.00");
            tbPubCost.Text = balance.FT.PubCost.ToString("0.00");
            tbOwnCost.Text = balance.FT.OwnCost.ToString("0.00");
            tbPayCost.Text = balance.FT.PayCost.ToString("0.00");
            chkGoupAllQuit.Visible = false;
            tbQuitCost.Visible = false;
            lbQuitCash.Visible = false;
            lbLeftCost.Visible = false;
            lbReturnCost.Visible = false;
            tbReturnCost.Visible = false;
            tbQuitCash.Visible = false;
            lbQuitCash.Visible = false; 
            #endregion
        }

        /// <summary>
        /// 初始化非药品费用明细表
        /// </summary>
        /// <param name="ItemList"></param>
        private void setUndrugItemList(ArrayList ItemList)
        {
            #region MyRegion
            this.fpSpread1_Sheet2.RowCount = 0;
            this.fpSpread2_Sheet2.RowCount = 0;
            int index = 0;
            foreach (FeeItemList feeItem in ItemList)
            {
                if (feeItem.Item.ItemType == EnumItemType.UnDrug)//非药品且是检验类但不包含病理
                {
                    if (!string.IsNullOrEmpty(undrugItemType)) //维护之后以维护的为准
                    {

                        if (undrugItemType.Contains(feeItem.Item.SysClass.ID.ToString()) || undrugItemType.Contains(feeItem.Item.ID))
                        {

                            if (feeItem.Item.SysClass.ID.ToString() != "UL" || feeItem.ExecOper.Dept.ID != "7002")
                            {
                                fpSpread1_Sheet2.Rows.Add(index, 1);
                                fpSpread1_Sheet2.Rows[index].Tag = feeItem;
                                this.fpSpread1_Sheet2.Cells[index, 0].Text = feeItem.Item.Name;
                                this.fpSpread1_Sheet2.Cells[index, 1].Text = feeItem.Order.ID;
                                this.fpSpread1_Sheet2.Cells[index, 3].Text = feeItem.Item.Qty.ToString();
                                this.fpSpread1_Sheet2.Cells[index, 4].Text = feeItem.Item.PriceUnit;
                                this.fpSpread1_Sheet2.Cells[index, 5].Text = feeItem.NoBackQty.ToString();
                                this.fpSpread1_Sheet2.Cells[index, 6].Text = feeItem.FT.OwnCost.ToString("0.00");
                                this.fpSpread1_Sheet2.Cells[index, 7].Text = feeItem.UndrugComb.Name;
                                index++;
                                FeeItemListALL.Add(feeItem);
                            }
                        }
                    }
                    else //不维护的，默认以前规则，只显示病理意外的检验项目
                    {
                        if (feeItem.Item.SysClass.ID.ToString() == "UL" && feeItem.ExecOper.Dept.ID != "7002")
                        {
                            fpSpread1_Sheet2.Rows.Add(index, 1);
                            fpSpread1_Sheet2.Rows[index].Tag = feeItem;
                            this.fpSpread1_Sheet2.Cells[index, 0].Text = feeItem.Item.Name;
                            this.fpSpread1_Sheet2.Cells[index, 1].Text = feeItem.Order.ID;
                            this.fpSpread1_Sheet2.Cells[index, 3].Text = feeItem.Item.Qty.ToString();
                            this.fpSpread1_Sheet2.Cells[index, 4].Text = feeItem.Item.PriceUnit;
                            this.fpSpread1_Sheet2.Cells[index, 5].Text = feeItem.NoBackQty.ToString();
                            this.fpSpread1_Sheet2.Cells[index, 6].Text = feeItem.FT.OwnCost.ToString("0.00");
                            this.fpSpread1_Sheet2.Cells[index, 7].Text = feeItem.UndrugComb.Name;
                            index++;
                            FeeItemListALL.Add(feeItem);
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 初始化确认表
        /// </summary>
        /// <param name="feeItem"></param>
        private void setListConfirm(FeeItemList feeItem)
        {
            #region MyRegion
            if (FeeItemListSelect.Contains(feeItem))
            {
                return;
            }

            FeeItemListSelect.Add(feeItem);

            this.fpSpread2_Sheet2.Rows.Add(this.fpSpread2_Sheet2.Rows.Count, 1);
            this.fpSpread2_Sheet2.Rows[this.fpSpread2_Sheet2.Rows.Count - 1].Tag = feeItem;
            this.fpSpread2_Sheet2.Cells[this.fpSpread2_Sheet2.Rows.Count - 1, 0].Text = feeItem.Item.Name;
            this.fpSpread2_Sheet2.Cells[this.fpSpread2_Sheet2.Rows.Count - 1, 1].Text = feeItem.Item.Qty.ToString();
            this.fpSpread2_Sheet2.Cells[this.fpSpread2_Sheet2.Rows.Count - 1, 2].Text = feeItem.Item.PriceUnit;
            //this.fpSpread2_Sheet2.Cells[this.fpSpread2_Sheet2.Rows.Count-1, 5].Text = feeItem.NoBackQty.ToString();
            //this.fpSpread2_Sheet2.Cells[this.fpSpread2_Sheet2.Rows.Count-1, 6].Text = feeItem.FT.OwnCost.ToString("0.00");
            //this.fpSpread2_Sheet2.Cells[this.fpSpread2_Sheet2.Rows.Count-1, 7].Text = feeItem.UndrugComb.Name; 
            #endregion
        }

        //全退
        private void AllQuit()
        {
            foreach (FeeItemList feeItem in FeeItemListALL)
            {
                setListConfirm(feeItem);
            }
        }

        //保存
        private void Save()
        {
            if (FeeItemListSelect.Count>0)
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

                foreach (FeeItemList feeItem in FeeItemListSelect)
                {
                    if (outpatientManager.UpdateUndrugItemNoBackQty(feeItem)<0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("保存失败");
                        return;
                    };
                }

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                MessageBox.Show("保存成功！");
                this.Clear();
            }
        }

        //还原
        private void Cancle()
        {
            if (FeeItemListSelect.Count > 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

                foreach (FeeItemList feeItem in FeeItemListSelect)
                {
                    if (outpatientManager.UpdateUndrugItemNoBackNum(feeItem) < 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("还原失败");
                        return;
                    };
                }

                Neusoft.FrameWork.Management.PublicTrans.Commit();
                MessageBox.Show("还原成功！");
                this.Clear();
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        private void Clear()
        {
            #region MyRegion
            tbInvoiceNO.Text = "";
            tbCardNo.Text = "";
            tbName.Text = "";
            tbPactName.Text = "";
            tbTotCost.Text = "";
            tbPubCost.Text = "";
            tbOwnCost.Text = "";
            tbPayCost.Text = "";

            this.fpSpread1_Sheet2.RowCount = 0;
            this.fpSpread2_Sheet2.RowCount = 0;

            FeeItemListSelect.Clear();
            FeeItemListALL.Clear();
            #endregion
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public override void Refresh()
        {
            Clear();
            getItemList();
        }

        #endregion

    }
}
