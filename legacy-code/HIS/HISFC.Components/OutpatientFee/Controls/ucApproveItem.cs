using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    public enum ColumnEnum
    {
        Checked,
        UseCode,
        ItemName,
        PackageName,
        ApproveInfo,
        Price,
        Qty,
        Unit,
        TotCost
    }

    public partial class ucApproveItem : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucApproveItem()
        {
            InitializeComponent();

            this.btnOk.Click += new EventHandler(btnOk_Click);
            this.btnExport.Click += new EventHandler(btnExport_Click);
            this.btnModifys.Click += new EventHandler(btnModifys_Click);
            this.fpFeeDetail.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(fpFeeDetail_CellDoubleClick);
            this.ckChecked.CheckedChanged += new EventHandler(ckChecked_CheckedChanged);
        }

        #region 变量

        private ArrayList feeDetails = new ArrayList();//费用信息
        private Neusoft.HISFC.Models.Registration.Register register = new Neusoft.HISFC.Models.Registration.Register();
        private string fileName = string.Empty;
        #endregion

        #region 属性
        /// <summary>
        /// feeDetails
        /// </summary>
        public ArrayList FeeDetails
        {
            get
            {
                return feeDetails;
            }
            set
            {
                feeDetails = value;
            }
        }

        public Neusoft.HISFC.Models.Registration.Register Register
        {
            get
            {
                return register;
            }
            set
            {
                register = value;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化费用列表
        /// </summary>
        public void InitFeeDetails()
        {
            if (this.feeDetails != null)
            {
                this.fpFeeDetail_Sheet1.RowCount=0;
                int i=this.fpFeeDetail_Sheet1.RowCount;

                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList in this.feeDetails)
                { 
                    string recipeNO = string.IsNullOrEmpty(feeItemList.UndrugComb.ID) ? feeItemList.RecipeNO : feeItemList.UndrugComb.Extend1;
                    int sequenceNO = string.IsNullOrEmpty(feeItemList.UndrugComb.ID) ? feeItemList.SequenceNO : Neusoft.FrameWork.Function.NConvert.ToInt32(feeItemList.UndrugComb.Extend2);
                    
                    this.fpFeeDetail_Sheet1.RowCount++; 
                    i=this.fpFeeDetail_Sheet1.RowCount-1;
                    this.fpFeeDetail_Sheet1.Rows[i].Tag = feeItemList;
                    this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Checked].Value = false;
                    if (string.IsNullOrEmpty(recipeNO))
                    {
                        this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Checked].Locked = true;
                        this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Checked].Note = "该项目没有划价保存，不允许批量修改！";
                        this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Checked].NoteStyle = FarPoint.Win.Spread.NoteStyle.PopupNote;
                    }
                    this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.UseCode].Text = feeItemList.Item.UserCode;
                    this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.ItemName].Text = feeItemList.Item.Name;
                    this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.PackageName].Text = feeItemList.UndrugComb.Name;
                    this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Qty].Text = feeItemList.Item.Qty.ToString();
                    this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Price].Text = feeItemList.Item.Price.ToString();
                    this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Unit].Text = feeItemList.Item.PriceUnit;
                    this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.TotCost].Text = feeItemList.FT.TotCost.ToString();

                    //根据主键找对应的审批记录
                    if (string.IsNullOrEmpty(recipeNO)==false)
                    {
                        BizLogic.ApproveItemBizLogic approveMgr = new BizLogic.ApproveItemBizLogic();
                        BizLogic.ApproveItemModel model = approveMgr.Get(recipeNO, sequenceNO);
                        if (model != null)
                        {
                            this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.UseCode].Tag = model;
                            this.fpFeeDetail_Sheet1.Rows[i].ForeColor = Color.Red;
                            this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.ApproveInfo].Text = model.ApplyType + "|" + model.HosOpinion + "|" + model.ApplyReason;
                            feeItemList.ItemRateFlag = "3";
                        }
                        else
                        {
                            feeItemList.ItemRateFlag = "";
                        }
                    }


                }
            }
        }

        #endregion

        #region 事件
        void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            //生成文件
            //查找当天的次数
            BizLogic.ApproveItemBizLogic approveMgr = new BizLogic.ApproveItemBizLogic();
            Neusoft.HISFC.BizLogic.Fee.Interface interfaceMgr = new Neusoft.HISFC.BizLogic.Fee.Interface();
            //查找所有未导出的项目
            List<BizLogic.ApproveItemModel> list = new List<BizLogic.ApproveItemModel>();   //approveMgr.Query(this.register.ID);
            for (int i = 0; i < this.fpFeeDetail_Sheet1.RowCount; i++)
            {
                if (this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.UseCode].Tag is BizLogic.ApproveItemModel)
                {
                    list.Add(this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.UseCode].Tag as BizLogic.ApproveItemModel);
                }
            }

            if (list != null)
            {
                if (list.Count > 0)
                {
                    //生成文件名称
                    //获取患者的医保信息
                    Neusoft.HISFC.Models.RADT.PatientInfo siPatientInfo = interfaceMgr.GetSIPersonInfo(this.register.ID);
                    if (siPatientInfo == null || string.IsNullOrEmpty(siPatientInfo.ID))
                    {
                        MessageBox.Show(this, string.Format("未找到患者：{0}  的医保记录", this.register.Name) + interfaceMgr.Err, "提示");
                        return;
                    }


                    DateTime dt = this.register.DoctorInfo.SeeDate;
                    int count = approveMgr.GetMaxExportCount(this.register.ID, dt);
                    count = count + 1;

                    dlg.FileName = this.register.Name + "_" + siPatientInfo.SIMainInfo.RegNo.Substring(0, 6) + "_" + dt.ToString("yyyyMMdd") + "_" + count.ToString().PadLeft(3, '0');
                    dlg.Filter = "(*.txt)|*.txt";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string txtInfo = string.Empty;
                        foreach (BizLogic.ApproveItemModel approveModel in list)
                        {
                            //就医登记号、医院编号、申请类别、医院项目编码、医院项目名称、中心项目编码、中心项目名称、申请数量、项目单价、用法、医院意见、申请理由、备注。
                            string importStr = "否";
                            string haveLocallyMaterailStr = "否";
                            string locallyPrice = string.Empty;
                            if (approveModel.ApplyType == "内置材料")
                            {
                                if (approveModel.ImportFlag)
                                {
                                    importStr = "是";
                                    if (approveModel.HaveLocallyMaterialFlag)
                                    {
                                        haveLocallyMaterailStr = "是";
                                        locallyPrice = approveModel.LocallyPrice.ToString("F2");
                                    }
                                }
                            }
                            txtInfo += approveModel.RegNO + "\t"
                                + approveModel.HosNO + "\t"
                                + approveModel.ApplyType + "\t"
                                + approveModel.Item.ID + "\t"
                                + approveModel.Item.Name + "\t"
                                + approveModel.Center.ID + "\t"
                                + approveModel.Center.Name + "\t"
                                + approveModel.Qty + "\t"
                                + approveModel.Price + "\t"
                                + approveModel.UseCode + "\t"
                                + importStr + "\t"
                                + haveLocallyMaterailStr + "\t"
                                + locallyPrice + "\t"
                                + approveModel.HosOpinion + "\t"
                                + approveModel.ApplyReason + "\t"
                                + approveModel.Memo + "\r\n";
                        }

                        //生成文件信息
                        System.IO.FileStream fileInfo = System.IO.File.Create(dlg.FileName);
                        Byte[] bts = System.Text.Encoding.Default.GetBytes(txtInfo);
                        fileInfo.Write(bts, 0, bts.Length);
                        fileInfo.Close();
                    }
                }
                else
                {
                    MessageBox.Show(this, string.Format("患者：{0}  没有需要导出的数据", this.register.Name) + interfaceMgr.Err, "提示");
                    return;
                }
            }
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        void fpFeeDetail_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.Row >= 0)
            {
                if (this.fpFeeDetail_Sheet1.Rows[e.Row].Tag is Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)
                {
                    string errorInfo=string.Empty;
                    Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = this.fpFeeDetail_Sheet1.Rows[e.Row].Tag as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                    ucApproveItemInfo ucApproveItemInfo = new ucApproveItemInfo();
                    BizLogic.ApproveItemModel model = null;
                    if (this.fpFeeDetail_Sheet1.Cells[e.Row, (int)ColumnEnum.UseCode].Tag is BizLogic.ApproveItemModel)
                    {
                        model = this.fpFeeDetail_Sheet1.Cells[e.Row, (int)ColumnEnum.UseCode].Tag as BizLogic.ApproveItemModel;
                    }
                    if (ucApproveItemInfo.SetInfo(this.register, feeItemList, model,ref errorInfo) <0)
                    {
                        MessageBox.Show(this, errorInfo, "提示");
                        return;
                    }
                    Neusoft.FrameWork.WinForms.Classes.Function.ShowControl(ucApproveItemInfo);
                    //特批了
                    if (ucApproveItemInfo.ApproveItemModel!=null)
                    {
                        this.fpFeeDetail_Sheet1.Rows[e.Row].ForeColor = Color.Red;
                        this.fpFeeDetail_Sheet1.Cells[e.Row, (int)ColumnEnum.UseCode].Tag = ucApproveItemInfo.ApproveItemModel;
                        this.fpFeeDetail_Sheet1.Cells[e.Row, (int)ColumnEnum.ApproveInfo].Text = ucApproveItemInfo.ApproveItemModel.ApplyType + "|" + ucApproveItemInfo.ApproveItemModel.HosOpinion + "|" + ucApproveItemInfo.ApproveItemModel.ApplyReason;
                    }
                    else
                    {
                        this.fpFeeDetail_Sheet1.Rows[e.Row].ForeColor = Color.Black;
                        this.fpFeeDetail_Sheet1.Cells[e.Row, (int)ColumnEnum.UseCode].Tag = null;
                        this.fpFeeDetail_Sheet1.Cells[e.Row, (int)ColumnEnum.ApproveInfo].Text = "";
                    }
                }
            }
        }

        void btnModifys_Click(object sender, EventArgs e)
        {
            List<Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList> list = new List<Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList>();   //approveMgr.Query(this.register.ID);
            Dictionary<string,BizLogic.ApproveItemModel> listModels = new Dictionary<string,BizLogic.ApproveItemModel>();  
            for (int i = 0; i < this.fpFeeDetail_Sheet1.RowCount; i++)
            {
                if (this.fpFeeDetail_Sheet1.Rows[i].Tag is Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)
                {
                    if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Checked].Value))
                    {

                        Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemListTemp = this.fpFeeDetail_Sheet1.Rows[i].Tag as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                        string recipeNO = string.IsNullOrEmpty(feeItemListTemp.UndrugComb.ID) ? feeItemListTemp.RecipeNO : feeItemListTemp.UndrugComb.Extend1;
                        int sequenceNO = string.IsNullOrEmpty(feeItemListTemp.UndrugComb.ID) ? feeItemListTemp.SequenceNO : Neusoft.FrameWork.Function.NConvert.ToInt32(feeItemListTemp.UndrugComb.Extend2);
                        list.Add(feeItemListTemp);

                        if (this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.UseCode].Tag is BizLogic.ApproveItemModel)
                        {
                            listModels.Add(recipeNO + sequenceNO, this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.UseCode].Tag as BizLogic.ApproveItemModel);
                        }
                    }
                }
            }

            if (list.Count > 0)
            {

                string errorInfo = string.Empty;
                ucApproveItemInfo ucApproveItemInfo = new ucApproveItemInfo();
                if (ucApproveItemInfo.SetInfo(this.register, list, listModels, ref errorInfo) < 0)
                {
                    MessageBox.Show(this, errorInfo, "提示");
                    return;
                }
                Neusoft.FrameWork.WinForms.Classes.Function.ShowControl(ucApproveItemInfo);

                //特批了
                for (int i = 0; i < this.fpFeeDetail_Sheet1.RowCount; i++)
                {
                    if (this.fpFeeDetail_Sheet1.Rows[i].Tag is Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)
                    {
                        if (Neusoft.FrameWork.Function.NConvert.ToBoolean(this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Checked].Value))
                        {
                            Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemListTemp = this.fpFeeDetail_Sheet1.Rows[i].Tag as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                            string recipeNO = string.IsNullOrEmpty(feeItemListTemp.UndrugComb.ID) ? feeItemListTemp.RecipeNO : feeItemListTemp.UndrugComb.Extend1;
                            int sequenceNO = string.IsNullOrEmpty(feeItemListTemp.UndrugComb.ID) ? feeItemListTemp.SequenceNO : Neusoft.FrameWork.Function.NConvert.ToInt32(feeItemListTemp.UndrugComb.Extend2);

                            if (ucApproveItemInfo.ApproveItemModels != null && ucApproveItemInfo.ApproveItemModels.ContainsKey(recipeNO+sequenceNO))
                            {
                                this.fpFeeDetail_Sheet1.Rows[i].ForeColor = Color.Red;
                                this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.UseCode].Tag = ucApproveItemInfo.ApproveItemModels[recipeNO + sequenceNO];
                                this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.ApproveInfo].Text = ucApproveItemInfo.ApproveItemModels[recipeNO + sequenceNO].ApplyType + "|" + ucApproveItemInfo.ApproveItemModels[recipeNO + sequenceNO].HosOpinion + "|" + ucApproveItemInfo.ApproveItemModels[recipeNO + sequenceNO].ApplyReason;
                            }
                            else
                            {
                                this.fpFeeDetail_Sheet1.Rows[i].ForeColor = Color.Black;
                                this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.UseCode].Tag = null ;
                                this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.ApproveInfo].Text = "";
                            }
                        }

                    }
                }
            }


        }

        void ckChecked_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.fpFeeDetail_Sheet1.RowCount; i++)
            {
                if (this.fpFeeDetail_Sheet1.Rows[i].Tag is Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)
                {
                    if (this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Checked].Locked==false)
                    {
                        this.fpFeeDetail_Sheet1.Cells[i, (int)ColumnEnum.Checked].Value = this.ckChecked.Checked;
                    }
                }
            }
        }
        #endregion
    }
}
