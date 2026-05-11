using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 计划分装标本实体]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-10-21]<br></br>
    /// Table : SPEC_SOURCEPLAN 
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SpecSourcePlan : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int planId;
        private int specId;
        private int specTypeID;
        private int count;
        private decimal capacity;
        private string unit = "";
        private int forSlefUse;
        private string tumorType = "";
        private char isHis = '1';
        private string limitUse = "";
        private string tumorPos = "";
        private string sideFrom = "";
        private string tumorPor = "";
        private string baomoEntire = "";
        private string breakPiece = "";
        private DateTime storeTime = DateTime.Now;
        private int storeCount;   //     
        private SpecType specType = new SpecType();//
        private string comment = "";//
        private List<string> subSpecBarCodeList = new List<string>();
        private string transPos = "";
        private string ext1 = "";
        private string ext2 = "";
        private string ext3 = "";
        #endregion

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public int PlanID
        {
            get
            {
                return planId;
            }
            set
            {
                planId = value;
            }
        }

        /// <summary>
        /// 原标本ID
        /// </summary>
        public int SpecID
        {
            get
            {
                return specId;
            }
            set
            {
                specId = value;
            }
        }

        /// <summary>
        /// 标本类型ID
        /// </summary>
        public int SpecTypeID
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
        /// 数量
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        /// <summary>
        /// 容量
        /// </summary>
        public decimal Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                capacity = value;
            }
        }

        /// <summary>
        /// 给送存医生留的标本数量
        /// </summary>
        public int ForSelfUse
        {
            get
            {
                return forSlefUse;
            }
            set
            {
                forSlefUse = value;
            }
        }

        public SpecType SpecType
        {
            get
            {
                return specType;
            }
            set
            {
                specType = value;
            }
        }

        /// <summary>
        /// 限制使用
        /// </summary>
        public string LimitUse
        {
            get
            {
                return limitUse;
            }
            set
            {
                limitUse = value;
            }
        }

        /// <summary>
        /// 肿物部位
        /// </summary>
        public string TumorPos
        {
            get
            {
                return tumorPos;
            }
            set
            {
                tumorPos = value;
            }
        }

        /// <summary>
        /// 侧别： L. 左侧   R. 右侧  A. 全部
        /// </summary>
        public string SideFrom
        {
            get
            {
                return sideFrom;
            }
            set
            {
                sideFrom = value;
            }
        }

        /// <summary>
        /// 标本属性1.原发癌 2.复发癌 3.转移癌
        /// </summary>
        public string TumorPor
        {
            get
            {
                return tumorPor;
            }
            set
            {
                tumorPor = value;
            }
        }

        /// <summary>
        /// 包膜完整，0：不完整，1：完整
        /// </summary>
        public string BaoMoEntire
        {
            get
            {
                return baomoEntire;
            }
            set
            {
                baomoEntire = value;
            }
        }

        /// <summary>
        /// 碎组织，0：是，1 否
        /// </summary>
        public string BreakPiece
        {
            get
            {
                return breakPiece;
            }
            set
            {
                breakPiece = value;
            }
        }

        /// <summary>
        /// 入库时间
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
        /// 来源于HIS
        /// </summary>
        public char IsHis
        {
            get
            {
                return isHis;
            }
            set
            {
                isHis = value;
            }
        }

        /// <summary>
        /// 肿瘤类型
        /// </summary>
        public string TumorType
        {
            get
            {
                return tumorType;
            }
            set
            {
                tumorType = value;
            }
        }

        /// <summary>
        /// 库存
        /// </summary>
        public int StoreCount
        {
            get
            {
                return storeCount;
            }
            set
            {
                storeCount = value;
            }
        }

        /// <summary>
        /// 容量规格
        /// </summary>
        public string Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
            }
        }

        /// <summary>
        /// 转移部位
        /// </summary>
        public string TransPos
        {
            get
            {
                return transPos;
            }
            set
            {
                transPos = value;
            }
        }

        public string Ext1
        {
            get
            {
                return ext1;
            }
            set
            {
                ext1 = value;
            }
        }

        public string Ext2
        {
            get
            {
                return ext2;
            }
            set
            {
                ext2 = value;
            }
        }

        public string Ext3
        {
            get
            {
                return ext3;
            }
            set
            {
                ext3 = value;
            }
        }

        public List<string> SubSpecCodeList
        {
            get
            {
                return subSpecBarCodeList;
            }
            set
            {
                subSpecBarCodeList = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new SpecSourcePlan Clone()
        {
            SpecSourcePlan plan = base.Clone() as SpecSourcePlan;
            plan.SpecType = this.SpecType.Clone();
            return plan;
        }

        /// <summary>
        /// 确定是否是一样的属性
        /// </summary>
        /// <returns></returns>
        public bool ChkFromSamePro(SpecSourcePlan s)
        {
            if (this.SpecID != s.SpecID)
                return false;
            //if (this.TumorPos != s.TumorPos)
            //    return false;
            if (this.TumorType != s.TumorType)
                return false;
            //if (this.SideFrom != s.SideFrom)
            //    return false;
            //if (this.BaoMoEntire != s.BaoMoEntire)
            //    return false;
            //if (this.BreakPiece != s.BreakPiece)
            //    return false;
            if (this.comment != s.comment)
                return false;
            return true;
        }
        #endregion
    }
}
