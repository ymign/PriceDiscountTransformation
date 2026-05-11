using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Container<br></br>
    /// [功能描述: 容器类]<br>所有的容器继承此类，Icebox,shelf,iceboxlayer</br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class Container : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量域

        private int row = 0;
        private int col = 0;
        private int height = 0;
        private int capacity = 0;
        private int occupyCount = 0;

        #endregion

        #region 属性
        /// <summary>
        /// 容器的行
        /// </summary>
        public int Row
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
            }
        }
        /// <summary>
        /// 容器的列
        /// </summary>
        public int Col
        {
            get
            {
                return col;
            }
            set
            {
                col = value;
            }
        }
        /// <summary>
        /// 容器的高度
        /// </summary>
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        /// <summary>
        /// 容器的容量
        /// </summary>
        public int Capacity
        {
            get
            {
                return this.capacity;
            }
            set
            {
                this.capacity = value;
            }
        }

        /// <summary>
        /// 容器占用的数量
        /// </summary>
        public int OccupyCount
        {
            get
            {
                return this.occupyCount;
            }
            set
            {
                this.occupyCount = value;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Container Clone()
        {
            Container container = base.Clone() as Container;
            return container;
        }
        #endregion
    }
}
