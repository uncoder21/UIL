using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class MethodDeclarationSyntax : MemberDeclarationSyntax
{
    public SyntaxToken Type { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken OpenParenToken { get; }
    public IReadOnlyList<ParameterSyntax> Parameters { get; }
    public SyntaxToken CloseParenToken { get; }
    public BlockStatementSyntax Body { get; }

    public MethodDeclarationSyntax(
        IReadOnlyList<AnnotationSyntax> annotations,
        SyntaxToken type,
        SyntaxToken identifier,
        SyntaxToken openParen,
        IReadOnlyList<ParameterSyntax> parameters,
        SyntaxToken closeParen,
        BlockStatementSyntax body)
        : base(new TextSpan(type.Span.Start, body.Span.End - type.Span.Start), annotations)
    {
        Type = type;
        Identifier = identifier;
        OpenParenToken = openParen;
        Parameters = parameters;
        CloseParenToken = closeParen;
        Body = body;
    }

    public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;
}
