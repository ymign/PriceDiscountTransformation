namespace Pricing.RuleCenter.Core.Engine.Evaluators;

/// <summary>
/// 医保身份条件评估器，从 ExtraParams["insuranceType"] 读取渠道传入的医保身份编码。
/// </summary>
public sealed class InsuranceTypeMatchEvaluator : ExtraParamMatchEvaluatorBase
{
    /// <summary>
    /// 初始化医保身份条件评估器。
    /// </summary>
    public InsuranceTypeMatchEvaluator()
        : base("insuranceType")
    {
    }

    /// <summary>
    /// 获取条件类型编码。
    /// </summary>
    public override string ConditionType => "INSURANCE_TYPE_MATCH";
}
