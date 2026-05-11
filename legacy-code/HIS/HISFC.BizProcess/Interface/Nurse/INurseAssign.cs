using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Nurse
{
    /// <summary>
    /// 接收叫号信息
    /// </summary>
    public interface INurseAssign
    {
        /// <summary>
        ///  门诊分诊接收叫号信息
        /// </summary>
        /// <param name="register">挂号实体</param>
        /// <param name="dept">看诊科室信息</param>
        /// <param name="room">看诊诊室</param>
        /// <param name="console">看诊诊台</param>
        /// <returns></returns>
        int Insert(Neusoft.HISFC.Models.Registration.Register register, Neusoft.FrameWork.Models.NeuObject dept, Neusoft.FrameWork.Models.NeuObject nurse, Neusoft.FrameWork.Models.NeuObject room, Neusoft.FrameWork.Models.NeuObject console, Neusoft.HISFC.Models.Base.Noon noon, ref string err);

        /// <summary>
        /// 门诊分诊叫号
        /// </summary>
        /// <param name="nurseCode">护士站</param>
        /// <param name="noon">午别</param>
        void Call(string nurseCode, string noonID);

    }
}
