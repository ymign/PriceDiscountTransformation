using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.Speciment
{
    public class MedDoc
    {
        #region 变量
        private Base.Employee mainDoc = new Neusoft.HISFC.Models.Base.Employee();
        private Base.Employee mainDoc1 = new Neusoft.HISFC.Models.Base.Employee();
        private Base.Employee mainDoc2 = new Neusoft.HISFC.Models.Base.Employee();
        private Base.Employee mainDoc3 = new Neusoft.HISFC.Models.Base.Employee();
        private Base.Department dept = new Neusoft.HISFC.Models.Base.Department();
        #endregion

        #region 属性
        /// <summary>
        /// 治疗科室
        /// </summary>
        public Base.Department Dept
        {
            get
            {
                return dept;
            }
            set
            {
                dept = value;
            }
        }

        /// <summary>
        /// 主治医生
        /// </summary>
        public Base.Employee MainDoc
        {
            get
            {
                return mainDoc;
            }
            set
            {
                mainDoc = value;
            }
        }

        /// <summary>
        /// 第一助理
        /// </summary>
        public Base.Employee MainDoc1
        {
            get
            {
                return mainDoc1;
            }
            set
            {
                mainDoc1 = value;
            }
        }

        /// <summary>
        /// 第二助理
        /// </summary>
        public Base.Employee MainDoc2
        {
            get
            {
                return mainDoc2;
            }
            set
            {
                mainDoc2 = value;
            }
        }

        /// <summary>
        /// 第三助理
        /// </summary>
        public Base.Employee MainDoc3
        {
            get
            {
                return mainDoc3;
            }
            set
            {
                mainDoc3 = value;
            }
        }
        #endregion

        #region 方法
        public new MedDoc Clone()
        {
            MedDoc doc = new MedDoc();
            doc.MainDoc = this.MainDoc.Clone();
            doc.MainDoc1 = this.MainDoc1.Clone();
            doc.MainDoc2 = this.MainDoc2.Clone();
            doc.MainDoc3 = this.MainDoc3.Clone();
            return doc;
        }
        #endregion
    }
}
