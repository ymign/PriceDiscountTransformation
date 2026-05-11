using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.Base
{
    /// <summary>
    /// WorkTypeEnumService <br></br>
	/// [功能描述: 职工类型枚举服务实体]<br></br>
	/// [创 建 者: 孙久海]<br></br>
	/// [创建时间: 2008-06-24]<br></br>
	/// <修改记录
	///		修改人=''
	///		修改时间=''
	///		修改目的=''
	///		修改描述=''
	///  />
    /// </summary>
    [System.Serializable]
    public class WorkTypeEnumService : EnumServiceBase
    {
        public WorkTypeEnumService()
		{
            this.Items[EnumWorkType.Bargainer] = "合同工";
            this.Items[EnumWorkType.Jobber] = "临时工";
		}

		#region 变量

		/// <summary>
		/// 职工类型
		/// </summary>
        EnumWorkType enumWorkType;

		/// <summary>
		/// 存储枚举定义
		/// </summary>
		protected static Hashtable items = new Hashtable();

		#endregion

		#region 属性


		/// <summary>
		/// 存贮枚举名称
		/// </summary>
		protected override Hashtable Items
		{
			get
			{
				return items;
			}
		}
		
		/// <summary>
		/// 枚举项目
		/// </summary>
		protected override Enum EnumItem
		{
			get
			{
                return this.enumWorkType;
			}
		}

		#endregion

		/// <summary>
		/// 克隆
		/// </summary>
		/// <returns></returns>
        public new WorkTypeEnumService Clone()
		{
            WorkTypeEnumService wordTypeEnumService = base.Clone() as WorkTypeEnumService;

            return wordTypeEnumService;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new static ArrayList List()
        {
            return (new ArrayList( GetObjectItems(items)));
        }
    }
}


		
