using System;
using Neusoft.HISFC.Models.Base;
using Neusoft.FrameWork.Models;
namespace Neusoft.HISFC.Models.Order.OutPatient
{
    /// <summary>
    /// Neusoft.HISFC.Models.Order.OutPatient.Order<br></br>
    /// [功能描述: 门诊医嘱资料实体]<br></br>
    /// [创 建 者: 李云凡]<br></br>
    /// [创建时间: 2006-09-10]<br></br>
    /// <修改记录
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///		/>
    /// </summary>
    [Serializable]
    public class Order : Neusoft.HISFC.Models.Order.Order
    {
        public Order()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 变量

        #region 私有

        /// <summary>
        /// 看诊序号
        /// </summary>
        private string seeno;

        /// <summary>
        /// 收费序列
        /// </summary>
        private string recipeSeq;

        /// <summary>
        /// 挂号日期
        /// </summary>
        private DateTime regDate;

        /// <summary>
        /// 项目费用信息
        /// </summary>
        private FT ft;

        /// <summary>
        /// 收费人员
        /// </summary>
        private Base.OperEnvironment chargeOper;

        /// <summary>
        /// 确认人
        /// </summary>
        private Base.OperEnvironment confirmOper;

        /// <summary>
        /// 是否已经收费
        /// </summary>
        protected bool isHaveCharged = false;

        /// <summary>
        /// 是否需要终端确认
        /// </summary>
        private bool isNeedConfirm = false;

        /// <summary>
        /// 用药天数
        /// </summary>
        private int useDays;

        /// <summary>
        ///  处方类型
        ///  用于新医嘱开立方式
        ///  add by liuww 2011-3-8
        /// </summary>
        private NeuObject recipeType;

        /// <summary>
        /// 医院编码
        /// </summary>
        private string hosCode;

        private Neusoft.HISFC.Models.Order.Inpatient.OrderExtend ordExtend;


        /// <summary>
        /// 是否外延处方标记 - MK
        /// </summary>
        private bool isExtendRecipe;
        

        #endregion

        #endregion

        #region 属性

        /// <summary>
        /// 看诊序号
        /// </summary>
        public string SeeNO
        {
            get
            {
                if (seeno == null)
                {
                    seeno = string.Empty;
                }
                return this.seeno;
            }
            set
            {
                this.seeno = value;
            }
        }

        /// <summary>
        /// 收费序列
        /// </summary>
        public string ReciptSequence
        {
            get
            {
                if (recipeSeq == null)
                {
                    recipeSeq = string.Empty;
                }
                return this.recipeSeq;
            }
            set
            {
                this.recipeSeq = value;
            }
        }

        /// <summary>
        /// 挂号日期
        /// </summary>
        public DateTime RegTime
        {
            get
            {
                return this.regDate;
            }
            set
            {
                this.regDate = value;
            }
        }

        /// <summary>
        /// 项目费用信息
        /// </summary>
        public FT FT
        {
            get
            {
                if (ft == null)
                {
                    ft = new FT();
                }
                return ft;
            }
            set
            {
                this.ft = value;
            }
        }

        /// <summary>
        /// 收费人员
        /// </summary>
        public Base.OperEnvironment ChargeOper
        {
            get
            {
                if (chargeOper == null)
                {
                    chargeOper = new OperEnvironment();
                }
                return this.chargeOper;
            }
            set
            {
                this.chargeOper = value;
            }
        }

        /// <summary>
        /// 确认人
        /// </summary>
        public Base.OperEnvironment ConfirmOper
        {
            get
            {
                if (confirmOper == null)
                {
                    confirmOper = new OperEnvironment();
                }
                return this.confirmOper;
            }
            set
            {
                this.confirmOper = value;
            }
        }

        /// <summary>
        /// 是否已经收费
        /// </summary>
        public bool IsHaveCharged
        {
            get
            {
                return isHaveCharged;
            }
            set
            {
                isHaveCharged = value;
            }
        }

        /// <summary>
        /// 是否需要终端确认
        /// </summary>
        public bool IsNeedConfirm
        {
            get
            {
                return this.isNeedConfirm;
            }
            set
            {
                this.isNeedConfirm = value;
            }
        }

        /// <summary>
        /// 用药天数
        /// </summary>
        public int UseDays
        {
            get
            {
                return useDays;
            }
            set
            {
                useDays = value;
            }
        }

        /// <summary>
        ///  处方类型
        ///  用于新医嘱开立方式
        ///  add by liuww 2011-3-8
        /// </summary>
        public NeuObject RecipeType
        {
            get
            {
                if (recipeType == null)
                {
                    recipeType = new NeuObject();
                }
                return this.recipeType;
            }
            set
            {
                this.recipeType = value;
            }
        }

        /// <summary>
        /// 医院编码
        /// </summary>
        public string HosCode
        {
            get { return hosCode; }
            set { hosCode = value; }
        }
        
        /// <summary>
        /// 医嘱扩展表
        /// </summary>
        public Neusoft.HISFC.Models.Order.Inpatient.OrderExtend OrdExtend
        {
            get
            {
                if (this.ordExtend == null)
                {
                    this.ordExtend = new Neusoft.HISFC.Models.Order.Inpatient.OrderExtend();
                }
                return this.ordExtend;
            }
            set
            {
                this.ordExtend = value;
            }
        }

        /// <summary>
        /// 是否外延处方标记 - MK
        /// </summary>
        public bool IsExtendRecipe
        {
            get
            {
                return this.isExtendRecipe;
            }
            set
            {
                this.isExtendRecipe = value;
            }
        }

        #endregion

        #region 作废的

        /// <summary>
        /// 看诊序号
        /// </summary>
        [Obsolete("用SeeNO", true)]
        public string SeeNo
        {
            get
            {
                return seeno;
            }
            set
            {
                seeno = value;
            }
        }

        /// <summary>
        /// 项目内流水号
        /// </summary>
        [Obsolete("用SequenceNO", true)]
        public int SeqNo
        {
            get
            {
                return int.Parse(base.ID);
            }
            set
            {
                base.ID = value.ToString();
            }
        }

        /// <summary>
        /// 收费序列
        /// </summary>
        [Obsolete("用ReciptSequence", true)]
        public string RecipeSeq
        {
            get
            {
                return this.recipeSeq;
            }
            set
            {
                this.recipeSeq = value;
            }
        }

        /// <summary>
        /// 挂号日期
        /// </summary>
        [Obsolete("用RegTime", true)]
        public DateTime RegDate
        {
            get
            {
                return regDate;
            }
            set
            {
                regDate = value;
            }
        }

        /// <summary>
        /// 收费者
        /// </summary>
        [Obsolete("用ChargeOper.Oper代替", true)]
        public NeuObject ChargeUser;

        /// <summary>
        /// 收费科室
        /// </summary>
        [Obsolete("用ChargeOper.Dept代替", true)]
        public NeuObject ChargeDept;

        /// <summary>
        /// 收费时间
        /// </summary>
        [Obsolete("用ChargeOper.OperTime代替", true)]
        public DateTime ChargeTime;

        /// <summary>
        /// 确认科室
        /// </summary>
        [Obsolete("用ConfirmOper.Dept代替", true)]
        public NeuObject ComfirmDept;
        /// <summary>
        /// 确认人
        /// </summary>
        [Obsolete("用ConfirmOper.Oper代替", true)]
        public NeuObject User_Comfirm;

        #endregion

        #region 方法

        #region 克隆

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Order Clone()
        {
            // TODO:  添加 Order.Clone 实现
            Order obj = base.Clone() as Order;
            obj.chargeOper = this.ChargeOper.Clone();
            obj.confirmOper = this.ConfirmOper.Clone();
            obj.ft = this.FT.Clone();
            return obj;
        }

        #endregion

        #endregion
    }


    public enum EnumOutPatientBill
    {
        /// <summary>
        /// 0 全部
        /// </summary>
        [Neusoft.FrameWork.Public.Description("全部")]
        AllBill = 0,

        /// <summary>
        /// 1 处方单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("处方单")]
        RecipeBill,

        /// <summary>
        /// 2 注射单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("注射单")]
        InjectBill,

        /// <summary>
        /// 3 检查单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("检查单")]
        PacsBill,

        /// <summary>
        /// 4 检验单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("检验单")]
        LisBill,

        /// <summary>
        /// 5 治疗单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("治疗单")]
        TreatmentBill,

        /// <summary>
        /// 6 诊疗单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("诊疗单")]
        ClinicsBill,

        /// <summary>
        /// 7 指引单（门诊记录单）
        /// </summary>
        [Neusoft.FrameWork.Public.Description("门诊记录")]
        GuideBill,

        /// <summary>
        /// 8 诊断单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("诊断证明")]
        DiagNoseBill,

        /// <summary>
        /// 9 博济处方单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("博济处方单")]
        OutRecipeBill,

        /// <summary>
        /// 10 凤凰园处方单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("凤凰园处方单")]
        FHYRecipeBill,
        /// <summary>
        /// 11 凤凰园审批单
        /// </summary>
        [Neusoft.FrameWork.Public.Description("凤凰园审批单")]
        FHYDrugCheckBill=11
        
    }
}
