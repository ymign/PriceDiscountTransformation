using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class RuntimePackageTraceResolverTests
{
    [Fact]
    public void PricingWorkflows_DoNotResolveRuntimePackageTrace()
    {
        var workflowSources = new[]
        {
            ReadRepoFile(
                "src",
                "Pricing.RuleCenter.Application",
                "Application",
                "Pricing",
                "Workflows",
                "PricingSimulateWorkflow.cs"),
            ReadRepoFile(
                "src",
                "Pricing.RuleCenter.Application",
                "Application",
                "Pricing",
                "Workflows",
                "PricingConfirmWorkflow.cs")
        };

        foreach (var source in workflowSources)
        {
            Assert.DoesNotContain("RuntimePackageTraceResolver", source);
            Assert.DoesNotContain("RuntimeTrace =", source);
            Assert.DoesNotContain("ResolveAsync(calculations)", source);
        }
    }

    [Fact]
    public void RuntimePackageTraceResolver_IsNotPartOfPricingApplication()
    {
        Assert.False(RepoFileExists(
            "src",
            "Pricing.RuleCenter.Application",
            "Application",
            "RuntimePackages",
            "RuntimePackageTraceResolver.cs"));
        Assert.False(RepoFileExists(
            "src",
            "Pricing.RuleCenter.Application",
            "Application",
            "RuntimePackages",
            "RuntimePackageTraceResolution.cs"));
    }

    private static string ReadRepoFile(params string[] pathParts)
    {
        var path = ResolveRepoPath(pathParts);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"无法定位仓库文件：{Path.Combine(pathParts)}", path);
        }

        return File.ReadAllText(path);
    }

    private static bool RepoFileExists(params string[] pathParts) => File.Exists(ResolveRepoPath(pathParts));

    private static string ResolveRepoPath(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(candidate) || Directory.Exists(Path.GetDirectoryName(candidate)))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"无法定位仓库路径：{Path.Combine(pathParts)}");
    }
}
