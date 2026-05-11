using System;
using System.Collections;
using Neusoft.HISFC.Models;
using Neusoft.FrameWork.Models;
using Neusoft.HISFC.Models;
using Neusoft.HISFC.Models.Pharmacy;

namespace Neusoft.HISFC.BizLogic.Pharmacy
{
    /// <summary>
    /// 中选药品管理类
    /// </summary>

    public class  PCContro : Neusoft.FrameWork.Management.Database
    {
        public PCContro()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public ArrayList QueryPCControList()
        {
            string sql = @"   select a.item_code,a.item_name,a.item1_code,a.item2_code,a.dept_code,spell_code,wb_code,valid_state,mark,oper_code,oper_date,mark1,mark2,mark3,mark4
            from PHA_COM_PCCONTRO a where a.valid_state='1'";
            ArrayList list = this.GetPCControList(sql);
            if (list == null || list.Count == 0) return null;
            return list;
        }

        public ArrayList QueryPCControListAll()
        {
            string sql = @"   select a.item_code,a.item_name,a.item1_code,a.item2_code,a.dept_code,spell_code,wb_code,valid_state,mark,oper_code,oper_date,mark1,mark2,mark3,mark4
            from PHA_COM_PCCONTRO a ";
            ArrayList list = this.GetPCControList(sql);
            if (list == null || list.Count == 0) return null;
            return list;
        }


        public ArrayList QueryDrugList()
        {
            string sql = @"select a.drug_code,a.trade_name,a.spell_code,a.wb_code,a.valid_state from pha_com_baseinfo a
                          where a.valid_state='1' ";
            ArrayList list = this.GetDrugList(sql);
            if (list == null || list.Count == 0) return null;
            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 通过非采购药获取实体
        /// </summary>
        /// <param name="itemcode1"></param>
        /// <returns></returns>
        public PcContro QueryPCControList(string itemcode1)
        {
            string sql = @"   select a.item_code,a.item_name,a.item1_code,a.item2_code,a.dept_code,spell_code,wb_code,valid_state,mark,oper_code,oper_date,mark1,mark2,mark3,mark4
            from PHA_COM_PCCONTRO a where a.valid_state='1' and a.item1_code= '{0}'";
            sql = string.Format(sql, itemcode1);
            ArrayList al = new ArrayList(); 
            PcContro PcControtemp = new PcContro();
            try
            {
                al= this.GetPCControList(sql); ;
                if (al.Count == 0) return PcControtemp;
                PcControtemp = (PcContro)al[0];
            }
            catch (Exception ee)
            {
                string Error = ee.Message;
                return null;
            }
            return PcControtemp;
        }

        /// <summary>
        /// 指引单检验地址对照列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList GetPCControList(string sql)
        {

            if (this.ExecQuery(sql) == -1) return null;
            ArrayList list = new ArrayList();
            while (this.Reader.Read())
            {
                Neusoft.HISFC.Models.Pharmacy.PcContro item = new  Neusoft.HISFC.Models.Pharmacy.PcContro();

                if (!Reader.IsDBNull(0))
                item.ItemCode = this.Reader[0].ToString();
                if (!Reader.IsDBNull(1))
                item.ItemName = this.Reader[1].ToString();
                if (!Reader.IsDBNull(2))
                item.Itemcode1 = this.Reader[2].ToString();
                if (!Reader.IsDBNull(3))
                item.Itemcode2 = this.Reader[3].ToString();
                if (!Reader.IsDBNull(4))
                item.Deptcode = this.Reader[4].ToString();

                if (!Reader.IsDBNull(5))
                item.SpellCode = this.Reader[5].ToString();
                if (!Reader.IsDBNull(6))
                item.Wb_code = this.Reader[6].ToString();
                item.Valid_State = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[7].ToString());
                if (!Reader.IsDBNull(8))
                item.Mark = this.Reader[8].ToString();
                if (!Reader.IsDBNull(9))
                item.OperCode = this.Reader[9].ToString();

                if (!Reader.IsDBNull(10))
                item.OperDate = Convert.ToDateTime(this.Reader[10].ToString());

                if (!Reader.IsDBNull(11))
                item.Mark1 = this.Reader[11].ToString();
                if (!Reader.IsDBNull(12))
                item.Mark2 = this.Reader[12].ToString();
                if (!Reader.IsDBNull(13))
                item.Mark3 = this.Reader[13].ToString();
                if (!Reader.IsDBNull(14))
                item.Mark4 = this.Reader[14].ToString();
                list.Add(item);
            }
            return list;

        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int DelPCContro(Neusoft.HISFC.Models.Pharmacy.PcContro item, ref string errMsg)
        {
            try
            {
                string sql = "delete from Pha_Com_Pccontro where ITEM_CODE='{0}'";
                sql = string.Format(sql, item.ItemCode);
                return this.ExecNoQuery(sql);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return -1;
            }

        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int ModefyPCContro(Neusoft.HISFC.Models.Pharmacy.PcContro item, ref string errMsg)
        {
            try
            {
                string sql = @"
 update Pha_Com_Pccontro a
 set a.item1_code='{2}',
     a.item2_code='{3}',
     a.dept_code='{4}',  
     a.spell_code='{5}',
     a.wb_code='{6}',
     a.valid_state='{7}',
     a.oper_date=sysdate,
     a.oper_code='{8}'
     where a.item_code='{0}'

";

                sql = string.Format(sql, item.ItemCode, item.ItemName, item.Itemcode1, item.Itemcode2, item.Deptcode, item.SpellCode, item.Wb_code,
                    item.Valid_State == true ? "1" : "0", item.OperCode);
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
        /// 新增数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int AddPCContro(Neusoft.HISFC.Models.Pharmacy.PcContro item, ref string errMsg)
        {
            try
            {
                string sql = @" insert into Pha_Com_Pccontro
 (item_code,item_name,item1_code,item2_code,dept_code,spell_code,wb_code,valid_state,oper_code,oper_date)
 values ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',to_date('{9}','yyyy-mm-dd hh24:mi:ss')) ";
                sql = string.Format(sql, item.ItemCode, item.ItemName, item.Itemcode1, item.Itemcode2, item.Deptcode, item.SpellCode, item.Wb_code,
                    item.Valid_State==true?"1":"0", item.OperCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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


    }
}
