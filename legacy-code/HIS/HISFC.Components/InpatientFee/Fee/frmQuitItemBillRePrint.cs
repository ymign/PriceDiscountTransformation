using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    /// <summary>
    /// 护士站退费补打
    /// </summary>
    public partial class frmQuitItemBillRePrint : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public frmQuitItemBillRePrint()
        {
            InitializeComponent();
            if (this.DesignMode) return;

            this.Load += new EventHandler(frmQuitItemBillRePrint_Load);
            this.treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);
            this.toolBar1.ButtonClick += new ToolBarButtonClickEventHandler(toolBar1_ButtonClick);
            this.ucQueryInpatientNo1.myEvent += new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(ucQueryInpatientNo1_myEvent);
        }

        /// <summary>
        /// 退费单打印接口
        /// </summary>
        private Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBackFeeApplyPrint IBackFeePrint = null;
        /// <summary>
        /// 患者信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.PatientInfo patient = null;
        /// <summary>
        /// 患者信息
        /// </summary>
        public Neusoft.HISFC.Models.RADT.PatientInfo Patient
        {
            get { return patient; }
            set
            {
                if (value == null)
                    this.patient = new Neusoft.HISFC.Models.RADT.PatientInfo();
                else
                    this.patient = value;
                this.ucQueryInpatientNo1.Text = this.patient.Patient.PID.PatientNO;
                this.SetPatientInfo();
            }

        }

        /// <summary>
        /// 管理类
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.ReturnApply applyReturnManager = new Neusoft.HISFC.BizLogic.Fee.ReturnApply();
        Neusoft.HISFC.BizLogic.Fee.InPatient FeeInpatient = new Neusoft.HISFC.BizLogic.Fee.InPatient();
        /// <summary>
        /// 当前选中的节点
        /// </summary>
        Neusoft.FrameWork.Models.NeuObject SelectInfo = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 科室信息
        /// </summary>
        private ArrayList alDept = new ArrayList();

        /// <summary>
        /// 设置患者信息
        /// </summary>
        private void SetPatientInfo()
        {
            if (this.patient == null)
                return;
            this.lbPatientInfo.Text = string.Format("姓名：{0}  所在病区： {1}   床号： {2}", this.patient.Name, this.patient.PVisit.PatientLocation.Dept.Name, this.patient.PVisit.PatientLocation.Bed.Name);
        }
        /// <summary>
        /// 显示退费单列表
        /// </summary>
        protected void ShowBill()
        {
            this.treeView1.Nodes.Clear();

            #region 未确认退费节点加载
            TreeNode rootNode = new TreeNode();
            rootNode.Text = "未确认退费单";
            rootNode.Tag = null;
            rootNode.ImageIndex = 0;
            rootNode.SelectedImageIndex = 0;

            TreeNode drugNode = new TreeNode();
            Neusoft.FrameWork.Models.NeuObject info1 = new Neusoft.FrameWork.Models.NeuObject();
            info1.ID = "AAAA";			//单据号
            info1.Memo = "1";			//药品标志
            info1.User01 = "0";			//退费确认标志
            drugNode.Text = "药品";
            drugNode.Tag = info1;
            drugNode.ImageIndex = 0;
            drugNode.SelectedImageIndex = 0;
            rootNode.Nodes.Add(drugNode);

            TreeNode unDrugNode = new TreeNode();
            Neusoft.FrameWork.Models.NeuObject info2 = new Neusoft.FrameWork.Models.NeuObject();
            info2.ID = "AAAA";
            info2.Memo = "0";
            info2.User01 = "0";
            unDrugNode.Text = "非药品";
            unDrugNode.Tag = info2;
            unDrugNode.ImageIndex = 0;
            unDrugNode.SelectedImageIndex = 0;
            rootNode.Nodes.Add(unDrugNode);
            this.treeView1.Nodes.Add(rootNode);
            #endregion

            #region 已确认退费节点加载
            TreeNode approveNode = new TreeNode();
            approveNode.Text = "已确认退费单";
            approveNode.Tag = null;
            approveNode.ImageIndex = 0;
            approveNode.SelectedImageIndex = 0;

            TreeNode approveDrugNode = new TreeNode();
            Neusoft.FrameWork.Models.NeuObject info3 = new Neusoft.FrameWork.Models.NeuObject();
            info3.ID = "AAAA";
            info3.Memo = "1";
            info3.User01 = "1";
            approveDrugNode.Text = "药品";
            approveDrugNode.Tag = info3;
            approveDrugNode.ImageIndex = 0;
            approveDrugNode.SelectedImageIndex = 0;
            approveNode.Nodes.Add(approveDrugNode);

            TreeNode approveUnDrugNode = new TreeNode();
            Neusoft.FrameWork.Models.NeuObject info4 = new Neusoft.FrameWork.Models.NeuObject();
            info4.ID = "AAAA";
            info4.Memo = "0";
            info4.User01 = "1";
            approveUnDrugNode.Text = "非药品";
            approveUnDrugNode.Tag = info4;
            approveUnDrugNode.ImageIndex = 0;
            approveUnDrugNode.SelectedImageIndex = 0;
            approveNode.Nodes.Add(approveUnDrugNode);
            this.treeView1.Nodes.Add(approveNode);
            #endregion

            if (this.patient != null)
            {

                DateTime dt1 = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.dtBegin.Text).Date;
                DateTime dt2 = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.dtEnd.Text).Date.AddDays(1);


                //ArrayList al = applyReturnManager.GetList(this.patient.ID, this.patient.PVisit.PatientLocation.Dept.ID, dt1, dt2, "A");
                ArrayList al = applyReturnManager.GetList(this.patient.ID, ((Neusoft.HISFC.Models.Base.Employee)FeeInpatient.Operator).Dept.ID, dt1, dt2, "A");

                if (al == null)
                {
                    MessageBox.Show("获取退费单列表失败");
                    return;
                }
                TreeNode tempNode;
                foreach (Neusoft.FrameWork.Models.NeuObject info in al)
                {
                    tempNode = new TreeNode();
                    tempNode.Text = info.ID;
                    tempNode.Tag = info;
                    tempNode.ImageIndex = 1;
                    tempNode.SelectedImageIndex = 2;
                    if (info.User01 == "0")			//未确认
                    {
                        if (info.Memo == "0")		//非药品
                            unDrugNode.Nodes.Add(tempNode);
                        else						//药品
                            drugNode.Nodes.Add(tempNode);
                    }
                    else							//确认
                    {
                        if (info.Memo == "0")		//非药品
                        {
                            approveUnDrugNode.Nodes.Add(tempNode);
                            if (info.User01 == "2")		//作废的申请
                            {
                                tempNode.ForeColor = System.Drawing.Color.Red;
                                tempNode.Text = tempNode.Text + "[作废]";
                            }
                        }
                        else						//药品
                        {
                            approveDrugNode.Nodes.Add(tempNode);
                            if ((info.User02 == "1" && info.User01 == "3") || (info.User02 == "0" && info.User01 == "2"))	//作废的申请
                            {
                                tempNode.ForeColor = System.Drawing.Color.Red;
                                tempNode.Text = tempNode.Text + "[作废]";
                            }
                        }
                    }
                }
            }
            ucQueryInpatientNo1.Text = this.Patient.PID.PatientNO;
            this.treeView1.SelectedNode = this.treeView1.Nodes[0];
            this.treeView1.ExpandAll();
        }


        /// <summary>
        /// 显示退费明细
        /// </summary>
        protected void ShowData(Neusoft.FrameWork.Models.NeuObject info)
        {
            if (this.patient == null || info == null)
            {
                return;
            }
            System.Data.DataSet ds = new System.Data.DataSet();
            string strIndex = "";
            if (this.SelectInfo.Memo == "0")		//非药品
            {
                strIndex = "Fee.ApplyReturn.GetBillDetail.2";	//由退费申请表内检索非药品
            }
            else									//药品
            {
                strIndex = "Fee.ApplyReturn.GetBillDetail.1";	//由退费申请表/出库申请表内 检索药品
            }
            //			if (this.applyReturnManager.ExecQuery(strIndex,ref ds,this.SelectInfo.ID,this.patient.PVisit.PatientLocation.Dept.ID,this.SelectInfo.User01) == -1)
            if (this.applyReturnManager.ExecQuery(strIndex, ref ds, this.SelectInfo.ID, "AAAA", this.SelectInfo.User01, this.patient.ID) == -1)
            {
                MessageBox.Show("获取退费明细失败");
                return;
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                this.fpSpread1_Sheet1.Rows.Count = 0;					//清空公式
                this.fpSpread1_Sheet1.DataSource = ds;
            }

            #region 格式化
            try
            {
                this.fpSpread1_Sheet1.Columns[0].Visible = false;		//处方号
                this.fpSpread1_Sheet1.Columns[1].Visible = false;		//序号
                this.fpSpread1_Sheet1.Columns[2].Width = 120F;			//名称
                this.fpSpread1_Sheet1.Columns[3].Width = 75F;			//规格
                this.fpSpread1_Sheet1.Columns[4].Width = 60F;			//数量
                this.fpSpread1_Sheet1.Columns[5].Width = 60F;			//金额
                this.fpSpread1_Sheet1.Columns[6].Width = 80F;			//备注
                this.fpSpread1_Sheet1.Columns[7].Width = 80F;			//退药确认
                this.fpSpread1_Sheet1.Columns[8].Width = 80F;			//申请人
                this.fpSpread1_Sheet1.Columns[9].Width = 80F;			//申请时间
                this.fpSpread1_Sheet1.Columns[10].Visible = false;		//拼音码
                this.fpSpread1_Sheet1.Columns[11].Visible = false;		//五笔码
                this.fpSpread1_Sheet1.Columns[12].Visible = false;		//自定义码,
                this.fpSpread1_Sheet1.Columns[13].Visible = false;		//通用名拼音码
                this.fpSpread1_Sheet1.Columns[14].Visible = false;		//通用名五笔码

                this.fpSpread1_Sheet1.Rows.Add(this.fpSpread1_Sheet1.Rows.Count, 1);
                this.fpSpread1_Sheet1.Cells[this.fpSpread1_Sheet1.Rows.Count - 1, 2].Text = "合计";
                this.fpSpread1_Sheet1.Cells[this.fpSpread1_Sheet1.Rows.Count - 1, 5].Formula = string.Format("SUM(F{0}:F{1})", 1, this.fpSpread1_Sheet1.Rows.Count - 1);
            }
            catch { }
            #endregion
        }


        /// <summary>
        /// 打印
        /// </summary>
        protected void Print()
        {
            #region 有效性判断
            if (this.patient == null)
            {
                return;
            }
            if (this.fpSpread1_Sheet1.Rows.Count <= 0)
                return;
            if (this.SelectInfo == null)
            {
                return;
            }
            if (this.SelectInfo.User01 != "0" && !((Neusoft.HISFC.Models.Base.Employee)applyReturnManager.Operator).IsManager)
            {
                MessageBox.Show("不能对已退费确认的退费单进行补打");
                return;
            }
            #endregion

            System.DateTime dt = this.applyReturnManager.GetDateTimeFromSysDateTime();
            if (this.IBackFeePrint == null)
            {
                this.IBackFeePrint = Neusoft.FrameWork.WinForms.Classes.UtilInterface.CreateObject(this.GetType(), typeof(Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBackFeeApplyPrint)) as Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBackFeeApplyPrint;
            }
            if (this.IBackFeePrint == null)
            {
                this.IBackFeePrint = new ucQuitDrugBill();
            }
            string recipeNo = "";
            int sequenceNo = 0;
            Neusoft.HISFC.Models.Base.EnumItemType isPharmacy;
            Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItem;
            if (this.SelectInfo.Memo == "0")		//非药品
            {
                isPharmacy = Neusoft.HISFC.Models.Base.EnumItemType.UnDrug;
            }
            else
            {
                isPharmacy = isPharmacy = Neusoft.HISFC.Models.Base.EnumItemType.Drug;
            }
            ArrayList alAllItem = new ArrayList();

            //存储按照药房的分组信息<药房编码,退费申请列表>
            Dictionary<string, ArrayList> dicApply = new Dictionary<string, ArrayList>();

            //不打印合计行
            for (int i = 0; i < this.fpSpread1_Sheet1.Rows.Count - 1; i++)
            {
                if (this.SelectInfo.Memo != "0" && this.fpSpread1_Sheet1.Cells[i, 7].Text != "需退药")
                {
                    continue;
                }
                recipeNo = this.fpSpread1_Sheet1.Cells[i, 0].Text;
                sequenceNo = (int)Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, 1].Text);

                feeItem = this.FeeInpatient.GetItemListByRecipeNO(recipeNo, sequenceNo, isPharmacy);
                if (feeItem == null)
                {
                    MessageBox.Show("获取费用明细出错" + "-" + recipeNo);
                    return;
                }
                feeItem.User01 = this.fpSpread1_Sheet1.Cells[i, 6].Text;		//备注
                feeItem.User02 = this.SelectInfo.ID;		//方号 单据号

                feeItem.Item.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, 4].Text);			//数量
                feeItem.FT.TotCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[i, 5].Text);	//金额
                feeItem.User03 = this.fpSpread1_Sheet1.Cells[i, 9].Text;	//申请日期                
                alAllItem.Add(feeItem);

                string key = feeItem.StockOper.Dept.ID;
                if (dicApply.ContainsKey(key))
                {
                    dicApply[key].Add(feeItem);
                }
                else
                {
                    ArrayList alApp = new ArrayList();
                    alApp.Add(feeItem);
                    dicApply.Add(key, alApp);
                }
            }

            if (this.IBackFeePrint != null)
            {
                if (alAllItem == null || alAllItem.Count == 0)
                {
                    MessageBox.Show("温馨提示：您所退的药品药房没有发药，请核对，谢谢！");
                }

                #region 按药房分组打印
                foreach(string key in dicApply.Keys)
                {
                    ArrayList alAppPrint = dicApply[key];

                    if (alAppPrint != null && alAppPrint.Count > 0)
                    {
                        if (alAppPrint.Count == 1)
                        {
                            this.IBackFeePrint.Patient = this.patient;
                            this.IBackFeePrint.SetData(alAppPrint);
                            this.IBackFeePrint.Print();
                        }
                        else
                        {
                            this.IBackFeePrint.Patient = this.patient;
                            string item_name = string.Empty;//对比
                            int row = 0;
                            Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList tempInfo = new Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList();
                            ArrayList needList = new ArrayList();
                            foreach (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList info in alAppPrint)
                            {
                                if (row == 0)
                                {
                                    item_name = info.Item.Name;
                                    tempInfo = info;
                                    row++;
                                }
                                else
                                {
                                    if (info.Item.Name == item_name)//第一行和第二行相同
                                    {
                                        tempInfo.Item.Qty += info.Item.Qty;
                                        row++;
                                        if (row == alAppPrint.Count)//最后一个和倒数第二个相同
                                        {
                                            needList.Add(tempInfo);
                                        }
                                    }
                                    else
                                    {
                                        needList.Add(tempInfo);
                                        tempInfo = new Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList();
                                        tempInfo = info;
                                        item_name = info.Item.Name;
                                        row++;
                                        if (row == alAppPrint.Count)//最后一个和倒数第二个不同
                                        {
                                            needList.Add(info);
                                        }
                                    }
                                }
                            }
                            this.IBackFeePrint.SetData(needList);

                            //this.IBackFeePrint.SetData(al);  

                            this.IBackFeePrint.Print();
                        }
                    }
                }
                #endregion
            }
        }


        private void frmQuitItemBillRePrint_Load(object sender, EventArgs e)
        {
            this.dtBegin.Value = this.applyReturnManager.GetDateTimeFromSysDateTime().AddDays(-7);
            this.dtEnd.Value = this.dtBegin.Value.AddDays(7);

            Neusoft.HISFC.BizLogic.Manager.Department departmentManager = new Neusoft.HISFC.BizLogic.Manager.Department();
            this.alDept = departmentManager.GetDeptmentAll();

            this.ShowBill();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
                return;
            this.SelectInfo = e.Node.Tag as Neusoft.FrameWork.Models.NeuObject;
            if (this.SelectInfo == null)
            {
                MessageBox.Show("获取退费单据号失败");
                return;
            }
            if (this.SelectInfo.ID == "AAAA")
                return;

            this.ShowData(this.SelectInfo);
        }

        private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button == this.tbExit)
            {
                this.Close();
                return;
            }
            if (e.Button == this.tbQuery)
            {
                this.ShowBill();
                return;
            }
            if (e.Button == this.tbPrint)
            {
                this.Print();
                return;
            }
            if (e.Button == this.tbTime)
            {
                //选择时间段，如果没有选择就返回
                //				if(neusoft.neuFC.Interface.Classes.Function.ChooseDate(ref this.dtBegin, ref this.dtEnd)==0)
                //					return;
                this.ShowBill();
                return;
            }
        }

        private void ucQueryInpatientNo1_myEvent()
        {
            Neusoft.HISFC.BizLogic.RADT.InPatient inPatient = new Neusoft.HISFC.BizLogic.RADT.InPatient();
            //this.Patient = inPatient.PatientQuery(this.ucQueryInpatientNo1.InpatientNo);
            this.Patient = inPatient.QueryPatientInfoByInpatientNO(this.ucQueryInpatientNo1.InpatientNo);
            this.ShowBill();
        }
    }
}
