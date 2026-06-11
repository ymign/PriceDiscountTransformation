using Pricing.RuleCenter.Core.Services;

namespace Pricing.RuleCenter.Application.Engine.Formula;

/// <summary>
/// 表达式公式函数白名单。
/// </summary>
public sealed class FormulaFunctionRegistry
{
    private static readonly HashSet<string> SupportedFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        "min",
        "max",
        "round",
        "ceil",
        "floor"
    };

    /// <summary>
    /// 判断函数是否在表达式白名单内。
    /// </summary>
    public bool IsSupported(string name)
    {
        return SupportedFunctions.Contains(name);
    }

    /// <summary>
    /// 执行白名单函数。
    /// </summary>
    public decimal Invoke(string name, IReadOnlyList<decimal> args)
    {
        switch (name.ToLowerInvariant())
        {
            case "min":
                EnsureArgCount(name, args, 2);
                return Math.Min(args[0], args[1]);
            case "max":
                EnsureArgCount(name, args, 2);
                return Math.Max(args[0], args[1]);
            case "round":
                EnsureArgCount(name, args, 2);
                return PricingAmountRounder.RoundFinal(args[0], DecimalToScale(args[1]));
            case "ceil":
                EnsureArgCount(name, args, 1);
                return Math.Ceiling(args[0]);
            case "floor":
                EnsureArgCount(name, args, 1);
                return Math.Floor(args[0]);
            default:
                throw new InvalidOperationException($"不支持的公式函数: {name}");
        }
    }

    private static void EnsureArgCount(string name, IReadOnlyList<decimal> args, int expected)
    {
        if (args.Count != expected)
        {
            throw new InvalidOperationException($"函数 {name} 需要 {expected} 个参数");
        }
    }

    private static int DecimalToScale(decimal value)
    {
        if (value < 0m || value > 8m || value != Math.Truncate(value))
        {
            throw new InvalidOperationException("round 的 scale 必须是 0 到 8 的整数");
        }

        return (int)value;
    }
}
