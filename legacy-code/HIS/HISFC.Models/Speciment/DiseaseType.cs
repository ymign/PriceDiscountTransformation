using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// Speciment<br></br>
    /// [功能描述: 病种类型实体类]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2009-09-24]<br></br>
    /// Table : SPEC_DISEASETYPE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class DiseaseType : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int disTypeID = 0;
        private string disName = "";
        private string disColor = "";
        private string comment = "";
        private string orgOrBld = "";
        private string ext1 = "";
        private string ext2 = "";
        private string ext3 = "";
        #endregion

        #region 属性
        public DiseaseType(int distypeid)
        {
            disTypeID = distypeid;

        }
        public DiseaseType()
        {

        }
        /// <summary>
        /// 病种类型ID
        /// </summary>
        public int DisTypeID
        {
            get
            {
                return disTypeID;
            }
            set
            {
                disTypeID = value;
            }
        }
        /// <summary>
        /// 病种类型名称
        /// </summary>
        public string DiseaseName
        {
            get
            {
                return disName;
            }
            set
            {
                disName = value;
            }
        }
        /// <summary>
        /// 病种颜色
        /// </summary>
        public string DiseaseColor
        {
            get
            {
                return disColor;
            }
            set
            {
                disColor = value;
            }
        }

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

        public string OrgOrBld
        {
            get
            {
                return orgOrBld;
            }
            set
            {
                orgOrBld = value;
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
        #endregion

        #region 方法
        public new DiseaseType Clone()
        {
            DiseaseType dis = base.Clone() as DiseaseType;
            return dis;
        }
        #endregion
    }
}
