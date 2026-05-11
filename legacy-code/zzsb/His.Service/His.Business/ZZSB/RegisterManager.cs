using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace His.Business.ZZSB
{
    internal class RegisterManager : Shadow.Util.Data.Management.OracleBase
    {

        public static string OPERID = "00W999";
        #region 看诊序号

        /// <summary>
        /// 查找当前看诊序号
        /// </summary>
        /// <param name="schemaNo"></param>
        /// <param name="seeNo"></param>
        /// <returns></returns>
        public int GetCurrentSeeNo(string schemaNo, ref int seeNo)
        {
            string str = "Registration.Register.SeeNo.Current";
            string sql = string.Empty, No = string.Empty;
            if (this.GetSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                seeNo = Shadow.Util.Data.Func.NConvert.ToInt32(No);
            }
            catch (Exception ex)
            {
                this.Err = "查找当前看诊序号出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 查找最小看诊序号
        /// </summary>
        /// <param name="schemaNo"></param>
        /// <param name="minNo"></param>
        /// <returns></returns>
        public int GetMinSeeNo(string schemaNo, ref int minNo)
        {
            //Registration.Register.SeeNo.Begin.1
            string str = "Registration.Register.SeeNo.Begin.1";
            string sql = string.Empty, No = string.Empty;
            if (this.GetSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                minNo = Shadow.Util.Data.Func.NConvert.ToInt32(No);
            }
            catch (Exception ex)
            {
                this.Err = "查找最小看诊序号出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }


        /// <summary>
        /// 取排班的号源总额数
        /// </summary>
        /// <param name="schemaNo"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public int GetSourceCount(string schemaNo, ref int cnt)
        {
            string sql = @"select nvl(sum(a.tel_lmt+a.reg_lmt+a.spe_lmt ),-1) cnt
                        from fin_opr_schema a
                        where a.id='{0}' ";
            string No = string.Empty;
          
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                cnt = Shadow.Util.Data.Func.NConvert.ToInt32(No);
                if (cnt == -1)
                {
                    this.Err = "没有找到相关行数！";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Err = "查找号源数量出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }
        #endregion
        /// <summary>
        /// 查找有效挂号数
        /// </summary>
        /// <param name="schemaNo"></param>
        /// <param name="Residue"></param>
        /// <returns></returns>
        public int GetResidue(string schemaNo, ref int Residue)
        {
            //Registration.Register.Residue
            string str = "Registration.Register.Residue";
            string sql = string.Empty, No = string.Empty;
            if (this.GetSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                Residue = Neusoft.FrameWork.Function.NConvert.ToInt32(No);
            }
            catch (Exception ex)
            {
                this.Err = "查找有效挂号数出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// 查询 返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            DataSet ds = new DataSet();
            if (this.ExecQuery(sql, ref ds) == -1)
            {
                return null;
            }
            return ds.Tables[0];

        }


        /// <summary>
        /// 执行单条sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql, ref string errMsg)
        {
            int i=this.ExecNoQuery(sql);
            errMsg = Err;
            return i;
        }


        public Neusoft.FrameWork.Models.NeuObject GetConstant(string type, string ID)
        {
            string sql = @"select * from com_dictionary a
                      where a.type='{0}'
                      and a.code='{1}'";
            sql = string.Format(sql, type, ID);
            Neusoft.FrameWork.Models.NeuObject obj=new Neusoft.FrameWork.Models.NeuObject();

            try
            {
                if (this.ExecQuery(sql) == -1)
                    return null;
                while (this.Reader.Read())
                {
                    obj.ID = Reader[1].ToString();
                    obj.Name = Reader[2].ToString();
                    obj.Memo = Reader[3].ToString();
                    obj.User01 = Reader[0].ToString();
                    obj.User02 = Reader[6].ToString();
                    obj.User03 = Reader[8].ToString();
                    
                }
                this.Reader.Close();
                return obj;
            }
            catch (Exception ex)
            {
                this.Reader.Close();
                return null;
            }


        }
        public string GetAge14LimitDept(string deptcode)
        {
            string sql = @"SELECT p.dept_code FROM com_department p WHERE p.bro_name='内科' AND p.dept_type='C' AND dept_code='{0}'";
            sql = string.Format(sql, deptcode);
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 生成预约流水号
        /// </summary>
        /// <returns></returns>
        public int GetBookSerialNo()
        {
            string sql = "select SEQ_FIN_BOOKING.Nextval from dual ";
            return Shadow.Util.Data.Func.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
        }
    }
}
