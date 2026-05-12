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
    public partial class ucApproveItemInfo : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucApproveItemInfo()
        {
            InitializeComponent();
            this.btnConfrimApprove.Click += new EventHandler(btnConfrimApprove_Click);
            this.btnCancelApprove.Click += new EventHandler(btnCancelApprove_Click);
            this.cmbApplyType.SelectedIndexChanged += new EventHandler(cmbApplyType_SelectedIndexChanged);
            this.ckImport.CheckedChanged += new EventHandler(ckImport_CheckedChanged);
            this.ckHaveLocallyMaterial.CheckedChanged += new EventHandler(ckHaveLocallyMaterial_CheckedChanged);
        }

        void ckHaveLocallyMaterial_CheckedChanged(object sender, EventArgs e)
        {
            this.txtLocallyPrice.Enabled = this.ckHaveLocallyMaterial.Checked;
            if (this.txtLocallyPrice.Enabled == false)
            {
                this.txtLocallyPrice.Text = "0.00";
            }
        }

        void ckImport_CheckedChanged(object sender, EventArgs e)
        {
            this.ckHaveLocallyMaterial.Enabled = this.ckImport.Checked;
            if (this.ckHaveLocallyMaterial.Enabled == false)
            {
                this.ckHaveLocallyMaterial.Checked = false;
                this.txtLocallyPrice.Text = "0.00";
            }
        }

        void cmbApplyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ckImport.Enabled = this.cmbApplyType.Text == "内置材料";
            if (this.ckImport.Enabled == false)
            {
                this.ckImport.Checked = false;
                this.ckHaveLocallyMaterial.Checked = false;
                this.txtLocallyPrice.Text = "0.00";
            }
        }

        void btnCancelApprove_Click(object sender, EventArgs e)
        {
            if (this.cancelApprove() > 0)
            {
                this.ParentForm.Close();
            }
        }

        void btnConfrimApprove_Click(object sender, EventArgs e)
        {
            if (this.confirmApprove() > 0)
            {
                this.ParentForm.Close();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            this.cmbApplyType.Focus();
            base.OnLoad(e);
        }

        private Neusoft.HISFC.Models.Registration.Register p = null;
        private Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = null;
        private  List<Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList> feeItemLists = null;

        public Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList FeeItemList
        {
            get { return feeItemList; }
            set { feeItemList = value; }
        }

        private BizLogic.ApproveItemModel model = null;
        public BizLogic.ApproveItemModel ApproveItemModel
        {
            get
            {
                return model;
            }
        }

        private Dictionary<string, BizLogic.ApproveItemModel> approveItemModels = new Dictionary<string,Neusoft.HISFC.Components.OutpatientFee.BizLogic.ApproveItemModel>();
        public Dictionary<string, BizLogic.ApproveItemModel> ApproveItemModels
        {
            get
            {
                return approveItemModels;
            }
        }
        private BizLogic.ApproveItemBizLogic approveItemMgr = new BizLogic.ApproveItemBizLogic();
        private Neusoft.HISFC.BizLogic.Fee.InPatient feeInPatientMgr = new Neusoft.HISFC.BizLogic.Fee.InPatient();

        public int SetInfo(Neusoft.HISFC.Models.Registration.Register p, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList, BizLogic.ApproveItemModel model, ref string errorInfo)
        {
            //如果该项目价格低于300，并且组套包含的项目没有超过300的，提示不能审批
            //if (feeItemList.Item.Price < 300)
            //{
            //    if (string.IsNullOrEmpty(feeItemList.UndrugComb.ID))
            //    {
            //        errorInfo = "此项目价格未超过300，不允许审批！";
            //        return -1;
            //    }
            //}
            //查找是否已经审批过
            //如果审批过，则直接读取审批信息
            //如果未审批，则提示需要审批


            this.p = p;
            this.feeItemList = feeItemList;
            this.model = model;


            //查找项目的医保信息
            Neusoft.HISFC.BizLogic.Fee.Interface interfaceMgr = new Neusoft.HISFC.BizLogic.Fee.Interface();
            Neusoft.HISFC.BizLogic.Manager.Constant constMgr = new Neusoft.HISFC.BizLogic.Manager.Constant();

            Neusoft.HISFC.Models.SIInterface.Compare siItem = new Neusoft.HISFC.Models.SIInterface.Compare();
            if (interfaceMgr.GetCompareSingleItem(p.Pact.ID, feeItemList.Item.ID, ref siItem) < 0 || siItem == null || string.IsNullOrEmpty(siItem.CenterItem.ID))
            {
                errorInfo = string.Format("未找到编码：{0}，名称：{1}  的医保对照项目", feeItemList.Item.ID, feeItemList.Item.Name) + interfaceMgr.Err;
                return -1;
            }

            //获取患者的医保信息
            Neusoft.HISFC.Models.RADT.PatientInfo siPatientInfo = interfaceMgr.GetSIPersonInfo(p.ID);
            if (siPatientInfo == null || string.IsNullOrEmpty(siPatientInfo.ID))
            {
                errorInfo = string.Format("未找到患者：{0}  的医保记录", p.Name) + interfaceMgr.Err;
                return -1;
            }

            //国产材料？
            Neusoft.FrameWork.Models.NeuObject obj = constMgr.GetConstant("UndruyItemJK", feeItemList.Item.ID);
            if (obj == null)
            {
                errorInfo = constMgr.Err;
                return -1;
            }

            if (string.IsNullOrEmpty(obj.ID) == false)
            {
                this.cmbApplyType.Text = "内置材料";
                this.ckImport.Checked = true;
                this.ckHaveLocallyMaterial.Checked = Neusoft.FrameWork.Function.NConvert.ToBoolean(obj.Memo);
                this.txtLocallyPrice.Text = Neusoft.FrameWork.Function.NConvert.ToDecimal(((Neusoft.HISFC.Models.Base.Const)obj).UserCode).ToString("F4");
            }

            //赋值
            this.txtRegNO.Text = siPatientInfo.SIMainInfo.RegNo;
            this.txtHosNO.Text = siPatientInfo.SIMainInfo.RegNo.Substring(0, 6);
            if (feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
            {
                this.txtItemCode.Text = SOC.HISFC.BizProcess.CommonInterface.CommonController.Instance.GetItem(feeItemList.Item.ID).UserCode;
            }
            else if(feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
            {
                this.txtItemCode.Text =
                    SOC.HISFC.BizProcess.Cache.Pharmacy.GetItemCustomNO(feeItemList.Item.ID);
            }
            this.txtItemName.Text = feeItemList.Item.Name;
            this.txtCenterCode.Text = siItem.CenterItem.ID;
            this.txtCenterName.Text = siItem.CenterItem.Name;
            this.txtPrice.Text = feeItemList.Item.Price.ToString("F4");
            this.txtQty.Text = feeItemList.Item.Qty.ToString("F2");
            this.txtUse.Text = string.IsNullOrEmpty(feeItemList.Order.Usage.ID) ? "无" : feeItemList.Order.Usage.ID;
            this.txtLocallyPrice.Text = "0.00";

            if (string.IsNullOrEmpty(this.feeItemList.RecipeNO) == false)
            {
                //取数据
                BizLogic.ApproveItemModel approveItem = this.approveItemMgr.Get(feeItemList.RecipeNO, feeItemList.SequenceNO);
                if (approveItem != null)
                {
                    if (string.IsNullOrEmpty(approveItem.RecipeNO) == false)
                    {
                        this.model = approveItem;
                    }
                }


            }


            if (this.model != null)
            {
                this.cmbApplyType.Text = this.model.ApplyType;
                this.ckImport.Checked = this.model.ImportFlag;
                this.ckHaveLocallyMaterial.Checked = this.model.HaveLocallyMaterialFlag;
                this.txtLocallyPrice.Text = this.model.LocallyPrice.ToString("F4");
                this.txtHosOpinion.Text = this.model.HosOpinion;
                this.txtApplyReason.Text = this.model.ApplyReason;
                this.txtMemo.Text = this.model.Memo;
            }


            return 1;
        }

        public int SetInfo(Neusoft.HISFC.Models.Registration.Register p, List<Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList> feeItemLists, Dictionary<string,BizLogic.ApproveItemModel> models, ref string errorInfo)
        {
            //如果该项目价格低于300，并且组套包含的项目没有超过300的，提示不能审批
            //if (feeItemList.Item.Price < 300)
            //{
            //    if (string.IsNullOrEmpty(feeItemList.UndrugComb.ID))
            //    {
            //        errorInfo = "此项目价格未超过300，不允许审批！";
            //        return -1;
            //    }
            //}
            //查找是否已经审批过
            //如果审批过，则直接读取审批信息
            //如果未审批，则提示需要审批

            Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = feeItemLists[0];
            this.p = p;
            this.feeItemList = feeItemList;
            this.feeItemLists = feeItemLists;
            if (models.Count > 0)
            {
                this.model = models.First().Value;
                this.approveItemModels = models;
            }


            //查找项目的医保信息
            Neusoft.HISFC.BizLogic.Fee.Interface interfaceMgr = new Neusoft.HISFC.BizLogic.Fee.Interface();
            Neusoft.HISFC.BizLogic.Manager.Constant constMgr = new Neusoft.HISFC.BizLogic.Manager.Constant();

            Neusoft.HISFC.Models.SIInterface.Compare siItem = new Neusoft.HISFC.Models.SIInterface.Compare();
            if (interfaceMgr.GetCompareSingleItem(p.Pact.ID, feeItemList.Item.ID, ref siItem) < 0 || siItem == null || string.IsNullOrEmpty(siItem.CenterItem.ID))
            {
                errorInfo = string.Format("未找到编码：{0}，名称：{1}  的医保对照项目", feeItemList.Item.ID, feeItemList.Item.Name) + interfaceMgr.Err;
                return -1;
            }

            //获取患者的医保信息
            Neusoft.HISFC.Models.RADT.PatientInfo siPatientInfo = interfaceMgr.GetSIPersonInfo(p.ID);
            if (siPatientInfo == null || string.IsNullOrEmpty(siPatientInfo.ID))
            {
                errorInfo = string.Format("未找到患者：{0}  的医保记录", p.Name) + interfaceMgr.Err;
                return -1;
            }

            //国产材料？
            Neusoft.FrameWork.Models.NeuObject obj = constMgr.GetConstant("UndruyItemJK", feeItemList.Item.ID);
            if (obj == null)
            {
                errorInfo = constMgr.Err;
                return -1;
            }

            if (string.IsNullOrEmpty(obj.ID) == false)
            {
                //this.cmbApplyType.Text = "内置材料";
                this.ckImport.Checked = true;
                this.ckHaveLocallyMaterial.Checked = Neusoft.FrameWork.Function.NConvert.ToBoolean(obj.Memo);
                this.txtLocallyPrice.Text = Neusoft.FrameWork.Function.NConvert.ToDecimal(((Neusoft.HISFC.Models.Base.Const)obj).UserCode).ToString("F4");
            }

            //赋值
            this.txtRegNO.Text = siPatientInfo.SIMainInfo.RegNo;
            this.txtHosNO.Text = siPatientInfo.SIMainInfo.RegNo.Substring(0, 6);
            //this.txtItemCode.Text = SOC.HISFC.BizProcess.CommonInterface.CommonController.Instance.GetItem(feeItemList.Item.ID).UserCode;
            //this.txtItemName.Text = feeItemList.Item.Name;
            //this.txtCenterCode.Text = siItem.CenterItem.ID;
            //this.txtCenterName.Text = siItem.CenterItem.Name;
            //this.txtPrice.Text = feeItemList.Item.Price.ToString("F4");
            //this.txtQty.Text = feeItemList.Item.Qty.ToString("F2");
            //this.txtUse.Text = string.IsNullOrEmpty(feeItemList.Order.Usage.ID) ? "无" : feeItemList.Order.Usage.ID;
            //this.txtLocallyPrice.Text = "0.00";

            if (string.IsNullOrEmpty(this.feeItemList.RecipeNO) == false)
            {
                //取数据
                BizLogic.ApproveItemModel approveItem = this.approveItemMgr.Get(feeItemList.RecipeNO, feeItemList.SequenceNO);
                if (approveItem != null)
                {
                    if (string.IsNullOrEmpty(approveItem.RecipeNO) == false)
                    {
                        this.model = approveItem;
                    }
                }

            }


            if (this.model != null)
            {
                this.cmbApplyType.Text = this.model.ApplyType;
                this.ckImport.Checked = this.model.ImportFlag;
                this.ckHaveLocallyMaterial.Checked = this.model.HaveLocallyMaterialFlag;
                this.txtLocallyPrice.Text = this.model.LocallyPrice.ToString("F4");
                this.txtHosOpinion.Text = this.model.HosOpinion;
                this.txtApplyReason.Text = this.model.ApplyReason;
                this.txtMemo.Text = this.model.Memo;
            }


            return 1;
        }

        private int cancelApprove()
        {
            if (this.feeItemLists != null && this.feeItemLists.Count > 0)
            {
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                this.feeInPatientMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                this.approveItemMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemListTemp in this.feeItemLists)
                {
                    string recipeNO = string.IsNullOrEmpty(feeItemListTemp.UndrugComb.ID) ? feeItemListTemp.RecipeNO : feeItemListTemp.UndrugComb.Extend1;
                    int sequence = string.IsNullOrEmpty(feeItemListTemp.UndrugComb.ID) ? feeItemListTemp.SequenceNO : Neusoft.FrameWork.Function.NConvert.ToInt32(feeItemListTemp.UndrugComb.Extend2);

                    if (string.IsNullOrEmpty(recipeNO) == false)
                    {
                        //删除数据
                        if (this.approveItemMgr.Delete(recipeNO, sequence) < 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("删除特批信息失败" + this.approveItemMgr.Err);
                            return -1;
                        }
                    }
                }
                Neusoft.FrameWork.Management.PublicTrans.Commit();
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemListTemp in this.feeItemLists)
                {
                    feeItemListTemp.ItemRateFlag = "1";
                }
                this.feeItemList.ItemRateFlag = "1";
                this.approveItemModels = null;
                this.model = null;
            }
            else
            {
                #region 单个记录
                string recipeNO = string.IsNullOrEmpty(this.feeItemList.UndrugComb.ID) ? this.feeItemList.RecipeNO : this.feeItemList.UndrugComb.Extend1;
                int sequence = string.IsNullOrEmpty(this.feeItemList.UndrugComb.ID) ? this.feeItemList.SequenceNO : Neusoft.FrameWork.Function.NConvert.ToInt32(this.feeItemList.UndrugComb.Extend2);

                if (string.IsNullOrEmpty(recipeNO) == false)
                {
                    Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                    this.feeInPatientMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                    this.approveItemMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                    //删除数据
                    if (this.approveItemMgr.Delete(recipeNO, sequence) < 0)
                    {
                        Neusoft.FrameWork.Management.PublicTrans.RollBack();
                        MessageBox.Show("删除特批信息失败" + this.approveItemMgr.Err);
                        return -1;
                    }
                    Neusoft.FrameWork.Management.PublicTrans.Commit();
                }
                this.feeItemList.ItemRateFlag = "1";
                model = null;
                #endregion
            }

            return 1;
        }

        private BizLogic.ApproveItemModel getModel()
        {
            BizLogic.ApproveItemModel approve = new BizLogic.ApproveItemModel();
            approve.RecipeNO = string.IsNullOrEmpty(this.feeItemList.UndrugComb.ID) ? this.feeItemList.RecipeNO : this.feeItemList.UndrugComb.Extend1;
            approve.SequenceNO = string.IsNullOrEmpty(this.feeItemList.UndrugComb.ID) ? this.feeItemList.SequenceNO : Neusoft.FrameWork.Function.NConvert.ToInt32(this.feeItemList.UndrugComb.Extend2);
            approve.ID = this.p.ID;
            approve.Name = this.p.Name;
            approve.RegNO = this.txtRegNO.Text;
            approve.HosNO = this.txtHosNO.Text;
            approve.ApplyType = this.cmbApplyType.Text;
            approve.Item.ID = this.txtItemCode.Text;
            approve.Item.Name = this.txtItemName.Text;
            approve.Center.ID = this.txtCenterCode.Text;
            approve.Center.Name = this.txtCenterName.Text;
            approve.Price = this.feeItemList.Item.Price;
            approve.Qty = this.feeItemList.Item.Qty;
            approve.UseCode = this.txtUse.Text;
            approve.ImportFlag = this.ckImport.Checked;
            approve.HaveLocallyMaterialFlag = this.ckHaveLocallyMaterial.Checked;
            approve.LocallyPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtLocallyPrice.Text);
            approve.HosOpinion = this.txtHosOpinion.Text;
            approve.ApplyReason = this.txtApplyReason.Text;
            approve.Memo = this.txtMemo.Text;
            approve.Oper.ID = this.approveItemMgr.Operator.ID;

            return approve;
        }

        private List<BizLogic.ApproveItemModel> getModels()
        {
            Neusoft.HISFC.BizLogic.Fee.Interface interfaceMgr = new Neusoft.HISFC.BizLogic.Fee.Interface();
            List<BizLogic.ApproveItemModel> approves = new List<Neusoft.HISFC.Components.OutpatientFee.BizLogic.ApproveItemModel>();
            if (this.feeItemLists != null && this.feeItemLists.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemListTemp in this.feeItemLists)
                {
                    Neusoft.HISFC.Models.SIInterface.Compare siItem = new Neusoft.HISFC.Models.SIInterface.Compare();
                    if (interfaceMgr.GetCompareSingleItem(p.Pact.ID, feeItemListTemp.Item.ID, ref siItem) < 0 || siItem == null || string.IsNullOrEmpty(siItem.CenterItem.ID))
                    {
                        MessageBox.Show(string.Format("未找到编码：{0}，名称：{1}  的医保对照项目", feeItemListTemp.Item.ID, feeItemListTemp.Item.Name) + interfaceMgr.Err);
                        return null;
                    }

                    BizLogic.ApproveItemModel approve = new BizLogic.ApproveItemModel();
                    approve.RecipeNO = string.IsNullOrEmpty(feeItemListTemp.UndrugComb.ID) ? feeItemListTemp.RecipeNO : feeItemListTemp.UndrugComb.Extend1;
                    approve.SequenceNO = string.IsNullOrEmpty(feeItemListTemp.UndrugComb.ID) ? feeItemListTemp.SequenceNO : Neusoft.FrameWork.Function.NConvert.ToInt32(feeItemListTemp.UndrugComb.Extend2);
                    approve.ID = this.p.ID;
                    approve.Name = this.p.Name;
                    approve.RegNO = this.txtRegNO.Text;
                    approve.HosNO = this.txtHosNO.Text;
                    approve.ApplyType = this.cmbApplyType.Text;
                     if (feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
                    {
                            approve.Item.ID = SOC.HISFC.BizProcess.CommonInterface.CommonController.Instance.GetItem(feeItemListTemp.Item.ID).UserCode;
                    }
                    else if(feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                            approve.Item.ID = 
                            SOC.HISFC.BizProcess.Cache.Pharmacy.GetItemCustomNO(feeItemListTemp.Item.ID);
                    }
                    approve.Item.Name = feeItemListTemp.Item.Name;
                    approve.Center.ID = siItem.CenterItem.ID;
                    approve.Center.Name = siItem.CenterItem.Name;
                    approve.Price = feeItemListTemp.Item.Price;
                    approve.Qty = feeItemListTemp.Item.Qty;
                    approve.UseCode = string.IsNullOrEmpty(feeItemListTemp.Order.Usage.Name) ? "无" : feeItemListTemp.Order.Usage.Name;
                    approve.ImportFlag = this.ckImport.Checked;
                    approve.HaveLocallyMaterialFlag = this.ckHaveLocallyMaterial.Checked;
                    approve.LocallyPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.txtLocallyPrice.Text);
                    approve.HosOpinion = this.txtHosOpinion.Text;
                    approve.ApplyReason = this.txtApplyReason.Text;
                    approve.Memo = this.txtMemo.Text;
                    approve.Oper.ID = this.approveItemMgr.Operator.ID;

                    approves.Add(approve);
                }
            }

            return approves;
        }

        private bool valid(BizLogic.ApproveItemModel approve, ref string errorInfo)
        {
            if (string.IsNullOrEmpty(approve.ApplyType))
            {
                errorInfo = "申请类别为空！";
                return false;
            }

            if (string.IsNullOrEmpty(approve.HosOpinion))
            {
                errorInfo = "医院意见为空！";
                return false;
            }

            if (string.IsNullOrEmpty(approve.ApplyReason))
            {
                errorInfo = "申请理由为空！";
                return false;
            }

            if (approve.ApplyType == "内置材料")
            {
                if (approve.ImportFlag && approve.HaveLocallyMaterialFlag)
                {
                    if (approve.LocallyPrice <= 0)
                    {
                        errorInfo = "国产价格不能小于等于零！";
                        return false;
                    }
                }
            }
            return true;
        }

        private int confirmApprove()
        {
            if (this.feeItemLists != null && this.feeItemLists.Count > 0)
            {
                #region 多个记录
                List<BizLogic.ApproveItemModel> approves = this.getModels();

                string errInfo = string.Empty;
                Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                this.feeInPatientMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                this.approveItemMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                this.approveItemModels = new Dictionary<string, Neusoft.HISFC.Components.OutpatientFee.BizLogic.ApproveItemModel>();
                foreach (BizLogic.ApproveItemModel approve in approves)
                {
                    if (valid(approve, ref errInfo) == false)
                    {
                        MessageBox.Show(this, errInfo, "提示");
                        return -1;
                    }

                    if (string.IsNullOrEmpty(approve.RecipeNO) == false)
                    {
                        //插入数据
                        if (this.approveItemMgr.Insert(approve) < 0)
                        {
                            if (this.approveItemMgr.Update(approve) <= 0)
                            {
                                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                                MessageBox.Show("更新特批信息失败" + this.approveItemMgr.Err);
                                return -1;
                            }
                        }
                    }


                    this.approveItemModels.Add(approve.RecipeNO + approve.SequenceNO, approve);

                }
                Neusoft.FrameWork.Management.PublicTrans.Commit();

                this.feeItemList.ItemRateFlag = "3";
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemListTemp in this.feeItemLists)
                {
                    feeItemListTemp.ItemRateFlag = "3";
                }
                model = approves[0];
                #endregion
            }
            else
            {
                #region 单个记录
                BizLogic.ApproveItemModel approve = this.getModel();

                string errInfo = string.Empty;
                if (valid(approve, ref errInfo) == false)
                {
                    MessageBox.Show(this, errInfo, "提示");
                    return -1;
                }

                if (string.IsNullOrEmpty(approve.RecipeNO) == false)
                {
                    Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
                    this.feeInPatientMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
                    this.approveItemMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

                    //插入数据
                    if (this.approveItemMgr.Insert(approve) < 0)
                    {
                        if (this.approveItemMgr.Update(approve) <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("更新特批信息失败" + this.approveItemMgr.Err);
                            return -1;
                        }
                    }
                    Neusoft.FrameWork.Management.PublicTrans.Commit();
                }
                this.feeItemList.ItemRateFlag = "3";
                model = approve;
                #endregion
            }

            return 1;
        }
    }
}
