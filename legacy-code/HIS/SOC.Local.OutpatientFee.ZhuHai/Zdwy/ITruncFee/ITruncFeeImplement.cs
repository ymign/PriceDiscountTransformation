using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.ITruncFee
{
    public class ITruncFeeImplement:Neusoft.HISFC.BizProcess.Interface.Fee.ITruncFee
    {
        /// <summary>
        /// 金额位数
        /// </summary>
        private int median = 2;

        #region ITruncFee 成员

        public object[] TruncFee(object[] args)
        {
            object[] returnObj = null;
            #region 1.门诊收费界面
            if (args.Length >= 2 && (args[0] is Neusoft.HISFC.Models.Base.FT) && ((args[1] is Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList)))
            {
                Neusoft.HISFC.Models.Base.FT ft = args[0] as Neusoft.HISFC.Models.Base.FT;
                Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemLit = args[1] as Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList;
                ft.TotCost = Neusoft.FrameWork.Public.String.TruncateNumber(feeItemLit.Item.Price * feeItemLit.Item.Qty / feeItemLit.Item.PackQty, median);
                ft.RebateCost = Neusoft.FrameWork.Public.String.TruncateNumber(feeItemLit.FT.RebateCost * feeItemLit.Item.Qty / feeItemLit.Item.PackQty, median);
                returnObj = new object[] { ft };
            }
            #endregion
            return returnObj;
        }

        #endregion


        #region ITruncFee 成员

        public object TruncFee(object arg)
        {
            object returnObj = null;

            #region 2、门诊医嘱实体转化为费用实体
            if (arg is Neusoft.HISFC.Models.Order.OutPatient.Order)
            {
                Neusoft.HISFC.Models.Order.OutPatient.Order order = arg as Neusoft.HISFC.Models.Order.OutPatient.Order;
                returnObj = new Neusoft.HISFC.Models.Base.FT();
                //为NULL返回新实体
                if (order == null || order.FT == null)
                {
                    return returnObj;
                }
                
                ((Neusoft.HISFC.Models.Base.FT)returnObj).AdjustOvertopCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.AdjustOvertopCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).AirLimitCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.AirLimitCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).BalancedCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.BalancedCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).BalancedPrepayCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.BalancedPrepayCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).BedLimitCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.BedLimitCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).BedOverDeal = order.FT.BedOverDeal;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).BloodLateFeeCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.BloodLateFeeCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).BoardCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.BoardCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).BoardPrepayCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.BoardPrepayCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).DrugFeeTotCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.DrugFeeTotCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).TransferPrepayCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.TransferPrepayCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).TransferTotCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.TransferTotCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).DayLimitCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.DayLimitCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).DerateCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.DerateCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).FixFeeInterval = order.FT.FixFeeInterval;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).ID = order.FT.ID;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).LeftCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.LeftCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).OvertopCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.OvertopCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).DayLimitTotCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.DayLimitTotCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).Memo = order.FT.Memo;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).Name = order.FT.Name;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).OwnCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.OwnCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).FTRate.OwnRate = order.FT.FTRate.OwnRate;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).PayCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.PayCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).FTRate.PayRate = order.FT.FTRate.PayRate;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).PreFixFeeDateTime = order.FT.PreFixFeeDateTime;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).PrepayCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.PrepayCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).PubCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.PubCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).RebateCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.RebateCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).ReturnCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.ReturnCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).SupplyCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.SupplyCost, median);
                ((Neusoft.HISFC.Models.Base.FT)returnObj).TotCost = Neusoft.FrameWork.Public.String.TruncateNumber(order.FT.TotCost, median);

                ((Neusoft.HISFC.Models.Base.FT)returnObj).User01 = order.FT.User01;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).User02 = order.FT.User02;
                ((Neusoft.HISFC.Models.Base.FT)returnObj).User03 = order.FT.User03;
            }
            #endregion

            #region 3、传入金额直接调用转换
            if (arg is decimal)
            {
                returnObj = (object)Neusoft.FrameWork.Public.String.TruncateNumber(Neusoft.FrameWork.Function.NConvert.ToDecimal(arg), median);
            }
            #endregion

            #region 4、传入出库申请实体
            if (arg is Neusoft.HISFC.Models.Pharmacy.ApplyOut)
            {
                Neusoft.HISFC.Models.Pharmacy.ApplyOut applyOut = arg as Neusoft.HISFC.Models.Pharmacy.ApplyOut;
                returnObj = (object)Neusoft.FrameWork.Public.String.TruncateNumber(applyOut.Item.PriceCollection.RetailPrice * (applyOut.Operation.ApplyQty / applyOut.Item.PackQty),median);
            }
            #endregion

            #region 5、传入出库实体
            if (arg is Neusoft.HISFC.Models.Pharmacy.Output)
            {
                Neusoft.HISFC.Models.Pharmacy.Output output = arg as Neusoft.HISFC.Models.Pharmacy.Output;
                returnObj = (object)Neusoft.FrameWork.Public.String.TruncateNumber(output.Item.PriceCollection.RetailPrice * (output.Quantity / output.Item.PackQty),median);
            }
            #endregion

            return returnObj;
        }

        #endregion
    }
}
