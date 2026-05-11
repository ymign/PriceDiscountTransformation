using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Fee.Item;
using Neusoft.FrameWork.Function;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IGetItemPrice
{
    public class ItemPrice : Neusoft.FrameWork.Management.Database, Neusoft.HISFC.BizProcess.Interface.Fee.IGetItemPrice
    {
        Neusoft.HISFC.BizLogic.Fee.Item itemManager = new Neusoft.HISFC.BizLogic.Fee.Item();
        Neusoft.SOC.HISFC.Fee.BizLogic.Undrug undrugManagemt = new Neusoft.SOC.HISFC.Fee.BizLogic.Undrug();

        string sql = @"select t.item_code,t.unit_price,t.unit_price1,t.unit_price3,t.GB_CODE,t.mark5 from fin_com_undruginfo t where t.item_code='{0}'";

        string mdtsql = @"select t.item_code,t.unit_price,t.unit_price1,t.unit_price2,t.MDT_PRICE from fin_com_undruginfo t where t.item_code='{0}'";


        #region IGetItemPrice 成员
        /// <summary>
        /// 获取价格
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <param name="register">当前门诊患者实体</param>
        /// <param name="UnitPrice">三甲价（基本价）</param>
        /// <param name="ChildPrice">儿童价</param>
        /// <param name="SPPrice">特诊价</param>
        /// <param name="PurchasePrice">购入价</param>
        /// <param name="orgPrice"></param>
        /// <returns></returns>
        public decimal GetPrice(string itemCode, Neusoft.HISFC.Models.Registration.Register register, decimal UnitPrice, decimal ChildPrice, decimal SPPrice, decimal PurchasePrice, ref decimal orgPrice)
        {
            decimal num;
            Undrug undrug;
            
            orgPrice = UnitPrice;
            string gbCode ="";//国标码
            string ISChildPrice = "";//是否需要收取儿童价

            if (Neusoft.FrameWork.Management.Connection.Hospital.ID != "CORE_HIS50")//校区门诊
            {
                #region MyRegion
                System.Collections.Hashtable hsFeeCode = GetFeeCodeHs("11");

                if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")//非药品 
                {
                    //this.getPrice(itemCode, ref UnitPrice, ref ChildPrice, ref SPPrice);
                    //旧的价格获取方式
                    //if (SPPrice == 0m || SPPrice.ToString() == "")//表的字段是UNIT_PRICE2特诊价
                    //{
                    //    return UnitPrice;
                    //}
                    //else
                    //{
                    //    return UnitPrice * SPPrice;
                    //}

                    //新的获取非药品项目价格方式
                    Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                    if (GetItemFeeCodeAndSchoolPrice(itemCode, ref obj) < 0 || hsFeeCode.Count < 0)
                    {
                        if (SPPrice == 0m || SPPrice.ToString() == "")//表的字段是UNIT_PRICE2特诊价
                        {
                            return UnitPrice;
                        }
                        else
                        {
                            return UnitPrice * SPPrice;
                        }
                    }
                    if (hsFeeCode.Contains(obj.ID))//材料费
                    {
                        return ChildPrice;//儿童价，表数据是unit_price1，HERP通过平台把价格传过来
                    }
                    else//医疗服务项目
                    {
                        if (string.IsNullOrEmpty(obj.Memo))
                        {
                            return UnitPrice * SPPrice;
                        }
                        return Neusoft.FrameWork.Function.NConvert.ToDecimal(obj.Memo);//校区价格school_price
                    }

                }
                else//药品
                {
                    Neusoft.HISFC.Models.Pharmacy.Item item = Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemCode);
                    //中成药，中草药，二类疫苗 不走零差价
                    //zzf 2020-09-09 疫苗走零差价了
                    if (item.Type.ID.ToString() == "C")// || item.ExtendData2.ToString() == "Y")
                    {
                        if (item.PriceCollection.RetailPrice != 0)
                        {
                            return item.PriceCollection.RetailPrice;
                        }
                        else
                        {
                            return UnitPrice;
                        }
                    }
                    else
                    {
                        if (PurchasePrice != 0)
                        {
                            return PurchasePrice;
                        }
                        else
                        {
                            decimal retailPrice2 = item.RetailPrice2;
                            if (retailPrice2 == 0)
                            {
                                return UnitPrice;
                            }
                            else
                            {
                                return retailPrice2;
                            }
                        }
                    } 
                } 
                #endregion
            }
            if (register != null && register.Pact != null && register.Pact.PriceForm == "购入价")
            {
                if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "Y")//药品
                {
                    #region 药品
                    Neusoft.HISFC.Models.Pharmacy.Item item = Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemCode);
                    //中成药，中草药，二类疫苗 不走零差价
                    //zzf 2020-09-09 疫苗走零差价了
                    if (item.Type.ID.ToString() == "C")// || item.ExtendData2.ToString() == "Y")
                    {
                        if (item.PriceCollection.RetailPrice != 0)
                        {
                            return item.PriceCollection.RetailPrice;
                        }
                        else
                        {
                            return UnitPrice;
                        }
                    }
                    else
                    {
                        if (PurchasePrice != 0)
                        {
                            return PurchasePrice;
                        }
                        else
                        {
                            decimal retailPrice2 = item.RetailPrice2;
                            if (retailPrice2 == 0)
                            {
                                return UnitPrice;
                            }
                            else
                            {
                                return retailPrice2;
                            }
                        }
                    } 
                    #endregion
                }
                else if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")
                {
                    #region 非药品
                    //6岁及以下儿童临床诊疗类项目加收30%
                    //undrug = Neusoft.SOC.HISFC.BizProcess.Cache.Fee.GetItem(itemCode);
                    getPrice(itemCode, ref UnitPrice, ref ChildPrice, ref SPPrice,ref gbCode, ref ISChildPrice);
                    if ((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date))//6岁及以下儿童
                    {
                            decimal spPrice = 0;
                            if (this.GetChildSpPrice(itemCode, ref spPrice) == -1)//获取儿童价，临床诊疗类加收30%的价格
                            {
                                spPrice = UnitPrice;
                            }
                            if (spPrice <= 0)
                            {
                                spPrice = UnitPrice;
                            }
                            return spPrice;//儿童价
                    }
                    //if ((((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date)) && !string.IsNullOrEmpty(gbCode)) && (gbCode.Substring(0, 1).ToString() == "3"))//6岁以下儿童，并且是临床诊疗类项目
                    //{
                    //    decimal spPrice = 0;
                    //    if (this.GetChildSpPrice(itemCode, ref spPrice) == -1)//获取儿童价，临床诊疗类加收30%的价格
                    //    {
                    //        spPrice = UnitPrice;
                    //    }
                    //    if (spPrice <= 0)
                    //    {
                    //        spPrice = UnitPrice;
                    //    }
                    //    return spPrice;//儿童价
                    //}
                    return UnitPrice; 
                    #endregion
                }
                else
                {
                    return UnitPrice;
                }
            }
            else if (register != null && register.Pact != null && register.Pact.PriceForm == "MDT价")
            {
                #region 药品
                if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "Y")
                {
                    Neusoft.HISFC.Models.Pharmacy.Item item = Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemCode);
                    //中成药，中草药，二类疫苗 不走零差价
                    if (item.Type.ID.ToString() == "C" )//|| item.ExtendData2.ToString() == "Y")
                    {
                        if (item.PriceCollection.RetailPrice != 0)
                        {
                            return item.PriceCollection.RetailPrice;
                        }
                        else
                        {
                            return UnitPrice;
                        }
                    }
                    else
                    {
                        if (PurchasePrice != 0)
                        {
                            return PurchasePrice;
                        }
                        else
                        {
                            decimal retailPrice2 = item.RetailPrice2;
                            if (retailPrice2 == 0)
                            {
                                return UnitPrice;
                            }
                            else
                            {
                                return retailPrice2;
                            }
                        }
                    }
                }
                #endregion
                    
                #region  非药品
                //else if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")
                //{
                //    decimal mdtpric = 0;
                //    int i = this.ExecQuery(string.Format(mdtsql, itemCode));
                //    if (i > 0)
                //    {
                //        if (this.Reader != null)
                //        {
                //            try
                //            {
                //                if (this.Reader.Read())
                //                {
                //                    mdtpric = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[4]);
                //                    SPPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[3]);
                //                }
                //            }
                //            finally
                //            {
                //                if (this.Reader.IsClosed == false)
                //                {
                //                    this.Reader.Close();
                //                }
                //                if (mdtpric > 0)
                //                {
                //                    UnitPrice = mdtpric;
                //                    decimal ssss = 0.0m;
                //                    //undrug = Neusoft.SOC.HISFC.BizProcess.Cache.Fee.GetItem(itemCode);
                //                    getPrice(itemCode, ref ssss, ref ChildPrice, ref SPPrice, ref gbCode, ref ISChildPrice);
                //                   // 6岁及以下儿童临床诊疗类项目加收30%
                //                    if ((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date))//6岁及以下儿童
                //                    {
                //                        UnitPrice = ChildPrice;
                //                    }


                //                    //if ((((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date)) && !string.IsNullOrEmpty(undrug.GBCode)) && (undrug.GBCode.Substring(0, 1).ToString() == "3"))//6岁以下儿童，并且是临床诊疗类项目
                //                    //{
                //                    //    UnitPrice *= 1.3M;
                //                    //}

                //                }
                //            }
                //        }
                //    }

                //    return UnitPrice;


                //}
                #endregion
                else
                {
                    return UnitPrice;
                }
            }
            else if (register != null && register.Pact != null && register.Pact.PriceForm == "围产中心价")//20190821  {D5525B74-0581-41fe-962D-76C2C7C800E2}
            {
                if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "Y")//药品
                {
                    #region 药品
                    Neusoft.HISFC.Models.Pharmacy.Item item = Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemCode);
                    //中成药，中草药，二类疫苗 不走零差价
                    if (item.Type.ID.ToString() == "C" )//|| item.ExtendData2.ToString() == "Y")
                    {
                        if (item.PriceCollection.RetailPrice != 0)
                        {
                            return item.PriceCollection.RetailPrice;
                        }
                        else
                        {
                            return UnitPrice;
                        }
                    }
                    else
                    {
                        if (PurchasePrice != 0)
                        {
                            return PurchasePrice;
                        }
                        else
                        {
                            decimal retailPrice2 = item.RetailPrice2;
                            if (retailPrice2 == 0)
                            {
                                return UnitPrice;
                            }
                            else
                            {
                                return retailPrice2;
                            }
                        }
                    } 
                    #endregion
                }
                //else if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")//非药品
                //{
                //    #region 非药品
                //    Neusoft.SOC.HISFC.Fee.Models.Undrug undrugItem = this.undrugManagemt.GetUndrug(itemCode);
                //    if (undrugItem == null||undrugItem.WeiChan_Price == 0)
                //    {
                //        return UnitPrice;
                //    }

                //    UnitPrice = undrugItem.WeiChan_Price;
                //    SPPrice = undrugItem.SpecialPrice;
                //    //6岁及以下儿童临床诊疗类项目加收30%
                //    if ((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date))//6岁及以下儿童
                //    {
                //        UnitPrice = undrugItem.ChildPrice;
                //    }

                //    //if ((((register.Birthday > DateTime.MinValue) && (register.Birthday.AddYears(6) >= DateTime.Now.Date)) && !string.IsNullOrEmpty(undrugItem.NameCollection.GbCode)) && (undrugItem.NameCollection.GbCode.Substring(0, 1).ToString() == "3"))
                //    //{
                //    //    UnitPrice *= 1.3M;
                //    //}

                //    return UnitPrice;
                //    #endregion
                //}
                else
                {
                    return UnitPrice;
                }
            }
            else
            {
                return UnitPrice;
            }
        }

        public decimal GetPriceForInpatient(string itemCode, Neusoft.HISFC.Models.RADT.PatientInfo patientInfo, decimal UnitPrice, decimal ChildPrice, decimal SPPrice, decimal PurchasePrice, ref decimal orgPrice)
        {
            if (patientInfo != null && patientInfo.Pact != null && !string.IsNullOrEmpty(patientInfo.Pact.ID) )
            {
                if ((!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "Y")&& SOC.HISFC.BizProcess.Cache.Fee.GetPactUnitInfo(patientInfo.Pact.ID).PriceForm == "购入价")
                {
                    #region 药品
                    Neusoft.HISFC.Models.Pharmacy.Item item = Neusoft.SOC.HISFC.BizProcess.Cache.Pharmacy.GetItem(itemCode);
                    //中成药，中草药，二类疫苗 不走零差价
                    if (item.Type.ID.ToString() == "C")// || item.ExtendData2.ToString() == "Y")
                    {
                        orgPrice = UnitPrice;
                        return UnitPrice;
                    }
                    else
                    {
                        decimal defaultPrice = ((Neusoft.HISFC.Models.Pharmacy.Item)item).PriceCollection.RetailPrice;
                        decimal purchasePrice = ((Neusoft.HISFC.Models.Pharmacy.Item)item).RetailPrice2;
                        orgPrice = UnitPrice;
                        if (purchasePrice != 0)
                        {
                            return purchasePrice;
                        }
                        else
                        {
                            decimal retailPrice2 = item.RetailPrice2;
                            if (retailPrice2 == 0)
                            {
                                return defaultPrice;
                            }
                            else
                            {
                                return retailPrice2;
                            }
                        }
                    } 
                    #endregion
                }
                else if (!string.IsNullOrEmpty(itemCode) && itemCode.Substring(0, 1) == "F")//非药品
                {
                    #region 非药品
                    decimal spPrice = 0.00M;
                    string ISChildPrice = "";//是否需要收取儿童价
                    string gbCode = "";//国标码
                    decimal sssssPrice = 0.00M;

                    Undrug undrug = Neusoft.SOC.HISFC.BizProcess.Cache.Fee.GetItem(itemCode);
                    getPrice(itemCode, ref UnitPrice, ref ChildPrice, ref SPPrice, ref gbCode, ref ISChildPrice);//初始化一下价格20191128

                    //凤凰国际高端病区价格
                    //if (this.SpecialDept.Split(',').ToList<string>().Where(x => x == patientInfo.PVisit.PatientLocation.Dept.ID).Count() > 0)//凤凰国际高端病区
                    //{
                    //    if (this.GetInpatientItemSpPrice(itemCode, ref spPrice, ref orgPrice) == 1)//获取高端价与原始价
                    //    {
                    //        if (spPrice > 0)
                    //        {
                    //            UnitPrice = spPrice;
                    //        }
                    //    }
                    //}

                    //围产中心高端价格
                    //if (this.SpecialDeptWC.Split(',').ToList<string>().Where(x => x == patientInfo.PVisit.PatientLocation.Dept.ID).Count() > 0)//围产中心高端病区
                    //{
                    //    if (this.GetInpatientItemWcPrice(itemCode, ref spPrice, ref orgPrice) == 1)//获取围产中心高端价与原始价
                    //    {
                    //        if (spPrice > 0)
                    //        {
                    //            UnitPrice = spPrice;
                    //        }
                    //    }
                    //}

                    //6岁及以下儿童临床诊疗类项目加收30%
                    if ((patientInfo.Birthday > DateTime.MinValue) && (patientInfo.Birthday.AddYears(6) >= DateTime.Now.Date))
                    {
                        //if ((!string.IsNullOrEmpty(gbCode) && (gbCode.Substring(0, 1).ToString() == "3")) || ISChildPrice == "1")//国标码为3开头
                        //{
                        //    if (spPrice > 0M)//高端病区
                        //    {
                        //        return (spPrice * 1.3M);
                        //    }
                            if (this.GetChildSpPrice(itemCode, ref spPrice) == -1)
                            {
                                return UnitPrice;
                            }
                            if (spPrice > 0M)
                            {
                                UnitPrice = spPrice;
                            }
                        //}
                    }

                    //if ((((patientInfo.Birthday > DateTime.MinValue) && (patientInfo.Birthday.AddYears(6) >= DateTime.Now.Date)) && !string.IsNullOrEmpty(undrug.GBCode)) && (undrug.GBCode.Substring(0, 1).ToString() == "3"))//6岁及以下儿童，且是临床诊疗类项目
                    //{
                    //    if (spPrice > 0M)//高端病区
                    //    {
                    //        return (spPrice * 1.3M);
                    //    }
                    //    if (this.GetChildSpPrice(itemCode, ref spPrice) == -1)
                    //    {
                    //        return UnitPrice;
                    //    }
                    //    if (spPrice > 0M)
                    //    {
                    //        UnitPrice = spPrice;
                    //    }
                    //}
                    return UnitPrice; 
                    #endregion
                }
                else
                {
                    return UnitPrice;
                }
            }
            else
            {
                orgPrice = UnitPrice;
                return UnitPrice;
            }
        }

        /// <summary>
        /// 初始化价格
        /// </summary>
        /// <param name="itemCode">项目编号</param>
        /// <param name="UnitPrice">原价</param>
        /// <param name="ChildPrice">儿童价</param>
        /// <param name="SPPrice">特诊价</param>
        /// <param name="ISChildPrice"></param>
        /// <returns></returns>
        private int getPrice(string itemCode, ref decimal UnitPrice, ref decimal ChildPrice, ref decimal SPPrice,ref string gbCode, ref string ISChildPrice)
        {
            int i = this.ExecQuery(string.Format(sql, itemCode));
            if (this.ExecQuery(string.Format(sql, itemCode)) == -1)
            {
                return -1;
            }
            while (this.Reader.Read())
            {
                UnitPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[1]);
                ChildPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[2]);
                SPPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[3]);
                gbCode = this.Reader[4].ToString();
                ISChildPrice = this.Reader[5].ToString();
            };
            return 1;
        }

        /// <summary>
        /// 获取住院MDT价
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="spPrice"></param>
        /// <param name="orgPrice"></param>
        /// <returns></returns>
        private int GetInpatientItemSpPrice(string itemCode, ref decimal spPrice,ref decimal orgPrice)
        {
            string sql = "select t.item_code,t.unit_price,t.unit_price1,t.unit_price2,t.MDT_PRICE,t.unit_price3 from fin_com_undruginfo t where t.item_code='{0}'";
            if (this.ExecQuery(string.Format(sql, itemCode)) == -1)
            {
                return -1;
            }
            while (this.Reader.Read())
            {
                orgPrice=Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[1]);
                spPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[5]);
            };
            return 1;
        }

        /// <summary>
        /// 获取围产中心价格
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="spPrice"></param>
        /// <param name="orgPrice"></param>
        /// <returns></returns>
        private int GetInpatientItemWcPrice(string itemCode, ref decimal spPrice, ref decimal orgPrice)
        {
            string sql = "select t.item_code,t.unit_price,t.unit_price1,t.unit_price2,t.MDT_PRICE,t.weichan_price from fin_com_undruginfo t where t.item_code='{0}'";
            if (this.ExecQuery(string.Format(sql, itemCode)) == -1)
            {
                return -1;
            }
            while (this.Reader.Read())
            {
                orgPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[1]);
                spPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[5]);
            };
            return 1;
        }

        string spDept;
        /// <summary>
        /// 凤凰国际高端病区
        /// </summary>
        public string SpecialDept
        {
            get
            {
                if (string.IsNullOrEmpty(spDept))
                {
                    spDept = this.ExecSqlReturnOne("select wm_concat(a.code) from com_dictionary a where a.type ='INPATIENTSPDEPT' and a.valid_state='1'");
                }
                return spDept;
            }
            set { spDept = value; }
        }

        string spDeptWC;
        /// <summary>
        /// 围产中心高端病区
        /// </summary>
        public string SpecialDeptWC
        {
            get
            {
                if (string.IsNullOrEmpty(spDeptWC))
                {
                    spDeptWC = this.ExecSqlReturnOne("select wm_concat(a.code) from com_dictionary a where a.type ='INPATIENTSPDEPT2' and a.valid_state='1'");
                }
                return spDeptWC;
            }
            set { spDeptWC = value; }
        }

        /// <summary>
        /// 获取费用与统计大类，用来区分项目是材料费还是医疗服务项目by zhaoyq 20180910
        /// </summary>
        /// <param name="fee_stat_cate">统计大类，11为材料费</param>
        /// <returns></returns>
        private System.Collections.Hashtable GetFeeCodeHs(string fee_stat_cate)
        {
            System.Collections.Hashtable hsFeeCode = new System.Collections.Hashtable();
            string strSql = " select fee_stat_name,fee_code from fin_com_feecodestat where report_code = 'MZ01' and fee_stat_cate = '{0}'";
            if (this.ExecQuery(string.Format(strSql, fee_stat_cate)) == -1)
            {
                return hsFeeCode;
            }
            while (this.Reader.Read())
            {
                Neusoft.FrameWork.Models.NeuObject obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.Name = this.Reader[0].ToString();//项目分类名称
                obj.ID = this.Reader[1].ToString();//最小费用代码
                hsFeeCode.Add(obj.ID, obj);
            };
            return hsFeeCode;
        }

        /// <summary>
        /// 获取最小费用代码和校区门诊价格by zhaoyq 20180910
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int GetItemFeeCodeAndSchoolPrice(string itemCode , ref Neusoft.FrameWork.Models.NeuObject obj)
        {
            string strSql = " select fee_code,school_price,item_name from fin_com_undruginfo where item_code = '{0}' and valid_state = fun_get_valid";
            if (this.ExecQuery(string.Format(strSql, itemCode)) == -1)
            {
                return -1;
            }
            while (this.Reader.Read())
            {
                obj.ID = this.Reader[0].ToString();//最小费用代码
                obj.Memo = this.Reader[1].ToString();//校区门诊价格shcool_price
                obj.Name = this.Reader[2].ToString();//项目名称
            }
            return 1;
        }

        private int GetChildSpPrice(string itemCode, ref decimal spPrice)
        {
            try
            {
                if (base.ExecQuery(string.Format(this.sql, itemCode)) == -1)
                {
                    return -1;
                }
                while (base.Reader.Read())
                {
                    spPrice = NConvert.ToDecimal(base.Reader[2]);//unit_price1 字段
                }
            }
            catch (Exception exception)
            {
                base.Err = exception.Message;
                return -1;
            }
            return 1;
        }


        #endregion
    }
}
