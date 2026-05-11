using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Fee
{
    /// <summary>
    /// [功能描述: 实际发票管理类]<br></br>
    /// [创 建 者: maokb]<br></br>
    /// [创建时间: 2008-09]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public class RealInvoice : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 实际发票管理类
        /// </summary>
        public RealInvoice()
        {
        }

        #region 内部方法

        /// <summary>
        /// 更新插入删除单个表中数据        
        /// <param name="sqlIndex"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        /// </summary>
        private int UpdateSingleTable(string sqlIndex, params string[] parms)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql(sqlIndex, ref strSql) == -1)
            {
                this.Err = "没有找到Sql " + sqlIndex;
                return -1;
            }

            return this.ExecNoQuery(strSql, parms);
        }

        /// <summary>
        /// 根据实体转换成参数，操作时间无须赋值
        /// </summary>
        /// <param name="rInvoice"></param>
        /// <returns></returns>
        private string[] GetParams(Neusoft.HISFC.Models.Fee.InvoiceExtend rInvoice)
        {
            string[] strParams = new string[]{rInvoice.ID, //发票流水号
                                   rInvoice.InvoiceType,//发票类型
                                    rInvoice.RealInvoiceNo,//实际发票号
                                   rInvoice.InvvoiceHead,//发票号字母头
                                   "1", //插入时都是有效发票
                                   this.Operator.ID
            };
            return strParams;
        }

        protected Neusoft.HISFC.BizLogic.Fee.InvoiceServiceNoEnum invoiceServiceManager = new InvoiceServiceNoEnum();

        /// <summary>
        /// 获取操作员当前使用的发票号
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public string GetUsingInvoiceNO(string invoiceType)
        {
            ArrayList al = this.invoiceServiceManager.QueryInvoices(this.invoiceServiceManager.Operator.ID, invoiceType);
            if (al == null)
            {
                this.Err = this.invoiceServiceManager.Err;
                return "";
            }
            if (al.Count == 0)
            {
                return "";
            }
            foreach (Neusoft.HISFC.Models.Fee.Invoice invoiceObj in al)
            {
                if (invoiceObj.ValidState == "1")
                {
                    return invoiceObj.UsedNO;
                }
            }
            return "";
        }

        /// <summary>
        /// 查询基本函数
        /// </summary>
        /// <param name="whereIndex">“”为没有where条件</param>
        /// <param name="param"></param>
        /// <returns></returns>
        private List<Neusoft.HISFC.Models.Fee.InvoiceExtend> QueryRealInvoiceBase(string whereIndex, params string[] param)
        {
            //定义获取查询的主SQL语句
            string strSql = "";
            if (this.Sql.GetCommonSql("Fee.RealInvoice.QueryRealInvoiceBase", ref strSql) == -1)
            {
                this.Err = "没有找到Sql  Fee.RealInvoice.QueryRealInvoiceBase";
                return null;
            }

            //定义获取查询的where条件
            string strWhere = "";
            if (whereIndex != string.Empty)
            {
                if (this.Sql.GetCommonSql(whereIndex, ref strWhere) == -1)
                {
                    this.Err = "没有找到Sql " + whereIndex;
                    return null;
                }
            }

            //sql语句赋值，查询
            try
            {
                strSql = string.Format(strSql + "\n" + strWhere, param);
                if (this.ExecQuery(strSql) == -1)
                {
                    return null;
                }
                //定义列表，循环把结果存入列表
                List<Neusoft.HISFC.Models.Fee.InvoiceExtend> lsInvoice = new List<Neusoft.HISFC.Models.Fee.InvoiceExtend>();
                while (Reader.Read())
                {
                    Neusoft.HISFC.Models.Fee.InvoiceExtend rInvoice = new Neusoft.HISFC.Models.Fee.InvoiceExtend();
                    rInvoice.ID = this.Reader[0].ToString();
                    rInvoice.InvoiceType = this.Reader[1].ToString();
                    rInvoice.RealInvoiceNo = this.Reader[2].ToString();
                    rInvoice.InvvoiceHead = this.Reader[3].ToString();
                    rInvoice.InvoiceState = this.Reader[4].ToString();
                    rInvoice.WasteOper.ID = this.Reader[5].ToString();
                    rInvoice.WasteOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6]);
                    rInvoice.Oper.ID = this.Reader[7].ToString();
                    rInvoice.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[8]);

                    lsInvoice.Add(rInvoice);
                }
                return lsInvoice;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return null;
            }
            finally
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }

        #endregion

        #region 公有方法 增加、修改、查询

        /// <summary>
        /// 插入发票项目
        /// </summary>
        /// <param name="menu">菜谱项目实体</param>
        /// <returns>-1失败, 1成功</returns>
        public int InsertRealInvoice(Neusoft.HISFC.Models.Fee.InvoiceExtend rInvoice)
        {
            return this.UpdateSingleTable("Fee.RealInvoice.InsertRealInvoice", this.GetParams(rInvoice));
        }

        /// <summary>
        /// 根据发票ID,更新发票
        /// </summary>
        /// <param name="menu">菜谱实体</param>
        /// <returns>-1失败, 1成功</returns>
        public int UpdateRealInvoice(Neusoft.HISFC.Models.Fee.InvoiceExtend rInvoice)
        {
            return this.UpdateSingleTable("Fee.RealInvoice.UpdateRealInvoice", this.GetParams(rInvoice));
        }

        /// <summary>
        /// 查询所有发票
        /// </summary>
        /// <returns>null失败</returns>
        public List<Neusoft.HISFC.Models.Fee.InvoiceExtend> QueryAll()
        {
            //查询所有菜谱明细，没有where条件
            return this.QueryRealInvoiceBase("");
        }

        /// <summary>
        /// 查询发票段
        /// {E535C33C-6A96-4259-8550-DBEFF4D87EC1}
        /// </summary>
        /// <returns>null失败</returns>
        public List<Neusoft.HISFC.Models.Fee.InvoiceExtend> QueryByBeginEndID(string beginNO, string endNO, string invoiceType)
        {
            //查询所有菜谱明细，没有where条件.修改gmz.只能查询某个收费员的一段发票号
            return this.QueryRealInvoiceBase("Fee.RealInvoice.QueryByBeginEndID", beginNO, endNO, invoiceType, this.Operator.ID);
        }

        /// <summary>
        /// 查询一条实际发票号
        /// {E535C33C-6A96-4259-8550-DBEFF4D87EC1}
        /// </summary>
        /// <param name="invoiceNO">电脑号</param>
        /// <param name="invoiceType">发票类型</param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Fee.InvoiceExtend GetRealInvoice(string invoiceNO, string invoiceType)
        {
            List<Neusoft.HISFC.Models.Fee.InvoiceExtend> list = this.QueryRealInvoiceBase("Fee.RealInvoice.GetOneRealInvoice", invoiceNO, invoiceType);

            if (list == null || list.Count == 0)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        /// <summary>
        /// 获取收款员下一张实际发票号
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="realInvoiceNo"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public int QueryNextRealInvoiceNo(Neusoft.FrameWork.Models.NeuObject Obj, ref string realInvoiceNo)
        {
            string sqlQuery = string.Empty;

            //取SELECT语句
            if (this.Sql.GetCommonSql("Fee.GetNextRealInvoice.Query", ref sqlQuery) == -1)
            {
                this.Err = "没有找到Fee.GetNextRealInvoice.Query字段!";
                this.WriteErr();

                return -1;
            }
            //格式化SQL语句
            try
            {
                sqlQuery = string.Format(sqlQuery, Obj.ID, Obj.Name);
                this.ExecQuery(sqlQuery);
            }
            catch (Exception e)
            {
                this.Err = e.Message;
                this.WriteErr();

                return -1;
            }
            try
            {
                while (this.Reader.Read())
                {
                    realInvoiceNo = this.Reader[0].ToString();
                }
            }
            catch (Exception ex)
            {
                this.Err += ex.Message;
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            finally
            {
                if (!this.Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
            return 1;
        }

        /// <summary>
        /// 更新实际发票号表
        /// </summary>
        /// <param name="ID">流水号</param>
        /// <param name="invoiceType">发票类型</param>
        /// <param name="wasteFlag">作废标志</param>
        /// <returns></returns>
        public int UpdateRealInvoice(string ID, string invoiceType, string realInvoice, string wasteFlag)
        {
            if (wasteFlag == "1")
            {
                string[] parm = new string[] { ID, invoiceType, realInvoice, wasteFlag, "", DateTime.MinValue.ToString() };
                return this.UpdateSingleTable("Fee.UpdateSingleRealInvoice", parm);

            }
            else
            {
                string[] parm = new string[] { ID, invoiceType, realInvoice, wasteFlag, this.Operator.ID, this.GetSysDateTime() };
                return this.UpdateSingleTable("Fee.UpdateSingleRealInvoice", parm);
            }
        }

        /// <summary>
        /// 获取下一张实际发票号
        /// </summary>
        /// <param name="operID"></param>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public string GetNextRealInvoiceNO(string operID, string invoiceType)
        {
            this.ExecQuery("Fee.GetNextRealInvoice", operID, invoiceType);
            string realInvoice = "-1";
            while (this.Reader.Read())
            {
                realInvoice = this.Reader[0].ToString();
            }
            this.Reader.Close();
            return realInvoice;
        }

        /// <summary>
        /// 根据物理发票号查找是否存在此记录
        /// </summary>
        /// <param name="invoiceNO">电脑号</param>
        /// <param name="invoiceType">发票类型</param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Fee.InvoiceExtend GetInoviceByRealInvoice(string realInvoiceNO, string invoiceType)
        {
            List<Neusoft.HISFC.Models.Fee.InvoiceExtend> list = this.QueryRealInvoiceBase("Fee.RealInvoice.GetRealInvoice.Where.1", invoiceType, realInvoiceNO);

            if (list == null || list.Count == 0)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        /// <summary>
        /// 根据发票电脑号更新fin_opb_invoiceinfo中的印刷号
        /// </summary>
        /// <param name="invoiceNO"></param>
        /// <param name="realInvoice"></param>
        /// <returns></returns>
        public int UpdateInvoiceInfoRealInvoice(string invoiceNO, string realInvoice)
        {
            string strSQL = string.Empty;

            if (this.Sql.GetCommonSql("Neusoft.Fee.InvoiceInfo.UpdatePrintInvoice", ref strSQL) == -1)
            {
                this.Err = "没有找到SQLID为Neusoft.Fee.InvoiceInfo.UpdatePrintInvoice的SQL语句!";
                return -1;
            }

            //发票基本信息表中，realInvoice有12位
            //realInvoice = realInvoice.PadLeft(12, '0');

            return this.ExecNoQuery(strSQL, invoiceNO, realInvoice);
        }

        /// <summary>
        /// 根据发票电脑号更新FIN_OPB_ACCOUNTCARDFEE中的印刷号
        /// </summary>
        /// <param name="invoiceNO"></param>
        /// <param name="realInvoice"></param>
        /// <returns></returns>
        public int UpdateInvoiceInfoRealInvoiceForAccountCardFee(string invoiceNO, string realInvoice)
        {
            string strSQL = string.Empty;

            if (this.Sql.GetCommonSql("Neusoft.Fee.InvoiceInfo.UpdatePrintInvoiceForAccountCardFee", ref strSQL) == -1)
            {
                this.Err = "没有找到SQLID为Neusoft.Fee.InvoiceInfo.UpdatePrintInvoiceForAccountCardFee的SQL语句!";
                return -1;
            }

            //发票基本信息表中，realInvoice有12位
            //realInvoice = realInvoice.PadLeft(12, '0');

            return this.ExecNoQuery(strSQL, invoiceNO, realInvoice);
        }

        #endregion
    }
}
