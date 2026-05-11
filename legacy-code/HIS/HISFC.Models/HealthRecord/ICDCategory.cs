using System;
using Neusoft.FrameWork.Models;


namespace Neusoft.HISFC.Models.HealthRecord
{
    /// <summary>
    /// ICDTemplate诊断模板类<br></br>
    /// [功能描述: ICDTemplate]<br></br>
    /// [创 建 者: 喻赟]<br></br>
    /// [创建时间: 2009-04-17]<br></br>
    /// <修改记录 
    ///		修改人='' 
    ///		修改时间='yyyy-mm-dd' 
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class ICDCategory : NeuObject, Neusoft.HISFC.Models.Base.ISpell
    {
        public ICDCategory()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 变量

        /// <summary>
        /// 上级编码
        /// </summary>
        private string parentID = "";

        /// <summary>
        /// 上级编码
        /// </summary>
        public string ParentID
        {
            get
            {
                return parentID;
            }
            set
            {
                parentID = value;
            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        private string seqNO = "";

        /// <summary>
        /// 序号
        /// </summary>
        public string SeqNO
        {
            get
            {
                return seqNO;
            }
            set
            {
                seqNO = value;
            }
        }

        /// <summary>
        /// 拼音码
        /// </summary>
        private string spellCode = "";

        /// <summary>
        /// 五笔码
        /// </summary>
        private string wbCode = "";

        /// <summary>
        /// 自定义码
        /// </summary>
        private string userCode = "";

        /// <summary>
        /// 分类
        /// </summary>
        private string sortID = "";

        /// <summary>
        /// 分类
        /// </summary>
        public string SortID
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
        /// ICD编码范围
        /// </summary>
        private string range = "";

        /// <summary>
        /// ICD编码范围
        /// </summary>
        public string Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
            }
        }

        /// <summary>
        /// 不知道干啥的
        /// </summary>
        private string sfbr = "";


        /// <summary>
        /// 不知道干啥的
        /// </summary>
        public string Sfbr
        {
            get
            {
                return sfbr;
            }
            set
            {
                sfbr = value;
            }
        }

        #endregion

        #region 克隆

        /// <summary>
        /// 克隆函数
        /// </summary>
        /// <returns></returns>
        public new ICDCategory Clone()
        {
            ICDCategory ICDTemplate = base.Clone() as ICDCategory;
            return ICDTemplate;
        }

        #endregion

        #region ISpell 成员

        public string SpellCode
        {
            get
            {
                return spellCode;
            }
            set
            {
                spellCode = value;
            }
        }

        public string WBCode
        {
            get
            {
                return wbCode;
            }
            set
            {
                wbCode = value;
            }
        }

        public string UserCode
        {
            get
            {
                return userCode;
            }
            set
            {
                userCode = value;
            }
        }

        #endregion
    }
}
