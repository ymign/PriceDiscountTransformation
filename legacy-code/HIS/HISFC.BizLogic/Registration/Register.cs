using System;
using System.Collections;
using Neusoft.HISFC.Models.RADT;
using Neusoft.FrameWork.Function;
using System.Collections.Generic;
using System.Data;

namespace Neusoft.HISFC.BizLogic.Registration
{
    /// <summary>
    /// 挂号管理类
    /// </summary>
    public class Register : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        ///  挂号管理类
        /// </summary>
        public Register()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        //private ArrayList al = new ArrayList();
        private Neusoft.HISFC.Models.Registration.Register reg;

        #region 增、删、改

        //账户流程 医生站收挂号费，置挂号费收费状态 {6FC43DF1-86E1-4720-BA3F-356C25C74F16}
        /// <summary>
        /// 置已收挂号费标志
        /// </summary>
        /// <param name="clinicID"></param>
        /// <param name="operID"></param>
        /// <param name="operDate"></param>
        /// <returns></returns>
        public int UpdateAccountFeeState(string clinicID, string operID, string dept, DateTime operDate)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.UpdateAccountFeeState", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, clinicID, operID, dept, operDate.ToString());
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "置患者收费标志出错![Registration.Register.UpdateAccountFeeState]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        /// <summary>
        /// 更新挂号记录费用信息
        /// </summary>
        /// <param name="objRegister"></param>
        /// <returns></returns>
        public int UpdateRegFeeCost(Neusoft.HISFC.Models.Registration.Register objRegister)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.UpdateRegFeeCost", ref sql) == -1)
            {
                this.Err = "查找索引为 Registration.Register.UpdateRegFeeCost 的Sql语句失败！";
                return -1;
            }

            try
            {
                sql = string.Format(sql,
                    objRegister.ID,
                    objRegister.InvoiceNO,
                    objRegister.RegLvlFee.RegFee,
                    objRegister.RegLvlFee.ChkFee,
                    objRegister.RegLvlFee.OwnDigFee,
                    objRegister.RegLvlFee.OthFee,
                    objRegister.OwnCost,
                    objRegister.PubCost,
                    objRegister.PayCost);

                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "置患者收费标志出错![Registration.Register.UpdateRegFeeCost]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 删除挂号记录表{E43E0363-0B22-4d2a-A56A-455CFB7CF211}
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int DeleteByClinic(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Delete", ref sql) == -1) return -1;

            try
            {
                //{6FC43DF1-86E1-4720-BA3F-356C25C74F16}
                sql = string.Format(sql, register.ID);
            }
            catch
            {
                return -1;
            }

            return this.ExecNoQuery(sql);

        }


        /// <summary>
        /// 插入挂号记录表{E43E0363-0B22-4d2a-A56A-455CFB7CF211}
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = "";
            if (register.TranType == Neusoft.HISFC.Models.Base.TransTypes.Positive)
            {
                if (this.Sql.GetCommonSql("Registration.Register.GetInTimes", ref sql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }

                //先获取登记次数
                string inTimes = this.ExecSqlReturnOne(string.Format(sql, register.PID.CardNO));
                if (string.IsNullOrEmpty(inTimes) || inTimes.Equals("-1"))
                {
                    return -1;
                }

                register.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(inTimes);
            }

            if (this.Sql.GetCommonSql("Registration.Register.Insert.1", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }

            try
            {
                //{6FC43DF1-86E1-4720-BA3F-356C25C74F16}
                sql = string.Format(sql,
                    register.ID,
                    register.PID.CardNO,
                    register.DoctorInfo.SeeDate.ToString(),
                    register.DoctorInfo.Templet.Noon.ID,
                    register.Name,
                    register.IDCard,
                    register.Sex.ID,
                    register.Birthday.ToString(),
                    register.Pact.PayKind.ID,
                    register.Pact.PayKind.Name,
                    register.Pact.ID,
                    register.Pact.Name,
                    register.SSN,
                    register.DoctorInfo.Templet.RegLevel.ID,
                    register.DoctorInfo.Templet.RegLevel.Name,
                    register.DoctorInfo.Templet.Dept.ID,
                    register.DoctorInfo.Templet.Dept.Name,
                    register.DoctorInfo.SeeNO,
                    register.DoctorInfo.Templet.Doct.ID,
                    register.DoctorInfo.Templet.Doct.Name,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsFee),
                    (int)register.RegType,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsFirst),
                    register.RegLvlFee.RegFee.ToString(),
                    register.RegLvlFee.ChkFee.ToString(),
                    register.RegLvlFee.OwnDigFee.ToString(),
                    register.RegLvlFee.OthFee.ToString(),
                    register.OwnCost.ToString(),
                    register.PubCost.ToString(),
                    register.PayCost.ToString(),
                    (int)register.Status,
                    register.InputOper.ID,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsSee),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.CheckOperStat.IsCheck),
                    register.PhoneHome,
                    register.AddressHome,
                    (int)register.TranType,
                    register.CardType.ID,
                    register.DoctorInfo.Templet.Begin.ToString(),
                    register.DoctorInfo.Templet.End.ToString(),
                    register.CancelOper.ID,
                    register.CancelOper.OperTime.ToString(),
                    register.InvoiceNO,
                    register.RecipeNO,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.DoctorInfo.Templet.IsAppend),
                    register.OrderNO,
                    register.DoctorInfo.Templet.ID,
                    register.InputOper.OperTime.ToString(),
                    register.InSource.ID,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.CaseState),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsEncrypt),
                    register.NormalName,
                    register.EcoCost,
                    NConvert.ToInt32(register.IsAccount).ToString(),
                    /*{156C449B-60A9-4536-B4FB-D00BC6F476A1}*/
                    NConvert.ToInt32(register.DoctorInfo.Templet.RegLevel.IsEmergency),
                    register.Mark1,
                    register.Card.ID,
                    register.Card.CardType.ID,
                    register.InTimes.ToString(),

                    register.PatientType
                    );

                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "插入挂号主表类别表出错![Registration.Register.Insert.1]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 查询gd表中合同单位
        /// </summary>
        public int GetGDPact(string ClinicNo, ref DataTable dt)
        {
            Neusoft.FrameWork.Management.ExtendParam extentManager = new Neusoft.FrameWork.Management.ExtendParam();
            string strSql = @"SELECT f.pact_code,f.pact_name,f.paykind_code FROM fin_ipr_siinmaininfo_gd f where f.inpatient_no = '{0}' and f.valid_flag = '1' and f.type_code = '0'";
            strSql = string.Format(strSql, ClinicNo);
            DataSet dataSet = new DataSet();
            try
            {
                if (this.ExecQuery(strSql, ref dataSet) == -1)
                {
                    this.Err = "查询医保结算合同单位失败！";
                    return -1;
                }
                dt = dataSet.Tables[0];
                return 1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        /// <summary>
        /// 插入诊查费到费用表
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int InsertZZQRegFeeDetail(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Registration.Register.RegFeeDetailInsert", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            try
            {
                string belongDept = this.GetBelongDeptCodeForEmplCode(register.DoctorInfo.Templet.Dept.ID);
                string itemCode = this.GetRegItemCode(register.DoctorInfo.Templet.RegLevel.ID);
                if (string.IsNullOrEmpty(itemCode))
                {
                    this.Err = "获取诊疗项目出错!";
                    return -1;
                }
                string itemName = this.GetItemNameForItemCode(itemCode);
                if (string.IsNullOrEmpty(itemName))
                {
                    this.Err = "获取诊疗项目名称出错!";
                    return -1;
                }
                string itemPrice = this.GetPriceForItemCode(itemCode);
                if (string.IsNullOrEmpty(itemPrice))
                {
                    this.Err = "获取诊疗项目价格出错!";
                    return -1;
                }
                sql = string.Format(sql,
                    this.GetOpbRecipeNoSequece(),
                    "1",
                    "1",
                    register.ID,
                    register.PID.CardNO,
                    register.DoctorInfo.SeeDate.ToString(),
                    register.DoctorInfo.Templet.Dept.ID,
                    register.DoctorInfo.Templet.Doct.ID,
                    register.DoctorInfo.Templet.Dept.ID,
                    itemCode,
                    itemName,
                    "0",
                    "次",
                    "015",
                    "U",
                    itemPrice,
                    "1",
                    "1",
                    "0",
                    "0",
                    "0",
                    "1",
                    "次",
                    "0",
                    "0",
                    itemPrice,
                    register.DoctorInfo.Templet.Dept.ID,
                    register.DoctorInfo.Templet.Dept.Name,
                    "0",
                    register.InputOper.ID,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "0",
                    "1",
                    "0",
                    "0",
                    "1",
                    "1",
                    "0",
                    this.GetMetMOOrderIDSequece(),
                    itemPrice,
                    "0", 
                    "0", 
                    "0", 
                    "0", 
                    "0",
                    belongDept,
                    "01",
                    register.Pact.ID,
                    itemPrice,
                    "0",
                    belongDept,
                    "CORE_HIS50",
                    "NULL"
                    );

                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "插入门诊费用表出错![Registration.Register.RegFeeDetailInsert]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

        }

        /// <summary>
        /// 插入挂号记录表{E43E0363-0B22-4d2a-A56A-455CFB7CF211}
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int InsertReg(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = "";
            if (register.TranType == Neusoft.HISFC.Models.Base.TransTypes.Positive)
            {
                if (this.Sql.GetCommonSql("Registration.Register.GetInTimes", ref sql) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                }

                //先获取登记次数
                string inTimes = this.ExecSqlReturnOne(string.Format(sql, register.PID.CardNO));
                if (string.IsNullOrEmpty(inTimes) || inTimes.Equals("-1"))
                {
                    return -1;
                }

                register.InTimes = Neusoft.FrameWork.Function.NConvert.ToInt32(inTimes);
            }

            //if (this.Sql.GetCommonSql("Registration.Register.RegInsert.1", ref sql) == -1)
            //if (this.Sql.GetCommonSql("Registration.Register.RegInsert.Greenway.1", ref sql) == -1)
            if (this.Sql.GetCommonSql("Registration.Register.RegInsert.Greenway.2", ref sql) == -1)
            {
                this.Err = this.Sql.Err;
                return -1;
            }
            if (register.RegExtend == null)
            {
                register.RegExtend = new Neusoft.HISFC.Models.Registration.RegisterExtend();
            }
            try
            {
                DataTable dtPact = new DataTable();
                if (this.GetGDPact(register.ID, ref dtPact) == -1)
                {
                    this.Err = this.Sql.Err;
                    return -1;
                };
                if (dtPact.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtPact.Rows[0][0].ToString()))
                    {
                        register.Pact.ID = dtPact.Rows[0][0].ToString();
                    }
                    if (!string.IsNullOrEmpty(dtPact.Rows[0][1].ToString()))
                    {
                        register.Pact.Name = dtPact.Rows[0][1].ToString();
                    } 
                    if (!string.IsNullOrEmpty(dtPact.Rows[0][2].ToString()))
                    {
                        register.Pact.PayKind.ID = dtPact.Rows[0][2].ToString();
                    }
                }
                //{6FC43DF1-86E1-4720-BA3F-356C25C74F16}
                sql = string.Format(sql,
                    register.ID,
                    register.PID.CardNO,
                    register.DoctorInfo.SeeDate.ToString(),
                    register.DoctorInfo.Templet.Noon.ID,
                    register.Name,
                    register.IDCard,
                    register.Sex.ID,
                    register.Birthday.ToString(),
                    register.Pact.PayKind.ID,
                    register.Pact.PayKind.Name,
                    register.Pact.ID,
                    register.Pact.Name,
                    register.SSN,
                    register.DoctorInfo.Templet.RegLevel.ID,
                    register.DoctorInfo.Templet.RegLevel.Name,
                    register.DoctorInfo.Templet.Dept.ID,
                    register.DoctorInfo.Templet.Dept.Name,
                    register.DoctorInfo.SeeNO,
                    register.DoctorInfo.Templet.Doct.ID,
                    register.DoctorInfo.Templet.Doct.Name,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsFee),
                    (int)register.RegType,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsFirst),
                    register.RegLvlFee.RegFee.ToString(),
                    register.RegLvlFee.ChkFee.ToString(),
                    register.RegLvlFee.OwnDigFee.ToString(),
                    register.RegLvlFee.OthFee.ToString(),
                    register.OwnCost.ToString(),
                    register.PubCost.ToString(),
                    register.PayCost.ToString(),
                    (int)register.Status,
                    register.InputOper.ID,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsSee),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.CheckOperStat.IsCheck),
                    register.PhoneHome,
                    register.AddressHome,
                    (int)register.TranType,
                    register.CardType.ID,
                    register.DoctorInfo.Templet.Begin.ToString(),
                    register.DoctorInfo.Templet.End.ToString(),
                    register.CancelOper.ID,
                    register.CancelOper.OperTime.ToString(),
                    register.InvoiceNO,
                    register.RecipeNO,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.DoctorInfo.Templet.IsAppend),
                    register.OrderNO,
                    register.DoctorInfo.Templet.ID,
                    register.InputOper.OperTime.ToString(),
                    register.InSource.ID,
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.CaseState),
                    Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsEncrypt),
                    register.NormalName,
                    register.EcoCost,
                    NConvert.ToInt32(register.IsAccount).ToString(),
                    /*{156C449B-60A9-4536-B4FB-D00BC6F476A1}*/
                    NConvert.ToInt32(register.DoctorInfo.Templet.RegLevel.IsEmergency),
                    register.Mark1,
                    register.Card.ID,
                    register.Card.CardType.ID,
                    register.InTimes.ToString(),

                    register.PatientType,
                    register.RegExtend.DiagFeeRegCode,
                    register.RegExtend.DiagFee,
                    register.RegExtend.DiagItemCode,
                    register.Greenway,
                    register.Triage_SerialNum
                    );

                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "插入挂号主表类别表出错![Registration.Register.RegInsert.Greenway.1]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 更新挂号信息,作废(注销)、退号、取消作废、换科、修改患者信息
        /// </summary>
        /// <param name="status"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        public int Update(EnumUpdateStatus status, Neusoft.HISFC.Models.Registration.Register register)
        {
            if (status == EnumUpdateStatus.Cancel)
            {
                return this.CancelReg(register.ID, register.CancelOper.ID, register.CancelOper.OperTime, status);
            }
            else if (status == EnumUpdateStatus.Return)
            {
                return this.CancelReg(register.ID, register.CancelOper.ID, register.CancelOper.OperTime, status);
            }
            else if (status == EnumUpdateStatus.ChangeDept)
            {
                return this.ChangeDept(register);
            }
            else if (status == EnumUpdateStatus.PatientInfo)
            {
                return this.UpdatePatientInfo(register);
            }
            else if (status == EnumUpdateStatus.Uncancel)
            {
                return this.Uncancel(register.ID);
            }
            else if (status == EnumUpdateStatus.Bad)
            {
                return this.CancelReg(register.ID, register.CancelOper.ID, register.CancelOper.OperTime, status);
            }
            return 0;
        }

        /// <summary>
        /// 更新挂号吧医院信息
        /// </summary>
        /// <param name="register"></param>
        /// <param name="hosCode">医院编码</param>
        /// <returns></returns>
        public int UpdateHosCode(Neusoft.HISFC.Models.Registration.Register register, string hosCode)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Registration.Register.UpdateHosCode", ref sql) == -1)
            {
                return -1;
            }
            try
            {
                sql = string.Format(sql, register.ID, hosCode);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新医院编码出错！【Registration.Register.UpdateHosCode】";
                this.ErrCode = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 置已分诊标志
        /// </summary>
        /// <param name="clinicID"></param>
        /// <param name="operID"></param>
        /// <param name="operDate"></param>
        /// <returns></returns>
        public int Update(string clinicID, string operID, DateTime operDate)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.UpdateTriage", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, clinicID, operID, operDate.ToString());
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "置患者分诊标志出错![Registration.Register.UpdateTriage]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 作废原有挂号记录
        /// </summary>
        /// <param name="clinicID"></param>
        /// <param name="cancelID"></param>
        /// <param name="cancelDate"></param>
        /// <param name="cancelFlag"></param>
        /// <returns></returns>
        private int CancelReg(string clinicID, string cancelID, DateTime cancelDate, EnumUpdateStatus cancelFlag)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.CancelReg", ref sql) == -1) return -1;

            try
            {
                int flag = (int)cancelFlag;
                if (cancelFlag == EnumUpdateStatus.Bad)
                {
                    flag = 3;
                }
                sql = string.Format(sql, clinicID, cancelID, cancelDate.ToString(), flag);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "作废挂号记录出错![Registration.Register.CancelReg]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
         /// <summary>
        /// 查询患者基础信息
        /// </summary>
        /// <param name="pactCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="minCode"></param>
        /// <returns></returns>
        public int Setpatientinfo(string operID)
        {
            string strSql = "";
            int returnRows = 0;
            if (this.Sql.GetCommonSql("RADT.Inpatient.PatientInfoQuery.WithCadre", ref strSql) == -1) return 1;
            strSql = string.Format(strSql, operID);

            try
            {
                returnRows = this.ExecQuery(strSql);
                if (NConvert.ToInt32(returnRows) > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ee)
            {
                this.Err = ee.Message;
                return 1;
            }
        }
        /// <summary>
        /// 换科(无用，暂无该需求)
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        private int ChangeDept(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.ChangeDept", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, register.ID, register.DoctorInfo.Templet.Dept.ID, register.DoctorInfo.Templet.Dept.Name,
                    register.DoctorInfo.SeeNO, register.DoctorInfo.Templet.Doct.ID, register.DoctorInfo.Templet.Doct.Name,
                    register.RegLvlFee.RegFee, register.RegLvlFee.ChkFee, register.RegLvlFee.OwnDigFee, register.RegLvlFee.OthFee,
                    register.OwnCost, register.PubCost, register.PayCost);

                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新挂号记录出错![Registration.Register.ChangeDept]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        /// <summary>
        /// 取消作废(注销)
        /// </summary>
        /// <param name="clinicID"></param>
        /// <returns></returns>
        private int Uncancel(string clinicID)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Uncancel", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, clinicID);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "作废挂号记录出错![Registration.Register.Uncancel]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        /// <summary>
        /// 取消分诊状态
        /// </summary>
        /// <param name="clinicID"></param>
        /// <returns></returns>
        public int CancelTriage(string clinicID)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.CancelTriage", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, clinicID);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "取消挂号信息的分诊状态出错![Registration.Register.CancelTriage]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        #region 更新患者信息（门诊收费）
        /// <summary>
        /// 更新患者基本信息（门诊收费）
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int UpdatePatientInfoForClinicFee(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = "";

            #region SQL
            /* UPDATE com_patientinfo   --病人基本信息表
               SET name='{0}',   --姓名
                   birthday=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),   --出生日期
                   sex_code='{2}',   --性别
                   home='{3}',   --户口或家庭所在
                   home_tel='{4}',   --家庭电话       
                   mark ='{6}',
                   inhos_source='{7}',
                   paykind_code='{8}',
                   pact_code='{9}',
                   pact_name='{10}',
                   mcard_no='{11}',
                   is_encryptname = '{12}',
                   normalname = '{13}'
             WHERE card_no = '{5}'*/
            #endregion

            if (this.Sql.GetCommonSql("Registration.Register.Update.PatientInfo.2", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, register.Name, register.Birthday.ToString(), register.Sex.ID,
                                        register.AddressHome, register.PhoneHome, register.PID.CardNO, register.CardType.ID,
                                        register.InSource.ID, register.Pact.PayKind.ID, register.Pact.ID, register.Pact.Name,
                                        register.SSN, Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsEncrypt), register.NormalName);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新患者信息出错![Registration.Register.Update.PatientInfo.2]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        /// <summary>
        /// 更新挂号表中的患者信息（门诊收费）
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int UpdateRegInfoForClinicFee(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = "";

            #region SQL
            /* UPDATE fin_opr_register   --挂号主表
                SET name='{0}',   --姓名
                    birthday=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),   --出生日期
                    sex_code='{2}',   --性别
                    address='{3}',   --地址
                    rela_phone ='{4}' --联系电话
               WHERE clinic_code='{5}' and trans_type='1'*/
            #endregion

            if (this.Sql.GetCommonSql("Registration.Register.Update.PatientInfo.3", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, register.Name,
                                        register.Birthday.ToString(),
                                        register.Sex.ID,
                                        register.AddressHome,
                                        register.PhoneHome,
                                        register.ID);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新患者信息出错![Registration.Register.Update.PatientInfo.3]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

        }
        #endregion
        /// <summary>
        /// 更新患者基本信息
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        private int UpdatePatientInfo(Neusoft.HISFC.Models.Registration.Register register)
        {
            //{D944AF1A-3BDE-4d51-BBA3-EB0FE779C7FC}增加身份证号
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Update.PatientInfo", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql,
                    register.Name,
                    register.Birthday.ToString(),
                    register.Sex.ID,
                    register.AddressHome,
                    register.PhoneHome,
                    register.PID.CardNO,
                    register.CardType.ID,
                    register.InSource.ID,
                    register.Pact.PayKind.ID,
                    register.Pact.ID,
                    register.Pact.Name,
                    register.SSN,
                    NConvert.ToInt32(register.IsEncrypt),
                    register.NormalName,
                    register.IDCard,
                    register.PatientType,
                    //register.CardType.ID,
                    register.PhoneBusiness
                    );
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新患者信息出错![Registration.Register.Update.PatientInfo]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

        #region {FCEC42B4-DF78-45c2-8D1A-EDAB94AA56DD} 分诊时修改患者基本信息

        /// <summary>
        /// 更新挂号表中的患者信息
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int UpdateRegInfo(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Update.PatientInfo.1", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, register.Name,
                                                        register.Birthday.ToString(),
                                                        register.Sex.ID,
                                                        register.AddressHome,
                                                        register.PhoneHome,
                    //register.CardType.ID,
                    //register.InSource.ID,
                    //register.Pact.PayKind.ID,
                    //register.Pact.PayKind.Name,
                    //register.Pact.ID, 
                    //register.Pact.Name, 
                    //register.Pact.Name,
                    //register.SSN,
                    //Neusoft.FrameWork.Function.NConvert.ToInt32(register.IsEncrypt),
                    //register.NormalName,
                                                        register.IDCard,
                                                        register.ID);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新患者信息出错![Registration.Register.Update.PatientInfo.1]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

        }

        /// <summary>
        /// 修改信息更新挂号表急诊标志与温度
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int UpdateRegInfoAdd(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Registration.Register.Update.PatientInfo.2", ref sql) == -1)
            {
                sql = "UPDATE fin_opr_register   SET is_emergency='{0}',  temperature='{1}'  WHERE clinic_code='{2}' and trans_type='1'";
            }
            string isEmergency = "";
            if (register.DoctorInfo.Templet.RegLevel.IsEmergency == true)
            { isEmergency = "1"; }
            else
            { isEmergency = "0"; }
            try
            {
                sql = string.Format(sql, isEmergency,
                                                        register.Temperature,
                                                        register.ID);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新患者信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

        }

        /// <summary>
        /// 分诊时更新患者基本信息
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int UpdatePatientForNurse(Neusoft.HISFC.Models.Registration.Register register)
        {
            //{D944AF1A-3BDE-4d51-BBA3-EB0FE779C7FC}增加身份证号
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Update.PatientInfo", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, register.Name, register.Birthday.ToString(), register.Sex.ID,
                                        register.AddressHome, register.PhoneHome, register.PID.CardNO,
                                        register.IDCard, register.PatientType, register.PhoneBusiness);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新患者信息出错![Registration.Register.Update.PatientInfo]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        #endregion

        /// <summary>
        /// 换科{87C56F02-B81A-4fac-BA4D-654C8E56C500}
        /// </summary>
        /// <param name="clinicNO">挂号流水号</param>
        /// <param name="deptCode">科室编码</param>
        /// <param name="deptName">科室名称</param>
        /// <param name="doctCode">医生编码</param>
        /// <param name="doctName">医生名称</param>
        /// <param name="dtReg">挂号时间</param>
        /// <returns></returns>
        public int UpdateDeptAndDoct(string clinicNO, string deptCode, string deptName, string doctCode, string doctName, string dtReg)
        {
            string strSql = string.Empty;
            int returnValue = this.Sql.GetCommonSql("Registration.Register.UpdateDeptAndDoct", ref  strSql);
            if (returnValue < 0)
            {
                this.Err = "没有Registration.Register.UpdateDeptAndDoct对应的sql语句";
                return -1;
            }
            strSql = string.Format(strSql, clinicNO, deptCode, deptName, doctCode, doctName, dtReg);
            return this.ExecNoQuery(strSql);
        }

        #endregion

        #region 更新

        #region 挂号更新限额
        /// <summary>
        /// 更新看诊序号
        /// </summary>
        /// <param name="Type">1医生 2科室 4全院</param>
        /// <param name="seeDate">看诊日期</param>
        /// <param name="Subject">Type=1时,医生代码;Type=2,科室代码;Type=4,ALL</param>
        /// <param name="noonID">午别</param>
        /// <returns></returns>
        public int UpdateSeeNo(string Type, DateTime seeDate, string Subject, string noonID)
        {
            string sql = "";

            #region 更新看诊序号

            if (this.Sql.GetCommonSql("Registration.Register.UpdateSeeSequence", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql, seeDate.Date.ToString(), Type, Subject, noonID);
                int rtn = this.ExecNoQuery(sql);

                if (rtn == -1) return -1;

                //没有更新记录,插入一条新记录
                if (rtn == 0)
                {
                    if (this.Sql.GetCommonSql("Registration.Register.InsertSeeSequence", ref sql) == -1) return -1;

                    sql = string.Format(sql, seeDate.Date.ToString(), Type, Subject, "", 1, noonID);

                    if (this.ExecNoQuery(sql) == -1) return -1;
                }
            }
            catch (Exception e)
            {
                this.Err = "更新看诊序号出错" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
            #endregion
            return 0;
        }

        /// <summary>
        /// 更新看诊序号
        /// </summary>
        /// <param name="Type">1医生 2科室 4全院</param>
        /// <param name="seeDate">看诊日期</param>
        /// <param name="Subject">Type=1时,医生代码;Type=2,科室代码;Type=4,ALL</param>
        /// <param name="noonID">午别</param>
        /// <returns></returns>
        public int SetSeeNo(string Type, DateTime seeDate, string Subject, string noonID, int curnub)
        {
            string sql = "";

            #region 更新看诊序号
            try
            {
                if (this.Sql.GetCommonSql("Registration.Register.SetSeeSequence", ref sql) == -1) return -1;

                sql = string.Format(sql, seeDate.Date.ToString(), Type, Subject, noonID, curnub);

                if (this.ExecNoQuery(sql) == -1) return -1;

            }
            catch (Exception e)
            {
                this.Err = "更新看诊序号出错" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
            #endregion
            return 0;
        }

        /// <summary>
        /// 获取看诊序号
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="current"></param>
        /// <param name="deptCode"></param>
        /// <param name="docCode"></param>
        /// <param name="noonID"></param>
        /// <param name="endTime"></param>
        /// <param name="isPrv">是否预约号</param>
        /// <param name="seeNo"></param>
        /// <returns></returns>
        public int GetSeeNoNew(string Type, DateTime current, string deptCode, string docCode, string noonID,
                               DateTime endTime, bool isPrv, string schemaNO, DateTime operDate, ref int seeNo)
        {
            string sql = "", rtn = "";
            if (isPrv)
            {
                if (this.Sql.GetCommonSql("Registration.Register.PrvSeeNo", ref sql) == -1) return -1;
            }
            else
            {
                if (this.Sql.GetCommonSql("Registration.Register.LocalSeeNo", ref sql) == -1) return -1;
            }
            try
            {
                sql = string.Format(sql, current.Date.ToString(), noonID, docCode, deptCode, Type, endTime.ToString(),
                                    schemaNO, operDate.ToString());

                rtn = this.ExecSqlReturnOne(sql, "0");

                seeNo = Neusoft.FrameWork.Function.NConvert.ToInt32(rtn);

                return 0;
            }
            catch (Exception e)
            {
                this.Err = "查询看诊序号出错![Registration.Register.SeeNo_New]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }


        /// <summary>
        /// 验证患者卡号是否有效
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="IsValid"></param>
        /// <returns></returns>
        public int IsValidCardNo(string CardNo, ref bool IsValid)
        {
            string sql = @" select count(*) from com_patientinfo a  where a.card_no='{0}'";
            try
            {
                sql = string.Format(sql, CardNo);
                string cnt = this.ExecSqlReturnOne(sql);
                if (cnt == "0")
                    IsValid = false;
                else
                    IsValid = true;

                return 1;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }

        public int GetCardNoByPhone(string patientName, string PhoneNo, ref string CardNo)
        {
            string sql = @" select a.card_no from com_patientinfo a where a.name='{0}' and a.home_tel='{1}' and rownum=1";
            try
            {
                sql = string.Format(sql, patientName, PhoneNo);
                CardNo = this.ExecSqlReturnOne(sql);
                return 1;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                return -1;
            }
        }


        /// <summary>
        /// 获取最大看诊时间
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="noonID"></param>
        /// <param name="docCode"></param>
        /// <param name="maxSeeDate"></param>
        /// <returns></returns>
        public int GetMaxSeeDate(DateTime endTime, string noonID, string docCode, ref string maxSeeDate)
        {
            string sql = "";

            #region //

            if (this.Sql.GetCommonSql("Registration.Register.MaxSeeDate", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql, endTime.ToString(), noonID, docCode);
                maxSeeDate = this.ExecSqlReturnOne(sql);
            }
            catch (Exception e)
            {
                this.Err = "获取最大看诊时间出错！" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
            #endregion
            return 0;
        }


        /// <summary>
        /// 获取退号数量
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="noonID"></param>
        /// <param name="docCode"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public int GetRegCancelCnt(DateTime endTime, string noonID, string docCode, ref int cnt)
        {
            string sql = "";

            #region //

            if (this.Sql.GetCommonSql("Registration.Register.CancelCount", ref sql) == -1) return -1;
            try
            {
                sql = string.Format(sql, endTime.ToString(), noonID, docCode);
                string result = this.ExecSqlReturnOne(sql);

                if (!int.TryParse(result, out cnt))
                {
                    cnt = 0;
                }
            }
            catch (Exception e)
            {
                this.Err = "获取退号数量出错！" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
            #endregion
            return 0;
        }

        /// <summary>
        /// 获得患者看诊序号
        /// </summary>
        /// <param name="Type">Type:1专家序号、2科室序号、4全院序号</param>
        /// <param name="current">看诊日期</param>
        /// <param name="subject">Type=1时,医生代码;Type=2,科室代码;Type=4,ALL</param>
        /// <param name="noonID">午别</param>
        /// <param name="seeNo">当前看诊号</param>
        /// <returns></returns>
        public int GetSeeNo(string Type, DateTime current, string subject, string noonID, ref int seeNo)
        {
            string sql = "", rtn = "";

            if (this.Sql.GetCommonSql("Registration.Register.getSeeNo", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, current.Date.ToString(), Type, subject, noonID);

                rtn = this.ExecSqlReturnOne(sql, "0");

                seeNo = Neusoft.FrameWork.Function.NConvert.ToInt32(rtn);

                return 0;
            }
            catch (Exception e)
            {
                this.Err = "查询看诊序号出错![Registration.Register.getSeeNo]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }



        #endregion

        #region 更新日结数据
        /// <summary>
        /// 根据操作员、时间段更新日结信息
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="OperID"></param>
        /// <param name="BalanceID"></param>
        /// <returns></returns>
        public int Update(DateTime begin, DateTime end, string OperID, string BalanceID)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Update.DayBalance", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, begin.ToString(), end.ToString(), OperID, BalanceID);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "置挂号信息日结标志出错![Registration.Register.Update.DayBalance]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        #endregion

        #region 更新已看诊、已收费标记

        /// <summary>
        /// 更新已看诊、已收费标记
        /// </summary>
        /// <param name="Type">1医生 2科室 4全院</param>
        /// <param name="seeDate">看诊日期</param>
        /// <param name="Subject">Type=1时,医生代码;Type=2,科室代码;Type=4,ALL</param>
        /// <param name="noonID">午别</param>
        /// <returns></returns>
        public int UpdateYNSeeAndCharge(string clinicCode)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.UpdateYNFlag", ref sql) == -1)
                return -1;
            try
            {
                sql = string.Format(sql, clinicCode);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新已看诊、已收费标记" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return 0;
        }

        #endregion

        #region 内镜中心流程改造，保存或预览处方时，更新FIN_OPR_REGISTER的ASSESSS_FLAG评估状态
        // {57D49CC0-3BEE-4168-8E71-4FAE394DF6A6}  内镜中心流程改造
        /// <summary>
        /// 更新评估状态
        /// </summary>
        /// <param name="clinicCode">门诊流水号</param>
        /// <param name="code">评估值:1,待评估，2评估完成</param>
        /// <returns></returns>
        public int UpdateASSESSS_FLAG(string clinicCode, string code)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.UpdateASSESSS_FLAG", ref sql) == -1)
                return -1;
            try
            {
                sql = string.Format(sql, clinicCode, code);
                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新待评估状态失败" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
            return 0;
        }
        #endregion

        #region 更新com_patientinfo时更新挂号表

        /// <summary>
        /// 修改患者基本信息时，更新挂号部分信息 根据clinicCode
        /// </summary>
        /// <param name="patientInfo">患者基本信息实体</param>
        /// <returns></returns>
        public int UpdateRegInfoByClinicCode(Neusoft.HISFC.Models.Registration.Register patientInfo)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.UpdateRegByClinicNo", ref sql) == -1)
                return -1;
            try
            {
                sql = string.Format(sql,
                                patientInfo.ID,
                                patientInfo.Name,
                                patientInfo.Sex.ID,
                                patientInfo.Birthday,
                                patientInfo.IDCard,
                                patientInfo.Pact.PayKind.ID,
                                patientInfo.Pact.PayKind.Name,
                                patientInfo.Pact.ID,
                                patientInfo.Pact.Name
                                );
                this.ExecNoQuery(sql);
                return 1;
            }
            catch (Exception e)
            {
                this.Err = "挂号信息失败：" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// 修改患者基本信息时，更新挂号相关信息
        /// </summary>
        /// <param name="patientInfo">患者基本信息实体</param>
        /// <returns></returns>
        public int UpdateRegByPatientInfo(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.UpdateRegByPatientInfo", ref sql) == -1)
                return -1;
            try
            {
                sql = string.Format(sql,

                                patientInfo.PID.CardNO,
                                patientInfo.Name, patientInfo.Sex.ID,
                                patientInfo.Birthday,
                                patientInfo.IDCard,
                                patientInfo.Pact.PayKind.ID,
                                patientInfo.Pact.PayKind.Name,
                                patientInfo.Pact.ID,
                                patientInfo.Pact.Name,
                                patientInfo.PatientType
                                );

                this.ExecNoQuery(sql);
                return 1;
            }
            catch (Exception e)
            {
                this.Err = "挂号信息失败：" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }

            return 0;
        }

        #endregion

        #region 更新挂号主表，看诊医生所属科室
        /// <summary>
        ///  更新挂号主表，看诊医生所属科室 根据clinicCode
        /// </summary>
        /// <param name="patientInfo">患者信息</param>
        /// <param name="DocInDept">医生所属科室</param>
        /// <param name="DocInDept">是否根据Ynsee(看诊标志)来更新</param>
        /// <returns></returns>
        public int UpdateRegDocInDeptByClinicCode(Neusoft.HISFC.Models.Registration.Register patientInfo, string DocInDept, bool isUseYnSee)
        {
            string sql = "";
            if (isUseYnSee)
            {
                if (this.Sql.GetCommonSql("Registration.Register.UpdateRegInfoDocInDeptByClinicCode", ref sql) == -1)
                    return -1;
            }
            else
            {
                if (this.Sql.GetCommonSql("Registration.Register.UpdateRegInfoDocInDept", ref sql) == -1)
                    return -1;
            }


            try
            {
                sql = string.Format(sql,
                                DocInDept,
                                patientInfo.ID
                                );

                this.ExecNoQuery(sql);
                return 1;
            }
            catch (Exception e)
            {
                this.Err = "更新看诊医生所属科室失败：" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        #endregion

        #endregion

        #region 自动取卡号
        /// <summary>
        /// 取数据库序列值来作为就诊卡号
        /// </summary>
        /// <returns>序列值</returns>
        public int AutoGetCardNO()
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.GetNewCardNo", ref sql) == -1) return -1;

            try
            {
                return Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));
            }
            catch (Exception e)
            {
                this.Err = "自动取卡号出错![Registration.Register.GetNewCardNo]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        /// <summary>
        /// 取数据库序列值来作为就诊卡号（用于自助挂号）
        /// </summary>
        /// <returns>序列值</returns>
        public long AutoGetCardNOForSelfHelpReg()
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.GetNewCardNo.SelfHelpReg", ref sql) == -1) return -1;

            try
            {
                return Convert.ToInt64(this.ExecSqlReturnOne(sql));
            }
            catch (Exception e)
            {
                this.Err = "自动取卡号出错![Registration.Register.GetNewCardNo.SelfHelpReg]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        #endregion

        #region 诊间使用
        #region 更新已经看诊

        /// <summary>
        ///  更新已经看诊－－根据门诊流水号
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public int UpdateSeeDone(string clinicNo)
        {
            string sql = "Registration.Register.Update.SeeDone";
            if (this.Sql.GetCommonSql(sql, ref sql) == -1) return -1;
            return this.ExecNoQuery(sql, clinicNo);
        }

        #endregion

        #region 更新看诊科室
        /// <summary>
        /// 更新看诊科室
        /// </summary>
        /// <param name="clinicID"></param>
        /// <param name="seeDeptID"></param>
        /// <param name="seeDoctID"></param>
        /// <returns></returns>
        public int UpdateDept(string clinicID, string seeDeptID, string seeDoctID)
        {
            string sql = "";
            string[] parm = new string[] { clinicID, seeDeptID, seeDoctID };

            if (this.Sql.GetCommonSql("Registration.Register.Query.17", ref sql) == -1) return -1;

            return this.ExecNoQuery(sql, parm);
        }
        #endregion



        #endregion


        #region 按病历号查询一条最近的挂号信息,屏蔽

        /// <summary>
        /// 根据病历号查询患者最近一次挂号信息
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.Register Query(string cardNo)
        {
            ArrayList al = this.QueryRegListBase("Registration.Register.Query.2", cardNo);

            if (al == null)
            {
                return null;
            }
            else if (al.Count == 0)
            {
                return new Neusoft.HISFC.Models.Registration.Register();
            }
            else
            {
                return (Neusoft.HISFC.Models.Registration.Register)al[0];
            }
        }

        #endregion
        /// <summary>
        /// 根据卡号及日期查询当天最新一条挂号记录
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.Register QueryByCardNoAndDate(string cardNo, string dt)
        {
            string where = @" WHERE card_no='{0}'
   AND to_char(reg_date, 'yyyy-mm-dd') = '{1}' order by reg_date desc";
            where = string.Format(where, cardNo, dt);
            ArrayList al = this.QueryRegListBase(where);

            if (al == null)
            {
                return null;
            }
            else if (al.Count == 0)
            {
                return new Neusoft.HISFC.Models.Registration.Register();
            }
            else
            {
                return (Neusoft.HISFC.Models.Registration.Register)al[0];
            }
        }

        #region 按患者姓名从挂号表查询患者信息
        public ArrayList QueryRegisterByName(string name)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.QueryByName", ref sql) == -1) return null;

            sql = string.Format(sql, name);

            if (this.ExecQuery(sql) == -1) return null;

            ArrayList al = new ArrayList();

            try
            {
                while (this.Reader.Read())
                {
                    this.reg = new Neusoft.HISFC.Models.Registration.Register();

                    reg.PID.CardNO = this.Reader[0].ToString();
                    reg.Name = this.Reader[1].ToString();
                    reg.IDCard = this.Reader[2].ToString();
                    reg.Sex.ID = this.Reader[3].ToString();
                    reg.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4].ToString());
                    reg.PhoneHome = this.Reader[5].ToString();
                    reg.AddressHome = this.Reader[6].ToString();
                    reg.DoctorInfo.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[7].ToString());

                    al.Add(reg);
                }

                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "检索患者基本信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return al;

        }
        #endregion

        #region 按患者名称查询患者基本信息
        /// <summary>
        /// 根据患者姓名查询
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public ArrayList QueryByName(string Name)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.10", ref sql) == -1) return null;

            sql = string.Format(sql, Name);

            if (this.ExecQuery(sql) == -1) return null;

            ArrayList al = new ArrayList();

            try
            {
                while (this.Reader.Read())
                {
                    this.reg = new Neusoft.HISFC.Models.Registration.Register();

                    reg.PID.CardNO = this.Reader[0].ToString();
                    reg.Name = this.Reader[1].ToString();
                    reg.IDCard = this.Reader[2].ToString();
                    reg.Sex.ID = this.Reader[3].ToString();
                    reg.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[4].ToString());
                    reg.PhoneHome = this.Reader[5].ToString();
                    reg.AddressHome = this.Reader[6].ToString();

                    al.Add(reg);
                }

                this.Reader.Close();
            }
            catch (Exception e)
            {
                this.Err = "检索患者基本信息出错!" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return al;
        }
        #endregion

        public ArrayList GetByIDCard(string IDCard)
        {
            return this.QueryRegListBase("Registration.Register.Query.IDCard", IDCard);
        }

        #region 按门诊号查询一条挂号信息
        /// <summary>
        /// 按门诊流水号查询挂号信息
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.Register GetByClinic(string clinicNo)
        {
            ArrayList al = this.QueryRegListBase("Registration.Register.Query.4", clinicNo);

            if (al == null)
            {
                return null;
            }
            else if (al.Count == 0)
            {
                return new Neusoft.HISFC.Models.Registration.Register();
            }
            else
            {
                return (Neusoft.HISFC.Models.Registration.Register)al[0];
            }
        }

        #endregion

        #region 按处方号查询一条挂号信息
        /// <summary>
        /// 按处方号查询
        /// </summary>
        /// <param name="recipeNo"></param>
        /// <returns></returns>
        public ArrayList QueryByRecipe(string recipeNo)
        {
            string sql = "", where = "";
            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.14", ref where) == -1) return null;

            try
            {
                where = string.Format(where, recipeNo);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.14]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);

        }
        #endregion

        //{B6E76F4C-1D79-4fa2-ABAD-4A22DE89A6F7}
        #region 根据发票号查询挂号信息
        /// <summary>
        /// 根据发票号查询挂号信息
        /// </summary>
        /// <param name="recipeNo"></param>
        /// <returns></returns>
        public ArrayList QueryByRegInvoice(string invoiceNo)
        {
            string sql = "", where = "";
            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.22", ref where) == -1) return null;

            try
            {
                where = string.Format(where, invoiceNo);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.22]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);

        }

        /// <summary>
        /// add by lijp 2012-08-24
        /// 根据发票号查询一段时间内患者的有效挂号信息
        /// </summary>
        /// <param name="recipeNo"></param>
        /// <returns></returns>
        public ArrayList QueryByRegInvoice(string invoiceNo, DateTime limitDate)
        {
            string sql = "", where = "";
            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.24", ref where) == -1)
            {
                this.Err = "SQL语句没有找到：Registration.Register.Query.24";
                return null;
            }

            try
            {
                where = string.Format(where, invoiceNo, limitDate.ToString());
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.24]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);

        }

        #endregion

        #region 按照病历号，医保类别（大类），时间有效查询挂号信息
        /// <summary>
        ///  按照病历号，医保类别（大类），时间有效查询挂号信息{46F865E4-9B79-4cc6-814D-3847DDBC85F9}
        /// </summary>
        /// <param name="cardNO"></param>
        /// <param name="beginDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="payKindCode"></param>
        /// <returns></returns>
        public ArrayList QueryRegInfo(string cardNO, string beginDateTime, string EndDateTime, string payKindCode)
        {
            string sql = "", where = "";
            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.23", ref where) == -1) return null;

            try
            {
                where = string.Format(where, cardNO, beginDateTime, EndDateTime, payKindCode);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.23]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);

        }
        #endregion

        #region 按病历号、开始时间查询患者的挂号信息

        public ArrayList QueryRegListBase(string whereSQL)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1)
            {
                return null;
            }

            sql = sql + "\r\n" + whereSQL;

            return this.QueryRegister(sql);
        }

        private ArrayList QueryRegListBase(string whereSQLIndex, params string[] args)
        {
            string where = "";

            if (this.Sql.GetCommonSql(whereSQLIndex, ref where) == -1)
            {
                return null;
            }

            try
            {
                where = string.Format(where, args);
            }
            catch (Exception e)
            {
                this.Err = "[" + whereSQLIndex + "]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            return QueryRegListBase(where);
        }

        /// <summary>
        /// 按照病历号查询一段时间内的挂号记录
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="valide">是否有效 1 有效；0 退费；2 作废； 其他 全部记录</param>
        /// <returns></returns>
        public ArrayList QueryRegList(string cardNo, DateTime beginDate, DateTime endDate, string valide)
        {
            if (valide != "1" && valide != "0" && valide != "2")
            {
                valide = "All";
            }

            return this.QueryRegListBase("Registration.Register.Query.ByDateAndState", cardNo, beginDate.ToString(), endDate.ToString(), valide);
        }

        /// <summary>
        /// 按照病历号查询一段时间内的挂号记录
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="valide">是否有效 1 有效；0 退费；2 作废； 其他 全部记录</param>
        /// <returns></returns>
        public ArrayList QueryRegListWithHosCode(string cardNo, DateTime beginDate, DateTime endDate, string valide, string hosCode)
        {
            if (valide != "1" && valide != "0" && valide != "2")
            {
                valide = "All";
            }

            return this.QueryRegListBase("Registration.Register.Query.ByDateAndState.HosCode", cardNo, beginDate.ToString(), endDate.ToString(), valide, hosCode);
        }

        /// <summary>
        /// 查询患者一段时间内挂的有效号
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList Query(string cardNo, DateTime limitDate)
        {
            return this.QueryRegListBase("Registration.Register.Query.3", cardNo, limitDate.ToString());
        }

        /// <summary>
        /// 查询患者一段时间内挂的有效号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList QueryName(string name, DateTime limitDate)
        {
            return this.QueryRegListBase("Registration.Register.Query.25", name, limitDate.ToString());
        }

        /// <summary>
        /// 查询患者一段时间内挂的有效号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList QueryWitHosCode(string name, DateTime limitDate, string hosCode)
        {
            return this.QueryRegListBase("Registration.Register.Query.HosCode.3", name, limitDate.ToString(), hosCode);

        }

        /// <summary>
        /// 查询患者一段时间内挂的有效号
        /// 中大五院预约支付后可直接看诊，经常出现医生今天调了后天的挂号记录看诊情况
        /// 故增加改方法限制：1、普通门诊 在门诊医生站只允许调取当天的挂号记录 2、急诊科室允许调取挂号日期后24小时内有效（当前日期减去24小时）
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="limitDate">挂号开始时间</param>
        /// <param name="dtEnd">挂号结束时间</param>
        /// <param name="hosCode">医院编码</param>
        /// <returns></returns>
        public ArrayList QueryWitHosCode(string cardNo, DateTime limitDate, DateTime dtEnd, string hosCode)
        {
            return this.QueryRegListBase("Registration.Register.Query.HosCode.3.1", cardNo, limitDate.ToString(), dtEnd.ToString(), hosCode);

        }

        /// <summary>
        /// 放射诊断科查询患者一段时间内挂的有效号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList QueryWitHosCodeFSZD(string cardNo, DateTime limitDate, DateTime dtEnd, string hosCode)
        {
            return this.QueryRegListBase("Registration.Register.Query.HosCode.3.2", cardNo, limitDate.ToString(), dtEnd.ToString(), hosCode);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList QueryUnionNurse(string cardNo, DateTime limitDate)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.20", ref where) == -1) return null;

            try
            {
                where = string.Format(where, cardNo, limitDate.ToString());
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.20]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }
        /// <summary>
        /// 查询一段时间内作废挂号信息
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList QueryCancel(string cardNo, DateTime limitDate)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.16", ref where) == -1) return null;

            try
            {
                where = string.Format(where, cardNo, limitDate.ToString());
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.16]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }
        /// <summary>
        /// 根据病历号查询已看诊的有效挂号信息
        /// </summary>
        /// <param name="cardNO">病历号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结算时间</param>
        public ArrayList GetRegisterByCardNODate(string cardNO, DateTime beginDate, DateTime endDate)
        {
            //Registration.Register.Query.Where
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.Where", ref where) == -1) return null;

            try
            {
                where = string.Format(where, cardNO, beginDate.ToString(), endDate.ToString());
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.Where]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 根据病历号查询已看诊的有效挂号信息
        /// </summary>
        /// <param name="cardNO">病历号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结算时间</param>
        public ArrayList GetRegisterByCardNOAndSIRegNo(string clinicNo, string diagFeeRegCode)
        {
            //Registration.Register.Query.Where
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.GetRegisterByCardNOAndSIRegNo", ref where) == -1) return null;

            try
            {
                where = string.Format(where, clinicNo, diagFeeRegCode);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.GetRegisterByCardNOAndSIRegNo]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        #endregion

        #region 按患者费用执行科室查询挂号信息
        /// <summary>
        /// 按患者费用执行科室查询挂号信息
        /// 
        /// {FCC85123-05D4-4baa-AB14-3DB983608766}
        /// </summary>
        /// <param name="excuDeptID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ArrayList QueryRegisterByFeeExcuDept(string excuDeptID, string beginDate, string endDate)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.QueryRegisterByFeeExcuDept", ref where) == -1) return null;

            try
            {
                where = string.Format(where, beginDate, endDate, excuDeptID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.QueryRegisterByFeeExcuDept]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }
        #endregion

        #region 按患者费用执行科室查询挂号信息--按挂号时间
        /// <summary>
        /// 按患者费用执行科室查询挂号信息
        /// 
        /// {FCC85123-05D4-4baa-AB14-3DB983608766}
        /// </summary>
        /// <param name="excuDeptID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ArrayList QueryRegisterByFeeExcuDeptOrderByRegDate(string excuDeptID, string beginDate, string endDate)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.QueryRegisterByFeeExcuDeptOrderByRegDate", ref where) == -1) return null;

            try
            {
                where = string.Format(where, beginDate, endDate, excuDeptID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.QueryRegisterByFeeExcuDept]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }
        #endregion

        /// <summary>
        /// 按患者费用最小费用挂号信息
        /// 
        /// {FCC85123-05D4-4baa-AB14-3DB983608766}
        /// </summary>
        /// <param name="excuDeptID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ArrayList QueryRegisterByMinFeeOrderByRegDate(string minFee, string beginDate, string endDate)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.QueryRegisterByMinFeeOrderByRegDate", ref where) == -1) return null;

            try
            {
                where = string.Format(where, beginDate, endDate, minFee);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.QueryRegisterByMinFeeOrderByRegDate]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        #region 按患者费用执行科室和卡号查询挂号信息
        /// <summary>
        /// 按患者费用执行科室和卡号查询挂号信息
        /// 
        /// {FCC85123-05D4-4baa-AB14-3DB983608766}
        /// </summary>
        /// <param name="excuDeptID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ArrayList QueryRegisterByFeeExcuDeptAndCardNo(string excuDeptID, string beginDate, string endDate, string CardNo)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.QueryRegisterByFeeExcuDeptAndCardNo", ref where) == -1) return null;

            try
            {
                where = string.Format(where, beginDate, endDate, excuDeptID, CardNo);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.QueryRegisterByFeeExcuDeptAndCardNo]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 按患者费用执行科室和卡号查询挂号信息--按挂号时间
        /// 
        /// {FCC85123-05D4-4baa-AB14-3DB983608766}
        /// </summary>
        /// <param name="excuDeptID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ArrayList QueryRegisterByFeeExcuDeptAndCardNoOrderByRegDate(string excuDeptID, string beginDate, string endDate, string CardNo)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.QueryRegisterByFeeExcuDeptAndCardNoOrderByRegDate", ref where) == -1) return null;

            try
            {
                where = string.Format(where, beginDate, endDate, excuDeptID, CardNo);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.QueryRegisterByFeeExcuDeptAndCardNoOrderByFeeDate]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 按患者最小费用和卡号查询挂号信息--按挂号时间
        /// 
        /// {FCC85123-05D4-4baa-AB14-3DB983608766}
        /// </summary>
        /// <param name="excuDeptID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ArrayList QueryRegisterByMinFeeAndCardNoOrderByRegDate(string excuDeptID, string beginDate, string endDate, string CardNo)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.QueryRegisterByMinFeeAndCardNoOrderByRegDate", ref where) == -1) return null;

            try
            {
                where = string.Format(where, beginDate, endDate, excuDeptID, CardNo);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.QueryRegisterByMinFeeAndCardNoOrderByRegDate]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        #endregion



        #region 按看诊序号查询患者挂号信息 门诊收费使用
        /// <summary>
        /// 按看诊序号、开始时间查询挂号信息
        /// </summary>
        /// <param name="seeNo"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList QueryBySeeNo(string seeNo, DateTime limitDate)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.18", ref where) == -1) return null;

            try
            {
                where = string.Format(where, seeNo, limitDate.ToString());
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.18]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }
        #endregion

        /// <summary>
        /// 检验是否院内职工，根据身份证号码
        /// </summary>
        /// <param name="IdenNO">身份者号码</param>
        /// <returns></returns>
        public bool CheckIsEmployee(string IdenNO)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Registration.Register.CheckIsEmployee", ref sql) == -1)
            {
                this.Err += "没有找到索引为:Registration.Register.CheckIsEmployee 的SQL语句";
                return false;
            }
            try
            {
                sql = string.Format(sql, IdenNO);
            }
            catch (Exception e)
            {
                this.Err = "查找sql语句失败[Registration.Register.CheckIsEmployee]" + e.Message;
                this.ErrCode = e.Message;
                return false;
            }

            int count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));

            if (count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断本院职工当天是否已经挂过免费号
        /// </summary>
        /// <param name="idenNO"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CheckIsEmployeeAndHaveFreeRegInfo(string idenNO, string name)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Registration.Register.CheckIsEmployeeAndHaveFreeRegInfo", ref sql) == -1)
            {
                this.Err += "没有找到索引为:Registration.Register.CheckIsEmployeeAndHaveFreeRegInfo 的SQL语句";
                return false;
            }
            try
            {
                sql = string.Format(sql, idenNO, name);
                int count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql, "0"));
                if (count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                this.Err = "查找sql语句失败[Registration.Register.CheckIsEmployeeAndHaveFreeRegInfo]" + e.Message;
                this.ErrCode = e.Message;
                return false;
            }
        }
        /// <summary>
        /// 判断公医公费挂号次数
        /// </summary>
        /// <param name="idenNO"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CheckIsEmployeeAndHaveFreeRegInfoGY(string idenNO, string name)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Registration.Register.CheckIsEmployeeAndHaveFreeRegInfogy", ref sql) == -1)
            {
                this.Err += "没有找到索引为:Registration.Register.CheckIsEmployeeAndHaveFreeRegInfogy 的SQL语句";
                return false;
            }
            try
            {
                sql = string.Format(sql, idenNO, name);
                int count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql, "0"));
                if (count >=2)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                this.Err = "查找sql语句失败[Registration.Register.CheckIsEmployeeAndHaveFreeRegInfo]" + e.Message;
                this.ErrCode = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 检验是否院内职工，根据身份证号码
        /// </summary>
        /// <param name="IdenNO">身份者号码</param>
        /// <returns></returns>
        public bool CheckIsEmployee(Neusoft.HISFC.Models.Registration.Register register)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Registration.Register.CheckIsEmployeeByClinicNO", ref sql) == -1)
            {
                return this.CheckIsEmployee(register.IDCard);
            }
            try
            {
                sql = string.Format(sql, register.ID);
            }
            catch (Exception e)
            {
                this.Err = "查找sql语句失败[Registration.Register.CheckIsEmployeeByClinicNO]" + e.Message;
                this.ErrCode = e.Message;
                return false;
            }

            int count = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));

            if (count > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 按时间段统计查询挂号员的有效挂号数
        /// </summary>
        /// <param name="operID">挂号员id</param>
        /// <param name="beginDateTime">起始时间</param>
        /// <param name="endDateTime">截至时间</param>
        /// <returns></returns>
        public string QueryValidRegNumByOperAndOperDT(string operID, string beginDateTime, string endDateTime)
        {
            string sql = string.Empty;
            if (this.Sql.GetCommonSql("Registration.QueryValidRegNumByOperAndOperDT.Select1", ref sql) == -1)
            {
                this.Err += "没有找到索引为:Registration.QueryValidRegNumByOperAndOperDT.Select1 的SQL语句";
                return "-1";
            }
            try
            {
                sql = string.Format(sql, operID, beginDateTime, endDateTime);
            }
            catch (Exception e)
            {
                this.Err = "组成sql语句失败[Registration.QueryValidRegNumByOperAndOperDT.Select1]" + e.Message;
                this.ErrCode = e.Message;
            }

            return this.ExecSqlReturnOne(sql);
        }

        #region 按操作员、时间段查询挂号信息
        /// <summary>
        /// 按操作员、时间段查询挂号信息
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="operID"></param>
        /// <returns></returns>
        public ArrayList Query(DateTime beginDate, DateTime endDate, string operID)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.9", ref where) == -1) return null;

            try
            {
                where = string.Format(where, beginDate.ToString(), endDate.ToString(), operID);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.9]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }
        #endregion

        /// <summary>
        /// 查询复诊记录
        /// </summary>
        /// <param name="cardNO"></param>
        /// <returns></returns>
        public int QueryRegiterByCardNO(string cardNO)
        {
            string sql = string.Empty;
            int returnValue = Sql.GetCommonSql("Registration.QueryRegiterByCardNO.Select.1", ref sql);
            if (returnValue == -1)
            {
                return -1;
            }
            try
            {
                sql = string.Format(sql, cardNO);
            }
            catch (Exception e)
            {
                this.Err = "[Registration.QueryRegiterByCardNO.Select.1]出错" + e.Message;
                return -1;

            }


            int result = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));

            return result;
        }

        #region 查询一段时间内未分诊的挂号患者 门诊护士使用
        /// <summary>
        /// 查询一段时间内未分诊的挂号患者
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public ArrayList QueryNoTriage(DateTime begin)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.5", ref where) == -1) return null;

            try
            {
                where = string.Format(where, begin.ToString());
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.5]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }
        #endregion

        #region 分诊
        /// <summary>
        /// 通过一段时间内 某护理站对应科室的挂号患者 addby sunxh
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="myNurseDept">护理站代码</param>
        /// <returns></returns>
        public ArrayList QueryNoTriagebyDept(DateTime begin, string myNurseDept)
        {

            string sql = ""; string where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.byNurseDept", ref where) == -1) return null;

            where = string.Format(where, begin.ToString(), myNurseDept);

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 通过一段时间内 某护理站的挂号患者{F044FCF3-6736-4aaa-AA04-4088BB194C20}
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="myNurseDept">护理站代码</param>
        /// <returns></returns>
        public ArrayList QueryNoTriagebyNurse(DateTime begin, string NurseID)
        {
            string sql = ""; string where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.byNurseID", ref where) == -1) return null;

            where = string.Format(where, begin.ToString(), NurseID);

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 通过一段时间内 某护理站对应科室的挂号患者未看诊 addby niuxy
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="myNurseDept">护理站代码</param>
        /// <returns></returns>
        public ArrayList QueryNoTriagebyDeptUnSee(DateTime begin, string myNurseDept)
        {
            string sql = ""; string where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.byNurseDept1", ref where) == -1) return null;

            where = string.Format(where, begin.ToString(), myNurseDept);

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 根据门诊号判断挂号信息是否分诊
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public bool QueryIsTriage(string clinicNo)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.IsTriage", ref sql) == -1) return false;

            try
            {
                sql = string.Format(sql, clinicNo);

                string rtn = this.ExecSqlReturnOne(sql, "0");

                // return Neusoft.FrameWork.Function.NConvert.ToBoolean(rtn) ;
                if (rtn == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.IsTriage]" + e.Message;
                this.ErrCode = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 根据门诊号判断挂号信息是否作废
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public bool QueryIsCancel(string clinicNo)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.IsCancel", ref sql) == -1) return false;

            try
            {
                sql = string.Format(sql, clinicNo);

                string rtn = this.ExecSqlReturnOne(sql, "0");

                if (rtn == "1")
                {
                    return false;//有效,未作废
                }
                else
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.IsCancel]" + e.Message;
                this.ErrCode = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 查询患者有效挂号记录
        /// 不包括进诊和诊出状态
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList QueryUnionNurseTriage(string cardNo, DateTime limitDate)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.ByInTriage", ref where) == -1) return null;

            try
            {
                where = string.Format(where, cardNo, limitDate.ToString());
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.ByInTriage]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 获得患者看诊序号
        /// </summary>
        /// <param name="Type">Type:1专家序号、2科室序号、4全院序号</param>
        /// <param name="current">看诊日期</param>
        /// <param name="subject">Type=1时,医生代码;Type=2,科室代码;Type=4,ALL</param>
        /// <param name="noonID">午别</param>
        /// <param name="seeNo">当前看诊号</param>
        /// <returns></returns>
        public int GetSeeNo(string Type, DateTime current, string subject, string noonID, ref string seeNo)
        {
            string sql = "", rtn = "";

            if (this.Sql.GetCommonSql("Registration.Register.getSeeNo", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, current.Date.ToString(), Type, subject, noonID);

                rtn = this.ExecSqlReturnOne(sql, "0");

                seeNo = rtn;

                return 0;
            }
            catch (Exception e)
            {
                this.Err = "查询看诊序号出错![Registration.Register.getSeeNo]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

        #endregion

        #region 查询公费患者某日挂号数量
        /// <summary>
        /// 查询公费患者某日挂号数量
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="regDate"></param>
        /// <returns></returns>
        public int QuerySeeNum(string cardNo, DateTime regDate)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.12", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, cardNo, regDate.Date.ToString(), regDate.Date.AddDays(1).ToString());
                string Cnt = this.ExecSqlReturnOne(sql, "0");

                return Neusoft.FrameWork.Function.NConvert.ToInt32(Cnt);
            }
            catch (Exception e)
            {
                this.Err = "获得患者挂号数量出错![Registration.Register.Query.12]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        #endregion

        #region 按门诊号查询已打印发票数量
        /// <summary>
        /// 按门诊号查询已打印发票数量
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public int QueryPrintedInvoiceCnt(string clinicNo)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.15", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, clinicNo);
                string Cnt = this.ExecSqlReturnOne(sql, "0");

                return Neusoft.FrameWork.Function.NConvert.ToInt32(Cnt);
            }
            catch (Exception e)
            {
                this.Err = "获得患者打印发票数量出错![Registration.Register.Query.15]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }

        /// <summary>
        /// 按门诊号更新已打印发票数量
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public int UpdatePrintInvoiceCnt(string clinicNo)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Update.InvoiceCnt", ref sql) == -1) return -1;

            try
            {
                sql = string.Format(sql, clinicNo);

                return this.ExecNoQuery(sql);
            }
            catch (Exception e)
            {
                this.Err = "更新患者打印发票数量出错![Registration.Register.Update.InvoiceCnt]" + e.Message;
                this.ErrCode = e.Message;
                return -1;
            }
        }
        #endregion

        #region 共有查询

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
                    if (Reader.FieldCount > 75)
                    {
                        reg.Greenway = Reader[75].ToString();
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

        /// <summary>
        /// 查询医保上传日志表中的合同单位代码
        /// </summary>
        /// <param name="clinicCode"></param>
        /// <returns></returns>
        public string GetPactCodeFoMedcare(string clinicCode)
        {
            string defaultsql = "select pact_code from fin_ipr_sirecord where clinic_code='{0}'";
            string sql = "";
            if (this.Sql.GetCommonSql("Registration.Register.GetPactCodeFoMedcare.1", ref sql) == -1)
            {
                sql = defaultsql;
            }
            try
            {
                sql = string.Format(sql, clinicCode);
            }
            catch (Exception e)
            {
                this.Err = "Registration.Register.GetPactCodeFoMedcare.1" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }
            if (this.ExecQuery(sql) == -1) return null;
            return ExecSqlReturnOne(sql);
        }
        #endregion

        #region 门诊医生站使用查询

        /// <summary>
        /// 按挂号医生查询某一段时间内挂的有效号
        /// </summary>
        /// <param name="doctID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isSee"></param>
        /// <returns></returns>
        public ArrayList QueryByDoct(string doctID, DateTime beginDate, DateTime endDate, bool isSee)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.7", ref where) == -1) return null;

            try
            {
                where = string.Format(where, doctID, beginDate.ToString(), endDate.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isSee));
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.7]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 按挂号科室查询某一段时间内挂的有效号
        /// </summary>
        /// <param name="deptID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isSee"></param>
        /// <returns></returns>
        public ArrayList QueryByDept(string deptID, DateTime beginDate, DateTime endDate, bool isSee)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.8", ref where) == -1) return null;

            try
            {
                where = string.Format(where, deptID, beginDate.ToString(), endDate.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isSee));
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.8]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 按看诊医生查询某一段时间内挂的有效号
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isSee"></param>
        /// <returns></returns>
        public ArrayList QueryBySeeDoc(string docID, DateTime beginDate, DateTime endDate, bool isSee)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.19", ref where) == -1) return null;

            try
            {
                where = string.Format(where, docID, beginDate.ToString(), endDate.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isSee));
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.19]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 按看诊医生查询某一段时间内已经看诊的有效号
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isSee"></param>
        /// <returns></returns>
        public ArrayList QueryBySeeDocAndSeeDate(string docID, DateTime beginDate, DateTime endDate, bool isSee)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.21", ref where) == -1) return null;

            try
            {
                where = string.Format(where, docID, beginDate.ToString(), endDate.ToString(), Neusoft.FrameWork.Function.NConvert.ToInt32(isSee));
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.21]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        #endregion

        #region
        /// <summary>
        /// 查询注射室患者信息
        /// </summary>
        /// <param name="cardNo">卡号，为空时表示查询全部</param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isPrint">是否打印</param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int QueryInject(string cardNo, DateTime beginTime, DateTime endTime, bool isPrint, string dept, string unDrugUsage, string drugUsage, ref System.Data.DataSet ds)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Registration.Register.Query.Inject", ref sql) == -1)
            {
                ds = null;
                return -1;
            }

            try
            {
                if (isPrint)
                {
                    //已打印的限定  999999>打印次数>=1
                    sql = string.Format(sql, beginTime.ToString(), endTime.ToString(), cardNo.Trim(), 1, 9999999, dept, unDrugUsage, drugUsage);
                }
                else
                {
                    //未打印的限定  1>打印次数>=0
                    sql = string.Format(sql, beginTime.ToString(), endTime.ToString(), cardNo.Trim(), 0, 1, dept, unDrugUsage, drugUsage);
                }
            }
            catch (Exception e)
            {
                this.Err = "[Registration.Register.Query.Inject]" + e.Message;
                this.ErrCode = e.Message;
                ds = null;
                return -1;
            }

            return this.ExecQuery(sql, ref ds);
        }

        #endregion



        #region 按照姓名查询具有划价信息的患者
        /// <summary>
        /// 按照姓名查询具有划价信息的患者
        /// </summary>
        /// <param name="name" >姓名</param>
        /// <param name="days ">有效天数</param>
        /// <returns></returns>
        public ArrayList QueryRegHaveChargedInfo(string name, int days)
        {
            string strSql = "";

            ArrayList al = new ArrayList();

            if (this.Sql.GetCommonSql("Registration.Register.Query.HaveChargedInfo", ref strSql) == -1)
            {
                this.Err = "Can't Find Sql:Registration.Register.Query.HaveChargedInfo";
                return null;
            }
            strSql = System.String.Format(strSql, name, days);
            if (this.ExecQuery(strSql) < 0)
            {
                this.Err = "Execute Err;";
                return null;
            }

            while (this.Reader.Read())
            {
                this.reg = new Neusoft.HISFC.Models.Registration.Register();

                reg.ID = this.Reader[0].ToString();//流水号
                reg.PID.CardNO = this.Reader[1].ToString();//病利号
                reg.OrderNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[2].ToString());//方号
                reg.Name = this.Reader[3].ToString();//姓名
                reg.DoctorInfo.Templet.Dept.ID = this.Reader[4].ToString();
                reg.DoctorInfo.Templet.Dept.Name = this.Reader[5].ToString();//挂号科室
                reg.DoctorInfo.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[6].ToString());
                reg.Sex.ID = this.Reader[7].ToString();
                reg.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(this.Reader[8].ToString());//出生日期
                reg.Pact.ID = this.Reader[9].ToString();
                reg.Pact.Name = this.Reader[10].ToString();//合同单位
                reg.DoctorInfo.Templet.Doct.ID = this.Reader[11].ToString();
                reg.DoctorInfo.Templet.Doct.Name = this.Reader[12].ToString();//挂号医生
                reg.SSN = this.Reader[13].ToString();//医疗证号
                reg.DoctorInfo.Templet.RegLevel.ID = this.Reader[14].ToString();
                reg.DoctorInfo.Templet.RegLevel.Name = this.Reader[15].ToString();

                al.Add(reg);
            }
            this.Reader.Close();
            return al;
        }
        #endregion


        #region 按护士站和急诊留观状态查询患者列表
        /// <summary>
        /// 按护士站和急诊留观状态查询患者列表
        /// </summary>
        /// <param name="nurseCellCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ArrayList PatientQueryByNurseCell(string nurseCellCode, string status)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.byNurseCellCode", ref where) == -1) return null;

            where = string.Format(where, nurseCellCode, status);

            sql = sql + " " + where;

            return this.QueryRegister(sql);

        }

        //{1C0814FA-899B-419a-94D1-789CCC2BA8FF}
        /// <summary>
        /// 医生站加载留观患者信息
        /// </summary>
        /// <param name="deptCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ArrayList PatientQueryByNurseCell(string deptCode)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.QueryEnEmergencyPatient.byDeptCode", ref where) == -1) return null;

            where = string.Format(where, deptCode);

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }

        #endregion

        #region 麻醉评估列表
        // {57D49CC0-3BEE-4168-8E71-4FAE394DF6A6}
        /// <summary>
        /// 门诊医生站加载评估列表
        /// </summary>
        /// <param name="assessState"></param>
        /// <returns></returns>
        public ArrayList AssessPatientQueryByDeptCode(string assessState)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.QueryAssessPatient.byState", ref where) == -1) return null;

            where = string.Format(where, assessState);

            sql = sql + " " + where;

            return this.QueryRegister(sql);
        }
        #endregion

        #region 按护士站和急诊留观状态查询患者列表

        /// <summary>
        /// 按科室查询和急诊留观状态查询患者列表
        /// </summary>
        /// <param name="nurseCellCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ArrayList QueryPatient(string deptcode, string status)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.byDeptCode", ref where) == -1) return null;

            where = string.Format(where, deptcode, status);

            sql = sql + " " + where;

            return this.QueryRegister(sql);

        }

        /// <summary>
        /// 急诊留观查询当前护理站的不同状态的病人信息(出观)
        /// </summary>
        /// <param name="deptcode">科室编码</param>
        /// <param name="status">状态</param>
        /// <param name="fromDate">出观起始时间</param>
        /// <param name="toDate">出观截至时间</param>
        /// <returns></returns>
        public ArrayList QueryPatient(string deptcode, string status, string fromDate, string toDate)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
            if (this.Sql.GetCommonSql("Registration.Register.Query.byDeptCodeAndOutDate", ref where) == -1) return null;

            where = string.Format(where, deptcode, status, fromDate, toDate);

            sql = sql + " " + where;

            return this.QueryRegister(sql);

        }

        /// <summary>
        /// 根据门诊号去有效的挂号信息
        /// </summary>
        /// <param name="clinicNO">门诊号</param>
        /// <returns></returns>
        public ArrayList QueryPatient(string clinicNO)
        {
            string sql = string.Empty;
            string whereSql = string.Empty;

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1)
            {
                this.Err = "未能找到索引为[Registration.Register.Query.1]的sql语句";
                return null;
            }

            if (this.Sql.GetCommonSql("Registration.Register.Query.WhereByClinic", ref whereSql) == -1)
            {
                this.Err = "未能找到索引为[Registration.Register.Query.WhereByClinic]的sql语句";
                return null;
            }

            try
            {
                whereSql = string.Format(whereSql, clinicNO);
                sql = sql + "  " + whereSql;
            }
            catch (Exception ex)
            {

                this.Err = "设置参数出错" + ex.Message;
                return null;
            }

            return this.QueryRegister(sql);
        }

        /// <summary>
        /// 根据门诊流水号查询挂号记录
        /// </summary>
        /// <param name="clinicNO"></param>
        /// <param name="state">0 无效；1 有效；其他 全部</param>
        /// <returns></returns>
        public ArrayList QueryPatientByState(string clinicNO, string state)
        {
            string sql = string.Empty;
            string whereSql = string.Empty;

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1)
            {
                this.Err = "未能找到索引为[Registration.Register.Query.1]的sql语句";
                return null;
            }

            if (this.Sql.GetCommonSql("Registration.Register.Query.WhereByClinicAndState", ref whereSql) == -1)
            {
                this.Err = "未能找到索引为[Registration.Register.Query.WhereByClinicAndState]的sql语句";
                return null;
            }

            try
            {
                whereSql = string.Format(whereSql, clinicNO, state);
                sql = sql + "  " + whereSql;
            }
            catch (Exception ex)
            {

                this.Err = "设置参数出错" + ex.Message;
                return null;
            }

            return this.QueryRegister(sql);
        }

        #endregion

        #region 根据职称获取诊查费项目

        /// <summary>
        /// 根据医生职级获取对应的诊查费项目
        /// </summary>
        /// <param name="doctRank"></param>
        /// <returns></returns>
        [Obsolete("作废", true)]
        public string GetDiagItemCodeByDoctRank(string doctRank)
        {
            string sql = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1)
                return null;

            try
            {
                sql = string.Format(sql, doctRank);

                return this.ExecSqlReturnOne(sql);
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;
                return null;
            }
        }

        #endregion

        #region 急诊判断

        #endregion

        #region 查询指定合单位已看诊患者信息
        /// <summary>
        /// 查询指定合单位已看诊患者信息
        /// {4C5542EA-E90E-4831-B430-3D3DBDE12066}
        /// </summary>
        /// <param name="strPactArr"></param>
        /// <param name="dtSeeDateBeg"></param>
        /// <param name="dtSeeDateEnd"></param>
        /// <returns></returns>
        public ArrayList QueryYNSeeRegister(DateTime dtSeeDateBeg, DateTime dtSeeDateEnd)
        {
            string sql = ""; string where = "";

            try
            {
                if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1) return null;
                if (this.Sql.GetCommonSql("Registration.Register.Query.24", ref where) == -1) return null;

                where = string.Format(where, dtSeeDateBeg.ToString(), dtSeeDateEnd.ToString());

                sql = sql + " " + where;

                return this.QueryRegister(sql);
            }
            catch (Exception objEx)
            {
                this.Err = objEx.Message;
                return null;
            }
        }


        #endregion

        #region 顺德医保用来存储医保返回的门诊流水号2010-9-16
        /// <summary>
        /// 更新判断是否为30种病种
        /// 顺德医保用来存储医保返回的门诊流水号
        /// {2C4A235D-390F-41d5-92DE-B59E87448BDE}
        /// </summary>
        /// <param name="clinicID"></param>
        /// <param name="seeDeptID"></param>
        /// <param name="seeDoctID"></param>
        /// <returns></returns>
        public int UpdateDiagnose(Neusoft.HISFC.Models.Registration.Register reg)
        {
            string sql = "";

            string[] parm = new string[] { reg.ID, reg.NormalName };

            if (this.Sql.GetCommonSql("Registration.Register.Update.Diagnose", ref sql) == -1) return -1;

            return this.ExecNoQuery(sql, parm);
        }

        public string QueryDiagnose(Neusoft.HISFC.Models.Registration.Register reg)
        {
            string sql = "";
            if (this.Sql.GetCommonSql("Registration.Register.Query.Diagnose", ref sql) == -1)
            {
                return "";
            }

            sql = string.Format(sql, reg.ID);
            return this.ExecSqlReturnOne(sql);
        }
        #endregion

        #region 补挂号相关查询

        /// <summary>
        /// 根据医生职级获取对于的挂号级别和诊查费
        /// </summary>
        /// <param name="doctCode">医生编码</param>
        /// <param name="doctLevl">医生职级编码</param>
        /// <param name="deptCode">科室编码</param>
        /// <param name="regLevl">挂号级别编码</param>
        /// <param name="diagItemCode">诊查费项目</param>
        /// <returns></returns>
        public int GetSupplyRegInfo(string doctCode, string doctLevl, string deptCode, ref string regLevl, ref string diagItemCode)
        {
            string sql = "";
            #region 先按照排班获取排班的挂号级别及诊查费项目

            sql = @"select f.reglevl_code,
                       (select t.item_code from fin_com_regfeeset t
                       where t.reglevl_code=f.reglevl_code
                               and t.dept_code='ALL'
                               and rownum=1) item_code,
                               1 sort
                        from fin_opr_schema f
                        where f.doct_code='{0}'
                        and f.dept_code='{1}'
                        --and f.noon_code='{2}'
                        and f.begin_time<=to_date('{3}','yyyy-mm-dd hh24:mi:ss')
                        and f.end_time>=to_date('{3}','yyyy-mm-dd hh24:mi:ss')

                        union all

                        select f.reglevl_code,
                               (select t.item_code from fin_com_regfeeset t
                               where t.reglevl_code=f.reglevl_code
                               and t.dept_code='{1}'
                               and rownum=1) item_code,
                               2 sort
                        from fin_opr_schema f
                        where f.doct_code='{0}'
                        and f.dept_code='{1}'
                        --and f.noon_code='{2}'
                        and f.begin_time<=to_date('{3}','yyyy-mm-dd hh24:mi:ss')
                        and f.end_time>=to_date('{3}','yyyy-mm-dd hh24:mi:ss')
                        order by sort";

            sql = string.Format(sql, doctCode, deptCode, "", this.GetDateTimeFromSysDateTime().ToString());

            try
            {
                if (this.ExecQuery(sql) == -1)
                {
                    return -1;
                }
                while (this.Reader.Read())
                {
                    regLevl = Reader[0].ToString();
                    diagItemCode = Reader[1].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return -1;
            }
            #endregion

            #region 如果没有排班时，则按照职级获取

            if (string.IsNullOrEmpty(regLevl) || string.IsNullOrEmpty(diagItemCode))
            {
                sql = @"select t.reglevl_code,--挂号级别
                               t.item_code, --诊查费项目
                               1 sort
                        from fin_com_regfeeset t
                        where t.levl_code='{0}'
                        and t.dept_code='{1}'

                        union

                        select t.reglevl_code,--挂号级别
                               t.item_code, --诊查费项目
                               2 sort
                        from fin_com_regfeeset t
                        where t.levl_code='{0}'
                        and t.dept_code='ALL'
                        
   
                        union

                        select t.reglevl_code, --挂号级别
                                t.item_code, --诊查费项目
                                3 sort
                        from fin_com_regfeeset t
                        where t.levl_code = 'ALL'
                        and t.dept_code = 'ALL'
   
                        order by sort --按照序号排序";

                sql = string.Format(sql, doctLevl, deptCode);
                try
                {
                    if (this.ExecQuery(sql) == -1)
                    {
                        return -1;
                    }
                    while (this.Reader.Read())
                    {
                        regLevl = Reader[0].ToString();
                        diagItemCode = Reader[1].ToString();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Err = ex.Message;
                    return -1;
                }
            }

            #endregion
            return 1;
        }

        /// <summary>
        /// 根据挂号级别获取诊查费项目
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        /// <param name="regLevl">挂号级别编码</param>
        /// <param name="diagItemCode">诊查费项目</param>
        /// <returns></returns>
        public int GetSupplyRegInfo(string deptCode, string regLevl, ref string diagItemCode)
        {
            string sql = @"select t.item_code, --诊查费项目
                               1 sort
                        from fin_com_regfeeset t
                        where t.reglevl_code='{0}'
                        and t.dept_code='{1}'

                        union

                        select t.item_code, --诊查费项目
                               2 sort
                        from fin_com_regfeeset t
                        where t.reglevl_code='{0}'
                        and t.dept_code='ALL'
   
                        union

                        select --t.reglevl_code, --挂号级别
                        t.item_code, --诊查费项目
                        3 sort
                        from fin_com_regfeeset t
                        where t.levl_code = 'ALL'
                        and t.dept_code = 'ALL'
   
                        order by sort --按照序号排序";

            try
            {
                if (this.ExecQuery(sql, regLevl, deptCode) == -1)
                {
                    Err = this.Sql.Err;
                    return -1;
                }
                while (this.Reader.Read())
                {
                    diagItemCode = Reader[0].ToString();
                    break;
                }
                this.Reader.Close();
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return -1;
            }
            return 1;
        }


        /// <summary>
        /// 根据挂号级别获取诊查费项目
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        /// <param name="doctLevl">医生职级编码</param>
        /// <param name="regLevl">挂号级别编码</param>
        /// <param name="diagItemCode">诊查费项目</param>
        /// <returns></returns>
        public int GetSupplyRegInfo(string deptCode, string operLevel, string regLevl, ref string diagItemCode)
        {
            string sql = @"select t.item_code, --诊查费项目
                               1 sort
                        from fin_com_regfeeset t
                        where t.reglevl_code='{0}'
                        and t.dept_code='{1}'
                        and t.levl_code='{2}'

                        union

                        select t.item_code, --诊查费项目
                               2 sort
                        from fin_com_regfeeset t
                        where t.reglevl_code='{0}'
                        and t.dept_code='ALL'
                        --and t.levl_code='{2}'
   
                        union

                        select --t.reglevl_code, --挂号级别
                        t.item_code, --诊查费项目
                        3 sort
                        from fin_com_regfeeset t
                        where t.levl_code = 'ALL'
                        and t.dept_code = 'ALL'
   
                        order by sort --按照序号排序";

            try
            {
                if (this.ExecQuery(sql, regLevl, deptCode, operLevel) == -1)
                {
                    Err = this.Sql.Err;
                    return -1;
                }
                while (this.Reader.Read())
                {
                    diagItemCode = Reader[0].ToString();
                    break;
                }
                this.Reader.Close();
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return -1;
            }
            return 1;
        }

        #endregion

        #region 优化查询

        /// <summary>
        /// 查询挂号信息 
        /// 精简查询：门诊流水号、结算类别、合同单位、姓名、性别、出生日期
        /// </summary>
        /// <param name="whereIndex"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private ArrayList QuerySimpleRegInfo(string whereIndex, params string[] args)
        {
            //查询主SQL
            string sql = @"select clinic_code,--门诊流水号
                                   name,--姓名
                                   sex_code,--性别
                                   birthday,--生日
                                   paykind_code,--结算类别
                                   pact_code,--合同单位
                                   seeno 看诊序号,
                                   card_no ,--病历号
                                   reg_date ,--挂号时间
                                   dept_code, --挂号科室
                                   doct_code, --挂号医生
                                   reglevl_code,    --挂号级别
                                   reglevl_name,
                                   order_no, --每日序号
                                   reglevl_code,
                                   reglevl_name,
                                    oper_date
                            from fin_opr_register
                            ";
            if (this.Sql.GetCommonSql(whereIndex, ref whereIndex) == -1)
            {
                this.Err = Sql.Err;
                this.ErrCode = Sql.ErrCode;
                return null;
            }

            try
            {
                sql = sql + "\r\n" + whereIndex;

                sql = string.Format(sql, args);

                if (this.ExecQuery(sql) == -1)
                {
                }

                ArrayList al = new ArrayList();

                Neusoft.HISFC.Models.Registration.Register regObj = null;
                while (this.Reader.Read())
                {
                    regObj = new Neusoft.HISFC.Models.Registration.Register();
                    regObj.ID = this.Reader[0].ToString();//门诊流水号
                    regObj.Name = this.Reader[1].ToString();//姓名
                    regObj.Sex.ID = this.Reader[2].ToString();//性别
                    regObj.Birthday = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[3]);//生日
                    regObj.Pact.PayKind.ID = Reader[4].ToString();//结算类别
                    regObj.Pact.ID = this.Reader[5].ToString();//合同单位
                    regObj.DoctorInfo.SeeNO = Neusoft.FrameWork.Function.NConvert.ToInt32(this.Reader[6].ToString()); //看诊序号
                    regObj.PID.CardNO = this.Reader[7].ToString();
                    regObj.DoctorInfo.SeeDate = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[8]);
                    regObj.DoctorInfo.Templet.Dept.ID = Reader[9].ToString();
                    regObj.DoctorInfo.Templet.Doct.ID = Reader[10].ToString();
                    regObj.DoctorInfo.Templet.RegLevel.ID = Reader[11].ToString();
                    regObj.DoctorInfo.Templet.RegLevel.Name = Reader[12].ToString();
                    regObj.OrderNO = Neusoft.FrameWork.Function.NConvert.ToInt32(Reader[13].ToString());
                    regObj.DoctorInfo.Templet.RegLevel.ID = Reader[14].ToString();
                    regObj.DoctorInfo.Templet.RegLevel.Name = Reader[15].ToString();
                    regObj.InputOper.OperTime = Neusoft.FrameWork.Function.NConvert.ToDateTime(Reader[16]);

                    al.Add(regObj);
                }

                return al;
            }
            catch (Exception ex)
            {
                this.Err = ex.Message;
                this.ErrCode = ex.Message;

                return null;
            }
            finally
            {
                if (this.Reader != null && !Reader.IsClosed)
                {
                    this.Reader.Close();
                }
            }
        }

        /// <summary>
        /// 按照挂号科室查询一段时间内有效挂号信息
        /// 只查询必要信息：门诊流水号、结算类别、合同单位、姓名、性别、出生日期
        /// </summary>
        /// <param name="deptID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isSee">0 否；1 是；ALL 全部</param>
        /// <param name="isValid">0 退费；1 有效；2 作废；ALL 全部</param>
        /// <returns></returns>
        public ArrayList QuerySimpleRegByDept(string deptID, DateTime beginDate, DateTime endDate, string isSee, string isValid)
        {
            return this.QuerySimpleRegInfo("Registration.Register.QuerySimple.ByDept", deptID, beginDate.ToString(), endDate.ToString(), isSee, isValid);
        }

        #endregion

        /// <summary>
        /// 得到当前操作员从当前开始计算前N张发票的信息
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>成功: 结算信息数组 失败: null</returns>
        public ArrayList QueryRegistersByCount(string operCode, int count)
        {
            string sql = "", where = "";

            if (this.Sql.GetCommonSql("Registration.Register.Query.1", ref sql) == -1)
            {
                return null;
            }
            if (this.Sql.GetCommonSql("Registration.Register.Query.ByOperAndCount", ref where) == -1)
            {
                where = @" where ROWNUM <= {1}
                                       and  oper_date > trunc(sysdate)
	                                   and  oper_code = '{0}'
	                                   order by   OPER_DATE DESC";
            }

            try
            {
                where = string.Format(where, operCode, count);
            }
            catch (Exception e)
            {
                this.Err = "[" + where + "]" + e.Message;
                this.ErrCode = e.Message;
                return null;
            }

            sql = sql + " " + where;
            return this.QueryRegister(sql);
        }

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
            if (this.Sql.GetSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                seeNo = FrameWork.Function.NConvert.ToInt32(No);
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
            if (this.Sql.GetSql(str, ref sql) == -1)
            {
                this.Err = "没有找到sql，Id：" + str;
                return -1;
            }
            try
            {
                sql = string.Format(sql, schemaNo);
                No = this.ExecSqlReturnOne(sql);
                minNo = FrameWork.Function.NConvert.ToInt32(No);
            }
            catch (Exception ex)
            {
                this.Err = "查找最小看诊序号出错，错误信息：" + ex.Message;
                return -1;
            }
            return 1;
        }
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
            if (this.Sql.GetCommonSql(str, ref sql) == -1)
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
                cnt = FrameWork.Function.NConvert.ToInt32(No);
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
        /// 根据卡号和限定日期查询无身份证挂号次数
        /// 中大五院规定无身份证号挂号超过3次不允许再挂号
        /// </summary>
        /// <param name="cardNO">卡号</param>
        /// <param name="dtBegin">计算开始时间</param>
        /// <returns></returns>
        public int QueryRegiterByCardNOAndDtBegin(string cardNO, DateTime dtBegin)
        {
            string sql = string.Empty;
            sql = @"select sum(reg_times) times  from (
select case 
   when   length(p.idenno)=18  then 0
   when    length(p.idenno)=15 then 0 
   when p.idcardtype<>'01' and p.idenno is not null  then  0 
   when p.idcardtype<>'01' and p.idenno is not null  then  0 
   when r.trans_type='1' and p.idcardtype='01' and length(p.idenno)<>18 and length(p.idenno)<>15  then  1 
   when r.trans_type='2' and p.idcardtype='01' and length(p.idenno)<>18 and length(p.idenno)<>15  then  -1 
   when  r.trans_type='1' and  p.idenno is null   then 1
   when  r.trans_type='2' and  p.idenno is null   then -1
  else 1   end reg_times
    ,p.idenno,
r.* from fin_opr_register r ,com_patientinfo p 
where r.card_no='{0}' and r.card_no=p.card_no
and r.reg_date>to_date('{1}','yyyy-mm-dd HH24:mi:ss')
) ";
            try
            {
                sql = string.Format(sql, cardNO, dtBegin.Date.ToString());
            }
            catch (Exception e)
            {
                this.Err = "出错" + e.Message;
                return -1;

            }
            int result = Neusoft.FrameWork.Function.NConvert.ToInt32(this.ExecSqlReturnOne(sql));

            return result;
        }


        /// <summary>
        /// 判断是否开立过电子票(同时返回电子票图片数据)
        /// </summary>
        /// <param name="ClincCode"></param>
        /// <returns></returns>
        public int QueryElecBillImgForClincCode(string clincCode, string billtype, ref string imgData)
        {
            string sql = string.Format(@"select p.billqrcode from elec_outpatientrecord p where p.clinic_code='{0}' and p.billtype='{1}' and rownum<=1 ", clincCode, billtype);
            try
            {

                imgData = this.ExecSqlReturnOne(sql);
                if (string.IsNullOrEmpty(imgData) || imgData == "-1")
                {
                    this.Err = "该次挂号没有对应的电子票信息,无法打印挂号指引单！";
                    return -1;
                }
                return 1;

            }
            catch (Exception ex)
            {
                this.Err = "查询本次挂号的电子票图片数据出现异常！错误信息：" + ex.Message;
                return -1;
            }

        }

        /// <summary>
        /// 判断是否开立过电子票
        /// </summary>
        /// <param name="ClincCode"></param>
        /// <returns></returns>
        public int QueryElecDataForClincCode(string clincCode, string billtype)
        {
            string sql = string.Format(@"select p.clinic_code from elec_outpatientrecord p where p.clinic_code='{0}' and p.billtype='{1}' and rownum<=1 ", clincCode, billtype);
            try
            {

                string result = this.ExecSqlReturnOne(sql);
                if (string.IsNullOrEmpty(result) || result == "-1")
                {
                    this.Err = "没有查询到对应的电子票信息！";
                    return -1;
                }
                return 1;

            }
            catch (Exception ex)
            {
                this.Err = "查询对应的电子票数据出现异常！错误信息：" + ex.Message;
                return -1;
            }

        }

        /// <summary>
        /// 根据主键查询电子票信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billtype"></param>
        /// <returns></returns>
        public int QueryElecDataForId(string id, string billtype, ref Neusoft.HISFC.Models.ElecBill.Elec_OutPatientRecord model)
        {
            string sql = string.Format(@"select p.clinic_code,
       p.card_no,
       p.billbatchcode,
       p.billno,
       p.billtype,
       p.random,
       p.createtime,
       p.billqrcode,
       p.pictureurl,
       p.pictureneturl,
       p.createcode,
       p.createname,
       p.state from elec_outpatientrecord p where  p.clinic_code='{0}' and p.billtype='{1}' and p.state=0 and rownum<=1 ", id, billtype);
            if (this.ExecQuery(sql) == -1)
            {
                this.Err = "该挂号记录没有查询到对应的电子票信息！";
                return -1;
            }
            try
            {
                while (this.Reader.Read())
                {
                    model.clinic_code = this.Reader[0].ToString();
                    model.card_no = this.Reader[1].ToString();
                    model.billBatchCode = this.Reader[2].ToString();
                    model.billNo = this.Reader[3].ToString();
                    model.billType = this.Reader[4].ToString();
                    model.random = this.Reader[5].ToString();
                    model.createTime = this.Reader[6].ToString();
                    model.billQRCode = this.Reader[7].ToString();
                    model.pictureUrl = this.Reader[8].ToString();
                    model.pictureNetUrl = this.Reader[9].ToString();
                    model.createCode = this.Reader[10].ToString();
                    model.createName = this.Reader[11].ToString();
                    model.state = this.Reader[12].ToString();
                }
                this.Reader.Close();
                return 1;
            }
            catch (Exception ex)
            {

                this.Err = "查询对应的电子票数据出现异常！错误信息：" + ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 根据主键查询纸质票信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billtype"></param>
        /// <returns></returns>
        public int QueryElecPaperDataForId(string id, string billtype, ref Neusoft.HISFC.Models.ElecBill.Elec_OutPatientPaperBill model)
        {
            string sql = string.Format(@"select * from ( select p.clinic_code,
       p.billbatchcode,
       p.billno,
       p.pbillbatchcode,
       p.pbillno,
       p.billtype,
       p.state,
       p.createtime,
       p.createcode,
       p.createname,
 (select count(1) from  elec_outpatientpaperbillinfo p where p.clinic_code='{0}' and p.billtype='{1}') as returnCount
        from elec_outpatientpaperbillinfo p where p.clinic_code='{0}' and p.billtype='{1}' and (p.state=1 or p.state=3) order by p.createtime desc ) where  rownum=1", id, billtype);
            if (this.ExecQuery(sql) <= 0)
            {
                this.Err = "该发票号没有查询到对应的纸质票信息！";
                return -1;
            }
            try
            {
                while (this.Reader.Read())
                {
                    model.id = this.Reader[0].ToString();
                    model.billBatchCode = this.Reader[1].ToString();
                    model.billNo = this.Reader[2].ToString();
                    model.pBillBatchCode = this.Reader[3].ToString();
                    model.pBillNo = this.Reader[4].ToString();
                    model.billType = this.Reader[5].ToString();
                    model.state = this.Reader[6].ToString();
                    model.createTime = this.Reader[7].ToString();
                    model.createCode = this.Reader[8].ToString();
                    model.createName = this.Reader[9].ToString();
                    model.lastmodifycode = this.Reader[10].ToString();
                }
                this.Reader.Close();
                return 1;
            }
            catch (Exception ex)
            {

                this.Err = "查询对应的电子票数据出现异常！错误信息：" + ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 查询指定收费员某个类型的实际发票号
        /// </summary>
        /// <returns></returns>
        public int QueryRealInvoiceNO()
        {
            return 0;
        }

        /// <summary>
        /// 判断是否有纸质票信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billtype"></param>
        /// <returns></returns>
        public int IsHaveElecPaperData(string id, string billtype)
        {

            string sql = string.Format(@"select p.pbillbatchcode from Elec_OutPatientPaperBillInfo p where p.clinic_code='{0}' and p.billtype='{1}' and rownum<=1  and (p.state=1 or p.state=3)", id, billtype);
            try
            {

                string result = this.ExecSqlReturnOne(sql);
                if (string.IsNullOrEmpty(result) || result == "-1")
                {
                    this.Err = "该记录属于电子票,请先换开为纸质票！";
                    return -1;
                }
                return 1;

            }
            catch (Exception ex)
            {
                this.Err = "查询对应的纸质票数据出现异常！错误信息：" + ex.Message;
                return -1;
            }
        }


        /// <summary>
        /// 判断是否有红票信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billtype"></param>
        /// <returns></returns>
        public int IsHaveElecRedBillInfoData(string id, string billtype)
        {

            string sql = string.Format(@"select p.clinic_code from Elec_OutPatientRedBillInfo p where p.clinic_code='{0}' and p.billtype='{1}' and rownum<=1", id, billtype);
            try
            {

                string result = this.ExecSqlReturnOne(sql);
                if (string.IsNullOrEmpty(result) || result == "-1")
                {
                    this.Err = "该记录属于电子票,请先冲红电子票！";
                    return -1;
                }
                return 1;

            }
            catch (Exception ex)
            {
                this.Err = "查询对应的红票信息数据出现异常！错误信息：" + ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// 判断分诊流水号是否已经被使用
        /// </summary>
        /// <param name="triageSerialNum"></param>
        /// <returns></returns>
        public bool IsExistRegTriageSerialNum(string triageSerialNum)
        {
            try
            {
                string sql = @" select p.clinic_code from fin_opr_register p where p.triage_serialnum='{0}' ";
                sql = string.Format(sql, triageSerialNum);
                string result = this.ExecSqlReturnOne(sql, "");
                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Err = "判断分诊流水号是否存在时出现异常：" + ex.Message;
                return true;
            }
        }

        /// <summary>
        /// 获取处方号
        /// </summary>
        /// <returns></returns>
        public string GetOpbRecipeNoSequece()
        {
            string sql = string.Empty;
            sql = @"select SEQ_OPB_RECIPE_NO.NEXTVAL from dual ";
            try
            {
                sql = string.Format(sql);
            }
            catch (Exception e)
            {
                this.Err = "出错" + e.Message;
                return null;

            }
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 获取医嘱流水
        /// </summary>
        /// <returns></returns>
        public string GetMetMOOrderIDSequece()
        {
            string sql = string.Empty;
            sql = @"SELECT SEQ_MET_ORDER_ID.NEXTVAL FROM dual ";
            try
            {
                sql = string.Format(sql);
            }
            catch (Exception e)
            {
                this.Err = "获取医嘱流水出错" + e.Message;
                return null;

            }
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 根据工号查询人员所属科室
        /// </summary>
        /// <returns></returns>
        public string GetBelongDeptCodeForEmplCode(string emplCode)
        {
            string sql = string.Empty;
            sql = @"SELECT dept_code FROM com_employee WHERE empl_code = '{0}' ";
            try
            {
                sql = string.Format(sql,emplCode);
            }
            catch (Exception e)
            {
                this.Err = "根据工号查询人员所属科室出错" + e.Message;
                return null;

            }
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 获取诊疗项目的医保项目编码
        /// </summary>
        /// <returns></returns>
        public string GetRegItemCode(string reglevl_code)
        {
            string sql = string.Empty;
            sql = @"select t.item_code from fin_com_regfeeset t where  t.valid_flag = '1'and t.reglevl_code = '{0}' ";
            try
            {
                sql = string.Format(sql, reglevl_code);
            }
            catch (Exception e)
            {
                this.Err = "获取诊疗项目的医保项目编码出错" + e.Message;
                return null;

            }
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 根据项目编码获取项目名称
        /// </summary>
        /// <returns></returns>
        public string GetItemNameForItemCode(string itemCode)
        {
            string sql = string.Empty;
            sql = @"select item_name from fin_com_undruginfo  where item_code='{0}' ";
            try
            {
                sql = string.Format(sql, itemCode);
            }
            catch (Exception e)
            {
                this.Err = "根据项目编码获取项目名称出错" + e.Message;
                return null;

            }
            return this.ExecSqlReturnOne(sql);
        }

        /// <summary>
        /// 根据项目编码获取对应金额
        /// </summary>
        /// <returns></returns>
        public string GetPriceForItemCode(string itemCode)
        {
            string sql = string.Empty;
            sql = @"select  decode(unitflag, '1', fun_get_packageprice(item_code), unit_price) unit_price from fin_com_undruginfo  where item_code='{0}' ";
            try
            {
                sql = string.Format(sql, itemCode);
            }
            catch (Exception e)
            {
                this.Err = "根据项目编码获取对应金额出错" + e.Message;
                return null;

            }
            return this.ExecSqlReturnOne(sql);
        }

        public int GetDataForTriageSerialNum(string triageSerialNum, ref List<Neusoft.HISFC.Models.Registration.JZTriageWithoutRegModel> list)
        {
            try
            {
                string sql = @" select p.control_value from com_controlargument  p where p.control_code='TriageNumViewUrl' ";
                string url = this.ExecSqlReturnOne(sql, "");
                if (string.IsNullOrEmpty(url))
                {
                    this.Err = "急诊系统分诊视图数据库连接字符串暂未配置！";
                    return -1;
                }

                if (this.Sql.GetCommonSql("JZ.TriageSerialNum.Query.1", ref sql) == -1)
                {
                    this.Err = "Sql索引JZ.TriageSerialNum.Query.1暂未配置！";
                    return -1;
                }
                sql = string.Format(sql, triageSerialNum);
                Neusoft.HISFC.Models.Registration.JZTriageWithoutRegModel model;
                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(url))
                {
                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, con);
                    con.Open();
                    System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model = new Neusoft.HISFC.Models.Registration.JZTriageWithoutRegModel();
                        model.TriageNum = reader.GetValue(0).ToString();
                        model.TriageTime = reader.GetValue(1).ToString();
                        model.PatientName = reader.GetValue(2).ToString();
                        model.Age = reader.GetValue(3).ToString();
                        model.Sex = reader.GetValue(4).ToString();
                        model.IDCard = reader.GetValue(5).ToString();
                        model.Tel = reader.GetValue(6).ToString();
                        model.TDeptID = reader.GetValue(7).ToString();
                        model.TDeptName = reader.GetValue(8).ToString();
                        list.Add(model);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                con.Close();
                this.Err = ex.Message;
                return -1;
            }
        }


    }


    /// <summary>
    /// 挂号操作的类型
    /// </summary>
    public enum EnumUpdateStatus
    {
        /// <summary>
        /// 退号
        /// </summary>
        Return,
        /// <summary>
        /// 换科
        /// </summary>
        ChangeDept,
        /// <summary>
        /// 作废
        /// </summary>
        Cancel,
        /// <summary>
        /// 患者信息
        /// </summary>
        PatientInfo,
        /// <summary>
        /// 取消作废
        /// </summary>
        Uncancel,
        /// <summary>
        /// 废号
        /// </summary>
        Bad
    }

    /// <summary>
    /// 挂号打印接口
    /// </summary>
    public interface IRegPrint
    {
        /// <summary>
        /// 患者挂号信息
        /// </summary>
        Neusoft.HISFC.Models.Registration.Register RegInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 打印函数
        /// </summary>
        /// <returns></returns>
        int Print();
    }
}
