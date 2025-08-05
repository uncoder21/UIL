using UIL.Diagnostics;

namespace UIL.Syntax;

public abstract class ExpressionSyntax : SyntaxNode
{
    protected ExpressionSyntax(TextSpan span) : base(span) { }
}
