using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Registration
{
    /// <summary>
    /// 朝阳居民医保错误数据处理类
    /// </summary>
    public class ChaoYangDataDeal : Neusoft.FrameWork.Management.Database
    {
        #region 朝阳居民医保错误数据处理

        public ArrayList QueryRegister(string sql)
        {
            if (this.ExecQuery(sql) == -1) return null;

            ArrayList al = new ArrayList();

            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Registration.Register reg = new Neusoft.HISFC.Models.Registration.Register();

                    reg.ID = this.Reader[0].ToString();//序号
                    reg.PID.CardNO = this.Reader[1].ToString();//病历号
                    reg.DoctorInfo.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[2].ToString());//挂号日期
                    reg.DoctorInfo.Templet.Noon.ID = this.Reader[3].ToString();
                    reg.Name = this.Reader[4].ToString();
                    reg.IDCard = this.Reader[5].ToString();
                    reg.Sex.ID = this.Reader[6].ToString();

                    reg.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[7].ToString());//出生日期

                    reg.Pact.PayKind.ID = this.Reader[8].ToString();//结算类别
                    reg.Pact.PayKind.Name = this.Reader[9].ToString();

                    reg.Pact.ID = this.Reader[10].ToString();//合同单位
                    reg.Pact.Name = this.Reader[11].ToString();
                    reg.SSN = this.Reader[12].ToString();
                    reg.SIMainInfo.RegNo = reg.SSN;

                    reg.DoctorInfo.Templet.RegLevel.ID = this.Reader[13].ToString();//挂号级别
                    reg.DoctorInfo.Templet.RegLevel.Name = this.Reader[14].ToString();

                    reg.DoctorInfo.Templet.Dept.ID = this.Reader[15].ToString();//挂号科室
                    reg.DoctorInfo.Templet.Dept.Name = this.Reader[16].ToString();

                    reg.DoctorInfo.SeeNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[17].ToString());

                    reg.DoctorInfo.Templet.Doct.ID = this.Reader[18].ToString();//看诊医生
                    reg.DoctorInfo.Templet.Doct.Name = this.Reader[19].ToString();

                    reg.RegType = (Neusoft.HISFC.Models.Base.EnumRegType)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[20].ToString());
                    reg.IsFirst = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[21].ToString());

                    reg.RegLvlFee.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[22].ToString());
                    reg.RegLvlFee.ChkFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[23].ToString());
                    reg.RegLvlFee.OwnDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[24].ToString());
                    reg.RegLvlFee.OthFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[25].ToString());

                    reg.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26].ToString());
                    reg.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27].ToString());
                    reg.PayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[28].ToString());

                    reg.Status = (Neusoft.HISFC.Models.Base.EnumRegisterStatus)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[29].ToString());

                    reg.InputOper.ID = this.Reader[30].ToString();
                    reg.IsSee = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[31].ToString());
                    reg.InputOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[32].ToString());
                    reg.TranType = (Neusoft.HISFC.Models.Base.TransTypes)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[33].ToString());
                    reg.BalanceOperStat.IsCheck = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[34]);//日结
                    reg.BalanceOperStat.CheckNO = this.Reader[35].ToString();
                    reg.BalanceOperStat.Oper.ID = this.Reader[36].ToString();

                    if (!this.Reader.IsDBNull(37))
                        reg.BalanceOperStat.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[37].ToString());

                    reg.PhoneHome = this.Reader[38].ToString();//联系电话
                    reg.AddressHome = this.Reader[39].ToString();//地址
                    reg.IsFee = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[40].ToString());
                    //作废人信息
                    reg.CancelOper.ID = this.Reader[41].ToString();
                    reg.CancelOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[42].ToString());
                    reg.CardType.ID = this.Reader[43].ToString();//证件类型
                    reg.DoctorInfo.Templet.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[44].ToString());
                    reg.DoctorInfo.Templet.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[45].ToString());
                    //reg.InvoiceNo = this.Reader[50].ToString() ;
                    //reg.InvoiceNO = this.Reader[51].ToString() ; by niuxinyuan
                    reg.InvoiceNO = this.Reader[50].ToString();
                    reg.RecipeNO = this.Reader[51].ToString();

                    reg.DoctorInfo.Templet.IsAppend = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[52].ToString());
                    reg.OrderNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[53].ToString());
                    reg.DoctorInfo.Templet.ID = this.Reader[54].ToString();
                    reg.InSource.ID = this.Reader[55].ToString();
                    reg.PVisit.InState.ID = this.Reader[56].ToString();
                    reg.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[57].ToString());
                    reg.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[58].ToString());
                    reg.PVisit.ZG.ID = this.Reader[59].ToString();
                    reg.PVisit.PatientLocation.Bed.ID = this.Reader[60].ToString();

                    //{6FC43DF1-86E1-4720-BA3F-356C25C74F16}
                    //标识是否是账户流程挂号 1代表是
                    reg.IsAccount = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[61].ToString());

                    //{E26C3EE9-D480-421e-9FD3-7094D8E4E1D0}
                    reg.SeeDoct.Dept.ID = this.Reader[62].ToString(); //看诊科室
                    reg.SeeDoct.ID = this.Reader[63].ToString();//看诊医生
                    //{156C449B-60A9-4536-B4FB-D00BC6F476A1}
                    reg.DoctorInfo.Templet.RegLevel.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[64].ToString());
                    //{921FBFCA-3D0D-4bc6-8EEA-A9BBE152E69A}
                    reg.Mark1 = this.Reader[65].ToString();
                    // reg.PID.CaseNO =this.q;

                    // {531B6C65-1DF5-4f16-94EC-F7D87287966F}
                    reg.SeeDoct.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[46].ToString());
                    //患者是否已经分诊
                    reg.IsTriage = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[47].ToString());
                    //{4AC12996-BC4B-4272-9FA4-E06DB8326330}
                    if (this.Reader.FieldCount >= 67)
                    {
                        reg.NormalName = this.Reader[66].ToString();

                    }
                    if (this.Reader.FieldCount > 67)
                    {
                        reg.Card.ID = this.Reader[67].ToString();
                        reg.Card.CardType.ID = this.Reader[68].ToString();
                        reg.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[69].ToString());
                    }

                    al.Add(reg);
                }
            }
            catch (Exception e)
            {
                this.Err = "检索挂号信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return al;
        }

        public ArrayList QueryWrongRegister(string clinicNo)
        {
            string sql = "";
            string where = @" where Clinic_code='{0}' ";

            try
            {
                if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;

                where = string.Format(where, clinicNo);

                sql = sql + " " + where;

                return this.QueryRegister(sql);
            }
            catch (Exception objEx)
            {
                this.Err = objEx.Message;
                return null;
            }
        }

        public ArrayList QueryChaoYangWrongRegister(string clinicNos)
        {

            ArrayList al = null;
            try
            {

                if (clinicNos != null && clinicNos.Length > 0)
                {
                    al = new ArrayList();
                    string[] strClinicNos = clinicNos.Split('|');
                    if (strClinicNos == null || strClinicNos.Length == 0)
                    {
                        this.Err = "门诊流水号分离出错！";
                        return null;
                    }
                    for (int i = 0; i <= strClinicNos.Length - 1; ++i)
                    {
                        if (strClinicNos[i].Length == 0) continue;
                        al.AddRange(QueryWrongRegister(strClinicNos[i]));
                    }


                }

                return al;
            }
            catch (Exception objEx)
            {
                this.Err = objEx.Message;
                return null;
            }
        }

        /// <summary>
        /// 根据门诊流水号，获取医保结算单号
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public string QuerySiMedNo(string clinicNo)
        {
            string sql = @"select ybmedno from fin_ipr_siinmaininfo a where a.inpatient_no='{0}' and a.valid_flag='1' and type_code='1' ";
            try
            {
                sql = string.Format(sql, clinicNo);
                return this.ExecSqlReturnOne(sql);
            }
            catch (Exception e)
            {

                this.Err = "根据门诊流水号，获取医保结算单号出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
        }

        /// <summary>
        /// 根据结算单号删除本地医保明细
        /// </summary>
        /// <param name="medno"></param>
        /// <returns></returns>
        public int DeleteLocalSiFeeDetail(string  medno)
        {
            string sql = @"delete from fs_local_masmzjsmx l where l.medno={0} ";
            try
            {
                int intMedno = int.Parse(medno);
                sql = string.Format(sql, intMedno);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {

                this.Err = "根据结算单号删除本地医保明细出错!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 更新医保主表
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <param name="medno"></param>
        /// <returns></returns>
        public int UpdateSiMainInfo(string clinicNo, string medno)
        {
            string sql = @"UPDATE fin_ipr_siinmaininfo a --医保信息住院主表
   SET VALID_FLAG = '0',
       OPERATECODE3='错误处理'
 WHERE a.type_code = '1'
   and a.inpatient_no = '{0}'
   and a.ybmedno = '{1}'";
            try
            {
                sql = string.Format(sql, clinicNo,medno);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {

                this.Err = "更新医保主表出错!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 更新上传日志表
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <param name="medno"></param>
        /// <returns></returns>
        public int UpdateSiSirecord(string clinicNo)
        {
            string sql = @"update fin_ipr_sirecord
   set valid_flag     = 1
 where clinic_code = '{0}'";
            try
            {
                sql = string.Format(sql, clinicNo);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {

                this.Err = "更新上传日志表出错!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }




        #endregion
    }
}
