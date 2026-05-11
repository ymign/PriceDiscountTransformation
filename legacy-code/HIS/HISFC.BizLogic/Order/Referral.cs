using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Order
{
    /// <summary>
    /// 转诊
    /// </summary>
    public class Referral : Neusoft.FrameWork.Management.Database
    {
        public Referral()
        {
 
        }

        /// <summary>
        /// 插入一条
        /// </summary>
        /// <param name="referralInfo"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.Referral.ReferralInfo referralInfo)
        {
            if (Select(referralInfo.ClinicCode) != null)
            {
                return Update(referralInfo);
            }
            string sql = @"INSERT INTO met_ord_Referral(
clinic_code, --门诊号
name, --姓名
sex, --性别
age, --年龄
deptname, --开单科室名称
deptcode, --开单科室ID
cardno, --就诊卡号
phone, --电话
diagnosis, --门诊诊断
proposal, --建议
unitname,--下转单位
doctName, --开单医生姓名
doctCode --开单医生工号
)values
('{0}',
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
'{12}')

";
            Object[] s = { 
                                    referralInfo.ClinicCode,
                                    referralInfo.Name,
                                    referralInfo.Sex,
                                    referralInfo.Age,
                                    referralInfo.DeptName,
                                    referralInfo.DeptCode,
                                    referralInfo.CardNo,
                                    referralInfo.Phone,
                                    referralInfo.Diagnosis,
                                    referralInfo.Proposal,
                                    referralInfo.UnitName,
                                    referralInfo.DoctName,
                                    referralInfo.DoctCode
                                };
            sql = string.Format(sql, s);
            if (this.ExecNoQuery(sql) <= 0) return -1;
            return 0;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="Clinic_code"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Referral.ReferralInfo Select(string Clinic_code)
        {
            Neusoft.HISFC.Models.Referral.ReferralInfo referralInfo = null;
            string sql = @"select clinic_code, --门诊号
name, --姓名
sex, --性别
age, --年龄
deptname, --开单医生姓名
deptcode, --开单医生工号
cardno, --就诊卡号
phone, --电话
diagnosis, --门诊诊断
proposal, --建议
unitname,--下转单位
doctName, --开单医生姓名
doctCode, --开单医生工号
operDate
 from met_ord_Referral m where clinic_code = '{0}'
";
            sql = string.Format(sql, Clinic_code);
            DataSet ds = null;
            if (this.ExecQuery(sql, ref ds) <= 0) return null;
            if (ds != null && ds.Tables.Count != 0)
            {
                if(ds.Tables[0].Rows.Count!=0)
                {
                    referralInfo = new Neusoft.HISFC.Models.Referral.ReferralInfo();
                    referralInfo.ClinicCode = ds.Tables[0].Rows[0][0].ToString();
                    referralInfo.Name = ds.Tables[0].Rows[0][1].ToString();
                    referralInfo.Sex = ds.Tables[0].Rows[0][2].ToString();
                    referralInfo.Age = ds.Tables[0].Rows[0][3].ToString();
                    referralInfo.DeptName = ds.Tables[0].Rows[0][4].ToString();
                    referralInfo.DeptCode = ds.Tables[0].Rows[0][5].ToString();
                    referralInfo.CardNo = ds.Tables[0].Rows[0][6].ToString();
                    referralInfo.Phone = ds.Tables[0].Rows[0][7].ToString();
                    referralInfo.Diagnosis = ds.Tables[0].Rows[0][8].ToString();
                    referralInfo.Proposal = ds.Tables[0].Rows[0][9].ToString();
                    referralInfo.UnitName = ds.Tables[0].Rows[0][10].ToString();
                    referralInfo.DoctName = ds.Tables[0].Rows[0][11].ToString();
                    referralInfo.DoctCode = ds.Tables[0].Rows[0][12].ToString();
                    referralInfo.OperDate = (DateTime)ds.Tables[0].Rows[0][13];
                }
            }
            return referralInfo;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="referralInfo"></param>
        /// <returns></returns>
        public int Update(Neusoft.HISFC.Models.Referral.ReferralInfo  referralInfo)
        {
            if (this.Delete(referralInfo.ClinicCode) < 0)
            {
                return -1;//删除不成功
            }
            return this.Insert(referralInfo);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ClinicCode"></param>
        /// <returns></returns>
        public int Delete(string ClinicCode)
        {
            string sql = @"Delete met_ord_Referral where clinic_code = '{0}'";
            try
            {
                sql = string.Format(sql, ClinicCode);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(sql);
        }
    }
}
