using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data;

namespace His.Business.Recipe
{
    public class RecipeInfo : Shadow.Util.Data.Management.OracleBase
    {

        #region 门诊药房


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="xml"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public int QueryRecipeXml(List<string> list, ref string xml, ref string err)
        {
            try
            {
                string dtlSql = @"select  a.card_no as  Mz_No,
                            decode(b.sex_code,'F','男','M','女','') as Patient_Sex,
                            fun_get_age(b.birthday) as  Age,
                            b.name as Patient_Name  ,
                            a.recipe_no as Prescription_HIS_Id,
                            fun_get_employee_name(a.doct_code) as Prescription_Doctor_Name,
                            fun_get_dept_name(a.doct_dept) as Department_Name,
                            (select r.send_terminal from pha_sto_recipe r 
                                 where a.recipe_no=r.recipe_no and rownum=1) Fetch_Window,
                            a.item_code as Med_his_id,
                            a.specs as Med_unit,
                            a.qty as Medicine_Num,
                            0 as Medicine_Heteromorphism,
                            a.use_name as Medicine_Hint,
                            a.dose_unit as DOSAGE_UNITS,
                            '' as Med_Factory
                            from fin_opb_feedetail a join com_patientinfo b on a.card_no=b.card_no
                            where a.recipe_no = '{0}'
                               and a.drug_flag = '1'
                               and a.cancel_flag = '1'
                               and a.pay_flag = '1'
                               and a.exec_dpcd = '9004'";
                DataSet detail = new DataSet();
                XElement rt = new XElement("ROOT");
                string execDtlSql = string.Empty;
                foreach (var rpcNo in list)
                {
                    execDtlSql = string.Format(dtlSql, rpcNo);
                    if (this.ExecQuery(execDtlSql, ref detail) < 0)
                    {
                        err = this.Err; return -1;
                    }
                    foreach (DataRow row in detail.Tables[0].Rows)
                    {
                        XElement d = new XElement("RECIPEINFO");
                        for (int i = 0; i < detail.Tables[0].Columns.Count; i++)
                            d.Add(new XElement(detail.Tables[0].Columns[i].ColumnName, row[i].ToString()));
                        rt.Add(d);
                    }
                    detail.Tables[0].Rows.Clear();
                }
                xml = rt.ToString();
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public int QueryRecipeInfo(ref string xml, ref string err)
        {
            string sql = @" select a.recipe_no from pha_sto_recipe a
                    where a.fee_date>trunc(sysdate)
                    and a.recipe_state in('0','1','2')
                    and a.valid_state='1'
                    and a.sendpackge='1'
                    and a.senddismount='0' 
                    and a.drug_dept_code='9004'
                    order by a.fee_date asc ";

            List<string> list = new List<string>();

            if (this.ExecQuery(sql) == -1)
            {
                err = this.Err;
                return -1;
            }

            while (this.Reader.Read())
            {
                list.Add(this.Reader[0].ToString());
            }
            this.Reader.Close();
            if (list.Count == 0)
            {
                err = "没有相关数据！";
                return 0;
            }
            if (list.Count > 20)
            {
                list.RemoveRange(20, list.Count - 20);
            }

            return QueryRecipeXml(list, ref xml, ref err);
        }

        /// <summary>
        /// 处方接收后更新接收状态（拆零机）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public int UpdateRecipeFlag(List<string> list, ref string err)
        {
            if (list == null || list.Count == 0)
            {
                err = "处方号列表为空，没有要更新的数据！";
                Shadow.Util.Data.Func.Log.WriteLog("Recipe", err);
            }

            string sql = @"update pha_sto_recipe a 
                        set a.senddismount = '1' 
                        where a.recipe_no = '{0}' ";

            foreach (var item in list)
            {
                sql = string.Format(sql, item);
                if (this.ExecNoQuery(sql) != 1)
                {
                    err = "更新处方状态错误！" + this.Err + sql;
                    Shadow.Util.Data.Func.Log.WriteLog("Recipe", err);
                }
            }
            return 1;
        }


        /// <summary>
        /// 摆药完成更新处方状态
        /// </summary>
        /// <param name="list"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public int UpdateDrugedState(Dictionary<string, string> list, ref string err)
        {
            try
            {
                string sql = @"update pha_sto_recipe a
                    set a.druged_date=sysdate ,
                    a.druged_oper='{1}'
                    where a.recipe_no='{0}'
                    and a.drug_dept_code='9004' ";
                string sqlExec = string.Empty;
                foreach (var item in list)
                {
                    sqlExec = string.Format(sql, item.Key, item.Value);
                    if (this.ExecNoQuery(sqlExec) < 1)
                    {
                        err = this.Err;
                        return -1;
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                err = "摆药完成更新错误！错误信息：" + ex.Message;
                return -1;
            }
        }


        #endregion


        #region 中心药房

        /// <summary>
        /// 更新住院处方接收状态
        /// </summary>
        /// <param name="infs"></param>
        /// <returns></returns>
        public string UpdateDrugedInStatus(List<His.Models.Pha.RecipeStatusInfo> infs)
        {
            try
            {
                string sql = string.Empty;
                sql = @"update pha_com_applyout a
                       set a.pkStatus = '{2}'
                     where a.recipe_no = '{0}'
                       and a.apply_number = '{1}'
                       and a.class3_meaning_code = 'Z1'
                       and a.drug_dept_code = '9008'";
                string execSql = string.Empty;

                Shadow.Util.Data.Management.Trans.BeginTransaction();
                foreach (var item in infs)
                {
                    execSql = string.Format(sql, item.RecipeNo, item.PresDetailID, item.SendStatus);
                    if (this.ExecNoQuery(execSql) == -1)
                    {
                        Shadow.Util.Data.Management.Trans.RollBack();
                        Shadow.Util.Data.Func.Log.WriteLog("InRecipe", this.Err + " *** " + execSql);

                        return new XElement("ROOT",
                            new XElement("RETVAL", "0"),
                            new XElement("RETMSG", this.Err),
                        new XElement("RETCODE", "0")
                        ).ToString();
                    }
                }

                Shadow.Util.Data.Management.Trans.Commit();
            }
            catch (Exception ex)
            {
                Shadow.Util.Data.Func.Log.WriteLog("InRecipe", "更新住院处方状态错误" + ex.Message);
            }
            return new XElement("ROOT",
                           new XElement("RETVAL", "1"),
                           new XElement("RETMSG", "状态更新成功！"),
                       new XElement("RETCODE", "1")
                       ).ToString();
        }



        #endregion
    }
}
