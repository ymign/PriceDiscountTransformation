using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// <br>Neusoft.HISFC.Models.HR.ContractChange</br>
    /// <br>[功能描述: 合同变更实体类]</br>
    /// <br>[创 建 者: 赵阳]</br>
    /// <br>[创建时间: 2008-07-14]</br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class ContractChange : Neusoft.FrameWork.Models.NeuObject
    {
        #region 字段

        /// <summary>
        /// 放生序号
        /// </summary>
        private string occurNumber = "";

        /// <summary>
        /// 员工实体
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 合同编号
        /// </summary>
        private string contractCode = "";

        /// <summary>
        /// 执行时间
        /// </summary>
        private DateTime executeTime;

        /// <summary>
        /// 签订方式
        /// </summary>
        private string signedType = "";

        /// <summary>
        /// 合同状态
        /// </summary>
        private string contractStatus = "";

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        #endregion

        #region 属性

        /// <summary>
        /// 放生序号
        /// </summary>
        public string OccurNumber
        {
            get
            {
                return occurNumber;
            }
            set
            {
                occurNumber = value;
            }
        }

        /// <summary>
        /// 员工实体
        /// </summary>
        public Neusoft.HISFC.Models.Base.Employee Employee
        {
            get
            {
                return employee;
            }
            set
            {
                employee = value;
            }
        }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContractCode
        {
            get
            {
                return contractCode;
            }
            set
            {
                contractCode = value;
            }
        }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime ExecuteTime
        {
            get
            {
                return executeTime;
            }
            set
            {
                executeTime = value;
            }
        }

        /// <summary>
        /// 签订方式
        /// </summary>
        public string SignedType
        {
            get
            {
                return signedType;
            }
            set
            {
                signedType = value;
            }
        }

        /// <summary>
        /// 合同状态
        /// </summary>
        public string ContractStatus
        {
            get
            {
                return contractStatus;
            }
            set
            {
                contractStatus = value;
            }
        }

        /// <summary>
        /// 操作员
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

        #endregion

        #region 方法

        /// <summary>
        /// 克隆方法
        /// </summary>
        /// <returns></returns>
        public new Contract Clone()
        {
            Contract contract = base.Clone() as Contract;

            contract.Employee = this.Employee.Clone();
            contract.Oper = this.Oper.Clone();

            return contract;
        }

        #endregion
    }
}
