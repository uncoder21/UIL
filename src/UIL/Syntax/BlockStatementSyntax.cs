using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class BlockStatementSyntax : StatementSyntax
{
    public SyntaxToken OpenBraceToken { get; }
    public IReadOnlyList<StatementSyntax> Statements { get; }
    public SyntaxToken CloseBraceToken { get; }

    public BlockStatementSyntax(SyntaxToken openBrace, IReadOnlyList<StatementSyntax> statements, SyntaxToken closeBrace)
        : base(new TextSpan(openBrace.Span.Start, closeBrace.Span.End - openBrace.Span.Start))
    {
        OpenBraceToken = openBrace;
        Statements = statements;
        CloseBraceToken = closeBrace;
    }

    public override SyntaxKind Kind => SyntaxKind.BlockStatement;
}
