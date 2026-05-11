using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOrder
{
    public class Manager : Neusoft.FrameWork.Management.Database 
    {
        /// <summary>
        /// 根据合同单位代码获取字头
        /// </summary>
        /// <param name="PactID"></param>
        /// <returns></returns>
        public string GetPactHeadByPact(string PactCode)
        {
            string rtn = "";
            string strSql = "select pact_head from com_pactcompare where pact_code = '{0}'";
            try
            {
                strSql = string.Format(strSql, PactCode);
                rtn = this.ExecSqlReturnOne(strSql, "NF");
                if (rtn == "-1")
                {
                    return "NotFound";
                }
                else
                {
                    return rtn;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return "NotFound";
            }
        }

        public string GetSeq(string seq)
        {
            string strSql = "";
            strSql = " select  " + seq + ".nextval  from dual ";
            return this.ExecSqlReturnOne(strSql);
        }

        /// <summary>
        /// 插入一条
        /// </summary>
        /// <param name="info">摆药单分类实体</param>
        /// <returns></returns>
        public int InsertMZPubReport(PubReport info)
        {
            string strSql = "";

            strSql = @"INSERT INTO gfhz_main   ( id, static_month,lsh, bh, zzs,
            fy1,fy2,fy3, fy4,fy5,fy6,fy7, fy8,fy9, fy10,fy11,fy12,fy13,fy14,fy15,fy16,fy17,fy18, fy19,fy20,fy51,
            jzzje,sjzje,fee_type,pay_rate,is_valid,inpatient_no,
            begin_date,end_date,MCARD_NO,SP_TOT,IN_STATE,
                FEE_FLAG,REP_FLAG,EXT_FLAG,EXT_FLAG2,oper_code,oper_date,seq,sortid,name,work_name,work_code,patientNO,zfbl,
                OVERLIMITDRUGFEE,CANCERDRUGFEE,BEDFEE_JIANHU,BEDFEE_CENGLIU,COMPANYFAY,SELFPAY,TOTALFEE,INVOICE_NO)
                VALUES(  '{0}',to_date('{1}','YYYY-MM-DD HH24:MI:SS'),'{2}', '{3}', {4}, {5},{6},{7},{8},{9},    {10},   {11},   {12},    {13},
                {14}, {15},     {16},      {17},  {18},   {19},  {20},{21},    {22},    {23},   {24},    {25},
                {26},  {27},   '{28}',    {29},    '{30}',   '{31}',  
                to_date('{32}','YYYY-MM-DD HH24:MI:SS'),
                to_date('{33}','YYYY-MM-DD HH24:MI:SS'),
               '{34}',{35},'{36}',
                '{37}','{38}','{39}','{40}','{41}', to_date('{42}','YYYY-MM-DD HH24:MI:SS'),{43},'{44}','{45}','{46}','{47}','{48}',{49},
                {50},{51},{52},{53},{54},{55},{56},'{57}')";
            try
            {

                string[] strParm = myGetParmPubReport(info);  //取参数列表
                strSql = string.Format(strSql, strParm);       //
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        private string[] myGetParmPubReport(PubReport info)
        {

            string[] strParm ={   info.ID,
								 info.Static_Month.ToString(),
								 info.Bill_No,  
                                 info.Pact.ID,
								 info.Amount.ToString(),
								 info.Bed_Fee.ToString(),
								 info.YaoPin.ToString() , 
								 info.ChengYao.ToString() ,
								 info.HuaYan.ToString(),
								 info.JianCha.ToString(),
								 info.FangShe.ToString() , 
								 info.ZhiLiao.ToString() , 
								 info.ShouShu.ToString() , 
								 info.ShuXue.ToString() , 
								 info.ShuYang.ToString() ,
								 info.TeZhi.ToString() ,
								 info.TeZhiRate.ToString() , 
								 info.MR.ToString() , 
								 info.CT.ToString() , 
								 info.XueTou.ToString(), 
								 info.ZhenJin.ToString()  , 
								 info.CaoYao.ToString() , 
								 info.TeJian.ToString()  , 
								 info.ShenYao.ToString()  ,
								 info.JianHu.ToString() , 
								 info.ShengZhen.ToString()  , 
								 info.Tot_Cost.ToString()  ,
								 info.Pub_Cost.ToString(),
								 info.Fee_Type  , 
								 info.Pay_Rate.ToString()  , 
								 info.IsValid,
								 info.Pact.Name, 
								 info.Begin.ToString(),
								 info.End.ToString(),
								 info.MCardNo,
                                 info.SpDrugFeeTot.ToString(),
								 info.IsInHos,
				                 info.FEE_FLAG,
				                 info.REP_FLAG,
				                 info.EXT_FLAG,
				                 info.EXT_FLAG2,
                                 info.OperCode,
                                 info.OperDate.ToString(),
                                 info.Seq.ToString(),
                                 info.SortId,
                                 info.Name,
                                 info.WorkName,
                                 info.WorkCode,
                                 info.PatientNO,
                                 info.TeYaoRate.ToString(),           
                                 info.OverLimitDrugFee.ToString(),
                                 info.CancerDrugFee.ToString(),
                                 info.BedFee_JianHu.ToString(),
                                 info.BedFee_CengLiu.ToString(),
                                 info.CompanyPay.ToString(),
                                 info.SelfPay.ToString(),
                                 info.TotalFee.ToString(),
                                 info.InvoiceNo.ToString()
							 };

            return strParm;
        }

        /// <summary>
        /// 删除托收单(根据住院流水号，发票号)
        /// </summary>
        /// <param name="inpatientNo"></param>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        public int DeleteMZPubReport(string invoiceNo, string invoiceNoSeq)
        {
            string strSql = "";
            //if (this.Sql.GetSql("OutpatientFee.GFReport.DeletePubReport", ref strSql) == -1)
            //{
            //    this.Err = this.Sql.Err;
            //    return -1;
            //}

            strSql = @"delete from gfhz_main f where f.invoice_no ='{0}' and f.seq ={1}";

            try
            {
                strSql = string.Format(strSql, invoiceNo, invoiceNoSeq);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }
    }
}
