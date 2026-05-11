using System;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using Neusoft.HISFC.Models.RADT;
using System.Collections.Generic;
using Neusoft.HISFC.Models.Registration;
using Neusoft.FrameWork.Function;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.BizProcess.Integrate.Pass
{
    /// <summary>
    /// 美康Pass 的摘要说明。
    /// </summary>
    public class MKPass : Neusoft.HISFC.BizProcess.Interface.Order.IReasonableMedicine
    {
        public MKPass()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 接口调用

        /// <summary>
        /// 注册服务器（PASS客户端共享应用摸式注册是否成功）
        /// </summary>
        /// <returns>0	注册成功，否则失败；255  PASS服务器物理连接失败</returns>
        [DllImport("ShellRunAs.dll")]
        public static extern int RegisterServer();

        /// <summary>
        /// 初始化PASS是否正确
        /// </summary>
        /// <param name="userInfo">用户工号/登录用户</param>
        /// <param name="deptInfo">科室代码/科室中文名称</param>
        /// <param name="stationType">必须传值，10--指住院医生站、门诊医生站、护士站、PIVA静脉药物配置中心；20-指临床药学工作站</param>
        /// <returns>0 初始化PASS失败 1初始化PASS正常通过</returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassInit(string userInfo, string deptInfo, int stationType);

        /// <summary>
        /// PASS运行模式设置
        /// </summary>
        /// <param name="saveCheckResult">是否进行采集，取值： 0-不采集 1-依赖系统设置  （必须传值）</param>
        /// <param name="allowAllegen">是否管理病人过敏史/病生状态，取值：0-不管理1-由用户传入 2-PASS管理 3-PASS强制管理（必须传值）</param>
        /// <param name="checkMode">审查模式，取值：0-普通模式 1-IV模式 （必须传值）</param>
        /// <param name="disqMode">调用药研究时是否传入医嘱信息，取值：0-不传 1-要传 2-提示 （必须传值）</param>
        /// <param name="useDiposeIden">是否使用处理意见，取值：0-不使用处理 1-根据设置（必须传值）</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassSetControlParam(int saveCheckResult, int allowAllegen, int checkMode, int disqMode, int useDiposeIden);

        /// <summary>
        /// 传病人基本信息
        /// </summary>
        /// <param name="PatientID">病人病案编号（必须传值）</param>
        /// <param name="VisitID">当前就诊次数（必须传值）</param>
        /// <param name="Name">病人姓名    （必须传值）</param>
        /// <param name="Sex">病人性别    （必须传值）如：男、女，其他值传：未知</param>
        /// <param name="Birthday">出生日期    （必须传值）格式：2005-09-20</param>
        /// <param name="Weight">体重        （可以不传值）单位：KG</param>
        /// <param name="cHeight">身高        （可以不传值）单位：CM</param>
        /// <param name="DepartMentName"> 医嘱科室ID/医嘱科室名称  （可以不传值）</param>
        /// <param name="Doctor">主治医生ID/主治医生姓名 （可以不传值）</param>
        /// <param name="LeaveHospitalDate">出院日期 （可以不传值）</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassSetPatientInfo(string PatientID, string VisitID, string Name, string Sex, string Birthday, string Weight, string cHeight, string DepartMentName, string Doctor, string LeaveHospitalDate);

        /// <summary>
        /// 传入当前病人用药信息接口
        /// 如果当前病人有多条用药医嘱时，循环传入。传入的医嘱为用药医嘱，对于工作站类型为10即时性审查时(如：住院医生站或门诊医生站)，用药结束日期可以不用传值，默认为当天；而对于工作站类型为20回顾性审查时(如：临床药学工作或查询统计)，用药结束日期必须传值。
        /// </summary>
        /// <param name="OrderUniqueCode">医嘱唯一码（必须传值）</param>
        /// <param name="DrugCode">药品编码  （必须传值）</param>
        /// <param name="DrugName">药品名称  （必须传值）</param>
        /// <param name="SingleDose">每次用量  （必须传值）</param>
        /// <param name="DoseUnit">剂量单位  （必须传值）</param>
        /// <param name="Frequency">用药频率(次/天)（必须传值）</param>
        /// <param name="StartOrderDate">用药开始日期，格式：yyyy-mm-dd  （必须传值）</param>
        /// <param name="StopOrderDate">用药结束日期，格式：yyyy-mm-dd  （可以不传值），默认值为当天</param>
        /// <param name="RouteName">给药途径中文名称 （必须传值）</param>
        /// <param name="GroupTag">成组医嘱标志  （必须传值）</param>
        /// <param name="OrderType">是否为临时医嘱 1-是临时医嘱 0或空 长期医嘱 （必须传值）</param>
        /// <param name="OrderDoctor">下嘱医生ID/下嘱医生姓名 （必须传值）</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassSetRecipeInfo(string OrderUniqueCode, string DrugCode, string DrugName, string SingleDose, string DoseUnit, string Frequency, string StartOrderDate, string StopOrderDate, string RouteName, string GroupTag, string OrderType, string OrderDoctor);

        /// <summary>
        /// 当病人过敏史信息由HIS系统管理，并传入到PASS系统进行审查时，调用该接口
        /// </summary>
        /// <param name="allergenIndex">过敏原在医嘱中的顺序编号（必须传值）</param>
        /// <param name="allergenCode">过敏原编码（必须传值）</param>
        /// <param name="allergenDesc">过敏原名称（必须传值）</param>
        /// <param name="allergenType">过敏原类型（必须传值）取值如下(判断不分大小写)：
        ///     AllergenGroup  PASS过敏组
        ///     USER_Drug   用户药品
        ///     DrugName     PASS药物名称</param>
        /// <param name="reaction">过敏症状  （可以不传值）</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassSetAllergenInfo(string allergenIndex, string allergenCode, string allergenDesc, string allergenType, string reaction);

        /// <summary>
        /// 传入病生状态
        /// </summary>
        /// <param name="medCondIndex">医疗条件在医嘱中的顺序编号（必须传值）</param>
        /// <param name="medCondCode">医疗条件编码（必须传值）</param>
        /// <param name="medCondDesc">医疗条件名称（必须传值）</param>
        /// <param name="medCondType">医疗条件类型（必须传值）传值如下(判断不分大小写)
        /// USER_MedCond	用户医疗条件
        /// ICD				ICD-9CM编码</param>
        /// <param name="startDate">开始日期  格式：yyyy-mm-dd（可以不传值）</param>
        /// <param name="endDate">结束日期  格式：yyyy-mm-dd（可以不传值）</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassSetMedCond(string medCondIndex, string medCondCode, string medCondDesc, string medCondType, string startDate, string endDate);

        /// <summary>
        /// 传入当前药品查询信息
        /// </summary>
        /// <param name="drugCode">药品编码（必须传值）</param>
        /// <param name="drugName">药品名称（必须传值）</param>
        /// <param name="doseUnit">剂量单位（必须传值）</param>
        /// <param name="routeName">用药途径中文名称（必须传值）</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassSetQueryDrug(string drugCode, string drugName, string doseUnit, string routeName);

        /// <summary>
        /// PASS系统功能是否有效性
        /// 在刷新界面、切换病人、弹出右键菜单，总之是需要刷新查询功能时，都根据此函数的返回值来设置相应控件Enabled属性
        /// </summary>
        /// <param name="queryItemNo">查询的项目标识或项目编号，其中带”/”的部分，表示两上标识都可以用来查询此项目的状态(项目标识不区分大小写)，见下表</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassGetState(string queryItemNo);
        /*
        项目编号	项目标识	项目描述	说明
            0	PASSENABLE	PASS连接是否可用	审查相关命令状态

            11	SYS-SET	系统参数设置	合理用药辅助功能
            12	DISQUISITION	用药研究	
            13	MATCH-DRUG	药品配对信息查询	
            14	MATCH-ROUTE	药品给药途径信息查询	

            24	AlleyEnable	病生状态/过敏史管理	过敏史/病生状态管理

            101	CPRRes/CPR	临床用药指南查询	一级菜单查询状态
            102	Directions	药品说明书查询	
            103	CPERes/CPE	病人用药教育查询	
            104	CheckRes/CHECKINFO	药物检验值查询	
            105	HisDrugInfo	医院药品信息查询	
            106	MEDInfo	药物信息查询中心	
            107	Chp	中国药典	
            501	DISPOSE	处理意见设置	
            220	LMIM	临床检验信息参考查询	

            201	DDIM	药物与药物相互作用查询	二级菜单查询状态(专项信息)
            202	DFIM	药物与食物相互作用查询	
            203	MatchRes/IV	国内注射剂体外配伍查询	
            204	TriessRes/IVM	国外注射剂体外配伍查询	
            205	DDCM	禁忌症查询	
            206	SIDE	副作用查询	
            207	GERI	老年人用药警告查询	
            208	PEDI	儿童用药警告查询	
            209	PREG	妊娠期用药警告查询	
            210	LACT	哺乳期用药警告查询	

            301	HELP	Pass系统帮助	合理用药帮助系统状态
         * */

        /// <summary>
        /// PASS功能调用
        /// </summary>
        /// <param name="commandNo">CommandNo命令号，表示调用PASS系统功能命令号</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassDoCommand(int commandNo);
        /*
         * 审查类
         * 
         *  1	住院医生工作站保存自动审查	1-审查正常通过            0-存在未处理问题
            2	住院医生工作站提交自动审查	1-审查正常通过            0-存在未处理问题
            33	门诊医生工作站保存自动审查	1-审查正常通过            0-存在未处理问题
            34	门诊医生工作站提交自动审查	1-审查正常通过            0-存在未处理问题
            3	手工审查	暂无意义
            4	临床药学单病人审查	暂无意义
            5	临床药学多病人审查	暂无意义
            6	单药警告	暂无意义
            7	手动审查,不显结果界面	暂无意义
         * 
            查询类
         * 
            13	药品配对信息查询	暂无意义
            14	药品给药途径信息查询	暂无意义
            101	临床用药指南查询	暂无意义
            102	药品说明书查询	暂无意义
            103	病人用药教育查询	暂无意义
            104	药物检验值查询	暂无意义
            105	医院药品信息查询	暂无意义
            106	药物信息查询中心	暂无意义
            107	中国药典	暂无意义
            201	药物与药物相互作用查询	暂无意义
            202	药物与食物相互作用查询	暂无意义
            203	国内注射剂体外配伍查询	暂无意义
            204	国外注射剂体外配伍查询	暂无意义
            205	禁忌症查询	暂无意义
            206	副作用查询	暂无意义
            207	老年人用药警告查询	暂无意义
            208	儿童用药警告查询	暂无意义
            209	妊娠期用药警告查询	暂无意义
            210	哺乳期用药警告查询	暂无意义
            220	临床检验信息参考查询	暂无意义
         * 
         * 窗口控制类
         * 
            401	显示浮动窗口	暂无意义
            402	关闭所有浮动窗口	暂无意义
            403	显示单药最近一次审查结果浮动提示窗口	暂无意义
         * 
         * 综合类
         * 
            11	系统参数设置	暂无意义
            12	用药研究	暂无意义
            21	病生状态/过敏史管理(只读)	暂无意义
            22	病生状态/过敏史管理(编辑)	病人过敏史/病生状态是否发生了变化。2-发生了变化，1-未发生变化，<=0-出现程序错误。
            23	病生状态/过敏史管理(强制)	病人过敏史/病生状态是否发生了变化。2-发生了变化，1-未发生变化，<=0-出现程序错误。

         * */

        /// <summary>
        /// 获取PASS审查结果警示级别
        /// 嵌套时用户可以根据审查返回警告值，进行对医嘱是否需要保存或提交控制，同时还可以将该警告值保存到HIS系统数据库中，便于其它工作站查看等。如果一条用药医嘱经过PASS审查发现可能存在多个潜在用药问题，系统将以其中警示级别最高的警示色标示该条医嘱 。需要注意的是，审查返回警告值不是最高代表最严重，而是警示级别最高的警示色标代表最严重
        /// </summary>
        /// <param name="drugUniqeCode">医嘱唯一编码</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassGetWarn(string drugUniqeCode);
        /*
         * 审查返回警告值	警示色	说明
            -3	无灯	表示PASS出现错误，未进行审查
            -2	无灯	表示该药品在处方传送时被过滤
            -1	无灯	表示未经过PASS系统的监测
            0	蓝灯	PASS监测未提示相关用药问题
            1	黄灯	危害较低或尚不明确，适度关注
            2	红灯	不推荐或较严重危害，高度关注
            4	橙灯	慎用或有一定危害，较高度关注
            3	黑灯	绝对禁忌、错误或致死性危害，严重关注
         * */

        /// <summary>
        /// 设置药品浮动窗口位置
        /// 传入当前药品浮动窗口显示位置时，调用本接口，但是调用本接口之前首先调用PassSetQueryDrug() 函数，传入当前药品查询信息
        /// </summary>
        /// <param name="left">左上角x</param>
        /// <param name="top">左上角y</param>
        /// <param name="right">右下角x</param>
        /// <param name="bottom">右下角y</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassSetFloatWinPos(int left, int top, int right, int bottom);

        /// <summary>
        /// 显示警告浮动窗口或单药警告
        /// </summary>
        /// <param name="drugUniqueCode">医嘱唯一编码</param>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassSetWarnDrug(string drugUniqueCode);

        /// <summary>
        /// PASS退出
        /// </summary>
        /// <returns></returns>
        [DllImport("DIFPassDll.dll")]
        public static extern int PassQuit();

        #endregion

        #region IReasonableMedicine 成员

        /// <summary>
        /// 错误信息
        /// </summary>
        private string err = "";

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Err
        {
            get
            {
                return this.err;
            }
            set
            {
                this.err = value;
            }
        }

        /// <summary>
        /// 是否启用合理用药审查
        /// </summary>
        private bool passEnabled = false;

        /// <summary>
        /// 是否启用合理用药审查
        /// </summary>
        public bool PassEnabled
        {
            get
            {
                return this.passEnabled;
            }
            set
            {
                this.passEnabled = value;
            }
        }

        /// <summary>
        /// 当前患者信息
        /// </summary>
        private Neusoft.HISFC.Models.RADT.Patient myPatient = null;

        /// <summary>
        /// 当前开立医生
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject myReciptDoct = null;

        /// <summary>
        /// 工作站类别
        /// </summary>
        ServiceTypes stationType = ServiceTypes.C;

        /// <summary>
        /// 工作站类别
        /// </summary>
        ServiceTypes Neusoft.HISFC.BizProcess.Interface.Order.IReasonableMedicine.StationType
        {
            get
            {
                return stationType;
            }
            set
            {
                stationType = value;
            }
        }

        /// <summary>
        /// 合理用药初始化
        /// </summary>
        /// <param name="logEmpl"></param>
        /// <param name="logDept"></param>
        /// <param name="workStationType"></param>
        /// <returns></returns>
        public int PassInit(Neusoft.FrameWork.Models.NeuObject logEmpl, Neusoft.FrameWork.Models.NeuObject logDept, string workStationType)
        {
            Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam controlMgr = new Neusoft.HISFC.BizProcess.Integrate.Common.ControlParam();

            bool isEnablePass = controlMgr.GetControlParam<bool>("200014", true, false);

            if (!isEnablePass)
            {
                this.passEnabled = false;
                return 0;
            }
            try
            {
                int rev = PassInit(logEmpl.ID + "/" + logEmpl.Name, logDept.ID + "/" + logDept.Name, 10);

                if (rev != 1)
                {
                    try
                    {
                        if (System.IO.File.Exists(Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + "MedicomSoftWare.exe"))
                        {
                            System.Diagnostics.Process.Start(Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + "MedicomSoftWare.exe");
                            DateTime time = DateTime.Now;
                            while (DateTime.Now < time.AddSeconds(3))
                            {
                            }
                        }
                        //rev = PassInit(logEmpl.ID + "/" + logEmpl.Name, logDept.ID + "/" + logDept.Name, 10);
                    }
                    catch
                    {
                        this.passEnabled = false;
                        return rev;
                    }
                }

                if (rev == 0)
                {
                    this.passEnabled = false;
                    this.err = "合理用药系统 初始化失败! 请与管理员联系";
                    return -1;
                }
                if (PassGetState("0") != 0)
                {
                    //PassSetControlParam(1, 2, 0, 2, 1);
                    PassSetControlParam(1, 1, 0, 2, 1);  //第二个参数，指的是过敏史/病生状态：1用户传入；2美康系统管理； 2014-04-26 gumzh修改
                    passEnabled = true;
                }
            }
            catch (DllNotFoundException)
            {
                try
                {
                    if (System.IO.File.Exists(Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + "MedicomSoftWare.exe"))
                    {
                        System.Diagnostics.Process.Start(Neusoft.FrameWork.WinForms.Classes.Function.CurrentPath + "MedicomSoftWare.exe");
                        DateTime time = DateTime.Now;
                        while (DateTime.Now < time.AddSeconds(3))
                        {
                        }
                    }
                    //int rev = PassInit(logEmpl.ID + "/" + logEmpl.Name, logDept.ID + "/" + logDept.Name, 10);

                    //if (rev > 0)
                    //{
                    //    return rev;
                    //}
                }
                catch
                {
                }

                //this.err = "未找到合理用药系统正常运行所需Dll 合理用药系统将无法正常启动！\n如有疑问，请与信息科联系！";
                this.passEnabled = false;
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// 合理用药功能初始化刷新
        /// </summary>
        /// <returns></returns>
        public int PassRefresh()
        {
            return 1;
        }

        /// <summary>
        /// 查询单个药品要点提示
        /// </summary>
        /// <param name="order"></param>
        /// <param name="LeftTop"></param>
        /// <param name="RightButton"></param>
        /// <param name="isFirst"></param>
        /// <returns></returns>
        public int PassShowSingleDrugInfo(Neusoft.HISFC.Models.Order.Order order, System.Drawing.Point LeftTop, System.Drawing.Point RightButton, bool isFirst)
        {
            if (this.passEnabled && (PassGetState("0") != 0))
            {
                int rev = 0;
                //rev = PassSetPatientInfo(this.myPatient, this.myReciptDoct);
                rev = PassSetQueryDrug(order.Item.ID, order.Item.Name, order.DoseUnit, order.Usage.Name);
                rev = PassSetFloatWinPos(LeftTop.X, LeftTop.Y, RightButton.X, RightButton.Y);
                this.PassShowFloatWindow(true);
            }
            return 1;
        }

        /// <summary>
        /// 获取药品警告信息
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public int PassShowWarnDrug(Neusoft.HISFC.Models.Order.Order order)
        {
            if (this.passEnabled && (PassGetState("0") != 0))
            {
                try
                {
                    PassSetWarnDrug(order.ID);
                    PassDoCommand(403);
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                    return -1;
                }
            }
            return 1;
        }

        /// <summary>
        /// 药品审查
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="alOrder"></param>
        /// <param name="isSave">分为保存时自动审查和右键手工审查</param>
        /// <returns></returns>
        public int PassDrugCheck(ArrayList alOrder, bool isSave)
        {
            int warn = 0;
            if (isSave)
            {
                if (this.stationType == ServiceTypes.C)
                {
                    //warn = PassDrugCheckBase(alOrder, 1);
                    //warn = PassDrugCheckBase(alOrder, 2);
                    warn = PassDrugCheckBase(alOrder, 33);
                }
                else
                {
                    warn = PassDrugCheckBase(alOrder, 33);
                }
            }
            else
            {
                warn = PassDrugCheckBase(alOrder, 1);
            }

            if (warn == 3)
            {
                if (MessageBox.Show("PASS系统审查出存在黑灯用药？\r\n继续保存、执行处方吗？", "PASS安全用药审查提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return -1;
                }
            }

            return warn;
        }

        /// <summary>
        /// 药品审查通用函数
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <param name="alOrder"></param>
        /// <param name="isSave">分为保存时自动审查和右键手工审查</param>
        /// <param name="type">查询类别</param>
        /// <returns></returns>
        public int PassDrugCheckBase(ArrayList alOrder, int type)
        {
            /* 1：住院医生工作站保存自动审查
             * 33：门诊医生工作站保存自动审查
             * 3：手工审查,在点击弹出菜单中的"审查"命令时调用
             * 12：用药研究,在点击弹出菜单中的"审查"命令时调用
             * 6：单药警告,在点击弹出菜单中的"单药警告"命令时调用
             * */

            if (!this.passEnabled)
            {
                return 0;
            }
            if ((alOrder == null) || (alOrder.Count == 0))
            {
                return -1;
            }

            this.PassSetPatientInfo(myPatient, myReciptDoct);

            //0-蓝色、1-黄色、2-红色、3-黑色和4-橙色
            int warn = 1;

            foreach (Neusoft.HISFC.Models.Order.Order order in alOrder)
            {
                if (order == null)
                {
                    this.err = "执行医嘱用药审查时出错! 发生类型转换错误";
                    return -1;
                }
                if ((order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug) && (order.Status != 3))
                {
                    try
                    {
                        this.PassSetRecipeInfo(order);
                    }
                    catch (Exception exception)
                    {
                        this.err = "合理用药审查传送医嘱数据过程中发生错误! " + exception.Message;
                        return -1;
                    }
                }
            }
            PassDoCommand(type);

            //单药警告
            if (type == 6)
            {
                PassSetWarnDrug((alOrder[0] as Neusoft.HISFC.Models.Order.Order).ID);
            }
            //审查功能
            else if (type == 1 || type == 2 || type == 3 || type == 33 || type == 34)
            {
                foreach (Neusoft.HISFC.Models.Order.Order order in alOrder)
                {
                    if (order == null)
                    {
                        this.err = "执行医嘱用药审查时出错! 发生类型转换错误";
                        return -1;
                    }
                    if ((order.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug) && (order.Status != 3))
                    {
                        int rev = PassGetWarn(order.ID);
                        if (warn < rev)
                        {
                            warn = rev;
                        }
                    }
                }
            }

            return warn;
        }

        /// <summary>
        /// 退出PASS
        /// </summary>
        /// <returns></returns>
        public int PassClose()
        {
            if (this.passEnabled && (PassGetState("0") != 0))
            {
                return PassQuit();
            }
            return -1;
        }

        /// <summary>
        /// 设置浮动窗口是否显示
        /// </summary>
        /// <param name="isShow"></param>
        public int PassShowFloatWindow(bool isShow)
        {
            if (this.passEnabled && (PassGetState("0") != 0))
            {
                if (isShow)
                {
                    PassDoCommand(401);
                }
                else
                {
                    PassDoCommand(402);
                }
            }
            return 1;
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        ArrayList alMenu = null;

        /// <summary>
        /// 根据名称获得查询类别ID
        /// </summary>
        /// <param name="queryTypeName"></param>
        /// <returns></returns>
        private int GetQueryTypeID(string queryTypeName)
        {
            if (alMenu == null || alMenu.Count == 0)
            {
                return 0;
            }
            foreach (TreeNode node in alMenu)
            {
                if (node.Tag == null)
                {
                    foreach (TreeNode secondNode in node.Nodes)
                    {
                        if (secondNode.Text.Trim() == queryTypeName.Trim())
                        {
                            return Neusoft.FrameWork.Function.NConvert.ToInt32(secondNode.Tag);
                        }
                    }
                }

                if (node.Text.Trim() == queryTypeName.Trim())
                {
                    return Neusoft.FrameWork.Function.NConvert.ToInt32(node.Tag);
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取查询功能
        /// </summary>
        /// <param name="order">医嘱</param>
        /// <param name="queryType">查询功能类别</param>
        /// <param name="alShowMenu">查询菜单列表</param>
        /// <returns></returns>
        public int PassShowOtherInfo(Neusoft.HISFC.Models.Order.Order order, Neusoft.FrameWork.Models.NeuObject queryType, ref ArrayList alShowMenu)
        {
            try
            {
                if (queryType != null)
                {
                    int queryTypeID = GetQueryTypeID(queryType.Name);
                    if (queryTypeID == 0)
                    {
                        return 1;
                    }
                    PassShowFloatWindow(false);
                    PassSetQueryDrug(order.Item.ID, order.Item.Name, order.DoseUnit, order.Usage.Name);
                    DoCommand(Neusoft.FrameWork.Function.NConvert.ToInt32(queryTypeID));
                }
                else
                {
                    if (alMenu != null)
                    {
                        alShowMenu = alMenu;
                    }
                    else
                    {
                        TreeNode passTemp = new TreeNode();

                        #region 一级菜单
                        TreeNode pass22 = new TreeNode("过敏史/病生状态");
                        pass22.Tag = 22;

                        TreeNode pass101 = new TreeNode("药物临床信息参考");
                        pass101.Tag = 101;

                        TreeNode pass102 = new TreeNode("药品说明书");
                        pass102.Tag = 102;

                        TreeNode pass103 = new TreeNode("病人用药教育");
                        pass103.Tag = 103;

                        TreeNode pass104 = new TreeNode("药物检验值");
                        pass104.Tag = 104;

                        TreeNode pass105 = new TreeNode("医院药品信息");
                        pass105.Tag = 105;

                        TreeNode pass106 = new TreeNode("医药信息中心");
                        pass106.Tag = 106;

                        TreeNode pass107 = new TreeNode("中国药典");
                        pass107.Tag = 107;

                        TreeNode pass = new TreeNode("专项信息");

                        TreeNode pass13 = new TreeNode("药品配对信息");
                        pass13.Tag = 13;

                        TreeNode pass14 = new TreeNode("给药途径配对信息");
                        pass14.Tag = 14;

                        TreeNode pass11 = new TreeNode("系统设置");
                        pass11.Tag = 11;

                        TreeNode pass12 = new TreeNode("用药研究");
                        pass12.Tag = 12;

                        TreeNode pass6 = new TreeNode("警告");
                        pass6.Tag = 6;

                        TreeNode pass3 = new TreeNode("审查");
                        pass3.Tag = 3;

                        TreeNode pass301 = new TreeNode("帮助");
                        pass301.Tag = 301;

                        alMenu = new ArrayList();
                        alMenu.Add(pass22);
                        alMenu.Add(passTemp);
                        alMenu.Add(pass101);
                        alMenu.Add(pass102);
                        alMenu.Add(pass103);
                        alMenu.Add(pass104);
                        alMenu.Add(pass105);
                        alMenu.Add(passTemp);
                        alMenu.Add(pass106);
                        alMenu.Add(passTemp);
                        alMenu.Add(pass107);

                        alMenu.Add(passTemp);
                        alMenu.Add(pass);
                        alMenu.Add(passTemp);

                        alMenu.Add(pass13);
                        alMenu.Add(pass14);
                        alMenu.Add(passTemp);
                        alMenu.Add(pass11);
                        alMenu.Add(passTemp);
                        alMenu.Add(pass12);
                        alMenu.Add(passTemp);
                        alMenu.Add(pass6);
                        alMenu.Add(pass3);
                        alMenu.Add(passTemp);
                        alMenu.Add(pass301);

                        #endregion

                        #region 二级菜单 专项信息

                        TreeNode pass201 = new TreeNode("药物-药物相互作用");
                        pass201.Tag = 201;

                        TreeNode pass202 = new TreeNode("药物-食物相互作用");
                        pass202.Tag = 202;

                        TreeNode pass203 = new TreeNode("国内注射剂体外配伍");
                        pass203.Tag = 203;

                        TreeNode pass204 = new TreeNode("国外注射剂体外配伍");
                        pass204.Tag = 204;

                        TreeNode pass205 = new TreeNode("禁忌症");
                        pass205.Tag = 205;

                        TreeNode pass206 = new TreeNode("副作用");
                        pass206.Tag = 206;

                        TreeNode pass207 = new TreeNode("老年人用药");
                        pass207.Tag = 207;

                        TreeNode pass208 = new TreeNode("儿童用药");
                        pass208.Tag = 208;

                        TreeNode pass209 = new TreeNode("妊娠期用药");
                        pass209.Tag = 209;

                        TreeNode pass210 = new TreeNode("哺乳期用药");
                        pass210.Tag = 210;

                        TreeNode pass220 = new TreeNode("临床检验信息参考");
                        pass220.Tag = 220;

                        pass.Nodes.Add(pass201);
                        pass.Nodes.Add(pass202);
                        pass.Nodes.Add(passTemp);
                        pass.Nodes.Add(pass203);
                        pass.Nodes.Add(pass204);
                        pass.Nodes.Add(passTemp);
                        pass.Nodes.Add(pass205);
                        pass.Nodes.Add(pass206);
                        pass.Nodes.Add(passTemp);
                        pass.Nodes.Add(pass207);
                        pass.Nodes.Add(pass208);
                        pass.Nodes.Add(pass209);
                        pass.Nodes.Add(pass210);
                        pass.Nodes.Add(pass220);

                        #endregion

                        foreach (TreeNode node in alMenu)
                        {
                            if (node == null)
                            {
                                continue;
                            }

                            if (node.Tag == null)
                            {
                                foreach (TreeNode secondNode in node.Nodes)
                                {
                                    if (node == null)
                                    {
                                        continue;
                                    }
                                    if (secondNode.Tag != null && PassGetState(secondNode.Tag.ToString()) == 0)
                                    {
                                        secondNode.Tag = "不可用";
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (node.Tag != null && PassGetState(node.Tag.ToString()) == 0)
                                    {
                                        node.Tag = "不可用";
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }

                        alShowMenu = alMenu;
                    }
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return -1;
            }

            return 1;
        }

        #endregion

        #region 方法

        /// <summary>
        /// PASS功能调用
        /// </summary>
        /// <param name="commandType">PASS功能命令号</param>
        /// <returns></returns>
        public int DoCommand(int commandType)
        {
            //判断连接是否可用
            if (this.passEnabled && (PassGetState("0") != 0))
            {
                return PassDoCommand(commandType);
            }
            return -4;
        }

        /// <summary>
        /// 获取PASS审查结果警示级别
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public int PassGetWarnFlag(string orderId)
        {
            if (this.passEnabled && (PassGetState("0") != 0))
            {
                return PassGetWarn(orderId);
            }
            return -4;
        }

        /// <summary>
        /// 显示警告浮动窗口或单药警告
        /// </summary>
        /// <param name="orderId">医嘱唯一编码</param>
        /// <param name="flag">暂时无用</param>
        /// <returns></returns>
        public int PassGetWarnInfo(string orderId, string flag)
        {
            if (this.passEnabled && (PassGetState("0") != 0))
            {
                PassSetWarnDrug(orderId);
                if (flag == "0")
                {
                    PassDoCommand(0x193);
                }
                else
                {
                    PassDoCommand(6);
                }
            }
            return 1;
        }


        /// <summary>
        /// 添加合理用药结果警世标志
        /// </summary>
        /// <param name="iRow">欲更改行索引</param>
        /// <param name="iSheet">欲更改Sheet索引</param>
        /// <param name="warnFlag">警世标志</param>
        public void AddWarnPicturn(int iRow, int iSheet, int warnFlag)
        {
            //string picturePath = Application.StartupPath + "\\pic";
            //switch (warnFlag)
            //{
            //    case 0:										//0 (蓝色)无问题
            //        picturePath = picturePath + "\\warn0.gif";
            //        break;
            //    case 1:										//1 (黄色)危害较低或尚不明确
            //        picturePath = picturePath + "\\warn1.gif";
            //        break;
            //    case 2:										//2 (红色)不推荐或较严重危害
            //        picturePath = picturePath + "\\warn2.gif";
            //        break;
            //    case 3:										// 3 (黑色)绝对禁忌、错误或致死性危害
            //        picturePath = picturePath + "\\warn3.gif";
            //        break;
            //    case 4:										//4 (澄色)慎用或有一定危害 
            //        picturePath = picturePath + "\\warn4.gif";
            //        break;
            //    default:
            //        break;
            //}
            //if (!System.IO.File.Exists(picturePath))
            //{
            //    return;
            //}
            //try
            //{
            //    FarPoint.Win.Spread.CellType.TextCellType t = new FarPoint.Win.Spread.CellType.TextCellType();
            //    FarPoint.Win.Picture pic = new FarPoint.Win.Picture();
            //    pic.Image = System.Drawing.Image.FromFile(picturePath, true);
            //    pic.TransparencyColor = System.Drawing.Color.Empty;
            //    t.BackgroundImage = pic;
            //    this.neuSpread1.Sheets[iSheet].Cells[iRow, GetColumnIndexFromName("警")].CellType = t;			//医嘱名称
            //    this.neuSpread1.Sheets[iSheet].Cells[iRow, GetColumnIndexFromName("警")].Tag = "1";							//已做过审查
            //    this.neuSpread1.Sheets[iSheet].Cells[iRow, GetColumnIndexFromName("警")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("设置合理用药审查结果显示过程中出错!" + ex.Message);
            //}
        }

        /// <summary>
        /// 传入病人信息
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="docID"></param>
        /// <param name="docName"></param>
        public int PassSetPatientInfo(Patient patientObj, Neusoft.FrameWork.Models.NeuObject recipeDoct)
        {
            if (((patientObj != null) && this.passEnabled) && (PassGetState("0") != 0))
            {
                myPatient = patientObj;
                myReciptDoct = recipeDoct;

                string docID = recipeDoct.ID;
                string docName = recipeDoct.Name;

                if (stationType == ServiceTypes.I)
                {
                    #region 住院患者信息

                    Neusoft.HISFC.Models.RADT.PatientInfo patient = patientObj as Neusoft.HISFC.Models.RADT.PatientInfo;

                    string str2;
                    string patientNO = patient.PID.PatientNO;
                    try
                    {
                        str2 = NConvert.ToInt32(patient.ID.Substring(2, 2)).ToString();
                    }
                    catch
                    {
                        str2 = "1";
                    }
                    string name = patient.Name;
                    string sex = "";
                    if ((patient.Sex.ID.ToString() == "F") || (patient.Sex.ID.ToString() == "M"))
                    {
                        sex = patient.Sex.Name;
                    }
                    else
                    {
                        sex = "未知";
                    }
                    string birthday = patient.Birthday.ToString("yyyy-MM-dd");
                    string weight = patient.Weight;
                    string height = patient.Height;
                    //string departMentName = patient.PVisit.PatientLocation.Dept.ID + "/" + patient.PVisit.PatientLocation.Dept.Name;
                    string departMentName = ((Neusoft.HISFC.Models.Base.Employee)recipeDoct).Dept.ID + "/" + ((Neusoft.HISFC.Models.Base.Employee)recipeDoct).Dept.Name;
                    string doctor = recipeDoct.ID + "/" + recipeDoct.Name;
                    string leaveHospitalDate = "";
                    PassDoCommand(402);
                    PassSetPatientInfo(patientNO, str2, name, sex, birthday, weight, height, departMentName, doctor, leaveHospitalDate);

                    #endregion
                }
                else
                {
                    #region 门诊患者信息

                    Neusoft.HISFC.Models.Registration.Register patient = patientObj as Neusoft.HISFC.Models.Registration.Register;

                    string str2;
                    string patientNO = patient.PID.CardNO;
                    try
                    {
                        str2 = NConvert.ToInt32(patient.ID.Substring(2, 2)).ToString();
                    }
                    catch
                    {
                        str2 = "1";
                    }
                    string name = patient.Name;
                    string sex = "";
                    if ((patient.Sex.ID.ToString() == "F") || (patient.Sex.ID.ToString() == "M"))
                    {
                        sex = patient.Sex.Name;
                    }
                    else
                    {
                        sex = "未知";
                    }
                    string birthday = patient.Birthday.ToString("yyyy-MM-dd");
                    string weight = patient.Weight;
                    string height = patient.Height;
                    //string departMentName = patient.PVisit.PatientLocation.Dept.ID + "/" + patient.PVisit.PatientLocation.Dept.Name;
                    string departMentName = ((Neusoft.HISFC.Models.Base.Employee)recipeDoct).Dept.ID + "/" + ((Neusoft.HISFC.Models.Base.Employee)recipeDoct).Dept.Name;
                    string doctor = recipeDoct.ID + "/" + recipeDoct.Name;
                    string leaveHospitalDate = "";
                    PassDoCommand(402);
                    PassSetPatientInfo(patientNO, str2, name, sex, birthday, weight, height, departMentName, doctor, leaveHospitalDate);

                    //设置过敏史信息
                    PassSetAllergenInfo(patientObj);

                    #endregion
                }
            }
            return 1;
        }

        /// <summary>
        /// 传入处方（医嘱）信息
        /// </summary>
        /// <param name="order"></param>
        public void PassSetRecipeInfo(Neusoft.HISFC.Models.Order.Order orderObj)
        {
            if (this.passEnabled && ((orderObj != null)
                && (orderObj.Item.ItemType == Neusoft.HISFC.Models.Base.EnumItemType.Drug)))
            {
                if (stationType == ServiceTypes.I)
                {
                    #region 住院医嘱

                    Neusoft.HISFC.Models.Order.Inpatient.Order order = orderObj as Neusoft.HISFC.Models.Order.Inpatient.Order;

                    //string applyNO = order.ApplyNo;
                    string applyNO = order.ID; //医嘱唯一编号

                    string iD = order.Item.ID;//项目编码

                    //string iD = "Y00000001077";
                    string name = order.Item.Name;
                    string singleDose = order.DoseOnce.ToString();
                    string doseUnit = order.DoseUnit;
                    int length = 0;
                    string str7 = "";
                    if ((order.Frequency != null) && (order.Usage != null))
                    {
                        if (order.Frequency.Time == null)
                        {
                            Neusoft.HISFC.BizLogic.Manager.Frequency frequency = new Neusoft.HISFC.BizLogic.Manager.Frequency();
                            order.Frequency = (Neusoft.HISFC.Models.Order.Frequency)frequency.Get(order.Frequency, "ROOT");
                            if (order.Frequency == null)
                            {
                                return;
                            }
                        }
                        if (order.Frequency.Time == null)
                        {
                            length = 1;
                        }
                        else
                        {
                            length = order.Frequency.Times.Length;
                        }
                        if (order.Frequency.Days == null)
                        {
                            str7 = "1";
                        }
                        else
                        {
                            str7 = order.Frequency.Days[0];
                        }
                        string str6 = length.ToString() + "/" + str7.ToString();
                        string startOrderDate = order.BeginTime.ToString("yyyy-MM-dd");
                        string stopOrderDate = "";
                        string routeName = order.Usage.Name;
                        string groupTag = order.Combo.ID;
                        string orderType = order.OrderType.ID;
                        //string orderDoctor = order.Doctor.ID + "/" + order.Doctor.Name;
                        string orderDoctor = order.ReciptDoctor.ID + "/" + order.ReciptDoctor.Name;
                        PassSetRecipeInfo(applyNO, iD, name, singleDose, doseUnit, str6, startOrderDate, stopOrderDate, routeName, groupTag, orderType, orderDoctor);
                    }
                    #endregion
                }
                else
                {
                    #region 门诊处方

                    Neusoft.HISFC.Models.Order.OutPatient.Order order = orderObj as Neusoft.HISFC.Models.Order.OutPatient.Order;

                    //string applyNO = order.ApplyNo; //医嘱唯一编号【废弃】
                    string applyNO = order.ID;    //医嘱唯一编号

                    //string iD = order.Item.UserCode;
                    string iD = order.Item.ID; //项目编码

                    string name = order.Item.Name;
                    string singleDose = order.DoseOnce.ToString();
                    string doseUnit = order.DoseUnit;
                    int length = 0;
                    string str7 = "";
                    if ((order.Frequency != null) && (order.Usage != null))
                    {
                        if (order.Frequency.Time == null)
                        {
                            Neusoft.HISFC.BizLogic.Manager.Frequency frequency = new Neusoft.HISFC.BizLogic.Manager.Frequency();
                            order.Frequency = (Neusoft.HISFC.Models.Order.Frequency)frequency.Get(order.Frequency, "ROOT");
                            if (order.Frequency == null)
                            {
                                return;
                            }
                        }
                        if (order.Frequency.Time == null)
                        {
                            length = 1;
                        }
                        else
                        {
                            length = order.Frequency.Times.Length;
                        }
                        if (order.Frequency.Days == null)
                        {
                            str7 = "1";
                        }
                        else
                        {
                            str7 = order.Frequency.Days[0];
                        }
                        string str6 = length.ToString() + "/" + str7.ToString();
                        string startOrderDate = order.BeginTime.ToString("yyyy-MM-dd");
                        string stopOrderDate = "";
                        string routeName = order.Usage.Name;
                        string groupTag = order.Combo.ID;
                        string orderType = "1";
                        //string orderDoctor = order.Doctor.ID + "/" + order.Doctor.Name;
                        string orderDoctor = order.ReciptDoctor.ID + "/" + order.ReciptDoctor.Name;

                        PassSetRecipeInfo(applyNO, iD, name, singleDose, doseUnit, str6, startOrderDate, stopOrderDate, routeName, groupTag, orderType, orderDoctor);
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// 设置患者的过敏史
        /// </summary>
        /// <param name="patientObj"></param>
        public void PassSetAllergenInfo(Patient patientObj)
        {
            if (((patientObj != null) && this.passEnabled) && (PassGetState("0") != 0))
            {
                if (stationType == ServiceTypes.C)
                {
                    #region 门诊过敏史

                    try
                    {
                        Neusoft.HISFC.Models.Registration.Register patient = patientObj as Neusoft.HISFC.Models.Registration.Register;
                        string patientKind = "1";
                        string cardNO = patient.PID.CardNO;
                        Neusoft.HISFC.BizLogic.Order.Medical.AllergyManager allergyMgr = new Neusoft.HISFC.BizLogic.Order.Medical.AllergyManager();
                        ArrayList al = allergyMgr.QueryAllergyInfo(cardNO, patientKind);
                        if (al != null && al.Count > 0)
                        {
                            int index = 0;
                            foreach (Neusoft.HISFC.Models.Order.Medical.AllergyInfo allergyInfo in al)
                            {
                                if (allergyInfo.Allergen.ID.StartsWith("Y"))
                                {
                                    //索引要不相同；药品HIS编码；药品HIS名称；来源HIS(USER_Drug)；备注
                                    PassSetAllergenInfo(index.ToString(), allergyInfo.Allergen.ID, allergyInfo.Allergen.Name, "USER_Drug", allergyInfo.Remark);
                                    index++;
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }


                    #endregion
                }
            }
        }

        #endregion

        #region IReasonableMedicine 成员


        public int PassSetDiagnoses(ArrayList diagnoseList)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
