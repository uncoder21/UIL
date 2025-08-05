using System.Collections.Generic;
using System.IO;
using Xunit;

namespace UIL.Tests;

/// <summary>
/// Discovers pairs of source and expected IL files and verifies that compiling
/// the source code results in the exact expected IL sequence. This allows new
/// tests to be added by simply creating <c>.uil</c> and <c>.il</c> files in the
/// <c>TestCases</c> directory.
/// </summary>
public class ILGoldenTests
{
    public static IEnumerable<object[]> GetTestCases()
    {
        var baseDir = Path.Combine(AppContext.BaseDirectory, "TestCases");
        foreach (var sourcePath in Directory.EnumerateFiles(baseDir, "*.uil"))
        {
            var expectedPath = Path.ChangeExtension(sourcePath, ".il");
            yield return new object[] { sourcePath, expectedPath };
        }
    }

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void EmittedILMatchesExpectation(string sourcePath, string expectedPath)
    {
        var source = File.ReadAllText(sourcePath);
        var expected = File.ReadAllText(expectedPath);

        var actual = TestCompiler.EmitIL(source);
        Assert.Equal(TestCompiler.Normalize(expected), actual);
    }
}

