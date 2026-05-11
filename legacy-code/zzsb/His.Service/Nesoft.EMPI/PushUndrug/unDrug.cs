using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.EMPI.PushUndrug
{
    public class unDrug
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string MD_UNDRUG_CODE { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string MD_UNDRUG_NAME { get; set; }
        /// <summary>
        /// 项目费别
        /// </summary>
        public string MD_FEE_CODE { get; set; }
        /// <summary>
        /// 拼音码
        /// </summary>
        public string MD_SPELL_CODE { get; set; }
        /// <summary>
        /// 执行科室
        /// </summary>
        public string MD_EXEDEPT_CODE { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string MD_STOCK_UNIT { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public string MD_UNIT_PRICE { get; set; }
        /// <summary>
        /// 自费项目1是自费，0是非自费
        /// </summary>
        public string MD_SPECIAL_FLAG2 { get; set; }
        /// <summary>
        /// 自付比例
        /// </summary>
        public string MD_SPECIAL_FLAG3 { get; set; }
        /// <summary>
        /// 适用范围0 全部  1 门诊 2住院 
        /// </summary>
        public string MD_APPLICABILITYAREA { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string MD_VALID_STATE { get; set; }
        /// <summary>
        /// 物价编码
        /// </summary>
        public string MD_UNDRUG_FEE_CODE { get; set; }
        /// <summary>
        /// 物价名称
        /// </summary>
        public string MD_UNDRUG_FEE_NAME { get; set; }
        /// <summary>
        /// 备用一
        /// </summary>
        public string MD_UNDRUG_EXT1 { get; set; }
        /// <summary>
        /// 备用二
        /// </summary>
        public string MD_UNDRUG_EXT2 { get; set; }
        /// <summary>
        /// 备用三
        /// </summary>
        public string MD_UNDRUG_EXT3 { get; set; }
        /// <summary>
        /// 操作员姓名
        /// </summary>
        public string MD_OPER_NAME { get; set; }
        /// <summary>
        /// 操作员编号
        /// </summary>
        public string MD_OPER_CODE { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string MD_OPER_TIME { get; set; }
        /// <summary>
        /// 操作类型标志新增N 更新U 删除D
        /// </summary>
        public string DOEVENT { get; set; }

    }
}
