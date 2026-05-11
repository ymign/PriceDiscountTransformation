using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.HISFC.Models.Order;

namespace Neusoft.HISFC.BizLogic.Order
{
    /// <summary>
    /// 用法带出维护
    /// </summary>
    public class SubtblManager : Neusoft.FrameWork.Management.Database
    {
        #region 院注维护

        /// <summary>
        /// 获得对象参数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected string[] myGetParmSubtblInfo(OrderSubtblNew obj)
        {
            string[] strParm ={	
                               obj.Area,
                               obj.TypeCode,
                               obj.Dept_code,
                               obj.Item.ID,
                               obj.Qty.ToString(),
                               obj.CombArea,
                               obj.FeeRule,
                               obj.LimitType,
                               obj.FirstFeeFlag,
                               obj.Oper.ID,
                               obj.OperDate.ToString(),
                               obj.OrderType,
                               Neusoft.FrameWork.Function.NConvert.ToInt32(obj.IsAllowReFee).ToString(),
                               Neusoft.FrameWork.Function.NConvert.ToInt32(obj.IsAllowPopChose).ToString(),
                               Neusoft.FrameWork.Function.NConvert.ToInt32(obj.IsCalculateByOnceDose).ToString(),
                               obj.DoseUnit,
                               obj.OnceDoseFrom.ToString(),
                               obj.OnceDoseTo.ToString(),
                               obj.Extend1,
                               obj.Extend2,
                               obj.Extend3
							 };

            return strParm;

        }

        /// <summary>
        /// 根据用法删除所有项目信息
        /// </summary>
        /// <param name="usage"></param>
        /// <returns></returns>
        public int DelSubtblInfo(string usage)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Met.Com.DelSubtblInfo.DelAllByUsage", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, usage);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 删除用法项目信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int DelSubtblInfo(OrderSubtblNew obj)
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Met.Com.DelSubtblInfo.Del", ref strSql) == -1) return -1;
            try
            {
                strSql = string.Format(strSql, obj.Area, obj.OrderType, obj.TypeCode, obj.Dept_code, obj.Item.ID);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 插入用法项目信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertSubtblInfo(OrderSubtblNew obj)
        {
            string sql = string.Empty;
            string[] strParam;

            if (this.Sql.GetCommonSql("Met.Com.InsertSubtblInfo.Insert", ref sql) == -1)
            {
                this.Err = "没有找到字段!";
                return -1;
            }
            try
            {
                if (obj.ID == null)
                    return -1;
                strParam = this.myGetParmSubtblInfo(obj);
            }
            catch (Exception ex)
            {
                this.Err = "格式化SQL语句时出错[Met.Com.InsertSubtblInfo.Insert]:" + ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(sql, strParam);
        }

        /// <summary>
        /// 获得用法项目信息sql语句
        /// </summary>
        /// <returns></returns>
        public string GetSqlSubtbl()
        {
            string strSql = "";
            if (this.Sql.GetCommonSql("Met.Com.GetSqlSubtbl.Select", ref strSql) == -1)
            {
                return null;
            }
            return strSql;
        }

        /// <summary>
        /// 获取附材项目
        /// </summary>
        /// <param name="area">适用范围：0 门诊；1 住院；3 全部</param>
        /// <param name="typeCode">用法分类，0 药品按用法，1 非药品按项目代码</param>
        /// <param name="deptCode">科室代码，全院统一附材'ROOT'</param>
        /// <returns></returns>
        public ArrayList GetSubtblInfo(string area, string typeCode, string deptCode)
        {
            string strSql1 = "";
            string strSql2 = "";
            //获得项目明细的SQL语句
            strSql1 = this.GetSqlSubtbl();
            if (this.Sql.GetCommonSql("Met.Com.GetSqlSubtbl.Where1", ref strSql2) == -1) return null;
            strSql1 = strSql1 + " " + strSql2;
            try
            {
                strSql1 = string.Format(strSql1, area, typeCode, deptCode);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return null;
            }
            return this.MyGetSubtblInfo(strSql1);
        }

        /// <summary>
        /// 获取附材项目
        /// </summary>
        /// <param name="area">适用范围：0 门诊；1 住院；3 全部</param>
        /// <param name="typeCode">用法分类，0 药品按用法，1 非药品按项目代码</param>
        /// <param name="deptCode">科室代码，全院统一附材'ROOT'</param>
        /// <returns></returns>
        public ArrayList GetSubtblInfoByItem(string area, string deptCode, string itemCode, string usageCode)
        {
            string strSql1 = "";
            string strSql2 = @"where (area='{0}' or area='3')
                                    and (type_code='{1}' or type_code='{2}')
                                    and (dept_code='{3}' or dept_code='ROOT')";
            //获得项目明细的SQL语句
            strSql1 = this.GetSqlSubtbl();
            //if (this.Sql.GetCommonSql("Met.Com.GetSqlSubtbl.Where1", ref strSql2) == -1) return null;
            strSql1 = strSql1 + " \r\n" + strSql2;
            try
            {
                strSql1 = string.Format(strSql1, area, itemCode, usageCode, deptCode);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return null;
            }
            return this.MyGetSubtblInfo(strSql1);
        }

        /// <summary>
        /// 获取附材信息
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private ArrayList MyGetSubtblInfo(string strSql)
        {
            ArrayList al = new ArrayList();
            OrderSubtblNew obj;
            this.ExecQuery(strSql);
            while (this.Reader.Read())
            {
                obj = new OrderSubtblNew();
                try
                {
                    obj.Area = this.Reader[0].ToString();//适用范围：0 门诊；1 住院；3 全部                    
                    obj.TypeCode = this.Reader[1].ToString();//用法分类，0 药品按用法，1 非药品按项目代码
                    obj.Dept_code = this.Reader[2].ToString();//科室代码，全院统一附材'ROOT'
                    obj.Item.ID = this.Reader[3].ToString();//附加项目编码
                    obj.Qty = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[4].ToString());//数量
                    obj.CombArea = this.Reader[5].ToString();//组范围：0 每组收取、1 第一组收取、2 第二组起加收
                    obj.FeeRule = this.Reader[6].ToString();//收费规则：固定数量、*最大院注、*组内品种数、*最大医嘱数量、*频次
                    obj.LimitType = this.Reader[7].ToString();//限制类别：0 不限制 1 婴儿使用 2 非婴儿使用
                    obj.FirstFeeFlag = this.Reader[8].ToString();//首次收取项目
                    obj.Oper.ID = this.Reader[9].ToString();//操作员
                    obj.OperDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[10].ToString());//操作时间
                    obj.OrderType = this.Reader[11].ToString(); //医嘱类别：全部、长嘱、临嘱

                    obj.IsAllowReFee = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[12]);
                    obj.IsAllowPopChose = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[13]);
                    obj.IsCalculateByOnceDose = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[14]);
                    obj.DoseUnit = this.Reader[15].ToString();
                    obj.OnceDoseFrom = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[16]);
                    obj.OnceDoseTo = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[17]);
                    if (this.Reader.FieldCount > 18)
                    {
                        obj.Extend1 = this.Reader[18].ToString();
                    }

                    if (this.Reader.FieldCount > 19)
                    {
                        obj.Extend2 = this.Reader[19].ToString();
                    }

                    if (this.Reader.FieldCount > 20)
                    {
                        obj.Extend3 = this.Reader[20].ToString();
                    }
                }
                catch (Exception ex)
                {
                    this.Err = "查询明细赋值错误" + ex.Message;
                    this.ErrCode = ex.Message;
                    this.WriteErr();
                    return null;
                }
                al.Add(obj);
            }
            this.Reader.Close();
            return al;
        }

        /// <summary>
        /// 获得所有附材信息
        /// </summary>
        /// <returns></returns>
        public ArrayList GetAllSubInfo()
        {
            //获得项目明细的SQL语句
            string strSql1 = this.GetSqlSubtbl();
            return this.MyGetSubtblInfo(strSql1);
        }

        /// <summary>
        /// 根据科室名称获取当前科室已经维护了的用法类型
        /// </summary>
        /// <param name="DeptCode"></param>
        /// <returns></returns>
        public List<string> GetAllUsageTypeByDeptCode(string DeptCode)
        {
            this.ErrCode = "";
            this.Err = "";
            string strSql = "select distinct type_code from met_com_subtblitem m where dept_code = '{0}'";
            try
            {
                strSql = string.Format(strSql, DeptCode);
                this.ExecQuery(strSql);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return null;
            }
            this.ExecQuery(strSql);
            try
            {
                List<string> strList = new List<string>();
                while (this.Reader.Read())
                {
                    strList.Add(this.Reader[0].ToString());
                }
                return strList;
            }
            catch (Exception ex)
            {

                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 判断工号是否为 付材维护是否可看到全院住院的维护情况的管理员
        /// </summary>
        /// <param name="EmplId"></param>
        /// <returns></returns>
        public bool IsZyManager(string EmplId)
        {
            this.ErrCode = "";
            this.Err = "";
            string strSql = "select * from com_dictionary d where Type = 'ZyManager' and valid_state = '1' and Code ='{0}' ";
            System.Data.DataSet DataSet = null;
            try
            {
                strSql = string.Format(strSql, EmplId);
                this.ExecQuery(strSql,ref DataSet);
                return DataSet != null && DataSet.Tables.Count > 0 && DataSet.Tables[0].Rows.Count > 0;
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = ex.Message;
                return false;
            }
        }
        #endregion
    }
}
