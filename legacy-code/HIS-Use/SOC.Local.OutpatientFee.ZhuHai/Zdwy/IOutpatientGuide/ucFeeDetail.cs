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
    public partial class ucFeeDetail : UserControl
    {
        public ucFeeDetail()
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

        public void SetValue(Neusoft.HISFC.Models.Registration.Register rInfo, System.Collections.ArrayList invoices, System.Collections.ArrayList feeDetails)
        {
            if (feeDetails.Count > 0)
            {
                this.lblName.Text = rInfo.Name;

                this.fpSpreadItemsSheet.RowCount = 1;
                this.fpSpreadItemsSheet.RowCount += feeDetails.Count + 1;
                int row = 1;

                Neusoft.HISFC.Models.Base.PactInfo pact = SOC.HISFC.BizProcess.Cache.Fee.GetPactUnitInfo(rInfo.Pact.ID);

                decimal totCost = 0;
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList in feeDetails)
                {
                    lblInvoiceNo.Text = feeItemList.Invoice.ID;
                    lblFeeDate.Text = feeItemList.FeeOper.OperTime.ToString("yyyy-MM-dd");
                    lblDoctName.Text = SOC.HISFC.BizProcess.Cache.Common.GetEmployeeName(feeItemList.RecipeOper.ID);
                    lblDept.Text = SOC.HISFC.BizProcess.Cache.Common.GetDeptName(feeItemList.RecipeOper.Dept.ID);


                    #region 医保标记
                    string SIFlag = "";

                    //if (pact.PactDllName == "OwnFee.dll")
                    //{
                    //    SIFlag = "自费类";
                    //}
                    //else
                    //{
                    string strPactCode = "14";

                    Neusoft.HISFC.Models.SIInterface.Compare compareItem = this.GetCompareItemInfo(strPactCode, feeItemList.Item.ID);

                    if (compareItem == null
                        || compareItem.CenterItem.Rate == 1
                        || string.IsNullOrEmpty(compareItem.CenterItem.ItemGrade)
                        || compareItem.CenterItem.ItemGrade == "3")
                    {
                        SIFlag = "丙类";
                    }
                    else if (compareItem.CenterItem.ItemGrade == "2")
                    {
                        SIFlag = "乙类";
                    }
                    else if (compareItem.CenterItem.ItemGrade == "1")
                    {
                        SIFlag = "甲类";
                    }
                    else
                    {
                        SIFlag = "";
                    }
                    //}

                    #endregion

                    #region 发票分类

                    string strTypeName = "";
                    if (dicInvoceType.ContainsKey(feeItemList.Item.MinFee.ID))
                    {
                        strTypeName = dicInvoceType[feeItemList.Item.MinFee.ID];
                    }
                    else
                    {
                        string strSQL = @"  select d.fee_stat_name from fin_com_feecodestat d
                                        where d.report_code='MZ01'
                                        and d.fee_code='{0}'";
                        strSQL = string.Format(strSQL, feeItemList.Item.MinFee.ID);
                        strTypeName = dbMgr.ExecSqlReturnOne(strSQL);
                        dicInvoceType.Add(feeItemList.Item.MinFee.ID, strTypeName);
                    }
                    #endregion


                    if (feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                    {
                        Neusoft.HISFC.Models.Pharmacy.Item phaItem = SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(feeItemList.Item.ID);
                        if (phaItem != null)
                        {
                            fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmdm项目代码].Text = phaItem.UserCode;
                            fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmmc项目名称].Text = phaItem.Name;
                            fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.gg规格].Text = phaItem.Specs;
                            fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.dw单位].Text = phaItem.MinUnit;
                        }
                    }
                    else
                    {
                        Neusoft.HISFC.Models.Fee.Item.Undrug undrug = SOC.HISFC.BizProcess.Cache.Fee.GetItem(feeItemList.Item.ID);
                        
                        if (undrug != null)
                        {
                            #region AD248EC5-D724-3A06-A420-6BE15A9B1CA1
                            //物价需求20211026 yhm 更改患者费用清单编码显示:
                            //一、编码更改为:国家编码+‘E’,同时满足以下规则：
                            //    1、6岁以下儿童(含6岁生日当天)患者
                            //    2、收取医疗服务项目为国家编码3开头的收费项目
                            //    3、收取价格为儿童价字段，且收费价格等于默认价的项目的1.3倍的项目                                   
                            string code = Function.GetCodeToPrint(rInfo.ID, feeItemList, feeItemList.FeeOper.OperTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmdm项目代码].Text = code;
                            fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmmc项目名称].Text = undrug.Name;
                            fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.gg规格].Text = undrug.Specs; 
                            #endregion
                        }

                        //if (undrug != null)//旧注释
                        //{
                        //    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmdm项目代码].Text = undrug.GBCode == "" ? undrug.UserCode : undrug.GBCode;
                        //    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.xmmc项目名称].Text = undrug.Name;
                        //    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.gg规格].Text = undrug.Specs;
                        //}
                    }
                    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.fpfl发票分类].Text = strTypeName;
                    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.yblx医保类型].Text = SIFlag;
                    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.dj单价].Text = (feeItemList.Item.Price / feeItemList.Item.PackQty).ToString("f4");
                    //fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.dw单位].Text = feeItemList.Item.PriceUnit;
                    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.sl数量].Text = feeItemList.Item.Qty.ToString();
                    fpSpreadItemsSheet.Cells[row, (Int32)EnumCol.je金额].Text = (feeItemList.FT.OwnCost + feeItemList.FT.PubCost + feeItemList.FT.PayCost).ToString();

                    fpSpreadItemsSheet.Rows[row].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                    totCost += feeItemList.FT.OwnCost + feeItemList.FT.PubCost + feeItemList.FT.PayCost;

                    row += 1;
                }

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
