using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.KangMei;
using Neusoft.FrameWork.Function;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Order
{
    public class KangMei : Neusoft.FrameWork.Management.Database
    {
        #region 订单

        /// <summary>
        /// 序列号
        /// </summary>
        /// <returns></returns>
        public string OrderSeq()
        {
            string sql = "select KM_ORD_SEQ.Nextval from dual";
            return this.ExecSqlReturnOne(sql);
        }


        public int GetKmDrugDept(ref string code)
        {
            try
            {
                string sql = @"select code from com_dictionary a
            where a.type='KMCOOKDEPTCODE' and rownum =1";

                code = this.ExecSqlReturnOne(sql);
            }
            catch (Exception ex)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 按处方号删除订单
        /// </summary>
        /// <param name="recipeNo"></param>
        /// <returns></returns>
        public int OrderDeleteByRecipeNo(string recipeNo)
        {
            string sql = string.Empty;
            string sqlIndex = "HISFC.Compoments.Order.Km.KmAddr.Delete.ByRecipeNo";
            //HISFC.Compoments.Order.Km.KmAddr.Delete.ByReceipeNo
            try
            {
               
                if (this.Sql.GetCommonSql(sqlIndex, ref sql) == -1)
                {
                    this.Err = @"修改康美订单出错！\r\n错误信息：" + sqlIndex;
                    return -1;
                }
                sql = string.Format(sql, recipeNo);
                return this.ExecQuery(sql);
            }
            catch (Exception ex)
            {
                this.Err = @"修改康美订单出错！\r\n错误信息：" + sqlIndex + ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 按订单号删除订单
        /// </summary>
        /// <param name="ordNo"></param>
        /// <param name="dept_code"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int OrderDeleteByOrderNo(string ordNo,string dept_code,string status)
        {
            string sql = string.Empty;
            string sqlIndex = "HISFC.Compoments.Order.Km.KmAddr.Delete.ByOrderNo";
            //HISFC.Compoments.Order.Km.KmAddr.Delete.ByReceipeNo
            try
            {

                if (this.Sql.GetCommonSql(sqlIndex, ref sql) == -1)
                {
                    this.Err = @"修改康美订单出错！\r\n错误信息：" + sqlIndex;
                    return -1;
                }
                sql = string.Format(sql, ordNo,dept_code,status);
                return this.ExecQuery(sql);
            }
            catch (Exception ex)
            {
                this.Err = @"修改康美订单出错！\r\n错误信息：" + sqlIndex + ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 订单新增
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int OrderInsert(KangMeiOrder order)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.Order.Insert", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }


            sql = string.Format(sql, order.ID, order.ClinicCode, order.CardNo, order.PatientName, order.RecipeNo, order.OrderNo, order.Addr,
                order.Addr2, order.Tel, order.Phone, order.Zip, order.Consignee, order.Sex, order.Age, order.OrderDate, order.DrugDeptCode,
                order.State, order.IsSend, order.IsCook, order.Memo, order.Mark, order.Mark2, order.Mark3, order.OperCode);

            return this.ExecNoQuery(sql);

        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int OrderQuery(string sql, ref List<KangMeiOrder> list)
        {
            try
            {

                list = new List<KangMeiOrder>();
                if (this.ExecQuery(sql) == -1)
                {
                    return -1;
                }
                while (this.Reader.Read())
                {

                    KangMeiOrder order = new KangMeiOrder();

                    order.ID = this.Reader[0].ToString(); /*ORD_SEQ[流水号] */
                    order.ClinicCode = this.Reader[1].ToString(); /*CLINIC_CODE[看诊流水号] */
                    order.CardNo = this.Reader[2].ToString(); /*CARD_NO[病历号] */
                    order.PatientName = this.Reader[3].ToString(); /*PATIENT_NAME[病人名称] */
                    order.RecipeNo = this.Reader[4].ToString(); /*RECEIPENO[处方号] */
                    order.OrderNo = this.Reader[5].ToString(); /*ORDNO[订单号] */
                    order.Addr = this.Reader[6].ToString(); /*ADDR[地址] */
                    order.Addr2 = this.Reader[7].ToString(); /*ADDR2[备用地址] */
                    order.Tel = this.Reader[8].ToString(); /*TEL[电话] */
                    order.Phone = this.Reader[9].ToString(); /*PHONE[手机] */
                    order.Zip = this.Reader[10].ToString(); /*ZIP[邮编] */
                    order.Consignee = this.Reader[11].ToString(); /*CONSIGNEE[收货人] */
                    order.Sex = this.Reader[12].ToString(); /*SEX[性别] */
                    order.Age = this.Reader[13].ToString(); /*AGE[年龄] */
                    order.OrderDate = NConvert.ToDateTime(this.Reader[14].ToString()); /*ORD_DATE[订单日期] */
                    order.DrugDeptCode = this.Reader[15].ToString(); /*DRUG_DEPT_CODE[药房编码] */
                    order.State = this.Reader[16].ToString(); /*ORD_STATE[订单状态] */
                    order.IsSend = this.Reader[17].ToString(); /*ISSEND[是否送药] */
                    order.IsCook = this.Reader[18].ToString(); /*ISCOOK[是否煎药] */
                    order.Memo = this.Reader[19].ToString(); /*MEMO[说明] */
                    order.Mark = this.Reader[20].ToString(); /*MARK[备注] */
                    order.Mark2 = this.Reader[21].ToString(); /*MARK2[拓展1] */
                    order.Mark3 = this.Reader[22].ToString(); /*MARK3[拓展2] */
                    order.OperDate = NConvert.ToDateTime(this.Reader[23].ToString()); /*OPER_DATE[操作时间] */
                    order.OperCode = this.Reader[24].ToString(); /*OPER_CODE[操作人] */

                    list.Add(order);

                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;

            }
            return 1;
        }

        /// <summary>
        /// 查询所有订单
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int OrderQueryAll(ref List<KangMeiOrder> list)
        {
            string sql = "";
            list = new List<KangMeiOrder>();

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.Order.Select", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            if (!string.IsNullOrEmpty(sql))
            {
                if (this.OrderQuery(sql, ref list) == -1)
                {
                    return -1;
                }
            }


            return 1;
        }

        /// <summary>
        /// 查询订单，按处方号
        /// </summary>
        /// <param name="receipeNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int OrderQueryByReceipeNo(string receipeNo, ref List<KangMeiOrder> list)
        {

            string sql = "";
            list = new List<KangMeiOrder>();
            string where = " WHERE RECEIPENO='{0}' ";

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.Order.Select", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            where = string.Format(where, receipeNo);
            sql += where;


            if (!string.IsNullOrEmpty(sql))
            {
                if (this.OrderQuery(sql, ref list) == -1)
                {
                    return -1;
                }
            }

            return 1;
        }
        /// <summary>
        /// 查询订单，使用流水号进行查询
        /// </summary>
        /// <param name="receipeNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int OrderQueryByClinicNO(string ClinicNO, ref List<KangMeiOrder> list)
        {

            string sql = "";
            list = new List<KangMeiOrder>();
            string where = " WHERE CLINIC_CODE='{0}' ";

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.Order.Select", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            where = string.Format(where, ClinicNO);
            sql += where;


            if (!string.IsNullOrEmpty(sql))
            {
                if (this.OrderQuery(sql, ref list) == -1)
                {
                    return -1;
                }
            }

            return 1;
        }

        /// <summary>
        /// 查询订单，按订单号
        /// </summary>
        /// <param name="ordNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int OrderQueryByOrderNo(string ordNo, ref List<KangMeiOrder> list)
        {

            string sql = "";
            list = new List<KangMeiOrder>();
            string where = " WHERE ORDNO='{0}' ";

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.Order.Select", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            where = string.Format(where, ordNo);
            sql += where;


            if (!string.IsNullOrEmpty(sql))
            {
                if (this.OrderQuery(sql, ref list) == -1)
                {
                    return -1;
                }
            }


            return 1;
        }

        /// <summary>
        /// 查询订单，按挂号流水号
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int OrderQueryByClinicCode(string clinicCode, ref List<KangMeiOrder> list)
        {

            string sql = string.Empty;
            list = new List<KangMeiOrder>();
            string where = " WHERE  CLINICCODE='{0}' ";

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.Order.Select", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            where = string.Format(where, clinicCode);
            sql += where;


            if (!string.IsNullOrEmpty(sql))
            {
                if (this.OrderQuery(sql, ref list) == -1)
                    return -1;
            }


            return 1;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int OrderUpdate(KangMeiOrder order)
        {
            string sql = string.Empty;

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.Order.Update", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }


            sql = string.Format(sql, order.ID, order.ClinicCode, order.CardNo, order.PatientName, order.RecipeNo, order.OrderNo, order.Addr,
                order.Addr2, order.Tel, order.Phone, order.Zip, order.Consignee, order.Sex, order.Age, order.OrderDate, order.DrugDeptCode,
                order.State, order.IsSend, order.IsCook, order.Memo, order.Mark, order.Mark2, order.Mark3, order.OperCode);

            return this.ExecNoQuery(sql);
        }

#region inpatient

        public int UpdateRecipeNoByOrderConfirm(ArrayList list, ref string errMsg)
        {
            try
            {
                Dictionary<string, Neusoft.HISFC.Models.Order.Inpatient.Order> dic =
                   new Dictionary<string, Neusoft.HISFC.Models.Order.Inpatient.Order>();

                foreach (Neusoft.HISFC.Models.Order.Inpatient.Order item in list)
                    if (item.StockDept.ID == "9092")
                        if (!dic.ContainsKey(item.Combo.ID))
                            dic.Add(item.Combo.ID, item);

                if (dic.Count < 1)
                    return 0;               

                foreach (var item in dic.Values)
                {
                    string sql = @" update km_herbalorder a
                                   set a.receipeno = '{0}', a.ord_state = '1',a.ord_date =sysdate
                                 where a.ordno = '{1}'
                                   and a.ord_state = '0'";
                    sql = string.Format(sql, item.ReciptNO, item.Combo.ID);
                    this.ExecNoQuery(sql);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }
            return 1;
        }



#endregion


        #endregion

        #region 订单地址

        /// <summary>
        /// 取订单地址的流水号
        /// </summary>
        /// <returns></returns>
        public string AddressSeq()
        {
            string sql = "select KM_ORD_ADDR_SEQ.Nextval from dual";
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 地址查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressQuery(string sql, ref List<OrderAddress> list)
        {

            try
            {
                list = new List<OrderAddress>();
                if (this.ExecQuery(sql) == -1)
                {
                    return -1;
                }
                while (this.Reader.Read())
                {
                    OrderAddress item = new OrderAddress();
                    item.ID = this.Reader[0].ToString(); /*ADDR_SEQ[流水号] */
                    item.CardNo = this.Reader[1].ToString(); /*CARD_NO[病历号] */
                    item.PatientName = this.Reader[2].ToString(); /*PATIENT_NAME[姓名] */
                    item.Consignee = this.Reader[3].ToString(); /*CONSIGNEE[收货人姓名] */
                    item.Tel = this.Reader[4].ToString(); /*TEL[电话] */
                    item.Phone = this.Reader[5].ToString(); /*PHONE[手机] */
                    item.Zip = this.Reader[6].ToString(); /*ZIP[邮编] */
                    item.IsVaild = this.Reader[7].ToString(); /*ISVALID[是否有效] */
                    item.IsDefault = this.Reader[8].ToString(); /*ISDEFAULT[是否默认地址] */
                    item.Addr = this.Reader[9].ToString(); /*ADDR[地址] */
                    item.Addr2 = this.Reader[10].ToString(); /*ADDR2[备用地址] */
                    item.Memo = this.Reader[11].ToString(); /*MEMO[说明] */
                    item.Memo = this.Reader[12].ToString(); /*MARK[备注] */
                    item.Mark2 = this.Reader[13].ToString(); /*MARK2[拓展1] */
                    item.Mark3 = this.Reader[14].ToString(); /*MARK3[拓展2] */
                    item.OperDate = NConvert.ToDateTime(this.Reader[15].ToString()); /*OPER_DATE[操作时间] */
                    item.OperCode = this.Reader[16].ToString(); /*OPER_CODE[操作人] */

                    list.Add(item);

                }

            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            return 1;


        }

        /// <summary>
        /// 地址查询，按卡号（门诊号）
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressQueryByCardNo(string cardNo, ref List<OrderAddress> list)
        {

            string sql = string.Empty;
            list = new List<OrderAddress>();
            string where = " AND P.CARD_NO='{0}' ";

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.KmAddr.Select", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            where = string.Format(where, cardNo);
            sql += where;


            if (!string.IsNullOrEmpty(sql))
            {
                if (this.AddressQuery(sql, ref list) == -1)
                    return -1;
            }


            return 1;
        }

        /// <summary>
        /// 取医院住院部地址
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int OrderAddressHospital(ref List<OrderAddress> list)
        {
            string cardNo = "999999999";
            int i = AddressQueryByCardNo(cardNo, ref list);
            return i;
        }

        /// <summary>
        /// 订单送货地址新增
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public int AddressInsert(OrderAddress addr)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.KmAddr.Insert", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }


            sql = string.Format(sql, addr.ID, addr.CardNo, addr.PatientName, addr.Consignee, addr.Tel, addr.Phone, addr.Zip, addr.IsVaild,
                addr.IsDefault, addr.Addr, addr.Addr2, addr.Memo, addr.Mark, addr.Mark2, addr.Mark3, addr.OperCode);

            return this.ExecNoQuery(sql);

        }

        /// <summary>
        /// 订单送货地址修改
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public int AddressUpdate(OrderAddress addr)
        {
            string sql = string.Empty;

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.KmAddr.Update", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            //string where = string.Format(" where addr_seq='{0}'", addr.ID);
            sql = string.Format(sql, addr.ID, addr.CardNo, addr.PatientName, addr.Consignee, addr.Tel, addr.Phone, addr.Zip, addr.IsVaild,
                addr.IsDefault, addr.Addr, addr.Addr2, addr.Memo, addr.Mark, addr.Mark2, addr.Mark3, addr.OperCode);
         //   sql += where;


            return this.ExecNoQuery(sql);
        }


        public int AddressUpdateDefault(OrderAddress addr)
        {
            string sql = @"update km_addr a 
set a.isdefault='0'
where a.card_no='{0}'
and a.addr_seq <>'{1}'
            ";
            sql = string.Format(sql, addr.CardNo, addr.ID);
            return this.ExecNoQuery(sql);
        }

        #endregion

        #region 地址基础库

        /// <summary>
        /// 按唯一编号查询单条基础地址
        /// </summary>
        /// <param name="code"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressBaseQueryByCode(string code,ref List<AddressBase> list)
        {

            string sql = string.Empty;
            try
            {
                if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.AddressBase.Select.1", ref sql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }
                if (!string.IsNullOrEmpty(sql))
                {
                    sql = string.Format(sql, code);
                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            return this.AddressBaseQuery(sql, ref list);

        }

        /// <summary>
        /// 查询基础地址库地址
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressBaseQuery(string sql, ref List<AddressBase> list)
        {
            try
            {
                list = new List<AddressBase>();
                if (this.ExecQuery(sql) == -1)
                {
                    return -1;
                }
                while (this.Reader.Read())
                {

                    AddressBase addr = new AddressBase();
                    addr.CODE = this.Reader[0].ToString(); /*CODE[代码] */
                    addr.NAME = this.Reader[1].ToString(); /*NAME[名称] */
                    addr.ENG_NAME = this.Reader[2].ToString(); /*ENG_NAME[英文名称] */
                    addr.SHORT = this.Reader[3].ToString(); /*SHORT[简写] */
                    addr.SPELL_CODE = this.Reader[4].ToString(); /*SPELL_CODE[拼音码] */
                    addr.WU_CODE = this.Reader[5].ToString(); /*WU_CODE[五笔码] */
                    addr.ZIP = this.Reader[6].ToString(); /*ZIP[邮编] */
                    addr.ZONE = this.Reader[7].ToString(); /*ZONE[区域编号] */
                    addr.TEL_LENGHT = this.Reader[8].ToString(); /*TEL_LENGHT[电话长度] */
                    addr.ZONE2 = this.Reader[9].ToString(); /*ZONE2[电话区号] */
                    addr.PARENTNODE = this.Reader[10].ToString(); /*PARENTNODE[父节点代码] */
                    addr.NODE = this.Reader[11].ToString(); /*NODE[节点代码] */
                    addr.SORTCODE = this.Reader[12].ToString(); /*SORTCODE[排序号] */
                    addr.ISSHOW = this.Reader[13].ToString(); /*ISSHOW[是否显示] */
                    addr.ISVALID = this.Reader[14].ToString(); /*ISVALID[是否有效] */
                    addr.CREATECODE = this.Reader[15].ToString(); /*CREATECODE[创建人] */
                    addr.CREATEDATE = NConvert.ToDateTime(this.Reader[16].ToString()); /*CREATEDATE[创建时间] */
                    addr.OPER_CODE = this.Reader[17].ToString(); /*OPER_CODE[更新人] */
                    addr.OPER_DATE = NConvert.ToDateTime(this.Reader[18].ToString()); /*OPER_DATE[更新时间] */
                    addr.ISHOT = this.Reader[19].ToString(); /*ISHOT[是否热门] */

                    list.Add(addr);

                }
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 查询树结构地址列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressBaseQueryByParentCode(string code, ref List<AddressBase> list)
        {
            string sql = string.Empty;

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.AddressBase.Select.2", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            sql = string.Format(sql, code);
            return this.AddressBaseQuery(sql, ref list);
        }
        /// <summary>
        /// 查询树结构地址列表查询所有地址
        /// </summary>
        /// <param name="code"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressBaseQueryByParentCodeALL(ref List<AddressBase> list)
        {
            string sql = string.Empty;

            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.AddressBase.Select.3", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            sql = string.Format(sql);
            return this.AddressBaseQuery(sql, ref list);
        }
        /// <summary>
        /// 查询所有地址
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddressBaseQueryAll(ref List<AddressBase> list)
        {
            return AddressBaseQueryByParentCode("00", ref list);
        }

        #endregion

        /// <summary>
        /// 自助设备缴费状态修改
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateZZSBJF(string Recipe_No)
        {
            string sql0 = string.Empty;
            string sql1 = string.Empty;
            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.UpdateZZSBJF0", ref sql0) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            if (this.Sql.GetCommonSql("HISFC.Compoments.Order.Km.UpdateZZSBJF1", ref sql1) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            sql0 = string.Format(sql0, Recipe_No);
            sql1 = string.Format(sql1, Recipe_No);
            this.ExecNoQuery(sql0);
            return this.ExecNoQuery(sql1);
        }
    }
}
