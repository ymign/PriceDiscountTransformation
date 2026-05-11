using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Pharmacy
{
    /// <summary>
    /// 麻醉药品一类精神药品使用登记管理类
    /// </summary>
    public class MJRegister : Neusoft.FrameWork.Management.Database
    {
        public MJRegister()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        /// <summary>
        /// 绑定药品下拉框
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryMJDrugList()
        {
            string sql = @"select a.drug_code,a.trade_name,a.spell_code,a.wb_code,a.valid_state from pha_com_baseinfo a
                          where a.valid_state='1' and a.drug_quality in ('SY','S1','P1','YZ') ";
            ArrayList list = this.GetDrugList(sql);
            if (list == null || list.Count == 0) return null;
            return list;
        }

        public ArrayList GetDrugList(string sql)
        {

            if (this.ExecQuery(sql) == -1) return null;
            ArrayList list = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Pharmacy.Item item = new Neusoft.HISFC.Models.Pharmacy.Item();
                if (!Reader.IsDBNull(0))
                    item.ID = Reader[0].ToString();
                if (!Reader.IsDBNull(1))
                    item.Name = Reader[1].ToString();
                if (!Reader.IsDBNull(2))
                    item.SpellCode = Reader[2].ToString();
                if (!Reader.IsDBNull(3))
                    item.WBCode = Reader[3].ToString();
                list.Add(item);
            }

            return list;
        }

        public int InsertMJRegisterInfo(Neusoft.HISFC.Models.Pharmacy.PhaComMjregister model, ref string errMsg)
        {
            try
            {
                string sql = @"INSERT INTO PHA_COM_MJREGISTER
  (DRUG_DEPT_CODE,
   OUT_BILL_CODE,
   SERIAL_CODE,
   BATCH_NO,
   IS_BACK,
   REMARK,
   EXECUTOR,
   EXECUTE_DATE)
VALUES
  ('{0}',
   '{1}',
   '{2}',
   '{3}',
   '{4}',
   '{5}',
   '{6}',
   TO_DATE('{7}', 'yyyy-mm-dd hh24:mi:ss'))";
                sql = string.Format(sql, model.DrugDeptCode, model.OutBillCode, model.SerialCode, model.BatchNo, model.IsBack, model.Remark, model.Executor, model.ExecuteDate.ToString("yyyy-MM-dd HH:mm:ss"));
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }
        }

        public int UpdateMJRegisterInfo(Neusoft.HISFC.Models.Pharmacy.PhaComMjregister model,ref string errMsg)
        {
            try
            {
                string sql = "UPDATE pha_com_MJREGISTER f SET f.batch_no = '{3}',f.is_back = '{4}',f.remark = '{5}',f.reviewer = '{6}',f.review_date = TO_DATE('{7}','yyyy-mm-dd hh24:mi:ss') WHERE f.drug_dept_code = '{0}' AND f.out_bill_code = '{1}' AND f.serial_code = '{2}'";
                sql = string.Format(sql, model.DrugDeptCode, model.OutBillCode, model.SerialCode, model.BatchNo, model.IsBack, model.Remark, model.Reviewer, model.ReviewDate.ToString("yyyy-MM-dd HH:mm:ss"));
                int result = this.ExecNoQuery(sql);
                if (result == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }
        }


    }
}
