using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizLogic.Order
{
    /// <summary>
    /// 
    /// </summary>
    public class EmergencyQS : Neusoft.FrameWork.Management.Database
    {
        #region 急诊质量与安全指标
        /// <summary>
        /// 插入信息
        /// </summary>
        /// <param name="emsqinfo"></param>
        /// <returns></returns>
        public int InsertEmergencyQS(Neusoft.HISFC.Models.Order.EmergencyQS emsqinfo)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("RADT.OutPatient.InsertEmergencyQS", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = string.Format(strSql, GetParams(emsqinfo));
            if (strSql == null)
            {
                this.Err = "格式化Sql语句时出错";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="emsqinfo"></param>
        /// <returns></returns>
        public string[] GetParams(Neusoft.HISFC.Models.Order.EmergencyQS emsqinfo)
        {
            string[] str = new string[] {
                emsqinfo.Clinic_code    ,
                emsqinfo.Card_no        ,
                emsqinfo.Name           ,
                emsqinfo.Sex_code       ,
                emsqinfo.Birthday.ToString(),
                emsqinfo.Reg_date.ToString()       ,
                emsqinfo.Triage_opcd    ,
                emsqinfo.Diag_name      ,
                emsqinfo.Gcreason       ,
                emsqinfo.Gone           ,
                emsqinfo.Goother        ,
                emsqinfo.Inobservation.ToString()  ,
                emsqinfo.Outobservation.ToString() ,
                emsqinfo.Observationtime.ToString("F2"),
                emsqinfo.Rescue         ,
                emsqinfo.Inrescue.ToString()       ,
                emsqinfo.Outrescue.ToString()      ,
                emsqinfo.Rescuetime.ToString("F2"),
                emsqinfo.Death          ,
                emsqinfo.Inhospital     ,
                emsqinfo.Indept         ,
                emsqinfo.Heartrescue    ,
                emsqinfo.Breath         ,
                emsqinfo.Emoperation    ,
                emsqinfo.Isdeath        ,
                emsqinfo.Returnrescue   ,
                emsqinfo.Greenchannel   ,
                emsqinfo.Gcother       ,
                emsqinfo.Heartdeath     ,
                emsqinfo.Inpci.ToString(),
                emsqinfo.Pcitime.ToString("F2"),
                emsqinfo.Thrombolysis.ToString()   ,
                emsqinfo.Thrombolysistime.ToString("F2"),
                emsqinfo.Operdoc,
                emsqinfo.Docdate.ToString()
                
            };
            return str;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="emsqinfo"></param>
        /// <returns></returns>
        public int UpdatetEmergencyQS(Neusoft.HISFC.Models.Order.EmergencyQS emsqinfo)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("RADT.OutPatient.UpdatetEmergencyQS", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = string.Format(strSql, GetParams(emsqinfo));
            if (strSql == null)
            {
                this.Err = "格式化Sql语句时出错";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 查询急诊科质量与安全指标实体 （通过门诊流水号）
        /// </summary>
        /// <param name="emsqinfo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Order.EmergencyQS QueryEmergencyQS(string clinicno)
        {
            Neusoft.HISFC.Models.Order.EmergencyQS emsqinfo = new Neusoft.HISFC.Models.Order.EmergencyQS();
            string strSql = "";
            if (this.Sql.GetCommonSql("RADT.OutPatient.QueryEmergencyQS", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = string.Format(strSql, clinicno);
            if (this.ExecQuery(strSql) == -1) return null;
            try
            {
                while (this.Reader.Read())
                {
                    emsqinfo.Clinic_code = this.Reader[0].ToString();
                    emsqinfo.Card_no = this.Reader[1].ToString();
                    emsqinfo.Name = this.Reader[2].ToString();
                    emsqinfo.Sex_code = this.Reader[3].ToString();
                    emsqinfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4].ToString());
                    emsqinfo.Reg_date = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[5].ToString());
                    emsqinfo.Triage_opcd = this.Reader[6].ToString();
                    emsqinfo.Diag_name = this.Reader[7].ToString();
                    emsqinfo.Gcother = this.Reader[8].ToString();
                    emsqinfo.Gone = this.Reader[9].ToString();
                    emsqinfo.Goother = this.Reader[10].ToString();
                    emsqinfo.Inobservation = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[11].ToString());
                    emsqinfo.Outobservation = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[12].ToString());
                    emsqinfo.Observationtime = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());
                    emsqinfo.Rescue = this.Reader[14].ToString();
                    emsqinfo.Inrescue = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[15].ToString());
                    emsqinfo.Outrescue = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[16].ToString());
                    emsqinfo.Rescuetime = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[17].ToString());
                    emsqinfo.Death = this.Reader[18].ToString();
                    emsqinfo.Inhospital = this.Reader[19].ToString();
                    emsqinfo.Indept = this.Reader[20].ToString();
                    emsqinfo.Heartrescue = this.Reader[21].ToString();
                    emsqinfo.Breath = this.Reader[22].ToString();
                    emsqinfo.Emoperation = this.Reader[23].ToString();
                    emsqinfo.Isdeath = this.Reader[24].ToString();
                    emsqinfo.Returnrescue = this.Reader[25].ToString();
                    emsqinfo.Greenchannel = this.Reader[26].ToString();
                    emsqinfo.Gcreason = this.Reader[27].ToString();
                    emsqinfo.Heartdeath = this.Reader[28].ToString();
                    emsqinfo.Inpci = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[29].ToString());
                    emsqinfo.Pcitime = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[30].ToString());
                    emsqinfo.Thrombolysis = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[31].ToString());
                    emsqinfo.Thrombolysistime = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[32].ToString());
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得患者治疗阶段信息出错！" + ex.Message;
                this.WriteErr();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return emsqinfo;
        }

        /// <summary>
        /// 查询急诊科质量与安全指标实体 （通过门诊卡号）
        /// </summary>
        /// <param name="emsqinfo"></param>
        /// <returns></returns>
        public ArrayList QueryEmergencyQSbyCarno(string carno)
        {
            ArrayList al = new ArrayList();
            
            string strSql = "";
            if (this.Sql.GetCommonSql("RADT.OutPatient.QueryEmergencyQSbyCarno", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = string.Format(strSql, carno);
            if (this.ExecQuery(strSql) == -1) return null;
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Order.EmergencyQS emsqinfo = new Neusoft.HISFC.Models.Order.EmergencyQS();
                    emsqinfo.Clinic_code = this.Reader[0].ToString();
                    emsqinfo.Card_no = this.Reader[1].ToString();
                    emsqinfo.Name = this.Reader[2].ToString();
                    emsqinfo.Sex_code = this.Reader[3].ToString();
                    emsqinfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4].ToString());
                    emsqinfo.Reg_date = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[5].ToString());
                    emsqinfo.Triage_opcd = this.Reader[6].ToString();
                    emsqinfo.Diag_name = this.Reader[7].ToString();
                    emsqinfo.Gcother = this.Reader[8].ToString();
                    emsqinfo.Gone = this.Reader[9].ToString();
                    emsqinfo.Goother = this.Reader[10].ToString();
                    emsqinfo.Inobservation = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[11].ToString());
                    emsqinfo.Outobservation = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[12].ToString());
                    emsqinfo.Observationtime = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[13].ToString());
                    emsqinfo.Rescue = this.Reader[14].ToString();
                    emsqinfo.Inrescue = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[15].ToString());
                    emsqinfo.Outrescue = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[16].ToString());
                    emsqinfo.Rescuetime = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[17].ToString());
                    emsqinfo.Death = this.Reader[18].ToString();
                    emsqinfo.Inhospital = this.Reader[19].ToString();
                    emsqinfo.Indept = this.Reader[20].ToString();
                    emsqinfo.Heartrescue = this.Reader[21].ToString();
                    emsqinfo.Breath = this.Reader[22].ToString();
                    emsqinfo.Emoperation = this.Reader[23].ToString();
                    emsqinfo.Isdeath = this.Reader[24].ToString();
                    emsqinfo.Returnrescue = this.Reader[25].ToString();
                    emsqinfo.Greenchannel = this.Reader[26].ToString();
                    emsqinfo.Gcreason = this.Reader[27].ToString();
                    emsqinfo.Heartdeath = this.Reader[28].ToString();
                    emsqinfo.Inpci = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[29].ToString());
                    emsqinfo.Pcitime = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[30].ToString());
                    emsqinfo.Thrombolysis = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[31].ToString());
                    emsqinfo.Thrombolysistime = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[32].ToString());
                    al.Add(emsqinfo);
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得患者治疗阶段信息出错！" + ex.Message;
                this.WriteErr();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return al;
        }

        #endregion

        /// <summary>
        /// 分诊人
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string QueryTriageOpcd(string id)
        {
            string operid = "";
            string strSql = @"SELECT triage_opcd FROM fin_opr_register where clinic_code='{0}'";
            strSql = string.Format(strSql, id);
            if (this.ExecQuery(strSql) == -1) return "";
            try
            {
                while (this.Reader.Read())
                {
                    operid = this.Reader[0].ToString();
                }
            }
            catch (Exception ex)
            {
                this.Err = "获得患者信息出错！" + ex.Message;
                this.WriteErr();
                return "";
            }
            finally
            {
                this.Reader.Close();
            }
            return operid;
        }

        Neusoft.HISFC.Models.Registration.Register reg = null;

        /// <summary>
        /// 挂号查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ArrayList QueryRegister(string sql)
        {
            if (this.ExecQuery(sql) == -1) return null;

            ArrayList al = new ArrayList();

            try
            {
                while (this.Reader.Read())
                {
                    this.reg = new Neusoft.HISFC.Models.Registration.Register();

                    this.reg.ID = this.Reader[0].ToString();//序号
                    this.reg.PID.CardNO = this.Reader[1].ToString();//病历号
                    this.reg.DoctorInfo.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[2].ToString());//挂号日期
                    this.reg.DoctorInfo.Templet.Noon.ID = this.Reader[3].ToString();
                    this.reg.Name = this.Reader[4].ToString();
                    this.reg.IDCard = this.Reader[5].ToString();
                    this.reg.Sex.ID = this.Reader[6].ToString();

                    this.reg.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[7].ToString());//出生日期

                    this.reg.Pact.PayKind.ID = this.Reader[8].ToString();//结算类别
                    this.reg.Pact.PayKind.Name = this.Reader[9].ToString();

                    this.reg.Pact.ID = this.Reader[10].ToString();//合同单位
                    this.reg.Pact.Name = this.Reader[11].ToString();
                    this.reg.SSN = this.Reader[12].ToString();
                    this.reg.SIMainInfo.RegNo = this.reg.SSN;

                    this.reg.DoctorInfo.Templet.RegLevel.ID = this.Reader[13].ToString();//挂号级别
                    this.reg.DoctorInfo.Templet.RegLevel.Name = this.Reader[14].ToString();

                    this.reg.DoctorInfo.Templet.Dept.ID = this.Reader[15].ToString();//挂号科室
                    this.reg.DoctorInfo.Templet.Dept.Name = this.Reader[16].ToString();

                    this.reg.DoctorInfo.SeeNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[17].ToString());

                    this.reg.DoctorInfo.Templet.Doct.ID = this.Reader[18].ToString();//看诊医生
                    this.reg.DoctorInfo.Templet.Doct.Name = this.Reader[19].ToString();

                    this.reg.RegType = (Neusoft.HISFC.Models.Base.EnumRegType)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[20].ToString());
                    this.reg.IsFirst = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[21].ToString());

                    this.reg.RegLvlFee.RegFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[22].ToString());
                    this.reg.RegLvlFee.ChkFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[23].ToString());
                    this.reg.RegLvlFee.OwnDigFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[24].ToString());
                    this.reg.RegLvlFee.OthFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[25].ToString());

                    this.reg.OwnCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[26].ToString());
                    this.reg.PubCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[27].ToString());
                    this.reg.PayCost = Neusoft.FrameWork.Function.NConvert.ToDecimal(this.Reader[28].ToString());

                    this.reg.Status = (Neusoft.HISFC.Models.Base.EnumRegisterStatus)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[29].ToString());

                    this.reg.InputOper.ID = this.Reader[30].ToString();
                    this.reg.IsSee = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[31].ToString());
                    this.reg.InputOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[32].ToString());
                    this.reg.TranType = (Neusoft.HISFC.Models.Base.TransTypes)Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[33].ToString());
                    this.reg.BalanceOperStat.IsCheck = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[34]);//日结
                    this.reg.BalanceOperStat.CheckNO = this.Reader[35].ToString();
                    this.reg.BalanceOperStat.Oper.ID = this.Reader[36].ToString();

                    if (!this.Reader.IsDBNull(37))
                        this.reg.BalanceOperStat.Oper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[37].ToString());

                    this.reg.PhoneHome = this.Reader[38].ToString();//联系电话
                    this.reg.AddressHome = this.Reader[39].ToString();//地址
                    this.reg.IsFee = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[40].ToString());
                    //作废人信息
                    this.reg.CancelOper.ID = this.Reader[41].ToString();
                    this.reg.CancelOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[42].ToString());
                    this.reg.CardType.ID = this.Reader[43].ToString();//证件类型
                    this.reg.DoctorInfo.Templet.Begin = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[44].ToString());
                    this.reg.DoctorInfo.Templet.End = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[45].ToString());
                    //this.reg.InvoiceNo = this.Reader[50].ToString() ;
                    //this.reg.InvoiceNO = this.Reader[51].ToString() ; by niuxinyuan
                    this.reg.InvoiceNO = this.Reader[50].ToString();
                    this.reg.RecipeNO = this.Reader[51].ToString();

                    this.reg.DoctorInfo.Templet.IsAppend = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[52].ToString());
                    this.reg.OrderNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[53].ToString());
                    this.reg.DoctorInfo.Templet.ID = this.Reader[54].ToString();
                    this.reg.InSource.ID = this.Reader[55].ToString();
                    this.reg.PVisit.InState.ID = this.Reader[56].ToString();
                    this.reg.PVisit.InTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[57].ToString());
                    this.reg.PVisit.OutTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[58].ToString());
                    this.reg.PVisit.ZG.ID = this.Reader[59].ToString();
                    this.reg.PVisit.PatientLocation.Bed.ID = this.Reader[60].ToString();

                    //{6FC43DF1-86E1-4720-BA3F-356C25C74F16}
                    //标识是否是账户流程挂号 1代表是
                    this.reg.IsAccount = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[61].ToString());

                    //{E26C3EE9-D480-421e-9FD3-7094D8E4E1D0}
                    this.reg.SeeDoct.Dept.ID = this.Reader[62].ToString(); //看诊科室
                    this.reg.SeeDoct.ID = this.Reader[63].ToString();//看诊医生
                    //{156C449B-60A9-4536-B4FB-D00BC6F476A1}
                    this.reg.DoctorInfo.Templet.RegLevel.IsEmergency = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[64].ToString());
                    //{921FBFCA-3D0D-4bc6-8EEA-A9BBE152E69A}
                    this.reg.Mark1 = this.Reader[65].ToString();
                    // this.reg.PID.CaseNO =this.q;

                    // {531B6C65-1DF5-4f16-94EC-F7D87287966F}
                    this.reg.SeeDoct.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[46].ToString());
                    //患者是否已经分诊
                    this.reg.IsTriage = Neusoft.FrameWork.Function.NConvert.ToBoolean(this.Reader[47].ToString());
                    //{4AC12996-BC4B-4272-9FA4-E06DB8326330}
                    if (this.Reader.FieldCount >= 67)
                    {
                        this.reg.NormalName = this.Reader[66].ToString();

                    }
                    if (this.Reader.FieldCount > 67)
                    {
                        this.reg.Card.ID = this.Reader[67].ToString();
                        this.reg.Card.CardType.ID = this.Reader[68].ToString();
                        this.reg.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[69].ToString());
                    }
                    if (this.Reader.FieldCount > 70)
                    {
                        this.reg.Temperature = this.Reader[70].ToString();
                    }
                    if (Reader.FieldCount > 71)
                    {
                        reg.PatientType = Reader[71].ToString();
                    }
                    reg.RegExtend = new Neusoft.HISFC.Models.Registration.RegisterExtend();
                    if (Reader.FieldCount > 72)
                    {
                        reg.RegExtend.DiagFeeRegCode = Reader[72].ToString();
                    }
                    if (Reader.FieldCount > 73)
                    {
                        reg.RegExtend.DiagFee = Neusoft.FrameWork.Function.NConvert.ToDecimal(Reader[73].ToString());
                    }
                    if (Reader.FieldCount > 74)
                    {
                        reg.RegExtend.DiagItemCode = Reader[74].ToString();
                    }

                    al.Add(this.reg);
                }
                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "检索挂号信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            return al;
        }

        public ArrayList Querypation(DateTime begin,DateTime end)
        {
            string strSql = ""; string where = "";
            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }

            if (this.Sql.GetCommonSql("RADT.OutPatient.QueryZDWYpation", ref where) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }


            //where = @" WHERE reg_date>=to_date('{0}','yyyy-mm-dd HH24:mi:ss') AND reg_date<=to_date('{1}','yyyy-mm-dd HH24:mi:ss') AND valid_flag='1' AND is_emergency='1' and hos_code='CORE_HIS50'";
            where = string.Format(where,begin.ToString(),end.ToString());
            return QueryRegister(strSql + where);
        }


        #region 急诊检诊、分诊登记
        /// <summary>
        /// 插入信息
        /// </summary>
        /// <param name="emsqinfo"></param>
        /// <returns></returns>
        public int InsertDetectionTriageNote(Neusoft.HISFC.Models.Order.EmergencyQS emsqinfo)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("RADT.OutPatient.InsertDetectionTriageNote", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = string.Format(strSql, GetDTNParams(emsqinfo));
            if (strSql == null)
            {
                this.Err = "格式化Sql语句时出错";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="emsqinfo"></param>
        /// <returns></returns>
        public int UpdatetDetectionTriageNote(Neusoft.HISFC.Models.Order.EmergencyQS emsqinfo)
        {
            string strSql = "";

            if (this.Sql.GetCommonSql("RADT.OutPatient.UpdatetDetectionTriageNote", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            strSql = string.Format(strSql, GetDTNParams(emsqinfo));
            if (strSql == null)
            {
                this.Err = "格式化Sql语句时出错";
                this.WriteErr();
                return -1;
            }
            return this.ExecNoQuery(strSql);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="emsqinfo"></param>
        /// <returns></returns>
        public string[] GetDTNParams(Neusoft.HISFC.Models.Order.EmergencyQS emsqinfo)
        {
            string[] str = new string[] {
                emsqinfo.Clinic_code    ,
                emsqinfo.Card_no        ,
                emsqinfo.Name           ,
                emsqinfo.Sex_code       ,
                emsqinfo.Birthday.ToString(),
                emsqinfo.Reg_date.ToString(),
                emsqinfo.Triage_opcd    ,
                emsqinfo.Level          ,
                emsqinfo.Gone           ,
                emsqinfo.Goother        ,
                emsqinfo.Diag_neu       ,
                emsqinfo.Dept           ,
                emsqinfo.Contact        ,
                emsqinfo.Greenchannel   ,
                emsqinfo.Operneu        ,
                emsqinfo.Neudate.ToString()        

            };
            return str;
        }

        /// <summary>
        /// 查询急诊科质量与安全指标实体 （通过门诊流水号）
        /// </summary>
        /// <param name="emsqinfo"></param>
        /// <returns></returns>
        public ArrayList QueryDetectionTriageNote(DateTime begin, DateTime end)
        {
            ArrayList al = new ArrayList();
            
            string strSql = "";
            if (this.Sql.GetCommonSql("RADT.OutPatient.QueryDetectionTriageNote", ref strSql) == -1)
            {
                this.Err = this.Sql.Err;
                return null;
            }
            strSql = string.Format(strSql, begin.ToShortDateString(), end.ToShortDateString());
            if (this.ExecQuery(strSql) == -1) return null;
            try
            {
                while (this.Reader.Read())
                {
                    Neusoft.HISFC.Models.Order.EmergencyQS emsqinfo = new Neusoft.HISFC.Models.Order.EmergencyQS();
                    emsqinfo.Clinic_code = this.Reader[0].ToString();
                    emsqinfo.Card_no = this.Reader[1].ToString();
                    emsqinfo.Name = this.Reader[2].ToString();
                    emsqinfo.Sex_code = this.Reader[3].ToString();
                    emsqinfo.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4].ToString());
                    emsqinfo.Reg_date = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[5].ToString());
                    emsqinfo.Triage_opcd = this.Reader[6].ToString();
                    emsqinfo.Level = this.Reader[7].ToString();
                    emsqinfo.Gone = this.Reader[8].ToString();
                    emsqinfo.Goother = this.Reader[9].ToString();
                    emsqinfo.Diag_neu = this.Reader[10].ToString();
                    emsqinfo.Dept = this.Reader[11].ToString();
                    emsqinfo.Contact = this.Reader[12].ToString();
                    emsqinfo.Greenchannel = this.Reader[13].ToString();
                    emsqinfo.Operneu = this.Reader[14].ToString();
                    emsqinfo.Neudate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[15].ToString());
                    al.Add(emsqinfo);

                }
            }
            catch (Exception ex)
            {
                this.Err = "获得患者治疗阶段信息出错！" + ex.Message;
                this.WriteErr();
                return null;
            }
            finally
            {
                this.Reader.Close();
            }
            return al;
        }

        #endregion
    }
}
