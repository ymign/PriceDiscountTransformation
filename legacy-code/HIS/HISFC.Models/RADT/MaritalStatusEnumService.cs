using System;
using System.Collections;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.RADT
{
	/// <summary>
	/// [功能描述: 婚姻状态实体]<br></br>
	/// [创 建 者: 李云凡]<br></br>
	/// [创建时间: 2006-09-05]<br></br>
	/// <修改记录
	///		修改人='张立伟'
	///		修改时间='2006-9-12'
	///		修改目的=''
	///		修改描述=''
	///  />
	/// </summary> 
    [Serializable]
    public class MaritalStatusEnumService :EnumServiceBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		static MaritalStatusEnumService()
		{
			items[EnumMaritalStatus.S] = "未婚";
			items[EnumMaritalStatus.M] = "已婚";
			items[EnumMaritalStatus.D] = "离婚";
			//items[EnumMaritalStatus.R] = "再婚";
			//items[EnumMaritalStatus.A] = "分居";
			items[EnumMaritalStatus.W] = "丧偶";
            items[EnumMaritalStatus.O] = "其他";
		}
		
		
		#region 变量

		private EnumMaritalStatus enumMaritalStatus;
		
		/// <summary>
		/// 存贮枚举名称
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
		
		protected override Enum EnumItem
		{
			get
			{
				return this.enumMaritalStatus;
			}
		}

		#endregion  	
		
		#region 方法
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new static ArrayList List()
        {
            ArrayList al = new ArrayList(GetObjectItems(items));
            al.Sort(new MaritalCompare());
            return al;
            //return (new ArrayList(GetObjectItems(items)));
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new MaritalStatusEnumService Clone()
        {
            return base.Clone() as MaritalStatusEnumService;
        }

		#endregion

    }

    #region 排序

    /// <summary>
    /// 对婚姻状况进行排序-因为哈希表不是顺序表，输出不一定有序
    /// </summary>
    public class MaritalCompare  :IComparer
    {

        #region IComparer 成员

        public int Compare(object x, object y)
        {
            try
            {
                Neusoft.FrameWork.Models.NeuObject obj1 = x as Neusoft.FrameWork.Models.NeuObject;
                Neusoft.FrameWork.Models.NeuObject obj2 = y as Neusoft.FrameWork.Models.NeuObject;
                int order1 = this.GetOrderNum(obj1);
                int order2 = this.GetOrderNum(obj2);
                if (order1 > order2)
                {
                    return 1;
                }
                else if (order1 == order2)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }


            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        /// <summary>
        /// 获取婚姻状况的顺序
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int GetOrderNum(Neusoft.FrameWork.Models.NeuObject obj)
        {
            int i = 0;
            if (obj.ID == "S")
            {
                i = 0;
            }
            else if (obj.ID == "M")
            {
                i = 1;
            }
            else if (obj.ID == "D")
            {
                i = 3;
            }
            else if (obj.ID == "R")
            {
                i = 4;
            }
            else if (obj.ID == "A")
            {
                i = 5;
            }
            else if (obj.ID == "W")
            {
                i = 6;
            }
            else
            {
                i = 99;
            }
            return i;
        }
    }

    #endregion

}
