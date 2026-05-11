using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Integrate.Registration
{
    public class Registration : IntegrateBase
    {
        public Registration()
        {
        }

        #region 变量

        /// <summary>
        /// 挂号管理业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Registration.Register registerManager = new Neusoft.HISFC.BizLogic.Registration.Register();

        /// <summary>
        /// 专家排班管理
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Registration.DoctSchema doctSchemaMgr = new Neusoft.HISFC.BizLogic.Registration.DoctSchema();

        /// <summary>
        /// 排版管理
        /// </summary>
        private Neusoft.HISFC.BizLogic.Registration.Schema schemaManager = new Neusoft.HISFC.BizLogic.Registration.Schema();

        private Neusoft.HISFC.BizLogic.Manager.Constant constManager = new Neusoft.HISFC.BizLogic.Manager.Constant();

        /// <summary>
        /// 挂号级别业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Registration.RegLevel regLevelManager = new Neusoft.HISFC.BizLogic.Registration.RegLevel();

        /// <summary>
        /// 挂号级别业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Registration.RegLvlFee regLvlFeeManager = new Neusoft.HISFC.BizLogic.Registration.RegLvlFee();

        /// <summary>
        /// 分诊队列业务层
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Nurse.Assign assingManager = new Neusoft.HISFC.BizLogic.Nurse.Assign();

        /// <summary>
        /// 无别管理
        /// </summary>
        protected Neusoft.HISFC.BizLogic.Registration.Noon noonManager = new Neusoft.HISFC.BizLogic.Registration.Noon();

        /// <summary>
        /// 队列信息管理类
        /// </summary>
        private Neusoft.HISFC.BizLogic.Nurse.Queue queueBizLogic = new Neusoft.HISFC.BizLogic.Nurse.Queue();

        private Neusoft.HISFC.BizLogic.Registration.Schema schemaBizLogic = new Neusoft.HISFC.BizLogic.Registration.Schema();
        #endregion

        #region 事务处理

        /// <summary>
        /// 处理事务
        /// </summary>
        /// <param name="trans"></param>
        public override void SetTrans(System.Data.IDbTransaction trans)
        {
            this.trans = trans;
            doctSchemaMgr.SetTrans(trans);
            registerManager.SetTrans(trans);
            regLvlFeeManager.SetTrans(trans);
            regLevelManager.SetTrans(trans);
            assingManager.SetTrans(trans);
            noonManager.SetTrans(trans);
        }

        #endregion

        #region 插入

        /// <summary>
        /// 插入一条挂号记录
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        [Obsolete("作废,使用Insert(Neusoft.HISFC.Models.Registration.Register register)", true)]
        public int InsertByDoct(Neusoft.HISFC.Models.Registration.Register reg)
        {
            this.SetDB(registerManager);
            return registerManager.Insert(reg);
        }

        /// <summary>
        /// 插入挂号记录表{E43E0363-0B22-4d2a-A56A-455CFB7CF211}
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.Registration.Register register)
        {
            this.SetDB(registerManager);
            return registerManager.Insert(register);
        }
        #endregion

        #region 更新

        #region 更新看诊信息

        /// <summary>
        /// 更新看诊科室
        /// </summary>
        /// <param name="clinicID"></param>
        /// <param name="seeDeptID"></param>
        /// <param name="seeDoctID"></param>
        /// <returns></returns>
        public int UpdateDept(string clinicID, string seeDeptID, string seeDoctID)
        {
            this.SetDB(registerManager);
            return registerManager.UpdateDept(clinicID, seeDeptID, seeDoctID);
        }

        /// <summary>
        /// 更新看诊
        /// </summary>
        /// <param name="clinicNO"></param>
        /// <returns></returns>
        public int UpdateSeeDone(string clinicNO)
        {
            this.SetDB(registerManager);
            return registerManager.UpdateSeeDone(clinicNO);
        }

        /// <summary>
        /// 置已收挂号费标志
        /// </summary>
        /// <param name="clinicID"></param>
        /// <param name="operID"></param>
        /// <param name="operDate"></param>
        /// <returns></returns>
        public int UpdateAccountFeeState(string clinicID, string operID, string dept, DateTime operDate)
        {
            this.SetDB(registerManager);
            return registerManager.UpdateAccountFeeState(clinicID, operID, dept, operDate);
        }

        public int UpdateHosCode(Neusoft.HISFC.Models.Registration.Register register, string hosCode)
        {
            this.SetDB(registerManager);
            return registerManager.UpdateHosCode(register, hosCode);
        }

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
            this.SetDB(registerManager);
            return registerManager.UpdateSeeNo(Type, seeDate, Subject, noonID);
        }

        #endregion

        #region 更新已看诊、已收费标记

        /// <summary>
        /// 更新已看诊、已收费标记
        /// 主要用于补挂号
        /// </summary>
        /// <param name="clinicCode">门诊流水号</param>
        /// <returns></returns>
        public int UpdateYNSeeAndCharge(string clinicCode)
        {
            this.SetDB(this.registerManager);
            return this.registerManager.UpdateYNSeeAndCharge(clinicCode);
        }

        #endregion

        #region 更改基本信息时，更新挂号信息

        /// <summary>
        /// 修改患者基本信息时，更新挂号部分信息 根据clinicCode
        /// 姓名、性别、生日、身份者号、合同单位
        /// </summary>
        /// <param name="patientInfo">患者基本信息实体</param>
        /// <returns></returns>
        public int UpdateRegInfoByClinicCode(Neusoft.HISFC.Models.Registration.Register patientInfo)
        {
            this.SetDB(this.registerManager);
            return this.registerManager.UpdateRegInfoByClinicCode(patientInfo);
        }

        /// <summary>
        /// 修改患者基本信息时，更新挂号相关信息
        /// 姓名、性别、生日、身份者号、合同单位
        /// </summary>
        /// <param name="patientInfo">患者基本信息实体</param>
        /// <returns></returns>
        public int UpdateRegByPatientInfo(Neusoft.HISFC.Models.RADT.PatientInfo patientInfo)
        {
            this.SetDB(this.registerManager);
            return this.registerManager.UpdateRegByPatientInfo(patientInfo);
        }

        /// <summary>
        /// 更新挂号主表，看诊医生所属科室 根据clinicCode
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="doctInDept">医生所属科室</param>
        /// <param name="DocInDept">是否根据Ynsee(看诊标志)来更新</param>
        /// <returns></returns>
        public int UpdateRegDocInDeptByClinicCode(Neusoft.HISFC.Models.Registration.Register patientInfo, string doctInDept, bool isUseYnSee)
        {
            this.SetDB(this.registerManager);
            return this.registerManager.UpdateRegDocInDeptByClinicCode(patientInfo, doctInDept, isUseYnSee);
        }
        
        #endregion


        #region
        /// <summary>
        /// 根据排版信息更新护士站队列有效性
        /// </summary>
        /// <param name="validFlag"></param>
        /// <param name="seeDate"></param>
        /// <param name="noonCode"></param>
        /// <param name="deptCode"></param>
        /// <param name="doctCode"></param>
        /// <returns></returns>
        public int UpdateQueueValid(string validFlag, string seeDate, string noonCode, string deptCode, string doctCode)
        {
            this.SetDB(this.queueBizLogic);
            return this.queueBizLogic.UpdateQueueValid(validFlag, seeDate, noonCode, deptCode, doctCode);
        }

        /// <summary>
        /// 根据队列有效维护排版信息
        /// </summary>
        /// <param name="validFlag"></param>
        /// <param name="seeDate"></param>
        /// <param name="noonCode"></param>
        /// <param name="deptCode"></param>
        /// <param name="doctCode"></param>
        /// <returns></returns>
        public int UpdateDoctSchemaValid(string validFlag, string seeDate, string noonCode, string deptCode, string doctCode, string reasonNo, string reasonName, string stopOpcd, DateTime stopDate)
        {
            this.SetDB(this.schemaBizLogic);
            return this.schemaBizLogic.UpdateDoctSchemaValid(validFlag, seeDate, noonCode, deptCode, doctCode, reasonNo, reasonName, stopOpcd, stopDate);
        }
        #endregion





        #endregion

        #region 查询

        #region 查询基础数据

        /// <summary>
        /// 取数据库序列值来作为就诊卡号
        /// </summary>
        /// <returns>序列值</returns>
        public int AutoGetCardNO()
        {
            this.SetDB(this.registerManager);

            return registerManager.AutoGetCardNO();
        }

        #region 挂号级别

        /// <summary>
        /// 获取所有有效的挂号级别
        /// </summary>
        /// <returns>成功 所有有效的挂号级别集合 失败 null</returns>
        public ArrayList QueryRegLevel()
        {
            this.SetDB(regLevelManager);

            return regLevelManager.Query(true);
        }

        /// <summary>
        /// 获取所有的挂号级别 不区分是否有效
        /// </summary>
        /// <returns>成功 所有的挂号级别集合 失败 null</returns>
        public ArrayList QueryAllRegLevel()
        {
            this.SetDB(regLevelManager);

            return regLevelManager.Query();
        }

        /// <summary>
        /// 通过合同单位,和挂号级别获得挂号费
        /// </summary>
        /// <param name="pactCode">合同单位编码</param>
        /// <param name="regLevel">挂号级别</param>
        /// <returns>成功 挂号费实体 失败 null</returns>
        public Neusoft.HISFC.Models.Registration.RegLvlFee GetRegLevelByPactCode(string pactCode, string regLevel)
        {
            this.SetDB(regLvlFeeManager);

            return regLvlFeeManager.Get(pactCode, regLevel);
        }

        /// <summary>
        /// 查询一条挂号级别
        /// </summary>
        /// <param name="regLevelCode"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.RegLevel QueryRegLevelByCode(string regLevelCode)
        {
            this.SetDB(regLevelManager);
            return regLevelManager.Query(regLevelCode);
        }

        //根据各种卡号，查询挂号级别

        #endregion

        #region 根据医生职级查询对应的诊查费项目

        /// <summary>
        /// 根据医生职级获取对应的诊查费项目
        /// </summary>
        /// <param name="doctRank"></param>
        /// <returns></returns>
        [Obsolete("作废", true)]
        public Neusoft.HISFC.Models.Fee.Item.Undrug GetDiagItemCodeByDoctRank(string doctRank)
        {
            this.SetDB(noonManager);
            string itemCode = this.registerManager.GetDiagItemCodeByDoctRank(doctRank);
            //出错返回Null
            if (itemCode == null)
            {
                this.Err = this.registerManager.Err;
                return null;
            }
            //为空，表示没有查到
            else if (string.IsNullOrEmpty(itemCode))
            {
                this.Err = "没有维护改职级对应的诊查费项目！";
                return null;
            }
            else
            {
                Neusoft.HISFC.BizProcess.Integrate.Fee feeMgr = new Fee();
                Neusoft.HISFC.Models.Fee.Item.Undrug diagItem = feeMgr.GetItem(itemCode);
                if (diagItem == null || string.IsNullOrEmpty(diagItem.ID))
                {
                    this.Err = "查询非药品项目出错：代码[" + itemCode + "] " + feeMgr.Err;
                    return null;
                }

                return diagItem;
            }
        }
        #endregion

        /// <summary>
        /// 查询午别
        /// </summary>
        /// <returns></returns>
        public ArrayList QueryNoon()
        {
            this.SetDB(noonManager);
            return noonManager.Query();
        }

        #endregion

        #region 排班管理

        /// <summary>
        /// 查询排班信息
        /// </summary>
        /// <returns></returns>
        public ArrayList Query()
        {
            this.SetDB(doctSchemaMgr);
            return doctSchemaMgr.Query();
        }

        /// <summary>
        /// 获取排版信息
        /// {231D1A80-6BF6-413f-8BBF-727DC2BF47D9}
        /// </summary>
        /// <param name="doctCode">医生编码</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.Schema GetSchema(string doctCode, DateTime time)
        {
            this.SetDB(schemaManager);
            return schemaManager.GetSchema(doctCode, time);
        }

        #endregion

        #region 分诊

        /// <summary>
        /// 置已分诊标志
        /// </summary>
        /// <param name="clinicID"></param>
        /// <param name="operID"></param>
        /// <param name="operDate"></param>
        /// <returns></returns>
        public int Update(string clinicID, string operID, DateTime operDate)
        {
            this.SetDB(registerManager);
            return registerManager.Update(clinicID, operID, operDate);
        }

        /// <summary>
        /// 取消分诊状态
        /// </summary>
        /// <param name="clinicID"></param>
        /// <returns></returns>
        public int CancelTriage(string clinicID)
        {
            this.SetDB(registerManager);
            return registerManager.CancelTriage(clinicID);
        }

        /// <summary>
        /// 查询患者一段时间未分诊的有效号
        /// </summary>
        /// <param name="cardNo">就诊卡号</param>
        /// <param name="limitDate">现号时间</param>
        /// <returns></returns>
        public ArrayList QueryUnionNurse(string cardNo, DateTime limitDate)
        {
            this.SetDB(registerManager);
            return registerManager.QueryUnionNurse(cardNo, limitDate);
        }

        /// <summary>
        /// 查询患者一段时间的有效号
        /// 已分诊或未分诊（不包括进诊和诊出状态）
        /// </summary>
        /// <param name="cardNo">就诊卡号</param>
        /// <param name="limitDate">现号时间</param>
        /// <returns></returns>
        public ArrayList QueryUnionNurseTriage(string cardNo, DateTime limitDate)
        {
            this.SetDB(registerManager);
            return registerManager.QueryUnionNurseTriage(cardNo, limitDate);
        }

        /// <summary>
        /// 根据门诊号判断挂号信息是否分诊
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public bool QueryIsTriage(string clinicNo)
        {
            this.SetDB(registerManager);
            return registerManager.QueryIsTriage(clinicNo);
        }

        /// <summary>
        /// 根据门诊号判断患者是否在分诊队列中
        /// </summary>
        /// <param name="clinicNo">门诊号</param>
        /// <returns>大于等于1：分诊队列中有患者  0： 没有  -1:查询出错</returns>
        public int JudgeInQueue(string clinicNo)
        {
            this.SetDB(assingManager);
            return assingManager.JudgeInQueue(clinicNo);
        }

        #endregion

        #region 查询挂号记录

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
            this.SetDB(registerManager);
            return registerManager.GetSeeNo(Type, current, subject, noonID, ref seeNo);
        }

        #region 重复方法...

        /// <summary>
        /// 查询患者一段时间内挂的有效号
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList Query(string cardNo, DateTime limitDate)
        {
            this.SetDB(registerManager);
            return registerManager.Query(cardNo, limitDate);
        }

        /// <summary>
        /// 查询患者一段时间内挂的有效号
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="limitDate"></param>
        /// <returns></returns>
        public ArrayList QueryWithHosCode(string cardNo, DateTime limitDate, string hosCode)
        {
            this.SetDB(registerManager);
            return registerManager.QueryWitHosCode(cardNo, limitDate, hosCode);
        }
        /// <summary>
        ///  查询患者一段时间内挂的有效号
        /// 中大五院预约支付后可直接看诊，经常出现医生今天调了后天的挂号记录看诊情况
        /// 故增加改方法限制：1、普通门诊 在门诊医生站只允许调取当天的挂号记录 2、急诊科室允许调取挂号日期后24小时内有效（当前日期减去24小时）
        /// 2017-4-25 chengym
        /// *****************************************************************************************
        /// 新增登陆科室为反射诊断科可调出一周内挂号患者
        /// 2021-10-27
        /// </summary>
        /// <param name="cardNo">卡号</param>
        /// <param name="limitDate">挂号开始日期</param>
        /// <param name="dtEnd">挂号结束</param>
        /// <param name="hosCode">医院编码</param>
        /// <returns></returns>
        public ArrayList QueryWithHosCode(string cardNo, DateTime limitDate, DateTime dtEnd, string hosCode)
        {
            this.SetDB(registerManager);
            if (((Neusoft.HISFC.Models.Base.Employee)regLvlFeeManager.Operator).Dept.Clone().ID == "9225")//登陆科室为反射诊断科可调出一周内挂号患者
            {
                return registerManager.QueryWitHosCodeFSZD(cardNo, limitDate, dtEnd, hosCode);
            }
            else
            {
                return registerManager.QueryWitHosCode(cardNo, limitDate, dtEnd, hosCode);
            }


        }
        /// <summary>
        /// 通过门诊卡号查询一段时间内的有效的挂号患者
        /// </summary>
        /// <param name="cardNO">卡号</param>
        /// <param name="limitDate">有效的截至时间</param>
        /// <returns>成功 返回挂号患者的集合 失败 返回 null 没有查找到数据返回 ArrayList.Count == 0</returns>
        public ArrayList QueryValidPatientsByCardNO(string cardNO, DateTime limitDate)
        {
            this.SetDB(registerManager);

            return registerManager.Query(cardNO, limitDate);
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
            this.SetDB(registerManager);

            return registerManager.QueryRegList(cardNo, beginDate, endDate, valide);
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
            this.SetDB(registerManager);

            return registerManager.QueryRegListWithHosCode(cardNo, beginDate, endDate, valide, hosCode);
        }

        #endregion

        /// <summary>
        /// 根据病历号查询已看诊的有效挂号信息
        /// </summary>
        /// <param name="cardNO">病历号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结算时间</param>
        public ArrayList GetRegisterByCardNODate(string cardNO, DateTime beginDate, DateTime endDate)
        {
            this.SetDB(registerManager);
            return registerManager.GetRegisterByCardNODate(cardNO, beginDate, endDate);
        }

        /// <summary>
        /// 通过看诊序号查询一段时间内的有效的挂号患者
        /// </summary>
        /// <param name="seeNO">看诊序号</param>
        /// <param name="limitDate">有效的截至时间</param>
        /// <returns>成功 返回挂号患者的集合 失败 返回 null 没有查找到数据返回 ArrayList.Count == 0</returns>
        public ArrayList QueryValidPatientsBySeeNO(string seeNO, DateTime limitDate)
        {
            this.SetDB(registerManager);

            return registerManager.QueryBySeeNo(seeNO, limitDate);
        }

        /// <summary>
        /// 通过姓名查询有效的挂号患者
        /// </summary>
        /// <param name="name">患者姓名</param>
        /// <returns>成功 返回挂号患者的集合 失败 返回 null 没有查找到数据返回 ArrayList.Count == 0</returns>
        public ArrayList QueryValidPatientsByName(string name)
        {
            this.SetDB(registerManager);

            return registerManager.QueryByName(name);
        }

        /// <summary>
        /// 按门诊流水号查询挂号信息
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.Register GetByClinic(string clinicNo)
        {
            this.SetDB(registerManager);
            return registerManager.GetByClinic(clinicNo);
        }

        /// <summary>
        /// 通过一段时间内 某护理站的挂号患者{F044FCF3-6736-4aaa-AA04-4088BB194C20}
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="myNurseDept">护理站代码</param>
        /// <returns></returns>
        public ArrayList QueryNoTriagebyNurse(DateTime begin, string NurseID)
        {
            this.SetDB(registerManager);
            return registerManager.QueryNoTriagebyNurse(begin, NurseID);
        }

        /// <summary>
        /// 通过一段时间内 某护理站对应科室的挂号患者 addby sunxh
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="myNurseDept">护理站代码</param>
        /// <returns></returns>
        public ArrayList QueryNoTriagebyDept(DateTime begin, string myNurseDept)
        {
            this.SetDB(registerManager);
            return registerManager.QueryNoTriagebyDept(begin, myNurseDept);
        }

        /// <summary>
        /// 通过一段时间内 某护理站对应科室的挂号患者未看诊 addby sunxh
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="myNurseDept">护理站代码</param>
        /// <returns></returns>
        public ArrayList QueryNoTriagebyDeptUnSee(DateTime begin, string myNurseDept)
        {
            this.SetDB(registerManager);
            return registerManager.QueryNoTriagebyDeptUnSee(begin, myNurseDept);
        }

        /// <summary>
        /// 按挂号科室查询某一段时间内挂的有效号
        /// </summary>
        /// <param name="deptID">科室编码</param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isSee"></param>
        /// <returns></returns>
        public ArrayList QueryByDept(string deptID, DateTime beginDate, DateTime endDate, bool isSee)
        {
            this.SetDB(registerManager);
            return registerManager.QueryByDept(deptID, beginDate, endDate, isSee);
        }

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
            this.SetDB(registerManager);
            return registerManager.QueryRegisterByFeeExcuDept(excuDeptID, beginDate, endDate);
        }

        #endregion

        #region 按患者费用执行科室与卡号查询挂号信息
        /// <summary>
        /// 按患者费用执行科室查询挂号信息
        /// 
        /// {FCC85123-05D4-4baa-AB14-3DB983608766}
        /// </summary>
        /// <param name="excuDeptID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ArrayList QueryRegisterByFeeExcuDeptAndCardNo(string excuDeptID, string beginDate, string endDate, string cardNo)
        {
            this.SetDB(registerManager);
            return registerManager.QueryRegisterByFeeExcuDeptAndCardNo(excuDeptID, beginDate, endDate, cardNo);
        }

        #endregion


        #region 太多重复方法了...
        /// <summary>
        /// 按挂号医生查询某一段时间内挂的有效号
        /// </summary>
        /// <param name="doctID">医生编码</param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isSee"></param>
        /// <returns></returns>
        public ArrayList QueryByDoct(string doctID, DateTime beginDate, DateTime endDate, bool isSee)
        {
            this.SetDB(registerManager);
            return registerManager.QueryByDoct(doctID, beginDate, endDate, isSee);
        }
        /// <summary>
        /// 按看诊医生查询某一段时间内挂的有效号
        /// </summary>
        /// <param name="docID">医生编码</param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isSee"></param>
        /// <returns></returns>
        public ArrayList QueryBySeeDoc(string docID, DateTime beginDate, DateTime endDate, bool isSee)
        {
            this.SetDB(registerManager);
            return registerManager.QueryBySeeDoc(docID, beginDate, endDate, isSee);
        }

        /// <summary>
        /// 按看诊医生看诊时间查询某一段时间内的有效号
        /// </summary>
        /// <param name="docID">医生编码</param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isSee"></param>
        /// <returns></returns>
        public ArrayList QueryBySeeDocAndSeeDate(string docID, DateTime beginDate, DateTime endDate, bool isSee)
        {
            this.SetDB(registerManager);
            return registerManager.QueryBySeeDocAndSeeDate(docID, beginDate, endDate, isSee);
        }

        #endregion

        #region 作废  方法重复了

        /// <summary>
        /// 根据流水号查询门诊挂号记录
        /// </summary>
        /// <param name="clincNo"></param>
        /// <param name="state">0 无效；1 有效；其他 全部</param>
        /// <returns></returns>
        public ArrayList QueryPatientByState(string clincNo, string state)
        {
            this.SetDB(registerManager);
            return registerManager.QueryPatientByState(clincNo, state);
        }

        /// <summary>
        ///根据住门诊流水号查询挂号信息
        /// </summary>
        /// <param name="clinicNO"></param>
        /// <returns></returns>
        public ArrayList QueryPatient(string clinicNO)
        {
            this.SetDB(registerManager);
            return registerManager.QueryPatient(clinicNO);
        }

        #endregion

        #region  麻醉评估列表
        /// <summary>
        /// 医生站加载麻醉评估列表 {57D49CC0-3BEE-4168-8E71-4FAE394DF6A6} 
        /// </summary>
        /// <param name="assessState">评估代码</param>
        /// <returns>null为错</returns>
        public ArrayList AssessPatientQueryByDeptCode(string assessState)
        {
            this.SetDB(registerManager);
            return registerManager.AssessPatientQueryByDeptCode(assessState);
        }
        #endregion

        #endregion

        #region 数据判断

        /// <summary>
        /// 根据门诊号判断挂号信息是否作废
        /// </summary>
        /// <param name="clinicNo"></param>
        /// <returns></returns>
        public bool QueryIsCancel(string clinicNo)
        {
            this.SetDB(registerManager);
            return registerManager.QueryIsCancel(clinicNo);
        }

        /// <summary>
        /// 检验是否院内职工，根据身份证号码
        /// </summary>
        /// <param name="IdenNO">身份者号码</param>
        /// <returns></returns>
        public bool CheckIsEmployee(string IdenNO)
        {
            this.SetDB(registerManager);
            return registerManager.CheckIsEmployee(IdenNO);
        }

        /// <summary>
        /// 检验是否院内职工，根据挂号信息
        /// </summary>
        /// <param name="register">挂号信息</param>
        /// <returns></returns>
        public bool CheckIsEmployee(Neusoft.HISFC.Models.Registration.Register register)
        {
            this.SetDB(registerManager);
            return registerManager.CheckIsEmployee(register);
        }

        #endregion

        #region 急诊留观

        /// <summary>
        /// 按护士站和在院状态查询急诊留观患者
        /// </summary>
        /// <param name="nursecellcode">护士站代码</param>
        /// <param name="status">急诊留观人员状态</param>
        /// <returns>null为错</returns>
        public ArrayList PatientQueryByNurseCell(string nursecellcode, string status)
        {
            this.SetDB(registerManager);
            return registerManager.PatientQueryByNurseCell(nursecellcode, status);
        }

        /// <summary>
        /// 医生站加载留观患者
        /// </summary>
        /// <param name="nursecellcode">护士站代码</param>
        /// <param name="status">急诊留观人员状态</param>
        /// <returns>null为错</returns>
        public ArrayList PatientQueryByNurseCell(string deptCode)
        {
            this.SetDB(registerManager);
            return registerManager.PatientQueryByNurseCell(deptCode);
        }

        #endregion

        #region 补收挂号费

        /// <summary>
        /// 普诊科室列表
        /// </summary>
        ArrayList alOrdinaryRegDept = null;

        /// <summary>
        /// 普通门诊对应的挂号级别
        /// </summary>
        ArrayList alOrdinaryLevl = null;

        /// <summary>
        /// 获取急诊号常数
        /// </summary>
        ArrayList emergRegLevlList = null;

        /// <summary>
        /// 通过开立科室、医生职级获取对应的挂号级别
        /// </summary>
        /// <param name="recipeDept"></param>
        /// <param name="doctLevl"></param>
        /// <returns></returns>
        public Neusoft.HISFC.Models.Registration.RegLevel GetRegLevl(string recipeDept, string doctCode, string doctLevl)
        {
            #region 挂号级别

            string regLevl = "";

            //是否普诊科室
            bool isOrdinaryRegDept = false;

            #region 普诊挂号科室

            if (alOrdinaryRegDept == null)
            {
                alOrdinaryRegDept = new ArrayList();
                Neusoft.HISFC.BizLogic.Manager.Constant conManager = new Neusoft.HISFC.BizLogic.Manager.Constant();
                alOrdinaryRegDept = conManager.GetList("OrdinaryRegLevlDept");
                if (alOrdinaryRegDept == null)
                {
                    Err = "获取普诊挂号科室失败！" + conManager.Err;
                    return null;
                }
            }

            foreach (Neusoft.HISFC.Models.Base.Const constObj in alOrdinaryRegDept)
            {
                if (constObj.IsValid && constObj.ID.Trim() == recipeDept)
                {
                    isOrdinaryRegDept = true;
                    break;
                }
            }

            #endregion

            //普诊
            if (isOrdinaryRegDept)
            {
                if (alOrdinaryLevl == null)
                {
                    alOrdinaryLevl = new ArrayList();

                    Neusoft.HISFC.BizLogic.Manager.Constant conManager = new Neusoft.HISFC.BizLogic.Manager.Constant();
                    alOrdinaryLevl = conManager.GetList("OrdinaryRegLevel");
                    if (alOrdinaryLevl == null || alOrdinaryLevl.Count == 0)
                    {
                        Err = "获取普通门诊对应的挂号级别错误：" + conManager.Err;
                        return null;
                    }
                }

                foreach (Neusoft.HISFC.Models.Base.Const constObj in alOrdinaryLevl)
                {
                    if (constObj.IsValid)
                    {
                        regLevl = constObj.ID.Trim();
                        break;
                    }
                }
            }
            else
            {
                //是否急诊
                bool isEmerg = this.IsEmergency(recipeDept);

                string diagItemCode = "";
                if (isEmerg)
                {
                    if (emergRegLevlList == null)
                    {
                        emergRegLevlList = new ArrayList();
                        Neusoft.HISFC.BizLogic.Manager.Constant conManager = new Neusoft.HISFC.BizLogic.Manager.Constant();
                        emergRegLevlList = conManager.GetList("EmergencyLevel");
                        if (emergRegLevlList == null || emergRegLevlList.Count == 0)
                        {
                            Err = "获取急诊号失败！" + conManager.Err;
                            return null;
                        }
                    }

                    if (emergRegLevlList.Count > 0)
                    {
                        regLevl = ((Neusoft.FrameWork.Models.NeuObject)emergRegLevlList[0]).ID.Trim();
                    }
                    if (string.IsNullOrEmpty(regLevl))
                    {
                        Err = "获取急诊号错误，请联系信息科！";
                        return null;
                    }
                }
                else
                {
                    if (this.GetSupplyRegInfo(doctCode, doctLevl, recipeDept, ref regLevl, ref diagItemCode) == -1)
                    {
                        return null;
                    }
                }
            }

            Neusoft.HISFC.Models.Registration.RegLevel regLevlObj = this.regLevelManager.Query(regLevl);
            if (regLevlObj == null)
            {
                Err = "查询挂号级别错误，编码[" + regLevl + "]！请联系信息科重新维护" + Err;
                return null;
            }

            return regLevlObj;
            #endregion
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
            return registerManager.GetSupplyRegInfo(deptCode, regLevl, ref diagItemCode);
        }

        /// <summary>
        /// 根据挂号级别获取诊查费项目
        /// </summary>
        /// <param name="deptCode">科室编码</param>
        /// <param name="operLevel">医生职级编码</param>
        /// <param name="regLevl">挂号级别编码</param>
        /// <param name="diagItemCode">诊查费项目</param>
        /// <returns></returns>
        public int GetSupplyRegInfo(string deptCode, string operLevel, string regLevl, ref string diagItemCode)
        {
            return registerManager.GetSupplyRegInfo(deptCode, operLevel, regLevl, ref diagItemCode);
        }

        /// <summary>
        /// 根据医生职级获取对于的挂号级别和诊查费
        /// </summary>
        /// <param name="doctLevl">医生职级编码</param>
        /// <param name="deptCode">科室编码</param>
        /// <param name="regLevl">挂号级别编码</param>
        /// <param name="diagItemCode">诊查费项目</param>
        /// <returns></returns>
        public int GetSupplyRegInfo(string doctCode, string doctLevl, string deptCode, ref string regLevl, ref string diagItemCode)
        {
            return registerManager.GetSupplyRegInfo(doctCode, doctLevl, deptCode, ref regLevl, ref diagItemCode);

            //Neusoft.FrameWork.Models.NeuObject constObj = this.constManager.GetConstant("DOCLEVEL_REGLEVEL", doctLevl);
            //if (constObj == null || string.IsNullOrEmpty(constObj.ID))
            //{
            //    this.Err = "获取职级对应的挂号级别出错：" + this.constManager.Err;
            //    return -1;
            //}
            //regLevl = constObj.Name.Trim();

            //constObj = this.constManager.GetConstant("REGLEVEL_DIAGFEE", regLevl);
            //if (constObj == null || string.IsNullOrEmpty(constObj.ID))
            //{
            //    this.Err = "获取职级对应的诊查费项目出错：" + this.constManager.Err;
            //    return -1;
            //}
            //diagItemCode = constObj.Name.Trim();

            //return 1;
        }

        /// <summary>
        /// 急诊科室
        /// </summary>
        static Neusoft.FrameWork.Public.ObjectHelper emergencyDeptHelper = null;

        /// <summary>
        /// 非急诊时间段
        /// </summary>
        static Neusoft.FrameWork.Public.ObjectHelper noEmergencyTimeHelper = null;

        /// <summary>
        /// 法定节假日
        /// </summary>
        static Neusoft.FrameWork.Public.ObjectHelper holidayHelper = null;

        DateTime begin1 = DateTime.MinValue;
        DateTime end1 = DateTime.MinValue;
        DateTime begin2 = DateTime.MinValue;
        DateTime end2 = DateTime.MinValue;

        /// <summary>
        /// 检验是否是急诊的方式，现在只有澜石是每天判断的
        /// </summary>
        private string CheckEmergencyType;

        /// <summary>
        /// 是否急诊标记
        /// </summary>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public bool IsEmergency(string deptCode)
        {
            DateTime now = this.registerManager.GetDateTimeFromSysDateTime();
            return this.IsEmergency(deptCode, now);
        }

        /// <summary>
        /// 判断门诊是否急诊标记
        /// </summary>
        /// <param name="deptCode">开立科室</param>
        /// <param name="operDate">开立时间</param>
        /// <returns></returns>
        public bool IsEmergency(string deptCode, DateTime operDate)
        {
            if (string.IsNullOrEmpty(this.CheckEmergencyType))
            {
                Common.ControlParam controlMgr = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                this.CheckEmergencyType = controlMgr.GetControlParam<string>("HNGH01", true, "0");
            }

            return this.CheckIsEmergency(deptCode, operDate, CheckEmergencyType, true);
        }

        /// <summary>
        /// 是否急诊科室，节假日或周末
        /// </summary>
        /// <param name="operDate"></param>
        /// <returns></returns>
        public bool IsEmergencyHolidays(string deptCode, DateTime operDate)
        {
            if (string.IsNullOrEmpty(this.CheckEmergencyType))
            {
                Common.ControlParam controlMgr = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();
                this.CheckEmergencyType = controlMgr.GetControlParam<string>("HNGH01", true, "0");
            }

            return this.CheckIsEmergency(deptCode, operDate, CheckEmergencyType, false);
        }

        /// <summary>
        /// 判断门诊是否急诊标记
        /// </summary>
        /// <param name="deptCode">开立科室</param>
        /// <param name="operDate">开立时间</param>
        /// <param name="checkEmergencyType">开立时间</param>
        /// <returns></returns>
        private bool CheckIsEmergency(string deptCode, DateTime operDate, string checkEmergencyType, bool isCheckTime)
        {
            if (checkEmergencyType == "1")
            {
                try
                {
                    string dateNow = operDate.ToString("yyyy-MM-dd");
                    ArrayList al = new ArrayList();

                    #region 急诊科室判断

                    if (emergencyDeptHelper == null)
                    {
                        al = this.constManager.GetList("EmergencyDept");
                        if (al == null)
                        {
                            this.Err = constManager.Err;
                        }
                        else
                        {
                            emergencyDeptHelper = new Neusoft.FrameWork.Public.ObjectHelper(al);
                        }
                    }
                    if (emergencyDeptHelper != null)
                    {
                        if (emergencyDeptHelper.GetObjectFromID(deptCode) != null)
                        {
                            return true;
                        }
                    }

                    #endregion

                    #region 节假日判断

                    if (holidayHelper == null)
                    {
                        al = this.constManager.GetList("Holiday");
                        if (al == null)
                        {
                            this.Err = constManager.Err;
                        }
                        else
                        {
                            holidayHelper = new Neusoft.FrameWork.Public.ObjectHelper(al);
                        }
                    }
                    if (holidayHelper != null)
                    {
                        if (holidayHelper.GetObjectFromID(operDate.ToString("yyyy-MM-dd")) != null)
                        {
                            return true;
                        }
                    }

                    #endregion

                    #region 周末判断
                    //只要维护了数据 就认为周末当做急诊处理

                    //随便维护了常数，即认为需要判断
                    //al = this.constManager.GetList("WeekendIsEmergency");

                    //if (al.Count > 0)
                    //{
                    //    if (operDate.DayOfWeek.ToString().ToUpper() == "Saturday".ToUpper())
                    //    {
                    //        return true;
                    //    }
                    //    else if (operDate.DayOfWeek.ToString().ToUpper() == "Sunday".ToUpper())
                    //    {
                    //        return true;
                    //    }
                    //}
                    #endregion

                    if (isCheckTime)
                    {
                        #region 时间点判断

                        //name是星期
                        //memo是开始时间
                        //usercode是结束时间

                        if (noEmergencyTimeHelper == null)
                        {
                            al = this.constManager.GetList("EmergencyTimeSet");
                            if (al == null)
                            {
                                this.Err = constManager.Err;
                            }
                            else
                            {
                                noEmergencyTimeHelper = new Neusoft.FrameWork.Public.ObjectHelper(al);
                            }
                        }

                        foreach (Neusoft.HISFC.Models.Base.Const constObj in noEmergencyTimeHelper.ArrayObject)
                        {
                            if (constObj.Name.Trim() == this.GetDayOfWeek(operDate))
                            {
                                begin1 = Neusoft.FrameWork.Function.NConvert.ToDateTime(dateNow + " " + constObj.Memo);
                                end1 = Neusoft.FrameWork.Function.NConvert.ToDateTime(dateNow + " " + constObj.UserCode);


                                if (operDate >= begin1 && operDate <= end1)
                                {
                                    return false;
                                }
                            }
                        }
                        return true;

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return false;
                }

                return false;
            }
            else
            {

                try
                {
                    string dateNow = operDate.ToString("yyyy-MM-dd");
                    ArrayList al = new ArrayList();

                    #region 急诊科室判断

                    if (emergencyDeptHelper == null)
                    {
                        al = this.constManager.GetList("EmergencyDept");
                        if (al == null)
                        {
                            this.Err = constManager.Err;
                        }
                        else
                        {
                            emergencyDeptHelper = new Neusoft.FrameWork.Public.ObjectHelper(al);
                        }
                    }
                    if (emergencyDeptHelper != null)
                    {
                        if (emergencyDeptHelper.GetObjectFromID(deptCode) != null)
                        {
                            return true;
                        }
                    }

                    #endregion

                    #region 节假日判断

                    #region 周末判断
                    //只要维护了数据 就认为周末当做急诊处理
                    //随便维护了常数，即认为需要判断
                    al = this.constManager.GetList("WeekendIsEmergency");

                    if (al.Count > 0)
                    {
                        if (operDate.DayOfWeek.ToString().ToUpper() == "Saturday".ToUpper())
                        {
                            return true;
                        }
                        else if (operDate.DayOfWeek.ToString().ToUpper() == "Sunday".ToUpper())
                        {
                            return true;
                        }
                    }
                    #endregion

                    #region 假日判断

                    if (holidayHelper == null)
                    {
                        al = this.constManager.GetList("Holiday");
                        if (al == null)
                        {
                            this.Err = constManager.Err;
                        }
                        else
                        {
                            holidayHelper = new Neusoft.FrameWork.Public.ObjectHelper(al);
                        }
                    }
                    if (holidayHelper != null)
                    {
                        if (holidayHelper.GetObjectFromID(operDate.ToString("yyyy-MM-dd")) != null)
                        {
                            return true;
                        }
                    }

                    #endregion

                    #endregion

                    if (isCheckTime)
                    {
                        #region 时间点判断

                        if (noEmergencyTimeHelper == null)
                        {
                            al = this.constManager.GetList("EmergencyTime");
                            if (al == null)
                            {
                                this.Err = constManager.Err;
                            }
                            else
                            {
                                noEmergencyTimeHelper = new Neusoft.FrameWork.Public.ObjectHelper(al);
                            }
                        }

                        if (noEmergencyTimeHelper != null)
                        {
                            //4个时间点才是正确的时间点
                            if (noEmergencyTimeHelper.ArrayObject.Count == 4)
                            {
                                if (begin1 == DateTime.MinValue || begin1.ToString("yyyy-MM-dd") != dateNow)
                                {
                                    begin1 = Neusoft.FrameWork.Function.NConvert.ToDateTime(dateNow + " " + noEmergencyTimeHelper.GetName("1"));
                                }
                                if (end1 == DateTime.MinValue || end1.ToString("yyyy-MM-dd") != dateNow)
                                {
                                    end1 = Neusoft.FrameWork.Function.NConvert.ToDateTime(dateNow + " " + noEmergencyTimeHelper.GetName("2"));
                                }
                                if (begin2 == DateTime.MinValue || begin2.ToString("yyyy-MM-dd") != dateNow)
                                {
                                    begin2 = Neusoft.FrameWork.Function.NConvert.ToDateTime(dateNow + " " + noEmergencyTimeHelper.GetName("3"));
                                }
                                if (end2 == DateTime.MinValue || end2.ToString("yyyy-MM-dd") != dateNow)
                                {
                                    end2 = Neusoft.FrameWork.Function.NConvert.ToDateTime(dateNow + " " + noEmergencyTimeHelper.GetName("4"));
                                }

                                if (!(operDate >= begin1 && operDate <= end1 || operDate >= begin2 && operDate <= end2))
                                {
                                    return true;
                                }
                            }
                        }

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    this.Err = ex.Message;
                    return false;
                }

                return false;
            }
        }

        /// <summary>
        /// 获取星期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string GetDayOfWeek(DateTime date)
        {
            switch (date.DayOfWeek.ToString().ToUpper())
            {
                case "MONDAY":
                    return "星期一";
                    break;
                case "TUESDAY":
                    return "星期二";
                    break;
                case "WEDNESDAY":
                    return "星期三";
                    break;
                case "THURSDAY":
                    return "星期四";
                    break;
                case "FRIDAY":
                    return "星期五";
                    break;
                case "SATURDAY":
                    return "星期六";
                    break;
                case "SUNDAY":
                    return "星期日";
                    break;
                default:
                    return null;
                    break;
            }
        }

        #endregion

        #endregion

        #region 业务处理

        //public int Get

        #endregion
    }

    #region 作废

    ///// <summary>
    ///// 挂号票打印 统一放到HISFC.BizProcess.Interface下
    ///// </summary>
    //public interface IRegPrint
    //{
    //     ///<summary>
    //     ///数据库连接
    //     ///</summary>
    //    System.Data.IDbTransaction Trans
    //    {
    //        get;
    //        set;
    //    }
    //    /// <summary>
    //    /// 添值
    //    /// </summary>
    //    /// <param name="register"></param>
    //    /// <param name="reglvlfee"></param>
    //    /// <returns></returns>

    //    int SetPrintValue(Neusoft.HISFC.Models.Registration.Register register);

    //    /// <summary>
    //    /// 打印预览
    //    /// </summary>
    //    /// <returns>>成功 1 失败 -1</returns>
    //    int PrintView();
    //    /// <summary>
    //   /// 打印
    //   /// </summary>
    //    /// <returns>成功 1 失败 -1</returns>

    //    int Print();

    //    /// <summary>
    //    /// 清空当前信息
    //    /// </summary>
    //    /// <returns>成功 1 失败 -1</returns>
    //    int Clear();

    //    /// <summary>
    //    /// 设置本地数据库连接
    //    /// </summary>
    //    /// <param name="trans">数据库连接</param>
    //    void SetTrans(System.Data.IDbTransaction trans);
    //}
    ///// <summary>
    ///// 挂号票打印
    ///// </summary>
    //public interface IShowLED
    //{
    //    ///<summary>
    //    ///数据库连接
    //    ///</summary>
    //    //System.Data.IDbTransaction Trans
    //    //{
    //    //    get;
    //    //    set;
    //    //}
    //    /// <summary>
    //    /// 查找
    //    /// </summary>
    //    /// <param name="register"></param>
    //    /// <param name="reglvlfee"></param>
    //    /// <returns></returns>

    //    string  Query();


    //    /// <summary>
    //    /// 显示farpoint格式
    //    /// </summary>
    //    /// <returns>成功 1 失败 -1</returns>

    //    int SetFPFormat();

    //    /// <summary>
    //    ///  调用LED 接口 组成显示串给LED
    //    /// </summary>
    //    /// <returns>成功 1 失败 -1</returns>
    //    int CreateString();

    //    /// <summary>
    //    /// 设置本地数据库连接
    //    /// </summary>
    //    /// <param name="trans">数据库连接</param>
    //    //void SetTrans(System.Data.IDbTransaction trans);
    //}
    #endregion
}