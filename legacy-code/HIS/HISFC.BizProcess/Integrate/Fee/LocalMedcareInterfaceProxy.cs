using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Neusoft.HISFC.BizProcess.Interface.FeeInterface;
using System.Reflection;
using Neusoft.HISFC.Models.Fee.Outpatient;

namespace Neusoft.HISFC.BizProcess.Integrate.FeeInterface
{
    /// <summary>
    /// 本地医保结算--接口管理类
    /// {6F40EB9D-278E-4dd5-A9BE-45DBBA13F803}
    /// </summary>
    public class LocalMedcareInterfaceProxy : Neusoft.HISFC.BizProcess.Interface.FeeInterface.ILocalMedcare
    {
        #region 变量

        /// <summary>
        /// 医保接口实例
        /// </summary>
        protected ILocalMedcare medcaredInterface = null;
        /// <summary>
        /// 本地数据库连接
        /// </summary>
        private System.Data.IDbTransaction trans = null;
        /// <summary>
        /// 当前错误信息
        /// </summary>
        private string errMsg = string.Empty;
        /// <summary>
        /// 合同单位编码
        /// </summary>
        private string pactCode = null;
        /// <summary>
        /// 合同单位管理业务层
        /// </summary>
        private Neusoft.HISFC.BizLogic.Fee.PactUnitInfo pactManager = new Neusoft.HISFC.BizLogic.Fee.PactUnitInfo();
        /// <summary>
        /// 当前载入的接口类型
        /// </summary>
        private Hashtable interfaceHash = new Hashtable();

        #endregion

        #region 属性
        /// <summary>
        /// 合同单位编码
        /// </summary>
        public string PactCode
        {
            get { return this.pactCode;}
            set 
            {
                if (this.pactCode != value)
                {
                    this.pactCode = value;
                    this.medcaredInterface = this.GetInterfaceFromPact(this.pactCode);
                }
            }
        }
        /// <summary>
        /// 当前接口实例
        /// </summary>
        public Neusoft.HISFC.BizProcess.Interface.FeeInterface.ILocalMedcare NowMedcaredInterface
        {
            get
            {
                return this.medcaredInterface;
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public LocalMedcareInterfaceProxy()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strPactCode"></param>
        /// <param name="trans"></param>
        public LocalMedcareInterfaceProxy(string strPactCode, System.Data.IDbTransaction trans)
            : this()
        {
            this.PactCode = strPactCode;
            this.trans = trans;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~LocalMedcareInterfaceProxy()
        {
            this.medcaredInterface = null;
            if (this.interfaceHash != null && this.interfaceHash.Keys.Count > 0)
            {
                ILocalMedcare obj = null;
                foreach (string strKey in this.interfaceHash.Keys)
                {
                    obj = this.interfaceHash[strKey] as ILocalMedcare;
                    if (obj != null)
                    {
                        obj.Disconnect();
                    }
                }
            }
        }
        #endregion

        #region ILocalMedcare 成员
        /// <summary>
        /// 获取接口描述
        /// </summary>
        public string Description
        {
            get
            {
                if (this.medcaredInterface != null)
                {
                    return this.medcaredInterface.Description;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 错误编码
        /// </summary>
        public string ErrCode
        {
            get { return this.medcaredInterface.ErrCode; }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg
        {
            get { return this.medcaredInterface.ErrMsg; }
        }
        /// <summary>
        /// 设置事务
        /// </summary>
        /// <param name="t"></param>
        public void SetTrans(System.Data.IDbTransaction t)
        {
            this.trans = t;
            if (t != null && this.medcaredInterface != null)
            {
                try
                {
                    this.medcaredInterface.SetTrans(t);
                }
                catch (Exception objEx)
                {
                    this.errMsg = objEx.Message;
                }
            }
            return;
        }
        /// <summary>
        /// 本地事务
        /// </summary>
        public System.Data.IDbTransaction Trans
        {
            set
            {
                if (value != null)
                {
                    this.SetTrans(value);
                }
            }
        }

        /// <summary>
        /// 根据合同单位，计算病人费用信息
        /// pub_cost、 pay_cost、 own_cost、 eco_cost
        /// </summary>
        /// <param name="r">病人挂号信息</param>
        /// <param name="lstFeeItem">费用明细信息</param>
        /// <returns>成功 1 失败 -1</returns>
        public long ComputeOutPatientFeeCost(Neusoft.HISFC.Models.Registration.Register r, ref List<FeeItemList> lstFeeItem)
        {
            long lngRes = 1;
            if (r == null || lstFeeItem == null || lstFeeItem.Count <= 0)
                return lngRes;

            if (this.medcaredInterface != null)
            {
                lngRes = this.medcaredInterface.ComputeOutPatientFeeCost(r, ref lstFeeItem);
                if (lngRes < 0)
                {
                    this.errMsg = this.medcaredInterface.ErrMsg;
                }
            }
            return lngRes;
        }

        #endregion

        #region IMedcareTranscation 成员
        /// <summary>
        /// 接口开始数据事务函数
        /// </summary>
        public void BeginTranscation()
        {
            errMsg = null;
            if (this.pactCode == null)
            {
                this.errMsg = "合同单位没有赋值";

                return;
            }
            if (this.medcaredInterface == null)
            {
                this.medcaredInterface = this.GetInterfaceFromPact(this.pactCode);
            }

            if (this.medcaredInterface != null)
            {
                this.medcaredInterface.BeginTranscation();
            }
        }
        /// <summary>
        /// 接口提交方法
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        public long Commit()
        {
            long lngRes = 1;
            if (this.medcaredInterface != null)
            {
                lngRes = this.medcaredInterface.Commit();
                if (lngRes < 0)
                {
                    this.errMsg = this.medcaredInterface.ErrMsg;
                }
            }
            return lngRes;
        }
        /// <summary>
        /// 接口连接,初始化方法
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        public long Connect()
        {
            long lngRes = 1;
            this.errMsg = null;

            if (this.pactCode == null)
            {
                lngRes = -1;
                this.errMsg = "合同单位没有赋值";

                return lngRes;
            }
            if (this.medcaredInterface == null)
            {
                this.medcaredInterface = this.GetInterfaceFromPact(this.pactCode);
            }

            if (this.medcaredInterface != null)
            {
                lngRes = this.medcaredInterface.Connect();
                if (lngRes < 0)
                {
                    this.errMsg = this.medcaredInterface.ErrMsg;
                }
            }
            return lngRes;
        }
        /// <summary>
        /// 关闭接口连接 清空方法
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        public long Disconnect()
        {
            long lngRes = 1;
            if (this.medcaredInterface != null)
            {
                lngRes = this.medcaredInterface.Disconnect();
                if (lngRes < 0)
                {
                    this.errMsg = this.medcaredInterface.ErrMsg;
                }
            }

            return lngRes;
        }
        /// <summary>
        /// 接口回滚方法
        /// </summary>
        /// <returns>成功 1 失败 -1</returns>
        public long Rollback()
        {
            long lngRes = 1;
            if (this.medcaredInterface != null)
            {
                lngRes = this.medcaredInterface.Rollback();
                if (lngRes < 0)
                {
                    this.errMsg = this.medcaredInterface.ErrMsg;
                }
            }

            return lngRes;
        }

        #endregion

        #region 内部调用方法

        /// <summary>
        /// 通过合同单位编码获得
        /// </summary>
        /// <param name="pactCode">合同单位编码</param>
        /// <returns></returns>
        private ILocalMedcare GetInterfaceFromPact(string pactCode)
        {
            this.medcaredInterface = null;

            if (this.interfaceHash.ContainsKey(pactCode))
            {
                this.medcaredInterface = this.interfaceHash[pactCode] as ILocalMedcare;
                if (this.medcaredInterface != null)
                {
                    return this.medcaredInterface;
                }
            }

            if (this.trans != null)
            {
                this.pactManager.SetTrans(this.trans);
            }

            Neusoft.HISFC.Models.Base.PactInfo pactInfo = this.pactManager.GetPactUnitInfoByPactCode(pactCode);
            if (pactInfo == null)
            {
                this.errMsg = "获得患者合同单位出错!(接口)" + this.pactManager.Err;

                return this.medcaredInterface;
            }

            if (!string.IsNullOrEmpty(pactInfo.PactLocalDllName))
            {
                try
                {
                    Assembly a = Assembly.LoadFrom(System.Windows.Forms.Application.StartupPath + "\\" + Neusoft.FrameWork.WinForms.Classes.Function.PluginPath + "\\SI\\" + pactInfo.PactLocalDllName);

                    System.Type[] types = a.GetTypes();
                    foreach (System.Type type in types)
                    {
                        if (type.GetInterface("ILocalMedcare") != null)
                        {
                            this.medcaredInterface = System.Activator.CreateInstance(type) as ILocalMedcare;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    this.errMsg = e.Message;
                    this.medcaredInterface = null;
                }
                if (this.medcaredInterface != null)
                {
                    this.interfaceHash.Add(pactCode, this.medcaredInterface);
                }
            }

            return this.medcaredInterface;
        }

        #endregion
    }
}
