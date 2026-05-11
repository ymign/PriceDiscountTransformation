using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    /// <summary>
    /// SourceCode<br></br>
    /// [功能描述: 原标本的条码生成]<br></br>
    /// [创 建 者: 刘伊]<br></br>
    /// [创建时间: 2010-01-11]<br></br>
    /// Table : SPEC_GENERATEBARCODE
    /// <修改记录 
    ///		修改人='林国科' 
    ///		修改时间='2011-09-29' 
    ///		修改目的='版本转换4.5转5.0'
    ///		修改描述='除版本转换外，从DB2数据库迁移到ORACLE'
    ///  />
    /// </summary>
    public class SourceCode : Neusoft.FrameWork.Models.NeuObject
    {
        #region 变量
        private int barCodeId = 0;
        private string inpatientNo = "";
        private string barCode = "";
        #endregion

        #region 属性
        public int BarCodeID
        {
            get
            {
                return barCodeId;
            }
            set
            {
                barCodeId = value;
            }
        }

        public string InPatientNo
        {
            get
            {
                return inpatientNo;
            }
            set
            {
                inpatientNo = value;
            }
        }

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
        #endregion

        #region 方法
        /// <summary>
        /// Clone 方法
        /// </summary>
        /// <returns></returns>
        public new SourceCode Clone()
        {
            SourceCode sourceCode = base.Clone() as SourceCode;
            return sourceCode;
        }
        #endregion
    }
}
