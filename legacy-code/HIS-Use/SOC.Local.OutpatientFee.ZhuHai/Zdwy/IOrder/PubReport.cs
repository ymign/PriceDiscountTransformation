using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.SOC.Local.OutpatientFee.ZhuHai.Zdwy.IOrder
{
    public class PubReport : Neusoft.FrameWork.Models.NeuObject
    {
        public PubReport()
        {
            this.Begin = new DateTime();
            this.Bill_No = "";
            this.End = new DateTime();
            this.EXT_FLAG = "";
            this.EXT_FLAG2 = "";
            this.FEE_FLAG = "";
            this.Fee_Type = "";
            this.InpatientNo = "";
            this.IsInHos = "";
            this.IsValid = "";
            this.MCardNo = "";
            this.OperCode = "";
            this.OperDate = new DateTime();
            this.Pact = new Neusoft.FrameWork.Models.NeuObject();
            this.REP_FLAG = "";
            this.SortId = "";
            this.Static_Month = new DateTime();
            this.OperDate = new DateTime();
            WorkCode = "";
            WorkName = "";
            PatientNO = "";
            this.InvoiceNo = "";

        }
        /// <summary>
        /// 统计月份
        /// </summary>
        public DateTime Static_Month;
        /// <summary>
        /// 门诊费用记账号 或者 住院费用托收单号
        /// </summary>
        public string Bill_No;
        /// <summary>
        /// 公费类别代码--卡号前2位或3位
        /// </summary>
        public Neusoft.FrameWork.Models.NeuObject Pact = new Neusoft.FrameWork.Models.NeuObject();
        /// <summary>
        /// 1 住院费用 2 门诊费用
        /// </summary>
        public string Fee_Type;
        /// <summary>
        /// 数量
        /// </summary>
        public int Amount;
        /// <summary>
        /// 01床位费
        /// </summary>
        public decimal Bed_Fee;
        /// <summary>
        /// 02药品费
        /// </summary>
        public decimal YaoPin;
        /// <summary>
        /// 03成药费
        /// </summary>
        public decimal ChengYao;
        /// <summary>
        /// 04化验费
        /// </summary>
        public decimal HuaYan;
        /// <summary>
        /// 05检查费
        /// </summary>
        public decimal JianCha;
        /// <summary>
        /// 06放射费
        /// </summary>
        public decimal FangShe;
        /// <summary>
        /// 07治疗费
        /// </summary>
        public decimal ZhiLiao;
        /// <summary>
        /// 08手术费
        /// </summary>
        public decimal ShouShu;
        /// <summary>
        /// 09输血费
        /// </summary>
        public decimal ShuXue;
        /// <summary>
        /// 10输氧费
        /// </summary>
        public decimal ShuYang;
        /// <summary>
        /// 11特殊治疗
        /// </summary>
        public decimal TeZhi;
        /// <summary>
        /// 12特殊治疗比例
        /// </summary>
        public decimal TeZhiRate;
        /// <summary>
        /// 13MR费
        /// </summary>
        public decimal MR;
        /// <summary>
        /// 14CT费
        /// </summary>
        public decimal CT;
        /// <summary>
        ///15血透费 
        /// </summary>
        public decimal XueTou;
        /// <summary>
        /// 16诊金费
        /// </summary>
        public decimal ZhenJin;
        /// <summary>
        /// 17草药费
        /// </summary>
        public decimal CaoYao;
        /// <summary>
        /// 18特检费
        /// </summary>
        public decimal TeJian;
        /// <summary>
        /// 19审药费
        /// </summary>
        public decimal ShenYao;
        /// <summary>
        /// 20监护费
        /// </summary>
        public decimal JianHu;
        /// <summary>
        /// 51省诊费
        /// </summary>
        public decimal ShengZhen;
        /// <summary>
        /// 总记账
        /// </summary>
        public decimal Tot_Cost;
        /// <summary>
        /// 实际记账
        /// </summary>
        public decimal Pub_Cost;
        /// <summary>
        /// 自付比例
        /// </summary>
        public decimal Pay_Rate;
        /// <summary>
        /// 特殊药品总费用 30%比例
        /// </summary>
        public decimal SpDrugFeeTot;

        public decimal TeYaoRate;

        /// <summary>
        /// 特殊药品实际费用 30%比例
        /// </summary>
        public decimal SpDrugFeeSj;

        //--------------4.0新增-------
        /// <summary>
        /// 11接生费
        /// </summary>
        public decimal JieSheng;
        /// <summary>
        /// 12高氧费
        /// </summary>
        public decimal GaoYang;
        //--------------4.0新增-------

        /// <summary>
        /// 是否可用. 2未确认，1已确认，0已作废
        /// </summary>
        public string IsValid = "1";
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Begin;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End;
        /// <summary>
        /// 全位卡号
        /// </summary>
        public string MCardNo;
        /// <summary>
        /// 是否在院 1在 0 不在
        /// </summary>
        public string IsInHos;
        /// <summary>
        /// 操作员
        /// </summary>
        public string OperCode;
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime OperDate = DateTime.MinValue;
        /// <summary>
        /// 费用类型 0 正常 1 特殊
        /// </summary>
        public string FEE_FLAG;
        /// <summary>
        /// 报表标志 0 未统计,1已统计
        /// </summary>
        public string REP_FLAG;
        /// <summary>
        /// 附加标志
        /// </summary>
        public string EXT_FLAG;
        public string EXT_FLAG2;

        /// <summary>
        /// 发票号内流水号
        /// </summary>
        public int Seq = 0;

        public string InpatientNo;

        public string SortId;

        /// <summary>
        /// 单位
        /// </summary>
        public string WorkName;

        /// <summary>
        /// 单位代号
        /// </summary>
        public string WorkCode;

        /// <summary>
        /// 住院号
        /// </summary>
        public string PatientNO;

        #region [2010-01-27]修改公医报表新增
        /// <summary>
        /// 药费超标金额
        /// </summary>
        public decimal OverLimitDrugFee;

        /// <summary>
        /// 审批肿瘤药费
        /// </summary>
        public decimal CancerDrugFee;

        /// <summary>
        /// 监护床位费
        /// </summary>
        public decimal BedFee_JianHu;

        /// <summary>
        /// 层流床位费
        /// </summary>
        public decimal BedFee_CengLiu;

        /// <summary>
        /// 单位支付金额
        /// </summary>
        public decimal CompanyPay;

        /// <summary>
        /// 自费金额
        /// </summary>
        public decimal SelfPay;

        /// <summary>
        /// 医疗费总金额
        /// </summary>
        public decimal TotalFee;

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo;

        #endregion

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new PubReport Clone()
        {
            PubReport obj = base.Clone() as PubReport;
            obj.Pact = this.Pact.Clone();
            return obj;
        }	
    }
}
