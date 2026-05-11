using System;
using Neusoft.HISFC.Models.Base;
using Neusoft.HISFC.Models.RADT;
using Neusoft.HISFC.Models.Account;
using System.Collections.Generic;

namespace Neusoft.HISFC.Models.Registration
{
    /// <summary>
    /// Register<br></br>
    /// [功能描述: 挂号扩展信息实体]<br></br>
    /// <summary>
    [Serializable]
    public class RegisterExtend : Patient
    {
        /// <summary>
        /// 
        /// </summary>
        public RegisterExtend()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            // 
        }

        #region 变量
        /// <summary>
        /// 预约挂号类型ID
        /// </summary>
        private string bookingTypeId = string.Empty;

        /// <summary>
        /// 预约挂号类型名称
        /// </summary>
        private string bookingTypeName = string.Empty;

        /// <summary>
        /// 医保挂号时间
        /// </summary>
        private DateTime siRegDate = new DateTime();

        /// <summary>
        /// 终端标识号
        /// </summary>
        private string rfsamCode = string.Empty;

        /// <summary>
        /// 随机数
        /// </summary>
        private string randomNum = string.Empty;

        /// <summary>
        /// 验证码MAC
        /// </summary>
        private string captcha = string.Empty;

        /// <summary>
        /// 诊金登记单号
        /// </summary>
        private string diagFeeRegCode = string.Empty;

        /// <summary>
        /// 诊金金额
        /// </summary>
        private decimal diagFee = 0m;

        /// <summary>
        /// 诊金代码
        /// </summary>
        private string diagItemCode = string.Empty;

        /// <summary>
        /// 诊金登记单号【诊金减免结算登记】
        /// </summary>
        private string recipeNo = string.Empty;
        /// <summary>
        /// 医改减免金额【诊金减免结算登记】
        /// </summary>
        private string yigaiAccount = string.Empty;
        /// <summary>
        /// 病种报销金额【诊金减免结算登记】
        /// </summary>
        private string bzAccount = string.Empty;
        /// <summary>
        /// 个人自付金额【诊金减免结算登记】
        /// </summary>
        private string payAccount = string.Empty;
        /// <summary>
        /// 险种【诊金减免结算登记】
        /// </summary>
        private string assurance = string.Empty;
        /// <summary>
        /// 门特结算单号【诊金减免结算登记】
        /// </summary>
        private string mtRecipeNo = string.Empty;

        #endregion

        #region 属性
        /// <summary>
        /// 预约挂号类型ID
        /// </summary>
        public string BookingTypeId
        {
            get
            {
                return bookingTypeId;
            }
            set
            {
                bookingTypeId = value;
            }
        }

        /// <summary>
        /// 预约挂号类型名称
        /// </summary>
        public string BookingTypeName
        {
            get
            {
                return bookingTypeName;
            }
            set
            {
                bookingTypeName = value;
            }
        }

        /// <summary>
        /// 医保挂号时间
        /// </summary>
        public DateTime SIRegDate
        {
            get
            {
                return siRegDate;
            }
            set
            {
                siRegDate = value;
            }
        }

        /// <summary>
        /// 终端标识号
        /// </summary>
        public string RfsamCode
        {
            get
            {
                return rfsamCode;
            }
            set
            {
                rfsamCode = value;
            }
        }

        /// <summary>
        /// 随机数
        /// </summary>
        public string RandomNum
        {
            get
            {
                return randomNum;
            }
            set
            {
                randomNum = value;
            }
        }

        /// <summary>
        /// 验证码MAC
        /// </summary>
        public string Captcha
        {
            get
            {
                return captcha;
            }
            set
            {
                captcha = value;
            }
        }

        /// <summary>
        /// 诊金登记单号
        /// </summary>
        public string DiagFeeRegCode
        {
            get
            {
                return diagFeeRegCode;
            }
            set
            {
                diagFeeRegCode = value;
            }
        }

        /// <summary>
        /// 诊金金额
        /// </summary>
        public decimal DiagFee
        {
            get
            {
                return diagFee;
            }
            set
            {
                diagFee = value;
            }
        }

        /// <summary>
        /// 诊金代码
        /// </summary>
        public string DiagItemCode
        {
            get
            {
                return diagItemCode;
            }
            set
            {
                diagItemCode = value;
            }
        }

        /// <summary>
        /// 诊金登记单号【诊金减免结算登记】
        /// </summary>
        public string RecipeNo
        {
            get
            {
                return recipeNo;
            }
            set
            {
                recipeNo = value;
            }
        }
        /// <summary>
        /// 医改减免金额【诊金减免结算登记】
        /// </summary>
        public string YigaiAccount
        {
            get
            {
                return yigaiAccount;
            }
            set
            {
                yigaiAccount = value;
            }
        }
        /// <summary>
        /// 病种报销金额【诊金减免结算登记】
        /// </summary>
        public string BzAccount
        {
            get
            {
                return bzAccount;
            }
            set
            {
                bzAccount = value;
            }
        }
        /// <summary>
        /// 个人自付金额【诊金减免结算登记】
        /// </summary>
        public string PayAccount
        {
            get
            {
                return payAccount;
            }
            set
            {
                payAccount = value;
            }
        }
        /// <summary>
        /// 险种【诊金减免结算登记】
        /// </summary>
        public string Assurance
        {
            get
            {
                return assurance;
            }
            set
            {
                assurance = value;
            }
        }
        /// <summary>
        /// 门特结算单号【诊金减免结算登记】
        /// </summary>
        public string MtRecipeNo
        {
            get
            {
                return mtRecipeNo;
            }
            set
            {
                mtRecipeNo = value;
            }
        }

        /// <summary>
        /// 报销类型
        /// </summary>
        private string expenseType = "";
        /// <summary>
        /// 报销类型
        /// </summary>
        public string ExpenseType
        {
            get { return expenseType; }
            set { expenseType = value; }
        }

        /// <summary>
        /// 疾病代码
        /// </summary>
        private string diseaseCode = "";
        /// <summary>
        /// 疾病代码
        /// </summary>
        public string DiseaseCode
        {
            get { return diseaseCode; }
            set { diseaseCode = value; }
        }

        /// <summary>
        /// 转急诊医疗机构代码
        /// </summary>
        private string zzHosCode = "";
        /// <summary>
        /// 转急诊医疗机构代码
        /// </summary>
        public string ZzHosCode
        {
            get { return zzHosCode; }
            set { zzHosCode = value; }
        }

        #endregion

        #region 方法
        ///// <summary>
        /////  挂号的副本
        ///// </summary>
        ///// <returns></returns>
        public new RegisterExtend Clone()
        {
            RegisterExtend regExtend = base.Clone() as RegisterExtend;
            return regExtend;
        }
        #endregion
    }
}
