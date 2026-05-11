using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.Base;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    ///<br>ExpandColumn</br>
    ///<br> [功能描述: 列扩展实体]</br>
    ///<br> [创 建 者: 宋德宏]</br>
    ///<br>[创建时间: 2008-09-26]</br>
    ///    <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class ExpandColumn : Neusoft.FrameWork.Models.NeuObject, Base.ISort, Base.IValid
    {

        #region 字段
        /// <summary>
        /// 是否充许为空
        /// </summary>
        private bool isNull = true;

        /// <summary>
        /// 表名
        /// </summary>
        private string tableName = string.Empty;

        /// <summary>
        /// 字段名称
        /// </summary>
        private string columnName = string.Empty;

        /// <summary>
        /// 字段类型
        /// </summary>
        private string columnType = string.Empty;

        /// <summary>
        /// 列长度
        /// </summary>
        private int columnLength = 0;


        /// <summary>
        /// 小数长度
        /// </summary>
        private int columnDecimalLen = 0;


        /// <summary>
        /// 默认值
        /// </summary>
        private string defaultValue = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        private string remark = string.Empty;


        /// <summary>
        /// 有效性
        /// </summary>
        private bool isValid = true;

        /// <summary>
        /// 序号
        /// </summary>
        private int sortID = 0;

        /// <summary>
        /// 操作信息
        /// </summary>				
        private OperEnvironment oper = new OperEnvironment();
        #endregion

        #region 属性

        /// <summary>
        /// 是否允许空值
        /// </summary>
        public bool IsNull
        {
            get
            {
                return isNull;
            }
            set
            {
                isNull = value;
            }
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get
            {
                return tableName;
            }
            set
            {
                tableName = value;
            }
        }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName
        {
            get
            {
                return columnName;
            }
            set
            {
                columnName = value;
            }
        }

        /// <summary>
        /// 列类型
        /// </summary>
        public string ColumnType
        {
            get
            {
                return columnType;
            }
            set
            {
                columnType = value;
            }
        }

        /// <summary>
        /// 列长度
        /// </summary>
        public int ColumnLength
        {
            get
            {
                return columnLength;
            }
            set
            {
                columnLength = value;
            }
        }

        /// <summary>
        /// 列小数长度
        /// </summary>
        public int ColumnDecimalLen
        {
            get
            {
                return columnDecimalLen;
            }
            set
            {
                columnDecimalLen = value;
            }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue
        {
            get
            {
                return defaultValue;
            }
            set
            {
                defaultValue = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get
            {
                return remark;
            }
            set
            {
                remark = value;
            }
        }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
            }
        }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int SortID
        {
            get
            {
                return sortID;
            }
            set
            {
                sortID = value;
            }
        }

        /// <summary>
        /// 操作环境
        /// </summary>
        public OperEnvironment Oper
        {
            get
            {
                return this.oper;
            }
            set
            {
                this.oper = value;
            }
        }
        #endregion

        #region 克隆
        /// <summary>
        /// 克隆函数
        /// </summary>
        /// <returns>Const类实例</returns>
        public new ExpandColumn Clone()
        {
            ExpandColumn expandColumn = base.Clone() as ExpandColumn;
            expandColumn.Oper = this.Oper.Clone();

            return expandColumn;

        }
        #endregion
    }
}
