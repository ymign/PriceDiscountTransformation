using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data;

namespace His.Business.WeiXin
{
    public class InpatientFee
    {
        Manager mgr = new Manager();

        /// <summary>
        /// 功能：缴纳住院预交金
        /// </summary>
        /// <param name="inpatientFeePrepayRequest"></param>
        /// <returns></returns>
        public string InpatientFeePrepay(His.Models.ZZSB.InpatientPrePayReq reqInfo)
        {
            try
            {
                #region 判断

                if (string.IsNullOrEmpty(reqInfo.InpatientNo))
                    return Function.DataSource("0", "住院流水号不能为空！", reqInfo.FunCode).ToString();

                if (reqInfo.TotalFee <= 0)
                    return Function.DataSource("0", "押金金额不能为零！", reqInfo.FunCode).ToString();

                #endregion

                #region 获取住院状态

                string InState = string.Empty;
                decimal freecost = 0;
                string PatientName = string.Empty;

                string getInstateSql = string.Format(@"
                select decode(t.in_state,'I','0','R','0','1') in_state,
                t.free_cost,t.name
                from fin_ipr_inmaininfo t 
                where t.inpatient_no='{0}'",
                reqInfo.InpatientNo);

                InState = mgr.ExecSqlReturnOne(getInstateSql);

                DataSet ds = new DataSet();
                if (mgr.ExecQuery(getInstateSql, ref ds) == -1)
                    return Function.DataSource("0", "查找住院状态出错！", reqInfo.FunCode).ToString();
                DataTable dt = ds.Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        InState = dt.Rows[i][0].ToString();//0-在院  1-出院
                        freecost = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][1].ToString());
                        PatientName = dt.Rows[i][2].ToString();
                    }
                }

                if (InState.Equals("-1"))
                    return Function.DataSource("0", "查找住院状态出错！", reqInfo.FunCode).ToString();
                if (InState.Equals("1"))
                    return Function.DataSource("0", "患者已出院！", reqInfo.FunCode).ToString();

                #endregion

                #region 获取操作员

                string paytype = string.Empty;
                string recept_no = string.Empty;
                string oper_code = string.Empty;
                if (reqInfo.PaymentWay.Equals(0))
                {
                    paytype = "CCB";
                }
                else if (reqInfo.PaymentWay.Equals(1))
                {
                    paytype = "MCZH";
                }
                else if (reqInfo.PaymentWay.Equals(2))
                {
                    paytype = "ZFB";
                }
                else if (reqInfo.PaymentWay.Equals(3))
                {
                    paytype = "WX";
                }
                else if (reqInfo.PaymentWay.Equals(5))
                {
                    paytype = "YBXYF";
                }
                else if (reqInfo.PaymentWay.Equals(6))
                {
                    paytype = "JHRMB";
                }
                else
                {
                    paytype = "COMM";
                }

                recept_no = GetInvoiceNo();
                if (string.IsNullOrEmpty(recept_no) || recept_no == "-1")
                    return Function.DataSource("0", "生成发票序列出错！", reqInfo.FunCode).ToString();
                #endregion

                //开始事务
                Shadow.Util.Data.Management.Trans.BeginTransaction();

                #region insertPrepay

                string insertPrepay = string.Format(@"
                insert into fin_ipb_inprepay
                (inpatient_no,
                happen_no,
                name,
                prepay_cost,
                pay_way,
                dept_code,
                receipt_no,
                stat_date,
                balance_date,
                balance_state,
                prepay_state,
                old_recipeno,
                open_bank,
                open_accounts,
                invoice_no,
                balance_no,
                balance_opercode,
                report_flag,
                check_no,
                fingrp_code,
                work_name,
                trans_flag,
                change_balance_no,
                trans_code,
                trans_date,
                print_flag,
                ext_flag,
                ext1_flag,
                postrans_no,
                oper_code,
                oper_date,
                oper_deptcode,
                mark,
                daybalance_flag,
                daybalance_no,
                daybalance_opcd,
                daybalance_date
                )
                select 
                t.inpatient_no,
                nvl((select max(p.happen_no)+1 from fin_ipb_inprepay p where p.inpatient_no=t.inpatient_no),1)happen_no,
                t.name,
                '{1}' prepay_cost,
                '{2}' pay_way,
                t.dept_code,
                '{3}' receipt_no,
                null stat_date,
                to_date('0001-01-01','yyyy-mm-dd') balance_date,
                '0' balance_state,
                '0' prepay_state,
                null old_recipeno,
                null open_bank,
                null open_accounts,
                null invoice_no,
                '0' balance_no,
                null balance_opercode,
                '0' repore_flag,
                null check_no,
                null fingrp_code,
                null work_name,
                '0' trans_flag,
                null change_balance_no,
                null trans_code,
                to_date('0001-01-01','yyyy-mm-dd') trans_date,
                '0' print_flag,
                '1' ext_flag,
                '0' ext1_flag,
                null postrans_no,
                '{4}' oper_code,
                sysdate oper_date,
                null oper_deptcode,
                'NH' mark,
                '0' daybalance_flag,
                null daybalance_no,
                null daybalance_opcd,
                null daybalance_date
                from fin_ipr_inmaininfo t where t.inpatient_no='{0}'",
                        reqInfo.InpatientNo,
                        reqInfo.TotalFee,
                        paytype,
                        recept_no,
                        Function.OPERID);

                if (mgr.ExecNoQuery(insertPrepay) == -1)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    return Function.DataSource("0", "插入预交金失败！" + mgr.Err, reqInfo.FunCode).ToString();
                }
                #endregion

                #region updatePrepay

                string updatePrepay = string.Format(@"UPDATE fin_ipr_inmaininfo  
                    SET prepay_cost = nvl(prepay_cost,0) + {1}, 
                    free_cost = nvl(free_cost,0) + {1}  
                    WHERE inpatient_no = '{0}'",
                reqInfo.InpatientNo,
                reqInfo.TotalFee);

                if (mgr.ExecNoQuery(updatePrepay) == -1)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    return Function.DataSource("0", "更新患者主表失败！" + mgr.Err, reqInfo.FunCode).ToString();
                }

                #endregion

                //提交事务
                Shadow.Util.Data.Management.Trans.Commit();

                #region return

                string msg = "尊敬的：" + PatientName + "\n" +
                "您已成功补交住院押金 " + reqInfo.TotalFee +
                " 元，请前往住院收费处打印住院押金收据，谢谢！";

                XElement result = new XElement("Result",
                     new XElement("Balance", freecost + reqInfo.TotalFee),
                     new XElement("ReceptNo", recept_no),
                     new XElement("Note"));
                XElement root = Function.DataSource("1", msg, reqInfo.FunCode);
                root.Element("return").Add(result);
                return root.ToString();

                #endregion

            }
            catch (Exception ex)
            {
                Shadow.Util.Data.Management.Trans.RollBack();
                return Function.DataSource("0", ex.Message, reqInfo.FunCode).ToString();
            }

        }

        /// <summary>
        /// 微信预交金的发票号/凭证号
        /// </summary>
        /// <returns></returns>
        private string GetInvoiceNo()
        {
            string inv_sql = "select 'WX'||lpad(SEQ_WeiXin_PREPAY_W.Nextval,8,'0') from dual ";
            return mgr.ExecSqlReturnOne(inv_sql);
        }

    }
}
