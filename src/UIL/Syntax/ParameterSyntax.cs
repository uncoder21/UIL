using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class ParameterSyntax : SyntaxNode
{
    public SyntaxToken Type { get; }
    public SyntaxToken Identifier { get; }

    public ParameterSyntax(SyntaxToken type, SyntaxToken identifier)
        : base(new TextSpan(type.Span.Start, identifier.Span.End - type.Span.Start))
    {
        Type = type;
        Identifier = identifier;
    }

    public override SyntaxKind Kind => SyntaxKind.Parameter;
}
