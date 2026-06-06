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
