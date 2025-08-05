using UIL.Diagnostics;

namespace UIL.Syntax;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }
    public TextSpan Span { get; }

    protected SyntaxNode(TextSpan span)
    {
        Span = span;
    }
}
