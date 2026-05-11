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
    public class ExecBill : Neusoft.FrameWork.Models.NeuObject
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
        /// 执行单打印类型，0病人分组(无护理组)，1病人分组(按护理组)，2(项目分组)
        /// </summary>
        private string billKind;

        /// <summary>
        /// 执行单打印类型，0病人分组(无护理组)，1病人分组(按护理组)，2(项目分组)
        /// </summary>
        public string BillKind
        {
            get
            {
                return billKind;
            }
            set
            {
                billKind = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string mark;

        /// <summary>
        /// 执行单备注
        /// </summary>
        public string Mark
        {
            get
            {
                return mark;
            }
            set
            {
                mark = value;
            }
        }

        /// <summary>
        /// 项目执行单标记：0  不是;1 是
        /// </summary>
        private string itemFlag;

        /// <summary>
        /// 项目执行单标记：0  不是;1 是
        /// </summary>
        public string ItemFlag
        {
            get
            {
                return itemFlag;
            }
            set
            {
                itemFlag = value;
            }
        }

        /// <summary>
        /// 排序号
        /// </summary>
        private int sortID;

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortID
        {
            get
            {
                return sortID;
            }
            set
            {
                sortID = value;
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

        public new ExecBill Clone()
        {
            ExecBill bill = base.Clone() as ExecBill;
            bill.Oper = this.Oper.Clone();
            return bill;
        }
    }
}
