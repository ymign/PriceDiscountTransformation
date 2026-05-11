using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
    public class InPatientInfo
    {
        public InPatientInfo()
        {

        }


        /// <summary>
        /// 住院流水号
        /// </summary>
        public string InpatientNo { get; set; }
        /// <summary>
        /// 门诊号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 住院号
        /// </summary>
        public string PatientNo { get; set; }
        /// <summary>
        /// 医疗证号
        /// </summary>
        public string McardNo { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string PatientName { get; set; }
        /// <summary>
        /// 出生年月
        /// </summary>
        public string BirthDay { get; set; }
        /// <summary>
        /// 性别代码
        /// </summary>
        public string SexCode { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContectName { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string ContectTel { get; set; }
        /// <summary>
        /// 住址
        /// </summary>
        public string HomeAddr { get; set; }
        /// <summary>
        /// 合同单位代码
        /// </summary>
        public string PactCode { get; set; }
        /// <summary>
        /// 合同单位名称
        /// </summary>
        public string PactName { get; set; }
        /// <summary>
        /// 住院科室编码
        /// </summary>
        public string DeptCode { get; set; }
        /// <summary>
        /// 住院科室名
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 住院护理站编码
        /// </summary>
        public string NurseCellCode { get; set; }
        /// <summary>
        /// 住院护理站名
        /// </summary>
        public string NurseCellName { get; set; }
        /// <summary>
        /// 主治医生编码
        /// </summary>
        public string DoctCode { get; set; }
        /// <summary>
        /// 主治医生名
        /// </summary>
        public string DoctName { get; set; }
        /// <summary>
        /// 入院日期
        /// </summary>
        public string InDate { get; set; }
        /// <summary>
        /// 出院日期
        /// </summary>
        public string OutDate { get; set; }
        /// <summary>
        /// 费用总额
        /// </summary>
        public double TotalCost { get; set; }
        /// <summary>
        /// 预交金总额
        /// </summary>
        public double PrepayCost { get; set; }
        /// <summary>
        /// 自费金额
        /// </summary>
        public double OwnCost { get; set; }
        /// <summary>
        /// 账户消费金额
        /// </summary>
        public double PayCost { get; set; }
        /// <summary>
        /// 统筹报销金额
        /// </summary>
        public double PubCost { get; set; }
        /// <summary>
        /// 其他报销金额
        /// </summary>
        public double OthCost { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public double Balance { get; set; }
        /// <summary>
        /// 出院小结
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 住院状态
        /// </summary>
        public string InState { get; set; }
        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }
        /// <summary>
        /// 入院诊断
        /// </summary>
        public string InDiagnose { get; set; }
        /// <summary>
        /// 出院诊断
        /// </summary>
        public string OutDiagnose { get; set; }
        /// <summary>
        /// 住院病房号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 床位号
        /// </summary>
        public string BedNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// 拓展字段1
        /// </summary>
        public string Extend1 { get; set; }
        /// <summary>
        /// 拓展字段2
        /// </summary>
        public string Extend2 { get; set; }
        /// <summary>
        /// 拓展字段3
        /// </summary>
        public string Extend3 { get; set; }
        /// <summary>
        /// 订单号或者银行卡卡号
        /// </summary>
        public string BankCardNo { get; set; }
    }

    public class InPatientReq : Base.Request
    {
        public string CardNo { get; set; }
        public string PatientID { get; set; }
        public string Name { get; set; }
        public string DebitCardNo { get; set; }
    }


    public class InpatientPrePayReq : Base.Request
    {
        public string InpatientNo { get; set; }
        public int PaymentWay { get; set; }
        public string SettleDate { get; set; }
        public string TermialType { get; set; }
        public decimal TotalFee { get; set; }
        public string BankCardNo { get; set; }
        //应用系统订单号
        public string ApplicationOrderNo { get; set; }

        public string PlatformOrderNo { get; set; }

    }

    public class InpatientTotDayFeeReq : Base.Request
    {
        public string InpatientNo { get; set; }
        public string FeeDate { get; set; }
    }

    public class InpatientFeeDetailReq : Base.Request
    {
        public string InpatientNo { get; set; }
        public string InvoiceNo { get; set; }
        public string CardNo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
