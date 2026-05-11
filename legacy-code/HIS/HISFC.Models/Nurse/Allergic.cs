using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.FrameWork.Models;

namespace Neusoft.HISFC.Models.Nurse
{
    public class Allergic : NeuObject
    {
        public Allergic()
        {
        }

        #region 变量

        /// <summary>
        /// 患者编号(病历号或住院号)
        /// </summary>
        private string patientNO;

        /// <summary>
        /// 患者类型（门诊、住院)
        /// </summary>
        private Neusoft.HISFC.Models.Base.ServiceTypes patientKind;

        /// <summary>
        /// 患者流水号
        /// </summary>
        private string inPatientNO;

        /// <summary>
        /// 发生序号
        /// </summary>
        private int happenNo;

        /// <summary>
        /// 药理作用编码
        /// </summary>
        private string phyfunctionCode;

        /// <summary>
        /// 药理作用名称
        /// </summary>
        private string phyfunctionName;

        /// <summary>
        /// 过敏药品编码
        /// </summary>
        private string drugCode;

        /// <summary>
        /// 过敏药品名称
        /// </summary>
        private string drugName;

        /// <summary>
        /// 过敏类别（皮试阳性、休克、药疹等）
        /// </summary>
        private string allergicType;

        /// <summary>
        /// 皮试药品编码
        /// </summary>
        private string astDrugCode;

        /// <summary>
        /// 皮试药品名称
        /// </summary>
        private string astDrugName;

        /// <summary>
        /// 操作环境
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper;

        /// <summary>
        /// 有效性
        /// </summary>
        private Neusoft.HISFC.Models.Base.EnumValidState validState;

        /// <summary>
        /// 作废环境
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment cancelOper;

        #endregion

        #region 属性

        /// <summary>
        /// 患者编号(病历号或住院号)
        /// </summary>
        public string PatientNO
        {
            get
            {
                return patientNO;
            }
            set
            {
                patientNO = value;
            }
        }

        /// <summary>
        /// 患者类型（门诊、住院)
        /// </summary>
        public Neusoft.HISFC.Models.Base.ServiceTypes PatientKind
        {
            get
            {
                return patientKind;
            }
            set
            {
                patientKind = value;
            }
        }

        /// <summary>
        /// 患者流水号
        /// </summary>
        public string InPatientNO
        {
            get
            {
                return inPatientNO;
            }
            set
            {
                inPatientNO = value;
            }
        }

        /// <summary>
        /// 发生序号
        /// </summary>
        public int HappenNo
        {
            get
            {
                return happenNo;
            }
            set
            {
                happenNo = value;
            }
        }

        /// <summary>
        /// 药理作用编码
        /// </summary>
        public string PhyfunctionCode
        {
            get
            {
                return phyfunctionCode;
            }
            set
            {
                phyfunctionCode = value;
            }
        }

        /// <summary>
        /// 药理作用名称
        /// </summary>
        public string PhyfunctionName
        {
            get
            {
                return phyfunctionName;
            }
            set
            {
                phyfunctionName = value;
            }
        }

        /// <summary>
        /// 过敏药品编码
        /// </summary>
        public string DrugCode
        {
            get
            {
                return drugCode;
            }
            set
            {
                drugCode = value;
            }
        }

        /// <summary>
        /// 过敏药品名称
        /// </summary>
        public string DrugName
        {
            get
            {
                return drugName;
            }
            set
            {
                drugName = value;
            }
        }

        /// <summary>
        /// 过敏类别（皮试阳性、休克、药疹等）
        /// </summary>
        public string AllergicType
        {
            get
            {
                return allergicType;
            }
            set
            {
                allergicType = value;
            }
        }

        /// <summary>
        /// 皮试药品编码
        /// </summary>
        public string AstDrugCode
        {
            get
            {
                return astDrugCode;
            }
            set
            {
                astDrugCode = value;
            }
        }

        /// <summary>
        /// 皮试药品名称
        /// </summary>
        public string AstDrugName
        {
            get
            {
                return astDrugName;
            }
            set
            {
                astDrugName = value;
            }
        }

        /// <summary>
        /// 操作环境
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment Oper
        {
            get
            {
                return oper;
            }
            set
            {
                oper = value;
            }
        }

        /// <summary>
        /// 有效性
        /// </summary>
        public Neusoft.HISFC.Models.Base.EnumValidState ValidState
        {
            get
            {
                return validState;
            }
            set
            {
                validState = value;
            }
        }

        /// <summary>
        /// 作废环境
        /// </summary>
        public Neusoft.HISFC.Models.Base.OperEnvironment CancelOper
        {
            get
            {
                return cancelOper;
            }
            set
            {
                cancelOper = value;
            }
        }
        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Allergic Clone()
        {
            Allergic ergen = base.Clone() as Allergic;

            ergen.oper = this.oper.Clone();
            ergen.cancelOper = this.cancelOper.Clone();

            return ergen;
        }
        #endregion
    }
}
