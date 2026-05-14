using System;
using System.Collections;
using System.Text;
using Neusoft.FrameWork.Models;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Registration;
using System.Data;
using Neusoft.HISFC.Models.Fee.Outpatient;
using Neusoft.HISFC.BizProcess.Interface.FeeInterface;
using Neusoft.SOC.HISFC.BizProcess.CommonInterface;
using System.Collections.Generic;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy
{
    public class Function
    {
        public static Neusoft.HISFC.BizLogic.Manager.DataBase dbMgr = new Neusoft.HISFC.BizLogic.Manager.DataBase();

        /// <summary>
        /// 返回项目比例
        /// </summary>
        /// <param name="pactId">合同单位编码</param>
        /// <param name="f">费用明细</param>
        /// <returns></returns>
        public static Neusoft.HISFC.Models.Base.PactItemRate PactRate(Neusoft.HISFC.Models.Registration.Register r, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList f, ref string errMsg)
        {
            Neusoft.HISFC.Models.Base.PactItemRate pRate = new Neusoft.HISFC.Models.Base.PactItemRate();
            pRate.Rate.RebateRate = 0;
            return pRate;
        }


        /// <summary>
        /// 返回执行科室
        /// </summary>
        /// <param name="recipeDept"></param>
        /// <param name="item"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public static ArrayList GetExecDept(Neusoft.FrameWork.Models.NeuObject recipeDept, Neusoft.HISFC.Models.Fee.Item.Undrug item, ref string errorInfo)
        {
            Neusoft.HISFC.BizProcess.Interface.Fee.IExecDept IExecDept = InterfaceManager.GetIExecDept();

            if (IExecDept != null)
            {
                return IExecDept.GetExecDept(recipeDept, item, ref errorInfo);
            }

            return null;
        }

        public static DataTable GetGFJZ()
        {
            Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
            DataSet ds = new DataSet();
            outpatientManager.GetInvoiceClass("MZJZ", ref ds);

            return ds.Tables[0];
        }

        public static string GetRecipeType(string CombNo) 
        {
            string sql = string.Format(@"SELECT m.recipe_flag FROM met_ord_recipedetail m WHERE m.comb_no = '{0}' AND ROWNUM = 1", CombNo);
            string RecipeType = dbMgr.ExecSqlReturnOne(sql);
            if (RecipeType == "2")
            {
                //自费
                return "(自费)";
            }
            else if (RecipeType == "6")
            {
                //门诊共济
                return "(门诊共济)";
            }
            else if (RecipeType == "7")
            {
                //门诊共济
                return "(门诊共济(谈判药))";
            }
            else if (RecipeType == "1")
            {
                //急救抢救
                return "(急救)";
            }
            else
            {
                //普通
                return "";
            }

        }

        public static string GetCombNoByRecipeSeq(string RecipeSeq)
        {
            string sql = string.Format(@"SELECT f.comb_no FROM fin_opb_feedetail f WHERE f.recipe_seq = '{0}' AND ROWNUM = 1", RecipeSeq);
            string result = dbMgr.ExecSqlReturnOne(sql);
            if (result == "-1")
            {
                return "";
            }
            return result;
        }


        /// <summary>
        /// 判断患者是否属于6岁以下儿童 AD248EC5-D724-3A06-A420-6BE15A9B1CA1
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="compareTime"></param>
        /// <param name="IsOutPatient"></param>
        /// <returns></returns>
        public static bool JudgeIsUnderSixYearsOld(string ID, string compareTime, bool IsOutPatient)
        {
            string sql = string.Empty;
            if (IsOutPatient)//门诊业务
            {
                sql = string.Format(@"select p.birthday from fin_opr_register p where p.clinic_code='{0}' and rownum=1
 ", ID);

            }
            else
            {
                sql = string.Format(@" select p.birthday from fin_ipr_inmaininfo p where p.inpatient_no='{0}' and rownum=1 
 ", ID);
            }
            string strbir = dbMgr.ExecSqlReturnOne(sql);
            DateTime dt = Convert.ToDateTime(strbir);
            DateTime now = Convert.ToDateTime(compareTime);
            int age = now.Year - dt.Year;
            if (now.Month < dt.Month)
            {
                age--;
            }
            else if (now.Month == dt.Month && now.Day < dt.Day)
            {
                age--;
            }
            if (age < 6)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 物价需求20211026 yhm 更改患者费用清单编码显示: AD248EC5-D724-3A06-A420-6BE15A9B1CA1
        ///一、编码更改为:国家编码+‘E’,同时满足以下规则：
        ///    1、6岁以下儿童(含6岁生日当天)患者
        ///    2、收取医疗服务项目为国家编码3开头的收费项目
        ///    3、收取价格为儿童价字段，且收费价格等于默认价的项目的1.3倍的项目
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="feeItem"></param>
        /// <param name="comPareTime"></param>
        /// <returns></returns>
        public static string GetCodeToPrint(string ID, Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItem, string comPareTime)
        {
            string code = string.Empty;
            Neusoft.HISFC.Models.Fee.Item.Undrug undrug = SOC.HISFC.BizProcess.Cache.Fee.GetItem(feeItem.Item.ID);
            bool isUnderSixYearsOld = JudgeIsUnderSixYearsOld(ID, comPareTime, true);
            string gbCode = string.Empty;
            decimal price = 0M;
            decimal zyMdtPrice = 0M;
            decimal mzMdtPrice = 0M;
            decimal weiChangPrice = 0M;
            Function.getUndrugPriceForCode(feeItem.Item.ID, ref gbCode, ref price, ref zyMdtPrice, ref mzMdtPrice, ref weiChangPrice);
            code = gbCode;
            if (isUnderSixYearsOld)
            {
                if ((!string.IsNullOrEmpty(gbCode) && (gbCode.Substring(0, 1).ToString() == "3")) && feeItem.Item.Price == price * 1.3M)
                {
                    code = code + "E";
                }
                else//判断是否需要加T 
                {

                        //未拆分项目 且收费价格为特需价格中：住院mdt价、门诊mdt价、围产中心价的其中一个字段，且收费金额大于默认价1.3倍以上(不含1.3倍)收费项目 需要加T
                        if ((feeItem.Item.Price == zyMdtPrice || feeItem.Item.Price == mzMdtPrice || feeItem.Item.Price == weiChangPrice) && feeItem.Item.Price > price * 1.3M)
                        {
                            code = code + "T";
                        }

                    
                }

            }//看是否需要加T 特需
            else
            {

                    //未拆分项目 且收费价格为特需价格中：住院mdt价、门诊mdt价、围产中心价的其中一个字段，且收费金额大于默认价1.3倍以上(不含1.3倍)收费项目 需要加T
                    if ((feeItem.Item.Price == zyMdtPrice || feeItem.Item.Price == mzMdtPrice || feeItem.Item.Price == weiChangPrice) && feeItem.Item.Price > price * 1.3M)
                    {
                        code = code + "T";
                    }

                
            }
            if (string.IsNullOrEmpty(code))
            {
                code = undrug.UserCode;
            }
            return code;

        }

        private static int getUndrugPriceForCode(string itemCode, ref string gbCode, ref decimal price, ref decimal zyMdtPrice, ref decimal mzMdtPrice, ref decimal weiChangPrice)
        {
            string sql = " select p.gb_code,p.unit_price,p.unit_price3,p.mdt_price,p.weichan_price from fin_com_undruginfo  p where p.item_code='{0}' ";
            int i = dbMgr.ExecQuery(string.Format(sql, itemCode));
            if (dbMgr.ExecQuery(string.Format(sql, itemCode)) == -1)
            {
                return -1;
            }
            while (dbMgr.Reader.Read())
            {
                gbCode = dbMgr.Reader[0].ToString();
                price = Neusoft.FrameWork.Function.NConvert.ToDecimal(dbMgr.Reader[1].ToString());
                zyMdtPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(dbMgr.Reader[2].ToString());
                mzMdtPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(dbMgr.Reader[3].ToString());
                weiChangPrice = Neusoft.FrameWork.Function.NConvert.ToDecimal(dbMgr.Reader[4].ToString());
            };
            return 1;
        }

    }
}
