using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Fee
{
    /// <summary>
    /// 门诊指引单
    /// </summary>
    public class MZGuide : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clinic_code"></param>
        /// <returns></returns>
        public List<HISFC.Models.Fee.Outpatient.MZGuide> QueryGuide(string clinic_code)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("Components.OutpatientFee.MZGuide.Query", ref strSql) == -1) return null;
            strSql = string.Format(strSql, clinic_code);

            List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuide> list = this.GetMZGuide(strSql);

            if (list == null || list.Count == 0) return null;
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clinic_code"></param>
        /// <returns></returns>
        public List<HISFC.Models.Fee.Outpatient.MZGuide> QueryZYFGuide(string clinic_code)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("Components.OutpatientFee.MZYFGuide.Query", ref strSql) == -1) return null;
            strSql = string.Format(strSql, clinic_code);

            List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuide> list = this.GetMZGuide(strSql);

            if (list == null || list.Count == 0) return null;
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clinic_code"></param>
        /// <returns></returns>
        public List<HISFC.Models.Fee.Outpatient.MZGuide> QueryGuideRePrint(string clinic_code)
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("Components.OutpatientFee.MZGuide.RePrintQuery", ref strSql) == -1) return null;
            strSql = string.Format(strSql, clinic_code);

            List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuide> list = this.GetMZGuide(strSql);

            if (list == null || list.Count == 0) return null;
            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<HISFC.Models.Fee.Outpatient.MZGuide> GetMZGuide(string sql)
        {
            try
            {
                if (this.ExecQuery(sql) == -1) return null;
                List<HISFC.Models.Fee.Outpatient.MZGuide> list = new List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuide>();
                HISFC.Models.Fee.Outpatient.MZGuide Guide = null;
                while (this.Reader.Read())
                {
                    Guide = new Neusoft.HISFC.Models.Fee.Outpatient.MZGuide();
                    #region 实体
                    if (!Reader.IsDBNull(0))
                        Guide.ID = Reader[0].ToString();
                    if (!Reader.IsDBNull(1))
                        Guide.Item_Code = Reader[1].ToString();
                    if (!Reader.IsDBNull(2))
                        Guide.Item_Name = Reader[2].ToString();
                    if (!Reader.IsDBNull(3))
                        Guide.MO_Order = Reader[3].ToString();
                    if (!Reader.IsDBNull(4))
                        Guide.Exec_Dpcd = Reader[4].ToString();
                    if (!Reader.IsDBNull(5))
                        Guide.Exec_Dpnm = Reader[5].ToString();
                    if (!Reader.IsDBNull(6))
                        Guide.Clinic_Code = Reader[6].ToString();
                    if (!Reader.IsDBNull(7))
                        Guide.Recipe_NO = Reader[7].ToString();
                    if (!Reader.IsDBNull(8))
                        Guide.Class_Code = Reader[8].ToString();
                    if (!Reader.IsDBNull(9))
                        Guide.Drug_Terminal = Reader[9].ToString();
                    if (!Reader.IsDBNull(10))
                        Guide.Send_Terminal = Reader[10].ToString();
                    if (!Reader.IsDBNull(11))
                        Guide.Subjob_Flag = Reader[11].ToString();
                    if (!Reader.IsDBNull(12))
                        Guide.Address = Reader[12].ToString();
                    if (!Reader.IsDBNull(13))
                        Guide.Note = Reader[13].ToString();
                    if (!Reader.IsDBNull(14))
                        Guide.Drug_Flag = Reader[14].ToString();
                    if (!Reader.IsDBNull(15))
                        Guide.Spes = Reader[15].ToString();
                    if (!Reader.IsDBNull(16))
                        Guide.Qty = Reader[16].ToString();
                    if (!Reader.IsDBNull(17))
                        Guide.Tot_Cost = Reader[17].ToString();
                    if (!Reader.IsDBNull(18))
                        Guide.Fee_Date = Reader[18].ToString();
                    if (!Reader.IsDBNull(19))
                        Guide.Usage_Code = Reader[19].ToString();
                    if (!Reader.IsDBNull(20))
                        Guide.Usage_Name = Reader[20].ToString();
                    if (!Reader.IsDBNull(21))
                        Guide.Unit = Reader[21].ToString();
                    if (!Reader.IsDBNull(22))
                        Guide.Check_Body = Reader[22].ToString();
                    if (!Reader.IsDBNull(23))
                        Guide.Lab_Type = Reader[23].ToString();
                    if (!Reader.IsDBNull(24))
                        Guide.InvoiceNo = Reader[24].ToString();
                    //挂号科室
                    if (!Reader.IsDBNull(25))
                        Guide.See_Dpcd = Reader[25].ToString();
                    // {097AA15C-C4CB-4d19-B5C0-76EE20C1ACDE} 内镜中心用药单独备注
                    if (!Reader.IsDBNull(26))
                        Guide.Assess_Flag = Reader[26].ToString();
                    #endregion
                    list.Add(Guide);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed && this.Reader != null)
                {
                    this.Reader.Close();
                }
            }
        }

        public List<HISFC.Models.Fee.Outpatient.MZGuideSpecialExecDept> QueryGuideSpecialDept()
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("Components.OutpatientFee.MZGuideSepecial.Query", ref strSql) == -1) return null;
            // strSql = string.Format(strSql,);

            List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideSpecialExecDept> list = this.GetMZGuideSpecialDept(strSql);

            if (list == null || list.Count == 0) return null;
            return list;
        }

        private List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideSpecialExecDept> GetMZGuideSpecialDept(string strSql)
        {
            try
            {
                if (this.ExecQuery(strSql) == -1) return null;
                List<HISFC.Models.Fee.Outpatient.MZGuideSpecialExecDept> list = new List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideSpecialExecDept>();
                HISFC.Models.Fee.Outpatient.MZGuideSpecialExecDept GuideSpecial = null;
                while (this.Reader.Read())
                {
                    GuideSpecial = new Neusoft.HISFC.Models.Fee.Outpatient.MZGuideSpecialExecDept();
                    if (!Reader.IsDBNull(0))
                        GuideSpecial.Usage_Code = Reader[0].ToString();
                    if (!Reader.IsDBNull(1))
                        GuideSpecial.Usage_Name = Reader[1].ToString();
                    if (!Reader.IsDBNull(2))
                        GuideSpecial.Exec_Dpcd = Reader[2].ToString();
                    if (!Reader.IsDBNull(3))
                        GuideSpecial.Address = Reader[3].ToString();
                    if (!Reader.IsDBNull(4))
                        GuideSpecial.Exec_Dpnm = Reader[4].ToString();
                    list.Add(GuideSpecial);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed && this.Reader != null)
                {
                    this.Reader.Close();
                }
            }
        }



        public List<HISFC.Models.Fee.Outpatient.MZGuideContrast> QueryGuideULContrast()
        {
            string strSql = string.Empty;
            if (this.Sql.GetCommonSql("Components.OutpatientFee.GuideULContrast.Query", ref strSql) == -1) return null;
            // strSql = string.Format(strSql,);

            List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast> list = this.GetGuideULContrast(strSql);

            if (list == null || list.Count == 0) return null;
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast> GetGuideULContrast(string strSql)
        {
            try
            {
                if (this.ExecQuery(strSql) == -1) return null;
                List<HISFC.Models.Fee.Outpatient.MZGuideContrast> list = new List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast>();
                HISFC.Models.Fee.Outpatient.MZGuideContrast GuideContrast = null;
                while (this.Reader.Read())
                {
                    GuideContrast = new Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast();
                    if (!Reader.IsDBNull(0))
                        GuideContrast.ItemCode = Reader[0].ToString();
                    if (!Reader.IsDBNull(1))
                        GuideContrast.ItemName = Reader[1].ToString();
                    if (!Reader.IsDBNull(2))
                        GuideContrast.LabCode = Reader[2].ToString();
                    if (!Reader.IsDBNull(3))
                        GuideContrast.LabName = Reader[3].ToString();
                    if (!Reader.IsDBNull(4))
                        GuideContrast.Addr_Code = Reader[4].ToString();
                    if (!Reader.IsDBNull(5))
                        GuideContrast.Addresses = Reader[5].ToString();
                    if (!Reader.IsDBNull(6))
                        GuideContrast.SpellCode = Reader[6].ToString();
                    if (!Reader.IsDBNull(7))
                        GuideContrast.FineCode = Reader[7].ToString();
                    if (!Reader.IsDBNull(8))
                        GuideContrast.OperCode = Reader[8].ToString();
                    if (!Reader.IsDBNull(9))
                        GuideContrast.OperDate = Reader[9].ToString();
                    if (!Reader.IsDBNull(10))
                        GuideContrast.Mark = Reader[10].ToString();
                    if (!Reader.IsDBNull(11))
                        GuideContrast.ValidState = Reader[11].ToString();
                    if (!Reader.IsDBNull(12))
                        GuideContrast.Urgency = Reader[12].ToString();
                    list.Add(GuideContrast);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed && this.Reader != null)
                {
                    this.Reader.Close();
                }
            }
        }

        public ArrayList QueryULList()
        {
            string sql = @"   select a.item_code,a.item_name,a.spell_code,a.wb_code,a.valid_state from fin_com_undruginfo a
                          where a.sys_class='UL' and a.valid_state='1'";
            ArrayList list = this.GetULList(sql);
            if (list == null || list.Count == 0) return null;
            return list;
        }

        /// <summary>
        /// 指引单检验地址对照列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList GetULList(string sql)
        {

            if (this.ExecQuery(sql) == -1) return null;
            ArrayList list = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.SOC.HISFC.Fee.Models.Undrug item = new Neusoft.SOC.HISFC.Fee.Models.Undrug();
                if (!Reader.IsDBNull(0))
                    item.ID = Reader[0].ToString();
                if (!Reader.IsDBNull(1))
                    item.Name = Reader[1].ToString();
                if (!Reader.IsDBNull(2))
                    item.SpellCode = Reader[2].ToString();
                if (!Reader.IsDBNull(3))
                    item.WBCode = Reader[3].ToString();
                if (!Reader.IsDBNull(4))
                    item.ValidState = Reader[4].ToString();
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int AddULContrast(Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item, ref string errMsg)
        {
            try
            {
                //                string sql = @"  insert into FIN_COM_ULContrast
                // (ITEM_CODE,ITEM_NAME,LAB_CODE,LAB_NAME,ADDR_CODE,ADDRESS,SPELL_CODE,WB_CODE,VALID_STATE,MARK,OPER_CODE,OPER_DATE)
                // values ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',ULContrast_Seq.Nextval,'{10}',to_date('{11}','yyyy-mm-dd hh24:mi:ss') ) ";
                string sql = @" insert into FIN_COM_ULContrast
 (ITEM_CODE,ITEM_NAME,LAB_CODE,LAB_NAME,ADDR_CODE,ADDRESS,SPELL_CODE,WB_CODE,VALID_STATE,MARK,OPER_CODE,OPER_DATE,urgency )
 values ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',ULContrast_Seq.Nextval,'{10}',to_date('{11}','yyyy-mm-dd hh24:mi:ss'),'{12}' ) ";
                sql = string.Format(sql, item.ItemCode, item.ItemName, item.LabCode, item.LabName, item.Addr_Code, item.Addresses, item.SpellCode,
                    item.FineCode, item.ValidState, item.Mark, item.OperCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), item.Urgency);
                // return
                if (this.ExecNoQuery(sql) == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return 1;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int ModefyULContrast(Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item, ref string errMsg)
        {
            try
            {
                //                string sql = @"
                //update FIN_COM_ULContrast a
                // set a.lab_code='{0}',
                //     a.lab_name='{1}',
                //     a.addr_code='{2}',
                //     a.address='{3}',
                //     a.valid_state='{4}',
                //     a.oper_code='{5}',
                //     a.oper_date=sysdate
                //     where a.item_code='{6}'
                //　　　and a.mark='{7}'
                string sql = @"
update FIN_COM_ULContrast a
 set a.lab_code='{0}',
     a.lab_name='{1}',
     a.addr_code='{2}',
     a.address='{3}',
     a.valid_state='{4}',
     a.oper_code='{5}',
     a.oper_date=sysdate,
     a.urgency='{8}'
     where a.item_code='{6}'
　　　and a.mark='{7}'

";

                sql = string.Format(sql, item.LabCode, item.LabName, item.Addr_Code, item.Addresses, item.ValidState, item.OperCode, item.ItemCode, item.Mark, item.Urgency);
                if (this.ExecNoQuery(sql) == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return 1;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int DelULContrast(Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item, ref string errMsg)
        {
            try
            {
                string sql = "delete from FIN_COM_ULContrast where mark='{0}'";
                sql = string.Format(sql, item.Mark);
                return this.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }

        }

        public string GetCardNoByClincCode(string code)
        {
            try
            {
                string sql = @" select r.card_no from fin_opr_register r where r.clinic_code='{0}' and rownum=1 ";
                if (string.IsNullOrEmpty(code))
                {
                    return string.Empty;
                }
                sql = string.Format(sql, code);
                return this.ExecSqlReturnOne(sql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return string.Empty;
            }
        }

        /// <summary>
        /// 查询非药品项目能否加急
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetUrgency(string code)
        {
            try
            {
                string sql = @"select a.urgency from FIN_COM_ULContrast a where a.item_code ='{0}'";
                if (string.IsNullOrEmpty(code))
                {
                    return string.Empty;
                }
                sql = string.Format(sql, code);
                return this.ExecSqlReturnOne(sql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return string.Empty;
            }
        }


        #region  非药品非检验项目维护
        public ArrayList QueryUndrugNotULList()
        {
            string sql = @"   select a.item_code,a.item_name,a.spell_code,a.wb_code,a.valid_state from fin_com_undruginfo a
                          where a.sys_class <>'UL' and a.valid_state='1'";
            ArrayList list = this.GetULList(sql);
            if (list == null || list.Count == 0) return null;
            return list;
        }

        /// <summary>
        /// 指引单非药品非检验地址对照列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList GetUndrugNotULList(string sql)
        {

            if (this.ExecQuery(sql) == -1) return null;
            ArrayList list = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.SOC.HISFC.Fee.Models.Undrug item = new Neusoft.SOC.HISFC.Fee.Models.Undrug();
                if (!Reader.IsDBNull(0))
                    item.ID = Reader[0].ToString();
                if (!Reader.IsDBNull(1))
                    item.Name = Reader[1].ToString();
                if (!Reader.IsDBNull(2))
                    item.SpellCode = Reader[2].ToString();
                if (!Reader.IsDBNull(3))
                    item.WBCode = Reader[3].ToString();
                if (!Reader.IsDBNull(4))
                    item.ValidState = Reader[4].ToString();
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// 插入指引单非药品非检验地址对照列表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int AddNotULContrast(Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item, ref string errMsg)
        {
            try
            {
                string sql = @" insert into fin_com_undrugcontrast
 (ITEM_CODE,ITEM_NAME,DEPT_CODE,DEPT_NAME,ADDR_CODE,ADDRESS,SPELL_CODE,WB_CODE,VALID_STATE,MARK,OPER_CODE,OPER_DATE,urgency )
 values ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',NOTULCONTRAST_SEQ.Nextval,'{10}',to_date('{11}','yyyy-mm-dd hh24:mi:ss'),'{12}' ) ";
                sql = string.Format(sql, item.ItemCode, item.ItemName, item.LabCode, item.LabName, item.Addr_Code, item.Addresses, item.SpellCode,
                    item.FineCode, item.ValidState, item.Mark, item.OperCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), item.Urgency);
                // return
                if (this.ExecNoQuery(sql) == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return 1;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }


        }

        /// <summary>
        /// 更新指引单非药品非检验地址对照列表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int ModefyNotULContrast(Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item, ref string errMsg)
        {
            try
            {
                string sql = @"
update fin_com_undrugcontrast a
 set a.dept_code='{0}',
     a.dept_name='{1}',
     a.addr_code='{2}',
     a.address='{3}',
     a.valid_state='{4}',
     a.oper_code='{5}',
     a.oper_date=sysdate,
     a.urgency='{8}'
     where a.item_code='{6}'
　　　and a.mark='{7}'

";

                sql = string.Format(sql, item.LabCode, item.LabName, item.Addr_Code, item.Addresses, item.ValidState, item.OperCode, item.ItemCode, item.Mark, item.Urgency);
                if (this.ExecNoQuery(sql) == -1)
                {
                    errMsg = this.Err;
                    return -1;
                }
                else
                {
                    return 1;
                }

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }

        }

        /// <summary>
        /// 删除指引单非药品非检验地址对照列表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int DelNotULContrast(Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast item, ref string errMsg)
        {
            try
            {
                string sql = "delete from fin_com_undrugcontrast where mark='{0}'";
                sql = string.Format(sql, item.Mark);
                return this.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }

        }


        public List<HISFC.Models.Fee.Outpatient.MZGuideContrast> QueryGuideNotULContrast()
        {
            string strSql = string.Empty;
            strSql = @"
select a.item_code,a.item_name,a.dept_code,a.dept_name,a.addr_code,a.address,a.spell_code,a.wb_code,a.oper_code,a.oper_date,a.mark,
decode(a.valid_state,'1','是','否') valid_state,decode(a.urgency,'1','是','否') urgency 
from Fin_Com_Undrugcontrast a  where valid_state='1'
order by a.item_code";

            List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast> list = this.GetGuideNotULContrast(strSql);

            if (list == null || list.Count == 0) return null;
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast> GetGuideNotULContrast(string strSql)
        {
            try
            {
                if (this.ExecQuery(strSql) == -1) return null;
                List<HISFC.Models.Fee.Outpatient.MZGuideContrast> list = new List<Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast>();
                HISFC.Models.Fee.Outpatient.MZGuideContrast GuideContrast = null;
                while (this.Reader.Read())
                {
                    GuideContrast = new Neusoft.HISFC.Models.Fee.Outpatient.MZGuideContrast();
                    if (!Reader.IsDBNull(0))
                        GuideContrast.ItemCode = Reader[0].ToString();
                    if (!Reader.IsDBNull(1))
                        GuideContrast.ItemName = Reader[1].ToString();
                    if (!Reader.IsDBNull(2))
                        GuideContrast.LabCode = Reader[2].ToString();
                    if (!Reader.IsDBNull(3))
                        GuideContrast.LabName = Reader[3].ToString();
                    if (!Reader.IsDBNull(4))
                        GuideContrast.Addr_Code = Reader[4].ToString();
                    if (!Reader.IsDBNull(5))
                        GuideContrast.Addresses = Reader[5].ToString();
                    if (!Reader.IsDBNull(6))
                        GuideContrast.SpellCode = Reader[6].ToString();
                    if (!Reader.IsDBNull(7))
                        GuideContrast.FineCode = Reader[7].ToString();
                    if (!Reader.IsDBNull(8))
                        GuideContrast.OperCode = Reader[8].ToString();
                    if (!Reader.IsDBNull(9))
                        GuideContrast.OperDate = Reader[9].ToString();
                    if (!Reader.IsDBNull(10))
                        GuideContrast.Mark = Reader[10].ToString();
                    if (!Reader.IsDBNull(11))
                        GuideContrast.ValidState = Reader[11].ToString();
                    if (!Reader.IsDBNull(12))
                        GuideContrast.Urgency = Reader[12].ToString();
                    list.Add(GuideContrast);
                }
                return list;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed && this.Reader != null)
                {
                    this.Reader.Close();
                }
            }
        }

        #endregion
    }
}
