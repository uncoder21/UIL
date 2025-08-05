using UIL.Diagnostics;

namespace UIL.Syntax;

public abstract class StatementSyntax : SyntaxNode
{
    protected StatementSyntax(TextSpan span) : base(span) { }
}
