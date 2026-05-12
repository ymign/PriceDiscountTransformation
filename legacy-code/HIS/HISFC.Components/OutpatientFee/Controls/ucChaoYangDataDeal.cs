using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.FrameWork.Management;

namespace Neusoft.HISFC.Components.OutpatientFee.Controls
{
    /// <summary>
    /// 朝阳居民医保错误数据处理控件
    /// 
    /// 针对社保局无效、本地医保有效，进行补单登记
    /// {4C5542EA-E90E-4831-B430-3D3DBDE12066}
    /// </summary>
    public partial class ucChaoYangDataDeal : Neusoft.FrameWork.WinForms.Controls.ucBaseControl, Neusoft.FrameWork.WinForms.Forms.IInterfaceContainer
    {
        #region 成员
        /// <summary>
        /// 工具条
        /// </summary>
        protected Neusoft.FrameWork.WinForms.Forms.ToolBarService toolBarService = new Neusoft.FrameWork.WinForms.Forms.ToolBarService();
        /// <summary>
        /// 查询上传挂号有效天数
        /// </summary>
        private int regVaidDays = 1;
        /// <summary>
        /// 查询上传合同单位ID|ID
        /// </summary>
        private string strPact = "";
        /// <summary>
        /// 合同单位列表
        /// </summary>
        private List<string> lstPact = new List<string>();

        #endregion


        #region 属性
        /// <summary>
        /// 查询上传患者挂号有效天数
        /// </summary>
        [Category("控件设置"), Description("查询上传患者挂号有效天数")]
        public int RegVaidDays
        {
            get { return regVaidDays; }
            set { regVaidDays = value; }
        }
        /// <summary>
        /// 查询上传合同单位ID|ID
        /// </summary>
        [Category("控件设置"), Description("合同单位ID过滤显示，只显示已设置合同单位的患者")]
        public string StrPact
        {
            get { return strPact; }
            set 
            {
                strPact = value;
                if (!string.IsNullOrEmpty(strPact))
                {
                    lstPact.AddRange(strPact.Split(new char[] { '|' }));
                }
            }
        }

        #endregion 

        #region IInterfaceContainer 成员

        public Type[] InterfaceTypes
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ucChaoYangDataDeal()
        {
            InitializeComponent();
        }

        private void ucPactUpdateLoad_Load(object sender, EventArgs e)
        {
            //DateTime currentTime = outpatientManage.GetDateTimeFromSysDateTime();
            //dtpSeeDateBeg.Value = currentTime.Date;
            //dtpSeeDateEnd.Value = currentTime.Date.AddDays(1).AddSeconds(-1);
        }
        #endregion

        #region 设置toolBar按钮
        /// <summary>
        /// 设置toolBar按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="neuObject"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override Neusoft.FrameWork.WinForms.Forms.ToolBarService Init(object sender, object neuObject, object param)
        {
            toolBarService.AddToolButton("刷新", "刷新！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.S刷新, true, false, null);
            toolBarService.AddToolButton("上传", "上传！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.D导入, true, false, null);
            toolBarService.AddToolButton("作废", "作废！", (int)Neusoft.FrameWork.WinForms.Classes.EnumImageList.Z作废, true, false, null);
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
                case "刷新":
                    Refelash();
                    break;

                case "上传":
                    if (MessageBox.Show("是否要上传医保费用?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        UpdateLoad();
                    }
                    break;
                case "作废":
                    if (MessageBox.Show("是否要作废医保?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        Invalid();
                    }
                    break;
                default:
                    break;
            }
            base.ToolStrip_ItemClicked(sender, e);
        }

        #endregion

        #region 事件
        private void trvPatient_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.ClearAll();
            if (e.Node.Tag == null)
            {
                return;
            }

            Neusoft.HISFC.Models.Registration.Register regInfo = e.Node.Tag as Neusoft.HISFC.Models.Registration.Register;
            if (regInfo == null)
                return;

            SetPatientInfo(regInfo);

            ArrayList arlFeeItemList = outpatientManage.QueryFeeItemByClinicCode(regInfo.ID, regInfo.Pact.ID);
            this.ShowFeeItemList(arlFeeItemList);
        }

        #endregion





        #region 内部方法


        #region 作废医保

        /// <summary>
        /// 作废医保
        /// </summary>
        private void Invalid()
        {
            if (this.tbCardNO.Tag == null)
            {
                MessageBox.Show("请选择患者作废记录!");
                return;
            }

            Neusoft.HISFC.Models.Registration.Register regInfo = this.tbCardNO.Tag as Neusoft.HISFC.Models.Registration.Register;
            if (regInfo == null)
                return;
            //开始事务
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            int returnValue = outpatientManage.InvalidByClinicCode(regInfo.ID, regInfo.Pact.ID);

            if (returnValue == -1)
            {
                //回滚事务
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("作废医保操作失败！"));
                return;
            }

            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show("已作废患者 " + this.tbCardNO.Tag.ToString() + " 的医保记录！");

            //清空显示以及树节点
            this.fpSpread1_Sheet1.RowCount = 0;

            trvPatient.Nodes.Remove(trvPatient.SelectedNode);

        }

        #endregion 作废医保


        #region 上传数据

        /// <summary>
        /// 上传数据,并结算
        /// </summary>
        private void UpdateLoad()
        {
            if (this.tbCardNO.Tag == null)
            {
                MessageBox.Show("请选择患者看诊记录!");
                return;
            }

            Neusoft.HISFC.Models.Registration.Register regInfo = this.tbCardNO.Tag as Neusoft.HISFC.Models.Registration.Register;
            if (regInfo == null)
            {
                MessageBox.Show("请选择患者看诊记录!");
                return;
            }

            if (this.fpSpread1_Sheet1.Tag == null)
            {
                MessageBox.Show("该患者没有费用信息!");
                return;
            }
            ArrayList arlFeeItemList = this.fpSpread1_Sheet1.Tag as ArrayList;
            if (arlFeeItemList == null)
            {
                MessageBox.Show("该患者没有费用信息!");
                return;
            }

            // 判断是否有未收费费用
            bool hasUnFeeItem = false;
            foreach (FeeItemList item in arlFeeItemList)
            {
                if (item.PayType == Neusoft.HISFC.Models.Base.PayTypes.Charged)
                {
                    hasUnFeeItem = true;
                    break;
                }
            }
            if (hasUnFeeItem)
            {
                if (MessageBox.Show("存在未收费的项目，是否继续上传？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                {
                    return;
                }
            }

            ArrayList arlFeeItemUpdateload = new ArrayList();
            System.Collections.Hashtable hsItems = new Hashtable();
            foreach (FeeItemList item in arlFeeItemList)
            {
                if (item.PayType == Neusoft.HISFC.Models.Base.PayTypes.Charged)
                {
                    continue;
                }
                //if (item.User03 == "1")
                //{
                //    // 已上传
                //    continue;
                //}
                if (hsItems.Contains(item.Item.ID))
                {
                    FeeItemList obj = hsItems[item.Item.ID] as FeeItemList;
                    if (obj.RecipeNO == item.RecipeNO && obj.Order.ID == item.Order.ID )
                    {
                        MessageBox.Show("存在 项目代码：" + item.Item.ID + " 项目名称：" + item.Item.Name + " 的处方号、医嘱号一致，违反fs_local_masmzjsmx表的唯一性");
                        return;
                    }
                    else 
                    {
                        arlFeeItemUpdateload.Add(item);
                    }
                }
                else
                {
                    hsItems.Add(item.Item.ID, item);
                    arlFeeItemUpdateload.Add(item);
                }
            }
            if (arlFeeItemUpdateload.Count <= 0)
            {
                MessageBox.Show("无需要上传的费用！");
                return;
            }

            //// 防止挂号信息与收费信息的合同单位不一致
            //if (regInfo.Pact.PayKind.ID !="02")
            //{
            //    string pactcode = this.regManage.GetPactCodeFoMedcare(regInfo.ID);
            //    if (pactcode !=null &&　pactcode!=string.Empty)
            //    {
            //        regInfo.Pact.ID = pactcode;                  
            //    }
            //}
            #region 本地操作
            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();
            regManage.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);


            string medno=regManage.QuerySiMedNo(regInfo.ID);
            if (medno==null||medno=="") return;

            int result = regManage.UpdateSiMainInfo(regInfo.ID, medno);
            if (result==-1)
            {
                 Neusoft.FrameWork.Management.PublicTrans.RollBack();
                 if (regManage.Err != "")
                 {
                     MessageBox.Show(regManage.Err);
                     return;
                 }
            }

            result = regManage.UpdateSiSirecord(regInfo.ID);
            if (result == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                if (regManage.Err != "")
                {
                    MessageBox.Show(regManage.Err);
                    return;
                }
            }

            result = regManage.DeleteLocalSiFeeDetail(medno);
            if (result == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                if (regManage.Err != "")
                {
                    MessageBox.Show(regManage.Err);
                    return;
                }
            }

            #endregion


            medcareProxy.SetPactCode(regInfo.Pact.ID);
            medcareProxy.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            medcareProxy.BeginTranscation();
            
            medcareProxy.IsLocalProcess = false;

            regInfo.UpFlag = "1";//保存补单标记

            //连接待遇接口
            long returnValue = this.medcareProxy.Connect();
            if (returnValue == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //医保回滚可能出错，此处提示
                if (this.medcareProxy.Rollback() == -1)
                {
                    MessageBox.Show(this.medcareProxy.ErrMsg);
                    return ;
                }
                MessageBox.Show(Language.Msg("医疗待遇接口连接失败!") + this.medcareProxy.ErrMsg);

                return ;
            }

            // 判断是否允许报销
            if(this.medcareProxy.IsInBlackList(regInfo))
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //医保回滚可能出错，此处提示
                if (this.medcareProxy.Rollback() == -1)
                {
                    MessageBox.Show(this.medcareProxy.ErrMsg);
                    return;
                }

                MessageBox.Show(this.medcareProxy.ErrMsg);

                return;
            }

            
            //删除本次因为错误或者其他原因上传的明细
            returnValue = this.medcareProxy.DeleteUploadedFeeDetailsAllOutpatient(regInfo);
            //重新上传所有明细
            returnValue = this.medcareProxy.UploadFeeDetailsOutpatient(regInfo, ref arlFeeItemUpdateload);
            if (returnValue == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //医保回滚可能出错，此处提示
                if (this.medcareProxy.Rollback() == -1)
                {
                    MessageBox.Show(this.medcareProxy.ErrMsg);
                    return;
                }
                this.medcareProxy.Disconnect();
                MessageBox.Show(Language.Msg("上传费用明细失败!") + this.medcareProxy.ErrMsg);

                return ;
            }

            returnValue = this.medcareProxy.BdBalanceOutpatient(regInfo, ref arlFeeItemUpdateload);
            
            if (returnValue == -1)
            {
                Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //医保回滚可能出错，此处提示
                if (this.medcareProxy.Rollback() == -1)
                {
                    MessageBox.Show(this.medcareProxy.ErrMsg);
                    return ;
                }
                this.medcareProxy.Disconnect();
                MessageBox.Show(Language.Msg("医保补单结算失败!") + this.medcareProxy.ErrMsg);

                return ;
            }

            #region 补单结算后，不需要调用医保结算
            //returnValue = this.medcareProxy.BalanceOutpatient(regInfo, ref arlFeeItemUpdateload);
            //if (returnValue != 1)
            //{
            //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
            //    this.medcareProxy.Rollback();
            //    MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("待遇接口门诊结算失败") + this.medcareProxy.ErrMsg);
            //    return;
            //}
            #endregion
            Neusoft.FrameWork.Management.PublicTrans.Commit();

            MessageBox.Show("上传成功！");
        }


        #endregion



        #region 加载数据
        /// <summary>
        /// 刷新
        /// </summary>
        private void Refelash()
        {
            //if (lstPact == null || lstPact.Count <= 0)
            //{
            //    MessageBox.Show("没有指定合同单位!");
            //    return;
            //}
            //if (regVaidDays <= 0 || regVaidDays > 30)
            //{
            //    MessageBox.Show("上传挂号有效天数设置出错!");
            //    return;
            //}

            //DateTime dtSeeDateBeg = dtpSeeDateBeg.Value;
            //DateTime dtSeeDateEnd = dtpSeeDateEnd.Value;

            this.trvPatient.Nodes.Clear();
            if (neuTextBox1.Text.Trim().Length==0) return;

            ArrayList regInfoList = regManage.QueryChaoYangWrongRegister(neuTextBox1.Text.Trim());

            AddRegToTree(regInfoList);
        }

        /// <summary>
        /// 添加发票信息到树
        /// </summary>
        /// <param name="regInfoList"></param>
        private void AddRegToTree(ArrayList regInfoList)
        {
            if (regInfoList == null || regInfoList.Count <= 0)
                return;

            foreach (Neusoft.HISFC.Models.Registration.Register regInfo in regInfoList)
            {
                AddRegToTree(regInfo);
            }

        }
        /// <summary>
        /// 添加树节点
        /// </summary>
        private void AddRegToTree(Neusoft.HISFC.Models.Registration.Register regInfo)
        {
            if (regInfo == null)
                return;
            

            //合同单位过滤显示
            if (lstPact.Count > 0)
            {
                if (!lstPact.Contains(regInfo.Pact.ID))
                {
                    return;
                }
            }

            //如果自助挂号没有挂号科室，把看诊科室赋给挂号科室
            if (regInfo.DoctorInfo.Templet.Dept.ID == null || regInfo.DoctorInfo.Templet.Dept.ID == "")
            {
                regInfo.DoctorInfo.Templet.Dept.ID = regInfo.SeeDoct.Dept.ID;
            }

            TreeNode[] tnArr = trvPatient.Nodes.Find(regInfo.DoctorInfo.Templet.Dept.ID, true);

            TreeNode tn = null;
            TreeNode tnTemp = null;
            if (tnArr == null || tnArr.Length <= 0)
            {
                tn = new TreeNode();
                tn.Name = regInfo.DoctorInfo.Templet.Dept.ID;

                tn.Text = regInfo.DoctorInfo.Templet.Dept.Name;

                trvPatient.Nodes.Add(tn);

                tnTemp = new TreeNode();
                tnTemp.Name = regInfo.ID;
                tnTemp.Text = regInfo.Name + " [" + regInfo.SeeDoct.OperTime.ToString("yyyy-MM-dd HH:mm") + "]";
                tnTemp.Tag = regInfo;

                tn.Nodes.Add(tnTemp);
            }
            else
            {
                tnTemp = new TreeNode();
                tnTemp.Name = regInfo.ID;
                tnTemp.Text = regInfo.Name + " [" + regInfo.SeeDoct.OperTime.ToString("yyyy-MM-dd HH:mm") + "]";
                tnTemp.Tag = regInfo;

                tn = tnArr[0];
                tn.Nodes.Add(tnTemp);
            }
        }

        #endregion

        #region 设置界面信息
        /// <summary>
        /// 设置病人信息
        /// </summary>
        /// <param name="regInfo"></param>
        private void SetPatientInfo(Neusoft.HISFC.Models.Registration.Register regInfo)
        {
            this.tbCardNO.Tag = regInfo;

            this.tbCardNO.Text = regInfo.PID.CardNO;
            this.tbName.Text = regInfo.Name;
            this.txtSex.Text = regInfo.Sex.Name;
            this.tbAge.Text = regInfo.Age;
            this.txtRegDept.Text = regInfo.DoctorInfo.Templet.Dept.Name;
            this.txtDoct.Text = regInfo.DoctorInfo.Templet.Doct.Name;
            this.txtPact.Text = regInfo.Pact.Name;
            this.txtMarkNo.Text = "";
            this.tbMCardNO.Text = regInfo.SSN;
            this.txtRebate.Text = "";
        }

        /// <summary>
        /// 显示费用信息
        /// </summary>
        /// <param name="allFeeItemList">所有费用信息</param>
        public void ShowFeeItemList(ArrayList allFeeItemList)
        {
            if (allFeeItemList == null || allFeeItemList.Count <= 0)
            {
                return;
            }

            this.fpSpread1_Sheet1.RowCount = allFeeItemList.Count;

            this.fpSpread1_Sheet1.Tag = allFeeItemList;

            int iRowIdx = 0;
            foreach (FeeItemList item in allFeeItemList)
            {
                this.ShowFeeItem(item, iRowIdx);
                iRowIdx++;
            }
        }
        /// <summary>
        /// 显示一行数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="iRowIdx"></param>
        public void ShowFeeItem(FeeItemList item, int iRowIdx)
        {
            if (item == null)
                return;
            if (iRowIdx < 0 || iRowIdx >= this.fpSpread1_Sheet1.RowCount)
            {
                return;
            }

            string strFee = "未扣费";
            if (item.PayType == Neusoft.HISFC.Models.Base.PayTypes.Balanced)
            {
                this.SetFPFeeFont(iRowIdx);
                strFee = "已扣费";
            }
            else
            {
                this.SetFPUnFeeFont(iRowIdx);
            }

            this.fpSpread1_Sheet1.Rows[iRowIdx].Tag = item;

            this.fpSpread1_Sheet1.Cells[iRowIdx, 0].Value = item.Item.Name;
            this.fpSpread1_Sheet1.Cells[iRowIdx, 1].Value = item.Item.Specs;
            this.fpSpread1_Sheet1.Cells[iRowIdx, 2].Value = item.Item.Qty.ToString();
            this.fpSpread1_Sheet1.Cells[iRowIdx, 3].Value = System.Convert.ToString(item.FT.PayCost + item.FT.OwnCost + item.FT.PubCost);
            this.fpSpread1_Sheet1.Cells[iRowIdx, 4].Value = item.ExecOper.Dept.Name;
            this.fpSpread1_Sheet1.Cells[iRowIdx, 5].Value = strFee;
            this.fpSpread1_Sheet1.Cells[iRowIdx, 6].Value = item.User03 == "1" ? "已上传" : "未上传";

        }
        /// <summary>
        /// 设置未收费字体
        /// </summary>
        /// <param name="iRowIdx"></param>
        private void SetFPUnFeeFont(int iRowIdx)
        {
            this.fpSpread1_Sheet1.Rows[iRowIdx].Font = new Font("宋体", 10.5f, FontStyle.Bold);
            this.fpSpread1_Sheet1.Rows[iRowIdx].ForeColor = Color.FromArgb(192, 0, 192);
        }
        /// <summary>
        /// 设置收费字体
        /// </summary>
        /// <param name="iRowIdx"></param>
        private void SetFPFeeFont(int iRowIdx)
        {
            this.fpSpread1_Sheet1.Rows[iRowIdx].Font = new Font("宋体", 10.5f, FontStyle.Bold);
            this.fpSpread1_Sheet1.Rows[iRowIdx].ForeColor = Color.Black;
        }
        #endregion

        #region 清空界面信息
        /// <summary>
        /// 清空
        /// </summary>
        private void ClearAll()
        {
            this.ClearPatientInfo();
            this.ClearFeeInfo();
        }
        /// <summary>
        /// 清空病人信息
        /// </summary>
        private void ClearPatientInfo()
        {
            this.tbCardNO.Tag = null;
            this.tbCardNO.Text = "";
            this.tbName.Text = "";
            this.txtSex.Text = "";
            this.tbAge.Text = "";
            this.txtRegDept.Text = "";
            this.txtDoct.Text = "";
            this.txtPact.Text = "";
            this.txtMarkNo.Text = "";
            this.tbMCardNO.Text = "";
            this.txtRebate.Text = "";
        }
        private void ClearFeeInfo()
        {
            this.fpSpread1_Sheet1.RowCount = 0;
        }

        #endregion

        #endregion

        #region 业务层
        /// <summary>
        /// 门诊挂号业务管理
        /// </summary>
        Neusoft.HISFC.BizLogic.Registration.ChaoYangDataDeal regManage = new Neusoft.HISFC.BizLogic.Registration.ChaoYangDataDeal();
        /// <summary>
        /// 门诊费用业务管理
        /// </summary>
        Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManage = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        /// <summary>
        /// 医疗待遇接口
        /// </summary>
        Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy medcareProxy = new Neusoft.HISFC.BizProcess.Integrate.FeeInterface.MedcareInterfaceProxy();


        #endregion

        
    }
}
