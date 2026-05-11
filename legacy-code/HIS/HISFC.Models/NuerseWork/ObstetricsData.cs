using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.NuerseWork
{
    /// <summary>
    /// 免费孕检项目确认表数据
    /// </summary>
    public class ObstetricsData
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime op_Date { get; set; }
        /// <summary>
        /// 本市
        /// </summary>
        public string local { get; set; }
        /// <summary>
        /// 非本市
        /// </summary>
        public string non_local { get; set; }
        /// <summary>
        /// 孕妇姓名
        /// </summary>
        public string full_name { get; set; }
        /// <summary>
        /// 所属行政（功能）区
        /// </summary>
        public string administrativeregion { get; set; }
        /// <summary>
        /// 是否首次申请孕检
        /// </summary>
        public string firstapplication { get; set; }
        /// <summary>
        /// 当前孕周
        /// </summary>
        public string gestational { get; set; }
        /// <summary>
        /// NIPT
        /// </summary>
        public string nipt { get; set; }
        /// <summary>
        /// 首次孕检建卡
        /// </summary>
        public string first_pregnancytest { get; set; }
        /// <summary>
        /// 常规孕检复检
        /// </summary>
        public string pregnancytest_retest { get; set; }
        /// <summary>
        /// HIV筛查
        /// </summary>
        public string hiv { get; set; }
        /// <summary>
        /// 梅毒筛查
        /// </summary>
        public string syphilis { get; set; }
        /// <summary>
        /// 乙肝筛查
        /// </summary>
        public string hepatitisb { get; set; }
        /// <summary>
        /// 全血细胞分析
        /// </summary>
        public string wholebloodcellanalysis { get; set; }
        /// <summary>
        /// 唐筛
        /// </summary>
        public string downsyndrome { get; set; }
        /// <summary>
        /// NT
        /// </summary>
        public string nt { get; set; }
        /// <summary>
        /// B超
        /// </summary>
        public string b_modeultrasonography { get; set; }
        /// <summary>
        /// 血糖筛查
        /// </summary>
        public string bloodsugar { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 操作人工号
        /// </summary>
        public string oper_code { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string oper_date { get; set; }
        /// <summary>
        /// 顺序号
        /// </summary>
        public string soid_id { get; set; }

    }
}
