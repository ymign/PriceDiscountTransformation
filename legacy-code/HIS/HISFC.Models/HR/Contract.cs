using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.Models.HR
{
    /// <summary>
    /// Neusoft.HISFC.Models.HR.Constract<br></br>
    /// [功能描述: 合同实体类。]<br></br>
    /// [创 建 者: 赵阳]<br></br>
    /// [创建时间: 2008-07-03]<br></br>
    /// <修改记录
    ///		修改人=''
    ///		修改时间='yyyy-mm-dd'
    ///		修改目的=''
    ///		修改描述=''
    ///  />
    /// </summary>
    [System.Serializable]
    public class Contract : Neusoft.FrameWork.Models.NeuObject 
    {
        #region 字段

        /// <summary>
        /// 放生序号
        /// </summary>
        //private string occurNumber = "";

        /// <summary>
        /// 员工实体
        /// </summary>
        private Neusoft.HISFC.Models.Base.Employee employee = new Neusoft.HISFC.Models.Base.Employee();

        /// <summary>
        /// 合同编号
        /// </summary>
        private string contractCode = "";

        /// <summary>
        /// 起始时间
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// 终止时间
        /// </summary>
        private DateTime endTime;

        /// <summary>
        /// 签订时间
        /// </summary>
        private DateTime signedTime;

        /// <summary>
        /// 解除时间
        /// </summary>
        private DateTime releaseTime;

        /// <summary>
        /// 解除合同原因
        /// </summary>
        private string releaseReason;

        /// <summary>
        /// 签订方式
        /// </summary>
        private string signedType;

        /// <summary>
        /// 人员类别
        /// </summary>
        private string personType;
        
        /// <summary>
        /// 人员类别名称
        /// </summary>
        private string  personTypeName;

        /// <summary>
        /// 违约金
        /// </summary>
        private decimal renegeMoney;

        /// <summary>
        /// 聘期
        /// </summary>
        private decimal retainTerm;

        /// <summary>
        /// 聘用方式
        /// </summary>
        private string retainType;

        /// <summary>
        /// 操作员
        /// </summary>
        private Neusoft.HISFC.Models.Base.OperEnvironment oper = new Neusoft.HISFC.Models.Base.OperEnvironment();

        /// <summary>
        /// 合同附件
        /// </summary>
        private byte[] contractAttach = null;

        #endregion

        #region 字段

        /// <summary>
        /// 放生序号
        /// </summary>
        //public string OccurNumber
        //{
        //    get
        //    {
        //        return occurNumber;
        //    }
        //    set
        //    {
        //        occurNumber = value;
        //    }
        //}

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
        /// 起始时间
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }

        /// <summary>
        /// 终止时间
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                endTime = value;
            }
        }

        /// <summary>
        /// 签订时间
        /// </summary>
        public DateTime SignedTime
        {
            get
            {
                return signedTime;
            }
            set
            {
                signedTime = value;
            }
        }

        /// <summary>
        /// 解除时间
        /// </summary>
        public DateTime ReleaseTime
        {
            get
            {
                return releaseTime;
            }
            set
            {
                releaseTime = value;
            }
        }

        /// <summary>
        /// 解除合同原因
        /// </summary>
        public string ReleaseReason
        {
            get
            {
                return releaseReason;
            }
            set
            {
                releaseReason = value;
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
        /// 人员类别
        /// </summary>
        public string PersonType
        {
            get
            {
                return personType;
            }
            set
            {
                personType = value;
            }
        }

        /// <summary>
        /// 人员类别名称
        /// </summary>
        public string PersonTypeName
        {
            get
            {
                return personTypeName;
            }
            set
            {
                personTypeName = value;
            }
        }

        /// <summary>
        /// 违约金
        /// </summary>
        public decimal RenegeMoney
        {
            get
            {
                return renegeMoney;
            }
            set
            {
                renegeMoney = value;
            }
        }

        /// <summary>
        /// 聘期
        /// </summary>
        public decimal RetainTerm
        {
            get
            {
                return retainTerm;
            }
            set
            {
                retainTerm = value;
            }
        }

        /// <summary>
        /// 聘用方式
        /// </summary>
        public string RetainType
        {
            get
            {
                return retainType;
            }
            set
            {
                retainType = value;
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

        /// <summary>
        /// 合同附件
        /// </summary>
        public byte[] ContractAttach
        {
            get
            {
                return contractAttach;
            }
            set
            {
                contractAttach = value;
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
            contract.ContractAttach = this.ContractAttach.Clone() as byte[];

            return contract;
        }
        #endregion
    }
}
