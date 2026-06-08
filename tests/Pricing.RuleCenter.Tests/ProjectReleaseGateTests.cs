using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pricing.RuleCenter.Api.Controllers;
using Pricing.RuleCenter.Api.Security;
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
    public void Projects_ShouldNotReferenceMicrosoftExtensions6xWhenTargetingNet8()
    {
        var root = FindRepositoryRoot();
        var projectFiles = new[]
        {
            Path.Combine(root, "src", "Pricing.RuleCenter.Application", "Pricing.RuleCenter.Application.csproj"),
            Path.Combine(root, "src", "Pricing.RuleCenter.Infrastructure", "Pricing.RuleCenter.Infrastructure.csproj")
        };

        var failures = new List<string>();
        foreach (var projectFile in projectFiles)
        {
            var document = XDocument.Load(projectFile);
            var packageReferences = document
                .Descendants("PackageReference")
                .Select(element => new
                {
                    Include = element.Attribute("Include")?.Value,
                    Version = element.Attribute("Version")?.Value
                })
                .Where(item => !string.IsNullOrWhiteSpace(item.Include) && !string.IsNullOrWhiteSpace(item.Version))
                .ToArray();

            foreach (var package in packageReferences)
            {
                if (package.Include!.StartsWith("Microsoft.Extensions.", StringComparison.OrdinalIgnoreCase) &&
                    package.Version!.StartsWith("6.", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add($"{projectFile} still references {package.Include} {package.Version}");
                }
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

    [Fact]
    public void LegacyAuthoringControllers_ShouldBeHiddenFromSwaggerAndGuarded()
    {
        var controllers = new[]
        {
            typeof(RuleHeaderController),
            typeof(RuleVersionController),
            typeof(RuleConditionController),
            typeof(RuleActionController),
            typeof(RuleApprovalController),
            typeof(RulePublishController)
        };

        foreach (var controller in controllers)
        {
            var explorer = controller.GetCustomAttributes(typeof(ApiExplorerSettingsAttribute), inherit: false)
                .Cast<ApiExplorerSettingsAttribute>()
                .SingleOrDefault();
            Assert.NotNull(explorer);
            Assert.True(explorer!.IgnoreApi);

            var serviceFilter = controller.GetCustomAttributes(typeof(ServiceFilterAttribute), inherit: false)
                .Cast<ServiceFilterAttribute>()
                .SingleOrDefault(attribute => attribute.ServiceType == typeof(LegacyRuleAuthoringGuardFilter));
            Assert.NotNull(serviceFilter);
        }
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
