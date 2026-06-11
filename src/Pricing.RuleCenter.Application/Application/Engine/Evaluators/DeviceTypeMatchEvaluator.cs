namespace Pricing.RuleCenter.Application.Engine.Evaluators;

/// <summary>
/// 设备型号条件评估器，从 ExtraParams["deviceType"] 读取设备型号或设备等级编码。
/// </summary>
public sealed class DeviceTypeMatchEvaluator : ExtraParamMatchEvaluatorBase
{
    /// <summary>
    /// 初始化设备型号条件评估器。
    /// </summary>
    public DeviceTypeMatchEvaluator()
        : base("deviceType")
    {
    }

    /// <summary>
    /// 获取条件类型编码。
    /// </summary>
    public override string ConditionType => "DEVICE_TYPE_MATCH";
}
