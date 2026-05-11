using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>Neusoft.HISFC.Models.HR.WorkCheckRefer</br>
    /// <br>[功能描述: 考勤员对照实体，指定考勤员可以对哪些科室进行考勤维护]</br>
    /// <br>[创 建 者: 赵阳]</br>
    /// <br>[创建时间: 2008-08-04]</br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class WorkCheckRefer : Neusoft.FrameWork.Models.NeuObject
    {

        #region 字段

        /// <summary>
        /// 考勤员
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject workChecker = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 考勤科室
        /// </summary>
        private Neusoft.FrameWork.Models.NeuObject dept = new Neusoft.FrameWork.Models.NeuObject();

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion

        #region 属性

        /// <summary>
        /// 考勤员
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject WorkChecker
        {
            get
            {
                return workChecker;
            }
            set
            {
                workChecker = value;
            }
        }

        /// <summary>
        /// 考勤科室
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Dept
        {
            get
            {
                return dept;
            }
            set
            {
                dept = value;
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

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new WorkCheckRefer Clone()
        {
            WorkCheckRefer wcRefer = base.Clone() as WorkCheckRefer;

            wcRefer.WorkChecker = this.WorkChecker.Clone();
            wcRefer.Dept = this.Dept.Clone();
            wcRefer.Oper = this.Oper.Clone();

            return wcRefer;
        }

        #endregion
    }
}
