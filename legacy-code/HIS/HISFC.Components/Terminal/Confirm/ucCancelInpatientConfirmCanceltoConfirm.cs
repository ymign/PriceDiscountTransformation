using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Neusoft.HISFC.Components.Terminal.Confirm
{
    /// <summary>
    /// ucCancelInpatientConfirm <br></br>
    /// [功能描述: 住院终端确认取消]<br></br>
    /// [创 建 者: 喻赟]<br></br>
    /// [创建时间: 2008-07-11]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public partial class ucCancelInpatientConfirmCanceltoConfirm : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucCancelInpatientConfirmCanceltoConfirm()
        {
            InitializeComponent();
        }

        #region 变量

        /// <summary>
        /// 费用业务
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Fee feeManager = new Neusoft.HISFC.BizProcess.Integrate.Fee();

        /// <summary>
        /// 医嘱业务
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Order orderManager = new Neusoft.HISFC.BizProcess.Integrate.Order();

        /// <summary>
        /// 患者业务
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.RADT radtManager = new Neusoft.HISFC.BizProcess.Integrate.RADT();

        /// <summary>
        /// 终端确认业务
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm confirmIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Terminal.Confirm();

        /// <summary>
        /// 系统管理业务
        /// </summary>
        private Neusoft.HISFC.BizProcess.Integrate.Manager deptManager = new Neusoft.HISFC.BizProcess.Integrate.Manager();

        /// <summary>
        /// 终端业务
        /// </summary>
        private Neusoft.HISFC.BizLogic.Terminal.TerminalConfirm tecManager = new Neusoft.HISFC.BizLogic.Terminal.TerminalConfirm();

        /// <summary>
        /// 患者信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.PatientInfo patientInfo = new Neusoft.HISFC.Models.RADT.PatientInfo();

        /// <summary>
        /// 当前操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee oper = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;

        private Neusoft.HISFC.BizProcess.Integrate.Terminal.Result result = Neusoft.HISFC.BizProcess.Integrate.Terminal.Result.Failure;
        private bool seeAll = false;
        #endregion

        #region 属性
        /// <summary>
        /// 查看所有科室终端确认项目
        /// </summary>
        [Category("控件设置"), Description("查看所有科室终端确认项目")]
        public bool SeeAll
        {
            get
            {
                return seeAll;
            }
            set
            {
                seeAll = value;
            }
        }
        /// <summary>
        /// 患者信息
        /// </summary>
        public Neusoft.HISFC.Models.RADT.PatientInfo PatientInfo
        {
            set
            {
                this.patientInfo = value;
                this.txtName.Text = this.patientInfo.Name;
                this.txtPact.Text = this.patientInfo.Pact.Name;
                this.AddDataToFp(this.QueryExeData(this.patientInfo.ID));
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Init()
        {
            //this.neuSpread1_Sheet1.Columns[0].Visible = false;
            //this.neuSpread1_Sheet1.Columns[1].Visible = false;
            //this.neuSpread1_Sheet1.Columns[6].Visible = false;
            //this.neuSpread1_Sheet1.Columns[7].Visible = false;
            for (int i = 0; i < this.fpSpread1_Sheet1.ColumnCount; i++)
            {
                //取消数量不允许修改
                //if (i != 5)
                //{
                this.fpSpread1_Sheet1.Columns[i].Locked = true;
                //}
            }
            this.ucQueryInpatientNo1.myEvent += new Neusoft.HISFC.Components.Common.Controls.myEventDelegate(this.ucQueryInpatientNo1_myEvent);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        private ArrayList QueryExeData(string inpatientNO)
        {
            string operDept = this.oper.Dept.ID;
            if (seeAll)
            {
                operDept = "all";
            }

            ArrayList al = new ArrayList();

            result = this.confirmIntegrate.QueryConfirmInfoByInpatientNOCancel(inpatientNO, operDept, ref al);
            if (result != Neusoft.HISFC.BizProcess.Integrate.Terminal.Result.Success)
            {
                MessageBox.Show("查找患者终端确认信息失败!" + tecManager.Err);
                this.ucQueryInpatientNo1.Focus();

                return null;
            }

            return al;
        }

        /// <summary>
        /// 添加数据到表格
        /// </summary>
        /// <param name="al"></param>
        protected virtual void AddDataToFp(ArrayList al)
        {
            #region 处理组套项目{5C2A9C83-D165-434c-ACA4-86F23E956442}
            List<string> combIDList = new List<string>();
            #endregion

            this.neuSpread1_Sheet1.RowCount = 0;
            if (al != null && al.Count > 0)
            {
                foreach (Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail confirmDetail in al)
                {
                    #region 处理组套项目{5C2A9C83-D165-434c-ACA4-86F23E956442}
                    int rowIndex = 0;
                    bool isComb = false;
                    Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList itemList = this.feeManager.GetItemListByRecipeNO(confirmDetail.Apply.Item.RecipeNO, confirmDetail.Apply.Item.SequenceNO, Neusoft.HISFC.Models.Base.EnumItemType.UnDrug);
                    if (itemList != null)
                    {
                        if (!string.IsNullOrEmpty(itemList.UndrugComb.ID))
                        {
                            isComb = true;
                            if (!combIDList.Contains(itemList.UndrugComb.ID + confirmDetail.ExecMoOrder))
                            {
                                combIDList.Add(itemList.UndrugComb.ID + confirmDetail.ExecMoOrder);
                                this.neuSpread1_Sheet1.Rows.Add(0, 1);
                                this.neuSpread1_Sheet1.RowHeader.Cells[0, 0].Text = "+";
                                this.neuSpread1_Sheet1.RowHeader.Cells[0, 0].BackColor = Color.YellowGreen;
                                this.neuSpread1_Sheet1.Rows[0].BackColor = Color.LightBlue;
                                this.neuSpread1_Sheet1.Cells[0, 0].Text = confirmDetail.MoOrder;//医嘱号
                                this.neuSpread1_Sheet1.Cells[0, 1].Text = confirmDetail.ExecMoOrder;//医嘱执行号
                                this.neuSpread1_Sheet1.Cells[0, 3].Text = itemList.UndrugComb.ID;//项目编码
                                this.neuSpread1_Sheet1.Cells[0, 4].Text = itemList.UndrugComb.Name;//项目名称
                                this.neuSpread1_Sheet1.Cells[0, 5].Text = confirmDetail.Apply.Item.ConfirmedQty.ToString();//已确认数量
                                Neusoft.HISFC.Models.Base.Employee tmpEmp = this.deptManager.GetEmployeeInfo(confirmDetail.Apply.Item.ConfirmOper.ID);
                                this.neuSpread1_Sheet1.Cells[0, 6].Text = tmpEmp.Name;
                                Neusoft.HISFC.Models.Base.Department tmpDept = this.deptManager.GetDepartment(confirmDetail.Apply.ConfirmOperEnvironment.Dept.ID);//确认科室
                                this.neuSpread1_Sheet1.Cells[0, 7].Tag = tmpDept.ID;
                                this.neuSpread1_Sheet1.Cells[0, 7].Text = tmpDept.Name;
                                this.neuSpread1_Sheet1.Cells[0, 8].Text = confirmDetail.Apply.ConfirmOperEnvironment.OperTime.ToString();//操作时间
                                rowIndex++;
                            }
                            else
                            {
                                //查找组套节点
                                for (int i = 0; i < this.neuSpread1_Sheet1.Rows.Count; i++)
                                {
                                    if (this.neuSpread1_Sheet1.Cells[i, 2].Text == "" && this.neuSpread1_Sheet1.Cells[i, 1].Text == confirmDetail.ExecMoOrder && this.neuSpread1_Sheet1.Cells[i, 3].Text == itemList.UndrugComb.ID)
                                    {
                                        rowIndex = i + 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    //this.neuSpread1_Sheet1.Rows.Add(0, 1);
                    //{5C2A9C83-D165-434c-ACA4-86F23E956442}
                    this.neuSpread1_Sheet1.Rows.Add(rowIndex, 1);
                    if (isComb)
                    {
                        this.neuSpread1_Sheet1.RowHeader.Cells[rowIndex, 0].BackColor = Color.Yellow;
                        this.neuSpread1_Sheet1.Rows[rowIndex].BackColor = Color.LightYellow;
                        this.neuSpread1_Sheet1.RowHeader.Cells[rowIndex, 0].Text = ".";
                        this.neuSpread1_Sheet1.Rows[rowIndex].Visible = false;
                    }

                    Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();
                    Neusoft.HISFC.Models.Base.Department dept = new Neusoft.HISFC.Models.Base.Department();

                    this.neuSpread1_Sheet1.Cells[rowIndex, 0].Text = confirmDetail.MoOrder;//医嘱号
                    this.neuSpread1_Sheet1.Cells[rowIndex, 1].Text = confirmDetail.ExecMoOrder;//医嘱执行号
                    this.neuSpread1_Sheet1.Cells[rowIndex, 2].Text = confirmDetail.Sequence;//流水号  主键
                    this.neuSpread1_Sheet1.Cells[rowIndex, 3].Text = confirmDetail.Apply.Item.ID;//项目编码
                    this.neuSpread1_Sheet1.Cells[rowIndex, 4].Text = confirmDetail.Apply.Item.Name;//项目名称
                    this.neuSpread1_Sheet1.Cells[rowIndex, 5].Text = confirmDetail.Apply.Item.ConfirmedQty.ToString();//已确认数量
                    this.neuSpread1_Sheet1.Cells[rowIndex, 6].Tag = confirmDetail.Apply.Item.ConfirmOper.ID;//确认人
                    employee = this.deptManager.GetEmployeeInfo(confirmDetail.Apply.Item.ConfirmOper.ID);
                    this.neuSpread1_Sheet1.Cells[rowIndex, 6].Text = employee.Name;
                    dept = this.deptManager.GetDepartment(confirmDetail.Apply.ConfirmOperEnvironment.Dept.ID);//确认科室
                    this.neuSpread1_Sheet1.Cells[rowIndex, 7].Tag = dept.ID;
                    this.neuSpread1_Sheet1.Cells[rowIndex, 7].Text = dept.Name;
                    this.neuSpread1_Sheet1.Cells[rowIndex, 8].Text = confirmDetail.Apply.ConfirmOperEnvironment.OperTime.ToString();//操作时间
                    this.neuSpread1_Sheet1.Cells[rowIndex, 9].Text = confirmDetail.Apply.Item.RecipeNO;//处方号
                    this.neuSpread1_Sheet1.Cells[rowIndex, 10].Text = confirmDetail.Apply.Item.SequenceNO.ToString();//处方内流水号
                    this.neuSpread1_Sheet1.Cells[rowIndex, 11].Text = confirmDetail.ExecDevice;//执行设备
                    this.neuSpread1_Sheet1.Cells[rowIndex, 12].Text = confirmDetail.Oper.ID;//执行技师

                    this.neuSpread1_Sheet1.Rows[rowIndex].Tag = confirmDetail;
                }
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        private int Save()
        {
            if (this.neuSpread1_Sheet1.RowCount <= 0)
            {
                return 0;
            }
            if (this.fpSpread1_Sheet1.RowCount <= 0)
            {
                MessageBox.Show("请选择需召回的明细");
                return 0;
            }

            if (MessageBox.Show("是否召回该次终端确认？\r\n 召回确认操作不可回退", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return 0;
            }

            Neusoft.FrameWork.Management.PublicTrans.BeginTransaction();

            Neusoft.HISFC.BizLogic.Terminal.TerminalConfirm terMgr = new Neusoft.HISFC.BizLogic.Terminal.TerminalConfirm();
            //Neusoft.FrameWork.Management.Transaction t = new Neusoft.FrameWork.Management.Transaction(this.tecManager.Connection);
            //t.BeginTransaction();
            this.feeManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            this.orderManager.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);
            //terMgr.SetTrans(Neusoft.FrameWork.Management.PublicTrans.Trans);

            #region 可以一次取消多条 by yuyun 08-8-12 {58B76F7C-A35D-4cbb-8948-8163EA3C5191}

            System.Collections.Hashtable hsMoExecNOList = new Hashtable();

            foreach (FarPoint.Win.Spread.Row r in this.fpSpread1_Sheet1.Rows)
            {
                //Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail obj = ((Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail)this.fpSpread1_Sheet1.Cells[this.fpSpread1_Sheet1.ActiveRowIndex, (int)Cols.MoOrder].Tag).Clone();
                Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail obj = ((Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail)this.fpSpread1_Sheet1.Cells[r.Index, (int)Cols.MoOrder].Tag).Clone();
                obj.FreeCount = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.fpSpread1_Sheet1.Cells[r.Index, (int)Cols.ConfirmQty].Value.ToString());

                #region 医嘱

                if (!hsMoExecNOList.ContainsKey(obj.MoOrder + obj.ExecMoOrder))
                {
                    if (!string.IsNullOrEmpty(obj.MoOrder))//如果就剩一条，就说明所有的都取消了  ???不懂
                    {
                        if (terMgr.CancelInpatientConfirmMoOrderCancel(obj) <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("更新医嘱失败" + terMgr.Err);

                            return -1;
                        }
                    }

                    hsMoExecNOList.Add(obj.MoOrder + obj.ExecMoOrder, null);
                }

                #endregion

                #region 费用

                //更新可退数量
                //if (terMgr.CancelInpatientItemListCancel(obj) <= 0)
                //{
                //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //    MessageBox.Show("更新费用明细失败" + terMgr.Err);

                //    return -1;
                //}

                #endregion

                #region 判断是否需要作废执行档，对已全部确认完的项目作废执行档，未全部确认的项目不作废

                //{0A8C4027-210C-49e0-977F-576789F46946} by yuyun 08-8-13
                //取医嘱执行档的数量
                decimal execOrderQty = terMgr.GetExecOrderQty(obj.ExecMoOrder);
                if (execOrderQty == -1)
                {
                    MessageBox.Show(terMgr.Err);
                    return -1;
                }
                //取已确认的总数量
                decimal confirmedQty = terMgr.GetAlreadConfirmNum(obj.MoOrder, obj.ExecMoOrder);
                //MessageBox.Show(execOrderQty.ToString() + "|||||||" + confirmedQty.ToString());

                //对比两个数量  判断是否需要作废执行档  
                if (confirmedQty == 0)
                {
                    //作废执行档
                    if (!string.IsNullOrEmpty(obj.ExecMoOrder))  //为医嘱开立项目才进行执行档作废
                    {
                        if (terMgr.CancelExecOrderCancel(obj.ExecMoOrder) <= 0)
                        {
                            Neusoft.FrameWork.Management.PublicTrans.RollBack();
                            MessageBox.Show("作废医嘱执行档失败" + terMgr.Err);

                            return -1;
                        }
                    }
                    //MessageBox.Show("更新");
                }

                #endregion

                #region 确认明细

                //if (terMgr.CancelInpatientConfirmDetailCancal(obj) <= 0)
                //{
                //    Neusoft.FrameWork.Management.PublicTrans.RollBack();
                //    MessageBox.Show("更新确认明细失败" + terMgr.Err);

                //    return -1;
                //}

                #endregion
            }

            #endregion

            Neusoft.FrameWork.Management.PublicTrans.Commit();
            MessageBox.Show("取消成功");
            //可以一次取消多条 by yuyun 08-8-12 {58B76F7C-A35D-4cbb-8948-8163EA3C5191}
            //this.fpSpread1_Sheet1.Rows.Remove(this.fpSpread1_Sheet1.ActiveRowIndex,1);
            this.fpSpread1_Sheet1.RowCount = 0;
            //---------------
            return 1;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Init();
        }

        protected override int OnSave(object sender, object neuObject)
        {
            this.Save();
            this.AddDataToFp(this.QueryExeData(this.ucQueryInpatientNo1.InpatientNo));
            return base.OnSave(sender, neuObject);
        }

        #endregion

        #region 事件

        private void ucQueryInpatientNo1_myEvent()
        {

            if (this.ucQueryInpatientNo1.InpatientNo == null || this.ucQueryInpatientNo1.InpatientNo == string.Empty)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("该患者不存在!请验证后输入"));

                return;
            }

            Neusoft.HISFC.Models.RADT.PatientInfo patientTemp = this.radtManager.GetPatientInfomation(this.ucQueryInpatientNo1.InpatientNo);
            if (patientTemp == null || patientTemp.ID == null || patientTemp.ID == string.Empty)
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("该患者不存在!请验证后输入"));

                return;
            }

            if (patientTemp.PVisit.InState.ID.ToString() == "N")
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("该患者已经无费退院，不允许收费!"));

                //this.Clear();
                this.ucQueryInpatientNo1.Focus();

                return;
            }

            if (patientTemp.PVisit.InState.ID.ToString() == "O")
            {
                MessageBox.Show(Neusoft.FrameWork.Management.Language.Msg("该患者已经出院结算，不允许收费!"));

                //this.Clear();
                this.ucQueryInpatientNo1.Focus();

                return;
            }
            this.patientInfo = patientTemp;

            this.txtName.Text = this.patientInfo.Name;
            this.txtPact.Text = this.patientInfo.Pact.Name;
            this.AddDataToFp(this.QueryExeData(this.patientInfo.ID));
        }


        #endregion

        private int AddDetailToFp(Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail tecDetail)
        {
            //可以一次取消多条 by yuyun 08-8-12 {58B76F7C-A35D-4cbb-8948-8163EA3C5191}
            int rowCount = this.fpSpread1_Sheet1.RowCount;
            this.fpSpread1_Sheet1.Rows.Add(rowCount, 1);
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.MoOrder].Text = tecDetail.MoOrder;
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.ExecMoOrder].Text = tecDetail.ExecMoOrder;
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.ApplyNum].Text = tecDetail.Sequence;
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.ItemID].Text = tecDetail.Apply.Item.ID;
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.ItemName].Text = tecDetail.Apply.Item.Name;
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.ConfirmQty].Text = tecDetail.Apply.Item.ConfirmedQty.ToString();
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.OperCode].Text = tecDetail.Apply.Item.ConfirmOper.ID;
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.OperDept].Text = tecDetail.Apply.ConfirmOperEnvironment.Dept.ID;
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.OperTime].Text = tecDetail.Apply.ConfirmOperEnvironment.OperTime.ToString();
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.RecipeNo].Text = tecDetail.Apply.Item.RecipeNO;
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.SequenceNo].Text = tecDetail.Apply.Item.SequenceNO.ToString();
            //by yuyun 08-7-7
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.Operator].Text = tecDetail.Oper.ID;//技师：默认是当前操作员，可以修改
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.Machine].Text = tecDetail.ExecDevice;//项目使用设备：从医技载体表进行查找
            this.fpSpread1_Sheet1.Cells[rowCount, (int)Cols.MoOrder].Tag = tecDetail;
            this.fpSpread1_Sheet1.Rows[rowCount].Tag = tecDetail;
            //------------{58B76F7C-A35D-4cbb-8948-8163EA3C5191}

            return 1;
        }

        private enum Cols
        {
            MoOrder,//0
            ExecMoOrder,//1
            ApplyNum,//2
            ItemID,//3
            ItemName,//4
            ConfirmQty,//5
            OperCode,//6
            OperDept,//7
            OperTime,//8
            RecipeNo,//9
            SequenceNo,//10
            //by yuyun 08-7-7
            Operator,//11技师：默认是当前操作员，可以修改
            Machine//12项目使用设备：从医技载体表进行查找
            //by yuyun 08-7-7
        }

        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            #region 处理组合项目{5C2A9C83-D165-434c-ACA4-86F23E956442}
            Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail confirmDetail = this.fpSpread1_Sheet1.Rows[e.Row].Tag as Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail;
            if (!string.IsNullOrEmpty(confirmDetail.User01))
            {
                int rowCount = this.fpSpread1_Sheet1.RowCount;
                for (int i = rowCount - 1; i >= 0; i--)
                {
                    Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail tmpConfirmDetail = this.fpSpread1_Sheet1.Rows[i].Tag as Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail;
                    if (tmpConfirmDetail.User01 == confirmDetail.User01)
                    {
                        this.fpSpread1_Sheet1.RemoveRows(i, 1);
                    }
                }
                return;
            }
            #endregion

            this.fpSpread1_Sheet1.RemoveRows(this.fpSpread1_Sheet1.ActiveRowIndex, 1);
        }

        private void neuSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            #region 处理组合项目{5C2A9C83-D165-434c-ACA4-86F23E956442}
            int rowIndex = e.Row;
            //选中组合项目明细-查找组合节点进行处理
  //          if (this.neuSpread1_Sheet1.RowHeader.Cells[e.Row, 0].Text == ".")
  //          {
  //              for (int i = 0; i < this.neuSpread1_Sheet1.RowCount; i++)
  //              {
  //                  Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail confirmDetail = this.neuSpread1_Sheet1.Rows[e.Row].Tag as Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail;
  //                  if (this.neuSpread1_Sheet1.Cells[i, 0].Text == confirmDetail.MoOrder && //医嘱号 
  //                      this.neuSpread1_Sheet1.Cells[i, 1].Text == confirmDetail.ExecMoOrder //医嘱执行号
  //)
  //                  {
  //                      rowIndex = i;
  //                      break;
  //                  }
  //              }
  //          }
            if (string.IsNullOrEmpty(this.neuSpread1_Sheet1.Cells[rowIndex, 2].Text))
            {
                for (int i = 0; i < this.neuSpread1_Sheet1.RowCount; i++)
                {
                    Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail confirmDetail = this.neuSpread1_Sheet1.Rows[i].Tag as Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail;
                    if (confirmDetail == null)
                    {
                        continue;
                    }
                    if (this.neuSpread1_Sheet1.Cells[rowIndex, 0].Text == confirmDetail.MoOrder && //医嘱号 
                        this.neuSpread1_Sheet1.Cells[rowIndex, 1].Text == confirmDetail.ExecMoOrder //医嘱执行号
                       )
                    {
                        //判断是否存在重复记录
                        foreach (FarPoint.Win.Spread.Row r in this.fpSpread1_Sheet1.Rows)
                        {
                            if (this.fpSpread1_Sheet1.Rows[r.Index].Tag == confirmDetail)
                            {
                                MessageBox.Show("该行已被选择过");

                                return;
                            }
                        }
                        //医嘱执行号+组合项目编码  --方便取消
                        confirmDetail.User01 = this.neuSpread1_Sheet1.Cells[rowIndex, 3].Text + this.neuSpread1_Sheet1.Cells[rowIndex, 1].Text;
                        AddDetailToFp(confirmDetail);
                    }
                }
                return;
            }

            #endregion

            //可以一次取消多条 by yuyun 08-8-12 {58B76F7C-A35D-4cbb-8948-8163EA3C5191}
            //this.fpSpread1_Sheet1.RowCount = 0;
            //---------------------------------------
            int RowIndex = this.neuSpread1_Sheet1.ActiveRowIndex;

            Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail tecDetail = new Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail();

            tecDetail = this.neuSpread1_Sheet1.ActiveRow.Tag as Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail;
            //可以一次取消多条 by yuyun 08-8-12 {58B76F7C-A35D-4cbb-8948-8163EA3C5191}
            //判断是否存在重复记录
            foreach (FarPoint.Win.Spread.Row r in this.fpSpread1_Sheet1.Rows)
            {
                if (this.fpSpread1_Sheet1.Rows[r.Index].Tag == tecDetail)
                {
                    MessageBox.Show("该行已被选择过");

                    return;
                }
            }
            //---------------------------------------
            AddDetailToFp(tecDetail);
        }

        /// <summary>
        /// 单元格单击，处理组套项目折叠{5C2A9C83-D165-434c-ACA4-86F23E956442}
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void neuSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.RowHeader && this.neuSpread1_Sheet1.Cells[e.Row, 2].Text == "")
            {
                this.ExpendOrCloseRows(e.Row);
            }
        }

        /// <summary>
        /// 处理组套项目折叠{5C2A9C83-D165-434c-ACA4-86F23E956442}
        /// </summary>
        /// <param name="rowIndex"></param>
        private void ExpendOrCloseRows(int rowIndex)
        {
            if (!string.IsNullOrEmpty(this.neuSpread1_Sheet1.Cells[rowIndex, 2].Text))
            {
                return;
            }
            bool isExpend = true;
            if (this.neuSpread1_Sheet1.RowHeader.Cells[rowIndex, 0].Text == "+")
            {
                this.neuSpread1_Sheet1.RowHeader.Cells[rowIndex, 0].Text = "-";
                isExpend = true;
            }
            else
            {
                this.neuSpread1_Sheet1.RowHeader.Cells[rowIndex, 0].Text = "+";
                isExpend = false;
            }

            for (int i = 0; i < this.neuSpread1_Sheet1.RowCount; i++)
            {
                Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail confirmDetail = this.neuSpread1_Sheet1.Rows[i].Tag as Neusoft.HISFC.Models.Terminal.TerminalConfirmDetail;
                if (confirmDetail == null)
                {
                    continue;
                }
                if (this.neuSpread1_Sheet1.Cells[rowIndex, 0].Text == confirmDetail.MoOrder && //医嘱号 
                    this.neuSpread1_Sheet1.Cells[rowIndex, 1].Text == confirmDetail.ExecMoOrder &&//医嘱执行号
                    this.neuSpread1_Sheet1.RowHeader.Cells[i, 0].Text == ".")
                {
                    this.neuSpread1_Sheet1.Rows[i].Visible = isExpend;
                }
            }

        }

    }
}

