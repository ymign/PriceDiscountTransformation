using System;
using System.Collections.Generic;

using System.Text;

namespace Neusoft.HISFC.Models.PacsApply
{
    public class PacsItem:Neusoft.FrameWork.Models.NeuObject
    {

        private string applicationID;
        /// <summary>
        ///  申请单ID
        /// </summary>
        public String ApplicationID
        {
            get
            {
                return ApplicationID;
            }
            set
            {
                this.ApplicationID = value;
            }
        }


        private string spellCode;
        /// <summary>
        /// 拼音码
        /// </summary>
        public String SpellCode
        {
            get
            {
                return spellCode;
            }
            set
            {
                this.spellCode = value;
            }
        }

        
        /// <summary>
        /// 项目总费用
        /// </summary>
        private string totleCost;
        public String TotleCost
        {
            get
            {
                return totleCost;
            }
            set
            {
                this.totleCost = value;
            }
        }

        private string cost;
        /// <summary>
        /// 项目单价
        /// </summary>
        public String Cost
        {
            get
            {
                return cost;
            }
            set
            {
                this.cost = value;
            }
        }


        private string itemID;

        /// <summary>
        /// His项目ID编码
        /// </summary>
        public string ItemID
        {
            get { return this.itemID; }
            set { this.itemID = value; }
        }


        private string orderID;

        /// <summary>
        /// 住院医嘱ID或门诊SEQUENCE_NO
        /// </summary>
        public string OrderID
        {
            get { return this.orderID; }
            set { this.orderID = value; }
        }




        private Neusoft.FrameWork.Models.NeuObject checkPosition = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// PACS检查部位
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject CheckPosition
        {
            get
            {
                return checkPosition;
            }
            set
            {
                this.checkPosition = value;
            }
        }

        private Neusoft.FrameWork.Models.NeuObject checkMethod = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// PACS检查方法
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject CheckMethod
        {
            get
            {
                return checkMethod;
            }
            set
            {
                this.checkMethod = value;
            }
        }

        private Neusoft.FrameWork.Models.NeuObject checkType = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// PACS设备类型（与申请单类型通用）
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject CheckType
        {
            get
            {
                return checkType;
            }
            set
            {
                this.checkType = value;
            }
        }

        private DateTime operDate;
        /// <summary>
        ///检查消耗时间
        /// </summary>
        public DateTime OperDate
        {
            get
            {
                return operDate;
            }
            set
            {
                this.operDate = value;
            }
        }

        private string oper ;
        /// <summary>
        ///操作（确认）医生
        /// </summary>
        public String Oper
        {
            get
            {
                return oper;
            }
            set
            {
                this.oper = value;
            }
        }


        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new PacsItem Clone()
        {
            PacsItem pacsItem = base.Clone() as PacsItem;

            return pacsItem;
        }

        
    }
}
