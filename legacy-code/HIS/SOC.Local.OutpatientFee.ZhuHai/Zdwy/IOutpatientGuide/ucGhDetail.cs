using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientGuide
{
    /// <summary>
    /// 门诊收费清单
    /// </summary>
    public partial class ucGhDetail : UserControl
    {
        public ucGhDetail()
        {
            InitializeComponent();
        }

        enum EnumCol
        {
            kk空空,
            xmdm项目代码,
            xmmc项目名称,
            fpfl发票分类,
            gg规格,
            yblx医保类型,
            dw单位,
            dj单价,
            sl数量,
            je金额
        }

        private Dictionary<string, string> dicInvoceType = new Dictionary<string, string>();

        Neusoft.HISFC.BizLogic.Manager.DataBase dbMgr = new Neusoft.HISFC.BizLogic.Manager.DataBase();

        public void SetValue(DataTable dtinfo)
        {
            if (dtinfo.Rows.Count > 0)
            {
                decimal totCost = decimal.Parse(dtinfo.Rows[0]["TOT_COST"].ToString());
                decimal ownCost = decimal.Parse(dtinfo.Rows[0]["OWN_COST"].ToString());
                decimal payCost = decimal.Parse(dtinfo.Rows[0]["PAY_COST"].ToString());
                decimal pubCost = decimal.Parse(dtinfo.Rows[0]["PUB_COST"].ToString());

                this.lblName.Text = dtinfo.Rows[0]["NAME"].ToString();

                this.fpSpreadItemsSheet.RowCount = 1;

                this.fpSpreadItemsSheet.RowCount += 10 + 1;
                int row = 1;

                Neusoft.HISFC.Models.Base.PactInfo pact = SOC.HISFC.BizProcess.Cache.Fee.GetPactUnitInfo(dtinfo.Rows[0]["PAY_TYPE"].ToString());

                Neusoft.HISFC.Models.Fee.Item.Undrug undrug = SOC.HISFC.BizProcess.Cache.Fee.GetItem(dtinfo.Rows[0]["ITEM_CODE"].ToString());
                lblInvoiceNo.Text = dtinfo.Rows[0]["INVOICE_NO"].ToString();
                DateTime dateTime = (DateTime)dtinfo.Rows[0]["FEE_DATE"];
                string formattedDate = dateTime.ToString("yyyyMMdd");
                lblFeeDate.Text = formattedDate;
                lblDoctName.Text = SOC.HISFC.BizProcess.Cache.Common.GetEmployeeName(dtinfo.Rows[0]["DOCT_CODE"].ToString());
                lblDept.Text = SOC.HISFC.BizProcess.Cache.Common.GetDeptName(dtinfo.Rows[0]["DEPT_CODE"].ToString());

                string xmdm = "";
                if (undrug.GBCode == "特需")
                {
                    xmdm = "";
                }
                else
                {
                    xmdm = undrug.GBCode;
                }
                string ybdj = dtinfo.Rows[0]["YBDJ"].ToString();
                if (ybdj == "1")
                {
                    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.yblx医保类型].Text = "甲类";
                }
                else if (ybdj == "2")
                {
                    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.yblx医保类型].Text = "乙类";
                }
                else
                {
                    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.yblx医保类型].Text = "丙类"; 
                }
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmdm项目代码].Text = xmdm;
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmmc项目名称].Text = undrug.Name;
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.gg规格].Text = "次";

                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.fpfl发票分类].Text = "诊查费";
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.dj单价].Text = undrug.Price.ToString("f4");
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.sl数量].Text = "1";
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.je金额].Text = (ownCost + pubCost + payCost).ToString();

                fpSpreadItemsSheet.Rows[row].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                totCost = ownCost + pubCost + payCost;

                row += 1;

                #region 显示设置

                fpSpreadItemsSheet.Rows[row].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(System.Drawing.SystemColors.WindowFrame, 2));
                FarPoint.Win.ComplexBorder complexBorder2 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(System.Drawing.SystemColors.WindowFrame, 2));
                FarPoint.Win.ComplexBorder complexBorder3 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(System.Drawing.SystemColors.WindowFrame, 2));
                FarPoint.Win.ComplexBorder complexBorder4 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(System.Drawing.SystemColors.WindowFrame, 2));
                FarPoint.Win.ComplexBorder complexBorder5 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(System.Drawing.SystemColors.WindowFrame, 2));
                FarPoint.Win.ComplexBorder complexBorder6 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(System.Drawing.SystemColors.WindowFrame, 2));
                FarPoint.Win.ComplexBorder complexBorder7 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(System.Drawing.SystemColors.WindowFrame, 2));
                FarPoint.Win.ComplexBorder complexBorder8 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(System.Drawing.SystemColors.WindowFrame, 2));
                FarPoint.Win.ComplexBorder complexBorder9 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(System.Drawing.SystemColors.WindowFrame, 2));

                this.fpSpreadItemsSheet.Cells.Get(row - 1, 0).Border = complexBorder1;
                this.fpSpreadItemsSheet.Cells.Get(row - 1, 1).Border = complexBorder2;
                this.fpSpreadItemsSheet.Cells.Get(row - 1, 2).Border = complexBorder3;
                this.fpSpreadItemsSheet.Cells.Get(row - 1, 3).Border = complexBorder4;
                this.fpSpreadItemsSheet.Cells.Get(row - 1, 4).Border = complexBorder5;
                this.fpSpreadItemsSheet.Cells.Get(row - 1, 5).Border = complexBorder6;
                this.fpSpreadItemsSheet.Cells.Get(row - 1, 6).Border = complexBorder7;
                this.fpSpreadItemsSheet.Cells.Get(row - 1, 7).Border = complexBorder8;
                this.fpSpreadItemsSheet.Cells.Get(row - 1, 8).Border = complexBorder9;
                this.fpSpreadItemsSheet.Cells.Get(row - 1, 9).Border = complexBorder9;

                #endregion
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmdm项目代码].ColumnSpan = 3;
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmdm项目代码].Text = "打印日期：" + dbMgr.GetDateTimeFromSysDateTime().ToString();

                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.dj单价].ColumnSpan = 2;
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.dj单价].Text = "合计：";

                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.je金额].ColumnSpan = 1;
                fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.je金额].Text = totCost.ToString();
            }
        }

        /// <summary>
        /// 存放对照项目
        /// </summary>
        Dictionary<string, Dictionary<string, Neusoft.HISFC.Models.SIInterface.Compare>> dicCompare = null;

        Neusoft.HISFC.BizLogic.Fee.Interface interfaceMgr = new Neusoft.HISFC.BizLogic.Fee.Interface();

        /// <summary>
        /// 获取医保对照项目信息
        /// </summary>
        /// <param name="item.ID">项目编码</param>
        /// <param name="compareItem">对照项目信息</param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.SIInterface.Compare GetCompareItemInfo(string pactCode, string itemCode)
        {
            if (dicCompare == null)
            {
                dicCompare = new Dictionary<string, Dictionary<string, Neusoft.HISFC.Models.SIInterface.Compare>>();
            }

            Neusoft.HISFC.Models.SIInterface.Compare compareItem = null;

            if (dicCompare.ContainsKey(pactCode))
            {
                if (dicCompare[pactCode].ContainsKey(itemCode))
                {
                    return dicCompare[pactCode][itemCode];
                }
                else
                {
                    int rev = interfaceMgr.GetCompareSingleItem(pactCode, itemCode, ref compareItem);
                    if (rev == -1)
                    {
                        //errInfo = "获取医保对照项目失败：" + interfaceMgr.Err;
                        compareItem = null;
                    }
                    else
                    {
                        dicCompare[pactCode].Add(itemCode, compareItem);
                    }
                    return compareItem;
                }
            }
            else
            {
                int rev = interfaceMgr.GetCompareSingleItem(pactCode, itemCode, ref compareItem);
                if (rev == -1)
                {
                    //errInfo = "获取医保对照项目失败：" + interfaceMgr.Err;
                    compareItem = null;
                }
                else
                {
                    Dictionary<string, Neusoft.HISFC.Models.SIInterface.Compare> dicPactCompare = new Dictionary<string, Neusoft.HISFC.Models.SIInterface.Compare>();
                    dicPactCompare.Add(itemCode, compareItem);
                    dicCompare.Add(pactCode, dicPactCompare);
                }
                return compareItem;
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        public void PrintPage()
        {
            if (fpSpreadItemsSheet.RowCount > 0)
            {
                Neusoft.FrameWork.WinForms.Classes.Print print = new Neusoft.FrameWork.WinForms.Classes.Print();

                print.ControlBorder = Neusoft.FrameWork.WinForms.Classes.enuControlBorder.None;
                print.IsDataAutoExtend = false;
                if (FrameWork.WinForms.Classes.Function.IsManager())
                {
                    print.PrintPreview(5, 5, this);
                }
                else
                {
                    //print.PrintPage(5, 5, this);
                    print.PrintPreview(5, 5, this);
                }
            }
        }
    }
}
