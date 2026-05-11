using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// CapLog<br></br>
    /// [功能描述: 标本容器的操作记录]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-12-14]<br></br>
    /// Table : SPEC_CAPLOG
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class CapLog : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int operId = 0;
        private int capId = 0;
        private string barCode = "";
        private int oldDesCapId = 0;
        private int oldDesCapRow = 0;
        private int oldDesCapCol = 0;
        private int oldDesCapHeight = 0;
        private char oldInType = 'L';
        private int newDesCapId = 0;
        private int newDesCapRow = 0;
        private int newDesCapCol = 0;
        private int newDesCapHeight = 0;
        private char newInType = 'L';
        private string operName = "";
        private DateTime operTime = DateTime.Now;
        private string operDes = "M";
        private string comment = "";
        private int oldDisType = 0;
        private int newDisType = 0;
        private int oldSpecType = 0;
        private int newSpecType = 0;
        #endregion

        #region 属性
        /// <summary>
        /// 操作ID，主键
        /// </summary>
        public int OperId
        {
            get
            {
                return operId;
            }
            set
            {
                operId = value;
            }
        }

        /// <summary>
        /// 容器Id
        /// </summary>
        public int CapId
        {
            get
            {
                return capId;
            }
            set
            {
                capId = value;
            }
        }

        /// <summary>
        /// 容器条形码
        /// </summary>
        public string BarCode
        {
            get
            {
                return barCode;
            }
            set
            {
                barCode = value;
            }
        }

        #region 旧位置信息
        /// <summary>
        /// 所存放容器的Id
        /// </summary>
        public int OldDesCapID
        {
            get
            {
                return oldDesCapId;
            }
            set
            {
                oldDesCapId = value;
            }
        }

        /// <summary>
        /// 旧位置的行
        /// </summary>
        public int OldDesCapRow
        {
            get
            {
                return oldDesCapRow;
            }
            set
            {
                oldDesCapRow = value;
            }
        }

        /// <summary>
        ///旧位置列
        /// </summary>
        public int OldDesCapCol
        {
            get
            {
                return oldDesCapCol;
            }
            set
            {
                oldDesCapCol = value;
            }
        }

        /// <summary>
        /// 旧位置层
        /// </summary>
        public int OldDesCapHeight
        {
            get
            {
                return oldDesCapHeight;
            }
            set
            {
                oldDesCapHeight = value;
            }
        }

        /// <summary>
        /// 旧位置存放容器的类型，S:架子,B,盒子,L,层,F,冰箱
        /// </summary>
        public char OldInType
        {
            get
            {
                return oldInType;
            }
            set
            {
                oldInType = value;
            }
        }
        #endregion

        #region 新位置信息
        public int NewDesCapID
        {
            get
            {
                return newDesCapId;
            }
            set
            {
                newDesCapId = value;
            }
        }

        public int NewDesCapRow
        {
            get
            {
                return newDesCapRow;
            }
            set
            {
                newDesCapRow = value;
            }
        }

        public int NewDesCapCol
        {
            get
            {
                return newDesCapCol;
            }
            set
            {
                newDesCapCol = value;
            }
        }

        public int NewDesCapHeight
        {
            get
            {
                return newDesCapHeight;
            }
            set
            {
                newDesCapHeight = value;
            }
        }

        public char NewInType
        {
            get
            {
                return newInType;
            }
            set
            {
                newInType = value;
            }
        }
        #endregion

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperName
        {
            get
            {
                return operName;
            }
            set
            {
                operName = value;
            }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperTime
        {
            get
            {
                return operTime;
            }
            set
            {
                operTime = value;
            }
        }

        /// <summary>
        /// 操作目的,T,转移，M: 修改，D:废弃
        /// </summary>
        public string OperDes
        {
            get
            {
                return operDes;
            }
            set
            {
                operDes = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
            }
        }

        public int OldDisType
        {
            get
            {
                return oldDisType;
            }
            set
            {
                oldDisType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NewDisType
        {
            get
            {
                return newDisType;
            }
            set
            {
                newDisType = value;
            }
        }

        public int OldSpecType
        {
            get
            {
                return oldSpecType;
            }
            set
            {
                oldSpecType = value;
            }
        }

        public int NewSpecType
        {
            get
            {
                return newSpecType;
            }
            set
            {
                newSpecType = value;
            }
        }
        #endregion

        #region 方法
        public new CapLog Clone()
        {
            return base.Clone() as CapLog;
        }
        #endregion
    }
}
