using System.Xml.Linq;
using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ProjectReleaseGateTests
{
    [Fact]
    public void Projects_ShouldTargetSupportedRuntimeAndNotSuppressEndOfSupportWarning()
    {
        var projectFiles = Directory.GetFiles(FindRepositoryRoot(), "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.NotEmpty(projectFiles);

        var failures = new List<string>();
        foreach (var projectFile in projectFiles)
        {
            var document = XDocument.Load(projectFile);
            var targetFrameworks = document
                .Descendants("TargetFramework")
                .Select(element => element.Value.Trim())
                .ToArray();
            var noWarnValues = document
                .Descendants("NoWarn")
                .Select(element => element.Value)
                .ToArray();

            if (targetFrameworks.Any(value => string.Equals(value, "net6.0", StringComparison.OrdinalIgnoreCase)))
            {
                failures.Add($"{projectFile} targets net6.0");
            }

            if (noWarnValues.Any(value => value.Contains("NETSDK1138", StringComparison.OrdinalIgnoreCase)))
            {
                failures.Add($"{projectFile} suppresses NETSDK1138");
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void Repository_ShouldContainWindowsFriendlyReleaseWorkflowAndSqlValidationScript()
    {
        var root = FindRepositoryRoot();
        var workflowPath = Path.Combine(root, ".github", "workflows", "rule-center-ci.yml");
        var scriptPath = Path.Combine(root, "scripts", "validate-release-assets.ps1");
        var oracleScriptPath = Path.Combine(root, "scripts", "run-oracle-integration-tests.ps1");

        Assert.True(File.Exists(workflowPath), $"Missing workflow: {workflowPath}");
        Assert.True(File.Exists(scriptPath), $"Missing release validation script: {scriptPath}");
        Assert.True(File.Exists(oracleScriptPath), $"Missing Oracle integration script: {oracleScriptPath}");

        var workflowContent = File.ReadAllText(workflowPath);
        Assert.Contains("runs-on: windows-latest", workflowContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Validate release assets", workflowContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@".\scripts\validate-release-assets.ps1", workflowContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Oracle integration tests", workflowContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@".\scripts\run-oracle-integration-tests.ps1", workflowContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("PRICING_ORACLE_CONNECTION_STRING", workflowContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("docker", workflowContent, StringComparison.OrdinalIgnoreCase);
    }

    private static string FindRepositoryRoot()
    {
        var directory = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(directory))
        {
            if (Directory.Exists(Path.Combine(directory, ".git")))
            {
                return directory;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new DirectoryNotFoundException("Cannot locate repository root.");
    }
}
