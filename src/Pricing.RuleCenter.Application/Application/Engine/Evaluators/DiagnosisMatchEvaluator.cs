namespace Pricing.RuleCenter.Application.Engine.Evaluators;

/// <summary>
/// 诊断/病种条件评估器，从 ExtraParams["diagnosisCodes"] 读取诊断编码列表。
/// </summary>
public sealed class DiagnosisMatchEvaluator : ExtraParamMatchEvaluatorBase
{
    /// <summary>
    /// 初始化诊断/病种条件评估器。
    /// </summary>
    public DiagnosisMatchEvaluator()
        : base("diagnosisCodes")
    {
    }

    /// <summary>
    /// 获取条件类型编码。
    /// </summary>
    public override string ConditionType => "DIAGNOSIS_MATCH";
}
