using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;


namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientGuide
{
    public partial class ucFeeDetailGuide : UserControl
    {
        public ucFeeDetailGuide()
        {
            InitializeComponent();
        }
        #region 业务层
        /// <summary>
        /// 常数维护业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Manager.Constant constManager = new Neusoft.HISFC.BizLogic.Manager.Constant();

        /// <summary>
        /// 医保接口业务层(本地)
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.Interface interfaceManager = new Neusoft.HISFC.BizLogic.Fee.Interface();

        /// <summary>
        /// 统计大类业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.FeeCodeStat feeCodeStatManager = new Neusoft.HISFC.BizLogic.Fee.FeeCodeStat();

        /// <summary>
        /// 复合项目业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.UndrugPackAge packageManager = new Neusoft.HISFC.BizLogic.Fee.UndrugPackAge();

        /// <summary>
        /// 项目业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.Item itemManager = new Neusoft.HISFC.BizLogic.Fee.Item();
        #endregion

        private void InitHeader()
        {
            this.neuSpread1_Sheet1.RowCount = 0;
            this.neuSpread1_Sheet1.GrayAreaBackColor = System.Drawing.Color.White;
            this.neuSpread1_Sheet1.HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);
            this.neuSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.neuSpread1_Sheet1.SheetCornerHorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);
            this.neuSpread1_Sheet1.SheetCornerVerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);
            this.neuSpread1_Sheet1.VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.None);
            this.neuSpread1_Sheet1.ColumnHeader.DefaultStyle.BackColor = System.Drawing.Color.White;
        }

        /// <summary>
        /// 添加空白
        /// </summary>
        private void AddSpace()
        {
            //空白
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
        }

        /// <summary>
        /// 添加表头
        /// by lizy
        /// </summary>
        private void AddHead()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = constManager.GetHospitalName() + "门诊费用清单";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
        }

        /// <summary>
        /// 添加病人信息
        /// by lizy
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seq"></param>
        /// <param name="feeDate"></param>
        private void AddPatientInfo(string name, string seq, DateTime feeDate)
        {
            this.neuSpread1_Sheet1.RowCount++;

            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "发票号:" + seq;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 3);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 3, 1, 3);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "姓  名:" + name;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Font = new Font("宋体", 8);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = string.Format("{0:yyyy-MM-dd}", feeDate);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;

        }

        /// <summary>
        /// 增加费用表头
        /// by lizy
        /// </summary>
        private void AddFeeHeader()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 2);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 4, 1, 2);
            //单元格边框
            FarPoint.Win.ComplexBorder complexbrdr = new FarPoint.Win.ComplexBorder(
                    new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None),
                    new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine),
                    new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None),
                    new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine));
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "项目名称[规格]";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Border = complexbrdr;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Text = "单价";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Border = complexbrdr;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = "数量";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Border = complexbrdr;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = "金额";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Border = complexbrdr;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 8, FontStyle.Bold);
        }

        private string tempCombID = string.Empty;

        /// <summary>
        /// 增加一行费用明细
        /// by lizy
        /// </summary>
        /// <param name="feeItemList">费用明细</param>
        /// <returns>费用金额</returns>
        private decimal AddFeeDetailLine(Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList)
        {
            this.neuSpread1_Sheet1.RowCount++;
            string itemName = string.Empty;
            //组合项目先显示组合项目名称、数量
            if (!string.IsNullOrEmpty(feeItemList.UndrugComb.ID))
            {
                //组合明细，判断是否已经显示组合项目
                if (string.IsNullOrEmpty(tempCombID) || !tempCombID.Equals(feeItemList.UndrugComb.ID))
                {
                    this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 3);
                    //根据组合项目ID判断
                    tempCombID = feeItemList.UndrugComb.ID;
                    //增加一行显示组合项目名称、数量
                    Neusoft.HISFC.Models.Fee.Item.UndrugComb undrugComb = packageManager.GetUndrugComb(feeItemList.UndrugComb.ID, feeItemList.Item.ID);
                    if (undrugComb != null)
                    {
                        Neusoft.HISFC.Models.Base.Item Combitem = itemManager.GetUndrugByCode(feeItemList.UndrugComb.ID);
                        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = Combitem.Name;
                        if (undrugComb.Qty == 0)
                        {
                            undrugComb.Qty = 1;
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = (feeItemList.Item.Qty / undrugComb.Qty).ToString() + ("无".Equals(Combitem.PriceUnit) ? "" : Combitem.PriceUnit);
                        }
                        else
                        {
                            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = (feeItemList.Item.Qty / undrugComb.Qty).ToString() + ("无".Equals(Combitem.PriceUnit) ? "" : Combitem.PriceUnit);
                        }
                    }
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                    this.neuSpread1_Sheet1.RowCount++;
                }
                itemName += "┗";
            }
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            if (!string.IsNullOrEmpty(feeItemList.Item.Specs))
            {
                itemName += feeItemList.Item.Name + "[" + feeItemList.Item.Specs + "]";
            }
            else
            {
                itemName += feeItemList.Item.Name;
            }
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = itemName;//feeItemList.Item.Name + "  " + feeItemList.Item.Specs;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 3);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 4, 1, 2);
            if (feeItemList.Item.PackQty <= 0)
            {
                feeItemList.Item.PackQty = 1;
            }
            if ("1".Equals(feeItemList.FeePack))//包装单位1，最小单位0
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].CellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = feeItemList.Item.Price.ToString("F2");
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = (feeItemList.Item.Qty / feeItemList.Item.PackQty).ToString() + feeItemList.Item.PriceUnit;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].CellType = new FarPoint.Win.Spread.CellType.NumberCellType();
            }
            else
            {
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].CellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = (feeItemList.Item.Price / feeItemList.Item.PackQty).ToString("F2");
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = feeItemList.Item.Qty.ToString() + feeItemList.Item.PriceUnit;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].CellType = new FarPoint.Win.Spread.CellType.NumberCellType();
            }
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = (feeItemList.FT.PubCost + feeItemList.FT.OwnCost + feeItemList.FT.PayCost).ToString("F2");
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 8);
            return feeItemList.FT.PubCost + feeItemList.FT.OwnCost + feeItemList.FT.PayCost;
        }

        /// <summary>
        /// 增加大类小计
        /// by lizy
        /// </summary>
        /// <param name="feeName">大类费用名</param>
        /// <param name="feeTotal">小计金额</param>
        private void AddFeeTotalLine(string feeName, decimal feeTotal)
        {
            //单元格边框
            FarPoint.Win.ComplexBorder complexbrdr = new FarPoint.Win.ComplexBorder(
                    new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None),
                    new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None),
                    new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None),
                    new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine));
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 3);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 3, 1, 3);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = feeName + "小计:";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Border = complexbrdr;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].CellType = new FarPoint.Win.Spread.CellType.NumberCellType();
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = feeTotal.ToString("F2");
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Border = complexbrdr;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 8);
        }

        /// <summary>
        /// 增加合计
        /// by lizy
        /// </summary>
        /// <param name="total"></param>
        private void AddFeeTotal(decimal total)
        {
            //操作员
            Neusoft.HISFC.Models.Base.Employee emp = Neusoft.FrameWork.Management.Connection.Operator as Neusoft.HISFC.Models.Base.Employee;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 2);
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 3, 1, 3);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "打印:" + emp.ID;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Text = "合计:";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].CellType = new FarPoint.Win.Spread.CellType.NumberCellType();
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = total.ToString("F2");
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.neuSpread1_Sheet1.Rows[this.neuSpread1_Sheet1.RowCount - 1].Font = new Font("宋体", 8, FontStyle.Bold);
        }

        private string GetFeeStatName(ArrayList listFeeStat, string minFeeID)
        {
            foreach (Neusoft.HISFC.Models.Fee.FeeCodeStat feeCodeStat in listFeeStat)
            {
                string temp = (int.Parse(minFeeID)).ToString();
                string temp2 = feeCodeStat.MinFee.ID;
                if (feeCodeStat.MinFee.ID.Equals(minFeeID) || feeCodeStat.MinFee.ID.Equals((int.Parse(minFeeID)).ToString()))
                {
                    return feeCodeStat.StatCate.Name;
                }
            }
            return "";
        }

        private HashSet<string> GetStatCateList(ArrayList list)
        {
            HashSet<string> statCateList = new HashSet<string>();
            foreach (Neusoft.HISFC.Models.Fee.FeeCodeStat feeCodeStat in list)
            {
                if (statCateList.Contains(feeCodeStat.StatCate.Name))
                {
                    continue;
                }
                else
                {
                    statCateList.Add(feeCodeStat.StatCate.Name); ;
                }
            }
            return statCateList;
        }

        private void AddFee(ArrayList alFeeDetail)
        {
            //获取门诊发票大类明细
            ArrayList listFeeStat = this.feeCodeStatManager.QueryFeeCodeStatByReportCode("MZ01");

            string tempName = null;
            //大类小计金额
            decimal feeTotal;
            //合计
            decimal total = 0;
            //listFeeStat.Sort(new IcompareFeeCodeStat());
            HashSet<string> statCateList = this.GetStatCateList(listFeeStat);
            foreach (string feeStatName in statCateList)
            {
                feeTotal = 0;
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList in alFeeDetail)
                {
                    tempName = this.GetFeeStatName(listFeeStat, feeItemList.Item.MinFee.ID);
                    if (feeStatName == "" || !feeStatName.Equals(tempName))
                    {
                        continue;
                    }
                    else
                    {
                        feeTotal += this.AddFeeDetailLine(feeItemList);
                    }
                }
                if (feeTotal != 0)
                {
                    this.AddFeeTotalLine(feeStatName, feeTotal);
                    total += feeTotal;
                }
            }
            this.AddFeeTotal(total);
        }

        class IcompareFeeCodeStat : IComparer<Neusoft.HISFC.Models.Fee.FeeCodeStat>
        {
            public int Compare(Neusoft.HISFC.Models.Fee.FeeCodeStat x, Neusoft.HISFC.Models.Fee.FeeCodeStat y)
            {
                return x.SortID.CompareTo(y.SortID);
            }
        }


        /// <summary>
        /// 添加医院名称
        /// </summary>
        private void AddHospitalInfo()
        {
            //医院名称
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8, System.Drawing.FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = constManager.GetHospitalName();
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
        }

        /// <summary>
        /// 添加姓名
        /// </summary>
        private void AddName(string name, DateTime feeDate)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8, System.Drawing.FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "姓名:" + name;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = " 收费日期:" + feeDate.ToShortDateString();
        }

        /// <summary>
        /// 添加姓名和年龄
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <param name="feeDate"></param>
        private void AddNameAndAge(string name, string sexName, string age, DateTime feeDate)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8, System.Drawing.FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "姓名:" + name + " " + " " + sexName + age;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text =
                " 收费日期:" + feeDate.ToShortDateString();

        }

        /// <summary>
        /// 添加发票号和病历号
        /// </summary>
        private decimal AddCardNOAndInvoice(string cardNO, ArrayList alInvoice)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8, System.Drawing.FontStyle.Bold);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "病历号:" + cardNO;
            decimal TotCost = 0M;
            foreach (Neusoft.HISFC.Models.Fee.Outpatient.Balance balance in alInvoice)
            {
                TotCost += balance.FT.TotCost;
                if (balance.Invoice.Memo == "5")
                {
                    continue;
                }

                this.neuSpread1_Sheet1.RowCount++;
                this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = " 发票号:" + balance.Invoice.ID;
            }

            return TotCost;
        }

        /// <summary>
        /// 添加诊断
        /// </summary>
        /// <param name="clinicCode"></param>
        private void AddDiagnoseInfo(string clinicCode, string diagnoseName)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "诊断:" + diagnoseName;

        }


        ///// <summary>
        ///// 添加费用明细
        ///// </summary>
        ///// <param name="alFeeDetail"></param>
        //private void AddFeeDetail(ArrayList alFeeDetail)
        //{
        //    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList in alFeeDetail)
        //    {
        //        //项目名称
        //        this.neuSpread1_Sheet1.RowCount++;
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].ColumnSpan = 5;
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = feeItemList.Item.Name;
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 9);

        //        //项目明细（甲乙类，数量，单位，单价，金额）
        //        this.neuSpread1_Sheet1.RowCount++;

        //        //医保信息
        //        try
        //        {
        //            //this.interfaceManager.GetCompareSingleItem("2", f.Item.ID, ref f.Compare);
        //            this.interfaceManager.GetCompareSingleItem("2", feeItemList.Item.ID, ref feeItemList.Compare);
        //        }
        //        catch { }


        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = this.GetItemGrade(feeItemList);
        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Font = new Font("宋体", 9);

        //        if (feeItemList.Item.ChildPrice > feeItemList.Item.Price)
        //        {
        //            feeItemList.Item.Price = feeItemList.Item.ChildPrice;
        //        }

        //        if (feeItemList.Item.PackQty <= 0)
        //        {
        //            feeItemList.Item.PackQty = 1;
        //        }
        //        if (feeItemList.FeePack.Equals("1"))//包装单位
        //        {
        //            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = (feeItemList.Item.Qty / feeItemList.Item.PackQty).ToString();
        //            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = feeItemList.Item.Price.ToString("F2");
        //            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = (feeItemList.Item.Price * int.Parse(this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text)).ToString("F2");
        //        }
        //        else
        //        {
        //            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = feeItemList.Item.Qty.ToString("F2");
        //            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = (feeItemList.Item.Price / feeItemList.Item.PackQty).ToString("F2");
        //            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = (feeItemList.Item.Price / feeItemList.Item.PackQty * int.Parse(this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text)).ToString("F2");
        //        }

        //        this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Text = feeItemList.Item.PriceUnit;




        //    }
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.RowCount++;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].ColumnSpan = 6;
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8);
        //    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "备注：以上医保等级为广州医保中心甲乙类等级";

        //}

        /// <summary>
        /// 添加尾注
        /// </summary>
        /// <param name="TotCost"></param>
        /// <param name="dtNow"></param>
        private void AddFeeFooter(decimal TotCost, DateTime dtNow)
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 5);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "收款员: " + Neusoft.FrameWork.Management.Connection.Operator.ID + " 合计: " + TotCost.ToString();
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 5);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "打印日期: " + dtNow.ToShortDateString();
        }

        /// <summary>
        /// 添加撕纸线
        /// </summary>
        private void AddLine()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "---------撕 纸 线----------";
        }

        /// <summary>
        /// 添加温馨提示
        /// </summary>
        private void AddHospitalTipForUL()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = constManager.GetHospitalName() + "门诊检验项目温馨提示单";
        }

        /// <summary>
        /// 添加温馨提示
        /// </summary>
        private void AddHospitalTipForUC()
        {
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Models.Span.Add(this.neuSpread1_Sheet1.RowCount - 1, 0, 1, 6);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Font = new Font("宋体", 8);
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = constManager.GetHospitalName() + "门诊检查项目温馨提示单";
        }

        /// <summary>
        /// 添加分组项目
        /// </summary>
        private ArrayList GetLisGroupItem(ArrayList alFeeDetail)
        {
            //分组显示
            ArrayList ulList = new ArrayList();
            int count = alFeeDetail.Count;
            for (int i = 0; i < count; i++)
            {
                Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = alFeeDetail[i] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                //检验分组
                if (feeItemList.Item.SysClass.ID.ToString().Equals(Neusoft.HISFC.Models.Base.EnumSysClass.UL.ToString()))
                {
                    ulList.Add(feeItemList);
                }
            }

            for (int i = 0; i < ulList.Count; i++)
            {
                alFeeDetail.Remove(ulList[i]);
            }

            ArrayList sameNotes = new ArrayList();
            ArrayList ReturnList = new ArrayList();
            while (ulList.Count > 0)
            {
                Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = ulList[0] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                sameNotes.Add(feeItemList);
                for (int j = 1; j < ulList.Count; j++)
                {
                    Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemListCompare = ulList[j] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                    if (!feeItemListCompare.Item.SysClass.ID.ToString().Equals(Neusoft.HISFC.Models.Base.EnumSysClass.UL.ToString()))
                    {
                        continue;
                    }
                    //执行科室一致，样本一致分到一组
                    if (feeItemList.ExecOper.Dept.ID.Equals(feeItemListCompare.ExecOper.Dept.ID) && feeItemList.Order.Sample.Name.Equals(feeItemListCompare.Order.Sample.Name))
                    {
                        sameNotes.Add(feeItemListCompare);
                    }

                }
                ReturnList.Add(sameNotes);
                for (int i = 0; i < sameNotes.Count; i++)
                {
                    ulList.Remove(sameNotes[i]);
                }
            }

            return ReturnList;

        }

        /// <summary>
        /// 添加检验分组项目
        /// </summary>
        /// <param name="alFeeDetail"></param>
        private void AddLisGroupItem(ArrayList alFeeDetail)
        {
            if (alFeeDetail.Count == 0)
            {
                return;
            }
            Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = alFeeDetail[0] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = feeItemList.ExecOper.Dept.Name;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Text = "样本:";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = feeItemList.Order.Sample.Name;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = " 项目";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "数量";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = "单价";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = "金额";

            foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItem in alFeeDetail)
            {
                this.neuSpread1_Sheet1.RowCount++;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].ColumnSpan = 5;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = feeItem.Item.Name;

                this.neuSpread1_Sheet1.RowCount++;


                //医保信息
                try
                {
                    //this.interfaceManager.GetCompareSingleItem("2", f.Item.ID, ref f.Compare);
                    this.interfaceManager.GetCompareSingleItem("2", feeItem.Item.ID, ref feeItem.Compare);
                }
                catch { }

                //甲乙类
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = this.GetItemGrade(feeItem);
                if (feeItem.FeePack.Equals("1"))//包装单位
                {
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = (feeItem.Item.Qty / feeItem.Item.PackQty).ToString();
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = feeItem.Item.Price.ToString("F2");
                }
                else
                {
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = feeItem.Item.Qty.ToString();
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = (feeItem.Item.Price / feeItem.Item.PackQty).ToString("F2");
                }
                //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = (feeItem.FT.PubCost+feeItem.FT.PayCost+feeItem.FT.OwnCost).ToString("F2");
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = ((feeItem.Item.Price / feeItem.Item.PackQty) * feeItem.Item.Qty).ToString("F2");
            }

        }

        /// <summary>
        /// 添加检验分组项目
        /// </summary>
        /// <param name="alFeeDetail"></param>
        private void AddConfrimGroupItem(ArrayList alFeeDetail)
        {
            if (alFeeDetail.Count == 0)
            {
                return;
            }
            Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = alFeeDetail[0] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = feeItemList.ExecOper.Dept.Name;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 2].Text = "部位:";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = feeItemList.Order.CheckPartRecord;

            this.neuSpread1_Sheet1.RowCount++;
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = "  项目名称";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = "数量";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = "单价";
            this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = "金额";

            foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItem in alFeeDetail)
            {
                this.neuSpread1_Sheet1.RowCount++;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].ColumnSpan = 5;
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = feeItem.Item.Name;

                this.neuSpread1_Sheet1.RowCount++;

                //医保信息
                try
                {
                    //this.interfaceManager.GetCompareSingleItem("2", f.Item.ID, ref f.Compare);
                    this.interfaceManager.GetCompareSingleItem("2", feeItem.Item.ID, ref feeItem.Compare);
                }
                catch { }

                //甲乙类
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 0].Text = this.GetItemGrade(feeItem);
                if (feeItem.FeePack.Equals("1"))//包装单位
                {
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = (feeItem.Item.Qty / feeItem.Item.PackQty).ToString();
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = feeItem.Item.Price.ToString("F2");
                }
                else
                {
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 1].Text = feeItem.Item.Qty.ToString();
                    this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 3].Text = (feeItem.Item.Price / feeItem.Item.PackQty).ToString("F2");
                }
                //this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = (feeItem.FT.PubCost + feeItem.FT.PayCost + feeItem.FT.OwnCost).ToString("F2");
                this.neuSpread1_Sheet1.Cells[this.neuSpread1_Sheet1.RowCount - 1, 4].Text = ((feeItem.Item.Price / feeItem.Item.PackQty) * feeItem.Item.Qty).ToString("F2");
            }

        }

        /// <summary>
        /// 获取终端确认信息
        /// </summary>
        /// <param name="alFeeDetail"></param>
        /// <returns></returns>
        private ArrayList GetConfirmGroupItem(ArrayList alFeeDetail)
        {
            //分组显示
            ArrayList ulList = new ArrayList();
            int count = alFeeDetail.Count;
            for (int i = 0; i < count; i++)
            {
                Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = alFeeDetail[i] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                //检验分组
                if (feeItemList.Item.IsNeedConfirm)
                {
                    if (feeItemList.Order.CheckPartRecord == null)
                    {
                        feeItemList.Order.CheckPartRecord = string.Empty;
                    }
                    ulList.Add(feeItemList);
                }
            }

            for (int i = 0; i < ulList.Count; i++)
            {
                alFeeDetail.Remove(ulList[i]);
            }

            ArrayList sameNotes = new ArrayList();
            ArrayList ReturnList = new ArrayList();
            while (ulList.Count > 0)
            {
                Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = ulList[0] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                sameNotes.Add(feeItemList);
                for (int j = 1; j < ulList.Count; j++)
                {
                    Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemListCompare = ulList[j] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                    if (!feeItemListCompare.Item.IsNeedConfirm)
                    {
                        continue;
                    }
                    //执行科室一致，样本一致分到一组
                    if (feeItemList.ExecOper.Dept.ID.Equals(feeItemListCompare.ExecOper.Dept.ID) && feeItemList.Order.CheckPartRecord.Equals(feeItemListCompare.Order.CheckPartRecord))
                    {
                        sameNotes.Add(feeItemListCompare);
                    }

                }
                ReturnList.Add(sameNotes);
                for (int i = 0; i < sameNotes.Count; i++)
                {
                    ulList.Remove(sameNotes[i]);
                }
            }

            return ReturnList;
        }

        /// <summary>
        /// 添加第一部分
        /// </summary>
        /// <param name="register"></param>
        /// <param name="balanceList"></param>
        /// <param name="itemList"></param>
        private void AddList(Neusoft.HISFC.Models.Registration.Register register, ArrayList balanceList, ArrayList itemList)
        {
            this.AddHead();
            string invoiceTmp = (itemList[0] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList).Invoice.ID;
            DateTime dtTmp = (itemList[0] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList).FeeOper.OperTime;
            this.AddPatientInfo(register.Name, invoiceTmp, dtTmp);//register.InvoiceNO.ToString(), DateTime.Now);
            this.AddFeeHeader();
            this.AddFee(itemList);
        }

        #region 暂时不用
        ///// <summary>
        ///// 添加第二部分
        ///// </summary>
        ///// <param name="register"></param>
        ///// <param name="balanceList"></param>
        ///// <param name="itemList"></param>
        //private void AddGroupList(Neusoft.HISFC.Models.Registration.Register register, ArrayList balanceList, ArrayList itemList)
        //{
        //    //分组显示
        //    ArrayList alUL = this.GetLisGroupItem(itemList);
        //    ArrayList alDiagnoses = new ArrayList();
        //    alDiagnoses = (new Neusoft.HISFC.BizLogic.HealthRecord.Diagnose()).QueryDiagnoseNoOps(register.ID);
        //    string diagnoseName = "";
        //    if (alDiagnoses != null)
        //    {
        //        foreach (Neusoft.HISFC.Models.HealthRecord.Diagnose dg in alDiagnoses)
        //        {
        //            if (dg.Memo == register.ID || dg.Is30Disease == "0")
        //            {//把非本次挂号的诊断过滤掉，如果以后诊断很多，通过这种方式很慢的话，要重写一个业务层
        //                continue;
        //            }
        //            else
        //            {//把本次挂号的诊断连接起来放到lblDiagnose里
        //                diagnoseName += dg.DiagInfo.ICD10.Name + " ";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        diagnoseName = "";
        //    }

        //    //进行显示
        //    if (alUL.Count > 0)
        //    {
        //        foreach (ArrayList feeDetail in alUL)
        //        {
        //            this.AddSpace();
        //            this.AddSpace();
        //            this.AddLine();
        //            this.AddSpace();
        //            this.AddSpace();
        //            this.AddHospitalTipForUL();
        //            this.AddNameAndAge(register.Name, register.Sex.Name, register.Age, DateTime.Now);
        //            this.AddCardNOAndInvoice(register.PID.CardNO, balanceList);
        //            this.AddDiagnoseInfo(register.ID, diagnoseName);
        //            this.AddLisGroupItem(feeDetail);
        //        }
        //    }

        //    ArrayList alConfrim = this.GetConfirmGroupItem(itemList);
        //    //进行显示
        //    if (alConfrim.Count > 0)
        //    {
        //        foreach (ArrayList feeDetail in alConfrim)
        //        {
        //            this.AddSpace();
        //            this.AddSpace();

        //            this.AddLine();

        //            this.AddSpace();
        //            this.AddSpace();
        //            this.AddHospitalTipForUC();

        //            this.AddNameAndAge(register.Name, register.Sex.Name, register.Age, DateTime.Now);
        //            this.AddCardNOAndInvoice(register.PID.CardNO, balanceList);
        //            this.AddDiagnoseInfo(register.ID, diagnoseName);
        //            this.AddConfrimGroupItem(feeDetail);
        //        }
        //    }

        //}
        #endregion

        /// <summary>
        /// 显示费用信息
        /// </summary>
        /// <param name="itemList"></param>
        public void SetValue(Neusoft.HISFC.Models.Registration.Register register, ArrayList balanceList, ArrayList itemList)
        {
            this.InitHeader();
            this.AddList(register, balanceList, itemList);
            //this.AddGroupList(register, balanceList, itemList);
        }

        /// <summary>
        /// 返回高度
        /// </summary>
        /// <returns></returns>
        public int GetHeigth()
        {
            int height = Neusoft.FrameWork.Function.NConvert.ToInt32(this.neuSpread1_Sheet1.RowCount * this.neuSpread1_Sheet1.Rows.Default.Height) + 20;
            return this.Height > height ? this.Height + 5 : height;
        }
        private string GetItemGrade(Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList obj)
        {
            string result = "自费";
            if (obj == null || obj.Compare == null || obj.Compare.CenterItem == null)
            {
                return result;
            }
            if (!string.IsNullOrEmpty(obj.UndrugComb.Memo) && obj.UndrugComb.Memo == "复合项")
            {
                result = "";
            }
            switch (obj.Compare.CenterItem.ItemGrade)
            {
                case "1":
                    result = "甲类";
                    break;
                case "2":
                    result = "乙类";
                    break;
                default:
                    break;

            }
            return result;
        }
    }
}
