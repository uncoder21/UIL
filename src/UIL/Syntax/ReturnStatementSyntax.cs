using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class ReturnStatementSyntax : StatementSyntax
{
    public SyntaxToken ReturnKeyword { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken SemicolonToken { get; }

    public ReturnStatementSyntax(SyntaxToken returnKeyword, ExpressionSyntax expression, SyntaxToken semicolon)
        : base(new TextSpan(returnKeyword.Span.Start, semicolon.Span.End - returnKeyword.Span.Start))
    {
        ReturnKeyword = returnKeyword;
        Expression = expression;
        SemicolonToken = semicolon;
    }

    public override SyntaxKind Kind => SyntaxKind.ReturnStatement;
}
