using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Integrate.Common
{
    /// <summary>
    /// 类名称<br>ICDMedicare</br>
    /// <Font color='#FF1111'>[功能描述: 用户默认设置信息类]</Font><br></br>
    /// [创 建 者: ]<br></br>
    /// [创建时间: ]<br></br>
    /// <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///		/>
    /// </summary>
    public class UserDefaultSetting : IntegrateBase
    {
        private Neusoft.HISFC.BizLogic.Manager.UserDefaultSetting userMgr = new Neusoft.HISFC.BizLogic.Manager.UserDefaultSetting();

        /// <summary>
        /// 查询用户设置信息
        /// </summary>
        public Neusoft.HISFC.Models.Base.UserDefaultSetting Query(string emplCode)
        {
            this.SetDB(userMgr);
            return this.userMgr.Query(emplCode);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public int Insert(Neusoft.HISFC.Models.Base.UserDefaultSetting setting)
        {
            this.SetDB(userMgr);
            return this.userMgr.Insert(setting);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public int Update(Neusoft.HISFC.Models.Base.UserDefaultSetting setting)
        {
            this.SetDB(userMgr);
            return this.userMgr.Update(setting);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public int Delete(Neusoft.HISFC.Models.Base.UserDefaultSetting setting)
        {
            this.SetDB(userMgr);
            return this.userMgr.Delete(setting);
        }
    }
}
