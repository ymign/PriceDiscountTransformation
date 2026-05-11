using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Order
{
    /// <summary>
    /// 附材维护实体
    /// </summary>
    [Serializable]
    public class OrderSubtblNew : Neusoft.FrameWork.Models.NeuObject
    {
        public OrderSubtblNew()
        {
        }

        /// <summary>
        /// 适用范围：0 门诊；1 住院；3 全部
        /// </summary>
        private string area;

        /// <summary>
        /// 适用范围：0 门诊；1 住院；3 全部
        /// </summary>
        public string Area
        {
            get
            {
                return area;
            }
            set
            {
                area = value;
            }
        }

        /// <summary>
        /// 医嘱类别（住院）：全部、临嘱、长嘱
        /// </summary>
        private string orderType;

        /// <summary>
        /// 医嘱类别（住院）：全部、临嘱、长嘱
        /// </summary>
        public string OrderType
        {
            get
            {
                return orderType;
            }
            set
            {
                orderType = value;
            }
        }

        /// <summary>
        /// 用法分类，0 药品按用法，1 非药品按项目代码
        /// </summary>
        private string typeCode;

        /// <summary>
        /// 用法分类，0 药品按用法，1 非药品按项目代码
        /// </summary>
        public string TypeCode
        {
            get
            {
                return typeCode;
            }
            set
            {
                typeCode = value;
            }
        }

        /// <summary>
        /// 科室代码，全院统一附材'ROOT'
        /// </summary>
        private string dept_code;

        /// <summary>
        /// 科室代码，全院统一附材'ROOT'
        /// </summary>
        public string Dept_code
        {
            get
            {
                return dept_code;
            }
            set
            {
                dept_code = value;
            }
        }

        /// <summary>
        /// 组范围：0 每组收取、1 第一组收取、2 第二组起加收
        /// </summary>
        private string combArea;

        /// <summary>
        /// 组范围：0 每组收取、1 第一组收取、2 第二组起加收
        /// </summary>
        public string CombArea
        {
            get
            {
                return combArea;
            }
            set
            {
                combArea = value;
            }
        }

        /// <summary>
        /// 是否首次收取（长嘱） 0 全部收取 1 首次执行收取（类似AST皮试）
        /// </summary>
        private string firstFeeFlag;

        /// <summary>
        /// 是否首次收取（长嘱） 0 全部收取 1 首次执行收取（类似AST皮试）
        /// </summary>
        public string FirstFeeFlag
        {
            get
            {
                return firstFeeFlag;
            }
            set
            {
                firstFeeFlag = value;
            }
        }

        /// <summary>
        /// 收费规则：固定数量、*最大院注、*组内品种数、*最大医嘱数量、*频次、*组内医嘱数量、*天数、*院注天数（院注次数/频次 上取整）
        /// </summary>
        private string feeRule;

        /// <summary>
        /// 收费规则：固定数量、*最大院注、*组内品种数、*最大医嘱数量、*频次、*组内医嘱数量、*天数、*院注天数（院注次数/频次 上取整）
        /// </summary>
        public string FeeRule
        {
            get
            {
                return feeRule;
            }
            set
            {
                feeRule = value;
            }
        }

        /// <summary>
        /// 项目
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject item;

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject oper;

        /// <summary>
        /// 收费数量
        /// </summary>
        private decimal qty = 0;

        /// <summary>
        /// 收费数量规则
        /// </summary>
        public decimal Qty
        {
            get
            {
                return qty;
            }
            set
            {
                qty = value;
            }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime operDate = DateTime.Now;

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperDate
        {
            get
            {
                return operDate;
            }
            set
            {
                operDate = value;
            }
        }

        /// <summary>
        /// 操作员
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Oper
        {
            get
            {
                if (oper == null)
                {
                    oper = new Neusoft.FrameWork.Models.NeuObject();
                }
                return this.oper;
            }
            set
            {
                this.oper = value;
            }
        }

        /// <summary>
        /// 项目
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Item
        {
            get
            {
                if (item == null)
                {
                    item = new Neusoft.FrameWork.Models.NeuObject();
                }
                return this.item;
            }
            set
            {
                this.item = value;
            }
        }

        /// <summary>
        /// 限制类别：0 不限制 1 婴儿使用 2 非婴儿使用
        /// </summary>
        private string limitType;

        /// <summary>
        /// 限制类别：0 不限制 1 婴儿使用 2 非婴儿使用
        /// </summary>
        public string LimitType
        {
            get
            {
                if (limitType == null)
                {
                    limitType = string.Empty;
                }
                return limitType;
            }
            set
            {
                limitType = value;
            }
        }

        //2011-10-16 增加 houwb 

        /// <summary>
        /// 是否允许重复收取
        /// </summary>
        private bool isAllowReFee;

        /// <summary>
        /// 是否允许重复收取
        /// </summary>
        public bool IsAllowReFee
        {
            get
            {
                return isAllowReFee;
            }
            set
            {
                isAllowReFee = value;
            }
        }

        /// <summary>
        /// 是否允许弹出选择
        /// </summary>
        private bool isAllowPopChose;

        /// <summary>
        /// 是否允许弹出选择
        /// </summary>
        public bool IsAllowPopChose
        {
            get
            {
                return isAllowPopChose;
            }
            set
            {
                isAllowPopChose = value;
            }
        }

        /// <summary>
        /// 是否根据每次量限制带出
        /// </summary>
        private bool isCalculateByOnceDose;

        /// <summary>
        /// 是否根据每次量限制带出
        /// </summary>
        public bool IsCalculateByOnceDose
        {
            get
            {
                return isCalculateByOnceDose;
            }
            set
            {
                isCalculateByOnceDose = value;
            }
        }

        /// <summary>
        /// 每次量单位
        /// </summary>
        private string doseUnit;

        /// <summary>
        /// 每次量单位
        /// </summary>
        public string DoseUnit
        {
            get
            {
                if (doseUnit == null)
                {
                    doseUnit = string.Empty;
                }
                return doseUnit;
            }
            set
            {
                doseUnit = value;
            }
        }

        /// <summary>
        /// 每次量开始值
        /// </summary>
        private decimal onceDoseFrom;

        /// <summary>
        /// 每次量开始值
        /// </summary>
        public decimal OnceDoseFrom
        {
            get
            {
                return onceDoseFrom;
            }
            set
            {
                onceDoseFrom = value;
            }
        }

        /// <summary>
        /// 每次量结束值
        /// </summary>
        private decimal onceDoseTo;

        /// <summary>
        /// 每次量结束值
        /// </summary>
        public decimal OnceDoseTo
        {
            get
            {
                return onceDoseTo;
            }
            set
            {
                onceDoseTo = value;
            }
        }

        //2012-4-9 增加 houwb

        /// <summary>
        /// 扩展1
        /// </summary>
        private string extend1;

        /// <summary>
        /// 扩展1 (目前妇幼用于处理药袋费用带出）
        /// </summary>
        public string Extend1
        {
            get
            {
                if (extend1 == null)
                {
                    extend1 = string.Empty;
                }
                return extend1;
            }
            set
            {
                extend1 = value;
            }
        }

        /// <summary>
        /// 扩展2
        /// </summary>
        private string extend2;

        /// <summary>
        /// 扩展2
        /// </summary>
        public string Extend2
        {
            get
            {
                if (extend2 == null)
                {
                    extend2 = string.Empty;
                }
                return extend2;
            }
            set
            {
                extend2 = value;
            }
        }

        /// <summary>
        /// 扩展3
        /// </summary>
        private string extend3;

        /// <summary>
        /// 扩展3
        /// </summary>
        public string Extend3
        {
            get
            {
                if (extend3 == null)
                {
                    extend3 = string.Empty;
                }
                return extend3;
            }
            set
            {
                extend3 = value;
            }
        }
    }
}
