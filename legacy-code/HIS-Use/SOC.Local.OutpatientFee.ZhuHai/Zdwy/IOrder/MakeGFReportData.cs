using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOrder
{
    class MakeGFReportData : Neusoft.SOC.HISFC.BizProcess.MessagePatternInterface.IOrder
    {

        #region IOrder 成员

        public string Err
        {
            get { return ""; }
            set { }
        }

        public int SendDrugInfo(object patientInfo, System.Collections.ArrayList alFeeInfo, bool isPositive)
        {
            return 1;
        }

        public int SendFeeInfo(object regInfo, System.Collections.ArrayList alObj, bool isPositive, string flag)
        {
            return 1;
        }

        public int SendFeeInfo(object regInfo, System.Collections.ArrayList alObj, bool isPositive)
        {
            Neusoft.HISFC.Models.Registration.Register regPatientInfo = (Neusoft.HISFC.Models.Registration.Register)regInfo;
            if (regPatientInfo.Pact.PayKind.ID != "03")
            {
                return 1;
            }
            Manager manager = new Manager();
            if (isPositive == true)
            {
                decimal TotCost = 0.00M;
                decimal JZCost = 0.00M;
                decimal PubCost = 0.00M;
                decimal PayCost = 0.00M;
                decimal OwnCost = 0.00M;
                decimal OverLimitDrugFee = 0.00M;

                #region

                PubReport report = new PubReport();
                report.ID = manager.GetSeq("SEQ_GFHZ");//序号
                //report.Bill_No = manager.GetSeq("SEQ_GFHZ_BILLNO");//记账单号
                if (!string.IsNullOrEmpty(regPatientInfo.LSH))
                {
                    report.Bill_No = regPatientInfo.LSH;
                }
                else
                {
                    report.Bill_No = manager.GetSeq("SEQ_GFHZ_BILLNO");
                }
               
                string PactHead = manager.GetPactHeadByPact(regPatientInfo.Pact.ID);
                report.Pact.ID = PactHead;
                report.Amount = 1;//门诊人数
                //2 住院费用 1 门诊费用
                report.Fee_Type = "1";//门诊费用
                report.MCardNo = regPatientInfo.SSN;//医疗证号
                report.Pay_Rate = regPatientInfo.Pact.Rate.PayRate;//自负比例
                report.IsValid = "1";//有效
                report.Name = regPatientInfo.Name;//姓名

                #endregion

                #region
                if (alObj.Count > 0)
                {
                    report.Static_Month = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)alObj[0]).FeeOper.OperTime;//统计月份
                    report.Begin = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)alObj[0]).ChargeOper.OperTime;
                    report.End = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)alObj[0]).FeeOper.OperTime;
                    report.OperDate = manager.GetDateTimeFromSysDateTime();
                    report.OperCode = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)alObj[0]).FeeOper.ID;
                    report.InvoiceNo = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)alObj[0]).Invoice.ID;
                    report.Seq = int.Parse(((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)alObj[0]).InvoiceCombNO);
                    report.PatientNO = regPatientInfo.PID.CardNO;

                    #region 获取费用信息
                    DataTable dtInvoice = Function.GetGFReportDataFeeCodeStat();
                    Dictionary<int, decimal> dictionaryGFJZCost = new Dictionary<int, decimal>();
                    int seq = 0;
                    //药费
                    foreach (Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList in alObj)
                    {
                        TotCost += feeItemList.FT.PubCost + feeItemList.FT.PayCost + feeItemList.FT.OwnCost;
                        JZCost += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                        PubCost += feeItemList.FT.PubCost;
                        PayCost += feeItemList.FT.PayCost;
                        OwnCost += feeItemList.FT.OwnCost;
                        OverLimitDrugFee += feeItemList.FT.ExcessCost;

                        if (feeItemList.ItemRateFlag != "3")//记账的，自费的等
                        {
                            DataRow[] row = dtInvoice.Select("FEE_CODE = '" + feeItemList.Item.MinFee.ID + "'");
                            if (row == null || row.Length == 0)//没找到，则全部归到治疗费-5
                            {
                                if (dictionaryGFJZCost.ContainsKey((int)(EnumPubFeeType.ZhiLiao)) == false)
                                {
                                    dictionaryGFJZCost[(int)EnumPubFeeType.ZhiLiao] = 0;
                                }
                                dictionaryGFJZCost[(int)EnumPubFeeType.ZhiLiao] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                            }
                            else
                            {
                                seq = Neusoft.FrameWork.Function.NConvert.ToInt32(row[0]["SEQ"]);
                                if (dictionaryGFJZCost.ContainsKey(seq) == false)
                                {
                                    dictionaryGFJZCost[seq] = 0;
                                }
                                //if (objPactBH != null && objPactBH.ID.Length >= 0 && regPatientInfo.Name.IndexOf("省") >= 0 && feeItemList.Item.MinFee.ID == "030")
                                //{
                                //    dictionaryGFJZCost[(int)EnumPubFeeType.ShengZhen] += feeItemList.FT.PubCost;//省诊金
                                //}
                                //else if (feeItemList.Item.MinFee.ID == "030")
                                //{
                                //    dictionaryGFJZCost[(int)EnumPubFeeType.ZhenJin] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                //}
                                //else
                                //{
                                //    dictionaryGFJZCost[Neusoft.FrameWork.Function.NConvert.ToInt32(row[0]["SEQ"])] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                //}
                                dictionaryGFJZCost[Neusoft.FrameWork.Function.NConvert.ToInt32(row[0]["SEQ"])] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
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
                                    if (dictionaryGFJZCost.ContainsKey((int)EnumPubFeeType.TeJian) == false)
                                    {
                                        dictionaryGFJZCost[(int)EnumPubFeeType.TeJian] = 0;
                                    }
                                    dictionaryGFJZCost[(int)EnumPubFeeType.TeJian] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                }
                                else
                                {
                                    if (dictionaryGFJZCost.ContainsKey((int)EnumPubFeeType.SpDrugFeeTot) == false)
                                    {
                                        dictionaryGFJZCost[(int)EnumPubFeeType.SpDrugFeeTot] = 0;
                                    }
                                    dictionaryGFJZCost[(int)EnumPubFeeType.SpDrugFeeTot] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                }
                            }
                            else
                            {
                                DataRow[] row = dtInvoice.Select("FEE_CODE = '" + feeItemList.Item.MinFee.ID + "'");
                                if (row == null || row.Length == 0)//没找到，则全部归到治疗费-5
                                {
                                    if (dictionaryGFJZCost.ContainsKey((int)EnumPubFeeType.ZhiLiao) == false)
                                    {
                                        dictionaryGFJZCost[(int)EnumPubFeeType.ZhiLiao] = 0;
                                    }
                                    dictionaryGFJZCost[(int)EnumPubFeeType.ZhiLiao] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                }

                                if (feeItemList.FT.FTRate.User03 == "2")//高级仪器
                                {
                                    if (dictionaryGFJZCost.ContainsKey((int)EnumPubFeeType.TeJian) == false)
                                    {
                                        dictionaryGFJZCost[(int)EnumPubFeeType.TeJian] = 0;
                                    }
                                    dictionaryGFJZCost[(int)EnumPubFeeType.TeJian] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                }
                                else
                                {
                                    seq = Neusoft.FrameWork.Function.NConvert.ToInt32(row[0]["SEQ"]);
                                    if (dictionaryGFJZCost.ContainsKey(seq) == false)
                                    {
                                        dictionaryGFJZCost[seq] = 0;
                                    }
                                    dictionaryGFJZCost[Neusoft.FrameWork.Function.NConvert.ToInt32(row[0]["SEQ"])] += feeItemList.FT.PubCost + feeItemList.FT.PayCost;
                                }

                            }
                        }
                    }
                    #endregion

                    //最后赋值
                    foreach (KeyValuePair<int, decimal> de in dictionaryGFJZCost)
                    {
                        #region FY1-FY51
                        switch (de.Key)
                        {
                            case (int)EnumPubFeeType.Bed_Fee: report.Bed_Fee = de.Value;
                                break;
                            case (int)EnumPubFeeType.YaoPin: report.YaoPin = de.Value;
                                break;
                            case (int)EnumPubFeeType.ChengYao: report.ChengYao = de.Value;
                                break;
                            case (int)EnumPubFeeType.HuaYan: report.HuaYan = de.Value;
                                break;
                            case (int)EnumPubFeeType.JianCha: report.JianCha = de.Value;
                                break;
                            case (int)EnumPubFeeType.FangShe: report.FangShe = de.Value;
                                break;
                            case (int)EnumPubFeeType.ZhiLiao: report.ZhiLiao = de.Value;
                                break;
                            case (int)EnumPubFeeType.ShouShu: report.ShouShu = de.Value;
                                break;
                            case (int)EnumPubFeeType.ShuYang: report.ShuYang = de.Value;
                                break;
                            case (int)EnumPubFeeType.JieSheng: report.JieSheng = de.Value;
                                break;
                            case (int)EnumPubFeeType.GaoYang: report.GaoYang = de.Value;
                                break;
                            case (int)EnumPubFeeType.MR: report.MR = de.Value;
                                break;
                            case (int)EnumPubFeeType.CT: report.CT = de.Value;
                                break;
                            case (int)EnumPubFeeType.XueTou: report.XueTou = de.Value;
                                break;
                            case (int)EnumPubFeeType.ZhenJin: report.ZhenJin = de.Value;
                                break;
                            case (int)EnumPubFeeType.CaoYao: report.CaoYao = de.Value;
                                break;
                            case (int)EnumPubFeeType.TeJian: report.TeJian = de.Value;
                                break;
                            case (int)EnumPubFeeType.ShenYao: report.ShenYao = de.Value;
                                break;
                            case (int)EnumPubFeeType.JianHu: report.JianHu = de.Value;
                                break;
                            case (int)EnumPubFeeType.ShengZhen: report.ShengZhen = de.Value;
                                break;
                            case (int)EnumPubFeeType.SpDrugFeeTot: report.SpDrugFeeTot = de.Value;
                                break;
                            default: report.ZhiLiao = de.Value;
                                break;
                        }
                        #endregion
                    }

                    report.Tot_Cost = JZCost;
                    report.Pub_Cost = PubCost;
                    report.TotalFee = TotCost;
                    report.SelfPay = OwnCost;
                    report.OverLimitDrugFee = OverLimitDrugFee;
                    //report.Tot_Cost = TotCost - report.ShengZhen;
                    //report.Pub_Cost = PubCost - report.ShengZhen;

                    int iReturn = manager.InsertMZPubReport(report);

                    if (iReturn <= 0)
                    {
                        this.Err = "插入门诊记账单信息出错!" + manager.Err;
                        return -1;
                    }
                }
                #endregion

                return 1;
            }
            else
            {
                if (alObj.Count > 0)
                {
                    string invoiceNo = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)alObj[0]).Invoice.ID;
                    string invoiceSeq = ((Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)alObj[0]).InvoiceCombNO;
                    int iReturn = manager.DeleteMZPubReport(invoiceNo, invoiceSeq);
                    if (iReturn <= 0)
                    {
                        this.Err = "删除门诊记账单信息出错!" + manager.Err;
                        return -1;
                    }
                }
                return 1;
            }
        }

        public enum EnumPubFeeType
        {
            #region  FY1-FY51        
            /// <summary>
            /// 01床位费
            /// </summary>
            Bed_Fee=1,
            /// <summary>
            /// 02药品费
            /// </summary>
            YaoPin,
            /// <summary>
            /// 03成药费
            /// </summary>
            ChengYao,
            /// <summary>
            /// 04化验费
            /// </summary>
            HuaYan,
            /// <summary>
            /// 05检查费
            /// </summary>
            JianCha,
            /// <summary>
            /// 06放射费
            /// </summary>
            FangShe,
            /// <summary>
            /// 07治疗费
            /// </summary>
            ZhiLiao,
            /// <summary>
            /// 08手术费
            /// </summary>
            ShouShu,
            /// <summary>
            /// 09输血费
            /// </summary>
            ShuXue,
            /// <summary>
            /// 10输氧费
            /// </summary>
            ShuYang,
            /// <summary>
            /// 11接生费
            /// </summary>
            JieSheng,
            /// <summary>
            /// 12高氧费
            /// </summary>
            GaoYang,
            /// <summary>
            /// 13MR费
            /// </summary>
            MR,
            /// <summary>
            /// 14CT费
            /// </summary>
            CT,
            /// <summary>
            /// 15血透费 
            /// </summary>
            XueTou,
            /// <summary>
            /// 16诊金费
            /// </summary>
            ZhenJin,
            /// <summary>
            /// 17草药费
            /// </summary>
            CaoYao,
            /// <summary>
            /// 18特检费
            /// </summary>
            TeJian,
            /// <summary>
            /// 19审药费就是审批的肿瘤药费，特殊药品费用
            /// </summary>
            ShenYao,
            /// <summary>
            /// 20监护费
            /// </summary>
            JianHu,
            /// <summary>
            /// 51省诊费
            /// </summary>
            ShengZhen,
            /// <summary>
            /// 特殊药品费用
            /// </summary>
            SpDrugFeeTot,

            #endregion

            /// <summary>
            /// 总记账
            /// </summary>
            Tot_Cost,
            /// <summary>
            /// 实际记账
            /// </summary>
            Pub_Cost
         
        }

        #endregion
    }
}
