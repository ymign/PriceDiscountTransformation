using System;
using System.Collections.Generic;
using System.Text;
using Neusoft.HISFC.Models.RADT;
using Neusoft.HISFC.Models.Registration;
using System.Drawing;
using System.Collections;

namespace Neusoft.HISFC.BizProcess.Interface.Order
{
    /// <summary>
    /// 合理用药接口定义
    /// </summary>
    public interface IPASS
    {
        /* 基本功能描述
         * 1、输入药品后，显示要点提示
         * 2、点击药品名称列或者双击行，再次显示要点提示
         * 3、每输入一个药品都提交合理用药进行审查
         * 4、保存时进行统一审查
         * 5、右键可以查看合理用药的功能信息菜单
         * 
         * */

        /// <summary>
        /// 工作站类别
        /// </summary>
        Neusoft.HISFC.Models.Base.ServiceTypes PassStationType
        {
            get;
            set;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        string PassErr
        {
            get;
            set;
        }
        
        /// <summary>
        /// 合理用药系统初始化
        /// </summary>
        /// <param name="logEmpl">登陆人员</param>
        /// <param name="logDept">登陆科室</param>
        /// <param name="workStationType">工作站类型</param>
        /// <returns>0 初始化失败 1 初始化成功</returns>
        int Pass4_Init(int w, string pcCheckMode, string pcHisCode, string pcDoctorCode);

        /// <summary>
        /// 设置传入患者基本信息
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        int PassSetPatientInfo(Neusoft.HISFC.Models.RADT.Patient patient, Neusoft.FrameWork.Models.NeuObject recipeDoct);

        /// <summary>
        /// 传入病人诊断记录信息
        /// </summary>
        /// <param name="diagnoseList"></param>
        /// <returns></returns>
        int PassSetDiagnoses(ArrayList diagnoseList);

        /// <summary>
        /// 合理用药审查
        /// </summary>
        /// <param name="checkType">1 用药审查，0 用药指导单</param>
        /// <param name="alOrder"></param>
        /// <returns></returns>
        int PASSCheck(int checkType, ArrayList alOrder);

        /// <summary>
        /// 合理用药功能初始化刷新
        /// </summary>
        /// <returns></returns>
        int PassRefresh();

        /// <summary>
        /// 合理用药功能关闭
        /// </summary>
        /// <returns></returns>
        int PassClose();

        /// <summary>
        /// <summary>
        /// 传病人药品记录信息
        /// </summary>
        /// <param name="alOrder"></param>
        void PassSetRecipeInfo(int w, ArrayList alOrder);

        /// <summary>
        /// 右键查询药品信息
        /// </summary>
        /// <param name="drugCode"></param>
        /// <param name="drugName"></param>
        /// <returns></returns>
        int Pass4DrugInfo(int w, string drugCode, string drugName);

        /// <summary>
        /// 设置浮动窗口是否显示
        /// </summary>
        /// <param name="isShow"></param>
        int PassShowFloatWindow(int rownum);

        /// <summary>
        /// 传入一个查询药品函数(显示工具条)
        /// </summary>
        /// <param name="drugCode"></param>
        /// <param name="drugName"></param>
        /// <returns></returns>
        int Pass4Toolbar(string drugCode, string drugName);

        /// <summary>
        /// 传入病人过敏史信息
        /// </summary>
        /// <param name="diagnoseList"></param>
        /// <returns></returns>
        int PassAllergyInfo(ArrayList AllergyList);
    }
}
