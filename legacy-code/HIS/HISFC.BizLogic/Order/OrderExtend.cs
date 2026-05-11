using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Order
{
    /// <summary>
    /// [功能描述：住院医嘱信息扩展业务类]
    /// [创 建 者：]
    /// [创建时间：]
    /// </summary>
    public class OrderExtend : Neusoft.FrameWork.Management.Database
    {
        public OrderExtend()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 内部私有方法

        /// <summary>
        /// 根据SQL查询医嘱扩展信息
        /// </summary>
        /// <param name="wheSql">Whe子句</param>
        /// <returns>成功返回医嘱扩展信息实体 失败返回null</returns>
        private ArrayList QueryOrderExtends(string wheSql, params string[] args)
        {
            string strSql = "";
            string selSql = "";
            //取SELECT子句
            selSql = this.GetCommonSqlForSelectAllOrderExtends();

            //取WHERE子句
            try
            {
                if (!string.IsNullOrEmpty(wheSql))
                {
                    if (this.Sql.GetCommonSql(wheSql, ref wheSql) == -1)
                    {
                        this.Err = "没有找到" + wheSql + "字段!";
                        return null;
                    }
                    strSql = selSql + "\r\n" + wheSql;
                    strSql = string.Format(strSql, args);
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }

            ArrayList orderExtendList = new ArrayList();

            //执行Sql语句 
            try
            {
                this.ExecQuery(strSql);

                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Order.Inpatient.OrderExtend orderExtend = new Neusoft.HISFC.Models.Order.Inpatient.OrderExtend();
                    orderExtend.InPatientNo = this.Reader[0].ToString();  //住院流水号
                    orderExtend.MoOrder = this.Reader[1].ToString();   //医嘱流水号
                    orderExtend.Indications = this.Reader[2].ToString();//适应症信息
                    orderExtend.Extend1 = this.Reader[3].ToString(); //备注1
                    orderExtend.Extend2 = this.Reader[4].ToString();                                            //备注2 
                    orderExtend.Extend3 = this.Reader[5].ToString();                                       //备注3 
                    orderExtend.Extend4 = this.Reader[6].ToString();                                      //备注4 
                    orderExtend.Extend5 = this.Reader[7].ToString();                                         //备注5 
                    orderExtend.Extend6 = this.Reader[8].ToString();
                    orderExtend.Extend7 = this.Reader[9].ToString();
                    orderExtend.Extend8 = this.Reader[10].ToString();
                    orderExtend.Extend9 = this.Reader[11].ToString();
                    orderExtend.Extend10 = this.Reader[12].ToString();
                    orderExtend.Oper.ID = this.Reader[13].ToString();
                    orderExtend.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[14]);

                    orderExtendList.Add(orderExtend);
                }
                return orderExtendList;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (Reader != null)
                {
                    this.Reader.Close();
                }
            }
        }

        /// <summary>
        /// 更新单表操作
        /// </summary>
        /// <param name="sqlIndex">SQL语句索引</param>
        /// <param name="args">参数</param>
        /// <returns>成功: >= 1 失败 -1 没有更新到数据 0</returns>
        private int UpdateSingleTable(string sqlIndex, params string[] args)
        {
            string sql = string.Empty;//Update语句

            //获得Where语句
            if (this.Sql.GetCommonSql(sqlIndex, ref sql) == -1)
            {
                this.Err = "没有找到索引为:" + sqlIndex + "的SQL语句";

                return -1;
            }
            sql = string.Format(sql, args);
            return this.ExecNoQuery(sql);
        }

        /// <summary>
        /// 获得医嘱扩展信息字符串数组
        /// </summary>
        /// <param name="prepay">医嘱扩展信息实体</param>
        /// <returns>成功: 医嘱扩展信息字符串数组 失败: null</returns>
        private string[] GetOrderExtendParams(Neusoft.HISFC.Models.Order.Inpatient.OrderExtend orderExtend)
        {
            string[] args ={
                               //住院流水号
                               orderExtend.InPatientNo,
                               //医嘱流水号
                               orderExtend.MoOrder,
							   //适应症
							   orderExtend.Indications,
							   //备注1
							   orderExtend.Extend1,
                                //备注2
							   orderExtend.Extend2,
                                //备注3
							   orderExtend.Extend3,
                                //备注4
							   orderExtend.Extend4,
				               //备注5
							   orderExtend.Extend5,
                                //备注6
							   orderExtend.Extend6,
                               orderExtend.Extend7,
                               orderExtend.Extend8,
                               orderExtend.Extend9,
                               orderExtend.Extend10,
                               orderExtend.Oper.ID,
                               orderExtend.Oper.OperTime.ToString()
						   };

            return args;
        }

        /// <summary>
        /// 获取检索met_ipm_order_extend的全部数据的sql
        /// </summary>
        /// <returns></returns>
        private string GetCommonSqlForSelectAllOrderExtends()
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Order.Extend.SelectAllOrderExtend", ref strSql) == -1)
            {
                return null;
            }
            return strSql;
        }

        #endregion

        #region 增删改

        /// <summary>
        /// 插入医嘱扩展信息
        /// </summary>
        /// <param name="prepay">医嘱扩展信息实体</param>
        /// <returns>成功: 1 失败 -1 没有插入数据 0</returns>
        public int InsertOrderExtend(Neusoft.HISFC.Models.Order.Inpatient.OrderExtend orderExtend)
        {
            string[] parms = new string[9];
            parms = this.GetOrderExtendParams(orderExtend);
            return this.UpdateSingleTable("Order.Extend.InsertOrderExtend", parms);
        }

        /// <summary>
        /// 更新医嘱扩展信息
        /// </summary>
        /// <param name="prepay">医嘱扩展信息实体</param>
        /// <returns></returns>
        public int UpdateOrderExtend(Neusoft.HISFC.Models.Order.Inpatient.OrderExtend orderExtend)
        {
            return this.UpdateSingleTable("Order.Extend.UpdateOrderExtend", this.GetOrderExtendParams(orderExtend));
        }

        /// <summary>
        /// 删除医嘱扩展信息
        /// </summary>
        /// <param name="prepay">医嘱扩展信息实体</param>
        /// <returns></returns>
        public int DeleteOrderExtend(Neusoft.HISFC.Models.Order.Inpatient.OrderExtend orderExtend)
        {
            return this.UpdateSingleTable("Order.Extend.DeleteOrderExtend", this.GetOrderExtendParams(orderExtend));
        }

        public int UpdateOrderExtendWhenAgainChose(string inpatientNO, string moOder, string limitResult)
        {
            try
            {
                string sql = string.Format(@" update met_ipm_orderextend
set
       EXTEND3='{2}'
       oper_date=sysdate --操作时间
where INPATIENT_NO='{0}'--住院流水号
       and MO_ORDER='{1}'--医嘱流水号 ", inpatientNO, moOder, limitResult);
                return this.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }

        public int UpdateOrInsertWhenAgainChose(Neusoft.HISFC.Models.Order.Inpatient.OrderExtend orderExtend)
        {
            string sql = "";
            try
            {
                sql = string.Format(@" select count(1) from met_ipm_orderextend p where p.inpatient_no='{0}' and p.mo_order='{1}' ", orderExtend.InPatientNo, orderExtend.MoOrder);
                string result = this.ExecSqlReturnOne(sql, "");
                if (string.IsNullOrEmpty(result) || result == "0") //代表新增
                {
                    if (this.InsertOrderExtend(orderExtend) <= 0)
                    {
                        this.Err = "插入数据失败，流水号:" + orderExtend.MoOrder;
                        return -1;
                    }
                    return 1;
                }
                else
                {
                    sql = string.Format(@" update met_ipm_orderextend
set
       EXTEND3='{2}',
       oper_date=sysdate --操作时间
where INPATIENT_NO='{0}'--住院流水号
       and MO_ORDER='{1}'--医嘱流水号 ", orderExtend.InPatientNo, orderExtend.MoOrder, orderExtend.Extend3);
                }
                return this.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }


        /// <summary>
        /// 划价时复制原有数据的限制用药信息
        /// </summary>
        /// <param name="inpatientNO"></param>
        /// <param name="oldMoOrder"></param>
        /// <param name="newMoOrder"></param>
        /// <returns></returns>
        public int CopyOrderExtend(string inpatientNO, string oldMoOrder, string newMoOrder)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("Order.Extend.CopyOrderExtend", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = string.Format(strSql, inpatientNO, oldMoOrder, newMoOrder);
            if (strSql == null) return -1;

            return this.ExecNoQuery(strSql);
        }

        #endregion

        #region 查询函数

        /// <summary>
        /// 根据住院流水号、医嘱流水号取医嘱扩展信息
        /// </summary>
        /// <param name="prepay">医嘱扩展信息实体</param>
        /// <return></return></returns>
        public Neusoft.HISFC.Models.Order.Inpatient.OrderExtend QueryByInpatineNoOrderID(string inpatientNO, string orderID)
        {
            ArrayList al = this.QueryOrderExtends("Order.Extend.QueryByInpatineNoOrderID", inpatientNO, orderID);

            if (al == null || al.Count == 0)
            {
                return null;
            }

            return al[0] as Neusoft.HISFC.Models.Order.Inpatient.OrderExtend;
        }

        /// <summary>
        /// 根据住院流水号取医嘱扩展信息
        /// </summary>
        /// <param name="prepay">医嘱扩展信息实体</param>
        /// <return></return></returns>
        public ArrayList QueryByInpatine(string inpatientNO)
        {
            ArrayList al = this.QueryOrderExtends("Order.Extend.QueryByInpatient", inpatientNO);

            //if (al == null || al.Count == 0)
            //{
            //    return null;
            //}

            return al;
        }

        public bool IsExistNotChose(string inpatientNo)
        {
            string sql = string.Format(@"select sum(numCount) from (
select  count(1) numCount from fin_ipb_itemlist b,com_dictionary a where a.type='ZHUHAILIMITLimtHC' and b.inpatient_no='{0}' and b.item_code=a.code   and not exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order)
union all
 select count(1) numCount from fin_ipb_itemlist b,com_dictionary a where a.type='ZHUHAILIMITUNDRUG' and b.inpatient_no='{0}' and b.item_code=a.code and b.operationno is not null  and not exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order)
union all
 select count(1) numCount from fin_ipb_medicinelist b,com_dictionary a where a.type='ZHUHAILIMITDRUG' and b.inpatient_no='{0}' and b.DRUG_CODE=(select v.drug_code from pha_com_baseinfo  v where v.custom_code=a.code and rownum='1') and b.operationno is not null  and not exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order) 
 
) ", inpatientNo);
            try
            {
                string strReturn = this.ExecSqlReturnOne(sql, "0");
                if (strReturn == "0" || string.IsNullOrEmpty(strReturn)|| strReturn == "-1")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return false;
            }
        }

        public List<Neusoft.HISFC.Models.Order.Inpatient.OrderExtend> QueryNotChoseResultWhenAgainChose(string inpatientNO) 
        {

            //执行Sql语句 
            try
            {
                string strSql = string.Format(@"  select b.inpatient_no,b.mo_order,b.item_code,b.item_name,b.charge_date mo_date,'2' extend3,a.mark extend4 from fin_ipb_itemlist b,com_dictionary a where a.type='ZHUHAILIMITUNDRUG' and b.inpatient_no='{0}' and b.item_code=a.code and b.operationno is not null  and not exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order)
union all
select b.inpatient_no,b.mo_order,b.DRUG_CODE,b.DRUG_Name,b.charge_date mo_date,'2' extend3,a.mark extend4 from fin_ipb_medicinelist b,com_dictionary a where a.type='ZHUHAILIMITDRUG' and b.inpatient_no='{0}' and b.DRUG_CODE=(select v.drug_code from pha_com_baseinfo  v where v.custom_code=a.code and rownum='1') and b.operationno is not null  and not exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order)
union all
 select b.inpatient_no,b.mo_order,b.item_code,b.item_name,b.charge_date mo_date,'2' extend3,a.mark extend4 from fin_ipb_itemlist b,com_dictionary a where a.type='ZHUHAILIMITLimtHC' and b.inpatient_no='{0}' and b.item_code=a.code   and not exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order)
", inpatientNO);
                List<Neusoft.HISFC.Models.Order.Inpatient.OrderExtend> orderExtendList = new List<Neusoft.HISFC.Models.Order.Inpatient.OrderExtend>();
                this.ExecQuery(strSql);
                Neusoft.HISFC.Models.Order.Inpatient.OrderExtend orderExtend;
                while (this.Reader.Read())
                {
                    orderExtend = new Neusoft.HISFC.Models.Order.Inpatient.OrderExtend();
                    orderExtend.InPatientNo = this.Reader[0].ToString();  //住院流水号
                    orderExtend.MoOrder = this.Reader[1].ToString();   //医嘱流水号
                    orderExtend.Extend1 = this.Reader[2].ToString(); //备注1
                    orderExtend.Extend2 = this.Reader[3].ToString();
                    orderExtend.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[4]);
                    orderExtend.Extend3 = this.Reader[5].ToString();
                    orderExtend.Extend4 = this.Reader[6].ToString();

                    orderExtendList.Add(orderExtend);
                }
                return orderExtendList;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (Reader != null)
                {
                    this.Reader.Close();
                }
            }
        }

        public List<Neusoft.HISFC.Models.Order.Inpatient.OrderExtend> QueryWhenAgainChoseByInpatientNo(string inpatientNO)
        {
            //执行Sql语句 
            try
            {
                string strSql = string.Format(@"  select b.inpatient_no,b.mo_order,b.item_code,b.item_name,b.charge_date mo_date,'2' extend3,a.mark extend4 from fin_ipb_itemlist b,com_dictionary a where a.type='ZHUHAILIMITUNDRUG' and b.inpatient_no='{0}' and b.item_code=a.code and b.operationno is not null  and not exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order)
union all
select b.inpatient_no,b.mo_order,b.DRUG_CODE,b.DRUG_Name,b.charge_date mo_date,'2' extend3,a.mark extend4 from fin_ipb_medicinelist b,com_dictionary a where a.type='ZHUHAILIMITDRUG' and b.inpatient_no='{0}' and b.DRUG_CODE=(select v.drug_code from pha_com_baseinfo  v where v.custom_code=a.code and rownum='1') and b.operationno is not null  and not exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order)
union all
 select b.inpatient_no,b.mo_order,b.item_code,b.item_name,b.charge_date mo_date,'2' extend3,a.mark extend4 from fin_ipb_itemlist b,com_dictionary a where a.type='ZHUHAILIMITLimtHC' and b.inpatient_no='{0}' and b.item_code=a.code   and not exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order)
union all
select b.inpatient_no,b.mo_order,b.item_code,b.item_name,b.charge_date mo_date,(select v.extend3 from met_ipm_orderextend v where v.inpatient_no=b.inpatient_no and v.mo_order=b.mo_order) extend3,a.mark extend4 from fin_ipb_itemlist b,com_dictionary a where a.type='ZHUHAILIMITUNDRUG'  and b.operationno is not null and b.inpatient_no='{0}' and b.item_code=a.code  and  exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order) 
union all
select b.inpatient_no,b.mo_order,b.DRUG_CODE,b.DRUG_Name,b.charge_date mo_date,(select v.extend3 from met_ipm_orderextend v where v.inpatient_no=b.inpatient_no and v.mo_order=b.mo_order) extend3,a.mark extend4 from fin_ipb_medicinelist b,com_dictionary a where a.type='ZHUHAILIMITDRUG' and b.inpatient_no='{0}' and b.DRUG_CODE=(select v.drug_code from pha_com_baseinfo  v where v.custom_code=a.code and rownum='1') and b.operationno is not null  and  exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order)
union all
select b.inpatient_no,b.mo_order,b.item_code,b.item_name,b.charge_date mo_date,(select v.extend3 from met_ipm_orderextend v where v.inpatient_no=b.inpatient_no and v.mo_order=b.mo_order) extend3,a.mark extend4 from fin_ipb_itemlist b,com_dictionary a where a.type='ZHUHAILIMITLimtHC' and b.inpatient_no='{0}' and b.item_code=a.code  and  exists (select 1 from met_ipm_orderextend q where q.inpatient_no=b.inpatient_no and q.mo_order=b.mo_order) 
union all 
select * from (
 select p.inpatient_no,p.mo_order,a.item_code,a.item_name,a.mo_date,p.extend3,
nvl(p.extend4,nvl((select l.mark from com_dictionary l where l.type in ('ZHUHAILIMITUNDRUG','ZHUHAILIMITLimtHC') and a.ITEM_CODE=l.code),(select l.mark from com_dictionary l,pha_com_baseinfo x where l.type in ('ZHUHAILIMITDRUG') and l.code=x.custom_code  and a.ITEM_CODE=x.drug_code))) extend4
from met_ipm_orderextend p left join  met_ipm_order a on p.inpatient_no=a.inpatient_no and p.mo_order=a.mo_order where  p.inpatient_no='{0}' and p.extend3 is not null and a.mo_order is not null order by p.oper_date )  ", inpatientNO);
                List<Neusoft.HISFC.Models.Order.Inpatient.OrderExtend> orderExtendList = new List<Neusoft.HISFC.Models.Order.Inpatient.OrderExtend>();
                this.ExecQuery(strSql);
                Neusoft.HISFC.Models.Order.Inpatient.OrderExtend orderExtend;
                while (this.Reader.Read())
                {
                    orderExtend = new Neusoft.HISFC.Models.Order.Inpatient.OrderExtend();
                    orderExtend.InPatientNo = this.Reader[0].ToString();  //住院流水号
                    orderExtend.MoOrder = this.Reader[1].ToString();   //医嘱流水号
                    orderExtend.Extend1 = this.Reader[2].ToString(); //备注1
                    orderExtend.Extend2 = this.Reader[3].ToString();
                    orderExtend.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[4]);
                    orderExtend.Extend3 = this.Reader[5].ToString();
                    orderExtend.Extend4 = this.Reader[6].ToString();

                    orderExtendList.Add(orderExtend);
                }
                return orderExtendList;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (Reader != null)
                {
                    this.Reader.Close();
                }
            }
        }

        #endregion
    }
}
