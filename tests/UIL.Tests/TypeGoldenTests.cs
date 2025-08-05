using System.Collections.Generic;
using System.IO;
using System.Linq;
using UIL.Binding;
using UIL.Syntax;
using UIL.Symbols;
using Xunit;

namespace UIL.Tests;

/// <summary>
/// Discovers source files paired with expected type declarations and verifies
/// that binding the source registers the expected set of types.
/// </summary>
public class TypeGoldenTests
{
    public static IEnumerable<object[]> GetTestCases()
    {
        var baseDir = Path.Combine(AppContext.BaseDirectory, "TestCases");
        foreach (var sourcePath in Directory.EnumerateFiles(baseDir, "*.uil"))
        {
            var expectedPath = Path.ChangeExtension(sourcePath, ".types");
            if (File.Exists(expectedPath))
                yield return new object[] { sourcePath, expectedPath };
        }
    }

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void DeclaredTypesMatchExpectation(string sourcePath, string expectedPath)
    {
        var source = File.ReadAllText(sourcePath);
        var expected = File.ReadAllLines(expectedPath);

        var tree = SyntaxTree.Parse(source);
        var binder = new Binder();
        binder.BindCompilationUnit(tree.Root);

        var actual = binder.Types.Values
            .Select(FormatType)
            .OrderBy(t => t)
            .ToArray();

        foreach (var line in expected)
        {
            var name = line.Split('<')[0];
            Assert.True(binder.TryLookupType(name, out _), $"Type '{name}' not found");
        }

        Assert.Equal(expected, actual);
    }

    private static string FormatType(TypeSymbol type)
        => type.TypeParameters.Count == 0
            ? type.Name
            : $"{type.Name}<" + string.Join(",", type.TypeParameters) + ">";
}
