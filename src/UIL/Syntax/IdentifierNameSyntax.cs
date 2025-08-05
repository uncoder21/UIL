using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class IdentifierNameSyntax : ExpressionSyntax
{
    public SyntaxToken Identifier { get; }

    public IdentifierNameSyntax(SyntaxToken identifier)
        : base(identifier.Span)
    {
        Identifier = identifier;
    }

    public override SyntaxKind Kind => SyntaxKind.IdentifierName;
}
