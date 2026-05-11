namespace Neusoft.HISFC.Models.Base
{
	#region 服务属性
	/// <summary>
	/// 服务属性
	/// </summary>
	public enum ServiceTypes 
	{

		/// <summary>
		/// 门诊
		/// </summary>
		C = 1,
		/// <summary>
		/// 住院
		/// </summary>
		I = 2
	}
	#endregion
	
	#region 模糊查询项目方式
	/// <summary>
	/// 模糊查询项目方式
	/// </summary>
	public enum InputTypes
	{
		/// <summary>
		/// 拼音码
		/// </summary>
		Spell = 1,
		/// <summary>
		/// 五笔码
		/// </summary>
		WB = 2, 
		/// <summary>
		/// 自定义码
		/// </summary>
		UserCode = 3,
		/// <summary>
		/// 名称
		/// </summary>
		Name = 4
	}
	#endregion
	
	#region 项目类别
	/// <summary>
	/// 项目类别
	/// </summary>
	public enum ItemTypes
	{
		/// <summary>
		/// 所有项目,包括药品,诊疗项目
		/// </summary>
		All = 1, 
		/// <summary>
		///所有药品 
		/// </summary>
		AllMedicine = 2,
		/// <summary>
		/// 西药
		/// </summary>
		WesternMedicine = 3,
		/// <summary>
		/// 中成药
		/// </summary>
		ChineseMedicine = 4,
		/// <summary>
		/// 中草药
		/// </summary>
		HerbalMedicine = 5,
		/// <summary>
		/// 非药品
		/// </summary>
		Undrug = 6
	}
	
	#endregion
	
	#region 交易类型
	/// <summary>
	/// 交易类型
	/// </summary>
	public enum TransTypes
	{
		/// <summary>
		/// 正交易
		/// </summary>
		Positive = 1, 
		/// <summary>
		/// 负交易
		/// </summary>
		Negative = 2
	}
	#endregion
	
	#region 收费类型
	/// <summary>
	/// 收费类型
	/// </summary>
	public enum PayTypes
	{
		/// <summary>
		/// 划价未收费
		/// </summary>
		Charged = 0,
		/// <summary>
		/// 已经收费
		/// </summary>
		Balanced = 1,
        /// <summary>
        /// 已经发药
        /// </summary>
        SendDruged = 2,
		/// <summary>
		/// 团体体检
		/// </summary>
		EXAMINE = 3,
		/// <summary>
		/// 药品预审核
		/// </summary>
		PhaConfim = 4,
        /// <summary>
        /// 记账
        /// </summary>
        Account = 5
	}
	#endregion
	
	#region 作废信息
	/// <summary>
	/// 作废信息
	/// </summary>
	public enum CancelTypes
	{
		/// <summary>
		/// 有效
		/// </summary>
		Valid = 1,
		/// <summary>
		/// 已经作废退费
		/// </summary>
		Canceled = 0,
		/// <summary>
		/// 重打
		/// </summary>
		Reprint = 2,
		/// <summary>
		/// 注销
		/// </summary>
		LogOut = 3
	}
	#endregion
	
	#region 收费参数
	/// <summary>
	/// 收费参数
	/// </summary>
	public enum ChargeTypes
	{
		/// <summary>
		/// 划价保存
		/// </summary>
		Save = 1,
		/// <summary>
		/// 收费
		/// </summary>
		Fee = 2
	}
	#endregion

    #region 有效性状态

    /// <summary>
    /// 有效性状态
    /// </summary>
    public enum EnumValidState
    {
        /// <summary>
        /// 有效
        /// </summary>
        Valid = 1,
        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// 忽略/废弃
        /// </summary>
        Ignore = 2,
        /// <summary>
        /// 扩展
        /// </summary>
        Extend = 3
    }

    #endregion

    #region 按照作废情况查询
    /// <summary>
	/// 按照作废情况查询
	/// </summary>
	public enum QueryValidTypes
	{
		/// <summary>
		/// 有效
		/// </summary>
		Valid = 1,
		/// <summary>
		/// 作废
		/// </summary>
		Cancel = 0,
		/// <summary>
		/// 所有
		/// </summary>
		All = 2
	}
#endregion
	
	#region 病床状态
	/// <summary>
	/// 病床状态
	/// </summary>
	public enum EnumBedStatus
	{
		/// <summary>
		/// Closed
		/// </summary>
		C,
		/// <summary>
		/// Unoccupied
		/// </summary>
		U,
		/// <summary>
		/// Contaminated污染的
		/// </summary>
		K,
		/// <summary>
		/// 隔离的
		/// </summary>
		I,
		/// <summary>
		/// Occupied
		/// </summary>
		O,
		/// <summary>
		/// 假床  user define
		/// </summary>
		R,
		/// <summary>
		/// 包床 user define
		/// </summary>
		W,
		/// <summary>
		/// 挂床
		/// </summary>
		H
	}	
	#endregion
     
    #region Rhd信息
    /// <summary>
    /// Rhd信息
    /// </summary>
    public enum RhDs
    {
        /// <summary>
        /// 阳性
        /// </summary>
        Positive = 1,
        /// <summary>
        /// 阴性
        /// </summary>
        Negative = 2
    }

    #endregion

    #region 血液隐性阳性信息
    /// <summary>
    /// Rhd信息
    /// </summary>
    public enum EnumBloodTypeByRh
    {
        /// <summary>
        /// 阳性
        /// </summary>
        P = 1,
        /// <summary>
        /// 阴性
        /// </summary>
        N = 2,
    }

    #endregion

	#region 血液类型
	/// <summary>
	/// 血液类型枚举
	/// </summary>
	public enum EnumBloodTypeByABO
	{

		/// <summary>
		/// 未知
		/// </summary>
		U = 0,
		/// <summary>
		///A 
		/// </summary>
		A=1,
		/// <summary>
		/// B
		/// </summary>
		B=2,
		/// <summary>
		/// AB
		/// </summary>
		AB=3,
		/// <summary>
		/// O
		/// </summary>
		O=4
	};
	#endregion
	
	#region 诊断类型

	/// <summary>
	/// 诊断类型枚举
	/// </summary>
	public enum EnumDiagnoseType
	{	
		/// <summary>
		/// 入院诊断
		/// </summary>
		IN = 1,
		/// <summary>
		/// 转入诊断
		/// </summary>
		TURNIN = 2,
		/// <summary>
		/// 出院诊断
		/// </summary>
		OUT = 3,
		/// <summary>
		/// 转出诊断
		/// </summary>
		TURNOUT = 4,
		/// <summary>
		/// 确诊诊断
		/// </summary>
		SURE = 5,
		/// <summary>
		/// 死亡诊断
		/// </summary>
		DEAD = 6,
		/// <summary>
		/// 术前诊断
		/// </summary>
		OPSFRONT = 7,
		/// <summary>
		/// 术后诊断
		/// </summary>
		OPSAFTER = 8,
		/// <summary>
		/// 感染诊断
		/// </summary>
		INFECT = 9,
		/// <summary>
		/// 损伤中毒诊断
		/// </summary>
		DAMNIFY = 10,
		/// <summary>
		/// 并发症诊断
		/// </summary>
		COMPLICATION = 11,
		/// <summary>
		/// 病理诊断
		/// </summary>
		PATHOLOGY = 12,
		/// <summary>
		/// 抢救诊断
		/// </summary>
		SAVE = 13,
		/// <summary>
		/// 病危诊断
		/// </summary>
		FAIL = 14,
		/// <summary>
		/// 门诊诊断
		/// </summary>
		CLINIC = 15,
		/// <summary>
		/// 其他诊断
		/// </summary>
		OTHER = 16,
		/// <summary>
		/// 结算诊断
		/// </summary>
		BALANCE = 17

	};
	
	#endregion
	
	#region 婚姻状态
	/// <summary>
	/// 婚姻状态
	/// </summary>
	public enum EnumMaritalStatus 
	{
		/// <summary>
		/// Single
		/// </summary>
		S=1,
		/// <summary>
		/// Married
		/// </summary>
		M,
		/// <summary>
		/// Divorced
		/// </summary>
		D,
		/// <summary>
		/// remarriage
		/// </summary>
		//R,
		/// <summary>
		/// Separated
		/// </summary>
		//A,
		/// <summary>
		/// Widowed
		/// </summary>
		W,
        /// <summary>
        /// Other
        /// </summary>
        O
	};
	
	#endregion
	
	#region 性别
	/// <summary>
	/// 性别
	/// </summary>
	public enum EnumSex
	{
		/// <summary>
		/// 男
		/// </summary>
		M=10,
		/// <summary>
		/// 女
		/// </summary>
		F=20,
		/// <summary>
		/// 其他
		/// </summary>
		O=3,
		/// <summary>
		/// 未知
		/// </summary>
		U=0,
        /// <summary>
		/// 全部
		/// </summary>
		A=4
	};
	
	#endregion

    //{1C0814FA-899B-419a-94D1-789CCC2BA8FF}
	#region 住院患者在院状态
	/// <summary>
	/// 住院患者在院状态
	/// </summary>
	public enum EnumInState
	{
		/// <summary>
		/// Registration 住院登记完成 等待接诊:0
		/// </summary>
		R =0,
		/// <summary>
		/// after Receiption,in 病房接诊完成 在院状态:1
		/// </summary>
		I =1,
		/// <summary>
		/// Balance  出院登记完成 结算状态:2
		/// </summary>
		B =2,
		/// <summary>
		/// out Balance出院结算完成:3
		/// </summary>
		O =3,
		/// <summary>
		///PreOut预约出院:4
		/// </summary>
		P =4,
		/// <summary>
		/// NoFee无费退院:5
		/// </summary>
		N =5,
		/// <summary>
		/// Close 封账状态:6
		/// </summary>
		C =6,
        /// <summary>
        /// 转住院
        /// </summary>
        E
	};
	#endregion
	
	#region	变更类型
    //{1C0814FA-899B-419a-94D1-789CCC2BA8FF}
	/// <summary>
	/// 变更类型
	/// </summary>
	public enum EnumShiftType 
	{
					
		/// <summary>
		/// 转科
		/// </summary>
		RD,
		/// <summary>
		/// 转床1
		/// </summary>
		RB,
		/// <summary>
		/// 转入
		/// </summary>
		RI,
		/// <summary>
		/// 转出
		/// </summary>
		RO,
		/// <summary>
		/// 接诊2
		/// </summary>
		K,
		/// <summary>
		/// 住院登记3
		/// </summary>
		B,
		/// <summary>
		/// 召回4
		/// </summary>
		C,
		/// <summary>
		/// 出院登记5
		/// </summary>
		O,
		/// <summary>
		/// 无费退院
		/// </summary>
		OF,
		/// <summary>
		/// 结算
		/// </summary>
		BA,
		/// <summary>
		/// 结算召回
		/// </summary>
		BB,
		/// <summary>
		///中途结算 
		/// </summary>
		MB,
		/// <summary>
		/// 患者信息修改
		/// </summary>
		F,
		/// <summary>
		/// 超标床和超标空调修改
		/// </summary>
		LB,
		/// <summary>
		/// 公费日限额变更
		/// </summary>
		DL,
		/// <summary>
		/// 公费日限额累计
		/// </summary>
		BT,
		/// <summary>
		/// 结算清单打印
		/// </summary>
		BP,
		/// <summary>
		/// 身份变更
		/// </summary>
		CP,
		/// <summary>
		/// 开医疗收费证明单
		/// </summary>
		ZM,
        /// <summary>
        /// 变更患者住院科室
        /// </summary>
        CD,
        /// <summary>
        /// 留观登记
        /// </summary>
        EB,
        /// <summary>
        /// 住院登记3
        /// </summary>
        EK,
        /// <summary>
        /// 召回4
        /// </summary>
        EC,
        /// <summary>
        /// 出院登记5
        /// </summary>
        EO,
        /// <summary>
        /// 转入病区{F0BF027A-9C8A-4bb7-AA23-26A5F3539586}
        /// </summary>
        CN,
        /// <summary>
        /// 转出病区{F0BF027A-9C8A-4bb7-AA23-26A5F3539586}
        /// </summary>
        CNO,
        /// <summary>
        /// 留观转住院
        /// </summary>
        CPI,
        /// <summary>
        /// 留观住院
        /// </summary>
        CI,
        /// <summary>
        /// 留观转住院召回
        /// </summary>
        IC,
        //{D97A6AA0-5AFB-443f-B74D-1AD1604B1567} 增加开关帐日志 yangw 20100907
        /// <summary>
        /// 关账
        /// </summary>
        AC,
        /// <summary>
        /// 开账
        /// </summary>
        AO,
         /// <summary>
        /// 包床
        /// </summary>
        ABD,
        /// <summary>
        /// 解除包床
        /// </summary>
        RBD,
        /// <summary>
        /// 费用开锁
        /// </summary>
        UNFL,
        /// <summary>
        /// 费用关锁
        /// </summary>
        FL,
        /// <summary>
        /// 警戒线修改
        /// </summary>
        AWL,
        /// <summary>
        /// 取消接诊
        /// </summary>
        CK
	}
	#endregion

    #region 预约状态
    /// <summary>
    /// 预约状态
    /// </summary>
    public enum EnumBookingState
    {
        /// <summary>
        /// 预约申请
        /// </summary>
        Apply = 0,
        /// <summary>
        /// 预约登记
        /// </summary>
        Booking = 1,
        /// <summary>
        /// 取消登记
        /// </summary>
        CancelBooking = 2,
        /// <summary>
        /// 执行
        /// </summary>
        Execute = 3,
        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 4
    }
    #endregion

    #region 挂号状态
    
    /// <summary>
    /// 挂号状态
    /// </summary>
    public enum EnumRegisterStatus
    {
        /// <summary>
        /// 退费
        /// </summary>
        Back,
        /// <summary>
        /// 有效
        /// </summary>
        Valid,
        /// <summary>
        /// 作废
        /// </summary>
        Cancel,
        /// <summary>
        /// 废号
        /// </summary>
        Bad


    }
    #endregion

    #region 挂号费用明细状态

    /// <summary>
    /// 挂号费用明细状态
    /// </summary>
    public enum EnumAccountCardFeeStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        Cancel,

        /// <summary>
        /// 有效
        /// </summary>
        Valid,

        /// <summary>
        /// 退费
        /// </summary>
        Back

    }

    #endregion

    #region 挂号类型
    /// <summary>
    /// 挂号类型
    /// </summary>
    public enum EnumRegType
    {
        /// <summary>
        /// 现场挂号
        /// </summary>
        Reg,
        /// <summary>
        /// 预约挂号
        /// </summary>
        Pre,
        /// <summary>
        /// 特诊挂号
        /// </summary>
        Spe
    }
    #endregion

    #region 挂号排班类型
    /// <summary>
    /// 排班类型
    /// </summary>
    public enum EnumSchemaType
    {
        /// <summary>
        /// 科室排班
        /// </summary>
        Dept,
        /// <summary>
        /// 医生排班
        /// </summary>
        Doct
    }
    #endregion

    #region 收费加载列表类型
    /// <summary>
    /// 加载项目类别
    /// </summary>
    public enum ItemKind
    {
        /// <summary>
        /// 药品
        /// </summary>
        Pharmacy,
        /// <summary>
        /// 非药品
        /// </summary>
        Undrug,
        /// <summary>
        /// 全部
        /// </summary>
        All
    }
    #endregion

    #region 项目类型
    /// <summary>
    /// 项目类型
    /// </summary>
    public enum EnumItemType
    {
        /// <summary>
        /// 非药品
        /// </summary>
        UnDrug=0,
        /// <summary>
        /// 药品
        /// </summary>
        Drug,
        /// <summary>
        /// 物资项目
        /// </summary>
        MatItem
    }
    #endregion

    /// <summary>
    /// 欠费判断提示类型Y：只提示欠费,不可以收费 M：提示欠费，还还可收费
    /// N：不判断是否欠费
    /// </summary>
    public enum MessType
    {
        /// <summary>
        /// 提示欠费,不可以收费
        /// </summary>
        Y,
        /// <summary>
        /// 提示欠费，还还可收费
        /// </summary>
        M,
        /// <summary>
        /// 不判断是否欠费
        /// </summary>
        N,

    }

    /// <summary>
    /// 结算类型
    /// </summary>
    public enum EBlanceType
    {
        /// <summary>
        /// 出院结算
        /// </summary>
        Out = 0,
        /// <summary>
        /// 中途结算
        /// </summary>
        Mid = 1,
        /// <summary>
        /// 欠费结算:欠费患者按照费用全额结算
        /// </summary>
        Owe = 2,
        /// <summary>
        /// 欠费中结:欠费患者按照预交金全额结算
        /// </summary>
        OweMid=3,
        /// <summary>
        /// 按照项目结算
        /// </summary>
        ItemBalance=4,
        /// <summary>
        /// 不欠费宰账结算:退费里显示宰账
        /// </summary>
        OutCredit = 5
       
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum EnumPatientShiftValid
    {
        /// <summary>
        /// 出院登记
        /// </summary>
        O,
        /// <summary>
        /// 出院召回
        /// </summary>
        C,
        /// <summary>
        /// 转科
        /// </summary>
        R,
    }

    //{A45EE85D-B1E3-4af0-ACAD-9DAF65610611}
    /// <summary>
    /// 警戒线设置类型
    /// </summary>
    public enum EnumAlertType
    {
        /// <summary>
        /// 按金钱设置
        /// </summary>
        M = 0,
        /// <summary>
        /// 按时间段设置
        /// </summary>
        D=1
    }

    /// <summary>
    /// 医生站权限判断
    /// </summary>
    public enum DoctorPrivType
    {
        /// <summary>
        /// 处方权限
        /// </summary>
        RecipePriv = 0,
        /// <summary>
        /// 组套管理权
        /// </summary>
        GroupManager = 1,
        /// <summary>
        /// 等级药品
        /// </summary>
        LevelDrug = 2,
        /// <summary>
        /// 特限药品
        /// </summary>
        SpecialDrug = 3,
        /// <summary>
        /// 特限非药品
        /// </summary>
        SpecialUndrug = 4
    }
}