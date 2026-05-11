using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kmOrder = Neusoft.HISFC.BizLogic.Order;
using Neusoft.HISFC.Models.KangMei;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Integrate
{
    public class KmOrder : IntegrateBase
    {
       static kmOrder.KangMei km = new Neusoft.HISFC.BizLogic.Order.KangMei();

        private static string kmCode = string.Empty;
        public static string KmDeptCdoe
        {
            get
            {
                if (string.IsNullOrEmpty(kmCode))
                {
                    string code = string.Empty;
                    if (km.GetKmDrugDept(ref code) == -1)
                    {
                        kmCode = "9092";
                        return kmCode;
                    }
                    if (string.IsNullOrEmpty(code))
                    {
                        kmCode = "9092";
                        return kmCode;
                    }
                    kmCode = code;
                }
                return kmCode;
            }
        }

      

        //public int AddressQueryByCardNo(string cardNo, ref List<OrderAddress> list)
        //{
        //    return km.AddressQueryByCardNo(cardNo, ref list);
        //}

         /// <summary>
        /// 查询树结构地址列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressBaseQueryByParentCode(string code, ref List<AddressBase> list)
        {
            return km.AddressBaseQueryByParentCode(code, ref list);
        }
         /// <summary>
        /// 查询树结构地址列表所有
        /// </summary>
        /// <param name="code"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressBaseQueryByParentCodeALL( ref List<AddressBase> list)
        {
            return km.AddressBaseQueryByParentCodeALL( ref list);
        }
        
         /// <summary>
        /// 地址查询，按卡号（门诊号）
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressQueryByCardNo(string cardNo, ref List<OrderAddress> list)
        {
            return km.AddressQueryByCardNo(cardNo, ref list);
        }

           /// <summary>
        /// 订单送货地址新增
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public int AddressInsert(OrderAddress addr)
        {
            return km.AddressInsert(addr);
        }

         /// <summary>
        /// 订单送货地址修改
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public int AddressUpdate(OrderAddress addr)
        {
            return km.AddressUpdate(addr);
        }
         /// <summary>
        /// 取订单地址的流水号
        /// </summary>
        /// <returns></returns>
        public string AddressSeq()
        {
            return km.AddressSeq();
        }



         /// <summary>
        /// 查询订单，按处方号
        /// </summary>
        /// <param name="receipeNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int OrderQueryByReceipeNo(string recipeNo, ref List<KangMeiOrder> list)
        {
            return km.OrderQueryByReceipeNo(recipeNo, ref list);
        }
        /// <summary>
        /// 查询订单，按处方号
        /// </summary>
        /// <param name="receipeNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int OrderQueryByClinicNO(string ClinicNO, ref List<KangMeiOrder> list)
        {
            return km.OrderQueryByClinicNO(ClinicNO, ref list);
        }
        
          /// <summary>
        /// 修改
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int OrderUpdate(KangMeiOrder order)
        {
            return km.OrderUpdate(order);
        }

        /// <summary>
        /// 更改默认地址
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public int AddressUpdateDefault(OrderAddress addr)
        {
            return km.AddressUpdateDefault(addr);
        }

        /// <summary>
        /// 取患者基本信息
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public  Neusoft.HISFC.Models.RADT.PatientInfo GetPatientInfoByCardNo(string cardNo)
        {

             return new Neusoft.HISFC.BizLogic.RADT.OutPatient().PatientQuery(cardNo);
        }



        public int CreateKmOrder(ArrayList alOrder)
        {
            Hashtable hs = new Hashtable();
            foreach (Neusoft.HISFC.Models.Order.Order item in alOrder)
            {
                if (hs.Contains(item.ReciptNO))
                    continue;
                hs.Add(item.ReciptNO, item);
            }
            foreach (string key in hs.Keys)
            {
                Neusoft.HISFC.Models.Order.Order order = (Neusoft.HISFC.Models.Order.Order)hs[key];
                Neusoft.HISFC.Models.KangMei.KangMeiOrder kmOrder = new KangMeiOrder();
                kmOrder.ID = km.OrderSeq();
                kmOrder.RecipeNo = order.ReciptNO;
                kmOrder.ClinicCode = order.Patient.ID;
                kmOrder.CardNo = order.Patient.PID.CardNO;
                kmOrder.PatientName = order.Patient.Name;
                kmOrder.Sex = order.Patient.Sex.ID.ToString();
                kmOrder.Consignee = order.Patient.Name;
                kmOrder.OrderDate = DateTime.Now;
                kmOrder.DrugDeptCode = order.StockDept.ID;
                kmOrder.OrderNo = order.Combo.ID;
                kmOrder.IsSend = "1";
                kmOrder.IsCook = "1";
                kmOrder.Phone = order.Patient.PhoneBusiness;
                kmOrder.Tel = order.Patient.PhoneHome;
                kmOrder.State = "1";
            }
            return 1;
        }

        /// <summary>
        /// Neusoft.HISFC.Models.Order.Inpatient.Order
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int UpdateRecipeNo(ArrayList list, ref string err)
        {
            return km.UpdateRecipeNoByOrderConfirm(list, ref err);
        }

    }
}
