using System;
using System.Collections.Generic;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface
{
    #region 虚方法实现

    ///// <summary>
    ///// 处方打印接口，虚方法
    ///// </summary>
    //public abstract class IRecipePrint : IRecipePrintBase
    //{
    //    #region IRecipePrintBase 成员

    //    public virtual void SetPatientInfo(Neusoft.HISFC.Models.Registration.Register register)
    //    {
    //        return;
    //    }

    //    public virtual string RecipeNO
    //    {
    //        get
    //        {
    //            return null;
    //        }
    //        set
    //        {
    //        }
    //    }

    //    public void ppp()
    //    {
    //    }

    //    public virtual void PrintRecipe()
    //    {
    //        return;
    //    }

    //    public virtual void PrintRecipeView()
    //    {
    //        return;
    //    }

    //    public virtual string Printer
    //    {
    //        get
    //        {
    //            return null;
    //        }
    //        set
    //        {
    //        }
    //    }

    //    #endregion
    //}

    #endregion

    #region 接口定义

    /// <summary>
    /// 门诊处方打印接口
    /// </summary>
    public interface IRecipePrint
    {
        /// <summary>
        /// 患者信息
        /// </summary>
        /// <param name="register"></param>
        int SetPatientInfo(Neusoft.HISFC.Models.Registration.Register register);

        /// <summary>
        /// 处方号
        /// </summary>
        string RecipeNO
        {
            get;
            set;
        }

        /// <summary>
        /// 打印
        /// </summary>
        int PrintRecipe();

        /// <summary>
        /// 打印预览用
        /// </summary>
        int PrintRecipeView(System.Collections.ArrayList alRecipe); 

        /// <summary>
        /// 设置打印机
        /// </summary>
        string Printer
        {
            get;
            set;
        }
    }

    #endregion
}
