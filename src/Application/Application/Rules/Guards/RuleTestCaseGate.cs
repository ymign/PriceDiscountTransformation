using Pricing.RuleCenter.Application.Dto;
using Pricing.RuleCenter.Core.Constants;
using Pricing.RuleCenter.Core.Interfaces.Rules;

namespace Pricing.RuleCenter.Application.Rules.Guards;

/// <summary>
/// 规则发布前测试用例门禁。
/// </summary>
public sealed class RuleTestCaseGate
{
    private readonly IRuleTestCaseRepository _testCaseRepository;
    private readonly IRuleTestRunRepository _testRunRepository;

    /// <summary>
    /// 初始化规则测试用例门禁。
    /// </summary>
    public RuleTestCaseGate(
        IRuleTestCaseRepository testCaseRepository,
        IRuleTestRunRepository testRunRepository)
    {
        _testCaseRepository = testCaseRepository;
        _testRunRepository = testRunRepository;
    }

    /// <summary>
    /// 确保目标版本存在启用测试用例且最新运行全部通过。
    /// </summary>
    public async Task EnsurePassedAsync(long ruleId, int versionNo)
    {
        var enabledCases = (await _testCaseRepository.GetByRuleAndVersionAsync(ruleId, versionNo))
            .Where(c => string.Equals(c.IsEnabled, EnableFlag.Yes, StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (enabledCases.Count == 0)
        {
            throw new BizException(
                BizErrorCode.MissingTestCase,
                409,
                $"规则 RuleId={ruleId}, VersionNo={versionNo} 缺少启用测试用例");
        }

        foreach (var testCase in enabledCases)
        {
            if (string.IsNullOrWhiteSpace(testCase.InputJson) ||
                string.IsNullOrWhiteSpace(testCase.ExpectedJson))
            {
                throw new BizException(
                    BizErrorCode.TestCaseIncomplete,
                    409,
                    $"TestCaseId={testCase.TestCaseId} 缺少 InputJson 或 ExpectedJson");
            }

            var latestRun = (await _testRunRepository.GetByTestCaseIdAsync(testCase.TestCaseId))
                .OrderByDescending(r => r.RunAt)
                .ThenByDescending(r => r.TestRunId)
                .FirstOrDefault();
            if (latestRun is null)
            {
                throw new BizException(
                    BizErrorCode.TestRunMissing,
                    409,
                    $"TestCaseId={testCase.TestCaseId} 尚未执行测试");
            }

            if (!string.Equals(latestRun.IsPass, EnableFlag.Yes, StringComparison.OrdinalIgnoreCase))
            {
                throw new BizException(
                    BizErrorCode.TestRunFailed,
                    409,
                    $"TestCaseId={testCase.TestCaseId} 最新测试未通过");
            }
        }
    }
}
