using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.RADT
{
    /// <summary>
    /// [功能描述: 转科申请实体]<br></br>
    /// [创 建 者: zhaorong]<br></br>
    /// [创建时间: 2013-9-13]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间=''
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    public class ShiftApply : Neusoft.FrameWork.Models.NeuObject 
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ShiftApply()
        {

        }

        #region 变量
        /// <summary>
        /// 住院流水号
        /// </summary>
        private string inPatientNo;
        /// <summary>
        /// 发生序号
        /// </summary>
        private int happenNo;
        /// <summary>
        /// 转往科室
        /// </summary>
        private string newDeptCode;
        /// <summary>
        /// 原来科室
        /// </summary>
        private string oldDeptCode;
        /// <summary>
        /// 转往护理站代码
        /// </summary>
        private string newNurseCellCode;
        /// <summary>
        /// 护士站代码
        /// </summary>
        private string nurseCellCode;
        /// <summary>
        /// 当前状态,0未生效,1转科申请,2确认,3取消申请
        /// </summary>
        private string shiftState;
        /// <summary>
        /// 确认环境(确认人,转科确认时间)
        /// </summary>
        private OperEnvironment confirmOper;
        /// <summary>
        /// 取消环境(取消人,取消申请时间)
        /// </summary>
        private OperEnvironment cancelOper;
        /// <summary>
        /// 备注
        /// </summary>
        private string mark;
        /// <summary>
        /// 操作环境(操作员，操作日期)
        /// </summary>
        private OperEnvironment oper;
        /// <summary>
        /// 原病床号
        /// </summary>
        private string oldBedCode;
        /// <summary>
        /// 转往床号
        /// </summary>
        private string newBedCode;
        #endregion

        #region 属性
        /// <summary>
        /// 住院流水号
        /// </summary>
        public string InPatientNo
        {
            get
            {
                return this.inPatientNo;
            }
            set
            {
                this.inPatientNo = value;
            }
        }
        /// <summary>
        /// 发生序号
        /// </summary>
        public int HappenNo
        {
            get
            {
                return this.happenNo;
            }
            set
            {
                this.happenNo = value;
            }
        }
        /// <summary>
        /// 转往科室
        /// </summary>
        public string NewDeptCode
        {
            get
            {
                return this.newDeptCode;
            }
            set
            {
                this.newDeptCode = value;
            }
        }
        /// <summary>
        /// 原来科室
        /// </summary>
        public string OldDeptCode
        {
            get
            {
                return this.oldDeptCode;
            }
            set
            {
                this.oldDeptCode = value;
            }
        }
        /// <summary>
        /// 转往护理站代码
        /// </summary>
        public string NewNurseCellCode
        {
            get
            {
                return this.newNurseCellCode;
            }
            set
            {
                this.newNurseCellCode = value;
            }
        }
        /// <summary>
        /// 护士站代码
        /// </summary>
        public string NurseCellCode
        {
            get
            {
                return this.nurseCellCode;
            }
            set
            {
                this.nurseCellCode = value;
            }
        }
        /// <summary>
        /// 当前状态,0未生效,1转科申请,2确认,3取消申请
        /// </summary>
        public string ShiftState
        {
            get
            {
                return this.shiftState;
            }
            set
            {
                this.shiftState = value;
            }
        }
        /// <summary>
        /// 确认环境(确认人,转科确认时间)
        /// </summary>
        public OperEnvironment ConfirmOper
        {
            get
            {
                if (this.confirmOper == null)
                {
                    this.confirmOper = new OperEnvironment();
                }
                return this.confirmOper;
            }
            set
            {
                this.confirmOper = value;
            }
        }
        /// <summary>
        /// 取消环境(取消人,取消申请时间)
        /// </summary>
        public OperEnvironment CancelOper
        {
            get
            {
                if (this.cancelOper == null)
                {
                    this.cancelOper = new OperEnvironment();
                }
                return this.cancelOper;
            }
            set
            {
                this.cancelOper = value;
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Mark
        {
            get
            {
                return this.mark;
            }
            set
            {
                this.mark = value;
            }
        }
        /// <summary>
        /// 操作环境(操作员，操作日期)
        /// </summary>
        public OperEnvironment Oper
        {
            get
            {
                if (this.oper == null)
                {
                    this.oper = new OperEnvironment();
                }
                return this.oper;
            }
            set
            {
                this.oper = value;
            }
        }
        /// <summary>
        /// 原病床号
        /// </summary>
        public string OldBedCode
        {
            get
            {
                return this.oldBedCode;
            }
            set
            {
                this.oldBedCode = value;
            }
        }
        /// <summary>
        /// 转往床号
        /// </summary>
        public string NewBedCode
        {
            get
            {
                return this.newBedCode;
            }
            set
            {
                this.newBedCode = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new ShiftApply Clone()
        {
            ShiftApply shiftApply = base.Clone() as ShiftApply;
            shiftApply.ConfirmOper = this.ConfirmOper.Clone();
            shiftApply.CancelOper = this.CancelOper.Clone();
            shiftApply.Oper = this.Oper.Clone();
            return shiftApply;
        }
        #endregion
    }
}
