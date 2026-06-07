using Xunit;

namespace Pricing.RuleCenter.Tests;

public sealed class ClockUsageTests
{
    [Fact]
    public void ProductionCode_UsesClockAbstractionForCurrentTechnicalTime()
    {
        var repositoryRoot = FindRepositoryRoot();
        var srcRoot = Path.Combine(repositoryRoot, "src");
        var allowedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Path.GetFullPath(Path.Combine(srcRoot, "Infrastructure", "SystemClock.cs"))
        };

        var violations = Directory
            .EnumerateFiles(srcRoot, "*.cs", SearchOption.AllDirectories)
            .Where(path => !allowedFiles.Contains(Path.GetFullPath(path)))
            .SelectMany(ReadViolations)
            .ToList();

        Assert.True(
            violations.Count == 0,
            "生产代码禁止直接使用 DateTime.Now/DateTime.UtcNow，请通过 IClock 统一注入时间源。" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    private static IEnumerable<string> ReadViolations(string path)
    {
        var lines = File.ReadLines(path).ToArray();
        for (var index = 0; index < lines.Length; index++)
        {
            var line = StripLineComment(lines[index]);
            if (line.Contains("DateTime.Now", StringComparison.Ordinal) ||
                line.Contains("DateTime.UtcNow", StringComparison.Ordinal))
            {
                yield return $"{Path.GetRelativePath(FindRepositoryRoot(), path)}:{index + 1}: {lines[index].Trim()}";
            }
        }
    }

    private static string StripLineComment(string line)
    {
        var commentIndex = line.IndexOf("//", StringComparison.Ordinal);
        return commentIndex >= 0 ? line[..commentIndex] : line;
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")) &&
                Directory.Exists(Path.Combine(directory.FullName, "src")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("无法从测试输出目录定位仓库根目录。");
    }
}
