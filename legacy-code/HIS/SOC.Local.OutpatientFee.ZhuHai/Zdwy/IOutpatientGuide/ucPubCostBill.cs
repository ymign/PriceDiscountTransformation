using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.FrameWork.Function;
using System.Collections;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOutpatientGuide
{
    public partial class ucPubCostBill : Neusoft.FrameWork.WinForms.Controls.ucBaseControl
    {
        public ucPubCostBill()
        {
            InitializeComponent();
        }

        public void SetValue(Neusoft.HISFC.Models.Registration.Register rInfo, System.Collections.ArrayList invoices, System.Collections.ArrayList feeDetails)
        {
            this.neuSpread1_Sheet1.Cells[1, 5].Value = rInfo.Name;//患者姓名
            this.neuSpread1_Sheet1.Cells[1, 8].Value = rInfo.SSN;//医疗证号
            this.neuSpread1_Sheet1.Cells[1, 11].Value = rInfo.Pact.Name;//合同单位名称

          
            if (invoices.Count > 0)
            {
                this.neuSpread1_Sheet1.Cells[1, 1].Value = (invoices[0] as Neusoft.HISFC.Models.Fee.Outpatient.Balance).BalanceOper.OperTime.ToString();
            }

            decimal TotCost = 0.00M;
            decimal PubCost = 0.00M;
            decimal PayCost = 0.00M;

            if (feeDetails.Count > 0)
            {
                DataTable dtInvoice = Function.GetGFJZ();
                Dictionary<int, decimal> dictionaryGFJZCost = new Dictionary<int, decimal>();
                int seq = 0;
                //药费
                foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList in feeDetails)
                {
                    TotCost += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                    PubCost += feeItemList.FT.PubCost;
                    PayCost += feeItemList.FT.PayCost;

                    if (feeItemList.ItemRateFlag != "3")//记账的，自费的等
                    {
                        DataRow[] row = dtInvoice.Select("FEE_CODE = '" + feeItemList.Item.MinFee.ID + "'");
                        if (row == null || row.Length == 0)//没找到，则全部归到治疗费-5
                        {
                            if (dictionaryGFJZCost.ContainsKey(5) == false)
                            {
                                dictionaryGFJZCost[5] = 0;
                            }
                            dictionaryGFJZCost[5] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                        }
                        else
                        {
                            seq = NConvert.ToInt32(row[0]["SEQ"]);
                            if (dictionaryGFJZCost.ContainsKey(seq) == false)
                            {
                                dictionaryGFJZCost[seq] = 0;
                            }
                            dictionaryGFJZCost[NConvert.ToInt32(row[0]["SEQ"])] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                        }
                    }
                    else//特批的
                    {
                        //特殊药品-7
                        if (feeItemList.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)
                        {
                            //如果是选高级仪器
                            if (feeItemList.FT.FTRate.User03 == "2")//高级仪器
                            {
                                if (dictionaryGFJZCost.ContainsKey(6) == false)
                                {
                                    dictionaryGFJZCost[6] = 0;
                                }
                                dictionaryGFJZCost[6] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                            }
                            else
                            {
                                if (dictionaryGFJZCost.ContainsKey(7) == false)
                                {
                                    dictionaryGFJZCost[7] = 0;
                                }
                                dictionaryGFJZCost[7] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                            }
                        }
                        else
                        {
                            if (feeItemList.FT.FTRate.User03 == "2")//高级仪器
                            {
                                if (dictionaryGFJZCost.ContainsKey(6) == false)
                                {
                                    dictionaryGFJZCost[6] = 0;
                                }
                                dictionaryGFJZCost[6] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                            }
                            else
                            {

                                DataRow[] row = dtInvoice.Select("FEE_CODE = '" + feeItemList.Item.MinFee.ID + "'");
                                if (row == null || row.Length == 0)//没找到，则全部归到治疗费-5
                                {
                                    if (dictionaryGFJZCost.ContainsKey(5) == false)
                                    {
                                        dictionaryGFJZCost[5] = 0;
                                    }
                                    dictionaryGFJZCost[5] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                }

                                if (row[0]["FEE_STAT_NAME"].ToString() == "高级仪器")//高级仪器-6
                                {
                                    if (dictionaryGFJZCost.ContainsKey(6) == false)
                                    {
                                        dictionaryGFJZCost[6] = 0;
                                    }
                                    dictionaryGFJZCost[6] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                }
                                else
                                {
                                    seq = NConvert.ToInt32(row[0]["SEQ"]);
                                    if (dictionaryGFJZCost.ContainsKey(seq) == false)
                                    {
                                        dictionaryGFJZCost[seq] = 0;
                                    }

                                    if (seq == 7)
                                    {
                                        dictionaryGFJZCost[7] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                    }
                                    else
                                    {
                                        dictionaryGFJZCost[NConvert.ToInt32(row[0]["SEQ"])] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                    }
                                }
                            }
                        }
                    }
                }

                //最后赋值
                foreach (KeyValuePair<int, decimal> de in dictionaryGFJZCost)
                {
                    this.neuSpread1_Sheet1.Cells[4, de.Key - 1].Value = de.Value;
                }

                //如果是查看
                if (rInfo.PrintInvoiceCnt != 3)
                {
                    this.neuSpread1_Sheet1.Cells[4, 10].Value = rInfo.SIMainInfo.PayCost + rInfo.SIMainInfo.PubCost;
                    this.neuSpread1_Sheet1.Cells[4, 11].Value = rInfo.SIMainInfo.PayCost;
                    this.neuSpread1_Sheet1.Cells[4, 12].Value = Neusoft.FrameWork.Public.String.FormatNumber(rInfo.Pact.Rate.PayRate, 2);
                    this.neuSpread1_Sheet1.Cells[4, 13].Value = rInfo.SIMainInfo.PubCost;
                }
                else
                {
                    if (TotCost != 0)
                    {
                        //查比例
                        rInfo.Pact = new Neusoft.HISFC.BizLogic.Fee.PactUnitInfo().GetPactUnitInfoByPactCode(rInfo.Pact.ID);
                        this.neuSpread1_Sheet1.Cells[4, 10].Value = TotCost;
                        this.neuSpread1_Sheet1.Cells[4, 11].Value = PayCost;
                        this.neuSpread1_Sheet1.Cells[4, 12].Value = Neusoft.FrameWork.Public.String.FormatNumber(rInfo.Pact.Rate.PayRate, 2);
                        this.neuSpread1_Sheet1.Cells[4, 13].Value = PubCost;
                    }
                }

            }
        }

        protected override int OnPrint(object sender, object neuObject)
        {
            this.neuSpread1.Sheets[0].PrintInfo.Preview = false;
            this.neuSpread1.PrintSheet(0);
            return base.OnPrint(sender, neuObject);
        }

        protected override int OnPrintPreview(object sender, object neuObject)
        {
            this.neuSpread1.Sheets[0].PrintInfo.Preview = true;
            this.neuSpread1.PrintSheet(0);
            return base.OnPrintPreview(sender, neuObject);
        }

        public override int Export(object sender, object neuObject)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "(*.xls)|*.xls|(*.xlsx)|*.xlsx";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.neuSpread1.SaveExcel(dlg.FileName);

                }
                catch
                {
                    this.neuSpread1.SaveExcel(dlg.FileName, FarPoint.Excel.ExcelSaveFlags.DataOnly);
                }
            }

                return 1;

            return base.Export(sender, neuObject);
        }
    }
}
