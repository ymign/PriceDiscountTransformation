using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizProcess.Interface.Fee
{
    /// <summary>
    /// 珠海单病种分值预计算接口
    /// </summary>
    public interface IZHICDScore
    {
        /// <summary>
        /// 设置住院流水号
        /// </summary>
        /// <param name="inpatientNo">住院流水号</param>
        /// <param name="erro"> 错误信息</param>
        /// <returns>-1 失败; 1 成功</returns>
        int SetinpatientNo(string inpatientNo,ref string erro);

        /// <summary>
        /// 计算分值
        /// </summary>
        /// <param name="yearcode">年度</param>
        /// <param name="icd10">病种名称</param>
        /// <param name="icd9">手术与操作编码</param>
        /// <param name="totcost">住院总费用 totcost=0 住院患者,totcost>0出院患者</param>
        /// <param name="controlCost">住院控制费用</param>
        /// <param name="erro">错误信息</param>
        /// <returns>-1 计算失败;>0 计算分值</returns>
        decimal Calculate(string yearcode, string icd10, string icd9, decimal totcost, ref decimal controlCost, ref string erro);

        /// <summary>
        /// 计算分值
        /// </summary>
        /// <param name="icd10">病种名称</param>
        /// <param name="icd9">手术与操作编码</param>
        /// <param name="totcost">住院总费用</param>
        /// <param name="controlCost">住院控制费用</param>
        /// <param name="erro">错误信息</param>
        /// <returns>-1 计算失败;>0 计算分值</returns>
        decimal Calculate(string icd10, string icd9, ref decimal controlCost, ref string erro);

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="icd10">ICD-10编码</param>
        /// <param name="icd9">手术与操作编码</param>
        /// <returns></returns>
        void SetInfo(Neusoft.FrameWork.Models.NeuObject icd10 , Neusoft.FrameWork.Models.NeuObject icd9);

        /// <summary>
        /// 界面初始化
        /// </summary>
        void LoadData();
    }
}
