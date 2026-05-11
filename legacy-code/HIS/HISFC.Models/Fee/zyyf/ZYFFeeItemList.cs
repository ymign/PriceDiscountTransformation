using System;
using Neusoft.HISFC.Models.Base;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Models.Fee.ZYYF
{
    /// <summary>
    /// FeeItemList<br></br>
    /// [功能描述: 门诊费用明细类]<br></br>
    /// [创 建 者: 王宇]<br></br>
    /// [创建时间: 2006-09-13]<br></br>
    /// <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    /// 
    [System.Serializable]
    public class ZYFFeeItemList : NeuObject
    {
        #region 变量

        /// <summary>
        /// 门诊费用明细类
        /// </summary>
        private Outpatient.FeeItemList feeItemList = null;

        /// <summary>
        /// 患者类型 1-门诊 2-住院
        /// </summary>
        private string patientType;

        #endregion

        #region 属性

        /// <summary>
        /// 门诊费用明细类
        /// </summary>
        public Outpatient.FeeItemList FeeItemList
        {
            get
            {
                if (feeItemList == null)
                {
                    feeItemList = new Neusoft.HISFC.Models.Fee.Outpatient.FeeItemList();
                }
                return this.feeItemList;
            }
            set
            {
                this.feeItemList = value;
            }
        }

        /// <summary>
        /// 患者类型 1-门诊 2-住院
        /// </summary>
        public string PatientType
        {
            get
            {
                return this.patientType;
            }
            set
            {
                this.patientType = value;
            }
        }

        #endregion

        #region 方法

        #region 克隆

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>返回当前对象实例</returns>
        public new ZYFFeeItemList Clone()
        {
            ZYFFeeItemList f = base.Clone() as ZYFFeeItemList;
            f.FeeItemList = FeeItemList.Clone();
            return f;
        }

        #endregion

        #endregion

        #region 无用变量属性

        

        #endregion

    }
}
