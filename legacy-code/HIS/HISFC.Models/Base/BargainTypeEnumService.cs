using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.Base
{
    /// <summary>
    /// SexEnumService <br></br>
    /// [功能描述: 合同类型枚举服务实体]<br></br>
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
    public class BargainTypeEnumService : EnumServiceBase
    {
        public BargainTypeEnumService()
		{
            this.Items[EnumBargainType.InPlan] = "在编";
            this.Items[EnumBargainType.Town] = "城镇合同";
            this.Items[EnumBargainType.Boor] = "农民合同";
		}

		#region 变量

		/// <summary>
		/// 职工类型
		/// </summary>
        EnumBargainType enumBargainType;

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
                return this.enumBargainType;
			}
		}

		#endregion

		/// <summary>
		/// 克隆
		/// </summary>
		/// <returns></returns>
        public new BargainTypeEnumService Clone()
		{
            BargainTypeEnumService bargainTypeEnumService = base.Clone() as BargainTypeEnumService;

            return bargainTypeEnumService;
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
