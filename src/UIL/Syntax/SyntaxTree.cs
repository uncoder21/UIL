using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class SyntaxTree
{
    public CompilationUnitSyntax Root { get; }
    public DiagnosticBag Diagnostics { get; }

    private SyntaxTree(string text)
    {
        var parser = new Parser(text);
        Root = parser.ParseCompilationUnit();
        Diagnostics = parser.Diagnostics;
    }

    public static SyntaxTree Parse(string text) => new SyntaxTree(text);
}
