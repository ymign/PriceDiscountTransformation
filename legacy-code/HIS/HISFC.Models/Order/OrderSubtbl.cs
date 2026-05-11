using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Order
{
    /// <summary>
    /// 医嘱辅材
    /// </summary>
    [Serializable]
    public class OrderSubtbl : Neusoft.FrameWork.Models.NeuObject
    {
        public OrderSubtbl()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 用法
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject usage;

        /// <summary>
        /// 项目
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject item;

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject oper;

        /// <summary>
        /// 收费数量规则
        /// </summary>
        private int qtyRule=0;
        /// <summary>
        /// 收费数量规则
        /// </summary>
        public int QtyRule
        {
            get
            {
                return qtyRule;
            }
            set
            {
                qtyRule = value;
            }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        private DateTime operDate = DateTime .Now;
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
        /// 用法
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Usage
        {
            get
            {
                if (usage == null)
                {
                    usage = new Neusoft.FrameWork.Models.NeuObject();
                }
                return this.usage;
            }
            set
            {
                this.usage = value;
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

        #region 克隆

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new OrderSubtbl Clone()
        {
            OrderSubtbl orderSubtbl = base.Clone() as OrderSubtbl;
            orderSubtbl.Oper = this.Oper.Clone();
            orderSubtbl.Item = this.Item.Clone();
            orderSubtbl.Usage = this.Usage.Clone();

            return orderSubtbl;
        }

        #endregion
    }
}
