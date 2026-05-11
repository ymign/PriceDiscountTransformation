using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Linq;
using His.Models.ZZSB;

namespace His.Business.ZZSB
{
    public class Inpatient
    {

        public Inpatient()
        {
            if (mgr == null)
            {
                mgr = new RegisterManager();
            }
        }

        RegisterManager mgr = null;
        public string err = string.Empty;

        /*
          /// <summary>
          /// 功能：查询患者的住院流水号，返回住院流水的信息集合
          /// </summary>
          /// <param name="bindInpatientNoRequest"></param>
          /// <returns></returns>
          public Contract.Response.GetInpatientNoResponse GetInPatientSeriNos(Contract.Request.GetInpatientSeriNoRequest bindInpatientNoRequest)
          {
              Contract.Response.GetInpatientNoResponse response = new GetInpatientNoResponse();
              response.InpatientInfos = new List<Contract.MasterData.InpatientInfo>();

              try
              {
                  string sql = string.Format(@" 
                  select t.free_cost,
                  to_char(t.out_date,'yyyyMMddHH24miss') out_date,
                  to_char(t.in_date,'yyyyMMddHH24miss') in_date,
                  t.inpatient_no,
                  decode(t.in_state,'R',0,'I',0,1) in_state,
                  t.prepay_cost 
                  from fin_ipr_inmaininfo t  
                  where t.name='{0}' 
                  and t.patient_no=lpad('{1}',10,'0') 
                  order by t.in_date desc ",
                      bindInpatientNoRequest.PatientInfo.Name,
                      bindInpatientNoRequest.InpatientNo);
                  DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                  if (dt != null && dt.Rows.Count > 0)
                  {
                      for (int i = 0; i < dt.Rows.Count; i++)
                      {
                          Contract.MasterData.InpatientInfo Inpatient = new Contract.MasterData.InpatientInfo();
                          Inpatient.Balance = Convert.ToDouble(dt.Rows[i][0].ToString());
                          Inpatient.DischargeDate = dt.Rows[i][1].ToString();
                          Inpatient.InpatientDate = dt.Rows[i][2].ToString();
                          Inpatient.InpatientSeriNo = dt.Rows[i][3].ToString();
                          Inpatient.InpatientStatus = Convert.ToInt32(dt.Rows[i][4].ToString());
                          Inpatient.PrepayCost = Convert.ToDouble(dt.Rows[i][5].ToString());
                          Inpatient.Summary = "";
                          response.InpatientInfos.Add(Inpatient);
                      }
                  }
              }
              catch
              {

              }

              return response;
          }

      
          /// <summary>
          /// 功能：获取住院信息
          /// </summary>
          /// <param name="inpatientDetailsRequest"></param>
          /// <returns></returns>
          public Contract.Response.InpatientDetailResponse GetInpatientDetail(Contract.Request.InpatientDetailRequest inpatientDetailsRequest)
          {
              Contract.Response.InpatientDetailResponse result = new InpatientDetailResponse();

              result.InpatientDetails = new List<Contract.MasterData.InpatientDetail>();

              try
              {
                  foreach (string inpatientno in inpatientDetailsRequest.InpatientSeriNos)
                  {
                      string sql = string.Format(@"
                      select 
                         case when t.in_state in('O') then 0 else t.free_cost end balance ,
                         t.dept_code DeptId,
                         t.dept_name DeptName,
                         case when t.in_state in ('I','R') then '00010101' else  to_char(t.out_date,'yyyyMMdd') end DischargeDate,
                         t.house_doc_code DrId,
                         t.house_doc_name DrName,
                         to_char(t.in_date,'yyyyMMddHH24miss') InpatientDate,
                         t.patient_no InpatientNo,
                         t.inpatient_no InpatientSeriNo,
                         decode(t.in_state,'I',0,'R',0,1) InpatientStatus,
                         '' InvoiceNo,
                         t.mcard_no McardNo,
                         t.nurse_cell_code NurId,
                         t.nurse_cell_name NurName,
                         0 OthCost,
                         t.card_no OutpatientId,
                         case when t.in_state in ('O') then nvl((select sum(s.own_cost) from FIN_IPR_SIINMAININFO s where s.inpatient_no=t.inpatient_no and s.valid_flag='1'),t.balance_cost) else t.own_cost end  OwnCost,
                         t.name PatientName,
                         case when t.in_state in ('O') then nvl((select sum(s.own_cost) from FIN_IPR_SIINMAININFO s where s.inpatient_no=t.inpatient_no and s.valid_flag='1'),t.balance_cost) else t.pay_cost end   PayCost,
                         case when t.in_state='O' then t.balance_prepay else t.prepay_cost+t.balance_prepay end PrepayCost,
                         case when t.in_state='O' then nvl((select sum(s.pub_cost) from FIN_IPR_SIINMAININFO s where s.inpatient_no=t.inpatient_no and s.valid_flag='1'),t.pub_cost)  else t.pub_cost end PubCost,
                         '' Summary,
                         case when t.in_state='O' then nvl((select sum(s.tot_cost) from FIN_IPR_SIINMAININFO s where s.inpatient_no=t.inpatient_no and s.valid_flag='1'),t.balance_cost)  else t.tot_cost+t.balance_cost end TotalCost,
                         (SELECT bed.SORT_ID FROM COM_BEDINFO bed WHERE bed.BED_NO =  t.bed_no and bed.valid_state='1' and rownum=1) BedNo,
                         t.diag_name InDiagnose,
                         (select m.diag_name from met_cas_diagnose m where m.inpatient_no=t.inpatient_no and m.diag_kind ='14' and rownum=1) OutDiagnose,
                         t.nurse_cell_name
                         from fin_ipr_inmaininfo t where t.inpatient_no='{0}' ",
                      inpatientno);
                      DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                      if (dt != null && dt.Rows.Count > 0)
                      {
                          for (int i = 0; i < dt.Rows.Count; i++)
                          {
                              Contract.MasterData.InpatientDetail detail = new Contract.MasterData.InpatientDetail();
                              detail.Balance = Convert.ToDouble(dt.Rows[i][0].ToString());//余额
                              detail.DeptId = dt.Rows[i][1].ToString();//住院科室编码
                              detail.DeptName = dt.Rows[i][2].ToString();//住院科室名
                              detail.DischargeDate = dt.Rows[i][3].ToString();//出院日期
                              detail.DrId = dt.Rows[i][4].ToString();//主治医生编码
                              detail.DrName = dt.Rows[i][5].ToString();//主治医生名
                              detail.InpatientDate = dt.Rows[i][6].ToString();//入院日期
                              detail.InpatientNo = dt.Rows[i][7].ToString();//住院号
                              detail.InpatientSeriNo = dt.Rows[i][8].ToString();//住院流水号
                              detail.InpatientStatus = Convert.ToInt32(dt.Rows[i][9].ToString());//住院状态：0：未出院 1：已出院
                              detail.InvoiceNo = "111";//发票号
                              detail.McardNo = dt.Rows[i][11].ToString();//医疗证号
                              detail.NurId = dt.Rows[i][12].ToString();//住院护理站编码
                              detail.NurName = dt.Rows[i][13].ToString();//住院护理站名
                              detail.OthCost = Convert.ToDouble(dt.Rows[i][14].ToString());//其他报销金额
                              detail.OutpatientId = dt.Rows[i][15].ToString();//门诊号
                              detail.OwnCost = Convert.ToDouble(dt.Rows[i][16].ToString());//自费金额
                              detail.PatientName = dt.Rows[i][17].ToString();//姓名
                              detail.PayCost = Convert.ToDouble(dt.Rows[i][18].ToString());//账户消费金额
                              detail.PrepayCost = Convert.ToDouble(dt.Rows[i][19].ToString());//预交金总额
                              detail.PubCost = Convert.ToDouble(dt.Rows[i][20].ToString());//统筹报销金额
                              detail.Summary = GetFcontent(detail.InpatientSeriNo);//出院小结
                              detail.Summary = detail.Summary.Replace(" ", "").Replace("\r\n", "") + "\r\n住院小结！";
                              detail.TotalCost = Convert.ToDouble(dt.Rows[i][22].ToString());//费用总额

                              detail.BedNo = dt.Rows[i][23].ToString();
                              detail.InDiagnose = dt.Rows[i][24].ToString();
                              detail.OutDiagnose = dt.Rows[i][25].ToString();
                              detail.RoomNo = dt.Rows[i][26].ToString();
                              if (detail.RoomNo.Length > 6)
                              {
                                  detail.RoomNo = detail.RoomNo.Substring(0, 6) + "...";
                              }
                              result.InpatientDetails.Add(detail);
                              //余额=预交金-自费       
                              //费用总额=自费金额+账户消费金额+统筹报销金额+其他报销金额
                          }
                      }
                  }
                  result.Code = 0;
                  result.Message = "";
              }
              catch (Exception ex)
              {
                  result.Code = -1;
                  result.Message = ex.ToString();
              }
              return result;
          }
         * 
         * 
         * 
         * */

        public int QueryInpatientInfo(His.Models.ZZSB.InPatientReq req, string str, ref string xml)
        {
            try
            {
                #region sql

                string sql = @"select t.inpatient_no as inpatientNo,
       t.card_no as cardNo,
       t.patient_no as PatientNo,
       t.mcard_no as McardNo,
       t.name as PatientName,
       t.birthday,
       t.sex_code as sexCode,
       nvl(t.home_tel, t.work_tel) as tel,
       t.linkman_name as contectName,
       t.linkman_tel as contectTel,
       t.home as homeAddr,
       t.pact_code as pactCode,
       t.pact_name as PactName,
       t.dept_code as DeptCode,
       t.dept_name as DeptName,
       t.nurse_cell_code as NurseCellCode,
       t.nurse_cell_name as NurseCellName,
       t.house_doc_code as DoctCode,
       t.house_doc_name as DoctName,
       t.in_date as InDate,    
       t.out_date as OutDate,
       case
         when t.in_state = 'O'  then 
           t.balance_cost
         else
          t.tot_cost + t.balance_cost
       end TotalCost,
       case
         when t.in_state = 'O' then
          t.balance_prepay
         else
          t.prepay_cost + t.balance_prepay
       end PrepayCost,
       case
         when t.in_state in ('O') and t.paykind_code='02' then
         
              t.balance_cost-(select p.pub_cost from fin_ipr_siinmaininfo p where p.inpatient_no=t.inpatient_no and p.balance_state='1' and
  p.balance_no =(select max(r.balance_no) from fin_ipr_siinmaininfo r where r.inpatient_no=t.inpatient_no))
         else
        t.own_cost
       end OwnCost,
       case
         when t.in_state in ('O') then         
             0.00
         else
          t.pay_cost
       end PayCost,
       case
         when t.in_state = 'O' and t.paykind_code='02'  then
 (select p.pub_cost from fin_ipr_siinmaininfo p where p.inpatient_no=t.inpatient_no and p.balance_state='1' and
  p.balance_no =(select max(r.balance_no) from fin_ipr_siinmaininfo r where r.inpatient_no=t.inpatient_no))
         else
          t.pub_cost
       end PubCost,
       0 OthCost,
       case
         when t.in_state in ('O') then
          0
         else
          t.free_cost
       end balance,
       '' as Summary,
       decode(t.in_state, 'I', 0, 'R', 0,'B',0, 1) as InState,
         (select wm_concat( distinct x.invoice_no) from fin_ipb_feeinfo x where x.inpatient_no=t.inpatient_no) as InvoiceNo,
       t.diag_name InDiagnose,
       (select m.diag_name
          from met_cas_diagnose m
         where m.inpatient_no = t.inpatient_no
           and m.diag_kind = '14'
           and rownum = 1) as OutDiagnose,
       '' RoomNo,
       (SELECT bed.SORT_ID
          FROM COM_BEDINFO bed
         WHERE bed.BED_NO = t.bed_no
           and bed.valid_state = '1'
           and rownum = 1) as BedNo,
       t.memo as mark,
       '' as extend1,
       '' as extend2,
       '' as extend3
  from fin_ipr_inmaininfo t ";

                #endregion

                xml = string.Empty;
                if (string.IsNullOrEmpty(str))
                {
                    return -1;
                }
                sql += str;
                /*<DataSource>
     <return>
	     <Code>1</Code><!--成功：1 失败：0 -->
	     <ErrorMsg></ErrorMsg><!-- 错误说明 -->
		 <OpTime></OpTime><!--响应时间 -->
		 <FunCode></FunCode><!--业务编号 -->
	     <Result> <!--具体的返回值 -->
			...
	     </Result>
     </return>
</DataSource>*/
                XElement rt = new XElement("Result");
                DataSet ds = new DataSet();
                ds = DataBaseHelp.DataExecHelp.GetDataSet(sql);
                if (ds.Tables[0].Rows.Count < 0 && ds != null)
                {
                    err = "没有相关数据";
                    return -1;
                }
                //if (mgr.ExecQuery(sql, ref ds) == -1)
                //{
                //    err = mgr.Err;
                //    return -1;
                //}
                else
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            XElement d = new XElement("PatientInfo");
                            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                                d.Add(new XElement(ds.Tables[0].Columns[i].ColumnName, row[i].ToString()));
                            rt.Add(d);
                            // break;
                        }

                    }
                    XElement source = Function.DataSource("1", string.Empty, req.FunCode);
                    source.Element("return").Add(rt);
                    xml = source.ToString();
                    return 1;
                }

            }
            catch (Exception ex)
            {
                err = ex.Message;
                xml = string.Empty;
                return -1;
            }

        }





        public string GetInfoByInpNo(His.Models.ZZSB.InPatientReq req)
        {

            string where = " where patient_no ='{0}' order by in_date desc  ";
            string xml = string.Empty;
            if (string.IsNullOrEmpty(req.PatientID))
            {
                return Function.DataSource("0", "住院号不能为空", req.FunCode).ToString();
            }
            where = string.Format(where, req.PatientID);
            if (this.QueryInpatientInfo(req, where, ref xml) == -1)
                return Function.DataSource("0", this.err, req.FunCode).ToString();
            else
                return xml;



        }

        public string GetElecInvoiceUrlListByInpatientNo(string req)
        {

            string sql = @"select distinct z.Pictureurl
from fin_ipb_balancehead b,Elec_OutPatientRecord z 
where b.inpatient_no = '{0}' and z.clinic_code = b.invoice_no||'{0}'";
            string xml = string.Empty;
            if (string.IsNullOrEmpty(req))
            {
                return Function.DataSource("0", "住院号不能为空", req).ToString();
            }
            sql = string.Format(sql, req);
            DataSet ds = new DataSet();
            ds = DataBaseHelp.DataExecHelp.GetDataSet(sql);
            if (ds == null)
                return Function.DataSource("0", "没有相关数据", req).ToString();
            XElement rt = new XElement("Result");
            if (ds.Tables[0].Rows.Count < 0 && ds != null)
            {
                return Function.DataSource("0", "没有相关数据", req).ToString();
            }
            else
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        XElement d = new XElement("URL");
                        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                            d.Add(new XElement(ds.Tables[0].Columns[i].ColumnName, row[i].ToString()));
                        rt.Add(d);
                    }

                }
                XElement source = Function.DataSource("1", string.Empty, "");
                source.Element("return").Add(rt);
                xml = source.ToString();
                return xml;
            }



        }


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

                //string payflag = GetPayFlag(inpatientFeePrepayRequest.TradeSerialNumber, "3", inpatientFeePrepayRequest.InpatientSeriNo);
                //if (payflag.Equals("1"))
                //{
                //    result.Code = 0;
                //    result.Message = "";
                //    return result;
                //}

                if (string.IsNullOrEmpty(reqInfo.InpatientNo))
                    return Function.DataSource("0", "住院流水号不能为空！", reqInfo.FunCode).ToString();

                if (reqInfo.TotalFee <= 0)
                    return Function.DataSource("0", "押金金额不能为零！", reqInfo.FunCode).ToString();

                #endregion

                #region 获取住院状态

                string InState = string.Empty;
                decimal freecost = 0;
                string PatientName = string.Empty;
                string card_no = string.Empty;

                string getInstateSql = string.Format(@"
                select decode(t.in_state,'I','0','R','0','B','0','1') in_state,
                t.free_cost,t.name,t.card_no
                from fin_ipr_inmaininfo t 
                where t.inpatient_no='{0}'",
                reqInfo.InpatientNo);

                InState = mgr.ExecSqlReturnOne(getInstateSql);
                DataSet ds = new DataSet();
                ds = DataBaseHelp.DataExecHelp.GetDataSet(getInstateSql);
                if (ds.Tables[0].Rows.Count < 0 && ds != null)
                {
                    return Function.DataSource("0", "查找住院状态出错！", reqInfo.FunCode).ToString();
                }
                //if (mgr.ExecQuery(getInstateSql, ref ds) == -1)
                //    return Function.DataSource("0", "查找住院状态出错！", reqInfo.FunCode).ToString();
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        InState = dt.Rows[i][0].ToString();//0-在院  1-出院
                        freecost = Shadow.Util.Data.Func.NConvert.ToDecimal(dt.Rows[i][1].ToString());
                        PatientName = dt.Rows[i][2].ToString();
                        card_no = dt.Rows[i][3].ToString();
                    }
                }
                if (InState.Equals("-1"))
                    return Function.DataSource("0", "查找住院状态出错！", reqInfo.FunCode).ToString();
                if (InState.Equals("1"))
                {
                    return Function.DataSource("0", "患者已出院！", reqInfo.FunCode).ToString();
                }

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

                #region 插入交易记录表
                string InsertTradeRecords = Sql.Sql.InsertTradeRecords;

                #region 获取交易记录信息
                Models.ZZSB.TradeRecords recordsInfo = new His.Models.ZZSB.TradeRecords();
                recordsInfo.TranserNo = reqInfo.ReqTraceNo;//交易流水号
                recordsInfo.INVOICE_NO = recept_no;//发票号
                recordsInfo.CLINIC_NO = reqInfo.InpatientNo;//
                recordsInfo.CARDNO = "";//卡号
                recordsInfo.NAME = PatientName;//姓名
                recordsInfo.ORDERID = reqInfo.BankCardNo;//订单号或者银行卡卡号
                recordsInfo.PAY_TYPE = paytype;//支付方式
                recordsInfo.TYPE = "5";//交易类型
                recordsInfo.TOT_COST = reqInfo.TotalFee.ToString("0.00");//交易金额
                recordsInfo.DEVICEID = reqInfo.DeviceID;//设备编号
                recordsInfo.REMARK = (freecost + reqInfo.TotalFee).ToString("0.00");//备注,挂号插入的是看诊序号
                recordsInfo.PACTCODE = "";//合同单位
                #endregion

                string[] tradeRecordsArgm = Function.GetTradeRecordsInfo(recordsInfo);
                InsertTradeRecords = string.Format(InsertTradeRecords, tradeRecordsArgm);

                if (mgr.ExecNoQuery(InsertTradeRecords) < 0)
                {
                    Shadow.Util.Data.Management.Trans.RollBack();
                    return Function.DataSource("0", "插入交易记录表失败！" + mgr.Err, reqInfo.FunCode).ToString();
                }
                #endregion

                //插入支付平台交易记录
                if (!string.IsNullOrEmpty(reqInfo.ApplicationOrderNo) || !string.IsNullOrEmpty(reqInfo.PlatformOrderNo))
                {
                    FinTransRecord payRecordInfo = new FinTransRecord();
                    payRecordInfo.Id = Guid.NewGuid().ToString();
                    payRecordInfo.TransactionNo = recept_no;
                    payRecordInfo.TransType = "1";
                    payRecordInfo.ClientCode = "ZDWY_ZZSB";
                    payRecordInfo.PlatformOrderNo = reqInfo.PlatformOrderNo;
                    payRecordInfo.ApplicationOrderNo = reqInfo.ApplicationOrderNo;
                    string PayChannelCode = "";
                    if (paytype == "WX")
                    {
                        PayChannelCode = "WeChat_FKM";
                    }
                    else if (paytype == "ZFB")
                    {
                        PayChannelCode = "ZFB_FKM";
                    }
                    else
                    {

                        Shadow.Util.Data.Management.Trans.RollBack();
                        return Function.DataSource("0", "插入支付交易记录失败:支付方式不符合要求" + paytype + "", reqInfo.FunCode).ToString();
                    }
                    payRecordInfo.PayChannelCode = PayChannelCode;
                    payRecordInfo.TransAmount = reqInfo.TotalFee;
                    payRecordInfo.OrderBigType ="1";
                    payRecordInfo.OrderSmallType = "01";
                    payRecordInfo.PatientNo = card_no;
                    payRecordInfo.PatientName = PatientName;
                    payRecordInfo.CreatedCode = "00W999";
                    payRecordInfo.CreatedName = "自助机";
                    payRecordInfo.HospitalCode = "H44040200001";
                    payRecordInfo.BusinessNo = reqInfo.InpatientNo;
                    string strSql = @"
insert into FIN_Trans_RECORD(
                            id,
                            trans_type,
                            platform_order_no,
                            client_code,
                            application_order_no,
                            pay_channel_code,
                            pay_trans_finish_time,
                            TRANS_AMOUNT,
                            order_big_type,
                            order_small_type,
                            patient_no,
                            patient_name,
                            hospital_code,
                            created_code,
                            created_name,
                            transactionno,
                            businessno
                           ) 
values(
       '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', to_date('{6}','YYYY-MM-DD hh24:mi:ss'), 
       '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', 
       '{14}', '{15}','{16}'
      )";

                    string formattedSql = string.Format(strSql,
                        payRecordInfo.Id,
                        payRecordInfo.TransType,
                        payRecordInfo.PlatformOrderNo,
                        payRecordInfo.ClientCode,
                        payRecordInfo.ApplicationOrderNo,
                        payRecordInfo.PayChannelCode,
                        payRecordInfo.PayTransFinishTime,
                        payRecordInfo.TransAmount,
                        payRecordInfo.OrderBigType,
                        payRecordInfo.OrderSmallType,
                        payRecordInfo.PatientNo,
                        payRecordInfo.PatientName,
                        payRecordInfo.HospitalCode,
                        payRecordInfo.CreatedCode,
                        payRecordInfo.CreatedName,
                        payRecordInfo.TransactionNo,
                        payRecordInfo.BusinessNo
                        );
                    if (mgr.ExecNoQuery(formattedSql) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        return Function.DataSource("0", "插入支付交易记录失败！" + mgr.Err, reqInfo.FunCode).ToString();
                    }
                }

                #region insertAppPaymode
                /*
                string PatientNo = "";
                string PatientName = "";
                string PateintSex = "";
                string Phone = "";
                string Idenno = "";
                GetPatientNoByInpatientNo(inpatientFeePrepayRequest.InpatientSeriNo, out PatientNo, out PatientName, out PateintSex, out Phone, out Idenno);

                string insertAppPaymode = string.Format(@"
                insert into fin_app_paymode
                (
                serino,
                patientid,
                patienttype,
                termialtype,
                paytype,
                paymode,
                totalfee,
                settledate,
                orderid,
                tradeserialnumber,
                invoiceno,
                hiscost,
                recipeno,
                transtype
                )
                values
                (
                '{0}',--serino,
                '{1}',--patientid,
                '{2}',--patienttype,
                '{3}',--terialtype,
                '{4}',--paytype,
                '{5}',--paymode,
                '{6}',--totlfee,
                to_date('{7}','yyyy-mm-dd hh24:mi:ss'),--settledate,
                '{8}',--orderid,
                '{9}',--tradeserialnumber,
                '{10}',--invoiceno,
                '{11}',--hiscost,
                '{12}',--recipeno
                '{13}' --transtype
                )",
                inpatientFeePrepayRequest.InpatientSeriNo,
                PatientNo,
                "2",
                inpatientFeePrepayRequest.TermialType,
                "3",
                inpatientFeePrepayRequest.PaymentWay,
                inpatientFeePrepayRequest.TotalFee,
                inpatientFeePrepayRequest.SettleDate,
                "",
                inpatientFeePrepayRequest.TradeSerialNumber,
                recept_no,
                "",
                "",
                "1");

                sqlList.Add(insertAppPaymode);
               * */
                #endregion

                #region old


                /*   if (DataBaseHelp.DataExecHelp.ExecArrayList(sqlList, ref errtext))
                {
                    try
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(s =>
                        {
                            string content = "尊敬的：" + PatientName + "\n" +
                            "您已成功补交住院押金 " + inpatientFeePrepayRequest.TotalFee + " 元，请前往住院收费处打印住院押金收据，谢谢！";
                            this.SendTextMessage(inpatientFeePrepayRequest.InpatientSeriNo, content, "I");
                        });
                    }
                    catch { }
                    result.Code = 0;
                    result.Balance = freecost + inpatientFeePrepayRequest.TotalFee;
                    return result;
                }
                else
                {
                    result.Code = 1;
                    result.Message = errtext;
                    return result;
                }*/

                #endregion
                #region 支付平台发票绑定
                if (paytype == "YBXYF")
                {
                    ZFPTService zfptSer = new ZFPTService();
                    His.Models.ZZSB.PayPlatform.InvoiceBinding invoiceBinding = new His.Models.ZZSB.PayPlatform.InvoiceBinding();
                    string Msg = "";
                    invoiceBinding.invoiceNo = recordsInfo.INVOICE_NO;//发票号
                    invoiceBinding.payorderId = recordsInfo.ORDERID;
                    invoiceBinding.payMode = "1";
                    invoiceBinding.orderType = "3";
                    if (!zfptSer.ZFPTInvoiceBinding(invoiceBinding, ref Msg))
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        return Function.DataSource("0", "绑定支付平台订单失败！" + Msg, reqInfo.FunCode).ToString();
                    }
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

        /*

                /// <summary>
                /// 功能：查询缴纳住院预交金历史记录
                /// </summary>
                /// <param name="feePrepayRecordRequest"></param>
                /// <returns></returns>
                public FeePrepayRecordResponse GetFeePrepayRecord(Contract.Request.FeePrepayRecordRequest feePrepayRecordRequest)
                {
                    FeePrepayRecordResponse result = new FeePrepayRecordResponse();
                    result.InpatientFeePrepayInfos = new List<Contract.MasterData.InpatientFeePrepayInfo>();
                    Contract.MasterData.InpatientFeePrepayInfo prepay = new Contract.MasterData.InpatientFeePrepayInfo();

                    try
                    {
                        string sql = string.Format(@"select t.totalfee,
                        t.patientid,
                        t.serino,
                        to_char(t.settledate,'yyyyMMddHH24miss'),
                        t.paymode,
                        t.tradeserialnumber
                         from fin_app_paymode t where t.paytype='3'
                         and t.serino='{0}'", feePrepayRecordRequest.InpatientSeriNo);
                        DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                prepay.ExtItems = new List<Contract.Base.ExtItem>();
                                prepay.Fee = Convert.ToDouble(dt.Rows[i][0].ToString());
                                prepay.InpatientNo = dt.Rows[i][1].ToString();
                                prepay.InpatientSeriNo = dt.Rows[i][2].ToString();
                                prepay.PayDate = dt.Rows[i][3].ToString();
                                prepay.PaymentWay = Convert.ToInt32(dt.Rows[i][4].ToString());
                                prepay.TradeSerialNumber = dt.Rows[i][5].ToString();

                                result.InpatientFeePrepayInfos.Add(prepay);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Code = -1;
                        result.Message = ex.ToString();
                    }

                    return result;
                }
              */

        //        /// <summary>
        //        /// 获取订单支付状态
        //        /// </summary>
        //        /// <param name="TradeNo"></param>
        //        /// <param name="paytype"></param>
        //        /// <param name="serino"></param>
        //        /// <returns></returns>
        //        private string GetPayFlag(string TradeNo, string paytype, string serino)
        //        {
        //            string flag = "0";
        //            try
        //            {

        //                string sql = string.Format(@"select count(*) v_count from fin_app_paymode t where t.tradeserialnumber='{0}' 
        //                and t.serino='{1}'
        //                and invoiceno is not null",
        //                TradeNo, serino);

        //                DataTable dt = DataBaseHelp.DataExecHelp.GetDataTable(sql);
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    for (int i = 0; i < dt.Rows.Count; i++)
        //                    {
        //                        int v_count = Convert.ToInt32(dt.Rows[i][0].ToString());
        //                        if (v_count >= 1)
        //                            flag = "1";
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //            return flag;
        //        }

        private string GetInvoiceNo()
        {
            string inv_sql = "select 'ZS'||lpad(Seq_ZZSB_PrePay_Z.Nextval,8,'0') from dual ";
            return mgr.ExecSqlReturnOne(inv_sql);
        }

        /// <summary>
        /// 一日清单汇总
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public string InPatientFeeInfoTot(His.Models.ZZSB.InpatientTotDayFeeReq reqInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(reqInfo.InpatientNo) || string.IsNullOrEmpty(reqInfo.FeeDate))
                {
                    err = "住院流水号和费用日期不能为空！";
                    return Function.DataSource("0", err, reqInfo.FunCode).ToString();
                }
                DateTime feeDate = DateTime.Now;
                if (!DateTime.TryParse(reqInfo.FeeDate, out feeDate))
                {
                    err = "费用日期格式不正确！";
                    return Function.DataSource("0", err, reqInfo.FunCode).ToString();
                }

                string sql = @"select distinct-- d.inpatient_no ,
                c.fee_stat_name,
                nvl(d.tot_cost, 0) as tot_cost,
                (select 
                       sum(a.tot_cost) as tot_cost2
                  from FIN_COM_FEECODESTAT b, fin_ipb_feeinfo a
                 where b.fee_code = a.fee_code(+)
                   and a.inpatient_no = '{0}'
                   AND a.FEE_DATE <
                       TO_DATE('{1}', 'YYYY-MM-DD HH24:MI:SS')+1
                   and b.report_code = 'ZY01'
                   and b.valid_state = '1') as tot_cost2
                 
  from FIN_COM_FEECODESTAT c,
       (select a.inpatient_no, b.fee_stat_name, sum(a.tot_cost) as tot_cost
          from FIN_COM_FEECODESTAT b, fin_ipb_feeinfo a
         where b.fee_code = a.fee_code(+)
           and a.inpatient_no = '{0}'
           AND a.FEE_DATE >=trunc( TO_DATE('{1}', 'YYYY-MM-DD HH24:MI:SS'))
           AND a.FEE_DATE <trunc(TO_DATE('{1}', 'YYYY-MM-DD HH24:MI:SS')+1)
           and b.report_code = 'ZY01'
           and b.valid_state = '1'
         group by a.inpatient_no, b.fee_stat_name) d
 where c.fee_stat_name = d.fee_stat_name(+)
   and c.report_code = 'ZY01'
   and c.valid_state = '1' ";

                sql = string.Format(sql, reqInfo.InpatientNo, feeDate.ToString("yyyy-MM-dd HH:mm:ss"));

                DataSet ds = new DataSet();
                ds = DataBaseHelp.DataExecHelp.GetDataSet(sql);
                if (ds.Tables[0].Rows.Count < 0 && ds != null)
                {
                    err = "查询费用过程错误！";
                    return Function.DataSource("0", err + mgr.Err, reqInfo.FunCode).ToString();
                }

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    err = "没有查到相关费用数据！";
                    return Function.DataSource("0", err, reqInfo.FunCode).ToString();
                }
                XElement root = Function.DataSource("1", string.Empty, reqInfo.FunCode);
                XElement r = new XElement("Rsult");
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    XElement d = new XElement("FEEINFO");
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                        d.Add(new XElement(ds.Tables[0].Columns[i].ColumnName, row[i].ToString()));
                    r.Add(d);
                }
                root.Element("return").Add(new XElement("InPatientNo", reqInfo.InpatientNo));
                root.Element("return").Add(r);
                return root.ToString();
            }
            catch (Exception ex)
            {
                return Function.DataSource("0", ex.Message, reqInfo.FunCode).ToString();
            }
        }

        /// <summary>
        /// 出院患者费用明细
        /// </summary>
        /// <param name="reqInfo"></param>
        /// <returns></returns>
        public string InPatientFeeDetial(His.Models.ZZSB.InpatientFeeDetailReq reqInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(reqInfo.InpatientNo) || string.IsNullOrEmpty(reqInfo.InvoiceNo))
                {
                    return Function.DataSource("0", "住院流水号和发票不能为空！", reqInfo.FunCode).ToString();
                }


                #region sql

                string sql = @"select 
fi.inpatient_no as InPatientNo,
fi.name,
fi.pact_name as FeeType,
fi.dept_name as DepName,
'1' as IsPrintable,
'' as PrintDate,
fi.in_date as StartDate,
fi.out_date as EndDate,
fee.ItemCode as ItemCode,
fee.feeName as ItemClass,
 fee.ItemName as ItemName,
fee.Standard as Standard,
fee.Units as Units,
 fee.Price as Price,
fee.qty ,
 --ItemFee 
 fee.totCost,
fee.FeeClass as  FeeClass       
  from (select b.inpatient_no as InPatientNo,   
  (select g.fee_stat_name
                  from fin_com_feecodestat g
                 where g.report_code = 'ZY01'
                   and g.fee_code = e.fee_code) as feeName,
               (select nvl(t.gb_code, t.custom_code)
                  from pha_com_baseinfo t
                 where t.drug_code = e.drug_code) as ItemCode,
               e.drug_name as ItemName,
               e.specs as Standard,
               --decode(b.paykind_code, '03', '公费', '') as 公费类别,
               decode(b.paykind_code,
                      '02',
                      (select decode(fc.center_item_grade,
                                     '1',
                                     '甲类',
                                     '2',
                                     '乙类',
                                     '丙类')
                         from fin_com_compare fc
                        where fc.pact_code = e.pact_code
                          and fc.his_code = e.drug_code),
                      '') as 医保类别,
               (case
                 when round(e.unit_price / e.pack_qty, 4) < 1 then
                  '0' || to_char(round(e.unit_price / e.pack_qty, 4))
                 else
                  to_char(round(e.unit_price / e.pack_qty, 4))
               end) Price,
               (case
                 when round(sum(e.qty), 2) < 1 then
                  '0' || to_char(round(sum(e.qty), 2))
                 else
                  to_char(round(sum(e.qty), 2))
               end) as qty,
               e.current_unit as units,
               decode(b.paykind_code,
                      '03',
                      (select round(fp.pay_ratio, 2) * 100 || '%'
                         from fin_com_pactunitinfo fp
                        where fp.pact_code = b.pact_code),
                      '') as 自负比例,
               round(sum(e.tot_cost), 2) as totCost,
               NVL((SELECT CASE C.CENTER_ITEM_GRADE
                            WHEN '1' THEN
                             '甲类'
                            WHEN '2' THEN
                             '乙类'
                            WHEN '3' THEN
                             '丙类'
                            ELSE
                             '未维护级别'
                          END GRADE_LEVEL
                     FROM FIN_COM_COMPARE C
                    WHERE C.PACT_CODE = '14'
                      AND C.HIS_CODE = e.drug_code
                      AND ROWNUM = 1),
                   '未对照') AS FeeClass
          from fin_ipb_medicinelist e, fin_ipr_inmaininfo b
         where e.inpatient_no = b.inpatient_no
           and e.inpatient_no = '{0}'
           and e.invoice_no = '{1}'
           and e.balance_state = '1'
         group by b.inpatient_no,
                  e.fee_code,
                  e.drug_name,
                  e.current_unit,
                  e.drug_code,
                  e.specs,
                  e.unit_price,
                  b.pact_code,
                  e.pack_qty,
                  b.paykind_code,
                  e.execute_deptcode,
                  e.pact_code
        having round(sum(e.tot_cost), 2) <> 0
        union all
        select b.inpatient_no as InPatientNo , 
        (select c.fee_stat_name
                  from fin_com_feecodestat c
                 where c.report_code = 'ZY01'
                   and c.fee_code = a.fee_code) as feeName,
               (select nvl(t.gb_code, t.input_code)
                  from fin_com_undruginfo t
                 where t.item_code = a.item_code) as itemCode,
               a.item_name as itemName,
               (select t.specs
                  from fin_com_undruginfo t
                 where t.item_code = a.item_code) as Standard,
              -- decode(b.paykind_code, '03', '公费', null) as 公费类别,
               decode(b.paykind_code,
                      '02',
                      (select decode(fc.center_item_grade,
                                     '1',
                                     '甲类',
                                     '2',
                                     '乙类',
                                     '丙类')
                         from fin_com_compare fc
                        where fc.pact_code = a.pact_code
                          and fc.his_code = a.item_code),
                      '') as feeClass,
               (case
                 when a.unit_price < 1 then
                  '0' || to_char(a.unit_price)
                 else
                  to_char(a.unit_price)
               end) as 单价,
               (case
                 when round(sum(a.qty), 2) < 1 then
                  '0' || to_char(round(sum(a.qty), 2))
                 else
                  to_char(round(sum(a.qty), 2))
               end) as 数量,
               a.current_unit as 单位,
               decode(b.paykind_code,
                      '03',
                      (select round(fp.pay_ratio, 2) * 100 || '%'
                         from fin_com_pactunitinfo fp
                        where fp.pact_code = b.pact_code),
                      '') as 自负比例,
               round(sum(a.tot_cost), 2) as 金额,
               NVL((SELECT CASE C.CENTER_ITEM_GRADE
                            WHEN '1' THEN
                             '甲类'
                            WHEN '2' THEN
                             '乙类'
                            WHEN '3' THEN
                             '丙类'
                            ELSE
                             '未维护级别'
                          END GRADE_LEVEL
                     FROM FIN_COM_COMPARE C
                    WHERE C.PACT_CODE = '14'
                      AND C.HIS_CODE = a.item_code
                      AND ROWNUM = 1),
                   '未对照') AS itemClass
          from fin_ipb_itemlist a, fin_ipr_inmaininfo b
         where a.inpatient_no = b.inpatient_no
           and b.inpatient_no = '{0}'
           and a.invoice_no = '{1}'
           and a.balance_state = '1'
         group by b.inpatient_no,
         a.fee_code,
                  a.item_name,
                  a.current_unit,
                  a.unit_price,
                  a.fee_code,
                  a.item_code,
                  a.package_name,
                  b.pact_code,
                  b.paykind_code,
                  a.execute_deptcode,
                  a.pact_code
        having round(sum(a.tot_cost), 2) <> 0
         order by feeName, feeName) fee join fin_ipr_inmaininfo fi on fee.inpatientno=fi.inpatient_no      
 order by feeName, feeName ";

                #endregion

                sql = string.Format(sql, reqInfo.InpatientNo, reqInfo.InvoiceNo);
                DataSet ds = new DataSet();
                ds = DataBaseHelp.DataExecHelp.GetDataSet(sql);
                if (ds.Tables[0].Rows.Count < 0 && ds != null)
                {
                    return Function.DataSource("0", "查询错误！", reqInfo.FunCode).ToString();

                }
                else if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return Function.DataSource("0", "没有查到相关费用明细！", reqInfo.FunCode).ToString();
                }

                XElement root = Function.DataSource("1", string.Empty, reqInfo.FunCode);
                XElement r = new XElement("Rsult");
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    XElement d = new XElement("FEEINFO");
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                        d.Add(new XElement(ds.Tables[0].Columns[i].ColumnName, row[i].ToString()));
                    r.Add(d);
                }
                root.Element("return").Add(new XElement("InPatientNo", reqInfo.InpatientNo));
                root.Element("return").Add(r);
                return root.ToString();
            }
            catch (Exception ex)
            {
                return Function.DataSource("0", ex.Message, reqInfo.FunCode).ToString();
            }
        }
    }
}
