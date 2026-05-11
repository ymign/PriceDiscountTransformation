using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    ///<br> [功能描述: 轮转模板类]</br>
    ///<br>[创 建 者: 欧宪成]</br>
    ///<br>[创建时间: 2008-07]</br>
    /// </summary>
    [System.Serializable]
    public class TurnMode : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段

        /// <summary>
        /// 轮转方式
        /// </summary>
        string cycleType;

        /// <summary>
        /// 轮转科室
        /// </summary>
        string cycleDeptCode;

        /// <summary>
        /// 轮转时间
        /// </summary>
        int cycleNum;

        /// <summary>
        /// 操作员
        /// </summary>
        Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion

        #region 属性

        /// <summary>
        /// 轮转方式
        /// </summary>
        public string CycleType
        {
            get
            {
                return cycleType;
            }
            set
            {
                cycleType = value;
            }
        }

        /// <summary>
        /// 轮转科室
        /// </summary>
        public string CycleDeptCode
        {
            get
            {
                return cycleDeptCode;
            }
            set
            {
                cycleDeptCode = value;
            }
        }

        /// <summary>
        /// 轮转时间
        /// </summary>
        public int CycleNum
        {
            get
            {
                return cycleNum;
            }
            set
            {
                cycleNum = value;
            }
        }

        /// <summary>
        /// 操作员
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get
            {
                return oper;
            }
            set
            {
                oper = value;
            }
        }

        #endregion

        #region 方法

        public new TurnMode Clone()
        {
            TurnMode turnMode = base.Clone() as TurnMode;
            turnMode.Oper = this.Oper;
            return turnMode;
        }

        #endregion

    }
}
