using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Order.Inpatient
{
    /// <summary>
    /// <br></br>
    /// [功能描述: 住院执行单明细]<br></br>
    /// [创 建 者: houwb]<br></br>
    /// [创建时间: 2014年8月19日]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [Serializable]
    public class ExecBillDetail : Neusoft.FrameWork.Models.NeuObject
    {

        //ID表示执行单编号

        /// <summary>
        /// 护士站编码
        /// </summary>
        private string nurseStationID;

        /// <summary>
        /// 护士站编码
        /// </summary>
        public string NurseStationID
        {
            get
            {
                return nurseStationID;
            }
            set
            {
                nurseStationID = value;
            }
        }

        /// <summary>
        /// 医嘱类型编码
        /// </summary>
        private string orderTypeID;

        /// <summary>
        /// 医嘱类型编码
        /// </summary>
        public string OrderTypeID
        {
            get
            {
                return orderTypeID;
            }
            set
            {
                orderTypeID = value;
            }
        }

        /// <summary>
        /// 项目类型编码
        /// </summary>
        private string classCode;

        /// <summary>
        /// 项目类型编码
        /// </summary>
        public string ClassCode
        {
            get
            {
                return classCode;
            }
            set
            {
                classCode = value;
            }
        }

        /// <summary>
        /// 药品类别编码
        /// </summary>
        private string drugTypeID;

        /// <summary>
        /// 药品类别编码
        /// </summary>
        public string DrugTypeID
        {
            get
            {
                return drugTypeID;
            }
            set
            {
                drugTypeID = value;
            }
        }

        /// <summary>
        /// 用法编码
        /// </summary>
        private string usageID;

        /// <summary>
        /// 用法编码
        /// </summary>
        public string UsageID
        {
            get
            {
                return usageID;
            }
            set
            {
                usageID = value;
            }
        }

        /// <summary>
        /// 项目（项目执行单）
        /// </summary>
        private Neusoft.HISFC.Models.Base.Item item;

        /// <summary>
        /// 项目（项目执行单）
        /// </summary>
        public Neusoft.HISFC.Models.Base.Item Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        /// <summary>
        /// 操作员信息
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = null;

        /// <summary>
        /// 操作员信息
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get
            {
                if (oper == null)
                {
                    oper = new Neusoft.HISFC.Models.Base.OperEnvironment();
                }

                return oper;
            }
            set
            {
                oper = value;
            }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new ExecBillDetail Clone()
        {
            // TODO:  添加 Order.Clone 实现
            ExecBillDetail obj = base.Clone() as ExecBillDetail;
            obj.item = this.Item.Clone();
            obj.Oper = this.Oper.Clone();
            return obj;
        }
    }
}
