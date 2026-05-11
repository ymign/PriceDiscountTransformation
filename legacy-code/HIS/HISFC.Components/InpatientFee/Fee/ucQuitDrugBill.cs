using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.HISFC.Components.InpatientFee.Fee
{
    /// <summary>
    /// 退费申请单
    /// </summary>
    public partial class ucQuitDrugBill : UserControl, Neusoft.HISFC.BizProcess.Interface.FeeInterface.IBackFeeApplyPrint
    {
        /// <summary>
        /// 退费申请单构造函数
        /// </summary>
        public ucQuitDrugBill()
        {
            InitializeComponent();
            unDrugList.AddRange(new string[] { "UL", "UC", "UZ", "UO" });
        }
        Neusoft.HISFC.Models.RADT.PatientInfo patient;        
         /// <summary>
        /// 非药品
        /// </summary>
        List<string> unDrugList = new List<string>();
       
        bool isReprint = false;
        #region IBackFeeRecipePrint 成员
        /// <summary>
        /// 是否补打
        /// </summary>
        public bool IsRePrint
        {
            get { return isReprint; }
            set { isReprint = value; }
        }
        public Neusoft.HISFC.Models.RADT.PatientInfo Patient
        {
            set
            {
                patient = value;
            }

        }
        /// <summary>
        /// 打印
        /// </summary>
        public void Print()
        {
            Neusoft.FrameWork.WinForms.Classes.Print print = new Neusoft.FrameWork.WinForms.Classes.Print();
            Neusoft.HISFC.BizLogic.Manager.PageSize pageSet = new Neusoft.HISFC.BizLogic.Manager.PageSize();
            Neusoft.HISFC.Models.Base.PageSize ps = pageSet.GetPageSize("QuitDrugBill");
            if (ps == null)
            {
                //默认大小
                ps = new Neusoft.HISFC.Models.Base.PageSize("PrepayPrint", 880, 450);
            }
            print.ControlBorder = Neusoft.FrameWork.WinForms.Classes.enuControlBorder.None;
            print.SetPageSize(ps);
            if (((Neusoft.HISFC.Models.Base.Employee)Neusoft.FrameWork.Management.Connection.Operator).IsManager)
            {
                print.PrintPreview(ps.Left, ps.Top, this);
            }
            else
            {
                print.PrintPage(ps.Left, ps.Top, this);
            }
        }

        public int SetData(System.Collections.ArrayList alBackFee)
        {
            //Neusoft.HISFC.BizLogic.Registration.Register registerIntegrate = new Neusoft.HISFC.BizLogic.Registration.Register();

            //patient = registerIntegrate.GetByClinic(this.patient.ID);
            this.lbRePrint.Visible = this.IsRePrint;     //是否补打
            this.lbName.Text = "姓名：" + patient.Name;
            this.lbCardNo.Text = "住院号：" + patient.PID.PatientNO;
            this.lbSex.Text = "性别：" + patient.Sex.Name;
            this.lbDeptName.Text = "科室：" + patient.PVisit.PatientLocation.Dept.Name;
            this.labArea.Text = "病区：" + patient.PVisit.PatientLocation.NurseCell;   //病区
            this.labPrintDate.Text = "打印时间" + DateTime.Now.ToString();  //打印时间

            decimal sum = 0;
            this.neuSpread1_Sheet1.Rows.Count = 0;

            this.neuSpread1_Sheet1.Rows.Add(0, alBackFee.Count + 1);



            for (int i = 0; i < alBackFee.Count; i++)
            {
                Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList feeItemList = (Neusoft.HISFC.Models.Fee.Inpatient.FeeItemList)alBackFee[i];


                this.neuSpread1_Sheet1.Cells[i, 0].Text = feeItemList.Item.Name.ToString();
                if (feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                {
                    this.neuSpread1_Sheet1.Cells[i, 1].Text = feeItemList.Item.Specs.ToString();
                }
                //if (feeItemList.Order.DoseUnit == feeItemList.Item.PriceUnit)
                //{
                //    this.neuSpread1_Sheet1.Cells[i, 2].Text = feeItemList.Item.Qty.ToString();
                //}
                //else
                //{
                //    this.neuSpread1_Sheet1.Cells[i, 2].Text = Neusoft.FrameWork.Public.String.FormatNumber(feeItemList.Item.Qty / feeItemList.Item.PackQty, 2).ToString();
                //}
                this.neuSpread1_Sheet1.Cells[i, 2].Text = feeItemList.Item.Qty.ToString();
                this.neuSpread1_Sheet1.Cells[i, 3].Text = feeItemList.Item.PriceUnit;
                this.neuSpread1_Sheet1.Cells[i, 4].Text = Neusoft.FrameWork.Public.String.FormatNumber(feeItemList.Item.Qty / feeItemList.Item.PackQty * feeItemList.Item.Price, 2).ToString();
                this.neuSpread1_Sheet1.Cells[i, 5].Text = feeItemList.User02.ToString();    //方号
                //this.neuSpread1_Sheet1.Cells[i, 6].Text = feeItemList.Order.Item.SysClass.ID.ToString();  //来源
                this.neuSpread1_Sheet1.Cells[i, 7].Text = feeItemList.User01.ToString();  //备注
                this.neuSpread1_Sheet1.Cells[i, 8].Text = feeItemList.ExecOrder.User01;  // 应执行时间
                this.neuSpread1_Sheet1.Cells[i, 9].Text = feeItemList.User03;  // 申请日期

                try
                {
                    #region 来源--有问题
                    //是否需要原单
                    bool isNeedBill = false;
                    //判断最小费用代码
                    //switch (feeItemList.Item.MinFee.ID.ToString())
                    //{
                    //    case "004":		//化验
                    //    case "005":		//检查
                    //    case "006":		//放射
                    //    case "013":		//MR
                    //    case "014":		//CT
                    //        isNeedBill = true;
                    //        break;
                    //    default:
                    //        isNeedBill = false;
                    //        break;
                    //}
                    if (unDrugList.Contains(feeItemList.Order.Item.SysClass.ID.ToString()))
                    {
                        isNeedBill = true;
                    }

                    Neusoft.HISFC.BizLogic.Fee.Item undrugMgr = new Neusoft.HISFC.BizLogic.Fee.Item();
                    Neusoft.HISFC.Models.Fee.Item.Undrug undrug = null;		//非药品项目信息
                    //if (!feeItemList.Item.IsPharmacy)
                    if (feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug)
                    {
                        //undrug = undrugMgr.GetItemAll(feeItemList.Item.ID);
                        undrug = undrugMgr.Query(feeItemList.Item.ID, "1")[0] as Neusoft.HISFC.Models.Fee.Item.Undrug;
                        if (undrug == null || undrug.ID == "")
                        {
                            System.Windows.Forms.MessageBox.Show("由项目字典表内未找到非药品信息");
                            return -1;
                        }
                    }

                    if (feeItemList.ExtFlag2.Length > 0)   //这样走不过去，又不知道ExtFlag2是什么
                    {
                        switch (feeItemList.ExtFlag2.Substring(0, 1))
                        {
                            case "0":
                                //此处通过系统类别判断 不使用最小费用代码
                                if (feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug && undrug != null)
                                {
                                    if (undrug.SysClass.ID.ToString() == "UL")
                                    {
                                        if (feeItemList.ItemGroup.ID != null)
                                        {
                                            this.neuSpread1_Sheet1.Cells[i, 6].Text = "直接记帐需原单[" + feeItemList.ItemGroup.ID + "]";
                                        }
                                        else
                                        {
                                            this.neuSpread1_Sheet1.Cells[i, 6].Text = "直接记帐、需送原单";
                                        }
                                        return 1;
                                    }
                                }
                                this.neuSpread1_Sheet1.Cells[i, 6].Text = "直接记帐";
                                break;
                            case "1":
                                //此处通过系统类别判断 不使用最小费用代码
                                if (feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.UnDrug && undrug != null)
                                {
                                    if (undrug.SysClass.ID.ToString() == "UL")
                                    {
                                        if (feeItemList.ItemGroup.ID != null)
                                        {
                                            this.neuSpread1_Sheet1.Cells[i, 6].Text = "直传需原单[" + feeItemList.ItemGroup.ID + "]";
                                        }
                                        else
                                        {
                                            this.neuSpread1_Sheet1.Cells[i, 6].Text = "病区直传、需送原单";
                                        }
                                        return 1;
                                    }
                                }
                                this.neuSpread1_Sheet1.Cells[i, 6].Text = "病区直传" + feeItemList.ExecOper.OperTime.ToString("MM-dd hh:mm");
                                break;
                            case "2":
                                // if (feeItemList.SendState == "1")
                                if (feeItemList.PayType == Neusoft.HISFC.Models.Base.PayTypes.Balanced)
                                {
                                    this.neuSpread1_Sheet1.Cells[i, 6].Text = "确认记帐、需送原单";
                                }
                                else
                                {
                                    if (isNeedBill)		//需送原单
                                    {
                                        this.neuSpread1_Sheet1.Cells[i, 6].Text = "确认记帐、需送原单";
                                    }
                                    else
                                    {
                                        this.neuSpread1_Sheet1.Cells[i, 6].Text = "确认记帐";
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                }
                catch (Exception){}                
                sum += Neusoft.FrameWork.Public.String.FormatNumber(feeItemList.Item.Qty / feeItemList.Item.PackQty * feeItemList.Item.Price, 2);
            }
            this.neuSpread1_Sheet1.Cells[alBackFee.Count, 0].Text = "合计：";
            this.neuSpread1_Sheet1.Cells[alBackFee.Count, 4].Text = sum.ToString();
            return 1;
        }

        #endregion
    }
}
