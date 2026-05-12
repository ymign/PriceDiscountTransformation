using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Neusoft.HISFC.Models.SqlSugar;

namespace Neusoft.HISFC.Components.OutpatientFee.DB
{
    public class DbBizStatus : Neusoft.FrameWork.Management.Database
    {

        public DataTable GetBizDataTable(string invoiceNo)
        {
            try
            {
                var sql = @" /* 优化说明：使用 LEFT JOIN 替代 select中的子查询，减少表扫描次数 */
SELECT '挂号' AS biz_type,
       NVL(reg.ynsee, '0') AS biz_state,
       reg.name,
       reg.card_no,
       reg.idenno,
       p.invoice_no,
       p.trans_type,
       p.clinic_no,
       p.tot_cost,
       p.own_cost,
       p.pub_cost,
       p.pay_cost,
       p.pay_type,
       fun_get_pay_name(p.pay_type) AS pay_name,
       p.oper_code,
       fun_get_employee_name(p.oper_code) AS oper_name,
       p.oper_date
  FROM fin_opb_accountcardfee p
  LEFT JOIN (SELECT clinic_code, invoice_no, ynsee, name, card_no, idenno
               FROM fin_opr_register
              WHERE invoice_no = '{0}'
                AND ROWNUM = 1) reg
    ON reg.clinic_code = p.clinic_no
   AND reg.invoice_no = p.invoice_no
 WHERE p.invoice_no = '{0}' and p.fee_type<>'3'

UNION ALL

SELECT '缴费' AS biz_type,
       '1' AS biz_state,
       (select cc.name from fin_opr_register cc where cc.clinic_code=reg.clinic_code and rownum=1) name,
       reg.card_no,
       (select cc.idenno from fin_opr_register cc where cc.clinic_code=reg.clinic_code and rownum=1) idenno,
       m.invoice_no,
       m.trans_type,
       reg.clinic_code clinic_no,
       m.tot_cost,
       0 AS own_cost,
       0 AS pub_cost,
       0 AS pay_cost,
       m.mode_code,
       fun_get_pay_name(m.mode_code) AS pay_name,
       m.oper_code,
       fun_get_employee_name(m.oper_code) AS oper_name,
       m.oper_date
  FROM fin_opb_paymode m
  LEFT JOIN (SELECT fee.invoice_no, fee.card_no, fee.clinic_code
               FROM fin_opb_feedetail fee
              WHERE invoice_no = '{0}'
                AND ROWNUM = 1) reg
    ON reg.invoice_no = m.invoice_no
 WHERE m.invoice_no = '{0}'
 ";

                sql = string.Format(sql, invoiceNo);

                var dt = new DataTable();
                DataSet ds = new DataSet();

                var result = this.ExecQuery(sql, ref ds);
                if (result <= 0)
                {
                    return dt;
                }

                if (ds == null || ds.Tables.Count <= 0)
                {
                    return dt;
                }

                dt = ds.Tables[0];
                return dt;


            }
            catch (Exception ex)
            {
                this.Err = "[GetBizList]执行出现异常:" + ex.Message;
                return null;
            }

        }

        /// <summary>
        /// 根据字典type获取字典数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fliter"></param>
        /// <returns></returns>
        public List<Neusoft.FrameWork.Models.NeuObject> GetComDictionaryForType(string type, string code)
        {
            List<Neusoft.FrameWork.Models.NeuObject> al = new List<Neusoft.FrameWork.Models.NeuObject>();
            string sql = @"select p.code,p.name,p.mark,p.sort_id from com_dictionary p where p.type='{0}' and p.valid_state=1";
            if (!string.IsNullOrEmpty(code))
            {
                sql = sql + string.Format(@" and p.code='{0}' ", code);
            }
            sql = string.Format(sql, type, code);
            if (this.ExecQuery(sql) == -1)
                return null;
            Neusoft.FrameWork.Models.NeuObject obj;
            while (this.Reader.Read())
            {
                obj = new Neusoft.FrameWork.Models.NeuObject();
                obj.ID = this.Reader[0].ToString();
                obj.Name = this.Reader[1].ToString();
                obj.Memo = this.Reader[2].ToString();
                obj.User01 = this.Reader[3].ToString();
                al.Add(obj);
            }
            this.Reader.Close();
            return al;
        }

        public bool InsertBizLog(MntBizAdjustLog info)
        {
            try
            {

                var sql = @" INSERT INTO MNT_BIZ_ADJUST_LOG (
    BIZ_TYPE,
    ORIGIN_PK,
    INVOICE_NO,
    PATIENT_NAME,
    OUTPATIENT_ID,
    ID_CARD,
    ITEM_TYPE,
    OLD_VALUE,
    NEW_VALUE,
    OPER_CODE,
    OPER_NAME,
    OPER_IP,
    REMARK,
		CHANGE_RESON
) VALUES (
    '{0}',                     
    '{1}',  
    '{2}',               
    '{3}',                     
    '{4}',                 
    '{5}',        
    '{6}',                 
    '{7}',                         
    '{8}',                          
    '{9}',                          
    '{10}',                        
    '{11}',                
    '{12}',
		'{13}'           
) ";

                sql = string.Format(sql,
                    info.BizType,
                    info.OriginPk,
                    info.InvoiceNo,
                    info.PatientName,
                    info.OutpatientId,
                    info.IdCard,
                    info.ItemType,
                    info.OldValue,
                    info.NewValue,
                    info.OperCode,
                    info.OperName,
                    info.OperIp,
                    info.Remark,
                    info.ChangeReason
                    );

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "执行[InsertBizLog]出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateRegSeeFlag(string clincCode, string seeFlag)
        {
            try
            {

                var sql = @" update fin_opr_register p set p.ynsee='{1}' where p.clinic_code='{0}' ";
                sql = string.Format(sql, clincCode, seeFlag);

                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "执行[UpdateRegSeeFlag]出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateRegPayType(string clincCode, string invoiceNo, string transType, string newPayType, string oldPayType)
        {
            try
            {
                var sql = @" update fin_opb_accountcardfee p set p.pay_type='{0}' where p.invoice_no='{1}' and p.clinic_no='{2}' and p.trans_type='{3}'  and p.pay_type='{4}' ";
                sql = string.Format(sql,
                    newPayType,
                    invoiceNo,
                    clincCode,
                    transType,
                    oldPayType
                    );
                return this.ExecNoQuery(sql) > 0;

            }
            catch (Exception ex)
            {
                this.Err = "执行[UpdateRegPayType]出现异常:" + ex.Message;
                return false;
            }
        }

        public bool UpdateMZPayType(string invoiceNo, string transType, string newPayType, string oldPayType)
        {
            try
            {
                var sql = @" update fin_opb_paymode p set p.mode_code='{0}' where p.invoice_no='{1}' and p.trans_type='{2}' and p.mode_code='{3}' ";
                sql = string.Format(sql,
                    newPayType,
                    invoiceNo,
                    transType,
                    oldPayType
                    );

                return this.ExecNoQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                this.Err = "执行[UpdateMZPayType]出现异常:" + ex.Message;
                return false;
            }
        }

        public List<MntBizAdjustLog> GetLogList()
        {
            try
            {
                var sql = @" select p.log_id,
       p.biz_type,
       p.origin_pk,
       p.invoice_no,
       p.patient_name,
       p.outpatient_id,
       p.id_card,
       p.item_type,
       p.old_value,
       p.new_value,
       p.oper_code,
       p.oper_name,
       p.oper_date,
       p.oper_ip,
       p.remark,
       p.change_reson from mnt_biz_adjust_log p where p.oper_date>sysdate-30 order by p.log_id desc ";

                this.ExecQuery(sql);

                var list = new List<MntBizAdjustLog>();
                MntBizAdjustLog info;
                while (Reader.Read())
                {
                    var i = 0;
                    info = new MntBizAdjustLog();
                    info.LogId = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[i].ToString()); i++;
                    info.BizType = Reader[i].ToString(); i++;
                    info.OriginPk = Reader[i].ToString(); i++;
                    info.InvoiceNo = Reader[i].ToString(); i++;
                    info.PatientName = Reader[i].ToString(); i++;
                    info.OutpatientId = Reader[i].ToString(); i++;
                    info.IdCard = Reader[i].ToString(); i++;
                    info.ItemType = Reader[i].ToString(); i++;
                    info.OldValue = Reader[i].ToString(); i++;
                    info.NewValue = Reader[i].ToString(); i++;
                    info.OperCode = Reader[i].ToString(); i++;
                    info.OperName = Reader[i].ToString(); i++;
                    info.OperDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[i].ToString()); i++;
                    info.OperIp = Reader[i].ToString(); i++;
                    info.Remark = Reader[i].ToString(); i++;
                    info.ChangeReason = Reader[i].ToString(); i++;

                    list.Add(info);

                }

                return list;
            }
            catch (Exception ex)
            {
                this.Err = "执行[GetLogList]出现异常:" + ex.Message;
                return null;
            }




        }




    }
}
