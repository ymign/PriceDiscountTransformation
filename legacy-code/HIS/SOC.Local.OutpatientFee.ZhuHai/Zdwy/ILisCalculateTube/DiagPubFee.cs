using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.ILisCalculateTube
{
    public class DiagPubFee
    {
        Neusoft.HISFC.BizLogic.Fee.Outpatient outpatientManager = new Neusoft.HISFC.BizLogic.Fee.Outpatient();
        Neusoft.HISFC.BizLogic.Registration.RegLvlFee regLvlFeeMgr = new Neusoft.HISFC.BizLogic.Registration.RegLvlFee();

        private string roundFeeItemCode = "F00000049496";

        public Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList  GetFeeItemList(Neusoft.HISFC.Models.Registration.Register r,decimal qty , string recipeSequence,ref string errorInfo)
        {
        
            DataSet dsItemNow = new DataSet();
            int iReturn = this.outpatientManager.QueryItemList("undrug", roundFeeItemCode, ref dsItemNow);
            if (iReturn == -1)
            {
                errorInfo = "获得诊金费用项目出错!" + this.outpatientManager.Err;
                return null;
            }
            DataRow findRowNow;
            DataRow[] rowFindsNow = dsItemNow.Tables[0].Select("ITEM_CODE = " + "'" + roundFeeItemCode + "' and drug_flag = '0'");

            if (rowFindsNow == null || rowFindsNow.Length == 0)
            {
                errorInfo = "编码为: [" + this.roundFeeItemCode + " ] 的项目查找失败!";
                return null;
            }

            Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList feeItemList = new Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList();
            feeItemList.Item = new Neusoft.HISFC.Models.Fee.Item.Undrug();
            feeItemList.Item.ItemType = Neusoft.HISFC.Models.Base.EnumItemType.UnDrug;
            feeItemList.IsGroup = false;
            feeItemList.Item.ID = roundFeeItemCode;
            feeItemList.ID = roundFeeItemCode;

            findRowNow = rowFindsNow[0];

            feeItemList.Item.UserCode = findRowNow["User_Code"].ToString();
            feeItemList.Item.Specs = findRowNow["SPECS"].ToString();
            feeItemList.Item.Name = findRowNow["ITEM_NAME"].ToString();
            feeItemList.Name = feeItemList.Item.Name;
            feeItemList.Days = 1;
            feeItemList.Item.SysClass.ID = findRowNow["SYS_CLASS"].ToString();
            feeItemList.Order.Sample.Name = findRowNow["DEFAULT_SAMPLE"].ToString();//样本
            //是否需要预约
            feeItemList.Item.IsNeedBespeak = Neusoft.FrameWork.Function.NConvert.ToBoolean(findRowNow["NEEDBESPEAK"].ToString());
            if (r.DoctorInfo.Templet.Doct.ID == null || r.DoctorInfo.Templet.Doct.ID == string.Empty)
            {
                if (r.SeeDoct.ID != null && r.SeeDoct.ID != string.Empty)
                {
                    feeItemList.RecipeOper.ID = r.SeeDoct.ID;
                    feeItemList.RecipeOper.Name = r.SeeDoct.Name;
                }
                else
                {
                    feeItemList.RecipeOper.ID = r.DoctorInfo.Templet.Doct.ID;
                    feeItemList.RecipeOper.Name = r.DoctorInfo.Templet.Doct.Name;
                }
            }
            else
            {
                feeItemList.RecipeOper.ID = r.DoctorInfo.Templet.Doct.ID;
                feeItemList.RecipeOper.Name = r.DoctorInfo.Templet.Doct.Name;
            }
            //{33607355-C383-4271-B46C-0FBBAC251382} 开方医生所属科室编码
            feeItemList.RecipeOper.Dept.ID = r.DoctorInfo.Templet.Dept.ID;
            feeItemList.RecipeOper.Dept.Name = r.DoctorInfo.Templet.Dept.Name;
            feeItemList.DoctDeptInfo.ID = r.DoctorInfo.Templet.Doct.User01;
            feeItemList.FTSource = "0";//收费员自己收费
            feeItemList.ExecOper.Dept.ID = r.DoctorInfo.Templet.Dept.ID;
            feeItemList.ExecOper.Dept.Name = r.DoctorInfo.Templet.Dept.Name;
            //包装数量，非药品，组合项目为1
            decimal pactQty = Neusoft.FrameWork.Function.NConvert.ToDecimal(findRowNow["PACK_QTY"].ToString());
            feeItemList.Item.PackQty = pactQty;
            string minUnit = findRowNow["MIN_UNIT"].ToString();
            if (minUnit == string.Empty)
            {
                minUnit = "次";
            }
            feeItemList.Item.PriceUnit = minUnit;
            feeItemList.FeePack = "0";
            feeItemList.Item.Qty = 1M;//数量为1次
            //包装单位单价，保留4位小数
            decimal price = qty;
            feeItemList.Item.Price = price;
            feeItemList.OrgPrice = price;
            feeItemList.SpecialPrice = price;
            feeItemList.Item.MinFee.ID = findRowNow["FEE_CODE"].ToString();
            feeItemList.RecipeSequence = recipeSequence;
            feeItemList.Patient = r.Clone();
            feeItemList.OrgItemRate = r.Pact.Rate.PayRate;//
            if (this.IsShenPub(r, ref errorInfo))
            {
                feeItemList.NewItemRate = 0;//特殊比例
                feeItemList.ItemRateFlag = "3";
            }
            else
            {
                feeItemList.NewItemRate = r.Pact.Rate.PayRate;//特殊比例
                feeItemList.ItemRateFlag = "3";
            }
            feeItemList.FT.TotCost = Neusoft.FrameWork.Public.String.FormatNumber(feeItemList.Item.Price * feeItemList.Item.Qty / feeItemList.Item.PackQty, 2);
            feeItemList.FT.PayCost =0;
            feeItemList.FT.PubCost = feeItemList.FT.TotCost ;
            feeItemList.FT.OwnCost =0 ;
            feeItemList.FT.RebateCost = 0;
            feeItemList.NoBackQty = Math.Abs(feeItemList.Item.Qty);

            return feeItemList;
        }

        /// <summary>
        /// 获取患者需要收取的数量
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public decimal GetNumber(Neusoft.HISFC.Models.Registration.Register r, ref string errInfo)
        {
            if (r.Pact.PayKind.ID != "03")
            {
                return 0;
            }

            string sql = @"select sum(t.pub_cost+t.pay_cost+t.own_cost) from fin_opb_feedetail t  where t.clinic_code='{0}' and t.item_code='{1}' and ((t.cancel_flag='1' and t.pay_flag='1') or t.pay_flag='0')";

            sql = string.Format(sql, r.ID, roundFeeItemCode);

            Neusoft.FrameWork.Management.DataBaseManger dbMgr = new Neusoft.FrameWork.Management.DataBaseManger();
            string num = dbMgr.ExecSqlReturnOne(sql);
            if (num == null || num.Equals("-1"))
            {
                errInfo = "获取项目数量信息失败，原因：" + dbMgr.Err;
                return -1;
            }

            decimal paycost = Neusoft.FrameWork.Function.NConvert.ToDecimal(num);

            if (paycost >= 0)
            {
                //获取挂号级别的记账诊金
                Neusoft.HISFC.Models.Registration.RegLvlFee lvlFee = regLvlFeeMgr.Get(r.Pact.ID, r.DoctorInfo.Templet.RegLevel.ID);
                if (lvlFee == null)
                {
                    errInfo = "获取挂号费失败，原因：" + regLvlFeeMgr.Err;
                    return -1;
                }
                r.RegLvlFee.PubDigFee = lvlFee.PubDigFee;


                if (r.RegLvlFee.PubDigFee > paycost)
                {
                    return r.RegLvlFee.PubDigFee - paycost;
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }

        /// <summary>
        /// 是否省公医
        /// </summary>
        /// <param name="pactCode"></param>
        /// <returns></returns>
        public bool IsShenPub(Neusoft.HISFC.Models.Registration.Register r, ref string errInfo)
        {
            if (r.Pact.PayKind.ID != "03")
            {
                return false;
            }

            string sql = @"select 1 from com_dictionary cd where cd.type='BILLPACT' AND CD.CODE='{0}' and CD.NAME  like '省%'";
            sql = string.Format(sql, r.Pact.ID);

            Neusoft.FrameWork.Management.DataBaseManger dbMgr = new Neusoft.FrameWork.Management.DataBaseManger();
            string num = dbMgr.ExecSqlReturnOne(sql);
            if (num == null || num.Equals("-1"))
            {
                return false;
            }
            else
            {
                return Neusoft.FrameWork.Function.NConvert.ToBoolean(num);
            }

            return false;
        }


    }
}
