using BarcodeLib;
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using FarPoint.Win.Spread.Model;
using Neusoft.FrameWork.Function;
using Neusoft.FrameWork.Management;
using Neusoft.FrameWork.WinForms.Classes;
using Neusoft.FrameWork.WinForms.Controls;

//using Neusoft.HISFC.BizLogic.Manager;
using Neusoft.HISFC.BizProcess.Integrate.Common;
using Neusoft.HISFC.BizProcess.Interface.Fee;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.Fee;
using Neusoft.HISFC.Models.Fee.Item;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.HISFC.Models.Registration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientGuide
{
    public partial class ucMZGuideElecBillPrint : UserControl
    {

        public ucMZGuideElecBillPrint()
        {
            InitializeComponent();
        }
        //  private class IcompareFeeCodeStat : IComparer<Neusoft.HISFC.Models.Fee.FeeCodeStat>
        //{
        //    public int Compare(Neusoft.HISFC.Models.Fee.FeeCodeStat x, Neusoft.HISFC.Models.Fee.FeeCodeStat y)
        //    {
        //        return x.get_SortID().CompareTo(y.get_SortID());
        //    }
        //}
        private const string LabAddress = "医技楼4楼检验科";
        private Neusoft.HISFC.BizLogic.Manager.PageSize pageSizeManager = new Neusoft.HISFC.BizLogic.Manager.PageSize();
        private Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlIntegrate = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
        private Neusoft.HISFC.BizLogic.Manager.Constant constManager = new Neusoft.HISFC.BizLogic.Manager.Constant();
        private Interface interfaceManager = new Interface();
        private Neusoft.HISFC.Models.Fee.FeeCodeStat feeCodeStatManager = new FeeCodeStat();
        // private Neusoft.HISFC.BizLogic.Fee.UndrugPackAge packageManager = new Neusoft.HISFC.BizLogic.Fee.UndrugPackAge.UndrugPackAge();

        /// <summary>
        /// 门诊费用业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        private Neusoft.HISFC.BizLogic.Fee.Item itemManager = new Neusoft.HISFC.BizLogic.Fee.Item();
        private Register Register;
        private string InvoiceNo = string.Empty;
        private bool isRePrint = false;
        private List<MZGuideContrast> ULContrast;
        private List<MZGuideContrast> NotULContrast;//非检验项目非药品执行地点
        private string LabTypeID = "L55";
        private string tempCombID = string.Empty;
        private List<KeyValuePair<string, string>> NumberText = null;
        private IContainer components = null;
        private NeuSpread neuSpread1;
        private SheetView neuSpread1_Sheet1;
        // {097AA15C-C4CB-4d19-B5C0-76EE20C1ACDE} 内镜中心用药单独备注
        private Hashtable hsNJitem = new Hashtable();
        /// <summary>
        /// 常数缓存列表
        /// </summary>
        private static Dictionary<string, ArrayList> dicConList = null;

        private static Image billImg = null;

        /// <summary>
        /// 获取常数列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ArrayList GetConList(string type)
        {
            if (dicConList == null)
            {
                dicConList = new Dictionary<string, ArrayList>();
            }

            if (dicConList.ContainsKey(type))
            {
                return dicConList[type];
            }
            ArrayList alCon = this.constManager.GetList(type);

            if (alCon != null)
            {
                dicConList.Add(type, alCon);
            }
            return alCon;
        }

        // {0EB9E59D-25AD-4dea-A4B3-35A3AB9B829E} 检验采血项目如果某个科室维护了单独的地点，取对照的地点
        Dictionary<string, string> dicLis_Dept = null;

        #region 渲染表头
        /// <summary>
        /// 渲染表头
        /// </summary>
        private void InitHeader()
        {
            this.neuSpread1_Sheet1.RowCount = 0;
            this.neuSpread1_Sheet1.GrayAreaBackColor = Color.White;
            this.neuSpread1_Sheet1.HorizontalGridLine = new GridLine(GridLineType.None);
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.neuSpread1_Sheet1.SheetCornerHorizontalGridLine = new GridLine(GridLineType.None);
            this.neuSpread1_Sheet1.SheetCornerVerticalGridLine = new GridLine(GridLineType.None);
            this.neuSpread1_Sheet1.VerticalGridLine = new GridLine(GridLineType.None);
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = Color.White;
        }
        #endregion

        private void AddSpace()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        }
        private void AddHead()
        {
            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 4, 1, 2);
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Font = new Font("宋体", 6f);
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = "剩余额度:" + this.Register.SIMainInfo.YearCost.ToString() + Environment.NewLine + "累计使用:" + this.Register.SIMainInfo.AddTotCost.ToString();
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].HorizontalAlignment = CellHorizontalAlignment.Center;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 12f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = this.constManager.GetHospitalName();//门诊指引单
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].HorizontalAlignment = CellHorizontalAlignment.Center;
        }
        private void AddPatientInfo()
        {
            ImageCellType cellType = new ImageCellType();
            cellType.Style = RenderStyle.StretchAndScale;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 5);
            Image im = CreateBarCode(this.Register.Card.ID);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].CellType = cellType;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Value = im;//条码
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].HorizontalAlignment = CellHorizontalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].VerticalAlignment = CellVerticalAlignment.Top;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 60;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 3, 1, 3);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            if (this.isRePrint)
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "发票号:" + this.InvoiceNo + "(补打)";
            }
            else
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "发票号:" + this.InvoiceNo;
            }
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].HorizontalAlignment = CellHorizontalAlignment.Left;

            Neusoft.HISFC.BizLogic.Registration.Register regMgr1 = new Neusoft.HISFC.BizLogic.Registration.Register();
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Font = new Font("宋体", 7.5f);
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = "年龄:" + regMgr1.GetAge(this.Register.Birthday);
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = CellHorizontalAlignment.Left;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 4);
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 3, 1, 3);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "姓名:" + this.Register.Name + "[" + (this.Register.Sex == null ? "未填" : this.Register.Sex.ToString()) + regMgr1.GetAge(this.Register.Birthday) + "]";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].HorizontalAlignment = CellHorizontalAlignment.Left;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 4);
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 3, 1, 3);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "就诊号：" + this.Register.Card.ID;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].HorizontalAlignment = CellHorizontalAlignment.Left;

            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 2, 4, 2, 2);
            //ImageCellType cellType = new ImageCellType();
            ////this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 30;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 2, 4].CellType = cellType;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 2, 4].Value = this.CreateBarCode(this.Register.Card.ID);
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 2, 4].HorizontalAlignment = CellHorizontalAlignment.Left;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "就诊日期:" + this.Register.DoctorInfo.SeeDate.ToString("yyyy-MM-dd");

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "就诊科室:" + this.Register.DoctorInfo.Templet.Dept.ToString();

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "就诊医生:" + this.Register.DoctorInfo.Templet.Doct.ToString();
            ArrayList comBalances = new ArrayList();
            comBalances = outpatientManager.QueryBalancesSameInvoiceCombNOByInvoiceNO(this.InvoiceNo);
            decimal totCost = 0, ownCost = 0, payCost = 0, pubCost = 0;
            string strPayMode = "";
            foreach (Balance balance in comBalances)
            {
                totCost += balance.FT.TotCost;
                ownCost += balance.FT.OwnCost;
                payCost += balance.FT.PayCost;
                pubCost += balance.FT.PubCost;

                ArrayList payModes = outpatientManager.QueryBalancePaysByInvoiceSequence(balance.CombNO);
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.BalancePay payMode in payModes)
                {
                    strPayMode += " " + GetPayModeName(payMode.PayType.ID);
                }
            }
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "收费总额:￥" + totCost + "元";

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "自费金额:￥" + ownCost + "元";

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "自付金额:￥" + payCost + "元";
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "报销金额:￥" + pubCost + "元";

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "支付方式:" + strPayMode;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 10f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "打印时间:" + DateTime.Now.ToString();
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].HorizontalAlignment = CellHorizontalAlignment.Left;
        }
        //private void AddFeeHeader()
        //{
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 2);
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 4, 1, 2);
        //    ComplexBorder border = new ComplexBorder(new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.ThinLine), new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.ThinLine));
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "项目名称[规格]";
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Border = border;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].VerticalAlignment = CellVerticalAlignment.Center;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Text = "单价";
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Border = border;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].HorizontalAlignment = CellHorizontalAlignment.Right;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].VerticalAlignment = CellVerticalAlignment.Center;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = "数量";
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Border = border;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = CellHorizontalAlignment.Right;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].VerticalAlignment = CellVerticalAlignment.Center;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = "金额";
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Border = border;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].HorizontalAlignment = CellHorizontalAlignment.Right;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].VerticalAlignment = CellVerticalAlignment.Center;
        //    this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 8f, FontStyle.Bold);
        //}
        //private decimal AddFeeDetailLine(FeeItemList feeItemList)
        //{
        //    this.neuSpread1_Sheet1.RowCount++;
        //    string text = string.Empty;
        //    if (!string.IsNullOrEmpty(feeItemList.get_UndrugComb().get_ID()))
        //    {
        //        if (string.IsNullOrEmpty(this.tempCombID) || !this.tempCombID.Equals(feeItemList.get_UndrugComb().get_ID()))
        //        {
        //            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 3);
        //            this.tempCombID = feeItemList.get_UndrugComb().get_ID();
        //            UndrugComb undrugComb = this.packageManager.GetUndrugComb(feeItemList.get_UndrugComb().get_ID(), feeItemList.get_Item().get_ID());
        //            if (undrugComb != null)
        //            {
        //                Item undrugByCode = this.itemManager.GetUndrugByCode(feeItemList.get_UndrugComb().get_ID());
        //                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = undrugByCode.get_Name();
        //                if (undrugComb.get_Qty() == 0m)
        //                {
        //                    undrugComb.set_Qty(1m);
        //                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = (feeItemList.get_Item().get_Qty() / undrugComb.get_Qty()).ToString() + ("无".Equals(undrugByCode.get_PriceUnit()) ? "" : undrugByCode.get_PriceUnit());
        //                }
        //                else
        //                {
        //                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = (feeItemList.get_Item().get_Qty() / undrugComb.get_Qty()).ToString() + ("无".Equals(undrugByCode.get_PriceUnit()) ? "" : undrugByCode.get_PriceUnit());
        //                }
        //            }
        //            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = CellHorizontalAlignment.Right;
        //            this.neuSpread1_Sheet1.RowCount++;
        //        }
        //        text += "┗";
        //    }
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        //    if (!string.IsNullOrEmpty(feeItemList.get_Item().get_Specs()))
        //    {
        //        string text2 = text;
        //        text = string.Concat(new string[]
        //        {
        //            text2,
        //            feeItemList.get_Item().get_Name(),
        //            "[",
        //            feeItemList.get_Item().get_Specs(),
        //            "]"
        //        });
        //    }
        //    else
        //    {
        //        text += feeItemList.get_Item().get_Name();
        //    }
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = text;
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 3);
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 4, 1, 2);
        //    if (feeItemList.get_Item().get_PackQty() <= 0m)
        //    {
        //        feeItemList.get_Item().set_PackQty(1m);
        //    }
        //    if ("1".Equals(feeItemList.get_FeePack()))
        //    {
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].CellType = new NumberCellType();
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = feeItemList.get_Item().get_Price().ToString("F2");
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = CellHorizontalAlignment.Right;
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = (feeItemList.get_Item().get_Qty() / feeItemList.get_Item().get_PackQty()).ToString() + feeItemList.get_Item().get_PriceUnit();
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].CellType = new NumberCellType();
        //    }
        //    else
        //    {
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].CellType = new NumberCellType();
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = (feeItemList.get_Item().get_Price() / feeItemList.get_Item().get_PackQty()).ToString("F2");
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = CellHorizontalAlignment.Right;
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = feeItemList.get_Item().get_Qty().ToString() + feeItemList.get_Item().get_PriceUnit();
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].CellType = new NumberCellType();
        //    }
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = CellHorizontalAlignment.Right;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = (feeItemList.get_FT().get_PubCost() + feeItemList.get_FT().get_OwnCost() + feeItemList.get_FT().get_PayCost()).ToString("F2");
        //    this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 7.5f);
        //    return feeItemList.get_FT().get_PubCost() + feeItemList.get_FT().get_OwnCost() + feeItemList.get_FT().get_PayCost();
        //}
        //private void AddFeeTotalLine(string feeName, decimal feeTotal)
        //{
        //    ComplexBorder border = new ComplexBorder(new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.ThinLine));
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 3);
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 3, 1, 3);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = feeName + "小计:";
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = CellHorizontalAlignment.Right;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Border = border;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].CellType = new NumberCellType();
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = feeTotal.ToString("F2");
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Border = border;
        //    this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 7.5f);
        //}
        //private void AddFeeTotal(decimal total)
        //{
        //    Employee employee = Connection.get_Operator() as Employee;
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 2);
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 3, 1, 3);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "打印:" + employee.get_ID();
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = CellHorizontalAlignment.Center;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Text = "合计:";
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].HorizontalAlignment = CellHorizontalAlignment.Right;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].CellType = new NumberCellType();
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = total.ToString("F2");
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = CellHorizontalAlignment.Right;
        //    this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 7.5f, FontStyle.Bold);
        //}
        //private string GetFeeStatName(ArrayList listFeeStat, string minFeeID)
        //{
        //    string result;
        //    foreach (FeeCodeStat feeCodeStat in listFeeStat)
        //    {
        //        string text = int.Parse(minFeeID).ToString();
        //        string iD = feeCodeStat.get_MinFee().get_ID();
        //        if (feeCodeStat.get_MinFee().get_ID().Equals(minFeeID) || feeCodeStat.get_MinFee().get_ID().Equals(int.Parse(minFeeID).ToString()))
        //        {
        //            result = feeCodeStat.get_StatCate().get_Name();
        //            return result;
        //        }
        //    }
        //    result = "";
        //    return result;
        //}
        //private HashSet<string> GetStatCateList(ArrayList list)
        //{
        //    HashSet<string> hashSet = new HashSet<string>();
        //    foreach (FeeCodeStat feeCodeStat in list)
        //    {
        //        if (!hashSet.Contains(feeCodeStat.get_StatCate().get_Name()))
        //        {
        //            hashSet.Add(feeCodeStat.get_StatCate().get_Name());
        //        }
        //    }
        //    return hashSet;
        //}
        //private void AddFee(ArrayList alFeeDetail)
        //{
        //    ArrayList arrayList = this.feeCodeStatManager.QueryFeeCodeStatByReportCode("MZ01");
        //    decimal num = 0m;
        //    HashSet<string> statCateList = this.GetStatCateList(arrayList);
        //    foreach (string current in statCateList)
        //    {
        //        decimal num2 = 0m;
        //        foreach (FeeItemList feeItemList in alFeeDetail)
        //        {
        //            string feeStatName = this.GetFeeStatName(arrayList, feeItemList.get_Item().get_MinFee().get_ID());
        //            if (!(current == "") && current.Equals(feeStatName))
        //            {
        //                num2 += this.AddFeeDetailLine(feeItemList);
        //            }
        //        }
        //        if (num2 != 0m)
        //        {
        //            this.AddFeeTotalLine(current, num2);
        //            num += num2;
        //        }
        //    }
        //    this.AddFeeTotal(num);
        //}
        //private void AddHospitalInfo()
        //{
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8f, FontStyle.Bold);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = this.constManager.GetHospitalName();
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = CellHorizontalAlignment.Left;
        //}
        //private void AddName(string name, DateTime feeDate)
        //{
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8f, FontStyle.Bold);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "姓名:" + name;
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = " 收费日期:" + feeDate.ToShortDateString();
        //}
        //private void AddNameAndAge(string name, string sexName, string age, DateTime feeDate)
        //{
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8f, FontStyle.Bold);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = string.Concat(new string[]
        //    {
        //        "姓名:",
        //        name,
        //        "  ",
        //        sexName,
        //        age
        //    });
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = " 收费日期:" + feeDate.ToShortDateString();
        //}
        //private decimal AddCardNOAndInvoice(string cardNO, ArrayList alInvoice)
        //{
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8f, FontStyle.Bold);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "病历号:" + cardNO;
        //    decimal num = 0m;
        //    foreach (Balance balance in alInvoice)
        //    {
        //        num += balance.get_FT().get_TotCost();
        //        if (!(balance.get_Invoice().get_Memo() == "5"))
        //        {
        //            this.neuSpread1_Sheet1.RowCount++;
        //            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        //            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = " 发票号:" + balance.get_Invoice().get_ID();
        //        }
        //    }
        //    return num;
        //}
        //private void AddDiagnoseInfo(string clinicCode, string diagnoseName)
        //{
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "诊断:" + diagnoseName;
        //}
        //private void AddFeeFooter(decimal TotCost, DateTime dtNow)
        //{
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 5);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "收款员: " + Connection.get_Operator().get_ID() + " 合计: " + TotCost.ToString();
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 5);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "打印日期: " + dtNow.ToShortDateString();
        //}



        private void ULAddLine()
        {
            //FarPoint.Win.Spread.CellType.TextCellType tc = new TextCellType();
            //tc.WordWrap = true;
            //tc.Multiline = true;
            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            //this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 30;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].CellType = tc;           
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "注：请当天缴费，从开单之日起30天内完成检查，逾期作废。";
        }

        private void AddLine()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "------------------------------------------";
        }
        private void AddRealLine()
        {
            ComplexBorder border = new ComplexBorder(new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.ThinLine), new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.None));
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "------------------------------------------";
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Border = border;
        }
        private void AddHospitalTipForUL()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8f);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = this.constManager.GetHospitalName() + "门诊检验项目温馨提示单";
        }
        private void AddHospitalTipForUC()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8f);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = this.constManager.GetHospitalName() + "门诊检查项目温馨提示单";
        }

        private void AddList(Register register, ArrayList balanceList, List<MZGuide> itemList)
        {
            if (itemList != null || itemList.Count != 0)
            {
                this.InvoiceNo = itemList[0].InvoiceNo;
                register.Card.ID = new Neusoft.HISFC.BizLogic.Fee.MZGuide().GetCardNoByClincCode(register.ID);
                string iD = register.Card.ID.ToString().PadLeft(10, '0');
                register.Card.ID = iD;
                this.Register = register;
                //begin检验项目获取维护的检验项目执行地址
                if ((
                    from x in itemList
                    where x.Class_Code == "UL"
                    select x).ToList<MZGuide>().Count > 0)
                {
                    this.InitULContrast();
                    itemList = this.ExecUlAddress(itemList);
                }
                //end 
                #region 指引单非药品非检验项目获取执行地址
                if ((
                   from x in itemList
                   where x.Class_Code == "UZ"
                   select x).ToList<MZGuide>().Count > 0 ||
                   (
                   from x in itemList
                   where x.Class_Code == "UO"
                   select x).ToList<MZGuide>().Count > 0 ||
                    (
                   from x in itemList
                   where x.Class_Code == "U"
                   select x).ToList<MZGuide>().Count > 0 ||
                    (
                   from x in itemList
                   where x.Class_Code == "UN"
                   select x).ToList<MZGuide>().Count > 0 ||
                    (
                   from x in itemList
                   where x.Class_Code == "MF"
                   select x).ToList<MZGuide>().Count > 0 ||
                    (
                   from x in itemList
                   where x.Class_Code == "UT"
                   select x).ToList<MZGuide>().Count > 0 ||
                    (
                   from x in itemList
                   where x.Class_Code == "M"
                   select x).ToList<MZGuide>().Count > 0 ||
                     (
                   from x in itemList
                   where x.Class_Code == "MC"
                   select x).ToList<MZGuide>().Count > 0 ||
                     (
                   from x in itemList
                   where x.Class_Code == "UC"
                   select x).ToList<MZGuide>().Count > 0
                   )
                {
                    this.InitNotULContrast();
                    itemList = this.ExecNotUlAddress(itemList);
                }
                #endregion

                this.AddHead();
                string empty = string.Empty;
                DateTime now = DateTime.Now;
                List<MZGuideSpecialExecDept> splist = new List<MZGuideSpecialExecDept>();
                splist = new Neusoft.HISFC.BizLogic.Fee.MZGuide().QueryGuideSpecialDept();
                this.AddSpecialItem(itemList, splist);
                this.AddRealLine();
                this.SetData(itemList);
            }
        }
        public void SetValue(Register register, ArrayList balanceList, List<MZGuide> itemList, Image img)
        {
            if (itemList != null && itemList.Count != 0)
            {
                if (register.User02 == "1")
                {
                    this.isRePrint = true;
                }
                billImg = img;
                this.InitHeader();
                this.SetNumberText();
                this.InitDicLis();
                this.AddList(register, balanceList, itemList);
            }
        }
        public void Print()
        {
            PageSize pageSize = this.GetPageSize();
            if (pageSize == null)
            {
                pageSize = new PageSize("OutPatientGuide", 86, this.GetHeigth());
            }
            Print print = new Print();
            print.SetPageSize(pageSize);
            string controlParam = this.controlIntegrate.GetControlParam<string>("MZGuide", true, "");
            if (!string.IsNullOrEmpty(controlParam))
            {
                print.PrintDocument.PrinterSettings.PrinterName = controlParam;
            }
            print.ControlBorder = 0;
            print.PrintPreview(pageSize.Left, pageSize.Top, this);
        }
        private void PrintOnePage()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 9f;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "  .  ";
            Print print = new Print();
            PageSize pageSize = this.GetPageSize();
            print.SetPageSize(pageSize);
            string controlParam = this.controlIntegrate.GetControlParam<string>("MZGuide", true, "");
            if (!string.IsNullOrEmpty(controlParam))
            {
                print.PrintDocument.PrinterSettings.PrinterName = controlParam;
            }
            else
            {
                print.PrintDocument.PrinterSettings.PrinterName = "MZGuide";
            }
            print.ControlBorder = 0;
            if (print.PrintPage(pageSize.Left, 0, this) == -1)
            {

                if (MessageBox.Show("是否尝试重新打印？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    this.PrintOnePage();
                }

            }
            //this.neuSpread1_Sheet1.Rows.Remove(0, this.neuSpread1_Sheet1.Rows.Count);
        }
        public PageSize GetPageSize()
        {
            PageSize pageSize = this.pageSizeManager.GetPageSize("MZGuide");
            if (pageSize == null)
            {
                pageSize = new PageSize("MZGuide", 310, 260);
            }
            pageSize.Height = this.GetHeigth();
            return pageSize;
        }
        public int GetHeigth()
        {
            int num = NConvert.ToInt32((float)this.neuSpread1_Sheet1.RowCount * this.neuSpread1_Sheet1.Rows.Default.Height) + 20;
            return (base.Height > num) ? (base.Height + 5) : num;
        }

        private void SetNumberText()
        {
            if (this.NumberText == null)
            {
                this.NumberText = new List<KeyValuePair<string, string>>();
            }
            if (this.NumberText.Count > 0)
            {
                this.NumberText.Clear();
            }
            this.NumberText.Add(new KeyValuePair<string, string>("1", "1"));
            this.NumberText.Add(new KeyValuePair<string, string>("2", "2"));
            this.NumberText.Add(new KeyValuePair<string, string>("3", "3"));
            this.NumberText.Add(new KeyValuePair<string, string>("4", "4"));
            this.NumberText.Add(new KeyValuePair<string, string>("5", "5"));
            this.NumberText.Add(new KeyValuePair<string, string>("6", "6"));
            this.NumberText.Add(new KeyValuePair<string, string>("7", "7"));
            this.NumberText.Add(new KeyValuePair<string, string>("8", "8"));
            this.NumberText.Add(new KeyValuePair<string, string>("9", "9"));
            this.NumberText.Add(new KeyValuePair<string, string>("10", "10"));
            this.NumberText.Add(new KeyValuePair<string, string>("11", "11"));
            this.NumberText.Add(new KeyValuePair<string, string>("12", "12"));
            this.NumberText.Add(new KeyValuePair<string, string>("13", "13"));
            this.NumberText.Add(new KeyValuePair<string, string>("14", "14"));
            this.NumberText.Add(new KeyValuePair<string, string>("15", "15"));
        }
        private void SetData(List<MZGuide> ItemList)
        {
            this.AddPatientInfo();
            this.AddElcImg();
            //this.AddRealLine();
            List<string> list = new List<string>();
            foreach (MZGuide current in ItemList)
            {
                if (!list.Contains(current.Address))
                {
                    list.Add(current.Address);
                }
            }
            foreach (string item in list)
            {
                List<MZGuide> list2 = (
                    from x in ItemList
                    where x.Address == item
                    select x).ToList<MZGuide>();
                if (list2.Count > 0)
                {
                    this.SetGuideList(list2);
                }
            }
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.RowCount++;
            this.AddLine();
            this.PrintOnePage();
        }
        private void SetGuideList(List<MZGuide> ItemList)
        {
            string b = string.Empty;
            IOrderedEnumerable<MZGuide> orderedEnumerable =
                from x in
                    (
                        from x in ItemList
                        where x.Drug_Flag == "1"
                        select x).ToList<MZGuide>()
                orderby x.Exec_Dpcd
                select x;
            IOrderedEnumerable<MZGuide> orderedEnumerable2 =
                from x in
                    (
                        from x in ItemList
                        where x.Drug_Flag == "9"
                        select x).ToList<MZGuide>()
                orderby x.Exec_Dpcd
                select x;
            IOrderedEnumerable<MZGuide> orderedEnumerable3 =
                from x in
                    (
                        from x in ItemList
                        where x.Drug_Flag == "2"
                        select x).ToList<MZGuide>()
                orderby x.Address
                select x;
            bool flag = true;

            foreach (MZGuide current in orderedEnumerable)
            {
                if (current.Exec_Dpcd != b)
                {
                    if (!flag)
                    {
                        this.AddLine();
                        //this.PrintOnePage();
                    }
                    b = current.Exec_Dpcd;
                    //this.AddPatientInfo();
                    this.AddRealLine();
                    this.AddGuideTips(current);
                    this.AddDrugHeader();
                    if (flag)
                    {
                        flag = false;
                    }
                }
                this.AddItemToSheet(current);
            }
            //this.AddRealLine();
            b = string.Empty;
            foreach (MZGuide current in orderedEnumerable2)
            {
                if (current.Exec_Dpcd != b)
                {
                    if (!flag)
                    {
                        this.AddLine();
                        //this.PrintOnePage();
                    }
                    b = current.Exec_Dpcd;
                    //this.AddPatientInfo();
                    this.AddRealLine();
                    this.AddGuideTips(current);
                    this.AddCureHeader();
                    if (flag)
                    {
                        flag = false;
                    }
                }
                this.AddItemToSheet(current);
            }
            //this.AddRealLine();
            b = string.Empty;
            foreach (MZGuide current in orderedEnumerable3)
            {
                string tem = current.Address;
                if (string.IsNullOrEmpty(tem))
                    tem = current.Exec_Dpcd;
                if (tem != b)
                {
                    if (!flag)
                    {
                        this.AddLine();
                        // this.PrintOnePage();
                    }
                    b = tem;//current.Exec_Dpcd;
                    //this.AddPatientInfo();
                    this.AddRealLine();
                    this.AddGuideTips(current);
                    this.AddUnDrugHeader(current);
                    if (flag)
                    {
                        flag = false;
                    }
                }
                this.AddItemToSheet(current);
            }
            //this.AddRealLine();
            if ((
                    from x in ItemList
                    where x.Class_Code == "UL"
                    select x).ToList<MZGuide>().Count > 0)
            {

                this.ULAddLine();
            }
            //this.AddLine();
            // this.PrintOnePage();
        }


        private void AddElcImg()
        {
            #region 作废
            //// this.AddRealLine();//加条实线
            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 5, 1, 5, 2);
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 5, 4, 5, 2);
            //ImageCellType cellType = new ImageCellType();
            //cellType.Style = RenderStyle.StretchAndScale;
            //string path = Application.StartupPath;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 0].Column.Width = 10;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 1].Column.Width = 65;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 2].Column.Width = 65;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 3].Column.Width = 1;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 4].Column.Width = 65;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 5].Column.Width = 65;
            //// this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 5].Column.Width = 90;
            ////this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Column.Width = 80;
            ////this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 30;
            //Image im = new Bitmap(Application.StartupPath + @"\电子票据平台二维码.png");
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 1].CellType = cellType;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 1].Value = im;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 1].HorizontalAlignment = CellHorizontalAlignment.Right;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 1].VerticalAlignment = CellVerticalAlignment.Center;




            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 4].CellType = cellType;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 4].Value = billImg;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 4].HorizontalAlignment = CellHorizontalAlignment.Right;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 4].VerticalAlignment = CellVerticalAlignment.Center;//Application.StartupPath





            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 5, 2);
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 4, 5, 2);

            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 7.5f);
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "票据平台二维码";
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].HorizontalAlignment = CellHorizontalAlignment.Center;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].VerticalAlignment = CellVerticalAlignment.Center;

            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Font = new Font("宋体", 7.5f);
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = "电子票二维码";
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].HorizontalAlignment = CellHorizontalAlignment.Center;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].VerticalAlignment = CellVerticalAlignment.Center;

            //FarPoint.Win.Spread.CellType.TextCellType tc1 = new TextCellType();
            //tc1.WordWrap = true;
            //tc1.Multiline = true;
            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.RowCount++;
            //this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 2, 6);
            ////this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 30;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].CellType = tc1;
            //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "温馨提示：请微信扫描左侧二维码,登陆电子票夹,进行扫码查票。"; 
            #endregion
            this.AddLine();

            ImageCellType cellType = new ImageCellType();
            cellType.Style = RenderStyle.StretchAndScale;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 5, 1, 5, 3);
            string path = Application.StartupPath;
            Image im = new Bitmap(Application.StartupPath + @"\电子票据平台二维码.png");
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 1].CellType = cellType;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 1].Value = im;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 1].HorizontalAlignment = CellHorizontalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 1].VerticalAlignment = CellVerticalAlignment.Center;

            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 5, 4, 5, 3);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 4].CellType = cellType;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 4].Value = billImg;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 4].HorizontalAlignment = CellHorizontalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 5, 4].VerticalAlignment = CellVerticalAlignment.Center;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 3);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 9f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = " 票据平台二维码";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].HorizontalAlignment = CellHorizontalAlignment.Left;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].VerticalAlignment = CellVerticalAlignment.Center;

            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 4, 1, 3);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Font = new Font("宋体", 9f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = "扫码查票  ";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].HorizontalAlignment = CellHorizontalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].VerticalAlignment = CellVerticalAlignment.Center;

            FarPoint.Win.Spread.CellType.TextCellType tc1 = new TextCellType();
            tc1.WordWrap = true;
            tc1.Multiline = true;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 40;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].CellType = tc1;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "温馨提示：请微信扫描左侧二维码,登陆电子票夹,进行扫码查票。";


        }

        private void AddCureHeader()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 4);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 5, 1, 2);
            ComplexBorder complexBorder = new ComplexBorder(new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.ThinLine), new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.ThinLine));
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "检查/治疗/手术/化验项目";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].VerticalAlignment = CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].Text = "数量/单位";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].HorizontalAlignment = CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].VerticalAlignment = CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 10f, FontStyle.Bold);
        }
        private void AddDrugHeader()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 4);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 5, 1, 2);
            ComplexBorder complexBorder = new ComplexBorder(new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.ThinLine), new ComplexBorderSide(ComplexBorderSideStyle.None), new ComplexBorderSide(ComplexBorderSideStyle.ThinLine));
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "检查/治疗/手术/化验项目";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].VerticalAlignment = CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].Text = "数量/单位";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].HorizontalAlignment = CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].VerticalAlignment = CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 10f, FontStyle.Bold);
        }
        private void AddUnDrugHeader(MZGuide obj)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 4);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 5, 1, 2);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "检查/治疗/手术/化验项目";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].VerticalAlignment = CellVerticalAlignment.Center;
            if (obj.Class_Code == "UC")
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].Text = "数量/部位";
            }
            else
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].Text = "数量";
            }
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].HorizontalAlignment = CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].VerticalAlignment = CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 10f, FontStyle.Bold);
        }
        /// <summary>
        /// 指引
        /// </summary>
        /// <param name="obj"></param>
        private void AddGuideTips(MZGuide obj)
        {
            string text = string.Empty;
            string empty = string.Empty;
            string text2 = string.Empty;
            string textrecipe = "\r\n处方当日有效，缴费后请当日取药";
            // {097AA15C-C4CB-4d19-B5C0-76EE20C1ACDE} 内镜中心用药单独备注
            string textTemp = "(胃肠镜检查的处方无需在药房取药，请" + "\r\n前往门诊楼四楼内镜中心进行检查预约。)";

            if (!string.IsNullOrEmpty(obj.Address))
            {
                ////if (obj.Address.Contains("门诊二楼采血室") && obj.See_Dpcd == "6005")
                //if (obj.Address.Contains("采血室") && obj.See_Dpcd == "6005")
                //{
                //    text = "医技楼六楼随访室采血";
                //}
                ////else if (obj.Address.Contains("门诊二楼采血室") && obj.See_Dpcd == "6002")
                //else if (obj.Address.Contains("采血室") && obj.See_Dpcd == "6002")
                //{
                //    text = "门诊三楼儿科采血室";
                //}
                // {8861089B-2BBE-4d22-97E8-74D80637E68F}医院现有采血地点有多个（感染楼采血室、门诊楼采血室、急诊采血室、计划免疫采血室），目前HIS系统没有可维护界面，导致指引单显示执行地点不对。
                if (obj.Address.Contains("采血室") && dicLis_Dept.ContainsKey(obj.See_Dpcd))
                {
                    text = dicLis_Dept[obj.See_Dpcd];
                }
                else
                {
                    text = obj.Address;
                }
            }
                ArrayList alCKitem = GetConList("FKitem");
                foreach (Neusoft.HISFC.Models.Base.Const dic in alCKitem)
                {
                    if (obj.Item_Name.Contains(dic.Name))
                    {
                        text = "三楼妇产超声";
                    }
                }
            if (obj.Drug_Flag == "1" && !string.IsNullOrEmpty(obj.Send_Terminal))
            {
                string text3 = obj.Send_Terminal;
                string str = string.Empty;
                try
                {
                    int num;
                    if (!int.TryParse(obj.Send_Terminal.Substring(1, 1), out num))
                    {
                        if (int.TryParse(obj.Send_Terminal.Substring(0, 1), out num))
                        {
                            str = (
                                from x in this.NumberText
                                where x.Key == obj.Send_Terminal.Substring(0, 1)
                                select x).FirstOrDefault<KeyValuePair<string, string>>().Value.ToString();
                            text3 = str + text3.Substring(1);
                        }
                    }
                    else
                    {
                        str = (
                            from x in this.NumberText
                            where x.Key == obj.Send_Terminal.Substring(0, 2)
                            select x).FirstOrDefault<KeyValuePair<string, string>>().Value.ToString();
                        text3 = str + text3.Substring(2);
                    }
                }
                catch (Exception var_6_171)
                {
                }
                obj.Send_Terminal = text3;
            }
            if (obj.Drug_Flag == "1")
            {
                text = text + obj.Exec_Dpnm + obj.Send_Terminal;
            }
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            if (obj.Drug_Flag == "1")
            {
                text2 = "";
            }
            if (string.IsNullOrEmpty(text))
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "请" + text2 + "到" + obj.Exec_Dpnm;
            }
            else
            {
                if (obj.Class_Code == "UL" || obj.Drug_Flag == "1")
                {
                    if (obj.Drug_Flag == "1")
                    {
                        // {097AA15C-C4CB-4d19-B5C0-76EE20C1ACDE} 内镜中心用药单独备注                     
                        if (hsNJitem.Contains(obj.Item_Code) && !string.IsNullOrEmpty(obj.Assess_Flag))
                        {
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 12f, FontStyle.Bold);//10f 
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = string.Concat(new string[]
					{
						textTemp
					});

                        }
                        else
                        {
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 12f, FontStyle.Bold);//10f
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = string.Concat(new string[]
					{
						"请",
						text2,
						"到",
						text,
                        textrecipe
					});
                        }
                    }
                    else
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = string.Concat(new string[]
					{
						"请",
						text2,
						"到",
						text
					});
                }
                else
                {
                    //非药品非检验项目  还需要考虑为什么这么赋值usopp 8-2
                    if (!string.IsNullOrEmpty(text2))
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = string.Concat(new string[]
					{
						"请",
						text2,
						"到",
						text //,
						//obj.Exec_Dpnm
					});
                    else
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = text;
                }
            }
            if (this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text.Length > 16)
                this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 80;     
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 14.25f, FontStyle.Bold);//10f 
            FarPoint.Win.Spread.CellType.TextCellType tc = new TextCellType();
            tc.WordWrap = true;
            tc.Multiline = true;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 40;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].CellType = tc;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].VerticalAlignment = CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 10.25f, FontStyle.Bold);//10f 
        }
        private void AddItemToSheet(MZGuide obj)
        {
            if (obj.Drug_Flag == "1")
            {
                this.AddDrugItem(obj);
            }
            else
            {
                this.AddUnDrugItem(obj);
                if (!string.IsNullOrEmpty(obj.Note))
                {
                    this.AddUnDrugNote(obj.Note);
                }
            }
        }
        private void AddUnDrugNote(string Note)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "注意事项：" + Note;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 9f, FontStyle.Bold);
        }
        private void AddDrugItem(MZGuide obj)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 4);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 5, 1, 2);
            string name1 = string.Empty;//,// name2 = string.Empty;
            //if (!string.IsNullOrEmpty(obj.Item_Name)&&obj.Item_Name.Length>16)
            //{
            //     name1 = obj.Item_Name.Substring(0, 16);
            //    name2 = obj.Item_Name.Substring(16);
            //}
            if (string.IsNullOrEmpty(name1))
            {
                name1 = obj.Item_Name;
            }
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = name1;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].Text = obj.Qty + "/" + obj.Unit;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 9f, FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].HorizontalAlignment = CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 30;
            //if (!string.IsNullOrEmpty(name2))
            //{
            //    float height = this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height;
            //    this.neuSpread1_Sheet1.RowCount++;
            //    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = name2;
            //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 2, 0].VerticalAlignment = CellVerticalAlignment.Bottom;
            //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].VerticalAlignment = CellVerticalAlignment.Top;
            //    this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height =(float) (height * 0.7);
            //    this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 2].Height =(float) (height * 0.7);
            //    this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 7.5f, FontStyle.Bold); 
            //}

            float len = this.neuSpread1_Sheet1.Columns[1].Width;
            //换行
            float xlength = obj.Item_Name.Length / 16; //System.Text.Encoding.GetEncoding("gb2312").GetBytes(obj.Item_Name).Length / len;
            int length = (int)Math.Ceiling((double)xlength);
            if (length > 1)
                this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height * length;

        }
        private void AddUnDrugItem(MZGuide obj)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 4);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 5, 1, 2);
            string name1 = string.Empty, name2 = string.Empty;
            if (!string.IsNullOrEmpty(obj.Item_Name) && obj.Item_Name.Length > 12)
            {
                name1 = obj.Item_Name.Substring(0, 12);
                name2 = obj.Item_Name.Substring(12);
            }
            if (string.IsNullOrEmpty(name1))
            {
                name1 = obj.Item_Name;
            }
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = name1;
            if (obj.Drug_Flag == "9")
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].Text = obj.Qty + "/" + obj.Unit;
            }
            else
            {
                if (obj.Class_Code == "UC")
                {
                    if (!string.IsNullOrEmpty(obj.Check_Body))
                    {
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].Text = obj.Qty + "/" + obj.Check_Body;
                    }
                    else
                    {
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].Text = obj.Qty;
                    }
                }
                else
                {
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].Text = obj.Qty;
                }
            }
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 5].HorizontalAlignment = CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 9f, FontStyle.Bold);
            if (!string.IsNullOrEmpty(name2))
            {
                float height = this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height;
                this.neuSpread1_Sheet1.RowCount++;
                this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = name2;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 2, 1].VerticalAlignment = CellVerticalAlignment.Bottom;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].VerticalAlignment = CellVerticalAlignment.Top;
                this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = (float)(height * 0.9);
                this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 2].Height = (float)(height * 0.9);
                this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 9f, FontStyle.Bold);

                float len = this.neuSpread1_Sheet1.Columns[1].Width;
                //换行
                if (System.Text.Encoding.GetEncoding("gb2312").GetBytes(name2).Length > len)
                {
                    this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 3 * this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height;
                }
                else
                {
                    this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 2 * this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height;
                }

            }
        }
        private List<MZGuide> AddSpecialItem(List<MZGuide> list, List<MZGuideSpecialExecDept> splist)
        {
            List<MZGuide> result;
            if (list == null)
            {
                result = list;
            }
            else
            {
                List<MZGuide> list2 = new List<MZGuide>();
                foreach (MZGuide item in list)
                {
                    if (splist.Exists((MZGuideSpecialExecDept x) => x.Usage_Code == item.Usage_Code))
                    {
                        MZGuideSpecialExecDept mZGuideSpecialExecDept = (
                            from y in splist
                            where y.Usage_Code == item.Usage_Code
                            select y).FirstOrDefault<MZGuideSpecialExecDept>();
                        MZGuide mZGuide = new MZGuide();
                        mZGuide.ID = item.ID;
                        mZGuide.Item_Code = item.Item_Code;
                        mZGuide.Item_Name = item.Item_Name;
                        mZGuide.MO_Order = item.MO_Order;
                        mZGuide.Qty = item.Qty;
                        mZGuide.Class_Code = item.Class_Code;
                        mZGuide.Clinic_Code = item.Clinic_Code;
                        mZGuide.Drug_Flag = "9";
                        mZGuide.Drug_Terminal = item.Drug_Terminal;
                        mZGuide.Note = item.Note;
                        mZGuide.Recipe_NO = item.Recipe_NO;
                        mZGuide.Send_Terminal = item.Send_Terminal;
                        mZGuide.Spes = item.Spes;

                        mZGuide.Subjob_Flag = item.Subjob_Flag;
                        mZGuide.Tot_Cost = item.Tot_Cost;
                        mZGuide.Exec_Dpcd = mZGuideSpecialExecDept.Exec_Dpcd;
                        mZGuide.Exec_Dpnm = mZGuideSpecialExecDept.Exec_Dpnm;
                        mZGuide.Address = mZGuideSpecialExecDept.Address;
                        mZGuide.Usage_Code = item.Usage_Code;
                        mZGuide.Usage_Name = item.Usage_Name;
                        mZGuide.Unit = "次";

                        list2.Add(mZGuide);
                    }
                }
                foreach (MZGuide current in list2)
                {
                    list.Add(current);
                }
                result = list;
            }
            return result;
        }
        private void AddBarCode(string code)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 1, 1, 6);
            ImageCellType cellType = new ImageCellType();
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Height = 50f;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].CellType = cellType;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].HorizontalAlignment = CellHorizontalAlignment.Left;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Value = this.CreateBarCode(code);
        }
        private Image CreateBarCode(string code)
        {
            Barcode barcode = new Barcode();
            TYPE tYPE = TYPE.CODE128;
            AlignmentPositions alignment = 0;
            barcode.IncludeLabel = true;
            barcode.Alignment = alignment;
            return barcode.Encode(tYPE, code, Color.Black, Color.White, 120, 32);
        }
        private void InitULContrast()
        {
            this.ULContrast = new Neusoft.HISFC.BizLogic.Fee.MZGuide().QueryGuideULContrast();
        }


        #region {0EB9E59D-25AD-4dea-A4B3-35A3AB9B829E} 检验采血项目如果某个科室维护了单独的地点，取对照的地点
        private void InitDicLis()
        {

            ArrayList alNJitem = GetConList("NJitem");
            if (alNJitem.Count == 0)
            {
                MessageBox.Show("内镜项目列表加载失败！");
                return;
            }
            foreach (Neusoft.HISFC.Models.Base.Const dic in alNJitem)
            {
                hsNJitem.Add(dic.ID, dic);
            }

            if (dicLis_Dept == null)
            {
                dicLis_Dept = new Dictionary<string, string>();
                ArrayList alLis_Dept = constManager.GetAllList("LIS_DEPT");
                foreach (Neusoft.HISFC.Models.Base.Const con in alLis_Dept)
                {
                    if (con.IsValid)
                    {
                        if (!dicLis_Dept.ContainsKey(con.ID))
                        {
                            dicLis_Dept.Add(con.ID, con.Name);
                        }
                    }
                }
            }
        }
        #endregion

        private List<MZGuide> ExecUlAddress(List<MZGuide> list)
        {
            List<MZGuide> list2 = (
                from x in list
                where x.Class_Code == "UL"
                select x).ToList<MZGuide>();
            MZGuideContrast c = null;
            if (this.ULContrast != null)
            {
                //InitULContrast();
                c = this.ULContrast.Where(x => x.LabCode == this.LabTypeID).FirstOrDefault();
            }

            string labName = string.Empty;
            if (c != null)
            {
                labName = c.LabName;
            }
            if (list2 == null || list2.Count == 0)
            {
                return list;
            }
            foreach (MZGuide item in list2)
            {
                if (item.Lab_Type.Replace(" ", "") == "24小时尿液")
                {
                    item.Address = "医技楼4楼检验科";
                }
                else
                {
                    MZGuideContrast mZGuideContrast = null;
                    if (this.ULContrast != null)
                    {
                        //InitULContrast();   
                        mZGuideContrast = (
                        from x in this.ULContrast
                        where x.ItemCode == item.Item_Code && x.LabName == item.Lab_Type
                        select x).FirstOrDefault<MZGuideContrast>();
                    }


                    if (mZGuideContrast != null)
                    {

                        item.Address = mZGuideContrast.Addresses;

                    }
                    else
                    {
                        MZGuideContrast mZGuideContrast2 = null;
                        if (this.ULContrast != null)
                        {
                            mZGuideContrast2 = (
                           from x in this.ULContrast
                           where x.ItemCode == item.Item_Code
                           select x).FirstOrDefault<MZGuideContrast>();
                        }

                        if (mZGuideContrast2 == null)
                        {
                            item.Address = "门诊采血室";
                            continue;
                        }
                        if (mZGuideContrast2.LabCode == "All" || mZGuideContrast2.LabName == item.Lab_Type)
                        {
                            item.Address = mZGuideContrast2.Addresses;
                        }
                        else
                        {
                            item.Address = "门诊采血室";
                        }
                    }
                }
                if (string.IsNullOrEmpty(item.Address))
                    item.Address = item.Exec_Dpnm;
            }
            return list;
        }

        /// <summary>
        /// 非检验项目的其他非药品执行地点
        /// </summary>
        private void InitNotULContrast()
        {
            this.NotULContrast = new Neusoft.HISFC.BizLogic.Fee.MZGuide().QueryGuideNotULContrast();
        }
        /// <summary>
        /// 非检验项目的其他非药品执行地点赋值
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<MZGuide> ExecNotUlAddress(List<MZGuide> list)
        {
            List<MZGuide> list2 = (
                from x in list
                where x.Class_Code == "UL" || x.Class_Code == "UO" || x.Class_Code == "U" || x.Class_Code == "UN"
                || x.Class_Code == "MF" || x.Class_Code == "UT" || x.Class_Code == "M" || x.Class_Code == "MC" || x.Class_Code == "UC"
                select x).ToList<MZGuide>();
            foreach (MZGuide item in list2)
            {
                if (this.NotULContrast == null || this.NotULContrast.Count == 0)
                    return list;
                MZGuideContrast mZGuideContrast = (
                    from x in this.NotULContrast
                    where x.ItemCode == item.Item_Code
                    select x).FirstOrDefault<MZGuideContrast>();
                if (mZGuideContrast != null)
                {
                    item.Address = mZGuideContrast.Addresses;
                }
                if (string.IsNullOrEmpty(item.Address))
                    item.Address = item.Exec_Dpnm;
            }
            return list;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && this.components != null)
        //    {
        //        this.components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        //private void InitializeComponent()
        //{
        //    TipAppearance tipAppearance = new TipAppearance();
        //    this.neuSpread1 = new NeuSpread();
        //    this.neuSpread1_Sheet1 = new SheetView();
        //    this.neuSpread1.BeginInit();
        //    ((ISupportInitialize)this.neuSpread1_Sheet1).BeginInit();
        //    base.SuspendLayout();
        //    this.neuSpread1.About = "3.0.2004.2005";
        //    this.neuSpread1.AccessibleDescription = "neuSpread1, Sheet1, Row 0, Column 0, ";
        //    this.neuSpread1.BackColor = Color.White;
        //    this.neuSpread1.BorderStyle = BorderStyle.None;
        //    this.neuSpread1.Dock = DockStyle.Fill;
        //    this.neuSpread1.set_FileName("");
        //    this.neuSpread1.HorizontalScrollBarPolicy = ScrollBarPolicy.Never;
        //    this.neuSpread1.set_IsAutoSaveGridStatus(false);
        //    this.neuSpread1.set_IsCanCustomConfigColumn(false);
        //    this.neuSpread1.Location = new Point(0, 0);
        //    this.neuSpread1.Name = "neuSpread1";
        //    this.neuSpread1.RightToLeft = RightToLeft.No;
        //    this.neuSpread1.Sheets.AddRange(new SheetView[]
        //    {
        //        this.neuSpread1_Sheet1
        //    });
        //    this.neuSpread1.Size = new Size(260, 70);
        //    this.neuSpread1.set_Style(0);
        //    this.neuSpread1.TabIndex = 1;
        //    tipAppearance.BackColor = SystemColors.Info;
        //    tipAppearance.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
        //    tipAppearance.ForeColor = SystemColors.InfoText;
        //    this.neuSpread1.TextTipAppearance = tipAppearance;
        //    this.neuSpread1.VerticalScrollBarPolicy = ScrollBarPolicy.Never;
        //    this.neuSpread1_Sheet1.Reset();
        //    this.neuSpread1_Sheet1.SheetName = "Sheet1";
        //    this.neuSpread1_Sheet1.ReferenceStyle = ReferenceStyle.R1C1;
        //    this.neuSpread1_Sheet1.ColumnCount = 6;
        //    this.neuSpread1_Sheet1.ColumnHeader.RowCount = 0;
        //    this.neuSpread1_Sheet1.RowCount = 1;
        //    this.neuSpread1_Sheet1.RowHeader.ColumnCount = 0;
        //    this.neuSpread1_Sheet1.Columns.Get(0).Width = 35f;
        //    this.neuSpread1_Sheet1.Columns.Get(1).Width = 35f;
        //    this.neuSpread1_Sheet1.Columns.Get(2).Width = 45f;
        //    this.neuSpread1_Sheet1.Columns.Get(3).Width = 35f;
        //    this.neuSpread1_Sheet1.Columns.Get(4).Width = 45f;
        //    this.neuSpread1_Sheet1.Columns.Get(5).Width = 45f;
        //    this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
        //    base.AutoScaleDimensions = new SizeF(6f, 12f);
        //    base.AutoScaleMode = AutoScaleMode.Font;
        //    this.BackColor = Color.White;
        //    base.Controls.Add(this.neuSpread1);
        //    //base.Name = "ucFeeDetailGuide";
        //    base.Size = new Size(260, 70);
        //    this.neuSpread1.EndInit();
        //    ((ISupportInitialize)this.neuSpread1_Sheet1).EndInit();
        //    base.ResumeLayout(false);
        //}
        #region 支付方式帮助方法

        private string GetPayModeName(string paymodeID)
        {
            switch (paymodeID)
            {
                case "CA":
                    return "现金";
                case "MER":
                    return "银联卡";
                case "ICBC":
                    return "银联卡";
                case "NXS":
                    return "银联卡";
                case "COMM":
                    return "银联卡";
                case "PB":
                    return "记账";
                case "UP":
                    return "银联卡";
                case "MCZH":
                    return "社会保障卡(珠海)";
                case "MCDZ":
                    return "社会保障卡(电子支付)";
                case "MCZS":
                    return "社会保障卡(中山)";
                case "PBZH":
                    return "珠海医保统筹";
                case "PBZS":
                    return "中山医保统筹";
                case "RC":
                    return "优惠";
                case "CH":
                    return "支票";
                case "PO":
                    return "汇款";
                case "ZZ":
                    return "宰账";
                case "GZ":
                    return "社会保障卡(广州)";
                case "ZSMZ":
                    return "中山门诊民政救助";
                case "ZFB":
                    return "支付宝";
                case "WX":
                    return "微信";
                case "CCB":
                    return "建行";
                case "MCSNYD":
                    return "社会保障卡（省内异地）：";
                case "SNYDYBTC":
                    return "省内异地医保统筹";
                default:
                    return "其他";

            }
        }
        #endregion
    }
}
