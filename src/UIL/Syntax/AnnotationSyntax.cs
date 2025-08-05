using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class AnnotationSyntax : SyntaxNode
{
    public SyntaxToken OpenBracketToken { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken CloseBracketToken { get; }

    public AnnotationSyntax(SyntaxToken openBracketToken, SyntaxToken identifier, SyntaxToken closeBracketToken)
        : base(new TextSpan(openBracketToken.Span.Start, closeBracketToken.Span.End - openBracketToken.Span.Start))
    {
        OpenBracketToken = openBracketToken;
        Identifier = identifier;
        CloseBracketToken = closeBracketToken;
    }

    public override SyntaxKind Kind => SyntaxKind.Annotation;
}

