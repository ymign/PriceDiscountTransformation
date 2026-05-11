using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// SubSpec<br></br>
    /// [功能描述: 分装标本实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_SUBSPEC
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SubSpec : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int subSpecID = 0;
        private string subBarCode = "";
        private int specID = 0;
        private DateTime storeTime = DateTime.MinValue;
        private int specCount = 0;
        private decimal specCap = 0;
        private char isCancer = '1';
        private int specTypeID = 0;
        private DateTime lastReturnTime = DateTime.MinValue;
        private DateTime dateReturnTime = DateTime.MinValue;
        private string status = "";
        private char isReturnable = '1';
        private int boxId = 0;
        private int boxStartRow = 0;
        private int boxStratCol = 0;
        private int boxEndRow = 0;
        private int boxEndCol = 0;
        private string inStore = "O";
        private string comment = "";
        private int storeId = 0;
        private string boxBarCode = "";
        private string useAlone = "0";
        private string user = "";
        #endregion

        #region 属性
        /// <summary>
        /// 分装标本ID
        /// </summary>
        public int SubSpecId
        {
            get
            {
                return subSpecID;
            }
            set
            {
                subSpecID = value;
            }
        }
        /// <summary>
        /// 分装标本条形码
        /// </summary>
        public string SubBarCode
        {
            get
            {
                return subBarCode;
            }
            set
            {
                subBarCode = value;
            }
        }
        /// <summary>
        /// 原标本ID
        /// </summary>
        public int SpecId
        {
            get
            {
                return specID;
            }
            set
            {
                specID = value;
            }
        }
        /// <summary>
        /// 标本存储入库时间
        /// </summary>
        public DateTime StoreTime
        {
            get
            {
                return storeTime;
            }
            set
            {
                storeTime = value;
            }
        }

        /// <summary>
        /// 标本的数量
        /// </summary>
        public int SpecCount
        {
            get
            {
                return specCount;
            }
            set
            {
                specCount = value;
            }
        }

        /// <summary>
        /// 每一只标本的容量
        /// </summary>
        public decimal SpecCap
        {
            get
            {
                return specCap;
            }
            set
            {
                specCap = value;
            }
        }
        /// <summary>
        /// 是否癌变
        /// </summary>
        public char IsCancer
        {
            get
            {
                return isCancer;
            }
            set
            {
                isCancer = value;
            }
        }
        /// <summary>
        /// 标本类型的ID，玻片 Or 血清 or?
        /// </summary>
        public int SpecTypeId
        {
            get
            {
                return specTypeID;
            }
            set
            {
                specTypeID = value;
            }
        }

        /// <summary>
        /// 约定返回日期
        /// </summary>
        public DateTime DateReturnTime
        {
            get
            {
                return dateReturnTime;
            }
            set
            {
                dateReturnTime = value;
            }
        }

        /// <summary>
        /// 标本上次返还时间
        /// </summary>
        public DateTime LastReturnTime
        {
            get
            {
                return lastReturnTime;
            }
            set
            {
                lastReturnTime = value;
            }
        }
        /// <summary>
        /// 标本的在库情况，1 在库，2借出未还，0没有
        /// </summary>
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        /// <summary>
        /// 标本是否可还
        /// </summary>
        public char IsReturnable
        {
            get
            {
                return isReturnable;
            }
            set
            {
                isReturnable = value;
            }
        }
        /// <summary>
        /// 存放的标本盒ID
        /// </summary>
        public int BoxId
        {
            get
            {
                return boxId;
            }
            set
            {
                boxId = value;
            }
        }
        /// <summary>
        /// 所在标本盒开始行
        /// </summary>
        public int BoxStartRow
        {
            get
            {
                return boxStartRow;
            }
            set
            {
                boxStartRow = value;
            }
        }
        /// <summary>
        /// 所在标本盒列
        /// </summary>
        public int BoxStartCol
        {
            get
            {
                return boxStratCol;
            }
            set
            {
                boxStratCol = value;
            }
        }

        /// <summary>
        /// 所在标本列
        /// </summary>
        public int BoxEndCol
        {
            get
            {
                return boxEndCol;
            }
            set
            {
                boxEndCol = value;
            }
        }

        /// <summary>
        /// 所在标本行
        /// </summary>
        public int BoxEndRow
        {
            get
            {
                return boxEndRow;
            }
            set
            {
                boxEndRow = value;
            }
        }

        public string BoxBarCode
        {
            get
            {
                return boxBarCode;
            }
            set
            {
                boxBarCode = value;
            }
        }

        /// <summary>
        /// S:已放入存储库，B:在盒子中, O 在外面
        /// </summary>
        public string InStore
        {
            get
            {
                return inStore;
            }
            set
            {
                inStore = value;
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

        /// <summary>
        /// 对应的Spec_Source_Store StoreID
        /// </summary>
        public int StoreID
        {
            get
            {
                return storeId;
            }
            set
            {
                storeId = value;
            }
        }

        /// <summary>
        /// 是否专用
        /// </summary>
        public string UseAlone
        {
            get
            {
                return useAlone;
            }
            set
            {
                useAlone = value;
            }
        }

        /// <summary>
        /// 专用使用者
        /// </summary>
        public string User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
            }
        }
        #endregion

        #region 方法
        public SubSpec(int subspecid, string subbarcode)
        {
            subBarCode = subbarcode;
            subSpecID = subspecid;
        }
        public SubSpec()
        {

        }
        public new SubSpec Clone()
        {
            return base.Clone() as SubSpec;
        }
        #endregion
    }
}
